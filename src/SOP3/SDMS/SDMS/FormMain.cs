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
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace SDMS
{
   

	public partial class FormMain : Form
	{
        [System.Runtime.InteropServices.DllImport("user32", EntryPoint = "PostMessageA", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int PostMessage(int hwnd, int wMsg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Ansi, SetLastError = true)]
        private static extern int SendMessage(int hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
        private const int CB_SHOWDROPDOWN = 0X14F;
        private const int WM_SETTEXT = 0x000C;

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

		public DBUtility.WebDBManager DBManager
		{
			get { return m_dbMgr; }
		}

		private static FormMain m_instance = null;
		public static FormMain Instance
		{
			get { return m_instance; }
		}

		public PageBackstageHome PageHome
		{
			get { return m_PageHome; }
		}

		private FormClock m_ClockForm = null;
		public FormClock ClockForm
		{
			get { return m_ClockForm; }
		}

		private FormStatus m_StatusForm = null;
		public FormStatus StatusForm
		{
			get { return m_StatusForm; }
		}
        
        private FormRealTimeInfo m_InfoForm = null;
        public FormRealTimeInfo RealTimeInfoForm
        {
            get { return m_InfoForm; }
            set { m_InfoForm = value; }
        }

        private FormReportFire m_ReportFireForm = null;
        public FormReportFire ReportFireForm
        {
            get { return m_ReportFireForm; }
            set { m_ReportFireForm = value; }
        }

		public SDMS.DataManager DataManager
		{
			get { return m_dataMgr; }
		}

		// window resotre event process
		private bool bSetRestore = false;
		private bool bEndRestore = false;
		private bool m_bExit = false;
		private bool bCheckRedarw = false;
		private bool bVisiblePane = false;
		private bool bPrevVisibleState = false;

		private CommandBarPopup m_ControlOptions = null;
		public CommandBarPopup m_ControlFile;
	   
		public CommandBarsGlobalSettings CommandBarsGlobalSettings;
		private RibbonBackstageTab m_ControlOption = null;

		private PageBackstageOption m_pageOption = null;
		private PageBackstageHome m_PageHome = null;
		public  XtremeCommandBars.StatusBar m_StatusBar = null;
		private StatusBarPane m_StatusPane = null;
		private string m_strSkinFolder = "";
		
		private int m_nDefaultThemeID = ID.ID_OPTIONS_STYLEOFFCIE2010BLACK;

		private DBUtility.WebDBManager m_dbMgr = new DBUtility.WebDBManager();
		private DataManager m_dataMgr = null;

		//////////////////////////////////////////////////////////////////////////
		// Tab
		private RibbonTab tabMonitoring = null;
		private RibbonTab tabAdmin = null;
		private RibbonTab tabReport = null;

		private RibbonTab m_CurrentTab = null;
		private RibbonTab m_PrevTab = null;

		//////////////////////////////////////////////////////////////////////////
		// Ribbon Group
		private RibbonGroup groupMonitoring = null;

		//////////////////////////////////////////////////////////////////////////
		// ToolBar
		private CommandBar m_LayerToolBar = null;
		private CommandBar m_3DToolBar = null;
		private CommandBar m_FloorToolBar = null;
		private CommandBar m_ReportToolBar = null;
		private CommandBar m_SaveReportToolBar = null;
        private CommandBar m_ReportActionBar = null;

		private ArrayList m_arToolBarList = new ArrayList();
		//////////////////////////////////////////////////////////////////////////
        // Reprot
        //1 - 탐지, 2 - 처리, 3 - 대응
        private int m_nReportPage = 1;

        private CommandBarControl m_ctrlSaveData = null;
        /// /////////////////////////////////////////////////////////////////////
        /// </summary>
		private CommandBarControl m_ctrlFireSensor = null, m_ctrlCoolerSensor = null, m_ctrlPressureSensor = null, m_ctrlCCTV = null, m_ctrlDelFacility = null;
		private CommandBarControl m_ctrlPick = null, m_ctrlPanning = null, m_ctrlOrbit = null;

        // 투명 배경용 Form
        private PopupTranslucentForm m_PopupPane = new PopupTranslucentForm();
        public PopupTranslucentForm PopupPane
        {
            get { return m_PopupPane; }
        }

        private int m_nMinWidth = 1200;
        private int m_nMaxWidth = 1920;

		public FormMain()
		{
            POI poi = new POI();
            poi.Facility = new CCTV();
            poi.Facility = null;
			m_instance = this;

            m_dataMgr = new DataManager(m_dbMgr);
			LoadBaseData();

			InitializeComponent();

			LoadIcons();

			//SkinLoad();

			CreateRibbonBar();
			CreateToolBar();
			CreateBackstageView();
			CreateBackstageHome();
			CreateStatusBar();

			this.Name = "SDMS";
			this.FormClosing += FormMain_FormClosing;
			this.FormClosed += FormMain_FormClosed;
			this.Load += FormMain_Load;           
            this.SizeChanged += this.FormMain_SizeChanged;

            AddPythonFunction();

            StartDatePickerPopUp.Visible = false;
            EndDatePickerPopUp.Visible = false;
		}

		/// <summary>
		/// UI 생성되기 이전에 필요한 DB Data 로드
		/// Form 생성 이전에 호출
		/// </summary>
		public void LoadBaseData()
		{
			ZoneManager.Instance.LoadBuildingData();
			ZoneManager.Instance.LoadZones();
            m_dataMgr.LoadFireEquipment();
		}

		/// <summary>
		/// UI가 생성된 이후에 사용될 DB Data 로드
		/// Form Load 이벤트에서 호출
		/// </summary>
		public void LoadExtraData()
		{
            SensorManager.Instance.ReadSensorData();
		}

		public void AddPythonFunction()
		{
			ScriptProxy proxy = ScriptProxy.Instance;
			proxy.UserObject.SelectMonitoringTab = new Func<bool>(SelectMonitoringTab);
			proxy.UserObject.SelectAdminTab = new Func<bool>(SelectAdminTab);
			proxy.UserObject.SelectReportTab = new Func<bool>(SelectReportTab);
			proxy.UserObject.SetToolBar = new Func<bool>(AdjustDockingToolBar);
		}

		/// <summary>
		/// 스킨 이미지를 사용하는 경우 사용
		/// </summary>
		private void SkinLoad()
		{
			m_strSkinFolder = StylesPath();
			SkinFramework.LoadSkin(m_strSkinFolder + "WinXP.Luna.cjstyles", "");
			SkinFramework.ApplyWindow(this.Handle.ToInt32());
			this.BackColor = SkinFramework.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BACKGROUND);
		}


        public static string EnginPath()
        {
            string szMainPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            string szWorkPath = szMainPath;
            if (File.Exists(szWorkPath + "Core.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "common\\";
            if (File.Exists(szWorkPath + "Core.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "SOP\\";
            if (File.Exists(szWorkPath + "Core.dll"))
                return szWorkPath;

            return szMainPath;
        }

		/// <summary>
		/// Resource Dll path를 찾는 함수
		/// </summary>
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

            if (System.IO.Directory.Exists(strExePath + "\\SOP\\Styles"))
                return strExePath + "\\SOP\\Styles\\";

			return strExePath + "\\Styles\\";
		}

		/// <summary>
		/// 지정된 이름의 리소스 이미지를 bitmap으로 로드
		/// </summary>
		/// <param name="imageName"></param>
		/// <returns></returns>
		public static Bitmap GetImageByName(string imageName)
		{
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			string resourceName = asm.GetName().Name + ".Properties.Resources";
			var rm = new System.Resources.ResourceManager(resourceName, asm);
			return (Bitmap)rm.GetObject(imageName);
		}
		
		/// <summary>
		/// 지정된 이름의 리소스 이미지를 CommandBar에서 사용하도록 지정
		/// </summary>
		/// <param name="name">리소스 이름</param>
		/// <param name="id">사용할 Command id</param>
		private void AddBitmapFormRes(string name, object id)
		{
			string szName = name.Replace("-", "_");
			Bitmap bImage = GetImageByName(szName);
			CommandBars.Icons.AddBitmap(bImage.GetHbitmap().ToInt32(), id, XTPImageState.xtpImageNormal, true);        
		}

		private int[] fileMenuIcons = 
		{  ID.ID_FILE_NEW,     
			ID.ID_FILE_OPEN,    
			ID.ID_FILE_SAVE,
			ID.ID_EDIT_PASTE,
			ID.ID_EDIT_FIND,    // 5
			ID.ID_FILE_PRINT,
			7,8,9,10,11,12,13
		};

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

		private int[] viewToolBars =
		{             
			ID.ID_VIEW_HOME,
            ID.ID_VIEW_FULLSCREEN,
			ID.ID_VIEW_PICK,
			ID.ID_VIEW_PAN,
			ID.ID_VIEW_ORBIT,			
			ID.ID_VIEW_ZOOMIN,
			ID.ID_VIEW_ZOOMOUT,
			ID.ID_VIEW_OUTSIDE,
			ID.ID_VIEW_BOTHSIDE,
			ID.ID_VIEW_INSIDE,
            ID.ID_VIEW_CCTV,
			ID.ID_VIEW_SCREENSHOT
		};

		private int[] adminMenuIcons =
		{  
            ID.ID_SAVE_DATA,
			ID.ID_NEW_FIRE_SENSOR,
			ID.ID_NEW_COOLER_SENSOR,
			ID.ID_NEW_PRESSURE_SENSOR,
			ID.ID_NEW_CCTV,
			ID.ID_DEL_FACILITY,
			ID.ID_SHOW_LIST_FACILITY,
			ID.ID_MANAGE_MANAGER,
			ID.ID_MANAGE_MESSAGE,
			ID.ID_MANAGE_BROADCAST,
			11,12,13
		};

		private int[] layerMenuIcons =
		{
			ID.ID_LAYER_DETECTOR,
			ID.ID_LAYER_COOLER,
			ID.ID_LAYER_PERSURE,
			ID.ID_LAYER_CCTV,
			ID.ID_LAYER_FIREEXT,
			ID.ID_LAYER_FIREHYD,
			ID.ID_LAYER_ALARMSTA,
			ID.ID_LAYER_RECIVER,
			ID.ID_LAYER_TEXTPOI
		};

		private int[] selectFloorIcons =
		{
			ID.ID_FLOOR_GORUP,
			ID.ID_FLOOR_BUILDING,
			ID.ID_FLOOR_FLOOR,
			ID.ID_FLOOR_SELECT
		};

		private void LoadIcons()
		{
			CommandBars.Options.UseSharedImageList = false;            
			
			AddBitmapFormRes("LargeIcons", fileMenuIcons);
			AddBitmapFormRes("PrintPreview", printMenuIcons);
			AddBitmapFormRes("LargeIcons", adminMenuIcons);
		}

		public RibbonBar RibbonBar()
		{
			return (XtremeCommandBars.RibbonBar)CommandBars.ActiveMenuBar;
		}
		
		/// <summary>
		/// Ribbon 바 생성
		/// </summary>
		private void CreateRibbonBar()
		{           
			RibbonBar RibbonBar = null;
			CommandBarPopup ControlPopup = null;
			
			RibbonBar = CommandBars.AddRibbonBar("The Ribbon");
			RibbonBar.EnableDocking(XTPToolBarFlags.xtpFlagStretched);
			RibbonBar.ShowQuickAccess = false;


			XtremeCommandBars.CommandBarControl btnDetect = null;
			XtremeCommandBars.CommandBarControl btnNoOperation = null;
			XtremeCommandBars.CommandBarControl btnAction = null;


			m_ControlOptions = (CommandBarPopup)RibbonBar.Controls.Add(XTPControlType.xtpControlPopup, 0, "설정", -1, false);
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

			tabMonitoring = RibbonBar.InsertTab(0, "모니터링");
			tabMonitoring.Id = 1000;
			groupMonitoring = tabMonitoring.Groups.AddGroup("File", 100);
			groupMonitoring.Add(XTPControlType.xtpControlButton, fileMenuIcons[0], "&New", false, false);
			groupMonitoring.Add(XTPControlType.xtpControlButton, fileMenuIcons[1], "&Open", false, false);
			groupMonitoring.Add(XTPControlType.xtpControlButton, fileMenuIcons[2], "&Close", false, false);

			CreateMonitoringItems(groupMonitoring);
						
			RibbonGroup GroupEdit = tabMonitoring.Groups.AddGroup("Edit", 200);
			GroupEdit.Add(XTPControlType.xtpControlButton, fileMenuIcons[3], "&Paste", false, false);

			tabAdmin = RibbonBar.InsertTab(1, "관리");
			tabAdmin.Id = 1001;

            RibbonGroup groupEdit = tabAdmin.Groups.AddGroup("데이터", 100);
            m_ctrlSaveData = groupEdit.Add(XTPControlType.xtpControlButton, adminMenuIcons[0], "저장", false, false);

			RibbonGroup groupAdmin = tabAdmin.Groups.AddGroup("생성", 200);
			m_ctrlFireSensor = groupAdmin.Add(XTPControlType.xtpControlButton, adminMenuIcons[1], "화재탐지", false, false);
			m_ctrlCoolerSensor = groupAdmin.Add(XTPControlType.xtpControlButton, adminMenuIcons[2], "스프링쿨러", false, false);
			m_ctrlPressureSensor = groupAdmin.Add(XTPControlType.xtpControlButton, adminMenuIcons[3], "펌프압력", false, false);
			m_ctrlCCTV = groupAdmin.Add(XTPControlType.xtpControlButton, adminMenuIcons[4], "CCTV", false, false);

			RibbonGroup GroupDel = tabAdmin.Groups.AddGroup("삭제", 300);
			m_ctrlDelFacility = GroupDel.Add(XTPControlType.xtpControlButton, adminMenuIcons[5], "삭제", false, false);

			RibbonGroup GroupList = tabAdmin.Groups.AddGroup("리스트", 400);
			GroupList.Add(XTPControlType.xtpControlButton, adminMenuIcons[6], "리스트 보기", false, false);

			RibbonGroup GroupConfig = tabAdmin.Groups.AddGroup("설정", 500);
			GroupConfig.Add(XTPControlType.xtpControlButton, adminMenuIcons[7], "담당자관리", false, false);
			GroupConfig.Add(XTPControlType.xtpControlButton, adminMenuIcons[8], "메시지관리", false, false);
			GroupConfig.Add(XTPControlType.xtpControlButton, adminMenuIcons[9], "방송관리", false, false);

			tabReport = RibbonBar.InsertTab(2, "리포트");
			tabReport.Id = 1002;
			RibbonGroup groupReport = tabReport.Groups.AddGroup("운영통계", 100);
			btnDetect = groupReport.Add(XTPControlType.xtpControlButton, ID.ID_BTN_DETECT, "탐지 이력", false, false);
			
			
			groupReport.Add(XTPControlType.xtpControlButton, ID.ID_BTN_NOTOPERATION, "오작동 이력", false, false);
			groupReport.Add(XTPControlType.xtpControlButton, ID.ID_BTN_ACTION, "대응 이력", false, false);
			
			//RibbonGroup GroupEdit3 = tabReport.Groups.AddGroup("Edit", 200);
			//GroupEdit3.Add(XTPControlType.xtpControlButton, fileMenuIcons[3], "&Paste", false, false);
		}


		#region (AddButton 함수)
		//////////////////////////////////////////////////////////////////////////        
		// CommandBar에 Button을 추가하는 보조 함수
		//////////////////////////////////////////////////////////////////////////

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
		//////////////////////////////////////////////////////////////////////////
		#endregion

		#region (CreateToolBar)
		///////////////////////////////////////////////////////////////////////////////////////
		
		private void CreateToolBar()
		{
			CreateViewToolBar();

			CreateLayerToolBar();

			CreateFloorToolBar();

			CreateReportToolBar();

            CreateReportActionBar();

			CreateSaveHangulToolBar();
		}

		/// <summary>
		/// 3D 뷰용 툴바 생성
		/// </summary>
		private void CreateViewToolBar()
		{
			// 툴바 Add

			m_3DToolBar = CommandBars.Add("3DView", XTPBarPosition.xtpBarTop);

			AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_HOME, "Home");
            AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_FULLSCREEN, "FullScreen");

			m_ctrlPick = AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_PICK, "Pick", true);
			m_ctrlPanning = AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_PAN, "Pan");
			m_ctrlOrbit = AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_ORBIT, "Orbit");
			

			AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_ZOOMIN, "ZoomIn", true);
			AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_ZOOMOUT, "ZoomOut");
			AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_OUTSIDE, "Outside", true);
			AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_BOTHSIDE, "Both");
			AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_INSIDE, "Inside");

            AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_CCTV, "CCTV", true);
			AddButton(m_3DToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_VIEW_SCREENSHOT, "ScreenShot");

			m_3DToolBar.SetIconSize(24, 24);
			m_arToolBarList.Add(m_3DToolBar);
		}

		/// <summary>
		/// CCTV, 소방설비 관련 Layer 설정용 툴바 생성
		/// </summary>
		private void CreateLayerToolBar()
		{  
			m_LayerToolBar = CommandBars.Add("Layer", XTPBarPosition.xtpBarLeft);
			AddButton(m_LayerToolBar.Controls, XTPControlType.xtpControlButton, layerMenuIcons[0], "탐지기");
			AddButton(m_LayerToolBar.Controls, XTPControlType.xtpControlButton, layerMenuIcons[1], "쿨러");
			AddButton(m_LayerToolBar.Controls, XTPControlType.xtpControlButton, layerMenuIcons[2], "압력계");
			AddButton(m_LayerToolBar.Controls, XTPControlType.xtpControlButton, layerMenuIcons[3], "CCTV");
			AddButton(m_LayerToolBar.Controls, XTPControlType.xtpControlButton, layerMenuIcons[4], "소화기");
			AddButton(m_LayerToolBar.Controls, XTPControlType.xtpControlButton, layerMenuIcons[5], "소화전");
			AddButton(m_LayerToolBar.Controls, XTPControlType.xtpControlButton, layerMenuIcons[6], "발신기");
			AddButton(m_LayerToolBar.Controls, XTPControlType.xtpControlButton, layerMenuIcons[7], "수신기");
			AddButton(m_LayerToolBar.Controls, XTPControlType.xtpControlButton, layerMenuIcons[8], "이름");
			m_LayerToolBar.SetIconSize(36, 36);
			m_arToolBarList.Add(m_LayerToolBar);
		}

		/// <summary>
		/// 3D 실내모델 층 이동용 툴바 생성
		/// </summary>

		private CommandBarComboBox m_cmbBuildingGroup = null;
		private CommandBarGallery  m_GalleryBuildingGroup = null;
		private CommandBarGalleryItems m_ItemsGroup = null;

		private CommandBarComboBox m_cmbBuilding = null;
		private CommandBarGallery m_GalleryBuilding = null;
		private CommandBarGalleryItems m_ItemsBuilding = null;

		private CommandBarComboBox m_cmbFloor = null;
		private CommandBarGallery m_GalleryFloor = null;
		private CommandBarGalleryItems m_ItemsFloor = null;

		private BuildingGroup SetBuildingGroupGallary()
		{
			int nCount = 1;
			bool bInit = false;
			if (m_ItemsGroup == null)
				bInit = true;

			if (bInit == true)
				m_ItemsGroup = CommandBars.CreateGalleryItems(ID.ID_GALLARY_GROUP);
			else
				m_ItemsGroup.DeleteAll();

			m_ItemsGroup.ItemWidth = 0;
			m_ItemsGroup.ItemHeight = 17;
			string szDefault = "";
			BuildingGroup defultGroup = null;
			Dictionary<int, BuildingGroup> dicGroup = ZoneManager.Instance.DicBuildingGroup;
			foreach (KeyValuePair<int, BuildingGroup> kv in dicGroup)
			{
				BuildingGroup obj = kv.Value;
				CommandBarGalleryItem item = m_ItemsGroup.AddItem(ID.ID_GALLARY_GROUP + nCount, obj.BuildingGroupName);
				if (nCount == 1)
				{
					szDefault = obj.BuildingGroupName;
					defultGroup = obj;
				}
				item.Tag = obj;
				nCount++;
			}

			if (bInit == true)
			{
				CommandBar ComboPopup = CommandBars.Add("GroupPopup", XTPBarPosition.xtpBarComboBoxGalleryPopup);
				m_GalleryBuildingGroup = (CommandBarGallery)ComboPopup.Controls.Add(XTPControlType.xtpControlGallery, ID.ID_GALLARY_GROUP, "", -1, false);
				m_cmbBuildingGroup.CommandBar = ComboPopup;
			}
		   
			m_GalleryBuildingGroup.Width = 135;
			m_GalleryBuildingGroup.Height = 17 * dicGroup.Count;
			m_GalleryBuildingGroup.Items = m_ItemsGroup;

			m_cmbBuildingGroup.Width = 135;            
			m_cmbBuildingGroup.DropDownListStyle = true;
			m_cmbBuildingGroup.Text = szDefault;

			return defultGroup;
		}

		private Building SetBuildingGallary(BuildingGroup group)
		{
			int nCount = 1;
			bool bInit = false;

			if (m_ItemsBuilding == null)
				bInit = true;

			if (bInit == true)
				m_ItemsBuilding = CommandBars.CreateGalleryItems(ID.ID_GALLARY_BUILDING);
			else
				m_ItemsBuilding.DeleteAll();

			m_ItemsBuilding.ItemWidth = 0;
			m_ItemsBuilding.ItemHeight = 17;
			string szDefault = "";
			Building dBuilding = null;
			Dictionary<int, Building> dicBuilding = ZoneManager.Instance.DicBuildings;
			foreach (KeyValuePair<int, Building> kv in dicBuilding)
			{
				Building obj = kv.Value;
				if (obj.BuildingGroup.BuildingGroupName == group.BuildingGroupName)
				{
					CommandBarGalleryItem item = m_ItemsBuilding.AddItem(ID.ID_GALLARY_BUILDING + nCount, obj.BuildingName);
					if (nCount == 1)
					{
						szDefault = obj.BuildingName;
						dBuilding = obj;
					}
					item.Tag = obj;
					nCount++;
				}
			}

			if (bInit == true)
			{
				CommandBar ComboPopup2 = CommandBars.Add("BuidingPopup", XTPBarPosition.xtpBarComboBoxGalleryPopup);
				m_GalleryBuilding = (CommandBarGallery)ComboPopup2.Controls.Add(XTPControlType.xtpControlGallery, ID.ID_GALLARY_BUILDING, "", -1, false);
				m_cmbBuilding.CommandBar = ComboPopup2;
			}

			m_GalleryBuilding.Width = 135;
			m_GalleryBuilding.Height = 17 * (nCount - 1);
			m_GalleryBuilding.Items = m_ItemsBuilding;

			m_cmbBuilding.Width = 135;            
			m_cmbBuilding.DropDownListStyle = true;
			m_cmbBuilding.Text = szDefault;

			return dBuilding;
		}

		private int SetFloorGallary(Building building)
		{
			int nCount = 1;
			bool bInit = false;

			if (m_ItemsFloor == null)
				bInit = true;

			if (bInit == true)
				m_ItemsFloor = CommandBars.CreateGalleryItems(ID.ID_GALLARY_FLOOR);
			else
				m_ItemsFloor.DeleteAll();

			m_ItemsFloor.ItemWidth = 0;
			m_ItemsFloor.ItemHeight = 17;
			string szDefault = "";

			ArrayList arZone = building.FloorList;
			foreach (Zone floor in arZone)
			{
				string szCaption = floor.Floor.StrFloor;
				CommandBarGalleryItem item = m_ItemsFloor.AddItem(ID.ID_GALLARY_FLOOR + nCount, szCaption);
				item.Tag = floor;
				if (nCount == 1)
				{
					szDefault = szCaption;
				}
				nCount++;
			}

			int nMin = building.MinFloorIndex;
			//int nMax = building.MaxFloorIndex;


			//for (int i = nMin; i <= nMax; i++)
			//{
			//    string szCaption = "";
			//    if( i < 0)
			//    {
			//        szCaption = "B" + Math.Abs(i).ToString();
			//    }
			//    else
			//    {
			//        szCaption = "" + (i+1).ToString();
			//    }
			//    CommandBarGalleryItem item = m_ItemsFloor.AddItem(ID.ID_GALLARY_FLOOR + nCount, szCaption);
			//    item.Tag = i;
			//    if (nCount == 1)
			//    {
			//        szDefault = szCaption;
			//    }
			//    nCount++;
			//}

			if (bInit == true)
			{
				CommandBar ComboPopup2 = CommandBars.Add("FloorPopup", XTPBarPosition.xtpBarComboBoxGalleryPopup);
				m_GalleryFloor = (CommandBarGallery)ComboPopup2.Controls.Add(XTPControlType.xtpControlGallery, ID.ID_GALLARY_FLOOR, "", -1, false);
				m_cmbFloor.CommandBar = ComboPopup2;
			}

			m_GalleryFloor.Width = 80;
			m_GalleryFloor.Height = 17 * (nCount - 1);
			m_GalleryFloor.Items = m_ItemsFloor;

			m_cmbFloor.Width = 80;
			m_cmbFloor.DropDownListStyle = true;
			m_cmbFloor.Text = szDefault;

			return nMin;
		}

		private void CreateFloorToolBar()
		{
			m_FloorToolBar = CommandBars.Add("Layer", XTPBarPosition.xtpBarTop);
			AddButton(m_FloorToolBar.Controls, XTPControlType.xtpControlLabel, selectFloorIcons[0], "그룹");
			m_cmbBuildingGroup = (CommandBarComboBox)AddButton(m_FloorToolBar.Controls, XTPControlType.xtpControlComboBox, selectFloorIcons[0], "그룹");

			BuildingGroup dGroup = SetBuildingGroupGallary();
			
			AddButton(m_FloorToolBar.Controls, XTPControlType.xtpControlLabel, selectFloorIcons[1], "건물", true);
			m_cmbBuilding = (CommandBarComboBox)AddButton(m_FloorToolBar.Controls, XTPControlType.xtpControlComboBox, selectFloorIcons[1], "건물");

			Building dBuilding = SetBuildingGallary(dGroup);

			AddButton(m_FloorToolBar.Controls, XTPControlType.xtpControlLabel, selectFloorIcons[2], "층", true);
			m_cmbFloor = (CommandBarComboBox)AddButton(m_FloorToolBar.Controls, XTPControlType.xtpControlComboBox, selectFloorIcons[2], "층");

			SetFloorGallary(dBuilding);

			AddButton(m_FloorToolBar.Controls, XTPControlType.xtpControlButton, selectFloorIcons[3], "선택", true);
			m_FloorToolBar.SetIconSize(24, 24);
		   
			m_arToolBarList.Add(m_FloorToolBar);           
		}

		/// <summary>
		/// 리포트용 툴바 생성
		/// </summary>
		/// 

		private CommandBarComboBox cboBuildingGroup = null;
        private XtremeCommandBars.CommandBarGallery GroupGallery = null;
        private XtremeCommandBars.CommandBarGalleryItems Groupitem = null;

        private CommandBarComboBox cboBuilding = null;
        private XtremeCommandBars.CommandBarGallery BuildingGallery = null;
        private XtremeCommandBars.CommandBarGalleryItems Buildingitem = null;

        private CommandBarComboBox cboFloor = null;
		private XtremeCommandBars.CommandBarGallery FloorGallery = null;
        private XtremeCommandBars.CommandBarGalleryItems Flooritem = null;

        private CommandBarComboBox cboLatelyDate = null;
        private CommandBarComboBox cboStartDate = null;
        private CommandBarComboBox cboEndDate = null;

        private CommandBarComboBox cboActionStartDate = null;
        private CommandBarComboBox cboActionEndDate = null;

        private CommandBarComboBox cboStartTime = null;
        private CommandBarComboBox cboEndTime = null;
        private CommandBarComboBox cboSelectFire = null;


        private void CreateReportActionBar()
        {
            m_ReportActionBar = CommandBars.Add("ReportAction", XTPBarPosition.xtpBarTop);
            AddButton(m_ReportActionBar.Controls, XTPControlType.xtpControlLabel, 0, "기간 선택");

            cboActionStartDate = (CommandBarComboBox)m_ReportActionBar.Controls.Add(XTPControlType.xtpControlComboBox, ID.ID_CBO_START_DATE, "시작 일");
            cboActionEndDate = (CommandBarComboBox)m_ReportActionBar.Controls.Add(XTPControlType.xtpControlComboBox, ID.ID_CBO_END_DATE, "끝 일");

            AddButton(m_ReportActionBar.Controls, XTPControlType.xtpControlLabel, 0, "시간 선택");

            cboStartTime = (CommandBarComboBox)AddButton(m_ReportActionBar.Controls, XTPControlType.xtpControlComboBox, ID.ID_CBO_START_TIME, "시간");
            XtremeCommandBars.CommandBarGallery StartTimeGallery = null;
            XtremeCommandBars.CommandBarGalleryItems StartTimeitem = null;

            StartTimeitem = CommandBars.CreateGalleryItems(6450);
            StartTimeitem.ItemWidth = 0;
            StartTimeitem.ItemHeight = 17;

            for (int i = 0; i < 24; i++)
            {
                StartTimeitem.AddItem(i, i+"시");
            }
            CommandBar StartTimePopup = CommandBars.Add("StartTime Popup", XtremeCommandBars.XTPBarPosition.xtpBarComboBoxGalleryPopup);
            StartTimeGallery = (XtremeCommandBars.CommandBarGallery)StartTimePopup.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlGallery, 6450, "", -1, false);
            StartTimeGallery.Width = 80;
            StartTimeGallery.Height = 16 * 24;
            StartTimeGallery.Items = StartTimeitem;

            cboStartTime.Width = 80;
            cboStartTime.CommandBar = StartTimePopup;
            cboStartTime.DropDownListStyle = true;
            cboStartTime.Text = "시작 시간";

            cboEndTime = (CommandBarComboBox)AddButton(m_ReportActionBar.Controls, XTPControlType.xtpControlComboBox, ID.ID_CBO_END_TIME, "시간");
            XtremeCommandBars.CommandBarGallery EndTimeGallery = null;
            XtremeCommandBars.CommandBarGalleryItems EndTimeitem = null;

            EndTimeitem = CommandBars.CreateGalleryItems(6451);
            EndTimeitem.ItemWidth = 0;
            EndTimeitem.ItemHeight = 17;

            for (int i = 1; i < 25; i++)
            {
                EndTimeitem.AddItem(i, i + "시");
            }
            CommandBar EndTimePopup = CommandBars.Add("EndTime Popup", XtremeCommandBars.XTPBarPosition.xtpBarComboBoxGalleryPopup);
            EndTimeGallery = (XtremeCommandBars.CommandBarGallery)EndTimePopup.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlGallery, 6451, "", -1, false);
            EndTimeGallery.Width = 80;
            EndTimeGallery.Height = 16 * 24;
            EndTimeGallery.Items = EndTimeitem;

            cboEndTime.Width = 80;
            cboEndTime.CommandBar = EndTimePopup;
            cboEndTime.DropDownListStyle = true;
            cboEndTime.Text = "끝 시간";

            AddButton(m_ReportActionBar.Controls, XTPControlType.xtpControlLabel, 0, "화재 선택");

            cboSelectFire = (CommandBarComboBox)AddButton(m_ReportActionBar.Controls, XTPControlType.xtpControlComboBox, ID.ID_CBO_SELECT_FIRE, "화재 선택");
            XtremeCommandBars.CommandBarGallery FireGallery = null;
            XtremeCommandBars.CommandBarGalleryItems Fireitem = null;

            Fireitem = CommandBars.CreateGalleryItems(6452);
            Fireitem.ItemWidth = 0;
            Fireitem.ItemHeight = 17;

            CommandBar SelectFirePopup = CommandBars.Add("SelecFire Popup", XtremeCommandBars.XTPBarPosition.xtpBarComboBoxGalleryPopup);
            FireGallery = (XtremeCommandBars.CommandBarGallery)SelectFirePopup.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlGallery, 6452, "", -1, false);
            FireGallery.Width = 200;
            FireGallery.Height = 16 * 24;
            FireGallery.Items = Fireitem;

            cboSelectFire.Width = 200;
            cboSelectFire.CommandBar = SelectFirePopup;
            cboSelectFire.DropDownListStyle = true;
            cboSelectFire.Text = "불러오기";

            m_ReportActionBar.SetIconSize(24, 24);
            m_ReportActionBar.Visible = false;
            m_arToolBarList.Add(m_ReportActionBar);
        }

		private void CreateReportToolBar()
		{
			m_ReportToolBar = CommandBars.Add("Report", XTPBarPosition.xtpBarTop);

			AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlLabel, 0, "기간 선택");

            cboStartDate = (CommandBarComboBox)AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlComboBox, ID.ID_CBO_START_DATE, "시작 일");
            XtremeCommandBars.CommandBarGallery StartGallery = null;
            XtremeCommandBars.CommandBarGalleryItems Startitem = null;

            Startitem = CommandBars.CreateGalleryItems(6400);
            Startitem.ItemWidth = 0;
            Startitem.ItemHeight = 17;

            CommandBar StartPopup = CommandBars.Add("Start Popup", XtremeCommandBars.XTPBarPosition.xtpBarComboBoxGalleryPopup);
            StartGallery = (XtremeCommandBars.CommandBarGallery)StartPopup.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlGallery, 6400, "", -1, false);
            StartGallery.Width = 100;
            StartGallery.Height = 16;
            StartGallery.Items = Startitem;

            cboStartDate.CommandBar = StartPopup;
            cboStartDate.DropDownListStyle = true;

            cboStartDate.Text = "시작 일";
            cboEndDate = (CommandBarComboBox)AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlComboBox, ID.ID_CBO_END_DATE, "끝 일");
            XtremeCommandBars.CommandBarGallery EndGallery = null;
            XtremeCommandBars.CommandBarGalleryItems Enditem = null;

            Enditem = CommandBars.CreateGalleryItems(6401);
            Enditem.ItemWidth = 0;
            Enditem.ItemHeight = 17;

            CommandBar EndPopup = CommandBars.Add("End Popup", XtremeCommandBars.XTPBarPosition.xtpBarComboBoxGalleryPopup);
            EndGallery = (XtremeCommandBars.CommandBarGallery)EndPopup.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlGallery, 6401, "", -1, false);
            EndGallery.Width = 100;
            EndGallery.Height = 16 * 4;
            EndGallery.Items = Enditem;

            cboEndDate.CommandBar = EndPopup;
            cboEndDate.DropDownListStyle = true;
            cboEndDate.Text = "끝 일";


            cboLatelyDate = (CommandBarComboBox)AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlComboBox, ID.ID_CBO_LATELY_DATE, "최근");
            XtremeCommandBars.CommandBarGallery LatelyGallery = null;
            XtremeCommandBars.CommandBarGalleryItems Latelyitem = null;

            Latelyitem = CommandBars.CreateGalleryItems(6500);
            Latelyitem.ItemWidth = 0;
            Latelyitem.ItemHeight = 17;
            Latelyitem.AddItem(6, "최근 6개월");
            Latelyitem.AddItem(3, "최근 3개월");
            Latelyitem.AddItem(1, "최근 1개월");

			CommandBar ComboPopup = CommandBars.Add("Combo Popup", XtremeCommandBars.XTPBarPosition.xtpBarComboBoxGalleryPopup);
            LatelyGallery = (XtremeCommandBars.CommandBarGallery)ComboPopup.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlGallery, 6500, "", -1, false);
            LatelyGallery.Width = 100;
            LatelyGallery.Height = 16 * 3;
            LatelyGallery.Items = Latelyitem;

            cboLatelyDate.CommandBar = ComboPopup;
            cboLatelyDate.Text = "기간 선택";
            //cboLatelyDate.DropDownListStyle = false;
            cboLatelyDate.DropDownListStyle = true;
			
			AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlLabel, 0, "범위 선택",true);

			AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlLabel, ID.ID_CBO_GROUP, "그룹", true);
			cboBuildingGroup = (CommandBarComboBox)AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlComboBox, ID.ID_CBO_GROUP, "그룹");
		   
			BuildingGroup buildingGroup = SetBuildingGroup();
			
			AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlLabel, 0, "건물", true);
			cboBuilding = (CommandBarComboBox)AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlComboBox, ID.ID_CBO_BUILDING, "건물");
			
			Building building = SetBuilding(buildingGroup);

			AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlLabel, 0,  "층", true);
			cboFloor = (CommandBarComboBox)AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlComboBox, ID.ID_CBO_FLOOR, "층");
			SetFloor(building);


			AddButton(m_ReportToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_BTN_GROUP, "선택", true);
            
            
            if (cboBuildingGroup.ListIndex == 1)
            {
                cboBuilding.ListIndex = 1;
                cboFloor.ListIndex = 1;
                cboBuilding.Enabled = false;
                cboFloor.Enabled = false;

                return;
            }
            else
            {
                cboBuilding.Enabled = true;
                cboFloor.Enabled = true;
            }

            if (cboBuilding.ListIndex == 1)
            {
                cboFloor.ListIndex = 1;
                cboBuilding.Enabled = false;

                return;
            }
            else
            {
                cboFloor.Enabled = true;
            }
            
			m_ReportToolBar.SetIconSize(24, 24);
			m_ReportToolBar.Visible = false;
			m_arToolBarList.Add(m_ReportToolBar);
		}

		private BuildingGroup SetBuildingGroup()
		{
			int nCount = 1;
			bool bInit = false;
			string szDefault = "";

			if (Groupitem == null)
				bInit = true;

			if (bInit == true)
				Groupitem = CommandBars.CreateGalleryItems(6501);
			else
				Groupitem.DeleteAll();

			Groupitem.ItemWidth = 0;
			Groupitem.ItemHeight = 17;

            CommandBarGalleryItem item = Groupitem.AddItem(6501 + nCount, "모든 건물그룹");

			BuildingGroup defultGroup = null;
			Dictionary<int, BuildingGroup> dicGroup = ZoneManager.Instance.DicBuildingGroup;
		   
			//BuildingGroup에서 key값을 찾아옴
			foreach (KeyValuePair<int, BuildingGroup> kv in dicGroup)
			{
				BuildingGroup obj = kv.Value;
			    item = Groupitem.AddItem(6501 + nCount +1, obj.BuildingGroupName);
				if (nCount == 1)
				{
					szDefault = obj.BuildingGroupName;
					defultGroup = obj;
				}
				item.Tag = obj;
				nCount++;
			}

			if (bInit == true)
			{
				CommandBar GroupPopup = CommandBars.Add("Group Popup", XtremeCommandBars.XTPBarPosition.xtpBarComboBoxGalleryPopup);
				GroupGallery = (XtremeCommandBars.CommandBarGallery)GroupPopup.Controls.Add(XtremeCommandBars.XTPControlType.xtpControlGallery, 6501, "", -1, false);
				cboBuildingGroup.CommandBar = GroupPopup;
			}
			
			GroupGallery.Width = 135;
			GroupGallery.Height = 17 * nCount;
			GroupGallery.Items = Groupitem;

			cboBuildingGroup.Width = 135;
			cboBuildingGroup.DropDownListStyle = true;
			//cboBuildingGroup.Text = szDefault;

			if (cboBuildingGroup.DropDownItemCount > 0)
				cboBuildingGroup.ListIndex = 1;
			
			return defultGroup;
		}

	   

		private Building SetBuilding(BuildingGroup group)
		{            
			int nCount = 1;
			bool bInit = false;

			if (Buildingitem == null)
				bInit = true;

			if (bInit == true)
				Buildingitem = CommandBars.CreateGalleryItems(6502);
			else
				Buildingitem.DeleteAll();

			Buildingitem.ItemWidth = 0;
			Buildingitem.ItemHeight = 17;
			string szDefault = "";

			Building dBuilding = null;
			Dictionary<int, Building> dicBuilding = ZoneManager.Instance.DicBuildings;

            CommandBarGalleryItem item = Buildingitem.AddItem(6502 + nCount, "모든 건물" );

			foreach (KeyValuePair<int, Building> pair in dicBuilding)
			{
				Building obj = pair.Value;
				ArrayList arrZones = ZoneManager.Instance.GetZoneList(obj.ID);
				if (obj.BuildingGroup.BuildingGroupName == group.BuildingGroupName)
				{
					if (arrZones != null)
					{
						item = Buildingitem.AddItem(6502 + nCount+1, obj.BuildingName);

						if (nCount == 1)
						{
							szDefault = obj.BuildingName;
							dBuilding = obj;
						}
						item.Tag = obj;
						nCount++;
					}
				}
			}

			if (bInit == true)
			{
				CommandBar ComboPopup2 = CommandBars.Add("BuidingPopup", XTPBarPosition.xtpBarComboBoxGalleryPopup);
				BuildingGallery = (CommandBarGallery)ComboPopup2.Controls.Add(XTPControlType.xtpControlGallery, 6502, "", -1, false);
				cboBuilding.CommandBar = ComboPopup2;
			}

			BuildingGallery.Width = 135;
			BuildingGallery.Height = 17 * nCount;
			BuildingGallery.Items = Buildingitem;

 
			cboBuilding.Width = 135;
			cboBuilding.DropDownListStyle = true;
			//cboBuilding.Text = szDefault;

			if (cboBuilding.DropDownItemCount > 0)
				cboBuilding.ListIndex = 1;

			return dBuilding;
		}

		
		private void SetFloor(Building building)
		{
			int nCount = 1;
			bool bInit = false;

			if (Flooritem == null)
				bInit = true;

			if (bInit == true)
				Flooritem = CommandBars.CreateGalleryItems(6503);
			else
				Flooritem.DeleteAll();

			Flooritem.ItemWidth = 0;
			Flooritem.ItemHeight = 17;
			string szDefault = "";

            CommandBarGalleryItem item = Flooritem.AddItem(6503 + nCount, "모든 층");

			ArrayList arZone = building.FloorList;
			foreach (Zone floor in arZone)
			{
				string szCaption = floor.Floor.StrFloor;
				item = Flooritem.AddItem(6503 + nCount, szCaption);
				item.Tag = floor;
				if (nCount == 1)
				{
					szDefault = szCaption;
				}
				nCount++;
			}

			//Dictionary<int, Building> dicBuildings = ZoneManager.Instance.DicBuildings;
			//foreach (KeyValuePair<int, Building> pair in dicBuildings)
			//{
			//    Building obj = pair.Value;
			//    if (obj.BuildingName == building.BuildingName)
			//    {

			//        ArrayList arrFloor = new ArrayList();

			//        foreach (KeyValuePair<int, Zone> pair2 in ZoneManager.Instance.DicZones)
			//        {
			//            if (pair2.Value.Building == building)
			//                arrFloor.Add(new Floor(pair2.Value.FloorIndex + pair2.Value.AddFloor));
			//        }


			//       // arrFloor.Sort();

			//        foreach (Floor floor in arrFloor)
			//        {
			//            CommandBarGalleryItem item = Flooritem.AddItem(6503 + nCount, floor.StrFloor);

			//            item.Tag = floor;
			//            nCount++;

			//        }
			//    }
			//}

			if (bInit == true)
			{
				CommandBar ComboPopup2 = CommandBars.Add("FloorPopup", XTPBarPosition.xtpBarComboBoxGalleryPopup);
				FloorGallery = (CommandBarGallery)ComboPopup2.Controls.Add(XTPControlType.xtpControlGallery, 6503, "", -1, false);
				cboFloor.CommandBar = ComboPopup2;
			}

			FloorGallery.Width = 80;
			FloorGallery.Height = 17 * nCount;
			FloorGallery.Items = Flooritem;

			cboFloor.Width = 80;
			cboFloor.DropDownListStyle = true;
			cboFloor.Text = szDefault;

			if (cboFloor.DropDownItemCount > 0)
				cboFloor.ListIndex = 1;
		}

		
		/// <summary>
		/// 한글 파일 저장용 툴바 생성
		/// </summary>
		private void CreateSaveHangulToolBar()
		{
			m_SaveReportToolBar = CommandBars.Add("Layer", XTPBarPosition.xtpBarTop);
			CommandBarControl c = AddButton(m_SaveReportToolBar.Controls, XTPControlType.xtpControlButton, ID.ID_FILE_NEW, "그룹");
			c.Style = XtremeCommandBars.XTPButtonStyle.xtpButtonIconAndCaption;
			
			m_SaveReportToolBar.SetIconSize(24, 24);
			m_SaveReportToolBar.Visible = false;
			m_arToolBarList.Add(m_FloorToolBar);
		}
		///////////////////////////////////////////////////////////////////////////////////////
		#endregion

		/// <summary>
		/// 시계폼과 메세지폼을 생성
		/// </summary>
		/// <param name="group">폼이 나타날 대상 그룹</param>
		private void CreateMonitoringItems(RibbonGroup group)
		{
			int left, top, right, bottom;
			group.GetRect(out left, out top, out right, out bottom);

			group.DeleteAll();

			top += 51;

			m_ClockForm = new FormClock(this);
			m_ClockForm.Location = new Point(left, top);
			m_ClockForm.Show();

			m_StatusForm = new FormStatus(this);
            int width = left + m_ClockForm.Size.Width;
			m_StatusForm.Location = new Point(width, top);
			m_StatusForm.Show();

			m_StatusForm.SetStatus("이상 없음");

            width = width + m_StatusForm.Width;
            m_InfoForm = new FormRealTimeInfo(this);
            m_InfoForm.Location = new Point(width, top);
            m_InfoForm.Show();


            width = width + m_InfoForm.Width;
            m_ReportFireForm = new FormReportFire(this);
            m_InfoForm.Location = new Point(width, top);
            m_ReportFireForm.Show();
		}

		/// <summary>
		/// Main 패널을 가지는 BackstageHome을 생성
		/// </summary>
		private void CreateBackstageHome()
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

		private void CreateStatusBar()
		{
			m_StatusBar = CommandBars.StatusBar;
			m_StatusBar.Visible = true;
			m_StatusPane = m_StatusBar.AddPane(0);
			m_StatusBar.AddPane(ID.ID_INDICATOR_CAPS);
			m_StatusBar.AddPane(ID.ID_INDICATOR_NUM);
			m_StatusBar.AddPane(ID.ID_INDICATOR_SCRL);
		}

		/// <summary>
		/// Option용 Backstage 뷰를 생성
		/// </summary>
		private void CreateBackstageView()
		{
			RibbonBar RibbonBar = (RibbonBar)CommandBars.ActiveMenuBar;
			RibbonBackstageView BackstageView = (RibbonBackstageView)CommandBars.CreateCommandBar("CXTPRibbonBackstageView");
			// 시스템 버튼 추가
			m_ControlFile = RibbonBar.AddSystemButton();
			m_ControlFile.Caption = "파일";
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

		#region ( Commandbar 이벤트 )
		/////////////////////////////////////////////////////////////////////////////////
		private void CommandBars_UpdateEvent(object sender, AxXtremeCommandBars._DCommandBarsEvents_UpdateEvent e)
		{
			// 종료되는 경우 Update를 처리하지 않도록 한다
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
				//bCheckRedarw = true;
				m_PrevTab = m_CurrentTab;
			}

            //int width = this.Size.Width;
            //Debug.WriteLine(width);
			// Update 루틴 시작
			CommandBarsMenu_Update(sender, e);
			// Update 루틴 종료

			if (bCheckRedarw == true)
			{
				if (m_PageHome != null)
				{
					m_PageHome.Redraw3DView();
				}
			}
		}
		
		/// <summary>
		/// 테마를 변경하는 이벤트를 처리. 일반적인 메뉴의 처리는 CommandBarsMenu_Execute 에서 수행함
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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
					return;                   
			};

			if (m_PageHome != null)
			{
				//m_PageHome.OnChangeTheme(e.control.Id);
			}
		}
		/////////////////////////////////////////////////////////////////////////////////
		#endregion

		/// <summary>
		/// 일반적인 메뉴, 툴바의 업데이트 이벤트를 처리
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CommandBarsMenu_Update(object sender, AxXtremeCommandBars._DCommandBarsEvents_UpdateEvent e)
        {

            switch (e.control.Id)
            {
                case ID.ID_FLOOR_GORUP:
                case ID.ID_FLOOR_BUILDING:
                case ID.ID_FLOOR_FLOOR:
                case ID.ID_FLOOR_SELECT:
                    break;

                case ID.ID_VIEW_HOME:
                case ID.ID_VIEW_FULLSCREEN:
                case ID.ID_VIEW_PICK:
                case ID.ID_VIEW_PAN:
                case ID.ID_VIEW_ORBIT:                
                case ID.ID_VIEW_ZOOMIN:
                case ID.ID_VIEW_ZOOMOUT:
                case ID.ID_VIEW_OUTSIDE:
                case ID.ID_VIEW_BOTHSIDE:
                case ID.ID_VIEW_INSIDE:
                case ID.ID_VIEW_CCTV:
                case ID.ID_VIEW_SCREENSHOT:

                case ID.ID_NEW_FIRE_SENSOR:
                case ID.ID_NEW_COOLER_SENSOR:
                case ID.ID_NEW_PRESSURE_SENSOR:
                case ID.ID_NEW_CCTV:
                case ID.ID_DEL_FACILITY:

                case ID.ID_SAVE_DATA:

                    m_PageHome.OnUpdateCommandBarMenu(sender, e);
                    break;

                case ID.ID_LAYER_DETECTOR:
                case ID.ID_LAYER_COOLER:
                case ID.ID_LAYER_PERSURE:
                case ID.ID_LAYER_CCTV:
                case ID.ID_LAYER_FIREEXT:
                case ID.ID_LAYER_FIREHYD:
                case ID.ID_LAYER_ALARMSTA:
                case ID.ID_LAYER_RECIVER:
                case ID.ID_LAYER_TEXTPOI:
                    m_PageHome.OnUpdateChangeLayer(sender, e);
                    break;
                default:
                    break;
            };
        }

		/// <summary>
		/// Floor 툴바의 실행 이벤트를 처리
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnChangeFloor(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)        
		{
			int nID = e.control.Id;
			switch (nID)
			{
                case ID.ID_GALLARY_GROUP:
				case ID.ID_FLOOR_GORUP:
					int nIdx = m_cmbBuildingGroup.ListIndex;
                    if (nIdx == 0)
                        nIdx = 1;
					BuildingGroup group = (BuildingGroup)m_ItemsGroup[nIdx - 1].Tag;
					Building building = SetBuildingGallary(group);
					SetFloorGallary(building);
					break;
				case ID.ID_FLOOR_BUILDING:
					int nIdx2 = m_cmbBuilding.ListIndex;
                    if (nIdx2 == 0)
                        nIdx2 = 1;
					Building building2 = (Building)m_ItemsBuilding[nIdx2 - 1].Tag;
					SetFloorGallary(building2);
					break;
				case ID.ID_FLOOR_FLOOR:
					break;
				case ID.ID_FLOOR_SELECT:
					int nIdxGroup = m_cmbBuildingGroup.ListIndex;
                    if (nIdxGroup == 0)
                        nIdxGroup = 1;
					BuildingGroup groupSelect = (BuildingGroup)m_ItemsGroup[nIdxGroup - 1].Tag;
					int nIdxBuilding = m_cmbBuilding.ListIndex;
                    if (nIdxBuilding == 0)
                        nIdxBuilding = 1;
					Building buildingSelect = (Building)m_ItemsBuilding[nIdxBuilding - 1].Tag;
					int nIdxFloor = m_cmbFloor.ListIndex;
                    if (nIdxFloor == 0)
                        nIdxFloor = 1;
					Zone selectFloor = (Zone)m_ItemsFloor[nIdxFloor - 1].Tag;
					SetFloorStatus(groupSelect, buildingSelect, selectFloor);
					break;             
				default:
					break;
			}
		}
       
		/// <summary>
		/// 일반적인 메뉴, 툴바의 실행 이벤트를 처리
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CommandBarsMenu_Execute(object sender, AxXtremeCommandBars._DCommandBarsEvents_ExecuteEvent e)
		{
			if ((e.control.Id >= ID.ID_FLOOR_GORUP) && (e.control.Id < 6000))
			{
				OnChangeFloor(sender, e);
			}
		
			switch (e.control.Id)
			{
				case ID.ID_BTN_DETECT:
                    m_nReportPage = 1;
                    m_ReportToolBar.Visible = true;
                    m_ReportActionBar.Visible = false;
					m_PageHome.FrmReport.DetectPanel.Visible = true;
					m_PageHome.FrmReport.NotOperationPanel.Visible = false;
					m_PageHome.FrmReport.ActionPanel.Visible = false;
					break;
				case ID.ID_BTN_NOTOPERATION:
                    m_nReportPage = 2;
                    m_ReportToolBar.Visible = true;
                    m_ReportActionBar.Visible = false;
					m_PageHome.FrmReport.DetectPanel.Visible = false;
					m_PageHome.FrmReport.NotOperationPanel.Visible = true;
					m_PageHome.FrmReport.ActionPanel.Visible = false;
					break;
				case ID.ID_BTN_ACTION:
                    m_nReportPage = 3;
                    m_ReportToolBar.Visible = false;
                    m_ReportActionBar.Visible = true;
					m_PageHome.FrmReport.DetectPanel.Visible = false;
					m_PageHome.FrmReport.NotOperationPanel.Visible = false;
					m_PageHome.FrmReport.ActionPanel.Visible = true;
					break;
				
                case ID.ID_CBO_LATELY_DATE:
                    DateTime dt = DateTime.Now;
                    DateTime dtOld = new DateTime();
                    if (cboLatelyDate.ListIndex == 1) //6개월
                        dtOld = dt.AddMonths(-6);
                    else if (cboLatelyDate.ListIndex == 2) //3개월
                        dtOld = dt.AddMonths(-3);
                    else if (cboLatelyDate.ListIndex == 3) //1개월
                        dtOld = dt.AddMonths(-1);

                    cboStartDate.Text = dtOld.ToString().Substring(0, 10);
                    cboEndDate.Text = dt.ToString().Substring(0, 10);
                    break;

                case ID.ID_CBO_GROUP:
                    if (cboBuildingGroup.ListIndex == 1)
                    {
                        cboBuilding.ListIndex = 1;
                        cboFloor.ListIndex = 1;
                        cboBuilding.Enabled = false;
                        cboFloor.Enabled = false;

                        return;
                    }
                    else
                    {
                        cboBuilding.Enabled = true;
                        //cboFloor.Enabled = true;
                    }

                    int nIdx = cboBuildingGroup.ListIndex;
                    BuildingGroup group = (BuildingGroup)Groupitem[nIdx-1].Tag;
                    Building building = SetBuilding(group);
                    SetFloor(building);
                    break;
                case ID.ID_CBO_BUILDING:
                    if (cboBuilding.ListIndex == 1)
                    {
                        cboFloor.ListIndex = 1;
                        cboFloor.Enabled = false;

                        return;
                    }
                    else
                        cboFloor.Enabled = true;

                    int nIdx2 = cboBuilding.ListIndex;
                    Building building2 = (Building)Buildingitem[nIdx2 - 1].Tag;
                    SetFloor(building2);
                    break;

				case ID.ID_BTN_GROUP:
                    bool BtnSelect = true;
                    bool AllBuildingGroup = false;
                    bool AllBuilding = false;
                    bool AllFoor = false;
                    BuildingGroup group3 = new BuildingGroup();
                    Building building3 = new Building();
                    Zone zone = new Zone();
                    Object Allobj = null;

                    if (cboStartDate.Text == "시작 일" || cboEndDate.Text == "끝 일")
                    {
                        MessageBox.Show("날짜를 입력해주세요");
                        return;
                    }

                    DateTime startDate = DateTime.ParseExact(cboStartDate.Text, "yyyy-MM-dd", null);
                    DateTime EndDate = DateTime.ParseExact(cboEndDate.Text, "yyyy-MM-dd", null);
                    if (startDate > EndDate)
                    {
                        MessageBox.Show("시작 일이 더 클 수 없습니다.");
                        return;
                    }

                    if (cboBuildingGroup.ListIndex == 1)
                    {
                        AllBuildingGroup = true;
                    }
                    if (cboBuilding.ListIndex == 1)
                    {
                        //FindGroup
                        AllBuilding = true;
                        if (cboBuildingGroup.ListIndex != 1)
                        {
                            int selectedGroup = cboBuildingGroup.ListIndex;
                            group3 = (BuildingGroup)Groupitem[selectedGroup - 1].Tag;
                        }
                    }
                    if (cboFloor.ListIndex == 1)
                    {
                        //FindBuilding
                        AllFoor = true;
                        if (cboBuildingGroup.ListIndex != 1 && cboBuilding.ListIndex != 1)
                        {
                            int selectedBuilding = cboBuilding.ListIndex;
                            building3 = (Building)Buildingitem[selectedBuilding - 1].Tag;
                        }
                    }

                    if (cboBuildingGroup.ListIndex != 1 && cboBuilding.ListIndex != 1 && cboFloor.ListIndex != 1)
                    {
                        //FIndZone
                        int nIdx3 = cboBuilding.ListIndex;
                        if (nIdx3 == 0)
                            return;
                        building3 = (Building)Buildingitem[nIdx3 - 1].Tag;
                        zone = ZoneManager.Instance.FindZone(building3, cboFloor.Text);
                    }
                    //m_PageHome.FrmReport.DetectPage.DateSubmit(m_Month);
                    if (m_nReportPage == 1)
                    {
                        //시작일, 종료일
                        m_PageHome.FrmReport.DetectPage.AllSubmit(AllBuildingGroup, AllBuilding, AllFoor);
                        m_PageHome.FrmReport.DetectPage.ComboTxtDate(cboStartDate.Text, cboEndDate.Text);
                        m_PageHome.FrmReport.DetectPage.ComboSubmit(group3, building3, zone, BtnSelect);
                        m_PageHome.FrmReport.DetectPage.ComboSubmit(cboBuildingGroup.Text, cboBuilding.Text, cboFloor.Text); //Label에 띄울 값
                        m_PageHome.FrmReport.DetectPage.Function_DataGrid();
                        m_PageHome.FrmReport.DetectPage.CreateLineChart();
                    }
                    else if (m_nReportPage == 2)
                    {
                        //오작동
                        m_PageHome.FrmReport.NotOperation.AllSubmit(AllBuildingGroup, AllBuilding, AllFoor);
                        m_PageHome.FrmReport.NotOperation.ComboTxtDate(cboStartDate.Text, cboEndDate.Text);
                        m_PageHome.FrmReport.NotOperation.ComboSubmit(group3, building3, zone, BtnSelect);
                        //m_PageHome.FrmReport.NotOperation.ComboSubmit(cboBuildingGroup.Text, cboBuilding.Text, cboFloor.Text); //Label에 띄울 값
                        m_PageHome.FrmReport.NotOperation.createBarChart(m_PageHome.FrmReport.NotOperation.PercentBarChart);
                        m_PageHome.FrmReport.NotOperation.ReportData.ComboTxtDate(cboStartDate.Text, cboEndDate.Text);
                        m_PageHome.FrmReport.NotOperation.ReportData.AllSubmit(AllBuildingGroup, AllBuilding, AllFoor);
                        m_PageHome.FrmReport.NotOperation.ReportData.ComboSubmit(group3, building3, zone, BtnSelect);
                        m_PageHome.FrmReport.NotOperation.ReportData.LoadReactionHistory();
                        m_PageHome.FrmReport.NotOperation.Function_DataGrid();
                    }
                    else
                    {
                    }
					break;

                case ID.ID_VIEW_HOME:
                case ID.ID_VIEW_FULLSCREEN:
				case ID.ID_VIEW_PICK:
				case ID.ID_VIEW_PAN:
				case ID.ID_VIEW_ORBIT:				
				case ID.ID_VIEW_ZOOMIN:
				case ID.ID_VIEW_ZOOMOUT:
				case ID.ID_VIEW_OUTSIDE:
				case ID.ID_VIEW_BOTHSIDE:
				case ID.ID_VIEW_INSIDE:
                case ID.ID_VIEW_CCTV:
				case ID.ID_VIEW_SCREENSHOT:

				case ID.ID_NEW_FIRE_SENSOR:
				case ID.ID_NEW_COOLER_SENSOR:
				case ID.ID_NEW_PRESSURE_SENSOR:
				case ID.ID_NEW_CCTV:
                case ID.ID_DEL_FACILITY:

                case ID.ID_SAVE_DATA:
					m_PageHome.CommandBarsMenu_Execute(sender, e);
					return;

				case ID.ID_LAYER_DETECTOR:
                    Splash s = new Splash();
                    FormMain.ShowPopupForm(s, 20, 20, 600, 300);
                    break;
				case ID.ID_LAYER_COOLER:
				case ID.ID_LAYER_PERSURE:
				case ID.ID_LAYER_CCTV:
				case ID.ID_LAYER_FIREEXT:
				case ID.ID_LAYER_FIREHYD:
				case ID.ID_LAYER_ALARMSTA:
				case ID.ID_LAYER_RECIVER:
				case ID.ID_LAYER_TEXTPOI:
					m_PageHome.OnChangeLayer(sender, e);
					break;
				default:
					break;
			};
		}

        static public void ShowPopupForm(Form formTarget, int x, int y, int width, int height)
        {
            m_instance.PopupPane.Parent = m_instance.PageHome;
            m_instance.PopupPane.AddContentForm(formTarget, x, y, width, height);
            m_instance.PopupPane.Show(m_instance);
        }

		private void MainTimer_Tick(object sender, EventArgs e)
		{
			m_MainTimer.Enabled = false;
			m_MainTimer.Stop();

			if (m_PageHome != null)
			{
				m_PageHome.Redraw3DView();
			}


			m_MainTimer.Enabled = true;
			m_MainTimer.Start();
		}

        private bool m_bDetectFireSensor = false;
        public bool DetectFireSensor
        {
            get { return m_bDetectFireSensor; }
            set { m_bDetectFireSensor = value; }
        }

        


        private void CheckFireAlarmTimer_Tick(object sender, EventArgs e)
        {
            if (m_bDetectFireSensor == true)
            {
                m_bDetectFireSensor = false;
                //BeginFireDetectProcess(); 
            }
        }
		
		private void FormMain_Load(object sender, EventArgs e)
		{
			RibbonBar().EnableFrameTheme();
			CommandBars.EnableCustomization(true);
			CommandBars.FindControl(XtremeCommandBars.XTPControlType.xtpControlButton, m_nDefaultThemeID, true, true).Execute();

			AdjustDockingToolBar();
				 
			m_PageHome.Show();

			SelectMonitoringTab();

			LoadExtraData();

			m_MainTimer.Enabled = true;
			m_MainTimer.Start();

            SetInfoMessage("테스트 메세지");


            //m_CheckFireAlarmTimer.Enabled = true;
            //m_CheckFireAlarmTimer.Interval = 400;
            //m_CheckFireAlarmTimer.Start();
            SensorSignalReciver.Instance.StartPolling();
            
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			m_bExit = true;

            SensorSignalReciver.Instance.StopPolling();
            SensorSignalReciver.Instance.Dispose();

            ProcessManager.Instance.Dispose();

		}

		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
		}
		
		public void SetFloorStatus(BuildingGroup grp, Building building, Zone zoneFloor)
		{
			m_PageHome.ChangeFloor(grp, building, zoneFloor);
		}

        public static void SetInfoMessage(string szMessage)
        {
            m_instance.RealTimeInfoForm.RealTimeInfo = szMessage;
            m_instance.RealTimeInfoForm.DrawMovingText();
        }

		#region Size 변경관련 함수
		/////////////////////////////////////////////////////////////////////////////////       

		private void CommandBars_ResizeEvent(object sender, EventArgs e)
		{
			int left, top, right, bottom;
			CommandBars.GetClientRect(out left, out top, out right, out bottom);
			m_MainPanel.SetBounds(left, top, right - left, bottom - top);

            
            int curWidth = this.ClientSize.Width - 3;
            if (curWidth < m_nMinWidth)
                curWidth = m_nMinWidth;
            int extraLength = this.ClientSize.Width - 3 - m_nMinWidth;
            if (extraLength < 0)
                extraLength = 0;

			//int left, top, right, bottom;
			if (groupMonitoring != null)
			{
                int x, y;
                groupMonitoring.GetRect(out x, out top, out y, out bottom);
               
                int width = x;
                int height = bottom - top;

                if (m_ClockForm != null )
				{                    
                    m_ClockForm.Location = new Point(width, top);
                    m_ClockForm.Size = new Size(m_ClockForm.Size.Width, height);
                    width += m_ClockForm.Size.Width;
                }

				if (m_StatusForm != null)
				{
                    int e1 = (int)extraLength / 3;
					m_StatusForm.Location = new Point(width, top);
                    m_StatusForm.Size = new Size(350 + e1, height);
                    width += m_StatusForm.Size.Width;
				}
                
                if( m_InfoForm != null)
                {

                    int infoFormWidth = curWidth - width - m_ReportFireForm.Width;
                    m_InfoForm.Location = new Point(width, top);
                    m_InfoForm.Size = new Size(infoFormWidth, height);
                    width += m_InfoForm.Size.Width;
                }

                if (m_ReportFireForm != null)
                {                    
                    m_ReportFireForm.Location = new Point(width, top);
                    m_ReportFireForm.Size = new Size(m_ReportFireForm.Width, height);
                    width += m_ReportFireForm.Size.Width;
                }
            }
		}

		/// <summary>
		/// Docking 툴바의 순서와 위치를 조정
		/// </summary>
		public bool AdjustDockingToolBar()
		{
			// Docking Toolbar 위치를 조정
			int left, top, right, bottom;
			int width, height = 0;
			if (m_3DToolBar.Visible == true)
			{

				CommandBars.DockToolBar(m_3DToolBar, 0, 0, XTPBarPosition.xtpBarTop);
				m_3DToolBar.GetWindowRect(out left, out top, out right, out bottom);
				width = right - left;
				height = bottom - top;
				
				CommandBars.DockToolBar(m_FloorToolBar, width + 10, 0,  XTPBarPosition.xtpBarTop);
			}
			if (m_ReportToolBar.Visible == true)
			{
				CommandBars.DockToolBar(m_ReportToolBar, 0, 0, XTPBarPosition.xtpBarTop);
				m_ReportToolBar.GetWindowRect(out left, out top, out right, out bottom);
				width = right - left;
				height = bottom - top;
				CommandBars.DockToolBar(m_SaveReportToolBar, width + 20, 0, XTPBarPosition.xtpBarTop);
			}

            CommandBars.DockToolBar(m_ReportActionBar, 0, 0, XTPBarPosition.xtpBarTop);
            m_ReportActionBar.GetWindowRect(out left, out top, out right, out bottom);
            width = right - left;
            height = bottom - top;
            CommandBars.DockToolBar(m_SaveReportToolBar, width + 20, 0, XTPBarPosition.xtpBarTop);

			return true;
		}

		/// <summary>
		/// 폼의 사이즈가 변경이 완료된 경우 호출. 폼헤더를 클릭하여 사이즈가 변경되는 경우 별도처리가 추가됨
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FormMain_SizeChanged(object sender, EventArgs e)
		{
            // 크기가 일정수준 이하이면 RibbonBar가 보이지 않게 되므로
            // 폰트 및 탭 추가등 길이에 영향이 있는경우 아래 길이를 수정해야 함
            if (this.Size.Width <= 265)
            {
                SetVisibleMonitoringForm(false);
            }
            else
            {
                SetVisibleMonitoringForm(true);
            }

			// 윈도우 헤더를 클릭하여 사이즈가 변경되는 경우를 체크
			if (WindowState == FormWindowState.Normal && bSetRestore == true)
			{
				bEndRestore = true;
			}

			int left, top, right, bottom;
			CommandBars.GetClientRect(out left, out top, out right, out bottom);

			int height = 0;

			// 윈도우 헤더를 클릭하여 사이즈가 변경되는 경우 높이 오류를 수정한다.
			if (bSetRestore == true)
				height = CalcToolBarHeight();

			m_MainPanel.SetBounds(left, top, right - left, bottom - top - height);

			// 리스토어가 종료이면 변수값 설정한다.
			if (bEndRestore == true)
			{
				bSetRestore = false;
				bEndRestore = false;
			}

			if (m_PageHome != null)
				m_PageHome.Redraw3DView();
		}

		/// <summary>
		/// 윈도우 헤더를 클릭하여 사이즈가 변경되는 경우 변경할 높이를 계산
		/// </summary>
		/// <returns>height</returns>
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
		/////////////////////////////////////////////////////////////////////////////////
		#endregion
		
		#region (WndProc)
		///////////////////////////////////////////////////////////////////////////////////////////////
		// 
		// Wnd Proc 
		//
		///////////////////////////////////////////////////////////////////////////////////////////////


		// WndProc을 이용하여 윈도우 헤더 클릭하는 순간을 지정
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
		#endregion

		#region (ChangeTab)
		/////////////////////////////////////////////////////////////////////////////////        
		public bool SelectMonitoringTab()
		{
			tabMonitoring.Selected = true;
            tabAdmin.Selected = false;
            tabReport.Selected = false;
			OnSelectMonitoringTab();
			return true;
		}
		public bool SelectAdminTab()
		{
			tabAdmin.Selected = true;
            tabMonitoring.Selected = false;
            tabReport.Selected = false;
			OnSelectAdminTab();
			return true;
		}
		public bool SelectReportTab()
		{
			tabReport.Selected = true;
            tabMonitoring.Selected = false;
            tabAdmin.Selected = false;
			OnSelectReportTab();
			return true;
		}
		
		private void OnSelectMonitoringTab()
		{
			m_SaveReportToolBar.Visible = false;
            m_ReportActionBar.Visible = false;
			m_ReportToolBar.Visible = false;
			m_3DToolBar.Visible = true;
			m_FloorToolBar.Visible = true;

            SetVisibleMonitoringForm(true);
			PageHome.ChangeTab(PageBackstageHome.Tab.MONITORING_TAB);
			
			AdjustDockingToolBar();
		}
		
		private void OnSelectAdminTab()
		{
            m_ReportActionBar.Visible = false;
			m_SaveReportToolBar.Visible = false;
			m_ReportToolBar.Visible = false;
			m_3DToolBar.Visible = true;
			m_FloorToolBar.Visible = true;

            SetVisibleMonitoringForm(false);
            PageHome.ChangeTab(PageBackstageHome.Tab.ADMIN_TAB);

			AdjustDockingToolBar();
		}        

		private void OnSelectReportTab()
		{            
			m_3DToolBar.Visible = false;
			m_FloorToolBar.Visible = false;
			m_SaveReportToolBar.Visible = true;
            m_ReportToolBar.Visible = true;
            m_ReportActionBar.Visible = false;
  
            SetVisibleMonitoringForm(false);
			PageHome.ChangeTab(PageBackstageHome.Tab.REPORT_TAB);

			AdjustDockingToolBar();

            cboEndDate.SetFocus();
            cboStartDate.SetFocus();

		}

        private void DropDownStartDate()
        {            
            if (IsDate(cboStartDate.Text))
                StartDatePickerPopUp.EnsureVisible(System.Convert.ToDateTime(cboStartDate.Text));
            int x, y, witdth, height;
            m_ReportToolBar.GetWindowRect(out x, out y, out witdth, out height);
            Point p = PointToClient(new Point(x, y));
            StartDatePickerPopUp.Left = p.X + cboStartDate.Left;
            StartDatePickerPopUp.Top = p.Y + cboStartDate.Top + cboStartDate.Height;
            StartDatePickerPopUp.Visible = true;
            int nCount = 0;
            if (StartDatePickerPopUp.ShowModal(1, 1))
            {
                nCount = StartDatePickerPopUp.Selection.BlocksCount;
                if (nCount > 0)
                {
                    cboStartDate.Clear();
                    cboStartDate.Parent.RedrawBar();
                    string szText = StartDatePickerPopUp.Selection[0].DateBegin.ToShortDateString();
                    cboStartDate.Text = szText;
                    cboLatelyDate.Text = "기간 선택";
                }
                else
                {
                    cboStartDate.Clear();

                }
            }
            StartDatePickerPopUp.Visible = false;
        }

        private void DropDownEndDate()
        {

            //EndDatePickerPopUp

            if (IsDate(cboEndDate.Text))
                EndDatePickerPopUp.EnsureVisible(System.Convert.ToDateTime(cboEndDate.Text));
            int x, y, witdth , height;
            m_ReportToolBar.GetWindowRect(out x, out y, out witdth, out height);
            Point p = PointToClient(new Point(x, y));
            EndDatePickerPopUp.Left = p.X + cboEndDate.Left;
            EndDatePickerPopUp.Top = p.Y + cboEndDate.Top + cboEndDate.Height;
           
            int nCount = 0;
            EndDatePickerPopUp.Visible = true;
            if (EndDatePickerPopUp.ShowModal(1, 1))
            {
                nCount = EndDatePickerPopUp.Selection.BlocksCount;
                if (nCount > 0)
                {
                    string szText = EndDatePickerPopUp.Selection[0].DateBegin.ToShortDateString();
                    cboEndDate.Text = szText;
                    cboLatelyDate.Text = "기간 선택";
                }
            }            
            EndDatePickerPopUp.Visible = false;
        }

        private bool IsDate(string sDate)
        {
            try
            {
                DateTime dtDate = DateTime.Parse(sDate);
            }
            catch (FormatException)
            {
                return false;
            }
            return true;
        }

		private void CommandBars_ControlNotify(object sender, AxXtremeCommandBars._DCommandBarsEvents_ControlNotifyEvent e)
		{
            if(e.control == cboStartDate)
            {
                int i = 0;
                i++;

                Debug.WriteLine(""+ e.code);
            }
			if (e.code == (int)XtremeCommandBars.XTPControlNotify.XTP_BS_TABCHANGED)
			{
				//System.Diagnostics.Trace.WriteLine("Selected File Tab");
			}
			else if (e.code == -551 && e.control.Id == (int)XtremeCommandBars.XTPCommandBarsSpecialCommands.XTP_ID_RIBBONCONTROLTAB)
			{
				XtremeCommandBars.TabControlItem tab = RibbonBar().SelectedTab;
				if (tab != null)
				{
					//System.Diagnostics.Trace.WriteLine(string.Format("Selected {0} Tab, Tab Index : {1}", tab.Caption, tab.Index));

					if (tab == tabMonitoring)
					{
						OnSelectMonitoringTab();                        
					}
					else if (tab == tabAdmin)
					{
						OnSelectAdminTab();                        
					}
					else if (tab == tabReport)
					{
						OnSelectReportTab();
					}
				}
			}

            if (e.code == (int)XTPControlNotify.XTP_CBN_DROPDOWN)
            {
                if (e.control == cboStartDate)
                {
                    if( cboStartDate.HasFocus == true)
                        DropDownStartDate();


                }
                else if (e.control == cboEndDate)
                {
                    PostMessage(cboEndDate.Parent.hWnd, CB_SHOWDROPDOWN, 0, 0);
                    if (cboEndDate.HasFocus == true)
                        DropDownEndDate();
                }
                
            }
		}

        private void CommandBars_ToolBarVisibleChanged(object sender, AxXtremeCommandBars._DCommandBarsEvents_ToolBarVisibleChangedEvent e)
        {
            int i = 0;
            i++;
        }
		//////////////////////////////////////////////////////////////////////////
		#endregion
		

        private void SetVisibleMonitoringForm(bool bShow)
        {
            if (tabMonitoring == null )
            {
                return;
            }
            if(bShow == true && tabMonitoring.Selected == false)
            {
                return;
            }

            if (m_StatusForm != null)
                m_StatusForm.Visible = bShow;
            if (m_ClockForm != null)
                m_ClockForm.Visible = bShow;
            if (m_InfoForm != null)
                m_InfoForm.Visible = bShow;
            if (m_ReportFireForm != null)
                m_ReportFireForm.Visible = bShow;
            
        }

	}
}
