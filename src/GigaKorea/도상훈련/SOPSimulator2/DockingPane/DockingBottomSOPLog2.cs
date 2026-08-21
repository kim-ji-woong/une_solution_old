using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using SOPMonitoringSystem.Process;


using Sections;
using UnE.SOP.Tree;
using UnE.SOP.History;
using UnE.SOP.Log;
using UnE.SOP.Data;
using UnE.SOP.Workstate;
using UnE.SOP.Sections;
using UnE.SOP.TTS;
using UnE.SOP;

namespace SOPMonitoringSystem
{
	public partial class DockingBottomSOPLog : Form, ISOPLogContainer
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
		
		// ActionStepID(0보다 크면 실제 모드, 0보다 작으면 모의훈련모드), ActionStepDetailLog
		private Dictionary<int, ActionStepDetailLog> m_dicActionStepHistory = new Dictionary<int, ActionStepDetailLog>();
		private DateTime m_dtPrev = new DateTime();

		private string m_strDefaultTitle = "";

		private bool m_isWorkingTimer = false;
		private int m_reservationComboBoxChange = -1;

		//private int DBNum = 0;
		private FormSOP frm;
		private WebDBManager dbMgr;
		private bool _DBSetting = false;
		private ArrayList m_arReadMessages;
		private bool _DBAlive = false;
		private DateTime dt;

        private int m_nSiteID = 1;
		public DockingBottomSOPLog()
		{
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

			InitializeComponent();
			InitGrid();

			m_strDefaultTitle = this.Text;

			// 로그 정보가 쓰레드에서 전달되기 때문에 이를 처리할 Timer를 가동한다.
			
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);

			if (this.Visible)
			{
				//if (FormSOP.Instance.FrmMain2 != null)
				//    FormSOP.Instance.FrmMain2.ApplyWindow(this.Handle.ToInt32());
			}
		}

		private char szDeli = (char)0x06;
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

				string result = arrDisastercategory[1].ToString() + szDeli + arrSubDisastercategory[1].ToString() + szDeli + arrDisaster[1].ToString() + szDeli + arrActionStep[1].ToString();
				return result;
			}
			catch
			{
				return null;
			}
		}
		private bool SearchDB()
		{
			if (m_arReadMessages == null)
				return false;

			m_arReadMessages.Clear();
            // DB 부하가 너무 심하여 아래의 로그조회는 수행하지 않는다.
            // [2017/06/10] 김지웅
            return false;
			/*DateTime dtLastRead = ReadLastMsgTimeInFile();
            
            string strTime = string.Format("'{0}-{1}-{2} {3}:{4}:{5}'", dtLastRead.Year, dtLastRead.Month, dtLastRead.Day, dtLastRead.Hour, dtLastRead.Minute, dtLastRead.Second);
            
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("SELECT msg.SendTime, msg.ActionStepID, msg.Message, msg.ID, msg.ActionStepHistoryID FROM Message as msg ");
            sb1.AppendFormat(" INNER JOIN ActionStep as step on msg.ActionStepID = step.ID and msg.SendTime > {0} ", strTime);
            sb1.Append(" INNER JOIN Disaster as dis on step.DisasterID = dis.ID ");
            sb1.Append(" INNER JOIN SubDisasterCategory as sdc on dis.SubDisasterID = sdc.ID ");
            sb1.AppendFormat(" INNER JOIN DisasterCategory as dc on sdc.DisasterID = dc.ID and dc.SiteID = {0} ", m_nSiteID);

			//string strSQL = "SELECT SendTime, ActionStepID, Message, ID, ActionStepHistoryID from Message WHERE SendTime > " +

            string strSQL = sb1.ToString();
			m_arReadMessages = dbMgr.GetResultData(strSQL, 0);

			if (m_arReadMessages == null)
				return false;

			return m_arReadMessages.Count != 0 ? true : false;*/
		}

		private void InitGrid()
		{
			frm = FormSOP.Instance;
			dbMgr = frm.DBManager;
			dt = new DateTime();

			m_arReadMessages = new ArrayList();

			foreach (DataGridViewColumn column in gridLog.Columns)
			{
				column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
				column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
			}

			gridLog.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

			cboSelectType.SelectedIndex = 0;    // 전체 로그
		}

		public void HideLog()
		{
			if (m_showType == ShowType.HIDE)
				return;

			Text = string.Format("{0} - Hide", m_strDefaultTitle);

			m_showType = ShowType.HIDE;
			gridLog.Rows.Clear();
		}

		public void ShowAllLog()
		{
			if (m_showType == ShowType.ALL)
				return;

			Text = string.Format("{0} - All", m_strDefaultTitle);

			m_showType = ShowType.ALL;
			gridLog.Rows.Clear();

			int nRowCount = m_arrAllGridRow.Count;

			for (int i = 0; i < nRowCount; i++)
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

				if (cboSelectType.SelectedIndex == 0)
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

				Text = string.Format("{0} - {1}", m_strDefaultTitle, strFullPath);

				m_showType = ShowType.ACTION_STEP;
				m_nShowingID = nActionStepID;
				m_isShowingRealMode = isRealMode;

				int nIndex = 0;
				gridLog.Rows.Clear();

				if (updateComponentContents)
					FormSOP.Instance.GetPageHome().ClearProcess();

				int nActionStepHistoryID = FormSOP.Instance.SOPManager.GetActionStepHistoryID(nActionStepID, isRealMode);

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
								SectionState state = WorkFlowManager.Instance.Find(section, isRealMode);
								if (state == null)
									continue;

								row.Cells[5].Tag = state.CheckNotify1;

								if (row.Cells[6].Tag == null)
									row.Cells[6].Tag = state.CheckNotify2;
							}

							if (row.Cells[6].Tag == null)
							{
								SectionState state = WorkFlowManager.Instance.Find(section, isRealMode);
								if (state == null)
									continue;

								row.Cells[6].Tag = state.CheckNotify2;
							}

							if (updateComponentContents)
							{
								int nCheckNotify1 = (int)row.Cells[5].Tag;
								string strStatus = row.Cells[6].Value.ToString();
								int nCheckNotify2 = (int)row.Cells[6].Tag;
								FormSOP.Instance.GetPageHome().GetComponentContents(row.ActionStepID, row.ComponentHistoryID, row.ComponentType, time, strComponentType, strTask, strStatus, section, row.SectionState, nCheckNotify1, nCheckNotify2, row);
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

        public void UpdateComponentContents(int nActionStepID, bool isRealMode)
        {
            if (!m_isWorkingTimer)
            {
               

                m_isWorkingTimer = true;
                

                int nActionStepHistoryID = FormSOP.Instance.SOPManager.GetActionStepHistoryID(nActionStepID, isRealMode);

                ArrayList arTemp = (ArrayList)m_arrAllGridRow.Clone();
                foreach (DataLogGridViewRow row in arTemp)
                {
                    if (row.ActionStepID == nActionStepID && row.IsRealMode == isRealMode && row.ActionStepHistoryID == nActionStepHistoryID)
                    {
                        Sections.Section section = row.Section;
                        if (section != null)
                        {
                            DateTime time = DateTime.ParseExact(row.Cells[1].Value.ToString(), "yyyy-MM-dd HH:mm:ss", null);
                            string strComponentType = row.Cells[4].Value.ToString();
                            string strTask = row.Cells[5].Value.ToString();

                            int nCheckedNotify1 = 0;
                            int nCheckedNotify2 = 0;

                            if (row.Cells[5].Tag == null)
                            {
                                SectionState state = WorkFlowManager.Instance.Find(section, isRealMode);
                                if (state == null)
                                    continue;

                                nCheckedNotify1 = state.CheckNotify1;

                                if (row.Cells[6].Tag == null)
                                    nCheckedNotify2 = state.CheckNotify2;
                            }
                            else
                                nCheckedNotify1 = (int)row.Cells[5].Tag;

                            if (row.Cells[6].Tag == null)
                            {
                                SectionState state = WorkFlowManager.Instance.Find(section, isRealMode);
                                if (state == null)
                                    continue;

                                nCheckedNotify2 = state.CheckNotify2;
                            }
                            else
                                nCheckedNotify2 = (int)row.Cells[6].Tag;

                            string strStatus = row.Cells[6].Value.ToString();
                            FormSOP.Instance.GetPageHome().GetComponentContents(row.ActionStepID, row.ComponentHistoryID, row.ComponentType, time, strComponentType, strTask, strStatus, section, row.SectionState, nCheckedNotify1, nCheckedNotify2, row);
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
			if (cboSelectType.SelectedIndex != 1)
			{
				ShowAllLog();
				return;
			}

			if (m_showType == ShowType.COMPONENT &&
				m_nShowingID == nComponentID && m_isShowingRealMode == isRealMode &&
				m_showingComponentType == type)
				return;

			Text = string.Format("{0} - Component({1})", m_strDefaultTitle, section.Title);

			m_showType = ShowType.COMPONENT;
			m_nShowingID = nComponentID;
			m_showingComponentType = type;
			m_isShowingRealMode = isRealMode;

			int nIndex = 0;
			gridLog.Rows.Clear();

			try
			{
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
			catch (System.InvalidOperationException e)
			{
				// Loop를 도는 도중 m_arrAllGridRow가 변경되었음
				System.Diagnostics.Trace.WriteLine(e.Message);
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
		public ActionStepDetailLog MakeActionStepLog(int nActionStepID, bool isRealMode, int nHistoryID, int nSensorZoneHistory, DateTime dtBegin, ArrayList arrProcess)
		{
			ActionStepDetailLog log = new ActionStepDetailLog();

			log.HistoryID = nHistoryID;
			log.IsRealMode = isRealMode;
			log.BeginTime = new TimeInfo(dtBegin);
            log.SensorZoneHistoryID = nSensorZoneHistory;

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

		private void SetActionStepDetailLog(HistorySectionData data, int nActionStepID, bool isRealMode, int nComponentID)
		{
			Sections.Section.ComponentType componentType = data.Section.GetComponentType();

			if (componentType != Sections.Section.ComponentType.PROCESS)
			{
				if (componentType == Sections.Section.ComponentType.ENDPOINT)
				{
					Sections.SectionDataEndPoint sectionData = (Sections.SectionDataEndPoint)data.Section.Data;

					if (!sectionData.IsBegin && data.State == State.DONE)
						CompleteActionStepDetailLog(nActionStepID, isRealMode, data.Time);
				}
				return;
			}

			ActionStepDetailLog.Status status;
			if (data.State == State.NORMAL)
				status = ActionStepDetailLog.Status.WAITING;
			else if (data.State == State.RUN)
				status = ActionStepDetailLog.Status.PROCESSING;
			else if (data.State == State.DONE)
				status = ActionStepDetailLog.Status.COMPLETED;
			else if (data.State == State.SKIP)
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

		public bool WriteLastMsgTimeToFile(DateTime dtLastRead) // 현재까지 읽은 마지막 메시지의 발송시간
		{
            string dirPath = Application.StartupPath + "\\logs";
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                string strPath = dirPath + "\\ReceivedMessage.txt";

                StreamWriter WriteFile = new StreamWriter(strPath, false, Encoding.Unicode);

                string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtLastRead.Year, dtLastRead.Month, dtLastRead.Day, dtLastRead.Hour, dtLastRead.Minute, dtLastRead.Second);

                WriteFile.Write(strTime);
                WriteFile.Close();
                WriteFile.Dispose();
            }
            catch(Exception)
            {
                return false;
            }
            return true;
		}

		private int GetMaxMessageID()
		{
			string strSQL = "select max(id) from Message";

			WebDBManager dbMgr = FormSOP.Instance.DBManager;
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null || arrResult.Count == 0)
				return 0;

			return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
		}

		private DateTime GetMaxMessageTime(ref bool isSuccess)
		{
			//string strSQL = "select max(SendTime) from Message";

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT max(msg.SendTime) FROM Message as msg ");
            sb.Append(" INNER JOIN ActionStep as step ON msg.ActionStepID = step.ID ");
            sb.Append(" INNER JOIN Disaster as dis ON step.DisasterID = dis.ID ");
            sb.Append(" INNER JOIN SubDisasterCategory as sdc ON dis.SubDisasterID = sdc.ID ");
            sb.AppendFormat(" INNER JOIN DisasterCategory as dc ON sdc.DisasterID = dc.ID AND dc.SiteID = {0}", m_nSiteID);

            string strSQL = sb.ToString();

			WebDBManager dbMgr = FormSOP.Instance.DBManager;
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

		public DateTime ReadLastMsgTimeInFile() // 프로그램 종료 전까지 읽은 마지막 메시지 발생 시간 읽어오기
		{
            string dirPath = Application.StartupPath + "\\logs";
            string szReadTime = "";
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                string strPath = dirPath + "\\ReceivedMessage.txt";
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
                szReadTime = ReadFile.ReadToEnd().ToString();
                ReadFile.Close();
                ReadFile.Dispose();

            }
            catch(Exception)
            {
                return DateTime.Now;
            }
                      
            if( szReadTime == "")
                return DateTime.Now;

            DateTime result;
			try
			{
                result = Convert.ToDateTime(szReadTime);
			}
			catch (Exception)
			{
                return DateTime.Now;
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
					row.SectionState = State.NORMAL;
				else if (strStatus == "입력대기")
					row.SectionState = State.INPUT;
				else if (strStatus == "실행중")
					row.SectionState = State.RUN;
				else if (strStatus == "건너뛰기")
					row.SectionState = State.SKIP;
				else if (strStatus == "실행 완료")
					row.SectionState = State.DONE;
				else
				{
					WorkFlow currentWork = FormSOP.Instance.CurrentWork;

					if (currentWork != null)
					{
						SectionState state = FormSOP.Instance.CurrentWork.FindState(section);
						if (state != null)
							row.SectionState = state.State;
					}
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
				SectionState state = WorkFlowManager.Instance.Find(section, isRealMode);
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
				ArrayList arrRunAction = SOPScenarioManager.Instance.GetRunActionStepHistory();
				int nStepID = WebDBManager.GetIntField(arrRunAction[1].ToString(), -1);
				gridLog.Rows.Add(row);

				//scroll을 마지막 행으로 위치 변경
				gridLog.FirstDisplayedScrollingRowIndex = gridLog.RowCount - 1;
				gridLog.Rows[gridLog.RowCount - 1].Selected = true;
			}

			m_arrAllGridRow.Add(row);

			if (FormSOP.Instance.GetReport() == null)
				return null;

			FormSOP.Instance.GetReport().AddProgressReport(strStepMemberName, strTeamList, strComponentType, strTask, strStatus);
			
			FormSOP.Instance.GetRealTimeInfo(strStepMemberName, strTeamList, strComponentType, strTask, strStatus, UnE.Utility.RealTimeInfoPane.MessageType.LOG_DATA);

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
			cell.Value = "수신 메시지";
			row.Cells.Add(cell);

			ArrayList arrRunAction = SOPScenarioManager.Instance.GetRunActionStepHistory();

			if (arrRunAction != null)
			{
				foreach (object obj in arrRunAction)
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

			RcvMessage_Invoke CI = new RcvMessage_Invoke(FormSOP.Instance.GetPageHome().DockingMessage.AddGridData);
            
            if (!FormSOP.Instance.CloseThread)
				FormSOP.Instance.GetPageHome().DockingMessage.Invoke(CI, time, ActionStepID, message);		  
			FormSOP.Instance.GetRealTimeInfo("", "", "", message, "", UnE.Utility.RealTimeInfoPane.MessageType.RECV_MESSAGE);
		}
				
		public void AddLog(int nActionStepHistoryID, int nComponentHistoryID, HistorySectionData data, int nActionStepID, bool isRealMode, int nComponentID, string strStepMemberName, string strTeamList, string strComponentType, string strTask, string strStatus, int nCompleteCount = -1, bool callByThread = false)
		{
			if (HistoryManager.Instance.Exit)
				return;

			while (callByThread && m_isWorkingTimer)
			{
				System.Threading.Thread.Sleep(50);
			}

			Sections.Section section = data.Section;
			DateTime time = data.Time;

			int nID = gridLog.Rows.Count + 1;

			Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();


            //System.Diagnostics.Trace.WriteLine("AddLog Begin : " + DateTime.Now);
            //System.Diagnostics.Trace.WriteLine("AddLog Info : " + data.Section.SectionName);
            //System.Diagnostics.Trace.WriteLine("AddLog Info : " + nActionStepHistoryID);
            //System.Diagnostics.Trace.WriteLine("AddLog Info : " + nComponentHistoryID);
            //System.Diagnostics.Trace.WriteLine("AddLog Info : " + nActionStepID);
            //System.Diagnostics.Trace.WriteLine("AddLog Info : " + isRealMode);
            //System.Diagnostics.Trace.WriteLine("AddLog Info : " + nComponentID);
            //System.Diagnostics.Trace.WriteLine("AddLog Info : " + strStepMemberName + " " + strTeamList + " " + strComponentType + " " + strTask + " " + strStatus + " " + nCompleteCount + " " + callByThread);

            

			DataLogGridViewRow row = AddLogData(data.Section, data.NoDBWrite, nActionStepHistoryID, nComponentHistoryID, nActionStepID, isRealMode, nComponentID, section.GetComponentType(), time, strStepMemberName, strTeamList, strComponentType, strTask, strStatus, nCompleteCount, callByThread, data.ShowBoard);
            //System.Diagnostics.Trace.WriteLine("AddLog End : " + DateTime.Now);
			PageBackstageSOP pageHome = FormSOP.Instance.GetPageHome();
			pageHome.CurrentSection = section;
			SectionState state = WorkFlowManager.Instance.Find(section, isRealMode);
			if (state != null)
			{
				int nCheckNotify1 = state.CheckNotify1;
				int nCheckNotify2 = state.CheckNotify2;
				if (!FormSOP.Instance.CloseThread)
				{
                    //System.Diagnostics.Trace.WriteLine("SetComponent Begin : " + DateTime.Now);

                    pageHome.Invoke((MethodInvoker)delegate
                    {
                        pageHome.GetComponentContents(nActionStepID, nComponentHistoryID, section.GetComponentType(), time, strComponentType, strTask, strStatus, section, data.State, nCheckNotify1, nCheckNotify2, row);
                    });

					//pageHome.BeginInvoke(new Action(() => );
                    //System.Diagnostics.Trace.WriteLine("SetComponent End : " + DateTime.Now);
                }
			}

			SetActionStepDetailLog(data, nActionStepID, isRealMode, nComponentID);
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (m_isWorkingTimer == false)
			{
				m_isWorkingTimer = true;

				int nThreadRowCount = m_arrThreadGridRow.Count;

                // 개별로그 보기일 때, 이전에 쌓여있던 SOP로그를 지우고 새로 보여주도록 함.
                if (cboSelectType.SelectedIndex != 0 && nThreadRowCount > 0)
                {
                    int nCurrActionStepHistoryID = (m_arrThreadGridRow[0] as DataLogGridViewRow).ActionStepHistoryID;

                    List<DataLogGridViewRow> delRows = new List<DataLogGridViewRow>();

                    foreach (DataLogGridViewRow row in from rows in gridLog.Rows.Cast<DataLogGridViewRow>()
                                                       where rows.ActionStepHistoryID != nCurrActionStepHistoryID
                                                       select rows
                                                      )
                    {
                        delRows.Add(row);
                    }

                    foreach (DataLogGridViewRow delRow in delRows)
                    {
                        gridLog.Rows.Remove(delRow);
                    }

                }

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
					}
					m_arrThreadGridRow.RemoveAt(0);
				}

				PageBackstageSOP pageHome = frm.GetPageHome();
				if (pageHome == null)
				{
					m_isWorkingTimer = false;
					return;
				}

				TabPage tabCurrent = pageHome.TabControls.GetValidTabPageCount() > 0 ? pageHome.TabControls.SelectedTab : null;
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
						
						DateTime dtLastRead = ReadLastMsgTimeInFile();
						string strTime = string.Format("'{0}-{1}-{2} {3}:{4}:{5}'", dtLastRead.Year, dtLastRead.Month, dtLastRead.Day, dtLastRead.Hour, dtLastRead.Minute, dtLastRead.Second);
						//string strSQL = string.Format("SELECT SendTime, ActionStepID, Message, ID, ActionStepHistoryID from Message where ActionStepID = {0} and SendTime > {1}",
						//	nActionStepID, strTime);

                        StringBuilder sb = new StringBuilder();
                        sb.Append("SELECT msg.SendTime, msg.ActionStepID, msg.Message, msg.ID, msg.ActionStepHistoryID FROM Message as msg ");
                        sb.AppendFormat(" INNER JOIN ActionStep as step on msg.ActionStepID = step.ID and msg.ActionStepID = {0} and msg.SendTime > {1} ",nActionStepID, strTime);
                        sb.Append(" INNER JOIN Disaster as dis on step.DisasterID = dis.ID ");
                        sb.Append(" INNER JOIN SubDisasterCategory as sdc on dis.SubDisasterID = sdc.ID ");
                        sb.AppendFormat(" INNER JOIN DisasterCategory as dc on sdc.DisasterID = dc.ID and dc.SiteID = {0} ", m_nSiteID);

                        string strSQL = sb.ToString();

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

                        // 시스템 기동중이지 않을 때 발생한 메시지를 표시한다.
                        // 전체 새 메시지를 표시하기 위해 전체 실행중인 ActionStep의 메시지를 가져온다.
						ArrayList arResult2 = new ArrayList();
						try
						{							    
                            StringBuilder sb1 = new StringBuilder();
                            sb1.Append("SELECT msg.SendTime, msg.ActionStepID, msg.Message, msg.ID, msg.ActionStepHistoryID FROM Message as msg ");
                            sb1.AppendFormat(" INNER JOIN ActionStep as step on msg.ActionStepID = step.ID and msg.SendTime > {0} ",strTime);
                            sb1.Append(" INNER JOIN Disaster as dis on step.DisasterID = dis.ID ");
                            sb1.Append(" INNER JOIN SubDisasterCategory as sdc on dis.SubDisasterID = sdc.ID ");
                            sb1.AppendFormat(" INNER JOIN DisasterCategory as dc on sdc.DisasterID = dc.ID and dc.SiteID = {0} ", m_nSiteID);

                            strSQL = sb1.ToString();
                            arResult2 = dbMgr.GetResultData(strSQL, 0);
						}
						catch(Exception ex)
						{
                            System.Diagnostics.Trace.WriteLine(ex.Message);
                            System.Diagnostics.Trace.WriteLine(ex.StackTrace);
						}

                        if (arResult2 == null)
						{
							_DBSetting = true;
							m_isWorkingTimer = false;
							return;
						}

                        if (arResult2.Count > 0)
						{
                            DateTime dtLastMessage = ReadLastMsgTimeInFile();

							NewReceiveMessage NRM = new NewReceiveMessage();
                            for (int i = 0; i < arResult2.Count - 4; i += 5)
							{
                                dt = Convert.ToDateTime(arResult2[i].ToString());
                                dtLastMessage = WebDBManager.GetDateTimeField(arResult2[i], new DateTime());
                                NRM.AddGridData(dt.ToString("yyyy-MM-dd HH:mm:ss"), SearchDisa(arResult2[i + 1].ToString()), arResult2[i + 2].ToString());
							}

                            // 마지막 읽은 시간을 기록한다.
                            WriteLastMsgTimeToFile(dtLastMessage);

                            // 새 메시지 알려주기
							frm.PlayDoorBell();
							NRM.Show();                          
                                                 
						}
						_DBSetting = true;

					}
					else if (SearchDB())
					{
						frm.PlayDoorBell();

						if (m_arReadMessages.Count > 0)
						{
							DateTime dt = new DateTime();

							for (int i = 0; i < m_arReadMessages.Count - 4; i += 5)
							{
								try
								{
									dt = Convert.ToDateTime(m_arReadMessages[i].ToString());
									AddLogDBData(dt.ToString("yyyy-MM-dd HH:mm:ss"), WebDBManager.GetIntField(m_arReadMessages[i + 1].ToString(), -1), m_arReadMessages[i + 2].ToString());
								}
								catch (Exception)
								{
								}
							}
							try
							{
								WriteLastMsgTimeToFile(WebDBManager.GetDateTimeField(m_arReadMessages[0], new DateTime()));
							}
							catch (Exception)
							{
							}
						}
					}
				}

				m_isWorkingTimer = false;
				_DBAlive = true;

				if (!FormSOP.Instance.HasControl)
				{
					// 처음 프로그램 로딩후 ComponentHistory를 로딩한 후에 DoMonitoring을 실시할 수 있다.
					if (SOPScenarioManager.Instance.FinishLoadingComponentHistory)
					{
                        ReadControlUser();
						// 제어권이 없는 상태이므로 제어권 가진 쪽에서 남긴 DB 정보를 확인하여 프로그램 갱신
						HistoryManager.Instance.DoMonitoring();
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

        private void ReadControlUser()
        {
            string strSQL = "Select UserID from ControlUser where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            int nUserID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            if (nUserID > 0)
                FormSOP.Instance.ControlUserID = nUserID;
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
				//FormSOP.Instance.GetPageHome().GetDockProgress().Initialize(dtCurrent);
				return;
			}

			ActionStepDetailLog log = m_dicActionStepHistory[nActionStepID];

			int nTotalCount = log.TotalMissionCount;
			int nCompletedCount = log.CompletedMissionCount;
			int nProcessingCount = log.ProcessingMissionCount;
			int nSkippedCount = log.SkippedMissionCount;

			//DockingRightProgress progress = FormSOP.Instance.GetPageHome().GetDockProgress();

			//progress.SetStartTime(log.BeginTime.m_time);
			//progress.SetCurrentTime(dtCurrent, log.EndTime, log.CancelTime);
			//progress.SetMissionInfo(nTotalCount, nCompletedCount, nProcessingCount, nSkippedCount);
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
				cboSelectType.SelectedIndex = 0;
			else
				cboSelectType.SelectedIndex = 1;
		}

		public int GetCurrentLogCount()
		{
			return gridLog.Rows.Count;
		}

		private void cboSelectType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cboSelectType.SelectedIndex == 0)   // 전체 보기
			{
				ShowAllLog();
			}
			else                                        // 개별 보기
			{
				PageBackstageSOP pageHome = FormSOP.Instance.GetPageHome();
				if (pageHome != null)
				{
					try
					{
						SectionTabPage tabPage = pageHome.TabControls.GetValidTabPageCount() > 0 ? (SectionTabPage)pageHome.TabControls.SelectedTab : null;

						if (tabPage != null)
						{
							ISOPTreeContainer tree = ProxySOP.Instance.SOPTreeContainer;
							if (tree != null)
							{
								TreeNode node = tree.FindActionStepNode(tabPage.ActionStepID);
								if (node != null)
								{
									string strFullPath = node.FullPath.Replace('\\', szDeli);
									ShowActionStepLog(tabPage.ActionStepID, !tabPage.VirtualMode, strFullPath, false);
								}
							}
						}
					}
					catch(Exception ex)
					{
						System.Diagnostics.Trace.WriteLine(ex.Message);
						System.Diagnostics.Trace.WriteLine(ex.StackTrace);
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

				FormSOP.Instance.DBManager.GetResultData(strSQL, 0);
			}
		}

		private void DockingBottomSOPLog_Resize(object sender, EventArgs e)
		{
			this.panelTitleBar.Size = new Size(this.Size.Width, panelTitleBar.Size.Height);
			this.panelSelectLog.Size = new Size(this.Size.Width, panelSelectLog.Size.Height);
			this.gridLog.Size = new Size(this.Size.Width, this.Size.Height - panelTitleBar.Size.Height - panelSelectLog.Size.Height);
		}

        public void AddSOPSectionLog(int nActionStepID, ArrayList arrComponentHistoryID, ArrayList arrSections, ArrayList arrStatus, ArrayList arrProcessDirections, ArrayList arrTask, ArrayList arrTime, ArrayList arrDescription, ArrayList arrShowBoard, ArrayList arrCheckedNotify1, ArrayList arrCheckedNotify2, ArrayList arrCheckedRun, ArrayList arrCheckedComplete, ArrayList arrAccessedUserID, bool isRealMode, Dictionary<int, List<HistorySectionData.DetailData>> dicDetailDatas, WorkFlow workFlow)
		{
            SOPScenarioManager.Instance.AddSOPSectionLog(nActionStepID, arrComponentHistoryID, arrSections, arrStatus, arrProcessDirections, arrTask, arrTime, arrDescription, arrShowBoard, arrCheckedNotify1, arrCheckedNotify2, arrCheckedRun, arrCheckedComplete, arrAccessedUserID, isRealMode, dicDetailDatas, workFlow);
		}

		private void DockingBottomSOPLog_Load(object sender, EventArgs e)
		{
            StartTimer();
		}

        public void StartTimer()
        {
            if (timer1.Tag == null || (bool)(timer1.Tag) == false)
            {
                timer1.Tag = true;
                timer1.Start();
            }
        }

        public void StopTimer()
        {
            if (timer1.Tag == null)
                return;

            if ((bool)(timer1.Tag) == true)
            {
                timer1.Stop();
                timer1.Tag = false;
            }
        }

        private void OnDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                FormSOP.Instance.GetPageHome().OneTop(PageBackstageSOP.Player.SectionLog);
            }
        }

        private void gridLog_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridViewRow row = gridLog.Rows[e.RowIndex];
            if (row == null)
                return;

            // ID 값을 시퀀스에 맞게 조정
            row.Cells[0].Value = e.RowIndex + 1;

            // Scroll을 마지막 행으로 위치 변경
            gridLog.FirstDisplayedScrollingRowIndex = gridLog.RowCount - 1;
            gridLog.Rows[gridLog.RowCount - 1].Selected = true;
        }
	}
   
}
