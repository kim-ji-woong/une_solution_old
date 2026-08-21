using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using Sections;
using DBUtility;

namespace SOPManager.Popup
{
    public partial class PopupSelectCommander : Form
    {
        // 새로 추가되거나 변경된 것을 포함한 Data_ExternalTeam List
        // Grid Row Index, 행별 Data_ExternalTeam
        private Dictionary<int, Data_ExternalTeam> m_dicExternalTeamList = new Dictionary<int, Data_ExternalTeam>();
        // 삭제될 Data_ExternalTeam List
        private ArrayList m_arrRemoveExternalTeamList = new ArrayList();

        // 새로 추가되거나 변경된 것을 포함한 Data_UserDefinedTeam List
        // Grid Row Index, 행별 Data_UserDefinedTeam
        private Dictionary<int, Data_UserDefinedTeam> m_dicUserDefinedTeamList = new Dictionary<int, Data_UserDefinedTeam>();
        // 삭제될 Data_UserDefinedTeam List
        private ArrayList m_arrRemoveUserDefinedTeamList = new ArrayList();

        private SOPTeam m_teamSelected = null;
        private string m_strDisplayText = "";
        private SOPTeam.SOPTeamType m_currentTeamType = SOPTeam.SOPTeamType.None;
        // 이 값이 true이면 SOP 제어권을 가진곳의 책임자가 발신자가 된다.
        private bool m_isDefaultOption = false;

        private Sections.SectionCommander m_initCommander = null;

        // 이 값이 true이면 SOP 제어권을 가진곳의 책임자가 발신자가 된다.
        public bool DefaultOption
        {
            get { return m_isDefaultOption; }
            set { m_isDefaultOption = value; }
        }

        public SOPTeam SelectedTeam
        {
            get { return m_teamSelected; }
            set { m_teamSelected = value; }
        }

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public SOPTeam.SOPTeamType CurrentTeamType
        {
            get { return m_currentTeamType; }
            set { m_currentTeamType = value; }
        }

        public PopupSelectCommander(SectionCommander commander)
        {
            InitializeComponent();
            dataGridViewUserDefined.CellPainting += dataGridView_CellPainting;
            dataGridViewExternal.CellPainting += dataGridView_CellPainting;
            m_initCommander = commander;

            rbBtnRegular.Tag = SOPTeam.SOPTeamType.Regular;
            if (SopDocManager.Instance.WeekMode)
            {
                rbBtnEmergency.Text = "평일 비상 조직";
                rbBtnEmergency.Tag = SOPTeam.SOPTeamType.Normal;
            }
            else
            {
                rbBtnEmergency.Text = "야간 및 휴일 비상 조직";
                rbBtnEmergency.Tag = SOPTeam.SOPTeamType.Holiday;
            }
            rbBtnExternal.Tag = SOPTeam.SOPTeamType.External;
            rbBtnUserDefine.Tag = SOPTeam.SOPTeamType.UserDefined;
            rbBtnControlRoom.Tag = SOPTeam.SOPTeamType.ControlRoom;

            UpdateRadioButtos();
            UpdateControlSize();
        }

        void dataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView gdv = sender as DataGridView;
            if (gdv == null) return;

            foreach (DataGridViewRow row in gdv.Rows)
                row.MinimumHeight = gdv.RowTemplate.Height;
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            FormMain.Instance.UpdateWindowRate(btnOK, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnCancel, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(flowLayoutPanel1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picRegular, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblRegular, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picEmergency, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblEmergency, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picExternal, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblExternal, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picUserDefine, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblUserDefine, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picControlRoom, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblControlRoom, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(labelTeamType, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picDefault, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblDefault, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(labelFullPath, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(textBoxDisplay, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(dataGridViewUserDefined, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(dataGridViewExternal, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(treeViewTeam, WindowRateWidth, WindowRateHeight);
        }

        private void PopupSelectTeam4_Load(object sender, EventArgs e)
        {
            if (m_initCommander == null || m_initCommander.Team == null)
                m_currentTeamType = SOPTeam.SOPTeamType.Normal;
            else
                m_currentTeamType = m_initCommander.Team.TeamType;
             
            SetTeamTypeLabel(m_currentTeamType);
            InitTree();

            //InitExternalGrid();

            InitUserDefinedGrid();

            if (m_initCommander != null)
            {
                if (m_initCommander.Team == null)
                    checkBoxDefault.Checked = true;
                else
                {
                    if (m_currentTeamType == SOPTeam.SOPTeamType.Regular ||
                        m_currentTeamType == SOPTeam.SOPTeamType.Normal ||
                        m_currentTeamType == SOPTeam.SOPTeamType.Holiday)
                    { 
                        if(m_currentTeamType == SOPTeam.SOPTeamType.Regular)
                            rbBtnRegular.Checked = true;

                        if (m_currentTeamType == SOPTeam.SOPTeamType.Normal || m_currentTeamType == SOPTeam.SOPTeamType.Holiday)
                            rbBtnEmergency.Checked = true;

                        TreeNode node = FindNode(m_initCommander.Team.TeamID);
                        treeViewTeam.SelectedNode = node;
                        treeViewTeam.Select(); 
                    }
                    else if (m_currentTeamType == SOPTeam.SOPTeamType.External)
                    {
                        rbBtnExternal.Checked = true;
                        TreeNode node = FindNode(m_initCommander.Team.TeamID);
                        treeViewTeam.SelectedNode = node;
                        treeViewTeam.Select();
                    }
                    else if (m_currentTeamType == SOPTeam.SOPTeamType.UserDefined)
                    { 
                        rbBtnUserDefine.Checked = true;
                        FindGridRowUserDefined(m_initCommander.Team.TeamID); 
                    }
                    else if (m_currentTeamType == SOPTeam.SOPTeamType.ControlRoom)
                    {
                        rbBtnControlRoom.Checked = true;
                        TreeNode node = FindNode(m_initCommander.Team.TeamID);
                        treeViewTeam.SelectedNode = node;
                        treeViewTeam.Select(); 
                    }
                }
                textBoxDisplay.Text = m_initCommander.DisplayText;
            }
            UpdateRadioButtos();
        }

        private void UpdateRadioButtos()
        {
            if (rbBtnRegular.Checked == true)            
                picRegular.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            else
                picRegular.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;

            lblRegular.Text = rbBtnRegular.Text;

            if (rbBtnEmergency.Checked == true)
                picEmergency.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            else
                picEmergency.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;

            lblEmergency.Text = rbBtnEmergency.Text;

            if (rbBtnExternal.Checked == true)
                picExternal.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            else
                picExternal.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;

            lblExternal.Text = rbBtnExternal.Text;

            if (rbBtnUserDefine.Checked == true)
                picUserDefine.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            else
                picUserDefine.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;

            lblUserDefine.Text = rbBtnUserDefine.Text;

            if (rbBtnControlRoom.Checked == true)
                picControlRoom.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            else
                picControlRoom.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;

            lblControlRoom.Text = rbBtnControlRoom.Text;
        }

        public void FindGridRowUserDefined(int TeamID)
        {
            dataGridViewUserDefined.ClearSelection();
            foreach(DataGridViewRow row in dataGridViewUserDefined.Rows)
            {
                Data_UserDefinedTeam team = (Data_UserDefinedTeam)row.Tag;
                if( team.ID == TeamID)
                {
                    row.Selected = true;
                    break;
                }
            }
        }

        public void FindGridRowExternal(int TeamID)
        {
            dataGridViewExternal.ClearSelection();
            foreach (DataGridViewRow row in dataGridViewExternal.Rows)
            {
                Data_ExternalTeam team = (Data_ExternalTeam)row.Tag;
                if (team.ID == TeamID)
                {
                    row.Selected = true;
                    break;
                }
            }
        }

        public void InitTree(Sections.SOPTeam.SOPTeamType teamType = Sections.SOPTeam.SOPTeamType.None)
        {
            treeViewTeam.Nodes.Clear();

            Sections.SOPTeam.SOPTeamType nTeamType = m_currentTeamType;

            if (teamType != Sections.SOPTeam.SOPTeamType.None)
                nTeamType = teamType;

            SetTeamTypeLabel(nTeamType);

            if (nTeamType == Sections.SOPTeam.SOPTeamType.External)        // 외부 조직
                LoadExternalTeamTree();
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.UserDefined)// 사용자 정의 조직
                LoadUserDefinedTeamTree();
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.Normal)     // 평일 비상 조직
                LoadTemporaryNormalTeamTree();
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.Holiday)    // 야간 및 휴일 비상 조직
                LoadTemporaryEmergencyTeamTree();
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.ControlRoom)// 교대 근무자
                LoadControlRoomTree();
            else// if (nTeamType == Sections.SOPTeam.SOPTeamType.Regular)  // 정규 조직
                LoadRegularTeamTree();
        }

        private void LoadExternalTeamTree()
        {
            ArrayList arrExternalTeam = FormMain.Instance.ExternalTeam;

            foreach (Data_ExternalTeam data in arrExternalTeam)
            {
                if (data.ParentTeam == null)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;

                    AddExternalCompanySubTeamTree(data, node);
                }
            }
            treeViewTeam.ExpandAll();
        }
        
        private void AddExternalCompanySubTeamTree(Data_ExternalTeam teamParent, TreeNode nodeParent)
        {
            foreach (Data_ExternalTeam team in FormMain.Instance.ExternalTeam)
            {
                if (team.ParentTeam != null && team.ParentTeam.ID == teamParent.ID)
                {
                    TreeNode node = nodeParent.Nodes.Add(team.TeamName);
                    node.Tag = team.ID;

                    AddExternalCompanySubTeamTree(team, node);
                }
            }
        }

        private void LoadUserDefinedTeamTree()
        {
            ArrayList arrUserDefinedTeam = FormMain.Instance.UserDefinedTeam;

            foreach (Data_UserDefinedTeam data in arrUserDefinedTeam)
            {
                TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                node.Tag = data.ID;
                treeViewTeam.ExpandAll();
            }
        }

        private void LoadTemporaryNormalTeamTree()
        {
            ArrayList arrRegularTeam = FormMain.Instance.TemporaryNormalTeam;

            foreach (Data_NormalTeam data in arrRegularTeam)
            {
                if (data.ParentTeamID <= 0)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                    if (child == null) return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data.ID;
                }
            }

            treeViewTeam.ExpandAll();
        }

        private void LoadTemporaryEmergencyTeamTree()
        {
            ArrayList arrEmergencyTeam = FormMain.Instance.TemporaryEmergencyTeam;

            foreach (Data_EmergencyTeam data in arrEmergencyTeam)
            {
                if (data.ParentTeamID <= 0)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                    if (child == null) return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data.ID;
                }
            }

            treeViewTeam.ExpandAll();
        }

        private void LoadRegularTeamTree()
        {
            ArrayList arrRegularTeam = FormMain.Instance.RegularTeam;

            foreach (Data_RegularTeam data in arrRegularTeam)
            {
                if (data.ParentTeamID == 0)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                    if (child == null) return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data.ID;
                }
            }

            treeViewTeam.ExpandAll();
        }

        private void LoadControlRoomTree()
        {
            TreeNode rootNode = treeViewTeam.Nodes.Add("교대 근무자");
            rootNode.Tag = 0;

            ArrayList arrControlRoom = FormMain.Instance.ControlRoom;
             
            foreach (Data_ControlRoom data in arrControlRoom)
            {
                if (data.ParentTeam == null) continue; 

                TreeNode child = FindNode(data.ParentTeam.ID, treeViewTeam.Nodes);
                if (child == null) return;

                TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                newNode.Tag = data.ID; 
            }

            treeViewTeam.ExpandAll();
        }

        private TreeNode FindNode(int nTag, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewTeam.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if ((int)node.Tag == nTag)
                    return node;

                TreeNode result = FindNode(nTag, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewTeam.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if (strValue == node.Text)
                    return node;
                TreeNode result = FindNode(strValue, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void SetTeamTypeLabel(Sections.SOPTeam.SOPTeamType nTeamType)
        {
            if (nTeamType == Sections.SOPTeam.SOPTeamType.Normal)
                labelTeamType.Text = "평일 비상 조직";
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.Holiday)
                labelTeamType.Text = "야간 및 휴일 비상 조직";
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.External)
                labelTeamType.Text = "외부 기관";
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.UserDefined)
                labelTeamType.Text = "사용자 정의 조직";
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.Regular)
                labelTeamType.Text = "정규 조직";
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.ControlRoom)
                labelTeamType.Text = "교대근무자";
        }

        private void btnSelectTeam_Click(object sender, EventArgs e)
        {
            PopupSelectTeam2 frm = new PopupSelectTeam2(m_currentTeamType);
            UnE.GUI.DialogFormFrameRibbon frame = new UnE.GUI.DialogFormFrameRibbon(frm);
            frame.StartPosition = FormStartPosition.CenterScreen;
            if (frame.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                m_currentTeamType = frm.SelectedTeamType;

                SetTeamTypeLabel(m_currentTeamType);
                InitTree(m_currentTeamType);
            }
        }

        private void treeViewTeam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // 협력업체 회사명을 클릭하였다.

            // 협력업체 회사명도 선택할 수 있도록 변경함. skkim 2015-08-03
            //if (m_currentTeamType == SOPTeam.SOPTeamType.External && e.Node.Parent == null)
           //     return;

            checkBoxDefault.Checked = false;
            labelFullPath.Visible = true;
            labelFullPath.Text = e.Node.FullPath;

            treeViewTeam.SelectedNode = e.Node;

            if (m_currentTeamType == SOPTeam.SOPTeamType.Regular)
            {
                if (e.Node.Text.EndsWith("장"))
                    textBoxDisplay.Text = e.Node.Text;
                else
                    textBoxDisplay.Text = e.Node.Text + "장";
            }
            else
                textBoxDisplay.Text = e.Node.Text;
        }

        private void checkBoxDefault_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxDefault.Checked)
            {
                textBoxDisplay.Text = checkBoxDefault.Text;
                labelFullPath.Visible = false;
                picDefault.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_enable;
            }
            else
            {
                textBoxDisplay.Text = "";
                treeViewTeam.SelectedNode = null;
                picDefault.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_disable;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (checkBoxDefault.Checked)
                m_isDefaultOption = true;
            else
            {
                if ((treeViewTeam.SelectedNode == null || 
                    treeViewTeam.SelectedNode.Tag == null || 
                    (treeViewTeam.SelectedNode.Tag is int) == false)&&
                    mSelectedRow == null)
                {
                    MessageBox.Show("발신자를 선택하여 주세요");
                    return;
                }
                else
                {
                    if (m_currentTeamType == SOPTeam.SOPTeamType.Normal || m_currentTeamType == SOPTeam.SOPTeamType.Holiday)
                    {
                        int nTeamID = (int)treeViewTeam.SelectedNode.Tag;
                        foreach (Data_TemporaryTeam team in FormMain.Instance.TemporaryNormalTeam)
                        {
                            if (team.ID == nTeamID)
                            {
                                m_teamSelected = new SOPTeam();
                                m_teamSelected.TeamID = team.ID;
                                m_teamSelected.TeamType = m_currentTeamType;
                                m_teamSelected.TeamName = team.TeamName;

                                break;
                            }
                        }
                    }
                    else if (m_currentTeamType == SOPTeam.SOPTeamType.External)
                    {
                        int nTeamID = (int)treeViewTeam.SelectedNode.Tag;
                        foreach (Data_ExternalTeam team in FormMain.Instance.ExternalTeam)
                        {
                            if (team.ID == nTeamID)
                            {
                                m_teamSelected = new SOPTeam();
                                m_teamSelected.TeamID = team.ID;
                                m_teamSelected.TeamType = m_currentTeamType;
                                m_teamSelected.TeamName = team.TeamName;
                               
                                break;
                            }
                        }
                    }
                    else if (m_currentTeamType == SOPTeam.SOPTeamType.UserDefined)
                    {
                        Data_UserDefinedTeam teamSelected = (Data_UserDefinedTeam)mSelectedRow.Tag;
                        foreach (Data_UserDefinedTeam team in FormMain.Instance.UserDefinedTeam)
                        {
                            if (team.ID == teamSelected.ID)
                            {
                                m_teamSelected = new SOPTeam();
                                m_teamSelected.TeamID = team.ID;
                                m_teamSelected.TeamType = m_currentTeamType;
                                m_teamSelected.TeamName = team.TeamName;

                                break;
                            }
                        }
                    }
                    else if (m_currentTeamType == SOPTeam.SOPTeamType.Regular)
                    {
                        int nTeamID = (int)treeViewTeam.SelectedNode.Tag;
                        foreach (Data_RegularTeam team in FormMain.Instance.RegularTeam)
                        {
                            if (team.ID == nTeamID)
                            {
                                m_teamSelected = new SOPTeam();
                                m_teamSelected.TeamID = team.ID;
                                m_teamSelected.TeamType = m_currentTeamType;
                                m_teamSelected.TeamName = team.TeamName;

                                break;
                            }
                        }
                    }
                    else if (m_currentTeamType == SOPTeam.SOPTeamType.ControlRoom)
                    {
                        int nTeamID = (int)treeViewTeam.SelectedNode.Tag;
                        foreach (Data_ControlRoom team in FormMain.Instance.ControlRoom)
                        {
                            if (team.ID == nTeamID)
                            {
                                m_teamSelected = new SOPTeam();
                                m_teamSelected.TeamID = team.ID;
                                m_teamSelected.TeamType = m_currentTeamType;
                                m_teamSelected.TeamName = team.TeamName;

                                break;
                            }
                        }
                    }
                }
            }

            //SaveExternalList();

            SaveUserDefinedList();

            m_strDisplayText = textBoxDisplay.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void radioTeam_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (btn == null)
                return;

            if (btn.Checked == false)
                return;

            SOPTeam.SOPTeamType nTeamType = (SOPTeam.SOPTeamType)btn.Tag;
            m_currentTeamType = nTeamType;

            if (nTeamType == SOPTeam.SOPTeamType.Normal || nTeamType == SOPTeam.SOPTeamType.Holiday)
            {
                if (nTeamType == SOPTeam.SOPTeamType.Normal)
                {
                    labelTeamType.Text = "평일 비상 조직";
                }
                else
                    labelTeamType.Text = "휴일 비상 조직";

                InitTree(nTeamType);

                treeViewTeam.Visible = true;

                dataGridViewExternal.Visible = false;
                dataGridViewUserDefined.Visible = false;

                if (treeViewTeam.Nodes.Count > 0)
                    treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];
            }
            else if (nTeamType == SOPTeam.SOPTeamType.External)
            {
                labelTeamType.Text = "외부 기관";

                InitTree(nTeamType);
                treeViewTeam.Visible = true;

                dataGridViewExternal.Visible = false;
                dataGridViewUserDefined.Visible = false;

                if (treeViewTeam.Nodes.Count > 0)
                    treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];
            }
            else if (nTeamType == SOPTeam.SOPTeamType.UserDefined)
            {
                labelTeamType.Text = "사용자정의조직";

                treeViewTeam.Visible = false;

                dataGridViewExternal.Visible = false;
                dataGridViewUserDefined.Visible = true;

            }
            else if (nTeamType == SOPTeam.SOPTeamType.Regular)
            {
                labelTeamType.Text = "정규조직";

                InitTree(nTeamType);

                treeViewTeam.Visible = true;

                if (treeViewTeam.Nodes.Count > 0)
                    treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];

                dataGridViewExternal.Visible = false;
                dataGridViewUserDefined.Visible = false;
            }
            else if (nTeamType == SOPTeam.SOPTeamType.ControlRoom)
            {
                labelTeamType.Text = "교대 근무자";

                InitTree(nTeamType);
                treeViewTeam.Visible = true;

                dataGridViewExternal.Visible = false;
                dataGridViewUserDefined.Visible = false;

                if (treeViewTeam.Nodes.Count > 0)
                    treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];
            }

            UpdateRadioButtos();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void InitUserDefinedGrid()
        {
            m_dicUserDefinedTeamList.Clear();
            dataGridViewUserDefined.ClearSelection();
            dataGridViewUserDefined.Rows.Clear();

            ArrayList arrUserDefinedTeam = FormMain.Instance.UserDefinedTeam;
            foreach (Data_UserDefinedTeam data in arrUserDefinedTeam)
            {
                AllUserDefinedTeam(data);
            }
        }

        private void AllUserDefinedTeam(Data_UserDefinedTeam data)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            Data_UserDefinedTeam team = new Data_UserDefinedTeam();

            cell.Value = data.TeamName;
            gridRow.Cells.Add(cell);
            team.TeamName = data.TeamName;

            cell = new DataGridViewTextBoxCell();
            cell.Value = data.PhoneNumber == null ? "" : data.PhoneNumber;
            gridRow.Cells.Add(cell);
            team.PhoneNumber = data.PhoneNumber;

            cell = new DataGridViewTextBoxCell();
            cell.Value = data.FaxNumber == null ? "" : data.FaxNumber;
            gridRow.Cells.Add(cell);
            team.FaxNumber = data.FaxNumber;

            gridRow.Tag = data;
            team.ID = data.ID;

            if (dataGridViewUserDefined.AllowUserToAddRows)
                m_dicUserDefinedTeamList[dataGridViewUserDefined.Rows.Count - 1] = team;
            else
                m_dicUserDefinedTeamList[dataGridViewUserDefined.Rows.Count] = team;

            dataGridViewUserDefined.Rows.Add(gridRow);
        }

        private void InitExternalGrid()
        {
            m_dicExternalTeamList.Clear();
            dataGridViewExternal.ClearSelection();
            dataGridViewExternal.Rows.Clear();

            ArrayList arrExternalTeam = FormMain.Instance.ExternalTeam;
            foreach (Data_ExternalTeam data in arrExternalTeam)
            {
                AllExternalTeam(data);
            }
        }

        private void AllExternalTeam(Data_ExternalTeam data)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            Data_ExternalTeam team = new Data_ExternalTeam();

            cell.Value = data.TeamName;
            gridRow.Cells.Add(cell);
            team.TeamName = data.TeamName;

            cell = new DataGridViewTextBoxCell();
            cell.Value = data.PhoneNumber == null ? "" : data.PhoneNumber;
            gridRow.Cells.Add(cell);
            team.PhoneNumber = data.PhoneNumber;

            cell = new DataGridViewTextBoxCell();
            cell.Value = data.FaxNumber == null ? "" : data.FaxNumber;
            gridRow.Cells.Add(cell);
            team.FaxNumber = data.FaxNumber;

            gridRow.Tag = data;
            team.ID = data.ID;

            if (dataGridViewExternal.AllowUserToAddRows)
                m_dicExternalTeamList[dataGridViewExternal.Rows.Count - 1] = team;
            else
                m_dicExternalTeamList[dataGridViewExternal.Rows.Count] = team;

            dataGridViewExternal.Rows.Add(gridRow);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 사용자 정의팀 , 외부조직 변경사항 체크 및 저장
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        // 기존에 존재하던 외부팀 데이터인가 여부.
        // 만일 기존에 존재하던 팀이라면 데이터가 바뀌었는지 여부
        // Return 값 : 0(기존에 존재하던 팀이며 아무것도 바뀌지 않음)
        //             1(기존에 존재하던 팀이며, 데이터가 바뀌었음)
        //            -1(새로운 팀)
        //            -1(잘못된 데이터)
        private int CheckExternalTeam(Data_ExternalTeam team)
        {
            if (team.TeamName.Length == 0)
                return -2;


            ArrayList arExternalTeam = FormMain.Instance.ExternalTeam;
            foreach (Data_ExternalTeam data in arExternalTeam)
            {
                if (data.TeamName == team.TeamName)
                {
                    team.ID = data.ID;

                    if (team.PhoneNumber.Length == 0)
                        return -2;

                    if (team.PhoneNumber == data.PhoneNumber &&
                        team.FaxNumber == data.FaxNumber)
                        return 0;
                    else
                        return 1;
                }
            }

            team.ID = -1;
            return -1;
        }

        private int FindExternalTeam(int nTeamID, ArrayList arrTeamList)
        {
            int nTeamCount = arrTeamList.Count;

            for (int i = 0; i < nTeamCount; i++)
            {
                Data_ExternalTeam team = (Data_ExternalTeam)arrTeamList[i];
                if (team.ID == nTeamID)
                    return i;
            }

            return -1;
        }

        private int FindUserDefinedTeam(int nTeamID, ArrayList arrTeamList)
        {
            int nTeamCount = arrTeamList.Count;

            for (int i = 0; i < nTeamCount; i++)
            {
                Data_UserDefinedTeam team = (Data_UserDefinedTeam)arrTeamList[i];
                if (team.ID == nTeamID)
                    return i;
            }

            return -1;
        }

        private void SaveExternalList()
        {
            ArrayList arrNewTeam = new ArrayList();
            ArrayList arrUpdateTeam = new ArrayList();

            foreach (KeyValuePair<int, Data_ExternalTeam> pair in m_dicExternalTeamList)
            {
                int nResult = CheckExternalTeam(pair.Value);

                if (nResult == 1)
                    arrUpdateTeam.Add(pair.Value);
                else if (nResult == -1)
                    arrNewTeam.Add(pair.Value);
            }

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strRemoveIDs = "", strSQL;

            ////////////////////////////////////////////////////////////////////
            // 데이터 삭제
            foreach (Data_ExternalTeam team in m_arrRemoveExternalTeamList)
            {
                if (strRemoveIDs.Length == 0)
                    strRemoveIDs = team.ID.ToString();
                else
                    strRemoveIDs += ", " + team.ID.ToString();

                int nIndex = FindExternalTeam(team.ID, FormMain.Instance.ExternalTeam);
                if (nIndex >= 0)
                    FormMain.Instance.ExternalTeam.RemoveAt(nIndex);
            }

            if (strRemoveIDs.Length > 0)
            {
                if (IOManager.DeleteActionStepUsingTeam(dbMgr, strRemoveIDs, 2) == false)
                    return;

                strSQL = string.Format("Delete from ExternalTeam where id in ({0})", strRemoveIDs);
                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }

            m_arrRemoveExternalTeamList.Clear();
            ////////////////////////////////////////////////////////////////////

            strSQL = "select max(id) from ExternalTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            int nTeamID;

            if (arrResult == null || arrResult.Count == 0)
                nTeamID = 0;
            else
                nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            foreach (Data_ExternalTeam team in arrNewTeam)
            {
                string strFaxNumber = team.FaxNumber == null || team.FaxNumber.Length == 0 ? "NULL" : "'" + team.FaxNumber + "'";
                string strPhoneNumber = team.PhoneNumber == null || team.PhoneNumber.Length == 0 ? "0000000" : team.PhoneNumber;

                strSQL = string.Format("Insert into ExternalTeam (ID, TeamName, PhoneNumber, FaxNumber, SiteID) values ({0}, '{1}', '{2}', {3}, {4})",
                    ++nTeamID, team.TeamName, strPhoneNumber, strFaxNumber, FormMain.Instance.SiteID);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                team.ID = nTeamID;
                FormMain.Instance.ExternalTeam.Add(team);
            }

            foreach (Data_ExternalTeam team in arrUpdateTeam)
            {
                string strFaxNumber = team.FaxNumber == null || team.FaxNumber.Length == 0 ? "NULL" : "'" + team.FaxNumber + "'";
                string strPhoneNumber = team.PhoneNumber == null || team.PhoneNumber.Length == 0 ? "0000000" : team.PhoneNumber;

                strSQL = string.Format("Update ExternalTeam set PhoneNumber = '{0}', FaxNumber = {1} where id = {2}",
                    strPhoneNumber, strFaxNumber, team.ID);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                int nIndex = FindExternalTeam(team.ID, FormMain.Instance.ExternalTeam);
                if (nIndex >= 0)
                {
                    Data_ExternalTeam _team = (Data_ExternalTeam)FormMain.Instance.ExternalTeam[nIndex];
                    _team.FaxNumber = team.FaxNumber;
                    _team.PhoneNumber = team.PhoneNumber;
                    _team.TeamName = team.TeamName;
                }
            }
        }

        // 기존에 존재하던 사용자 정의 조직 데이터인가 여부.
        // 만일 기존에 존재하던 팀이라면 데이터가 바뀌었는지 여부
        // Return 값 : 0(기존에 존재하던 팀이며 아무것도 바뀌지 않음)
        //             1(기존에 존재하던 팀이며, 데이터가 바뀌었음)
        //            -1(새로운 팀)
        //            -1(잘못된 데이터)
        private int CheckUserDefinedTeam(Data_UserDefinedTeam team)
        {
            if (team.TeamName.Length == 0)
                return -2;


            ArrayList arrUserDefineTeam = FormMain.Instance.UserDefinedTeam;
            foreach (Data_UserDefinedTeam data in arrUserDefineTeam)
            {
                if (data.TeamName == team.TeamName)
                {
                    team.ID = data.ID;

                    if (team.PhoneNumber.Length == 0)
                        return -2;

                    if (team.PhoneNumber == data.PhoneNumber &&
                        team.FaxNumber == data.FaxNumber)
                        return 0;
                    else
                        return 1;
                }
            }

            team.ID = -1;
            return -1;
        }

        private void SaveUserDefinedList()
        {
            ArrayList arrNewTeam = new ArrayList();
            ArrayList arrUpdateTeam = new ArrayList();

            foreach (KeyValuePair<int, Data_UserDefinedTeam> pair in this.m_dicUserDefinedTeamList)
            {
                int nResult = CheckUserDefinedTeam(pair.Value);

                if (nResult == 1)
                    arrUpdateTeam.Add(pair.Value);
                else if (nResult == -1)
                    arrNewTeam.Add(pair.Value);
            }

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strRemoveIDs = "", strSQL;

            ////////////////////////////////////////////////////////////////////
            // 데이터 삭제
            foreach (Data_UserDefinedTeam team in m_arrRemoveUserDefinedTeamList)
            {
                if (strRemoveIDs.Length == 0)
                    strRemoveIDs = team.ID.ToString();
                else
                    strRemoveIDs += ", " + team.ID.ToString();

                int nIndex = FindUserDefinedTeam(team.ID, FormMain.Instance.UserDefinedTeam);
                if (nIndex >= 0)
                    FormMain.Instance.UserDefinedTeam.RemoveAt(nIndex);
            }

            if (strRemoveIDs.Length > 0)
            {
                // ActionStepUsingUserDefinedTeam에서 먼저 삭제. skkim 2015-09-03
                strSQL = string.Format("Delete from ActionStepUsingTeam where TeamType = 3 and TeamID in ({0})", strRemoveIDs);
                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                strSQL = string.Format("Delete from UserDefinedTeam where id in ({0})", strRemoveIDs);
                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }

            m_arrRemoveUserDefinedTeamList.Clear();
            ////////////////////////////////////////////////////////////////////

            strSQL = "select max(id) from UserDefinedTeam";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            int nTeamID;

            if (arrResult == null || arrResult.Count == 0)
                nTeamID = 0;
            else
                nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

            foreach (Data_UserDefinedTeam team in arrNewTeam)
            {
                string strFaxNumber = team.FaxNumber == null || team.FaxNumber.Length == 0 ? "NULL" : "'" + team.FaxNumber + "'";
                string strPhoneNumber = team.PhoneNumber == null || team.PhoneNumber.Length == 0 ? "0000000" : team.PhoneNumber;

                strSQL = string.Format("Insert into UserDefinedTeam (ID, TeamName, PhoneNumber, FaxNumber, SiteID) values ({0}, '{1}', '{2}', {3}, {4})",
                    ++nTeamID, team.TeamName, strPhoneNumber, strFaxNumber, FormMain.Instance.SiteID);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                team.ID = nTeamID;
                FormMain.Instance.UserDefinedTeam.Add(team);
            }

            foreach (Data_UserDefinedTeam team in arrUpdateTeam)
            {
                string strFaxNumber = team.FaxNumber == null || team.FaxNumber.Length == 0 ? "NULL" : "'" + team.FaxNumber + "'";
                string strPhoneNumber = team.PhoneNumber == null || team.PhoneNumber.Length == 0 ? "0000000" : team.PhoneNumber;

                strSQL = string.Format("Update UserDefinedTeam set PhoneNumber = '{0}', FaxNumber = {1} where id = {2}",
                    strPhoneNumber, strFaxNumber, team.ID);

                if (dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                int nIndex = FindUserDefinedTeam(team.ID, FormMain.Instance.UserDefinedTeam);
                if (nIndex >= 0)
                {
                    Data_UserDefinedTeam _team = (Data_UserDefinedTeam)FormMain.Instance.UserDefinedTeam[nIndex];
                    _team.FaxNumber = team.FaxNumber;
                    _team.PhoneNumber = team.PhoneNumber;
                    _team.TeamName = team.TeamName;
                }
            }
        }

        private void dataGridViewUserDefined_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (sender != dataGridViewUserDefined)
                    return;

                if (dataGridViewUserDefined.SelectedRows == null || dataGridViewUserDefined.SelectedRows.Count == 0)
                    return;

                int nRowCount = dataGridViewUserDefined.Rows.Count;
                if (dataGridViewUserDefined.AllowUserToAddRows)
                    nRowCount--;

                int nRowIndex = dataGridViewUserDefined.SelectedRows[0].Index;
                if (nRowIndex >= nRowCount)
                    return;

                dataGridViewUserDefined.Rows.RemoveAt(nRowIndex);

                if (!m_dicUserDefinedTeamList.ContainsKey(nRowIndex))
                    return;

                Data_UserDefinedTeam selectedTeam = m_dicUserDefinedTeamList[nRowIndex];
                if (selectedTeam.ID > 0)
                    m_arrRemoveUserDefinedTeamList.Add(selectedTeam);

                /////////////////////////////////////////////////////////////////
                // dictionary의 데이터를 삭제된 행을 기준으로 하나씩 아래로 내린다.
                for (int i = nRowIndex + 1; i < nRowCount; i++)
                {
                    m_dicUserDefinedTeamList[i - 1] = m_dicUserDefinedTeamList[i];
                }

                m_dicUserDefinedTeamList.Remove(nRowCount - 1);
                /////////////////////////////////////////////////////////////////
            }
        }

        private void dataGridViewExternal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (sender != dataGridViewExternal)
                    return;

                if (dataGridViewExternal.SelectedRows == null || dataGridViewExternal.SelectedRows.Count == 0)
                    return;

                int nRowCount = dataGridViewExternal.Rows.Count;
                if (dataGridViewExternal.AllowUserToAddRows)
                    nRowCount--;

                int nRowIndex = dataGridViewExternal.SelectedRows[0].Index;
                if (nRowIndex >= nRowCount)
                    return;

                dataGridViewExternal.Rows.RemoveAt(nRowIndex);

                if (!m_dicExternalTeamList.ContainsKey(nRowIndex))
                    return;

                Data_ExternalTeam selectedTeam = m_dicExternalTeamList[nRowIndex];
                if (selectedTeam.ID > 0)
                    m_arrRemoveExternalTeamList.Add(selectedTeam);

                /////////////////////////////////////////////////////////////////
                // dictionary의 데이터를 삭제된 행을 기준으로 하나씩 아래로 내린다.
                for (int i = nRowIndex + 1; i < nRowCount; i++)
                {
                    m_dicExternalTeamList[i - 1] = m_dicExternalTeamList[i];
                }

                m_dicExternalTeamList.Remove(nRowCount - 1);
                /////////////////////////////////////////////////////////////////
            }
        }

        // nRowIndex의 첫번째 Cell의 텍스트가 다른 행에 이미 존재하는지 여부를 확인한다.
        // 이미 존재하면 false, 존재하지 않으면 true를 리턴한다.
        private bool CheckDuplicate(DataGridView grid, int nRowIndex, string strValue)
        {
            if (strValue == null || strValue == "")
                return false;

            int nRowCount = grid.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                if (i == nRowIndex)
                    continue;

                if (grid.Rows[i].Cells[0].Value != null)
                {
                    if (grid.Rows[i].Cells[0].Value.ToString() == strValue)
                        return false;
                }

            }

            return true;
        }

        private void dataGridViewExternal_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            DataGridViewRow row = grid.Rows[e.RowIndex];
            if (row == null)
                return;
            object value = row.Cells[e.ColumnIndex].Value;

            if (value == null)
                return;

            string strValue = value.ToString();
            Data_ExternalTeam team = (Data_ExternalTeam)row.Tag;

            if (e.ColumnIndex == 0)
            {
                if (team != null && !CheckDuplicate(grid, e.RowIndex, strValue))
                {
                    value = team.TeamName;

                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_ExternalTeam();
                        m_dicExternalTeamList[e.RowIndex] = team;
                        row.Tag = team;

                        team.PhoneNumber = "";
                        team.FaxNumber = "";
                        team.ID = -1;
                    }

                    // 새로 추가된 TeamName 이므로 ID를 -1로 둔다.(DB에 존재하지 않음)
                    team.TeamName = strValue;
                    

                }
            }
            else if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                bool isCheck = FormMain.Instance.GetPageLevel().numericCheck(strValue);

                if (!isCheck)
                {
                    MessageBox.Show("숫자 입력만 가능합니다.");

                    if (team == null)
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = e.ColumnIndex == 1 ? team.PhoneNumber : team.FaxNumber;
                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_ExternalTeam();
                        m_dicExternalTeamList[e.RowIndex] = team;
                        row.Tag = team;

                        team.PhoneNumber = "";
                        team.FaxNumber = "";
                    }

                    if (e.ColumnIndex == 1)
                        team.PhoneNumber = strValue;
                    else
                        team.FaxNumber = strValue;
                }
            }
        }

        private void dataGridViewUserDefined_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            DataGridViewRow row = grid.Rows[e.RowIndex];
            if (row == null)
                return;

            object value = row.Cells[e.ColumnIndex].Value;

            if (value == null)
                return;

            string strValue = value.ToString();
            

            Data_UserDefinedTeam team = (Data_UserDefinedTeam)row.Tag;

            if (e.ColumnIndex == 0)
            {
                if (team != null && !CheckDuplicate(grid, e.RowIndex, strValue))
                {
                    value = team.TeamName;
                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_UserDefinedTeam();
                        m_dicUserDefinedTeamList[e.RowIndex] = team;
                        row.Tag = team;

                        team.PhoneNumber = "";
                        team.FaxNumber = "";
                        // 새로 추가된 TeamName 이므로 ID를 -1로 둔다.(DB에 존재하지 않음)
                        team.ID = -1;
                    }
                    // 이름이 변경된 경우이므로 이름만 바꾼다.
                    team.TeamName = strValue;
                    
                }
            }
            else if (e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                bool isCheck = FormMain.Instance.GetPageLevel().numericCheck(strValue);

                if (!isCheck)
                {
                    MessageBox.Show("숫자 입력만 가능합니다.");

                    if (team == null)
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    else
                        grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = e.ColumnIndex == 1 ? team.PhoneNumber : team.FaxNumber;
                }
                else
                {
                    if (team == null)
                    {
                        team = new Data_UserDefinedTeam();
                        m_dicUserDefinedTeamList[e.RowIndex] = team;
                        row.Tag = team;

                        team.PhoneNumber = "";
                        team.FaxNumber = "";
                    }

                    if (e.ColumnIndex == 1)
                        team.PhoneNumber = strValue;
                    else
                        team.FaxNumber = strValue;
                }
            }
        }


        private DataGridViewRow mSelectedRow = null;
        private void dataGridViewUserDefined_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataGridViewUserDefined.SelectedRows;
            if( rows != null && rows.Count > 0)
            {
                DataGridViewRow row = rows[0];
                Data_UserDefinedTeam team = (Data_UserDefinedTeam)row.Tag;
                if (team != null)
                {
                    checkBoxDefault.Checked = false;
                    labelFullPath.Visible = true;
                    labelFullPath.Text = "사용자정의조직\\" + team.TeamName;
                    textBoxDisplay.Text = team.TeamName;

                    mSelectedRow = row;
                }
            }
        }

        private void dataGridViewExternal_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataGridViewExternal.SelectedRows;
            if (rows != null && rows.Count > 0)
            {
                DataGridViewRow row = rows[0];
                Data_ExternalTeam team = (Data_ExternalTeam)row.Tag;
                if( team != null)
                {
                    checkBoxDefault.Checked = false;
                    labelFullPath.Visible = true;
                    labelFullPath.Text = "외부기관\\" + team.TeamName;
                    textBoxDisplay.Text = team.TeamName;

                    mSelectedRow = row;
                }               
            }
        }

        private void Regular_Click(object sender, EventArgs e)
        {
            rbBtnRegular.Checked = true;
            rbBtnEmergency.Checked = false;
            rbBtnExternal.Checked = false;
            rbBtnUserDefine.Checked = false;
            rbBtnControlRoom.Checked = false;
            radioTeam_CheckedChanged(rbBtnRegular, null);
        }

        private void Emergency_Click(object sender, EventArgs e)
        {
            rbBtnRegular.Checked = false;
            rbBtnEmergency.Checked = true;
            rbBtnExternal.Checked = false;
            rbBtnUserDefine.Checked = false;
            rbBtnControlRoom.Checked = false;
            radioTeam_CheckedChanged(rbBtnEmergency, null);
        }

        private void External_Click(object sender, EventArgs e)
        {
            rbBtnRegular.Checked = false;
            rbBtnEmergency.Checked = false;
            rbBtnExternal.Checked = true;
            rbBtnUserDefine.Checked = false;
            rbBtnControlRoom.Checked = false;
            radioTeam_CheckedChanged(rbBtnExternal, null);
        }

        private void UserDefine_Click(object sender, EventArgs e)
        {
            rbBtnRegular.Checked = false;
            rbBtnEmergency.Checked = false;
            rbBtnExternal.Checked = false;
            rbBtnUserDefine.Checked = true;
            rbBtnControlRoom.Checked = false;
            radioTeam_CheckedChanged(rbBtnUserDefine, null);
        }

        private void ControlRoom_Click(object sender, EventArgs e)
        {
            rbBtnRegular.Checked = false;
            rbBtnEmergency.Checked = false;
            rbBtnExternal.Checked = false;
            rbBtnUserDefine.Checked = false;
            rbBtnControlRoom.Checked = true;
            radioTeam_CheckedChanged(rbBtnControlRoom, null);
        }

        private void checkBoxDefault_VisibleChanged(object sender, EventArgs e)
        {
            picDefault.Visible = checkBoxDefault.Visible;
            lblDefault.Visible = checkBoxDefault.Visible;
        }

        private void Default_Click(object sender, EventArgs e)
        {
            if (checkBoxDefault.Enabled == false) return;
            //if (checkBoxDefault.Visible == false) return;

            checkBoxDefault.Checked = !checkBoxDefault.Checked;
            checkBoxDefault_CheckedChanged(checkBoxDefault, null);
        }
     }
}
