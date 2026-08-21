using System;
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
using System.Collections;

namespace KpxPipeMonitoring.Popups
{
    public partial class FormEditManager : Form
    {
        public enum SelectMode { REGULAR_TEAM = 0, REGULAR_MEMBER, EXTERNAL_TEAM, EXTERNAL_MEMBER, DUTY_MEMBER, CONTROL_ROOM, NONE };

        #region Form 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();
        #endregion

        private SelectMode m_selectMode = SelectMode.NONE;
        private FacilityManagerGroup m_faciltyMgrGroup = new FacilityManagerGroup();
        private DataManager m_dataManager = new DataManager();

        private List<FacilityManager> m_facilityManagersDB = new List<FacilityManager>();
        
        public FormEditManager()
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            SetDoubleBuffer(gridManager, true);
            SetDoubleBuffer(gridMember, true);

            InitFacilityManagers();
            InitTitle();
        }

        private void InitFacilityManagers()
        {
            foreach (KeyValuePair<FacilityType, FacilityManagerGroup> pair in m_dataManager.FacilityManagerGroups)
            {
                m_faciltyMgrGroup.CopyFrom(pair.Value);
                break;
            }
        }

        private void InitTitle()
        {
            Point ptParent = pictureBoxTitle2.Location;
            Size frmSize = this.Size;

            int xLabel = labelTitle.Location.X - pictureBoxTitle2.Location.X;
            int xClose = btnClose.Location.X - pictureBoxTitle2.Location.X;

            labelTitle.Location = new Point(xLabel, labelTitle.Location.Y);
            btnClose.Location = new Point(xClose, btnClose.Location.Y);

            labelTitle.Parent = pictureBoxTitle2;
            btnClose.Parent = pictureBoxTitle2;
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

        public static void SetDoubleBuffer(DataGridView gvView, bool bEnabled)
        {
            Type dgvType1 = gvView.GetType();
            System.Reflection.PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            pi1.SetValue(gvView, bEnabled, null);
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

                if (row.Tag is DataCompanyMember)
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            btnCancel_Click(null, null);
        }

        Image optionCloseMouseover = global::KpxPipeMonitoring.Properties.Resources.OptionClose_mouseover;
        Image optionCloseNormal = global::KpxPipeMonitoring.Properties.Resources.OptionClose_normal;
        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            this.btnClose.BackgroundImage = optionCloseMouseover;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            this.btnClose.BackgroundImage = optionCloseNormal;
        }

        private void timer1_Tick(object sender, EventArgs e)
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

            if (team.External)
                SelectExternalTeam();
            else
                SelectTeam();
            
            gridMember.Rows.Clear();

            int nIndex = 1;
            UpdateMembers(team, ref nIndex);
        }

        private void UpdateMembers(DataTeam team, ref int nIndex)
        {
            ArrayList arrMembers = m_dataManager.GetTeamMembers(team);

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

            if (objMember != null && objMember is CompanyMember)
            {
                cell.Value = ((CompanyMember)objMember).SubJobPositionName;
            }

            //cell.Value = nLevel <= 0 ? "" : nLevel.ToString() + "급";
            row.Cells.Add(cell);

            gridMember.Rows.Add(row);
        }

        private void SelectTeam()
        {
            m_selectMode = SelectMode.REGULAR_TEAM;
        }

        private void SelectExternalTeam()
        {
            m_selectMode = SelectMode.EXTERNAL_TEAM;
        }

        private void FormEditManager_Load(object sender, EventArgs e)
        {
            InitTree();
            InitGrid();
            InitManagers();

            // 읽어들인 FacilityManager 정보들을 기억해 둔다.
            SetDBManagers();
        }

        private void SetDBManagers()
        {
            foreach (DataGridViewRow row in gridManager.Rows)
            {
                if (row.Cells[0].Tag == null)
                    continue;

                if (row.Cells[0].Tag is FacilityManager)
                {
                    FacilityManager mgr = (FacilityManager)row.Cells[0].Tag;
                    m_facilityManagersDB.Add(mgr.Clone());
                }
            }
        }

        private void InitTree()
        {
            MakeTeam(treeViewTeam.Nodes, m_dataManager.RegularTeamRoot);
            MakeExternalTeams(treeViewTeam.Nodes, m_dataManager.ExternalTeamRootList);

            if (treeViewTeam.Nodes.Count > 0)
            {
                treeViewTeam.ExpandAll();
                treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];
            }

            OnAfterTreeSelect();
        }

        private void MakeExternalTeams(TreeNodeCollection nodes, ArrayList arrTeams)
        {
            foreach (DataTeam team in arrTeams)
            {
                MakeTeam(nodes, team);
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

        private void InitManagers()
        {
            foreach (FacilityManager mgr in m_faciltyMgrGroup.RegularTeams)
            {
                if (mgr.LevelLimit == 0)
                    continue;

                DataTeam team = (DataTeam)mgr.Tag;

                if (team == null)
                    continue;

                AddManager(team, mgr.LevelLimit, "", mgr.UpperLimit, mgr);
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

            if (mgr == null)
            {
                mgr = new FacilityManager();

                mgr.MemberID = nMemberID;
                mgr.MemberType = nMemberType;

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

        private void GetMemberType(DataTeam team, FacilityManager mgr, out int nMemberID, out int nMemberType)
        {
            if (mgr != null)
            {
                nMemberID = mgr.MemberID;
                nMemberType = mgr.MemberType;
                return;
            }

            nMemberID = team.ID;

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

        private void pictureBoxTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void pictureBoxTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void pictureBoxTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
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

        private void btnDelete_Click(object sender, EventArgs e)
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

        private void btnOK_Click(object sender, EventArgs e)
        {
            List<FacilityManager> newManagers = new List<FacilityManager>();

            if (IsChanged(m_facilityManagersDB, newManagers))
            {
                SaveDB(m_facilityManagersDB, newManagers);
            }

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            List<FacilityManager> oldManagers = new List<FacilityManager>();
            List<FacilityManager> newManagers = new List<FacilityManager>();

            foreach (FacilityManager mgr in m_facilityManagersDB)
            {
                oldManagers.Add(mgr.Clone());
            }

            if (IsChanged(oldManagers, newManagers))
            {
                if (UnE.Utility.UMessageBox.Show("저장하지 않은 변경사항이 있습니다.\r\n그냥 창을 닫으시겠습니까?", "확인", MessageBoxButtons.YesNo)
                    == System.Windows.Forms.DialogResult.No)
                    return;
                //if (MessageBox.Show("저장하지 않은 변경사항이 있습니다.\r\n그냥 창을 닫으시겠습니까?", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                //    return;
            }

            this.Close();
        }

        private bool SaveDB(List<FacilityManager> removeManagers, List<FacilityManager> addManagers)
        {
            string strIDs = GetIDs(removeManagers);

            if (strIDs.Length > 0)
            {
                string strSQL = "Delete from FacilityManager where ID in (" + strIDs + ")";

                if (MainForm.Instance.dbMgr.GetResultData(strSQL, 0) == null)
                    return false;
            }

            int nID = new CommonFunction().GetMaxTableID("FacilityManager") + 1;

            foreach (FacilityManager mgr in addManagers)
            {
                string strSQL = string.Format("Insert into FacilityManager (ID, MemberID, MemberType, FacilityType, SiteID) values ({0}, {1}, {2}, {3}, {4})",
                    nID++, mgr.MemberID, mgr.MemberType, (int)FacilityType.Pipe, MainForm.Instance.SiteID);

                if (MainForm.Instance.dbMgr.GetResultData(strSQL, 0) == null)
                    return false;
            }

            return true;
        }

        private string GetIDs(List<FacilityManager> managers)
        {
            string strIDs = "";

            foreach (FacilityManager mgr in managers)
            {
                if (mgr.ID > 0)
                {
                    if (strIDs.Length == 0)
                        strIDs = mgr.ID.ToString();
                    else
                        strIDs += ", " + mgr.ID.ToString();
                }
            }

            return strIDs;
        }

        private bool IsChanged(List<FacilityManager> oldManagers, List<FacilityManager> newManagers)
        {
            foreach (DataGridViewRow row in gridManager.Rows)
            {
                if (row.Cells[0].Tag == null)
                    continue;

                if (row.Cells[0].Tag is FacilityManager)
                {
                    FacilityManager mgr = (FacilityManager)row.Cells[0].Tag;
                    FacilityManager dbMgr = FindFacilityManager(mgr, oldManagers);

                    if (dbMgr != null)
                        oldManagers.Remove(dbMgr);
                    else
                        newManagers.Add(mgr);
                }
            }

            return newManagers.Count > 0 || oldManagers.Count > 0;
        }

        private FacilityManager FindFacilityManager(FacilityManager mgr, List<FacilityManager> oldManagers)
        {
            foreach (FacilityManager manager in oldManagers)
            {
                if (IsSameManager(mgr, manager))
                    return manager;
            }

            return null;
        }

        private bool IsSameManager(FacilityManager mgr1, FacilityManager mgr2)
        {
            return mgr1.MemberID == mgr2.MemberID && mgr1.MemberType == mgr2.MemberType;
        }
    }
}
