using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace TeamManagementSystem
{
    public partial class MDIParent : Form
    {
        private int childFormNumber = 0;
        
        public XtremeCommandBars.CommandBarPopup ControlFile;
        public XtremeCommandBars.CommandBarsGlobalSettings CommandBarsGlobalSettings;
        private XtremeCommandBars.CommandBarControl m_ctrlCheckBox;
        
        private XtremeCommandBars.CommandBarControl m_ctrlRegular;
        private XtremeCommandBars.CommandBarControl m_ctrlWeekday;
        private XtremeCommandBars.CommandBarControl m_ctrlWeekend;
        private XtremeCommandBars.CommandBarControl m_ctrlBoth;

        protected string m_strSkinFolder;

        private FormMain m_frmMain = new FormMain();
        private WebDBManager m_dbMgr = null;

        private ArrayList m_arrTeamVersion = new ArrayList();
        private ArrayList m_arrMember = new ArrayList();
        private ArrayList m_arrRegular = new ArrayList();
        private ArrayList m_arrOrgani = new ArrayList();
        private ArrayList m_arrNormal = new ArrayList();
        private ArrayList m_arrEmergency = new ArrayList();

        private bool m_isActivate = false;

        //private Dictionary<int, Data_CompanyMemberHistory> m_dicMember = new Dictionary<int, Data_CompanyMemberHistory>();
        private Dictionary<int, Data_OrganizationHistory> m_dicMember = new Dictionary<int, Data_OrganizationHistory>();
        private Dictionary<int, Data_NormalHistory> m_dicNormal = new Dictionary<int, Data_NormalHistory>();
        private Dictionary<int, Data_EmergencyHistory> m_dicEmergency = new Dictionary<int, Data_EmergencyHistory>();

        public MDIParent(string[] args)
        {
            InitializeComponent();
            
            m_strSkinFolder = StylesPath();
            SkinLoad();

            if (args.Count() == 0)
            {
                MessageBox.Show("입력 Parameter가 없습니다.\r\n프로그램을 시작할 수 없습니다.");
                throw new Exception();
            }
            else
                m_frmMain.LoginID = int.Parse(args[0]);
        }

        public void GetTeamVersion(ref ArrayList arrTeamVersion)
        {
            //// 가장 최근버전을 읽어온다.
            //string strSQL = "SELECT * FROM TeamVersion where CreateTime = (SELECT MAX(CreateTime) FROM TeamVersion)";

            string strSQL = "SELECT ID, VersionName, GenUserID, MemberName, CreateTime, Description FROM View_TeamVersion ORDER BY ID DESC";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            DateTime dtDefault = new DateTime();
            if (arrResult.Count > 0)
            {
                for (int i = 0; i < arrResult.Count - 5; i += 6)
                {
                    Data_TeamVersion dataNew = new Data_TeamVersion();

                    dataNew.VersionID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                    dataNew.VersionName = m_dbMgr.GetStringField(arrResult[i + 1], "");
                    dataNew.GenUserID = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                    dataNew.UserName = m_dbMgr.GetStringField(arrResult[i + 3], "");
                    dataNew.CreateTime = m_dbMgr.GetDateTimeField(arrResult[i + 4].ToString(), dtDefault);
                    dataNew.Description = m_dbMgr.GetStringField(arrResult[i + 5], "");

                    arrTeamVersion.Add(dataNew);
                }
            }
        }
        
        public void GetCompanyMember(ref ArrayList arrTeamVersion)
        {
            string strSQL = "SELECT ID, MemberName, RegularTeamID, LevelID, PositionID, MemberID, SecondRegularTeamID, SecondPositionID FROM CompanyMember";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult.Count > 0)
            {
                for (int i = 0; i < arrResult.Count - 7; i += 8)
                {
                    Data_CompanyMember dataNew = new Data_CompanyMember();

                    dataNew.ID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                    dataNew.MemberName = m_dbMgr.GetStringField(arrResult[i + 1], "");
                    dataNew.RegularTeamID = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                    dataNew.LevelID = m_dbMgr.GetIntField(arrResult[i + 3].ToString(), 0);
                    dataNew.PositionID = m_dbMgr.GetIntField(arrResult[i + 4].ToString(), 0);
                    dataNew.MemberID = m_dbMgr.GetStringField(arrResult[i + 5], "");
                    dataNew.SecondRegularTeamID = m_dbMgr.GetIntField(arrResult[i + 6].ToString(), 0);
                    dataNew.SecondPositionID = m_dbMgr.GetIntField(arrResult[i + 7].ToString(), 0);
                    

                    //m_dicMember[dataNew.ID] = dataNew;
                    arrTeamVersion.Add(dataNew);
                }
            }
        }

        public void GetRegularTeam(ref ArrayList arrRegular)
        {
            string strSQL = "SELECT ID, TeamName, ParentTeamID FROM RegularTeam";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 2; i += 3)
            {
                Data_RegularTeam dataNew = new Data_RegularTeam();

                dataNew.ID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                dataNew.TeamName = m_dbMgr.GetStringField(arrResult[i + 1], "");
                dataNew.ParentTeamID = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                
                arrRegular.Add(dataNew);
            }
        }

        public void GetOrganigation(ref ArrayList arrInfo)
        {
            string strSQL = "SELECT ID, MemberName ,RegularTeamID, TeamName, ParentTeamID, LevelID, LevelName, MemberID, SecondRegularTeamID, SecondPositionID, PositionID, PositionName FROM View_Organization ORDER BY ID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 11; i += 12)
            {
                Data_Organization dataNew = new Data_Organization();

                dataNew.CompanyMemberID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                dataNew.MemberName = m_dbMgr.GetStringField(arrResult[i + 1], "");
                dataNew.RegularTeamID = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.TeamName = m_dbMgr.GetStringField(arrResult[i + 3], "");
                dataNew.ParentID = m_dbMgr.GetIntField(arrResult[i + 4].ToString(), 0);
                dataNew.LevelID = m_dbMgr.GetIntField(arrResult[i + 5].ToString(), 0);
                dataNew.LevelName = m_dbMgr.GetStringField(arrResult[i + 6], "");
                dataNew.MemberID = m_dbMgr.GetIntField(arrResult[i + 7].ToString(), 0);
                dataNew.PositionID = m_dbMgr.GetIntField(arrResult[i + 8].ToString(), 0);
                dataNew.PositionName = m_dbMgr.GetStringField(arrResult[i + 9], "");
                dataNew.SecondRegularTeamID = m_dbMgr.GetIntField(arrResult[i + 10].ToString(), 0);
                dataNew.SecondPositionID = m_dbMgr.GetIntField(arrResult[i + 11].ToString(), 0);

                arrInfo.Add(dataNew);
            }
        }

        public void GetOrganigationHistory(ref ArrayList arrOrgani)
        {
            string strSQL = "SELECT ID, MemberName, RegularTeamID, TeamName, ParentTeamID, LevelID, LevelName, MemberID, SecondRegularTeamID, SecondPositionID, PositionID, PositionName FROM View_OrganizationHistory ORDER BY ID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 11; i += 12)
            {
                Data_OrganizationHistory dataNew = new Data_OrganizationHistory();

                dataNew.CompanyMemberID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                dataNew.MemberName = m_dbMgr.GetStringField(arrResult[i + 1], "");
                dataNew.RegularTeamID = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.TeamName = m_dbMgr.GetStringField(arrResult[i + 3], "");
                dataNew.ParentTeamID = m_dbMgr.GetIntField(arrResult[i + 4].ToString(), 0);
                dataNew.LevelID = m_dbMgr.GetIntField(arrResult[i + 5].ToString(), 0);
                dataNew.LevelName = m_dbMgr.GetStringField(arrResult[i + 6], "");
                dataNew.MemberID = m_dbMgr.GetIntField(arrResult[i + 7].ToString(), 0);
                dataNew.SecondRegularTeamID = m_dbMgr.GetIntField(arrResult[i + 8].ToString(), 0);
                dataNew.SecondPositionID = m_dbMgr.GetIntField(arrResult[i + 9].ToString(), 0);
                dataNew.PositionID = m_dbMgr.GetIntField(arrResult[i + 10].ToString(), 0);
                dataNew.PositionName = m_dbMgr.GetStringField(arrResult[i + 11], "");
               
                m_dicMember[dataNew.CompanyMemberID] = dataNew;
                arrOrgani.Add(dataNew);
            }
        }

        public void GetNormalTeamHistory(ref ArrayList arrNormal)
        {
            string strSQL = "SELECT ID, TeamName, ParentTeamID, GroupName, Description, RegularTeamLink, TeamVersionID, VersionName FROM View_NormalTeamHistory";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 7; i += 8)
            {
                Data_NormalHistory dataNew = new Data_NormalHistory();

                dataNew.ID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                dataNew.TeamName = m_dbMgr.GetStringField(arrResult[i + 1], "");
                dataNew.ParentTeamID = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.GroupName = m_dbMgr.GetStringField(arrResult[i + 3], "");
                dataNew.Description = m_dbMgr.GetStringField(arrResult[i + 4], "");
                dataNew.RegularTeamLink = m_dbMgr.GetStringField(arrResult[i + 5], "");
                dataNew.TeamVersionID = m_dbMgr.GetIntField(arrResult[i + 6].ToString(), 0);
                dataNew.TeamVersionName = m_dbMgr.GetStringField(arrResult[i + 7], "");

                m_dicNormal[dataNew.ID] = dataNew;
                arrNormal.Add(dataNew);
            }
        }

        public void GetEmergencyTeam(ref ArrayList arrEmergency)
        {
            string strSQL = "SELECT ID, TeamName, ParentTeamID, GroupName, Description, RegularTeamLink, TeamVersionID, VersionName FROM View_EmergencyTeamHistory";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 7; i += 8)
            {
                Data_EmergencyHistory dataNew = new Data_EmergencyHistory();

                dataNew.ID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                dataNew.TeamName = m_dbMgr.GetStringField(arrResult[i + 1], "");
                dataNew.ParentTeamID = m_dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.GroupName = m_dbMgr.GetStringField(arrResult[i + 3], "");
                dataNew.Description = m_dbMgr.GetStringField(arrResult[i + 4], "");
                dataNew.RegularTeamLink = m_dbMgr.GetStringField(arrResult[i + 5], "");
                dataNew.TeamVersionID = m_dbMgr.GetIntField(arrResult[i + 6].ToString(), 0);
                dataNew.TeamVersionName = m_dbMgr.GetStringField(arrResult[i + 7], "");

                m_dicEmergency[dataNew.ID] = dataNew;
                arrEmergency.Add(dataNew);
            }
        }

        //////////////////////////////////////////////////////////////////////////
        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "창 " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }
        //////////////////////////////////////////////////////////////////////////

        private void SkinLoad()
        {
            axSkinFramework.LoadSkin(m_strSkinFolder + "Vista.cjstyles", "");
            axSkinFramework.ApplyWindow(this.Handle.ToInt32());
            this.BackColor = axSkinFramework.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BTNFACE);
        }

        public string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");

            return strExePath + "\\Styles\\";
        }

        private void MDIParent_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void MDIParent_Load(object sender, EventArgs e)
        {
            CreateRibbonBar();
            LoadIcons();
            RibbonBar().EnableFrameTheme();

            axCommandBars.Options.KeyboardCuesShow = XtremeCommandBars.XTPKeyboardCuesShow.xtpKeyboardCuesShowWindowsDefault;

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is MdiClient)
                {
                    axCommandBars.SetMDIClient(ctrl.Handle.ToInt32());
                }
            }

            axCommandBars.EnableCustomization(true);

            //axCommandBars.FindControl(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLESCENIC, true, true).Execute();

            m_dbMgr = new WebDBManager(m_frmMain);
            GetTeamVersion(ref m_arrTeamVersion);
            ReadData();
        }
        private void Init()
        {
            m_arrMember = new ArrayList();
            m_arrRegular = new ArrayList();
            m_arrOrgani = new ArrayList();
            m_arrNormal = new ArrayList();
            m_arrEmergency = new ArrayList();
        }

        private void ReadData()
        {
            Init();

            GetCompanyMember(ref m_arrMember);
            GetRegularTeam(ref m_arrRegular);
            GetOrganigationHistory(ref m_arrOrgani);
            GetNormalTeamHistory(ref m_arrNormal);
            GetEmergencyTeam(ref m_arrEmergency);

            m_frmMain.TeamVersion = m_arrTeamVersion;
            m_frmMain.CompanyMember = m_arrMember;
            m_frmMain.RegularTeam = m_arrRegular;
            m_frmMain.Organigation = m_arrOrgani;
            m_frmMain.NormalTeam = m_arrNormal;
            m_frmMain.EmergencyTeam = m_arrEmergency;

            m_frmMain.DictionaryMember = m_dicMember;
            m_frmMain.DictionaryNormal = m_dicNormal;
            m_frmMain.DictionaryEmergency = m_dicEmergency;

            m_frmMain.NormalCount = m_arrNormal.Count;
            m_frmMain.EmergencyCount = m_arrEmergency.Count;
        }

        private void MDIParent_Activated(object sender, EventArgs e)
        {
            if (!m_isActivate)
            {
                m_isActivate = true;
                m_frmMain.GetVersion(false, this);
                CreateMainSub();
                SelectCheck(ID.ID_FILE_REGULAR);
            }
        }

        private void CreateMainSub()
        {
            m_frmMain.MdiParent = this;
            m_frmMain.WindowState = FormWindowState.Maximized;
            m_frmMain.Show();
            m_frmMain.Text = "";

            //m_frmMain.CreatePane();
            this.Refresh();
        }
		public static Bitmap GetImageByName(string imageName)
		{
			//System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("neutral");
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			string resourceName = "TeamManagementSystem.Properties.Resources";
			var rm = new System.Resources.ResourceManager(resourceName, asm);
			return (Bitmap)rm.GetObject(imageName);
		}

		private void AddBitmapFormRes(string name, object id)
		{
			string szName = name.Replace("-", "_");
			Bitmap bImage = GetImageByName(szName);
			axCommandBars.Icons.AddBitmap(bImage.GetHbitmap().ToInt32(), id, XtremeCommandBars.XTPImageState.xtpImageNormal, true);
		}

        private void LoadIcons()
        {
            axCommandBars.Options.UseSharedImageList = false;
            //string resDir = ResourcePath();
            //axCommandBars.Icons.LoadBitmap(resDir + "SmallIcons.png", new int[] { ID.ID_FILE_NEW, ID.ID_FILE_OPEN, ID.ID_FILE_SAVE, ID.ID_EDIT_CUT, ID.ID_EDIT_COPY, ID.ID_EDIT_PASTE, ID.ID_EDIT_UNDO, ID.ID_EDIT_REDO, ID.ID_FILE_PRINT, ID.ID_APP_ABOUT }, XtremeCommandBars.XTPImageState.xtpImageNormal);
            //axCommandBars.Icons.LoadBitmap(resDir + "LargeIcons.png", new int[] { ID.ID_FILE_NEW, ID.ID_FILE_OPEN, ID.ID_FILE_SAVE, ID.ID_EDIT_SETUP, ID.ID_FILE_CLOSE,
            //    ID.ID_FILE_REGULAR, ID.ID_FILE_WEEKDAY, ID.ID_FILE_WEEKEND, ID.ID_FILE_BOTH, ID.ID_EDIT_TEAM_ADD, ID.ID_EDIT_TEAM_DEL }, XtremeCommandBars.XTPImageState.xtpImageNormal);
			AddBitmapFormRes("LargeIcons", new int[] { ID.ID_FILE_NEW, ID.ID_FILE_OPEN, ID.ID_FILE_SAVE, ID.ID_EDIT_SETUP, ID.ID_FILE_CLOSE,
                ID.ID_FILE_REGULAR, ID.ID_FILE_WEEKDAY, ID.ID_FILE_WEEKEND, ID.ID_FILE_BOTH, ID.ID_EDIT_TEAM_ADD, ID.ID_EDIT_TEAM_DEL });

            XtremeCommandBars.ToolTipContext ToolTipContext = null;
            ToolTipContext = axCommandBars.ToolTipContext;
            ToolTipContext.Style = XtremeCommandBars.XTPToolTipStyle.xtpToolTipResource;
            ToolTipContext.ShowTitleAndDescription(true, XtremeCommandBars.XTPToolTipIcon.xtpToolTipIconNone);
            ToolTipContext.SetMargin(2, 2, 2, 2);
            ToolTipContext.MaxTipWidth = 180;
        }

        private void CreateRibbonBar()
        {
            XtremeCommandBars.RibbonTab TabHome = null;
            XtremeCommandBars.RibbonTab TabEdit = null;

            //XtremeCommandBars.RibbonTab TabPrintPreview = null;
            XtremeCommandBars.RibbonGroup GroupFile = null;
            XtremeCommandBars.RibbonGroup GroupOrganization = null;
            XtremeCommandBars.RibbonGroup GroupEditing = null;
            //XtremeCommandBars.RibbonGroup GroupPrint = null;
            //XtremeCommandBars.RibbonGroup GroupPageSetup = null;
            //XtremeCommandBars.RibbonGroup GroupZoom = null;
            //XtremeCommandBars.RibbonGroup GroupPreview = null;
            
            //XtremeCommandBars.CommandBarPopup ControlSaveAs = null;
            //XtremeCommandBars.CommandBarPopup ControlPrint = null;
            XtremeCommandBars.CommandBarControl Control = null;
            //XtremeCommandBars.CommandBarPopup ControlMargins = null;
            //XtremeCommandBars.CommandBarPopup ControlOrientation = null;
            //XtremeCommandBars.CommandBarPopup ControlSize = null;
            //XtremeCommandBars.CommandBarPopup ControlTeamAdd = null;
            //XtremeCommandBars.CommandBarPopup ControlTeamDel = null;
            //XtremeCommandBars.CommandBarPopup ControlTeamMemberAdd = null;
            //XtremeCommandBars.CommandBarPopup ControlTeamMemberDel = null;
            

            XtremeCommandBars.RibbonBar RibbonBar = null;
            RibbonBar = axCommandBars.AddRibbonBar("The Ribbon");
            RibbonBar.EnableDocking(XtremeCommandBars.XTPToolBarFlags.xtpFlagStretched);

            ControlFile = RibbonBar.AddSystemButton();
            ControlFile.Caption = "&File";
            ControlFile.IconId = ID.ID_SYSTEM_ICON;
            ControlFile.CommandBar.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_NEW, "&New", false, false);
            ControlFile.CommandBar.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_OPEN, "&Open...", false, false);
            //Control = ControlFile.CommandBar.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_PRINT_SETUP, "Pr&int Setup...", false, false);
            //Control.BeginGroup = true;
            Control = ControlFile.CommandBar.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_MRU_FILE1, "Recent File", false, false);
            Control.BeginGroup = true;
            Control.Enabled = false;
            Control = ControlFile.CommandBar.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_APP_EXIT, "E&xit", false, false);
            Control.BeginGroup = true;
            ControlFile.CommandBar.SetIconSize(32, 32);

            TabHome = RibbonBar.InsertTab(0, "&Home");
            TabHome.Id = ID.ID_TAB_HOME;

            GroupFile = TabHome.Groups.AddGroup("File", ID.ID_GROUP_FILE);
            GroupFile.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_NEW, "&새 프로젝트", false, false);
            GroupFile.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_OPEN, "&프로젝트 열기", false, false);
            GroupFile.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_SAVE, "&프로젝트 저장", false, false);
            GroupFile.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_EDIT_SETUP, "&환경설정", false, false);
            GroupFile.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_CLOSE, "&프로젝트 종료", false, false);

            GroupOrganization = TabHome.Groups.AddGroup("Organization Chart", ID.ID_GROUP_FILE);
            m_ctrlRegular = GroupOrganization.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_REGULAR, "&상시조직도", false, false);
            m_ctrlWeekday = GroupOrganization.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_WEEKDAY, "&평일 비상조직도", false, false);
            m_ctrlWeekend = GroupOrganization.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_WEEKEND, "&휴일 비상조직도", false, false);
            m_ctrlBoth = GroupOrganization.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_FILE_BOTH, "&상시&&비상 조직도", false, false);

            GroupEditing = TabHome.Groups.AddGroup("&Edit", ID.ID_GROUP_EDITING);
            GroupEditing.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_EDIT_TEAM_ADD, "&조직추가", false, false);
            GroupEditing.Add(XtremeCommandBars.XTPControlType.xtpControlButton, ID.ID_EDIT_TEAM_DEL, "&조직삭제", false, false);
            GroupEditing.Add(XtremeCommandBars.XTPControlType.xtpControlLabel, ID.ID_NONE, "&", false, false);
            GroupEditing.Add(XtremeCommandBars.XTPControlType.xtpControlLabel, ID.ID_NONE, "&", false, false);
            m_ctrlCheckBox = GroupEditing.Add(XtremeCommandBars.XTPControlType.xtpControlCheckBox, ID.ID_EDIT_MODE, "&편집모드", false, false);
        }

        private XtremeCommandBars.RibbonBar RibbonBar()
        {
            return (XtremeCommandBars.RibbonBar)axCommandBars.ActiveMenuBar;
        }

        private void axCommandBars_Customization(object sender, AxXtremeCommandBars._DCommandBarsEvents_CustomizationEvent e)
        {
        }

        private void axCommandBars_Execute(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
        {
            switch (e.control.Id)
            {
                case (int)XtremeCommandBars.XTPCommandBarsSpecialCommands.XTP_ID_RIBBONCUSTOMIZE:
                    axCommandBars.ShowCustomizeDialog(3);
                    break;
                //case ID.ID_APP_ABOUT:
                //    axCommandBars.ShowAboutBox();
                //    break;
                case ID.ID_FILE_NEW:
                    m_frmMain.SectionIndex = 1;
                    m_frmMain.NewFile();
                    break;
                case ID.ID_APP_EXIT:
                    this.Close();
                    break;
                case (int)XtremeCommandBars.XTPCommandBarsSpecialCommands.XTP_ID_RIBBONCONTROLTAB:
                    System.Diagnostics.Debug.WriteLine("Selected Tab has Changed");
                    //XtremeCommandBars.RibbonBar ribbon = RibbonBar();
                    //if (ribbon.SelectedTab.Index == 0)
                    //    m_frmMain.ReadOnly(true);
                    //else
                    //    m_frmMain.ReadOnly(false);
                    break;
                case ID.ID_FILE_CLOSE:
                    Application.Exit();
                    //this.ActiveMdiChild.Close();
                    break;
                case ID.ID_FILE_SAVE:
                //    SaveFileDialog SaveDialog = new SaveFileDialog();
                //    SaveDialog.ShowDialog(axCommandBars);
                    FormSaveTeamVersion frm = new FormSaveTeamVersion(m_frmMain);

                    if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        m_frmMain.Save_OrganizationChart(frm.VersionID, frm.VersionName, frm.Description);
                    }
                    break;
                case ID.ID_FILE_OPEN:
                    m_arrTeamVersion.Clear();
                    GetTeamVersion(ref m_arrTeamVersion);
                    ReadData();
                    m_frmMain.GetVersion(true, this);
                    break;
                case ID.ID_EDIT_TEAM_ADD:
                    if(m_frmMain.EditMode)
                        m_frmMain.AddSection(m_frmMain.TeamMode);
                    m_frmMain.SectionIndex++;
                    break;
                case ID.ID_EDIT_TEAM_DEL:
                    if (m_frmMain.EditMode )
                    {
                        m_frmMain.DeleteSection(m_frmMain.TeamMode);
                    }
                    break;
                case ID.ID_FILE_REGULAR:
                    m_frmMain.TeamMode = 0;
                    m_frmMain.Organization_Regular();
                    SelectCheck(e.control.Id);
                    break;
                case ID.ID_FILE_BOTH:
                    //m_frmMain.TeamMode = 3;
                    //m_frmMain.SelectMode();
                    m_frmMain.Organization_Both();
                    SelectCheck(e.control.Id);
                    break;
                case ID.ID_FILE_WEEKDAY:
                    m_frmMain.TeamMode = 1;
                    m_frmMain.Weekday = true;
                    m_frmMain.Weekend = false;
                    m_frmMain.Check_Weekday();
                    SelectCheck(e.control.Id);
                    break;
                case ID.ID_FILE_WEEKEND:
                    m_frmMain.TeamMode = 2;
                    m_frmMain.Weekday = false;
                    m_frmMain.Weekend = true;
                    m_frmMain.Check_Weekend();
                    SelectCheck(e.control.Id);
                    break;
                case ID.ID_EDIT_MODE:
                    e.control.Checked = !e.control.Checked;
                    m_frmMain.EditMode = e.control.Checked;
                    
                    if (!e.control.Checked)
                        m_frmMain.ReadOnly(true);
                    else
                        m_frmMain.ReadOnly(false);
                    break;
            };
        }

        private void axCommandBars_UpdateEvent(object sender, AxXtremeCommandBars._DCommandBarsEvents_UpdateEvent e)
        {
            switch (e.control.Id)
            {
                case ID.ID_VIEW_STATUS_BAR:
                    e.control.Checked = axCommandBars.StatusBar.Visible;
                    break;
                case ID.ID_FILE_PRINT_PREVIEW:
                case ID.ID_FILE_PRINT:
                case ID.ID_FILE_CLOSE:
                case ID.ID_FILE_SAVE:
                case ID.ID_WINDOW_ARRANGE:
                case ID.ID_WINDOW_NEW:
                case ID.ID_WINDOW_SWITCH:
                    e.control.Enabled = (this.MdiChildren.Length != 0 ? true : false);
                    break;
                case (int)XtremeCommandBars.XTPCommandBarsSpecialCommands.XTP_ID_RIBBONCONTROLTAB:
					if (RibbonBar().FindTab(ID.ID_TAB_PRINT_PREVIEW) != null)
					{
						if (RibbonBar().FindTab(ID.ID_TAB_PRINT_PREVIEW).Visible == true)
						{
							if (RibbonBar().FindTab(ID.ID_TAB_EDIT) != null)
								RibbonBar().FindTab(ID.ID_TAB_EDIT).Visible = false;
						}
						else if (this.MdiChildren.Length != 0)
						{
							RibbonBar().FindTab(ID.ID_TAB_EDIT).Visible = (this.MdiChildren.Length != 0 ? true : false);
						}
					}
                    break;
                case ID.ID_EDIT_REPLACE:
                case ID.ID_EDIT_FIND:
                case ID.ID_EDIT_SELECT_ALL:
                    if (this.MdiChildren.Length == 0)
                    {
                        e.control.Enabled = false;
                    }
                    else
                    {
                        System.Windows.Forms.RichTextBox rtfText = (System.Windows.Forms.RichTextBox)this.ActiveMdiChild.Controls[0];
                        e.control.Enabled = rtfText.CanSelect;
                    }
                    break;
                case ID.ID_EDIT_CUT:
                case ID.ID_EDIT_COPY:
                    if (this.MdiChildren.Length == 0)
                    {
                        e.control.Enabled = false;
                    }
                    else
                    {
                        System.Windows.Forms.RichTextBox rtfText = (System.Windows.Forms.RichTextBox)this.ActiveMdiChild.Controls[0];
                        e.control.Enabled = (rtfText.SelectionLength == 0 ? false : true);
                    }
                    break;
                case ID.ID_EDIT_UNDO:
                    if (this.MdiChildren.Length == 0)
                    {
                        e.control.Enabled = false;
                    }
                    else
                    {
                        System.Windows.Forms.RichTextBox rtfText = (System.Windows.Forms.RichTextBox)this.ActiveMdiChild.Controls[0];
                        e.control.Enabled = rtfText.CanUndo;
                    }
                    break;
                case ID.ID_EDIT_PASTE:
                case ID.ID_EDIT_PASTE_SPECIAL:
                    if (this.MdiChildren.Length == 0)
                    {
                        e.control.Enabled = false;
                    }
                    else
                    {
                        System.Windows.Forms.RichTextBox rtfText = (System.Windows.Forms.RichTextBox)this.ActiveMdiChild.Controls[0];
                        System.Windows.Forms.DataFormats.Format myFormat = System.Windows.Forms.DataFormats.GetFormat(DataFormats.Text);
                        e.control.Enabled = rtfText.CanPaste(myFormat);
                    }
                    break;

//                 case ID.ID_FILE_REGULAR:
//                     e.control.Checked = true;
//                     break;
//                 case ID.ID_FILE_WEEKDAY:
//                     e.control.Checked = true;
//                     break;
//                 case ID.ID_FILE_WEEKEND:
//                     e.control.Checked = true;
//                     break;
//                 case ID.ID_FILE_BOTH:
//                     e.control.Checked = true;
//                     break;
            };
        }

        public string ResourcePath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            if (System.IO.Directory.Exists(strExePath + "\\..\\res"))
                return strExePath + "\\..\\res\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\..\\res"))
                return strExePath + "\\..\\..\\res\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\res"))
                return strExePath + "\\..\\..\\..\\res\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\..\\src\\SOP2\\TeamManagementSystem\\TeamManagementSystem\\res"))
            {
                int nIndex = strExePath.LastIndexOf('\\');
                string strTemp = strExePath.Substring(0, nIndex);
                nIndex = strTemp.LastIndexOf('\\');
                string strTemp2 = strTemp.Substring(0, nIndex);
                strExePath = strTemp2 + "\\src\\SOP2\\TeamManagementSystem\\TeamManagementSystem\\res\\";
                return strExePath;
            }

            return strExePath + "\\res\\";
        }

        private void SelectCheck(int nCtrl)
        {
            m_ctrlRegular.Checked = false;
            m_ctrlWeekday.Checked = false;
            m_ctrlWeekend.Checked = false;
            m_ctrlBoth.Checked = false;

            switch(nCtrl)
            {
                case ID.ID_FILE_REGULAR:
                    m_ctrlRegular.Checked = true;
                    break;
                case ID.ID_FILE_WEEKDAY:
                    m_ctrlWeekday.Checked = true;
                    break;
                case ID.ID_FILE_WEEKEND:
                    m_ctrlWeekend.Checked = true;
                    break;
                case ID.ID_FILE_BOTH:
                    m_ctrlBoth.Checked = true;
                    if(m_frmMain.Weekday)
                    {
                        m_ctrlWeekday.Checked = true;
                    }
                    else
                    {
                        m_ctrlWeekend.Checked = true;
                    }
                    break;
            }
        }
    }
}
