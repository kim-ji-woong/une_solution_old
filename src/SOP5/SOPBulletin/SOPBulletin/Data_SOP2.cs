using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SOPBulletin
{
    public class SectionState
    {
        public enum State
        {
            NORMAL = 1,
            RUN = 2,
            DONE = 3,
            INPUT = 4,
            SKIP = 5
        }

        private Sections.Section m_section;
        private State m_state = State.NORMAL;
        private int m_nCompleteCount = 0;

        public Sections.Section Section
        {
            get { return m_section; }
            set { m_section = value; }
        }

        public int CompleteCount
        {
            get { return m_nCompleteCount; }
            set { m_nCompleteCount = value; }
        }

        public State GetState()
        {
            return m_state;
        }

        public void SetState(State state)
        {
            m_state = state;
        }

        public void SetState(int nState)
        {
            if (nState < (int)State.NORMAL || nState > (int)State.SKIP)
                return;

            m_state = (State)nState;
        }
    }

    public class ComponentHistory : IComparable
    {
        // 대기(0), 단위임무 완료, 단위임무 완료 해제, 단위임무 문자메시지 전송(Process), 단위임무 방송 실행(Internal 혹은 External),
        // 문자메시지 전송(Internal 혹은 External), 방송 실행(Internal 혹은 External), 임무확인, 임무완료
        public enum HistoryType { NONE = -1, WAIT = 0, CHECK_MISSION, UNCHECK_MISSION, SEND_UNIT_SMS, SEND_UNIT_BROADCAST, SEND_SMS, SEND_BROADCAST, CONFIRM_MISSION, COMPLETE_MISSION };
        public enum ProcessDataType
        {
            COMPLETE_CHECKED = 1,
            SEND_SMS = 2,
            SEND_BROADCAST = 3,
            COMPLETE_UNCHECKED = 4,
        }

        #region Process 이외의 Section에 대한 Data Index
        // (내부상황전파)방송 실행시 Datas에 방송횟수 + 사이렌 사용여부 + 방송메시지 저장
        // ex) 방송횟수 1, 사이렌 사용안함, 방송메시지 : 훈련상황입니다.
        //     => "1, 0, 훈련상황입니다."
        public const int RUN_BROADCAST_INTERNAL = -1;
        // (내부상황전파)방송 완료시 Datai가 1이면 Checked, 0이면 Unchecked
        public const int COMPLETE_BROADCAST_INTERNAL = -2;
        // (내부상황전파)문자 실행시 Datas에 발신자 + 수신자 + 방송메시지 저장
        // 발신자 : CommanderMemberID(CommanderMemberType), DisplayText
        //          CommanderMemberType => -1보다 작으면 NULL, -1이면 SOPGenUserCommander의 값을 따른다. (0 : 평일 비상 조직-TemporaryNormalTeam, 1 : 휴일 비상 조직-TemporaryEmergencyTeam, 2 : 외부 기관-ExternalTeam 또는 ExternalCompanyTeam, 3 : 사용자 정의 조직-UserDefinedTeam, 4 : 상시조직-RegularTeam, 5 : 평일 비상 조직 조직원, 6 : 휴일 비상 조직 조직원, 7 : 협력업체 직원, 8 : 정규직원). 팀일 경우 해당 팀의 팀장으로 설정됨.
        // 수신자 : 수신할 팀들의 ID List이며 쉼표로 구분됨. ID의 Type은 괄호로 표시됨(0 : 평일 비상 조직-TemporaryNormalTeam, 1 : 휴일 비상 조직-TemporaryEmergencyTeam, 2 : 외부 기관-ExternalTeam, 3 : 사용자 정의 조직-UserDefinedTeam, 4 : 상시조직-RegularTeam) 예 : 1(0), 1(3)
        //          마지막에는 팀장한테만 메시지를 보내는지 여부를 표시(1이면 팀장한테만 보냄, 0이면 모두 보냄)
        // ex) 발신자 : 종합상황실장, 수신자 : "제1발전처장, 안전품질팀장"이며 팀장에게만 메시지 전송, 문자메시지 : 훈련상황
        //     => "[3(0), 최초발견자], [2(4), 6(4), 1], 훈련상황"
        public const int RUN_SMS_INTERNAL = -3;
        // (내부상황전파)문자 완료시 Datai가 1이면 Checked, 0이면 Unchecked
        public const int COMPLETE_SMS_INTERNAL = -4;
        #endregion

        private ActionStepHistory m_actionStepHistory = null;
        private int m_nComponentHistoryID = -1;
        private DateTime m_time;
        private SectionState m_sectionState = null;
        private string m_strCommander = "";
        private string m_strReceiver = "";
        private string m_strTask = "";
        private HistoryType m_type = HistoryType.WAIT;
        // SOPGenUser ID
        private int m_nAccessedUserID = -1;
        // ActionStepHistory의 ComponentHistories에 속해있는 ComponentHistory일 경우 마지막 상태값만 기억한다.
        // 하나의 Component(Section)에 대한 전체 로그는 m_allHistories에 기록한다.
        private List<ComponentHistory> m_allHistories = new List<ComponentHistory>();
        private bool m_isDetailLog = false;

        public ActionStepHistory ActionStepHistory
        {
            get { return m_actionStepHistory; }
            set { m_actionStepHistory = value; }
        }

        public int ComponentHistoryID
        {
            get { return m_nComponentHistoryID; }
            set { m_nComponentHistoryID = value; }
        }

        public DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public SectionState SectionState
        {
            get { return m_sectionState; }
            set { m_sectionState = value; }
        }

        public string Commander
        {
            get { return m_strCommander; }
            set { m_strCommander = value; }
        }

        public string Receiver
        {
            get { return m_strReceiver; }
            set { m_strReceiver = value; }
        }

        public string Task
        {
            get { return m_strTask; }
            set { m_strTask = value; }
        }

        public HistoryType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        // SOPGenUser ID
        public int AccessedUserID
        {
            get { return m_nAccessedUserID; }
            set { m_nAccessedUserID = value; }
        }

        public List<ComponentHistory> AllHistories
        {
            get { return m_allHistories; }
        }

        public bool IsDetailLog
        {
            get { return m_isDetailLog; }
            set { m_isDetailLog = value; }
        }

        public static HistoryType GetComponentHistoryType(int nDataIndex, DBUtility.VariousData<int> nData, DBUtility.VariousData<float> fData, string strData)
        {
            if (nDataIndex >= 0)
            {
                if (nData == null)
                    return HistoryType.NONE;

                if (nData.Data == (int)ProcessDataType.COMPLETE_CHECKED)
                    return HistoryType.CHECK_MISSION;
                else if (nData.Data == (int)ProcessDataType.COMPLETE_UNCHECKED)
                    return HistoryType.UNCHECK_MISSION;
                else if (nData.Data == (int)ProcessDataType.SEND_BROADCAST)
                    return HistoryType.SEND_UNIT_BROADCAST;
                else if (nData.Data == (int)ProcessDataType.SEND_SMS)
                    return HistoryType.SEND_UNIT_SMS;
                else
                    return HistoryType.NONE;
            }

            if (nDataIndex == RUN_BROADCAST_INTERNAL)
                return HistoryType.SEND_BROADCAST;
            else if (nDataIndex == COMPLETE_BROADCAST_INTERNAL)
            {
                if (nData == null)
                    return HistoryType.NONE;
                else if (nData.Data == 1)
                    return HistoryType.CHECK_MISSION;
                else if (nData.Data == 0)
                    return HistoryType.UNCHECK_MISSION;
            }
            else if (nDataIndex == RUN_SMS_INTERNAL)
                return HistoryType.SEND_SMS;
            else if (nDataIndex == COMPLETE_SMS_INTERNAL)
            {
                if (nData == null)
                    return HistoryType.NONE;
                else if (nData.Data == 1)
                    return HistoryType.CHECK_MISSION;
                else if (nData.Data == 0)
                    return HistoryType.UNCHECK_MISSION;
            }

            return HistoryType.NONE;
        }

        public int CompareTo(object obj)
        {
            ComponentHistory history = (ComponentHistory)obj;

            bool thisIsBegin, thatIsBegin;
            bool result1 = IsEndPointSection(this, out thisIsBegin);
            bool result2 = IsEndPointSection(history, out thatIsBegin);

            // 시작 Section은 항상 처음에, 종료 Section은 항상 마지막에 위치하도록 한다.
            if (result1 && result2)
            {
                if (thisIsBegin)
                    return -1;
                else
                    return 1;
            }
            else if (result1)
            {
                if (thisIsBegin)
                    return -1;
                else
                    return 1;
            }
            else if (result2)
            {
                if (thatIsBegin)
                    return 1;
                else
                    return -1;
            }

            if (this.Time < history.Time)
                return -1;
            else if (this.Time > history.Time)
                return 1;
            else
            {
                if (this.ComponentHistoryID < history.ComponentHistoryID)
                    return -1;
                else if (this.ComponentHistoryID > history.ComponentHistoryID)
                    return 1;
            }

            return 0;
        }

        private bool IsEndPointSection(ComponentHistory history, out bool isBegin)
        {
            isBegin = true;

            if (history.SectionState != null && history.SectionState.Section != null)
            {
                if (history.SectionState.Section.GetComponentType() != Sections.Section.ComponentType.ENDPOINT)
                    return false;

                Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)history.SectionState.Section.Data;
                isBegin = data.IsBegin;

                return true;
            }

            return false;
        }

        public static string ToHistoryTypeString(HistoryType type)
        {
            if (type == HistoryType.WAIT)
                return "대기";
            else if (type == HistoryType.CHECK_MISSION)
                return "완료";
            else if (type == HistoryType.SEND_UNIT_SMS)
                return "문자전송";
            else if (type == HistoryType.SEND_UNIT_BROADCAST)
                return "방송실행";
            else if (type == HistoryType.SEND_SMS)
                return "문자전송";
            else if (type == HistoryType.SEND_BROADCAST)
                return "방송실행";
            else if (type == HistoryType.CONFIRM_MISSION)
                return "부분완료";
            else if (type == HistoryType.COMPLETE_MISSION)
                return "완료";

            return "";
        }

        public ComponentHistory Clone()
        {
            ComponentHistory componentHistory = new ComponentHistory();

            componentHistory.AccessedUserID = this.AccessedUserID;
            componentHistory.ActionStepHistory = this.ActionStepHistory;
            componentHistory.Commander = this.Commander;
            componentHistory.ComponentHistoryID = this.ComponentHistoryID;
            componentHistory.IsDetailLog = this.IsDetailLog;
            componentHistory.Receiver = this.Receiver;
            componentHistory.SectionState = this.SectionState;
            componentHistory.Task = this.Task;
            componentHistory.Time = this.Time;
            componentHistory.Type = this.Type;

            foreach (ComponentHistory history in this.AllHistories)
            {
                componentHistory.AllHistories.Add(history);
            }

            return componentHistory;
        }
    }

    public class ActionStepHistory
    {
        private string m_strActionStepPath = "";
        private int m_nActionStepID = -1;
        private int m_nActionStepHistoryID = -1;
        private bool m_isRealMode = true;
        private bool m_isNormal = true;
        private string m_strPosition = "";
        // Key : int의 첫번째 Byte => Sections.Section.ComponentType
        //       int의 나머지 세 Byte => ComponentID
        private Dictionary<int, SectionState> m_dicSections = new Dictionary<int, SectionState>();
        private List<ComponentHistory> m_componentHistories = new List<ComponentHistory>();
        private TimeInfo m_timeBegin = null;
        private TimeInfo m_timeEnd = null;
        private TimeInfo m_timeCancel = null;
        private TimeInfo m_timeDetect = null;
        private string m_strCommanderName = "";

        // 특정 SectionState가 실행완료되었을 경우 진행률
        private Dictionary<SectionState, int> m_dicProcessPercentage = new Dictionary<SectionState, int>();
        // (현재까지 완료된 임무 개수) / (총 임무 개수) 가 아니라
        // 종료 Section과 얼마나 가까운 곳의 임무가 완료되었는가에 따른 진행률 값
        private int m_nCurrentSectionNumberPercentage = 0;

        public string ActionStepPath
        {
            get { return m_strActionStepPath; }
            set { m_strActionStepPath = value; }
        }

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        // 실제모드 : true, 모의훈련 모드 : false
        public bool RealMode
        {
            get { return m_isRealMode; }
            set { m_isRealMode = value; }
        }

        public string Position
        {
            get { return m_strPosition; }
            set { m_strPosition = value; }
        }

        public TimeInfo BeginTime
        {
            get { return m_timeBegin; }
            set { m_timeBegin = value; }
        }

        public TimeInfo EndTime
        {
            get { return m_timeEnd; }
            set { m_timeEnd = value; }
        }

        public TimeInfo CancelTime
        {
            get { return m_timeCancel; }
            set { m_timeCancel = value; }
        }

        public TimeInfo DetectTime
        {
            get { return m_timeDetect; }
            set { m_timeDetect = value; }
        }

        public List<ComponentHistory> ComponentHistories
        {
            get { return m_componentHistories; }
        }

        public string CommanderName
        {
            get { return m_strCommanderName; }
            set { m_strCommanderName = value; }
        }

        // (현재까지 완료된 임무 개수) / (총 임무 개수) 가 아니라
        // 종료 Section과 얼마나 가까운 곳의 임무가 완료되었는가에 따른 진행률 값
        public int CurrentSectionNumberPercentage
        {
            get
            {
                if (m_timeEnd != null)
                    return 100;

                return m_nCurrentSectionNumberPercentage;
            }
        }

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }

        public SectionState AddSectionState(int nComponentID, int nComponentType, Sections.Section section)
        {
            int nKey = (nComponentType << 24) | nComponentID;

            SectionState state = new SectionState();
            state.Section = section;

            m_dicSections[nKey] = state;
            return state;
        }

        public SectionState GetSectionState(int nComponentID, int nComponentType)
        {
            SectionState state;
            int nKey = (nComponentType << 24) | nComponentID;

            if (m_dicSections.TryGetValue(nKey, out state))
                return state;

            return null;
        }

        public int GetSectionStateCount()
        {
            return m_dicSections.Count;
        }

        public override string ToString()
        {
            string szResult = m_strActionStepPath;
            if (RealMode)
            {
                if (IsNormal)
                    szResult += "(실제상황)";
                else
                    szResult += "(실제상황/야간 및 휴일)";
            }
            else
            {
                if (!IsNormal)
                    szResult += "(야간 및 휴일)";
            }
            return szResult;
        }

        public void CalcProcessPercentage()
        {
            List<int> values = new List<int>();
            List<SectionState> keys = new List<SectionState>();

            foreach (KeyValuePair<int, SectionState> pair in m_dicSections)
            {
                int nSectionNumber = pair.Value.Section.Data.SectionNumber;
                m_dicProcessPercentage[pair.Value] = nSectionNumber;

                keys.Add(pair.Value);
                values.Add(nSectionNumber);
            }

            values.Sort();

            foreach (SectionState key in keys)
            {
                int value = m_dicProcessPercentage[key];
                int nIndex = values.IndexOf(value);

                if (nIndex < 0)
                    continue;

                // 시작 Section과 종료 Section도 진행률 계산에 포함시킨다.
                // 종료 Section이 실행되지 않을 경우 진행률은 항상 100%보다 작으며,
                // 시작 Section이 눌려졌을 경우 진행률은 항상 0%보다 크다.
                int nPercent = (nIndex + 1) * 100 / values.Count;
                m_dicProcessPercentage[key] = nPercent;
            }
        }

        public void SetCompleteSectionState(SectionState sectionState)
        {
            if (sectionState == null)
                return;

            int nPercent;

            if (m_dicProcessPercentage.TryGetValue(sectionState, out nPercent))
            {
                if (m_nCurrentSectionNumberPercentage < nPercent)
                    m_nCurrentSectionNumberPercentage = nPercent;
            }
        }
    }
    
    public class ColorStyle
    {
        private Color m_clrActionStepTitleBack = Color.FromArgb(16, 37, 63);
        private Color m_clrActionStepBodyBack = Color.Black;
        private Color m_clrActionStepTitleFore = Color.White;
        private Color m_clrActionStepBodyFore = Color.White;
        private Color m_clrLogColumnBack = Color.FromArgb(16, 37, 63);
        private Color m_clrLogColumnFore = Color.White;
        private Color m_clrLogNoBack = Color.FromArgb(16, 37, 63);
        private Color m_clrLogNoFore = Color.White;
        private Dictionary<ComponentHistory.HistoryType, Color> m_dicHistoryTypeBackColor = new Dictionary<ComponentHistory.HistoryType, Color>();
        private Dictionary<ComponentHistory.HistoryType, Color> m_dicHistoryTypeForeColor = new Dictionary<ComponentHistory.HistoryType, Color>();
        private Color m_clrTitleBarBack = Color.FromArgb(16, 37, 63);
        private Color m_clrTitleBarFore = Color.White;
        private Color m_clrProgressTitleBack = Color.FromArgb(16, 37, 63);
        private Color m_clrProgressTitleFore = Color.White;
        private Color m_clrProgressBar = Color.FromArgb(99, 37, 35);
        private Color m_clrOddRow = Color.FromArgb(223, 223, 223);
        private Color m_clrEvenRow = Color.White;

        public Color ActionStepTitleBackColor
        {
            get { return m_clrActionStepTitleBack; }
            set { m_clrActionStepTitleBack = value; }
        }

        public Color ActionStepBodyBackColor
        {
            get { return m_clrActionStepBodyBack; }
            set { m_clrActionStepBodyBack = value; }
        }

        public Color ActionStepTitleForeColor
        {
            get { return m_clrActionStepTitleFore; }
            set { m_clrActionStepTitleFore = value; }
        }

        public Color ActionStepBodyForeColor
        {
            get { return m_clrActionStepBodyFore; }
            set { m_clrActionStepBodyFore = value; }
        }

        public Color LogColumnBackColor
        {
            get { return m_clrLogColumnBack; }
            set { m_clrLogColumnBack = value; }
        }

        public Color LogColumnForeColor
        {
            get { return m_clrLogColumnFore; }
            set { m_clrLogColumnFore = value; }
        }

        public Color LogNoBackColor
        {
            get { return m_clrLogNoBack; }
            set { m_clrLogNoBack = value; }
        }

        public Color LogNoForeColor
        {
            get { return m_clrLogNoFore; }
            set { m_clrLogNoFore = value; }
        }

        public Color TitleBarBackColor
        {
            get { return m_clrTitleBarBack; }
            set { m_clrTitleBarBack = value; }
        }

        public Color TitleBarForeColor
        {
            get { return m_clrTitleBarFore; }
            set { m_clrTitleBarFore = value; }
        }

        public Color ProgressTitleBackColor
        {
            get { return m_clrProgressTitleBack; }
            set { m_clrProgressTitleBack = value; }
        }

        public Color ProgressTitleForeColor
        {
            get { return m_clrProgressTitleFore; }
            set { m_clrProgressTitleFore = value; }
        }

        public Color ProgressBarColor
        {
            get { return m_clrProgressBar; }
            set { m_clrProgressBar = value; }
        }

        public Color OddRowColor
        {
            get { return m_clrOddRow; }
            set { m_clrOddRow = value; }
        }

        public Color EvenRowColor
        {
            get { return m_clrEvenRow; }
            set { m_clrEvenRow = value; }
        }

        public void SetHistoryTypeColor(ComponentHistory.HistoryType type, bool isBackColor, Color color)
        {
            Dictionary<ComponentHistory.HistoryType, Color> dicTypeColor = isBackColor ? m_dicHistoryTypeBackColor : m_dicHistoryTypeForeColor;
            dicTypeColor[type] = color;
        }

        public bool GetHistoryTypeColor(ComponentHistory.HistoryType type, bool isBackColor, out Color color)
        {
            Dictionary<ComponentHistory.HistoryType, Color> dicTypeColor = isBackColor ? m_dicHistoryTypeBackColor : m_dicHistoryTypeForeColor;

            if (dicTypeColor.TryGetValue(type, out color))
                return true;

            if (isBackColor)
                color = Color.White;
            else
                color = Color.Black;

            return false;
        }
    }

    public class SectionCommanderEx : Sections.SectionCommander
    {
        // SOPGenUser에 설정된 값에 따라 바뀌는 Commander인가?
        private bool m_isDefaultCommander = false;

        public bool IsDefaultCommander
        {
            get { return m_isDefaultCommander; }
            set { m_isDefaultCommander = value; }
        }
    }

    public class NoString : IComparable
    {
        private DBUtility.VariousData<int> m_nHeadNo = null;
        private DBUtility.VariousData<int> m_nTailNo = null;

        public DBUtility.VariousData<int> HeadNumber
        {
            get { return m_nHeadNo; }
            set { m_nHeadNo = value; }
        }

        public DBUtility.VariousData<int> TailNumber
        {
            get { return m_nTailNo; }
            set { m_nTailNo = value; }
        }

        public NoString()
        {
        }

        public NoString(int nHeadNo)
        {
            m_nHeadNo = new DBUtility.VariousData<int>(nHeadNo);
        }

        public NoString(int nHeadNo, int nTailNo)
        {
            m_nHeadNo = new DBUtility.VariousData<int>(nHeadNo);
            m_nTailNo = new DBUtility.VariousData<int>(nTailNo);
        }

        public override string ToString()
        {
            if (m_nHeadNo == null)
                return "";
            else if (m_nTailNo == null)
                return m_nHeadNo.Data.ToString();

            return string.Format("{0}-{1}", m_nHeadNo.Data, m_nTailNo.Data);
        }

        public int CompareTo(object obj)
        {
            NoString noStr = (NoString)obj;

            if (this.m_nHeadNo == null)
            {
                if (noStr.m_nHeadNo == null)
                    return 0;
                else
                    return -1;
            }
            else if (noStr.m_nHeadNo == null)
                return 1;

            if (this.m_nHeadNo.Data < noStr.m_nHeadNo.Data)
                return -1;
            else if (this.m_nHeadNo.Data > noStr.m_nHeadNo.Data)
                return 1;

            if (this.m_nTailNo == null)
            {
                if (noStr.m_nTailNo == null)
                    return 0;
                else
                    return -1;
            }
            else if (noStr.m_nTailNo == null)
                return 1;

            if (this.m_nTailNo.Data < noStr.m_nTailNo.Data)
                return -1;
            else if (this.m_nTailNo.Data > noStr.m_nTailNo.Data)
                return 1;

            return 0;
        }
    }
}
