using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using IronPython;

namespace HSMS
{
    public partial class FormMain : Form, UnE.GUI.ITextPictureBoxOwner, UnE.GUI.IRibbonButtonOwner, IronPython.ITextCommanderOwner
    {
        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        class AlarmState
        {
            private bool m_clearScreen = false;
            private string m_strDetail = "";
            private bool m_isCritical = false;
            private string m_strAlarmStatus = "";
            private string m_strMessage = "";
            private string m_strShortMessage = "";
            private DangerState m_dangerState = null;

            public bool ClearScreen
            {
                get { return m_clearScreen; }
                set { m_clearScreen = value; }
            }

            public string Detail
            {
                get { return m_strDetail; }
                set { m_strDetail = value; }
            }

            public bool IsCritical
            {
                get { return m_isCritical; }
                set { m_isCritical = value; }
            }

            public string Status
            {
                get { return m_strAlarmStatus; }
                set { m_strAlarmStatus = value; }
            }

            public string Message
            {
                get { return m_strMessage; }
                set { m_strMessage = value; }
            }

            public string ShortMessage
            {
                get { return m_strShortMessage; }
                set { m_strShortMessage = value; }
            }

            public DangerState DangerState
            {
                get { return m_dangerState; }
                set { m_dangerState = value; }
            }
        }

        public enum AlarmType { NO_ALARM = 0, ALARM_LEVEL1, ALARM_LEVEL2 };

        private DataManager m_DataMgr = null;
        internal DataManager DataMgr
        {
            get { return m_DataMgr; }
            set { m_DataMgr = value; }
        }

        private ArrayList m_arrAdminCheckedButtons = new ArrayList();
        private ArrayList m_arrToolBarCheckedButtons = new ArrayList();

        private NetworkManager m_netMgr = null;
        public NetworkManager NetMgr
        {
            get { return m_netMgr; }
            set { m_netMgr = value; }
        }

        //위험 레벨
        //private int m_danger_Level = 0;
        //private SafetyChecker m_safetyChecker = null;
        private AlarmManager m_alarmMgr = null;

        private AlarmState m_alarmStatus = null;

        private bool m_lockMessage = false;
        public bool LockMessage
        {
            get { return m_lockMessage; }
            set { m_lockMessage = value; }
        }

        private AlarmType m_alarmType = AlarmType.NO_ALARM;
        /*public AlarmType CurrentAlarm
        {
            get { return m_alarmType; }
            set { m_alarmType = value; }
        }*/
        public DangerState CurrentAlarm
        {
            get
            {
                DangerState state = null;

                this.Invoke((MethodInvoker)delegate
                {
                    if (cboAlarmList.SelectedIndex >= 0)
                        state = (DangerState)cboAlarmList.Items[cboAlarmList.SelectedIndex];
                });

                return state;
            }
        }

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private FormContent m_FormContent = new FormContent();

        private DataWorker m_selectedWorkerPOI = null;
        private object m_selectedTargetPOI = null;
        
        Bitmap m_bmpAdmin = null;
        private static SoundPlayerEx m_player = new SoundPlayerEx();

        private string m_strLoginID = "";

        public string LoginID
        {
            get { return m_strLoginID; }
            set { m_strLoginID = value; }
        }

        // DB 갱신시 호출되는 항목 , UI초기화 항목을 추가한다.
        public void Reload()
        {

        }

        public void RefreshMessage()
        {
            m_lockMessage = false;

            if (m_alarmStatus == null)
            {
                if (m_alarmType != AlarmType.NO_ALARM)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        SetDangerLevel(0);
                    });
                }
                return;
            }

            this.Invoke((MethodInvoker)delegate
            {
                if (m_alarmStatus.ClearScreen)
                {
                    ClearAlarm();
                }
                else
                {
                    SetAlarm(m_alarmStatus.IsCritical, m_alarmStatus.Detail, m_alarmStatus.Status, m_alarmStatus.Message, m_alarmStatus.ShortMessage, m_alarmStatus.DangerState);
                }
            });
        }

        private DataReport m_DataReport = null;
        internal DataReport DataReport
        {
            get { return m_DataReport; }
        }

        #region Python ==========================================================================
        private TextCommander commander = null;
        public TextCommander Commander
        {
            get { return commander; }
            set { commander = value; }
        }
        public void AddPythonFunction()
        {
            ScriptProxy proxy = ScriptProxy.Instance;
            proxy.UserObject.MainForm = this;
            proxy.UserObject.Script = ScriptProxy.Instance;

        }

        private bool m_bWriteConsole = false;
        public void SetConsoleLog(bool bOnOff)
        {
            m_bWriteConsole = bOnOff;
            
            if( bOnOff == true)
            {
                if (tmrUpdate == null)
                {
                    ConsoleDebugger.Instance.EnableLogger = true;
                    tmrUpdate = new System.Timers.Timer();

                    tmrUpdate.Elapsed += new System.Timers.ElapsedEventHandler(tmrUpdate_Tick);
                    tmrUpdate.Interval = 1000;
                    tmrUpdate.Enabled = false;
                }

                if (tmrUpdate.Enabled == false)
                {
                    
                    _logger = ScriptProxy.Instance.Logger;
                    tmrUpdate.Enabled = true;
                    tmrUpdate.Start();
                }               
            }
            else
            {
                ConsoleDebugger.Instance.EnableLogger = false;
                _logger = null;
                tmrUpdate.Enabled = false;
                tmrUpdate.Stop();
            }
        }

        private System.Timers.Timer tmrUpdate = null;
        private PythonLogger _logger = null;
        private void tmrUpdate_Tick(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_logger == null)
                return;

            List<PythonLogger.Entry> entries = _logger.GetAll();
            foreach (var entry in entries)
            {
                string szLog = entry.ToString();
                if (szLog.Contains("Fault"))
                {
                    ConsoleDebugger.WriteLine(entry.ToString(), ConsoleColor.Red);
                }
                else
                    ConsoleDebugger.WriteLine(entry.ToString(), ConsoleColor.Green);
            }

            if (m_bWriteConsole == true && entries.Count > 0)
                ConsoleDebugger.Write(">> ", ConsoleColor.Red);           
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if(!base.ProcessCmdKey(ref msg, keyData))
            {
                if (keyData.Equals(Keys.Control | Keys.Insert))
                {
                    if(commander!= null && commander.IsInit == true)
                    {
                        ConsoleDebugger.Instance.ShowConsole(true);
                        return true;
                    }                    
                }
                return false;
            }
            return true;
        }
        public void CreaetPythonContext()
        {
            commander = new TextCommander(this);
            // Init Text commander
            commander.InitCommander();

            // Create Console and Begin Input Thread
            if (commander.BeginCommnander())
            {
                // Create Python Context
                AddPythonFunction();
            }

            
        }
        #endregion//Python ==========================================================================


        public FormMain()
        {
            
            m_instance = this;
            InitializeComponent();

            CreaetPythonContext();

            btnSaveAdmin.Enabled = false;
            btnDeleteAdmin.Enabled = false;

            monthCalendar1.Visible = false;
            monthCalendar2.Visible = false;

            m_alarmMgr = new AlarmManager();
            m_DataMgr = new DataManager();
            m_DataReport = new DataReport();

            
        }

        private PageBackstageHome m_PageHome = null;
        public PageBackstageHome PageHome
        {
            get { return m_PageHome; }
        }

        // Button별 ID
        private Dictionary<Button, int> m_dicButtonIDs = new Dictionary<Button, int>();
        private Dictionary<int, UnE.GUI.RibbonButton> m_dicIDButtons = new Dictionary<int, UnE.GUI.RibbonButton>();
        private Dictionary<Button, bool> m_dicButtonChecked = new Dictionary<Button, bool>();

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public AlarmManager AlarmManager
        {
            get { return m_alarmMgr; }
            set { m_alarmMgr = value; }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            if (LoginManager.Instance.LoginState == false)
                Application.Exit();
           
            this.DoubleBuffered = true;

            //시간표시 타이머 시작
            ClockTimer.Start();

            InitTab();

            CreateBackstageHome();
            m_PageHome.CreateReportForm();
            m_PageHome.Show();

            InitButton();
            initComboBox();

            
            Create3DView();

            SelectedMonitoringTab();
            SetDangerLevel(0);

            ERPManager.Instance.ReadErpData();

            //if (m_netMgr == null)
            //m_netMgr = new NetworkManager();
            //m_safetyChecker = new SafetyChecker(m_DataMgr);

            MakeSensorOwner();

            m_FormContent.Update3DView();
            m_FormContent.CreateCCTVs();
            m_FormContent.CreateAPs();
            m_FormContent.CreateGasSensors();

            lblDangerDetail.Text = "";

            btnReportDateStart.Text = DateTime.Now.AddMonths(-6).ToShortDateString();
            btnReportDateEnd.Text = DateTime.Now.ToShortDateString();
            m_PageHome.FrmReport.Visible = false;

            //리포트 타이머 시작
            m_DataReport.tm_Tick(null, null);


            CheckLeftLayerButton(btnWorkerLayer);
            CheckLeftLayerButton(btnVehicleLayer);
            CheckLeftLayerButton(btnDangerFacilityLayer);
        }

        private void initComboBox()
        {
            cboReportLatelyDate.Items.Add("6개월");
            cboReportLatelyDate.Items.Add("3개월");
            cboReportLatelyDate.Items.Add("1개월");

            if (cboReportLatelyDate.SelectedIndex == -1)
            {
                cboReportLatelyDate.SelectedIndex = 0;
            }

            cboAlarmStep.Items.Add("전체 알람");
            cboAlarmStep.Items.Add("1단계 알람");
            cboAlarmStep.Items.Add("2단계 알람");


            if (cboAlarmStep.SelectedIndex == -1)
            {
                cboAlarmStep.SelectedIndex = 0;
            }
        }

        private void CreateBackstageHome()
        {
            m_PageHome = new PageBackstageHome();
            m_PageHome.Location = new Point(0, 0);
            m_PageHome.Dock = DockStyle.Fill;
            m_PageHome.TopLevel = false;
            m_PageHome.Parent = panelBottom;

            panelBottom.Controls.Add(m_PageHome);
        }


        private void Create3DView()
        {
            m_FormContent.Dock = DockStyle.Fill;
            m_FormContent.TopLevel = false;
            m_PageHome.ContentView = m_FormContent;

            m_PageHome.ContentPane.Controls.Add(m_FormContent);
            m_FormContent.Visible = true;
            m_FormContent.Init3DView();
        }

        public static void SetInfoMessage(string szMessage)
        {
            if (((RealTimeInfoPane)m_instance.panelLog).RealTimeInfo != szMessage)
            {
                ((RealTimeInfoPane)m_instance.panelLog).RealTimeInfo = szMessage;
                ((RealTimeInfoPane)m_instance.panelLog).DrawMovingText();
            }
        }

        //위험레벨설정
        private bool SetDangerLevel(int nDanger_Level)
        {
            //평상시
            if (nDanger_Level == 0)
            {
                if (m_alarmType != AlarmType.NO_ALARM)
                {
                    m_alarmType = AlarmType.NO_ALARM;
                    lblDangerDetail.Visible = false;

                    btnStatusEnd.Enabled = false;
                    btnStatusEnd.Text = "";
                    btnStatusEnd.Enabled = false;
                    btnStatusEnd.ForeColor = System.Drawing.Color.FromArgb(175, 167, 164);
                    this.btnStatusEnd.BackgroundImage = global::HSMS.Properties.Resources.status_end_nomal;
                    this.btnStatusEnd.MouseOverBkgndImage = global::HSMS.Properties.Resources.status_end_mouseover;
                    this.btnStatusEnd.CheckedBkgndImage = global::HSMS.Properties.Resources.status_end_click;
                    return true;
                }
            }
            else if (nDanger_Level == 1) //위험레벨1
            {
                if (m_alarmType != AlarmType.ALARM_LEVEL1)
                {
                    m_alarmType = AlarmType.ALARM_LEVEL1;
                    lblDangerDetail.Visible = true;
                    btnStatusEnd.Enabled = true;
                    btnStatusEnd.Text = "상황종료";
                    btnStatusEnd.Enabled = true;
                    btnStatusEnd.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
                    this.btnStatusEnd.BackgroundImage = global::HSMS.Properties.Resources.Danger_Level1;
                    this.btnStatusEnd.MouseOverBkgndImage = global::HSMS.Properties.Resources.위험1단계_over;
                    this.btnStatusEnd.CheckedBkgndImage = global::HSMS.Properties.Resources.위험1단계_click;
                    return true;
                }
            }
            else if (nDanger_Level == 2) //위험레벨2
            {
                if (m_alarmType != AlarmType.ALARM_LEVEL2)
                {
                    m_alarmType = AlarmType.ALARM_LEVEL2;
                    lblDangerDetail.Visible = true;
                    btnStatusEnd.Enabled = true;
                    btnStatusEnd.Text = "상황종료";
                    btnStatusEnd.Enabled = true;
                    btnStatusEnd.ForeColor = System.Drawing.Color.FromArgb(255, 255, 255);
                    this.btnStatusEnd.BackgroundImage = global::HSMS.Properties.Resources.Danger_Level2;
                    this.btnStatusEnd.MouseOverBkgndImage = global::HSMS.Properties.Resources.위험2단계_over;
                    this.btnStatusEnd.CheckedBkgndImage = global::HSMS.Properties.Resources.위험2단계_click;
                    return true;
                }
            }
            return false;
        }

        private void InitTab()
        {
            //PictureBox
            this.pictureBoxMonitoring.Owner = this;
            this.pictureBoxAdmin.Owner = this;
            this.pictureBoxReport.Owner = this;

            //Panel
            panelMonitoringTab.Location = new Point(0, pictureBoxMonitoring.Size.Height);
            panelAdminTab.Location = panelMonitoringTab.Location;
            panelReportTab.Location = panelMonitoringTab.Location;

            panelMiddle.Location = new Point(0, panelTop.Location.Y + panelTop.Size.Height);
            panelAdminBar.Location = new Point(0, 0);
            panelReportBar.Location = panelAdminBar.Location;

            panelMiddle.Size = new Size(panelAdminBar.Size.Width, panelAdminBar.Size.Height);


            panelLeft.Location = new Point(0, panelMiddle.Location.Y + panelMiddle.Size.Height);
            panelBottom.Location = new Point(panelLeft.Size.Width, panelLeft.Location.Y);
            Size size = panelBottom.Size;
            panelBottom.Size = new Size(panelAdminBar.Size.Width - panelLeft.Size.Width - 3, size.Height);
            //panelBottom.Location = new Point(0,

            int nisAdmin = 0;
            UnE.Utility.Properties.GetProperty("isAdmin", ref nisAdmin);

            if (nisAdmin != 1)
            {
                this.pictureBoxAdmin.Enabled = false;
                m_bmpAdmin = global::HSMS.Properties.Resources.Tab_disable;
            }
            else
                m_bmpAdmin = global::HSMS.Properties.Resources.Tab_off;
        }

        private Bitmap GetLayerBtnImage(int nLayerID, int nState)
        {
            int nID = nLayerID;

            if (nID <= ID.ID_LAYER_BEGIN || nID >= ID.ID_LAYER_END)
                return null; ;
            int nSelected = nState;
            Bitmap selectIcon = null;
            if (nID == ID.ID_LAYER_WORKER)
            {
                switch (nSelected)
                {
                    case 1:
                        selectIcon = global::HSMS.Properties.Resources.leftBar_worker_over;
                        break;
                    case 2:
                        selectIcon = global::HSMS.Properties.Resources.leftBar_worker_disabled;
                        break;
                    case 3:
                        selectIcon = global::HSMS.Properties.Resources.leftBar_worker_nomal;
                        break;
                }
            }
            else if (nID == ID.ID_LAYER_EQUIP)
            {
                switch (nSelected)
                {
                    case 1:
                        selectIcon = global::HSMS.Properties.Resources.leftBar_dangerfacility_over;
                        break;
                    case 2:
                        selectIcon = global::HSMS.Properties.Resources.leftBar_dangerfacility_disabled;
                        break;
                    case 3:
                        selectIcon = global::HSMS.Properties.Resources.leftBar_dangerfacility_nomal;
                        break;
                }
            }
            else if (nID == ID.ID_LAYER_CAR)
            {
                switch (nSelected)
                {
                    case 1:
                        selectIcon = global::HSMS.Properties.Resources.leftBar_vehicle_over;
                        break;
                    case 2:
                        selectIcon = global::HSMS.Properties.Resources.leftBar_vehicle_disabled;
                        break;
                    case 3: nSelected = 1;
                        selectIcon = global::HSMS.Properties.Resources.leftBar_vehicle_nomal;
                        break;
                }
            }
            return selectIcon;
        }

        private void InitButton()
        {
            //PanelLeft
            this.btnWorkerLayer.Owner = this;
            this.btnDangerFacilityLayer.Owner = this;
            this.btnVehicleLayer.Owner = this;

            btnWorkerLayer.Tag = 1;
            btnDangerFacilityLayer.Tag =1;
            btnVehicleLayer.Tag = 1;

            

            //this.btnDangerZoneLayer.Owner = this;

            SetButtonID(btnWorkerLayer, ID.ID_LAYER_WORKER);
            SetButtonID(btnVehicleLayer, ID.ID_LAYER_CAR);
            SetButtonID(btnDangerFacilityLayer, ID.ID_LAYER_EQUIP);

            this.btnStatusEnd.Owner = this;
            SetButtonID(btnStatusEnd, ID.ID_MANAGE_STATUSEND);

            this.btnStatusEnd.Font = new System.Drawing.Font("맑은 고딕", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnWorkerAdmin.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            //PanelMenu(Admin)
            this.btnWorkerAdmin.Owner = this;
            this.btnVehicleAdmin.Owner = this;
            this.btnFacilityAdmin.Owner = this;
            this.btnDangerZoneAdmin.Owner = this;
            this.btnSaveAdmin.Owner = this;
            this.btnDeleteAdmin.Owner = this;
            this.btnManagerAdmin.Owner = this;
            this.btnMessageAdmin.Owner = this;
            this.btnAlarmAdmin.Owner = this;
            this.btnDetectAdmin.Owner = this;
            this.btnListAdmin.Owner = this;
            this.btnOption.Owner = this;
            this.btnReportHistory.Owner = this;

            SetButtonID(btnWorkerAdmin, ID.ID_ADMIN_WORKER);
            SetButtonID(btnVehicleAdmin, ID.ID_ADMIN_VEHICLE);
            SetButtonID(btnFacilityAdmin, ID.ID_ADMIN_DANGERFACILITY);
            SetButtonID(btnDangerZoneAdmin, ID.ID_ADMIN_DANGERZONE);
            SetButtonID(btnSaveAdmin, ID.ID_ADMIN_SAVE);
            SetButtonID(btnDeleteAdmin, ID.ID_ADMIN_DELETE);
            SetButtonID(btnManagerAdmin, ID.ID_ADMIN_MANAGER);
            SetButtonID(btnMessageAdmin, ID.ID_ADMIN_MESSAGE);
            SetButtonID(btnAlarmAdmin, ID.ID_ADMIN_ALARMDISTANCE);
            SetButtonID(btnDetectAdmin, ID.ID_ADMIN_DETECT);
            SetButtonID(btnListAdmin, ID.ID_ADMIN_LIST);
            SetButtonID(btnOption, ID.ID_ADMIN_OPTION);
            SetButtonID(btnReportHistory, ID.ID_ADMIN_HISTORY);

            //관리탭 체크 버튼들 집합
            m_arrAdminCheckedButtons.Add(btnWorkerAdmin);
            m_arrAdminCheckedButtons.Add(btnVehicleAdmin);
            m_arrAdminCheckedButtons.Add(btnFacilityAdmin);
            m_arrAdminCheckedButtons.Add(btnDangerZoneAdmin);
            m_arrAdminCheckedButtons.Add(btnManagerAdmin);
            m_arrAdminCheckedButtons.Add(btnMessageAdmin);
            m_arrAdminCheckedButtons.Add(btnAlarmAdmin);
            m_arrAdminCheckedButtons.Add(btnDetectAdmin);
            m_arrAdminCheckedButtons.Add(btnListAdmin);
            m_arrAdminCheckedButtons.Add(btnOption);
            m_arrAdminCheckedButtons.Add(btnReportHistory);

            //PanelMiddle(툴바)
            this.btnSaveHome.Owner = this;
            this.btnHome.Owner = this;
            this.btnPick.Owner = this;
            this.btnFullScreen.Owner = this;
            this.btnOrbit.Owner = this;
            this.btnPanning.Owner = this;
            this.btnZoomOut.Owner = this;
            this.btnZoomIn.Owner = this;

            SetButtonID(btnSaveHome, ID.ID_VIEW_SAVEHOME);
            SetButtonID(btnHome, ID.ID_VIEW_HOME);
            SetButtonID(btnPick, ID.ID_VIEW_PICK);
            SetButtonID(btnOrbit, ID.ID_VIEW_ORBIT);
            SetButtonID(btnPanning, ID.ID_VIEW_PAN);
            SetButtonID(btnZoomOut, ID.ID_VIEW_ZOOMOUT);
            SetButtonID(btnZoomIn, ID.ID_VIEW_ZOOMIN);
            SetButtonID(btnFullScreen, ID.ID_VIEW_TOPVIEW);

            //툴바 체크 버튼들 집합
            m_arrToolBarCheckedButtons.Add(btnPick);
            m_arrToolBarCheckedButtons.Add(btnOrbit);
            m_arrToolBarCheckedButtons.Add(btnPanning);

            //버튼 위치 정렬
            ArrangeAdminRibbonButton();
            ArrangeToolbarRibbonButton();
            //기본값
            btnPick.IsChecked = true;

            SetButtonImage();

            btnDangerZoneLayer.Visible = false;
            btnStatusEnd.Enabled = false;



        }


        private void SetButtonID(UnE.GUI.RibbonButton btn, int nID, string strTooltipText = "")
        {
            m_dicButtonIDs[btn] = nID;
            m_dicIDButtons[nID] = btn;
            m_dicButtonChecked[btn] = false;
        }

        /// <summary>
        /// 관리메뉴리본버튼 체크
        /// </summary>
        /// <param name="rbbtn">대상 Ribbon버튼</param>
        public void CheckRibbonButtonAdmin(UnE.GUI.RibbonButton rbbtn)
        {
            int nID = GetButtonID(rbbtn);
            if (nID <= ID.ID_ADMIN_BEGIN || nID >= ID.ID_ADMIN_END)
                return;

            foreach (UnE.GUI.RibbonButton btn in m_arrAdminCheckedButtons)
            {
                if (btn == rbbtn)
                {
                    btn.IsChecked = true;
                    btn.Refresh();
                }
                else
                {
                    btn.IsChecked = false;
                    btn.Refresh();
                }
            }
        }

        public void ResetCheckRibbonButtonAdmin()
        {
            foreach (UnE.GUI.RibbonButton btn in m_arrAdminCheckedButtons)
            {
                if (btn.IsChecked == true)
                {
                    btn.IsChecked = false;
                    btn.Refresh();
                }
            }
        }

        /// <summary>
        /// 툴바 Ribbon버튼 체크
        /// </summary>
        /// <param name="rbbtn">대상 Ribbon버튼</param>
        private void CheckRibbonButtonToolBar(UnE.GUI.RibbonButton rbbtn)
        {
            int nID = GetButtonID(rbbtn);

            if (nID <= ID.ID_VIEW_BEGIN || nID >= ID.ID_VIEW_END)
                return;

            foreach (UnE.GUI.RibbonButton btn in m_arrToolBarCheckedButtons)
            {
                if (btn == rbbtn)
                {
                    btn.IsChecked = true;
                    btn.Refresh();
                }
                else
                {
                    btn.IsChecked = false;
                    btn.Refresh();
                }
            }
        }

        /// <summary>
        /// 모니터링 탭 선택
        /// </summary>
        private void SelectedMonitoringTab()
        {
            panelMonitoringTab.Visible = true;
            panelAdminTab.Visible = false;
            panelReportTab.Visible = false;

            pictureBoxMonitoring.BackgroundImage = global::HSMS.Properties.Resources.Tab_on;
            pictureBoxAdmin.BackgroundImage = m_bmpAdmin;
            pictureBoxReport.BackgroundImage = global::HSMS.Properties.Resources.Tab_off;

            panelAdminBar.Visible = true;
            panelReportBar.Visible = false;
            panelLeft.Visible = true;

            panelBottom.Location = new Point(panelLeft.Size.Width, panelLeft.Location.Y);
            Size size = panelBottom.Size; 
            panelBottom.Size = new Size(this.Size.Width - panelLeft.Width, size.Height);
            m_PageHome.Update();
        }

        /// <summary>
        /// 관리자 탭 선택했을때
        /// </summary>
        private void SelectedAdminTab()
        {
            panelMonitoringTab.Visible = false;
            panelAdminTab.Visible = true;
            panelReportTab.Visible = false;

            pictureBoxMonitoring.BackgroundImage = global::HSMS.Properties.Resources.Tab_off;
            pictureBoxAdmin.BackgroundImage = global::HSMS.Properties.Resources.Tab_on;
            pictureBoxReport.BackgroundImage = global::HSMS.Properties.Resources.Tab_off;

            panelAdminBar.Visible = true;
            panelReportBar.Visible = false;
            panelLeft.Visible = true;

            panelBottom.Location = new Point(panelLeft.Size.Width, panelLeft.Location.Y);
            Size size = panelBottom.Size; 
            panelBottom.Size = new Size(this.Size.Width - panelLeft.Width, size.Height);
            m_PageHome.Update();
        }

        /// <summary>
        /// 리포트 탭 선택 했을때
        /// </summary>
        private void SelectedReportTab()
        {
            btnReportHistory.IsChecked = true;

            panelMonitoringTab.Visible = false;
            panelAdminTab.Visible = false;
            panelReportTab.Visible = true;

            pictureBoxMonitoring.BackgroundImage = global::HSMS.Properties.Resources.Tab_off;
            pictureBoxAdmin.BackgroundImage = m_bmpAdmin;
            pictureBoxReport.BackgroundImage = global::HSMS.Properties.Resources.Tab_on;

            panelAdminBar.Visible = false;
            panelReportBar.Visible = true;
            panelLeft.Visible = false;
            
            Size size = panelBottom.Size;
            panelBottom.Location = new Point(0, panelBottom.Location.Y);
            panelBottom.Size = new Size(this.Size.Width, size.Height);

            btnSearch_Click(null, null);

            m_PageHome.Update();
        }

        public void TextPictureBox_MouseDown(UnE.GUI.TextPictureBox pictureBox, MouseEventArgs e)
        {
            if (e != null)
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left)
                    return;
            }

            if (pictureBox == pictureBoxMonitoring)
            {
                SelectedMonitoringTab();
                m_PageHome.FrmReport.Visible = false;
                panelLeft.Visible = true;
            }
            else if (pictureBox == pictureBoxAdmin)
            {
                SelectedAdminTab();
                m_PageHome.FrmReport.Visible = false;
                panelLeft.Visible = true;
            }
            else if (pictureBox == pictureBoxReport)
            {
                SelectedReportTab();
                m_PageHome.FrmReport.Visible = true;
                panelLeft.Visible = false;
            }
        }

        public void TextPictureBox_MouseUp(UnE.GUI.TextPictureBox pictureBox, MouseEventArgs e)
        {
        }

        /// <summary>
        /// 레이어 Ribbon 버튼, 3 State 설정
        /// </summary>
        /// <param name="rbButton">레이어 Ribbon 버튼</param>
        private void CheckLeftLayerButton(UnE.GUI.RibbonButton rbButton)
        {
            int nID = GetButtonID(rbButton);

            if (nID <= ID.ID_LAYER_BEGIN || nID >= ID.ID_LAYER_END)
                return;

            int nSelected = (int)rbButton.Tag;

            Bitmap selectIcon = GetLayerBtnImage(nID, nSelected);
            rbButton.BackgroundImage = selectIcon;



            // 현재 상태를 변경
            nSelected = ++nSelected >= 4 ? 1 : nSelected;
            rbButton.Tag = nSelected;

            switch (nSelected)
            {
                case 1:
                    rbButton.ForeColor = Color.FromArgb(169, 169, 169);
                    break;
                case 2:
                    rbButton.ForeColor = Color.FromArgb(169, 169, 169);
                    break;
                case 3:

                    rbButton.ForeColor = Color.FromArgb(70, 70, 70);
                    break;
            }

            HSMS.LayerType type = LayerType.Worker;
            switch (nID)
            {
                case ID.ID_LAYER_WORKER:
                    type = LayerType.Worker;
                    break;
                case ID.ID_LAYER_EQUIP:
                    type = LayerType.Equipment;
                    break;
                case ID.ID_LAYER_CAR:
                    type = LayerType.Vehicle;
                    break;
            }
            m_FormContent.SetLayerLOD(nSelected, type);
        }

        public int GetButtonID(UnE.GUI.RibbonButton btn)
        {
            if (m_dicButtonIDs.ContainsKey(btn))
                return m_dicButtonIDs[btn];

            return -1;
        }

        public UnE.GUI.RibbonButton GetButton(int nID)
        {
            if (m_dicIDButtons.ContainsKey(nID))
                return m_dicIDButtons[nID];

            return null;
        }

        private Dictionary<UnE.GUI.RibbonButton, Bitmap> clickedImage = new Dictionary<UnE.GUI.RibbonButton, Bitmap>();
        private Dictionary<UnE.GUI.RibbonButton, Bitmap> normalImage = new Dictionary<UnE.GUI.RibbonButton, Bitmap>();
        private void SetButtonImage()
        {
            clickedImage.Add(btnWorkerAdmin, global::HSMS.Properties.Resources.btnWorker_click);
            clickedImage.Add(btnVehicleAdmin, global::HSMS.Properties.Resources.btnVehicle_click);
            clickedImage.Add(btnFacilityAdmin, global::HSMS.Properties.Resources.btndangerFacility_click);
            clickedImage.Add(btnDangerZoneAdmin, global::HSMS.Properties.Resources.btndangerZone_click);
            clickedImage.Add(btnSaveAdmin, global::HSMS.Properties.Resources.btnSave_click);
            clickedImage.Add(btnDeleteAdmin, global::HSMS.Properties.Resources.btnDelete_click);
            clickedImage.Add(btnManagerAdmin, global::HSMS.Properties.Resources.btnManager_click);
            clickedImage.Add(btnMessageAdmin, global::HSMS.Properties.Resources.btnMessage_click);
            clickedImage.Add(btnAlarmAdmin, global::HSMS.Properties.Resources.btnAlarmDistance_click);
            clickedImage.Add(btnDetectAdmin, global::HSMS.Properties.Resources.btnDetect_click);
            clickedImage.Add(btnListAdmin, global::HSMS.Properties.Resources.btnList_click);
            clickedImage.Add(btnOption, global::HSMS.Properties.Resources.option_click);
            clickedImage.Add(btnReportHistory, global::HSMS.Properties.Resources.btnhistoryCheck_click);
            //clickedImage.Add(btnStatusEnd, global::HSMS.Properties.Resources.status_end_click);

            // Toolbar Image
            clickedImage.Add(btnSaveHome, global::HSMS.Properties.Resources.toolBar_click_BG);
            clickedImage.Add(btnHome, global::HSMS.Properties.Resources.toolBar_click_BG);
            clickedImage.Add(btnPick, global::HSMS.Properties.Resources.toolBar_click_BG);
            clickedImage.Add(btnOrbit, global::HSMS.Properties.Resources.toolBar_click_BG);
            clickedImage.Add(btnPanning, global::HSMS.Properties.Resources.toolBar_click_BG);
            clickedImage.Add(btnZoomOut, global::HSMS.Properties.Resources.toolBar_click_BG);
            clickedImage.Add(btnZoomIn, global::HSMS.Properties.Resources.toolBar_click_BG);
            clickedImage.Add(btnFullScreen, global::HSMS.Properties.Resources.toolBar_click_BG);


            normalImage.Add(btnWorkerAdmin, global::HSMS.Properties.Resources.btnWorker_nomal);
            normalImage.Add(btnVehicleAdmin, global::HSMS.Properties.Resources.btnVehicle_nomal);
            normalImage.Add(btnFacilityAdmin, global::HSMS.Properties.Resources.btndangerFacility_nomal);
            normalImage.Add(btnDangerZoneAdmin, global::HSMS.Properties.Resources.btndangerZone_nomal);
            normalImage.Add(btnSaveAdmin, global::HSMS.Properties.Resources.btnSave_nomal);
            normalImage.Add(btnDeleteAdmin, global::HSMS.Properties.Resources.btnDelete_nomal);
            normalImage.Add(btnManagerAdmin, global::HSMS.Properties.Resources.btnManager_nomal);
            normalImage.Add(btnMessageAdmin, global::HSMS.Properties.Resources.btnMessage_nomal);
            normalImage.Add(btnAlarmAdmin, global::HSMS.Properties.Resources.btnAlarmDistancenomal);
            normalImage.Add(btnDetectAdmin, global::HSMS.Properties.Resources.btnDetect_nomal);
            normalImage.Add(btnListAdmin, global::HSMS.Properties.Resources.btnList_nomal);
            normalImage.Add(btnOption, global::HSMS.Properties.Resources.option_normal);
            normalImage.Add(btnReportHistory, global::HSMS.Properties.Resources.btnhistoryCheck_nomal);
            // Toolbar Image
            normalImage.Add(btnSaveHome, null);
            normalImage.Add(btnHome, null);
            normalImage.Add(btnPick, null);
            normalImage.Add(btnOrbit, null);
            normalImage.Add(btnPanning, null);
            normalImage.Add(btnZoomOut, null);
            normalImage.Add(btnZoomIn, null);
            normalImage.Add(btnFullScreen, null);

        }

        private Image m_StatusBackImage = null;
        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
            UnE.GUI.RibbonButton btn = (UnE.GUI.RibbonButton)sender;

            if (btn == btnStatusEnd)
                m_StatusBackImage = btn.BackgroundImage;

            if (clickedImage.ContainsKey(btn))
            {
                btn.BackgroundImage = clickedImage[btn];
            }
        }


        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            UnE.GUI.RibbonButton rbButton = (UnE.GUI.RibbonButton)sender;


            if (normalImage.ContainsKey(rbButton))
            {
                rbButton.BackgroundImage = normalImage[rbButton];
            }

            if (rbButton == btnStatusEnd)
            {
                //rbButton.BackgroundImage = m_StatusBackImage;
            }
            //관리버튼
            CheckRibbonButtonAdmin(rbButton);

            //툴바버튼
            CheckRibbonButtonToolBar(rbButton);

            //왼쪽 레이어버튼 3단계
            CheckLeftLayerButton(rbButton);

            m_PageHome.OnCommandExcute(GetButtonID(rbButton));

            //if (rbButton == btnStatusEnd)
            //{
            //    rbButton.IsChecked = true;
            //}

            rbButton.Refresh();
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ResizePanel()
        {

            panelTop.Size = new Size(this.Size.Width, pictureBoxReport.Size.Height + panelMonitoringTab.Size.Height);
            panelTop.Location = new Point(0, 0);

            panelMonitoringTab.Size = new Size(panelTop.Size.Width, panelMonitoringTab.Size.Height);
            panelAdminTab.Size = panelMonitoringTab.Size;
            panelReportTab.Size = panelMonitoringTab.Size;

            
            panelLeft.Size = new Size(panelLeft.Size.Width, this.Size.Height - panelTop.Size.Height - panelAdminBar.Size.Height);

            int height = this.Size.Height - (pictureBoxReport.Size.Height + panelMonitoringTab.Size.Height) - panelMiddle.Size.Height;


            panelAdminBar.Size = new Size(this.Size.Width, panelAdminBar.Size.Height);
            panelReportBar.Size = panelAdminBar.Size;

            if(panelReportTab.Visible == true)
                panelBottom.Size = new Size(this.Size.Width, height);
            else
                panelBottom.Size = new Size(this.Size.Width - panelLeft.Width, height);
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            ResizePanel();
        }

        private void ArrangeToolbarRibbonButton()
        {
            ArrangeRibbonButton(btnSaveHome, btnHome);
            ArrangeRibbonButton(btnHome, btnFullScreen);
            ArrangeRibbonButton(btnFullScreen, pictureBox6, btnPick);
            ArrangeRibbonButton(btnPick, btnOrbit);
            ArrangeRibbonButton(btnOrbit, btnPanning);
            ArrangeRibbonButton(btnPanning, pictureBox7, btnZoomOut);
            ArrangeRibbonButton(btnZoomOut, btnZoomIn);
        }

        private void ArrangeAdminRibbonButton()
        {
            btnSaveAdmin.Visible = false;
            btnDeleteAdmin.Visible = false;
            pictureBox11.Visible = false;

            ArrangeRibbonButton(btnWorkerAdmin, btnVehicleAdmin);
            ArrangeRibbonButton(btnVehicleAdmin, btnFacilityAdmin);
            ArrangeRibbonButton(btnFacilityAdmin, btnDangerZoneAdmin);
            ArrangeRibbonButton(btnDangerZoneAdmin, pictureBox10, btnManagerAdmin);
            //ArrangeRibbonButton(btnSaveAdmin, btnDeleteAdmin);
            //ArrangeRibbonButton(btnDeleteAdmin, pictureBox11, btnManagerAdmin);
            ArrangeRibbonButton(btnManagerAdmin, btnMessageAdmin);
            ArrangeRibbonButton(btnMessageAdmin, btnAlarmAdmin);
            ArrangeRibbonButton(btnAlarmAdmin, btnDetectAdmin);
            ArrangeRibbonButton(btnDetectAdmin, pictureBox12, btnListAdmin);
            ArrangeRibbonButton(btnListAdmin, btnOption);
        }

        private void ArrangeRibbonButton(UnE.GUI.RibbonButton btnPrev, UnE.GUI.RibbonButton btnNext)
        {
            btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width, btnPrev.Location.Y);
        }

        private void ArrangeRibbonButton(UnE.GUI.RibbonButton btnPrev, PictureBox pictureBoxMiddle, UnE.GUI.RibbonButton btnNext)
        {
            pictureBoxMiddle.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width, pictureBoxMiddle.Location.Y);
            btnNext.Location = new Point(pictureBoxMiddle.Location.X + pictureBoxMiddle.Size.Width, btnPrev.Location.Y);
        }

        //타이머(시간표시)
        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                labelDate.Text = string.Format("{0}년 {1}월 {2}일", dtNow.Year, dtNow.Month, dtNow.Day);
                labelTime.Text = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);
            }
            catch(Exception)
            {

            }
          
        }

        public void ClearAlarm()
        {
            

            if (m_alarmStatus == null)
                m_alarmStatus = new AlarmState();

            m_alarmStatus.ClearScreen = true;

            this.Invoke((MethodInvoker)delegate
            {
                cboAlarmList.Items.Clear();
                SelectPOI(null);
            });

            if (m_lockMessage)
                return;
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    SetDangerLevel(0);

                    btnStatusEnd.IsChecked = false;
                    panelTop.BackgroundImage = global::HSMS.Properties.Resources.PanelTop_skin;
                    pictureBoxbell.BackgroundImage = global::HSMS.Properties.Resources.state_img;

                    lblDangerDetail.Text = "";
                    lblDangerLevel.Text = "위험 상황 없음";
                    SetInfoMessage("");

                    StopSound();
                });
            }
        }

        private int m_nStateCount = 0;
        private void AddAlarm(DangerState state)
        {
            if (state == null)
                return;

            foreach (DangerState _state in cboAlarmList.Items)
            {
                if (state == _state)
                    return;
            }

            cboAlarmList.Items.Add(state);
            m_nStateCount++;
            lbCount.Text = "현재 상황 : " + m_nStateCount + "개";

            cboAlarmList.SelectedIndex = cboAlarmList.Items.Count - 1;
        }

        public void RemoveAlarm(DangerState state)
        {
            if (state == null)
                return;

           
            if (state.Worker != null && state.Worker.SensorWorker != null)
                state.Worker.SensorWorker.ClearSetAccidentText();
            

            cboAlarmList.Items.Remove(state);
            
            if (m_nStateCount <= 0)
                m_nStateCount = 0;
            else
            {
                m_nStateCount--;
            }
            lbCount.Text = "현재 상황 : " + m_nStateCount + "개";

            if (cboAlarmList.Items.Count > 0)
                cboAlarmList.SelectedIndex = 0;
            else
                ClearAlarm();
        }


        private bool m_bShowExclusiveAccidentText = true;
        public void ResetAllAccidentText()
        {
            ArrayList arWorkers = Core.WorkerManager.Instance.Workers;
            foreach(Core.Worker worker in arWorkers)
            {
                worker.ClearSetAccidentText();
            }
        }

        public void SetAlarm(bool isCritical, string strDetail, string strAlarmStatus, string strMessage, string strShortMessage, DangerState state)
        {
            if (cboAlarmList.Items.Contains(state))
            {
                DangerState _state = (DangerState)cboAlarmList.Items[cboAlarmList.SelectedIndex];

                // 이미 ComboBox에 포함되어 있는 알람이면서, 현재 화면에 표시중인 알람이 아닐 경우 무시한다.
                if (_state != state)
                    return;
            }

            if (m_alarmStatus == null)
                m_alarmStatus = new AlarmState();

            m_alarmStatus.Status = strAlarmStatus;
            m_alarmStatus.ClearScreen = false;
            m_alarmStatus.Detail = strDetail;
            m_alarmStatus.IsCritical = isCritical;
            m_alarmStatus.Message = strMessage;
            m_alarmStatus.ShortMessage = strShortMessage;
            m_alarmStatus.DangerState = state;

            if (state.Worker != null)
            {
                if (m_bShowExclusiveAccidentText == true)
                {
                    ResetAllAccidentText();
                }
                state.Worker.SensorWorker.SetAccidentText(strShortMessage);
                state.Worker.SensorWorker.ToggleText(false);
                
            }
             

            AddAlarm(state);

            if (m_lockMessage)
                return;
            else
            {
                if (isCritical)
                {
                    if (SetDangerLevel(2))
                    {
                        panelTop.BackgroundImage = global::HSMS.Properties.Resources.PanelTop_skin_Level2;
                        pictureBoxbell.BackgroundImage = global::HSMS.Properties.Resources.state_img_Level2;
                        PlaySoundCritical();
                    }
                }
                else
                {
                    if (SetDangerLevel(1))
                    {
                        panelTop.BackgroundImage = global::HSMS.Properties.Resources.PanelTop_skin_Level1;
                        pictureBoxbell.BackgroundImage = global::HSMS.Properties.Resources.state_img_Level1;
                        PlaySound();
                    }
                }

                if (lblDangerDetail.Text != strDetail)
                    lblDangerDetail.Text = strDetail;
                if (lblDangerLevel.Text != strAlarmStatus)
                    lblDangerLevel.Text = strAlarmStatus;

                //SendSMS(FormMain.Instance.DataMgr.Caller, FormMain.Instance.DataMgr.Receiver, strMessage, state.AlarmProcessHistoryID);
                SetInfoMessage(strMessage);
                SelectPOI(state);
            }
        }

        private void SendSMS(string strCaller, string strReceiver, string strMsg, int nAlarmProcessHistoryID)
        {
            string strFilePath = GetSMSSenderPath();

            if (strFilePath == null)
            {
                MessageBox.Show("sms.exe를 찾을수 없습니다.");
                return;
            }

            string strMessage = "[HSMS]{" + nAlarmProcessHistoryID.ToString() + "}번 알림 발생 - {" + strMsg + "}";

            String arg0 = "--charset=EUC-KR --means=11 --key=612389fc5e1612525e7a1d8ffeeaf4ac --unique=547896512612";
            String arg1 = " --callback=" + strCaller;//발신자
            String arg2 = " --phone=" + strReceiver;//수신자
            String arg3 = " --message=\"" + strMessage + "\""; //메시지

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = strFilePath;
            startInfo.ErrorDialog = true;
            startInfo.Arguments = arg0 + arg1 + arg2 + arg3;

            System.Diagnostics.Process process;

            try
            {
                process = System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private string GetSMSSenderPath()
        {
            int nIndex = Application.ExecutablePath.LastIndexOf('\\');

            if (nIndex < 0)
                return null;

            string strPath = Application.ExecutablePath.Substring(0, nIndex + 1) + "sms.exe";

            if (System.IO.File.Exists(strPath))
                return strPath;

            return null;
        }

        private void PlaySound()
        {
            string szWavPath = Application.StartupPath + "\\FireSignalAlarm.WAV";
            if (System.IO.File.Exists(szWavPath))
            {
                m_player.SoundLocation = szWavPath;
                m_player.Play();
            }
        }
        private void PlaySoundCritical()
        {
            string szWavPath = Application.StartupPath + "\\FireSignalAlarm2.WAV";
            if (System.IO.File.Exists(szWavPath))
            {
                m_player.SoundLocation = szWavPath;
                m_player.Play();
            }
        }

        private void StopSound()
        {
            m_player.Stop();
        }

        private void SelectPOI(DangerState state)
        {
            bool changed = false;

            if (state == null)
            {
                SelectPOI(m_selectedWorkerPOI, false, ref changed);

                if (m_selectedTargetPOI != null)
                {
                    Type type = m_selectedTargetPOI.GetType();
                    changed = true;

                    if (type == typeof(DataEquip))
                    {
                        DataEquip equip = (DataEquip)m_selectedTargetPOI;
                        SelectPOI(equip, false, ref changed);
                    }
                    else if (type == typeof(DataCar))
                    {
                        DataCar car = (DataCar)m_selectedTargetPOI;
                        SelectPOI(car, false, ref changed);
                    }
                    else if (type == typeof(DataZone))
                    {
                        DataZone zone = (DataZone)m_selectedTargetPOI;
                        SelectPOI(zone, false, ref changed);
                    }
                }

                m_selectedWorkerPOI = null;
                m_selectedTargetPOI = null;
            }
            else
            {
                if (m_selectedWorkerPOI != state.Worker)
                {
                    SelectPOI(m_selectedWorkerPOI, false, ref changed);
                    SelectPOI(state.Worker, true, ref changed);

                    m_selectedWorkerPOI = state.Worker;
                }

                if (m_selectedTargetPOI != null)
                {
                    Type type = m_selectedTargetPOI.GetType();

                    if (type == typeof(DataEquip))
                    {
                        DataEquip equip = (DataEquip)m_selectedTargetPOI;

                        if (equip != state.TargetEquipment)
                            SelectPOI(equip, false, ref changed);
                    }
                    else if (type == typeof(DataCar))
                    {
                        DataCar car = (DataCar)m_selectedTargetPOI;

                        if (car != state.TargetCar)
                            SelectPOI(car, false, ref changed);
                    }
                    else if (type == typeof(DataZone))
                    {
                        DataZone zone = (DataZone)m_selectedTargetPOI;

                        if (zone != state.TargetZone)
                            SelectPOI(zone, false, ref changed);
                    }

                    m_selectedTargetPOI = null;
                }

                if (state.TargetCar != null)
                {
                    SelectPOI(state.TargetCar, true, ref changed);
                    m_selectedTargetPOI = state.TargetCar;
                }
                else if (state.TargetEquipment != null)
                {
                    SelectPOI(state.TargetEquipment, true, ref changed);
                    m_selectedTargetPOI = state.TargetEquipment;
                }
                else if (state.TargetZone != null)
                {
                    SelectPOI(state.TargetZone, true, ref changed);
                    m_selectedTargetPOI = state.TargetZone;
                }
            }

            if (changed)
                m_FormContent.Update3DView();
        }

        private void SelectPOI(DataWorker worker, bool selected, ref bool isChanged)
        {
            if (worker != null)
            {
                if (worker.SensorWorker != null)
                {
                    if (selected)
                        worker.SensorWorker.Select();
                    else
                        worker.SensorWorker.ClearSelect();

                    isChanged = true;
                }
            }
        }

        private void SelectPOI(DataCar car, bool selected, ref bool isChanged)
        {
            if (car != null)
            {
                if (car.SensorVehicle != null)
                {
                    if (selected)
                        car.SensorVehicle.Select();
                    else
                        car.SensorVehicle.ClearSelect();

                    isChanged = true;
                }
            }
        }

        private void SelectPOI(DataEquip equip, bool selected, ref bool isChanged)
        {
            if (equip != null)
            {
                /*Core.MovingEquipment movingEquip = m_DataMgr.FindMovingEquipment(equip);

                if (movingEquip != null)
                {
                    if (selected)
                        movingEquip.Select();
                    else
                        movingEquip.ClearSelect();

                    isChanged = true;
                }*/
                if (equip.Linked3DEquipment != null)
                {
                    if (selected)
                        equip.Linked3DEquipment.Select();
                    else
                        equip.Linked3DEquipment.Unselect();

                    isChanged = true;
                }
            }
        }

        private void SelectPOI(DataZone zone, bool selected, ref bool isChanged)
        {
            if (zone != null)
            {
                if (m_FormContent.OutdoorView.ZoneVolumes.ContainsKey(zone.ZoneName))
                {
                    Core.ZoneVolume volume = m_FormContent.OutdoorView.ZoneVolumes[zone.ZoneName];
                    volume.SetVisible(selected);
                    isChanged = true;
                }
            }
        }

        public void MakeSensorOwner()
        {
            int nWorkerCount = m_DataMgr.GetWorkerCount();
            int nCarCount = m_DataMgr.GetCarCount();
            int nEquipCount = m_DataMgr.GetEquipCount();

            for (int i = 0; i < nWorkerCount; i++)
            {
                DataWorker worker = m_DataMgr.GetWorker(i);

                if (worker.Sensor.Length > 0)
                    m_FormContent.AddWorker(worker);
            }

            for (int i = 0; i < nCarCount; i++)
            {
                DataCar car = m_DataMgr.GetCar(i);

                if (car.Sensor.Length > 0)
                    m_FormContent.AddVehicle(car);
            }

            for (int i = 0; i < nEquipCount; i++)
            {
                DataEquip equip = m_DataMgr.GetEquip(i);

                _3DEquipment _3dEquip = equip.SetLiked3DEquipmentFromName();
                
                if (_3dEquip != null)
                {
                    Type type = _3dEquip.GetType();

                    m_bUseDB = ModelManager.Instance.UseDB;
                    if (m_bUseDB == false)
                    {
                        if (type == typeof(MovingEquip3D))
                            SetMovingEquipOption(equip);
                        else if (type == typeof(Crane3D))
                            SetCraneOption(equip);
                    }

                   

                    SetLinked3DEquipmentMovingArea(equip.Linked3DEquipment, equip.SensorPosition, equip.SensorFinishPosition);
                }
            }
        }

        private void SetLinked3DEquipmentMovingArea(_3DEquipment equip3D, UnE.Geometry.Vertex2D vSensorPos, UnE.Geometry.Vertex2D vSensorFinishPos)
        {
            if (equip3D != null)
            {
                if (equip3D.GetType() == typeof(MovingEquip3D))
                {
                    if (vSensorPos.x < vSensorFinishPos.x)
                    {
                        equip3D.MinMovedX = new PrimitiveData<double>(vSensorPos.x);
                        equip3D.MaxMovedX = new PrimitiveData<double>(vSensorFinishPos.x);
                    }
                    else
                    {
                        equip3D.MinMovedX = new PrimitiveData<double>(vSensorFinishPos.x);
                        equip3D.MaxMovedX = new PrimitiveData<double>(vSensorPos.x);
                    }

                    if (vSensorPos.y < vSensorFinishPos.y)
                    {
                        equip3D.MinMovedY = new PrimitiveData<double>(vSensorPos.y);
                        equip3D.MaxMovedY = new PrimitiveData<double>(vSensorFinishPos.y);
                    }
                    else
                    {
                        equip3D.MinMovedY = new PrimitiveData<double>(vSensorFinishPos.y);
                        equip3D.MaxMovedY = new PrimitiveData<double>(vSensorPos.y);
                    }
                }
                else if (equip3D.GetType() == typeof(Crane3D))
                {
                    if (vSensorPos.x < vSensorFinishPos.x)
                    {
                        equip3D.MinMovedX = new PrimitiveData<double>(vSensorPos.x);
                        equip3D.MaxMovedX = new PrimitiveData<double>(vSensorFinishPos.x);
                    }
                    else
                    {
                        equip3D.MinMovedX = new PrimitiveData<double>(vSensorFinishPos.x);
                        equip3D.MaxMovedX = new PrimitiveData<double>(vSensorPos.x);
                    }
                }
            }
        }

        private void SetCraneOption(DataEquip equip)
        {
            if (equip.Sensor.Length > 0)
            {
                string strEquipName = equip.Name.ToLower();
                
                int num = 0;
                bool beginNumber = false;

                for (int i=0;i<strEquipName.Length;i++)
                {
                    char ch = strEquipName.ElementAt(i);

                    if (beginNumber)
                    {
                        if (ch >= '0' && ch <= '9')
                            num = num * 10 + (int)(ch - '0');
                        else
                            break;
                    }
                    else
                    {
                        if (ch >= '0' && ch <= '9')
                        {
                            num = (int)(ch - '0');
                            beginNumber = true;
                        }
                    }
                }

                m_bUseDB = ModelManager.Instance.UseDB;
                if (m_bUseDB == false)
                {

                    Core.Crane crane = Core.CraneManager.Instance.GetCrane(num - 1);

                    if (crane != null)
                    {
                        Crane3D crane3D = (Crane3D)equip.Linked3DEquipment;
                        crane3D.Crane = crane;
                    }
                }
            }
        }

        private bool m_bUseDB = false;

        private void SetMovingEquipOption(DataEquip equip)
        {
            if (equip.Sensor.Length > 0)
            {
                Core.MovingEquipment movingEquip = m_FormContent.AddMovingEquipment();

                if (movingEquip != null)
                {
                    float fMaxValue = (float)equip.SensorPosition.GetDistance(equip.SensorFinishPosition);
                    movingEquip.SetMinValue(0.0f);
                    movingEquip.SetMaxValue(fMaxValue);

                    MovingEquip3D equip3D = (MovingEquip3D)equip.Linked3DEquipment;
                    equip3D.MovingEquipment = movingEquip;
                }
            }
        }

        public void OnReceiveSensorLocation(string strSensorID, double x, double y)
        {
            /*lock (m_safetyChecker)
            {
                m_safetyChecker.AddSensorHistory(strSensorID, new EventSensorData(strSensorID, DateTime.Now, x, y));
            }*/

            DataWorker worker = m_DataMgr.FindWorker2(strSensorID);

            if (worker != null)
            {
                SensorWorker sWorker = worker.SensorWorker;

                if (sWorker != null)
                {
                    if(worker.SensorDetect == true)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            sWorker.OnVisible(true);

                            //System.Diagnostics.Trace.WriteLine(strSensorID + ", Location : " + x.ToString() + ", " + y.ToString());
                            sWorker.SetLocation((float)x, 0.0f, (float)-y);
                            m_FormContent.Update3DView();
                        });
                    }
                    else
                    {
                        sWorker.OnVisible(false);
                    }
                }
            }
            else
            {
                DataCar car = m_DataMgr.FindCar2(strSensorID);

                if (car != null)
                {
                    SensorVehicle vehicle = car.SensorVehicle;

                    if (vehicle != null)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            vehicle.OnVisible(true);

                            //System.Diagnostics.Trace.WriteLine(strSensorID + ", Location : " + x.ToString() + ", " + y.ToString());
                            vehicle.SetLocation((float)x, 0.0f, (float)y);
                            m_FormContent.Update3DView();
                        });
                    }
                }
                else
                {
                    DataEquip equip = m_DataMgr.FindEquip2(strSensorID);

                    if (equip != null)
                    {
                        equip.Moved.x = x - (equip.SensorPosition.x + equip.OriginPosition.x);
                        equip.Moved.y = y - (equip.SensorPosition.y + equip.OriginPosition.y);

                        if (equip.Linked3DEquipment != null)
                        {
                            equip.Linked3DEquipment.SetPosition((float)equip.Moved.x, (float)equip.Moved.y, 0.0f);

                            //if (equip.Linked3DEquipment.GetType() != typeof(Crane3D))
                            {
                                m_FormContent.Update3DView();
                            }
                        }

                        /*Core.MovingEquipment movingEquip = m_DataMgr.FindMovingEquipment(equip);

                        if (movingEquip != null)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                movingEquip.OnVisible(true);

                                float fDistance = (float)equip.GetMovingDistance(equip.Moved.x, equip.Moved.y);

                                System.Diagnostics.Trace.WriteLine(strSensorID + ", Location : " + x.ToString() + ", " + y.ToString());
                                System.Diagnostics.Trace.WriteLine(strSensorID + ", Location : " + equip.Moved.x.ToString() + ", " + equip.Moved.y.ToString() + ", distance : " + fDistance.ToString());

                                movingEquip.SetLocation(fDistance);
                                //movingEquip.SetLocation((float)-equip.Moved.y);
                                m_FormContent.Update3DView();
                            });
                        }*/
                    }
                }
            }
        }

        public void RemoveSensor(string strSensorID)
        {
            DataWorker worker = m_DataMgr.FindWorker2(strSensorID);

            if (worker != null)
            {
                SensorWorker sWorker = worker.SensorWorker;
                m_alarmMgr.RemoveAlarm(worker);

                if (sWorker != null)
                {
                    sWorker.OnVisible(false);
                }
            }
            else
            {
                DataCar car = m_DataMgr.FindCar2(strSensorID);

                if (car != null)
                {
                    m_alarmMgr.RemoveAlarm(car, null, null);
                    SensorVehicle vehicle = car.SensorVehicle;

                    if (vehicle != null)
                    {
                        vehicle.OnVisible(false);
                    }
                }
                else
                {
                    DataEquip equip = m_DataMgr.FindEquip2(strSensorID);

                    if (equip != null)
                    {
                        m_alarmMgr.RemoveAlarm(null, equip, null);
                    }
                }
            }
        }

        private void cboAlarmList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboAlarmList.SelectedIndex < 0)
                return;

            DangerState state = (DangerState)cboAlarmList.Items[cboAlarmList.SelectedIndex];

            bool isCritical;
            string strWorkerInfo, strAlarmStatus, strMessage, strShortMessage;

            if (m_alarmMgr.GetAlarmMessage(state, state.Worker, out strWorkerInfo, out strAlarmStatus, out strMessage, out strShortMessage, out isCritical))
            {
                btnStatusEnd.Enabled = true;
                SetAlarm(isCritical, strWorkerInfo, strAlarmStatus, strMessage, strShortMessage, state);
            }
        }

        private void btnStatusEnd_Click(object sender, EventArgs e)
        {
            if (cboAlarmList.SelectedIndex < 0 || cboAlarmList.Items.Count == 0)
            {
                btnStatusEnd.Enabled = false;
                return;
            }

            DangerState state = (DangerState)cboAlarmList.Items[cboAlarmList.SelectedIndex];
            if (state != null)
            {
                cboAlarmList.Items.Remove(state);

                string strSensorID;
                int nGasType;

                if (state.Worker == null && m_alarmMgr.RemoveGasAlarm(state, out strSensorID, out nGasType))
                    m_netMgr.ClientProvider.SendFinishGasAlarm(strSensorID, nGasType);
                else
                    m_netMgr.ClientProvider.SendFinishAlarm(state);

                int nIdx = cboAlarmList.Items.Count - 1;
                if (nIdx < 0)
                {
                    btnStatusEnd.Enabled = false;
                    ClearAlarm();
                    return;
                }
                cboAlarmList.SelectedIndex = nIdx;
            }
        }

        private void panelAdminTab_Paint(object sender, PaintEventArgs e)
        {

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

        private void button1_Click(object sender, EventArgs e)
        {
            monthCalendar1.Visible = true;

            monthCalendar1.SetBounds(btnReportDateStart.Location.X , btnReportDateStart.Location.Y - panelReportBar.Size.Height + btnReportDateStart.Size.Height, monthCalendar1.Size.Width, monthCalendar1.Size.Height);
        }

        private void btnReportDateEnd_Click(object sender, EventArgs e)
        {
            monthCalendar2.Visible = true;

            monthCalendar2.SetBounds(btnReportDateEnd.Location.X , btnReportDateEnd.Location.Y - panelReportBar.Size.Height + btnReportDateEnd.Size.Height, monthCalendar2.Size.Width, monthCalendar2.Size.Height);
        }

        //private bool bLoad1 = false;
        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            //if (bLoad1 == false)
            //{
            //    btnReportDateStart.Text = monthCalendar1.SelectionStart.ToShortDateString();
            //    bLoad1 = true;
            //    return;
            //}

            //string strStartDate = e.Start.ToString();
            //string strEndDate = btnReportDateEnd.Text;
            //DateTime dtStartDate = DateTime.ParseExact(strStartDate, "yyyy-MM-dd", null);
            //DateTime dtEndDate = DateTime.ParseExact(strEndDate, "yyyy-MM-dd", null);

            //if(dtStartDate > dtEndDate)
            //{
            //    MessageBox.Show("시작 날짜가 더 클 수 없습니다.");
            //    return;
            //}
            //else if(dtStartDate > DateTime.Now)
            //{
            //    MessageBox.Show("오늘 날짜보다 더 클 수 없습니다.");
            //    return;
            //}


            btnReportDateStart.Text = monthCalendar1.SelectionStart.ToShortDateString();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string strStartDate = btnReportDateStart.Text;
            string strEndDate = btnReportDateEnd.Text;
            string strAlarmStep = cboAlarmStep.Text;

            DateTime dtStartDate = DateTime.ParseExact(strStartDate, "yyyy-MM-dd",null);
            DateTime dtEndDate = DateTime.ParseExact(strEndDate, "yyyy-MM-dd", null);

            //m_DataReport.SetDetectedHistory(dtStartDate, dtEndDate, strAlarmStep);
            m_DataReport.ProcessSearchData(dtStartDate, dtEndDate, strAlarmStep);
            m_PageHome.FrmReport.SetDataGridView(strStartDate, strEndDate, strAlarmStep);
            m_PageHome.FrmReport.CreateLineChart(dtStartDate, dtEndDate);
        }

        private void cboReportLatelyDate_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;

            if (cboReportLatelyDate.SelectedIndex == 0)
            {
                dtStart = dtStart.AddMonths(-6);
            }
            else if (cboReportLatelyDate.SelectedIndex == 1)
            {
                dtStart = dtStart.AddMonths(-3);
            }
            else if (cboReportLatelyDate.SelectedIndex == 2)
            {
                dtStart = dtStart.AddMonths(-1);
            }

            btnReportDateStart.Text = dtStart.ToShortDateString();
            //monthCalendar1.TodayDate = dtStart;
            monthCalendar1.SetDate(dtStart); 

            btnReportDateEnd.Text = dtEnd.ToShortDateString();
            monthCalendar2.TodayDate = dtEnd;
            monthCalendar2.SetDate(dtEnd);
        }

        private void monthCalendar1_Leave(object sender, EventArgs e)
        {
            monthCalendar1.Visible = false;
            
        }

        private void monthCalendar1_MouseCaptureChanged(object sender, EventArgs e)
        {
            int i = 0;
            i++;
        }

        private void monthCalendar1_Enter(object sender, EventArgs e)
        {
           
        }

        Timer tCal = new Timer();
        Timer tCal2 = new Timer();
        private void monthCalendar1_MouseLeave(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Interval = 100;
            timer1.Start();
        }

        private int nTimerCount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if( nTimerCount == 2)
            {
                nTimerCount = 0;
                monthCalendar1.Visible = false;

                timer1.Enabled = false;
                timer1.Stop();
            }
            nTimerCount++;
        }

        private void monthCalendar1_MouseEnter(object sender, EventArgs e)
        {
            nTimerCount = 0;
            timer1.Enabled = false;
            timer1.Stop();
        }

        private void monthCalendar2_MouseLeave(object sender, EventArgs e)
        {
            timer2.Enabled = true;
            timer2.Interval = 100;
            timer2.Start();
        }

        private int nTimerCount2 = 0;
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (nTimerCount2 == 2)
            {
                nTimerCount2 = 0;
                monthCalendar2.Visible = false;

                timer2.Enabled = false;
                timer2.Stop();
            }
            nTimerCount2++;
        }

        private void monthCalendar2_Leave(object sender, EventArgs e)
        {
            monthCalendar2.Visible = false;
        }

        private void monthCalendar2_MouseEnter(object sender, EventArgs e)
        {
            nTimerCount = 0;
            timer2.Enabled = false;
            timer2.Stop();
        }

        //private bool bLoad2 = false;
        private void monthCalendar2_DateChanged(object sender, DateRangeEventArgs e)
        {
            //if(bLoad2 == false)
            //{
            //    btnReportDateEnd.Text = monthCalendar2.SelectionStart.ToShortDateString();
            //    bLoad2 = true;
            //    return;
            //}

            //string strStartDate = btnReportDateStart.Text;
            //string strEndDate = btnReportDateEnd.Text;
            //DateTime dtStartDate = DateTime.ParseExact(strStartDate, "yyyy-MM-dd", null);
            //DateTime dtEndDate = DateTime.ParseExact(strEndDate, "yyyy-MM-dd", null);

            //if (dtStartDate > dtEndDate)
            //{
            //    MessageBox.Show("시작 날짜가 더 클 수 없습니다.");
            //    return;
            //}
            //else if (dtEndDate > DateTime.Now)
            //{
            //    MessageBox.Show("오늘 날짜보다 더 클 수 없습니다.");
            //    return;
            //}

            btnReportDateEnd.Text = monthCalendar2.SelectionStart.ToShortDateString();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (commander != null)
            {
                if (tmrUpdate != null)
                {
                    tmrUpdate.Stop();
                    tmrUpdate.Enabled = false;
                }
                
                commander.StopCommander();
                commander.Dispose();
            }
            
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.Y <= pictureBoxReport.Location.Y + pictureBoxReport.Size.Height)
                    FormFrame.Instance.TitleBarMouseDown(e, Control.MousePosition);
            }
        }

        private void panelTop_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                FormFrame.Instance.TitleBarMouseUp(e);
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            FormFrame.Instance.TitleBarMouseDrag(e, Control.MousePosition);
        }

        private void panelTop_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.Y <= pictureBoxReport.Location.Y + pictureBoxReport.Size.Height)
                    FormFrame.Instance.TitleBarMouseDoubleClick();
            }
        }

        public void Update3DView()
        {
            m_FormContent.Invalidate3DView(true);
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            //m_FormContent.SetAccidentText();
        }

        public FormContent Get3DView()
        {
            return m_FormContent;
        }
    }

    public class SoundPlayerEx : System.Media.SoundPlayer
    {
        private bool m_isPlaying = false;

        public new void Play()
        {
            if (m_isPlaying)
                Stop();

            m_isPlaying = true;
            base.PlayLooping();
        }

        public new void Stop()
        {
            base.Stop();
            m_isPlaying = false;
        }

        protected override void Dispose(bool disposing)
        {
            Stop();
            base.Dispose(disposing);
        }
    }
}
