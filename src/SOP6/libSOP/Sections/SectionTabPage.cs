using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Sections;
using DBUtility2;

namespace UnE.SOP.Sections
{
    public enum TabPageState 
    {
        USE = 1,
        NOUSE = 2
    }

    public partial class SectionTabPage : TabPage
    {
        /// <summary>
        /// SOP 페이지들의 실행모드
        /// Monitoring : SOP 페이지가 모니터링 상태로 실행중인 상태
        /// Control : SOP 페이지가 제어권이 있는 상태로 실행중인 상태
        /// Wait_Self : SOP 페이지를 직접 열었는데 아직 실행은 되지않은 상태
        /// Wait_Server : SOP 페이지가 서버에 의해 열렸는데 아직 Component들의 실행이력을 못받은 상태
        /// Done : SOP가 종료되었을 경우
        /// </summary>
        public enum SOPMode { None = 0, Monitoring, Control, Wait_Self, Wait_Server, Done };

        public int height = 0;
        private ITabPageSpecialWorker m_tabPageSpecialWorker = null;
                
        private static TabControl m_ParentTab = null;
        private static ISectionContentsFactory m_sectionContentsFactory = null;

        private Dictionary<Section, ISectionContents> m_dicSectionContents = null;
        private ISectionContentsFactory m_ownSectionContentsFactory = null;

        private SOPMode m_mode = SOPMode.None;

        private List<Section> m_sectionForComponentContents = null;
        private int m_nComponentContentsIndex = -1;

        public static System.Windows.Forms.TabControl ParentTab
        {
            get { return m_ParentTab; }
            set { m_ParentTab = value; }
        }

        public static ISectionContentsFactory SectionContentsFactory
        {
            get { return m_sectionContentsFactory; }
            set { m_sectionContentsFactory = value; }
        }

        // ComponentContents Loading이 오래 걸리기 때문에 Timer에서 조금씩 실행시키도록 한다.
        public bool FinishComponentContentsLoading
        {
            get
            {
                if (m_sectionForComponentContents == null || m_nComponentContentsIndex < 0)
                    return true;

                return m_nComponentContentsIndex >= m_sectionForComponentContents.Count;
            }
        }

        public ISectionContentsFactory OwnSectionContentsFactory
        {
            get { return m_ownSectionContentsFactory; }
            set { m_ownSectionContentsFactory = value; }
        }

        public Dictionary<Section, ISectionContents> SectionContents
        {
            get { return m_dicSectionContents; }
        }

        public ITabPageSpecialWorker SpecialWorker
        {
            get { return m_tabPageSpecialWorker; }
            set { m_tabPageSpecialWorker = value; }
        }

        public SOPMode Mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }
        
        public SectionTabPage(TabControl tabControl)
            : base()
        {
            if (m_ParentTab == null)
                m_ParentTab = tabControl;

            if (m_sectionContentsFactory != null)
                m_ownSectionContentsFactory = m_sectionContentsFactory;

            InitializeComponent();

        }


        // Tab없애기
        //private string m_szText = "";
        //public override string Text 
        //{
        //    get { return m_szText; }
        //    set
        //    {
        //        base.Text = "";
        //        m_szText = value;
        //    } 
        //}


        private bool bVirtualMode = false;
        public bool VirtualMode
        {
            get { return bVirtualMode; }
            set { 
                bVirtualMode = value;
                WatermarkImage();
            }
        }

        // 평일모드인가?
        private bool m_isNormal = true;
        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }

        private bool bNewCreate = true;
        public bool CreateNew
        {
            get { return bNewCreate; }
            set { bNewCreate = value; }
        }

        private TabPageState mState = TabPageState.NOUSE;
        public TabPageState State
        {
            get { return mState; }
            set { mState = value; }
        }
        private int nActionStepID = 0;
        public int ActionStepID
        {
            get { return nActionStepID; }
            set { nActionStepID = value; }
        }

        private int m_nActionStepHistoryID = 0;
        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set
            {
                m_nActionStepHistoryID = value;                
            }
        }

        private bool bUseWaterMark = false;
        public bool UseWaterMark
        {
            get { return bUseWaterMark; }
            set {
                bUseWaterMark = value;
                WatermarkImage();
            }
        }

        private int m_nSensorZoneHistoryID = -1;
        public int SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        private int m_nSensorID = -1;
        public int SensorID
        {
            get { return m_nSensorID; }
            set { m_nSensorID = value; }
        }
        

        #region 이 TabPage에 나타나는 SOP에 사용된 팀 리스트
        // 사용자 정의조직 ID, 연결된 정의 조직 Data
        private Dictionary<int, Data_UserDefinedTeam> m_dicUserDefinedTeam = new Dictionary<int, Data_UserDefinedTeam>();
        // 외부조직 ID, 연결된 외부조직 Data
        private Dictionary<int, Data_ExternalTeam> m_dicExternalTeam = new Dictionary<int, Data_ExternalTeam>();
        // 평일 비상조직 ID, 연결된 연결된 평일 비상조직 Data
        private Dictionary<int, Data_NormalTeam> m_dicTemporaryNormalTeam = new Dictionary<int, Data_NormalTeam>();
        // 야간 및 휴일 비상조직 ID, 연결된 연결된 야간 및 휴일 비상조직 Data
        private Dictionary<int, Data_EmergencyTeam> m_dicTemporaryEmergencyTeam = new Dictionary<int, Data_EmergencyTeam>();
        // 정규조직 ID, 연결된 정규조직 Data
        private Dictionary<int, Data_RegularTeam> m_dicRegularTeam = new Dictionary<int, Data_RegularTeam>();
        private Dictionary<int, Data_ControlRoom> m_dicControlRoom = new Dictionary<int, Data_ControlRoom>();
        #endregion

        public List<Data_UserDefinedTeam> GetUsingUserDefineTeams()
        {
            List<Data_UserDefinedTeam> arResult = new List<Data_UserDefinedTeam>();
            arResult.AddRange(m_dicUserDefinedTeam.Values);
            return arResult;
        }

        public List<Data_ExternalTeam> GetUsingExternalTeams()
        {
            List<Data_ExternalTeam> arResult = new List<Data_ExternalTeam>();
            arResult.AddRange(m_dicExternalTeam.Values);
            return arResult;
        }

        public List<Data_NormalTeam> GetUsingTemporaryNormalTeams()
        {
            List<Data_NormalTeam> arResult = new List<Data_NormalTeam>();
            arResult.AddRange(m_dicTemporaryNormalTeam.Values);
            return arResult;
        }

        public List<Data_EmergencyTeam> GetUsingTemporaryEmergencyTeams()
        {
            List<Data_EmergencyTeam> arResult = new List<Data_EmergencyTeam>();
            arResult.AddRange(m_dicTemporaryEmergencyTeam.Values);
            return arResult;
        }

        public List<Data_RegularTeam> GetUsingRegularTeams()
        {
            List<Data_RegularTeam> arResult = new List<Data_RegularTeam>();
            arResult.AddRange(m_dicRegularTeam.Values);
            return arResult;
        } 

        public void AddExternalTeam(Data_ExternalTeam team)
        {
            m_dicExternalTeam[team.ID] = team;
            /*if (!m_dicExternalTeam.ContainsKey(team.ID))
            {
                m_dicExternalTeam.Add(team.ID, team);
            }*/
        }

        public void AddUserDefinedTeam(Data_UserDefinedTeam team)
        {
            m_dicUserDefinedTeam[team.ID] = team;
            /*if (!m_dicUserDefinedTeam.ContainsKey(team.ID))
            {
                m_dicUserDefinedTeam.Add(team.ID, team);
            }*/
        }

        public void AddTemporaryNormalTeam(Data_NormalTeam team)
        {
            m_dicTemporaryNormalTeam[team.ID] = team;
            /*if (!m_dicTemporaryNormalTeam.ContainsKey(team.ID))
            {
                m_dicTemporaryNormalTeam.Add(team.ID, team);
            }*/
        }

        public void AddTemporaryEmergencyTeam(Data_EmergencyTeam team)
        {
            m_dicTemporaryEmergencyTeam[team.ID] = team;
            /*if (!m_dicTemporaryEmergencyTeam.ContainsKey(team.ID))
            {
                m_dicTemporaryEmergencyTeam.Add(team.ID, team);
            }*/
        }

        public void AddRegularTeam(Data_RegularTeam team)
        {
            m_dicRegularTeam[team.ID] = team;
            /*if (!m_dicRegularTeam.ContainsKey(team.ID))
            {
                m_dicRegularTeam.Add(team.ID, team);
            }*/
        }
        public void AddControlRoom(Data_ControlRoom team)
        {
            m_dicControlRoom[team.ID] = team; 
        }

        /*public void AddUserDefinedTeams(ArrayList teams)
        {
            m_dicUserDefinedTeam.Clear();
            foreach (Data_UserDefinedTeam team in teams)
            {
                AddUserDefinedTeam(team);
            }
        }*/

        public Data_EmergencyTeam GetTemporaryEmergencyTeamMember(int nTeamID)
        {
            if (!m_dicTemporaryEmergencyTeam.ContainsKey(nTeamID))
                return null;

            return m_dicTemporaryEmergencyTeam[nTeamID];
        }

        public Data_NormalTeam GetTemporaryNormalTeamMember(int nTeamID)
        {
            if (!m_dicTemporaryNormalTeam.ContainsKey(nTeamID))
                return null;

            return m_dicTemporaryNormalTeam[nTeamID];
        }

        public Data_RegularTeam GetRegularTeamMember(int nTeamID)
        {
            if (!m_dicRegularTeam.ContainsKey(nTeamID))
            {
                if (UnE.SOP.ProxySOP.Instance.SOPDataContainer == null)
                    return null;

                SOPManager mgr = (SOPManager)UnE.SOP.ProxySOP.Instance.SOPDataContainer;
                return mgr.GetRegularTeam(nTeamID);
            }

            return m_dicRegularTeam[nTeamID];
        }
        public Data_ControlRoom GetControlRoomMember(int nTeamID)
        {
            if (!m_dicControlRoom.ContainsKey(nTeamID))
                return null;

            return m_dicControlRoom[nTeamID];
        }

        public Data_ExternalTeam GetExternalTeamMember(int nTeamID)
        {
            if (!m_dicExternalTeam.ContainsKey(nTeamID))
                return null;

            return m_dicExternalTeam[nTeamID];
        }

        public Data_UserDefinedTeam GetUserDefinedTeamMember(int nUserDefinedTeamID)
        {
            if (!m_dicUserDefinedTeam.ContainsKey(nUserDefinedTeamID))
                return null;

            return m_dicUserDefinedTeam[nUserDefinedTeamID];
        }       

        private Panel panelComponentContents = new Panel();
        public Panel PanelComponentContents
        {
            get { return panelComponentContents; }
        }

        private Panel panelPreviewComponentContents = null;
        public Panel PanelPreviewComponentContents
        {
            get { return panelPreviewComponentContents; }
            set { panelPreviewComponentContents = value; }
        }

        private string m_strLinkedZoneName = "";
        public string LinkedZoneName
        {
            get { return m_strLinkedZoneName; }
            set { m_strLinkedZoneName = value; }
        }

        private int m_nLinkedZoneID = -1;
        public int LinkedZoneID
        {
            get { return m_nLinkedZoneID; }
            set { m_nLinkedZoneID = value; }
        }

        private DateTime m_dtLinkedTime = new DateTime();
        public DateTime LinkedTime
        {
            get { return m_dtLinkedTime; }
            set { m_dtLinkedTime = value; }
        }

        public void WatermarkImage()
        {
            if (bVirtualMode && UseWaterMark)
            {
                Bitmap bitmap = new Bitmap(global::libSOP.Properties.Resources.BackgroundLog);
                foreach (Control contorl in Controls)
                {
                    if (typeof(PanelSection).IsAssignableFrom(contorl.GetType()))                  
                    {
                        PanelSection panel = (PanelSection)contorl;
                        panel.BackgroundImage = bitmap;
                        panel.BackgroundImageLayout = ImageLayout.None;
                    }                        
                }
            }
            else
            {
                Bitmap bitmap = new Bitmap(global::libSOP.Properties.Resources.BackgroundNon);
                foreach (Control contorl in Controls)
                {
                    if (typeof(PanelSection).IsAssignableFrom(contorl.GetType()))
                    {
                        PanelSection panel = (PanelSection)contorl;
                        panel.BackgroundImage = bitmap;
                        panel.BackgroundImageLayout = ImageLayout.None;
                    }                        
                }                    
            }
        }


        public override void Refresh()
        {
            base.Refresh();

            foreach (Control contorl in Controls)
            {
                if (typeof(PanelSection).IsAssignableFrom(contorl.GetType()))               
                {
                    PanelSection panel = (PanelSection)contorl;
                    panel.Refresh();
                }
            }
        }


        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SectionTabPage
            // 
            this.SizeChanged += new System.EventHandler(this.SectionTabPage_SizeChanged);           
            this.ResumeLayout(false);
        }

	    public new System.Drawing.Size Size
	    {
		    get { return base.Size; }
		    set { base.Size = value; }
	    }

        public void ReSizePanel()
        {
            TabControl tabControl = m_ParentTab;
                
            if( tabControl != null)
            {
                TabPage tabPage1 = this;
                Size sz = tabPage1.Size;
                int nCount = tabPage1.Controls.Count;
                int nVisibleCount = 0;
                foreach (PanelSection panel in tabPage1.Controls)
                {
                    if (panel.Visible == true)
                        nVisibleCount++;
                    else
                    {
                        panel.Size = new System.Drawing.Size(sz.Width, sz.Height);
                        panel.Location = new System.Drawing.Point(0, 0);
                    }
                }

                if (nCount > 0 && nVisibleCount > 0)
                {
                    sz.Width = tabPage1.Width / nVisibleCount;
                    sz.Height = tabPage1.Size.Height;
                    Point pt = new Point(0, 0);
                    foreach (PanelSection panel in tabPage1.Controls)
                    {
                        if (panel.Visible == true)
                        {
                            panel.Size = new System.Drawing.Size(sz.Width, sz.Height);
                            panel.Location = new System.Drawing.Point(pt.X, 0);
                            pt.X += sz.Width;
                        }
                    }
                }
            }
        }

        private void SectionTabPage_SizeChanged(object sender, EventArgs e)
        {
            ReSizePanel();
        }

        public List<PanelSection> GetPanelSections()
        {
            List<PanelSection> arPanels = new List<PanelSection>();
            foreach (PanelSection panel in Controls)
            {
                arPanels.Add(panel);
            }
            return arPanels;
        }

        public Dictionary<Section, ISectionContents> CreateSectionContents(List<Section> sections, ISectionContentsOwner owner)
        {
            if (m_dicSectionContents != null)
                m_dicSectionContents.Clear();

            if (m_ownSectionContentsFactory != null)
            {
                m_dicSectionContents = m_ownSectionContentsFactory.CreateSectionContents(sections, PanelComponentContents, owner);
            }

            return m_dicSectionContents;
        }

        public ISectionContents CreateSectionContentsOneByOne(ISectionContentsOwner owner, List<Section> sections = null)
        {
            if (sections != null)
            {
                m_nComponentContentsIndex = 0;
                m_sectionForComponentContents = sections;
                m_dicSectionContents = new Dictionary<Section, ISectionContents>();
                return null;
            }

            if (FinishComponentContentsLoading)
                return null;

            if (m_ownSectionContentsFactory != null && m_dicSectionContents != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    Section section = m_sectionForComponentContents[m_nComponentContentsIndex++];
                    ISectionContents contents = m_ownSectionContentsFactory.CreateSectionContents(section, PanelComponentContents, owner);

                    if (contents != null)
                    {
                        SectionTabPage page = (SectionTabPage)section.GetParent().Parent;
                        Workstate.SectionState state = Workstate.WorkFlowManager.Instance.Find(section, !page.VirtualMode);

                        if (state != null)
                            state.SectionContents = contents;

                        m_dicSectionContents[section] = contents;
                    }

                    if (i == 1)
                        return contents;
                    else if (FinishComponentContentsLoading)
                        return contents;
                }
            }

            return null;
        }
    }

    public interface ITabPageSpecialWorker
    {
        void Work(object arg);
    }

    public enum SectionContentsEvent
    {
        RunSOP = 0,
        FinishSOP,
        Done,
        CheckedMission,
        UncheckedMission,
        SendSMS,
        SendBroadcast,
        RunMissionExternal
    }

    public interface ISectionContentsOwner
    {
        void OnSectionContentsEvent(SectionContentsEvent e, object arg);
        bool AllowSectionRefresh { get; set; }
        void NeedRefreshContents(ISectionContents contents);
        #region 문자메시지 관련
        // 문자메시지 발신자 번호
        string GetSMSCaller(ISectionContents contents);
        // dicPhoneNumbers : Key와 Value가 같은 값이다.
        //                   중복으로 전화번호가 입력되는걸 방지하기 위해 List 대신 Dictioanry를 사용한다.
        void GetSOPTeamPhoneNumbers(SOPTeam team, bool onlyTeamLeader, Dictionary<string, string> dicPhoneNumbers);
        // 교대근무자를 감안하여 수신자 리스트를 조정한다.
        void CheckControlTeamValidPhoneNumbers(ArrayList phoneNumbers);
        bool OnSendSMSClick(ArrayList phoneNumbers, string strSender, string strMessage, bool needConfirm, out string strErrorMessage);
        #endregion
        bool OnRunBroadcastClick(string strMessage, int nBroadcastCount, bool useSiren, bool needConfirm);
    }

    public interface ISectionContents
    {
        Section Section { get; set; }
        UnE.SOP.Workstate.State State { get; set; }
        SectionCommander Commander { get; set; }
        ISectionContents NextContents { get; set; }
        bool EnableControl { get; set; }
        bool IsSelected { get; set; }
        bool Collapsed { get; set; }
        string Title { get; set; }
        string TeamName { get; }
        ISectionContentsOwner ContentsOwner { get; set; }
        int ComponentHistoryID { get; set; }
        void FocusContents();
        // 내부상황전파
        bool GetItem(out bool isBroadcast, out bool isExecute, out bool isComplete, out int nBroadcastCount, out bool useSiren, out VariousData<DateTime> excuteTime, out VariousData<DateTime> completeTime, out VariousData<DateTime> unCompleteTime, out string strMessage);
        // 프로세스
        bool GetItem(int nRowIndex, out bool isSendSMS, out bool isComplete, out string strSender, out string strItem, out string strTeamName, out string strPerformer, out VariousData<DateTime> excuteTime, out VariousData<DateTime> completeTime, out VariousData<DateTime> unCompleteTime);
        bool SelectRow(List<int> rowIndexList);
        void SetDetailData(int nRowIndex, int nData, DBUtility2.VariousData<DateTime> time);
        void SetDetailDatas(int nComponentHistoryID, List<UnE.SOP.History.HistorySectionData.DetailData> detailDatas);
        void UpdateContents(int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete);
        // SOP 실행상태로 만든다.
        void Start(UnE.SOP.Workstate.WorkflowOption option, bool isRealMode);
        // SOP 실행이 종료되었다.
        void Finish();
    }

    public interface ISectionContentsFactory
    {
        ISectionContents CreateSectionContents(Section section, ISectionContentsOwner owner);
        // sections에 있는 모든 Section들에 대한 ComponentContents를 한꺼번에 만든다.
        Dictionary<Section, ISectionContents> CreateSectionContents(List<Section> sections, Control parent, ISectionContentsOwner owner);
        // 하나의 Section에 대해서만 ComponentContents를 만든다.
        ISectionContents CreateSectionContents(Section section, Control parent, ISectionContentsOwner owner);
    }
}
