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
using libSensorProcess;
using UnE.GUI;
using System.Runtime.InteropServices;
using SDMS.Help;
using SDMS.PopupDialog;
using DBUtility2;
using libExternalUI;

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

    public enum Resolution {  NONE = -1, FourK = 0, FullHD = 1, Other = 2 }

    public partial class FormMain : Form, ITextPictureBoxOwner, IRibbonButtonOwner, libSensorProcess.IProcessOwner, IImageButtonOwner
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
                //m_nLabelFireDetectInitPos = FormMain.Instance.labelFireDetect.Location.X;
                m_nComboBoxFireDetectInitPos = FormMain.Instance.cmbFireDetect.Location.X;

                m_nPanelMiddleInitSize = FormMain.Instance.panelMiddle.Size.Width;
            }
        }       

        private const string CloseManualFire = "기타재난 상황종료";
        private const string RaiseManualFire = "기타재난 전파";

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
        private Resolution m_Resolution = Resolution.FourK;
        public Resolution Resolution
        {
            get { return m_Resolution; }
        }

        private Resolution m_ResolutionBefore = Resolution.NONE;

        private int m_nPanelTopHeight1 = 83;//154;//169;
        private int m_nPanelTopHeight2 = 67;//154;//169;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private WebDBManager m_dbMgr = null;//new DBUtility.WebDBManager();

        public WebDBManager DBManager
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

        public Size nContentSize
        {
            get { return panelBottom.Size; }
        }

        // Button별 ID
        private Dictionary<ImageButton, int> m_dicButtonIDs = new Dictionary<ImageButton, int>();
        private Dictionary<int, ImageButton> m_dicIDButtons = new Dictionary<int, ImageButton>();
        private Dictionary<ImageButton, bool> m_dicButtonChecked = new Dictionary<ImageButton, bool>();
        // OptionSDMS 테이블 Button의 이름 : Visible 여부를 위해서
        private Dictionary<string, ImageButton> m_dicOptionSDMSNameButtons = new Dictionary<string, ImageButton>();
        //private Dictionary<string, ImageButton> m_dicOptionSDMSNameButtons = new Dictionary<string, ImageButton>();

        // Form Move 를 위한 Panel Top 클릭 체크
        private bool m_bLeftMouseDown = false;

        // Form Move 를 위한 Panel Top 클릭 점
        private Point m_ptMove;

        private NetworkWebManager m_netMgr = null;

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
        private bool m_useTemperatureHumidity = false;

        // SOP 시스템을 사용하는가?
        private bool m_useSOPSystem = true;
        // CCTV Viewer를 사용하는가?
        private bool m_useCCTVViewer = true;

        private IUIManager m_uiManager = null;

        public bool UseTH
        {
            get { return m_useTemperatureHumidity; }
        }

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

        private ManualManager m_manualManager = null;

        private int m_nSOPGenUserID = -1;
        private string m_strSOPGenUserRealName = "";
        private bool m_isVisibleEquipZoneCCTV = false;

        private WeatherDisplay.FormWeatherFrame m_frmWeather = null;
        private PopupDialog.FloatingToolbar m_toolbar = null;
        //private PopupDialog.FormPSMList m_frmPSMList = null;

        public bool UseNanumFont = false;

        private PopupDialog.FormMessageSender m_frmMessageSender = null;
        private PopupDialog.FormMessageReceiver m_frmMessageReceiver = null;

        //private List<Control> m_fireReportButtons = new List<Control>();
        //private List<Control> m_psmReportButtons = new List<Control>();
        //private List<Control> m_intrusionReportButtons = new List<Control>();

        private SplashManager m_splashManager = null;

        private IProxyMessenser m_proxyMessenger = new ProxyMessenger();

        // ToggleWindow() 호출에 의하여 화면에서 사라진 상태인가?
        private bool m_toggleHideStatus = false;

        #region EquipZoneVolume
        private bool m_useEquipZoneVolume = false;
        // Key : EquipZoneID
        // Value : VolumeName
        private Dictionary<int, string> m_dicEquiZoneVolume = new Dictionary<int, string>();
        // Key : Zone ID
        // Value : SceneName
        private Dictionary<int, string> m_dicZoneScene = new Dictionary<int, string>();
        // Key : BuildingGroup ID
        // Value : Scene Name
        private Dictionary<int, string> m_dicBuildingGroupScene = new Dictionary<int, string>();
        #endregion

        public IProxyMessenser ProxyMessenger
        {
            get { return m_proxyMessenger; }
            //set { m_proxyMessenger = value; }
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

        public bool OpenSOPOnDetectSensor
        {
            get { return UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect; }
        }

        public bool IsDisaster { get { return m_IsDisaster; } }
        private bool m_IsDisaster
        {
            get
            {
                if (cmbFireDetect.Items != null && cmbFireDetect.Items.Count > 0)
                    return true;

                return false;
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

                    SetVisibleDisasterPanel(false);
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

        private bool SetMonitorForm(int nDisplay)
        {
            FormFrame form = FormFrame.Instance;

            //Screen[] sc = Screen.AllScreens;
            Screen[] sc = Screen.AllScreens.OrderBy(p => p.Bounds.Location.Y).OrderBy(p => p.Bounds.Location.X).ToArray();
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
                if (i == (nDisplay - 1))
                //if (sc[i].DeviceName.IndexOf(szNum) != -1)
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
                form.OriginLocation = sc[nIdx].Bounds.Location;
                //form.Location = sc[nIdx].Bounds.Location;
                //form.Size = new Size(sc[nIdx].Bounds.Width, sc[nIdx].Bounds.Height);
                form.Size = new Size(sc[nIdx].Bounds.Width - 40, sc[nIdx].Bounds.Height - 40);
            }

            form.WindowState = FormWindowState.Maximized;

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
            string strTH = "UseTH", strReport = "UseReport";
            string strSQL = string.Format("Select PropertyName, PropertyValue from OptionSDMS where (PropertyName = '{0}' or PropertyName = '{1}') and SiteID = {2}",
                strTH, strReport, UnE.SOP.ProxySOP.Instance.SiteID);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            bool useReport = true;
            bool useTH = false;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyName == null || strPropertyValue == null)
                    continue;

                if (string.Compare(strPropertyName, strReport, true) == 0)
                {
                    if (ReadBoolean(strPropertyValue, out useReport) == false)
                        return false;
                }
                else if (string.Compare(strPropertyName, strTH, true) == 0)
                {
                    if (ReadBoolean(strPropertyValue, out useTH))
                        m_useTemperatureHumidity = useTH;
                }
            }

            return useReport;
        }

        private bool ReadBoolean(string strValue, out bool value)
        {
            value = false;

            if (string.Compare(strValue, "true", true) == 0 || strValue == "1")
            {
                value = true;
                return true;
            }
            else if (string.Compare(strValue, "false", true) == 0 || strValue == "0")
            {
                value = false;
                return true;
            }

            return false;
        }

        private bool ReadPSMInfo()
        {         
            string strSQL = "Select PropertyName, PropertyValue from OptionSDMS where PropertyName = 'UsePSM' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 1]);

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

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 1]);

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

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strPropertyValue = WebDBManager.GetStringField(arrResult[0]);

            if (strPropertyValue == null)
                return false;

            if (strPropertyValue == "1" || string.Compare("true", strPropertyValue, true) == 0)
                return true;

            return false;
        }

        private void ReadAppOptions()
        {
            string strTargetUseSOP = "UseSOP", strTargetUseCCTVViewer = "UseCCTVViewer";
            string strSQL = string.Format("Select PropertyName, PropertyValue from OptionSDMS where (PropertyName = '{0}' or PropertyName = '{1}') and SiteID = {2}",
                strTargetUseSOP, strTargetUseCCTVViewer, UnE.SOP.ProxySOP.Instance.SiteID.ToString());

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyName == null || strPropertyValue == null)
                    continue;

                bool use = strPropertyValue.ToLower() == "false" || strPropertyValue == "0" ? false : true;

                if (string.Compare(strPropertyName, strTargetUseSOP, true) == 0)
                    m_useSOPSystem = use;
                else if (string.Compare(strPropertyName, strTargetUseCCTVViewer, true) == 0)
                {
                    m_useCCTVViewer = use;
                    UnE.SOP.ProxySOP.Instance.ShowCCTVForm = use;
                }
            }
        }

        private void ReadEquipZoneVolumeOption()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'UseEquipZoneVolume' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strPropertyValue = WebDBManager.GetStringField(arrResult[0]);

            if (strPropertyValue == null)
                return;

            if (strPropertyValue == "1" || string.Compare("true", strPropertyValue, true) == 0)
            {
                m_useEquipZoneVolume = true;
                ReadEquipZoneVolume();
                ReadZoneScene();
                ReadBuildingGroupScene();
            }
        }

        private void ReadBuildingGroupScene()
        {
            string strSQL = "Select bg.ID, bgs.SceneName from BuildingGroup as bg, BuildingGroupScene as bgs where bg.ID = bgs.BuildingGroupID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> buildingGroupID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSceneName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (buildingGroupID == null || strSceneName == null)
                    continue;

                m_dicBuildingGroupScene[buildingGroupID.Data] = strSceneName;
            }
        }

        private void ReadZoneScene()
        {
            string strSQL = "Select Zone.ID, zs.SceneName from Zone, ZoneScene as zs where Zone.ID = zs.ZoneID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSceneName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (zoneID == null || strSceneName == null)
                    continue;

                m_dicZoneScene[zoneID.Data] = strSceneName;
            }
        }

        private void ReadEquipZoneVolume()
        {
            string strSQL = "Select ez.ID, ezv.VolumeName from EquipmentZone as ez, EquipZoneVolume as ezv where ez.ID = ezv.EquipZoneID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strVolumeName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (equipZoneID == null || strVolumeName == null)
                    continue;

                m_dicEquiZoneVolume[equipZoneID.Data] = strVolumeName;
            }
        }

        public FormMain(int nSOPGenUserID, string strSOPGenUserRealName, int nMonitor, bool isSimulationMode)
        {
            this.DoubleBuffered = true;            
            UnE.View.Content.FormContentUnity.KillProcess("CCTVViewer");
            UnE.View.Content.FormContentUnity.KillProcess("CCTVViewer2");
            UnE.View.Content.FormContentUnity.KillProcess("UnitySam");
            UnE.View.Content.FormContentUnity.KillProcess("UnitySamInside");
            UnE.View.Content.FormContentUnity.KillProcess("UnityA10");
            UnE.View.Content.FormContentUnity.KillProcess("libCCTV");
            UnE.View.Content.FormContentUnity.KillProcess("libCCTV2");
            UnE.View.Content.FormContentUnity.KillProcess("EnergyOutside");
            UnE.View.Content.FormContentUnity.KillProcess("SeoulUnv");
            UnE.View.Content.FormContentUnity.KillProcess("BusanUnv");
            ReadSiteID();

            if (UnE.SOP.ProxySOP.Instance.SiteID == 3)
            {
                Program.prgFont = "맑은 고딕";
            }

            // 프로세스 종료후 1초정도 기다린다. -> skkim 2016-02-01
            System.Threading.Thread.Sleep(1000);

            m_instance = this;
            //m_nMonitor = nMonitor;
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

            m_dbMgr = new WebDBManager(nSiteID);
            SetMonitors();

            UnE.SOP.ProxySOP.Instance.UsePSM = ReadPSMInfo();
            UnE.SOP.ProxySOP.Instance.UseIntrusion = ReadIntrusionInfo();
            m_useReport = ReadReportOption();
            UnE.SOP.ProxySOP.Instance.UseEarthquake = ReadEarthquake();
            ReadEquipZoneVolumeOption();
            ReadAppOptions();

            m_dataMgr = new DataManager(m_dbMgr);
            //Debug.WriteLine(DateTime.Now);

            InitializeComponent();
             
            // 2018 디자인에 적용된 폰트 있는지 확인
            System.Drawing.Text.InstalledFontCollection font = new System.Drawing.Text.InstalledFontCollection();
            FontFamily[] fonts = font.Families;
            for (int i = 0; i < fonts.Length; i++)
            {
                if (fonts[i].Name == Program.prgFont)
                {
                    UseNanumFont = true;
                    break;
                }
            }

            if (nSiteID == 102)
            {
                m_Img3dTabDefault = global::SDMS.Properties.Resources._2DTab_Default;
                m_Img3dTabClick = global::SDMS.Properties.Resources._2DTab_Click;

                btnSensorMonitor.Visible = labelSensorMonitor.Visible = false;
            }

            m_splashManager = new SplashManager(m_dbMgr, nSiteID);
            m_splashManager.RunSplash();

            m_splashManager.SendSplashMessage("건물 및 센서 정보 로딩중...", libSplash.Message.SPLASH_MESSAGE, 10256);
            LoadBaseData();

            ProcessManager.Instance.ProcessOwner = this;
            ProcessManager.Instance.ZoneManager = ZoneManager.Instance;
            //Debug.WriteLine(DateTime.Now);
            
            //InitializeComponent();
            InitCtrlSize();  
            SetDoubleBuffer(panelTop, true);
            SetDoubleBuffer(panelTop2, true);
            SetDoubleBuffer(panelLeft2, true);
            SetDoubleBuffer(panelLeftItem, true);
            SetDoubleBuffer(panelLeft3DTabItemCtrl, true);
            SetDoubleBuffer(panelLeftAdminTabItemCtrl, true);    
            SetDoubleBuffer(panelStatus, true);
            SetDoubleBuffer(panelLog, true); 

            if (m_bUse2D == true)
            {
                labelSelectZone.Visible = true;
                cboBuildingGroup.Visible = true;
                cboBuilding.Visible = true;
                cboFloor.Visible = true;
                btnSelectZone.Visible = true;
            }
             
            //m_nOriginLeftPanelWidth = panelLeft.Size.Width;

            this.Name = "SDMS";
            this.FormClosing += FormMain_FormClosing;
            this.FormClosed += FormMain_FormClosed;
            this.Load += FormMain_Load;
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            //Debug.WriteLine(DateTime.Now);
            AddPythonFunction();
            //Debug.WriteLine(DateTime.Now);
            m_nSystemButtonSpace = btnMax.Location.X - (btnMin.Location.X + btnMin.Size.Width);
            //btnMax.Parent = panelTop;
            //btnMin.Parent = panelTop;
            //btnClose.Parent = panelTop;

            m_splashManager.SendSplashMessage("탭 초기화...", libSplash.Message.SPLASH_MESSAGE, 10256);
            InitTab();
            //Debug.WriteLine(DateTime.Now);

            m_splashManager.SendSplashMessage("Layout 작성중...", libSplash.Message.SPLASH_MESSAGE, 46154);
            CreateBackstageHome();
            //Debug.WriteLine(DateTime.Now);

            WeatherDisplay.FormWeatherDisplay frmWeather = new WeatherDisplay.FormWeatherDisplay();
            m_frmWeather = new WeatherDisplay.FormWeatherFrame(frmWeather);
            m_frmWeather.Size = frmWeather.Size;
             
            m_splashManager.SendSplashMessage("CCTV Viewer 로딩...", libSplash.Message.SPLASH_MESSAGE, 56410);
            UnE.View.Content.FormContentUnity.KillProcess("libCCTV");
            UnE.View.Content.FormContentUnity.KillProcess("libCCTV2");
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

            m_manualManager = new ManualManager(this);
            SetManualID();
            m_uiManager = libExternalUI.Factory.GetUIManager(this);

            if (!m_useCCTVViewer)
                btnDefaultCCTV.Enabled = false;
        }

        private void ReadSiteID()
        {
            DBUtility2.Utility util = new DBUtility2.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                UnE.Utility.UMessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                UnE.SOP.ProxySOP.Instance.SiteID = nSiteId;
            }
            else
            {
                UnE.Utility.UMessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
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
            // 핫키 해제
            UnregisterHotKey((int)this.Handle, 0); // ID = 0
            UnregisterHotKey((int)this.Handle, 1); 
            UnregisterHotKey((int)this.Handle, 2);
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
        
        private bool m_bUse2D = false;

        private void InitTab()
        {  
            if (m_bUse2D  == false)
            {
                //pictureBoxAdmin.Location = pictureBoxReport.Location;
                //pictureBoxReport.Location = pictureBox2D.Location;
                pictureBox2D.Visible = false;              
            } 
          
            pictureBox2D.SetPictureBoxOwner(this);
            pictureBoxCCTV.SetPictureBoxOwner(this);
             
            pictureBox2D.Text = "2D";
            pictureBoxCCTV.Text = "CCTV";

            panelTop.Size = new Size(this.Size.Width, m_nPanelTopHeight1);
            panelTop2.Size = new System.Drawing.Size(this.Size.Width, m_nPanelTopHeight2);
            //panelMiddle.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
            panelMiddle.Visible = false;

            SetTopPanelControls();
        }

        // Control을 Dictionary에 넣는 이유 : 해상도에 따라서 컨트롤 위치를 변경해야 하는데 Dictionary에 넣지않으면 컨트롤의 순서를 알수 없음         
        // CCTV 정보, 상세정보, 화면설정, 알림공지, 도움말, 알람, 상세:, 재난발생: ...
        private Dictionary<int, Control> m_PanelTop3DTabItemCtrl = new Dictionary<int, Control>();
        private Dictionary<int, Control> m_PanelTopReportTabActionPSMItemCtrl = new Dictionary<int, Control>();
        private Dictionary<int, Control> m_PanelTopReportTabSMSPSMItemCtrl = new Dictionary<int, Control>();
        private Dictionary<int, Control> m_PanelTopReportTabDetectPSMItemCtrl = new Dictionary<int, Control>();
        private Dictionary<int, Control> m_PanelTopReportTabReactionHistoryItemCtrl = new Dictionary<int, Control>();
        private Dictionary<int, Control> m_PanelTopReportTabProcessHistoryItemCtrl = new Dictionary<int, Control>();
        private Dictionary<int, Control> m_PanelTopReportTabNotOperationPSMItemCtrl = new Dictionary<int, Control>(); 
        // 재난탐지, SOP 시스템, 상황판, 현황판, CCTV
        private List<Control> m_PanelLeft3DTabItemCtrl = new List<Control>();
        // 설비목록, 담당자관리, 메세지관리, 방송관리, 탐지관리, 저장
        private List<Control> m_PanelLeftAdminTabItemCtrl = new List<Control>(); 
        private void SetTopPanelControls()
        {
            pictureBoxApp.Parent = panelTop;

            #region 3D, 관리, 리포트 Tab 이동 버튼
            btn3DTab.Parent = panelTop;
            btnAdminTab.Parent = panelTop;
            btnReportTab.Parent = panelTop;

            btn3DTab.Image = m_Img3dTabClick;
            btn3DTab.ImageNormal = m_Img3dTabClick;
            btn3DTab.ImageMouseOver = m_Img3dTabClick;

            btnAdminTab.Image = m_ImgAdminTabDefault;
            btnAdminTab.ImageNormal = m_ImgAdminTabDefault;
            btnAdminTab.ImageMouseOver = m_ImgAdminTabDefault;

            btnReportTab.Image = m_ImgReportdTabDefault;
            btnReportTab.ImageNormal = m_ImgReportdTabDefault;
            btnReportTab.ImageMouseOver = m_ImgReportdTabDefault; 
            #endregion
              
            #region 재난상황 (알람발생)
            panelStatus.Size = new System.Drawing.Size(712, 164);
            mLabelStatus.Parent = panelStatus;
            panelLog.Size = new System.Drawing.Size(2361, 164);
            mLabelLog.Parent = panelLog;
            
            btnFire.Size = new System.Drawing.Size(596, 164);            
            lblFireText.Parent = btnFire;
            lblFireText.Location = new Point(btnFire.Width / 2 - lblFireText.Width / 2, btnFire.Height / 2 - lblFireText.Height / 2);
            //lblFireText.Visible = false;
            #endregion

            #region panelTop2
            // 3D랑 관리탭에서 보여지는 Control
            m_PanelTop3DTabItemCtrl.Add(1, btnLayerCCTV); 
            m_PanelTop3DTabItemCtrl.Add(2, btnLayerBuildingText); 
            m_PanelTop3DTabItemCtrl.Add(3, btnSaveHome);
            m_PanelTop3DTabItemCtrl.Add(4, btnSendMessage);
            m_PanelTop3DTabItemCtrl.Add(5, btnHelp);
            m_PanelTop3DTabItemCtrl.Add(6, btnLayerNotice);
            if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
            {
                m_PanelTop3DTabItemCtrl.Add(7, labelFireDetect);
                m_PanelTop3DTabItemCtrl.Add(8, cmbFireDetect); 
            }
            else
            {
                m_PanelTop3DTabItemCtrl.Add(7, btnSensorMonitor);
                m_PanelTop3DTabItemCtrl.Add(8, labelFireDetect);
                m_PanelTop3DTabItemCtrl.Add(9, cmbFireDetect);

                labelSensorMonitor.ForeColor = Color.FromArgb(0x3f, 0x3f, 0x3f);
                labelSensorMonitor.Parent = btnSensorMonitor;
            }

            foreach (KeyValuePair<int, Control> item in m_PanelTop3DTabItemCtrl)
            {
                item.Value.Parent = panelTop3DTabItemCtrl;
            }
            panelTop3DTabItemCtrl.Parent = panelTop2; 

            // 보고서탭에서 보여지는 Control 
            m_PanelTopReportTabActionPSMItemCtrl.Add(1, btnActionPSMStartDate);
            m_PanelTopReportTabActionPSMItemCtrl.Add(2, btnActionPSMEndDate);
            m_PanelTopReportTabActionPSMItemCtrl.Add(3, cboActionPSMStartTime);
            m_PanelTopReportTabActionPSMItemCtrl.Add(4, label1);
            m_PanelTopReportTabActionPSMItemCtrl.Add(5, cboActionPSMEndTime);
            m_PanelTopReportTabActionPSMItemCtrl.Add(6, cboActionPSMSearchType);
            m_PanelTopReportTabActionPSMItemCtrl.Add(7, btnReactionPSMSelectDisaster);
            m_PanelTopReportTabActionPSMItemCtrl.Add(8, lblActionPSMSelect);
            m_PanelTopReportTabActionPSMItemCtrl.Add(9, cboActionPSMSelect);
            pnActionPSM.Parent = panelTop2;
             
            m_PanelTopReportTabSMSPSMItemCtrl.Add(1, btnSMSPSMStartDate);
            m_PanelTopReportTabSMSPSMItemCtrl.Add(2, btnSMSPSMEndDate);
            m_PanelTopReportTabSMSPSMItemCtrl.Add(3, cboSMSPSMLatelyDate);
            m_PanelTopReportTabSMSPSMItemCtrl.Add(4, lblSMSPSMSelectZone);
            m_PanelTopReportTabSMSPSMItemCtrl.Add(5, cboSMSPSMBuilding);
            m_PanelTopReportTabSMSPSMItemCtrl.Add(6, btnSMSPSMSelectZone); 
            pnSMSPSM.Parent = panelTop2;
            
            m_PanelTopReportTabDetectPSMItemCtrl.Add(1, btnDetectPSMStartDate);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(2, btnDetectPSMEndDate);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(3, cboDetectPSMLatelyDate);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(4, lblDetectPSMSplitUnit);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(5, cboDetectPSMSplitUnit);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(6, labelDetectPSMDateFormat);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(7, nudDetectPSMSplitUnitDetail);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(8, lblDetectPSMSplitUnitDetail);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(9, lblDetectPSMViewCount);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(10, cboDetectPSMViewCount);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(11, lblDetectPSMSelectZone);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(12, cboDetectPSMBuilding);
            m_PanelTopReportTabDetectPSMItemCtrl.Add(13, btnDetectPSMSelectZone);
            pnDetectPSM.Parent = panelTop2;
             
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(1, react_btnStartDate);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(2, react_btnEndDate);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(3, react_cboStartTime);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(4, label14);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(5, react_cboEndTime);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(6, react_cboSearchTypeIntrusion);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(7, react_cboSearchTypeFire);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(8, btnReactionIntrusionSelectDisaster);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(9, btnReactionFireSelectDisaster);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(10, lblFireSelect);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(11, cboActionIntrusionSelect);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(12, cboActionFireSelect);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(13, react_cboSearchTypeEarthquake);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(14, btnReactionEarthquakeSelectDisaster);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(15, cboActionEarthquakeSelect);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(16, react_cboSearchTypeTH);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(17, btnReactionTHSelectDisaster);
            m_PanelTopReportTabReactionHistoryItemCtrl.Add(18, cboActionTHSelect);
            panelReactionHistory.Parent = panelTop2;
             
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(1, proc_btnStartDate);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(2, proc_btnEndDate);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(3, proc_cboLatelyDate);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(4, lblSplitUnit);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(5, proc_cboSplitUnit);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(6, labelDetectDateFormat);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(7, nudSplitUnitDetail);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(8, lblSplitUnitDetail);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(9, lblViewCount);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(10, proc_cboViewCount);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(11, proc_lblSelectZone);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(12, proc_cboBuildingGroup);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(13, proc_cboBuilding);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(14, proc_cboFloor);
            m_PanelTopReportTabProcessHistoryItemCtrl.Add(15, proc_btnSelectZone);
            panelProcessHistory.Parent = panelTop2;
             
            m_PanelTopReportTabNotOperationPSMItemCtrl.Add(1, btnNotOperationPSMStartDate);
            m_PanelTopReportTabNotOperationPSMItemCtrl.Add(2, btnNotOperationPSMEndDate);
            m_PanelTopReportTabNotOperationPSMItemCtrl.Add(3, cboNotOperationPSMLatelyDate);
            m_PanelTopReportTabNotOperationPSMItemCtrl.Add(4, lblNotOperationPSMSelectZone);
            m_PanelTopReportTabNotOperationPSMItemCtrl.Add(5, cboNotOperationPSMBuilding);
            m_PanelTopReportTabNotOperationPSMItemCtrl.Add(6, btnNotOperationPSMSelectZone);
            pnNotOperationPSM.Parent = panelTop2;
            #endregion

            #region Left Item
            panelLeftItem.Parent = panelLeft2;
            panelLeftItem.BackColor = Color.Transparent;

            // 3D Item 
            panelLeft3DTabItemCtrl.Parent = panelLeft2;
            panelLeft3DTabItemCtrl.BackColor = Color.Transparent;

            // 관리탭 Item 
            panelLeftAdminTabItemCtrl.Parent = panelLeft2;
            panelLeftAdminTabItemCtrl.BackColor = Color.Transparent; 
            #endregion
        }

        private void ResizeControls()
        {
            //if (m_ResolutionBefore == m_Resolution)
            //    return;

            m_ResolutionBefore = m_Resolution;

            foreach (KeyValuePair<Control, Font> item in m_CtrlFontSize)
            {
                OtherCtrlFontResize(item.Key, item.Value);
            }
            OtherCtrlFontResize(panelLog.DisplayLabel, m_CtrlFontSize[mLabelStatus]);
             
            dd(this, 3840, 2160);

            if (UnE.SOP.ProxySOP.Instance.SiteID != 102)
            {
                if (m_Resolution == Resolution.FourK)
                    labelSensorMonitor.Location = new Point(70, (int)(btnSensorMonitor.Height * 0.5 - labelSensorMonitor.Height * 0.5));
                else if (m_Resolution == Resolution.FullHD)
                    labelSensorMonitor.Location = new Point(35, (int)(btnSensorMonitor.Height * 0.5 - labelSensorMonitor.Height * 0.5));  
            } 
             
            panelTop2.Location = new Point(0, panelTop.Size.Height);
            panelLeft2.Location = new Point(0, panelTop2.Location.Y + panelTop2.Height);

            // 재난상황 패널
            panelStatus.Location = new Point(panelLeft2.Width, panelTop2.Location.Y + panelTop2.Size.Height);
            mLabelStatus.Location = new Point((int)(panelStatus.Size.Width * 0.5 - mLabelStatus.Size.Width * 0.5), (int)(btnFire.Size.Height * 0.5 - mLabelStatus.Size.Height * 0.5));
            mLabelZone.Location = new Point(panelStatus.Width - mLabelZone.Width - 10, 10);

            panelLog.Location = new Point(panelStatus.Location.X + panelStatus.Width, panelTop2.Location.Y + panelTop2.Size.Height);
            mLabelLog.Location = new Point((int)(panelLog.Size.Width * 0.5 - mLabelLog.Size.Width * 0.5), (int)(panelLog.Size.Height * 0.5 - mLabelLog.Size.Height * 0.5));
            //panelLog.DisplayFont = m_CtrlFontSize[mLabelStatus];
            panelLog.DisplayLabelLocation = new Point((int)(panelLog.Size.Width * 0.5 - mLabelLog.Size.Width * 0.5), (int)(panelLog.Size.Height * 0.5 - mLabelLog.Size.Height * 0.5));

            btnFire.Location = new Point(panelLog.Location.X + panelLog.Width, panelTop2.Location.Y + panelTop2.Size.Height);
            OtherCtrlFontResize(btnFire, m_CtrlFontSize[btnFire]);

            int nSpace0 = 18;
            int nSpace1 = 22;
            int nSpace2 = 13;
            int nSpace3 = 26;
            if (m_Resolution == Resolution.FullHD)
            {
                nSpace0 = (int)(nSpace0 * 0.5);
                nSpace1 = (int)(nSpace1 * 0.5);
                nSpace2 = (int)(nSpace2 * 0.5);
                nSpace3 = (int)(nSpace3 * 0.5);
            }

            int nTemp = 0;

            // PanelTop
            int nTopPanelCenterV = Convert.ToInt32((panelTop.Height * 0.5) - (btn3DTab.Height * 0.5));
            int nBeginH = 50;
            int nSpace = 35;
            if (m_Resolution == Resolution.FullHD)
            {
                nBeginH = (int)(nBeginH * 0.5);
                nSpace = (int)(35 * 0.5); 
            }

            pictureBoxApp.Location = new Point(nSpace2, Convert.ToInt32((panelTop.Height * 0.5) - (pictureBoxApp.Height * 0.5)));

            btn3DTab.Location = new Point(pictureBoxApp.Location.X + pictureBoxApp.Width + nBeginH, nTopPanelCenterV);
            btnAdminTab.Location = new Point(btn3DTab.Location.X + btn3DTab.Width + nSpace, nTopPanelCenterV);
            btnReportTab.Location = new Point(btnAdminTab.Location.X + btnAdminTab.Width + nSpace, nTopPanelCenterV); 

            // PanelTop2   
            nTemp = panelLeft2.Width + nSpace3;            
            CtrlWidthLineUp(m_PanelTop3DTabItemCtrl, (int)(panelTop2.Height * 0.5), nTemp, 0);
            panelTop3DTabItemCtrl.Location = new Point(0, 0);
            panelTop3DTabItemCtrl.Size = panelTop2.Size;
             
            ReportCtrlWidthLineUp(nSpace3, mCurrentTab == UnE.View.Content.ContentOwnerTab.REPORT_TAB); 
              
            // 왼쪽 버튼 (선택, 이동...) 
            nTemp = 0;
            foreach (KeyValuePair<string, ImageButton> item in m_dicOptionSDMSNameButtons)
            {
                ImageButton imgBtn = item.Value as ImageButton;
                if (imgBtn == null || imgBtn.ImageNormal == null)
                    continue;

                int width = imgBtn.ImageNormal.Width;
                int height = imgBtn.ImageNormal.Height;
                OtherCtrlResize(imgBtn, width, height);
                if (m_Resolution == Resolution.FullHD)
                {
                    width = Convert.ToInt32(width * 0.5);
                    height = Convert.ToInt32(height * 0.5);
                }

                //imgBtn.Size = new Size(width, height);
                imgBtn.Location = new Point(0, height * nTemp);
                nTemp++;
            } 

            panelLeftItem.Size = new System.Drawing.Size(btnPick.Width, btnPick.Height * nTemp);
            panelLeftItem.Location = new Point(Convert.ToInt32((panelLeft2.Width * 0.5) - (panelLeftItem.Width * 0.5)), 0);              

            // 왼쪽 버튼 (재난탐지, SOP시스템..)
            CtrlHeightLineUp(m_PanelLeft3DTabItemCtrl, nSpace2); 
            panelLeft3DTabItemCtrl.Size = new System.Drawing.Size(btnSDMS.Width, btnSDMS.Height * 5 + nSpace2 * 4);
            panelLeft3DTabItemCtrl.Location = new Point(Convert.ToInt32((panelLeft2.Width * 0.5) - (panelLeft3DTabItemCtrl.Width * 0.5)), panelLeft2.Height - panelLeft3DTabItemCtrl.Height - 31);
            
            // 관리자탭 버튼 
            CtrlHeightLineUp(m_PanelLeftAdminTabItemCtrl, nSpace2);
            panelLeftAdminTabItemCtrl.Size = new System.Drawing.Size(btnShowList.Width, btnShowList.Height * m_PanelLeftAdminTabItemCtrl.Count + nSpace2 * (m_PanelLeftAdminTabItemCtrl.Count - 1));
            panelLeftAdminTabItemCtrl.Location = new Point(Convert.ToInt32((panelLeft2.Width * 0.5) - (panelLeftAdminTabItemCtrl.Width * 0.5)), panelLeft2.Height - panelLeftAdminTabItemCtrl.Height - 31);

            // 보고서탭 버튼
            ResizeReportRibbonBar();
            pnActionPSM.Size = panelTop2.Size;
            pnSMSPSM.Size = panelTop2.Size;
            pnDetectPSM.Size = panelTop2.Size;
            panelReactionHistory.Size = panelTop2.Size;
            panelProcessHistory.Size = panelTop2.Size;
            pnNotOperationPSM.Size = panelTop2.Size;

            pnActionPSM.Location = new Point(panelLeft2.Width, 0);
            pnSMSPSM.Location = new Point(panelLeft2.Width, 0);
            pnDetectPSM.Location = new Point(panelLeft2.Width, 0);
            panelReactionHistory.Location = new Point(panelLeft2.Width, 0);
            panelProcessHistory.Location = new Point(panelLeft2.Width, 0);
            pnNotOperationPSM.Location = new Point(panelLeft2.Width, 0);
        }

        public void ReportCtrlWidthLineUp(int nSpace, bool visibleChk = false)
        {
            CtrlWidthLineUp(m_PanelTopReportTabActionPSMItemCtrl, (int)(panelTop2.Height * 0.5), 0, nSpace, visibleChk);
            CtrlWidthLineUp(m_PanelTopReportTabSMSPSMItemCtrl, (int)(panelTop2.Height * 0.5), 0, nSpace, visibleChk);
            CtrlWidthLineUp(m_PanelTopReportTabDetectPSMItemCtrl, (int)(panelTop2.Height * 0.5), 0, nSpace, visibleChk);
            CtrlWidthLineUp(m_PanelTopReportTabReactionHistoryItemCtrl, (int)(panelTop2.Height * 0.5), 0, nSpace, visibleChk);
            CtrlWidthLineUp(m_PanelTopReportTabProcessHistoryItemCtrl, (int)(panelTop2.Height * 0.5), 0, nSpace, visibleChk);
            CtrlWidthLineUp(m_PanelTopReportTabNotOperationPSMItemCtrl, (int)(panelTop2.Height * 0.5), 0, nSpace, visibleChk);
        }

        private void OtherCtrlResize(Control ctrl, int width, int height)
        {
            if (m_Resolution == Resolution.FullHD)
            {
                width = Convert.ToInt32(width * 0.5);
                height = Convert.ToInt32(height * 0.5);
            }

            ctrl.Size = new Size(width, height);
        }

        private void OtherCtrlFontResize(Control ctrl, Font font)
        {
            float fontSize = font.Size;
            FontFamily fontFamily = font.FontFamily;
            if (m_Resolution == Resolution.FullHD)
            {

                fontSize = Convert.ToInt32(font.Size * 0.5); 
            }

            ctrl.Font = new System.Drawing.Font(font.FontFamily, fontSize, font.Style, System.Drawing.GraphicsUnit.Point, ((byte)(129))); 
        }

        /// <summary>
        /// Resize시 Control Size 조절
        /// </summary>
        /// <param name="ctrl"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void dd(Control ctrl, int width, int height)
        {
            if (ctrl != this)
            {
                if (ctrl == panelTop || ctrl == panelTop2)
                {
                    if (m_Resolution == Resolution.FourK)
                        ctrl.Size = new Size(width, height);
                    else if (m_Resolution == Resolution.FullHD) 
                        ctrl.Size = new Size(width, Convert.ToInt32(height * 0.5)); 
                }
                else
                {
                    float sizePer = 1.0f; 
                    if (m_Resolution == Resolution.FullHD) 
                        sizePer = 0.5f; 
                    else if (m_Resolution == SDMS.Resolution.Other) 
                        sizePer = 0.75f; 

                    ctrl.Size = new Size(Convert.ToInt32(width * sizePer), Convert.ToInt32(height * sizePer));

                    if (ctrl is ImageButton)
                    {
                        ImageButton btn = ctrl as ImageButton;
                        FontFamily fontFamily = btn.TextFont.FontFamily;
                        if (btn != null)
                        {
                            if (FormMain.Instance.UseNanumFont)
                                fontFamily = new FontFamily(Program.prgFont);
                            else
                                fontFamily = new FontFamily("굴림"); 
                        }
                        float fontSize = 21.0f;
                        FontStyle fontStyle = FontStyle.Bold;
                        if (FormMain.Instance.Resolution != Resolution.FourK)
                            fontStyle = FontStyle.Regular;
                        

                        btn.TextFont = new Font(fontFamily, fontSize * sizePer, fontStyle, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                    }
                }
            }
              
            foreach (Control childCtrl in ctrl.Controls)
            {
                if (ctrl is ImageComboBox && childCtrl.Name.Contains("_pic"))
                    continue;

                int width2 = -1;
                int height2 = -1;

                if (childCtrl == panelTop || childCtrl == panelTop2)
                {
                    width2 = this.Size.Width;
                    height2 = childCtrl.BackgroundImage.Height;
                }
                else
                {
                    if (m_CtrlSize.ContainsKey(childCtrl))
                    {
                        width2 = m_CtrlSize[childCtrl].Width;
                        height2 = m_CtrlSize[childCtrl].Height;
                    }
                    else
                    {
                        if (childCtrl is ImageButton)
                        {
                            ImageButton imgBtn = childCtrl as ImageButton;
                            if (imgBtn.ImageNormal != null)
                            {
                                width2 = imgBtn.ImageNormal.Width;
                                height2 = imgBtn.ImageNormal.Height;
                            }
                        }
                        else if (childCtrl is PictureBox)
                        {
                            PictureBox pic = childCtrl as PictureBox;
                            if (pic.Image != null)
                            {
                                width2 = pic.Image.Width;
                                height2 = pic.Image.Height;
                            }
                        }
                        else if (childCtrl is Panel)
                        {
                            if (childCtrl.BackgroundImage != null)
                            {
                                width2 = childCtrl.BackgroundImage.Width;
                                height2 = childCtrl.BackgroundImage.Height;
                            }
                        }
                    }
                }

                if (width2 < 0 || height2 < 0)
                    continue;

                dd(childCtrl, width2, height2);
            }
        }

        /// <summary>
        /// Panel Resize
        /// </summary>
        /// <param name="list"></param> 
        /// <param name="nSpaceVer">세로 간격 (없으면 0)</param>
        private void CtrlHeightLineUp(List<Control> list, int nSpaceVer)
        {
            int nIndex = 0;
            foreach (Control childCtrl in list)
            {
                ImageButton imgBtn = childCtrl as ImageButton;
                if (imgBtn == null || imgBtn.ImageNormal == null)
                    continue;

                int width = imgBtn.ImageNormal.Width;
                int height = imgBtn.ImageNormal.Height;
                if (m_Resolution == Resolution.FullHD)
                {
                    width = Convert.ToInt32(width * 0.5);
                    height = Convert.ToInt32(height * 0.5);
                }

                imgBtn.Size = new Size(width, height);
                imgBtn.Location = new Point(0, (nSpaceVer * nIndex) + (height * nIndex));

                nIndex++;
            }
        }
         
        /// <summary>
        /// 가로 정렬 
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="standardHeight"></param>
        /// <param name="x"></param>
        /// <param name="spaceWidth"></param>
        /// <returns>총 사이즈</returns>
        private int CtrlWidthLineUp(Dictionary<int, Control> dic, int standardHeight, int x, int spaceWidth, bool visibleChk = false)
        {
            foreach (Control childCtrl in dic.Values)
            {
                if (visibleChk)
                {
                    if (!childCtrl.Visible)
                        continue;
                }
                 
                // 부모 Panel의 수직 중간점
                int nTopPanel2CenterV = Convert.ToInt32((standardHeight) - (childCtrl.Height * 0.5));
                 
                childCtrl.Location = new Point(x, nTopPanel2CenterV);
                //if (childCtrl is ImageButton)
                //{
                //    ImageButton imgBtn = childCtrl as ImageButton;
                //    int width = imgBtn.ImageNormal.Width;
                //    int height = imgBtn.ImageNormal.Height;

                //    //OtherCtrlResize(imgBtn, width, height);
                //}
                //else if (childCtrl is PictureBox)
                //{
                //    PictureBox pic = childCtrl as PictureBox;
                //    int width = pic.Image.Width;
                //    int height = pic.Image.Height;

                //    //OtherCtrlResize(pic, width, height);
                //}
                //else
                //{
                //    if (m_CtrlSize.ContainsKey(childCtrl))
                //    {
                //        int width = m_CtrlSize[childCtrl].Width;
                //        int height = m_CtrlSize[childCtrl].Height;

                //        OtherCtrlResize(childCtrl, width, height);
                //    } 
                    
                //    if (m_CtrlFontSize.ContainsKey(childCtrl))
                //    {
                //        OtherCtrlFontResize(childCtrl, m_CtrlFontSize[childCtrl]);
                //    }
                //}

                Size imgBtnSize = childCtrl.Size;
                x += imgBtnSize.Width + spaceWidth;

                if (childCtrl == btnLayerNotice || childCtrl == btnSensorMonitor)
                    x += 26;

            }

            return x;
        }

        // A,B,C 중 한개만 유지될 버튼모음 (선택, 이동, 회전)
        private List<ImageButton> LeftPanelContinueBtns = new List<ImageButton>();
        // 해당 팝업이 떠있을 경우 유지될 버튼모음 (테스트, 분할화면, 화면설정, 알림공지, 알람)
        private List<ImageButton> PopupContinueBtns = new List<ImageButton>();
        // A팝업이던지 B팝업이던지 한개의 팝업만 뜨는 버튼모음 (CCTV, 유해물질, 방재장비)
        private List<ImageButton> LeftPanelPopupToggleContinueBtns = new List<ImageButton>();
        // 관리탭 (설비목록, 담당자관리, 메세지관리, 방송관리, 탐지관리, 센서관리, 지진관리, 저장)
        private List<ImageButton> LeftAdminPanelPopupToggleContinueBtns = new List<ImageButton>();
        
        private void SetButtonBackColor(ImageButton button)
        {
            if (LeftPanelContinueBtns.Contains(button))
            {
                foreach (ImageButton item in LeftPanelContinueBtns)
                {
                    if (item == button)
                        item.BackColor = m_OrangeColor;
                    else
                        item.BackColor = Color.Transparent;
                }
            }
            else if (PopupContinueBtns.Contains(button))
            {
                if (button.BackColor == Color.Transparent)
                    button.BackColor = m_OrangeColor;
                else
                    button.BackColor = Color.Transparent;
            }
            else if (LeftPanelPopupToggleContinueBtns.Contains(button))
            {
                foreach (ImageButton item in LeftPanelPopupToggleContinueBtns)
                {
                    if (item == button)
                    {
                        if (button.BackColor == Color.Transparent)
                            button.BackColor = m_OrangeColor;
                        else
                            button.BackColor = Color.Transparent; 
                    }
                    else
                        item.BackColor = Color.Transparent;
                }
            }
            else if (LeftAdminPanelPopupToggleContinueBtns.Contains(button))
            {
                foreach (ImageButton item in LeftAdminPanelPopupToggleContinueBtns)
                {
                    int btnID = GetButtonID(item); 
                    SetImageLeftAdminBtn(btnID, item == button);     
                }
            } 
            else
            {
                
            }
        }

        // 관리탭 메뉴 이미지
        private Image imgSensorListDefault = global::SDMS.Properties.Resources.DeviceList_Default;
        private Image imgSensorListClick = global::SDMS.Properties.Resources.DeviceList_Click;
        private Image imgSensorMgrDefault = global::SDMS.Properties.Resources.SensorMgr_Default;
        private Image imgSensorMgrClick = global::SDMS.Properties.Resources.SensorMgr_Click;
        private Image imgManageDetectDefault = global::SDMS.Properties.Resources.DetectManag_Default;
        private Image imgManageDetectClick = global::SDMS.Properties.Resources.DetectManag_Click;
        private Image imgAdminManageDefault = global::SDMS.Properties.Resources.AdminManage_Default;
        private Image imgAdminManageClick = global::SDMS.Properties.Resources.AdminManage_Click;
        private Image imgMessageManageDefault = global::SDMS.Properties.Resources.MessageManage_Default;
        private Image imgMessageManageClick = global::SDMS.Properties.Resources.MessageManage_Click;
        private Image imgManageBroadcastDefault = global::SDMS.Properties.Resources.Broadcast_Default;
        private Image imgManageBroadcastClick = global::SDMS.Properties.Resources.Broadcast_Click;
        private Image imgManageEarthquakeDefault = global::SDMS.Properties.Resources.EarthquakeMgr_Default;
        private Image imgManageEarthquakeClick = global::SDMS.Properties.Resources.Earthquake_Click;
         
        public void SetImageLeftAdminBtn(int btnID, bool isCheck)
        {
            if (btnID == ID.ID_SHOW_LIST_FACILITY)  // 모든설비목록
            {
                if (isCheck)
                {
                    btnShowList.ImageNormal = imgSensorListClick;
                    btnShowList.Enabled = false;
                }
                else
                {
                    btnShowList.ImageNormal = imgSensorListDefault;
                    btnShowList.Enabled = true;
                }


                btnShowList.ImageMouseOver = imgSensorListClick;
                btnShowList.ImageClicked = imgSensorListClick;
                btnShowList.Refresh();
            }
            else if (btnID == ID.ID_MANAGE_SENSOR) // 센서동작관리
            {
                if (isCheck)
                {
                    btnSensorMgr.ImageNormal = imgSensorMgrClick;
                    btnSensorMgr.Enabled = false;
                }
                else
                {
                    btnSensorMgr.ImageNormal = imgSensorMgrDefault;
                    btnSensorMgr.Enabled = true;
                }
                 
                btnSensorMgr.ImageMouseOver = imgSensorMgrClick;
                btnSensorMgr.ImageClicked = imgSensorMgrClick;
                btnSensorMgr.Refresh();
            } 
            else if (btnID == ID.ID_MANAGE_DETECT) // 탐지관리
            {
                if (isCheck)
                {
                    btnManageDetect.ImageNormal = imgManageDetectClick;
                    btnManageDetect.Enabled = false;
                }
                else
                {
                    btnManageDetect.ImageNormal = imgManageDetectDefault;
                    btnManageDetect.Enabled = true;
                }                    

                btnManageDetect.ImageMouseOver = imgManageDetectClick;
                btnManageDetect.ImageClicked = imgManageDetectClick;
                btnManageDetect.Refresh();
            }
            else if (btnID == ID.ID_MANAGE_MANAGER) // 담당자관리
            {
                if (isCheck)
                {
                    btnManageManager.ImageNormal = imgAdminManageClick;
                    btnManageManager.Enabled = false;
                }
                else
                {
                    btnManageManager.ImageNormal = imgAdminManageDefault;
                    btnManageManager.Enabled = true;
                }

                btnManageManager.ImageMouseOver = imgAdminManageClick;
                btnManageManager.ImageClicked = imgAdminManageClick;
                btnManageManager.Refresh();
            }
            else if (btnID == ID.ID_MANAGE_MESSAGE) // 메시지관리
            {
                if (isCheck)
                {
                    btnManageSMS.ImageNormal = imgMessageManageClick;
                    btnManageSMS.Enabled = false;
                }
                else
                {
                    btnManageSMS.ImageNormal = imgMessageManageDefault;
                    btnManageSMS.Enabled = true;
                }

                btnManageSMS.ImageMouseOver = imgMessageManageClick;
                btnManageSMS.ImageClicked = imgMessageManageClick;
                btnManageSMS.Refresh();
            }
            else if (btnID == ID.ID_MANAGE_BROADCAST) // 방송관리
            {
                if (isCheck)
                {
                    btnManageBroadcast.ImageNormal = imgManageBroadcastClick;
                    btnManageBroadcast.Enabled = false;
                }
                else
                {
                    btnManageBroadcast.ImageNormal = imgManageBroadcastDefault;
                    btnManageBroadcast.Enabled = true;
                }

                btnManageBroadcast.ImageMouseOver = imgManageBroadcastClick;
                btnManageBroadcast.ImageClicked = imgManageBroadcastClick;
                btnManageBroadcast.Refresh();
            }
            else if (btnID == ID.ID_MANAGE_EARTHQUAKE) // 지진관리
            {
                if (isCheck)
                {
                    btnEarthquake.ImageNormal = imgManageEarthquakeClick;
                    btnEarthquake.Enabled = false;
                }
                else
                {
                    btnEarthquake.ImageNormal = imgManageEarthquakeDefault;
                    btnEarthquake.Enabled = true;
                }

                btnEarthquake.ImageMouseOver = imgManageEarthquakeClick;
                btnEarthquake.ImageClicked = imgManageEarthquakeClick;
                btnEarthquake.Refresh();
            }
        }

        private bool m_bCmbLocBtm = false;
         
        private void ResizePanels()
        {
            int nHeight = 48;

            if (this.Size.Width > 1400)
            {
                //panelMiddle.Size = new Size(this.Size.Width, nHeight);
                //panelProcessHistory.Size = panelMiddle.Size;
                //panelReactionHistory.Size = panelMiddle.Size;
                //pnDetectPSM.Size = panelMiddle.Size;
                //pnNotOperationPSM.Size = panelMiddle.Size;
                //pnActionPSM.Size = panelMiddle.Size;
                //pnSMSPSM.Size = panelMiddle.Size;


                //panelMiddle.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
                //panelLeft.Location = new Point(0, panelMiddle.Location.Y + panelMiddle.Size.Height);

                if (!m_isThumbnailMode)//|| (m_isThumbnailMode && m_cctvMode == CCTVMode.NORMAL))
                {
                    //panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - panelMiddle.Size.Height);

                    //if (mCurrentTab != UnE.View.Content.ContentOwnerTab.REPORT_TAB)
                    //    panelLeft.Show();

                    int nBottomHeight = panelLeft2.Size.Height;
                    if (m_IsDisaster && mCurrentTab == UnE.View.Content.ContentOwnerTab.M3D_TAB)
                    {
                        panelBottom.Location = new Point(panelStatus.Location.X, panelStatus.Location.Y + panelStatus.Height);
                    }
                    else
                        panelBottom.Location = new Point(panelLeft2.Location.X + panelLeft2.Size.Width, panelLeft2.Location.Y);

                    //panelBottom.Location = new Point(panel_mainBackV.Location.X, panel_mainBackV.Location.Y);
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }
                else
                {
                    //panelLeft.Hide();
                    int nBottomHeight = panelLeft2.Size.Height;
                    if (m_IsDisaster && mCurrentTab == UnE.View.Content.ContentOwnerTab.M3D_TAB)
                    {
                        panelBottom.Location = new Point(panelStatus.Location.X, panelStatus.Location.Y + panelStatus.Height);
                    }
                    else
                        panelBottom.Location = new Point(panelLeft2.Location.X + panelLeft2.Size.Width, panelLeft2.Location.Y);
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }
                m_bCmbLocBtm = false;
            }
            else
            {
                //panelMiddle.Size = new Size(this.Size.Width, nHeight * 2);
                //panelProcessHistory.Size = panelMiddle.Size;
                //panelReactionHistory.Size = panelMiddle.Size;
                //pnDetectPSM.Size = panelMiddle.Size;
                //pnNotOperationPSM.Size = panelMiddle.Size;
                //pnActionPSM.Size = panelMiddle.Size;
                //pnSMSPSM.Size = panelMiddle.Size;


                //panelMiddle.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
                //panelLeft.Location = new Point(0, panelMiddle.Location.Y + panelMiddle.Size.Height);

                if (!m_isThumbnailMode || (m_isThumbnailMode && m_cctvMode == CCTVMode.NORMAL))
                {
                    //panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - nHeight);

                    //if (mCurrentTab != UnE.View.Content.ContentOwnerTab.REPORT_TAB)
                    //    panelLeft.Show();

                    int nBottomHeight = panelLeft2.Size.Height;
                    if (m_IsDisaster && mCurrentTab == UnE.View.Content.ContentOwnerTab.M3D_TAB)
                    {
                        panelBottom.Location = new Point(panelStatus.Location.X, panelStatus.Location.Y + panelStatus.Height);
                    }
                    else
                        panelBottom.Location = new Point(panelLeft2.Location.X + panelLeft2.Size.Width, panelLeft2.Location.Y);
                    
                    //panelBottom.Location = new Point(panelLeft.Location.X, panelLeft.Location.Y);  
                    panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
                }
                else
                {
                    //panelLeft.Hide();

                    int nBottomHeight = panelLeft2.Size.Height;
                    if (m_IsDisaster && mCurrentTab == UnE.View.Content.ContentOwnerTab.M3D_TAB)
                    {
                        panelBottom.Location = new Point(panelStatus.Location.X, panelStatus.Location.Y + panelStatus.Height);
                    }
                    else
                        panelBottom.Location = new Point(panelLeft2.Location.X + panelLeft2.Size.Width, panelLeft2.Location.Y);
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
             
            //cmbFireDetect.Location = new Point(632, cmbFireDetect.Location.Y);

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
                panel_mainBackV.Size = new Size(panel_mainBackV.Size.Width, this.Size.Height - panelTop.Size.Height - panelMiddle.Size.Height);
                panel_mainBackV.Show();

                int nBottomHeight = panel_mainBackV.Size.Height;
                panelBottom.Location = new Point(panel_mainBackV.Location.X + panel_mainBackV.Size.Width, panel_mainBackV.Location.Y);
                panelBottom.Size = new Size(this.Size.Width - panelBottom.Location.X, nBottomHeight);
            }
            else
            {
                //panel_mainBackV.Size = new Size(panel_mainBackV.Size.Width, this.Size.Height - panelTop.Size.Height - panelMiddle.Size.Height);
                panel_mainBackV.Hide();

                int nBottomHeight = panel_mainBackV.Size.Height;
                panelBottom.Location = new Point(panel_mainBackV.Location.X, panel_mainBackV.Location.Y);
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

        private void GetResolution()
        {
            int curMonitorIndex = 0;
            for (int i = 0; i < Screen.AllScreens.Count(); i++)
            {
                Screen sc2 = Screen.AllScreens[i];

                Size MainFrameSize = MainFrame.Size;

                //if (MainFrameSize.Width * 0.5 >= sc2.Bounds.X && sc2.Bounds.X + sc2.Bounds.Width > MainFrameSize.Width * 0.5 &&
                //    MainFrameSize.Height * 0.5 >= sc2.Bounds.Y && sc2.Bounds.Y + sc2.Bounds.Height > MainFrameSize.Height * 0.5)
                if (MainFrame.Location.X >= sc2.Bounds.X && sc2.Bounds.X + sc2.Bounds.Width > MainFrame.Location.X &&
                    MainFrame.Location.Y >= sc2.Bounds.Y && sc2.Bounds.Y + sc2.Bounds.Height > MainFrame.Location.Y)  
                {
                    //System.Diagnostics.Trace.WriteLine("Current Monitor is " + (i + 1).ToString());
                    curMonitorIndex = i;
                    break;
                }
            }

            m_Resolution = Resolution.FullHD;
            /*Screen sc = Screen.AllScreens[curMonitorIndex];
            if (sc.Bounds.Width == 1920 && sc.Bounds.Height == 1080)
            {
                m_Resolution = Resolution.FullHD;
            }
            else if (sc.Bounds.Width == 3840 && sc.Bounds.Height == 2160)
            {
                m_Resolution = Resolution.FourK;
            }
            else
            {
                m_Resolution = Resolution.Other;
            } */
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (this.Size.Width == 0 || this.Size.Height == 0)
                return;

            GetResolution();
            ResizeControls();

            ResizePanels();
            ResizeReportMenu(); 
            //ResizeComboBox(); 
             
            //m_PageHome.SetBounds(0, 0, panelBottom.Size.Width, panelLeft.Size.Height);

            ResizeSystemButtons();
//            ResizeButtons();

            ClearSelectDlg();

            if (m_uiManager != null)
                m_uiManager.OnResize();
        }

        public void ClearSelectDlg()
        { 
            if (DlgSelectCase.Instance.DetectFireCount == 0 && m_readyToReceiveMessage == true)
            {

                //MessageBox.Show("lgSelectCase.Instance.Visible" + DlgSelectCase.Instance.Visible);
                DlgSelectCase.Instance.Visible = false;
            }
        }

        private Dictionary<PopupDialog.Report.ReportCategory, PopupDialog.Report.FormReportMenu> m_dicReportMenus = new Dictionary<PopupDialog.Report.ReportCategory, PopupDialog.Report.FormReportMenu>();
        /*private SDMS.PopupDialog.Report.FormReportMenu fireMenu = null;
        private SDMS.PopupDialog.Report.FormReportMenu psmMenu = null;
        private SDMS.PopupDialog.Report.FormReportMenu securityMenu = null;*/

        private void ResizeReportRibbonBar()
        { 

        }

        private void ArrangeRibbonButton(ImageButton btnPrev, ImageButton btnNext)
        {
            btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width, btnPrev.Location.Y);
        }  

        private void ArrangeButtonHorizontal(ImageButton btnPrev, ImageButton btnNext, int horizontal)
        {
            btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width + horizontal, btnPrev.Location.Y);
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

            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (m_frmMessageReceiver == null || m_frmMessageReceiver.IsDisposed)
                        m_frmMessageReceiver = new PopupDialog.FormMessageReceiver();

                    PopupDialog.FormMessageReceiver.ReadNewMessage(ref m_frmMessageReceiver, m_dbMgr, nMessageID);
                });
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("ReadSDMSMessage Error : " + e.Message);
            }
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
            {
                m_frmMessageReceiver.SetChildCtrlResize(m_frmMessageReceiver, 408, 400);   
                
                m_frmMessageReceiver.Show(this);
            }

            Point pt = new Point(btnSendMessage.Location.X + m_frmMessageSender.Width, panelTop2.Location.Y + panelTop2.Height);
            Point ptScr = this.PointToScreen(pt);
            m_frmMessageReceiver.Location = ptScr;// PointToClient(ptScr); 
        }

        private void LoadSDMSDBOptions()
        {
            if (m_bLoadDBOption)
                return;

            string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='HiddenClock' AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                {
                    m_bHiddenClock = strValue == "1";
                }
            }

            strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='PopupSensorOn' AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                {
                    m_usePopupSensorOn = strValue == "1";
                }
            }

            strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='MovingText' AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                {
                    m_useMovingText = strValue == "1";
                }
            }

            string strUseBulletIn = "UseBulletIn", strUseMissionStatus = "UseMissionStatus";
            bool useBulletIn = true, useMissionStatus = true;

            strSQL = "SELECT PropertyName, PropertyValue FROM OptionSDMS where (PropertyName ='" + strUseBulletIn + "' or PropertyName ='" + strUseMissionStatus + "') AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null)
            {
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i+=2)
                {
                    string strName = WebDBManager.GetStringField(arrResult[i], "");
                    string strValue = WebDBManager.GetStringField(arrResult[i + 1], "");

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

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                    strButtonText = strValue;


                strValue = WebDBManager.GetStringField(arrResult[1], "");

                if (strValue != "" && strValue != "null")
                    strButtonTooltip = strValue;

            }
            else
            {
                strSQL = String.Format("INSERT INTO OptionSDMS (PropertyName, PropertyValue, Description, SiteID) VALUES ('HomeButton_{0}', '#{0}', '{1} 초기 화면', {2})"
                    , nIndex
                    , (nIndex == 1 ? "전체" : (nIndex == 2 ? "1발전소" : (nIndex == 3 ? "2발전소" : "저탄장")))
                    , UnE.SOP.ProxySOP.Instance.SiteID);

                if (m_dbMgr.GetResultData(strSQL) != null)
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

        //핫키등록
        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(int hwnd, int id, int fsModifiers, int vk);

        //핫키제거
        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(int hwnd, int id);

        private void FormMain_Load(object sender, EventArgs e)
        {
            //F1:매뉴얼, F2:리모콘Visible, F3:리모콘 탭이동 
            RegisterHotKey((int)this.Handle, 0, 0x0, (int)Keys.F1);

            // 아직 OnReadyDataLoad()가 호출되지 않았다면...
            if (m_readyDataLoad && m_netMgr == null)
                OnReadyDataLoad();

            
            Point ptOriginMainFrame = MainFrame.Location;
            MainFrame.Visible = false;
            MainFrame.Location = ptOriginMainFrame;

            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                pictureBoxCCTV.Visible = false;
                if(m_bUse2D == true)
                {
                   // pictureBoxReport.Location = pictureBoxAdmin.Location;
                    //pictureBoxAdmin.Location = pictureBoxCCTV.Location;                  
                }
                else
                {
                    //pictureBoxAdmin.Location = pictureBoxReport.Location;
                    //pictureBoxReport.Location = pictureBoxCCTV.Location;
                }
            }

            //m_PaneBtnHome.TopLevel = false;
            //m_PaneBtnHome.Parent = this;

            SDMS.FormFrame.Instance.LocationChanged += new System.EventHandler(this.FormMain_LocationChanged);
            LoadSDMSDBOptions();

            SetMonitorForm(/*MainFrame, */m_nMonitor);

            MainFrame.Visible = false;
            FormMain.Instance.Visible = false;

            m_splashManager.SendSplashMessage("3D 화면 초기화...", libSplash.Message.SPLASH_MESSAGE, 60256);
            m_PageHome.Init3DView();
            
            InitPanels();
            //InitButtons();
            InitComboBox();
            m_PageHome.FrmReport.SetComboText(proc_btnStartDate.Text, proc_btnEndDate.Text);
            

            SelectMonitoringTab(); 
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
                //labelTitle.Text = "연습용 모드";
            }
            else
            {
                //labelTitle.Text = "";
            }

            //FormMain.Instance.Visible = true;
            //MainFrame.Visible = true;


            FormMain_LocationChanged(null, null);
            FormMain_LocationChanged(null, null);
             
            //m_netMgr = NetworkManager.Instance;

            int x = panelMiddle.Location.X + panelMiddle.Size.Width - m_frmWeather.Size.Width + FormFrame.Instance.Location.X;
            int y = panelMiddle.Location.Y + panelMiddle.Size.Height + FormFrame.Instance.Location.Y;
            m_frmWeather.StartPosition = FormStartPosition.Manual;
            m_frmWeather.Location = new Point(x, y);

            InitButtons();

            if (UnE.SOP.ProxySOP.Instance.SiteID == 999 || UnE.SOP.ProxySOP.Instance.SiteID == 102)
                m_PageHome.ContentForm.AddMainToolStrip(null, UnE.View.Content.ViewType.OUTSIDE);

            this.panelTop.DoubleClick += new System.EventHandler(this.panelTop_DoubleClick);
            this.panelTop.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseDown);
            this.panelTop.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseMove);
            this.panelTop.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelTop_MouseUp);

            btnReportTab.Enabled = m_useReport;

            //OnReadyDataLoad();
            m_PanelBtnNotice.chgSensorDectect += m_PaneBtnNotice_chgSensorDectect;

            UnE.SOP.ProxySOP.Instance.SOPDisasterContainer = (UnE.SOP.IDisasterContainer)this.PageHome.ContentForm;

            InitReportMenuButtons();

            //m_uiManager = libExternalUI.Factory.GetUIManager(this);
#if SAFE_KOREA_YH_2017
            RunInternalMessagePopup();
#endif
        }

        // 모니터 출력을 지정
        private void SetMonitors()
        {
            int nCCTV = 3, nDisaster = 1;
            string szCCTVForm = RegUtil.ReadRegValue("Monitor Info", "CCTV", UnE.SOP.ProxySOP.Instance.SiteID);
            if (szCCTVForm == null || szCCTVForm == "")
                szCCTVForm = DBManager.LoadIni("CCTVForm", "Monitor Info");
            int.TryParse(szCCTVForm, out nCCTV);

            UnE.SOP.ProxySOP.Instance.CCTVMontior = nCCTV;

            string szDisaster = RegUtil.ReadRegValue("Monitor Info", "SDMS", UnE.SOP.ProxySOP.Instance.SiteID);
            if (szDisaster == null || szDisaster == "")
                szDisaster = DBManager.LoadIni("DisasterSystem", "Monitor Info");
            int.TryParse(szDisaster, out nDisaster);

            m_nMonitor = nDisaster;
        }

        private void RunInternalMessagePopup()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'UseInternalMessagePopup' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strValue = WebDBManager.GetStringField(arrResult[0]);

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

        private bool m_readyDataLoad = false;

        public void OnReadyDataLoad()
        {
            m_readyDataLoad = true;

            if (MainFrame == null)
                return;

            m_splashManager.SendSplashMessage("POI 로딩중...", libSplash.Message.SPLASH_MESSAGE, 84615);
            m_PageHome.LoadPOI();
            int nLayerState = ReadLayerState();
            InitLayerButtonCheck(nLayerState);
            m_splashManager.SendSplashMessage("나머지 DB 데이터 불러오기...", libSplash.Message.SPLASH_MESSAGE, 78000);
            m_proxyMessenger.OnAfterLoadingCCTV();
            LoadExtraData();
            m_netMgr = NetworkWebManager.Instance;

            // 초기화가 끝났으므로 SOP Server와 통신을 개시한다.
            m_netMgr.WaitForSOPServer = false;

            m_PageHome.ContentForm.RedrawWindow();

            // SOP Simulator가 꺼져있는동안 새로운 메시지가 수신된 것이 있는지 확인한다.
            PopupDialog.FormMessageReceiver.ReadNewMessage(ref m_frmMessageReceiver, m_dbMgr);
            // 이미 삭제된 DB Data를 가지고 있는지 확인한다.
            PopupDialog.FormMessageReceiver.CheckDeletedIDs(m_dbMgr);
            m_readyToReceiveMessage = true;

            ShowToolbar();

            SendUnityProcessIDs();

            m_splashManager.SendSplashMessage("", libSplash.Message.SPLASH_CLOSE, 78000);
            MainFrame.Location = FormFrame.Instance.OriginLocation;
            MainFrame.WindowState = FormWindowState.Normal;
            MainFrame.Visible = true;
            
            // Form 로딩후 잘못된 위치에 Form이 위치하는 경우가 생겨
            // 이를 보정하기 위하여 임시 Timer를 사용한다.
            Timer timerLocation = new Timer();
            timerLocation.Interval = 100;
            timerLocation.Tick += timerLocation_Tick;
            timerLocation.Start();

            /*if (ProxyMessenger.OnlySDMS() == false)
                ProxyMessenger.ShowSOPSimulator();*/
        }

        private void SendUnityProcessIDs()
        {
            List<int> processIDs = new List<int>();

            if (m_PageHome.ContentForm.OutdoorView != null && m_PageHome.ContentForm.OutdoorView is Panel4Unity)
            {
                Panel4Unity panel = (Panel4Unity)m_PageHome.ContentForm.OutdoorView;
                int nUnityProcessID = panel.ProcessID;

                if (nUnityProcessID > 0)
                    processIDs.Add(nUnityProcessID);
            }

            if (m_PageHome.ContentForm.IndoorView != null && m_PageHome.ContentForm.IndoorView is Panel4Unity)
            {
                Panel4Unity panel = (Panel4Unity)m_PageHome.ContentForm.IndoorView;
                int nUnityProcessID = panel.ProcessID;

                if (nUnityProcessID > 0)
                    processIDs.Add(nUnityProcessID);
            }

            if (processIDs.Count > 0)
            {
                ProxyMessenger.SetViewProcessID(processIDs);
            }
        }

        private void CloseUnityProcess()
        {
            CloseUnityProcess(m_PageHome.ContentForm.OutdoorView);
            CloseUnityProcess(m_PageHome.ContentForm.IndoorView);
        }

        private void CloseUnityProcess(ISensorTooltipOwner tooltip)
        {
            if (tooltip != null && tooltip is Panel4Unity)
            {
                Panel4Unity panel = (Panel4Unity)tooltip;
                int nUnityProcessID = panel.ProcessID;

                if (nUnityProcessID > 0)
                {
                    Process process = Process.GetProcessById(nUnityProcessID);

                    if (process != null)
                        process.Kill();
                }
            }
        }

        /// <summary>
        ///  로드가 끝난 후 활성화된 폼이 있으면 해당 버튼 표시 (CCTV, 유해물질, 방재장비)
        /// </summary> 
        public void LoadActiveFormInDockingMode(ContentOwnerTabRightDockingMode mode)
        {
            if (mode == ContentOwnerTabRightDockingMode.NONE)
                return;

            ImageButton button = null;

            if (mode == ContentOwnerTabRightDockingMode.SHOW_CCTV)
                button = btnMultiCCTV;
            else if (mode == ContentOwnerTabRightDockingMode.SHOW_DISASTER)
                button = btnDisasterPrevention;
            else if (mode == ContentOwnerTabRightDockingMode.SHOW_PSM)
                button = btnPSMStatus;

            SetButtonBackColor(button);
        }

        private void timerLocation_Tick(object sender, EventArgs e)
        { 
            MainFrame.Location = FormFrame.Instance.OriginLocation;
            MainFrame.WindowState = FormWindowState.Maximized;
            ((Timer)sender).Stop();

            GetResolution();
        }

        private void InitPanels()
        {
            //panelProcessHistory.Location = new Point(0, 0);
            //panelReactionHistory.Location = panelMiddle.Location;
            //pnDetectPSM.Location = panelMiddle.Location;
            //pnNotOperationPSM.Location = panelMiddle.Location;
            //pnActionPSM.Location = panelMiddle.Location;
            //pnSMSPSM.Location = panelMiddle.Location;

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

            react_btnStartDate.ButtonText = DateTime.Now.AddDays(-6).ToString().Substring(0, 10);
            react_btnEndDate.ButtonText = DateTime.Now.ToString().Substring(0, 10);

            btnActionPSMStartDate.ButtonText = DateTime.Now.AddDays(-6).ToString().Substring(0, 10);
            btnActionPSMEndDate.ButtonText = DateTime.Now.ToString().Substring(0, 10);


            react_cboSearchTypeFire.Items.Add("화재신고만");
            react_cboSearchTypeFire.Items.Add("오작동 처리 포함");
            react_cboSearchTypeFire.Items.Add("현장에서 꺼진 신호 포함");

            react_cboSearchTypeIntrusion.Items.Add("방범신고만");
            react_cboSearchTypeIntrusion.Items.Add("오작동 처리 포함");
            react_cboSearchTypeIntrusion.Items.Add("현장에서 꺼진 신호 포함");

            react_cboSearchTypeEarthquake.Items.Add("시스템복구");

            cboActionPSMSearchType.Items.Add("누출신고만");
            cboActionPSMSearchType.Items.Add("누출신고 및 시스템복구");
            cboActionPSMSearchType.Items.Add("누출신고 및 현장복구");
            cboActionPSMSearchType.Items.Add("모든 신호");

            react_cboSearchTypeTH.Items.Add("온/습도 신고만");
            react_cboSearchTypeTH.Items.Add("오작동 처리 포함");
            react_cboSearchTypeTH.Items.Add("현장에서 꺼진 신호 포함");

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

        private Color m_OrangeColor = Color.FromArgb(0xF7, 0xA9, 0x2B);
        private void InitButtons()
        {
            string strHomeButtonText_1 = string.Empty, strHomeButtonText_2 = string.Empty, strHomeButtonText_3 = string.Empty, strHomeButtonText_4 = string.Empty,
                strHomeButtonToolTip_1 = string.Empty, strHomeButtonToolTip_2 = string.Empty, strHomeButtonToolTip_3 = string.Empty, strHomeButtonToolTip_4 = string.Empty;

            TextData data = new TextData();
            data.Brush = new SolidBrush(Color.White);
            data.Text = "재난신고";
            data.Rectangle = new Rectangle(5, 65, 60, 12);

            //btnFire.ExtraImage = global::SDMS.Properties.Resources.Fire_Icon;
            //btnFire.X = 20;
            //btnFire.Y = 5; 

            //btnFire.Text = data.Text;
            lblFireText.Text = data.Text;

            GetHomeButtonText(1, out strHomeButtonText_1, out strHomeButtonToolTip_1);
            GetHomeButtonText(2, out strHomeButtonText_2, out strHomeButtonToolTip_2);
            GetHomeButtonText(3, out strHomeButtonText_3, out strHomeButtonToolTip_3);
            GetHomeButtonText(4, out strHomeButtonText_4, out strHomeButtonToolTip_4);

            //m_toolbar = new PopupDialog.FloatingToolbar2();
            /// Toolbar
            //if (m_toolbar != null)
            //{
            //    m_toolbar.SetHomeButtonText(1, strHomeButtonText_1);
            //    m_toolbar.SetHomeButtonText(2, strHomeButtonText_2);
            //    m_toolbar.SetHomeButtonText(3, strHomeButtonText_3);
            //    m_toolbar.SetHomeButtonText(4, strHomeButtonText_4);
            //}

            /// 가로 바  
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

            // 버튼에 표기되는 최대 글자 수는 6개로 고정(나머지는 짤림)
            if (UnE.SOP.ProxySOP.Instance.SiteID == 3)
            {
                m_PaneBtnSaveHome.BtnMainHome.Text = strHomeButtonText_1;
                m_PaneBtnSaveHome.Btn14Home.Text = strHomeButtonText_2;
                m_PaneBtnSaveHome.Btn56Home.Text = strHomeButtonText_3;
                m_PaneBtnSaveHome.BtnCoalHome.Text = strHomeButtonText_4;                
            }
            else
            {
                m_PaneBtnSaveHome.BtnMainHome.Text = strHomeButtonText_1.Length > 6 ? strHomeButtonText_1.Substring(0, 6) : strHomeButtonText_1;
                m_PaneBtnSaveHome.Btn14Home.Text = strHomeButtonText_2.Length > 6 ? strHomeButtonText_2.Substring(0, 6) : strHomeButtonText_2;
                m_PaneBtnSaveHome.Btn56Home.Text = strHomeButtonText_3.Length > 6 ? strHomeButtonText_3.Substring(0, 6) : strHomeButtonText_3;
                m_PaneBtnSaveHome.BtnCoalHome.Text = strHomeButtonText_4.Length > 6 ? strHomeButtonText_4.Substring(0, 6) : strHomeButtonText_4;
            }

            m_PaneBtnSaveHome.SetButtonVisible();

            m_PaneBtnSaveHome.BtnMainHome.Click += new System.EventHandler(this.btnSaveHomeSub_Click);
            m_PaneBtnSaveHome.Btn14Home.Click += new System.EventHandler(this.btnSaveHomeSub_Click);
            m_PaneBtnSaveHome.Btn56Home.Click += new System.EventHandler(this.btnSaveHomeSub_Click);
            m_PaneBtnSaveHome.BtnCoalHome.Click += new System.EventHandler(this.btnSaveHomeSub_Click);
              
            //초기화면(1),전체화면(1),선택(1),이동(0),회전(1),확대(1),축소(1),3D(0),3D/2D(0),2D(0),CCTV(1),화면캡쳐(1),테스트(1),유해물질(1)
            // 신규 
            SetButtonID(btnFullScreen, ID.ID_VIEW_HOME_MAIN, "전체 화면");
            //SetButtonID(m_PaneBtnHome.BtnMainHome, ID.ID_VIEW_HOME_MAIN, strHomeButtonToolTip_1);
            SetButtonID(m_PaneBtnHome.Btn14Home, ID.ID_VIEW_HOME_14, strHomeButtonToolTip_2);
            SetButtonID(m_PaneBtnHome.Btn56Home, ID.ID_VIEW_HOME_56, strHomeButtonToolTip_3);
            SetButtonID(m_PaneBtnHome.BtnCoalHome, ID.ID_VIEW_HOME_COAL, strHomeButtonToolTip_4);

            SetButtonID(btnHome, ID.ID_VIEW_HOME, "분할 화면");
            SetButtonID(btnPick, ID.ID_VIEW_PICK, "선택");            
            SetButtonID(btnPanning, ID.ID_VIEW_PAN, "화면 이동");            
            SetButtonID(btnOrbit, ID.ID_VIEW_ORBIT, "화면 회전");            
            SetButtonID(btnZoomIn, ID.ID_VIEW_ZOOMIN, "확대");
            SetButtonID(btnZoomOut, ID.ID_VIEW_ZOOMOUT, "축소");
            if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
                SetButtonID(btnCampus, ID.ID_VIEW_CAMPUS, "지도 전환");
            SetButtonID(btnOutside, ID.ID_VIEW_OUTSIDE, "외부공간 보기");
            SetButtonID(btnBoth, ID.ID_VIEW_BOTHSIDE, "외부/실내 같이 보기");
            btnBoth.Enabled = false;
            SetButtonID(btnInside, ID.ID_VIEW_INSIDE, "실내공간 보기");
            btnInside.Enabled = false;
            SetButtonID(btnMultiCCTV, ID.ID_VIEW_CCTV, "CCTV 크게 보기");
            SetButtonID(btnScreenShot, ID.ID_VIEW_SCREENSHOT, "화면 캡쳐"); 
            SetButtonID(btnSimulator, ID.ID_VIEW_SIMULATOR, "센서 시뮬레이터 기동");
            SetButtonID(btnPSMStatus, ID.ID_VIEW_PSM, "유해 화학물질 리스트 보기");
            SetButtonID(btnDisasterPrevention, ID.ID_VIEW_DISASTER, "방재장비 관리");

            SetButtonID(btn3DTab, ID.ID_TAB_3D, "3D");
            SetButtonID(btnAdminTab, ID.ID_TAB_MANAGE, "관리");
            SetButtonID(btnReportTab, ID.ID_TAB_REPORT, "리포트");

            // 2018 디자인 시안의 버튼 순서대로
            m_dicOptionSDMSNameButtons.Add("선택", btnPick);
            m_dicOptionSDMSNameButtons.Add("이동", btnPanning);
            m_dicOptionSDMSNameButtons.Add("회전", btnOrbit);
            m_dicOptionSDMSNameButtons.Add("확대", btnZoomIn);
            m_dicOptionSDMSNameButtons.Add("축소", btnZoomOut);
            m_dicOptionSDMSNameButtons.Add("CCTV", btnMultiCCTV);
            m_dicOptionSDMSNameButtons.Add("테스트", btnSimulator);
            m_dicOptionSDMSNameButtons.Add("화면캡쳐", btnScreenShot);
            m_dicOptionSDMSNameButtons.Add("유해물질", btnPSMStatus);
            m_dicOptionSDMSNameButtons.Add("방재장비", btnDisasterPrevention);
            m_dicOptionSDMSNameButtons.Add("전체화면", btnFullScreen);
            m_dicOptionSDMSNameButtons.Add("분할화면", btnHome);
            m_dicOptionSDMSNameButtons.Add("3D", btnOutside);
            m_dicOptionSDMSNameButtons.Add("3D/2D", btnBoth);
            m_dicOptionSDMSNameButtons.Add("2D", btnInside);
            m_dicOptionSDMSNameButtons.Add("리모콘", btnShowRemote);
                    
            if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
                m_dicOptionSDMSNameButtons.Add("지도전환", btnCampus);

            LeftPanelContinueBtns.Add(btnPick);
            LeftPanelContinueBtns.Add(btnPanning);
            LeftPanelContinueBtns.Add(btnOrbit);

            //PopupContinueBtns.Add(btnSimulator);
            PopupContinueBtns.Add(btnHome);
            PopupContinueBtns.Add(btnSaveHome);
            PopupContinueBtns.Add(btnSendMessage);
            PopupContinueBtns.Add(btnLayerNotice);
            
            if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
                PopupContinueBtns.Add(btnCampus);

            LeftPanelPopupToggleContinueBtns.Add(btnMultiCCTV); 
            LeftPanelPopupToggleContinueBtns.Add(btnPSMStatus);
            LeftPanelPopupToggleContinueBtns.Add(btnDisasterPrevention);


            m_PageHome.OnClickToolBarButton(m_PaneBtnHome.BtnMainHome);
            //m_PageHome.OnClickToolBarButton(btnPick);
            OnClickToolBarButton(btnPick, null);
            btnSimulator.Click += new System.EventHandler(this.OnClickToolBarButton);
            
            //if (!GetWeatherInfoOption())
            //    btnWeatherInfo.Visible = false;

            //CheckButton(btnOrbit, true);
            CheckButton(btnOutside, true);
            //////////////////////////////////////////

            /// 세로바(Layer)
            //SetButtonID(btnLayerFire, ID.ID_LAYER_DETECTOR);
            //SetButtonID(btnLayerSpringCooler, ID.ID_LAYER_COOLER);
            //SetButtonID(btnLayerPump, ID.ID_LAYER_PERSURE);
            SetButtonID(btnLayerCCTV, ID.ID_LAYER_CCTV);
            //SetButtonID(btnLayerFE, ID.ID_LAYER_FIREEXT);
            //SetButtonID(btnLayerHD, ID.ID_LAYER_FIREHYD);
            //SetButtonID(btnLayerFA, ID.ID_LAYER_ALARMSTA);
            //SetButtonID(btnLayerFR, ID.ID_LAYER_RECIVER);
            //SetButtonID(btnLayerLowCCTV, ID.ID_LAYER_CCTVLOW);
            //SetButtonID(btnLayerCCTVDisconnected, ID.ID_LAYER_CCTV_DISCONNECTED);
            SetButtonID(btnLayerBuildingText, ID.ID_LAYER_BUILDING_TEXT);
            SetButtonID(btnLayerNotice, ID.ID_LAYER_NOTICE);

            OptionsReadButton();

            //int nLayerState = ReadLayerState();
            //InitLayerButtonCheck(nLayerState);

            ReadLeftBarThumbnailOption();

            //////////////////////////////////////////

            Init3DTabButton();
            //InitReportMenuButtons();
            InitAdminRibbonButtons();

            labelSensorMonitor.Text = "수신반 연결상태 알수없음";

            //int nSpace = labelSensorMonitor.Location.X - (btnSensorMonitor.Location.X + btnSensorMonitor.Size.Width);
            //btnSensorMonitor.Location = new Point(checkBoxEquipZoneCCTV.Location.X, btnSensorMonitor.Location.Y);
            //labelSensorMonitor.Location = new Point(btnSensorMonitor.Location.X + btnSensorMonitor.Size.Width + nSpace, labelSensorMonitor.Location.Y); 
        }

        private void OptionsReadButton()
        {
            Point pDefaultButtonLocation = new Point(18, 0);
            int nButtonSpaceH = 18; // 가로 간격
            int nButtonSpaceV = 22; // 세로 간격            
            Size sButtonSize = new Size(138, 50);

            int nIndex = 0;

            // Option Read
            string strSQL = String.Format("SELECT PropertyValue FROM OptionSDMS WHERE PropertyName = 'ToolbarOption' AND SiteID = {0}", UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            List<string> options = new List<string>();
            List<string> deleteOptions = new List<string>(); 
            if (arrResult != null)
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    string strOptionString = WebDBManager.GetStringField(arrResult[i]);

                    foreach (string strOption in strOptionString.Split(','))
                    {
                        string strOptionName = strOption.Trim().Split('(')[0].Trim();
                        bool isVisible = strOption.Trim().Split('(')[1].Trim().Replace(")", "") != "0";

                        // ADD Display Option
                        if (isVisible)
                        {
                            options.Add(strOptionName);
                        }

                        //if (m_dicOptionSDMSNameButtons.ContainsKey(strOptionName))
                        //{
                        //    if (!isVisible)
                        //        m_dicOptionSDMSNameButtons.Remove(strOptionName);
                        //    else
                        //    {
                        //        m_dicOptionSDMSNameButtons[strOptionName].Visible = isVisible;
                        //        ImageButton btn = m_dicOptionSDMSNameButtons[strOptionName];
                        //        btn.Parent = panelLeftItem;
                        //    }
                        //}
                    }
                }

                // DELETE Display Option
                foreach (KeyValuePair<string, ImageButton> item in m_dicOptionSDMSNameButtons)
                {
                    if (!options.Contains(item.Key))
                    {
                        deleteOptions.Add(item.Key);
                    }
                    else
                    {
                        item.Value.Visible = true;
                        item.Value.Parent = panelLeftItem;
                    }
                }

                foreach (string item in deleteOptions)
                {
                    m_dicOptionSDMSNameButtons.Remove(item);
                }
            }  
            if (options.Contains("리모콘"))
            {
                // 0x0 : 조합키 없이 사용, 0x1: ALT, 0x2: Ctrl, 0x3: Shift
                //RegisterHotKey(핸들러함수, 등록키의_ID, 조합키, 등록할_키)
                //F1:매뉴얼, F2:리모콘Visible, F3:리모콘 탭이동                 
                RegisterHotKey((int)this.Handle, 1, 0x0, (int)Keys.F2);
                RegisterHotKey((int)this.Handle, 2, 0x0, (int)Keys.F3);
            }
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
            //InitLayerButtonCheck(nLayerState, LAYER_TYPE.FIRE_DETECT, btnLayerFire);
            //InitLayerButtonCheck(nLayerState, LAYER_TYPE.SPRING_COOLER, btnLayerSpringCooler);
            //InitLayerButtonCheck(nLayerState, LAYER_TYPE.PUMP, btnLayerPump);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.CCTV, btnLayerCCTV);
            //InitLayerButtonCheck(nLayerState, LAYER_TYPE.CCTV_L, btnLayerLowCCTV);
            //InitLayerButtonCheck(nLayerState, LAYER_TYPE.CCTV_DISCONNECTED, btnLayerCCTVDisconnected);
            //InitLayerButtonCheck(nLayerState, LAYER_TYPE.FE, btnLayerFE);
            //InitLayerButtonCheck(nLayerState, LAYER_TYPE.HD, btnLayerHD);
            //InitLayerButtonCheck(nLayerState, LAYER_TYPE.FA, btnLayerFA);
            //InitLayerButtonCheck(nLayerState, LAYER_TYPE.FR, btnLayerFR);
            InitLayerButtonCheck(nLayerState, LAYER_TYPE.BUILDING_TEXT, btnLayerBuildingText);
            //InitLayerButtonCheck(nLayerState, LAYER_TYPE.NOTICE, btnLayerNotice);
        }

        private void InitLayerButtonCheck(int nLayerState, LAYER_TYPE type, ImageButton btn)
        {
            bool checkLayerState = (nLayerState & (int)type) == (int)type;

            CheckButton(btn, checkLayerState);
            m_PageHome.OnChangeLayer(GetButtonID(btn), checkLayerState);
        }

        public int ReadLayerState()
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

        private PopupDialog.Report.FormReportMenu GetReportMenu(PopupDialog.Report.ReportCategory category)
        {
            PopupDialog.Report.FormReportMenu frmMenu;

            if (m_dicReportMenus.TryGetValue(category, out frmMenu))
                return frmMenu;

            return null;
        }

        private PopupDialog.Report.FormReportMenu CreateReportMenu(PopupDialog.Report.ReportCategory category)
        {
            PopupDialog.Report.FormReportMenu frmMenu = new PopupDialog.Report.FormReportMenu(category);
            m_dicReportMenus[category] = frmMenu;
            return frmMenu;
        }

        //화재 탐지 이력에서 GridView DoubleClick 이벤트를 통해 화재대응이력으로 페이지 이동할 경우 화재메뉴 변경
        public void FromDetectPageToActionPage()
        {
            PopupDialog.Report.FormReportMenu fireMenu = GetReportMenu(PopupDialog.Report.ReportCategory.FIRE);

            if (fireMenu != null)
                fireMenu.FromDetectPageToActionPage();
        }
        //누출 탐지 이력에서 GridView DoubleClick 이벤트를 통해 누출대응이력으로 페이지 이동할 경우 화재메뉴 변경
        public void FromDetectPsmPageToActionPage()
        {
            PopupDialog.Report.FormReportMenu psmMenu = GetReportMenu(PopupDialog.Report.ReportCategory.PSM);

            if (psmMenu != null)
                psmMenu.FromDetectPageToActionPage();
        }
        //방범탐지이력에서 GridView DoubleClick 이벤트를 통해 방범대응이력으로 페이지 이동할 경우 방범메뉴 변경
        public void FromDetectIntrusionPageToActionIntrusionPage()
        {
            PopupDialog.Report.FormReportMenu securityMenu = GetReportMenu(PopupDialog.Report.ReportCategory.SECURITY);

            if (securityMenu != null)
                securityMenu.FromDetectPageToActionPage();
        }
        //지진 탐지 이력에서 GridView DoubleClick 이벤트를 통해 지진대응이력으로 페이지 이동할 경우 지진메뉴 변경
        public void FromDetectEarthquakePageToActionEarthquakePage()
        {
            PopupDialog.Report.FormReportMenu earthquakeMenu = GetReportMenu(PopupDialog.Report.ReportCategory.EARTHQUAKE);

            if (earthquakeMenu != null)
                earthquakeMenu.FromDetectPageToActionPage();
        }
        //온도/습도 탐지 이력에서 GridView DoubleClick 이벤트를 통해 대응이력으로 페이지 이동할 경우 메뉴 변경
        public void FromDetectTHPageToActionTHPage()
        {
            PopupDialog.Report.FormReportMenu thMenu = GetReportMenu(PopupDialog.Report.ReportCategory.TemperatureHumidity);

            if (thMenu != null)
                thMenu.FromDetectPageToActionPage();
        }

        private void InitReportMenuButtons()
        {
            // 화재            
            InitReportMenuButtons(SDMS.PopupDialog.Report.ReportCategory.FIRE, ID.ID_BTN_DETECT_ANALYZE, ID.ID_BTN_DETECT, ID.ID_BTN_NOTOPERATION, ID.ID_BTN_ACTION, ID.ID_BTN_SMSREPORT);
            /*m_fireReportButtons.Add(fireMenu.BtnReport);
            InitImageButton(fireMenu.BtnDetectAnalyze, ID.ID_BTN_DETECT_ANALYZE, m_fireReportButtons, fireMenu);
            InitImageButton(fireMenu.BtnDetectHistory, ID.ID_BTN_DETECT, m_fireReportButtons, fireMenu);
            InitImageButton(fireMenu.BtnProcessHistory, ID.ID_BTN_NOTOPERATION, m_fireReportButtons, fireMenu);
            InitImageButton(fireMenu.BtnReactionHistory, ID.ID_BTN_ACTION, m_fireReportButtons, fireMenu);
            InitImageButton(fireMenu.BtnSMSHistory, ID.ID_BTN_SMSREPORT, m_fireReportButtons, fireMenu);
            fireMenu.Visible = false; */
             
            // 누출
            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                InitReportMenuButtons(SDMS.PopupDialog.Report.ReportCategory.PSM, ID.ID_BTN_DETECT_PSM_ANALYZE, ID.ID_BTN_DETECT_PSM, ID.ID_BTN_NOTOPERATION_PSM, ID.ID_BTN_ACTION_PSM, ID.ID_BTN_SMSREPORT_PSM);
                /*psmMenu = new PopupDialog.Report.FormReportMenu(SDMS.PopupDialog.Report.ReportCategory.PSM);
                m_psmReportButtons.Add(psmMenu.BtnReport);
                InitImageButton(psmMenu.BtnDetectAnalyze, ID.ID_BTN_DETECT_PSM_ANALYZE, m_psmReportButtons, psmMenu);
                InitImageButton(psmMenu.BtnDetectHistory, ID.ID_BTN_DETECT_PSM, m_psmReportButtons, psmMenu);
                InitImageButton(psmMenu.BtnProcessHistory, ID.ID_BTN_NOTOPERATION_PSM, m_psmReportButtons, psmMenu);
                InitImageButton(psmMenu.BtnReactionHistory, ID.ID_BTN_ACTION_PSM, m_psmReportButtons, psmMenu);
                InitImageButton(psmMenu.BtnSMSHistory, ID.ID_BTN_SMSREPORT_PSM, m_psmReportButtons, psmMenu);
                psmMenu.Visible = false;*/

            }

            // 방범
            if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                InitReportMenuButtons(SDMS.PopupDialog.Report.ReportCategory.PSM, ID.ID_BTN_DETECT_INTRUSION_ANALYZE, ID.ID_BTN_DETECT_INTRUSION, ID.ID_BTN_NOTOPERATION_INTRUSION, ID.ID_BTN_ACTION_INTRUSION, ID.ID_BTN_SMSREPORT_INTRUSION);
                /*securityMenu = new PopupDialog.Report.FormReportMenu(SDMS.PopupDialog.Report.ReportCategory.SECURITY);
                m_intrusionReportButtons.Add(securityMenu.BtnReport);
                InitImageButton(securityMenu.BtnDetectAnalyze, ID.ID_BTN_DETECT_INTRUSION_ANALYZE, m_intrusionReportButtons, securityMenu);
                InitImageButton(securityMenu.BtnDetectHistory, ID.ID_BTN_DETECT_INTRUSION, m_intrusionReportButtons, securityMenu);
                InitImageButton(securityMenu.BtnProcessHistory, ID.ID_BTN_NOTOPERATION_INTRUSION, m_intrusionReportButtons, securityMenu);
                InitImageButton(securityMenu.BtnReactionHistory, ID.ID_BTN_ACTION_INTRUSION, m_intrusionReportButtons, securityMenu);
                InitImageButton(securityMenu.BtnSMSHistory, ID.ID_BTN_SMSREPORT_INTRUSION, m_intrusionReportButtons, securityMenu);
                securityMenu.Visible = false;*/
            }

            // 지진
            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
            {
                InitReportMenuButtons(SDMS.PopupDialog.Report.ReportCategory.EARTHQUAKE, ID.ID_BTN_DETECT_EARTHQUAKE_ANALYZE, ID.ID_BTN_DETECT_EARTHQUAKE, ID.ID_BTN_NOTOPERATION_EARTHQUAKE, ID.ID_BTN_ACTION_EARTHQUAKE, ID.ID_BTN_SMSREPORT_EARTHQUAKE);
            }

            if (m_useTemperatureHumidity)
            {
                InitReportMenuButtons(SDMS.PopupDialog.Report.ReportCategory.TemperatureHumidity, ID.ID_BTN_DETECT_TH_ANALYZE, ID.ID_BTN_DETECT_TH, -1, ID.ID_BTN_ACTION_TH, -1);
            }
        }

        private void InitReportMenuButtons(SDMS.PopupDialog.Report.ReportCategory category, int nAnalyze, int nDetect, int nProcess, int nReaction, int nSMS)
        {
            PopupDialog.Report.FormReportMenu frmMenu = CreateReportMenu(category);

            if (nAnalyze > 0)
                InitImageButton(frmMenu.BtnDetectAnalyze, nAnalyze, null, frmMenu);

            if (nDetect > 0)
                InitImageButton(frmMenu.BtnDetectHistory, nDetect, null, frmMenu);

            if (nProcess > 0)
                InitImageButton(frmMenu.BtnProcessHistory, nProcess, null, frmMenu);

            if (nReaction > 0)
                InitImageButton(frmMenu.BtnReactionHistory, nReaction, null, frmMenu);

            if (nSMS > 0)
                InitImageButton(frmMenu.BtnSMSHistory, nSMS, null, frmMenu);

            frmMenu.Visible = false;
        }


        private void SetDefaultReportMenuButton()
        {
            //SDMS.Data.ReportMode nPage = m_PageHome.FrmReport.ReportPage;

            //switch (nPage)
            //{
            //    case SDMS.Data.ReportMode.DetectFireAnalyze:
            //        btnDetectAnalyze.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.DetectFire:
            //        btnDetectHistory.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.ProcessFire:
            //        btnProcessHistory.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.ActionFire:
            //        btnReactionHistory.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.SMSFire:
            //        btnSMSHistory.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.DetectPSMAnalyze:
            //        btnDetectPSMAnalyze.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.DetectPSM:
            //        btnDetectPSMHistory.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.ProcessPSM:
            //        btnProcessPSMHistory.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.ActionPSM:
            //        btnReactionPSMHistory.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.SMSPSM:
            //        btnSMSPSMHistory.IsChecked = true;
            //        break;

            //    //방범
            //    case SDMS.Data.ReportMode.DetectIntrusionAnalyze:
            //        btnDetectIntrusionAnalyze.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.DetectIntrusion:
            //        btnDetectIntrusionHistory.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.ProcessIntrusion:
            //        btnProcessIntrusionHistory.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.ActionIntrusion:
            //        btnReactionIntrusionHistory.IsChecked = true;
            //        break;

            //    case SDMS.Data.ReportMode.SMSIntrusion:
            //        btnSMSIntrusionHistory.IsChecked = true;
            //        break;
            //}
        }

        private void Init3DTabButton()
        {
            m_PanelLeft3DTabItemCtrl.Add(btnSDMS);
            m_PanelLeft3DTabItemCtrl.Add(btnSOP);
            m_PanelLeft3DTabItemCtrl.Add(btnBulletin);
            m_PanelLeft3DTabItemCtrl.Add(btnMissionStatus);
            m_PanelLeft3DTabItemCtrl.Add(btnDefaultCCTV);
        }

        private void InitAdminRibbonButtons()
        {
            Image imgMouseOverBkgnd = global::SDMS.Properties.Resources.RibbonMouseOver_bkgnd;
            Image imgCheckedBkgnd = global::SDMS.Properties.Resources.RibbonChecked_bkgnd;
            
            InitImageButton(btnSensorMgr, ID.ID_MANAGE_SENSOR, m_PanelLeftAdminTabItemCtrl);
            InitImageButton(btnShowList, ID.ID_SHOW_LIST_FACILITY, m_PanelLeftAdminTabItemCtrl);
            InitImageButton(btnManageManager, ID.ID_MANAGE_MANAGER, m_PanelLeftAdminTabItemCtrl);
            InitImageButton(btnManageSMS, ID.ID_MANAGE_MESSAGE, m_PanelLeftAdminTabItemCtrl);
            InitImageButton(btnManageBroadcast, ID.ID_MANAGE_BROADCAST, m_PanelLeftAdminTabItemCtrl);
            //InitRibbonButton(btnManagePrint, ID.ID_MANAGE_PRINT, "도면관리", global::SDMS.Properties.Resources.Manage_Print_Normal, global::SDMS.Properties.Resources.Manage_Print_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            //InitRibbonButton(btnManageFacility, ID.ID_MANAGE_FACILITY, "장비현황", global::SDMS.Properties.Resources.Manage_Facility_Normal, global::SDMS.Properties.Resources.Manage_Facility_Checked, imgMouseOverBkgnd, imgCheckedBkgnd);
            InitImageButton(btnManageDetect, ID.ID_MANAGE_DETECT, m_PanelLeftAdminTabItemCtrl);
            //InitRibbonButton(btnBackupDB, ID.ID_MANAGE_BACKUPDB, "백업/복원", global::SDMS.Properties.Resources.Backup_Restore, global::SDMS.Properties.Resources.Backup_restore_checked, imgMouseOverBkgnd, imgCheckedBkgnd);

            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
                InitImageButton(btnEarthquake, ID.ID_MANAGE_EARTHQUAKE, m_PanelLeftAdminTabItemCtrl);

            InitImageButton(btnSave, ID.ID_SAVE_DATA, m_PanelLeftAdminTabItemCtrl);
             
            foreach (Control ctrl in m_PanelLeftAdminTabItemCtrl)
            {
                ctrl.Parent = panelLeftAdminTabItemCtrl;
                ctrl.Visible = true;
            }

            LeftAdminPanelPopupToggleContinueBtns.Add(btnSensorMgr);
            LeftAdminPanelPopupToggleContinueBtns.Add(btnShowList);
            LeftAdminPanelPopupToggleContinueBtns.Add(btnManageManager);
            LeftAdminPanelPopupToggleContinueBtns.Add(btnManageSMS);
            LeftAdminPanelPopupToggleContinueBtns.Add(btnManageBroadcast);
            LeftAdminPanelPopupToggleContinueBtns.Add(btnManageDetect);
            LeftAdminPanelPopupToggleContinueBtns.Add(btnEarthquake);
            
            btnSave.Enabled = false;
        } 
         
        private void InitImageButton(ImageButton btn, int nID, List<Control> buttons = null, IImageButtonOwner owner = null)
        {
            //btn.ImageNormal = imgNormal;
            //btn.ImageClicked = imgChecked;
            //btn.MouseOverBkgndImage = imgMouseOverBkgnd;
            //btn.CheckedBkgndImage = imgCheckedBkgnd;
            //btn.Title = strTitle; 
            if (owner == null)
                btn.Owner = this;
            else
                btn.Owner = owner;
            btn.Tag = nID;
            SetButtonID(btn, nID);

            if (buttons != null)
                buttons.Add(btn);
        }        

        private void SetButtonID(ImageButton btn, int nID, string strTooltipText = "")
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

        public ImageButton GetButton(int nID)
        {
            if (m_dicIDButtons.ContainsKey(nID))
                return m_dicIDButtons[nID];

            return null;
        }

        public int GetButtonID(ImageButton btn)
        {
            if (m_dicButtonIDs.ContainsKey(btn))
                return m_dicButtonIDs[btn];

            return -1;
        }

        public bool IsChecked(int nButtonID)
        {
            if (!m_dicIDButtons.ContainsKey(nButtonID))
                return false;

            ImageButton btn = m_dicIDButtons[nButtonID];
            return IsChecked(btn);
        }

        public bool IsChecked(ImageButton btn)
        {
            if (m_dicButtonChecked.ContainsKey(btn))
                return m_dicButtonChecked[btn];

            return false;
        }

        public void CheckButton(int nButtonID, bool isChecked)
        {
            if (!m_dicIDButtons.ContainsKey(nButtonID))
                return;

            ImageButton btn = m_dicIDButtons[nButtonID];
            CheckButton(btn, isChecked);
            //m_toolbar.CheckButton(nButtonID, isChecked);
        }

        public void CheckButton(UnE.GUI.ImageButton btn, bool isChecked)
        {
            if (btn == null)
                return;

            if (!m_dicButtonChecked.ContainsKey(btn))
                return;

            bool checkedOld = m_dicButtonChecked[btn];
            m_dicButtonChecked[btn] = isChecked;

            SetButtonBackColor(btn);

            if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_HOME)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.Home_Checked;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.Home_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_FULLSCREEN)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.Home_Click;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.Home_Defulat;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_PICK)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.Pick_Click;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.Pick_Default;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_PAN)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.Panning_Click;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.Panning_Default;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_ORBIT)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.Orbit_Click;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.Orbit_Default;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_ZOOMIN)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.ZoomIn_Click;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.ZoomIn_Default;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_ZOOMOUT)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.ZoomOut_Click;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.ZoomOut_Default;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_OUTSIDE)
            {
                if (isChecked) 
                    btn.ImageNormal = global::SDMS.Properties.Resources.LeftBar_3D_Click; 
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.LeftBar_3D_Default; 
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_BOTHSIDE)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.LeftBar_2D3D_Click;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.LeftBar_2D3D_Default;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_INSIDE)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.LeftBar_2D_Click; 
                else 
                    btn.ImageNormal = global::SDMS.Properties.Resources.LeftBar_2D_Default; 
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_CCTV)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.CCTV_Checked;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.CCTV_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_SCREENSHOT)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.ScreenShot_Click;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.ScreenShot_Default;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_VIEW_WEATHER_INFO)
            {
                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.Weather_checked;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.Weather_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_DETECTOR)
            {
                WriteLayerState(LAYER_TYPE.FIRE_DETECT, isChecked);

                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.Layer_Fire_Checked;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.Layer_Fire_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_COOLER)
            {
                WriteLayerState(LAYER_TYPE.SPRING_COOLER, isChecked);

                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.Layer_SpringCooler_Checked;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.Layer_SpringCooler_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_PERSURE)
            {
                WriteLayerState(LAYER_TYPE.PUMP, isChecked);

                if (isChecked)
                    btn.ImageNormal = global::SDMS.Properties.Resources.Layer_Pump_Checked;
                else
                    btn.ImageNormal = global::SDMS.Properties.Resources.Layer_Pump_Normal;
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_LAYER_CCTV)
            {
                WriteLayerState(LAYER_TYPE.CCTV, isChecked);

                if (isChecked)
                {
                    btn.ImageNormal = global::SDMS.Properties.Resources.CCTVInfo_Click;
                    btn.ImageMouseOver = global::SDMS.Properties.Resources.CCTVInfo_Click;
                }
                else
                {
                    btn.ImageNormal = global::SDMS.Properties.Resources.CCTVInfo_Default;
                    btn.ImageMouseOver = global::SDMS.Properties.Resources.CCTVInfo_Default;
                }
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
                {
                    btn.ImageNormal = global::SDMS.Properties.Resources.DetailInfo_Click;
                    btn.ImageMouseOver = global::SDMS.Properties.Resources.DetailInfo_Click;
                }
                else
                {
                    btn.ImageNormal = global::SDMS.Properties.Resources.DetailInfo_Default;
                    btn.ImageMouseOver = global::SDMS.Properties.Resources.DetailInfo_Default;
                }
            }
            else if (btn.Tag != null && (int)btn.Tag == ID.ID_SAVE_DATA)
            {
                if (isChecked)
                {
                    btn.ImageNormal = global::SDMS.Properties.Resources.Save_Click;
                    btn.ImageMouseOver = global::SDMS.Properties.Resources.Save_Click;
                }
                else
                {
                    btn.ImageNormal = global::SDMS.Properties.Resources.Save_Default;
                    btn.ImageMouseOver = global::SDMS.Properties.Resources.Save_Default;
                }
            }
            else if (btn.GetType() == typeof(RibbonButton))
            {
                //RibbonButton ribbonButton = (RibbonButton)btn;
                //ribbonButton.IsChecked = isChecked;
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
            //int nClockRight = panelClock.Location.X + panelClock.Size.Width;
            //int nPanelSpace = panelStatus.Location.X - nClockRight - btnDefaultCCTV.Width;

            //int nStatusRight = panelStatus.Location.X + panelStatus.Size.Width;
            //panelLog.Location = new Point(nStatusRight + nPanelSpace, panelLog.Location.Y);

            //btnFire.Location = new Point(panelTop.Size.Width - btnFire.Size.Width, btnFire.Location.Y);
            //panelLog.Size = new Size(btnFire.Location.X - nPanelSpace - panelLog.Location.X, panelLog.Size.Height);
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
            int nSpace = 9;
            //btnMin.Location = new Point(btnMax.Location.X - m_nSystemButtonSpace - nImageWidth, btnMin.Location.Y);
            //btnMax.Location = new Point(btnClose.Location.X - m_nSystemButtonSpace - nImageWidth, btnMax.Location.Y);
            //btnClose.Location = new Point(nWidth - nImageWidth, btnClose.Location.Y);            
            btnTargetHelp.Location = new Point(this.Size.Width - (nImageWidth * 4) - (nSpace * 4) - 10, (int)(panelTop.Height * 0.5 - btnTargetHelp.Height * 0.5));
            btnMin.Location = new Point(this.Size.Width - (nImageWidth * 3) - (nSpace * 3) - 10, (int)(panelTop.Height * 0.5 - btnMin.Height * 0.5));
            btnMax.Location = new Point(this.Size.Width - (nImageWidth * 2) - (nSpace * 2) - 10, (int)(panelTop.Height * 0.5 - btnMax.Height * 0.5));
            btnClose.Location = new Point(this.Size.Width - (nImageWidth * 1) - (nSpace * 1) - 10, (int)(panelTop.Height * 0.5 - btnClose.Height * 0.5)); 
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            MainFrame.WindowState = FormWindowState.Minimized;
        }

        private Image imgWindowNormal = global::SDMS.Properties.Resources.WindowNormal;
        private Image imgWindowNormalClick = global::SDMS.Properties.Resources.WindowNormal_Click;
        private Image imgWindowMax = global::SDMS.Properties.Resources.WindowMax;
        private Image imgWindowMaxClick = global::SDMS.Properties.Resources.WindowMax_Click;

        private void btnMax_Click(object sender, EventArgs e)
        { 
            if (MainFrame.WindowState == FormWindowState.Normal)
            { 
                MainFrame.WindowState = FormWindowState.Maximized;
                btnMax.ImageNormal = imgWindowNormal;
                btnMax.ImageClicked = imgWindowNormalClick;                
                btnMax.ImageMouseOver = imgWindowNormalClick;

                GetResolution();
            }
            else if (MainFrame.WindowState == FormWindowState.Maximized)
            {
                MainFrame.WindowState = FormWindowState.Normal;
                btnMax.ImageNormal = imgWindowMax;
                btnMax.ImageClicked = imgWindowMaxClick;                
                btnMax.ImageMouseOver = imgWindowMaxClick;
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

                    if (MainFrame.WindowState == FormWindowState.Normal && btnMax.ImageNormal != imgWindowMax)
                    {
                        btnMax.ImageNormal = imgWindowMax;
                        btnMax.ImageClicked = imgWindowMaxClick; 
                        btnMax.ImageMouseOver = imgWindowMaxClick;
                    }

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
                btnMax.ImageNormal = imgWindowNormal;
                btnMax.ImageClicked = imgWindowNormalClick;                
                btnMax.ImageMouseOver = imgWindowNormalClick;

                GetResolution();
            }
            else if (MainFrame.WindowState == FormWindowState.Maximized)
            {
                Size sizeCur = MainFrame.Size;
                MainFrame.WindowState = FormWindowState.Normal;
                btnMax.ImageNormal = imgWindowMax;
                btnMax.ImageClicked = imgWindowMaxClick;                
                btnMax.ImageMouseOver = imgWindowMaxClick;
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

        private Image m_Img3dTabDefault = global::SDMS.Properties.Resources._3DTab_Default;
        private Image m_Img3dTabClick = global::SDMS.Properties.Resources._3DTab_Click;
        private Image m_ImgAdminTabDefault = global::SDMS.Properties.Resources.AdminTab_Default;
        private Image m_ImgAdminTabClick = global::SDMS.Properties.Resources.AdminTab_Click;
        private Image m_ImgReportdTabDefault = global::SDMS.Properties.Resources.ReportTab_Default;
        private Image m_ImgReportTabClick = global::SDMS.Properties.Resources.ReportTab_Click;

        private void btnTopTab_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ImageButton btn = sender as ImageButton;
            if (btn == null)
                return;

            if (btn == btn3DTab)
            {
                if (mCurrentTab != UnE.View.Content.ContentOwnerTab.M3D_TAB)
                { 
                    m_PageHome.ContentForm.ClearTabState();
                    SelectMonitoringTab();
                }
            }
            else if (btn == btnAdminTab)
            {
                if (mCurrentTab != UnE.View.Content.ContentOwnerTab.ADMIN_TAB)
                { 
                    m_PageHome.ContentForm.ClearTabState();
                    SelectAdminTab();
                    mCurrentTab = UnE.View.Content.ContentOwnerTab.ADMIN_TAB; 
                }
            }
            else if (btn == btnReportTab)
            {
                if (mCurrentTab != UnE.View.Content.ContentOwnerTab.REPORT_TAB)
                { 
                    m_PageHome.ContentForm.ClearTabState();
                    SelectReportTab();
                    mCurrentTab = UnE.View.Content.ContentOwnerTab.REPORT_TAB;
                }
            }  
             
            if (m_IsDisaster && mCurrentTab == UnE.View.Content.ContentOwnerTab.M3D_TAB)
            {
                panelBottom.Location = new Point(panelStatus.Location.X, panelStatus.Location.Y + panelStatus.Height);
            }
            else
                panelBottom.Location = new Point(panelLeft2.Location.X + panelLeft2.Size.Width, panelLeft2.Location.Y);

            // 3D탭, 관리탭 별로 오른쪽 Docking form이 다르다.
            // Docking 된 form에 따라서 왼쪽 메뉴 버튼을 활성화 시켜야 한다.
            if (PageHome.DicTabDockingMode[mCurrentTab] == ContentOwnerTabRightDockingMode.SHOW_PSM)
            {
                if (btnPSMStatus.BackColor == Color.Transparent)
                    SetButtonBackColor(btnPSMStatus);
            }
            else if (PageHome.DicTabDockingMode[mCurrentTab] == ContentOwnerTabRightDockingMode.SHOW_CCTV)
            {
                if (btnMultiCCTV.BackColor == Color.Transparent)
                    SetButtonBackColor(btnMultiCCTV);
            }
            else if (PageHome.DicTabDockingMode[mCurrentTab] == ContentOwnerTabRightDockingMode.SHOW_DISASTER)
            {
                if (btnDisasterPrevention.BackColor == Color.Transparent)
                    SetButtonBackColor(btnDisasterPrevention);
            }
            else
            {
                btnPSMStatus.BackColor = btnMultiCCTV.BackColor = btnDisasterPrevention.BackColor = Color.Transparent;
            }

            // 리모콘 활성화중일때 동기화
            if (m_frmRemoteCtrl != null && m_frmRemoteCtrl.Visible)
            {
                m_frmRemoteCtrl.ChangeTab(mCurrentTab);
            }
        } 

        public void TextPictureBox_MouseDown(TextPictureBox pictureBox, MouseEventArgs e)
        {
            if (e != null)
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left)
                    return;
            }
             
            if (pictureBox == pictureBox2D)
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
            //if (panelReportRibbonBarLeft.Visible)
            //{
            //    m_dtLastReport = DateTime.Now;
            //    if (SensorHistoryManager.Instance.LastSensorZoneHistoryID != -1)
            //        m_nReadHistoryID = SensorHistoryManager.Instance.LastSensorZoneHistoryID;
            //}

            if (this.HiddenClock)
            {
                btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = btnDefaultCCTV.Visible = true;
            }
            else
                panelClock.Visible = true;
             
            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;
              
            panelLeftAdminTabItemCtrl.Visible = false; 

            labelFireDetect.Visible = true;
            cmbFireDetect.Visible = true;

            m_PageHome.CloseExternal();
            //m_toolbar.Mode = 1;
            ShowToolbar();

            pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Pressed;
            pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal; 

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
            //if (panelReportRibbonBarLeft.Visible)
            //{
            //    m_dtLastReport = DateTime.Now;
            //    if (SensorHistoryManager.Instance.LastSensorZoneHistoryID != -1)
            //        m_nReadHistoryID = SensorHistoryManager.Instance.LastSensorZoneHistoryID;
            //}

            if (this.HiddenClock)
            {
                btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = btnDefaultCCTV.Visible = true;
            }
            else
                panelClock.Visible = true;
             
            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;
              
            panelLeftAdminTabItemCtrl.Visible = false; 

            labelFireDetect.Visible = true;
            cmbFireDetect.Visible = true;

            //if (!ShowEquipZoneCCTV)
            //    ShowToolbar();
            //m_toolbar.Mode = 1;
            HideToolbar();

            pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Pressed; 

            OnSelectCCTVTab();

            if (bLoadCCTV == true)
                m_PageHome.OnClickBigCCTV();

            mCurrentTab = UnE.View.Content.ContentOwnerTab.CCTV_TAB;

            return true;
        }
         
        public bool SelectMonitoringTab()
        {
            btn3DTab.Image = m_Img3dTabClick;
            btn3DTab.ImageNormal = m_Img3dTabClick;
            btn3DTab.ImageMouseOver = m_Img3dTabClick;

            btnAdminTab.Image = m_ImgAdminTabDefault;
            btnAdminTab.ImageNormal = m_ImgAdminTabDefault;
            btnAdminTab.ImageMouseOver = m_ImgAdminTabDefault;

            btnReportTab.Image = m_ImgReportdTabDefault;
            btnReportTab.ImageNormal = m_ImgReportdTabDefault;
            btnReportTab.ImageMouseOver = m_ImgReportdTabDefault;
             
            m_PageHome.ContentForm.Visible = true;

            FormMain.Instance.PageHome.ContentForm.SaveCurrentTabLayout();

            // 직전 모드가 Report 탭이었다면...
            //if (panelReportRibbonBarLeft.Visible)
            //{
            //    m_dtLastReport = DateTime.Now;
            //    if (SensorHistoryManager.Instance.LastSensorZoneHistoryID != -1)
            //        m_nReadHistoryID = SensorHistoryManager.Instance.LastSensorZoneHistoryID;
            //}

            if (this.HiddenClock)
            {
                btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = btnDefaultCCTV.Visible = true;
            }
            else
                panelClock.Visible = true;
             
            panelTop3DTabItemCtrl.Visible = true; 
            panelLeftItem.Visible = true;
            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;

            panelLeft3DTabItemCtrl.Visible = true; 
            panelLeftAdminTabItemCtrl.Visible = false;  

            m_PageHome.CloseExternal();

            //m_toolbar.Mode = 1;
            ShowToolbar();
             
            pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            OnSelectMonitoringTab();

            //m_PageHome.OnClick3D();

            mCurrentTab = UnE.View.Content.ContentOwnerTab.M3D_TAB;
            m_PageHome.ContentForm.LoadTabLayout((int)mCurrentTab);

            SetVisibleDisasterPanel(m_IsDisaster);

            CloseOtherReportMenu(PopupDialog.Report.ReportCategory.NONE);

            foreach (KeyValuePair<PopupDialog.Report.ReportCategory, PopupDialog.Report.FormReportMenu> pair in m_dicReportMenus)
            {
                pair.Value.Visible = false;
            }
            /*if (fireMenu != null)
                fireMenu.Visible = false;

            if (psmMenu != null)
                psmMenu.Visible = false;

            if (securityMenu != null)
                securityMenu.Visible = false;*/

            if (m_uiManager != null)
                m_uiManager.ShowControl("Monitoring");

            return true;
        }

        public bool SelectAdminTab()
        {
            btn3DTab.Image = m_Img3dTabDefault;
            btn3DTab.ImageNormal = m_Img3dTabDefault;
            btn3DTab.ImageMouseOver = m_Img3dTabDefault;

            btnAdminTab.Image = m_ImgAdminTabClick;
            btnAdminTab.ImageNormal = m_ImgAdminTabClick;
            btnAdminTab.ImageMouseOver = m_ImgAdminTabClick;

            btnReportTab.Image = m_ImgReportdTabDefault;
            btnReportTab.ImageNormal = m_ImgReportdTabDefault;
            btnReportTab.ImageMouseOver = m_ImgReportdTabDefault;

            m_PageHome.ContentForm.Visible = true;
             
            FormMain.Instance.PageHome.ContentForm.SaveCurrentTabLayout();
            // 직전 모드가 Report 탭이었다면...
            //if (panelReportRibbonBarLeft.Visible)
            //{
            //    m_dtLastReport = DateTime.Now;
            //    if (SensorHistoryManager.Instance.LastSensorZoneHistoryID != -1)
            //        m_nReadHistoryID = SensorHistoryManager.Instance.LastSensorZoneHistoryID;
            //}
             
            btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = btnDefaultCCTV.Visible = false;
            //btnSensorMonitor.Visible = labelSensorMonitor.Visible = btnSendMessage.Visible = false;
            panelClock.Visible = false; 
            panelTop3DTabItemCtrl.Visible = true;
            //알람 창
            m_PanelBtnNotice.Visible = false;
            //btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Normal;

            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;

            panelLeft3DTabItemCtrl.Visible = false; 
            panelLeftAdminTabItemCtrl.Visible = true;
            panelLeftItem.Visible = true;   

            m_PageHome.CloseExternal();

            //m_toolbar.Mode = 2;
            ShowToolbar();
             
            pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            
            OnSelectAdminTab();

            mCurrentTab = UnE.View.Content.ContentOwnerTab.ADMIN_TAB;
            m_PageHome.ContentForm.LoadTabLayout((int)mCurrentTab);

            SetVisibleDisasterPanel(m_IsDisaster);

            CloseOtherReportMenu(PopupDialog.Report.ReportCategory.NONE);

            foreach (KeyValuePair<PopupDialog.Report.ReportCategory, PopupDialog.Report.FormReportMenu> pair in m_dicReportMenus)
            {
                pair.Value.Visible = false;
            }
            /*if (fireMenu != null)
                fireMenu.Visible = false;

            if (psmMenu != null)
                psmMenu.Visible = false;

            if (securityMenu != null)
                securityMenu.Visible = false;*/

            if (m_uiManager != null)
                m_uiManager.ShowControl("Admin");

            return true;
        }

        public bool SelectReportTab()
        {
            btn3DTab.Image = m_Img3dTabDefault;
            btn3DTab.ImageNormal = m_Img3dTabDefault;
            btn3DTab.ImageMouseOver = m_Img3dTabDefault;

            btnAdminTab.Image = m_ImgAdminTabDefault;
            btnAdminTab.ImageNormal = m_ImgAdminTabDefault;
            btnAdminTab.ImageMouseOver = m_ImgAdminTabDefault;

            btnReportTab.Image = m_ImgReportTabClick;
            btnReportTab.ImageNormal = m_ImgReportTabClick;
            btnReportTab.ImageMouseOver = m_ImgReportTabClick; 

            //m_dtLastReport = DateTime.Now;
            if (SensorHistoryManager.Instance.LastSensorZoneHistoryID != -1)
                m_nReadHistoryID = SensorHistoryManager.Instance.LastSensorZoneHistoryID;
             
            btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = btnDefaultCCTV.Visible = false;
            //btnSensorMonitor.Visible = labelSensorMonitor.Visible = false;
            //panelMiddle.Refresh();

            panelTop3DTabItemCtrl.Visible = false;

            panelClock.Visible = false;
             
            //알람창
            m_PanelBtnNotice.Visible = false;
            //btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Normal;

            checkedReportRibbonbar();
            panelLeftItem.Visible = false;   

            panelLeft3DTabItemCtrl.Visible = false; 
            panelLeftAdminTabItemCtrl.Visible = false;

            /*
            if (UnE.SOP.ProxySOP.Instance.SiteID == 3)
            {
                if (bCheckedbtnEarthquakeDetection)
                    btnEarthquakeDetection_Click(null, null);
                if (bCheckedbtnWorkStatus)
                    btnWorkStatus_Click(null, null);
                if (bCheckedbtnAirQuality)
                    btnAirQuality_Click(null, null);
            }
            */


            m_PageHome.CloseExternal();
             
            pictureBox2D.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;
            pictureBoxCCTV.BackgroundImage = global::SDMS.Properties.Resources.Tab_Normal;


            //m_toolbar.Mode = 1;
            HideToolbar();

            OnSelectReportTab();
            mCurrentTab = UnE.View.Content.ContentOwnerTab.REPORT_TAB;
            //m_PageHome.ContentForm.LoadTabLayout((int)mCurrentTab);

            // Unity 화면의 간섭을 없애기 위하여 안보이도록 한다.
            m_PageHome.ContentForm.Visible = false;

            ResizeReportMenu();

            // 화면설정 팝업
            if (m_PaneBtnSaveHome.Visible)
                btnSaveHome_Click(btnSaveHome, null);

            // 알림공지 팝업
            if (m_frmMessageSender != null && m_frmMessageSender.Visible)
                ShowSendMessage();

            if (m_frmMessageReceiver != null && m_frmMessageReceiver.Visible)
                m_frmMessageReceiver.Visible = false;

            SetVisibleDisasterPanel(m_IsDisaster);

            foreach (KeyValuePair<PopupDialog.Report.ReportCategory, PopupDialog.Report.FormReportMenu> pair in m_dicReportMenus)
            {
                pair.Value.Visible = true;
            }
            /*if (fireMenu != null)
                fireMenu.Visible = true;

            if (psmMenu != null) 
                psmMenu.Visible = true;

            if (securityMenu != null)
                securityMenu.Visible = true;*/

            if (m_uiManager != null)
                m_uiManager.ShowControl("Report");

            return true;
        }

        private void ResizeReportMenu()
        {
            int frmWidth = 371;
            int frmHeight = 66;

            int horizontal = 5;
            int vertical = 5;
            int padding = 10;
            int y = panelTop2.Location.Y + panelTop2.Height;

            double sizePer = 1;
            if (FormMain.Instance.Resolution == Resolution.FourK) 
                sizePer = 2; 
            else if (FormMain.Instance.Resolution == SDMS.Resolution.Other) 
                sizePer = 0.75; 

            horizontal = (int)(horizontal * sizePer);
            vertical = (int)(vertical * sizePer);
            padding = (int)(padding * sizePer);

            y += padding;

            SetReportMenuButton(PopupDialog.Report.ReportCategory.FIRE, frmWidth, frmHeight, ref y, padding, sizePer, horizontal);
            
            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                SetReportMenuButton(PopupDialog.Report.ReportCategory.PSM, frmWidth, frmHeight, ref y, padding, sizePer, horizontal);
            }

            if (UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                SetReportMenuButton(PopupDialog.Report.ReportCategory.SECURITY, frmWidth, frmHeight, ref y, padding, sizePer, horizontal);
            }

            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
            {
                SetReportMenuButton(PopupDialog.Report.ReportCategory.EARTHQUAKE, frmWidth, frmHeight, ref y, padding, sizePer, horizontal);
            }

            if (m_useTemperatureHumidity)
            {
                SetReportMenuButton(PopupDialog.Report.ReportCategory.TemperatureHumidity, frmWidth, frmHeight, ref y, padding, sizePer, horizontal);
            }
        }

        private void SetReportMenuButton(PopupDialog.Report.ReportCategory category, int frmWidth, int frmHeight, ref int y, int padding, double sizePer, int horizontal)
        {
            PopupDialog.Report.FormReportMenu frmMenu = GetReportMenu(category);

            if (frmMenu == null)
                frmMenu = CreateReportMenu(category);
            else
                frmMenu.SetChildCtrlResize(frmMenu, frmWidth, frmHeight);
            frmMenu.SetLocation();

            int x = (int)(panelLeft2.Width * 0.5 - frmMenu.btnReport.Width * 0.5) - horizontal;

            Point pt2 = new Point(x, y);
            Point ptScr2 = this.PointToScreen(pt2);

            if (mCurrentTab == UnE.View.Content.ContentOwnerTab.REPORT_TAB)
            {
                if (frmMenu.Visible)
                    frmMenu.Focus();
                else
                    frmMenu.Show(this);
            }

            frmMenu.Location = ptScr2;
            if (frmMenu.SizeFull)
                frmMenu.Size = new Size((int)(frmWidth * sizePer), (int)(frmHeight * sizePer));
            else
                frmMenu.Size = new Size(frmMenu.btnReport.Width + (horizontal * 2), frmMenu.Height);

            y += frmMenu.Height + padding;
        }

        public void ResetOtherReportMenu(SDMS.PopupDialog.Report.ReportCategory category)
        {
            foreach (KeyValuePair<PopupDialog.Report.ReportCategory, PopupDialog.Report.FormReportMenu> pair in m_dicReportMenus)
            {
                if (category == pair.Key)
                    continue;

                pair.Value.ResetImageButton();
            }
            /*if (category == PopupDialog.Report.ReportCategory.FIRE)
            {
                if (psmMenu != null)
                    psmMenu.ResetImageButton();
                if (securityMenu != null)
                    securityMenu.ResetImageButton();
            }
            else if (category == PopupDialog.Report.ReportCategory.PSM)
            {
                if (fireMenu != null)
                    fireMenu.ResetImageButton();
                if (securityMenu != null)
                    securityMenu.ResetImageButton();
            }
            else if (category == PopupDialog.Report.ReportCategory.SECURITY)
            {
                if (fireMenu != null)
                    fireMenu.ResetImageButton();
                if (psmMenu != null)
                    psmMenu.ResetImageButton(); 
            }*/
        }
        public void CloseOtherReportMenu(SDMS.PopupDialog.Report.ReportCategory category = PopupDialog.Report.ReportCategory.NONE)
        {
            foreach (KeyValuePair<PopupDialog.Report.ReportCategory, PopupDialog.Report.FormReportMenu> pair in m_dicReportMenus)
            {
                if (category == pair.Key)
                    continue;

                if (pair.Value.SizeFull)
                    pair.Value.btnReport_Click(null, null);
            }
            /*if (category == PopupDialog.Report.ReportCategory.NONE)
            {
                if (fireMenu != null && fireMenu.SizeFull)
                    fireMenu.btnReport_Click(null, null);
                if (psmMenu != null && psmMenu.SizeFull)
                    psmMenu.btnReport_Click(null, null);
                if (securityMenu != null && securityMenu.SizeFull)
                    securityMenu.btnReport_Click(null, null);
            }
            else
            {
                if (category == PopupDialog.Report.ReportCategory.FIRE)
                {
                    if (psmMenu != null && psmMenu.SizeFull)
                        psmMenu.btnReport_Click(null, null);
                    if (securityMenu != null && securityMenu.SizeFull)
                        securityMenu.btnReport_Click(null, null);
                }
                else if (category == PopupDialog.Report.ReportCategory.PSM)
                {
                    if (fireMenu != null && fireMenu.SizeFull)
                        fireMenu.btnReport_Click(null, null);
                    if (securityMenu != null && securityMenu.SizeFull)
                        securityMenu.btnReport_Click(null, null);
                }
                else if (category == PopupDialog.Report.ReportCategory.SECURITY)
                {
                    if (fireMenu != null && fireMenu.SizeFull)
                        fireMenu.btnReport_Click(null, null);
                    if (psmMenu != null && psmMenu.SizeFull)
                        psmMenu.btnReport_Click(null, null);
                }
            }*/
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

                case Data.ReportMode.DetectEarthquake:
                    panelProcessHistory.Visible = true;
                    panelReactionHistory.Visible = false;
                    pnDetectPSM.Visible = false;
                    pnNotOperationPSM.Visible = false;
                    pnActionPSM.Visible = false;
                    pnSMSPSM.Visible = false;
                    
                    btnDateFormat.Visible =
                    lblSplitUnit.Visible =
                    lblSplitUnitDetail.Visible =
                    lblViewCount.Visible =
                    proc_cboSplitUnit.Visible =
                    nudSplitUnitDetail.Visible =
                    proc_cboViewCount.Visible = true;
                    break;
                case Data.ReportMode.ActionEarthquake:
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

                ImageButton activeButton = GetActiveReportButton();

                if (activeButton != null)
                    OnImageButtonMouseUp(activeButton, null);
            }

            FormMain.Instance.EnableFireReportBtn(false);
            PageHome.ChangeTab(UnE.View.Content.ContentOwnerTab.REPORT_TAB);

            int nSpace3 = 26;
            if (m_Resolution == Resolution.FullHD)
            {
                nSpace3 = (int)(nSpace3 * 0.5);
            }
            ReportCtrlWidthLineUp(nSpace3, true);
        }

        // 현재 활성화 상태인 Report Button을 얻어온다.
        private ImageButton GetActiveReportButton()
        { 
            //if (btnDetectHistory.IsChecked)
            //    return btnDetectHistory;

            //if (btnProcessHistory.IsChecked)
            //    return btnProcessHistory;

            //if (btnReactionHistory.IsChecked)
            //    return btnReactionHistory;

            //if (btnSMSHistory.IsChecked)
            //    return btnSMSHistory;

            //if (btnDetectPSMHistory.IsChecked)
            //    return btnDetectPSMHistory;

            //if (btnProcessPSMHistory.IsChecked)
            //    return btnProcessPSMHistory;

            //if (btnReactionPSMHistory.IsChecked)
            //    return btnReactionPSMHistory;

            //if (btnSMSPSMHistory.IsChecked)
            //    return btnSMSPSMHistory;

            //if (btnDetectIntrusionHistory.IsChecked)
            //    return btnDetectIntrusionHistory;

            //if (btnProcessIntrusionHistory.IsChecked)
            //    return btnProcessIntrusionHistory;

            //if (btnReactionIntrusionHistory.IsChecked)
            //    return btnReactionIntrusionHistory;

            //if (btnSMSIntrusionHistory.IsChecked)
            //    return btnSMSIntrusionHistory;

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

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            int nLastReactionHistoryID = -1;

            if (arrResult != null && arrResult.Count > 0)
            {
                nLastReactionHistoryID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

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
            if (m_uiManager != null)
                m_uiManager.HideControl("AppClose");

            HideToolbar();

            if (cmbSensorDetectTooltip.IsBalloon)
                cmbSensorDetectTooltip.Hide(cmbFireDetect);
            cmbSensorDetectTooltip.Dispose();
            cmbSensorDetectTooltip = null;

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
                FormMain.Instance.PageHome.CloseExternal();

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

            //UnE.View.Content.FormContentUnity.KillProcess("UnitySam");
            //UnE.View.Content.FormContentUnity.KillProcess("UnitySamInside");
            UnE.View.Content.FormContentUnity.KillProcess("CCTVViewer");
            UnE.View.Content.FormContentUnity.KillProcess("CCTVViewer2");
            UnE.View.Content.FormContentUnity.KillProcess("libCCTV");
            UnE.View.Content.FormContentUnity.KillProcess("libCCTV2");
            /*UnE.View.Content.FormContentUnity.KillProcess("EnergyOutside");
            UnE.View.Content.FormContentUnity.KillProcess("SeoulUnv");
            UnE.View.Content.FormContentUnity.KillProcess("BusanUnv");*/

            CloseUnityProcess();
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

        public void RemoteOperation(object sender, EventArgs e)
        {            
            ImageButton btn = sender as ImageButton;
            if (btn == null)
                return;

            if (btn.Tag == null)
                return;

            int nBtnID = (int)btn.Tag;

            ImageButton btnReal = GetButton(nBtnID);

            if (nBtnID == ID.ID_TAB_3D || nBtnID == ID.ID_TAB_MANAGE || nBtnID == ID.ID_TAB_REPORT) // 탭
                btnTopTab_Click(btnReal, e);
            else if (nBtnID >= ID.ID_BTN_DETECT_ANALYZE && nBtnID <= ID.ID_BTN_SMSREPORT_PSM) // 리포트
            {
                if (mCurrentTab != ContentOwnerTab.REPORT_TAB)
                {
                    btnTopTab_Click(btnReportTab, e);
                }

                if (nBtnID >= ID.ID_BTN_DETECT_ANALYZE && nBtnID <= ID.ID_BTN_SMSREPORT) // 화재
                {
                    PopupDialog.Report.FormReportMenu fireMenu = GetReportMenu(PopupDialog.Report.ReportCategory.FIRE);

                    if (fireMenu != null)
                    {
                        fireMenu.SizeFull = true;
                        fireMenu.OnImageButtonMouseUp(btnReal, null);
                        fireMenu.SizeFull = false;
                    }
                }
                else if (nBtnID >= ID.ID_BTN_DETECT_PSM_ANALYZE && nBtnID <= ID.ID_BTN_SMSREPORT_PSM) // 누출
                {
                    PopupDialog.Report.FormReportMenu psmMenu = GetReportMenu(PopupDialog.Report.ReportCategory.PSM);

                    if (psmMenu != null)
                    {
                        psmMenu.SizeFull = true;
                        psmMenu.OnImageButtonMouseUp(btnReal, null);
                        psmMenu.SizeFull = false;
                    }
                }
            }
            else if (nBtnID >= ID.ID_SAVE_DATA && nBtnID <= ID.ID_MANAGE_EARTHQUAKE) // 관리
            {
                if (mCurrentTab != ContentOwnerTab.REPORT_TAB)
                {
                    btnTopTab_Click(btnAdminTab, e);
                }

                OnImageButtonMouseUp(btnReal, new MouseEventArgs(MouseButtons.Left, 1, 1, 1, 1));
            }
            else
            {
                if (mCurrentTab != ContentOwnerTab.M3D_TAB)
                {
                    btnTopTab_Click(btn3DTab, e);
                }

                OnClickToolBarButton(btnReal, e);
            }
        }

        private void OnClickToolBarButton(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ImageButton button = (ImageButton)sender;
            SetButtonBackColor(button);
             
            if (button.Tag != null && (int)button.Tag == ID.ID_VIEW_HOME)
            {
                if (m_PaneBtnHome.Visible == true)
                {
                    m_PaneBtnHome.Visible = false;
                    return;
                }

                m_PaneBtnHome.SetChildCtrlResize(m_PaneBtnHome, 113, 111);
                m_PaneBtnHome.SetFont();

                Point pt = new Point(btnHome.Location.X + btnHome.Width, btnHome.Location.Y + panelTop.Height + panelTop2.Height);
                Point ptScr = this.PointToScreen(pt); 
                m_PaneBtnHome.Location = ptScr;
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

            m_PageHome.OnClickToolBarButton((ImageButton)sender);

            // 분할화면
            if (button == m_PaneBtnHome.BtnMainHome || button == m_PaneBtnHome.Btn14Home || button == m_PaneBtnHome.Btn56Home || button == m_PaneBtnHome.BtnCoalHome)
            {
                m_PaneBtnHome.Visible = false;
                SetButtonBackColor(btnHome);
            }
             
            if (button.Tag != null && (int)button.Tag == ID.ID_VIEW_SIMULATOR)
            {
                btnSimulator_Click(null, null);
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
            if (lblFireText.Text == RaiseManualFire)
            {
                btnFire.Enabled = bEnable;
                btnFire.Enabled = bEnable;
            }

            if (nCase == 2)
            {
                lblFireText.Text = RaiseManualFire;

                panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1_Black;
                panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2_Black;
                btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3_Black; 
                btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Black_Click;
                btnFire.Refresh();
            }
            else if (nCase == 1)
            {
                lblFireText.Text = CloseManualFire; 
            }

            lblFireText.Location = new Point(btnFire.Width / 2 - lblFireText.Width / 2, btnFire.Height / 2 - lblFireText.Height / 2);

            if (nCase == 2)
            {
                //btnFire.Enabled = bEnable;
                btnFire.Enabled = bEnable;

                SetVisibleDisasterPanel(bEnable);
                SetInfoMessage(PageBackstageHome.Instance.ContentForm.ManualClickZone.DisplayText);
                
            }

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
                SetButtonBackColor(btnHome);
            }
        }

        private void OnClickLayerToolBarButton(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ImageButton btn = (ImageButton)sender;
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

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            ImageButton btn = (ImageButton)sender;
            m_PageHome.OnCommandExcute(GetButtonID(btn));

            CheckReportButton(btn);
        }

        #region IImageButtonOwner 멤버

        public void OnImageButtonMouseDown(object sender, MouseEventArgs e)
        {
            
        }

        public void OnImageButtonMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;
            
            ImageButton btn = (ImageButton)sender;
            m_PageHome.OnCommandExcute(GetButtonID(btn));

            SetButtonBackColor(btn);

            if (mCurrentTab == UnE.View.Content.ContentOwnerTab.REPORT_TAB)    
                CheckReportButton(btn); 
        }

        #endregion 

        public void CheckReportButton(ImageButton btn)
        {
            //panelMiddle.Visible = false;
            panelProcessHistory.Visible = false;
            panelReactionHistory.Visible = false;
            pnDetectPSM.Visible = false;
            pnNotOperationPSM.Visible = false;
            pnActionPSM.Visible = false;
            pnSMSPSM.Visible = false;
            EnableProcZone(true);

            switch (Convert.ToInt32(btn.Tag))
            {
                case ID.ID_BTN_DETECT_ANALYZE:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(true);

                    m_PageHome.FrmReport.ShowDetectAnalyze();
                    
                    proc_btnSelectZone_Click(proc_btnSelectZone, null);
                    //proc_btnSelectZone.PerformClick();

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

                    lblViewCount.Visible = false;
                    proc_cboViewCount.Visible = false;

                    m_PageHome.FrmReport.ShowDetectReport();

                    proc_btnSelectZone_Click(proc_btnSelectZone, null);
                    //proc_btnSelectZone.PerformClick(); 

                    break;

                case ID.ID_BTN_NOTOPERATION:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(false);

                    m_PageHome.FrmReport.ShowProcessHistoryReport();

                    proc_btnSelectZone_Click(proc_btnSelectZone, null);
                    //proc_btnSelectZone.PerformClick();

                    break;

                case ID.ID_BTN_ACTION:
                    SelectActionPage(); 
                    break;

                case ID.ID_BTN_SMSREPORT:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(false);

                    m_PageHome.FrmReport.ShowSmsHistoryReport();

                    proc_btnSelectZone_Click(proc_btnSelectZone, null);
                    //proc_btnSelectZone.PerformClick();

                    break;

                case ID.ID_BTN_DETECT_PSM_ANALYZE:
                    pnDetectPSM.Visible = true;
                    m_PageHome.FrmReport.ShowDetectPSMAnalyze(); 

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

                    lblDetectPSMViewCount.Visible = cboDetectPSMViewCount.Visible = false;

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

                    RefreshReportNotOperationPSM(true);

                    break;

                case ID.ID_BTN_ACTION_PSM:
                    SelectPSMActionPage(); 
                    break;

                case ID.ID_BTN_SMSREPORT_PSM:

                    pnSMSPSM.Visible = true;

                    m_PageHome.FrmReport.ShowSMSPSMReport(); 
                    RefreshReportSMSPSM(true);

                    break;

                case ID.ID_BTN_DETECT_INTRUSION_ANALYZE: 
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(true);

                    m_PageHome.FrmReport.ShowDetectIntrusionAnalyze();
                    proc_btnSelectZone_Click(proc_btnSelectZone, null);
                    //proc_btnSelectZone.PerformClick(); 

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
                    proc_btnSelectZone_Click(proc_btnSelectZone, null);
                    //proc_btnSelectZone.PerformClick();

                    break;

                case ID.ID_BTN_NOTOPERATION_INTRUSION:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(false);

                    m_PageHome.FrmReport.ShowProcessIntrusionHistoryReport();
                    proc_btnSelectZone_Click(proc_btnSelectZone, null);
                    //proc_btnSelectZone.PerformClick();

                    break;

                case ID.ID_BTN_ACTION_INTRUSION:
                    SelectActionIntrusionPage(); 
                    break;

                case ID.ID_BTN_SMSREPORT_INTRUSION:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(false);

                    m_PageHome.FrmReport.ShowSmsIntrusionHistoryReport();
                    proc_btnSelectZone_Click(proc_btnSelectZone, null);
                    //proc_btnSelectZone.PerformClick();

                    break;

                case ID.ID_BTN_DETECT_EARTHQUAKE:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(true);
                    EnableProcZone(false);

                    m_PageHome.FrmReport.ShowDetectEarthquakeReport();
                    proc_btnSelectZone_Click(proc_btnSelectZone, null);
                    break;
                    
                case ID.ID_BTN_ACTION_EARTHQUAKE:
                    SelectActionEarthquakePage();
                    break;

                case ID.ID_BTN_DETECT_TH_ANALYZE:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(true);

                    m_PageHome.FrmReport.ShowDetectTHAnalyze();
                    proc_btnSelectZone_Click(proc_btnSelectZone, null);

                    btnDateFormat.Visible =
                    lblSplitUnit.Visible =
                    lblSplitUnitDetail.Visible =
                    proc_cboSplitUnit.Visible =
                    nudSplitUnitDetail.Visible =
                    labelDetectDateFormat.Visible = false;
                    break;

                case ID.ID_BTN_DETECT_TH:
                    panelProcessHistory.Visible = true;
                    EnableSubViewOption(true);

                    lblViewCount.Visible = false;
                    proc_cboViewCount.Visible = false;

                    m_PageHome.FrmReport.ShowDetectTHReport();

                    proc_btnSelectZone_Click(proc_btnSelectZone, null);
                    break;

                case ID.ID_BTN_ACTION_TH:
                    SelectActionTHPage();
                    break;
            }

            int nSpace3 = 26;
            if (m_Resolution == Resolution.FullHD)
            { 
                nSpace3 = (int)(nSpace3 * 0.5);
            }
            ReportCtrlWidthLineUp(nSpace3, true);
        }
        public void SelectActionIntrusionPage(int nSensorZoneHistoryID = 0)
        {
            this.Cursor = Cursors.WaitCursor;

            lblFireSelect.Text = "방범선택";
            // 화재
            react_cboSearchTypeFire.Visible = false;
            cboActionFireSelect.Visible = false;
            btnReactionFireSelectDisaster.Visible = false;

            // 방범
            react_cboSearchTypeIntrusion.Visible = true;
            cboActionIntrusionSelect.Visible = true;
            btnReactionIntrusionSelectDisaster.Visible = true;

            // 지진
            react_cboSearchTypeEarthquake.Visible = false;
            cboActionEarthquakeSelect.Visible = false;
            btnReactionEarthquakeSelectDisaster.Visible = false;

            // 온/습도
            react_cboSearchTypeTH.Visible = false;
            cboActionTHSelect.Visible = false;
            btnReactionTHSelectDisaster.Visible = false;

            DateTime dtToday = DateTime.Now; 
            panelReactionHistory.Visible = true;

            m_PageHome.FrmReport.ShowReactionIntrusionHistoryReport();
             
            //btnReactionIntrusionSelectDisaster.PerformClick();

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
             
            if (nSensorZoneHistoryID != 0)
                m_PageHome.FrmReport.SelectPSMActionPage(nSensorZoneHistoryID);

            panelReactionHistory.Refresh();
            //btnReactionPSMSelectDisaster.PerformClick();

            this.Cursor = Cursors.Arrow;
        }

        public void SelectActionEarthquakePage(int nSensorZoneHistoryID = 0)
        {
            this.Cursor = Cursors.WaitCursor;
            
            lblFireSelect.Text = "지진선택";

            // 화재
            react_cboSearchTypeFire.Visible = false;
            cboActionFireSelect.Visible = false;
            btnReactionFireSelectDisaster.Visible = false;

            // 방범
            react_cboSearchTypeIntrusion.Visible = false;
            cboActionIntrusionSelect.Visible = false;
            btnReactionIntrusionSelectDisaster.Visible = false;

            // 지진
            react_cboSearchTypeEarthquake.Visible = true;
            cboActionEarthquakeSelect.Visible = true;
            btnReactionEarthquakeSelectDisaster.Visible = true;

            // 온/습도
            react_cboSearchTypeTH.Visible = false;
            cboActionTHSelect.Visible = false;
            btnReactionTHSelectDisaster.Visible = false;

            panelReactionHistory.Visible = true;

            m_PageHome.FrmReport.ShowActionEarthquakeReport();
             
            panelReactionHistory.Refresh();
            //btnReactionSelectDisaster.PerformClick();

            if (nSensorZoneHistoryID > 0)
                m_PageHome.FrmReport.SelectActionEarthquakePage(nSensorZoneHistoryID);

            this.Cursor = Cursors.Arrow;
        }

        public void SelectActionTHPage(int nSensorZoneHistoryID = 0)
        {
            this.Cursor = Cursors.WaitCursor;

            lblFireSelect.Text = "온/습도선택";

            // 화재
            react_cboSearchTypeFire.Visible = false;
            cboActionFireSelect.Visible = false;
            btnReactionFireSelectDisaster.Visible = false;

            // 방범
            react_cboSearchTypeIntrusion.Visible = false;
            cboActionIntrusionSelect.Visible = false;
            btnReactionIntrusionSelectDisaster.Visible = false;

            // 지진
            react_cboSearchTypeEarthquake.Visible = false;
            cboActionEarthquakeSelect.Visible = false;
            btnReactionEarthquakeSelectDisaster.Visible = false;

            // 온/습도
            react_cboSearchTypeTH.Visible = true;
            cboActionTHSelect.Visible = true;
            btnReactionTHSelectDisaster.Visible = true;

            panelReactionHistory.Visible = true;

            m_PageHome.FrmReport.ShowActionTHReport();

            panelReactionHistory.Refresh();
            //btnReactionSelectDisaster.PerformClick();

            if (nSensorZoneHistoryID > 0)
                m_PageHome.FrmReport.SelectActionTHePage(nSensorZoneHistoryID);

            this.Cursor = Cursors.Arrow;
        }

        public void SelectActionPage(int nSensorZoneHistoryID = 0)
        {
            this.Cursor = Cursors.WaitCursor;

            lblFireSelect.Text = "화재선택";
            // 화재
            react_cboSearchTypeFire.Visible = true;
            cboActionFireSelect.Visible = true;
            btnReactionFireSelectDisaster.Visible = true;

            // 방범
            react_cboSearchTypeIntrusion.Visible = false;
            cboActionIntrusionSelect.Visible = false;
            btnReactionIntrusionSelectDisaster.Visible = false;

            // 지진
            react_cboSearchTypeEarthquake.Visible = false;
            cboActionEarthquakeSelect.Visible = false;
            btnReactionEarthquakeSelectDisaster.Visible = false;

            // 온/습도
            react_cboSearchTypeTH.Visible = false;
            cboActionTHSelect.Visible = false;
            btnReactionTHSelectDisaster.Visible = false;

            panelReactionHistory.Visible = true;

            m_PageHome.FrmReport.ShowReactionHistoryReport();

            panelReactionHistory.Refresh();
            //btnReactionSelectDisaster.PerformClick();

            if (nSensorZoneHistoryID > 0)
                m_PageHome.FrmReport.SelectActionPage(nSensorZoneHistoryID);

            this.Cursor = Cursors.Arrow;
        }

        private void EnableSubViewOption(bool isView)
        {
            btnDateFormat.Visible =
            lblSplitUnit.Visible = // 단위
            proc_cboSplitUnit.Visible = // 단위 콤보
            labelDetectDateFormat.Visible = // 날짜 형식            
            nudSplitUnitDetail.Visible =
            lblSplitUnitDetail.Visible =  // 단위 마다            
            lblViewCount.Visible = // 최대 표기
            proc_cboViewCount.Visible = isView;
        }

        /// <summary>
        /// 지진 리포트에는 위치 선택 콤보가 필요없음
        /// </summary>
        /// <param name="isView"></param>
        private void EnableProcZone(bool isView)
        {
            proc_lblSelectZone.Visible =
            proc_cboBuildingGroup.Visible =
            proc_cboBuilding.Visible =
            proc_cboFloor.Visible = isView;
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
                proc_btnStartDate.ButtonText = strDate;
                react_btnStartDate.ButtonText = strDate;
                btnDetectPSMStartDate.ButtonText = strDate;
                btnNotOperationPSMStartDate.ButtonText = strDate;
                btnActionPSMStartDate.ButtonText = strDate;
                btnSMSPSMStartDate.ButtonText = strDate;

                proc_btnStartDate.Refresh();
                react_btnStartDate.Refresh();
                btnDetectPSMStartDate.Refresh();
                btnNotOperationPSMStartDate.Refresh();
                btnActionPSMStartDate.Refresh();
                btnSMSPSMStartDate.Refresh();
            }
            else
            {
                proc_btnEndDate.ButtonText = strDate;
                react_btnEndDate.ButtonText = strDate;
                btnDetectPSMEndDate.ButtonText = strDate;
                btnNotOperationPSMEndDate.ButtonText = strDate;
                btnActionPSMEndDate.ButtonText = strDate;
                btnSMSPSMEndDate.ButtonText = strDate;

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

        private void ClickReportDateButton(ImageButton btn, DateTimePicker timePicker, Panel pn)
        {
            HideDateTimePicker();

            if (IsDate(btn.ButtonText))
                timePicker.Value = System.Convert.ToDateTime(btn.ButtonText);

            int x = btn.Left;
            int y = (btn.Top + btn.Height - 22);

            Point pt = pn.PointToScreen(new Point(x, y));
            timePicker.Location = new Point(pt.X - FormFrame.Instance.Location.X, pt.Y - FormFrame.Instance.Location.Y);
            timePicker.DropDownAlign = LeftRightAlignment.Left;

            //if (m_Resolution == Resolution.FourK)
            //    timePicker.Size = new System.Drawing.Size();
            timePicker.Show();

            timePicker.Select();
            SendKeys.Send("%{DOWN}");
        }

        private bool ChangeReportDateTime(DateTimePicker timePicker, ImageButton btn)
        {
            DateTime dtToday = DateTime.Now;
            string szText = timePicker.Value.ToShortDateString();
            DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);

            if (dtszText > dtToday)
            {
                MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                return false;
            }

            btn.ButtonText = szText;
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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;
            
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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;
            
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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;
            
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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ClickReportDateButton(btnDetectPSMStartDate, DatePickerDetectPSMStart, pnDetectPSM);
        }

        private void btnDetectPSMEndDate_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ClickReportDateButton(btnNotOperationPSMStartDate, DatePickerNotOperationPSMStart, pnNotOperationPSM);
        }

        private void btnNotOperationPSMEndDate_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ClickReportDateButton(btnActionPSMStartDate, DatePickerActionPSMStart, pnActionPSM);
        }

        private void btnActionPSMEndDate_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ClickReportDateButton(btnSMSPSMStartDate, DatePickerSMSPSMStart, pnSMSPSM);
        }

        private void btnSMSPSMEndDate_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

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
                else if (nPage == Data.ReportMode.DetectEarthquake || nPage == Data.ReportMode.ActionEarthquake)
                    m_PageHome.FrmReport.LoadReportForEarthquake(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
                else if (nPage == Data.ReportMode.ActionTH || nPage == Data.ReportMode.DetectTH || nPage == Data.ReportMode.DetectTHAnalyze)
                    m_PageHome.FrmReport.LoadReportForTH(strGroupName, strBuildingName, strFloorName, startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
                else
                    m_PageHome.FrmReport.LoadReport(strGroupName, strBuildingName, strFloorName, startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void ImageButtonPerfClick(ImageButton btn)
        {
            
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

        private bool CheckValiedPriodDate(bool isPopMessage, ImageButton btnStart, ImageButton btnEnd, ref DateTime dtStart, ref DateTime dtEnd)
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

        public void RefreshReportIntrusion()
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

        public void RefreshReportEarthquake()
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
                
                int nSplitUnitOfMeansure = proc_cboSplitUnit.SelectedIndex;
                int nViewCount = Convert.ToInt32(proc_cboViewCount.SelectedItem);
                int nSplitUnitOfMeansureDetail = Convert.ToInt32(nudSplitUnitDetail.Value);

                m_PageHome.FrmReport.LoadReportForEarthquake(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount);
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



        public void ShowLeftThumbnail(bool bSituation, ArrayList arrCCTVs)
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
            //    //int nFrmHeight = (panel_mainBackV.Size.Height - nLineThick * 4) / 3;

            //    panel_mainBackV.Visible = false;

            //    PageHome.ShowSituationCCTV(bSituation);

            //    m_isThumbnailMode = true;
            //    FormMain_Resize(null, null);
            //}
            //else
            {
                PageHome.ShowSituationCCTV(bSituation, arrCCTVs);
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
                            string szZoneName = zone.LinkedZone == null ? zone.ZoneName : zone.LinkedZone.DisplayText;
                            mLabelZone.Text = text1 + "," + szZoneName;
                            mLabelZone.Location = new Point(panelStatus.Width - mLabelZone.Width - 10, 10);
                            return zone.LinkedZone;
                        }
                        else if (sensor.Type == IFacility.FacilityType.SecomExternalAlarmBell)  //외부 비상벨은 zone.Building이 없으므로...by hypark
                        {
                            //string text1 = zone.Building.BuildingGroup.BuildingGroupName;
                            string szZoneName = zone.LinkedZone == null ? zone.ZoneName : zone.LinkedZone.DisplayText;
                            mLabelZone.Text = szZoneName;
                            mLabelZone.Location = new Point(panelStatus.Width - mLabelZone.Width - 10, 10);
                            return zone.LinkedZone;                         //외부 비상벨 타겟팅을 위해서 꼭 넘겨줘야 한다. by hypark.
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

        public void SetVisibleDisasterPanel(bool isDisaster)
        {
            if (mCurrentTab == UnE.View.Content.ContentOwnerTab.M2D_TAB || mCurrentTab == UnE.View.Content.ContentOwnerTab.M3D_TAB || mCurrentTab == UnE.View.Content.ContentOwnerTab.CCTV_TAB)
            {
                panelStatus.Visible = isDisaster;
                panelLog.Visible = isDisaster;
                btnFire.Visible = isDisaster;

                if (isDisaster)
                {
                    panelBottom.Location = new Point(panelStatus.Location.X, panelStatus.Location.Y + panelStatus.Height);
                }
                else
                    panelBottom.Location = new Point(panelLeft2.Location.X + panelLeft2.Size.Width, panelLeft2.Location.Y);
            }
            else
            {
                panelStatus.Visible = false;
                panelLog.Visible = false;
                btnFire.Visible = false;
            }
        }

        public void BeginNotifyProcess(ReactionLog log, ISensor sensor)
        { 
            int nHistoryID = log.SensorHistoryID;

            if (log.ReactionType == (int)ReactionType.NOTIFY_SIGNAL && sensor != null)
            {
                if (IFacility.IsFireSensorType(sensor.Type))
                {
                    if (log.Message.IndexOf("[훈련상황]") != -1)
                        StatusLableText = "[훈련]화재 발생";
                    else
                        StatusLableText = "화재 발생";
                }
                else if (IFacility.IsPSMSensorType(sensor.Type))
                {
                    if (log.Message.IndexOf("[훈련상황]") != -1)
                        StatusLableText = "[훈련]누출 발생";
                    else
                        StatusLableText = "누출 발생";
                }
                else if (IFacility.IsSecurityType(sensor.Type))
                {
                    if (log.Message.IndexOf("[훈련상황]") != -1)
                        StatusLableText = "[훈련]방범 상황";
                    else
                        StatusLableText = "방범 상황";
                }
            }
            /*else if (log.ReactionType == (int)ReactionType.NOTIFY_PSM)
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
            }*/
            //((RealTimeInfoPane)panelLog).TextColor = Color.Red;
            //panelLog.Refresh();
            //mLabelStatus.ForeColor = Color.Red;
            //mLabelZone.ForeColor = Color.Red;

            panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1;
            panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2;
            btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3;
            btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Click;
            btnFire.Refresh();

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
            mLabelZone.Location = new Point(panelStatus.Width - mLabelZone.Width - 10, 10);

            if (isRealMode)
            {
                StatusLableText = "건물 붕괴";
                SetInfoMessage(DateTime.Now.ToLongTimeString() + " " + strMessage);
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

            //mLabelStatus.ForeColor = Color.Orange;
            //mLabelZone.ForeColor = Color.Orange;
            //((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
            //panelLog.Refresh();
            //panelStatus.Refresh();

            panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1;
            panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2;
            btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3;
            btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Click;
            btnFire.Refresh();
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

            //mLabelZone.Text = strPosition;
            mLabelZone.Text = "";

            if (isRealMode)
            {
                StatusLableText = "지진 탐지";
                SetInfoMessage(DateTime.Now.ToLongTimeString() + " " + strMessage);
            }
            else
            {
                StatusLableText = "[훈련상황]지진 탐지";
                SetInfoMessage(DateTime.Now.ToLongTimeString() + " [훈련상황]" + strMessage);
            }

            //mLabelStatus.ForeColor = Color.Orange;
            //mLabelZone.ForeColor = Color.Orange;
            //((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
            //panelLog.Refresh();

            panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1;
            panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2;
            btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3;
            btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Click;
            btnFire.Refresh();
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
                //mLabelStatus.ForeColor = Color.Orange;
                //mLabelZone.ForeColor = Color.Orange;
                //((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
                //panelLog.Refresh();

                panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1_Black;
                panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2_Black;
                btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3_Black;
                btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Black_Click;
            }
            else
            {
                //mLabelStatus.ForeColor = Color.GreenYellow;
                //mLabelZone.ForeColor = Color.GreenYellow;
                //((RealTimeInfoPane)panelLog).TextColor = Color.GreenYellow;
                //panelLog.Refresh();

                panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1_Black;
                panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2_Black;
                btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3_Black;
                btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Black_Click;
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
                //mLabelStatus.ForeColor = Color.Orange;
                //mLabelZone.ForeColor = Color.Orange;
                //((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
                //panelLog.Refresh();

                panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1_Black;
                panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2_Black;
                btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3_Black;
                btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Black_Click;
            }
            else
            {
                //mLabelStatus.ForeColor = Color.GreenYellow;
                //mLabelZone.ForeColor = Color.GreenYellow;
                //((RealTimeInfoPane)panelLog).TextColor = Color.GreenYellow;
                //panelLog.Refresh();

                panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1_Black;
                panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2_Black;
                btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3_Black;
                btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Black_Click;
            }
            SetDetectZoneName(nHistoryID);
        }

        public void SetEarthquakeDetectMode(ReactionLog log)
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
                panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1_Black;
                panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2_Black;
                btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3_Black;
                btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Black_Click;
            }
            else
            {
                panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1_Black;
                panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2_Black;
                btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3_Black;
                btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Black_Click;
            }
            
        }

        public void SetPSMDetectMode(ReactionLog log)
        { 
            bool bTestMode = false;
            int nHistoryID = log.SensorHistoryID;

            if (UnE.SOP.ProxySOP.Instance.SiteID == 3 && (log.Parameter4 == "산소" || log.Parameter4 == "이산화탄소" || log.Parameter4 == "일산화탄소" || log.Parameter4 == "메탄"))
            {
                if (log.Message.IndexOf("훈련상황]") != -1)
                    StatusLableText = "[훈련]밀폐공간 탐지";
                else if (log.Message.IndexOf("테스트]") != -1)
                {
                    StatusLableText = "[테스트]밀폐공간 탐지";
                    bTestMode = true;
                }
                else
                    StatusLableText = "밀폐공간 탐지"; 
            }
            else
            {
                if (log.Message.IndexOf("훈련상황]") != -1)
                    StatusLableText = "[훈련]누출 탐지";
                else if (log.Message.IndexOf("테스트]") != -1)
                {
                    StatusLableText = "[테스트]누출 탐지";
                    bTestMode = true;
                }
                else
                    StatusLableText = "누출 탐지";
            }

            if (bTestMode == false)
            {
                //mLabelStatus.ForeColor = Color.Orange;
                //mLabelZone.ForeColor = Color.Orange;
                //((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
                //panelLog.Refresh();

                panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1_Black;
                panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2_Black;
                btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3_Black;
                btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Black_Click;
            }
            else
            {
                //mLabelStatus.ForeColor = Color.GreenYellow;
                //mLabelZone.ForeColor = Color.GreenYellow;
                //((RealTimeInfoPane)panelLog).TextColor = Color.GreenYellow;
                //panelLog.Refresh();

                panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1_Black;
                panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2_Black;
                btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3_Black;
                btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Black_Click;
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

            //((RealTimeInfoPane)panelLog).TextColor = Color.Orange;
            //panelLog.Refresh();
            //mLabelStatus.ForeColor = Color.Orange;
            //mLabelZone.ForeColor = Color.Orange;

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
            //mLabelStatus.ForeColor = Color.White;
            //mLabelZone.ForeColor = Color.White;

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
            //mLabelStatus.ForeColor = Color.White;
            //mLabelZone.ForeColor = Color.White;

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
            //mLabelStatus.ForeColor = Color.White;
            //mLabelZone.ForeColor = Color.White;

            SetDetectZoneName(nHistoryID);
        }

        private bool m_IsDetect = false;

        public void SetNormalMode(int nHistoryID)
        { 
            StatusLableText = "탐지 신호 없음";
            ((RealTimeInfoPane)panelLog).TextColor = Color.White;
            panelLog.Refresh();
            //mLabelStatus.ForeColor = Color.White;
            //mLabelZone.ForeColor = Color.White;
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

            //btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar_Blue; 
        }

        public void SetNormalMode(ReactionLog log)
        { 
            int nHistoryID = log.SensorHistoryID;
            StatusLableText = "탐지 신호 없음";
            ((RealTimeInfoPane)panelLog).TextColor = Color.White;
            //mLabelStatus.ForeColor = Color.White;
            //mLabelZone.ForeColor = Color.White;
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

            //btnFire.BackgroundImage = global::SDMS.Properties.Resources.Fire_Bar_Blue; 
        }

        public void AddLogMessage(ReactionLog log)
        {
            if (log == null)
                return;
            ((RealTimeInfoPane)panelLog).RealTimeInfo = log.ToString();
            mLabelLog.Text = "";
            mLabelLog.Tag = log;
            ((RealTimeInfoPane)panelLog).DrawMovingText();
            ((RealTimeInfoPane)panelLog).SetLocation();

            //panelLog.DisplayLabelLocation = new Point((int)(panelLog.Size.Width * 0.5 - mLabelLog.Size.Width * 0.5), (int)(panelLog.Size.Height * 0.5 - mLabelLog.Size.Height * 0.5));
        }

        private void btnFire_Click(object sender, EventArgs e)
        {
            if (lblFireText.Text == RaiseManualFire)
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
                    int sensorZoneID = SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.FIRE_SENSOR;

                    int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
                    SDMS.NetworkWebManager.Instance.SendMessage(1, SOPWebServer.Header.NOTIFY_DISASTER, 0, zone.ID, sensorZoneID, nSOPGenUserID);
                    SendDetectMessageToSOPSimulator();

                    panelStatus.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus1;
                    panelLog.BackgroundImage = global::SDMS.Properties.Resources.AlarmStatus2;
                    btnFire.ImageNormal = global::SDMS.Properties.Resources.AlarmStatus3;
                    btnFire.ImageClicked = btnFire.ImageMouseOver = global::SDMS.Properties.Resources.AlarmStatus3_Click;
                    btnFire.Refresh();
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
                    else if (CurrentSensorDetectProcess.ProcessType == ProcessType.FireAlarm)
                    {

                        if (MessageBox.Show("화재상황을 종료하시겠습니까?", "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                            return;
                    }

                    btnFire.Enabled = false;

                    int nHistoryID = CurrentSensorDetectProcess.SensorHistoryID;
                    int nSOPGenUserID = FormMain.Instance.SOPGenUserID;
                    SDMS.NetworkWebManager.Instance.SendMessage(1, SOPWebServer.Header.CLEAR_DETECT_REPORT, nHistoryID, nSOPGenUserID);
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
                    ReactionTypeInfo info = process.LastLog.GetReactionTypeInfo();

                    if (info == ReactionTypeInfo.BEGIN_STATUS || info == ReactionTypeInfo.CHANGE_ALARM_LEVEL)
                    /*if (process.LastLog.ReactionType == (int)(ReactionType.BEGIN_STATUS) || process.LastLog.ReactionType == (int)(ReactionType.BEGIN_PSM_STATUS) ||
                        process.LastLog.ReactionType == (int)(ReactionType.BEGIN_S1ACCESS_STATUS) || process.LastLog.ReactionType == (int)(ReactionType.BEGIN_S1SVMS_STATUS) ||
                        process.LastLog.ReactionType == (int)(ReactionType.CHANGE_PSM_ALARM_DEPTH))*/
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
                                lblFireText.Text = CloseManualFire;
                                btnFire.Enabled = true; 
                            }
                            else
                            {
                                lblFireText.Text = RaiseManualFire;
                                btnFire.Enabled = false;
                            }

                            lblFireText.Location = new Point(btnFire.Width / 2 - lblFireText.Width / 2, btnFire.Height / 2 - lblFireText.Height / 2);

                            if (m_frmRemoteCtrl != null && m_frmRemoteCtrl.Visible)
                            {
                                m_frmRemoteCtrl.SensorDectect();
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
                        //if (processSelect != null && processSelect.LastLog != null)
                        //{
                        //    if (processSelect.LastLog.ReactionType == (int)(ReactionType.BEGIN_STATUS) || processSelect.LastLog.ReactionType == (int)ReactionType.BEGIN_PSM_STATUS)
                        //    {
                        //        int nSensorID = processSelect.DetectSensorID;
                        //        int nHistoryID = processSelect.SensorHistoryID;
                        //        SeletCaseData form = new SeletCaseData(processSelect.ProcessType, processSelect.TargetSensor, nHistoryID, processSelect.ShowOpenSOP, processSelect.DetectTime);
                        //        ConfirmDialogManager.Instance.AddDialogFirst(form);
                        //        SeletCaseData form2 = ConfirmDialogManager.Instance.ShowDialogNext();
                        //        if (form2 != null)
                        //        {
                        //            int nID = form2.SensorHistoryID;
                        //            int nSensorID2 = form2.Sensor.ID;
                        //            FormMain.Instance.SelectSensorDetectProcess(nID, nSensorID2);
                        //        }
                        //    }
                        //}
                    }
                }
            }
            else
            {
                if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
                {
                    // (신호가 현재 보여주고 있는 2d view에 있는 신호인지 여부 아니라면 change view 해야함);
                    this.PageHome.ContentForm.IsSameCampus(process.TargetZone.Building.BuildingGroup);
                }

                SetNormalMode(0);

                //if (ShowEquipZoneCCTV == false)
                {
                    string strSceneName;

                    if (m_useEquipZoneVolume && m_dicBuildingGroupScene.TryGetValue(1, out strSceneName))
                    {
                        SelectScene(strSceneName, null);
                        /*this.PageHome.ContentForm.HideAllAlarmZones();
                        this.PageHome.ContentForm.SelectScene(strSceneName);*/
                    }
                    else
                    {
                        FormMain.Instance.PageHome.ContentForm.RestoreViewState();
                    }
                }
                //OnClickToolBarButton(m_PaneBtnHome.BtnMainHome, null);				
            }
            process.HideCCTV();

            RemoveNotice(process);
            SetVisibleDisasterPanel(m_IsDisaster);
            if (m_frmRemoteCtrl != null && m_frmRemoteCtrl.Visible)
            {
                m_frmRemoteCtrl.SensorDectect();
            }
        }

        private void SelectScene(string strSceneName, Zone zone)
        {
            this.PageHome.ContentForm.HideAllAlarmZones();
            this.PageHome.ContentForm.SelectScene(strSceneName);

            int nLayerState = ReadLayerState();
            bool showCCTV = (nLayerState & (int)LAYER_TYPE.CCTV) == (int)LAYER_TYPE.CCTV;

            ISensorTooltipOwner owner = this.PageHome.ContentForm.OutdoorView;
            owner.ClearPOI("");

            if (zone == null)
                return;

            CCTVManager.Instance.LoadCCTVFile(owner, true, zone.ID, showCCTV);
        }

        private void SelectScene(string strSceneName, Zone zone, string strAlarmZone, bool hideAllOthers)
        {
            SelectScene(strSceneName, zone);
            this.PageHome.ContentForm.ShowAlarmZone(strAlarmZone, hideAllOthers);
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

        public void RemoveAllSecuritySensorDetect()
        {
            List<ProcessIF> rList = ProcessManager.Instance.GetAllSecurityAlarmProcess();
            for (int i = 0; i < rList.Count; i++)
            {
                ProcessIF process = rList[i];
                ProcessManager.Instance.RemoveProcess(process);
                RemoveSensorDetect(process, false);
            }
            SelectLastFireDectectProcess();
        }

        public void AddSensorDectectInvoke(ProcessIF process, bool bAddSelect, bool bCallSelect)
        {
            this.Invoke((MethodInvoker)delegate
            {
                AddSensorDectect(process, bAddSelect, bCallSelect);
            });
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
                //SetVisibleDisasterPanel(m_IsDisaster);
                if (m_frmRemoteCtrl != null && m_frmRemoteCtrl.Visible)
                {
                    m_frmRemoteCtrl.SensorDectect();

                    //Point pt = new Point(m_frmRemoteCtrl.Location.X, m_frmRemoteCtrl.Location.Y);
                    //Point ptScr = this.PointToScreen(pt);
                    
                    for (int i = 0; i < Screen.AllScreens.Count(); i++)
                    {
                        Screen sc2 = Screen.AllScreens[i];

                        Size MainFrameSize = m_frmRemoteCtrl.Size;
                        
                        if (m_frmRemoteCtrl.Location.X >= sc2.Bounds.X && sc2.Bounds.X + sc2.Bounds.Width > m_frmRemoteCtrl.Location.X &&
                            m_frmRemoteCtrl.Location.Y >= sc2.Bounds.Y && sc2.Bounds.Y + sc2.Bounds.Height > m_frmRemoteCtrl.Location.Y)
                        {
                            int allY = MainFrameSize.Height + m_frmRemoteCtrl.Location.Y;
                            if (allY > sc2.Bounds.Height)
                            {
                                int sum = allY - sc2.Bounds.Height;
                                m_frmRemoteCtrl.Location = new Point(m_frmRemoteCtrl.Location.X, m_frmRemoteCtrl.Location.Y - sum);
                            }

                            break;
                        }
                    }
                }
            }

            DlgSelectCase.Instance.DetectFireCount = cmbFireDetect.Items.Count;
        }

        #region panel_mainBackV 알림 함수
        private void AddNotice(ProcessIF process)
        { 
            m_PanelBtnNotice.m_noticeListItem.Add(process);
            m_PanelBtnNotice.RefreshList();
            ChangeNotice(process);

            SetCountNotice();

            if (m_PanelBtnNotice.m_noticeListItem.Count > 1 && !m_PanelBtnNotice.Visible)
            { 
                //Point pt = new Point(panelLeft2.Location.X + panelLeft2.Width, panelStatus.Location.Y + panelStatus.Height);
                //Point ptScr = this.PointToScreen(pt);
                //m_PanelBtnNotice.Location = ptScr;// PointToClient(ptScr);
                //m_PanelBtnNotice.Visible = true;
            }

            if (m_PanelBtnNotice.m_noticeListItem.Count > 0)
            {
                btnLayerNotice.ImageNormal = global::SDMS.Properties.Resources.AlarmOn;
                btnLayerNotice.ImageMouseOver = global::SDMS.Properties.Resources.AlarmOn;
                btnLayerNotice.Refresh();             
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
                btnLayerNotice.ImageNormal = global::SDMS.Properties.Resources.Alarm;
                btnLayerNotice.ImageMouseOver = global::SDMS.Properties.Resources.Alarm;
                btnLayerNotice.Refresh();
                m_PanelBtnNotice.Visible = false;
            }
            else if (m_PanelBtnNotice.m_noticeListItem.Count > 0)
            {
                btnLayerNotice.ImageNormal = global::SDMS.Properties.Resources.AlarmOn;
                btnLayerNotice.ImageMouseOver = global::SDMS.Properties.Resources.AlarmOn;
                btnLayerNotice.Refresh();
            }
        }
        private void ClearNotice()
        { 
            m_PanelBtnNotice.m_noticeListItem.Clear();
            m_PanelBtnNotice.m_dicCountUpNoticeList.Clear(); 
            m_PanelBtnNotice.Controls.Clear();

            SetCountNotice();

            m_PanelBtnNotice.Visible = false;
            btnLayerNotice.ImageNormal = global::SDMS.Properties.Resources.Alarm;
            btnLayerNotice.ImageMouseOver = global::SDMS.Properties.Resources.Alarm;
            btnLayerNotice.Refresh();
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
            //if (m_PanelBtnNotice.Visible == true || m_PanelBtnNotice.m_noticeListItem.Count == 0)
            //{
            //    m_PanelBtnNotice.Visible = false;
            //    btnLayerNotice.ImageNormal = global::SDMS.Properties.Resources.Alarm;
            //    btnLayerNotice.ImageMouseOver = global::SDMS.Properties.Resources.Alarm;
            //    btnLayerNotice.Refresh();
            //    return;
            //}

            //SetButtonBackColor(btnLayerNotice);
            ////btnLayerNotice.BackgroundImage = global::SDMS.Properties.Resources.Layer_Notice_Checked;

            //Point pt = new Point(panelLeft2.Location.X + panelLeft2.Width, panelStatus.Location.Y + panelStatus.Height);
            //Point ptScr = this.PointToScreen(pt);
            //m_PanelBtnNotice.Location = ptScr; 
            //m_PanelBtnNotice.Show(this);
            return;
        }

        void m_PaneBtnNotice_chgSensorDectect(ProcessIF process)
        {
            cmbFireDetect.SelectedItem = process;
            cmbFireDetect_SelectionChangeCommitted(this, new EventArgs());
        }
        private void SetCountNotice()
        {
            //if (btnLayerNotice.Controls != null)
            //{
            //    if (btnLayerNotice.Controls[0] is PictureBox)
            //    {
            //        PictureBox pic = btnLayerNotice.Controls[0] as PictureBox;
            //        if (pic.Controls != null && pic.Controls.Count > 0)
            //        {
            //            if (pic.Controls[0] is Label)
            //            {
            //                Label label = pic.Controls[0] as Label;
            //                label.Text = m_PanelBtnNotice.m_noticeListItem.Count.ToString();

            //                if (m_PanelBtnNotice.m_noticeListItem.Count == 0) 
            //                    pic.Visible = false; 
            //                else if (m_PanelBtnNotice.m_noticeListItem.Count >= 10)
            //                {
            //                    pic.Visible = true;
            //                    label.Location = new Point(0, 1);
            //                    label.Font = new Font("나눔바른고딕", 6.5F);
            //                }
            //                else
            //                {
            //                    pic.Visible = true;
            //                    label.Location = new Point(1, 1);
            //                    label.Font = new Font("나눔바른고딕", 8F);
            //                } 
            //            }
            //        }
            //    }
            //}  
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
            ArrayList arContorl = new ArrayList();
            arContorl.Add(react_btnEndDate);
            arContorl.Add(react_btnStartDate);
            arContorl.Add(react_cboSearchTypeFire);
            arContorl.Add(cboActionFireSelect);
            arContorl.Add(react_cboEndTime);
            arContorl.Add(react_cboStartTime);
            arContorl.Add(btnReactionFireSelectDisaster);
            return arContorl;
        }

        public ArrayList GetActionIntrusionContorl()
        {
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

        public ArrayList GetActionEarthquakeContorl()
        {
            ArrayList arContorl = new ArrayList();
            arContorl.Add(react_btnEndDate);
            arContorl.Add(react_btnStartDate);
            arContorl.Add(react_cboSearchTypeEarthquake);
            arContorl.Add(cboActionEarthquakeSelect);
            arContorl.Add(react_cboEndTime);
            arContorl.Add(react_cboStartTime);
            arContorl.Add(btnReactionEarthquakeSelectDisaster);
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

        public ArrayList GetActionTHContorl()
        {
            ArrayList arContorl = new ArrayList();
            arContorl.Add(react_btnEndDate);
            arContorl.Add(react_btnStartDate);
            arContorl.Add(react_cboSearchTypeTH);
            arContorl.Add(cboActionTHSelect);
            arContorl.Add(react_cboEndTime);
            arContorl.Add(react_cboStartTime);
            arContorl.Add(btnReactionTHSelectDisaster);
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

        FormReciverState m_formReciverState = null;

        private void btnSensorMonitor_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;
            
            if (m_formReciverState == null || m_formReciverState.IsDisposed) 
                m_formReciverState = new FormReciverState(); 

            if (m_formReciverState != null && !m_formReciverState.Visible)
                m_formReciverState.SetChildCtrlResize(m_formReciverState, 395, 319);  

            if (m_formReciverState.Visible)
                m_formReciverState.Visible = false;
            else
            {

                int x = btnSensorMonitor.Location.X;
                //if (sender is Label)
                //{
                //    Label label = (Label)sender;
                //    x = label.Location.X;
                //}
                //else
                //{
                //    ImageButton button = (ImageButton)sender;
                //    x = button.Location.X;
                //}

                Point pt = new Point(x, panelTop2.Location.Y + panelTop2.Height);
                Point ptScr = this.PointToScreen(pt);
                m_formReciverState.Show(this);
                m_formReciverState.Location = ptScr;// PointToClient(ptScr);
            }
             
            //PageBackstageHome.ShowTranslucentForm(frm, 300, 200, frm.Size.Width, frm.Size.Height, ID.ID_VIEW_SENSOR_MONITOR);
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
                    labelSensorMonitor.ForeColor = Color.FromArgb(0x3f, 0x3f, 0x3f);
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
                            lblFireText.Text = CloseManualFire;
                            btnFire.Enabled = true;
                        }
                        else
                        {
                            lblFireText.Text = RaiseManualFire;
                            btnFire.Enabled = false;                            
                        }

                        lblFireText.Location = new Point(btnFire.Width / 2 - lblFireText.Width / 2, btnFire.Height / 2 - lblFireText.Height / 2);

                        CountUpNotice(process);
                        ChangeNotice(process);

                        string strVolume, strSceneName;

                        if (m_useEquipZoneVolume && m_dicEquiZoneVolume.TryGetValue(process.TargetZone.ID, out strVolume) && m_dicZoneScene.TryGetValue(zone.ID, out strSceneName))
                        {
                            SelectScene(strSceneName, process.TargetZone.LinkedZone, strVolume, true);
                            /*this.PageHome.ContentForm.HideAllAlarmZones();
                            this.PageHome.ContentForm.SelectScene(strSceneName);
                            this.PageHome.ContentForm.ShowAlarmZone(strVolume, true);*/
                        }
                    }
                }
            }
        }

        private void btnSaveHome_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ImageButton button = (ImageButton)sender;
            SetButtonBackColor(button);

            if (m_PaneBtnSaveHome.Visible == true)
            {
                m_PaneBtnSaveHome.Visible = false; 
                return;
            }

            m_PaneBtnSaveHome.SetChildCtrlResize(m_PaneBtnSaveHome, 112, 148);
            m_PaneBtnSaveHome.SetFont();

            Point pt = new Point(button.Location.X, panelTop2.Location.Y + panelTop2.Height);            
            Point ptScr = this.PointToScreen(pt);
            m_PaneBtnSaveHome.Location = ptScr;// PointToClient(ptScr);
            m_PaneBtnSaveHome.Show(this);
            return;
        }

        private void btnSaveHomeSub_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ImageButton button = (ImageButton)sender;

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
            SetButtonBackColor(btnSaveHome);
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
            proc_btnSelectZone_Click(proc_btnSelectZone, null);
            //proc_btnSelectZone.PerformClick();
        }

        // Return 값 : true이면 기후정보 표시 창을 사용한다.
        private bool GetWeatherInfoOption()
        {
            string strPropertyName = "ShowWeatherInfo";

            string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='" + strPropertyName + "' AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            int nOption = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            if (nOption == 1)
                return true;

            return false;
        }

        // Return 값 : true이면 기후정보 표시 창을 사용한다.
        private void ReadLeftBarThumbnailOption()
        {
            string strPropertyName = "LeftThumbnailOption";

            string strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='" + strPropertyName + "' AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strOption = WebDBManager.GetStringField(arrResult[0], "");

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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            if (m_PanelBtnSimulator.Visible == true)
            {
                m_PanelBtnSimulator.Visible = false;
                return;
            }

            string strSQL = String.Format("SELECT PropertyValue FROM OptionSDMS WHERE PropertyName = 'TestSimulator' AND SiteID = {0}", UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            string strOptionString = string.Empty;

            if (arrResult != null)
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    strOptionString = WebDBManager.GetStringField(arrResult[i]);
                }
            }

            if (String.IsNullOrWhiteSpace(strOptionString))
                return;

            strSQL = String.Format("SELECT PropertyValue FROM OptionSDMS WHERE PropertyName = '{0}' AND SiteID = {1}", strOptionString, UnE.SOP.ProxySOP.Instance.SiteID);
            arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            strOptionString = string.Empty;

            if (arrResult != null)
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    strOptionString = WebDBManager.GetStringField(arrResult[i]);
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

            //Point pt = new Point(m_toolbar.Location.X + m_toolbar.GetButton(ID.ID_VIEW_SIMULATOR).Location.X, m_toolbar.Location.Y + m_toolbar.Size.Height);

            //// Toolbar가 화면 아래쪽에 있으면 m_PaneBtnHome을 Toolbar 위쪽에 띄운다.
            //if (pt.Y + m_PaneBtnHome.Size.Height > FormFrame.Instance.Location.Y + FormFrame.Instance.Size.Height)
            //    pt = new Point(m_toolbar.Location.X + m_toolbar.GetButton(ID.ID_VIEW_SIMULATOR).Location.X, m_toolbar.Location.Y - m_PaneBtnHome.Size.Height);

            //m_PanelBtnSimulator.Location = pt;
            m_PanelBtnSimulator.Show(this);
        }

        private void RunSimulator(object sender, EventArgs e)
        {
            m_PanelBtnSimulator.Hide();

            Button btn = sender as Button;
            if (btn.Tag == null)
                return;


            string strSQL = String.Format("SELECT PropertyValue FROM OptionSDMS WHERE PropertyName = '{0}' AND SiteID = {1}", btn.Tag, UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            string strOptionString = string.Empty;

            if (arrResult != null)
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    strOptionString = WebDBManager.GetStringField(arrResult[i]);
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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            if (m_proxyMessenger == null)
                return;

            FormMain.Instance.ProxyMessenger.ShowHideSOPSimulator(); 
            /*if (!m_proxyMessenger.IsVisibleSOPSimulator())
                m_proxyMessenger.ShowSOPSimulator();
            else
                m_proxyMessenger.HideSOPSimulator();*/
        }

        private void btnBulletin_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            if (m_proxyMessenger == null)
                return;

            m_proxyMessenger.ToggleSOPBulletin();
            //ToggleSOPBulletin();
        }

        private void btnMissionStatus_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            if (m_proxyMessenger == null)
                return;

            m_proxyMessenger.ShowHideMissionStatus();
            /*if (!m_proxyMessenger.IsVisibleMissionStatus())
                m_proxyMessenger.ShowMissionStatus();
            else
                m_proxyMessenger.HideMissionStatus();*/
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
                m_proxyMessenger.OpenSOP_PSM(equipZone.ID, sopTime,nSensorID, nHistoryID);
            else if (type == ProcessType.SecurityAlarm)
                m_proxyMessenger.OpenSOP_Security(equipZone.ID, sopTime,nSensorID, nHistoryID, (int)process.TargetSensor.Type);
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

            Point pt = new Point(btnHome.Location.X + btnHome.Width, btnHome.Location.Y + panelTop.Height + panelTop2.Height);
            Point ptScr = this.PointToScreen(pt);
            m_PaneBtnHome.Location = ptScr;// PointToClient(ptScr);

            if (m_PaneBtnSaveHome.Visible == false)
            {
                m_PaneBtnSaveHome.Show(this);
                m_PaneBtnSaveHome.Visible = false;
            }

            Point pt2 = new Point(btnSaveHome.Location.X, panelTop2.Location.Y + panelTop2.Height);
            Point ptScr2 = this.PointToScreen(pt2);
            m_PaneBtnSaveHome.Location = ptScr2;// PointToClient(ptScr);

            if (m_PanelBtnNotice.Visible == false)
            {
                m_PanelBtnNotice.Show(this);
                m_PanelBtnNotice.Visible = false;
            }
            Point pt3 = new Point(panelLeft2.Location.X + panelLeft2.Width, panelStatus.Location.Y + panelStatus.Height);
            Point ptScr3 = this.PointToScreen(pt3);
            m_PanelBtnNotice.Location = ptScr3;

            if (m_PanelBtnSimulator.Visible == false)
            {
                m_PanelBtnSimulator.Show(this);
                m_PanelBtnSimulator.Visible = false;
            }

            Point pt4 = new Point(btnSimulator.Location.X - 8, btnFullScreen.Location.Y + btnFullScreen.Height);
            Point ptScr4 = this.PointToScreen(pt4);
            m_PanelBtnSimulator.Location = ptScr4;// PointToClient(ptScr);

            if (m_frmMessageSender != null)
            {
                if (m_frmMessageSender.Visible == false)
                {
                    m_frmMessageSender.Show(this);
                    m_frmMessageSender.Visible = false;
                }
                Point pt5 = new Point(btnSendMessage.Location.X, panelTop2.Location.Y + panelTop2.Height);
                Point ptScr5 = this.PointToScreen(pt5);
                m_frmMessageSender.Location = ptScr5;// PointToClient(ptScr);  

                if (m_frmMessageReceiver.Visible == false)
                {
                    m_frmMessageReceiver.Show(this);
                    m_frmMessageReceiver.Visible = false;
                }
                Point pt6 = new Point(btnSendMessage.Location.X + m_frmMessageSender.Width, panelTop2.Location.Y + panelTop2.Height);
                Point ptScr6 = this.PointToScreen(pt6);
                m_frmMessageReceiver.Location = ptScr6;// PointToClient(ptScr);  
            }

            int horizontal = 10;
            int nTemp = 20;
            int nTemp2 = panelTop2.Location.Y + panelTop2.Height;
            if (m_Resolution == Resolution.FullHD)
            {
                horizontal = (int)(horizontal * 0.5);
                nTemp = (int)(nTemp * 0.5);
            }
            else if (m_Resolution == SDMS.Resolution.Other)
            {
                horizontal = (int)(horizontal * 0.75);
                nTemp = (int)(nTemp * 0.75);
            }
            nTemp2 += nTemp;

            foreach (KeyValuePair<PopupDialog.Report.ReportCategory, PopupDialog.Report.FormReportMenu> pair in m_dicReportMenus)
            {
                if (pair.Value.Visible)
                {
                    Point pt7 = new Point((int)(panelLeft2.Width * 0.5 - pair.Value.btnReport.Width * 0.5) - horizontal, nTemp2);
                    Point ptScr7 = this.PointToScreen(pt7);
                    pair.Value.Location = ptScr7;// PointToClient(ptScr);
                    nTemp2 += pair.Value.Height + nTemp;
                }
            }

            /*if (fireMenu != null)
            {
                if (fireMenu.Visible)
                {
                    //fireMenu.Show(this);
                    //fireMenu.Visible = false;
                    //}
                    Point pt7 = new Point((int)(panelLeft2.Width * 0.5 - fireMenu.btnReport.Width * 0.5) - horizontal, nTemp2);
                    Point ptScr7 = this.PointToScreen(pt7);
                    fireMenu.Location = ptScr7;// PointToClient(ptScr);
                    nTemp2 += fireMenu.Height + nTemp;
                }
            }
            if (psmMenu != null && UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                if (psmMenu.Visible)
                {
                    //psmMenu.Show(this);
                    //psmMenu.Visible = false;
                    //}

                    Point pt8 = new Point((int)(panelLeft2.Width * 0.5 - psmMenu.btnReport.Width * 0.5) - horizontal, nTemp2);
                    Point ptScr8 = this.PointToScreen(pt8);
                    psmMenu.Location = ptScr8;// PointToClient(ptScr);
                    nTemp2 += psmMenu.Height + nTemp;
                }
            }
            if (securityMenu != null && UnE.SOP.ProxySOP.Instance.UseIntrusion)
            {
                if (securityMenu.Visible)
                {
                    //securityMenu.Show(this);
                    //securityMenu.Visible = false;
                    //}

                    Point pt8 = new Point((int)(panelLeft2.Width * 0.5 - securityMenu.btnReport.Width * 0.5) - horizontal, nTemp2);
                    Point ptScr8 = this.PointToScreen(pt8);
                    securityMenu.Location = ptScr8;// PointToClient(ptScr); 
                }
            }*/
        }

        public void SetTitle(string strTitle)
        {
            //labelTitle.Text = strTitle;
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
                string szFileName = szDir + "\\" + "libCCTV2.exe";

                if (File.Exists(szFileName))
                {
                    m_CCTVProcess = StartPocess(szFileName, szDir, args);
                    //m_CCTVProcess = GetProcess("libCCTV2");
                }
                else
                {
                    szDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                    szFileName = szDir + "\\common\\" + "libCCTV2.exe";

                    m_CCTVProcess = StartPocess(szFileName, szDir, args);
                    //m_CCTVProcess = GetProcess("libCCTV2");
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

                m_proxyMessenger.ShowSOPSimulatorIfInvisible();
                /*if (!m_proxyMessenger.IsVisibleSOPSimulator())
                    m_proxyMessenger.ShowSOPSimulator();*/


                m_proxyMessenger.EnableCCTV();
            }
        }


        private void btnDefaultCCTV_Click(object sender, EventArgs e)
        {
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ShowCCTVForm();
        }

        public void ShowToolbar()
        {
            //if (m_readyToReceiveMessage == true)
            //{
            //    //m_toolbar.TopMost = true;
            //    if (!m_toolbar.Visible)
            //    {
            //        //m_toolbar.TopLevel = false;
            //       // m_toolbar.TopMost = true;
            //        //this.PageHome.ContentForm.OutdoorView.ControlPane
            //        //m_toolbar.Parent = this.PageHome.ContentForm.OutdoorView;
            //        //m_toolbar.Owner = this.PageHome.ContentForm.OutdoorView;
            //        m_toolbar.Show(this);
                    
            //    }
            //    m_toolbar.Focus();
            //}

          
        }

        public void HideToolbar()
        {
            //if (m_toolbar.Visible)
            //    m_toolbar.Hide();
        }

        public void OnClickToolbarButton(int nID)
        {
            //if (nID == ID.ID_VIEW_SIMULATOR)
            //    btnSimulator_Click(null, null);
            //else
            //{
            //    Button btn = m_toolbar.GetButton(nID);

            //    if (btn != null)
            //        OnClickToolBarButton(btn, null);
            //    else
            //    {
            //        if(m_dicIDButtons.ContainsKey(nID))
            //        {
            //            btn = m_dicIDButtons[nID];
            //            OnClickToolBarButton(btn, null);
            //        }
            //    }
            //} 
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
            //DockBtnHome();
        }

        private void m_toolbar_VisibleChanged(object sender, EventArgs e)
        {
            //DockBtnHome();
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
            //if (m_toolbar.Visible == false)
            //{
            //    m_PaneBtnHome.Hide();
            //    m_PanelBtnSimulator.Hide();
            //}

            //if (m_PaneBtnHome.Visible == true)
            //{
            //    Point pt = new Point(m_toolbar.Location.X, m_toolbar.Location.Y + m_toolbar.Size.Height);

            //    // Toolbar가 화면 아래쪽에 있으면 m_PaneBtnHome을 Toolbar 위쪽에 띄운다.
            //    if (pt.Y + m_PaneBtnHome.Size.Height > FormFrame.Instance.Location.Y + FormFrame.Instance.Size.Height)
            //        pt = new Point(m_toolbar.Location.X, m_toolbar.Location.Y - m_PaneBtnHome.Size.Height);

            //    m_PaneBtnHome.Location = pt;
            //}

            //if (m_PanelBtnSimulator.Visible == true)
            //{
            //    Point pt = new Point(m_toolbar.Location.X + m_toolbar.GetButton(ID.ID_VIEW_SIMULATOR).Location.X, m_toolbar.Location.Y + m_toolbar.Size.Height);

            //    // Toolbar가 화면 아래쪽에 있으면 m_PaneBtnHome을 Toolbar 위쪽에 띄운다.
            //    if (pt.Y + m_PaneBtnHome.Size.Height > FormFrame.Instance.Location.Y + FormFrame.Instance.Size.Height)
            //        pt = new Point(m_toolbar.Location.X + m_toolbar.GetButton(ID.ID_VIEW_SIMULATOR).Location.X, m_toolbar.Location.Y - m_PaneBtnHome.Size.Height);

            //    m_PanelBtnSimulator.Location = pt;
            //}

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
            if (((System.Windows.Forms.MouseEventArgs)(e)) != null && ((System.Windows.Forms.MouseEventArgs)(e)).Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_manualManager.IsHelpMode)
                return;

            ShowSendMessage();
        }
         
        public void ShowSendMessage()
        { 
            if (m_frmMessageSender == null || m_frmMessageSender.IsDisposed)
            {
                m_frmMessageSender = new PopupDialog.FormMessageSender();
                m_frmMessageSender.VisibleChanged += (s, e) =>
                    { 
                        SetButtonBackColor(btnSendMessage);
                    };
            }

            if (m_frmMessageSender != null && !m_frmMessageSender.Visible)
            {
                m_frmMessageSender.SetChildCtrlResize(m_frmMessageSender, 408, 400); 
            }

            if (m_frmMessageSender.Visible) 
                m_frmMessageSender.Visible = false;  
            else
            {
                //btnSendMessage.ImageNormal = btnSendMessage.ImageClicked;
                Point pt = new Point(btnSendMessage.Location.X, panelTop2.Location.Y + panelTop2.Height);
                Point ptScr = this.PointToScreen(pt);
                
                m_frmMessageSender.Show(this);
                m_frmMessageSender.Location = ptScr;// PointToClient(ptScr); 
            }
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            ClearSelectDlg();
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {

        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)0x312)
            {
                if (m.WParam == (IntPtr)0x0)
                {
                    btnHelp_Click(null, null);
                }
                else if (m.WParam == (IntPtr)0x1)
                {
                    btnShowRemote_Click(null, null);
                }
                else if (m.WParam == (IntPtr)0x2)
                {
                    if (m_frmRemoteCtrl != null && m_frmRemoteCtrl.Visible)
                    {
                        m_frmRemoteCtrl.SetNextTab();
                    }
                }
            }
            else if (m.Msg == libSplash.Message.WM_COPYDATA)
            {
                //libSplash.COPYDATASTRUCT cds = (libSplash.COPYDATASTRUCT)m.GetLParam(typeof(libSplash.COPYDATASTRUCT));

                //if (cds.lpData.ToLower() == "splashhandle")
                //    m_splashManager.SplashHandle = cds.dwData;

                //return;
            }

            base.WndProc(ref m);
        }


        internal void SendSensorCloseMessageToSOPSimulator(int nSensorID, int nSensorZoneHistoryID)
        {
            m_proxyMessenger.SensorClose(nSensorID, nSensorZoneHistoryID);
        }

        internal void SendSensorSameSensorGroupRunningToSOPSimulator(int sleepProcessSensorHistoryID, int activeProcessSensorHistoryID)
        {
            m_proxyMessenger.SameSensorGroupRunning(sleepProcessSensorHistoryID, activeProcessSensorHistoryID);
        }

        public void ToggleWindow(bool visible)
        {
            // ToggleHide 상태가 아닐 경우에는 Visible True에 대하여 아무일도 하지 않는다.
            if (visible && m_toggleHideStatus == false)
                return;

            FormFrame.Instance.Visible = true;
            FormMain.Instance.Visible = true;
            FormFrame.Instance.ShowInTaskbar = visible;

            try
            {
                if (visible == false)
                {
                    m_toggleHideStatus = true;

                    // PopupTranslucentForm 닫기
                    m_PageHome.CloseExternal();
                    m_PageHome.CloseExternal();
                }
                else
                    m_toggleHideStatus = false;

                FormFrame.Instance.Visible = visible;
                if (visible)
                {
                    FormFrame.Instance.WindowState = FormWindowState.Maximized;
                }
            }
            catch (Exception)
            {
            }
        }

        public void ShowEvacCircleInvoke(int nLevel)
        {
            this.Invoke((MethodInvoker)delegate
            {
                m_PageHome.ContentForm.ShowEvacCircle(nLevel);
            });
        }

        // 새로운 센서신호가 탐지되었음을 알린다.
        public void ShowSensorAlarmInvoke(ProcessIF process, ReactionType notifyType)
        {
            if (process == null)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                ISensorTooltipOwner view = null;
                m_PageHome.FireDetect(process.TargetSensor, process.TargetZone, process.SensorHistoryID);

                try
                {
                    if (this.WindowState != FormWindowState.Maximized)
                    {
                        FormFrame.Instance.WindowState = FormWindowState.Maximized;
                        this.Activate();
                        this.Focus();
                    }
                }
                catch (System.Exception)
                {
                }



                SeletCaseData form = new SeletCaseData(process.ProcessType, view, process.TargetSensor, process.SensorHistoryID, process.ShowOpenSOP, process.DetectTime);
                ConfirmDialogManager.Instance.AddDialogFirst(form);

                if (process.LastLog != null && process.LastLog.ReactionType != (int)notifyType)
                    ConfirmDialogManager.Instance.ShowDialogNext(); 

                FormMain.Instance.Update3DView();
            });
        }

        // 기존에 탐지된 센서신호 가운데 특정 센서신호를 현재 화면에 나타나도록 한다.
        public void SelectProcessInvoke(ProcessIF process, bool showDetectSensorTooltipCCTV, ArrayList arrCCTVs, int nSensorZoneID)
        {
            if (process == null)
                return;

            int nSituation = 0;

            if (process.ProcessType == ProcessType.PSMAlarm)
                nSituation = 2;
            else
                nSituation = 1;

            this.Invoke((MethodInvoker)delegate
            {
                this.PageHome.ContentForm.PushViewState(true);
                                
                if (UnE.SOP.ProxySOP.Instance.SiteID == 102) 
                {
                    // (신호가 현재 보여주고 있는 2d view에 있는 신호인지 여부 아니라면 change view 해야함);
                    this.PageHome.ContentForm.IsSameCampus(process.TargetZone.Building.BuildingGroup);
                }
                this.PageHome.ContentForm.HideZoneVolume();

                if (process.ProcessType != ProcessType.PSMAlarm)
                    this.PageHome.ContentForm.HideEvacCircle();

                if (process.TargetZone == null || process.TargetZone.Building == null)
                {
                    //FormMain.Instance.PageHome.ContentForm.LayoutOutside();
                }
                else
                {
                    BuildingGroup grp = process.TargetZone.Building.BuildingGroup;
                    Building building = process.TargetZone.Building;

                    //FormMain.Instance.PageHome.ContentForm.LayoutBothside();
                    //PageBackstageHome.Instance.SetCheckBothSide();
                    if (process.TargetZone.LinkedZone != null)
                        this.SetFloorStatus(grp, building, (process.TargetZone.LinkedZone));

                    if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                    {
                        this.ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);
                    }


                    this.EnableChangeViewBtn();
                }

                if (process.TargetSensor != null && process.TargetSensor.Type == IFacility.FacilityType.ExternalAlarmBell || process.TargetSensor.Type == IFacility.FacilityType.SecomExternalAlarmBell)
                {
                    m_PageHome.ContentForm.ShowEmPoll(nSensorZoneID);
                    m_PageHome.ContentForm.ZoomBuilding("EMPOLL_" + nSensorZoneID);
                }
                else
                {
                    if (process.TargetZone.Building != null && process.TargetZone.Building.BuildingID != "yhNONE")
                    {
                        if (UnE.SOP.ProxySOP.Instance.SiteID == 999 || UnE.SOP.ProxySOP.Instance.SiteID == 102)
                        {
                            if (process.TargetZone.LinkedZone != null)
                                this.PageHome.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, false, true); 
                        }
                        else
                        {
                            string szName = process.TargetZone.Building.BuildingID;

                            this.PageHome.ContentForm.ZoomBuilding(szName);

                            if (process.TargetZone.LinkedZone != null)
                            {
                                this.PageHome.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.LinkedZone.ID, true, true);
                                this.PageHome.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, false, true);
                            }
                        }
                    }
                    else
                    {
                        if (process.TargetZone.Polygon != null)
                        {
                            UnE.Geometry.Vertex2D pos = process.TargetZone.Polygon.CalcWeightCenter();
                            float dx = ZoneManager.Instance.Dx;
                            float dy = ZoneManager.Instance.Dy;

                            float x = (float)pos.x - dx;
                            float y = 1.0f;
                            float z = dy - (float)pos.y;
                            x /= 1000;
                            z /= 1000;

                            if (process.TargetZone.LinkedZone != null)
                            {
                                this.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                                this.PageHome.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, true, true);
                            }
                        }
                    }
                }

                if (arrCCTVs != null)
                {
                    foreach (CCTV cctv in arrCCTVs)
                    {
                        if (process.TargetZone.LinkedZone != null)
                        {
                            if (cctv.POI.Zone == process.TargetZone.LinkedZone && process.TargetZone.IsOutdoor == false)
                            {
                                if (cctv.POI.ViewType == 1)
                                {
                                    UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                    if (view != null)
                                    {
                                        System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                                        if (cctv.POI.Popup != null)
                                        {
                                            if (showDetectSensorTooltipCCTV)
                                                cctv.POI.Popup.Show(p.X, p.Y);
                                        }
                                    }

                                }
                                else
                                {
                                    UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                    if (view != null)
                                    {
                                        System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                                        if (cctv.POI.Popup != null)
                                        {
                                            if (showDetectSensorTooltipCCTV)
                                                cctv.POI.Popup.Show(p.X, p.Y);
                                        }
                                    }

                                }
                            }
                            else
                            {
                                if (cctv.POI.IsIndoor == false)
                                {
                                    UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                    if (view != null)
                                    {
                                        System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                                        if (cctv.POI.Popup != null)
                                        {
                                            if (showDetectSensorTooltipCCTV)
                                                cctv.POI.Popup.Show(p.X, p.Y);
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    if (process.TargetZone.LinkedZone != null)
                    {
                        if (process.TargetZone != null && process.TargetZone.LinkedZone != null)
                        {
                            //FormMain.Instance.PageHome.ShowSituationCCTV(true);
                            this.PageHome.ShowBigCCTV(process.TargetZone.LinkedZone, nSituation, true);
                            this.SelectCCTVTab(false);
                        } 
                    }
                }

                if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                {
                    if (process.TargetZone != null && process.TargetZone.LinkedZone != null)
                    {
                        this.CCTVPipe.Send("SetHistoryID(" + process.SensorHistoryID + ")");
                        //FormMain.Instance.PageHome.ShowSituationCCTV(true);
                        this.PageHome.ShowBigCCTV(process.TargetZone.LinkedZone, nSituation, true);
                        this.SelectCCTVTab(false);
                    } 
                }

                this.Update3DView();
            });
        }

        // 탐지된 센서신호를 실제 재난상황으로 판단한다.
        // Return 값 : CCTV List
        public ArrayList ConfirmDisasterInvoke(ProcessIF process, bool showDetectSensorTooltipCCTV, int nSensorZoneID, ReactionType notifyType, int nAlarmLevel)
        {
            ArrayList arrCCTVs = null;

            if (process == null)
                return arrCCTVs;

            int nSituation = 0;

            if (process.ProcessType == ProcessType.PSMAlarm)
                nSituation = 2;
            else
                nSituation = 1;

            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.PageHome.ContentForm.PushViewState(true);


                    ProcessManager.PlaySound();

                    this.PageHome.ContentForm.HideZoneVolume();


                    //if (!FormMain.Instance.ShowEquipZoneCCTV)
                    {
                        this.SelectMonitoringTab();
                    }

                    this.DetectFireSensor = true;

                    if (process.TargetZone.Building == null)
                    {
                        //FormMain.Instance.PageHome.ContentForm.LayoutOutside();
                    }
                    else
                    {
                        BuildingGroup grp = process.TargetZone.Building.BuildingGroup;
                        Building building = process.TargetZone.Building;

                        //PageBackstageHome.Instance.ContentForm.LayoutBothside();
                        //PageBackstageHome.Instance.SetCheckBothSide();
                        if (process.TargetZone.LinkedZone != null)
                            this.SetFloorStatus(grp, building, process.TargetZone.LinkedZone);

                        if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                        {
                            this.ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);
                        }

                        this.EnableChangeViewBtn();
                    }

                    this.PageHome.HideAllPOIPopup();

                    arrCCTVs = CCTVManager.Instance.AutoPopupCCTV(process.TargetZone.LinkedZone);

                    foreach (CCTV cctv in arrCCTVs)
                    {
                        if (cctv.POI.Zone == process.TargetZone.LinkedZone && process.TargetZone.IsOutdoor == false)
                        {
                            if (cctv.POI != null && cctv.POI.Popup != null)
                            {
                                UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);

                                if (showDetectSensorTooltipCCTV)
                                    cctv.POI.Popup.Show(p.X, p.Y);
                            }
                        }
                        else
                        {
                            if (cctv.POI.IsIndoor == false)
                            {
                                UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                if (view != null && cctv.POI != null)
                                {
                                    System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                                    if (cctv.POI.Popup != null)
                                    {
                                        if (showDetectSensorTooltipCCTV)
                                            cctv.POI.Popup.Show(p.X, p.Y);
                                    }
                                }
                            }
                        }
                    }

                    if (process.TargetZone.Building != null && process.TargetZone.Building.BuildingID != "yhNONE")
                    {
                        if (process.TargetSensor != null && process.TargetSensor.Type == IFacility.FacilityType.ExternalAlarmBell || process.TargetSensor.Type == IFacility.FacilityType.SecomExternalAlarmBell)
                        {
                            this.PageHome.ContentForm.ShowEmPoll(nSensorZoneID);
                            this.PageHome.ContentForm.ZoomBuilding("EMPOLL_" + nSensorZoneID);
                        }
                        else
                        {
                            string szName = process.TargetZone.Building.BuildingID;

                            this.PageHome.ContentForm.ZoomBuilding(szName);

                            if (process.TargetZone.LinkedZone != null)
                            {
                                string strVolume, strSceneName;

                                if (m_useEquipZoneVolume && m_dicEquiZoneVolume.TryGetValue(process.TargetZone.ID, out strVolume) && m_dicZoneScene.TryGetValue(process.TargetZone.LinkedZone.ID, out strSceneName))
                                {
                                    SelectScene(strSceneName, process.TargetZone.LinkedZone, strVolume, true);
                                    /*this.PageHome.ContentForm.HideAllAlarmZones();
                                    this.PageHome.ContentForm.SelectScene(strSceneName);
                                    this.PageHome.ContentForm.ShowAlarmZone(strVolume, true);*/
                                }
                                else
                                {
                                    this.PageHome.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.LinkedZone.ID, true, true);
                                    this.PageHome.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, false, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (process.TargetSensor != null && process.TargetSensor.Type == IFacility.FacilityType.ExternalAlarmBell || process.TargetSensor.Type == IFacility.FacilityType.SecomExternalAlarmBell)
                        {
                            this.PageHome.ContentForm.ShowEmPoll(nSensorZoneID);
                            this.PageHome.ContentForm.ZoomBuilding("EMPOLL_" + nSensorZoneID);
                        }
                        else if (process.TargetZone.Polygon != null)
                        {
                            UnE.Geometry.Vertex2D pos = process.TargetZone.Polygon.CalcWeightCenter();
                            float dx = ZoneManager.Instance.Dx;
                            float dy = ZoneManager.Instance.Dy;


                            if (UnE.SOP.ProxySOP.Instance.SiteID == 2)
                            {
                                float x = (float)pos.x - dx;
                                float y = 0.0f;
                                float z = dy - (float)pos.y;

                                x /= 1000;
                                z /= 1000;

                                if (process.TargetZone.LinkedZone != null)
                                {
                                    this.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                                    this.PageHome.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, true, true);
                                }
                            }
                            else if (UnE.SOP.ProxySOP.Instance.SiteID == 100)
                            {
                                float x = (float)pos.x;
                                float y = 2.0f;
                                float z = (float)pos.y;

                                if (process.TargetZone.LinkedZone != null)
                                {
                                    this.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                                    this.PageHome.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, true, true);
                                }
                            }
                            else
                            {
                                float x = (float)pos.x - dx;
                                float y = 0.0f;
                                float z = dy - (float)pos.y;

                                if (process.TargetZone.LinkedZone != null)
                                {
                                    this.PageHome.ContentForm.ZoomTarget(x, y, z, false);
                                    this.PageHome.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, true, true);
                                }
                            }
                        }
                    }
                });

                DialogResult result = DialogResult.Cancel;

                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        ISensorTooltipOwner view = null;

                        if (process.TargetSensor.POI == null)
                        {
                            //view = PageBackstageHome.Instance.ContentForm.IndoorView;
                        }
                        else
                        {
                            if (process.TargetSensor.POI.IsIndoor == true)
                            {
                                //view = PageBackstageHome.Instance.ContentForm.IndoorView;
                            }
                            else
                            {
                                //view = PageBackstageHome.Instance.ContentForm.OutdoorView;
                            }
                        }

                        PageBackstageHome.Instance.FireDetect(process.TargetSensor, process.TargetZone, process.SensorHistoryID);

                        try
                        {
                            if (this.MainFrame.WindowState != FormWindowState.Maximized)
                            {
                                this.MainFrame.WindowState = FormWindowState.Maximized;
                                this.MainFrame.Activate();
                                this.Focus();
                            }
                            /*if (this.WindowState != FormWindowState.Maximized)
                            {
                                this.WindowState = FormWindowState.Maximized;
                                this.Activate();
                                this.Focus();
                            }*/
                        }
                        catch (System.Exception)
                        {
                        }

                        SeletCaseData form = new SeletCaseData(process.ProcessType, view, process.TargetSensor, process.SensorHistoryID, process.ShowOpenSOP, process.DetectTime);
                        ConfirmDialogManager.Instance.AddDialogFirst(form);

                        if (process.ProcessType == ProcessType.PSMAlarm && process.TargetZone != null)
                        {
                            this.PageHome.ContentForm.SetEvacDistance(nSensorZoneID);
                            this.PageHome.ContentForm.SetEvacCenter(process.TargetZone);
                            this.PageHome.ContentForm.ShowEvacCircle(nAlarmLevel);
                        }

                        PageBackstageHome.Instance.ShowBigCCTV(process.TargetZone, nSituation);
                        
                        PageBackstageHome.Instance.SetTargetCCTVPreset(process.TargetSensor.EquipZoneID);

                        if (process.LastLog != null && process.LastLog.ReactionType != (int)notifyType)
                            ConfirmDialogManager.Instance.ShowDialogNext();


                        this.Update3DView();
                    });
                }
                catch (System.Threading.ThreadInterruptedException e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                if (result == DialogResult.Cancel)
                {
                    // 데이터 갱신을 한번 기다린후에 제거 한다.
                    //Thread.Sleep(1500);
                    //ProcessManager.Instance.EndProcess(this);
                }
            }
            catch (Exception)
            {
            }

            return arrCCTVs;
        }

        public void EndNotifyProcessInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.EndNotifyProcess(log);
            });
        }

        public void SetPSMDetectModeInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetPSMDetectMode(log);
            });
        }

        public void SetNormalModeInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetNormalMode(log);
            });
        }

        public void SetFireDetectModeInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                // 화재 탐지 모드
                this.SetFireDetectMode(log);
            });
        }

        public void SetSecurityDetectModeInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                // 방범신호 탐지 모드
                this.SetSecurityDetectMode(log);
            });
        }

        public void SetEarthquakeDetectModeInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                // 지진신호 탐지 모드
                this.SetEarthquakeDetectMode(log);
            });
        }

        public void NotifyProcessInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                if (nSensorID != -1)
                {
                    // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                    ProcessIF process = (ProcessIF)ProcessManager.Instance.GetProcess(nSensorID);

                    if (process != null && process.TargetSensor != null)
                        this.BeginNotifyProcess(log, process.TargetSensor);
                    else
                        this.BeginNotifyProcess(log, null);

                    if (process != null)
                        process.Select();

                    this.SendDetectMessageToSOPSimulator();
                }
            });
        }

        /*public void BeginNotifyProcessInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.BeginNotifyProcess(log);
            });
        }*/

        public void RunSOPInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetRunSOPMode(log);
                int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                if (nSensorID != -1)
                {
                    // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                    ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
                    if (process != null)
                        process.Select();
                }
            });
        }

        public void RunNCancelSOPInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetRunNCancelSOPMode(log);
                int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                if (nSensorID != -1)
                {
                    // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                    ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
                    if (process != null)
                        process.Select();
                }
            });
        }

        public void FinishSOPInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetFinishSOPMode(log);

                int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                if (nSensorID != -1)
                {
                    // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                    ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
                    if (process != null)
                        process.Select();
                }
            });
        }

        public void IgnoreSOPInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetIgnoreSOPMode(log);
                int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                if (nSensorID != -1)
                {
                    // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                    ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
                    if (process != null)
                        process.Select();
                }
            });
        }

        public void AddLogMessageInvoke(ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.AddLogMessage(log);
            });
        }

        private void SetMainBackVButtons()
        {

        }

        private Dictionary<Control, Size> m_CtrlSize = new Dictionary<Control, Size>();
        private Dictionary<Control, Font> m_CtrlFontSize = new Dictionary<Control, Font>();

        /// <summary>
        /// 컨트롤 기준 사이즈 정의
        /// </summary>
        private void InitCtrlSize()
        {
            m_CtrlSize.Add(panelTop3DTabItemCtrl, new Size(3840, 67));
            m_CtrlSize.Add(pnActionPSM, new Size(3840, 67));
            m_CtrlSize.Add(pnSMSPSM, new Size(3840, 67));
            m_CtrlSize.Add(pnDetectPSM, new Size(3840, 67));
            m_CtrlSize.Add(panelReactionHistory, new Size(3840, 67));
            m_CtrlSize.Add(panelProcessHistory, new Size(3840, 67));
            m_CtrlSize.Add(pnNotOperationPSM, new Size(3840, 67));
            m_CtrlSize.Add(btnMin, new Size(44, 44));
            m_CtrlSize.Add(btnMax, new Size(44, 44));
            m_CtrlSize.Add(btnClose, new Size(44, 44));
            //m_CtrlSize.Add(panelLeft2, new Size(172, 1970));

            m_CtrlSize.Add(cmbFireDetect, new Size(1000, 38));
            m_CtrlFontSize.Add(cmbFireDetect, new System.Drawing.Font(Program.prgFont, 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(labelSensorMonitor, new System.Drawing.Font(Program.prgFont, 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(labelFireDetect, new System.Drawing.Font(Program.prgFont, 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(mLabelStatus, new System.Drawing.Font(Program.prgFont, 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(btnFire, new System.Drawing.Font(Program.prgFont, 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(mLabelLog, new System.Drawing.Font(Program.prgFont, 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(mLabelZone, new System.Drawing.Font(Program.prgFont, 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            
            m_CtrlFontSize.Add(proc_lblSelectZone, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(lblDetectPSMSplitUnit, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(lblSplitUnit, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(labelDetectPSMDateFormat, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(labelDetectDateFormat, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(lblFireSelect, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(label1, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(label14, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(lblSMSPSMSelectZone, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(lblDetectPSMSelectZone, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(lblNotOperationPSMSelectZone, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)))); 
            m_CtrlFontSize.Add(lblDetectPSMViewCount, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(lblViewCount, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
             
            m_CtrlSize.Add(cboDetectPSMViewCount, new Size(180, 55));
            m_CtrlFontSize.Add(cboDetectPSMViewCount, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(proc_cboViewCount, new Size(180, 55));
            m_CtrlFontSize.Add(proc_cboViewCount, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(react_cboSearchTypeIntrusion, new Size(700, 55));
            m_CtrlFontSize.Add(react_cboSearchTypeIntrusion, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(react_cboSearchTypeFire, new Size(700, 55));
            m_CtrlFontSize.Add(react_cboSearchTypeFire, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(react_cboSearchTypeEarthquake, new Size(700, 55));
            m_CtrlFontSize.Add(react_cboSearchTypeEarthquake, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(react_cboSearchTypeTH, new Size(700, 55));
            m_CtrlFontSize.Add(react_cboSearchTypeTH, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboActionPSMStartTime, new Size(280, 55));
            m_CtrlFontSize.Add(cboActionPSMStartTime, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboActionPSMEndTime, new Size(280, 55));
            m_CtrlFontSize.Add(cboActionPSMEndTime, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboSMSPSMLatelyDate, new Size(280, 55));
            m_CtrlFontSize.Add(cboSMSPSMLatelyDate, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboDetectPSMLatelyDate, new Size(280, 55));
            m_CtrlFontSize.Add(cboDetectPSMLatelyDate, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(react_cboStartTime, new Size(280, 38));
            m_CtrlFontSize.Add(react_cboStartTime, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(react_cboEndTime, new Size(280, 55));
            m_CtrlFontSize.Add(react_cboEndTime, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(proc_cboLatelyDate, new Size(280, 55));
            m_CtrlFontSize.Add(proc_cboLatelyDate, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboNotOperationPSMLatelyDate, new Size(280, 55));
            m_CtrlFontSize.Add(cboNotOperationPSMLatelyDate, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(proc_cboBuildingGroup, new Size(350, 55));
            m_CtrlFontSize.Add(proc_cboBuildingGroup, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));

            m_CtrlSize.Add(proc_cboFloor, new Size(200, 55));
            m_CtrlFontSize.Add(proc_cboFloor, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(proc_cboBuilding, new Size(500, 55));
            m_CtrlFontSize.Add(proc_cboBuilding, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboSMSPSMBuilding, new Size(620, 55));
            m_CtrlFontSize.Add(cboSMSPSMBuilding, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboDetectPSMBuilding, new Size(350, 55));
            m_CtrlFontSize.Add(cboDetectPSMBuilding, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboNotOperationPSMBuilding, new Size(350, 55));
            m_CtrlFontSize.Add(cboNotOperationPSMBuilding, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)))); 
            m_CtrlSize.Add(cboActionPSMSearchType, new Size(500, 55));
            m_CtrlFontSize.Add(cboActionPSMSearchType, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboActionPSMSelect, new Size(1000, 55));
            m_CtrlFontSize.Add(cboActionPSMSelect, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboActionIntrusionSelect, new Size(1000, 55));
            m_CtrlFontSize.Add(cboActionIntrusionSelect, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboActionFireSelect, new Size(1000, 55));
            m_CtrlFontSize.Add(cboActionFireSelect, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboActionTHSelect, new Size(1000, 55));
            m_CtrlFontSize.Add(cboActionTHSelect, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboActionEarthquakeSelect, new Size(1000, 55));
            m_CtrlFontSize.Add(cboActionEarthquakeSelect, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(cboDetectPSMSplitUnit, new Size(150, 55));
            m_CtrlFontSize.Add(cboDetectPSMSplitUnit, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlSize.Add(proc_cboSplitUnit, new Size(150, 55));
            m_CtrlFontSize.Add(proc_cboSplitUnit, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)))); 
            m_CtrlFontSize.Add(nudSplitUnitDetail, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
            m_CtrlFontSize.Add(nudDetectPSMSplitUnitDetail, new System.Drawing.Font(Program.prgFont, 26F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
       
            m_CtrlFontSize.Add(lblFireText, new System.Drawing.Font(Program.prgFont, 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129))));
        } 

        public void CustomizeGridView(DataGridView gridview)
        {
            gridview.RowHeadersVisible = false;
            gridview.EnableHeadersVisualStyles = false;

            gridview.GridColor = Color.FromArgb(237, 237, 237);
            gridview.BorderStyle = BorderStyle.Fixed3D;
            gridview.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            gridview.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            gridview.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            //if (autoFont)            
                //gridview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;            
            gridview.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells; 

            float sizePer = 1f;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
            {
                sizePer = 0.5f;
            }
            else if (FormMain.Instance.Resolution == Resolution.Other)
            {
                sizePer = 0.75f;
            }

            System.Windows.Forms.DataGridViewCellStyle columnCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            columnCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            columnCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));             
            columnCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
            columnCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            columnCellStyle.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            columnCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            gridview.ColumnHeadersDefaultCellStyle = columnCellStyle;
            gridview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            System.Windows.Forms.DataGridViewCellStyle rowCellStyle = new System.Windows.Forms.DataGridViewCellStyle(); 
            rowCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            rowCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            gridview.RowsDefaultCellStyle = rowCellStyle;

            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            gridview.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;

            gridview.Font = new System.Drawing.Font(Program.prgFont, 18F * sizePer, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }

        [DllImport("user32")]
        private static extern bool SetForegroundWindow(IntPtr handle);
        [DllImport("User32")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
            
        public void ShowHelpManual()
        {
            string strFilePath = Help.ManualManager.GetHelpViewerPath();

            if (strFilePath == null)
                return;

            Process[] proc = Process.GetProcessesByName("HelpViewer");
            if (proc != null && proc.Length > 0)
            {
                // 이미 실행중인 경우 맨 앞으로 띄우기
                SetForegroundWindow(proc[0].MainWindowHandle);
                ShowWindow(proc[0].MainWindowHandle, 3/*maximize*/);
                return;
            }

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();

            info.CreateNoWindow = true;
            info.FileName = strFilePath;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            ShowHelpManual();
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear(); 
            m_manualManager.SetID(btnPick, "Toolbar_Arrow");
            m_manualManager.SetID(btnPanning, "Toolbar_Panning");
            m_manualManager.SetID(btnOrbit, "Toolbar_Orbit");
            m_manualManager.SetID(btnZoomIn, "Toolbar_ZoomIn");
            m_manualManager.SetID(btnZoomOut, "Toolbar_ZoomOut");
            if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
                m_manualManager.SetID(btnCampus, "Toolbar_Campus");
            m_manualManager.SetID(btnMultiCCTV, "Toolbar_CCTV");
            m_manualManager.SetID(btnSimulator, "SensorTester");
            m_manualManager.SetID(btnScreenShot, "Toolbar_ScreenCapture");
            m_manualManager.SetID(btnPSMStatus, "PSMList");
            m_manualManager.SetID(btnDisasterPrevention, "");
            m_manualManager.SetID(btnFullScreen, "Toolbar_FullScreen");
            m_manualManager.SetID(btnHome, "Toolbar_HomeButton");
            m_manualManager.SetID(btnLayerCCTV, "SDMS_3D_Layer");
            m_manualManager.SetID(btnLayerBuildingText, "SDMS_3D_Layer");
            m_manualManager.SetID(btnSaveHome, "Layer_화면설정");
            m_manualManager.SetID(btnSendMessage, "SDMS_Messenger");
            m_manualManager.SetID(btnSensorMonitor, "센서수신반");
            m_manualManager.SetID(labelSensorMonitor, "센서수신반");
            //m_manualManager.SetID(cmbFireDetect, "SDMS_SelectDisaster");
            m_manualManager.SetID(labelFireDetect, "SDMS_SelectDisaster");            
            m_manualManager.SetID(btnSDMS, "SDMS");
            m_manualManager.SetID(btnSOP, "SOPSimulator");
            m_manualManager.SetID(btnBulletin, "");
            m_manualManager.SetID(btnMissionStatus, "");
            m_manualManager.SetID(btnDefaultCCTV, "");
            m_manualManager.SetID(btnShowList, "SDMS_Show_FacilityList");
            m_manualManager.SetID(btnManageManager, "SDMS_EditManager");
            m_manualManager.SetID(btnManageSMS, "SDMS_Manage_SMS");
            m_manualManager.SetID(btnManageBroadcast, "SDMS_Manage_Broadcast");
            m_manualManager.SetID(btnManageDetect, "SDMS_Manage_SensorDetection");
            m_manualManager.SetID(btnSave, "SDMS_Admin_Manager_Edit_Save");
            m_manualManager.SetID(btnSensorMgr, "SDMS_Show_SensorMgrList");
            m_manualManager.SetID(btnEarthquake, "");
            m_manualManager.SetID(btn3DTab, "SDMS_3D");
            m_manualManager.SetID(btnAdminTab, "SDMS_Admin");
            m_manualManager.SetID(btnReportTab, "SDMS_Report");
            m_manualManager.SetID(panelStatus, "재난상황정보");
            m_manualManager.SetID(panelLog, "재난상황정보");
            m_manualManager.SetID(btnFire, "재난상황정보");

            // 리포트 panel
            m_manualManager.SetID(pnActionPSM, "SDMS_Report_Select_Date");
            m_manualManager.SetID(pnSMSPSM, "SDMS_Report_Select_Date");
            m_manualManager.SetID(pnDetectPSM, "SDMS_Report_Select_Date");
            m_manualManager.SetID(panelReactionHistory, "SDMS_Report_Select_Date");
            m_manualManager.SetID(panelProcessHistory, "SDMS_Report_Select_Date");
            m_manualManager.SetID(pnNotOperationPSM, "SDMS_Report_Select_Date");

            // 화재, 방범 위치선택
            m_manualManager.SetID(proc_lblSelectZone, "SDMS_Report_Select_Zone");

            // 누출 시설선택
            m_manualManager.SetID(lblSMSPSMSelectZone, "SDMS_Report_Select_Tank");
            m_manualManager.SetID(lblDetectPSMSelectZone, "SDMS_Report_Select_Tank");
            m_manualManager.SetID(lblNotOperationPSMSelectZone, "SDMS_Report_Select_Tank");

            // 단위, 날짜형식, 최대표기
            m_manualManager.SetID(lblDetectPSMSplitUnit, "SDMS_Report_Select_Unit");
            m_manualManager.SetID(labelDetectPSMDateFormat, "SDMS_Report_Select_Unit");
            m_manualManager.SetID(lblDetectPSMSplitUnitDetail, "SDMS_Report_Select_Unit");
            m_manualManager.SetID(lblDetectPSMViewCount, "SDMS_Report_Select_Unit");
            m_manualManager.SetID(lblSplitUnit, "SDMS_Report_Select_Unit");
            m_manualManager.SetID(labelDetectDateFormat, "SDMS_Report_Select_Unit");
            m_manualManager.SetID(lblSplitUnitDetail, "SDMS_Report_Select_Unit");
            m_manualManager.SetID(lblViewCount, "SDMS_Report_Select_Unit");

            // 시작일, 끝일
            m_manualManager.SetID(btnActionPSMStartDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(btnActionPSMEndDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(btnSMSPSMStartDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(btnSMSPSMEndDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(btnDetectPSMStartDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(btnDetectPSMEndDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(react_btnStartDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(react_btnEndDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(proc_btnStartDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(proc_btnEndDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(btnNotOperationPSMStartDate, "SDMS_Report_Select_Date");
            m_manualManager.SetID(btnNotOperationPSMEndDate, "SDMS_Report_Select_Date");

            //m_manualManager.SetID(cboSMSPSMLatelyDate, "SDMS_Report_Select_Date");
            //m_manualManager.SetID(cboDetectPSMLatelyDate, "SDMS_Report_Select_Date");
            //m_manualManager.SetID(react_cboStartTime, "SDMS_Report_Select_Date");
            //m_manualManager.SetID(react_cboEndTime, "SDMS_Report_Select_Date");
            //m_manualManager.SetID(proc_cboLatelyDate, "SDMS_Report_Select_Date");
            //m_manualManager.SetID(cboNotOperationPSMLatelyDate, "SDMS_Report_Select_Date");

            m_manualManager.ProcessEvent(); 
        }
        public bool IsHelpMode = false;
        public void btnTargetHelp_Click(object sender, EventArgs e)
        {
            if (IsHelpMode)
            {
                btnTargetHelp.ImageNormal = global::SDMS.Properties.Resources.BtnQuestionMark_Default;
                btnTargetHelp.ImageClicked = global::SDMS.Properties.Resources.BtnQuestionMark_Click;
                btnTargetHelp.ImageMouseOver = global::SDMS.Properties.Resources.BtnQuestionMark_Click;
                btnTargetHelp.Refresh();

                //this.Cursor = Cursors.Default;
                IsHelpMode = false;
            }
            else
            {
                btnTargetHelp.ImageNormal = global::SDMS.Properties.Resources.BtnQuestionMark_Click;
                btnTargetHelp.ImageClicked = global::SDMS.Properties.Resources.BtnQuestionMark_Default;
                btnTargetHelp.ImageMouseOver = global::SDMS.Properties.Resources.BtnQuestionMark_Default;
                btnTargetHelp.Refresh();

                //this.Cursor = Cursors.Help;
                IsHelpMode = true;
            }
        }

        private void panelLeft2_MouseDown(object sender, MouseEventArgs e)
        {
            CloseOtherReportMenu(PopupDialog.Report.ReportCategory.NONE);
        }

        private FormRemoteControl m_frmRemoteCtrl = null;
        private void btnShowRemote_Click(object sender, EventArgs e)
        {
            bool first = false;
            if (m_frmRemoteCtrl == null || m_frmRemoteCtrl.IsDisposed)
            {
                m_frmRemoteCtrl = new FormRemoteControl();
                first = true;
                m_frmRemoteCtrl.FormClosed += (cs, ce) =>
                    {
                        btnShowRemote.BackColor = Color.Transparent;
                    };
            }

            //if (m_frmRemoteCtrl != null && !m_frmRemoteCtrl.Visible)
            //{
            //    m_frmMessageSender.SetChildCtrlResize(m_frmMessageSender, 408, 400);
            //}

            if (m_frmRemoteCtrl.Visible)
            {
                m_frmRemoteCtrl.Visible = false;                
                btnShowRemote.BackColor = Color.Transparent;
            }
            else
            {
                btnShowRemote.BackColor = m_OrangeColor;
                m_frmRemoteCtrl.TopLevel = true;
                m_frmRemoteCtrl.TopMost = true;
                m_frmRemoteCtrl.SetTab();
                m_frmRemoteCtrl.Show(this);

                if (first)
                {
                    Point pt = new Point(panelLeft2.Location.X + panelLeft2.Width, panelTop2.Location.Y);
                    Point ptScr = this.PointToScreen(pt);
                    m_frmRemoteCtrl.Location = ptScr;// PointToClient(ptScr); 
                }
            }

            SetButtonBackColor(btnShowRemote);
        }
    } 
}