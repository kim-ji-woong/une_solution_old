using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Threading;

namespace SDMSServer
{
    public class ClientDataSOPMonitor : ClientData
    {
        public ClientDataSOPMonitor(ServiceProvider provider)
        {
            m_provider = provider;
            Type = ClientType.SOP_MONITOR;

			ReciverManager.Instance.LoadReciverList();
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
			PingCount = 0;
			if (nHeader == TCP_ID.SENSOR_DATA)
			{
				// 센서 데이터에 대해 반드시 한번에 한개만 전송해야 하므로 lock 이 필요함
				// skkim 2014-01-09
				lock (m_provider)
				{
					int nSensorID, data, nPrevSensorHistoryID = -1;
					bool connected = false;
					int nHistoryID = NetworkServer.Instance.SensorManager.ProcessSensorData(bytes, out nSensorID, out data, out connected, ref nPrevSensorHistoryID);
					PostProcessSensorData(nHistoryID, nPrevSensorHistoryID, nSensorID, data, connected);
				}				
			}
			else if (nHeader == TCP_ID.RECIVER_CONNECT)
			{
				ProcessReciverConnect(bytes);
			}
			else if (nHeader == TCP_ID.RECIVER_DISCONNECT)
			{
				lock (m_provider)
				{
					ProcessReciverDisconnect(bytes);
				}
			}
			else if (nHeader == TCP_ID.ALL_RECIVER_STATE)
			{
				ProcessAllReciverState(bytes);
			}
			return true;
		}

        private int GetReciverID(byte[] bytes, int nIdx = 0)
        {
			int nData = BitConverter.ToInt32(bytes, nIdx);
			return nData;
        }

		private bool IsReciverConnected(byte[] bytes, int nIdx = 0)
		{
			int nData = BitConverter.ToInt32(bytes, nIdx);
			if (nData == 1)
				return true;
			
			return false;
		}

        private void ProcessAllReciverState(byte[] bytes)
        {
            int nReciverID = -1;
            bool bConnected = false;

			int nDataCount = BitConverter.ToInt32(bytes, 2);

			int nReadData = 11;
			for (int i = 0; i < nDataCount; i += 2)
			{
				nReciverID = GetReciverID(bytes, nReadData);				
				nReadData += 9;

				bConnected = IsReciverConnected(bytes, nReadData);
				nReadData += 9;

				if (ReciverManager.Instance.DicReciverList.ContainsKey(nReciverID))
				{
					Reciver reciver = ReciverManager.Instance.DicReciverList[nReciverID];
					if (reciver.State != (bConnected == true ? 1 : 0))
					{
						SendReciverState(nReciverID, bConnected);
						PingCount = 0;
						Thread.Sleep(10);
					}
					reciver.State = (bConnected == true ? 1 : 0);
				}
			}            
        }

		private void SendReciverState(int nReciverID , bool bConnected)
		{
			ReciverManager.Instance.UpdateState(nReciverID, bConnected);


			ClientData.ClientType nClientType = (bClient == true ? ClientData.ClientType.SDMS_CLIENT : ClientData.ClientType.SDMS_CLIENT_SECOND);
			bClient = !bClient;

			m_provider.SendReciverState(nReciverID, bConnected, nClientType);
		}


        private void ProcessReciverDisconnect(byte[] bytes)
        {
			this.PingCount = 0;
			int nReciverID = GetReciverID(bytes, 11);
			SendReciverState(nReciverID, false);
        }

        private void ProcessReciverConnect(byte[] bytes)
        {
			this.PingCount = 0;
			int nReciverID = GetReciverID(bytes, 11);

			SendReciverState(nReciverID, true);
        }
		static bool bClient = true;
		private void PostProcessSensorData(int nHistoryID, int nPrevSensorHistoryID, int nSensorID, int nData, bool bConnected)
		{
			// comment by skkim : AbnormalSensorManager에서 대행
			// 임시로 무시된 Sensor List에서 해제할 것이 있는지 검사
			if (nSensorID > 0 && nData == 0)
			{
				m_provider.RemoveTempIgnoreSensor(nSensorID);
			}

			// Connection만 변경되는 경우 리턴값이 -2임
			if (nHistoryID == -2)
			{
				m_provider.SendSensorZoneData(nData, nSensorID, ClientData.ClientType.SDMS_CLIENT);
			}

			if (nData == 1 && nHistoryID != -1)
			{
				if (!m_provider.CheckSituation(nHistoryID))
				{
					TimeHistory hs = new TimeHistory(nHistoryID, DateTime.Now);
					m_provider.AddTimeHistory(hs);
					PingCount = 0;
					m_provider.SendSensorZoneData(nData, nSensorID, ClientData.ClientType.SDMS_CLIENT);
					PingCount = 0;
					SensorReactionLog log = CreateFireDetect(nHistoryID, nSensorID);
					m_provider.AddReactionLog(log);

					// 사내방송 실시 - 신호에 대해 현장확인 후 방송 보내도록 함(삼천포:김명수대리요청)
                    // 2013-12-18
					RunBroadcast(log);
					m_provider.SendSMS(log);

					// Send Reaction Log
					m_provider.SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT_SECOND);

					hs.LastReactionLog = log;
					// Send History ID
					//SendSensorHistoryID(nHistoryID);

					m_provider.MonitorDetectFireProcess(log);
				}
			}
			else if (nData == 0 && nHistoryID != -1)
			{
				//int nPrevSensorHistoryID = SensorManager.Instance.GetSensorHistoryID(nSensorID, true, 1);

				if (nPrevSensorHistoryID > 0)
				{
					TimeHistory history = m_provider.FindTimeHistory(nPrevSensorHistoryID);

					if (history != null && history.LastReactionLog != null/* && history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS*/)
					{
						ClientData.ClientType nClientType = (bClient == true ? ClientData.ClientType.SDMS_CLIENT : ClientData.ClientType.SDMS_CLIENT_SECOND);
						bClient = !bClient;
						Thread.Sleep(5);
						PingCount = 0;
						lock (m_provider)
						{
							m_provider.SendSensorZoneData(nData, nSensorID, nClientType);
						}
						
						PingCount = 0;
						// 화재 상황 종료
						Thread.Sleep(5);
						nClientType = (bClient == true ? ClientData.ClientType.SDMS_CLIENT : ClientData.ClientType.SDMS_CLIENT_SECOND);
						bClient = !bClient;

						lock (m_provider)
						{
							m_provider.SendClearDetectReport(nPrevSensorHistoryID, nClientType);
						}

						Thread.Sleep(5);
						m_provider.RemoveTimeHistory(history);
						m_provider.RemoveSituation(nHistoryID, false);

						SensorManager.Instance.RemoveSensorHistory(nPrevSensorHistoryID);
						SensorManager.Instance.RemoveSensorHistory(nHistoryID);

						if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS)
						{
							SensorReactionLog log = new SensorReactionLog();

							log.LogTime = DateTime.Now;
							log.Message = "화재 신호가 무시되었습니다.";
							log.SensorHistoryID = nPrevSensorHistoryID;
							log.Type = SensorReactionLog.ReactionType.IGNORE_FIRE;

							m_provider.AddReactionLog(log);
						}
					}
				}
			}
		}

		private SensorReactionLog CreateFireDetect(int nHistoryID, int nSensorID)
		{
			SensorReactionLog log = new SensorReactionLog();

			log.LogTime = DateTime.Now;
			log.SensorHistoryID = nHistoryID;
			log.Type = SensorReactionLog.ReactionType.BEGIN_STATUS;
			int nEquipZoneID = SensorManager.Instance.GetSensorZone(nSensorID);
			if (nEquipZoneID == -1)
			{
				log.Message = "화재가 탐지 되었습니다";
			}
			else
			{
				EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
				if (equipZone != null)
				{
					string szZoneName = equipZone.BroadcastName;
					log.Message = string.Format("[{0}]에서 화재가 탐지 되었습니다", szZoneName);
				}
				log.Param1 = nEquipZoneID.ToString();
			}

			log.Param2 = nSensorID.ToString();

			return log;
		}

		private void RunBroadcast(SensorReactionLog log)
		{
			// 화재 발생 방송
			string szBroadcastMsg = "";
			int nRepeat = 1;
			bool bUseSiren = false;
			bool bResult = GetBroadcastMessage(log, out szBroadcastMsg, out nRepeat, out bUseSiren);
			if (bResult)
			{
                if (BroadcastManager.Instance.IsEnabled(BroadcastManager.SituationType.DETECT_FIRE) == true)
                {
                    SensorReactionLog smsLog = new SensorReactionLog();
                    smsLog.Message = "사내 방송 실시";
                    smsLog.Param1 = log.Param1;
                    smsLog.Param2 = log.Param2;
                    smsLog.SensorHistoryID = log.SensorHistoryID;
                    smsLog.Type = SensorReactionLog.ReactionType.RUN_BROADCAST;
                    m_provider.AddReactionLog(smsLog);

                    BroadcastManager.Instance.AddSpeech(szBroadcastMsg, nRepeat, bUseSiren, BroadcastManager.SituationType.DETECT_FIRE);
                }				
			}
		}

		private bool GetBroadcastMessage(SensorReactionLog log, out string szBroadcastMessage, out int nRepeat, out bool bSiren)
		{
			szBroadcastMessage = "";
			bSiren = false;
			nRepeat = 1;
			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            // 화재 탐지시 방송
			string strSQL = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = 0";

			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
			if (arrResult == null)
				return false;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 4; i += 5)
			{
				int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				bool useBroadcast = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0) == 0 ? false : true;
				string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
				bool useSiren = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
				int nRepeatCount = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);

				if (useBroadcast == false)
				{
					return false;
				}

				bSiren = useSiren;
				nRepeat = 1;

				int nEquipZoneID = -1;
				int.TryParse(log.Param1, out nEquipZoneID);

				string szOnce = "";
				int nIdx = strMessage.IndexOf("<<");
				int nIdx2 = strMessage.IndexOf(">>");
				if (nIdx != -1 && nIdx2 != -1)
					szOnce = strMessage.Substring(nIdx + 2, (nIdx2 - nIdx) - 2);

				string szMsg = strMessage.Substring(nIdx2 + 2);
				string szMsg1 = "";
				if (nEquipZoneID != -1)
				{
					EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);

					if (equipZone != null)
						szMsg1 = szMsg.Replace("●", equipZone.BroadcastName);
				}
				szBroadcastMessage = szOnce + szMsg1;
				for (int j = 0; j < nRepeatCount; j++)
				{
					szBroadcastMessage += "...";
					szBroadcastMessage += szMsg1;
				}
			}
			return true;
		}
	}
}
