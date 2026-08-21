using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;
using System.Configuration;

namespace SensorSimulator
{
    using Data;
    using API;

    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;
        
        private bool m_checkToUnE = true;

        private int m_nLocalSiteID = -1;

        public int LocalSiteID
        {
            get { return m_nLocalSiteID; }
            set { m_nLocalSiteID = value; }
        }

        private string m_strLocalDBName = null;
        public string strLocalDBName
        {
            get { return m_strLocalDBName; }
            set { m_strLocalDBName = value; }
        }

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
            m_dbMgr = MakeDBManager(ref m_nLocalSiteID, ref m_strLocalDBName);

            if (m_dbMgr != null)
            {
                if (DataManager.InitData(m_dbMgr))
                {
                    InitTree();
                    timer1.Start();
                }
            }

            string strCheckToUnE = ConfigurationManager.AppSettings.Get("checkToUnE");

            if (strCheckToUnE == "0")
                m_checkToUnE = false;
        }

        private static WebDBManager MakeDBManager(ref int nLocalSiteID, ref string strLocalDBName)
        {
            string strSiteID = ConfigurationManager.AppSettings.Get("siteid");
            string strDBName = ConfigurationManager.AppSettings.Get("name");
            string strDBType = ConfigurationManager.AppSettings.Get("type");
            string strWebServerURL = ConfigurationManager.AppSettings.Get("url");

            if (strSiteID == null || strSiteID.Length == 0)
                return null;
            if (strDBName == null || strDBName.Length == 0)
                return null;
            if (strDBType == null || strDBType.Length == 0)
                return null;
            if (strWebServerURL == null || strWebServerURL.Length == 0)
                return null;

            int nSiteID, nDBType;

            if (int.TryParse(strSiteID.Trim(), out nSiteID) == false)
                return null;
            if (int.TryParse(strDBType.Trim(), out nDBType) == false)
                return null;

            WebDBManager dbMgr = new WebDBManager(nSiteID);

            dbMgr.DatabaseName = strDBName.Trim();
            dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;
            dbMgr.WebServerURL = strWebServerURL.Trim();

            string strLocalSiteID = ConfigurationManager.AppSettings.Get("localSiteID");
            string _strLocalDBName = ConfigurationManager.AppSettings.Get("localDBName");

            if (strLocalSiteID != null && strLocalSiteID.Length > 0)
            {
                int.TryParse(strLocalSiteID, out nLocalSiteID);
            }

            if (_strLocalDBName != null && _strLocalDBName.Length > 0)
            {
                strLocalDBName = _strLocalDBName;
            }

            return dbMgr;
        }

        private void InitTree()
        {
            List<BuildingGroup> rootBuildingGroups = DataManager.RootBuildingGroups;

            if (rootBuildingGroups == null)
                return;

            foreach (BuildingGroup buildingGroup in rootBuildingGroups)
            {
                TreeNode node = treeSensors.Nodes.Add(buildingGroup.Name);
                node.Tag = buildingGroup;

                AddTree(node, buildingGroup);
            }

            treeSensors.ExpandAll();
        }

        private void AddTree(TreeNode node, BuildingGroup buildingGroup)
        {
            if (buildingGroup.ChildGroups.Count > 0)
            {
                foreach (BuildingGroup child in buildingGroup.ChildGroups)
                {
                    TreeNode childNode = node.Nodes.Add(child.Name);
                    childNode.Tag = child;
                    AddTree(childNode, child);
                }
            }
            else
            {
                foreach (Building building in buildingGroup.Buildings)
                {
                    TreeNode childNode = node.Nodes.Add(building.Name);
                    childNode.Tag = building;
                    AddTree(childNode, building);
                }
            }
        }

        private void AddTree(TreeNode node, Building building)
        {
            List<Zone> zones = DataManager.GetZones(building);

            if (zones == null)
                return;

            foreach (Zone zone in zones)
            {
                TreeNode childNode = node.Nodes.Add(zone.Name);
                childNode.Tag = zone;
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            int nSiteID = m_dbMgr.SiteID;
            string strDBName = m_dbMgr.DatabaseName;

            if (m_nLocalSiteID > 0 && m_strLocalDBName != null)
            {
                nSiteID = m_nLocalSiteID;
                strDBName = m_strLocalDBName;
            }

            string strSQL = "SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID, szh.Param3 FROM SensorReactionHistory as srh ";
            strSQL += "INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ";
            strSQL += "WHERE SensorHistoryID in (  SELECT srh2.SensorHistoryID FROM SensorReactionHistory as srh2 WHERE srh2.ReactionType in " + GetAlarmReactionHistoryQueryString() + " ) ";
            strSQL += " AND SensorHistoryID not in (  SELECT srh3.SensorHistoryID FROM SensorReactionHistory as srh3 WHERE srh3.ReactionType in " + GetAlarmOffReactionHistoryQueryString() + " ) ";
            strSQL += " AND szh.SiteID = " + nSiteID.ToString();
            strSQL += " ORDER BY srh.Time, szh.SensorID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, strDBName);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            DateTime dtDefault = new DateTime();
            ArrayList arrTimeHistory = new ArrayList();

            SortedList<int, int> keyExistList = new SortedList<int, int>();

            List<int> sensorZoneIDs = new List<int>();
            int nEquipZoneID;
            
            // Key : SensorZoneHistoryID
            Dictionary<int, Zone> dicSensorZoneHistoryZones = new Dictionary<int, Zone>();
            Dictionary<int, DateTime> dicSensorZoneHistoryTimes = new Dictionary<int, DateTime>();

            DateTime dtNow = DateTime.Now;
            DateTime dt24 = dtNow.AddHours(-24.0);

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nReactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strParam1 = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strParam2 = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strParam3 = WebDBManager.GetStringField(arrResult[i + 7], "");
                string strParam4 = WebDBManager.GetStringField(arrResult[i + 8], "");
                string strParam5 = WebDBManager.GetStringField(arrResult[i + 9], "");

                if (nID < 0 || nHistoryID < 0)
                    continue;

                if (int.TryParse(strParam1.Trim(), out nEquipZoneID) == false)
                    continue;

                // SiteID 202에서는 EquipZoneID와 ZoneID가 동일하다.
                Zone zone = DataManager.GetZone(nEquipZoneID);

                if (zone == null)
                    continue;

                int nSensorID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);

                int nSensorType = -1;
                if (DataManager.GetSensorZoneType(nSensorID, out nSensorType) == false)
                    continue;

                // 화재센서만 취급한다.
                if (nReactionType == (int)libSensorProcess.ReactionType.BEGIN_STATUS && nSensorType == (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
                {
                    dicSensorZoneHistoryZones[nHistoryID] = zone;
                    dicSensorZoneHistoryTimes[nHistoryID] = time;
                    //sensorZoneIDs.Add(nSensorID);
                }
            }

            if (DataManager.BaseBuildingGroupID < 0)
            {
                List<FireAlarm> alarms = ReadFireAlarms(dicSensorZoneHistoryZones);
                UpdateGrid(alarms);
            }
            else
            {
                List<FireAlarm> alarms = ReadFireAlarmsFromLink(dicSensorZoneHistoryZones);
                UpdateGrid(alarms);
            }
        }

        private List<FireAlarm> ReadFireAlarmsFromLink(Dictionary<int, Zone> dicSensorZoneHistoryZones)
        {
            string strSensorZoneHistoryIDs = "";

            foreach (KeyValuePair<int, Zone> pair in dicSensorZoneHistoryZones)
            {
                if (strSensorZoneHistoryIDs.Length == 0)
                    strSensorZoneHistoryIDs = pair.Key.ToString();
                else
                    strSensorZoneHistoryIDs += ", " + pair.Key.ToString();
            }

            List<FireAlarm> alarms = new List<FireAlarm>();

            if (strSensorZoneHistoryIDs.Length == 0)
                return alarms;

            string strSQL = "Select WebFireAlarmHistoryID, SensorZoneHistoryID from WebFireAlarmSensorZoneHistory where SensorZoneHistoryID in (" + strSensorZoneHistoryIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, m_strLocalDBName);

            if (arrResult == null)
                return alarms;

            Dictionary<int, int> dicWebFireSensorZoneHistoryIDs = new Dictionary<int, int>();
            string strWebHistoryIDs = "";
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || sensorZoneHistoryID == null)
                    continue;

                dicWebFireSensorZoneHistoryIDs[id.Data] = sensorZoneHistoryID.Data;

                if (strWebHistoryIDs.Length == 0)
                    strWebHistoryIDs = id.Data.ToString();
                else
                    strWebHistoryIDs += ", " + id.Data.ToString();
            }

            if (strWebHistoryIDs.Length == 0)
                return alarms;

            strSQL = "Select ID, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId, SensorZoneHistoryID from WebFireAlarmHistory where ID in (" + strWebHistoryIDs + ")";
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return alarms;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strEquipCode = WebDBManager.GetStringField(arrResult[i + 1]);
                string strEquipStatus = WebDBManager.GetStringField(arrResult[i + 2]);
                string strEventID = WebDBManager.GetStringField(arrResult[i + 3]);
                string strEventTime = WebDBManager.GetStringField(arrResult[i + 4]);
                string strEventType = WebDBManager.GetStringField(arrResult[i + 5]);
                string strMapCode = WebDBManager.GetStringField(arrResult[i + 6]);
                string strFloorID = WebDBManager.GetStringField(arrResult[i + 7]);
                //VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[i + 8].ToString());

                if (id == null || strEquipCode == null || strEventID == null || strEventTime == null || strEventType == null || strMapCode == null || strFloorID == null)
                    return null;

                int nSensorZoneHistoryID;

                if (dicWebFireSensorZoneHistoryIDs.TryGetValue(id.Data, out nSensorZoneHistoryID) == false)
                    return null;

                Building building = DataManager.GetBuilding(strMapCode);

                if (building == null)
                {
                    return null;
                }

                Zone zone = DataManager.GetZone(building, strFloorID);

                if (zone == null)
                    return null;

                FireAlarm alarm = new FireAlarm();

                alarm.WebHistoryID = id.Data;
                alarm.EquipCode = strEquipCode;
                alarm.EquipStatus = strEquipStatus;
                alarm.EventID = strEventID;

                DateTime timeStamp;

                if (DateTime.TryParse(strEventTime, out timeStamp))
                    alarm.TimeStamp = timeStamp;

                alarm.EventType = strEventType;
                alarm.Zone = zone;
                alarm.SensorZoneHistoryID = nSensorZoneHistoryID;

                alarms.Add(alarm);
            }

            return alarms;
        }

        private List<FireAlarm> ReadFireAlarms(Dictionary<int, Zone> dicSensorZoneHistoryZones)
        {
            string strSensorZoneHistoryIDs = "";

            foreach (KeyValuePair<int, Zone> pair in dicSensorZoneHistoryZones)
            {
                if (strSensorZoneHistoryIDs.Length == 0)
                    strSensorZoneHistoryIDs = "'" + m_nLocalSiteID.ToString() + "_" + pair.Key.ToString() + "'";
                else
                    strSensorZoneHistoryIDs += ", '" + m_nLocalSiteID.ToString() + "_" + pair.Key.ToString() + "'";
            }

            List<FireAlarm> alarms = new List<FireAlarm>();

            if (strSensorZoneHistoryIDs.Length == 0)
                return alarms;

            string strSQL = "Select ID, dvcCd, dvcStatus, evtId, evtTime, evtType, mapCd, floorId, SensorZoneHistoryID from WebFireAlarmHistory where SensorZoneHistoryID in (" + strSensorZoneHistoryIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return alarms;

            // 같은 SensorZoneHistoryID를 가진 알람이 중복되는 것을 막기 위하여 Dictionary를 사용한다.
            Dictionary<string, FireAlarm> dicSensorZoneHistoryAlarms = new Dictionary<string, FireAlarm>();

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-8;i+=9)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strEquipCode = WebDBManager.GetStringField(arrResult[i + 1]);
                string strEquipStatus = WebDBManager.GetStringField(arrResult[i + 2]);
                string strEventID = WebDBManager.GetStringField(arrResult[i + 3]);
                string strEventTime = WebDBManager.GetStringField(arrResult[i + 4]);
                string strEventType = WebDBManager.GetStringField(arrResult[i + 5]);
                string strMapCode = WebDBManager.GetStringField(arrResult[i + 6]);
                string strFloorID = WebDBManager.GetStringField(arrResult[i + 7]);
                string strSensorZoneHistoryID = WebDBManager.GetStringField(arrResult[i + 8]);

                if (id == null || strEquipCode == null || strEventID == null || strEventTime == null ||
                    strEventType == null || strMapCode == null || strFloorID == null || strSensorZoneHistoryID == null)
                    return null;

                Building building = DataManager.GetBuilding(strMapCode);

                if (building == null)
                {
                    return null;
                }

                Zone zone = DataManager.GetZone(building, strFloorID);

                if (zone == null)
                    return null;

                FireAlarm alarm = new FireAlarm();

                alarm.WebHistoryID = id.Data;
                alarm.EquipCode = strEquipCode;
                alarm.EquipStatus = strEquipStatus;
                alarm.EventID = strEventID;

                DateTime timeStamp;

                if (DateTime.TryParse(strEventTime, out timeStamp))
                    alarm.TimeStamp = timeStamp;

                alarm.EventType = strEventType;
                alarm.Zone = zone;

                int nIndex = strSensorZoneHistoryID.LastIndexOf('_');

                if (nIndex >= 0)
                {
                    string strID = strSensorZoneHistoryID.Substring(nIndex + 1).Trim();

                    int nSensorZoneHistoryID;

                    if (int.TryParse(strID, out nSensorZoneHistoryID))
                        alarm.SensorZoneHistoryID = nSensorZoneHistoryID;
                }

                dicSensorZoneHistoryAlarms[strSensorZoneHistoryID] = alarm;
                //alarms.Add(alarm);
            }

            alarms.AddRange(dicSensorZoneHistoryAlarms.Values.ToList());
            return alarms;
        }

        private FireAlarm FindAlarm(List<FireAlarm> alarms, FireAlarm alarm)
        {
            foreach (FireAlarm _alarm in alarms)
            {
                if (alarm.WebHistoryID == _alarm.WebHistoryID)
                    return _alarm;
            }

            return null;
        }

        private void UpdateGrid(List<FireAlarm> alarms)
        {
            List<DataGridViewRow> removeRows = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in gridAlarms.Rows)
            {
                if (row.IsNewRow || row.Tag == null)
                    continue;

                FireAlarm alarm = FindAlarm(alarms, (FireAlarm)row.Tag);

                if (alarm != null)
                    alarms.Remove(alarm);
                else
                    removeRows.Add(row);
            }

            foreach (DataGridViewRow row in removeRows)
            {
                gridAlarms.Rows.Remove(row);
            }

            if (alarms != null)
            {
                foreach (FireAlarm alarm in alarms)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = alarm;
                    cell.Tag = alarm;
                    row.Cells.Add(cell);
                    row.Tag = alarm;

                    gridAlarms.Rows.Add(row);
                }
            }

            if (gridAlarms.SelectedCells == null || gridAlarms.SelectedCells.Count == 0)
            {
                btnMalf.Enabled = btnReal.Enabled = btnAlarmOff.Enabled = false;
            }
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

        private void btnMalf_Click(object sender, EventArgs e)
        {
            if (gridAlarms.SelectedCells == null || gridAlarms.SelectedCells.Count == 0)
            {
                btnMalf.Enabled = btnReal.Enabled = btnAlarmOff.Enabled = false;
                return;
            }

            FireAlarm alarm = (FireAlarm)gridAlarms.SelectedCells[0].Tag;
            WebServiceManager.SendMalfunction(alarm, m_checkToUnE);
        }

        private void btnReal_Click(object sender, EventArgs e)
        {
            if (gridAlarms.SelectedCells == null || gridAlarms.SelectedCells.Count == 0)
            {
                btnMalf.Enabled = btnReal.Enabled = btnAlarmOff.Enabled = false;
                return;
            }

            FireAlarm alarm = (FireAlarm)gridAlarms.SelectedCells[0].Tag;
            WebServiceManager.SendRealFire(alarm, m_checkToUnE);
        }

        private void btnAlarmOn_Click(object sender, EventArgs e)
        {
            if (treeSensors.SelectedNode == null || treeSensors.SelectedNode.Tag == null)
            {
                btnAlarmOn.Enabled = false;
                return;
            }

            if (treeSensors.SelectedNode.Tag is Zone)
            {
                Zone zone = (Zone)treeSensors.SelectedNode.Tag;
                WebServiceManager.SendAlarmOn(zone);
            }
        }

        private void btnAlarmOff_Click(object sender, EventArgs e)
        {
            if (gridAlarms.SelectedCells == null || gridAlarms.SelectedCells.Count == 0)
            {
                btnMalf.Enabled = btnReal.Enabled = btnAlarmOff.Enabled = false;
                return;
            }

            FireAlarm alarm = (FireAlarm)gridAlarms.SelectedCells[0].Tag;
            WebServiceManager.SendAlarmOff(alarm);
        }

        private void treeSensors_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeSensors.SelectedNode == null || treeSensors.SelectedNode.Tag == null)
                return;

            if (treeSensors.SelectedNode.Tag is Zone)
            {
                btnAlarmOn.Enabled = true;
                btnMalf.Enabled = btnReal.Enabled = btnAlarmOff.Enabled = false;
            }
        }

        private void gridAlarms_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridAlarms.SelectedCells == null || gridAlarms.SelectedCells.Count == 0)
                return;

            btnAlarmOn.Enabled = false;
            btnMalf.Enabled = btnReal.Enabled = btnAlarmOff.Enabled = true;
        }
    }
}
