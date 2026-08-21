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

namespace UBMLViewer
{
    public partial class FormMain : Form
    {  

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

        // window resotre event process
        private bool bSetRestore = false;
        private bool bEndRestore = false;

        public CommandBarPopup m_ControlFile;
        public CommandBarsGlobalSettings CommandBarsGlobalSettings;
        private RibbonBackstageTab m_ControlOption = null;

        private PageBackstageOption m_pageOption = null;
        private PageBackstageHome m_PageHome = null;
        public  XtremeCommandBars.StatusBar m_StatusBar = null;
        private StatusBarPane m_StatusPane = null;
        private string m_strSkinFolder = "";
        private int m_nDefaultThemeID = ID.ID_OPTIONS_STYLEOFFCIE2010BLACK;


        public FormMain()
        {
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

            return strExePath + "\\..\\..\\..\\Styles\\";
        }

        // 리소스에서 이미지를 로드해주는 함수
        public static Bitmap GetImageByName(string imageName)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            string resourceName = asm.GetName().Name + ".Properties.Resources";
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

        private void LoadIcons()
        {
            CommandBars.Options.UseSharedImageList = false;
            //CommandBars.Icons.LoadBitmap(ResourcePath() + "BackstageIcons.png", new int[] { 1, 2, 3, 4 }, XtremeCommandBars.XTPImageState.xtpImageNormal);
            
            
            Bitmap bMenu = GetImageByName("LargeIcons");
            CommandBars.Icons.AddBitmap(bMenu.GetHbitmap().ToInt32(), fileMenuIcons, XTPImageState.xtpImageNormal, true);

            Bitmap bMenu2 = GetImageByName("PrintPreview");
            CommandBars.Icons.AddBitmap(bMenu2.GetHbitmap().ToInt32(), printMenuIcons, XTPImageState.xtpImageNormal, true);
        }

        public RibbonBar RibbonBar()
        {
            return (XtremeCommandBars.RibbonBar)CommandBars.ActiveMenuBar;
        }

        CommandBarControl AddButton(CommandBarControls Controls, XTPControlType ControlType, int Id, string Caption)
        {
            return AddButton(Controls, ControlType, Id, Caption, false, "");
        }
        CommandBarControl AddButton(CommandBarControls Controls, XTPControlType ControlType, int Id, string Caption, bool BeginGroup)
        {
            return AddButton(Controls, ControlType, Id, Caption, BeginGroup, "");
        }
        CommandBarControl AddButton(CommandBarControls Controls, XTPControlType ControlType, int Id, string Caption, bool BeginGroup, string DescriptionText)
        {
            CommandBarControl Control = Controls.Add(ControlType, Id, Caption, -1, false);
            Control.BeginGroup = BeginGroup;
            Control.DescriptionText = DescriptionText;
            return Control;
        }

        private void CreateRibbonBar()
        {
           
            RibbonBar RibbonBar = null;
            CommandBarPopup ControlPopup = null;
            CommandBarPopup ControlOptions = null;

            RibbonBar = CommandBars.AddRibbonBar("The Ribbon");
            RibbonBar.EnableDocking(XTPToolBarFlags.xtpFlagStretched);
            RibbonBar.ShowQuickAccess = false;          
            
            ControlOptions = (CommandBarPopup)RibbonBar.Controls.Add(XTPControlType.xtpControlPopup, 0, "설정", -1, false);
            ControlOptions.Flags = XTPControlFlags.xtpFlagRightAlign;

            ControlPopup = (CommandBarPopup)ControlOptions.CommandBar.Controls.Add(XTPControlType.xtpControlPopup, 0, "스타일", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEBLUE, "Office 2007 Blue", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEBLACK, "Office 2007 Black", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLESILVER, "Office 2007 Silver", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEAQUA, "Office 2007 Aqua", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEOFFICE2010SILVER, "Office 2010 Silver", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEOFFCIE2010BLUE, "Office 2010 Blue", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLEOFFCIE2010BLACK, "Office 2010 Black", -1, false);
            ControlPopup.CommandBar.Controls.Add(XTPControlType.xtpControlButton, ID.ID_OPTIONS_STYLESCENIC, "Windows 7 Scenic", -1, false);

            RibbonTab TabMenu = RibbonBar.InsertTab(0, "&File");
            TabMenu.Id = 1000;
            RibbonGroup GroupFile = TabMenu.Groups.AddGroup("File", 100);
            GroupFile.Add(XTPControlType.xtpControlButton, fileMenuIcons[0], "&New", false, false);
            GroupFile.Add(XTPControlType.xtpControlButton, fileMenuIcons[1], "&Open", false, false);
            GroupFile.Add(XTPControlType.xtpControlButton, fileMenuIcons[2], "&Close", false, false);
                        
            RibbonGroup GroupEdit = TabMenu.Groups.AddGroup("Edit", 200);
            GroupEdit.Add(XTPControlType.xtpControlButton, fileMenuIcons[3], "&Paste", false, false);

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
            RibbonBar RibbonBar = (RibbonBar)CommandBars.ActiveMenuBar;
            RibbonBackstageView BackstageView = (RibbonBackstageView)CommandBars.CreateCommandBar("CXTPRibbonBackstageView");
            // 시스템 버튼 추가
            m_ControlFile = RibbonBar.AddSystemButton();
            m_ControlFile.Caption = "설정";
            // 아이콘을 시스템 아이콘과 통합
            m_ControlFile.IconId = ID.ID_SYSTEM_ICON;
            m_ControlFile.CommandBar = (XtremeCommandBars.CommandBar)BackstageView;
            if (m_pageOption == null)
                m_pageOption = new PageBackstageOption();
            // 패널을 붙인다.
            m_ControlOption = BackstageView.AddTab(ID.ID_PANE_OPTION, "설정", m_pageOption.Handle.ToInt32());
            m_ControlOption.DefaultItem = true;
            // 메뉴를 추가
            BackstageView.AddCommand(ID.ID_APP_EXIT, "끝내기");
        }

        CommandBar ToolBar;
        CommandBar ToolBar2;
        ArrayList m_arToolBarList = new ArrayList();
        private void CreateViewToolBar()
        {
            // 툴바 Add
            ToolBar = CommandBars.Add("Standard", XTPBarPosition.xtpBarTop);
            AddButton(ToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_FILE_NEW, "&New Project");
            AddButton(ToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_FILE_OPEN, "&Open Project");
            AddButton(ToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_FILE_SAVE, "&Save Project", true);
            ToolBar.SetIconSize(24, 24);
            m_arToolBarList.Add(ToolBar);
            ToolBar2 = CommandBars.Add("Standard", XTPBarPosition.xtpBarTop);
            AddButton(ToolBar2.Controls, XTPControlType.xtpControlButton, ID.ID_FILE_NEW, "&New Project");
            AddButton(ToolBar2.Controls, XTPControlType.xtpControlButton, ID.ID_FILE_OPEN, "&Open Project");
            AddButton(ToolBar2.Controls, XTPControlType.xtpControlButton, ID.ID_FILE_SAVE, "&Save Project", true);
            ToolBar2.SetIconSize(24, 24);
            m_arToolBarList.Add(ToolBar2);
        }


        private void CommandBars_UpdateEvent(object sender, AxXtremeCommandBars._DCommandBarsEvents_UpdateEvent e)
        {

        }
        
        private void CommandBars_ResizeEvent(object sender, EventArgs e)
        {
            int left, top, right, bottom;
            CommandBars.GetClientRect(out left, out top, out right, out bottom);
            m_MainPanel.SetBounds(left, top, right - left, bottom - top);
        }

        private void CommandBarsMenu_Execute(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
        {
            switch (e.control.Id)
            {
                default:
                    break;
            };
        }


        private void CommandBars_Execute(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
        {
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

            //if (m_PageHome != null)
            //{
            //    m_PageHome.OnChangeTheme(sender, e);
            //}
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {

        }

        
        private void FormMain_Load(object sender, EventArgs e)
        {
            RibbonBar().EnableFrameTheme();
            CommandBars.EnableCustomization(true);
            CommandBars.FindControl(XtremeCommandBars.XTPControlType.xtpControlButton, m_nDefaultThemeID, true, true).Execute();
                      
            m_PageHome.Show();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal && bSetRestore == true)
            {
                //bSetRestore = false;
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
            Refresh();
        }


        private int CalcToolBarHeight()
        {
            int height = 0;
            int left2, top2, right2, bottom2;
            foreach (CommandBar toolbar in m_arToolBarList)
            {
                if (toolbar.Position == XTPBarPosition.xtpBarTop || toolbar.Position == XTPBarPosition.xtpBarBottom)
                {                    
                    ToolBar.GetWindowRect(out left2, out top2, out right2, out bottom2);
                    height += bottom2 - top2;
                }
            }
            return height;
        }      

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

            if (m.Msg == WM_NCLBUTTONDBLCLK)       //preventing the form being resized by the mouse double click on the title bar.
            {
                m.Result = IntPtr.Zero;
                return;
            }

            base.WndProc(ref m);
        }       
       
    }
}
