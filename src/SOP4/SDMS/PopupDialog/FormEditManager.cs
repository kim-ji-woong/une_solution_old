using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using DBUtility;

namespace SDMS
{
	public partial class FormEditManager : Form
	{
		public enum SelectMode { REGULAR_TEAM = 0, REGULAR_MEMBER, EXTERNAL_TEAM, EXTERNAL_MEMBER, DUTY_MEMBER, CONTROL_ROOM, NONE };

		private SelectMode m_selectMode = SelectMode.NONE;
		private FacilityManagerGroup m_faciltyMgrGroup = new FacilityManagerGroup();
		private int m_nGridManagerGap = 0;

		public FormEditManager(IFacility.FacilityType type, FacilityManagerGroup group = null, Building building = null, Zone zone = null, EquipmentZone equipZone = null)
		{
            this.DoubleBuffered = true;

			InitializeComponent();

            FormMain.SetDoubleBuffer(gridManager, true);
            FormMain.SetDoubleBuffer(gridMember, true);

			if (group != null)
				m_faciltyMgrGroup.CopyFrom(group);

			m_faciltyMgrGroup.Type = type;
			m_faciltyMgrGroup.Building = building;
			m_faciltyMgrGroup.Zone = zone;
			m_faciltyMgrGroup.EquipZone = equipZone;

			m_nGridManagerGap = gridManager.Location.Y - radioLevelLow.Location.Y;
		}

		public void SetFacilityManagerGroup(FacilityManagerGroup group)
		{
			if (group != null)
				m_faciltyMgrGroup.CopyFrom(group);
		}

		public FacilityManagerGroup GetFacilityManagerGroup()
		{
			return m_faciltyMgrGroup;
		}

		private void FormEditManager_Load(object sender, EventArgs e)
		{
			InitTree();
			InitGrid();
			InitRadio();
			InitManagers();
		}

		private string GetLimitText(int nUpperLimit)
		{
			if (nUpperLimit > 0)
				return " 및 그 상위 직급만 해당";
			else if (nUpperLimit < 0)
				return " 및 그 하위 직급만 해당";

			return "만 해당";
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

		private void InitRadio()
		{
			radioLevelLow.Checked = true;
		}

		private void InitTree()
		{
			MakeTeam(treeViewTeam.Nodes, FormMain.Instance.DataManager.RegularTeamRoot);
            // 교대 근무자
            MakeControlRoomMembers(treeViewTeam.Nodes);
			MakeExternalTeams(treeViewTeam.Nodes, FormMain.Instance.DataManager.ExternalTeamRootList);
			//MakeDutyMembers(treeViewTeam.Nodes);

            if (treeViewTeam.Nodes.Count > 0)
            {
                treeViewTeam.ExpandAll();
                treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];
            }

			OnAfterTreeSelect();
		}

        private void MakeControlRoomMembers(TreeNodeCollection nodes)
        {
            DataTeamControlRoom teamRoot = FormMain.Instance.DataManager.GetRootControlRoomTeam();
            MakeTeam(nodes, teamRoot);

            /*WebDBManager dbMgr = FormMain.Instance.DBManager;
            string strSQL = "select cr.ID, cr.RoomType, cr.LocationName, crt.TypeName from ControlRoom as cr, ControlRoomType as crt ";
            strSQL += "where cr.RoomType = crt.ID and crt.SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString() + " order by cr.RoomType";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            if (nResultCount < 4)
                return;

            string strRoomTypeIDs = "";
            List<int> roomTypeList = new List<int>();
            Dictionary<int, List<DataTeam>> dicControlRooms = new Dictionary<int, List<DataTeam>>();

            DataTeamControlRoom teamParent = new DataTeamControlRoom();

            for (int i=0;i<nResultCount-3;i+=4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nRoomType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strLocationName = WebDBManager.GetStringField(arrResult[i + 2]);
                string strRoomTypeName = WebDBManager.GetStringField(arrResult[i + 3]);

                if (nID < 0 || nRoomType < 0 || strLocationName == null || strRoomTypeName == null)
                    continue;

                DataTeamControlRoom team = new DataTeamControlRoom();
                team.ParentTeam = teamParent;
                team.ID = nID;

                if (strLocationName == strRoomTypeName)
                    team.TeamName = strLocationName;
                else
                    team.TeamName = strLocationName + " " + strRoomTypeName;

                List<DataTeam> controlRooms;

                if (!dicControlRooms.TryGetValue(nRoomType, out controlRooms))
                {
                    controlRooms = new List<DataTeam>();
                    dicControlRooms[nRoomType] = controlRooms;
                }

                controlRooms.Add(team);

                if (!roomTypeList.Contains(nRoomType))
                {
                    roomTypeList.Add(nRoomType);

                    if (strRoomTypeIDs.Length == 0)
                        strRoomTypeIDs = nRoomType.ToString();
                    else
                        strRoomTypeIDs += ", " + nRoomType.ToString();
                }
            }

            if (roomTypeList.Count == 0)
                return;

            strSQL = string.Format("Select ID, JobName, RoomType from ControlTeamJobPosition where RoomType in ({0}) order by RoomType", strRoomTypeIDs);
            arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strJobName = WebDBManager.GetStringField(arrResult[i + 1]);
                int nRoomType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                if (nID < 0 || nRoomType < 0 || strJobName == null)
                    continue;

                List<DataTeam> controlRooms;

                if (!dicControlRooms.TryGetValue(nRoomType, out controlRooms))
                    continue;

                foreach (DataTeam teamControlRoom in controlRooms)
                {
                    DataTeamControlRoom team = new DataTeamControlRoom();

                    team.ID = nID;
                    team.ParentTeam = teamControlRoom;
                    team.TeamName = strJobName;
                }
            }

            MakeTeam(nodes, teamParent);*/
        }

		/*private void MakeDutyMembers(TreeNodeCollection nodes)
		{
			DataTeamDuty team = FormMain.Instance.DataManager.TeamDuty;
			MakeTeam(nodes, team);
		}*/

		private void MakeExternalTeams(TreeNodeCollection nodes, ArrayList arrTeams)
		{
			foreach (DataTeam team in arrTeams)
			{
				MakeTeam(nodes, team);
			}
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

		private void InitGrid()
		{
			colNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			colNo.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

			colName.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			colName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			colTeam.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			colTeam.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			colIndex.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			colIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

			colMember.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			colMember.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

			colLevel.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
			colLevel.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
		}

		private void UpdateMembers(DataTeam team, ref int nIndex)
		{
			ArrayList arrMembers = FormMain.Instance.DataManager.GetTeamMembers(team);

			if (team.External)
			{
				colLevel.Visible = false;

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
				colLevel.Visible = true;

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

			cell = new DataGridViewTextBoxCell();
			cell.Value = nLevel <= 0 ? "" : nLevel.ToString() + "급";
			row.Cells.Add(cell);

			gridMember.Rows.Add(row);
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

		// 당직자
		private void SelectDuty()
		{
			m_selectMode = SelectMode.DUTY_MEMBER;

			labelMode.Text = "";
			radioLevelLimit.Visible = false;
			radioLevelLow.Visible = false;
			radioLevelMiddle.Visible = false;
			radioNoLimit.Visible = false;
			textBoxLevel.Visible = false;
			textBoxLow.Visible = false;
			textBoxMiddle.Visible = false;
			labelLevel.Visible = false;
			labelLow.Visible = false;
			labelMiddle.Visible = false;

            gridManager.Location = new Point(gridManager.Location.X, radioLevelLow.Location.Y + m_nGridManagerGap);
			//gridManager.Location = new Point(gridManager.Location.X, radioLevelLimit.Location.Y + m_nGridManagerGap);
		}

		private void SelectTeam()
		{
			m_selectMode = SelectMode.REGULAR_TEAM;

			labelMode.Text = "팀 선택";
			radioLevelLimit.Visible = true;
			radioLevelLow.Visible = true;
			radioLevelMiddle.Visible = true;
			radioNoLimit.Visible = true;
			textBoxLevel.Visible = true;
			textBoxLow.Visible = true;
			textBoxMiddle.Visible = true;
			labelLevel.Visible = true;
			labelLow.Visible = true;
			labelMiddle.Visible = true;

			gridManager.Location = new Point(gridManager.Location.X, radioLevelLow.Location.Y + m_nGridManagerGap);
		}

		private void SelectMember(bool isRegular)
		{
			m_selectMode = isRegular ? SelectMode.REGULAR_MEMBER : SelectMode.EXTERNAL_MEMBER;

			labelMode.Text = "팀원 선택";
			radioLevelLimit.Visible = false;
			radioNoLimit.Visible = false;
			textBoxLevel.Visible = false;
			labelLevel.Visible = false;
			radioLevelLow.Visible = false;
			textBoxLow.Visible = false;
			labelLow.Visible = false;
			radioLevelMiddle.Visible = false;
			textBoxMiddle.Visible = false;
			labelMiddle.Visible = false;

            gridManager.Location = new Point(gridManager.Location.X, radioLevelLow.Location.Y + m_nGridManagerGap);
			//gridManager.Location = new Point(gridManager.Location.X, radioLevelLimit.Location.Y + m_nGridManagerGap);
		}

		private void SelectExternalTeam()
		{
			m_selectMode = SelectMode.EXTERNAL_TEAM;

			labelMode.Text = "협력업체 선택";
			radioLevelLimit.Visible = false;
			radioNoLimit.Visible = false;
			textBoxLevel.Visible = false;
			labelLevel.Visible = false;
			radioLevelLow.Visible = false;
			textBoxLow.Visible = false;
			labelLow.Visible = false;
			radioLevelMiddle.Visible = false;
			textBoxMiddle.Visible = false;
			labelMiddle.Visible = false;

            gridManager.Location = new Point(gridManager.Location.X, radioLevelLow.Location.Y + m_nGridManagerGap);
			//gridManager.Location = new Point(gridManager.Location.X, radioLevelLimit.Location.Y + m_nGridManagerGap);
		}

        private void SelectControlRoom()
        {
            m_selectMode = SelectMode.CONTROL_ROOM;

            labelMode.Text = "교대근무자 선택";
            radioLevelLimit.Visible = false;
            radioNoLimit.Visible = false;
            textBoxLevel.Visible = false;
            labelLevel.Visible = false;
            radioLevelLow.Visible = false;
            textBoxLow.Visible = false;
            labelLow.Visible = false;
            radioLevelMiddle.Visible = false;
            textBoxMiddle.Visible = false;
            labelMiddle.Visible = false;

            gridManager.Location = new Point(gridManager.Location.X, radioLevelLow.Location.Y + m_nGridManagerGap);
            //gridManager.Location = new Point(gridManager.Location.X, radioLevelLimit.Location.Y + m_nGridManagerGap);
        }

		private void RemoveFacilityManager(int nGridRowIndex)
		{
			FacilityManager mgr = (FacilityManager)gridManager.Rows[nGridRowIndex].Cells[0].Tag;

			if (mgr == null)
				return;

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

		private void btnRemove_Click(object sender, EventArgs e)
		{
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

		private void btnAdd_Click(object sender, EventArgs e)
		{
			if (m_selectMode == SelectMode.REGULAR_TEAM)
			{
				if (treeViewTeam.SelectedNode == null || treeViewTeam.SelectedNode.Tag == null)
					return;

				DataTeam team = (DataTeam)treeViewTeam.SelectedNode.Tag;

				if (radioLevelLimit.Checked)
				{
					if (textBoxLevel.Text.Length == 0)
					{
						MessageBox.Show("몇 급 이상으로 지정할 것인지 입력해 주세요.");
						return;
					}

					int nLevel;
					if (!int.TryParse(textBoxLevel.Text, out nLevel))
					{
						MessageBox.Show("직급 부분은 숫자로 입력되어야만 합니다.");
						return;
					}

					if (nLevel <= 0)
					{
						MessageBox.Show("직급은 0보다 큰 양의 정수값을 입력해 주세요.");
						return;
					}

					AddManager(team, nLevel, string.Format("{0}급 및 그 상위 직급만 해당", nLevel), 1, null);
				}
				else if (radioLevelLow.Checked)
				{
					if (textBoxLow.Text.Length == 0)
					{
						MessageBox.Show("몇 급 이하로 지정할 것인지 입력해 주세요.");
						return;
					}

					int nLevel;
					if (!int.TryParse(textBoxLow.Text, out nLevel))
					{
						MessageBox.Show("직급 부분은 숫자로 입력되어야만 합니다.");
						return;
					}

					if (nLevel <= 0)
					{
						MessageBox.Show("직급은 0보다 큰 양의 정수값을 입력해 주세요.");
						return;
					}

					AddManager(team, nLevel, string.Format("{0}급 및 그 하위 직급만 해당", nLevel), -1, null);
				}
				else if (radioLevelMiddle.Checked)
				{
					if (textBoxMiddle.Text.Length == 0)
					{
						MessageBox.Show("몇 급으로 지정할 것인지 입력해 주세요.");
						return;
					}

					int nLevel;
					if (!int.TryParse(textBoxMiddle.Text, out nLevel))
					{
						MessageBox.Show("직급 부분은 숫자로 입력되어야만 합니다.");
						return;
					}

					if (nLevel <= 0)
					{
						MessageBox.Show("직급은 0보다 큰 양의 정수값을 입력해 주세요.");
						return;
					}

					AddManager(team, nLevel, string.Format("{0}급만 해당", nLevel), 0, null);
				}
				else
				{
					AddManager(team, -1, "모든 팀원", 0, null);
				}
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

			if (mgr == null)
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

			if (mgr == null)
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
			}

			row.Cells[0].Tag = mgr;
		}

		private void UpdateManager(DataGridViewRow row, DataTeam team, int nLevel, string strEtc)
		{
			DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[2];
			cell.Value = strEtc;
			cell.Tag = nLevel;

			if (!team.External)
			{
				foreach (FacilityManager mgr in m_faciltyMgrGroup.RegularTeams)
				{
					if (mgr.Tag == team)
					{
						mgr.LevelLimit = nLevel;
						break;
					}
				}
			}
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
            else */if (team is DataTeamControlRoom)
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

		private void AddManager(DataTeam team, int nLevel, string strEtc, int nUpperLimit, FacilityManager mgr)
		{
			int nMemberType, nMemberID;
			GetMemberType(team, mgr, out nMemberID, out nMemberType);

			if (FindGridRow(nMemberID, nMemberType, nLevel, nUpperLimit) != null)
				return;

			//ArrayList arrOldRows = FindGridRowList(team);

			//foreach (DataGridViewRow rowOld in arrOldRows)
			//{
			//    DataGridViewTextBoxCell cell2 = (DataGridViewTextBoxCell)rowOld.Cells[2];

			//    if (cell2.Value.ToString() == strEtc && cell2.Tag != null && (int)cell2.Tag == nLevel)
			//        return;

			//    // 같은 Team에 대하여 여러 데이터가 존재할 수 있다.
			//    // 예를들어, 재난안전팀 3급과 4급을 동시에 담당자로 지정할 수 있다.
			//    /*if (MessageBox.Show(string.Format("{0} 데이터가 이미 입력되어 있습니다. 기존 데이터를 갱신하시겠습니까?", team.TeamName),
			//        "데이터 중복", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
			//        return;

			//    UpdateManager(rowOld, team, nLevel, strEtc);
			//    return;*/
			//}

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

			if (mgr == null)
			{
				/*if (nMemberType == 6)
					mgr = FormMain.Instance.DataManager.NewFacilityManagerDuty();
				else
					*/mgr = new FacilityManager();

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
			}

			row.Cells[0].Tag = mgr;
		}

		private DataGridViewRow FindGridRow(int nMemberID, int nMemberType, int nLevel, int nUpperLimit)
		{
			foreach (DataGridViewRow row in gridManager.Rows)
			{
				if (row.Cells[0].Tag == null)
					continue;

				FacilityManager mgrSrc = (FacilityManager)row.Cells[0].Tag;

				if (mgrSrc.MemberID == nMemberID &&
					mgrSrc.MemberType == nMemberType &&
					mgrSrc.LevelLimit == nLevel &&
					mgrSrc.UpperLimit == nUpperLimit)
					return row;
			}

			return null;
		}

		/*private DataGridViewRow FindGridRow(object obj)
		{
			foreach (DataGridViewRow row in gridManager.Rows)
			{
				if (row.Cells[1].Value == obj)
					return row;
			}

			return null;
		}*/

		private ArrayList FindGridRowList(object obj)
		{
			ArrayList arrResult = new ArrayList();

			foreach (DataGridViewRow row in gridManager.Rows)
			{
				if (row.Cells[1].Value == obj)
					arrResult.Add(row);
			}

			return arrResult;
		}

		private void treeViewTeam_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			// 아직 MouseClick으로 인한 SelectedNode가 바뀌지 않았기 때문에 Timer를 이용해 약 0.1초후에 동작하도록 한다.
			timer1.Start();
		}

		private void treeViewTeam_AfterCollapse(object sender, TreeViewEventArgs e)
		{
		}

		private void treeViewTeam_AfterExpand(object sender, TreeViewEventArgs e)
		{
		}

		private void OnTimer(object sender, EventArgs e)
		{
			OnAfterTreeSelect();
			timer1.Stop();
		}

		private void OnAfterTreeSelect()
		{
			TreeNode node = treeViewTeam.SelectedNode;
			if (node == null || node.Tag == null)
				return;

			DataTeam team = (DataTeam)node.Tag;

			/*if (team is DataTeamDuty)
			{
				// 당직자
				SelectDuty();
			}
            else */if (team is DataTeamControlRoom)
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

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Close();
		}

		private void rdLevelLow_CheckedChanged(object sender, EventArgs e)
		{
		}

		private void gridManager_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (gridManager.SelectedCells.Count == 0)
				return;

			int nRowIndex = gridManager.SelectedCells[0].RowIndex;
			if (nRowIndex < 0 || nRowIndex >= gridManager.Rows.Count)
				return;

			FacilityManager mgr = (FacilityManager)gridManager.Rows[nRowIndex].Cells[0].Tag;

			// RegularTeam
			if (mgr == null || mgr.MemberType != 1)
				return;

			if (mgr.LevelLimit <= 0)
				radioNoLimit.Checked = true;
			else
			{
				if (mgr.UpperLimit > 0)
					radioLevelLimit.Checked = true;
				else if (mgr.UpperLimit < 0)
					radioLevelLow.Checked = true;
				else
					radioLevelMiddle.Checked = true;
			}
		}
	}
}