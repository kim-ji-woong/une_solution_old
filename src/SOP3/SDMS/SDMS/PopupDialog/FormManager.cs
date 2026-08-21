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
    public partial class FormManager : Form
    {
        public FormManager()
        {
            InitializeComponent();

            labelDescription.Text = "";
        }

        private void FormManager_Load(object sender, EventArgs e)
        {
            InitComboBox();
            InitGrid();
        }

        private Facility.FacilityType GetCurrentType()
        {
            Facility.FacilityType type = Facility.FacilityType.NONE;

            if (cboSensorType.SelectedIndex == 0)
                type = Facility.FacilityType.FIRE_SENSOR;
            else if (cboSensorType.SelectedIndex == 1)
                type = Facility.FacilityType.CCTV;
            else if (cboSensorType.SelectedIndex == 2)
                type = Facility.FacilityType.FE;

            return type;
        }


        private void InitGrid()
        {
            colNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colName.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            colTeam.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colTeam.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
        }

        private void InitComboBox()
        {
			
            cboSensorType.SelectedIndex = 0;

			cmbType.SelectedIndex = 0;

            foreach (KeyValuePair<int, BuildingGroup> pair in ZoneManager.Instance.DicBuildingGroup)
            {
                cboBuildingGroup.Items.Add(pair.Value);
            }

            cboBuildingGroup.Items.Add(ZoneManager.Instance.OutdoorBuildingGroup);
            cboBuildingGroup.SelectedIndex = 0;
        }

        private void cboSensorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Facility.FacilityType type = Facility.FacilityType.NONE;

            if (cboSensorType.SelectedIndex == 0)
            {
                labelDescription.Text = "(화재센서 / 스프링쿨러 / 펌프압력센서)";
                type = Facility.FacilityType.FIRE_SENSOR;
            }
            else if (cboSensorType.SelectedIndex == 1)
            {
                labelDescription.Text = "";
                type = Facility.FacilityType.CCTV;
            }
            else if (cboSensorType.SelectedIndex == 2)
            {
                labelDescription.Text = "(소화기 / 소화전 / 발신기)";
                type = Facility.FacilityType.FE;
            }

            if (checkBoxBuilding.Checked)
            {

                object item = cboBuilding.Items[cboBuilding.SelectedIndex];

                if (item.GetType() == typeof(Building))
                    LoadBuildingManager(type, (Building)item);
                else
                    LoadOutdoorManager(type, (Zone)item);

            }
            else
            {
                LoadEntireManager(type);
            }
        }

        private void LoadBuildingManager(Facility.FacilityType type, Building building)
        {
            gridManager.Rows.Clear();

            FacilityManagerGroup group = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(type, building);
            AddGridData(group);
        }

        private void LoadOutdoorManager(Facility.FacilityType type, Zone zone)
        {
            gridManager.Rows.Clear();

            FacilityManagerGroup group = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(type, zone);
            AddGridData(group);
        }

        private void LoadEntireManager(Facility.FacilityType type)
        {
            gridManager.Rows.Clear();

            FacilityManagerGroup group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(type);
            AddGridData(group);
        }

        private string GetLimitText(int nUpperLimit)
        {
            if (nUpperLimit > 0)
                return "급 및 그 상위 직급";
            else if (nUpperLimit < 0)
                return "급 및 그 하위 직급";

            return "급";
        }

        private void AddGridData(FacilityManagerGroup group)
        {
            if (group == null)
                return;

            int nIndex = 1;
            string strDescription = "";

            foreach (FacilityManager mgr in group.RegularTeams)
            {
                if (mgr.LevelLimit == 0)
                    continue;

                DataTeam team = (DataTeam)mgr.Tag;
                if (team == null)
                    continue;

				//string szSub = mgr.UpperLimit == true ? "급 및 그 상위 직급" : "급 및 그 하위 직급";
                string szSub = GetLimitText(mgr.UpperLimit);

                // 당직자
                if (mgr.MemberType == 6)
                    strDescription = "";
                else
                {
                    strDescription = mgr.LevelLimit < 0 ? "팀원 모두" : (mgr.LevelLimit.ToString() + szSub);
                }

                AddGridRowData(ref nIndex, team.TeamName, strDescription, mgr);
            }

            foreach (FacilityManager mgr in group.CompanyMembers)
            {
                DataCompanyMember member = (DataCompanyMember)mgr.Tag;

                if (member == null || member.Team == null)
                    continue;

                AddGridRowData(ref nIndex, member.MemberName, member.Team.TeamName, mgr);
            }

            foreach (FacilityManager mgr in group.ExternalTeams)
            {
                DataTeam team = (DataTeam)mgr.Tag;

                if (team == null)
                    continue;

                AddGridRowData(ref nIndex, team.TeamName, team.CompanyName, mgr);
            }

            foreach (FacilityManager mgr in group.ExternalCompanyMembers)
            {
                DataExternalMember member = (DataExternalMember)mgr.Tag;

                if (member == null || member.Team == null)
                    continue;

                AddGridRowData(ref nIndex, member.Name, member.Team.CompanyName + " " + member.Team.TeamName, mgr);
            }
        }

        private void AddGridRowData(ref int nIndex, string strName, string strDescription, FacilityManager mgr)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.Tag = mgr;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nIndex.ToString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strName;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strDescription;
            row.Cells.Add(cell);

            gridManager.Rows.Add(row);
            nIndex++;
        }

        private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuildingGroup.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            BuildingGroup buildingGroup = (BuildingGroup)cboBuildingGroup.Items[nSelectedIndex];

            cboBuilding.Items.Clear();
            cmbFloor.Enabled = true;

            if (buildingGroup.GroupID > 0)
            {
                cboBuilding.Enabled = true;
                btnEdit.Enabled = true;

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
                cboBuilding.Enabled = false;

                if (cmbType.SelectedIndex == 1) // 건물별 보기
                    btnEdit.Enabled = false;
                else // 설비영역별 보기
                {
                    btnEdit.Enabled = true;
                    cmbFloor.Enabled = false;

                    cmbFloor.Items.Clear();
                    cmbEquipZone.Items.Clear();

                    foreach (KeyValuePair<int, EquipmentZone> pair in ZoneManager.Instance.DicOutdoorEquipZones)
                    {
                        cmbEquipZone.Items.Add(pair.Value);
                    }

                    if (cmbEquipZone.Items.Count > 0)
                        cmbEquipZone.SelectedIndex = 0;
                }
            }

            if (cboBuilding.Items.Count > 0)
                cboBuilding.SelectedIndex = 0;
        }

		private int m_nManagerMode = 0;

		private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nSelectIdx = cmbType.SelectedIndex;
			if (nSelectIdx < 0)
				return;

			m_nManagerMode = nSelectIdx;

			Building buildingCurrent = null;
			Zone zoneCurrent = null;
			EquipmentZone equipZone = null;
			FacilityManagerGroup group = GetCurrentFacilityGroup(ref buildingCurrent, ref zoneCurrent, ref equipZone);

            cboBuilding.Enabled = true;
            cmbFloor.Enabled = true;
            btnEdit.Enabled = true;

			if (nSelectIdx == 0)
			{
				cboBuildingGroup.Visible = false;
				cboBuilding.Visible = false;
				cmbEquipZone.Visible = false;
				cmbFloor.Visible = false;

				gridManager.Location = new Point(13, 76);
				gridManager.Size = new Size(520, 313);

				gridManager.Rows.Clear();

				if (group != null)
					AddGridData(group);
			}
			else if( nSelectIdx == 1)
			{
				cboBuildingGroup.Visible = true;
				cboBuilding.Visible = true;
				cmbEquipZone.Visible = false;
				cmbFloor.Visible = false;

                if (cboBuildingGroup.SelectedIndex >= 0 &&
                    (BuildingGroup)cboBuildingGroup.Items[cboBuildingGroup.SelectedIndex] == ZoneManager.Instance.OutdoorBuildingGroup)
                {
                    btnEdit.Enabled = false;
                    cboBuilding.Items.Clear();
                    cboBuilding.Enabled = false;
                }

				gridManager.Location = new Point(13, 76);
				gridManager.Size = new Size(520, 313);

				gridManager.Rows.Clear();

				if (group != null)
					AddGridData(group);
			}
			else if( nSelectIdx == 2)
			{
                cboBuildingGroup.Visible = true;
                cboBuilding.Visible = true;
                cmbEquipZone.Visible = true;
                cmbFloor.Visible = true;

                if (cboBuildingGroup.SelectedIndex >= 0 &&
                    (BuildingGroup)cboBuildingGroup.Items[cboBuildingGroup.SelectedIndex] == ZoneManager.Instance.OutdoorBuildingGroup)
                {
                    cboBuilding.Items.Clear();
                    cmbFloor.Items.Clear();
                    cmbEquipZone.Items.Clear();

                    cboBuilding.Enabled = false;
                    cmbFloor.Enabled = false;

                    foreach (KeyValuePair<int, EquipmentZone> pair in ZoneManager.Instance.DicOutdoorEquipZones)
                    {
                        cmbEquipZone.Items.Add(pair.Value);
                    }

                    if (cmbEquipZone.Items.Count > 0)
                        cmbEquipZone.SelectedIndex = 0;
                }
                
				gridManager.Location = new Point(13, 111);
				gridManager.Size = new Size(520, 278);

				gridManager.Rows.Clear();
				if (group != null)
					AddGridData(group);
			}

			
			
		}

		private Zone FindZone(Building building, float fFloorIndex)
		{
			int nFloorIndex = fFloorIndex > 0.0f ? (int)(fFloorIndex + 0.01f) : (int)(fFloorIndex - 0.01f);
			string strAddFloor = string.Format("{0:f1}", fFloorIndex - nFloorIndex);

			foreach (Zone zone in building.FloorList)
			{
				if (zone.Building == building && zone.FloorIndex == nFloorIndex)
				{
					if (strAddFloor == string.Format("{0:f1}", zone.AddFloor))
						return zone;
				}
			}
			return null;
		}

		private void cmbEquipZone_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (m_nManagerMode != 2)
				return;
			EquipmentZone equipZone = (EquipmentZone)cmbEquipZone.Items[cmbEquipZone.SelectedIndex];

			FacilityManagerGroup group = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(GetCurrentType(),equipZone);

			gridManager.Rows.Clear();

			if (group != null)
				AddGridData(group);
		}


		private void cmbFloor_SelectedIndexChanged(object sender, EventArgs e)
		{
			//if (cmbFloor.Visible == false)
			//	return;

			int nSelectedIndex = cmbFloor.SelectedIndex;
			if (nSelectedIndex < 0)
				return;

			cmbEquipZone.Items.Clear();

			Object obj = cmbFloor.Items[nSelectedIndex];
			Type type = obj.GetType();

			Zone zone = null;

			if (type == typeof(Floor))
			{
				Building building = (Building)cboBuilding.Items[cboBuilding.SelectedIndex];
				Floor floor = (Floor)obj;
				zone = FindZone(building, floor.FloorIndex);
			}

			if (zone == null || zone.ID <= 0)
				return;

			ArrayList arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
			if (arrEquipZones == null)
				return;

			foreach (EquipmentZone equipZone in arrEquipZones)
			{
				cmbEquipZone.Items.Add(equipZone);
			}

			if (cmbEquipZone.Items.Count > 0)
				cmbEquipZone.SelectedIndex = 0;
		}

		//private void checkBoxBuilding_CheckedChanged(object sender, EventArgs e)
		//{
		//    Building buildingCurrent = null;
		//    Zone zoneCurrent = null;
		//    EquipmentZone eu
		//    FacilityManagerGroup group = GetCurrentFacilityGroup(ref buildingCurrent, ref zoneCurrent);

		//    if (checkBoxBuilding.Checked)
		//    {
		//        cboBuildingGroup.Visible = true;
		//        cboBuilding.Visible = true;
		//    }
		//    else
		//    {
		//        cboBuildingGroup.Visible = false;
		//        cboBuilding.Visible = false;
		//    }

		//    gridManager.Rows.Clear();

		//    if (group != null)
		//        AddGridData(group);
		//}

        private FacilityManager FindFacilityManager(int nID, ArrayList arrManagers)
        {
            foreach (FacilityManager mgr in arrManagers)
            {
                if (mgr.ID == nID)
                    return mgr;
            }

            return null;
        }

        private void RemoveGrid(FacilityManager mgr)
        {
            int nRemoveIndex = -1;
            int nRowCount = gridManager.Rows.Count;

            for (int i=0;i<nRowCount;i++)
            {
                DataGridViewRow row = gridManager.Rows[i];

                if (row.Tag == mgr)
                {
                    gridManager.Rows.RemoveAt(i);
                    nRemoveIndex = i;
                    nRowCount--;
                    break;
                }
            }

            if (nRemoveIndex < 0)
                return;

            for (int i = nRemoveIndex; i < nRowCount; i++)
            {
                DataGridViewRow row = gridManager.Rows[i];
                row.Cells[0].Value = (i + 1).ToString();
            }
        }

        private void UpdateChangedData(ArrayList arrOrigin, ArrayList arrNew)
        {
            foreach (FacilityManager mgr in arrOrigin)
            {
                // 삭제된 데이터
                if (FindFacilityManager(mgr.ID, arrNew) == null)
                {
                    EditFacilityManager editMgr = new EditFacilityManager();
                    editMgr.Manager = mgr;
                    editMgr.IsDeleting = true;
                    editMgr.AddToManager(FormMain.Instance.PageHome);
                }
            }

            foreach (FacilityManager mgr in arrNew)
            {
                if (mgr.ID < 0)
                {
                    // 새로 추가된 데이터
                    EditFacilityManager editMgr = new EditFacilityManager();
                    editMgr.Manager = mgr;
                    editMgr.Description = mgr.Description;
                    editMgr.FacilityType = Facility.ToIntType(mgr.Type);
                    editMgr.LevelLimit = mgr.LevelLimit;
                    editMgr.MemberID = mgr.MemberID;
                    editMgr.MemberType = mgr.MemberType;
					editMgr.UpperLimit = mgr.UpperLimit;
                    editMgr.AddToManager(FormMain.Instance.PageHome);
                }
                else
                {
                    // 변경된 데이터
                    FacilityManager mgr2 = FindFacilityManager(mgr.ID, arrOrigin);
                    if (mgr2 == null)
                        continue;

                    EditFacilityManager editMgr = null;

                    if (mgr.Description != mgr2.Description)
                    {
                        if (editMgr == null)
                            editMgr = new EditFacilityManager();
                        editMgr.Description = mgr.Description;
                    }

                    if (mgr.Type != mgr2.Type)
                    {
                        if (editMgr == null)
                            editMgr = new EditFacilityManager();
                        editMgr.FacilityType = Facility.ToIntType(mgr.Type);
                    }

                    if (mgr.LevelLimit != mgr2.LevelLimit)
                    {
                        if (editMgr == null)
                            editMgr = new EditFacilityManager();
                        editMgr.LevelLimit = mgr.LevelLimit;
                    }

                    if (mgr.MemberID != mgr2.MemberID)
                    {
                        if (editMgr == null)
                            editMgr = new EditFacilityManager();
                        editMgr.MemberID = mgr.MemberID;
                    }

                    if (mgr.MemberType != mgr2.MemberType)
                    {
                        if (editMgr == null)
                            editMgr = new EditFacilityManager();
                        editMgr.MemberType = mgr.MemberType;
                    }

					if (mgr.UpperLimit != mgr2.UpperLimit)
					{
						if (editMgr == null)
							editMgr = new EditFacilityManager();
						editMgr.UpperLimit = mgr.UpperLimit;
					}

                    if (editMgr != null)
                    {
                        editMgr.Manager = mgr2;
                        editMgr.AddToManager(FormMain.Instance.PageHome);
                    }
                }
            }
        }

        private FacilityManagerGroup GetCurrentFacilityGroup(bool checkedBuilding, bool alwaysGet = false)
        {
            FacilityManagerGroup groupCurrent = null;

            if (checkedBuilding)
            {
                if (cboBuilding.SelectedIndex < 0)
                    return null;

                object item = cboBuilding.Items[cboBuilding.SelectedIndex];

                if (item.GetType() == typeof(Building))
                    groupCurrent = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(GetCurrentType(), (Building)item, alwaysGet);
                else if (item.GetType() == typeof(Zone))
                    groupCurrent = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(GetCurrentType(), (Zone)item, alwaysGet);
            }
            else
                groupCurrent = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(GetCurrentType(), alwaysGet);

            return groupCurrent;
        }

        private FacilityManagerGroup GetCurrentFacilityGroup(ref Building building, ref Zone zone, ref EquipmentZone equipZone)
        {
            FacilityManagerGroup groupCurrent = null;

			if (m_nManagerMode == 0)
			{
				groupCurrent = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(GetCurrentType());
			}
			else if( m_nManagerMode == 1)
			{
				if (cboBuilding.SelectedIndex < 0)
					return null;

				object item = cboBuilding.Items[cboBuilding.SelectedIndex];

				if (item.GetType() == typeof(Building))
				{
					building = (Building)item;
					groupCurrent = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(GetCurrentType(), building);
				}
				else if (item.GetType() == typeof(Zone))
				{
					zone = (Zone)item;
					groupCurrent = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(GetCurrentType(), zone);
				}
			}
			else if (m_nManagerMode == 2)
			{
				if (cmbEquipZone.SelectedIndex < 0)
					return null;
				EquipmentZone equip = (EquipmentZone)cmbEquipZone.Items[cmbEquipZone.SelectedIndex];
				equipZone = equip;
				groupCurrent = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(GetCurrentType(), equip);
			} 
            return groupCurrent;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Building buildingCurrent = null;
            Zone zoneCurrent = null;
			FacilityManagerGroup groupCurrent = null;
			EquipmentZone equipZone = null;
			groupCurrent = GetCurrentFacilityGroup(ref buildingCurrent, ref zoneCurrent, ref equipZone);
			
           
			Facility.FacilityType type = GetCurrentType();

			FormEditManager frm = new FormEditManager(type, groupCurrent, buildingCurrent, zoneCurrent, equipZone);
			frm.ShowInTaskbar = false;
            if (PageBackstageHome.ShowTranslucentSubForm(frm) == System.Windows.Forms.DialogResult.OK)
            {
                FacilityManagerGroup group = frm.GetFacilityManagerGroup();

                if (groupCurrent == null)
                {
					if (group.EquipZone != null)
					{
						groupCurrent = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(group.Type, equipZone, true);
					}
					else
					{
						if (group.Building == null && group.Zone == null)
							groupCurrent = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(group.Type, true);
						else if (group.Building != null)
							groupCurrent = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(group.Type, group.Building, true);
						else// if (group.Zone != null)
							groupCurrent = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(group.Type, group.Zone, true);
                
					}
                }

                UpdateChangedData(groupCurrent.RegularTeams, group.RegularTeams);
                UpdateChangedData(groupCurrent.CompanyMembers, group.CompanyMembers);
                UpdateChangedData(groupCurrent.ExternalTeams, group.ExternalTeams);
                UpdateChangedData(groupCurrent.ExternalCompanyMembers, group.ExternalCompanyMembers);

                groupCurrent.CopyFrom(group);
                UpdateGridMember(groupCurrent);
            }
        }

        private void UpdateGridMember(FacilityManagerGroup group)
        {
			          
            //cmbType.SelectedIndex = m_nManagerMode;
            
            gridManager.Rows.Clear();
            AddGridData(group);
        }

        private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
			int nSelectedIndex = cboBuilding.SelectedIndex;
			if (nSelectedIndex == -1)
				return;

            Building buildingCurrent = null;
            Zone zoneCurrent = null;
			EquipmentZone equipZone = null;
			FacilityManagerGroup group = GetCurrentFacilityGroup(ref buildingCurrent, ref zoneCurrent, ref equipZone);

            gridManager.Rows.Clear();

            if (group != null)
                AddGridData(group);

			
			cmbFloor.Items.Clear();
			Object obj = cboBuilding.Items[nSelectedIndex];
			Type type = obj.GetType();

			if (type == typeof(Building))
			{
				Building building = (Building)obj;
				ArrayList arrFloor = (ArrayList)building.FloorList.Clone();
				
				foreach (Zone floor in arrFloor)
				{
					cmbFloor.Items.Add(floor.Floor);
				}
			}
			else
			{
				cmbFloor.Items.Add("-");
			}

			if (cmbFloor.Items.Count > 0)
				cmbFloor.SelectedIndex = 0;
        }

		private void gridManager_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
		{
			
		}
	}
}
