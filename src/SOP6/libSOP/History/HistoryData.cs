using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Sections;
using DBUtility2;
using UnE.SOP;
using UnE.SOP.Sections;



namespace UnE
{ 
    namespace SOP
    {
        namespace History
        {
            public class ActionStepHistory
            {
                // ActionStepID(0보다 크면 실제 모드, 0보다 작으면 모의훈련모드), ActionStepUnitHistory List
                private Dictionary<int, ArrayList> m_dicHistory = new Dictionary<int, ArrayList>();
                
                internal void AddHistory(int nActionStepID, bool isRealMode, int nHistoryID, DateTime dtBegin, DateTime dtEnd, int nSensorZoneHistoryID)
                {
                    ArrayList arrHistory = null;

                    if (!isRealMode)
                        nActionStepID = -nActionStepID;

                    if (!m_dicHistory.ContainsKey(nActionStepID))
                    {
                        arrHistory = new ArrayList();
                        m_dicHistory[nActionStepID] = arrHistory;
                    }
                    else
                        arrHistory = m_dicHistory[nActionStepID];

                    ActionStepUnitHistory history = FindHistory(nHistoryID, arrHistory);
                    if (history == null)
                        arrHistory.Add(new ActionStepUnitHistory(nHistoryID, dtBegin, dtEnd, nSensorZoneHistoryID));
                }

                // SensorZoneHistoryID 추가로 사용하지 않음 . 2018-01-03 skkim
                //private void AddHistory(int nActionStepID, bool isRealMode, int nHistoryID, DateTime dtBegin, DateTime dtEnd)
                //{
                //    ArrayList arrHistory = null;

                //    if (!isRealMode)
                //        nActionStepID = -nActionStepID;

                //    if (!m_dicHistory.ContainsKey(nActionStepID))
                //    {
                //        arrHistory = new ArrayList();
                //        m_dicHistory[nActionStepID] = arrHistory;
                //    }
                //    else
                //        arrHistory = m_dicHistory[nActionStepID];

                //    ActionStepUnitHistory history = FindHistory(nHistoryID, arrHistory);
                //    if (history == null)
                //        arrHistory.Add(new ActionStepUnitHistory(nHistoryID, dtBegin, dtEnd));
                //}

                public int GetCompletedCount(int nActionStepID, bool isRealMode)
                {
                    if (!isRealMode)
                        nActionStepID = -nActionStepID;

                    if (m_dicHistory.ContainsKey(nActionStepID))
                    {
                        ArrayList arrHistory = m_dicHistory[nActionStepID];
                        return arrHistory.Count;
                    }

                    return 0;
                }

                private ActionStepUnitHistory FindHistory(int nHistoryID, ArrayList arrHistory)
                {
                    foreach (ActionStepUnitHistory history in arrHistory)
                    {
                        if (history.ID == nHistoryID)
                            return history;
                    }

                    return null;
                }

                public ActionStepUnitHistory GetHistory(int nActionStepID, bool isRealMode, int nIndex)
                {
                    if (!isRealMode)
                        nActionStepID = -nActionStepID;

                    if (m_dicHistory.ContainsKey(nActionStepID))
                    {
                        ArrayList arrHistory = m_dicHistory[nActionStepID];
                        if (nIndex >= arrHistory.Count)
                            return null;

                        return (ActionStepUnitHistory)arrHistory[nIndex];
                    }

                    return null;
                }

            }

            public class ActionStepUnitHistory
            {
                private int m_nHistoryID = -1;
                private DateTime m_timeBegin;
                private DateTime m_timeEnd;

                private ActionStepUnitHistory(int nHistoryID, DateTime dtBegin, DateTime dtEnd)
                {
                    m_nHistoryID = nHistoryID;
                    m_timeBegin = dtBegin;
                    m_timeEnd = dtEnd;
                }

                public ActionStepUnitHistory(int nHistoryID, DateTime dtBegin, DateTime dtEnd, int nSensorZoneHistoryID)
                {
                    m_nHistoryID = nHistoryID;
                    m_timeBegin = dtBegin;
                    m_timeEnd = dtEnd;

                    m_nSensorZoneHistoryID = nSensorZoneHistoryID;
                }


                public int ID
                {
                    get { return m_nHistoryID; }
                    set { m_nHistoryID = value; }
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

                private int m_nSensorZoneHistoryID = -1;
                public int SensorZoneHistoryID
                {
                    get { return m_nSensorZoneHistoryID; }
                    set { m_nSensorZoneHistoryID = value; }
                }

            }



            public class HistoryActionStepData
            {
                private int m_nActionStepHistoryID = -1;
                // SOP에 대한 재난 위치 정보
                private HistoryDisasterPosition m_Position = null;
                // SOP에 대한 재난위치 이외의 정보
                private HistoryDisasterNoPosition m_noPosition = null;

                private DateTime m_time;
                private Workstate.WorkFlowState m_state;
                private ActionStepInfo m_actionStep = null;
                private bool m_isRealMode = true;
                // 이 값이 true이면 DB에 기록하지 않고 Log 창에만 표시한다.
                private bool m_noDBWrite = false;
                // m_sectionSelected를 기록하기 위한 데이터인가?
                private bool m_sectionSelectedData = false;
                // 현재 선택된 Section
                private Section m_sectionSelected = null;

                private bool m_bSendSMS = false;

                private string m_strAmountSnowfall = "";

                protected string m_szHistoryDataName = "ActionStepHistory";

                public int ActionStepHistoryID
                {
                    get { return m_nActionStepHistoryID; }
                    set { m_nActionStepHistoryID = value; }
                }

                public string HistoryDataName
                {
                    get { return m_szHistoryDataName; }
                }

                public HistoryActionStepData(DateTime time, Workstate.WorkFlowState state, ActionStepInfo actionStep, bool isRealMode, bool bSendSMS)
                {
                    m_time = time;
                    m_state = state;
                    m_actionStep = actionStep;
                    m_isRealMode = isRealMode;
                    m_bSendSMS = bSendSMS;
                }

                public DateTime Time
                {
                    get { return m_time; }
                    set { m_time = value; }
                }

                public Workstate.WorkFlowState State
                {
                    get { return m_state; }
                    set { m_state = value; }
                }

                public ActionStepInfo ActionStep
                {
                    get { return m_actionStep; }
                    set { m_actionStep = value; }
                }

                public bool IsRealMode
                {
                    get { return m_isRealMode; }
                    set { m_isRealMode = value; }
                }

                public bool NoDBWrite
                {
                    get { return m_noDBWrite; }
                    set { m_noDBWrite = value; }
                }

                /// <summary>
                /// SOP에 대한 재난 위치 정보
                /// </summary>
                public HistoryDisasterPosition Position
                {
                    get { return m_Position; }
                    set { m_Position = value; }
                }

                /// <summary>
                /// SOP에 대한 재난위치 이외의 정보
                /// </summary>
                public HistoryDisasterNoPosition NoPosition
                {
                    get { return m_noPosition; }
                    set { m_noPosition = value; }
                }

                public Section SelectedSection
                {
                    get { return m_sectionSelected; }
                    set { m_sectionSelected = value; }
                }

                // m_sectionSelected를 기록하기 위한 데이터인가?
                public bool SectionSelectedData
                {
                    get { return m_sectionSelectedData; }
                    set { m_sectionSelectedData = value; }
                }

                public bool SendSMS
                {
                    get { return m_bSendSMS; }
                    set { m_bSendSMS = value; }
                }

                public bool UseAmountSnowfall
                {
                    get { return m_strAmountSnowfall == null || m_strAmountSnowfall.Length == 0 ? false : true; }
                }

                public string AmountSnowfall
                {
                    get { return m_strAmountSnowfall; }
                    set { m_strAmountSnowfall = value; }
                }

                private int m_nSensorZoneHistoryID = -1;
                public int SensorZoneHistoryID
                {
                    get { return m_nSensorZoneHistoryID; }
                    set { m_nSensorZoneHistoryID = value; }
                }

            }


            public class HistoryActionStepDataEx : HistoryActionStepData
            {
                public HistoryActionStepDataEx(DateTime time, Workstate.WorkFlowState state, ActionStepInfo actionStep, bool isRealMode, bool bSendSMS)
                    : base(time, state, actionStep, isRealMode, bSendSMS)
                {
                    m_szHistoryDataName = "ActionStepHistoryAutoClose";

                }
            }

            public class HistorySectionData
            {
                public class DetailData
                {
                    public enum DataType { COMPLETE_CHECKED = 1, SEND_SMS, SEND_BROADCAST, COMPLETE_UNCHECKED };

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

                    private VariousData<int> m_datai = null;
                    private VariousData<float> m_dataf = null;
                    private string m_datas = null;
                    private VariousData<DateTime> m_time = null;
                    private VariousData<int> m_dataIndex = null;
                    private int m_nComponentHistoryID = -1;

                    public VariousData<int> Datai
                    {
                        get { return m_datai; }
                        set { m_datai = value; }
                    }

                    public VariousData<float> Dataf
                    {
                        get { return m_dataf; }
                        set { m_dataf = value; }
                    }

                    public string Datas
                    {
                        get { return m_datas; }
                        set { m_datas = value; }
                    }

                    public VariousData<DateTime> Time
                    {
                        get { return m_time; }
                        set { m_time = value; }
                    }

                    public VariousData<int> DataIndex
                    {
                        get { return m_dataIndex; }
                        set { m_dataIndex = value; }
                    }

                    public int ComponentHistoryID
                    {
                        get { return m_nComponentHistoryID; }
                        set { m_nComponentHistoryID = value; }
                    }

                    public DetailData()
                    {
                    }

                    public DetailData(int data)
                    {
                        m_datai = new VariousData<int>(data);
                    }

                    public DetailData(float data)
                    {
                        m_dataf = new VariousData<float>(data);
                    }

                    public DetailData(string data)
                    {
                        m_datas = data;
                    }

                    public DetailData(int data, DateTime time)
                    {
                        m_datai = new VariousData<int>(data);
                        m_time = new VariousData<DateTime>(time);
                    }

                    public DetailData(float data, DateTime time)
                    {
                        m_dataf = new VariousData<float>(data);
                        m_time = new VariousData<DateTime>(time);
                    }

                    public DetailData(string data, DateTime time)
                    {
                        m_datas = data;
                        m_time = new VariousData<DateTime>(time);
                    }

                    public override bool Equals(object obj)
                    {
                        if (obj == null)
                            return false;

                        if (obj is DetailData)
                        {
                            DetailData detail = (DetailData)obj;

                            if ((this.DataIndex == null && detail.DataIndex != null) || (this.DataIndex != null && detail.DataIndex == null))
                                return false;
                            else if (this.DataIndex != null && detail.DataIndex != null && this.DataIndex.Data != detail.DataIndex.Data)
                                return false;

                            if ((this.Datai == null && detail.Datai != null) || (this.Datai != null && detail.Datai == null))
                                return false;
                            else if (this.Datai != null && detail.Datai != null && this.Datai.Data != detail.Datai.Data)
                                return false;

                            if ((this.Dataf == null && detail.Dataf != null) || (this.Dataf != null && detail.Dataf == null))
                                return false;
                            else if (this.Dataf != null && detail.Dataf != null && this.Dataf.Data != detail.Dataf.Data)
                                return false;

                            if ((this.Datas == null && detail.Datas != null) || (this.Datas != null && detail.Datas == null))
                                return false;
                            else if (this.Datas != null && detail.Datas != null && this.Datas != detail.Datas)
                                return false;

                            if ((this.Time == null && detail.Time != null) || (this.Time != null && detail.Time == null))
                                return false;
                            else if (this.Time != null && detail.Time != null && this.Time.Data != detail.Time.Data)
                                return false;

                            return true;
                        }

                        return false;
                    }
                }

                private DateTime m_time;
                private Workstate.State m_state;
                private int m_nProcessDirections;
                private Section m_section = null;
                // 이 값이 true이면 DB에 기록하지 않고 Log 창에만 표시한다.
                private bool m_noDBWrite = false;
                // 상황판에 기록하는가?
                private bool m_showBoard = false;
                // 첫번째 실행옵션에 대한 BitFlag
                // Process의 경우 MissionItem들의 SMS에 대한 실행 여부
                // 상황전파의 경우 Popup Message(0), SMS(1), 방송(2) 순서
                private int m_nCheckNotify1 = 0;
                // 두번째 실행옵션에 대한 BitFlag
                // Process의 경우 MissionItem들의 방송에 대한 실행 여부
                // 상황전파의 경우 사용하지 않음
                private int m_nCheckNotify2 = 0;
                private int m_nCheckedRun = 0;
                private int m_nCheckedComplete = 0;
                private int m_nComponentHistoryID = -1;
                // Key : ComponentHistoryID
                private Dictionary<int, List<DetailData>> m_dicHistoryDetail = new Dictionary<int, List<DetailData>>();

                public HistorySectionData(DateTime time, Workstate.State state, int nProcessDirections, Section section)
                {
                    m_time = time;
                    m_state = state;
                    m_section = section;
                    m_nProcessDirections = nProcessDirections;
                }

                public DateTime Time
                {
                    get { return m_time; }
                    set { m_time = value; }
                }

                public Workstate.State State
                {
                    get { return m_state; }
                    set { m_state = value; }
                }

                public int ProcessDirections
                {
                    get { return m_nProcessDirections; }
                    set { m_nProcessDirections = value; }
                }

                public Section Section
                {
                    get { return m_section; }
                    set { m_section = value; }
                }

                public bool NoDBWrite
                {
                    get { return m_noDBWrite; }
                    set { m_noDBWrite = value; }
                }

                public bool ShowBoard
                {
                    get { return m_showBoard; }
                    set { m_showBoard = value; }
                }

                // 첫번째 실행옵션에 대한 BitFlag
                // Process의 경우 MissionItem들의 SMS에 대한 실행 여부
                // 상황전파의 경우 Popup Message(0), SMS(1), 방송(2) 순서
                public int CheckNotify1
                {
                    get { return m_nCheckNotify1; }
                    set { m_nCheckNotify1 = value; }
                }

                // 두번째 실행옵션에 대한 BitFlag
                // Process의 경우 MissionItem들의 방송에 대한 실행 여부
                // 상황전파의 경우 사용하지 않음
                public int CheckNotify2
                {
                    get { return m_nCheckNotify2; }
                    set { m_nCheckNotify2 = value; }
                }

                public int CheckedRun
                {
                    get { return m_nCheckedRun; }
                    set { m_nCheckedRun = value; }
                }

                public int CheckedComplete
                {
                    get { return m_nCheckedComplete; }
                    set { m_nCheckedComplete = value; }
                }

                public int ComponentHistoryID
                {
                    get { return m_nComponentHistoryID; }
                    set { m_nComponentHistoryID = value; }
                }

                // Key : ComponentHistoryID
                public Dictionary<int, List<DetailData>> HistoryDetailDatas
                {
                    get { return m_dicHistoryDetail; }
                }
            }

            public class HistorySectionDecisionData : HistorySectionData
            {
                // SectionDecision만 사용하며, 분기 다음에 선택된 Section이 어느것인지 알려준다.
                private Section m_sectionNext = null;

                public HistorySectionDecisionData(DateTime time, Workstate.State state, int nProcessDirections, Section section, Section sectionNext)
                    : base(time, state, nProcessDirections, section)
                {
                    m_sectionNext = sectionNext;
                }

                public Section NextSection
                {
                    get { return m_sectionNext; }
                    set { m_sectionNext = value; }
                }
            }

            public class HistorySectionInternalData : HistorySectionData
            {
                private bool m_usePopupMessage = false;
                private bool m_useSMS = false;
                private bool m_useBroadcast = false;

                public HistorySectionInternalData(DateTime time, Workstate.State state, int nProcessDirections, Section section, bool usePopupMessage, bool useSMS, bool useBroadcast)
                    : base(time, state, nProcessDirections, section)
                {
                    m_usePopupMessage = usePopupMessage;
                    m_useSMS = useSMS;
                    m_useBroadcast = useBroadcast;
                }

                public bool UsePopupMessage
                {
                    get { return m_usePopupMessage; }
                    set { m_usePopupMessage = value; }
                }

                public bool UseSMS
                {
                    get { return m_useSMS; }
                    set { m_useSMS = value; }
                }

                public bool UseBroadcast
                {
                    get { return m_useBroadcast; }
                    set { m_useBroadcast = value; }
                }
            }

            public class HistorySectionExternalData : HistorySectionData
            {
                private bool m_useSMS = false;
                private bool m_useFax = false;

                public HistorySectionExternalData(DateTime time, Workstate.State state, int nProcessDirections, Section section, bool useSMS, bool useFax = false)
                    : base(time, state, nProcessDirections, section)
                {
                    m_useSMS = useSMS;
                }

                public bool UseSMS
                {
                    get { return m_useSMS; }
                    set { m_useSMS = value; }
                }

                public bool UseFax
                {
                    get { return m_useFax; }
                    set { m_useFax = value; }
                }
            }

            public class HistorySectionTransmissionData : HistorySectionData
            {
                private bool m_usePopupMessage = false;
                private bool m_useSMS = false;
                private bool m_useBroadcast = false;
                private bool m_useExSMS = false;
                private bool m_useExFax = false;

                public HistorySectionTransmissionData(DateTime time, Workstate.State state, int nProcessDirections, Section section, bool usePopupMessage, bool useSMS, bool useBroadcast, bool useExSMS, bool useExFax = false)
                    : base(time, state, nProcessDirections, section)
                {
                    m_usePopupMessage = usePopupMessage;
                    m_useSMS = useSMS;
                    m_useBroadcast = useBroadcast;
                    m_useExSMS = useExSMS;
                }

                public bool UsePopupMessage
                {
                    get { return m_usePopupMessage; }
                    set { m_usePopupMessage = value; }
                }

                public bool UseSMS
                {
                    get { return m_useSMS; }
                    set { m_useSMS = value; }
                }

                public bool UseBroadcast
                {
                    get { return m_useBroadcast; }
                    set { m_useBroadcast = value; }
                }

                public bool UseExSMS
                {
                    get { return m_useExSMS; }
                    set { m_useExSMS = value; }
                }

                public bool UseExFax
                {
                    get { return m_useExFax; }
                    set { m_useExFax = value; }
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

                public enum Status { WAITING = 0, PROCESSING, COMPLETED, SKIPPED };

                public void SetMissionStatus(long nComponentID, Status status)
                {
                    m_dicComponentStatus[nComponentID] = status;
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
                    set { m_timeEnd = value; }
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

                private int m_nSensorZoneHistoryID = -1;
                public int SensorZoneHistoryID
                {
                    get { return m_nSensorZoneHistoryID; }
                    set { m_nSensorZoneHistoryID = value; }
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
            
        }
    }
}
