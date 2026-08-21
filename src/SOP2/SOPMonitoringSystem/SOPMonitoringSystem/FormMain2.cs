using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.GUI;
using System.Collections;
using System.Threading;
using UnE.Geometry;
using UnE.Utility;
using SOP;

namespace SOPMonitoringSystem
{
    using Process;
    using Sections;

    public partial class FormMain : Form, IRibbonButtonOwner, ITextPictureBoxOwner, SOPDisasterSystem.ISOPInfo
    {
        // Button별 ID
        private Dictionary<Button, int> m_dicButtonIDs = new Dictionary<Button, int>();
        private Dictionary<int, Button> m_dicIDButtons = new Dictionary<int, Button>();
        private Dictionary<Button, bool> m_dicButtonChecked = new Dictionary<Button, bool>();

        // Form Move 를 위한 Panel Top 클릭 체크
        private bool m_bLeftMouseDown = false;
        // Form Move 를 위한 Panel Top 클릭 점
        private Point m_ptMove;

        private int m_nPanelTopInitHeight = -1;

        private SOPLog m_logFile = new SOPLog();
        public SOPLog LogFile
        {
            get { return m_logFile; }
        }

        private SOPManager m_sopMgr = null;
        private static IntPtr instanceHandle = IntPtr.Zero;
        private MP3Player m_player = new MP3Player();

        private bool m_CloseThread = false;

        public bool CloseThread
        {
            get { return m_CloseThread; }
            set { m_CloseThread = value; }
        }

        private PageBackstageHome m_pageHome;
        private PageBackstageOption m_pageOption;
        private PageBackStageMessage m_pageMessage;

        private bool m_isFirst = false;
        private bool m_isOpen = false;
        public bool isOpen
        {
            get { return m_isOpen; }
            set { m_isOpen = value; }
        }
        private WebDBManager m_dbMgr = null;

        private string m_strVersion = "V1.5";
        private bool m_isReadVersion = false;

        private FormStatus m_frmStatus = null;
        //private FormRealTimeInfo m_frmReal = null;
        private SOPDisasterSystem.FormMain m_frmMain2 = null;
        public SOPDisasterSystem.FormMain FrmMain2
        {
            get { return m_frmMain2; }
            set { m_frmMain2 = value; }
        }

        private FormMissionStatus m_frmMain3 = null;
        public FormMissionStatus FrmMain3
        {
            get { return m_frmMain3; }
        }

        private PopupProgressReport m_frmReport = null;

        private Sections.WorkFlow m_currentWork = null;
        public Sections.WorkFlow CurrentWork
        {
            get { return m_currentWork; }
        }
        private bool DoorBellChecked = true; // 초기 DoorBellChecked

        private Sections.Section m_currentSection = null;
        public Sections.Section CurrentSection
        {
            get { return m_currentSection; }
            set { m_currentSection = value; }
        }

        /*private int m_nChangeUserID;
        public int ChangeUserID
        {
            get { return m_nChangeUserID; }
            set { m_nChangeUserID = value; }
        }*/

        private SDMS.NetworkManager m_netMgr = null;

        public SDMS.NetworkManager NetworkManager
        {
            get { return m_netMgr; }
        }

        //////////////////////////////////////////////////////////////////////////
        static private FormMain m_instance = null;

        static public FormMain Instance
        {
            get { return m_instance; }
        }

        //private int m_nSirenCount = 0;
        private int m_nSOPGenUserID = -1;
        private int m_nSOPGenUserLevel = 1;

        private string m_strSOPGenUserRealName = "";

        private ArrayList m_arrConnectedUser = new ArrayList();
        private Thread DBWrite = null;
        //private bool m_isStop = false; // 쓰레드를 중지하고자 할때 true

        // 제어권 요청으로 인하여 WorkerThread가 일시 정지된 상태인가?
        //private bool m_isWorkingTempThread = false;

        private bool m_smsOn = false;
        // 협력업체들에게도 SOP 시작 및 종료시 문자메시지를 보낼것인가?
        private bool m_smsExternalCompanyMemberOn = false;

        private bool m_useEzSMS = false;
        public bool UseEzSMS
        {
            get { return m_useEzSMS; }
            set { m_useEzSMS = value; }
        }

        private bool m_useBroadcast = false;
        public bool UseBroadcast
        {
            get { return m_useBroadcast; }
            set { m_useBroadcast = value; }
        }

        private bool m_showMissionText = false;
        public bool ShowMissionText
        {
            get { return m_showMissionText; }
            set { m_showMissionText = value; }
        }

        // Workflow가 Event와 함께 시작되었는지 여부
        private bool bStartWorkflowEvent = false;

        // 제어권 요청을 하였는지 여부
        //private bool m_bRequestControl = false;

        // 제어권 요청창
        private PopupRequestProgress m_frmRequestProgress = null;

        // 제어권 요청 리스트
        PopupRequestControl m_frmRequestControl = null;

        // 현재 실행중인 SOP의 실행임무들에 대한 상세 옵션
        private Dictionary<Sections.MissionItem, MissionItemInfo> m_dicMissionInfo = new Dictionary<MissionItem, MissionItemInfo>();

        //////////////////////////////////////////////////////////////////////////
        public SOPManager SOPManager
        {
            get { return m_sopMgr; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        // 실제 모드인가?
        public bool IsReal
        {
            get { return radioRealMode.Checked; }
        }

        // 등록 버전인가?
        public bool IsRegular
        {
            get { return radioRegistMode.Checked; }
        }

        // 평일 버전인가?
        public bool IsNormal
        {
            get { return radioNormal.Checked; }
        }

        //////////////////////////////////////////////////////////////////////////

        private bool m_enableFocusSection = true;
        public bool EnableFocusSection
        {
            get { return m_enableFocusSection; }
            set { m_enableFocusSection = value; }
        }

        public Form MainFrame
        {
            //get { return FormFrame.Instance; }
            get { return this; }
        }

        // 시스템 버튼들이 Frame 가장자리로부터 얼마나 떨어져 있는가?
        private int m_nCloseButtonPos = 0;
        private int m_nMaxButtonPos = 0;
        private int m_nMinButtonPos = 0;

        public void SetMissionInfo(Sections.MissionItem item, MissionItemInfo info)
        {
            m_dicMissionInfo[item] = info;
        }

        public MissionItemInfo GetMissionInfo(Sections.MissionItem item)
        {
            if (m_dicMissionInfo.ContainsKey(item))
                return m_dicMissionInfo[item];

            return null;
        }

        public FormMain(int nSOPGenUserID, string strSOPGenUserRealName)
        {
            RibbonButton.OriginInitButtonWidth = 50;

            m_nSOPGenUserID = nSOPGenUserID;
            m_strSOPGenUserRealName = strSOPGenUserRealName;

            InitializeComponent();

            m_nPanelTopInitHeight = panelTop.Size.Height;

            m_instance = this;

            m_dbMgr = new WebDBManager(this);
            m_sopMgr = new SOPManager(m_dbMgr);

            //////////////////////////////////////////////////////////////////////////
            ProcessManager pProcessManager = ProcessManager.Instance;
            WorkFlowManager pWorkflowManager = WorkFlowManager.Instance;
            TabPageManager pPageManager = TabPageManager.Instance;

            TTSManager pTtsManager = TTSManager.Instance;
            pTtsManager.DBMgr = m_dbMgr;

            instanceHandle = this.Handle;
        }

        private void LinkSOPDisasterSystem()
        {
            if (m_frmMain2 != null)
                m_frmMain2.GetSituation().SetSOPInfo(this);
        }

        public Point GetMonitoringPosition()
        {
            Screen[] sc;
            sc = Screen.AllScreens;

            if (sc.Length == 0)
            {
                return new Point(0, 0);
            }

            string szNum = nMonitoring.ToString();
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

            if (sc.Length >= nMonitoring)
            {
                return sc[nIdx].Bounds.Location;
            }
            return new Point(0, 0);
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

            int nIdx = nDisplay - 1;
            int nScreenCount = sc.Count();

            if (nIdx < 0)
                nIdx = 0;
            else if (nIdx >= nScreenCount)
                nIdx = nScreenCount - 1;

            /*string szNum = nDisplay.ToString();
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
                nIdx = 0;*/

            if (sc.Length >= nDisplay)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = sc[nIdx].Bounds.Location;
                form.Size = new Size(sc[nIdx].Bounds.Width, sc[nIdx].Bounds.Height);

                form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                form.WindowState = FormWindowState.Maximized;
            }

            return true;
        }


        private int nMonitoring = 1;
        private int nDisaster = 2;
        private int nMission = 3;

        private void FormMain_Load(object sender, EventArgs e)
        {
            ReadOption();
            InitTab();
            InitPanels();

            m_sopMgr.Load(IsRegular, IsNormal);
            History.HistoryManager.Instance.LoadActionStepHistory(m_dbMgr);

            CreateStatusForm();

            SetMonitors();
            InitButtons();

            ResizeViewRibbonBar();

            // 초기 제어권 없음으로 설정
            SetControl(false);

            m_nSOPGenUserLevel = DBManager.GetGenUserLevel(m_nSOPGenUserID);
            m_netMgr = SDMS.NetworkManager.Instance;

            m_pageOption.mPreviewBox.OriginSize = m_pageHome.tabControl.Size;
            m_pageOption.mPreviewBox.ThumbnailBackColor = m_pageHome.tabControl.BackColor;
            m_pageOption.mPreviewBox.TargetContorl = m_pageHome.tabControl;

            labelTitle.Text += " " + GetAppVersion();
            m_pageHome.panel.Visible = m_pageHome.GetDockScenario().ScenarioCount > 0;
            m_pageHome.SetBackgroundImage(false);
            TTSManager.Instance.UseBroadcast = m_useBroadcast;

            ShowMissionText = m_pageOption.GetVisbleMissionText();

            timer1.Start();
        }

        private void CreateStatusForm()
        {
            m_frmStatus = new FormStatus(labelMode, pictureBoxStatus, labelStatus);
            
            //m_frmReal = new FormRealTimeInfo(this);
            //AddForm(m_frmReal, panelRealTimeInfo);
            //m_frmReal.Show();
        }

        // 모니터 출력을 지정
        private void SetMonitors()
        {
            m_frmMain2 = new SOPDisasterSystem.FormMain(this);
            m_frmReport = new PopupProgressReport(this);

            string szMonitoring = DBUtility.RegUtil.ReadRegValue("Monitor Info", "SOPSimulator");
            if( szMonitoring == null || szMonitoring == "")
                szMonitoring = DBManager.LoadIni("MonitoringSystem", "Monitor Info");
            int.TryParse(szMonitoring, out nMonitoring);

            string szDisaster = DBUtility.RegUtil.ReadRegValue("Monitor Info", "SOPDiaster");
            if (szDisaster == null || szDisaster == "")
                szDisaster = DBManager.LoadIni("DisasterSystem", "Monitor Info");
            int.TryParse(szDisaster, out nDisaster);

            string szMission = DBUtility.RegUtil.ReadRegValue("Monitor Info", "MissionList");
            if (szMission == null || szMission == "")
                szMission = DBManager.LoadIni("MissionList", "Monitor Info");
            if (szMission == null || szMission.Equals(""))
            {
                szMission = "-1";
            }

            int.TryParse(szMission, out nMission);

            //SetMonitorForm(this, nMonitoring);
            SetMonitorForm(MainFrame, nMonitoring);
            //this.WindowState = FormWindowState.Maximized;

            SetMonitorForm(m_frmMain2, nDisaster);
            m_frmMain2.Show();
    
            ReportInfo();
            LinkSOPDisasterSystem();


            m_frmMain3 = new FormMissionStatus();

            if (nMission != -1)
            {
                SetMonitorForm(m_frmMain3, nMission);
                m_frmMain3.ShowMaximize();
                m_frmMain3.Show();

                //showMonitor3();
            }
        }

        void ReportInfo()
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            m_frmReport.StartPosition = FormStartPosition.Manual;
            if (sc.Length == 2)
            {
                this.Location = sc[0].Bounds.Location;
                Point pt = sc[1].Bounds.Location;
                pt.X = sc[1].Bounds.Location.X + 265;
                pt.Y = sc[1].Bounds.Location.Y + 87;
                m_frmReport.Location = pt;
            }
            else
            {
                m_frmReport.Location = sc[0].Bounds.Location;
            }

            //m_frmReport.Show();
        }

        private void ReadOption()
        {
            m_smsOn = LoadDBOption(SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.USE_SMS), "문자 사용여부");
            //string strSMSOn = DBManager.LoadIni("sms_on", "Server Connection Info");
            //m_smsOn = strSMSOn == "1";

            string strezSMSOn = DBManager.LoadIni("ez_sms_on", "Server Connection Info");
            m_useEzSMS = strezSMSOn == "1";

            string strSMSExternalCompanyMemberOn = DBManager.LoadIni("sms_externalCompanyMember_on", "Server Connection Info");
            m_smsExternalCompanyMemberOn = strSMSExternalCompanyMemberOn == "1";

            m_useBroadcast = LoadDBOption(SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.USE_BROADCAST), "방송 사용여부");
            /*string strBroadcast = DBManager.LoadIni("broadcast_on", "Server Connection Info");
            m_useBroadcast = strBroadcast == "1";*/

            string strMIssionText = DBManager.LoadIni("show_mission_text", "Server Connection Info");
            m_showMissionText = strMIssionText == "1";
        }

        public bool LoadDBOption(string strPropertyName, string strDescription)
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strPropertyName + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0 || arrResult[0] == null)
            {
                InsertDBOption(strPropertyName, "0", strDescription);
                return false;
            }

            string strValue = WebDBManager.GetStringField(arrResult[0], "");
            int nValue;

            if (!int.TryParse(strValue, out nValue))
                return false;

            return nValue == 0 ? false : true;
        }

        private void InsertDBOption(string strPropertyName, string strPropertyValue, string strDescription)
        {
            string strSQL = string.Format("Insert into OptionSOPSimulator (PropertyName, PropertyValue, Description) values ('{0}', '{1}', '{2}')",
                strPropertyName, strPropertyValue, strDescription);

            m_dbMgr.GetResultData(strSQL, 0);
        }

        private void InitTab()
        {
            m_pageHome = new PageBackstageHome();
            m_pageOption = new PageBackstageOption();
            m_pageMessage = new PageBackStageMessage();

            //this.Controls.Add(m_pageOption);
            m_pageOption.Visible = false;
            m_pageMessage.Visible = false;

            pictureBoxOpt.SetPictureBoxOwner(this);
            pictureBoxView.SetPictureBoxOwner(this);
            pictureBoxMessage.SetPictureBoxOwner(this);

            pictureBoxView.Location = pictureBoxMessage.Location;
            pictureBoxMessage.Visible = false;

            SelectViewTab(false);
        }

        private void InitPanels()
        {
            panelTop.Size = new Size(this.Size.Width, panelTop.Size.Height);
            panelMain.Location = new Point(panelTop.Location.X, panelTop.Location.Y + panelTop.Size.Height);
            panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelMain.Location.Y);

            panelRealTimeInfo.DisplayBeginPosition = new Point(30, panelRealTimeInfo.DisplayBeginPosition.Y);
        }

        private void InitButtons()
        {
            Image imgMouseOverBkgnd = global::SOPMonitoringSystem.Properties.Resources.RibbonMouseOver_bkgnd;
            Image imgCheckedBkgnd = global::SOPMonitoringSystem.Properties.Resources.RibbonChecked_bkgnd;
            //Image imgDisabledBkgnd = global::SOPMonitoringSystem.Properties.Resources.RibbonDisabled_bkgnd;

            SetControl(false);

            // 컨트롤
            InitRibbonButton(btnControl, ID.ID_CONTROL_CONTROL, global::SOPMonitoringSystem.Properties.Resources.Control_icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Control_icon_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null);
            InitRibbonButton(btnReturnControl, ID.ID_CONTROL_RETURN, global::SOPMonitoringSystem.Properties.Resources.ReturnControl_icon_Normal, global::SOPMonitoringSystem.Properties.Resources.ReturnControl_icon_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null);

            // 실행
            InitRibbonButton(btnStartSOP, ID.ID_RUN_PLAY, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Checked, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);
            InitRibbonButton(btnCancelSOP, ID.ID_RUN_CANCEL, global::SOPMonitoringSystem.Properties.Resources.CancelSOP_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.CancelSOP_Icon_Checked, global::SOPMonitoringSystem.Properties.Resources.CancelSOP_Icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);

            // 안내방송
            InitRibbonButton(btnStartBroadcast, ID.ID_ANNOUNCE_PLAY, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Checked, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);
            InitRibbonButton(btnPauseBroadcast, ID.ID_ANNOUNCE_PAUSE, global::SOPMonitoringSystem.Properties.Resources.Pause_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Pause_Icon_Checked, global::SOPMonitoringSystem.Properties.Resources.Pause_Icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);
            InitRibbonButton(btnStopBroadcast, ID.ID_ANNOUNCE_STOP, global::SOPMonitoringSystem.Properties.Resources.Stop_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Stop_Icon_Checked, global::SOPMonitoringSystem.Properties.Resources.Stop_Icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);
            InitRibbonButton(btnRepeatBroadcast, ID.ID_ANNOUNCE_COUNT, global::SOPMonitoringSystem.Properties.Resources.Repeat_icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Repeat_icon_Checked, global::SOPMonitoringSystem.Properties.Resources.Repeat_icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);

            // 현황판
            InitRibbonButton(btnFitToCurrentComponent, ID.ID_VIEW_CURRENT, global::SOPMonitoringSystem.Properties.Resources.Zoom_Selected_icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Zoom_Selected_icon_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null);
            InitRibbonButton(btnFitToScale, ID.ID_VIEW_SCALETOFIT, global::SOPMonitoringSystem.Properties.Resources.FitScreen_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.FitScreen_Icon_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null);

            ArrangeRibbonButtons();

            btnControl.Enabled = false;
            btnPauseBroadcast.Enabled = false;

            int nEdgeThick = MainFrame == this ? 0 : FormFrame.Instance.EdgeThick;
            btnClose.Location = new Point(btnClose.Location.X - nEdgeThick, btnClose.Location.Y);
            btnMax.Location = new Point(btnMax.Location.X - nEdgeThick, btnMax.Location.Y);
            btnMin.Location = new Point(btnMin.Location.X - nEdgeThick, btnMin.Location.Y);

            m_nCloseButtonPos = MainFrame.Size.Width - btnClose.Location.X;
            m_nMaxButtonPos = MainFrame.Size.Width - btnMax.Location.X;
            m_nMinButtonPos = MainFrame.Size.Width - btnMin.Location.X;
        }

        public bool HasControl
        {
            get
            {
                return btnControl.Text == "제어";
            }
        }

        public bool SMSOn
        {
            get { return m_smsOn; }
            set { m_smsOn = value; }
        }

		private bool m_bShowLegend = false;
		public bool ShowLegend
		{
			get { return m_bShowLegend; }
			set 
			{
				if (m_pageHome != null)
				{
					if (m_pageHome.frmLegend != null)
					{
						m_pageHome.frmLegend.Visible = value;
					}
				}
				m_bShowLegend = value; 
			}
		}

        public void SetControl(bool hasControl)
        {
            if (hasControl)
            {
                btnControl.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Control_icon_Normal;
                btnControl.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Control_icon_Checked;
                btnControl.Text = "제어";

                btnReturnControl.NormalImage = global::SOPMonitoringSystem.Properties.Resources.ReturnControl_icon_Normal;
                btnReturnControl.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.ReturnControl_icon_Checked;
                btnReturnControl.Text = "제어권 반납";

                if (m_netMgr != null)
                {
                    // 제어권을 가지게 되면 현재 진행중인 화재 상황에 대하여 SOP List를 팝업시킨다.
                    m_netMgr.ShowDetectSignal();
                }

                OnEnabled(ID.ID_CONTROL_REQUEST);
            }
            else
            {
                btnControl.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Monitoring_icon_Normal;
                btnControl.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Monitoring_icon_Checked;
                btnControl.Text = "모니터링";

                btnReturnControl.NormalImage = global::SOPMonitoringSystem.Properties.Resources.RequestControl_icon_Normal;
                btnReturnControl.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.RequestControl_icon_Checked;
                btnReturnControl.Text = "제어권 요청";

                // 제어권을 잃었으므로 SOP List 창을 감춘다.
                Popup.PopupSensorOn.Instance.HideForm();

                // 제어권을 잃었으므로 제어권 요청창을 닫는다.
                if (m_frmRequestControl != null)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        m_frmRequestControl.CancelForm();
                        m_frmRequestControl = null;
                    });
                }

                OnEnabled(ID.ID_CONTROL_RETURN);
            }
        }

        private void OnEnabled(int nID)
        {
            switch (nID)
            {
                case ID.ID_CONTROL_REQUEST:
                    CommandBarControlEnabled(true);
                    EnabledRunGroup();
                    GetPageHome().GetDockScenario().Enabled = true;
                    GetPageHome().OnEnabled(true);
                    m_frmMain2.GetSpace().OnEnabled(true);
                    m_frmMain2.GetToolbar().Enabled = true;
                    break;

                case ID.ID_CONTROL_RETURN:
                    CommandBarControlEnabled(false);
                    GetPageHome().GetDockScenario().Enabled = false;
                    //GetPageHome().OnEnabled(false);
                    m_frmMain2.GetSpace().OnEnabled(false);
                    m_frmMain2.GetToolbar().Enabled = false;
                    break;
            }
        }

        public void EnabledRunGroup()
        {
            if (!HasControl)
            {
                btnStartSOP.Enabled = btnCancelSOP.Enabled = false;
                return;
            }

            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;
            if (page != null)
            {
                int ActionID = page.ActionStepID;
                if (Sections.WorkFlowManager.Instance.DeleteComplete == true)
                {
                    Sections.TabPageManager.Instance.RemovePage(page, !page.VirtualMode);
                }
                Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(ActionID, !page.VirtualMode);

                if (work != null)
                {
                    switch (work.State)
                    {
                        case Sections.WorkFlowState.RUN: //시작
                            btnStartSOP.Enabled = false;
                            btnCancelSOP.Enabled = true;
                            break;
                        case Sections.WorkFlowState.STOP: //실행취소
                            btnStartSOP.Enabled = true;
                            btnCancelSOP.Enabled = false;
                            break;
                        case Sections.WorkFlowState.DONE: //완료
                            btnStartSOP.Enabled = true;
                            btnCancelSOP.Enabled = false;
                            break;
                        case Sections.WorkFlowState.STANDBY: //대기
                        case Sections.WorkFlowState.PAUSE:
                        case Sections.WorkFlowState.WAIT:
                        case Sections.WorkFlowState.DISABLE:
                            break;
                    }
                }
                else
                {
                    btnStartSOP.Enabled = true;
                    btnCancelSOP.Enabled = false;
                }
            }

        }

        public PageBackstageHome GetPageHome()
        {
            return m_pageHome;
        }

        public PageBackstageOption GetPageOption()
        {
            return m_pageOption;
        }

        public PageBackStageMessage GetPageMessage()
        {
            return m_pageMessage;
        }

        public PopupProgressReport GetReport()
        {
            return m_frmReport;
        }

        public RealTimeInfoPane GetRealTime()
        {
            return panelRealTimeInfo;
        }
        /*public FormRealTimeInfo GetRealTime()
        {
            return m_frmReal;
        }*/

        private void CommandBarControlEnabled(bool isFlag)
        {
            btnStartSOP.Enabled = isFlag;
            btnCancelSOP.Enabled = isFlag;
            radioRealMode.Enabled = isFlag;
            radioVirtualMode.Enabled = isFlag;
            radioRegistMode.Enabled = isFlag;
            radioNonRegistMode.Enabled = isFlag;
            radioNormal.Enabled = isFlag;
            radioHoliday.Enabled = isFlag;
            btnStartBroadcast.Enabled = isFlag;
            //btnPauseBroadcast.Enabled = isFlag;
            btnStopBroadcast.Enabled = isFlag;
            btnRepeatBroadcast.Enabled = isFlag;
            labelReal.Enabled = isFlag;
            labelVirtual.Enabled = isFlag;
            labelRegular.Enabled = isFlag;
            labelNonRegular.Enabled = isFlag;
            labelNormal.Enabled = isFlag;
            labelHoliday.Enabled = isFlag;
        }

        private void ArrangeRibbonButtons()
        {
            ArrangeRibbonButton(btnControl, btnReturnControl);

            ArrangeRibbonButton(btnReturnControl, pictureBox2, btnStartSOP);
            ArrangeRibbonButton(btnStartSOP, btnCancelSOP);
            ArrangeRibbonButton(btnCancelSOP, panelRealMode);

            ArrangeRibbonButton(panelRealMode, pictureBox3, panelRegistMode);
            ArrangeRibbonButton(panelRegistMode, panelNormalMode);

            ArrangeRibbonButton(panelNormalMode, pictureBox4, btnStartBroadcast);
            ArrangeRibbonButton(btnStartBroadcast, btnPauseBroadcast);
            ArrangeRibbonButton(btnPauseBroadcast, btnStopBroadcast);
            ArrangeRibbonButton(btnStopBroadcast, btnRepeatBroadcast);

            ArrangeRibbonButton(btnRepeatBroadcast, pictureBox5, btnFitToCurrentComponent);
            ArrangeRibbonButton(btnFitToCurrentComponent, btnFitToScale);

            ArrangeRibbonButton(btnFitToScale, pictureBox6, panelStatus);

            ArrangeRibbonButton(panelStatus, pictureBox7, panelRealTimeInfo);
        }

        private void ArrangeRibbonButton(Control ctrlPrev, Control ctrlNext)
        {
            ctrlNext.Location = new Point(ctrlPrev.Location.X + ctrlPrev.Size.Width, ctrlPrev.Location.Y);
        }

        private void ArrangeRibbonButton(Control ctrlPrev, Control ctrlMiddle, Control ctrlNext)
        {
            ctrlMiddle.Location = new Point(ctrlPrev.Location.X + ctrlPrev.Size.Width - 3, ctrlMiddle.Location.Y);
            ctrlNext.Location = new Point(ctrlMiddle.Location.X + ctrlMiddle.Size.Width - 3, ctrlPrev.Location.Y);
        }

        private void InitRibbonButton(RibbonButton btn, int nID, Image imgNormal, Image imgChecked, Image imgDisabled, Image imgMouseOverBkgnd, Image imgCheckedBkgnd, Image imgDisabledBkgnd)
        {
            btn.NormalImage = imgNormal;
            btn.CheckedImage = imgChecked;
            btn.DisabledImage = imgDisabled;
            btn.MouseOverBkgndImage = imgMouseOverBkgnd;
            btn.CheckedBkgndImage = imgCheckedBkgnd;
            btn.DisabledBkgndImage = imgDisabledBkgnd;
            btn.Owner = this;
			btn.UseTextLocation = false;
            SetButtonID(btn, nID);
        }

        private void SetButtonID(Button btn, int nID, string strTooltipText = "")
        {
            m_dicButtonIDs[btn] = nID;
            m_dicIDButtons[nID] = btn;
            m_dicButtonChecked[btn] = false;

            if (strTooltipText.Length > 0)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(btn, strTooltipText);
            }
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            InitPanels();
            ResizeViewRibbonBar();
            ResizeSystemButtons();

            if (m_pageHome != null)
            {
                foreach (SectionTabPage page in m_pageHome.TabControls.Controls)
                {
                    page.ReSizePanel();
                    GetPageHome().changeLocation(page.Height);
                }
            }
        }

        private void ResizeSystemButtons()
        {
            if (m_nCloseButtonPos > 0)
            {
                btnClose.Location = new Point(MainFrame.Size.Width - m_nCloseButtonPos, btnClose.Location.Y);
                btnMax.Location = new Point(MainFrame.Size.Width - m_nMaxButtonPos, btnMax.Location.Y);
                btnMin.Location = new Point(MainFrame.Size.Width - m_nMinButtonPos, btnMin.Location.Y);
            }
        }

        private void ResizeViewRibbonBar()
        {
            panelViewRibbonBarMiddle.Location = new Point(20, panelViewRibbonBarLeft.Location.Y + 1);
            panelViewRibbonBarMiddle.Size = new Size(this.Size.Width - panelViewRibbonBarMiddle.Location.X - 20, panelViewRibbonBarMiddle.Size.Height);
            panelViewRibbonBarRight.Location = new Point(this.Size.Width - panelViewRibbonBarRight.Size.Width, panelViewRibbonBarLeft.Location.Y + 1);

            panelRealTimeInfo.Size = new Size(panelViewRibbonBarMiddle.Size.Width - panelRealTimeInfo.Location.X, panelRealTimeInfo.Size.Height);
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {

        }
		char szDeli = (char)0x06;
        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;
            int nButtonID = GetButtonID(btn);

            switch (nButtonID)
            {
                case ID.ID_CONTROL_RETURN:
                    if (HasControl)
                    {
                        // 제어권 반납
                        SetControl(false);
                        OnEnabled(ID.ID_CONTROL_RETURN);
                        UpdateUserInfo(0);  //제어권 반납
                    }
                    else
                    {
                        // 제어권 요청
                        //m_bRequestControl = true;
                        m_frmRequestProgress = new PopupRequestProgress();
                        m_frmRequestProgress.StartPosition = FormStartPosition.Manual;
                        Point p = GetMonitoringPosition();
                        p.X += 100;
                        p.Y += 100;
                        int width = m_frmRequestProgress.Bounds.Width;
                        int height = m_frmRequestProgress.Bounds.Height;
                        m_frmRequestProgress.SetBounds(p.X, p.Y, width, height);
                        UpdateUserInfo(1);
                        m_frmRequestProgress.Show();
                    }
                    break;
                case ID.ID_RUN_PLAY:
                    if (HasControl == true)
                        Play();
                    break;
                case ID.ID_RUN_CANCEL:
                    if (HasControl == true)
                        StopWorkflow(DateTime.Now);
                    break;
                case ID.ID_RUN_COMPLETE:
                    {
                        TreeNode node = m_pageHome.GetDockScenario().GetBarLevelTree().GetSelectedNode();
                        if (node == null) return;
                        m_pageHome.GetDockScenario().DeleteGridRowScenario(node.FullPath.Replace("\\", szDeli.ToString()));
                        DoneWorkflow();
                    }
                    break;
                case ID.ID_ANNOUNCE_PLAY:
                    btnStartBroadcast.Enabled = false;
                    //btnPauseBroadcast.Enabled = true;
                    btnStopBroadcast.Enabled = true;
                    ResumeSpeech();
                    break;
                case ID.ID_ANNOUNCE_PAUSE:
                    btnStartBroadcast.Enabled = true;
                    //btnPauseBroadcast.Enabled = false;
                    btnStopBroadcast.Enabled = true;
                    PauseSpeech();
                    break;
                case ID.ID_ANNOUNCE_STOP:
                    /*btnStartBroadcast.Enabled = true;
                    //btnPauseBroadcast.Enabled = false;
                    btnStopBroadcast.Enabled = false;*/
                    StopSpeech();
                    break;
                case ID.ID_VIEW_CURRENT: // zoomsection
                    ZoomCurrent();
                    break;
                case ID.ID_VIEW_SCALETOFIT:
                    ZoomScaletoFit();
                    break;
            }
        }

        private void ZoomCurrent()
        {
            if (m_currentSection == null)
                return;
            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)m_currentSection.GetParent();
            if (panel != null)
            {
                panel.ZoomSection(m_currentSection);
            }
        }

        private void ZoomScaletoFit()
        {
            ArrayList arrPanels = this.GetPageHome().GetPanels();
            if (arrPanels == null || arrPanels.Count == 0)
                return;

            if (arrPanels.Count == 1)
            {
                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)arrPanels[0];
                panel.ZoomPanel();
                return;
            }

            Sections.PanelSectionEx panelCurrent = this.GetPageHome().GetCurrentPanel();
            panelCurrent.ZoomPanel();
        }

        private void radioNormalMode_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (btn == null)
                return;

            if (btn.Checked)
            {
                ChangeMode();
            }
        }

        private void radioRegistMode_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (btn == null)
                return;

            if (btn.Checked)
            {
                ChangeMode();
            }
        }

        private void radioRealMode_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = (RadioButton)sender;
            if (btn == null)
                return;

            if (btn.Checked)
            {
                bool isRealMode = btn == radioRealMode;

                radioNonRegistMode.Enabled = !isRealMode;
                GetPageHome().GetDockScenario().GetBarLevelTree().SelectSop(null);

                m_frmStatus.RealMode(isRealMode);

                ChangeMode();
            }
        }

        private void ChangeMode()
        {
            if (!m_sopMgr.IsOpened)
                return;

            bool isRegular = radioRegistMode.Checked;
            bool isNormal = radioNormal.Checked;

            BarLevelTree tree = m_pageHome.GetDockScenario().GetBarLevelTree();

            if (tree.IsRegular != isRegular || tree.IsNormal != isNormal)
                tree.Load(m_sopMgr, isRegular, isNormal);

            m_pageOption.mPreviewBox.Refresh();
        }

        public void ChangeMode(bool isReal, bool isRegular, bool isNormal)
        {
            if (radioRealMode.Checked == isReal &&
                radioRegistMode.Checked == isRegular &&
                radioNormal.Checked == isNormal)
                return;

            radioRealMode.Checked = isReal;
            radioVirtualMode.Checked = !isReal;

            radioRegistMode.Checked = isRegular;
            radioNonRegistMode.Checked = !isRegular;

            radioNormal.Checked = isNormal;
            radioHoliday.Checked = !isNormal;

            ChangeMode();
        }

        public bool Play()
        {
            if (btnStartSOP.Enabled == false)
                return false;

            GetPageHome().ClearProcess();

            TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            if (tapPage == null || tapPage.GetType() != typeof(Sections.SectionTabPage))
                return false;
            Sections.SectionTabPage page = (Sections.SectionTabPage)tapPage;
            if (page == null)
                return false;

            // 각 Section들의 CompleteCounte를 모두 초기화한다.
            InitCompleteCount(page);

            TreeNode node = m_pageHome.GetDockScenario().GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return false;

            RunWorkflowWithEvent();

            return true;
        }

        public bool PlayWithDisasterPosition(int nZoneID, int nSensorID, int nSensorHistoryID)
        {
            if (btnStartSOP.Enabled == false)
                return false;



            GetPageHome().ClearProcess();

            TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            if (tapPage == null || tapPage.GetType() != typeof(Sections.SectionTabPage))
                return false;
            Sections.SectionTabPage page = (Sections.SectionTabPage)tapPage;
            if (page == null)
                return false;

            // 각 Section들의 CompleteCounte를 모두 초기화한다.
            InitCompleteCount(page);

            TreeNode node = m_pageHome.GetDockScenario().GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return false;

			

            RunWorkflowWithoutEvent(nZoneID, nSensorID, nSensorHistoryID);

            return true;
        }

        public void RunWorkflowWithoutEvent(int nZoneID, int nSensorID, int nSensorHistoryID)
        {
            TabPage page = m_pageHome.tabControl.SelectedTab;
            if (page == null)
            {
                return;
            }

			SDMS.FireDetectSignal signal = m_netMgr.FindDetectSignal(nSensorHistoryID);
			if (signal == null)
				return;

            Sections.SectionTabPage tabPage = (Sections.SectionTabPage)page;
            int nActionStepID = GetTabActionStepID(tabPage);
            BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(nActionStepID);
            string szName = node.FullPath;
            bool bHasPos = true;
			if (szName.IndexOf("자연재해") != -1 || szName.IndexOf("태풍") != -1)
            {
                bHasPos = false;
            }

            string sopName = szName.Substring(szName.IndexOf("\\") + 1);
            string disasterName = szName.Substring(0, szName.IndexOf("\\"));
            ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();

            if (m_smsExternalCompanyMemberOn)
            {
                // 협력업체 직원들의 전화번호 추가(시작 및 종료시에만...)
                AddExternalCompanyMemberPhoneNumbers(arrListCall);
            }

            Process.WorkFlowStartNotifyProcess start = new Process.WorkFlowStartNotifyProcess();
            start.VirtualMode = !FormMain.Instance.IsReal;
            start.ActionStepID = nActionStepID;
            start.HasPosition = bHasPos;
            start.SOPName = sopName;
            start.CallList = arrListCall;
            start.NoPopup = true;
			start.UseSMS = false;
            start.PositionName = signal.PositionName;
			start.DetectTime = signal.DetectTime;
            start.OnPostProcess += new PostProcessEvent(RunWorkflowAsync);


            SOPDisasterSystem.Zone zone = SOPDisasterSystem.DataManager.Instance.GetZone(nZoneID);
            HistoryDiasterPosition disasterPos = new SOPMonitoringSystem.HistoryDiasterPosition();
            disasterPos.PoistionName = signal.PositionName;

            //start.PositionName = zone.BroadcastName;

            Vertex2D pos3D = zone.Polygon.CalcWeightCenter();
            disasterPos.X = (float)pos3D.x;
            disasterPos.Y = 0.0f;
            disasterPos.Z = (float)pos3D.y;
            if (zone.IsOutdoor == true)
                disasterPos.FloorIndex = -999.0f;
            else
                disasterPos.FloorIndex = zone.Floor.FloorIndex;
            if (zone.Building != null)
                disasterPos.BuildingID = zone.Building.BuildingID;
            else
                disasterPos.BuildingID = "ZONE";
            disasterPos.DisasterName = "화재";
            Popup.PopupStartEvent form = start.Popup;
            form.DisasterName = disasterName;
            form.AddLastHistoryDisasterPoistion(disasterPos);

            if (m_frmMain2.LayoutForm != null)
            {
                m_frmMain2.LayoutForm.SetCheckPoistion(form, true);
            }
            Process.ProcessManager.Instance.AddFirst(start);
        }

        private void InitCompleteCount(Sections.SectionTabPage tabPage)
        {
            Type type = typeof(Sections.PanelSectionEx);

            foreach (Control ctrl in tabPage.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl;
                    InitSectionCompleteCount(panel);
                }
            }
        }

        private void InitSectionCompleteCount(Sections.PanelSectionEx panel)
        {
            foreach (Sections.Section section in panel.Sections)
            {
                section.CompleteCount = 0;
            }
        }

        // isControl : 0이면 제어권 반납
        //             1이면 제어권 요청
        private void UpdateUserInfo(int isControl)
        {
            if (isControl == 0)
                m_netMgr.SendMessage((short)SDMS.TCP_ID.RETURN_CONTROL);
            else
                m_netMgr.SendMessage((short)SDMS.TCP_ID.REQUEST_CONTROL);
        }

        /*private void UpdateUserInfo(int isControl)
        {
            // 제어권 반납
            if (m_arrConnectedUser.Count == 0)
            {
                string strSQL = string.Format("update ControlCheck set ControlCheck = {0} where UserID = {1}", isControl, m_nSOPGenUserID);

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return;

                strSQL = string.Format("update ControlUser set UserID = null where UserID = {0}", m_nSOPGenUserID);
                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }
            // 제어권 요청
            else
            {
                foreach (UserInfo data in m_arrConnectedUser)
                {
                    if (data.UserID == m_nSOPGenUserID)
                    {
                        string strSQL = string.Format("update ControlCheck set ControlCheck = {0} where id = {1}", isControl, data.ID);

                        if (m_dbMgr.GetResultData(strSQL, 0) == null)
                            return;
                        break;
                    }
                }
            }
        }*/

        public int GetButtonID(Button btn)
        {
            if (m_dicButtonIDs.ContainsKey(btn))
                return m_dicButtonIDs[btn];

            return -1;
        }

        #region Top패널 Mouse 이벤트 , Maximized, Minimized, Move

        private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = panelTop.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void panelTop_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
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
            if (MainFrame.WindowState == FormWindowState.Normal)
            {
                MainFrame.WindowState = FormWindowState.Maximized;
                btnMax.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.NormalWindow_Normal;
            }
            else if (MainFrame.WindowState == FormWindowState.Maximized)
            {
                Size sizeCur = MainFrame.Size;
                MainFrame.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.MaxWindow_Normal;
                Size sizeNormal = MainFrame.Size;

                double hRate = (double)sizeNormal.Height / (double)sizeCur.Height;
                MainFrame.Size = new Size((int)(sizeCur.Width * hRate), sizeNormal.Height);
            }
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
                btnMax.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.NormalWindow_Normal;
            }
            else if (MainFrame.WindowState == FormWindowState.Maximized)
            {
                MainFrame.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.MaxWindow_Normal;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            MainFrame.Close();
        }

        #endregion

        #region Tab 전환

        public void TextPictureBox_MouseDown(TextPictureBox pictureBox, MouseEventArgs e)
        {
            if (e != null)
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left)
                    return;
            }

            if (pictureBox == pictureBoxOpt)
            {
                SelectOptionTab();
            }
            else if (pictureBox == pictureBoxView)
            {
                SelectViewTab();
            }
            else if (pictureBox == pictureBoxMessage)
            {
                SelectMessageTab();
            }
        }

        public void SelectMessageTab()
        {
            pictureBoxOpt.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            pictureBoxView.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            pictureBoxMessage.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Pressed;

            m_pageOption.Visible = false;
            m_pageHome.Visible = false;
            m_pageMessage.Visible = true;

            panelTop.Size = new Size(panelTop.Size.Width, panelViewRibbonBarLeft.Location.Y);

            panelMain.Location = new Point(0, panelTop.Size.Height);
            panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelTop.Size.Height);
        }

        public void SelectOptionTab()
        {
            pictureBoxOpt.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Pressed;
            pictureBoxView.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            pictureBoxMessage.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;

            m_pageOption.Initialize();

            m_pageOption.Visible = true;
            m_pageHome.Visible = false;
            m_pageMessage.Visible = false;

            panelTop.Size = new Size(panelTop.Size.Width, panelViewRibbonBarLeft.Location.Y);

            panelMain.Location = new Point(0, panelTop.Size.Height);
            panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelTop.Size.Height);
        }

        public void SelectViewTab(bool showPageHome = true)
        {
            pictureBoxOpt.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            pictureBoxView.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Pressed;
            pictureBoxMessage.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;

            m_pageOption.Visible = false;
            m_pageHome.Visible = showPageHome;
            m_pageMessage.Visible = false;

            panelTop.Size = new Size(panelTop.Size.Width, m_nPanelTopInitHeight);

            panelMain.Location = new Point(0, panelTop.Size.Height);
            panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelTop.Size.Height);
        }

        #endregion

        #region Version 정보

        public string GetAppVersion()
        {
            if (!m_isReadVersion)
                ReadAppVersion();

            return m_strVersion;
        }

        private void ReadAppVersion()
        {
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader("svnMonitorInfo.txt", Encoding.Default);

                string strLine = reader.ReadLine();

                if (strLine != null)
                {
                    int nLen = strLine.Length;
                    int nFirstIndex = -1, nSecondIndex = -1;

                    for (int i = 0; i < nLen; i++)
                    {
                        char ch = strLine.ElementAt(i);

                        if (ch < '0' || ch > '9')
                        {
                            if (nFirstIndex < 0)
                                nFirstIndex = i;
                            else
                            {
                                nSecondIndex = i;
                                break;
                            }
                        }
                    }

                    if (nFirstIndex < 0)
                    {
                        m_strVersion += "." + strLine;
                    }
                    else if (nSecondIndex < 0)
                    {
                        m_strVersion += "." + strLine.Substring(0, nFirstIndex);
                    }
                    else
                    {
                        m_strVersion += "." + strLine.Substring(nFirstIndex + 1, nSecondIndex - nFirstIndex - 1);
                    }
                }

                reader.Close();
            }
            catch (System.IO.FileNotFoundException)
            {
            }

            m_isReadVersion = true;
        }

        #endregion

        private void AddForm(Form frmPage, Panel panelParent)
        {
            frmPage.Location = new Point(0, 0);
            frmPage.Dock = DockStyle.Fill;
            frmPage.TopLevel = false;
            frmPage.Parent = this;

            panelParent.Controls.Add(frmPage);
        }

        public void FormMain_Activated(object sender, EventArgs e)
        {
            if (!m_isFirst)
            {
                if (m_pageHome == null || m_pageOption == null || m_pageMessage == null)
                    return;

                AddForm(m_pageHome, panelMain);
                AddForm(m_pageOption, panelMain);
                AddForm(m_pageMessage, panelMain);
                m_pageHome.Show();
                m_isFirst = true;

                if (m_sopMgr.IsOpened)
                {
                    if (LoadSOP())
                    {
                        //StartWriteDB(); //사용자 접속 정보 DB에 쓰기

                        // 기존에 실행되고 있던 SOP를 불러온다.
                        LoadHistory();

                        SectionTabPage page = (SectionTabPage)GetPageHome().TabControls.SelectedTab;
                        int nCount = GetPageHome().TabControls.Controls.Count;
                        if (page != null)
                        {
                            int nActionStepID = page.ActionStepID;

                            if (nActionStepID != 0)
                            {
                                m_isOpen = true;
                                FormMain.Instance.GetPageHome().panel.Visible = true;
                                FormMain.Instance.GetPageHome().SetBackgroundImage(true);

                                radioRealMode.Checked = !page.VirtualMode;
                                ActionStepInfo info = FormMain.Instance.SOPManager.GetActionStepInfo(nActionStepID);
                                BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
                                TreeNode node = tree.FindActionStepNode(nActionStepID);
                                tree.PrevSelectedDisasterID = info.DisasterID;
                                //tree.SelectSop(node);

                                bool isRealMode = true;
                                int nCurrentActionStepID = ReadCurrentActionStep(ref isRealMode);

                                // if (HasControl == true)
                                {
                                    if (nCurrentActionStepID >= 0)
                                        GetPageHome().GetDockScenario().SelectedGridRow(nCurrentActionStepID, isRealMode);
                                    else
                                    {
                                        // 마지막 시나리오 선택
                                        GetPageHome().GetDockScenario().SetSelectedGridRow();
                                    }
                                }



                                page.ReSizePanel();
                            }
                        }
                    }
                    LoadCompanyMember();
                }
            }
        }

        private bool LoadSOP()
        {
            BarLevelTree tree = m_pageHome.GetDockScenario().GetBarLevelTree();
            return tree.Load(m_sopMgr, radioRegistMode.Checked, radioNormal.Checked);
        }

        /*private void StartWriteDB()
        {
            DBWrite = new Thread(new ThreadStart(WorkerThreadMethod));
            DBWrite.IsBackground = false;
            DBWrite.Start();
            Thread.Sleep(500);
        }*/

        private void StopWriteDB()
        {
            try
            {
                if (DBWrite != null && DBWrite.IsAlive)
                {
                    //m_isWorkingTempThread = false;
                    //m_isStop = true;
                    DBWrite.Join(500);
                    DBWrite.Abort();

                    DBWrite = null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
            }
        }

        private bool LoadHistory()
        {
            return m_pageHome.GetDockScenario().LoadHistory(m_dbMgr, m_sopMgr);
        }

        public int ReadCurrentActionStep(ref bool isRealMode)
        {
            string strSQL = "select ActionStepID, RealMode from CurrentActionStep where id = 1";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count <= 1)
                return -1;

            int nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            isRealMode = WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;
            SOPManager.SetCurrentActionStep(nActionStepID, isRealMode);

            return nActionStepID;
        }

        private bool LoadCompanyMember()
        {
            DockingRightPersonnel personnel = m_pageHome.GetDockPersonnel();
            return personnel.Load(m_sopMgr);
        }

        public void CloseRequestProgress()
        {
            if (m_frmRequestProgress != null)
            {
                if (m_frmRequestProgress.Visible)
                {
                    m_frmRequestProgress.Close();
                    m_frmRequestProgress = null;
                }
            }
        }

        //private delegate void Invoke_SetControl(bool enabled);
        //private delegate void Invoke_OnEnabled(int nID);

        /*public void WorkerThreadMethod()
        {
            Invoke_SetControl InvokeSetControl = new Invoke_SetControl(this.SetControl);

            // 처음 로딩시에는 ControlCheck를 무조건 -1로 준다.
            WriteUserInfo(true);

            int nThreadCount = 0;

            while (!m_isStop)
            {
                Thread.Sleep(1000);

                if (++nThreadCount >= 5)
                {
                    // 사용자 접속 정보 주기적으로 DB에 입력
                    WriteUserInfo();
                    nThreadCount = 0;
                }

                //제어자의 ID를 얻음
                int nUserID = LoadControlUser();
                if (nUserID < 0)
                {
                    if (m_bRequestControl == true)
                    {
                        if (m_frmRequestProgress != null && m_frmRequestProgress.Visible == true)
                        {
                            m_frmRequestProgress.Invoke((MethodInvoker)delegate
                            {
                                m_frmRequestProgress.Close();
                            });
                        }
                        m_bRequestControl = false;
                    }

                    this.Invoke(InvokeSetControl, false);
                    continue;
                }
                else
                {
                    //제어 중
                    if (nUserID == m_nSOPGenUserID && HasControl)
                    {
                        ArrayList arrRequest = GetRequestControl();
                        if (arrRequest == null || arrRequest.Count == 0) continue;

                        foreach (ControlCheck data in arrRequest)
                        {
                            if (data.ControlChecked == 1)
                            {
                                Thread t = new Thread(new ParameterizedThreadStart(TempWorkerThread));
                                t.Start(nThreadCount);

                                m_isWorkingTempThread = true;
                                SendMessagetoContolUser(nUserID);
                                m_isWorkingTempThread = false;
                                break;
                            }
                        }
                    }
                    else if (nUserID != m_nSOPGenUserID && HasControl)
                    {
                        m_bRequestControl = false;
                        //모니터링으로 변경
                        this.Invoke(InvokeSetControl, false);
                    }
                    else if (nUserID == m_nSOPGenUserID && !HasControl)
                    {
                        if (m_bRequestControl == true)
                        {
                            if (m_frmRequestProgress != null && m_frmRequestProgress.Visible == true)
                            {
                                try
                                {
                                    m_frmRequestProgress.Invoke((MethodInvoker)delegate
                                    {
                                        m_frmRequestProgress.Close();
                                    });
                                }
                                catch (System.ObjectDisposedException)
                                {
                                }
                            }
                            m_bRequestControl = false;
                        }

                        this.Invoke(InvokeSetControl, true);
                    }
                }

                if (m_bRequestControl == true)
                {
                    ArrayList arRequestUsers = GetRequestControl();
                    bool bfind = false;
                    foreach (ControlCheck check in arRequestUsers)
                    {
                        if (check.UserID == m_nSOPGenUserID)
                        {
                            bfind = true;
                            break;
                        }
                    }
                    if (bfind == false)
                    {
                        m_bRequestControl = false;
                        RejectRequest();
                    }
                }
            }
        }

        private void WriteUserInfo(bool initControlCheck = false)
        {
            string strSQL = "";
            bool isCheck = false;

            if (m_isStop == true)
                return;

            ArrayList arrConnectedUser = LoadConnectedUser();

            Thread.Sleep(500);
            if (arrConnectedUser == null)
                return;

            foreach (UserInfo data in arrConnectedUser)
            {
                if (data.UserID == m_nSOPGenUserID)
                {
                    isCheck = true;

                    DateTime time = DateTime.Now;
                    string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", time.ToShortDateString(), time.Hour, time.Minute, time.Second);

                    if (initControlCheck)
                        strSQL = string.Format("update ControlCheck set Time = {0}, ControlCheck = -1 where id = {1}", strTime, data.ID);
                    else
                        strSQL = string.Format("update ControlCheck set Time = {0} where id = {1}", strTime, data.ID);
                    break;
                }
            }

            if (!isCheck)
            {
                isCheck = false;
                strSQL = "select Max(id) from ControlCheck";
                ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
                int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                if (nID < 0) nID = 0;

                DateTime time = DateTime.Now;
                string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", time.ToShortDateString(), time.Hour, time.Minute, time.Second);

                strSQL = string.Format("insert into ControlCheck (id, UserID, Time, ControlCheck) values ({0}, {1}, {2}, {3})",
                                ++nID, m_nSOPGenUserID, strTime, -1);
            }

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return;

            ArrayList arrRequest = GetRequestControl();
            if (arrRequest == null) return;

        }

        // 제어권을 갖고 있는 User
        private int LoadControlUser()
        {
            string strSQL = "SELECT ControlUser.UserID FROM ControlUser, ControlCheck Where ControlUser.UserID = ControlCheck.UserID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nControlUser = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nControlUser;
        }*/
        // 제어권을 요청한 사용자를 얻음
        public ArrayList GetRequestControl()
        {
            string strSQL = "select ControlCheck.id, ControlCheck.userid, ControlCheck.time, ControlCheck.controlcheck, CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel " +
                            "from ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
                            "where ControlCheck.controlcheck = 1 AND ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID AND SOPGenUser.UserLevel = SOPGenLevel.ID " +
                            "order by SOPGenUser.UserLevel desc";

            ArrayList arrSendControl = m_dbMgr.GetResultData(strSQL, 0);
            if (arrSendControl == null)
                return null;

            int nResultCount = arrSendControl.Count;
            DateTime dtDefult = new DateTime();
            ArrayList arrRequest = new ArrayList();

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                ControlCheck data = new ControlCheck();
                data.ID = WebDBManager.GetIntField(arrSendControl[i].ToString(), -1);
                data.UserID = WebDBManager.GetIntField(arrSendControl[i + 1].ToString(), -1);
                data.Time = WebDBManager.GetDateTimeField(arrSendControl[i + 2].ToString(), dtDefult);
                data.ControlChecked = WebDBManager.GetIntField(arrSendControl[i + 3].ToString(), -1);
                data.MemberName = WebDBManager.GetStringField(arrSendControl[i + 4].ToString(), "");
                data.MemberID = WebDBManager.GetStringField(arrSendControl[i + 5].ToString(), "");
                data.UserLevel = WebDBManager.GetIntField(arrSendControl[i + 6].ToString(), -1);

                arrRequest.Add(data);
            }
            return arrRequest;
        }

        public void ShowRequestControl(string strUserID, string strUserName, string strUserNickName, string strIP)
        {
            if (m_frmRequestControl == null)
            {
                m_frmRequestControl = new PopupRequestControl();
                m_frmRequestControl.Show();
            }

            m_frmRequestControl.AddUser(strUserID, strUserName, strUserNickName, strIP);
        }

        public void ClearRequestControl()
        {
            m_frmRequestControl = null;
        }

        /*private void TempWorkerThread(object arg)
        {
            Invoke_SetControl InvokeSetControl = new Invoke_SetControl(this.SetControl);

            int nThreadCount = int.Parse(arg.ToString());

            while (m_isWorkingTempThread)
            {
                Thread.Sleep(1000);

                if (++nThreadCount >= 5)
                {
                    // 사용자 접속 정보 주기적으로 DB에 입력
                    WriteUserInfo();
                    nThreadCount = 0;
                }

                //제어자의 ID를 얻음
                int nUserID = LoadControlUser();
                if (nUserID < 0)
                {
                    this.Invoke(InvokeSetControl, false);
                    continue;
                }
                else
                {
                    //제어 중
                    if (nUserID == m_nSOPGenUserID && HasControl)
                    {
                    }
                    else if (nUserID != m_nSOPGenUserID && HasControl)
                    {
                        //모니터링으로 변경
                        this.Invoke(InvokeSetControl, false);
                    }
                    else if (nUserID == m_nSOPGenUserID && !HasControl)
                    {
                        this.Invoke(InvokeSetControl, true);
                    }
                }
            }
        }

        private void SendMessagetoContolUser(int nControlUser)
        {
            //제어권을 갖고있는 사용자이면 제어권 요청한 사용자의 목록을 출력
            if (nControlUser == m_nSOPGenUserID)
            {
                m_frmRequestControl = new PopupRequestControl(m_nSOPGenUserID, m_nSOPGenUserLevel);

                int nStrongUserID = m_frmRequestControl.StrongUserID;

                // 제어권 요청을 한 User 가운데 현재 제어권 가진 User보다 높은 레벨의 User가 존재하는가?
                if (nStrongUserID > 0)
                {
                    m_nChangeUserID = nStrongUserID;
                    if (m_frmRequestProgress != null && m_frmRequestProgress.Visible)
                        m_frmRequestProgress.Close();
                    ChangeControlUser(true);

                }
                else
                {
                    if (m_frmRequestControl.HasData())
                    {
                        m_frmRequestControl.Focus();

                        if (m_frmRequestControl.ShowDialog() == DialogResult.OK)
                        {
                            //제어요청 허락
                            ChangeControlUser(true);
                        }
                        else
                        {
                            //제어요청 거부
                            ChangeControlUser(false);
                        }
                    }
                }

                m_frmRequestControl = null;
            }
        }

        private void RejectRequest()
        {
            if (m_frmRequestProgress != null && m_frmRequestProgress.Visible == true)
            {
                m_frmRequestProgress.Invoke((MethodInvoker)delegate
                {
                    m_frmRequestProgress.SetMessage("거부되었습니다.");
                });
            }
        }

        // 현재 접속중인 User 현황 - 접속 중인 User는 주기적으로 접속시간이 업데이트 됨.
        private ArrayList LoadConnectedUser()
        {
            ArrayList arrConnectedUser = new ArrayList();

            string strSQL = "SELECT  ControlCheck.ID, ControlCheck.UserID, ControlCheck.Time, ControlCheck.ControlCheck, " +
                            "CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel, SOPGenLevel.LevelName " +
                            "FROM ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
                            "where ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID and SOPGenUser.UserLevel = SOPGenLevel.ID";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null) return null;

            int nResultCount = arrResult.Count;
            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                UserInfo data = new UserInfo();

                data.ID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                data.UserID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                data.Time = WebDBManager.GetDateTimeField(arrResult[i + 2], dtDefault);
                data.ControlChecked = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                data.MemberName = WebDBManager.GetStringField(arrResult[i + 4], "");
                data.MemberID = WebDBManager.GetStringField(arrResult[i + 5], "");
                data.UserLevel = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                data.LevelName = WebDBManager.GetStringField(arrResult[i + 7], "");

                arrConnectedUser.Add(data);
            }

            return arrConnectedUser;
        }

        // 제어자가 제어요청을 받음
        private void ChangeControlUser(bool isContol)
        {
            if (!HasControl)
                return;

            if (isContol) //제어요청 허락
            {
                string strSQL = string.Format("update ControlUser set UserID = NULL");

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }
            else //제어요청 거부
            {
                ArrayList arrConnecedtUser = LoadConnectedUser();
                foreach (UserInfo data in arrConnecedtUser)
                {
                    if (data.ControlChecked == 1)
                    {
                        string strSQL = string.Format("update ControlCheck set ControlCheck = {0} where UserID = {1}", -1, data.UserID);
                        if (m_dbMgr.GetResultData(strSQL, 0) == null)
                            return;
                    }
                }
            }
        }*/

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (panelRealTimeInfo != null)
                panelRealTimeInfo.StopTimer();
            if (m_frmMain2 != null && m_frmMain2.Visible == true)
            {
                m_frmMain2.Visible = false;
                m_frmMain2.Invoke((MethodInvoker)delegate
                {
                    m_frmMain2.Close();
                    m_frmMain2.Dispose();
                });
            }

            if (m_frmReport != null)
                m_frmReport.Dispose();
        }

        #region ADD , RUN , STOP WORKFLOW
        private void AddWorkflow(TabPage page)
        {
            int tabId = -1;
            ArrayList arSections = new ArrayList();
            foreach (Control control in page.Controls)
            {
                if (control.GetType() == typeof(Sections.PanelSectionEx))
                {
                    Sections.PanelSectionEx pane = (Sections.PanelSectionEx)control;
                    tabId = pane.ActionStepID;
                    arSections.AddRange(pane.Sections);
                }
            }
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;

            SectionTabPage tabPage = (SectionTabPage)page;
            bool bTabPage = !tabPage.VirtualMode;
            tabPage.UseWaterMark = GetPageOption().GetVirtualMode();
            tabPage.VirtualMode = !FormMain.Instance.IsReal;
            manager.Add(tabPage.ActionStepID, arSections, !tabPage.VirtualMode);
        }

        public int GetTabActionStepID(TabPage tabPage)
        {
            foreach (Control control in tabPage.Controls)
            {
                if (control.GetType() == typeof(Sections.PanelSectionEx))
                {
                    Sections.PanelSectionEx pane = (Sections.PanelSectionEx)control;
                    return pane.ActionStepID;
                }
            }
            return -1;
        }

        private void RunWorkflowAsync(object sender, ProcessEventArgs ex)
        {
            if (sender == null)
            {
                return;
            }
            WorkFlowStartNotifyProcess start = (WorkFlowStartNotifyProcess)sender;
            if (start == null || start.Popup.DialogResult == DialogResult.Cancel)
            {
                bStartWorkflowEvent = false;
                return;
            }

            if (start == null || start.PopupOption.DialogResult == DialogResult.Cancel)
            {
                bStartWorkflowEvent = false;
                return;
            }

            bool bSendSMS = start.UseSMS;

            if (GetPageHome().TabControls.SelectedTab == null)
            {
                bStartWorkflowEvent = false;
                return;
            }

            WorkFlow work = RunWorkflow();

            if (work != null)
            {
                work.BeginEndEventSendSMS = bSendSMS;
                work.SOPName = start.SOPName;
				work.DetectTime = start.DetectTime;

                if (start.HasPosition)
                {
                    work.Position = start.PositionName;
                    work.HasPosition = true;
                    work.LastPosition = start.Popup.LastPoistion;
                    SetCurrentWorkflow(work);
                }
                else
                {
                    work.Position = "";
                    work.HasPosition = false;
                    work.LastPosition = null;
                }
            }
            bStartWorkflowEvent = false;
        }

        public Sections.WorkFlow GetCurrentWorkflow()
        {
            return m_currentWork;
        }

        public void SetCurrentWorkflow(Sections.WorkFlow work)
        {
            if (m_currentWork != null)
            {
                if (m_currentWork.HasPosition == true)
                {
                    if (m_currentWork.LastPosition != null)
                    {
                        m_frmMain2.LayoutForm.LastPos = m_currentWork.LastPosition;
                        m_frmMain2.LayoutForm.RemoveDisasterPos();
                    }
                }
            }

            m_currentWork = work;
            if (m_currentWork != null && m_currentWork.HasPosition == true)
            {
                if (m_currentWork.LastPosition != null)
                {
                    SOPMonitoringSystem.HistoryDiasterPosition pos = m_currentWork.LastPosition;
                    m_frmMain2.LayoutForm.LastPos = pos;
                    m_frmMain2.LayoutForm.AddDisasterPos(pos.DisasterName, pos.X, pos.Y, pos.Z);
                }
            }
        }

        public void toolstripSetting(string str)
        {
            GetPageHome().toolstripSetting(str);
        }
        public Sections.WorkFlow RunWorkflow()
        {
            PageBackstageHome pageHome = GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;
            if (page == null)
                return null;

            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;          
            
            page.State = Sections.TabPageState.USE;
            page.CreateNew = false;
            page.VirtualMode = !FormMain.Instance.IsReal;
            Sections.TabPageManager.Instance.AddPage(page, !page.VirtualMode);
            int ActionID = page.ActionStepID;
            if (!manager.Exist(ActionID, !page.VirtualMode))
            {
                AddWorkflow(page);
            }

            if (HasControl == true)
                WriteCurrentActionStepID(ActionID, !page.VirtualMode);

            page.ActionStepID = ActionID;
            Sections.TabPageManager.Instance.SetUsePage(ActionID, true, !page.VirtualMode);

            BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(ActionID);
            string szPath = node.FullPath;
            bool bHasPos = true;
			if (szPath.IndexOf("자연재해") != -1 || szPath.IndexOf("태풍") != -1)
            {
                bHasPos = false;
            }
            string sopName = szPath.Substring(szPath.IndexOf("\\") + 1);

            Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(ActionID, !page.VirtualMode);
            work.HasPosition = bHasPos;
            work.SOPName = sopName;
            if (work != null)
                work.Start();

            m_pageHome.GetDockScenario().AddGridRowScenario(szPath.Replace("\\", szDeli.ToString()), page.ActionStepID, !page.VirtualMode, page.ActionStepHistoryID);

            m_frmStatus.StatusBoard(work.State);
            SetCurrentWorkflow(work);
            EnabledRunGroup();
            return work;
        }


        private ArrayList LoadHistoryDisasterPosition()
        {
            SOPMonitoringSystem.WebDBManager webDB = SOPMonitoringSystem.FormMain.Instance.DBManager;

            string strSQL = "select top 5 Description, PosX, PosY, PosZ, FloorIndex, DisasterType, BuildingID ";
            strSQL += "from HistoryDisasterPos Where FloorIndex = -999 order by ID Desc";

            ArrayList arrResult = webDB.GetResultData(strSQL, 0);

            ArrayList result = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                try
                {
                    string strNamePos = WebDBManager.GetStringField(arrResult[i], "");
                    float posX = WebDBManager.GetFloatField((string)arrResult[i + 1], 0.0f);
                    float posY = WebDBManager.GetFloatField((string)arrResult[i + 2], 0.0f);
                    float posZ = WebDBManager.GetFloatField((string)arrResult[i + 3], 0.0f);
                    float floorIdx = WebDBManager.GetFloatField((string)arrResult[i + 4], -999.0f);
                    string strDiasterName = WebDBManager.GetStringField((string)arrResult[i + 5], "");
                    string strBuildingID = WebDBManager.GetStringField((string)arrResult[i + 6], "");

                    HistoryDiasterPosition hisPos = new HistoryDiasterPosition();
                    hisPos.PoistionName = strNamePos;
                    System.Drawing.PointF pos = new System.Drawing.PointF(posX, posY);
                    hisPos.X = posX;
                    hisPos.Y = posY;
                    hisPos.Z = posZ;
                    hisPos.DisasterName = strDiasterName;
                    hisPos.FloorIndex = floorIdx;
                    hisPos.BuildingID = strBuildingID;
                    result.Add(hisPos);
                }
                catch (Exception)
                {
                }
            }
            return result;
        }

        public void RunWorkflowWithEvent()
        {
            TabPage page = m_pageHome.tabControl.SelectedTab;
            if (page == null)
            {
                return;
            }
            if (bStartWorkflowEvent == true)
            {
                return;
            }
            bStartWorkflowEvent = true;

            Sections.SectionTabPage tabPage = (Sections.SectionTabPage)page;
            int nActionStepID = GetTabActionStepID(tabPage);
            BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(nActionStepID);
            string szName = node.FullPath;
            bool bHasPos = true;
			if (szName.IndexOf("자연재해") != -1 || szName.IndexOf("태풍") != -1)
            {
                bHasPos = false;
            }

            string sopName = szName.Substring(szName.IndexOf("\\") + 1);
            string disasterName = szName.Substring(0, szName.IndexOf("\\"));
            ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();

            if (m_smsExternalCompanyMemberOn)
            {
                // 협력업체 직원들의 전화번호 추가(시작 및 종료시에만...)
                AddExternalCompanyMemberPhoneNumbers(arrListCall);
            }

            Process.WorkFlowStartNotifyProcess start = new Process.WorkFlowStartNotifyProcess();
            start.VirtualMode = !FormMain.Instance.IsReal;
            start.ActionStepID = nActionStepID;
            start.HasPosition = bHasPos;
            start.SOPName = sopName;
            start.CallList = arrListCall;

            start.OnPostProcess += new PostProcessEvent(RunWorkflowAsync);

            Popup.PopupStartEvent form = start.Popup;
            form.DisasterName = disasterName;

            ArrayList arPosList = LoadHistoryDisasterPosition();
            form.SetRecentPosition(arPosList);

            if (m_frmMain2.LayoutForm != null)
            {
                m_frmMain2.LayoutForm.SetCheckPoistion(form, true);
            }
            Process.ProcessManager.Instance.AddFirst(start);


        }

        private void AddExternalCompanyMemberPhoneNumbers(ArrayList arrListCall)
        {
            foreach (ExternalCompanyMember member in FormMain.Instance.SOPManager.ExternalCompanyMembers)
            {
                arrListCall.Add(member.PhoneNumber);
            }
        }

        public void DoneWorkflow()
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;

            int ActionID = page.ActionStepID;
            if (Sections.WorkFlowManager.Instance.DeleteComplete == true)
            {
                Sections.TabPageManager.Instance.RemovePage(page, !page.VirtualMode);
            }

            ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();
            Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(ActionID, !page.VirtualMode);

            if (this.HasControl == true)
                if (m_smsExternalCompanyMemberOn)
                {
                    // 협력업체 직원들의 전화번호 추가(시작 및 종료시에만...)
                    AddExternalCompanyMemberPhoneNumbers(arrListCall);
                }

            Process.WorkflowEndNotifyProcess endEvent = new Process.WorkflowEndNotifyProcess();
            endEvent.VirtualMode = (work.RunMode == WorkFlowMode.VIRTUAL);
            endEvent.HasPosition = work.HasPosition;
            endEvent.SOPName = work.SOPName;
            endEvent.CallList = arrListCall;

            if (this.HasControl == true)
                endEvent.UseSMS = work.BeginEndEventSendSMS;
            else
                endEvent.UseSMS = false;

            if (work.HasPosition == true)
            {
                if (work.LastPosition != null)
                    endEvent.PositionName = work.LastPosition.PoistionName;
                else
                    endEvent.PositionName = "";
            }

            if (work != null && work.HasPosition == true)
            {
                if (work.LastPosition != null)
                {
                    m_frmMain2.LayoutForm.LastPos = work.LastPosition;
                    m_frmMain2.LayoutForm.RemoveDisasterPos();
                    work.LastPosition = null;
                    m_frmMain2.LayoutForm.LastPos = null;
                }
            }

            if (this.HasControl == true)
            {
                Process.ProcessManager.Instance.AddFirst(endEvent);
            }
            History.HistoryManager.Instance.RemoveHistoryDisasterPosition(page.ActionStepID, !page.VirtualMode);

            if (HasControl == true)
                WriteCurrentActionStepID(-1, false);

            m_frmStatus.StatusBoard(work.State);
            SetCurrentWorkflow(work);
            EnabledRunGroup();
        }

        public void StopWorkflow(DateTime dtStop, bool noDBWrite = false)
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;

            StopWorkflow(dtStop, noDBWrite, page);
        }

        public void StopWorkflow(DateTime dtStop, bool noDBWrite, int nActionStepID, bool isRealMode)
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();

            foreach (Sections.SectionTabPage page in pageHome.TabControls.Controls)
            {
                if (page.ActionStepID == nActionStepID && !page.VirtualMode == isRealMode)
                {
                    StopWorkflow(dtStop, noDBWrite, page);
                    return;
                }
            }
        }

        private void StopWorkflow(DateTime dtStop, bool noDBWrite, Sections.SectionTabPage page)
        {
            int ActionID = page.ActionStepID;
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(ActionID, !page.VirtualMode);
            if (work == null)
                return;

            manager.Remove(ActionID, !page.VirtualMode);

            Sections.TabPageManager.Instance.SetUsePage(ActionID, false, !page.VirtualMode);



            if (Sections.WorkFlowManager.Instance.DeleteComplete == true)
            {
                Sections.TabPageManager.Instance.RemovePage(page, !page.VirtualMode);
            }

            if (work != null && work.State == Sections.WorkFlowState.RUN)
            {
                work.Stop(dtStop, noDBWrite);

                if (work.HasPosition == true)
                {
                    if (work.LastPosition != null)
                    {
                        m_frmMain2.LayoutForm.LastPos = work.LastPosition;
                        m_frmMain2.LayoutForm.RemoveDisasterPos();
                        work.LastPosition = null;
                        m_frmMain2.LayoutForm.LastPos = null;
                    }
                }
            }

            History.HistoryManager.Instance.RemoveHistoryDisasterPosition(page.ActionStepID, !page.VirtualMode);

            if (HasControl == true)
                WriteCurrentActionStepID(-1, false);

            if (work == null)
                return;



            m_frmStatus.StatusBoard(Sections.WorkFlowState.STOP);
            SetCurrentWorkflow(null);
            EnabledRunGroup();
        }

        public void WaitWorkflow()
        {
            m_frmStatus.StatusBoard(Sections.WorkFlowState.STANDBY);
        }

        public void ChangeWorkflow()
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = FormMain.Instance.GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;
            if (page == null)
                return;

            int ActionID = page.ActionStepID;
            Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(ActionID, !page.VirtualMode);
            if (work != null)
            {
                m_frmStatus.StatusBoard(work.State);
                SetCurrentWorkflow(work);
            }
            else
            {
                m_frmStatus.StatusBoard(Sections.WorkFlowState.STANDBY);
            }
        }

        public void AllStopWorkflow()
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            PageBackstageHome pageHome = GetPageHome();
            Sections.SectionTabPage page = (Sections.SectionTabPage)pageHome.tabControl.SelectedTab;

            DataGridView gridView = GetPageHome().GetDockScenario().GetGridView();
            foreach (DataGridViewRow row in gridView.Rows)
            {
                int nActionID = (int)row.Cells[3].Tag;

                bool isVirtual = (bool)row.Cells[0].Tag;
                Sections.TabPageManager.Instance.SetUsePage(nActionID, false, isVirtual);
                Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(nActionID, isVirtual);
                if (work != null)
                {
                    work.Stop(DateTime.Now);

                    if (work.HasPosition == true)
                    {
                        if (work.LastPosition != null)
                        {
                            m_frmMain2.LayoutForm.LastPos = work.LastPosition;
                            m_frmMain2.LayoutForm.RemoveDisasterPos();
                            work.LastPosition = null;
                            m_frmMain2.LayoutForm.LastPos = null;
                        }
                    }
                    m_frmStatus.StatusBoard(work.State);
                }
                else
                {
                    m_frmStatus.StatusBoard(Sections.WorkFlowState.WAIT);
                }
                SetCurrentWorkflow(work);
                EnabledRunGroup();
            }

            History.HistoryManager.Instance.HistoryDisasterPosition.Clear();
        }

        public Sections.WorkFlowState CheckWorkflow(int nActionID, bool isVirtual)
        {
            Sections.WorkFlowManager manager = Sections.WorkFlowManager.Instance;
            Sections.WorkFlow work = (Sections.WorkFlow)manager.Get(nActionID, !isVirtual);

            if (work == null)
                return Sections.WorkFlowState.STANDBY;

            return work.State;
        }
        #endregion

        public void WriteCurrentActionStepID(int nActionStepID, bool isRealMode)
        {
            if (!HasControl)
                return;

            SOPManager.SetCurrentActionStep(nActionStepID, isRealMode);

            string strSQL = string.Format("Update CurrentActionStep set ActionStepID = {0}, RealMode = {1} where id = 1", nActionStepID, isRealMode ? 1 : 0);
            DBManager.GetResultData(strSQL, 0);
        }

        #region TTS
        public void StopSpeech()
        {
            TTSManager.Instance.StopSpeech();
        }
        public void PauseSpeech()
        {
            TTSManager.Instance.PauseSpeech();
        }
        public void ResumeSpeech()
        {
            TTSManager.Instance.ResumeSpeech();
        }
        #endregion

        public void InitReport()
        {
            m_frmReport = null;
        }

        public void ChangeMode(VersionInfo vInfo, ActionStepInfo aInfo, bool isRealMode)
        {
            if (vInfo.IsRegular == radioRegistMode.Checked)
            {
                if (vInfo.IsNormal == radioNormal.Checked)
                    return;
            }

            if (vInfo.IsRegular == true)
            {
                radioRegistMode.Checked = true;
                radioNonRegistMode.Checked = false;
            }
            else
            {
                radioRegistMode.Checked = false;
                radioNonRegistMode.Checked = true;
            }

            if (vInfo.IsNormal == true)
            {
                radioNormal.Checked = true;
                radioHoliday.Checked = false;
            }
            else
            {
                radioNormal.Checked = false;
                radioHoliday.Checked = true;
            }
            ChangeMode();
            BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(aInfo.ActionStepID);
            if (node != null)
            {
                TreeNode selectedNode = tree.GetSelectedNode();

                if (selectedNode != node)
                {
                    tree.SelectSop(node);

                    if (PageBackstageHome.IsWorkingMode(aInfo.ActionStepID, isRealMode))
                    {
                        // 현재 화면에 나타나고 있는 ActionStep을 기록한다.
                        WriteCurrentActionStepID(aInfo.ActionStepID, isRealMode);
                    }
                }
            }
        }

        public void ChangeSOP(VersionInfo vInfo, ActionStepInfo aInfo, bool isRealMode)
        {
            ChangeMode(vInfo, aInfo, isRealMode);

            Sections.SectionTabPage page = (Sections.SectionTabPage)m_pageHome.TabControls.SelectedTab;
            if (page == null)
                return;
            TreeNode node = m_pageHome.GetDockScenario().GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return;
            WorkFlow work = RunWorkflow();
            if( work != null)
                m_pageHome.GetDockScenario().AddGridRowScenario(node.FullPath.Replace("\\", szDeli.ToString()), page.ActionStepID, !page.VirtualMode, page.ActionStepHistoryID);
        }

        public void VirtualMode(bool bRun)
        {
            if (bRun == false)
            {
                radioRealMode.Checked = true;
                radioVirtualMode.Checked = false;
                if (m_frmStatus != null)
                {
                    m_frmStatus.RealMode(true);
                }
            }
            else
            {
                radioRealMode.Checked = false;
                radioVirtualMode.Checked = true;
                if (m_frmStatus != null)
                {
                    m_frmStatus.RealMode(false);
                }
            }
        }

        public ArrayList GetAllMemberPhoneNumber()
        {
            ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();
            return arrListCall;
        }

        public string GetActionStepPath(int actionStepID)
        {
            BarLevelTree tree = GetPageHome().GetDockScenario().GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(actionStepID);
            if (node == null)
                return "";

            string szName = node.FullPath.Replace('\\', szDeli);
            return szName;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            History.HistoryManager.Instance.Dispose();

            // 방송과 SMS전송 옵션 저장
            /*if (m_smsOn == true)
                DBManager.SaveIni("sms_on", "1", "Server Connection Info");
            else
                DBManager.SaveIni("sms_on", "0", "Server Connection Info");

            if (m_useBroadcast == true)
                DBManager.SaveIni("broadcast_on", "1", "Server Connection Info");
            else
                DBManager.SaveIni("broadcast_on", "0", "Server Connection Info");*/

            StopWriteDB();

            if (m_netMgr != null)
                m_netMgr.ReleaseThread();

            if (HasControl)
            {
                //m_netMgr.SendMessage((short)SDMS.TCP_ID.RETURN_CONTROL);
                //m_netMgr.ClientProvier.Close();
            }

            FormMain.Instance.CloseThread = true;

            Process.ProcessManager.Instance.Dispose();
            Process.TTSManager.Instance.Dispose();

            if (m_pageOption != null)
                m_pageOption.Dispose();
            if (m_pageMessage != null)
                m_pageMessage.Dispose();
            if (m_pageHome != null)
                m_pageHome.Dispose();

            //Marshal.FinalReleaseComObject(axCommandBars);
            //Marshal.FinalReleaseComObject(axSkinFramework);


            Thread.Sleep(200);
        }

        public ArrayList GetLevelMember(int nTeamID)
        {
            ArrayList arrSOPMember = new ArrayList();
            string strSQL = "select ID, MemberName, LevelID from CompanyMember where LevelID in (select id from JobLevel where LevelNo = " + nTeamID.ToString() + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);

                Sections.SOPMember member = new Sections.SOPMember();
                member.MemberID = nID;
                member.MemberName = strMemberName;
                member.LevelID = nLevelID;

                arrSOPMember.Add(member);
            }
            return arrSOPMember;
        }

        public void DoorBellCheck(bool is_checked)
        {
            DoorBellChecked = is_checked;
        }

        public void PlayDoorBell()
        {
            if (DoorBellChecked)
            {
                m_player.Open(m_dbMgr.DoorBellPath, this);
                m_player.Play(false, this, instanceHandle);
                m_player.Close(this);
            }
        }

        public void GetRealTimeInfo(string strStepMemberName, string strTeamList, string strComponentType, string strTask, string strStatus, RealTimeInfoPane.MessageType type)
        {
            int nType = 0;
            nType = GetDisasterType();

            string strReport = "";
            if (strComponentType == "프로세스")
            {
                strReport = strTeamList + strTask + " " + strStatus;
            }
            else
            {
                strReport = strStepMemberName + strTask + " " + strStatus;
            }

            SOPLog log = FormMain.Instance.LogFile;
            log.Write(string.Format("In GetRealTimeInfo, strReport : {0}\n", strReport));

            panelRealTimeInfo.RealTimeInfo = strReport;
            panelRealTimeInfo.SetForeColor(type);
            panelRealTimeInfo.DrawMovingText();

        }

        private int GetDisasterType()
        {
            string strTitle = FormMain.Instance.GetPageHome().GetDockPropertiesLevel().GetTitle();
            string[] strDisaster = strTitle.Split(szDeli);

            int nType = 0;

            if (strDisaster[0] == "자연재해")
                nType = 0;
			else if (strDisaster[0] == "태풍")
				nType = 0;
			else if (strDisaster[0] == "화재")
				nType = 1;
			else if (strDisaster[0] == "유출사고")
				nType = 2;
			else if (strDisaster[0] == "테러")
				nType = 3;
			else if (strDisaster[0] == "인명구조 및 의료지원")
				nType = 4;
			else
				nType = 5;

            return nType;
        }

        public void FocusSection(Sections.Section section)
        {
            if (section == null || EnableFocusSection == false)
                return;

            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();

            if (panel != null)
            {
                panel.FocusSection(section);
            }
        }

        public int SOPGenUserID
        {
            get { return m_nSOPGenUserID; }
        }

        public string SOPGenUserRealName
        {
            get { return m_strSOPGenUserRealName; }
        }

        public int SOPGenUserLevel
        {
            get { return m_nSOPGenUserLevel; }
        }

        /*public void ForceControl()
        {
            string strSQL = string.Format("update ControlUser set UserID = {0}", m_nSOPGenUserID);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return;

            strSQL = string.Format("update ControlCheck set ControlCheck = -1 where UserID = {0}", m_nSOPGenUserID);

            if (m_dbMgr.GetResultData(strSQL, 0) == null)
                return;
        }*/

        public void ControlUse(int nCtrlID)
        {
            // ControlCheck DB 테이블의 ControlCheck 컬럼에 update 
            // 제어권 필요없음: 0 필요함: -1 요청: 1
            switch (nCtrlID)
            {
                case ID.ID_CONTROL_RETURN:
                    SetControl(false);
                    OnEnabled(ID.ID_CONTROL_RETURN);
                    UpdateUserInfo(0);  //제어권 반납
                    break;
                case ID.ID_CONTROL_REQUEST:

                    //제어권 요청
                    //m_bRequestControl = true;
                    m_frmRequestProgress = new PopupRequestProgress();
                    m_frmRequestProgress.StartPosition = FormStartPosition.Manual;
                    Point p = GetMonitoringPosition();
                    p.X += 100;
                    p.Y += 100;
                    int width = m_frmRequestProgress.Bounds.Width;
                    int height = m_frmRequestProgress.Bounds.Height;
                    m_frmRequestProgress.SetBounds(p.X, p.Y, width, height);
                    UpdateUserInfo(1);
                    m_frmRequestProgress.Show();
                    break;
            }
        }

        private void labelRadio_Click(object sender, EventArgs e)
        {
            if (sender == labelReal)
            {
                if (radioRealMode.Enabled)
                    radioRealMode.Checked = true;
            }
            else if (sender == labelVirtual)
            {
                if (radioVirtualMode.Enabled)
                    radioVirtualMode.Checked = true;
            }
            else if (sender == labelRegular)
            {
                if (radioRegistMode.Enabled)
                    radioRegistMode.Checked = true;
            }
            else if (sender == labelNonRegular)
            {
                if (radioNonRegistMode.Enabled)
                    radioNonRegistMode.Checked = true;
            }
            else if (sender == labelNormal)
            {
                if (radioNormal.Enabled)
                    radioNormal.Checked = true;
            }
            else if (sender == labelHoliday)
            {
                if (radioHoliday.Enabled)
                    radioHoliday.Checked = true;
            }
        }

        bool prevStart = false;
        bool prevPause = false;

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool broadcastPause = (TTSManager.Instance.State == SpeechState.PAUSE ? true : false);
            bool broadcastStart = (TTSManager.Instance.State == SpeechState.PLAY ? true : false);

            if (prevStart != broadcastStart || prevPause != broadcastPause)
            {
                DateTime dtNow = DateTime.Now;
                string strTime = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);

                prevStart = broadcastStart;
                prevPause = broadcastPause;
            }
            
            btnStartBroadcast.Enabled = broadcastPause;
            //btnPauseBroadcast.Enabled = broadcastStart || broadcastPause;
            btnStopBroadcast.Enabled = broadcastStart || broadcastPause;
        }

        private void pictureBoxMainIcon_DoubleClick(object sender, EventArgs e)
        {
            History.HistoryManager.Instance.Dispose();
            this.Close();
        }
    }

    public class SOPLog
    {
        //private StreamWriter sw = new StreamWriter("log.txt", false, Encoding.UTF8);

        public void Write(string strLog)
        {
            //DateTime t = DateTime.Now;
            //sw.Write(string.Format("{0} {1} {2}", t.ToShortDateString(), t.ToShortTimeString(), strLog));
            //sw.Flush();
        }

        public void WriteLine(string strLog)
        {
            //DateTime t = DateTime.Now;
            //sw.WriteLine(string.Format("{0} {1} {2}", t.ToShortDateString(), t.ToShortTimeString(), strLog));
            //sw.Flush();
        }
    }
}
