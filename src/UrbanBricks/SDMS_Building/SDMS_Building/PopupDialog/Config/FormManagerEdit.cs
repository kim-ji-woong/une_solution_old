using DBUtility2;
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
using UnE.Sensor;
using UnE.Spatial;

namespace SDMS_Building.PopupDialog.Config
{
    public partial class FormManagerEdit : Form
    {
        private UEWpfControl.WpfComboBox m_cbType = null;
        private UEWpfControl.WpfComboBox m_cbBuilding = null;
        private UEWpfControl.WpfComboBox m_cbFloor = null;

        private int FIRE_SENSOR = -1;
        private int PSM_SENSOR = -1;
        private int STRONG_WIND = -1;
        private int EARTHQUAKE_SENSOR = -1;
        private int FIREWALL_SENSOR = -1;
        private int DOOR_SENSOR = -1;
        private int BLACKOUT_SENSOR = -1;

        private enum ComboFacility { TYPE = 0, BUILDING, FLOOR };

        private ComboFacility CB_FACILITY = ComboFacility.TYPE;

        public enum SelectMode { REGULAR_TEAM = 0, REGULAR_MEMBER, EXTERNAL_TEAM, EXTERNAL_MEMBER, DUTY_MEMBER, CONTROL_ROOM, NONE };

        private SelectMode m_selectMode = SelectMode.NONE;

        private FacilityManagerGroup m_faciltyMgrGroup = new FacilityManagerGroup();

        private IFacility.FacilityType mCurrentType = IFacility.FacilityType.NONE;
        private Building m_building = null;
        private Floor m_floor = null;

        public FormManagerEdit(IFacility.FacilityType type, Building building = null, Floor floor = null)
        {
            InitializeComponent();

            Region = System.Drawing.Region.FromHrgn(FormMain.CreateRoundRectRgn(0, 0, this.Width, this.Height, 35, 35));

            m_cbType = new UEWpfControl.WpfComboBox();
            eleType.Child = m_cbType;
            m_cbType.customComboBox.SelectionChanged += EleTypeComboBox_SelectionChanged;
            m_cbType.SetSize(eleType.Width, eleType.Height);

            m_cbBuilding = new UEWpfControl.WpfComboBox();
            eleBuilding.Child = m_cbBuilding;
            m_cbBuilding.customComboBox.SelectionChanged += EleBuildingComboBox_SelectionChanged;
            m_cbBuilding.SetSize(eleBuilding.Width, eleBuilding.Height);

            m_cbFloor = new UEWpfControl.WpfComboBox();
            eleFloor.Child = m_cbFloor;
            m_cbFloor.customComboBox.SelectionChanged += EleFloorComboBox_SelectionChanged;
            m_cbFloor.SetSize(eleFloor.Width, eleFloor.Height);

            mCurrentType = type;
            m_building = building;
            m_floor = floor;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            FormMain.Instance.DataManager.LoadFacilityManager();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void InitTypeComboBox()
        {
            int nItemIndex = 0;
            m_cbType.customComboBox.Items.Add(Data.CommonString.POI_Fire_Kor);
            FIRE_SENSOR = nItemIndex++;

            if (UnE.SOP.ProxySOP.Instance.UsePSM == true)
            {
                m_cbType.customComboBox.Items.Add(Data.CommonString.POI_Gas_Kor);
                PSM_SENSOR = nItemIndex++;
            }

            if (UnE.SOP.ProxySOP.Instance.UseEarthquake == true)
            {
                m_cbType.customComboBox.Items.Add(Data.CommonString.POI_Earthquake_Kor);
                EARTHQUAKE_SENSOR = nItemIndex++;
            }

            if (UnE.SOP.ProxySOP.Instance.UseStrongWind == true)
            {
                m_cbType.customComboBox.Items.Add(Data.CommonString.POI_StrongWind_Kor);
                STRONG_WIND = nItemIndex++;
            }

            if (UnE.SOP.ProxySOP.Instance.UseBlackout == true)
            {
                m_cbType.customComboBox.Items.Add(Data.CommonString.POI_Blackout_Kor);
                BLACKOUT_SENSOR = nItemIndex++;
            }

            InitTypeSelect();
        }

        private void InitTypeSelect()
        {
            if (mCurrentType == IFacility.FacilityType.FIRE_SENSOR)
            {
                m_cbType.customComboBox.SelectedIndex = FIRE_SENSOR;
            }
            else if (mCurrentType == IFacility.FacilityType.PSM_SENSOR)
            {
                m_cbType.customComboBox.SelectedIndex = PSM_SENSOR;
            }
            else if (mCurrentType == IFacility.FacilityType.STRONG_WIND)
            {
                m_cbType.customComboBox.SelectedIndex = STRONG_WIND;
            }
            else if (mCurrentType == IFacility.FacilityType.Earthquake)
            {
                m_cbType.customComboBox.SelectedIndex = EARTHQUAKE_SENSOR;
            }
            else if (mCurrentType == IFacility.FacilityType.FIREWALL)
            {
                m_cbType.customComboBox.SelectedIndex = FIREWALL_SENSOR;
            }
            else if (mCurrentType == IFacility.FacilityType.DOOR)
            {
                m_cbType.customComboBox.SelectedIndex = DOOR_SENSOR;
            }
            else if (mCurrentType == IFacility.FacilityType.BLACKOUT)
            {
                m_cbType.customComboBox.SelectedIndex = BLACKOUT_SENSOR;
            }
        }

        private void InitBuildingComboBox()
        {
            m_cbBuilding.customComboBox.Items.Clear();
            m_cbBuilding.customComboBox.Items.Add("모두");

            foreach (KeyValuePair<int, Building> item in UnE.Spatial.ZoneManager.Instance.DicBuildings)
            {
                m_cbBuilding.customComboBox.Items.Add(item.Value);
            }

            if (m_cbBuilding.customComboBox.Items.Count > 0)
                m_cbBuilding.customComboBox.SelectedIndex = 0;

            if (m_building != null)
                m_cbBuilding.customComboBox.SelectedItem = m_building;
        }

        private void InitFloorComboBox()
        {
            if (m_floor != null)
                m_cbFloor.customComboBox.SelectedItem = m_floor;
        }

        private void InitTree()
        {
            MakeTeam(treeViewTeam.Nodes, FormMain.Instance.DataManager.RegularTeamRoot);
            // 교대 근무자
            MakeControlRoomMembers(treeViewTeam.Nodes);
            MakeExternalTeams(treeViewTeam.Nodes, FormMain.Instance.DataManager.ExternalTeamRootList);

            if (treeViewTeam.Nodes.Count > 0)
            {
                treeViewTeam.ExpandAll();
                treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];
            }

            OnAfterTreeSelect();
        }

        private void MakeTeam(TreeNodeCollection nodes, DataTeam team)
        {
            TreeNode node = new TreeNode();
            node.Text = team.TeamName;
            node.Tag = team;

            nodes.Add(node);

            foreach (DataTeam teamChild in team.ChildTeams)
            {
                MakeTeam(node.Nodes, teamChild);
            }
        }

        private void MakeControlRoomMembers(TreeNodeCollection nodes)
        {
            DataTeamControlRoom teamRoot = FormMain.Instance.DataManager.GetRootControlRoomTeam();
            MakeTeam(nodes, teamRoot);
        }

        private void MakeExternalTeams(TreeNodeCollection nodes, ArrayList arrTeams)
        {
            foreach (DataTeam team in arrTeams)
            {
                MakeTeam(nodes, team);
            }
        }

        private void OnAfterTreeSelect()
        {
            TreeNode node = treeViewTeam.SelectedNode;
            if (node == null || node.Tag == null)
                return;

            DataTeam team = (DataTeam)node.Tag;

            if (team is DataTeamControlRoom)
            {
                SelectControlRoom();
            }
            else
            {
                if (team.External)
                    SelectExternalTeam();
                else
                    SelectTeam();
            }

            gridMember.Rows.Clear();

            int nIndex = 1;
            UpdateMembers(team, ref nIndex);
        }

        private void SelectControlRoom()
        {
            m_selectMode = SelectMode.CONTROL_ROOM;
        }

        private void SelectExternalTeam()
        {
            m_selectMode = SelectMode.EXTERNAL_TEAM;
        }

        private void SelectTeam()
        {
            m_selectMode = SelectMode.REGULAR_TEAM;
        }

        private void UpdateMembers(DataTeam team, ref int nIndex)
        {
            ArrayList arrMembers = FormMain.Instance.DataManager.GetTeamMembers(team);

            if (team.External)
            {
                //colLevel.Visible = false;

                if (arrMembers != null)
                {
                    foreach (DataExternalMember member in arrMembers)
                    {
                        AddMember(nIndex++, member.Name, -1, member);
                    }
                }
            }
            else
            {
                //colLevel.Visible = true;

                if (arrMembers != null)
                {
                    foreach (DataCompanyMember member in arrMembers)
                    {
                        AddMember(nIndex++, member.MemberName, member.LevelID, member);
                    }
                }
            }

            foreach (DataTeam teamChild in team.ChildTeams)
            {
                UpdateMembers(teamChild, ref nIndex);
            }
        }

        private void AddMember(int nIndex, string strName, int nLevel, object objMember)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.Tag = objMember;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nIndex.ToString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strName;
            row.Cells.Add(cell);

            /*
            cell = new DataGridViewTextBoxCell();
            cell.Value = nLevel <= 0 ? "" : nLevel.ToString() + "급";
            row.Cells.Add(cell);
            */

            gridMember.Rows.Add(row);
        }

        private void InitManagers()
        {
            foreach (FacilityManager mgr in m_faciltyMgrGroup.RegularTeams)
            {
                if (mgr.LevelLimit == 0)
                    continue;

                DataTeam team = (DataTeam)mgr.Tag;

                if (team == null)
                    continue;

                //string szSub = mgr.UpperLimit == true ? "및 그 상위 직급만 해당" : "및 그 하위 직급만 해당";
                string szSub = GetLimitText(mgr.UpperLimit);
                string strEtc = mgr.LevelLimit < 0 ? "팀원 모두" : string.Format("{0}급", mgr.LevelLimit) + szSub;
                AddManager(team, mgr.LevelLimit, strEtc, mgr.UpperLimit, mgr);
            }

            foreach (FacilityManager mgr in m_faciltyMgrGroup.CompanyMembers)
            {
                DataCompanyMember member = (DataCompanyMember)mgr.Tag;

                if (member == null)
                    continue;

                AddManager(member, mgr);
            }

            foreach (FacilityManager mgr in m_faciltyMgrGroup.ExternalTeams)
            {
                DataTeam team = (DataTeam)mgr.Tag;

                if (team == null)
                    continue;

                AddManager(team, mgr.LevelLimit, team.CompanyName, mgr.UpperLimit, mgr);
            }

            foreach (FacilityManager mgr in m_faciltyMgrGroup.ExternalCompanyMembers)
            {
                DataExternalMember member = (DataExternalMember)mgr.Tag;

                if (member == null)
                    continue;

                AddManager(member, mgr);
            }

            foreach (FacilityManager mgr in m_faciltyMgrGroup.ControlRoomMembers)
            {
                DataTeamControlRoom team = (DataTeamControlRoom)mgr.Tag;

                if (team == null)
                    continue;

                string strParentTeamName = team.ParentTeam == null ? "" : team.ParentTeam.TeamName;
                AddManager(team, -1, strParentTeamName, -1, mgr);
            }
        }

        private string GetLimitText(int nUpperLimit)
        {
            if (nUpperLimit > 0)
                return " 및 그 상위 직급만 해당";
            else if (nUpperLimit < 0)
                return " 및 그 하위 직급만 해당";

            return "만 해당";
        }

        private void AddManager(DataTeam team, int nLevel, string strEtc, int nUpperLimit, FacilityManager mgr)
        {
            int nMemberType, nMemberID;
            GetMemberType(team, mgr, out nMemberID, out nMemberType);

            if (FindGridRow(nMemberID, nMemberType, nLevel, nUpperLimit) != null)
                return;


            DataGridViewRow row = new DataGridViewRow();
            row.Tag = m_selectMode;

            int nIndex = gridManager.Rows.Count + 1;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nIndex.ToString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = team;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strEtc;
            row.Cells.Add(cell);
            cell.Tag = nLevel;

            gridManager.Rows.Add(row);

            if (mgr == null && CB_FACILITY == ComboFacility.FLOOR)
            {
                // 층 정보를 받아
                string floorName = (string)m_cbFloor.customComboBox.SelectedItem.ToString().Trim();
                Floor floor = (Floor)m_cbFloor.customComboBox.SelectedItem;

                // 해당 층 이킵존을 조회
                List<int> listEquipZoneID = GetEquipZoneList(floorName);

                // 이킵존 리스트를 이용하여 담당자 추가
                foreach (int nEquipZoneID in listEquipZoneID)
                {
                    EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                    mgr = new FacilityManager();

                    mgr.MemberID = nMemberID;
                    mgr.MemberType = nMemberType;

                    //mgr.MemberType = team.External ? 3 : 1;
                    mgr.Type = m_faciltyMgrGroup.Type;
                    mgr.LevelLimit = nLevel;
                    if (nLevel >= 0)
                    {
                        if (strEtc.IndexOf("상위") != -1)
                        {
                            mgr.UpperLimit = 1;
                        }
                        else if (strEtc.IndexOf("하위") != -1)
                        {
                            mgr.UpperLimit = -1;
                        }
                        else
                        {
                            mgr.UpperLimit = 0;
                        }
                    }
                    mgr.Tag = team;
                    mgr.Building = m_faciltyMgrGroup.Building;
                    mgr.Zone = m_faciltyMgrGroup.Zone;
                    mgr.EquipZone = zone;

                    if (nMemberType == 7)
                        m_faciltyMgrGroup.ControlRoomMembers.Add(mgr);
                    else
                    {
                        if (team.External)
                            m_faciltyMgrGroup.ExternalTeams.Add(mgr);
                        else
                            m_faciltyMgrGroup.RegularTeams.Add(mgr);
                    }

                    if (CB_FACILITY == ComboFacility.FLOOR)
                        FormMain.Instance.DataManager.AddEquipZoneFacilityManager(mgr, mgr.EquipZone, mCurrentType);
                    else if (CB_FACILITY == ComboFacility.BUILDING)
                        FormMain.Instance.DataManager.AddBuildingFacilityManager(mgr, mgr.Building, mCurrentType);
                    else if (CB_FACILITY == ComboFacility.TYPE)
                        FormMain.Instance.DataManager.AddFacilityManager(mgr, mCurrentType); ;
                }
            }
            else if (mgr == null)
            {
                /*if (nMemberType == 6)
					mgr = FormMain.Instance.DataManager.NewFacilityManagerDuty();
				else
					*/
                mgr = new FacilityManager();

                mgr.MemberID = nMemberID;
                mgr.MemberType = nMemberType;

                //mgr.MemberType = team.External ? 3 : 1;
                mgr.Type = m_faciltyMgrGroup.Type;
                mgr.LevelLimit = nLevel;
                if (nLevel >= 0)
                {
                    if (strEtc.IndexOf("상위") != -1)
                    {
                        mgr.UpperLimit = 1;
                    }
                    else if (strEtc.IndexOf("하위") != -1)
                    {
                        mgr.UpperLimit = -1;
                    }
                    else
                    {
                        mgr.UpperLimit = 0;
                    }
                }
                mgr.Tag = team;
                mgr.Building = m_faciltyMgrGroup.Building;
                mgr.Zone = m_faciltyMgrGroup.Zone;
                mgr.EquipZone = m_faciltyMgrGroup.EquipZone;

                if (nMemberType == 7)
                    m_faciltyMgrGroup.ControlRoomMembers.Add(mgr);
                else
                {
                    if (team.External)
                        m_faciltyMgrGroup.ExternalTeams.Add(mgr);
                    else
                        m_faciltyMgrGroup.RegularTeams.Add(mgr);
                }

                if (CB_FACILITY == ComboFacility.FLOOR)
                    FormMain.Instance.DataManager.AddEquipZoneFacilityManager(mgr, mgr.EquipZone, mCurrentType);
                else if (CB_FACILITY == ComboFacility.BUILDING)
                    FormMain.Instance.DataManager.AddBuildingFacilityManager(mgr, mgr.Building, mCurrentType);
                else if (CB_FACILITY == ComboFacility.TYPE)
                    FormMain.Instance.DataManager.AddFacilityManager(mgr, mCurrentType);
            }

            row.Cells[0].Tag = mgr;
            
        }

        private void AddManager(DataCompanyMember member, FacilityManager mgr)
        {
            /*if (FindGridRow(member) != null)
				return;*/

            int nMemberID, nMemberType;

            if (mgr != null)
            {
                nMemberID = mgr.MemberID;
                nMemberType = mgr.MemberType;
            }
            else
            {
                nMemberID = member.ID;
                nMemberType = 0;
            }

            if (FindGridRow(nMemberID, nMemberType, -1, 0) != null)
                return;

            DataGridViewRow row = new DataGridViewRow();
            row.Tag = m_selectMode;

            int nIndex = gridManager.Rows.Count + 1;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nIndex.ToString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = member;
            row.Cells.Add(cell);

            DataTeam team = member.GetFirstTeam();

            cell = new DataGridViewTextBoxCell();
            cell.Value = team == null ? "" : team.TeamName;
            row.Cells.Add(cell);

            gridManager.Rows.Add(row);

            if (mgr == null && CB_FACILITY == ComboFacility.FLOOR)
            {
                // 층 정보를 받아
                string floorName = (string)m_cbFloor.customComboBox.SelectedItem.ToString().Trim();
                Floor floor = (Floor)m_cbFloor.customComboBox.SelectedItem;

                // 해당 층 이킵존을 조회
                List<int> listEquipZoneID = GetEquipZoneList(floorName);

                // 이킵존 리스트를 이용하여 담당자 추가
                foreach (int nEquipZoneID in listEquipZoneID)
                {
                    EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                    mgr = new FacilityManager();
                    mgr.MemberID = member.ID;
                    mgr.MemberType = 0;
                    mgr.Type = m_faciltyMgrGroup.Type;
                    mgr.Tag = member;
                    mgr.Building = m_faciltyMgrGroup.Building;
                    mgr.Zone = m_faciltyMgrGroup.Zone;
                    mgr.EquipZone = zone;

                    m_faciltyMgrGroup.CompanyMembers.Add(mgr);

                    if (CB_FACILITY == ComboFacility.FLOOR)
                        FormMain.Instance.DataManager.AddEquipZoneFacilityManager(mgr, mgr.EquipZone, mCurrentType);
                    else if (CB_FACILITY == ComboFacility.BUILDING)
                        FormMain.Instance.DataManager.AddBuildingFacilityManager(mgr, mgr.Building, mCurrentType);
                    else if (CB_FACILITY == ComboFacility.TYPE)
                        FormMain.Instance.DataManager.AddFacilityManager(mgr, mCurrentType);

                }
            }
            else if (mgr == null)
            {
                mgr = new FacilityManager();
                mgr.MemberID = member.ID;
                mgr.MemberType = 0;
                mgr.Type = m_faciltyMgrGroup.Type;
                mgr.Tag = member;
                mgr.Building = m_faciltyMgrGroup.Building;
                mgr.Zone = m_faciltyMgrGroup.Zone;
                mgr.EquipZone = m_faciltyMgrGroup.EquipZone;

                m_faciltyMgrGroup.CompanyMembers.Add(mgr);

                if (CB_FACILITY == ComboFacility.FLOOR)
                    FormMain.Instance.DataManager.AddEquipZoneFacilityManager(mgr, mgr.EquipZone, mCurrentType);
                else if (CB_FACILITY == ComboFacility.BUILDING)
                    FormMain.Instance.DataManager.AddBuildingFacilityManager(mgr, mgr.Building, mCurrentType);
                else if (CB_FACILITY == ComboFacility.TYPE)
                    FormMain.Instance.DataManager.AddFacilityManager(mgr, mCurrentType);
            }

            row.Cells[0].Tag = mgr;
        }

        private void AddManager(DataExternalMember member, FacilityManager mgr)
        {
            /*if (FindGridRow(member) != null)
				return;*/

            int nMemberID, nMemberType;

            if (mgr != null)
            {
                nMemberID = mgr.MemberID;
                nMemberType = mgr.MemberType;
            }
            else
            {
                nMemberID = member.ID;
                nMemberType = 2;
            }

            if (FindGridRow(nMemberID, nMemberType, -1, 0) != null)
                return;

            DataGridViewRow row = new DataGridViewRow();
            row.Tag = m_selectMode;

            int nIndex = gridManager.Rows.Count + 1;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nIndex.ToString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = member;
            row.Cells.Add(cell);

            //DataTeam team = member.GetFirstTeam();
            DataTeam team = member.Team;

            cell = new DataGridViewTextBoxCell();
            cell.Value = team == null ? "" : team.CompanyName;
            row.Cells.Add(cell);

            gridManager.Rows.Add(row);
            
            /*
            if (mgr == null && (m_cbFloor.customComboBox.SelectedItem != null && m_cbFloor.customComboBox.SelectedIndex != 0) && m_listEquipZoneID != null)
            {
                // 이킵존 리스트를 이용하여 담당자 추가
                foreach (int nEquipZoneID in m_listEquipZoneID)
                {
                    EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                    mgr = new FacilityManager();
                    mgr.MemberID = member.ID;
                    mgr.MemberType = 2;
                    mgr.Type = m_faciltyMgrGroup.Type;
                    mgr.Tag = member;
                    mgr.Building = m_faciltyMgrGroup.Building;
                    mgr.Zone = m_faciltyMgrGroup.Zone;
                    mgr.EquipZone = zone;

                    m_faciltyMgrGroup.ExternalCompanyMembers.Add(mgr);

                    if (m_cbFloor.customComboBox.SelectedItem != null && m_cbFloor.customComboBox.SelectedIndex != 0)
                        FormMain.Instance.DataManager.AddEquipZoneFacilityManager(mgr, mgr.EquipZone, mCurrentType);
                    else if (m_cbBuilding.customComboBox.SelectedItem != null && m_cbBuilding.customComboBox.SelectedIndex != 0)
                        FormMain.Instance.DataManager.AddBuildingFacilityManager(mgr, mgr.Building, mCurrentType);
                    else
                        FormMain.Instance.DataManager.AddFacilityManager(mgr, mCurrentType);
                }
            }
            else if (mgr == null)
            {
                mgr = new FacilityManager();
                mgr.MemberID = member.ID;
                mgr.MemberType = 2;
                mgr.Type = m_faciltyMgrGroup.Type;
                mgr.Tag = member;
                mgr.Building = m_faciltyMgrGroup.Building;
                mgr.Zone = m_faciltyMgrGroup.Zone;
                mgr.EquipZone = m_faciltyMgrGroup.EquipZone;

                m_faciltyMgrGroup.ExternalCompanyMembers.Add(mgr);

                if (m_cbFloor.customComboBox.SelectedItem != null && m_cbFloor.customComboBox.SelectedIndex != 0)
                    FormMain.Instance.DataManager.AddEquipZoneFacilityManager(mgr, mgr.EquipZone, mCurrentType);
                else if (m_cbBuilding.customComboBox.SelectedItem != null && m_cbBuilding.customComboBox.SelectedIndex != 0)
                    FormMain.Instance.DataManager.AddBuildingFacilityManager(mgr, mgr.Building, mCurrentType);
                else
                    FormMain.Instance.DataManager.AddFacilityManager(mgr, mCurrentType);
            }
            */


            if (mgr == null && CB_FACILITY == ComboFacility.FLOOR)
            {
                // 층 정보를 받아
                string floorName = (string)m_cbFloor.customComboBox.SelectedItem.ToString().Trim();
                Floor floor = (Floor)m_cbFloor.customComboBox.SelectedItem;

                // 해당 층 이킵존을 조회
                List<int> listEquipZoneID = GetEquipZoneList(floorName);

                foreach (int nEquipZoneID in listEquipZoneID)
                {
                    EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                    mgr = new FacilityManager();
                    mgr.MemberID = member.ID;
                    mgr.MemberType = 2;
                    mgr.Type = m_faciltyMgrGroup.Type;
                    mgr.Tag = member;
                    mgr.Building = m_faciltyMgrGroup.Building;
                    mgr.Zone = m_faciltyMgrGroup.Zone;
                    mgr.EquipZone = zone;

                    m_faciltyMgrGroup.ExternalCompanyMembers.Add(mgr);

                    if (CB_FACILITY == ComboFacility.FLOOR)
                        FormMain.Instance.DataManager.AddEquipZoneFacilityManager(mgr, mgr.EquipZone, mCurrentType);
                    else if (CB_FACILITY == ComboFacility.BUILDING)
                        FormMain.Instance.DataManager.AddBuildingFacilityManager(mgr, mgr.Building, mCurrentType);
                    else if (CB_FACILITY == ComboFacility.TYPE)
                        FormMain.Instance.DataManager.AddFacilityManager(mgr, mCurrentType);
                }
            }
            else if (mgr == null)
            {
                mgr = new FacilityManager();
                mgr.MemberID = member.ID;
                mgr.MemberType = 2;
                mgr.Type = m_faciltyMgrGroup.Type;
                mgr.Tag = member;
                mgr.Building = m_faciltyMgrGroup.Building;
                mgr.Zone = m_faciltyMgrGroup.Zone;
                mgr.EquipZone = m_faciltyMgrGroup.EquipZone;

                m_faciltyMgrGroup.ExternalCompanyMembers.Add(mgr);

                if (CB_FACILITY == ComboFacility.FLOOR)
                    FormMain.Instance.DataManager.AddEquipZoneFacilityManager(mgr, mgr.EquipZone, mCurrentType);
                else if (CB_FACILITY == ComboFacility.BUILDING)
                    FormMain.Instance.DataManager.AddBuildingFacilityManager(mgr, mgr.Building, mCurrentType);
                else if (CB_FACILITY == ComboFacility.TYPE)
                    FormMain.Instance.DataManager.AddFacilityManager(mgr, mCurrentType);
            }





            row.Cells[0].Tag = mgr;
        }

        private void GetMemberType(DataTeam team, FacilityManager mgr, out int nMemberID, out int nMemberType)
        {
            if (mgr != null)
            {
                nMemberID = mgr.MemberID;
                nMemberType = mgr.MemberType;
                return;
            }

            nMemberID = team.ID;

            /*if (team is DataTeamDuty)
			{
				nMemberType = 6;
			}
            else */
            if (team is DataTeamControlRoom)
            {
                nMemberType = 7;
            }
            else
            {
                if (team.External)
                {
                    if (team.IsCompany)
                        nMemberType = 5;
                    else
                        nMemberType = 3;
                }
                else
                {
                    if (team.IsCompany)
                        nMemberType = 4;
                    else
                        nMemberType = 1;
                }
            }
        }

        private DataGridViewRow FindGridRow(int nMemberID, int nMemberType, int nLevel, int nUpperLimit)
        {
            foreach (DataGridViewRow row in gridManager.Rows)
            {
                if (row.Cells[0].Tag == null)
                    continue;

                FacilityManager mgrSrc = (FacilityManager)row.Cells[0].Tag;

                /*
                if (mgrSrc.MemberID == nMemberID &&
                    mgrSrc.MemberType == nMemberType &&
                    mgrSrc.LevelLimit == nLevel &&
                    mgrSrc.UpperLimit == nUpperLimit)*/
                if (mgrSrc.MemberID == nMemberID &&
                    mgrSrc.MemberType == nMemberType)
                    return row;
            }

            return null;
        }

        private void FormManagerEdit_Load(object sender, EventArgs e)
        {
            InitTypeComboBox();
            InitBuildingComboBox();
            InitFloorComboBox();
            InitTree();
            //InitManagers();
        }

        private void EleTypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            gridManager.Rows.Clear();

            mCurrentType = IFacility.FacilityType.NONE;

            if (m_cbType.customComboBox.SelectedIndex == FIRE_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.FIRE_SENSOR;
            }
            else if (m_cbType.customComboBox.SelectedIndex == PSM_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.PSM_SENSOR;
            }
            else if (m_cbType.customComboBox.SelectedIndex == STRONG_WIND)
            {
                mCurrentType = IFacility.FacilityType.STRONG_WIND;
            }
            else if (m_cbType.customComboBox.SelectedIndex == EARTHQUAKE_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.Earthquake;
            }
            else if (m_cbType.customComboBox.SelectedIndex == FIREWALL_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.FIREWALL;
            }
            else if (m_cbType.customComboBox.SelectedIndex == DOOR_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.DOOR;
            }
            else if (m_cbType.customComboBox.SelectedIndex == BLACKOUT_SENSOR)
            {
                mCurrentType = IFacility.FacilityType.BLACKOUT;
            }

            // 지진은 건물 정보가 필요없음
            if (mCurrentType == IFacility.FacilityType.Earthquake)
            {
                eleBuilding.Visible = false;
                eleFloor.Visible = false;
                label3.Visible = false;
            }
            else
            {
                eleBuilding.Visible = true;
                eleFloor.Visible = true;
                label3.Visible = true;
            }

            if (m_cbFloor.customComboBox.SelectedItem != null && m_cbFloor.customComboBox.SelectedIndex != 0)
                CB_FACILITY = ComboFacility.FLOOR;
            else if (m_cbBuilding.customComboBox.SelectedItem != null && m_cbBuilding.customComboBox.SelectedIndex != 0)
                CB_FACILITY = ComboFacility.BUILDING;
            else
                CB_FACILITY = ComboFacility.TYPE;




            if (CB_FACILITY == ComboFacility.FLOOR)
            {
                List<FacilityManagerGroup> facilityManagerGroups = GetFaciltyMgrGroupList();

                foreach (FacilityManagerGroup group in facilityManagerGroups)
                {
                    m_faciltyMgrGroup = group;

                    if (m_faciltyMgrGroup != null)
                        InitManagers();
                }

                if (facilityManagerGroups.Count == 0)
                {
                    m_faciltyMgrGroup = new FacilityManagerGroup();
                    m_faciltyMgrGroup.Type = mCurrentType;
                    m_faciltyMgrGroup.Building = (Building)m_cbBuilding.customComboBox.SelectedItem;
                    Floor floor = (Floor)m_cbFloor.customComboBox.SelectedItem;
                    m_faciltyMgrGroup.Zone = floor.Zone;
                }
            }
            else
            {
                m_faciltyMgrGroup = GetFaciltyMgrGroup();

                if (m_faciltyMgrGroup != null)
                    InitManagers();
            }
        }

        private void EleBuildingComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CB_FACILITY = ComboFacility.BUILDING;
            gridManager.Rows.Clear();

            m_cbFloor.customComboBox.Items.Clear();
            m_cbFloor.customComboBox.Items.Add("모두");
            m_cbFloor.customComboBox.SelectedIndex = 0;

            if (m_cbBuilding.customComboBox.SelectedItem == null || m_cbBuilding.customComboBox.SelectedIndex == 0)
            {
                CB_FACILITY = ComboFacility.TYPE;
                m_faciltyMgrGroup = GetFaciltyMgrGroup();

                if (m_faciltyMgrGroup != null)
                    InitManagers();

                return;
            }

            

            object obj = m_cbBuilding.customComboBox.Items[m_cbBuilding.customComboBox.SelectedIndex];
            Type type = obj.GetType();

            // 층 콤보박스 채우기
            if (type == typeof(Building))
            {
                Building building = (Building)obj;
                ArrayList arrFloor = (ArrayList)building.FloorList.Clone();

                foreach (Zone floor in arrFloor)
                {
                    m_cbFloor.customComboBox.Items.Add(floor.Floor);
                }
            }

            // 빌딩 관리자 표시하기
            m_faciltyMgrGroup = GetFaciltyMgrGroup();

            if (m_faciltyMgrGroup != null)
                InitManagers();
        }

        private void EleFloorComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (m_cbFloor.customComboBox.SelectedItem == null || m_cbFloor.customComboBox.SelectedIndex == 0)
            {
                if (m_cbBuilding.customComboBox.SelectedItem == null || m_cbBuilding.customComboBox.SelectedIndex == 0)
                    CB_FACILITY = ComboFacility.TYPE;
                else
                    CB_FACILITY = ComboFacility.BUILDING;

                return;
            }

            CB_FACILITY = ComboFacility.FLOOR;
            gridManager.Rows.Clear();

            List<FacilityManagerGroup> facilityManagerGroups = GetFaciltyMgrGroupList();

            foreach (FacilityManagerGroup group in facilityManagerGroups)
            {
                m_faciltyMgrGroup = group;

                if (m_faciltyMgrGroup != null)
                    InitManagers();
            }

            if (facilityManagerGroups.Count == 0)
            {
                m_faciltyMgrGroup = new FacilityManagerGroup();
                m_faciltyMgrGroup.Type = mCurrentType;
                m_faciltyMgrGroup.Building = (Building)m_cbBuilding.customComboBox.SelectedItem;

                Floor floor = (Floor)m_cbFloor.customComboBox.SelectedItem;
                m_faciltyMgrGroup.Zone = floor.Zone;
            }

        }

        private List<int> GetEquipZoneList(string floor)
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;
            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            List<int> listEquipZoneID = new List<int>();

            string szText = "select EquipmentZone.ID from Zone inner join EquipmentZone on Zone.ID = EquipmentZone.LinkedZoneIDList where Zone.ZoneName = '{0}' AND EquipmentZone.SiteID = {1}";
            string strSQL = string.Format(szText, floor, nSiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            for (int i = 0; arrResult.Count > i; i++)
            {
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                listEquipZoneID.Add(nEquipZoneID);
            }

            return listEquipZoneID;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            OnAfterTreeSelect();
            timer1.Stop();
        }

        private void treeViewTeam_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 아직 MouseClick으로 인한 SelectedNode가 바뀌지 않았기 때문에 Timer를 이용해 약 0.1초후에 동작하도록 한다.
            timer1.Start();
        }

        private void gridMember_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewCell cell in gridMember.SelectedCells)
            {
                if (cell.RowIndex < 0)
                    continue;

                DataGridViewRow row = gridMember.Rows[cell.RowIndex];
                if (row.Tag == null)
                    continue;

                if (row.Tag.GetType() == typeof(DataCompanyMember))
                    SelectMember(true);
                else
                    SelectMember(false);

                break;
            }
        }

        private void SelectMember(bool isRegular)
        {
            m_selectMode = isRegular ? SelectMode.REGULAR_MEMBER : SelectMode.EXTERNAL_MEMBER;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;


            if (m_selectMode == SelectMode.REGULAR_TEAM)
            {
                if (treeViewTeam.SelectedNode == null || treeViewTeam.SelectedNode.Tag == null)
                    return;

                DataTeam team = (DataTeam)treeViewTeam.SelectedNode.Tag;

                AddManager(team, -1, "모든 팀원", 0, null);
            }
            else if (m_selectMode == SelectMode.REGULAR_MEMBER)
            {
                DataCompanyMember member = (DataCompanyMember)GetSelectedMember();
                if (member == null)
                    return;

                AddManager(member, null);
            }
            else if (m_selectMode == SelectMode.EXTERNAL_TEAM)
            {
                if (treeViewTeam.SelectedNode == null || treeViewTeam.SelectedNode.Tag == null)
                    return;

                DataTeam team = (DataTeam)treeViewTeam.SelectedNode.Tag;
                AddManager(team, -1, team.CompanyName, 0, null);
            }
            else if (m_selectMode == SelectMode.EXTERNAL_MEMBER)
            {
                DataExternalMember member = (DataExternalMember)GetSelectedMember();
                if (member == null)
                    return;

                AddManager(member, null);
            }
            else if (m_selectMode == SelectMode.DUTY_MEMBER)
            {
                if (treeViewTeam.SelectedNode == null || treeViewTeam.SelectedNode.Tag == null)
                    return;

                DataTeam team = (DataTeam)treeViewTeam.SelectedNode.Tag;
                AddManager(team, -1, "", 0, null);
            }
            else if (m_selectMode == SelectMode.CONTROL_ROOM)
            {
                if (treeViewTeam.SelectedNode == null || treeViewTeam.SelectedNode.Tag == null)
                    return;

                DataTeam team = (DataTeam)treeViewTeam.SelectedNode.Tag;
                string strParentTeamName = team.ParentTeam == null ? "" : team.ParentTeam.TeamName;
                AddManager(team, -1, strParentTeamName, 0, null);
            }
        }

        private object GetSelectedMember()
        {
            foreach (DataGridViewCell cell in gridMember.SelectedCells)
            {
                if (cell.RowIndex < 0)
                    continue;

                DataGridViewRow row = gridMember.Rows[cell.RowIndex];
                if (row.Tag == null)
                    continue;

                return row.Tag;
            }

            return null;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (gridManager.SelectedCells.Count == 0)
                return;

            DataGridViewCell cellSelected = gridManager.SelectedCells[0];
            int nRowIndex = cellSelected.RowIndex;

            RemoveFacilityManager(nRowIndex);
            gridManager.Rows.RemoveAt(nRowIndex);

            int nRowCount = gridManager.Rows.Count;

            for (int i = nRowIndex; i < nRowCount; i++)
            {
                DataGridViewRow row = gridManager.Rows[i];
                DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[0];
                cell.Value = (i + 1).ToString();
            }
        }

        private void RemoveFacilityManager(int nGridRowIndex)
        {
            FacilityManager mgr = (FacilityManager)gridManager.Rows[nGridRowIndex].Cells[0].Tag;

            if (mgr == null)
                return;

            if (CB_FACILITY == ComboFacility.FLOOR)
            {
                List<FacilityManagerGroup> facilityManagerGroups = GetFaciltyMgrGroupList();

                foreach (FacilityManagerGroup group in facilityManagerGroups)
                {
                    m_faciltyMgrGroup = group;
                    FacilityManager findMgr = FindFacilityManager(m_faciltyMgrGroup, mgr.MemberID);

                    if (mgr.MemberType == 0)
                        m_faciltyMgrGroup.CompanyMembers.Remove(findMgr);
                    else if (mgr.MemberType == 1)
                        m_faciltyMgrGroup.RegularTeams.Remove(findMgr);
                    else if (mgr.MemberType == 2)
                        m_faciltyMgrGroup.ExternalCompanyMembers.Remove(findMgr);
                    else if (mgr.MemberType == 3)
                        m_faciltyMgrGroup.ExternalTeams.Remove(findMgr);
                    else if (mgr.MemberType == 7)
                        m_faciltyMgrGroup.ControlRoomMembers.Remove(findMgr);
                    else
                    {
                        if (m_faciltyMgrGroup.RegularTeams.Contains(findMgr))
                            m_faciltyMgrGroup.RegularTeams.Remove(findMgr);
                        else if (m_faciltyMgrGroup.CompanyMembers.Contains(findMgr))
                            m_faciltyMgrGroup.CompanyMembers.Remove(findMgr);
                        else if (m_faciltyMgrGroup.ExternalCompanyMembers.Contains(findMgr))
                            m_faciltyMgrGroup.ExternalCompanyMembers.Remove(findMgr);
                        else if (m_faciltyMgrGroup.ExternalTeams.Contains(findMgr))
                            m_faciltyMgrGroup.ExternalTeams.Remove(findMgr);
                    }
                } 
            }
            else
            {
                m_faciltyMgrGroup = GetFaciltyMgrGroup();

                if (mgr.MemberType == 0)
                    m_faciltyMgrGroup.CompanyMembers.Remove(mgr);
                else if (mgr.MemberType == 1)
                    m_faciltyMgrGroup.RegularTeams.Remove(mgr);
                else if (mgr.MemberType == 2)
                    m_faciltyMgrGroup.ExternalCompanyMembers.Remove(mgr);
                else if (mgr.MemberType == 3)
                    m_faciltyMgrGroup.ExternalTeams.Remove(mgr);
                else if (mgr.MemberType == 7)
                    m_faciltyMgrGroup.ControlRoomMembers.Remove(mgr);
                else
                {
                    if (m_faciltyMgrGroup.RegularTeams.Contains(mgr))
                        m_faciltyMgrGroup.RegularTeams.Remove(mgr);
                    else if (m_faciltyMgrGroup.CompanyMembers.Contains(mgr))
                        m_faciltyMgrGroup.CompanyMembers.Remove(mgr);
                    else if (m_faciltyMgrGroup.ExternalCompanyMembers.Contains(mgr))
                        m_faciltyMgrGroup.ExternalCompanyMembers.Remove(mgr);
                    else
                        m_faciltyMgrGroup.ExternalTeams.Remove(mgr);
                }
            }

        }

        private FacilityManager FindFacilityManager(FacilityManagerGroup group, int memberID)
        {
            FacilityManager retMgr = null;

            if (group == null)
                return retMgr;
           
            foreach (FacilityManager mgr in group.CompanyMembers)
            {
                if (mgr.MemberID == memberID)
                    return mgr;
            }

            foreach (FacilityManager mgr in group.RegularTeams)
            {
                if (mgr.MemberID == memberID)
                    return mgr;
            }

            foreach (FacilityManager mgr in group.ExternalCompanyMembers)
            {
                if (mgr.MemberID == memberID)
                    return mgr;
            }

            foreach (FacilityManager mgr in group.ExternalTeams)
            {
                if (mgr.MemberID == memberID)
                    return mgr;
            }

            foreach (FacilityManager mgr in group.ControlRoomMembers)
            {
                if (mgr.MemberID == memberID)
                    return mgr;
            }

            return retMgr;
        }

        private FacilityManagerGroup GetFaciltyMgrGroup()
        {
            FacilityManagerGroup facilityManagerGroup = null;

            if (CB_FACILITY == ComboFacility.BUILDING)
            {
                object obj = m_cbBuilding.customComboBox.Items[m_cbBuilding.customComboBox.SelectedIndex];
                Type type = obj.GetType();

                if (type == typeof(Building))
                {
                    Building building = (Building)obj;
                    facilityManagerGroup = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(mCurrentType, building, true);

                    if (facilityManagerGroup == null)
                    {
                        facilityManagerGroup = new FacilityManagerGroup();
                        facilityManagerGroup.Type = mCurrentType;
                        facilityManagerGroup.Building = building;
                    }
                }
            }
            else if (CB_FACILITY == ComboFacility.TYPE)
            {
                facilityManagerGroup = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(mCurrentType, true);
                
                if (facilityManagerGroup == null)
                {
                    facilityManagerGroup = new FacilityManagerGroup();
                    facilityManagerGroup.Type = mCurrentType;
                }
            }

            return facilityManagerGroup;
        }

        private List<FacilityManagerGroup> GetFaciltyMgrGroupList()
        {
            List<FacilityManagerGroup> listFacilityManagerGroup = new List<FacilityManagerGroup>();

            if (CB_FACILITY == ComboFacility.FLOOR)
            {
                // 층 정보를 받아
                string floorName = (string)m_cbFloor.customComboBox.SelectedItem.ToString().Trim();
                Floor floor = (Floor)m_cbFloor.customComboBox.SelectedItem;

                // 해당 층 이킵존을 조회
                List<int> listEquipZoneID = GetEquipZoneList(floorName);

                if (listEquipZoneID != null)
                {
                    // 이킵존 리스트를 이용하여 이킵존 그룹 검색
                    foreach (int nEquipZoneID in listEquipZoneID)
                    {
                        FacilityManagerGroup facilityManagerGroup = new FacilityManagerGroup();
                        EquipmentZone equipmentZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                        facilityManagerGroup = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(mCurrentType, equipmentZone, true);

                        if (facilityManagerGroup != null)
                            listFacilityManagerGroup.Add(facilityManagerGroup);
                    }
                }
            }

            return listFacilityManagerGroup;
        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormMain.Instance.DataManager.LoadFacilityManager();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }


    }
}
