using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBUtility2;
using System.Drawing;
using System.Collections;

namespace PersonalSOP.History
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

        public static HistoryType GetComponentHistoryType(int nDataIndex, VariousData<int> nData, VariousData<float> fData, string strData)
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
        private VariousData<int> m_nHeadNo = null;
        private VariousData<int> m_nTailNo = null;

        public VariousData<int> HeadNumber
        {
            get { return m_nHeadNo; }
            set { m_nHeadNo = value; }
        }

        public VariousData<int> TailNumber
        {
            get { return m_nTailNo; }
            set { m_nTailNo = value; }
        }

        public NoString()
        {
        }

        public NoString(int nHeadNo)
        {
            m_nHeadNo = new VariousData<int>(nHeadNo);
        }

        public NoString(int nHeadNo, int nTailNo)
        {
            m_nHeadNo = new VariousData<int>(nHeadNo);
            m_nTailNo = new VariousData<int>(nTailNo);
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

    class Data_CompanyMember
    {
        private int m_nID;
        private string m_strMemberName;
        private int m_nRegularTeamID;
        private int m_nTemporaryTeamID;
        private int m_nLevelID;
        private int m_nPositionID;
        private int m_nTemporaryPositionID;
        private string m_strMemberID;
        private string m_strPhoneNumber;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public int RegularTeamID
        {
            get { return m_nRegularTeamID; }
            set { m_nRegularTeamID = value; }
        }

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public int PositionID
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        public int SecondRegularTeamID
        {
            get { return m_nTemporaryTeamID; }
            set { m_nTemporaryTeamID = value; }
        }

        public int SecondPositionID
        {
            get { return m_nTemporaryPositionID; }
            set { m_nTemporaryPositionID = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }
    }

    public class Data_SOPGenUser
    {
        private int m_nID = -1;
        private int m_nMemberID = -1;
        private string m_strUserName = "";
        private int m_nUserLevel = -1;
        private int m_nTeamID = -1;
        private string m_strPassword = "";
        private string m_strUserID = "";
        private string m_strNickName = "";
        private Commander m_commanderDayLight = null;
        private Commander m_commanderNight = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string NickName
        {
            get { return m_strNickName; }
            set { m_strNickName = value; }
        }

        public Commander DayLightCommander
        {
            get { return m_commanderDayLight; }
            set { m_commanderDayLight = value; }
        }

        public Commander NightCommander
        {
            get { return m_commanderNight; }
            set { m_commanderNight = value; }
        }
    }

    class Data_DisasterCategory
    {
        private int m_nID;
        private string m_strCategoryName;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string CategoryName
        {
            get { return m_strCategoryName; }
            set { m_strCategoryName = value; }
        }
    }

    class Data_SubDisasterCategory
    {
        private int m_nID;
        private int m_nDisasterID;
        private string m_strCategoryName;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }
        public string CategoryName
        {
            get { return m_strCategoryName; }
            set { m_strCategoryName = value; }
        }
    }

    class Data_Disaster
    {
        private int m_nID;
        private string m_strDisasterName;
        private int m_nSubDisasterID;
        private int m_nVersionID;
        private string m_strDescription;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string DisasterName
        {
            get { return m_strDisasterName; }
            set { m_strDisasterName = value; }
        }
        public int SubDisasterID
        {
            get { return m_nSubDisasterID; }
            set { m_nSubDisasterID = value; }
        }
        public int VersionID
        {
            get { return m_nVersionID; }
            set { m_nVersionID = value; }
        }
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }

    public class Data_RegularTeam
    {
        private int m_nID = -1;
        private string m_strTeamName = "";
        private Data_RegularTeam m_parentTeam = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public Data_RegularTeam ParentTeam
        {
            get { return m_parentTeam; }
            set { m_parentTeam = value; }
        }
    }

    class Data_SearchMember
    {
        private int m_nMemberID;
        private string m_strMemberName;
        private int m_nTeamID;
        private string m_strTeamName;
        private string m_strFullPathName;

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public string FullPathName
        {
            get { return m_strFullPathName; }
            set { m_strFullPathName = value; }
        }
    }

    class Data_Task
    {
        private int m_nID;
        private int m_nStepMemberID;
        private string m_strTaskCategory;
        private string m_strTaskName;
        private string m_strDescription;

        public int TaskID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int StepMemberID
        {
            get { return m_nStepMemberID; }
            set { m_nStepMemberID = value; }
        }

        public string TaskCategory
        {
            get { return m_strTaskCategory; }
            set { m_strTaskCategory = value; }
        }

        public string TaskName
        {
            get { return m_strTaskName; }
            set { m_strTaskName = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

    }

    public class Data_Building
    {
        private int m_nID = -1;
        private string m_strBuildingCode = "";
        private string m_strBuildingName = "";
        private Data_BuildingGroup m_buildingGroup = null;
        private int m_nMaxFloorIndex = -1;
        private int m_nMinFloorIndex = -1;
        private string m_strBroadCastingText = null;

        public Data_Building()
        {
        }

        public Data_Building(int nID, string strBuildingCode, string strBuildingName, Data_BuildingGroup buildingGroup, int nMaxFloorIndex, int nMinFloorIndex, string strBroadCastingText)
        {
            m_nID = nID;
            m_strBuildingCode = strBuildingCode;
            m_strBuildingName = strBuildingName;
            m_buildingGroup = buildingGroup;
            m_nMaxFloorIndex = nMaxFloorIndex;
            m_nMinFloorIndex = nMinFloorIndex;
            m_strBroadCastingText = strBroadCastingText;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string BuildingCode
        {
            get { return m_strBuildingCode; }
            set { m_strBuildingCode = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public Data_BuildingGroup BuildingGroup
        {
            get { return m_buildingGroup; }
            set { m_buildingGroup = value; }
        }

        public int MaxFloor
        {
            get { return m_nMaxFloorIndex; }
            set { m_nMaxFloorIndex = value; }
        }

        public int MinFloor
        {
            get { return m_nMinFloorIndex; }
            set { m_nMinFloorIndex = value; }
        }

        public string BroadCastingText
        {
            get { return m_strBroadCastingText == null ? m_strBuildingName : m_strBroadCastingText; }
            set { m_strBroadCastingText = value; }
        }
    }

    public class Data_BuildingGroup
    {
        private int m_nID = -1;
        private string m_strGroupName = "";
        private string m_strSiteName = "";

        public Data_BuildingGroup()
        {
        }

        public Data_BuildingGroup(int nID, string strGroupName, string strSiteName)
        {
            m_nID = nID;
            m_strGroupName = strGroupName;
            m_strSiteName = strSiteName;
        }

        public int GroupID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }

        public string SiteName
        {
            get { return m_strSiteName; }
            set { m_strSiteName = value; }
        }

    }

    class Data_EquipmentInfo
    {
        private string m_strEquipID;
        private int m_nZoneID;
        private string m_strZoneName;
        private int m_nFloorIndex;
        private int m_nBuildingID;
        private string m_strBuildingName;
        private int m_nGroupID;
        private string m_strGroupName;
        private string m_strSiteName;
        private int m_nMaxFloor;
        private int m_nMinFloor;

        public string EquipID
        {
            get { return m_strEquipID; }
            set { m_strEquipID = value; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }

        public string BuildingName
        {
            get { return m_strBuildingName; }
            set { m_strBuildingName = value; }
        }

        public int GroupID
        {
            get { return m_nGroupID; }
            set { m_nGroupID = value; }
        }

        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }

        public string SiteName
        {
            get { return m_strSiteName; }
            set { m_strSiteName = value; }
        }

        public int MaxFloor
        {
            get { return m_nMaxFloor; }
            set { m_nMaxFloor = value; }
        }

        public int MinFloor
        {
            get { return m_nMinFloor; }
            set { m_nMinFloor = value; }
        }



    }

    public class Data_NormalTeam
    {
        private int m_nID = -1;
        private string m_strTeamName = "";
        private Data_NormalTeam m_parentTeam = null;
        private string m_strGroupName = "";
        private int m_nLevelNo = -1;
        private string m_strDescription = "";
        private string m_strRegularTeamLink = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }
        public Data_NormalTeam ParentTeam
        {
            get { return m_parentTeam; }
            set { m_parentTeam = value; }
        }
        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }
        public int LevelNo
        {
            get { return m_nLevelNo; }
            set { m_nLevelNo = value; }
        }
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
        public string RegularTeamLink
        {
            get { return m_strRegularTeamLink; }
            set { m_strRegularTeamLink = value; }
        }
    }

    public class Data_EmergencyTeam
    {
        private int m_nID = -1;
        private string m_strTeamName = "";
        private Data_EmergencyTeam m_parentTeam = null;
        private string m_strGroupName = "";
        private int m_nLevelNo = -1;
        private string m_strDescription = "";
        private string m_strRegularTeamLink = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }
        public Data_EmergencyTeam ParentTeam
        {
            get { return m_parentTeam; }
            set { m_parentTeam = value; }
        }
        public string GroupName
        {
            get { return m_strGroupName; }
            set { m_strGroupName = value; }
        }
        public int LevelNo
        {
            get { return m_nLevelNo; }
            set { m_nLevelNo = value; }
        }
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
        public string RegularTeamLink
        {
            get { return m_strRegularTeamLink; }
            set { m_strRegularTeamLink = value; }
        }
    }

    class Data_CheckTask
    {
        private int m_nID;
        private int m_nProcessID;
        private string m_strCategory;
        private string m_strSubCategory;
        private string m_strTaskName;
        private int m_nTargetCount;
        private string m_strPosition;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public int ProcessID
        {
            get { return m_nProcessID; }
            set { m_nProcessID = value; }
        }
        public string Category
        {
            get { return m_strCategory; }
            set { m_strCategory = value; }
        }
        public string SubCategory
        {
            get { return m_strSubCategory; }
            set { m_strSubCategory = value; }
        }
        public string TaskName
        {
            get { return m_strTaskName; }
            set { m_strTaskName = value; }
        }
        public int TargetCount
        {
            get { return m_nTargetCount; }
            set { m_nTargetCount = value; }
        }
        public string Position
        {
            get { return m_strPosition; }
            set { m_strPosition = value; }
        }
    }

    public class Data_ActionStep
    {
        // PeriodType : 기간 Type : 0(사용 안함), 1(날짜 옵션, n1월 n2일 ~ m1월 m2일까지), 2(시간 옵션, n1시 n2분 ~ m1월 m2일까지), 3(날짜 옵션 + 시간 옵션),
        //                                      11(고정 년도 사용 + 날짜 옵션), 12(고정 년도 사용 + 시간 옵션), 13(고정 년도 사용 + 날짜 옵션 + 시간 옵션)
        // WeekDayOption : 요일 옵션(bit 연산), bit : 1(일요일), 2(월요일), 4(화요일), 8(수요일), 16(목요일), 32(금요일), 64(토요일)
        // Iteration : 반복 회수
        // IterationType : 반복 회수 옵션 : 0(전체 기간중 몇회), 1(년중 몇회), 2(월중 몇회), 3(주중 몇회), 4(하루중 몇회), 5(시간당 몇회)
        // ProcessTimeType : 처리시간 옵션, 0(개월), 1(주), 2(일), 3(시간), 4(분)

        private int m_nID;
        private string m_strStepName;
        private int m_nPeriodType;
        private DateTime m_dtBeginTime;
        private DateTime m_dtEndTime;
        private int m_nWeekdayOption = 127;
        private int m_nIteration;
        private int m_nIterationType;
        private int m_nProcessTime;
        private int m_nProcessTimeType = 5;
        private int m_nDisasterID;
        private int m_nParentStepID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string StepName
        {
            get { return m_strStepName; }
            set { m_strStepName = value; }
        }
        public int PeriodType
        {
            get { return m_nPeriodType; }
            set { m_nPeriodType = value; }
        }
        public DateTime BeginTime
        {
            get { return m_dtBeginTime; }
            set { m_dtBeginTime = value; }
        }
        public DateTime EndTime
        {
            get { return m_dtEndTime; }
            set { m_dtEndTime = value; }
        }
        public int WeekdayOption
        {
            get { return m_nWeekdayOption; }
            set { m_nWeekdayOption = value; }
        }
        public int Iteration
        {
            get { return m_nIteration; }
            set { m_nIteration = value; }
        }
        public int IterationType
        {
            get { return m_nIterationType; }
            set { m_nIterationType = value; }
        }
        public int ProcessTime
        {
            get { return m_nProcessTime; }
            set { m_nProcessTime = value; }
        }
        public int ProcessTimeType
        {
            get { return m_nProcessTimeType; }
            set { m_nProcessTimeType = value; }
        }
        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }
        public int ParentStepID
        {
            get { return m_nParentStepID; }
            set { m_nParentStepID = value; }
        }
    }

    class Data_UserDefinedTeam
    {
        private int m_nID;
        private string m_strTeamName;
        private string m_strPhoneNumber;
        private string m_strFaxNumber;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }
        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }
        public string FaxNumber
        {
            get { return m_strFaxNumber; }
            set { m_strFaxNumber = value; }
        }
    }

    public class Data_ExternalTeam
    {
        private int m_nID = -1;
        private string m_strTeamName = "";
        private string m_strPhoneNumber = "";
        private string m_strFaxNumber = "";

        public Data_ExternalTeam()
        {
        }

        public Data_ExternalTeam(int nID, string strTeamName, string strPhoneNumber, string strFaxNumber)
        {
            m_nID = nID;
            m_strTeamName = strTeamName;
            m_strPhoneNumber = strPhoneNumber;
            m_strFaxNumber = strFaxNumber;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }
        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }
        public string FaxNumber
        {
            get { return m_strFaxNumber; }
            set { m_strFaxNumber = value; }
        }
    }

    class Data_Version
    {
        private int m_nID;
        private int m_nRegular;
        private int m_nNormal;
        private DateTime m_dtCreateTime;
        private DateTime m_dtLastAccessTime;
        private string m_strVersionName;
        private int m_nOwnerID;
        private string m_strDescription;


        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        public int Regular
        {
            get { return m_nRegular; }
            set { m_nRegular = value; }
        }
        public int Normal
        {
            get { return m_nNormal; }
            set { m_nNormal = value; }
        }
        public DateTime CreateTime
        {
            get { return m_dtCreateTime; }
            set { m_dtCreateTime = value; }
        }
        public DateTime LastAccessTime
        {
            get { return m_dtLastAccessTime; }
            set { m_dtLastAccessTime = value; }
        }
        public string VersionName
        {
            get { return m_strVersionName; }
            set { m_strVersionName = value; }
        }
        public int OwnerID
        {
            get { return m_nOwnerID; }
            set { m_nOwnerID = value; }
        }
        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

    }




    public class DisasterInfo
    {
        private int m_nDisasterID = -1;
        private int m_nVersionID = -1;
        private ArrayList m_arrActionSteps = new ArrayList();

        public ActionStepInfo FindActionStep(int nActionStepID)
        {
            foreach (ActionStepInfo actionStep in m_arrActionSteps)
            {
                if (actionStep.ActionStepID == nActionStepID)
                    return actionStep;
            }

            return null;
        }

        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }

        public int VersionID
        {
            get { return m_nVersionID; }
            set { m_nVersionID = value; }
        }

        public ArrayList ActionSteps
        {
            get { return m_arrActionSteps; }
        }
    }

    public class ActionStepInfo
    {
        private int m_nActionStepID = -1;
        private string m_strActionStepName = "";
        private int m_nParentStepID = -1;
        private int m_nPeriodType = -1;
        private DateTime m_timeBegin;
        private DateTime m_timeEnd;
        private int m_nWeekdayOption = 127;
        private int m_nIteration = 1;
        private int m_nIterationType = 0;
        private int m_nProcessTime = 1;
        private int m_nProcessTimeType = 5;
        private int m_nDisasterID = -1;

        public Data_ActionStep ToData_ActionStep()
        {
            Data_ActionStep data = new Data_ActionStep();

            data.BeginTime = m_timeBegin;
            data.DisasterID = m_nDisasterID;
            data.EndTime = m_timeEnd;
            data.ID = m_nActionStepID;
            data.Iteration = m_nIteration;
            data.IterationType = m_nIterationType;
            data.ParentStepID = m_nParentStepID;
            data.PeriodType = m_nPeriodType;
            data.ProcessTime = m_nProcessTime;
            data.ProcessTimeType = m_nProcessTimeType;
            data.StepName = m_strActionStepName;
            data.WeekdayOption = m_nWeekdayOption;

            return data;
        }

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        public string ActionStepName
        {
            get { return m_strActionStepName; }
            set { m_strActionStepName = value; }
        }

        public int ParentStepID
        {
            get { return m_nParentStepID; }
            set { m_nParentStepID = value; }
        }

        public int PeriodType
        {
            get { return m_nPeriodType; }
            set { m_nPeriodType = value; }
        }

        public DateTime BeginTime
        {
            get { return m_timeBegin; }
            set { m_timeBegin = value; }
        }

        public DateTime EndTime
        {
            get { return m_timeEnd; }
            set { m_timeEnd = value; }
        }

        public int WeekDayOption
        {
            get { return m_nWeekdayOption; }
            set { m_nWeekdayOption = value; }
        }

        public int Iteration
        {
            get { return m_nIteration; }
            set { m_nIteration = value; }
        }

        public int IterationType
        {
            get { return m_nIterationType; }
            set { m_nIterationType = value; }
        }

        public int ProcessTime
        {
            get { return m_nProcessTime; }
            set { m_nProcessTime = value; }
        }

        public int ProcessTimeType
        {
            get { return m_nProcessTimeType; }
            set { m_nProcessTimeType = value; }
        }

        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }
    }

    public class VersionInfo
    {
        private int m_nVersionID = -1;
        private string m_strVersionName = "";
        private string m_strUserName = "";
        private DateTime m_dtBegin;
        private DateTime m_dtEnd;
        private string m_strDescription = "";
        private bool m_isRegular = true;    // 등록 모드인가?
        private bool m_isNormal = true;     // 평일 버전인가?

        public int VersionID
        {
            get { return m_nVersionID; }
            set { m_nVersionID = value; }
        }

        public string VersionName
        {
            get { return m_strVersionName; }
            set { m_strVersionName = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public DateTime BeginTime
        {
            get { return m_dtBegin; }
            set { m_dtBegin = value; }
        }

        public DateTime EndTime
        {
            get { return m_dtEnd; }
            set { m_dtEnd = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public bool IsRegular
        {
            get { return m_isRegular; }
            set { m_isRegular = value; }
        }

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }
    }

    public class ActionStepHistoryData
    {
        private string m_strActionStepPath = "";
        private int m_nActionStepID = -1;
        private int m_nActionStepHistoryID = -1;
        private bool m_isRealMode = true;
        private bool m_isRegular = true;
        private bool m_isNormal = true;
        private string m_strPosition = "";
        private ArrayList m_arrStepMemberList = new ArrayList();
        private ArrayList m_arrComponentHistoryList = new ArrayList();
        private TimeInfo m_timeBegin = null;
        private TimeInfo m_timeEnd = null;
        private TimeInfo m_timeCancel = null;
        private bool m_finishLog = false;

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

        // 등록모드 : true, 미등록모드 : false
        public bool RegularMode
        {
            get { return m_isRegular; }
            set { m_isRegular = value; }
        }

        // 주간모드 : true, 야간 및 휴일모드 : false
        public bool NormalMode
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
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

        public ArrayList StepMembers
        {
            get { return m_arrStepMemberList; }
        }

        public ArrayList ComponentHistoryList
        {
            get { return m_arrComponentHistoryList; }
        }

        public bool FinishLog
        {
            get { return m_finishLog; }
            set { m_finishLog = value; }
        }
    }

    public class ComponentHistoryData
    {
        public enum ComponentHistoryType { SECTION_TYPE = 0, MESSENGER_MESSAGE_TYPE, MONITORING_MESSAGE_TYPE };

        private ActionStepHistoryData m_actionStepHistory = null;
        private int m_nComponentHistoryID = -1;
        private DateTime m_time;
        private SectionData m_section = null;
        private ArrayList m_arrTeamList = new ArrayList();
        private string m_strTask = "";
        private string m_strStatus = "";
        private bool m_isVisible = true;
        private ComponentHistoryType m_type = ComponentHistoryType.SECTION_TYPE;
        // SOPGenUser ID
        private int m_nAccessedUserID = -1;

        public ActionStepHistoryData ActionStepHistory
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

        public SectionData Section
        {
            get { return m_section; }
            set { m_section = value; }
        }

        public ArrayList TeamList
        {
            get { return m_arrTeamList; }
        }

        public string Task
        {
            get { return m_strTask; }
            set { m_strTask = value; }
        }

        public string Status
        {
            get { return m_strStatus; }
            set { m_strStatus = value; }
        }

        public bool Visible
        {
            get { return m_isVisible; }
            set { m_isVisible = value; }
        }

        public ComponentHistoryType HistoryType
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
    }

    public class StepMemberData
    {
        private string m_strTeamName = "";
        private int m_nTeamID = -1;
        private int m_nTeamType = -1;
        private int m_nStepMemberID = -1;
        //private int m_nLevelNo = -1;

        //public StepMemberData()
        //{
        //    m_nTeamID = -1;
        //    m_strTeamName = "";
        //    m_nTeamType = -1;
        //    m_nStepMemberID = -1;
        //}

        public StepMemberData(string strTeamName, int nTeamID, int nTeamType)
        {
            m_strTeamName = strTeamName;
            m_nTeamID = nTeamID;
            m_nTeamType = nTeamType;
            m_nStepMemberID = -1;
            //m_nLevelNo = -1;
        }

        public StepMemberData(string strTeamName, int nTeamID, int nTeamType, int nStepMemberID)//, int nLevelNo)
        {
            m_strTeamName = strTeamName;
            m_nTeamID = nTeamID;
            m_nTeamType = nTeamType;
            m_nStepMemberID = nStepMemberID;
            //m_nLevelNo = nLevelNo;
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public int TeamType
        {
            get { return m_nTeamType; }
            set { m_nTeamType = value; }
        }

        public int StepMemberID
        {
            get { return m_nStepMemberID; }
            set { m_nStepMemberID = value; }
        }

        /*public int LevelNo
        {
            get { return m_nLevelNo; }
            set { m_nLevelNo = value; }
        }*/
    }

    public class SectionData
    {
        private int m_nID = -1;
        private ComponentType m_nSectionType = ComponentType.NONE;
        private StepMemberData m_stepMember = null;
        private string m_strText = "";
        private Sections.Section m_section = null;

        public enum State
        {
            NORMAL = 1,
            RUN = 2,
            DONE = 3,
            INPUT = 4,
            SKIP = 5
        }

        public enum ComponentType
        {
            PROCESS = 0, DECISION, ANNOTATION, ENDPOINT, LINK, TRANSSOP, INTERNAL, EXTERNAL, TRANSMISSION, NONE
        }

        public enum ProcessDirection
        {
            NONE = 0,
            TOP = 1,
            RIGHT = 2,
            BOTTOM = 4,
            LEFT = 8
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public ComponentType SectionType
        {
            get { return m_nSectionType; }
            set { m_nSectionType = value; }
        }

        public StepMemberData StepMember
        {
            get { return m_stepMember; }
            set { m_stepMember = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public Sections.Section Section
        {
            get { return m_section; }
            set { m_section = value; }
        }
    }

    public class ActionStepDetailLog
    {
        private int m_nHistoryID = -1;
        private bool m_isRealMode = true;
        private TimeInfo m_timeBegin = null;
        private TimeInfo m_timeEnd = null;
        private TimeInfo m_timeCancel = null;
        // long, 상위 4바이트(Component Type, Section.ComponentType), 하위 4바이트(Component ID)
        private Dictionary<long, Status> m_dicComponentStatus = new Dictionary<long, Status>();
        // Key : 상위 4바이트(Component Type, Section.ComponentType), 하위 4바이트(Component ID).
        // Value : Key에 해당하는 Component가 실행완료되었을 경우 진행률
        private Dictionary<long, int> m_dicProcessPercentage = new Dictionary<long, int>();
        private bool m_finishCalcPercentage = false;
        // (현재까지 완료된 임무 개수) / (총 임무 개수) 가 아니라
        // 종료 Section과 얼마나 가까운 곳의 임무가 완료되었는가에 따른 진행률 값
        private int m_nCurrentSectionNumberPercentage = 0;

        public enum Status { WAITING = 0, PROCESSING, COMPLETED, SKIPPED };

        public bool FinishCalcPercentage
        {
            get { return m_finishCalcPercentage; }
        }

        // (현재까지 완료된 임무 개수) / (총 임무 개수) 가 아니라
        // 종료 Section과 얼마나 가까운 곳의 임무가 완료되었는가에 따른 진행률 값
        public int CurrentSectionNumberPercentage
        {
            get { return m_nCurrentSectionNumberPercentage; }
        }

        public void SetMissionStatus(long nComponentID, Status status)
        {
            m_dicComponentStatus[nComponentID] = status;

            if (status == Status.COMPLETED)
            {
                int nPercent;

                if (m_dicProcessPercentage.TryGetValue(nComponentID, out nPercent))
                {
                    if (m_nCurrentSectionNumberPercentage < nPercent)
                        m_nCurrentSectionNumberPercentage = nPercent;
                }
            }
        }

        public void SetSectionNumber(long nComponentID, int nSectionNumber)
        {
            m_dicProcessPercentage[nComponentID] = nSectionNumber;
        }

        public void CalcProcessPercentage()
        {
            List<int> values = new List<int>();
            List<long> keys = new List<long>();

            foreach (KeyValuePair<long, int> pair in m_dicProcessPercentage)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }

            values.Sort();

            foreach (long key in keys)
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

            m_finishCalcPercentage = true;
        }

        private int GetStatusCount(Status status)
        {
            int nCount = 0;

            foreach (KeyValuePair<long, Status> pair in m_dicComponentStatus)
            {
                if (pair.Value == status)
                    nCount++;
            }

            return nCount;
        }

        public int HistoryID
        {
            get { return m_nHistoryID; }
            set { m_nHistoryID = value; }
        }

        public bool IsRealMode
        {
            get { return m_isRealMode; }
            set { m_isRealMode = value; }
        }

        public TimeInfo BeginTime
        {
            get { return m_timeBegin; }
            set { m_timeBegin = value; }
        }

        public TimeInfo EndTime
        {
            get { return m_timeEnd; }
            set
            {
                m_timeEnd = value;
                m_nCurrentSectionNumberPercentage = 100;
            }
        }

        public TimeInfo CancelTime
        {
            get { return m_timeCancel; }
            set { m_timeCancel = value; }
        }

        public int TotalMissionCount
        {
            get { return m_dicComponentStatus.Count; }
        }

        public int CompletedMissionCount
        {
            get { return GetStatusCount(Status.COMPLETED); }
        }

        public int ProcessingMissionCount
        {
            get { return GetStatusCount(Status.PROCESSING); }
        }

        public int SkippedMissionCount
        {
            get { return GetStatusCount(Status.SKIPPED); }
        }
    }

    public class TimeInfo
    {
        public DateTime m_time;

        public TimeInfo(DateTime time)
        {
            m_time = time;
        }
    }

    public class Commander
    {
        private int m_nMemberType = -1;
        private int m_nMemberID = -1;
        private string m_strCommanderName = "";

        public int MemberType
        {
            get { return m_nMemberType; }
            set { m_nMemberType = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        public string Name
        {
            get { return m_strCommanderName; }
            set { m_strCommanderName = value; }
        }
    }
}