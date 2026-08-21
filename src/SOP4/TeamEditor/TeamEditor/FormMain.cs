using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;

namespace TeamEditor
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;//new WebDBManager("SOP4");
        private int m_nSOPGenUserID = -1;
        private int m_nSiteID = -1;
        private string m_strSiteName = "";
        private string m_strSOPGenUserRealName = "";
        private int m_nSplitDistance = 210;
        private bool m_initSplitDistance = false;

        private Command.CommandManagerEx m_cmdMgr = null;
        private bool m_closeApplication = false;

        //private NetworkManager m_NetWorkClient = null;

        private Popup.FormSelectTemporaryMember m_frmTemporaryMember = null;

        // 비상조직과 상시조직을 함께 보여줄 것인가?
        private bool m_useSplitContainerEmergency = false;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public bool IsEditMode
        {
            get { return rbtnEdit.IsChecked; }
        }

        public Command.CommandManagerEx CommandManager
        {
            get { return m_cmdMgr; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
        }

        public string SiteName
        {
            get { return m_strSiteName; }
        }

        public ImageList ImageListDrag
        {
            get { return imageListDrag; }
        }

        public bool CloseApplication
        {
            get { return m_closeApplication; }
            set { m_closeApplication = value; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public TeamTreeView RegularTeamTree
        {
            get { return treeRegularTeam; }
        }

        public TeamTreeView TemporaryNormalTeamTree
        {
            get { return treeNormal; }
        }

        public TeamTreeView TemporaryEmergencyTeamTree
        {
            get { return treeEmergency; }
        }

        PageBackstageOption m_pageOption = null;

        public FormMain(int nSOPGenUserID, string strSOPGenUserRealName, int nSiteID)
        {
            m_instance = this;
            m_dbMgr = new TeamEditor.WebDBManagerEx(nSiteID);

            InitializeComponent();

            treeRegularTeam.ValidateLabelEdit += new TeamTreeView.ValidateLabelEditEventHandler(TeamTreeView_ValidateLabelEdit);
            treeNormal.ValidateLabelEdit += new TeamTreeView.ValidateLabelEditEventHandler(TeamTreeView_ValidateLabelEdit);
            treeEmergency.ValidateLabelEdit += new TeamTreeView.ValidateLabelEditEventHandler(TeamTreeView_ValidateLabelEdit);
            treeExternalCompanyTeam.ValidateLabelEdit += new TeamTreeView.ValidateLabelEditEventHandler(TeamTreeView_ValidateLabelEdit);

            m_nSOPGenUserID = nSOPGenUserID;
            m_nSiteID = nSiteID;
            ReadSiteName();
            m_strSOPGenUserRealName = strSOPGenUserRealName;

            m_cmdMgr = new Command.CommandManagerEx(rbtnUndo, rbtnRedo, rbtnSave, rbtnEdit, m_dbMgr);

            gridRegularMember.LinkedTree = treeRegularTeam;
            gridExternal.LinkedTree = treeExternalCompanyTeam;

            SetMergeColumnOfTemporaryGrid();

            m_pageOption = new PageBackstageOption();
            m_pageOption.Location = new Point(0, 0);
            m_pageOption.Dock = DockStyle.Fill;
            m_pageOption.TopLevel = false;
            m_pageOption.Parent = this;
            m_pageOption.Visible = false;
            panelMain.Controls.Add(m_pageOption);
        }

        private void ReadSiteName()
        {
            string strSQL = string.Format("Select TeamName from Site, RegularTeam where Site.ID = {0} and Site.TeamID = RegularTeam.ID", m_nSiteID.ToString());
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            m_strSiteName = WebDBManager.GetStringField(arrResult[0]);

            if (m_strSiteName == null)
                m_strSiteName = "";
        }

        private void SetServerConnection()
        {
            return;

            //m_NetWorkClient = new NetworkManager(m_dbMgr, null, FormMain.Instance.SiteID);
        }

        private void SetMergeColumnOfTemporaryGrid()
        {
            gridTemporary.MergeColumns(3, 4);
            gridTemporary.MergeColumns(6, 7);
        }

        private void TeamTreeView_ValidateLabelEdit(object sender, TeamTreeView.ValidateLabelEditEventArgs e)
        {
            if (e.Label.Trim() == "")
            {
                MessageBox.Show("The tree node label cannot be empty",
                    "Label Edit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }
            if (e.Label.IndexOfAny(new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' }) != -1)
            {
                MessageBox.Show("Invalid tree node label.\n" +
                    "The tree node label must not contain following characters:\n \\ / : * ? \" < > |",
                    "Label Edit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.label2.Text = SiteName + " 조직도";

            splitContainerMain.SplitterDistance = splitContainerMain.Panel1MinSize = m_nSplitDistance;

            DataManager.InitData(m_dbMgr, m_nSiteID);
            SetPositionItems();

            treeRegularTeam.LoadData(m_dbMgr, m_nSiteID, TeamTreeView.TeamType.REGULAR);
            treeExternalCompanyTeam.LoadData(m_dbMgr, m_nSiteID, TeamTreeView.TeamType.EXTERNAL);
            treeNormal.LoadData(m_dbMgr, m_nSiteID, TeamTreeView.TeamType.TEMPORARY_NORMAL);
            treeEmergency.LoadData(m_dbMgr, m_nSiteID, TeamTreeView.TeamType.TEMPORARY_EMERGENCY);

            treeNormal.Dock = DockStyle.Fill;
            treeEmergency.Dock = DockStyle.Fill;
            gridUserDefinedTeam.Dock = DockStyle.Fill;
            panelRegular.Dock = DockStyle.Fill;
            panelExternal.Dock = DockStyle.Fill;

            gridRegularMember.SetColumnsAlignment(DataGridViewContentAlignment.MiddleCenter);
            gridTemporary.SetColumnsAlignment(DataGridViewContentAlignment.MiddleCenter);
            gridExternal.SetColumnsAlignment(DataGridViewContentAlignment.MiddleCenter);
            gridUserDefinedTeam.SetColumnsAlignment(DataGridViewContentAlignment.MiddleCenter);

            gridRegularMember.MultiSelect = true;
            gridTemporary.MultiSelect = true;
            gridExternal.MultiSelect = true;
            gridUserDefinedTeam.MultiSelect = true;

            gridRegularMember.Type = TeamGrid.GridType.RegularMember;
            // 아직 Normal인지 Emergency인지 결정되지 않았지만, ReadOnly 속성을 위하여 아무것이으로나 설정한다.
            // 정확한 타입은 툴바 메뉴버튼 클릭으로 결정된다.
            gridTemporary.Type = TeamGrid.GridType.TemporaryNormal;
            gridExternal.Type = TeamGrid.GridType.ExternalCompanyTeam;
            gridUserDefinedTeam.Type = TeamGrid.GridType.UserDefinedTeam;

            rbtnRegular_Click(null, null);
            EditMode(rbtnEdit.IsChecked);

            // 정규조직과 비상조직을 함께 화면에 나타내지 않는 옵션일 경우
            if (!m_useSplitContainerEmergency)
            {
                splitContainerEmergency.Panel1.Controls.Remove(treeNormal);
                splitContainerEmergency.Panel1.Controls.Remove(treeEmergency);
                splitContainerEmergency.Panel2.Controls.Remove(panelTemporary);

                splitContainerMain.Panel1.Controls.Add(treeNormal);
                splitContainerMain.Panel1.Controls.Add(treeEmergency);
                splitContainerMain.Panel2.Controls.Add(panelTemporary);

                treeNormal.Visible = treeEmergency.Visible = false;
            }

            SetBandsPosition();
            RememberDefaultControlColor();
            InitControlColor();
            SetServerConnection();
        }

        private void SetPositionItems()
        {
            bool init = false;

            for (int i = 0; ; i++)
            {
                string strPositionName = DataManager.GetJobPositionName(i);

                if (strPositionName == null)
                {
                    if (init)
                        break;
                }
                else
                {
                    init = true;
                    colPosition.Items.Add(strPositionName);
                }
            }

            /*init = false;

            for (int i = -1; ; i--)
            {
                string strPositionName = DataManager.GetJobPositionName(i);

                if (strPositionName == null)
                {
                    if (init)
                        break;
                }
                else
                {
                    init = true;
                    colPosition.Items.Add(strPositionName);
                }
            }*/

        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (!m_initSplitDistance && FormFrame.Instance.WindowState == FormWindowState.Maximized)
            {
                splitContainerMain.SplitterDistance = splitContainerMain.Panel1MinSize = m_nSplitDistance;
                m_initSplitDistance = true;
            }
        }

        private void SetToVisibilityForControl(TeamGrid.GridType gridType)
        {
            // 전체 컨트롤에 대해서 비활성화... Visible = false;
            treeRegularTeam.Visible =
                treeNormal.Visible =
                treeEmergency.Visible =
                treeExternalCompanyTeam.Visible = false;

            rbtnImportRegular.IsChecked = 
            rbtnUserDefined.IsChecked =
            rbtnRegular.IsChecked =
            rbtnExternal.IsChecked =
            rbtnNormal.IsChecked =
            rbtnEmergency.IsChecked = 
            rbtnOption.IsChecked = false;

            rbtnImportRegular.Refresh();
            rbtnUserDefined.Refresh();
            rbtnRegular.Refresh();
            rbtnNormal.Refresh();
            rbtnEmergency.Refresh();
            rbtnExternal.Refresh();
            rbtnOption.Refresh();

            m_pageOption.Visible =
            splitContainerMain.Visible =
            gridUserDefinedTeam.Visible =
            panelRegular.Visible =
            panelExternal.Visible =
            panelTemporary.Visible =
            splitContainerEmergency.Visible = false;

            rbtnImportRegular.Enabled = false;

            // 각 타입에 따른 활성화 컨트롤 지정
            switch (gridType)
            {
                case TeamGrid.GridType.RegularMember:

                    m_cmdMgr.ChangeCommandTarget(false);

                    // 파일불러오기버튼 비활성
                    rbtnImportRegular.Enabled = IsEditMode;

                    splitContainerMain.Visible =
                    treeRegularTeam.Visible =
                    panelRegular.Visible =
                    rbtnRegular.IsChecked = true;

                    if (treeRegularTeam.SelectedNode == null && treeRegularTeam.Nodes.Count > 0)
                        treeRegularTeam.SelectedNode = treeRegularTeam.Nodes[0];

                    break;
                case TeamGrid.GridType.TemporaryNormal:

                    m_cmdMgr.ChangeCommandTarget(false);

                    splitContainerMain.Visible = true;
                    treeNormal.Visible = true;
                    rbtnNormal.IsChecked = true;
                    gridTemporary.LinkedTree = treeNormal;
                    gridTemporary.Type = TeamGrid.GridType.TemporaryNormal;
                    panelTemporary.Visible = true;

                    if (m_useSplitContainerEmergency)
                    {
                        splitContainerEmergency.Visible = true;
                    }

                    if (treeNormal.SelectedNode == null)
                    {
                        if (treeNormal.Nodes.Count > 0)
                            treeNormal.SelectedNode = treeNormal.Nodes[0];
                    }
                    else
                    {
                        string strTeamPath = String.Empty;
                        GetTeamPath(treeNormal.SelectedNode, ref strTeamPath);
                        lblTeamPathForTemporary.Text = strTeamPath;

                        gridTemporary.SelectTeam((Team)treeNormal.SelectedNode.Tag, true);
                    }

                    break;
                case TeamGrid.GridType.TemporaryEmergency:

                    m_cmdMgr.ChangeCommandTarget(false);

                    splitContainerMain.Visible = true;
                    treeEmergency.Visible = true;
                    rbtnEmergency.IsChecked = true;
                    gridTemporary.LinkedTree = treeEmergency;
                    gridTemporary.Type = TeamGrid.GridType.TemporaryEmergency;
                    panelTemporary.Visible = true;

                    if (m_useSplitContainerEmergency)
                    {
                        splitContainerEmergency.Visible = true;
                    }

                    if (treeEmergency.SelectedNode == null)
                    {
                        if (treeEmergency.Nodes.Count > 0)
                            treeEmergency.SelectedNode = treeEmergency.Nodes[0];
                    }
                    else
                    {
                        string strTeamPath = String.Empty;
                        GetTeamPath(treeEmergency.SelectedNode, ref strTeamPath);
                        lblTeamPathForTemporary.Text = strTeamPath;

                        gridTemporary.SelectTeam((Team)treeEmergency.SelectedNode.Tag, true);
                    }

                    break;
                case TeamGrid.GridType.ExternalCompanyTeam:

                    m_cmdMgr.ChangeCommandTarget(false);

                    splitContainerMain.Visible =
                    treeExternalCompanyTeam.Visible =
                    panelExternal.Visible =
                    rbtnExternal.IsChecked = true;

                    if (treeExternalCompanyTeam.SelectedNode == null && treeExternalCompanyTeam.Nodes.Count > 0)
                    {
                        treeExternalCompanyTeam.SelectedNode = treeExternalCompanyTeam.Nodes[0];
                    }

                    break;
                case TeamGrid.GridType.UserDefinedTeam:

                    m_cmdMgr.ChangeCommandTarget(false);

                    gridUserDefinedTeam.Visible =
                    rbtnUserDefined.IsChecked = true;

                    gridUserDefinedTeam.SelectTeam(null, true);

                    break;
                case TeamGrid.GridType.None:

                    m_cmdMgr.ChangeCommandTarget(true);

                    m_pageOption.Visible = true;
                    rbtnOption.IsChecked = true;

                    break;
                default:
                    throw new Exception("타입을 알 수 없는 그리드입니다 확인하여 주세요.");

            }
        }

        public string PrintTeamPath(TreeNode node)
        {
            string strTeamPath = String.Empty;

            GetTeamPath(node, ref strTeamPath);

            return strTeamPath;
        }

        public void SelectRegularTeam(RegularTeam team)
        {
            string strTeamPath = PrintTeamPath(treeRegularTeam.SelectedNode);
            lblTeamPathForRegular.Text = strTeamPath;

            gridRegularMember.SelectTeam(team);
        }

        public void SelectTemporaryTeam(Team team, bool isNormal)
        {
            string strTeamPath = String.Empty;

            if (isNormal == false)
            {
                strTeamPath = PrintTeamPath(treeEmergency.SelectedNode);
            }
            else if (isNormal == true)
            {
                strTeamPath = PrintTeamPath(treeNormal.SelectedNode);
            }

            lblTeamPathForTemporary.Text = strTeamPath;

            gridTemporary.SelectTeam(team);
        }

        public void SelectExternalCompanyTeam(Team team)
        {
            string strTeamPath = PrintTeamPath(treeExternalCompanyTeam.SelectedNode);
            lblTeamPathForExternal.Text = strTeamPath;

            gridExternal.SelectTeam(team);
        }

        private void rbtnRegular_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.RegularMember);
            return;

            treeRegularTeam.Visible = true;
            treeNormal.Visible = treeEmergency.Visible = treeExternalCompanyTeam.Visible = false;

            rbtnRegular.IsChecked = true;
            rbtnExternal.IsChecked = rbtnNormal.IsChecked = rbtnEmergency.IsChecked = false;

            rbtnNormal.Refresh();
            rbtnEmergency.Refresh();
            rbtnExternal.Refresh();

            gridRegularMember.Visible = true;
            gridExternal.Visible = false;
            gridTemporary.Visible = false;
            splitContainerEmergency.Visible = false;

            if (treeRegularTeam.SelectedNode == null)
            {
                if (treeRegularTeam.Nodes.Count > 0)
                    treeRegularTeam.SelectedNode = treeRegularTeam.Nodes[0];
            }
        }

        private void rbtnNormal_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.TemporaryNormal);
            return;

            /*Popup.FormSelectTemporaryTeam frm = new Popup.FormSelectTemporaryTeam(true);

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (frm.SelectedTeam == null)
                    return;

                treeNormal.Nodes.Clear();

            }
            else
                return;*/

            rbtnNormal.IsChecked = true;
            rbtnExternal.IsChecked = rbtnRegular.IsChecked = rbtnEmergency.IsChecked = false;

            rbtnRegular.Refresh();
            rbtnEmergency.Refresh();
            rbtnExternal.Refresh();

            gridRegularMember.Visible = false;
            gridExternal.Visible = false;
            gridTemporary.LinkedTree = treeNormal;
            gridTemporary.Type = TeamGrid.GridType.TemporaryNormal;
            gridTemporary.Visible = true;

            if (m_useSplitContainerEmergency)
            {
                splitContainerEmergency.Visible = true;
                treeNormal.Visible = true;
                treeEmergency.Visible = false;
                treeExternalCompanyTeam.Visible = false;
            }
            else
            {
                treeRegularTeam.Visible = false;
                treeNormal.Visible = true;
                treeEmergency.Visible = false;
                treeExternalCompanyTeam.Visible = false;
            }

            if (treeNormal.SelectedNode == null)
            {
                if (treeNormal.Nodes.Count > 0)
                    treeNormal.SelectedNode = treeNormal.Nodes[0];
            }
            else
                gridTemporary.SelectTeam((Team)treeNormal.SelectedNode.Tag, true);
        }

        private void rbtnEmergency_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.TemporaryEmergency);
            return;

            rbtnEmergency.IsChecked = true;
            rbtnExternal.IsChecked = rbtnRegular.IsChecked = rbtnNormal.IsChecked = false;

            rbtnNormal.Refresh();
            rbtnRegular.Refresh();
            rbtnExternal.Refresh();

            gridRegularMember.Visible = false;
            gridExternal.Visible = false;
            gridTemporary.LinkedTree = treeEmergency;
            gridTemporary.Type = TeamGrid.GridType.TemporaryEmergency;
            gridTemporary.Visible = true;

            if (m_useSplitContainerEmergency)
            {
                splitContainerEmergency.Visible = true;
                treeNormal.Visible = false;
                treeEmergency.Visible = true;
                treeExternalCompanyTeam.Visible = false;
            }
            else
            {
                treeRegularTeam.Visible = false;
                treeNormal.Visible = false;
                treeEmergency.Visible = true;
                treeExternalCompanyTeam.Visible = false;
            }

            if (treeEmergency.SelectedNode == null)
            {
                if (treeEmergency.Nodes.Count > 0)
                    treeEmergency.SelectedNode = treeEmergency.Nodes[0];
            }
            else
                gridTemporary.SelectTeam((Team)treeEmergency.SelectedNode.Tag, true);

        }

        private void rbtnImportRegular_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = "Excel CSV File |*.csv|Excel TXT File |*.txt";

            if (openDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RegularMemberReader reader = new RegularMemberReader();
                reader.OpenFile(openDlg.FileName);
            }
            /*FormImportRegularMember pop = new FormImportRegularMember();

            if (pop.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Command.CommandImportRegularMemberInfo cmd = new Command.CommandImportRegularMemberInfo(treeRegularTeam, pop.HeaderPosition, pop.ImportData);
                m_cmdMgr.AddCommand(cmd);
                cmd.ReadImportData();
            }*/
        }

        private void rbtnOption_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.None);
        }

        private void rbtnEdit_Click(object sender, EventArgs e)
        {
            if (rbtnEdit.IsChecked)
            {
                rbtnEdit.IsChecked = false;
                rbtnEdit.Refresh();
            }
            else
                rbtnEdit.IsChecked = true;

            EditMode(rbtnEdit.IsChecked);

            treeRegularTeam.AllowDrop = treeNormal.AllowDrop = treeEmergency.AllowDrop = treeExternalCompanyTeam.AllowDrop = rbtnEdit.IsChecked;
        }

        private void EditMode(bool editable)
        {
            // 파일불러오기버튼 비활성
            rbtnImportRegular.Enabled = (editable ? rbtnRegular.IsChecked : false);

            gridRegularMember.AllowUserToAddRows = gridTemporary.AllowUserToAddRows = gridExternal.AllowUserToAddRows = gridUserDefinedTeam.AllowUserToAddRows = editable;
            gridRegularMember.ReadOnly = gridTemporary.ReadOnly = gridExternal.ReadOnly = gridUserDefinedTeam.ReadOnly = !editable;
            colMemberType.ReadOnly = colManager2.ReadOnly = colTeam.ReadOnly = true;

            if (m_frmTemporaryMember != null)
            {
                if (m_frmTemporaryMember.Visible == true)
                    m_frmTemporaryMember.Close();
            }
        }

        /*private void treeRegularTeam_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (rbtnEdit.IsChecked)
                {
                    if (treeRegularTeam.SelectedNode == e.Node)
                    {
                        treeRegularTeam.LabelEdit = true;
                        e.Node.BeginEdit();
                    }
                }
            }
        }*/

        private void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag == null || e.Label == null)
            {
                if (e.Node != null)
                    e.Node.Text = e.Label;

                e.Node.EndEdit(false);
                return;
            }

            Team team = (Team)e.Node.Tag;

            if (team == null)
            {
                e.Node.EndEdit(false);
                return;
            }

            if (team.TeamName == e.Label)
            {
                e.Node.EndEdit(false);
                return;
            }

            TeamTreeView tree = (TeamTreeView)sender;

            // 빈문자열이거나 중복된 팀 이름은 허용하지 않는다.
            if (e.Label.Length == 0 || IsSameNameNode(e.Node, e.Label, e.Node.Parent == null ? tree.Nodes : e.Node.Parent.Nodes))
            {
                e.CancelEdit = true;
                //e.Node.BeginEdit();
                return;
            }

            ChangedData<string> data = new ChangedData<string>(e.Label, team.TeamName);
            Command.CommandChangeTeamInfo info = new Command.CommandChangeTeamInfo(team, data, e.Node, tree.GetTeamType());

            info.Do();
            m_cmdMgr.AddCommand(info);
            e.Node.EndEdit(false);
            tree.LabelEdit = false;

            if (team is RegularTeam)
            {
                lblTeamPathForRegular.Text = PrintTeamPath(e.Node);
            }
            else if (team is TemporaryNormalTeam || team is TemporaryEmergencyTeam)
            {
                lblTeamPathForTemporary.Text = PrintTeamPath(e.Node);
            }
            else if (team is ExternalTeam)
            {
                lblTeamPathForExternal.Text = PrintTeamPath(e.Node);
            }

        }

        // 형제노드들 가운데 중복된 이름이 있는가?
        private bool IsSameNameNode(TreeNode node, string strNodeText, TreeNodeCollection nodes)
        {
            foreach (TreeNode child in nodes)
            {
                if (child == node)
                    continue;

                if (child.Text == strNodeText)
                    return true;
            }

            return false;
        }

        public void OnTreeViewMouseUp(TreeView tree, MouseEventArgs e)
        {
            if (tree == treeRegularTeam)
                OnRegularTreeMouseUp(e);
            else if (tree == treeNormal || tree == treeEmergency)
                OnTemporaryTreeMouseUp(tree, e);
            else if (tree == treeExternalCompanyTeam)
                OnExternalTreeMouseUp(e);
        }

        private void OnExternalTreeMouseUp(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (!IsEditMode)
                {
                    tsMenuNewExternalTeam.Visible =
                    tsMenuAddExternalCompanyTeam.Visible =
                    tsMenuRemoveExternal.Visible =
                    tsMenuRenameExternalCompanyTeam.Visible = false;
                }
                else
                {
                    TreeNode node = treeExternalCompanyTeam.GetNodeAt(e.X, e.Y);

                    tsMenuNewExternalTeam.Visible =
                    tsMenuAddExternalCompanyTeam.Visible =
                    tsMenuRemoveExternal.Visible =
                    tsMenuRenameExternalCompanyTeam.Visible = true;

                    if (node == null)
                    {
                        tsMenuAddExternalCompanyTeam.Enabled = false;
                        tsMenuRemoveExternal.Enabled = false;
                        tsMenuRenameExternalCompanyTeam.Enabled = false;
                    }
                    else
                    {
                        treeExternalCompanyTeam.SelectedNode = node;

                        tsMenuAddExternalCompanyTeam.Enabled = true;
                        tsMenuRemoveExternal.Enabled = true;
                        tsMenuRenameExternalCompanyTeam.Enabled = true;
                    }
                }

                contextMenuExternal.Show(treeExternalCompanyTeam, e.Location);
            }
        }

        private void OnTemporaryTreeMouseUp(TreeView tree, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (!IsEditMode)
                {
                    tsMenuNewGroup.Visible =
                    tsMenuAddTempTeam.Visible =
                    tsMenuDeleteTempTeam.Visible =
                    tsMenuRenameTempTeam.Visible = false;
                }
                else
                {
                    tsMenuNewGroup.Visible =
                    tsMenuAddTempTeam.Visible =
                    tsMenuDeleteTempTeam.Visible =
                    tsMenuRenameTempTeam.Visible = true;

                    TreeNode node = tree.GetNodeAt(e.X, e.Y);

                    if (node == null)
                    {
                        tsMenuAddTempTeam.Enabled = false;
                        tsMenuDeleteTempTeam.Enabled = false;
                        tsMenuRenameTempTeam.Enabled = false;
                    }
                    else
                    {
                        tree.SelectedNode = node;

                        tsMenuAddTempTeam.Enabled = true;
                        tsMenuDeleteTempTeam.Enabled = true;
                        tsMenuRenameTempTeam.Enabled = true;
                    }
                }

                contextMenuTemporaryTeam.Show(tree, e.Location);
            }
        }

        private void OnRegularTreeMouseUp(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (!IsEditMode)
                {
                    tsMenuAddTeam.Visible =
                    tsMenuDeleteTeam.Visible =
                    tsMenuRenameTeam.Visible = false;
                }
                else
                {
                    TreeNode node = treeRegularTeam.GetNodeAt(e.X, e.Y);

                    if (node != null)
                    {
                        treeRegularTeam.SelectedNode = node;

                        tsMenuAddTeam.Visible =
                        tsMenuDeleteTeam.Visible =
                        tsMenuRenameTeam.Visible = true;
                    }
                    else
                    {
                        tsMenuAddTeam.Visible =
                        tsMenuDeleteTeam.Visible =
                        tsMenuRenameTeam.Visible = false;
                    }
                }

                contextMenuRegularTeam.Show(treeRegularTeam, e.Location);
            }
        }

        private void tsMenuAddTeam_Click(object sender, EventArgs e)
        {
            if (treeRegularTeam.SelectedNode == null)
                return;

            string strTeamName = "이름없는 팀";
            SetNewTeamName(ref strTeamName, treeRegularTeam.SelectedNode.Nodes);

            int nNodeCount = treeRegularTeam.SelectedNode.Nodes.Count;
            TreeNode node = treeRegularTeam.SelectedNode.Nodes.Insert(nNodeCount, strTeamName);

            if (node != null)
            {
                treeRegularTeam.SelectedNode.ExpandAll();
                treeRegularTeam.SelectedNode = node;
                treeRegularTeam.StartLabelEdit();

                Command.CommandAddRegularTeam cmd = new Command.CommandAddRegularTeam(treeRegularTeam, node, null);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        private void tsMenuDeleteTeam_Click(object sender, EventArgs e)
        {
            DeleteRegularTeam();
        }

        private void tsMenuRenameTeam_Click(object sender, EventArgs e)
        {
            if (treeRegularTeam.SelectedNode == null)
                return;

            treeRegularTeam.StartLabelEdit();
        }

        private void treeRegularTeam_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsEditMode)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                DeleteRegularTeam();
            }
        }

        private void DeleteRegularTeam()
        {
            if (treeRegularTeam.SelectedNode == null || treeRegularTeam.SelectedNode.Tag == null)
                return;

            TreeNode node = treeRegularTeam.SelectedNode;

            RegularTeam team = (RegularTeam)node.Tag;
            //int nTeamID = (int)node.Tag;
            //RegularTeam team = DataManager.GetRegularTeam(nTeamID);

            if (team == null)
                return;

            if (treeRegularTeam.Nodes.Contains(treeRegularTeam.SelectedNode))
            {
                MessageBox.Show("최상위 팀은 삭제할 수 없습니다.");
                return;
            }

            string strMsg = "[" + team.TeamName + "]을 삭제하시겠습니까?\r\n해당팀을 포함한 하위팀과 그 팀에 소속된 직원 정보가 모두 삭제됩니다.\r\n계속 진행할까요?";

            if (MessageBox.Show(treeRegularTeam, strMsg, "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                Command.CommandRemoveRegularTeam cmd = new Command.CommandRemoveRegularTeam();
                cmd.Team = team;
                cmd.TreeNode = node;

                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        public void OnDropNode(TeamTreeView tree, TreeNode nodeSrcParent, TreeNode nodeSrc, TreeNode nodeTrg)
        {
            if (tree == treeRegularTeam)
            {
                Command.CommandMoveRegularTeam cmd = new Command.CommandMoveRegularTeam(tree, nodeSrcParent, nodeSrc, nodeTrg);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
            else if (tree == treeNormal)
            {
            }
            else if (tree == treeEmergency)
            {
            }
            else if (tree == treeExternalCompanyTeam)
            {
                if (nodeSrcParent == null)
                    return;

                Command.CommandMoveExternalTeam cmd = new Command.CommandMoveExternalTeam(tree, nodeSrcParent, nodeSrc, nodeTrg);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        public void OnDropRegularMembers(Command.CommandMoveRegularMembers cmd, TreeNode dropNode)
        {
            if (dropNode == null || dropNode.Tag == null)
                return;

            RegularTeam team = (RegularTeam)dropNode.Tag;
            //int nTeamID = (int)dropNode.Tag;
            //RegularTeam team = DataManager.GetRegularTeam(nTeamID);

            if (team == null)
                return;

            if (team == cmd.TeamOrigin)
                return;

            cmd.TeamMoved = team;
            cmd.Do();
            m_cmdMgr.AddCommand(cmd);
        }

        public void OnDropTemporaryMembers(Command.CommandMoveTemporaryMembers cmd, TreeNode dropNode)
        {
            if (dropNode == null || dropNode.Tag == null)
                return;

            Team team = (Team)dropNode.Tag;

            if (team == null)
                return;

            if (team == cmd.TeamOrigin)
                return;

            cmd.TeamMoved = team;
            cmd.Do();
            m_cmdMgr.AddCommand(cmd);
        }

        public void OnDropExternalMembers(Command.CommandMoveExternalMembers cmd, TreeNode dropNode)
        {
            if (dropNode == null || dropNode.Tag == null)
                return;

            ExternalTeam team = (ExternalTeam)dropNode.Tag;

            if (team == null)
                return;

            if (team == cmd.TeamOrigin)
                return;

            cmd.TeamMoved = team;
            cmd.Do();
            m_cmdMgr.AddCommand(cmd);
        }

        public void AddCommand(Command.CommandEx cmd, bool executeCommand = true)
        {
            if (executeCommand)
                cmd.Do();

            m_cmdMgr.AddCommand(cmd);
        }

        public void SetCurrentRegularTeam(RegularTeam team)
        {
            gridRegularMember.CurrentTeam = team;
        }

        public void SetCurrentTemporaryTeam(Team team)
        {
            gridTemporary.CurrentTeam = team;
        }

        public void SetCurrentExternalTeam(Team team)
        {
            gridExternal.CurrentTeam = team;
        }

        private void SetNewTeamName(ref string strTeamName, TreeNodeCollection nodes)
        {
            int nMax = -1;

            foreach (TreeNode node in nodes)
            {
                if (node.Text.StartsWith(strTeamName))
                {
                    int n = -1;
                    string str = node.Text.Substring(strTeamName.Length);

                    if (str.Length > 0 && int.TryParse(str, out n))
                    {
                        if (nMax < n)
                            nMax = n;
                    }
                    else if (nMax < 0)
                    {
                        nMax = 0;
                    }

                }
            }

            if (nMax > -1)
            {
                strTeamName = String.Format("{0}{1}", strTeamName, nMax + 1);
            }

        }

        private void tsMenuNewGroup_Click(object sender, EventArgs e)
        {
            TeamTreeView tree = (TeamTreeView)contextMenuTemporaryTeam.SourceControl;
            bool isNormal = tree == treeNormal;

            string strNewGroupName = "신규조직";
            SetNewTeamName(ref strNewGroupName, tree.Nodes);

            TreeNode newNode = tree.Nodes.Add(strNewGroupName);

            if (newNode != null)
            {
                tree.SelectedNode = newNode;
                tree.ExpandAll();
                tree.StartLabelEdit();

                Command.CommandAddTemporaryTeam cmd = new Command.CommandAddTemporaryTeam(tree, newNode, isNormal);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        private void tsMenuAddTempTeam_Click(object sender, EventArgs e)
        {
            TeamTreeView tree = (TeamTreeView)contextMenuTemporaryTeam.SourceControl;
            bool isNormal = tree == treeNormal;

            if (tree.SelectedNode == null)
                return;

            string strTeamName = "이름없는 팀";
            SetNewTeamName(ref strTeamName, tree.SelectedNode.Nodes);

            int nNodeCount = tree.SelectedNode.Nodes.Count;
            TreeNode node = tree.SelectedNode.Nodes.Insert(nNodeCount, strTeamName);

            if (node != null)
            {
                tree.SelectedNode.ExpandAll();
                tree.SelectedNode = node;
                tree.StartLabelEdit();

                Command.CommandAddTemporaryTeam cmd = new Command.CommandAddTemporaryTeam(tree, node, isNormal);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        private void tsMenuDeleteTempTeam_Click(object sender, EventArgs e)
        {
            TeamTreeView tree = (TeamTreeView)contextMenuTemporaryTeam.SourceControl;

            DeleteTemporaryTeam(tree);
        }

        private void tsMenuRenameTempTeam_Click(object sender, EventArgs e)
        {
            TeamTreeView tree = (TeamTreeView)contextMenuTemporaryTeam.SourceControl;

            if (tree.SelectedNode == null)
                return;

            tree.StartLabelEdit();
        }

        private void DeleteTemporaryTeam(TeamTreeView tree)
        {
            if (tree.SelectedNode == null || tree.SelectedNode.Tag == null)
                return;

            TreeNode node = tree.SelectedNode;

            Team team = (Team)node.Tag;

            if (team == null)
                return;

            string strMsg = "[" + team.TeamName + "]을 삭제하시겠습니까?\r\n해당팀을 포함한 하위팀과 그 팀에 연관된 정보가 모두 삭제됩니다.\r\n계속 진행할까요?";

            if (MessageBox.Show(tree, strMsg, "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                Command.CommandRemoveTemporaryTeam cmd = new Command.CommandRemoveTemporaryTeam(tree, node, team, tree == treeNormal);

                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_closeApplication = true;

            //if (m_NetWorkClient != null)
            //{
            //    m_NetWorkClient.ReleaseThread();
            //}

            if (m_frmTemporaryMember != null)
                m_frmTemporaryMember.Close();
        }

        public void ShowTemporaryMemberForm()
        {
            if (m_frmTemporaryMember == null)
                m_frmTemporaryMember = new Popup.FormSelectTemporaryMember(treeRegularTeam, treeNormal, treeEmergency, treeExternalCompanyTeam, gridRegularMember, gridExternal, gridUserDefinedTeam);

            if (m_frmTemporaryMember.Visible == false)
                m_frmTemporaryMember.Show(this);
        }

        public string GetLevelName(int nLevelID)
        {
            if (nLevelID < 0 || nLevelID >= colLevel.Items.Count)
                return "";

            return colLevel.Items[nLevelID].ToString();
        }

        public void SetTemporaryMember(object selectedTeam, object selectedMember)
        {
            if (gridTemporary.Visible == false)
                return;

            Command.CommandChangeTemporaryMemberInfo cmd = gridTemporary.GetTemporaryMemberChangingCommand(selectedTeam, selectedMember, Command.CommandChangeTemporaryMemberInfo.InfoType.Member);

            if (cmd != null)
            {
                m_cmdMgr.AddCommand(cmd);
                // 편집 도중에 Grid가 정렬되는 것을 막기 위하여 NoSort 속성을 true로 준다.
                gridTemporary.NoSort = true;
                cmd.Do();
                gridTemporary.NoSort = false;

                // 마지막으로 편집한 멤버의 다음 순서에 해당되는 Row를 선택
                foreach(DataGridViewRow row in  gridTemporary.Rows)
                {
                    if(row.Tag != null)
                    {
                        if (object.Equals(row.Tag, cmd.Member))
                        {
                            gridTemporary.ClearSelection();
                            gridTemporary.Rows[row.Index + 1].Selected = true;
                            break;
                        }
                    }
                }
                
            }
        }

        private void rbtnExternal_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.ExternalCompanyTeam);
            return;

            treeExternalCompanyTeam.Visible = true;
            treeNormal.Visible = treeEmergency.Visible = treeRegularTeam.Visible = false;

            rbtnExternal.IsChecked = true;
            rbtnRegular.IsChecked = rbtnNormal.IsChecked = rbtnEmergency.IsChecked = false;

            rbtnNormal.Refresh();
            rbtnEmergency.Refresh();
            rbtnRegular.Refresh();

            gridExternal.Visible = true;
            gridRegularMember.Visible = false;
            gridTemporary.Visible = false;
            splitContainerEmergency.Visible = false;

            if (treeExternalCompanyTeam.SelectedNode == null)
            {
                if (treeExternalCompanyTeam.Nodes.Count > 0)
                    treeExternalCompanyTeam.SelectedNode = treeExternalCompanyTeam.Nodes[0];
            }
        }

        private void tsMenuNewExternalTeam_Click(object sender, EventArgs e)
        {
            string strNewCompanyName = "신규 협력업체";
            SetNewTeamName(ref strNewCompanyName, treeExternalCompanyTeam.Nodes);

            TreeNode newNode = treeExternalCompanyTeam.Nodes.Add(strNewCompanyName);

            if (newNode != null)
            {
                treeExternalCompanyTeam.SelectedNode = newNode;
                treeExternalCompanyTeam.ExpandAll();
                treeExternalCompanyTeam.StartLabelEdit();

                Command.CommandAddExternalTeam cmd = new Command.CommandAddExternalTeam(newNode);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        private void tsMenuAddExternalCompanyTeam_Click(object sender, EventArgs e)
        {
            if (treeExternalCompanyTeam.SelectedNode == null)
                return;

            string strTeamName = "이름없는 팀";
            SetNewTeamName(ref strTeamName, treeExternalCompanyTeam.SelectedNode.Nodes);

            int nNodeCount = treeExternalCompanyTeam.SelectedNode.Nodes.Count;
            TreeNode node = treeExternalCompanyTeam.SelectedNode.Nodes.Insert(nNodeCount, strTeamName);

            if (node != null)
            {
                treeExternalCompanyTeam.SelectedNode.ExpandAll();
                treeExternalCompanyTeam.SelectedNode = node;
                treeExternalCompanyTeam.StartLabelEdit();

                Command.CommandAddExternalTeam cmd = new Command.CommandAddExternalTeam(node);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        private void tsMenuRenameExternalCompanyTeam_Click(object sender, EventArgs e)
        {
            if (treeExternalCompanyTeam.SelectedNode == null)
                return;

            treeExternalCompanyTeam.StartLabelEdit();
        }

        private void tsMenuRemoveExternalCompanyTeam_Click(object sender, EventArgs e)
        {
            DeleteExternalCompanyTeam();
        }

        private void treeExternalCompanyTeam_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsEditMode)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                DeleteExternalCompanyTeam();
            }
        }

        private void DeleteExternalCompanyTeam()
        {
            if (treeExternalCompanyTeam.SelectedNode == null || treeExternalCompanyTeam.SelectedNode.Tag == null)
                return;

            TreeNode node = treeExternalCompanyTeam.SelectedNode;

            Team team = (Team)node.Tag;

            if (team == null)
                return;

            string strMsg = "[" + team.TeamName + "]을 삭제하시겠습니까?\r\n해당팀을 포함한 하위팀과 그 팀에 소속된 직원 정보가 모두 삭제됩니다.\r\n계속 진행할까요?";

            if (MessageBox.Show(treeExternalCompanyTeam, strMsg, "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                Command.CommandRemoveExternalTeam cmd = new Command.CommandRemoveExternalTeam(node, team);

                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        private void rbtnUserDefined_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.UserDefinedTeam);
        }

        private void gridTemporary_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            SetBandsPosition();
        }

        private void gridTemporary_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            SetBandsPosition();
        }

        private void SetBandsPosition()
        {
            panel1.Visible =
            panel2.Visible =
            panel3.Visible = false;

            panelBand1.BringToFront();
            panelBand2.BringToFront();

            panelBand1.BackColor = gridTemporary.GridColor;
            panelBand2.BackColor = gridTemporary.GridColor;
            label1.BackColor = gridTemporary.Columns[0].HeaderCell.InheritedStyle.BackColor;
            label2.BackColor = gridTemporary.Columns[0].HeaderCell.InheritedStyle.BackColor;

            label1.Location = new Point(1, 1);
            label2.Location = new Point(1, 1);

            int nX = 0;
            int nY = 21;
            int nColumnBorderWidth = 1;

            panelBand1.Location = new Point(nX, nY);
            panelBand1.Width = gridTemporary.Columns[0].Width
                + gridTemporary.Columns[1].Width
                + gridTemporary.Columns[2].Width
                + (nColumnBorderWidth * 1);

            nX += panelBand1.Width - 1;

            panelBand2.Location = new Point(nX, nY);
            panelBand2.Width = gridTemporary.Columns[3].Width
                + gridTemporary.Columns[4].Width
                + gridTemporary.Columns[5].Width
                + gridTemporary.Columns[6].Width
                + gridTemporary.Columns[7].Width
                + gridTemporary.Columns[8].Width
                + gridTemporary.Columns[9].Width
                + (nColumnBorderWidth * 1);

            label1.Width = panelBand1.Width - 2;
            label1.Height = panelBand1.Height - 2;

            label2.Width = panelBand2.Width - 2;
            label2.Height = panelBand2.Height - 2;

        }

        private void GetTeamPath(TreeNode node, ref string strTeamPath)
        {
            if (node.Parent != null)
            {
                GetTeamPath(node.Parent, ref strTeamPath);
            }

            if (strTeamPath.Length != 0)
            {
                strTeamPath += " > ";
            }

            strTeamPath += node.Text;
        }

        public void SetServerConnection(string strIP, bool isConnected)
        {
            string strMsg = "";

            if (!isConnected)
                strMsg = String.Format("SOP Server( {0} )와의 접속 시도중...", strIP);
            else
                strMsg = String.Format("SOP Server( {0} )와의 접속 성공", strIP);

            this.Invoke((MethodInvoker)delegate
            {
                lblRegularServerState.Text =
                lblExternalServerState.Text =
                lblTemporaryServerState.Text = strMsg;
            });
        }

        private void RememberDefaultControlColor()
        {
            m_pageOption.SetDefaultColor(treeRegularTeam.BackColor, treeRegularTeam.ForeColor, gridRegularMember.DefaultCellStyle.BackColor, gridRegularMember.ForeColor);
        }

        public void InitControlColor()
        {
            if (m_pageOption.HasColorInfo() == false)
                return;

            treeRegularTeam.BackColor =
            treeNormal.BackColor =
            treeEmergency.BackColor =
            treeExternalCompanyTeam.BackColor = m_pageOption.ColorTreeBack;

            treeRegularTeam.ForeColor =
            treeNormal.ForeColor =
            treeEmergency.ForeColor =
            treeExternalCompanyTeam.ForeColor = m_pageOption.ColorTreeFont;

            treeRegularTeam.Refresh();
            treeNormal.Refresh();
            treeEmergency.Refresh();
            treeExternalCompanyTeam.Refresh();


            panelRegular.BackColor =
            panelTemporary.BackColor =
            panelExternal.BackColor =
            gridRegularMember.BackgroundColor =
            gridTemporary.BackgroundColor =
            gridExternal.BackgroundColor =
            gridUserDefinedTeam.BackgroundColor = m_pageOption.ColorGridBack;

            //gridRegularMember.GridColor=
            //gridTemporary.GridColor =
            //gridExternal.GridColor =
            //gridUserDefinedTeam.GridColor = m_pageOption.ColorGridBack;

            foreach (DataGridViewColumn column in gridTemporary.Columns)
            {
                column.HeaderCell.Style.BackColor = m_pageOption.ColorGridBack;
            }

            gridRegularMember.DefaultCellStyle.BackColor =
            gridTemporary.DefaultCellStyle.BackColor =
            gridExternal.DefaultCellStyle.BackColor =
            gridUserDefinedTeam.DefaultCellStyle.BackColor = m_pageOption.ColorGridBack;


            gridRegularMember.ForeColor =
            gridTemporary.ForeColor =
            gridExternal.ForeColor =
            gridUserDefinedTeam.ForeColor = m_pageOption.ColorGridFont;


            gridRegularMember.Refresh();
            gridTemporary.Refresh();
            gridExternal.Refresh();
            gridUserDefinedTeam.Refresh();
        }

        public void RefreshRegularMemberGrid()
        {
            gridRegularMember.RefreshGrid();
        }


    }
}
