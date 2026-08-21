using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using System.IO;
using SOPMonitoringSystem.Process;

namespace SOPMonitoringSystem
{
    public partial class DockingBottomSOPLog : Form
    {
        //private Dictionary<SOPData, ArrayList> m_dicTasks = new Dictionary<SOPData, ArrayList>();
        //private SOPData m_currentSOP = null;
        private ArrayList m_arrThreadGridRow = new ArrayList();
        // 전체 GridRow
        private ArrayList m_arrAllGridRow = new ArrayList();

        public enum ShowType { HIDE = 0, ALL, ACTION_STEP, COMPONENT };

        private ShowType m_showType = ShowType.ALL;
        // m_showType이 Action_STEP이면 ActionStepID, COMPONENT이면 ComponentID
        private int m_nShowingID = -1;
        private bool m_isShowingRealMode = true;
        private Sections.Section.ComponentType m_showingComponentType = Sections.Section.ComponentType.NONE;
        private XtremeDockingPane.Pane m_paneParent = null;

        // ActionStepID(0보다 크면 실제 모드, 0보다 작으면 모의훈련모드), ActionStepDetailLog
        private Dictionary<int, ActionStepDetailLog> m_dicActionStepHistory = new Dictionary<int, ActionStepDetailLog>();
        private DateTime m_dtPrev = new DateTime();

        private string m_strDefaultTitle = "";

        private bool m_isWorkingTimer = false;
        private int m_reservationComboBoxChange = -1;

        //private int DBNum = 0;
        private FormMain frm;
        private WebDBManager dbMgr;
        private bool _DBSetting = false;
        private ArrayList arrSQLResult;
        private bool _DBAlive = false;
        private DateTime dt;

        public DockingBottomSOPLog()
        {
            InitializeComponent();
            InitGrid();
            
            m_strDefaultTitle = this.Text;

            // 로그 정보가 쓰레드에서 전달되기 때문에 이를 처리할 Timer를 가동한다.
            timer1.Start();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Visible)
            {
                FormMain.Instance.FrmMain2.ApplyWindow(this.Handle.ToInt32());               
            }
        }

        private string SearchDisa(string actionStepID)
        {
            try
            {
                string strSQL = "select DisasterID, StepName from ActionStep where ID = " + actionStepID;
                ArrayList arrActionStep = dbMgr.GetResultData(strSQL, 0);

                strSQL = "select ID, DisasterName, SubDisasterID from Disaster where ID = " + arrActionStep[0];
                ArrayList arrDisaster = dbMgr.GetResultData(strSQL, 0);

                strSQL = "select ID, SubCategoryName, DisasterID from SubDisastercategory where ID = " + arrDisaster[2];
                ArrayList arrSubDisastercategory = dbMgr.GetResultData(strSQL, 0);

                strSQL = "select ID, CategoryName from DisasterCategory where ID = " + arrSubDisastercategory[2];
                ArrayList arrDisastercategory = dbMgr.GetResultData(strSQL, 0);

                string result = arrDisastercategory[1].ToString() + "/" + arrSubDisastercategory[1].ToString() + "/" + arrDisaster[1].ToString() + "/" + arrActionStep[1].ToString();
                return result;
            }
            catch
            {
                return null;
            }
        }
        private bool SearchDB()
        {
            if (arrSQLResult == null)
                return false;

            arrSQLResult.Clear();

            //string nID = FileRead();
            DateTime dtLastRead = FileRead();
            string strSQL = "SELECT SendTime, ActionStepID, Message, ID, ActionStepHistoryID from Message WHERE SendTime > " +
                string.Format("'{0}-{1}-{2} {3}:{4}:{5}'", dtLastRead.Year, dtLastRead.Month, dtLastRead.Day, dtLastRead.Hour, dtLastRead.Minute, dtLastRead.Second);
            arrSQLResult = dbMgr.GetResultData(strSQL, 0);
            /*
            try
            {
                string strSQL = "SELECT SendTime, ActionStepID, Message, ID from Message WHERE ID > " + FileRead();
                arrSQLResult = dbMgr.GetResultData(strSQL, 1);
            }
            catch
            {
                string strSQL = "SELECT SendTime, ActionStepID, Message, ID from Message WHERE ID > 0";
                arrSQLResult = dbMgr.GetResultData(strSQL, 1);
            }*/
            if (arrSQLResult == null)
                return false;

            return arrSQLResult.Count != 0 ? true : false;
        }
        //private bool SearchDB()
        //{
        //    if (arrResult != null)
        //        arrResult.Clear();

        //    //runningSOP();

        //    string strSQL = "SELECT SendTime, ActionStepID, Message, State, ID from Message WHERE ID > " + DBNum;
        //    StringBuilder strResult = new StringBuilder();
        //    strResult.Append(strSQL);

        //    if (arrSOP.Count > 0)
        //    {
        //        strResult.Append(" and ( ");
        //        for (int i = 0; i < arrSOP.Count; i++)
        //        {
        //            strResult.Append(" ActionStepID = " + arrSOP[i]);
        //            if (i != arrSOP.Count - 1)
        //                strResult.Append(" or ");
        //        }
        //        strResult.Append(" ) order by ID asc");
        //    }

        //    arrResult = dbMgr.GetResultData(strResult.ToString(), 1);

        //    if (arrResult == null)
        //        return false;

        //    return arrResult.Count != 0 ? true : false;
        //}
        private void InitGrid()
        {
            frm = FormMain.Instance;
            dbMgr = frm.DBManager;
            dt = new DateTime();

            arrSQLResult = new ArrayList();

            foreach (DataGridViewColumn column in gridLog.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            gridLog.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //toolStripComboBox.SelectedIndex = 1;
            toolStripComboBox.SelectedIndex = 0;    // 전체 로그

        }

        public void SetPane(XtremeDockingPane.Pane pane)
        {
            m_paneParent = pane;
        }

        public void HideLog()
        {
            if (m_showType == ShowType.HIDE)
                return;

            if (m_paneParent != null)
                m_paneParent.Title = string.Format("{0} - Hide", m_strDefaultTitle);

            m_showType = ShowType.HIDE;
            gridLog.Rows.Clear();
        }

        public void ShowAllLog()
        {
            if (m_showType == ShowType.ALL)
                return;

            if (m_paneParent != null)
                m_paneParent.Title = string.Format("{0} - All", m_strDefaultTitle);

            m_showType = ShowType.ALL;
            gridLog.Rows.Clear();

            int nRowCount = m_arrAllGridRow.Count;

            for (int i=0;i<nRowCount;i++)
            {
                DataLogGridViewRow row = (DataLogGridViewRow)m_arrAllGridRow[i];
                row.Cells[0].Value = (i + 1).ToString();
                gridLog.Rows.Add(row);
            }
        }

        public void ShowActionStepLog(int nActionStepID, bool isRealMode, string strFullPath, bool updateComponentContents = true)
        {
            if (!m_isWorkingTimer)
            {
                m_isWorkingTimer = true;

                if (toolStripComboBox.SelectedIndex != 1)
                {
                    ShowAllLog();
                    m_isWorkingTimer = false;
                    return;
                }

                if (m_showType == ShowType.ACTION_STEP &&
                    m_nShowingID == nActionStepID && m_isShowingRealMode == isRealMode)
                {
                    m_isWorkingTimer = false;
                    return;
                }

                if (m_paneParent != null)
                    m_paneParent.Title = string.Format("{0} - {1}", m_strDefaultTitle, strFullPath);

                m_showType = ShowType.ACTION_STEP;
                m_nShowingID = nActionStepID;
                m_isShowingRealMode = isRealMode;

                int nIndex = 0;
                gridLog.Rows.Clear();

                if (updateComponentContents)
                    FormMain.Instance.GetPageHome().ClearProcess();

                int nActionStepHistoryID = FormMain.Instance.SOPManager.GetActionStepHistoryID(nActionStepID, isRealMode);

                foreach (DataLogGridViewRow row in m_arrAllGridRow)
                {
                    if (row.ActionStepID == nActionStepID && row.IsRealMode == isRealMode && row.ActionStepHistoryID == nActionStepHistoryID)
                    {
                        ++nIndex;
                        row.Cells[0].Value = nIndex.ToString();
                        gridLog.Rows.Add(row);

                        Sections.Section section = row.Section;
                        if (section != null)
                        {
                            DateTime time = DateTime.ParseExact(row.Cells[1].Value.ToString(), "yyyy-MM-dd HH:mm:ss", null);
                            string strComponentType = row.Cells[4].Value.ToString();
                            string strTask = row.Cells[5].Value.ToString();

                            if (row.Cells[5].Tag == null)
                            {
                                Sections.SectionState state = Sections.WorkFlowManager.Instance.Find(section, isRealMode);
                                if (state == null)
                                    continue;

                                row.Cells[5].Tag = state.CheckNotify1;

                                if (row.Cells[6].Tag == null)
                                    row.Cells[6].Tag = state.CheckNotify2;
                            }

                            if (row.Cells[6].Tag == null)
                            {
                                Sections.SectionState state = Sections.WorkFlowManager.Instance.Find(section, isRealMode);
                                if (state == null)
                                    continue;

                                row.Cells[6].Tag = state.CheckNotify2;
                            }

                            if (updateComponentContents)
                            {
                                int nCheckNotify1 = (int)row.Cells[5].Tag;
                                string strStatus = row.Cells[6].Value.ToString();
                                int nCheckNotify2 = (int)row.Cells[6].Tag;
                                FormMain.Instance.GetPageHome().GetComponentContents(row.ActionStepID, row.ComponentHistoryID, row.ComponentType, time, strComponentType, strTask, strStatus, section, row.SectionState, nCheckNotify1, nCheckNotify2, row);
                            }
                        }
                    }
                    else if (row.ActionStepID < 0)
                    {

                    }
                }

                m_isWorkingTimer = false;
            }
        }

        public void ShowComponentLog(int nComponentID, bool isRealMode, Sections.Section.ComponentType type, Sections.Section section)
        {
            if (toolStripComboBox.SelectedIndex != 1)
            {
                ShowAllLog();
                return;
            }

            if (m_showType == ShowType.COMPONENT &&
                m_nShowingID == nComponentID && m_isShowingRealMode == isRealMode &&
                m_showingComponentType == type)
                return;

            if (m_paneParent != null)
                m_paneParent.Title = string.Format("{0} - Component({1})", m_strDefaultTitle, section.Title);

            m_showType = ShowType.COMPONENT;
            m_nShowingID = nComponentID;
            m_showingComponentType = type;
            m_isShowingRealMode = isRealMode;

            int nIndex = 0;
            gridLog.Rows.Clear();

            foreach (DataLogGridViewRow row in m_arrAllGridRow)
            {
                if (row.ComponentID == nComponentID && row.IsRealMode == isRealMode && row.ComponentType == type)
                {
                    ++nIndex;
                    row.Cells[0].Value = nIndex.ToString();
                    gridLog.Rows.Add(row);
                }
            }
        }

        public void RemoveLog(int nActionStepID, bool isRealMode)
        {
            int nAllLogCount = m_arrAllGridRow.Count;

            for (int i = nAllLogCount - 1; i >= 0; i--)
            {
                DataLogGridViewRow row = (DataLogGridViewRow)m_arrAllGridRow[i];

                if (row.ActionStepID == nActionStepID && row.IsRealMode == isRealMode)
                {
                    m_arrAllGridRow.RemoveAt(i);
                }
            }

            int nCurrentLogCount = gridLog.Rows.Count;

            for (int i = nCurrentLogCount - 1; i >= 0; i--)
            {
                DataLogGridViewRow row = (DataLogGridViewRow)m_arrAllGridRow[i];

                if (row.ActionStepID == nActionStepID && row.IsRealMode == isRealMode)
                    gridLog.Rows.RemoveAt(i);
            }
        }

        // arrProcess : SectionProcess들의 ID List(long), 상위 4바이트(Component Type, Section.ComponentType), 하위 4바이트(Component ID)
        // isRealMode
        public ActionStepDetailLog MakeActionStepLog(int nActionStepID, bool isRealMode, int nHistoryID, DateTime dtBegin, ArrayList arrProcess)
        {
            ActionStepDetailLog log = new ActionStepDetailLog();

            log.HistoryID = nHistoryID;
            log.IsRealMode = isRealMode;
            log.BeginTime = new TimeInfo(dtBegin);

            foreach (long nComponentID in arrProcess)
            {
                log.SetMissionStatus(nComponentID, ActionStepDetailLog.Status.WAITING);
            }

            m_dicActionStepHistory[isRealMode ? nActionStepID : -nActionStepID] = log;
            return log;
        }

        public void CompleteActionStepDetailLog(int nActionStepID, bool isRealMode, DateTime dtEnd)
        {
            if (!isRealMode)
                nActionStepID = -nActionStepID;

            if (!m_dicActionStepHistory.ContainsKey(nActionStepID))
                return;

            ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];
            log.EndTime = new TimeInfo(dtEnd);
        }

        public void CancelActionStepDetailLog(int nActionStepID, bool isRealMode, DateTime dtCancel)
        {
            if (!isRealMode)
                nActionStepID = -nActionStepID;

            if (!m_dicActionStepHistory.ContainsKey(nActionStepID))
                return;

            ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];
            log.CancelTime = new TimeInfo(dtCancel);
        }

        private void SetActionStepDetailLog(History.HistorySectionData data, int nActionStepID, bool isRealMode, int nComponentID)
        {
            Sections.Section.ComponentType componentType = data.Section.GetComponentType();

            if (componentType != Sections.Section.ComponentType.PROCESS)
            {
                if (componentType == Sections.Section.ComponentType.ENDPOINT)
                {
                    Sections.SectionDataEndPoint sectionData = (Sections.SectionDataEndPoint)data.Section.Data;

                    if (!sectionData.IsBegin && data.State == Sections.State.DONE)
                        CompleteActionStepDetailLog(nActionStepID, isRealMode, data.Time);
                }

                return;
            }

            ActionStepDetailLog.Status status;

            if (data.State == Sections.State.NORMAL)
                status = ActionStepDetailLog.Status.WAITING;
            else if (data.State == Sections.State.RUN)
                status = ActionStepDetailLog.Status.PROCESSING;
            else if (data.State == Sections.State.DONE)
                status = ActionStepDetailLog.Status.COMPLETED;
            else if (data.State == Sections.State.SKIP)
                status = ActionStepDetailLog.Status.SKIPPED;
            else
                return;

            if (!isRealMode)
                nActionStepID = -nActionStepID;

            if (!m_dicActionStepHistory.ContainsKey(nActionStepID))
                return;

            ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];
            long nID = ((int)data.Section.GetComponentType() << 32) | nComponentID;

            log.SetMissionStatus(nID, status);
        }

        /*public void FileWrite(int SendData) // 현재까지 읽은 디비 갯수 입력
        {
            string strPath = Application.StartupPath + "\\SOPMonitoringReceiveMessage.txt";
            StreamWriter WriteFile = new StreamWriter(strPath, false, Encoding.Unicode);
            WriteFile.Write(SendData);
            WriteFile.Close();
            WriteFile.Dispose();
        }*/
        public void FileWrite(DateTime dtLastRead) // 현재까지 읽은 마지막 메시지의 발송시간
        {
            string strPath = Application.StartupPath + "\\SOPMonitoringReceiveMessage.txt";
            StreamWriter WriteFile = new StreamWriter(strPath, false, Encoding.Unicode);

            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtLastRead.Year, dtLastRead.Month, dtLastRead.Day, dtLastRead.Hour, dtLastRead.Minute, dtLastRead.Second);

            WriteFile.Write(strTime);
            WriteFile.Close();
            WriteFile.Dispose();
        }

        private int GetMaxMessageID()
        {
            string strSQL = "select max(id) from Message";

            WebDBManager dbMgr = FormMain.Instance.DBManager;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private DateTime GetMaxMessageTime(ref bool isSuccess)
        {
            string strSQL = "select max(SendTime) from Message";

            WebDBManager dbMgr = FormMain.Instance.DBManager;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                isSuccess = false;
                return new DateTime();
            }

            DateTime result;

            try
            {
                result = Convert.ToDateTime(arrResult[0]);
                isSuccess = true;
            }
            catch (Exception)
            {
                result = new DateTime();
                isSuccess = false;
            }

            return result;
        }

        /*public string FileRead() // 프로그램 종료 전까지 읽은 디비 갯수 읽어오기
        {
            string strPath = Application.StartupPath + "\\SOPMonitoringReceiveMessage.txt";

            if (!System.IO.File.Exists(strPath))
            {
                StreamWriter WriteFile = new StreamWriter(strPath, false, Encoding.Unicode);
                int nMaxID = GetMaxMessageID();

                WriteFile.Write(nMaxID);
                WriteFile.Close();
                return nMaxID.ToString();
            }

            StreamReader ReadFile = new StreamReader(strPath, System.Text.Encoding.Default);
            string Read_num = ReadFile.ReadToEnd().ToString();
            ReadFile.Close();
            ReadFile.Dispose();

            return Read_num;
        }*/

        public DateTime FileRead() // 프로그램 종료 전까지 읽은 마지막 메시지 발생 시간 읽어오기
        {
            string strPath = Application.StartupPath + "\\SOPMonitoringReceiveMessage.txt";

            if (!System.IO.File.Exists(strPath))
            {
                StreamWriter WriteFile = new StreamWriter(strPath, false, Encoding.Unicode);
                
                bool isSuccess = true;
                DateTime dtMax = GetMaxMessageTime(ref isSuccess);

                if (!isSuccess)
                {
                    WriteFile.Close();
                    return dtMax;
                }

                string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtMax.Year, dtMax.Month, dtMax.Day, dtMax.Hour, dtMax.Minute, dtMax.Second);

                WriteFile.Write(strTime);
                WriteFile.Close();
                return dtMax;
            }

            StreamReader ReadFile = new StreamReader(strPath, System.Text.Encoding.Default);
            string Read_Time = ReadFile.ReadToEnd().ToString();
            ReadFile.Close();
            ReadFile.Dispose();

            DateTime result;

            try
            {
                result = Convert.ToDateTime(Read_Time);
            }
            catch (Exception)
            {
                result = new DateTime();
            }

            return result;
        }

        // nActionStepHistoryID : 0보다 작을 경우 -1이면 수신 메시지, -2이면 발신메시지
        // nComponentID가 0보다 작으면 단위 Component가 아닌 전체 ActionStep에 대한 로그
        public DataLogGridViewRow AddLogData(Sections.Section section, bool noDBWrite, int nActionStepHistoryID, int nComponentHistoryID, int nActionStepID, bool isRealMode, int nComponentID, Sections.Section.ComponentType componentType, DateTime time, string strStepMemberName, string strTeamList, string strComponentType, string strTask, string strStatus, int nCompleteCount = -1, bool callByThread = false, bool showBoard = false)
        {
            int nID = gridLog.Rows.Count + 1;

            DataLogGridViewRow row = new DataLogGridViewRow();

            row.ActionStepID = nActionStepID;
            row.ComponentID = nComponentID;
            row.ComponentType = componentType;
            row.IsRealMode = isRealMode;
            row.Section = section;
            row.NoDBWrite = noDBWrite;

            if (section != null)
            {
                if (strStatus == "대기")
                    row.SectionState = Sections.State.NORMAL;
                else if (strStatus == "입력대기")
                    row.SectionState = Sections.State.INPUT;
                else if (strStatus == "실행중")
                    row.SectionState = Sections.State.RUN;
                else if (strStatus == "건너뛰기")
                    row.SectionState = Sections.State.SKIP;
                else if (strStatus == "실행 완료")
                    row.SectionState = Sections.State.DONE;
                else
                {
                    Sections.SectionState state = FormMain.Instance.CurrentWork.FindState(section);
                    if (state != null)
                        row.SectionState = state.State;
                }
            }


            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nID.ToString();
            row.Cells.Add(cell);

            row.ActionStepHistoryID = nActionStepHistoryID;
            row.ComponentHistoryID = nComponentHistoryID;

            cell = new DataGridViewTextBoxCell();
            cell.Value = string.Format("{0} {1:00}:{2:00}:{3:00}", time.ToShortDateString(), time.Hour, time.Minute, time.Second);
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strStepMemberName == null ? "-" : strStepMemberName;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strTeamList == null ? "-" : strTeamList;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strComponentType == null ? "-" : strComponentType;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strTask == null ? "-" : strTask.Replace("\r\n", " ");
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strStatus == null ? "-" : strStatus;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = nCompleteCount <= 0 ? "-" : nCompleteCount.ToString();
            row.Cells.Add(cell);

            DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
            checkCell.Value = showBoard;
            row.Cells.Add(checkCell);

            checkCell = new DataGridViewCheckBoxCell();
            checkCell.Value = true;
            row.Cells.Add(checkCell);

            //////////////////////////////////////////////////////////////////////////
            // ADD Check Notify
            if (section != null)
            {
                Sections.SectionState state = Sections.WorkFlowManager.Instance.Find(section, isRealMode);
                if (state != null)
                {
                    row.Cells[5].Tag = state.CheckNotify1;                   
                    row.Cells[6].Tag = state.CheckNotify2;
                }
            }
            //////////////////////////////////////////////////////////////////////////

            if (callByThread)
            {
                m_arrThreadGridRow.Add(row);
            }
            else
            {
                ArrayList arrRunAction = FormMain.Instance.GetPageHome().GetDockScenario().GetRunActionStepHistory();
                int nStepID = WebDBManager.GetIntField(arrRunAction[1].ToString(), -1);
                gridLog.Rows.Add(row);

                //scroll을 마지막 행으로 위치 변경
                gridLog.FirstDisplayedScrollingRowIndex = gridLog.RowCount - 1;
                gridLog.Rows[gridLog.RowCount - 1].Selected = true;
            }

            m_arrAllGridRow.Add(row);

            if (FormMain.Instance.GetReport() == null) return null;
            FormMain.Instance.GetReport().AddProgressReport(strStepMemberName, strTeamList, strComponentType, strTask, strStatus);
            //FormMain.Instance.GetRealTimeInfo(strStepMemberName, strTeamList, strComponentType, strTask, strStatus, false);
            FormMain.Instance.GetRealTimeInfo(strStepMemberName, strTeamList, strComponentType, strTask, strStatus, FormRealTimeInfo.MessageType.LOG_DATA);

            return row;
        }

        delegate void RcvMessage_Invoke(string time, string disa, string act);
        public void AddLogDBData(string time, int nActionStepID, string message)
        {
            string ActionStepID = SearchDisa(nActionStepID.ToString());
            int nID = gridLog.Rows.Count + 1;

            DataLogGridViewRow row = new DataLogGridViewRow();
            row.ActionStepID = nActionStepID;
            
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nID.ToString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = time;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "-";
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "-";
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = ActionStepID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = message;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "수신 메세지";
            row.Cells.Add(cell);

            ArrayList arrRunAction = FormMain.Instance.GetPageHome().GetDockScenario().GetRunActionStepHistory();

            if (arrRunAction != null)
            {
                foreach (string data in arrRunAction)
                {
                    int nStepID = WebDBManager.GetIntField(arrRunAction[0].ToString(), -1);
                    int nHistoryID = WebDBManager.GetIntField(arrRunAction[1].ToString(), -1);

                    if (nHistoryID == nActionStepID)
                    {
                        gridLog.Rows.Add(row);

                        //scroll을 마지막 행으로 위치 변경 
                        gridLog.FirstDisplayedScrollingRowIndex = gridLog.RowCount - 1;
                        gridLog.Rows[gridLog.RowCount - 1].Selected = true;

                        break;
                    }
                }
            }
            else
            {
                gridLog.Rows.Add(row);

                //scroll을 마지막 행으로 위치 변경 
                gridLog.FirstDisplayedScrollingRowIndex = gridLog.RowCount - 1;
                gridLog.Rows[gridLog.RowCount - 1].Selected = true;
            }

            m_arrAllGridRow.Add(row);

            //ArrayList arrRunAction = FormMain.Instance.GetPageHome().GetDockScenario().GetRunActionStepHistory();

            RcvMessage_Invoke CI = new RcvMessage_Invoke(FormMain.Instance.GetPageHome().DockingMessage.AddGridData);

            if (!FormMain.Instance.CloseThread)
                FormMain.Instance.GetPageHome().DockingMessage.Invoke(CI, time, ActionStepID, message);

            //FormMain.Instance.GetRealTimeInfo("", "", "", message, "", false);
            FormMain.Instance.GetRealTimeInfo("", "", "", message, "", FormRealTimeInfo.MessageType.RECV_MESSAGE);

            ArrayList arrLoadHistory = FormMain.Instance.GetPageHome().GetDockScenario().ArrLoadHistory;
        }
        
        //delegate void Ctrl_Invoke(int nActionStepID, Sections.Section.ComponentType componentType, DateTime time, string strComponentType, string strTask, string strStatus, Sections.Section section, int nCheck1, int nCheck2);
        public void AddLog(int nActionStepHistoryID, int nComponentHistoryID, History.HistorySectionData data, int nActionStepID, bool isRealMode, int nComponentID, string strStepMemberName, string strTeamList, string strComponentType, string strTask, string strStatus, int nCompleteCount = -1, bool callByThread = false)
        {
            while (callByThread && m_isWorkingTimer)
            {
                System.Threading.Thread.Sleep(50);
            }

            Sections.Section section = data.Section;
            DateTime time = data.Time;

            int nID = gridLog.Rows.Count + 1;

            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();
            //DataLogGridViewRow row = new DataLogGridViewRow();

            DataLogGridViewRow row = AddLogData(data.Section, data.NoDBWrite, nActionStepHistoryID, nComponentHistoryID, nActionStepID, isRealMode, nComponentID, section.GetComponentType(), time, strStepMemberName, strTeamList, strComponentType, strTask, strStatus, nCompleteCount, callByThread, data.ShowBoard);

            //if(strStatus != "대기")
            {
                PageBackstageHome pageHome = FormMain.Instance.GetPageHome();

                //Ctrl_Invoke CI = new Ctrl_Invoke(pageHome.GetComponentContents);
                pageHome.CurrentSection = section;
                Sections.SectionState state = Sections.WorkFlowManager.Instance.Find( section, isRealMode);

                if( state != null)
                {
                    int nCheckNotify1 = state.CheckNotify1;
                    int nCheckNotify2 = state.CheckNotify2;
                    if (!FormMain.Instance.CloseThread)
                    {
                        pageHome.Invoke((MethodInvoker)delegate
                        {
                            pageHome.GetComponentContents(nActionStepID, nComponentHistoryID, section.GetComponentType(), time, strComponentType, strTask, strStatus, section, data.State, nCheckNotify1, nCheckNotify2, row);
                            
                        });

                    }
                        //pageHome.Invoke(CI, nActionStepID, section.GetComponentType(), time, strComponentType, strTask, strStatus, section, nCheckNotify1, nCheckNotify2);
         
                }
            }

            //FormMain.Instance.GetPageHome().GetComponentContents();

            /*row.ActionStepID = panel.ActionStepID;
            row.ComponentID = nComponentID;
            row.ComponentType = section.GetComponentType();
            row.IsRealMode = isRealMode;
            
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nID.ToString();
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = string.Format("{0} {1:00}:{2:00}:{3:00}", time.ToShortDateString(), time.Hour, time.Minute, time.Second);
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strStepMemberName == null ? "-" : strStepMemberName;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strTeamList == null ? "-" : strTeamList;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strComponentType == null ? "-" : strComponentType;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strTask == null ? "-" : strTask;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strStatus == null ? "-" : strStatus;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = nCompleteCount < 0 ? "-" : nCompleteCount.ToString();
            row.Cells.Add(cell);

            if (callByThread)
            {
                //lock(m_arrThreadGridRow)
                //{
                    m_arrThreadGridRow.Add(row);
                //}
            }
            else
                gridLog.Rows.Add(row);

            m_arrAllGridRow.Add(row);*/
            SetActionStepDetailLog(data, nActionStepID, isRealMode, nComponentID);
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            if (m_isWorkingTimer == false)
            {
                m_isWorkingTimer = true;

                int nThreadRowCount = m_arrThreadGridRow.Count;
                //Debug.WriteLine("Enter : " + System.Threading.Thread.CurrentThread.GetHashCode());
                for (int i = 0; i < nThreadRowCount; i++)
                {
                    DataLogGridViewRow row = (DataLogGridViewRow)m_arrThreadGridRow[0];
                    try
                    {
                        if (!gridLog.Rows.Contains(row))
                            gridLog.Rows.Add(row);
                    }
                    catch (Exception)
                    {
                        //Debug.WriteLine(ex.Message);
                    }

                    m_arrThreadGridRow.RemoveAt(0);
                }

                TabPage tabCurrent = frm.GetPageHome().TabControls.SelectedTab;
                int nActionStepID = -1;
                if (tabCurrent != null)
                {
                    nActionStepID = frm.GetTabActionStepID(tabCurrent);
                    SendProgress(nActionStepID, frm.IsReal);
                }

                if (_DBAlive == true) // DB 감시
                {
                    if (_DBSetting == false) // DB 초기 세팅
                    {
                        //FileWrite(0);

                        //frm.PlaySiren(1);

                        //InitDB(FormMain.Instance.GetPageHome().GetDockScenario().GetRunActionStepHistory(), 0);
                        
                        /*for (int i = 0; i < arrSQLResult.Count-4 ; i += 5)
                        {
                            AddLogDBData(arrSQLResult[i].ToString(), SearchDisa(arrSQLResult[i + 1].ToString()), arrSQLResult[i + 2].ToString(), arrSQLResult[i + 3].ToString());
                        }

                        if (arrSQLResult.Count > 0)
                            FileWrite(int.Parse(arrSQLResult[arrSQLResult.Count-1].ToString()));
                        */

                        //string strSQL = "SELECT SendTime, ActionStepID, Message, ID, ActionStepHistoryID from Message";
                        DateTime dtLastRead = FileRead();
                        string strTime = string.Format("'{0}-{1}-{2} {3}:{4}:{5}'", dtLastRead.Year, dtLastRead.Month, dtLastRead.Day, dtLastRead.Hour, dtLastRead.Minute, dtLastRead.Second);
                        string strSQL = string.Format("SELECT SendTime, ActionStepID, Message, ID, ActionStepHistoryID from Message where ActionStepID = {0} and SendTime > {1}",
                            nActionStepID, strTime);

                        ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                        if (arrResult == null || arrResult.Count == 0)
                        {
                            _DBSetting = true;
                            m_isWorkingTimer = false;
                            return;
                        }

                        if (arrResult.Count > 0)
                        {
                            for (int i = 0; i < arrResult.Count - 4; i += 5)
                            {
                                dt = Convert.ToDateTime(arrResult[i].ToString());
                                    AddLogDBData(dt.ToString("yyyy-MM-dd HH:mm:ss"), WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1), arrResult[i + 2].ToString());
                            }
                        }

                        ArrayList arrResul = new ArrayList();

                        try
                        {
                            strSQL = "SELECT SendTime, ActionStepID, Message, ID, ActionStepHistoryID from Message WHERE ID > " + FileRead();
                            arrResul = dbMgr.GetResultData(strSQL, 0);
                        }
                        catch
                        {
                            strSQL = "SELECT SendTime, ActionStepID, Message, ID, ActionStepHistoryID from Message WHERE ID > 0";
                            arrResul = dbMgr.GetResultData(strSQL, 0);
                        }

                        if (arrResul == null)
                        {
                            _DBSetting = true;
                            m_isWorkingTimer = false;
                            return;
                        }

                        if (arrResul.Count > 0)
                        {
                            NewReceiveMessage NRM = new NewReceiveMessage();
                            for (int i = 0; i < arrResul.Count - 4; i += 5)
                            {
                                dt = Convert.ToDateTime(arrResul[i].ToString());
                                NRM.AddGridData(dt.ToString("yyyy-MM-dd HH:mm:ss"), SearchDisa(arrResul[i + 1].ToString()), arrResul[i + 2].ToString());
                            }

                            frm.PlayDoorBell();

                            NRM.Show();
                            //int nId = int.Parse(arrResult[arrResult.Count - 2].ToString());
                            //FileWrite(nId);
                            DateTime dtSend = WebDBManager.GetDateTimeField(arrResult[0], new DateTime());
                            FileWrite(dtSend);
                        }
                        _DBSetting = true;           

                    }
                    else if (SearchDB())
                    {
                        frm.PlayDoorBell();

                        if (arrSQLResult.Count > 0)
                        {
                            DateTime dt = new DateTime();
                            
                            for (int i = 0; i < arrSQLResult.Count - 3; i += 4)
                            {
                                try
                                {
                                    dt = Convert.ToDateTime(arrSQLResult[i].ToString());
                                    AddLogDBData(dt.ToString("yyyy-MM-dd HH:mm:ss"), WebDBManager.GetIntField(arrSQLResult[i + 1].ToString(), -1), arrSQLResult[i + 2].ToString());
                                }
                                catch (Exception)
                                {
                                }
                            }
                            try
                            {
                                //FileWrite(int.Parse(arrSQLResult[3].ToString()));
                                FileWrite(WebDBManager.GetDateTimeField(arrSQLResult[0], new DateTime()));
                            }
                            catch (Exception)
                            {
                            }
                        }                     
                    }             
                }

                // 자탐센서 감시
				//Popup.PopupSensorOn.MonitorSensors(FormMain.Instance.HasControl);

                m_isWorkingTimer = false;
                _DBAlive = true;

                if (!FormMain.Instance.HasControl)
                {
                    DockingLeftScenario scenario = FormMain.Instance.GetPageHome().GetDockScenario();

                    if (scenario != null)
                    {
                        // 처음 프로그램 로딩후 ComponentHistory를 로딩한 후에 DoMonitoring을 실시할 수 있다.
                        if (scenario.FinishLoadingComponentHistory)
                        {
                            // 제어권이 없는 상태이므로 제어권 가진 쪽에서 남긴 DB 정보를 확인하여 프로그램 갱신
                            History.HistoryManager.Instance.DoMonitoring();
                        }
                    }
                }
            }

            if (m_reservationComboBoxChange >= 0)
            {
                SetLogViewOption(m_reservationComboBoxChange == 0 ? false : true);
                m_reservationComboBoxChange = -1;
            }

            try
            {
                TTSManager.Instance.SetState();
            }
            catch (System.Exception)
            {
			}
        }

        public void ReservationComboBoxChange(bool allLogView)
        {
            m_reservationComboBoxChange = allLogView ? 1 : 0;
        }

        private void SendProgress(int nActionStepID, bool isRealMode)
        {
            if (nActionStepID < 0)
                return;

            if (!isRealMode)
                nActionStepID = -nActionStepID;

            DateTime dtCurrent = DateTime.Now;

            if (m_dtPrev == dtCurrent)
                return;

            m_dtPrev = dtCurrent;

            if (!m_dicActionStepHistory.ContainsKey(nActionStepID))
            {
                FormMain.Instance.GetPageHome().GetDockProgress().Initialize(dtCurrent);
                return;
            }

            ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];

            int nTotalCount = log.TotalMissionCount;
            int nCompletedCount = log.CompletedMissionCount;
            int nProcessingCount = log.ProcessingMissionCount;
            int nSkippedCount = log.SkippedMissionCount;

            DockingRightProgress progress = FormMain.Instance.GetPageHome().GetDockProgress();

            progress.SetStartTime(log.BeginTime.m_time);
            progress.SetCurrentTime(dtCurrent, log.EndTime, log.CancelTime);
            progress.SetMissionInfo(nTotalCount, nCompletedCount, nProcessingCount, nSkippedCount);
        }

        public ActionStepDetailLog GetActionStepDetailLog(int nActionStepID, bool isRealMode)
        {
            if (!isRealMode)
                nActionStepID = -nActionStepID;

            if (!m_dicActionStepHistory.ContainsKey(nActionStepID))
                return null;

            return m_dicActionStepHistory[nActionStepID];
        }

        public void SetLogViewOption(bool allLogView)
        {
            if (allLogView)
                toolStripComboBox.SelectedIndex = 0;
            else
                toolStripComboBox.SelectedIndex = 1;
        }

        public int GetCurrentLogCount()
        {
            return gridLog.Rows.Count;
        }

        private void toolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox.SelectedIndex == 0)   // 전체 보기
            {
                ShowAllLog();
            }
            else                                        // 개별 보기
            {
                PageBackstageHome pageHome = FormMain.Instance.GetPageHome();

                if (pageHome != null)
                {
                    Sections.SectionTabPage tabPage = (Sections.SectionTabPage)pageHome.TabControls.SelectedTab;

                    if (tabPage != null)
                    {
                        TreeNode node = FormMain.Instance.GetPageHome().GetDockScenario().GetBarLevelTree().FindActionStepNode(tabPage.ActionStepID);

                        if (node != null)
                        {
                            string strFullPath = node.FullPath.Replace('\\', '/');
                            ShowActionStepLog(tabPage.ActionStepID, !tabPage.VirtualMode, strFullPath);
                        }
                    }
                }
            }
        }

        private void gridLog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 8)
            {
                DataLogGridViewRow row = (DataLogGridViewRow)gridLog.Rows[e.RowIndex];
                bool isChecked = !(bool)row.Cells[e.ColumnIndex].Value;

                string strSQL = string.Format("update ComponentHistory set ShowBoard = {0} where id = {1}",
                    isChecked ? 1 : 0, row.ComponentHistoryID);

                FormMain.Instance.DBManager.GetResultData(strSQL, 0);
            }
        }
    }

    public class DataLogGridViewRow : DataGridViewRow
    {
        private int m_nActionStepID = -1;
        private int m_nComponentID = -1;
        private bool m_isRealMode = true;
        private Sections.Section.ComponentType m_componentType = Sections.Section.ComponentType.NONE;
        private int m_nActionStepHistoryID = -1;
        private int m_nComponentHistoryID = -1;
        private Sections.Section m_section = null;
        private Sections.State m_sectionState = Sections.State.NORMAL;
        private bool m_noDBWrite = false;

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        public int ComponentID
        {
            get { return m_nComponentID; }
            set { m_nComponentID = value; }
        }

        public bool IsRealMode
        {
            get { return m_isRealMode; }
            set { m_isRealMode = value; }
        }

        public Sections.Section.ComponentType ComponentType
        {
            get { return m_componentType; }
            set { m_componentType = value; }
        }

        public int ActionStepHistoryID
        {
            get { return m_nActionStepHistoryID; }
            set { m_nActionStepHistoryID = value; }
        }

        public int ComponentHistoryID
        {
            get { return m_nComponentHistoryID; }
            set { m_nComponentHistoryID = value; }
        }

        public Sections.Section Section
        {
            get { return m_section; }
            set { m_section = value; }
        }

        public Sections.State SectionState
        {
            get { return m_sectionState; }
            set { m_sectionState = value; }
        }

        public bool NoDBWrite
        {
            get { return m_noDBWrite; }
            set { m_noDBWrite = value; }
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
        private Dictionary<long, Status> m_dicComponentStatus = new Dictionary<long,Status>();

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
