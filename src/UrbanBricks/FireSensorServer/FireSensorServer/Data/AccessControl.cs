using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FireSensorServer.Data
{
    public class AccessControl
    {
        private WebDBManager m_dbMgr = null;
        private DirectDBManager m_doorDBMgr = null;
        
        private bool m_shutdownThread = false;

        /// <summary>
        /// ZoneID, List(DoorName)
        /// </summary>
        private static Dictionary<int, List<SensorInfo>> m_dicDoorSensor = new Dictionary<int, List<SensorInfo>>();
        /// <summary>
        /// 알람 발생중인 Zone
        /// </summary>
        private List<int> m_alarmZones = new List<int>();

        public AccessControl(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;

            string strDoorServerIP = System.Configuration.ConfigurationManager.AppSettings["DoorServerIP"].ToString();
            string strDoorDbID = System.Configuration.ConfigurationManager.AppSettings["DoorDbID"].ToString();
            string strDoorDbPw = System.Configuration.ConfigurationManager.AppSettings["DoorDbPw"].ToString();
            string strDoorDbName = System.Configuration.ConfigurationManager.AppSettings["DoorDbName"].ToString();
            if (strDoorDbID.Length == 0 || strDoorDbID.Length == 0 || strDoorDbPw.Length == 0 || strDoorDbName.Length == 0)
                return;

            m_doorDBMgr = DirectDBManager.MakeInstance(DirectDBManager.DBType.sqlserver, strDoorServerIP, strDoorDbID, strDoorDbPw, strDoorDbName);
            m_doorDBMgr.Connect();

            LoadDoor();

            System.Threading.Thread thread = new System.Threading.Thread(Display);
            thread.Start();
        }

        private void LoadDoor()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select ID, Name, ZoneID From DoorSensor");

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null)
                return;

            int arrCount = arrResult.Count;
            for (int i = 0; i < arrCount; i += 3)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strDoorName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> nZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (nID == null || nZoneID == null)
                    continue;

                SensorInfo sensor = new SensorInfo();
                sensor.SensorZoneID = nID.Data;
                sensor.SensorName = strDoorName;
                sensor.ZoneID = nZoneID.Data;

                if (!m_dicDoorSensor.ContainsKey(sensor.ZoneID))
                    m_dicDoorSensor.Add(sensor.ZoneID, new List<SensorInfo>());

                m_dicDoorSensor[sensor.ZoneID].Add(sensor);
            }
        }

        private void Display()
        {
            while (!m_shutdownThread)
            {
                DisplayAlarm();
                foreach (int zoneID in m_alarmZones.ToList())
                {
                    DisplayCloseDoor(zoneID);
                }
            }
        }

        private void DisplayCloseDoor(int nZoneID)
        {
            if (!m_doorDBMgr.IsConnected)
                m_doorDBMgr.Connect();

            if (!m_doorDBMgr.IsConnected)
                return;

            string strCondition = "";
            List<SensorInfo> doorNames = m_dicDoorSensor[nZoneID];
            foreach (SensorInfo info in doorNames)
            {
                if (strCondition.Length == 0)
                    strCondition = string.Format("DeviceName like '%_{0}'", info.SensorName);
                else
                    strCondition += string.Format(" or DeviceName like '%_{0}'", info.SensorName);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Select DeviceName, OpenStatus ");
            sb.Append(" From View_Door_Status ");
            sb.AppendFormat("Where ({0})", strCondition);

            ArrayList arrResult = m_doorDBMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            for (int i = 0; i < arrResult.Count; i += 2)
            {
                string strName = WebDBManager.GetStringField(arrResult[i]);
                int nStatus = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);

                int doorCount = m_dicDoorSensor[nZoneID].Count;
                for (int j = 0; j < doorCount; j++)
                {
                    SensorInfo sensor = m_dicDoorSensor[nZoneID][j];
                    if (strName.Contains(sensor.SensorName))
                    {
                        if (nStatus == 0)
                            m_dbMgr.GetResultData(string.Format("Update DoorSensor Set Description='{0}' Where ID={1}", "닫힘", sensor.SensorZoneID));
                        else
                            m_dbMgr.GetResultData(string.Format("Update DoorSensor Set Description='{0}' Where ID={1}", "열림", sensor.SensorZoneID));
                    }
                }
            }
        }

        public void Close()
        {
            m_shutdownThread = true;
        }

        private void DisplayAlarm()
        {
            string strSQL = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param3, szh.SensorID, szh.Param3";
            strSQL += " FROM SensorReactionHistory as srh ";
            strSQL += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            strSQL += "WHERE SensorHistoryID in (SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + " ) ";
            strSQL += " AND SensorHistoryID not in (SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + " ) ";
            strSQL += " AND szh.SiteID = " + m_dbMgr.SiteID;
            //strSQL += " ORDER BY srh.Time, szh.SensorID";
            
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            DateTime dtDefault = new DateTime();
            
            int nSensorID = -1;

            ArrayList arrTimeHistory = new ArrayList();

            SortedList<int, int> keyExistList = new SortedList<int, int>();

            List<int> sensorZoneIDs = new List<int>();
            
            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 4], "");
                
                if (nID < 0 || nHistoryID < 0)
                    continue;

                nSensorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                
                if ((nReactionType == (int)libSensorProcess.ReactionType.BEGIN_STATUS))
                    nSensorID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                string strSensorZoneIDs = WebDBManager.GetStringField(arrResult[i + 7]);

                if (strSensorZoneIDs == null || strSensorZoneIDs.Length == 0)
                    CheckAlarmSensorZone(nSensorID, nHistoryID, nReactionType, strMessage, keyExistList, sensorZoneIDs);
                else
                {
                    string[] ids = strSensorZoneIDs.Split(',');
                    int id;

                    foreach (string strID in ids)
                    {
                        if (int.TryParse(strID.Trim(), out id))
                        {
                            CheckAlarmSensorZone(id, nHistoryID, nReactionType, strMessage, keyExistList, sensorZoneIDs);
                        }
                    }
                }
            }

            m_alarmZones = sensorZoneIDs;
        }

        // 현재 Alarm이 발생중인 SensorReactionLog에 대한 Query 조건문
        private string GetAlarmReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.BEGIN_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.NOTIFY_SIGNAL).ToString();

            return "(" + strCondition + ")";
        }

        //현재 Alarm이 꺼진 SensorReactionLog에 대한 Query조건문
        private string GetAlarmOffReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.MALFUNCTION).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SIGNAL).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SOP).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.END_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.USER_RESET).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.TIME_OUT).ToString();

            return "(" + strCondition + ")";
        }

        private bool CheckAlarmSensorZone(int nSensorZoneID, int nSensorZoneHistoryID, int nReactionType, string strMessage, SortedList<int, int> keyExistList, List<int> sensorZoneIDs)
        {
            string szHashKey = nSensorZoneHistoryID.ToString() + "_-_" + nSensorZoneID + "_-_" + nReactionType.ToString() + "_-_" + strMessage;
            int nHash = szHashKey.GetHashCode();
            if (keyExistList.ContainsKey(nHash))
                return false;

            keyExistList.Add(nHash, nHash);

            bool isSuccess;
            libSensorProcess.ReactionType type = ToReactionType(nReactionType, out isSuccess);

            if (type == libSensorProcess.ReactionType.SEND_SMS || type == libSensorProcess.ReactionType.RUN_BROADCAST)
                return false;

            if (!isSuccess)
                return false;

            // 화학물질 센서는 통합처리되므로 data가 같은 SensorZone이므로 각기 SensorZone의 Data를 확인하도록 한다.
            // skkim 2016-02-26 
            string szText2 = "SELECT Data, Zone FROM SensorZone WHERE ID = {0}";
            string szSQL2 = string.Format(szText2, nSensorZoneID);
            ArrayList arrResult2 = m_dbMgr.GetResultData(szSQL2);
            if (arrResult2 == null || arrResult2.Count == 0)
                return false;

            int nSensorData = WebDBManager.GetIntField(arrResult2[0].ToString(), -1);
            int nZoneID = WebDBManager.GetIntField(arrResult2[1].ToString(), -1);
            if (nSensorData == 1 || nSensorData == 21 || nSensorData == 22 || nSensorData == 23)
            {
                if (nZoneID > 0)
                {
                    if (!sensorZoneIDs.Contains(nZoneID))
                    {
                        sensorZoneIDs.Add(nZoneID);
                        return true;
                    } 
                }
            }

            return false;
        }

        private Dictionary<int, libSensorProcess.ReactionType> m_dicReactionType = null;
        public libSensorProcess.ReactionType ToReactionType(int nType, out bool isSuccess)
        {
            isSuccess = true;

            if (m_dicReactionType == null)
            {
                m_dicReactionType = new Dictionary<int, libSensorProcess.ReactionType>();

                foreach (libSensorProcess.ReactionType type in Enum.GetValues(typeof(libSensorProcess.ReactionType)))
                {
                    m_dicReactionType[(int)type] = type;
                }
            }

            libSensorProcess.ReactionType fType;

            if (m_dicReactionType.TryGetValue(nType, out fType))
                return fType;

            isSuccess = false;
            return libSensorProcess.ReactionType.ETC;
        }
    }
}
