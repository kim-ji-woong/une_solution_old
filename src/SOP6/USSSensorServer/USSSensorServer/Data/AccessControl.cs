using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using UnE.Sensor;

namespace USSSensorServer.Data
{
    public class AccessControl
    {
        private WebDBManager m_dbMgr = null;
        private DirectDBManager m_s1dbMgr = null;
        private string strSiteID = "";

        private string strServerIP = "";
        private string strDBName = "";
        private string strUserID = "";
        private string strPassword = "";

        private bool m_shutdownThread = false;
        
        public AccessControl(WebDBManager dbMgr)
        {
            strSiteID = System.Configuration.ConfigurationManager.AppSettings["siteid"].ToString().Trim();
            strServerIP = System.Configuration.ConfigurationManager.AppSettings["S1ServerIP"].ToString().Trim();
            strDBName = System.Configuration.ConfigurationManager.AppSettings["S1DbName"].ToString().Trim();
            strUserID = System.Configuration.ConfigurationManager.AppSettings["S1UserID"].ToString().Trim();
            strPassword = System.Configuration.ConfigurationManager.AppSettings["S1Password"].ToString().Trim();

            if (strServerIP.Length == 0 || strDBName.Length == 0 || strUserID.Length == 0 || strPassword.Length == 0)
                return;

            m_s1dbMgr = DirectDBManager.MakeInstance(DirectDBManager.DBType.sqlserver, strServerIP, strUserID, strPassword, strDBName);
            m_s1dbMgr.Connect();
            m_dbMgr = dbMgr;

            LoadDoor();

            System.Threading.Thread thread = new System.Threading.Thread(Display);
            thread.Start();
        }
        /// <summary>
        /// zoneID, BuildingName
        /// </summary>
        private Dictionary<int, string> m_dicBuildingName = new Dictionary<int, string>();
        /// <summary>
        /// TagID, ZoneID
        /// </summary>
        private Dictionary<int, int> m_dicTagZone = new Dictionary<int, int>();
        /// <summary>
        /// ZoneID, List(DoorName)
        /// </summary>
        private Dictionary<int, List<string>> m_dicDoor = new Dictionary<int, List<string>>();
        private List<int> m_alarmZones = new List<int>(); // 알람 발생중인 Zone
        public List<int> AlarmZones
        {
            get { return m_alarmZones; }
            set { m_alarmZones = value; }
        }
        private void LoadDoor()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select z.ID, b.BuildingName ");
            sb.Append("  From Zone as z, Building as b ");
            sb.Append(" Where z.BuildingID = b.ID");

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            for (int i = 0; i < arrResult.Count; i += 2)
            {
                int nZoneID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strBuildingName = WebDBManager.GetStringField(arrResult[i + 1]);

                m_dicBuildingName[nZoneID] = strBuildingName;
            }

            sb = new StringBuilder();
            sb.Append("Select TagID, Zone ");
            sb.Append("  From SensorZone as sz, SensorTagInfo as sti ");
            sb.Append(" Where sz.ID = sti.SensorZoneID ");
            sb.AppendFormat("   And sz.Type = {0} ", (int)IFacility.FacilityType.FIRE_SENSOR);

            arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            for (int i = 0; i < arrResult.Count; i += 2)
            {
                int nTagID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                if (nTagID == -1 || nZoneID == -1)
                    continue;

                m_dicTagZone[nTagID] = nZoneID;
            }

            sb = new StringBuilder();
            sb.Append("Select Name, ZoneID From DoorSensor");

            arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            for (int i = 0; i < arrResult.Count; i+=2)
            {
                string strName = WebDBManager.GetStringField(arrResult[i]);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                if (nZoneID == -1)
                    continue;

                if (!m_dicDoor.ContainsKey(nZoneID))
                    m_dicDoor.Add(nZoneID, new List<string>());

                m_dicDoor[nZoneID].Add(strName);
            }
        }

        private void Display()
        {
            while (!m_shutdownThread)
            {
                atc();
                foreach (int zoneID in m_alarmZones.ToList())
                {
                    DisplayCloseDoor(zoneID);
                }
            }
        }

        private void DisplayCloseDoor(int nZoneID)
        {
            if (!m_s1dbMgr.IsConnected)
            {
                m_s1dbMgr.Connect();
            }

            if (!m_s1dbMgr.IsConnected)
                return;

            if (!m_dicBuildingName.ContainsKey(nZoneID))
                return;

            string buildingName = m_dicBuildingName[nZoneID];
            string strTableName = "";
            if (buildingName == "호텔")
                strTableName = "EXPORTEVENT_H";
            else if (buildingName == "리테일")
                strTableName = "EXPORTEVENT_R";
            else if (buildingName == "타워1")
                strTableName = "EXPORTEVENT_T1";
            else if (buildingName == "타워2")
                strTableName = "EXPORTEVENT_T2";

            if (strTableName.Length == 0)
                return;

            string strCondition = "";
            if (!m_dicDoor.ContainsKey(nZoneID))
                return;

            List<string> doorNames = m_dicDoor[nZoneID];
            foreach (string str in doorNames)
            {
                if (strCondition.Length == 0)
                    strCondition = string.Format("Name Like '%_{0}_%'", str);
                else
                    strCondition += string.Format(" or Name Like '%_{0}_%'", str);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Select Name, StatusContent ");
            sb.AppendFormat(" From {0} ", strTableName);
            sb.Append("Where Concat(ATime,'/',LACode) In ( ");
            sb.Append(" Select Concat(Max(ATime),'/', LACode) From ( ");
            sb.AppendFormat("Select ATime, LACode From {0} Where ({1})) as vv Group by LACode)", strTableName, strCondition);

            ArrayList arrResult = m_s1dbMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            for (int i = 0; i < arrResult.Count; i += 2)
            {
                string strName = WebDBManager.GetStringField(arrResult[i]);
                string strStatus = WebDBManager.GetStringField(arrResult[i + 1]);

                for (int j = 0; j < m_dicDoor[nZoneID].Count; j++)
                {
                    string doorName = m_dicDoor[nZoneID][j];
                    if (strName.Contains(doorName))
                    {
                        if (strStatus.Contains("잠김 닫힘") || strStatus.Contains("풀림 닫힘"))
                            m_dbMgr.GetResultData(string.Format("Update DoorSensor Set Description='{0}' Where Name='{1}'", "닫힘", doorName));
                        else
                            m_dbMgr.GetResultData(string.Format("Update DoorSensor Set Description='{0}' Where Name='{1}'", "열림", doorName));
                    }
                }
            }
        }

        public void Close()
        {
            m_shutdownThread = true;
        }

        private void atc()
        {
            string szText = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param3, szh.SensorID, szh.Param3";
            szText += " FROM SensorReactionHistory as srh ";
            szText += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            szText += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + " ) ";
            szText += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + " ) ";
            szText += " AND szh.SiteID = " + strSiteID;
            szText += " ORDER BY srh.Time, szh.SensorID";
            
            string strSQL = string.Format(szText, strSiteID);

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
