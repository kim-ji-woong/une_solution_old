using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;


namespace SDMS
{
	public partial class FormFacilityZone : Form
	{
		public FormFacilityZone()
		{
            this.DoubleBuffered = true;

			InitializeComponent();

            FormMain.SetDoubleBuffer(this.gridSensorList, true);

			//checkBoxFA.Location = checkBoxFireSensor.Location;
			InitComboBox();
		}

		private void InitComboBox()
		{
			//radioSensor.Checked = true;
            string strEtc = "";

			foreach (KeyValuePair<int, BuildingGroup> pair in ZoneManager.Instance.DicBuildingGroup)
			{
                if (pair.Value.ToString() == "종합창고")
                {
                    strEtc = "실외 및 종합창고";
                    //cboBuildingGroup.Items.Add("실외 및 종합창고");
                }
                //else if (pair.Value.ToString() == "기타건물")
                //{
                //    cboBuildingGroup.Items.Add("실외 및 기타건물", );
                //}
                else
                    cboBuildingGroup.Items.Add(pair.Value);
			}

            cboBuildingGroup.Sorted = true;
            cboBuildingGroup.Sorted = false;

            if (strEtc.Length > 0)
                cboBuildingGroup.Items.Add(strEtc);

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

            List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);

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


        private void AddGridData(EquipmentZoneObjectList list)
        {
            gridSensorList.ClearSelection();
            gridSensorList.Rows.Clear();

            int nCount = 1;
            foreach(ISensor iSensor in list.SensorList)
            {
                if (iSensor.Type == IFacility.FacilityType.PSM_SENSOR)
                    continue;

                FireSensor sensor = (FireSensor)iSensor;
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = nCount.ToString();
                row.Cells.Add(cell);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = sensor.TypeString;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                cell4.Value = sensor.SensorName;
                row.Cells.Add(cell4);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = sensor.PositionName;
                row.Cells.Add(cell3);

                
                gridSensorList.Rows.Add(row);

                nCount++;
            }            
        }


        private EquipmentZoneObjectList mCurrentList = null;
		private void cboEquipZone_SelectedIndexChanged(object sender, EventArgs e)
		{
			EquipmentZone equipZone = (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];

            mCurrentList = SensorManager.Instance.FindZoneInSensor(equipZone.ID);

            if (mCurrentList != null)
            {
                AddGridData(mCurrentList);
            }
            else
            {
                gridSensorList.ClearSelection();
                gridSensorList.Rows.Clear();
            }
		}

		private void checkBoxSpringCooler_CheckedChanged(object sender, EventArgs e)
		{
			//SaveEditData2();
		}

        //public void SaveEditData()
        //{
        //    bool bValue = checkBoxFireSensor.Checked;
        //    if (cboEquipZone.SelectedIndex < 0)
        //        return;
        //    // Create Sensor
        //    if (m_bExistFireSensor == false && bValue == true)
        //    {
        //        EquipmentZone equipZone = (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];
        //        if (equipZone == null)
        //            return;
        //        UnE.Geometry.Vertex2D vert = equipZone.Polygon.CalcWeightCenter();
        //        Zone zone = equipZone.LinkedZone;
        //        UnE.Geometry.Vertex2D vert2 = zone.Polygon.CalcWeightCenter();
        //        float x = -(float)(vert2.x - vert.x);
        //        float z = -(float)(vert.y - vert2.y);
        //        Core.Position3D pos = new Core.Position3D(x, 0.5f, z);

        //        FireSensor sensor = new FireSensor();
        //        POI poi = new POI();
        //        poi.X = pos.X;
        //        poi.Y = pos.Y;
        //        poi.Z = pos.Z;
        //        poi.Facility = sensor;

        //        poi.IsIndoor = !zone.IsOutdoor;
        //        poi.Popup = null;
        //        poi.Zone = zone;
        //        sensor.EquipZoneID = equipZone.ID;

        //        EditFireSensor editFireSEnsor = new EditFireSensor(sensor);
        //        editFireSEnsor.AddToManager(FormMain.Instance.PageHome);

        //        m_FireSensor = sensor;
        //    }
        //    // Delete Sensor
        //    else if (m_bExistFireSensor == true && bValue == false)
        //    {
        //        if (m_FireSensor != null)
        //        {
        //            EditFireSensor editFireSEnsor = new EditFireSensor(m_FireSensor);
        //            editFireSEnsor.IsDeleting = true;
        //            editFireSEnsor.AddToManager(FormMain.Instance.PageHome);
        //        }
        //    }
        //}

        //private void SaveEditData2()
        //{
        //    bool bValue = checkBoxSpringCooler.Checked;
        //    if (m_bExistSpringCooler == false && bValue == true)
        //    {
        //        EquipmentZone equipZone = (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];
        //        if (equipZone == null)
        //            return;
        //        UnE.Geometry.Vertex2D vert = equipZone.Polygon.CalcWeightCenter();
        //        Zone zone = equipZone.LinkedZone;
        //        UnE.Geometry.Vertex2D vert2 = zone.Polygon.CalcWeightCenter();
        //        float x = -(float)(vert2.x - vert.x);
        //        float z = -(float)(vert.y - vert2.y);
        //        Core.Position3D pos = new Core.Position3D(x, 0.5f, z);

        //        SpringCooler sensor = new SpringCooler();
        //        POI poi = new POI();
        //        poi.X = pos.X;
        //        poi.Y = pos.Y;
        //        poi.Z = pos.Z;
        //        poi.Facility = sensor;

        //        poi.IsIndoor = !zone.IsOutdoor;
        //        poi.Popup = null;
        //        poi.Zone = zone;
        //        sensor.EquipZoneID = equipZone.ID;

        //        EditSpringCooler editSpringCooler = new EditSpringCooler(sensor);
        //        editSpringCooler.AddToManager(FormMain.Instance.PageHome);

        //        m_SpringCooler = sensor;
        //    }
        //    // Delete Sensor
        //    else if (m_bExistSpringCooler == true && bValue == false)
        //    {
        //        if (m_SpringCooler != null)
        //        {
        //            EditSpringCooler coolingSensor = new EditSpringCooler(m_SpringCooler);
        //            coolingSensor.IsDeleting = true;
        //            coolingSensor.AddToManager(FormMain.Instance.PageHome);
        //        }
        //    }
        //}

		private void checkBoxFireSensor_CheckedChanged(object sender, EventArgs e)
		{
			//SaveEditData();
		}

        private void btnEdit_Click(object sender, EventArgs e)
        {
            FormEditFacilityZone frm = new FormEditFacilityZone();
            frm.ShowInTaskbar = false;
            frm.EquipmentZoneObjectList = mCurrentList;
            if (PageBackstageHome.ShowTranslucentSubForm(frm) == System.Windows.Forms.DialogResult.OK)
            {
                ArrayList arResult = frm.GetSensorList();
                List<ISensor> arTarget = null;

                if (mCurrentList != null)
                {
                    arTarget = mCurrentList.SensorList;
                }
                else
                    arTarget = new List<ISensor>();

                SaveEditSenserZone(arTarget, arResult,  mCurrentList);

                if(mCurrentList == null)
                {
                    EquipmentZone equipZone = (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];
                    if (equipZone != null)
                        mCurrentList = SensorManager.Instance.FindZoneInSensor(equipZone.ID);
                }

                if (mCurrentList != null)
                {
                    AddGridData(mCurrentList);
                }
                else
                {
                    gridSensorList.ClearSelection();
                    gridSensorList.Rows.Clear();
                }
            }
        }

        private void SaveEditSenserZone(List<ISensor> arSrc, ArrayList arTrg, EquipmentZoneObjectList currentList)
        {

            EquipmentZone equipZone = (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];
            if(equipZone == null)
                return;

            ArrayList arDiff = new ArrayList();
            foreach(ISensor szSrc in arSrc )
            {
                bool bFind = false;
                foreach (ISensor szTrg in arTrg)
                {
                    if( szTrg.ID == szSrc.ID)
                    {
                        bFind = true;
                        break;
                    }
                }

                // 현재 리스트에서 제외된 아이템 추가
                if( bFind == false)
                {                    
                    arDiff.Add(szSrc);                   
                }
            }
            foreach (ISensor sz in arDiff)
            {
                if (currentList != null)
                    currentList.SensorList.Remove(sz);
            }


            ArrayList arAdded = new ArrayList();
            foreach (ISensor szTrg in arTrg)
            {
                bool bFind = false;
                foreach (ISensor szSrc in arSrc)
                {
                    if (szTrg.ID == szSrc.ID)
                    {
                        bFind = true;
                        break;
                    }
                }

                // 현재 리스트에 새로 추가된 아이템 추가
                if (bFind == false)
                {
                    szTrg.EquipZoneID = equipZone.ID;
                    //arDiff.Add(szTrg);
                    arAdded.Add(szTrg);
                }
            }

            foreach (ISensor sz in arAdded)
            {
                if (currentList != null)
                    currentList.SensorList.Add(sz);
                else
                    SensorManager.Instance.AddSensor((FireSensor)sz);

                EditFacilityZone szEdit = new EditFacilityZone(sz, EditFacilityZone.EditFacilityZoneType.SET);
                szEdit.AddToManager(FormMain.Instance.PageHome);

                SensorManager.Instance.EndEditSensor(sz);
            }

            foreach (ISensor sz in arDiff)
            {
                EditFacilityZone szEdit = new EditFacilityZone(sz, EditFacilityZone.EditFacilityZoneType.RESET);
                szEdit.EquipZoneID = 0;
                szEdit.AddToManager(FormMain.Instance.PageHome);

                SensorManager.Instance.BeginEditSensor(sz);
            }
        }
	}
}