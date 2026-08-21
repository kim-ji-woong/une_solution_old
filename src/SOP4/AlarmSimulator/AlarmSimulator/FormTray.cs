using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;

namespace AlarmSimulator
{
    public partial class FormTray : Form
    {
        private NotifyIcon m_trayIcon = null;
        private ContextMenu m_trayMenu = null;
        private Timer m_timer = new Timer();
        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = -1;
        private List<AlarmBoard> m_alarms = new List<AlarmBoard>();
        // Key : SensorTag ID
        private Dictionary<int, SensorTag> m_dicSensorTags = new Dictionary<int, SensorTag>();
        // Key : SensorZone ID
        private Dictionary<int, SensorTag> m_dicSensorZoneSensorTags = new Dictionary<int, SensorTag>();

        private List<SensorTag> m_fireSensorTags = new List<SensorTag>();
        private List<SensorTag> m_nh3SensorTags = new List<SensorTag>();
        private List<SensorTag> m_hclSensorTags = new List<SensorTag>();
        private List<SensorTag> m_h2SensorTag = new List<SensorTag>();
        private List<SensorTag> m_acetilenSensorTag = new List<SensorTag>();
        private List<SensorTag> m_metanolSensorTag = new List<SensorTag>();
        private List<SensorTag> m_sodaSensorTag = new List<SensorTag>();
        private List<SensorTag> m_dieselSensorTag = new List<SensorTag>();

        private NetworkManager m_netMgr = null;
        
        private const string ALARM_SIMULATION_DB = "AlarmSimulation";
        
        public FormTray()
        {
            m_trayMenu = new ContextMenu();
            m_trayMenu.MenuItems.Add("종료", OnClose);

            m_trayIcon = new NotifyIcon();
            m_trayIcon.Text = "알람 시뮬레이터";
            m_trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            m_trayIcon.ContextMenu = m_trayMenu;
            m_trayIcon.Visible = true;

            m_timer.Interval = 1000;
            m_timer.Tick += new System.EventHandler(this.OnTimer);

            m_nSiteID = ReadSiteID();
            m_dbMgr = new WebDBManager(m_nSiteID);

            m_netMgr = new NetworkManager(m_dbMgr, "127.0.0.1", m_nSiteID);
        }

        private int ReadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");
            
            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            ReadCurrentAlarms();
            ReadNewAlarms();
        }

        private void ReadNewAlarms()
        {
            WebDBManager dbMgr = new WebDBManager(ALARM_SIMULATION_DB, m_nSiteID);
            string strSQL = "Select ID, AlarmCategory, AlarmParameter from RequestAlarm where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0, ALARM_SIMULATION_DB);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            // 수신반 서버는 클라이언트와 PingCount를 세지 않기 때문에 접속이 계속 유지되고 있는지 여부를 확인할 수 없다.
            // 따라서, 필요한 경우마다 Socket을 연결하도록 한다.
            if (nResultCount > 0)
                m_netMgr.Connect();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strAlarmCategory = WebDBManager.GetStringField(arrResult[i + 1]);
                string strAlarmParameter = WebDBManager.GetStringField(arrResult[i + 2]);

                if (id == null || strAlarmCategory == null || strAlarmParameter == null)
                    continue;

                if (strAlarmCategory == "화재")
                    ProcessFireAlarm(id.Data, strAlarmParameter);
                else if (strAlarmCategory == "오염")
                    ProcessPSMAlarm(id.Data, strAlarmParameter);
                else if (strAlarmCategory == "지진")
                    ProcessEarthquakeAlarm(id.Data, strAlarmParameter);
            }

            if (nResultCount > 0)
                m_netMgr.Close();
        }

        private SensorTag GetRandomSensorTag(List<SensorTag> sensorTags)
        {
            int nSensorCount = sensorTags.Count;

            if (nSensorCount == 0)
                return null;

            Random rnd = new Random((int)DateTime.Now.ToBinary());
            int nIndex = rnd.Next(0, nSensorCount - 1);
            return sensorTags[nIndex];
        }

        private void RemoveRequest(int nRequestID)
        {
            WebDBManager dbMgr = new WebDBManager(ALARM_SIMULATION_DB, m_nSiteID);
            string strSQL = "Delete from RequestAlarm where ID = " + nRequestID.ToString();
            dbMgr.GetResultData(strSQL, 0, ALARM_SIMULATION_DB);
        }

        private void ProcessEarthquakeAlarm(int nRequestID, string strParameter)
        {
            string[] tokens = strParameter.Split('\t');
            int nTokenCount = tokens.Count();

            // SensorID는 의미없는 값이다.
            int nSensorID = 1;

            if (nTokenCount < 2)
            {
                m_netMgr.SendEarthquakeSignal(nSensorID, -1.0f, -1, -1, "", DateTime.Now);
                RemoveRequest(nRequestID);
            }
            else if (nTokenCount >= 2)
            {
                if (tokens[0].Trim() == "규모")
                {
                    string strMagnitude = tokens[1].Trim();
                    float fMagnitude;

                    if (float.TryParse(strMagnitude, out fMagnitude))
                    {
                        m_netMgr.SendEarthquakeSignal(nSensorID, fMagnitude, -1, -1, "", DateTime.Now);
                        RemoveRequest(nRequestID);
                    }
                }
                else if (tokens[0].Trim() == "진도")
                {
                    string strIntensity = tokens[1].Trim();
                    int nIntensity;

                    if (int.TryParse(strIntensity, out nIntensity))
                    {
                        m_netMgr.SendEarthquakeSignal(nSensorID, -1.0f, nIntensity, -1, "", DateTime.Now);
                        RemoveRequest(nRequestID);
                    }
                }
            }
        }

        private void ProcessPSMAlarm(int nRequestID, string strParameter)
        {
            string[] tokens = strParameter.Split('\t');
            int nTokenCount = tokens.Count();

            if (nTokenCount < 2)
                return;

            if (tokens[0].Trim() == "1")
            {
                string strMaterialName = tokens[1].Trim();
                List<SensorTag> sensorTags = GetSensorTagsFromMaterial(strMaterialName);

                if (sensorTags == null)
                    return;

                SensorTag sensor = GetRandomSensorTag(sensorTags);

                if (m_netMgr.SendSensorData(sensor, 0x87))
                {
                    RemoveRequest(nRequestID);
                }
            }
            else if (tokens[0].Trim() == "0")
            {
                int nSensorTagID;

                if (!int.TryParse(tokens[1].Trim(), out nSensorTagID))
                    return;

                SensorTag sensor;

                if (m_dicSensorTags.TryGetValue(nSensorTagID, out sensor))
                {
                    if (m_netMgr.SendSensorData(sensor, 0x93))
                    {
                        RemoveRequest(nRequestID);
                    }
                }
            }
        }

        private void ProcessFireAlarm(int nRequestID, string strParameter)
        {
            string[] tokens = strParameter.Split('\t');
            int nTokenCount = tokens.Count();

            if (nTokenCount == 0)
                return;

            if (tokens[0].Trim() == "1")
            {
                SensorTag sensor = GetRandomSensorTag(m_fireSensorTags);

                if (m_netMgr.SendSensorData(sensor, 0x92))
                {
                    RemoveRequest(nRequestID);
                }
            }
            else if (tokens[0].Trim() == "0")
            {
                if (nTokenCount < 2)
                    return;

                int nSensorTagID;

                if (!int.TryParse(tokens[1].Trim(), out nSensorTagID))
                    return;

                SensorTag sensor;

                if (m_dicSensorTags.TryGetValue(nSensorTagID, out sensor))
                {
                    if (m_netMgr.SendSensorData(sensor, 0x93))
                    {
                        RemoveRequest(nRequestID);
                    }
                }
            }
        }

        private void ReadCurrentAlarms()
        {
            string szText = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID FROM SensorReactionHistory as srh ";
            szText += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            szText += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in ( 0, 60, 62) ) ";
            szText += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in (21, 23, 33, 50, 70)) ";
            szText += " AND szh.SiteID = " + m_nSiteID.ToString();
            szText += " ORDER BY srh.Time, szh.SensorID";

            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            DateTime dtDefault = new DateTime();

            SensorReactionLog log = new SensorReactionLog();
            bool isSuccess;
            int nSensorID = -1;
            
            ArrayList arrTimeHistory = new ArrayList();

            SortedList<int, int> keyExistList = new SortedList<int, int>();

            // Key : SensorZone ID
            Dictionary<int, DateTime> dicSensorZoneIDs = new Dictionary<int, DateTime>();
            //List<int> sensorZoneIDs = new List<int>();

            DateTime dtNow = DateTime.Now;
            DateTime dt24 = dtNow.AddHours(-24.0);

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strMessage = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                string strParam1 = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");
                string strParam2 = DBUtility.WebDBManager.GetStringField(arrResult[i + 6], "");
                string strParam3 = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");
                string strParam4 = DBUtility.WebDBManager.GetStringField(arrResult[i + 8], "");
                string strParam5 = DBUtility.WebDBManager.GetStringField(arrResult[i + 9], "");

                if (time < dt24)
                {
                    continue;
                }

                nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                if (nReactionType == (int)SensorReactionLog.ReactionType.BEGIN_PSM_STATUS || nReactionType == (int)SensorReactionLog.ReactionType.CHANGE_PSM_ALARM_DEPTH)
                {
                    nSensorID = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                }

                if (nID < 0 || nHistoryID < 0)
                    continue;

                string szHashKey = nHistoryID.ToString() + "_-_" + nReactionType.ToString() + "_-_" + strMessage;
                int nHash = szHashKey.GetHashCode();
                if (keyExistList.ContainsKey(nHash))
                    continue;

                keyExistList.Add(nHash, nHash);


                SensorReactionLog.ReactionType type = SensorReactionLog.ToReactionType(nReactionType, out isSuccess);

                if (type == SensorReactionLog.ReactionType.SEND_SMS || type == SensorReactionLog.ReactionType.RUN_BROADCAST)
                    continue;

                if (!isSuccess)
                    continue;

                // 화학물질 센서는 통합처리되므로 data가 같은 SensorZone이므로 각기 SensorZone의 Data를 확인하도록 한다.
                // skkim 2016-02-26 
                string szText2 = "SELECT Data FROM SensorZone WHERE ID = {0}";
                string szSQL2 = string.Format(szText2, nSensorID);
                ArrayList arrResult2 = m_dbMgr.GetResultData(szSQL2, 0);
                if (arrResult2 == null || arrResult2.Count == 0)
                    continue;

                int nSensorData = DBUtility.WebDBManager.GetIntField(arrResult2[0].ToString(), -1);
                if (nSensorData == 1 || nSensorData == 21 || nSensorData == 22 || nSensorData == 23)
                {
                    if (!dicSensorZoneIDs.ContainsKey(nSensorID))
                        dicSensorZoneIDs[nSensorID] = time;
                }
            }

            UpdateAlarmBoard(dicSensorZoneIDs);
        }

        private void UpdateAlarmBoard(Dictionary<int, DateTime> dicSensorZoneIDs)
        {
            string strRemoveIDs = "";
            List<AlarmBoard> removeAlarms = new List<AlarmBoard>();

            foreach (AlarmBoard alarm in m_alarms)
            {
                if (dicSensorZoneIDs.ContainsKey(alarm.SensorZoneID))
                    dicSensorZoneIDs.Remove(alarm.SensorZoneID);
                else
                {
                    if (strRemoveIDs.Length == 0)
                        strRemoveIDs = alarm.ID.ToString();
                    else
                        strRemoveIDs += ", " + alarm.ID.ToString();

                    removeAlarms.Add(alarm);
                }
            }

            WebDBManager dbMgr = new WebDBManager(ALARM_SIMULATION_DB, m_nSiteID);

            if (strRemoveIDs.Length > 0)
            {
                string strSQL = "Delete from AlarmBoard where ID in (" + strRemoveIDs + ")";

                if (dbMgr.GetResultData(strSQL, 0, ALARM_SIMULATION_DB) == null)
                    return;

                foreach (AlarmBoard alarm in removeAlarms)
                {
                    m_alarms.Remove(alarm);
                }
            }

            if (dicSensorZoneIDs.Count > 0)
            {
                int nMaxID = GetMaxID("AlarmBoard", ALARM_SIMULATION_DB);

                foreach (KeyValuePair<int, DateTime> pair in dicSensorZoneIDs)
                {
                    SensorTag sensor;

                    if (!m_dicSensorZoneSensorTags.TryGetValue(pair.Key, out sensor))
                        continue;

                    string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", pair.Value.Year, pair.Value.Month, pair.Value.Day, pair.Value.Hour, pair.Value.Minute, pair.Value.Second);
                    string strAlarmName = "(" + sensor.TypeName + ")" + sensor.SensorName;

                    string strFormat = "Insert into AlarmBoard (ID, TimeStamp, AlarmName, SiteID, SensorZoneID, SensorTagInfoID) values ({0}, '{1}', '{2}', {3}, {4}, {5})";
                    string strSQL = string.Format(strFormat, ++nMaxID, strTime, strAlarmName, m_nSiteID, pair.Key, sensor.ID);

                    if (dbMgr.GetResultData(strSQL, 0, ALARM_SIMULATION_DB) == null)
                        break;
                    else
                    {
                        AlarmBoard alarm = new AlarmBoard();

                        alarm.ID = nMaxID;
                        alarm.AlarmName = strAlarmName;
                        alarm.SensorTagInfoID = sensor.ID;
                        alarm.SensorZoneID = pair.Key;
                        alarm.TimeStamp = pair.Value;

                        m_alarms.Add(alarm);
                    }
                }
            }
        }

        private int GetMaxID(string strTableName, string strDatabaseName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0, strDatabaseName);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            return id == null ? 0 : id.Data;
        }

        private void OnClose(object sender, EventArgs e)
        {
            m_netMgr.ShutDownThread = true;
            m_timer.Stop();
            this.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            // Hide Window
            this.Visible = false;
            this.ShowInTaskbar = false;

            ReadSensorTagInfo();
            ReadAlarmBoard();

            m_timer.Start();
            base.OnLoad(e);
        }

        // Key : SensorZone ID
        private Dictionary<int, List<SensorTag>> ReadSensorZoneSensorType()
        {
            Dictionary<int, List<SensorTag>> dicResult = new Dictionary<int,List<SensorTag>>();

            string strSQL = "Select ID, MaterialName from PSMMaterial";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            int nResultCount = 0;
            Dictionary<int, string> dicMaterialName = null;
            Dictionary<int, int> dicPSMSensorMaterial = null;

            if (arrResult != null)
            {
                //if (arrResult == null)
                //    return dicResult;

                nResultCount = arrResult.Count;
                // Key : Material ID
                dicMaterialName = new Dictionary<int, string>();

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strMaterialName = WebDBManager.GetStringField(arrResult[i + 1]);

                    if (id == null || strMaterialName == null)
                        continue;

                    dicMaterialName[id.Data] = strMaterialName;
                }

                strSQL = "Select ID, MaterialType from PSMSensor";
                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return dicResult;

                nResultCount = arrResult.Count;
                // Key : PSMSensor ID
                // Value : Material ID
                dicPSMSensorMaterial = new Dictionary<int, int>();

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<int> materialID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                    if (materialID == null || id == null)
                        continue;

                    dicPSMSensorMaterial[id.Data] = materialID.Data;
                }
            }

            strSQL = "Select ID, Type, OrgSensorID from SensorZone";
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return dicResult;

            nResultCount = arrResult.Count;
            Dictionary<int, List<SensorTag>> dicMaterialSensorTags = new Dictionary<int,List<SensorTag>>();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> type = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> orgSensorID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (id == null || type == null)
                    continue;

                SensorTag.SensorType sensorType = SensorTag.ToSensorType(type.Data);

                if (sensorType == SensorTag.SensorType.화재센서 ||
                    (sensorType >= SensorTag.SensorType.화재감지기_A && sensorType <= SensorTag.SensorType.모니터링))
                    dicResult[id.Data] = m_fireSensorTags;
                else if (sensorType == SensorTag.SensorType.PSM센서)
                {
                    if (orgSensorID == null)
                        continue;

                    int nMaterialID;

                    if (dicPSMSensorMaterial != null && dicPSMSensorMaterial.TryGetValue(orgSensorID.Data, out nMaterialID))
                    {
                        List<SensorTag> sensorTags = null;

                        if (dicMaterialSensorTags.TryGetValue(nMaterialID, out sensorTags))
                        {
                            dicResult[id.Data] = sensorTags;
                        }
                        else
                        {
                            string strMaterialName;

                            if (dicMaterialName != null && dicMaterialName.TryGetValue(nMaterialID, out strMaterialName))
                            {
                                sensorTags = GetSensorTagsFromMaterial(strMaterialName);

                                if (sensorTags != null)
                                {
                                    dicResult[id.Data] = sensorTags;
                                    dicMaterialSensorTags[nMaterialID] = sensorTags;
                                }
                            }
                        }
                    }
                }
            }

            return dicResult;
        }

        private List<SensorTag> GetSensorTagsFromMaterial(string strMaterialName)
        {
            List<SensorTag> sensorTags = null;

            if (strMaterialName.Contains("암모니아"))
                sensorTags = m_nh3SensorTags;
            else if (strMaterialName.Contains("염산"))
                sensorTags = m_hclSensorTags;
            else if (strMaterialName.Contains("가성소다"))
                sensorTags = m_sodaSensorTag;
            else if (strMaterialName.Contains("경유") || strMaterialName.Contains("부생연료유"))
                sensorTags = m_dieselSensorTag;
            else if (strMaterialName.Contains("수소"))
                sensorTags = m_h2SensorTag;
            else if (strMaterialName.Contains("메탄올"))
                sensorTags = m_metanolSensorTag;
            else if (strMaterialName.Contains("아세틸렌"))
                sensorTags = m_acetilenSensorTag;

            return sensorTags;
        }

        private void ReadSensorTagInfo()
        {
            Dictionary<int, List<SensorTag>> dicSensorZoneSensorTags = ReadSensorZoneSensorType();

            string strSQL = "Select ID, SensorServerID, TagNo, SensorName, SensorType, SensorZoneID from SensorTagInfo";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-5;i+=6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorServerID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> tagNo = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strSensorName = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<int> sensorType = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString());

                if (id == null || sensorServerID == null || tagNo == null || strSensorName == null || sensorType == null)
                    continue;

                SensorTag sensorTag = new SensorTag();

                sensorTag.ID = id.Data;
                sensorTag.ReceiverID = sensorServerID.Data;
                sensorTag.SensorTagID = tagNo.Data;
                sensorTag.SensorName = strSensorName;
                sensorTag.TagType = SensorTag.ToSensorType(sensorType.Data);
                sensorTag.SensorZoneID = sensorZoneID;

                m_dicSensorTags[sensorTag.ID] = sensorTag;

                if (sensorZoneID != null)
                {
                    m_dicSensorZoneSensorTags[sensorZoneID.Data] = sensorTag;

                    List<SensorTag> sensorTags;

                    if (dicSensorZoneSensorTags.TryGetValue(sensorZoneID.Data, out sensorTags))
                        sensorTags.Add(sensorTag);
                }
            }
        }

        private void ReadAlarmBoard()
        {
            WebDBManager dbMgr = new WebDBManager(ALARM_SIMULATION_DB, m_nSiteID);
            string strSQL = "Select ID, TimeStamp, AlarmName, SensorZoneID, SensorTagInfoID from AlarmBorad where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0, ALARM_SIMULATION_DB);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 1]);
                string strAlarmName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> sensorTagInfoID = WebDBManager.GetIntField(arrResult[i + 4].ToString());

                if (id == null || timeStamp == null || strAlarmName == null || sensorZoneID == null || sensorTagInfoID == null)
                    continue;

                AlarmBoard alarm = new AlarmBoard();

                alarm.ID = id.Data;
                alarm.TimeStamp = timeStamp.Data;
                alarm.AlarmName = strAlarmName;
                alarm.SensorZoneID = sensorZoneID.Data;
                alarm.SensorTagInfoID = sensorTagInfoID.Data;

                m_alarms.Add(alarm);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_trayIcon.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
