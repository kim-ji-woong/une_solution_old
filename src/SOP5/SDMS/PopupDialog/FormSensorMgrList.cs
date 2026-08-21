using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;

using System.Collections;
using System.Diagnostics;
using System.Threading;
using SDMS.Help;
namespace SDMS
{
    public partial class FormSensorMgrList : PopupFormBase
    {
        private class Sensor
        {
            private string m_strSensorName = "";
            private SensorType m_type = SensorType.ALL;

            public string Name
            {
                get { return m_strSensorName; }
                set { m_strSensorName = value; }
            }

            public SensorType Type
            {
                get { return m_type; }
                set { m_type = value; }
            }

            public Sensor()
            {
            }

            public Sensor(string strSensorName, SensorType type)
            {
                m_strSensorName = strSensorName;
                m_type = type;
            }

            public override string ToString()
            {
                return m_strSensorName;
            }
        }
        
        private enum SensorType { ALL = 0, DETECT_FIRE = 1, COOLER, PUMP, PSM_SENSOR, DETECT_PROTECT, SECOM }
        private bool editable = false;
        private int m_nSiteID = 0;
                
        Dictionary<int, int> findResultDic = new Dictionary<int, int>();    // row, sensor tag id
        Dictionary<int, Admin.SensorMgrListGridData> store = new Dictionary<int, Admin.SensorMgrListGridData>();          // sensor tag id , DeActivate bool

        private ManualManager m_manualManager = null;

        public FormSensorMgrList()
        {
            this.DoubleBuffered = true;

            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            InitializeComponent();

            FormMain.SetDoubleBuffer(gvSensorMgrList, true);

            storeBtn.Enabled = false;

            this.UseFrmMove = false;
            InitCtrlSize(this);
            SetChildCtrlResize(this, this.Width, this.Height);
            FormMain.Instance.CustomizeGridView(gvSensorMgrList);

            m_manualManager = new ManualManager(this);
            SetManualID();
        }


        private IFacility.FacilityType ToFacilityType(SensorType sensorType)
        {
            switch (sensorType)
            {
                case SensorType.DETECT_FIRE:
                    return IFacility.FacilityType.FIRE_SENSOR;
            }
            return IFacility.FacilityType.NONE;
        }

        private void InitComboBox()
        {           
            foreach (KeyValuePair<int, BuildingGroup> pair in ZoneManager.Instance.DicBuildingGroup)
            {
                cboBuildingGroup.Items.Add(pair.Value);
            }

            cboBuildingGroup.Sorted = true;
            cboBuildingGroup.Sorted = false;
            cboBuildingGroup.Items.Insert(0, "모두");

            cboBuildingGroup.SelectedIndex = 0;
            
            cboSensorType.Items.Add(new Sensor("모두", SensorType.ALL));
            cboSensorType.Items.Add(new Sensor("화재센서", SensorType.DETECT_FIRE));

            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                cboSensorType.Items.Add(new Sensor("유해화학물질센서", SensorType.PSM_SENSOR));                
            }
            if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                cboSensorType.Items.Add(new Sensor("방범센서", SensorType.DETECT_PROTECT));             
            }
            
            cboSensorType.SelectedIndex = 0;
            colDeActivated.ReadOnly = true;
            //colDeActivated.DefaultCellStyle.BackColor = Color.LightGray;

        }
        
        
        private void FormSensorMgrList_Load(object sender, EventArgs e)
        {
            InitGrid();
            InitComboBox();
            searchBtnClick(null, null);
            
        }


        private void InitGrid()
        {
            colNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colName.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colBuildingGroup.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colBuildingGroup.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colBuilding.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colBuilding.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colEZone.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colEZone.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colDeActivated.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
           
            
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
                
        private void searchBtnClick(object sender, EventArgs e)
        {
            search();
        }
        private void search()
        {
            int nSelectedTypeIndex = cboSensorType.SelectedIndex;

            if (nSelectedTypeIndex < 0)
                return;

            Sensor sensor = (Sensor)cboSensorType.Items[nSelectedTypeIndex];

            gvSensorMgrList.DataSource = null;

            store.Clear();
            sensorMgrBindingSource.Clear();
            gvSensorMgrList.Invalidate();
            //gvSensorList.Rows.Clear();            

            int no = 1;

            gvSensorMgrList.Visible = true;

            Dictionary<int, Zone> dicZones = null;
            Dictionary<int, EquipmentZone> dicEquipZones = null;
            GetZoneCondition(ref dicZones, ref dicEquipZones);


            LoadSensorData(ref no, dicEquipZones, sensor.Type);


            gvSensorMgrList.DataSource = sensorMgrBindingSource;

            setSelectRowDeactivateWithGrayColor(); 
        }
        private void setSelectRowDeactivateWithGrayColor()
        {
            IEnumerator enumerator = sensorMgrBindingSource.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Admin.SensorMgrListGridData tmpdata = (Admin.SensorMgrListGridData) enumerator.Current;
                if (tmpdata.SensorDeActivated)
                {
                    gvSensorMgrList.Rows[tmpdata.No-1].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }


            //Admin.SensorMgrListGridData tmpdata = (Admin.SensorMgrListGridData)gvSensorMgrList.Rows[e.RowIndex];
        }

        private void LoadSensorData(ref int no, Dictionary<int, EquipmentZone> dicEquipZones, SensorType sensorType)
        { 
            string strCondition = "";
            
           

            if (sensorType > SensorType.ALL)
            {
                string strTypeCondition = "";

                if (sensorType == SensorType.DETECT_PROTECT)
                {
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.Intrusion_S1).ToString(), false);        //SVMS
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.GeneralIntrusionT1_S1).ToString(), false);   //S1ACCESS
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.ExternalAlarmBell).ToString(), false);       //EMPOLL
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.SecomExternalAlarmBell).ToString(), false);       //여자 비상벨 센서
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.SecomWomenAlarmBell).ToString(), false);       //여자 화장실 센서
                    
                }
                //센서유형콤보박스에 secom을 추가한 경우 이 코드를 사용할 수 있다 (위 방범센서로 현재 통합)
                //else if (sensorType == SensorType.SECOM)          
                //{
                //    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.SecomExternalAlarmBell).ToString(), false);       //여자 비상벨 센서
                //    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.SecomWomenAlarmBell).ToString(), false);       //여자 화장실 센서
                //}
                
                else if (sensorType == SensorType.PSM_SENSOR)
                {
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.PSM_SENSOR).ToString());
                }                
                else
                {
                    IFacility.FacilityType type = ToFacilityType(sensorType);
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)type).ToString());

                    if (type == IFacility.FacilityType.FIRE_SENSOR)
                    {
                        for (IFacility.FacilityType facilityType = IFacility.FacilityType.FireSensor_TypeA; facilityType <= IFacility.FacilityType.FireSensor_MonitoringType; facilityType++)
                        {
                            AddConditionString(ref strTypeCondition, "Type = " + ((int)facilityType).ToString(), false);
                        }
                    }
                }

                AddConditionString(ref strCondition, "(" + strTypeCondition + ")");
            }          
         
            if (strCondition.Length > 0)
                strCondition = "where " + strCondition;

            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string szText = "Select sz.ID, sz.Type, sz.Connected, sz.Zone, sz.Data, sz.EquipZoneID, sti.ID, sti.SensorName, sti.DeActivate from SensorZone as sz " +
                            "INNER JOIN SensorTagInfo as sti on sti.SensorZoneID = sz.ID " +
                            "INNER JOIN Zone as z on z.ID = sz.Zone and z.SiteID = {0} {1} order by sti.DeActivate DESC, sz.ID, sti.SensorName";

            string strSQL = string.Format(szText, m_nSiteID, strCondition);

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nType = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                bool isConnected = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
                int nZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nData = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nEquipZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nTagID = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                string szSensorName = DBUtility.WebDBManager.GetStringField(arrResult[i + 7], "");
                string deActivate = DBUtility.WebDBManager.GetStringField(arrResult[i + 8], "");
                if (nID == 0)
                    continue;

                if (nType < 0 || nZoneID < 0 || nData < 0)
                    continue;

                if (dicEquipZones != null)
                {
                    if (dicEquipZones.ContainsKey(nEquipZoneID) == false)
                        continue;
                }
                                
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                if (zone == null)
                    continue;

                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                string szEquipZoneName = (equipZone != null ? equipZone.DisplayText : "");

                SensorType sensorType2 = GetSensorType(nType);
                string strSensorType = GetSensorTypeString(nType);

                if (strSensorType.Length == 0)
                    continue;

                if (zone.Building == null)
                    AddGridData(ref no, strSensorType, szSensorName, zone.DisplayText, zone.DisplayText, szEquipZoneName, nTagID, deActivate, zone, equipZone, sensorType2);
                else
                    AddGridData(ref no, strSensorType, szSensorName, zone.Building.BuildingGroup.BuildingGroupName, zone.Building.BuildingName, szEquipZoneName, nTagID, deActivate,zone, equipZone, sensorType2);
                    
            }
        }
        private void AddConditionString(ref string strConditionMain, string strConditionItem, bool bAnd = true)
        {
            if (strConditionMain.Length == 0)
                strConditionMain = strConditionItem;
            else
            {
                if (bAnd == true)
                {
                    strConditionMain += " and " + strConditionItem;
                }
                else
                {
                    strConditionMain += " or " + strConditionItem;
                }
            }

        }

        private void AddGridData(ref int no, string strSensorType, string szName, string strBuildingGroupName, string strBuildingName, string strETC, int nTagID, string deActivate, Zone zone, EquipmentZone equipZone, SensorType sensorType)
        {
            if (strBuildingName == "NOT_USE")
                strBuildingName = "실외";

            if (strETC == "NOT_USE")
                strETC = "알수 없음";

            Admin.SensorMgrListGridData data = new Admin.SensorMgrListGridData();

            data.No = no++;
            data.Type = strSensorType;
            data.Name = szName;
            data.BuildingGroupName = strBuildingGroupName;
            data.BuildingName = strBuildingName;
            data.EZoneName = strETC;
            data.TagID = nTagID;
            data.SensorDeActivated = deActivate.StartsWith("N") ? false : true;
            
            data.SensorTypeID = (int)sensorType;
            data.Zone = zone;
            data.EquipmentZone = equipZone;

            sensorMgrBindingSource.Add(data);         
        }
        private void GetZoneCondition(ref Dictionary<int, Zone> dicZones, ref Dictionary<int, EquipmentZone> dicEquipZones)
        {
            dicZones = null;
            dicEquipZones = null;

            int nSelectedBuildingIndex = cboBuilding.SelectedIndex;

            if (nSelectedBuildingIndex > 0)
            {
                object item = cboBuilding.Items[nSelectedBuildingIndex];

                if (item.GetType() == typeof(Building))
                {
                    Building building = (Building)item;

                    
                    ArrayList arrZones = ZoneManager.Instance.GetZoneList(building.ID);

                    if (arrZones.Count > 0)
                    {
                        if (dicZones == null)
                            dicZones = new Dictionary<int, Zone>();

                        if (dicEquipZones == null)
                            dicEquipZones = new Dictionary<int, EquipmentZone>();

                        foreach (Zone zone in arrZones)
                        {
                            dicZones[zone.ID] = zone;

                            List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
                            if (arrEquipZones != null)          //zone은 존재하나 EquipmentZone이 없는 경우가 있음. Zone 46
                            {
                                foreach (EquipmentZone equipZone in arrEquipZones)
                                {
                                    dicEquipZones[equipZone.ID] = equipZone;
                                }
                            }
                            
                        }
                    }
                    
                }
                else
                {
                    Zone zone = (Zone)item;

                    if (dicZones == null)
                        dicZones = new Dictionary<int, Zone>();

                    if (dicEquipZones == null)
                        dicEquipZones = new Dictionary<int, EquipmentZone>();

                    if (zone != null)
                    {
                        dicZones[zone.ID] = zone;

                        List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);

                        foreach (EquipmentZone equipZone in arrEquipZones)
                        {
                            dicEquipZones[equipZone.ID] = equipZone;
                        }
                    }
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

                            List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(pair.Value);

                            if (dicZones == null)
                                dicZones = new Dictionary<int, Zone>();

                            if (dicEquipZones == null)
                                dicEquipZones = new Dictionary<int, EquipmentZone>();

                            if (arrEquipZones != null)
                            {
                                foreach (EquipmentZone equipZone in arrEquipZones)
                                {
                                    dicEquipZones[equipZone.ID] = equipZone;
                                }
                            }

                            dicZones[pair.Key] = pair.Value;
                        }
                    }
                }
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

                            List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
                            AddEquipZoneList(arrEquipZoneList, arrEquipZones);

                            arrZoneList.Add(zone);
                        }

                        strCondition = "ZoneID in (" + strCondition + ")";
                    }
                    
                }
                else
                {
                    Zone zone = (Zone)item;
                    strCondition = "ZoneID = " + zone.ID.ToString();

                    if (arrZoneList == null)
                        arrZoneList = new ArrayList();

                    arrZoneList.Add(zone);

                    List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
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

                            List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(pair.Value);
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


        private SensorType GetSensorType(int nFacilityType)
        {
            if (nFacilityType == (int)IFacility.FacilityType.FIRE_SENSOR ||
                nFacilityType == (int)IFacility.FacilityType.FireSensor_TypeA ||
                nFacilityType == (int)IFacility.FacilityType.FireSensor_TypeB)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.COOLER_SENSOR)
                return SensorType.COOLER;
            else if (nFacilityType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
                return SensorType.PUMP;
            else if (nFacilityType == (int)IFacility.FacilityType.PSM_SENSOR)
                return SensorType.PSM_SENSOR;            
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_Monitoring)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_SensingLine)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_MonitoringType)
                return SensorType.DETECT_FIRE;
            
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_GasEmission)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_ManualControl)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_SiemensType)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_LightType)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.SecomWomenAlarmBell)
                return SensorType.DETECT_PROTECT;
            else if (nFacilityType == (int)IFacility.FacilityType.SecomExternalAlarmBell)
                return SensorType.DETECT_PROTECT;
            else if (nFacilityType == (int)IFacility.FacilityType.ExternalAlarmBell)
                return SensorType.DETECT_PROTECT;
            else if (nFacilityType == (int)IFacility.FacilityType.GeneralIntrusionT1_S1)
                return SensorType.DETECT_PROTECT;
            else if (nFacilityType == (int)IFacility.FacilityType.Intrusion_S1)
                return SensorType.DETECT_PROTECT;
            return SensorType.ALL;
        }

        private string GetSensorTypeString(int nType)
        {
            if (nType == (int)IFacility.FacilityType.FIRE_SENSOR ||
                nType == (int)IFacility.FacilityType.FireSensor_TypeA ||
                nType == (int)IFacility.FacilityType.FireSensor_TypeB)

                return "화재센서";
            else if (nType == (int)IFacility.FacilityType.COOLER_SENSOR)
                return "스프링쿨러";
            else if (nType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
                return "펌프압력";
            else if (nType == (int)IFacility.FacilityType.PSM_SENSOR)
                return "유해화학물질 센서";
            else if (nType == (int)IFacility.FacilityType.DISASTER_PREVENTION_EQUIPMENT)
                return "방재장비";
            else if (nType == (int)IFacility.FacilityType.FireSensor_Monitoring)
                return "감시";
            else if (nType == (int)IFacility.FacilityType.FireSensor_SensingLine)
                return "감지선";
            else if (nType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType)
                return "연기감지기";
            else if (nType == (int)IFacility.FacilityType.FireSensor_MonitoringType)
                return "감시센서";
            else if (nType == (int)IFacility.FacilityType.CCTV)
                return "CCTV";
            else if (nType == (int)IFacility.FacilityType.FE)
                return "소화기";
            else if (nType == (int)IFacility.FacilityType.HD)
                return "소화전";
            else if (nType == (int)IFacility.FacilityType.FA)
                return "발신기";
            else if (nType == (int)IFacility.FacilityType.FR)
                return "수신기";
            else if (nType == (int)IFacility.FacilityType.FireSensor_GasEmission)
                return "가스방출";
            else if (nType == (int)IFacility.FacilityType.FireSensor_ManualControl)
                return "수동조작함";
            else if (nType == (int)IFacility.FacilityType.FireSensor_SiemensType)
                return "지멘스자탐";
            else if (nType == (int)IFacility.FacilityType.FireSensor_LightType)
                return "광선식";
            else if (nType == (int)IFacility.FacilityType.Intrusion_S1)
                return "SVMS";
            else if (nType == (int)IFacility.FacilityType.GeneralIntrusionT1_S1)
                return "Access";
            else if (nType == (int)IFacility.FacilityType.ExternalAlarmBell)
                return "외부 비상벨";
            else if (nType == (int)IFacility.FacilityType.SecomExternalAlarmBell)
                return "외부비상벨";
            else if (nType == (int)IFacility.FacilityType.SecomWomenAlarmBell)
                return "여자화장실비상벨";
            return "";
        }

        private void AddEquipZoneList(Dictionary<EquipmentZone, EquipmentZone> dicEquipZoneTarget, List<EquipmentZone> arrEquipZoneSource)
        {
            if (arrEquipZoneSource == null)
                return;

            foreach (EquipmentZone equipZone in arrEquipZoneSource)
            {
                if (!dicEquipZoneTarget.ContainsKey(equipZone))
                    dicEquipZoneTarget[equipZone] = equipZone;
            }
        }

        private void FormSensorMgrList_VisibleChanged(object sender, EventArgs e)
        {
            
        }
        
        private void store_btnClick(object sender, EventArgs e)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;
            StringBuilder msg = new StringBuilder();
            Dictionary<int, string> storedDic = new Dictionary<int, string>();
            foreach (KeyValuePair<int, Admin.SensorMgrListGridData> pair in store)
            {
                msg.Append(String.Format("Cell at row {0}, column {1} value changed",pair.Key.ToString(), pair.Value.ToString()));
                Debug.WriteLine(msg);
                Admin.SensorMgrListGridData data = (Admin.SensorMgrListGridData)pair.Value;

                string code = data.SensorDeActivated ? "Y" : "N";
                
                //string strSQL = "Update SensorTagInfo set DeActivate = '"+code+"' where ID = " + data.TagID;      
                storedDic.Add(data.TagID, code);
                
                
            }
            if (storedDic.Count > 0)
                NetworkManager.Instance.SendTagChangeDeactivationInfo(storedDic, changeListCallBack);  

        }
        delegate void SearchFunction();


        public void changeListCallBack()
        {
            NetworkManager.Instance.removeChangeEventCallback();
            if (this.InvokeRequired)
            {
                SearchFunction call = new SearchFunction(search);
                this.Invoke(call);
            }
            else
                search();

        }
        private void gvSensorMgrList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            gvSensorMgrList.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void gvSensorMgrList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (editable)
            {   
                if (sensorMgrBindingSource != null && sensorMgrBindingSource.Count > 0)
                {
                    Admin.SensorMgrListGridData tmpdata = (Admin.SensorMgrListGridData)gvSensorMgrList.Rows[e.RowIndex].DataBoundItem;
                    store[tmpdata.TagID] = tmpdata;

                }
            }
                
        }

        private Image imgUnChecked = global::SDMS.Properties.Resources.CheckBox_Default;
        private Image imgChecked = global::SDMS.Properties.Resources.CheckBox_Click;
        private bool isEditCheck = false;

        private void btnEditCheck_Click(object sender, EventArgs e)
        {
            if (isEditCheck)
            {
                isEditCheck = false;
                this.colDeActivated.ReadOnly = true;

                btnEditCheck.ImageNormal = imgUnChecked;
                btnEditCheck.ImageClicked = imgUnChecked;
                btnEditCheck.ImageMouseOver = imgUnChecked;

                storeBtn.Enabled = false;
                editable = false;
            }
            else
            {
                isEditCheck = true;
                this.colDeActivated.ReadOnly = false;

                btnEditCheck.ImageNormal = imgChecked;
                btnEditCheck.ImageClicked = imgChecked;
                btnEditCheck.ImageMouseOver = imgChecked;

                storeBtn.Enabled = true;
                editable = true;
            }    
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(this, "SDMS_Show_SensorMgrList");
            m_manualManager.SetID(searchBtn, "SDMS_Show_SensorMgrList");
            m_manualManager.SetID(storeBtn, "SDMS_Show_SensorMgrList");
            m_manualManager.SetID(gvSensorMgrList, "SDMS_Show_SensorMgrList");
            m_manualManager.ProcessEvent();
        } 
    }
}
