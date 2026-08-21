using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SDMS;
using TcpLib2;
using System.Collections;
using SOP;

namespace SDMSServer
{
	public class ClientDataSDMSSub : ClientData
	{
		public ClientDataSDMSSub(ServiceProvider provider)
		{
			m_provider = provider;
			Type = ClientType.SDMS_CLIENT_SECOND;
		}

		// OnAccept() 이후 WhoIAm을 받은 뒤 처리해야 할 로직
		protected override bool ProcessFirstConnection(ConnectionState state)
		{
			// 현재 수신반 상태를 전송한다.
			SendAllReciverState(state);
			return true;
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
			return m_provider.Send(bytes, 0, bytes.Length, state);
		}

		// bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
		{			
			if (nHeader == TCP_ID.MALFUNCTION_REPORT)
			{
				ProcessMalfunction(bytes);
			}
			/*else if (nHeader == TCP_ID.CHANGE_FACILITY_MANAGER)
			{
				ProcessChangeFacilityManager(bytes);
			}
			else if (nHeader == TCP_ID.CHANGE_EQUIPZONE_CCTV)
			{
				ProcessChangeEquipZoneCCTV(bytes);
			}*/
            else if (nHeader == TCP_ID.CHANGE_CONFIG)
            {
                ProcessChangedConfig(arrDatas, bytes);
            }
			else if (nHeader == TCP_ID.REQUEST_RESTORE)
			{

				bool bExist = m_provider.ExistFireDetectSituation();
				if (bExist == true)
				{
					byte[] sendbytes = new byte[6] { TCP_ID.REJECT_RESTORE, 0, 0, 0, 0, 0 };
					m_provider.Send(sendbytes, 0, sendbytes.Length, state);
				}
				else
				{
					byte[] sendbytes = new byte[6] { TCP_ID.ACCEPT_RESTORE, 0, 0, 0, 0, 0 };
					m_provider.Send(sendbytes, 0, sendbytes.Length, state);

					m_provider.SendBeginRestore();
				}
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
            catch (Exception e)
            {
                TcpLib2.ConnectionLog.Instance.WriteLine(e.StackTrace);
            }
        }

		private void ProcessChangeEquipZoneCCTV(byte[] bytes)
		{
			m_provider.SendDataToOther(bytes, this, false, ClientData.ClientType.SDMS_CLIENT_SECOND);
		}

		private void ProcessChangeFacilityManager(byte[] bytes)
		{
			m_provider.SendDataToOther(bytes, this, false, ClientData.ClientType.SDMS_CLIENT_SECOND);

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
				m_provider.SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT_SECOND);
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
			string sqlInsert = "insert into SensorZoneHistory(ID, SensorID,Connected,Data,Time, param1) Values('"
				+ nHistoryID + "','" + 0 + "','" + 1 + "','" + 1 + "','" + strDateTimeField + "','"+log.Param1+"')";
			NetworkServer.Instance.DBManager.GetResultData(sqlInsert, 0);

			NetworkServer.Instance.SensorManager.DicSensorHistory[nHistoryID] = 0;
			System.Diagnostics.Trace.WriteLine(string.Format("__SensorHistory[{0}] = {1}", nHistoryID, 0));

			log.SensorHistoryID = nHistoryID;

			TimeHistory hs = new TimeHistory(nHistoryID, DateTime.Now);
			m_provider.AddTimeHistory(hs);

			m_provider.AddReactionLog(log);

			m_provider.MonitorNotifyFireProcess(log);
			m_provider.SendSensorReactionLog(log, ClientData.ClientType.SDMS_CLIENT);

			int nZoneID = -1;
			int.TryParse(log.Param1, out nZoneID);
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

		private SensorReactionLog ReadFireReport(byte[] bytes)
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
			log.Param2 = nSensorID.ToString();
			if (nEquipZoneID == -1)
			{
				log.Message = "화재발생이 신고되었습니다";
			}
			else
			{
				if (log.Param2 == "0")
				{
					Zone equipZone = ZoneManager.Instance.GetZone(nEquipZoneID);
					if (equipZone != null)
					{
						string szZoneName = equipZone.BroadcastName;
						log.Message = string.Format("[{0}]에서 화재발생이 신고되었습니다", szZoneName);
					}
					log.Param1 = nEquipZoneID.ToString();
				}
				else
				{
					EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
					if (equipZone != null)
					{
						string szZoneName = equipZone.BroadcastName;
						log.Message = string.Format("[{0}]에서 화재발생이 신고되었습니다", szZoneName);
					}
					log.Param1 = nEquipZoneID.ToString();
				}

			}
			log.Type = SensorReactionLog.ReactionType.NOTIFY_FIRE;

			return log;
		}
	}
}
