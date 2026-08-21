using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;
using UnE.View.Content;


namespace SDMS
{
    public enum CCTVMode { CCTV_ONLY = 0, NORMAL }

    public enum LAYER_TYPE
    {
        FIRE_DETECT = 1,
        SPRING_COOLER = 2,
        PUMP = 4,
        CCTV = 8,
        CCTV_L = 16,
        FE = 32,
        HD = 64,
        FA = 128,
        FR = 256,
        BUILDING_TEXT = 512,
        CCTV_DISCONNECTED = 1024
        , NOTICE = 2048
    }

    public partial class FormMain : Form, ITextPictureBoxOwner, IRibbonButtonOwner
    {
        private class ControlInitPos
        {
            private static ControlInitPos m_instance = null;

            public static ControlInitPos Instance
            {
                get
                {
                    if (m_instance == null)
                        m_instance = new ControlInitPos();

                    return m_instance;
                }
            }

            private int m_nLabelSelectZoneInitPos = 0;
            private int m_nComboBoxBuildingGroupInitPos = 0;
            private int m_nComboBoxBuildingInitPos = 0;
            private int m_nComboBoxFloorInitPos = 0;
            private int m_nButtonSelectZoneInitPos = 0;
            private int m_nLabelFireDetectInitPos = 0;
            private int m_nComboBoxFireDetectInitPos = 0;
            private int m_nPanelMiddleInitSize = 0;

            private bool m_visibleLayerFire = true;
            private bool m_visibleLayerSpringCooler = true;
            private bool m_visibleLayerPump = true;
            private bool m_visibleLayerCCTV = true;
            private bool m_visibleLayerLowCCTV = true;
            private bool m_visibleLayerCCTVDisconnected = true;
            private bool m_visibleLayerFE = true;
            private bool m_visibleLayerHD = true;
            private bool m_visibleLayerFA = true;
            private bool m_visibleLayerFR = true;
            private bool m_visibleLayerBuildingText = true;
            private bool m_visibleLayerNotice = true;

            public int LabelSelectZoneInitPos
            {
                get { return m_nLabelSelectZoneInitPos; }
            }

            public int ComboBoxBuildingGroupInitPos
            {
                get { return m_nComboBoxBuildingGroupInitPos; }
            }

            public int ComboBoxBuildingInitPos
            {
                get { return m_nComboBoxBuildingInitPos; }
            }

            public int ComboBoxFloorInitPos
            {
                get { return m_nComboBoxFloorInitPos; }
            }

            public int ButtonSelectZoneInitPos
            {
                get { return m_nButtonSelectZoneInitPos; }
            }

            public int LabelFireDetectInitPos
            {
                get { return m_nLabelFireDetectInitPos; }
            }

            public int ComboBoxFireDetectInitPos
            {
                get { return m_nComboBoxFireDetectInitPos; }
            }

            public int PanelMiddleInitSize
            {
                get { return m_nPanelMiddleInitSize; }
            }

            public bool VisibleLayerFire
            {
                get { return m_visibleLayerFire; }
                set { m_visibleLayerFire = value; }
            }

            public bool VisibleLayerSpringCooler
            {
                get { return m_visibleLayerSpringCooler; }
                set { m_visibleLayerSpringCooler = value; }
            }

            public bool VisibleLayerPump
            {
                get { return m_visibleLayerPump; }
                set { m_visibleLayerPump = value; }
            }

            public bool VisibleLayerCCTV
            {
                get { return m_visibleLayerCCTV; }
                set { m_visibleLayerCCTV = value; }
            }

            public bool VisibleLayerLowCCTV
            {
                get { return m_visibleLayerLowCCTV; }
                set { m_visibleLayerLowCCTV = value; }
            }

            public bool VisibleLayerCCTVDisconnected
            {
                get { return m_visibleLayerCCTVDisconnected; }
                set { m_visibleLayerCCTVDisconnected = value; }
            }

            public bool VisibleLayerFE
            {
                get { return m_visibleLayerFE; }
                set { m_visibleLayerFE = value; }
            }

            public bool VisibleLayerHD
            {
                get { return m_visibleLayerHD; }
                set { m_visibleLayerHD = value; }
            }

            public bool VisibleLayerFA
            {
                get { return m_visibleLayerFA; }
                set { m_visibleLayerFA = value; }
            }

            public bool VisibleLayerFR
            {
                get { return m_visibleLayerFR; }
                set { m_visibleLayerFR = value; }
            }

            public bool VisibleLayerBuildingText
            {
                get { return m_visibleLayerBuildingText; }
                set { m_visibleLayerBuildingText = value; }
            }

            public bool VisibleLayerNotice
            {
                get { return m_visibleLayerNotice; }
                set { m_visibleLayerNotice = value; }
            }

            private ControlInitPos()
            {
                m_nLabelSelectZoneInitPos = FormMain.Instance.labelSelectZone.Location.X;
                m_nComboBoxBuildingGroupInitPos = FormMain.Instance.cboBuildingGroup.Location.X;
                m_nComboBoxBuildingInitPos = FormMain.Instance.cboBuilding.Location.X;
                m_nComboBoxFloorInitPos = FormMain.Instance.cboFloor.Location.X;
                m_nButtonSelectZoneInitPos = FormMain.Instance.btnSelectZone.Location.X;
                m_nLabelFireDetectInitPos = FormMain.Instance.labelFireDetect.Location.X;
                m_nComboBoxFireDetectInitPos = FormMain.Instance.cmbFireDetect.Location.X;

                m_nPanelMiddleInitSize = FormMain.Instance.panelMiddle.Size.Width;
            }
        }       

        private const string CloseManualFire = "기타재난\r\n상황종료";
        private const string RaiseManualFire = "기타재난\r\n전파";

        // 연습용 모드인가?
        private bool m_isSimulationMode = false;
        public bool SimulationMode
        {
            get { return m_isSimulationMode; }
            set { m_isSimulationMode = value; }
        }

        private bool m_closeApplication = false;

        public bool CloseApplication
        {
            get { return m_closeApplication; }
            set { m_closeApplication = value; }
        }

        public string SimulationConfigFilePath
        {
            get { return Application.StartupPath + "\\SimulationConfig.xml"; }
        }

        private CCTVMode m_cctvMode = CCTVMode.NORMAL;

        private int m_nPanelTopHeight = 154;//169;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private DBUtility.WebDBManager m_dbMgr = null;//new DBUtility.WebDBManager();

        public DBUtility.WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        //////////////////////////////////////////////////////////////////////////
        // Reprot
        //1 - 재난탐지, 2 - 재난처리, 3 - 재난대응, 4 - 재난문자, 5 - 누출탐지, 6 - 누출처리, 7 - 누출대응, 8 - 누출문자
        private int m_nReportPage = 1;

        private PageBackstageHome m_PageHome = null;
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

        private FormReportFire m_ReportFireForm = null;
        public FormReportFire ReportFireForm
        {
            get { return m_ReportFireForm; }
            set { m_ReportFireForm = value; }
        }

        private DataManager m_dataMgr = null;
        public SDMS.DataManager DataManager
        {
            get { return m_dataMgr; }
        }

        private SDMS.PopupDialog.FormPSMSensorTrendData m_frmPSMSensorData = null;
        public SDMS.PopupDialog.FormPSMSensorTrendData PSMSensorDataForm
        {
            get { return m_frmPSMSensorData; }
            set { m_frmPSMSensorData = value; }
        }

        private int m_nSystemButtonSpace = 0;

        private bool m_bExit = false;

        public bool Exit
        {
            get { return m_bExit; }
        }

        // Button별 ID
        private Dictionary<Button, int> m_dicButtonIDs = new Dictionary<Button, int>();

        private Dictionary<int, Button> m_dicIDButtons = new Dictionary<int, Button>();
        private Dictionary<Button, bool> m_dicButtonChecked = new Dictionary<Button, bool>();

        // Form Move 를 위한 Panel Top 클릭 체크
        private bool m_bLeftMouseDown = false;

        // Form Move 를 위한 Panel Top 클릭 점
        private Point m_ptMove;

        private NetworkManager m_netMgr = null;

        private bool m_bDetectFireSensor = false;
        public bool DetectFireSensor
        {
            get { return m_bDetectFireSensor; }
            set { m_bDetectFireSensor = value; }
        }

        //private FormViewOutdoor m_frmOutdoor = null;
        //private FormViewIndoor m_frmIndoor = null;
        //private FormCCTVGuide m_frmCCTVGuide = null;

        private int m_nOriginLeftPanelWidth = 0;

        private bool m_isFirstReport = true;
        private DateTime m_dtLastReport = new DateTime();
        private int m_nReadHistoryID = -1;

        private bool m_isThumbnailMode = false;

        public bool ThumbnailMode
        {
            get { return m_isThumbnailMode; }
            set { m_isThumbnailMode = value; }
        }

        //public FormCCTVGuide CCTVGuide
        //{
        //    get { return m_frmCCTVGuide; }
        //}


        private FormHomeView m_PaneBtnHome = new FormHomeView();
        private FormSaveHomeView m_PaneBtnSaveHome = new FormSaveHomeView();
        private FormSimulationSelector m_PanelBtnSimulator = new FormSimulationSelector();
        private SDMS.PopupDialog.FormNotice m_PanelBtnNotice = new SDMS.PopupDialog.FormNotice();

        private bool m_bHiddenClock = false;
        private bool m_usePopupSensorOn = true;
        private bool m_bLoadDBOption = false;

        private bool m_useMovingText = false;
        private bool m_readyToReceiveMessage = false;
        //private int m_nMessageInterval = 3, m_nMessageTimeCount = 0;

        private bool m_useReport = true;

        public bool HiddenClock
        {
            get
            {
                if (!m_bLoadDBOption)
                    LoadSDMSDBOptions();

                return m_bHiddenClock;
            }
        }

        public bool UsePopupSensorOn
        {
            get
            {
                if (!m_bLoadDBOption)
                    LoadSDMSDBOptions();

                return m_usePopupSensorOn;
            }
        }

        public bool UseMovingText
        {
            get
            {
                if (!m_bLoadDBOption)
                    LoadSDMSDBOptions();

                return m_useMovingText;
            }
        }

        public Form MainFrame
        {
            get { return /*this; }*/FormFrame.Instance; }
        }

        private static int m_nDefaultLayerState =
            (int)LAYER_TYPE.FIRE_DETECT | (int)LAYER_TYPE.SPRING_COOLER |
            (int)LAYER_TYPE.PUMP | (int)LAYER_TYPE.CCTV |
            (int)LAYER_TYPE.FE | (int)LAYER_TYPE.HD;

        private int m_nSOPGenUserID = -1;
        private string m_strSOPGenUserRealName = "";
        private bool m_isVisibleEquipZoneCCTV = false;

        private WeatherDisplay.FormWeatherFrame m_frmWeather = null;
        private PopupDialog.FloatingToolbar m_toolbar = null;
        //private PopupDialog.FormPSMList m_frmPSMList = null;

        private PopupDialog.FormMessageSender m_frmMessageSender = null;
        private PopupDialog.FormMessageReceiver m_frmMessageReceiver = null;

        private List<RibbonButton> m_reportButtons = new List<RibbonButton>();

        private SplashManager m_splashManager = null;

        private IProxyMessenser m_proxyMessenger = null;

        public IProxyMessenser ProxyMessenger
        {
            get { return m_proxyMessenger; }
            set { m_proxyMessenger = value; }
        }

        private FormCCTVList m_frmCCTVList = null;

        public FormCCTVList CCTVList
        {
            get { return m_frmCCTVList; }
            set { m_frmCCTVList = value; }
        }

        public ComboBox ComboFireDetect
        {
            get { return cmbFireDetect; }
        }

        public int SOPGenUserID
        {
            get { return m_nSOPGenUserID; }
        }

        public string SOPGenUserRealName
        {
            get { return m_strSOPGenUserRealName; }
        }

       
        public bool EquipZoneCCTVMode
        {
            get { return checkBoxEquipZoneCCTV.Visible && checkBoxEquipZoneCCTV.Checked; }
        }

        public EquipmentZone CurrentEquipZone
        {
            get
            {
                if (cboEquipZone.SelectedIndex < 0)
                    return null;

                return (EquipmentZone)cboEquipZone.Items[cboEquipZone.SelectedIndex];
            }
        }

        public ProcessIF CurrentSensorDetectProcess
        {
            get
            {
                if (cmbFireDetect.SelectedIndex < 0)
                    return null;

                return (ProcessIF)cmbFireDetect.Items[cmbFireDetect.SelectedIndex];
            }
        }

        public ProcessIF LastSensorDetectProcess
        {
            get
            {
                int nCount = cmbFireDetect.Items.Count;
                if (nCount == 0)
                    return null;

                return (ProcessIF)cmbFireDetect.Items[nCount - 1];
            }
        }

        public void ClearAllFireDetect()
        {
            //if (cmbFireDetect.Items.Count > 0)
            {
                DlgSelectCase.Instance.Visible = false;
                DlgSelectCase.Instance.DetectFireCount = 0;

                if (cmbFireDetect.Items.Count > 0)
                {
                    cmbFireDetect.Items.Clear();
                    SetNormalMode(-1);

                    ClearNotice();
                }
            }
        }

        public void SelectLastFireDectectProcess()
        {
            int nCount = cmbFireDetect.Items.Count;
            if (nCount == 0)
                return;
            cmbFireDetect.SelectedIndex = nCount - 1;
        }

        public void SelectSensorDetectProcess(int nSensorHistoryID, int nSensorID)
        {
            int nIdx = 0;
            foreach (ProcessIF process in cmbFireDetect.Items)
            {
                if (process.SensorHistoryID == nSensorHistoryID && process.DetectSensorID == nSensorID)
                {
                    if (CurrentSensorDetectProcess == process)
                    {
                        break;
                    }

                    cmbFireDetect.SelectedIndex = nIdx;
                    bool bSelected = process.Select();
                    if (bSelected)
                    {
                        ReactionLogManager.Instance.ProcessLog(process.LastLog, true);
                    }
                    break;
                }
                nIdx++;
            }
        }

        public void SelectFireDetectProcess(int nIdx)
        {
            if (nIdx < 0)
                return;

            int nCount = cmbFireDetect.Items.Count;
            if (nCount == 0 || nIdx >= nCount)
                return;

            cmbFireDetect.SelectedIndex = nIdx;
        }

        private bool SetMonitorForm(Form form, int nDisplay)
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            if (form == null)
                return false;

            if (sc.Length == 0)
            {
                return false;
            }

            string szNum = nDisplay.ToString();
            int nIdx = -1;
            for (int i = 0; i < sc.Length; i++)
            {
                if (sc[i].DeviceName.IndexOf(szNum) != -1)
                {
                    nIdx = i;
                    break;
                }
            }

            if (nIdx == -1)
                nIdx = 0;

            if (sc.Length >= nDisplay)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = sc[nIdx].Bounds.Location;
                //form.Size = new Size(sc[nIdx].Bounds.Width, sc[nIdx].Bounds.Height);
                form.Size = new Size(sc[nIdx].Bounds.Width - 40, sc[nIdx].Bounds.Height - 40);

                form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                form.WindowState = FormWindowState.Maximized;
            }
            return true;
        }

        private int m_nMonitor = 1;
        
        public static void SetDoubleBuffer(Panel panel, bool bEnabled)
        {
            Type dgvType1 = panel.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(panel, bEnabled, null);
        }

        public static void SetDoubleBuffer(DataGridView gvView, bool bEnabled)
        {
            Type dgvType1 = gvView.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(gvView, bEnabled, null);
        }

        private bool ReadReportOption()
        {
            string strSQL = "Select PropertyName, PropertyValue from OptionSDMS where PropertyName = 'UseReport' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyName == null || strPropertyValue == null)
                    continue;

                if (strPropertyValue.ToLower() == "false" || strPropertyValue == "0")
                {
                    return false;
                }
            }
            return true;
        }

        private bool ReadPSMInfo()
        {         
            string strSQL = "Select PropertyName, PropertyValue from OptionSDMS where PropertyName = 'UsePSM' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyName == null || strPropertyValue == null)
                    continue;

                if (strPropertyValue.ToLower() == "false" || strPropertyValue == "0")
                {
                    return false;
                }
            }
            return true;
        }

        private bool ReadIntrusionInfo()
        {
            string strSQL = "Select PropertyName, PropertyValue from OptionSDMS where PropertyName = 'UseIntrusion' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyName == null || strPropertyValue == null)
                    continue;

                if (strPropertyValue.ToLower() == "false" || strPropertyValue == "0")
                    return false;
                else
                    return true;
            }
            return false;
        }

        private bool ReadEarthquake()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'UseEarthquake' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strPropertyValue = DBUtility.WebDBManager.GetStringField(arrResult[0]);

            if (strPropertyValue == null)
                return false;

            if (strPropertyValue == "1" || string.Compare("true", strPropertyValue, true) == 0)
                return true;

            return false;
        }
        
        public FormMain(int nSOPGenUserID, string strSOPGenUserRealName, int nMonitor, bool isSimulationMode)
        {
            this.DoubleBuffered = true;

            UnE.View.Content.FormContentUnity.KillProcess("CCTVViewer");
            UnE.View.Content.FormContentUnity.KillProcess("UnitySam");
            UnE.View.Content.FormContentUnity.KillProcess("UnitySamInside");
            UnE.View.Content.FormContentUnity.KillProcess("EnergyOutside");
            UnE.View.Content.FormContentUnity.KillProcess("SeoulUnv");
            UnE.View.Content.FormContentUnity.KillProcess("BusanUnv");
            ZoneManager z = ZoneManager.Instance;

            m_instance = this;
            m_nMonitor = nMonitor;
            m_isSimulationMode = isSimulationMode;

            m_nSOPGenUserID = nSOPGenUserID;
            m_strSOPGenUserRealName = strSOPGenUserRealName;

            Debug.WriteLine("Start : " + DateTime.Now);
            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
            if (nSiteID == 1)
            {                
                UnE.SOP.ProxySOP.Instance.Use2D = false;
                m_bUse2D = false;
            }
            else if (nSiteID == 2)
            {
                UnE.SOP.ProxySOP.Instance.Use2D = true;
                m_bUse2D = true;
            }

            m_dbMgr = new DBUtility.WebDBManager(nSiteID);

            UnE.SOP.ProxySOP.Instance.UsePSM = ReadPSMInfo();
            UnE.SOP.ProxySOP.Instance.UseIntrusion = ReadIntrusionInfo();
            m_useReport = ReadReportOption();
            UnE.SOP.ProxySOP.Instance.UseEarthquake = ReadEarthquake();

            m_dataMgr = new DataManager(m_dbMgr);
            //Debug.WriteLine(DateTime.Now);

            InitializeComponent();

            m_splashManager = new SplashManager(m_dbMgr, nSiteID);
            m_splashManager.RunSplash();

            m_splashManager.SendSplashMessage("건물 및 센서 정보 로딩중...", libSplash.Message.SPLASH_MESSAGE, 10256);
            LoadBaseData();
            //Debug.WriteLine(DateTime.Now);
            
            //InitializeComponent();

            SetDoubleBuffer(panelTop, true);
            SetDoubleBuffer(panelAdminRibbonBarMiddle, true);
            SetDoubleBuffer(panelReportRibbonBarMiddle, true);
            SetDoubleBuffer(panelStatus, true);
            SetDoubleBuffer(panelLeft, true);

            if( m_bUse2D == true)
            {
                labelSelectZone.Visible = true;
                cboBuildingGroup.Visible = true;
                cboBuilding.Visible = true;
                cboFloor.Visible = true;
                btnSelectZone.Visible = true;
            }

            m_nOriginLeftPanelWidth = panelLeft.Size.Width;

            this.Name = "SDMS";
            this.FormClosing += FormMain_FormClosing;
            this.FormClosed += FormMain_FormClosed;
            this.Load += FormMain_Load;
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            //Debug.WriteLine(DateTime.Now);
            AddPythonFunction();
            //Debug.WriteLine(DateTime.Now);
            m_nSystemButtonSpace = btnMax.Location.X - (btnMin.Location.X + btnMin.Size.Width);

            m_splashManager.SendSplashMessage("탭 초기화...", libSplash.Message.SPLASH_MESSAGE, 10256);
            InitTab();
            //Debug.WriteLine(DateTime.Now);

            m_splashManager.SendSplashMessage("Layout 작성중...", libSplash.Message.SPLASH_MESSAGE, 46154);
            CreateBackstageHome();
            //Debug.WriteLine(DateTime.Now);

            WeatherDisplay.FormWeatherDisplay frmWeather = new WeatherDisplay.FormWeatherDisplay();
            m_frmWeather = new WeatherDisplay.FormWeatherFrame(frmWeather);
            m_frmWeather.Size = frmWeather.Size;

            m_toolbar = new PopupDialog.FloatingToolbar();
            m_toolbar.LocationChanged += m_toolbar_LocationChanged;
            m_toolbar.VisibleChanged += m_toolbar_VisibleChanged;

            m_splashManager.SendSplashMessage("CCTV Viewer 로딩...", libSplash.Message.SPLASH_MESSAGE, 56410);
            FormContentUnity.KillProcess("libCCTV");
            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                Guid guid = Guid.NewGuid();
                string szName = string.Format("libCCTV{0}", guid.ToString());
                //string szName = "CCTVPipe";
                m_CCTVPipe = new Pipelib.PassivePipeServer(true, szName);
                m_CCTVPipe.OnReciveMessage += m_CCTVPipe_OnReciveMessage;
                m_CCTVPipe.BeginPipe();
                
                CreateCCTVProcess(szName);
            }

            m_PanelBtnSimulator.ButtonClickEvent += new EventHandler(this.RunSimulator);

            SDMSPopupFactory factory = SDMSPopupFactory.Instance;
        }

        void m_CCTVPipe_OnReciveMessage(string Reply)
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                if (Reply.StartsWith("SetVisible"))
                {
                    string szTemp = Reply.Replace("SetVisible(", "").Replace(")", "");
                    if (szTemp == "True")
                    {
                        m_bShowCCTVForm = true;
                    }
                    else
                    {
                        m_bShowCCTVForm = false;
                    }
                }
            });
            System.Diagnostics.Trace.WriteLine(Reply);
        }

        private Pipelib.PassivePipeServer m_CCTVPipe = null;
        public Pipelib.PassivePipeServer CCTVPipe
        {
            get { return m_CCTVPipe; }
            set { m_CCTVPipe = value; }
        }

        private int GetExeIndex()
        {
            int nIndex = Application.ExecutablePath.LastIndexOf('.');

            if (nIndex < 0)
                return 0;

            int num = 0;
            int multiple = 1;

            for (int i = nIndex - 1; i >= 0; i--)
            {
                char ch = Application.ExecutablePath.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    num += (ch - '0') * multiple;
                    multiple *= 10;
                }
                else
                    break;
            }

            return num;
        }

        public void AddPythonFunction()
        {
            ScriptProxy proxy = ScriptProxy.Instance;
            proxy.UserObject.SelectMonitoringTab = new Func<bool>(SelectMonitoringTab);
            proxy.UserObject.SelectAdminTab = new Func<bool>(SelectAdminTab);
            proxy.UserObject.SelectReportTab = new Func<bool>(SelectReportTab);
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
        }


        /// <summary>
        /// UI 생성되기 이전에 필요한 DB Data 로드
        /// Form 생성 이전에 호출
        /// </summary>
        public void LoadBaseData()
        {
            ZoneManager.Instance.LoadBuildingData();
            ZoneManager.Instance.LoadZones();
            ZoneManager.Instance.LoadEquipmentZone();
            ZoneManager.Instance.LoadShelters();
            ZoneManager.Instance.Load3DText();
            m_dataMgr.LoadFireEquipment();
            m_dataMgr.LoadFacilityManager();

            ReciverManager.Instance.LoadReciverList();
            SensorTagHistoryManager.Instance.LoadSensorTags(m_dbMgr);

            // PSMSensor Icon 로딩할 것...
            // [2016/02/17] 김지웅
        }

        /// <summary>
        /// UI가 생성된 이후에 사용될 DB Data 로드
        /// Form Load 이벤트에서 호출
        /// </summary>
        public void LoadExtraData()
        {
            SensorManager.Instance.ReadAllSensorData();
        }

        public void SetDisableToolBar()
        {
            panelMiddle.Enabled = false;
            panelLeft.Enabled = false;
        }

        public void SetEnableToolBar()
        {
            panelMiddle.Enabled = true;
            panelLeft.Enabled = true;
        }

        private bool m_bUse2D = false;

        private void InitTab()
        {
            pictureBoxMonitoring.SetPictureBoxOwner(this);
            pictureBoxAdmin.SetPictureBoxOwner(this);
            pictureBoxReport.SetPictureBoxOwner(this);

          
            if (m_bUse2D  == false)
            {
                pictureBoxAdmin.Location = pictureBoxReport.Location;
                pictureBoxReport.Location = pictureBox2D.Location;
                pictureBox2D.Visible = false;              
            }
            
          
            pictureBox2D.SetPictureBoxOwner(this);
            pictureBoxCCTV.SetPictureBoxOwner(this);

            pictureBoxMonitoring.Text = "3D";
            pictureBoxAdmin.Text = "관리";
            pictureBoxReport.Text = "리포트";

            pictureBox2D.Text = "2D";
            pictureBoxCCTV.Text = "CCTV";

            panelTop.Size = new Size(this.Size.Width, m_nPanelTopHeight);
            panelMiddle.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
            panelLeft.Location = new Point(0, panelMiddle.Location.Y + panelMiddle.Size.Height);
        }

        private bool m_bCmbLocBtm = false;

        private void ResizePanels()
        {
            int nHeight = 48;

            if (this.Size.Width > 1400)
            {
                panelMiddle.Size = new Size(this.Size.Width, nHeight);
                panelProcessHistory.Size = panelMiddle.Size;
                panelReactionHistory.Size = panelMiddle.Size;
                pnDetectPSM.Size = panelMiddle.Size;
                pnNotOperationPSM.Size = panelMiddle.Size;
                pnActionPSM.Size = panelMiddle.Size;
                pnSMSPSM.Size = panelMiddle.Size;


                panelMiddle.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
                panelLeft.Location = new Point(0, panelMiddle.Location.Y + panelMiddle.Size.Height);

                if (!m_isThumbnailMode)//|| (m_isThumbnailMode && m_cctvMode == CCTVMode.NORMAL))
                {
                    panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - panelMiddle.Size.Height);

                    if (mCurrentTab != UnE.View.Content.ContentOwnerTab.REPORT_TAB)
                        panelLeft.Show();

                    int nBottomHeight = panelLeft.Size.Height;
                    panelBottom.Location = new Point(panelLeft.Location.X + panelLeft.Size.Width, panelLeft.Location.Y);
                    //panelBottom.Location = new Point(panelLeft.Location.X, panelLeft.Location.Y);
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }
                else
                {
                    panelLeft.Hide();
                    int nBottomHeight = panelLeft.Size.Height;
                    panelBottom.Location = new Point(panelLeft.Location.X, panelLeft.Location.Y);
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }
                m_bCmbLocBtm = false;
            }
            else
            {
                panelMiddle.Size = new Size(this.Size.Width, nHeight * 2);
                panelProcessHistory.Size = panelMiddle.Size;
                panelReactionHistory.Size = panelMiddle.Size;
                pnDetectPSM.Size = panelMiddle.Size;
                pnNotOperationPSM.Size = panelMiddle.Size;
                pnActionPSM.Size = panelMiddle.Size;
                pnSMSPSM.Size = panelMiddle.Size;


                panelMiddle.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
                panelLeft.Location = new Point(0, panelMiddle.Location.Y + panelMiddle.Size.Height);

                if (!m_isThumbnailMode || (m_isThumbnailMode && m_cctvMode == CCTVMode.NORMAL))
                {
                    panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - nHeight);

                    if (mCurrentTab != UnE.View.Content.ContentOwnerTab.REPORT_TAB)
                        panelLeft.Show();

                    int nBottomHeight = panelLeft.Size.Height;
                    panelBottom.Location = new Point(panelLeft.Location.X + panelLeft.Size.Width, panelLeft.Location.Y);
                    //panelBottom.Location = new Point(panelLeft.Location.X, panelLeft.Location.Y);  
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }
                else
                {
                    panelLeft.Hide();

                    int nBottomHeight = panelLeft.Size.Height;
                    panelBottom.Location = new Point(panelLeft.Location.X, panelLeft.Location.Y);
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }

                m_bCmbLocBtm = true;
            }
        }

        private void ResizeComboBox()
        {
            //int nPanelWidth = panelMiddle.Size.Width;
            int nPanelWidth = ControlInitPos.Instance.PanelMiddleInitSize;

            if (m_bCmbLocBtm == false)
            {
                labelSelectZone.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.LabelSelectZoneInitPos), 13);
                cboBuildingGroup.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxBuildingGroupInitPos), 10);
                cboBuilding.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxBuildingInitPos), 10);
                cboFloor.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxFloorInitPos), 10);
                btnSelectZone.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ButtonSelectZoneInitPos), 8);

                //button1.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button1InitPos), button1.Location.Y);
                //btnSaveHWP.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button2InitPos), btnSaveHWP.Location.Y);
            }
            else
            {
                int nHeight = panelMiddle.Size.Height / 2 - 5;
                //int nWidth = 630;
                labelSelectZone.Location = new Point(684, 13 + nHeight);
                cboBuildingGroup.Location = new Point(742, 10 + nHeight);
                cboBuilding.Location = new Point(886, 10 + nHeight);
                cboFloor.Location = new Point(1141, 10 + nHeight);
                btnSelectZone.Location = new Point(1204, 8 + nHeight);

                //button1.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button1InitPos), button1.Location.Y);
                //btnSaveHWP.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button2InitPos), btnSaveHWP.Location.Y);
            }

            labelFireDetect.Location = new Point(544, labelFireDetect.Location.Y);
            cmbFireDetect.Location = new Point(632, cmbFireDetect.Location.Y);

            if (m_isVisibleEquipZoneCCTV)
                ResizeEquipZoneCCTVControl(true, false);
        }

        /*
        private void ResizePanels()
        {
            panelMiddle.Size = new Size(this.Size.Width, panelMiddle.Size.Height);
            panelProcessHistory.Size = panelMiddle.Size;
            panelReactionHistory.Size = panelMiddle.Size;

            if (!m_isThumbnailMode || (m_isThumbnailMode && m_cctvMode == CCTVMode.NORMAL))
            {
                panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - panelMiddle.Size.Height);
                panelLeft.Show();

                int nBottomHeight = panelLeft.Size.Height;
                panelBottom.Location = new Point(panelLeft.Location.X + panelLeft.Size.Width, panelLeft.Location.Y);
                panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
            }
            else
            {
                //panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - panelMiddle.Size.Height);
                panelLeft.Hide();

                int nBottomHeight = panelLeft.Size.Height;
                panelBottom.Location = new Point(panelLeft.Location.X, panelLeft.Location.Y);
                panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
            }
        }

        private void ResizeComboBox()
        {
            int nPanelWidth = panelMiddle.Size.Width;

            //if (ShowEquipZoneCCTV)
            {
                labelSelectZone.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.LabelSelectZoneInitPos), labelSelectZone.Location.Y);
                cboBuildingGroup.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxBuildingGroupInitPos), cboBuildingGroup.Location.Y);
                cboBuilding.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxBuildingInitPos), cboBuilding.Location.Y);
                cboFloor.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxFloorInitPos), cboFloor.Location.Y);
                btnSelectZone.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ButtonSelectZoneInitPos), btnSelectZone.Location.Y);

                button1.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button1InitPos), button1.Location.Y);
                button2.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.Button2InitPos), button2.Location.Y);

                labelFireDetect.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.LabelFireDetectInitPos), labelFireDetect.Location.Y);
                cmbFireDetect.Location = new Point(nPanelWidth - (ControlInitPos.Instance.PanelMiddleInitSize - ControlInitPos.Instance.ComboBoxFireDetectInitPos), cmbFireDetect.Location.Y);

                if (m_isVisibleEquipZoneCCTV)
                    ResizeEquipZoneCCTVControl(true, false);
            }
        }
        */

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (this.Size.Width == 0 || this.Size.Height == 0)
                return;


            ResizePanels();
            ResizeComboBox();

            ResizeAdminRibbonBar();
            ResizeReportRibbonBar();



            //m_PageHome.SetBounds(0, 0, panelBottom.Size.Width, panelLeft.Size.Height);

            ResizeSystemButtons();
            ResizeButtons();

            ClearSelectDlg();
        }

        public void ClearSelectDlg()
        {
            if (DlgSelectCase.Instance.DetectFireCount == 0 && m_readyToReceiveMessage == true)
            {

                //MessageBox.Show("lgSelectCase.Instance.Visible" + DlgSelectCase.Instance.Visible);
                DlgSelectCase.Instance.Visible = false;
            }
        }

        private void ResizeAdminRibbonBar()
        {
            panelAdminRibbonBarLeft.Location = panelClock.Location;
            panelAdminRibbonBarMiddle.Location = new Point(40, panelAdminRibbonBarLeft.Location.Y + 1);
            panelAdminRibbonBarRight.Location = new Point(this.Size.Width - panelAdminRibbonBarRight.Size.Width, panelAdminRibbonBarLeft.Location.Y + 1);

            panelAdminRibbonBarMiddle.Size = new Size(panelAdminRibbonBarRight.Location.X - panelAdminRibbonBarMiddle.Location.X, panelAdminRibbonBarLeft.Size.Height);
        }

        private void ResizeReportRibbonBar()
        {
            panelReportRibbonBarLeft.Location = panelClock.Location;
            panelReportRibbonBarMiddle.Location = new Point(40, panelReportRibbonBarLeft.Location.Y + 1);
            panelReportRibbonBarRight.Location = new Point(this.Size.Width - panelReportRibbonBarRight.Size.Width, panelReportRibbonBarLeft.Location.Y + 1);

            panelReportRibbonBarMiddle.Size = new Size(panelReportRibbonBarRight.Location.X - panelReportRibbonBarMiddle.Location.X, panelReportRibbonBarLeft.Size.Height);
        }

        private void OnTimer(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            labelDate.Text = string.Format("{0}년 {1}월 {2}일", dtNow.Year, dtNow.Month, dtNow.Day);
            labelTime.Text = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);

            /*if (m_readyToReceiveMessage)
            {
                // 대략 3초에 한번씩만 알림 메시지가 있는지 확인하도록 한다.
                if (m_nMessageTimeCount++ == 0)
                    PopupDialog.FormMessageReceiver.ReadNewMessage(ref m_frmMessageReceiver, m_dbMgr);

                if (m_nMessageTimeCount >= m_nMessageInterval)
                    m_nMessageTimeCount = 0;
            }*/

            // SDMSMessage 가운데 이미 DB에서 지워진 것이 있는지 새벽 1시에 한번만 확인한다.
            if (dtNow.Hour == 1 && dtNow.Minute == 0 && dtNow.Second == 0)
            {
                PopupDialog.FormMessageReceiver.CheckDeletedIDs(m_dbMgr);
            }
        }

        public void ReadSDMSMessage(List<PopupDialog.FormMessageReceiver.Message> messages)
        {
            if (!m_readyToReceiveMessage)
            {
                // FormMain이 완전히 로딩되기 전에 SOPServer로부터 메시지를 받은 상황이다.
                // 이 경우는 그냥 무시해도 상관없는데, FormMain의 로딩이 끝나면 DB로부터 직접 읽게된다.
                return;
            }

            this.Invoke((MethodInvoker)delegate
            {
                if (m_frmMessageReceiver == null || m_frmMessageReceiver.IsDisposed)
                    m_frmMessageReceiver = new PopupDialog.FormMessageReceiver();

                m_frmMessageReceiver.ReadNewMessages(messages);
            });
        }

        public void ReadSDMSMessage(int nMessageID)
        {
            if (!m_readyToReceiveMessage)
            {
                // FormMain이 완전히 로딩되기 전에 SOPServer로부터 메시지를 받은 상황이다.
                // 이 경우는 그냥 무시해도 상관없는데, FormMain의 로딩이 끝나면 DB로부터 직접 읽게된다.
                return;
            }

            this.Invoke((MethodInvoker)delegate
            {
                if (m_frmMessageReceiver == null || m_frmMessageReceiver.IsDisposed)
                    m_frmMessageReceiver = new PopupDialog.FormMessageReceiver();

                PopupDialog.FormMessageReceiver.ReadNewMessage(ref m_frmMessageReceiver, m_dbMgr, nMessageID);
            });
        }

        public void ShowSDMSReceiveForm()
        {
            if (m_frmMessageReceiver == null || m_frmMessageReceiver.IsDisposed)
            {
                PopupDialog.FormMessageReceiver.ReadNewMessage(ref m_frmMessageReceiver, m_dbMgr);
                //m_frmMessageReceiver = new PopupDialog.FormMessageReceiver();
            }

            if (m_frmMessageReceiver == null || m_frmMessageReceiver.IsDisposed)
                return;

            if (m_frmMessageReceiver.Visible)
                m_frmMessageReceiver.Focus();
            else
                m_frmMessageReceiver.Show(this);
        }

        private void LoadSDMSDBOptions()
        {
            if (m_bLoadDBOption)
                return;

            string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='HiddenClock' AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = DBUtility.WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                {
                    m_bHiddenClock = strValue == "1";
                }
            }

            strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='PopupSensorOn' AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = DBUtility.WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                {
                    m_usePopupSensorOn = strValue == "1";
                }
            }

            strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='MovingText' AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = DBUtility.WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                {
                    m_useMovingText = strValue == "1";
                }
            }

            string strUseBulletIn = "UseBulletIn", strUseMissionStatus = "UseMissionStatus";
            bool useBulletIn = true, useMissionStatus = true;

            strSQL = "SELECT PropertyName, PropertyValue FROM OptionSDMS where (PropertyName ='" + strUseBulletIn + "' or PropertyName ='" + strUseMissionStatus + "') AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null)
            {
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i+=2)
                {
                    string strName = DBUtility.WebDBManager.GetStringField(arrResult[i], "");
                    string strValue = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");

                    if (strName != "" && strName != "null" && strValue != "" && strValue != "null")
                    {
                        if (SetOptionProperty(ref useBulletIn, strName, strValue, strUseBulletIn) == false)
                            SetOptionProperty(ref useMissionStatus, strName, strValue, strUseMissionStatus);
                        
                    }
                }
            }

            SetHiddenClockOption(useBulletIn, useMissionStatus);

            m_bLoadDBOption = true;
        }

        private bool SetOptionProperty(ref bool prop, string strName, string strValue, string strTagName)
        {
            if (string.Compare(strTagName, strName, true) == 0)
            {
                if (string.Compare(strValue, "true", true) == 0 || strValue == "1")
                    prop = true;
                else if (string.Compare(strValue, "false", true) == 0 || strValue == "0")
                    prop = false;

                return true;
            }

            return false;
        }

        private void GetHomeButtonText(int nIndex, out string strButtonText, out string strButtonTooltip)
        {
            strButtonText = string.Empty;
            strButtonTooltip = string.Empty;

            string strSQL = String.Format("SELECT PropertyValue, Description FROM OptionSDMS WHERE PropertyName ='HomeButton_{0}' AND SiteID = {1}"
                , nIndex
                , UnE.SOP.ProxySOP.Instance.SiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = DBUtility.WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                    strButtonText = strValue;


                strValue = DBUtility.WebDBManager.GetStringField(arrResult[1], "");

                if (strValue != "" && strValue != "null")
                    strButtonTooltip = strValue;

            }
            else
            {
                strSQL = String.Format("INSERT INTO OptionSDMS (PropertyName, PropertyValue, Description, SiteID) VALUES ('HomeButton_{0}', '#{0}', '{1} 초기 화면', {2})"
                    , nIndex
                    , (nIndex == 1 ? "전체" : (nIndex == 2 ? "1발전소" : (nIndex == 3 ? "2발전소" : "저탄장")))
                    , UnE.SOP.ProxySOP.Instance.SiteID);

                if (m_dbMgr.GetResultData(strSQL, 0) != null)
                {
                    GetHomeButtonText(nIndex, out strButtonText, out strButtonTooltip);
                }
            }

            // 버튼에 표기되는 최대 글자 수는 2개로 고정(나머지는 짤림)
            /*if (strButtonText.Length > 3)
            {
                strButtonText = strButtonText.Substring(0, 3);
            }*/

        }

        private void SetHiddenClockOption(bool useBulletIn, bool useMissionStatus)
        {
            if (!m_bHiddenClock)
            {
                btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = false;

                

                this.panelTop.Controls.Remove(this.labelTime);
                this.panelTop.Controls.Remove(this.labelDate);

                this.panelClock.Controls.Add(this.labelTime);
                this.panelClock.Controls.Add(this.labelDate);
                this.panelClock.Location = new System.Drawing.Point(0, 67);

                this.labelTime.Font = new System.Drawing.Font("맑은 고딕", 21F, System.Drawing.FontStyle.Bold);
                this.labelTime.Location = new System.Drawing.Point(138, 33);
                this.labelTime.Size = new System.Drawing.Size(127, 38);

                this.labelDate.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                this.labelDate.Location = new System.Drawing.Point(146, 14);
                this.labelDate.Size = new System.Drawing.Size(105, 17);

                this.panelClock.Visible = true;
            }

            btnBulletin.Enabled = useBulletIn;
            btnMissionStatus.Enabled = useMissionStatus;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            MainFrame.Visible = false;
            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                pictureBoxCCTV.Visible = false;
                if(m_bUse2D == true)
                {
                   // pictureBoxReport.Location = pictureBoxAdmin.Location;
                    pictureBoxAdmin.Location = pictureBoxCCTV.Location;                  
                }
                else
                {
                    pictureBoxAdmin.Location = pictureBoxReport.Location;
                    pictureBoxReport.Location = pictureBoxCCTV.Location;
                }
            }

            //m_PaneBtnHome.TopLevel = false;
            //m_PaneBtnHome.Parent = this;

            SDMS.FormFrame.Instance.LocationChanged += new System.EventHandler(this.FormMain_LocationChanged);

            LoadSDMSDBOptions();

            SetMonitorForm(MainFrame, m_nMonitor);
            MainFrame.Visible = false;
            FormMain.Instance.Visible = false;

            m_splashManager.SendSplashMessage("3D 화면 초기화...", libSplash.Message.SPLASH_MESSAGE, 60256);
            m_PageHome.Init3DView();




            InitPanels();
            //InitButtons();
            InitComboBox();
            m_PageHome.FrmReport.SetComboText(proc_btnStartDate.Text, proc_btnEndDate.Text);
            

            SelectMonitoringTab();

            TextPictureBox_MouseDown(pictureBoxMonitoring, null);
            mClockTimer.Start();

            m_MainTimer.Enabled = true;
            m_MainTimer.Start();

            DatePickerStart.Visible = false;
            DatePickerEnd.Visible = false;



            btnFire.Enabled = false;

            Debug.WriteLine("Load : " + DateTime.Now);

            m_CheckReciver.Enabled = true;
            m_CheckReciver.Interval = 3000;
            m_CheckReciver.Start();
            //Test();

            if (ProxyMessenger != null)
                ProxyMessenger.CompleteLoading();

            m_PageHome.Show();

            if (m_isSimulationMode)
            {
                labelTitle.Text = "연습용 모드";
            }
            else
            {
                labelTitle.Text = "";
            }

            //FormMain.Instance.Visible = true;
            //MainFrame.Visible = true;


            FormMain_LocationChanged(null, null);
            FormMain_LocationChanged(null, null);

            m_PageHome.OnClickToolBarButton(m_PaneBtnHome.BtnMainHome);
            m_PageHome.OnClickToolBarButton(btnPick);

            //m_netMgr = NetworkManager.Instance;

            int x = panelMiddle.Location.X + panelMiddle.Size.Width - m_frmWeather.Size.Width + FormFrame.Instance.Location.X;
            int y = panelMiddle.Location.Y + panelMiddle.Size.Height + FormFrame.Instance.Location.Y;
            m_frmWeather.StartPosition = FormStartPosition.Manual;
            m_frmWeather.Location = new Point(x, y);

            InitButtons();

            x = panelLeft.Location.X + panelLeft.Size.Width + FormFrame.Instance.Location.X + 5;
            y = panelMiddle.Location.Y + panelMiddle.Size.Height + FormFrame.Instance.Location.Y;
           
            
            m_toolbar.StartPosition = FormStartPosition.Manual;
            m_toolbar.Location = new Point(x, y);


            m_PageHome.ContentForm.AddMainToolStrip(m_toolbar.MainToolStrip, UnE.View.Content.ViewType.OUTSIDE);

            this.panelTop.DoubleClick += new System.EventHandler(this.panelTop_DoubleClick);
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            this.panelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseUp);

            pictureBoxReport.Enabled = m_useReport;

            //OnReadyDataLoad();
            m_PanelBtnNotice.chgSensorDectect += m_PaneBtnNotice_chgSensorDectect;

#if SAFE_KOREA_YH_2017
            RunInternalMessagePopup();
#endif
        }

        private void RunInternalMessagePopup()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'UseInternalMessagePopup' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strValue = DBUtility.WebDBManager.GetStringField(arrResult[0]);

            if (strValue == null)
                return;

            if (strValue == "1")
            {
                string strFileName = "InternalMessagePopup.exe";

                if (File.Exists(strFileName))
                {
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.FileName = strFileName;
                    startInfo.ErrorDialog = true;

                    string strArgument = string.Format("{0} {1} {2}", this.Location.X, this.Location.Y, System.Diagnostics.Process.GetCurrentProcess().Id);
                    startInfo.Arguments = strArgument;

                    System.Diagnostics.Process process;
                    try
                    {
                        process = System.Diagnostics.Process.Start(startInfo);
                    }
                    catch (Exception)
                    {
                        //System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        public void OnReadyDataLoad()
        {
            m_splashManager.SendSplashMessage("POI 로딩중...", libSplash.Message.SPLASH_MESSAGE, 84615);
            m_PageHome.LoadPOI();

            m_splashManager.SendSplashMessage("나머지 DB 데이터 불러오기...", libSplash.Message.SPLASH_MESSAGE, 78000);
            m_proxyMessenger.OnAfterLoadingCCTV();
            LoadExtraData();
            m_netMgr = NetworkManager.Instance;

            m_PageHome.ContentForm.RedrawWindow();

            // SOP Simulator가 꺼져있는동안 새로운 메시지가 수신된 것이 있는지 확인한다.
            PopupDialog.FormMessageReceiver.ReadNewMessage(ref m_frmMessageReceiver, m_dbMgr);
            // 이미 삭제된 DB Data를 가지고 있는지 확인한다.
            PopupDialog.FormMessageReceiver.CheckDeletedIDs(m_dbMgr);
            m_readyToReceiveMessage = true;

            ShowToolbar();

            m_splashManager.SendSplashMessage("", libSplash.Message.SPLASH_CLOSE, 78000);
            MainFrame.Location = FormFrame.Instance.OriginLocation;
            MainFrame.WindowState = FormWindowState.Maximized;
            MainFrame.Visible = true;

            if (ProxyMessenger.OnlySDMS() == false)
                ProxyMessenger.ShowSOPSimulator();
        }

        private void InitPanels()
        {
            panelProcessHistory.Location = panelMiddle.Location;
            panelReactionHistory.Location = panelMiddle.Location;
            pnDetectPSM.Location = panelMiddle.Location;
            pnNotOperationPSM.Location = panelMiddle.Location;
            pnActionPSM.Location = panelMiddle.Location;
            pnSMSPSM.Location = panelMiddle.Location;

            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;


            //m_frmOutdoor = new FormViewOutdoor();
            //m_frmOutdoor.TopLevel = false;
            //panelLeft.Controls.Add(m_frmOutdoor);

            //m_frmIndoor = new FormViewIndoor();
            //m_frmIndoor.TopLevel = false;
            //panelLeft.Controls.Add(m_frmIndoor);

            //m_frmCCTVGuide = new FormCCTVGuide();
            //m_frmCCTVGuide.TopLevel = false;
            //panelLeft.Controls.Add(m_frmCCTVGuide);
        }

        private void InitComboBox()
        {
            ComboHelper.InitBuildingGroupComboBox(cboBuildingGroup);


            //proc_cboBuildingGroup.Items.Add("모든 건물 그룹");
            proc_cboBuilding.Items.Add("모든 건물");
            proc_cboFloor.Items.Add("모든 층");

            ComboHelper.InitBuildingGroupComboBox(proc_cboBuildingGroup);

            //cboBuildingGroup.Items.Add(ZoneManager.Instance.OutdoorBuildingGroup);
            proc_cboBuildingGroup.Items.Add(ZoneManager.Instance.OutdoorBuildingGroup);
            proc_cboBuildingGroup.Sorted = true;
            proc_cboBuildingGroup.Sorted = false;
            proc_cboBuildingGroup.Items.Insert(0, "모든 건물 그룹");

            cboDetectPSMBuilding.Items.Add("모든 시설");
            cboNotOperationPSMBuilding.Items.Add("모든 시설");
            cboSMSPSMBuilding.Items.Add("모든 시설");

            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                if (PSMManager.Instance.GetTanks() != null)
                {
                    foreach (string strLocationName in from tanks in PSMManager.Instance.GetTanks()
                                                       orderby tanks.LocationName ascending
                                                       select tanks.LocationName)
                    {
                        if (cboDetectPSMBuilding.Items.Contains(strLocationName) == false)
                        {
                            cboDetectPSMBuilding.Items.Add(strLocationName);
                            cboNotOperationPSMBuilding.Items.Add(strLocationName);
                            cboSMSPSMBuilding.Items.Add(strLocationName);
                        }
                    }
                }
            }

            cboBuildingGroup.Sorted = true;
            cboBuildingGroup.Sorted = false;

            // 30개 초과시에 그래프에서 안이쁘게 보이나 한달기준 최대 31일이라고 할 때, 한 눈에 보기 편하도록 최대31분할 표기함.
            for (int nIndex = 5; nIndex < 32; nIndex++)
            {
                proc_cboViewCount.Items.Add(nIndex);
                cboDetectPSMViewCount.Items.Add(nIndex);
            }

            proc_cboSplitUnit.Items.Add("분");
            proc_cboSplitUnit.Items.Add("시간");
            proc_cboSplitUnit.Items.Add("일");
            proc_cboSplitUnit.Items.Add("주");
            proc_cboSplitUnit.Items.Add("월");
            proc_cboSplitUnit.Items.Add("연");

            cboDetectPSMSplitUnit.Items.Add("분");
            cboDetectPSMSplitUnit.Items.Add("시간");
            cboDetectPSMSplitUnit.Items.Add("일");
            cboDetectPSMSplitUnit.Items.Add("주");
            cboDetectPSMSplitUnit.Items.Add("월");
            cboDetectPSMSplitUnit.Items.Add("연");

            proc_cboLatelyDate.Items.Add("기간 선택");
            proc_cboLatelyDate.Items.Add("최근 1년");
            proc_cboLatelyDate.Items.Add("최근 6개월");
            proc_cboLatelyDate.Items.Add("최근 3개월");
            proc_cboLatelyDate.Items.Add("최근 1개월");
            proc_cboLatelyDate.Items.Add("최근 1주일");
            proc_cboLatelyDate.Items.Add("오늘 일자");

            cboDetectPSMLatelyDate.Items.Add("기간 선택");
            cboDetectPSMLatelyDate.Items.Add("최근 1년");
            cboDetectPSMLatelyDate.Items.Add("최근 6개월");
            cboDetectPSMLatelyDate.Items.Add("최근 3개월");
            cboDetectPSMLatelyDate.Items.Add("최근 1개월");
            cboDetectPSMLatelyDate.Items.Add("최근 1주일");
            cboDetectPSMLatelyDate.Items.Add("오늘 일자");

            cboNotOperationPSMLatelyDate.Items.Add("기간 선택");
            cboNotOperationPSMLatelyDate.Items.Add("최근 1년");
            cboNotOperationPSMLatelyDate.Items.Add("최근 6개월");
            cboNotOperationPSMLatelyDate.Items.Add("최근 3개월");
            cboNotOperationPSMLatelyDate.Items.Add("최근 1개월");
            cboNotOperationPSMLatelyDate.Items.Add("최근 1주일");
            cboNotOperationPSMLatelyDate.Items.Add("오늘 일자");

            cboSMSPSMLatelyDate.Items.Add("기간 선택");
            cboSMSPSMLatelyDate.Items.Add("최근 1년");
            cboSMSPSMLatelyDate.Items.Add("최근 6개월");
            cboSMSPSMLatelyDate.Items.Add("최근 3개월");
            cboSMSPSMLatelyDate.Items.Add("최근 1개월");
            cboSMSPSMLatelyDate.Items.Add("최근 1주일");
            cboSMSPSMLatelyDate.Items.Add("오늘 일자");

            for (int i = 0; i < 24; i++)
            {
                react_cboStartTime.Items.Add(String.Format("{0}시", i));
                cboActionPSMStartTime.Items.Add(String.Format("{0}시", i));
            }

            react_cboStartTime.SelectedIndex = 0;
            cboActionPSMStartTime.SelectedIndex = 0;

            for (int i = 1; i < 25; i++)
            {
                react_cboEndTime.Items.Add(String.Format("{0}시", i));
                cboActionPSMEndTime.Items.Add(String.Format("{0}시", i));
            }

            react_cboEndTime.SelectedIndex = react_cboEndTime.Items.Count - 1;
            cboActionPSMEndTime.SelectedIndex = cboActionPSMEndTime.Items.Count - 1;

            react_btnStartDate.Text = DateTime.Now.AddDays(-6).ToString().Substring(0, 10);
            react_btnEndDate.Text = DateTime.Now.ToString().Substring(0, 10);

            btnActionPSMStartDate.Text = DateTime.Now.AddDays(-6).ToString().Substring(0, 10);
            btnActionPSMEndDate.Text = DateTime.Now.ToString().Substring(0, 10);


            react_cboSearchType.Items.Add("화재신고만");
            react_cboSearchType.Items.Add("오작동 처리 포함");
            react_cboSearchType.Items.Add("현장에서 꺼진 신호 포함");

            react_cboSearchTypeIntrusion.Items.Add("방범신고만");
            react_cboSearchTypeIntrusion.Items.Add("오작동 처리 포함");
            react_cboSearchTypeIntrusion.Items.Add("현장에서 꺼진 신호 포함");

            cboActionPSMSearchType.Items.Add("누출신고만");
            cboActionPSMSearchType.Items.Add("누출신고 및 시스템복구");
            cboActionPSMSearchType.Items.Add("누출신고 및 현장복구");
            cboActionPSMSearchType.Items.Add("모든 신호");


            react_cboStartTime.Enabled = false;
            react_cboEndTime.Enabled = false;

            cboActionPSMStartTime.Enabled = false;
            cboActionPSMEndTime.Enabled = false;


            //탐지,처리이력 콤보박스 설정
            if (cboBuildingGroup.Items.Count > 0)
                cboBuildingGroup.SelectedIndex = 0;

            if (proc_cboBuildingGroup.Items.Count > 0)
                proc_cboBuildingGroup.SelectedIndex = 0;

            if (proc_cboSplitUnit.Items.Count > 0)
                proc_cboSplitUnit.SelectedIndex = 2;

            if (proc_cboViewCount.Items.Count > 0)
                proc_cboViewCount.SelectedIndex = 15;

            if (proc_cboLatelyDate.Items.Count > 0)
                proc_cboLatelyDate.SelectedIndex = proc_cboLatelyDate.Items.Count - 2;

            if (cboDetectPSMBuilding.Items.Count > 0)
                cboDetectPSMBuilding.SelectedIndex = 0;

            if (cboDetectPSMSplitUnit.Items.Count > 0)
                cboDetectPSMSplitUnit.SelectedIndex = 2;

            if (cboDetectPSMViewCount.Items.Count > 0)
                cboDetectPSMViewCount.SelectedIndex = 15;

            if (cboDetectPSMLatelyDate.Items.Count > 0)
                cboDetectPSMLatelyDate.SelectedIndex = cboDetectPSMLatelyDate.Items.Count - 2;

            if (cboNotOperationPSMBuilding.Items.Count > 0)
                cboNotOperationPSMBuilding.SelectedIndex = 0;

            if (cboNotOperationPSMLatelyDate.Items.Count > 0)
                cboNotOperationPSMLatelyDate.SelectedIndex = cboNotOperationPSMLatelyDate.Items.Count - 2;

            if (cboSMSPSMBuilding.Items.Count > 0)
                cboSMSPSMBuilding.SelectedIndex = 0;

            if (cboSMSPSMLatelyDate.Items.Count > 0)
                cboSMSPSMLatelyDate.SelectedIndex = cboSMSPSMLatelyDate.Items.Count - 2;

        }

        private void InitButtons()
        {
            string strHomeButtonText_1 = string.Empty, strHomeButtonText_2 = string.Empty, strHomeButtonText_3 = string.Empty, strHomeButtonText_4 = string.Empty,
                strHomeButtonToolTip_1 = string.Empty, strHomeButtonToolTip_2 = string.Empty, strHomeButtonToolTip_3 = string.Empty, strHomeButtonToolTip_4 = string.Empty;

            TextData data = new TextData();
            data.Brush = new SolidBrush(Color.White);
            data.Text = "재난신고";
            data.Rectangle = new Rectangle(5, 65, 60, 12);

            btnFire.ExtraImage = global::SDMS.Properties.Resources.Fire_Icon;
            btnFire.X = 20;
            btnFire.Y = 5;
            btnFire.TextData = data;

            GetHomeButtonText(1, out strHomeButtonText_1, out strHomeButtonToolTip_1);
            GetHomeButtonText(2, out strHomeButtonText_2, out strHomeButtonToolTip_2);
            GetHomeButtonText(3, out strHomeButtonText_3, out strHomeButtonToolTip_3);
            GetHomeButtonText(4, out strHomeButtonText_4, out strHomeButtonToolTip_4);

            /// Toolbar
            if (m_toolbar != null)
            {
                m_toolbar.SetHomeButtonText(1, strHomeButtonText_1);
                m_toolbar.SetHomeButtonText(2, strHomeButtonText_2);
                m_toolbar.SetHomeButtonText(3, strHomeButtonText_3);
                m_toolbar.SetHomeButtonText(4, strHomeButtonText_4);
            }

            /// 가로 바
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_HOME)/*btnHome*/, ID.ID_VIEW_HOME, "전체 화면");
            //SetButtonID(m_PaneBtnHome.BtnMainHome, ID.ID_VIEW_HOME_MAIN, strHomeButtonToolTip_1);
            SetButtonID(m_PaneBtnHome.Btn14Home, ID.ID_VIEW_HOME_14, strHomeButtonToolTip_2);
            SetButtonID(m_PaneBtnHome.Btn56Home, ID.ID_VIEW_HOME_56, strHomeButtonToolTip_3);
            SetButtonID(m_PaneBtnHome.BtnCoalHome, ID.ID_VIEW_HOME_COAL, strHomeButtonToolTip_4);
            m_PaneBtnHome.BtnMainHome.Text = strHomeButtonText_1;
            m_PaneBtnHome.Btn14Home.Text = strHomeButtonText_2;
            m_PaneBtnHome.Btn56Home.Text = strHomeButtonText_3;
            m_PaneBtnHome.BtnCoalHome.Text = strHomeButtonText_4;
            m_PaneBtnHome.BtnMainHome.Click += new System.EventHandler(this.OnClickToolBarButton);
            m_PaneBtnHome.Btn14Home.Click += new System.EventHandler(this.OnClickToolBarButton);
            m_PaneBtnHome.Btn56Home.Click += new System.EventHandler(this.OnClickToolBarButton);
            m_PaneBtnHome.BtnCoalHome.Click += new System.EventHandler(this.OnClickToolBarButton);

            SetButtonID(m_PaneBtnSaveHome.BtnMainHome, ID.ID_VIEW_SAVE_HOME_MAIN, String.Format("{0} 저장", strHomeButtonToolTip_1));
            SetButtonID(m_PaneBtnSaveHome.Btn14Home, ID.ID_VIEW_SAVE_HOME_14, String.Format("{0} 저장", strHomeButtonToolTip_2));
            SetButtonID(m_PaneBtnSaveHome.Btn56Home, ID.ID_VIEW_SAVE_HOME_56, String.Format("{0} 저장", strHomeButtonToolTip_3));
            SetButtonID(m_PaneBtnSaveHome.BtnCoalHome, ID.ID_VIEW_SAVE_HOME_COAL, String.Format("{0} 저장", strHomeButtonToolTip_4));

            // 버튼에 표기되는 최대 글자 수는 3개로 고정(나머지는 짤림)
            m_PaneBtnSaveHome.BtnMainHome.Text = strHomeButtonText_1.Length > 3 ? strHomeButtonText_1.Substring(0, 3) : strHomeButtonText_1;
            m_PaneBtnSaveHome.Btn14Home.Text = strHomeButtonText_2.Length > 3 ? strHomeButtonText_2.Substring(0, 3) : strHomeButtonText_2;
            m_PaneBtnSaveHome.Btn56Home.Text = strHomeButtonText_3.Length > 3 ? strHomeButtonText_3.Substring(0, 3) : strHomeButtonText_3;
            m_PaneBtnSaveHome.BtnCoalHome.Text = strHomeButtonText_4.Length > 3 ? strHomeButtonText_4.Substring(0, 3) : strHomeButtonText_4;
            m_PaneBtnSaveHome.BtnMainHome.Click += new System.EventHandler(this.btnSaveHomeSub_Click);
            m_PaneBtnSaveHome.Btn14Home.Click += new System.EventHandler(this.btnSaveHomeSub_Click);
            m_PaneBtnSaveHome.Btn56Home.Click += new System.EventHandler(this.btnSaveHomeSub_Click);
            m_PaneBtnSaveHome.BtnCoalHome.Click += new System.EventHandler(this.btnSaveHomeSub_Click);

            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_HOME_MAIN)/*btnFullScreen*/, ID.ID_VIEW_HOME_MAIN, "분할 화면");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_PICK)/*btnPick*/, ID.ID_VIEW_PICK, "선택");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_PAN)/*btnPanning*/, ID.ID_VIEW_PAN, "화면 이동");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_ORBIT)/*btnOrbit*/, ID.ID_VIEW_ORBIT, "화면 회전");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_ZOOMIN)/*btnZoomIn*/, ID.ID_VIEW_ZOOMIN, "확대");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_ZOOMOUT)/*btnZoomOut*/, ID.ID_VIEW_ZOOMOUT, "축소");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_OUTSIDE)/*btnOutside*/, ID.ID_VIEW_OUTSIDE, "외부공간 보기");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_BOTHSIDE)/*btnBoth*/, ID.ID_VIEW_BOTHSIDE, "외부/실내 같이 보기");
            btnBoth.Enabled = false;
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_INSIDE)/*btnInside*/, ID.ID_VIEW_INSIDE, "실내공간 보기");
            btnInside.Enabled = false;
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_CCTV)/*btnMultiCCTV*/, ID.ID_VIEW_CCTV, "CCTV 크게 보기");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_SCREENSHOT)/*btnScreenShot*/, ID.ID_VIEW_SCREENSHOT, "화면 캡쳐");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_WEATHER_INFO)/*btnWeatherInfo*/, ID.ID_VIEW_WEATHER_INFO, "기후정보 표시");

            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_SIMULATOR)/*btnSimulator*/, ID.ID_VIEW_SIMULATOR, "센서 시뮬레이터 기동");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_PSM), ID.ID_VIEW_PSM, "유해 화학물질 리스트 보기");
            SetButtonID(m_toolbar.GetButton(ID.ID_VIEW_DISASTER), ID.ID_VIEW_DISASTER, "방재장비 관리");
            btnSimulator.Click += new System.EventHandler(this.OnClickToolBarButton);


            if (!GetWeatherInfoOption())
                btnWeatherInfo.Visible = false;

            CheckButton(m_toolbar.GetButton(ID.ID_VIEW_ORBIT)/*btnOrbit*/, true);
            CheckButton(m_toolbar.GetButton(ID.ID_VIEW_OUTSIDE)/*btnOutside*/, true);
            //////////////////////////////////////////

            /// 세로바(Layer)
            SetButtonID(btnLayerFire, ID.ID_LAYER_DETECTOR);
            SetButtonID(btnLayerSpringCooler, ID.ID_LAYER_COOLER);
            SetButtonID(btnLayerPump, ID.ID_LAYER_PERSURE);
            SetButtonID(btnLayerCCTV, ID.ID_LAYER_CCTV);
            SetButtonID(btnLayerFE, ID.ID_LAYER_FIREEXT);
            SetButtonID(btnLayerHD, ID.ID_LAYER_FIREHYD);
            SetButtonID(btnLayerFA, ID.ID_LAYER_ALARMSTA);
            SetButtonID(btnLayerFR, ID.ID_LAYER_RECIVER);
            SetButtonID(btnLayerLowCCTV, ID.ID_LAYER_CCTVLOW);
            SetButtonID(btnLayerCCTVDisconnected, ID.ID_LAYER_CCTV_DISCONNECTED);
            SetButtonID(btnLayerBuildingText, ID.ID_LAYER_BUILDING_TEXT);
            SetButtonID(btnLayerNotice, ID.ID_LAYER_NOTICE);

            int nLayerState = ReadLayerState();
            InitLayerButtonCheck(nLayerState);

            ReadLeftBarThumbnailOption();
            InitLayerButtonPosition();

            //////////////////////////////////////////

            InitReportRibbonButtons();
            InitAdminRibbonButtons();

            labelSensorMonitor.Text = "수신반 연결상태 알수없음";

            int nSpace = labelSensorMonitor.Location.X - (btnSensorMonitor.Location.X + btnSensorMonitor.Size.Width);
            btnSensorMonitor.Location = new Point(checkBoxEquipZoneCCTV.Location.X, btnSensorMonitor.Location.Y);
            labelSensorMonitor.Location = new Point(btnSensorMonitor.Location.X + btnSensorMonitor.Size.Width + nSpace, labelSensorMonitor.Location.Y);
            btnSendMessage.Location = new Point(419, btnSensorMonitor.Location.Y);
        }

        private void InitLayerButtonPosition()
        {
            int x = btnLayerFire.Location.X;
            int y = btnLayerFire.Location.Y;
            int buttonSpace = btnLayerSpringCooler.Location.Y - btnLayerFire.Location.Y;
            int labelSpace = labelFire.Location.Y - btnLayerFire.Location.Y;

            SetLayerButtonPosition(btnLayerFire, labelFire, ControlInitPos.Instance.VisibleLayerFire, x, ref y, buttonSpace, labelSpace);
            SetLayerButtonPosition(btnLayerSpringCooler, labelCooler, ControlInitPos.Instance.VisibleLayerSpringCooler, x, ref y, buttonSpace, labelSpace);
            SetLayerButtonPosition(btnLayerPump, labelPump, ControlInitPos.Instance.VisibleLayerPump, x, ref y, buttonSpace, labelSpace);
            SetLayerButtonPosition(btnLayerCCTV, labelCCTV, ControlInitPos.Instance.VisibleLayerCCTV, x, ref y, buttonSpace, labelSpace);
            SetLayerButtonPosition(btnLayerLowCCTV, labelCCTVLow, ControlInitPos.Instance.VisibleLayerLowCCTV, x, ref y, buttonSpace, labelSpace);
            // Disconnected Type의 CCTV 는 컨트롤 창의 최 하단에 위치하도록 함.
            //SetLayerButtonPosition(btnLayerCCTVDisconnected, labelCCTVDisconnected, ControlInitPos.Instance.VisibleLayerCCTVDisconnected, x, ref y, buttonSpace, labelSpace);
            SetLayerButtonPosition(btnLayerFE, labelFE, ControlInitPos.Instance.VisibleLayerFE, x, ref y, buttonSpace, labelSpace);
            SetLayerButtonPosition(btnLayerHD, labelHD, ControlInitPos.Instance.VisibleLayerHD, x, ref y, buttonSpace, labelSpace);
            SetLayerButtonPosition(btnLayerFA, labelFA, ControlInitPos.Instance.VisibleLayerFA, x, ref y, buttonSpace, labelSpace);
            SetLayerButtonPosition(btnLayerFR, labelFR, ControlInitPos.Instance.VisibleLayerFR, x, ref y, buttonSpace, labelSpace);
            SetLayerButtonPosition(btnLayerBuildingText, labelBuildingText, ControlInitPos.Instance.VisibleLayerBuildingText, x, ref y, buttonSpace, labelSpace);

            SetLayerButtonPosition(btnLayerNotice, labelNotice, ControlInitPos.Instance.VisibleLayerNotice, x, ref y, buttonSpace, labelSpace);
            PictureBox pic = new PictureBox();
            pic.Image = global::SDMS.Properties.Resources.Notice_Cycle;
            pic.Size = new System.Drawing.Size(14, 14);
            pic.Location = new Point(0, 0);
            pic.SizeMode = PictureBoxSizeMode.CenterImage;
            pic.Visible = false;

            Label label = new Label();
            label.Location = new Point(1, 1);
            label.Text = "0";
            label.ForeColor = Color.White;
            label.Font = new System.Drawing.Font("나눔바른고딕", 8F);
            label.AutoSize = true; 
            pic.Controls.Add(label);            
            btnLayerNotice.Controls.Add(pic);            

            y = panelLeft.Height - (buttonSpace + 10);
            SetLayerButtonPosition(btnLayerCCTVDisconnected, labelCCTVDisconnected, ControlInitPos.Instance.VisibleLayerCCTVDisconnected, x, ref y, buttonSpace, labelSpace, false);
            SetLayerButtonPosition(btnSaveHome, labelSaveHome, true, x, ref y, buttonSpace, labelSpace, false);
            
            btnLayerCCTVDisconnected.Visible = false;
            labelCCTVDisconnected.Visible = false;
        }

        private void SetLayerButtonPosition(Button btn, Label label, bool visible, int x, ref int y, int buttonSpace, int labelSpace, bool isStartWithTop = true)
        {
            // isStartWithTop : True => 위에서 아래로 좌표값이 증가하도록 함    False => 아래서 위로 좌표값이 증감하도록 함

            SetLayerButtonVisible(btn, label, visible);

            btn.Location = new Point(x, y);
            label.Location = new Point(label.Location.X, btn.Location.Y + labelSpace);

            if (isStartWithTop)
                btn.Anchor = label.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            else
                btn.Anchor = label.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;

            if (visible)
            {
                if (isStartWithTop)
                    y += buttonSpace;
                else
                    y -= buttonSpace;
            }
        }

        private void SetLayerButtonVisible(Button btn, Label label, bool visible)
        {
            btn.Visible = label.Visible = visible;
        }

        private void InitLayerButtonCheck(int nLayerState)
        {
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.FIRE_DETECT, btnLayerFire);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.SPRING_COOLER, btnLayerSpringCooler);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.PUMP, btnLayerPump);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.CCTV, btnLayerCCTV);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.CCTV_L, btnLayerLowCCTV);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.CCTV_DISCONNECTED, btnLayerCCTVDisconnected);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.FE, btnLayerFE);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.HD, btnLayerHD);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.FA, btnLayerFA);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.FR, btnLayerFR);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.BUILDING_TEXT, btnLayerBuildingText);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.NOTICE, btnLayerNotice);
        }

        private void InitLayerButtonCheck(int nLayerState, LAYER_TYPE type, Button btn)
        {
            bool checkLayerState = (nLayerState & (int)type) == (int)type;
            CheckButton(btn, checkLayerState);
            m_PageHome.OnChangeLayer(GetButtonID(btn), checkLayerState);
        }

        private int ReadLayerState()
        {
            string strPath = Application.ExecutablePath;
            string szParentPath = Path.GetDirectoryName(strPath);
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            string szAssemName = currentProcess.ProcessName;
            string strFilePath = szParentPath + "\\" + szAssemName + "LayerState.ini";

            int nResult = m_nDefaultLayerState;

            if (File.Exists(strFilePath))
            {
                StreamReader reader = new StreamReader(strFilePath);

                while (!reader.EndOfStream)
                {
                    string strLine = reader.ReadLine();

                    strLine = strLine.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                    strLine = strLine.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                    if (strLine.Length == 0)
                        continue;

                    int.TryParse(strLine, out nResult);
                    break;
                }

                reader.Close();
            }
            else
            {
                WriteLayerState(m_nDefaultLayerState);
            }

            return nResult;
        }

        private void WriteLayerState(int nLayerState)
        {
            string strPath = Application.ExecutablePath;
            string szParentPath = Path.GetDirectoryName(strPath);
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            string szAssemName = currentProcess.ProcessName;
            string strFilePath = szParentPath + "\\" + szAssemName + "LayerState.ini";

            StreamWriter writer = new StreamWriter(strFilePath);
            writer.Write(nLayerState);
            writer.Close();
        }

        private void InitReportRibbonButtons()
        {
            Image imgMouseOverBkgnd = global::SDMS.Properties.Resources.RibbonMouseOver_bkgnd;
            Image imgCheckedBkgnd = global::SDMS.Properties.Resources.RibbonChecked_bkgnd;

            InitRibbonButton(btnDetectAnalyze, ID.ID_BTN_DETECT_ANALYZE, "화재탐지분석", global::SDMS.Properties.Resources.Pareto_Normal, global::SDMS.Properties.Resources.Pareto_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnDetectHistory, ID.ID_BTN_DETECT, "화재탐지이력", global::SDMS.Properties.Resources.FindHistory_Normal, global::SDMS.Properties.Resources.FindHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnProcessHistory, ID.ID_BTN_NOTOPERATION, "화재처리이력", global::SDMS.Properties.Resources.ProcessHistory_Normal, global::SDMS.Properties.Resources.ProcessHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnReactionHistory, ID.ID_BTN_ACTION, "화재대응이력", global::SDMS.Properties.Resources.ReactionHistory_Normal, global::SDMS.Properties.Resources.ReactionHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnSMSHistory, ID.ID_BTN_SMSREPORT, "화재문자이력", global::SDMS.Properties.Resources.Manage_SMS_Normal, global::SDMS.Properties.Resources.Manage_SMS_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);

            InitRibbonButton(btnDetectPSMAnalyze, ID.ID_BTN_DETECT_PSM_ANALYZE, "누출탐지분석", global::SDMS.Properties.Resources.Pareto_Normal, global::SDMS.Properties.Resources.Pareto_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnDetectPSMHistory, ID.ID_BTN_DETECT_PSM, "누출탐지이력", global::SDMS.Properties.Resources.FindHistory_Normal, global::SDMS.Properties.Resources.FindHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnNotOperationPSMHistory, ID.ID_BTN_NOTOPERATION_PSM, "누출처리이력", global::SDMS.Properties.Resources.ProcessHistory_Normal, global::SDMS.Properties.Resources.ProcessHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnActionPSMHistory, ID.ID_BTN_ACTION_PSM, "누출대응이력", global::SDMS.Properties.Resources.ReactionHistory_Normal, global::SDMS.Properties.Resources.ReactionHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnSMSPSMHistory, ID.ID_BTN_SMSREPORT_PSM, "누출문자이력", global::SDMS.Properties.Resources.Manage_SMS_Normal, global::SDMS.Properties.Resources.Manage_SMS_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);

            //방범
            InitRibbonButton(btnDetectIntrusionAnalyze, ID.ID_BTN_DETECT_INTRUSION_ANALYZE, "방범탐지분석", global::SDMS.Properties.Resources.Pareto_Normal, global::SDMS.Properties.Resources.Pareto_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnDetectIntrusionHistory, ID.ID_BTN_DETECT_INTRUSION, "방범탐지이력", global::SDMS.Properties.Resources.FindHistory_Normal, global::SDMS.Properties.Resources.FindHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnProcessIntrusionHistory, ID.ID_BTN_NOTOPERATION_INTRUSION, "방범처리이력", global::SDMS.Properties.Resources.ProcessHistory_Normal, global::SDMS.Properties.Resources.ProcessHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnReactionIntrusionHistory, ID.ID_BTN_ACTION_INTRUSION, "방범대응이력", global::SDMS.Properties.Resources.ReactionHistory_Normal, global::SDMS.Properties.Resources.ReactionHistory_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);
            InitRibbonButton(btnSMSIntrusionHistory, ID.ID_BTN_SMSREPORT_INTRUSION, "방범문자이력", global::SDMS.Properties.Resources.Manage_SMS_Normal, global::SDMS.Properties.Resources.Manage_SMS_Checked, imgMouseOverBkgnd, imgCheckedBkgnd, m_reportButtons);

            SetDefaultReportMenuButton();
            //btnDetectAnalyze.IsChecked = true;

            btnDetectHistory.Tag = ID.ID_BTN_DETECT;
            btnProcessHistory.Tag = ID.ID_BTN_NOTOPERATION;
            btnReactionHistory.Tag = ID.ID_BTN_ACTION;
            btnSMSHistory.Tag = ID.ID_BTN_SMSREPORT;
            btnDetectPSMHistory.Tag = ID.ID_BTN_DETECT_PSM;
            btnNotOperationPSMHistory.Tag = ID.ID_BTN_NOTOPERATION_PSM;
            btnActionPSMHistory.Tag = ID.ID_BTN_ACTION_PSM;
            btnSMSPSMHistory.Tag = ID.ID_BTN_SMSREPORT_PSM;
            btnDetectIntrusionHistory.Tag = ID.ID_BTN_DETECT_INTRUSION;
            btnProcessIntrusionHistory.Tag = ID.ID_BTN_NOTOPERATION_INTRUSION;
            btnReactionIntrusionHistory.Tag = ID.ID_BTN_ACTION_INTRUSION;
            btnSMSIntrusionHistory.Tag = ID.ID_BTN_SMSREPORT_INTRUSION;            
            
            if (UnE.SOP.ProxySOP.Instance.UsePSM == false)
            {
                imgReportSplit.Visible = false;
                btnDetectPSMAnalyze.Visible = false;
                btnDetectPSMHistory.Visible = false;
                btnNotOperationPSMHistory.Visible = false;
                btnActionPSMHistory.Visible = false;
                btnSMSPSMHistory.Visible = false;
            }

            if (UnE.SOP.ProxySOP.Instance.UseIntrusion == false)
            {
                imgReportSplit2.Visible = false;
                btnDetectIntrusionAnalyze.Visible = false;
                btnDetectIntrusionHistory.Visible = false;
                btnProcessIntrusionHistory.Visible = false;
                btnReactionIntrusionHistory.Visible = false;
                btnSMSIntrusionHistory.Visible = false;
            }
        }

        private void SetDefaultReportMenuButton()
        {
            SDMS.Data.ReportMode nPage = m_PageHome.FrmReport.ReportPage;

            switch (nPage)
            {
                case SDMS.Data.ReportMode.DetectFireAnalyze:
                    btnDetectAnalyze.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.DetectFire:
                    btnDetectHistory.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.ProcessFire:
                    btnProcessHistory.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.ActionFire:
                    btnReactionHistory.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.SMSFire:
                    btnSMSHistory.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.DetectPSMAnalyze:
                    btnDetectPSMAnalyze.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.DetectPSM:
                    btnDetectPSMHistory.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.ProcessPSM:
                    btnNotOperationPSMHistory.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.ActionPSM:
                    btnActionPSMHistory.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.SMSPSM:
                    btnSMSPSMHistory.IsChecked = true;
                    break;

                //방범
                case SDMS.Data.ReportMode.DetectIntrusionAnalyze:
                    btnDetectIntrusionAnalyze.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.DetectIntrusion:
                    btnDetectIntrusionHistory.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.ProcessIntrusion:
                    btnProcessIntrusionHistory.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.ActionIntrusion:
                    btnReactionIntrusionHistory.IsChecked = true;
                    break;

                case SDMS.Data.ReportMode.SMSIntrusion:
                    btnSMSIntrusionHistory.IsChecked = true;
                    break;
            }
        }

        private void InitAdminRibbonButtons()
        {
            Image imgMouseOverBkgnd = global::SDMS.Properties.Resources.RibbonMouseOver_bkgnd;
            Image imgCheckedBkgnd = global::SDMS.Properties.Resources.RibbonChecked_bkgnd;

            //btnCreateFire.Visible = btnCreateSpringCooler.Visible = btnEditFacilityZone.Visible = btnCreatePump.Visible = btnCreateCCTV.Visible = btnDelete.Visible = true;
            btnCreateFire.Visible = btnCreateSpringCooler.Visible = btnEditFacilityZone.Visible = btnCreatePump.Visible = btnCreateCCTV.Visible = btnDelete.Visible = false;
            btnBackupDB.Visible = false;
            //btnEditFacilityZone.Location = btnCreateFire.Location;
            sensorMgrBtn.Location = btnCreateFire.Location;
            //btnShowList.Location = btnCreateFire.Location;


            //InitRibbonButton(btnCreateFire, ID.ID_NEW_FIRE_SENSOR, "화재탐지", global::SDMS.Properties.Resources.Create_Fire_Normal, global::SDMS.Properties.Resources.Create_Fire_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            //InitRibbonButton(btnCreateSpringCooler, ID.ID_NEW_COOLER_SENSOR, "스프링쿨러", global::SDMS.Properties.Resources.Create_SpringCooler_Normal, global::SDMS.Properties.Resources.Create_SpringCooler_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            //InitRibbonButton(btnEditFacilityZone, ID.ID_EDIT_FACILITY_ZONE, "설비영역", global::SDMS.Properties.Resources.EditFacilityZone_Normal, global::SDMS.Properties.Resources.EditFacilityZone_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            //InitRibbonButton(btnCreatePump, ID.ID_NEW_PRESSURE_SENSOR, "펌프압력", global::SDMS.Properties.Resources.Create_Pump_Normal, global::SDMS.Properties.Resources.Create_Pump_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            //InitRibbonButton(btnCreateCCTV, ID.ID_NEW_CCTV, "CCTV", global::SDMS.Properties.Resources.Create_CCTV_Normal, global::SDMS.Properties.Resources.Create_CCTV_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            //InitRibbonButton(btnDelete, ID.ID_DEL_FACILITY, "삭제", global::SDMS.Properties.Resources.Del_Normal, global::SDMS.Properties.Resources.Del_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(sensorMgrBtn, ID.ID_MANAGE_SENSOR, "센서동작관리", global::SDMS.Properties.Resources.Manage_Sensor_Normal, global::SDMS.Properties.Resources.Manage_Sensor_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnShowList, ID.ID_SHOW_LIST_FACILITY, "설비목록보기", global::SDMS.Properties.Resources.Show_List_Normal, global::SDMS.Properties.Resources.Show_List_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnManageManager, ID.ID_MANAGE_MANAGER, "담당자관리", global::SDMS.Properties.Resources.Manage_Manager_Normal, global::SDMS.Properties.Resources.Manage_Manager_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnManageSMS, ID.ID_MANAGE_MESSAGE, "메시지관리", global::SDMS.Properties.Resources.Manage_SMS_Normal, global::SDMS.Properties.Resources.Manage_SMS_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnManageBroadcast, ID.ID_MANAGE_BROADCAST, "방송관리", global::SDMS.Properties.Resources.Manage_Broadcast_Normal, global::SDMS.Properties.Resources.Manage_Broadcast_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            //InitRibbonButton(btnManagePrint, ID.ID_MANAGE_PRINT, "도면관리", global::SDMS.Properties.Resources.Manage_Print_Normal, global::SDMS.Properties.Resources.Manage_Print_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            //InitRibbonButton(btnManageFacility, ID.ID_MANAGE_FACILITY, "장비현황", global::SDMS.Properties.Resources.Manage_Facility_Normal, global::SDMS.Properties.Resources.Manage_Facility_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnManageDetect, ID.ID_MANAGE_DETECT, "탐지관리", global::SDMS.Properties.Resources.Manage_Find_Normal, global::SDMS.Properties.Resources.Manage_Find_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            //InitRibbonButton(btnBackupDB, ID.ID_MANAGE_BACKUPDB, "백업/복원", global::SDMS.Properties.Resources.Backup_Restore, global::SDMS.Properties.Resources.Backup_restore_checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitRibbonButton(btnSave, ID.ID_SAVE_DATA, "저장", global::SDMS.Properties.Resources.Save_Normal, global::SDMS.Properties.Resources.Save_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);

            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
                InitRibbonButton(btnEarthquake, ID.ID_MANAGE_EARTHQUAKE, "지진관리", global::SDMS.Properties.Resources.EarthquakeOption_Normal, global::SDMS.Properties.Resources.EarthquakeOption_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);

            ArrangeAdminRibbonButtons();
            ArrangeReportRibbonButtons();

            btnSave.Enabled = false;
        }

        private void InitRibbonButton(RibbonButton btn, int nID, string strTitle, Image imgNormal, Image imgChecked, Image imgMouseOverBkgnd, Image imgCheckedBkgnd, List<RibbonButton> buttons = null)
        {
            btn.NormalImage = imgNormal;
            btn.CheckedImage = imgChecked;
            btn.MouseOverBkgndImage = imgMouseOverBkgnd;
            btn.CheckedBkgndImage = imgCheckedBkgnd;
            btn.Title = strTitle;
            btn.Owner = this;
            btn.Tag = nID;

            SetButtonID(btn, nID);

            if (buttons != null)
                buttons.Add(btn);
        }

        private void ArrangeAdminRibbonButtons()
        {
            //ArrangeRibbonButton(btnCreateFire, btnCreateSpringCooler);
            //ArrangeRibbonButton(btnCreateSpringCooler, btnEditFacilityZone);
            //ArrangeRibbonButton(btnEditFacilityZone, btnCreatePump);
            //ArrangeRibbonButton(btnCreatePump, btnCreateCCTV);

            //ArrangeRibbonButton(btnCreateCCTV, pictureBoxAdminRibbon1, (RibbonButton)btnDelete);
            //ArrangeRibbonButton(btnDelete, pictureBoxAdminRibbon2, btnShowList);
            pictureBoxAdminRibbon1.Visible = pictureBoxAdminRibbon2.Visible = false;
            ArrangeRibbonButton(sensorMgrBtn, pictureBoxAdminRibbon3, btnShowList);
            ArrangeRibbonButton(btnShowList, btnManageManager);
            ArrangeRibbonButton(btnManageManager, btnManageSMS);            
            ArrangeRibbonButton(btnManageSMS, btnManageBroadcast);
            ArrangeRibbonButton(btnManageBroadcast, btnManageDetect);
            //ArrangeRibbonButton(btnManagePrint, btnManageFacility);
            //ArrangeRibbonButton(btnManageFacility, btnManageDetect);
            //ArrangeRibbonButton(btnManageDetect, btnBackupDB);

            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
            {
                PictureBox pictureBoxAdminRibbon5 = new PictureBox();

                pictureBoxAdminRibbon5.BackColor = pictureBoxAdminRibbon4.BackColor;
                pictureBoxAdminRibbon5.BackgroundImage = pictureBoxAdminRibbon4.BackgroundImage;
                pictureBoxAdminRibbon5.BackgroundImageLayout = pictureBoxAdminRibbon4.BackgroundImageLayout;

                pictureBoxAdminRibbon4.Parent.Controls.Add(pictureBoxAdminRibbon5);
                pictureBoxAdminRibbon5.Size = pictureBoxAdminRibbon4.Size;
                pictureBoxAdminRibbon5.Location = pictureBoxAdminRibbon4.Location;

                ArrangeRibbonButton(btnManageDetect, pictureBoxAdminRibbon4, btnEarthquake);
                ArrangeRibbonButton(btnEarthquake, pictureBoxAdminRibbon5, btnSave);
                pictureBoxAdminRibbon5.Show();
            }
            else
            {
                btnEarthquake.Visible = false;
                ArrangeRibbonButton(btnManageDetect, pictureBoxAdminRibbon4, btnSave);
            }

            //ArrangeRibbonButton(btnBackupDB, pictureBoxAdminRibbon4, btnSave);
        }

        private void ArrangeReportRibbonButtons()
        {
            ArrangeRibbonButton(btnDetectAnalyze, btnDetectHistory);
            ArrangeRibbonButton(btnDetectHistory, btnProcessHistory);
            ArrangeRibbonButton(btnProcessHistory, btnReactionHistory);
            ArrangeRibbonButton(btnReactionHistory, btnSMSHistory);

            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                ArrangeRibbonButton(btnSMSHistory, imgReportSplit, btnDetectPSMAnalyze);
                ArrangeRibbonButton(btnDetectPSMAnalyze, btnDetectPSMHistory);
                ArrangeRibbonButton(btnDetectPSMHistory, btnNotOperationPSMHistory);
                ArrangeRibbonButton(btnNotOperationPSMHistory, btnActionPSMHistory);
                ArrangeRibbonButton(btnActionPSMHistory, btnSMSPSMHistory);
            }

            if (UnE.SOP.ProxySOP.Instance.UsePSM && UnE.SOP.ProxySOP.Instance.UseIntrusion) 
                ArrangeRibbonButton(btnSMSPSMHistory, imgReportSplit2, btnDetectIntrusionAnalyze); 
            else if (!UnE.SOP.ProxySOP.Instance.UsePSM && UnE.SOP.ProxySOP.Instance.UseIntrusion)
                ArrangeRibbonButton(btnSMSHistory, imgReportSplit2, btnDetectIntrusionAnalyze);

            if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                ArrangeRibbonButton(btnDetectIntrusionAnalyze, btnDetectIntrusionHistory);
                ArrangeRibbonButton(btnDetectIntrusionHistory, btnProcessIntrusionHistory);
                ArrangeRibbonButton(btnProcessIntrusionHistory, btnReactionIntrusionHistory);
                ArrangeRibbonButton(btnReactionIntrusionHistory, btnSMSIntrusionHistory);
            }
        }

        private void ArrangeRibbonButton(RibbonButton btnPrev, RibbonButton btnNext)
        {
            btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width, btnPrev.Location.Y);
        }

        private void ArrangeRibbonButton(RibbonButton btnPrev, PictureBox pictureBoxMiddle, RibbonButton btnNext)
        {
            pictureBoxMiddle.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width - 3, pictureBoxMiddle.Location.Y);
            btnNext.Location = new Point(pictureBoxMiddle.Location.X + pictureBoxMiddle.Size.Width - 3, btnPrev.Location.Y);
        }

        private void SetButtonID(Button btn, int nID, string strTooltipText = "")
        {
            if (btn == null)
                return;

            m_dicButtonIDs[btn] = nID;
            m_dicIDButtons[nID] = btn;
            m_dicButtonChecked[btn] = false;
            btn.Tag = nID;

            if (strTooltipText.Length > 0)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(btn, strTooltipText);
            }
        }

        public Button GetButton(int nID)
        {
            if (m_dicIDButtons.ContainsKey(nID))
                return m_dicIDButtons[nID];

            return null;
        }

        public int GetButtonID(Button btn)
        {
            if (m_dicButtonIDs.ContainsKey(btn))
                return m_dicButtonIDs[btn];

            return -1;
        }

        public bool IsChecked(int nButtonID)
        {
            if (!m_dicIDButtons.ContainsKey(nButtonID))
                return false;

            Button btn = m_dicIDButtons[nButtonID];
            return IsChecked(btn);
        }

        public bool IsChecked(Button btn)
        {
            if (m_dicButtonChecked.ContainsKey(btn))
                return m_dicButtonChecked[btn];

            return false;
        }

        public void CheckButton(int nButtonID, bool isChecked)
        {
            if (!m_dicIDButtons.ContainsKey(nButtonID))
                return;

            Button btn = m_dicIDButtons[nButtonID];
            CheckButton(btn, isChecked);
            m_toolbar.CheckButton(nButtonID, isChecked);
        }

        public void CheckButton(Button btn, bool isChecked)
        {
            if (btn == null)
                return;

            if (!m_dicButtonChecked.ContainsKey(btn))
                return;

            bool checkedOld = m_dicButtonChecked[btn];
            m_dicButtonChecked[btn] = isChecked;

            if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_HOME)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Home_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Home_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_FULLSCREEN)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.FullScreen_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.FullScreen_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_PICK)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Pick_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Pick_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_PAN)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Panning_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Panning_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_ORBIT)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Orbit_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Orbit_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_ZOOMIN)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ZoomIn_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ZoomIn_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_ZOOMOUT)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ZoomOut_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ZoomOut_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_OUTSIDE)
            {
                if (isChecked)
                {
                    btn.BackColor = System.Drawing.Color.FromArgb(62, 81, 97);
                    btn.BackgroundImage = global::SDMS.Properties.Resources._3d_checked;
                }
                else
                {
                    btn.BackColor = System.Drawing.Color.Transparent;
                    btn.BackgroundImage = global::SDMS.Properties.Resources._3d_normal;
                }
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_BOTHSIDE)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Both_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Both_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_INSIDE)
            {
                if (isChecked)
                {
                    btn.BackColor = System.Drawing.Color.FromArgb(62, 81, 97);
                    btn.BackgroundImage = global::SDMS.Properties.Resources._2d_checked;
                }
                else
                {
                    btn.BackColor = System.Drawing.Color.Transparent;
                    btn.BackgroundImage = global::SDMS.Properties.Resources._2d_normal;
                }
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_CCTV)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.CCTV_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.CCTV_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_SCREENSHOT)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ScreenShot_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.ScreenShot_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_WEATHER_INFO)
            {
                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Weather_checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Weather_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_DETECTOR)
            {
                WriteLayerState(LAYER_TYPE.FIRE_DETECT, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_Fire_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_Fire_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_COOLER)
            {
                WriteLayerState(LAYER_TYPE.SPRING_COOLER, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_SpringCooler_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_SpringCooler_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_PERSURE)
            {
                WriteLayerState(LAYER_TYPE.PUMP, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_Pump_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_Pump_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_CCTV)
            {
                WriteLayerState(LAYER_TYPE.CCTV, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_FIREEXT)
            {
                WriteLayerState(LAYER_TYPE.FE, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FE_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FE_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_FIREHYD)
            {
                WriteLayerState(LAYER_TYPE.HD, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_HD_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_HD_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_ALARMSTA)
            {
                WriteLayerState(LAYER_TYPE.FA, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FA_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FA_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_RECIVER)
            {
                WriteLayerState(LAYER_TYPE.FR, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FR_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_FR_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_CCTVLOW)
            {
                WriteLayerState(LAYER_TYPE.CCTV_L, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_CCTV_DISCONNECTED)
            {
                WriteLayerState(LAYER_TYPE.CCTV_DISCONNECTED, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_CCTV_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_BUILDING_TEXT)
            {
                WriteLayerState(LAYER_TYPE.BUILDING_TEXT, isChecked);

                if (isChecked)
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_Building_Checked;
                else
                    btn.BackgroundImage = global::SDMS.Properties.Resources.Layer_Building_Normal;
            } 
            else if (btn.GetType() == typeof(RibbonButton))
            {
                RibbonButton ribbonButton = (RibbonButton)btn;
                ribbonButton.IsChecked = isChecked;
            }

            if (checkedOld != isChecked)
                btn.Refresh();
        }

        private void WriteLayerState(LAYER_TYPE type, bool isChecked)
        {
            int nLayerState = ReadLayerState();

            if (isChecked)
                nLayerState = nLayerState | (int)type;
            else
                nLayerState = nLayerState & (~(int)type);

            WriteLayerState(nLayerState);
        }

        private string StatusLableText
        {
            set
            {
                mLabelStatus.Text = value;
                mLabelStatus.Refresh();
            }
        }

        public static void SetInfoMessage(string szMessage)
        {
            ((RealTimeInfoPane)m_instance.panelLog).RealTimeInfo = szMessage;
            ((RealTimeInfoPane)m_instance.panelLog).DrawMovingText();
        }

        private void ResizeButtons()
        {
            int nClockRight = panelClock.Location.X + panelClock.Size.Width;
            int nPanelSpace = panelStatus.Location.X - nClockRight - btnDefaultCCTV.Width;

            int nStatusRight = panelStatus.Location.X + panelStatus.Size.Width;
            panelLog.Location = new Point(nStatusRight + nPanelSpace, panelLog.Location.Y);

            btnFire.Location = new Point(panelTop.Size.Width - btnFire.Size.Width, btnFire.Location.Y);
            panelLog.Size = new Size(btnFire.Location.X - nPanelSpace - panelLog.Location.X, panelLog.Size.Height);
        }

        public Point SelectCaseDlgLocation()
        {
            int x = cmbFireDetect.Location.X + cmbFireDetect.Size.Width + 10;
            int y = panelLog.Location.Y + panelLog.Size.Height + cmbFireDetect.Location.Y;
            Point pt = new Point(x, y);
            //Point pt = new Point(panelLog.Location.X, panelLog.Location.Y - 73);
            return pt;
        }



        private void ResizeSystemButtons()
        {
            int nWidth = this.Size.Width;
            int nImageWidth = btnMax.Width;

            btnClose.Location = new Point(nWidth - nImageWidth, btnClose.Location.Y);
            btnMax.Location = new Point(btnClose.Location.X - m_nSystemButtonSpace - nImageWidth, btnMax.Location.Y);
            btnMin.Location = new Point(btnMax.Location.X - m_nSystemButtonSpace - nImageWidth, btnMin.Location.Y);
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            MainFrame.WindowState = FormWindowState.Minimized;
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            if (MainFrame.WindowState == FormWindowState.Normal)
            {
                MainFrame.WindowState = FormWindowState.Maximized;
                btnMax.BackgroundImage = global::SDMS.Properties.Resources.NormalWindow_Normal;
            }
            else if (MainFrame.WindowState == FormWindowState.Maximized)
            {
                MainFrame.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = global::SDMS.Properties.Resources.MaxWindow_Normal;
                MainFrame.Refresh();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            MainFrame.Visible = false;
            MainFrame.Close();
        }

        #region Top패널 Mouse 이벤트 , Maximized, Minimized, Move

        private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.Y > 40)
                    return;

                m_bLeftMouseDown = true;
                m_ptMove = panelTop.PointToScreen(new Point(e.X, e.Y));
            }
            FormMain.Instance.EnableFireReportBtn(false);
        }

        private void panelTop_MouseUp(object sender, MouseEventArgs e)
        {
            FormMain.Instance.EnableFireReportBtn(false);
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                  

                    FormFrame.Instance.ToNormalWindow();

                    Point pt = panelTop.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = MainFrame.Location;
                        MainFrame.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void panelTop_DoubleClick(object sender, EventArgs e)
        {
            Point pt = Cursor.Position;
            Point ptLoc = panelTop.PointToClient(pt);
            if (ptLoc.Y > 40)
                return;


            if (MainFrame.WindowState == FormWindowState.Normal)
            {
                MainFrame.WindowState = FormWindowState.Maximized;
                btnMax.BackgroundImage = global::SDMS.Properties.Resources.NormalWindow_Normal;
            }
            else if (MainFrame.WindowState == FormWindowState.Maximized)
            {
                Size sizeCur = MainFrame.Size;
                MainFrame.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = global::SDMS.Properties.Resources.MaxWindow_Normal;
                //Size sizeNormal = MainFrame.Size;

                //double hRate = (double)sizeNormal.Height / (double)sizeCur.Height;
                //MainFrame.Size = new Size((int)(sizeCur.Width * hRate), sizeNormal.Height);
            }
        }

        #endregion Top패널 Mouse 이벤트 , Maximized, Minimized, Move

        private void CreateBackstageHome()
        {
            m_PageHome = new PageBackstageHome();
            m_PageHome.Location = new Point(0, 0);
            m_PageHome.Dock = DockStyle.Fill;
            m_PageHome.TopLevel = false;
            m_PageHome.Parent = panelBottom;

            panelBottom.Controls.Add(m_PageHome);
        }




        public void TextPictureBox_MouseDown(TextPictureBox pictureBox, MouseEventArgs e)
        {
            if (e != null)
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left)
                    return;
            }

            if (pictureBox == pictureBoxMonitoring)
            {
                if (mCurrentTab != UnE.View.Content.ContentOwnerTab.M3D_TAB)
                {
                    m_PageHome.ContentForm.ClearTabState();
                    SelectMonitoringTab();

                }
            }
            else if (pictureBox == pictureBoxAdmin)
            {
                if (mCurrentTab != UnE.View.Content.ContentOwnerTab.ADMIN_TAB)
                {
                    m_PageHome.ContentForm.ClearTabState();
                    SelectAdminTab();
                    mCurrentTab = UnE.View.Content.ContentOwnerTab.ADMIN_TAB;
                }
            }
            else if (pictureBox == pictureBoxReport)
            {
                if (mCurrentTab != UnE.View.Content.ContentOwnerTab.REPORT_TAB)
                {
                    m_PageHome.ContentForm.ClearTabState();
                    SelectReportTab();
                    mCurrentTab = UnE.View.Content.ContentOwnerTab.REPORT_TAB;
                }
            }
            else if (pictureBox == pictureBox2D)
            {
                if (mCurrentTab != UnE.View.Content.ContentOwnerTab.M2D_TAB)
                {
                    m_PageHome.ContentForm.ClearTabState();
                    Select2DTab();
                    mCurrentTab = UnE.View.Content.ContentOwnerTab.M2D_TAB;
                }
            }
            else if (pictureBox == pictureBoxCCTV)
            {
                if (mCurrentTab != UnE.View.Content.ContentOwnerTab.CCTV_TAB)
                {
                    SelectCCTVTab();
                    mCurrentTab = UnE.View.Content.ContentOwnerTab.CCTV_TAB;
                }
            }
        }

        private UnE.View.Content.ContentOwnerTab mCurrentTab = UnE.View.Content.ContentOwnerTab.M3D_TAB;
        public UnE.View.Content.ContentOwnerTab CurrentTab
        {
            get { return mCurrentTab; }
        }

        public void ChangeTab(UnE.View.Content.ContentOwnerTab tab)
        {
            if (tab == mCurrentTab)
                return;

            mCurrentTab = tab;

            if (tab == UnE.View.Content.ContentOwnerTab.M3D_TAB)
            {
                SelectMonitoringTab();
            }
            else if (tab == UnE.View.Content.ContentOwnerTab.ADMIN_TAB)
            {
                SelectAdminTab();
            }
            else if (tab == UnE.View.Content.ContentOwnerTab.REPORT_TAB)
            {
                SelectReportTab();
            }
            else if (tab == UnE.View.Content.ContentOwnerTab.M2D_TAB)
            {
                int i = 0;
                i++;
                Select2DTab();
            }
            else if (tab == UnE.View.Content.ContentOwnerTab.CCTV_TAB)
            {
                SelectCCTVTab();
            }
        }

        private object m_LockObject = new object();

        public void SetFloorStatus(BuildingGroup grp, Building building, Zone zoneFloor)
        {
            //FormMain.Instance.EnableFireReportBtn(false);
            lock (m_LockObject)
            {
                m_PageHome.ChangeFloor(grp, building, zoneFloor);
            }
        }

        #region (ChangeTab)

        /////////////////////////////////////////////////////////////////////////////////

        public bool Select2DTab()
        {
            if(m_bUse2D == false)
                return true;

            FormMain.Instance.PageHome.ContentForm.SaveCurrentTabLayout();

            // 직전 모드가 Report 탭이었다면...
            if (panelReportRibbonBarLeft.Visible)
            {
                m_dtLastReport = DateTime.Now;
                if (SensorHistoryManager.Instance.LastSensorZoneHistoryID != -1)
                    m_nReadHistoryID = SensorHistoryManager.Instance.LastSensorZoneHistoryID;
            }

            if (this.HiddenClock)
            {
                btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = btnDefaultCCTV.Visible = true;
            }
            else
                panelClock.Visible = true;

            panelStatus.Visible = true;
            panelLog.Visible = true;
            btnFire.Visible = true;
            panelMiddle.Visible = true;
            panelLeft.Visible = true;

            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;


            panelAdminRibbonBarLeft.Visible = false;
            panelAdminRibbonBarMiddle.Visible = false;
            panelAdminRibbonBarRight.Visible = false;

            panelReportRibbonBarLeft.Visible = false;
            panelReportRibbonBarMiddle.Visible = false;
            panelReportRibbonBarRight.Visible = false;

            labelFireDetect.Visible = true;
            cmbFireDetect.Visible = true;

            m_PageHome.CloseExternal();
            m_toolbar.Mode = 1;
            ShowToolbar();

            pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Pressed;
            pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxMonitoring.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxAdmin.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxReport.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;

            OnSelect2DTab();
            //m_PageHome.OnClick2D();
            mCurrentTab = UnE.View.Content.ContentOwnerTab.M2D_TAB;
            m_PageHome.ContentForm.LoadTabLayout((int)mCurrentTab);
            
            return true;
        }



        public bool SelectCCTVTab(bool bLoadCCTV = true)
        {
            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                return true;

            FormMain.Instance.PageHome.ContentForm.SaveCurrentTabLayout();

            // 직전 모드가 Report 탭이었다면...
            if (panelReportRibbonBarLeft.Visible)
            {
                m_dtLastReport = DateTime.Now;
                if (SensorHistoryManager.Instance.LastSensorZoneHistoryID != -1)
                    m_nReadHistoryID = SensorHistoryManager.Instance.LastSensorZoneHistoryID;
            }

            if (this.HiddenClock)
            {
                btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = btnDefaultCCTV.Visible = true;
            }
            else
                panelClock.Visible = true;

            panelStatus.Visible = true;
            panelLog.Visible = true;
            btnFire.Visible = true;
            panelMiddle.Visible = true;

            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;


            panelAdminRibbonBarLeft.Visible = false;
            panelAdminRibbonBarMiddle.Visible = false;
            panelAdminRibbonBarRight.Visible = false;

            panelReportRibbonBarLeft.Visible = false;
            panelReportRibbonBarMiddle.Visible = false;
            panelReportRibbonBarRight.Visible = false;

            labelFireDetect.Visible = true;
            cmbFireDetect.Visible = true;

            //if (!ShowEquipZoneCCTV)
            //    ShowToolbar();
            m_toolbar.Mode = 1;
            HideToolbar();

            pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Pressed;
            pictureBoxMonitoring.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxAdmin.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxReport.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;

            OnSelectCCTVTab();

            if (bLoadCCTV == true)
                m_PageHome.OnClickBigCCTV();

            mCurrentTab = UnE.View.Content.ContentOwnerTab.CCTV_TAB;

            return true;
        }


        public bool SelectMonitoringTab()
        {
            m_PageHome.ContentForm.Visible = true;

            FormMain.Instance.PageHome.ContentForm.SaveCurrentTabLayout();

            // 직전 모드가 Report 탭이었다면...
            if (panelReportRibbonBarLeft.Visible)
            {
                m_dtLastReport = DateTime.Now;
                if (SensorHistoryManager.Instance.LastSensorZoneHistoryID != -1)
                    m_nReadHistoryID = SensorHistoryManager.Instance.LastSensorZoneHistoryID;
            }

            if (this.HiddenClock)
            {
                btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = btnDefaultCCTV.Visible = true;
            }
            else
                panelClock.Visible = true;

            btnSensorMonitor.Visible = labelSensorMonitor.Visible = btnSendMessage.Visible = true;

            panelStatus.Visible = true;
            panelLog.Visible = true;
            btnFire.Visible = true;
            panelMiddle.Visible = true;
            panelLeft.Visible = true;

            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;


            panelAdminRibbonBarLeft.Visible = false;
            panelAdminRibbonBarMiddle.Visible = false;
            panelAdminRibbonBarRight.Visible = false;

            panelReportRibbonBarLeft.Visible = false;
            panelReportRibbonBarMiddle.Visible = false;
            panelReportRibbonBarRight.Visible = false;

            labelFireDetect.Visible = true;
            cmbFireDetect.Visible = true;

            m_PageHome.CloseExternal();

            m_toolbar.Mode = 1;
            ShowToolbar();

            pictureBoxMonitoring.BackgroundImage = global::SDMS.Properties.Resources.Tab_Pressed;
            pictureBoxAdmin.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxReport.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            OnSelectMonitoringTab();

            //m_PageHome.OnClick3D();

            mCurrentTab = UnE.View.Content.ContentOwnerTab.M3D_TAB;
            m_PageHome.ContentForm.LoadTabLayout((int)mCurrentTab);

            return true;
        }

        public bool SelectAdminTab()
        {
            m_PageHome.ContentForm.Visible = true;

            FormMain.Instance.PageHome.ContentForm.SaveCurrentTabLayout();
            // 직전 모드가 Report 탭이었다면...
            if (panelReportRibbonBarLeft.Visible)
            {
                m_dtLastReport = DateTime.Now;
                if (SensorHistoryManager.Instance.LastSensorZoneHistoryID != -1)
                    m_nReadHistoryID = SensorHistoryManager.Instance.LastSensorZoneHistoryID;
            }

            btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = btnDefaultCCTV.Visible = false;
            btnSensorMonitor.Visible = labelSensorMonitor.Visible = btnSendMessage.Visible = false;
            panelClock.Visible = false;
            panelStatus.Visible = false;
            panelLog.Visible = false;
            btnFire.Visible = false;
            panelMiddle.Visible = true;
            panelLeft.Visible = true;

            //알람 창
            m_PanelBtnNotice.Visible = false;
            btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Normal;

            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;

            panelAdminRibbonBarLeft.Visible = true;
            panelAdminRibbonBarMiddle.Visible = true;
            panelAdminRibbonBarRight.Visible = true;

            panelReportRibbonBarLeft.Visible = false;
            panelReportRibbonBarMiddle.Visible = false;
            panelReportRibbonBarRight.Visible = false;

            labelFireDetect.Visible = false;
            cmbFireDetect.Visible = false;

            m_PageHome.CloseExternal();

            m_toolbar.Mode = 2;
            ShowToolbar();


            pictureBoxMonitoring.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxAdmin.BackgroundImage = global::SDMS.Properties.Resources.Tab_Pressed;
            pictureBoxReport.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            
            OnSelectAdminTab();

            mCurrentTab = UnE.View.Content.ContentOwnerTab.ADMIN_TAB;
            m_PageHome.ContentForm.LoadTabLayout((int)mCurrentTab);

            return true;
        }

        public bool SelectReportTab()
        {
            //m_dtLastReport = DateTime.Now;
            if (SensorHistoryManager.Instance.LastSensorZoneHistoryID != -1)
                m_nReadHistoryID = SensorHistoryManager.Instance.LastSensorZoneHistoryID;

            btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = btnDefaultCCTV.Visible = false;
            btnSensorMonitor.Visible = labelSensorMonitor.Visible = btnSendMessage.Visible = labelFireDetect.Visible = cmbFireDetect.Visible = false;
            panelMiddle.Refresh();

            panelClock.Visible = false;
            panelStatus.Visible = false;
            panelLog.Visible = false;
            btnFire.Visible = false;
            panelMiddle.Visible = false;
            panelLeft.Visible = false;

            //알람창
            m_PanelBtnNotice.Visible = false;
            btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Normal;

            checkedReportRibbonbar();


            panelReportRibbonBarLeft.Visible = true;
            panelReportRibbonBarMiddle.Visible = true;
            panelReportRibbonBarRight.Visible = true;

            panelAdminRibbonBarLeft.Visible = false;
            panelAdminRibbonBarMiddle.Visible = false;
            panelAdminRibbonBarRight.Visible = false;


            labelFireDetect.Visible = false;
            cmbFireDetect.Visible = false;

            m_PageHome.CloseExternal();

            pictureBoxMonitoring.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxAdmin.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxReport.BackgroundImage = global::SDMS.Properties.Resources.Tab_Pressed;
            pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;


            m_toolbar.Mode = 1;
            HideToolbar();

            OnSelectReportTab();
            mCurrentTab = UnE.View.Content.ContentOwnerTab.REPORT_TAB;
            //m_PageHome.ContentForm.LoadTabLayout((int)mCurrentTab);

            // Unity 화면의 간섭을 없애기 위하여 안보이도록 한다.
            m_PageHome.ContentForm.Visible = false;

            return true;
        }

        private void checkedReportRibbonbar()
        {
            SDMS.Data.ReportMode nPage = m_PageHome.FrmReport.ReportPage;

            switch (nPage)
            {
                case SDMS.Data.ReportMode.DetectFireAnalyze:
                case SDMS.Data.ReportMode.DetectFire:
                case SDMS.Data.ReportMode.ProcessFire:
                case SDMS.Data.ReportMode.SMSFire:

                    panelProcessHistory.Visible = true;
                    panelReactionHistory.Visible = false;
                    pnDetectPSM.Visible = false;
                    pnNotOperationPSM.Visible = false;
                    pnActionPSM.Visible = false;
                    pnSMSPSM.Visible = false;


                    if (nPage == SDMS.Data.ReportMode.DetectFire)
                    {
                        btnDateFormat.Visible =
                        lblSplitUnit.Visible =
                        lblSplitUnitDetail.Visible =
                        lblViewCount.Visible =
                        proc_cboSplitUnit.Visible =
                        nudSplitUnitDetail.Visible =
                        proc_cboViewCount.Visible = true;
                    }
                    else
                    {
                        btnDateFormat.Visible =
                        lblSplitUnit.Visible =
                        lblSplitUnitDetail.Visible =
                        lblViewCount.Visible =
                        proc_cboSplitUnit.Visible =
                        nudSplitUnitDetail.Visible =
                        proc_cboViewCount.Visible = false;
                    }

                    if (nPage == SDMS.Data.ReportMode.DetectFireAnalyze)
                    {
                        lblViewCount.Visible =
                        proc_cboViewCount.Visible = true;
                        labelDetectDateFormat.Visible = false;
                    }

                    break;

                case SDMS.Data.ReportMode.ActionFire:

                    panelProcessHistory.Visible = false;
                    panelReactionHistory.Visible = true;
                    pnDetectPSM.Visible = false;
                    pnNotOperationPSM.Visible = false;
                    pnActionPSM.Visible = false;
                    pnSMSPSM.Visible = false;


                    break;

                case SDMS.Data.ReportMode.DetectPSMAnalyze:
                    panelProcessHistory.Visible = false;
                    panelReactionHistory.Visible = false;
                    pnDetectPSM.Visible = true;
                    pnNotOperationPSM.Visible = false;
                    pnActionPSM.Visible = false;
                    pnSMSPSM.Visible = false;
                    break;

                case SDMS.Data.ReportMode.DetectPSM:
                    
                    panelProcessHistory.Visible = false;
                    panelReactionHistory.Visible = false;
                    pnDetectPSM.Visible = true;
                    pnNotOperationPSM.Visible = false;
                    pnActionPSM.Visible = false;
                    pnSMSPSM.Visible = false;

                    break;

                case SDMS.Data.ReportMode.ProcessPSM:
                    
                    panelProcessHistory.Visible = false;
                    panelReactionHistory.Visible = false;
                    pnDetectPSM.Visible = false;
                    pnNotOperationPSM.Visible = true;
                    pnActionPSM.Visible = false;
                    pnSMSPSM.Visible = false;

                    break;

                case SDMS.Data.ReportMode.ActionPSM:

                    panelProcessHistory.Visible = false;
                    panelReactionHistory.Visible = false;
                    pnDetectPSM.Visible = false;
                    pnNotOperationPSM.Visible = false;
                    pnActionPSM.Visible = true;
                    pnSMSPSM.Visible = false;

                    break;

                case SDMS.Data.ReportMode.SMSPSM:

                    panelProcessHistory.Visible = false;
                    panelReactionHistory.Visible = false;
                    pnDetectPSM.Visible = false;
                    pnNotOperationPSM.Visible = false;
                    pnActionPSM.Visible = false;
                    pnSMSPSM.Visible = true;

                    break;

                case SDMS.Data.ReportMode.DetectIntrusionAnalyze:
                case SDMS.Data.ReportMode.DetectIntrusion:
                case SDMS.Data.ReportMode.ProcessIntrusion:
                case SDMS.Data.ReportMode.SMSIntrusion:

                    panelProcessHistory.Visible = true;
                    panelReactionHistory.Visible = false;
                    pnDetectPSM.Visible = false;
                    pnNotOperationPSM.Visible = false;
                    pnActionPSM.Visible = false;
                    pnSMSPSM.Visible = false;


                    if (nPage == SDMS.Data.ReportMode.DetectIntrusion)
                    {
                        btnDateFormat.Visible =
                        lblSplitUnit.Visible =
                        lblSplitUnitDetail.Visible =
                        lblViewCount.Visible =
                        proc_cboSplitUnit.Visible =
                        nudSplitUnitDetail.Visible =
                        proc_cboViewCount.Visible = true;
                    }
                    else
                    {
                        btnDateFormat.Visible =
                        lblSplitUnit.Visible =
                        lblSplitUnitDetail.Visible =
                        lblViewCount.Visible =
                        proc_cboSplitUnit.Visible =
                        nudSplitUnitDetail.Visible =
                        proc_cboViewCount.Visible = false;
                    }

                    if (nPage == SDMS.Data.ReportMode.DetectIntrusionAnalyze)
                    {
                        lblViewCount.Visible =
                        proc_cboViewCount.Visible = true;
                        labelDetectDateFormat.Visible = false;
                    }

                    break;

                case SDMS.Data.ReportMode.ActionIntrusion:

                    panelProcessHistory.Visible = false;
                    panelReactionHistory.Visible = true;
                    pnDetectPSM.Visible = false;
                    pnNotOperationPSM.Visible = false;
                    pnActionPSM.Visible = false;
                    pnSMSPSM.Visible = false;


                    break;
            }

        }

        private void OnSelectMonitoringTab()
        {
            HideDateTimePicker();
            FormMain.Instance.EnableFireReportBtn(false);
            PageHome.ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);
        }

        private void OnSelectAdminTab()
        {
            HideDateTimePicker();
            FormMain.Instance.EnableFireReportBtn(false);
            PageHome.ChangeTab(UnE.View.Content.ContentOwnerTab.ADMIN_TAB);

            //this.Close();
            //Application.Restart();
        }

        private void OnSelectCCTVTab()
        {
            HideDateTimePicker();
            FormMain.Instance.EnableFireReportBtn(false);
            PageHome.ChangeTab(UnE.View.Content.ContentOwnerTab.CCTV_TAB);
        }

        private void OnSelect2DTab()
        {
            HideDateTimePicker();
            FormMain.Instance.EnableFireReportBtn(false);

            PageHome.ChangeTab(UnE.View.Content.ContentOwnerTab.M2D_TAB);
        }
        private void OnSelectBothTab()
        {
            HideDateTimePicker();
            FormMain.Instance.EnableFireReportBtn(false);

            PageHome.ChangeTab(UnE.View.Content.ContentOwnerTab.BOTH);
        }



        private int m_nLastHistoryID = -1;
        private void OnSelectReportTab()
        {
            HideDateTimePicker();

            proc_cboLatelyDate_SelectedIndexChanged(null, null);

            if (m_isFirstReport)
            {
                m_isFirstReport = false;
            }
            else
            {
                DateTime dtNow = DateTime.Now;
                bool bReload = NeedRefreshReport(ref dtNow);

                /*bool bReload = false;
				// 마지막에 리포트탭을 보았던 날짜가 아닐 경우 리포트 데이터를 새로 로딩한다.
                if (m_dtLastReport.Year != dtNow.Year || m_dtLastReport.Month != dtNow.Month || m_dtLastReport.Day != dtNow.Day)
                    bReload = true;

                if (m_nReadHistoryID != m_nLastHistoryID)
                {
                    bReload = true;
                }*/

                if (bReload == true)
                {
                    proc_btnSelectZone_Click(null, null);
                    btnDetectPSMSelectZone_Click(null, null);
                    //btnNotOperationPSMSelectZone_Click(null, null);
                    m_dtLastReport = dtNow;
                    m_nLastHistoryID = m_nReadHistoryID;
                }

                RibbonButton activeButton = GetActiveReportButton();

                if (activeButton != null)
                    OnRibbonButtonMouseUp(activeButton, null);
            }

            FormMain.Instance.EnableFireReportBtn(false);
            PageHome.ChangeTab(UnE.View.Content.ContentOwnerTab.REPORT_TAB);
        }

        // 현재 활성화 상태인 Report Button을 얻어온다.
        private RibbonButton GetActiveReportButton()
        {
            if (btnDetectHistory.IsChecked)
                return btnDetectHistory;

            if (btnProcessHistory.IsChecked)
                return btnProcessHistory;

            if (btnReactionHistory.IsChecked)
                return btnReactionHistory;

            if (btnSMSHistory.IsChecked)
                return btnSMSHistory;

            if (btnDetectPSMHistory.IsChecked)
                return btnDetectPSMHistory;

            if (btnNotOperationPSMHistory.IsChecked)
                return btnNotOperationPSMHistory;

            if (btnActionPSMHistory.IsChecked)
                return btnActionPSMHistory;

            if (btnSMSPSMHistory.IsChecked)
                return btnSMSPSMHistory;

            if (btnDetectIntrusionHistory.IsChecked)
                return btnDetectIntrusionHistory;

            if (btnProcessIntrusionHistory.IsChecked)
                return btnProcessIntrusionHistory;

            if (btnReactionIntrusionHistory.IsChecked)
                return btnReactionIntrusionHistory;

            if (btnSMSIntrusionHistory.IsChecked)
                return btnSMSIntrusionHistory;

            return null;
        }

        // 날짜 옵션이 오늘까지인가?
        private bool UntilToday()
        {
            if (m_dtLastReport.Year == DatePickerEnd.Value.Year &&
                m_dtLastReport.Month == DatePickerEnd.Value.Month &&
                m_dtLastReport.Day == DatePickerEnd.Value.Day)
                return true;

            return false;
        }

        private bool NeedRefreshReport(ref DateTime dtNow)
        {
            // 날짜 옵션이 오늘까지인가?
            if (UntilToday())
            {
                if (DatePickerEnd.Value.Year != dtNow.Year || DatePickerEnd.Value.Month != dtNow.Month || DatePickerEnd.Value.Day != dtNow.Day)
                {
                    // 날짜가 변경되었으므로 오늘 날짜로 바꾸어준다.
                    DatePickerEnd.Value = dtNow;
                    return true;
                }
            }

            // SensorZoneHistory를 검사한다.
            if (m_nReadHistoryID != m_nLastHistoryID)
                return true;

            // SensorReactionHistory를 검사한다.
            string strSQL = "Select max(srh.ID) from SensorReactionHistory as srh, SensorZoneHistory as szh where srh.SensorHistoryID = szh.id and szh.SiteID = ";
            strSQL += UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            int nLastReactionHistoryID = -1;

            if (arrResult != null && arrResult.Count > 0)
            {
                nLastReactionHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);

                if (nLastReactionHistoryID > SensorHistoryManager.Instance.LastSensorReactionHistoryID)
                {
                    SensorHistoryManager.Instance.LastSensorReactionHistoryID = nLastReactionHistoryID;
                    return true;
                }
            }

            return false;
        }

        //////////////////////////////////////////////////////////////////////////

        #endregion (ChangeTab)

        public static string EnginPath()
        {
            string szMainPath = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            string szWorkPath = szMainPath;
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "common\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            szWorkPath = szMainPath + "SOP\\";
            if (File.Exists(szWorkPath + "CoreDn.dll"))
                return szWorkPath;

            return szMainPath;
        }

        /// <summary>
        /// Resource Dll path를 찾는 함수
        /// </summary>
        //public static string StylesPath()
        //{
        //    string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

        //    if (System.IO.Directory.Exists(strExePath + "\\Styles"))
        //        return strExePath + "\\Styles\\";

        //    if (System.IO.Directory.Exists(strExePath + "\\..\\Styles"))
        //        return strExePath + "\\..\\Styles\\";

        //    if (System.IO.Directory.Exists(strExePath + "\\..\\..\\Styles"))
        //        return strExePath + "\\..\\..\\Styles\\";

        //    if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\Styles"))
        //        return strExePath + "\\..\\..\\..\\Styles\\";

        //    if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\..\\Styles"))
        //        return strExePath + "\\..\\..\\..\\..\\Styles\\";

        //    if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\..\\..\\Styles"))
        //        return strExePath + "\\..\\..\\..\\..\\..\\Styles\\";

        //    if (System.IO.Directory.Exists(strExePath + "\\..\\..\\..\\..\\..\\..\\Styles"))
        //        return strExePath + "\\..\\..\\..\\..\\..\\..\\Styles\\";

        //    if (System.IO.Directory.Exists(strExePath + "\\SOP\\Styles"))
        //        return strExePath + "\\SOP\\Styles\\";

        //    return strExePath + "\\Styles\\";
        //}

        public void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {

            HideToolbar();

            if (cmbSensorDetectTooltip != null)
            {
                if (cmbSensorDetectTooltip.IsBalloon)
                    cmbSensorDetectTooltip.Hide(cmbFireDetect);
                cmbSensorDetectTooltip.Dispose();
                cmbSensorDetectTooltip = null;
            }

            if (sender != this)
            {
                
                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.PageHome.CloseExternal();
                    FormMain.Instance.Close();
                    return;
                });
            }
            else
            {
                FormMain.Instance.PageHome.CloseExternal();
            }

            mClockTimer.Stop();
            mClockTimer.Enabled = false;

            m_CheckReciver.Stop();
            m_CheckReciver.Enabled = false;

            m_bExit = true;

            if (m_frmMessageSender != null && !m_frmMessageSender.IsDisposed)
            {
                m_frmMessageSender.CloseForm = true;
            }

            if (m_frmMessageReceiver != null && !m_frmMessageReceiver.IsDisposed)
            {
                m_frmMessageReceiver.CloseForm = true;
            }


            if (m_netMgr != null)
                m_netMgr.ReleaseThread();

            SensorSignalReciver.Instance.Dispose();

            ProcessManager.Instance.Dispose();

            KillCCTVProcess();

            UnE.View.Content.FormContentUnity.KillProcess("UnitySam");
            UnE.View.Content.FormContentUnity.KillProcess("UnitySamInside");
            UnE.View.Content.FormContentUnity.KillProcess("CCTVViewer");
            UnE.View.Content.FormContentUnity.KillProcess("libCCTV");
            UnE.View.Content.FormContentUnity.KillProcess("EnergyOutside");
            UnE.View.Content.FormContentUnity.KillProcess("SeoulUnv");
            UnE.View.Content.FormContentUnity.KillProcess("BusanUnv");
        }

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            m_MainTimer.Enabled = false;
            m_MainTimer.Stop();

            //if (m_bExit != true && m_PageHome != null)
            //{
            //    m_PageHome.Redraw3DView();
            //}

            //if (m_bExit != true)
            //{
            //    m_MainTimer.Enabled = true;
            //    m_MainTimer.Start();
            //}
        }

        private void OnClickToolBarButton(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (button.Tag != null && (int)button.Tag == ID.ID_VIEW_HOME)
            {
                if (m_PaneBtnHome.Visible == true)
                {
                    m_PaneBtnHome.Visible = false;
                    return;
                }

                Point pt = new Point(m_toolbar.Location.X, m_toolbar.Location.Y + m_toolbar.Size.Height);

                // Toolbar가 화면 아래쪽에 있으면 m_PaneBtnHome을 Toolbar 위쪽에 띄운다.
                if (pt.Y + m_PaneBtnHome.Size.Height > FormFrame.Instance.Location.Y + FormFrame.Instance.Size.Height)
                    pt = new Point(m_toolbar.Location.X, m_toolbar.Location.Y - m_PaneBtnHome.Size.Height);

                m_PaneBtnHome.Location = pt;

                m_PaneBtnHome.Show(this);
                return;
            }
            //if (button.Tag != null && (int)button.Tag == ID.ID_VIEW_OUTSIDE)
            //{
            //    SelectMonitoringTab();
            //    return;
            //}
            //if (button.Tag != null && (int)button.Tag == ID.ID_VIEW_INSIDE)
            //{
            //    Select2DTab();
            //    return;
            //}
            if (button.Tag != null && (int)button.Tag == ID.ID_VIEW_CCTV)
            {
                // 내부 CCTV탭은 사용하지 않으므로 CCTVList와 연결함 skkim 2016-07-25
                //SelectCCTVTab();
                //return;
            }

            m_PageHome.OnClickToolBarButton((Button)sender);

            if (button == m_PaneBtnHome.BtnMainHome)
                m_PaneBtnHome.Visible = false;
            else if (button == m_PaneBtnHome.Btn14Home)
                m_PaneBtnHome.Visible = false;
            else if (button == m_PaneBtnHome.Btn56Home)
                m_PaneBtnHome.Visible = false;
            else if (button == m_PaneBtnHome.BtnCoalHome)
                m_PaneBtnHome.Visible = false;

            if (button.Tag != null && (int)button.Tag == ID.ID_VIEW_SIMULATOR)
            {
            }
        }

        private void cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuildingGroup.SelectedIndex;
            if (nSelectedIndex < 0)
                return;
            BuildingGroup buildingGroup = (BuildingGroup)cboBuildingGroup.Items[nSelectedIndex];
            ComboHelper.InitBuildingComboBox(cboBuilding, buildingGroup);

            cboBuilding.Sorted = true;
            cboBuilding.Sorted = false;

            if (cboBuilding.Items.Count > 0)
                cboBuilding.SelectedIndex = 0;

            if (buildingGroup != ZoneManager.Instance.OutdoorBuildingGroup)
                cboFloor.Enabled = true;
        }

        private void cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboBuilding.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            Object obj = cboBuilding.Items[nSelectedIndex];
            if (obj.GetType() == typeof(Building))
            {
                ComboHelper.InitFloorComboBox(cboFloor, (Building)obj);
                btnSelectZone.Enabled = true;
            }
            else
            {
                cboFloor.Items.Clear();
                btnSelectZone.Enabled = false;

                if (checkBoxEquipZoneCCTV.Checked)
                {
                    cboFloor.Enabled = false;

                    cboEquipZone.Items.Clear();

                    if (obj.GetType() != typeof(Zone))
                        return;

                    Zone zone = (Zone)obj;
                    List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);

                    foreach (EquipmentZone equipZone in arrEquipZones)
                    {
                        cboEquipZone.Items.Add(equipZone);
                    }

                    if (cboEquipZone.Items.Count > 0)
                        cboEquipZone.SelectedIndex = 0;
                }
                else
                {
                    cboFloor.Enabled = true;
                    cboFloor.Items.Add("-");
                }
            }

            if (cboFloor.Items.Count > 0)
                cboFloor.SelectedIndex = 0;
        }

        private void btnSelectZone_Click(object sender, EventArgs e)
        {
            Zone zoneSelected = null;
            if (cboFloor.Text.Length == 0)
                return;
            else if (cboFloor.Text == "-")
            {
                zoneSelected = (Zone)cboBuilding.Items[cboBuilding.SelectedIndex];
            }
            else
            {
                Floor floor = (Floor)cboFloor.Items[cboFloor.SelectedIndex];
                zoneSelected = floor.Zone;
            }

            if (zoneSelected == null)
                return;

            // AdminTab에서는 모든뷰가 나타나야 하므로 이동하면 안된다. 2015-07-09 skkim
            if (mCurrentTab != UnE.View.Content.ContentOwnerTab.ADMIN_TAB)
                Select2DTab();

            BuildingGroup buildingGroupSelected = (BuildingGroup)cboBuildingGroup.Items[cboBuildingGroup.SelectedIndex];
            m_PageHome.ChangeFloor(buildingGroupSelected, zoneSelected.Building, zoneSelected);

            EnableChangeViewBtn();
        }

        public void EnableChangeViewBtn()
        {
            btnBoth.Enabled = true;
            btnInside.Enabled = true;
        }

        public void SelectIndoorZone(Zone zone)
        {
            if (zone == null || zone.Building == null)
                return;

            BuildingGroup buildingGroupSelected = zone.Building.BuildingGroup;
            if (buildingGroupSelected != null)
            {
                m_PageHome.ChangeFloor(buildingGroupSelected, zone.Building, zone);

                EnableChangeViewBtn();

                ChangeZoneComboBox(zone);
            }
        }

        public void EnableFireReportBtn(bool bEnable, int nCase = 1)
        {
            if (btnFire.Text == RaiseManualFire)
                btnFire.Enabled = bEnable;

            if (nCase == 2)
            {
                btnFire.Text = RaiseManualFire;
            }
            else if (nCase == 1)
            {
                btnFire.Text = CloseManualFire;
                //btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar_Blue;
            }

            if (nCase == 2)
                btnFire.Enabled = bEnable;

            if (bEnable == false)
            {
                int i = 0;
                i++;
            }
        }
        public void HideAllPopup()
        {
            // 화면설정 팝업
            if (m_PaneBtnSaveHome.Visible)
                btnSaveHome_Click(btnSaveHome, null);

            if (m_PaneBtnHome.Visible)
            {
                m_PaneBtnHome.Visible = false;
               
            }
        }
        private void OnClickLayerToolBarButton(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            m_PageHome.OnChangeLayer(GetButtonID(btn));

            bool isChecked = IsChecked(btn);
            CheckButton(btn, !isChecked);
        }

        private void btnLayerFR_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerFA_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerHD_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerFE_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerCCTV_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerPump_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerSpringCooler_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerFire_Click(object sender, EventArgs e)
        {
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;
            m_PageHome.OnCommandExcute(GetButtonID(btn));

            CheckReportButton(btn);
        }

        private void RefreshReportButtons(RibbonButton btn)
        {
            foreach (RibbonButton rbtn in m_reportButtons)
            {
                if (rbtn == btn)
                    RefreshReportButton(rbtn, true);
                else
                    RefreshReportButton(rbtn);
            }
        }

        private void CheckReportButton(Button btn)
        {
            //panelMiddle.Visible = false;
            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;

            RefreshReportButtons((RibbonButton)btn);

            switch (Convert.ToInt32(btn.Tag))
            {
                case ID.ID_BTN_DETECT_ANALYZE:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(true);

                    m_PageHome.FrmReport.ShowDetectAnalyze();

                    /*RefreshReportButton(btnDetectAnalyze, true);
                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMAnalyze);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    proc_btnSelectZone.PerformClick();

                    btnDateFormat.Visible =
                    lblSplitUnit.Visible =
                    lblSplitUnitDetail.Visible =
                    proc_cboSplitUnit.Visible =
                    nudSplitUnitDetail.Visible =
                    labelDetectDateFormat.Visible = false;
                    break;

                case ID.ID_BTN_DETECT:

                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(true);

                    m_PageHome.FrmReport.ShowDetectReport();

                    /*RefreshReportButton(btnDetectAnalyze);
                    RefreshReportButton(btnDetectHistory, true);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMAnalyze);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    proc_btnSelectZone.PerformClick();

                    break;

                case ID.ID_BTN_NOTOPERATION:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(false);

                    m_PageHome.FrmReport.ShowProcessHistoryReport();

                    /*RefreshReportButton(btnDetectAnalyze);
                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory, true);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMAnalyze);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    proc_btnSelectZone.PerformClick();

                    break;

                case ID.ID_BTN_ACTION:
                    SelectActionPage();
                    /*DateTime dtToday = DateTime.Now;

                    //SetReactionEndDate(dtToday, dtToday);
                    //SetReactionStartDate(dtToday.Subtract(TimeSpan.FromDays(6)), dtToday);

                    panelReactionHistory.Visible = true;

                    m_PageHome.FrmReport.ShowReactionHistoryReport();

                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory, true);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);

                    btnReactionSelectDisaster.PerformClick();*/

                    break;

                case ID.ID_BTN_SMSREPORT:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(false);

                    m_PageHome.FrmReport.ShowSmsHistoryReport();

                    /*RefreshReportButton(btnDetectAnalyze);
                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory, true);
                    RefreshReportButton(btnDetectPSMAnalyze);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    proc_btnSelectZone.PerformClick();

                    break;

                case ID.ID_BTN_DETECT_PSM_ANALYZE:
                    pnDetectPSM.Visible = true;
                    m_PageHome.FrmReport.ShowDetectPSMAnalyze();

                    /*RefreshReportButton(btnDetectAnalyze);
                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMAnalyze, true);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    RefreshReportDetectPSMAnalyze(true);
                    btnDetectPSMDateFormat.Visible =
                    lblDetectPSMSplitUnit.Visible =
                    lblDetectPSMSplitUnitDetail.Visible =
                    cboDetectPSMSplitUnit.Visible =
                    nudDetectPSMSplitUnitDetail.Visible =
                    labelDetectPSMDateFormat.Visible = false;
                    break;

                case ID.ID_BTN_DETECT_PSM:

                    pnDetectPSM.Visible = true;
                    m_PageHome.FrmReport.ShowDetectPSMReport();

                    /*RefreshReportButton(btnDetectAnalyze);
                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMHistory, true);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    RefreshReportDetectPSM(true);
                    btnDetectPSMDateFormat.Visible =
                    lblDetectPSMSplitUnit.Visible =
                    lblDetectPSMSplitUnitDetail.Visible =
                    cboDetectPSMSplitUnit.Visible =
                    nudDetectPSMSplitUnitDetail.Visible =
                    labelDetectPSMDateFormat.Visible = true;

                    break;

                case ID.ID_BTN_NOTOPERATION_PSM:

                    pnNotOperationPSM.Visible = true;

                    m_PageHome.FrmReport.ShowNotOperationPSMReport();

                    /*RefreshReportButton(btnDetectAnalyze);
                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory, true);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    RefreshReportNotOperationPSM(true);

                    break;

                case ID.ID_BTN_ACTION_PSM:

                    SelectPSMActionPage();
                    /*pnActionPSM.Visible = true;

                    m_PageHome.FrmReport.ShowActionPSMReport();

                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory, true);
                    RefreshReportButton(btnSMSPSMHistory);

                    btnReactionPSMSelectDisaster.PerformClick();*/

                    break;

                case ID.ID_BTN_SMSREPORT_PSM:

                    pnSMSPSM.Visible = true;

                    m_PageHome.FrmReport.ShowSMSPSMReport();

                    /*RefreshReportButton(btnDetectAnalyze);
                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory, true);*/

                    RefreshReportSMSPSM(true);

                    break;

                case ID.ID_BTN_DETECT_INTRUSION_ANALYZE: 
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(true);

                    m_PageHome.FrmReport.ShowDetectIntrusionAnalyze();

                    /*RefreshReportButton(btnDetectAnalyze, true);
                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMAnalyze);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    proc_btnSelectZone.PerformClick(); 

                    btnDateFormat.Visible =
                    lblSplitUnit.Visible =
                    lblSplitUnitDetail.Visible =
                    proc_cboSplitUnit.Visible =
                    nudSplitUnitDetail.Visible =
                    labelDetectDateFormat.Visible = false;
                    break;

                case ID.ID_BTN_DETECT_INTRUSION:

                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(true);

                    m_PageHome.FrmReport.ShowDetectIntrusionReport();

                    /*RefreshReportButton(btnDetectAnalyze);
                    RefreshReportButton(btnDetectHistory, true);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMAnalyze);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    proc_btnSelectZone.PerformClick();

                    break;

                case ID.ID_BTN_NOTOPERATION_INTRUSION:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(false);

                    m_PageHome.FrmReport.ShowProcessIntrusionHistoryReport();

                    /*RefreshReportButton(btnDetectAnalyze);
                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory, true);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMAnalyze);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    proc_btnSelectZone.PerformClick();

                    break;

                case ID.ID_BTN_ACTION_INTRUSION:
                    SelectActionIntrusionPage();
                    /*DateTime dtToday = DateTime.Now;

                    //SetReactionEndDate(dtToday, dtToday);
                    //SetReactionStartDate(dtToday.Subtract(TimeSpan.FromDays(6)), dtToday);

                    panelReactionHistory.Visible = true;

                    m_PageHome.FrmReport.ShowReactionHistoryReport();

                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory, true);
                    RefreshReportButton(btnSMSHistory);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);

                    btnReactionSelectDisaster.PerformClick();*/

                    break;

                case ID.ID_BTN_SMSREPORT_INTRUSION:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(false);

                    m_PageHome.FrmReport.ShowSmsIntrusionHistoryReport();

                    /*RefreshReportButton(btnDetectAnalyze);
                    RefreshReportButton(btnDetectHistory);
                    RefreshReportButton(btnProcessHistory);
                    RefreshReportButton(btnReactionHistory);
                    RefreshReportButton(btnSMSHistory, true);
                    RefreshReportButton(btnDetectPSMAnalyze);
                    RefreshReportButton(btnDetectPSMHistory);
                    RefreshReportButton(btnNotOperationPSMHistory);
                    RefreshReportButton(btnActionPSMHistory);
                    RefreshReportButton(btnSMSPSMHistory);*/

                    proc_btnSelectZone.PerformClick();

                    break;
            }

        }
        public void SelectActionIntrusionPage(int nSensorZoneHistoryID = 0)
        {
            this.Cursor = Cursors.WaitCursor;

            react_cboSearchType.Visible = false;
            react_cboSearchTypeIntrusion.Visible = true;
            lblFireSelect.Visible = false;
            lblIntrusionSelect.Visible = true;
            cboFireSelect.Visible = false;
            cboActionIntrusionSelect.Visible = true;

            btnReactionSelectDisaster.Visible = false;
            btnReactionIntrusionSelectDisaster.Visible = true; 

            DateTime dtToday = DateTime.Now;

            //SetReactionEndDate(dtToday, dtToday);
            //SetReactionStartDate(dtToday.Subtract(TimeSpan.FromDays(6)), dtToday);

            panelReactionHistory.Visible = true;

            m_PageHome.FrmReport.ShowReactionIntrusionHistoryReport();

            RefreshReportButtons(btnReactionIntrusionHistory);
            /*RefreshReportButton(btnDetectAnalyze);
            RefreshReportButton(btnDetectHistory);
            RefreshReportButton(btnProcessHistory);
            RefreshReportButton(btnReactionHistory, true);
            RefreshReportButton(btnSMSHistory);
            RefreshReportButton(btnDetectPSMAnalyze);
            RefreshReportButton(btnDetectPSMHistory);
            RefreshReportButton(btnNotOperationPSMHistory);
            RefreshReportButton(btnActionPSMHistory);
            RefreshReportButton(btnSMSPSMHistory);*/

            btnReactionIntrusionSelectDisaster.PerformClick();

            if (nSensorZoneHistoryID > 0)
                m_PageHome.FrmReport.SelectActionIntrusionPage(nSensorZoneHistoryID); 

            panelReactionHistory.Refresh(); 
            this.Cursor = Cursors.Arrow;
        }

        public void SelectPSMActionPage(int nSensorZoneHistoryID = 0)
        {
            this.Cursor = Cursors.WaitCursor;

            pnActionPSM.Visible = true;

            m_PageHome.FrmReport.ShowActionPSMReport();

            RefreshReportButtons(btnActionPSMHistory);
            /*RefreshReportButton(btnDetectAnalyze);
            RefreshReportButton(btnDetectHistory);
            RefreshReportButton(btnProcessHistory);
            RefreshReportButton(btnReactionHistory);
            RefreshReportButton(btnSMSHistory);
            RefreshReportButton(btnDetectPSMHistory);
            RefreshReportButton(btnNotOperationPSMHistory);
            RefreshReportButton(btnActionPSMHistory, true);
            RefreshReportButton(btnSMSPSMHistory);*/

            if (nSensorZoneHistoryID != 0)
                m_PageHome.FrmReport.SelectPSMActionPage(nSensorZoneHistoryID);

            btnReactionPSMSelectDisaster.PerformClick();

            this.Cursor = Cursors.Arrow;
        }

        public void SelectActionPage(int nSensorZoneHistoryID = 0)
        {
            this.Cursor = Cursors.WaitCursor;
            react_cboSearchType.Visible = true;
            react_cboSearchTypeIntrusion.Visible = false;
            lblFireSelect.Visible = true;
            lblIntrusionSelect.Visible = false;
            cboFireSelect.Visible = true;
            cboActionIntrusionSelect.Visible = false;
            btnReactionSelectDisaster.Visible = true;
            btnReactionIntrusionSelectDisaster.Visible = false; 

            DateTime dtToday = DateTime.Now;

            //SetReactionEndDate(dtToday, dtToday);
            //SetReactionStartDate(dtToday.Subtract(TimeSpan.FromDays(6)), dtToday);

            panelReactionHistory.Visible = true;

            m_PageHome.FrmReport.ShowReactionHistoryReport();

            RefreshReportButtons(btnReactionHistory);
            /*RefreshReportButton(btnDetectAnalyze);
            RefreshReportButton(btnDetectHistory);
            RefreshReportButton(btnProcessHistory);
            RefreshReportButton(btnReactionHistory, true);
            RefreshReportButton(btnSMSHistory);
            RefreshReportButton(btnDetectPSMAnalyze);
            RefreshReportButton(btnDetectPSMHistory);
            RefreshReportButton(btnNotOperationPSMHistory);
            RefreshReportButton(btnActionPSMHistory);
            RefreshReportButton(btnSMSPSMHistory);*/

            panelReactionHistory.Refresh();
            btnReactionSelectDisaster.PerformClick();

            if (nSensorZoneHistoryID > 0)
                m_PageHome.FrmReport.SelectActionPage(nSensorZoneHistoryID);

            this.Cursor = Cursors.Arrow;
        }

        private void EnableSubViewOption(bool isView)
        {
            btnDateFormat.Visible =
            labelDetectDateFormat.Visible =
            lblSplitUnit.Visible =
            lblSplitUnitDetail.Visible =
            lblViewCount.Visible =
            proc_cboSplitUnit.Visible =
            nudSplitUnitDetail.Visible =
            proc_cboViewCount.Visible = isView;
        }

        private void RefreshReportButton(RibbonButton btn, bool isCheck = false)
        {
            if (btn.IsChecked == !isCheck)
            {
                btn.IsChecked = isCheck;
                btn.Refresh();
            }
        }

        private void RefreshReportButton(RibbonButton btn1, RibbonButton btn2)
        {
            if (btn1.IsChecked == true)
            {
                btn1.IsChecked = false;
                btn1.Refresh();
            }
            else if (btn2.IsChecked == true)
            {
                btn2.IsChecked = false;
                btn2.Refresh();
            }
        }


        #region Report Building ComboBox

        private void ChangeReportBuildingGroup(ComboBox cboBG, ComboBox cboB, ComboBox cboF) // BuildingGroup, Building, Floor
        {
            if (cboBG.SelectedIndex == 0)
            {
                cboB.SelectedIndex = 0;
                cboF.SelectedIndex = 0;
                cboB.Enabled = false;
                cboF.Enabled = false;
                return;
            }
            cboB.Enabled = true;

            int nSelectedIndex = cboBG.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            BuildingGroup buildingGroup = (BuildingGroup)cboBG.Items[nSelectedIndex];

            ComboHelper.InitBuildingComboBox(cboB, buildingGroup);

            cboB.Sorted = true;
            cboB.Sorted = false;
            cboB.Items.Insert(0, "모든 건물");

            if (cboB.Items.Count > 0)
                cboB.SelectedIndex = 0;
        }

        private void ChangeReportBuilding(ComboBox cboB, ComboBox cboF) // Building, Floor
        {
            if (cboB.SelectedIndex == 0)
            {
                cboF.SelectedIndex = 0;
                cboF.Enabled = false;
                return;
            }
            cboF.Enabled = true;

            int nSelectedIndex = cboB.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            Object obj = cboB.Items[nSelectedIndex];
            if (obj.GetType() == typeof(Building))
            {
                ComboHelper.InitFloorComboBox(cboF, (Building)obj);
                cboF.Items.Insert(0, "모든 층");
            }
            else
            {
                cboF.Items.Clear();
                cboF.Items.Add("-");
            }

            if (cboF.Items.Count > 0)
                cboF.SelectedIndex = 0;
        }

        #region Process Panel Building Selection Event

        private void Proc_cboBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeReportBuildingGroup(proc_cboBuildingGroup, proc_cboBuilding, proc_cboFloor);
        }

        private void proc_cboBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeReportBuilding(proc_cboBuilding, proc_cboFloor);
        }

        #endregion

        #region Detect PSM Panel Building Selection Event

        private void cboDetectPSMBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region SMS PSM Panel Building Selection Event

        private void cboSMSPSMBuilding_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Not Operation PSM Panel Building Selection Event

        private void cboNotOperationPSMBuildingGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        #endregion

        #endregion


        #region Report DateTime Picker

        bool m_isSameReportDataTimeSetting = false;
        private void SetAllSameDateTime(string strDate, bool isStartDate)
        {
            if (isStartDate == true)
            {
                proc_btnStartDate.Text = strDate;
                react_btnStartDate.Text = strDate;
                btnDetectPSMStartDate.Text = strDate;
                btnNotOperationPSMStartDate.Text = strDate;
                btnActionPSMStartDate.Text = strDate;
                btnSMSPSMStartDate.Text = strDate;

                proc_btnStartDate.Refresh();
                react_btnStartDate.Refresh();
                btnDetectPSMStartDate.Refresh();
                btnNotOperationPSMStartDate.Refresh();
                btnActionPSMStartDate.Refresh();
                btnSMSPSMStartDate.Refresh();
            }
            else
            {
                proc_btnEndDate.Text = strDate;
                react_btnEndDate.Text = strDate;
                btnDetectPSMEndDate.Text = strDate;
                btnNotOperationPSMEndDate.Text = strDate;
                btnActionPSMEndDate.Text = strDate;
                btnSMSPSMEndDate.Text = strDate;

                proc_btnEndDate.Refresh();
                react_btnEndDate.Refresh();
                btnDetectPSMEndDate.Refresh();
                btnNotOperationPSMEndDate.Refresh();
                btnActionPSMEndDate.Refresh();
                btnSMSPSMEndDate.Refresh();
            }
        }

        private void SetAllSameLatelyDate(int nIndex)
        {
            m_isSameReportDataTimeSetting = true;

            cboDetectPSMLatelyDate.SelectedIndex = nIndex;
            proc_cboLatelyDate.SelectedIndex = nIndex;
            cboSMSPSMLatelyDate.SelectedIndex = nIndex;
            cboNotOperationPSMLatelyDate.SelectedIndex = nIndex;

            m_isSameReportDataTimeSetting = false;
        }

        // 모든 DateTimePicker 컨트롤 숨김
        private void HideDateTimePicker()
        {
            DatePickerEnd.Visible = false;
            DatePickerEnd2.Visible = false;
            DatePickerStart.Visible = false;
            DatePickerStart2.Visible = false;

            DatePickerDetectPSMStart.Visible = false;
            DatePickerDetectPSMEnd.Visible = false;
            DatePickerNotOperationPSMStart.Visible = false;
            DatePickerNotOperationPSMEnd.Visible = false;
            DatePickerActionPSMStart.Visible = false;
            DatePickerActionPSMEnd.Visible = false;
            DatePickerSMSPSMStart.Visible = false;
            DatePickerSMSPSMEnd.Visible = false;
        }

        // 문자열이 날짜 데이터인지 확인
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

        private void ClickReportDateButton(Button btn, DateTimePicker timePicker, Panel pn)
        {
            HideDateTimePicker();

            if (IsDate(btn.Text))
                timePicker.Value = System.Convert.ToDateTime(btn.Text);

            int x = btn.Left;
            int y = (btn.Top + btn.Height - 22);

            Point pt = pn.PointToScreen(new Point(x, y));
            timePicker.Location = new Point(pt.X - FormFrame.Instance.Location.X, pt.Y - FormFrame.Instance.Location.Y);
            timePicker.DropDownAlign = LeftRightAlignment.Left;
            timePicker.Show();

            timePicker.Select();
            SendKeys.Send("%{DOWN}");
        }

        private bool ChangeReportDateTime(DateTimePicker timePicker, Button btn)
        {
            DateTime dtToday = DateTime.Now;
            string szText = timePicker.Value.ToShortDateString();
            DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);

            if (dtszText > dtToday)
            {
                MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                return false;
            }

            btn.Text = szText;
            btn.Refresh();

            HideDateTimePicker();

            return true;
        }

        private void ChangeReportLatelyDate(ComboBox cboLayelyDate)
        {
            if (m_isSameReportDataTimeSetting == true)
                return;

            if (cboLayelyDate.SelectedIndex == 0)
                return;

            DateTime dt = DateTime.Now;
            DateTime dtOld = new DateTime();

            switch (cboLayelyDate.SelectedIndex)
            {
                case 1:
                    dtOld = dt.AddMonths(-12);
                    break;
                case 2:
                    dtOld = dt.AddMonths(-6);
                    break;
                case 3:
                    dtOld = dt.AddMonths(-3);
                    break;
                case 4:
                    dtOld = dt.AddMonths(-1);
                    break;
                case 5:
                    dtOld = dt.AddDays(-6);
                    break;
                case 6:
                    dtOld = dt;
                    break;
            }

            SetAllSameDateTime(dtOld.ToString().Substring(0, 10), true);
            SetAllSameDateTime(dt.ToString().Substring(0, 10), false);
            SetAllSameLatelyDate(cboLayelyDate.SelectedIndex);
        }

        #region Process Panel DateTime Event

        private void proc_btnStartDate_Click(object sender, EventArgs e)
        {
            HideDateTimePicker();

            if (IsDate(proc_btnStartDate.Text))
                DatePickerStart.Value = System.Convert.ToDateTime(proc_btnStartDate.Text);

            int x = proc_btnStartDate.Left;
            int y = (proc_btnStartDate.Top + proc_btnStartDate.Height - 22);

            Point pt = panelProcessHistory.PointToScreen(new Point(x, y));
            DatePickerStart.Location = new Point(pt.X - FormFrame.Instance.Location.X, pt.Y - FormFrame.Instance.Location.Y);
            DatePickerStart.DropDownAlign = LeftRightAlignment.Left;
            DatePickerStart.Show();

            DatePickerStart.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void proc_btnEndDate_Click(object sender, EventArgs e)
        {
            HideDateTimePicker();

            if (IsDate(proc_btnEndDate.Text))
                DatePickerEnd.Value = System.Convert.ToDateTime(proc_btnEndDate.Text);

            int x = proc_btnEndDate.Left;
            int y = (proc_btnEndDate.Top + proc_btnEndDate.Height - 22);

            Point pt = panelProcessHistory.PointToScreen(new Point(x, y));
            DatePickerEnd.Location = new Point(pt.X - FormFrame.Instance.Location.X, pt.Y - FormFrame.Instance.Location.Y);
            DatePickerEnd.DropDownAlign = LeftRightAlignment.Left;
            DatePickerEnd.Show();

            DatePickerEnd.Select();
            SendKeys.Send("%{DOWN}");

        }

        private void DatePickerStart_Leave(object sender, EventArgs e)
        {
            DatePickerStart.Visible = false;
        }

        private void DatePickerStart_ValueChanged(object sender, EventArgs e)
        {
            DateTime dtToday = DateTime.Now;
            string szText = DatePickerStart.Value.ToShortDateString();
            DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);

            if (dtszText > dtToday)
            {
                MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                return;
            }

            SetAllSameDateTime(szText, true);

            HideDateTimePicker();
        }

        private void DatePickerEnd_ValueChanged(object sender, EventArgs e)
        {
            DateTime dtToday = DateTime.Now;

            proc_btnEndDate.Refresh();
            string szText = DatePickerEnd.Value.ToShortDateString();
            DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);
            if (dtszText > dtToday)
            {
                MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                return;
            }

            SetAllSameDateTime(szText, false);

            HideDateTimePicker();
        }

        public void proc_cboLatelyDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_isSameReportDataTimeSetting == true)
                return;

            if (proc_cboLatelyDate.SelectedIndex == 0)
                return;

            DateTime dt = DateTime.Now;
            DateTime dtOld = new DateTime();

            switch (proc_cboLatelyDate.SelectedIndex)
            {
                case 1:
                    dtOld = dt.AddMonths(-12);
                    break;
                case 2:
                    dtOld = dt.AddMonths(-6);
                    break;
                case 3:
                    dtOld = dt.AddMonths(-3);
                    break;
                case 4:
                    dtOld = dt.AddMonths(-1);
                    break;
                case 5:
                    dtOld = dt.AddDays(-6);
                    break;
                case 6:
                    dtOld = dt;
                    break;
            }

            SetAllSameDateTime(dtOld.ToString().Substring(0, 10), true);
            SetAllSameDateTime(dt.ToString().Substring(0, 10), false);
            SetAllSameLatelyDate(proc_cboLatelyDate.SelectedIndex);

        }

        #endregion

        #region Reaction Panel DateTime Event

        private void react_btnStartDate_Click(object sender, EventArgs e)
        {
            HideDateTimePicker();

            if (IsDate(react_btnStartDate.Text))
                DatePickerStart2.Value = System.Convert.ToDateTime(react_btnStartDate.Text);

            int x = react_btnStartDate.Left;
            int y = (react_btnStartDate.Top + react_btnStartDate.Height - 22);

            Point pt = this.panelReactionHistory.PointToScreen(new Point(x, y));
            DatePickerStart2.Location = new Point(pt.X - FormFrame.Instance.Location.X, pt.Y - FormFrame.Instance.Location.Y);
            DatePickerStart2.Show();
            DatePickerStart2.DropDownAlign = LeftRightAlignment.Left;

            DatePickerStart2.Select();
            SendKeys.Send("%{DOWN}");

        }

        private void react_btnEndDate_Click(object sender, EventArgs e)
        {
            HideDateTimePicker();

            if (IsDate(react_btnEndDate.Text))
                DatePickerEnd2.Value = System.Convert.ToDateTime(react_btnEndDate.Text);

            int x = react_btnEndDate.Left;
            int y = (react_btnEndDate.Top + react_btnEndDate.Height - 22);

            Point pt = this.panelReactionHistory.PointToScreen(new Point(x, y));
            DatePickerEnd2.Location = new Point(pt.X - FormFrame.Instance.Location.X, pt.Y - FormFrame.Instance.Location.Y);
            DatePickerEnd2.Show();
            DatePickerEnd2.DropDownAlign = LeftRightAlignment.Left;

            DatePickerEnd2.Select();
            SendKeys.Send("%{DOWN}");

        }

        private void DatePickerStart2_ValueChanged(object sender, EventArgs e)
        {
            DateTime dtToday = DateTime.Now;
            SetReactionStartDate(DatePickerStart2.Value, dtToday);

            HideDateTimePicker();
        }

        private void DatePickerEnd2_ValueChanged(object sender, EventArgs e)
        {
            DateTime dtToday = DateTime.Now;
            SetReactionEndDate(DatePickerEnd2.Value, dtToday);

            HideDateTimePicker();
        }

        private bool SetReactionStartDate(DateTime dtTime, DateTime dtToday)
        {
            react_btnStartDate.Refresh();
            string szText = dtTime.ToShortDateString();

            DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);
            DateTime dtToday_Compare = DateTime.ParseExact(dtToday.ToShortDateString(), "yyyy-MM-dd", null);
            if (dtszText > dtToday_Compare)
            {
                MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                react_btnStartDate.Text = DateTime.Now.ToShortDateString();
                return false;
            }

            if (react_btnStartDate.Text == "시작 일" || react_btnEndDate.Text == "끝 일")
            {
            }
            else
            {
                DateTime dtEndDate = DateTime.ParseExact(react_btnEndDate.Text, "yyyy-MM-dd", null);
                if (dtEndDate < dtszText)
                {
                    MessageBox.Show("시작 일이 끝 일보다 클 수 없습니다.");
                    return false;
                }
                SetAllSameDateTime(szText, true);
            }


            return true;
        }

        private bool SetReactionEndDate(DateTime dtTime, DateTime dtToday)
        {
            react_btnEndDate.Refresh();
            string szText = dtTime.ToShortDateString();

            DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);
            DateTime dtToday_Compare = DateTime.ParseExact(dtToday.ToShortDateString(), "yyyy-MM-dd", null);
            if (dtszText > dtToday_Compare)
            {
                MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                react_btnEndDate.Text = DateTime.Now.ToShortDateString();
                return false;
            }

            if (react_btnStartDate.Text == "시작 일" || react_btnEndDate.Text == "끝 일")
            {
            }
            else
            {
                DateTime dtStartDate = DateTime.ParseExact(react_btnStartDate.Text, "yyyy-MM-dd", null);
                if (dtStartDate > dtszText)
                {
                    MessageBox.Show("시작 일이 끝 일보다 클 수 없습니다.");
                    return false;
                }
                SetAllSameDateTime(szText, false);
            }


            return true;
        }

        #endregion

        #region Detect PSM Panel DateTime Event

        private void btnDetectPSMStartDate_Click(object sender, EventArgs e)
        {
            ClickReportDateButton(btnDetectPSMStartDate, DatePickerDetectPSMStart, pnDetectPSM);
        }

        private void btnDetectPSMEndDate_Click(object sender, EventArgs e)
        {
            ClickReportDateButton(btnDetectPSMEndDate, DatePickerDetectPSMEnd, pnDetectPSM);
        }

        private void DatePickerDetectPSMStart_ValueChanged(object sender, EventArgs e)
        {
            if (ChangeReportDateTime(DatePickerDetectPSMStart, btnDetectPSMStartDate))
            {
                SetAllSameDateTime(btnDetectPSMStartDate.Text, true);
            }
        }

        private void DatePickerDetectPSMEnd_ValueChanged(object sender, EventArgs e)
        {
            if (ChangeReportDateTime(DatePickerDetectPSMEnd, btnDetectPSMEndDate))
            {
                SetAllSameDateTime(btnDetectPSMEndDate.Text, false);
            }
        }

        private void cboDetectPSMLatelyDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboDetectPSMLatelyDate.SelectedIndex == 0)
                return;

            DateTime dt = DateTime.Now;
            DateTime dtOld = new DateTime();

            switch (cboDetectPSMLatelyDate.SelectedIndex)
            {
                case 1:
                    dtOld = dt.AddMonths(-12);
                    break;
                case 2:
                    dtOld = dt.AddMonths(-6);
                    break;
                case 3:
                    dtOld = dt.AddMonths(-3);
                    break;
                case 4:
                    dtOld = dt.AddMonths(-1);
                    break;
                case 5:
                    dtOld = dt.AddDays(-6);
                    break;
                case 6:
                    dtOld = dt;
                    break;
            }

            SetAllSameDateTime(dtOld.ToString().Substring(0, 10), true);
            SetAllSameDateTime(dt.ToString().Substring(0, 10), false);
            SetAllSameLatelyDate(cboDetectPSMLatelyDate.SelectedIndex);
        }

        #endregion

        #region Not OperationPSM Panel DateTime Event

        private void btnNotOperationPSMStartDate_Click(object sender, EventArgs e)
        {
            ClickReportDateButton(btnNotOperationPSMStartDate, DatePickerNotOperationPSMStart, pnNotOperationPSM);
        }

        private void btnNotOperationPSMEndDate_Click(object sender, EventArgs e)
        {
            ClickReportDateButton(btnNotOperationPSMEndDate, DatePickerNotOperationPSMEnd, pnNotOperationPSM);
        }

        private void DatePickerNotOperationPSMStart_ValueChanged(object sender, EventArgs e)
        {
            if (ChangeReportDateTime(DatePickerNotOperationPSMStart, btnNotOperationPSMStartDate))
            {
                SetAllSameDateTime(btnNotOperationPSMStartDate.Text, true);
            }
        }

        private void DatePickerNotOperationPSMEnd_ValueChanged(object sender, EventArgs e)
        {
            if (ChangeReportDateTime(DatePickerNotOperationPSMEnd, btnNotOperationPSMEndDate))
            {
                SetAllSameDateTime(btnNotOperationPSMEndDate.Text, false);
            }
        }

        public void cboNotOperationPSMLatelyDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeReportLatelyDate(cboNotOperationPSMLatelyDate);
        }

        #endregion

        #region Action PSM Panel DateTime Event

        private void btnActionPSMStartDate_Click(object sender, EventArgs e)
        {
            ClickReportDateButton(btnActionPSMStartDate, DatePickerActionPSMStart, pnActionPSM);
        }

        private void btnActionPSMEndDate_Click(object sender, EventArgs e)
        {
            ClickReportDateButton(btnActionPSMEndDate, DatePickerActionPSMEnd, pnActionPSM);
        }

        private void DatePickerActionPSMStart_ValueChanged(object sender, EventArgs e)
        {
            if (ChangeReportDateTime(DatePickerActionPSMStart, btnActionPSMStartDate))
            {
                SetAllSameDateTime(btnActionPSMStartDate.Text, true);
            }
        }

        private void DatePickerActionPSMEnd_ValueChanged(object sender, EventArgs e)
        {
            if (ChangeReportDateTime(DatePickerActionPSMEnd, btnActionPSMEndDate))
            {
                SetAllSameDateTime(btnActionPSMEndDate.Text, false);
            }
        }

        #endregion

        #region SMS PSM Panel DateTime Event

        private void btnSMSPSMStartDate_Click(object sender, EventArgs e)
        {
            ClickReportDateButton(btnSMSPSMStartDate, DatePickerSMSPSMStart, pnSMSPSM);
        }

        private void btnSMSPSMEndDate_Click(object sender, EventArgs e)
        {
            ClickReportDateButton(btnSMSPSMEndDate, DatePickerSMSPSMEnd, pnSMSPSM);
        }

        private void DatePickerSMSPSMStart_ValueChanged(object sender, EventArgs e)
        {
            if (ChangeReportDateTime(DatePickerSMSPSMStart, btnSMSPSMStartDate))
            {
                SetAllSameDateTime(btnSMSPSMStartDate.Text, true);
            }
        }

        private void DatePickerSMSPSMEnd_ValueChanged(object sender, EventArgs e)
        {
            if (ChangeReportDateTime(DatePickerSMSPSMEnd, btnSMSPSMEndDate))
            {
                SetAllSameDateTime(btnSMSPSMEndDate.Text, false);
            }
        }

        public void cboSMSPSMLatelyDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeReportLatelyDate(cboSMSPSMLatelyDate);
        }

        #endregion

        #endregion Report DateTime Picker
         
        private void proc_btnSelectZone_Click(object sender, EventArgs e)
        {
            if (proc_btnStartDate.Text == "시작 일" || proc_btnEndDate.Text == "끝 일")
            {
                MessageBox.Show("날짜를 입력해주세요");
                return;
            }

            DateTime startDate = DateTime.ParseExact(proc_btnStartDate.Text, "yyyy-MM-dd", null);
            DateTime EndDate = DateTime.ParseExact(proc_btnEndDate.Text, "yyyy-MM-dd", null);

            if (startDate > EndDate)
            {
                MessageBox.Show("시작 일이 더 클 수 없습니다.");
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                string strGroupName = proc_cboBuildingGroup.Text.ToString();
                string strBuildingName = proc_cboBuilding.Text.ToString();
                string strFloorName = proc_cboFloor.Text.ToString();

                int nSplitUnitOfMeansure = proc_cboSplitUnit.SelectedIndex;
                int nViewCount = Convert.ToInt32(proc_cboViewCount.SelectedItem);
                int nSplitUnitOfMeansureDetail = Convert.ToInt32(nudSplitUnitDetail.Value);

                SDMS.Data.ReportMode nPage = m_PageHome.FrmReport.ReportPage;
                if (nPage == Data.ReportMode.ActionIntrusion || nPage == Data.ReportMode.DetectIntrusion || nPage == Data.ReportMode.DetectIntrusionAnalyze || 
                    nPage == Data.ReportMode.ProcessIntrusion || nPage == Data.ReportMode.SMSIntrusion)
                    m_PageHome.FrmReport.LoadReportForIntrusion(strGroupName, strBuildingName, strFloorName, startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
                else
                    m_PageHome.FrmReport.LoadReport(strGroupName, strBuildingName, strFloorName, startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void GetFireBuildingInfo(ref string strBuildingGroup, ref string strBuilding, ref string strFloor)
        {
            strBuildingGroup = proc_cboBuildingGroup.Text.ToString();
            strBuilding = proc_cboBuilding.Text.ToString();
            strFloor = proc_cboFloor.Text.ToString();
        }

        public void GetPSMLocationInfo(ref string strLocation)
        {
            strLocation = cboDetectPSMBuilding.Text.ToString();
        }

        // Graph에서 한번에 표시할 수 있는 최대 데이터의 개수
        public int GetReportChartMaxItemCount()
        {
            int nViewCount = Convert.ToInt32(proc_cboViewCount.SelectedItem);
            return nViewCount;
        }

        private bool CheckValiedPriodDate(bool isPopMessage, Button btnStart, Button btnEnd, ref DateTime dtStart, ref DateTime dtEnd)
        {
            if (btnStart.Text == "시작 일" || btnEnd.Text == "끝 일")
            {
                if (isPopMessage == true)
                    MessageBox.Show("날짜를 입력해주세요");

                return false;
            }

            dtStart = DateTime.ParseExact(btnStart.Text, "yyyy-MM-dd", null);
            dtEnd = DateTime.ParseExact(btnEnd.Text, "yyyy-MM-dd", null);

            if (dtStart > dtEnd)
            {
                if (isPopMessage == true)
                    MessageBox.Show("시작 일이 더 클 수 없습니다.");

                return false;
            }

            return true;
        }

        public void RefreshReportDetectPSMAnalyze(bool isNotMassage = true)
        {
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;

            if (CheckValiedPriodDate(isNotMassage, btnDetectPSMStartDate, btnDetectPSMEndDate, ref dtStart, ref dtEnd) == false)
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                int nSplitUnitOfMeansure = cboDetectPSMSplitUnit.SelectedIndex;
                int nViewCount = Convert.ToInt32(cboDetectPSMViewCount.SelectedItem);
                int nSplitUnitOfMeansureDetail = Convert.ToInt32(nudDetectPSMSplitUnitDetail.Value);

                List<int> liEquipZoneIDs = new List<int>();

                if (cboDetectPSMBuilding.Text != "모든 시설")
                    liEquipZoneIDs.AddRange(PSMManager.Instance.GetTankLocationEquipZoneID(cboDetectPSMBuilding.Text));
                else
                    liEquipZoneIDs.Add(-1);

                m_PageHome.FrmReport.LoadReportForDetectPSMAnalyze(liEquipZoneIDs.ToArray(), dtStart, dtEnd, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount, cboDetectPSMBuilding.Text);

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void RefreshReportDetectPSM(bool isNotMassage = true)
        {
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;

            if (CheckValiedPriodDate(isNotMassage, btnDetectPSMStartDate, btnDetectPSMEndDate, ref dtStart, ref dtEnd) == false)
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                int nSplitUnitOfMeansure = cboDetectPSMSplitUnit.SelectedIndex;
                int nViewCount = Convert.ToInt32(cboDetectPSMViewCount.SelectedItem);
                int nSplitUnitOfMeansureDetail = Convert.ToInt32(nudDetectPSMSplitUnitDetail.Value);

                List<int> liEquipZoneIDs = new List<int>();

                if (cboDetectPSMBuilding.Text != "모든 시설")
                    liEquipZoneIDs.AddRange(PSMManager.Instance.GetTankLocationEquipZoneID(cboDetectPSMBuilding.Text));
                else
                    liEquipZoneIDs.Add(-1);

                m_PageHome.FrmReport.LoadReportForDetectPSM(liEquipZoneIDs.ToArray(), dtStart, dtEnd, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount, cboDetectPSMBuilding.Text);

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void RefreshReportNotOperationPSM(bool isNotMassage = true)
        {
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;

            if (CheckValiedPriodDate(isNotMassage, btnNotOperationPSMStartDate, btnNotOperationPSMEndDate, ref dtStart, ref dtEnd) == false)
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                List<int> liEquipZoneIDs = new List<int>();

                if (cboNotOperationPSMBuilding.Text != "모든 시설")
                    liEquipZoneIDs.AddRange(PSMManager.Instance.GetTankLocationEquipZoneID(cboNotOperationPSMBuilding.Text));
                else
                    liEquipZoneIDs.Add(-1);

                m_PageHome.FrmReport.LoadReportForNotOperationPSM(liEquipZoneIDs.ToArray(), dtStart, dtEnd, cboNotOperationPSMBuilding.Text);

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void RefreshReportSMSPSM(bool isNotMassage = true)
        {
            DateTime dtStart = DateTime.Now, dtEnd = DateTime.Now;

            if (CheckValiedPriodDate(isNotMassage, btnSMSPSMStartDate, btnSMSPSMEndDate, ref dtStart, ref dtEnd) == false)
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                List<int> liEquipZoneIDs = new List<int>();

                if (cboSMSPSMBuilding.Text != "모든 시설")
                    liEquipZoneIDs.AddRange(PSMManager.Instance.GetTankLocationEquipZoneID(cboSMSPSMBuilding.Text));
                else
                    liEquipZoneIDs.Add(-1);

                m_PageHome.FrmReport.LoadReportForSMSPSM(liEquipZoneIDs.ToArray(), dtStart, dtEnd, cboNotOperationPSMBuilding.Text);

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        public void RefreshReportIntrusion(bool isNotMassage = true)
        {
            if (proc_btnStartDate.Text == "시작 일" || proc_btnEndDate.Text == "끝 일")
            {
                MessageBox.Show("날짜를 입력해주세요");
                return;
            }

            DateTime startDate = DateTime.ParseExact(proc_btnStartDate.Text, "yyyy-MM-dd", null);
            DateTime EndDate = DateTime.ParseExact(proc_btnEndDate.Text, "yyyy-MM-dd", null);

            if (startDate > EndDate)
            {
                MessageBox.Show("시작 일이 더 클 수 없습니다.");
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;

                string strGroupName = proc_cboBuildingGroup.Text.ToString();
                string strBuildingName = proc_cboBuilding.Text.ToString();
                string strFloorName = proc_cboFloor.Text.ToString();

                int nSplitUnitOfMeansure = proc_cboSplitUnit.SelectedIndex;
                int nViewCount = Convert.ToInt32(proc_cboViewCount.SelectedItem);
                int nSplitUnitOfMeansureDetail = Convert.ToInt32(nudSplitUnitDetail.Value);

                m_PageHome.FrmReport.LoadReportForIntrusion(strGroupName, strBuildingName, strFloorName, startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void btnDetectPSMSelectZone_Click(object sender, EventArgs e)
        {
            SDMS.Data.ReportMode nPage = m_PageHome.FrmReport.ReportPage;

            if (nPage == SDMS.Data.ReportMode.DetectPSMAnalyze)
                RefreshReportDetectPSMAnalyze();
            else if (nPage == SDMS.Data.ReportMode.DetectPSM)
                RefreshReportDetectPSM();
        }

        private void btnNotOperationPSMSelectZone_Click(object sender, EventArgs e)
        {
            RefreshReportNotOperationPSM();
        }

        private void btnSMSPSMSelectZone_Click(object sender, EventArgs e)
        {
            RefreshReportSMSPSM();
        }



        public void ChangeZoneComboBox(Zone zone)
        {
            if (zone.Building == null)
                return;

            if (zone.Building.BuildingGroup == null)
                return;

            cboBuildingGroup.SelectedItem = zone.Building.BuildingGroup;
            cboBuilding.SelectedItem = zone.Building;

            foreach (Floor floor in cboFloor.Items)
            {
                if (floor.ToString() == zone.Floor.ToString())
                {
                    cboFloor.SelectedItem = null;
                    cboFloor.SelectedItem = floor;
                    break;
                }
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            MainFrame.Close();
        }



        public void ShowLeftThumbnail(bool bSituation = false)
        {
            //if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == false)
            //{

            //    SetLayerButtonVisible(btnLayerFire, labelFire, false);
            //    SetLayerButtonVisible(btnLayerSpringCooler, labelCooler, false);
            //    SetLayerButtonVisible(btnLayerPump, labelPump, false);
            //    SetLayerButtonVisible(btnLayerCCTV, labelCCTV, false);
            //    SetLayerButtonVisible(btnLayerLowCCTV, labelCCTVLow, false);
            //    SetLayerButtonVisible(btnLayerCCTVDisconnected, labelCCTVDisconnected, false);
            //    SetLayerButtonVisible(btnLayerFE, labelFE, false);
            //    SetLayerButtonVisible(btnLayerHD, labelHD, false);
            //    SetLayerButtonVisible(btnLayerFA, labelFA, false);
            //    SetLayerButtonVisible(btnLayerFR, labelFR, false);
            //    SetLayerButtonVisible(btnLayerBuildingText, labelBuildingText, false);
            //    SetLayerButtonVisible(btnSaveHome, labelSaveHome, false);

            //    //int nLineThick = 5;
            //    //int nFrmWidth = m_frmOutdoor.Size.Width;
            //    //int nFrmHeight = (panelLeft.Size.Height - nLineThick * 4) / 3;

            //    panelLeft.Visible = false;

            //    PageHome.ShowSituationCCTV(bSituation);

            //    m_isThumbnailMode = true;
            //    FormMain_Resize(null, null);
            //}
            //else
            {
                PageHome.ShowSituationCCTV(bSituation);
            }
        }

        public void ShowLeftLayer(bool bShowLeftPane = true)
        {
            //if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == false)
            //{
            //    if (!m_isThumbnailMode)
            //        return;

            //    if (bShowLeftPane == true)
            //    {
            //        SetLayerButtonVisible(btnLayerFire, labelFire, ControlInitPos.Instance.VisibleLayerFire);
            //        SetLayerButtonVisible(btnLayerSpringCooler, labelCooler, ControlInitPos.Instance.VisibleLayerSpringCooler);
            //        SetLayerButtonVisible(btnLayerPump, labelPump, ControlInitPos.Instance.VisibleLayerPump);
            //        SetLayerButtonVisible(btnLayerCCTV, labelCCTV, ControlInitPos.Instance.VisibleLayerCCTV);
            //        SetLayerButtonVisible(btnLayerLowCCTV, labelCCTVLow, ControlInitPos.Instance.VisibleLayerLowCCTV);
            //        SetLayerButtonVisible(btnLayerCCTVDisconnected, labelCCTVDisconnected, ControlInitPos.Instance.VisibleLayerCCTVDisconnected);
            //        SetLayerButtonVisible(btnLayerFE, labelFE, ControlInitPos.Instance.VisibleLayerFE);
            //        SetLayerButtonVisible(btnLayerHD, labelHD, ControlInitPos.Instance.VisibleLayerHD);
            //        SetLayerButtonVisible(btnLayerFA, labelFA, ControlInitPos.Instance.VisibleLayerFA);
            //        SetLayerButtonVisible(btnLayerFR, labelFR, ControlInitPos.Instance.VisibleLayerFR);
            //        SetLayerButtonVisible(btnLayerBuildingText, labelBuildingText, ControlInitPos.Instance.VisibleLayerBuildingText);
            //        SetLayerButtonVisible(btnSaveHome, labelSaveHome, true);

            //    }


            //    PageHome.ShowNormalCCTV();
            //    m_isThumbnailMode = false;

            //    if (bShowLeftPane == true)
            //    {
            //        FormMain_Resize(null, null);
            //    }
            //}
            //else
            {
                //PageHome.ShowNormalCCTV();
            }
        }

        private Zone SetDetectZoneName(int nHistoryID)
        {
            if (nHistoryID == -1)
            {
                mLabelZone.Text = "";
                return null;
            }

            int nSensorID = SensorHistoryManager.Instance.GetSensorID(nHistoryID);
            if (nSensorID != -1)
            {
                ISensor sensor = SensorManager.Instance.FindSensor(nSensorID);
                if (sensor != null)
                {
                    int nEquipZoneID = sensor.EquipZoneID;
                    EquipmentZone zone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);
                    if (zone != null)
                    {
                        if (zone.Building != null)
                        {
                            string text1 = zone.Building.BuildingGroup.BuildingGroupName;
                            string szZoneName = zone.LinkedZone.DisplayText;
                            mLabelZone.Text = text1 + "," + szZoneName;
                            return zone.LinkedZone;
                        }
                    }
                    else
                    {
                        mLabelZone.Text = "";
                    }
                }
                else
                {
                    mLabelZone.Text = "";
                }
            }
            else
            {
                mLabelZone.Text = "";
            }
            return null;
        }

        public void BeginNotifyProcess(ReactionLog log)
        {
            int nHistoryID = log.SensorHistoryID;

            if (log.ReactionType == (int)ReactionType.NOTIFY_FIRE)
            {
                if (log.Message.IndexOf("[훈련상황]") != -1)
                    StatusLableText = "[훈련]화재 발생";
                else
                    StatusLableText = "화재 발생";
            }
            else if (log.ReactionType == (int)ReactionType.NOTIFY_PSM)
            {
                if (log.Message.IndexOf("[훈련상황]") != -1)
                    StatusLableText = "[훈련]누출 발생";
                else
                    StatusLableText = "누출 발생";
            }

            else if (log.ReactionType == (int)ReactionType.NOTIFY_SECURITY)
            {
                if (log.Message.IndexOf("[훈련상황]") != -1)
                    StatusLableText = "[훈련]방범 상황";
                else
                    StatusLableText = "방범 상황";
            }
            ((RealTimeInfoPane)panelLog).TextColor = Color.Red;
            panelLog.Refresh();
            mLabelStatus.ForeColor = Color.Red;
            mLabelZone.ForeColor = Color.Red;

            Zone zone = SetDetectZoneName(nHistoryID);
            if (zone != null)
            {
            }
        }
     
        public void EndNotifyProcess(ReactionLog log)
        {
            int nHistoryID = log.SensorHistoryID;
            int nSensorID = SensorHistoryManager.Instance.GetSensorID(nHistoryID);
            if (nSensorID != -1)
            {
                ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
                ProcessManager.Instance.EndProcess(nSensorID);
            }
            SetNormalMode(log);
        }

        public void SetBuilingCollapseDetect( string strPosition, bool isRealMode)
        {
            string strMessage = "";

            strMessage = strPosition + " 건물이 붕괴되었습니다.";

            mLabelZone.Text = strPosition;

            if (isRealMode)
            {
                StatusLableText = "건물 붕괴";
                SetInfoMessage(DateTime.Now.ToLongTimeString() + " " + strMessage);
                //SetInfoMessage("[오후 02:00]" + " " + strMessage);
            }
            else
            {
                StatusLableText = "[훈련상황]건물 붕괴";
                SetInfoMessage(DateTime.Now.ToLongTimeString() + " [훈련상황]" + strMessage);
            }

            // 현재 뷰상태 저장
            m_PageHome.ContentForm.PushViewState(true);
            // 건물붕괴 상태임을 알림(재난 ComboBox에는 나타나지 않으므로...)
            m_PageHome.SetNoProcessDisaster(NoProcessDisaster.DisasterType.CollapseBuilding);

            mLabelStatus.ForeColor = Color.Orange;
            mLabelZone.ForeColor = Color.Orange;
            ((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
            panelLog.Refresh();
            panelStatus.Refresh();
        }

        public void FinishBuildingCollapse(string strBuildingName)
        {
            m_PageHome.SetNoProcessDisaster(NoProcessDisaster.DisasterType.None);
            SetNormalMode(0);
            m_PageHome.ContentForm.RestoreViewState();
        }

        public void SetEarthquakeDetect(int nIntensity, float fMagnitude, string strPosition, bool isRealMode)
        {

            EarthquakeSoundProcess.PlaySound();
            string strMessage = "";

            if (nIntensity > 0 && fMagnitude > 0.0f)
                strMessage = string.Format("규모 {0:F1}, 진도 {1}의 지진이 발생하였습니다.", fMagnitude, nIntensity);
            else if (nIntensity > 0)
                strMessage = string.Format("진도 {0}의 지진이 발생하였습니다.", nIntensity);
            else if (fMagnitude > 0.0f)
                strMessage = string.Format("규모 {0:F1}의 지진이 발생하였습니다.", fMagnitude);
            else
            {
                SetNormalMode(0);
                return;
            }

          //  mLabelZone.Text = strPosition;

            mLabelZone.Text = "";
            if (isRealMode)
            {
                StatusLableText = "지진 탐지";
                SetInfoMessage(DateTime.Now.ToLongTimeString() + " " + strMessage);
                //SetInfoMessage("[오전 10:30]"+ " " + strMessage);
            }
            else
            {
                StatusLableText = "[훈련상황]지진 탐지";
                SetInfoMessage(DateTime.Now.ToLongTimeString() + " [훈련상황]" + strMessage);
            }

            m_PageHome.SetNoProcessDisaster(NoProcessDisaster.DisasterType.Earthquake);

            mLabelStatus.ForeColor = Color.Orange;
            mLabelZone.ForeColor = Color.Orange;
            ((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
            panelLog.Refresh();
        }
        public void ShowCollapseBuilding(bool isReal, string buildingID)
        {

            String buildingName = ZoneManager.Instance.GetBuilding(buildingID).BuildingName;


            SetBuilingCollapseDetect(buildingName, isReal);
        }
        public void SetFireDetectMode(ReactionLog log)
        {
            bool bTestMode = false;
            int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("훈련상황]") != -1)
                StatusLableText = "[훈련]화재 탐지";
            else if (log.Message.IndexOf("테스트]") != -1)
            {
                StatusLableText = "[테스트]화재 탐지";
                bTestMode = true;
            }
            else
                StatusLableText = "화재 탐지";



            if (bTestMode == false)
            {
                mLabelStatus.ForeColor = Color.Orange;
                mLabelZone.ForeColor = Color.Orange;
                ((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
                panelLog.Refresh();
            }
            else
            {
                mLabelStatus.ForeColor = Color.GreenYellow;
                mLabelZone.ForeColor = Color.GreenYellow;
                ((RealTimeInfoPane)panelLog).TextColor = Color.GreenYellow;
                panelLog.Refresh();
            }
            
            //((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
            //panelLog.Refresh();
            //mLabelStatus.ForeColor = Color.Orange;
            //mLabelZone.ForeColor = Color.Orange;

            SetDetectZoneName(nHistoryID);
        }

        public void SetSecurityDetectMode(ReactionLog log)
        {
            bool bTestMode = false;
            int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("훈련상황]") != -1)
                StatusLableText = "[훈련]상황 탐지";
            else if (log.Message.IndexOf("테스트]") != -1)
            {
                StatusLableText = "[테스트]상황 탐지";
                bTestMode = true;
            }
            else
                StatusLableText = "상황 탐지";

            if (bTestMode == false)
            {
                mLabelStatus.ForeColor = Color.Orange;
                mLabelZone.ForeColor = Color.Orange;
                ((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
                panelLog.Refresh();
            }
            else
            {
                mLabelStatus.ForeColor = Color.GreenYellow;
                mLabelZone.ForeColor = Color.GreenYellow;
                ((RealTimeInfoPane)panelLog).TextColor = Color.GreenYellow;
                panelLog.Refresh();
            }
            SetDetectZoneName(nHistoryID);
        }

        public void SetPSMDetectMode(ReactionLog log)
        {
            bool bTestMode = false;
            int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("훈련상황]") != -1)
                StatusLableText = "[훈련]누출 탐지";
            else if (log.Message.IndexOf("테스트]") != -1)
            {
                StatusLableText = "[테스트]누출 탐지";
                bTestMode = true;
            }
            else
                StatusLableText = "누출 탐지";

           

            if (bTestMode == false)
            {
                mLabelStatus.ForeColor = Color.Orange;
                mLabelZone.ForeColor = Color.Orange;
                ((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
                panelLog.Refresh();
            }
            else
            {
                mLabelStatus.ForeColor = Color.GreenYellow;
                mLabelZone.ForeColor = Color.GreenYellow;
                ((RealTimeInfoPane)panelLog).TextColor = Color.GreenYellow;
                panelLog.Refresh();
            }
            
            SetDetectZoneName(nHistoryID);
        }

        public void SetRunSOPMode(ReactionLog log)
        {
            int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("[훈련상황]") != -1)
                StatusLableText = "[훈련]SOP 실행중";
            else
                StatusLableText = "SOP 실행중";

            ((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
            panelLog.Refresh();
            mLabelStatus.ForeColor = Color.Orange;
            mLabelZone.ForeColor = Color.Orange;

            SetDetectZoneName(nHistoryID);
        }

        public void SetRunNCancelSOPMode(ReactionLog log)
        {
            int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("[훈련상황]") != -1)
                StatusLableText = "[훈련]상황종료(SOP 실행취소)";
            else
                StatusLableText = "상황종료(SOP 실행취소)";

            ((RealTimeInfoPane)panelLog).TextColor = Color.White;
            panelLog.Refresh();
            mLabelStatus.ForeColor = Color.White;
            mLabelZone.ForeColor = Color.White;

            SetDetectZoneName(nHistoryID);
        }

        public void SetFinishSOPMode(ReactionLog log)
        {
            int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("[훈련상황]") != -1)
                StatusLableText = "[훈련]상황종료(SOP 종료)";
            else
                StatusLableText = "상황종료(SOP 종료)";
            ((RealTimeInfoPane)panelLog).TextColor = Color.White;
            panelLog.Refresh();
            mLabelStatus.ForeColor = Color.White;
            mLabelZone.ForeColor = Color.White;

            SetDetectZoneName(nHistoryID);
        }

        public void SetIgnoreSOPMode(ReactionLog log)
        {
            int nHistoryID = log.SensorHistoryID;
            if (log.Message.IndexOf("[훈련상황]") != -1)
                StatusLableText = "[훈련]상황종료";
            else
                StatusLableText = "상황종료";

            FormMain.Instance.ProxyMessenger.IgnoreSOP(nHistoryID);

            ((RealTimeInfoPane)panelLog).TextColor = Color.White;
            panelLog.Refresh();
            mLabelStatus.ForeColor = Color.White;
            mLabelZone.ForeColor = Color.White;

            SetDetectZoneName(nHistoryID);
        }

        public void SetNormalMode(int nHistoryID)
        {
            StatusLableText = "탐지 신호 없음";
            ((RealTimeInfoPane)panelLog).TextColor = Color.White;
            panelLog.Refresh();
            mLabelStatus.ForeColor = Color.White;
            mLabelZone.ForeColor = Color.White;
            SetDetectZoneName(-1);

            ((RealTimeInfoPane)panelLog).RealTimeInfo = "";
            ((RealTimeInfoPane)panelLog).DrawMovingText();

            PageHome.ContentForm.HideZoneVolume();
            PageHome.ContentForm.HideEvacCircle();
            // PageHome.ContentForm.HideAllPOIPopup();

            //if (ShowEquipZoneCCTV == true)
            //{
            //    if (PageHome.CCTVForm.ZoneTarget != null)
            //    {
            //        FormMain.Instance.ShowLeftLayer(false);
            //        PageHome.ShowDefaultCCTV();
            //    }
            //}

            btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar_Blue;
        }

        public void SetNormalMode(ReactionLog log)
        {
            int nHistoryID = log.SensorHistoryID;
            StatusLableText = "탐지 신호 없음";
            ((RealTimeInfoPane)panelLog).TextColor = Color.White;
            mLabelStatus.ForeColor = Color.White;
            mLabelZone.ForeColor = Color.White;
            SetDetectZoneName(-1);

            ((RealTimeInfoPane)panelLog).RealTimeInfo = "";
            ((RealTimeInfoPane)panelLog).DrawMovingText();

            PageHome.ContentForm.HideZoneVolume();
            PageHome.ContentForm.HideEvacCircle();

            //if (ShowEquipZoneCCTV == true)
            //{
            //    if (PageHome.CCTVForm.ZoneTarget != null)
            //    {
            //        FormMain.Instance.ShowLeftLayer(false);
            //        PageHome.ShowDefaultCCTV();
            //    }
            //}


            //PageHome.ContentForm.HideAllPOIPopup();

            btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar_Blue;
        }

        public void AddLogMessage(ReactionLog log)
        {
            if (log == null)
                return;
            ((RealTimeInfoPane)panelLog).RealTimeInfo = log.ToString();           
            mLabelLog.Text = "";
            mLabelLog.Tag = log;
            ((RealTimeInfoPane)panelLog).DrawMovingText();
        }

        private void btnFire_Click(object sender, EventArgs e)
        {

            if (btnFire.Text == RaiseManualFire)
            {
                if (MessageBox.Show("전직원에게 화재발생을 전파합니다.\r\n화재발생으로 신고하시겠습니까?", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                    return;

                btnFire.Enabled = false;

                Zone zone = PageBackstageHome.Instance.ContentForm.ManualClickZone;

                List<EquipmentZone> arEquipZone = ZoneManager.Instance.GetEquipmentZoneList(zone);

                //FormMain.Instance.SelectCCTVTab(false);
                //FormMain.Instance.PageHome.ShowBigCCTV(zone, true, true);

                if (zone != null)
                {
                    int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
                    SDMS.NetworkManager.Instance.SendMessage(1, TCP_ID.FIRE_DETECT_REPORT, 0, zone.ID, 0, nSOPGenUserID);
                    SendDetectMessageToSOPSimulator();
                }
            }
            else
            {
                if (CurrentSensorDetectProcess != null)
                {
                    if (CurrentSensorDetectProcess.ProcessType == ProcessType.PSMAlarm)
                    {
                        if (MessageBox.Show("현장의 이상유무 확인하셨나요?\r\n현장 신호를 복구하시겠습니까?", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                            return;
                    }
                    else if (CurrentSensorDetectProcess.ProcessType == ProcessType.PSMAlarm)
                    {

                        if (MessageBox.Show("화재상황을 종료하시겠습니까?", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                            return;
                    }

                    btnFire.Enabled = false;

                    int nHistoryID = CurrentSensorDetectProcess.SensorHistoryID;
                    int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
                    SDMS.NetworkManager.Instance.SendMessage(1, TCP_ID.CLEAR_DETECT_REPORT, nHistoryID, nSOPGenUserID);
                }
            }
        }


        //공백 제거
        private string subGap(string _str)
        {
            int num = 0;//중간 띄어쓰기 위치
            string tmp = _str;
            while (tmp.IndexOf(" ") > 0)
            {
                num = tmp.IndexOf(" ");
                string tmp1 = tmp.Substring(0, num);

                tmp1 += "_" + tmp.Substring(num + 1);
                tmp = tmp1;
            }
            return tmp;
        }

        private void btnFire_KeyDown(object sender, KeyEventArgs e)
        {
            FormMain.Instance.EnableFireReportBtn(false);
        }

        public void SendDetectMessageToSOPSimulator()
        {
            Debug.WriteLine("Run Simulator");
            bool bRun = FormSMSConfig.ReadRunSimulator();
            if (bRun == true)
            {
                if (m_proxyMessenger != null)
                    m_proxyMessenger.RunSOPSimulator();
            }
        }

        private void RunStartProcess(string strFileName, string args)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = strFileName;
            startInfo.WorkingDirectory = Application.StartupPath;
            startInfo.ErrorDialog = true;
            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(strFileName, args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool RunCheckProcess(string strProcessName)
        {
            int nIndex2 = strProcessName.LastIndexOf('.');
            int nIndex1 = strProcessName.LastIndexOf('\\');

            if (nIndex1 < 0 || nIndex2 < 0)
                return false;

            strProcessName = strProcessName.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                    return true;

                /*if (process.ProcessName  ==(strProcessName + ".vshost"))
                    return true;*/
            }

            return false;
        }

        private void cmbFireDetect_SelectionChangeCommitted(object sender, EventArgs e)
        {
            m_bSelectCombo = true;
            ProcessIF process = (ProcessIF)cmbFireDetect.SelectedItem;
            CountUpNotice(process);
            
            if (process != null)
            {
                //if (process == CurrentSensorDetectProcess)
                //    return;

                bool bSelected = process.Select();
                if (bSelected)
                {
                    int nSensorID = process.DetectSensorID;
                    int nHistoryID = process.SensorHistoryID;

                    if (process.LastLog.ReactionType == (int)(ReactionType.BEGIN_STATUS) || process.LastLog.ReactionType == (int)(ReactionType.BEGIN_PSM_STATUS) ||
                        process.LastLog.ReactionType == (int)(ReactionType.BEGIN_S1ACCESS_STATUS) || process.LastLog.ReactionType == (int)(ReactionType.BEGIN_S1SVMS_STATUS) ||
                        process.LastLog.ReactionType == (int)(ReactionType.CHANGE_PSM_ALARM_DEPTH))                        
                    {
                        SeletCaseData form = new SeletCaseData(process.ProcessType, process.TargetSensor, nHistoryID, process.ShowOpenSOP, process.DetectTime);
                        ConfirmDialogManager.Instance.AddDialogFirst(form);
                        ConfirmDialogManager.Instance.ShowDialogNext();
                    }
                    else
                    {
                        ConfirmDialogManager.Instance.CloseAllDialog();
                    }

                    ReactionLogManager.Instance.ProcessLog(process.LastLog, true);

                    if (process.TargetZone != null)
                    {
                        Zone zone = process.TargetZone.LinkedZone;
                        if (zone != null)
                        {
                            int nZoneId = zone.ID;
                            int nMode = nSensorID - 50000;
                            // 수동 신고임
                            if (nMode > 0)
                            {
                                btnFire.Text = CloseManualFire;
                                btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar;
                                btnFire.Enabled = true;
                            }
                            else
                            {
                                btnFire.Text = RaiseManualFire;
                                btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar_Blue;
                                btnFire.Enabled = false;
                            }
                        }
                    }
                }
            }
        }

        private ProcessIF mTempProcessCurrent = null;
        private bool m_bSelectCombo = false;
        private void cmbFireDetect_DropDown(object sender, EventArgs e)
        {
            m_bSelectCombo = false;
            mTempProcessCurrent = CurrentSensorDetectProcess;
        }

        private void cmbFireDetect_DropDownClosed(object sender, EventArgs e)
        {
            if (m_bSelectCombo == false)
            {
                if (mTempProcessCurrent != null)
                    cmbFireDetect.SelectedItem = mTempProcessCurrent;
            }

            cmbSensorDetectTooltip.Hide(cmbFireDetect);
        }

        public void RemoveSensorDetect(ProcessIF process, bool bNextSelect = true)
        {
            int nCurIdx = cmbFireDetect.SelectedIndex;

            cmbFireDetect.Items.Remove(process);
            Debug.WriteLine(process);
            int nCount = cmbFireDetect.Items.Count;
            DlgSelectCase.Instance.DetectFireCount = nCount;

            if (nCount > 0 && bNextSelect == true)
            {
                cmbFireDetect.SelectedIndex = (nCount - 1);
                ProcessIF processSelect = (ProcessIF)cmbFireDetect.SelectedItem;
                if (processSelect != null)
                {
                    bool bSelected = processSelect.Select();
                    if (bSelected)
                    {
                        PageHome.SetDetectSensor(processSelect.TargetSensor);
                        ReactionLogManager.Instance.ProcessLog(processSelect.LastLog, true);
                    }

                    //if (!ShowEquipZoneCCTV)
                    {
                        if (processSelect != null && processSelect.LastLog != null)
                        {
                            if (processSelect.LastLog.ReactionType == (int)(ReactionType.BEGIN_STATUS) || processSelect.LastLog.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS)
                            {
                                int nSensorID = processSelect.DetectSensorID;
                                int nHistoryID = processSelect.SensorHistoryID;
                                SeletCaseData form = new SeletCaseData(processSelect.ProcessType, processSelect.TargetSensor, nHistoryID, processSelect.ShowOpenSOP, processSelect.DetectTime);
                                ConfirmDialogManager.Instance.AddDialogFirst(form);
                                SeletCaseData form2 = ConfirmDialogManager.Instance.ShowDialogNext();
                                if (form2 != null)
                                {
                                    int nID = form2.SensorHistoryID;
                                    int nSensorID2 = form2.Sensor.ID;
                                    FormMain.Instance.SelectSensorDetectProcess(nID, nSensorID2);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                SetNormalMode(0);

                //if (ShowEquipZoneCCTV == false)
                {
                    FormMain.Instance.PageHome.ContentForm.RestoreViewState();
                }
                //OnClickToolBarButton(m_PaneBtnHome.BtnMainHome, null);				
            }
            process.HideCCTV();

            RemoveNotice(process);
        }

        public void RemoveSensorDetect(int nSesnorZoneID)
        {
            ProcessIF process = ProcessManager.Instance.GetProcess(nSesnorZoneID);
            if( process != null)
            {
                ProcessManager.Instance.RemoveProcess(process);
                RemoveSensorDetect(process);
            }
        }


        public void RemoveAllFireSensorDetect()
        {
            List<ProcessIF> rList = ProcessManager.Instance.GetAllFireSignalProcess();
            for(int i = 0 ; i < rList.Count ; i++ )
            {
                ProcessIF process = rList[i];
                ProcessManager.Instance.RemoveProcess(process);

                RemoveSensorDetect(process, false);
            }
            SelectLastFireDectectProcess();
        }

        public void RemoveAllPSMSensorDetect()
        {
            List<ProcessIF> rList = ProcessManager.Instance.GetAllPSMSignalProcess();
            for (int i = 0; i < rList.Count; i++)
            {
                ProcessIF process = rList[i];
                ProcessManager.Instance.RemoveProcess(process);
                RemoveSensorDetect(process, false);
            }
            SelectLastFireDectectProcess();
        }

        public void AddSensorDectect(ProcessIF process, bool bAddSelect = true, bool bCallSelect = true)
        {
            int nCurIdx = cmbFireDetect.SelectedIndex;

            if (!cmbFireDetect.Items.Contains(process))
            {
                int nIdx = cmbFireDetect.Items.Add(process);
                
                if (bAddSelect == true)
                {
                    cmbFireDetect.SelectedIndex = (nIdx);

                    if (bCallSelect == true)
                    {
                        ProcessIF processSelect = (ProcessIF)cmbFireDetect.SelectedItem;
                        if (processSelect != null)
                        {
                            bool bSelected = processSelect.Select();
                            if (bSelected)
                            {
                                ReactionLogManager.Instance.ProcessLog(processSelect.LastLog, true);
                            }
                        }
                    }

                }
                else
                {
                    if (nCurIdx != -1)
                        cmbFireDetect.SelectedIndex = nCurIdx;
                }

                AddNotice(process);
            }

            DlgSelectCase.Instance.DetectFireCount = cmbFireDetect.Items.Count;
        }

        #region PanelLeft 알림 함수
        private void AddNotice(ProcessIF process)
        { 
            m_PanelBtnNotice.m_noticeListItem.Add(process);
            m_PanelBtnNotice.RefreshList();
            ChangeNotice(process);

            SetCountNotice();

            if (m_PanelBtnNotice.m_noticeListItem.Count > 1 && !m_PanelBtnNotice.Visible)
            {
                btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Checked;
                Point pt = new Point(btnLayerNotice.Location.X + btnLayerNotice.Width + 4, btnLayerNotice.Location.Y + btnLayerNotice.Height);
                Point ptScr = panelMiddle.PointToScreen(pt);
                m_PanelBtnNotice.Location = ptScr;// PointToClient(ptScr);
                m_PanelBtnNotice.Visible = true;
            }
        }
        private void RemoveNotice(ProcessIF process)
        { 
            m_PanelBtnNotice.m_noticeListItem.Remove(process);
            m_PanelBtnNotice.m_dicCountUpNoticeList.Remove(process);
            m_PanelBtnNotice.RefreshList();

            SetCountNotice();

            if (m_PanelBtnNotice.m_noticeListItem.Count == 0)
            {
                btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Normal;
                m_PanelBtnNotice.Visible = false;
            }
        }
        private void ClearNotice()
        { 
            m_PanelBtnNotice.m_noticeListItem.Clear();
            m_PanelBtnNotice.m_dicCountUpNoticeList.Clear(); 
            m_PanelBtnNotice.Controls.Clear();

            SetCountNotice();

            m_PanelBtnNotice.Visible = false;
            btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Normal;
        }
        private void ChangeNotice(ProcessIF process)
        {
            m_PanelBtnNotice.m_selectedItem = process;
            m_PanelBtnNotice.CountUpNotice(process);
            m_PanelBtnNotice.ChangeNotice();
        } 
        private void CountUpNotice(ProcessIF process)
        {
            m_PanelBtnNotice.CountUpNotice(process);
        }

        private void btnLayerNotice_Click(object sender, EventArgs e)
        {
            if (m_PanelBtnNotice.Visible == true || m_PanelBtnNotice.m_noticeListItem.Count == 0)
            {
                m_PanelBtnNotice.Visible = false;
                btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Normal;
                return;
            }

            btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Checked;

            Point pt = new Point(btnLayerNotice.Location.X + btnLayerNotice.Width + 4, btnLayerNotice.Location.Y + btnLayerNotice.Height);
            Point ptScr = panelMiddle.PointToScreen(pt);
            m_PanelBtnNotice.Location = ptScr; 
            m_PanelBtnNotice.Show(this);
            return;
        }

        void m_PaneBtnNotice_chgSensorDectect(ProcessIF process)
        {
            cmbFireDetect.SelectedItem = process;
            cmbFireDetect_SelectionChangeCommitted(this, new EventArgs());
        }
        private void SetCountNotice()
        {
            if (btnLayerNotice.Controls != null)
            {
                if (btnLayerNotice.Controls[0] is PictureBox)
                {
                    PictureBox pic = btnLayerNotice.Controls[0] as PictureBox;
                    if (pic.Controls != null && pic.Controls.Count > 0)
                    {
                        if (pic.Controls[0] is Label)
                        {
                            Label label = pic.Controls[0] as Label;
                            label.Text = m_PanelBtnNotice.m_noticeListItem.Count.ToString();

                            if (m_PanelBtnNotice.m_noticeListItem.Count == 0) 
                                pic.Visible = false; 
                            else if (m_PanelBtnNotice.m_noticeListItem.Count >= 10)
                            {
                                pic.Visible = true;
                                label.Location = new Point(0, 1);
                                label.Font = new Font("나눔바른고딕", 6.5F);
                            }
                            else
                            {
                                pic.Visible = true;
                                label.Location = new Point(1, 1);
                                label.Font = new Font("나눔바른고딕", 8F);
                            } 
                        }
                    }
                }
            }  
        }
        #endregion

        private void btnLayerLowCCTV_Click(object sender, EventArgs e)
        {
        }

        private void btnLayerLowCCTV_Click_1(object sender, EventArgs e)
        {
        }

        private void checkBoxEquipZoneCCTV_CheckedChanged(object sender, EventArgs e)
        {
            SetEquipZoneCCTV(checkBoxEquipZoneCCTV.Checked);
        }

        public void SetEquipZoneCCTV(bool enable)
        {
            if (m_isVisibleEquipZoneCCTV == enable)
                return;

            m_isVisibleEquipZoneCCTV = enable;

            ResizeEquipZoneCCTVControl(enable, true);
        }

        private void ResizeEquipZoneCCTVControl(bool enable, bool showCCTV)
        {
            int nGap = cboBuildingGroup.Location.X - (labelSelectZone.Location.X + labelSelectZone.Size.Width);

            cboEquipZone.Size = new Size(323, cboEquipZone.Size.Height);

            if (enable)
            {
                cboEquipZone.Visible = true;
                labelFireDetect.Visible = false;
                cmbFireDetect.Visible = false;

                cboEquipZone.Location = new Point(btnSelectZone.Location.X - nGap - cboEquipZone.Size.Width, cboFloor.Location.Y);
                cboFloor.Location = new Point(cboEquipZone.Location.X - nGap - cboFloor.Size.Width, cboFloor.Location.Y);
            }
            else
            {
                cboEquipZone.Visible = false;
                labelFireDetect.Visible = true;
                cmbFireDetect.Visible = true;

                cboFloor.Location = new Point(btnSelectZone.Location.X - nGap - cboFloor.Size.Width, cboFloor.Location.Y);
            }

            cboBuilding.Location = new Point(cboFloor.Location.X - nGap - cboBuilding.Size.Width, cboBuilding.Location.Y);
            cboBuildingGroup.Location = new Point(cboBuilding.Location.X - nGap - cboBuildingGroup.Size.Width, cboBuildingGroup.Location.Y);
            labelSelectZone.Location = new Point(cboBuildingGroup.Location.X - nGap - labelSelectZone.Size.Width, labelSelectZone.Location.Y);

            if (enable && showCCTV)
                PageHome.ShowEquipZoneCCTVs(CurrentEquipZone.ID);
        }

        private void cboFloor_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSelectedIndex = cboFloor.SelectedIndex;
            if (nSelectedIndex < 0)
                return;

            cboEquipZone.Items.Clear();

            Object obj = cboFloor.Items[nSelectedIndex];
            Type type = obj.GetType();

            Zone zone = null;

            if (type == typeof(Floor))
            {
                Building building = (Building)cboBuilding.Items[cboBuilding.SelectedIndex];
                Floor floor = (Floor)obj;
                zone = ZoneManager.Instance.GetZone(building.BuildingID, floor.FloorIndex);
            }

            if (zone == null || zone.ID <= 0)
                return;

            List<EquipmentZone> arrEquipZones = ZoneManager.Instance.GetEquipmentZoneList(zone);
            if (arrEquipZones == null)
                return;

            foreach (EquipmentZone equipZone in arrEquipZones)
            {
                cboEquipZone.Items.Add(equipZone);
            }

            if (cboEquipZone.Items.Count > 0)
            {
                if (cmbFireDetect.SelectedItem != null)
                {
                    ProcessIF process = (ProcessIF)cmbFireDetect.SelectedItem;

                    if (cboEquipZone.Items.Contains(process.TargetZone))
                    {
                        cboEquipZone.SelectedItem = process.TargetZone;
                    }
                    else
                    {
                        cboEquipZone.SelectedIndex = 0;
                    }
                }
                else
                {
                    cboEquipZone.SelectedIndex = 0;
                }
            }
        }

        private void cboEquipZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            PageHome.ShowEquipZoneCCTVs(CurrentEquipZone.ID);
        }

        public ArrayList GetActionContorl()
        {
            //react_cboSearchType.Visible = true;
            //react_cboSearchTypeIntrusion.Visible = false;
            //lblFireSelect.Visible = true;
            //lblIntrusionSelect.Visible = false;
            ArrayList arContorl = new ArrayList();
            arContorl.Add(react_btnEndDate);
            arContorl.Add(react_btnStartDate);
            arContorl.Add(react_cboSearchType);
            arContorl.Add(cboFireSelect);
            arContorl.Add(react_cboEndTime);
            arContorl.Add(react_cboStartTime);
            arContorl.Add(btnReactionSelectDisaster);
            return arContorl;
        }

        public ArrayList GetActionIntrusionContorl()
        {
            //react_cboSearchType.Visible = false;
            //react_cboSearchTypeIntrusion.Visible = true;
            //lblFireSelect.Visible = false;
            //lblIntrusionSelect.Visible = true;
            ArrayList arContorl = new ArrayList();
            arContorl.Add(react_btnEndDate);
            arContorl.Add(react_btnStartDate);
            arContorl.Add(react_cboSearchTypeIntrusion);
            arContorl.Add(cboActionIntrusionSelect);
            arContorl.Add(react_cboEndTime);
            arContorl.Add(react_cboStartTime);
            arContorl.Add(btnReactionIntrusionSelectDisaster);
            return arContorl;
        }

        public ArrayList GetActionPSMContorl()
        {
            ArrayList arContorl = new ArrayList();
            arContorl.Add(btnActionPSMEndDate);
            arContorl.Add(btnActionPSMStartDate);
            arContorl.Add(cboActionPSMSearchType);
            arContorl.Add(cboActionPSMSelect);
            arContorl.Add(cboActionPSMEndTime);
            arContorl.Add(cboActionPSMStartTime);
            arContorl.Add(btnReactionPSMSelectDisaster);
            return arContorl;
        }


        private bool m_bLoadingList = false;
        private void btnShowCCTVList_Click(object sender, EventArgs e)
        {
            if (m_frmCCTVList != null && m_frmCCTVList.IsHandleCreated == true && m_frmCCTVList.Visible == true)
                return;

            if (m_bLoadingList == false)
            {
                m_bLoadingList = true;

                m_frmCCTVList = new FormCCTVList();
                m_frmCCTVList.Text = "CCTV 리스트";
                m_frmCCTVList.Show(this);
            }

            m_bLoadingList = false;

        }

        private void btnSensorMonitor_Click(object sender, EventArgs e)
        {
            FormReciverState frm = new FormReciverState();
            PageBackstageHome.ShowTranslucentForm(frm, 300, 200, frm.Size.Width, frm.Size.Height, ID.ID_VIEW_SENSOR_MONITOR);
        }

        private int nReciveReadCount = 0;

        private void m_CheckReciver_Tick(object sender, EventArgs e)
        {
            nReciveReadCount++;
            if (nReciveReadCount == 7)
            {
                ReciverManager.Instance.LoadReciverList();
                nReciveReadCount = 0;
            }

            try
            {

                ArrayList arReciverList = ReciverManager.Instance.GetReciverList();

                bool bFailConnection = false;
                int nFailCount = 0;
                string szText = "수신반 : ";
                foreach (Reciver reciver in arReciverList)
                {
                    if (reciver.State <= 0)
                    {
                        bFailConnection = true;
                        nFailCount++;
                    }
                }

                if (bFailConnection == true)
                {
                    szText += string.Format("통신 상태 불량 ({0})", nFailCount);
                    labelSensorMonitor.Text = szText;
                    labelSensorMonitor.ForeColor = Color.Red;
                }
                else
                {
                    szText += "통신 상태 양호";
                    labelSensorMonitor.Text = szText;
                    labelSensorMonitor.ForeColor = Color.Green;
                }
            }
            catch (Exception)
            {
            }
        } 

        private void cmbFireDetect_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessIF process = (ProcessIF)cmbFireDetect.SelectedItem;            
            if (process != null)
            {
                if (process.TargetZone != null)
                {
                    int nSensorID = process.DetectSensorID;
                    int nHistoryID = process.SensorHistoryID;
                    Zone zone = process.TargetZone.LinkedZone;
                    if (zone != null)
                    {
                        int nZoneId = zone.ID;
                        int nMode = nSensorID - 50000;
                        // 수동 신고임
                        if (nMode > 0)
                        {
                            btnFire.Text = CloseManualFire;
                            btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar;
                            btnFire.Enabled = true;
                        }
                        else
                        {
                            btnFire.Text = RaiseManualFire;
                            btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar_Blue;
                            btnFire.Enabled = false;
                        }

                        CountUpNotice(process);
                        ChangeNotice(process);                        
                    }
                }
            }
        }

        private void btnSaveHome_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            if (m_PaneBtnSaveHome.Visible == true)
            {
                m_PaneBtnSaveHome.Visible = false;
                return;
            }

            Point pt = new Point(button.Location.X + button.Width + 4, button.Location.Y + button.Height);
            Point ptScr = panelMiddle.PointToScreen(pt);
            m_PaneBtnSaveHome.Location = ptScr;// PointToClient(ptScr);
            m_PaneBtnSaveHome.Show(this);
            return;
        }

        private void btnSaveHomeSub_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)PageBackstageHome.Instance.ContentForm.OutdoorView;
            if (view != null)
            {
                if (button == m_PaneBtnSaveHome.BtnMainHome)
                    view.SaveHomeView("Main");
                if (button == m_PaneBtnSaveHome.Btn14Home)
                    view.SaveHomeView("14");
                if (button == m_PaneBtnSaveHome.Btn56Home)
                    view.SaveHomeView("56");
                if (button == m_PaneBtnSaveHome.BtnCoalHome)
                    view.SaveHomeView("Coal");
            }

            m_PaneBtnSaveHome.Visible = false;
        }


        private void toolStripMenuItemBig_Click(object sender, EventArgs e)
        {
            SetCCTVMode(CCTVMode.CCTV_ONLY);

            ResizePanels();

            //if (PageHome.CCTVForm != null)
            //{
            //    if (PageBackstageHome.TranslucentForm.InnerForm != null)
            //    {
            //        if (PageBackstageHome.TranslucentForm.InnerForm.GetType() == typeof(Form4CCTV))
            //        {
            //            int x = PageHome.CCTVForm.Location.X;
            //            int y = PageHome.CCTVForm.Location.Y;
            //            int width = PageHome.CCTVForm.Size.Width;
            //            int height = PageHome.CCTVForm.Size.Height;
            //            PageBackstageHome.TranslucentForm.ResizeInner(x, y, width, height);
            //        }
            //    }
            //}
        }

        private void toolStripMenuItemNormal_Click(object sender, EventArgs e)
        {
            SetCCTVMode(CCTVMode.NORMAL);
            ResizePanels();

            //if (PageHome.CCTVForm != null)
            //{
            //    if (PageBackstageHome.TranslucentForm.InnerForm != null)
            //    {
            //        if (PageBackstageHome.TranslucentForm.InnerForm.GetType() == typeof(Form4CCTV))
            //        {
            //            int x = PageHome.CCTVForm.Location.X;
            //            int y = PageHome.CCTVForm.Location.Y;
            //            int width = PageHome.CCTVForm.Size.Width;
            //            int height = PageHome.CCTVForm.Size.Height;
            //            PageBackstageHome.TranslucentForm.ResizeInner(x, y, width, height);
            //        }
            //    }
            //}
        }

        private void panelMiddle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip1.Show(panelMiddle, e.X, e.Y);
            }
        }

        public CCTVMode GetCCTVMode()
        {
            return m_cctvMode;
        }

        public void SetCCTVMode(CCTVMode mode)
        {
            if (m_cctvMode == mode)
                return;

            m_cctvMode = mode;

            //if (m_isThumbnailMode)
            //{
            //    m_PageHome.SetCCTVMode(mode);
            //}
        }

        public void Update3DView()
        {
            if (m_PageHome != null)
                this.m_PageHome.Invalidate3DView(false);
        }

        private void DatePickerStart2_Leave(object sender, EventArgs e)
        {
            DatePickerStart2.Visible = false;
        }

        private void DatePickerEnd_Leave(object sender, EventArgs e)
        {
            DatePickerEnd.Visible = false;
        }

        private void DatePickerEnd2_Leave(object sender, EventArgs e)
        {
            DatePickerEnd2.Visible = false;
        }

        public void PerformClickSelectReport()
        {
            proc_btnSelectZone.PerformClick();
        }

        // Return 값 : true이면 기후정보 표시 창을 사용한다.
        private bool GetWeatherInfoOption()
        {
            string strPropertyName = "ShowWeatherInfo";

            string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='" + strPropertyName + "' AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            int nOption = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            if (nOption == 1)
                return true;

            return false;
        }

        // Return 값 : true이면 기후정보 표시 창을 사용한다.
        private void ReadLeftBarThumbnailOption()
        {
            string strPropertyName = "LeftThumbnailOption";

            string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='" + strPropertyName + "' AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strOption = DBUtility.WebDBManager.GetStringField(arrResult[0], "");

            if (strOption == null || strOption == "null")
                return;

            string[] arrTokens = strOption.Split(',');

            foreach (string strToken in arrTokens)
            {
                string strData = strToken.Trim();

                int nIndex1 = strData.LastIndexOf('(');
                int nIndex2 = strData.LastIndexOf(')');

                if (nIndex1 < 0 || nIndex2 < 0 || nIndex2 <= nIndex1)
                    continue;

                string strName = strData.Substring(0, nIndex1).Trim();
                string strShowHide = strData.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();

                if (strName == "화재탐지")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerFire = true;
                    else
                        ControlInitPos.Instance.VisibleLayerFire = false;
                }
                else if (strName == "스프링쿨러")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerSpringCooler = true;
                    else
                        ControlInitPos.Instance.VisibleLayerSpringCooler = false;
                }
                else if (strName == "펌프")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerPump = true;
                    else
                        ControlInitPos.Instance.VisibleLayerPump = false;
                }
                else if (strName == "CCTV")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerCCTV = true;
                    else
                        ControlInitPos.Instance.VisibleLayerCCTV = false;
                }
                else if (strName == "CCTV_L")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerLowCCTV = true;
                    else
                        ControlInitPos.Instance.VisibleLayerLowCCTV = false;
                }
                else if (strName == "CCTV_X")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerCCTVDisconnected = true;
                    else
                        ControlInitPos.Instance.VisibleLayerCCTVDisconnected = false;
                }
                else if (strName == "소화기")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerFE = true;
                    else
                        ControlInitPos.Instance.VisibleLayerFE = false;
                }
                else if (strName == "소화전")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerHD = true;
                    else
                        ControlInitPos.Instance.VisibleLayerHD = false;
                }
                else if (strName == "발신기")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerFA = true;
                    else
                        ControlInitPos.Instance.VisibleLayerFA = false;
                }
                else if (strName == "수신기")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerFR = true;
                    else
                        ControlInitPos.Instance.VisibleLayerFR = false;
                }
                else if (strName == "빌딩Text")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerBuildingText = true;
                    else
                        ControlInitPos.Instance.VisibleLayerBuildingText = false;
                }
                else if (strName == "알림")
                {
                    if (strShowHide == "1")
                        ControlInitPos.Instance.VisibleLayerNotice = true;
                    else
                        ControlInitPos.Instance.VisibleLayerNotice = false;
                }
            }
        }

        public void ShowWeatherInfo()
        {
            if (m_frmWeather.Visible == false)
            {
                m_frmWeather.Show(this);
            }
        }

        public void HideWeatherInfo()
        {
            m_frmWeather.Hide();
        }

        public void UpdateWeatherInfo()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                m_frmWeather.UpdateData(m_dbMgr, UnE.SOP.ProxySOP.Instance.SiteID);
            });
        }

        private void btnHome_Leave(object sender, EventArgs e)
        {
            m_PaneBtnHome.Visible = false;
        }

        private void btnSaveHome_Leave(object sender, EventArgs e)
        {
            m_PaneBtnSaveHome.Visible = false;
        } 

        private Process m_pSensorTester = null;
        private void btnSimulator_Click(object sender, EventArgs e)
        {
            if (m_PanelBtnSimulator.Visible == true)
            {
                m_PanelBtnSimulator.Visible = false;
                return;
            }

            string strSQL = String.Format("SELECT PropertyValue FROM OptionSDMS WHERE PropertyName = 'TestSimulator' AND SiteID = {0}", UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            string strOptionString = string.Empty;

            if (arrResult != null)
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    strOptionString = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                }
            }

            if (String.IsNullOrWhiteSpace(strOptionString))
                return;

            strSQL = String.Format("SELECT PropertyValue FROM OptionSDMS WHERE PropertyName = '{0}' AND SiteID = {1}", strOptionString, UnE.SOP.ProxySOP.Instance.SiteID);
            arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            strOptionString = string.Empty;

            if (arrResult != null)
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    strOptionString = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                }
            }

            if (String.IsNullOrWhiteSpace(strOptionString))
                return;

            string strPgName = string.Empty;
            string strExt = string.Empty;

            // 바로 실행
            if (strOptionString.Split(',')[0].Trim() == "Run")
            {
                strPgName = strOptionString.Split(',')[1].Split('.')[0];
                strExt = strOptionString.Split(',')[1].Split('.')[1];

                ExecuteManager mgr = new ExecuteManager();
                m_pSensorTester = mgr.Run(strPgName, strExt);
            }
            // 옵션 선택에 따른 차별 실행
            else if (strOptionString.Split(',')[0].Trim() == "Select")
            {
                CreatePanelSimulator(strOptionString.Split(','));
            }
        }

        private void CreatePanelSimulator(string[] strOptionValue)
        {
            if (strOptionValue.Length % 2 == 0) return;


            m_PanelBtnSimulator.ClearButtons();

            int nSimulatorCount = (strOptionValue.Length - 1) / 2;

            for (int i = 0; i < nSimulatorCount; i++)
            {
                m_PanelBtnSimulator.AddButton(strOptionValue[i + 1], strOptionValue[i + 1 + nSimulatorCount]);
            }

            Point pt = new Point(m_toolbar.Location.X + m_toolbar.GetButton(ID.ID_VIEW_SIMULATOR).Location.X, m_toolbar.Location.Y + m_toolbar.Size.Height);

            // Toolbar가 화면 아래쪽에 있으면 m_PaneBtnHome을 Toolbar 위쪽에 띄운다.
            if (pt.Y + m_PaneBtnHome.Size.Height > FormFrame.Instance.Location.Y + FormFrame.Instance.Size.Height)
                pt = new Point(m_toolbar.Location.X + m_toolbar.GetButton(ID.ID_VIEW_SIMULATOR).Location.X, m_toolbar.Location.Y - m_PaneBtnHome.Size.Height);

            m_PanelBtnSimulator.Location = pt;
            m_PanelBtnSimulator.Show(this);
        }

        private void RunSimulator(object sender, EventArgs e)
        {
            m_PanelBtnSimulator.Hide();

            Button btn = sender as Button;
            if (btn.Tag == null)
                return;


            string strSQL = String.Format("SELECT PropertyValue FROM OptionSDMS WHERE PropertyName = '{0}' AND SiteID = {1}", btn.Tag, UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            string strOptionString = string.Empty;

            if (arrResult != null)
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    strOptionString = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                }
            }

            if (String.IsNullOrWhiteSpace(strOptionString))
                return;

            string strPgName = string.Empty;
            string strExt = string.Empty;

            // 바로 실행
            if (strOptionString.Split(',')[0].Trim() == "Run")
            {
                strPgName = strOptionString.Split(',')[1].Split('.')[0];
                strExt = strOptionString.Split(',')[1].Split('.')[1];

                ExecuteManager mgr = new ExecuteManager();
                m_pSensorTester = mgr.Run(strPgName, strExt);
            }
            // 옵션 선택에 따른 차별 실행
            else if (strOptionString.Split(',')[0].Trim() == "Select")
            {
                CreatePanelSimulator(strOptionString.Split(','));
            }
        }

        private void btnSOP_Click(object sender, EventArgs e)
        {
            if (m_proxyMessenger == null)
                return;

            if (!m_proxyMessenger.IsVisibleSOPSimulator())
                m_proxyMessenger.ShowSOPSimulator();
            else
                m_proxyMessenger.HideSOPSimulator();
        }

        private void btnBulletin_Click(object sender, EventArgs e)
        {
            
            ToggleSOPBulletin();
        }

        private void btnMissionStatus_Click(object sender, EventArgs e)
        {
            if (m_proxyMessenger == null)
                return;

            if (!m_proxyMessenger.IsVisibleMissionStatus())
                m_proxyMessenger.ShowMissionStatus();
            else
                m_proxyMessenger.HideMissionStatus();
        }

        public Process ToggleSOPBulletin()
        {
            ExecuteManager mgr = new ExecuteManager();

            Process process = mgr.RunCheckProcess("SOPBulletin");

            if (process == null)
            {
                return mgr.RunStartProcess("SOPBulletin.exe", null);
            }
            else
                process.CloseMainWindow();

            return null;
        }

        public void ToggleMinimumWindow()
        {
            if (FormFrame.Instance.WindowState == FormWindowState.Minimized)
                FormFrame.Instance.ToMaxWindow();
            else
                FormFrame.Instance.ToMinWindow();
        }
        
        public void OpenSOP(EquipmentZone equipZone, DateTime sopTime, ProcessIF process)
        {
            // 화재 탐지시 자동으로 SOP열기에서 연결된 SOP버튼 클릭시 열기로 바꿈
            if (equipZone == null || equipZone.LinkedZone == null || process == null)
                return;

            int nSensorID = process.DetectSensorID;
            int nHistoryID = process.SensorHistoryID;

            ProcessType type = process.ProcessType;
            if (type == ProcessType.FireAlarm)
                m_proxyMessenger.OpenSOP_Fire(equipZone.LinkedZone.ID, sopTime, nSensorID, nHistoryID);
            else if (type == ProcessType.PSMAlarm)
            {
                UnE.PSM.PSMSensorZone sensorZone = (UnE.PSM.PSMSensorZone)process.TargetSensor;
                if (sensorZone != null)
                {
                    UnE.PSM.PSMSensor sensor = sensorZone.OrgSensor;
                    if( sensor != null)
                    {
                        int nMaterialType = sensor.MaterialType;
                    }
                   
                }
                else
                {
                }
                m_proxyMessenger.OpenSOP_PSM(equipZone.ID, sopTime, nSensorID, nHistoryID);
            }
            else if (type == ProcessType.SecurityAlarm)
                m_proxyMessenger.OpenSOP_Security(equipZone.ID, sopTime, nSensorID, nHistoryID, process.TargetSensor);
        }

        public void OpenSOPClicked(EquipmentZone equipZone, DateTime situationTime, ProcessIF process)
        {
            OpenSOP(equipZone, situationTime, process);
        }

        private void FormMain_LocationChanged(object sender, EventArgs e)
        {
            if (m_PaneBtnHome.Visible == false)
            {
                m_PaneBtnHome.Show(this);
                m_PaneBtnHome.Visible = false;
            }

            Point pt = new Point(btnHome.Location.X - 8, btnHome.Location.Y + btnHome.Height);
            Point ptScr = panelMiddle.PointToScreen(pt);
            m_PaneBtnHome.Location = ptScr;// PointToClient(ptScr);

            if (m_PaneBtnSaveHome.Visible == false)
            {
                m_PaneBtnSaveHome.Show(this);
                m_PaneBtnSaveHome.Visible = false;
            }

            Point pt2 = new Point(btnSaveHome.Location.X + btnSaveHome.Width + 4, btnSaveHome.Location.Y + btnSaveHome.Height);
            Point ptScr2 = panelMiddle.PointToScreen(pt2);
            m_PaneBtnSaveHome.Location = ptScr2;// PointToClient(ptScr);

            if (m_PanelBtnNotice.Visible == false)
            {
                m_PanelBtnNotice.Show(this);
                m_PanelBtnNotice.Visible = false;
            }
            Point pt3 = new Point(btnLayerNotice.Location.X + btnLayerNotice.Width + 4, btnLayerNotice.Location.Y + btnLayerNotice.Height);
            Point ptScr3 = panelMiddle.PointToScreen(pt3);
            m_PanelBtnNotice.Location = ptScr3;

            if (m_PanelBtnSimulator.Visible == false)
            {
                m_PanelBtnSimulator.Show(this);
                m_PanelBtnSimulator.Visible = false;
            }

            Point pt4 = new Point(btnSimulator.Location.X - 8, btnHome.Location.Y + btnHome.Height);
            Point ptScr4 = panelMiddle.PointToScreen(pt4);
            m_PanelBtnSimulator.Location = ptScr;// PointToClient(ptScr);

            m_toolbar.Location = new Point(FormFrame.Instance.Location.X + m_toolbar.RelativePosition.X, FormFrame.Instance.Location.Y + m_toolbar.RelativePosition.Y);
        }

        public void SetTitle(string strTitle)
        {
            labelTitle.Text = strTitle;
        }



        private Process m_CCTVProcess = null;
        public Process CCTVProcess
        {
            get { return m_CCTVProcess; }
        }

        public bool IsShowCCTVForm()
        {
            if (m_CCTVProcess == null || m_CCTVProcess.HasExited == true)
                return false;
            return true;
        }

        private Process StartPocess(string szFileName, string szWorkDir, string args)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = szFileName;
            startInfo.WorkingDirectory = szWorkDir;
            startInfo.ErrorDialog = true;
            startInfo.Arguments = args;

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);
                return process;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            return null;
        }

        public Process GetProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                {
                    try
                    {
                        return process;
                    }
                    catch (Exception)
                    { }
                    //break;
                }
            }
            return null;
        }

        private void CreateCCTVProcess(string szName)
        {
            try
            {
                if (m_CCTVProcess != null && m_CCTVProcess.HasExited == false)
                    return;

#if SAFE_KOREA_YH_2017
                return;

                if(UnE.SOP.ProxySOP.Instance.SiteID == 2)
                {
                    DateTime dtNow = DateTime.Now;
                    DateTime dtTarget = new DateTime(2017, 11, 4);
                    if (dtNow < dtTarget)
                        return;
                }
#endif
                string args = string.Format("{0} {1} {2}", UnE.SOP.ProxySOP.Instance.CCTVMontior, UnE.SOP.ProxySOP.Instance.SiteID, szName);

                string szDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string szFileName = szDir + "\\" + "libCCTV.exe";

                if (File.Exists(szFileName))
                {
                    m_CCTVProcess = StartPocess(szFileName, szDir, args);
                    //m_CCTVProcess = GetProcess("libCCTV");
                }
                else
                {
                    szDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                    szFileName = szDir + "\\common\\" + "libCCTV.exe";

                    m_CCTVProcess = StartPocess(szFileName, szDir, args);
                    //m_CCTVProcess = GetProcess("libCCTV");
                }
            }
            catch (Exception)
            {
                m_CCTVProcess = null;
            }
        }

        private void KillCCTVProcess()
        {
            Process p = m_CCTVProcess;

            if (p != null && p.HasExited == false)
            {
                try
                {
                    p.Kill();
                }
                catch (Exception)
                {
                }
            }
            m_CCTVProcess = null;
        }


        private bool m_bShowCCTVForm = true;
        public void ShowCCTVForm(bool bOnlyShow = false)
        {
            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                if (m_bShowCCTVForm == false)
                {
                    if (m_CCTVProcess == null || m_CCTVProcess.HasExited == true)
                    {
                        Guid guid = Guid.NewGuid();
                        string szName = string.Format("libCCTV{0}", guid.ToString());
                        //string szName = "CCTVPipe";
                        if (m_CCTVPipe != null)
                            m_CCTVPipe.OnReciveMessage -= m_CCTVPipe_OnReciveMessage;

                        m_CCTVPipe = new Pipelib.PassivePipeServer(true, szName);
                        m_CCTVPipe.OnReciveMessage += m_CCTVPipe_OnReciveMessage;
                        m_CCTVPipe.BeginPipe();

                        CreateCCTVProcess(szName);
                    }
                    m_bShowCCTVForm = true;
                    m_CCTVPipe.Send("SetVisible(True)");
                }
                else
                {
                    if (m_bShowCCTVForm == true && bOnlyShow == true)
                    {
                        m_bShowCCTVForm = true;
                        m_CCTVPipe.Send("SetVisible(True)");
                    }
                    else if (bOnlyShow == false)
                    {
                        m_bShowCCTVForm = false;
                        m_CCTVPipe.Send("SetVisible(False)");
                    }
                }
            }
            else
            {
                if (m_proxyMessenger == null)
                    return;

                if (!m_proxyMessenger.IsVisibleSOPSimulator())
                    m_proxyMessenger.ShowSOPSimulator();


                m_proxyMessenger.EnableCCTV();
            }
        }


        private void btnDefaultCCTV_Click(object sender, EventArgs e)
        {
            ShowCCTVForm();
        }

        public void ShowToolbar()
        {
            if (m_readyToReceiveMessage == true)
            {
                //m_toolbar.TopMost = true;
                if (!m_toolbar.Visible)
                {
                    //m_toolbar.TopLevel = false;
                   // m_toolbar.TopMost = true;
                    //this.PageHome.ContentForm.OutdoorView.ControlPane
                    //m_toolbar.Parent = this.PageHome.ContentForm.OutdoorView;
                    //m_toolbar.Owner = this.PageHome.ContentForm.OutdoorView;
                    m_toolbar.Show(this);
                    
                }
                m_toolbar.Focus();
            }

          
        }

        public void HideToolbar()
        {
            if (m_toolbar.Visible)
                m_toolbar.Hide();
        }

        public void OnClickToolbarButton(int nID)
        {
            if (nID == ID.ID_VIEW_SIMULATOR)
                btnSimulator_Click(null, null);
            else
            {
                Button btn = m_toolbar.GetButton(nID);

                if (btn != null)
                    OnClickToolBarButton(btn, null);
                else
                {
                    if(m_dicIDButtons.ContainsKey(nID))
                    {
                        btn = m_dicIDButtons[nID];
                        OnClickToolBarButton(btn, null);
                    }
                }
            } 
        }

        public bool GetCurrentReportDate(out DateTime dtBegin, out DateTime dtEnd)
        {
            try
            {
                dtBegin = DateTime.ParseExact(proc_btnStartDate.Text, "yyyy-MM-dd", null);
                dtEnd = DateTime.ParseExact(proc_btnEndDate.Text, "yyyy-MM-dd", null);
            }
            catch (Exception)
            {
                dtBegin = dtEnd = new DateTime();
                return false;
            }

            return true;
        }

        public bool GetCurrentReportOption(out int nSplitUnitOfMeasure, out int nSplitUnitOfMeasureDetail, out int nViewCount)
        {
            nSplitUnitOfMeasure = proc_cboSplitUnit.SelectedIndex;
            nSplitUnitOfMeasureDetail = Convert.ToInt32(nudSplitUnitDetail.Value);
            nViewCount = Convert.ToInt32(proc_cboViewCount.SelectedItem);

            return true;
        }

        public bool GetDetectPSMReportDate(out DateTime dtBegin, out DateTime dtEnd)
        {
            try
            {
                dtBegin = DateTime.ParseExact(btnDetectPSMStartDate.Text, "yyyy-MM-dd", null);
                dtEnd = DateTime.ParseExact(btnDetectPSMEndDate.Text, "yyyy-MM-dd", null);
            }
            catch (Exception)
            {
                dtBegin = dtEnd = new DateTime();
                return false;
            }

            return true;
        }

        public bool GetDetectPSMReportOption(out int nSplitUnitOfMeansure, out int nSplitUnitOfMeansureDetail, out int nViewCount)
        {
            nSplitUnitOfMeansure = cboDetectPSMSplitUnit.SelectedIndex;
            nSplitUnitOfMeansureDetail = Convert.ToInt32(nudDetectPSMSplitUnitDetail.Value);
            nViewCount = Convert.ToInt32(cboDetectPSMViewCount.SelectedItem);

            return true;
        }

        public bool GetNotOperationPSMReportDate(out DateTime dtBegin, out DateTime dtEnd)
        {
            try
            {
                dtBegin = DateTime.ParseExact(btnNotOperationPSMStartDate.Text, "yyyy-MM-dd", null);
                dtEnd = DateTime.ParseExact(btnNotOperationPSMEndDate.Text, "yyyy-MM-dd", null);
            }
            catch (Exception)
            {
                dtBegin = dtEnd = new DateTime();
                return false;
            }

            return true;
        }

        private void proc_cboSplitUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 기본 값으로 항상 1로 초기화
            nudSplitUnitDetail.Value = 1;

            // 주기 테스트 변경
            this.lblSplitUnitDetail.Text = String.Format("{0} 마다", (sender as ComboBox).Text);
        }

        private void cboDetectPSMSplitUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 기본 값으로 항상 1로 초기화
            nudDetectPSMSplitUnitDetail.Value = 1;

            // 주기 테스트 변경
            this.lblDetectPSMSplitUnitDetail.Text = String.Format("{0} 마다", (sender as ComboBox).Text);
        }

        private void btnDateFormat_Click(object sender, EventArgs e)
        {
            SetDateTimeFormat();
        }

        private void m_toolbar_LocationChanged(object sender, EventArgs e)
        {
            DockBtnHome();
        }

        private void m_toolbar_VisibleChanged(object sender, EventArgs e)
        {
            DockBtnHome();
        }

        private void btnDetectPSMDateFormat_Click(object sender, EventArgs e)
        {
            SetDateTimeFormat();
        }


        private void SetDateTimeFormat()
        {
            SDMS.PopupDialog.FormDateTimeFormat form = new PopupDialog.FormDateTimeFormat();

            PageBackstageHome.ShowTranslucentForm(form, 200, 100, form.Width, form.Size.Height, ID.ID_BTN_DATE_FORMAT);
        }

        private void DockBtnHome()
        {
            if (m_toolbar.Visible == false)
            {
                m_PaneBtnHome.Hide();
                m_PanelBtnSimulator.Hide();
            }

            if (m_PaneBtnHome.Visible == true)
            {
                Point pt = new Point(m_toolbar.Location.X, m_toolbar.Location.Y + m_toolbar.Size.Height);

                // Toolbar가 화면 아래쪽에 있으면 m_PaneBtnHome을 Toolbar 위쪽에 띄운다.
                if (pt.Y + m_PaneBtnHome.Size.Height > FormFrame.Instance.Location.Y + FormFrame.Instance.Size.Height)
                    pt = new Point(m_toolbar.Location.X, m_toolbar.Location.Y - m_PaneBtnHome.Size.Height);

                m_PaneBtnHome.Location = pt;
            }

            if (m_PanelBtnSimulator.Visible == true)
            {
                Point pt = new Point(m_toolbar.Location.X + m_toolbar.GetButton(ID.ID_VIEW_SIMULATOR).Location.X, m_toolbar.Location.Y + m_toolbar.Size.Height);

                // Toolbar가 화면 아래쪽에 있으면 m_PaneBtnHome을 Toolbar 위쪽에 띄운다.
                if (pt.Y + m_PaneBtnHome.Size.Height > FormFrame.Instance.Location.Y + FormFrame.Instance.Size.Height)
                    pt = new Point(m_toolbar.Location.X + m_toolbar.GetButton(ID.ID_VIEW_SIMULATOR).Location.X, m_toolbar.Location.Y - m_PaneBtnHome.Size.Height);

                m_PanelBtnSimulator.Location = pt;
            }

        }

        private void react_btnStartDate_TextChanged(object sender, EventArgs e)
        {

        }

        private void cboActionPSMStartTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            react_cboStartTime.Text = cboActionPSMStartTime.Text;
        }

        private void cboActionPSMEndTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            react_cboEndTime.Text = cboActionPSMEndTime.Text;
        }

        private void react_cboStartTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboActionPSMStartTime.Text = react_cboStartTime.Text;
        }

        private void react_cboEndTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboActionPSMEndTime.Text = react_cboEndTime.Text;
        }

        private ToolTip cmbSensorDetectTooltip = new ToolTip();
        private void cmbFireDetect_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            { 
                return;
            }

            e.DrawBackground();

            int nCount = cmbFireDetect.Items.Count;
            if (nCount == 0 || e.Index >= nCount)
                return;

            // added this line thanks to Andrew's comment
            string text = cmbFireDetect.GetItemText(cmbFireDetect.Items[e.Index]);            
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            { 
                e.Graphics.DrawString(text, e.Font, br, e.Bounds);
            }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                if (cmbSensorDetectTooltip != null && cmbFireDetect.DroppedDown == true)
                    cmbSensorDetectTooltip.Show(text, cmbFireDetect, e.Bounds.Right, e.Bounds.Bottom);
            }
            e.DrawFocusRectangle();
        }



        /*private void PopPSMList()
        {
            if (m_frmPSMList == null || m_frmPSMList.IsDisposed)
            {
                m_frmPSMList = new PopupDialog.FormPSMList();
                m_frmPSMList.Show(this);
            }
            m_frmPSMList.Focus();
        }*/

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            ShowSendMessage();
        }

        public void ShowSendMessage()
        {
            if (m_frmMessageSender == null || m_frmMessageSender.IsDisposed)
                m_frmMessageSender = new PopupDialog.FormMessageSender();

            if (m_frmMessageSender.Visible)
                m_frmMessageSender.Focus();
            else
                m_frmMessageSender.Show(this);
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            ClearSelectDlg();
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {

        }

        /*protected override void WndProc(ref Message m)
        {
            if (m.Msg == libSplash.Message.WM_COPYDATA)
            {
                libSplash.COPYDATASTRUCT cds = (libSplash.COPYDATASTRUCT)m.GetLParam(typeof(libSplash.COPYDATASTRUCT));

                if (cds.lpData.ToLower() == "splashhandle")
                    m_splashManager.SplashHandle = cds.dwData;

                return;
            }

            base.WndProc(ref m);
        }*/


        internal void SendSensorCloseMessageToSOPSimulator(int nSensorID, int nSensorHistoryID)
        {
            m_proxyMessenger.SensorClose(nSensorID, nSensorHistoryID);
        }
        
    }
}