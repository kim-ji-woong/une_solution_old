using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SDMS
{
    public partial class FormSensorList : Form
    {
        class FireEquipmentHistoryData
        {
            private int m_nStatus = 0;
            private string m_strOpinion = "";

            public int Status
            {
                get { return m_nStatus; }
                set { m_nStatus = value; }
            }

            public string Opinion
            {
                get { return m_strOpinion; }
                set { m_strOpinion = value; }
            }
        }

        private enum SensorType { DETECT_FIRE = 1, COOLER, PUMP, CCTV, FE, HD, FA }

        public FormSensorList()
        {
            InitializeComponent();
        }

        private void FormSensorList_Load(object sender, EventArgs e)
        {
            InitGrid();
            InitComboBox();
        }

        private void InitComboBox()
        {
            cboBuildingGroup.Items.Add("모두");

            foreach (KeyValuePair<int, BuildingGroup> pair in ZoneManager.Instance.DicBuildingGroup)
            {
                cboBuildingGroup.Items.Add(pair.Value);
            }

            cboBuildingGroup.SelectedIndex = 0;

            cboSensorType.Items.Add("모두");
            cboSensorType.Items.Add("화재센서");
            cboSensorType.Items.Add("스프링쿨러");
            cboSensorType.Items.Add("펌프압력");
            cboSensorType.Items.Add("CCTV");
            cboSensorType.Items.Add("소화기");
            cboSensorType.Items.Add("소화전");
            cboSensorType.Items.Add("발신기");

            cboStatus.Items.Add("모두");
            cboStatus.Items.Add("정상");
            cboStatus.Items.Add("비정상");

            cboSensorType.SelectedIndex = 0;
            cboStatus.SelectedIndex = 0;
        }

        private void InitGrid()
        {
            colNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colStatus.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colStatus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colBuilding.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colBuilding.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colFloor.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colFloor.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colETC.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colETC.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuildingGroup.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            cboBuilding.Items.Clear();
            cboBuilding.Items.Add("모두");

            if (nSelectedIndex == 0)
            {
                foreach (KeyValuePair<int, Building> pair in ZoneManager.Instance.DicBuildings)
                {
                    cboBuilding.Items.Add(pair.Value);
                }
            }
            else
            {
                BuildingGroup buildingGroup = (BuildingGroup)cboBuildingGroup.Items[nSelectedIndex];

                if (buildingGroup.GroupID > 0)
                {
                    ArrayList arrBuildings = buildingGroup.BuildingList;

                    if (arrBuildings == null)
                        return;

                    foreach (Building building in arrBuildings)
                    {
                        ArrayList arrFloors = building.FloorList;

                        if (arrFloors != null && arrFloors.Count > 0)
                        {
                            // Zone이 하나도 없는 빌딩, 즉 도면이 하나도 없는 빌딩은 콤보박스에 보여주지 않는다.
                            cboBuilding.Items.Add(building);
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<int, Zone> pair in ZoneManager.Instance.DicOutdoorZones)
                    {
                        cboBuilding.Items.Add(pair.Value);
                    }
                }
            }

            if (cboBuilding.Items.Count > 0)
                cboBuilding.SelectedIndex = 0;
        }

        private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuilding.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            cboFloor.Items.Clear();
            cboFloor.Items.Add("모두");

            if (nSelectedIndex > 0)
            {
                Object obj = cboBuilding.Items[nSelectedIndex];
                Type type = obj.GetType();

                if (type == typeof(Building))
                {
                    Building building = (Building)obj;
                    ArrayList arrZones = ZoneManager.Instance.GetZoneList(building.ID);

                    ArrayList arrFloor = new ArrayList();

                    foreach (Zone zone in arrZones)
                    {
                        Floor floor = new Floor(zone.FloorIndex + zone.AddFloor);
                        floor.Zone = zone;
                        arrFloor.Add(floor);
                    }

                    arrFloor.Sort();

                    foreach (Floor floor in arrFloor)
                    {
                        cboFloor.Items.Add(floor);
                    }
                }
                else
                {
                    cboFloor.Items.Clear();
                    cboFloor.Items.Add("-");
                }
            }

            if (cboFloor.Items.Count > 0)
                cboFloor.SelectedIndex = 0;
        }

        private void btnSelectZone_Click(object sender, EventArgs e)
        {
            ArrayList arrZones = null;
            string strCondition = "", strCondition2 = "", strEquipZoneCondition = "";
            string strZoneCondition = GetZoneConditionString(ref arrZones, ref strEquipZoneCondition);

            if (strZoneCondition.Length > 0)
                AddConditionString(ref strCondition, strZoneCondition);

            if (strEquipZoneCondition.Length > 0)
                AddConditionString(ref strCondition2, strEquipZoneCondition);

            int nSelectedTypeIndex = cboSensorType.SelectedIndex;

            gridSensorList.Rows.Clear();

            if (nSelectedTypeIndex == 0)
            {
                LoadSensorData(strCondition2, nSelectedTypeIndex);
                LoadCCTVData(strCondition);
                LoadFireEquipmentData(strCondition, nSelectedTypeIndex, arrZones);
            }
            else if (nSelectedTypeIndex >= (int)SensorType.DETECT_FIRE && nSelectedTypeIndex <= (int)SensorType.PUMP)
                LoadSensorData(strCondition2, nSelectedTypeIndex);
            else if (nSelectedTypeIndex == (int)SensorType.CCTV)
                LoadCCTVData(strCondition);
            else if (nSelectedTypeIndex >= (int)SensorType.FE && nSelectedTypeIndex <= (int)SensorType.FA)
                LoadFireEquipmentData(strCondition, nSelectedTypeIndex, arrZones);
        }

        private void LoadFireEquipmentHistory(Dictionary<int, FireEquipmentHistoryData> dicEquipStatus, int nSelectedTypeIndex)
        {
            string strSQL = "";

            if (nSelectedTypeIndex == 0)
                strSQL = "Select FireEquipmentID, Time, Status, CheckersOpinion from FireEquipmentHistory order by FireEquipmentID";
            else if (nSelectedTypeIndex == (int)SensorType.FE)
                strSQL = "Select FireEquipmentID, Time, Status, CheckersOpinion from FireEquipmentHistory, FireEquipment where FireEquipmentHistory.FireEquipmentID = FireEquipment.ID and FireEquipment.EquipType = 1 order by FireEquipmentID";
            else if (nSelectedTypeIndex == (int)SensorType.HD)
                strSQL = "Select FireEquipmentID, Time, Status, CheckersOpinion from FireEquipmentHistory, FireEquipment where FireEquipmentHistory.FireEquipmentID = FireEquipment.ID and FireEquipment.EquipType = 2 order by FireEquipmentID";
            else if (nSelectedTypeIndex == (int)SensorType.FA)
                strSQL = "Select FireEquipmentID, Time, Status, CheckersOpinion from FireEquipmentHistory, FireEquipment where FireEquipmentHistory.FireEquipmentID = FireEquipment.ID and FireEquipment.EquipType = 3 order by FireEquipmentID";
            else
                return;

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nPrevEquipID = -1;
            DateTime dtPrev = new DateTime();

            int nResultCount = arrResult.Count;
            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nEquipID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DateTime time = DBUtility.WebDBManager.GetDateTimeField(arrResult[i+1], dtDefault);
                int nStatus = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strOpinion = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");

                if (nEquipID < 0)
                    continue;

                if (nStatus < 0 || nStatus > 3)
                    continue;

                if (nPrevEquipID == nEquipID)
                {
                    if (time < dtPrev)
                        continue;

                    dtPrev = time;
                }
                else
                {
                    nPrevEquipID = nEquipID;
                    dtPrev = time;
                }

                FireEquipmentHistoryData data = new FireEquipmentHistoryData();
                data.Status = nStatus;
                data.Opinion = strOpinion;

                dicEquipStatus[nEquipID] = data;
            }
        }

        private void LoadFireEquipmentData(string strCondition, int nSelectedTypeIndex, ArrayList arrZones)
        {
            int nStatusIndex = cboStatus.SelectedIndex;

            Dictionary<int, FireEquipmentHistoryData> dicEquipStatus = new Dictionary<int, FireEquipmentHistoryData>();
            LoadFireEquipmentHistory(dicEquipStatus, nSelectedTypeIndex);

            Dictionary<Zone, ArrayList> dicEquipments = FormMain.Instance.DataManager.ZoneFireEquipments;
            string[] arrStatus = new string[] { "정상", "고장", "수리중", "기타" };

            foreach (KeyValuePair<Zone, ArrayList> pair in dicEquipments)
            {
                if (arrZones != null && !arrZones.Contains(pair.Key))
                    continue;

                foreach (FireEquipment equip in pair.Value)
                {
                    if (nSelectedTypeIndex == (int)SensorType.FE)
                    {
                        if (equip.Type != Facility.FacilityType.FE)
                            continue;
                    }
                    else if (nSelectedTypeIndex == (int)SensorType.HD)
                    {
                        if (equip.Type != Facility.FacilityType.HD)
                            continue;
                    }
                    else if (nSelectedTypeIndex == (int)SensorType.FA)
                    {
                        if (equip.Type != Facility.FacilityType.FA)
                            continue;
                    }

                    int nEquipStatus = 0;
                    string strOpinion = "";

                    if (dicEquipStatus.ContainsKey(equip.ID))
                    {
                        FireEquipmentHistoryData data = dicEquipStatus[equip.ID];
                        nEquipStatus = data.Status;
                        strOpinion = data.Opinion;
                    }

                    if (nStatusIndex == 1)
                    {
                        if (nEquipStatus != 0)
                            continue;
                    }
                    else if (nStatusIndex == 2)
                    {
                        if (nEquipStatus == 0)
                            continue;
                    }

                    string strStatus = arrStatus[nEquipStatus];
                    AddGridData(equip.TypeString, strStatus, equip.Zone.Building == null ? equip.Zone.BroadcastName : equip.Zone.Building.BuildingName, equip.Zone.Floor.ToString(), strOpinion);
                }
            }
        }

        private void LoadCCTVData(string strCondition)
        {
            if (cboStatus.SelectedIndex == 2)
                return;

            if (strCondition.Length > 0)
                strCondition = "where " + strCondition;

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = string.Format("select IPAddr, ZoneID from CCTV {0} order by ZoneID", strCondition);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount-1; i += 2)
            {
                string strIP = DBUtility.WebDBManager.GetStringField(arrResult[i], "");
                int nZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i+1].ToString(), -1);

                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                if (zone == null)
                    continue;

                AddGridData("CCTV", "정상", zone.Building == null ? zone.BroadcastName : zone.Building.BuildingName, zone.Floor.ToString(), strIP);
            }
        }

        private void LoadSensorData(string strCondition, int nSelectedTypeIndex)
        {
            if (nSelectedTypeIndex > 0)
            {
                AddConditionString(ref strCondition, "Type = " + nSelectedTypeIndex.ToString());
            }

            if (cboStatus.SelectedIndex == 1)
            {
                string strDataCondition = "Connected = 1 and Data = 0";
                AddConditionString(ref strCondition, strDataCondition);
            }
            else if (cboStatus.SelectedIndex == 2)
            {
                string strDataCondition = "Connected = 0 or Data = 1";
                AddConditionString(ref strCondition, strDataCondition);
            }

            if (strCondition.Length > 0)
                strCondition = "where " + strCondition;

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

			string strSQL = string.Format("Select ID, Type, Connected, EquipZoneID, Data from SensorZone {0} order by ZoneID", strCondition);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
				int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nType = DBUtility.WebDBManager.GetIntField(arrResult[i+1].ToString(), -1);
                bool isConnected = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
                int nZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nData = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
				if (nID == 0)
					continue;

                if (nType < 0 || nZoneID < 0 || nData < 0)
                    continue;

                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                if (zone == null)
                    continue;

                string strSensorType = GetSensorTypeString(nType);
                string strStatus = GetSensorStatusString(nType, isConnected, nData);
                AddGridData(strSensorType, strStatus, zone.Building == null ? zone.BroadcastName : zone.Building.BuildingName, zone.Floor.ToString(), "");
            }
        }

        private void AddGridData(string strSensorType, string strStatus, string strBuildingName, string strFloor, string strETC)
        {
            int nID = gridSensorList.Rows.Count + 1;

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strSensorType;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strStatus;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strBuildingName;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strFloor;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strETC;
            row.Cells.Add(cell);

            gridSensorList.Rows.Add(row);
        }

        private string GetSensorStatusString(int nType, bool isConnected, int nData)
        {
            string strStatus = "";

            if (nType == 1)
            {
                if (!isConnected)
                    strStatus = "통신 두절";
                else if (nData == 1)
                    strStatus = "화재 감지";
                else
                    strStatus = "정상";
            }
            else if (nType == 2)
            {
                if (!isConnected)
                    strStatus = "통신 두절";
                else if (nData == 1)
                    strStatus = "스프링쿨러 동작중";
                else
                    strStatus = "정상";
            }
            else if (nType == 3)
            {
                if (!isConnected)
                    strStatus = "통신 두절";
                else if (nData == 1)
                    strStatus = "펌프 압력 이상";
                else
                    strStatus = "정상";
            }

            return strStatus;
        }

        private string GetSensorTypeString(int nType)
        {
            if (nType == 1)
                return "화재센서";
            else if (nType == 2)
                return "스프링쿨러";
            //else if (nType == 3)
                return "펌프압력";
        }

        private void AddEquipZoneList(Dictionary<EquipmentZone, EquipmentZone> dicEquipZoneTarget, ArrayList arrEquipZoneSource)
        {
            if (arrEquipZoneSource == null)
                return;

            foreach (EquipmentZone equipZone in arrEquipZoneSource)
            {
                if (!dicEquipZoneTarget.ContainsKey(equipZone))
                    dicEquipZoneTarget[equipZone] = equipZone;
            }
        }

        private string GetZoneConditionString(ref ArrayList arrZoneList, ref string strEquipZoneCondition)
        {
            string strCondition = "";
            int nSelectedBuildingIndex = cboBuilding.SelectedIndex;

            Dictionary<EquipmentZone, EquipmentZone> arrEquipZoneList = new Dictionary<EquipmentZone, EquipmentZone>();

            if (nSelectedBuildingIndex > 0)
            {
                object item = cboBuilding.Items[nSelectedBuildingIndex];

                if (item.GetType() == typeof(Building))
                {
                    Building building = (Building)item;

                    if (cboFloor.SelectedIndex == 0)
                    {
                        ArrayList arrZones = ZoneManager.Instance.GetZoneList(building.ID);

                        if (arrZones.Count > 0)
                        {
                            if (arrZoneList == null)
                                arrZoneList = new ArrayList();

                            foreach (Zone zone in arrZones)
                            {
                                if (strCondition.Length == 0)
                                    strCondition = zone.ID.ToString();
                                else
                                    strCondition += ", " + zone.ID.ToString();

                                ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
                                AddEquipZoneList(arrEquipZoneList, arrEquipZones);

                                arrZoneList.Add(zone);
                            }

                            strCondition = "ZoneID in (" + strCondition + ")";
                        }
                    }
                    else
                    {
                        Floor floor = (Floor)cboFloor.Items[cboFloor.SelectedIndex];
                        strCondition = "ZoneID = " + floor.Zone.ID.ToString();

                        if (arrZoneList == null)
                            arrZoneList = new ArrayList();

                        arrZoneList.Add(floor.Zone);

                        ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(floor.Zone);
                        AddEquipZoneList(arrEquipZoneList, arrEquipZones);
                    }
                }
                else
                {
                    Zone zone = (Zone)item;
                    strCondition = "ZoneID = " + zone.ID.ToString();

                    if (arrZoneList == null)
                        arrZoneList = new ArrayList();

                    arrZoneList.Add(zone);

                    ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
                    AddEquipZoneList(arrEquipZoneList, arrEquipZones);
                }
            }
            else
            {
                if (cboBuildingGroup.SelectedIndex > 0)
                {
                    BuildingGroup group = (BuildingGroup)cboBuildingGroup.Items[cboBuildingGroup.SelectedIndex];

                    string strZoneList = "";

                    foreach (KeyValuePair<int, Zone> pair in ZoneManager.Instance.DicZones)
                    {
                        if ((pair.Value.Building != null && pair.Value.Building.BuildingGroup == group) ||
                            (group.GroupID > 3 && pair.Value.Building == null))
                        {
                            if (strZoneList.Length == 0)
                                strZoneList = pair.Value.ID.ToString();
                            else
                                strZoneList += ", " + pair.Value.ID.ToString();

                            ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(pair.Value);
                            AddEquipZoneList(arrEquipZoneList, arrEquipZones);

                            if (arrZoneList == null)
                                arrZoneList = new ArrayList();

                            arrZoneList.Add(pair.Value);
                        }
                    }

                    if (strZoneList.Length > 0)
                        strCondition = "ZoneID in (" + strZoneList + ")";
                }
            }

            if (arrEquipZoneList.Count > 0)
            {
                foreach (KeyValuePair<EquipmentZone, EquipmentZone> pair in arrEquipZoneList)
                {
                    if (strEquipZoneCondition.Length == 0)
                        strEquipZoneCondition = pair.Key.ID.ToString();
                    else
                        strEquipZoneCondition += ", " + pair.Key.ID.ToString();
                }

                strEquipZoneCondition = "EquipZoneID in (" + strEquipZoneCondition + ")";
            }

            return strCondition;
        }

        private void AddConditionString(ref string strConditionMain, string strConditionItem)
        {
            if (strConditionMain.Length == 0)
                strConditionMain = strConditionItem;
            else
                strConditionMain += " and " + strConditionItem;
        }
    }
}
