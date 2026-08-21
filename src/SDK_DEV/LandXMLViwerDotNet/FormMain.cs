using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using XtremeCommandBars;
using System.Collections;
using System.Globalization;
using UBMLViewer.Properties;
using Microsoft.Win32;

namespace UBMLViewer
{
    public partial class FormMain : Form
    {
        private const int WM_SYSCOMMAND = 0x0112;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_RESTORE = 0xF120;
        private const int SC_MAXIMIZE = 0xF030;
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;
        
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = m.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MINIMIZE || command == SC_MAXIMIZE)
                    {
                        bSetRestore = false;
                        bEndRestore = false;
                    }
                    if (command == SC_RESTORE)
                    {
                        bSetRestore = true;
                        bEndRestore = false;
                    }
                    break;
            }
            if (m.Msg == WM_NCLBUTTONDBLCLK)      
            {
                m.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref m);
        }

        public System.Windows.Forms.Timer MainTimer
        {
            get { return m_MainTimer; }
        }
        public AxXtremeSkinFramework.AxSkinFramework SkinFramework
        {
            get { return m_axSkinFramework; }
        }        
        public AxXtremeCommandBars.AxCommandBars CommandBars
        {
            get { return m_axCommandBars; }
        }
        public AxXtremeCommandBars.AxImageManager ImageManager
        {
            get { return m_axImageManager; }
        }
        public System.Windows.Forms.Panel MainPanel
        {
            get { return m_MainPanel; }
        }

        private static FormMain m_Instance = null;
        public static FormMain Instance
        {
            get { return m_Instance; }
        }

        //////////////////////////////////////////////////////
        // window resotre event process
        private bool bSetRestore = false;
        private bool bEndRestore = false;
        private RibbonTab m_CurrentTab = null;
        private RibbonTab m_PrevTab = null;
        private bool bVisiblePane = false;
        private bool bPrevVisibleState = false;
        private bool bCheckRedarw = false;
        // Resize & Restore , BackstateView Open 시 3D뷰 갱신용
        ///////////////////////////////////////////////////////

        private CommandBarPopup m_ControlFile = null;
        private CommandBarPopup m_ControlOptions = null;

        private CommandBarsGlobalSettings CommandBarsGlobalSettings;
        
        private RibbonBackstageTab m_ControlOption = null;
        private RibbonBackstageView m_BackstageView = null;

        private PageBackstageOption m_pageOption = null;
        private PageBackstageHome m_PageHome = null;

        public  XtremeCommandBars.StatusBar m_StatusBar = null;
        
        private StatusBarPane m_StatusPane = null;
        public XtremeCommandBars.StatusBarPane StatusPane
        {
            get { return m_StatusPane; }
        }

        private string m_strSkinFolder = "";

        private int m_nDefaultThemeID = ID.ID_OPTIONS_STYLEOFFCIE2010BLACK;

        
        private CommandBar m_ToolBarView = null;
        
        private CommandBar m_ToolBar3D = null;

        private ArrayList m_arToolBarList = new ArrayList();

        private bool m_bExit = false;

        public FormMain()
        {
            m_Instance = this;

            InitializeComponent();
                        
            LoadIcons();

            //SkinLoad();

            CreateRibbonBar();

            CreateBackstageView();

            CreateBackstageHome();

            CreateStatusBar();

            this.Name = "UBMLViewer";
            this.FormClosing += FormMain_FormClosing;
            this.FormClosed += FormMain_FormClosed;
            this.Load += FormMain_Load;

            this.MouseWheel += new MouseEventHandler(OnMouseWheel);

            ShowSplash();

            AddPythonFunction();
        }

        public bool ShowSplash()
        {
            Splash splash = new Splash(this);
            splash.TopMost = true;
            splash.Show();
            return true;
        }

        public void AddPythonFunction()
        {
            ScriptProxy proxy = ScriptProxy.Instance;
            proxy.UserObject.FileOpen = new Func<bool>(OpenFile);
			proxy.UserObject.DBOpen = new Func<bool>(OpenDB);
            proxy.UserObject.UpdateView = new Func<bool>(UpdateView);
            proxy.UserObject.Update3DView = new Func<bool>(Update3DView);
            proxy.UserObject.FileClose = new Func<bool>(CloseFile);
            proxy.UserObject.ShowSplash = new Func<bool>(ShowSplash);
            proxy.UserObject.Sleep = new Func<int, bool>(Sleep);
        }

        public bool Sleep(int milisecond)
        {
            System.Threading.Thread.Sleep(milisecond);
            return true;
        }

        public static void Call(string szCode)
        {
            ScriptProxy.Instance.Call(szCode);
        }

        public bool CloseFile()
        {
            MessageBox.Show("Close");
            return true;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (m_PageHome != null)
            {
                m_PageHome.OnMouseWheel(sender, e);
            }
        }

        // 스킨 이미지를 사용하는 경우 사용
        private void SkinLoad()
        {
            m_strSkinFolder = StylesPath();
            SkinFramework.LoadSkin(m_strSkinFolder + "WinXP.Luna.cjstyles", "");
            SkinFramework.ApplyWindow(this.Handle.ToInt32());
            this.BackColor = SkinFramework.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BACKGROUND);
        }

        public static string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

            if (System.IO.Directory.Exists(strExePath + "\\Styles"))
                return strExePath + "\\Styles\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\Styles"))
                return strExePath + "\\..\\Styles\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\..\\Styles"))
                return strExePath + "\\..\\..\\Styles\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\Styles"))
                return strExePath + "\\..\\..\\..\\Styles\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\..\\Styles"))
                return strExePath + "\\..\\..\\..\\..\\Styles\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\..\\..\\Styles"))
                return strExePath + "\\..\\..\\..\\..\\..\\Styles\\";

            if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\..\\..\\..\\Styles"))
                return strExePath + "\\..\\..\\..\\..\\..\\..\\Styles\\";

            return strExePath + "\\.\\";
        }

        // 리소스에서 이미지를 로드해주는 함수
        public static Bitmap GetImageByName(string imageName)
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("neutral");
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            string resourceName = "UBMLViewer.Properties.Resources";
            var rm = new System.Resources.ResourceManager(resourceName, asm);
            return (Bitmap)rm.GetObject(imageName);
        }

        private int[] printMenuIcons = 
                    {
                        ID.ID_PREVIEW_PREVIEW_CLOSE,
                        ID.ID_PREVIEW_PREVIEW_MAGNIFIER,
                        ID.ID_PREVIEW_PREVIEW_NEXT,
                        ID.ID_PREVIEW_PREVIEW_RULER,
                        ID.ID_PREVIEW_PAGESETUP_ORIENTATION,   // 5
                        ID.ID_PREVIEW_PREVIEW_SHRINK,
                        ID.ID_PREVIEW_PRINT_OPTIONS,
                        ID.ID_PREVIEW_PRINT_PRINT              // 8
                    };


        private int[] fileMenuIcons = 
                    {  ID.ID_FILE_NEW,     
                       ID.ID_FILE_OPEN,    
                       ID.ID_FILE_SAVE,
                       ID.ID_EDIT_PASTE,
                       ID.ID_EDIT_FIND,    // 5
                       ID.ID_FILE_PRINT,
                       7,8,9,10,11,12,13
                    };


        private void AddBitmapFormRes(string name, object id)
        {
            string szName = name.Replace("-", "_");
            Bitmap bImage = GetImageByName(szName);
            CommandBars.Icons.AddBitmap(bImage.GetHbitmap().ToInt32(), id, XTPImageState.xtpImageNormal, true);        
        }

        private void LoadIcons()
        {
            CommandBars.Options.UseSharedImageList = false;           

            AddBitmapFormRes("LargeIcons", fileMenuIcons);
            AddBitmapFormRes("PrintPreview", printMenuIcons);

            AddBitmapFormRes("document-open",   ID.ID_FILE_OPEN);
            AddBitmapFormRes("document-save",   ID.ID_FILE_SAVE);
            AddBitmapFormRes("document-save-as", ID.ID_FILE_SAVE_AS);
            AddBitmapFormRes("document-new",    ID.ID_FILE_NEW);

            AddBitmapFormRes("edit-copy",   ID.ID_EDIT_COPY);
            AddBitmapFormRes("edit-cut",    ID.ID_EDIT_CUT);
            AddBitmapFormRes("edit-paste",  ID.ID_EDIT_PASTE);
            AddBitmapFormRes("edit-delete", ID.ID_EDIT_DELETE);
            AddBitmapFormRes("edit-find",   ID.ID_EDIT_FIND);

            AddBitmapFormRes("edit-undo", ID.ID_EDIT_UNDO);
            AddBitmapFormRes("edit-redo", ID.ID_EDIT_REDO);

            AddBitmapFormRes("utilities-terminal", ID.ID_EDIT_SCRIPT);

            int [] mode3d = {
                                1,
                                ID.ID_VIEW_TEXTURED,                                
                                ID.ID_VIEW_WIREFRAME,
                                2,3,
                                ID.ID_VIEW_HIDDENLINE,
                                ID.ID_VIEW_SHADING,
                                4,5,6,7
                            };
            AddBitmapFormRes("toolbar_3dview", mode3d);

            int[] view3d = {
                                ID.ID_VIEW_ISO,
                                ID.ID_VIEW_FRONT,
                                ID.ID_VIEW_REAR,
                                ID.ID_VIEW_LEFT,
                                ID.ID_VIEW_RIGHT,
                                ID.ID_VIEW_TOP,
                                ID.ID_VIEW_BOTTOM,
                                ID.ID_VIEW_HOME,
                                1,2
                            };
            AddBitmapFormRes("toolbar_view", view3d);

            int[] edit = {
                             ID.ID_NAVI_PICK,
                             2,3,4,5
                         };
            AddBitmapFormRes("toolbar_edit", edit);

            int[] navi = {
                             ID.ID_NAVI_ZOOMIN,
                             ID.ID_NAVI_ZOOMOUT,
                             ID.ID_NAVI_PAN,
                             ID.ID_SAVE_IMAGE,
                             ID.ID_VIEW_FULLSCREEN,
                             ID.ID_NAVI_ORBIT,
                             ID.ID_NAVI_FPV
                         };
            AddBitmapFormRes("toolbar_navi", navi);
        }

        public RibbonBar RibbonBar()
        {
            return (XtremeCommandBars.RibbonBar)CommandBars.ActiveMenuBar;
        }

        private CommandBarControl AddButton(CommandBarControls Controls, XTPControlType ControlType, int Id, string Caption)
        {
            return AddButton(Controls, ControlType, Id, Caption, false, "");
        }

        private CommandBarControl AddButton(CommandBarControls Controls, XTPControlType ControlType, int Id, string Caption, bool BeginGroup)
        {
            return AddButton(Controls, ControlType, Id, Caption, BeginGroup, "");
        }

        private CommandBarControl AddButton(CommandBarControls Controls, XTPControlType ControlType, int Id, string Caption, bool BeginGroup, string DescriptionText)
        {
            CommandBarControl Control = Controls.Add(ControlType, Id, Caption, -1, false);
            Control.BeginGroup = BeginGroup;
            Control.DescriptionText = DescriptionText;
            return Control;
        }
                
        private void CreateRibbonBar()
        {           
            RibbonBar ribbonBar = null;
            CommandBarPopup ControlPopup = null;

            string szRibbonName = Resources.RibbonName;
            ribbonBar = CommandBars.AddRibbonBar(szRibbonName);
            ribbonBar.EnableDocking(XTPToolBarFlags.xtpFlagStretched);
            ribbonBar.ShowQuickAccess = false;

            m_ControlOptions = (CommandBarPopup)ribbonBar.Controls.Add(XTPControlType.xtpControlPopup, 0, "설정", -1, false);
            m_ControlOptions.Flags = XTPControlFlags.xtpFlagRightAlign;

            ControlPopup = (CommandBarPopup)m_ControlOptions.CommandBar.Controls.Add(XTPControlType.xtpControlPopup, 0, "스타일", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEBLUE, "Office 2007 Blue", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEBLACK, "Office 2007 Black", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLESILVER, "Office 2007 Silver", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEAQUA, "Office 2007 Aqua", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEOFFICE2010SILVER, "Office 2010 Silver", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEOFFCIE2010BLUE, "Office 2010 Blue", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEOFFCIE2010BLACK, "Office 2010 Black", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLESCENIC, "Windows 7 Scenic", -1, false);

            RibbonTab TabMenu = ribbonBar.InsertTab(0, "홈");
            TabMenu.Id = 1000;
            RibbonGroup GroupFile = TabMenu.Groups.AddGroup("파일", 100);
            GroupFile.Add(XTPControlType.xtpControlButton, fileMenuIcons[0], "&New", false, false);
            GroupFile.Add(XTPControlType.xtpControlButton, fileMenuIcons[1], "&Open", false, false);
            GroupFile.Add(XTPControlType.xtpControlButton, fileMenuIcons[2], "&Close", false, false);
                        
            RibbonGroup GroupEdit = TabMenu.Groups.AddGroup("편집", 200);
            //GroupEdit.Add(XTPControlType.xtpControlButton, fileMenuIcons[3], "&Paste", false, false);
            GroupEdit.Add(XTPControlType.xtpControlButton, ID.ID_EDIT_UNDO, "Undo", false, false);
            GroupEdit.Add(XTPControlType.xtpControlButton, ID.ID_EDIT_REDO, "Redo", false, false);


            RibbonGroup GroupScript = TabMenu.Groups.AddGroup("스크립트", 300);
            GroupScript.Add(XTPControlType.xtpControlButton, ID.ID_EDIT_SCRIPT, "Script", false, false);


            CreateViewToolBar();
        }

        public void CreateBackstageHome()
        {
            int left, top, right, bottom;
            CommandBars.GetClientRect(out left, out top, out right, out bottom);
            m_MainPanel.SetBounds(left, top, right - left, bottom - top);
           
            m_PageHome = new PageBackstageHome();
            m_PageHome.Location = new Point(0, 0);
            m_PageHome.Dock = DockStyle.Fill;
            m_PageHome.TopLevel = false;
            m_PageHome.Parent = m_MainPanel;
            
            m_MainPanel.Controls.Add(m_PageHome);
        }

        public void CreateStatusBar()
        {
            m_StatusBar = CommandBars.StatusBar;
            m_StatusBar.Visible = true;
            m_StatusPane = m_StatusBar.AddPane(0);
            m_StatusBar.AddPane(ID.ID_INDICATOR_CAPS);
            m_StatusBar.AddPane(ID.ID_INDICATOR_NUM);
            m_StatusBar.AddPane(ID.ID_INDICATOR_SCRL);
        }

        private void CreateBackstageView()
        {
            // BackstageView 생성
            RibbonBar RibbonBar = (RibbonBar)CommandBars.ActiveMenuBar;
            m_BackstageView = (RibbonBackstageView)CommandBars.CreateCommandBar("CXTPRibbonBackstageView");
            
            // 시스템 버튼 추가
            string szPrefName = Resources.PrefGroup;
            m_ControlFile = RibbonBar.AddSystemButton();
            m_ControlFile.Caption = szPrefName;
            
            // 아이콘을 시스템 아이콘과 통합
            m_ControlFile.IconId = ID.ID_SYSTEM_ICON;
            m_ControlFile.CommandBar = (CommandBar)m_BackstageView;
                        
            // 패널을 탭을 붙인다.     
            if (m_pageOption == null)
                m_pageOption = new PageBackstageOption();
            m_ControlOption = m_BackstageView.AddTab(ID.ID_PANE_OPTION, szPrefName, m_pageOption.Handle.ToInt32());
            m_ControlOption.DefaultItem = true;
            
            // 메뉴를 추가
            string szExit = Resources.ExitButton;
            m_BackstageView.AddCommand(ID.ID_PANE_CLOSE, szExit);            
        }

        private void CreateViewToolBar()
        {
            // 툴바 Add
            string szStdToolBarName = Resources.StdToolBarName;
            m_ToolBarView = CommandBars.Add(szStdToolBarName, XTPBarPosition.xtpBarLeft);         
            AddButton(m_ToolBarView.Controls, XTPControlType.xtpControlButton, ID.ID_NAVI_PICK, "Select");            
            AddButton(m_ToolBarView.Controls, XTPControlType.xtpControlButton, ID.ID_NAVI_PAN, "Pan");
            AddButton(m_ToolBarView.Controls, XTPControlType.xtpControlButton, ID.ID_NAVI_ORBIT, "Oribt");
            AddButton(m_ToolBarView.Controls, XTPControlType.xtpControlButton, ID.ID_NAVI_ZOOMIN, "ZoomIn", true);
            AddButton(m_ToolBarView.Controls, XTPControlType.xtpControlButton, ID.ID_NAVI_ZOOMOUT, "ZoomOut");
            m_ToolBarView.ShowExpandButton = false;
            m_ToolBarView.SetIconSize(24, 24);

            string szViewToolBarName = Resources.ViewToolBarName;
            m_ToolBar3D = CommandBars.Add(szViewToolBarName, XTPBarPosition.xtpBarBottom);          
            AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_TEXTURED, "Texture");
            AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_WIREFRAME, "Wireframe");
            AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_HIDDENLINE, "HiddenLine");
            AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_SHADING, "FlatShading");

            CommandBarControl c = AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_ISO, "ISO", true);
            c.Enabled = false;
            AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_FRONT, "Front");
            AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_REAR, "Rear");
            AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_LEFT, "Left");
            AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_RIGHT, "Right");
            AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_TOP, "Top");
            AddButton(m_ToolBar3D.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_HOME, "Home");



            m_ToolBar3D.ShowExpandButton = false;
            m_ToolBar3D.SetIconSize(24, 24);

        

            m_arToolBarList.Add(m_ToolBarView);
            m_arToolBarList.Add(m_ToolBar3D);
        }
        
        private void CommandBars_UpdateEvent(object sender, AxXtremeCommandBars._DCommandBarsEvents_UpdateEvent e)
        {
            if (m_bExit == true)
                return;

            bCheckRedarw = false; 
            // BackStageOption이 활성 상태인지 검사
            if (m_ControlOptions != null)
            {
                bVisiblePane = m_ControlOptions.Enabled;
                if (bPrevVisibleState == false && bVisiblePane == true)
                {
                    bCheckRedarw = true;
                }
                bPrevVisibleState = bVisiblePane;
            }            

            // 탭이 전환 상태인지 검사
            m_CurrentTab = (RibbonTab)RibbonBar().SelectedTab;
            if (m_CurrentTab != null && m_CurrentTab != m_PrevTab)
            {
                bCheckRedarw = true;                
                m_PrevTab = m_CurrentTab;
            }

            //////////////////////////////////////////////////////////////////
            // Update 루틴은 여기에 작업 할것.
            switch(e.control.Id)
            {
                case ID.ID_VIEW_TEXTURED:
                    e.control.Checked = (ViewMode() == 4);
                    break;                
                case ID.ID_VIEW_SHADING:
                    e.control.Checked = (ViewMode() == 3);
                    break;
                case ID.ID_VIEW_HIDDENLINE:
                    e.control.Checked = (ViewMode() == 2);
                    break;
                case ID.ID_VIEW_WIREFRAME:
                    e.control.Checked = (ViewMode() == 1);
                    break;

                case ID.ID_EDIT_SCRIPT:
                    if (m_PageHome != null)
                        e.control.Checked = m_PageHome.IsShowScriptPane();
                    else
                        e.control.Checked = false;
                    break;
            }
            // Update 루틴 여기까지
            ///////////////////////////////////////////////////////////////////

            // BackStageOption으로 화면이 가려졌다 복구되면 3D 뷰를 갱신해야 한다.
            if (bCheckRedarw == true)
            {
                UpdateView();
            }
        }

        public int ViewMode()
        {
            if (m_PageHome != null && m_PageHome.Visible == true)
            {
                if (m_PageHome.View3D != null && m_PageHome.View3D.Visible == true)
                {
                    return m_PageHome.View3D.ViewMode;
                }
            }
            return -1;
        }

        public static bool UpdateView()
        {
            if (m_Instance.m_PageHome != null && m_Instance.m_PageHome.Visible == true)
            {
                m_Instance.m_PageHome.Refresh();
                m_Instance.m_PageHome.View3D.UpdateWindow();
                return true;
            }
            return false;              
        }

        public static bool Update3DView()
        {
            if (m_Instance.m_PageHome != null && m_Instance.m_PageHome.Visible == true)
            {
                m_Instance.m_PageHome.View3D.RedrawScene();
                return true;
            }
            return false;
        }
        
        
        private void CommandBars_ResizeEvent(object sender, EventArgs e)
        {
            int left, top, right, bottom;
            CommandBars.GetClientRect(out left, out top, out right, out bottom);
            m_MainPanel.SetBounds(left, top, right - left, bottom - top);
        }

        private void CommandBarsMenu_Execute(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
        {
            if (m_bExit == true)
                return;

            switch (e.control.Id)
            {
                case ID.ID_EDIT_SCRIPT:
                    Call("ShowScriptWnd()");
                    break;
                case ID.ID_PANE_CLOSE:                    
                    break;

                case ID.ID_FILE_OPEN:
                    //Call("FileOpen()");    
					Call("DBOpen()");
                    break;

                case ID.ID_VIEW_TEXTURED:
                    if (e.control.Checked == false)
                    {
                        Call("ViewTextured()");                      
                    }                    
                    break;
                case ID.ID_VIEW_HIDDENLINE:
                    if (e.control.Checked == false)
                    {
                        Call("ViewHidden()");
                    }
                    break;
                case ID.ID_VIEW_SHADING:
                    if (e.control.Checked == false)
                    {
                        Call("ViewShading()");
                    }
                    break;
                case ID.ID_VIEW_WIREFRAME:
                    if (e.control.Checked == false)
                    {
                        Call("ViewWire()");
                    }
                    break;
                case ID.ID_VIEW_ISO:
                    
                    break;
                case ID.ID_VIEW_FRONT:
                    Call("ViewFront()");
                    break;
                case ID.ID_VIEW_REAR:
                    Call("ViewRear()");
                    break;
                case ID.ID_VIEW_LEFT:
                    Call("ViewLeft()");
                    break;
                case ID.ID_VIEW_RIGHT:
                    Call("ViewRight()");
                    break;
                case ID.ID_VIEW_TOP:
                    Call("ViewTop()");
                    break;
                case ID.ID_VIEW_BOTTOM:

                    break;
                case ID.ID_VIEW_HOME:
                    Call("ViewHome()");
                    break;
                default:
                    break;
            };            
        }
       
        public bool OpenFile()
        {
            if (m_OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                UseWaitCursor = true;
                string szFile = m_OpenFileDialog.FileName;

                Call("OpenMesh('" + szFile.Replace("\\", "/") + "')");
            
                UseWaitCursor = false;
                return true;
            }
            return false;
        }

		private OracleManager m_OracleDB = null;
		public bool OpenDB()
		{
			if (m_OracleDB == null)
			{
				char[] arrID = new char[] { 'l', 'a', 'n', 'd', 'x', 'm', 'l' };
				char[] arrPW = new char[] { 'l', 'a', 'n', 'd', 'x', 'm', 'l' };
				char[] arrDB = new char[] { 'O', 'R', 'C', 'L' };
				m_OracleDB = new OracleManager(new string(arrID), new string(arrPW), new string(arrDB));

				m_OracleDB.OpenConnection();

				if (m_OpenFileDialog.ShowDialog() == DialogResult.OK)
				{
					string szFile = m_OpenFileDialog.FileName;
					LandXML xml = new LandXML();
					xml.LoadLandXmlFile(szFile, m_OracleDB);
				}

				m_OracleDB.CloseConnection();
				return true;
			}
			return false;
		}

        private void CommandBars_Execute(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
        {
            if (m_bExit == true)
                return;

            switch (e.control.Id)
            {
                case (int)XtremeCommandBars.XTPCommandBarsSpecialCommands.XTP_ID_RIBBONCUSTOMIZE:
                    CommandBars.ShowCustomizeDialog(3);
                    break;

                case ID.ID_OPTIONS_STYLEBLACK:
                    {
                        CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
                        CommandBarsGlobalSettings.ResourceImages.LoadFromFile(StylesPath() + "Office2007.dll", "Office2007Black.ini");
                        m_ControlFile.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonAutomatic;
                        CommandBars.PaintManager.RefreshMetrics();
                        CommandBars.RecalcLayout();
                    }
                    break;
                case ID.ID_OPTIONS_STYLEBLUE:
                    {
                        CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
                        CommandBarsGlobalSettings.ResourceImages.LoadFromFile("", "");
                        m_ControlFile.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonAutomatic;
                        CommandBars.PaintManager.RefreshMetrics();
                        CommandBars.RecalcLayout();
                    }
                    break;
                case ID.ID_OPTIONS_STYLEAQUA:
                    {
                        CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
                        CommandBarsGlobalSettings.ResourceImages.LoadFromFile(StylesPath() + "Office2007.dll", "Office2007Aqua.ini");
                        m_ControlFile.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonAutomatic;
                        CommandBars.PaintManager.RefreshMetrics();
                        CommandBars.RecalcLayout();
                    }
                    break;
                case ID.ID_OPTIONS_STYLESILVER:
                    {
                        CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
                        CommandBarsGlobalSettings.ResourceImages.LoadFromFile(StylesPath() + "Office2007.dll", "Office2007Silver.ini");
                        m_ControlFile.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonAutomatic;
                        CommandBars.PaintManager.RefreshMetrics();
                        CommandBars.RecalcLayout();
                    }
                    break;
                case ID.ID_OPTIONS_STYLEOFFCIE2010BLUE:
                    {
                        CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
                        CommandBarsGlobalSettings.ResourceImages.LoadFromFile(StylesPath() + "Office2010.dll", "Office2010Blue.ini");
                        m_ControlFile.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonCaption;
                        CommandBars.PaintManager.RefreshMetrics();
                        CommandBars.RecalcLayout();
                    }
                    break;
                case ID.ID_OPTIONS_STYLEOFFICE2010SILVER:
                    {
                        CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
                        CommandBarsGlobalSettings.ResourceImages.LoadFromFile(StylesPath() + "Office2010.dll", "Office2010Silver.ini");
                        m_ControlFile.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonCaption;
                        CommandBars.PaintManager.RefreshMetrics();
                        CommandBars.RecalcLayout();
                    }
                    break;
                case ID.ID_OPTIONS_STYLEOFFCIE2010BLACK:
                    {
                        CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
                        CommandBarsGlobalSettings.ResourceImages.LoadFromFile(StylesPath() + "Office2010.dll", "Office2010Black.ini");
                        m_ControlFile.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonCaption;
                        CommandBars.PaintManager.RefreshMetrics();
                        CommandBars.RecalcLayout();
                    }
                    break;
                case ID.ID_OPTIONS_STYLESCENIC:
                    {
                        CommandBarsGlobalSettings = new XtremeCommandBars.CommandBarsGlobalSettings();
                        CommandBarsGlobalSettings.ResourceImages.LoadFromFile(StylesPath() + "Windows7.dll", "Windows7Blue.ini");
                        m_ControlFile.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonCaption;
                        CommandBars.PaintManager.RefreshMetrics();
                        CommandBars.RecalcLayout();
                    }
                    break;
                default:
                    CommandBarsMenu_Execute(sender, e);
                    break;
            };

            if (m_PageHome != null)
            {
                m_PageHome.OnChangeTheme(e.control.Id);
            }
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {

        }
                
        private void FormMain_Load(object sender, EventArgs e)
        {
            Visible = false;

            RibbonBar().EnableFrameTheme();
            CommandBars.EnableCustomization(true);
            CommandBars.FindControl(XtremeCommandBars.XTPControlType.xtpControlButton, m_nDefaultThemeID, true, true).Execute();
                      
            m_PageHome.Show();

            System.Threading.Thread.Sleep(2000);           
            
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_bExit = true;
            m_PageHome.Close();
            CommandBars.DeleteAll();
            m_axCommandBars.Dispose();            
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            if (m_bExit == true)
                return;

            if (WindowState == FormWindowState.Normal && bSetRestore == true)
            {
                bEndRestore = true;
            }

            int left, top, right, bottom;
            CommandBars.GetClientRect(out left, out top, out right, out bottom);

            int height = 0;

            if (bSetRestore == true)
                height = CalcToolBarHeight();

            m_MainPanel.SetBounds(left, top, right - left, bottom - top - height);

            // 리스토어가 종료이면 변수값 설정한다.
            if (bEndRestore == true)
            {
                bSetRestore = false;
                bEndRestore = false;
            }       
        }
        
        private int CalcToolBarHeight()
        {
            int height = 0;
            int left2, top2, right2, bottom2;
            foreach (CommandBar toolbar in m_arToolBarList)
            {
                if (toolbar.Position == XTPBarPosition.xtpBarTop || toolbar.Position == XTPBarPosition.xtpBarBottom)
                {
                    toolbar.GetWindowRect(out left2, out top2, out right2, out bottom2);
                    height += bottom2 - top2;
                }
            }
            return height;
        }                     
       
    }
}
