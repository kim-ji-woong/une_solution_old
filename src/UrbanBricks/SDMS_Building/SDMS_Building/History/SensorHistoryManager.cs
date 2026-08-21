using System.Collections;
using System.Collections.Generic;
using DBUtility2;
using libSensorProcess;

namespace SDMS_Building.History
{
	public class SensorHistoryManager
	{
		public static SensorHistoryManager m_Instance = null;

		public static SensorHistoryManager Instance
		{
			get
			{
				if (m_Instance == null)
					m_Instance = new SensorHistoryManager();
				return m_Instance;
			}
		}

        private List<ReactionLog> m_arrCurrentSensorZoneHistories = new List<ReactionLog>();
		//private ArrayList m_arCurrentSensorZoneHistoryIDs = new ArrayList();
		private Dictionary<int, int> m_mapSensor = new Dictionary<int, int>();
		private int nCurSensorZoneHistoryID = -1;

		public int SensorZoneHistoryID
		{
			get { return nCurSensorZoneHistoryID; }
			set { nCurSensorZoneHistoryID = value; }
		}

        public ReactionLog SensorZoneHistoryLog
        {
            get { return GetSensorLog(nCurSensorZoneHistoryID); }
        }

        private int m_nLastSensorZoneHistoryID = -1;
        public int LastSensorZoneHistoryID
        {
            get { return m_nLastSensorZoneHistoryID; }
            set { m_nLastSensorZoneHistoryID = value; }
        }

        private int m_nLastSensorReactionHistoryID = -1;
        public int LastSensorReactionHistoryID
        {
            get { return m_nLastSensorReactionHistoryID; }
            set { m_nLastSensorReactionHistoryID = value; }
        }

        private int m_nSiteID = 1;
		private SensorHistoryManager()
		{
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
		}

        public ReactionLog GetSensorLog(int nSensorZoneHistoryID)
        {
            foreach (ReactionLog log in m_arrCurrentSensorZoneHistories)
            {
                if (log.SensorHistoryID == nSensorZoneHistoryID)
                    return log;
            }

            return null;
        }

        public void AddSensorHistory(ReactionLog log)
        {
            if (GetSensorLog(log.SensorHistoryID) == null)
			{
                m_arrCurrentSensorZoneHistories.Add(log);
				int nSensorID = GetSensorIDInternal(log.SensorHistoryID);
				m_mapSensor.Add(log.SensorHistoryID, nSensorID);
				nCurSensorZoneHistoryID = log.SensorHistoryID;
                m_nLastSensorZoneHistoryID = log.SensorHistoryID;
			}

            if (m_nLastSensorReactionHistoryID < log.ID)
                m_nLastSensorReactionHistoryID = log.ID;
        }

		/*public void AddSensorHistoryID(int nSensorZoneHistoryID, int nSensorReactionHistoryID)
		{
            if (!m_arCurrentSensorZoneHistoryIDs.Contains(nSensorZoneHistoryID))
			{
                m_arCurrentSensorZoneHistoryIDs.Add(nSensorZoneHistoryID);
				int nSensorID = GetSensorIDInternal(nSensorZoneHistoryID);
				m_mapSensor.Add(nSensorZoneHistoryID, nSensorID);
				nCurSensorZoneHistoryID = nSensorZoneHistoryID;
                m_nLastSensorZoneHistoryID = nSensorZoneHistoryID;
			}

            if (m_nLastSensorReactionHistoryID < nSensorReactionHistoryID)
                m_nLastSensorReactionHistoryID = nSensorReactionHistoryID;
		}*/

        public void RemoveSensorHistory(int nSensorZoneHistoryID)
		{
            ReactionLog log = GetSensorLog(nSensorZoneHistoryID);

            if (log != null)
			{
				if (m_arrCurrentSensorZoneHistories.Contains(log))
					m_arrCurrentSensorZoneHistories.Remove(log);

				if (m_mapSensor.ContainsKey(nSensorZoneHistoryID))
					m_mapSensor.Remove(nSensorZoneHistoryID);

				if (m_arrCurrentSensorZoneHistories.Count == 0)
					nCurSensorZoneHistoryID = -1;
				else
					nCurSensorZoneHistoryID = (int)m_arrCurrentSensorZoneHistories[0].SensorHistoryID;
			}
		}

        /*public void RemoveSensorHistory(int nSensorZoneHistoryID)
		{
            if (m_arCurrentSensorZoneHistoryIDs.Contains(nSensorZoneHistoryID))
			{
                m_arCurrentSensorZoneHistoryIDs.Remove(nSensorZoneHistoryID);
                m_mapSensor.Remove(nSensorZoneHistoryID);
				if (m_arCurrentSensorZoneHistoryIDs.Count == 0)
				{
					nCurSensorZoneHistoryID = -1;
				}
				else
					nCurSensorZoneHistoryID = (int)m_arCurrentSensorZoneHistoryIDs[0];
			}
		}*/

		/*public void AddSensorHistoryIDList(ArrayList arIDList)
		{
			foreach (int nID in arIDList)
			{
				AddSensorHistoryID(nID);
			}
		}*/

        public int GetManualFireReportZone(int nSensorZoneHistoryID)
		{
            string szSQL = string.Format("select param1 from SensorZoneHistory where id = {0} and SiteID = {1}", nSensorZoneHistoryID, m_nSiteID);

			WebDBManager dbMgr = FormMain.Instance.DBManager;
			ArrayList arrResult = dbMgr.GetResultData(szSQL);
			if (arrResult == null)
				return -1;
			int nResultCount = arrResult.Count;

			int nZoneID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			return nZoneID;
		}

        private int GetSensorIDInternal(int nSensorZoneHistoryID)
		{
            string szSQL = string.Format("select SensorID from SensorZoneHistory where id = {0} and SiteID = {1}", nSensorZoneHistoryID, m_nSiteID);

			WebDBManager dbMgr = FormMain.Instance.DBManager;
			ArrayList arrResult = dbMgr.GetResultData(szSQL);
			if (arrResult == null || arrResult.Count == 0)
				return -1;
			int nResultCount = arrResult.Count;

			int nSensorID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			return nSensorID;
		}

		public int GetSensorID(int nSensorHistoryID)
		{
			if (m_mapSensor.ContainsKey(nSensorHistoryID))
			{
				int nValue;
				if (m_mapSensor.TryGetValue(nSensorHistoryID, out nValue))
				{
					return nValue;
				}
				return -1;
			}
			return -1;
		}
	}
}