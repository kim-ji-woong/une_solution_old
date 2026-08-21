using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;
using System.IO;

namespace SDMSServer
{
	public class SensorManager
	{
		private static SensorManager m_Instance = null;
		public static SensorManager Instance
		{
			get
			{
				return m_Instance;
			}
		}

		private DBUtility.WebDBManager m_dbMgr = null;
        private ServiceProvider m_provider = null;

        private int m_nLastReadReactionHistoryID = -1;
        // Key : SensorHistoryID
        // Value : SensorZone ID
        private Dictionary<int, int> m_dicSensorHistory = new Dictionary<int, int>();

       
		public SensorManager(DBUtility.WebDBManager dbMgr, ServiceProvider provider)
		{
			m_Instance = this;
			m_dbMgr = dbMgr;
            m_provider = provider;
            ReadLastLogID();
		}

        private void ReadLastLogID()
        {
            string strFilePath = System.Windows.Forms.Application.StartupPath + "\\LastSensorHistory.log";
            
            if (File.Exists(strFilePath))
            {
                StreamReader reader = new StreamReader(strFilePath, Encoding.Default);
                string strLine = reader.ReadLine();
                reader.Close();

                string strReactionHistoryID = strLine.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                strReactionHistoryID = strReactionHistoryID.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                int.TryParse(strReactionHistoryID, out m_nLastReadReactionHistoryID);
            }
        }

        public void SetLastReadSensorHistoryID(int nHistoryID)
        {
            string strFilePath = System.Windows.Forms.Application.StartupPath + "\\LastSensorHistory.log";

            StreamWriter writer = new StreamWriter(strFilePath, false, Encoding.Default);
            writer.Write(nHistoryID);
            writer.Close();
        }

		public int ProcessSensorData(byte[] bytesSensorData, out int outSensorID, out int data, out bool bconnected, ref int nPrevSensorHistoryID)
		{
			outSensorID = -1;
			bconnected = true;
			int nSensorType = BitConverter.ToInt32(bytesSensorData, 7);
			int nEquipZoneID = BitConverter.ToInt32(bytesSensorData, 16);
			int nSensorData = BitConverter.ToInt32(bytesSensorData, 25);
						
            data = 0;
            EquipmentZone equipZone = ZoneManager.Instance.GetEquipmentZone(nEquipZoneID);
			if (equipZone == null)
				return -1;		

            SensorZone sensor = null;
			IOManager ioMgr = FormMain.Instance.IOManager;

            if (!ioMgr.D_EquipZoneSensor.ContainsKey(equipZone))
                return -1;

            ArrayList arrSensorZone = ioMgr.D_EquipZoneSensor[equipZone];
			int nSensorID = -1;

			//선택된 센서아이디를 구함
			foreach (SensorZone sensorZone in arrSensorZone)
			{
				if (sensorZone.Type == nSensorType)
				{
                    sensor = sensorZone;
					nSensorID = sensorZone.ID;
					break;
				}
			}

            if (nSensorID < 0)
                return -1;

            outSensorID = nSensorID;

			int connected = 0;
			

			if (nSensorData == 0)
			{
				connected = 1;
				data = 0;
			}
			else if (nSensorData == 1)
			{
				connected = 1;
				data = 1;
			}
			else if (nSensorData == 2)
			{
				connected = 0;
				data = 0;

				if (sensor.IsConnected != (connected == 1))
				{
					// 센서 접속 정보가 변경되었으므로 DB에 저장한다.
					string strUpdate1 = "Update SensorZone Set Connected ='" + connected + "', Data = '" + data + "' Where Type ='" + nSensorType + "' and EquipZoneID = '" + equipZone.ID + "'";
					m_dbMgr.GetResultData(strUpdate1, 0);

					sensor.IsConnected = (connected == 1);
					bconnected = (connected == 1);
					return -2;
				}
			}
			else if (nSensorData == 3)
			{
				connected = 1;
				data = 0;

				if (sensor.IsConnected != (connected == 1))
				{
					// 센서 접속 정보가 변경되었으므로 DB에 저장한다.
					string strUpdate1 = "Update SensorZone Set Connected ='" + connected + "', Data = '" + data + "' Where Type ='" + nSensorType + "' and EquipZoneID = '" + equipZone.ID + "'";
					m_dbMgr.GetResultData(strUpdate1, 0);

					sensor.IsConnected = (connected == 1);
					bconnected = (connected == 1);
					return -2;
				}
			}

            // 이미 존재하는 값인지 확인해서 존재하는 값이면 더이상 진행하지 않는다.
            int nHistoryID = GetSensorHistoryID(nSensorID, connected == 1, data, ref nPrevSensorHistoryID);
            if (nHistoryID > 0)
            {
                return nHistoryID;
            }

            // 무시할 센서인지 확인
			if (AbnormalSensorManager.Instance.Exist(nSensorID) == true)
            {
                return -1;
            }

            sensor.IsConnected = connected == 1;
            sensor.SensorData = data;

			//SensorZone
			string strUpdate = "Update SensorZone Set Connected ='" + connected + "', Data = '" + data + "' Where Type ='" + nSensorType + "' and EquipZoneID = '" + equipZone.ID + "'";

			m_dbMgr.GetResultData(strUpdate, 0);

			//최대ID값 찾기
			string sqlID = "select max(id) as id from SensorZoneHistory";

			ArrayList arrResult = m_dbMgr.GetResultData(sqlID, 0);
			int nResultCount = arrResult.Count;

			int Max_ID = 0;
			for (int i = 0; i < nResultCount; i += 1)
			{
				//Data가 아예 안들어가 있을경우 0부터 시작
				int Find_Maxid = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				Max_ID = Find_Maxid;
			}
			Max_ID++;

			DateTime dtNow = DateTime.Now;
			string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);

			//History
			string sqlInsert = "insert into SensorZoneHistory(ID, SensorID,Connected,Data,Time) Values('"
				+ Max_ID + "','" + nSensorID + "','" + connected + "','" + data + "','" + strDateTimeField + "')";

			/*//가장 최근에 올라온 데이터와 같을 경우 비교
			bool result = CompareData(Max_ID, nSensorID, connected, data);

			if (result)*/
				m_dbMgr.GetResultData(sqlInsert, 0);

                m_dicSensorHistory[Max_ID] = nSensorID;
                System.Diagnostics.Trace.WriteLine(string.Format("__SensorHistory[{0}] = {1}", Max_ID, nSensorID));

			return Max_ID;
		}

        public void RemoveSensorHistory(int nSensorHistoryID)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("RemoveSensorHistory({0})", nSensorHistoryID));
            m_dicSensorHistory.Remove(nSensorHistoryID);
        }

        // 이미 같은 값이 존재하면 해당 HistoryID를 리턴하고,
        // 존재하지 않으면 -1을 리턴한다.
        public int GetSensorHistoryID(int nSensorID, bool isConnected, int nData, ref int nPrevSensorHistoryID)
        {
            int nHistoryCount = m_provider.GetTimeHistoryCount();

            for (int i = 0; i < nHistoryCount; i++)
            {
                TimeHistory history = m_provider.GetTimeHistory(i);

                if (!m_dicSensorHistory.ContainsKey(history.HistoryID))
                    continue;

                int sensorID = m_dicSensorHistory[history.HistoryID];

                if (nSensorID != sensorID)
                    continue;

                SensorZone sensor = FormMain.Instance.IOManager.GetSensorZone(nSensorID);
                if (sensor == null)
                    continue;

                nPrevSensorHistoryID = history.HistoryID;

                if (sensor.IsConnected == isConnected && sensor.SensorData == nData)
                    return history.HistoryID;
            }

            return -1;
        }
        
        // Return : EquipmentZone ID
        public int GetSensorZone(int nSensorID)
		{
			string szSQP = string.Format("SELECT EquipZoneID FROM SensorZone WHERE ID = {0}", nSensorID);

			ArrayList arrResult = m_dbMgr.GetResultData(szSQP, 0);

			if (arrResult == null || arrResult.Count == 0)
				return -1;

			int nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			return nID;
		}

		public void GetSensorInfo(int nSensorID, out int nType, out int nOrgID)
		{
			nType = 0;
			nOrgID = -1;
			string szSQP = string.Format("SELECT Type, OrgSensorID FROM SensorZone WHERE ID = {0}", nSensorID);
			//1(화재탐지 센서), 2(소화 센서), 3(압력 센서)
			ArrayList arrResult = m_dbMgr.GetResultData(szSQP, 0);

			if (arrResult == null || arrResult.Count < 2)
				return;

			nType = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			nOrgID = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), -1);

		}

		public bool GetSensorLocation(int nOrgID, int nType, out float x, out float y, out float z)
		{
			x = 0.0f;
			y = 0.0f;
			z = 0.0f;
			//1(화재탐지 센서), 2(소화 센서), 3(압력 센서)
			string szSQL = "";

			if (nType == 1)
			{
				szSQL = string.Format("SELECT X, Y, Z FROM FireSensor WHERE ID= {0}", nOrgID);
			}
			else if (nType == 2)
			{
				szSQL = string.Format("SELECT X, Y, Z FROM SpringCooler WHERE ID= {0}", nOrgID);
			}
			else if (nType == 3)
			{
				szSQL = string.Format("SELECT X, Y, Z FROM PumpPressureSensor WHERE ID= {0}", nOrgID);
			}
			else
				return false;

			WebDBManager webDB = FormMain.Instance.DBManager;
			ArrayList arrResult = webDB.GetResultData(szSQL, 0);
			int nResultCount = 0;
			if (arrResult == null || arrResult.Count == 0)
			{
				return true;
			}
			nResultCount = arrResult.Count;
			for (int i = 0; i < nResultCount -2; i += 3)
			{
				x = WebDBManager.GetFloatField(arrResult[i + 0].ToString(), 0.0f);
				y = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
				z = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
			}
			return true;
		}

        // Server가 꺼져있는 동안 발생했던 History 정보를 읽어온다.
        public void ReadSensorHistory(ServiceProvider provider)
        {
            string strSQL = string.Format("select srh.ID, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, szh.SensorID from SensorReactionHistory as srh, SensorZoneHistory as szh where SensorHistoryID > {0} and srh.SensorHistoryID = szh.ID order by SensorHistoryID",
                m_nLastReadReactionHistoryID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            DateTime dtDefault = new DateTime();

            SensorReactionLog log = new SensorReactionLog();
            bool isSuccess;
            int nMaxID = -1, nPrevSensorID = -1;

            ArrayList arrTimeHistory = new ArrayList();

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                string strParam1 = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
                string strParam2 = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");
                int nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);

                if (nID < 0 || nHistoryID < 0)
                    continue;

                SensorReactionLog.ReactionType type = SensorReactionLog.ToReactionType(nReactionType, out isSuccess);
				
				// 방송정보와 sms송신 로그는 보내지 않는다.
				if (type == SensorReactionLog.ReactionType.SEND_SMS || type == SensorReactionLog.ReactionType.RUN_BROADCAST)
					continue;

                if (!isSuccess)
                    continue;

                if (log.SensorHistoryID > 0 && log.SensorHistoryID != nHistoryID)
                {
                    CheckHistory(ref nMaxID, nPrevSensorID, log, arrTimeHistory);
                }
                else if (log.SensorHistoryID > 0 && log.LogTime > time)
                    continue;

                log.ID = nID;
                log.SensorHistoryID = nHistoryID;
                log.Type = type;
                log.LogTime = time;
                log.Message = string.Compare(strMessage, "null", true) == 0 ? "" : strMessage;
                log.Param1 = string.Compare(strParam1, "null", true) == 0 ? "" : strParam1;
                log.Param2 = string.Compare(strParam2, "null", true) == 0 ? "" : strParam2;
                nPrevSensorID = nSensorID;
            }

            CheckHistory(ref nMaxID, nPrevSensorID, log, arrTimeHistory);
            provider.AddTimeHistoryList(arrTimeHistory);

            foreach (TimeHistory history in arrTimeHistory)
            {
                if (history.LastReactionLog == null)
                    continue;

                if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.RUN_SOP)
                    provider.ProcessRunSOP(history.LastReactionLog);
                else if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.NOTIFY_FIRE)
                    provider.MonitorNotifyFireProcess(history.LastReactionLog);
                else if (history.LastReactionLog.Type == SensorReactionLog.ReactionType.BEGIN_STATUS)
                    provider.MonitorDetectFireProcess(history.LastReactionLog);
            }
        }

        private void CheckHistory(ref int nMaxReactionID, int nSensorID, SensorReactionLog log, ArrayList arrTimeHistory)
        {
            if (log.SensorHistoryID < 0)
                return;

            if (log.Type == SensorReactionLog.ReactionType.IGNORE_FIRE ||
                log.Type == SensorReactionLog.ReactionType.IGNORE_SOP ||
                log.Type == SensorReactionLog.ReactionType.MALFUNCTION)
                return;
            else if (log.Type == SensorReactionLog.ReactionType.FINISH_SOP ||
                log.Type == SensorReactionLog.ReactionType.RUN_N_CANCEL_SOP)
            {
                SensorZone sensor = FormMain.Instance.IOManager.GetSensorZone(nSensorID);

                if (sensor != null && sensor.SensorData == 0)
                    return;
            }

            TimeSpan span = DateTime.Now - log.LogTime;

            if (log.Type == SensorReactionLog.ReactionType.RUN_SOP)
            {
                // 화재 SOP 발동후 Timeout(일)이 지나도록 종료되지 않은 것은 취소된 것으로 간주한다.
                if (span.TotalDays > FormMain.Instance.ServiceProvider.SOPTimeout)
                {
                    UpdateReactionHistory(ref nMaxReactionID, log, SensorReactionLog.ReactionType.RUN_N_CANCEL_SOP, arrTimeHistory);
                    return;
                }
            }
            else if (log.Type == SensorReactionLog.ReactionType.NOTIFY_FIRE)
            {
                if (span.TotalHours > FormMain.Instance.ServiceProvider.NotifyFireTimeout)
                {
                    // 화재 신고후 Timeout(시간)이 지나도록 발동하지 않은 SOP는 무시된 것으로 간주한다.
                    UpdateReactionHistory(ref nMaxReactionID, log, SensorReactionLog.ReactionType.IGNORE_SOP, arrTimeHistory);
                    return;
                }
            }

            // nSensorID에 대한 SensorHistory가 존재하면, 그 이후 History가 생성되었으므로 기존것은 없앤다.
            int nOldSensorHistoryID = GetSensorHistoryID(nSensorID);
            if (nOldSensorHistoryID > 0)
            {
                m_dicSensorHistory.Remove(nOldSensorHistoryID);
                System.Diagnostics.Trace.WriteLine(string.Format("__RemoveHistory({0})", nOldSensorHistoryID));
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////

            m_dicSensorHistory[log.SensorHistoryID] = nSensorID;
            System.Diagnostics.Trace.WriteLine(string.Format("SensorHistory[{0}] = {1}", log.SensorHistoryID, nSensorID));

            TimeHistory history = FormMain.Instance.ServiceProvider.FindTimeHistory(log.SensorHistoryID);

            if (history != null)
            {
                history.LastReactionLog = log;
                history.Time = log.LogTime;
            }
            else
            {
                history = new TimeHistory(log.SensorHistoryID, log.LogTime);
                history.LastReactionLog = log;
                arrTimeHistory.Add(history);
            }
        }

        private int GetSensorHistoryID(int nSensorID)
        {
            foreach (KeyValuePair<int, int> pair in m_dicSensorHistory)
            {
                if (pair.Value == nSensorID)
                    return pair.Key;
            }
            return -1;
        }

        private void UpdateReactionHistory(ref int nMaxReactionID, SensorReactionLog log, SensorReactionLog.ReactionType type, ArrayList arrTimeHistory)
        {
            if (nMaxReactionID < 0)
            {
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData("Select max(ID) from SensorReactionHistory", 0);
                if (arrResult == null)
                    return;

                if (arrResult.Count == 0)
                    nMaxReactionID = 0;
                else
                    nMaxReactionID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            }

            DateTime dtNow = DateTime.Now;

            string strSQL = string.Format("Insert into SensorReactionHistory (ID, SensorHistoryID, ReactionType, Time, Message, Param1, Param2) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', '{6}')",
                ++nMaxReactionID, log.SensorHistoryID, (int)type, string.Format("{0} {1:00}:{2:00}:{3:00}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second),
                log.Message, "System Reaction", "");

            if (FormMain.Instance.DBManager.GetResultData(strSQL, 0) != null)
            {
                foreach (TimeHistory history in arrTimeHistory)
                {
                    if (history.HistoryID > log.SensorHistoryID)
                        return;
                }
                SetLastReadSensorHistoryID(log.SensorHistoryID);
            }
        }

        public int GetSensorID(int nSensorHistoryID)
        {
            if (m_dicSensorHistory.ContainsKey(nSensorHistoryID))
                return m_dicSensorHistory[nSensorHistoryID];

            return -1;
        }	
	}
}
