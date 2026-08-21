using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDMS;
using TcpLib2;
using System.Collections;
using System.Threading;
using SOP;

namespace SDMSServer
{
	public class ClientDataSDMS : ClientData
	{
        private int m_nSiteID = 1;
		public ClientDataSDMS(ServiceProvider provider)
		{
            m_nSiteID = NetworkServer.Instance.SiteID;

			m_provider = provider;
			Type = ClientType.SDMS_CLIENT;
		}

		// OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
		protected override bool ProcessFirstConnection(ConnectionState state)
		{
			// 현재 수신반 상태를 전송한다.
			//SendAllReciverState(state);

			// 현재 진행중인 화재들에 대한 마지막 Log List를 전송한다.
			return SendSensorReactionLogList(state);
		}


		public bool SendAllReciverState(ConnectionState state)
		{
			ArrayList arReciverList = ReciverManager.Instance.GetReciverList();
			if (arReciverList == null)
				return false;

			int nDataCount = arReciverList.Count * 2;
			int nSize = 6 + (nDataCount * 9);
			byte[] bytes = new byte[nSize];

			byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.ALL_RECIVER_STATE);
			bytes[0] = byteHeader[0];
			bytes[1] = byteHeader[1];

			// SET DATA COUNT
			byte[] nCount = BitConverter.GetBytes(nDataCount);
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			int nIndex = 6;

			if (arReciverList != null)
			{
				foreach (Reciver reciver in arReciverList)
				{
					byte[] nReciverIDBytes = ServiceProvider.MakeBytes(reciver.ID);
					byte[] nConnectedBytes = ServiceProvider.MakeBytes(reciver.State);

					SensorReactionLog.CopyBytes(bytes, ref nIndex, nReciverIDBytes);
					SensorReactionLog.CopyBytes(bytes, ref nIndex, nConnectedBytes);
				}
			}

            try
            {
                return  m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendAllReciverState", ex); 
            }
            return false;	
		}

		// SensorReactionLog가 하나도 없으면 2바이트만 전송된다.
		// 이를 받은 Client는 모든 화재 상황이 해제된다.
		public bool SendSensorReactionLogList(ConnectionState state)
		{
			ArrayList arrLogBytes = new ArrayList();
			int nByteCount = 0;

			int nHistoryCount = m_provider.GetTimeHistoryCount();

			for (int i = 0; i < nHistoryCount; i++)
			{
				TimeHistory history = m_provider.GetTimeHistory(i);

				if (history.LastReactionLog == null)
					continue;

				byte[] dataBytes = history.LastReactionLog.MakeBytes();
				arrLogBytes.Add(dataBytes);

				nByteCount += dataBytes.Length - 6;
			}

			byte[] bytes = new byte[nByteCount + 6];

			bytes[0] = TCP_ID.SENSOR_REACTION_HISTORY_DATA_LIST;
			bytes[1] = 0;

			int nLogCount = (int)arrLogBytes.Count;
			byte[] chunkBytes = BitConverter.GetBytes(nLogCount * 10);
			int nIndex = 6;

			System.Buffer.BlockCopy(chunkBytes, 0, bytes, 2, 4);

			for (int i = 0; i < nLogCount; i++)
			{
				byte[] dataBytes = (byte[])arrLogBytes[i];
				int nDataLength = dataBytes.Length - 6;

				// dataBytes가 헤더 정보를 포함하여 있어 이를 제외 하기 위해 시작을 6번째 부터 한다.
				System.Buffer.BlockCopy(dataBytes, 6, bytes, nIndex, nDataLength);
				nIndex += nDataLength;
			}

            try
            {
                return m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendSensorRecationLogList", ex); 
            }
            return false;
		}

        // nSituationType : 0(화재탐지), 1(화재신고)
        private static bool GetBroadcastMessage(SensorReactionLog log, BroadcastManager.SituationType type, out string szBroadcastMessage, out int nRepeat, out bool bSiren)
		{
			szBroadcastMessage = "";
			bSiren = false;
			nRepeat = 1;
			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            // 화재 신고시 방송
			//string strSQL = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = " + ((int)type).ToString();
            string szText = "select id, UseBroadcast, Message, UseSiren, RepeatCount from SDMSBroadcastConfig where SituationType = {0} and SiteID = {1}";
            string strSQL = string.Format(szText, ((int)type), SDMSServer.NetworkServer.Instance.SiteID);
            
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
				//nRepeat = 1;

				int nEquipZoneID = -1;
				int.TryParse(log.Param1, out nEquipZoneID);

                string strFireZoneName = "";

                if (nEquipZoneID != -1)
				{
					EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);

					if (equipZone != null)
						strFireZoneName = equipZone.BroadcastName;
				}

                szBroadcastMessage = GetBroadcastMessage(strMessage, strFireZoneName, nRepeatCount);

			}
			return true;
		}

        // strBeginTag와 strEndTag로 둘러쌓인 부분을 제거한 문자열을 리턴한다.
        // strFullMessage : strBeginTag와 strEndTag를 포함한 문자열
        private static string GetMessage(string strOriginMessage, string strBeginTag, string strEndTag, out string strFullMessage)
        {
            int nLen = strOriginMessage.Length;
            int nIndex = 0;

            string strMessage = "";
            strFullMessage = "";
            int nBeginTagLength = strBeginTag.Length;
            int nEndTagLength = strEndTag.Length;

            while (nIndex < nLen)
            {
                int nIndex1 = strOriginMessage.IndexOf(strBeginTag, nIndex);

                if (nIndex1 < 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex);
                    strMessage += strOriginMessage.Substring(nIndex);
                    break;
                }

                int len = nIndex1 - nIndex;

                if (len > 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex, len);
                    strMessage += strOriginMessage.Substring(nIndex, len);
                }

                int nIndex2 = strOriginMessage.IndexOf(strEndTag, nIndex1 + nBeginTagLength);

                if (nIndex2 < 0)
                {
                    strFullMessage += strOriginMessage.Substring(nIndex);
                    strMessage += strOriginMessage.Substring(nIndex1);
                    break;
                }

                len = nIndex2 - (nIndex1 + nBeginTagLength);

                if (len > 0)
                    strFullMessage += strOriginMessage.Substring(nIndex1 + nBeginTagLength, len);

                nIndex = nIndex2 + nEndTagLength;
            }

            return strMessage;
        }

        private static string GetBroadcastMessage(string strOriginMessage, string strFireZoneName, int nRepeatCount)
        {
            string szBroadcastMessage;
            string strRepeatMessage = GetMessage(strOriginMessage, "<<", ">>", out szBroadcastMessage);

            for (int j = 0; j < nRepeatCount; j++)
            {
                szBroadcastMessage += "...\n다시한번 알려드립니다...";
                szBroadcastMessage += strRepeatMessage;
            }

            szBroadcastMessage = szBroadcastMessage.Replace("●", strFireZoneName);
            return szBroadcastMessage;
        }

		public static void RunBroadcast(SensorReactionLog log, ServiceProvider provider, BroadcastManager.SituationType type)
		{
			// 화재 발생 방송
			string szBroadcastMsg = "";
			int nRepeat = 1;
			bool bUseSiren = false;
            
			if (BroadcastManager.Instance.IsEnabled(type) == true)
			{
                bool bResult = GetBroadcastMessage(log, type, out szBroadcastMsg, out nRepeat, out bUseSiren);

                if (bResult)
                {
                    SensorReactionLog smsLog = new SensorReactionLog();
                    smsLog.Message = "사내 방송 실시";
                    smsLog.Param1 = log.Param1;
                    smsLog.Param2 = log.Param2;
                    smsLog.Param3 = log.Param3;
                    smsLog.SensorHistoryID = log.SensorHistoryID;
                    smsLog.Type = SensorReactionLog.ReactionType.RUN_BROADCAST;
                    provider.AddReactionLog(smsLog);

                    BroadcastManager.Instance.AddSpeech(szBroadcastMsg, nRepeat, bUseSiren, type);
                }
			}
		}
		// bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
		{
			if (nHeader == TCP_ID.FIRE_DETECT_REPORT)
			{
                int nSOPGenUserID;

				// SensorZone이 아닌 개별 Sensor ID
				SensorReactionLog log = ReadFireReport(bytes, out nSOPGenUserID);

                // nSOPGenUserID에 해당하는 SOP Simulator가 제어권을 가지도록 한다.
                NoControlSimulator(nSOPGenUserID);

				if (m_provider.CheckSituation(log.SensorHistoryID))
				{
					m_provider.AddReactionLog(log);

					// 사내방송 실시 - 화재보고에서는 방송내보내지 않는다.(아래로바뀜 20130-12-18)
					// 사내방송 실시, 화재탐지의 방송을 중단하고  다시 화재신고시에 방송을보냄으로 변경'
					// 삼천포 김명수대리요청
					RunBroadcast(log, m_provider, BroadcastManager.SituationType.REPORT_FIRE);

					// SMS전송
					//m_provider.SendSMS(log);
					m_provider.SendSMSToAllCompanyMember(log);

					m_provider.MonitorNotifyFireProcess(log);
					// Send Reaction Log
					m_provider.SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT);
					m_provider.SendFireSensorSignal(log);
				}
				else
				{
					// 수동 신고의 처리
					if (log.Param2 == "0")
					{
						ProcessManualFireDetect(log);
					}
					
				}
			}
			else if (nHeader == TCP_ID.MALFUNCTION_REPORT)
			{
				ProcessMalfunction(bytes);
			}
            else if (nHeader == TCP_ID.CHANGE_CONFIG)
            {
                ProcessChangedConfig(arrDatas, bytes);
            }
            else if (nHeader == TCP_ID.CLEAR_DETECT_REPORT)
            {
                ProcessDetectReportClear(bytes);
            }
            else if (nHeader == TCP_ID.REQUEST_RESTORE)
            {

                bool bExist = m_provider.ExistFireDetectSituation();
                if (bExist == true)
                {
                    byte[] sendbytes = new byte[6] { TCP_ID.REJECT_RESTORE, 0, 0, 0, 0, 0 };
                    try
                    {
                        m_provider.Send(sendbytes, 0, sendbytes.Length, state);
                    }
                    catch (System.Exception ex)
                    {
                        ConnectionLogEx.Instance.WriteLine("RejectResotre", ex); 
                    }
                    
                }
                else
                {
                    byte[] sendbytes = new byte[6] { TCP_ID.ACCEPT_RESTORE, 0, 0, 0, 0, 0 };
                    try
                    {
                        m_provider.Send(sendbytes, 0, sendbytes.Length, state);
                    }
                    catch (System.Exception ex)
                    {
                        ConnectionLogEx.Instance.WriteLine("AcceptRestore", ex); 
                    }
                    

                    m_provider.SendBeginRestore();
                }

            }
            else if (nHeader == TCP_ID.REQUEST_SENSOR_REACTION_HISTORY_DATA_LIST)
            {
                SendSensorReactionLogList(state);
            }

			return true;
		}

        private void ProcessChangedConfig(ArrayList arrDatas, byte[] bytes)
        {
            if (arrDatas == null)
                return;

            if (arrDatas.Count < 3)
                return;

            try
            {
                byte byteClientType = (byte)arrDatas[0];
                string strPropertyName = (string)arrDatas[1];
                string strPropertyValue = (string)arrDatas[2];

                if (byteClientType != TCP_CLIENT.SDMS_CLIENT)
                    return;

                if (strPropertyName == SDMSConfig.PropertyName)
                {
                    int nConfigValue;

                    if (int.TryParse(strPropertyValue, out nConfigValue))
                    {
                        if (((nConfigValue & (int)SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER) == (int)SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER) ||
                            ((nConfigValue & (int)SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER) == (int)SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER) ||
                            ((nConfigValue & (int)SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER) == (int)SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER))
                            ProcessChangeFacilityManager(bytes);
                    }
                }
                else if (strPropertyName == SDMSConfig.GetPropertyName(SDMSConfig.ConfigType.EQUIPZONE_CCTV))
                {
                    int nEquipZoneID;

                    if (int.TryParse(strPropertyValue, out nEquipZoneID))
                    {
                        ProcessChangeEquipZoneCCTV(bytes);
                    }
                }
            }
            catch (Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("ProcessChangedConfig", ex);                 
            }
        }

        // nSOPGenUserID를 가진 SOP Simulator가 제어권을 가져갈 때까지 10초간 기다린다.
        // 그동안은 아무도 제어권을 가지지 않는 상태가 된다.
        private void NoControlSimulator(int nSOPGenUserID)
        {
            Thread t = new Thread(new ParameterizedThreadStart(NoControlThread));
            t.Start(nSOPGenUserID);
        }

        // 
        private void NoControlThread(object param)
        {
            int nSOPGenUserID = (int)param;

            ControlMonitoring.ControlManager.Instance.ControlClient = null;
            ControlMonitoring.ControlManager.Instance.ControlSOPGenUserID = nSOPGenUserID;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSOPGenUserID);
            byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.GIVE_CONTROL_KEY, arrDatas);

            for (int i = 0; i < 15; i++)
            {
                m_provider.SendData(bytes, false, ClientType.SOP_SIMULATOR);
                Thread.Sleep(1000);

                // 제어권 소유자가 생겼다.
                if (ControlMonitoring.ControlManager.Instance.ControlSOPGenUserID < 0)
                {
                    return;
                }
            }

            // 제어권 소유자가 아무도 없으므로 접속한 Client 가운데 첫번째 Client에게 제어권을 넘긴다.
            ControlMonitoring.ControlManager.Instance.ControlSOPGenUserID = -1;

            byte[] bytes2 = ServiceProvider.MakeBytes(TCP_ID.GIVE_CONTROL, null);
            m_provider.SendData(bytes2, false, ClientType.SOP_SIMULATOR, 1);
        }

        private void ProcessDetectReportClear(byte[] bytes)
		{
			int nPrevSensorHistoryID = BitConverter.ToInt32(bytes, 11);
			int nGenUserID = BitConverter.ToInt32(bytes, 20);

			if (nPrevSensorHistoryID > 0)
			{
				TimeHistory history = m_provider.FindTimeHistory(nPrevSensorHistoryID);

				if (history != null && history.LastReactionLog != null/* && history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS*/)
				{
					ClientData.ClientType nClientType = ClientData.ClientType.SDMS_CLIENT;
					
					PingCount = 0;
					// 화재 상황 종료

                    
						m_provider.SendClearDetectReport(nPrevSensorHistoryID, nClientType);
					
					
					m_provider.RemoveTimeHistory(history);
					SensorManager.Instance.RemoveSensorHistory(nPrevSensorHistoryID);

					if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.NOTIFY_FIRE)
					{
						SensorReactionLog log = new SensorReactionLog();
						log.Param3 = nGenUserID.ToString();
						log.LogTime = DateTime.Now;
						log.Message = "화재 신호가 무시되었습니다.";
						log.SensorHistoryID = nPrevSensorHistoryID;
						log.Type = SensorReactionLog.ReactionType.IGNORE_FIRE;

						m_provider.AddReactionLog(log);
					}
				}
			}
		}

		private void ProcessChangeEquipZoneCCTV(byte[] bytes)
		{
			//int nEquipZoneID = ReadChangeEquipZoneCCTV(bytes);	


			m_provider.SendDataToOther(bytes,this, false, ClientData.ClientType.SDMS_CLIENT);
		}

		private void ProcessChangeFacilityManager(byte[] bytes)
		{
			m_provider.SendDataToOther(bytes, this, false, ClientData.ClientType.SDMS_CLIENT);

			DataManager.Instance.LoadFacilityManager();
		}

		private void ProcessMalfunction(byte[] bytes)
		{
			SensorReactionLog log = ReadMalfunctionReport(bytes);

			if (m_provider.CheckSituation(log.SensorHistoryID))
			{
				NetworkServer.Instance.SensorManager.SetLastReadSensorHistoryID(log.SensorHistoryID);
				int nSensorID = NetworkServer.Instance.SensorManager.GetSensorID(log.SensorHistoryID);

				m_provider.AddReactionLog(log);
				m_provider.SendSMS(log);

				m_provider.RemoveSituation(log.SensorHistoryID);
				if (nSensorID > 0)
				{
					SensorZone sensor = NetworkServer.Instance.IOManager.GetSensorZone(nSensorID);

					if (sensor != null && sensor.SensorData == 1)
					{
						// 무시할 센서 리스트에 포함
						m_provider.AddTempIgnoreSensor(sensor);

						AbnormalSensorManager.Instance.Add(sensor.ID);
					}
				}

				// Send Reaction Log
				m_provider.SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT);
			}
		}

		public int ReadChangeEquipZoneCCTV(byte[] bytes)
		{
			int chunkSize = BitConverter.ToInt32(bytes, 2);
			int nReadDataCount = 6;

			int nEquipZoneID = -1;
			byte dataHeader = bytes[nReadDataCount++];
			int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
			nReadDataCount += 4;
			if (dataHeader == TCP_TYPE.INTEGER)
			{
				nEquipZoneID = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += nDataLength;
			}
			return nEquipZoneID;
		}

		public SensorReactionLog ReadMalfunctionReport(byte[] bytes)
		{
			SensorReactionLog log = new SensorReactionLog();

			//int nReadDataCount = 1;
			//int chunkSize = (int)bytes[nReadDataCount++];
			int chunkSize = BitConverter.ToInt32(bytes, 2);
			int nReadDataCount = 6;

			int nSensorHistoryID = -1;
			byte dataHeader = bytes[nReadDataCount++];
			int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
			nReadDataCount += 4;
			if (dataHeader == TCP_TYPE.INTEGER)
			{
				nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += nDataLength;
			}
			chunkSize -= 1;
			log.SensorHistoryID = nSensorHistoryID;

			int nSensorID = -1;
			if (chunkSize > 0)
			{
				dataHeader = bytes[nReadDataCount++];
				nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += 4;
				if (dataHeader == TCP_TYPE.INTEGER)
				{
					nSensorID = BitConverter.ToInt32(bytes, nReadDataCount);
					nReadDataCount += nDataLength;
				}
			}
			chunkSize -= 1;
			//ResetSensorData(nSensorID);
			log.Param2 = nSensorID.ToString();

			int nSOPGenUser = -1;
			if (chunkSize > 0)
			{
				dataHeader = bytes[nReadDataCount++];
				nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += 4;
				if (dataHeader == TCP_TYPE.INTEGER)
				{
					nSOPGenUser = BitConverter.ToInt32(bytes, nReadDataCount);
					nReadDataCount += nDataLength;
				}
			}
			if (nSOPGenUser != -1)
				log.Param3 = nSOPGenUser.ToString();

			int nEquipZoneID = SensorManager.Instance.GetSensorZone(nSensorID);
			if (nEquipZoneID == -1)
			{
				log.Message = "탐지된 화재가 오작동으로 신고되었습니다";
			}
			else
			{
				EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
				if (equipZone != null)
				{
					string szZoneName = equipZone.BroadcastName;
					log.Message = string.Format("[{0}]에서 탐지된 화재가 오작동으로 신고되었습니다", szZoneName);
				}
				log.Param1 = nEquipZoneID.ToString();
			}

			log.Type = SensorReactionLog.ReactionType.MALFUNCTION;
			log.SensorHistoryID = nSensorHistoryID;

			return log;
		}

		private void ProcessManualFireDetect(SensorReactionLog log)
		{
			int nZoneID = -1;
			int.TryParse(log.Param1, out nZoneID);
			int nPrevHistoryID = -1;
			if (SensorManager.Instance.GetSensorHistoryIDForManual(nZoneID, ref  nPrevHistoryID) != -1)
			{
				return;
			}

			string sqlID = "select max(id) as id from SensorZoneHistory";
			ArrayList arrResult = NetworkServer.Instance.DBManager.GetResultData(sqlID, 0);
			int nResultCount = arrResult.Count;

			int nHistoryID = 0;
			for (int i = 0; i < nResultCount; i += 1)
			{
				//Data가 아예 안들어가 있을경우 0부터 시작
				int Find_Maxid = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				nHistoryID = Find_Maxid;
			}
			nHistoryID++;

			DateTime dtNow = DateTime.Now;
			string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);

			//History
			//string sqlInsert = "insert into SensorZoneHistory(ID, SensorID,Connected,Data,Time) Values('"
			//	+ nHistoryID + "','" + 0 + "','" + 1 + "','" + 1 + "','" + strDateTimeField + "')";

			string sqlInsert = "insert into SensorZoneHistory(ID, SensorID,Connected,Data,Time, param1, SiteID ) Values('"
				+ nHistoryID + "','" + 0 + "','" + 1 + "','" + 1 + "','" + strDateTimeField + "','" + log.Param1 + "','" + m_nSiteID +"')";
			NetworkServer.Instance.DBManager.GetResultData(sqlInsert, 0);

			NetworkServer.Instance.SensorManager.DicSensorHistory[nHistoryID] = 0;
			
			log.SensorHistoryID = nHistoryID;

			TimeHistory hs = new TimeHistory(nHistoryID, DateTime.Now);
			m_provider.AddTimeHistory(hs);

			m_provider.AddReactionLog(log);

            RunBroadcast(log, m_provider, BroadcastManager.SituationType.REPORT_FIRE);
			// SMS전송
			//m_provider.SendSMS(log);
			m_provider.SendSMSToAllCompanyMember(log);

			m_provider.MonitorNotifyFireProcess(log);
			m_provider.SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT);

			
			byte[] sensorIDBytes = ServiceProvider.MakeBytes(0);
			byte[] sensorHistoryIDBytes = ServiceProvider.MakeBytes(log.SensorHistoryID);
			byte[] zoneIDBytes = ServiceProvider.MakeBytes(nZoneID);
			byte[] timeBytes = ServiceProvider.MakeBytes(log.LogTime.ToBinary());
			float x = 0.0f;
			float y = 0.0f;
			float z = 0.0f;
			byte[] xBytes = ServiceProvider.MakeBytes(x);
			byte[] yBytes = ServiceProvider.MakeBytes(y);
			byte[] zBytes = ServiceProvider.MakeBytes(z);

			bool bReal = !DataManager.GetTranningMode();
			byte[] realByte = ServiceProvider.MakeBytes(bReal == true ? 0 : 1);

			int nBlockLength = sensorIDBytes.Length + sensorHistoryIDBytes.Length + zoneIDBytes.Length + timeBytes.Length + xBytes.Length + yBytes.Length + zBytes.Length + realByte.Length;
			byte[] bytes = new byte[6 + nBlockLength];

			bytes[0] = TCP_ID.FIRE_SENSOR_SIGNAL;
			bytes[1] = 0;

			int nChunkCount = 8;
			byte[] chunkBytes = BitConverter.GetBytes(nChunkCount);
			System.Buffer.BlockCopy(chunkBytes, 0, bytes, 2, 4);

			int nIndex = 6;
			SensorReactionLog.CopyBytes(bytes, ref nIndex, sensorIDBytes);
			SensorReactionLog.CopyBytes(bytes, ref nIndex, sensorHistoryIDBytes);
			SensorReactionLog.CopyBytes(bytes, ref nIndex, zoneIDBytes);
			SensorReactionLog.CopyBytes(bytes, ref nIndex, timeBytes);
			SensorReactionLog.CopyBytes(bytes, ref nIndex, xBytes);
			SensorReactionLog.CopyBytes(bytes, ref nIndex, yBytes);
			SensorReactionLog.CopyBytes(bytes, ref nIndex, zBytes);
			SensorReactionLog.CopyBytes(bytes, ref nIndex, realByte);

			ServiceProvider.SendData(bytes, true, ClientData.ClientType.SOP_SIMULATOR);
		}

		private SensorReactionLog ReadFireReport(byte[] bytes, out int nSOPGenUserID)
		{
			SensorReactionLog log = new SensorReactionLog();

			//int nReadDataCount = 1;
			//int chunkSize = (int)bytes[nReadDataCount++];
			int chunkSize = BitConverter.ToInt32(bytes, 2);
			int nReadDataCount = 6;

			int nSensorHistoryID = -1;
			byte dataHeader = bytes[nReadDataCount++];
			int nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
			nReadDataCount += 4;

			if (dataHeader == TCP_TYPE.INTEGER)
			{
				nSensorHistoryID = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += nDataLength;
			}

			chunkSize -= 1;
			log.SensorHistoryID = nSensorHistoryID;

			int nEquipZoneID = -1;
			dataHeader = bytes[nReadDataCount++];
			nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
			nReadDataCount += 4;

			if (dataHeader == TCP_TYPE.INTEGER)
			{
				nEquipZoneID = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += nDataLength;
			}
			chunkSize -= 1;

			log.Param1 = nEquipZoneID.ToString();

			int nSensorID = -1;
			if (chunkSize > 0)
			{
				dataHeader = bytes[nReadDataCount++];
				nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += 4;
				if (dataHeader == TCP_TYPE.INTEGER)
				{
					nSensorID = BitConverter.ToInt32(bytes, nReadDataCount);
					nReadDataCount += nDataLength;
				}
			}
			chunkSize -= 1;
			log.Param2 = nSensorID.ToString();

			nSOPGenUserID = -1;
			if (chunkSize > 0)
			{
				dataHeader = bytes[nReadDataCount++];
				nDataLength = BitConverter.ToInt32(bytes, nReadDataCount);
				nReadDataCount += 4;
				if (dataHeader == TCP_TYPE.INTEGER)
				{
					nSOPGenUserID = BitConverter.ToInt32(bytes, nReadDataCount);
					nReadDataCount += nDataLength;
				}
			}

			if (nSOPGenUserID != -1)
			{
				log.Param3 = nSOPGenUserID.ToString();
			}

            GetFireReportString(nEquipZoneID, log);			
			log.Type = SensorReactionLog.ReactionType.NOTIFY_FIRE;
			return log;
		}

        public static string GetFireReportString(int nEquipZoneID, SensorReactionLog log)
        {
            string strMessage = "";

            if (nEquipZoneID == -1)
            {
                strMessage = "화재발생이 신고되었습니다";
            }
            else
            {
                if (log != null && log.Param2 == "0")
                {
                    Zone equipZone = ZoneManager.Instance.GetZone(nEquipZoneID);
                    if (equipZone != null)
                    {
                        string szZoneName = equipZone.BroadcastName;
                        strMessage = string.Format("[{0}]에서 화재발생이 신고되었습니다", szZoneName);
                    }
                }
                else
                {
                    EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
                    if (equipZone != null)
                    {
                        string szZoneName = equipZone.BroadcastName;
                        strMessage = string.Format("[{0}]에서 화재발생이 신고되었습니다", szZoneName);
                    }
                }

                if (log != null)
                    log.Param1 = nEquipZoneID.ToString();
            }

            if (log != null)
                log.Message = strMessage;

            return strMessage;
        }
	}
}
