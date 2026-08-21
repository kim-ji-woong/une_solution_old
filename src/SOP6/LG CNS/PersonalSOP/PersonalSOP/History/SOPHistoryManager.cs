using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBUtility2;
using System.Threading;
using System.Collections;
using System.Collections.Concurrent;

namespace PersonalSOP.History
{
    using Models;

    public class SOPHistoryManager
    {
        public enum ComponentState
        {
            //대기상태(1), 실행중(2), 완료(3), 입력대기상태(4), 건너뜀 상태(5)
            STANDBY = 1,
            RUN = 2,
            DONE = 3,
            WAIT = 4,
            IGNORE = 5
        }

        private WebDBManager m_dbMgr = null;
        private bool m_shutdownThread = false;

        private static SOPHistoryManager m_instance = null;

        private ConcurrentDictionary<int, ActionStepHistory> m_dicActionStepHistories = new ConcurrentDictionary<int, ActionStepHistory>();
        // 하나의 ActionStep 안에 있는 Section들의 배치순서
        // Key : ActionStepID
        // Value : Key => 상위 4바이트(ComponentType) + 하위 4바이트(각 Section Component들의 DB ID)
        //         Value => 배치 순서
        private ConcurrentDictionary<int, Dictionary<long, int>> m_dicActionStepSectionsOrder = new ConcurrentDictionary<int, Dictionary<long, int>>();
        private SOPHistory m_sopHistory = new SOPHistory();

        public List<ActionStepHistory> ActionStepHistories
        {
            get { return m_dicActionStepHistories.Values.ToList(); }
        }

        public SOPHistory SOPHistory
        {
            get { return m_sopHistory; }
        }

        public static SOPHistoryManager Instance
        {
            get { return m_instance; }
        }

        public static void InitInstance()
        {
            m_instance = new SOPHistoryManager();
        }

        private SOPHistoryManager()
        {
            m_dbMgr = Network.NetworkWebManager.Instance.DBMgr;
            m_instance = this;

            Thread t = new Thread(new ThreadStart(MonitoringThread));
            t.Start();
        }

        private void MonitoringThread()
        {
            //bool isClosed;

            while (m_shutdownThread == false)
            {
                Secretary.LoadCurrentActionStepHistoryList(m_dbMgr, this);
                Secretary.FindNewActionStepHistory(m_dbMgr, this);

                int nActionStepID;
                bool isRealMode;

                if (Secretary.GetCurrentActionStepID(m_dbMgr, out nActionStepID, out isRealMode))
                    SetCurrentActionStep(nActionStepID, isRealMode);
                /*int nCurrentActionStepHistoryID = ReadCurrentActionStepHistoryID(out isClosed);

                if (nCurrentActionStepHistoryID > 0)
                {
                    ReadActionStepHistoryInfo(nCurrentActionStepHistoryID, ref m_currentHistory);

                    if (m_currentHistory != null)
                        ReadComponentHistory(m_currentHistory);
                }*/

                Thread.Sleep(1000);
            }
        }

        private void SetCurrentActionStep(int nActionStepID, bool isRealMode)
        {
            ActionStepHistory actionStepHistory = GetActionStepHistory(nActionStepID, isRealMode);

            //if (m_currentActionStepHistory == actionStepHistory)
            //    m_ignoreAutoChangeActionStep = false;

            if (actionStepHistory == null)
                return;

            if (NeedChangeActionStep(m_sopHistory.ActionStepHistory, actionStepHistory))
            {
                //if (m_ignoreAutoChangeActionStep)
                //    return;

                ChangeActionStepHistory(actionStepHistory);
            }
        }

        private bool NeedChangeActionStep(ActionStepHistory currentActionStepHistory, ActionStepHistory newActionStepHistory)
        {
            if (currentActionStepHistory != newActionStepHistory)
                return true;

            if (currentActionStepHistory == newActionStepHistory)
            {
                if (m_sopHistory.HistoryDataCount == 0)
                {
                    if (newActionStepHistory != null)
                        return true;
                }
                else
                {
                    if (m_sopHistory.SOPName.Length == 0)
                    {
                        if (newActionStepHistory != null)
                            return true;
                    }
                    else
                    {
                        string strCurrent = m_sopHistory.SOPName;
                        string strNew = newActionStepHistory == null ? "" : newActionStepHistory.ActionStepPath;

                        RemoveAddition(ref strCurrent);
                        RemoveAddition(ref strNew);

                        if (strCurrent != strNew)
                            return true;
                    }
                }
            }

            return false;
        }

        // (실제상황)과 같은 부분이 ActionStep 이름에 첨가되어 있으면 제거한다.
        private void RemoveAddition(ref string strActionStepName)
        {
            int nSlashIndex = strActionStepName.LastIndexOf('/');
            int nBracketIndex = strActionStepName.LastIndexOf('(');

            if (nBracketIndex < 0 || nBracketIndex < nSlashIndex)
                return;

            strActionStepName = strActionStepName.Substring(0, nBracketIndex);
        }

        private void AddComponentHistory(ComponentHistory componentHistory)
        {
            if (componentHistory == null)
                return;

            SOPHistoryData data = new SOPHistoryData();
            m_sopHistory.AddHistoryData(data);

            UpdateComponentHistory(data, componentHistory);
        }

        private void UpdateComponentHistory(SOPHistoryData data, ComponentHistory componentHistory, string strTaskAdd = "")
        {
            data.No = m_sopHistory.HistoryDataCount + 1;
            data.SetTime(componentHistory.Time);
            componentHistory.Time = data.GetTime();
            //data.Time = MakeLogTimeString(componentHistory.Time);
            data.Task = strTaskAdd + componentHistory.Task;
            data.State = ComponentHistory.ToHistoryTypeString(componentHistory.Type);
            data.ComponentHistory = componentHistory;
        }

        public void UpdateData()
        {
            if (m_sopHistory.ActionStepHistory != null && m_sopHistory.ActionStepHistory.EndTime == null && m_sopHistory.ActionStepHistory.CancelTime == null)
                UpdateElapsedTime(m_sopHistory.ActionStepHistory.BeginTime, DateTime.Now);
        }

        public static string MakeLogTimeString(DateTime time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", time.Hour, time.Minute, time.Second);
        }

        private SOPHistoryData UpdateComponentHistory(ActionStepHistory actionStepHistory, ComponentHistory oldHistory, ComponentHistory newHistory)
        {
            if (oldHistory == null || newHistory == null)
                return null;

            List<SOPHistoryData> datas = m_sopHistory.HistoryDatas;

            foreach (SOPHistoryData data in datas)
            {
                if (data.ComponentHistory != null)
                {
                    if (data.ComponentHistory == oldHistory)
                    {
                        UpdateComponentHistory(actionStepHistory, oldHistory, newHistory, data);
                        return data;
                    }
                }
            }

            if (actionStepHistory == m_sopHistory.ActionStepHistory)
            {
                SOPHistoryData data2 = new SOPHistoryData();
                m_sopHistory.AddHistoryData(data2);

                UpdateComponentHistory(actionStepHistory, oldHistory, newHistory, data2);
                return data2;
            }
            else
            {
                UpdateComponentHistory(actionStepHistory, oldHistory, newHistory, null);
                return null;
            }
        }

        private void UpdateComponentHistory(ActionStepHistory actionStepHistory, ComponentHistory oldHistory, ComponentHistory newHistory, SOPHistoryData data)
        {
            //oldHistory.Time = newHistory.Time;
            oldHistory.Commander = newHistory.Commander;
            oldHistory.Receiver = newHistory.Receiver;
            oldHistory.Task = newHistory.Task;
            oldHistory.Type = newHistory.Type;

            AddDetailHistory(actionStepHistory, oldHistory, newHistory);

            if (data != null)
                UpdateComponentHistory(data, oldHistory);

        }

        private ActionStepHistory GetActionStepHistory(int nActionStepID, bool isRealMode)
        {
            //lock (LockActionStepInfo)
            {
                List<ActionStepHistory> actionStepHistories = m_dicActionStepHistories.Values.ToList();
                int nHistoryCount = actionStepHistories.Count;

                for (int i = 0; i < nHistoryCount; i++)
                {
                    ActionStepHistory history = actionStepHistories[i];

                    if (history.ActionStepID == nActionStepID && history.RealMode == isRealMode)
                    {
                        return history;
                    }
                }
            }

            return null;
        }

        public void AddComponentHistory(ActionStepHistory actionStepHistory, ComponentHistory componentHistory)
        {
            ComponentHistory oldHistory = FindComponentHistory(componentHistory.SectionState, actionStepHistory);

            //lock (LockLog)
            {
                if (oldHistory != null)
                {
                    if (componentHistory.IsDetailLog)
                    {
                        AddDetailHistory(actionStepHistory, oldHistory, componentHistory);
                    }
                    else
                    {
                        UpdateComponentHistory(actionStepHistory, oldHistory, componentHistory);
                    }
                }
                else
                {
                    actionStepHistory.ComponentHistories.Add(componentHistory);

                    if (actionStepHistory == m_sopHistory.ActionStepHistory)
                    {
                        AddDetailHistory(actionStepHistory, componentHistory, componentHistory.Clone());
                        AddComponentHistory(componentHistory);
                    }
                }
            }

            if (componentHistory.Type == ComponentHistory.HistoryType.COMPLETE_MISSION)
                actionStepHistory.SetCompleteSectionState(componentHistory.SectionState);
        }

        private bool AddDetailHistory(ActionStepHistory actionStepHistory, ComponentHistory historyParent, ComponentHistory historyDetail)
        {
            foreach (ComponentHistory history in historyParent.AllHistories)
            {
                // 이미 같은 로그가 존재하는지 검사
                if (history.Time == historyDetail.Time && history.Task == historyDetail.Task && history.Type == historyDetail.Type)
                    return false;
            }

            ComponentHistory.HistoryType lastHistoryType = GetLastNotDetailLogHistoryType(historyParent);

            // 마지막 상태와 같은 값을 가진 로그는 무시한다.
            /*if (historyDetail.Type == lastHistoryType)
                return false;*/
            /*// 마지막 상태가 [임무확인]일 경우 다시 [임무확인] 로그가 들어오면 무시한다.
            if (historyDetail.Type == ComponentHistory.HistoryType.CONFIRM_MISSION &&
                GetLastNotDetailLogHistoryType(historyParent) == ComponentHistory.HistoryType.CONFIRM_MISSION)
                return false;*/

            AddComponentHistoryDetail(actionStepHistory, historyParent, historyDetail);
            return true;
        }

        private void AddComponentHistoryDetail(ActionStepHistory actionStepHistory, ComponentHistory historyParent, ComponentHistory historyDetail)
        {
            SOPHistoryData data = FindSOPHistoryData(historyParent);

            if (data == null)
            {
                if (actionStepHistory != m_sopHistory.ActionStepHistory)
                {
                    historyParent.AllHistories.Add(historyDetail);
                }
                return;
            }
            else
            {
                historyParent.AllHistories.Add(historyDetail);
            }

            if (data.ShowingDetails)
            {
                int nDetailHistoryCount = historyParent.AllHistories.Count;
                int nIndex = data.GetIndex(m_sopHistory) + nDetailHistoryCount - 1;

                SOPHistoryData detailData = new SOPHistoryData();
                data.DetailDatas.Add(detailData);

                UpdateComponentHistory(detailData, historyDetail);
            }
        }

        private SOPHistoryData FindSOPHistoryData(ComponentHistory history)
        {
            List<SOPHistoryData> datas = m_sopHistory.HistoryDatas;

            foreach (SOPHistoryData data in datas)
            {
                if (data.ComponentHistory == history)
                    return data;
            }

            return null;
        }

        private ComponentHistory.HistoryType GetLastNotDetailLogHistoryType(ComponentHistory componentHistory)
        {
            int nCount = componentHistory.AllHistories.Count;

            for (int i = nCount - 1; i >= 0; i--)
            {
                ComponentHistory history = componentHistory.AllHistories[i];

                if (history.IsDetailLog)
                    continue;

                return history.Type;
            }

            return ComponentHistory.HistoryType.NONE;
        }

        private ComponentHistory FindComponentHistory(SectionState sectionState, ActionStepHistory actionStepHistory)
        {
            if (sectionState == null)
                return null;

            foreach (ComponentHistory componentHistory in actionStepHistory.ComponentHistories)
            {
                if (componentHistory.SectionState == sectionState)
                    return componentHistory;
            }

            return null;
        }

        public void LoadActionStepHistory(ActionStepHistory actionStepHistory)
        {
            Dictionary<int, ActionStepHistory> dicActionStepHistory = new Dictionary<int, ActionStepHistory>();
            dicActionStepHistory[actionStepHistory.ActionStepHistoryID] = actionStepHistory;

            ActionStepHistory prev = m_sopHistory.ActionStepHistory;

            AddActionStepHistory(actionStepHistory);

            Secretary.AddActionStepSections(m_dbMgr, new IOManager(), actionStepHistory.ActionStepID, actionStepHistory);
            Secretary.LoadComponentHistory(this, m_dbMgr, actionStepHistory.ActionStepHistoryID.ToString(), dicActionStepHistory, true);

            if (prev != actionStepHistory)
            {
                ChangeActionStepHistory(actionStepHistory);
            }
        }

        public void AddActionStepHistory(ActionStepHistory actionStepHistory)
        {
            m_dicActionStepHistories[actionStepHistory.ActionStepHistoryID] = actionStepHistory;
            ChangeActionStepHistory(actionStepHistory);
        }

        public void RemoveActionStepHistory(ActionStepHistory actionStepHistory)
        {
            ActionStepHistory temp;
            m_dicActionStepHistories.TryRemove(actionStepHistory.ActionStepHistoryID, out temp);

            List<ActionStepHistory> actionStepHistories = this.ActionStepHistories;

            if (actionStepHistories.Count > 0)
                ChangeActionStepHistory(actionStepHistories.Last());
        }

        private void ChangeActionStepHistory(ActionStepHistory actionStepHistory)
        {
            m_sopHistory.ActionStepHistory = actionStepHistory;

            if (actionStepHistory != null)
            {
                m_sopHistory.SOPName = actionStepHistory.ToString();
                m_sopHistory.SOPInfo = actionStepHistory.Position;
                m_sopHistory.BeginTime = actionStepHistory.BeginTime.m_time;

                if (actionStepHistory != null)
                {
                    if (actionStepHistory.EndTime == null && actionStepHistory.CancelTime == null)
                        UpdateElapsedTime(actionStepHistory.BeginTime, DateTime.Now);
                    else if (actionStepHistory.EndTime != null)
                        UpdateElapsedTime(actionStepHistory.BeginTime, actionStepHistory.EndTime.m_time);
                    else if (actionStepHistory.CancelTime != null)
                        UpdateElapsedTime(actionStepHistory.BeginTime, actionStepHistory.CancelTime.m_time);
                }
            }
            else
            {
                m_sopHistory.SOPName = "";
                m_sopHistory.SOPInfo = "";
                m_sopHistory.BeginTime = new DateTime();
            }

            m_sopHistory.ClearHistoryDatas();

            if (actionStepHistory != null)
            {
                List<ComponentHistory> histroies = new List<ComponentHistory>();
                histroies.AddRange(actionStepHistory.ComponentHistories);

                foreach (ComponentHistory componentHistory in histroies)
                {
                    AddComponentHistory(actionStepHistory, componentHistory);
                }
            }
        }

        public void CompleteActionStepHistory(ActionStepHistory actionStepHistory)
        {
            if (!HasEndComponentHistory(actionStepHistory))
            {
                ComponentHistory history = new ComponentHistory();

                history.ActionStepHistory = actionStepHistory;
                history.Task = "SOP 종료";
                history.Time = actionStepHistory.EndTime == null ? DateTime.Now : actionStepHistory.EndTime.m_time;
                history.Type = ComponentHistory.HistoryType.COMPLETE_MISSION;
                history.SectionState = Secretary.MakeEndPointSectionState(false);

                AddComponentHistory(actionStepHistory, history);
            }
        }

        public void CancelActionStepHistory(ActionStepHistory actionStepHistory)
        {
            if (!HasEndComponentHistory(actionStepHistory))
            {
                ComponentHistory history = new ComponentHistory();

                history.ActionStepHistory = actionStepHistory;
                history.Task = "SOP 실행취소";
                history.Time = actionStepHistory.CancelTime == null ? DateTime.Now : actionStepHistory.CancelTime.m_time;
                history.Type = ComponentHistory.HistoryType.COMPLETE_MISSION;
                history.SectionState = Secretary.MakeEndPointSectionState(false);
                history.Commander = actionStepHistory.CommanderName;

                AddComponentHistory(actionStepHistory, history);
            }
        }

        private bool HasEndComponentHistory(ActionStepHistory actionStepHistory)
        {
            foreach (ComponentHistory history in actionStepHistory.ComponentHistories)
            {
                if (history.SectionState != null && history.SectionState.Section != null &&
                    history.SectionState.Section.GetComponentType() == Sections.Section.ComponentType.ENDPOINT &&
                    ((Sections.SectionDataEndPoint)history.SectionState.Section.Data).IsBegin == false)
                    return true;
            }

            return false;
        }

        private void UpdateElapsedTime(TimeInfo timeBegin, DateTime dtCurrent)
        {
            if (timeBegin == null)
                m_sopHistory.ElapsedSeconds = 0;
            else
            {
                TimeSpan span = dtCurrent - timeBegin.m_time;
                m_sopHistory.ElapsedSeconds = (int)span.TotalSeconds;
            }
        }

        /*private void ReadComponentHistory(ActionStepHistory history)
        {
            string strFormat = "Select ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, CompleteCount";
            strFormat += ", ShowBoard, AccessedUserID from ComponentHistory where ActionStepHistoryID = {0} and ID > {1}";

            string strSQL = string.Format(strFormat, history.ActionStepHistoryID, history.LastComponentHistoryID);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            string strComponentHistoryIDs = "";
            Dictionary<int, ComponentHistory> dicComponentHistories = new Dictionary<int, ComponentHistory>();
            List<ComponentHistory> componentHistories = new List<ComponentHistory>();

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                VariousData<int> nID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> nActionStepHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> nComponentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> nComponentType = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[i + 4]);
                VariousData<int> nStatus = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strTask = WebDBManager.GetStringField(arrResult[i + 6].ToString());
                VariousData<int> nCompleteCount = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                VariousData<int> nShowBoard = WebDBManager.GetIntField(arrResult[i + 8].ToString());
                VariousData<int> nSOPGenUserID = WebDBManager.GetIntField(arrResult[i + 9].ToString());

                if (nID == null || nActionStepHistoryID == null || nComponentID == null || nComponentType == null ||
                    time == null || nStatus == null || nSOPGenUserID == null)
                    continue;

                if (strTask == null)
                    strTask = "";

                if (nCompleteCount == null)
                    nCompleteCount = new VariousData<int>(0);

                // ComponentID는 Type별로 중복될수 있으므로 ComponentType을 Int의 제일 첫번째 Byte를 ComponentType에 할당한다.
                int nSectionKey = (nComponentType.Data << 24) | nComponentID.Data;

                SectionState sectionState = history.GetSectionState(nComponentID.Data, nComponentType.Data);

                if (sectionState == null)
                    continue;

                int nDirection = nStatus.Data >> 16;
                int nStatus2 = nStatus.Data & 0x0000ffff;

                sectionState.SetState(nStatus2);

                string strStatus = "";
                if (!GetStatusString(sectionState.GetState(), ref strStatus))
                    continue;

                ComponentHistory.HistoryType historyType = ToHistoryType(nStatus2);

                if (historyType == ComponentHistory.HistoryType.NONE)
                    continue;

                ComponentHistory componentHistory = new ComponentHistory();

                componentHistory.ID = nID.Data;
                componentHistory.TimeStamp = time.Data;
                componentHistory.SectionState = sectionState;
                componentHistory.Task = GetDetailTask(sectionState.Section.Data);
                componentHistory.Type = historyType;
                componentHistory.Commander = GetUserCommanderName(nSOPGenUserID.Data);
                //componentHistory.Commander = GetCommanderName(sectionState.Section);
                componentHistory.Receiver = GetReceiverName(sectionState.Section);

                if (sectionState.Section is Sections.SectionDecision && strTask.Length > 0)
                    componentHistory.Task += " => " + strTask;

                dicComponentHistories[nID.Data] = componentHistory;
                componentHistories.Add(componentHistory);

                //realTime.AddComponentHistory(actionStepHistory, componentHistory);
                m_nLastReadComponentHistoryID = nID.Data;

                if (strComponentHistoryIDs.Length == 0)
                    strComponentHistoryIDs = nID.Data.ToString();
                else
                    strComponentHistoryIDs += ", " + nID.Data.ToString();
            }
        }

        private static ComponentHistory.HistoryType ToHistoryType(int nComponentState)
        {
            if (nComponentState == (int)ComponentState.WAIT || nComponentState == (int)ComponentState.STANDBY)
                return ComponentHistory.HistoryType.WAIT;
            else if (nComponentState == (int)ComponentState.RUN)
                return ComponentHistory.HistoryType.CONFIRM_MISSION;
            else if (nComponentState == (int)ComponentState.DONE)
                return ComponentHistory.HistoryType.COMPLETE_MISSION;

            return ComponentHistory.HistoryType.NONE;
        }

        private static bool GetStatusString(SectionState.State state, ref string strStatus)
        {
            if (state == SectionState.State.NORMAL)        // 대기
                strStatus = "대기";
            else if (state == SectionState.State.INPUT)    // 입력 대기
            {
                strStatus = "대기";
                //strStatus = "입력대기";
                // 입력 대기는 로그를 기록하지 않는다.
                //return false;
            }
            else if (state == SectionState.State.RUN)      // 실행중
                strStatus = "실행중";
            else if (state == SectionState.State.SKIP)     // 건너뛰기
            {
                strStatus = "건너뛰기";
                // 건너뛰기는 로그를 기록하지 않는다.
                return false;
            }
            else if (state == SectionState.State.DONE)     // 실행 완료
                strStatus = "완료";
            else
                return false;

            return true;
        }

        private void ReadActionStepHistoryInfo(int nActionStepHistoryID, ref ActionStepHistory history)
        {
            string strSQL = "Select ash.ActionStepID, ash.RealMode, ash.BeginTime, ash.EndTime, ash.CancelTime, ash.Position, dc.CategoryName, sdc.SubCategoryName, d.DisasterName, step.StepName ";
            strSQL += "from ActionStepHistory as ash, ActionStep as step, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strSQL += "where ash.ActionStepID = step.ID and step.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and ash.ID = " + nActionStepHistoryID.ToString();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 10)
                return;

            VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> realMode = WebDBManager.GetIntField(arrResult[1].ToString());
            VariousData<DateTime> beginTime = WebDBManager.GetDateTimeField(arrResult[2].ToString());
            VariousData<DateTime> endTime = WebDBManager.GetDateTimeField(arrResult[3].ToString());
            VariousData<DateTime> cancelTime = WebDBManager.GetDateTimeField(arrResult[4].ToString());
            string strPosition = WebDBManager.GetStringField(arrResult[5]);
            string strCategoryName = WebDBManager.GetStringField(arrResult[6]);
            string strSubCategoryName = WebDBManager.GetStringField(arrResult[7]);
            string strDisasterName = WebDBManager.GetStringField(arrResult[8]);
            string strActionStepName = WebDBManager.GetStringField(arrResult[9]);

            if (actionStepID == null || realMode == null || beginTime == null ||
                strCategoryName == null || strSubCategoryName == null ||
                strDisasterName == null || strActionStepName == null)
                return;

            ActionStepHistory _history = new ActionStepHistory();

            _history.ActionStepHistoryID = nActionStepHistoryID;
            _history.BeginTime = beginTime.Data;
            _history.DisasterInfo = strPosition == null ? "" : strPosition;
            _history.SOPName = strCategoryName + "/" + strSubCategoryName + "/" + strDisasterName + "/" + strActionStepName;

            if (endTime != null)
                _history.ElapsedSeconds = (int)(endTime.Data - _history.BeginTime).TotalSeconds;
            else if (cancelTime != null)
                _history.ElapsedSeconds = (int)(cancelTime.Data - _history.BeginTime).TotalSeconds;
            else
                _history.ElapsedSeconds = (int)(DateTime.Now - _history.BeginTime).TotalSeconds;

            if (history == null)
                history = _history;
            else
            {
                history.ActionStepHistoryID = _history.ActionStepHistoryID;
                history.BeginTime = _history.BeginTime;
                history.DisasterInfo = _history.DisasterInfo;
                history.ElapsedSeconds = _history.ElapsedSeconds;
                history.SOPName = _history.SOPName;
            }
        }

        private int ReadCurrentActionStepHistoryID(out bool isClosed)
        {
            isClosed = true;

            DateTime dtWeekAgo = DateTime.Now.AddDays(-7);
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtWeekAgo.Year, dtWeekAgo.Month, dtWeekAgo.Day, dtWeekAgo.Hour, dtWeekAgo.Minute, dtWeekAgo.Second);

            // 최소한 일주일 이내에 시작된 SOP들만 검색한다.
            string strSQL = "Select ID from ActionStepHistory where EndTime is NULL and CancelTime is NULL and BeginTime > '" + strTime + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;
            string strIDs = "";

            for (int i=0;i<nResultCount;i++)
            {
                VariousData<int> actionStepHistoryID = WebDBManager.GetIntField(arrResult[i].ToString());

                if (actionStepHistoryID == null)
                    continue;

                if (strIDs.Length == 0)
                    strIDs = actionStepHistoryID.Data.ToString();
                else
                    strIDs += "," + actionStepHistoryID.Data.ToString();
            }

            if (strIDs.Length == 0)
            {
                // 실행중인 SOP가 없다면...
                int nID = GetMaxID("ActionStepHistory");

                if (nID >= 0)
                    return nID;
            }
            else
            {
                // 실행중인 SOP가 있다면...
                isClosed = false;

                int nID = ReadCurrentActionStepHistoryIDFromComponentHistory(strIDs);

                if (nID < 0)
                    isClosed = true;
                else
                    return nID;
            }

            return -1;
        }

        private int GetMaxID(string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            return id == null ? 0 : id.Data;
        }

        private int ReadCurrentActionStepHistoryIDFromComponentHistory(string strActionStepHistoryIDs)
        {
            string strSQL = "Select ActionStepHistoryID from ComponentHistory where ID = (Select max(ID) from ComponentHistory where ActionStepHistoryID in (" + strActionStepHistoryIDs + "))";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id != null)
                return id.Data;

            return -1;
        }*/

        public void Close()
        {
            m_shutdownThread = true;
        }

        public void SetSectionNumber(int nActionStepID, int nSectionType, int nSectionID, int nSectionNumber)
        {
            Dictionary<long, int> dicSectionOrders;

            if (m_dicActionStepSectionsOrder.TryGetValue(nActionStepID, out dicSectionOrders) == false)
            {
                dicSectionOrders = new Dictionary<long, int>();
                m_dicActionStepSectionsOrder[nActionStepID] = dicSectionOrders;
            }

            long key = (((long)nSectionType) << 32) | ((long)nSectionID);
            dicSectionOrders[key] = nSectionNumber;
        }

        public int GetSectionNumber(int nActionStepID, int nSectionType, int nSectionID)
        {
            Dictionary<long, int> dicSectionOrders;

            if (m_dicActionStepSectionsOrder.TryGetValue(nActionStepID, out dicSectionOrders) == false)
                return -1;

            int nSectionNumber;
            long key = (((long)nSectionType) << 32) | ((long)nSectionID);

            if (dicSectionOrders.TryGetValue(key, out nSectionNumber))
                return nSectionNumber;

            return -1;
        }
    }
}