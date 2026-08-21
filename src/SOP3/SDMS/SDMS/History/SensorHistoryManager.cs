using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;

namespace SDMS
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

		ArrayList m_arCurrentHistoryIDs = new ArrayList();
		Dictionary<int, int> m_mapSensor = new Dictionary<int, int>();
		private int nCurHistoryID = -1;
		public int HistoryID
		{
			get { return nCurHistoryID; }
			set { nCurHistoryID = value; }
		}
		private SensorHistoryManager()
		{

		}

		public void AddSensorHistoryID(int nID)
		{
			if (!m_arCurrentHistoryIDs.Contains(nID))
			{
				m_arCurrentHistoryIDs.Add(nID);
				int nSensorID = GetSensorIDInternal(nID);
				m_mapSensor.Add(nID, nSensorID);
				nCurHistoryID = nID;
			}
		}

		public void RemoveSensorHistory(int nID)
		{
			if (m_arCurrentHistoryIDs.Contains(nID))
			{
				m_arCurrentHistoryIDs.Remove(nID);
				m_mapSensor.Remove(nID);
				if (m_arCurrentHistoryIDs.Count == 0)
				{
					nCurHistoryID = -1;
				}
				else
					nCurHistoryID = (int)m_arCurrentHistoryIDs[0];
			}
		}

		public void AddSensorHistoryIDList(ArrayList arIDList)
		{			
			foreach (int nID in arIDList)
			{
				AddSensorHistoryID(nID);
			}
		}

		public int GetManualFireReportZone(int nSensorHistoryID)
		{
			string szSQL = string.Format("select param1 from SensorZoneHistory where id = {0}", nSensorHistoryID);

			WebDBManager dbMgr = FormMain.Instance.DBManager;
			ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
			if (arrResult == null)
				return -1;
			int nResultCount = arrResult.Count;

			int nZoneID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			return nZoneID;
		}

		private int GetSensorIDInternal( int nSensorHistoryID )
		{
			string szSQL = string.Format("select SensorID from SensorZoneHistory where id = {0}", nSensorHistoryID);

			WebDBManager dbMgr = FormMain.Instance.DBManager;			
			ArrayList arrResult = dbMgr.GetResultData(szSQL, 0);
            if (arrResult == null)
                return -1;
            int nResultCount = arrResult.Count;

			int nSensorID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			return nSensorID;
		}

		public int GetSensorID( int nSensorHistoryID )
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
