using DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using UnE.Spatial;

namespace SDMS.PopupDialog
{
    public partial class FormPSMList : Form, IChildControl
    {
        public enum LifeTimeState { NORMAL = 0, NEED_CHANGE_SENSOR, ALREADY_EXPIRED };

        private class SensorStatus
        {
            private int m_nSensorID = -1;
            private int m_nStatus = -1;
            private VariousData<DateTime> m_nBeginTime = null;
            private VariousData<DateTime> m_nEndTime = null;

            public int SensorID
            {
                get { return m_nSensorID; }
                set { m_nSensorID = value; }
            }

            public int Status
            {
                get { return m_nStatus; }
                set { m_nStatus = value; }
            }

            public VariousData<DateTime> BeginTime
            {
                get { return m_nBeginTime; }
                set { m_nBeginTime = value; }
            }

            public VariousData<DateTime> EndTime
            {
                get { return m_nEndTime; }
                set { m_nEndTime = value; }
            }
        }

        private const int SENSOR_NO_INDEX = 0;
        private const int SENSOR_LOCATION_INDEX = 1;
        private const int SENSOR_MATERIAL_NAME_INDEX = 2;
        private const int SENSOR_OVERFLOW_INDEX = 3;
        private const int SENSOR_ALARM_DEPTH_INDEX = 4;
        private const int SENSOR_CCTV_INDEX = 5;
        private const int SENSOR_ONOFF_INDEX = 6;

        private const int TANK_NO_INDEX = 0;
        private const int TANK_LOCATION_INDEX = 1;
        private const int TANK_NAME_INDEX = 2;
        private const int TANK_ALARM_DEPTH_INDEX = 3;
        private const int TANK_REMAINS_INDEX = 4;
        private const int TANK_CCTV_INDEX = 5;
        private const int TANK_MATERIAL_NAME_INDEX = 6;

        private WebDBManager m_dbmgr = null;
        private bool m_isTankView = false;

        private int m_nLocationColumnIndex = -1;
        private int m_nOnOffColumnIndex = -1;

        // Key : Material ID
        //private Dictionary<int, UnE.PSM.PSMMaterial> m_dicPSMMaterials = new Dictionary<int, UnE.PSM.PSMMaterial>();
        // Key : Sensor ID
        //private Dictionary<int, UnE.PSM.PSMSensor> m_dicPSMSensor = new Dictionary<int, UnE.PSM.PSMSensor>();
        // Key : Tank ID
        //private Dictionary<int, UnE.PSM.PSMTank> m_dicPSMTank = new Dictionary<int, UnE.PSM.PSMTank>();

        // 유해화학물질별 탱크 위치들
        Dictionary<UnE.PSM.PSMMaterial, List<LocationComboBoxItem>> m_dicMaterialLocations = new Dictionary<UnE.PSM.PSMMaterial, List<LocationComboBoxItem>>();

        FormPSMTankDetail m_frmTank = null;
        FormPSMSensorWork m_frmSensor = null;
        FormPSMSensorLifeTime m_frmSensorLifeTime = null;
        FormPSMDepartment m_frmDepartment = null;
        FormPSMSensorAlarm m_frmSensorAlarm = null;

        private Color m_foreNormal = Color.Black;
        private Color m_backNormal = Color.White;
        private Color m_foreOffline = Color.Black;
        private Color m_backOffline = Color.FromArgb(231, 230, 230);
        // 교체 필요(사용기한 임박함)
        private Color m_foreNeedChangeSensor = Color.FromArgb(21, 78, 193);
        private Color m_backNeedChangeSensor = Color.White;
        // 교체 시기가 지났음
        private Color m_foreAlreadyExpiredSensor = Color.FromArgb(230, 70, 26);
        private Color m_backAlreadyExpiredSensor = Color.White;
        private Color m_foreAlarm = Color.White;
        private Color m_backAlarm = Color.FromArgb(192, 0, 0);
        private Color m_foreNotUse = Color.FromArgb(127, 127, 127);
        private Color m_backNotUse = Color.White;

        private Font m_fontBold = null;
        private Font m_fontAlarm = null;
        private Font m_fontExpired = null;
        private Font m_fontNeedChangeSensor = null;
        private Font m_fontNormal = null;
        private Font m_fontNotUse = null;
        private Font m_fontOffline = null;

        // 마지막으로 PSMSensor의 사용기한을 확인한 일자
        private DateTime m_dtLastCheckedPSMSensorLifeTime = new DateTime();
        // 사용기한이 경과하기 몇달전에 알람을 표시할 것인가?
        private int m_nLifeTimeAlertMonth = 1;

        private object LockSensorData
        {
            get { return timer1; }
        }

        public static int DockingWidth
        {
            get { return 651; }
        }


        private static FormPSMList m_Instance = null;
        public static FormPSMList Instance
        {
            get { return FormPSMList.m_Instance; }
        }

        public FormPSMList()
        {

            m_Instance = this;

            this.DoubleBuffered = true;

            InitializeComponent();

            Type dgvType1 = gvPSMSensor.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(gvPSMSensor, true, null);

            Type dgvType2 = this.gvPSMTank.GetType();
            PropertyInfo pi2 = dgvType2.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi2.SetValue(gvPSMTank, true, null);

            m_dbmgr = FormMain.Instance.DBManager;

            m_nLocationColumnIndex = colPSMSensorLocation.Index;
            m_nOnOffColumnIndex = colPSMSensorIsWorking.Index;

            InitEvent();

            LoadGroupData();

            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {

                LoadOptionData();
                LoadTankData();
                LoadSensorData();
            }

            if (UnE.SOP.ProxySOP.Instance.SiteID != 1)
                btnShowSensorManual.Visible = false;
        }

        //public enum Area { NONE = -1, ETC = 0, AREA_12 = 1, AREA_34 = 2, AREA_56 = 3, WATER = 4 }
        private void LoadGroupData()
        {
            CheckBox [] checkItems = { chkETC, chk12, chk34, chk56,chkWater };   
            for( int i = 0 ; i < 5; i++)
            {
                string strSQL = String.Format("SELECT AreaName FROM PSMAreaType where AreaValue = {0}", i);
                ArrayList arrResult = m_dbmgr.GetResultData(strSQL, 0);
                if (arrResult != null && arrResult.Count >= 1)
                {
                    string szValue = WebDBManager.GetStringField(arrResult[0].ToString(), "");

                    if (szValue != null && szValue != "")
                        checkItems[i].Text = szValue;
                }
            }

            CheckBox[] checkItems2 = { chk12, chk34, chk56, chkWater, chkETC};   
            for (int i = 1; i < 5; i++)
            {
                int x = checkItems2[i - 1].Location.X;
                int width = checkItems2[i - 1].Size.Width + 10;

                int y = checkItems2[i].Location.Y;
                checkItems2[i].Location = new Point(x + width, y);

                
            }
        }


        private void InitEvent()
        {
            this.Load += FormPSMList_Load;
            this.FormClosed += FormPSMList_FormClosed;

            rdoTankList.Click += rdoTankList_Click;

            rdoSensorList.Click += rdoSensorList_Click;


            gvPSMTank.CellDoubleClick += gvPSMTank_CellDoubleClick;
            gvPSMSensor.CellDoubleClick += gvPSMSensor_CellDoubleClick;

            btnSearch.Click += btnSearch_Click;
            btnClose.Click += btnClose_Click;
        }

        void rdoSensorList_Click(object sender, EventArgs e)
        {
            ChangeList(false);
        }

        void rdoTankList_Click(object sender, EventArgs e)
        {
            ChangeList(true);
        }

       

        private void LoadOptionData()
        {
            string strSQL = string.Empty;
            ArrayList arrResult = null;

            // 유해물질..
            strSQL = String.Format("SELECT ID, MaterialName, UOM, PageNo, Description FROM PSMMaterial");
            arrResult = m_dbmgr.GetResultData(strSQL, 0);

            if (arrResult != null)
            {
                MaterialComboBoxItem itemDefault = new MaterialComboBoxItem();
                itemDefault.ID = -1;
                itemDefault.MaterialName = "모두";
                cmbPSMMaterial.Items.Add(itemDefault);

                List<MaterialComboBoxItem> items = new List<MaterialComboBoxItem>();

                for (int i = 0; i < arrResult.Count; i += 5)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strMaterialName = WebDBManager.GetStringField(arrResult[i + 1]);
                    string strUOM = WebDBManager.GetStringField(arrResult[i + 2]);
                    int nPageNo = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                    string strDescription = WebDBManager.GetStringField(arrResult[i + 4]);

                    UnE.PSM.PSMMaterial material = GetPSMMaterial(nID, strMaterialName, strUOM, strDescription);

                    if (material != null)
                        material.PageNo = nPageNo;

                    MaterialComboBoxItem item = new MaterialComboBoxItem();
                    item.ID = nID;
                    item.MaterialName = strMaterialName;
                    item.UOM = strUOM;
                    item.Description = strDescription;
                    item.Material = material;

                    items.Add(item);
                    //cmbPSMMaterial.Items.Add(item);
                }

                items.Sort();

                foreach (MaterialComboBoxItem item in items)
                {
                    cmbPSMMaterial.Items.Add(item);
                }

                //cmbPSMMaterial.Sorted = true;
                cmbPSMMaterial.SelectedItem = itemDefault;
            }

            LocationComboBoxItem itemLocation = MakeDefaultLocationItem();
            cmbLocation.Items.Add(itemLocation);

            cmbLocation.SelectedItem = itemLocation;

            // 시내외별..
            /*{
                //모두, 실내, 실외
                InOutComboBoxItem itemAll = new InOutComboBoxItem();
                itemAll.DisplayText = "모두";
                itemAll.Value = -1;
                cmbInOut.Items.Add(itemAll);

                InOutComboBoxItem itemIn = new InOutComboBoxItem();
                itemIn.DisplayText = "실내";
                itemIn.Value = 0;
                cmbInOut.Items.Add(itemIn);

                InOutComboBoxItem itemOut = new InOutComboBoxItem();
                itemOut.DisplayText = "실외";
                itemOut.Value = 1;
                cmbInOut.Items.Add(itemOut);

                cmbInOut.SelectedItem = itemAll;
            }


            // 위치별.. Tank가 있는 EquipZone
            strSQL = string.Empty;
            arrResult = null;

            strSQL = String.Format("SELECT DISTINCT A.ID, A.ZoneName from EquipmentZone AS A INNER JOIN PSMTank AS B ON (A.ID = B.EquipZoneID) WHERE A.SiteID = {0} ORDER BY A.ZoneName ASC", UnE.SOP.ProxySOP.Instance.SiteID);
            arrResult = m_dbmgr.GetResultData(strSQL, 0);

            if (arrResult != null)
            {
                LocationComboBoxItem itemDefault = new LocationComboBoxItem();
                itemDefault.ID = -1;
                itemDefault.LocationName = "모두";
                cmbLocation.Items.Add(itemDefault);

                for (int i = 0; i < arrResult.Count; i += 2)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strLocationName = WebDBManager.GetStringField(arrResult[i + 1]);

                    LocationComboBoxItem item = new LocationComboBoxItem();
                    item.ID = nID;
                    item.LocationName = strLocationName;

                    cmbLocation.Items.Add(item);
                }

                cmbLocation.SelectedItem = itemDefault;
            }*/


            // 센서작동 유무..
            {
                //모두, ON, OFF, 작업중OFF(사용 안함)
                OnOffComboBoxItem itemAll = new OnOffComboBoxItem();
                itemAll.Status = UnE.PSM.PSMSensor.Status.Unknown;
                itemAll.DisplayText = "모두";
                itemAll.Value = -1;
                cmbOnOff.Items.Add(itemAll);

                OnOffComboBoxItem itemOn = new OnOffComboBoxItem();
                itemOn.Status = UnE.PSM.PSMSensor.Status.On;
                itemOn.DisplayText = "ON";
                itemOn.Value = 0;
                cmbOnOff.Items.Add(itemOn);

                OnOffComboBoxItem itemOff = new OnOffComboBoxItem();
                itemOff.Status = UnE.PSM.PSMSensor.Status.Off;
                itemOff.DisplayText = "OFF";
                itemOff.Value = 1;
                cmbOnOff.Items.Add(itemOff);

                //OnOffComboBoxItem itemWorkingOut = new OnOffComboBoxItem();
                //itemWorkingOut.DisplayText = "작업중OFF";
                //itemWorkingOut.Value = 2;
                //cmbOnOff.Items.Add(itemWorkingOut);

                cmbOnOff.SelectedItem = itemAll;
            }
        }

        private LocationComboBoxItem MakeDefaultLocationItem()
        {
            LocationComboBoxItem itemLocation = new LocationComboBoxItem();
            itemLocation.ID = -1;
            itemLocation.LocationName = "모두";
            return itemLocation;
        }

        private void LoadTankData()
        {
            lock (LockSensorData)
            {
                m_dicMaterialLocations.Clear();
                gvPSMTank.Rows.Clear();

                string strSQL = string.Empty;
                ArrayList arrResult = null;

                LocationComboBoxItem currentLocationItem = (LocationComboBoxItem)cmbLocation.SelectedItem;
                MaterialComboBoxItem currentMaterialItem = (MaterialComboBoxItem)cmbPSMMaterial.SelectedItem;
                cmbLocation.Items.Clear();

                strSQL += String.Format("SELECT A.ID, A.TankName, A.EquipZoneID, B.ZoneName, A.MaterialType, C.MaterialName, ");
                strSQL += String.Format("A.Capacity, A.Remains, A.UnitName, C.ID, A.LocationName, A.BroadcastName ");
                strSQL += String.Format("FROM PSMTank AS A ");
                strSQL += String.Format("INNER JOIN EquipmentZone AS B ON (A.EquipZoneID = B.ID) ");
                strSQL += String.Format("INNER JOIN PSMMaterial AS C ON (A.MaterialType = C.ID) ");
                /*strSQL += String.Format("WHERE (A.LocationName = '{0}' or {1} = -1)", currentLocationItem.LocationName, currentLocationItem.ID);
                //strSQL += String.Format("WHERE (B.ID = {0} OR {0} = -1) ", (cmbLocation.SelectedItem as LocationComboBoxItem).Value);
                strSQL += String.Format("AND (C.ID = {0} OR {0} = -1) ", (cmbPSMMaterial.SelectedItem as MaterialComboBoxItem).Value);*/

                /*int nInOut = int.Parse((cmbInOut.SelectedItem as InOutComboBoxItem).Value.ToString());
                if (nInOut > -1)
                {
                    string strInOut = (nInOut == 0 ? "실내" : "실외");
                    strSQL += String.Format("AND (B.LocationName LIKE '%{0}')", strInOut);
                }*/

                arrResult = m_dbmgr.GetResultData(strSQL, 0);

                if (arrResult != null)
                {
                    int nRowCount = 0;
                    DataGridViewRow row = null;
                    List<LocationComboBoxItem> locationItems = new List<LocationComboBoxItem>();

                    for (int i = 0; i < arrResult.Count; i += 12)
                    {
                        int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                        string strTankName = WebDBManager.GetStringField(arrResult[i + 1]);
                        int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                        string strZoneName = WebDBManager.GetStringField(arrResult[i + 3]);
                        int nMaterialType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                        string strMaterialName = WebDBManager.GetStringField(arrResult[i + 5]);
                        DBUtility.VariousData<float> fCapacity = WebDBManager.GetFloatField(arrResult[i + 6].ToString());
                        DBUtility.VariousData<float> fRemains = WebDBManager.GetFloatField(arrResult[i + 7].ToString());
                        string strUnitName = WebDBManager.GetStringField(arrResult[i + 8]);
                        int nMaterialID = WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);
                        string strLocationName = WebDBManager.GetStringField(arrResult[i + 10]);
                        string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 11]);

                        UnE.PSM.PSMTank tank = GetPSMTank(nID, strTankName, nEquipZoneID, nMaterialID);
                        UnE.PSM.PSMMaterial material = PSMManager.Instance.GetMaterial(nMaterialID);
                        List<LocationComboBoxItem> locations = null;

                        if (material != null)
                        //if (m_dicPSMMaterials.TryGetValue(nMaterialID, out material))
                        {
                            if (!m_dicMaterialLocations.TryGetValue(material, out locations))
                            {
                                locations = new List<LocationComboBoxItem>();
                                m_dicMaterialLocations[material] = locations;
                            }

                            LocationComboBoxItem item = new LocationComboBoxItem();
                            item.ID = nEquipZoneID;
                            item.LocationName = strLocationName;

                            if (!locationItems.Contains(item))
                                locationItems.Add(item);

                            if (!locations.Contains(item))
                                locations.Add(item);
                        }

                        switch (tank.AreaType)
                        {
                            case UnE.PSM.PSMTank.Area.AREA_12:
                                if (chk12.Checked == false)
                                    continue;

                                break;

                            case UnE.PSM.PSMTank.Area.AREA_34:
                                if (chk34.Checked == false)
                                    continue;

                                break;

                            case UnE.PSM.PSMTank.Area.AREA_56:
                                if (chk56.Checked == false)
                                    continue;

                                break;

                            case UnE.PSM.PSMTank.Area.WATER:
                                if (chkWater.Checked == false)
                                    continue;

                                break;

                            case UnE.PSM.PSMTank.Area.ETC:
                                if (chkETC.Checked == false)
                                    continue;

                                break;

                            default:
                                break;

                        }

                        if (currentLocationItem != null &&
                            (strLocationName == currentLocationItem.LocationName || currentLocationItem.ID == -1) &&
                            (nMaterialID == currentMaterialItem.ID || currentMaterialItem.ID == -1))
                        {
                            row = MakeNewRow(gvPSMTank);

                            if (strLocationName == null)
                                strLocationName = "";

                            row.Cells[TANK_NO_INDEX].Value = ++nRowCount;
                            row.Cells[TANK_LOCATION_INDEX].Value = /*strZoneName + */strLocationName;
                            row.Cells[TANK_NAME_INDEX].Value = strTankName; ;
                            row.Cells[TANK_ALARM_DEPTH_INDEX].Value = "-";
                            row.Cells[TANK_CCTV_INDEX].Value = "보기";
                            row.Cells[TANK_MATERIAL_NAME_INDEX].Value = strMaterialName;
                            row.Cells[TANK_REMAINS_INDEX].Value = GetTankRemainString(fCapacity, fRemains, strUnitName);
                            
                            row.Tag = tank;
                        }

                        if (tank != null)
                        {
                            tank.BroadcastName = strBroadcastName;
                            tank.LocationName = strLocationName;
                        }

                    }

                    if (currentMaterialItem.ID < 0)
                    {
                        locationItems.Sort();

                        foreach (LocationComboBoxItem item in locationItems)
                        {
                            cmbLocation.Items.Add(item);
                        }
                    }
                    else
                    {
                        List<LocationComboBoxItem> locationList = null;

                        if (m_dicMaterialLocations.TryGetValue(currentMaterialItem.Material, out locationList))
                        {
                            locationList.Sort();

                            foreach (LocationComboBoxItem item in locationList)
                            {
                                cmbLocation.Items.Add(item);
                            }
                        }
                    }

                    if (cmbLocation.Items.Count > 0)
                    {
                        cmbLocation.Items.Insert(0, MakeDefaultLocationItem());

                        if (currentLocationItem != null && cmbLocation.Items.Contains(currentLocationItem))
                            cmbLocation.SelectedItem = currentLocationItem;
                        else
                            cmbLocation.SelectedIndex = 0;
                    }
                }
            }
        }

        private string GetTankRemainString(DBUtility.VariousData<float> fCapacity, DBUtility.VariousData<float> fRemains, string strUnitName)
        {
            string strRemains = "";

            if (fCapacity != null && fRemains != null && strUnitName != null)
            {
                // 0보다 작으면 수동으로 입력한 값
                float data = fRemains.Data < 0.0f ? -fRemains.Data : fRemains.Data;

                //strRemains = String.Format("{0}/{1}({2})", GetGasVolumeString(fRemains), GetGasVolumeString(fCapacity), strUnitName);
                strRemains = String.Format("{0} {1}", GetGasVolumeString(data), strUnitName);
            }
            else
                strRemains = "-";

            return strRemains;
        }

        public static DataGridViewRow MakeNewRow(DataGridView grid)
        {
            int nRowIndex = grid.Rows.Add();

            if (nRowIndex < 0)
                return null;

            return grid.Rows[nRowIndex];
        }

        public static string GetGasVolumeString(float fVolume)
        {
            string str = string.Format("{0:N}", fVolume);

            int nIndex = str.LastIndexOf('.');

            if (nIndex < 0)
                return str;

            // 소숫점 이후에 마지막 끝자리가 0으로 끝나지 않도록 한다.
            while (str[str.Length - 1] == '0')
            {
                str = str.Remove(str.Length - 1);
            }

            if (str[str.Length - 1] == '.')
                str = str.Remove(str.Length - 1);

            //if (str.EndsWith("0"))
            //    return str.Substring(0, str.Length - 2);

            return str;
        }

        private List<int> ReadLocalOffList()
        {
            string strLocalOffList = m_dbmgr.LoadIni(FormPSMSensorWork.LocalOffTag, FormPSMSensorWork.LocalOffSection).Trim();

            List<int> localOffSensorIDList = new List<int>();

            if (strLocalOffList.Length == 0)
                return localOffSensorIDList;

            string[] sensorIDList = strLocalOffList.Split(',');

            foreach (string strID in sensorIDList)
            {
                int nSensorID;

                if (int.TryParse(strID.Trim(), out nSensorID))
                {
                    localOffSensorIDList.Add(nSensorID);
                }
            }

            return localOffSensorIDList;
        }

        private Dictionary<int, SensorStatus> LoadSensorStatusData()
        {
            List<int> localOffSensorIDList = ReadLocalOffList();

            string strSQL = "Select SensorID, Status, BeginTime, EndTime from PSMSensorSchedule";
            ArrayList arrResult = m_dbmgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            Dictionary<int, SensorStatus> dicSensorStatus = new Dictionary<int, SensorStatus>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                int nSensorID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nStatus = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                VariousData<DateTime> dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                VariousData<DateTime> dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 3]);                

                SensorStatus status = new SensorStatus();
                status.SensorID = nSensorID;
                status.Status = nStatus;
                status.BeginTime = dtBegin;
                status.EndTime = dtEnd;

                if (localOffSensorIDList.Contains(nSensorID))
                    status.Status = (int)UnE.PSM.PSMSensor.Status.LocalOff;

                dicSensorStatus[nSensorID] = status;
            }

            return dicSensorStatus;
        }

        private void LoadSensorData()
        {
            lock (LockSensorData)
            {
                gvPSMSensor.Rows.Clear();

                string strSQL = "select ps.ID, TankIDList, ps.SensorName, X, Y, CurrentData, LimitLevel1, LimitLevel2, LimitLevel3, CurrentLevel, ssi.ReciverID, sti.TagNo ";
                strSQL += "from PSMSensor as ps, SensorZone as sz, SensorTagInfo as sti, SensorServerInfo as ssi ";
                strSQL += string.Format("where sz.OrgSensorID = ps.ID and sz.Type = {0} and sti.SensorZoneID = sz.ID and sti.SensorServerID = ssi.ID order by ssi.ReciverID, sti.TagNo", (int)UnE.Sensor.IFacility.FacilityType.PSM_SENSOR);
                //string strSQL = "Select ID, TankIDList, SensorName, X, Y, CurrentData, LimitLevel1, LimitLevel2, LimitLevel3, CurrentLevel from PSMSensor";
                ArrayList arrResult = m_dbmgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return;

                MaterialComboBoxItem currentMaterialItem = (MaterialComboBoxItem)cmbPSMMaterial.SelectedItem;
                LocationComboBoxItem currentLocationItem = (LocationComboBoxItem)cmbLocation.SelectedItem;
                OnOffComboBoxItem currentStatusItem = (OnOffComboBoxItem)cmbOnOff.SelectedItem;

                if (currentMaterialItem == null || currentLocationItem == null || currentStatusItem == null)
                    return;

                Dictionary<int, SensorStatus> dicSensorStatus = LoadSensorStatusData();

                int nRowCount = 0;
                int nResultCount = arrResult.Count;
                DataGridViewRow row = null;
                SensorStatus sensorStatus = null;

                for (int i = 0; i < nResultCount - 11; i += 12)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strTankIDList = WebDBManager.GetStringField(arrResult[i + 1]);
                    string strSensorName = WebDBManager.GetStringField(arrResult[i + 2]);
                    VariousData<float> x = WebDBManager.GetFloatField(arrResult[i + 3].ToString());
                    VariousData<float> y = WebDBManager.GetFloatField(arrResult[i + 4].ToString());
                    float fCurrentData = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), -1);
                    float fLimitLevel1 = WebDBManager.GetFloatField(arrResult[i + 6].ToString(), -1);
                    float fLimitLevel2 = WebDBManager.GetFloatField(arrResult[i + 7].ToString(), -1);
                    float fLimitLevel3 = WebDBManager.GetFloatField(arrResult[i + 8].ToString(), -1);
                    VariousData<int> currentLevel = WebDBManager.GetIntField(arrResult[i + 9].ToString());
                    // 망번호
                    int nReceiverID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                    // 센서 회선번호
                    int nTagNo = WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);

                    UnE.PSM.PSMSensor sensor = GetPSMSensor(nID, strTankIDList, strSensorName, fCurrentData, fLimitLevel1, fLimitLevel2, fLimitLevel3);
                    UnE.PSM.PSMTank tank = GetFirstTank(sensor);
                    UnE.PSM.PSMMaterial material = tank != null ? tank.Material : null;

                    switch (tank.AreaType)
                    {
                        case UnE.PSM.PSMTank.Area.AREA_12:
                            if (chk12.Checked == false)
                                continue;

                            break;

                        case UnE.PSM.PSMTank.Area.AREA_34:
                            if (chk34.Checked == false)
                                continue;

                            break;

                        case UnE.PSM.PSMTank.Area.AREA_56:
                            if (chk56.Checked == false)
                                continue;

                            break;

                        case UnE.PSM.PSMTank.Area.WATER:
                            if (chkWater.Checked == false)
                                continue;

                            break;

                        case UnE.PSM.PSMTank.Area.ETC:
                            if (chkETC.Checked == false)
                                continue;

                            break;

                        default:
                            break;

                    }

                    sensor.ReceiverID = nReceiverID;
                    sensor.TagNo = nTagNo;

                    if (!dicSensorStatus.TryGetValue(nID, out sensorStatus))
                        sensorStatus = null;
                    else
                    {
                        sensor.SensorStatus = UnE.PSM.PSMSensor.ToStatus(sensorStatus.Status);
                        sensor.BeginWorkTime = sensorStatus.BeginTime;
                        sensor.EndWorkTime = sensorStatus.EndTime;
                    }

                    if (x != null && y != null && sensor != null)
                    {
                        sensor.Position = new UnE.Geometry.Vertex2D(x.Data, y.Data);
                    }

                    if ((currentMaterialItem.Material == material || currentMaterialItem.ID < 0) &&
                        (currentLocationItem.LocationName == tank.LocationName || currentLocationItem.ID < 0) &&
                        (currentStatusItem.IsSame(sensor.SensorStatus) == true))
                    {
                        row = MakeNewRow(gvPSMSensor);

                        row.Cells[SENSOR_NO_INDEX].Value = ++nRowCount;
                        row.Cells[SENSOR_LOCATION_INDEX].Value = GetSensorLocationName(sensor, tank);
                        //row.Cells[SENSOR_LOCATION_INDEX].Value = tank != null && tank.EquipZone != null ? tank.EquipZone.ZoneName : "-";
                        row.Cells[SENSOR_MATERIAL_NAME_INDEX].Value = material != null ? material.Name : "-";
                        row.Cells[SENSOR_MATERIAL_NAME_INDEX].Tag = material;
                        row.Cells[SENSOR_OVERFLOW_INDEX].Value = String.Format("{0:F1} {1}", fCurrentData, material != null ? material.UOM : "");
                        row.Cells[SENSOR_ALARM_DEPTH_INDEX].Value = MakeAlarmDepthString(currentLevel); ;
                        row.Cells[SENSOR_ALARM_DEPTH_INDEX].Tag = currentLevel;
                        row.Cells[SENSOR_CCTV_INDEX].Value = "보기";
                        SetSensorStatusCellValue(row.Cells[SENSOR_ONOFF_INDEX], sensor);

                        //gvPSMSensor.Rows.Add(row);
                        row.Tag = sensor;
                    }
                }
            }
        }

        private string GetSensorLocationName(UnE.PSM.PSMSensor sensor, UnE.PSM.PSMTank tank)
        {
            if (tank == null)
                return "-";

            int nIndex = sensor.Name.LastIndexOf('-');
            string strTagNo = "";

            if (nIndex > 0)
            {
                string str = sensor.Name.Substring(nIndex + 1).Trim();
                int nTagNo;

                if (int.TryParse(str, out nTagNo))
                    strTagNo = nTagNo.ToString();
            }

            if (strTagNo.Length > 0)
                return string.Format("[{0}]{1} - {2}", sensor.ReceiverID, tank.LocationName, strTagNo);

            return string.Format("[{0}]{1}", sensor.ReceiverID, tank.LocationName);
        }

        private UnE.PSM.PSMTank GetFirstTank(UnE.PSM.PSMSensor sensor)
        {
            foreach (UnE.PSM.PSMTank tank in sensor.LinkedTankList)
            {
                return tank;
            }
            return null;
        }      

        private UnE.PSM.PSMMaterial GetPSMMaterial(int nID, string strMaterialName, string strUOM, string strDescription)
        {
            UnE.PSM.PSMMaterial material = PSMManager.Instance.GetMaterial(nID);

            if (material == null)
            {
                material = new UnE.PSM.PSMMaterial();

                material.ID = nID;
                material.Name = strMaterialName;
                material.UOM = strUOM;

                PSMManager.Instance.AddMaterial(material);
            }
            return material;
        }

        public UnE.PSM.PSMMaterial GetPSMMaterial(int nID)
        {
            return PSMManager.Instance.GetMaterial(nID);    
        }

        private UnE.PSM.PSMTank GetPSMTank(int nID, string strTankName, int nEquipZoneID, int nMaterialID)
        {
            UnE.PSM.PSMTank tank = PSMManager.Instance.GetTank(nID);

            if (tank != null)
                return tank;

            tank = new UnE.PSM.PSMTank();

            tank.ID = nID;
            tank.Name = strTankName;
            tank.EquipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
            tank.Material = GetPSMMaterial(nMaterialID);

            PSMManager.Instance.AddTank(tank);
            return tank;
        }

        public UnE.PSM.PSMTank GetPSMTank(int nID)
        {
            return PSMManager.Instance.GetTank(nID);
        }

        private UnE.PSM.PSMSensor GetPSMSensor(int nID, string strTankIDList, string strSensorName, float fCurrentData, float fLimitLevel1, float fLimitLevel2, float fLimitLevel3)
        {
            UnE.PSM.PSMSensor sensor = PSMManager.Instance.GetSensor(nID);

            if (sensor == null)
            {
                sensor = new UnE.PSM.PSMSensor();
                sensor.ID = nID;
                PSMManager.Instance.AddSensor(sensor);
            }

            sensor.Name = strSensorName;
            sensor.CurrentData = fCurrentData;
            sensor.LimitLevel1 = fLimitLevel1;
            sensor.LimitLevel2 = fLimitLevel2;
            sensor.LimitLevel3 = fLimitLevel3;

            List<UnE.PSM.PSMTank> tankList = GetPSMTankList(strTankIDList);

            foreach (UnE.PSM.PSMTank tank in tankList)
            {
                sensor.AddTank(tank);
            }
            return sensor;
        }

        public UnE.PSM.PSMSensor GetPSMSensor(int nID)
        {
            return PSMManager.Instance.GetSensor(nID);
        }

        private List<UnE.PSM.PSMTank> GetPSMTankList(string strTankIDList)
        {
            int nID;
            string[] ids = strTankIDList.Split(',');

            List<UnE.PSM.PSMTank> tankList = new List<UnE.PSM.PSMTank>();

            foreach (string id in ids)
            {
                if (!int.TryParse(id.Trim(), out nID))
                    continue;

                UnE.PSM.PSMTank tank = GetPSMTank(nID);
                tankList.Add(tank);
            }

            return tankList;
        }

        private void ChangeList(bool isTankView)
        {
            m_isTankView = isTankView;

            rdoTankList.Checked = isTankView;
            rdoSensorList.Checked = !isTankView;

            gvPSMTank.Visible = isTankView;
            gvPSMSensor.Visible = !isTankView;

            lblOnOff.Visible =
            cmbOnOff.Visible = !isTankView;
        }

        private void PopSensorState(UnE.PSM.PSMSensor sensor, int nSensorNo, string strLocationName, string strMaterialName)
        {
            CloseOtherPopupFrame(m_frmSensor);

            if (m_frmSensor == null || m_frmSensor.IsDisposed)
            {
                m_frmSensor = new FormPSMSensorWork(sensor, nSensorNo, strLocationName, strMaterialName);

                Point pt = this.PointToScreen(new Point(0, 0));
                m_frmSensor.StartPosition = FormStartPosition.Manual;
                m_frmSensor.Location = new Point(
                    pt.X + Convert.ToInt32(Math.Round((this.Width - m_frmSensor.Width) / 1.9)),
                    pt.Y + Convert.ToInt32(Math.Round((this.Height - m_frmSensor.Height) / 1.9)));

                m_frmSensor.BeginSaveAllSensorWorkEvent += m_frmSensor_BeginSaveAllSensorWorkEvent;
            }
            else
            {
                m_frmSensor.Sensor = sensor;
                m_frmSensor.SensorNo = nSensorNo;
                m_frmSensor.LocationName = strLocationName;
                m_frmSensor.MaterialName = strMaterialName;
            }

            m_frmSensor.LoadData();

            if (m_frmSensor.Visible)
                m_frmSensor.Focus();
            else
                m_frmSensor.Show(this);
        }

        private List<UnE.PSM.PSMSensor> m_frmSensor_BeginSaveAllSensorWorkEvent()
        {
            List<UnE.PSM.PSMSensor> liSensors = new List<UnE.PSM.PSMSensor>();

            foreach (DataGridViewRow row in gvPSMSensor.Rows)
            {
                if (row.Tag == null) continue;
                if (row.Tag is UnE.PSM.PSMSensor == false) continue;

                UnE.PSM.PSMSensor sensor = row.Tag as UnE.PSM.PSMSensor;

                if (liSensors.Contains(sensor) == false)
                    liSensors.Add(sensor);
            }
            return liSensors;
        }

        private void CloseOtherPopupFrame(Form form)
        {
            if( form != m_frmSensor)
            {
                if (m_frmSensor != null && !m_frmSensor.IsDisposed)
                    m_frmSensor.Close();
            }
            if (form != m_frmSensorLifeTime)
            {
                if (m_frmSensorLifeTime != null && !m_frmSensorLifeTime.IsDisposed)
                    m_frmSensorLifeTime.Close();
            }
            if (form != m_frmTank)
            {
                if (m_frmTank != null && !m_frmTank.IsDisposed)
                    m_frmTank.Close();
            }
            if (form != m_frmDepartment)
            {
                if (m_frmDepartment != null && !m_frmDepartment.IsDisposed)
                    m_frmDepartment.Close();
            }
            if (form != m_frmSensorAlarm)
            {
                if (m_frmSensorAlarm != null && !m_frmSensorAlarm.IsDisposed)
                    m_frmSensorAlarm.Close();
            }
        }

        private void PopTankDetail(UnE.PSM.PSMTank tank)
        {
            CloseOtherPopupFrame(m_frmTank);

            if (m_frmTank == null || m_frmTank.IsDisposed)
            {
                m_frmTank = new FormPSMTankDetail(tank);

                Point pt = this.PointToScreen(new Point(0, 0));
                m_frmTank.StartPosition = FormStartPosition.Manual;
                m_frmTank.Location = new Point(pt.X - m_frmTank.Width, pt.Y + 200);
            }
            else
            {
                m_frmTank.Tank = tank;
            }

            if (m_frmTank.Visible)
            {
                m_frmTank.Focus();
            }
            else
            {
                m_frmTank.Show(this);
            }
        }

        private void PopDepartment(UnE.PSM.PSMSensor sensor)
        {            
            CloseOtherPopupFrame(m_frmDepartment);

            if (m_frmDepartment == null || m_frmDepartment.IsDisposed)
            {
                m_frmDepartment = new FormPSMDepartment(sensor);

                Point pt = this.PointToScreen(new Point(0, 0));
                m_frmDepartment.StartPosition = FormStartPosition.Manual;
                m_frmDepartment.Location = new Point(pt.X - m_frmDepartment.Width, pt.Y + 200);
            }
            else
            {
                m_frmDepartment.Sensor = sensor;
            }

            if (m_frmDepartment.Visible)
            {
                m_frmDepartment.Focus();
            }
            else
            {
                m_frmDepartment.Show(this);
            }
        }

        private void PopSensorLifeTime(UnE.PSM.PSMSensor sensor, int nSensorNo)
        {
            CloseOtherPopupFrame(m_frmSensorLifeTime);

            if (m_frmSensorLifeTime == null || m_frmSensorLifeTime.IsDisposed)
            {
                m_frmSensorLifeTime = new FormPSMSensorLifeTime(sensor, nSensorNo);

                Point pt = this.PointToScreen(new Point(0, 0));
                m_frmSensorLifeTime.StartPosition = FormStartPosition.Manual;
                m_frmSensorLifeTime.Location = new Point(pt.X - m_frmSensorLifeTime.Width, pt.Y + 200);
            }
            else
            {
                m_frmSensorLifeTime.SetData(sensor, nSensorNo);
            }

            if (m_frmSensorLifeTime.Visible)
            {
                m_frmSensorLifeTime.Focus();
            }
            else
            {
                m_frmSensorLifeTime.Show(this);
            }
        }

        public void SetPSMSensorStatus(int nSensorID, byte status, long beginWorkTime, long endWorkTime)
        {
            UnE.PSM.PSMSensor sensor = GetPSMSensor(nSensorID);
            
            if (sensor != null)
            //if (m_dicPSMSensor.TryGetValue(nSensorID, out sensor))
            {
                UnE.PSM.PSMSensor.Status _status = UnE.PSM.PSMSensor.ToStatus((int)status);

                bool spread = _status == UnE.PSM.PSMSensor.Status.Off4Work || sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off4Work;
                sensor.SensorStatus = _status;

                if (beginWorkTime != 0)
                {
                    DateTime dtBegin = DateTime.FromBinary(beginWorkTime);
                    sensor.BeginWorkTime = new VariousData<DateTime>(dtBegin);
                }
                else
                    sensor.BeginWorkTime = null;

                if (endWorkTime != 0)
                {
                    DateTime dtEnd = DateTime.FromBinary(endWorkTime);
                    sensor.EndWorkTime = new VariousData<DateTime>(dtEnd);
                }
                else
                    sensor.EndWorkTime = null;

                if (spread)
                {
                    List<UnE.PSM.PSMSensor> sensors = sensor.GetSameSensors();
                    sensors.Add(sensor);
                    SetPSMSensorStatus(sensors);
                }
                else
                    SetPSMSensorStatus(sensor);
            }
        }

        private void SetPSMSensorStatus(List<UnE.PSM.PSMSensor> sensors)
        {
            foreach (UnE.PSM.PSMSensor sensor in sensors)
            {
                SetPSMSensorStatus(sensor);
            }
        }

        private void SetPSMSensorStatus(UnE.PSM.PSMSensor sensor)
        {
            foreach (DataGridViewRow row in gvPSMSensor.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if ((UnE.PSM.PSMSensor)row.Tag == sensor)
                {
                    SetSensorStatusCellValue(row.Cells[SENSOR_ONOFF_INDEX], sensor);
                    break;
                }
            }
        }

        // Return 값 : true이면 sensor is on
        //             false이면 sensor is not on
        private bool SetSensorStatusCellValue(DataGridViewCell cell, UnE.PSM.PSMSensor sensor)
        {
            UnE.PSM.PSMSensor.Status status = sensor.SensorStatus;

            if (status == UnE.PSM.PSMSensor.Status.On)
            {
                cell.Value = "On";
                return true;
            }
            else if (status == UnE.PSM.PSMSensor.Status.Off || status == UnE.PSM.PSMSensor.Status.LocalOff)
                cell.Value = "Off";
            else if (status == UnE.PSM.PSMSensor.Status.Off4Work)
            {
                DateTime dtNow = DateTime.Now;

                if (sensor.BeginWorkTime != null && sensor.EndWorkTime != null &&
                    dtNow >= sensor.BeginWorkTime.Data && dtNow <= sensor.EndWorkTime.Data)
                    cell.Value = "작업중";
                else
                {
                    cell.Value = "On";
                    return true;
                }
            }
            else
                cell.Value = "-";

            return false;
        }

        public void OnAdded(Control parent)
        {

        }

        public void OnRemoved(Control parent)
        {
            if (m_frmTank != null && !m_frmTank.IsDisposed)
                m_frmTank.Close();

            if (m_frmSensor != null && !m_frmSensor.IsDisposed)
                m_frmSensor.Close();

            if (m_frmSensorLifeTime != null && !m_frmSensorLifeTime.IsDisposed)
                m_frmSensorLifeTime.Close();

            if (m_frmSensorAlarm != null && !m_frmSensorAlarm.IsDisposed)
                m_frmSensorAlarm.Close();
        }



        private void timer1_Tick(object sender, EventArgs e)
        {

            
            //lock (LockSensorData)

            if (m_bUpdateSensor == false)
            {
                m_bUpdateSensor = true;

                Dictionary<UnE.PSM.PSMSensor, VariousData<int>> dicSensorIDLevels = UpdateSensors();
                UpdateTanks(dicSensorIDLevels);

                // PSMSensor의 사용기한을 마지막으로 확인한 이후 날짜가 지났는지 검사
                if (DateTime.Now.Year != m_dtLastCheckedPSMSensorLifeTime.Year ||
                    DateTime.Now.Month != m_dtLastCheckedPSMSensorLifeTime.Month ||
                    DateTime.Now.Day != m_dtLastCheckedPSMSensorLifeTime.Day)
                {
                    CheckPSMSensorLifeTime();
                    m_dtLastCheckedPSMSensorLifeTime = DateTime.Now;
                }

                m_bUpdateSensor = false;
                //if (m_frmTank != null && m_frmTank.IsDisposed == false && m_frmTank.Visible)
                //    m_frmTank.SetTankData();
            }
        }

        private bool m_bUpdateSensor = true;
        private void UpdateSensorValues(object mForm)
        {
            Form form = (Form)mForm;
            while(m_bUpdateSensor == true)
            {
                try
                {
                    if (form == null || form.IsDisposed == true)
                        break;

                    if( form.IsHandleCreated == true)
                    {
                        if( form.IsDisposed == false)
                        {
                            form.Invoke((MethodInvoker)delegate
                            {
                                Dictionary<UnE.PSM.PSMSensor, VariousData<int>> dicSensorIDLevels = UpdateSensors();
                                UpdateTanks(dicSensorIDLevels);

                                // PSMSensor의 사용기한을 마지막으로 확인한 이후 날짜가 지났는지 검사
                                if (DateTime.Now.Year != m_dtLastCheckedPSMSensorLifeTime.Year ||
                                    DateTime.Now.Month != m_dtLastCheckedPSMSensorLifeTime.Month ||
                                    DateTime.Now.Day != m_dtLastCheckedPSMSensorLifeTime.Day)
                                {
                                    CheckPSMSensorLifeTime();
                                    m_dtLastCheckedPSMSensorLifeTime = DateTime.Now;
                                }
                            });
                        }                        
                    }
                    else
                    {
                        break;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        if (m_bUpdateSensor == false)
                            break;
                        System.Threading.Thread.Sleep(1250);
                    }
                }
                catch(Exception)
                {
                }
                
            }
        }

        private int GetAlarmDepth(int nAlarmData)
        {
            if ((nAlarmData & 4) == 4)
                return 3;
            else if ((nAlarmData & 2) == 2)
                return 2;
            else if ((nAlarmData & 1) == 1)
                return 1;

            return 0;
        }

        private void UpdateTanks(Dictionary<UnE.PSM.PSMSensor, VariousData<int>> dicSensorIDRow)
        {
            string strSQL = "Select ID, Remains, Capacity, UnitName from PSMTank";
            ArrayList arrResult = m_dbmgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3;i+=4 )
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DBUtility.VariousData<float> fRemains = WebDBManager.GetFloatField(arrResult[i + 1].ToString());
                DBUtility.VariousData<float> fCapacity = WebDBManager.GetFloatField(arrResult[i + 2].ToString());
                string strUnitName = WebDBManager.GetStringField(arrResult[i + 3]);

                UnE.PSM.PSMTank tank = GetPSMTank(nID);
                
                if (tank != null)
                //if (m_dicPSMTank.TryGetValue(nID, out tank))
                {
                    tank.Remains = fRemains;
                    tank.Capacity = fCapacity;
                    tank.UnitName = strUnitName;
                }
            }

            foreach (DataGridViewRow row in gvPSMTank.Rows)
            {
                UnE.PSM.PSMTank tank = (UnE.PSM.PSMTank)row.Tag;
                VariousData<int> currentLevel = null;
                float fCurrentSensorData = -999.0f;
                bool sensorIsOff = false;

                if (tank != null)
                {
                    foreach (UnE.PSM.PSMSensor sensor in tank.LinkedSensorList)
                    {
                        VariousData<int> level = null;
 
                        if (dicSensorIDRow.TryGetValue(sensor, out level))
                        {
                            if (currentLevel == null)
                            {
                                currentLevel = level;
                                fCurrentSensorData = sensor.CurrentData;
                                sensorIsOff = !CheckSensorOn(sensor);//sensor.SensorStatus != UnE.PSM.PSMSensor.Status.On;
                            }
                            else
                            {
                                int current = GetAlarmDepth(currentLevel.Data);
                                int data2 = GetAlarmDepth(level.Data);

                                if (data2 > current)
                                {
                                    currentLevel = level;
                                    fCurrentSensorData = sensor.CurrentData;
                                    sensorIsOff = !CheckSensorOn(sensor);//sensor.SensorStatus != UnE.PSM.PSMSensor.Status.On;
                                }
                            }
                        }
                    }

                    row.Cells[TANK_REMAINS_INDEX].Value = GetTankRemainString(tank.Capacity, tank.Remains, tank.UnitName);
                }

                string strAlarmDepth = MakeAlarmDepthString(currentLevel);
                row.Cells[TANK_ALARM_DEPTH_INDEX].Value = strAlarmDepth;

                SetRowStyle(strAlarmDepth, row, fCurrentSensorData, sensorIsOff);
            }
        }

        private bool CheckSensorOn(UnE.PSM.PSMSensor sensor)
        {
            if (sensor.SensorStatus == UnE.PSM.PSMSensor.Status.On)
                return true;
            else if (sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off4Work)
            {
                DateTime dtNow = DateTime.Now;

                if (sensor.BeginWorkTime != null && sensor.EndWorkTime != null &&
                    dtNow >= sensor.BeginWorkTime.Data && dtNow <= sensor.EndWorkTime.Data)
                    return false;
                else
                    return true;
            }

            return false;
        }

        // Return 값 : 0(교체 필요없음)
        //             1(m_nLifeTimeAlertMonth 이내 교체 필요)
        //             2(교체시기가 이미 지났음)
        private LifeTimeState CheckSensorLifeTime(UnE.PSM.PSMSensor sensor)
        {
            LifeTimeState result = LifeTimeState.NORMAL;

            if (sensor.InstallDate == null || sensor.SensorType == null)
                return result;

            DateTime dtNow = DateTime.Now;
            DateTime dtDeadLine = sensor.InstallDate.Data.AddMonths(sensor.SensorType.LifeTimeMonth);
            DateTime dtAlertLine = dtDeadLine.AddMonths(-m_nLifeTimeAlertMonth);

            if (dtNow.Year > dtDeadLine.Year || (dtNow.Year == dtDeadLine.Year && dtNow.Month > dtDeadLine.Month) ||
                (dtNow.Year == dtDeadLine.Year && dtNow.Month == dtDeadLine.Month && dtNow.Day > dtDeadLine.Day))
            {
                result = LifeTimeState.ALREADY_EXPIRED;
            }
            else if (dtNow.Year > dtAlertLine.Year || (dtNow.Year == dtAlertLine.Year && dtNow.Month > dtAlertLine.Month) ||
                (dtNow.Year == dtAlertLine.Year && dtNow.Month == dtAlertLine.Month && dtNow.Day > dtAlertLine.Day))
            {
                result = LifeTimeState.NEED_CHANGE_SENSOR;
            }

            return result;
        }

        private void SetRowStyle(DataGridViewRow row, Color foreColor)
        {
            if (row.DefaultCellStyle.ForeColor != foreColor)
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.ForeColor = foreColor;
                row.DefaultCellStyle = style;
            }
        }

        private void SetRowStyle(DataGridViewRow row, Color foreColor, Color backColor, Font font)
        {
            if (row.DefaultCellStyle.ForeColor != foreColor || row.DefaultCellStyle.BackColor != backColor)
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = font;
                style.ForeColor = foreColor;
                style.BackColor = backColor;
                row.DefaultCellStyle = style;
            }
        }

        private void SetRowStyle(DataGridViewRow row, Color foreColor, Font font)
        {
            if (row.DefaultCellStyle.ForeColor != foreColor)
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = font;
                style.ForeColor = foreColor;
                row.DefaultCellStyle = style;
            }
        }

        private void SetRowStyle(string strAlarmDepth, DataGridViewRow row, float fSensorData, bool isNotUse, LifeTimeState lifeTime = LifeTimeState.NORMAL)
        {
            if (isNotUse)
            {
                if (lifeTime == LifeTimeState.NEED_CHANGE_SENSOR)
                    SetRowStyle(row, m_foreNeedChangeSensor, m_backNeedChangeSensor, m_fontNeedChangeSensor);
                else if (lifeTime == LifeTimeState.ALREADY_EXPIRED)
                    SetRowStyle(row, m_foreAlreadyExpiredSensor, m_backAlreadyExpiredSensor, m_fontExpired);
                else
                    SetRowStyle(row, m_foreNotUse, m_backNotUse, m_fontNotUse);
                /*if (row.DefaultCellStyle.ForeColor != m_clrNotUse)
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    style.ForeColor = m_clrNotUse;
                    row.DefaultCellStyle = style;
                }*/
            }
            else if (strAlarmDepth == "-")
            {
                if (lifeTime == LifeTimeState.NEED_CHANGE_SENSOR)
                    SetRowStyle(row, m_foreNeedChangeSensor, m_backNeedChangeSensor, m_fontNeedChangeSensor);
                else if (lifeTime == LifeTimeState.ALREADY_EXPIRED)
                    SetRowStyle(row, m_foreAlreadyExpiredSensor, m_backAlreadyExpiredSensor, m_fontExpired);
                else
                {
                    if (fSensorData >= 0.0f)
                    {
                        SetRowStyle(row, m_foreNormal, m_backNormal, m_fontNormal);
                        /*if (row.DefaultCellStyle.ForeColor != Color.Black)
                        {
                            DataGridViewCellStyle style = new DataGridViewCellStyle();
                            style.ForeColor = Color.Black;
                            row.DefaultCellStyle = style;
                        }*/
                    }
                    else
                    {
                        SetRowStyle(row, m_foreOffline, m_backOffline, m_fontOffline);
                        /*if (row.DefaultCellStyle.ForeColor != m_foreOffline || row.DefaultCellStyle.BackColor != m_backOffline)
                        {
                            DataGridViewCellStyle style = new DataGridViewCellStyle();
                            style.ForeColor = m_foreOffline;
                            style.BackColor = m_backOffline;
                            row.DefaultCellStyle = style;
                        }*/
                    }
                }
            }
            else
            {
                SetRowStyle(row, m_foreAlarm, m_backAlarm, m_fontAlarm);
                /*if (row.DefaultCellStyle.ForeColor != m_clrAlarm)
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    style.Font = new Font(gvPSMSensor.Font, FontStyle.Bold);
                    style.ForeColor = m_clrAlarm;
                    row.DefaultCellStyle = style;
                }*/
            }
        }

        private Dictionary<UnE.PSM.PSMSensor, VariousData<int>> UpdateSensors()
        {
            Dictionary<int, DataGridViewRow> dicSensorIDRow = new Dictionary<int, DataGridViewRow>();

            foreach (DataGridViewRow row in gvPSMSensor.Rows)
            {
                UnE.PSM.PSMSensor sensor = (UnE.PSM.PSMSensor)row.Tag;

                if (sensor == null)
                    continue;

                dicSensorIDRow[sensor.ID] = row;
            }

            Dictionary<UnE.PSM.PSMSensor, VariousData<int>> dicSensorLevels = new Dictionary<UnE.PSM.PSMSensor, VariousData<int>>();

            string strSQL = "Select ID, CurrentData, CurrentLevel from PSMSensor";
            ArrayList arrResult = m_dbmgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return dicSensorLevels;

            Dictionary<int, SensorStatus> dicSensorStatus = LoadSensorStatusData();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                float fData = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), -999);
                VariousData<int> currentLevel = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                DataGridViewRow row = null;
                if (dicSensorIDRow.TryGetValue(nID, out row))
                {
                    UnE.PSM.PSMSensor sensor = (UnE.PSM.PSMSensor)row.Tag;

                    if (sensor == null)
                        continue;

                    SensorStatus sensorStatus;

                    if (dicSensorStatus != null && dicSensorStatus.TryGetValue(nID, out sensorStatus))
                    {
                        sensor.SensorStatus = UnE.PSM.PSMSensor.ToStatus(sensorStatus.Status);
                        sensor.BeginWorkTime = sensorStatus.BeginTime;
                        sensor.EndWorkTime = sensorStatus.EndTime;
                    }
                    else
                        sensorStatus = null;

                    bool sensorIsOn = SetSensorStatusCellValue(row.Cells[SENSOR_ONOFF_INDEX], sensor);

                    string strAlarmDepth = MakeAlarmDepthString(currentLevel);
                    row.Cells[SENSOR_ALARM_DEPTH_INDEX].Value = strAlarmDepth;
                    sensor.CurrentAlarmDepth = currentLevel;

                    if (sensor.IsTapeType)
                    {
                        if (strAlarmDepth == "-")
                            row.Cells[SENSOR_OVERFLOW_INDEX].Value = "-";
                        else
                            row.Cells[SENSOR_OVERFLOW_INDEX].Value = "누출감지";
                    }
                    else
                    {
                        UnE.PSM.PSMMaterial material = (UnE.PSM.PSMMaterial)row.Cells[SENSOR_MATERIAL_NAME_INDEX].Tag;
                        row.Cells[SENSOR_OVERFLOW_INDEX].Value = String.Format("{0:F1} {1}", fData, material != null ? material.UOM : "");
                        sensor.CurrentData = fData;
                    }

                    dicSensorLevels[sensor] = currentLevel;

                    SetRowStyle(strAlarmDepth, row, fData, !sensorIsOn, CheckSensorLifeTime(sensor));
                }
            }

            return dicSensorLevels;
        }

        private string MakeAlarmDepthString(VariousData<int> alarmDepth)
        {
            if (alarmDepth == null)
                return "-";

            if ((alarmDepth.Data & 4) == 4)
                return "3단계";
            else if ((alarmDepth.Data & 2) == 2)
                return "2단계";
            else if ((alarmDepth.Data & 1) == 1)
                return "1단계";

            return "-";
        }

        #region Event Func

        private System.Threading.Thread mCheckValueThread = null;
         
        private void FormPSMList_Load(object sender, EventArgs e)
        {

            //MessageBox.Show("Load!!!");
            // 실내외 구분을 안보이게 한다.
            int right = cmbLocation.Location.X + cmbLocation.Size.Width;
            lblLoacation.Location = lblInOut.Location;
            cmbLocation.Location = cmbInOut.Location;

            cmbLocation.Size = new System.Drawing.Size(right - cmbLocation.Location.X, cmbLocation.Size.Height);
            lblInOut.Visible = cmbInOut.Visible = false;
            ///////////////////////////////////

         

            rdoTankList.PerformClick();
            InitPanelGuide();

            //timer1.Start();

            if (mCheckValueThread == null)
            {
                mCheckValueThread = new System.Threading.Thread(UpdateSensorValues);
                mCheckValueThread.Name = "UpdateSensorValuePSMList";
                mCheckValueThread.Start(this);
            }
            
        }

        private void InitPanelGuide()
        {
            btnGuide.Location = new Point(0, 0);
            gridGuide.Location = new Point(btnGuide.Size.Width - 1, -1);
            gridGuide.Size = new Size(panelGuide.Size.Width - btnGuide.Size.Width + 1, panelGuide.Size.Height + 1);

            foreach (DataGridViewColumn column in gridGuide.Columns)
            {
                column.Width = (gridGuide.Width - 1) / gridGuide.Columns.Count;
            }

            DataGridViewRow row1 = MakeNewRow(gridGuide);
            DataGridViewRow row2 = MakeNewRow(gridGuide);

            m_fontBold = new Font(gvPSMSensor.Font, FontStyle.Bold);

            row1.Cells[0].Value = "알람 상태";
            row1.Cells[2].Value = "교체 필요";
            row1.Cells[1].Value = "기간 초과";
            row2.Cells[0].Value = "정상 상태";
            row2.Cells[2].Value = "통신 끊김";
            row2.Cells[1].Value = "사용 안함";
            row1.Cells[0].Style.ForeColor = this.m_foreAlarm;
            row1.Cells[0].Style.BackColor = this.m_backAlarm;
            row1.Cells[0].Style.Font = m_fontBold;
            row1.Cells[2].Style.ForeColor = this.m_foreNeedChangeSensor;
            row1.Cells[1].Style.ForeColor = this.m_foreAlreadyExpiredSensor;
            row1.Cells[1].Style.Font = m_fontBold;
            row2.Cells[2].Style.ForeColor = m_foreOffline;
            row2.Cells[2].Style.BackColor = m_backOffline;
            row2.Cells[1].Style.ForeColor = m_foreNotUse;

            m_fontAlarm = row1.Cells[0].Style.Font;
            m_fontExpired = row1.Cells[1].Style.Font;
            m_fontNeedChangeSensor = row1.Cells[2].Style.Font;
            m_fontNormal = row2.Cells[0].Style.Font;
            m_fontNotUse = row2.Cells[1].Style.Font;
            m_fontOffline = row2.Cells[2].Style.Font;

            gridGuide.ClearSelection();
            row1.Height = row2.Height = (gridGuide.Size.Height - 1) / 2;
        }

        private void FormPSMList_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_bUpdateSensor = false;

            try
            {
                if (mCheckValueThread != null)
                    mCheckValueThread.Join(1100);
            }
            catch(Exception)
            {
            }

            mCheckValueThread = null;
            this.Dispose();
        }

        
        private void gvPSMSensor_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewRow row = gvPSMSensor.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            UnE.PSM.PSMSensor sensor = (UnE.PSM.PSMSensor)row.Tag;

            if (sensor == null)
                return;

            int nSensorNo;

            if (!int.TryParse(row.Cells[0].Value.ToString(), out nSensorNo))
                return;

            if (gvPSMSensor.Columns[e.ColumnIndex] == this.colPSMSensorIsWorking)
            {
                //if (e.ColumnIndex == m_nOnOffColumnIndex)
                {
                    PopSensorState(sensor, nSensorNo, row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString());
                }
                /*else if (e.ColumnIndex == m_nLocationColumnIndex)
                {
                    PopTankDetail();
                }*/
            }
            else if (e.ColumnIndex != SENSOR_ONOFF_INDEX && e.ColumnIndex != SENSOR_CCTV_INDEX)
            {
                ShowSensorChart(sensor);
            }
        }

        private void ShowSensorChart(UnE.PSM.PSMSensor sensor)
        {
            try
            {
                int nMonitorPosition = UnE.SOP.ProxySOP.Instance.CCTVMontior;

                if (nMonitorPosition < 1 || nMonitorPosition > Screen.AllScreens.Length)
                    nMonitorPosition = 1;

                if (FormMain.Instance.PSMSensorDataForm == null || FormMain.Instance.PSMSensorDataForm.IsDisposed == true)
                {
                    FormMain.Instance.PSMSensorDataForm = new FormPSMSensorTrendData(sensor);

                    Rectangle bounds;
                    // CCTV Process가 활성화 되어있으면 CCTV의 위치를 중심으로 팝업
                    if (FormMain.Instance.CCTVProcess != null && FormMain.Instance.CCTVProcess.HasExited == false && FormMain.Instance.CCTVProcess.MainWindowHandle != IntPtr.Zero)
                    {
                        IntPtr hwnd = FormMain.Instance.CCTVProcess.MainWindowHandle;
                        UnE.Win32.NativeMethods.GetWindowRect(hwnd, out bounds);
                    }
                    // CCTV Processs가 없으면 모니터 설정에 따른 위치를 중심으로 팝업
                    else
                    {
                        bounds = Screen.AllScreens[nMonitorPosition - 1].Bounds;
                    }

                    int nX = ((bounds.Right - bounds.X) / 2) + (bounds.X) - (FormMain.Instance.PSMSensorDataForm.Width / 2);
                    int nY = ((bounds.Bottom - bounds.Y) / 2) + (bounds.Y) - (FormMain.Instance.PSMSensorDataForm.Height / 2);


                    // 모니터 사이즈를 벗어나지 않도록 설정
                    if (nX < 0)
                    {
                        nX = 0;
                    }
                    else
                    {
                        int nEndX = nX + FormMain.Instance.PSMSensorDataForm.Width;
                        int nMaxX = 0;

                        foreach (Screen screen in Screen.AllScreens)
                        {
                            if (nMaxX < screen.Bounds.X + screen.Bounds.Width)
                                nMaxX = screen.Bounds.X + screen.Bounds.Width;
                        }

                        if (nEndX > nMaxX)
                        {
                            nX = nMaxX - FormMain.Instance.PSMSensorDataForm.Width;
                        }

                    }

                    if (nY < 0)
                    {
                        nY = 0;
                    }
                    else
                    {
                        int nEndY = nY + FormMain.Instance.PSMSensorDataForm.Height;
                        int nMaxY = 0;

                        foreach (Screen screen in Screen.AllScreens)
                        {
                            if (nMaxY < screen.Bounds.Y + screen.Bounds.Height)
                                nMaxY = screen.Bounds.Y + screen.Bounds.Height;
                        }

                        if (nEndY > nMaxY)
                        {
                            nY = nMaxY - FormMain.Instance.PSMSensorDataForm.Height;
                        }
                    }

                    FormMain.Instance.PSMSensorDataForm.StartPosition = FormStartPosition.Manual;
                    FormMain.Instance.PSMSensorDataForm.Location = new Point(nX, nY);
                    FormMain.Instance.PSMSensorDataForm.Show(this);
                }
                else
                {
                    this.Cursor = Cursors.WaitCursor;
                    System.Diagnostics.Trace.WriteLine("Start : " + sensor.Name);
                    FormMain.Instance.PSMSensorDataForm.ChangeSensor(sensor);
                    this.ActiveControl = null;
                    FormMain.Instance.PSMSensorDataForm.Focus();
                    System.Diagnostics.Trace.WriteLine("End : " + sensor.Name);
                    this.Cursor = Cursors.Default;

                    FormMain.Instance.PSMSensorDataForm.BringToFront();
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void gvPSMTank_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewRow row = gvPSMTank.Rows[e.RowIndex];
            UnE.PSM.PSMTank tank = (UnE.PSM.PSMTank)row.Tag;
            PopTankDetail(tank);

            /*if (e.ColumnIndex == m_nLocationColumnIndex)
            {
                PopTankDetail();
            }*/
        }

        private void gridCellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (sender == gvPSMSensor)
            {
                UnE.PSM.PSMSensor sensor = (UnE.PSM.PSMSensor)gvPSMSensor.Rows[e.RowIndex].Tag;

                if (sensor == null) return;
                if (sensor.LinkedTankList.Count == 0) return;

                if (e.ColumnIndex == SENSOR_CCTV_INDEX)
                {
                    ShowBigCCTV(sensor);
                }
                
            }
            else if (sender == gvPSMTank && e.ColumnIndex == TANK_CCTV_INDEX)
            {
                UnE.PSM.PSMTank tank = (UnE.PSM.PSMTank)gvPSMTank.Rows[e.RowIndex].Tag;

                if (tank == null)
                    return;

                ShowBigCCTV(tank);
            }
        }


        private void ShowBigCCTV(UnE.PSM.PSMSensor sensor)
        {
            if (sensor == null)
                return;

            UnE.PSM.PSMTank tank = sensor.LinkedTankList[0];
            UnE.Spatial.EquipmentZone equipZone = tank.EquipZone;

            if (equipZone == null)
                return;

            string strOutdoorFilePath, strIndoorFilePath;
            DownloadEquipZoneImage(equipZone, out strOutdoorFilePath, out strIndoorFilePath);

            if (strOutdoorFilePath.Length > 0)
            {
                string tPath = strOutdoorFilePath.Replace("\\", "/");
                FormMain.Instance.CCTVPipe.Send("SetViewerImage(1, '" + tPath + "', '" + equipZone.ZoneName + "')");
            }

            //if (strIndoorFilePath.Length > 0)
            //{
            //    string tPath = strIndoorFilePath.Replace("\\", "/");
            //    FormMain.Instance.CCTVPipe.Send("SetViewerImage(2, '" + tPath + "')");
            //}

            if (sensor != null)
            {                
                ArrayList arPaths = FormMain.Instance.PageHome.DownloadPSMImage(sensor.ID);
                if (arPaths != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string tPath = arPaths[i].ToString().Replace("\\", "/");
                        string strImageTitle = arPaths[i + 3].ToString();
                        FormMain.Instance.CCTVPipe.Send("SetViewerImage(" + (i + 2) + ",'" + tPath + "', '" + strImageTitle + "')");
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        FormMain.Instance.CCTVPipe.Send("SetViewerImage(" + (i + 2) + ",'', '')");
                    }
                }
            }
            FormMain.Instance.CCTVPipe.Send("ShowSituationCCTV2(3, " + equipZone.ID.ToString() + ")");

            ArrayList arrCCTVs = PageBackstageHome.Instance.GetEquipZoneCCTVList(equipZone);

            if (arrCCTVs == null)
                return;

            PageBackstageHome.Instance.ShowBigCCTV(tank.LocationName, arrCCTVs);
        }

        private void ShowBigCCTV(UnE.PSM.PSMTank tank)
        {
            UnE.Spatial.EquipmentZone equipZone = tank.EquipZone;

            if (equipZone == null)
                return;

            string strOutdoorFilePath, strIndoorFilePath;
            DownloadEquipZoneImage(equipZone, out strOutdoorFilePath, out strIndoorFilePath);

            if (strOutdoorFilePath.Length > 0)
            {
                string tPath = strOutdoorFilePath.Replace("\\", "/");
                FormMain.Instance.CCTVPipe.Send("SetViewerImage(1, '" + tPath + "', '" + equipZone.ZoneName + "')");
            }

            //if (strIndoorFilePath.Length > 0)
            //{
            //    string tPath = strIndoorFilePath.Replace("\\", "/");
            //    FormMain.Instance.CCTVPipe.Send("SetViewerImage(2, '" + tPath + "')");
            //}

            if( tank.LinkedSensorList.Count > 0)
            {
                UnE.PSM.PSMSensor sensor = tank.LinkedSensorList[0];
                ArrayList arPaths = FormMain.Instance.PageHome.DownloadPSMImage(sensor.ID);
                if (arPaths != null)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        string tPath = arPaths[i].ToString().Replace("\\", "/");
                        string strImageTitle = arPaths[i + 3].ToString();
                        FormMain.Instance.CCTVPipe.Send("SetViewerImage(" + (i + 2) + ",'" + tPath + "', '" + strImageTitle + "')");
                    }
                }
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        FormMain.Instance.CCTVPipe.Send("SetViewerImage(" + (i + 2) + ",'', '')");
                    }
                }
            }


            FormMain.Instance.CCTVPipe.Send("ShowSituationCCTV2(3, " + equipZone.ID.ToString() + ")"); 

            ArrayList arrCCTVs = PageBackstageHome.Instance.GetEquipZoneCCTVList(equipZone);

            if (arrCCTVs == null)
                return;

            PageBackstageHome.Instance.ShowBigCCTV(tank.LocationName, arrCCTVs);
        }

        private void DownloadEquipZoneImage(UnE.Spatial.EquipmentZone equipZone, out string strOutdoorFilePath, out string strIndoorFilePath)
        {
            strOutdoorFilePath = strIndoorFilePath = "";
            string strOutdoorTarget = "EquipZoneOutdoorFolder";
            string strIndoorTarget = "EquipZoneIndoorFolder";

            string strSQL = string.Format("Select PropertyName, PropertyValue from OptionSDMS where (PropertyName = '{0}' or PropertyName = '{1}') and SiteID = {2}",
                strOutdoorTarget, strIndoorTarget, UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = m_dbmgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            string strRootURL = GetRootURL();
            string strOutdoorURL = "", strIndoorURL = "";

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyValue == null || strPropertyValue.Length == 0)
                    continue;

                strPropertyValue = strPropertyValue.Trim();

                if (!strPropertyValue.StartsWith("/"))
                    strPropertyValue = "/" + strPropertyValue;

                if (strPropertyName == strOutdoorTarget)
                    strOutdoorURL = strRootURL + strPropertyValue;
                else if (strPropertyName == strIndoorTarget)
                    strIndoorURL = strRootURL + strPropertyValue;
            }

            System.Net.WebClient web = new System.Net.WebClient();
            string strImageFileName = "/" + equipZone.ID.ToString() + ".png";

            if (strOutdoorURL.Length > 0)
            {
                strOutdoorFilePath = DownloadFile(web, strOutdoorURL + strImageFileName, "EquipZoneOutdoorImage.png");
                strOutdoorFilePath = strOutdoorFilePath.Replace("\\\\", "\\");
            }

            if (strIndoorURL.Length > 0)
            {
                strIndoorFilePath = DownloadFile(web, strIndoorURL + strImageFileName, "EquipZoneIndoorImage.png");
                strIndoorFilePath = strIndoorFilePath.Replace("\\\\", "\\");
            }

            web.Dispose();
        }

        private string DownloadFile(System.Net.WebClient web, string strURL, string strLocalFileName)
        {
            string strFolder = System.IO.Path.GetTempPath();
            string strFilePath = strFolder + "\\" + strLocalFileName;
            
            try
            {
                if (System.IO.File.Exists(strFilePath))
                    System.IO.File.Delete(strFilePath);
            }
            catch(Exception)
            {
            }           

            System.Diagnostics.Trace.WriteLine(strFilePath);

            try
            {                
                web.DownloadFile(strURL, strFilePath);
            }
            catch (Exception)
            {
            }            
            return strFilePath;
        }

        private string GetRootURL()
        {
            string strURL = m_dbmgr.WebServerURL;

            int nIndex = strURL.IndexOf("//");

            if (nIndex >= 0)
            {
                int nIndex2 = strURL.IndexOf('/', nIndex + 2);

                if (nIndex2 >= 0)
                    strURL = strURL.Substring(0, nIndex2);
            }
            else
            {
                int nIndex2 = strURL.IndexOf('/');

                if (nIndex2 >= 0)
                    strURL = strURL.Substring(0, nIndex2);
            }

            return strURL;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (m_isTankView)
                LoadTankData();
            else
                LoadSensorData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            UnE.View.Content.ContentOwnerTab tab = FormMain.Instance.PageHome.CurrentTab;
            FormMain.Instance.PageHome.HideRightDockingPane();

            if (tab == UnE.View.Content.ContentOwnerTab.ADMIN_TAB)
                FormMain.Instance.PageHome.ShowAllDockingPane();
        }

        private void cmbPSMMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            LocationComboBoxItem defaultItem = MakeDefaultLocationItem();
            cmbLocation.Items.Clear();

            if (cmbPSMMaterial.SelectedItem != null)
            {
                List<LocationComboBoxItem> locations = null;
                MaterialComboBoxItem item = (MaterialComboBoxItem)cmbPSMMaterial.SelectedItem;

                if (item.Material != null)
                {
                    m_dicMaterialLocations.TryGetValue(item.Material, out locations);
                }
                else if (item.ID < 0)
                {
                    locations = new List<LocationComboBoxItem>();

                    foreach (KeyValuePair<UnE.PSM.PSMMaterial, List<LocationComboBoxItem>> pair in m_dicMaterialLocations)
                    {
                        foreach (LocationComboBoxItem _item in pair.Value)
                        {
                            if (!locations.Contains(_item))
                                locations.Add(_item);
                        }
                    }
                }

                if (locations != null)
                {
                    locations.Sort();

                    if (defaultItem != null)
                        cmbLocation.Items.Add(defaultItem);

                    foreach (LocationComboBoxItem location in locations)
                    {
                        cmbLocation.Items.Add(location);
                    }

                    cmbLocation.SelectedItem = cmbLocation.Items[0];
                }
            }
        }

        private void btnShowSensorManual_Click(object sender, EventArgs e)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = string.Format("Select PropertyValue from OptionSDMS where PropertyName = 'PSMSensorManualPath' and SiteID = {0}", UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\" + DBUtility.WebDBManager.GetStringField(arrResult[0]);
            FormPSMTankDetail.OpenPDF(1, strPath);
        }

        #endregion Event Func


        #region ComboBox Item Class

        private class ComboBoxItem
        {
            public string DisplayText { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return DisplayText;
            }
        }

        private class MaterialComboBoxItem : ComboBoxItem, IComparable
        {
            public int ID
            {
                get
                {
                    int nReturn = -1;

                    if (int.TryParse(Value.ToString(), out nReturn))
                        return nReturn;
                    else
                        return -1;
                }
                set { Value = value; }

            }
            public string MaterialName
            {
                get { return DisplayText; }
                set { DisplayText = value; }
            }
            public string UOM { get; set; }
            public string Description { get; set; }
            public UnE.PSM.PSMMaterial Material { get; set; }

            public int CompareTo(object obj)
            {
                MaterialComboBoxItem item = (MaterialComboBoxItem)obj;

                if (this.ID < 0 && item.ID < 0)
                    return 0;
                else if (this.ID < 0)
                    return -1;
                else if (item.ID < 0)
                    return 1;

                return this.MaterialName.CompareTo(item.MaterialName);
            }
        }

        private class InOutComboBoxItem : ComboBoxItem { }

        private class LocationComboBoxItem : ComboBoxItem, IComparable
        {
            public int ID
            {
                get
                {
                    int nReturn = -1;

                    if (int.TryParse(Value.ToString(), out nReturn))
                        return nReturn;
                    else
                        return -1;
                }
                set { Value = value; }

            }
            public string LocationName
            {
                get { return DisplayText; }
                set { DisplayText = value; }
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is LocationComboBoxItem)
                {
                    LocationComboBoxItem item = (LocationComboBoxItem)obj;

                    if (this.LocationName == item.LocationName)
                        return true;
                }

                return false;
            }

            public int CompareTo(object obj)
            {
                LocationComboBoxItem item = (LocationComboBoxItem)obj;

                if (this.ID < 0 && item.ID < 0)
                    return 0;
                else if (this.ID < 0)
                    return -1;
                else if (item.ID < 0)
                    return 1;

                return this.LocationName.CompareTo(item.LocationName);
            }
        }

        private class OnOffComboBoxItem : ComboBoxItem
        {
            private UnE.PSM.PSMSensor.Status m_status = UnE.PSM.PSMSensor.Status.Unknown;
            public UnE.PSM.PSMSensor.Status Status
            {
                get { return m_status; }
                set { m_status = value; }
            }

            public bool IsSame(UnE.PSM.PSMSensor.Status status)
            {
                bool bReturn = false;

                if (m_status == UnE.PSM.PSMSensor.Status.Unknown)
                {
                    bReturn = true;
                }
                else
                {
                    switch (status)
                    {
                        case UnE.PSM.PSMSensor.Status.On:
                            if (m_status == UnE.PSM.PSMSensor.Status.On)
                                bReturn = true;

                            break;

                        case UnE.PSM.PSMSensor.Status.Off:
                        case UnE.PSM.PSMSensor.Status.LocalOff:
                        case UnE.PSM.PSMSensor.Status.Off4Work:
                            if (m_status == UnE.PSM.PSMSensor.Status.Off)
                                bReturn = true;

                            break;
                    }
                }
                return bReturn;
            }

        }

        #endregion ComboBox Item Class

        private void FormPSMList_Shown(object sender, EventArgs e)
        {

            //this.Focus();
            //this.rdoTankList.Focus();

           // btnHidden.PerformClick();
        }

        private Timer t = new Timer();
        public void SetFocusUser()
        {
            if (t.Enabled == true)
                t.Enabled = false;
            t = new Timer();
            t.Interval = 1000;
            t.Tick += t_Tick;
            t.Enabled = true;
           // t.Start();
        }

        private int m_nCount = 0;
        void t_Tick(object sender, EventArgs e)
        {
           // t.Stop();
            if (m_nCount == 3)
            {
                t.Enabled = false;
                m_nCount = -1;
            }
           

          //  btnHidden.PerformClick();

            OnMouseLButtonClick();

            m_nCount++;
        }


        //[DllImport("user32.dll")]
        //static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        //private static int WM_LBUTTONDOWN = 0x201; //Left mousebutton down
        //private static int WM_LBUTTONUP = 0x202; //Left mousebutton up
        //private static int WM_LBUTTONDBLCLK = 0x203; //Left mousebutton doubleclick
        //private static int WM_RBUTTONDOWN = 0x204; //Right mousebutton down
        //private static int WM_RBUTTONUP = 0x205;  //Right mousebutton up
        //private static int WM_RBUTTONDBLCLK = 0x206; //Right mousebutton doubleclick
        

        public void OnMouseLButtonClick()
        {
            IntPtr pt = UnE.Win32.NativeMethods.MakeLParam(0, 0);
            if (this.IsDisposed == false && this.IsHandleCreated == true)
            {
                UnE.Win32.NativeMethods.SendMessage(pnlType.Handle, UnE.Win32.NativeMethods.WM_LBUTTONDOWN, IntPtr.Zero, pt);
                UnE.Win32.NativeMethods.SendMessage(pnlType.Handle, UnE.Win32.NativeMethods.WM_LBUTTONUP, IntPtr.Zero, pt);
            }
            //IsSelected = !IsSelected;
        }

        private void pnlType_MouseDown(object sender, MouseEventArgs e)
        {
            int i = 0;
            i++;
        }

        private void gvPSMSensor_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (rdoSensorList.Checked)
                {
                    DataGridView.HitTestInfo hitInfo = gvPSMSensor.HitTest(e.X, e.Y);

                    if (hitInfo.RowIndex >= 0 && hitInfo.ColumnIndex >= 0)
                    {
                        DataGridViewRow row = gvPSMSensor.Rows[hitInfo.RowIndex];
                        row.Cells[hitInfo.ColumnIndex].Selected = true;

                        UnE.PSM.PSMSensor sensor = (UnE.PSM.PSMSensor)row.Tag;

                        if (sensor != null)
                        {
                            if (sensor.SensorStatus == UnE.PSM.PSMSensor.Status.On)
                            {
                                menuSensorOn.Checked = true;
                                menuSensorOff.Checked = false;
                            }
                            else if (sensor.SensorStatus == UnE.PSM.PSMSensor.Status.LocalOff || sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off || sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off4Work)
                            {
                                menuSensorOn.Checked = false;
                                menuSensorOff.Checked = true;
                            }
                            else
                                menuSensorOn.Checked = menuSensorOff.Checked = false;

                            //menuSensorOnOff.Checked = sensor.SensorStatus == UnE.PSM.PSMSensor.Status.On;
                            popupMenuSensor.Tag = row;
                            popupMenuSensor.Show(gvPSMSensor, e.Location);
                        }
                    }
                }
            }
        }

        private void gvPSMTank_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (rdoTankList.Checked)
                {
                    DataGridView.HitTestInfo hitInfo = gvPSMTank.HitTest(e.X, e.Y);

                    if (hitInfo.RowIndex >= 0 && hitInfo.ColumnIndex >= 0)
                    {
                        DataGridViewRow row = gvPSMTank.Rows[hitInfo.RowIndex];
                        row.Cells[hitInfo.ColumnIndex].Selected = true;

                        UnE.PSM.PSMTank tank = (UnE.PSM.PSMTank)row.Tag;

                        if (tank != null)
                        {
                            popupMenuTank.Tag = row;
                            popupMenuTank.Show(gvPSMTank, e.Location);
                        }
                    }
                }
            }
        }

        private void menuShowSensorChart_Click(object sender, EventArgs e)
        {
            if (popupMenuSensor.Tag != null && popupMenuSensor.Tag is DataGridViewRow)
            {
                DataGridViewRow row = (DataGridViewRow)popupMenuSensor.Tag;

                if (row.Tag != null && row.Tag is UnE.PSM.PSMSensor)
                {
                    UnE.PSM.PSMSensor sensor = (UnE.PSM.PSMSensor)row.Tag;
                    ShowSensorChart(sensor);
                }
            }
        }

        private void menuShowSensorLifeTime_Click(object sender, EventArgs e)
        {
            if (popupMenuSensor.Tag != null && popupMenuSensor.Tag is DataGridViewRow)
            {
                DataGridViewRow row = (DataGridViewRow)popupMenuSensor.Tag;

                if (row.Tag != null && row.Tag is UnE.PSM.PSMSensor)
                {
                    int nSensorNo;

                    if (!int.TryParse(row.Cells[0].Value.ToString(), out nSensorNo))
                        return;

                    UnE.PSM.PSMSensor sensor = (UnE.PSM.PSMSensor)row.Tag;
                    PopSensorLifeTime(sensor, nSensorNo);
                    /*FormPSMSensorLifeTime frm = new FormPSMSensorLifeTime(sensor, nSensorNo);

                    frm.ShowDialog(gvPSMSensor);*/
                }
            }
        }

        public void CheckPSMSensorLifeTime()
        {
            PSMManager.Instance.ReadPSMSensorTypes();

            string strSQL = "Select ID, InstallDate, SensorTypeName from PSMSensor";
            ArrayList arrResult = m_dbmgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<DateTime> installDate = WebDBManager.GetDateTimeField(arrResult[i + 1].ToString());
                string strTypeName = WebDBManager.GetStringField(arrResult[i + 2]);

                if (id == null)
                    continue;

                UnE.PSM.PSMSensor sensor = PSMManager.Instance.GetSensor(id.Data);

                if (sensor == null)
                    continue;

                sensor.InstallDate = installDate;
                sensor.SensorType = PSMManager.Instance.GetPSMSensorType(strTypeName);
            }
        }

        private void menuSensorOn_Click(object sender, EventArgs e)
        {
            UnE.PSM.PSMSensor sensor = GetSensorFromMenu();
            FormPSMSensorWork.SaveSensorStatus(sensor, UnE.PSM.PSMSensor.Status.On);
        }

        private void menuSensorOff_Click(object sender, EventArgs e)
        {
            UnE.PSM.PSMSensor sensor = GetSensorFromMenu();
            FormPSMSensorWork.SaveSensorStatus(sensor, UnE.PSM.PSMSensor.Status.LocalOff);
        }

        private UnE.PSM.PSMSensor GetSensorFromMenu()
        {
            if (popupMenuSensor.Tag == null || (popupMenuSensor.Tag is DataGridViewRow) == false)
                return null;

            DataGridViewRow row = (DataGridViewRow)popupMenuSensor.Tag;

            if (row.Tag == null || (row.Tag is UnE.PSM.PSMSensor) == false)
                return null;

            return (UnE.PSM.PSMSensor)row.Tag;
        }

        private UnE.PSM.PSMTank GetTankFromMenu()
        {
            if (popupMenuTank.Tag == null || (popupMenuTank.Tag is DataGridViewRow) == false)
                return null;

            DataGridViewRow row = (DataGridViewRow)popupMenuTank.Tag;

            if (row.Tag == null || (row.Tag is UnE.PSM.PSMTank) == false)
                return null;

            return (UnE.PSM.PSMTank)row.Tag;
        }

        public void RefreshSensorLifeTime()
        {
            m_dtLastCheckedPSMSensorLifeTime = new DateTime();
        }

        private void menuShowSensorCCTV_Click(object sender, EventArgs e)
        {
            UnE.PSM.PSMSensor sensor = GetSensorFromMenu();
            ShowBigCCTV(sensor);
        }

        private void menuShowTankDetail_Click(object sender, EventArgs e)
        {
            UnE.PSM.PSMTank tank = GetTankFromMenu();
            PopTankDetail(tank);
        }

        private void menuShowTankCCTV_Click(object sender, EventArgs e)
        {
            UnE.PSM.PSMTank tank = GetTankFromMenu();
            ShowBigCCTV(tank);
        }

        private void toolStripMenuItemDepartment_Click(object sender, EventArgs e)
        {
            UnE.PSM.PSMSensor sensor = GetSensorFromMenu();
            PopDepartment(sensor);
        }

        private void menuEditSensorAlarm_Click(object sender, EventArgs e)
        {
            if (popupMenuSensor.Tag != null && popupMenuSensor.Tag is DataGridViewRow)
            {
                DataGridViewRow row = (DataGridViewRow)popupMenuSensor.Tag;

                if (row.Tag != null && row.Tag is UnE.PSM.PSMSensor)
                {
                    int nSensorNo;

                    if (!int.TryParse(row.Cells[0].Value.ToString(), out nSensorNo))
                        return;

                    UnE.PSM.PSMSensor sensor = (UnE.PSM.PSMSensor)row.Tag;
                    PopSensorAlarm(sensor, nSensorNo);
                }
            }
        }

        private void PopSensorAlarm(UnE.PSM.PSMSensor sensor, int nSensorNo)
        {
            CloseOtherPopupFrame(m_frmSensorAlarm);

            if (m_frmSensorAlarm == null || m_frmSensorAlarm.IsDisposed)
            {
                m_frmSensorAlarm = new FormPSMSensorAlarm(sensor, nSensorNo);

                Point pt = this.PointToScreen(new Point(0, 0));
                m_frmSensorAlarm.StartPosition = FormStartPosition.Manual;
                m_frmSensorAlarm.Location = new Point(pt.X - m_frmSensorAlarm.Width, pt.Y + 200);
            }
            else
            {
                m_frmSensorAlarm.SetData(sensor, nSensorNo);
            }

            if (m_frmSensorAlarm.Visible)
            {
                m_frmSensorAlarm.Focus();
            }
            else
            {
                m_frmSensorAlarm.Show(this);
            }
        }

        private void grpArea_Enter(object sender, EventArgs e)
        {

        }

        private void chk12_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chk34_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chk56_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkWater_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkETC_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {

        }
    }
}