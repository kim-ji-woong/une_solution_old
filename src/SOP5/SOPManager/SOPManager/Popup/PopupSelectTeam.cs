using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DBUtility;
using Sections;

namespace SOPManager
{
    public partial class PopupSelectTeam : Form
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
    
        private Sections.Section m_section;
        private Sections.SOPTeam.SOPTeamType m_currentTeamType = Sections.SOPTeam.SOPTeamType.None;

        private Sections.Section.ComponentType mType = Section.ComponentType.PROCESS;

        private ImageList m_imgList = new ImageList();
        private TreeView m_treeMirror = new TreeView();
        private MirrorManager m_mirrorManager = null;

        private bool m_bCommander = false;
        private DataGridViewRow mSelectedRow = null;
        private SOPTeam m_teamSelected = null;

        private Sections.SectionCommander m_initCommander = null;
        public Sections.SectionCommander commander { get { return m_initCommander; } }

        // 하위팀 포함 여부
        private bool m_includeChildTeamRegular = false;
        private bool m_includeChildTeamTemporary = false;
        private bool m_includeChildTeamExternal = false;

        public const string INCLUDE_TAG = "(하위팀 포함)";

        private string m_strDisplayText = "";
        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

		private PropertiesProcess propertiesProcess = null;
		public PropertiesProcess PropertiesProcess
		{
			get { return propertiesProcess; }
			set 
			{ 
				propertiesProcess = value;
				if (propertiesProcess == null)
					return;

                mType = Section.ComponentType.PROCESS;

				m_section = propertiesProcess.GetSection();
				Sections.PanelSectionEx panel = (Sections.PanelSectionEx)m_section.GetParent();
                m_currentTeamType = panel.TeamType;

                InitTree(m_currentTeamType);
				InitGrid();
                InitUserDefinedGrid();

                SetRadioBtn(m_currentTeamType);
                UpdateCustomButtos();
			}
		}

        private PropertiesInternal propertiesInternal = null;
        public PropertiesInternal PropertiesInternal
        {
            get { return propertiesInternal; }
            set { 
                propertiesInternal = value;
                if (propertiesInternal == null)
                    return;

                mType = Section.ComponentType.INTERNAL;

                m_section = propertiesInternal.GetSection();
                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)m_section.GetParent();
                m_currentTeamType = panel.TeamType;
      
                InitTree();
                InitGrid();
                InitUserDefinedGrid();
                
                SetRadioBtn(m_currentTeamType);
                UpdateCustomButtos();
            }
        }

        ArrayList m_arrRemove = new ArrayList();

		ArrayList mSelectedTeamList = new ArrayList();
		public ArrayList TeamList
		{
			get { return mSelectedTeamList; }
			set { mSelectedTeamList = value; }
		}

        public SOPTeam SelectedTeam
        {
            get { return m_teamSelected; }
            set { m_teamSelected = value; }
        }

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();


        private bool m_bEditProperties = true;
        public PopupSelectTeam(bool bEdit)
        {                
            m_bEditProperties = bEdit;
            
            InitializeComponent();
            
            //SOP 제어권 가진곳의 책임자
            checkBoxDefault.Visible = false;
            checkBoxDefault.Checked = false;
            lblDefault.Visible = false;
            picDefault.Visible = false;

            Init();
            UpdateControlSize();
        }

        public PopupSelectTeam(SectionCommander commander)
        {
            m_bCommander = true;
            m_bEditProperties = true;
            InitializeComponent();

            this.Size = new System.Drawing.Size(490, 545);

            m_initCommander = commander;
            checkBoxDefault.Visible = true;

            if (m_initCommander == null || m_initCommander.Team == null)
                m_currentTeamType = SOPTeam.SOPTeamType.Normal;
            else
            {
                m_currentTeamType = m_initCommander.Team.TeamType;
            }

            if (m_initCommander != null && m_initCommander.Team == null)
                checkBoxDefault.Checked = true;

            InitTree(m_currentTeamType);
            InitGrid();
            InitUserDefinedGrid();

            Init();

            SetRadioBtn(m_currentTeamType);

            if (m_initCommander.Team != null)
            {
                TreeNode node = FindNode(m_initCommander.Team.TeamID);
                treeViewTeam.SelectedNode = node;
                treeViewTeam.Select();
            }

            UpdateCustomButtos();
            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            FormMain.Instance.UpdateWindowRate(panel1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picRegular, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblRegular, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picEmergency, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblEmergency, WindowRateWidth, WindowRateHeight);            
            FormMain.Instance.UpdateWindowRate(checkBoxDefault, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picExternal, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblExternal, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picUserDefine, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblUserDefine, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picControlRoom, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblControlRoom, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(panelSearch, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(pictureBoxSearch, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(rtextSearch, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(labelTeamType, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(treeViewTeam, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(dataGridViewUserDefined, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picDefault, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblDefault, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(labelFullPath, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(label2, WindowRateWidth, WindowRateHeight);            
            FormMain.Instance.UpdateWindowRate(textBoxDisplay, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(picChildTeams, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblChildTeams, WindowRateWidth, WindowRateHeight);            
            FormMain.Instance.UpdateWindowRate(dataGridView, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnAdd, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnDel, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnOK, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnCancel, WindowRateWidth, WindowRateHeight);           
        }

        private void Init()
        {
            rbBtnRegular.Tag = SOPTeam.SOPTeamType.Regular;
            rbBtnControlRoom.Tag = SOPTeam.SOPTeamType.ControlRoom;
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

            labelTeamType.Text = rbBtnEmergency.Text;

            ToolTip tooltipAdd = new ToolTip();
            ToolTip tooltipDelete = new ToolTip();
            tooltipAdd.SetToolTip(btnAdd, "조직에서 선택한 팀을 오른쪽에 추가합니다.");
            tooltipDelete.SetToolTip(btnDel, "선택된 팀을 오른쪽에서 제거합니다.");

            dataGridViewUserDefined.CellPainting += dataGridViewUserDefined_CellPainting;
            dataGridView.CellPainting += dataGridViewUserDefined_CellPainting;

            SetSearchOptions();
        }

        void dataGridViewUserDefined_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView gdv = sender as DataGridView;
            if (gdv == null) return;

            foreach (DataGridViewRow row in gdv.Rows)
                row.MinimumHeight = gdv.RowTemplate.Height;   
        }

        public void UpdateCustomButtos()
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

            if (checkBoxIncludeChildTeams.Checked == true)            
                picChildTeams.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_enable;            
            else            
                picChildTeams.BackgroundImage = global::SOPManager.Properties.Resources.__COMMON_ckb_disable;            
        }

        private void SetSearchOptions()
        {
            //m_imgList.ImageSize = new Size(32, 32);
            //m_imgList.Images.AddStrip(global::SOPManager.Properties.Resources.current_scale_to_fit);
            //m_imgList.Images.AddStrip(global::SOPManager.Properties.Resources.__COMMON_Search);
            //pictureBoxSearch.BackgroundImage = m_imgList.Images[0];

            m_mirrorManager = new MirrorManager(treeViewTeam, dataGridViewUserDefined, rtextSearch, this);            
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

        private void InitGrid()
        {
            if( mType == Section.ComponentType.PROCESS)
            {
                if (mSelectedTeamList.Count == 0) return;

                foreach (Sections.SOPTeam row in mSelectedTeamList)
                {
                    DataGridViewRow gridRow = new DataGridViewRow();
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Value = row.TeamName;
                    cell.Tag = row;
                    gridRow.Cells.Add(cell);
                    gridRow.Tag = row.TeamType;
                    dataGridView.Rows.Add(gridRow);

                    if (row.IncludeChildTeams)
                        cell.Value = cell.Value.ToString() + INCLUDE_TAG;
                }
            }
            else if(mType == Section.ComponentType.INTERNAL)
            {
                if (mSelectedTeamList.Count == 0) return;

                foreach (Sections.SOPTeam row in mSelectedTeamList)
                {
                    DataGridViewRow gridRow = new DataGridViewRow();
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Value = row.TeamName;
                    cell.Tag = row;
                    gridRow.Cells.Add(cell);
                    gridRow.Tag = row.TeamType;
                    dataGridView.Rows.Add(gridRow);

                    if (row.IncludeChildTeams)
                        cell.Value = cell.Value.ToString() + INCLUDE_TAG;
                }
            }           
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

        public void InitTree(Sections.SOPTeam.SOPTeamType teamType = Sections.SOPTeam.SOPTeamType.None)
        {
            Sections.SOPTeam.SOPTeamType nTeamType = m_currentTeamType;//panel.TeamType;
            treeViewTeam.Nodes.Clear();

            if (!m_bCommander)
            {
                if (m_section == null)
                    return;

                if (teamType == Sections.SOPTeam.SOPTeamType.None)
                {

                    if (mType == Section.ComponentType.PROCESS)
                    {
                        Sections.SectionDataProcess data = (Sections.SectionDataProcess)m_section.Data;

                        if (data.TeamList.Count > 0)
                        {
                            Sections.SOPTeam team = (Sections.SOPTeam)data.TeamList[0];
                            nTeamType = team.TeamType;
                        }
                    }
                    else if (mType == Section.ComponentType.INTERNAL)
                    {
                        Sections.SectionDataInternal data = (Sections.SectionDataInternal)m_section.Data;

                        if (data.TeamList.Count > 0)
                        {
                            Sections.SOPTeam team = (Sections.SOPTeam)data.TeamList[0];
                            nTeamType = team.TeamType;
                        }
                    }
                }
                else
                    nTeamType = teamType;
            }
            else
                nTeamType = teamType;

            m_currentTeamType = nTeamType;
            SetTeamTypeLabel(nTeamType);

            if (nTeamType == Sections.SOPTeam.SOPTeamType.External)         // 외부 조직
                LoadExternalTeamTree();
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.UserDefined)    // 사용자 정의 조직
                LoadUserDefinedTeamTree();
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.Normal)    // 평일 비상 조직
                LoadTemporaryNormalTeamTree();
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.Holiday)    // 야간 및 휴일 비상 조직
                LoadTemporaryEmergencyTeamTree();
            else if (nTeamType == SOPTeam.SOPTeamType.ControlRoom)
                LoadControlRoomTree();
            else// if (nTeamType == Sections.SOPTeam.SOPTeamType.Regular)  // 정규 조직
                LoadRegularTeamTree();
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

        private int FindTagInNode(string strValue)
        {
            TreeNode node = FindNode(strValue, treeViewTeam.Nodes);
            if (node != null)
            {
                if (node.Text == strValue)
                    return (int)node.Tag;
            }

            return -1;
        }

		private string GetOriginalTeam()
		{
			string strValue = "";
			int nCount = 0;

            if (mType == Section.ComponentType.PROCESS)
            {
                foreach (Sections.SOPTeam team in propertiesProcess.SelectedTeamList)
                {
                    strValue += team.TeamName;
                    if (nCount > 1 && nCount != propertiesProcess.SelectedTeamList.Count - 1)
                    {
                        strValue += ", ";
                    }
                    nCount++;
                }
            }
            else if( mType == Section.ComponentType.INTERNAL)
            {
                foreach (Sections.SOPTeam team in propertiesInternal.SelectedTeamList)
                {
                    strValue += team.TeamName;
                    if (nCount > 1 && nCount != propertiesInternal.SelectedTeamList.Count - 1)
                    {
                        strValue += ", ";
                    }
                    nCount++;
                }
            }
			
			return strValue;
		}		
		
		private void ApplySelectedTeam()
		{
            mSelectedTeamList.Clear();
			foreach (DataGridViewRow row in dataGridView.Rows)
			{
                Sections.SOPTeam.SOPTeamType teamType = (Sections.SOPTeam.SOPTeamType)row.Tag;
                if (teamType == SOPTeam.SOPTeamType.Regular || teamType == SOPTeam.SOPTeamType.Normal 
                    || teamType == SOPTeam.SOPTeamType.Holiday || teamType == SOPTeam.SOPTeamType.External
                    || teamType == SOPTeam.SOPTeamType.ControlRoom)
                {
                    Sections.SOPTeam teamTarget = (Sections.SOPTeam)row.Cells[0].Tag;
                    
                    Sections.SOPTeam sopTeam = new Sections.SOPTeam();
                    sopTeam.TeamID = teamTarget.TeamID;
                    sopTeam.TeamType = (Sections.SOPTeam.SOPTeamType)row.Tag;
                    sopTeam.TeamName = teamTarget.TeamName;
                    sopTeam.IncludeChildTeams = teamTarget.IncludeChildTeams;
                    //sopTeam.TeamName = (string)row.Cells[0].Value;
                    mSelectedTeamList.Add(sopTeam);
                }
                else if (teamType == SOPTeam.SOPTeamType.UserDefined)
                {
                    Sections.SOPTeam teamTarget = null;
                    teamTarget = (Sections.SOPTeam)row.Cells[0].Tag;

                    bool bFind = false;
                    if (teamType == SOPTeam.SOPTeamType.UserDefined)
                    {
                        ArrayList arTeams = FormMain.Instance.UserDefinedTeam;
                        foreach (Data_UserDefinedTeam tempTeam in arTeams)
                        {
                            if (tempTeam.TeamName == teamTarget.TeamName)
                            {
                                teamTarget.TeamID = tempTeam.ID;
                                bFind = true;
                                break;
                            }
                            else
                            {
                                if (m_bSelectedTeamUpdateUserDefine)
                                {
                                    if (teamTarget.TeamID == tempTeam.ID)
                                    {
                                        if (teamTarget.TeamName != tempTeam.TeamName)
                                        {
                                            bFind = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // ID가 존재하고 삭제되지 않은 팀인 경우만 추가해준다.
                    if (teamTarget.TeamID > 0 && bFind == true)
                    {
                        Sections.SOPTeam sopTeam = new Sections.SOPTeam();
                        sopTeam.TeamID = teamTarget.TeamID;
                        sopTeam.TeamType = (Sections.SOPTeam.SOPTeamType)row.Tag;
                        sopTeam.TeamName = (string)row.Cells[0].Value;
                        mSelectedTeamList.Add(sopTeam);
                    }
                }
			}
		}

        private string GetSelectedTeam()
        {
            string strValue = "";
            int nRow = 0;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {              
                strValue += (string)row.Cells[0].Value;
                if (dataGridView.Rows.Count > 1 && nRow != dataGridView.Rows.Count - 1)
                {
                    strValue += ", ";
                    nRow++;
                }
            }
            return strValue;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (m_bCommander)
                SelectCommander();
            else
                SelectTeam();
			
            this.Close();
        }

        private void SelectTeam()
        {
            string strOrgValue = "", strValue = "";
            if (!m_bSelectedTeamUpdateUserDefine)
            {
                strOrgValue = GetOriginalTeam();
                strValue = GetSelectedTeam();
            }

            SaveUserDefinedList();

            if (strOrgValue != strValue || m_bSelectedTeamUpdateUserDefine)
            {
                ApplySelectedTeam();

                if (m_bEditProperties == true)
                {
                    if (mType == Section.ComponentType.PROCESS)
                        propertiesProcess.SelectedTeamList = mSelectedTeamList;
                    else if (mType == Section.ComponentType.INTERNAL)
                        propertiesInternal.SelectedTeamList = mSelectedTeamList;
                }

                m_strDisplayText = textBoxDisplay.Text;

                this.DialogResult = DialogResult.OK;
            }
        }

        private void SelectCommander()
        {
            if (!checkBoxDefault.Checked)
            {
                if ((treeViewTeam.SelectedNode == null ||
                    treeViewTeam.SelectedNode.Tag == null ||
                    (treeViewTeam.SelectedNode.Tag is int) == false) &&
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
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        
        private void btnAdd_Click(object sender, EventArgs e)
        {              
            if( m_currentTeamType == SOPTeam.SOPTeamType.Regular 
                || m_currentTeamType == SOPTeam.SOPTeamType.Normal
                || m_currentTeamType == SOPTeam.SOPTeamType.Holiday
                || m_currentTeamType == SOPTeam.SOPTeamType.External
                || m_currentTeamType == SOPTeam.SOPTeamType.ControlRoom)
            {
                if (treeViewTeam.SelectedNode == null)
                    return;

                Sections.SOPTeam newTeam = new SOPTeam();
                newTeam.TeamID = (int)treeViewTeam.SelectedNode.Tag;
                newTeam.TeamName = treeViewTeam.SelectedNode.Text;
                newTeam.TeamType = m_currentTeamType;

                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = treeViewTeam.SelectedNode.Text;
                cell.Tag = newTeam;
                gridRow.Cells.Add(cell);

                if (checkBoxIncludeChildTeams.Visible && checkBoxIncludeChildTeams.Checked)
                {
                    newTeam.IncludeChildTeams = true;
                    cell.Value = cell.Value.ToString() + INCLUDE_TAG;
                }

                if (!FindTeamName2(cell)/* && !FindTeamName(treeViewTeam.SelectedNode)*/)
                {
                    gridRow.Tag = m_currentTeamType;
                    dataGridView.Rows.Add(gridRow);
                }
            }
            else if (m_currentTeamType == SOPTeam.SOPTeamType.UserDefined)
            {
                DataGridViewSelectedRowCollection rows = this.dataGridViewUserDefined.SelectedRows;
                if (rows != null && rows.Count > 0)
                {                    
                    foreach (DataGridViewRow row in rows)
                    {
                        if (row.Tag == null)
                            continue;

                        Data_UserDefinedTeam team = (Data_UserDefinedTeam)row.Tag;
                        if( team != null)
                        {
                            Sections.SOPTeam newTeam = new SOPTeam();
                            newTeam.TeamID = team.ID;
                            newTeam.TeamName = team.TeamName;
                            newTeam.TeamType = m_currentTeamType;

                            DataGridViewRow gridRow = new DataGridViewRow();
                            DataGridViewCell cell = new DataGridViewTextBoxCell();
                            cell.Value = newTeam.TeamName;
                            cell.Tag = newTeam;
                            gridRow.Cells.Add(cell);

                            if (!FindTeamName2(cell))
                            {
                                gridRow.Tag = m_currentTeamType;
                                dataGridView.Rows.Add(gridRow);
                            }
                        }
                    }
                }
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if(row.Cells[0].Selected)
                {
                    m_arrRemove.Add(row);
                    dataGridView.Rows.Remove(row);                    
                }
            }
        }

        // 추가하려는 레벨의 하위레벨 체크
        private bool FindTeamName(TreeNode newNode)
        {
            // 하위레벨 데이터 추출
            Dictionary<int, TreeNode> dic = new Dictionary<int, TreeNode>();
            ExtractLowLevel(newNode.Nodes, ref dic);

            bool bFirst = true;
            if (dic.Count != 0)
            {
                for (int i=dataGridView.Rows.Count-1; i>=0; --i)
                {
                    DataGridViewRow row = dataGridView.Rows[i];
                    Sections.SOPTeam team = (Sections.SOPTeam)row.Cells[0].Tag;

                    if (m_currentTeamType != team.TeamType)
                        continue;

                    if (dic.ContainsKey(team.TeamID))
                    {
                        if (bFirst)
                        {
                            DialogResult res = MessageBox.Show("하위 레벨의 팀이 존재합니다. 삭제 후 추가하시겠습니까?", "확인", MessageBoxButtons.YesNo);
                            if (res != DialogResult.Yes)
                            {
                                return true;
                            }

                            bFirst = false;
                        }
                        dataGridView.Rows.RemoveAt(i);
                    }
                }
            }

            // 상위레벨 데이터 추출
            dic.Clear();
            ExtractHighLevel(newNode, ref dic);
            bFirst = true;
            if (dic.Count != 0)
            {
                for (int i = dataGridView.Rows.Count - 1; i >= 0; --i)
                {
                    DataGridViewRow row = dataGridView.Rows[i];
                    Sections.SOPTeam team = (Sections.SOPTeam)row.Cells[0].Tag;

                    if (m_currentTeamType != team.TeamType)
                        continue;

                    if (dic.ContainsKey(team.TeamID))
                    {
                        if (bFirst)
                        {
                            DialogResult res = MessageBox.Show("상위 레벨의 팀이 존재합니다. 삭제 후 추가하시겠습니까?", "확인", MessageBoxButtons.YesNo);
                            if (res != DialogResult.Yes)
                                return true;

                            bFirst = false;
                        }
                        dataGridView.Rows.RemoveAt(i);
                    }
                }
            }

            return false;
        }

        // 하위레벨 데이터 추출
        private void ExtractLowLevel(TreeNodeCollection nodes, ref Dictionary<int, TreeNode> dic)
        {
            foreach(TreeNode node in nodes)
            {
                int id = (int)node.Tag;
                dic.Add(id, node);

                ExtractLowLevel(node.Nodes, ref dic);
            }
        }

        // 상위레벨 데이터 추출
        private void ExtractHighLevel(TreeNode node, ref Dictionary<int, TreeNode> dic)
        {
            while(node.Parent != null)
            {
                int id = (int)node.Parent.Tag;
                dic.Add(id, node);

                node = node.Parent;
            }
        }

        // 같은 레벨 존재 여부 체크
        private bool FindTeamName2(DataGridViewCell cell)
        {
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                Sections.SOPTeam team1 = (Sections.SOPTeam)row.Cells[0].Tag;
                Sections.SOPTeam team2 = (Sections.SOPTeam)cell.Tag;
                if ((row.Cells[0].Value.ToString() == cell.Value.ToString()) && (team1.TeamType == team2.TeamType))
                {
                    if (team1.TeamID != -1 && team2.TeamID != -1)
                    {
                        if (team1.TeamID == team2.TeamID)
                            return true;
                    }
                }
            }
            return false;
        }

        private void PopupSelectTeam_MouseDown(object sender, MouseEventArgs e)
        {
            //m_bLeftMouseDown = true;
            //m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupSelectTeam_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    if (m_bLeftMouseDown ==true)
            //    {
            //        Point pt = this.PointToScreen(new Point(e.X, e.Y));
            //        int dx = pt.X - m_ptMove.X;
            //        int dy = pt.Y - m_ptMove.Y;
            //        if (!(dx == 0 && dy == 0))
            //        {
            //            Point ptCur = this.Location;
            //            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            //            m_ptMove.X += dx;
            //            m_ptMove.Y += dy;
            //        }
            //    }
            //}
        }

        private void PopupSelectTeam_MouseUp(object sender, MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Left)
                //m_bLeftMouseDown = false;
        }

        private void SetRadioBtn(Sections.SOPTeam.SOPTeamType nTeamType)
        {
            if (nTeamType == SOPTeam.SOPTeamType.Normal || nTeamType == SOPTeam.SOPTeamType.Holiday)
            {
                rbBtnEmergency.Checked = true;
            }
            else if (nTeamType == SOPTeam.SOPTeamType.External)
            {
                rbBtnExternal.Checked = true;
            }
            else if (nTeamType == SOPTeam.SOPTeamType.UserDefined)
            {
                rbBtnUserDefine.Checked = true;
            }
            else if (nTeamType == SOPTeam.SOPTeamType.Regular)
            {
                rbBtnRegular.Checked = true;
            }
            else if (nTeamType == SOPTeam.SOPTeamType.ControlRoom)
            {
                rbBtnControlRoom.Checked = true;
            }
            else
                rbBtnRegular.Checked = true;
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

                dataGridViewUserDefined.Visible = false;

                if (treeViewTeam.Nodes.Count > 0)
                    treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];

                checkBoxIncludeChildTeams.Checked = m_includeChildTeamTemporary;
                checkBoxIncludeChildTeams.Visible = true;
            }
            else if (nTeamType == SOPTeam.SOPTeamType.External)
            {
                labelTeamType.Text = "외부 기관";

                InitTree(nTeamType);
                treeViewTeam.Visible = true;

                dataGridViewUserDefined.Visible = false;

                if (treeViewTeam.Nodes.Count > 0)
                    treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];

                checkBoxIncludeChildTeams.Checked = m_includeChildTeamExternal;
                checkBoxIncludeChildTeams.Visible = true;
            }
            else if (nTeamType == SOPTeam.SOPTeamType.UserDefined)
            {
                labelTeamType.Text = "사용자정의조직";
                
                treeViewTeam.Visible = false;

                dataGridViewUserDefined.Visible = true;

                if (dataGridViewUserDefined.RowCount != 0)
                {
                    Data_UserDefinedTeam team = (Data_UserDefinedTeam)dataGridViewUserDefined.Rows[0].Tag;
                    GridSelected(dataGridViewUserDefined.Rows[0], team);
                }

                checkBoxIncludeChildTeams.Visible = false;
            }
            else if (nTeamType == SOPTeam.SOPTeamType.Regular)
            {
                labelTeamType.Text = "정규조직";

                InitTree(nTeamType);
                
                treeViewTeam.Visible = true;

                if (treeViewTeam.Nodes.Count > 0)
                    treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];

                dataGridViewUserDefined.Visible = false;

                checkBoxIncludeChildTeams.Checked = m_includeChildTeamRegular;
                checkBoxIncludeChildTeams.Visible = true;
            }
            else if (nTeamType == SOPTeam.SOPTeamType.ControlRoom)
            {
                labelTeamType.Text = "교대근무자";

                InitTree(nTeamType);

                treeViewTeam.Visible = true;

                if (treeViewTeam.Nodes.Count > 0)
                    treeViewTeam.SelectedNode = treeViewTeam.Nodes[0];

                dataGridViewUserDefined.Visible = false;
                checkBoxIncludeChildTeams.Visible = false;
            }

            m_mirrorManager.Refresh(dataGridViewUserDefined.Visible);
            UpdateCustomButtos();
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
        //private int CheckUserDefinedTeam(Data_UserDefinedTeam team)
        //{
        //    if (team.TeamName.Length == 0)
        //        return -2;

        //    ArrayList arrUserDefineTeam = FormMain.Instance.UserDefinedTeam;
        //    foreach (Data_UserDefinedTeam data in arrUserDefineTeam)
        //    {
        //        if (data.TeamName == team.TeamName)
        //        {
        //            team.ID = data.ID;

        //            if (team.PhoneNumber.Length == 0)
        //                return -2;

        //            if (team.PhoneNumber == data.PhoneNumber &&
        //                team.FaxNumber == data.FaxNumber)
        //                return 0;
        //            else
        //                return 1;
        //        }
        //    }

        //    team.ID = -1;
        //    return -1;
        //}

        /// <summary>
        /// 기존에 존재하던 사용자 정의 조직 데이터인가 여부.
        /// 만일 기존에 존재하던 팀이라면 데이터가 바뀌었는지 여부
        /// </summary>
        private void CheckUserDefinedTeam(ref ArrayList arrNewTeam, ref ArrayList arrUpdateTeam)
        {
            foreach (KeyValuePair<int, Data_UserDefinedTeam> pair in this.m_dicUserDefinedTeamList)
            {
                if (pair.Value.TeamName.Length == 0)
                    continue;

                Data_UserDefinedTeam data = pair.Value;

                if (data.ID == -1)
                {
                    arrNewTeam.Add(data);
                }
                else
                {
                    foreach (Data_UserDefinedTeam team in FormMain.Instance.UserDefinedTeam)
                    {
                        if (data.TeamName == team.TeamName)
                        {
                            team.ID = data.ID;

                            if (team.PhoneNumber == data.PhoneNumber &&
                                team.FaxNumber == data.FaxNumber)
                                break;
                            else
                            {
                                arrUpdateTeam.Add(team);
                                break;
                            }
                        }
                        else if (data.TeamName != team.TeamName && data.ID == team.ID)
                        {
                            arrUpdateTeam.Add(team);
                            break;
                        }
                    }
                }
            }
        }

        private void SaveUserDefinedList()
        {
            ArrayList arrNewTeam = new ArrayList();
            ArrayList arrUpdateTeam = new ArrayList();

            //foreach (KeyValuePair<int, Data_UserDefinedTeam> pair in this.m_dicUserDefinedTeamList)
            //{
            //    int nResult = CheckUserDefinedTeam(pair.Value);

            //    if (nResult == 1)
            //        arrUpdateTeam.Add(pair.Value);
            //    else if (nResult == -1)
            //        arrNewTeam.Add(pair.Value);
            //}

            CheckUserDefinedTeam(ref arrNewTeam, ref arrUpdateTeam);

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

                strSQL = string.Format("Update UserDefinedTeam set PhoneNumber = '{0}', FaxNumber = {1}, TeamName='{3}' where id = {2}",
                    strPhoneNumber, strFaxNumber, team.ID, team.TeamName);

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

        public void dataGridViewUserDefined_KeyDown(object sender, KeyEventArgs e)
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

        // nRowIndex의 첫번째 Cell의 텍스트가 다른 행에 이미 존재하는지 여부를 확인한다.
        // 이미 존재하면 false, 존재하지 않으면 true를 리턴한다.
        public static bool CheckDuplicate(DataGridView grid, int nRowIndex, string strValue)
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
                        // 새로 추가된 TeamName 이므로 ID를 -1로 둔다.(DB에 존재하지 않음)
                        team.ID = -1;
                    }
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

        /// <summary>
        /// 선택된팀이 사용자정의조직이고 명칭이 변경되었을 경우 변경된 사용자 조직을 쓰고 있는 곳 데이터를 reload하기 위한 변수
        /// </summary>
        private bool m_bSelectedTeamUpdateUserDefine = false;
        public bool selectedTeamUpdateUserDefine
        {
            get { return m_bSelectedTeamUpdateUserDefine; }
            set { m_bSelectedTeamUpdateUserDefine = value; }
        }
        public void dataGridViewUserDefined_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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
            string orgTeamName = (team == null) ? "" : team.TeamName;

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
                    // 이름만 변경되는 경우
                    team.TeamName = strValue;

                    // 수신처에 추가된 항목이면 업데이트
                    if (!m_bCommander)
                    {
                        foreach (DataGridViewRow dataRow in dataGridView.Rows)
                        {
                            Sections.SOPTeam team1 = (Sections.SOPTeam)dataRow.Cells[0].Tag;
                            if (team1.TeamType == SOPTeam.SOPTeamType.UserDefined)
                            {
                                if (team1.TeamID == team.ID)
                                {
                                    if (team1.TeamID == -1)
                                    {
                                        if (team1.TeamName != strValue)
                                            continue;
                                    }

                                    dataRow.Cells[0].Value = strValue;
                                    SOPTeam dataRowTag = (SOPTeam)dataRow.Cells[0].Tag;
                                    m_bSelectedTeamUpdateUserDefine = true;
                                }
                            }
                        }
                    }
                    else
                        m_bSelectedTeamUpdateUserDefine = true;
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

        public void NodeSelected(TreeNode node)
        {
            checkBoxDefault.Checked = false;
            labelFullPath.Visible = true;
            labelFullPath.Text = node.FullPath;

            if (m_currentTeamType == SOPTeam.SOPTeamType.Regular)
            {
                if (node.Text.EndsWith("장"))
                    textBoxDisplay.Text = node.Text;
                else
                    textBoxDisplay.Text = node.Text + "장";
            }
            else
                textBoxDisplay.Text = node.Text;

            treeViewTeam.SelectedNode = node;
            treeViewTeam.Select();
        }

        public void GridSelected(DataGridViewRow row, Data_UserDefinedTeam team)
        {
            if (team != null)
            {
                labelFullPath.Text = "사용자정의조직\\" + team.TeamName;
                textBoxDisplay.Text = team.TeamName;
                mSelectedRow = row;
            }
        }

        private void checkBoxIncludeChildTeams_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBtnRegular.Checked)
                m_includeChildTeamRegular = checkBoxIncludeChildTeams.Checked;
            else if (rbBtnEmergency.Checked)
                m_includeChildTeamTemporary = checkBoxIncludeChildTeams.Checked;
            else if (rbBtnExternal.Checked)
                m_includeChildTeamExternal = checkBoxIncludeChildTeams.Checked;
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

        private void IncludeChildTeam_Click(object sender, EventArgs e)
        {
            if (checkBoxIncludeChildTeams.Visible == false) return;

            checkBoxIncludeChildTeams.Checked = !checkBoxIncludeChildTeams.Checked;
            checkBoxIncludeChildTeams_CheckedChanged(checkBoxIncludeChildTeams, null);
            UpdateCustomButtos();
        }

        private void checkBoxIncludeChildTeams_VisibleChanged(object sender, EventArgs e)
        {
            picChildTeams.Visible = checkBoxIncludeChildTeams.Visible;
            lblChildTeams.Visible = checkBoxIncludeChildTeams.Visible;
        }

        private void Default_Click(object sender, EventArgs e)
        {
            if (checkBoxDefault.Enabled == false) return;
            if (checkBoxDefault.Visible == false) return;

            checkBoxDefault.Checked = !checkBoxDefault.Checked;
            checkBoxDefault_CheckedChanged(checkBoxDefault, null);
        }

        private void checkBoxDefault_VisibleChanged(object sender, EventArgs e)
        {
            picDefault.Visible = checkBoxDefault.Visible;
            lblDefault.Visible = checkBoxDefault.Visible;
        }
    }

    class MirrorManager
    {
        private TreeView m_treeOrigin = null;
        private TreeView m_treeMirror = null;
        private DataGridView m_gridOrigin = null;
        private DataGridView m_gridMirror = null;
        private RichTextBox m_textBox = null;
        private PopupSelectTeam m_parent = null;
        private bool m_gridMode = false;

        public MirrorManager(TreeView treeOrigin, DataGridView gridOrigin, RichTextBox textBox, PopupSelectTeam parent)
        {
            m_parent = parent;

            m_treeOrigin = treeOrigin;
            m_treeMirror = new TreeView();

            m_gridOrigin = gridOrigin;
            m_gridMirror = new DataGridView();

            m_textBox = textBox;

            m_treeOrigin.Parent.Controls.Add(m_treeMirror);

            m_treeMirror.BackColor =    m_treeOrigin.BackColor;
            m_treeMirror.BorderStyle =  m_treeOrigin.BorderStyle;
            m_treeMirror.Font =         m_treeOrigin.Font;
            m_treeMirror.ForeColor =    m_treeOrigin.ForeColor;
            m_treeMirror.Location =     m_treeOrigin.Location;
            m_treeMirror.Size =         m_treeOrigin.Size;

            m_treeMirror.Hide();

            //this.m_textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.m_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.m_treeMirror.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewMirror_AfterSelect);
            this.m_treeOrigin.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewOrigin_AfterSelect);

            InitGrid();
        }

        private void InitGrid()
        {
            foreach (DataGridViewColumn column in m_gridOrigin.Columns)
            {
                DataGridViewColumn column2 = (DataGridViewColumn)column.Clone();
                m_gridMirror.Columns.Add(column2);
            }

            this.m_gridMirror.AllowUserToDeleteRows =           m_gridOrigin.AllowUserToDeleteRows;
            this.m_gridMirror.AlternatingRowsDefaultCellStyle = m_gridOrigin.AlternatingRowsDefaultCellStyle;
            this.m_gridMirror.BackgroundColor =                 m_gridOrigin.BackgroundColor;
            this.m_gridMirror.ColumnHeadersDefaultCellStyle =   m_gridOrigin.ColumnHeadersDefaultCellStyle;
            this.m_gridMirror.ColumnHeadersHeightSizeMode =     m_gridOrigin.ColumnHeadersHeightSizeMode;
            this.m_gridMirror.DefaultCellStyle =                m_gridOrigin.DefaultCellStyle;
            this.m_gridMirror.GridColor =                       m_gridOrigin.GridColor;
            this.m_gridMirror.Location =                        m_gridOrigin.Location;
            this.m_gridMirror.RowHeadersDefaultCellStyle =      m_gridOrigin.RowHeadersDefaultCellStyle;
            this.m_gridMirror.RowHeadersVisible =               m_gridOrigin.RowHeadersVisible;
            this.m_gridMirror.RowsDefaultCellStyle =            m_gridOrigin.RowsDefaultCellStyle;
            this.m_gridMirror.RowTemplate.Height =              m_gridOrigin.RowTemplate.Height;
            this.m_gridMirror.SelectionMode =                   m_gridOrigin.SelectionMode;
            this.m_gridMirror.Size =                            m_gridOrigin.Size;

            this.m_gridOrigin.Parent.Controls.Add(this.m_gridMirror);

            this.m_gridMirror.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMirror_CellEndEdit);
            this.m_gridMirror.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridMirror_KeyDown);
            this.m_gridMirror.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridMirror_CellMouseClick);
            this.m_gridOrigin.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridOrigin_CellMouseClick);
            //this.m_gridOrigin.SelectionChanged += new System.EventHandler(gridOrigin_SelectionChanged);

            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            m_gridMirror.Size = new Size((int)(m_gridMirror.Width * WindowRateWidth), (int)(m_gridMirror.Height * WindowRateHeight));
            m_gridMirror.Location = new Point((int)(m_gridMirror.Location.X * WindowRateWidth), (int)(m_gridMirror.Location.Y * WindowRateHeight));            
        }

        private void gridMirror_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (m_gridMirror.Rows[e.RowIndex].IsNewRow)
                m_gridOrigin.ClearSelection();
            else
            {
                Data_UserDefinedTeam team = (Data_UserDefinedTeam)m_gridMirror.Rows[e.RowIndex].Tag;
                DataGridViewRow row = FindRow(m_gridOrigin, team);

                m_gridOrigin.ClearSelection();
                row.Cells[e.ColumnIndex].Selected = true;

                m_parent.GridSelected(row, team);
            }
        }

        private void gridOrigin_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (m_gridOrigin.Rows[e.RowIndex].IsNewRow)
                m_gridOrigin.ClearSelection();
            else
            {
                Data_UserDefinedTeam team = (Data_UserDefinedTeam)m_gridOrigin.Rows[e.RowIndex].Tag;
                DataGridViewRow row = FindRow(m_gridOrigin, team);

                m_gridOrigin.ClearSelection();
                row.Cells[e.ColumnIndex].Selected = true;

                m_parent.GridSelected(row, team);
            }
        }

        private void gridOrigin_SelectionChanged(object sender, EventArgs e)
        {
            DataGridView grid = (DataGridView)sender;
            DataGridViewRow row = grid.SelectedRows[0];
            Data_UserDefinedTeam team = (Data_UserDefinedTeam)row.Tag;

            m_parent.GridSelected(row, team);
        }

        private void gridMirror_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (sender != m_gridMirror)
                    return;

                if (m_gridMirror.SelectedRows == null || m_gridMirror.SelectedRows.Count == 0)
                    return;

                int nRowCount = m_gridMirror.Rows.Count;
                if (m_gridMirror.AllowUserToAddRows)
                    nRowCount--;

                int nRowIndex = m_gridMirror.SelectedRows[0].Index;
                if (nRowIndex >= nRowCount)
                    return;

                m_gridMirror.Rows.RemoveAt(nRowIndex);

                m_parent.dataGridViewUserDefined_KeyDown(m_gridOrigin, e);
            }
        }

        private DataGridViewRow FindRow(DataGridView grid, Data_UserDefinedTeam team)
        {
            if (team == null)
                return null;

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Tag == team)
                    return row;
            }

            return null;
        }

        private void gridMirror_CellEndEdit(object sender, DataGridViewCellEventArgs e)
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

            DataGridViewRow row2 = FindRow(m_gridOrigin, team);

            if (row2 == null)
                row2 = MakeNewRow(m_gridOrigin);

            row2.Cells[e.ColumnIndex].Value = value;
            m_parent.dataGridViewUserDefined_CellEndEdit(m_gridOrigin, new DataGridViewCellEventArgs(e.ColumnIndex, row2.Index));

            row.Tag = row2.Tag;
            row.Cells[e.ColumnIndex].Value = row2.Cells[e.ColumnIndex].Value;
            row.Cells[e.ColumnIndex].Tag = row2.Cells[e.ColumnIndex].Tag;
        }

        public static DataGridViewRow MakeNewRow(DataGridView grid)
        {
            if (grid.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)grid.Rows[grid.Rows.Count - 1].Clone();
                grid.Rows.Add(row);

                return grid.Rows[grid.Rows.Count - 2];
            }
            else
            {
                grid.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)grid.Rows[grid.Rows.Count - 1].Clone();
                grid.Rows.Add(row);

                grid.AllowUserToAddRows = false;
            }

            return grid.Rows[grid.Rows.Count - 1];
        }

        //private void textBox_TextChanged(object sender, EventArgs e)
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e == null || (e.KeyChar != (char)Keys.Enter && e.KeyChar != (char)Keys.Escape))
                return;

            if (m_textBox.Text.Length == 0)
            {
                if (m_gridMode)
                {
                    m_gridOrigin.Show();
                    m_gridMirror.Hide();
                    m_treeOrigin.Hide();
                    m_treeMirror.Hide();
                }
                else
                {
                    m_treeOrigin.Show();
                    m_treeMirror.Hide();
                    m_gridOrigin.Hide();
                    m_gridMirror.Hide();
                }
            }
            else
            {
                if (m_gridMode)
                {
                    ClearMirrorGrid();
                    ResetMirrorGrid(m_textBox.Text);

                    m_gridMirror.Show();
                    m_gridOrigin.Hide();
                    m_treeMirror.Hide();
                    m_treeOrigin.Hide();
                }
                else
                {
                    ClearMirrorGrid();
                    //ResetMirrorTree(m_textBox.Text);

                    // 텍스트가 포함되는 트리의 레벨 및 상위 레벨만 꺼내옴
                    m_treeMirror.Nodes.Clear();
                    FindNodes(m_textBox.Text);

                    if (m_treeMirror.Nodes.Count == 0 && m_treeOrigin.Nodes.Count > 0)
                    {
                        AddNodeToMirror(m_treeOrigin.Nodes[0]);
                    }

                    //m_treeMirror.SelectedNode = m_treeMirror.Nodes[0];
                    m_treeMirror.ExpandAll();

                    m_treeOrigin.Hide();
                    m_treeMirror.Show();
                    m_gridOrigin.Hide();
                    m_gridMirror.Hide();
                }
            }
        }

        // 텍스트가 포함되는 트리의 레벨 및 상위 레벨만 꺼내옴
        private void FindNodes(string str, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
                nodes = this.m_treeOrigin.Nodes;

            foreach (TreeNode node in nodes)
            {
                if (node.Text.Contains(str))
                    AddNodeToMirror(node);

                FindNodes(str, node.Nodes);
            }
        }

        private void ClearMirrorGrid()
        {
            int nRowCount = m_gridMirror.Rows.Count;

            for (int i=0;i<nRowCount;i++)
            {
                DataGridViewRow row = m_gridMirror.Rows[0];

                if (row.IsNewRow)
                    continue;
                else
                    m_gridMirror.Rows.RemoveAt(0);
            }
        }

        private void ResetMirrorGrid(string str)
        {
            int nColumnCount = m_gridOrigin.Columns.Count;

            foreach (DataGridViewRow row in m_gridOrigin.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells[0].Value.ToString().Contains(str))
                {
                    DataGridViewRow row2 = MakeNewRow(m_gridMirror);

                    for (int i=0;i<nColumnCount;i++)
                    {
                        row2.Cells[i].Value = row.Cells[i].Value;
                        row2.Cells[i].Tag = row.Cells[i].Tag;
                    }

                    row2.Tag = row.Tag;
                }
            }
        }

        private void ResetMirrorTree(string str, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
                nodes = this.m_treeOrigin.Nodes;

            foreach (TreeNode node in nodes)
            {
                if (node.Text.Contains(str))
                    AddNodeToMirror(node);

                ResetMirrorTree(str, node.Nodes);
            }
        }

        // Mirror tree에 새로운 트리 구조 생성(text가 포함되는 레벨만)
        private void AddNodeToMirror(TreeNode node)
        {
            List<TreeNode> nodeFamily = new List<TreeNode>();

            int nNodeCount = 1;
            nodeFamily.Add(node);

            while (node.Parent != null)
            {
                node = node.Parent;
                nodeFamily.Add(node);
                nNodeCount++;
            }

            TreeNodeCollection nodes = m_treeMirror.Nodes;

            for (int i = nNodeCount - 1; i >= 0; i--)
            {
                node = nodeFamily[i];
                nodes = FindNAdd(nodes, node);
            }
        }

        private TreeNodeCollection FindNAdd(TreeNodeCollection nodes, TreeNode node)
        {
            foreach (TreeNode node2 in nodes)
            {
                if (node2.Tag != null && (int)node2.Tag == (int)node.Tag)
                    return node2.Nodes;
            }

            TreeNode node3 = new TreeNode();

            node3.Text = node.Text;
            node3.Tag = node.Tag;
            nodes.Add(node3);

            return node3.Nodes;
        }

        private void treeViewMirror_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (m_treeMirror.SelectedNode == null)
                m_treeOrigin.SelectedNode = null;
            else
                SelectNode((int)m_treeMirror.SelectedNode.Tag, m_treeOrigin);

            m_parent.NodeSelected(e.Node);
        }

        private void treeViewOrigin_AfterSelect(object sender, TreeViewEventArgs e)
        {
            m_parent.NodeSelected(e.Node);
        }

        private bool SelectNode(int nTag, TreeView tree, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
                nodes = tree.Nodes;

            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && (int)node.Tag == nTag)
                {
                    tree.SelectedNode = node;
                    return true;
                }

                if (SelectNode(nTag, tree, node.Nodes))
                    return true;
            }

            return false;
        }

        public void Refresh(bool gridMode)
        {
            m_gridMode = gridMode;
            textBox_KeyPress(null, null);
        }
    }
}
