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
using DBUtility2;
using libSOPPolicy;
using System.Collections.Concurrent;

namespace SOPMonitoringSystem
{
	using Process;
	using Sections;
    using SOPMonitoringSystem.Data;

    public enum LoginUserType { General = 0, IndividualManager/*개별 관리자*/, IntegratedManager/*통합관리자*/ }
    public partial class FormSOP : Form, ISOPContainer, IRibbonButtonOwner, ISOPOwner/*, SDMS.ICCTVFormOwner*/ //, SOPDisasterSystem.ISOPInfo
        , IWorkflowContainer
    {
        /*private class SOPConfirmData
        {
            private int m_nActionStepID = -1;
            private bool m_isRealMode = false;
            private int m_nSOPGenUserID = -1;
            private WorkFlow m_workflow = null;
            private bool m_confirm = false;

            public int ActionStepID
            {
                get { return m_nActionStepID; }
            }

            public bool IsRealMode
            {
                get { return m_isRealMode; }
            }

            public int SOPGenUserID
            {
                get { return m_nSOPGenUserID; }
            }

            public WorkFlow WorkFlow
            {
                get { return m_workflow; }
            }

            public bool Confirm
            {
                get { return m_confirm; }
                set { m_confirm = value; }
            }

            public SOPConfirmData(int nActionStepID, bool isRealMode, int nSOPGenUserID, WorkFlow workflow)
            {
                m_nActionStepID = nActionStepID;
                m_isRealMode = isRealMode;
                m_nSOPGenUserID = nSOPGenUserID;
                m_workflow = workflow;
            }
        }*/

        // 각 메인폼의 창 위치
        private int nMonitoring = 1;
        private int nDisaster = 2;
        private int nMission = 3;

        // Button별 ID
        private Dictionary<Button, int> m_dicButtonIDs = new Dictionary<Button, int>();
        private Dictionary<int, Button> m_dicIDButtons = new Dictionary<int, Button>();
        private Dictionary<Button, bool> m_dicButtonChecked = new Dictionary<Button, bool>();

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
        private VariousData<DateTime> m_dtLastAccessedSOP = null;
        // 마지막으로 인사정보가 수정된 시간
        private VariousData<DateTime> m_dtLastAccessedMember = null;

        // 제어권이 없는 SOP에 대하여 모니터링만 할수 있는가?
        // (다른 클라이언트가 제어하고 있는 SOP 화면을 나타낼 것인가?)
        private bool m_useSOPMonitoring = true;

        public bool UseSOPMonitoring
        {
            get { return m_useSOPMonitoring; }
        }

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

        private NetworkWebManager m_netMgr = null;

        public NetworkWebManager NetworkManager
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

        private LoginUserType m_userType = LoginUserType.General;
        public LoginUserType UserType
        {
            get { return m_userType; }
            set { m_userType = value; }
        }

        //private int m_nSirenCount = 0;
        private int m_nSOPGenUserID = -1;
        public int UserID
        {
            get { return m_nSOPGenUserID; }
            set { m_nSOPGenUserID = value; }
        }
        private int m_nSOPGenUserLevel = 1;
        // m_nSOPGenUserID에 대한 평일, 주간의 최고 책임자
        private Sections.SectionCommander m_sopGenUserCommanderDayLight = null;
        // m_nSOPGenUserID에 대한 휴일, 야간의 최고 책임자
        private Sections.SectionCommander m_sopGenUserCommanderNightHoliday = null;

        private string m_strSOPGenUserRealName = "";
        public string LoginID
        {
            get { return m_strSOPGenUserRealName; }
            set { m_strSOPGenUserRealName = value; }
        }

        private ArrayList m_arrConnectedUser = new ArrayList();
        private Thread DBWrite = null;

        private ControlTeamEditor.FormMemberWorkSchedule m_frmWorkSchedule = null;

        //private bool m_smsOn = false;
        // 협력업체들에게도 SOP 문자메시지를 보낼것인가?
        private bool m_smsExternalCompanyMemberOn = false;

        //private int m_nControlUserID = -1;

        private Font m_fontMenuButtons = new System.Drawing.Font("나눔바른고딕", 12.5F, System.Drawing.FontStyle.Bold);

        private ExecuteManager m_exeMgr = null;

        // 제어권 반납을 하면 적어도 m_nReturnControlWaitTime(초) 만큼은 제어권을 다시 갖지 않는다.
        private int m_nReturnControlWaitTime = 5;
        private int m_nReturnControlActionStepHistory = -1;
        private DateTime m_dtReturnControl = new DateTime();

        // Key : ActionStepHistoryID
        private Dictionary<int, DateTime> m_dicCloseActionStepHistoryIDs = new Dictionary<int, DateTime>();
        // CheckNewActionStepHistory()를 통하여 이미 처리된 ActionStepHistory인지 검사
        // Key : ActionStepHistoryID
        private Dictionary<int, Data_ActionStepHistory> m_dicOldActionStepHistorys = new Dictionary<int, Data_ActionStepHistory>();

        /*public int ControlUserID
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
        }*/

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
                if (m_pageHome != null)
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

        private BaseSOPUser m_sopUser = null;


        // 제어권 요청창
        private PopupRequestProgress m_frmRequestProgress = null;

        // 제어권 요청 리스트
        PopupRequestControl m_frmRequestControl = null;

        // 현재 실행중인 SOP의 실행임무들에 대한 상세 옵션
        private Dictionary<Sections.MissionItem, MissionItemInfo> m_dicMissionInfo = new Dictionary<MissionItem, MissionItemInfo>();

        UnE.GUI.DialogFormFrame m_frmForSpecialMessage = null;

        private bool m_initializeComponent = false;

        //////////////////////////////////////////////////////////////////////////

        // 처음 로딩시 기존에 실행중이던 SOP를 불러오게 되면 수많은 화면 갱신이 필요하게 된다.
        // 이때, 모든 DB정보를 읽은 다음 마지막 한번만 화면 갱신을 하기 위하여 마지막 ComponentHistoryID를 기억해둔다.
        private int m_nInitComponentHistoryID = -1;
        public int InitComponentHistoryID
        {
            get { return m_nInitComponentHistoryID; }
            set { m_nInitComponentHistoryID = value; }
        }

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
            get { return rbtnCheckRealMode.IsChecked; }
        }

        // 등록 버전인가?
        public bool IsRegular
        {
            get { return true/*radioRegistMode.Checked*/; }
        }

        private bool m_isNormal = true;
        // 평일 버전인가?
        public bool IsNormal
        {
            get { return m_isNormal; }
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
            get { return SOPMonitoringSystem.FormSOP.Instance; }
        }

        /*public ProxyMessenger ProxyMessenger
        {
            get { return (ProxyMessenger)m_frmMain2.ProxyMessenger; }
        }*/

        // 마지막으로 SOP가 수정된 시간
        public VariousData<DateTime> LastAccessedSOPTime
        {
            get { return m_dtLastAccessedSOP; }
            set { m_dtLastAccessedSOP = value; }
        }

        // 마지막으로 인사정보가 수정된 시간
        public VariousData<DateTime> LastAccessedMemberTime
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

        private OptionManager m_optionMgr = null;
        public OptionManager OptionMgr
        {
            get { return m_optionMgr; }
        }

        public BaseSOPUser SOPUser
        {
            get { return m_sopUser; }
        }

        // 센서신호 받으면 훈련모드로 동작하는가?
        private bool m_virtualModeInSensor = false;
        public bool VirtualModeInSensor
        {
            get { return m_virtualModeInSensor; }
            set { m_virtualModeInSensor = value; }
        }

        // 실행중인 SOP에 대한 제어권 보유 유무
        // Key : ActionStepHistory ID
        // Value : 제어권 유무
        //private ConcurrentDictionary<int, bool> m_dicSOPControls = new ConcurrentDictionary<int, bool>();
        // 이전에 실행했던 ActionStep ID들
        // 이미 읽은 데이터를 다시 읽지 않기 위함
        // 최근 10개까지만 저장한다.
        //private ConcurrentQueue<int> m_pastActionSteHistoryIDs = new ConcurrentQueue<int>();
        // SOP를 새로 실행시킨후 Server로부터 Confirm을 기다리는 리스트
        //private ConcurrentDictionary<SOPConfirmData, SOPConfirmData> m_sopWaitConfirmDatas = new ConcurrentDictionary<SOPConfirmData, SOPConfirmData>();

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
            //bool bSituationRoomMode = UnE.SOP.ProxySOP.Instance.ShowCCTVForm;
            FormSOP f = new FormSOP(nSOPGenUserID, strSOPGenUserRealName, isSimulationMode, onlySDMS, nTargetMonitor/*, bSituationRoomMode*/, false);
        }

        public void LinkDisasterSystem(IDisasterContainer form)
        {
            //m_frmMain2 = (SDMS.FormMain)form;
            if (FormSOP.Instance != null)
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

            return true;
        }

        private void ReadSiteID()
        {
            Utility util = new Utility();
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
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strSiteName = WebDBManager.GetStringField(arrResult[0]);

                if (strSiteName != null)
                    UnE.SOP.ProxySOP.Instance.SiteName = strSiteName;
            }
        }

        private void SetLoginUserType()
        {
            /*
            m_sopUser = SOPUserFactory.CreateSOPUser(m_nSOPGenUserID, m_dbMgr);
            int nUserGrade = m_sopUser.GetUserGrade();

            if (nUserGrade == 1)
                picUser.Image = SOPMonitoringSystem.Properties.Resources.userLevel1;
            else if (nUserGrade == 2)
                picUser.Image = SOPMonitoringSystem.Properties.Resources.userLevel2;
            else if (nUserGrade == 3)
                picUser.Image = SOPMonitoringSystem.Properties.Resources.userLevel3;

            labelUserName.Text = m_sopUser.UserID;
            if (labelUserName.Text.Length > 9)
                labelUserName.Text = labelUserName.Text.Substring(0, 9);

            if (m_sopUser.AbleToEditTools() == false)
            {
                btnSOPManager.Visible = false;
                btnTeamEditor.Visible = false;
                btnSOPManager2.Visible = false;
                btnTeamEditor2.Visible = false;
            }
            */
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT UserID, NickName, UserLevel, LevelName ");
            sb.Append("  FROM SopGenUser as u INNER JOIN SOPGenLevel as l ON u.UserLevel = l.ID ");
            sb.AppendFormat(" WHERE u.ID = {0} AND SiteID = {1}", m_nSOPGenUserID, UnE.SOP.ProxySOP.Instance.SiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count != 4)
                return;

            string strUserID = DBUtility2.WebDBManager.GetStringField(arrResult[0], "");
            string strNickName = DBUtility2.WebDBManager.GetStringField(arrResult[1], "");
            int nUserLevel = DBUtility2.WebDBManager.GetIntField(arrResult[2].ToString(), -1);
            string strLevelName = DBUtility2.WebDBManager.GetStringField(arrResult[3], "");

            labelUserName.Text = strUserID;
            if (labelUserName.Text.Length > 9)
                labelUserName.Text = labelUserName.Text.Substring(0, 9);

            // LevelID로 판단할 때
            //if (nUserLevel == 0)
            //    m_userType = LoginUserType.IntegratedManager;
            //else if (nUserLevel > 0 && nUserLevel < 5)
            //    m_userType = LoginUserType.IndividualManager;
            //else
            //    m_userType = LoginUserType.General;

            // LevelName으로 판단할 때            
            if (strLevelName.Replace(" ","").Trim().Contains("총괄관리자"))
            {
                m_userType = LoginUserType.IntegratedManager;
                picUser.Image = SOPMonitoringSystem.Properties.Resources.userLevel3;
            }
            else if (strLevelName.Replace(" ", "").Trim().Contains("관리자"))
            {
                m_userType = LoginUserType.IndividualManager;
                picUser.Image = SOPMonitoringSystem.Properties.Resources.userLevel2;
            }
            else
            {
                m_userType = LoginUserType.General;
                picUser.Image = SOPMonitoringSystem.Properties.Resources.userLevel1;
            }

            if (m_userType == LoginUserType.General)
            {
                btnSOPManager2.Visible = false;
                btnTeamEditor2.Visible = false;
                rbtnConfig.Visible = false;
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
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }
        public FormSOP(int nSOPGenUserID, string strSOPGenUserRealName, bool isSimulationMode, bool onlySDMS, int nTargetMonitor, bool fromLogin)
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
            m_initializeComponent = true;

            if (fromLogin)
                picUser.Click += picUser_Click;

            SetDoubleBuffer(panelTop, true);


            m_nPanelTopInitHeight = panelTop.Size.Height;

            m_instance = this;

            m_dbMgr = m_isSimulationMode ? new SimulationDBManager(this, m_nSiteID) : new WebDBManager(this, m_nSiteID);

            SetLogo();
            ReadSiteName();
            m_sopUser = SOPUserFactory.CreateSOPUser(m_nSOPGenUserID, m_dbMgr);

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

            labelUserName.Text = strSOPGenUserRealName;
            SetLoginUserType();

            m_exeMgr = new ExecuteManager(this);

            instanceHandle = this.Handle;
            SetRibbonButtonFont();

            m_optionMgr = new OptionManager();
        }

        private void picUser_Click(object sender, EventArgs e)
        {
            contextMenuStripLogout.Show(this, picUser.Location.X + picUser.Size.Width / 2, picUser.Location.Y + picUser.Size.Height);
        }

        private void SetLogo()
        {
            string strLogoName = "logo_" + m_dbMgr.SiteID.ToString();
            object obj = SOPMonitoringSystem.Properties.Resources.ResourceManager.GetObject(strLogoName, SOPMonitoringSystem.Properties.Resources.Culture);

            if (obj != null)
            {
                Bitmap bmpLogo = (System.Drawing.Bitmap)obj;

                int width = pictureBoxLogo.Size.Width < bmpLogo.Size.Width ? bmpLogo.Size.Width : pictureBoxLogo.Size.Width;
                int height = pictureBoxLogo.Size.Height < bmpLogo.Size.Height ? bmpLogo.Size.Height : pictureBoxLogo.Size.Height;

                this.pictureBoxLogo.Size = new Size(width, height);
                this.pictureBoxLogo.BackgroundImage = bmpLogo;
            }

            /*if (m_dbMgr.SiteID == 201)
                this.pictureBoxLogo.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.logo_201;*/
        }

        private void SetRibbonButtonFont()
        {
            rbtnLoadSOP.Font = m_fontMenuButtons;
            rbtnControlStatus.Font = m_fontMenuButtons;
            rbtnControlAction.Font = m_fontMenuButtons;
            rbtnStartSOP.Font = m_fontMenuButtons;
            rbtnCancelSOP.Font = m_fontMenuButtons;
            rbtnRealMode.Font = m_fontMenuButtons;
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
                FormSOP.Instance.Visible = true;
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

                    this.Visible = visible;
                    if (visible)
                    {
                        this.WindowState = FormWindowState.Maximized;

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
            SetMonitorForm(this, m_nTargetMonitor);
            this.Visible = false;

            UnE.SOP.ProxySOP.Instance.UsePSM = ReadPSMInfo();
            UnE.SOP.ProxySOP.Instance.UseIntrusion = ReadIntrusionInfo();

            ReadOption();
            m_optionMgr.ReadUsageStatus();
            InitTab();
            InitPanels();

            SetMonitors();
            m_pageHome.Visible = false;
            SendButtonsToPageHome();
            
            btnSOPManager2.Tag = ExecuteManager.APP_TYPE.SOP_MANAGER;
            btnTeamEditor2.Tag = ExecuteManager.APP_TYPE.TEAM_MANAGER;

            ProxySOP.Instance.NormalMode = IsNormal;
            ProxySOP.Instance.RealMode = IsReal;
            ProxySOP.Instance.RegisterMode = IsRegular;

            ProxySOP.Instance.WorkflowContainer = this;
            ProxySOP.Instance.SOPLogContainer = GetPageHome().GetDockSOPLog();
            ProxySOP.Instance.PageContainer = SOPScenarioManager.Instance;

            SOPScenarioManager.Instance.CreateLevelTree();
            ProxySOP.Instance.SOPTreeContainer = SOPScenarioManager.Instance.GetBarLevelTree();
            //ProxySOP.Instance.SOPDisasterContainer = (UnE.SOP.IDisasterContainer)m_frmMain2.PageHome.ContentForm;

            HistoryManager2.MakeInstance();
            SectionState.HistoryManager = HistoryManager2.Instance;

            m_sopMgr.Load(IsRegular, IsNormal);
            SetSOPGenUserCommander();
            //HistoryManager.Instance.LoadActionStepHistory(m_dbMgr);

            DataManager.Instance.Init();

            InitButtons();

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

            if (m_netMgr == null)
                m_netMgr = new NetworkWebManager(m_dbMgr);

            // Splash가 종료되면 m_onlySDMS에 따라 Visible이 결정되도록 한다.
            //MainFrame.Visible = !m_onlySDMS;

            //MainFrame.Location = FormFrame.Instance.OriginLocation;
            //MainFrame.WindowState = FormWindowState.Maximized;


#if SAFE_KOREA_YH_2017
            // 우선적으로 문자메시지를 받을 사람들을 지정한다.
            UnE.SOP.SMS.SMSManager.Instance.SetVIPPhoneNumbers(m_dbMgr, m_nSiteID);
#endif

            LoadSopSupervisor();
            // 우선순위에 따라 Disaster별 ActionStep을 정렬한다.
            SortDisasterActionSteps();

            SetDayLightMode(Popup.SOPLoader.IsDayLight_NoInvoke(DateTime.Now));
        }

        // 우선순위에 따라 Disaster별 ActionStep을 정렬한다.
        private void SortDisasterActionSteps()
        {
            SortDisasterActionSteps(m_sopMgr.GetSOPDictionary(true, true));
            SortDisasterActionSteps(m_sopMgr.GetSOPDictionary(true, false));
            SortDisasterActionSteps(m_sopMgr.GetSOPDictionary(false, true));
            SortDisasterActionSteps(m_sopMgr.GetSOPDictionary(false, false));
        }

        // 우선순위에 따라 Disaster별 ActionStep을 정렬한다.
        private void SortDisasterActionSteps(Dictionary<string, DisasterInfo> dicSOPDisasters)
        {
            foreach (KeyValuePair<string, DisasterInfo> pair in dicSOPDisasters)
            {
                if (m_sopMonitor != null)
                    m_sopMonitor.SortDisasterActionSteps(pair.Value);
            }
        }

        private ISupervisor m_sopMonitor = null;
        //private SupervisorSOPClose m_SopMonitor = null;

        public ISupervisor SOPSupervisor
        {
            get { return m_sopMonitor; }
        }

        public void LoadSopSupervisor()
        {
            //SDMS.ScriptProxy proxy = SDMS.ScriptProxy.Instance;

            m_sopMonitor = SupervisorFactory.MakeInstance(m_dbMgr.SiteID);
            m_sopMonitor.Start(m_dbMgr, SOPScenarioManager.Instance, this, this);
            //m_SopMonitor = new SupervisorSOPClose(m_dbMgr);
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
                    m_sopMonitor.TouchSOP(nActionHistoryID);
                    //SupervisorSOPClose.SupervisorSOPTouch(nActionHistoryID);
                    //SDMS.ScriptProxy.Instance.UserObject.SupervisorSOPTouch.Invoke(nActionHistoryID);
                    System.Diagnostics.Trace.WriteLine("Touch Section : " + section.SectionName);
                }
            }
        }



        public void ReadLastAccessedTime(ref VariousData<DateTime> dtSOP, ref VariousData<DateTime> dtMember)
        {
            string strSOPTag = "LastAccessedSOPTime";
            string strMemberTag = "LastAccessedMemberTime";

            string strSQL = string.Format("Select PropertyName, PropertyValue from OptionSOPSimulator where (PropertyName = '{0}' or PropertyName = '{1}') and SiteID = {2}",
                strSOPTag, strMemberTag, m_nSiteID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i]);
                VariousData<DateTime> dtValue = WebDBManager.GetDateTimeField(arrResult[i + 1]);

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
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
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
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null)
            {
                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
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
        }

        private void SetMovingText()
        {
            string strSQL = strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='MovingText' AND SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = WebDBManager.GetStringField(arrResult[0], "");

                if (strValue != "" && strValue != "null")
                {
                    m_useMovingText = strValue == "1";
                }
            }
        }

        private void LoadPopupSensorOn()
        {
            string strSQL = strSQL = "SELECT PropertyValue FROM OptionSDMS where PropertyName ='PopupSensorOn' AND SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count > 0)
            {
                string strValue = WebDBManager.GetStringField(arrResult[0], "");

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


            string szMonitoring = RegUtil.ReadRegValue("Monitor Info", "SOPSimulator", m_nSiteID);
            if (szMonitoring == null || szMonitoring == "")
                szMonitoring = DBManager.LoadIni("MonitoringSystem", "Monitor Info");
            int.TryParse(szMonitoring, out nMonitoring);

            UnE.SOP.ProxySOP.Instance.SimulatorMonitor = nMonitoring;

            int nCCTV = 3;
            string szCCTVForm = RegUtil.ReadRegValue("Monitor Info", "CCTV", m_nSiteID);
            if (szCCTVForm == null || szCCTVForm == "")
                szCCTVForm = DBManager.LoadIni("CCTVForm", "Monitor Info");
            int.TryParse(szCCTVForm, out nCCTV);

            UnE.SOP.ProxySOP.Instance.CCTVMontior = nCCTV;

            string szDisaster = RegUtil.ReadRegValue("Monitor Info", "SDMS", m_nSiteID);
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


            string szMission = RegUtil.ReadRegValue("Monitor Info", "MissionList", m_nSiteID);
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

            this.WindowState = FormWindowState.Normal;
            SetMonitorForm(this, nMonitoring, !OnlySDMS);
            //MainFrame.Visible = !OnlySDMS;
            this.Visible = false;
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

        private void SetOptionFromPage()
        {
            TTSManager.Instance.UseBroadcast = m_pageOption.UseBroadcast;
            SMSManagerEx.Instance.UseSMS = m_pageOption.UseSMS;
            m_smsExternalCompanyMemberOn = m_pageOption.UseExternalMemberSMS;
            m_virtualModeInSensor = m_pageOption.VirtualModeInSensor;
            UnE.SOP.ProxySOP.Instance.ConfirmSendSMS = m_pageOption.ConfirmSendSMS;
        }

        private void ReadOption()
        {
            //SMSManagerEx.Instance.UseSMS = LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_SMS), "문자 사용여부");

            //m_smsExternalCompanyMemberOn = FormSOP.Instance.LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.SMS_TO_EXTERNAL_MEMBER), "외부회사직원에게 문자 전송");

            //TTSManager.Instance.UseBroadcast = LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.USE_BROADCAST), "방송 사용여부");

            string strMIssionText = DBManager.LoadIni("show_mission_text", "Server Connection Info");
            m_showMissionText = strMIssionText == "1";

            ReadLastAccessedTime(ref m_dtLastAccessedSOP, ref m_dtLastAccessedMember);

            m_bSensorDetectLoadAndPlay = LoadDBOption(SOP.SOPSimulatorConfig.GetPropertyName(SOP.SOPSimulatorConfig.ConfigType.RUN_SOP_ON_LOADED), "센서 탐지 로딩 완료시 자동 시작", "1");

            ReadStandardActionStepNames();
            m_useSOPMonitoring = LoadDBOption("UseSOPMonitoring", "다른 클라이언트가 제어중인 SOP 화면을 실시간으로 모니터링 할 것인가?", "1");
        }

        private void ReadStandardActionStepNames()
        {
            string strPropertyName = "StandardActionStepNames";

            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = '" + strPropertyName + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
            {
                InsertSOPDBOption(strPropertyName, szSaveDefault, strDescription);
                return GetBooleanValue(szSaveDefault);
            }

            string strValue = WebDBManager.GetStringField(arrResult[0], "");
            return GetBooleanValue(strValue);
            /*int nValue;

            if (!int.TryParse(strValue, out nValue))
                return false;

            return nValue == 0 ? false : true;*/
        }

        private bool GetBooleanValue(string strValue)
        {
            int nValue;

            if (!int.TryParse(strValue, out nValue))
                return false;

            return nValue == 0 ? false : true;
        }

        private int GetMaxID(string strTableName, WebDBManager dbMgr)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        public bool InsertSOPDBOption(string strPropertyName, string strPropertyValue, string strDescription)
        {
            int nID = GetMaxID("OptionSOPSimulator", m_dbMgr) + 1;

            string strSQL = string.Format("Insert into OptionSOPSimulator (ID, PropertyName, PropertyValue, Description, SiteID) values ({0}, '{1}', '{2}', '{3}', {4})",
                nID, strPropertyName, strPropertyValue, strDescription, m_nSiteID);

            return m_dbMgr.GetResultData(strSQL) != null;
        }

        private void InitTab()
        {
            m_pageHome = new PageBackstageSOP();
            //m_pageHome.Visible = true;
            m_pageOption = new PageBackstageOption();
            m_pageMessage = new PageBackStageMessage();

            SetOptionFromPage();

            //this.Controls.Add(m_pageOption);
            m_pageOption.Visible = false;
            m_pageMessage.Visible = false;

            if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
            {
                GetPageHome().ShowCCTVToolStripMenuItem(false);
            }
            else
            {
                GetPageHome().ShowCCTVToolStripMenuItem(true);
            }

            SelectViewTab(false);
        }

        protected virtual void InitPanels()
        {
            panelTop.Size = new Size(this.ClientSize.Width, panelTop.Size.Height);
            panelMain.Location = new Point(panelTop.Location.X, panelTop.Location.Y + panelTop.Size.Height);
            panelMain.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - panelMain.Location.Y);

            labelUserName.Location = new Point(rbtnConfig.Location.X - labelUserName.Width - 5, (panelTop.Height / 2) - (labelUserName.Height / 2));
            picUser.Location = new Point(labelUserName.Location.X - picUser.Width, (panelTop.Height / 2) - (picUser.Height / 2));
            btnBulletin.Location = new Point(picUser.Location.X - btnSOPManager2.Width - 20, (panelTop.Height / 2) - (btnSOPManager2.Height / 2));
            btnSOPManager2.Location = new Point(btnBulletin.Location.X - btnSOPManager2.Width - 10, (panelTop.Height / 2) - (btnSOPManager2.Height / 2));
            btnTeamEditor2.Location = new Point(btnSOPManager2.Location.X - btnTeamEditor2.Width - 10, (panelTop.Height / 2) - (btnTeamEditor2.Height / 2));
            //btnSOPManager.Location = new Point(picUser.Location.X - btnSOPManager.Width - 20, (panelTop.Height / 2) - (btnSOPManager.Height / 2));
            //btnTeamEditor.Location = new Point(btnSOPManager.Location.X - btnTeamEditor.Width - 5, (panelTop.Height / 2) - (btnTeamEditor.Height / 2));
        }

        private void InitButtons()
        {
            Image imgMouseOverBkgnd = global::SOPMonitoringSystem.Properties.Resources.RibbonMouseOver_bkgnd;
            Image imgCheckedBkgnd = global::SOPMonitoringSystem.Properties.Resources.RibbonChecked_bkgnd;
            //Image imgDisabledBkgnd = global::SOPMonitoringSystem.Properties.Resources.RibbonDisabled_bkgnd;

            SetControl(false);

            // 컨트롤
            InitRibbonButton(rbtnControlStatus, ID.ID_CONTROL_CONTROL);
            InitRibbonButton(rbtnControlAction, ID.ID_CONTROL_RETURN);

            // 실행
            InitRibbonButton(rbtnStartSOP, ID.ID_RUN_PLAY);
            InitRibbonButton(rbtnCancelSOP, ID.ID_RUN_CANCEL);

            rbtnControlStatus.Enabled = false;
        }

        /*private bool m_hasControl = false;
		public bool HasControl
		{
			get
			{
                return m_hasControl;
				//return btnControl.Text == "제어";
			}
		}*/

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
                //btnControl.Text = "제어";
                rbtnControlStatus.Text = "제어";
                //btnReturnControl.Text = "제어권 반납";
                rbtnControlAction.Text = "제어권 반납";
            }
            else
            {
                //btnControl.Text = "모니터링";
                rbtnControlStatus.Text = "모니터링";
                //btnReturnControl.Text = "제어권 요청";
                rbtnControlAction.Text = "제어권 요청";
            }

            //m_hasControl = hasControl;
            rbtnControlStatus.Visible = rbtnControlAction.Visible = false;
        }

        // 현재 화면에서 실행중인 SOP에 대한 제어권이 있는가?
        private bool GetCurrentControlStatus()
        {
            return rbtnControlStatus.Visible && rbtnControlStatus.Text == "제어";
        }

        public void SetControl(bool hasControl)
        {
            if (hasControl)
            {
                SetControlText(hasControl);

                if (m_netMgr != null)
                {
                    // 제어권을 가지게 되면 현재 진행중인 화재 상황에 대하여 SOP List를 팝업시킨다.
                    //m_netMgr.ShowDetectSignal();
                }

                OnEnabled(ID.ID_CONTROL_REQUEST);
                //WriteControlUserToDB();

                // 제어권 획득시에 이전에 사용중이던 UserDefinedTeam정보를 업데이트 한다.
                SectionTabPage page = (SectionTabPage)m_pageHome.TabControls.SelectedTab;
                if (page != null)
                {
                    m_pageHome.EnableButton(true);
                    m_pageHome.SOPTeamMemberManager.UpdateUsingTeams(page);
                    //m_pageHome.UpdateUsingUserDefinedTeam(page);
                }
            }
            else
            {
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

        /*private void WriteControlUserToDB()
        {
            string strSQL = "Select UserID from ControlUser where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            if (arrResult.Count == 0)
            {
                int nID = GetMaxID("ControlUser", m_dbMgr) + 1;

                strSQL = string.Format("Insert into ControlUser (ID, UserID, SiteID) values ({0}, {1}, {2})", nID, m_nSOPGenUserID, m_nSiteID);
                if (m_dbMgr.GetResultData(strSQL) == null)
                    return;
            }
            else
            {
                strSQL = "Update ControlUser set UserID = " + this.m_nSOPGenUserID.ToString() + " where SiteID = " + m_nSiteID.ToString();
                if (m_dbMgr.GetResultData(strSQL) == null)
                    return;
            }

            ControlUserID = m_nSOPGenUserID;
        }*/

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
            /*if (!HasControl)
			{
				rbtnStartSOP.Enabled = rbtnCancelSOP.Enabled = false;
				return;
			}*/

            WorkFlowManager manager = WorkFlowManager.Instance;
            PageBackstageSOP pageHome = GetPageHome();

            if (pageHome.tabControl.IsHandleCreated)
            {
                SectionTabPage page = (SectionTabPage)pageHome.tabControl.SelectedTab;

                if (page != null && page.ActionStepHistoryID > 0)
                {
                    /*rbtnControlStatus.Visible = rbtnControlAction.Visible = */rbtnStartSOP.Visible = rbtnCancelSOP.Visible = true;
                    pictureBoxFirst.Visible = pictureBoxSecond.Visible = panelSOPMode.Visible = true;

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
                                rbtnCancelSOP.Enabled = false;
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
                else
                {
                    rbtnControlStatus.Visible = rbtnControlAction.Visible = rbtnStartSOP.Visible = rbtnCancelSOP.Visible = false;
                    pictureBoxFirst.Visible = pictureBoxSecond.Visible = panelSOPMode.Visible = false;
                }
            }
        }

        public void EmptySOP()
        {
            rbtnControlStatus.Visible = rbtnControlAction.Visible = rbtnStartSOP.Visible = rbtnCancelSOP.Visible = false;
            pictureBoxFirst.Visible = pictureBoxSecond.Visible = panelSOPMode.Visible = false;

            rbtnStartSOP.Enabled = rbtnCancelSOP.Enabled = false;
            rbtnStartSOP.Refresh();
            rbtnCancelSOP.Refresh();
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

        private void CommandBarControlEnabled(bool isFlag)
        {
            WorkFlowManager manager = WorkFlowManager.Instance;
            PageBackstageSOP pageHome = GetPageHome();

            WorkFlowState workState = WorkFlowState.DISABLE;
            WorkFlow work = null;

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
                    work = (WorkFlow)manager.Get(ActionID, !page.VirtualMode);

                    if (work != null)
                    {
                        workState = work.State;
                    }
                }
            }

            rbtnStartSOP.Enabled = isFlag;
            rbtnCancelSOP.Enabled = isFlag;

            if (workState == WorkFlowState.RUN && HasCurrentSOPControl() == true)
                rbtnCancelSOP.Enabled = true;

            if (m_pageHome != null)
            {
                if (m_pageHome.Visible == true)
                {
                    if (m_pageHome.TabControls.IsHandleCreated)
                    {
                        if (m_pageHome.TabControls.Visible == false)
                        {
                            rbtnStartSOP.Enabled = false;
                        }
                    }
                    else
                    {
                        rbtnStartSOP.Enabled = false;
                    }
                }

                if (rbtnCancelSOP.Enabled == true)
                {
                    if (rbtnStartSOP.Enabled == true)
                    {
                        if (workState == WorkFlowState.DISABLE)
                            rbtnCancelSOP.Enabled = false;
                        else if (workState == WorkFlowState.RUN)
                            rbtnStartSOP.Enabled = false;
                    }
                    else
                    {
                        if (workState == WorkFlowState.DISABLE)
                            rbtnCancelSOP.Enabled = false;
                    }
                }

                if (rbtnStartSOP.Enabled)
                {
                    rbtnControlStatus.Visible = rbtnControlAction.Visible = false;
                }
                else
                {
                    if (GetCurrentSOPScenario() != null && work != null)
                    {
                        //rbtnControlStatus.Visible = rbtnControlAction.Visible = true;
                    }
                }
            }
        }

        private void InitRibbonButton(RibbonButton btn, int nID)
        {
            btn.Owner = this;
            SetButtonID(btn, nID);
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
            if (m_initializeComponent == false)
                return;

            InitPanels();

            if (m_pageHome != null)
            {
                foreach (SectionTabPage page in m_pageHome.TabControls.Controls)
                {
                    page.ReSizePanel();
                    GetPageHome().changeLocation(page.Height);
                }
            }
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {

        }

        private ExecuteManager m_ExeManager = new ExecuteManager();

        char szDeli = (char)0x06;
        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            if (m_netMgr.IsConnected == false)
                return;

            RibbonButton btn = (RibbonButton)sender;
            int nButtonID = GetButtonID(btn);

            switch (nButtonID)
            {
                case ID.ID_SHOW_SOP_OPTION:
                    SelectOptionTab();
                    break;
                case ID.ID_CONTROL_RETURN:
                    if (HasCurrentSOPControl())
                    //if (HasControl)
                    {
                        // 제어권 반납
                        SetControl(false);
                        OnEnabled(ID.ID_CONTROL_RETURN);
                        UpdateUserInfo(0);  //제어권 반납
                    }
                    else
                    {
                        SectionTabPage tabPage = m_pageHome.GetCurrentTabPage();

                        if (tabPage != null && tabPage.ActionStepHistoryID > 0 /*&& FormSOP.Instance.HasSOPControl(tabPage.ActionStepHistoryID) == false*/)
                        {
                            // 제어권 요청
                            //m_bRequestControl = true;
                            m_frmRequestProgress = new PopupRequestProgress(tabPage.ActionStepHistoryID);
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
                    }
                    break;
                case ID.ID_RUN_PLAY:
                    //if (HasControl == true)
                    Play();
                    break;
                case ID.ID_RUN_CANCEL:
                    if (HasCurrentSOPControl())
                        //if (HasControl == true)
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

        private void OnChangeMode()
        {
            if (!m_sopMgr.IsOpened)
                return;

            bool isRegular = true;//radioRegistMode.Checked;
            bool isNormal = m_isNormal;
            bool isReal = IsReal;

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
            if (IsReal == isReal &&
                /*radioRegistMode.Checked == isRegular &&*/
                m_isNormal == isNormal)
                return;

            SetRealModeStatus(isReal);
            m_isNormal = isNormal;
            OnChangeMode();
        }

        public bool Play()
        {
            if (rbtnStartSOP.Enabled == false)
                return false;

            //GetPageHome().ClearProcess();

            if (m_pageHome.TabControls.IsHandleCreated == false)
                return false;

            TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            if (tapPage == null || (tapPage is SectionTabPage) == false)
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

        public WorkflowOption Play(DateTime sopTime, int nSensorZoneID, int nSensorHistoryID, string strSOPFullPath, out int nActionStepID, bool isRealMode)
        {
            nActionStepID = -1;
            //isRealMode = false;

            if (rbtnStartSOP.Enabled == false)
                return null;

            //GetPageHome().ClearProcess();

            if (m_pageHome.TabControls.IsHandleCreated == false)
                return null;

            TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            if (tapPage == null || (tapPage is SectionTabPage) == false)
                return null;
            SectionTabPage page = (SectionTabPage)tapPage;
            if (page == null)
                return null;

            // 각 Section들의 CompleteCounte를 모두 초기화한다.
            InitCompleteCount(page);

            /*TreeNode node = SOPScenarioManager.Instance.GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return false;*/

            WorkflowOption option = RunWorkflowWithoutEventWithoutPosition(sopTime, nSensorZoneID, nSensorHistoryID, strSOPFullPath);

            int nActionID = page.ActionStepID;
            //if (HasControl == true)
            //WriteCurrentActionStepID(nActionID, !page.VirtualMode);

            CommandBarControlEnabled(false);

            nActionStepID = nActionID;
            page.VirtualMode = !isRealMode;
            //isRealMode = !page.VirtualMode;
            return option;
        }

        public int PlayWithDisasterPosition(SectionTabPage page, DateTime sopTime, int nZoneID, int nSensorID, int nSensorHistoryID, string strDisasterOption, bool isRealMode, string strAlarmMessage = null)
        {
            //isRealMode = true;

            if (page == null || page.ActionStepHistoryID > 0)
            //if (rbtnStartSOP.Enabled == false)
                return -1;

            //GetPageHome().ClearProcess();

            if (m_pageHome.TabControls.IsHandleCreated == false)
                return -1;

            /*TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            if (tapPage == null || (tapPage is SectionTabPage) == false)
                return -1;
            SectionTabPage page = (SectionTabPage)tapPage;
            if (page == null)
                return -1;*/

            // 각 Section들의 CompleteCounte를 모두 초기화한다.
            InitCompleteCount(page);

            /*TreeNode node = SOPScenarioManager.Instance.GetBarLevelTree().FindActionStepNode(page.ActionStepID);
            if (node == null)
                return false;*/

            // Workflow를 실행하기 전에 먼저 TabPage를 전면에 내세운다.
            m_pageHome.SelectTab(page);
            RunWorkflowWithoutEvent(sopTime, nZoneID, nSensorID, nSensorHistoryID, strDisasterOption, strAlarmMessage);

            int nActionID = page.ActionStepID;
            //if (HasControl == true)
            //WriteCurrentActionStepID(nActionID, !page.VirtualMode);

            CommandBarControlEnabled(false);

            page.VirtualMode = !isRealMode;
            //isRealMode = !page.VirtualMode;
            return nActionID;

            //if (btnStartSOP.Enabled == false)
            //    return false;

            //GetPageHome().ClearProcess();

            //TabPage tapPage = m_pageHome.TabControls.SelectedTab;
            //if (tapPage == null || (tapPage is SectionTabPage) == false)
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

        private string LoadActionStepFullPath(int nActionStepID)
        {
            string strSQL = "Select dc.CategoryName, sdc.SubCategoryName, d.DisasterName, step.StepName ";
            strSQL += "from ActionStep as step, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strSQL += "where step.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID ";
            strSQL += "and step.ID = " + nActionStepID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (nResultCount < 4)
                return null;

            string strDisasterCategoryName = WebDBManager.GetStringField(arrResult[0]);
            string strSubDisasterCategoryName = WebDBManager.GetStringField(arrResult[1]);
            string strDisasterName = WebDBManager.GetStringField(arrResult[2]);
            string strActionStepName = WebDBManager.GetStringField(arrResult[3]);

            if (strDisasterCategoryName == null || strSubDisasterCategoryName == null ||
                strDisasterName == null || strActionStepName == null)
                return null;

            return strDisasterCategoryName + "\\" + strSubDisasterCategoryName + "\\" + strDisasterName + "\\" + strActionStepName;
        }

        public void RunWorkflowWithoutEvent(DateTime sopTime, int nZoneID, int nSensorID, int nSensorHistoryID, string strDisasterOption, string strAlarmMessage)
        {
            try
            {
                TabPage page = m_pageHome.tabControl.SelectedTab;
                if (page == null)
                {
                    return;
                }

                SensorDetectSignal signal = m_netMgr.FindDetectSignal(nSensorHistoryID);
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

                    if (strAlarmMessage != null)
                        start.Option.AlarmMessage = strAlarmMessage;

                    // 화재신호로 수행된 SOP는 화재발생문자가 대부분 나간경우 이므로 시작/종료문자는 보내지 않는다.
                    start.Option.UseSmsMessage = false;
                    start.Option.PositionName = signal.PositionName;
                    //start.PositionName = signal.PositionName;
                    start.Option.DetectTime = new VariousData<DateTime>(signal.DetectTime);
                    start.OnPostProcess += new PostProcessEvent(RunWorkflowAsync);
                    start.Option.SensorZoneID = nSensorID;
                    start.Option.SensorZoneHistoryID = nSensorHistoryID;
                    start.Option.SetDisasterOptions(strDisasterOption);

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
                    string szName = node == null ? LoadActionStepFullPath(nActionStepID) : node.FullPath;

                    if (szName == null)
                        return;

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
                    //string szPositionName = zone.DisplayName;

                    string szPositionName = "";
                    EquipmentZone equipZone = DataManager.Instance.GetEquipmentZoneFromSensorZoneID(nSensorID);

                    if (equipZone != null)
                        szPositionName = equipZone.EquipZoneName;
                    else
                        szPositionName = zone.DisplayName;

                    // 센서신호로 수행된 SOP는 화재발생문자가 대부분 나간경우 이므로 시작/종료문자는 보내지 않는다.
                    start.Option.UseSmsMessage = false;
                    start.Option.PositionName = szPositionName;
                    start.Option.DetectTime = new VariousData<DateTime>(sopTime);
                    start.Option.SensorZoneID = nSensorID;
                    start.Option.SensorZoneHistoryID = nSensorHistoryID;
                    start.Option.SetDisasterOptions(strDisasterOption);

                    if (strAlarmMessage != null)
                        start.Option.AlarmMessage = strAlarmMessage;

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
            catch (Exception exx)
            {
                System.Diagnostics.Trace.WriteLine(exx.Message);
                System.Diagnostics.Trace.WriteLine(exx.StackTrace);
                int i = 0;
                i++;
            }

        }

        public WorkflowOption RunWorkflowWithoutEventWithoutPosition(DateTime sopTime, int nSensorZoneID, int nSensorZoneHistoryID, string strSOPFullPath)
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

                strSOPFullPath = strSOPFullPath.Replace('/', '\\');
                string szName = node == null ? strSOPFullPath : node.FullPath;

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
                start.Option.SensorZoneHistoryID = nSensorZoneHistoryID;
                start.Option.SensorZoneID = nSensorZoneID;

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
            SOPScenario scenario = GetCurrentSOPScenario();

            if (scenario == null)
                return;

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(scenario.ActionStepHistoryID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

            if (isControl == 0)
            {
                SetReturnSOPControl(scenario.ActionStepHistoryID, DateTime.Now);

                if (m_netMgr.SendMessage(SOPWebServer.Header.RETURN_CONTROL, bytes) == false)
                {
                    // 제어권을 양도받을 대상이 없는 경우
                    m_nReturnControlActionStepHistory = -1;
                }
            }
            else
                m_netMgr.SendMessage(SOPWebServer.Header.REQUEST_CONTROL, bytes);
        }

        private void SetReturnSOPControl(int nActionStepHistoryID, DateTime timeStamp)
        {
            m_nReturnControlActionStepHistory = nActionStepHistoryID;
            m_dtReturnControl = timeStamp;

            SetSOPControl(nActionStepHistoryID, -1);
        }

        public int GetButtonID(Button btn)
        {
            if (m_dicButtonIDs.ContainsKey(btn))
                return m_dicButtonIDs[btn];

            return -1;
        }

        #region Tab 전환

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

                //GetPageHome().ShowTranslucentForm(m_pageOption, 180, 0, 1340, 935, ID.ID_SHOW_SOP_OPTION);
                m_pageOption.StartPosition = FormStartPosition.CenterParent;
                m_pageOption.ShowDialog(this);
            }
        }

        public void PopupQuickButtonSetup()
        {
            SelectOptionTab();
            Popup.PopupQuickButtonSetup form = new Popup.PopupQuickButtonSetup();
            //GetPageHome().ShowTranslucentForm(form, 200, 50, form.Width, form.Size.Height, ID.ID_SHOW_QUICK_MENU);
            form.ShowDialog(this);


            form.FormClosed += (s, e) => { ClosedOptionSubPopup(); };
        }

        public void PopupSelectFireSensorSOPLink()
        {
            SelectOptionTab();
            Popup.PopupSelectFireSensorSOPLink form = new Popup.PopupSelectFireSensorSOPLink(FormSOP.Instance.DBManager, m_nSiteID);
            //GetPageHome().ShowTranslucentForm(form, 200, 100, form.Width, form.Size.Height, ID.ID_SHOW_FIRE_SENSOR_SOP_LINK);
            form.ShowDialog(this);

            form.FormClosed += (s, e) => { ClosedOptionSubPopup(); };
        }

        public void PopupSelectPSMSensorSOPLink()
        {
            SelectOptionTab();
            Popup.PopupSelectPSMSensorSOPLink form = new Popup.PopupSelectPSMSensorSOPLink(FormSOP.Instance.DBManager, m_nSiteID);
            //GetPageHome().ShowTranslucentForm(form, 200, 100, form.Width, form.Size.Height, ID.ID_SHOW_PSM_SENSOR_SOP_LINK);
            form.ShowDialog(this);

            form.FormClosed += (s, e) => { ClosedOptionSubPopup(); };
        }

        public void PopupSelectIntrusionSensorSOPLink()
        {
            SelectOptionTab();
            Popup.PopupSelectIntrusionSensorSOPLink form = new Popup.PopupSelectIntrusionSensorSOPLink(FormSOP.Instance.DBManager, m_nSiteID);
            //GetPageHome().ShowTranslucentForm(form, 200, 100, form.Width, form.Size.Height, ID.ID_SHOW_INTRUSION_SENSOR_SOP_LINK);
            form.ShowDialog(this);

            form.FormClosed += (s, e) => { ClosedOptionSubPopup(); };
        }

        public void PopupSelectETCSensorSOPLink()
        {
            SelectOptionTab();
            //Popup.PopupSlectETCSensorSOPLink form = new Popup.PopupSlectETCSensorSOPLink(FormSOP.Instance.DBManager, m_nSiteID);
            Popup.PopupSlectETCSensorSOPLink form = new Popup.PopupSlectETCSensorSOPLink(FormSOP.Instance.DBManager, m_nSiteID);
            //GetPageHome().ShowTranslucentForm(form, 200, 100, form.Width, form.Size.Height, ID.ID_SHOW_PSM_SENSOR_SOP_LINK);
            form.ShowDialog(this);

            form.FormClosed += (s, e) => { ClosedOptionSubPopup(); };
        }

        private void ClosedOptionSubPopup()
        {
            SelectOptionTab();
        }

        public void SelectViewTab(bool showPageHome = true)
        {
            // 옵션창은 팝업으로 연결하기 때문에 속성적용을 안함.
            //m_pageOption.Visible = false;
            m_pageHome.Visible = showPageHome;
            m_pageMessage.Visible = false;

            panelTop.Size = new Size(panelTop.Size.Width, m_nPanelTopInitHeight);

            panelMain.Location = new Point(0, panelTop.Size.Height);
            panelMain.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - panelTop.Size.Height);
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

                                SetRealModeStatus(!page.VirtualMode);
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
            return tree.Load(m_sopMgr, true/*radioRegistMode.Checked*/, m_isNormal);
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

            int nLastID = SOPScenarioManager.Instance.LastComponentHistoryID;

            if (nLastID < 0)
                m_nInitComponentHistoryID = 0;
            else
                m_nInitComponentHistoryID = nLastID;

            m_pageHome.EndHistory(m_nInitComponentHistoryID == 0);

            return result;
        }

        public int GetCurrentActionStep(out bool isRealMode)
        {
            int nActionStepID = -1;
            isRealMode = false;
            bool virtualMode = true;

            this.Invoke((MethodInvoker)delegate
            {
                SectionTabPage tabPage = m_pageHome.GetCurrentTabPage();

                if (tabPage != null)
                {
                    nActionStepID = tabPage.ActionStepID;
                    virtualMode = !tabPage.VirtualMode;
                }
            });

            isRealMode = !virtualMode;
            return nActionStepID;
        }

        // 가장 마지막에 ReadCurrentActionStep()을 호출했던 시간
        //private DateTime m_dtPrevReadCurrentActionStep = new DateTime();
        //private int m_nPrevReadActionStepID = -1;
        //private bool m_bPrevReadActionMode = false;

        public int ReadCurrentActionStep(ref bool isRealMode)
        {
            SectionTabPage currentTabPage = m_pageHome.GetCurrentTabPage();

            if (currentTabPage == null)
                return -1;

            isRealMode = !currentTabPage.VirtualMode;
            return currentTabPage.ActionStepID;

            //DateTime dtNow = DateTime.Now;
            //TimeSpan span = dtNow - m_dtPrevReadCurrentActionStep;

            //// 마지막에 호출한 이후 1초가 지나지 않았으면 지난번 읽은 값을 리턴한다.
            //if (span.TotalSeconds < 1.0)
            //{
            //    isRealMode = m_bPrevReadActionMode;
            //    return m_nPrevReadActionStepID;
            //}

            //m_dtPrevReadCurrentActionStep = dtNow;

            //string szText = "SELECT cas.ActionStepID, cas.RealMode FROM CurrentActionStep as cas " +
            //                " INNER JOIN (SELECT min(id) as minID FROM CurrentActionStep ) cas2 ON cas.id = cas2.minID AND cas.SiteID = {0}";

            ///*DateTime now = DateTime.Now;
            //string strTime = string.Format("{0:00}:{1:00}", now.Minute, now.Second);
            //System.Diagnostics.Trace.WriteLine(strTime + ", " + "ReadCurrentActionStep");*/

            //string strSQL = string.Format(szText, m_nSiteID);

            //ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            //if (arrResult == null || arrResult.Count <= 1)
            //{
            //    m_nPrevReadActionStepID = -1;
            //    return -1;
            //}

            //int nActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            //isRealMode = WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;
            //SOPManager.SetCurrentActionStep(nActionStepID, isRealMode);
            //m_bPrevReadActionMode = isRealMode;
            //m_nPrevReadActionStepID = nActionStepID;
            //return nActionStepID;
        }

        private bool LoadCompanyMember()
        {
            DockingRightPersonnel personnel = m_pageHome.GetDockPersonnel();
            return personnel.Load(m_sopMgr);
        }

        public void CloseRequestProgress(bool needInvoke = false)
        {
            if (m_frmRequestProgress != null)
            {
                if (needInvoke)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        if (m_frmRequestProgress.Visible)
                        {
                            m_frmRequestProgress.Close();
                            m_frmRequestProgress = null;
                        }
                    });
                }
                else
                {
                    if (m_frmRequestProgress.Visible)
                    {
                        m_frmRequestProgress.Close();
                        m_frmRequestProgress = null;
                    }
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

			ArrayList arrSendControl = m_dbMgr.GetResultData(strSQL);
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

        public void ShowRequestControl(int nActionStepHistoryID, string strUserID, string strUserNickName, string strIP)
        {
            if (m_frmRequestControl == null)
            {
                m_frmRequestControl = new PopupRequestControl();
                m_frmRequestControl.Show();
            }

            m_frmRequestControl.AddUser(nActionStepHistoryID, strUserID, strUserNickName, strIP);
        }

        public void HideRequestControl(int nActionStepHistoryID, string strUserID)
        {
            if (m_frmRequestControl != null && m_frmRequestControl.IsDisposed == false)
            {
                int nUserCount = m_frmRequestControl.RemoveUser(nActionStepHistoryID, strUserID);

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
            tabPage.UseWaterMark = GetPageOption().UseWatermark();
            tabPage.VirtualMode = !FormSOP.Instance.IsReal;
            WorkFlow work = manager.Add(tabPage.ActionStepID, arSections, !tabPage.VirtualMode);

            if (page is SectionTabPage)
            {
                work.SetSectionContents(((SectionTabPage)page).SectionContents);
            }
        }

        public int GetTabActionStepID(TabPage tabPage)
        {
            if (tabPage == null)
                return -1;

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
                if (start.NoPopup == false)
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

                string strActionStepName = GetActionStepName(start.ActionStepID);
                string strFullPath = start.SOPName.EndsWith("\\" + strActionStepName) ? start.CategoryName + "/" + start.SOPName : start.CategoryName + "/" + start.SOPName + "/" + strActionStepName;

                SectionTabPage tabPage = m_pageHome.GetTabPage(start.ActionStepID, !start.VirtualMode);

                if (tabPage != null)
                    m_pageHome.SelectTabPage(tabPage);

                WorkFlow work = RunWorkflow(start.Option, strFullPath);
                System.Diagnostics.Trace.WriteLine("FormSOP.work success");
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
                    if (arShlters != null)
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

        private string GetActionStepName(int nActionStepID)
        {
            string strSQL = "Select StepName from ActionStep where ID = " + nActionStepID.ToString();
            ArrayList arrResult = this.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return "";

            string strActionStepName = WebDBManager.GetStringField(arrResult[0]);
            return strActionStepName;
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

                if (arShlters != null)
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

        public WorkFlow RunWorkflow(SectionTabPage page, WorkflowOption option, string strFullPath)
        {
            PageBackstageSOP pageHome = GetPageHome();
            pageHome.SelectTabPage(page);

            return RunWorkflow(option, strFullPath);
        }

        public WorkFlow RunWorkflow(WorkflowOption option = null, string strFullPath = null)
        // psmDistance : 미터
        //public WorkFlow RunWorkflow(bool bSendSMS = false, VariousData<DateTime> dtDetect = null, string strPosition = null, string strBroadcastPositionName = null, string strPSMMaterialName = null, VariousData<int> psmDistance = null, string strAmountSnowfall = null)
        {
            PageBackstageSOP pageHome = GetPageHome();
            SectionTabPage page = (SectionTabPage)pageHome.tabControl.SelectedTab;
            if (page == null)
                return null;

            pageHome.SetCurrentWorkflowOption(option);
            bool bReal = FormSOP.Instance.IsReal;



            WorkFlowManager manager = WorkFlowManager.Instance;
            WorkFlow work = (WorkFlow)manager.Get(page.ActionStepID, !page.VirtualMode);
            if (work != null)
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

            //if (HasControl == true)
            //WriteCurrentActionStepID(ActionID, !page.VirtualMode);

            page.ActionStepID = ActionID;
            TabPageManager.Instance.SetUsePage(ActionID, true, !page.VirtualMode);

            BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();
            TreeNode node = tree.FindActionStepNode(ActionID);

            if (strFullPath != null)
                strFullPath = strFullPath.Replace('/', '\\');

            string szPath = node == null ? strFullPath : node.FullPath;
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
            if (option.DetectTime != null/* && HasControl == true*/)
            {
                work.BeginEndEventSendSMS = option.UseSmsMessage;
                //work.DetectTime = dtDetect.Data;
            }

            if (work != null)
            {
                int nActionStepHistoryID = FormSOP.Instance.SOPManager.GetActionStepHistoryID(ActionID, !page.VirtualMode);
                page.ActionStepHistoryID = nActionStepHistoryID;
                work.ActionStepHistoryID = nActionStepHistoryID;

                SOPScenario scenario = SOPScenarioManager.Instance.AddSOPScenario(szPath.Replace("\\", szDeli.ToString()), page.ActionStepID, !page.VirtualMode, page.IsNormal, page.ActionStepHistoryID, nSensorZoneHistoryID, page);

                if (work.Start())
                {
                    pageHome.StartComponentContents(page.ActionStepID, !page.VirtualMode, option, false);
                    //pageHome.StartComponentContents(page.ActionStepID, !page.VirtualMode, dtDetect, strPosition, strBroadcastPositionName, false, strPSMMaterialName, psmDistance, strAmountSnowfall);

                    List<PanelSection> panels = page.GetPanelSections();
                    foreach (PanelSectionEx pane in panels)
                    {
                        pane.HideBeginSectionButton();
                        //pane.HideAllSectionButtons();

                        string szName = UnE.SOP.ProxySOP.Instance.SiteName;
                        if (option.HasPosition == true)
                        {
                            szName = option.PositionName;
                            //szName = strPosition;                            
                        }

                        pane.SetWorkflowOption(option);
                        /*PSMMaterial material = null;

                        if (option is WorkflowOptionPSM)
                        {
                            WorkflowOptionPSM optionPSM = (WorkflowOptionPSM)option;
                            material = optionPSM.PSMMaterial;
                        }

                        if (material != null)
                            pane.SetInfoText(szName, option.DetectTime.Data.ToString(), material.MaterialName);
                        else
                            pane.SetInfoText(szName, option.DetectTime.Data.ToString());*/
                    }
                }
                else
                {
                    if (scenario != null)
                    {
                        SOPScenarioManager.Instance.RemoveScenario(scenario.ActionStepID, scenario.RealMode);
                    }
                }
            }

            SetCurrentWorkflow(work);

            FormSOP.Instance.SelectViewTab(true);
            EnabledRunGroup();

            m_pageHome.toolstripSetting("");

            //Thread.Sleep(200);

            /*int nActionStepHistoryID = FormSOP.Instance.SOPManager.GetActionStepHistoryID(ActionID, !page.VirtualMode);
            page.ActionStepHistoryID = nActionStepHistoryID;
            SOPScenarioManager.Instance.AddSOPScenario(szPath.Replace("\\", szDeli.ToString()), page.ActionStepID, !page.VirtualMode, page.ActionStepHistoryID, nSensorZoneHistoryID);*/

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
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return ShowDisconnectedDB();

            if (arrResult.Count != 4)
                return ShowAlreadyRemovedVersion(tabPage);

            // 현재 열려있는 버전보다 더 새로운 버전이 나오지 않았는지 확인한다.
            VariousData<int> nDisasterID = WebDBManager.GetIntField(arrResult[0].ToString());
            string strDisasterName = WebDBManager.GetStringField(arrResult[1]);
            VariousData<int> nSubDisasterID = WebDBManager.GetIntField(arrResult[2].ToString());
            VariousData<DateTime> dtLastAccessed = WebDBManager.GetDateTimeField(arrResult[3]);

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

            string strAccessedTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                version.LastAccessedTime.Year, version.LastAccessedTime.Month, version.LastAccessedTime.Day,
                version.LastAccessedTime.Hour, version.LastAccessedTime.Minute, version.LastAccessedTime.Second);

            string strSQL = string.Format("select Disaster.ID, VersionID from Disaster, Version where VersionID = Version.ID and Version.SiteID = {0} and Version.IsRegular = {1} and Version.IsNormal = {2} and DisasterName = '{3}' and SubDisasterID = {4} and Version.LastAccessTime > '{5}'",
                m_nSiteID, version.IsRegular ? 1 : 0, version.IsNormal ? 1 : 0, strDisasterName, nSubDisasterID, strAccessedTime);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count != 2)
                return 1;

            VariousData<int> nNewDisaster = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> nNewVersion = WebDBManager.GetIntField(arrResult[1].ToString());

            if (nNewDisaster == null && nNewVersion == null)
                return 1;

            nNewDisasterID = nNewDisaster.Data;
            nNewVersionID = nNewVersion.Data;
            return 0;
        }

        public DisasterInfo ReloadDisaster(int nActionStepID)
        {
            string strSQL = "Select d.ID, d.VersionID, d.DisasterName from ActionStep as _as, Disaster as d, Version as v where _as.DisasterID = d.ID and d.VersionID = v.ID and _as.ID = " + nActionStepID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 3)
                return null;

            VariousData<int> disasterID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> versionID = WebDBManager.GetIntField(arrResult[1].ToString());
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
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 12)
                return false;

            string strDisasterName = WebDBManager.GetStringField(arrResult[0]);
            VariousData<int> nSubDisasterID = WebDBManager.GetIntField(arrResult[1].ToString());
            VariousData<int> nRegular = WebDBManager.GetIntField(arrResult[2].ToString());
            VariousData<int> nNormal = WebDBManager.GetIntField(arrResult[3].ToString());
            VariousData<DateTime> createTime = WebDBManager.GetDateTimeField(arrResult[4]);
            VariousData<DateTime> lastAccessedTime = WebDBManager.GetDateTimeField(arrResult[5]);
            string strVersionName = WebDBManager.GetStringField(arrResult[6]);
            VariousData<int> nOwnerID = WebDBManager.GetIntField(arrResult[7].ToString());
            VariousData<int> nMemberID = WebDBManager.GetIntField(arrResult[8].ToString());
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
                arrResult = m_dbMgr.GetResultData(strSQL);

                if (arrResult == null || arrResult.Count != 1)
                    return false;

                strUserName = WebDBManager.GetStringField(arrResult[0]);

                if (strUserName == null)
                    return false;
            }

            disaster = new DisasterInfo();

            disaster.DisasterID = nDisasterID;
            disaster.VersionID = nVersionID;
            disaster.DisasterName = strDisasterName;
            disaster.SubDisasterCategoryName = strSubCategoryName;
            disaster.DisasterCategoryName = strCategoryName;

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

            SetCloseActionStepHistory(work.ActionStepHistoryID, page.SensorZoneHistoryID);

            bool hasControl = HasSOPControl(page.ActionStepHistoryID);

            if (hasControl)
            //if (this.HasControl == true)
            {
                if (m_smsExternalCompanyMemberOn)
                {
                    // 협력업체 직원들의 전화번호 추가
                    AddExternalCompanyMemberPhoneNumbers(arrListCall);
                }
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

            if (hasControl)
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
                if (diasterForm != null && work.Option.LastPosition != null)
                {
                    diasterForm.LastPos = work.Option.LastPosition;
                    diasterForm.RemoveDisasterPos();
                    work.Option.LastPosition = null;
                    diasterForm.LastPos = null;
                }
            }

            if (hasControl)
            {
                ProcessSectionManager.Instance.AddFirst(endEvent);
            }
            HistoryManager2.Instance.RemoveHistoryDisasterPosition(page.ActionStepID, !page.VirtualMode);
            HistoryManager2.Instance.RemoveHistoryDisasterNoPosition(page.ActionStepID, !page.VirtualMode);

            //if (hasControl)
            //    WriteCurrentActionStepID(-1, false);

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
                    SetCloseActionStepHistory(page.ActionStepHistoryID, page.SensorZoneHistoryID);
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

        private void SetCloseActionStepHistory(int nActionStepHistoryID, int nSensorZoneHistoryID)
        {
            StubWorker.Instance.CloseSensorZoneHistory(nSensorZoneHistoryID);
            m_dicCloseActionStepHistoryIDs[nActionStepHistoryID] = DateTime.Now;
        }

        public void StopWorkflow(DateTime dtStop, bool noDBWrite, int nActionStepID, bool isRealMode)
        {
            WorkFlowManager manager = WorkFlowManager.Instance;
            PageBackstageSOP pageHome = GetPageHome();
            SectionTabPage removeTabPage = null;

            WorkFlow work = manager.Get(nActionStepID, isRealMode);

            if (work == null)
                return;

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

                    removeTabPage = page;
                    break;
                }
            }

            if (removeTabPage != null)
            {
                SetCloseActionStepHistory(removeTabPage.ActionStepHistoryID, removeTabPage.SensorZoneHistoryID);
            }
            else
            {
                // 현재 선택된 SOP 시나리오가 아니어서 page를 얻어오지 못했다.
                // 활성화되지 않은 시나리오를 삭제하도록 한다.
                int nSensorZoneHistoryID = work.Option != null ? work.Option.SensorZoneHistoryID : -1;
                pageHome.CloseSOPScenario(work.ActionStepHistoryID);
                
                if (work != null)
                {
                    if (work.State == WorkFlowState.RUN)
                        work.Stop(dtStop, noDBWrite);
                    else
                    {
                        WorkFlowManager.Instance.Remove(nActionStepID, isRealMode);
                        SOPScenarioManager.Instance.RemoveScenario(work.ActionStepHistoryID);
                    }

                    SetCloseActionStepHistory(work.ActionStepHistoryID, nSensorZoneHistoryID);

                    if (work.Option.HasPosition == true)
                    {
                        if (work.Option.LastPosition != null)
                        {
                            ProxyMessenger.Instance.SetLastPosition(work.Option.LastPosition.DisasterName, work.Option.LastPosition.PoistionName, work.Option.LastPosition.BroadcastName, work.Option.LastPosition.BuildingID, work.Option.LastPosition.FloorIndex, work.Option.LastPosition.HistoryActionStepID, work.Option.LastPosition.IconID, work.Option.LastPosition.PSMDistance, work.Option.LastPosition.PSMMaterial, work.Option.LastPosition.X, work.Option.LastPosition.Y, work.Option.LastPosition.Z, work.Option.LastPosition.ZoneID);
                            ProxyMessenger.Instance.RemoveDisasterPos();
                            work.Option.LastPosition = null;
                            ProxyMessenger.Instance.NullLastPosition();
                        }
                    }
                }

                HistoryManager2.Instance.RemoveHistoryDisasterPosition(nActionStepID, isRealMode);
                HistoryManager2.Instance.RemoveHistoryDisasterNoPosition(nActionStepID, isRealMode);
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
                SetCloseActionStepHistory(work.ActionStepHistoryID, page.SensorZoneHistoryID);

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

            HistoryManager2.Instance.RemoveHistoryDisasterPosition(page.ActionStepID, !page.VirtualMode);
            HistoryManager2.Instance.RemoveHistoryDisasterNoPosition(page.ActionStepID, !page.VirtualMode);

            /*if (HasSOPControl(page.ActionStepHistoryID))
                //if (HasControl == true)
                WriteCurrentActionStepID(-1, false);*/

            if (work == null)
                return;

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
                SetCurrentWorkflow(work);
            }
            else
            {
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
                }
                else
                {
                }
                SetCurrentWorkflow(work);
                EnabledRunGroup();
            }
            HistoryManager2.Instance.HistoryDisasterPosition.Clear();
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

   //     public void WriteCurrentActionStepID(int nActionStepID, bool isRealMode)
   //     {
   //         /*if (!HasControl)
			//	return;

			//SOPManager.SetCurrentActionStep(nActionStepID, isRealMode);

   //         int nCurrentID = -1;
   //         string strSQL = string.Format("SELECT id FROM CurrentActionStep WHERE id = (SELECT min(id) FROM CurrentActionStep WHERE SiteID = {0})", m_nSiteID);
   //         ArrayList arResult = DBManager.GetResultData(strSQL);
   //         if( arResult == null || arResult.Count == 0 )
   //         {
   //             strSQL = string.Format("INSERT INTO CurrentActionStep (id, ActionStepID, RealMode, SiteID) VALUES ( 1, {0} , {1}, {2})", nActionStepID, isRealMode ? 1 : 0, m_nSiteID);
   //             DBManager.GetResultData(strSQL);
   //             nCurrentID = 1;
   //         }
   //         else
   //         {
   //             nCurrentID = WebDBManager.GetIntField(arResult[0].ToString(), -1);
   //         }

   //         if (nCurrentID > -1)
   //         {
   //             strSQL = string.Format("Update CurrentActionStep set ActionStepID = {0}, RealMode = {1} where id = {2}", nActionStepID, isRealMode ? 1 : 0, nCurrentID);
   //             DBManager.GetResultData(strSQL);
   //         }*/
   //     }

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
                if (vInfo.IsNormal == m_isNormal)
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
                m_isNormal = true;
            }
            else
            {
                m_isNormal = false;
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
                        //WriteCurrentActionStepID(aInfo.ActionStepID, isRealMode);
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
                SOPScenarioManager.Instance.AddSOPScenario(node.FullPath.Replace("\\", szDeli.ToString()), page.ActionStepID, !page.VirtualMode, page.IsNormal, page.ActionStepHistoryID, nSensorZoneHistoryID, page);
            }
        }

        public void VirtualMode(bool bRun)
        {
            if (bRun == false)
            {
                SetRealModeStatus(true);
            }
            else
            {
                SetRealModeStatus(false);
            }
        }

        public void EnableOptions(bool enabled)
        {
            rbtnCheckRealMode.Enabled = rbtnCheckVirtualMode.Enabled = enabled;
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

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 7)
                return false;

            strCategoryName = WebDBManager.GetStringField(arrResult[0]);
            strSubCategoryName = WebDBManager.GetStringField(arrResult[1]);
            strDisasterName = WebDBManager.GetStringField(arrResult[2]);
            strActionStepName = WebDBManager.GetStringField(arrResult[3]);
            VariousData<int> categoryID = WebDBManager.GetIntField(arrResult[4].ToString());
            VariousData<int> subCategoryID = WebDBManager.GetIntField(arrResult[5].ToString());
            VariousData<int> disasterID = WebDBManager.GetIntField(arrResult[6].ToString());

            if (strCategoryName == null || strSubCategoryName == null || strDisasterName == null || strActionStepName == null ||
                categoryID == null || subCategoryID == null || disasterID == null)
                return false;

            nCategoryID = categoryID.Data;
            nSubCategoryID = subCategoryID.Data;
            nDisasterID = disasterID.Data;

            return true;
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

            FormSOP.Instance.CloseThread = true;
            HistoryManager2.Instance.Dispose();

            StopWriteDB();

            if (m_dbMgr is SimulationDBManager)
                ((SimulationDBManager)m_dbMgr).CloseLocalDB();

            m_netMgr.ReleaseThread();

            ProcessSectionManager.Instance.Dispose();
            TTSManager.Instance.Dispose();

            if (m_pageOption != null)
                m_pageOption.Dispose();
            if (m_pageMessage != null)
                m_pageMessage.Dispose();
            if (m_pageHome != null)
                m_pageHome.Dispose();


            if (m_sopMonitor != null)
            {
                m_sopMonitor.Stop();
            }

            Thread.Sleep(200);
            //MainFrame.Close();
        }

        public ArrayList GetLevelMember(int nLevelID)
        {
            ArrayList arrSOPMember = new ArrayList();
            string strSQL = "select ID, MemberName from CompanyMember where LevelID in (" + nLevelID.ToString() + ")";
            //string strSQL = "select ID, MemberName, LevelID from CompanyMember where LevelID in (select id from JobLevel where LevelNo = " + nTeamID.ToString() + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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
            if (GetPageHome() != null)
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
                    HistoryManager2.Instance.AddActionStepHistory(workflow.ActionStepHistoryID, args.ActionStepID, args.RealMode, args.State, args.Time, args.NoDBWrite, bSendSMS);
                    m_pageHome.OnCloseWorkFlow(workflow.ActionStepHistoryID, args.ActionStepID, args.RealMode, args.State);
                }

                {
                    int nActionStepHistoryID = workflow.ActionStepHistoryID;
                    //int nActionStepHistoryID = FormSOP.Instance.SOPManager.GetActionStepHistoryID(args.ActionStepID, args.RealMode);

                    try
                    {
                        m_sopMonitor.RemoveSOP(nActionStepHistoryID);
                        //SupervisorSOPClose.SupervisorSOPRemoveSOP(nActionStepHistoryID);
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
                HistoryManager2.Instance.AddActionStepHistory(workflow.ActionStepHistoryID, args.ActionStepID, args.RealMode, args.State, args.Time, args.NoDBWrite, bSendSMS);
                FormSOP.Instance.DoneWorkflow();
                m_pageHome.OnCloseWorkFlow(workflow.ActionStepHistoryID, args.ActionStepID, args.RealMode, args.State);

                {
                    int nActionStepHistoryID = workflow.ActionStepHistoryID;
                    //int nActionStepHistoryID = FormSOP.Instance.SOPManager.GetActionStepHistoryID(args.ActionStepID, args.RealMode);

                    SectionTabPage page = m_pageHome.GetTabPage(nActionStepHistoryID);

                    if (page != null)
                    {
                        SOPScenarioManager.Instance.FinishActionStepHistory(nActionStepHistoryID, args.Time);
                        page.Mode = SectionTabPage.SOPMode.Done;
                    }

                    try
                    {
                        m_sopMonitor.RemoveSOP(nActionStepHistoryID);
                        //SupervisorSOPClose.SupervisorSOPRemoveSOP(nActionStepHistoryID);
                        //SDMS.ScriptProxy.Instance.UserObject.SupervisorSOPRemoveSOP.Invoke(nActionStepHistoryID);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            if (args.State == WorkFlowState.RUN)
            {
                //if (m_netMgr.SendNewSOP(args.ActionStepID, args.RealMode, m_nSOPGenUserID))
                {
                    UnE.SOP.Workstate.WorkFlow workflow = (UnE.SOP.Workstate.WorkFlow)sender;
                    Section selectedSection = null;

                    // Server로부터의 Confirm 대기
                    /*SOPConfirmData waitConfirm = new SOPConfirmData(args.ActionStepID, args.RealMode, m_nSOPGenUserID, workflow);
                    m_sopWaitConfirmDatas[waitConfirm] = waitConfirm;

                    Thread t = new Thread(new ParameterizedThreadStart(WaitSOPConfirmThread));
                    t.Start(waitConfirm);*/

                    if (workflow.SelectedSectionState != null && workflow.SelectedSectionState.SectionContents != null)
                        selectedSection = workflow.SelectedSectionState.SectionContents.Section;

                    bool bSendSMS = workflow.BeginEndEventSendSMS;

                    // 새로운 Workflow가 실행되었음을 DB에 기록한다.
                    HistoryManager2.Instance.AddActionStepHistory(workflow.ActionStepHistoryID, args.ActionStepID, args.RealMode, args.State, args.Time, args.NoDBWrite, selectedSection, bSendSMS);

                    //if (workflow.State != WorkFlowState.RUN)
                    {
                        try
                        {
                            int nSensorID = workflow.Option != null ? workflow.Option.SensorZoneID : -1;
                            m_nLastSensorZoneID = nSensorID;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLine(ex.Message);
                            System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        }
                    }
                }
            }
        }

        // SOP를 실행시킨후 Server로부터 Confirm을 기다리는 쓰레드
        /*private void WaitSOPConfirmThread(object arg)
        {
            SOPConfirmData data = (SOPConfirmData)arg;

            // Server로부터의 응답을 최대 5초동안 기다린다.
            int nLimit = 50;

            for (int i = 0; i < nLimit; i++)
            {
                if (data.Confirm)
                {
                    SOPConfirmData temp;
                    m_sopWaitConfirmDatas.TryRemove(data, out temp);

                    if (data.WorkFlow != null && data.WorkFlow.ActionStepHistoryID > 0)
                    {
                        m_dicSOPControls[data.WorkFlow.ActionStepHistoryID] = true;
                        m_netMgr.SendConfirmSOPControl(data.WorkFlow.ActionStepHistoryID);
                    }

                    return;
                }

                Thread.Sleep(100);
            }

            if (data.WorkFlow != null)
            {
                // 일정시간동안 서버로부터 Confirm을 받지 못한 SOP는 즉시 중지시킨다.
                SOPConfirmData temp;
                m_sopWaitConfirmDatas.TryRemove(data, out temp);

                if (data.WorkFlow.ActionStepHistoryID > 0)
                {
                    SectionTabPage page = m_pageHome.GetTabPage(data.WorkFlow.ActionStepHistoryID);

                    if (page != null)
                        StopWorkflow(DateTime.Now, false, page);
                }
            }
        }*/

        private int m_nLastSensorZoneID = -1;
        internal void OnNewActionStepHistory(SOPScenario sco)
        {
            try
            {
                if (sco == null)
                    return;

                int nActionStepHistoryID = sco.ActionStepHistoryID;
                UnE.SOP.Workstate.WorkFlow work = WorkFlowManager.Instance.Get(sco.ActionStepID, sco.RealMode);

                if (work != null)
                    work.ActionStepHistoryID = nActionStepHistoryID;

                if (work != null && work.Option != null)
                {
                    int nSensorZoneID = work.Option.SensorZoneID;
                    int nSensorHistoryID = work.Option.SensorZoneHistoryID;

                    m_sopMonitor.AddSOP(nActionStepHistoryID, nSensorZoneID, nSensorHistoryID);
                    //SupervisorSOPClose.SupervisorSOPAddSOP(nActionStepHistoryID, nSensorZoneID, nSensorHistoryID);
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
            ISectionContents contents = m_pageHome.GetComponentContents(section);

            if (contents != null)
            {
                contents.State = state;

                if (state == State.INPUT)
                    m_pageHome.SelectComponentContents(contents);
            }
        }

        public void OnLoadScenario(SOPScenario sopSc)
        {
            if (m_pageHome == null)
                return;

            SectionTabPage page = null;

            if (sopSc.ActionStepHistoryID > 0)
                page = m_pageHome.GetTabPage(sopSc.ActionStepHistoryID);
            else
                page = m_pageHome.GetTabPage(sopSc.ActionStepID, sopSc.RealMode);

            SOPScenarioManager.Instance.AddSOPScenario(sopSc, page);
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
                    {
                        SectionTabPage tabPage = m_pageHome.GetCurrentTabPage();

                        if (tabPage != null && tabPage.ActionStepHistoryID > 0 && FormSOP.Instance.HasSOPControl(tabPage.ActionStepHistoryID) == false)
                        {
                            //제어권 요청
                            //m_bRequestControl = true;
                            m_frmRequestProgress = new PopupRequestProgress(tabPage.ActionStepHistoryID);
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
                    }
                    break;
            }
        }

        bool prevStart = false;
        bool prevPause = false;
        bool prevDayLight = false;

        protected virtual void OnTimer()
        {
            m_pageHome.RunUnprocessedExternalMissions();

            CheckNewSOP();
            //CheckSOPControl();
            SetPageMode(m_pageHome.GetCurrentTabPage());

            rbtnControlAction.Enabled = m_netMgr != null && m_netMgr.IsConnected;

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
            //labelDate.Text = string.Format("{0}년 {1}월 {2}일", dtNow.Year, dtNow.Month, dtNow.Day);
            //labelTime.Text = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);

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

            // 서버에 따로 질의하지 않아도 서버가 알아서 보내준다.
            m_netMgr.SendQueryComponentHistory();

            // 현재 화면에 나타난 페이지에 대한 설정이 제대로 이루어졌는지 확인한다.
            m_pageHome.CheckCurrentPage();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            OnTimer();
        }

        public void SetPageMode(SectionTabPage page)
        {
            if (page == null)
                EmptySOP();
            else
            {
                m_pageHome.CheckSectionPanelSize();

                if (page.Mode == SectionTabPage.SOPMode.Control)
                {
                    // 서버와 접속이 끊어지면 모든 제어권을 지운다.
                    if (m_netMgr != null && m_netMgr.IsConnected == false)
                    {
                        page.Mode = SectionTabPage.SOPMode.Monitoring;
                    }
                }

                if (page.Mode == SectionTabPage.SOPMode.Monitoring)
                    SetMonitoringMode(page);
                else if (page.Mode == SectionTabPage.SOPMode.Control)
                    SetControlMode(page);
                else if (page.Mode == SectionTabPage.SOPMode.Wait_Self || page.Mode == SectionTabPage.SOPMode.Done)
                    SetWaitSelfMode(page);
                else if (page.Mode == SectionTabPage.SOPMode.Wait_Server)
                    SetWaitServerMode(page);

                if (page.Mode == SectionTabPage.SOPMode.Wait_Self && page.ActionStepHistoryID < 0 && m_sopUser != null)
                {
                    SensorAlarmData sensorAlarm = m_netMgr.GetSensorAlarmData(page.SensorZoneHistoryID);

                    if (sensorAlarm != null && sensorAlarm.SOPGenUserID == m_sopUser.ID)
                    {
                        // Server로부터 SOP 실행권한을 받았는데 아직 SOP를 실행하지 못하고 있는 상태
                        sensorAlarm.Page = page;
                    }
                }
            }
        }

        private void SetEndPointMode(bool blinkBegin, bool blinkEnd, SectionTabPage page)
        {
            List<PanelSection> panels = page.GetPanelSections();

            if (panels == null || panels.Count == 0)
                return;

            PanelSection panel = panels[0];

            foreach (ButtonEndPoint btn in panel.Buttons)
            {
                SectionDataEndPoint data = (SectionDataEndPoint)btn.Data;

                if (data.IsBegin)
                    btn.Shape.DisableNotifier = !blinkBegin;
                else
                    btn.Shape.DisableNotifier = !blinkEnd;
            }
        }

        private void SetMonitoringMode(SectionTabPage page)
        {
            SetControlText(false);
            rbtnControlStatus.Enabled = rbtnControlAction.Enabled = true;
            //rbtnControlStatus.Visible = rbtnControlAction.Visible = true;

            rbtnStartSOP.Enabled = rbtnCancelSOP.Enabled = false;
            rbtnStartSOP.Visible = rbtnCancelSOP.Visible = true;

            SetRealModeButton(!page.VirtualMode);
            rbtnCheckRealMode.Visible = rbtnCheckVirtualMode.Visible = labelRealMode.Visible = labelVirtualMode.Visible = false;
            rbtnRealMode.Visible = true;

            if (GetSectionContentsEnabled(page))
                m_pageHome.OnEnabled(false);

            SetEndPointMode(false, true, page);
        }

        private void SetControlMode(SectionTabPage page)
        {
            SetControlText(true);
            rbtnControlStatus.Enabled = rbtnControlAction.Enabled = true;
            //rbtnControlStatus.Visible = rbtnControlAction.Visible = true;

            rbtnStartSOP.Enabled = false;
            rbtnCancelSOP.Enabled = true;
            rbtnStartSOP.Visible = rbtnCancelSOP.Visible = true;

            SetRealModeButton(!page.VirtualMode);
            rbtnCheckRealMode.Visible = rbtnCheckVirtualMode.Visible = labelRealMode.Visible = labelVirtualMode.Visible = false;
            rbtnRealMode.Visible = true;

            if (GetSectionContentsEnabled(page) == false)
                m_pageHome.OnEnabled(true);

            SetEndPointMode(false, true, page);
        }

        private void SetWaitSelfMode(SectionTabPage page)
        {
            rbtnControlStatus.Visible = rbtnControlAction.Visible = false;

            rbtnStartSOP.Enabled = true;
            rbtnCancelSOP.Enabled = false;
            rbtnStartSOP.Visible = rbtnCancelSOP.Visible = true;

            panelSOPMode.Visible = true;
            SetRealModeButton(!page.VirtualMode);
            rbtnCheckRealMode.Visible = rbtnCheckVirtualMode.Visible = labelRealMode.Visible = labelVirtualMode.Visible = true;
            rbtnRealMode.Visible = true;

            if (GetSectionContentsEnabled(page))
                m_pageHome.OnEnabled(false);

            SetEndPointMode(true, false, page);
        }

        private void SetWaitServerMode(SectionTabPage page)
        {
            rbtnControlStatus.Visible = rbtnControlAction.Visible = false;

            rbtnStartSOP.Enabled = rbtnCancelSOP.Enabled = false;
            rbtnStartSOP.Visible = rbtnCancelSOP.Visible = true;

            SetRealModeButton(!page.VirtualMode);
            rbtnCheckRealMode.Visible = rbtnCheckVirtualMode.Visible = labelRealMode.Visible = labelVirtualMode.Visible = false;
            rbtnRealMode.Visible = true;

            if (GetSectionContentsEnabled(page))
                m_pageHome.OnEnabled(false);

            SetEndPointMode(false, true, page);
        }

        private bool ContainsActionStepHistory(List<Data_ActionStepHistory> actionStepHistories, int nActionStepHistoryID)
        {
            foreach (Data_ActionStepHistory actionStepHistory in actionStepHistories)
            {
                if (actionStepHistory.ID == nActionStepHistoryID)
                    return true;
            }

            return false;
        }

        // 새로 실행시켜야 할 ActionStepHistory 가운데 이미 예전에 종료되었던 것들이 있는지 확인한다.
        private void RemoveCloseActionStepIDs(List<int> nullActionStepHistoryIDs)
        {
            string strActionStepHistoryIDs = "";

            foreach (int nActionStepHistoryID in nullActionStepHistoryIDs)
            {
                if (m_dicCloseActionStepHistoryIDs.ContainsKey(nActionStepHistoryID) || m_dicOldActionStepHistorys.ContainsKey(nActionStepHistoryID))
                {
                    if (strActionStepHistoryIDs.Length == 0)
                        strActionStepHistoryIDs = nActionStepHistoryID.ToString();
                    else
                        strActionStepHistoryIDs += ", " + nActionStepHistoryID.ToString();
                }
            }

            if (strActionStepHistoryIDs.Length == 0)
                return;

            string strSQL = "Select ID from ActionStepHistory where EndTime is NULL and CancelTime is NULL and ID in (" + strActionStepHistoryIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id != null)
                {
                    m_dicCloseActionStepHistoryIDs.Remove(id.Data);
                    m_dicOldActionStepHistorys.Remove(id.Data);
                }
            }
        }

        private void CheckNewSOP()
        {
            // 새로운 ActionStepHistory를 확인한다.
            List<Data_ActionStepHistory> newHistories = SOPScenarioManager.Instance.GetNewActionStepHistory(m_dicOldActionStepHistorys);
            List<int> nullActionStepHistoryIDs = m_dicNullActionStepHistoryIDs.Keys.ToList();

            RemoveCloseActionStepIDs(nullActionStepHistoryIDs);

            foreach (int nActionStepHistoryID in nullActionStepHistoryIDs)
            {
                // 이미 종료된 SOP 인가?
                if (m_dicCloseActionStepHistoryIDs.ContainsKey(nActionStepHistoryID))
                    continue;

                if (m_dicOldActionStepHistorys.ContainsKey(nActionStepHistoryID))
                    continue;

                if (ContainsActionStepHistory(newHistories, nActionStepHistoryID))
                    continue;

                Data_ActionStepHistory actionStepHistory = SOPScenarioManager.Instance.GetActionStepHistory(nActionStepHistoryID);
                newHistories.Add(actionStepHistory);
            }

            m_dicNullActionStepHistoryIDs.Clear();

            foreach (Data_ActionStepHistory actionStepHistory in newHistories)
            {
                if (m_pageHome.GetCurrentTabPage() != null)
                    m_pageHome.NoChangeSelectedTabPage = true;

                m_dicOldActionStepHistorys[actionStepHistory.ID] = actionStepHistory;

                SectionTabPage page = m_pageHome.GetTabPage(actionStepHistory);

                if (page == null)
                {
                    SOPScenarioManager.Instance.LoadActionStepHistory(actionStepHistory);
                    page = m_pageHome.GetTabPage(actionStepHistory.ID);
                }

                if (page != null)
                {
                    WorkFlow work = WorkFlowManager.Instance.Get(actionStepHistory.ActionStepID, actionStepHistory.RealMode);

                    if (work != null)
                    {
                        if (work.ControlUserID == m_nSOPGenUserID)
                            page.Mode = SectionTabPage.SOPMode.Control;
                        else
                            page.Mode = SectionTabPage.SOPMode.Monitoring;
                    }
                    else
                        page.Mode = SectionTabPage.SOPMode.Wait_Server;
                }

                m_pageHome.NoChangeSelectedTabPage = false;
            }

            // 새로운 ComponentHistory를 확인한다.
            List<Data_ActionStepHistory> runningHistories = SOPScenarioManager.Instance.GetRunningActionStepHistories();

            foreach (Data_ActionStepHistory actionStepHistory in runningHistories)
            {
                if (m_dicCloseActionStepHistoryIDs.ContainsKey(actionStepHistory.ID) || actionStepHistory.EndTime != null || actionStepHistory.CancelTime != null)
                    continue;

                if (m_pageHome.GetCurrentTabPage() != null)
                    m_pageHome.NoChangeSelectedTabPage = true;

                List<Data_ComponentHistory> componentHistories = SOPScenarioManager.Instance.PopNewComponentHistory(actionStepHistory.ID);
                WorkFlow workflow = WorkFlowManager.Instance.Get(actionStepHistory.ID);

                if (componentHistories.Count > 0 || workflow == null)
                {
                    if (SOPScenarioManager.Instance.LoadComponentHistory(actionStepHistory.ID, actionStepHistory.ActionStepID, actionStepHistory.RealMode, componentHistories))
                    {
                        // 이미 실행되고 있는 SOP의 재난위치와 시각을 표시한다.
                        //WorkFlow workflow = WorkFlowManager.Instance.Get(actionStepHistory.ID);
                        HistoryDisasterPosition pos = HistoryManager2.Instance.FindHistoryDisasterPosition(actionStepHistory.ActionStepID, actionStepHistory.RealMode);

                        workflow = WorkFlowManager.Instance.Get(actionStepHistory.ID);
                        WorkflowOption option = workflow == null ? null : workflow.Option;

                        if (pos == null)
                        {
                            m_pageHome.StartComponentContents(actionStepHistory.ActionStepID, actionStepHistory.RealMode, option, true);
                        }
                        else
                        {
                            m_pageHome.StartComponentContents(actionStepHistory.ActionStepID, actionStepHistory.RealMode, option, true);
                        }

                        // 이미 실행되고 있는 SOP의 재난위치와 시각을 표시한다.
                        //WorkFlow workflow = FormSOP.Instance.CurrentWork;
                        if (workflow != null)
                        {
                            bool bUseSMS = ((actionStepHistory.StartOption & 1) == 1 ? true : false);
                            workflow.BeginEndEventSendSMS = bUseSMS;

                            SectionTabPage tabPage = m_pageHome.GetTabPage(actionStepHistory.ID);

                            if (tabPage != null)
                            {
                                List<PanelSection> panels = tabPage.GetPanelSections();
                                string strPosition = actionStepHistory.Position;

                                foreach (PanelSectionEx pane in panels)
                                {
                                    // 이미 실행되고 있는 SOP에 대해서는 시작버튼을 비활성화 한다.
                                    pane.HideAllSectionButtons();

                                    string szName = UnE.SOP.ProxySOP.Instance.SiteName;
                                    if (workflow.Option.HasPosition == true && strPosition != null && strPosition != "")
                                    {
                                        szName = strPosition;
                                    }

                                    pane.SetWorkflowOption(workflow.Option);
                                }
                            }

                            FormSOP.Instance.SetWorkflowState(workflow.State);
                        }
                    }
                }

                m_pageHome.NoChangeSelectedTabPage = false;
            }

            if (newHistories.Count > 0)
                m_pageHome.PostAddNewActionStepHistories(newHistories);
        }

        /*private void CheckSOPControl()
        {
            SectionTabPage page = m_pageHome.GetCurrentTabPage();

            if (page == null || page.ActionStepHistoryID < 0)
                return;

            // 서버와 접속이 끊어지면 모든 제어권을 지운다.
            if (m_netMgr != null && m_netMgr.IsConnected == false)
            {
                ClearSOPControls();
            }

            bool hasControl;
            SOPScenario scenario = SOPScenarioManager.Instance.GetSOPScenario(page.ActionStepHistoryID);

            WorkFlow work = null;

            if (scenario != null)
                work = WorkFlowManager.Instance.Get(scenario.ActionStepHistoryID);

            rbtnCheckRealMode.Enabled = rbtnCheckVirtualMode.Enabled = work == null;

            if (work == null || work.State == WorkFlowState.DONE || work.State == WorkFlowState.STOP)
                RemoveSOPControl(page.ActionStepHistoryID);
            else
            {
                if (m_dicSOPControls.TryGetValue(page.ActionStepHistoryID, out hasControl) && hasControl)
                {
                    if (GetCurrentControlStatus() == false || (scenario != null && scenario.Enabled == false) || GetSectionContentsEnabled(page) == false)
                        SetControl(true);

                    if (work == null || work.State == WorkFlowState.DONE || work.State == WorkFlowState.STOP)
                    {
                        rbtnControlStatus.Visible = rbtnControlAction.Visible = false;
                        rbtnStartSOP.Enabled = true;
                    }

                    return;
                }
            }

            if (work == null || work.State == WorkFlowState.DONE || work.State == WorkFlowState.STOP)
            {
                rbtnControlStatus.Visible = rbtnControlAction.Visible = false;
                rbtnStartSOP.Enabled = true;
            }
            else
            {
                if (GetCurrentControlStatus() || (scenario != null && scenario.Enabled) || GetSectionContentsEnabled(page))
                    SetControl(false);
            }
        }*/

        private bool GetSectionContentsEnabled(SectionTabPage page)
        {
            foreach (KeyValuePair<Section, ISectionContents> pair in page.SectionContents)
            {
                if (pair.Key is SectionEndPoint)
                {
                    if (pair.Value.EnableControl)
                        return true;
                }
                else
                {
                    return pair.Value.EnableControl;
                }
            }

            return false;
        }

#if E_SOP
        // 에너지과제용 모바일 앱(e-SOP)을 위한 임시 기능
        private void CheckESOPCommand()
        {
            string strSQL = "Select ID, ActionStepID, RealMode, ProcessID, Checked from MobileAppCommand where Processed = 0";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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
                m_dbMgr.GetResultData(strSQL);
            }
        }
#endif

        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        public void Update3DView()
        {
            if (m_netMgr == null)
                m_netMgr = new NetworkWebManager(m_dbMgr);

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
                SectionTabPage page = (SectionTabPage)(btn.Section.GetParent().Parent);

                ButtonEndPoint startBtn = (ButtonEndPoint)btn;
                SectionDataEndPoint data = (SectionDataEndPoint)startBtn.Data;
                if(data.IsBegin == true)
                {
                    if (page != null/* && HasSOPControl(page.ActionStepHistoryID)*/)
                    //if (HasControl == true)
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
                    if (page != null && HasSOPControl(page.ActionStepHistoryID))
                    //if (HasControl == true)
                    {
                        this.BeginInvoke(new Action(() =>
                        {

                            WorkFlow work = WorkFlowManager.Instance.Get(page.ActionStepHistoryID);
                            //WorkFlow work = GetCurrentWorkflow();
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
            m_pageHome.EndHistory(m_nInitComponentHistoryID == 0);
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

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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

        private bool ChangeDayLight(bool isDayLight)
        {
            SetDayLightMode(isDayLight);

            //if (this.HasControl)
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
                    m_isNormal = isDayLight;
                }
            }
        }

        private void rbtnLoadSOP_Click(object sender, EventArgs e)
        {
            m_pageHome.OpenSOP();
        }

        private void SendButtonsToPageHome()
        {
            m_pageHome.SetMainFormButtons(rbtnLoadSOP);
        }

        private void rbtnConfig_Click(object sender, EventArgs e)
        {
            SelectOptionTab();
        }

        private void rbtnCheckRealMode_Click(object sender, EventArgs e)
        {
            if (rbtnCheckRealMode.Enabled)
                SetRealModeButton(!rbtnCheckRealMode.IsChecked);
        }

        private void rbtnCheckVirtualMode_Click(object sender, EventArgs e)
        {
            if (rbtnCheckVirtualMode.Enabled)
                SetRealModeButton(rbtnCheckVirtualMode.IsChecked);
        }

        private void SetRealModeButton(bool isReal)
        {
            SectionTabPage tabPage = m_pageHome.GetCurrentTabPage();

            if (tabPage != null && tabPage.VirtualMode == isReal)
            {
                tabPage.VirtualMode = !isReal;
            }

            rbtnCheckRealMode.IsChecked = isReal;
            rbtnCheckVirtualMode.IsChecked = !isReal;
            rbtnCheckRealMode.Refresh();
            rbtnCheckVirtualMode.Refresh();
        }

        public void SetRealModeStatus(bool isReal)
        {
            if (isReal)
            {
                rbtnRealMode.Text = "실제모드";
                rbtnRealMode.ToolTipText = "실제모드";
            }
            else
            {
                rbtnRealMode.Text = "훈련모드";
                rbtnRealMode.ToolTipText = "훈련모드";
            }

            SetRealModeButton(isReal);
            rbtnRealMode.Refresh();
        }

        private void btnSOPManager_Click(object sender, EventArgs e)
        {
            //Button btn = (Button)sender;
            Label btn = (Label)sender;
            m_exeMgr.Run((ExecuteManager.APP_TYPE)btn.Tag);
        }

        private void btnTeamEditor_Click(object sender, EventArgs e)
        {
            //Button btn = (Button)sender;
            Label btn = (Label)sender;
            m_exeMgr.Run((ExecuteManager.APP_TYPE)btn.Tag);
        }

        // SOP 불러오기창 위치
        public Point GetSOPListLocation()
        {
            Point pt = rbtnLoadSOP.PointToScreen(Point.Empty);
            pt.Y += 100;
            return pt;
        }

        public bool HasSOPControl(int nActionStepHistoryID)
        {
            int nControlUserID = SOPScenarioManager.Instance.GetSOPControlUserID(nActionStepHistoryID);
            return nControlUserID == m_nSOPGenUserID;

            /*bool hasControl;

            if (m_dicSOPControls.TryGetValue(nActionStepHistoryID, out hasControl))
                return hasControl;

            return false;*/
        }

        public bool HasCurrentSOPControl()
        {
            SectionTabPage tabPage = m_pageHome.GetCurrentTabPage();

            if (tabPage == null || tabPage.ActionStepHistoryID < 0)
                return false;

            return HasSOPControl(tabPage.ActionStepHistoryID);
        }

        // 실행중인 SOP들 가운데 제어권이 없는 것들만 얻어온다.
        /*public List<int> NoControlSOPList()
        {
            ArrayList arrScenarios = SOPScenarioManager.Instance.GetAllScenario();

            bool hasControl;
            List<int> actionStepHistories = new List<int>();

            foreach (SOPScenario scenario in arrScenarios)
            {
                if (scenario.ActionStepHistoryID > 0)
                    actionStepHistories.Add(scenario.ActionStepHistoryID);
            }

            //List<int> actionStepHistories = m_dicSOPControls.Keys.ToList();

            for (int i=actionStepHistories.Count-1;i>=0;i--)
            {
                int nActionStepHistoryID = actionStepHistories[i];

                if (m_dicSOPControls.TryGetValue(nActionStepHistoryID, out hasControl) && hasControl)
                {
                    actionStepHistories.RemoveAt(i);
                }
            }

            return actionStepHistories;
        }*/

        public int GetControlActionStepHistoryIDs(List<int> controlActionStepHistoryIDs)
        {
            controlActionStepHistoryIDs.Clear();
            SOPScenarioManager.Instance.GetControlActionStepIDList(m_nSOPGenUserID, controlActionStepHistoryIDs);
            return controlActionStepHistoryIDs.Count;
        }

        /*public void ConfirmNewSOP(int nActionStepID, bool isRealMode, int nSOPGenUserID, int nActionStepHistoryID)
        {
            List<SOPConfirmData> datas = m_sopWaitConfirmDatas.Keys.ToList();

            foreach (SOPConfirmData data in datas)
            {
                if (data.ActionStepID == nActionStepID && data.IsRealMode == IsReal && data.SOPGenUserID == nSOPGenUserID)
                {
                    if (data.WorkFlow != null && data.WorkFlow.ActionStepHistoryID > 0)
                    {
                        if (data.WorkFlow.ActionStepHistoryID == nActionStepHistoryID)
                        {
                            data.Confirm = true;
                            break;
                        }
                    }
                    else
                    {
                        if (data.WorkFlow != null)
                            data.WorkFlow.ActionStepHistoryID = nActionStepHistoryID;

                        data.Confirm = true;
                        break;
                    }
                }
            }
        }*/

        private ConcurrentDictionary<int, int> m_dicNullActionStepHistoryIDs = new ConcurrentDictionary<int, int>();

        public void SetSOPControl(int nActionStepHistoryID, int nSOPGenUserID)
        {
            WorkFlow work = WorkFlowManager.Instance.Get(nActionStepHistoryID);

            if (work != null)
                work.ControlUserID = nSOPGenUserID;

            if (m_frmRequestControl != null && m_frmRequestControl.IsDisposed == false && m_frmRequestControl.Visible)
            {
                if (nSOPGenUserID != m_nSOPGenUserID)
                {
                    // 제어권이 이미 넘겨진 경우
                    this.Invoke((MethodInvoker)delegate
                    {
                        int nUserCount = m_frmRequestControl.RemoveUser(nActionStepHistoryID);

                        if (nUserCount == 0)
                        {
                            m_frmRequestControl.Close();
                            m_frmRequestControl = null;
                        }
                    });
                }
            }

            SectionTabPage page = m_pageHome.GetTabPage(nActionStepHistoryID);

            if (page != null)
            {
                Data_ActionStepHistory actionStepHistory = SOPScenarioManager.Instance.GetActionStepHistory(nActionStepHistoryID);
                WorkFlow workFlow = WorkFlowManager.Instance.Get(nActionStepHistoryID);

                // 실행중인 SOP의 SensorZoneHistoryID가 제대로 설정되어 있지 않을 경우
                if (workFlow != null && actionStepHistory != null && page.SensorZoneHistoryID != actionStepHistory.SensorZoneHistoryID && actionStepHistory.SensorZoneHistoryID > 0)
                {
                    int nSensorZoneID = ReadSensorZoneID(actionStepHistory.SensorZoneHistoryID);

                    page.SensorZoneHistoryID = actionStepHistory.SensorZoneHistoryID;
                    page.SensorID = nSensorZoneID;

                    if (workFlow.Option != null)
                    {
                        workFlow.Option.SensorZoneHistoryID = actionStepHistory.SensorZoneHistoryID;
                        workFlow.Option.SensorZoneID = nSensorZoneID;
                    }

                    SOPSupervisor.AddSOP(actionStepHistory.ID, nSensorZoneID, actionStepHistory.SensorZoneHistoryID);
                }

                if (actionStepHistory != null && actionStepHistory.EndTime != null)
                {
                    // SOP가 취소되었을 경우는 Page가 닫히기 때문에 CancelTime을 조사할 필요는 없다.
                    page.Mode = SectionTabPage.SOPMode.Done;
                    FormSOP.Instance.SOPManager.RemoveActionStepHistoryID(page.ActionStepID, !page.VirtualMode);
                }
                else if (actionStepHistory != null && actionStepHistory.CancelTime != null)
                {
                    if (workFlow != null && workFlow.State != WorkFlowState.STOP)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            StopWorkflow(actionStepHistory.CancelTime.Data, true, page);
                            SOPScenarioManager.Instance.RemoveActionStepHistory(page.ActionStepID, !page.VirtualMode);
                        });
                    }
                }
                else if (nSOPGenUserID == m_nSOPGenUserID)
                    page.Mode = SectionTabPage.SOPMode.Control;
                else
                    page.Mode = SectionTabPage.SOPMode.Monitoring;

                if (workFlow != null)
                {
                    if (page.Mode == SectionTabPage.SOPMode.Done)
                        workFlow.State = WorkFlowState.DONE;
                    else if (workFlow.State == WorkFlowState.STANDBY)
                        workFlow.State = WorkFlowState.RUN;
                }
            }
            //else
            //    m_dicNullActionStepHistoryIDs[nActionStepHistoryID] = nActionStepHistoryID;

            if (nSOPGenUserID == m_nSOPGenUserID)
            {
                // 제어권을 반납하면 지정된 시간(초) 동안은 다시 제어권을 받지 않는다.
                if (nActionStepHistoryID != m_nReturnControlActionStepHistory || (DateTime.Now - m_dtReturnControl).TotalSeconds >= m_nReturnControlWaitTime)
                {
                    //m_dicSOPControls[nActionStepHistoryID] = true;

                    CloseRequestProgress(true);
                    //SupervisorSOPClose.SupervisorSOPObtainControlAuthority();
                }
            }
            else
            {
                //RemoveSOPControl(nActionStepHistoryID);
            }
        }

        private int ReadSensorZoneID(int nSensorZoneHistoryID)
        {
            string strSQL = "Select SensorID from SensorZoneHistory where ID = " + nSensorZoneHistoryID.ToString();
            ArrayList arrRessult = m_dbMgr.GetResultData(strSQL);

            if (arrRessult == null || arrRessult.Count == 0)
                return -1;

            VariousData<int> sensorZoneID = WebDBManager.GetIntField(arrRessult[0].ToString());

            if (sensorZoneID == null)
                return -1;

            return sensorZoneID.Data;
        }

        /*public void ClearSOPControls()
        {
            List<int> actionStepHistoryIDs = m_dicSOPControls.Keys.ToList();

            foreach (int nActionStepHistoryID in actionStepHistoryIDs)
            {
                m_pastActionSteHistoryIDs.Enqueue(nActionStepHistoryID);
            }

            CheckPastActionStepHistoryCount();
            m_dicSOPControls.Clear();
        }

        public void RemoveSOPControl(int nActionStepHistoryID)
        {
            bool hasControl;
            m_dicSOPControls.TryRemove(nActionStepHistoryID, out hasControl);

            m_pastActionSteHistoryIDs.Enqueue(nActionStepHistoryID);
            CheckPastActionStepHistoryCount();
        }

        // 큐에 쌓이는 데이터가 10개가 넘지 않도록 한다.
        private void CheckPastActionStepHistoryCount()
        {
            int nTemp;
            int nCount = m_pastActionSteHistoryIDs.Count;

            for (int i = 10; i < nCount; i++)
            {
                m_pastActionSteHistoryIDs.TryDequeue(out nTemp);
            }
        }

        public List<int> GetPastActionStepHistoryIDs()
        {
            return m_pastActionSteHistoryIDs.ToList();
        }*/

        private void rbtnCheckMode_EnabledChanged(object sender, EventArgs e)
        {
            RibbonButton checkButton = (RibbonButton)sender;

            if (checkButton.Enabled == false)
            {
                if (checkButton.IsChecked)
                    checkButton.DisabledImage = checkButton.CheckedImage;
                else
                    checkButton.DisabledImage = checkButton.NormalImage;
            }
        }

        public UnE.SOP.Workstate.SOPScenario GetSOPScenario(int nActionStepHistoryID)
        {
            return SOPScenarioManager.Instance.GetSOPScenario(nActionStepHistoryID);
        }
        /*public UnE.SOP.Sections.SectionTabPage GetTabPage(int nActionStepHistoryID)
        {
            return m_pageHome.GetTabPage(nActionStepHistoryID);
        }*/

        // 이미 실행중인 SOP(nActionStepHistoryID)와 같은 Disaster 안에서 위기경보 단계만 다른(nActionStepIndex) SOP를 로딩한다.
        public bool LoadSOP(int nSensorType, int nEquipZoneID, DateTime timeStamp, int nSensorZoneID, int nSensorZoneHistoryID, int nActionStepHistoryID, int nActionStepIndex, string strSensorValue, bool runSOP)
        {
            if (nActionStepIndex <= 0 || nActionStepIndex > UnE.SOP.Sections.SectionTabControl.StandardActionStepNames.Count())
                return false;

            string strActionStepName = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[nActionStepIndex - 1];

            string strFormat = "Select step.ID, (Select RealMode from ActionStepHistory where ID = {1}), (Select DisasterOption from ActionStepHistory where ID = {1}), dc.CategoryName, sdc.SubCategoryName, d.DisasterName ";
            strFormat += "from ActionStep as step, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc where StepName = '{0}' and step.DisasterID in ";
            strFormat += "(Select d.ID from ActionStepHistory as ash, ActionStep as step, Disaster as d ";
            strFormat += "where ash.ActionStepID = step.ID and step.DisasterID = d.ID and ash.ID = {1}) and step.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID";

            string strSQL = string.Format(strFormat, strActionStepName, nActionStepHistoryID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 6)
                return false;

            VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> realMode = WebDBManager.GetIntField(arrResult[1].ToString());
            string strDisasterOption = WebDBManager.GetStringField(arrResult[2]);
            string strDisasterCategoryName = WebDBManager.GetStringField(arrResult[3]);
            string strSubDisasterCategoryName = WebDBManager.GetStringField(arrResult[4]);
            string strDisasterName = WebDBManager.GetStringField(arrResult[5]);

            if (actionStepID == null || realMode == null || strDisasterCategoryName == null || strSubDisasterCategoryName == null || strDisasterName == null)
                return false;

            if (strDisasterOption != null && strDisasterOption.Length > 0 && strSensorValue.Length > 0)
                strDisasterOption = MakeDisasterOption(strDisasterOption, strSensorValue);
            else
                strDisasterOption = "";

            EquipmentZone equipZone = DataManager.Instance.GetEquipZone(nEquipZoneID);
            UnE.Sensor.IFacility.FacilityType type = UnE.Sensor.IFacility.ToFacilityType(nSensorType);

            if (UnE.Sensor.IFacility.IsETCSensorType(type))
            {
                if (type == UnE.Sensor.IFacility.FacilityType.STRONG_WIND)
                    StubWorker.Instance.OpenSOP_StrongWind(actionStepID.Data, timeStamp, nSensorZoneHistoryID, nSensorZoneID, nActionStepIndex, strSensorValue, equipZone);
            }
            else if (UnE.Sensor.IFacility.IsEarthquakeSensorType(type))
            {
                string strSOPFullPath = strDisasterCategoryName + "/" + strSubDisasterCategoryName + "/" + strDisasterName + "/" + strActionStepName;
                int nIntensity = -1;
                float fMagnitude = -1;

                string strOptionLower = strDisasterOption.ToLower();

                if (strOptionLower.Contains("intensity"))
                {
                    int.TryParse(strSensorValue, out nIntensity);
                }
                else if (strOptionLower.Contains("magnitude"))
                {
                    float.TryParse(strSensorValue, out fMagnitude);
                }

                // 이미 실행중인 SOP에 대한 단계를 변경하는 것이므로 Server로부터 다시 실행권한을 받을 필요는 없다.
                // m_nSOPGenUserID를 사용한다.
                StubWorker.Instance.OpenSOP_Earthquake(strSOPFullPath, timeStamp, nSensorZoneID, nSensorZoneHistoryID, nIntensity, fMagnitude, "", m_nSOPGenUserID);
            }

            return true;
        }

        private string MakeDisasterOption(string strDisasterOption, string strValue)
        {
            int nBeginIndex, nEndIndex;

            if (ReadLastValueIndex(strDisasterOption, out nBeginIndex, out nEndIndex) == false)
                return "";

            string strHead = strDisasterOption.Substring(0, nBeginIndex);
            string strTail = strDisasterOption.Substring(nEndIndex + 1);
            return strHead + strValue + strTail;
        }

        private static bool ReadLastValueIndex(string str, out int nBeginIndex, out int nEndIndex)
        {
            int len = str.Length;
            double num = 0;

            bool begin = false, readDot = false;
            int count = 0;
            nEndIndex = nBeginIndex = -1;

            for (int i = len - 1; i >= 0; i--)
            {
                char ch = str.ElementAt(i);

                if (begin == false)
                {
                    if (ch >= '0' && ch <= '9')
                    {
                        num = ch - '0';
                        count = 1;
                        begin = true;
                        nEndIndex = nBeginIndex = i;
                    }
                    else if (ch == '.')
                    {
                        readDot = true;
                        begin = true;
                        nEndIndex = nBeginIndex = i;
                    }
                }
                else
                {
                    if (ch >= '0' && ch <= '9')
                    {
                        num = num + (ch - '0') * System.Math.Pow(10, count);
                        count++;
                        nBeginIndex = i;
                    }
                    else if (ch == '.')
                    {
                        if (readDot)
                            break;
                        else
                        {
                            num = num * System.Math.Pow(10, -count);
                            readDot = true;
                            count = 0;
                            nBeginIndex = i;
                        }
                    }
                    else
                        break;
                }
            }

            if (nBeginIndex <= nEndIndex && nBeginIndex >= 0)
                return true;

            return false;
        }

        public bool GetEquipmentZoneInfo(int nEquipZoneID, out string strEquipZoneName, out int nZoneID, out int nFloorIndex, out int nBuildingID)
        {
            strEquipZoneName = null;
            nZoneID = nFloorIndex = nBuildingID = -1;

            EquipmentZone equipZone = DataManager.Instance.GetEquipZone(nEquipZoneID);

            if (equipZone == null)
                return false;

            strEquipZoneName = equipZone.EquipZoneName;

            if (equipZone.LinkedZoneList.Count > 0)
            {
                Zone zone = (Zone)equipZone.LinkedZoneList[0];
                nFloorIndex = (int)zone.Floor.FloorIndex;
                nZoneID = zone.ID;

                if (zone.Building != null)
                    nBuildingID = zone.Building.ID;
            }

            return true;
        }

        public bool GetZoneInfo(int nZoneID, out string strZoneName, out int nFloorIndex, out int nBuildingID)
        {
            strZoneName = null;
            nFloorIndex = nBuildingID = -1;

            Zone zone = DataManager.Instance.GetZone(nZoneID);

            if (zone == null)
                return false;

            strZoneName = zone.ZoneName;
            nFloorIndex = (int)zone.Floor.FloorIndex;
            nZoneID = zone.ID;

            if (zone.Building != null)
                nBuildingID = zone.Building.ID;

            return true;
        }

        public void SetRunningActionStepHistoryIDs(List<int> runningActionStepHistoryIDs)
        {
            List<SOPScenario> scenarios = m_pageHome.GetAllScenarios();
            ArrayList arrScenarios = SOPScenarioManager.Instance.GetAllScenario();

            foreach (SOPScenario scenario in arrScenarios)
            {
                if (scenarios.Contains(scenario) == false)
                    scenarios.Add(scenario);
            }

            int nScenarioCount = scenarios.Count;

            foreach (SOPScenario scenario in scenarios)
            {
                if (scenario.ActionStepHistoryID < 0)
                    continue;

                if (runningActionStepHistoryIDs.Contains(scenario.ActionStepHistoryID) == false)
                {
                    if (CheckClosedSOP(scenario.ActionStepHistoryID))
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            StopWorkflow(DateTime.Now, true, scenario.ActionStepID, scenario.RealMode);
                            nScenarioCount--;

                            if (nScenarioCount <= 0)
                                EmptySOP();
                        });
                    }
                }
            }
        }

        public bool CheckClosedSOP(int nActionStepHistoryID)
        {
            string strSQL = "Select EndTime, CancelTime from ActionStepHistory where ID = " + nActionStepHistoryID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            // DB를 읽을수 경우
            if (arrResult == null)
                return false;

            // DB에 없는 경우
            if (arrResult.Count < 2)
                return true;

            VariousData<DateTime> endTime = WebDBManager.GetDateTimeField(arrResult[0]);
            VariousData<DateTime> cancelTime = WebDBManager.GetDateTimeField(arrResult[1]);

            return endTime != null || cancelTime != null;
        }

        private void tsMenuLogout_Click(object sender, EventArgs e)
        {
            DBUtility2.RegUtil.WriteRegValue("IntegratedManager", "AutoLogin", "0", FormSOP.Instance.DBManager.SiteID);
            this.Close();
        }
    }
}
