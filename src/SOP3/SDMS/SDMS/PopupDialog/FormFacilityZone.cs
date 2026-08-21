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
    public partial class FormFacilityZone : Form
    {
        public FormFacilityZone()
        {
            InitializeComponent();

            //checkBoxFA.Location = checkBoxFireSensor.Location;
            InitComboBox();
        }

        private void InitComboBox()
        {
            //radioSensor.Checked = true;

            foreach (KeyValuePair<int, BuildingGroup> pair in ZoneManager.Instance.DicBuildingGroup)
            {
				if( pair.Value.ToString() == "종합창고")
					cboBuildingGroup.Items.Add("실외 및 종합창고");
				else
					cboBuildingGroup.Items.Add(pair.Value);
            }

            if (cboBuildingGroup.Items.Count > 0)
                cboBuildingGroup.SelectedIndex = 0;
        }

		private void cboBuildingGroup_SelectionChangeCommitted(object sender, EventArgs e)
		{
			
		}

        private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
			int nSelectedIndex = cboBuildingGroup.SelectedIndex;
			if (nSelectedIndex < 0)
				return;

			cboBuilding.Items.Clear();

			BuildingGroup buildingGroup = (BuildingGroup)cboBuildingGroup.Items[nSelectedIndex];

			if (buildingGroup.GroupID > 0)
			{
				ArrayList arrBuildings = buildingGroup.BuildingList;

				foreach (Building building in arrBuildings)
				{
					ArrayList arrZones = ZoneManager.Instance.GetZoneList(building.ID);

					if (arrZones != null && arrZones.Count > 0)
					{
						//Zone이 하나도 없는 빌딩은 콤보박스에 보여주지 않는다.
						cboBuilding.Items.Add(building);
					}
				}
			}

			if (cboBuilding.Items.Count > 0)
				cboBuilding.SelectedIndex = 0;
        }

		private void cboBuilding_SelectionChangeCommitted(object sender, EventArgs e)
		{
			
		}

        private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
			int nSelectedIndex = cboBuilding.SelectedIndex;
			if (nSelectedIndex < 0)
				return;

			cboFloor.Items.Clear();

			Object obj = cboBuilding.Items[nSelectedIndex];
			Type type = obj.GetType();

			if (type == typeof(Building))
			{
				Building building = (Building)obj;

				ArrayList arrZones = ZoneManager.Instance.GetZoneList(building.ID);
				ArrayList arrFloor = new ArrayList();

				foreach (Zone zone in arrZones)
				{
					arrFloor.Add(new Floor(zone.FloorIndex + zone.AddFloor));
				}

				arrFloor.Sort();

				foreach (Floor floor in arrFloor)
				{
					cboFloor.Items.Add(floor);
				}
			}
			else
			{
				cboFloor.Items.Add("-");
			}

			if (cboFloor.Items.Count > 0)
				cboFloor.SelectedIndex = 0;
        }


		private void cboFloor_SelectionChangeCommitted(object sender, EventArgs e)
		{
			
		}
        private void cboFloor_SelectedIndexChanged(object sender, EventArgs e)
        {   
         	int nSelectedIndex = cboFloor.SelectedIndex;
			if (nSelectedIndex < 0)
				return;

			cboEquipZone.Items.Clear();

			Object obj = cboFloor.Items[nSelectedIndex];
			Type type = obj.GetType();

			Zone zone = null;

			if (type == typeof(Floor))
			{
				Building building = (Building)cboBuilding.Items[cboBuilding.SelectedIndex];
				Floor floor = (Floor)obj;
				zone = ZoneManager.Instance.FindZone(building, floor.ToString());
			}

			if (zone == null || zone.ID <= 0)
				return;

			ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);

			if (arrEquipZones == null)
				return;
			//EquipmentZone.EquipZoneType zoneType = radioSensor.Checked ? EquipmentZone.EquipZoneType.SENSOR_TYPE : EquipmentZone.EquipZoneType.FA_TYPE;

			foreach (EquipmentZone equipZone in arrEquipZones)
			{
				//if (equipZone.ZoneType == zoneType)
				cboEquipZone.Items.Add(equipZone);
			}

			if (cboEquipZone.Items.Count > 0)
				cboEquipZone.SelectedIndex = 0;
        }

		private void cboEquipZone_SelectionChangeCommitted(object sender, EventArgs e)
		{
			
		}

		private bool m_bExistFireSensor = false;
		private bool m_bExistSpringCooler = false;
		private bool m_bExistDetector = false;

		private SpringCooler m_SpringCooler = null;
		private FireSensor m_FireSensor = null;
		private FireAlarm m_FireAlarm = null;

        private void cboEquipZone_SelectedIndexChanged(object sender, EventArgs e)
        {
			EquipmentZone equipZone = (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];

			EquipmentZoneObjectList list = SensorManager.Instance.FindZoneInSensor(equipZone.ID);

			if (list == null)
			{
				m_SpringCooler = null;
				m_FireSensor = null;
				m_FireAlarm = null;
				m_bExistFireSensor = false;
				m_bExistSpringCooler = false;
				m_bExistDetector = false;

				checkBoxFireSensor.Checked = false;
				checkBoxSpringCooler.Checked = false; 
				return;
			}

			bool findFireSensor = false, findSpringCooler = false, findFA = false;

			foreach (SensorZone sensor in list.SensorList)
			{
				if (sensor.Type == Facility.FacilityType.FIRE_SENSOR)
				{
					findFireSensor = true;
					m_FireSensor = (FireSensor)sensor;
				}
				else if (sensor.Type == Facility.FacilityType.COOLER_SENSOR)
				{
					findSpringCooler = true;
					m_SpringCooler = (SpringCooler)sensor;
				}
				else if (sensor.Type == Facility.FacilityType.FA)
				{
					findFA = true;
					m_FireAlarm = (FireAlarm)sensor;
				}
			}

			m_bExistFireSensor = findFireSensor;
			m_bExistSpringCooler = findSpringCooler;
			m_bExistDetector = findFA;
			
			checkBoxFireSensor.Checked = findFireSensor;
			checkBoxSpringCooler.Checked = findSpringCooler;
			checkBoxFA.Checked = findFA;

			/*if (equipZone.ZoneType == EquipmentZone.EquipZoneType.SENSOR_TYPE)
			{
				bool findFireSensor = false, findSpringCooler = false;

				foreach (SensorZone sensor in list.SensorList)
				{
					if (sensor.Type == Facility.FacilityType.FIRE_SENSOR)
						findFireSensor = true;
					else if (sensor.Type == Facility.FacilityType.COOLER_SENSOR)
						findSpringCooler = true;
				}

				checkBoxFireSensor.Checked = findFireSensor;
				checkBoxSpringCooler.Checked = findSpringCooler;
			}
			else if (equipZone.ZoneType == EquipmentZone.EquipZoneType.FA_TYPE)
			{
				bool findFA = false;

				foreach (SensorZone sensor in list.SensorList)
				{
					if (sensor.Type == Facility.FacilityType.FA)
						findFA = true;
				}

				checkBoxFA.Checked = findFA;
			}*/
        }

		private void checkBoxSpringCooler_CheckedChanged(object sender, EventArgs e)
		{
			SaveEditData2();

		}		

		public void SaveEditData()
		{
			bool bValue = checkBoxFireSensor.Checked;
            if (cboEquipZone.SelectedIndex < 0)
                return;
			// Create Sensor
			if (m_bExistFireSensor == false && bValue == true)
			{
				EquipmentZone equipZone = (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];
				if (equipZone == null)
					return;
				UnE.Geometry.Vertex2D vert = equipZone.Polygon.CalcWeightCenter();
				Zone zone = equipZone.LinkedZone;
				UnE.Geometry.Vertex2D vert2 = zone.Polygon.CalcWeightCenter();
				float x = -(float)(vert2.x - vert.x);
				float z = -(float)(vert.y - vert2.y);
				Core.Position3D pos = new Core.Position3D(x, 0.5f, z);

				FireSensor sensor = new FireSensor();
				POI poi = new POI();
				poi.X = pos.X;
				poi.Y = pos.Y;
				poi.Z = pos.Z;
				poi.Facility = sensor;
				
				poi.IsIndoor = !zone.IsOutdoor;
				poi.Popup = null;
				poi.Zone = zone;
				sensor.EquipZoneID = equipZone.ID;

				EditFireSensor editFireSEnsor = new EditFireSensor(sensor);
				editFireSEnsor.AddToManager(FormMain.Instance.PageHome);

				m_FireSensor = sensor;
			}
			// Delete Sensor
			else if (m_bExistFireSensor == true && bValue == false)
			{
				if (m_FireSensor != null)
				{
					EditFireSensor editFireSEnsor = new EditFireSensor(m_FireSensor);
					editFireSEnsor.IsDeleting = true;
					editFireSEnsor.AddToManager(FormMain.Instance.PageHome);
				}
			}
			
		}
		private void SaveEditData2()
		{
			bool bValue = checkBoxSpringCooler.Checked;
			if (m_bExistSpringCooler == false && bValue == true)
			{
				EquipmentZone equipZone = (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];
				if (equipZone == null)
					return;
				UnE.Geometry.Vertex2D vert = equipZone.Polygon.CalcWeightCenter();
				Zone zone = equipZone.LinkedZone;
				UnE.Geometry.Vertex2D vert2 = zone.Polygon.CalcWeightCenter();
				float x = -(float)(vert2.x - vert.x);
				float z = -(float)(vert.y - vert2.y);
				Core.Position3D pos = new Core.Position3D(x, 0.5f, z);

				SpringCooler sensor = new SpringCooler();
				POI poi = new POI();
				poi.X = pos.X;
				poi.Y = pos.Y;
				poi.Z = pos.Z;
				poi.Facility = sensor;

				poi.IsIndoor = !zone.IsOutdoor;
				poi.Popup = null;
				poi.Zone = zone;
				sensor.EquipZoneID = equipZone.ID;

				EditSpringCooler editSpringCooler = new EditSpringCooler(sensor);
				editSpringCooler.AddToManager(FormMain.Instance.PageHome);

				m_SpringCooler = sensor;
			}
			// Delete Sensor
			else if (m_bExistSpringCooler == true && bValue == false)
			{
				if (m_SpringCooler != null)
				{
					EditSpringCooler coolingSensor = new EditSpringCooler(m_SpringCooler);
					coolingSensor.IsDeleting = true;
					coolingSensor.AddToManager(FormMain.Instance.PageHome);
				}
			}	
		}
		private void checkBoxFireSensor_CheckedChanged(object sender, EventArgs e)
		{
			SaveEditData();
		}

        /*private void radioZoneType_CheckedChanged(object sender, EventArgs e)
        {
            if (radioSensor.Checked)
            {
                checkBoxFireSensor.Visible = true;
                checkBoxSpringCooler.Visible = true;
                checkBoxFA.Visible = false;
            }
            else
            {
                checkBoxFireSensor.Visible = false;
                checkBoxSpringCooler.Visible = false;
                checkBoxFA.Visible = true;
            }

            if (cboBuildingGroup.Items.Count == 0)
                return;

            cboFloor_SelectedIndexChanged(null, null);
        }*/
    }
}
