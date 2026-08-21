using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using SDMS_Building.Data;
using SDMS_Building.PopupDialog;
using SDMS_Building.PopupDialog.Config;
using SDMS_Building.PopupDialog.Controls;
using UnE.GUI;
using UnE.Spatial;

namespace SDMS_Building
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Aga.Controls.Tree;
    using Content;
    using libSensorProcess;
    using SDMS;
    using SDMS_Building.Edit;
    using SDMS_Building.History;
    using SDMS_Building.Network;
    using SDMS_Building.Report;
    using SDMS_Building.Report.ReportPopup;
    using SDMS_Building.Utility;
    using UnE.PSM;
    using UnE.Sensor;
    using UnE.Util.Unity;
    using UnE.View.Content;

    public enum LoginUserType { General = 0, IndividualManager/*개별 관리자*/, IntegratedManager/*통합관리자*/ }
    public partial class FormMain : Form, IProcessOwner
    {
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern System.IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRect")]
        public static extern System.IntPtr CreateRoundRect(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private LoginUserType m_userType = LoginUserType.General;
        public LoginUserType UserType
        {
            get { return m_userType; }
            set { m_userType = value; }
        }

        private ContentOwnerTab m_CurrentTab = ContentOwnerTab.M3D_TAB;
        public ContentOwnerTab CurrentTab
        {
            get { return m_CurrentTab; }
        }

        private ContentManager m_contentManager = null;
        public ContentManager ContentManager
        {
            get { return m_contentManager; }
            set { m_contentManager = value; }
        }

        private WebDBManager m_dbMgr = null;
        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        private OptionManager m_optionMgr = null;
        public OptionManager OptionMgr
        {
            get { return m_optionMgr; }
        }

        //private UEWpfControl.WpfBlueComboBox m_cbBuilding = null;
        //private UEWpfControl.WpfBlueComboBox m_cbFloor = null;

        private uPoiVisible m_ufrmPoiVisible = null;

        private uBroadcast m_ufrmBroadcast = null;

        private bool m_enableOutdoor = true;
        private bool m_systemInput = false;

        private DataGridViewCell m_editCell = null;
        private string m_strOriginText = "";

        private bool m_bExit = false;
        public bool Exit
        {
            get { return m_bExit; }
        }

        private List<ProcessIF> m_SensorDectects = new List<ProcessIF>();
        public List<ProcessIF> SensorDectects
        {
            get { return m_SensorDectects; }
            set { m_SensorDectects = value; }
        }

        public ProcessIF CurrentSensorDetectProcess
        {
            get
            {
                return null;
            }
        }

        public ProcessIF LastSensorDetectProcess
        {
            get
            {
                return null;
            }
        }

        private DataManager m_dataMgr = null;
        public DataManager DataManager
        {
            get { return m_dataMgr; }
        }

        private NetworkWebManager m_netMgr = null;
        public NetworkWebManager NetworkWebManager
        {
            get { return m_netMgr; }
        }

        private int m_nSOPGenUserID = -1;
        public int nSOPGentUserID
        {
            get { return m_nSOPGenUserID; }
        }

        private uFormReport m_ufrmReport = null;        
        private uFormEdit2 m_uFormEdit = null;
        public uFormEdit2 uFrmEdit
        {
            get { return m_uFormEdit; }
            set { m_uFormEdit = value; }
        }

        private FormManagement m_frmManagement = null;
        private FormManualReport m_frmManualReport = null;
        private PopupDetailLog m_popDetailLog = null;
        public PopupDetailLog PopDetailLog
        {
            get { return m_popDetailLog; }
            set { m_popDetailLog = value; }
        }

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

        public static void SetDoubleBuffer(ChartDirector.WinChartViewer chart, bool bEnabled)
        {
            Type dgvType1 = chart.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(chart, bEnabled, null);
        }

        public static void SetDoubleBuffer(Label label, bool bEnabled)
        {
            Type dgvType1 = label.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(label, bEnabled, null);
        }

        public static void SetDoubleBuffer(ImageButton button, bool bEnabled)
        {
            Type dgvType1 = button.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(button, bEnabled, null);
        }

        private Dictionary<int, int> m_dicAlarmStep = new Dictionary<int, int>();
        public Dictionary<int, int> DicAlarmStep
        {
            get { return m_dicAlarmStep; }
            set { m_dicAlarmStep = value; }
        }

        private Image m_imgAlarmLevel1 = SDMS_Building.Properties.Resources.alarmLevel1_normal;
        private Image m_imgAlarmLevel2 = SDMS_Building.Properties.Resources.alarmLevel2_normal;
        private Image m_imgAlarmLevel3 = SDMS_Building.Properties.Resources.alarmLevel3_normal;
        private Image m_imgAlarmLevel4 = SDMS_Building.Properties.Resources.alarmLevel4_normal;

        private int m_nBuildingInCharge = -1;
        /// <summary>
        /// 로그인 사용자가 담당하는 빌딩, -1이면 총괄
        /// </summary>
        public int nBuildingInCharge
        {
            get { return m_nBuildingInCharge; }
            set { m_nBuildingInCharge = value; }
        }

        private bool m_bGoOutside = true;
        public bool bGoOutside
        {
            get { return m_bGoOutside; }
            set { m_bGoOutside = value; }
        }

        private Timer m_timerCloseDoor = null;

        public FormMain(int nSOPGenUserID)
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            this.DoubleBuffered = true;

            SetDoubleBuffer(pnTop, true);
            SetDoubleBuffer(pnRight, true);
            SetDoubleBuffer(pnLeft, true);

            btnTabReport.Enabled = false;
            btnTabEdit.Enabled = false;

            SetButtonID(btnTabMonitoring, ID.ID_TAB_3D, "모니터링");
            SetButtonID(btnTabReport, ID.ID_TAB_REPORT, "리포팅");
            SetButtonID(btnTabEdit, ID.ID_TAB_EDIT, "편집");

            btnOutdoor.Font = new System.Drawing.Font("나눔바른고딕", 12.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btnMalfunction.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            m_instance = this;
            m_nSOPGenUserID = nSOPGenUserID;

            ReadSiteID();
            ReadGoOutside();

            int nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
            m_dbMgr = new WebDBManager(nSiteID);

            if (m_dbMgr.SiteID == 205 || m_dbMgr.SiteID == 205001) // 어반브릭스 로고 변경
            {
                picLogo.Image = SDMS_Building.Properties.Resources.Logo_urbanbricks;
                picLogo.Size = new Size(186, 66);
                picLogo.Location = new Point(6, 1);
            }

            SetLoginUserType();

            m_ufrmPoiVisible = new uPoiVisible();
            m_ufrmPoiVisible.Parent = panelBody;

            m_ufrmBroadcast = new uBroadcast();
            m_ufrmBroadcast.Parent = panelBody;

            InitPosition();

            m_optionMgr = new OptionManager();
            m_dataMgr = new DataManager(m_dbMgr);
        }

        private Image m_imgStatusConnect = SDMS_Building.Properties.Resources.status_connect;
        private Image m_imgStatusDisConnect = SDMS_Building.Properties.Resources.status_disconnect;
        private Image m_imgStatusAlarm = SDMS_Building.Properties.Resources.status_alarm;

        private Image m_imgCircleConnect = SDMS_Building.Properties.Resources.circle_connect;
        private Image m_imgCircleDisConnect = SDMS_Building.Properties.Resources.circle_disconnect;

        private void MainForm_Load(object sender, EventArgs e)
        {
            m_contentManager = new ContentManager(panelBody, m_dbMgr, this);

            //m_optionMgr = new OptionManager();
            m_optionMgr.ReadUsageStatus();
            m_optionMgr.ReadEquipZoneVolumeOption();

            m_ufrmPoiVisible.SetButtons();
            m_ufrmPoiVisible.Location = new Point(panelBody.Width - m_ufrmPoiVisible.Width - 58, panelBody.Location.Y + 490);
            m_ufrmPoiVisible.Visible = false;

            m_ufrmBroadcast.SetButtons();
            m_ufrmBroadcast.Location = new Point(panelBody.Width - m_ufrmBroadcast.Width - 58, panelBody.Location.Y + 550);
            m_ufrmBroadcast.Visible = false;

            m_netMgr = NetworkWebManager.Instance;

            ProcessManager.Instance.ProcessOwner = this;
            ProcessManager.Instance.ZoneManager = ZoneManager.Instance;

            LoadBaseData();            
            //this.WindowState = FormWindowState.Maximized;

            //System.Threading.Thread newFrmThread = new System.Threading.Thread(LoadFrm);
            //newFrmThread.Start();

            m_timerCloseDoor = new Timer();
            m_timerCloseDoor.Enabled = false;
            m_timerCloseDoor.Interval = 3000;
            m_timerCloseDoor.Tick += M_timerCloseDoor_Tick;
        }

        //private FormCCTVLink m_frmCCTVLink = null;
        //public FormCCTVLink FrmCCTVLink
        //{
        //    get { return m_frmCCTVLink; }
        //}


        public void LoadFrm()
        {
            //while (true)
            //{
            //    if (!m_netMgr.WaitForSOPServer)
            //    {
            //        if (m_frmCCTVLink == null)
            //            m_frmCCTVLink = new FormCCTVLink();
            //        break;
            //    }
            //    if (m_bExit)
            //        break;
            //}
        }

        //private bool m_readyDataLoad = false;
        public void OnReadyDataLoad()
        {
            //m_readyDataLoad = true;

            // UI가 생성된 이후에 사용될 DB Data 로드
            SensorManager.Instance.ReadAllSensorData();
            SensorZoneManager.Instance.LoadETCSensorZone();
            m_netMgr = NetworkWebManager.Instance;

            SetLayers();
            m_contentManager.ContentForm.LoadPOIs();

            LoadZoneTreeView();

            m_optionMgr.AddEquipZoneText((Panel4Unity)m_contentManager.ContentForm.OutdoorView);
            
            m_contentManager.ContentForm.RedrawWindow();

            m_ufrmReport = new uFormReport();
            m_ufrmReport.Parent = panelBody;
            m_ufrmReport.Dock = DockStyle.Fill;

            m_uFormEdit = new uFormEdit2();
            m_uFormEdit.Parent = panelBody;
            m_uFormEdit.Dock = DockStyle.Fill;

            btnTabReport.Enabled = true;
            btnTabEdit.Enabled = true;
            // 초기화가 끝났으므로 SOP Server와 통신을 개시한다.
            m_netMgr.WaitForSOPServer = false;

            // POI 타입별 LOD 사용여부를 DB로부터 읽어서 적용한다.
            m_optionMgr.SetPoiLod();

            
        }

        private void SetLayers()
        {
            ISensorTooltipOwner tooltipOwner = m_contentManager.ContentForm.OutdoorView;

            if (tooltipOwner != null && tooltipOwner is Panel4Unity)
            {
                uPoiVisible.SetLayers((Panel4Unity)tooltipOwner);
            }
        }

        public void LoadBaseData()
        {
            ZoneManager.Instance.LoadBuildingData();
            ZoneManager.Instance.LoadZones();
            ZoneManager.Instance.LoadEquipmentZone();
            SensorTagHistoryManager.Instance.LoadSensorTags(m_dbMgr);

            m_dataMgr.LoadFacilityManager();

            SensorManager.Instance.ReadDoorSensor();
        }
        
        private void SetLoginUserType()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT u.UserID, NickName, UserLevel, LevelName, BuildingID ");
            sb.Append("  FROM SopGenUser as u INNER JOIN SOPGenLevel as l ON u.UserLevel = l.ID INNER JOIN SOPGenUserBuilding as b ON u.ID=b.UserID ");            
            sb.AppendFormat(" WHERE u.ID = {0} AND SiteID = {1}", m_nSOPGenUserID, UnE.SOP.ProxySOP.Instance.SiteID);

             ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count != 5)
                return;

            string strUserID = DBUtility2.WebDBManager.GetStringField(arrResult[0], "");
            string strNickName = DBUtility2.WebDBManager.GetStringField(arrResult[1], "");
            int nUserLevel = DBUtility2.WebDBManager.GetIntField(arrResult[2].ToString(), -1);
            string strLevelName = DBUtility2.WebDBManager.GetStringField(arrResult[3], "");
            int nBuildingID = DBUtility2.WebDBManager.GetIntField(arrResult[4].ToString(), -1);

            lblUserName.Text = strUserID;
            if (lblUserName.Text.Length > 9)
                lblUserName.Text = lblUserName.Text.Substring(0, 9);

            // LevelID로 판단할 때
            //if (nUserLevel == 0)
            //    m_userType = LoginUserType.IntegratedManager;
            //else if (nUserLevel > 0 && nUserLevel < 5)
            //    m_userType = LoginUserType.IndividualManager;
            //else
            //    m_userType = LoginUserType.General;

            // LevelName으로 판단할 때            
            if (strLevelName.Replace(" ","").Contains("총괄관리자"))
            {
                m_userType = LoginUserType.IntegratedManager;
                picUser.Image = SDMS_Building.Properties.Resources.userLevel3;
            }
            else if (strLevelName.Replace(" ", "").Contains("관리자"))
            {
                m_userType = LoginUserType.IndividualManager;
                picUser.Image = SDMS_Building.Properties.Resources.userLevel2;
            }
            else
            {
                m_userType = LoginUserType.General;
                picUser.Image = SDMS_Building.Properties.Resources.userLevel1;
            }

            if (m_userType == LoginUserType.General)
            {
                btnTabEdit.Visible = false;
                panel1.Visible = false;
                btnTeamEditor.Visible = false;
            }

            m_nBuildingInCharge = nBuildingID;
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

        private void InitPosition()
        {
            this.Size = new Size(1920, 1080);
            picSensorMonitor.Location = new Point(btnTabEdit.Location.X + btnTabEdit.Width + 20, 15);
            lblDisasterInfo.Parent = picSensorMonitor;
            lblDisasterInfo.Location = new Point(50, (picSensorMonitor.Height / 2) - (lblDisasterInfo.Height / 2));

            pnRight.Location = new Point(1920, pnRight.Location.Y); // 알람이 없으므로 화면 바깥에

            // 출입문 미개폐
            picDoorInfo.Parent = pnRight;
            picDoorInfo.Location = new Point(5, 300);

            label5.Size = new Size(323, 224);
            dgvOpenDoor.Location = label5.Location;

            lblDetectCount.Location = new Point(150, 565);
            
            dgvDetectList.Location = new Point(8, 620);
            CustomGridView(dgvDetectList, 10f, Color.FromArgb(0xc4, 0xd2, 0xfa), Color.FromArgb(0x29, 0x37, 0x59), Color.FromArgb(0x53, 0x65, 0x96), Color.White);            
            CustomGridView(dgvOpenDoor, 10f, Color.FromArgb(0xc4, 0xd2, 0xfa), Color.FromArgb(0x29, 0x37, 0x59), Color.FromArgb(0x53, 0x65, 0x96), Color.White);
            dgvDetectList.ScrollBars = ScrollBars.Vertical;            

            label1.Location = new Point(dgvDetectList.Location.X, dgvDetectList.Location.Y - label1.Height);
            label2.Location = new Point(label1.Location.X + label1.Width, label1.Location.Y);
            label3.Location = new Point(label2.Location.X + label2.Width, label1.Location.Y);
            label4.Location = new Point(label3.Location.X + label3.Width, label1.Location.Y);

            lblUserName.Location = new Point(btnManagement.Location.X - lblUserName.Width - 10, (pnTop.Height / 2) - (lblUserName.Height / 2));
            picUser.Location = new Point(lblUserName.Location.X - picUser.Width, (pnTop.Height / 2) - (picUser.Height / 2));

            btnTester.Size = new Size(86, 31);
            btnTeamEditor.Size = new Size(105, 31);

            btnTester2.Location = new Point(picUser.Location.X - btnTester2.Width - 15, (pnTop.Height / 2) - (btnTester2.Height / 2));
            btnTeamEditor2.Location = new Point(btnTester2.Location.X - btnTeamEditor2.Width - 5, (pnTop.Height / 2) - (btnTeamEditor2.Height / 2));

            ResizeBody();
        }

        private Pen m_penLine1 = new Pen(Color.FromArgb(0x25, 0x31, 0x50));
        private void pnLeft_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Image imgBuildingInfo = global::SDMS_Building.Properties.Resources.pnLeft_BuildingInfo;
            g.DrawImage(imgBuildingInfo, 0, 20, imgBuildingInfo.Width, imgBuildingInfo.Height);
        }

        private void pnTop_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Image imgLogo = global::SDMS_Building.Properties.Resources.logo;
            g.DrawImage(imgLogo, 23, (pnTop.Height - imgLogo.Height) / 2, imgLogo.Width, imgLogo.Height);
        }

        private Image m_imgTab_Monitoring_Normal = global::SDMS_Building.Properties.Resources.pnTop_Monitoring_Normal;
        private Image m_imgTab_Monitoring_Click = global::SDMS_Building.Properties.Resources.pnTop_Monitoring_Click;
        private Image m_imgTab_Monitoring_Hover = global::SDMS_Building.Properties.Resources.pnTop_Monitoring_Hover;

        private Image m_imgTab_Report_Normal = global::SDMS_Building.Properties.Resources.pnTop_Report_Normal;
        private Image m_imgTab_Report_Click = global::SDMS_Building.Properties.Resources.pnTop_Report_Click;
        private Image m_imgTab_Report_Hover = global::SDMS_Building.Properties.Resources.pnTop_Report_Hover;

        private Image m_imgTab_Edit_Normal = global::SDMS_Building.Properties.Resources.pnTop_Edit_Normal;
        private Image m_imgTab_Edit_Click = global::SDMS_Building.Properties.Resources.pnTop_Edit_Click;
        private Image m_imgTab_Edit_Hover = global::SDMS_Building.Properties.Resources.pnTop_Edit_Hover;

        private void btnTab_Click(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            if (btn == null)
                return;

            if (btn == btnTabMonitoring)
            {
                ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);
            }
            else if (btn == btnTabReport)
            {
                ChangeTab(UnE.View.Content.ContentOwnerTab.REPORT_TAB);
            }
            else if (btn == btnTabEdit)
            {
                ChangeTab(UnE.View.Content.ContentOwnerTab.ADMIN_TAB);
            }
        }

        public int ChangeTab(UnE.View.Content.ContentOwnerTab tab)
        {
            switch (tab)
            {
                case UnE.View.Content.ContentOwnerTab.M3D_TAB:
                    if (m_CurrentTab != UnE.View.Content.ContentOwnerTab.M3D_TAB)
                    {
                        SelectMonitoringTab();
                    }
                    break;
                case UnE.View.Content.ContentOwnerTab.REPORT_TAB:
                    if (m_CurrentTab != UnE.View.Content.ContentOwnerTab.REPORT_TAB)
                    {
                        SelectReportTab();
                    }
                    break;
                case UnE.View.Content.ContentOwnerTab.ADMIN_TAB:
                    if (m_CurrentTab != UnE.View.Content.ContentOwnerTab.ADMIN_TAB)
                    {
                        SelectEditTab();
                    }
                    break;
            }

            m_contentManager.CurrentTab = m_CurrentTab;
            return (int)m_CurrentTab;
        }

        #region Change tab
        private void SelectMonitoringTab()
        {
            m_uFormEdit.VisibleImageEditMode(false);

            if (CheckEditData() == false)
            {
                // 편집모드에서 작업한 내용을 모두 버리고 새로 DB 내용을 얻어온다.
                bool isOutdoor;
                Floor floor;

                if (GetFloorInfo(out isOutdoor, out floor))
                {
                    LoadFloor(isOutdoor, floor);
                }
            }

            if (!m_bPnRightOpen && m_SensorDectects.Count > 0)
                SetRightPanelSlide();
            
            EnableOutdoor(true);
            m_CurrentTab = UnE.View.Content.ContentOwnerTab.M3D_TAB;

            btnTabMonitoring.Image = m_imgTab_Monitoring_Click;
            btnTabMonitoring.ImageNormal = m_imgTab_Monitoring_Click;
            btnTabMonitoring.ImageMouseOver = m_imgTab_Monitoring_Click;

            btnTabReport.Image = m_imgTab_Report_Normal;
            btnTabReport.ImageNormal = m_imgTab_Report_Normal;
            btnTabReport.ImageMouseOver = m_imgTab_Report_Hover;

            btnTabEdit.Image = m_imgTab_Edit_Normal;
            btnTabEdit.ImageNormal = m_imgTab_Edit_Normal;
            btnTabEdit.ImageMouseOver = m_imgTab_Edit_Hover;

            pnTabSelect.Location = new Point(btnTabMonitoring.Location.X, btnTabMonitoring.Location.Y + btnTabMonitoring.Height - pnTabSelect.Height);

            m_contentManager.ContentForm.Visible = true;
            m_ufrmReport.Visible = false;
            m_uFormEdit.Visible = false;

            m_contentManager.ContentForm.OutdoorView.RollBackPOIIcon("");
            TooltipCCTVCtrl2.CloseAll();
            
            uPoiVisible.SetLayers(null);
        }

        private void SelectReportTab()
        {
            if (m_bPnRightOpen && m_SensorDectects.Count > 0)
                SetRightPanelSlide();

            SDMSPopupFactory.Instance.CloseAll();

            CheckEditData();

            EnableOutdoor(true);
            m_CurrentTab = UnE.View.Content.ContentOwnerTab.REPORT_TAB;

            btnTabMonitoring.Image = m_imgTab_Monitoring_Normal;
            btnTabMonitoring.ImageNormal = m_imgTab_Monitoring_Normal;
            btnTabMonitoring.ImageMouseOver = m_imgTab_Monitoring_Hover;

            btnTabReport.Image = m_imgTab_Report_Click;
            btnTabReport.ImageNormal = m_imgTab_Report_Click;
            btnTabReport.ImageMouseOver = m_imgTab_Report_Click;

            btnTabEdit.Image = m_imgTab_Edit_Normal;
            btnTabEdit.ImageNormal = m_imgTab_Edit_Normal;
            btnTabEdit.ImageMouseOver = m_imgTab_Edit_Hover;

            pnTabSelect.Location = new Point(btnTabReport.Location.X, btnTabReport.Location.Y + btnTabReport.Height - pnTabSelect.Height);

            m_contentManager.ContentForm.Visible = false;
            m_ufrmReport.Visible = true;
            m_uFormEdit.Visible = false;

            SetVisible3DPopup(false);
        }

        private void SelectEditTab()
        {
            SDMSPopupFactory.Instance.CloseAll();
            m_contentManager.ContentForm.Visible = false;
            
            EnableOutdoor(false);
            
            m_CurrentTab = UnE.View.Content.ContentOwnerTab.ADMIN_TAB;

            btnTabMonitoring.Image = m_imgTab_Monitoring_Normal;
            btnTabMonitoring.ImageNormal = m_imgTab_Monitoring_Normal;
            btnTabMonitoring.ImageMouseOver = m_imgTab_Monitoring_Hover;

            btnTabReport.Image = m_imgTab_Report_Normal;
            btnTabReport.ImageNormal = m_imgTab_Report_Normal;
            btnTabReport.ImageMouseOver = m_imgTab_Report_Hover;

            btnTabEdit.Image = m_imgTab_Edit_Click;
            btnTabEdit.ImageNormal = m_imgTab_Edit_Click;
            btnTabEdit.ImageMouseOver = m_imgTab_Edit_Click;

            pnTabSelect.Location = new Point(btnTabEdit.Location.X, btnTabEdit.Location.Y + btnTabEdit.Height - pnTabSelect.Height);

            m_ufrmReport.Visible = false;
            m_uFormEdit.Visible = true;

            // 편집을 위하여 평면뷰로 변경한다
            Zone zone = GetZone();
            //LoadFloor(false, zone.Floor);
            string strSceneName;
            if (m_optionMgr.DicZoneScene.TryGetValue(zone.ID, out strSceneName))
            {
                m_contentManager.ContentForm.HideAllAlarmZones();
                m_contentManager.ContentForm.SelectScene(strSceneName);
            }

            m_contentManager.ContentForm.OutdoorView.RollBackPOIIcon("");
            TooltipCCTVCtrl2.CloseAll();

            SetVisible3DPopup(false);

            SetLayerState();

            m_uFormEdit.CurrentMode();
        }
        #endregion

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();

        private void pnTop_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Left)
            //{
            //    m_bLeftMouseDown = true;
            //    m_ptMove = Control.MousePosition;
            //    m_ptOrigin = this.Location;
            //}

            //m_isClicked = true;
        }

        private void pnTop_MouseMove(object sender, MouseEventArgs e)
        {
            //if (!m_isClicked)
            //    return;

            //if (!m_bLeftMouseDown)
            //    return;

            //Point ptScreen = Control.MousePosition;

            //int dx = ptScreen.X - m_ptMove.X;
            //int dy = ptScreen.Y - m_ptMove.Y;

            //if (dx == 0 && dy == 0)
            //    return;

            //Point ptCur = this.Location;
            //this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            //m_ptMove.X += dx;
            //m_ptMove.Y += dy;
        }

        private void pnTop_MouseUp(object sender, MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Left)
            //    m_bLeftMouseDown = false;

            //m_isClicked = false;
        }

        private void pnTop_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Left)
            //{
            //    SetWindowPosition(this);
            //}
        }

        private static bool SetWindowPosition(Form frm, Point pt)
        {
            foreach (Screen sc in Screen.AllScreens)
            {
                if (pt.X >= sc.Bounds.Left && pt.X <= sc.Bounds.Right &&
                    pt.Y >= sc.Bounds.Top && pt.Y <= sc.Bounds.Bottom)
                {
                    frm.Location = new Point(sc.Bounds.Left, sc.Bounds.Top);
                    return true;
                }
            }

            return false;
        }

        public static bool SetWindowPosition(Form frm)
        {
            if (SetWindowPosition(frm, frm.Location))
                return true;

            Point ptBL = new Point(frm.Location.X, frm.Location.Y + frm.Size.Height);
            Point ptTR = new Point(frm.Location.X + frm.Size.Width, frm.Location.Y);
            Point ptBR = new Point(frm.Location.X + frm.Size.Width, frm.Location.Y + frm.Size.Height);
            Point ptMiddle = new Point((frm.Location.X + ptBR.X) / 2, (frm.Location.Y + ptBR.Y) / 2);

            if (SetWindowPosition(frm, ptBL))
                return true;
            if (SetWindowPosition(frm, ptTR))
                return true;
            if (SetWindowPosition(frm, ptBR))
                return true;
            if (SetWindowPosition(frm, ptMiddle))
                return true;

            return false;
        }
        #endregion
                
        private void btnManagement_Click(object sender, EventArgs e)
        {
            PopupBackground back = new PopupBackground();
            back.StartPosition = FormStartPosition.Manual;
            back.Size = this.Size;
            back.Location = this.Location;
            back.Show();

            m_frmManagement = new FormManagement();
            m_frmManagement.StartPosition = FormStartPosition.CenterParent;
            m_frmManagement.ShowDialog();
            back.Close();
        }

        private void ClosePopup()
        {
            if (m_frmManagement != null)
            {
                m_frmManagement.Close();
                m_frmManagement = null;
            }

            if (m_frmManualReport != null)
            {
                m_frmManualReport.Close();
                m_frmManualReport = null;
            }

            if (m_popDetailLog != null)
            {
                m_popDetailLog.Close();
                m_popDetailLog = null;
            }
        }

        #region 3D Panel Slide
        private bool m_bPnLeftOpen = true;
        private Timer pnLeftTimer = null;
        public void SetLeftPanelSlide()
        {
            int maxWidth = 0;
            int offset = 5;
            int minWidth = 0 - pnLeft.Width;

            offset = 15;

            if (pnLeftTimer == null)
            {
                pnLeftTimer = new Timer();
                pnLeftTimer.Interval = 10;
            }

            pnLeftTimer.Enabled = true;

            pnLeftTimer.Tick += (s, g) =>
            {
                if (m_bPnLeftOpen)
                {
                    if (pnLeft.Location.X == maxWidth)
                    {
                        pnLeftTimer.Enabled = false;

                        ResizeBody();
                    }
                    else if (pnLeft.Location.X + offset >= maxWidth)
                    {
                        int ww = maxWidth - pnLeft.Location.X;
                        pnLeft.Location = new Point(pnLeft.Location.X + ww, pnLeft.Location.Y);
                        pnLeftTimer.Enabled = false;

                        ResizeBody();
                    }
                    else
                        pnLeft.Location = new Point(pnLeft.Location.X + offset, pnLeft.Location.Y);
                }
                else
                {
                    if (pnLeft.Location.X == minWidth)
                    {
                        pnLeftTimer.Enabled = false;

                        ResizeBody();
                    }
                    else if (pnLeft.Location.X - offset <= minWidth)
                    {
                        int ww = pnLeft.Location.X - minWidth;
                        pnLeft.Location = new Point(pnLeft.Location.X - ww, pnLeft.Location.Y);
                        pnLeftTimer.Enabled = false;

                        ResizeBody();
                    }
                    else
                        pnLeft.Location = new Point(pnLeft.Location.X - offset, pnLeft.Location.Y);
                }
            };

            m_bPnLeftOpen = !m_bPnLeftOpen;
        }

        private bool m_bPnRightOpen = false;
        private Timer pnRightTimer = null;
        public void SetRightPanelSlide()
        {
            int maxWidth = this.Width;
            int offset = 5;
            int minWidth = this.Width - pnRight.Width - 15;

            offset = 15;

            if (/*!m_bPnRightOpen*/pnRight.Location.X >= maxWidth)
            {
                m_bPnRightOpen = true;
                pnRight.Location = new Point(minWidth, pnRight.Location.Y);
                ResizeBody();
                return;
            }

            if (pnRightTimer == null)
            {
                pnRightTimer = new Timer();
                pnRightTimer.Interval = 10;
            }

            pnRightTimer.Enabled = true;

            pnRightTimer.Tick += (s, g) =>
            {
                if (!m_bPnRightOpen)
                {
                    //if (pnRight.Location.X == minWidth)
                    //    pnRightTimer.Enabled = false;
                    //else
                    //    pnRight.Location = new Point(pnRight.Location.X - offset, pnRight.Location.Y);

                    // 버벅대는 현상이 너무 심해서 펼칠 때는 사용하지 않음
                    pnRightTimer.Enabled = false;
                    m_bPnRightOpen = true;
                }
                else
                {
                    if (/*pnRight.Location.X == maxWidth*/ pnRight.Location.X >= maxWidth)
                    {
                        pnRightTimer.Enabled = false;
                        m_bPnRightOpen = false;
                    }
                    else if (pnLeft.Location.X + offset >= maxWidth)
                    {
                        int ww = maxWidth - pnLeft.Location.X;
                        pnLeft.Location = new Point(pnLeft.Location.X + ww, pnLeft.Location.Y);
                        pnLeftTimer.Enabled = false;
                        m_bPnRightOpen = false;
                    }
                    else
                        pnRight.Location = new Point(pnRight.Location.X + offset, pnRight.Location.Y);
                }

                if (!pnRightTimer.Enabled)
                    ResizeBody();
            };
        }
        #endregion

        private Image m_imgPnRightAlarmBorder = global::SDMS_Building.Properties.Resources.pnRight_alarmBorder;
        private Image m_imgPnRightDetectInfo = global::SDMS_Building.Properties.Resources.pnRight_DetectInfo;
        private Image m_imgPnRightDoorInfo = global::SDMS_Building.Properties.Resources.pnRight_DoorInfo;
        private void pnRight_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawImage(m_imgPnRightAlarmBorder, 5, 5, 337, 1000);
            g.DrawImage(m_imgPnRightDetectInfo, 5, 565, 337, 22);
        }

        private TreeModel m_treeModel = null;


        private void picTreeViewRefresh_Click(object sender, EventArgs e)
        {
            SetCurrentZone();
        }

        public void SetCurrentZone()
        {
            if (m_selectedFloorNode == null)
                return;

            treeViewAdv1.SelectedNode = treeViewAdv1.FindNode2(m_treeModel.GetPath(m_selectedFloorNode));
        }
        
        public void LoadZoneTreeView()
        {
            m_treeModel = new TreeModel();
            treeViewAdv1.Model = m_treeModel;
            
            foreach (KeyValuePair<int, Building> item in ZoneManager.Instance.DicBuildings.OrderBy(p => p.Key))
            {
                if (item.Value.ID == 0)
                    continue;

                Node node = new Node(item.Value.DisplayText);
                node.Tag = item.Value;
                node.ClrStatus = Color.FromArgb(0xff, 0xff, 0x00);
                
                AddNode(m_treeModel.Nodes, node, item.Value);
            }
        }

        private void LoadSensorTreeView(System.Collections.ObjectModel.Collection<Node> nodes, int zoneID)
        {            
            if (SensorManager.Instance.DicFireSensorByZoneID.ContainsKey(zoneID))
            {
                Node fireNode = new Node("화재 센서");                
                fireNode.Tag = IFacility.FacilityType.FIRE_SENSOR;
                nodes.Add(fireNode);

                List<ISensor> sensors = SensorManager.Instance.DicFireSensorByZoneID[zoneID];
                foreach (ISensor sensor in sensors)
                {
                    if (!sensor.DeActivate && !m_bViewAllSensor)
                        continue;

                    Node node = new Node(sensor.SensorName);                    
                    node.Tag = sensor;

                    if (sensor.DeActivate) // 센서 신호 비활성화
                        node.ClrStatus = Color.FromArgb(0x65, 0x65, 0x65);
                    else // 센서 신호 활성화
                    {
                        bool isAlarm = false;                        
                        foreach (ProcessIF item in m_SensorDectects)
                        {
                            if (item.ProcessType == ProcessType.FireAlarm)
                            {
                                if (item.TargetSensor.ID == sensor.ID)
                                {
                                    node.ClrStatus = Color.Red;
                                    isAlarm = true;
                                    break;
                                }
                            }
                        }

                        if (!isAlarm)
                            node.ClrStatus = Color.FromArgb(0x00, 0xff, 0x00);
                    }
                    
                    fireNode.Nodes.Add(node);
                }
            }

            if (CCTVManager.Instance.DicCCTVs.Count > 0)
            {
                bool addTitle = false;
                Node cctvNode = null;
                foreach (KeyValuePair<int, CCTV> cctv in CCTVManager.Instance.DicCCTVs)
                {
                    if (cctv.Value.POI.Zone.ID == zoneID)
                    {
                        if (!addTitle)
                        {
                            cctvNode = new Node("CCTV");
                            cctvNode.Tag = IFacility.FacilityType.CCTV;
                            nodes.Add(cctvNode);

                            addTitle = true;
                        }

                        Node node = new Node(cctv.Value.AccessKey);
                        node.Tag = cctv.Value;
                        cctvNode.Nodes.Add(node);
                    }
                }
            }

            if (SensorManager.Instance.DicDoorSensorByZoneID.ContainsKey(zoneID))
            {
                Node doorNode = new Node("출입문");
                doorNode.Tag = IFacility.FacilityType.DOOR;
                nodes.Add(doorNode);

                List<ISensor> sensors = SensorManager.Instance.DicDoorSensorByZoneID[zoneID];
                foreach (EtcSensor sensor in sensors)
                {
                    //if (!sensor.DeActivate && !m_bViewAllSensor)
                    //    continue;

                    Node node = new Node(sensor.SensorName);
                    node.Tag = sensor;
                    node.ClrStatus = Color.FromArgb(0x00, 0xff, 0x00);
                    doorNode.Nodes.Add(node);
                }
            }
        }

        private void AddNode(System.Collections.ObjectModel.Collection<Node> nodes, Node node, object data)
        {
            if (data is Building)
            {
                Building building = data as Building;
                //if (building.BuildingGroup.Parent == null)
                    nodes.Add(node);
                
                if (building.FloorList != null && building.FloorList.Count > 0)
                {
                    ArrayList arrFloor = (ArrayList)building.FloorList.Clone();

                    foreach (Zone floor in arrFloor)
                    {
                        Node floorNode = new Node(floor.DisplayText);
                        floorNode.Tag = floor;
                        AddNode(node.Nodes, floorNode, floor);
                    }
                }
            }   
            else if (data is Zone)
            {
                nodes.Add(node);

                Zone zone = data as Zone;

                // 총괄 관리자는 전체 센서를 관제/확인 가능, 건물별 관리자는 해당하는 건물 이외의 센서를 확인 불가능
                if (m_nBuildingInCharge != -1 && m_nBuildingInCharge != zone.Building.ID)
                    return;

                LoadSensorTreeView(node.Nodes, zone.ID);
            }
        }

        public Zone GetZone()
        {
            if (m_selectedFloor == null)
            {
                if (m_CurrentTab == ContentOwnerTab.ADMIN_TAB)
                {
                    for (int i = 0; i < m_treeModel.Nodes.Count; i++)
                    {
                        if (m_treeModel.Nodes[i].Tag is Building)
                        {
                            for (int j = 0; j  < m_treeModel.Nodes[i].Nodes.Count; j ++)
                            {
                                if (m_treeModel.Nodes[i].Nodes[j].Tag is Zone)
                                {
                                    Zone zone = m_treeModel.Nodes[i].Nodes[j].Tag as Zone;
                                    if (zone.Floor.ToString() == "1층")
                                    {
                                        m_selectedBuilding = zone.Building;
                                        m_selectedFloor = zone.Floor;
                                        m_selectedFloorNode = m_treeModel.Nodes[i].Nodes[j];
                                        lblZoneName.Text = zone.Floor.Zone.DisplayText;

                                        return zone;
                                    }
                                }
                            }
                        }    


                    }
                }

                return null;
            }

            return m_selectedFloor.Zone;

            //if (m_cbFloor.customComboBox.SelectedIndex < 0)
            //{
            //    m_systemInput = true;
            //    bool selected = false;
            //    for (int i = 0; i < m_cbFloor.customComboBox.Items.Count; i++)
            //    {
            //        if (m_cbFloor.customComboBox.Items[i].ToString() == "1층")
            //        {
            //            m_cbFloor.customComboBox.SelectedIndex = i;
            //            selected = true;
            //            break;
            //        }
            //    }

            //    if (!selected)
            //        m_cbFloor.customComboBox.SelectedIndex = 0;
            //    m_systemInput = false;
            //}

            //Floor floor = m_cbFloor.customComboBox.Items[m_cbFloor.customComboBox.SelectedIndex] as Floor;
            //if (floor == null)
            //    return null;
            //if (floor.Zone == null)
            //    return null;

            //return floor.Zone;
        }

        private Building m_selectedBuilding = null;
        public Building SelectedBuilding
        {
            get { return m_selectedBuilding; }
            set { m_selectedBuilding = value; }
        }

        private Node m_selectedFloorNode = null;

        private Floor m_selectedFloor = null;
        public Floor SelectedFloor
        {
            get { return m_selectedFloor; }
            set { m_selectedFloor = value; }
        }
        private bool m_bZoneSelection = false;

        private void treeViewAdv1_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Node.Tag is Aga.Controls.Tree.Node)
            {
                Node selectedNode = e.Node.Tag as Aga.Controls.Tree.Node;
                if (selectedNode.Tag is Building)
                {
                    Building building = selectedNode.Tag as Building;                    
                    if (building.ID == 0) // 외부
                    {
                        MoveZone(true, null);
                    }
                    
                }
                else if (selectedNode.Tag is Zone)
                {
                    Zone zone = selectedNode.Tag as Zone;
                    
                    MoveZone(false, zone);
                    m_selectedFloorNode = selectedNode;
                }
                else if (selectedNode.Tag is ISensor)
                {
                    ISensor sensor = selectedNode.Tag as ISensor;
                    if (sensor == null)
                        return;

                    if (m_selectedFloor == null || m_selectedFloor.Zone.ID != sensor.ZoneID)
                    {
                        // 층 이동부터 
                        Zone zone = ZoneManager.Instance.GetZone(sensor.ZoneID);
                        MoveZone(false, zone);
                    }
                    else if (m_selectedFloor == null || m_selectedFloor.Zone.ID == sensor.ZoneID)
                    {
                        if (m_CurrentTab == ContentOwnerTab.ADMIN_TAB)
                        {
                            SetDeActivate(sensor);
                        }
                    }

                    if (m_selectedFloor == null)
                        return;

                    if (m_selectedFloor.Zone.ID != sensor.ZoneID)
                        return;

                    POI poi = null;
                    string poiType = "";
                    if (sensor.Type == IFacility.FacilityType.FIRE_SENSOR)
                    {
                        if (!m_dataMgr.DicFirePOI.ContainsKey(sensor.OrgSensorID))
                            return;

                        poi = m_dataMgr.DicFirePOI[sensor.OrgSensorID];
                        poiType = CommonString.POI_Fire;
                        //string poiType = "Door";  
                    }
                    else if (sensor.Type == IFacility.FacilityType.DOOR)
                    {
                        if (!SensorManager.Instance.DicDoorPOI.ContainsKey(sensor.OrgSensorID))
                            return;

                        poi = SensorManager.Instance.DicDoorPOI[sensor.OrgSensorID];
                        poiType = CommonString.POI_Door;
                    }

                    Panel4Unity panel = (Panel4Unity)m_contentManager.ContentForm.OutdoorView;
                    panel.RollBackPOIIcon("");
                    m_dataMgr.ChangePOIIcon(poi, poiType + "_Click");
                }
                else if (selectedNode.Tag is CCTV)
                {
                    CCTV cctv = selectedNode.Tag as CCTV;
                    if (cctv == null)
                        return;

                    if (m_selectedFloor == null || m_selectedFloor.Zone.ID != cctv.POI.Zone.ID)
                    {
                        // 층 이동부터 
                        MoveZone(false, cctv.POI.Zone);
                    }

                    if (m_selectedFloor == null)
                        return;

                    if (m_selectedFloor.Zone.ID != cctv.POI.Zone.ID)
                        return;

                    POI poi = cctv.POI;
                    string poiType = "CCTV";

                    Panel4Unity panel = (Panel4Unity)m_contentManager.ContentForm.OutdoorView;
                    if (m_uFormEdit.CurEditSubType != EditSubType.EquipmentZone)
                        panel.RollBackPOIIcon("");
                    m_dataMgr.ChangePOIIcon(poi, poiType + "_Click");
                }
            }
        }

        private ISensor m_selectedSensor = null;
        private void treeViewAdv1_SelectionChanged(object sender, EventArgs e)
        {
            if (treeViewAdv1.SelectedNode == null)
                return;

            if (treeViewAdv1.SelectedNode.Tag is Node)
            {
                Node selectedNode = treeViewAdv1.SelectedNode.Tag as Aga.Controls.Tree.Node;
                //if (selectedNode.Tag is Zone)
                //{
                //    Zone zone = selectedNode.Tag as Zone;
                //    if (zone == null)
                //        return;

                //    m_selectedBuilding = zone.Building;
                //    m_selectedFloor = zone.Floor;
                //    m_selectedFloorNode = selectedNode;
                //}

                if (m_CurrentTab == ContentOwnerTab.ADMIN_TAB)
                {
                    if (m_uFormEdit == null)
                        return;

                    //if (selectedNode.Tag is ISensor)
                    //{
                    //    ISensor sensor = selectedNode.Tag as ISensor;
                    //    if (sensor == null)
                    //        return;

                    //    if (sensor.Type == IFacility.FacilityType.FIRE_SENSOR)
                    //    {
                    //        if (m_selectedSensor == sensor)
                    //        {
                    //            MessageBox.Show("같음");
                    //        }
                    //        else
                    //            m_selectedSensor = sensor;
                    //    }
                    //}
                    if (selectedNode.Tag is CCTV)
                    {
                        CCTV cctv = selectedNode.Tag as CCTV;
                        if (cctv != null)
                            m_uFormEdit.OnSelectSensor(cctv, IFacility.FacilityType.CCTV);
                    } 
                }
            }
        }

        private void MoveZone(bool isOutDoor, Zone zone)
        {
            string strMsg = "";
            if (!m_systemInput)
            {
                if (isOutDoor)
                {
                    if (m_selectedBuilding != null)
                    {
                        strMsg = "외부 전경으로 이동하시겠습니까 ?";
                    }
                }
                else
                {
                    if (m_selectedFloor == null || zone != m_selectedFloor.Zone)
                    {
                        strMsg = zone.Building.BuildingName + zone.Floor.ToString() + "으로 이동하시겠습니까 ?";
                    }
                }

                CheckEditData(); 
            }

            if (strMsg.Length > 0)
            {
                FormMessageBox msg = new FormMessageBox(strMsg, MessageBoxButtons.YesNo);
                Point pt = this.PointToScreen(pnLeft.Location);
                msg.StartPosition = FormStartPosition.Manual;
                msg.Location = new Point(pt.X + pnLeft.Width + 10, pt.Y + 10);
                if (msg.ShowDialog() == DialogResult.No)
                    return;
            }
                        
            if (m_selectedFloor != null && m_selectedFloor.Zone != null)
                RollbackCloseDoor(m_selectedFloor.Zone.ID);

            if (isOutDoor)
            {
                m_selectedBuilding = null;
                m_selectedFloor = null;
                m_selectedFloorNode = null;
                LoadFloor(true, null);

                lblZoneName.Text = "외부";
            }
            else
            {
                m_selectedBuilding = zone.Building;
                m_selectedFloor = zone.Floor;

                LoadFloor(false, zone.Floor);

                lblZoneName.Text = zone.Floor.Zone.DisplayText;
            }

            LoadPOIs();
            LoadWalls();            
        }

        public bool GetFloorInfo(out bool isoutDoor, out Floor floor)
        {
            isoutDoor = false;
            if (m_selectedBuilding == null)
            {
                isoutDoor = true;
            }

            if (isoutDoor)
            {
                floor = null;
            }
            else
            {
                floor = m_selectedFloor;
                if (m_selectedFloor.Zone == null)
                {
                    if (!m_systemInput)
                    {
                        FormMessageBox msg = new FormMessageBox("위치 정보가 없습니다.", MessageBoxButtons.YesNo);
                        msg.StartPosition = FormStartPosition.CenterParent;
                        msg.ShowDialog();
                    }

                    return false;
                }
            }

            return true;
        }

        private void LoadFloor(bool isoutDoor, Floor floor)
        {
            Panel4Unity panel = (Panel4Unity)m_contentManager.ContentForm.OutdoorView;

            if (isoutDoor)
            {
                m_contentManager.ContentForm.HideAllAlarmZones();
                m_contentManager.ContentForm.SelectScene("Outdoor");
                panel.ShowLayer(ID.ID_LAYER_BUILDING_TEXT, false);
            }
            else
            {
                string strSceneName;
                if (m_optionMgr.DicZoneScene.TryGetValue(floor.Zone.ID, out strSceneName))
                {
                    m_contentManager.ContentForm.HideAllAlarmZones();
                    m_contentManager.ContentForm.SelectScene(strSceneName);
                    //panel.ShowLayer(ID.ID_LAYER_BUILDING_TEXT, m_ufrmViewText.btnViewText.IsChecked);
                    
                    panel.RollBackPOIIcon("");
                }
                else
                {
                    if (!m_systemInput)
                    {
                        FormMessageBox msg = new FormMessageBox("위치 정보가 없습니다.", MessageBoxButtons.YesNo);
                        msg.StartPosition = FormStartPosition.CenterParent;
                        msg.ShowDialog();
                    }

                    return;
                }
            }

            //m_selectedBuilding = m_cbBuilding.customComboBox.Items[m_cbBuilding.customComboBox.SelectedIndex] as Building;
            //m_selectedBuilding = floor.Zone.Building;
            //m_selectedFloor = floor;

            foreach (ProcessIF process in m_SensorDectects)
            {
                //if (process.ProcessType == ProcessType.StrongWindAlarm || process.TargetZone == null || process.TargetZone.Floor == null)
                //{
                //    m_contentManager.ContentForm.SelectScene("Outdoor");
                //}
                //else
                {
                    if (process.TargetZone != null)
                    {
                        if (process.TargetZone.Building == m_selectedBuilding && process.TargetZone.Floor == m_selectedFloor)
                        {
                            string strVolume, strSceneName;

                            if (m_optionMgr.UseEquipZoneVolume && m_optionMgr.DicEquiZoneVolume.TryGetValue(process.TargetZone.ID, out strVolume) && m_optionMgr.DicZoneScene.TryGetValue(process.TargetZone.LinkedZone.ID, out strSceneName))
                            {
                                //m_contentManager.ContentForm.ShowAlarmZone(strVolume, true);
                                m_contentManager.ContentForm.ZoomBuilding(strVolume);
                            }

                            break;
                        }  
                    }
                }
            }
        }

        private void LoadPOIs()
        {
            Trace.WriteLine("LoadPOIs()");

            // 현재 3D화면에 나타나있는 모든 팝업창을 닫는다.
            SDMSPopupFactory.Instance.CloseAll();
            DataManager.LoadCCTVPOI(m_dbMgr, true, true);
            DataManager.LoadSensorPOI(m_dbMgr, false, true, IFacility.FacilityType.FIRE_SENSOR);
            DataManager.LoadSensorPOI(m_dbMgr, false, true, IFacility.FacilityType.PSM_SENSOR);
            DataManager.LoadSensorPOI(m_dbMgr, false, true, IFacility.FacilityType.DOOR);
            //DataManager.LoadSensorPOI(m_dbMgr, false, true, IFacility.FacilityType.FIREWALL);
        }

        private void LoadWalls()
        {
            m_uFormEdit.LoadWalls();
            m_uFormEdit.LoadSpaceTexts();
        }

        private bool CheckEditData()
        {
            if (m_CurrentTab == ContentOwnerTab.ADMIN_TAB && m_uFormEdit != null)
            {
                if (m_uFormEdit.HasChange())
                {
                    string str = "저장하지 않은 데이터가 존재합니다.\r\n이대로 두면 편집한 데이터는 모두 사라집니다.\r\n데이터를 저장하시겠습니까?";

                    FormMessageBox msg = new FormMessageBox(str, MessageBoxButtons.YesNo);
                    Point pt = this.PointToScreen(pnLeft.Location);
                    msg.StartPosition = FormStartPosition.Manual;
                    msg.Location = new Point(pt.X + pnLeft.Width + 10, pt.Y + 10);

                    if (msg.ShowDialog() == DialogResult.Yes)
                    {
                        m_uFormEdit.Save();
                    }
                    else
                    {
                        m_uFormEdit.ClearChange();
                        return false;
                    }
                }
                else
                {
                    m_uFormEdit.ClearChange();
                }
            }

            return true;
        }

        /// <summary>
        /// 선택 전 Zone으로 Combobox 변경
        /// </summary>
        private void RollbackComboBox()
        {
            // 외부
            //if (m_selectedBuilding == null || m_selectedBuilding.BuildingName == "외부")
            //{                
            //    m_bZoneSelection = true;
            //    m_cbBuilding.customComboBox.SelectedIndex = 0;
            //    m_bZoneSelection = false;

            //    m_cbFloor.customComboBox.SelectionChanged -= cbFloor_SelectionChanged;
            //    m_cbFloor.customComboBox.SelectedIndex = 0;
            //    m_cbFloor.customComboBox.SelectionChanged += cbFloor_SelectionChanged;
            //}
            //else
            //{
            //    for (int i = 0; i < m_cbBuilding.customComboBox.Items.Count; i++)
            //    {
            //        if (m_selectedBuilding == m_cbBuilding.customComboBox.Items[i] as Building)
            //        {
            //            m_bZoneSelection = true;
            //            m_cbBuilding.customComboBox.SelectedIndex = i;
            //            m_bZoneSelection = false;
            //            break;
            //        }
            //    }

            //    for (int i = 0; i < m_cbFloor.customComboBox.Items.Count; i++)
            //    {
            //        if (m_selectedFloor == m_cbFloor.customComboBox.Items[i] as Floor)
            //        {
            //            m_cbFloor.customComboBox.SelectionChanged -= cbFloor_SelectionChanged;
            //            m_cbFloor.customComboBox.SelectedIndex = i;
            //            m_cbFloor.customComboBox.SelectionChanged += cbFloor_SelectionChanged;

            //            break;
            //        }
            //    }
            //}
        }

        public void DisplaySensorInfo()
        {

        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            ResizeBody();
        }

        private void ResizeBody()
        {
            if (m_dataMgr == null)
                return;

            panelBody.Location = new Point(pnLeft.Location.X + pnLeft.Size.Width, pnLeft.Location.Y);
            panelBody.Size = new Size(pnRight.Location.X - panelBody.Location.X, pnLeft.Size.Height);
            
            m_ufrmPoiVisible.Location = new Point(panelBody.Width - m_ufrmPoiVisible.Width - 58, panelBody.Location.Y + 480);
            m_ufrmBroadcast.Location = new Point(panelBody.Width - m_ufrmBroadcast.Width - 58, panelBody.Location.Y + 550);
        }

        public void SetNormalMode()
        {
            //m_contentManager.ContentForm.HideZoneVolume();
            //m_contentManager.ContentForm.HideEvacCircle();
            //DataManager.ClearPOI("Fire");
            //DataManager.ClearPOI("FireAlarmOn");
        }

        private Image m_imgFireDetectLogo = SDMS_Building.Properties.Resources.poi_fire_detect;
        private Image m_imgPSMDetectLogo = SDMS_Building.Properties.Resources.poi_psm_detect;
        private Image m_imgStrongWindDetectLogo = SDMS_Building.Properties.Resources.poi_strongWind_detect;
        private Image m_imgTerrorDetectLogo = SDMS_Building.Properties.Resources.poi_terror_detect;
        private Image m_imgSubMergencyDetectLogo = SDMS_Building.Properties.Resources.poi_submergency_detect;
        private Image m_imgBlackoutDetectLogo = SDMS_Building.Properties.Resources.poi_blackout_detect;
        private Image m_imgCoronaDetectLogo = SDMS_Building.Properties.Resources.poi_corona_detect;
        private Image m_imgEarthquakeLogo = SDMS_Building.Properties.Resources.poi_earthquake_detect;

        public void AddSensorDectect(ProcessIF proc, bool bAddSelect = true, bool bCallSelect = true)
        {
            if (m_SensorDectects.Contains(proc))
                return;

            // 지진 신호는 SDMS에서 표현하지 않음.
            //if (proc.ProcessType == ProcessType.EarthquakeAlarm)
            //    return;

            ClosePopup();

            m_SensorDectects.Add(proc);
            LoadZoneTreeView();
            lblDetectCount.Text = "( " + m_SensorDectects.Count + " )";
            
            //m_bSound = true;
            btnSound_Click(null, null);

            m_contentManager.ContentForm.VisibleViewButton("btnSlideRight", true);

            if (/*!m_bPnRightOpen*/pnRight.Location.X >= this.Width)
            {
                m_bPnRightOpen = true;
                pnRight.Location = new Point(this.Width - pnRight.Width - 15, pnRight.Location.Y);
                ResizeBody();
            }

            IFacility.FacilityType alarmFacilityType = IFacility.FacilityType.NONE;

            Image imgCurrentDisasterLogo = null;
            string strType = "";
            if (proc.ProcessType == ProcessType.FireAlarm)
            {
                imgCurrentDisasterLogo = m_imgFireDetectLogo;
                strType = Data.CommonString.POI_Fire_Kor;
                alarmFacilityType = IFacility.FacilityType.FIRE_SENSOR;
            }
            else if (proc.ProcessType == ProcessType.PSMAlarm)
            {
                imgCurrentDisasterLogo = m_imgPSMDetectLogo;
                if (UnE.SOP.ProxySOP.Instance.SiteID == 201)
                    strType = Data.CommonString.POI_Gas_Kor;
                else
                    strType = "누출";

                alarmFacilityType = IFacility.FacilityType.PSM_SENSOR;
            }
            else if (proc.ProcessType == ProcessType.StrongWindAlarm)
            {
                imgCurrentDisasterLogo = m_imgStrongWindDetectLogo;
                strType = Data.CommonString.POI_StrongWind_Kor;

                alarmFacilityType = IFacility.FacilityType.STRONG_WIND;
            }
            else if (proc.ProcessType == ProcessType.TerrorAlarm)
            {
                imgCurrentDisasterLogo = m_imgTerrorDetectLogo;
                strType = Data.CommonString.POI_Terror_Kor;
                alarmFacilityType = IFacility.FacilityType.TERROR;
            }
            else if (proc.ProcessType == ProcessType.SubmergencyAlarm)
            {
                imgCurrentDisasterLogo = m_imgSubMergencyDetectLogo;
                strType = Data.CommonString.POI_Submergency_Kor;
                alarmFacilityType = IFacility.FacilityType.SUBMERGENCY;
            }
            else if (proc.ProcessType == ProcessType.BlackoutAlarm)
            {
                imgCurrentDisasterLogo = m_imgBlackoutDetectLogo;
                strType = Data.CommonString.POI_Blackout_Kor;
                alarmFacilityType = IFacility.FacilityType.BLACKOUT;
            }
            else if (proc.ProcessType == ProcessType.CoronaAlarm)
            {
                imgCurrentDisasterLogo = m_imgCoronaDetectLogo;
                strType = Data.CommonString.POI_Corona_Kor;
                alarmFacilityType = IFacility.FacilityType.CORONA;
            }
            else if (proc.ProcessType == ProcessType.EarthquakeAlarm)
            {
                imgCurrentDisasterLogo = m_imgEarthquakeLogo;
                strType = Data.CommonString.POI_Earthquake_Kor;
                alarmFacilityType = IFacility.FacilityType.Earthquake;
            }

            string strLocation = "모든 건물";
            string strDate = proc.DetectTime.ToString("yy.MM.dd");
            // 강풍 수동 신고는 Zone이 없음
            if (proc.TargetZone != null)
            {
                if (proc.TargetZone.Building == null) // 정전
                {
                    strLocation = proc.TargetZone.ToString();
                }
                else
                {
                    string strFloor = "";
                    if (proc.TargetZone.FloorIndex < 0)
                        strFloor = "B" + Math.Abs(proc.TargetZone.FloorIndex);
                    else
                        strFloor = (proc.TargetZone.FloorIndex + 1) + "F";

                    strLocation = proc.TargetZone.Building.BuildingName + " " + strFloor;
                }
            }

            Image imgLevel = null;
            string strLevel = "";

            // 사용자가 알람 단계를 선택해서 발생한 수동 신고 알람 (강풍, 침수, 테러)
            if (proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.FIRE_SENSOR ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.PSM_SENSOR ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.STRONG_WIND ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.SUBMERGENCY ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.TERROR ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.CORONA ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.BLACKOUT ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.Earthquake)
            {
                switch (proc.AlarmLevel)
                {
                    case 1:
                        imgLevel = m_imgAlarmLevel1;
                        strLevel = "관심";
                        break;
                    case 2:
                        imgLevel = m_imgAlarmLevel2;
                        strLevel = "주의";
                        break;
                    case 3:
                        imgLevel = m_imgAlarmLevel3;
                        strLevel = "경계";
                        break;
                    case 4:
                        imgLevel = m_imgAlarmLevel4;
                        strLevel = "심각";
                        break;
                }
            }
            else
                SetAlarmLevel(proc, ref imgLevel, ref strLevel);

            dgvDetectList.SelectionChanged -= dgvDetectList_SelectionChanged;
            DataGridViewRow row = (DataGridViewRow)dgvDetectList.RowTemplate.Clone();
            row.CreateCells(dgvDetectList, strDate, strType, strLocation, strLevel);
            row.Tag = proc;
            dgvDetectList.Rows.Insert(0, row);            
            
            bool bDisplay = false;
            // 총괄관리자(리테일) 이거나
            // 담당 건물의 신호이거나
            // 신호가 하나도 없을 때
            if (proc.TargetZone == null || (proc.TargetZone.Building != null && proc.TargetZone.Building.ID == m_nBuildingInCharge) || m_nBuildingInCharge == -1 || m_SensorDectects.Count == 1)
                bDisplay = true;

            if (proc.TargetZone == null || proc.TargetZone.Building == null)
                bDisplay = true;
            
            if (bDisplay)
            {
                pnDisaster.Tag = proc;
                
                lblDisasterType.Text = strType;
                picDisasterLogo.Image = imgCurrentDisasterLogo;
                lblDisasterLocation.Text = strLocation;
                lblDisasterDate.Text = proc.DetectTime.ToString("HH:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                picAlarmLevel.Image = imgLevel;

                dgvDetectList.Rows[0].Selected = true;


                m_systemInput = true;
                if (proc.TargetZone.Building == null)
                {                    
                    MoveZone(true, null);                    
                }
                else
                {
                    SetZone(proc.TargetZone);                    
                }
                m_systemInput = false;

                SetCurrentDisaster(proc);

                LoadPOIs();

                if (proc.TargetZone != null)
                {
                    if (m_selectedBuilding != proc.TargetZone.Building || m_selectedFloor != proc.TargetZone.Floor)
                    {
                        string strVolume, strSceneName;

                        if (m_optionMgr.UseEquipZoneVolume)
                        {
                            m_contentManager.ContentForm.HideAllAlarmZones();

                            if (m_optionMgr.DicZoneScene.TryGetValue(proc.TargetZone.LinkedZone.ID, out strSceneName))
                            {
                                m_contentManager.ContentForm.SelectScene(strSceneName);
                            }

                            if (m_optionMgr.DicEquiZoneVolume.TryGetValue(proc.TargetZone.ID, out strVolume))
                            {
                                //m_contentManager.ContentForm.ShowAlarmZone(strVolume, true);
                                m_contentManager.ContentForm.ZoomBuilding(strVolume);
                            }
                        } 
                    }
                }
                else
                {
                    m_contentManager.ContentForm.SelectScene("Outdoor");
                } 

                // CCTV 띄우기
                SDMSPopupFactory.Instance.CloseAll();
                EquipmentZone eqZone = ZoneManager.Instance.GetEquipZone(proc.TargetSensor.EquipZoneID);
                ArrayList arrCCTVs = GetEquipZoneCCTVList(eqZone);
                if (arrCCTVs != null)
                {
                    for (int i = 0; i < arrCCTVs.Count; i++)
                    {
                        CCTV cctv = arrCCTVs[i] as CCTV;

                        if (cctv != null)
                        {
                            UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                            if (view != null)
                            {
                                if (cctv.POI.Popup != null)
                                {
                                    Size viewSize = TooltipCCTVCtrl2.CctvPopupSize;
                                    int empty = 5;
                                    if (i == 0)
                                        cctv.POI.Popup.Show(empty, panelBody.Height - viewSize.Height - empty);
                                    else if (i == 1)
                                        cctv.POI.Popup.Show((empty * 2) + viewSize.Width, panelBody.Height - viewSize.Height - empty);
                                    else if (i == 2)
                                        cctv.POI.Popup.Show((empty * 3) + (viewSize.Width * 2), panelBody.Height - viewSize.Height - empty);
                                    else if (i == 3)
                                        cctv.POI.Popup.Show((empty * 4) + (viewSize.Width * 3), panelBody.Height - viewSize.Height - empty);
                                }
                            }
                        }
                    }
                } 

                if (proc.ProcessType == ProcessType.FireAlarm)
                {
                    SelectToTreeView(proc.TargetZone.LinkedZone.ID);                    
                }
            }

            // 리포트 업데이트
            m_ufrmReport.Display(alarmFacilityType);

            if (!m_timerCloseDoor.Enabled)
                m_timerCloseDoor.Enabled = true;
            dgvDetectList.SelectionChanged += dgvDetectList_SelectionChanged;
        }

        private void SetZone(EquipmentZone zone)
        {
            if (m_selectedBuilding == zone.Building && m_selectedFloor == zone.Floor)
                return;

            MoveZone(false, zone.LinkedZone);
        }

        private ArrayList GetEquipZoneCCTVList(EquipmentZone equipZone)
        {
            ArrayList arr = new ArrayList();

            if (equipZone == null)
                return null;

            CCTV[] arrCCTVs = CCTVManager.Instance.GetCCTVArray(equipZone);

            if (arrCCTVs == null)
            {
            }
            else
            {
                for (int i = 0; i < arrCCTVs.Length; i++)
                {
                    arr.Add(arrCCTVs[i]);
                }
            }

            return arr;
        }

        private bool m_bManualReport = false; // 현재 신호가 수동 신고 인가 ?
        private void SetCurrentDisaster(ProcessIF proc)
        {
            if (proc == null)
                return;

            pnDisaster.Tag = proc;

            string strType = "";
            if (proc.ProcessType == ProcessType.FireAlarm)
            {
                picDisasterLogo.Image = m_imgFireDetectLogo;
                strType = lblDisasterType.Text = Data.CommonString.POI_Fire_Kor;
            }
            else if (proc.ProcessType == ProcessType.PSMAlarm)
            {
                picDisasterLogo.Image = m_imgPSMDetectLogo;
                if (UnE.SOP.ProxySOP.Instance.SiteID == 201)
                    strType = lblDisasterType.Text = Data.CommonString.POI_Gas_Kor;
                else
                    strType = lblDisasterType.Text = "누출";
            }
            else if (proc.ProcessType == ProcessType.StrongWindAlarm)
            {
                picDisasterLogo.Image = m_imgStrongWindDetectLogo;
                strType = lblDisasterType.Text = Data.CommonString.POI_StrongWind_Kor;                
            }
            else if (proc.ProcessType == ProcessType.TerrorAlarm)
            {
                picDisasterLogo.Image = m_imgTerrorDetectLogo;
                strType = lblDisasterType.Text = Data.CommonString.POI_Terror_Kor;
            }
            else if (proc.ProcessType == ProcessType.SubmergencyAlarm)
            {
                picDisasterLogo.Image = m_imgSubMergencyDetectLogo;
                strType = lblDisasterType.Text = Data.CommonString.POI_Submergency_Kor;
            }
            else if (proc.ProcessType == ProcessType.BlackoutAlarm)
            {
                picDisasterLogo.Image = m_imgBlackoutDetectLogo;
                strType = lblDisasterType.Text = Data.CommonString.POI_Blackout_Kor;
            }
            else if (proc.ProcessType == ProcessType.CoronaAlarm)
            {
                picDisasterLogo.Image = m_imgCoronaDetectLogo;
                strType = lblDisasterType.Text = Data.CommonString.POI_Corona_Kor;
            }
            else if (proc.ProcessType == ProcessType.EarthquakeAlarm)
            {
                picDisasterLogo.Image = m_imgEarthquakeLogo;
                strType = Data.CommonString.POI_Earthquake_Kor;
            }

            string strLocation = "";
            string strZoneDisplayText = "모든 건물";

            // 강풍 수동 신고는 Zone이 없음
            if (proc.TargetZone != null)
            {
                if (proc.TargetZone.Building == null) // 정전
                {
                    strLocation = proc.TargetZone.ToString();
                }
                else
                {
                    string strFloor = "";
                    if (proc.TargetZone.FloorIndex < 0)
                        strFloor = "B" + Math.Abs(proc.TargetZone.FloorIndex);
                    else
                        strFloor = (proc.TargetZone.FloorIndex + 1) + "F";

                    strLocation = proc.TargetZone.Building.BuildingName + " " + strFloor;
                }
                strZoneDisplayText = proc.TargetZone.DisplayText;
            }

            lblDisasterDate.Text = proc.DetectTime.ToString("HH:mm tt", System.Globalization.CultureInfo.InvariantCulture);
            lblDisasterLocation.Text = strLocation;// + " " + strZoneDisplayText;

            Image imgLevel = null;
            string strLevel = "";
            // 사용자가 알람 단계를 선택해서 발생한 수동 신고 알람 (강풍, 침수, 테러)
            if (proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.FIRE_SENSOR ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.PSM_SENSOR ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.STRONG_WIND ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.SUBMERGENCY ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.TERROR ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.CORONA ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.BLACKOUT ||
                proc.DetectSensorID == SOPWebServer.Header.ManualReportDefaultID + (int)IFacility.FacilityType.Earthquake)
            {
                switch (proc.AlarmLevel)
                {
                    case 1:
                        imgLevel = m_imgAlarmLevel1;
                        strLevel = "관심";
                        break;
                    case 2:
                        imgLevel = m_imgAlarmLevel2;
                        strLevel = "주의";
                        break;
                    case 3:
                        imgLevel = m_imgAlarmLevel3;
                        strLevel = "경계";
                        break;
                    case 4:
                        imgLevel = m_imgAlarmLevel4;
                        strLevel = "심각";
                        break;
                }
            }
            else
                SetAlarmLevel(proc, ref imgLevel, ref strLevel);

            picAlarmLevel.Image = imgLevel;

            if (!picSensorMonitor.Visible)
                picSensorMonitor.Visible = true;

            lblDisasterInfo.Text = proc.LastLog.Message;

            if (proc.DetectSensorID >= SOPWebServer.Header.ManualReportDefaultID)
            {
                btnMalfunction.Text = "상황 종료";

                // 수동 신고가 아닐때만 출입문 미개폐를 표현한다
                if (picDoorInfo.Visible)
                {
                    picDoorInfo.Visible = false;
                    dgvOpenDoor.Visible = false;
                }

                m_bManualReport = true;

                label5.Text = "신고자 : " + GetSOPGenUser(proc.LastLog.Parameter3) + "\r\n메모 : " + GetLogMemo(proc.SensorHistoryID);

            }
            else
            {
                btnMalfunction.Text = "오작동";

                // 수동 신고가 아닐때만 출입문 미개폐를 표현한다
                if (!picDoorInfo.Visible)
                {
                    if (proc.ProcessType == ProcessType.FireAlarm)
                    {
                        picDoorInfo.Visible = true;
                        dgvOpenDoor.Visible = true; 
                    }
                }

                m_bManualReport = false;

                label5.Text = "";
            }
        }

        public string GetSOPGenUser(string memberID)
        {
            string strSQL = string.Format("select NickName From SOPGenUser WHERE SiteID = {0} AND ID = {1}", UnE.SOP.ProxySOP.Instance.SiteID, memberID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strNicName = WebDBManager.GetStringField(arrResult[0], "");

            return strNicName;
        }

        public string GetLogMemo(int nSensorZoneHistoryID)
        {
            string strSQL = string.Format("select DescriptionText From SensorReactionHistoryDescription WHERE SensorZoneHistoryID = {0}", nSensorZoneHistoryID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strNicName = WebDBManager.GetStringField(arrResult[0], "");

            return strNicName;
        }

        public void RemoveSensorDetect(ProcessIF process, bool bNextSelect = true)
        {
            int deleteRow = -1;
            foreach (DataGridViewRow row in dgvDetectList.Rows)
            {
                if (row.Tag == process)
                {
                    deleteRow = row.Index;
                    break;
                }
            }

            m_dicAlarmStep.Remove(process.TargetSensor.ID);

            SDMSPopupFactory.Instance.CloseAll();
            Panel4Unity panel = (Panel4Unity)m_contentManager.ContentForm.OutdoorView;
            panel.RollBackPOIIcon("");

            DataManager.SetPOIIcon(process.TargetSensor);
            if (process.TargetZone.Floor != null && process.TargetZone.Floor.Zone != null)
                RollbackCloseDoor(process.TargetZone.Floor.Zone.ID);

            if (deleteRow >= 0)
            {
                m_SensorDectects.Remove(process);
                LoadZoneTreeView();

                lblDetectCount.Text = "( " + m_SensorDectects.Count + " )";
                dgvDetectList.Rows.RemoveAt(deleteRow);
            }

            System.Diagnostics.Debug.WriteLine(process);
            int nCount = dgvDetectList.Rows.Count;

            if (nCount > 0 && bNextSelect == true)
            {
                ProcessIF processSelect = (ProcessIF)dgvDetectList.CurrentRow.Tag;
                if (processSelect != null)
                {
                    bool bSelected = processSelect.Select();
                    if (bSelected)
                    {
                        ReactionLogManager.Instance.ProcessLog(processSelect.LastLog, true);
                    }

                    Image imgLevel = null;
                    string strLevel = "";
                    SetAlarmLevel(processSelect, ref imgLevel, ref strLevel);
                }
            }
            else
            {
                m_timerCloseDoor.Enabled = false;

                if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
                {
                    // (신호가 현재 보여주고 있는 2d view에 있는 신호인지 여부 아니라면 change view 해야함);
                    m_contentManager.ContentForm.IsSameCampus(process.TargetZone.Building.BuildingGroup);
                }

                pnDisaster.Tag = null; // 재난 알람 영역
                picSensorMonitor.Visible = false;
                m_contentManager.ContentForm.VisibleViewButton("btnSlideRight", false);
                if (m_bPnRightOpen)
                    SetRightPanelSlide();

                SetNormalMode();

                string strSceneName;

                //Outdoor

                if (m_bGoOutside)
                {
                    //m_contentManager.ContentForm.SelectScene("Outdoor");
                    m_systemInput = true;
                    btnOutdoor_Click(null, null);
                    m_systemInput = false;
                }
                else
                {
                    m_contentManager.ContentForm.HideAllAlarmZones();
                }
            }
        }

        public void SelectSensorDetectProcess(int nSensorHistoryID, int nSensorID)
        {

        }
        public void SelectLastFireDectectProcess()
        {
        }

        public void SendDetectMessageToSOPSimulator()
        {
            //Debug.WriteLine("Run Simulator");
            //bool bRun = FormSMSConfig.ReadRunSimulator();
            //if (bRun == true)
            //{
            //    if (m_proxyMessenger != null)
            //        m_proxyMessenger.RunSOPSimulator();
            //}
        }

        public void Update3DView()
        {
            m_contentManager.ContentForm.Invalidate3DView(false);
        }

        private Zone SetDetectZoneName(libSensorProcess.ReactionLog log)
        {
            int nHistoryID = log.SensorHistoryID;
            if (nHistoryID == -1)
            {
                return null;
            }

            // TODO: 알람 영역 채우기
            /*
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
                           //mLabelZone.Text = text1 + "," + szZoneName;
                           //mLabelZone.Location = new Point(panelStatus.Width - mLabelZone.Width - 10, 10);
                           return zone.LinkedZone;
                       }
                       else if (sensor.Type == IFacility.FacilityType.SecomExternalAlarmBell)  //외부 비상벨은 zone.Building이 없으므로...by hypark
                       {
                           //string text1 = zone.Building.BuildingGroup.BuildingGroupName;
                           string szZoneName = zone.LinkedZone == null ? zone.ZoneName : zone.LinkedZone.DisplayText;
                           //mLabelZone.Text = szZoneName;
                           //mLabelZone.Location = new Point(panelStatus.Width - mLabelZone.Width - 10, 10);
                           return zone.LinkedZone;                         //외부 비상벨 타겟팅을 위해서 꼭 넘겨줘야 한다. by hypark.
                       }
                   }
                   else
                   {
                       //mLabelZone.Text = "";
                   }
               }
               else
               {
                   //mLabelZone.Text = "";
               }
           }
           else
           {
               //mLabelZone.Text = "";
           }
           */
            return null;
        }

        public void CustomGridView(DataGridView dgv, float fontSize, Color foreColor, Color defaultBackColor, Color selectionBackColor, Color selectionForeColor, DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleCenter)
        {
            dgv.AllowUserToAddRows = false;
            dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle1.Alignment = align;
            //dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("나눔바른고딕", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle1.ForeColor = foreColor;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgv.ColumnHeadersHeight = 50;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle2.Alignment = align;
            //dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("나눔바른고딕", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle2.ForeColor = foreColor;
            dataGridViewCellStyle2.SelectionBackColor = selectionBackColor;
            dataGridViewCellStyle2.SelectionForeColor = selectionForeColor;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            dgv.DefaultCellStyle = dataGridViewCellStyle2;
            dgv.RowHeadersVisible = false;
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            dataGridViewCellStyle3.Alignment = align;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("나눔바른고딕", fontSize, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            dataGridViewCellStyle3.ForeColor = foreColor;
            dataGridViewCellStyle3.SelectionBackColor = selectionBackColor;
            dataGridViewCellStyle3.SelectionForeColor = selectionForeColor;
            dgv.RowsDefaultCellStyle = dataGridViewCellStyle3;
            dgv.ScrollBars = System.Windows.Forms.ScrollBars.None;
            dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            dgv.BackgroundColor = defaultBackColor;
            dgv.RowTemplate.DefaultCellStyle.BackColor = defaultBackColor;
        }

        #region IProcessOwner 인터페이스
        public bool UsePopupSensorOn { get { return false; } } // 사용 안함

        public bool OpenSOPOnDetectSensor { get { return UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect; } }

        public void OpenSOP(EquipmentZone equipZone, DateTime sopTime, ProcessIF process)
        {
            // 사용 안함
        }

        public void AddSensorDectectInvoke(ProcessIF process, bool bAddSelect, bool bCallSelect)
        {
            this.Invoke((MethodInvoker)delegate
            {
                AddSensorDectect(process, bAddSelect, bCallSelect);
            });
        }

        public void ShowEvacCircleInvoke(int nLevel)
        {
            this.Invoke((MethodInvoker)delegate
            {
                m_contentManager.ContentForm.ShowEvacCircle(nLevel);
            });
        }

        /// <summary>
        /// 새로운 센서신호가 탐지되었음을 알린다.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="notifyType"></param>
        public void ShowSensorAlarmInvoke(ProcessIF process, ReactionType notifyType)
        {
            if (process == null)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                ISensorTooltipOwner view = null;
                //m_PageHome.FireDetect(process.TargetSensor, process.TargetZone, process.SensorHistoryID);

                try
                {
                    if (this.WindowState != FormWindowState.Maximized)
                    {
                        //FormFrame.Instance.WindowState = FormWindowState.Maximized;
                        this.Activate();
                        this.Focus();
                    }
                }
                catch (System.Exception)
                {
                }

                // TODO: 새로운 신호 처리
                //SeletCaseData form = new SeletCaseData(process.ProcessType, view, process.TargetSensor, process.SensorHistoryID, process.ShowOpenSOP, process.DetectTime);
                //ConfirmDialogManager.Instance.AddDialogFirst(form);

                //if (process.LastLog != null && process.LastLog.ReactionType != (int)notifyType)
                //    ConfirmDialogManager.Instance.ShowDialogNext();

                FormMain.Instance.Update3DView();
            });
        }

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
                m_contentManager.ContentForm.PushViewState(true);

                if (UnE.SOP.ProxySOP.Instance.SiteID == 102)
                {
                    // (신호가 현재 보여주고 있는 2d view에 있는 신호인지 여부 아니라면 change view 해야함);
                    m_contentManager.ContentForm.IsSameCampus(process.TargetZone.Building.BuildingGroup);
                }
                m_contentManager.ContentForm.HideZoneVolume();

                //if (process.ProcessType != ProcessType.PSMAlarm)
                //    m_contentManager.ContentForm.HideEvacCircle();

                if (process.TargetZone == null || process.TargetZone.Building == null)
                {

                }
                else
                {
                    BuildingGroup grp = process.TargetZone.Building.BuildingGroup;
                    Building building = process.TargetZone.Building;

                    //if (process.TargetZone.LinkedZone != null)
                    //    this.SetFloorStatus(grp, building, (process.TargetZone.LinkedZone));

                    if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                    {
                        this.ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);
                    }
                }

                if (process.TargetSensor != null && process.TargetSensor.Type == IFacility.FacilityType.ExternalAlarmBell || process.TargetSensor.Type == IFacility.FacilityType.SecomExternalAlarmBell)
                {
                    m_contentManager.ContentForm.ShowEmPoll(nSensorZoneID);
                    m_contentManager.ContentForm.ZoomBuilding("EMPOLL_" + nSensorZoneID);
                }
                else
                {
                    if (process.TargetZone != null && process.TargetZone.Building != null && process.TargetZone.Building.BuildingID != "yhNONE")
                    {
                        if (UnE.SOP.ProxySOP.Instance.SiteID == 999 || UnE.SOP.ProxySOP.Instance.SiteID == 102)
                        {
                            if (process.TargetZone.LinkedZone != null)
                                m_contentManager.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, false, true);
                        }
                        else
                        {
                            string szName = process.TargetZone.Building.BuildingID;

                            m_contentManager.ContentForm.ZoomBuilding(szName);

                            if (process.TargetZone.LinkedZone != null)
                            {
                                //m_contentManager.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.LinkedZone.ID, true, true);
                                //m_contentManager.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, false, true);
                            }
                        }
                    }
                    else
                    {
                        if (process.TargetZone != null && process.TargetZone.Polygon != null)
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
                                //m_contentManager.ContentForm.ZoomTarget(x, y, z, false);
                                //m_contentManager.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, true, true);
                            }
                        }
                    }
                }

                //if (arrCCTVs != null)
                //{
                //    foreach (CCTV cctv in arrCCTVs)
                //    {
                //        if (process.TargetZone.LinkedZone != null)
                //        {
                //            if (cctv.POI.Zone == process.TargetZone.LinkedZone && process.TargetZone.IsOutdoor == false)
                //            {
                //                if (cctv.POI.ViewType == 1)
                //                {
                //                    UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                //                    if (view != null)
                //                    {
                //                        System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                //                        if (cctv.POI.Popup != null)
                //                        {
                //                            if (showDetectSensorTooltipCCTV)
                //                                cctv.POI.Popup.Show(p.X, p.Y);
                //                        }
                //                    }

                //                }
                //                else
                //                {
                //                    UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                //                    if (view != null)
                //                    {
                //                        System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                //                        if (cctv.POI.Popup != null)
                //                        {
                //                            if (showDetectSensorTooltipCCTV)
                //                                cctv.POI.Popup.Show(p.X, p.Y);
                //                        }
                //                    }

                //                }
                //            }
                //            else
                //            {
                //                if (cctv.POI.IsIndoor == false)
                //                {
                //                    UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                //                    if (view != null)
                //                    {
                //                        System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                //                        if (cctv.POI.Popup != null)
                //                        {
                //                            if (showDetectSensorTooltipCCTV)
                //                                cctv.POI.Popup.Show(p.X, p.Y);
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    if (process.TargetZone != null && process.TargetZone.LinkedZone != null)
                //    {
                //        if (process.TargetZone != null && process.TargetZone.LinkedZone != null)
                //        {
                //            //this.PageHome.ShowBigCCTV(process.TargetZone.LinkedZone, nSituation, true);
                //            //this.SelectCCTVTab(false);
                //        }
                //    }
                //}

                if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                {
                    if (process.TargetZone != null && process.TargetZone.LinkedZone != null)
                    {
                        //this.CCTVPipe.Send("SetHistoryID(" + process.SensorHistoryID + ")");
                        //this.PageHome.ShowBigCCTV(process.TargetZone.LinkedZone, nSituation, true);
                        //this.SelectCCTVTab(false);
                    }
                }

                this.Update3DView();
            });
        }

        /// <summary>
        /// 탐지된 센서신호를 실제 재난상황으로 판단한다.
        /// </summary>
        /// <returns>CCTV List</returns>
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
                    m_contentManager.ContentForm.PushViewState(true);
                    if (process.ProcessType != ProcessType.EarthquakeAlarm)
                        ProcessManager.PlaySound();

                    m_contentManager.ContentForm.HideZoneVolume();

                    ChangeTab(ContentOwnerTab.M3D_TAB);
                    //this.SelectMonitoringTab();

                    //BuildingGroup grp = process.TargetZone.Building.BuildingGroup;
                    //Building building = process.TargetZone.Building;

                    //if (process.TargetZone.LinkedZone != null)
                    //    this.SetFloorStatus(grp, building, process.TargetZone.LinkedZone);

                    //if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                    //{
                    //    this.ChangeTab(UnE.View.Content.ContentOwnerTab.M3D_TAB);
                    //}

                    m_contentManager.ContentForm.HideAllPOIPopup();

                    arrCCTVs = CCTVManager.Instance.AutoPopupCCTV(process.TargetZone.LinkedZone);

                    //foreach (CCTV cctv in arrCCTVs)
                    //{
                    //    if (cctv.POI.Zone == process.TargetZone.LinkedZone && process.TargetZone.IsOutdoor == false)
                    //    {
                    //        if (cctv.POI != null && cctv.POI.Popup != null)
                    //        {
                    //            UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                    //            System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);

                    //            if (showDetectSensorTooltipCCTV)
                    //                cctv.POI.Popup.Show(p.X, p.Y);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        if (cctv.POI.IsIndoor == false)
                    //        {
                    //            UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                    //            if (view != null && cctv.POI != null)
                    //            {
                    //                System.Drawing.Point p = view.GetPosition2D(cctv.POI.ID, cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
                    //                if (cctv.POI.Popup != null)
                    //                {
                    //                    if (showDetectSensorTooltipCCTV)
                    //                        cctv.POI.Popup.Show(p.X, p.Y);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    if (process.TargetZone.Building != null && process.TargetZone.Building.BuildingID != "yhNONE")
                    {
                        if (process.TargetSensor != null && process.TargetSensor.Type == IFacility.FacilityType.ExternalAlarmBell || process.TargetSensor.Type == IFacility.FacilityType.SecomExternalAlarmBell)
                        {
                            m_contentManager.ContentForm.ShowEmPoll(nSensorZoneID);
                            m_contentManager.ContentForm.ZoomBuilding("EMPOLL_" + nSensorZoneID);
                        }
                        else
                        {
                            string szName = process.TargetZone.Building.BuildingID;

                            m_contentManager.ContentForm.ZoomBuilding(szName);

                            if (process.TargetZone.LinkedZone != null)
                            {
                                //string strVolume, strSceneName;

                                //if (m_useEquipZoneVolume && m_dicEquiZoneVolume.TryGetValue(process.TargetZone.ID, out strVolume) && m_dicZoneScene.TryGetValue(process.TargetZone.LinkedZone.ID, out strSceneName))
                                //{
                                //    m_contentManager.ContentForm.HideAllAlarmZones();
                                //    m_contentManager.ContentForm.SelectScene(strSceneName);
                                //    m_contentManager.ContentForm.ShowAlarmZone(strVolume, true);
                                //}
                                //else
                                {
                                    //m_contentManager.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.LinkedZone.ID, true, true);
                                    //m_contentManager.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, false, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (process.TargetSensor != null && process.TargetSensor.Type == IFacility.FacilityType.ExternalAlarmBell || process.TargetSensor.Type == IFacility.FacilityType.SecomExternalAlarmBell)
                        {
                            m_contentManager.ContentForm.ShowEmPoll(nSensorZoneID);
                            m_contentManager.ContentForm.ZoomBuilding("EMPOLL_" + nSensorZoneID);
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
                                    m_contentManager.ContentForm.ZoomTarget(x, y, z, false);
                                    m_contentManager.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, true, true);
                                }
                            }
                            else if (UnE.SOP.ProxySOP.Instance.SiteID == 100)
                            {
                                float x = (float)pos.x;
                                float y = 2.0f;
                                float z = (float)pos.y;

                                if (process.TargetZone.LinkedZone != null)
                                {
                                    m_contentManager.ContentForm.ZoomTarget(x, y, z, false);
                                    m_contentManager.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, true, true);
                                }
                            }
                            else
                            {
                                float x = (float)pos.x - dx;
                                float y = 0.0f;
                                float z = dy - (float)pos.y;

                                if (process.TargetZone.LinkedZone != null)
                                {
                                    m_contentManager.ContentForm.ZoomTarget(x, y, z, false);
                                    m_contentManager.ContentForm.ShowZoneVolume(process.TargetZone.LinkedZone.ID, process.TargetZone.ID, true, true);
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

                        try
                        {
                            //if (this.MainFrame.WindowState != FormWindowState.Maximized)
                            //{
                            //    this.MainFrame.WindowState = FormWindowState.Maximized;
                            //    this.MainFrame.Activate();
                            //    this.Focus();
                            //}
                        }
                        catch (System.Exception)
                        {
                        }

                        // TODO: 탐지된 센서신호를 실제 재난상황으로 판단한다.
                        /*
                        SeletCaseData form = new SeletCaseData(process.ProcessType, view, process.TargetSensor, process.SensorHistoryID, process.ShowOpenSOP, process.DetectTime);
                        ConfirmDialogManager.Instance.AddDialogFirst(form);

                        if (process.ProcessType == ProcessType.PSMAlarm && process.TargetZone != null)
                        {
                            m_contentManager.ContentForm.SetEvacDistance(nSensorZoneID);
                            m_contentManager.ContentForm.SetEvacCenter(process.TargetZone);
                            m_contentManager.ContentForm.ShowEvacCircle(nAlarmLevel);
                        }

                        PageBackstageHome.Instance.ShowBigCCTV(process.TargetZone, nSituation);

                        PageBackstageHome.Instance.SetTargetCCTVPreset(process.TargetSensor.EquipZoneID);

                        if (process.LastLog != null && process.LastLog.ReactionType != (int)notifyType)
                            ConfirmDialogManager.Instance.ShowDialogNext();
                        */

                        this.Update3DView();
                    });
                }
                catch (System.Threading.ThreadInterruptedException e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }
            }
            catch (Exception)
            {
            }

            return arrCCTVs;
        }

        public void EndNotifyProcessInvoke(libSensorProcess.ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.EndNotifyProcess(log);
            });
        }

        public void EndNotifyProcess(libSensorProcess.ReactionLog log)
        {
            int nHistoryID = log.SensorHistoryID;
            int nSensorID = SensorHistoryManager.Instance.GetSensorID(nHistoryID);
            if (nSensorID != -1)
            {
                ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
                ProcessManager.Instance.EndProcess(nSensorID);
            }

            SetNormalMode();
        }

        public void SetPSMDetectModeInvoke(libSensorProcess.ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetDetectZoneName(log);
            });
        }

        public void SetNormalModeInvoke(libSensorProcess.ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetNormalMode();
            });
        }

        /// <summary>
        /// 화재 신호 탐지 모드
        /// </summary>
        /// <param name="log"></param>
        public void SetFireDetectModeInvoke(libSensorProcess.ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetDetectZoneName(log);
            });
        }

        /// <summary>
        /// 방범 신호 탐지 모드
        /// </summary>
        /// <param name="log"></param>
        public void SetSecurityDetectModeInvoke(libSensorProcess.ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetDetectZoneName(log);
            });
        }

        public void SetEarthquakeDetectModeInvoke(libSensorProcess.ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                // 지진 신호 탐지 모드

            });
        }

        public void NotifyProcessInvoke(libSensorProcess.ReactionLog log)
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

        public void BeginNotifyProcess(libSensorProcess.ReactionLog log, ISensor sensor)
        {
            int nHistoryID = log.SensorHistoryID;

            if (log.ReactionType == (int)ReactionType.NOTIFY_SIGNAL && sensor != null)
            {
                if (IFacility.IsFireSensorType(sensor.Type))
                {
                    //if (log.Message.IndexOf("[훈련상황]") != -1)
                    //    StatusLableText = "[훈련]화재 발생";
                    //else
                    //    StatusLableText = "화재 발생";
                }
                else if (IFacility.IsPSMSensorType(sensor.Type))
                {
                    //if (log.Message.IndexOf("[훈련상황]") != -1)
                    //    StatusLableText = "[훈련]누출 발생";
                    //else
                    //    StatusLableText = "누출 발생";
                }
                else if (IFacility.IsSecurityType(sensor.Type))
                {
                    //if (log.Message.IndexOf("[훈련상황]") != -1)
                    //    StatusLableText = "[훈련]방범 상황";
                    //else
                    //    StatusLableText = "방범 상황";
                }
            }
        }

        public void RunSOPInvoke(libSensorProcess.ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetDetectZoneName(log);

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

        public void RunNCancelSOPInvoke(libSensorProcess.ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetDetectZoneName(log);

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

        public void FinishSOPInvoke(libSensorProcess.ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetDetectZoneName(log);

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

        public void IgnoreSOPInvoke(libSensorProcess.ReactionLog log)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.SetDetectZoneName(log);

                int nHistoryID = log.SensorHistoryID;
                //ProxyMessenger.IgnoreSOP(nHistoryID);

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

        public void AddLogMessageInvoke(libSensorProcess.ReactionLog log)
        {

        }
        #endregion

        private void dgvDetectList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDetectList.SelectedRows == null || dgvDetectList.SelectedRows.Count == 0)
                return;

            ProcessIF process = (ProcessIF)dgvDetectList.SelectedRows[0].Tag;
            if (process != null)
            {

                m_systemInput = true;
                if (process.TargetZone.Building == null)
                {
                    MoveZone(true, null);
                }
                else
                {
                    SetZone(process.TargetZone);
                }
                m_systemInput = false;

                //if (m_contentManager.ContentForm.OutdoorView is Panel4Unity)
                //{
                //    Panel4Unity panel = (Panel4Unity)m_contentManager.ContentForm.OutdoorView;
                //    panel.RollBackPOIIcon("");
                //}

                //LoadPOIs();

                SetCurrentDisaster(process);

                int nSensorID = process.DetectSensorID;
                int nHistoryID = process.SensorHistoryID;
                if (process.TargetZone != null )
                {   
                    if (m_selectedBuilding != process.TargetZone.Building || m_selectedFloor != process.TargetZone.Floor)
                    {
                        string strVolume, strSceneName;
                        if (m_optionMgr.UseEquipZoneVolume)
                        {
                            m_contentManager.ContentForm.HideAllAlarmZones();

                            if (m_optionMgr.DicZoneScene.TryGetValue(process.TargetZone.LinkedZone.ID, out strSceneName))
                            {
                                m_contentManager.ContentForm.SelectScene(strSceneName);
                            }

                            if (m_optionMgr.DicEquiZoneVolume.TryGetValue(process.TargetZone.ID, out strVolume))
                            {
                                //m_contentManager.ContentForm.ShowAlarmZone(strVolume, true);
                                m_contentManager.ContentForm.ZoomBuilding(strVolume);
                            }
                        } 
                    }
                    //if (m_optionMgr.UseEquipZoneVolume && m_optionMgr.DicEquiZoneVolume.TryGetValue(process.TargetZone.ID, out strVolume) && m_optionMgr.DicZoneScene.TryGetValue(process.TargetZone.LinkedZone.ID, out strSceneName))
                    //{
                    //    m_contentManager.ContentForm.HideAllAlarmZones();
                    //    m_contentManager.ContentForm.SelectScene(strSceneName);
                    //    m_contentManager.ContentForm.ShowAlarmZone(strVolume, true);
                    //    m_contentManager.ContentForm.ZoomBuilding(strVolume);
                    //}

                    SDMSPopupFactory.Instance.CloseAll();
                    EquipmentZone eqZone = ZoneManager.Instance.GetEquipZone(process.TargetSensor.EquipZoneID);
                    ArrayList arrCCTVs = GetEquipZoneCCTVList(eqZone);
                    if (arrCCTVs != null)
                    {
                        for (int i = 0; i < arrCCTVs.Count; i++)
                        {
                            CCTV cctv = arrCCTVs[i] as CCTV;

                            if (cctv != null)
                            {
                                UnE.View.Content.IBaseView view = (UnE.View.Content.IBaseView)cctv.POI.ParentView;
                                if (view != null)
                                {
                                    if (cctv.POI.Popup != null)
                                    {
                                        Size viewSize = TooltipCCTVCtrl2.CctvPopupSize;
                                        int empty = 7;
                                        if (i == 0)
                                            cctv.POI.Popup.Show(empty, empty);
                                        else if (i == 1)
                                            cctv.POI.Popup.Show(empty, panelBody.Height - viewSize.Height - empty);
                                            //cctv.POI.Popup.Show(panelBody.Width - viewSize.Width - empty, empty);
                                        else if (i == 2)
                                            cctv.POI.Popup.Show(panelBody.Width - viewSize.Width - empty, empty);
                                            //cctv.POI.Popup.Show(empty, panelBody.Height - viewSize.Height - empty);
                                        else if (i == 3)
                                            cctv.POI.Popup.Show(panelBody.Width - viewSize.Width - empty, panelBody.Height - viewSize.Height - empty);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    m_contentManager.ContentForm.SelectScene("Outdoor");
                }

                SelectToTreeView(process.TargetZone.LinkedZone.ID);
            }
        }

        private void dgvDetectList_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dgvDetectList.SelectionChanged -= dgvDetectList_SelectionChanged;
            dgvDetectList_SelectionChanged(sender, null);
            dgvDetectList.SelectionChanged += dgvDetectList_SelectionChanged;
        }

        private void btnMalfunction_Click(object sender, EventArgs e)
        {
            ProcessIF proc = (ProcessIF)pnDisaster.Tag;
            if (proc == null)
                return;

            int nSensorID = proc.DetectSensorID;
            if (nSensorID >= SOPWebServer.Header.ManualReportDefaultID)
            {
                string msgTxt = "";
                if (proc.ProcessType == ProcessType.FireAlarm)
                    msgTxt = "화재상황을 종료하시겠습니까?";
                else
                    msgTxt = "상황을 종료하시겠습니까?";

                FormMessageBox msg = new FormMessageBox(msgTxt, MessageBoxButtons.YesNo);
                msg.StartPosition = FormStartPosition.CenterParent;
                if (msg.ShowDialog() != DialogResult.Yes)
                    return;

                //if (proc.TargetSensor == null || proc.TargetSensor.ID >= SOPWebServer.Header.ManualReportDefaultID)
                //{
                // 수동 신고
                NetworkWebManager.Instance.SendMessage(1, SOPWebServer.Header.CLEAR_DETECT_REPORT, proc.SensorHistoryID, m_nSOPGenUserID);
                //}
                //else
                //{
                //    // 센서 신호
                //    NetworkWebManager.Instance.SendMessage(1, SOPWebServer.Header.SENSOR_MALFUNCTION, proc.SensorHistoryID, proc.TargetSensor.ID, m_nSOPGenUserID, "");
                //}
            }
            else if (nSensorID > 0 && nSensorID < SOPWebServer.Header.ManualReportDefaultID)
            {
                if (proc.ProcessType == ProcessType.FireAlarm)
                {
                    FormMessageBox msg = new FormMessageBox("화재 탐지결과를 오작동으로 신고하시겠습니까?", MessageBoxButtons.YesNo);
                    msg.StartPosition = FormStartPosition.CenterParent;
                    if (msg.ShowDialog() != DialogResult.Yes)
                        return;

                    if (ReportAbnormal(proc))
                    {
                        FireDetectProcess.SoundPlayer.Stop();
                        m_bSound = false;
                    }
                    else
                    {
                        MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
                    }
                }
                else if (proc.ProcessType == ProcessType.PSMAlarm)
                {
                    FormMessageBox msg = new FormMessageBox("가스 누출 탐지결과를 오작동으로 신고하시겠습니까?", MessageBoxButtons.YesNo);
                    msg.StartPosition = FormStartPosition.CenterParent;
                    if (msg.ShowDialog() != DialogResult.Yes)
                        return;

                    if (ReportPSMReset(proc, ""))
                    {
                        FireDetectProcess.SoundPlayer.Stop();
                        m_bSound = false;
                    }
                    else
                    {
                        MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    string strType = IFacility.GetFacilityTypeString(proc.TargetSensor.Type);
                    FormMessageBox msg = new FormMessageBox(strType + "신호 탐지결과를 오작동으로 신고하시겠습니까?", MessageBoxButtons.YesNo);
                    msg.StartPosition = FormStartPosition.CenterParent;
                    if (msg.ShowDialog() != DialogResult.Yes)
                        return;

                    if (ReportAbnormal(proc))
                    {
                        FireDetectProcess.SoundPlayer.Stop();
                        m_bSound = false;
                    }
                    else
                    {
                        MessageBox.Show("서버가 연결되지 않았습니다. 잠시 후에 시도하세요", "확인", MessageBoxButtons.OK);
                    }
                }
            }
        }

        public bool ReportAbnormal(ProcessIF proc)
        {
            string strDescriptionText = "";

            int nSensorID = proc.DetectSensorID;
            ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
            if (process != null)
            {
                int nSOPGenUserID = m_nSOPGenUserID;
                // 서버로 오작동 신고를 수행한다.
                return m_netMgr.SendMessage(1, SOPWebServer.Header.SENSOR_MALFUNCTION, process.SensorHistoryID, nSensorID, nSOPGenUserID, strDescriptionText);
            }
            return false;
        }

        public bool ReportPSMReset(ProcessIF proc, string strDescriptionText)
        {
            int nSensorID = proc.DetectSensorID;
            ProcessIF process = ProcessManager.Instance.GetProcess(nSensorID);
            if (process != null)
            {

                // 서버로 오작동 신고를 수행한다.
                return m_netMgr.SendMessage(1, SOPWebServer.Header.SENSOR_USER_RESET, process.SensorHistoryID, nSensorID, m_nSOPGenUserID, strDescriptionText);
            }
            return false;
        }

        private void btnReport_Click(object sender, EventArgs e)
        {

        }

        private bool m_bSound = false;
        private void btnSound_Click(object sender, EventArgs e)
        {
            if (m_bSound)
            {
                FireDetectProcess.SoundPlayer.Stop();
                btnSound.ImageNormal = SDMS_Building.Properties.Resources.soundon_normal;
                btnSound.ImageMouseOver = SDMS_Building.Properties.Resources.soundon_hover;
                btnSound.ImageClicked = SDMS_Building.Properties.Resources.soundon_click;
            }
            else
            {
                FireDetectProcess.PlaySound();
                btnSound.ImageNormal = SDMS_Building.Properties.Resources.soundoff_normal;
                btnSound.ImageMouseOver = SDMS_Building.Properties.Resources.soundoff_hover;
                btnSound.ImageClicked = SDMS_Building.Properties.Resources.soundoff_click;
            }

            btnSound.Refresh();
            m_bSound = !m_bSound;
        }

        private void btnOutdoor_Click(object sender, EventArgs e)
        {
            MoveZone(true, null);
            //m_contentManager.ContentForm.SelectScene("Outdoor");
        }
                
        public void ShowManualReport()
        {
            PopupBackground back = new PopupBackground();
            back.StartPosition = FormStartPosition.Manual;
            back.Size = this.Size;
            back.Location = this.Location;
            back.Show();

            m_frmManualReport = new FormManualReport();
            m_frmManualReport.StartPosition = FormStartPosition.CenterParent;
            m_frmManualReport.ShowDialog();
            back.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_bExit = true;
            NetworkWebManager.Instance.ReleaseThread();

            if (pnLeftTimer != null)
                pnLeftTimer.Enabled = false;
            if (pnRightTimer != null)
                pnRightTimer.Enabled = false;

            ProcessManager.Instance.Dispose();

            CloseUnityProcess(m_contentManager.ContentForm.OutdoorView);
            CloseUnityProcess(m_contentManager.ContentForm.IndoorView);
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
        

        public void ShowPOIVisibleForm()
        {
            m_ufrmPoiVisible.Visible = true;
            m_ufrmPoiVisible.BringToFront();
        }

        public void ShowBroadcast()
        {
            m_ufrmBroadcast.Visible = true;
            m_ufrmBroadcast.BringToFront();
        }

        private void SetButtonID(ImageButton btn, int nID, string strTooltipText = "")
        {
            if (btn == null)
                return;

            btn.Tag = nID;

            if (strTooltipText.Length > 0)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(btn, strTooltipText);
            }
        }

        public void SetPOIVisible(IFacility.FacilityType type, bool visible)
        {
            if (!visible)
            {
                if (type == IFacility.FacilityType.FIRE_SENSOR)
                {
                    DataManager.ClearPOI(Data.CommonString.POI_Fire);
                    DataManager.ClearPOI(Data.CommonString.POI_Fire + Data.CommonString.AlarmTag);
                }
                else if (type == IFacility.FacilityType.CCTV)
                {
                    DataManager.ClearPOI(Data.CommonString.POI_CCTV);
                }
                else if (type == IFacility.FacilityType.DOOR)
                {
                    DataManager.ClearPOI(Data.CommonString.POI_Door);
                    DataManager.ClearPOI(Data.CommonString.POI_Fire + Data.CommonString.AlarmTag);
                }
                else if (type == IFacility.FacilityType.FIREWALL)
                {
                    DataManager.ClearPOI(Data.CommonString.POI_FireWall);
                    DataManager.ClearPOI(Data.CommonString.POI_FireWall + Data.CommonString.AlarmTag);
                }
                else if (type == IFacility.FacilityType.PSM_SENSOR)
                {
                    DataManager.ClearPOI(Data.CommonString.POI_Gas);
                    DataManager.ClearPOI(Data.CommonString.POI_Gas + Data.CommonString.AlarmTag);
                }
            }
            else
            {
                if (type == IFacility.FacilityType.FIRE_SENSOR)
                {
                    DataManager.LoadSensorPOI(m_dbMgr, false, true, IFacility.FacilityType.FIRE_SENSOR);
                }
                else if (type == IFacility.FacilityType.CCTV)
                {
                    DataManager.LoadCCTVPOI(m_dbMgr, false, true);
                }
                else
                {
                    DataManager.LoadSensorPOI(m_dbMgr, false, true, type);
                }
            }
        }

        public void SetVisible3DPopup(bool visible)
        {
            if (visible)
            {

            }
            else
            {
                m_ufrmPoiVisible.Visible = false;
                m_ufrmBroadcast.Visible = false;
            }
        }

        public void SetLayerState()
        {
            if (m_CurrentTab == UnE.View.Content.ContentOwnerTab.ADMIN_TAB)
            {
                Panel4Unity panel = (Panel4Unity)FormMain.Instance.ContentManager.ContentForm.OutdoorView;

                List<string> itemTypeNames = new List<string>();
                itemTypeNames.Add(Data.CommonString.POI_CCTV);
                panel.ShowIconLayers(itemTypeNames, true);
            }
        }

        private Process m_pSensorTester = null;
        private void btnTeamEditor_Click(object sender, EventArgs e)
        {
            ExecuteManager mgr = new ExecuteManager();
            m_pSensorTester = mgr.Run(ExecuteManager.APP_TYPE.TEAM_MANAGER);
        }

        private void btnTester_Click(object sender, EventArgs e)
        {
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
                //CreatePanelSimulator(strOptionString.Split(','));
            }
        }

        public IFacility.FacilityType GetManualReportType(int nSensorZoneHistoryID)
        {
            ArrayList arrResult = m_dbMgr.GetResultData("Select param2 From SensorZoneHistory Where ID = " + nSensorZoneHistoryID);
            if (arrResult == null || arrResult.Count == 0)
                return IFacility.FacilityType.NONE;

            int nType = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            return IFacility.ToFacilityType(nType);
        }

        private void EnableOutdoor(bool enabled)
        {
            m_enableOutdoor = enabled;
            btnOutdoor.Enabled = enabled;

            // 외부화면일 경우 실내화면으로 강제로 바꾼다.
            if (enabled == false)
            {
                //int nSelectedIndex = m_cbBuilding.customComboBox.SelectedIndex;
                //Building building = m_cbBuilding.customComboBox.Items[nSelectedIndex] as Building;

                //if (building != null && building.BuildingName == "외부")
                //{
                //    m_systemInput = true;

                //    if (nSelectedIndex == 0)
                //        m_cbBuilding.customComboBox.SelectedIndex = nSelectedIndex + 1;
                //    else
                //        m_cbBuilding.customComboBox.SelectedIndex = nSelectedIndex - 1;

                //    m_systemInput = false;
                //}
            }
        }

        //public void SetEnableIconList(List<IFacility.FacilityType> enableList)
        //{
        //    IFacility.FacilityType selectedType = uSensorInfo.SetEnableList(enableList, uSensorInfo.SelectedType);

        //    if (selectedType != uSensorInfo.SelectedType)
        //        uSensorInfo.SelectType(selectedType);
        //}

        //public List<IFacility.FacilityType> GetEnableIconList()
        //{
        //    return uSensorInfo.GetEnableList();
        //}

        public void OnAddPOI(POI poi)
        {
            if (m_uFormEdit != null)
            {
                SensorInfo sensor = m_uFormEdit.OnAddPOI(poi);

                if (sensor != null)
                {
                    List<SensorInfo> sensors = new List<SensorInfo>();
                    sensors.Add(sensor);
                }
            }
        }

        public void OnMovePOI(POI poi)
        {
            if (m_uFormEdit != null)
            {
                SensorInfo sensor = m_uFormEdit.OnMovePOI(poi);
                //DataGridViewRow row = GetDataGridViewRow(sensor);

                //if (row != null)
                //    row.Selected = true;
            }
        }

        public void OnDeletePOI(POI poi)
        {
            if (m_uFormEdit != null)
            {
                SensorInfo sensor = m_uFormEdit.OnDeletePOI(poi);

                if (sensor != null)
                {
                    List<SensorInfo> sensors = new List<SensorInfo>();
                    sensors.Add(sensor);
                    //if (dgvSensorInfo.SelectedCells.Count == 0)
                        m_uFormEdit.OnSelectSensor(null, IFacility.FacilityType.NONE);
                }
            }
        }

        private void dgvSensorInfo_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex < 0)
            //{
            //    m_editCell = null;
            //    m_strOriginText = "";
            //    return;
            //}

            //DataGridViewRow row = dgvSensorInfo.Rows[e.RowIndex];
            //DataGridViewCell cell = row.Cells[e.ColumnIndex];
            //string strText = cell.Value == null ? "" : cell.Value.ToString();

            //if (cell != m_editCell || m_strOriginText == strText)
            //{
            //    m_editCell = null;
            //    m_strOriginText = "";
            //    return;
            //}

            //SensorInfo sensor = (SensorInfo)row.Tag;

            //if (sensor == null)
            //{
            //    m_editCell = null;
            //    m_strOriginText = "";
            //    return;
            //}

            //if (m_CurrentTab == ContentOwnerTab.ADMIN_TAB && m_uFormEdit != null)
            //    m_uFormEdit.OnChangeText(sensor, strText, IFacility.FacilityType.CCTV);

            //m_editCell = null;
            //m_strOriginText = "";
        }

        private void dgvSensorInfo_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            //if (e.RowIndex < 0)
            //    return;

            //DataGridViewRow row = dgvSensorInfo.Rows[e.RowIndex];
            //m_editCell = row.Cells[e.ColumnIndex];
            //m_strOriginText = m_editCell.Value == null ? "" : m_editCell.Value.ToString();
        }

        public void SelectedCCTV(CCTV cctv)
        {
            m_uFormEdit.OnSelectCCTV(cctv);
            //SelectedSensorInfoGridView(cctv.ID);
        }

        public void SelectedPOI(IFacility.FacilityType facilityType, int nID)
        {
            SelectToTreeView(facilityType, nID);
        }

        private void SelectToTreeView(IFacility.FacilityType facilityType, int nID)
        {
            Node node = SelectFacilityTypeNode(m_treeModel.Nodes, facilityType, nID);
            if (node == null)
                return;

            TreeNodeAdv selectedNode = treeViewAdv1.FindNode(m_treeModel.GetPath(node));            
            treeViewAdv1.SelectedNode = selectedNode;
            if (selectedNode == null)
                return;
            treeViewAdv1.ScrollTo(selectedNode);
        }

        private void SelectToTreeView(int nZoneID)
        {
            Node node = SelectZoneNode(m_treeModel.Nodes, nZoneID);
            if (node == null)
                return;

            
            TreeNodeAdv selectedNode = treeViewAdv1.FindNode(m_treeModel.GetPath(node));
            treeViewAdv1.SelectedNode = selectedNode;

            // 펼치기
            selectedNode.IsExpanded = true;
            if (selectedNode.Children != null)
            {
                foreach (TreeNodeAdv item in selectedNode.Children)
                {
                    item.IsExpanded = true;
                } 
            }             
        }

        private Node SelectFacilityTypeNode(System.Collections.ObjectModel.Collection<Node> nodes, IFacility.FacilityType facilityType, int nID)
        {
            foreach (Node node in nodes)
            {
                if (node.Tag != null)
                {
                    if (node.Tag is ISensor && facilityType == IFacility.FacilityType.FIRE_SENSOR)
                    {
                        ISensor sensor = node.Tag as ISensor;
                        if (sensor != null && sensor is FireSensor)
                        {
                            if (sensor.ID == nID)
                                return node;
                        }
                    }
                    else if (node.Tag is ISensor && facilityType == IFacility.FacilityType.DOOR)
                    {
                        ISensor sensor = node.Tag as ISensor;
                        if (sensor != null && sensor is EtcSensor)
                        {
                            if (sensor.ID == nID)
                                return node;
                        }
                    }
                    else if (node.Tag is CCTV && facilityType == IFacility.FacilityType.CCTV)
                    {
                        CCTV cctv = node.Tag as CCTV;
                        if (cctv != null)
                        {
                            if (cctv.ID == nID)
                                return node;
                        }
                    }
                }

                Node findNode = SelectFacilityTypeNode(node.Nodes, facilityType, nID);
                if (findNode != null)
                    return findNode;
            }

            return null;
        }

        private Node SelectZoneNode(System.Collections.ObjectModel.Collection<Node> nodes, int nZoneID)
        {
            foreach (Node node in nodes)
            {
                if (node.Tag != null)
                {
                    if (node.Tag is Zone)
                    {
                        Zone zone = node.Tag as Zone;
                        if (zone != null)
                        {
                            if (zone.ID == nZoneID)
                                return node;
                        }
                    }
                }

                Node findNode = SelectZoneNode(node.Nodes, nZoneID);
                if (findNode != null)
                    return findNode;
            }

            return null;
        }

        private void SetAlarmLevel(ProcessIF proc, ref Image imgLevel, ref string strLevel)
        {
            int nAlarmLevel = -1;
            if (m_dicAlarmStep.ContainsKey(proc.TargetSensor.ID))
            {
                nAlarmLevel = m_dicAlarmStep[proc.TargetSensor.ID];
                switch (nAlarmLevel)
                {
                    case 1:
                        imgLevel = m_imgAlarmLevel1;
                        strLevel = "관심";
                        break;
                    case 2:
                        imgLevel = m_imgAlarmLevel2;
                        strLevel = "주의";
                        break;
                    case 3:
                        imgLevel = m_imgAlarmLevel3;
                        strLevel = "경계";
                        break;
                    case 4:
                        imgLevel = m_imgAlarmLevel4;
                        strLevel = "심각";
                        break;
                }

                proc.AlarmLevel = nAlarmLevel;
                
                //m_dicAlarmStep.Remove(proc.TargetSensor.ID);
            }
            else
            {
                if (proc.ProcessType == ProcessType.BlackoutAlarm)
                {
                    imgLevel = m_imgAlarmLevel4;
                    strLevel = "심각";
                }
                else
                {
                    imgLevel = m_imgAlarmLevel2;
                    strLevel = "주의";
                }
            }
        }

        /// <summary>
        /// 현재 발생중인 알람들의 등급을 변경한다
        /// </summary>
        public void SetAlarmLevel()
        {
            if (m_dicAlarmStep.Count == 0)
                return;

            foreach (DataGridViewRow row in dgvDetectList.Rows)
            {
                ProcessIF proc = row.Tag as ProcessIF;
                if (proc == null)
                    continue;

                if (!m_dicAlarmStep.ContainsKey(proc.TargetSensor.ID))
                    continue;

                int nChgAlarmLevel = m_dicAlarmStep[proc.TargetSensor.ID];
                if (nChgAlarmLevel != proc.AlarmLevel) // 알람 등급이 변경됐다면
                {
                    string strValue = "";
                    switch (nChgAlarmLevel)
                    {
                        case 1: strValue = "관심"; break;
                        case 2: strValue = "주의"; break;
                        case 3: strValue = "경계"; break;
                        case 4: strValue = "심각"; break;
                    }
                    row.Cells[colStatus.Index].Value = strValue;
                    proc.AlarmLevel = nChgAlarmLevel;
                }
            }
        }

        public void ChangeWall()
        {
            m_uFormEdit.ChangeWall();
        }

        public void ChangeSpaceText()
        {
            m_uFormEdit.ChangeSpaceText();
        }

        public void GetWallInfo(float x, float y, float scale, float rotate)
        {
            m_uFormEdit.GetWallInfo(x, y, scale, rotate);
        }

        /// <summary>
        /// 신호 종료시 외부화면으로 이동할건가 ?
        /// </summary>
        private void ReadGoOutside()
        {
            DBUtility2.Utility util = new DBUtility2.Utility();
            string szValue = util.getinivalue("SDMS", "go_outside");

            int nValue = 1;
            if (int.TryParse(szValue, out nValue))
            {
                m_bGoOutside = (nValue == 1) ? true : false;
            }
        }

        // 자동시정 출입문 개폐 조회
        private void DisplayCloseDoor()
        {            
            if (dgvDetectList == null || dgvDetectList.Rows.Count == 0 || dgvDetectList.SelectedRows == null)
            {
                if (dgvOpenDoor.Rows.Count > 0)
                    dgvOpenDoor.Rows.Clear();
                return;
            }

            ProcessIF process = (ProcessIF)dgvDetectList.SelectedRows[0].Tag;
            if (process.ProcessType != ProcessType.FireAlarm)
            {
                if (dgvOpenDoor.Rows.Count > 0)
                    dgvOpenDoor.Rows.Clear();
                return;
            }

            if (m_selectedFloor == null || process.TargetZone.LinkedZone.ID != m_selectedFloor.Zone.ID)
            {
                if (dgvOpenDoor.Rows.Count > 0)
                    dgvOpenDoor.Rows.Clear();
                return;
            }

            List<POI> pois = null;
            List<string> poiTypes = null;

            m_dataMgr.LoadCloseDoorPOI(process.TargetZone.LinkedZone.ID, ref pois, ref poiTypes);
            
            // 변경된 사항이 있나?
            if (pois != null && poiTypes != null && pois.Count > 0 && poiTypes.Count > 0)
            {
                Panel4Unity panel = (Panel4Unity)m_contentManager.ContentForm.OutdoorView;
                panel.ChangePOIIcons(pois, poiTypes);
            }

            if (dgvOpenDoor.Rows.Count > 0)
                dgvOpenDoor.Rows.Clear();

            List<ISensor> doors = SensorManager.Instance.DicDoorSensorByZoneID[process.TargetZone.LinkedZone.ID];
            int doorCount = doors.Count;
            for (int i = 0; i < doorCount; i++)
            {
                if (doors[i] is EtcSensor)
                {
                    EtcSensor door = doors[i] as EtcSensor;                    
                    if (door.Description != "닫힘")
                        continue;

                    int rowCount = dgvOpenDoor.Rows.Add();
                    dgvOpenDoor.Rows[rowCount].Cells[0].Value = door.SensorName;
                }
            }
        }

        private void RollbackCloseDoor(int nZoneID)
        {
            if (!SensorManager.Instance.DicDoorSensorByZoneID.ContainsKey(nZoneID))
                return;

            List<ISensor> doors = SensorManager.Instance.DicDoorSensorByZoneID[nZoneID];
            if (doors == null)
                return;

            int doorCount = doors.Count;
            for (int i = 0; i < doorCount; i++)
            {
                doors[i].Description = "";

                Content.TooltipHandler handler = doors[i].POI.Popup as Content.TooltipHandler;
                if (handler != null)
                {
                    handler.CurrentPOIType = Data.CommonString.POI_Door;                    
                }
            }
        }

        private void M_timerCloseDoor_Tick(object sender, EventArgs e)
        {
            DisplayCloseDoor();
        }

        private bool m_bViewAllSensor = true; // TreeView에 전체 센서를 보여줄지 Off된 센서만 보여줄지 여부
        private void radioSensor_Click(object sender, EventArgs e)
        {
            RibbonButton rbtn = sender as RibbonButton;
            if (rbtn == null)
                return;

            if (rbtn == radioAllSensor && radioAllSensor.IsChecked)
                return;

            if (rbtn == radioOffSensor && radioOffSensor.IsChecked)
                return;

            if (rbtn == radioAllSensor)
            {
                m_bViewAllSensor = true;
                radioAllSensor.IsChecked = true;
                radioOffSensor.IsChecked = false;
            }
            else if (rbtn == radioOffSensor)
            {
                m_bViewAllSensor = false;
                radioAllSensor.IsChecked = false;
                radioOffSensor.IsChecked = true;
            }

            radioAllSensor.Refresh();
            radioOffSensor.Refresh();

            LoadZoneTreeView();
            if (m_selectedFloor != null && m_selectedFloor.Zone != null)
                SelectToTreeView(m_selectedFloor.Zone.ID);
        }

        private void SetDeActivate(ISensor sensor)
        {
            string strMsg = "";
            if (sensor.DeActivate)
                strMsg = "[" + sensor.SensorName +"] 센서의 알람 신호를 받을까요 ?";
            else
                strMsg = "[" + sensor.SensorName + "] 센서의 알람 신호를 무시할까요 ?";

            FormMessageBox msg = new FormMessageBox(strMsg, MessageBoxButtons.YesNo);
            msg.StartPosition = FormStartPosition.CenterParent;
            DialogResult result = msg.ShowDialog();
            if (result == DialogResult.Yes)
            {
                string value = (!sensor.DeActivate) ? "Y" : "N";

                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Update SensorTagInfo Set DeActivate = '{0}' ", value);
                sb.AppendFormat(" Where SensorZoneID = (select ID from sensorzone where OrgSensorID={0})", sensor.ID);

                if (m_dbMgr.GetResultData(sb.ToString()) != null)
                {
                    if (SensorManager.Instance.DicFireSensorByZoneID.ContainsKey(sensor.ZoneID))
                    {
                        foreach (ISensor item in SensorManager.Instance.DicFireSensorByZoneID[sensor.ZoneID])
                        {
                            if (item.ID == sensor.ID)
                                item.DeActivate = !sensor.DeActivate;
                        }
                        LoadZoneTreeView();
                        if (m_selectedFloor != null && m_selectedFloor.Zone != null)
                            SelectToTreeView(IFacility.FacilityType.FIRE_SENSOR, sensor.ID);
                    }
                }
            }
        }
                
        private void button1_Click_1(object sender, EventArgs e)
        {
            SetEquipmentZone popup = new SetEquipmentZone(m_dbMgr);
            popup.Show();
        }

        public void OnSelectedFirePOI(int orgSensorID, int zoneID)
        {
            if (m_selectedFloor == null || m_selectedFloor.Zone.ID != zoneID)
            {
                // 층 이동부터 
                Zone zone = ZoneManager.Instance.GetZone(zoneID);
                MoveZone(false, zone);
            }

            if (m_selectedFloor == null)
                return;

            if (m_selectedFloor.Zone.ID != zoneID)
                return;

            POI poi = null;
            string poiType = "";
            
            if (!m_dataMgr.DicFirePOI.ContainsKey(orgSensorID))
                return;

            poi = m_dataMgr.DicFirePOI[orgSensorID];
            poiType = CommonString.POI_Fire;

            Panel4Unity panel = (Panel4Unity)m_contentManager.ContentForm.OutdoorView;
            panel.RollBackPOIIcon("");
            m_dataMgr.ChangePOIIcon(poi, poiType + "_Click");
        }
    }
}
