using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Win32;
using DBUtility;


namespace SOPBulletin
{
    public partial class DockingRealTime : Form
    {
        private int m_nSelectedSOPIndex = -1;

        // Grid내의 Cell에 있는 ComboBox에 Text 이외의 값을 넣을수 없기 때문에,
        // ComboBox의 특정 행에 연결된 부가 데이터들을 저장한다.
        private ArrayList m_arrSOPPositions = new ArrayList();
        private ArrayList m_arrActionStepHistory = new ArrayList();

        // 근무 시작 시간
        private int m_nWorkingBeginHour = 9;
        private int m_nWorkingBeginMinute = 0;
        // 근무 종료 시간
        private int m_nWorkingEndHour = 18;
        private int m_nWorkingEndMinute = 0;

        private string m_strConfigFilePath = "";

        private const int SOP_NAME_TITLE_INDEX = 0;
        private const int SOP_NAME_INDEX = 1;
        private const int SOP_COMMANDER_TITLE_INDEX = 2;
        private const int SOP_COMMANDER_INDEX = 3;
        private const int SOP_LOCATION_TITLE_INDEX = 4;
        private const int SOP_LOCATION_INDEX = 5;
        private const int SOP_START_TIME_TITLE_INDEX = 6;
        private const int SOP_START_TIME_INDEX = 7;
        private const int SOP_ELAPSED_TIME_TITLE_INDEX = 8;
        private const int SOP_ELAPSED_TIME_INDEX = 9;

        private Color m_dataGridView1CellBorderLineColor = Color.Black;
        private int m_dataGridView1CellBorderLineThick = 3;
        private Dictionary<int, CellBorderLine> m_dicCellBorderLine = new Dictionary<int, CellBorderLine>();

        private Color m_titleCellBackColor = Color.FromArgb(185, 122, 87);
        private Color m_bodyCellBackColor = Color.FromArgb(181, 230, 29);
        //private Color m_bodyCellBackColor = Color.FromArgb(223, 244, 157);

        private FormDataList m_frmDataList = new FormDataList();

        public new ContextMenuStrip ContextMenu
        {
            get { return contextMenuStrip1; }
        }

        public DockingRealTime()
        {
            InitializeComponent();

            ReadGridSize();

            AddRowSOPState1();
            AddRowSOPState2();
            SetRegistry();

            contextMenuStrip1.Items.Remove(tsMenuDataList);
            //contextMenuStrip1.Items.Remove(tsMenuInitialize);

            dataGridView3.CellPainting += dataGridView3_CellPainting;

            ReadWorkingTime();
            SetBorderLine();
        }

        private void SetBorderLine()
        {
            m_dicCellBorderLine[SOP_NAME_TITLE_INDEX] = new CellBorderLine(true, true, false, true);
            m_dicCellBorderLine[SOP_NAME_INDEX] = new CellBorderLine(false, true, true, true);
            m_dicCellBorderLine[SOP_COMMANDER_TITLE_INDEX] = new CellBorderLine(true, true, false, true);
            m_dicCellBorderLine[SOP_COMMANDER_INDEX] = new CellBorderLine(false, true, true, true);
            m_dicCellBorderLine[SOP_LOCATION_TITLE_INDEX] = new CellBorderLine(true, true, false, true);
            m_dicCellBorderLine[SOP_LOCATION_INDEX] = new CellBorderLine(false, true, true, true);
            m_dicCellBorderLine[SOP_START_TIME_TITLE_INDEX] = new CellBorderLine(true, true, false, true);
            m_dicCellBorderLine[SOP_START_TIME_INDEX] = new CellBorderLine(false, true, true, true);
            m_dicCellBorderLine[SOP_ELAPSED_TIME_TITLE_INDEX] = new CellBorderLine(true, true, false, true);
            m_dicCellBorderLine[SOP_ELAPSED_TIME_INDEX] = new CellBorderLine(false, true, true, true);

            dataGridView1.GridColor = m_dataGridView1CellBorderLineColor;

            DataGridViewColumnCollection columns = dataGridView1.Columns;
            columns[SOP_NAME_TITLE_INDEX].DefaultCellStyle.BackColor = columns[SOP_COMMANDER_TITLE_INDEX].DefaultCellStyle.BackColor = columns[SOP_LOCATION_TITLE_INDEX].DefaultCellStyle.BackColor = columns[SOP_START_TIME_TITLE_INDEX].DefaultCellStyle.BackColor = columns[SOP_ELAPSED_TIME_TITLE_INDEX].DefaultCellStyle.BackColor = m_titleCellBackColor;
            //columns[SOP_NAME_TITLE_INDEX].DefaultCellStyle.ForeColor = columns[SOP_COMMANDER_TITLE_INDEX].DefaultCellStyle.ForeColor = columns[SOP_LOCATION_TITLE_INDEX].DefaultCellStyle.ForeColor = columns[SOP_START_TIME_TITLE_INDEX].DefaultCellStyle.ForeColor = columns[SOP_ELAPSED_TIME_TITLE_INDEX].DefaultCellStyle.ForeColor = Color.Black;
            columns[SOP_NAME_TITLE_INDEX].DefaultCellStyle.SelectionBackColor = columns[SOP_COMMANDER_TITLE_INDEX].DefaultCellStyle.SelectionBackColor = columns[SOP_LOCATION_TITLE_INDEX].DefaultCellStyle.SelectionBackColor = columns[SOP_START_TIME_TITLE_INDEX].DefaultCellStyle.SelectionBackColor = columns[SOP_ELAPSED_TIME_TITLE_INDEX].DefaultCellStyle.SelectionBackColor = m_titleCellBackColor;
            columns[SOP_NAME_INDEX].DefaultCellStyle.BackColor = columns[SOP_COMMANDER_INDEX].DefaultCellStyle.BackColor = columns[SOP_LOCATION_INDEX].DefaultCellStyle.BackColor = columns[SOP_START_TIME_INDEX].DefaultCellStyle.BackColor = columns[SOP_ELAPSED_TIME_INDEX].DefaultCellStyle.BackColor = m_bodyCellBackColor;
        }

        private void ReadWorkingTime()
        {
            string strSQL = "Select PropertyName, PropertyValue from OptionSOPSimulator where SiteID = " + FormMain.Instance.SiteID.ToString();
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nHour = 0, nMinute = 0;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i], "");
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 1], "");

                if (string.Compare(strPropertyName, "WorkingBeginHour", true) == 0)
                {
                    if (GetWoringTime(strPropertyValue, ref nHour, ref nMinute))
                    {
                        m_nWorkingBeginHour = nHour;
                        m_nWorkingBeginMinute = nMinute;
                    }
                }
                else if (string.Compare(strPropertyName, "WorkingEndHour", true) == 0)
                {
                    if (GetWoringTime(strPropertyValue, ref nHour, ref nMinute))
                    {
                        m_nWorkingEndHour = nHour;
                        m_nWorkingEndMinute = nMinute;
                    }
                }
            }
        }

        private bool GetWoringTime(string strValue, ref int nHour, ref int nMinute)
        {
            string[] arrTokens = strValue.Split(':');

            if (arrTokens.Count() != 2)
                return false;

            string strHour = arrTokens[0].Trim();
            string strMinute = arrTokens[1].Trim();

            if (int.TryParse(strHour, out nHour) && int.TryParse(strMinute, out nMinute))
            {
                if (nHour >= 0 && nHour < 24 && nMinute >= 0 && nMinute < 60)
                    return true;
            }

            return false;
        }

        // 평일 근무시간인가?
        private bool IsWorkingTime(DateTime time)
        {
            if (time.DayOfWeek == DayOfWeek.Saturday || time.DayOfWeek == DayOfWeek.Sunday)
                return false;

            if (time.Hour >= m_nWorkingBeginHour && time.Hour <= m_nWorkingEndHour)
            {
                if (time.Hour == m_nWorkingBeginHour && time.Minute < m_nWorkingBeginMinute)
                    return false;
                else if (time.Hour == m_nWorkingEndHour && time.Minute > m_nWorkingEndMinute)
                    return false;
                else
                    return true;
            }

            return true;
        }

        // 마지막에 사용하였던 Grid 크기를 얻어온다.
        private void ReadGridSize()
        {
            m_strConfigFilePath = Application.StartupPath + "\\bullet.ini";

            if (System.IO.File.Exists(m_strConfigFilePath))
            {
                Utility ini = new Utility();
                string[] strWidth = new string[6];

                strWidth[0] = ini.getinivalue("Grid3", "no", m_strConfigFilePath);
                strWidth[1] = ini.getinivalue("Grid3", "time", m_strConfigFilePath);
                strWidth[2] = ini.getinivalue("Grid3", "teamGroup", m_strConfigFilePath);
                strWidth[3] = ini.getinivalue("Grid3", "team", m_strConfigFilePath);
                strWidth[4] = ini.getinivalue("Grid3", "task", m_strConfigFilePath);
                strWidth[5] = ini.getinivalue("Grid3", "status", m_strConfigFilePath);

                for (int i = 0; i < 6; i++)
                {
                    int nWidth = WebDBManager.GetIntField(strWidth[i], -1);
                    if (nWidth < 0)
                        continue;

                    dataGridView3.Columns[i].Width = nWidth;
                }
            }

            if (dataGridView2.Visible == false)
            {
                dataGridView3.Location = dataGridView2.Location;
                dataGridView3.Size = new Size(dataGridView3.Size.Width, dataGridView3.Size.Height + dataGridView2.Size.Height);
            }
        }

        public void WriteGridSize(int nMonitor)
        {
            System.IO.StreamWriter writer = new System.IO.StreamWriter(m_strConfigFilePath);

            writer.WriteLine("[Grid3]");
            writer.WriteLine(string.Format("no={0}", dataGridView3.Columns[0].Width));
            writer.WriteLine(string.Format("time={0}", dataGridView3.Columns[1].Width));
            writer.WriteLine(string.Format("teamGroup={0}", dataGridView3.Columns[2].Width));
            writer.WriteLine(string.Format("team={0}", dataGridView3.Columns[3].Width));
            writer.WriteLine(string.Format("task={0}", dataGridView3.Columns[4].Width));
            writer.WriteLine(string.Format("status={0}", dataGridView3.Columns[5].Width));
            writer.WriteLine("");
            writer.WriteLine("[Monitor Info]");
            writer.WriteLine(string.Format("Bulletin={0}", nMonitor));
            writer.WriteLine("");
            writer.WriteLine("[Server Info]");
            writer.WriteLine(string.Format("siteid ={0}", FormMain.Instance.SiteID));
            writer.Close();
        }

        public void SetControlUserName(string strUserName)
        {
            if (dataGridView1.Rows.Count == 0)
                return;

            if ((string)dataGridView1.Rows[0].Cells[5].Value == strUserName)
                dataGridView1.Rows[0].Cells[5].Value = strUserName;
        }

        private void AddRowSOPState1()
        {
            /*DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            cell.Value = "SOP 이름";
            gridRow.Cells.Add(cell);

            DataGridViewComboBoxCell cellCombo = new DataGridViewComboBoxCell();
            //cell.Value = "야간터빈화재";
            AddComboBoxCellData(cellCombo);
            gridRow.Cells.Add(cellCombo);

            if (cellCombo.Items.Count > 0)
            {
                cellCombo.Value = cellCombo.Items[0];
                m_nSelectedSOPIndex = 0;
            }

            cell = new DataGridViewTextBoxCell();
            cell.Value = "진행총괄";
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            //cell.Value = "홍길동";
            cell.Value = FormMain.Instance.HistoryManager.ControlUserName;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "상황발생위치";
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            gridRow.Cells.Add(cell);

            gridRow.Height = 30;
            dataGridView1.Rows.Add(gridRow);*/
            DataGridViewRow gridRow = MakeNewRow(dataGridView1);

            foreach (DataGridViewCell cell in gridRow.Cells)
            {
                cell.ReadOnly = true;
            }

            gridRow.Cells[SOP_NAME_TITLE_INDEX].Value = "SOP 이름";
            SetSOPNameCell(gridRow);
            gridRow.Cells[SOP_COMMANDER_TITLE_INDEX].Value = "진행총괄";
            gridRow.Cells[SOP_COMMANDER_INDEX].Value = FormMain.Instance.HistoryManager.ControlUserName;
            gridRow.Cells[SOP_LOCATION_TITLE_INDEX].Value = "상황발생위치";
            gridRow.Cells[SOP_START_TIME_TITLE_INDEX].Value = "시작시간";
            gridRow.Cells[SOP_ELAPSED_TIME_TITLE_INDEX].Value = "경과시간";

            gridRow.Height = 30;            

            SetPositionText();

            if (m_nSelectedSOPIndex >= 0)
            {
                UpdateActionStepHistory((ActionStepHistoryData)m_arrActionStepHistory[m_nSelectedSOPIndex]);
                UpdateSOPState2();
            }
        }

        private void SetSOPNameCell(DataGridViewRow gridRow)
        {
            DataGridViewComboBoxCell cellCombo = (DataGridViewComboBoxCell)gridRow.Cells[SOP_NAME_INDEX];
            AddComboBoxCellData(cellCombo);
            cellCombo.ReadOnly = false;

            if (cellCombo.Items.Count > 0)
            {
                cellCombo.Value = cellCombo.Items[0];
                m_nSelectedSOPIndex = 0;
            }
        }

        public static DataGridViewRow MakeNewRow(DataGridView grid)
        {
            if (grid.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)grid.Rows[grid.Rows.Count - 1].Clone();
                grid.Rows.Add(row);

                return grid.Rows[grid.Rows.Count - 2];
            }
            else
            {
                grid.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)grid.Rows[grid.Rows.Count - 1].Clone();
                grid.Rows.Add(row);

                grid.AllowUserToAddRows = false;
            }

            return grid.Rows[grid.Rows.Count - 1];
        }

        private void CheckLockHistory()
        {
            while (FormMain.Instance.LockHistory)
            {
                System.Threading.Thread.Sleep(100);
            }
        }

        private void LockHistory()
        {
            FormMain.Instance.LockHistory = true;
        }

        private void UnLockHistory()
        {
            FormMain.Instance.LockHistory = false;
        }

        private void AddComboBoxCellData(DataGridViewComboBoxCell cell)
        {
            CheckLockHistory();
            LockHistory();

            m_arrActionStepHistory.Clear();
            m_arrSOPPositions.Clear();

            HistoryManager history = FormMain.Instance.HistoryManager;

            foreach (ActionStepHistoryData data in history.ActionStepHistoryList)
            {
                cell.Items.Add(data.ActionStepPath);
                m_arrSOPPositions.Add(data.Position);
                m_arrActionStepHistory.Add(data);
            }

            UnLockHistory();
        }

        public void ClearEndOrCancelActionStep()
        { 
            int nHistoryCount = m_arrActionStepHistory.Count;

            if (nHistoryCount == 0)
                return;

            m_arrActionStepHistory.Clear();

            DataGridViewComboBoxCell cellCombo = (DataGridViewComboBoxCell)dataGridView1.Rows[0].Cells[SOP_NAME_INDEX];

            string strValue = cellCombo.Value.ToString();
            int nSelectedIndex = cellCombo.Items.IndexOf(strValue);


            cellCombo.Items.Clear();
            m_nSelectedSOPIndex = -1;

            HistoryManager history = FormMain.Instance.HistoryManager;
            history.ActionStepHistoryList.Clear();            
        }

        public void UpdateActionSteps(ArrayList arrActionStepHistoryList)
        {
            int nHistoryCount = m_arrActionStepHistory.Count;

            if (nHistoryCount == 0)
                return;

            DataGridViewComboBoxCell cellCombo = (DataGridViewComboBoxCell)dataGridView1.Rows[0].Cells[SOP_NAME_INDEX];

            string strValue = cellCombo.Value.ToString();
            int nSelectedIndex = cellCombo.Items.IndexOf(strValue);

            ActionStepHistoryData currentData = nSelectedIndex < 0 ? null : (ActionStepHistoryData)m_arrActionStepHistory[nSelectedIndex];
            
            // 삭제된 ActionStep은 ComboBox에서 지우기
            ArrayList arrRemove = new ArrayList();

            for (int i=0;i<nHistoryCount;i++)
            {
                ActionStepHistoryData data = (ActionStepHistoryData)m_arrActionStepHistory[i];

                if (!arrActionStepHistoryList.Contains(data))
                    arrRemove.Add(i);

   
            }

            int nRemoveCount = arrRemove.Count;

            for (int i = nRemoveCount - 1; i >= 0; i--)
            {
                m_arrActionStepHistory.RemoveAt(i);
                m_arrSOPPositions.RemoveAt(i);
                cellCombo.Items.RemoveAt(i);
            }
            ////////////////////////////////////////////////////////

            // 새로운 ActionStep ComboBox에 추가
            foreach (ActionStepHistoryData data in arrActionStepHistoryList)
            {
                if (!m_arrActionStepHistory.Contains(data))
                {
                    m_arrActionStepHistory.Add(data);
                    m_arrSOPPositions.Add(data.Position);
                    cellCombo.Items.Add(data.ActionStepPath);
                }
            }
            ////////////////////////////////////////////////////////

            // ComboBox 표시값 설정
            nHistoryCount = m_arrActionStepHistory.Count;

            if (nHistoryCount > 0)
            {
                //ActionStepHistoryData lastData = (ActionStepHistoryData)m_arrActionStepHistory[nHistoryCount - 1];
                ActionStepHistoryData currentActionStepData = FormMain.Instance.HistoryManager.CurrentActionStepHistory;

                //if (lastData != currentData)
                if (currentActionStepData != currentData && currentActionStepData != null)
                {
                    m_nSelectedSOPIndex = m_arrActionStepHistory.IndexOf(currentActionStepData);
                    cellCombo.Value = cellCombo.Items[m_nSelectedSOPIndex];

                    SetPositionText();
                    UpdateSOPState2();
                }
                else
                {
                    m_nSelectedSOPIndex = cellCombo.Items.IndexOf(cellCombo.Value.ToString());
                }

                UpdateStartTime();
            }
            ////////////////////////////////////////////////////////
        }

        private void UpdateStartTime()
        {
            if (dataGridView2.Rows.Count < 2 || m_nSelectedSOPIndex < 0)
                return;

            /*if (dataGridView3.Tag == null)
                return;

            DateTime time = (DateTime)dataGridView3.Tag;*/
            ActionStepHistoryData data = (ActionStepHistoryData)m_arrActionStepHistory[m_nSelectedSOPIndex];
            TimeInfo timeBegin = data.BeginTime;

            if (timeBegin == null)
                return;

            DateTime time = timeBegin.m_time;

            string strStartTime = string.Format("{0}년 {1}월 {2}일 {3}시 {4}분",
                time.Year, time.Month, time.Day, time.Hour, time.Minute);

            dataGridView2.Rows[0].Cells[1].Value = strStartTime + " 상황발생";
            UpdateStartTime(strStartTime);

        }

        private string m_strSOPState = "";

        public void UpdateProcessedTime(TimeSpan span)
        {
            if (dataGridView2.Rows.Count < 2 || m_nSelectedSOPIndex < 0)
                return;


            string strTimeSpan = GetTimeSpan();

            //dataGridView2.Rows[1].Cells[1].Value = strTimeSpan;

            ActionStepHistoryData data = (ActionStepHistoryData)m_arrActionStepHistory[m_nSelectedSOPIndex];
            TimeInfo timeBegin = data.BeginTime;

            if (timeBegin == null)
                return;

            TimeInfo timeEnd = data.EndTime;
            TimeInfo timeCancel = data.CancelTime;

            if (timeEnd == null && timeCancel == null)
            {
                DateTime dtBegin = timeBegin.m_time;

                /*if (dataGridView3.Tag == null)
                    return;

                DateTime dtBegin = (DateTime)dataGridView3.Tag;*/
                DateTime dtCurrent = DateTime.Now + span;
                TimeSpan spanProcessed = dtCurrent - dtBegin;

                strTimeSpan = "현재 " + strTimeSpan + " 경과";
                m_strSOPState = "경과";

                UpdateElapsedTime(strTimeSpan);
            }
            else if (timeEnd != null)
            {
                //DateTime dtEnd = timeEnd.m_time;
                //dataGridView2.Rows[1].Cells[1].Value = string.Format("{0}년 {1}월 {2}일 {3}시 {4}분 상황종료",
                //    dtEnd.Year, dtEnd.Month, dtEnd.Day, dtEnd.Hour, dtEnd.Minute);

                strTimeSpan = "상황종료";
                m_strSOPState = "상황종료";

                ActionStepDetailLog log = FormMain.Instance.HistoryManager.GetActionStepHistory(data.ActionStepID);

                if (log != null)
                    log.EndTime = timeEnd;
            }
            else if (timeCancel != null)
            {
                //DateTime dtCancel = timeCancel.m_time;
                //dataGridView2.Rows[1].Cells[1].Value = string.Format("{0}년 {1}월 {2}일 {3}시 {4}분 상황취소",
                //    dtCancel.Year, dtCancel.Month, dtCancel.Day, dtCancel.Hour, dtCancel.Minute);

                strTimeSpan = "상황취소";
                m_strSOPState = "상황취소";

                ActionStepDetailLog log = FormMain.Instance.HistoryManager.GetActionStepHistory(data.ActionStepID);

                if (log != null)
                    log.CancelTime = timeCancel;
            }

            dataGridView2.Rows[1].Cells[1].Value = strTimeSpan;
        }

        private void UpdateSOPState2()
        {
            if (dataGridView2.Rows.Count < 2)
                return;

            if (m_nSelectedSOPIndex < 0)
                return;

            HistoryManager history = FormMain.Instance.HistoryManager;
            if (history.ActionStepHistoryList.Count <= m_nSelectedSOPIndex)
                return;

            ActionStepHistoryData data = (ActionStepHistoryData)history.ActionStepHistoryList[m_nSelectedSOPIndex];
            string strActionStepPath = data.ActionStepPath;

            int nIndex1 = strActionStepPath.IndexOf('/');
            if (nIndex1 < 0)
                return;

            int nIndex2 = strActionStepPath.IndexOf('/', nIndex1 + 1);
            if (nIndex2 < 0)
                return;

            int nIndex3 = strActionStepPath.IndexOf('/', nIndex2 + 1);
            if (nIndex3 < 0)
                return;

            dataGridView2.Rows[0].Cells[0].Value = "재난대응현황 : " + strActionStepPath.Substring(nIndex3 + 1);
            dataGridView2.Rows[1].Cells[0].Value = string.Format("재난명 : {0}", strActionStepPath.Substring(nIndex1 + 1, nIndex3 - (nIndex1 + 1)));
        }

        private void UpdateStartTime(string strStartTime)
        {
            if (dataGridView1.Rows.Count == 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[0];
            row.Cells[SOP_START_TIME_INDEX].Value = strStartTime;
        }

        private void UpdateElapsedTime(string strElapsedTime)
        {
            if (dataGridView1.Rows.Count == 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[0];
            row.Cells[SOP_ELAPSED_TIME_INDEX].Value = strElapsedTime;
        }

        private void AddRowSOPState2()
        {
            string strStartTime = "0000년 00월 00일 00시 00분";
            UpdateStartTime(strStartTime);

            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            //cell.Value = "재난대응현황 : 상황접수 완료(진행,대기,종료)";
            cell.Value = "";
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strStartTime + " 상황발생";
            gridRow.Cells.Add(cell);

            gridRow.Height = 30;
            dataGridView2.Rows.Add(gridRow);

            gridRow = new DataGridViewRow();
            cell = new DataGridViewTextBoxCell();
            cell.Value = FormMain.Instance.DefSOPName;//"<태풍-2저탄장 상하탄기 전도>";
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            //cell.Value = "현재 00:00:00경과";
            cell.Value = "";
            gridRow.Cells.Add(cell);

            gridRow.Height = 50;
            dataGridView2.Rows.Add(gridRow);

            UpdateSOPState2();
            UpdateStartTime();
        }

        public void SOPInfo1(string strSOP, string strDisaster, string strPosition, string strBuilding, string strProgress, string strName)
        {
            dataGridView1.Rows[0].Cells[0].Value = strSOP;
            dataGridView1.Rows[0].Cells[1].Value = strDisaster;
            dataGridView1.Rows[0].Cells[2].Value = strPosition;
            dataGridView1.Rows[0].Cells[3].Value = strBuilding;
            dataGridView1.Rows[0].Cells[4].Value = strProgress;
            dataGridView1.Rows[0].Cells[5].Value = strName;
        }

        public void SOPInfo2(string strState, string strDateTime, string strDisasterName, string strTime)
        {
            dataGridView2.Rows[0].Cells[0].Value = strState;
            dataGridView2.Rows[0].Cells[1].Value = strDateTime;
            dataGridView2.Rows[1].Cells[0].Value = strDisasterName;
            dataGridView2.Rows[1].Cells[1].Value = strTime;
        }

        public void SOPInfo3(DateTime dt, string strHeadquarters, string strResponsibility, string strTask, string strState)
        {
            DataGridViewRow gridRow = new DataGridViewRow();
            DataGridViewCell cell = new DataGridViewTextBoxCell();
            cell.Value = 1;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = dt.ToString();
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strHeadquarters;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strResponsibility;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strTask;
            gridRow.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = strState;
            gridRow.Cells.Add(cell);

            dataGridView3.Rows.Insert(0, gridRow);

            for (int i = 1; i < dataGridView3.RowCount + 1; i++ )
            {
                dataGridView3.Rows[i-1].Cells[0].Value = i.ToString();
            }
        }

        private void SetPositionText()
        {
            DataGridViewRow gridRow = dataGridView1.Rows[0];
            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)gridRow.Cells[SOP_LOCATION_INDEX];

            if (m_nSelectedSOPIndex < 0)
            {
                cell.Value = "";
            }
            else
            {
                cell.Value = (string)m_arrSOPPositions[m_nSelectedSOPIndex];
            }
        }

        private void dataGridView3_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex != -1)
                return;


            e.PaintBackground(e.ClipBounds, true);

            Rectangle r = e.CellBounds;

            r.Width -= 1;
            r.Height -= 1;

            DataGridViewColumn column = this.dataGridView3.Columns[e.ColumnIndex];

            using (SolidBrush brBk = new SolidBrush(column.HeaderCell.InheritedStyle.BackColor))
            using (SolidBrush brFr = new SolidBrush(column.HeaderCell.InheritedStyle.ForeColor))
            {
                e.Graphics.FillRectangle(brBk, r);

                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                r.Y += 2;
                e.Graphics.DrawString(column.HeaderText, e.CellStyle.Font, brFr, r, sf);
                sf.Dispose();
            }
            e.Handled = true;


            using (System.Drawing.Pen p = new System.Drawing.Pen(this.dataGridView3.GridColor))
            {
                e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Top,
                e.CellBounds.Right - 1, e.CellBounds.Top);
            }

            if (e.ColumnIndex >= 0)
            {
                using (System.Drawing.Pen p = new System.Drawing.Pen(this.dataGridView3.GridColor))
                {
                    e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Bottom - 1,
                        e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);

                    e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                        e.CellBounds.Right - 1, e.CellBounds.Bottom);
                }

                e.Handled = true;
            }

            if (e.ColumnIndex == 0)
            {
                using (System.Drawing.Pen p = new System.Drawing.Pen(this.dataGridView3.GridColor))
                {
                    e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Top,
                        e.CellBounds.Left, e.CellBounds.Bottom);
                }
            }
            else if (e.ColumnIndex == this.dataGridView3.Columns.Count - 1)
            {
                using (System.Drawing.Pen p = new System.Drawing.Pen(this.dataGridView3.GridColor))
                {
                    e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top,
                    e.CellBounds.Right - 1, e.CellBounds.Bottom);
                }
            }

        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {

        }

        public void UpdateActionStepHistory(ActionStepHistoryData data)
        {
            CheckLockHistory();
            LockHistory();

            int nIndex = GetActionStepHistoryIndex(data);

            if (nIndex < 0)
                NewHistory(data);
            else if (nIndex != m_nSelectedSOPIndex)
                ChangeHistory(nIndex, data);
            else
                UpdateHistory(data);

            UnLockHistory();
        }

        private int IndexOfHistory(ComponentHistoryData componentHistory)
        {
            int nRowCount = dataGridView3.Rows.Count;

            for (int i=0;i<nRowCount;i++)
            {
                DataGridViewRow row = dataGridView3.Rows[i];
                ComponentHistoryData data = (ComponentHistoryData)row.Tag;

                if (data != null && data.ComponentHistoryID == componentHistory.ComponentHistoryID)
                    return i;
            }

            return -1;
        }

        private int IndexOfHistory(ComponentHistoryData componentHistory, ArrayList arrComponentHistoryList)
        {
            int nCount = arrComponentHistoryList.Count;

            for (int i = 0; i < nCount; i++)
            {
                ComponentHistoryData data = (ComponentHistoryData)arrComponentHistoryList[i];
                if (!data.Visible)
                    continue;

                if (componentHistory.ComponentHistoryID == data.ComponentHistoryID)
                    return i;
            }

            return -1;
        }

        public void AddMessageRow(DateTime dtMessage, string strMessage, int nSOPGenUserID, ActionStepHistoryData data)
        {
            int nRowCount = dataGridView3.Rows.Count;

            ComponentHistoryData history = new ComponentHistoryData();
            history.ActionStepHistory = data;
            history.ComponentHistoryID = -1;
            history.HistoryType = ComponentHistoryData.ComponentHistoryType.MESSENGER_MESSAGE_TYPE;
            history.Section = null;
            history.Status = "-";
            history.Task = strMessage;
            history.Time = dtMessage;
            history.Visible = true;
            history.AccessedUserID = nSOPGenUserID;

            data.ComponentHistoryList.Add(history);
            UpdateHistory(data);
        }

        // GridView 데이터 업데이트
        private void UpdateHistory(ActionStepHistoryData data)
        {
            int nLastIndex = -1;
            //bool isFirst = true;

            foreach (ComponentHistoryData componentHistory in data.ComponentHistoryList)
            {
                if (!componentHistory.Visible)
                    continue;

                int nIndex = IndexOfHistory(componentHistory);

                if (nIndex < 0)
                {
                    nLastIndex++;
                    DataGridViewRow row = MakeHistoryRow(new DataGridViewRow(), componentHistory, nLastIndex);
                    dataGridView3.Rows.Insert(nLastIndex, row);

                    if (componentHistory != null && componentHistory.Section != null && componentHistory.Section.Section != null && componentHistory.Section.SectionType == SectionData.ComponentType.ENDPOINT)
                    {
                        Sections.SectionDataEndPoint dataEndPoint = (Sections.SectionDataEndPoint)componentHistory.Section.Section.Data;

                        if (dataEndPoint.IsBegin == false)
                        {
                            row.Cells[5].Value = "상황 종료";
                            row.Cells[5].Style.ForeColor = Color.Red;
                            data.FinishLog = true;
                        }
                    }
                }
                else
                {
                    nLastIndex = nIndex;

                    DataGridViewRow row = dataGridView3.Rows[nIndex];
                    ChangeHistoryRow(row, componentHistory, nIndex);
                }

                //if (isFirst)
                //{
                //    dataGridView3.Tag = componentHistory.Time;
                //    UpdateStartTime();
                //    isFirst = false;
                //}
            }

            int nRowCount = dataGridView3.Rows.Count;
            int nRowIndex = 1;

            ArrayList arrRemoveRows = new ArrayList();

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridView3.Rows[i];
                ComponentHistoryData componentHistory = (ComponentHistoryData)row.Tag;

                if (IndexOfHistory(componentHistory, data.ComponentHistoryList) >= 0)
                {
                    row.Cells[0].Value = (nRowIndex++).ToString();
                }
                else
                {
                    arrRemoveRows.Add(i);
                    //dataGridView3.Rows.Remove(row);
                }
            }

            int nRemoveCount = arrRemoveRows.Count;

            for (int i = nRemoveCount - 1; i >= 0; i--)
            {
                dataGridView3.Rows.RemoveAt((int)arrRemoveRows[i]);
            }

            nRowCount = dataGridView3.Rows.Count;

            if (nRowCount > 0)
                dataGridView3.CurrentCell = dataGridView3.Rows[nRowCount - 1].Cells[0];
        }

        private DataGridViewRow ChangeHistoryRow(DataGridViewRow row, ComponentHistoryData componentHistory, int nIndex)
        {
            row.Tag = componentHistory;
            
            row.Cells[0].Value = nIndex.ToString();
            row.Cells[1].Value = string.Format("{0:00}:{1:00}:{2:00}", componentHistory.Time.Hour, componentHistory.Time.Minute, componentHistory.Time.Second);

            if (componentHistory.Section != null)
            {
                string strCommanderName;
                Sections.SectionCommander commander = GetCommanderInfo(componentHistory.Section.Section, componentHistory.Time, componentHistory.AccessedUserID, out strCommanderName);

                if (commander != null)
                    row.Cells[2].Value = strCommanderName;
                else
                    row.Cells[2].Value = "-";
                //row.Cells[2].Value = componentHistory.Section.StepMember.TeamName;
                row.Cells[3].Value = GetTeamNameList(componentHistory.TeamList);
                string strItemDetails = GetSectionItemString(componentHistory.Section.Section);
                row.Cells[4].Value = componentHistory.Task + strItemDetails;
            }
            else
            {
                row.Cells[2].Value = "-";
                row.Cells[3].Value = "Message";
                row.Cells[4].Value = componentHistory.Task;
            }

            row.Cells[5].Value = componentHistory.Status;

            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[4];
            Size szPrefer = cell.PreferredSize;

            if (row.Height < szPrefer.Height)
                row.Height = szPrefer.Height;
            
            return row;
        }

        public string GetSectionItemString(Sections.Section section, int nCheckNotify1, int nCheckNotify2)
        {
            string strItems = "";
            Sections.Section.ComponentType sectionType = section.GetComponentType();

            if (sectionType == Sections.Section.ComponentType.PROCESS)
            {
                Sections.SectionDataProcess dataSection = (Sections.SectionDataProcess)section.Data;
                
                foreach (Sections.MissionItem data in dataSection.MissionItems)
                {
                    strItems += "\r\n- " + data.Mission;
                }
            }
            else if (sectionType == Sections.Section.ComponentType.TRANSMISSION)
            {
                Sections.SectionDataTransmission dataSection = (Sections.SectionDataTransmission)section.Data;

                int nBit = 1;
                if ((nCheckNotify1 & nBit) == nBit)
                    strItems += "\r\n- (내부상황전파) 팝업메시지 사용";
                
                nBit = 2;
                if ((nCheckNotify1 & nBit) == nBit)
                    strItems += "\r\n- (내부상황전파) 모바일메시지 사용";

                nBit = 2;
                if ((nCheckNotify1 & nBit) == nBit)
                    strItems += "\r\n- (내부상황전파) 사내방송 사용";

                int i = 3;

                if (dataSection.DataExternal.UseSMS)
                {
                    foreach (Sections.ExternalTeamData data in dataSection.DataExternal.SMSReceivers)
                    {
                        nBit = (1 << i);
                        if ((nCheckNotify1 & nBit) == nBit)
                            strItems += "\r\n- (외부상황전파) " + data.TeamName + " 메시지 전송";

                        if (++i == 16)
                            break;
                    }
                }

                int j = 0;
                if (dataSection.DataExternal.UseFax)
                {
                    foreach (Sections.ExternalTeamData data in dataSection.DataExternal.FaxReceivers)
                    {
                        nBit = 1 << j;
                        if ((nCheckNotify2 & nBit) == nBit)
                            strItems += "\r\n- (외부상황전파) " + data.TeamName + " 팩스 전송";

                        if (++j == 16)
                            break;
                    }
                }
            }
            else if (sectionType == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal dataSection = (Sections.SectionDataInternal)section.Data;

                int nBit = 1;
                /*if ((nCheckNotify1 & nBit) == nBit)
                    strItems += "\r\n- (내부상황전파) 팝업메시지";*/

                nBit = 2;
                if ((nCheckNotify1 & nBit) == nBit)
                {
                    //strItems += "\r\n- (내부상황전파) 모바일메시지";
                    strItems += "\r\n- 문자메시지";
                }

                nBit = 4;
                if ((nCheckNotify1 & nBit) == nBit)
                    strItems += "\r\n- 사내방송";
            }
            else if (sectionType == Sections.Section.ComponentType.EXTERNAL)
            {
                Sections.SectionDataExternal dataSection = (Sections.SectionDataExternal)section.Data;
                int i = 0;

                if (dataSection.UseSMS)
                {
                    foreach (Sections.ExternalTeamData data in dataSection.SMSReceivers)
                    {
                        int nBit = 1 << i;
                        if ((nCheckNotify1 & nBit) == nBit)
                            strItems += "\r\n- (외부상황전파) " + data.TeamName + " 메시지 전송";

                        if (++i == 16)
                            break;
                    }
                }

                int j = 0;
                if (dataSection.UseFax)
                {
                    foreach (Sections.ExternalTeamData data in dataSection.FaxReceivers)
                    {
                        int nBit = 1 << j;
                        if ((nCheckNotify2 & nBit) == nBit)
                            strItems += "\r\n- (외부상황전파) " + data.TeamName + " 팩스 전송";

                        if (++j == 16)
                            break;
                    }
                }
            }

            return strItems;
        }

        private string GetSectionItemString(Sections.Section section)
        {
            if (section == null)
                return "";

            int nCheckedNotify1 = 0, nCheckedNotify2 = 0;
            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
                IOManager.GetProcessCheckedNotify((Sections.SectionProcess)section, out nCheckedNotify1, out nCheckedNotify2);
            else if (type == Sections.Section.ComponentType.INTERNAL)
                IOManager.GetInternalCheckedNotify((Sections.SectionInternal)section, out nCheckedNotify1);
            else if (type == Sections.Section.ComponentType.EXTERNAL)
                IOManager.GetExternalCheckedNotify((Sections.SectionExternal)section, out nCheckedNotify1, out nCheckedNotify2);
            else if (type == Sections.Section.ComponentType.TRANSMISSION)
                IOManager.GetTransmissionCheckedNotify((Sections.SectionTransmission)section, out nCheckedNotify1, out nCheckedNotify2);

            return GetSectionItemString(section, nCheckedNotify1, nCheckedNotify2);
        }

        private DataGridViewRow MakeHistoryRow(DataGridViewRow row, ComponentHistoryData componentHistory, int nIndex)
        {
            row.Tag = componentHistory;
            row.Height = 30;

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nIndex.ToString();
            row.Cells.Add(cell);
            cell.ReadOnly = true;

            //cell.He

            cell = new DataGridViewTextBoxCell();
            cell.Value = string.Format("{0:00}:{1:00}:{2:00}", componentHistory.Time.Hour, componentHistory.Time.Minute, componentHistory.Time.Second);
            row.Cells.Add(cell);
            cell.ReadOnly = true;

            ComponentHistoryData.ComponentHistoryType type = componentHistory.HistoryType;

            cell = new DataGridViewTextBoxCell();

            if (type == ComponentHistoryData.ComponentHistoryType.SECTION_TYPE)
            {
                string strCommanderName;
                Sections.SectionCommander commander = GetCommanderInfo(componentHistory.Section.Section, componentHistory.Time, componentHistory.AccessedUserID, out strCommanderName);

                if (commander != null)
                    cell.Value = strCommanderName;
                else
                    cell.Value = "-";

                //cell.Value = componentHistory.Section.StepMember.TeamName;
            }
            else if (type == ComponentHistoryData.ComponentHistoryType.MESSENGER_MESSAGE_TYPE)
                cell.Value = "-";

            row.Cells.Add(cell);
            cell.ReadOnly = true;

            cell = new DataGridViewTextBoxCell();

            if (type == ComponentHistoryData.ComponentHistoryType.SECTION_TYPE)
                cell.Value = GetTeamNameList(componentHistory.TeamList);
            else
                cell.Value = "Message";

            row.Cells.Add(cell);
            cell.ReadOnly = true;

            cell = new DataGridViewTextBoxCell();
            string strItemDetails = GetSectionItemString(componentHistory.Section == null ? null : componentHistory.Section.Section);
            cell.Value = componentHistory.Task + strItemDetails;
            row.Cells.Add(cell);
            cell.ReadOnly = true;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // MultiLine Text Option
            cell.Style.WrapMode = DataGridViewTriState.True;
            Size szPrefer = cell.PreferredSize;

            if (row.Height < szPrefer.Height)
                row.Height = szPrefer.Height;

            cell = new DataGridViewTextBoxCell();
            cell.Value = componentHistory.Status;
            row.Cells.Add(cell);
            cell.ReadOnly = true;

            return row;
        }

        private Sections.SectionCommander GetCommanderInfo(Sections.Section section, DateTime time, int nAccessedUserID, out string strCommanderName)
        {
            strCommanderName = "-";
            Sections.SectionCommander commander = null;

            Sections.Section.ComponentType type = section.GetComponentType();

            if (type == Sections.Section.ComponentType.PROCESS)
            {
                Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;
                commander = data.Commander.Clone();
            }
            else if (type == Sections.Section.ComponentType.INTERNAL)
            {
                Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
                commander = data.Commander.Clone();

                if (data.UseBroadcast)
                    return commander;
            }

            if (commander != null)
            {
                strCommanderName = GetSectionCommanderName(commander, time, nAccessedUserID);
            }

            return commander;
        }

        private string GetSectionCommanderName(Sections.SectionCommander commander, DateTime time, int nAccessedUserID)
        {
            string strDisplayText = null;

            if (commander == null)
                return "-";

            Data_SOPGenUser user = FormMain.Instance.HistoryManager.GetSOPGenUser(nAccessedUserID);

            if (user == null)
                user = FormMain.Instance.HistoryManager.LoadSOPGenUser(nAccessedUserID);

            if (user == null)
                return "-";

            if (commander.DisplayText != null && commander.DisplayText.Length > 0)
                strDisplayText = commander.DisplayText;

            if (commander.Team == null)
            {
                bool isDayLight = IsWorkingTime(time);

                if (isDayLight)
                {
                    if (user.DayLightCommander != null)
                        return user.DayLightCommander.Name;
                    else if (user.NightCommander != null)
                        return user.NightCommander.Name;
                }
                else
                {
                    if (user.NightCommander != null)
                        return user.NightCommander.Name;
                    else if (user.DayLightCommander != null)
                        return user.DayLightCommander.Name;
                }
            }
            
            if (strDisplayText == null)
                return strDisplayText;

            return "-";
        }

        // ActionStepHistory Index 변경
        private void ChangeHistory(int nSelectedIndex, ActionStepHistoryData data)
        {
            int nRowCount = dataGridView3.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
                dataGridView3.Rows.RemoveAt(0);

            int nIndex = 1;
            //bool isFirst = true;

            foreach (ComponentHistoryData componentHistory in data.ComponentHistoryList)
            {
                if (!componentHistory.Visible)
                    continue;

                DataGridViewRow row = MakeHistoryRow(new DataGridViewRow(), componentHistory, nIndex++);
                dataGridView3.Rows.Add(row);

                /*if (isFirst)
                {
                    dataGridView3.Tag = componentHistory.Time;
                    UpdateStartTime();
                    isFirst = false;
                }*/
            }

            m_nSelectedSOPIndex = nSelectedIndex;

            //DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[0].Cells[1];
            //comboBoxCell.Value = m_nSelectedSOPIndex;
            //DataGridViewCell cell = dataGridView1.Rows[0].Cells[1];
            //cell.Value = m_nSelectedSOPIndex;

            SetPositionText();
            UpdateSOPState2();
        }

        // 새로운 ActionStepHistory
        private void NewHistory(ActionStepHistoryData data)
        {
            int nRowCount = dataGridView3.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
                dataGridView3.Rows.RemoveAt(0);

            int nIndex = 1;
            //bool isFirst = true;

            foreach (ComponentHistoryData componentHistory in data.ComponentHistoryList)
            {
                if (!componentHistory.Visible)
                    continue;

                DataGridViewRow row = MakeHistoryRow(new DataGridViewRow(), componentHistory, nIndex++);
                dataGridView3.Rows.Add(row);

                /*if (isFirst)
                {
                    dataGridView3.Tag = componentHistory.Time;
                    UpdateStartTime();
                    isFirst = false;
                }*/
            }

            m_arrActionStepHistory.Add(data);
            m_arrSOPPositions.Add(data.Position);
            m_nSelectedSOPIndex = m_arrActionStepHistory.Count - 1;

            DataGridViewComboBoxCell comboBoxCell = (DataGridViewComboBoxCell)dataGridView1.Rows[0].Cells[SOP_NAME_INDEX];
            comboBoxCell.Items.Add(data.ActionStepPath);
            comboBoxCell.Value = comboBoxCell.Items[m_nSelectedSOPIndex];

            UpdateStartTime();
            SetPositionText();
            UpdateSOPState2();
        }

        private string GetTeamNameList(ArrayList arrTeamNameList)
        {
            if (arrTeamNameList == null)
                return "-";

            string strTeamNameList = "";

            foreach (string strTeamName in arrTeamNameList)
            {
                if (strTeamNameList.Length == 0)
                    strTeamNameList = strTeamName;
                else
                    strTeamNameList += ", " + strTeamName;
            }

            if (strTeamNameList.Length == 0)
                return "-";

            return strTeamNameList;
        }

        private int GetActionStepHistoryIndex(ActionStepHistoryData data)
        {
            int nCount = m_arrActionStepHistory.Count;
            if (nCount < 0)
                return -1;

            for (int i = 0; i < nCount; i++)
            {
                ActionStepHistoryData actionStepHistory = (ActionStepHistoryData)m_arrActionStepHistory[i];

                if (actionStepHistory.ActionStepHistoryID == data.ActionStepHistoryID)
                    return i;
            }

            return -1;
        }

        public ActionStepHistoryData GetActionStepHistoryData(int nIndex)
        {
            int nHistoryCount = m_arrActionStepHistory.Count;

            if (nHistoryCount <= 0 || nIndex < 0 || nIndex >= nHistoryCount)
                return null;

            return (ActionStepHistoryData)m_arrActionStepHistory[nIndex];
        }

        public int SelectedSOPIndex
        {
            get { return m_nSelectedSOPIndex; }
            set { m_nSelectedSOPIndex = value; }
        }

        public ActionStepHistoryData CurrentActionStepHistory
        {
            get
            {
                if (m_nSelectedSOPIndex < 0)
                    return null;

                return (ActionStepHistoryData)m_arrActionStepHistory[m_nSelectedSOPIndex];
            }
        }

        private void DockingRealTime_KeyDown(object sender, KeyEventArgs e)
        {
            FormMain.Instance.OnKeyDown(sender, e);
        }

        private void dataGridView3_KeyDown(object sender, KeyEventArgs e)
        {
            FormMain.Instance.OnKeyDown(sender, e);
        }


        ////////////////////한글 파일 저장/////////////////////
        private void SaveHwp_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool isHwpSetup = false;
            isHwpSetup = GetRegistry();

            //한글 설치여부
            if (isHwpSetup == false)
            {
                MessageBox.Show("아래한글이 설치되지 않았습니다.");
                return;
            }

            string SavePath = GetHWPFilePath();

            if (SavePath == null)
                return;

            /*saveFileDialog1.Filter = "한글 문서 (*.hwp)|*.hwp";

            saveFileDialog1.FileName = "상황판_한글파일";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)*/
            {
                // 결과 파일을 초기화한다.
                ClearResultFile();

                //내용 txt에 저장
                SaveDataTxt();
                SaveAllDataTxt();

                //SavePath = saveFileDialog1.FileName;
                //SavePath = subGap(SavePath);
                /*SavePath = SavePath.Replace("\\", "/");
                SavePath = SavePath.Replace("/", "\\\\");*/

                string logoFileName = GetReportLogoFileName();

                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.Arguments = SavePath + " " + logoFileName + " " + FormMain2.Instance.SiteID; 
                info.CreateNoWindow = true;
                info.FileName = Application.StartupPath + "\\BulletinHwpEXE.exe";

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = info;

                process.Start();

                this.Cursor = Cursors.WaitCursor;
                int nCount = 0;
                bool bSuccess = true;
                
                // 30초 이상 경과하면 오류로 간주한다.
                while (process.HasExited == false)
                {
                    process.WaitForExit(500);
                    nCount++;

                    if (60 == nCount)
                    {
                        process.Kill();
                        //MessageBox.Show("오류 발생");
                        bSuccess = false;
                        break;
                    }

                    bSuccess = ReadResultFile();

                    // 한글문서 작성이 끝나면 강제로 Process를 종료시킨다.
                    if (bSuccess)
                    {
                        process.Kill();
                        break;
                    }
                }
                
                if (bSuccess)
                {
                    bSuccess = ReadResultFile();
                }

                if (bSuccess == true)
                {
                    RunHWP(SavePath);
                    //MessageBox.Show("저장되었습니다.");
                }
                else
                    MessageBox.Show("파일을 저장할 수 없습니다.");
                
                this.Cursor = Cursors.Default;

            }

        }

        private void RunHWP(string strFilePath)
        {
            System.Diagnostics.Process.Start("hwp.exe", strFilePath);
        }

        // 저장할 한글 파일의 경로
        private string GetHWPFilePath()
        {
            DateTime dtNow = DateTime.Now;
            string strNow = string.Format("{0}{1:00}{2:00}_{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            try
            {
                string strFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (strFolderPath != null && strFolderPath.Length > 0)
                    return strFolderPath + "\\상황판_" + strNow + ".hwp";
            }
            catch (Exception)
            {
            }

            string strSavePath = "";
            saveFileDialog1.Filter = "한글 문서 (*.hwp)|*.hwp";

            saveFileDialog1.FileName = "상황판_" + strNow;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                strSavePath = saveFileDialog1.FileName;
                strSavePath = subGap(strSavePath);
                return strSavePath;
            }

            return null;
        }

        // 결과를 읽어온다.
        private bool ReadResultFile()
        {
            return System.IO.File.Exists(Application.StartupPath + "\\report\\BulletinResult.txt");
            /*System.IO.StreamReader reader = new System.IO.StreamReader(Application.StartupPath + "\\BulletinResult.txt");
            int nData = reader.Read();
            reader.Close();

            return nData == 1;*/
        }

        // 결과 파일을 초기화한다.
        private void ClearResultFile()
        {
            System.IO.File.Delete(Application.StartupPath + "\\report\\BulletinResult.txt");
            /*System.IO.StreamWriter writer = new System.IO.StreamWriter(Application.StartupPath + "\\BulletinResult.txt");
            writer.Write(0);
            writer.Close();*/
        }

        //공백 제거
        private string subGap(string _str)
        {
            int num = 0;//중간 띄어쓰기 위치
            string tmp = _str;
            while (tmp.IndexOf(" ") > 0)
            {
                num = tmp.IndexOf(" ");
                string tmp1 = tmp.Substring(0, num);

                tmp1 += "_" + tmp.Substring(num + 1);
                tmp = tmp1;
            }
            return tmp;
        }

        private void SaveDataTxt()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\BulletHwpData.txt"))
            {
                //SOP명
                file.WriteLine(dataGridView1.Rows[0].Cells[1].Value);
                //상황발생위치
                file.WriteLine(dataGridView1.Rows[0].Cells[3].Value);
                //진행총괄
                //file.WriteLine(dataGridView1.Rows[0].Cells[5].Value);

                //재난대응현황
                string str = dataGridView2.Rows[0].Cells[0].Value.ToString();
                string[] arr = str.Split(':');
                if (arr[0] != "")
                    file.WriteLine(arr[1]);
                else
                    file.WriteLine("");

                //재난명
                string str2 = dataGridView2.Rows[1].Cells[0].Value.ToString();
                string[] arr2 = str2.Split(':');
                if (arr2[0] != "")
                    file.WriteLine(arr2[1]);
                else
                    file.WriteLine("");

                //상황발생시간
                file.WriteLine(dataGridView2.Rows[0].Cells[1].Value);


                //경과시간
                if (m_strSOPState == "경과")
                    file.WriteLine("SOP 진행중");
                else if (m_strSOPState == "상황종료")
                    file.WriteLine("SOP 종료");
                else if (m_strSOPState == "상황취소")
                    file.WriteLine("SOP 실행중 취소");

                string strSubTime = "";
                strSubTime = GetTimeSpan();

                file.WriteLine(strSubTime);
     
                //file.WriteLine(dataGridView2.Rows[1].Cells[1].Value);


                file.Close();
            }
        }

        private string GetTimeSpan()
        {
            string strSubTime = "";

            if (m_nSelectedSOPIndex == -1)
                return "";

            ActionStepHistoryData data = (ActionStepHistoryData)m_arrActionStepHistory[m_nSelectedSOPIndex];
            if (data == null)
                return "";


            TimeInfo timeBegin = data.BeginTime;
            if (timeBegin == null)
                return "";

            //상황발생시간
            DateTime time = timeBegin.m_time;
            //현재시간
            DateTime dTodaytime = DateTime.Now;

            TimeSpan tsTime = dTodaytime - time;

            if (tsTime.Days > 0)
            {
                strSubTime = tsTime.Days + "일 " + tsTime.Hours + "시간 " + tsTime.Minutes + "분 " + tsTime.Seconds + "초";
            }
            else
            {
                if (tsTime.Hours > 0)
                {
                    strSubTime = tsTime.Hours + "시간 " + tsTime.Minutes + "분 " + tsTime.Seconds + "초";
                }
                else
                {
                    if (tsTime.Minutes > 0)
                    {
                        strSubTime = tsTime.Minutes + "분 " + tsTime.Seconds + "초";
                    }
                    else
                    {
                        strSubTime = tsTime.Seconds + "초";
                    }
                }
            }

            return strSubTime;
        }

        private void SaveAllDataTxt()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\BulletHwpAllData.txt", false, System.Text.Encoding.UTF8))
            {
                for (int i = 0; i < dataGridView3.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView3.Rows[i].Cells.Count; j++)
                    {
                        if (j == 2 || j == 3)
                            continue;

                        string strValue = dataGridView3.Rows[i].Cells[j].Value.ToString();
                        strValue = strValue.Replace("\r", ((char)0x02).ToString());
                        strValue = strValue.Replace("\n", ((char)0x03).ToString());

                        file.WriteLine(strValue);
                        file.WriteLine("-----문단구분-----");
                    }
                }
            }
        }

        //보안모듈 등록
        private const string RegRoot = @"SoftWare\HNC\HwpCtrl\Modules";

        public void SetRegistry()
        {
            string FilePath = Application.StartupPath + @"\FilePathCheckerModule.dll";

            RegistryKey R = Registry.CurrentUser.OpenSubKey(RegRoot, true);
            if (R == null)
                R = Registry.CurrentUser.CreateSubKey(RegRoot);

            R.SetValue("FilePathCheckerModule", FilePath);

            R.Close();
        }

        public bool GetRegistry()
        {
            const string HwpRoot = @"Applications\Hwp.exe";

            RegistryKey R = Registry.ClassesRoot.OpenSubKey(HwpRoot);

            if (R == null)
            {
                string strPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
            
                if (strPath != null && strPath.Length > 0)
                {
                    strPath += "\\Programs";
                    return FindHWPFolder(strPath);
                }

                return false;
            }

            return true;
        }

        private bool FindHWPFolder(string strPath)
        {
            string[] files = System.IO.Directory.GetFiles(strPath, "*.lnk");

            if (files != null)
            {
                string strTarget = "한글";
                int nTargetLength = strTarget.Length;

                foreach (string strFile in files)
                {
                    int nIndex = strFile.LastIndexOf('\\');

                    if (nIndex < 0)
                        continue;

                    int nIndex2 = strFile.LastIndexOf('.');

                    if (nIndex2 < 0)
                        continue;

                    string strFileName = strFile.Substring(nIndex + 1, nIndex2 - nIndex - 1);

                    if (strFileName.StartsWith(strTarget))
                    {
                        string str = strFileName.Substring(nTargetLength).Trim();

                        if (IsHWP(str))
                        {
                            return true;
                        }
                    }
                }
            }

            string[] folders = System.IO.Directory.GetDirectories(strPath);

            if (folders != null)
            {
                foreach (string strFolder in folders)
                {
                    int nIndex = strFolder.LastIndexOf('\\');

                    if (nIndex < 0)
                        continue;

                    string strFolderName = strFolder.Substring(nIndex + 1);

                    if (strFolderName == "한글과컴퓨터")
                    {
                        if (FindHWPFolder(strFolder))
                            return true;
                    }
                }
            }

            return false;
        }

        private bool IsHWP(string str)
        {
            int nVer;

            if (int.TryParse(str, out nVer))
            {
                if (nVer > 2000)
                    return true;
            }

            return false;
        }

        private void DockingRealTime_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (m_nSelectedSOPIndex == -1)
                    contextMenuStrip1.Items[0].Enabled = false;
                else
                    contextMenuStrip1.Items[0].Enabled = true;
            }
        }


            


        private void windowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (FormMain.Instance.WindowState == FormWindowState.Maximized)
                FormMain.Instance.ToNormalWindow();
            else
                FormMain.Instance.ToFullWindow();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (FormMain.Instance.WindowState == FormWindowState.Maximized)
                windowToolStripMenuItem.Text = "작은 화면으로 전환";
            else
                windowToolStripMenuItem.Text = "전체 화면으로 전환";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormMain.Instance.Close();
        }

        private void tsMenuDataList_Click(object sender, EventArgs e)
        {
            m_frmDataList.ClearData();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value == null)
                    continue;

                string strNo = row.Cells[0].Value.ToString();

                int no;

                if (!int.TryParse(strNo, out no))
                    continue;

                m_frmDataList.UpdateData(no, row.Visible);
            }
            
            m_frmDataList.ShowList();
        }

        

        private void tsMenuInitialize_Click(object sender, EventArgs e)
        {
            FormMain.Instance.StopTimer();
            System.Threading.Thread.Sleep(100);
            ClearEndOrCancelActionStep();

            FormMain.Instance.ProgressClear();

            dataGridView1.ClearSelection();
            dataGridView1.Rows.Clear();
            AddRowSOPState1();
           
            dataGridView2.ClearSelection();
            dataGridView2.Rows.Clear();
            AddRowSOPState2();

            dataGridView3.ClearSelection();
            dataGridView3.Rows.Clear();

            FormMain.Instance.ResumeTimer();

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == SOP_NAME_INDEX)
            {
                dataGridView1.BeginEdit(true);

                ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;

                if (comboBox.Tag == null)
                {
                    comboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
                }

                comboBox.Tag = true;
            }
        }

        void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;

            if (cbo.SelectedIndex == m_nSelectedSOPIndex)
                return;

            m_nSelectedSOPIndex = cbo.SelectedIndex;
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //if (e.RowIndex == 0 || e.ColumnIndex == 0)
            {
                //Brush b = new SolidBrush(Color.Black);
                //e.Graphics.FillRectangle(b, e.CellBounds);
                //b.Dispose();

                CellBorderLine borderLine;

                if (!m_dicCellBorderLine.TryGetValue(e.ColumnIndex, out borderLine))
                    return;

                int nLeftThick = borderLine.DrawLeft ? m_dataGridView1CellBorderLineThick : 0;
                int nTopThick = borderLine.DrawTop ? m_dataGridView1CellBorderLineThick : 0;
                int nRightThick = borderLine.DrawRight ? m_dataGridView1CellBorderLineThick : 0;
                int nBottomThick = borderLine.DrawBottom ? m_dataGridView1CellBorderLineThick : 0;

                e.Paint(e.CellBounds, DataGridViewPaintParts.All | DataGridViewPaintParts.ContentBackground);
                ControlPaint.DrawBorder(e.Graphics, e.CellBounds, m_dataGridView1CellBorderLineColor, nLeftThick, ButtonBorderStyle.Outset,
                    m_dataGridView1CellBorderLineColor, nTopThick, ButtonBorderStyle.Outset, m_dataGridView1CellBorderLineColor, nRightThick,
                    ButtonBorderStyle.Inset, m_dataGridView1CellBorderLineColor, nBottomThick, ButtonBorderStyle.Inset);
                
                /*e.Paint(e.CellBounds, DataGridViewPaintParts.All | DataGridViewPaintParts.ContentBackground);
                ControlPaint.DrawBorder(e.Graphics, e.CellBounds, Color.Black, 0, ButtonBorderStyle.Outset,
                    Color.Black, 0, ButtonBorderStyle.Outset, Color.Black, 1,
                    ButtonBorderStyle.Outset, Color.Black, 1, ButtonBorderStyle.Outset);*/
                e.Handled = true;
            }
        }

        private string GetReportLogoFileName()
        {
            string strSQL = "Select PropertyValue from OptionSdms where PropertyName='LogoFileName' and SiteID=" + FormMain2.Instance.SiteID;
            System.Collections.ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0) return string.Empty;

            string logoName = DBUtility.WebDBManager.GetStringField(arrResult[0].ToString(), string.Empty);

            return logoName;
        }
    }

    class CellBorderLine
    {
        private bool m_drawLeft = true;
        private bool m_drawTop = true;
        private bool m_drawRight = true;
        private bool m_drawBottom = true;

        public bool DrawLeft
        {
            get { return m_drawLeft; }
            set { m_drawLeft = value; }
        }

        public bool DrawTop
        {
            get { return m_drawTop; }
            set { m_drawTop = value; }
        }

        public bool DrawRight
        {
            get { return m_drawRight; }
            set { m_drawRight = value; }
        }

        public bool DrawBottom
        {
            get { return m_drawBottom; }
            set { m_drawBottom = value; }
        }

        public CellBorderLine()
        {
        }

        public CellBorderLine(bool drawLeft, bool drawTop, bool drawRight, bool drawBottom)
        {
            m_drawLeft = drawLeft;
            m_drawTop = drawTop;
            m_drawRight = drawRight;
            m_drawBottom = drawBottom;
        } 
    }
}
