using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using Sections;
using UnE.SOP.Workstate;
using UnE.SOP.Tree;
using DBUtility2;
using System.Windows.Forms;
using System.Collections.Concurrent;

namespace UnE.SOP.History
{
    public class HistoryManager2 : ISOPHistoryContainer, IHistoryManager
    {
        private static HistoryManager2 m_instance = null;

        private Thread m_thread = null;
        private ConcurrentQueue<HistoryActionStepData> m_actionStepHistories = new ConcurrentQueue<HistoryActionStepData>();
        private ConcurrentQueue<HistorySectionData> m_sectionHistories = new ConcurrentQueue<HistorySectionData>();
        //private ArrayList m_arrSectionHistory = new ArrayList();
        //private ArrayList m_arrActionStepHistory = new ArrayList();

        // Exit Thread
        private bool bExit = false;
        private bool closedThread = false;

        // 실행중인 SOP에 대한 재난 위치 정보
        // Key : 상위 4바이트(ActionStepID)
        //       하위 4바이트(isRealMode, 1이면 실제 모드, 0이면 가상모드)
        private Dictionary<long, HistoryDisasterPosition> m_dicHistoryDisasterPosition = new Dictionary<long, HistoryDisasterPosition>();
        // 실행중인 SOP에 대한 재난위치 이외의 정보
        private Dictionary<long, HistoryDisasterNoPosition> m_dicHistoryDisasterNoPosition = new Dictionary<long, HistoryDisasterNoPosition>();

        public static HistoryManager2 Instance
        {
            get { return m_instance; }
        }

        public bool Exit
        {
            get { return bExit; }
        }

        // 실행중인 SOP에 대한 재난 위치 정보
        // Key : 상위 4바이트(ActionStepID)
        //       하위 4바이트(isRealMode, 1이면 실제 모드, 0이면 가상모드)
        public Dictionary<long, HistoryDisasterPosition> HistoryDisasterPosition
        {
            get { return m_dicHistoryDisasterPosition; }
        }

        // 실행중인 SOP에 대한 재난위치 이외의 정보
        // Key : 상위 4바이트(ActionStepID)
        //       하위 4바이트(isRealMode, 1이면 실제 모드, 0이면 가상모드)
        public Dictionary<long, HistoryDisasterNoPosition> HistoryDisasterNoPosition
        {
            get { return m_dicHistoryDisasterNoPosition; }
        }

        public void Dispose()
        {
            bExit = true;

            //m_thread.Join();
            int nTimeout = 3000;
            int nSleep = 0;
            int nSleepTime = 100;

            while (!closedThread)
            {
                System.Threading.Thread.Sleep(nSleepTime);

                nSleep += nSleepTime;

                if (nSleep >= nTimeout)
                {
                    break;
                }
            }
        }

        private int m_nSiteID = 1;

        private HistoryManager2()
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            ProxySOP.Instance.HistoryContainer = this;
            WorkFlowManager.HistoryManager = this;
            BeginHistroy();
        }

        public static void MakeInstance()
        {
            if (m_instance != null)
                return;

            m_instance = new HistoryManager2();
        }

        public void BeginHistroy()
        {
            m_thread = new Thread(HistoryThread);
            m_thread.Name = "History";
            m_thread.Start();
        }

        /*public void SetActionStepHistory(int nActionStepID, int nActionStepHistoryID)
        {
            m_dicActionStepHistory[nActionStepID] = nActionStepHistoryID;
        }

        // ActionStep별 마지막으로 저장된 ComponentHistory를 기억시킨다.
        public void SetLastComponentHistory(int nActionStepID, int nComponentHistoryID)
        {
            m_dicLastComponentHistory[nActionStepID] = nComponentHistoryID;
        }*/

        // nextSection : 분기문을 통하여 실행된 Section
        public HistorySectionDecisionData AddDecisionHistory(SectionDecision section, SectionState sectionState, Workstate.State state, int nProcessDirections, Section nextSection, bool noDBWrite, DateTime time, bool showBoard)
        {
            // 입력대기는 기록하지 않는다.
            //if (state == State.INPUT)
            //    return;
            if (sectionState.BeginTime == null)
                return null;

            //time = (DateTime)sectionState.BeginTime;

            HistorySectionDecisionData data = new HistorySectionDecisionData(time, state, nProcessDirections, section, nextSection);
            data.NoDBWrite = noDBWrite;
            data.ShowBoard = showBoard;
            m_sectionHistories.Enqueue(data);
            //m_arrSectionHistory.Add(data);

            return data;
        }

        // nextSection : 분기문을 통하여 실행된 Section
        public HistorySectionDecisionData AddDecisionHistory(SectionDecision section, SectionState sectionState, Workstate.State state, int nProcessDirections, Section nextSection = null, bool showBoard = false)
        {
            return AddDecisionHistory(section, sectionState, state, nProcessDirections, nextSection, false, DateTime.Now, showBoard);
        }

        public HistorySectionInternalData AddInternalHistory(SectionInternal section, SectionState sectionState, Workstate.State state, int nProcessDirections, int nCheckedRun, int nCheckedComplete, bool usePopupMessage, bool useSMS, bool useBroadcast, bool noDBWrite, DateTime time, bool showBoard, int nCheckedNotify1)
        {
            // 입력대기는 기록하지 않는다.
            //if (state == State.INPUT)
            //    return;
            if (sectionState.BeginTime == null)
                return null;

            //time = (DateTime)sectionState.BeginTime;

            HistorySectionInternalData data = new HistorySectionInternalData(time, state, nProcessDirections, section, usePopupMessage, useSMS, useBroadcast);
            data.NoDBWrite = noDBWrite;
            data.ShowBoard = showBoard;
            data.CheckNotify1 = nCheckedNotify1;
            data.CheckedRun = nCheckedRun;
            data.CheckedComplete = nCheckedComplete;
            m_sectionHistories.Enqueue(data);
            //m_arrSectionHistory.Add(data);

            return data;
        }

        public HistorySectionInternalData AddInternalHistory(SectionInternal section, SectionState sectionState, Workstate.State state, int nProcessDirections, int nCheckedNotify1, int nCheckedRun, int nCheckedComplete, bool usePopupMessage = false, bool useSMS = false, bool useBroadcast = false, bool showBoard = false)
        {
            return AddInternalHistory(section, sectionState, state, nProcessDirections, nCheckedRun, nCheckedComplete, usePopupMessage, useSMS, useBroadcast, false, DateTime.Now, showBoard, nCheckedNotify1);
        }

        public HistorySectionExternalData AddExternalHistory(SectionExternal section, Workstate.State state, int nProcessDirections, bool useSMS, bool useFax, bool noDBWrite, DateTime time, bool showBoard, int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete)
        {
            // 입력대기는 기록하지 않는다.
            //if (state == State.INPUT)
            //    return;

            HistorySectionExternalData data = new HistorySectionExternalData(time, state, nProcessDirections, section, useSMS, useFax);
            data.NoDBWrite = noDBWrite;
            data.ShowBoard = showBoard;
            data.CheckNotify1 = nCheckedNotify1;
            data.CheckNotify2 = nCheckedNotify2;
            data.CheckedRun = nCheckedRun;
            data.CheckedComplete = nCheckedComplete;
            m_sectionHistories.Enqueue(data);
            //m_arrSectionHistory.Add(data);

            return data;
        }

        public HistorySectionExternalData AddExternalHistory(SectionExternal section, Workstate.State state, int nProcessDirections, int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete, bool useSMS = false, bool useFax = false, bool showBoard = false)
        {
            return AddExternalHistory(section, state, nProcessDirections, useSMS, useFax, false, DateTime.Now, showBoard, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete);
        }

        public HistorySectionTransmissionData AddTransmissionHistory(SectionTransmission section, Workstate.State state, int nProcessDirections, bool usePopupMessage, bool useSMS, bool useBroadcast, bool useExSMS, bool useExFax, bool noDBWrite, DateTime time, bool showBoard, int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete)
        {
            HistorySectionTransmissionData data = new HistorySectionTransmissionData(time, state, nProcessDirections, section, usePopupMessage, useSMS, useBroadcast, useExSMS, useExFax);
            data.NoDBWrite = noDBWrite;
            data.ShowBoard = showBoard;
            data.CheckNotify1 = nCheckedNotify1;
            data.CheckNotify2 = nCheckedNotify2;
            data.CheckedRun = nCheckedRun;
            data.CheckedComplete = nCheckedComplete;
            m_sectionHistories.Enqueue(data);
            //m_arrSectionHistory.Add(data);

            return data;
        }

        public HistorySectionTransmissionData AddTransmissionHistory(SectionTransmission section, Workstate.State state, int nProcessDirections, int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete, bool usePopupMessage = false, bool useSMS = false, bool useBroadcast = false, bool useExSMS = false, bool useExFax = false, bool showBoard = false)
        {
            return AddTransmissionHistory(section, state, nProcessDirections, usePopupMessage, useSMS, useBroadcast, useExSMS, useExFax, false, DateTime.Now, showBoard, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete);
        }

        public HistorySectionData AddSectionHistory(Section section, Workstate.SectionState sectionState, int nComponentHistoryID, Workstate.State state, int nProcessDirections, bool noDBWrite, DateTime time, bool showBoard, int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete, Dictionary<int, List<HistorySectionData.DetailData>> detailDatas)
        {
            // 입력대기는 기록하지 않는다.
            //if (state == State.INPUT)
            //    return;
            if (sectionState.BeginTime == null)
                return null;

            //time = (DateTime)sectionState.BeginTime;

            Section.ComponentType type = section.GetComponentType();
            HistorySectionData data = null;

            if (type == Section.ComponentType.DECISION)
            {
                data = AddDecisionHistory((SectionDecision)section, sectionState, state, nProcessDirections, null, noDBWrite, time, showBoard);
                if (data != null)
                    data.ComponentHistoryID = nComponentHistoryID;
            }
            else if (type == Section.ComponentType.INTERNAL)
            {
                data = AddInternalHistory((SectionInternal)section, sectionState, state, nProcessDirections, nCheckedRun, nCheckedComplete, false, false, false, noDBWrite, time, showBoard, nCheckedNotify1);
                if (data != null)
                    data.ComponentHistoryID = nComponentHistoryID;
            }
            else if (type == Section.ComponentType.EXTERNAL)
            {
                data = AddExternalHistory((SectionExternal)section, state, nProcessDirections, false, false, noDBWrite, time, showBoard, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete);
                if (data != null)
                    data.ComponentHistoryID = nComponentHistoryID;
            }
            else if (type == Section.ComponentType.TRANSMISSION)
            {
                data = AddTransmissionHistory((SectionTransmission)section, state, nProcessDirections, false, false, false, false, false, noDBWrite, time, showBoard, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete);
                if (data != null)
                    data.ComponentHistoryID = nComponentHistoryID;
            }
            else
            {
                data = new HistorySectionData(time, state, nProcessDirections, section);
                data.NoDBWrite = noDBWrite;
                data.ShowBoard = showBoard;
                data.CheckNotify1 = nCheckedNotify1;
                data.CheckNotify2 = nCheckedNotify2;
                data.CheckedRun = nCheckedRun;
                data.CheckedComplete = nCheckedComplete;
                data.ComponentHistoryID = nComponentHistoryID;

                m_sectionHistories.Enqueue(data);
                //m_arrSectionHistory.Add(data);
            }

            if (data != null && detailDatas != null)
            {
                foreach (KeyValuePair<int, List<HistorySectionData.DetailData>> pair in detailDatas)
                {
                    // 이미 DB에 기록된 데이터는 다시 기록하지 않도록 한다.
                    if (pair.Key > 0)
                        continue;

                    data.HistoryDetailDatas[pair.Key] = pair.Value;

                    //System.Diagnostics.Trace.WriteLine("pair key " + pair.Key);

                    //foreach(HistorySectionData.DetailData datas in pair.Value)
                    // {
                    //    System.Diagnostics.Trace.WriteLine("List Idx" + datas.DataIndex.Data + "  data " + datas.Datai.Data);
                    //}

                }
            }

            return data;
        }

        public HistorySectionData AddSectionHistory(Section section, Workstate.SectionState sectionState, Workstate.State state, int nProcessDirections, bool showBoard, int nCheckedNotify1, int nCheckedNotify2, int nCheckedRun, int nCheckedComplete, Dictionary<int, List<HistorySectionData.DetailData>> detailDatas)
        {
            return AddSectionHistory(section, sectionState, -1, state, nProcessDirections, false, DateTime.Now, showBoard, nCheckedNotify1, nCheckedNotify2, nCheckedRun, nCheckedComplete, detailDatas);
        }

        public void AddActionStepHistory(int nActionStepHistoryID, int nActionStepID, bool isRealMode, WorkFlowState state, Section sectionSelected, bool bSendSMS)
        {
            ActionStepInfo actionStep = GetActionStep(nActionStepID);
            if (actionStep == null)
                return;

            HistoryActionStepData data = new HistoryActionStepData(DateTime.Now, state, actionStep, isRealMode, bSendSMS);
            data.ActionStepHistoryID = nActionStepHistoryID;
            data.SectionSelectedData = true;
            data.SelectedSection = sectionSelected;
            m_actionStepHistories.Enqueue(data);
            //m_arrActionStepHistory.Add(data);
        }

        public void AddActionStepHistory(int nActionStepHistoryID, int nActionStepID, bool isRealMode, Workstate.WorkFlowState state, DateTime time, bool noDBWrite, Section sectionSelected, bool bSendSMS)
        {
            HistoryActionStepData data = AddActionStepHistory(nActionStepHistoryID, nActionStepID, isRealMode, state, time, noDBWrite, bSendSMS);

            if (data != null)
            {
                if (sectionSelected != null)
                {
                    data.SectionSelectedData = true;
                    data.SelectedSection = sectionSelected;
                }
            }
        }

        // SOP 실행중 SOP 버전이 바뀌어 이전 버전의 SOP에 접근해야 할 경우
        // DB를 읽어 이전 ActionStep 정보를 얻어온다.
        private ActionStepInfo ReadOldActionStep(int nActionStepID)
        {
            string strFormat = "select step.ID, StepName, PeriodType, BeginTime, EndTIme, WeekDayOption, Iteration, IterationType, ProcessTime, ProcessTimeType, DisasterID, ParentStepID, v.isNormal ";
            strFormat += "from ActionStep as step, Disaster as d, Version as v ";
            strFormat += "where step.DisasterID = d.ID and d.VersionID = v.ID and step.ID = {0}";
            string strSQL = string.Format(strFormat, nActionStepID);

            ArrayList arrResult = ProxySOP.Instance.DBManager.GetResultData(strSQL);
            if (arrResult == null)
                return null;

            DateTime dtDefault = new DateTime();

            int nCount = arrResult.Count;

            if (nCount >= 13)
            {
                int nID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
                string strStepName = WebDBManager.GetStringField(arrResult[1], "");
                int nPeriodType = WebDBManager.GetIntField(arrResult[2].ToString(), 0);
                DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[3], dtDefault);
                DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[4], dtDefault);
                int nWeekdayOpt = WebDBManager.GetIntField(arrResult[5].ToString(), 127);
                int nIteration = WebDBManager.GetIntField(arrResult[6].ToString(), 1);
                int nIterationType = WebDBManager.GetIntField(arrResult[7].ToString(), 0);
                int nProcessTime = WebDBManager.GetIntField(arrResult[8].ToString(), 0);
                int nProcessTimeType = WebDBManager.GetIntField(arrResult[9].ToString(), 5);
                int nDisasterID = WebDBManager.GetIntField(arrResult[10].ToString(), -1);
                int nParentStepID = WebDBManager.GetIntField(arrResult[11].ToString(), -1);
                VariousData<int> isNormal = WebDBManager.GetIntField(arrResult[12].ToString());

                if (isNormal == null)
                    return null;

                ActionStepInfo actionStep = new ActionStepInfo();

                actionStep.ActionStepID = nID;
                actionStep.ActionStepName = strStepName;
                actionStep.ParentStepID = nParentStepID;
                actionStep.PeriodType = nPeriodType;
                actionStep.BeginTime = dtBegin;
                actionStep.EndTime = dtEnd;
                actionStep.WeekDayOption = nWeekdayOpt;
                actionStep.Iteration = nIteration;
                actionStep.IterationType = nIterationType;
                actionStep.ProcessTime = nProcessTime;
                actionStep.ProcessTimeType = nProcessTimeType;
                actionStep.DisasterID = nDisasterID;
                actionStep.IsNormal = isNormal.Data == 1;

                return actionStep;
            }

            return null;
        }

        public HistoryActionStepData AddActionStepHistory(int nActionStepHistoryID, int nActionStepID, bool isRealMode, Workstate.WorkFlowState state, DateTime time, bool noDBWrite, bool bSendSMS)
        {
            ActionStepInfo actionStep = GetActionStep(nActionStepID);

            if (actionStep == null)
            {
                // actionStep이 null인 것은 SOP 실행중 SOP 버전이 바뀌어 nActionStepID는 이전 버전을 가리키고 있는 경우가 된다.
                // DB를 읽어 이전 ActionStep 정보를 얻어온다.
                actionStep = ReadOldActionStep(nActionStepID);

                if (actionStep == null)
                    return null;
            }


            HistoryActionStepData data = new HistoryActionStepData(time, state, actionStep, isRealMode, bSendSMS);
            data.ActionStepHistoryID = nActionStepHistoryID;
            data.NoDBWrite = noDBWrite;
            if (state != Workstate.WorkFlowState.DONE)
            {
                IDisasterContainer disForm = ProxySOP.Instance.SOPDisasterContainer;
                if (disForm != null)
                {
                    data.Position = disForm.GetLastDisasterPosition();
                }
            }

            m_actionStepHistories.Enqueue(data);
            //m_arrActionStepHistory.Add(data);

            return data;
        }

        // History를 DB에 기록하는 부분은 Network을 통하여 이루어지므로 병목이 발생할 여지가 있다.
        // History 기록으로 인하여 Work Flow의 처리 속도가 영향을 받아선 안되므로, 
        // History 기록은 Thread를 사용하여 비동기로 진행한다.
        private void HistoryThread()
        {
            List<HistoryActionStepData> actionStepHistories = new List<HistoryActionStepData>();

            while (!bExit)
            {
                try
                {
                    // 방금 종료된 ActionStep 데이터
                    int nEndActionStepID = -1, nEndActionStepHistoryID = -1;
                    bool needUpdate = m_actionStepHistories.IsEmpty == false || m_sectionHistories.IsEmpty == false;
                    //bool needUpdate = m_arrActionStepHistory.Count > 0 || m_arrSectionHistory.Count > 0;

                    // changed by mwkim 2015-10-29 근무조 데이터는 최초로드시, 데이터 변경시 에만 로딩되도록 변경함.
                    //if (needUpdate)
                    //    ControlTeamEditor.VaildMemberPhoneNumber.LoadDB();

                    ProxySOP.Instance.SOPContainer.BeginHistory();


                    WriteActionStepHistory(ref nEndActionStepID, ref nEndActionStepHistoryID);


                    WriteSectionHistory(nEndActionStepID, nEndActionStepHistoryID);


                    ProxySOP.Instance.SOPContainer.EndHistory();

                    //if (needUpdate)
                    //    ControlTeamEditor.VaildMemberPhoneNumber.ReleaseDB();
                }
                catch (System.NullReferenceException e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                Thread.Sleep(400);
            }

            closedThread = true;
        }

        protected bool WriteActionStepHistory(ref int nEndActionStepID, ref int nEndActionStepHistoryID)
        {
            WebDBManager dbMgr = ProxySOP.Instance.DBManager;
            if (dbMgr == null)
                return false;

            HistoryActionStepData data;
            List<HistoryActionStepData> notRemovedHistories = new List<HistoryActionStepData>();

            while (m_actionStepHistories.IsEmpty == false)
            {
                if (m_actionStepHistories.TryDequeue(out data) == false)
                    break;
                
                int nHistoryID = data.ActionStepHistoryID;
                int nActionStepID = data.IsRealMode ? data.ActionStep.ActionStepID : -data.ActionStep.ActionStepID;

                if (nHistoryID > 0)
                {
                    UpdateActionStepHistory(dbMgr, nHistoryID, data);
                }
                else
                {

                    int nRealActionStepID = data.ActionStep.ActionStepID;
                    bool bReal = data.IsRealMode;

                    Sections.ISOPPageContainer container = ProxySOP.Instance.PageContainer;
                    if (container != null)
                    {
                        // SOP설정이 완료되었는지 검사 추가                                
                        if (container.ExistSOPScenario(nRealActionStepID, bReal))
                        {
                            nHistoryID = WriteActionStepHistory(dbMgr, data, ref nEndActionStepID, ref nEndActionStepHistoryID);
                            if (nHistoryID > 0)
                            {
                                data.ActionStepHistoryID = nHistoryID;
                            }
                        }
                        else
                        {
                            // 처리되지 않은 로그는 다음 루프에서 재사용 하도록 한다.
                            notRemovedHistories.Add(data);
                        }
                    }
                    else
                    {
                        // 처리되지 않은 로그는 다음 루프에서 재사용 하도록 한다.
                        notRemovedHistories.Add(data);
                    }
                }
            }

            foreach (HistoryActionStepData _data in notRemovedHistories)
            {
                m_actionStepHistories.Enqueue(_data);
            }

            return true;
        }

        private void UpdateActionStepSelectedSection(WebDBManager dbMgr, int nHistoryID, Section section)
        {
            string strSQL = "";

            if (section == null)
            {
                strSQL = string.Format("Update ActionStepHistory set SelectedComponentID = NULL, SelectedComponentType = NULL where ID = {0}",
                    nHistoryID);
            }
            else
            {
                strSQL = string.Format("Update ActionStepHistory set SelectedComponentID = {0}, SelectedComponentType = {1} where ID = {2}",
                    section.Data.ID, (int)section.GetComponentType(), nHistoryID);
            }

            dbMgr.GetResultData(strSQL);
        }

        private string ToShortDateString(DateTime time)
        {
            return string.Format("{0}-{1:00}-{2:00}", time.Year, time.Month, time.Day);
        }

        private bool UpdateActionStepHistory(WebDBManager dbMgr, int nHistoryID, HistoryActionStepData data)
        {
            if (data.SectionSelectedData && !data.NoDBWrite)
            {
                // 현재 선택된 Section을 DB에 기록한다.
                UpdateActionStepSelectedSection(dbMgr, nHistoryID, data.SelectedSection);
                return true;
            }

            string strSQL = "";
            string strType = "-", strTask = "-";

            GetActionStepHistoryData(data, ref strType, ref strTask);

            Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;

            bool result = false;

            if (data.NoDBWrite)
                result = true;

            int nSOPGenUser = ProxySOP.Instance.SOPGenUserID;

            if (data.State == Workstate.WorkFlowState.STANDBY)       // 대기
            {
                return true;
            }
            else if (data.State == Workstate.WorkFlowState.RUN)      // 실행중
            {
                ArrayList arrProcessIDList = GetProcessIDList(data.ActionStep.ActionStepID);
                if (arrProcessIDList == null)
                    return true;

                sopLog.MakeActionStepLog(data.ActionStep.ActionStepID, data.IsRealMode, nHistoryID, data.SensorZoneHistoryID, data.Time, arrProcessIDList);
                return true;
            }
            else if (data.State == Workstate.WorkFlowState.PAUSE)    // 일시정지
            {
                if (!data.NoDBWrite)
                {
                    strSQL = string.Format("update ActionStepHistory set EndTime = NULL, CancelTime = NULL, PausedTime = '{0} {1:00}:{2:00}:{3:00}', LastAccessedUserID = {4} where id = {5}",
                        ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second, nSOPGenUser, nHistoryID);

                    result = dbMgr.GetResultData(strSQL) != null;
                }
            }
            else if (data.State == Workstate.WorkFlowState.STOP)     // 실행취소
            {
                strSQL = string.Format("update ActionStepHistory set EndTime = NULL, CancelTime = '{0} {1:00}:{2:00}:{3:00}', PausedTime = NULL, LastAccessedUserID = {4} where id = {5}",
                    ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second, nSOPGenUser, nHistoryID);

                // WriteActionStepHistory 중복으로 comment 2014-03-18 skkim
                //sopLog.CancelActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode, data.Time);
                //sopLog.AddLogData(null, data.NoDBWrite, nHistoryID, -1, data.ActionStep.ActionStepID, data.IsRealMode, -1, Section.ComponentType.NONE, data.Time, "-", "-",  strType, strTask, "실행취소", -1, true, true);

                if (!data.NoDBWrite)
                {
                    result = dbMgr.GetResultData(strSQL) != null;
                }

                /*if (result)
                {
                    int nActionStepID = data.IsRealMode ? data.ActionStep.ActionStepID : -data.ActionStep.ActionStepID;
                    m_dicActionStepHistory.Remove(nActionStepID);
                }*/
            }
            else if (data.State == Workstate.WorkFlowState.DONE)     // 완료
            {
                strSQL = string.Format("update ActionStepHistory set EndTime = '{0} {1:00}:{2:00}:{3:00}', CancelTime = NULL, PausedTime = NULL, LastAccessedUserID = {4} where id = {5}",
                    ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second, nSOPGenUser, nHistoryID);

                // WriteActionStepHistory 중복으로 comment 2014-03-18 skkim
                //sopLog.CompleteActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode, data.Time);
                //sopLog.AddLogData(null, data.NoDBWrite, nHistoryID, -1, data.ActionStep.ActionStepID, data.IsRealMode, -1, Section.ComponentType.NONE, data.Time, "-", "-",  strType, strTask, "완료", -1, true, true);

                if (!data.NoDBWrite)
                {
                    result = dbMgr.GetResultData(strSQL) != null;
                }

                /*if (result)
                {
                    int nActionStepID = data.IsRealMode ? data.ActionStep.ActionStepID : -data.ActionStep.ActionStepID;
                    m_dicActionStepHistory.Remove(nActionStepID);
                }*/
            }
            else
                return false;

            return result;
        }

        private void GetProcessIDList(PanelSection panel, ArrayList arrProcessIDList)
        {
            long nHighWord = (long)Section.ComponentType.PROCESS << 32;

            ArrayList sections = (ArrayList)panel.Sections.Clone();

            foreach (Section section in sections)//panel.Sections)
            {
                if (section.GetComponentType() != Section.ComponentType.PROCESS)
                    continue;

                int nComponentID = panel.GetComponentID(section);
                if (nComponentID < 0)
                    continue;

                long nID = nHighWord | (long)nComponentID;
                arrProcessIDList.Add(nID);
            }

            sections.Clear();
        }

        private void GetProcessIDList(TabPage tabPage, ArrayList arrProcessIDList)
        {
            Type type = typeof(PanelSection);

            foreach (Control ctrl in tabPage.Controls)
            {
                if (typeof(PanelSection).IsAssignableFrom(ctrl.GetType()))
                {
                    PanelSection panel = (PanelSection)ctrl;
                    GetProcessIDList(panel, arrProcessIDList);
                }
            }
        }

        private ArrayList GetProcessIDList(int nActionStepID)
        {
            Sections.ISOPPageContainer sopMain = ProxySOP.Instance.PageContainer;
            if (sopMain == null)
                return null;

            ArrayList arrTabPages = sopMain.GetTabPage();

            foreach (UnE.SOP.Sections.SectionTabPage tabPage in arrTabPages)
            {
                if (tabPage.ActionStepID == nActionStepID)
                {
                    ArrayList arrProcessIDList = new ArrayList();
                    GetProcessIDList(tabPage, arrProcessIDList);
                    return arrProcessIDList;
                }
            }
            return null;
        }

        private bool GetActionStepHistoryData(HistoryActionStepData data, ref string strType, ref string strTask)
        {
            ISOPTreeContainer tree = ProxySOP.Instance.SOPTreeContainer;
            if (tree == null)
                return false;

            TreeNode actionStepNode = tree.FindActionStepNode(data.ActionStep.ActionStepID);
            if (actionStepNode != null)
            {
                string strFullPath = GetActionStepPath(actionStepNode);


                Data.ISOPDataContainer sopManager = ProxySOP.Instance.SOPDataContainer;
                if (sopManager == null)
                    return false;

                VersionInfo version = sopManager.GetActionStepVersionInfo(data.ActionStep.ActionStepID);
                string strAdd;

                if (version.IsRegular)
                {
                    if (version.IsNormal)
                        strAdd = "[평일 주간모드] ";
                    else
                        strAdd = "[휴일 및 야간모드] ";
                }
                else
                {
                    if (version.IsNormal)
                        strAdd = "[비등록모드/평일 주간모드] ";
                    else
                        strAdd = "[비등록모드/휴일 및 야간모드] ";
                }

                strType = data.IsRealMode ? "실제모드" : "모의훈련모드";
                strTask = strAdd + strFullPath;

                return true;
            }
            return false;
        }

        private bool CheckSameAliveActionStep(WebDBManager dbMgr, int actionStepID)
        {
            string strSQL = $"Select ID from ActionStepHistory where ActionStepID = {actionStepID} and EndTime is NULL and CancelTime is NULL";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                return false;

            return true;
        }

        private int WriteActionStepHistory(WebDBManager dbMgr, HistoryActionStepData data, ref int nEndActionStepID, ref int nEndActionStepHistoryID)
        {
            Data.ISOPDataContainer sopManager = ProxySOP.Instance.SOPDataContainer;
            if (sopManager == null)
                return -1;

            int nSOPGenUserID = ProxySOP.Instance.SOPGenUserID;

            if (data.State == Workstate.WorkFlowState.RUN)
            {
                if (CheckSameAliveActionStep(dbMgr, data.ActionStep.ActionStepID))
                    return -1;

                string strSQL = "select Max(id) from ActionStepHistory";

                ArrayList arrResult = dbMgr.GetResultData(strSQL);
                if (arrResult == null)
                    return -1;

                int nID = arrResult.Count == 0 ? 0 : WebDBManager.GetIntField(arrResult[0].ToString(), 0);
                string strBeginTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", data.Time.Year, data.Time.Month, data.Time.Day, data.Time.Hour, data.Time.Minute, data.Time.Second);
                //string strBeginTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second);

                Workstate.WorkFlow workFlow = Workstate.WorkFlowManager.Instance.Get(data.ActionStep.ActionStepID, data.IsRealMode);
                string strDetectTime = "NULL";

                // 동기화 문제로 인하여 WorkFlow가 생성되기 전이면 1초간 기다린다.
                if (workFlow == null)
                    Thread.Sleep(1000);

                if (workFlow != null)
                {
                    DateTime time = workFlow.Option != null && workFlow.Option.DetectTime != null ? workFlow.Option.DetectTime.Data : DateTime.Now;
                    //strDetectTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", time.ToShortDateString(), time.Hour, time.Minute, time.Second);
                    strDetectTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
                }
                else
                    return -1;

                string strPosition = "NULL";

                if (workFlow != null && workFlow.Option != null && workFlow.Option.HasPosition)
                {
                    // 동기화 문제로 인하여 WorkFlow의 Position이 지정되기 전에 호출될 경우 1초간 기다린다.
                    if (workFlow.Option.PositionName.Length == 0)
                        Thread.Sleep(1000);
                    strPosition = "'" + workFlow.Option.PositionName + "'";
                }


                if (!data.NoDBWrite)
                {
                    string strDescription = "NULL";
                    string strDisasterOption = workFlow.Option.GetDisasterTypeString();
                    int nSensorZoneHistoryID = workFlow.Option.SensorZoneHistoryID;
                    /*if (workFlow.UseAmountSnowfall)
                    {
                        strDescription = "'" + GetSnowfallDisasterTypeString(option.AmountSnowFall.ToString(), true) + "'";
                    }*/

                    if (strDisasterOption.Length > 0)
                        strDisasterOption = "'" + strDisasterOption + "'";
                    else
                        strDisasterOption = "NULL";

                    /* edit by skkim 2017-12-27 Add SensorZoneHistoryID for ActionStepHistory
                    strSQL = string.Format("insert into ActionStepHistory (id, ActionStepID, RealMode, BeginTime, EndTime, CancelTime, PausedTime, DetectTime, Position, LastAccessedUserID, SelectedComponentID, SelectedComponentType, Description, StartOption, DisasterOption) values ({0}, {1}, {2}, {3}, NULL, NULL, NULL, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})",
                        ++nID, data.ActionStep.ActionStepID, data.IsRealMode ? 1 : 0, strBeginTime, strDetectTime, strPosition, nSOPGenUserID,
                        data.SectionSelectedData && data.SelectedSection != null ? data.SelectedSection.Data.ID.ToString() : "NULL",
                        data.SectionSelectedData && data.SelectedSection != null ? ((int)data.SelectedSection.GetComponentType()).ToString() : "NULL",
                        strDescription,
                        data.SendSMS == true ? 1 : 0,
                        strDisasterOption);
                     */

                    if (CheckDuplicateActionStepHistory(dbMgr, data.ActionStep.ActionStepID, nSensorZoneHistoryID))
                    {
                        // 중복된 데이터이므로 삭제한다.
                        //m_arrActionStepHistory.RemoveAt(0);
                        return -1;
                    }

                    strSQL = string.Format("insert into ActionStepHistory ( " +
                         " id, ActionStepID, RealMode, BeginTime, EndTime, CancelTime, PausedTime, DetectTime, Position, " +
                         " LastAccessedUserID, SelectedComponentID, SelectedComponentType, Description, StartOption, DisasterOption, SensorZoneHistoryID) " +
                         " values ({0}, {1}, {2}, {3}, NULL, NULL, NULL, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12})",
                        ++nID, data.ActionStep.ActionStepID, data.IsRealMode ? 1 : 0, strBeginTime, strDetectTime, strPosition, nSOPGenUserID,
                        data.SectionSelectedData && data.SelectedSection != null ? data.SelectedSection.Data.ID.ToString() : "NULL",
                        data.SectionSelectedData && data.SelectedSection != null ? ((int)data.SelectedSection.GetComponentType()).ToString() : "NULL",
                        strDescription,
                        data.SendSMS == true ? 1 : 0,
                        strDisasterOption,
                        (nSensorZoneHistoryID == -1 ? "NULL" : nSensorZoneHistoryID.ToString()));

                    if (dbMgr.GetResultData(strSQL) == null)
                        return -1;

                    sopManager.SetActionStepHistoryID(data.ActionStep.ActionStepID, data.IsRealMode, nID);
                    sopManager.NewActionStepHistory(nID);
                    if (workFlow.Option.LastPosition != null)
                    //if (workFlow.LastPosition != null)
                    {
                        string strDisasterType = workFlow.Option.LastPosition.DisasterName;
                        // DisasterType은 HistoryDisasterPos의 DisasterType에서 ActionStepHistory의 DisasterOption으로 옮긴다.
                        // [2016/11/18] 김지웅
                        //strDisasterType += workFlow.Option.GetDisasterTypeString();

                        /*if (workFlow.LastPosition.UsePSM)
                        {
                            strDisasterType += "/" + workFlow.LastPosition.PSMMaterial + "/" + workFlow.LastPosition.PSMDistance.ToString();
                        }*/

                        /*if (workFlow.LastPosition.UseAmountSnowfall)
                        {
                            strDisasterType += GetSnowfallDisasterTypeString(workFlow.LastPosition);
                        }*/

                        int nHistoryDisasterPosID = GetMaxID("HistoryDisasterPos", dbMgr) + 1;

                        int nActionStepHistoryID = nID;
                        workFlow.Option.LastPosition.HistoryActionStepID = nActionStepHistoryID;
                        strSQL = string.Format("insert into HistoryDisasterPos (ID, PosX, PosY, PosZ, FloorIndex, " +
                                         " HistoryActionSetpID, DisasterType, Description, BuildingID, SiteID, BroadcastName  )  " +
                                         " values ({0}, {1}, {2}, {3}, {4}, {5}, '{6}', '{7}', '{8}', {9}, '{10}')",
                            nHistoryDisasterPosID,
                           workFlow.Option.LastPosition.X, workFlow.Option.LastPosition.Y, workFlow.Option.LastPosition.Z, workFlow.Option.LastPosition.FloorIndex,
                           nActionStepHistoryID, strDisasterType,
                           workFlow.Option.LastPosition.PoistionName, workFlow.Option.LastPosition.BuildingID, m_nSiteID, workFlow.Option.LastPosition.BroadcastName);
                        if (dbMgr.GetResultData(strSQL) == null)
                            return -1;
                    }
                }

                ArrayList arrProcessIDList = GetProcessIDList(data.ActionStep.ActionStepID);
                if (arrProcessIDList == null)
                    return -1;

                string strType = "-", strTask = "-";
                GetActionStepHistoryData(data, ref strType, ref strTask);


                //DockingBottomSOPLog sopLog = FormMain.Instance.GetPageHome().GetDockSOPLog();
                Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;
                if (sopLog != null)
                {
                    sopLog.MakeActionStepLog(data.ActionStep.ActionStepID, data.IsRealMode, nID, data.SensorZoneHistoryID, data.Time, arrProcessIDList);
                    sopLog.AddLogData(null, data.NoDBWrite, nID, -1, data.ActionStep.ActionStepID, data.IsRealMode, -1, Section.ComponentType.NONE, data.Time, "-", "-",
                        strType, strTask, "시작", -1, true, true);
                }

                return nID;
            }
            else if (data.State == Workstate.WorkFlowState.STOP)
            {
                //DockingBottomSOPLog sopLog = FormMain.Instance.GetPageHome().GetDockSOPLog();
                Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;
                if (sopLog == null)
                    return -1;

                ActionStepDetailLog detailLog = sopLog.GetActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode);

                if (detailLog == null)
                    return -1;

                // strSQL = string.Format("update ActionStepHistory set EndTime = NULL, CancelTime = NULL, PausedTime = '{0} {1:00}:{2:00}:{3:00}' where id = {4}",
                //detailLog.HistoryID;

                if (!data.NoDBWrite)
                {
                    string strSQL = string.Format("update ActionStepHistory set EndTime = NULL, CancelTime = '{0} {1:00}:{2:00}:{3:00}', PausedTime = NULL, LastAccessedUserID = {4} where id = {5}",
                        ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second, nSOPGenUserID, detailLog.HistoryID);

                    if (dbMgr.GetResultData(strSQL) == null)
                        return -1;
                }

                sopLog.CancelActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode, data.Time);

                string strType = "-", strTask = "-";
                GetActionStepHistoryData(data, ref strType, ref strTask);

                sopLog.AddLogData(null, data.NoDBWrite, detailLog.HistoryID, -1, data.ActionStep.ActionStepID, data.IsRealMode, -1, Section.ComponentType.NONE, data.Time, "-", "-",
                    strType, strTask, "실행취소", -1, true, true);
            }
            else if (data.State == Workstate.WorkFlowState.DONE)
            {
                //DockingBottomSOPLog sopLog = FormMain.Instance.GetPageHome().GetDockSOPLog();

                Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;
                if (sopLog == null)
                    return -1;

                ActionStepDetailLog detailLog = sopLog.GetActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode);

                if (detailLog == null)
                    return -1;

                if (!data.NoDBWrite)
                {
                    string strSQL = string.Format("update ActionStepHistory set EndTime = '{0} {1:00}:{2:00}:{3:00}', CancelTime = NULL, PausedTime = NULL, LastAccessedUserID = {4} where id = {5}",
                        ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second, nSOPGenUserID, detailLog.HistoryID);

                    if (dbMgr.GetResultData(strSQL) == null)
                    {
                        return -1;
                    }
                }

                sopLog.CompleteActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode, data.Time);

                string strType = "-", strTask = "-";
                GetActionStepHistoryData(data, ref strType, ref strTask);

                sopLog.AddLogData(null, data.NoDBWrite, detailLog.HistoryID, -1, data.ActionStep.ActionStepID, data.IsRealMode, -1, Section.ComponentType.NONE, data.Time, "-", "-",
                    strType, strTask, "완료", -1, true, true);

                nEndActionStepID = data.ActionStep.ActionStepID;
                nEndActionStepHistoryID = detailLog.HistoryID;
            }

            return -1;
        }

        // 같은 SensorZoneHistoryID에 대하여 중복된 ActionStepHistory가 존재하면 안된다.(SensorZoneHistoryID가 NULL일 경우는 예외)
        // DB에 이미 같은 SensorZoneHistoryID에 대한 ActionStepHistory가 존재하는지 여부를 검사한다.
        private bool CheckDuplicateActionStepHistory(WebDBManager dbMgr, int nActionStepID, int nSensorZoneHistoryID)
        {
            if (nSensorZoneHistoryID < 0)
                return false;

            string strSQL = "Select ash.ActionStepID, d.ID from ActionStepHistory as ash, ActionStep as step, Disaster as d ";
            strSQL += "where ash.ActionStepID = step.ID and step.DisasterID = d.ID and ash.ID = " + nSensorZoneHistoryID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            if (nResultCount < 2)
                return false;

            int nDisasterID = -1;
            Dictionary<int, int> dicActionStepDisaster = new Dictionary<int, int>();

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> disasterID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (actionStepID == null || disasterID == null)
                    continue;

                dicActionStepDisaster[actionStepID.Data] = disasterID.Data;
                nDisasterID = disasterID.Data;
            }

            if (dicActionStepDisaster.Count == 0)
                return false;

            if (dicActionStepDisaster.ContainsKey(nActionStepID))
                return true;

            strSQL = "Select DisasterID from ActionStep where ID = " + nActionStepID.ToString();
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return false;

            // 같은 Disaster 내에 속해있는 다른 위험단계의 SOP는 하나의 SensorZoneHistoryID를 사용하여 여럿이 동시에 사용될 수 있다.
            return id.Data != nDisasterID;

            /*string strSQL = "Select ID from ActionStepHistory where SensorZoneHistoryID = " + nSensorZoneHistoryID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            return id != null;*/
        }

        private int GetMaxID(string strTableName, WebDBManager dbMgr)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private int GetMaxIDBatch(string strTableName, WebDBManager dbMgr)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            //ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private string GetSnowfallDisasterTypeString(string strAmountSnowfall, bool isFirst)
        {
            string strTag = "[AmountSnowfall:";

            if (isFirst)
                return strTag + strAmountSnowfall + "]";
            return ";" + strTag + strAmountSnowfall + "]";
        }

        public static string ParseAmountSnowfall(string str)
        {
            return GetDisasterInfoString(str, "AmountSnowfall");
        }

        // nEndActionStepID, nEndActionStepHistoryID : 방금 종료된 ActionStep
        protected bool WriteSectionHistory(int nEndActionStepID, int nEndActionStepHistoryID)
        {
            WebDBManager dbMgr = ProxySOP.Instance.DBManager;
            if (dbMgr == null)
                return false;

            DateTime dtMaxComponentHistoryTime = new DateTime();
            int nMaxComponentHistoryID = m_sectionHistories.IsEmpty == false ? GetMaxSectionHistoryID(dbMgr, ref dtMaxComponentHistoryTime) : 0;

            // 정상적으로 처리되지 않은 데이터는 다음에 처리하도록 한다.
            List<HistorySectionData> failDatas = new List<HistorySectionData>();

            //if (arClonList.Count > 0)
            //{
            //    System.Diagnostics.Trace.WriteLine("arCloneList Count : " + arClonList.Count.ToString());
            //}

            // Thread가 끝난뒤 한꺼번에 Refresh()를 호출하기 위한 변수
            Dictionary<PanelSection, PanelSection> dicPanelSections = new Dictionary<PanelSection, PanelSection>();

            HistorySectionData data;

            while (m_sectionHistories.IsEmpty == false)
            {
                if (m_sectionHistories.TryDequeue(out data) == false)
                    break;

                bool bSuccess = false;

                PanelSection panel = (PanelSection)data.Section.GetParent();
                UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)panel.Parent;

                dicPanelSections[panel] = panel;

                int nActionStepID = tabPage.VirtualMode ? -panel.ActionStepID : panel.ActionStepID;

                IWorkflowContainer workMan = ProxySOP.Instance.WorkflowContainer;
                bool isRealMode = false;

                int nCurrentActionStepID = workMan.ReadCurrentActionStep(ref isRealMode);

                //System.Diagnostics.Trace.WriteLine("1211WriteSectionHistory Current : " + nCurrentActionStepID + " RealMode : " + isRealMode);
                if (isRealMode == false)
                    nCurrentActionStepID *= -1;

                //System.Diagnostics.Trace.WriteLine("1211WriteSectionHistory : " + nActionStepID + " , Current : " + nCurrentActionStepID);
                if (nActionStepID == nCurrentActionStepID)
                {
                    if (tabPage.ActionStepHistoryID > 0)
                    {
                        int nHistoryID = tabPage.ActionStepHistoryID;

                        if (nHistoryID < 0)
                        {
                            // Thread의 처리 순서 때문에 ActionStep보다 먼저 처리하려고 하는 Section
                            // 일단 미뤘다가 다시 실행
                            //System.Diagnostics.Trace.WriteLine("4WriteSectionHistory Continue : " + DateTime.Now);
                            continue;
                        }

                        //System.Diagnostics.Trace.WriteLine("1WriteSectionHistory Begin : " + DateTime.Now);
                        bSuccess = WriteSectionHistory(dbMgr, nHistoryID, data, ref nMaxComponentHistoryID, ref dtMaxComponentHistoryTime);
                        //System.Diagnostics.Trace.WriteLine("1WriteSectionHistory End : " + DateTime.Now + " , Result : " + bSuccess);
                    }
                    else
                    {
                        if (nEndActionStepHistoryID > 0)
                        {
                            //System.Diagnostics.Trace.WriteLine("2WriteSectionHistory Begin : " + DateTime.Now);
                            bSuccess = WriteSectionHistory(dbMgr, nEndActionStepHistoryID, data, ref nMaxComponentHistoryID, ref dtMaxComponentHistoryTime);
                            //System.Diagnostics.Trace.WriteLine("2WriteSectionHistory End : " + DateTime.Now+ " , Result : " + bSuccess);
                        }
                        else
                        {
                            //System.Diagnostics.Trace.WriteLine("3WriteSectionHistory Continue : " + DateTime.Now);
                            continue;
                        }
                    }
                }

                if (bSuccess == false)
                {
                    failDatas.Add(data);
                }
            }

            foreach (KeyValuePair<PanelSection, PanelSection> pair in dicPanelSections)
            {
                pair.Value.Invoke((MethodInvoker)delegate
                {
                    pair.Value.Refresh();
                });
            }

            // if (arClonList.Count > 0)
            // {
            //     System.Diagnostics.Trace.WriteLine("arDelete Count : " + arDelete.Count.ToString());
            // }

            foreach (HistorySectionData _data in failDatas)
            {
                m_sectionHistories.Enqueue(_data);
            }

            return true;
        }

        private int GetMaxSectionHistoryID(WebDBManager dbMgr, ref DateTime dtLatest)
        {
            string strSQL = "Select ID, Time from ComponentHistory where ID = (select max(ID) from ComponentHistory)";

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count < 2)
                return -1;

            int nMaxID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[1]);

            if (time != null)
                dtLatest = time.Data;

            return nMaxID;
        }

        // nHistoryID : ActionStep의 HistoryID
        private bool WriteSectionHistory(WebDBManager dbMgr, int nHistoryID, HistorySectionData data, ref int nMaxComponentHistoryID, ref DateTime dtMaxComponentHistoryTime)
        {
            Section.ComponentType type = data.Section.GetComponentType();

            PanelSection panel = (PanelSection)data.Section.GetParent();
            int nComponentID = panel.GetComponentID(data.Section);

            if (nComponentID < 0)
                return false;

            Workstate.SectionState sectionState = Workstate.WorkFlowManager.Instance.Find(data.Section, !((UnE.SOP.Sections.SectionTabPage)(panel.Parent)).VirtualMode);
            if (sectionState != null)
            {
                sectionState.CheckNotify1 = data.CheckNotify1;
                sectionState.CheckNotify2 = data.CheckNotify2;
            }
            else
                return false;

            if (type == Section.ComponentType.ENDPOINT)
                return WriteSectionEndPointHistory(dbMgr, nHistoryID, nComponentID, data, sectionState, ref nMaxComponentHistoryID, ref dtMaxComponentHistoryTime);
            else if (type == Section.ComponentType.PROCESS)
                return WriteSectionProcessHistory(dbMgr, nHistoryID, nComponentID, data, sectionState, ref nMaxComponentHistoryID, ref dtMaxComponentHistoryTime);
            else if (type == Section.ComponentType.DECISION)
                return WriteSectionDecisionHistory(dbMgr, nHistoryID, nComponentID, (HistorySectionDecisionData)data, sectionState, ref nMaxComponentHistoryID, ref dtMaxComponentHistoryTime);
            else if (type == Section.ComponentType.INTERNAL)
                return WriteSectionInternalHistory(dbMgr, nHistoryID, nComponentID, (HistorySectionInternalData)data, sectionState, ref nMaxComponentHistoryID, ref dtMaxComponentHistoryTime);
            else if (type == Section.ComponentType.EXTERNAL)
                return WriteSectionExternalHistory(dbMgr, nHistoryID, nComponentID, (HistorySectionExternalData)data, sectionState, ref nMaxComponentHistoryID, ref dtMaxComponentHistoryTime);
            else if (type == Section.ComponentType.TRANSMISSION)
                return WriteSectionTransmissionHistory(dbMgr, nHistoryID, nComponentID, (HistorySectionTransmissionData)data, sectionState, ref nMaxComponentHistoryID, ref dtMaxComponentHistoryTime);
            else if (type == Section.ComponentType.TRANSSOP)
                return WriteSectionTransSOPHistory(dbMgr, nHistoryID, nComponentID, data, sectionState, ref nMaxComponentHistoryID, ref dtMaxComponentHistoryTime);
            else if (type == Section.ComponentType.LINK)
                return WriteSectionLinkHistory(dbMgr, nHistoryID, nComponentID, data, sectionState, ref nMaxComponentHistoryID, ref dtMaxComponentHistoryTime);

            return false;
        }
        char szDeli = (char)0x06;
        private string GetActionStepPath(TreeNode nodeStep)
        {
            string strPath = nodeStep.Text;

            while (nodeStep.Level > 1)
            {
                nodeStep = nodeStep.Parent;
                strPath = nodeStep.Text + szDeli + strPath;
            }

            return strPath;
        }

        private int GetMaxComponentCompleteCount(WebDBManager dbMgr, int nActionStepHistoryID, int nComponentID)
        {
            string strSQL = string.Format("select max(CompleteCount) from ComponentHistory where ComponentID = {0} and ActionStepHistoryID = {1}",
                nComponentID, nActionStepHistoryID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null)
                return -1;

            return arrResult.Count == 0 ? 0 : WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        // 
        private int GetHiWordProcessDirection(HistorySectionData data)
        {
            int nDirection = data.ProcessDirections;
            return (nDirection << 16);
        }

        // time1이 time2보다 이전 시간인가?(초 까지만 비교)
        private bool CompareSecond(DateTime time1, DateTime time2)
        {
            long data1 = time1.Year * 10000000000 + time1.Month * 100000000 + time1.Day * 1000000 + time1.Hour * 10000 + time1.Minute * 100 + time1.Second;
            long data2 = time2.Year * 10000000000 + time2.Month * 100000000 + time2.Day * 1000000 + time2.Hour * 10000 + time2.Minute * 100 + time2.Second;

            return data1 <= data2;
        }

        private bool WriteSectionEndPointHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionData data, Workstate.SectionState sectionState, ref int nMaxComponentHistoryID, ref DateTime dtMaxComponentHistoryTime)
        {
            bool justDBInput = false;
            int nStatus = 3;

            if (data.State == Workstate.State.INPUT)
            {
                justDBInput = true;
                nStatus = 4;
            }
            else if (data.State == Workstate.State.RUN)      // 실행중
                nStatus = 2;
            else if (data.State == Workstate.State.SKIP)     // 건너뛰기
                nStatus = 5;
            else if (data.State != Workstate.State.DONE)     // 실행 완료
                return false;

            int nDirection = GetHiWordProcessDirection(data);
            nStatus |= nDirection;

            nStatus = SetNoDetailStatus(nStatus);

            ISOPTreeContainer tree = ProxySOP.Instance.SOPTreeContainer;
            if (tree == null)
            {
                return false;
            }

            PanelSection panel = (PanelSection)data.Section.GetParent();

            TreeNode node = tree.FindActionStepNode(panel.ActionStepID);
            if (node == null)
                return false;

            string strPath = GetActionStepPath(node);

            int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
            if (nComponentID < 0)
                return false;

            SectionDataEndPoint sectionData = (SectionDataEndPoint)data.Section.Data;
            string strTask = sectionData.IsBegin ? strPath + " 시작" : strPath + " 완료";

            if (!sectionData.IsBegin)
                nCompleteCount++;

            int nSOPGenUserID = sectionState.AccessedUserID;//ProxySOP.Instance.SOPGenUserID;

            if (CompareSecond(data.Time, dtMaxComponentHistoryTime))
                //if (data.Time <= dtMaxComponentHistoryTime)
                data.Time = dtMaxComponentHistoryTime.AddSeconds(1.0);

            string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second);
            string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, CompleteCount, ShowBoard, AccessedUserID, CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, '{6}', {7}, {8}, {9}, 0, 0, 0, 0, NULL)",
                ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, nCompleteCount, data.ShowBoard ? 1 : 0, nSOPGenUserID);

            dtMaxComponentHistoryTime = data.Time;


            if (!data.NoDBWrite)
            {
                //lock (m_objBatchQueryLock)
                {
                    RollbackManager rollback = new RollbackManager();
                    //dbMgr.BeginBatch();

                    if (dbMgr.GetResultData(strSQL) == null)
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        //dbMgr.BatchRollback();
                        return false;
                    }
                    else
                        rollback.AddData(new RollbackData("Delete from ComponentHistory where ID = " + nMaxComponentHistoryID.ToString()));

                    //if (dbMgr.GetResultData(strSQL) == null)
                    //    return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;

                    if (WriteSectionDetail(dbMgr, nMaxComponentHistoryID, data, sectionState, nStatus, rollback))
                    {
                        //dbMgr.BatchCommit();
                    }
                    else
                    {
                        rollback.Rollback(dbMgr);
                        //dbMgr.BatchRollback();
                    }
                }
            }
            else
                nMaxComponentHistoryID--;

            if (justDBInput)
            {
                // DB에 입력만 하고 리턴시킨다.
                return true;
            }

            UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)panel.Parent;

            Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;
            if (!sectionData.IsBegin && data.State == Workstate.State.DONE)
            {
                ActionStepDetailLog actionStepLog = sopLog.GetActionStepDetailLog(panel.ActionStepID, !tabPage.VirtualMode);
                if (actionStepLog == null || actionStepLog.BeginTime == null)
                    return false;
            }
            sopLog.AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, "-", "-", sectionData.IsBegin ? "시작" : "끝", data.Section.Title, strTask, nCompleteCount, true);

            return true;
        }

        private bool WriteSectionProcessHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionData data, Workstate.SectionState sectionState, ref int nMaxComponentHistoryID, ref DateTime dtMaxComponentHistoryTime)
        {
            int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);

            if (nComponentID < 0)
                return false;

            int nStatus = 0;
            string strStatus = "";
            bool justDBInput = false;

            if (data.State == Workstate.State.NORMAL)        // 대기
            {
                nStatus = 1;
                strStatus = "대기";
            }
            else if (data.State == Workstate.State.INPUT)    // 입력 대기
            {
                nStatus = 4;
                strStatus = "입력대기";
                justDBInput = true;
                // 입력 대기는 로그를 기록하지 않는다.
                //return true;
            }
            else if (data.State == Workstate.State.RUN)      // 실행중
            {
                nStatus = 2;
                strStatus = "실행중";
            }
            else if (data.State == Workstate.State.SKIP)     // 건너뛰기
            {
                nStatus = 5;
                strStatus = "건너뛰기";
            }
            else if (data.State == Workstate.State.DONE)     // 실행 완료
            {
                nStatus = 3;
                strStatus = "실행 완료";
                nCompleteCount++;
            }
            else
                return true;

            int nSOPGenUserID = sectionState.AccessedUserID;//ProxySOP.Instance.SOPGenUserID;

            int nDirection = GetHiWordProcessDirection(data);
            nStatus |= nDirection;

            nStatus = SetNoDetailStatus(nStatus);

            if (CompareSecond(data.Time, dtMaxComponentHistoryTime))
                //if (data.Time <= dtMaxComponentHistoryTime)
                data.Time = dtMaxComponentHistoryTime.AddSeconds(1.0);

            string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second);

            string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, NULL)",
                ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, "NULL", data.ShowBoard ? 1 : 0, nCompleteCount, nSOPGenUserID, data.CheckNotify1, data.CheckNotify2, data.CheckedRun, data.CheckedComplete);

            dtMaxComponentHistoryTime = data.Time;

            if (!data.NoDBWrite)
            {
                //lock (m_objBatchQueryLock)
                {
                    RollbackManager rollback = new RollbackManager();
                    //dbMgr.BeginBatch();

                    if (dbMgr.GetResultData(strSQL) == null)
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        //dbMgr.BatchRollback();
                        return false;
                    }
                    else
                        rollback.AddData(new RollbackData("Delete from ComponentHistory where ID = " + nMaxComponentHistoryID.ToString()));
                    //if (dbMgr.GetResultData(strSQL) == null)
                    //    return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;

                    if (WriteSectionDetail(dbMgr, nMaxComponentHistoryID, data, sectionState, nStatus, rollback))
                    {
                        //dbMgr.BatchCommit();
                    }
                    else
                    {
                        rollback.Rollback(dbMgr);
                        //dbMgr.BatchRollback();
                    }
                }
            }
            else
                nMaxComponentHistoryID--;

            PanelSection panel = (PanelSection)data.Section.GetParent();

            if (justDBInput)
            {
                // DB에 입력만 하고 리턴시킨다.
                return true;
            }

            //PanelSectionEx panel = (PanelSectionEx)data.Section.GetParent();
            SectionDataProcess sectionData = (SectionDataProcess)data.Section.Data;
            SectionProcess section = (SectionProcess)data.Section;

            string strTeamNameList = GetTeamNameList(sectionData.TeamList);
            UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)panel.Parent;

            if (tabPage == null)
                return false;

            Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;
            if (sopLog != null)
                sopLog.AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), strTeamNameList, "프로세스",
                section.TextUP, strStatus, nCompleteCount, true);
            return true;
        }

        private bool WriteSectionDetail(WebDBManager dbMgr, int nComponentHistoryID, HistorySectionData data, SectionState sectionState, int nStatus, RollbackManager rollback)
        {
            foreach (KeyValuePair<int, List<HistorySectionData.DetailData>> pair in data.HistoryDetailDatas)
            {
                int nDetailCount = pair.Value.Count;

                for (int i = nDetailCount - 1; i >= 0; i--)
                //foreach (HistorySectionData.DetailData detail in pair.Value)
                {
                    HistorySectionData.DetailData detail = (HistorySectionData.DetailData)pair.Value[i];

                    // 이미 DB에 기록된 데이터는 다시 기록하지 않는다.
                    if (detail.ComponentHistoryID > 0)
                        continue;

                    if (detail.DataIndex == null)
                        continue;

                    string strTime = "NULL";

                    if (detail.Time != null)
                    {
                        strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", ToShortDateString(detail.Time.Data), detail.Time.Data.Hour, detail.Time.Data.Minute, detail.Time.Data.Second);
                    }

                    int nID = GetMaxIDBatch("ComponentHistoryDetail", dbMgr) + 1;

                    string strSQL = string.Format("Insert into ComponentHistoryDetail (ID, ComponentHistoryID, DataIndex, Datai, Dataf, Datas, Time) values ({0}, {1}, {2}",
                        nID, nComponentHistoryID, detail.DataIndex.Data);

                    if (detail.Datai != null)
                        strSQL += ", " + detail.Datai.Data.ToString() + ", NULL, NULL, " + strTime + ")";
                    else if (detail.Dataf != null)
                        strSQL += ", NULL, " + detail.Dataf.Data.ToString() + ", NULL, " + strTime + ")";
                    else if (detail.Datas != null)
                        strSQL += ", NULL, NULL, '" + detail.Datas + "'," + strTime + ")";
                    else
                        continue;

                    if (dbMgr.GetResultData(strSQL) == null)
                        //if (dbMgr.GetBatchData(strSQL) == null)
                        return false;
                    else
                        rollback.AddData(new RollbackData("Delete from ComponentHistoryDetail where ID = " + nID.ToString()));

                    SetUnindexingDetailData(sectionState, detail, nComponentHistoryID);

                    // ActionStepHistory에 가장 마지막에 Access한 User를 업데이트 해준다.
                    strSQL = string.Format("update ActionStepHistory set LastAccessedUserID = {0} where ID = (select ActionStepHistoryID from ComponentHistory where ID = {1})",
                        ProxySOP.Instance.SOPGenUserID, nComponentHistoryID);

                    if (dbMgr.GetResultData(strSQL) == null)
                        //if (dbMgr.GetBatchData(strSQL) == null)
                        return false;
                    else
                    {
                        // 그리 중요하지 않으니 Rollback은 건너뛴다.
                    }

                    detail.ComponentHistoryID = nComponentHistoryID;
                }
            }

            // ComponentHistoryDetail이 생성되었거나 처리가 끝난 ComponentHistory 상태로 만든다.
            nStatus = SetDetailStatus(nStatus);

            string query = string.Format("Update ComponentHistory set Status = {0} where ID = {1}", nStatus, nComponentHistoryID);
            return dbMgr.GetResultData(query) != null;
        }

        // detail이 sectionState에 ComponentHistoryID가 부여되지 않은채로 저장되어 있을 경우
        // ComponentHistoryID로 Index를 부여하여 다시 저장한다.
        private void SetUnindexingDetailData(SectionState sectionState, HistorySectionData.DetailData detail, int nComponentHistoryID)
        {
            if (sectionState == null)
                return;

            List<HistorySectionData.DetailData> details;

            if (sectionState.DetailDatas.TryGetValue(-1, out details))
            {
                if (details.Contains(detail))
                {
                    // detail은 index가 부여된 곳으로 옮겨야 하므로 임시 저장소에서 삭제한다.
                    details.Remove(detail);

                    // 임시 저장소가 비어있으면 없앤다.
                    if (details.Count == 0)
                        sectionState.DetailDatas.Remove(-1);

                    if (!sectionState.DetailDatas.TryGetValue(nComponentHistoryID, out details))
                    {
                        details = new List<HistorySectionData.DetailData>();
                        sectionState.DetailDatas[nComponentHistoryID] = details;
                    }

                    details.Add(detail);
                }
            }
        }

        private string GetTeamNameList(ArrayList arrSOPTeams)
        {
            string strTeamNameList = "";

            foreach (SOPTeam team in arrSOPTeams)
            {
                if (strTeamNameList.Length == 0)
                    strTeamNameList = team.TeamName;
                else
                    strTeamNameList += ", " + team.TeamName;
            }

            return strTeamNameList;
        }

        private bool WriteSectionDecisionHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionDecisionData data, Workstate.SectionState sectionState, ref int nMaxComponentHistoryID, ref DateTime dtMaxComponentHistoryTime)
        {
            int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
            if (nComponentID < 0) return false;

            int nStatus = 0;
            bool justDBInput = false;

            if (data.State == Workstate.State.NORMAL)        // 대기
                nStatus = 1;
            else if (data.State == Workstate.State.INPUT)    // 입력 대기
            {
                nStatus = 4;
                justDBInput = true;
                // 입력 대기는 로그를 기록하지 않는다.
                //return true;
            }
            else if (data.State == Workstate.State.RUN)      // 실행중
                nStatus = 2;
            else if (data.State == Workstate.State.SKIP)     // 건너뛰기
                nStatus = 5;
            else if (data.State == Workstate.State.DONE)     // 실행 완료
            {
                nStatus = 3;
                nCompleteCount++;
            }
            else
                return true;

            int nDirection = GetHiWordProcessDirection(data);
            nStatus |= nDirection;

            nStatus = SetNoDetailStatus(nStatus);

            PanelSection panel = (PanelSection)data.Section.GetParent();
            string strTask = "NULL", strDescription = "NULL";

            if (data.NextSection != null && data.State != Workstate.State.NORMAL)
            {
                string strArrow = "";
                foreach (Arrow arrow in data.Section.Arrows)
                {
                    Section EndSection = arrow.EndLink;

                    if (data.NextSection.Data.ID == EndSection.Data.ID)
                    {
                        strArrow = arrow.Text;
                        break;
                    }
                }

                int nNextComponentID = panel.GetComponentID(data.NextSection);

                if (data.NextSection.GetComponentType() == Section.ComponentType.PROCESS)
                {
                    if (strArrow.Length > 0)
                        strTask = string.Format("'({0})로 분기'", strArrow);
                    else
                    {
                        SectionProcess section = (SectionProcess)data.NextSection;

                        if (section.TextUP.Length > 0)
                            strTask = string.Format("'({0})로 분기'", section.TextUP);
                        else
                            strTask = string.Format("'{0}로 분기'", section.Data.ComponentID);
                    }
                }
                else
                {
                    if (strArrow.Length > 0)
                        strTask = string.Format("'({0})로 분기'", strArrow);
                    else
                    {
                        Section section = (Section)data.NextSection;

                        if (section.Title.Length > 0)
                            strTask = string.Format("'({0})로 분기'", section.Title);
                        else
                            strTask = string.Format("'{0}로 분기'", section.Data.ComponentID);
                    }
                }

                strDescription = "'" + data.NextSection.Data.ComponentID + "'";
            }


            int nSOPGenUserID = sectionState.AccessedUserID;//ProxySOP.Instance.SOPGenUserID;

            if (CompareSecond(data.Time, dtMaxComponentHistoryTime))
                //if (data.Time <= dtMaxComponentHistoryTime)
                data.Time = dtMaxComponentHistoryTime.AddSeconds(1.0);

            string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second);

            string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, 0, 0, 0, 0, {10})",
                ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, nCompleteCount, nSOPGenUserID, strDescription);

            dtMaxComponentHistoryTime = data.Time;

            if (!data.NoDBWrite)
            {
                //lock (m_objBatchQueryLock)
                {
                    RollbackManager rollback = new RollbackManager();
                    //dbMgr.BeginBatch();

                    if (dbMgr.GetResultData(strSQL) == null)
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        //dbMgr.BatchRollback();
                        return false;
                    }
                    else
                        rollback.AddData(new RollbackData("Delete from ComponentHistory where ID = " + nMaxComponentHistoryID.ToString()));
                    //if (dbMgr.GetResultData(strSQL) == null)
                    //    return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;

                    if (WriteSectionDetail(dbMgr, nMaxComponentHistoryID, data, sectionState, nStatus, rollback))
                    {
                        //dbMgr.BatchCommit();
                    }
                    else
                    {
                        rollback.Rollback(dbMgr);
                        //dbMgr.BatchRollback();
                    }
                }
            }
            else
                nMaxComponentHistoryID--;

            if (justDBInput)
            {
                // DB에 입력만 하고 리턴시킨다.
                return true;
            }

            UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)panel.Parent;

            //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "분기", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);

            Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;
            if (sopLog != null)
                sopLog.AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "분기", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);

            return true;
        }

        private bool WriteSectionInternalHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionInternalData data, Workstate.SectionState sectionState, ref int nMaxComponentHistoryID, ref DateTime dtMaxComponentHistoryTime)
        {
            int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
            if (nComponentID < 0) return false;

            int nStatus = 0;
            bool justDBInput = false;

            if (data.State == Workstate.State.NORMAL)        // 대기
                nStatus = 1;
            else if (data.State == Workstate.State.INPUT)    // 입력 대기
            {
                nStatus = 4;
                justDBInput = true;
                // 입력 대기는 로그를 기록하지 않는다.
                //return true;
            }
            else if (data.State == Workstate.State.RUN)      // 실행중
                nStatus = 2;
            else if (data.State == Workstate.State.SKIP)     // 건너뛰기
                nStatus = 5;
            else if (data.State == Workstate.State.DONE)     // 실행 완료
            {
                nStatus = 3;
                nCompleteCount++;
            }
            else
                return true;

            int nDirection = GetHiWordProcessDirection(data);
            nStatus |= nDirection;

            nStatus = SetNoDetailStatus(nStatus);

            string strTask = "";

            if (data.State == Workstate.State.RUN || data.State == Workstate.State.DONE)
            {
                //TODO : MWKIM 영흥에서는 PC Popup Message 미사용
                //if (data.UsePopupMessage)
                //{
                //    if (strTask.Length == 0)
                //        strTask = "PC Popup Message 발송";
                //    else
                //        strTask += ", PC Popup Message 발송";
                //}

                if (data.UseSMS)
                {
                    if (strTask.Length == 0)
                        strTask = "문자 메시지 발송";
                    else
                        strTask += ", 문자 메시지 발송";
                }

                if (data.UseBroadcast)
                {
                    if (strTask.Length == 0)
                        strTask = "사내 방송 실시";
                    else
                        strTask += ", 사내 방송 실시";
                }

                if (strTask.Length > 0)
                    strTask = "'" + strTask + "'";
                else
                    strTask = "NULL";
            }
            else
                strTask = "NULL";


            int nSOPGenUserID = sectionState.AccessedUserID;//ProxySOP.Instance.SOPGenUserID;

            if (CompareSecond(data.Time, dtMaxComponentHistoryTime))
                //if (data.Time <= dtMaxComponentHistoryTime)
                data.Time = dtMaxComponentHistoryTime.AddSeconds(1.0);

            string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second);

            string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, 0, {11}, {12}, NULL)",
                ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, nCompleteCount, nSOPGenUserID, data.CheckNotify1, data.CheckedRun, data.CheckedComplete);

            dtMaxComponentHistoryTime = data.Time;

            if (!data.NoDBWrite)
            {
                //lock (m_objBatchQueryLock)
                {
                    RollbackManager rollback = new RollbackManager();
                    //dbMgr.BeginBatch();

                    if (dbMgr.GetResultData(strSQL) == null)
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        //dbMgr.BatchRollback();
                        return false;
                    }
                    else
                        rollback.AddData(new RollbackData("Delete from ComponentHistory where ID = " + nMaxComponentHistoryID.ToString()));
                    //if (dbMgr.GetResultData(strSQL) == null)
                    //    return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;

                    if (WriteSectionDetail(dbMgr, nMaxComponentHistoryID, data, sectionState, nStatus, rollback))
                    {
                        //dbMgr.BatchCommit();
                    }
                    else
                    {
                        rollback.Rollback(dbMgr);
                        //dbMgr.BatchRollback();
                    }
                }
            }
            else
                nMaxComponentHistoryID--;

            PanelSection panel = (PanelSection)data.Section.GetParent();

            if (justDBInput)
            {
                // DB에 입력만 하고 리턴시킨다.
                return true;
            }

            //PanelSectionEx panel = (PanelSectionEx)data.Section.GetParent();
            UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)panel.Parent;

            //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "내부 상황전파", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);

            Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;
            if (sopLog != null)
                sopLog.AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "내부 상황전파", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);

            return true;
        }

        private string GetExternalTeamList(ArrayList arrTeams)
        {
            string strTeamList = "";

            foreach (ExternalTeamData team in arrTeams)
            {
                if (strTeamList.Length == 0)
                    strTeamList = team.TeamName;
                else
                    strTeamList += ", " + team.TeamName;
            }

            if (strTeamList.Length > 0)
                strTeamList = "'" + strTeamList + "'";
            else
                strTeamList = "NULL";

            return strTeamList;
        }

        // ComponentHistory가 DB에 입력되었지만 아직 ComponentHistoryDetail이 생성되지 않은 상태임을 표시한다.
        // SOPWebServer에서 ComponentHistoryDetail이 생성되기 전에 ComponentHistory만 읽어가지 않도록 표시를 해둔다.
        private int SetNoDetailStatus(int nStatus)
        {
            return nStatus | 0x100;
        }

        // ComponentHistoryDetail이 생성되었거나 처리가 끝난 ComponentHistory 상태로 만든다.
        private int SetDetailStatus(int nStatus)
        {
            if ((nStatus & 0x100) == 0x100)
                return nStatus - 0x100;

            return nStatus;
        }

        private bool WriteSectionExternalHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionExternalData data, Workstate.SectionState sectionState, ref int nMaxComponentHistoryID, ref DateTime dtMaxComponentHistoryTime)
        {
            int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
            if (nComponentID < 0) return false;

            int nStatus = 0;
            bool justDBInput = false;

            if (data.State == Workstate.State.NORMAL)        // 대기
                nStatus = 1;
            else if (data.State == Workstate.State.INPUT)    // 입력 대기
            {
                nStatus = 4;
                justDBInput = true;
                // 입력 대기는 로그를 기록하지 않는다.
                //return true;
            }
            else if (data.State == Workstate.State.RUN)      // 실행중
                nStatus = 2;
            else if (data.State == Workstate.State.SKIP)     // 건너뛰기
                nStatus = 5;
            else if (data.State == Workstate.State.DONE)     // 실행 완료
            {
                nStatus = 3;
                nCompleteCount++;
            }
            else
                return true;

            int nDirection = GetHiWordProcessDirection(data);
            nStatus |= nDirection;

            nStatus = SetNoDetailStatus(nStatus);

            string strTask = "";

            if (data.State == Workstate.State.RUN || data.State == Workstate.State.DONE)
            {
                SectionDataExternal sectionData = (SectionDataExternal)data.Section.Data;

                if (data.UseSMS)
                {
                    if (strTask.Length == 0)
                        strTask = string.Format("문자 메시지 발송(수신처 : {0})", GetExternalTeamList(sectionData.SMSReceivers));
                    else
                        strTask += string.Format(", 문자 메시지 발송(수신처 : {0})", GetExternalTeamList(sectionData.SMSReceivers));
                }

                if (data.UseFax)
                {
                    if (strTask.Length == 0)
                        strTask = string.Format("Fax 발송(수신처 : {0})", GetExternalTeamList(sectionData.FaxReceivers));
                    else
                        strTask += string.Format(", Fax 발송(수신처 : {0})", GetExternalTeamList(sectionData.FaxReceivers));
                }

                if (strTask.Length > 0)
                    strTask = "'" + strTask + "'";
                else
                    strTask = "NULL";
            }
            else
                strTask = "NULL";


            int nSOPGenUserID = sectionState.AccessedUserID;//ProxySOP.Instance.SOPGenUserID;

            if (CompareSecond(data.Time, dtMaxComponentHistoryTime))
                //if (data.Time <= dtMaxComponentHistoryTime)
                data.Time = dtMaxComponentHistoryTime.AddSeconds(1.0);

            string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second);

            string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, NULL)",
                ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, nCompleteCount, nSOPGenUserID, data.CheckNotify1, data.CheckNotify2, data.CheckedRun, data.CheckedComplete);

            dtMaxComponentHistoryTime = data.Time;

            if (!data.NoDBWrite)
            {
                //lock (m_objBatchQueryLock)
                {
                    RollbackManager rollback = new RollbackManager();
                    //dbMgr.BeginBatch();

                    if (dbMgr.GetResultData(strSQL) == null)
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        //dbMgr.BatchRollback();
                        return false;
                    }
                    else
                        rollback.AddData(new RollbackData("Delete from ComponentHistory where ID = " + nMaxComponentHistoryID.ToString()));
                    //if (dbMgr.GetResultData(strSQL) == null)
                    //    return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;

                    if (WriteSectionDetail(dbMgr, nMaxComponentHistoryID, data, sectionState, nStatus, rollback))
                    {
                        //dbMgr.BatchCommit();
                    }
                    else
                    {
                        rollback.Rollback(dbMgr);
                        //dbMgr.BatchRollback();
                    }
                }
            }
            else
                nMaxComponentHistoryID--;

            PanelSection panel = (PanelSection)data.Section.GetParent();

            if (justDBInput)
            {
                // DB에 입력만 하고 리턴시킨다.
                return true;
            }

            //PanelSectionEx panel = (PanelSectionEx)data.Section.GetParent();
            UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)panel.Parent;

            //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "외부 상황전파", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);

            Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;
            if (sopLog != null)
                sopLog.AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "외부 상황전파", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);

            return true;
        }

        private bool WriteSectionTransmissionHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionTransmissionData data, Workstate.SectionState sectionState, ref int nMaxComponentHistoryID, ref DateTime dtMaxComponentHistoryTime)
        {
            int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
            if (nComponentID < 0) return false;

            int nStatus = 0;
            string strStatus = "";
            bool justDBInput = false;

            if (data.State == Workstate.State.NORMAL)        // 대기
            {
                nStatus = 1;
                strStatus = "대기";
            }
            else if (data.State == Workstate.State.INPUT)    // 입력 대기
            {
                nStatus = 4;
                strStatus = "입력대기";
                justDBInput = true;
                // 입력 대기는 로그를 기록하지 않는다.
                //return true;
            }
            else if (data.State == Workstate.State.RUN)      // 실행중
            {
                nStatus = 2;
                strStatus = "실행중";
            }
            else if (data.State == Workstate.State.SKIP)     // 건너뛰기
            {
                nStatus = 5;
                strStatus = "건너뛰기";
            }
            else if (data.State == Workstate.State.DONE)     // 실행 완료
            {
                nStatus = 3;
                strStatus = "실행 완료";
                nCompleteCount++;
            }
            else
                return true;

            int nDirection = GetHiWordProcessDirection(data);
            nStatus |= nDirection;

            nStatus = SetNoDetailStatus(nStatus);

            string strTask = "";

            if (data.State == Workstate.State.RUN || data.State == Workstate.State.DONE)
            {
                //TODO : MWKIM 영흥에서는 PC Popup Message 미사용
                //if (data.UsePopupMessage)
                //{
                //    if (strTask.Length == 0)
                //        strTask = "PC Popup Message 발송";
                //    else
                //        strTask += ", PC Popup Message 발송";
                //}

                if (data.UseSMS)
                {
                    if (strTask.Length == 0)
                        strTask = "문자 메시지 발송";
                    else
                        strTask += ", 문자 메시지 발송";
                }

                if (data.UseBroadcast)
                {
                    if (strTask.Length == 0)
                        strTask = "사내 방송 실시";
                    else
                        strTask += ", 사내 방송 실시";
                }

                SectionDataTransmission sectionData = (SectionDataTransmission)data.Section.Data;
                SectionDataTransmission.ExternalData ExternalData = (SectionDataTransmission.ExternalData)sectionData.DataExternal;

                if (data.UseExSMS)
                {
                    if (strTask.Length == 0)
                        strTask = string.Format("문자 메시지 발송(수신처 : {0})", GetExternalTeamList(ExternalData.SMSReceivers));
                    else
                        strTask += string.Format(", 문자 메시지 발송(수신처 : {0})", GetExternalTeamList(ExternalData.SMSReceivers));
                }

                if (data.UseExFax)
                {
                    if (strTask.Length == 0)
                        strTask = string.Format("Fax 발송(수신처 : {0})", GetExternalTeamList(ExternalData.FaxReceivers));
                    else
                        strTask += string.Format(", Fax 발송(수신처 : {0})", GetExternalTeamList(ExternalData.FaxReceivers));
                }

                if (strTask.Length > 0)
                    strTask = "'" + strTask + "'";
                else
                    strTask = "NULL";
            }
            else
                strTask = "NULL";

            int nSOPGenUserID = sectionState.AccessedUserID;//ProxySOP.Instance.SOPGenUserID;

            if (CompareSecond(data.Time, dtMaxComponentHistoryTime))
                //if (data.Time <= dtMaxComponentHistoryTime)
                data.Time = dtMaxComponentHistoryTime.AddSeconds(1.0);

            string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second);

            string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, NULL)",
                ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, nCompleteCount, nSOPGenUserID, data.CheckNotify1, data.CheckNotify2, data.CheckedRun, data.CheckedComplete);

            dtMaxComponentHistoryTime = data.Time;

            if (!data.NoDBWrite)
            {
                //lock (m_objBatchQueryLock)
                {
                    RollbackManager rollback = new RollbackManager();
                    //dbMgr.BeginBatch();

                    if (dbMgr.GetResultData(strSQL) == null)
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        //dbMgr.BatchRollback();
                        return false;
                    }
                    else
                        rollback.AddData(new RollbackData("Delete from ComponentHistory where ID = " + nMaxComponentHistoryID.ToString()));
                    //if (dbMgr.GetResultData(strSQL) == null)
                    //    return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;

                    if (WriteSectionDetail(dbMgr, nMaxComponentHistoryID, data, sectionState, nStatus, rollback))
                    {
                        //dbMgr.BatchCommit();
                    }
                    else
                    {
                        rollback.Rollback(dbMgr);
                        //dbMgr.BatchRollback();
                    }
                }
            }
            else
                nMaxComponentHistoryID--;

            PanelSection panel = (PanelSection)data.Section.GetParent();

            if (justDBInput)
            {
                // DB에 입력만 하고 리턴시킨다.
                return true;
            }

            //PanelSectionEx panel = (PanelSectionEx)data.Section.GetParent();
            UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)panel.Parent;

            //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "상황전파", data.Section.Title, strStatus/*strTask == "NULL" ? "-" : strTask*/, nCompleteCount, true);

            Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;
            if (sopLog != null)
                sopLog.AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "상황전파", data.Section.Title, strStatus/*strTask == "NULL" ? "-" : strTask*/, nCompleteCount, true);

            return true;
        }

        private bool GetSOPTask(WebDBManager dbMgr, ArrayList arrResult, int nBeginIndex, ref string strSOPTask)
        {
            string strCategoryName = WebDBManager.GetStringField(arrResult[nBeginIndex], "");
            string strSubCategoryName = WebDBManager.GetStringField(arrResult[nBeginIndex + 1], "");
            string strDisasterName = WebDBManager.GetStringField(arrResult[nBeginIndex + 2], "");
            string strStepName = WebDBManager.GetStringField(arrResult[nBeginIndex + 3], "");
            bool isRegular = WebDBManager.GetIntField(arrResult[nBeginIndex + 4].ToString(), 0) == 0 ? false : true;
            bool isNormal = WebDBManager.GetIntField(arrResult[nBeginIndex + 5].ToString(), 0) == 0 ? false : true;

            if (!isRegular)
                strSOPTask = "[비등록모드] ";

            strSOPTask += string.Format("{0} {1}/{2}/{3}/{4}", isNormal ? "[평일 주간모드]" : "[야간 및 휴일 모드]",
                strCategoryName, strSubCategoryName, strDisasterName, strStepName);

            return true;
        }

        private string GetTransSOPTask(WebDBManager dbMgr, HistorySectionData data)
        {
            SectionDataTransSOP sectionData = (SectionDataTransSOP)data.Section.Data;
            PanelSection panel = (PanelSection)data.Section.GetParent();

            string strFormat = "select dc.CategoryName, sc.SubCategoryName, d.DisasterName, a.StepName, v.isRegular, v.isNormal ";
            strFormat += "from DisasterCategory as dc, SubDisasterCategory as sc, Disaster as d, ActionStep as a, Version as v ";
            strFormat += "where a.ID in ({0}, {1}) and a.DisasterID = d.ID and d.VersionID = v.ID and d.SubDisasterID = sc.ID and sc.DisasterID = dc.ID";

            string strSQL = string.Format(strFormat, panel.ActionStepID, sectionData.LinkedActionStepID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return "NULL";

            string strTaskCurrent = "";
            string strTaskNext = "";

            if (!GetSOPTask(dbMgr, arrResult, 0, ref strTaskCurrent))
                return "NULL";
            if (!GetSOPTask(dbMgr, arrResult, 6, ref strTaskNext))
                return "NULL";

            return "'" + strTaskCurrent + "에서 " + strTaskNext + "로 전환'";
        }

        private bool WriteSectionTransSOPHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionData data, Workstate.SectionState sectionState, ref int nMaxComponentHistoryID, ref DateTime dtMaxComponentHistoryTime)
        {
            int nStatus = 0;
            bool justDBInput = false;

            if (data.State == Workstate.State.NORMAL)        // 대기
                nStatus = 1;
            else if (data.State == Workstate.State.INPUT)    // 입력 대기
            {
                nStatus = 4;
                justDBInput = true;
                // 입력 대기는 로그를 기록하지 않는다.
                //return true;
            }
            else if (data.State == Workstate.State.RUN)      // 실행중
                nStatus = 2;
            else if (data.State == Workstate.State.SKIP)     // 건너뛰기
                nStatus = 5;
            else if (data.State == Workstate.State.DONE)     // 실행 완료
                nStatus = 3;
            else
                return true;

            int nDirection = GetHiWordProcessDirection(data);
            nStatus |= nDirection;

            nStatus = SetNoDetailStatus(nStatus);

            string strTask = "NULL";

            if (data.State == Workstate.State.RUN || data.State == Workstate.State.DONE)
            {
                strTask = GetTransSOPTask(dbMgr, data);
            }

            int nSOPGenUserID = sectionState.AccessedUserID;//ProxySOP.Instance.SOPGenUserID;

            if (CompareSecond(data.Time, dtMaxComponentHistoryTime))
                //if (data.Time <= dtMaxComponentHistoryTime)
                data.Time = dtMaxComponentHistoryTime.AddSeconds(1.0);

            string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second);

            string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, NULL, {8}, 0, 0, 0, 0, NULL)",
                ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, nSOPGenUserID);

            dtMaxComponentHistoryTime = data.Time;

            if (!data.NoDBWrite)
            {
                //lock (m_objBatchQueryLock)
                {
                    RollbackManager rollback = new RollbackManager();
                    //dbMgr.BeginBatch();

                    if (dbMgr.GetResultData(strSQL) == null)
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        //dbMgr.BatchRollback();
                        return false;
                    }
                    else
                        rollback.AddData(new RollbackData("Delete from ComponentHistory where ID = " + nMaxComponentHistoryID.ToString()));
                    //if (dbMgr.GetResultData(strSQL) == null)
                    //    return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;

                    if (WriteSectionDetail(dbMgr, nMaxComponentHistoryID, data, sectionState, nStatus, rollback))
                    {
                        //dbMgr.BatchCommit();
                    }
                    else
                    {
                        rollback.Rollback(dbMgr);
                        //dbMgr.BatchRollback();
                    }
                }
            }
            else
                nMaxComponentHistoryID--;

            PanelSection panel = (PanelSection)data.Section.GetParent();

            if (justDBInput)
            {
                // DB에 입력만 하고 리턴시킨다.
                return true;
            }

            //PanelSectionEx panel = (PanelSectionEx)data.Section.GetParent();
            UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)panel.Parent;

            //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "SOP 전환", data.Section.Title, strTask == "NULL" ? "-" : strTask, -1, true);

            Log.ISOPLogContainer sopLog = ProxySOP.Instance.SOPLogContainer;
            if (sopLog != null)
                sopLog.AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "SOP 전환", data.Section.Title, strTask == "NULL" ? "-" : strTask, -1, true);

            return true;
        }

        private bool WriteSectionLinkHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionData data, Workstate.SectionState sectionState, ref int nMaxComponentHistoryID, ref DateTime dtMaxComponentHistoryTime)
        {
            int nStatus = 0;

            if (data.State == Workstate.State.NORMAL)        // 대기
                nStatus = 1;
            else if (data.State == Workstate.State.INPUT)    // 입력 대기
            {
                nStatus = 4;
                // 입력 대기는 로그를 기록하지 않는다.
                //return true;
            }
            else if (data.State == Workstate.State.RUN)      // 실행중
                nStatus = 2;
            else if (data.State == Workstate.State.SKIP)     // 건너뛰기
                nStatus = 5;
            else if (data.State == Workstate.State.DONE)     // 실행 완료
                nStatus = 3;
            else
                return true;

            int nDirection = GetHiWordProcessDirection(data);
            nStatus |= nDirection;

            nStatus = SetNoDetailStatus(nStatus);

            string strTask = "NULL";

            if (data.State == Workstate.State.RUN || data.State == Workstate.State.DONE)
            {
                strTask = GetTransSOPTask(dbMgr, data);
            }

            int nSOPGenUserID = sectionState.AccessedUserID;//ProxySOP.Instance.SOPGenUserID;

            if (CompareSecond(data.Time, dtMaxComponentHistoryTime))
                //if (data.Time <= dtMaxComponentHistoryTime)
                data.Time = dtMaxComponentHistoryTime.AddSeconds(1.0);

            string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", ToShortDateString(data.Time), data.Time.Hour, data.Time.Minute, data.Time.Second);

            string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, CheckedRun, CheckedComplete, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, NULL, {8}, 0, 0, 0, 0, NULL)",
                ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, nSOPGenUserID);

            dtMaxComponentHistoryTime = data.Time;

            if (!data.NoDBWrite)
            {
                //lock (m_objBatchQueryLock)
                {
                    RollbackManager rollback = new RollbackManager();
                    //dbMgr.BeginBatch();

                    if (dbMgr.GetResultData(strSQL) == null)
                    //if (dbMgr.GetBatchData(strSQL) == null)
                    {
                        //dbMgr.BatchRollback();
                        return false;
                    }
                    else
                        rollback.AddData(new RollbackData("Delete from ComponentHistory where ID = " + nMaxComponentHistoryID.ToString()));
                    //if (dbMgr.GetResultData(strSQL) == null)
                    //    return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;

                    if (WriteSectionDetail(dbMgr, nMaxComponentHistoryID, data, sectionState, nStatus, rollback))
                    {
                        //dbMgr.BatchCommit();
                    }
                    else
                    {
                        rollback.Rollback(dbMgr);
                        //dbMgr.BatchRollback();
                    }
                }
            }
            else
                nMaxComponentHistoryID--;

            PanelSection panel = (PanelSection)data.Section.GetParent();

            // Link는 DB에만 기록한다.
            return true;
        }

        protected ActionStepInfo GetActionStep(int nActionStepID)
        {
            //FormMain frm = FormMain.Instance;
            //BarLevelTree tree = frm.GetPageHome().GetDockScenario().GetBarLevelTree();

            ISOPTreeContainer tree = ProxySOP.Instance.SOPTreeContainer;
            if (tree == null)
                return null;

            bool isRegular, isNormal;
            if (!tree.ReloadTree(nActionStepID, out isRegular, out isNormal))
                return null;

            TreeNode node = tree.FindActionStepNode(nActionStepID);
            string strFullPath = "";

            if (node == null)
            {
                if (!ReadDisasterFullPath(nActionStepID, szDeli, out strFullPath))
                    return null;
            }
            else
            {
                while (node.Level > 2)
                {
                    node = node.Parent;
                }

                TreeNode nodeDisaster = node;

                //SOPManager sopMgr = frm.SOPManager;

                strFullPath = nodeDisaster.Parent.Parent.Text + szDeli + nodeDisaster.Parent.Text + szDeli + nodeDisaster.Text;
            }

            Data.ISOPDataContainer sopMan = ProxySOP.Instance.SOPDataContainer;
            if (sopMan == null)
                return null;

            //bool isRegular = ProxySOP.Instance.RegisterMode;
            //bool isNormal = ProxySOP.Instance.NormalMode;
            Dictionary<string, DisasterInfo> dicSOP = sopMan.GetSOPDictionary(isRegular, isNormal);

            if (!dicSOP.ContainsKey(strFullPath))
                return null;

            DisasterInfo disaster = dicSOP[strFullPath];

            foreach (ActionStepInfo actionStep in disaster.ActionSteps)
            {
                if (actionStep.ActionStepID == nActionStepID)
                    return actionStep;
            }

            return null;
        }

        private bool ReadDisasterFullPath(int nActionStepID, char szDeli, out string strFullPath)
        {
            strFullPath = "";

            string strSQL = "select _as.ID, _as.StepName, d.DisasterName, sdc.SubCategoryName, dc.CategoryName ";
            strSQL += "from ActionStep as _as, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strSQL += "where _as.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and _as.ID = " + nActionStepID.ToString();

            ArrayList arrResult = ProxySOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            if (nResultCount != 5)
                return false;

            string strActionStepName = WebDBManager.GetStringField(arrResult[1], "");
            string strDisasterName = WebDBManager.GetStringField(arrResult[2], "");
            string strSubCategoryName = WebDBManager.GetStringField(arrResult[3], "");
            string strCategoryName = WebDBManager.GetStringField(arrResult[4], "");

            strFullPath = strCategoryName + szDeli + strSubCategoryName + szDeli + strDisasterName;
            return true;
        }

        public HistoryDisasterPosition FindHistoryDisasterPosition(int nActionStepID, bool isRealMode)
        {
            long nActionStepInfo = ((long)nActionStepID) << 32;
            if (isRealMode) nActionStepInfo |= 1;

            if (m_dicHistoryDisasterPosition.ContainsKey(nActionStepInfo))
                return m_dicHistoryDisasterPosition[nActionStepInfo];

            return null;
        }

        public HistoryDisasterNoPosition FindHistoryDisasterNoPosition(int nActionStepID, bool isRealMode)
        {
            long nActionStepInfo = ((long)nActionStepID) << 32;
            if (isRealMode) nActionStepInfo |= 1;

            if (m_dicHistoryDisasterNoPosition.ContainsKey(nActionStepInfo))
                return m_dicHistoryDisasterNoPosition[nActionStepInfo];

            return null;
        }

        public void RemoveHistoryDisasterPosition(int nActionStepID, bool isRealMode)
        {
            long nActionStepInfo = ((long)nActionStepID) << 32;
            if (isRealMode) nActionStepInfo |= 1;

            m_dicHistoryDisasterPosition.Remove(nActionStepInfo);
        }

        public void RemoveHistoryDisasterNoPosition(int nActionStepID, bool isRealMode)
        {
            long nActionStepInfo = ((long)nActionStepID) << 32;
            if (isRealMode) nActionStepInfo |= 1;

            m_dicHistoryDisasterNoPosition.Remove(nActionStepInfo);
        }

        public void AddHistoryDisasterPosition(int nActionStepHistoryID, int nActionStepID, bool isRealMode)
        {
            WebDBManager dbMgr = ProxySOP.Instance.DBManager;
            if (dbMgr == null)
                return;

            string szText = "select id, PosX, PosY, PosZ, FloorIndex, DisasterType, Description, BuildingID, BroadcastName from HistoryDisasterPos where HistoryActionSetpID = {0} and SiteID = {1}";
            string strSQL = string.Format(szText, nActionStepHistoryID, m_nSiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                float z = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                int nFloorIndex = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -999);
                string strDisasterType = WebDBManager.GetStringField(arrResult[i + 5], "");
                string strDescription = WebDBManager.GetStringField(arrResult[i + 6], "");
                string strBuildingID = WebDBManager.GetStringField(arrResult[i + 7], "");
                string szBroadcastName = WebDBManager.GetStringField(arrResult[i + 8], "");

                HistoryDisasterPosition position = new HistoryDisasterPosition();
                position.X = x;
                position.Y = y;
                position.Z = z;
                position.FloorIndex = nFloorIndex;
                position.HistoryActionStepID = nActionStepHistoryID;
                position.DisasterName = strDisasterType;
                position.PoistionName = strDescription;
                position.BuildingID = strBuildingID;
                position.BroadcastName = szBroadcastName;

                //SetPSM(strDisasterType, position);
                //SetAmountSnowfall(strDisasterType, position);

                long nActionStepInfo = MakeKey(nActionStepID, isRealMode);
                //long nActionStepInfo = ((long)nActionStepID) << 32;
                //if (isRealMode) nActionStepInfo |= 1;

                m_dicHistoryDisasterPosition[nActionStepInfo] = position;
            }
        }

        public void AddHistoryDisasterNoPosition(int nActionStepID, bool isRealMode, HistoryDisasterNoPosition info)
        {
            long nActionStepInfo = MakeKey(nActionStepID, isRealMode);
            //long nActionStepInfo = ((long)nActionStepID) << 32;
            //if (isRealMode) nActionStepInfo |= 1;

            m_dicHistoryDisasterNoPosition[nActionStepInfo] = info;
        }

        // Key : 상위 4바이트(ActionStepID)
        //       하위 4바이트(isRealMode, 1이면 실제 모드, 0이면 가상모드)
        private long MakeKey(int nActionStepID, bool isRealMode)
        {
            long nActionStepInfo = ((long)nActionStepID) << 32;
            if (isRealMode)
                nActionStepInfo |= 1;

            return nActionStepInfo;
        }

        /*private void SetPSM(string strDisasterType, HistoryDisasterPosition pos)
        {
            strDisasterType = GetDisasterInfoString(strDisasterType, "PSM");

            if (strDisasterType == null)
                return;

            string[] arrDatas = strDisasterType.Split('/');
            int nDataCount = arrDatas.Count();

            if (nDataCount == 1)
                return;

            if (arrDatas[0] == "유출사고")
            {
                pos.PSMMaterial = arrDatas[1];
                int nDistance;

                if (nDataCount >= 3 && int.TryParse(arrDatas[2], out nDistance))
                    pos.PSMDistance = nDistance;
            }
        }*/

        /*private void SetAmountSnowfall(string strDisasterType, HistoryDisasterNoPosition info)
        {
            strDisasterType = GetDisasterInfoString(strDisasterType, "AmountSnowfall");

            if (strDisasterType == null)
                return;

            info.AmountSnowfall = strDisasterType;
        }*/

        public static string GetDisasterInfoString(string str, string strTag)
        {
            if (str == null)
                return null;

            strTag = "[" + strTag + ":";
            int nIndex = str.IndexOf(strTag, StringComparison.CurrentCultureIgnoreCase);

            if (nIndex >= 0)
            {
                int nIndex2 = str.IndexOf(']', nIndex + strTag.Length);

                if (nIndex2 < 0)
                    return null;

                return str.Substring(nIndex + strTag.Length, nIndex2 - nIndex - strTag.Length).Trim();
            }

            return null;
        }
    }
}
