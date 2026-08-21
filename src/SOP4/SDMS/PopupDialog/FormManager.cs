using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;

namespace SDMS
{
	public partial class FormManager : Form
	{
        private int FIRE_SENSOR = -1;
        private int PSM_SENSOR = -1;
        private int SECURITY_SENSOR = -1;

        // Key : cmbType Index
        private Dictionary<int, UnE.Security.SecurityType> m_dicSecurityTypes = null;

        private string[] m_strFireType = { "전체 시설물", "건물별 보기", "화재구역별 보기" };
        private string[] m_strPSMType = { "전체 시설물", "시설별 보기" };
        private string[] m_strSecurityType = { "전체 시설물", "위치별 보기" };

        protected enum ManagerType { Entire = 0, Building, EquipmentZone, SecuritySensor };
        private ManagerType m_nManagerMode = ManagerType.Entire;

		public FormManager()
		{
            
			InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(gridManager, true);

			labelDescription.Text = "";
			btnShowSimulationModeManager.Visible = FormMain.Instance.SimulationMode;
		}

		private void FormManager_Load(object sender, EventArgs e)
		{
            InitFacilityManagerType();
			InitComboBox();
			InitGrid();
		}

        private void InitFacilityManagerType()
        {
            if (FormMain.Instance.DataManager.UseReportFacilityManagers)
            {
                radioDetect.Visible = radioReport.Visible = true;
            }
            /*string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'UseFacilityManagerType' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strValue = DBUtility.WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return;

            strValue = strValue.Trim();

            if (strValue == "1" || string.Compare(strValue, "true", true) == 0)
            {
                radioDetect.Visible = radioReport.Visible = true;
            }*/
        }

		private IFacility.FacilityType GetCurrentType()
		{
            return mCurrentType;

            //IFacility.FacilityType type = IFacility.FacilityType.NONE;

            //if (cboSensorType.SelectedIndex == 0)
            //    type = IFacility.FacilityType.FIRE_SENSOR;
            //else if(cboSensorType.SelectedIndex == 1)
            //    type 
            //else if (cboSensorType.SelectedIndex == 2)
            //    type = IFacility.FacilityType.CCTV;
            //else if (cboSensorType.SelectedIndex == 3)
            //    type = IFacility.FacilityType.FE;

            //return type;
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
            int nItemIndex = 0;

            cboSensorType.Items.Add("화재센서");
            FIRE_SENSOR = nItemIndex++;

            if (UnE.SOP.ProxySOP.Instance.UsePSM == true)
            {
                cboSensorType.Items.Add("유해물질센서");
                PSM_SENSOR = nItemIndex++;
            }

            if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                cboSensorType.Items.Add("방범센서");
                SECURITY_SENSOR = nItemIndex++;
            }

			cboSensorType.SelectedIndex = 0;
		}

        private void InitBuildingGroup()
        {
            cboBuildingGroup.Items.Clear();
            foreach (KeyValuePair<int, BuildingGroup> pair in ZoneManager.Instance.DicBuildingGroup)
            {
                cboBuildingGroup.Items.Add(pair.Value);
            }

            cboBuildingGroup.Items.Add(ZoneManager.Instance.OutdoorBuildingGroup);

            cboBuildingGroup.Sorted = true;
            cboBuildingGroup.Sorted = false;

            cboBuildingGroup.SelectedIndex = 0;
        }

        private IFacility.FacilityType mCurrentType = IFacility.FacilityType.NONE;
        private void cboSensorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mCurrentType = IFacility.FacilityType.NONE;

            if (cboSensorType.SelectedIndex == FIRE_SENSOR)
            {
                labelDescription.Text = "(화재센서 / 연기식 아나로그 / 광센서)";
                mCurrentType = IFacility.FacilityType.FIRE_SENSOR;

                cmbType.Items.Clear();
                cmbType.Items.AddRange(m_strFireType);
                
                if (cmbType.Items.Count > 0)
                    cmbType.SelectedIndex = 0;

                cmbType.Refresh();

                InitBuildingGroup();
            }
            else if (cboSensorType.SelectedIndex == PSM_SENSOR)
            {
                labelDescription.Text = "";
                mCurrentType = IFacility.FacilityType.PSM_SENSOR;

                cmbType.Items.Clear();
                cmbType.Items.AddRange(m_strPSMType);

                if (cmbType.Items.Count > 0)
                    cmbType.SelectedIndex = 0;

                cmbType.Refresh();

                cmbEquipZone.Items.Clear();
                cmbEquipZone.Items.AddRange(ZoneManager.Instance.GetPSMEquipmentZone());

                if (cmbEquipZone.Items.Count > 0)
                    cmbEquipZone.SelectedIndex = 0;

                cmbType.Refresh();
            }
            else if (cboSensorType.SelectedIndex == SECURITY_SENSOR)
            {
                labelDescription.Text = "";
                mCurrentType = IFacility.FacilityType.Security_Sensor;

                cmbType.Items.Clear();
                cmbType.Items.AddRange(m_strSecurityType);

                if (cmbType.Items.Count > 0)
                    cmbType.SelectedIndex = 0;

                cmbType.Refresh();

                InitBuildingGroup();
                /*SetSecurityTypes();

                if (cmbType.Items.Count > 0)
                    cmbType.SelectedIndex = 0;

                cmbType.Refresh();*/
            }
            // 사용하지 않는 타입들.
            //else if (cboSensorType.SelectedIndex == 2)
            //{
            //    labelDescription.Text = "";
            //    type = IFacility.FacilityType.CCTV;
            //}
            //else if (cboSensorType.SelectedIndex == 3)
            //{
            //    labelDescription.Text = "(소화기 / 소화전 / 발신기)";
            //    type = IFacility.FacilityType.FE;
            //}



            // 건물별 보기는 안쓰는 기능...
            //if (checkBoxBuilding.Checked)
            //{
            //    object item = cboBuilding.Items[cboBuilding.SelectedIndex];

            //    if (item.GetType() == typeof(Building))
            //        LoadBuildingManager(type, (Building)item);
            //    else
            //        LoadOutdoorManager(type, (Zone)item);
            //}
            //else
            //{
            LoadEntireManager(mCurrentType);
            //}

        }

        private void SetSecurityTypes()
        {
            cboAdditional.Items.Clear();

            if (m_dicSecurityTypes == null)
            {
                m_dicSecurityTypes = new Dictionary<int, UnE.Security.SecurityType>();
                LoadSecurityTypes();
            }

            int nTypeCount = m_dicSecurityTypes.Count;

            for (int i=0;i<nTypeCount;i++)
            {
                UnE.Security.SecurityType type = m_dicSecurityTypes[i];
                cboAdditional.Items.Add(type);
            }
        }

        private void LoadSecurityTypes()
        {
            string strSQL = "select st.ID, st.FacilityTypeIDs, sdc.SubCategoryName ";
            strSQL += "from SecurityTypeTable as st, SubDisasterCategory as sdc ";
            strSQL += "where st.SecurityType = sdc.ID";

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nFacilityTypeID;
            int nResultCount = arrResult.Count;
            int nIndex = 1;

            Dictionary<IFacility.FacilityType, IFacility.FacilityType> dicAllSensorTypes = new Dictionary<IFacility.FacilityType, IFacility.FacilityType>();

            for (int i=0;i<nResultCount-2;i+=3)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());
                string strFacilityTypeIDs = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                string strTypeName = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);

                if (id == null || strFacilityTypeIDs == null || strTypeName == null)
                    continue;

                UnE.Security.SecurityType type = new UnE.Security.SecurityType();
                string[] tokens = strFacilityTypeIDs.Split(',');

                foreach (string strToken in tokens)
                {
                    if (int.TryParse(strToken.Trim(), out nFacilityTypeID) == false)
                        continue;

                    IFacility.FacilityType facilityType = UnE.Sensor.IFacility.ToFacilityType(nFacilityTypeID);

                    if (facilityType == IFacility.FacilityType.NONE)
                        continue;

                    type.LinkedFacilityTypes.Add(facilityType);
                    dicAllSensorTypes[facilityType] = facilityType;
                }

                if (type.LinkedFacilityTypes.Count == 0)
                    continue;

                type.TypeID = id.Data;
                type.TypeName = strTypeName;

                m_dicSecurityTypes[nIndex++] = type;
            }

            UnE.Security.SecurityType allSensors = new UnE.Security.SecurityType();
            allSensors.TypeID = 0;
            allSensors.TypeName = "전체 센서";

            foreach (KeyValuePair<UnE.Sensor.IFacility.FacilityType, UnE.Sensor.IFacility.FacilityType> pair in dicAllSensorTypes)
            {
                allSensors.LinkedFacilityTypes.Add(pair.Value);
            }

            m_dicSecurityTypes[allSensors.TypeID] = allSensors;
        }

		private void LoadBuildingManager(IFacility.FacilityType type, Building building)
		{
			gridManager.Rows.Clear();

			FacilityManagerGroup group = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(type, building, radioDetect.Checked);
			AddGridData(group);
		}

		private void LoadOutdoorManager(IFacility.FacilityType type, Zone zone)
		{
			gridManager.Rows.Clear();

            FacilityManagerGroup group = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(type, zone, radioDetect.Checked);
			AddGridData(group);
		}

		private void LoadEntireManager(IFacility.FacilityType type)
		{
			gridManager.Rows.Clear();

			FacilityManagerGroup group = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(type, radioDetect.Checked);
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

				if (member == null)
					continue;

                DataTeam team = member.GetFirstTeam();

                if (team == null)
                    continue;

				AddGridRowData(ref nIndex, member.MemberName, team.TeamName, mgr);
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

				if (member == null)
					continue;

                //DataTeam team = member.GetFirstTeam();
                DataTeam team = member.Team;

                if (team == null)
                    continue;

                if (team.CompanyName != team.TeamName)
                {
                    if (team.CompanyName.Length == 0)
                        AddGridRowData(ref nIndex, member.Name, team.TeamName, mgr);
                    else if (team.TeamName.Length == 0)
                        AddGridRowData(ref nIndex, member.Name, team.CompanyName, mgr);
                    else
                        AddGridRowData(ref nIndex, member.Name, team.CompanyName + " " + team.TeamName, mgr);
                }
                else
                    AddGridRowData(ref nIndex, member.Name, team.CompanyName, mgr);
			}

            foreach (FacilityManager mgr in group.ControlRoomMembers)
            {
                DataTeam team = (DataTeam)mgr.Tag;

                if (team == null)
                    continue;

                string strParentTeamName = "";

                if (team.ParentTeam != null)
                    strParentTeamName = team.ParentTeam.TeamName;

                AddGridRowData(ref nIndex, team.TeamName, strParentTeamName, mgr);
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

                // 화재센서의 빌딩
                if (mCurrentType == IFacility.FacilityType.FIRE_SENSOR || mCurrentType == IFacility.FacilityType.Security_Sensor)
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
			}
			else
			{
				cboBuilding.Enabled = false;

				if ((mCurrentType == IFacility.FacilityType.FIRE_SENSOR && cmbType.SelectedIndex == 1) ||
                    (mCurrentType == IFacility.FacilityType.PSM_SENSOR && cmbType.SelectedIndex == 1)) // 건물별 보기
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

		private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nSelectIdx = cmbType.SelectedIndex;
			if (nSelectIdx < 0)
				return;
             
			m_nManagerMode = (ManagerType)nSelectIdx;

            if (mCurrentType == IFacility.FacilityType.Security_Sensor && nSelectIdx == 1)
                m_nManagerMode = ManagerType.EquipmentZone;

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
                cboAdditional.Visible = false;

                ResizeGridView(1);

				gridManager.Rows.Clear();

				if (group != null)
					AddGridData(group);
			}
			else if (nSelectIdx == 1)
			{
                if (mCurrentType == IFacility.FacilityType.FIRE_SENSOR)
                {
                    cboBuildingGroup.Visible = true;
                    cboBuilding.Visible = true;
                    cmbEquipZone.Visible = false;
                    cmbFloor.Visible = false;
                    cboAdditional.Visible = false;

                    if (cboBuildingGroup.SelectedIndex >= 0 &&
                        (BuildingGroup)cboBuildingGroup.Items[cboBuildingGroup.SelectedIndex] == ZoneManager.Instance.OutdoorBuildingGroup)
                    {
                        btnEdit.Enabled = false;
                        cboBuilding.Items.Clear();
                        cboBuilding.Enabled = false;
                    }

                    ResizeGridView(1);

                    gridManager.Rows.Clear();

                    if (group != null)
                        AddGridData(group);
                }
                else if (mCurrentType == IFacility.FacilityType.PSM_SENSOR)
                {
                    m_nManagerMode = ManagerType.EquipmentZone;

                    cboBuildingGroup.Visible = false;
                    cboBuilding.Visible = false;
                    cmbEquipZone.Visible = true;
                    cmbFloor.Visible = false;
                    cboAdditional.Visible = false;

                    cmbEquipZone.Location = new Point(140, 45);

                    cmbEquipZone.Items.Clear();
                    cmbEquipZone.Items.AddRange(ZoneManager.Instance.GetPSMEquipmentZone());
                    if( cmbEquipZone.Items.Count > 0)
                        cmbEquipZone.SelectedIndex = 0;

                    ResizeGridView(1);

                    gridManager.Rows.Clear();

                    if (group != null)
                        AddGridData(group);
                }
                else if (mCurrentType == IFacility.FacilityType.Security_Sensor)
                {
                    m_nManagerMode = ManagerType.EquipmentZone;

                    cboBuildingGroup.Visible = true;
                    cboBuilding.Visible = true;
                    cmbEquipZone.Visible = true;
                    cmbFloor.Visible = true;
                    cboAdditional.Visible = false;
                    cmbEquipZone.Location = new Point(140, 76);

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

                    ResizeGridView(2);

                    gridManager.Rows.Clear();
                    if (group != null)
                        AddGridData(group);
                }  
			}
			else if (nSelectIdx == 2)
			{
				cboBuildingGroup.Visible = true;
				cboBuilding.Visible = true;
				cmbEquipZone.Visible = true;
				cmbFloor.Visible = true;
                cboAdditional.Visible = false;
                cmbEquipZone.Location = new Point(140, 76);

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

                ResizeGridView(2);

				gridManager.Rows.Clear();
				if (group != null)
					AddGridData(group);
			}
		}

        /// <summary>
        /// 그리드뷰 컨트롤 크기 및 위치 변경
        /// </summary>
        /// <param name="nMode">1 : point(13, 76) size(520, 313), 2 : point(13, 111) size(520, 278)</param>
        private void ResizeGridView(int nMode)
        {
            if (nMode == 1)
            {
                gridManager.Location = new Point(13, 76);
                gridManager.Size = new Size(520, 313);
            }
            else if (nMode == 2)
            {
                gridManager.Location = new Point(13, 111);
                gridManager.Size = new Size(520, 278);
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
			if (m_nManagerMode != ManagerType.EquipmentZone)
				return;
			EquipmentZone equipZone = (EquipmentZone)cmbEquipZone.Items[cmbEquipZone.SelectedIndex];

            FacilityManagerGroup group = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(GetCurrentType(), equipZone, radioDetect.Checked);

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

            List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
			if (arrEquipZones == null)
				return;

            // 같은 층에 중복된 EquipZone이 존재할 경우 중복을 제거한다.
            RemoveDuplicateEquipZone(arrEquipZones);

			foreach (EquipmentZone equipZone in arrEquipZones)
			{
				cmbEquipZone.Items.Add(equipZone);
			}

			if (cmbEquipZone.Items.Count > 0)
				cmbEquipZone.SelectedIndex = 0;
		}

        // 같은 층에 중복된 EquipZone이 존재할 경우 중복을 제거한다.
        private void RemoveDuplicateEquipZone(List<EquipmentZone> arrEquipZones)
        {
            ArrayList arrRemove = new ArrayList();

            foreach (EquipmentZone equipZone in arrEquipZones)
            {
                if (equipZone.LinkedZoneList.Count > 1)
                {
                    if (GetEquipmentZone(arrEquipZones, equipZone) != null)
                        arrRemove.Add(equipZone);
                }
            }

            foreach (EquipmentZone equipZone in arrRemove)
            {
                arrEquipZones.Remove(equipZone);
            }
        }

        // equipZone과 같은 LinkedZone을 공유하는 EquipmentZone 객체를 찾아낸다.
        private EquipmentZone GetEquipmentZone(List<EquipmentZone> arrEquipZones, EquipmentZone equipZone)
        {
            foreach (EquipmentZone eZone in arrEquipZones)
            {
                if (eZone == equipZone)
                    continue;

                if (eZone.LinkedZoneList.Count == 1)
                {
                    if (equipZone.LinkedZoneList.Contains(eZone.LinkedZone))
                        return eZone;
                }
            }

            return null;
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

			for (int i = 0; i < nRowCount; i++)
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
                    editMgr.IsDetect = radioDetect.Checked;
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
                    editMgr.IsDetect = radioDetect.Checked;
					editMgr.Description = mgr.Description;
					editMgr.FacilityType = (int)mgr.Type;
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
                        editMgr.FacilityType = (int)mgr.Type;
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
                        editMgr.IsDetect = radioDetect.Checked;
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
                    groupCurrent = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(GetCurrentType(), (Building)item, radioDetect.Checked, alwaysGet);
				else if (item.GetType() == typeof(Zone))
                    groupCurrent = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(GetCurrentType(), (Zone)item, radioDetect.Checked, alwaysGet);
			}
			else
                groupCurrent = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(GetCurrentType(), radioDetect.Checked, alwaysGet);

			return groupCurrent;
		}

		private FacilityManagerGroup GetCurrentFacilityGroup(ref Building building, ref Zone zone, ref EquipmentZone equipZone)
		{
			FacilityManagerGroup groupCurrent = null;

			if (m_nManagerMode == ManagerType.Entire)
			{
                groupCurrent = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(GetCurrentType(), radioDetect.Checked);
			}
			else if (m_nManagerMode == ManagerType.Building)
			{
				if (cboBuilding.SelectedIndex < 0)
					return null;

				object item = cboBuilding.Items[cboBuilding.SelectedIndex];

				if (item.GetType() == typeof(Building))
				{
					building = (Building)item;
                    groupCurrent = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(GetCurrentType(), building, radioDetect.Checked);
				}
				else if (item.GetType() == typeof(Zone))
				{
					zone = (Zone)item;
                    groupCurrent = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(GetCurrentType(), zone, radioDetect.Checked);
				}
			}
			else if (m_nManagerMode == ManagerType.EquipmentZone)
			{
				if (cmbEquipZone.SelectedIndex < 0)
					return null;
				EquipmentZone equip = (EquipmentZone)cmbEquipZone.Items[cmbEquipZone.SelectedIndex];
				equipZone = equip;
				groupCurrent = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(GetCurrentType(), equip, radioDetect.Checked);
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

            IFacility.FacilityType type = GetCurrentType();

			FormEditManager frm = new FormEditManager(type, groupCurrent, buildingCurrent, zoneCurrent, equipZone);
			frm.ShowInTaskbar = false;
			if (PageBackstageHome.ShowTranslucentSubForm(frm) == System.Windows.Forms.DialogResult.OK)
			{
				FacilityManagerGroup group = frm.GetFacilityManagerGroup();

				if (groupCurrent == null)
				{
					if (group.EquipZone != null)
					{
                        groupCurrent = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(group.Type, equipZone, radioDetect.Checked, true);
					}
					else
					{
						if (group.Building == null && group.Zone == null)
							groupCurrent = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(group.Type, radioDetect.Checked, true);
						else if (group.Building != null)
                            groupCurrent = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(group.Type, group.Building, radioDetect.Checked, true);
						else// if (group.Zone != null)
                            groupCurrent = FormMain.Instance.DataManager.GetOutdoorFacilityManagerGroup(group.Type, group.Zone, radioDetect.Checked, true);
					}
				}

				UpdateChangedData(groupCurrent.RegularTeams, group.RegularTeams);
				UpdateChangedData(groupCurrent.CompanyMembers, group.CompanyMembers);
				UpdateChangedData(groupCurrent.ExternalTeams, group.ExternalTeams);
				UpdateChangedData(groupCurrent.ExternalCompanyMembers, group.ExternalCompanyMembers);
                UpdateChangedData(groupCurrent.ControlRoomMembers, group.ControlRoomMembers);

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

		private void btnShowSimulationModeManager_Click(object sender, EventArgs e)
		{
			FormManager_Simulation frm = new FormManager_Simulation();
			frm.ShowInTaskbar = false;
			PageBackstageHome.ShowTranslucentSubForm(frm);
		}

        private void radioFacilityType_CheckedChanged(object sender, EventArgs e)
        {
            cboSensorType_SelectedIndexChanged(null, null);
        }
	}
}