using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using UnE.GUI;
using System.Drawing;
using SDMS.Help;
using DBUtility2;

namespace SDMS
{
	public partial class FormSensorList : PopupFormBase
	{
		private class FireEquipmentHistoryData
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

        private enum SensorType { ALL = 0, DETECT_FIRE = 1, COOLER, PUMP, CCTV, FE, HD, FA, FR, PSM_SENSOR, DISASTER_PREVENTION_EQUIPMENT, SVMS, S1ACCESS, EMPOLL, SECOM }

        private int m_nSiteID = 0;
        private List<Admin.SensorListGridData> m_allSensorDatas = null;

        private ManualManager m_manualManager = null;

		public FormSensorList()
		{
            this.DoubleBuffered = true;

            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			InitializeComponent();

            FormMain.SetDoubleBuffer(gvDisasterPreventionEquipment, true);
            FormMain.SetDoubleBuffer(gvSensorList, true);

            this.UseFrmMove = false;
            InitCtrlSize(this);
            SetChildCtrlResize(this, this.Width, this.Height);
            FormMain.Instance.CustomizeGridView(gvDisasterPreventionEquipment);
            FormMain.Instance.CustomizeGridView(gvSensorList);

            m_manualManager = new ManualManager(this);
            SetManualID();

            for (int i = 0; i < gvSensorList.ColumnCount; i++)
            {
                gvSensorList.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
		}

        private IFacility.FacilityType ToFacilityType(SensorType sensorType)
        {
            switch (sensorType)
            {
                case SensorType.DETECT_FIRE:
                    return IFacility.FacilityType.FIRE_SENSOR;

                case SensorType.COOLER:
                    return IFacility.FacilityType.COOLER_SENSOR;

                case SensorType.PUMP:
                    return IFacility.FacilityType.PRESSURE_SENSOR;

                case SensorType.CCTV:
                    return IFacility.FacilityType.CCTV;

                case SensorType.FE:
                    return IFacility.FacilityType.FE;

                case SensorType.HD:
                    return IFacility.FacilityType.HD;

                case SensorType.FA:
                    return IFacility.FacilityType.FA;

                case SensorType.FR:
                    return IFacility.FacilityType.FR;

                case SensorType.PSM_SENSOR:
                    return IFacility.FacilityType.PSM_SENSOR;

                case SensorType.DISASTER_PREVENTION_EQUIPMENT:
                    return IFacility.FacilityType.DISASTER_PREVENTION_EQUIPMENT;
            }

            return IFacility.FacilityType.NONE;
        }

        private bool m_isChanged = false;

		private void FormSensorList_Load(object sender, EventArgs e)
		{
            FormMain.Instance.DataManager.LoadDisasterPreventionEquipment();

			InitGrid();
			InitComboBox();

            btnSelectZone_Click(null, null);

            m_allSensorDatas = new List<Admin.SensorListGridData>();

            // Grid의 데이터를 보관하여 다시 DB를 읽지 않도록 한다.
            foreach (Admin.SensorListGridData data in sensorListGridDataBindingSource)
            {
                m_allSensorDatas.Add(data);
            }
		}

		private void InitComboBox()
		{
			//cboBuildingGroup.Items.Add("모두");

			foreach (KeyValuePair<int, BuildingGroup> pair in ZoneManager.Instance.DicBuildingGroup)
			{
				cboBuildingGroup.Items.Add(pair.Value);
			}

            cboBuildingGroup.Sorted = true;
            cboBuildingGroup.Sorted = false;
            cboBuildingGroup.Items.Insert(0, "모두");

			cboBuildingGroup.SelectedIndex = 0;

            if (UnE.SOP.ProxySOP.Instance.SiteID == 100)
            {
                cboSensorType.Items.Add(new Sensor("모두", SensorType.ALL));
                cboSensorType.Items.Add(new Sensor("CCTV", SensorType.CCTV));
                cboSensorType.Items.Add(new Sensor("화재센서", SensorType.DETECT_FIRE));
                cboSensorType.Items.Add(new Sensor("SVMS", SensorType.SVMS));
                cboSensorType.Items.Add(new Sensor("Access", SensorType.S1ACCESS));
                cboSensorType.Items.Add(new Sensor("외부 비상벨", SensorType.EMPOLL));             
            }
            else if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
            {
                cboSensorType.Items.Add(new Sensor("모두", SensorType.ALL));
                cboSensorType.Items.Add(new Sensor("CCTV", SensorType.CCTV));
                cboSensorType.Items.Add(new Sensor("화재센서", SensorType.DETECT_FIRE));
                cboSensorType.Items.Add(new Sensor("여자화장실비상벨", SensorType.S1ACCESS));
            }
            else if(UnE.SOP.ProxySOP.Instance.SiteID == 101)             
            {
                cboSensorType.Items.Add(new Sensor("모두", SensorType.ALL));
                cboSensorType.Items.Add(new Sensor("CCTV", SensorType.CCTV));
                cboSensorType.Items.Add(new Sensor("화재센서", SensorType.DETECT_FIRE));               
                cboSensorType.Items.Add(new Sensor("외부 비상벨", SensorType.EMPOLL));
                cboSensorType.Items.Add(new Sensor("여자화장실비상벨", SensorType.SECOM));
            }
            else if (UnE.SOP.ProxySOP.Instance.SiteID == 200)
            {
                cboSensorType.Items.Add(new Sensor("모두", SensorType.ALL));
                cboSensorType.Items.Add(new Sensor("CCTV", SensorType.CCTV));
                cboSensorType.Items.Add(new Sensor("화재센서", SensorType.DETECT_FIRE));
            }
            else
            {
                cboSensorType.Items.Add(new Sensor("모두", SensorType.ALL));
                cboSensorType.Items.Add(new Sensor("화재센서", SensorType.DETECT_FIRE));
                cboSensorType.Items.Add(new Sensor("스프링쿨러", SensorType.COOLER));
                cboSensorType.Items.Add(new Sensor("펌프압력", SensorType.PUMP));
                cboSensorType.Items.Add(new Sensor("CCTV", SensorType.CCTV));
                cboSensorType.Items.Add(new Sensor("소화기", SensorType.FE));
                cboSensorType.Items.Add(new Sensor("소화전", SensorType.HD));
                cboSensorType.Items.Add(new Sensor("발신기", SensorType.FA));
                cboSensorType.Items.Add(new Sensor("수신기", SensorType.FR));
                cboSensorType.Items.Add(new Sensor(GetSensorTypeString((int)IFacility.FacilityType.PSM_SENSOR), SensorType.PSM_SENSOR));
                cboSensorType.Items.Add(new Sensor(GetSensorTypeString((int)IFacility.FacilityType.DISASTER_PREVENTION_EQUIPMENT), SensorType.DISASTER_PREVENTION_EQUIPMENT));
            }
			/*cboSensorType.Items.Add("모두");
			cboSensorType.Items.Add("화재센서");
			cboSensorType.Items.Add("스프링쿨러");
			cboSensorType.Items.Add("펌프압력");
			cboSensorType.Items.Add("CCTV");
			cboSensorType.Items.Add("소화기");
			cboSensorType.Items.Add("소화전");
			cboSensorType.Items.Add("발신기");
            cboSensorType.Items.Add("수신기");
            cboSensorType.Items.Add(GetSensorTypeString((int)IFacility.FacilityType.PSM_SENSOR));
            cboSensorType.Items.Add(GetSensorTypeString((int)IFacility.FacilityType.DISASTER_PREVENTION_EQUIPMENT));*/

			cboStatus.Items.Add("모두");
			cboStatus.Items.Add("정상");
			cboStatus.Items.Add("비정상");

            cboPSMSensorStatus.Items.Add("모두");
            cboPSMSensorStatus.Items.Add("On");
            cboPSMSensorStatus.Items.Add("Off");

            BindingComboBoxPSMSensorLocation();
            BindingComboBoxDisasterPreventionEquipmentLocation();

			cboSensorType.SelectedIndex = 0;
			cboStatus.SelectedIndex = 0;
            cboPSMSensorLocations.SelectedIndex = 0;
            cboPSMSensorStatus.SelectedIndex = 0;
            cboDisasterPreventionEquipmentLocation.SelectedIndex = 0;

            cboPSMSensorLocations.Location = cboBuildingGroup.Location;
            cboPSMSensorLocations.Size = new System.Drawing.Size(cboFloor.Location.X + cboFloor.Size.Width - cboBuildingGroup.Location.X, cboPSMSensorLocations.Size.Height);

            cboDisasterPreventionEquipmentLocation.Location = cboBuildingGroup.Location;
            cboDisasterPreventionEquipmentLocation.Size = new System.Drawing.Size(cboFloor.Location.X + cboFloor.Size.Width - cboBuildingGroup.Location.X, cboPSMSensorLocations.Size.Height);

            cboPSMSensorStatus.Location = cboStatus.Location;
            cboPSMSensorStatus.Size = cboStatus.Size;

            gvDisasterPreventionEquipment.Location = gvSensorList.Location;
            gvDisasterPreventionEquipment.Size = gvSensorList.Size;
		}

        private void InitGrid()
        {
            colNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colName.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //colName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colStatus.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colStatus.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colBuilding.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colBuilding.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colFloor.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colFloor.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colETC.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colETC.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
             
            colDisasterPreventionEquipmentNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDisasterPreventionEquipmentNo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colDisasterPreventionEquipmentType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDisasterPreventionEquipmentType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colDisasterPreventionEquipmentName.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDisasterPreventionEquipmentName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colDisasterPreventionEquipmentLocation.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDisasterPreventionEquipmentLocation.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colDisasterPreventionEquipmentQuantity.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDisasterPreventionEquipmentQuantity.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colDisasterPreventionEquipmentDescription.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDisasterPreventionEquipmentDescription.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            BindingGridComboBox();
        }

        private void BindingGridComboBox()
        {
            string strSelectedLocationName = cboDisasterPreventionEquipmentLocation.Text;

            colDisasterPreventionEquipmentType.Items.Clear();
            colDisasterPreventionEquipmentLocation.Items.Clear();

            foreach (DisasterPreventionEquipmentType item in FormMain.Instance.DataManager.GetDisasterPreventionEquipmentType().Values)
            {
                colDisasterPreventionEquipmentType.Items.Add(item.Name);
            }
            foreach (DisasterPreventionEquipmentLocation item in FormMain.Instance.DataManager.GetDisasterRreventionEquipmentLocation().Values)
            {
                colDisasterPreventionEquipmentLocation.Items.Add(item.Name);
            }

            BindingComboBoxDisasterPreventionEquipmentLocation();
            cboDisasterPreventionEquipmentLocation.SelectedItem = strSelectedLocationName;
        }


        private void BindingComboBoxPSMSensorLocation()
        {
            string strSQL = "Select ID from EquipmentZone where Type = 2 and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            cboPSMSensorLocations.Items.Add("모두");
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nID);

                if (equipZone != null)
                    cboPSMSensorLocations.Items.Add(equipZone);
            }
        }

        private void BindingComboBoxDisasterPreventionEquipmentLocation()
        {
            cboDisasterPreventionEquipmentLocation.SelectedIndex = -1;
            cboDisasterPreventionEquipmentLocation.Items.Clear();
            cboDisasterPreventionEquipmentLocation.Items.Add("모두");

            foreach (DisasterPreventionEquipmentLocation ob in from locationInfos in FormMain.Instance.DataManager.GetDisasterRreventionEquipmentLocation().Values.ToArray<DisasterPreventionEquipmentLocation>()
                                                               orderby locationInfos.Index ascending
                                                               select locationInfos
                                                          )
            {
                cboDisasterPreventionEquipmentLocation.Items.Add(ob);
            }

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
            int nSelectedTypeIndex = cboSensorType.SelectedIndex;

            if (nSelectedTypeIndex < 0)
                return;

            Sensor sensor = (Sensor)cboSensorType.Items[nSelectedTypeIndex];

            if (nSelectedTypeIndex <= 0)
            {
                lblFacilityName.Text = "모든 설비 목록";
            }
            else
            {
                lblFacilityName.Text = String.Format("{0} 목록", cboSensorType.SelectedItem.ToString());
            }

            gvSensorList.DataSource = null;
            sensorListGridDataBindingSource.Clear();
            gvSensorList.Invalidate();
            //gvSensorList.Rows.Clear();
            gvDisasterPreventionEquipment.Rows.Clear();

            int no = 1;

            if (sensor.Type == SensorType.PSM_SENSOR)
            {
                colBuilding.HeaderText = "위치";
                colFloor.Visible = false;

                gvSensorList.Visible = true;
                gvDisasterPreventionEquipment.Visible = false;

                EquipmentZone equipZone = null;

                if (cboPSMSensorLocations.SelectedIndex > 0)
                    equipZone = (EquipmentZone)cboPSMSensorLocations.Items[cboPSMSensorLocations.SelectedIndex];

                LoadPSMSensorData(ref no, equipZone, cboPSMSensorStatus.SelectedIndex);
            }
            else if (sensor.Type == SensorType.DISASTER_PREVENTION_EQUIPMENT)
            {
                gvDisasterPreventionEquipment.Visible = true;
                gvSensorList.Visible = false;

                LoadDisasterPreventionEquipmentData(ref no);
            }
            else
            {
                colBuilding.HeaderText = "건물";
                colFloor.Visible = true;

                gvSensorList.Visible = true;
                gvDisasterPreventionEquipment.Visible = false;

                Dictionary<int, Zone> dicZones = null;
                Dictionary<int, EquipmentZone> dicEquipZones = null;
                GetZoneCondition(ref dicZones, ref dicEquipZones);

                if (sensor.Type == SensorType.ALL)
                {
                    LoadSensorData(ref no, dicEquipZones, sensor.Type);
                    LoadPSMSensorData(ref no);
                    LoadCCTVData(ref no, dicZones);
                    LoadFireEquipmentData(ref no, sensor.Type, dicZones);
                }
                else if (sensor.Type >= SensorType.DETECT_FIRE && sensor.Type <= SensorType.PUMP)
                    LoadSensorData(ref no, dicEquipZones, sensor.Type);
                else if (sensor.Type == SensorType.CCTV)
                    LoadCCTVData(ref no, dicZones);
                else if (sensor.Type >= SensorType.FE && sensor.Type <= SensorType.FR)
                    LoadFireEquipmentData(ref no, sensor.Type, dicZones);
                else
                    LoadSensorData(ref no, dicEquipZones, sensor.Type);
                /*ArrayList arrZones = null;
                string strCondition = "", strCondition2 = "", strEquipZoneCondition = "";
                string strZoneCondition = GetZoneConditionString(ref arrZones, ref strEquipZoneCondition);

                if (strZoneCondition.Length > 0)
                    AddConditionString(ref strCondition, strZoneCondition);

                if (strEquipZoneCondition.Length > 0)
                    AddConditionString(ref strCondition2, strEquipZoneCondition);

                if (sensor.Type == SensorType.ALL)
                {
                    LoadSensorData(ref no, strCondition2, sensor.Type);
                    LoadPSMSensorData(ref no);
                    LoadCCTVData(ref no, strCondition);
                    LoadFireEquipmentData(ref no, strCondition, sensor.Type, arrZones);
                }
                else if (sensor.Type >= SensorType.DETECT_FIRE && sensor.Type <= SensorType.PUMP)
                    LoadSensorData(ref no, strCondition2, sensor.Type);
                else if (sensor.Type == SensorType.CCTV)
                    LoadCCTVData(ref no, strCondition);
                else if (sensor.Type >= SensorType.FE && sensor.Type <= SensorType.FR)
                    LoadFireEquipmentData(ref no, strCondition, sensor.Type, arrZones);
                else
                    LoadSensorData(ref no, strCondition2, sensor.Type);*/
            }
             
            gvSensorList.DataSource = sensorListGridDataBindingSource;
		}

        #region ReadOnly GridView

        // nSensorStatus : 0(모두), 1(On), 2(Off)
        private void LoadPSMSensorData(ref int no, EquipmentZone equipZone = null, int nSensorStatus = 0)
        {
            List<UnE.PSM.PSMSensor> sensors = PSMManager.Instance.GetSensors();

            if (sensors == null)
                return;

            List<UnE.PSM.PSMSensor> removeList = new List<UnE.PSM.PSMSensor>();

            if (equipZone != null)
            {
                foreach (UnE.PSM.PSMSensor sensor in sensors)
                {
                    if (sensor.EquipZoneID != equipZone.ID)
                        removeList.Add(sensor);
                    else
                    {
                        if (nSensorStatus == 1)      // On
                        {
                            if (sensor.SensorStatus != UnE.PSM.PSMSensor.Status.On)
                                removeList.Add(sensor);
                        }
                        else if (nSensorStatus == 2) // Off
                        {
                            if (sensor.SensorStatus != UnE.PSM.PSMSensor.Status.Off)
                                removeList.Add(sensor);
                        }
                    }
                }
            }
            else
            {
                foreach (UnE.PSM.PSMSensor sensor in sensors)
                {
                    if (nSensorStatus == 1)      // On
                    {
                        if (sensor.SensorStatus != UnE.PSM.PSMSensor.Status.On)
                            removeList.Add(sensor);
                    }
                    else if (nSensorStatus == 2) // Off
                    {
                        if (sensor.SensorStatus != UnE.PSM.PSMSensor.Status.Off)
                            removeList.Add(sensor);
                    }
                }
            }

            foreach (UnE.PSM.PSMSensor sensor in removeList)
            {
                sensors.Remove(sensor);
            }

            foreach (UnE.PSM.PSMSensor sensor in sensors)
            {
                string strStatus = "Unknown";
                string strLocationName = "", strMaterialName = "";

                if (sensor.SensorStatus == UnE.PSM.PSMSensor.Status.On)
                    strStatus = "On";
                else if (sensor.SensorStatus == UnE.PSM.PSMSensor.Status.LocalOff ||
                    sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off ||
                    sensor.SensorStatus == UnE.PSM.PSMSensor.Status.Off4Work)
                    strStatus = "Off";

                if (sensor.LinkedTankList != null && sensor.LinkedTankList.Count > 0)
                {
                    UnE.PSM.PSMTank tank = sensor.LinkedTankList[0];
                    strLocationName = tank.LocationName;

                    if (tank.Material != null)
                        strMaterialName = tank.Material.Name;
                }
                string szName = sensor.Name;

                // 유해화학물질 센서 설치장소 표시를 LocationName 수정 20200305 K.D.R
                strMaterialName = strLocationName;
                    
                AddGridData(ref no, GetSensorTypeString((int)IFacility.FacilityType.PSM_SENSOR), szName, strStatus, strLocationName, "", strMaterialName, null, equipZone, SensorType.PSM_SENSOR);
            }
        }

		private void LoadFireEquipmentHistory(Dictionary<int, FireEquipmentHistoryData> dicEquipStatus, SensorType sensorType)
		{
			string strSQL = "";
            string szText = "";
			if (sensorType == SensorType.ALL)
            {
                //strSQL = "Select FireEquipmentID, Time, Status, CheckersOpinion from FireEquipmentHistory order by FireEquipmentID";
                szText = "SELECT fsh.FireEquipmentID, fsh.Time, fsh.Status, fsh.CheckersOpinion FROM FireEquipmentHistory AS fsh " +
                         " INNER JOIN FireEquipment AS fe ON fsh.FireEquipmentID = fe.ID " +
                         " INNER JOIN Zone AS z ON fe.ZoneID = z.ID AND z.SiteID = {0} " +
                         " ORDER BY FireEquipmentID";
            }

            else if (sensorType == SensorType.FE)
            {
                //strSQL = "Select FireEquipmentID, Time, Status, CheckersOpinion from FireEquipmentHistory as fsh, FireEquipment as fe where fsh.FireEquipmentID = fe.ID and fe.EquipType = 1 order by FireEquipmentID";
                szText = "SELECT fsh.FireEquipmentID, fsh.Time, fsh.Status, fsh.CheckersOpinion FROM FireEquipmentHistory AS fsh " +
                         " INNER JOIN FireEquipment AS fe ON fsh.FireEquipmentID = fe.ID AND fe.EquipType = " + ((int)IFacility.FacilityType.FE).ToString() +
                         " INNER JOIN Zone AS z ON fe.ZoneID = z.ID AND z.SiteID = {0}" +
                         " ORDER BY FireEquipmentID ";
            }
            else if (sensorType == SensorType.HD)
            {
                //strSQL = "Select FireEquipmentID, Time, Status, CheckersOpinion from FireEquipmentHistory, FireEquipment where FireEquipmentHistory.FireEquipmentID = FireEquipment.ID and FireEquipment.EquipType = 2 order by FireEquipmentID";
                szText = "SELECT fsh.FireEquipmentID, fsh.Time, fsh.Status, fsh.CheckersOpinion FROM FireEquipmentHistory AS fsh " +
                         " INNER JOIN FireEquipment AS fe ON fsh.FireEquipmentID = fe.ID AND fe.EquipType = " + ((int)IFacility.FacilityType.HD).ToString() +
                         " INNER JOIN Zone AS z ON fe.ZoneID = z.ID AND z.SiteID = {0}" +
                         " ORDER BY FireEquipmentID ";
            }
            else if (sensorType == SensorType.FA)
            {
                //strSQL = "Select FireEquipmentID, Time, Status, CheckersOpinion from FireEquipmentHistory, FireEquipment where FireEquipmentHistory.FireEquipmentID = FireEquipment.ID and FireEquipment.EquipType = 3 order by FireEquipmentID";
                szText = "SELECT fsh.FireEquipmentID, fsh.Time, fsh.Status, fsh.CheckersOpinion FROM FireEquipmentHistory AS fsh " +
                         " INNER JOIN FireEquipment AS fe ON fsh.FireEquipmentID = fe.ID AND fe.EquipType = " + ((int)IFacility.FacilityType.FA).ToString() +
                         " INNER JOIN Zone AS z ON fe.ZoneID = z.ID AND z.SiteID = {0}" +
                         " ORDER BY FireEquipmentID ";
            }
            else if (sensorType == SensorType.FR)
            {
                //strSQL = "Select FireEquipmentID, Time, Status, CheckersOpinion from FireEquipmentHistory, FireEquipment where FireEquipmentHistory.FireEquipmentID = FireEquipment.ID and FireEquipment.EquipType = 3 order by FireEquipmentID";
                szText = "SELECT fsh.FireEquipmentID, fsh.Time, fsh.Status, fsh.CheckersOpinion FROM FireEquipmentHistory AS fsh " +
                         " INNER JOIN FireEquipment AS fe ON fsh.FireEquipmentID = fe.ID AND fe.EquipType = " + ((int)IFacility.FacilityType.FR).ToString() +
                         " INNER JOIN Zone AS z ON fe.ZoneID = z.ID AND z.SiteID = {0}" +
                         " ORDER BY FireEquipmentID ";
            }
            else
                return;

            strSQL = string.Format(szText, m_nSiteID);

			ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);
			if (arrResult == null)
				return;

			int nPrevEquipID = -1;
			DateTime dtPrev = new DateTime();

			int nResultCount = arrResult.Count;
			DateTime dtDefault = new DateTime();

			for (int i = 0; i < nResultCount - 3; i += 4)
			{
				int nEquipID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 1], dtDefault);
				int nStatus = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
				string strOpinion = WebDBManager.GetStringField(arrResult[i + 3], "");

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

        private void LoadFireEquipmentData(ref int no, SensorType sensorType, Dictionary<int, Zone> dicZones)
        {
            int nStatusIndex = cboStatus.SelectedIndex;

            Dictionary<int, FireEquipmentHistoryData> dicEquipStatus = new Dictionary<int, FireEquipmentHistoryData>();
            LoadFireEquipmentHistory(dicEquipStatus, sensorType);

            Dictionary<Zone, ArrayList> dicEquipments = FormMain.Instance.DataManager.ZoneFireEquipments;
            string[] arrStatus = new string[] { "정상", "고장", "수리중", "기타" };

            foreach (KeyValuePair<Zone, ArrayList> pair in dicEquipments)
            {
                if (dicZones != null && !dicZones.ContainsKey(pair.Key.ID))
                    continue;

                foreach (FireEquipment equip in pair.Value)
                {
                    if (sensorType == SensorType.FE)
                    {
                        if (equip.Type != IFacility.FacilityType.FE)
                            continue;
                    }
                    else if (sensorType == SensorType.HD)
                    {
                        if (equip.Type != IFacility.FacilityType.HD)
                            continue;
                    }
                    else if (sensorType == SensorType.FA)
                    {
                        if (equip.Type != IFacility.FacilityType.FA)
                            continue;
                    }
                    else if (sensorType == SensorType.FR)
                    {
                        if (equip.Type != IFacility.FacilityType.FR)
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
                    string strSensorType = GetSensorTypeString((int)equip.Type);
                    string szName = equip.TagID;
                    string szLocation = equip.Zone.DisplayText;

                    AddGridData(ref no, strSensorType, szName, strStatus, equip.Zone.Building == null ? equip.Zone.DisplayText : equip.Zone.Building.BuildingName, equip.Zone.Floor.ToString(), szLocation, equip.Zone, null, sensorType);
                }
            }
        }

		private void LoadFireEquipmentData(ref int no, string strCondition, SensorType sensorType, ArrayList arrZones)
		{
			int nStatusIndex = cboStatus.SelectedIndex;

			Dictionary<int, FireEquipmentHistoryData> dicEquipStatus = new Dictionary<int, FireEquipmentHistoryData>();
            LoadFireEquipmentHistory(dicEquipStatus, sensorType);

			Dictionary<Zone, ArrayList> dicEquipments = FormMain.Instance.DataManager.ZoneFireEquipments;
			string[] arrStatus = new string[] { "정상", "고장", "수리중", "기타" };

			foreach (KeyValuePair<Zone, ArrayList> pair in dicEquipments)
			{
				if (arrZones != null && !arrZones.Contains(pair.Key))
					continue;

				foreach (FireEquipment equip in pair.Value)
				{
                    if (sensorType == SensorType.FE)
					{
                        if (equip.Type != IFacility.FacilityType.FE)
							continue;
					}
                    else if (sensorType == SensorType.HD)
					{
                        if (equip.Type != IFacility.FacilityType.HD)
							continue;
					}
                    else if (sensorType == SensorType.FA)
					{
                        if (equip.Type != IFacility.FacilityType.FA)
							continue;
					}
                    else if (sensorType == SensorType.FR)
                    {
                        if (equip.Type != IFacility.FacilityType.FR)
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
                    string strSensorType = GetSensorTypeString((int)equip.Type);
                    string szName = equip.TagID;
                    string szLocation = equip.Zone.DisplayText;
                    
                    AddGridData(ref no, strSensorType, szName, strStatus, equip.Zone.Building == null ? equip.Zone.DisplayText : equip.Zone.Building.BuildingName, equip.Zone.Floor.ToString(), szLocation, equip.Zone, null, sensorType);
				}
			}
		}

        private void LoadCCTVData(ref int no, Dictionary<int, Zone> dicZones)
        {
            if (cboStatus.SelectedIndex == 2)
                return;

            if (m_allSensorDatas != null)
            {
                foreach (Admin.SensorListGridData data in m_allSensorDatas)
                {
                    if (data.SensorTypeID != (int)SensorType.CCTV)
                        continue;

                    if (dicZones != null)
                    {
                        if (data.Zone == null)
                            continue;

                        if (dicZones.ContainsKey(data.Zone.ID) == false)
                            continue;
                    }

                    data.No = no++;
                    sensorListGridDataBindingSource.Add(data);
                }
                return;
            }

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            //string strSQL = string.Format("select IPAddr, ZoneID from CCTV {0} order by ZoneID", strCondition);

            string szText = "select IPAddr, ZoneID, CameraName from CCTV inner join Zone as z on z.ID = ZoneID and z.SiteID = {0} order by ZoneID";
            string strSQL = string.Format(szText, m_nSiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                string strIP = WebDBManager.GetStringField(arrResult[i], "");
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string szName = WebDBManager.GetStringField(arrResult[i + 2], "");

                if (dicZones != null)
                {
                    if (dicZones.ContainsKey(nZoneID) == false)
                        continue;
                }

                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                if (zone == null)
                    continue;

                AddGridData(ref no, "CCTV", szName, "정상", zone.Building == null ? zone.DisplayText : zone.Building.BuildingName, zone.Floor.ToString(), strIP, zone, null, SensorType.CCTV);
            }
        }

		private void LoadCCTVData(ref int no, string strCondition)
		{
			if (cboStatus.SelectedIndex == 2)
				return;

			if (strCondition.Length > 0)
				strCondition = "where " + strCondition;

			WebDBManager dbMgr = FormMain.Instance.DBManager;

			//string strSQL = string.Format("select IPAddr, ZoneID from CCTV {0} order by ZoneID", strCondition);

            string szText = "select IPAddr, ZoneID, CameraName from CCTV inner join Zone as z on z.ID = ZoneID and z.SiteID = {0} {1} order by ZoneID";
            string strSQL = string.Format(szText, m_nSiteID, strCondition);

			ArrayList arrResult = dbMgr.GetResultData(strSQL);

			if (arrResult == null)
				return;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 2; i += 3)
			{
				string strIP = WebDBManager.GetStringField(arrResult[i], "");
				int nZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string szName = WebDBManager.GetStringField(arrResult[i + 2], "");
				Zone zone = ZoneManager.Instance.GetZone(nZoneID);
				if (zone == null)
					continue;


                AddGridData(ref no, "CCTV", szName, "정상", zone.Building == null ? zone.DisplayText : zone.Building.BuildingName, zone.Floor.ToString(), strIP, zone, null, SensorType.CCTV);
			}
		}

        private void LoadSensorData(ref int no, Dictionary<int, EquipmentZone> dicEquipZones, SensorType sensorType)
        {
            if (m_allSensorDatas != null)
            {
                foreach (Admin.SensorListGridData data in m_allSensorDatas)
                {
                    if (sensorType != SensorType.ALL && data.SensorTypeID != (int)sensorType)
                        continue;

                    if (dicEquipZones != null)
                    {
                        if (data.EquipmentZone == null)
                            continue;

                        if (dicEquipZones.ContainsKey(data.EquipmentZone.ID) == false)
                            continue;
                    }

                    data.No = no++;

                    sensorListGridDataBindingSource.Add(data);
                }
                return;
            }

            string strCondition = "";

            if (sensorType > SensorType.ALL)
            {
                string strTypeCondition = "";

                if (sensorType == SensorType.SVMS)
                {
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.Intrusion_S1).ToString());
                }
                else if (sensorType == SensorType.S1ACCESS)
                {
                    // Access는 12가지 타입의 센서가 존재하는데
                    // 사실 1개의 S1Access Sensor에 대하여 12가지 타입의 SensorZone이 존재하는 것이므로
                    // 대표 Type 하나만 사용하도록 한다.
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.GeneralIntrusionT1_S1).ToString());
                }
                else if (sensorType == SensorType.EMPOLL)
                {
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.ExternalAlarmBell).ToString());
                 
                }                
                else if (sensorType == SensorType.SECOM)
                {
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.SecomExternalAlarmBell).ToString(), false);       //여자 비상벨 센서
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.SecomWomenAlarmBell).ToString(), false);       //여자 화장실 센서
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

            if (cboStatus.SelectedIndex == 1)
            {
                string strDataCondition = "Connected = 1 and Data = 0";
                AddConditionString(ref strCondition, strDataCondition);
            }
            else if (cboStatus.SelectedIndex == 2)
            {
                string strDataCondition = "(Connected = 0 or Data = 1)";
                AddConditionString(ref strCondition, strDataCondition);
            }

            if (strCondition.Length > 0)
                strCondition = "where " + strCondition;

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            //string strSQL = string.Format("Select ID, Type, Connected, Zone, Data, EquipZoneID from SensorZone {0} order by Zone", strCondition);
            string szText = "Select sz.ID, sz.Type, sz.Connected, sz.Zone, sz.Data, sz.EquipZoneID, sti.SensorName from SensorZone as sz " +
                            "INNER JOIN SensorTagInfo as sti on sti.SensorZoneID = sz.ID " +
                            "INNER JOIN Zone as z on z.ID = sz.Zone and z.SiteID = {0} {1} order by sz.Type, sz.Zone";

            string strSQL = string.Format(szText, m_nSiteID, strCondition);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                bool isConnected = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nData = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                string szSensorName = WebDBManager.GetStringField(arrResult[i + 6], "");
                if (nID == 0)
                    continue;

                if (nType < 0 || nZoneID < 0)
                    continue;

                if (dicEquipZones != null)
                {
                    if (dicEquipZones.ContainsKey(nEquipZoneID) == false)
                        continue;
                }

                // PSMSensor는 LoadPSMSensorData()에서 처리함
                if (nType == (int)IFacility.FacilityType.PSM_SENSOR)
                    continue;

                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                if (zone == null)
                    continue;

                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                string szEquipZoneName = (equipZone != null ? equipZone.DisplayText : "");

                SensorType sensorType2 = GetSensorType(nType);
                string strSensorType = GetSensorTypeString(nType);

                if (strSensorType.Length == 0)
                    continue;

                string strStatus = GetSensorStatusString(nType, isConnected, nData);

                if (zone.Building == null)
                    AddGridData(ref no, strSensorType, szSensorName, strStatus, zone.DisplayText, "", szEquipZoneName, zone, equipZone, sensorType2);
                else
                    AddGridData(ref no, strSensorType, szSensorName, strStatus, zone.Building.BuildingName, zone.Floor.ToString(), szEquipZoneName, zone, equipZone, sensorType2);
            }
        }

		private void LoadSensorData(ref int no, string strCondition, SensorType sensorType)
		{
			if (sensorType > SensorType.ALL)
			{
                string strTypeCondition = "";

                if (sensorType == SensorType.SVMS)
                {
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.Intrusion_S1).ToString());
                }
                else if (sensorType == SensorType.S1ACCESS)
                {
                    // Access는 12가지 타입의 센서가 존재하는데
                    // 사실 1개의 S1Access Sensor에 대하여 12가지 타입의 SensorZone이 존재하는 것이므로
                    // 대표 Type 하나만 사용하도록 한다.
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.GeneralIntrusionT1_S1).ToString());
                }
                else if (sensorType == SensorType.EMPOLL)
                {
                    AddConditionString(ref strTypeCondition, "Type = " + ((int)IFacility.FacilityType.ExternalAlarmBell).ToString());
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

			if (cboStatus.SelectedIndex == 1)
			{
				string strDataCondition = "Connected = 1 and Data = 0";
				AddConditionString(ref strCondition, strDataCondition);
			}
			else if (cboStatus.SelectedIndex == 2)
			{
				string strDataCondition = "(Connected = 0 or Data = 1)";
				AddConditionString(ref strCondition, strDataCondition);
			}

			if (strCondition.Length > 0)
				strCondition = "where " + strCondition;

			WebDBManager dbMgr = FormMain.Instance.DBManager;

            //string strSQL = string.Format("Select ID, Type, Connected, Zone, Data, EquipZoneID from SensorZone {0} order by Zone", strCondition);
            string szText = "Select sz.ID, sz.Type, sz.Connected, sz.Zone, sz.Data, sz.EquipZoneID, sti.SensorName from SensorZone as sz " +
                            "INNER JOIN SensorTagInfo as sti on sti.SensorZoneID = sz.ID " +
                            "INNER JOIN Zone as z on z.ID = sz.Zone and z.SiteID = {0} {1} order by sz.Type, sz.Zone";

            string strSQL = string.Format(szText, m_nSiteID, strCondition);

			ArrayList arrResult = dbMgr.GetResultData(strSQL);

			if (arrResult == null)
				return;

			int nResultCount = arrResult.Count;

			for (int i = 0; i < nResultCount - 6; i += 7)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
				int nType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
				bool isConnected = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
				int nZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
				int nData = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                string szSensorName = WebDBManager.GetStringField(arrResult[i + 6], "");
				if (nID == 0)
					continue;

				if (nType < 0 || nZoneID < 0)
					continue;

                // PSMSensor는 LoadPSMSensorData()에서 처리함
                if (nType == (int)IFacility.FacilityType.PSM_SENSOR)
                    continue;
                                
				Zone zone = ZoneManager.Instance.GetZone(nZoneID);
				if (zone == null)
					continue;

                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                string szEquipZoneName = (equipZone != null ? equipZone.DisplayText : "");

                SensorType sensorType2 = GetSensorType(nType);
				string strSensorType = GetSensorTypeString(nType);

                if (strSensorType.Length == 0)
                    continue;

				string strStatus = GetSensorStatusString(nType, isConnected, nData);

                if (zone.Building == null)
                    AddGridData(ref no, strSensorType, szSensorName, strStatus, zone.DisplayText, "", szEquipZoneName, zone, equipZone, sensorType2);
                else
                    AddGridData(ref no, strSensorType, szSensorName, strStatus, zone.Building.BuildingName, zone.Floor.ToString(), szEquipZoneName, zone, equipZone, sensorType2);
			}
		}

		private void AddGridData(ref int no, string strSensorType, string szName, string strStatus, string strBuildingName, string strFloor, string strETC, Zone zone, EquipmentZone equipZone, SensorType sensorType)
		{
            if (strBuildingName == "NOT_USE")
                strBuildingName = "실외";

            if (strETC == "NOT_USE")
                strETC = "알수 없음";

            Admin.SensorListGridData data = new Admin.SensorListGridData();

            data.No = no++;
            data.Type = strSensorType;
            data.Name = szName;
            data.Status = strStatus;
            data.Building = strBuildingName;
            data.Floor = strFloor;
            data.Description = strETC;
            data.SensorTypeID = (int)sensorType;
            data.Zone = zone;
            data.EquipmentZone = equipZone;

            sensorListGridDataBindingSource.Add(data);
			/*int nID = gvSensorList.Rows.Count + 1;

			DataGridViewRow row = new DataGridViewRow();

			DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
			cell.Value = nID;
            cell.ToolTipText = nID.ToString();
			row.Cells.Add(cell);

			cell = new DataGridViewTextBoxCell();
			cell.Value = strSensorType;
            cell.ToolTipText = strSensorType;
			row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = szName;
            cell.ToolTipText = szName;
            row.Cells.Add(cell);

			cell = new DataGridViewTextBoxCell();
			cell.Value = strStatus;
            cell.ToolTipText = strStatus;
			row.Cells.Add(cell);


            if (strBuildingName == "NOT_USE")
                strBuildingName = "실외";
			cell = new DataGridViewTextBoxCell();
			cell.Value = strBuildingName;
            cell.ToolTipText = strBuildingName;
			row.Cells.Add(cell);

			cell = new DataGridViewTextBoxCell();
			cell.Value = strFloor;
            cell.ToolTipText = strFloor;
			row.Cells.Add(cell);


            if (strETC == "NOT_USE")
                strETC = "알수 없음";
			cell = new DataGridViewTextBoxCell();
			cell.Value = strETC;
            cell.ToolTipText = strETC;
			row.Cells.Add(cell);

			gvSensorList.Rows.Add(row);*/
		}

        #endregion ReadOnly GridView

        #region Editabel GridView

        private void LoadDisasterPreventionEquipmentData(ref int no)
        {
            gvDisasterPreventionEquipment.AllowUserToAddRows = false;

            List<DisasterPreventionEquipment> liData = new List<DisasterPreventionEquipment>();

            // 시설위치에 관계없이 모든 데이터 조회
            if (cboDisasterPreventionEquipmentLocation.SelectedIndex <= 0)
            {
                foreach (DisasterPreventionEquipment data in from datas in FormMain.Instance.DataManager.GetDisasterPreventionEquipment().Values.ToArray<DisasterPreventionEquipment>()
                                                             orderby datas.Index ascending
                                                             select datas
                                                            )
                {
                    liData.Add(data);
                }
            }
            else
            {
                string strLocation = cboDisasterPreventionEquipmentLocation.Text;

                foreach (DisasterPreventionEquipment data in from datas in FormMain.Instance.DataManager.GetDisasterPreventionEquipment().Values.ToArray<DisasterPreventionEquipment>()
                                                             where datas.Location != null
                                                             && datas.Location.Name == strLocation
                                                             orderby datas.Index ascending
                                                             select datas
                                                            )
                {
                    liData.Add(data);
                }
            }

            foreach (DisasterPreventionEquipment data in liData)
            {
                AddGridData(ref no, data);
            }

            gvDisasterPreventionEquipment.EndEdit();
            gvDisasterPreventionEquipment.AllowUserToAddRows = true;
        }

        private void AddGridData(ref int no, DisasterPreventionEquipment data)
        {
            gvDisasterPreventionEquipment.Rows.Add();

            int nID = no++;//gvDisasterPreventionEquipment.Rows.Count;

            DataGridViewRow row = gvDisasterPreventionEquipment.Rows[gvDisasterPreventionEquipment.Rows.Count - 1];

            row.Tag = data;
            row.Cells[0].Value = nID;
            if (data.Type != null)
            {
                row.Cells[1].Value = data.Type.Name;
                row.Cells[1].Tag = data.Type;
            }
            row.Cells[2].Value = data.Name;
            if (data.Location != null)
            {
                row.Cells[3].Value = data.Location.Name;
                row.Cells[3].Tag = data.Location;
            }
            row.Cells[4].Value = data.Quantity;
            row.Cells[5].Value = data.Description;

        }

        public void SaveData()
        {
            if (ValildDisasterPreventionEquipmentData() == false)
                return;

            Dictionary<int, DisasterPreventionEquipment> dicDisasterPreventionEquipment = FormMain.Instance.DataManager.GetDisasterPreventionEquipment();
            foreach (DataGridViewRow row in gvDisasterPreventionEquipment.Rows)
            {
                if (row.IsNewRow == true) continue;

                DisasterPreventionEquipment disasterPreventionEquipment = null;

                // new
                if (row.Tag == null)
                    disasterPreventionEquipment = FormMain.Instance.DataManager.AddDisasterPreventionEquipment();
                else
                    disasterPreventionEquipment = row.Tag as DisasterPreventionEquipment;

                disasterPreventionEquipment.Type = row.Cells[1].Tag as DisasterPreventionEquipmentType;
                disasterPreventionEquipment.Location = row.Cells[2].Tag as DisasterPreventionEquipmentLocation;
                disasterPreventionEquipment.Quantity = Convert.ToInt32(row.Cells[3].Value.ToString());
                disasterPreventionEquipment.Description = row.Cells[4].Value.ToString();

            }

            FormMain.Instance.DataManager.SaveDisasterPreventionEquipment();
            BindingGridComboBox();
        }

        private bool ValildDisasterPreventionEquipmentData()
        {
            bool isValid = true;

            foreach (DataGridViewRow row in gvDisasterPreventionEquipment.Rows)
            {
                if (row.Tag == null) continue;

                if (row.Cells[1].Tag == null)
                {
                    MessageBox.Show("유형을 선택하세요.");
                    isValid = false;
                    break;
                }
                else if (row.Cells[2].Tag == null)
                {
                    MessageBox.Show("시설이름을 선택하세요.");
                    isValid = false;
                    break;
                }

            }

            return isValid;
        }

        #endregion Editabel GridView

        private string GetSensorStatusString(int nType, bool isConnected, int nData)
		{
			string strStatus = "";

			if (nType == (int)IFacility.FacilityType.FIRE_SENSOR || nType == (int)IFacility.FacilityType.FireSensor_TypeA)
			{
				if (!isConnected)
					strStatus = "통신 두절";
				else if (nData == 1)
					strStatus = "화재 감지";
				else
					strStatus = "정상";
			}
			else if (nType == (int)IFacility.FacilityType.COOLER_SENSOR)
			{
				if (!isConnected)
					strStatus = "통신 두절";
				else if (nData == 1)
					strStatus = "스프링쿨러 동작중";
				else
					strStatus = "정상";
			}
			else if (nType == (int)IFacility.FacilityType.PRESSURE_SENSOR)
			{
				if (!isConnected)
					strStatus = "통신 두절";
				else if (nData == 1)
					strStatus = "펌프 압력 이상";
				else
					strStatus = "정상";
			}

            else if (nType == (int)IFacility.FacilityType.FireSensor_Monitoring
                || nType == (int)IFacility.FacilityType.FireSensor_SensingLine
                || nType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType
                || nType == (int)IFacility.FacilityType.FireSensor_MonitoringType
                || nType == (int)IFacility.FacilityType.FireSensor_GasEmission
                || nType == (int)IFacility.FacilityType.FireSensor_ManualControl
                || nType == (int)IFacility.FacilityType.FireSensor_SiemensType
                || nType == (int)IFacility.FacilityType.FireSensor_LightType)
            {
                if (!isConnected)
                    strStatus = "통신 두절";
                else if (nData == 1)
                    strStatus = "신호 감지";
                else
                    strStatus = "정상";
            }
			return strStatus;
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
            else if (nFacilityType == (int)IFacility.FacilityType.DISASTER_PREVENTION_EQUIPMENT)
                return SensorType.DISASTER_PREVENTION_EQUIPMENT;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_Monitoring)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_SensingLine)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_AnalogSmokeType)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_MonitoringType)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.CCTV)
                return SensorType.CCTV;
            else if (nFacilityType == (int)IFacility.FacilityType.FE)
                return SensorType.FE;
            else if (nFacilityType == (int)IFacility.FacilityType.HD)
                return SensorType.HD;
            else if (nFacilityType == (int)IFacility.FacilityType.FA)
                return SensorType.FA;
            else if (nFacilityType == (int)IFacility.FacilityType.FR)
                return SensorType.FR;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_GasEmission)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_ManualControl)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_SiemensType)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.FireSensor_LightType)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.Intrusion_S1)
                return SensorType.SVMS;
            else if (nFacilityType == (int)IFacility.FacilityType.GeneralIntrusionT1_S1)
                return SensorType.S1ACCESS;
            else if (nFacilityType == (int)IFacility.FacilityType.ExternalAlarmBell)
                return SensorType.EMPOLL;
            else if (nFacilityType == (int)IFacility.FacilityType.SecomWomenAlarmBell)
                return SensorType.SECOM;
            else if (nFacilityType == (int)IFacility.FacilityType.SecomExternalAlarmBell)
                return SensorType.EMPOLL;
            else if (nFacilityType == (int)IFacility.FacilityType.FireF1_S1)
                return SensorType.DETECT_FIRE;
            else if (nFacilityType == (int)IFacility.FacilityType.CustomerEmergencyC1_S1 || nFacilityType == (int)IFacility.FacilityType.CustomerEmergencyC2_S1)
                return SensorType.S1ACCESS;

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
            else if (nType == (int)IFacility.FacilityType.SecomExternalAlarmBell)       //부산대secom외부비상벨
                return "외부비상벨";
            else if (nType == (int)IFacility.FacilityType.SecomWomenAlarmBell)
                return "여자화장실비상벨";
            else if (nType == (int)IFacility.FacilityType.FireF1_S1)
                return "화재";
            else if (nType == (int)IFacility.FacilityType.SecomWomenAlarmBell)
                return "여자화장실비상벨";
            else if (nType == (int)IFacility.FacilityType.CustomerEmergencyC1_S1 || nType == (int)IFacility.FacilityType.CustomerEmergencyC2_S1)
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

        // GetZoneConditionString()은 조건문(where절)을 쿼리 형태로 나타내주는데,
        // GetZoneCondition()은 조건에 해당하는 dicZones또는 dicEquipZones 객체를 만들어준다.
        // 해당사항이 없는 객체는 null로 초기화된다.
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

                    if (cboFloor.SelectedIndex == 0)
                    {
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
                                foreach (EquipmentZone equipZone in arrEquipZones)
                                {
                                    dicEquipZones[equipZone.ID] = equipZone;
                                }
                            }
                        }
                    }
                    else
                    {
                        Floor floor = (Floor)cboFloor.Items[cboFloor.SelectedIndex];

                        if (dicZones == null)
                            dicZones = new Dictionary<int, Zone>();

                        if (dicEquipZones == null)
                            dicEquipZones = new Dictionary<int, EquipmentZone>();

                        if (floor.Zone != null)
                        {
                            dicZones[floor.Zone.ID] = floor.Zone;

                            List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(floor.Zone);

                            foreach (EquipmentZone equipZone in arrEquipZones)
                            {
                                dicEquipZones[equipZone.ID] = equipZone;
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

                                List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
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

                        List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(floor.Zone);
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

		private void AddConditionString(ref string strConditionMain, string strConditionItem, bool bAnd = true)
		{
			if (strConditionMain.Length == 0)
				strConditionMain = strConditionItem;
			else
            {
                if( bAnd == true)
                {
                    strConditionMain += " and " + strConditionItem;
                }
                else
                {
                    strConditionMain += " or " + strConditionItem;
                }
            }
				
		}

        private void cboSensorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedTypeIndex = cboSensorType.SelectedIndex;

            if (nSelectedTypeIndex < 0)
                return;

            Sensor sensor = (Sensor)cboSensorType.Items[nSelectedTypeIndex];

            if (sensor.Type == SensorType.PSM_SENSOR)
            {
                lblOperationStatus.Visible = 
                cboPSMSensorLocations.Visible = 
                cboPSMSensorStatus.Visible = true;

                cboDisasterPreventionEquipmentLocation.Visible = 
                cboBuildingGroup.Visible = 
                cboBuilding.Visible = 
                cboFloor.Visible = 
                cboStatus.Visible = false; 
            }
            else if (sensor.Type == SensorType.DISASTER_PREVENTION_EQUIPMENT)
            {
                lblOperationStatus.Visible =
                cboBuildingGroup.Visible =
                cboBuilding.Visible =
                cboFloor.Visible =
                cboStatus.Visible =
                cboPSMSensorLocations.Visible =
                cboPSMSensorStatus.Visible = false;

                cboDisasterPreventionEquipmentLocation.Visible = true; 
            }
            else
            {
                cboDisasterPreventionEquipmentLocation.Visible =
                cboPSMSensorLocations.Visible = 
                cboPSMSensorStatus.Visible = false;

                lblOperationStatus.Visible = 
                cboBuildingGroup.Visible = 
                cboBuilding.Visible = 
                cboFloor.Visible = 
                cboStatus.Visible = true; 
            }
        }

        private void gvDisasterPreventionEquipment_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            //if (e.RowIndex < 0)
            //    return;


            //gvDisasterPreventionEquipment.Rows[e.RowIndex].Cells[0].Value = gvDisasterPreventionEquipment.Rows.Count;
        }

        private void gvDisasterPreventionEquipment_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex != 0)
                gvDisasterPreventionEquipment.BeginEdit(true);


            if (e.ColumnIndex == 1 || e.ColumnIndex == 3)
            {
                if (gvDisasterPreventionEquipment.EditingControl == null) return;


                DataGridViewCell cell = gvDisasterPreventionEquipment.Rows[e.RowIndex].Cells[e.ColumnIndex];

                ComboBox comboBox = (ComboBox)gvDisasterPreventionEquipment.EditingControl;
                comboBox.DropDownStyle = ComboBoxStyle.DropDown;

                comboBox.Leave += comboBox_Leave;
            }
        }

        private void comboBox_Leave(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;

            if (gvDisasterPreventionEquipment.SelectedCells.Count != 1)
                return;

            DataGridViewCell cell = gvDisasterPreventionEquipment.SelectedCells[0];

            if (cell.ColumnIndex == 1)
                EndEditDisasterPreventionEquipmentType(cell, cbo);
            else if (cell.ColumnIndex == 3)
                EndEditDisasterPreventionEquipmentLocation(cell, cbo);

            cbo.Leave -= comboBox_Leave;
        }

        private void EndEditDisasterPreventionEquipmentType(DataGridViewCell cell, ComboBox cbo)
        {
            if (cell.ColumnIndex != 1)
                return;

            if (cell.EditedFormattedValue == null || String.IsNullOrWhiteSpace(cell.EditedFormattedValue.ToString()) == true)
            {
                cell.Value = String.Empty;
                cell.Tag = null;
                return;
            }

            if (cell.Value != null && cell.Value.ToString() == cbo.Text)
                return;

            Dictionary<int, DisasterPreventionEquipmentType> dicDisasterPreventionEquipmentType = FormMain.Instance.DataManager.GetDisasterPreventionEquipmentType();
            foreach (DisasterPreventionEquipmentType equipType in from equipTypes in dicDisasterPreventionEquipmentType.Values.Cast<DisasterPreventionEquipmentType>()
                                                                  where equipTypes.Name == cbo.Text
                                                                  select equipTypes
                                                                 )
            {
                cell.Value = equipType.Name;
                cell.Tag = equipType;
                m_isChanged = true;
                return;
            }

            DisasterPreventionEquipmentType newType = FormMain.Instance.DataManager.AddDisasterPreventionEquipmentType(cbo.Text);
            cell.Value = newType.Name;
            cell.Tag = newType;
            m_isChanged = true;

            BindingGridComboBox();

        }

        private void EndEditDisasterPreventionEquipmentLocation(DataGridViewCell cell, ComboBox cbo)
        {
            if (cell.ColumnIndex != 3)
                return;

            if (cell.EditedFormattedValue == null || String.IsNullOrWhiteSpace(cell.EditedFormattedValue.ToString()) == true)
            {
                cell.Value = String.Empty;
                cell.Tag = null;
                return;
            }

            if (cell.Value != null && cell.Value.ToString() == cbo.Text)
                return;

            Dictionary<int, DisasterPreventionEquipmentLocation> dicDisasterPreventionEquipmentLocation = FormMain.Instance.DataManager.GetDisasterRreventionEquipmentLocation();
            foreach (DisasterPreventionEquipmentLocation equipLocation in from equipLocations in dicDisasterPreventionEquipmentLocation.Values.Cast<DisasterPreventionEquipmentLocation>()
                                                                          where equipLocations.Name == cbo.Text
                                                                          select equipLocations
                                                                          )
            {
                cell.Value = equipLocation.Name;
                cell.Tag = equipLocation;
                m_isChanged = true;
                return;
            }

            DisasterPreventionEquipmentLocation newLocation = FormMain.Instance.DataManager.AddDisasterPreventionEquipmentLocation(cbo.Text);
            cell.Value = newLocation.Name;
            cell.Tag = newLocation;
            m_isChanged = true;

            BindingGridComboBox();
        }

        private void gvDisasterPreventionEquipment_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (m_isChanged == false)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 0)
                return;

            m_isChanged = false;

            DataGridViewRow row = gvDisasterPreventionEquipment.Rows[e.RowIndex];

            if (row.IsNewRow == true)
                return;

            if (row.Tag != null)
            {
                if ((row.Tag as DisasterPreventionEquipment).Status != DisasterPreventionEquipment.STATUS.NEW)
                    (row.Tag as DisasterPreventionEquipment).Status = DisasterPreventionEquipment.STATUS.UPD;
            }
            else
            {
                row.Tag = FormMain.Instance.DataManager.AddDisasterPreventionEquipment();
                row.Cells[4].Value = 0;
            }

            DisasterPreventionEquipment disasterPreventionEquipment = row.Tag as DisasterPreventionEquipment;
            switch (e.ColumnIndex)
            {
                case 1:
                    if (row.Cells[e.ColumnIndex].Tag != null)
                        disasterPreventionEquipment.Type = row.Cells[e.ColumnIndex].Tag as DisasterPreventionEquipmentType;
                    else
                        disasterPreventionEquipment.Type = null;

                    break;
                case 2:
                    if (row.Cells[e.ColumnIndex].Value != null)
                        disasterPreventionEquipment.Name = row.Cells[e.ColumnIndex].Value.ToString();
                    else
                        disasterPreventionEquipment.Name = "";

                    break;
                case 3:
                    if (row.Cells[e.ColumnIndex].Tag != null)
                        disasterPreventionEquipment.Location = row.Cells[e.ColumnIndex].Tag as DisasterPreventionEquipmentLocation;
                    else
                        disasterPreventionEquipment.Location = null;

                    break;
                case 4:
                    if (row.Cells[e.ColumnIndex].Value != null)
                    {
                        int tmp = 0;
                        if (int.TryParse(row.Cells[e.ColumnIndex].Value.ToString().Replace(",", ""), out tmp) == false)
                            tmp = 0;

                        disasterPreventionEquipment.Quantity = tmp;
                    }
                    else
                        disasterPreventionEquipment.Quantity = 0;

                    break;
                case 5:
                    if (row.Cells[e.ColumnIndex].Value != null)
                        disasterPreventionEquipment.Description = row.Cells[e.ColumnIndex].Value.ToString();
                    else
                        disasterPreventionEquipment.Description = "";

                    break;
            }

            if (row.Cells[0].Value == null || row.Cells[0].Value.ToString() == "")
            {
                row.Cells[0].Value = e.RowIndex + 1;
            }

            ChangeDataStatus();

        }

        private void gvDisasterPreventionEquipment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("삭제하시겠습니까?", "경고", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    List<DataGridViewRow> liRow = new List<DataGridViewRow>();
                    foreach (DataGridViewCell cell in gvDisasterPreventionEquipment.SelectedCells)
                    {
                        if (liRow.Contains(cell.OwningRow) == false)
                            liRow.Add(cell.OwningRow);
                    }

                    foreach (DataGridViewRow row in liRow)
                    {
                        if (row.IsNewRow) continue;

                        if (row.Tag != null)
                            (row.Tag as DisasterPreventionEquipment).Status = DisasterPreventionEquipment.STATUS.DEL;

                        gvDisasterPreventionEquipment.Rows.Remove(row);
                    }

                    // 데이터 로우 해더번호 재정렬
                    int nIndex = 0;
                    foreach (DataGridViewRow row in gvDisasterPreventionEquipment.Rows)
                    {
                        if (row.IsNewRow == true)
                            continue;

                        row.Cells[0].Value = ++nIndex;
                    }

                    ChangeDataStatus();
                }
            }
        }

        private void gvDisasterPreventionEquipment_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (gvDisasterPreventionEquipment.Rows[e.RowIndex].IsNewRow == true)
                return;


            if (e.ColumnIndex == 4)
            {
                int temp = 0;

                if (e.FormattedValue == null)
                {
                    gvDisasterPreventionEquipment.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                    m_isChanged = true;
                    return;
                }
                else if (e.FormattedValue.ToString() == "")
                {
                    gvDisasterPreventionEquipment.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                    m_isChanged = true;
                    return;
                }
                else if (int.TryParse(e.FormattedValue.ToString().Replace(",", ""), out temp) == false)
                {
                    gvDisasterPreventionEquipment.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                    m_isChanged = true;
                    return;
                }

                if (gvDisasterPreventionEquipment.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                    m_isChanged = true;
                else
                {
                    int tmp = 0;
                    if (int.TryParse(gvDisasterPreventionEquipment.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Replace(",", ""), out tmp) == false)
                        tmp = 0;

                    if (tmp != temp)
                        m_isChanged = true;
                }

            }
            else
            {
                if (e.ColumnIndex == 2 || e.ColumnIndex == 5)
                {
                    if (e.FormattedValue != null && gvDisasterPreventionEquipment.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                    {
                        if (e.FormattedValue.ToString() != gvDisasterPreventionEquipment.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                        {
                            m_isChanged = true;
                        }
                    }
                    else if (e.FormattedValue != null && gvDisasterPreventionEquipment.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                    {
                        m_isChanged = true;
                    }


                }
            }

        }

        private void gvDisasterPreventionEquipment_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            gvDisasterPreventionEquipment.EndEdit();
        }

        private void ChangeDataStatus(bool isEnable = true)
        {
            ImageButton btn = FormMain.Instance.GetButton(ID.ID_SAVE_DATA);
            btn.Enabled = isEnable;
            FormMain.Instance.CheckButton(btn, btn.Enabled);
        }

        private void FormSensorList_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                FormMain.Instance.DataManager.ClearDisasterPreventionEquipment();
                PageBackstageHome.Instance.SomethingChanged(null);
            }
        }

        private void gvDisasterPreventionEquipment_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null)
                return;

            if (e.ColumnIndex == 4 && e.RowIndex != this.gvDisasterPreventionEquipment.NewRowIndex)
            {
                int tmp = 0;

                if (int.TryParse(e.Value.ToString().Replace(",", ""), out tmp) == false)
                {
                    tmp = 0;
                }

                e.Value = tmp.ToString("N0");
            }
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(this, "SDMS_Show_FacilityList");
            m_manualManager.SetID(btnSelectZone, "SDMS_Show_FacilityList");
            m_manualManager.SetID(gvDisasterPreventionEquipment, "SDMS_Show_FacilityList");
            m_manualManager.SetID(gvSensorList, "SDMS_Show_FacilityList_DisasterEquip");

            m_manualManager.ProcessEvent();
        } 
	}
}