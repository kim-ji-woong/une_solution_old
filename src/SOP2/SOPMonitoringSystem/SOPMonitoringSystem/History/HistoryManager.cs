using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using Sections;

namespace SOPMonitoringSystem
{
    namespace History
    {
        public class HistoryManager
        {
            private static HistoryManager m_instance = new HistoryManager();
            private Thread m_thread = null;
            private ArrayList m_arrSectionHistory = new ArrayList();
            private ArrayList m_arrActionStepHistory = new ArrayList();

            // 현재 실행중인 ActionStepHistory
            // ActionStepID(0보다 크면 실제 모드, 0보다 작으면 모의훈련모드), ActionStepHistoryID
            private Dictionary<int, int> m_dicActionStepHistory = new Dictionary<int, int>();

            // 현재 실행중인 SOP를 포함하여 DB에 기록된 것까지 모든 ActionStep들의 실행 이력을 가진 변수
            private ActionStepHistory m_allActionStepHistory = new ActionStepHistory();

            // ActionStep별 마지막으로 읽은 ComponentHistory ID
            // ActionStepID, ComponentHistoryID
            private Dictionary<int, int> m_dicLastComponentHistory = new Dictionary<int, int>();
            // ActionStep별 모든 Section들
            // ActionStepID, ArrayList(Section)
            private Dictionary<int, ArrayList> m_dicActionStepSections = new Dictionary<int, ArrayList>();

            // Exit Thread
            private bool bExit = false;
            private bool closedThread = false;

            public bool Exit
            {
                get { return bExit; }
            }

            private bool m_isWorkingMonitor = false;

            // 실행중인 SOP에 대한 재난 위치 정보
            // Key : 상위 4바이트(ActionStepID)
            //       하위 4바이트(isRealMode, 1이면 실제 모드, 0이면 가상모드)
            private Dictionary<long, SOPMonitoringSystem.HistoryDiasterPosition> m_dicHistoryDisasterPosition = new Dictionary<long, SOPMonitoringSystem.HistoryDiasterPosition>();

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
                        /*System.Diagnostics.Trace.WriteLine("Before ExitThread");
                        Application.ExitThread();
                        System.Diagnostics.Trace.WriteLine("Before Exit");
                        Application.Exit();
                        System.Diagnostics.Trace.WriteLine("After Exit");*/
                        break;
                    }
                }
            }

            private HistoryManager()
            {
                m_thread = new Thread(HistoryThread);
                m_thread.Start();
            }

            public void SetActionStepHistory(int nActionStepID, int nActionStepHistoryID)
            {
                m_dicActionStepHistory[nActionStepID] = nActionStepHistoryID;
            }

			//public int GetActionStepHistory(int nActionStepID)
			//{
			//    if (m_dicActionStepHistory.ContainsKey(nActionStepID))
			//        return m_dicActionStepHistory[nActionStepID];

			//    return -1;
			//}

            // ActionStep별 마지막으로 저장된 ComponentHistory를 기억시킨다.
            public void SetLastComponentHistory(int nActionStepID, int nComponentHistoryID)
            {
                m_dicLastComponentHistory[nActionStepID] = nComponentHistoryID;
            }

            // nextSection : 분기문을 통하여 실행된 Section
            public HistorySectionDecisionData AddDecisionHistory(Sections.SectionDecision section, Sections.State state, int nProcessDirections, Sections.Section nextSection, bool noDBWrite, DateTime time, bool showBoard)
            {
                // 입력대기는 기록하지 않는다.
                //if (state == Sections.State.INPUT)
                //    return;

                HistorySectionDecisionData data = new HistorySectionDecisionData(time, state, nProcessDirections, section, nextSection);
                data.NoDBWrite = noDBWrite;
                data.ShowBoard = showBoard;
                m_arrSectionHistory.Add(data);

                return data;
            }

            // nextSection : 분기문을 통하여 실행된 Section
            public HistorySectionDecisionData AddDecisionHistory(Sections.SectionDecision section, Sections.State state, int nProcessDirections, Sections.Section nextSection = null, bool showBoard = false)
            {
                return AddDecisionHistory(section, state, nProcessDirections, nextSection, false, DateTime.Now, showBoard);
            }

            public HistorySectionInternalData AddInternalHistory(Sections.SectionInternal section, Sections.State state, int nProcessDirections, bool usePopupMessage, bool useSMS, bool useBroadcast, bool noDBWrite, DateTime time, bool showBoard, int nCheckedNotify1)
            {
                // 입력대기는 기록하지 않는다.
                //if (state == Sections.State.INPUT)
                //    return;

                HistorySectionInternalData data = new HistorySectionInternalData(time, state, nProcessDirections, section, usePopupMessage, useSMS, useBroadcast);
                data.NoDBWrite = noDBWrite;
                data.ShowBoard = showBoard;
                data.CheckNotify1 = nCheckedNotify1;
                m_arrSectionHistory.Add(data);

                return data;
            }

            public HistorySectionInternalData AddInternalHistory(Sections.SectionInternal section, Sections.State state, int nProcessDirections, int nCheckedNotify1, bool usePopupMessage = false, bool useSMS = false, bool useBroadcast = false, bool showBoard = false)
            {
                return AddInternalHistory(section, state, nProcessDirections, usePopupMessage, useSMS, useBroadcast, false, DateTime.Now, showBoard, nCheckedNotify1);
            }

            public HistorySectionExternalData AddExternalHistory(Sections.SectionExternal section, Sections.State state, int nProcessDirections, bool useSMS, bool useFax, bool noDBWrite, DateTime time, bool showBoard, int nCheckedNotify1, int nCheckedNotify2)
            {
                // 입력대기는 기록하지 않는다.
                //if (state == Sections.State.INPUT)
                //    return;

                HistorySectionExternalData data = new HistorySectionExternalData(time, state, nProcessDirections, section, useSMS, useFax);
                data.NoDBWrite = noDBWrite;
                data.ShowBoard = showBoard;
                data.CheckNotify1 = nCheckedNotify1;
                data.CheckNotify2 = nCheckedNotify2;
                m_arrSectionHistory.Add(data);

                return data;
            }

            public HistorySectionExternalData AddExternalHistory(Sections.SectionExternal section, Sections.State state, int nProcessDirections, int nCheckedNotify1, int nCheckedNotify2, bool useSMS = false, bool useFax = false, bool showBoard = false)
            {
                return AddExternalHistory(section, state, nProcessDirections, useSMS, useFax, false, DateTime.Now, showBoard, nCheckedNotify1, nCheckedNotify2);
            }

            public HistorySectionTransmissionData AddTransmissionHistory(Sections.SectionTransmission section, Sections.State state, int nProcessDirections, bool usePopupMessage, bool useSMS, bool useBroadcast, bool useExSMS, bool useExFax, bool noDBWrite, DateTime time, bool showBoard, int nCheckedNotify1, int nCheckedNotify2)
            {
                HistorySectionTransmissionData data = new HistorySectionTransmissionData(time, state, nProcessDirections, section, usePopupMessage, useSMS, useBroadcast, useExSMS, useExFax);
                data.NoDBWrite = noDBWrite;
                data.ShowBoard = showBoard;
                data.CheckNotify1 = nCheckedNotify1;
                data.CheckNotify2 = nCheckedNotify2;
                m_arrSectionHistory.Add(data);

                return data;
            }

            public HistorySectionTransmissionData AddTransmissionHistory(Sections.SectionTransmission section, Sections.State state, int nProcessDirections, int nCheckedNotify1, int nCheckedNotify2, bool usePopupMessage = false, bool useSMS = false, bool useBroadcast = false, bool useExSMS = false, bool useExFax = false, bool showBoard = false)
            {
                return AddTransmissionHistory(section, state, nProcessDirections, usePopupMessage, useSMS, useBroadcast, useExSMS, useExFax, false, DateTime.Now, showBoard, nCheckedNotify1, nCheckedNotify2);
            }

            public void AddSectionHistory(Sections.Section section, int nComponentHistoryID, Sections.State state, int nProcessDirections, bool noDBWrite, DateTime time, bool showBoard, int nCheckedNotify1, int nCheckedNotify2)
            {
                // 입력대기는 기록하지 않는다.
                //if (state == Sections.State.INPUT)
                //    return;

                Sections.Section.ComponentType type = section.GetComponentType();

                if (type == Sections.Section.ComponentType.DECISION)
                {
                    HistorySectionDecisionData data = AddDecisionHistory((Sections.SectionDecision)section, state, nProcessDirections, null, noDBWrite, time, showBoard);
                    if (data != null)
                        data.ComponentHistoryID = nComponentHistoryID;
                }
                else if (type == Sections.Section.ComponentType.INTERNAL)
                {
                    HistorySectionInternalData data = AddInternalHistory((Sections.SectionInternal)section, state, nProcessDirections, false, false, false, noDBWrite, time, showBoard, nCheckedNotify1);
                    if (data != null)
                        data.ComponentHistoryID = nComponentHistoryID;
                }
                else if (type == Sections.Section.ComponentType.EXTERNAL)
                {
                    HistorySectionExternalData data = AddExternalHistory((Sections.SectionExternal)section, state, nProcessDirections, false, false, noDBWrite, time, showBoard, nCheckedNotify1, nCheckedNotify2);
                    if (data != null)
                        data.ComponentHistoryID = nComponentHistoryID;
                }
                else if (type == Sections.Section.ComponentType.TRANSMISSION)
                {
                    HistorySectionTransmissionData data = AddTransmissionHistory((Sections.SectionTransmission)section, state, nProcessDirections, false, false, false, false, false, noDBWrite, time, showBoard, nCheckedNotify1, nCheckedNotify2);
                    if (data != null)
                        data.ComponentHistoryID = nComponentHistoryID;
                }
                else
                {
                    HistorySectionData data = new HistorySectionData(time, state, nProcessDirections, section);
                    data.NoDBWrite = noDBWrite;
                    data.ShowBoard = showBoard;
                    data.CheckNotify1 = nCheckedNotify1;
                    data.CheckNotify2 = nCheckedNotify2;
                    data.ComponentHistoryID = nComponentHistoryID;
                    m_arrSectionHistory.Add(data);
                }
            }

            public void AddSectionHistory(Sections.Section section, Sections.State state, int nProcessDirections, bool showBoard, int nCheckedNotify1, int nCheckedNotify2)
            {
                AddSectionHistory(section, -1, state, nProcessDirections, false, DateTime.Now, showBoard, nCheckedNotify1, nCheckedNotify2);
            }

            public void AddActionStepHistory(int nActionStepID, bool isRealMode, Sections.WorkFlowState state)
            {
               
                AddActionStepHistory(nActionStepID, isRealMode, state, DateTime.Now, false);
            }

            public void AddActionStepHistory(int nActionStepID, bool isRealMode, Sections.WorkFlowState state, DateTime time, bool noDBWrite)
            {
                ActionStepInfo actionStep = GetActionStep(nActionStepID);
                if (actionStep == null)
                    return;              

                HistoryActionStepData data = new HistoryActionStepData(time, state, actionStep, isRealMode);
                data.NoDBWrite = noDBWrite;
                if (state != Sections.WorkFlowState.DONE)
                {
                    SOPDisasterSystem.FormMain form = FormMain.Instance.FrmMain2;
                    if (form != null && form.LayoutForm != null)
                    {
                        data.Position = form.LayoutForm.LastPos;
                    }
                }                
                m_arrActionStepHistory.Add(data);
            }

            // History를 DB에 기록하는 부분은 Network을 통하여 이루어지므로 병목이 발생할 여지가 있다.
            // History 기록으로 인하여 Work Flow의 처리 속도가 영향을 받아선 안되므로, 
            // History 기록은 Thread를 사용하여 비동기로 진행한다.
            private void HistoryThread()
            {
                while (!bExit)
                {
                    try
                    {
                        // 방금 종료된 ActionStep 데이터
                        int nEndActionStepID = -1, nEndActionStepHistoryID = -1;

                        m_instance.WriteActionStepHistory(ref nEndActionStepID, ref nEndActionStepHistoryID);
                        m_instance.WriteSectionHistory(nEndActionStepID, nEndActionStepHistoryID);
                    }
                    catch (System.NullReferenceException e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }

                    Thread.Sleep(200);
                }

                closedThread = true;
            }

            protected bool WriteActionStepHistory(ref int nEndActionStepID, ref int nEndActionStepHistoryID)
            {
                FormMain frm = FormMain.Instance;
                WebDBManager dbMgr = frm.DBManager;

                int nDataCount = m_arrActionStepHistory.Count;

                for (int i=0;i<nDataCount;i++)
                {
                    HistoryActionStepData data = (HistoryActionStepData)m_arrActionStepHistory[0];
                    int nHistoryID = -1;
                    int nActionStepID = data.IsRealMode ? data.ActionStep.ActionStepID : -data.ActionStep.ActionStepID;

                    if (m_dicActionStepHistory.ContainsKey(nActionStepID))
                    {
                        nHistoryID = m_dicActionStepHistory[nActionStepID];
                        UpdateActionStepHistory(dbMgr, nHistoryID, data);
                    }
                    else
                    {
                        nHistoryID = WriteActionStepHistory(dbMgr, data, ref nEndActionStepID, ref nEndActionStepHistoryID);
                        if (nHistoryID > 0)
                        {
                            m_dicActionStepHistory[nActionStepID] = nHistoryID;
                        }
                    }

                    // 처리된 로그는 바로 제거한다.
                    m_arrActionStepHistory.RemoveAt(0);
                }

                return true;
            }

            private bool UpdateActionStepHistory(WebDBManager dbMgr, int nHistoryID, HistoryActionStepData data)
            {
                string strSQL = "";
                string strType = "-", strTask = "-";
                
                GetActionStepHistoryData(data, ref strType, ref strTask);

                DockingBottomSOPLog sopLog = FormMain.Instance.GetPageHome().GetDockSOPLog();
                bool result = false;

                if (data.NoDBWrite)
                    result = true;

                if (data.State == Sections.WorkFlowState.STANDBY)       // 대기
                {
                    return true;
                }
                else if (data.State == Sections.WorkFlowState.RUN)      // 실행중
                {
                    ArrayList arrProcessIDList = GetProcessIDList(data.ActionStep.ActionStepID);
                    if (arrProcessIDList == null)
                        return true;

                    sopLog.MakeActionStepLog(data.ActionStep.ActionStepID, data.IsRealMode, nHistoryID, data.Time, arrProcessIDList);
                    return true;
                }
                else if (data.State == Sections.WorkFlowState.PAUSE)    // 일시정지
                {
                    if (!data.NoDBWrite)
                    {
                        strSQL = string.Format("update ActionStepHistory set EndTime = NULL, CancelTime = NULL, PausedTime = '{0} {1:00}:{2:00}:{3:00}', LastAccessedUserID = {4} where id = {5}",
                            data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second, FormMain.Instance.SOPGenUserID, nHistoryID);

                        result = dbMgr.GetResultData(strSQL, 0) != null;
                    }
                }
                else if (data.State == Sections.WorkFlowState.STOP)     // 실행취소
                {
                    strSQL = string.Format("update ActionStepHistory set EndTime = NULL, CancelTime = '{0} {1:00}:{2:00}:{3:00}', PausedTime = NULL, LastAccessedUserID = {4} where id = {5}",
                        data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second, FormMain.Instance.SOPGenUserID, nHistoryID);

                    sopLog.CancelActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode, data.Time);

                    sopLog.AddLogData(null, data.NoDBWrite, nHistoryID, -1, data.ActionStep.ActionStepID, data.IsRealMode, -1, Sections.Section.ComponentType.NONE, data.Time, "-", "-",
                        strType, strTask, "실행취소", -1, true, true);

                    if (!data.NoDBWrite)
                    {
                        result = dbMgr.GetResultData(strSQL, 0) != null;
                    }

                    if (result)
                    {
                        int nActionStepID = data.IsRealMode ? data.ActionStep.ActionStepID : -data.ActionStep.ActionStepID;
                        m_dicActionStepHistory.Remove(nActionStepID);
                    }
                }
                else if (data.State == Sections.WorkFlowState.DONE)     // 완료
                {
                    strSQL = string.Format("update ActionStepHistory set EndTime = '{0} {1:00}:{2:00}:{3:00}', CancelTime = NULL, PausedTime = NULL, LastAccessedUserID = {4} where id = {5}",
                        data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second, FormMain.Instance.SOPGenUserID, nHistoryID);

                    sopLog.CompleteActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode, data.Time);

                    sopLog.AddLogData(null, data.NoDBWrite, nHistoryID, -1, data.ActionStep.ActionStepID, data.IsRealMode, -1, Sections.Section.ComponentType.NONE, data.Time, "-", "-",
                        strType, strTask, "완료", -1, true, true);

                    if (!data.NoDBWrite)
                    {
                        result = dbMgr.GetResultData(strSQL, 0) != null;
                    }

                    if (result)
                    {
                        int nActionStepID = data.IsRealMode ? data.ActionStep.ActionStepID : -data.ActionStep.ActionStepID;
                        m_dicActionStepHistory.Remove(nActionStepID);
                    }
                }
                else
                    return false;

                return result;
                //return dbMgr.GetResultData(strSQL, 0) != null;
            }

            private void GetProcessIDList(Sections.PanelSectionEx panel, ArrayList arrProcessIDList)
            {
                long nHighWord = (long)Sections.Section.ComponentType.PROCESS << 32;

                foreach (Sections.Section section in panel.Sections)
                {
                    if (section.GetComponentType() != Sections.Section.ComponentType.PROCESS)
                        continue;

                    int nComponentID = panel.GetComponentID(section);
                    if (nComponentID < 0)
                        continue;

                    long nID = nHighWord | (long)nComponentID;
                    arrProcessIDList.Add(nID);
                }
            }

            private void GetProcessIDList(TabPage tabPage, ArrayList arrProcessIDList)
            {
                Type type = typeof(Sections.PanelSectionEx);

                foreach (Control ctrl in tabPage.Controls)
                {
                    if (ctrl.GetType() == type)
                    {
                        Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl;
                        GetProcessIDList(panel, arrProcessIDList);
                    }
                }
            }

            private ArrayList GetProcessIDList(int nActionStepID)
            {
                ArrayList arrTabPages = FormMain.Instance.GetPageHome().GetTabPage();

                foreach (Sections.SectionTabPage tabPage in arrTabPages)
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
                TreeNode actionStepNode = FormMain.Instance.GetPageHome().GetDockScenario().GetBarLevelTree().FindActionStepNode(data.ActionStep.ActionStepID);

                if (actionStepNode != null)
                {
                    string strFullPath = GetActionStepPath(actionStepNode);

                    VersionInfo version = FormMain.Instance.SOPManager.GetActionStepVersionInfo(data.ActionStep.ActionStepID);
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

            private int WriteActionStepHistory(WebDBManager dbMgr, HistoryActionStepData data, ref int nEndActionStepID, ref int nEndActionStepHistoryID)
            {
                if (data.State == Sections.WorkFlowState.RUN)
                {
                    string strSQL = "select Max(id) from ActionStepHistory";

                    ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
                    if (arrResult == null)
                        return -1;

                    int nID = arrResult.Count == 0 ? 0 : WebDBManager.GetIntField(arrResult[0].ToString(), 0);
                    string strBeginTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second);

                    Sections.WorkFlow workFlow = Sections.WorkFlowManager.Instance.Get(data.ActionStep.ActionStepID, data.IsRealMode);
                    string strDetectTime = "NULL";
                    
                    if (workFlow != null)
                        strDetectTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", workFlow.DetectTime.ToShortDateString(), workFlow.DetectTime.Hour, workFlow.DetectTime.Minute, workFlow.DetectTime.Second);

                    string strPosition = "NULL";

                    if (workFlow != null && workFlow.HasPosition)
                    {
                        strPosition = "'" + workFlow.Position + "'";
                    }

                    
                    if (!data.NoDBWrite)
                    {
                        strSQL = string.Format("insert into ActionStepHistory (id, ActionStepID, RealMode, BeginTime, EndTime, CancelTime, PausedTime, DetectTime, Position, LastAccessedUserID, Description) values ({0}, {1}, {2}, {3}, NULL, NULL, NULL, {4}, {5}, {6}, NULL)",
                            ++nID, data.ActionStep.ActionStepID, data.IsRealMode ? 1 : 0, strBeginTime, strDetectTime, strPosition, FormMain.Instance.SOPGenUserID);

                        if (dbMgr.GetResultData(strSQL, 0) == null)
                            return -1;

                        FormMain.Instance.SOPManager.SetActionStepHistoryID(data.ActionStep.ActionStepID, data.IsRealMode, nID);

                        if (workFlow.LastPosition != null)
                        {
                            int nActionStepHistoryID = nID;
                            workFlow.LastPosition.HistoryActionStepID = nActionStepHistoryID;
                            strSQL = string.Format("insert into HistoryDisasterPos (PosX, PosY, PosZ, FloorIndex, HistoryActionSetpID, DisasterType, Description, BuildingID ) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}')",
                               workFlow.LastPosition.X, workFlow.LastPosition.Y, workFlow.LastPosition.Z, workFlow.LastPosition.FloorIndex, nActionStepHistoryID, workFlow.LastPosition.DisasterName, workFlow.LastPosition.PoistionName, workFlow.LastPosition.BuildingID);
                            if (dbMgr.GetResultData(strSQL, 0) == null)
                                return -1;
                        }
                    }
                  
                    ArrayList arrProcessIDList = GetProcessIDList(data.ActionStep.ActionStepID);
                    if (arrProcessIDList == null)
                        return -1;

                    string strType = "-", strTask = "-";
                    GetActionStepHistoryData(data, ref strType, ref strTask);

                    DockingBottomSOPLog sopLog = FormMain.Instance.GetPageHome().GetDockSOPLog();

                    sopLog.MakeActionStepLog(data.ActionStep.ActionStepID, data.IsRealMode, nID, data.Time, arrProcessIDList);
                    sopLog.AddLogData(null, data.NoDBWrite, nID, -1, data.ActionStep.ActionStepID, data.IsRealMode, -1, Sections.Section.ComponentType.NONE, data.Time, "-", "-",
                        strType, strTask, "시작", -1, true, true);

                    return nID;
                }
                else if (data.State == Sections.WorkFlowState.STOP)
                {
                    DockingBottomSOPLog sopLog = FormMain.Instance.GetPageHome().GetDockSOPLog();
                    ActionStepDetailLog detailLog = sopLog.GetActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode);

                    if (detailLog == null)
                        return -1;

                    // strSQL = string.Format("update ActionStepHistory set EndTime = NULL, CancelTime = NULL, PausedTime = '{0} {1:00}:{2:00}:{3:00}' where id = {4}",
                    //detailLog.HistoryID;

                    if (!data.NoDBWrite)
                    {
                        string strSQL = string.Format("update ActionStepHistory set EndTime = NULL, CancelTime = '{0} {1:00}:{2:00}:{3:00}', PausedTime = NULL, LastAccessedUserID = {4} where id = {5}",
                            data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second, FormMain.Instance.SOPGenUserID, detailLog.HistoryID);

                        if (dbMgr.GetResultData(strSQL, 0) == null)
                            return -1;
                    }

                    sopLog.CancelActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode, data.Time);

                    string strType = "-", strTask = "-";
                    GetActionStepHistoryData(data, ref strType, ref strTask);

                    sopLog.AddLogData(null, data.NoDBWrite, detailLog.HistoryID, -1, data.ActionStep.ActionStepID, data.IsRealMode, -1, Sections.Section.ComponentType.NONE, data.Time, "-", "-",
                        strType, strTask, "실행취소", -1, true, true);
                }
                else if (data.State == Sections.WorkFlowState.DONE)
                {
                    DockingBottomSOPLog sopLog = FormMain.Instance.GetPageHome().GetDockSOPLog();
                    ActionStepDetailLog detailLog = sopLog.GetActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode);

                    if (detailLog == null)
                        return -1;

                    if (!data.NoDBWrite)
                    {
                        string strSQL = string.Format("update ActionStepHistory set EndTime = '{0} {1:00}:{2:00}:{3:00}', CancelTime = NULL, PausedTime = NULL, LastAccessedUserID = {4} where id = {5}",
                            data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second, FormMain.Instance.SOPGenUserID, detailLog.HistoryID);

                        if (dbMgr.GetResultData(strSQL, 0) == null)
                            return -1;
                    }

                    sopLog.CompleteActionStepDetailLog(data.ActionStep.ActionStepID, data.IsRealMode, data.Time);

                    string strType = "-", strTask = "-";
                    GetActionStepHistoryData(data, ref strType, ref strTask);

                    sopLog.AddLogData(null, data.NoDBWrite, detailLog.HistoryID, -1, data.ActionStep.ActionStepID, data.IsRealMode, -1, Sections.Section.ComponentType.NONE, data.Time, "-", "-",
                        strType, strTask, "완료", -1, true, true);

                    nEndActionStepID = data.ActionStep.ActionStepID;
                    nEndActionStepHistoryID = detailLog.HistoryID;
                }

                return -1;
            }

            // nEndActionStepID, nEndActionStepHistoryID : 방금 종료된 ActionStep
            protected bool WriteSectionHistory(int nEndActionStepID, int nEndActionStepHistoryID)
            {
                FormMain frm = FormMain.Instance;
                WebDBManager dbMgr = frm.DBManager;

                int nMaxComponentHistoryID = GetMaxSectionHistoryID(dbMgr);
                int nDataCount = m_arrSectionHistory.Count;
                int nIndex = 0;

                for (int i = 0; i < nDataCount; i++)
                {
                    HistorySectionData data = (HistorySectionData)m_arrSectionHistory[nIndex];
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();
                    Sections.SectionTabPage tabPage = (Sections.SectionTabPage)panel.Parent;

                    int nActionStepID = tabPage.VirtualMode ? -panel.ActionStepID : panel.ActionStepID;

                    if (m_dicActionStepHistory.ContainsKey(nActionStepID))
                    {
                        int nHistoryID = m_dicActionStepHistory[nActionStepID];

                        if (nHistoryID < 0)
                        {
                            // Thread의 처리 순서 때문에 ActionStep보다 먼저 처리하려고 하는 Section
                            // 일단 미뤘다가 다시 실행
                            nIndex++;
                            continue;
                        }

                        WriteSectionHistory(dbMgr, nHistoryID, data, ref nMaxComponentHistoryID);
                    }
                    else
                    {
                        if (nEndActionStepHistoryID > 0)
                        {
                            WriteSectionHistory(dbMgr, nEndActionStepHistoryID, data, ref nMaxComponentHistoryID);
                        }
                        else
                        {
                            // Thread의 처리 순서 때문에 ActionStep보다 먼저 처리하려고 하는 Section
                            // 일단 미뤘다가 다시 실행
                            nIndex++;
                            continue;
                        }
                    }

                    // 처리된 로그는 바로 제거한다.
                    m_arrSectionHistory.RemoveAt(nIndex);
                }
        
                return true;
            }

            private int GetMaxSectionHistoryID(WebDBManager dbMgr)
            {
                string strSQL = "select max(id) from ComponentHistory";

                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return -1;

                if (arrResult.Count == 0)
                    return 0;

                return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            }

            // nHistoryID : ActionStep의 HistoryID
            private bool WriteSectionHistory(WebDBManager dbMgr, int nHistoryID, HistorySectionData data, ref int nMaxComponentHistoryID)
            {
                Sections.Section.ComponentType type = data.Section.GetComponentType();

                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();
                int nComponentID = panel.GetComponentID(data.Section);

                if (nComponentID < 0)
                    return false;

                Sections.SectionState sectionState = Sections.WorkFlowManager.Instance.Find(data.Section, !((Sections.SectionTabPage)(panel.Parent)).VirtualMode);
                if (sectionState != null)
                {
                    sectionState.CheckNotify1 = data.CheckNotify1;
                    sectionState.CheckNotify2 = data.CheckNotify2;
                }

                if (type == Sections.Section.ComponentType.ENDPOINT)
                    return WriteSectionEndPointHistory(dbMgr, nHistoryID, nComponentID, data, ref nMaxComponentHistoryID);
                else if (type == Sections.Section.ComponentType.PROCESS)
                    return WriteSectionProcessHistory(dbMgr, nHistoryID, nComponentID, data, ref nMaxComponentHistoryID);
                else if (type == Sections.Section.ComponentType.DECISION)
                    return WriteSectionDecisionHistory(dbMgr, nHistoryID, nComponentID, (HistorySectionDecisionData)data, ref nMaxComponentHistoryID);
                else if (type == Sections.Section.ComponentType.INTERNAL)
                    return WriteSectionInternalHistory(dbMgr, nHistoryID, nComponentID, (HistorySectionInternalData)data, ref nMaxComponentHistoryID);
                else if (type == Sections.Section.ComponentType.EXTERNAL)
                    return WriteSectionExternalHistory(dbMgr, nHistoryID, nComponentID, (HistorySectionExternalData)data, ref nMaxComponentHistoryID);
                else if (type == Sections.Section.ComponentType.TRANSMISSION)
                    return WriteSectionTransmissionHistory(dbMgr, nHistoryID, nComponentID, (HistorySectionTransmissionData)data, ref nMaxComponentHistoryID);
                else if (type == Sections.Section.ComponentType.TRANSSOP)
                    return WriteSectionTransSOPHistory(dbMgr, nHistoryID, nComponentID, data, ref nMaxComponentHistoryID);
                else if (type == Sections.Section.ComponentType.LINK)
                    return WriteSectionLinkHistory(dbMgr, nHistoryID, nComponentID, data, ref nMaxComponentHistoryID);

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

                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
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

            private bool WriteSectionEndPointHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionData data, ref int nMaxComponentHistoryID)
            {
                bool justDBInput = false;
                int nStatus = 3;

                if (data.State == Sections.State.INPUT)
                {
                    justDBInput = true;
                    nStatus = 4;
                }
                else if (data.State != Sections.State.DONE)     // 실행 완료
                    return false;

                int nDirection = GetHiWordProcessDirection(data);
                nStatus |= nDirection;

                BarLevelTree tree = FormMain.Instance.GetPageHome().GetDockScenario().GetBarLevelTree();
                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();

                TreeNode node = tree.FindActionStepNode(panel.ActionStepID);
                if (node == null)
                    return false;

                string strPath = GetActionStepPath(node);

                int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
                if (nComponentID < 0) return false;
                
                Sections.SectionDataEndPoint sectionData = (Sections.SectionDataEndPoint)data.Section.Data;
                string strTask = sectionData.IsBegin ? strPath + " 시작" : strPath + " 완료";

                if (!sectionData.IsBegin)
                    nCompleteCount++;
                
                string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second);
                string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, CompleteCount, ShowBoard, AccessedUserID, CheckedNotify1, CheckedNotify2, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, '{6}', {7}, {8}, {9}, 0, 0, NULL)",
                    ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, nCompleteCount, data.ShowBoard ? 1 : 0, FormMain.Instance.SOPGenUserID);

			
                if (!data.NoDBWrite)
                {
                    if (dbMgr.GetResultData(strSQL, 0) == null)
                        return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;
                }
                else
                    nMaxComponentHistoryID--;

                // ActionStep별 마지막으로 기록된 ComponentHistoryID 저장
                m_dicLastComponentHistory[panel.ActionStepID] = nMaxComponentHistoryID;

                if (justDBInput)
                {
                    // DB에 입력만 하고 리턴시킨다.
                    return true;
                }

                Sections.SectionTabPage tabPage = (Sections.SectionTabPage)panel.Parent;
                DockingBottomSOPLog sopLog = FormMain.Instance.GetPageHome().GetDockSOPLog();

                if (!sectionData.IsBegin && data.State == Sections.State.DONE)
                {
                    ActionStepDetailLog actionStepLog = sopLog.GetActionStepDetailLog(panel.ActionStepID, !tabPage.VirtualMode);
                    if (actionStepLog == null || actionStepLog.BeginTime == null)
                        return false;

                    m_allActionStepHistory.AddHistory(panel.ActionStepID, !tabPage.VirtualMode, nHistoryID, actionStepLog.BeginTime.m_time, data.Time);
                }

                //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, "-", "-", sectionData.IsBegin ? "시작" : "끝", data.Section.Title, strTask, nCompleteCount, true);
                FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, "-", "-", sectionData.IsBegin ? "시작" : "끝", data.Section.Title, strTask, nCompleteCount, true);
                
                return true;
            }

            private bool WriteSectionProcessHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionData data, ref int nMaxComponentHistoryID)
            {
                int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
                if (nComponentID < 0) return false;

                int nStatus = 0;
                string strStatus = "";
                bool justDBInput = false;

                if (data.State == Sections.State.NORMAL)        // 대기
                {
                    nStatus = 1;
                    strStatus = "대기";
                }
                else if (data.State == Sections.State.INPUT)    // 입력 대기
                {
                    nStatus = 4;
                    strStatus = "입력대기";
                    justDBInput = true;
                    // 입력 대기는 로그를 기록하지 않는다.
                    //return true;
                }
                else if (data.State == Sections.State.RUN)      // 실행중
                {
                    nStatus = 2;
                    strStatus = "실행중";
                }
                else if (data.State == Sections.State.SKIP)     // 건너뛰기
                {
                    nStatus = 5;
                    strStatus = "건너뛰기";
                }
                else if (data.State == Sections.State.DONE)     // 실행 완료
                {
                    nStatus = 3;
                    strStatus = "실행 완료";
                    nCompleteCount++;
                }
                else
                    return true;

                int nDirection = GetHiWordProcessDirection(data);
                nStatus |= nDirection;

                string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second);

                string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, NULL)",
                    ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, "NULL", data.ShowBoard ? 1 : 0, nCompleteCount, FormMain.Instance.SOPGenUserID, data.CheckNotify1, data.CheckNotify2);

                if (!data.NoDBWrite)
                {
                    if (dbMgr.GetResultData(strSQL, 0) == null)
                        return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;
                }
                else
                    nMaxComponentHistoryID--;

                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();

                // ActionStep별 마지막으로 기록된 ComponentHistoryID 저장
                m_dicLastComponentHistory[panel.ActionStepID] = nMaxComponentHistoryID;

                if (justDBInput)
                {
                    // DB에 입력만 하고 리턴시킨다.
                    return true;
                }

                //Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();
                Sections.SectionDataProcess sectionData = (Sections.SectionDataProcess)data.Section.Data;
                Sections.SectionProcess section = (Sections.SectionProcess)data.Section;

                string strTeamNameList = GetTeamNameList(sectionData.TeamList);
                Sections.SectionTabPage tabPage = (Sections.SectionTabPage)panel.Parent;

                if (tabPage == null)
                    return false;

                FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), strTeamNameList, "프로세스", 
                    section.TextUP, strStatus, nCompleteCount, true);
                return true;
            }

            private string GetTeamNameList(ArrayList arrSOPTeams)
            {
                string strTeamNameList = "";

                foreach (Sections.SOPTeam team in arrSOPTeams)
                {
                    if (strTeamNameList.Length == 0)
                        strTeamNameList = team.TeamName;
                    else
                        strTeamNameList += ", " + team.TeamName;
                }

                return strTeamNameList;
            }

            private bool WriteSectionDecisionHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionDecisionData data, ref int nMaxComponentHistoryID)
            {
                int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
                if (nComponentID < 0) return false;

                int nStatus = 0;
                bool justDBInput = false;

                if (data.State == Sections.State.NORMAL)        // 대기
                    nStatus = 1;
                else if (data.State == Sections.State.INPUT)    // 입력 대기
                {
                    nStatus = 4;
                    justDBInput = true;
                    // 입력 대기는 로그를 기록하지 않는다.
                    //return true;
                }
                else if (data.State == Sections.State.RUN)      // 실행중
                    nStatus = 2;
                else if (data.State == Sections.State.SKIP)     // 건너뛰기
                    nStatus = 5;
                else if (data.State == Sections.State.DONE)     // 실행 완료
                {
                    nStatus = 3;
                    nCompleteCount++;
                }
                else
                    return true;

                int nDirection = GetHiWordProcessDirection(data);
                nStatus |= nDirection;

                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();
                string strTask = "NULL", strDescription = "NULL";

                if (data.NextSection != null && data.State != Sections.State.NORMAL)
                {
                    string strArrow = "";
                    foreach (Sections.Arrow arrow in data.Section.Arrows)
                    {
                        Sections.Section EndSection = arrow.EndLink;

                        if (data.NextSection.Data.ID == EndSection.Data.ID)
                        {
                            strArrow = arrow.Text;
                            break;
                        }
                    }

                    int nNextComponentID = panel.GetComponentID(data.NextSection);

                    if (data.NextSection.GetComponentType() == Sections.Section.ComponentType.PROCESS)
                    {
                        if (strArrow.Length > 0)
                            strTask = string.Format("'({0}) 으로 분기'", strArrow);
                        else
                        {
                            Sections.SectionProcess section = (Sections.SectionProcess)data.NextSection;

                            if (section.TextUP.Length > 0)
                                strTask = string.Format("'({0}) 으로 분기'", section.TextUP);
                            else
                                strTask = string.Format("'{0} 으로 분기'", section.Data.ComponentID);
                        }
                    }
                    else
                    {
                        if (strArrow.Length > 0)
                            strTask = string.Format("'({0}) 으로 분기'", strArrow);
                        else
                        {
                            Sections.SectionProcess section = (Sections.SectionProcess)data.NextSection;

                            if (section.Title.Length > 0)
                                strTask = string.Format("'({0}) 으로 분기'", section.Title);
                            else
                                strTask = string.Format("'{0} 으로 분기'", section.Data.ComponentID);
                        }
                    }

                    strDescription = "'" + data.NextSection.Data.ComponentID + "'";
                }

                string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second);

                string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, 0, 0, {10})",
                    ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, nCompleteCount, FormMain.Instance.SOPGenUserID, strDescription);

                if (!data.NoDBWrite)
                {
                    if (dbMgr.GetResultData(strSQL, 0) == null)
                        return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;
                }
                else
                    nMaxComponentHistoryID--;

                // ActionStep별 마지막으로 기록된 ComponentHistoryID 저장
                m_dicLastComponentHistory[panel.ActionStepID] = nMaxComponentHistoryID;

                if (justDBInput)
                {
                    // DB에 입력만 하고 리턴시킨다.
                    return true;
                }

                Sections.SectionTabPage tabPage = (Sections.SectionTabPage)panel.Parent;

                //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "분기", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);
                FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "분기", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);

                return true;
            }

            private bool WriteSectionInternalHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionInternalData data, ref int nMaxComponentHistoryID)
            {
                int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
                if (nComponentID < 0) return false;

                int nStatus = 0;
                bool justDBInput = false;

                if (data.State == Sections.State.NORMAL)        // 대기
                    nStatus = 1;
                else if (data.State == Sections.State.INPUT)    // 입력 대기
                {
                    nStatus = 4;
                    justDBInput = true;
                    // 입력 대기는 로그를 기록하지 않는다.
                    //return true;
                }
                else if (data.State == Sections.State.RUN)      // 실행중
                    nStatus = 2;
                else if (data.State == Sections.State.SKIP)     // 건너뛰기
                    nStatus = 5;
                else if (data.State == Sections.State.DONE)     // 실행 완료
                {
                    nStatus = 3;
                    nCompleteCount++;
                }
                else
                    return true;

                int nDirection = GetHiWordProcessDirection(data);
                nStatus |= nDirection;

                string strTask = "";

                if (data.State == Sections.State.RUN || data.State == Sections.State.DONE)
                {
                    if (data.UsePopupMessage)
                    {
                        if (strTask.Length == 0)
                            strTask = "PC Popup Message 발송";
                        else
                            strTask += ", PC Popup Message 발송";
                    }

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

                string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second);

                string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, 0, NULL)",
                    ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, nCompleteCount, FormMain.Instance.SOPGenUserID, data.CheckNotify1);

                if (!data.NoDBWrite)
                {
                    if (dbMgr.GetResultData(strSQL, 0) == null)
                        return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;
                }
                else
                    nMaxComponentHistoryID--;

                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();

                // ActionStep별 마지막으로 기록된 ComponentHistoryID 저장
                m_dicLastComponentHistory[panel.ActionStepID] = nMaxComponentHistoryID;

                if (justDBInput)
                {
                    // DB에 입력만 하고 리턴시킨다.
                    return true;
                }

                //Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();
                Sections.SectionTabPage tabPage = (Sections.SectionTabPage)panel.Parent;

                //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "내부 상황전파", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);
                FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "내부 상황전파", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);

                return true;
            }

            private string GetExternalTeamList(ArrayList arrTeams)
            {
                string strTeamList = "";

                foreach (Sections.ExternalTeamData team in arrTeams)
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

            private bool WriteSectionExternalHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionExternalData data, ref int nMaxComponentHistoryID)
            {
                int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
                if (nComponentID < 0) return false;

                int nStatus = 0;
                bool justDBInput = false;

                if (data.State == Sections.State.NORMAL)        // 대기
                    nStatus = 1;
                else if (data.State == Sections.State.INPUT)    // 입력 대기
                {
                    nStatus = 4;
                    justDBInput = true;
                    // 입력 대기는 로그를 기록하지 않는다.
                    //return true;
                }
                else if (data.State == Sections.State.RUN)      // 실행중
                    nStatus = 2;
                else if (data.State == Sections.State.SKIP)     // 건너뛰기
                    nStatus = 5;
                else if (data.State == Sections.State.DONE)     // 실행 완료
                {
                    nStatus = 3;
                    nCompleteCount++;
                }
                else
                    return true;

                int nDirection = GetHiWordProcessDirection(data);
                nStatus |= nDirection;

                string strTask = "";

                if (data.State == Sections.State.RUN || data.State == Sections.State.DONE)
                {
                    Sections.SectionDataExternal sectionData = (Sections.SectionDataExternal)data.Section.Data;
                    
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

                string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second);

                string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, NULL)",
                    ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, nCompleteCount, FormMain.Instance.SOPGenUserID, data.CheckNotify1, data.CheckNotify2);

                if (!data.NoDBWrite)
                {
                    if (dbMgr.GetResultData(strSQL, 0) == null)
                        return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;
                }
                else
                    nMaxComponentHistoryID--;

                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();

                // ActionStep별 마지막으로 기록된 ComponentHistoryID 저장
                m_dicLastComponentHistory[panel.ActionStepID] = nMaxComponentHistoryID;

                if (justDBInput)
                {
                    // DB에 입력만 하고 리턴시킨다.
                    return true;
                }

                //Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();
                Sections.SectionTabPage tabPage = (Sections.SectionTabPage)panel.Parent;

                //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "외부 상황전파", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);
                FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "외부 상황전파", data.Section.Title, strTask == "NULL" ? "-" : strTask, nCompleteCount, true);

                return true;
            }

            private bool WriteSectionTransmissionHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionTransmissionData data, ref int nMaxComponentHistoryID)
            {
                int nCompleteCount = GetMaxComponentCompleteCount(dbMgr, nHistoryID, nComponentID);
                if (nComponentID < 0) return false;

                int nStatus = 0;
                string strStatus = "";
                bool justDBInput = false;

                if (data.State == Sections.State.NORMAL)        // 대기
                {
                    nStatus = 1;
                    strStatus = "대기";
                }
                else if (data.State == Sections.State.INPUT)    // 입력 대기
                {
                    nStatus = 4;
                    strStatus = "입력대기";
                    justDBInput = true;
                    // 입력 대기는 로그를 기록하지 않는다.
                    //return true;
                }
                else if (data.State == Sections.State.RUN)      // 실행중
                {
                    nStatus = 2;
                    strStatus = "실행중";
                }
                else if (data.State == Sections.State.SKIP)     // 건너뛰기
                {
                    nStatus = 5;
                    strStatus = "건너뛰기";
                }
                else if (data.State == Sections.State.DONE)     // 실행 완료
                {
                    nStatus = 3;
                    strStatus = "실행 완료";
                    nCompleteCount++;
                }
                else
                    return true;

                int nDirection = GetHiWordProcessDirection(data);
                nStatus |= nDirection;

                string strTask = "";

                if (data.State == Sections.State.RUN || data.State == Sections.State.DONE)
                {

                    if (data.UsePopupMessage)
                    {
                        if (strTask.Length == 0)
                            strTask = "PC Popup Message 발송";
                        else
                            strTask += ", PC Popup Message 발송";
                    }

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

                    Sections.SectionDataTransmission sectionData = (Sections.SectionDataTransmission)data.Section.Data;
                    Sections.SectionDataTransmission.ExternalData ExternalData = (Sections.SectionDataTransmission.ExternalData)sectionData.DataExternal;

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

                string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second);

                string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, NULL)",
                    ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, nCompleteCount, FormMain.Instance.SOPGenUserID, data.CheckNotify1, data.CheckNotify2);

                if (!data.NoDBWrite)
                {
                    if (dbMgr.GetResultData(strSQL, 0) == null)
                        return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;
                }
                else
                    nMaxComponentHistoryID--;

                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();

                // ActionStep별 마지막으로 기록된 ComponentHistoryID 저장
                m_dicLastComponentHistory[panel.ActionStepID] = nMaxComponentHistoryID;

                if (justDBInput)
                {
                    // DB에 입력만 하고 리턴시킨다.
                    return true;
                }

                //Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();
                Sections.SectionTabPage tabPage = (Sections.SectionTabPage)panel.Parent;

                //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "상황전파", data.Section.Title, strStatus/*strTask == "NULL" ? "-" : strTask*/, nCompleteCount, true);
                FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "상황전파", data.Section.Title, strStatus/*strTask == "NULL" ? "-" : strTask*/, nCompleteCount, true);

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
                Sections.SectionDataTransSOP sectionData = (Sections.SectionDataTransSOP)data.Section.Data;
                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();

                string strFormat = "select dc.CategoryName, sc.SubCategoryName, d.DisasterName, a.StepName, v.isRegular, v.isNormal ";
                strFormat += "from DisasterCategory as dc, SubDisasterCategory as sc, Disaster as d, ActionStep as a, Version as v ";
                strFormat += "where a.ID in ({0}, {1}) and a.DisasterID = d.ID and d.VersionID = v.ID and d.SubDisasterID = sc.ID and sc.DisasterID = dc.ID";

                string strSQL = string.Format(strFormat, panel.ActionStepID, sectionData.LinkedActionStepID);
                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

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

            private bool WriteSectionTransSOPHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionData data, ref int nMaxComponentHistoryID)
            {
                int nStatus = 0;
                bool justDBInput = false;

                if (data.State == Sections.State.NORMAL)        // 대기
                    nStatus = 1;
                else if (data.State == Sections.State.INPUT)    // 입력 대기
                {
                    nStatus = 4;
                    justDBInput = true;
                    // 입력 대기는 로그를 기록하지 않는다.
                    //return true;
                }
                else if (data.State == Sections.State.RUN)      // 실행중
                    nStatus = 2;
                else if (data.State == Sections.State.SKIP)     // 건너뛰기
                    nStatus = 5;
                else if (data.State == Sections.State.DONE)     // 실행 완료
                    nStatus = 3;
                else
                    return true;

                int nDirection = GetHiWordProcessDirection(data);
                nStatus |= nDirection;

                string strTask = "NULL";

                if (data.State == Sections.State.RUN || data.State == Sections.State.DONE)
                {
                    strTask = GetTransSOPTask(dbMgr, data);
                }

                string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second);

                string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, NULL, {8}, 0, 0, NULL)",
                    ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, FormMain.Instance.SOPGenUserID);

                if (!data.NoDBWrite)
                {
                    if (dbMgr.GetResultData(strSQL, 0) == null)
                        return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;
                }
                else
                    nMaxComponentHistoryID--;

                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();

                // ActionStep별 마지막으로 기록된 ComponentHistoryID 저장
                m_dicLastComponentHistory[panel.ActionStepID] = nMaxComponentHistoryID;

                if (justDBInput)
                {
                    // DB에 입력만 하고 리턴시킨다.
                    return true;
                }

                //Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();
                Sections.SectionTabPage tabPage = (Sections.SectionTabPage)panel.Parent;

                //FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, nMaxComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "SOP 전환", data.Section.Title, strTask == "NULL" ? "-" : strTask, -1, true);
                FormMain.Instance.GetPageHome().GetDockSOPLog().AddLog(nHistoryID, data.ComponentHistoryID, data, panel.ActionStepID, !tabPage.VirtualMode, nComponentID, panel.GetTitle(), "-", "SOP 전환", data.Section.Title, strTask == "NULL" ? "-" : strTask, -1, true);

                return true;
            }

            private bool WriteSectionLinkHistory(WebDBManager dbMgr, int nHistoryID, int nComponentID, HistorySectionData data, ref int nMaxComponentHistoryID)
            {
                int nStatus = 0;
                
                if (data.State == Sections.State.NORMAL)        // 대기
                    nStatus = 1;
                else if (data.State == Sections.State.INPUT)    // 입력 대기
                {
                    nStatus = 4;
                    // 입력 대기는 로그를 기록하지 않는다.
                    //return true;
                }
                else if (data.State == Sections.State.RUN)      // 실행중
                    nStatus = 2;
                else if (data.State == Sections.State.SKIP)     // 건너뛰기
                    nStatus = 5;
                else if (data.State == Sections.State.DONE)     // 실행 완료
                    nStatus = 3;
                else
                    return true;

                int nDirection = GetHiWordProcessDirection(data);
                nStatus |= nDirection;

                string strTask = "NULL";

                if (data.State == Sections.State.RUN || data.State == Sections.State.DONE)
                {
                    strTask = GetTransSOPTask(dbMgr, data);
                }

                string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", data.Time.ToShortDateString(), data.Time.Hour, data.Time.Minute, data.Time.Second);

                string strSQL = string.Format("insert into ComponentHistory (ID, ActionStepHistoryID, ComponentID, ComponentType, Time, Status, Task, ShowBoard, CompleteCount, AccessedUserID, CheckedNotify1, CheckedNotify2, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, NULL, {8}, 0, 0, NULL)",
                    ++nMaxComponentHistoryID, nHistoryID, nComponentID, (int)data.Section.GetComponentType(), strTime, nStatus, strTask, data.ShowBoard ? 1 : 0, FormMain.Instance.SOPGenUserID);

                if (!data.NoDBWrite)
                {
                    if (dbMgr.GetResultData(strSQL, 0) == null)
                        return false;

                    data.ComponentHistoryID = nMaxComponentHistoryID;
                }
                else
                    nMaxComponentHistoryID--;

                Sections.PanelSectionEx panel = (Sections.PanelSectionEx)data.Section.GetParent();

                // ActionStep별 마지막으로 기록된 ComponentHistoryID 저장
                m_dicLastComponentHistory[panel.ActionStepID] = nMaxComponentHistoryID;

                // Link는 DB에만 기록한다.
                return true;
            }

            protected ActionStepInfo GetActionStep(int nActionStepID)
            {
                FormMain frm = FormMain.Instance;
                BarLevelTree tree = frm.GetPageHome().GetDockScenario().GetBarLevelTree();

                TreeNode node = tree.FindActionStepNode(nActionStepID);
                if (node == null)
                    return null;

                while (node.Level > 2)
                {
                    node = node.Parent;
                }

                TreeNode nodeDisaster = node;
                
                SOPManager sopMgr = frm.SOPManager;

				string strFullPath = nodeDisaster.Parent.Parent.Text + szDeli + nodeDisaster.Parent.Text + szDeli + nodeDisaster.Text;
                Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(frm.IsRegular, frm.IsNormal);

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

            // DB에서 모든 ActionStep들의 실행 이력을 받아온다.
            public bool LoadActionStepHistory(WebDBManager dbMgr)
            {
                string strSQL = "Select ID, ActionStepID, RealMode, BeginTime, EndTime from ActionStepHistory where EndTime is not null order by ActionStepID";
                ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;
                DateTime dtDefault = new DateTime();

                for (int i = 0; i < nResultCount - 4; i += 5)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    bool isRealMode = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0) == 0 ? false : true;
                    DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                    DateTime dtEnd = WebDBManager.GetDateTimeField(arrResult[i + 4], dtDefault);

                    m_allActionStepHistory.AddHistory(nActionStepID, isRealMode, nID, dtBegin, dtEnd);
                }

                return true;
            }

            private void MakeActionStepHistorySubSQL(Dictionary<string, DisasterInfo> dicSOP, ArrayList arrRunningActionStep, bool isRealMode, Dictionary<int, DisasterInfo> dicDisaster, Dictionary<DisasterInfo, string> dicDisasterFullPath, ref string strSubSQL)
            {
                foreach (KeyValuePair<string, DisasterInfo> pair in dicSOP)
                {
                    DisasterInfo disaster = pair.Value;
                    dicDisasterFullPath[disaster] = pair.Key;

                    foreach (ActionStepInfo actionStep in disaster.ActionSteps)
                    {
                        dicDisaster[actionStep.ActionStepID] = disaster;

                        long nActionStepInfo = ((long)actionStep.ActionStepID) << 32;
                        if (isRealMode) nActionStepInfo |= (1 << 24);

                        if (arrRunningActionStep.Contains(nActionStepInfo))
                            continue;

                        string strSQL = string.Format("(select max(id) from ActionStepHistory where BeginTime = (select max(BeginTime) from ActionStepHistory where ActionStepID = {0} and RealMode = {1}))",
                            actionStep.ActionStepID, isRealMode ? 1 : 0);

                        if (strSubSQL.Length == 0)
                            strSubSQL = strSQL;
                        else
                            strSubSQL += ", " + strSQL;
                    }
                }
            }

            public HistoryDiasterPosition FindHistoryDisasterPosition(int nActionStepID, bool isRealMode)
            {
                long nActionStepInfo = ((long)nActionStepID) << 32;
                if (isRealMode) nActionStepInfo |= 1;

                if (m_dicHistoryDisasterPosition.ContainsKey(nActionStepInfo))
                    return m_dicHistoryDisasterPosition[nActionStepInfo];

                return null;
            }

            public void RemoveHistoryDisasterPosition(int nActionStepID, bool isRealMode)
            {
                long nActionStepInfo = ((long)nActionStepID) << 32;
                if (isRealMode) nActionStepInfo |= 1;

                m_dicHistoryDisasterPosition.Remove(nActionStepInfo);
            }

            public void AddHistoryDisasterPosition(int nActionStepHistoryID, int nActionStepID, bool isRealMode)
            {
                string strSQL = "select id, PosX, PosY, PosZ, FloorIndex, DisasterType, Description, BuildingID from HistoryDisasterPos where HistoryActionSetpID = " + nActionStepHistoryID.ToString();
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 7; i += 8)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    float x = WebDBManager.GetFloatField(arrResult[i + 1].ToString(), 0.0f);
                    float y = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), 0.0f);
                    float z = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), 0.0f);
                    int nFloorIndex = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -999);
                    string strDisasterType = WebDBManager.GetStringField(arrResult[i + 5], "");
                    string strDescription = WebDBManager.GetStringField(arrResult[i + 6], "");
                    string strBuildingID = WebDBManager.GetStringField(arrResult[i + 7], "");

                    SOPMonitoringSystem.HistoryDiasterPosition position = new SOPMonitoringSystem.HistoryDiasterPosition();

                    position.X = x;
                    position.Y = y;
                    position.Z = z;
                    position.FloorIndex = nFloorIndex;
                    position.HistoryActionStepID = nActionStepHistoryID;
                    position.DisasterName = strDisasterType;
                    position.PoistionName = strDescription;
                    position.BuildingID = strBuildingID;

                    long nActionStepInfo = ((long)nActionStepID) << 32;
                    if (isRealMode) nActionStepInfo |= 1;

                    m_dicHistoryDisasterPosition[nActionStepInfo] = position;
                }
            }

            private bool MonitorNewActionStepHistory(Dictionary<string, DisasterInfo> dicSOP, bool isRealMode, bool isRegular, bool isNormal, ArrayList arrRunningActionStep)
            {
                // ActionStep ID, Disaster
                Dictionary<int, DisasterInfo> dicDisaster = new Dictionary<int, DisasterInfo>();
                // Disaster, Disaster Full Path
                Dictionary<DisasterInfo, string> dicDisasterFullPath = new Dictionary<DisasterInfo, string>();

                string strSQL = "select id, ActionStepID, BeginTime, DetectTime from ActionStepHistory where EndTime is NULL and CancelTime is NULL and id in (";
                string strSubSQL = "";

                MakeActionStepHistorySubSQL(dicSOP, arrRunningActionStep, isRealMode, dicDisaster, dicDisasterFullPath, ref strSubSQL);

                if (strSubSQL.Length == 0)
                    return true;

                strSQL += strSubSQL + ")";

                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;

                if (nResultCount == 0)
                    return true;

                DateTime dtDefault = new DateTime();

                string strActionStepIDs = "";
                ArrayList arrHistoryID = new ArrayList();
                ArrayList arrActionStepID = new ArrayList();
                ArrayList arrBeginTime = new ArrayList();
                ArrayList arrDetectTime = new ArrayList();
                ArrayList arrDisaster = new ArrayList();

                for (int i = 0; i < nResultCount - 3; i += 4)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nActionStepID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    DateTime dtBegin = WebDBManager.GetDateTimeField(arrResult[i + 2], dtDefault);
                    DateTime dtDetect = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);

                    if (!dicDisaster.ContainsKey(nActionStepID))
                        continue;

                    DisasterInfo disaster = dicDisaster[nActionStepID];

                    if (!dicDisasterFullPath.ContainsKey(disaster))
                        continue;

                    if (strActionStepIDs.Length == 0)
                        strActionStepIDs = nActionStepID.ToString();
                    else
                        strActionStepIDs += ", " + nActionStepID.ToString();

                    arrHistoryID.Add(nID);
                    arrActionStepID.Add(nActionStepID);
                    arrBeginTime.Add(dtBegin);
                    arrDetectTime.Add(dtDetect);
                    arrDisaster.Add(disaster);

                    AddHistoryDisasterPosition(nID, nActionStepID, isRealMode);
                    History.HistoryManager.Instance.SetActionStepHistory(nActionStepID, nID);
                }

                FormMain frmMain = FormMain.Instance;
                PageBackstageHome pageHome = frmMain.GetPageHome();

                Sections.SectionTabPage delPage = null;
                Sections.SectionTabPage curPage = (Sections.SectionTabPage)pageHome.TabControls.SelectedTab;
                if (curPage != null)
                {
                    ArrayList arrTabPages = pageHome.GetTabPage();

                    foreach (Sections.SectionTabPage page in arrTabPages)
                    {
                        if (page.ActionStepID == curPage.ActionStepID && page.VirtualMode == curPage.VirtualMode)
                        {
                            if (curPage == page)
                                delPage = page;
                            TabPageManager.Instance.RemovePage(curPage.ActionStepID, !curPage.VirtualMode);
                            break;
                        }
                    }
                }                       
                

                bool isSuccess = pageHome.GetDockScenario().LoadActionStepPanel(frmMain.DBManager, strActionStepIDs, arrHistoryID, arrActionStepID, arrBeginTime, arrDetectTime, arrDisaster, isRealMode, dicDisaster, dicDisasterFullPath, isRegular, isNormal);

                if (isSuccess)
                {
                    int nCurrentActionStepID = frmMain.ReadCurrentActionStep(ref isRealMode);

                    if (nCurrentActionStepID >= 0)
                    {
                        BarLevelTree tree = pageHome.GetDockScenario().GetBarLevelTree();

                        tree.IgnoreLoadSOP = true;
                        pageHome.GetDockScenario().SelectedGridRow(nCurrentActionStepID, isRealMode);
                        tree.IgnoreLoadSOP = false;

                        if (delPage != null)
                            pageHome.RemoveTabPage(delPage);
                    }

                    int nActionStepCount = arrActionStepID.Count;

                    for (int i = 0; i < nActionStepCount; i++)
                    {
                        frmMain.SOPManager.SetActionStepHistoryID((int)arrActionStepID[i], isRealMode, (int)arrHistoryID[i]);
                    }
                }

                return isSuccess;
            }

            private void MonitorNewActionStepHistory(ArrayList arrRunningActionStep)
            {
                SOPManager sopMgr = FormMain.Instance.SOPManager;

                Dictionary<string, DisasterInfo> dicRegularNormal = sopMgr.GetSOPDictionary(true, true);
                Dictionary<string, DisasterInfo> dicRegularAbnormal = sopMgr.GetSOPDictionary(true, false);
                Dictionary<string, DisasterInfo> dicNonregularNormal = sopMgr.GetSOPDictionary(false, true);
                Dictionary<string, DisasterInfo> dicNonregularAbnormal = sopMgr.GetSOPDictionary(false, false);

                if (!MonitorNewActionStepHistory(dicRegularNormal, true, true, true, arrRunningActionStep))
                    return;
                if (!MonitorNewActionStepHistory(dicRegularNormal, false, true, true, arrRunningActionStep))
                    return;
                if (!MonitorNewActionStepHistory(dicRegularAbnormal, true, true, false, arrRunningActionStep))
                    return;
                if (!MonitorNewActionStepHistory(dicRegularAbnormal, false, true, false, arrRunningActionStep))
                    return;
                if (!MonitorNewActionStepHistory(dicNonregularNormal, true, false, true, arrRunningActionStep))
                    return;
                if (!MonitorNewActionStepHistory(dicNonregularNormal, false, false, true, arrRunningActionStep))
                    return;
                if (!MonitorNewActionStepHistory(dicNonregularAbnormal, true, false, false, arrRunningActionStep))
                    return;
                if (!MonitorNewActionStepHistory(dicNonregularAbnormal, false, false, false, arrRunningActionStep))
                    return;
            }

            private int m_nPrevActionStepID = -1;
            public void MonitorCurrentActionStep()
            {
                string strSQL = "select ActionStepID, RealMode from CurrentActionStep where id = 1";
                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count <= 1)
                    return;

                int nCurrentActionStepID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                bool isRealMode = WebDBManager.GetIntField(arrResult[1].ToString(), 0) == 0 ? false : true;

                int nActionStepID;
                bool isReal, isRegular, isNormal;
                string szResult = FormMain.Instance.GetPageHome().GetDockScenario().GetCurrentSOPInfo(out nActionStepID, out isReal, out isRegular, out isNormal);

                if (m_nPrevActionStepID == -1 && nCurrentActionStepID == nActionStepID)
                {
                    //FormMain.Instance.GetPageHome().GetDockScenario().SelectedGridRow(nCurrentActionStepID, isRealMode);
                }

                if (nCurrentActionStepID > 0 && (nCurrentActionStepID != nActionStepID || isRealMode != isReal))
                {
                    FormMain.Instance.GetPageHome().GetDockScenario().SelectedGridRow(nCurrentActionStepID, isRealMode);
                }
                m_nPrevActionStepID = nCurrentActionStepID;
            }

            private void MonitorActionStepHistory(int nActionStepID, bool isRealMode, bool isRegular, bool isNormal, string strActionStepFullPath)
            {
                string strFormat = "select id, EndTime, CancelTime from ActionStepHistory where (EndTime is not NULL or CancelTime is not NULL) ";
                strFormat += "and id = (select max(id) from ActionStepHistory where ActionStepID = {0} and RealMode = {1})";

                string strSQL = string.Format(strFormat, nActionStepID, isRealMode ? 1 : 0);

                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);
                if (arrResult == null) return;

                if (arrResult.Count == 0)
                    return;

                string strEndTime = WebDBManager.GetStringField(arrResult[1], "");
                string strCancelTime = WebDBManager.GetStringField(arrResult[2], "");

                if (string.Compare(strEndTime, "null", true) != 0)
                {
                    try
                    {
                        DateTime dtEnd = Convert.ToDateTime(strEndTime);

                        Sections.WorkFlow work = Sections.WorkFlowManager.Instance.Get(nActionStepID, isRealMode);
                        if (work != null)
                            work.Done(dtEnd, true);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
                else// if (string.Compare(strCancelTime, "null", true) != 0)
                {
                    try
                    {
                        DateTime dtCancel = Convert.ToDateTime(strCancelTime);
                        FormMain.Instance.StopWorkflow(dtCancel, true, nActionStepID, isRealMode);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }

            private void MonitorComponentHistory(int nActionStepID, bool isRealMode, bool isRegular, bool isNormal, string strActionStepFullPath, int nActionStepHistoryID)
            {
                PageBackstageHome pageHome = FormMain.Instance.GetPageHome();
                DockingLeftScenario scenario = pageHome.GetDockScenario();

                ArrayList arrAllSections = null;

                if (m_dicActionStepSections.ContainsKey(nActionStepID))
                    arrAllSections = m_dicActionStepSections[nActionStepID];
                else
                {
                    ArrayList arrPanels = pageHome.GetAllPanels(nActionStepID);
                    if (arrPanels == null)
                        return;
    
                    arrAllSections = scenario.GetAllPanelSections(arrPanels);
                }

                if (arrAllSections == null)
                    return;

                int nLastComponentHistoryID = -1;

                if (m_dicLastComponentHistory.ContainsKey(nActionStepID))
                    nLastComponentHistoryID = m_dicLastComponentHistory[nActionStepID];

                string strHistories = "";

                lock (scenario.ArrLoadHistory)
                {
                    foreach (int _nActionStepHistoryID in scenario.ArrLoadHistory)
                    {
                        if (strHistories.Length == 0)
                            strHistories = _nActionStepHistoryID.ToString();
                        else
                            strHistories += ", " + _nActionStepHistoryID.ToString();
                    }
                }

                if (strHistories.Length == 0)
                    return;

                strHistories = "(" + strHistories + ")";

                string strFormat = "select ID, ComponentID, ComponentType, Time, Status, Task, CompleteCount, CheckedNotify1, CheckedNotify2, Description, ShowBoard ";
                strFormat += "from ComponentHistory where ID > {0} and ActionStepHistoryID in " + strHistories;

                string strSQL = string.Format(strFormat, nLastComponentHistoryID);

                /*string strFormat = "select ID, ComponentID, ComponentType, Time, Status, Task, CompleteCount, CheckedNotify1, CheckedNotify2, Description, ShowBoard ";
                strFormat += "from ComponentHistory where ID > {0} and ActionStepHistoryID in ";
                strFormat += "(select id from ActionStepHistory where EndTime is NULL and CancelTime is NULL ";
                strFormat += "and id = (select max(id) from ActionStepHistory where ActionStepID = {1} and RealMode = {2}))";

                string strSQL = string.Format(strFormat, nLastComponentHistoryID, nActionStepID, isRealMode ? 1 : 0);*/

                ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);
                if (arrResult == null) return;

                int nResultCount = arrResult.Count;
                DateTime dtDefault = new DateTime();

                // Section, Section Status
                Dictionary<Sections.Section, int> dicSectionStatus = new Dictionary<Sections.Section, int>();
                ArrayList arrSections4Log = new ArrayList();
                ArrayList arrSectionStatus4Log = new ArrayList();
                ArrayList arrSectionProcessDirections4Log = new ArrayList();
                ArrayList arrDescription = new ArrayList();
                ArrayList arrTask = new ArrayList();
                ArrayList arrTime = new ArrayList();
                ArrayList arrShowBoard = new ArrayList();
                ArrayList arrComponentHistoryID = new ArrayList();
                ArrayList arrCheckedNotify1 = new ArrayList();
                ArrayList arrCheckedNotify2 = new ArrayList();

                for (int i = 0; i < nResultCount - 10; i += 11)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nComponentID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nComponentType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);
                    int nStatus = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    string strTask = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
                    int nCompleteCount = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                    int nCheckedNotify1 = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                    int nCheckedNotify2 = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
                    string strDescription = WebDBManager.GetStringField(arrResult[i + 9].ToString(), "");
                    bool showBoard = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0) == 0 ? false : true;

                    Sections.Section section = scenario.FindSection(nComponentID, nComponentType, arrAllSections);
                    if (section == null)
                        continue;

                    section.CompleteCount = nCompleteCount;

                    dicSectionStatus[section] = nStatus;

                    int nDirections = nStatus >> 16;
                    nStatus = nStatus & 0x0000ffff;

                    // SOP Log창 기록을 위한 List
                    arrComponentHistoryID.Add(nID);
                    arrSections4Log.Add(section);
                    arrSectionStatus4Log.Add(nStatus);
                    arrSectionProcessDirections4Log.Add(nDirections);
                    arrDescription.Add(strDescription);
                    arrTask.Add(strTask);
                    arrTime.Add(time);
                    arrShowBoard.Add(showBoard);
                    arrCheckedNotify1.Add(nCheckedNotify1);
                    arrCheckedNotify2.Add(nCheckedNotify2);

                    if (nID > nLastComponentHistoryID)
                        nLastComponentHistoryID = nID;
                }

                m_dicLastComponentHistory[nActionStepID] = nLastComponentHistoryID;

                Sections.WorkFlow workFlow = Sections.WorkFlowManager.Instance.Get(nActionStepID, isRealMode);

                if (workFlow == null)
                    return;

                if (arrSections4Log.Count == 0)
                    return;

                scenario.AddSOPSectionLog(nActionStepID, arrComponentHistoryID, arrSections4Log, arrSectionStatus4Log, arrSectionProcessDirections4Log, arrTask, arrTime, arrDescription, arrShowBoard, arrCheckedNotify1, arrCheckedNotify2, isRealMode, workFlow);

                string szPath = strActionStepFullPath;
                bool bHasPos = true;
				if (szPath.IndexOf("자연재해") != -1 || szPath.IndexOf("태풍") != -1)
                {
                    bHasPos = false;
                }
                string sopName = szPath.Substring(szPath.IndexOf("\\") + 1);

                workFlow.HasPosition = bHasPos;
                workFlow.SOPName = sopName;

                workFlow.State = Sections.WorkFlowState.RUN;

                FormMain.Instance.SetCurrentWorkflow(workFlow);

                int nSectionCount = arrAllSections.Count;

                for (int i = 0; i < nSectionCount; i++)
                {
                    Sections.Section section = (Sections.Section)arrAllSections[i];

                    // add by skkim : 2013-01-07 링크 노드 상태 세팅 제외
                    if (section.GetComponentType() == Sections.Section.ComponentType.LINK)
                        continue;

                    int nStatus = dicSectionStatus.ContainsKey(section) ? dicSectionStatus[section] : 1/*대기상태*/;
                    int nDirection = nStatus >> 16;

                    nStatus = nStatus & 0x0000ffff;

                    // changed by skkim : 2013-01-07 링크노드 상태 세팅 제외
                    //Sections.SectionState state = workFlow.FindState(section, true);
                    Sections.SectionState state = workFlow.FindState(section, false);


                    if (nStatus == 2)
                    {
                        Sections.Section.ComponentType type = state.Section.GetComponentType();

                        // 내부, 외부, 통합 상황전파는 실행 상태에 대하여 상태 정보를 따라하지 않는다.
                        // 실행 상태로 바뀐후 제어권을 넘겨받으면 옵션 정보가 없기 때문에 상황전파를 할 수가 없다.
                        if (type != Sections.Section.ComponentType.INTERNAL &&
                            type != Sections.Section.ComponentType.EXTERNAL &&
                            type != Sections.Section.ComponentType.TRANSMISSION)
                        {
                            //state.InProgress();
                            state.CopyState(Sections.State.RUN, Sections.WorkFlowManager.Instance.InProgressColor);
                            FormMain.Instance.FocusSection(state.Section);
                            /*ArrayList arList = workFlow.FindNext(state);
                            foreach (Sections.SectionState next in arList)
                            {
                                if (next != null)
                                {
                                    next.CopyState(Sections.State.INPUT, Sections.WorkFlowManager.Instance.InputWaitColor);
                                    //next.InputWait();
                                }
                            }*/
                        }
                    }
                    else if (nStatus == 3)
                    {
                        //state.Complete();
                        state.CopyState(Sections.State.DONE, Sections.WorkFlowManager.Instance.CompleteColor);
                        state.ProcessDirections = nDirection;
                    }
                    else if (nStatus == 4)
                    {
                        state.CopyState(Sections.State.INPUT, Sections.WorkFlowManager.Instance.InputWaitColor);
                        FormMain.Instance.FocusSection(state.Section);
                        //state.InputWait();
                    }
                    else if (nStatus == 5)
                    {
                        state.CopyState(Sections.State.SKIP, Sections.WorkFlowManager.Instance.SkipColor);
                        //state.Skip();
                    }
                }

                /*string strDisasterPath = dicDisasterFullPath[disaster];
                string strActionStepPath = GetActionStepPath(disaster.ActionSteps, panel.ActionStepID);

                if (strActionStepPath.Length == 0)
                    return;*/

                scenario.AddGridRowScenario(strActionStepFullPath, nActionStepID, isRealMode, isRegular, isNormal, nActionStepHistoryID);
            }

            // 제어권이 없는 상태이므로 제어권 가진 쪽에서 남긴 DB 정보를 확인하여 프로그램 갱신
            public void DoMonitoring()
            {
                if (!m_isWorkingMonitor)
                {
                    m_isWorkingMonitor = true;
                    // 실행중인 ActionStep 정보
                    // Type : long
                    //        4바이트(ActionStepID) + 1바이트(RealMode 여부) + 3바이트(사용 안함)
                    ArrayList arrRunningActionStep = new ArrayList();

                    // 기존에 실행중인 ActionStep들의 실행 이력 감시
                    DataGridView gridScenario = FormMain.Instance.GetPageHome().GetDockScenario().GetGridView();

                    foreach (DataGridViewRow row in gridScenario.Rows)
                    {
                        bool isRealMode = (bool)row.Cells[0].Tag;
                        bool isRegular = (bool)row.Cells[1].Tag;
                        bool isNormal = (bool)row.Cells[2].Tag;

                        int nActionStepID = (int)row.Cells[3].Tag;
                        if (nActionStepID < 0)
                            continue;

                        int nActionStepHistoryID = (int)row.Tag;

                        string strActionStepFullPath = (string)row.Cells[3].Value;

                        if (strActionStepFullPath != null && strActionStepFullPath.IndexOf("(훈련모드)") != -1)
                        {
                            strActionStepFullPath = strActionStepFullPath.Replace("(훈련모드)", "");
                        }

                        MonitorComponentHistory(nActionStepID, isRealMode, isRegular, isNormal, strActionStepFullPath, nActionStepHistoryID);
                        MonitorActionStepHistory(nActionStepID, isRealMode, isRegular, isNormal, strActionStepFullPath);

                        if (PageBackstageHome.IsWorkingMode(nActionStepID, isRealMode))
                        {
                            long nActionStepInfo = ((long)nActionStepID) << 32;
                            if (isRealMode) nActionStepInfo |= (1 << 24);

                            arrRunningActionStep.Add(nActionStepInfo);
                        }
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////

                    if (gridScenario.Rows.Count > 0)
                        MonitorCurrentActionStep();

                    MonitorNewActionStepHistory(arrRunningActionStep);
                    m_isWorkingMonitor = false;
                }
            }

            public ActionStepHistory ActionStepHistory
            {
                get { return m_allActionStepHistory; }
            }

            public static HistoryManager Instance
            {
                get { return m_instance; }
            }

            // 실행중인 SOP에 대한 재난 위치 정보
            // Key : 상위 4바이트(ActionStepID)
            //       하위 4바이트(isRealMode, 1이면 실제 모드, 0이면 가상모드)
            public Dictionary<long, SOPMonitoringSystem.HistoryDiasterPosition> HistoryDisasterPosition
            {
                get { return m_dicHistoryDisasterPosition; }
            }
        }

        public class HistoryActionStepData
        {
            private HistoryDiasterPosition m_Position = null;
            
            private DateTime m_time;
            private Sections.WorkFlowState m_state;
            private ActionStepInfo m_actionStep = null;
            private bool m_isRealMode = true;
            // 이 값이 true이면 DB에 기록하지 않고 Log 창에만 표시한다.
            private bool m_noDBWrite = false;

            public HistoryActionStepData(DateTime time, Sections.WorkFlowState state, ActionStepInfo actionStep, bool isRealMode)
            {
                m_time = time;
                m_state = state;
                m_actionStep = actionStep;
                m_isRealMode = isRealMode;
            }

            public DateTime Time
            {
                get { return m_time; }
                set { m_time = value; }
            }

            public Sections.WorkFlowState State
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

            public HistoryDiasterPosition Position
            {
                get { return m_Position; }
                set { m_Position = value; }
            }
        }

        public class HistorySectionData
        {
            private DateTime m_time;
            private Sections.State m_state;
            private int m_nProcessDirections;
            private Sections.Section m_section = null;
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
            private int m_nComponentHistoryID = -1;

            public HistorySectionData(DateTime time, Sections.State state, int nProcessDirections, Sections.Section section)
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

            public Sections.State State
            {
                get { return m_state; }
                set { m_state = value; }
            }

            public int ProcessDirections
            {
                get { return m_nProcessDirections; }
                set { m_nProcessDirections = value; }
            }

            public Sections.Section Section
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

            public int ComponentHistoryID
            {
                get { return m_nComponentHistoryID; }
                set { m_nComponentHistoryID = value; }
            }
        }

        public class HistorySectionDecisionData : HistorySectionData
        {
            // SectionDecision만 사용하며, 분기 다음에 선택된 Section이 어느것인지 알려준다.
            private Sections.Section m_sectionNext = null;

            public HistorySectionDecisionData(DateTime time, Sections.State state, int nProcessDirections, Sections.Section section, Sections.Section sectionNext)
                : base(time, state, nProcessDirections, section)
            {
                m_sectionNext = sectionNext;
            }

            public Sections.Section NextSection
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

            public HistorySectionInternalData(DateTime time, Sections.State state, int nProcessDirections, Sections.Section section, bool usePopupMessage, bool useSMS, bool useBroadcast)
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

            public HistorySectionExternalData(DateTime time, Sections.State state, int nProcessDirections, Sections.Section section, bool useSMS, bool useFax = false)
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

            public HistorySectionTransmissionData(DateTime time, Sections.State state, int nProcessDirections, Sections.Section section, bool usePopupMessage, bool useSMS, bool useBroadcast, bool useExSMS, bool useExFax = false)
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
    }
}
