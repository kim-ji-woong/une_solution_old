using DBUtility2;
using SDMS;
using SDMS_Building.Data;
using SDMS_Building.PopupDialog.Controls;
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
using UnE.PSM;
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS_Building.PopupDialog.Config
{
    public partial class FormSensorList : Form
    {
		private UEWpfControl.WpfComboBox m_cbSensorType = null;
		private UEWpfControl.WpfComboBox m_cbBuilding = null;
		private UEWpfControl.WpfComboBox m_cbFloor = null;

		List<GridSensor> listGridSensor = new List<GridSensor>();
        
        public FormSensorList()
        {
            InitializeComponent();
            
            m_cbSensorType = new UEWpfControl.WpfComboBox();
			eleSensorType.Child = m_cbSensorType;
			m_cbSensorType.SetSize(eleSensorType.Width, eleSensorType.Height);

			m_cbBuilding = new UEWpfControl.WpfComboBox();
			eleBuilding.Child = m_cbBuilding;
			m_cbBuilding.customComboBox.SelectionChanged += BuildinglComboBox_SelectionChanged;
			m_cbBuilding.SetSize(eleBuilding.Width, eleBuilding.Height);

			m_cbFloor = new UEWpfControl.WpfComboBox();
			eleFloor.Child = m_cbFloor;
			m_cbFloor.SetSize(eleFloor.Width, eleFloor.Height);

			InitSensorTypeComboBox();
			InitBuildingComboBox();
		}

        private void FormSensorList_Load(object sender, EventArgs e)
        {
            gridSensorList.AutoGenerateColumns = false;

            DataGridViewTextBoxColumn noCol = new DataGridViewTextBoxColumn();
            noCol.DataPropertyName = "colNo";
            noCol.HeaderText = "No";
            noCol.Width = 50;

            DataGridViewTextBoxColumn typeCol = new DataGridViewTextBoxColumn();
            typeCol.DataPropertyName = "colType";
            typeCol.HeaderText = "유형";
            typeCol.Width = 150;

            DataGridViewTextBoxColumn nameCol = new DataGridViewTextBoxColumn();
            nameCol.DataPropertyName = "colName";
            nameCol.HeaderText = "이름";
            nameCol.Width = 200;

            DataGridViewTextBoxColumn buildingCol = new DataGridViewTextBoxColumn();
            buildingCol.DataPropertyName = "colBuilding";
            buildingCol.HeaderText = "건물";
            buildingCol.Width = 150;

            DataGridViewTextBoxColumn floorCol = new DataGridViewTextBoxColumn();
            floorCol.DataPropertyName = "colFloor";
            floorCol.HeaderText = "층";
            floorCol.Width = 70;

            DataGridViewTextBoxColumn locationCol = new DataGridViewTextBoxColumn();
            locationCol.DataPropertyName = "colLocation";
            locationCol.HeaderText = "설치장소";
            locationCol.Width = 300;

            gridSensorList.Columns.Add(noCol);
            gridSensorList.Columns.Add(typeCol);
            gridSensorList.Columns.Add(nameCol);
            gridSensorList.Columns.Add(buildingCol);
            gridSensorList.Columns.Add(floorCol);
            gridSensorList.Columns.Add(locationCol);

            btnSearch_Click(null, null);
        }

        private void InitSensorTypeComboBox()
		{
            m_cbSensorType.customComboBox.DisplayMemberPath = "DisplayName";
            m_cbSensorType.customComboBox.SelectedValuePath = "FacilityType";

            m_cbSensorType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.NONE, "모두"));
            m_cbSensorType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.FIRE_SENSOR, Data.CommonString.POI_Fire_Kor));
            m_cbSensorType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.CCTV, Data.CommonString.POI_CCTV_Kor));
            if (UnE.SOP.ProxySOP.Instance.UsePSM)
                m_cbSensorType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.PSM_SENSOR, Data.CommonString.POI_Gas_Kor));
            
            if (UnE.SOP.ProxySOP.Instance.UseDoor)
                m_cbSensorType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.DOOR, Data.CommonString.POI_Door_Kor));
            
            if (UnE.SOP.ProxySOP.Instance.UseFirewall)
                m_cbSensorType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.FIREWALL, Data.CommonString.POI_FireWall_Kor));
            
            if (UnE.SOP.ProxySOP.Instance.UseStrongWind)
                m_cbSensorType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.STRONG_WIND, Data.CommonString.POI_StrongWind_Kor));
            
            if (UnE.SOP.ProxySOP.Instance.UseBlackout)
                m_cbSensorType.customComboBox.Items.Add(new FacilityTypeComboBoxItem(IFacility.FacilityType.BLACKOUT, Data.CommonString.POI_Blackout_Kor));

            m_cbSensorType.customComboBox.SelectedIndex = 0;
		}

		private void InitBuildingComboBox()
		{
			m_cbBuilding.customComboBox.Items.Add("모두");

			foreach (KeyValuePair<int, Building> pair in ZoneManager.Instance.DicBuildings)
			{
				m_cbBuilding.customComboBox.Items.Add(pair.Value);
			}

			if (m_cbBuilding.customComboBox.Items.Count > 0)
				m_cbBuilding.customComboBox.SelectedIndex = 0;
		}

		private void BuildinglComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			int nSelectedIndex = m_cbBuilding.customComboBox.SelectedIndex;
			if (nSelectedIndex < 0)
				return;

			m_cbFloor.customComboBox.Items.Clear();
			m_cbFloor.customComboBox.Items.Add("모두");

			if (nSelectedIndex > 0)
			{
				Object obj = m_cbBuilding.customComboBox.Items[nSelectedIndex];
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
						m_cbFloor.customComboBox.Items.Add(floor);
					}
				}
				else
				{
					m_cbFloor.customComboBox.Items.Clear();
					m_cbFloor.customComboBox.Items.Add("-");
				}

			}

			if (m_cbFloor.customComboBox.Items.Count > 0)
				m_cbFloor.customComboBox.SelectedIndex = 0;
		}

		private void btnSearch_Click(object sender, EventArgs e)
		{
            gridSensorList.DataSource = null;
			listGridSensor.Clear();

            FacilityTypeComboBoxItem selectedItem = m_cbSensorType.customComboBox.SelectedItem as FacilityTypeComboBoxItem;
            if (selectedItem == null)
                return;

            LoadSensors(selectedItem.FacilityType);

            if (m_cbSensorType.customComboBox.SelectedIndex <= 0)
				lblFacilityName.Text = "모든 설비 목록";
			else
				lblFacilityName.Text = String.Format("{0} 목록", selectedItem.DisplayName);

			if (listGridSensor.Count == 0)
				return;

            //IEnumerable<GridSensor> dd = listGridSensor.OrderBy(p => p.facilityTypeID);

            int nNo = 1;

            System.Diagnostics.Trace.WriteLine("Begin : " + DateTime.Now.ToString("HH:mm:ss"));

            //gridSensorList.DataSource = listGridSensor;
            gridSensorList.DataSource = listGridSensor;

   //         foreach (GridSensor row in listGridSensor)
			//{
			//	//// 조건 검사하기
			//	//if (m_cbBuilding.customComboBox.SelectedIndex != 0)
			//	//{
			//	//	if (m_cbBuilding.customComboBox.SelectedItem != row.building)
			//	//		continue;
			//	//}
			//	//if (m_cbFloor.customComboBox.SelectedIndex != 0)
			//	//{
			//	//	Floor chkFloor = (Floor)m_cbFloor.customComboBox.SelectedItem;

			//	//	if (chkFloor.FloorIndex != row.floor.FloorIndex)
			//	//		continue;
			//	//}

			//	// 목록에 표시하기
			//	int rowIndex = gridSensorList.Rows.Add();

			//	gridSensorList.Rows[rowIndex].Tag = row;
			//	gridSensorList.Rows[rowIndex].Cells[colNo.Index].Value = nNo;
   //             gridSensorList.Rows[rowIndex].Cells[colType.Index].Value = row.type;
   //             gridSensorList.Rows[rowIndex].Cells[colName.Index].Value = row.name;				
			//	gridSensorList.Rows[rowIndex].Cells[colBuilding.Index].Value = (row.building != null) ? row.building.BuildingName : "";
			//	gridSensorList.Rows[rowIndex].Cells[colFloor.Index].Value = row.floor.ToString();
   //             gridSensorList.Rows[rowIndex].Cells[colLocation.Index].Value = row.strLocation;

   //             nNo++;
			//}

            System.Diagnostics.Trace.WriteLine("End : " + DateTime.Now.ToString("HH:mm:ss"));
        }

		private void LoadSensors(IFacility.FacilityType facilityType)
		{
            Building chkBuilding = null;
            // 조건 검사하기
            if (m_cbBuilding.customComboBox.SelectedIndex != 0)
                chkBuilding = (Building)m_cbBuilding.customComboBox.SelectedItem;

            Floor chkFloor = null;

            if (m_cbFloor.customComboBox.SelectedIndex != 0)
                chkFloor = (Floor)m_cbFloor.customComboBox.SelectedItem;

            System.Diagnostics.Trace.WriteLine("LoadSensors Begin : " + DateTime.Now.ToString("HH:mm:ss"));
            if (facilityType != IFacility.FacilityType.PSM_SENSOR && facilityType != IFacility.FacilityType.CCTV)
            {
                SortedList<int, ISensor> dicSensors = null;

                dicSensors = SensorManager.Instance.DicAllSenor;
                if (dicSensors != null)
                {
                    foreach (KeyValuePair<int, ISensor> sensors in SensorManager.Instance.DicAllSenor)
                    {
                        ISensor sensor = sensors.Value;
                        if (sensor.Type == facilityType || facilityType == IFacility.FacilityType.NONE)
                        {
                            if (sensor.Type != IFacility.FacilityType.Earthquake)
                            {
                                EquipmentZone equipmentZone = ZoneManager.Instance.GetEquipZone(sensor.EquipZoneID);
                                if (chkBuilding != null && chkBuilding != equipmentZone.Building)
                                    continue;
                                if (chkFloor != null && chkFloor.FloorIndex != equipmentZone.FloorIndex)
                                    continue;

                                string strType = IFacility.GetFacilityTypeString(sensor.Type);
                                if (sensor.Type == IFacility.FacilityType.PSM_SENSOR && UnE.SOP.ProxySOP.Instance.SiteID == 201)
                                    strType = Data.CommonString.POI_Gas_Kor;
                                listGridSensor.Add(new GridSensor(listGridSensor.Count+1, sensor.SensorName, strType, (equipmentZone.Building == null) ? sensor.SensorName : equipmentZone.Building.BuildingName, equipmentZone.Floor.ToString(), equipmentZone.ZoneName));
                            }
                        }
                    }
                }
            }
            System.Diagnostics.Trace.WriteLine("LoadSensors End : " + DateTime.Now.ToString("HH:mm:ss"));

            //if (facilityType == IFacility.FacilityType.PSM_SENSOR || facilityType == IFacility.FacilityType.NONE)
            //    LoadPSMSensors();

            if (facilityType == IFacility.FacilityType.CCTV || facilityType == IFacility.FacilityType.NONE)
                LoadCCTV();
        }

		private void LoadPSMSensors()
		{
			List<PSMSensor> sensors = PSMManager.Instance.GetSensors();
            int idx = 1;
			foreach (PSMSensor psm in sensors)
			{
				EquipmentZone equipmentZone = ZoneManager.Instance.GetEquipZone(psm.EquipZoneID);
                string strType = psm.Type.ToString();
                if (UnE.SOP.ProxySOP.Instance.SiteID == 201)
                    strType = Data.CommonString.POI_Gas_Kor;
                listGridSensor.Add(new GridSensor(listGridSensor.Count + 1, psm.Name, strType, equipmentZone.Building.BuildingName, equipmentZone.Floor.ToString(), equipmentZone.ZoneName));
                idx++;
			}
		}
        
		private void LoadCCTV()
		{
            Building chkBuilding = null;
            // 조건 검사하기
            if (m_cbBuilding.customComboBox.SelectedIndex != 0)
                chkBuilding = (Building)m_cbBuilding.customComboBox.SelectedItem;

            Floor chkFloor = null;
            if (m_cbFloor.customComboBox.SelectedIndex != 0)
                chkFloor = (Floor)m_cbFloor.customComboBox.SelectedItem;

            WebDBManager dbMgr = FormMain.Instance.DBManager;
			
            StringBuilder sb = new StringBuilder();
            sb.Append("Select IPAddr, ZoneID, CameraName ");
            sb.Append("  From CCTV Inner join Zone as z ON z.ID=ZoneID ");
            sb.AppendFormat("Where z.SiteID = {0} ", UnE.SOP.ProxySOP.Instance.SiteID);

            if (chkBuilding != null)
                sb.AppendFormat("  And z.BuildingID = {0}", chkBuilding.ID);

            if (chkFloor != null)
                sb.AppendFormat("  And ZoneID = {0}", chkFloor.Zone.ID);
            
			ArrayList arrResult = dbMgr.GetResultData(sb.ToString());

			if (arrResult == null)
				return;

			int nResultCount = arrResult.Count;
            
            for (int i = 0; i < nResultCount - 2; i += 3)
			{
				string strIP = WebDBManager.GetStringField(arrResult[i], "");
				int nZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
				string szName = WebDBManager.GetStringField(arrResult[i + 2], "");

				Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                
                listGridSensor.Add(new GridSensor(listGridSensor.Count + 1, szName, "CCTV", zone.Building.BuildingName, zone.Floor.ToString(), zone.ZoneName));
			}
		}

		private class GridSensor
		{
            public int colNo { get; set; }
            public string colName { get; set; }
			public string colType { get; set; }
            public string colBuilding { get; set; }
            public string colFloor { get; set; }
            public string colLocation { get; set; }

            public GridSensor(int no, string name, string type, string buildingName, string floor, string strLocation)
			{
                this.colNo = no;
				this.colName = name;
				this.colType = type;
				this.colBuilding = buildingName;
				this.colFloor = floor;
                this.colLocation = strLocation;            
			}
		}
	}

	public class FacilityTypeComboBoxItem
    {
        private IFacility.FacilityType m_facilityType = IFacility.FacilityType.NONE;
        public IFacility.FacilityType FacilityType
        {
            get { return m_facilityType; }
            set { m_facilityType = value; }
        }

        private string m_strDisplayName = "";
        public string DisplayName
        {
            get { return m_strDisplayName; }
            set { m_strDisplayName = value; }
        }

        public FacilityTypeComboBoxItem(IFacility.FacilityType facilityType, string strDisplayName)
        {
            this.m_facilityType = facilityType;
            this.m_strDisplayName = strDisplayName;            
        }
    }
}
