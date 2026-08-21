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
using System.Reflection;
using UnE.Geometry;
using UnE.Utility;

using Sections;
using UnE.SOP;
using UnE.SOP.Tree;
using UnE.SOP.History;
using UnE.SOP.Log;
using UnE.SOP.Data;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;
using UnE.SOP.Process;
using UnE.SOP.TTS;
using DBUtility;

namespace SOPMonitoringSystem
{
	using Process;
	using Sections;

    public partial class FormSOP : Form, ISOPContainer, IRibbonButtonOwner, ITextPictureBoxOwner/*, SDMS.ICCTVFormOwner*/ //, SOPDisasterSystem.ISOPInfo
		, IWorkflowContainer
	{
		// 각 메인폼의 창 위치
		private int nMonitoring = 1;
		private int nDisaster = 2;
		private int nMission = 3;

		// 시스템 버튼들이 Frame 가장자리로부터 얼마나 떨어져 있는가?
		private int m_nCloseButtonPos = 0;
		private int m_nMaxButtonPos = 0;
		private int m_nMinButtonPos = 0;

		// Button별 ID
		private Dictionary<Button, int> m_dicButtonIDs = new Dictionary<Button, int>();
		private Dictionary<int, Button> m_dicIDButtons = new Dictionary<int, Button>();
		private Dictionary<Button, bool> m_dicButtonChecked = new Dictionary<Button, bool>();

		// Form Move 를 위한 Panel Top 클릭 체크
		private bool m_bLeftMouseDown = false;
		// Form Move 를 위한 Panel Top 클릭 점
		private Point m_ptMove;

		private int m_nPanelTopInitHeight = -1;

		//private SOPLog m_logFile = new SOPLog();
		//public SOPLog LogFile
		//{
		//   get { return m_logFile; }
		//}

        private UnE.SOP.SOPManager m_sopMgr = null;
		private static IntPtr instanceHandle = IntPtr.Zero;
		private MP3Player m_player = new MP3Player();

		private bool m_CloseThread = false;

        // 마지막으로 SOP가 수정된 시간
        private DBUtility.VariousData<DateTime> m_dtLastAccessedSOP = null;
        // 마지막으로 인사정보가 수정된 시간
        private DBUtility.VariousData<DateTime> m_dtLastAccessedMember = null;

		public bool CloseThread
		{
			get { return m_CloseThread; }
			set { m_CloseThread = value; }
		}

		private PageBackstageSOP m_pageHome;
		private PageBackstageOption m_pageOption;
		private PageBackStageMessage m_pageMessage;

		private bool m_isFirst = false;
        private bool m_isFirst2 = false;
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

        //private SDMS.FormFrame m_frmSDMS = null;
		//private SDMS.FormMain m_frmMain2 = null;

		private FormMissionStatus m_frmMain3 = null;
		public FormMissionStatus FrmMain3
		{
			get { return m_frmMain3; }
		}

        public new bool Visible
        {
            get
            {
                return base.Visible;
            }

            set
            {
                base.Visible = value;
            }
        }

		private PopupProgressReport m_frmReport = null;

		private WorkFlow m_currentWork = null;
		public WorkFlow CurrentWork
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

		private SOPMonitoringSystem.NetworkManager m_netMgr = null;
        
		public SOPMonitoringSystem.NetworkManager NetworkManager
		{
			get { return m_netMgr; }
		}

        private bool m_initialize = false;
        public bool Initialization
        {
            get { return m_initialize; }
        }

		//////////////////////////////////////////////////////////////////////////
		static private FormSOP m_instance = null;

		static public FormSOP Instance
		{
			get { return m_instance; }
		}

		//private int m_nSirenCount = 0;
		private int m_nSOPGenUserID = -1;
		private int m_nSOPGenUserLevel = 1;
        // m_nSOPGenUserID에 대한 평일, 주간의 최고 책임자
        private Sections.SectionCommander m_sopGenUserCommanderDayLight = null;
        // m_nSOPGenUserID에 대한 휴일, 야간의 최고 책임자
        private Sections.SectionCommander m_sopGenUserCommanderNightHoliday = null;

		private string m_strSOPGenUserRealName = "";

		private ArrayList m_arrConnectedUser = new ArrayList();
		private Thread DBWrite = null;

        private ControlTeamEditor.FormMemberWorkSchedule m_frmWorkSchedule = null;

		//private bool m_smsOn = false;
        // 협력업체들에게도 SOP 문자메시지를 보낼것인가?
		private bool m_smsExternalCompanyMemberOn = false;

        private int m_nControlUserID = -1;
        public int ControlUserID
        {
            get { return m_nControlUserID; }
            set
            {
                if (m_nControlUserID != value)
                {
                    m_nControlUserID = value;
                    m_pageHome.OnChangeControlUser();
                }
            }
        }

		public bool UseBroadcast
		{
            get { return TTSManager.Instance.UseBroadcast; }
            set { TTSManager.Instance.UseBroadcast = value; }
		}

		private bool m_showMissionText = false;
		public bool ShowMissionText
		{
			get { return m_showMissionText; }
			set { m_showMissionText = value; }
		}

        public bool VisibleMissionStatus
        {
            get { return m_frmMain3.Visible; }
            set
            {
                if (value)
                    ShowMissionStatus();
                else
                    HideMissionStatus();
            }
        }

        // Section의 임무확인/실행 버튼을 보일것인가?
        private bool m_bShowSectionBtn = false;
        public bool ShowSectionBtn
        {
            get { return m_bShowSectionBtn; }
            set 
            {
                m_bShowSectionBtn = value; 
                if( m_pageHome != null)
                {
                    m_pageHome.ShowSectionBtn(m_bShowSectionBtn);
                }
            }
        }

        private bool m_bVisiblityToPerformer = true;
        public bool VisiblityToPerformer
        {
            get { return m_bVisiblityToPerformer; }
            set
            {
                m_bVisiblityToPerformer = value;
                if (m_pageHome != null)
                {
                    m_pageHome.OnChangeVisiblityToPerformer(m_bVisiblityToPerformer);
                }
            }
        }

        // 협력업체들에게도 SOP 문자메시지를 보낼것인가?
        public bool SmsExternalCompanyMemberOn
        {
            get { return m_smsExternalCompanyMemberOn; }
            set { m_smsExternalCompanyMemberOn = value; }
        }

        public Sections.SectionCommander SOPGenUserCommanderDayLight
        {
            get { return m_sopGenUserCommanderDayLight; }
        }

        public Sections.SectionCommander SOPGenUserCommanderNightHoliday
        {
            get { return m_sopGenUserCommanderNightHoliday; }
        }

        // ShowMonitoringSystem(false) 호출에 의하여 화면에서 사라진 상태인가?
        private bool m_toggleHideStatus = false;

		// Workflow가 Event와 함께 시작되었는지 여부
		private bool bStartWorkflowEvent = false;

        // WorkFlow가 두번 종료되는 것을 막기위한 장치
        // Key : 음수이면 Virtual Mode, 양수이면 Real Mode
        //       정수값은ActionStepID
        private Dictionary<int, DateTime> m_dicWorkFlowDone = new Dictionary<int, DateTime>();
        private double m_dMinimumWorkFlowTime = 2.0;
        	

		// 제어권 요청창
		private PopupRequestProgress m_frmRequestProgress = null;

		// 제어권 요청 리스트
		PopupRequestControl m_frmRequestControl = null;

		// 현재 실행중인 SOP의 실행임무들에 대한 상세 옵션
		private Dictionary<Sections.MissionItem, MissionItemInfo> m_dicMissionInfo = new Dictionary<MissionItem, MissionItemInfo>();

        SOPManager.PopupSpecialMessage m_frmSpecialMessageHelp = null;
        UnE.GUI.DialogFormFrame m_frmForSpecialMessage = null;

		//////////////////////////////////////////////////////////////////////////
        public UnE.SOP.SOPManager SOPManager
		{
			get { return m_sopMgr; }
		}

		public WebDBManager DBManager
		{
			get { return m_dbMgr; }
		}

		// 실제 모드인가?(아니면 훈련모드인가?)
		public bool IsReal
		{
			get { return radioRealMode.Checked; }
		}

		// 등록 버전인가?
		public bool IsRegular
		{
			get { return true/*radioRegistMode.Checked*/; }
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

        // 연습모드인가?(훈련모드와 다름)
        private bool m_isSimulationMode = false;
        public bool SimulationMode
        {
            get { return m_isSimulationMode; }
            set { m_isSimulationMode = value; }
        }
        
        private bool m_onlySDMS = true;
        public bool OnlySDMS
        {
            get { return m_onlySDMS; }
            set { m_onlySDMS = value; }
        }

        // SOPSimulator.exe, SOPSimulator1.exe를 구별
        private int m_nExeIndex = 0;
        public int ExecutableIndex
        {
            get { return m_nExeIndex; }
        }

        private bool m_closedSDMS = false;
        public bool SDMSisClosed
        {
            get { return m_closedSDMS; }
        }

		public Form MainFrame
		{
			get { return SOPMonitoringSystem.FormFrame.Instance; }
		}

        /*public ProxyMessenger ProxyMessenger
        {
            get { return (ProxyMessenger)m_frmMain2.ProxyMessenger; }
        }*/

        // 마지막으로 SOP가 수정된 시간
        public DBUtility.VariousData<DateTime> LastAccessedSOPTime
        {
            get { return m_dtLastAccessedSOP; }
            set { m_dtLastAccessedSOP = value; }
        }

        // 마지막으로 인사정보가 수정된 시간
        public DBUtility.VariousData<DateTime> LastAccessedMemberTime
        {
            get { return m_dtLastAccessedMember; }
            set { m_dtLastAccessedMember = value; }
        }

        private bool m_isThumbnailMode = true;
        public bool ThumbnailMode
        {
            get { return m_isThumbnailMode; }
        }

        private bool m_useMovingText = false;
        public bool UseMovingText
        {
            get { return m_useMovingText; }
        }

        private bool m_usePopupSensorOn = false;
        public bool UsePopupSensorOn
        {
            get { return m_usePopupSensorOn; }
        }

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

        public void CreateSOPContainer(int nSOPGenUserID, string strSOPGenUserRealName, bool isSimulationMode, bool onlySDMS, int nTargetMonitor = 1)
        {
            bool bSituationRoomMode = UnE.SOP.ProxySOP.Instance.ShowCCTVForm;
            FormSOP f = new FormSOP(nSOPGenUserID, strSOPGenUserRealName, isSimulationMode, onlySDMS, nTargetMonitor, bSituationRoomMode);
        }

        public void LinkDisasterSystem(IDisasterContainer form)
        {
            //m_frmMain2 = (SDMS.FormMain)form;
            if( FormSOP.Instance != null)
            {
                FormSOP.Instance.Show();
            }
        }

		/// <summary>
		/// Target Form을 대상 모니터 중앙으로 이동
		/// </summary>
		/// <param name="target">이동할 Form</param>
		/// <param name="nMontior">대상 모니터 번호, 1부터 시작</param>
		private void MoveToScreenCenter(Form target, int nMontior)
		{
			Size size = GetMonitorSize(nMontior);
			Point p = GetMonitorPosition(nMontior);
			int x = p.X + (size.Width / 2) - (target.Size.Width / 2);
			int y = p.Y + (size.Height / 2) - (target.Size.Height / 2);
			target.Location = new Point(x, y);
		}


		/// <summary>
		/// 특정 모니터의 해상도를 구하기
		/// </summary>
		/// <param name="nMonitor">대상 모니터 번호, 1부터 시작</param>
		/// <returns>대상 모니터의 해상도</returns>
		public Size GetMonitorSize(int nMonitor)
		{
			Screen[] sc;
			sc = Screen.AllScreens;

			if (sc.Length == 0)
			{
				return new Size(10, 10);
			}

			string szNum = nMonitor.ToString();
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

			if (sc.Length >= nMonitor)
			{
				return sc[nIdx].Bounds.Size;
			}
			return new Size(10, 10);
		}

		/// <summary>
		/// 특정 모니터의 시작위치 구하기
		/// </summary>
		/// <param name="nMonitor">대상 모니터 번호, 1부터 시작</param>
		/// <returns>대상 모니터의 시작위치</returns>
		public Point GetMonitorPosition(int nMonitor)
		{
			Screen[] sc;
			sc = Screen.AllScreens;

			if (sc.Length == 0)
			{
				return new Point(0, 0);
			}

			string szNum = nMonitor.ToString();
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

			if (sc.Length >= nIdx)
			{
				return sc[nIdx].Bounds.Location;
			}
			return new Point(0, 0);
		}


		/// <summary>
		/// 특정 모니터 전체를 Form이 사용하도록 설정
		/// </summary>
		/// <param name="form">대상 Form</param>
		/// <param name="nDisplay">대상 모니터</param>
		/// <returns>true면 완료/false면 1번모니터로 설정</returns>
		private bool SetMonitorForm(Form form, int nDisplay)
		{
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
				form.Location = sc[nIdx].Bounds.Location;
				form.Size = new Size(sc[nIdx].Bounds.Width - 40, sc[nIdx].Bounds.Height - 40);
			}
			form.WindowState = FormWindowState.Maximized;

			return true;
		}

        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
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

        private void ReadSiteName()
        {
            string strSQL = "Select SiteName from Site where ID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strSiteName = WebDBManager.GetStringField(arrResult[0]);

                if (strSiteName != null)
                    UnE.SOP.ProxySOP.Instance.SiteName = strSiteName;
            }
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

        private int m_nSiteID = 1;
		public FormSOP(int nSOPGenUserID, string strSOPGenUserRealName, bool isSimulationMode, bool onlySDMS, int nTargetMonitor, bool bSituationRoomMode)
		{
            // 이전에 작업중이던 CCTVViewer가 종료되지 않은것이 있다면 모두 강제 종료시킨다.
            // SDMS로 이전 [2018/01/03] 김지웅
            /*SDMS.FormContentUnity.KillProcess("CCTVViewer");
            SDMS.FormContentUnity.KillProcess("UnitySam");
            SDMS.FormContentUnity.KillProcess("UnitySamInside");
            SDMS.FormContentUnity.KillProcess("UnityA10");
            SDMS.FormContentUnity.KillProcess("libCCTV");
            SDMS.FormContentUnity.KillProcess("EnergyOutside");
            // 프로세스 종료후 1초정도 기다린다. -> skkim 2016-02-01
            Thread.Sleep(1000);*/


            // CCTV창이 별도로 보이는 통합상황실 모드 (내부 모드는 이제 사용하지 않는다.)
            UnE.SOP.ProxySOP.Instance.ShowCCTVForm = true;// bSituationRoomMode;

            ReadSiteID();

            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			m_nTargetMonitor = nTargetMonitor;
			RibbonButton.OriginInitButtonWidth = 50;

			m_nSOPGenUserID = nSOPGenUserID;
			m_strSOPGenUserRealName = strSOPGenUserRealName;
            m_isSimulationMode = isSimulationMode;
            m_onlySDMS = onlySDMS;

            m_nExeIndex = GetExeIndex();

			InitializeComponent();

            SetDoubleBuffer(panelTop, true);
            SetDoubleBuffer(panelViewRibbonBarMiddle, true);
            SetDoubleBuffer(panelViewRibbonBarLeft, true);


			m_nPanelTopInitHeight = panelTop.Size.Height;

			m_instance = this;

            m_dbMgr = m_isSimulationMode ? new SimulationDBManager(this, m_nSiteID) : new WebDBManager(this, m_nSiteID);

            ReadSiteName();

            m_sopMgr = new UnE.SOP.SOPManager(m_dbMgr);

			ProxySOP.Instance.DBManager = m_dbMgr;
			ProxySOP.Instance.SOPDataContainer = m_sopMgr;
			ProxySOP.Instance.InvokeForm = this;
            ProxySOP.Instance.SOPContainer = this;

			ProxySOP.Instance.SOPGenUserID = m_nSOPGenUserID;
			ProxySOP.Instance.SOPUserName = strSOPGenUserRealName;
            ProxySOP.Instance.SimulationMode = m_isSimulationMode;

            SMSManagerEx.SetManager(m_isSimulationMode);

            //SetSOPGenUserCommander();
			//////////////////////////////////////////////////////////////////////////
			ProcessSectionManager pProcessManager = ProcessSectionManager.Instance;
			pProcessManager.Factory = ProcessSectionFactory.Instance;

			WorkFlowManager pWorkflowManager = WorkFlowManager.Instance;
			TabPageManager pPageManager = TabPageManager.Instance;

			TTSManager pTtsManager = TTSManager.Instance;
			pTtsManager.DBMgr = m_dbMgr;
			
			instanceHandle = this.Handle;
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

		private bool SetMonitorForm(Form form, int nDisplay, bool visible = true)
		{
            //Screen[] sc = Screen.AllScreens;
            Screen[] sc = Screen.AllScreens.OrderBy(p => p.Bounds.Location.Y).OrderBy(p => p.Bounds.Location.X).ToArray();
            if (form == null)
				return false;

			if (sc.Length == 0)
			{
				return false;
			}

			int nIdx = nDisplay - 1;
            if (nIdx < 0)
                nIdx = 0;
			if (sc.Length >= nDisplay)
			{
				form.StartPosition = FormStartPosition.Manual;
				form.Location = sc[nIdx].Bounds.Location;
				form.Size = new Size(sc[nIdx].Bounds.Width, sc[nIdx].Bounds.Height);

                if (visible)
                    form.WindowState = FormWindowState.Maximized;
                else
                    form.WindowState = FormWindowState.Minimized;
			}
			else
			{
                if (visible)
                    form.WindowState = FormWindowState.Maximized;
                else
                    form.WindowState = FormWindowState.Minimized;
			}
			return true;
		}

        public void ShowMonitoringSystem(bool visible)
        {
            // ToggleHide 상태가 아닐 경우에는 Visible True에 대하여 아무일도 하지 않는다.
            if (visible && m_toggleHideStatus == false)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                MainFrame.Visible = true;
                FormSOP.Instance.Visible = true;
                MainFrame.ShowInTaskbar = visible;
                OnlySDMS = !visible;

                try
                {
                    if (visible == false)
                    {
                        m_toggleHideStatus = true;

                        // PopupTranslucentForm 닫기
                        GetPageHome().CloseTranslucentForm();
                        GetPageHome().CloseTranslucentForm();

                        if (m_frmWorkSchedule != null && !m_frmWorkSchedule.IsDisposed && m_frmWorkSchedule.Visible)
                        {
                            m_frmWorkSchedule.Close();
                        }
                    }
                    else
                        m_toggleHideStatus = false;

                    MainFrame.Visible = visible;
                    if (visible)
                    {
                        MainFrame.WindowState = FormWindowState.Maximized;

                        /*if (m_pageCCTV.Visible == true)
                            m_pageCCTV.ShowCCTV();*/
                    }
                    else
                    {
                        //m_pageCCTV.HideCCTV();
                    }
                }
                catch (Exception)
                {
                }
            });
        }
		private int m_nTargetMonitor = 1;
		private void FormMain_Load(object sender, EventArgs e)
		{
			SetMonitorForm(MainFrame, m_nTargetMonitor);
            MainFrame.Visible = false;
			this.Visible = false;

            UnE.SOP.ProxySOP.Instance.UsePSM = ReadPSMInfo();
            UnE.SOP.ProxySOP.Instance.UseIntrusion = ReadIntrusionInfo(); 

			ReadOption();
			InitTab();
			InitPanels();

			CreateStatusForm();
			SetMonitors();
			m_pageHome.Visible = false;
			ProxySOP.Instance.NormalMode = IsNormal;
			ProxySOP.Instance.RealMode = IsReal;
			ProxySOP.Instance.RegisterMode = IsRegular;

			ProxySOP.Instance.WorkflowContainer = this;
			ProxySOP.Instance.SOPLogContainer = GetPageHome().GetDockSOPLog();
			ProxySOP.Instance.PageContainer = SOPScenarioManager.Instance;
			
			SOPScenarioManager.Instance.CreateLevelTree();
			ProxySOP.Instance.SOPTreeContainer = SOPScenarioManager.Instance.GetBarLevelTree();
			//ProxySOP.Instance.SOPDisasterContainer = (UnE.SOP.IDisasterContainer)m_frmMain2.PageHome.ContentForm;

			ProxySOP.Instance.HistoryContainer = HistoryManager.Instance;

			m_sopMgr.Load(IsRegular, IsNormal);
            SetSOPGenUserCommander();
			HistoryManager.Instance.LoadActionStepHistory(m_dbMgr);

            DataManager.Instance.Init();
			

			InitButtons();
			ResizeViewRibbonBar();

			// 초기 제어권 없음으로 설정
            // 연습모드일때는 제어권 설정
            SetControl(m_isSimulationMode);

			m_nSOPGenUserLevel = DBManager.GetGenUserLevel(m_nSOPGenUserID);

			ProxySOP.Instance.SOPUserLevel = m_nSOPGenUserLevel;

			/*m_netMgr = SOPMonitoringSystem.NetworkManager.Instance;

            //if (m_isSimulationMode)
                m_netMgrInternal = SOPMonitoringSystem.NetworkManager_Internal.Instance;*/

			m_pageOption.mPreviewBox.OriginSize = m_pageHome.tabControl.Size;
			m_pageOption.mPreviewBox.ThumbnailBackColor = m_pageHome.tabControl.BackColor;
			m_pageOption.mPreviewBox.TargetContorl = m_pageHome.tabControl;

			//labelTitle.Text += " " + GetAppVersion();

            if (m_isSimulationMode)
                labelTitle.Text = "연습용모드 - " + labelTitle.Text;

			m_pageHome.panel.Visible = SOPScenarioManager.Instance.ScenarioCount > 0;
			m_pageHome.SetBackgroundImage(false);
			
			//TTSManager.Instance.UseBroadcast = m_useBroadcast;

			ShowMissionText = m_pageOption.GetVisbleMissionText();

			if (!ProxySOP.Instance.IsOK())
			{
				int i = 0;
				i++;
			}

            /*if (!m_isFirst)
            {
                ProxyMessenger messenger = (ProxyMessenger)m_frmMain2.ProxyMessenger;

                if (!messenger.SDMSisLoading)
                    MainFrame.Visible = false;
            }*/

			timer1.Start();

            SetHiddenClockOption();
            SetMovingText();
            LoadPopupSensorOn();

            //SetTitle();
            m_netMgr = SOPMonitoringSystem.NetworkManager.Instance;

            // Splash가 종료되면 m_onlySDMS에 따라 Visible이 결정되도록 한다.
            //MainFrame.Visible = !m_onlySDMS;
            
            //MainFrame.Location = FormFrame.Instance.OriginLocation;
            //MainFrame.WindowState = FormWindowState.Maximized;


#if SAFE_KOREA_YH_2017
            // 우선적으로 문자메시지를 받을 사람들을 지정한다.
            UnE.SOP.SMS.SMSManager.Instance.SetVIPPhoneNumbers(m_dbMgr, m_nSiteID);
#endif

            LoadSopSupervisor();

            SetDayLightMode(Popup.SOPLoader.IsDayLight_NoInvoke(DateTime.Now));
		}


        private SupervisorSOPClose m_SopMonitor = null;
        public void LoadSopSupervisor()
        {
            //SDMS.ScriptProxy proxy = SDMS.ScriptProxy.Instance;

            m_SopMonitor = new SupervisorSOPClose(m_dbMgr);
            //if (HasControl)
            //{
            //    m_SopMonitor.ObtainControlAuthority();
           // }
        }

        public void TouchSection(Sections.Section section)
        {
            PanelSectionEx panel = (PanelSectionEx)section.GetParent();
            if (panel != null)
            {
                SectionTabPage page = (SectionTabPage)panel.Parent;
                if (page != null)
                {
                    int nActionHistoryID = page.ActionStepHistoryID;
                    SupervisorSOPClose.SupervisorSOPTouch(nActionHistoryID);
                    //SDMS.ScriptProxy.Instance.UserObject.SupervisorSOPTouch.Invoke(nActionHistoryID);
                    System.Diagnostics.Trace.WriteLine("Touch Section : " + section.SectionName);
                }
            }
        }



        public void ReadLastAccessedTime(ref DBUtility.VariousData<DateTime> dtSOP, ref DBUtility.VariousData<DateTime> dtMember)
        {
            string strSOPTag = "LastAccessedSOPTime";
            string strMemberTag = "LastAccessedMemberTime";

            string strSQL = string.Format("Select PropertyName, PropertyValue from OptionSOPSimulator where (PropertyName = '{0}' or PropertyName = '{1}') and SiteID = {2}",
                strSOPTag, strMemberTag, m_nSiteID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i]);
                DBUtility.VariousData<DateTime> dtValue = WebDBManager.GetDateTimeField(arrResult[i + 1]);

                if (strPropertyName == null || dtValue == null)
                    continue;

                if (string.Compare(strSOPTag, strPropertyName, true) == 0)
                    dtSOP = dtValue;
                else if (string.Compare(strMemberTag, strPropertyName, true) == 0)
                    dtMember = dtValue;
            }
        }

        /*private void SetTitle()
        {
            string strName = SOPManager.GetSOPGenUser(m_nSOPGenUserID).DayLightCommander.DisplayText;
            this.labelTitle.Text += " - " + m_nSOPGenUserID.ToString() + "(" + strName + ")";
        }*/

        private void SetSOPGenUserCommander()
        {
            Data_SOPGenUser user = this.SOPManager.GetSOPGenUser(m_nSOPGenUserID);

            if (user != null)
            {
                m_sopGenUserCommanderDayLight = user.DayLightCommander;
                m_sopGenUserCommanderNightHoliday = user.NightCommander;
                return;
            }

            string strSQL = "Select DayLight, MemberType, MemberID, DisplayText, CallerPhoneNumber from SOPGenUserCommander where SOPGenUserID = " + m_nSOPGenUserID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                int nDayLight = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -2);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -2);
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 3]);
                string strCallerPhoneNumber = WebDBManager.GetStringField(arrResult[i + 4]);

                if (strDisplayText == null)
                    strDisplayText = "";

                Sections.SectionCommander commander = IOManager.LoadCommanderTeamMember(m_dbMgr, nMemberType, nMemberID, strDisplayText);

                if (commander == null)
                    continue;

                if (nDayLight == 1)
                    m_sopGenUserCommanderDayLight = commander;
                else if (nDayLight == 0)
                    m_sopGenUserCommanderNightHoliday = commander;

                commander.CallerPhoneNumber = strCallerPhoneNumber;
            }
        }

        private void SetHiddenClockOption()
        {
            /*if (!m_frmMain2.HiddenClock)
            {
                btnSDMS.Visible = btnSOP.Visible = btnBulletin.Visible = btnMissionStatus.Visible = false;
                //btnStartBroadcast.Visible = btnPauseBroadcast.Visible = btnStopBroadcast.Visible = btnRepeatBroadcast.Visible = true;
            }*/

            string strUseBulletIn = "UseBulletIn", strUseMissionStatus = "UseMissionStatus";
            bool useBulletIn = true, useMissionStatus = true;

            string strSQL = "SELECT PropertyName, PropertyValue FROM OptionSDMS where (PropertyName ='" + strUseBulletIn + "' or PropertyName ='" + strUseMissionStatus + "') AND SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null)
            {
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
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

            btnBulletin.Enabled = useBulletIn;
            btnMissionStatus.Enabled = useMissionStatus;
        }

        private void SetMovingText()
        {
            string strSQL = strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='MovingText' AND SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = DBUtility.WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                {
                    m_useMovingText = strValue == "1";
                }
            }
        }

        private void LoadPopupSensorOn()
        {
            string strSQL = strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='PopupSensorOn' AND SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = DBUtility.WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                {
                    m_usePopupSensorOn = strValue == "1";
                }
            }
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

		private void CreateStatusForm()
		{
			m_frmStatus = new FormStatus(labelMode, pictureBoxStatus, labelStatus);
		}

        private void RunSDMS()
        {
            //return;
            /*this.Invoke((MethodInvoker)delegate
            {
                //m_frmSDMS.Show();
                //m_frmMain2.Show();
            });*/
        }

		// 모니터 출력을 지정
		private void SetMonitors()
		{
		   // m_frmMain2 = new SOPDisasterSystem.FormMain(this);
			m_frmReport = new PopupProgressReport(this);


			string szMonitoring = DBUtility.RegUtil.ReadRegValue("Monitor Info", "SOPSimulator", m_nSiteID);
			if (szMonitoring == null || szMonitoring == "")
				szMonitoring = DBManager.LoadIni("MonitoringSystem", "Monitor Info");
			int.TryParse(szMonitoring, out nMonitoring);
            
            UnE.SOP.ProxySOP.Instance.SimulatorMonitor = nMonitoring;

            int nCCTV = 3;
            string szCCTVForm = DBUtility.RegUtil.ReadRegValue("Monitor Info", "CCTV", m_nSiteID);
            if (szCCTVForm == null || szCCTVForm == "")
                szCCTVForm = DBManager.LoadIni("CCTVForm", "Monitor Info");
            int.TryParse(szCCTVForm, out nCCTV);

            UnE.SOP.ProxySOP.Instance.CCTVMontior = nCCTV;

            string szDisaster = DBUtility.RegUtil.ReadRegValue("Monitor Info", "SDMS", m_nSiteID);
			if (szDisaster == null || szDisaster == "")
				szDisaster = DBManager.LoadIni("DisasterSystem", "Monitor Info");
			int.TryParse(szDisaster, out nDisaster);

            UnE.SOP.ProxySOP.Instance.SDMSMonitor = nDisaster;

            string strExecutePath = Application.ExecutablePath.ToLower();            

            if (strExecutePath.Contains("sopsimulator1.exe"))
            {
                int temp = nMonitoring;
                nMonitoring = nDisaster;
                nDisaster = temp;
            }

			//if (m_frmMain2 == null)
			{
				//m_frmMain2 = new SDMS.FormMain(m_nSOPGenUserID, m_strSOPGenUserRealName, nDisaster, m_isSimulationMode);

                //m_frmSDMS = new SDMS.FormFrame(m_frmMain2);
			}

            //m_frmMain2.ProxyMessenger = new ProxyMessenger();

            //m_frmMain2.FormClosing += this.SDMS_FormClosing;
			this.FormClosing += this.FormMain_FormClosing;


            string szMission = DBUtility.RegUtil.ReadRegValue("Monitor Info", "MissionList", m_nSiteID);
			if (szMission == null || szMission == "")
				szMission = DBManager.LoadIni("MissionList", "Monitor Info");
			if (szMission == null || szMission.Equals(""))
			{
				szMission = "-1";
			}
			int.TryParse(szMission, out nMission);

            UnE.SOP.ProxySOP.Instance.MissionListMonitor = nMission;


            if (OnlySDMS)
                this.ShowInTaskbar = false;

            MainFrame.WindowState = FormWindowState.Normal;
			SetMonitorForm(MainFrame, nMonitoring, !OnlySDMS);
            //MainFrame.Visible = !OnlySDMS;
            MainFrame.Visible = false;
            //SetMonitorForm(m_frmSDMS, nDisaster);
            /*if (m_nTargetMonitor == 1)
                SetMonitorForm(m_frmSDMS, nDisaster);
            else
                SetMonitorForm(m_frmSDMS, nDisaster == 1 ? 2 : nDisaster);*/
			//SetMonitorForm(m_frmMain2, nDisaster);

            Thread t = new Thread(new ThreadStart(RunSDMS));
            t.Name = "RunSDMS";
            t.Start();
	
			ReportInfo();	

			m_frmMain3 = new FormMissionStatus();

			/*if (nMission != -1)
			{
				SetMonitorForm(m_frmMain3, nMission);
				m_frmMain3.ShowMaximize();
				m_frmMain3.Show();
			}*/
		}

        private void ShowMissionStatus()
        {
            if (m_frmMain3.Visible)
                return;

            if (m_frmMain3.Tag == null)
            {
                if (nMission != -1)
                    SetMonitorForm(m_frmMain3, nMission);

                m_frmMain3.Tag = true;
            }

            m_frmMain3.ShowMaximize();
            m_frmMain3.Show();
        }

        private void HideMissionStatus()
        {
            m_frmMain3.Hide();
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
		}

		private void ReadOption()
		{
			SMSManagerEx.Instance.UseSMS = LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_SMS), "문자 사용여부");

			//string strezSMSOn = DBManager.LoadIni("ez_sms_on", "Server Connection Info");
			//m_useEzSMS = strezSMSOn == "1";

            m_smsExternalCompanyMemberOn = FormSOP.Instance.LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SMS_TO_EXTERNAL_MEMBER), "외부회사직원에게 문자 전송");
			//string strSMSExternalCompanyMemberOn = DBManager.LoadIni("sms_externalCompanyMember_on", "Server Connection Info");
			//m_smsExternalCompanyMemberOn = strSMSExternalCompanyMemberOn == "1";

            TTSManager.Instance.UseBroadcast = LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_BROADCAST), "방송 사용여부");
			//m_useBroadcast = LoadDBOption(SOPSimulatorConfig.GetPropertyName(SOPSimulatorConfig.ConfigType.USE_BROADCAST), "방송 사용여부");

			string strMIssionText = DBManager.LoadIni("show_mission_text", "Server Connection Info");
			m_showMissionText = strMIssionText == "1";

            ReadLastAccessedTime(ref m_dtLastAccessedSOP, ref m_dtLastAccessedMember);

            m_bSensorDetectLoadAndPlay = LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.RUN_SOP_ON_LOADED), "센서 탐지 로딩 완료시 자동 시작", "1");

            ReadStandardActionStepNames();
		}

        private void ReadStandardActionStepNames()
        {
            string strPropertyName = "StandardActionStepNames";

            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strPropertyName + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            if (arrResult.Count == 0 || arrResult[0] == null)
            {
                InsertSOPDBOption(strPropertyName, "예방, 대비, 대응, 복구", "표준 대응단계 이름들");
                return;
            }

            string strValue = WebDBManager.GetStringField(arrResult[0], "");

            string[] stepNames = strValue.Split(',');
            List<string> actionStepNames = new List<string>();

            foreach (string strStepName in stepNames)
            {
                actionStepNames.Add(strStepName.Trim());
            }

            SectionTabControl.SetStandardActionStepNames(actionStepNames);
        }

        private bool m_bSensorDetectLoadAndPlay = false;
        public bool SensorDetectLoadAndPlay
        {
            get { return m_bSensorDetectLoadAndPlay; }
            set { m_bSensorDetectLoadAndPlay = value; }
        }

		public bool LoadDBOption(string strPropertyName, string strDescription, string szSaveDefault = "0")
		{
			string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strPropertyName + "' and SiteID = " + m_nSiteID.ToString();
			ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return false;

			if (arrResult.Count == 0 || arrResult[0] == null)
			{
                InsertSOPDBOption(strPropertyName, szSaveDefault, strDescription);
				return false;
			}

			string strValue = WebDBManager.GetStringField(arrResult[0], "");
			int nValue;

			if (!int.TryParse(strValue, out nValue))
				return false;

			return nValue == 0 ? false : true;
		}

        private int GetMaxID(string strTableName, WebDBManager dbMgr)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

		public bool InsertSOPDBOption(string strPropertyName, string strPropertyValue, string strDescription)
		{
            int nID = GetMaxID("OptionSOPSimulator", m_dbMgr) + 1;

            string strSQL = string.Format("Insert into OptionSOPSimulator (ID, PropertyName, PropertyValue, Description, SiteID) values ({0}, '{1}', '{2}', '{3}', {4})",
                nID, strPropertyName, strPropertyValue, strDescription, m_nSiteID);

            return m_dbMgr.GetResultData(strSQL, 0) != null;
		}

		private void InitTab()
		{
			m_pageHome = new PageBackstageSOP();
			//m_pageHome.Visible = true;
			m_pageOption = new PageBackstageOption();
			m_pageMessage = new PageBackStageMessage();

			//this.Controls.Add(m_pageOption);
			m_pageOption.Visible = false;
			m_pageMessage.Visible = false;

			pictureBoxOpt.Owner = this;
			pictureBoxView.Owner = this;
			pictureBoxMessage.Owner = this;
           
            if( UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                pictureBoxCCTV.Visible = false;
                GetPageHome().ShowCCTVToolStripMenuItem(false);

                //pictureBoxCCTV.Location = pictureBoxView.Location;
                //pictureBoxView.Location = pictureBoxMessage.Location;
            }
            else
            {
                pictureBoxCCTV.Owner = this;
                GetPageHome().ShowCCTVToolStripMenuItem(true);
                //pictureBoxCCTV.Location = pictureBoxView.Location;
                //pictureBoxView.Location = pictureBoxMessage.Location;
            }
            
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

            InitRibbonButton(btnWork, ID.ID_CONTROL_WORK, global::SOPMonitoringSystem.Properties.Resources.ControlTeamEditor_Normal, global::SOPMonitoringSystem.Properties.Resources.ControlTeamEditor_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null);
            InitRibbonButton(btnOption, ID.ID_SHOW_SOP_OPTION, global::SOPMonitoringSystem.Properties.Resources.SOPOption_Normal, global::SOPMonitoringSystem.Properties.Resources.SOPOption_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null);

			// 컨트롤
			InitRibbonButton(btnControl, ID.ID_CONTROL_CONTROL, global::SOPMonitoringSystem.Properties.Resources.Control_icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Control_icon_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null);
			InitRibbonButton(btnReturnControl, ID.ID_CONTROL_RETURN, global::SOPMonitoringSystem.Properties.Resources.ReturnControl_icon_Normal, global::SOPMonitoringSystem.Properties.Resources.ReturnControl_icon_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null);

			// 실행
			InitRibbonButton(btnStartSOP, ID.ID_RUN_PLAY, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Checked, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);
			InitRibbonButton(btnCancelSOP, ID.ID_RUN_CANCEL, global::SOPMonitoringSystem.Properties.Resources.CancelSOP_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.CancelSOP_Icon_Checked, global::SOPMonitoringSystem.Properties.Resources.CancelSOP_Icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);


            //InitRibbonButton(btnSDMS, 0, 
            //    global::SOPMonitoringSystem.Properties.Resources.sop화재_normal,
            //    global::SOPMonitoringSystem.Properties.Resources.sop화재_over,
            //    global::SOPMonitoringSystem.Properties.Resources.sop화재_disable,
            //    imgMouseOverBkgnd, imgCheckedBkgnd, null
            //    );
            //InitRibbonButton(btnBulletin, 0,
            //    global::SOPMonitoringSystem.Properties.Resources.sop상황판_normal,
            //    global::SOPMonitoringSystem.Properties.Resources.sop상황판_over,
            //    global::SOPMonitoringSystem.Properties.Resources.sop상황판_disable,
            //    imgMouseOverBkgnd, imgCheckedBkgnd, null
            //    );
            //InitRibbonButton(btnMissionStatus, 0,
            //    global::SOPMonitoringSystem.Properties.Resources.sop임무현황_normal,
            //    global::SOPMonitoringSystem.Properties.Resources.sop임무현황_over,
            //    global::SOPMonitoringSystem.Properties.Resources.sop임무현황_disable,
            //    imgMouseOverBkgnd, imgCheckedBkgnd, null
            //    );
            //InitRibbonButton(btnSOP, 0,
            //        global::SOPMonitoringSystem.Properties.Resources.sop_sop_normal,
            //        global::SOPMonitoringSystem.Properties.Resources.sop_sop_over,
            //        global::SOPMonitoringSystem.Properties.Resources.sop_sop_disable,
            //        imgMouseOverBkgnd, imgCheckedBkgnd, null
            //        );


            InitRibbonButton(btnSOP, 0,
                global::SOPMonitoringSystem.Properties.Resources.sop_normal_03,
                global::SOPMonitoringSystem.Properties.Resources.sop_checked_03,
                global::SOPMonitoringSystem.Properties.Resources.sop_disable_03,
                imgMouseOverBkgnd, imgCheckedBkgnd, null
                );

            InitRibbonButton(btnBulletin, 0,
                global::SOPMonitoringSystem.Properties.Resources.상황판_normal_03,
                global::SOPMonitoringSystem.Properties.Resources.상황판_checked_03,
                global::SOPMonitoringSystem.Properties.Resources.상황판_disable_03,
                imgMouseOverBkgnd, imgCheckedBkgnd, null
                );
            InitRibbonButton(btnMissionStatus, 0,
                global::SOPMonitoringSystem.Properties.Resources.임무_normal_04,
                global::SOPMonitoringSystem.Properties.Resources.임무_checked_04,
                global::SOPMonitoringSystem.Properties.Resources.임무_disable_04,
                imgMouseOverBkgnd, imgCheckedBkgnd, null
                );
            InitRibbonButton(btnSDMS, 0,
                global::SOPMonitoringSystem.Properties.Resources.화재_normal_03,
                global::SOPMonitoringSystem.Properties.Resources.화재_checked_03,
                global::SOPMonitoringSystem.Properties.Resources.화재_disable_03,
                imgMouseOverBkgnd, imgCheckedBkgnd, null
                );
            InitRibbonButton(btnDefaultCCTV, 0,
                global::SOPMonitoringSystem.Properties.Resources.SOPcctv_normal,
                global::SOPMonitoringSystem.Properties.Resources.SOPcctv_checked,
                global::SOPMonitoringSystem.Properties.Resources.SOPcctv_disable,
                imgMouseOverBkgnd, imgCheckedBkgnd, null
                );


			// 안내방송
			//InitRibbonButton(btnStartBroadcast, ID.ID_ANNOUNCE_PLAY, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Checked, global::SOPMonitoringSystem.Properties.Resources.Start_Icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);
			//InitRibbonButton(btnPauseBroadcast, ID.ID_ANNOUNCE_PAUSE, global::SOPMonitoringSystem.Properties.Resources.Pause_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Pause_Icon_Checked, global::SOPMonitoringSystem.Properties.Resources.Pause_Icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);
			//InitRibbonButton(btnStopBroadcast, ID.ID_ANNOUNCE_STOP, global::SOPMonitoringSystem.Properties.Resources.Stop_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Stop_Icon_Checked, global::SOPMonitoringSystem.Properties.Resources.Stop_Icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);
			//InitRibbonButton(btnRepeatBroadcast, ID.ID_ANNOUNCE_COUNT, global::SOPMonitoringSystem.Properties.Resources.Repeat_icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Repeat_icon_Checked, global::SOPMonitoringSystem.Properties.Resources.Repeat_icon_Disabled, imgMouseOverBkgnd, imgCheckedBkgnd, null);

			// 현황판
			InitRibbonButton(btnFitToCurrentComponent, ID.ID_VIEW_CURRENT, global::SOPMonitoringSystem.Properties.Resources.Zoom_Selected_icon_Normal, global::SOPMonitoringSystem.Properties.Resources.Zoom_Selected_icon_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null);
			InitRibbonButton(btnFitToScale, ID.ID_VIEW_SCALETOFIT, global::SOPMonitoringSystem.Properties.Resources.FitScreen_Icon_Normal, global::SOPMonitoringSystem.Properties.Resources.FitScreen_Icon_Checked, null, imgMouseOverBkgnd, imgCheckedBkgnd, null);

			ArrangeRibbonButtons();
            SetPositionTabButton();

			btnControl.Enabled = false;
			//btnPauseBroadcast.Enabled = false;

			int nEdgeThick = MainFrame == this ? 0 : FormFrame.Instance.EdgeThick;
			btnClose.Location = new Point(btnClose.Location.X - nEdgeThick, btnClose.Location.Y);
			btnMax.Location = new Point(btnMax.Location.X - nEdgeThick, btnMax.Location.Y);
			btnMin.Location = new Point(btnMin.Location.X - nEdgeThick, btnMin.Location.Y);

			m_nCloseButtonPos = MainFrame.Size.Width - btnClose.Location.X;
			m_nMaxButtonPos = MainFrame.Size.Width - btnMax.Location.X;
			m_nMinButtonPos = MainFrame.Size.Width - btnMin.Location.X;
		}

        private bool m_hasControl = false;
		public bool HasControl
		{
			get
			{
                return m_hasControl;
				//return btnControl.Text == "제어";
			}
		}

		public bool SMSOn
		{
			get { return SMSManagerEx.Instance.UseSMS; }
            set { SMSManagerEx.Instance.UseSMS = value; }
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

        private void SetControlText(bool hasControl)
        {
            if (hasControl)
            {
                btnControl.Text = "제어";
                btnReturnControl.Text = "제어권 반납";
            }
            else
            {
                btnControl.Text = "모니터링";
                btnReturnControl.Text = "제어권 요청";
            }

            m_hasControl = hasControl;
        }

		public void SetControl(bool hasControl)
		{
			if (hasControl)
			{
				btnControl.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Control_icon_Normal;
				btnControl.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Control_icon_Checked;
				//btnControl.Text = "제어";

				btnReturnControl.NormalImage = global::SOPMonitoringSystem.Properties.Resources.ReturnControl_icon_Normal;
				btnReturnControl.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.ReturnControl_icon_Checked;
				//btnReturnControl.Text = "제어권 반납";
                SetControlText(hasControl);

				if (m_netMgr != null)
				{
					// 제어권을 가지게 되면 현재 진행중인 화재 상황에 대하여 SOP List를 팝업시킨다.
					m_netMgr.ShowDetectSignal();
				}

				OnEnabled(ID.ID_CONTROL_REQUEST);
                WriteControlUserToDB();

                // 제어권 획득시에 이전에 사용중이던 UserDefinedTeam정보를 업데이트 한다.
                SectionTabPage page = (SectionTabPage)m_pageHome.TabControls.SelectedTab;
                if( page != null)
                {
                    m_pageHome.EnableButton(true);
                    m_pageHome.SOPTeamMemberManager.UpdateUsingTeams(page);
                    //m_pageHome.UpdateUsingUserDefinedTeam(page);
                }


			}
			else
			{
				btnControl.NormalImage = global::SOPMonitoringSystem.Properties.Resources.Monitoring_icon_Normal;
				btnControl.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.Monitoring_icon_Checked;
				//btnControl.Text = "모니터링";

				btnReturnControl.NormalImage = global::SOPMonitoringSystem.Properties.Resources.RequestControl_icon_Normal;
				btnReturnControl.CheckedImage = global::SOPMonitoringSystem.Properties.Resources.RequestControl_icon_Checked;
				//btnReturnControl.Text = "제어권 요청";
                SetControlText(hasControl);

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

                m_pageHome.EnableButton(false);
			}
		}

        private void WriteControlUserToDB()
        {
            string strSQL = "Select UserID from ControlUser where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            if (arrResult.Count == 0)
            {
                int nID = GetMaxID("ControlUser", m_dbMgr) + 1;

                strSQL = string.Format("Insert into ControlUser (ID, UserID, SiteID) values ({0}, {1}, {2})", nID, m_nSOPGenUserID, m_nSiteID);
                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }
            else
            {
                strSQL = "Update ControlUser set UserID = " + this.m_nSOPGenUserID.ToString() + " where SiteID = " + m_nSiteID.ToString();
                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return;
            }

            ControlUserID = m_nSOPGenUserID;
        }

		private void OnEnabled(int nID)
		{
			switch (nID)
			{
				case ID.ID_CONTROL_REQUEST:
					CommandBarControlEnabled(true);
					EnabledRunGroup();
					//GetPageHome().GetDockScenario().Enabled = true;
					GetPageHome().OnEnabled(true);
					//m_frmMain2.GetSpace().OnEnabled(true);
					//m_frmMain2.GetToolbar().Enabled = true;
					break;

				case ID.ID_CONTROL_RETURN:
					CommandBarControlEnabled(false);
					//GetPageHome().GetDockScenario().Enabled = false;
					GetPageHome().OnEnabled(false);
					//m_frmMain2.GetSpace().OnEnabled(false);
					//m_frmMain2.GetToolbar().Enabled = false;
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

           

            btnFitToCurrentComponent.Enabled = btnFitToScale.Enabled = true;

			WorkFlowManager manager = WorkFlowManager.Instance;
			PageBackstageSOP pageHome = GetPageHome();

			if(pageHome.tabControl.IsHandleCreated)
			{
				SectionTabPage page = (SectionTabPage)pageHome.tabControl.SelectedTab;
				if (page != null)
				{
					int ActionID = page.ActionStepID;
					if (WorkFlowManager.Instance.DeleteComplete == true)
					{
						TabPageManager.Instance.RemovePage(page, !page.VirtualMode);
					}
					WorkFlow work = (WorkFlow)manager.Get(ActionID, !page.VirtualMode);

					if (work != null)
                    {
                        switch (work.State)
						{
							case WorkFlowState.RUN: //시작
                                CommandBarControlEnabled(false);

                                //btnStartSOP.Enabled = false;
                                //btnCancelSOP.Enabled = true;
								break;
							case WorkFlowState.STOP: //실행취소
                                CommandBarControlEnabled(true);
                                break;
                            case WorkFlowState.DONE: //완료
                                CommandBarControlEnabled(true);
                                //btnStartSOP.Enabled = true;
                                btnCancelSOP.Enabled = false;
								break;
							case WorkFlowState.STANDBY: //대기
							case WorkFlowState.PAUSE:
							case WorkFlowState.WAIT:
							case WorkFlowState.DISABLE:
								break;
						}
					}
					else
					{
                        CommandBarControlEnabled(true);

                        //btnStartSOP.Enabled = true;
                        //btnCancelSOP.Enabled = false;
					}
				}
			}
		}

        public void EmptySOP()
        {
            btnStartSOP.Enabled = btnCancelSOP.Enabled = false;
            btnFitToCurrentComponent.Enabled = btnFitToScale.Enabled = false;

            panelNormalMode.Visible = false;
        }

		public PageBackstageSOP GetPageHome()
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
            WorkFlowManager manager = WorkFlowManager.Instance;
            PageBackstageSOP pageHome = GetPageHome();

            WorkFlowState workState = WorkFlowState.DISABLE;

            if (pageHome.tabControl.IsHandleCreated)
            {
                SectionTabPage page = (SectionTabPage)pageHome.tabControl.SelectedTab;
                if (page != null)
                {
                    int ActionID = page.ActionStepID;
                    if (WorkFlowManager.Instance.DeleteComplete == true)
                    {
                        TabPageManager.Instance.RemovePage(page, !page.VirtualMode);
                    }
                    WorkFlow work = (WorkFlow)manager.Get(ActionID, !page.VirtualMode);

                    if (work != null)
                    {
                        workState = work.State;
                    }
                }
            }

            radioRealMode.Enabled =
            radioVirtualMode.Enabled =
            /*radioNormal.Enabled =
            radioHoliday.Enabled =*/
            labelReal.Enabled =
            labelVirtual.Enabled =
            /*labelNormal.Enabled =
            labelHoliday.Enabled =*/ (workState != WorkFlowState.RUN ? isFlag : false);

            btnStartSOP.Enabled = isFlag;
            btnCancelSOP.Enabled = isFlag;
            //radioRealMode.Enabled = isFlag;
            //radioVirtualMode.Enabled = isFlag;
            //radioRegistMode.Enabled = isFlag;
            //radioNonRegistMode.Enabled = isFlag;
            //radioNormal.Enabled = isFlag;
            //radioHoliday.Enabled = isFlag;
            //btnStartBroadcast.Enabled = isFlag;
            //btnPauseBroadcast.Enabled = isFlag;
            //btnStopBroadcast.Enabled = isFlag;
            //btnRepeatBroadcast.Enabled = isFlag;
            //labelReal.Enabled = isFlag;
            //labelVirtual.Enabled = isFlag;
            //labelRegular.Enabled = isFlag;
            //labelNonRegular.Enabled = isFlag;
            //labelNormal.Enabled = isFlag;
            //labelHoliday.Enabled = isFlag;

            //if (WorkFlowManager.Instance.RealWorkFlowList.Count == 0 && WorkFlowManager.Instance.VirtualWorkFlowList.Count == 0)
            //{
            //    btnCancelSOP.Enabled = false;
            //}

            if (workState == WorkFlowState.RUN && HasControl == true)
                btnCancelSOP.Enabled = true;

            if (m_pageHome != null)
            {
                if (m_pageHome.Visible == true)
                {
                    if (m_pageHome.TabControls.IsHandleCreated)
                    {
                        if (m_pageHome.TabControls.Visible == false)
                        {
                            btnStartSOP.Enabled = false;
                        }
                    }
                    else
                    {
                        btnStartSOP.Enabled = false;
                    }
                }

                if (btnCancelSOP.Enabled == true)
                {
                    if (btnStartSOP.Enabled == true)
                    {
                        if (workState == WorkFlowState.DISABLE)
                            btnCancelSOP.Enabled = false;
                        else if (workState == WorkFlowState.RUN)
                            btnStartSOP.Enabled = false;
                    }
                    else
                    {
                        if (workState == WorkFlowState.DISABLE)
                            btnCancelSOP.Enabled = false;
                    }
                }
            }

        }

        private void SetPositionTabButton()
        {
            pictureBoxOpt.Visible = false;

            pictureBoxView.Location = pictureBoxOpt.Location;
            pictureBoxCCTV.Location = new Point(pictureBoxView.Location.X + pictureBoxView.Size.Width, pictureBoxCCTV.Location.Y);

            //pictureBoxOpt.Size = new Size(btnControl.Location.X - pictureBoxOpt.Location.X + panelViewRibbonBarMiddle.Location.X - 5, pictureBoxOpt.Size.Height);
            //pictureBoxView.Size = new Size(pictureBox4.Location.X - btnControl.Location.X + panelViewRibbonBarMiddle.Location.X - 10, pictureBoxView.Size.Height);

            //pictureBoxView.Location = new Point(pictureBoxOpt.Location.X + pictureBoxOpt.Size.Width, pictureBoxView.Location.Y);
            //pictureBoxCCTV.Location = new Point(pictureBoxView.Location.X + pictureBoxView.Size.Width, pictureBoxCCTV.Location.Y);
        }

        private void ArrangeRibbonButtons()
        {
            ArrangeRibbonButton(btnWork, btnOption);
            ArrangeRibbonButton(btnOption, pictureBox8, btnControl);
            ArrangeRibbonButton(btnControl, btnReturnControl);

            ArrangeRibbonButton(btnReturnControl, pictureBox2, panelMode);
            ArrangeRibbonButton(panelMode, pictureBox1, btnStartSOP);
            ArrangeRibbonButton(btnStartSOP, btnCancelSOP);

            //ArrangeRibbonButton(panelRealMode, pictureBox3, panelNormalMode);
            //ArrangeRibbonButton(panelRealMode, pictureBox3, panelRegistMode);
            //ArrangeRibbonButton(panelRegistMode, panelNormalMode);

            //if (m_frmMain2.HiddenClock)
            {
                ArrangeRibbonButton(btnCancelSOP, pictureBox4, btnSDMS);
                ArrangeRibbonButton(btnSDMS, btnSOP);
                ArrangeRibbonButton(btnSOP, btnBulletin);
                ArrangeRibbonButton(btnBulletin, btnMissionStatus);
                ArrangeRibbonButton(btnMissionStatus, btnDefaultCCTV);
            }
            /*else
            {
                ArrangeRibbonButton(panelNormalMode, pictureBox4, btnStartBroadcast);
                ArrangeRibbonButton(btnStartBroadcast, btnPauseBroadcast);
                ArrangeRibbonButton(btnPauseBroadcast, btnStopBroadcast);
                ArrangeRibbonButton(btnStopBroadcast, btnRepeatBroadcast);
            }*/

            ArrangeRibbonButton(btnDefaultCCTV, pictureBox5, btnFitToCurrentComponent);
            //ArrangeRibbonButton(btnRepeatBroadcast, pictureBox5, btnFitToCurrentComponent);
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
            btn.ClickedImage = imgChecked;
            btn.MouseOverImage = imgNormal;
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

        private ExecuteManager m_ExeManager = new ExecuteManager();

		char szDeli = (char)0x06;
		public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
		{
			RibbonButton btn = (RibbonButton)sender;
			int nButtonID = GetButtonID(btn);

			switch (nButtonID)
			{
                case ID.ID_SHOW_SOP_OPTION:
                    SelectOptionTab();
                    break;
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
						TreeNode node = SOPScenarioManager.Instance.GetBarLevelTree().GetSelectedNode();
						if (node == null) return;
						SOPScenarioManager.Instance.DeleteGridRowScenario(node.FullPath.Replace("\\", szDeli.ToString()));
						DoneWorkflow();
					}
					break;
                case ID.ID_CONTROL_WORK:
                    {
                        if (m_frmWorkSchedule == null || m_frmWorkSchedule.IsDisposed)
                        {
                            m_frmWorkSchedule = new ControlTeamEditor.FormMemberWorkSchedule(m_nSiteID);
                            //m_frmWorkSchedule.WindowState = FormWindowState.Maximized;
                            //m_frmWorkSchedule.StartPosition = FormStartPosition.CenterScreen;
                            m_frmWorkSchedule.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

                            m_frmWorkSchedule.MemberWorkDataChanged += m_frmWorkSchedule_MemberWorkDataChanged;

                            GetPageHome().ShowTranslucentForm(m_frmWorkSchedule, ID.ID_CONTROL_WORK);
                        }
                        else
                        {
                            GetPageHome().CloseTranslucentForm();
                        }

                        //if (!m_frmWorkSchedule.Visible)
                        //    m_frmWorkSchedule.Show(this);
                        //else
                        //    m_frmWorkSchedule.Close();
                        //exeManager.Run(ExecuteManager.APP_TYPE.CONTROLROOM_WORKER_EDITOR);
                    }
                    break;
				/*case ID.ID_ANNOUNCE_PLAY:
					btnStartBroadcast.Enabled = false;
					btnStopBroadcast.Enabled = true;
					ResumeSpeech();
					break;
				case ID.ID_ANNOUNCE_PAUSE:
					btnStartBroadcast.Enabled = true;
					btnStopBroadcast.Enabled = true;
					PauseSpeech();
					break;*/
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
				OnChangeMode();
			}
		}

		private void radioRegistMode_CheckedChanged(object sender, EventArgs e)
		{
			RadioButton btn = (RadioButton)sender;
			if (btn == null)
				return;

			if (btn.Checked)
			{
				OnChangeMode();
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

				//radioNonRegistMode.Enabled = !isRealMode;
				SOPScenarioManager.Instance.GetBarLevelTree().SelectSop(null);

				m_frmStatus.RealMode(isRealMode);

				OnChangeMode();
			}
		}

		private void OnChangeMode()
		{
			if (!m_sopMgr.IsOpened)
				return;

			bool isRegular = true;//radioRegistMode.Checked;
			bool isNormal = radioNormal.Checked;
            bool isReal = radioRealMode.Checked;

            ProxySOP.Instance.NormalMode = isNormal;
            ProxySOP.Instance.RealMode = isReal;
            ProxySOP.Instance.RegisterMode = IsRegular;

			BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();

			if (tree.IsRegular != isRegular || tree.IsNormal != isNormal)
				tree.Load(m_sopMgr, isRegular, isNormal);

			m_pageOption.mPreviewBox.Refresh();
		}

		public void ChangeMode(bool isReal, bool isRegular, bool isNormal)
		{
			if (radioRealMode.Checked == isReal &&
				/*radioRegistMode.Checked == isRegular &&*/
				radioNormal.Checked == isNormal)
				return;

			radioRealMode.Checked = isReal;
			radioVirtualMode.Checked = !isReal;

			//radioRegistMode.Checked = isRegular;
			//radioNonRegistMode.Checked = !isRegular;

			radioNormal.Checked = isNormal;
			radioHoliday.Checked = !isNormal;

			OnChangeMode();
		}

		public bool Play()
		{
			if (btnStartSOP.Enabled == false)
				return false;

			//GetPageHome().ClearProcess();

            if (m_pageHome.TabControls.IsHandleCreated == false)
                return false;

			TabPage tapPage = m_pageHome.TabControls.SelectedTab;
			if (tapPage == null || tapPage.GetType() != typeof(SectionTabPage))
				return false;
			SectionTabPage page = (SectionTabPage)tapPage;
			if (page == null)
				return false;

			// 각 Section들의 CompleteCounte를 모두 초기화한다.
			InitCompleteCount(page);

			/*TreeNode node = SOPScenarioManager.Instance.GetBarLevelTree().FindActionStepNode(page.ActionStepID);
			if (node == null)
				return false;*/

			RunWorkflowWithEvent();

            CommandBarControlEnabled(false);

			return true;
		}

        public WorkflowOption Play(DateTime sopTime, int nSensorHistoryID)
        {
            if (btnStartSOP.Enabled == false)
                return null;

            //GetPageHome().ClearProcess();

            if (m_pageHome.TabControls.IsHandleCreated == false)
                return null;

            TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            if (tapPage == null || tapPage.GetType() != typeof(SectionTabPage))
                return null;
            SectionTabPage page = (SectionTabPage)tapPage;
            if (page == null)
                return null;

            // 각 Section들의 CompleteCounte를 모두 초기화한다.
            InitCompleteCount(page);

            /*TreeNode node = SOPScenarioManager.Instance.GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return false;*/

            WorkflowOption option = RunWorkflowWithoutEventWithoutPosition(sopTime);

            int nActionID = page.ActionStepID;
            if (HasControl == true)
                WriteCurrentActionStepID(nActionID, !page.VirtualMode);

            CommandBarControlEnabled(false);

            return option;
        }

		public bool PlayWithDisasterPosition(int nZoneID, int nSensorID, int nSensorHistoryID)
		{

            if (btnStartSOP.Enabled == false)
                return false;

            //GetPageHome().ClearProcess();

            if (m_pageHome.TabControls.IsHandleCreated == false)
                return false;

            TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            if (tapPage == null || tapPage.GetType() != typeof(SectionTabPage))
                return false;
            SectionTabPage page = (SectionTabPage)tapPage;
            if (page == null)
                return false;

            // 각 Section들의 CompleteCounte를 모두 초기화한다.
            InitCompleteCount(page);

            /*TreeNode node = SOPScenarioManager.Instance.GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return false;*/

            RunWorkflowWithoutEvent(nZoneID, nSensorID, nSensorHistoryID);

            int nActionID = page.ActionStepID;
            if (HasControl == true)
                WriteCurrentActionStepID(nActionID, !page.VirtualMode);
           
            CommandBarControlEnabled(false);

            return true;

            //if (btnStartSOP.Enabled == false)
            //    return false;
            
            //GetPageHome().ClearProcess();

            //TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            //if (tapPage == null || tapPage.GetType() != typeof(SectionTabPage))
            //    return false;
            //SectionTabPage page = (SectionTabPage)tapPage;
            //if (page == null)
            //    return false;

            //// 각 Section들의 CompleteCounte를 모두 초기화한다.
            //InitCompleteCount(page);

            //TreeNode node = SOPScenarioManager.Instance.GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            //if (node == null)
            //    return false;			

            //RunWorkflowWithoutEvent(nZoneID, nSensorID, nSensorHistoryID);

            //return true;
		}

		public void RunWorkflowWithoutEvent(int nZoneID, int nSensorID, int nSensorHistoryID)
		{
            try
            {
                TabPage page = m_pageHome.tabControl.SelectedTab;
                if (page == null)
                {
                    return;
                }

                FireDetectSignal signal = m_netMgr.FindDetectSignal(nSensorHistoryID);
                if (signal != null)
                {
                    SectionTabPage tabPage = (SectionTabPage)page;
                    int nActionStepID = GetTabActionStepID(tabPage);
                    BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
                    TreeNode node = tree.FindActionStepNode(nActionStepID);
                    string szName = node.FullPath;

                    string[] nodeNames = szName.Split('\\');
                    string strCategoryName = nodeNames[0];

                    bool bHasPos = true;
                    if (strCategoryName == "자연재해" || strCategoryName == "태풍")
                    //if (szName.IndexOf("자연재해") != -1 || szName.IndexOf("태풍") != -1)
                    {
                        bHasPos = false;
                    }

                    bool usePSM = strCategoryName == "유출사고";

                    string sopName = szName.Substring(szName.IndexOf("\\") + 1);
                    string disasterName = szName.Substring(0, szName.IndexOf("\\"));
                    ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();

                    if (m_smsExternalCompanyMemberOn)
                    {
                        // 협력업체 직원들의 전화번호 추가
                        AddExternalCompanyMemberPhoneNumbers(arrListCall);
                    }

                    Process.WorkFlowStartNotifyProcess start = new Process.WorkFlowStartNotifyProcess(strCategoryName, sopName, tabPage);
                    start.VirtualMode = !FormSOP.Instance.IsReal;
                    start.ActionStepID = nActionStepID;
                    start.Option.HasPosition = bHasPos;
                    //start.UsePSM = usePSM;
                    //start.SOPName = sopName;
                    start.CallList = arrListCall;
                    start.NoPopup = true;
                

                    // 화재신호로 수행된 SOP는 화재발생문자가 대부분 나간경우 이므로 시작/종료문자는 보내지 않는다.
                    start.Option.UseSmsMessage = false;
                    start.Option.PositionName = signal.PositionName;
                    //start.PositionName = signal.PositionName;
                    start.Option.DetectTime = new VariousData<DateTime>(signal.DetectTime);
                    start.OnPostProcess += new PostProcessEvent(RunWorkflowAsync);
                    start.Option.SensorZoneID = nSensorID;
                    start.Option.SensorZoneHistoryID = nSensorHistoryID;

                    Zone zone = DataManager.Instance.GetZone(nZoneID);
                    HistoryDisasterPosition disasterPos = new HistoryDisasterPosition();
                    disasterPos.PoistionName = signal.PositionName;
                    disasterPos.DisasterName = "화재";

                    Vertex2D pos3D = zone.Polygon == null ? new Vertex2D() : zone.Polygon.CalcWeightCenter();
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

                    Popup.PopupStartEvent form = start.Popup;
                    form.DisasterName = disasterName;
                    form.AddLastHistoryDisasterPoistion(disasterPos);

                    IDisasterContainer diasterForm = ProxySOP.Instance.SOPDisasterContainer;
                    if (diasterForm != null)
                    {
                        diasterForm.SetCheckPoistion(form, true);
                    }
                    ProcessSectionManager.Instance.AddFirst(start);


                    m_pageHome.toolstripSetting("");
                }
                else
                {

                    SectionTabPage tabPage = (SectionTabPage)page;
                    int nActionStepID = GetTabActionStepID(tabPage);
                    if (nActionStepID < 0)
                        return;

                    BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
                    TreeNode node = tree.FindActionStepNode(nActionStepID);
                    string szName = node.FullPath;

                    string[] nodeNames = szName.Split('\\');
                    string strCategoryName = nodeNames[0];

                    bool bHasPos = true;
                    if (strCategoryName == "자연재해" || strCategoryName == "태풍")
                    //if (szName.IndexOf("자연재해") != -1 || szName.IndexOf("태풍") != -1)
                    {
                        bHasPos = false;
                    }

                    bool usePSM = strCategoryName == "유출사고";

                    string sopName = szName.Substring(szName.IndexOf("\\") + 1);
                    string disasterName = szName.Substring(0, szName.IndexOf("\\"));
                    ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();

                    if (m_smsExternalCompanyMemberOn)
                    {
                        // 협력업체 직원들의 전화번호 추가
                        AddExternalCompanyMemberPhoneNumbers(arrListCall);
                    }

                    Process.WorkFlowStartNotifyProcess start = new Process.WorkFlowStartNotifyProcess(strCategoryName, sopName, tabPage);
                    start.VirtualMode = !FormSOP.Instance.IsReal;
                    start.ActionStepID = nActionStepID;
                    start.Option.HasPosition = bHasPos;
                    //start.UsePSM = usePSM;
                    //start.SOPName = sopName;
                    start.CallList = arrListCall;
                    start.NoPopup = true;

                    Zone zone = DataManager.Instance.GetZone(nZoneID);
                    string szPositionName = zone.DisplayName;

                    // 센서신호로 수행된 SOP는 화재발생문자가 대부분 나간경우 이므로 시작/종료문자는 보내지 않는다.
                    start.Option.UseSmsMessage = false;
                    start.Option.PositionName = szPositionName;
                    start.Option.DetectTime = new VariousData<DateTime>(DateTime.Now);
                    start.Option.SensorZoneID = nSensorID;
                    start.Option.SensorZoneHistoryID = nSensorHistoryID;

                    start.OnPostProcess += new PostProcessEvent(RunWorkflowAsync);
                                        
                    HistoryDisasterPosition disasterPos = new HistoryDisasterPosition();
                    disasterPos.PoistionName = szPositionName;
                    disasterPos.DisasterName = strCategoryName;

                    Vertex2D pos3D = zone.Polygon == null ? new Vertex2D() : zone.Polygon.CalcWeightCenter();
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

                    Popup.PopupStartEvent form = start.Popup;
                    form.DisasterName = disasterName;
                    form.AddLastHistoryDisasterPoistion(disasterPos);

                    IDisasterContainer diasterForm = ProxySOP.Instance.SOPDisasterContainer;
                    if (diasterForm != null)
                    {
                        diasterForm.SetCheckPoistion(form, true);
                    }
                    ProcessSectionManager.Instance.AddFirst(start);


                    m_pageHome.toolstripSetting("");
                }
              
            }
            catch(Exception exx)
            {
                System.Diagnostics.Trace.WriteLine(exx.Message);
                System.Diagnostics.Trace.WriteLine(exx.StackTrace);
                int i = 0;
                i++;
            }
			
		}

        public WorkflowOption RunWorkflowWithoutEventWithoutPosition(DateTime sopTime)
        {
            try
            {
                TabPage page = m_pageHome.tabControl.SelectedTab;
                if (page == null)
                {
                    return null;
                }

                SectionTabPage tabPage = (SectionTabPage)page;
                int nActionStepID = GetTabActionStepID(tabPage);
                if (nActionStepID < 0)
                    return null;

                BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
                TreeNode node = tree.FindActionStepNode(nActionStepID);
                string szName = node.FullPath;

                string[] nodeNames = szName.Split('\\');
                string strCategoryName = nodeNames[0];

                bool bHasPos = false;
                    
                string sopName = szName.Substring(szName.IndexOf("\\") + 1);
                string disasterName = szName.Substring(0, szName.IndexOf("\\"));
                ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();

                if (m_smsExternalCompanyMemberOn)
                {
                    // 협력업체 직원들의 전화번호 추가
                    AddExternalCompanyMemberPhoneNumbers(arrListCall);
                }

                Process.WorkFlowStartNotifyProcess start = new Process.WorkFlowStartNotifyProcess(strCategoryName, sopName, tabPage);
                start.VirtualMode = !FormSOP.Instance.IsReal;
                start.ActionStepID = nActionStepID;
                start.Option.HasPosition = bHasPos;
                start.CallList = arrListCall;
                start.NoPopup = true;

                // 외부신호로 수행된 SOP는 재난발생문자가 대부분 나간 경우이므로 시작/종료문자는 보내지 않는다.
                start.Option.UseSmsMessage = false;
                start.Option.DetectTime = new VariousData<DateTime>(sopTime);
                    
                start.OnPostProcess += new PostProcessEvent(RunWorkflowAsync);

                ProcessSectionManager.Instance.AddFirst(start);

                m_pageHome.toolstripSetting("");
                return start.Option;
            }
            catch (Exception exx)
            {
                System.Diagnostics.Trace.WriteLine(exx.Message);
                System.Diagnostics.Trace.WriteLine(exx.StackTrace);
            }

            return null;
        }

		private void InitCompleteCount(SectionTabPage tabPage)
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
                    Point ptCur = MainFrame.Location;

                    if (MainFrame is SOPMonitoringSystem.FormFrame)
                        FormFrame.Instance.ToNormalWindow();

					Point pt = panelTop.PointToScreen(new Point(e.X, e.Y));
					int dx = pt.X - m_ptMove.X;
					int dy = pt.Y - m_ptMove.Y;
					if (!(dx == 0 && dy == 0))
					{
						
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

		public void btnClose_Click(object sender, EventArgs e)
		{
            /*if (PopupMissionText.Instance != null && PopupMissionText.Instance.Visible == true)
                PopupMissionText.Instance.Close();

            ShowMonitoringSystem(false);*/
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
                // 탭버튼에 대한 액션 삭제
                //SelectOptionTab();
			}
			else if (pictureBox == pictureBoxView)
			{
                SelectViewTab();
			}
			else if (pictureBox == pictureBoxMessage)
			{
				SelectMessageTab();
			}
            else if( pictureBox == pictureBoxCCTV )
            {
                //SelectCCTVTab();
            }
		}

		public void TextPictureBox_MouseUp(TextPictureBox pictureBox, MouseEventArgs e)
		{
		}

		public void SelectMessageTab()
		{
            //if (PopupTranslucentForm.IsShowDialog())
            //    return;

			pictureBoxOpt.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
			pictureBoxView.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            pictureBoxMessage.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Pressed;
            pictureBoxCCTV.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;

            // 옵션창은 팝업으로 연결하기 때문에 속성적용을 안함.
            //m_pageOption.Visible = false;
			m_pageHome.Visible = false;
			m_pageMessage.Visible = true;
            
			panelTop.Size = new Size(panelTop.Size.Width, panelViewRibbonBarLeft.Location.Y);

			panelMain.Location = new Point(0, panelTop.Size.Height);
			panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelTop.Size.Height);
		}

		public void SelectOptionTab()
		{
            // Added by mwkim 2015.10.28 
            // 옵션창을 탭페이지형식으로 보여주지 않고 팝업으로 하도록 변경
            
            if (m_pageOption.Visible == true)
            {
                GetPageHome().CloseTranslucentForm();
            }
            else
            {
                GetPageHome().CloseTranslucentForm();

                // 옵션창 열기 전에 옵션내용 초기화 추가 added by skkim 2017.10.26
                m_pageOption.Initialize();

                GetPageHome().ShowTranslucentForm(m_pageOption, 180, 100, 979, 440, ID.ID_SHOW_SOP_OPTION);
            }

            return;
            // 아래 코드는 팝업으로 적용전 코드 (2015.10.28 mwkim)

            //if (PopupTranslucentForm.IsShowDialog())
            //    return;
            
            pictureBoxOpt.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Pressed;
			pictureBoxView.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
			pictureBoxMessage.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            pictureBoxCCTV.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;

            m_pageOption.Initialize();
            

			m_pageOption.Visible = true;
			m_pageHome.Visible = false;
			m_pageMessage.Visible = false;
            
			panelTop.Size = new Size(panelTop.Size.Width, panelViewRibbonBarLeft.Location.Y);

			panelMain.Location = new Point(0, panelTop.Size.Height);
			panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelTop.Size.Height);
		}

        public void PopupQuickButtonSetup()
        {
            SelectOptionTab();
            Popup.PopupQuickButtonSetup form = new Popup.PopupQuickButtonSetup();
            GetPageHome().ShowTranslucentForm(form, 200, 50, form.Width, form.Size.Height, ID.ID_SHOW_QUICK_MENU);

            form.FormClosed += (s, e) => { ClosedOptionSubPopup(); };
        }

        public void PopupSelectFireSensorSOPLink()
        {
            SelectOptionTab();
            Popup.PopupSelectFireSensorSOPLink form = new Popup.PopupSelectFireSensorSOPLink(FormSOP.Instance.DBManager, m_nSiteID);
            GetPageHome().ShowTranslucentForm(form, 200, 100, form.Width, form.Size.Height, ID.ID_SHOW_FIRE_SENSOR_SOP_LINK);

            form.FormClosed += (s, e) => { ClosedOptionSubPopup(); };
        }

        public void PopupSelectPSMSensorSOPLink()
        {
            SelectOptionTab();
            Popup.PopupSelectPSMSensorSOPLink form = new Popup.PopupSelectPSMSensorSOPLink(FormSOP.Instance.DBManager, m_nSiteID);
            GetPageHome().ShowTranslucentForm(form, 200, 100, form.Width, form.Size.Height, ID.ID_SHOW_PSM_SENSOR_SOP_LINK);

            form.FormClosed += (s, e) => { ClosedOptionSubPopup(); };
        }

        public void PopupSelectIntrusionSensorSOPLink()
        {
            SelectOptionTab();
            Popup.PopupSelectIntrusionSensorSOPLink form = new Popup.PopupSelectIntrusionSensorSOPLink(FormSOP.Instance.DBManager, m_nSiteID);
            GetPageHome().ShowTranslucentForm(form, 200, 100, form.Width, form.Size.Height, ID.ID_SHOW_INTRUSION_SENSOR_SOP_LINK);

            form.FormClosed += (s, e) => { ClosedOptionSubPopup(); };
        }

        private void ClosedOptionSubPopup()
        {
            SelectOptionTab();
        }

		public void SelectViewTab(bool showPageHome = true)
		{
          //  if (PopupTranslucentForm.IsShowDialog())
           //     return;

			pictureBoxOpt.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            pictureBoxView.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Pressed;
			pictureBoxMessage.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;
            pictureBoxCCTV.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.Tab_Normal;

            // 옵션창은 팝업으로 연결하기 때문에 속성적용을 안함.
            //m_pageOption.Visible = false;
			m_pageHome.Visible = showPageHome;
			m_pageMessage.Visible = false;

			panelTop.Size = new Size(panelTop.Size.Width, m_nPanelTopInitHeight);

			panelMain.Location = new Point(0, panelTop.Size.Height);
			panelMain.Size = new Size(this.Size.Width, this.Size.Height - panelTop.Size.Height);

            panelNormalMode.Visible = true;
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
				if (!m_sopMgr.IsOpened)
					return;

				if (m_pageHome == null || m_pageOption == null || m_pageMessage == null)
					return;

                m_isFirst = true;
				

                AddForm(m_pageHome, panelMain);
				
                //AddForm(m_pageOption, panelMain);
				AddForm(m_pageMessage, panelMain);
               
				m_pageOption.Visible = false;
				m_pageHome.Show();
				
				if (m_sopMgr.IsOpened)
				{
					if (LoadSOP())
					{
						// 기존에 실행되고 있던 SOP를 불러온다.
						LoadHistory();
					}
					LoadCompanyMember();
				}

                m_initialize = true;
				m_pageHome.Visible = true;


                // sop 초기에안보이도록 수정
				this.Visible = true;
				MainFrame.Visible = true;

                /*this.Visible = false;
                MainFrame.Visible = false;*/

                if (!m_isFirst2)
                {
                    if (m_pageHome.Visible)
                    {
                        m_isFirst2 = true;
                        SectionTabPage page = m_pageHome.TabControls.GetValidTabPageCount() > 0 ? (SectionTabPage)m_pageHome.TabControls.SelectedTab : null;

                        if (page != null)
                        {
                            int nActionStepID = page.ActionStepID;
                            if (nActionStepID != 0)
                            {
                                m_isOpen = true;
                                EnableOptions(false);

                                FormSOP.Instance.GetPageHome().panel.Visible = true;
                                FormSOP.Instance.GetPageHome().SetBackgroundImage(true);

                                radioRealMode.Checked = !page.VirtualMode;
                                ActionStepInfo info = FormSOP.Instance.SOPManager.GetActionStepInfo(nActionStepID);
                                BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
                                TreeNode node = tree.FindActionStepNode(nActionStepID);
                                tree.PrevSelectedDisasterID = info.DisasterID;
                                //tree.SelectSop(node);

                                bool isRealMode = true;
                                int nCurrentActionStepID = ReadCurrentActionStep(ref isRealMode);

                                // if (HasControl == true)
                                {
                                    if (nCurrentActionStepID >= 0)
                                        SOPScenarioManager.Instance.SelectedScenario(nCurrentActionStepID, isRealMode);
                                    else
                                    {
                                        // 마지막 시나리오 선택
                                        SOPScenarioManager.Instance.SetSelectedScenario();
                                    }
                                }

                                SectionTabControl contorl = (SectionTabControl)GetPageHome().TabControls;
                                contorl.ResizeTabContorl();
                            }
                        }
                    }
                }
			}

           
		}

		public bool LoadSOP()
		{
			BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
			return tree.Load(m_sopMgr, true/*radioRegistMode.Checked*/, radioNormal.Checked);
		}

		private void StopWriteDB()
		{
			try
			{
				if (DBWrite != null && DBWrite.IsAlive)
				{
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
            m_pageHome.BeginHistory();

			//return SOPScenarioManager.Instance.LoadHistory(m_dbMgr, m_sopMgr);
            bool result = SOPScenarioManager.Instance.LoadHistory(m_dbMgr, m_sopMgr);

            if (result && m_pageHome.TabControls.TabCount > 0)
            {
                // 처음 로딩시 TabPage가 열렸다는 것은 해당 TabPage의 SOP가 이미 실행중이라는 의미이므로
                // 시작 버튼을 비활성화 시킨다.
                FormSOP.Instance.SelectViewTab(true);
                EnabledRunGroup();
            }

            m_pageHome.EndHistory();

            return result;
		}

        // 가장 마지막에 ReadCurrentActionStep()을 호출했던 시간
        private DateTime m_dtPrevReadCurrentActionStep = new DateTime();
        private int m_nPrevReadActionStepID = -1;
        private bool m_bPrevReadActionMode = false;

		public int ReadCurrentActionStep(ref bool isRealMode)
		{
            DateTime dtNow = DateTime.Now;
            TimeSpan span = dtNow - m_dtPrevReadCurrentActionStep;

            // 마지막에 호출한 이후 1초가 지나지 않았으면 지난번 읽은 값을 리턴한다.
            if (span.TotalSeconds < 1.0)
            {
                isRealMode = m_bPrevReadActionMode;
                return m_nPrevReadActionStepID;
            }

            m_dtPrevReadCurrentActionStep = dtNow;

            //string strSQL = "select ActionStepID, RealMode from CurrentActionStep where id = 1";
            //string szText = "SELECT ActionStepID, RealMode FROM CurrentActionStep WHERE id = (SELECT min(id) FROM CurrentActionStep WHERE SiteID = {0})";

            string szText = "SELECT cas.ActionStepID, cas.RealMode FROM CurrentActionStep as cas " +
                            " INNER JOIN (SELECT min(id) as minID FROM CurrentActionStep ) cas2 ON cas.id = cas2.minID AND cas.SiteID = {0}";

            /*DateTime now = DateTime.Now;
            string strTime = string.Format("{0:00}:{1:00}", now.Minute, now.Second);
            System.Diagnostics.Trace.WriteLine(strTime + ", " + "ReadCurrentActionStep");*/

            string strSQL = string.Format(szText, m_nSiteID);
			
			ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count <= 1)
            {
                m_nPrevReadActionStepID = -1;
                return -1;
            }

			int nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			isRealMode = WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;
			SOPManager.SetCurrentActionStep(nActionStepID, isRealMode);
            m_bPrevReadActionMode = isRealMode;
            m_nPrevReadActionStepID = nActionStepID;
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
	 
		// 제어권을 요청한 사용자를 얻음
		/*public ArrayList GetRequestControl()
		{
            //string strSQL = "select ControlCheck.id, ControlCheck.userid, ControlCheck.time, ControlCheck.controlcheck, CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel " +
            //                "from ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
            //                "where ControlCheck.controlcheck = 1 AND ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID AND SOPGenUser.UserLevel = SOPGenLevel.ID " +
            //                "order by SOPGenUser.UserLevel desc";

            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT cc.id, cc.userid, cc.time, cc.controlcheck, cm.MemberName, cm.MemberID, sgu.UserLevel FROM ControlCheck as cc ");
            sb.AppendFormat(" INNER JOIN SOPGenUser as sgu ON cc.controlcheck = 1 AND cc.UserID = sgu.ID and sgu.SiteID = {0} ", m_nSiteID);
            sb.Append(" INNER JOIN SOPGenLevel as sgl ON sgu.UserLevel = sgl.ID ");
            sb.Append(" INNER JOIN CompanyMember as cm on cm.ID = sgu.MemberID ");
            sb.Append(" ORDER BY sgu.UserLevel desc");

            string strSQL = sb.ToString();

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
		}*/

		public void ShowRequestControl(string strUserID, string strUserName, string strUserNickName, string strIP)
		{
			if (m_frmRequestControl == null)
			{
				m_frmRequestControl = new PopupRequestControl();
				m_frmRequestControl.Show();
			}

			m_frmRequestControl.AddUser(strUserID, strUserName, strUserNickName, strIP);
		}

        public void HideRequestControl(string strUserID)
        {
            if (m_frmRequestControl != null && m_frmRequestControl.IsDisposed == false)
            {
                int nUserCount = m_frmRequestControl.RemoveUser(strUserID);

                if (nUserCount == 0)
                {
                    m_frmRequestControl.Close();
                    m_frmRequestControl = null;
                }
            }
            else
                m_frmRequestControl = null;
        }

		public void ClearRequestControl()
		{
			m_frmRequestControl = null;
		}

		private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (panelRealTimeInfo != null)
				panelRealTimeInfo.StopTimer();
			
			/*if (m_frmMain2 != null && m_frmMain2.Visible == true)
			{
				m_frmMain2.Visible = false;
				m_frmMain2.Invoke((MethodInvoker)delegate
				{
					m_frmMain2.Close();
					m_frmMain2.Dispose();
				});
			}*/

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
			WorkFlowManager manager = WorkFlowManager.Instance;

			SectionTabPage tabPage = (SectionTabPage)page;
			bool bTabPage = !tabPage.VirtualMode;
			tabPage.UseWaterMark = GetPageOption().GetVirtualMode();
			tabPage.VirtualMode = !FormSOP.Instance.IsReal;
			WorkFlow work = manager.Add(tabPage.ActionStepID, arSections, !tabPage.VirtualMode);
            
            work.WorkFlowEvent -= this.OnWorkflowChanged;
			work.WorkFlowEvent += this.OnWorkflowChanged;
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

        private bool ReadOptionSimulator(string szPropertyName)
        {
            return false;
        }

		private void RunWorkflowAsync(object sender, ProcessSectionEventArgs ex)
		{
            try
            {
			    if (sender == null)
			    {
				    return;
			    }			
                WorkFlowStartNotifyProcess start = (WorkFlowStartNotifyProcess)sender;
                if( start.NoPopup == false)
                {
                    if (start == null || start.DialogResult != DialogResult.OK)
                    {
                        bStartWorkflowEvent = false;
                        EnabledRunGroup();
                        return;
                    }

                    if (start == null || start.DialogResult == DialogResult.Cancel)
                    {
                        bStartWorkflowEvent = false;
                        EnabledRunGroup();
                        return;
                    }
                }           

			    //bool bSendSMS = start.UseSMS;
			    if (GetPageHome().TabControls.SelectedTab == null)
			    {
				    bStartWorkflowEvent = false;
				    return;
			    }

                //VariousData<DateTime> dtDetect = new VariousData<DateTime>(start.DetectTime);
                //string strPosition = start.HasPosition ? start.PositionName : null;
                //string strBroadcastPositionName = start.HasPosition ? start.Popup.LastPosition.BroadcastName : null;
                //string strPSMMaterialName = start.UsePSM ? start.PSMMaterialName : null;
                //VariousData<int> psmDistance = start.UsePSM ? new VariousData<int>(start.PSMDistance) : null;
                //string strAmountSnowfall = start.UseAmountSnowfall ? start.AmountSnowfall : null;

                if (start.Option != null && start.Popup.LastPosition != null)
                {
                    start.Option.BroadcastPositionName = start.Popup.LastPosition.BroadcastName;
                    start.Option.LastPosition = start.Popup.LastPosition;
                }

                WorkFlow work = RunWorkflow(start.Option);
                
                //WorkFlow work = RunWorkflow(bSendSMS, dtDetect, strPosition, strBroadcastPositionName, strPSMMaterialName, psmDistance, strAmountSnowfall);

			    if (work != null)
			    {
                    work.Option = start.Option;
                    work.BeginEndEventSendSMS = start.Option.UseSmsMessage;
				    //work.BeginEndEventSendSMS = bSendSMS;
				    work.SOPName = start.SOPName;
                    //work.DetectTime = start.Option.DetectTime != null ? start.Option.DetectTime.Data : DateTime.Now;
                    //work.Shelters = start.Option.UsingShelters;
				    //work.DetectTime = start.DetectTime;
                    //work.Shelters = start.Shelters;

                    if (start.Option.HasPosition)
				    //if (start.HasPosition)
				    {
                        //work.Position = start.Option.PositionName;
					    //work.Position = start.PositionName;
					    //work.HasPosition = true;
					    work.Option.LastPosition = start.Popup.LastPosition;
					    SetCurrentWorkflow(work);
				    }
				    else
				    {
					    work.Option.PositionName = "";
					    //work.HasPosition = false;
					    work.Option.LastPosition = null;
				    }

                    UnE.Spatial.Shelter.ShelterTypes shelterType = work.Option.ShelterType;

                    List<UnE.Spatial.Shelter> arShlters = start.Option.UsingShelters;
                    //List<Shelter> arShlters = start.Shelters;
                    if (arShlters!= null)
                    {
                        if (arShlters.Count == 2)
                        {
                            IDisasterContainer dist = ProxySOP.Instance.SOPDisasterContainer;
                            if (dist != null)
                            {
                                dist.ShowShelter(3, (int)shelterType);
                            }
                        }
                        else
                        {
                            foreach (UnE.Spatial.Shelter s in arShlters)
                            {
                                IDisasterContainer dist = ProxySOP.Instance.SOPDisasterContainer;
                                if (dist != null)
                                {
                                    dist.ShowShelter(s.ID, (int)shelterType);
                                }
                            }
                        }
                    }
			    }

			    bStartWorkflowEvent = false;
                m_pageHome.toolstripSetting("");

                PanelSectionEx panel = m_pageHome.GetCurrentPanel();
                if (panel != null)
                {
                    panel.HideBeginSectionButton();
                    //panel.HideAllSectionButtons();
                }
            }
            catch (Exception exx)
            {
                System.Diagnostics.Trace.WriteLine(exx.Message);
                System.Diagnostics.Trace.WriteLine(exx.StackTrace);              
            }
		}

		public WorkFlow GetCurrentWorkflow()
		{
			return m_currentWork;
		}

		public void SetCurrentWorkflow(WorkFlow work)
		{
			IDisasterContainer disasterForm = ProxySOP.Instance.SOPDisasterContainer;
			if (disasterForm != null && m_currentWork != null)
			{
				if (m_currentWork.Option.HasPosition == true)
				{
					if (m_currentWork.Option.LastPosition != null)
					{
						disasterForm.LastPos = m_currentWork.Option.LastPosition;
						disasterForm.RemoveDisasterPos();
					}
				}
                disasterForm.HideAllShelter();
			}

			m_currentWork = work;
			if (m_currentWork != null && m_currentWork.Option.HasPosition == true)
			{
				if (m_currentWork.Option.LastPosition != null)
				{
					HistoryDisasterPosition pos = m_currentWork.Option.LastPosition;
					if (disasterForm != null)
					{
						disasterForm.LastPos = pos;
						disasterForm.AddDisasterPos(pos.DisasterName, pos.X, pos.Y, pos.Z);
					}                   
				}

                UnE.Spatial.Shelter.ShelterTypes shelterType = m_currentWork.Option.ShelterType;
                List<UnE.Spatial.Shelter> arShlters = m_currentWork.Option.UsingShelters;

                if (arShlters!= null)
                {
                    if (arShlters.Count == 2)
                    {
                        IDisasterContainer dist = ProxySOP.Instance.SOPDisasterContainer;
                        if (dist != null)
                        {
                            dist.ShowShelter(3, (int)shelterType);
                        }
                    }
                    else
                    {
                        foreach (UnE.Spatial.Shelter s in arShlters)
                        {
                            IDisasterContainer dist = ProxySOP.Instance.SOPDisasterContainer;
                            if (dist != null)
                            {
                                dist.ShowShelter(s.ID, (int)shelterType);
                            }
                        }
                    }
                }
                
			}
		}

		public void toolstripSetting(string str)
		{
			GetPageHome().toolstripSetting(str);
		}

        public WorkFlow RunWorkflow(WorkflowOption option = null)
        // psmDistance : 미터
		//public WorkFlow RunWorkflow(bool bSendSMS = false, VariousData<DateTime> dtDetect = null, string strPosition = null, string strBroadcastPositionName = null, string strPSMMaterialName = null, VariousData<int> psmDistance = null, string strAmountSnowfall = null)
		{
			PageBackstageSOP pageHome = GetPageHome();
			SectionTabPage page = (SectionTabPage)pageHome.tabControl.SelectedTab;
			if (page == null)
				return null;

            bool bReal = FormSOP.Instance.IsReal;



            WorkFlowManager manager = WorkFlowManager.Instance;
            WorkFlow work = (WorkFlow)manager.Get(page.ActionStepID, !page.VirtualMode);
            if( work != null)
            {
                if (work.State == WorkFlowState.RUN)
                    return work;
            }


			

            page.SpecialWorker = FormSOP.Instance.GetPageHome();
			page.State = TabPageState.USE;
			page.CreateNew = false;
			page.VirtualMode = !FormSOP.Instance.IsReal;
			TabPageManager.Instance.AddPage(page, !page.VirtualMode);
			int ActionID = page.ActionStepID;
			if (!manager.Exist(ActionID, !page.VirtualMode))
			{
				AddWorkflow(page);
			}
            else
            {
                // 이미 종료된 SOP 이므로 시나리오 리스트에서 삭제한다.
                SOPScenarioManager.Instance.RemoveScenario(ActionID, !page.VirtualMode);
            }

			if (HasControl == true)
				WriteCurrentActionStepID(ActionID, !page.VirtualMode);

			page.ActionStepID = ActionID;
			TabPageManager.Instance.SetUsePage(ActionID, true, !page.VirtualMode);

			BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
			TreeNode node = tree.FindActionStepNode(ActionID);
			string szPath = node.FullPath;
			/*bool bHasPos = true;
			if (szPath.IndexOf("자연재해") != -1 || szPath.IndexOf("태풍") != -1)
			{
				bHasPos = false;
			}*/
			string sopName = szPath.Substring(szPath.IndexOf("\\") + 1);

            string strCategoryName, strSubCategoryName;
            GetDisasterInfo(szPath, out strCategoryName, out strSubCategoryName);

            if (option == null)
                option = WorkFlowStartNotifyProcess.MakeWorkflowOption(strCategoryName, strSubCategoryName);
            
			work = (WorkFlow)manager.Get(ActionID, !page.VirtualMode);
            work.Option = option;
			//work.HasPosition = bHasPos;
			work.SOPName = sopName;

            int nSensorZoneHistoryID = work.Option.SensorZoneHistoryID;
            //work.AmountSnowfall = strAmountSnowfall;

            // 제어권이 있고 시작일 경우만 sms설정을 수행
            if (option.DetectTime != null && HasControl == true)
            {
                work.BeginEndEventSendSMS = option.UseSmsMessage;
                //work.DetectTime = dtDetect.Data;
            }

            if (work != null)
            {
                if (work.Start())
                {
                    pageHome.StartComponentContents(page.ActionStepID, !page.VirtualMode, option, false);
                    //pageHome.StartComponentContents(page.ActionStepID, !page.VirtualMode, dtDetect, strPosition, strBroadcastPositionName, false, strPSMMaterialName, psmDistance, strAmountSnowfall);

                    List<PanelSection> panels = page.GetPanelSections();
                    foreach(PanelSectionEx pane in panels)
                    {
                        pane.HideBeginSectionButton();
                        //pane.HideAllSectionButtons();

                        string szName = UnE.SOP.ProxySOP.Instance.SiteName;
                        if( option.HasPosition == true)
                        {
                            szName = option.PositionName;
                            //szName = strPosition;                            
                        }

                        PSMMaterial material = null;

                        if (option is WorkflowOptionPSM)
                        {
                            WorkflowOptionPSM optionPSM = (WorkflowOptionPSM)option;
                            material = optionPSM.PSMMaterial;
                        }

                        if (material != null)
                            pane.SetInfoText(szName, option.DetectTime.Data.ToString(), material.MaterialName);
                        else
                            pane.SetInfoText(szName, option.DetectTime.Data.ToString());

                        /*if (strPSMMaterialName != null)
                            pane.SetInfoText(szName, dtDetect.Data.ToString(), strPSMMaterialName);
                        else
                            pane.SetInfoText(szName, dtDetect.Data.ToString());*/
                    }
                }
            }


        
			m_frmStatus.StatusBoard(work.State);
			SetCurrentWorkflow(work);


            FormSOP.Instance.SelectViewTab(true);
			EnabledRunGroup();
            
            m_pageHome.toolstripSetting("");

            //Thread.Sleep(200);

            int nActionStepHistoryID = FormSOP.Instance.SOPManager.GetActionStepHistoryID(ActionID, !page.VirtualMode);
            page.ActionStepHistoryID = nActionStepHistoryID;
            SOPScenarioManager.Instance.AddSOPScenario(szPath.Replace("\\", szDeli.ToString()), page.ActionStepID, !page.VirtualMode, page.ActionStepHistoryID, nSensorZoneHistoryID);

            // HistoryManager의 Thread 동기화 문제가 있기 때문에
            // 아래 코드는 ITabPageSpecialWorker에 맡긴다.
            /*// 시작할 때 사용중인 UserDefinedTeam정보를 저장한다.
            //m_pageHome.SaveUsingUserDefinedTeam(page);*/

            // 버튼 활성/비활성
            CommandBarControlEnabled(false);

			return work;
		}

        private void GetDisasterInfo(string strFullPath, out string strCategoryName, out string strSubCategoryName)
        {
            if (strFullPath != null)
            {
                int nIndex1 = strFullPath.IndexOf('\\');
                int nIndex2 = strFullPath.IndexOf('\\', nIndex1 + 1);

                if (nIndex1 >= 0 && nIndex2 > nIndex1)
                {
                    strCategoryName = strFullPath.Substring(0, nIndex1).Trim();
                    strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();
                    return;
                }
            }

            strCategoryName = "";
            strSubCategoryName = "";
        }
		
		private string[] SplitPath(string szFullPath)
		{
			char cDeli = (char)0x06;
			int idx = szFullPath.IndexOf((char)0x06);
			if (idx == -1)
			{
				cDeli = '/';
			}
			string[] szTemps = szFullPath.Split(cDeli);
			return szTemps;
		}

        public void RunWorkflowWithEvent()
		{
			SectionTabPage tabPage = (SectionTabPage)m_pageHome.TabControls.SelectedTab;
			if (tabPage == null)
			{
				return;
			}
			if (bStartWorkflowEvent == true)
			{
				return;
			}

			bStartWorkflowEvent = true;

            if (!CheckActionStepVersion(ref tabPage))
            {
                bStartWorkflowEvent = false;
                return;
            }

            int nActionStepID = tabPage.ActionStepID;
            bool isVirtual = tabPage.VirtualMode = !this.IsReal;

			string szFullPath = GetActionStepPath(nActionStepID);
            szFullPath = szFullPath.Replace((char)0x06, '\\');

            string[] arrPath = szFullPath.Split('\\');
            string strCategoryName = arrPath[0].Trim();
            string strSubCategoryName = arrPath.Count() > 1 ? arrPath[1].Trim() : "";

            bool checkShelterUse = false;

            if (strCategoryName == "화재" || strCategoryName == "유출사고" || strCategoryName == "태풍")
                checkShelterUse = true;

            bool usePSM = strCategoryName == "유출사고";

			bool bHasPos = true;
            if (strCategoryName == "자연재해" || strCategoryName == "태풍")
			//if (szFullPath.IndexOf("자연재해") != -1 || szFullPath.IndexOf("태풍") != -1)
			{
				bHasPos = false;
			}
			string[] path = SplitPath(szFullPath);
			string sopName = szFullPath.Substring(szFullPath.IndexOf("\\") + 1);
			string disasterName = szFullPath.Substring(0, szFullPath.IndexOf("\\"));
			ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();

			if (m_smsExternalCompanyMemberOn)
			{
				// 협력업체 직원들의 전화번호 추가
				AddExternalCompanyMemberPhoneNumbers(arrListCall);
			}

			Process.WorkFlowStartNotifyProcess start = new Process.WorkFlowStartNotifyProcess(strCategoryName, sopName, tabPage);
			start.VirtualMode = isVirtual;
			start.ActionStepID = nActionStepID;
            start.Option.HasPosition = bHasPos;
            start.Option.SensorZoneHistoryID = tabPage.SensorZoneHistoryID;
            start.Option.SensorZoneID = tabPage.SensorID;

			//start.HasPosition = bHasPos;
            //start.UsePSM = usePSM;
			//start.SOPName = sopName;
            //start.CategoryName = strCategoryName;
			start.CallList = arrListCall;
            

			start.OnPostProcess += new PostProcessEvent(RunWorkflowAsync);

			Popup.PopupStartEvent form = start.Popup;
			form.DisasterName = disasterName;

			ArrayList arPosList = DataManager.Instance.LoadHistoryDisasterPosition();
            List<UnE.Spatial.Shelter> shelters = DataManager.Instance.LoadShelter();

			form.SetRecentPosition(arPosList);
            form.SetShelters(shelters, checkShelterUse);

            StubWorker.Instance.WorkFlowStartOption = form;

            if (form.LastPosition == null)
                ProxyMessenger.Instance.SetCheckPosition(form.DisasterName, form.PositionName, "", "", 0.0f, -1, -1, -1, "", 0.0f, 0.0f, 0.0f, -1, true);
            else
                ProxyMessenger.Instance.SetCheckPosition(form.DisasterName, form.PositionName, form.LastPosition.BroadcastName, form.LastPosition.BuildingID, form.LastPosition.FloorIndex, form.LastPosition.HistoryActionStepID, form.LastPosition.IconID, form.LastPosition.PSMDistance, form.LastPosition.PSMMaterial, form.LastPosition.X, form.LastPosition.Y, form.LastPosition.Z, form.LastPosition.ZoneID, true);
			/*if (m_frmMain2.PageHome.ContentForm != null)
			{
                UnE.SOP.IDisasterContainer disContainer = (UnE.SOP.IDisasterContainer)m_frmMain2.PageHome.ContentForm;
                disContainer.SetCheckPoistion(form, true);
			}*/
			ProcessSectionManager.Instance.AddFirst(start);
		}

        private bool CheckActionStepVersion(ref SectionTabPage tabPage)
        {
            int nActionStepID = tabPage.ActionStepID;

            // 현재 열려있는 버전이 삭제되었거나 업데이트 되지 않았는지 확인한다.
            VersionInfo version = FormSOP.Instance.SOPManager.GetActionStepVersionInfo(nActionStepID);

            if (version == null)
                return ShowAlreadyRemovedVersion(tabPage);

            string strSQL = "Select Disaster.ID, DisasterName, SubDisasterID, Version.LastAccessTime from Disaster, Version where VersionID = Version.ID and VersionID = " + version.VersionID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return ShowDisconnectedDB();

            if (arrResult.Count != 4)
                return ShowAlreadyRemovedVersion(tabPage);
            
            // 현재 열려있는 버전보다 더 새로운 버전이 나오지 않았는지 확인한다.
            DBUtility.VariousData<int> nDisasterID = WebDBManager.GetIntField(arrResult[0].ToString());
            string strDisasterName = WebDBManager.GetStringField(arrResult[1]);
            DBUtility.VariousData<int> nSubDisasterID = WebDBManager.GetIntField(arrResult[2].ToString());
            DBUtility.VariousData<DateTime> dtLastAccessed = WebDBManager.GetDateTimeField(arrResult[3]);

            if (nDisasterID == null || strDisasterName == null || nSubDisasterID == null || dtLastAccessed == null)
                return ShowDisconnectedDB();

            int nNewDisasterID, nNewVersionID;

            if (version.LastAccessedTime != dtLastAccessed.Data)
            {
                int nNewVersionCheck = CheckNewVersion(strDisasterName, nSubDisasterID.Data, version, out nNewDisasterID, out nNewVersionID);

                if (nNewVersionCheck == 1)
                    return ShowExistNewVersion(ref tabPage, strDisasterName, nDisasterID.Data, version.VersionID);
                else if (nNewVersionCheck == 0)
                    return ShowExistNewVersion(ref tabPage, strDisasterName, nNewDisasterID, nNewVersionID);
                else// if (nNewVersionCheck < 0)
                    return ShowDisconnectedDB();
            }

            int nResult = CheckNewVersion(strDisasterName, nSubDisasterID.Data, version, out nNewDisasterID, out nNewVersionID);

            if (nResult == 1)
                return true;
            else if (nResult == 0)
                return ShowExistNewVersion(ref tabPage, strDisasterName, nNewDisasterID, nNewVersionID);

            return ShowDisconnectedDB();
        }

        // Return 값 : 1(새로운 버전이 존재하지 않는다.)
        //             0(새로운 버전이 존재한다.)
        //            -1(DB 오류)
        private int CheckNewVersion(string strDisasterName, int nSubDisasterID, VersionInfo version, out int nNewDisasterID, out int nNewVersionID)
        {
            nNewDisasterID = nNewVersionID = -1;

            string strSQL = string.Format("select Disaster.ID, VersionID from Disaster, Version where VersionID = Version.ID and Version.SiteID = {0} and Version.IsRegular = {1} and Version.IsNormal = {2} and DisasterName = '{3}' and SubDisasterID = {4} and VersionID > {5}",
                m_nSiteID, version.IsRegular ? 1 : 0, version.IsNormal ? 1 : 0, strDisasterName, nSubDisasterID, version.VersionID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            if (arrResult.Count != 2)
                return 1;

            DBUtility.VariousData<int> nNewDisaster = WebDBManager.GetIntField(arrResult[0].ToString());
            DBUtility.VariousData<int> nNewVersion = WebDBManager.GetIntField(arrResult[1].ToString());

            if (nNewDisaster == null && nNewVersion == null)
                return 1;

            nNewDisasterID = nNewDisaster.Data;
            nNewVersionID = nNewVersion.Data;
            return 0;
        }

        public DisasterInfo ReloadDisaster(int nActionStepID)
        {
            string strSQL = "Select d.ID, d.VersionID, d.DisasterName from ActionStep as _as, Disaster as d, Version as v where _as.DisasterID = d.ID and d.VersionID = v.ID and _as.ID = " + nActionStepID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count != 3)
                return null;

            DBUtility.VariousData<int> disasterID = WebDBManager.GetIntField(arrResult[0].ToString());
            DBUtility.VariousData<int> versionID = WebDBManager.GetIntField(arrResult[1].ToString());
            string strDisasterName = WebDBManager.GetStringField(arrResult[0]);

            if (disasterID == null || versionID == null || strDisasterName == null)
                return null;

            DisasterInfo disaster;
            VersionInfo version;
            string strSubCategoryName, strCategoryName;

            if (!LoadDisasterInfo(disasterID.Data, versionID.Data, out disaster, out version, out strSubCategoryName, out strCategoryName))
                return null;

            if (!SOPScenarioManager.Instance.GetBarLevelTree().LoadSOP(strCategoryName, strSubCategoryName, strDisasterName, disasterID.Data, disaster, version))
                return null;

            foreach (ActionStepInfo actionStep in disaster.ActionSteps)
            {
                m_sopMgr.SetActionStepInfo(actionStep);
                m_sopMgr.SetActionStepVersionInfo(actionStep, version);
            }

            return disaster;
        }

        private bool ShowExistNewVersion(ref SectionTabPage tabPage, string strCurrentDisasterName, int nNewDisasterID, int nNewVersionID)
        {
            if (MessageBox.Show(this, "현재 열려있는 SOP보다 더 새로운 버전의 SOP가 존재합니다.\r\nSOP를 다시 로딩할까요?", "버전확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                DateTime dtLinkTime = tabPage.LinkedTime;
                int nLinkZoneID = tabPage.LinkedZoneID;
                string szLinkZoneName = tabPage.LinkedZoneName;

                PanelSectionEx.CloseTabPage(tabPage, tabPage.ActionStepID);

                DisasterInfo disaster;
                VersionInfo version;
                string strSubCategoryName, strCategoryName;

                if (!LoadDisasterInfo(nNewDisasterID, nNewVersionID, out disaster, out version, out strSubCategoryName, out strCategoryName))
                    return ShowFailOpenNewVersion();

                if (!SOPScenarioManager.Instance.GetBarLevelTree().LoadSOP(strCategoryName, strSubCategoryName, strCurrentDisasterName, nNewDisasterID, disaster, version))
                    return ShowFailOpenNewVersion();

                if (m_pageHome.TabControls.SelectedTab == null)
                    return ShowFailOpenNewVersion();

                EnabledRunGroup();

                m_sopMgr.RemoveActionStepVersionInfo(tabPage.ActionStepID);
                m_sopMgr.RemoveActionStepInfo(tabPage.ActionStepID);

                foreach (ActionStepInfo actionStep in disaster.ActionSteps)
                {
                    m_sopMgr.SetActionStepInfo(actionStep);
                    m_sopMgr.SetActionStepVersionInfo(actionStep, version);
                }

                tabPage = (SectionTabPage)m_pageHome.TabControls.SelectedTab;
                tabPage.LinkedZoneName = szLinkZoneName;
                tabPage.LinkedTime = dtLinkTime;
                tabPage.LinkedZoneID = nLinkZoneID;
                return true;
            }

            PanelSectionEx.CloseTabPage(tabPage, tabPage.ActionStepID);
            MessageBox.Show(this, "[" + strCurrentDisasterName + "] SOP를 닫았습니다.");
            return false;
        }

        private bool ShowFailOpenNewVersion()
        {
            MessageBox.Show(this, "새로운 버전의 SOP를 불러오는데 실패하였습니다.", "오류");
            return false;
        }

        private bool LoadDisasterInfo(int nDisasterID, int nVersionID, out DisasterInfo disaster, out VersionInfo version, out string strSubCategoryName, out string strCategoryName)
        {
            disaster = null;
            version = null;
            strSubCategoryName = strCategoryName = "";

            string strFormat = "select DisasterName, SubDisasterID, isRegular, isNormal, CreateTime, LastAccessTime, VersionName, OwnerID, SOPGenUser.MemberID, SOPGenUser.UserID, sdc.SubCategoryName, dc.CategoryName ";
            strFormat += "from Disaster, Version, SOPGenUser, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strFormat += "where Disaster.VersionID = Version.ID and OwnerID = SOPGenUser.ID and SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and Disaster.ID = {0} and VersionID = {1}";

            string strSQL = string.Format(strFormat, nDisasterID, nVersionID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count != 12)
                return false;

            string strDisasterName = WebDBManager.GetStringField(arrResult[0]);
            DBUtility.VariousData<int> nSubDisasterID = WebDBManager.GetIntField(arrResult[1].ToString());
            DBUtility.VariousData<int> nRegular = WebDBManager.GetIntField(arrResult[2].ToString());
            DBUtility.VariousData<int> nNormal = WebDBManager.GetIntField(arrResult[3].ToString());
            DBUtility.VariousData<DateTime> createTime = WebDBManager.GetDateTimeField(arrResult[4]);
            DBUtility.VariousData<DateTime> lastAccessedTime = WebDBManager.GetDateTimeField(arrResult[5]);
            string strVersionName = WebDBManager.GetStringField(arrResult[6]);
            DBUtility.VariousData<int> nOwnerID = WebDBManager.GetIntField(arrResult[7].ToString());
            DBUtility.VariousData<int> nMemberID = WebDBManager.GetIntField(arrResult[8].ToString());
            string strUserID = WebDBManager.GetStringField(arrResult[9]);
            strSubCategoryName = WebDBManager.GetStringField(arrResult[10]);
            strCategoryName = WebDBManager.GetStringField(arrResult[11]);

            if (strDisasterName == null || nSubDisasterID == null || nRegular == null || nNormal == null ||
                createTime == null || lastAccessedTime == null || strVersionName == null || nOwnerID == null ||
                strUserID == null || strSubCategoryName == null || strCategoryName == null)
                return false;

            string strUserName = strUserID;

            if (nMemberID != null)
            {
                strSQL = "Select MemberName from CompanyMember where ID = " + nMemberID.Data.ToString();
                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count != 1)
                    return false;

                strUserName = WebDBManager.GetStringField(arrResult[0]);

                if (strUserName == null)
                    return false;
            }

            disaster = new DisasterInfo();

            disaster.DisasterID = nDisasterID;
            disaster.VersionID = nVersionID;

            Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();
            dicDisaster[nDisasterID] = disaster;
            string strDisasterIDs = nDisasterID.ToString();

            if (!m_sopMgr.LoadDisasterActionSteps(dicDisaster, strDisasterIDs))
                return false;

            version = new VersionInfo();

            version.BeginTime = createTime.Data;
            version.IsNormal = nNormal.Data == 1;
            version.IsRegular = nRegular.Data == 1;
            version.LastAccessedTime = lastAccessedTime.Data;
            version.UserName = strUserName;
            version.VersionID = nVersionID;
            version.VersionName = strVersionName;

            return true;
        }

        private bool ShowAlreadyRemovedVersion(SectionTabPage tabPage)
        {
            MessageBox.Show(this, "이미 삭제된 SOP입니다.\r\nSOP 화면을 닫습니다.", "오류", MessageBoxButtons.OK);
            
            PanelSectionEx.CloseTabPage(tabPage, tabPage.ActionStepID);
            m_sopMgr.RemoveActionStepVersionInfo(tabPage.ActionStepID);
            m_sopMgr.RemoveActionStepInfo(tabPage.ActionStepID);

            return false;
        }

        private bool ShowDisconnectedDB()
        {
            MessageBox.Show(this, "DB와 연결이 끊어졌습니다.", "오류", MessageBoxButtons.OK);
            return false;
        }

		public void AddExternalCompanyMemberPhoneNumbers(ArrayList arrListCall)
		{
			foreach (ExternalCompanyMember member in FormSOP.Instance.SOPManager.ExternalCompanyMembers)
			{
				arrListCall.Add(member.PhoneNumber);
			}
		}        

        public void SetWorkflowState(UnE.SOP.Workstate.WorkFlowState state)
        {
            if (m_frmStatus!= null)
                m_frmStatus.StatusBoard(state);
        }

        private bool IsValidWorkFlow(WorkFlow work, bool isVirtualMode)
        {
            if (work == null)
                return false;

            int nKey = work.ActionStepID;

            if (isVirtualMode)
                nKey = -work.ActionStepID;

            DateTime dtNow = DateTime.Now;
            DateTime lastTime;

            if (m_dicWorkFlowDone.TryGetValue(nKey, out lastTime))
            {
                TimeSpan span = dtNow - lastTime;

                if (span.TotalSeconds > m_dMinimumWorkFlowTime)
                {
                    m_dicWorkFlowDone[nKey] = dtNow;
                    return true;
                }
                else
                    return false;
            }
            else
                m_dicWorkFlowDone[nKey] = dtNow;

            return true;
        }


        private int m_nLastDoneWorkflowID = -1;
        private bool m_nLastDoneWorkflowMode = false;
		public void DoneWorkflow()
		{
			IDisasterContainer diasterForm = ProxySOP.Instance.SOPDisasterContainer;

			WorkFlowManager manager = WorkFlowManager.Instance;
			PageBackstageSOP pageHome = GetPageHome();
			SectionTabPage page = (SectionTabPage)pageHome.tabControl.SelectedTab;

			int ActionID = page.ActionStepID;

            if (m_nLastDoneWorkflowID == page.ActionStepHistoryID && m_nLastDoneWorkflowMode == !page.VirtualMode)
            {
                // 이미 한번 처리한 이벤트 이므로 처리하지 않는다.
                return;
            }

            m_nLastDoneWorkflowID = page.ActionStepHistoryID;
            m_nLastDoneWorkflowMode = !page.VirtualMode;

			if (WorkFlowManager.Instance.DeleteComplete == true)
			{
				TabPageManager.Instance.RemovePage(page, !page.VirtualMode);
			}

			ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();
			WorkFlow work = (WorkFlow)manager.Get(ActionID, !page.VirtualMode);
           
			if (this.HasControl == true)
				if (m_smsExternalCompanyMemberOn)
				{
					// 협력업체 직원들의 전화번호 추가
					AddExternalCompanyMemberPhoneNumbers(arrListCall);
				}

            if (!IsValidWorkFlow(work, page.VirtualMode))
            {
                //System.Diagnostics.Trace.WriteLine("CheckDone Return");
                // 이미 처리된 프로세스이다.
                return;
            }

            


            //System.Diagnostics.Trace.WriteLine("EndNotify");
			Process.WorkflowEndNotifyProcess endEvent = new Process.WorkflowEndNotifyProcess();
			endEvent.VirtualMode = (work.RunMode == WorkFlowMode.VIRTUAL);
			endEvent.HasPosition = work.Option.HasPosition;
			endEvent.SOPName = work.SOPName;
			endEvent.CallList = arrListCall;

			if (this.HasControl == true)
				endEvent.UseSMS = work.BeginEndEventSendSMS;
			else
				endEvent.UseSMS = false;

			if (work.Option.HasPosition == true)
			{
				if (work.Option.LastPosition != null)
					endEvent.PositionName = work.Option.LastPosition.PoistionName;
				else
					endEvent.PositionName = "";
			}

			if (work != null && work.Option.HasPosition == true)
			{
				if (diasterForm != null &&work.Option.LastPosition != null)
				{
					diasterForm.LastPos = work.Option.LastPosition;
					diasterForm.RemoveDisasterPos();
					work.Option.LastPosition = null;
					diasterForm.LastPos = null;
				}
			}

			if (this.HasControl == true)
			{
				ProcessSectionManager.Instance.AddFirst(endEvent);
			}
			HistoryManager.Instance.RemoveHistoryDisasterPosition(page.ActionStepID, !page.VirtualMode);
            HistoryManager.Instance.RemoveHistoryDisasterNoPosition(page.ActionStepID, !page.VirtualMode);

			if (HasControl == true)
				WriteCurrentActionStepID(-1, false);

			m_frmStatus.StatusBoard(work.State);
			SetCurrentWorkflow(work);
			EnabledRunGroup();
		}

        public void StopWorkflow(DateTime dtStop, bool noDBWrite = false)
        {
            WorkFlowManager manager = WorkFlowManager.Instance;
            PageBackstageSOP pageHome = GetPageHome();
            if (pageHome.tabControl.IsHandleCreated)
            {
                SectionTabPage page = (SectionTabPage)pageHome.tabControl.SelectedTab;
                if (page != null)
                {
                    StopWorkflow(dtStop, noDBWrite, page);
                    SOPScenarioManager.Instance.RemoveActionStepHistoryByUser(page.ActionStepID, !page.VirtualMode);
                }

                List<PanelSection> panels = page.GetPanelSections();
                foreach (PanelSection pane in panels)
                {
                    pane.ShowAllSectionButtons();
                }
            }

            CommandBarControlEnabled(true);
        }

		public void StopWorkflow(DateTime dtStop, bool noDBWrite, int nActionStepID, bool isRealMode)
		{
			WorkFlowManager manager = WorkFlowManager.Instance;
			PageBackstageSOP pageHome = GetPageHome();

			foreach (SectionTabPage page in pageHome.TabControls.Controls)
			{
				if (page.ActionStepID == nActionStepID && !page.VirtualMode == isRealMode)
				{
					StopWorkflow(dtStop, noDBWrite, page);

                    List<PanelSection> panels = page.GetPanelSections();
                    foreach (PanelSection pane in panels)
                    {
                        pane.ShowAllSectionButtons();
                    }

					break;
				}
			}
            SOPScenarioManager.Instance.RemoveActionStepHistory(nActionStepID, isRealMode);
		}

		private void StopWorkflow(DateTime dtStop, bool noDBWrite, SectionTabPage page)
		{
			int ActionID = page.ActionStepID;
			WorkFlowManager manager = WorkFlowManager.Instance;
			WorkFlow work = (WorkFlow)manager.Get(ActionID, !page.VirtualMode);
			if (work == null)
				return;

            EnableOptions(true);
			manager.Remove(ActionID, !page.VirtualMode);

			TabPageManager.Instance.SetUsePage(ActionID, false, !page.VirtualMode);



			if (WorkFlowManager.Instance.DeleteComplete == true)
			{
				TabPageManager.Instance.RemovePage(page, !page.VirtualMode);
			}

			if (work != null && work.State == WorkFlowState.RUN)
			{
				work.Stop(dtStop, noDBWrite);

				if (work.Option.HasPosition == true)
				{
					if (work.Option.LastPosition != null)
					{
                        ProxyMessenger.Instance.SetLastPosition(work.Option.LastPosition.DisasterName, work.Option.LastPosition.PoistionName, work.Option.LastPosition.BroadcastName, work.Option.LastPosition.BuildingID, work.Option.LastPosition.FloorIndex, work.Option.LastPosition.HistoryActionStepID, work.Option.LastPosition.IconID, work.Option.LastPosition.PSMDistance, work.Option.LastPosition.PSMMaterial, work.Option.LastPosition.X, work.Option.LastPosition.Y, work.Option.LastPosition.Z, work.Option.LastPosition.ZoneID);
                        ProxyMessenger.Instance.RemoveDisasterPos();
                        work.Option.LastPosition = null;
                        ProxyMessenger.Instance.NullLastPosition();
                        /*UnE.SOP.IDisasterContainer disContainer = (UnE.SOP.IDisasterContainer)m_frmMain2.PageHome.ContentForm;

                        disContainer.LastPos = work.Option.LastPosition;
                        disContainer.RemoveDisasterPos();
						work.Option.LastPosition = null;
                        disContainer.LastPos = null;*/
					}
				}
			}

			HistoryManager.Instance.RemoveHistoryDisasterPosition(page.ActionStepID, !page.VirtualMode);
            HistoryManager.Instance.RemoveHistoryDisasterNoPosition(page.ActionStepID, !page.VirtualMode);

			if (HasControl == true)
				WriteCurrentActionStepID(-1, false);

			if (work == null)
				return;



			m_frmStatus.StatusBoard(WorkFlowState.STOP);
			SetCurrentWorkflow(null);
			EnabledRunGroup();

            List<PanelSection> panels = page.GetPanelSections();
            foreach (PanelSection pane in panels)
            {
                pane.ShowAllSectionButtons();
            }

            
		}

		public void WaitWorkflow()
		{
			m_frmStatus.StatusBoard(WorkFlowState.STANDBY);
		}

		public void ChangeWorkflow()
		{
			WorkFlowManager manager = WorkFlowManager.Instance;
			PageBackstageSOP pageHome = FormSOP.Instance.GetPageHome();
			SectionTabPage page = (SectionTabPage)pageHome.tabControl.SelectedTab;
			if (page == null)
				return;

			int ActionID = page.ActionStepID;
			WorkFlow work = (WorkFlow)manager.Get(ActionID, !page.VirtualMode);
			if (work != null)
			{
				m_frmStatus.StatusBoard(work.State);
				SetCurrentWorkflow(work);
			}
			else
			{
				m_frmStatus.StatusBoard(WorkFlowState.STANDBY);
			}
		}

		public void AllStopWorkflow()
		{
			WorkFlowManager manager = WorkFlowManager.Instance;
			PageBackstageSOP pageHome = GetPageHome();
			SectionTabPage page = (SectionTabPage)pageHome.tabControl.SelectedTab;

			ArrayList arRunScenario = SOPScenarioManager.Instance.GetAllScenario();
			foreach (SOPScenario sop in arRunScenario)
			{
				TabPageManager.Instance.SetUsePage(sop.ActionStepID, false, sop.RealMode);
				WorkFlow work = (WorkFlow)manager.Get(sop.ActionStepID, sop.RealMode);
				if (work != null)
				{
					work.Stop(DateTime.Now);

					if (work.Option.HasPosition == true)
					{
						if (work.Option.LastPosition != null)
						{
                            ProxyMessenger.Instance.SetLastPosition(work.Option.LastPosition.DisasterName, work.Option.LastPosition.PoistionName, work.Option.LastPosition.BroadcastName, work.Option.LastPosition.BuildingID, work.Option.LastPosition.FloorIndex, work.Option.LastPosition.HistoryActionStepID, work.Option.LastPosition.IconID, work.Option.LastPosition.PSMDistance, work.Option.LastPosition.PSMMaterial, work.Option.LastPosition.X, work.Option.LastPosition.Y, work.Option.LastPosition.Z, work.Option.LastPosition.ZoneID);
                            ProxyMessenger.Instance.RemoveDisasterPos();
                            work.Option.LastPosition = null;
                            ProxyMessenger.Instance.NullLastPosition();
                            /*UnE.SOP.IDisasterContainer disContainer = (UnE.SOP.IDisasterContainer)m_frmMain2.PageHome.ContentForm;

                            disContainer.LastPos = work.Option.LastPosition;
                            disContainer.RemoveDisasterPos();
							work.Option.LastPosition = null;
                            disContainer.LastPos = null;*/
						}
					}
					m_frmStatus.StatusBoard(work.State);
				}
				else
				{
					m_frmStatus.StatusBoard(WorkFlowState.WAIT);
				}
				SetCurrentWorkflow(work);
				EnabledRunGroup();
			}
			HistoryManager.Instance.HistoryDisasterPosition.Clear();
		}
		
		public WorkFlowState CheckWorkflow(int nActionID, bool isVirtual)
		{
			WorkFlowManager manager = WorkFlowManager.Instance;
			WorkFlow work = (WorkFlow)manager.Get(nActionID, !isVirtual);

			if (work == null)
				return WorkFlowState.STANDBY;

			return work.State;
		}
		#endregion

		public void WriteCurrentActionStepID(int nActionStepID, bool isRealMode)
		{
			if (!HasControl)
				return;

			SOPManager.SetCurrentActionStep(nActionStepID, isRealMode);

            int nCurrentID = -1;
            string strSQL = string.Format("SELECT id FROM CurrentActionStep WHERE id = (SELECT min(id) FROM CurrentActionStep WHERE SiteID = {0})", m_nSiteID);
            ArrayList arResult = DBManager.GetResultData(strSQL, 0);
            if( arResult == null || arResult.Count == 0 )
            {
                strSQL = string.Format("INSERT INTO CurrentActionStep (id, ActionStepID, RealMode, SiteID) VALUES ( 1, {0} , {1}, {2})", nActionStepID, isRealMode ? 1 : 0, m_nSiteID);
                DBManager.GetResultData(strSQL, 0);
                nCurrentID = 1;
            }
            else
            {
                nCurrentID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
            }

            if (nCurrentID > -1)
            {
                strSQL = string.Format("Update CurrentActionStep set ActionStepID = {0}, RealMode = {1} where id = {2}", nActionStepID, isRealMode ? 1 : 0, nCurrentID);
                DBManager.GetResultData(strSQL, 0);
            }			
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
			if (vInfo.IsRegular == true/*radioRegistMode.Checked*/)
			{
				if (vInfo.IsNormal == radioNormal.Checked)
					return;
			}

			/*if (vInfo.IsRegular == true)
			{
				radioRegistMode.Checked = true;
				radioNonRegistMode.Checked = false;
			}
			else
			{
				radioRegistMode.Checked = false;
				radioNonRegistMode.Checked = true;
			}*/

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

			OnChangeMode();

			BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
			TreeNode node = tree.FindActionStepNode(aInfo.ActionStepID);
			if (node != null)
			{
				TreeNode selectedNode = tree.GetSelectedNode();
				if (selectedNode != node)
				{
					tree.SelectSop(node);

					if (PageBackstageSOP.IsWorkingMode(aInfo.ActionStepID, isRealMode))
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

            SectionTabPage page = (SectionTabPage)m_pageHome.TabControls.SelectedTab;
            if (page == null)
                return;
            TreeNode node = SOPScenarioManager.Instance.GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return;
            WorkFlow work = RunWorkflow();
            if (work != null)
            {
                int nSensorZoneHistoryID = work.Option.SensorZoneHistoryID;
                SOPScenarioManager.Instance.AddSOPScenario(node.FullPath.Replace("\\", szDeli.ToString()), page.ActionStepID, !page.VirtualMode, page.ActionStepHistoryID, nSensorZoneHistoryID);
            }
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

        public void EnableOptions(bool enabled)
        {
            return;

            //radioRealMode.Enabled = radioVirtualMode.Enabled = /*radioRegistMode.Enabled =*/ enabled;
            /*radioNonRegistMode.Enabled = */radioNormal.Enabled = radioHoliday.Enabled = enabled;
        }

		public ArrayList GetAllMemberPhoneNumber()
		{
			ArrayList arrListCall = GetPageHome().GetDockPersonnel().GetMemberPhoneNumber();
			return arrListCall;
		}

		public string GetActionStepPath(int actionStepID)
		{
            VersionInfo version = this.SOPManager.GetActionStepVersionInfo(actionStepID);

            if (version == null)
                return "";

			BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();

            if (tree.IsRegular != version.IsRegular || tree.IsNormal != version.IsNormal)
            {
                tree.Load(this.SOPManager, version.IsRegular, version.IsNormal);
            }

			TreeNode node = tree.FindActionStepNode(actionStepID);
            if (node == null)
            {
                string strCategoryName, strSubCategoryName, strDisasterName, strActionStepName;
                int nCategoryID, nSubCategoryID, nDisasterID;

                if (!LoadActionStepFullPath(actionStepID, out nCategoryID, out nSubCategoryID, out nDisasterID, out strCategoryName, out strSubCategoryName, out strDisasterName, out strActionStepName))
                    return "";

                tree.AddTreeNode(nCategoryID, strCategoryName, nSubCategoryID, strSubCategoryName, nDisasterID, strDisasterName, actionStepID, strActionStepName);

                string strPath = strCategoryName + szDeli + strSubCategoryName + szDeli + strDisasterName + szDeli + strActionStepName;
                return strPath;
            }

			string szName = node.FullPath.Replace('\\', szDeli);
			return szName;
		}

        private bool LoadActionStepFullPath(int nActionStepID, out int nCategoryID, out int nSubCategoryID, out int nDisasterID, out string strCategoryName, out string strSubCategoryName, out string strDisasterName, out string strActionStepName)
        {
            strCategoryName = strSubCategoryName = strDisasterName = strActionStepName = "";
            nCategoryID = nSubCategoryID = nDisasterID = -1;

            string strSQL = "select dc.CategoryName, sdc.SubCategoryName, d.DisasterName, _as.StepName, dc.ID, sdc.ID, d.ID ";
	        strSQL += "from ActionStep as _as, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strSQL += "where dc.ID = sdc.DisasterID and sdc.ID = d.SubDisasterID and d.ID = _as.DisasterID and _as.ID = " + nActionStepID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count != 7)
                return false;

            strCategoryName = WebDBManager.GetStringField(arrResult[0]);
            strSubCategoryName = WebDBManager.GetStringField(arrResult[1]);
            strDisasterName = WebDBManager.GetStringField(arrResult[2]);
            strActionStepName = WebDBManager.GetStringField(arrResult[3]);
            DBUtility.VariousData<int> categoryID = WebDBManager.GetIntField(arrResult[4].ToString());
            DBUtility.VariousData<int> subCategoryID = WebDBManager.GetIntField(arrResult[5].ToString());
            DBUtility.VariousData<int> disasterID = WebDBManager.GetIntField(arrResult[6].ToString());

            if (strCategoryName == null || strSubCategoryName == null || strDisasterName == null || strActionStepName == null ||
                categoryID == null || subCategoryID == null || disasterID == null)
                return false;

            nCategoryID = categoryID.Data;
            nSubCategoryID = subCategoryID.Data;
            nDisasterID = disasterID.Data;

            return true;
        }

        private void SDMS_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_closedSDMS = true;
            MainFrame.Close();
        }

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
            /*if (!m_closedSDMS)
            {
                e.Cancel = true;
                btnClose_Click(null, null);
                return;
            }*/

			// 종료 이벤트가 사용자가 만든것이 아니면 종료이벤트만 발생해준다.
			if (sender != this)
			{
				FormSOP.Instance.Invoke((MethodInvoker)delegate
				{
					FormSOP.Instance.Close();
				});
				return;
			}

			HistoryManager.Instance.Dispose();            

			StopWriteDB();

            if (m_dbMgr is SimulationDBManager)
                ((SimulationDBManager)m_dbMgr).CloseLocalDB();

			m_netMgr.ReleaseThread();

			FormSOP.Instance.CloseThread = true;

			ProcessSectionManager.Instance.Dispose();
			TTSManager.Instance.Dispose();

			if (m_pageOption != null)
				m_pageOption.Dispose();
			if (m_pageMessage != null)
				m_pageMessage.Dispose();
			if (m_pageHome != null)
				m_pageHome.Dispose();


            if( m_SopMonitor != null)
            {
                m_SopMonitor.Dispose();
            }

			Thread.Sleep(200);
            //MainFrame.Close();
		}

		public ArrayList GetLevelMember(int nLevelID)
		{
			ArrayList arrSOPMember = new ArrayList();
            string strSQL = "select ID, MemberName from CompanyMember where LevelID in (" + nLevelID.ToString() + ")";
			//string strSQL = "select ID, MemberName, LevelID from CompanyMember where LevelID in (select id from JobLevel where LevelNo = " + nTeamID.ToString() + ")";
			ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return null;

			int nResultCount = arrResult.Count;
			for (int i = 0; i < nResultCount - 1; i += 2)
			{
				int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
				string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
				//int nLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);

                // SiteID가 다른 곳의 CompanyMember인지 검사한다.
                if (DataManager.Instance.GetCompanyMember(nID) == null)
                    continue;
                //if (m_frmMain2.DataManager.GetCompanyMember(nID) == null)
                //    continue;

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
                if (strTeamList == null || strTeamList.Length == 0)
                    strReport = strTask + " " + strStatus;
                else
				    strReport = strTeamList + " " + strTask + " " + strStatus;
			}
			else
			{
				strReport = strStepMemberName + strTask + " " + strStatus;
			}

			//SOPLog log = FormSOP.Instance.LogFile;
			//log.Write(string.Format("In GetRealTimeInfo, strReport : {0}\n", strReport));

            if (strReport.StartsWith("-"))
                strReport = strReport.Substring(1);

			panelRealTimeInfo.RealTimeInfo = strReport;
			panelRealTimeInfo.SetForeColor(type);
            panelRealTimeInfo.UseMovingText = this.UseMovingText;
            //panelRealTimeInfo.UseMovingText = m_frmMain2.UseMovingText;
			panelRealTimeInfo.DrawMovingText();
		}

		private int GetDisasterType()
		{
			
			SOPScenario senario = SOPScenarioManager.Instance.CurrentScenario;
			if (senario == null)
				return 0;

			//string strTitle = FormSOP.Instance.GetPageHome().GetDockPropertiesLevel().GetTitle();
			//string[] strDisaster = strTitle.Split(szDeli);

			string strDisaster = SOPScenarioManager.Instance.CurrentScenario.CategoryName;

			int nType = 0;

			if (strDisaster == "자연재해")
				nType = 0;
			else if (strDisaster == "태풍")
				nType = 0;
			else if (strDisaster == "화재")
				nType = 1;
			else if (strDisaster == "유출사고")
				nType = 2;
			else if (strDisaster == "테러")
				nType = 3;
			else if (strDisaster == "인명구조 및 의료지원")
				nType = 4;
			else
				nType = 5;

			return nType;
		}

		public void ClearProcess()
		{
			if (GetPageHome()!= null)
				GetPageHome().ClearProcess();
		}

		public bool CompleteSection(Sections.Section section, Sections.PanelSection panel)
		{
			panel.CompleteSection(section);
			return true;
		}      
		  
		public ArrayList GetAllSenario()
		{
			return SOPScenarioManager.Instance.GetAllScenario();
		}

		public void OnWorkflowChanged(object sender, WorkFlowEventArgs args)
		{
			if (sender == null || args == null)
				return;
			
			// Workflow종료 Event의 HistoryEvent를 기록
			if (args.State == WorkFlowState.STOP)
			{
                UnE.SOP.Workstate.WorkFlow workflow = (UnE.SOP.Workstate.WorkFlow)sender;
                //if( workflow.State == WorkFlowState.RUN || workflow.State == WorkFlowState.PAUSE)
                {
                    bool bSendSMS = workflow.BeginEndEventSendSMS;
                    HistoryManager.Instance.AddActionStepHistory(args.ActionStepID, args.RealMode, args.State, args.Time, args.NoDBWrite, bSendSMS);
                    m_pageHome.OnCloseWorkFlow(args.ActionStepID, args.RealMode, args.State);
                }

                {
                    int nActionStepHistoryID = FormSOP.Instance.SOPManager.GetActionStepHistoryID(args.ActionStepID, args.RealMode);

                    try
                    {
                        SupervisorSOPClose.SupervisorSOPRemoveSOP(nActionStepHistoryID);
                        //SDMS.ScriptProxy.Instance.UserObject.SupervisorSOPRemoveSOP.Invoke(nActionStepHistoryID);
                    }
                    catch (Exception)
                    {
                    }
                }
               
			}
			if (args.State == WorkFlowState.DONE)
            {
                UnE.SOP.Workstate.WorkFlow workflow = (UnE.SOP.Workstate.WorkFlow)sender;
                bool bSendSMS = workflow.BeginEndEventSendSMS;
                TabPageManager.Instance.SetUsePage(args.ActionStepID, false, args.RealMode);
                HistoryManager.Instance.AddActionStepHistory(args.ActionStepID, args.RealMode, args.State, args.Time, args.NoDBWrite, bSendSMS);
                FormSOP.Instance.DoneWorkflow();
                m_pageHome.OnCloseWorkFlow(args.ActionStepID, args.RealMode, args.State);

                {
                    int nActionStepHistoryID = FormSOP.Instance.SOPManager.GetActionStepHistoryID(args.ActionStepID, args.RealMode);
                    
                    try
                    {
                        SupervisorSOPClose.SupervisorSOPRemoveSOP(nActionStepHistoryID);
                        //SDMS.ScriptProxy.Instance.UserObject.SupervisorSOPRemoveSOP.Invoke(nActionStepHistoryID);
                    }
                    catch(Exception)
                    {
                    }
                    
                }
            }
			if (args.State == WorkFlowState.RUN)
			{
                ComponentContents contents = m_pageHome.GetCurrentSelectedComponentContents();

                UnE.SOP.Workstate.WorkFlow workflow = (UnE.SOP.Workstate.WorkFlow)sender;
                bool bSendSMS = workflow.BeginEndEventSendSMS;
                HistoryManager.Instance.AddActionStepHistory(args.ActionStepID, args.RealMode, args.State, args.Time, args.NoDBWrite, contents == null ? null : contents.Section, bSendSMS);

                //if (workflow.State != WorkFlowState.RUN)
                {

                    try
                    {
                        int nSensorID = workflow.Option != null ? workflow.Option.SensorZoneID : -1;
                        m_nLastSensorZoneID = nSensorID;
                    }
                    catch(Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                        System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                    }
                    
                }
			}
		}

        private int m_nLastSensorZoneID = -1;
        internal void OnNewActionStepHistory(SOPScenario sco)
        {
            try
            {
                if (sco == null)
                    return;

                int nActionStepHistoryID = sco.ActionStepHistoryID;
                UnE.SOP.Workstate.WorkFlow work = WorkFlowManager.Instance.Get(sco.ActionStepID, sco.RealMode);
                if( work != null && work.Option != null)
                {
                    int nSensorZoneID = work.Option.SensorZoneID;
                    int nSensorHistoryID = work.Option.SensorZoneHistoryID;

                    SupervisorSOPClose.SupervisorSOPAddSOP(nActionStepHistoryID, nSensorZoneID, nSensorHistoryID);
                    //SDMS.ScriptProxy.Instance.UserObject.SupervisorSOPAddSOP.Invoke(nActionStepHistoryID, nSensorZoneID, nSensorHistoryID);
                }
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }                    
        }

		public bool FocusSection(Sections.Section section)
		{
			if (section == null)
				return false;

			Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();
			if (panel != null)
			{
                // 옵션에서 자동 컴포넌트 찾아가기가 켜진 경우에만 찾아감.
                if (m_enableFocusSection == true)
                    panel.FocusSection(section, 210);

				return true;
			}
			return false;
		}

        public void PostChangeSectionState(Section section, State state)
        {
            /*Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();

            if (panel == null)
                return;

            SectionTabPage page = (SectionTabPage)panel.Parent;*/
            ComponentContents contents = m_pageHome.GetComponentContents(section);

            if (contents != null)
            {
                contents.State = state;

                if (state == State.INPUT)
                    m_pageHome.SelectComponentContents(contents);
            }
        }

		public void OnLoadScenario(SOPScenario sopSc)
		{
			if (GetPageHome() == null)
				return;

			SOPScenarioManager.Instance.AddSOPScenario(sopSc);  
		}

		public void SelectedScenario(int nActionStepID, bool bReal)
		{
            EnableOptions(false);
			SetCurrentActionStep(nActionStepID, bReal);

			SOPScenarioManager.Instance.SelectedScenario(nActionStepID, bReal);
		}

		public SOPScenario GetCurrentSOPScenario()
		{
			return SOPScenarioManager.Instance.GetCurrentSOPScenario();
		}

		public bool IsWorkingMode(Section section)
		{
			return GetPageHome().IsWorkingMode(section);
		}

		public bool IsWorkingMode(int nActionStepID, bool bReal)
		{
			return PageBackstageSOP.IsWorkingMode(nActionStepID, bReal);
		}

		public void SetCurrentActionStep(int nActionStepID, bool bReal)
		{
			m_sopMgr.SetCurrentActionStep(nActionStepID, bReal);
		}

		private void StopScenario()
		{
			FormSOP.Instance.StopWorkflow(DateTime.Now);
		}


		private void StopAllScenario()
		{
			FormSOP.Instance.AllStopWorkflow();
		}


		public void DeleteAllScenario()
		{
			StopAllScenario();

			int nTargetActionStep = -1;

			ArrayList arScenario = SOPScenarioManager.Instance.GetAllScenario();
			foreach (SOPScenario row in arScenario)
			{
				nTargetActionStep = row.ActionStepID;
				bool bReal = row.RealMode;
				if (nTargetActionStep == -1)
					continue;

				SectionTabPage page = (SectionTabPage)TabPageManager.Instance.GetPage(nTargetActionStep, bReal);
				if (page != null)
				{
					TabPageManager.Instance.RemovePage(nTargetActionStep, bReal);
					WorkFlowManager.Instance.Remove(nTargetActionStep, bReal);
				}
			}
			SOPScenarioManager.Instance.ClearScenario();

			GetPageHome().panel.Visible = false;
			GetPageHome().SetBackgroundImage(false);
			WaitWorkflow();

			//GetBarLevelTree().UnSelectedNode();

			GetPageHome().ClearProcess();
		}        
   
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
				{
					radioRealMode.Checked = true;
					ProxySOP.Instance.RealMode = true;
				}
			}
			else if (sender == labelVirtual)
			{
				if (radioVirtualMode.Enabled)
				{
					radioVirtualMode.Checked = true;
					ProxySOP.Instance.RealMode = false;
				}
			}
			/*else if (sender == labelRegular)
			{
				if (radioRegistMode.Enabled)
				{
					radioRegistMode.Checked = true;
					ProxySOP.Instance.RegisterMode = true;
				}
			}
			else if (sender == labelNonRegular)
			{
				if (radioNonRegistMode.Enabled)
				{
					radioNonRegistMode.Checked = true;
					ProxySOP.Instance.RegisterMode = false;
				}
			}*/
			else if (sender == labelNormal)
			{
				if (radioNormal.Enabled)
				{
					radioNormal.Checked = true;
					ProxySOP.Instance.NormalMode = true;
				}
			}
			else if (sender == labelHoliday)
			{
				if (radioHoliday.Enabled)
				{
					radioHoliday.Checked = true;
					ProxySOP.Instance.NormalMode = false;
				}
			}
		}

		bool prevStart = false;
		bool prevPause = false;
        bool prevDayLight = false;

		private void timer1_Tick(object sender, EventArgs e)
		{
			bool broadcastPause = (TTSManager.Instance.State == SpeechState.PAUSE ? true : false);
			bool broadcastStart = (TTSManager.Instance.State == SpeechState.PLAY ? true : false);

			if (prevStart != broadcastStart || prevPause != broadcastPause)
			{
				//DateTime dtNow = DateTime.Now;
				//string strTime = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);

				prevStart = broadcastStart;
				prevPause = broadcastPause;
			}

            DateTime dtNow = DateTime.Now;
            labelDate.Text = string.Format("{0}년 {1}월 {2}일", dtNow.Year, dtNow.Month, dtNow.Day);
            labelTime.Text = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);

            bool isDayLight = Popup.SOPLoader.IsDayLight_NoInvoke(DateTime.Now);

            if (isDayLight != prevDayLight)
            {
                // 모드가 바뀌면 SOP 퀵버튼의 상태를 바꿔준다.
                if (ChangeDayLight(isDayLight))
                    prevDayLight = isDayLight;
            }

#if E_SOP
                // 에너지과제용 모바일 앱(e-SOP)을 위한 임시 기능
                // e-SOP(Mobile App)으로부터 전송된 Command가 있는지 확인한다.
                CheckESOPCommand();
#endif

            //btnStartBroadcast.Enabled = broadcastPause;
			//btnStopBroadcast.Enabled = broadcastStart || broadcastPause;
		}

#if E_SOP
        // 에너지과제용 모바일 앱(e-SOP)을 위한 임시 기능
        private void CheckESOPCommand()
        {
            string strSQL = "Select ID, ActionStepID, RealMode, ProcessID, Checked from MobileAppCommand where Processed = 0";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            string strIDs = "";

            for (int i=0;i<nResultCount-4;i+=5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> realMode = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> processID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> isChecked = WebDBManager.GetIntField(arrResult[i + 4].ToString());

                if (id == null || actionStepID == null || realMode == null || processID == null || isChecked == null)
                    continue;

                if (m_pageHome.ProcessComponentContents(actionStepID.Data, realMode.Data == 1, processID.Data, isChecked.Data == 1))
                {
                    if (strIDs.Length == 0)
                        strIDs = id.Data.ToString();
                    else
                        strIDs += ", " + id.Data.ToString();
                }
            }

            if (strIDs.Length > 0)
            {
                strSQL = string.Format("Update MobileAppCommand set Processed = 1 where ID in ({0})", strIDs);
                m_dbMgr.GetResultData(strSQL, 0);
            }
        }
#endif

        private void pictureBoxMainIcon_DoubleClick(object sender, EventArgs e)
		{
            ShowMonitoringSystem(false);
		}

        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        public void Update3DView()
        {
            ProxyMessenger.Instance.Update3DView();
            /*if(m_frmMain2 != null)
            {
                m_frmMain2.Update3DView();
            }*/
        }

        public System.Diagnostics.Process ToggleSOPBulletin()
        {
            ExecuteManager mgr = new ExecuteManager();

            System.Diagnostics.Process process = mgr.RunCheckProcess("SOPBulletin");

            if (process == null)
            {
                return mgr.RunStartProcess("SOPBulletin.exe", null);
            }
            else
                process.CloseMainWindow();

            return null;
        }

        private void btnSDMS_Click(object sender, EventArgs e)
        {
            ProxyMessenger.Instance.ToggleMinimumWindow();
            //m_frmMain2.ToggleMinimumWindow();
        }

        private void btnBulletin_Click(object sender, EventArgs e)
        {
            ToggleSOPBulletin();
            //m_frmMain2.ToggleSOPBulletin();
        }

        private void btnMissionStatus_Click(object sender, EventArgs e)
        {
            VisibleMissionStatus = !VisibleMissionStatus;
        }

        private void btnDefaultCCTV_Click(object sender, EventArgs e)
        {
            ToggleCCTV();
        }

        public void ToggleCCTV()
        {
            ProxyMessenger.Instance.ToggleCCTV();
            /*if(UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                IDisasterContainer container = UnE.SOP.ProxySOP.Instance.SOPDisasterContainer;
                if( container != null)
                {
                    container.ShowCCTVForm(true);
                }
            }*/
        }

        public void ShowSpecialMessageHelp()
        {
            if (m_frmForSpecialMessage == null || m_frmForSpecialMessage.IsDisposed)
            {
                m_frmSpecialMessageHelp = new SOPManager.PopupSpecialMessage();
                m_frmSpecialMessageHelp.Icon = this.Icon;
                m_frmForSpecialMessage = new DialogFormFrame(m_frmSpecialMessageHelp);
            }

            if (m_frmForSpecialMessage.Visible)
                return;

            m_frmForSpecialMessage.Show(this);
        }

        public void SelectComponent(int nActionStepID, bool isRealMode, Sections.Section section)
        {
            m_pageHome.SelectComponentContents(nActionStepID, isRealMode, section);
        }

        public Sections.SectionCommander LoadSectionCommander(int nTeamType, int nMemberID, string strDisplayText)
        {
            return IOManager.LoadCommanderTeamMember(m_dbMgr, nTeamType, nMemberID, strDisplayText);
        }


        public void ShowTestBroadcast()
        {
            if (m_ExeManager != null)
                m_ExeManager.Run(ExecuteManager.APP_TYPE.BROADCAST_TESTER);
        }


        public void ShowSendSMS()
        {
            if (m_ExeManager != null)
                m_ExeManager.Run(ExecuteManager.APP_TYPE.SMS_SENDER);
        }

        public void SectionButtonClicked(Sections.SectionButton btn, int x, int y)
        {
            if( btn.GetComponentType() == SectionButton.ComponentType.ENDPOINT)
            {
                ButtonEndPoint startBtn = (ButtonEndPoint)btn;
                SectionDataEndPoint data = (SectionDataEndPoint)startBtn.Data;
                if(data.IsBegin == true)
                {
                    if (HasControl == true)
                    {
                        this.BeginInvoke(new Action(() => 
                            {
                                Play();
                                
                            }
                        ));                 
                      
                        
                    }
                }
                else
                {
                    if (HasControl == true)
                    {
                        this.BeginInvoke(new Action(() =>
                        {
                            
                            WorkFlow work = GetCurrentWorkflow();
                            if (work != null && work.State == WorkFlowState.RUN)
                            {                            
                                SectionState state = work.FindState(btn.Section);
                                if( state != null)
                                {
                                    btn.Notify(false);
                                    btn.Hide();
                                    state.Complete();
                                }
                            }                            
                        }
                        ));


                    }
                }
            }
        }
        
        private void m_frmWorkSchedule_MemberWorkDataChanged(object sender, EventArgs e)
        {
            m_netMgr.SendChangedWorkingMemberData();
            NeedToUpdateWorkingMemberData();
        }

        public void NeedToUpdateWorkingMemberData()
        {
            ControlTeamEditor.VaildMemberPhoneNumber.NeedToUpdateWorkingMemberData();
        }

        public void BeginHistory()
        {
            m_pageHome.BeginHistory();
        }

        public void EndHistory()
        {
            m_pageHome.EndHistory();
        }

        public string GetDefaultCallerPhoneNumber()
        {
            bool isDayLight = Popup.SOPLoader.IsNormal(DateTime.Now);
            string strPhoneNumber = "";

            if (isDayLight)
            {
                if (m_sopGenUserCommanderDayLight != null)
                    strPhoneNumber = m_sopGenUserCommanderDayLight.CallerPhoneNumber;
                else if (m_sopGenUserCommanderNightHoliday != null)
                    strPhoneNumber = m_sopGenUserCommanderNightHoliday.CallerPhoneNumber;
            }
            else
            {
                if (m_sopGenUserCommanderNightHoliday != null)
                    strPhoneNumber = m_sopGenUserCommanderNightHoliday.CallerPhoneNumber;
                else if (m_sopGenUserCommanderDayLight != null)
                    strPhoneNumber = m_sopGenUserCommanderDayLight.CallerPhoneNumber;
            }

            return strPhoneNumber;
        }

        private bool ReadPSMInfo()
        {
            string strSQL = "Select PropertyName, PropertyValue from OptionSDMS where PropertyName = 'UsePSM' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

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

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

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

        private bool ChangeDayLight(bool isDayLight)
        {
            SetDayLightMode(isDayLight);

            if (this.HasControl)
            {
                m_pageHome.OnEnabled(true);
                return true;
            }

            return false;
        }

        private void SetDayLightMode(bool isDayLight)
        {
            // 현재 로딩중인(실행중이지 않은 SOP 포함) SOP가 없으면 시간대에 맞게 설정을 바꾼다.
            ArrayList arrTabPages = this.m_pageHome.GetTabPage();

            if (arrTabPages != null)
            {
                if (arrTabPages.Count == 0)
                {
                    if (isDayLight)
                        radioNormal.Checked = true;
                    else
                        radioHoliday.Checked = true;
                }
            }
        }
    }
}
