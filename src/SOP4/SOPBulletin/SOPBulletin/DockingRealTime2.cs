using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SOPBulletin
{
    public partial class DockingRealTime2 : Form, UnE.Controls.IMergedDataGridViewOwner
    {
        // gridActionStepInfo Column Index
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

        // gridLog Column Index
        private const int LOG_NO_INDEX = 0;
        private const int LOG_TIME_INDEX = 1;
        private const int LOG_COMMANDER_INDEX = 2;
        private const int LOG_RECEIVER_INDEX = 3;
        private const int LOG_TASK_INDEX = 4;
        private const int LOG_STATUS_INDEX = 5;

        private List<ActionStepHistory> m_actionStepHistories = new List<ActionStepHistory>();
        private ComboBox m_cboActionStepHistory = null;
        //private DataGridViewComboBoxCell m_cellActionStepHistory = null;
        private ActionStepHistory m_currentActionStepHistory = null;
        private StringFormat m_cboFormat = new StringFormat();

        private Font m_fontDetail = new Font("맑은고딕", 12.0f);
        private int m_nDetailRowHeight = 40;

        private Color m_cellBorderLineColor = Color.FromArgb(127, 127, 127);
        private int m_cellBorderLineThick = 2;
        private Pen m_penBorderLine = null;

        private Color m_boxLineColor = Color.FromArgb(99, 37, 35);
        private int m_boxLineThick = 5;
        private Pen m_penBoxLine = null;

        private Image m_imgDetailMark = null;

        private int m_nIndentSize = 40;
        private float m_fLineSpacing = 10.0f;
        private TextLineSpacing.TextLineSpaceRenderer m_textRenderer = new TextLineSpacing.TextLineSpaceRenderer();

        // 사용자가 임의로 특정 ActionStep을 보기 위하여 ActionStep ComboBox의 Item을 바꾼 경우에는
        // DB를 통해 읽은 현재 ActionStep 정보를 무시하도록 한다.
        private bool m_ignoreAutoChangeActionStep = false;

        private string m_strHWPPath = null;

        public object LockActionStepInfo
        {
            get { return gridActionStepInfo; }
        }

        public object LockLog
        {
            get { return gridLog; }
        }

        public List<ActionStepHistory> ActionStepHistories
        {
            get { return m_actionStepHistories; }
        }

        public ActionStepHistory CurrentActionStepHistory
        {
            get { return m_currentActionStepHistory; }
        }

        public DockingRealTime2()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
        }

        public void AddComponentHistory(ActionStepHistory actionStepHistory, ComponentHistory componentHistory)
        {
            ComponentHistory oldHistory = FindComponentHistory(componentHistory.SectionState, actionStepHistory);

            lock (LockLog)
            {
                if (oldHistory != null)
                {
                    if (componentHistory.IsDetailLog)
                    {
                        AddDetailHistory(actionStepHistory, oldHistory, componentHistory);
                    }
                    else
                    {
                        DataGridViewRow row = UpdateComponentHistory(actionStepHistory, oldHistory, componentHistory);

                        if (row != null)
                            gridLog.CurrentCell = row.Cells[0];
                    }
                }
                else
                {
                    actionStepHistory.ComponentHistories.Add(componentHistory);

                    if (actionStepHistory == m_currentActionStepHistory)
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
            if (historyDetail.Type == lastHistoryType)
                return false;
            /*// 마지막 상태가 [임무확인]일 경우 다시 [임무확인] 로그가 들어오면 무시한다.
            if (historyDetail.Type == ComponentHistory.HistoryType.CONFIRM_MISSION &&
                GetLastNotDetailLogHistoryType(historyParent) == ComponentHistory.HistoryType.CONFIRM_MISSION)
                return false;*/

            AddComponentHistoryDetail(actionStepHistory, historyParent, historyDetail);
            return true;
        }

        private void AddComponentHistoryDetail(ActionStepHistory actionStepHistory, ComponentHistory historyParent, ComponentHistory historyDetail)
        {
            DataGridViewRow row = FindDataGridViewRow(historyParent);

            if (row == null)
            {
                if (actionStepHistory != m_currentActionStepHistory)
                {
                    historyParent.AllHistories.Add(historyDetail);
                }
                return;
            }
            else
            {
                historyParent.AllHistories.Add(historyDetail);
            }

            if (IsShowingDetails(row))
            {
                int nDetailHistoryCount = historyParent.AllHistories.Count;
                int nIndex = row.Index + nDetailHistoryCount - 1;
                gridLog.Rows.Insert(nIndex, 1);

                UpdateComponentHistory(gridLog.Rows[nIndex], historyDetail);
                ProcessDetailRow(gridLog.Rows[nIndex], ((NoString)row.Cells[LOG_NO_INDEX].Value).HeadNumber.Data, nDetailHistoryCount);
                gridLog.CurrentCell = gridLog.Rows[nIndex].Cells[0];
            }
        }

        private DataGridViewRow FindDataGridViewRow(ComponentHistory history)
        {
            foreach (DataGridViewRow row in gridLog.Rows)
            {
                if (row.Tag != null && (row.Tag is ComponentHistory) && (ComponentHistory)row.Tag == history)
                    return row;
            }

            return null;
        }

        private ComponentHistory.HistoryType GetLastNotDetailLogHistoryType(ComponentHistory componentHistory)
        {
            int nCount = componentHistory.AllHistories.Count;

            for (int i=nCount-1;i>=0;i--)
            {
                ComponentHistory history = componentHistory.AllHistories[i];

                if (history.IsDetailLog)
                    continue;

                return history.Type;
            }

            return ComponentHistory.HistoryType.NONE;
        }

        private DataGridViewRow UpdateComponentHistory(ActionStepHistory actionStepHistory, ComponentHistory oldHistory, ComponentHistory newHistory)
        {
            if (oldHistory == null || newHistory == null)
                return null;

            foreach (DataGridViewRow row in gridLog.Rows)
            {
                if (row.Tag != null && row.Tag is ComponentHistory)
                {
                    if ((ComponentHistory)row.Tag == oldHistory)
                    {
                        UpdateComponentHistory(actionStepHistory, oldHistory, newHistory, row);
                        return row;
                    }
                }
            }

            if (actionStepHistory == m_currentActionStepHistory)
            {
                DataGridViewRow row2 = DockingRealTime.MakeNewRow(gridLog);
                UpdateComponentHistory(actionStepHistory, oldHistory, newHistory, row2);

                return row2;
            }
            else
            {
                UpdateComponentHistory(actionStepHistory, oldHistory, newHistory, null);

                return null;
            }
        }

        private void UpdateComponentHistory(ActionStepHistory actionStepHistory, ComponentHistory oldHistory, ComponentHistory newHistory, DataGridViewRow row)
        {
            oldHistory.Time = newHistory.Time;
            oldHistory.Commander = newHistory.Commander;
            oldHistory.Receiver = newHistory.Receiver;
            oldHistory.Task = newHistory.Task;
            oldHistory.Type = newHistory.Type;

            AddDetailHistory(actionStepHistory, oldHistory, newHistory);

            if (row != null)
                UpdateComponentHistory(row, oldHistory);

        }

        public void AddActionStepHistory(ActionStepHistory actionStepHistory)
        {
            lock (LockActionStepInfo)
            {
                if (gridActionStepInfo.Rows.Count == 0)
                    return;

                AddActionStepHistoryToComboBox(/*m_cellActionStepHistory, */actionStepHistory);
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
                history.SectionState = HistoryManager2.MakeEndPointSectionState(false);

                AddComponentHistory(actionStepHistory, history);
            }

            FormMain2.Instance.SetMenuButtonEnables();
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
                history.SectionState = HistoryManager2.MakeEndPointSectionState(false);

                AddComponentHistory(actionStepHistory, history);
            }

            FormMain2.Instance.SetMenuButtonEnables();
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

        private void AddComponentHistory(ComponentHistory componentHistory)
        {
            if (componentHistory == null)
                return;

            DataGridViewRow row = DockingRealTime.MakeNewRow(gridLog);
            UpdateComponentHistory(row, componentHistory);
            gridLog.CurrentCell = row.Cells[0];
        }


        private object UpdateComponentHistoryCommanderName(ComponentHistory componentHistory)
        {
            List<ComponentHistory> arHistorys = new List<ComponentHistory>(componentHistory.AllHistories);
            foreach (ComponentHistory history in arHistorys)
            {
                if( history.Commander != null)
                {
                    return history.Commander;
                }
            }
            return null;
        }

        private void UpdateComponentHistory(DataGridViewRow row, ComponentHistory componentHistory, string strTaskAdd = "")
        {
            row.Cells[LOG_NO_INDEX].Value = new NoString(row.Index + 1);//row.Index + 1;
            row.Cells[LOG_TIME_INDEX].Value = MakeLogTimeString(componentHistory.Time);
            //row.Cells[LOG_COMMANDER_INDEX].Value = componentHistory.Commander;
            row.Cells[LOG_RECEIVER_INDEX].Value = componentHistory.Receiver;
            row.Cells[LOG_TASK_INDEX].Value = strTaskAdd + componentHistory.Task;
            row.Cells[LOG_STATUS_INDEX].Value = ComponentHistory.ToHistoryTypeString(componentHistory.Type);

            if (componentHistory.Commander == null)
            {
                object szCommanderName = gridActionStepInfo.Rows[0].Cells[SOP_COMMANDER_INDEX].Value;
                if( szCommanderName == null || szCommanderName.ToString() == "")
                {
                    szCommanderName = UpdateComponentHistoryCommanderName(componentHistory);
                }
                row.Cells[LOG_COMMANDER_INDEX].Value = szCommanderName;
            }
            else
                row.Cells[LOG_COMMANDER_INDEX].Value = componentHistory.Commander;

            Color backColor = (row.Index + 1) % 2 == 0 ? FormMain2.Instance.ColorStyle.EvenRowColor : FormMain2.Instance.ColorStyle.OddRowColor;

            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.Style.BackColor = backColor;
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            row.Cells[LOG_TASK_INDEX].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            row.Tag = componentHistory;
        }

        private string MakeLogTimeString(DateTime time)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", time.Hour, time.Minute, time.Second);
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

        private void AddActionStepHistoryToComboBox(/*DataGridViewComboBoxCell cell, */ActionStepHistory actionStepHistory)
        {
            // DataGridViewComboBoxCell에 string 이외의 것을 넣으면 에러남
            // 그래서, 대신 m_actionStepHistories에 데이터 저장
            m_actionStepHistories.Add(actionStepHistory);

            m_cboActionStepHistory.Items.Add(actionStepHistory);

            int nLastIdx = m_cboActionStepHistory.Items.Count - 1;
            if (m_cboActionStepHistory.Items.Count != 0)
            {
                m_cboActionStepHistory.SelectedIndex = nLastIdx;
                ChangeActionStepHistory(actionStepHistory);
            }
            /*colSOPFullPath.Items.Add(actionStepHistory);
            
            int nLastIdx = colSOPFullPath.Items.Count - 1;
            if (colSOPFullPath.Items.Count != 0)
            {
                cell.Value = actionStepHistory;// cell.Items[0];
                ChangeActionStepHistory(actionStepHistory);
            }*/           
        }

        private void DockingRealTime2_Load(object sender, EventArgs e)
        {
            m_penBorderLine = new Pen(m_cellBorderLineColor);
            m_penBorderLine.Width = m_cellBorderLineThick;
            m_imgDetailMark = global::SOPBulletin.Properties.Resources.red_circle;

            m_penBoxLine = new Pen(m_boxLineColor);
            m_penBoxLine.Width = m_boxLineThick;

            gridActionStepInfo.Owner = this;
            gridActionStepInfo.DataError += DataGridView_DataError;
            gridLog.Owner = this;
            gridLog.DataError += DataGridView_DataError;

            gridActionStepInfo.CellValidating += gridActionStepInfo_CellValidating;

            DataGridViewRow row = DockingRealTime.MakeNewRow(gridActionStepInfo);

            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.ReadOnly = true;
            }

            row.Cells[SOP_NAME_TITLE_INDEX].Value = "SOP 이름";
            row.Cells[SOP_COMMANDER_TITLE_INDEX].Value = "진행총괄";
            row.Cells[SOP_LOCATION_TITLE_INDEX].Value = "상황발생위치";
            row.Cells[SOP_START_TIME_TITLE_INDEX].Value = "시작시간";
            row.Cells[SOP_ELAPSED_TIME_TITLE_INDEX].Value = "경과시간";


            //m_bSource.DataSource = m_arCmbData;

            /*m_cellActionStepHistory = (DataGridViewComboBoxCell)row.Cells[SOP_NAME_INDEX];
            //m_cellActionStepHistory.DataSource = m_bSource;
            //m_cellActionStepHistory.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            m_cellActionStepHistory.ValueType = typeof(ActionStepHistory);
            m_cellActionStepHistory.ReadOnly = false;*/

           

            InitComboBoxCell();
            UpdateColorStyle();
        }

        private void gridActionStepInfo_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == SOP_NAME_INDEX && e.RowIndex == 0)
            {
                // SOP_NAME_INDEX Cell의 값은 ActionStepHistory 타입이어야 함.
                if (e.FormattedValue.GetType() == typeof(string))
                {
                    e.Cancel = true;
                }
            }
        }

        private void UpdateColorStyle()
        {
            ColorStyle style = FormMain2.Instance.ColorStyle;

            gridActionStepInfo.Rows[0].Cells[SOP_NAME_TITLE_INDEX].Style.BackColor = style.ActionStepTitleBackColor;
            gridActionStepInfo.Rows[0].Cells[SOP_COMMANDER_TITLE_INDEX].Style.BackColor = style.ActionStepTitleBackColor;
            gridActionStepInfo.Rows[0].Cells[SOP_LOCATION_TITLE_INDEX].Style.BackColor = style.ActionStepTitleBackColor;
            gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_TITLE_INDEX].Style.BackColor = style.ActionStepTitleBackColor;
            gridActionStepInfo.Rows[0].Cells[SOP_ELAPSED_TIME_TITLE_INDEX].Style.BackColor = style.ActionStepTitleBackColor;
            gridActionStepInfo.Rows[0].Cells[SOP_NAME_TITLE_INDEX].Style.ForeColor = style.ActionStepTitleForeColor;
            gridActionStepInfo.Rows[0].Cells[SOP_COMMANDER_TITLE_INDEX].Style.ForeColor = style.ActionStepTitleForeColor;
            gridActionStepInfo.Rows[0].Cells[SOP_LOCATION_TITLE_INDEX].Style.ForeColor = style.ActionStepTitleForeColor;
            gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_TITLE_INDEX].Style.ForeColor = style.ActionStepTitleForeColor;
            gridActionStepInfo.Rows[0].Cells[SOP_ELAPSED_TIME_TITLE_INDEX].Style.ForeColor = style.ActionStepTitleForeColor;

            /*gridActionStepInfo.Rows[0].Cells[SOP_NAME_INDEX].Style.BackColor = style.ActionStepBodyBackColor;
            gridActionStepInfo.Rows[0].Cells[SOP_COMMANDER_INDEX].Style.BackColor = style.ActionStepBodyBackColor;
            gridActionStepInfo.Rows[0].Cells[SOP_LOCATION_INDEX].Style.BackColor = style.ActionStepBodyBackColor;
            gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_INDEX].Style.BackColor = style.ActionStepBodyBackColor;
            gridActionStepInfo.Rows[0].Cells[SOP_ELAPSED_TIME_INDEX].Style.BackColor = style.ActionStepBodyBackColor;
            gridActionStepInfo.Rows[0].Cells[SOP_NAME_INDEX].Style.ForeColor = style.ActionStepBodyForeColor;
            gridActionStepInfo.Rows[0].Cells[SOP_COMMANDER_INDEX].Style.ForeColor = style.ActionStepBodyForeColor;
            gridActionStepInfo.Rows[0].Cells[SOP_LOCATION_INDEX].Style.ForeColor = style.ActionStepBodyForeColor;
            gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_INDEX].Style.ForeColor = style.ActionStepBodyForeColor;
            gridActionStepInfo.Rows[0].Cells[SOP_ELAPSED_TIME_INDEX].Style.ForeColor = style.ActionStepBodyForeColor;*/

            foreach (DataGridViewColumn column in gridLog.Columns)
            {
                column.HeaderCell.Style.BackColor = style.LogColumnBackColor;
                column.HeaderCell.Style.ForeColor = style.LogColumnForeColor;
            }

            //forea
        }

        private void DataGridView_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {

            //MessageBox.Show("Error happened " + anError.Context.ToString());
            //if (anError.Context == DataGridViewDataErrorContexts.Formatting)
            //{
            //    MessageBox.Show("FormattedValue error");
            //}

            //if (anError.Context == DataGridViewDataErrorContexts.Commit)
            //{
            //    MessageBox.Show("Commit error");
            //}
            //if (anError.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            //{
            //    MessageBox.Show("Cell change");
            //}
            //if (anError.Context == DataGridViewDataErrorContexts.Parsing)
            //{
            //    MessageBox.Show("parsing error");
            //}
            //if (anError.Context == DataGridViewDataErrorContexts.LeaveControl)
            //{
            //    MessageBox.Show("leave control error");
            //}

            if ((anError.Exception) is ConstraintException)
            {
                DataGridView view = (DataGridView)sender;
                view.Rows[anError.RowIndex].ErrorText = "an error";
                view.Rows[anError.RowIndex].Cells[anError.ColumnIndex].ErrorText = "an error";

                anError.ThrowException = false;
            }
        }


        // ComboBox에 대한 EventHandler 정의
        private void InitComboBoxCell()
        {
            //m_cellActionStepHistory.Selected = true;

            //gridActionStepInfo.BeginEdit(true);

            m_cboActionStepHistory = new ComboBox();
            gridActionStepInfo.Controls.Add(m_cboActionStepHistory);

            Point location;
            Size cellSize = GetComboBoxCellSize(out location);

            m_cboActionStepHistory.Size = cellSize;
            m_cboActionStepHistory.Location = location;
            m_cboActionStepHistory.DropDownStyle = ComboBoxStyle.DropDownList;
            m_cboActionStepHistory.Font = gridActionStepInfo.DefaultCellStyle.Font;
            m_cboActionStepHistory.ItemHeight = cellSize.Height;
            m_cboActionStepHistory.DrawMode = DrawMode.OwnerDrawVariable;

            m_cboFormat.LineAlignment = StringAlignment.Center;

            //m_cboActionStepHistory = (ComboBox)gridActionStepInfo.EditingControl;
            //m_cboActionStepHistory.CausesValidation = false;
            
            if (m_cboActionStepHistory.Tag == null)
            {
                m_cboActionStepHistory.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
                m_cboActionStepHistory.DrawItem += new System.Windows.Forms.DrawItemEventHandler(comboBox_DrawItem);
                m_cboActionStepHistory.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(comboBox_MeasureItem);
            }

            //gridActionStepInfo.EndEdit();
            gridActionStepInfo.ClearSelection();
        }

        private void comboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                Brush brush = Brushes.Black;

                Font ft = m_cboActionStepHistory.Font;
                e.Graphics.DrawString(m_cboActionStepHistory.Items[e.Index].ToString(), ft, brush, e.Bounds, m_cboFormat);
                e.DrawFocusRectangle();
            }
        }

        private void comboBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 30;
        }

        private Size GetComboBoxCellSize(out Point location)
        {
            Rectangle rect = gridActionStepInfo.GetCellDisplayRectangle(SOP_NAME_INDEX, 0, false);
            location = new Point(rect.Left, rect.Top);
            return rect.Size;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            int nIdx = cbo.SelectedIndex;
            if (nIdx < 0 || nIdx >= m_actionStepHistories.Count)
            {
                //SendKeys.Send("{ESC}");
                return;
            }

            ActionStepHistory actionStepHistory = m_actionStepHistories[nIdx];
            if (m_currentActionStepHistory != actionStepHistory)
            {
                lock (LockActionStepInfo)
                {
                    // 사용자가 임의로 특정 ActionStep을 보기 위하여 ActionStep ComboBox의 Item을 바꾼 경우에는
                    // DB를 통해 읽은 현재 ActionStep 정보를 무시하도록 한다.
                    m_ignoreAutoChangeActionStep = true;
                    ChangeActionStepHistory(actionStepHistory);
                }
                SendKeys.Send("{ESC}");
                m_bShowCmb = false;
            }
            else
            {
                if (m_bShowCmb == true)
                {
                    SendKeys.Send("{ESC}");
                    m_bShowCmb = false;
                }               
            }
        }

        private void ChangeActionStepHistory(ActionStepHistory actionStepHistory)
        {
            m_currentActionStepHistory = actionStepHistory;

            if (actionStepHistory != null)
            {
                //UpdateCommanderName(actionStepHistory.CommanderName);
                gridActionStepInfo.Rows[0].Cells[SOP_NAME_INDEX].Value = actionStepHistory;//.ActionStepPath;
                gridActionStepInfo.Rows[0].Cells[SOP_LOCATION_INDEX].Value = actionStepHistory.Position;
                gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_INDEX].Value = MakeStartTime(actionStepHistory.BeginTime);
                gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_INDEX].Tag = actionStepHistory.BeginTime;

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
                //UpdateCommanderName("");

                gridActionStepInfo.Rows[0].Cells[SOP_NAME_INDEX].Value = null;// "";
                gridActionStepInfo.Rows[0].Cells[SOP_LOCATION_INDEX].Value = "";
                gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_INDEX].Value = "";
                gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_INDEX].Tag = null;
                gridActionStepInfo.Rows[0].Cells[SOP_ELAPSED_TIME_INDEX].Value = "";
            }

            gridLog.ClearSelection();
            gridLog.Rows.Clear();
            gridLog.Refresh();

            if (actionStepHistory != null)
            {
                List<ComponentHistory> histroies = new List<ComponentHistory>();
                histroies.AddRange(actionStepHistory.ComponentHistories);

                foreach (ComponentHistory componentHistory in histroies)
                {
                    AddComponentHistory(actionStepHistory, componentHistory);
                }
            }

            gridActionStepInfo.EndEdit();
            FormMain2.Instance.SetMenuButtonEnables();
        }

        private void UpdateCommanderName(string strCommanderName)
        {
            gridActionStepInfo.Rows[0].Cells[SOP_COMMANDER_INDEX].Value = strCommanderName;
        }

        private void UpdateElapsedTime(TimeInfo timeBegin, DateTime dtCurrent)
        {
            if (timeBegin == null)
                gridActionStepInfo.Rows[0].Cells[SOP_ELAPSED_TIME_INDEX].Value = "";
            else
            {
                TimeSpan span = dtCurrent - timeBegin.m_time;
                gridActionStepInfo.Rows[0].Cells[SOP_ELAPSED_TIME_INDEX].Value = MakeTimeSpanString(span);// +" 경과";
            }
        }

        public void UpdateData()
        {
            if (m_currentActionStepHistory != null && m_currentActionStepHistory.EndTime == null && m_currentActionStepHistory.CancelTime == null)
                UpdateElapsedTime(m_currentActionStepHistory.BeginTime, DateTime.Now);

            if (m_currentActionStepHistory == null)
                UpdateCommanderName("");
            else if (m_currentActionStepHistory.EndTime != null || m_currentActionStepHistory.CancelTime != null)
                UpdateCommanderName(m_currentActionStepHistory.CommanderName);
            else
                UpdateCommanderName(HistoryManager2.CurrentCommanderName);
        }

        private string MakeTimeSpanString(TimeSpan span)
        {
            string strTime = "";

            if (span.Days > 0)
            {
                strTime = span.Days + "일 " + span.Hours + "시간 " + span.Minutes + "분 " + span.Seconds + "초";
            }
            else
            {
                if (span.Hours > 0)
                {
                    strTime = span.Hours + "시간 " + span.Minutes + "분 " + span.Seconds + "초";
                }
                else
                {
                    if (span.Minutes > 0)
                    {
                        strTime = span.Minutes + "분 " + span.Seconds + "초";
                    }
                    else
                    {
                        strTime = span.Seconds + "초";
                    }
                }
            }

            return strTime;
        }

        private string MakeStartTime(TimeInfo time)
        {
            if (time == null)
                return "";

            return string.Format("{0}.{1:00}.{2:00} {3:00}:{4:00}", time.m_time.Year, time.m_time.Month, time.m_time.Day, time.m_time.Hour, time.m_time.Minute);
        }

        private bool IsShowingDetails(DataGridViewRow row)
        {
            if (row.Cells[LOG_NO_INDEX].Tag != null && (row.Cells[LOG_NO_INDEX].Tag is bool) && ((bool)row.Cells[LOG_NO_INDEX].Tag == true))
                return true;

            return false;
        }

        private bool HasDetails(DataGridViewRow row)
        {
            if (row.Tag != null && (row.Tag is ComponentHistory))
            {
                ComponentHistory componentHistory = (ComponentHistory)row.Tag;

                // Detail 로그가 두개 이상인 경우에 한한다.
                if (componentHistory.AllHistories.Count > 1)
                    return true;
            }

            return false;
        }

        // 특정 ComponentHistory에 대한 세부로그를 보여준다.
        private void ShowDetailComponentHistories(ComponentHistory componentHistory, DataGridViewRow row)
        {
            // 이미 세부로그를 보여주고 있다.
            if (IsShowingDetails(row))
                return;

            int nDetailCount = componentHistory.AllHistories.Count;

            if (nDetailCount == 0)
                return;

            int nAddCount = nDetailCount;

            foreach (ComponentHistory history in componentHistory.AllHistories)
            {
                if (history.Task == componentHistory.Task)
                    nAddCount--;
            }

            if (nAddCount <= 0)
                return;

            gridLog.Rows.Insert(row.Index + 1, nAddCount);

            int nIndex = 1;
            int nParentNo = ((NoString)row.Cells[LOG_NO_INDEX].Value).HeadNumber.Data;
            //string strParentNo = row.Cells[LOG_NO_INDEX].Value.ToString();

            foreach (ComponentHistory history in componentHistory.AllHistories)
            {
                if (history.Task == componentHistory.Task)
                    continue;

                DataGridViewRow _row = gridLog.Rows[row.Index + nIndex];
                //DataGridViewRow _row = DockingRealTime.MakeNewRow(gridLog);
                UpdateComponentHistory(_row, history/*, "         "*/);
                ProcessDetailRow(_row, nParentNo, nIndex++);
            }

            row.Cells[LOG_NO_INDEX].Tag = true;
        }

        private void ProcessDetailRow(DataGridViewRow row, int nParentNo, int nIndex)
        {
            row.Cells[LOG_NO_INDEX].Value = new NoString(nParentNo, nIndex);//string.Format("{0}-{1}", nParentNo, nIndex);

            row.Tag = null;
            row.Height = m_nDetailRowHeight;

            foreach (DataGridViewCell cell in row.Cells)
            {
                cell.Style.BackColor = nParentNo % 2 == 0 ? FormMain2.Instance.ColorStyle.EvenRowColor : FormMain2.Instance.ColorStyle.OddRowColor;
                cell.Style.Font = m_fontDetail;
            }
        }

        private void HideDetailComponentHistories(DataGridViewRow row)
        {
            int nRowCount = gridLog.Rows.Count;
            List<int> removeRowIndeces = new List<int>();

            for (int i=row.Index+1;i<nRowCount;i++)
            {
                DataGridViewRow _row = gridLog.Rows[i];

                if (_row.Tag == null)
                    removeRowIndeces.Add(i);
                else
                    break;
            }

            int nRemoveCount = removeRowIndeces.Count;

            for (int i=nRemoveCount-1;i>=0;i--)
            {
                int nRemoveIndex = removeRowIndeces[i];
                gridLog.Rows.RemoveAt(nRemoveIndex);
            }

            row.Cells[LOG_NO_INDEX].Tag = false;
        }

        public void OnPostDrawColumn(UnE.Controls.MergedDataGridView grid, DataGridViewCellPaintingEventArgs e)
        {
            Rectangle r = e.CellBounds;

            r.Width -= 1;
            r.Height -= 1;

            e.Graphics.DrawLine(m_penBorderLine, r.X, r.Y, r.X + r.Width, r.Y);
            e.Graphics.DrawLine(m_penBorderLine, r.X + r.Width, r.Y + r.Height, r.X, r.Y + r.Height);
            e.Graphics.DrawLine(m_penBorderLine, r.X, r.Y + r.Height, r.X, r.Y);

            if (e.ColumnIndex == grid.Columns.Count - 1)
                e.Graphics.DrawLine(m_penBorderLine, r.X + r.Width, r.Y, r.X + r.Width, r.Y + r.Height);
        }

        public void OnPostDrawCell(UnE.Controls.MergedDataGridView grid, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (grid.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
                return;

            bool indent = false;

            if (grid == gridLog && grid.Rows[e.RowIndex].Tag == null && e.ColumnIndex == LOG_TASK_INDEX)
                indent = true;

            // MergedCell이 아니기 때문에 직접 그린다.
            DrawCell(grid, e, indent);

            Rectangle r = e.CellBounds;

            r.Width -= 1;
            r.Height -= 1;

            DataGridViewRow row = grid.Rows[e.RowIndex];

            if (IsShowingDetails(row))
            {
                ComponentHistory history = (ComponentHistory)row.Tag;
                bool hasChildren = history.AllHistories.Count > 0;

                e.Graphics.DrawLine(m_penBoxLine, r.X, r.Y, r.X + r.Width, r.Y);

                if (e.ColumnIndex == 0)
                    e.Graphics.DrawLine(m_penBoxLine, r.X, r.Y + r.Height, r.X, r.Y);
                else
                    e.Graphics.DrawLine(m_penBorderLine, r.X, r.Y + r.Height, r.X, r.Y);

                if (e.ColumnIndex == grid.Columns.Count - 1)
                    e.Graphics.DrawLine(m_penBoxLine, r.X + r.Width, r.Y, r.X + r.Width, r.Y + r.Height);

                if (e.RowIndex == grid.Rows.Count - 1 || !hasChildren)
                    e.Graphics.DrawLine(m_penBoxLine, r.X + r.Width, r.Y + r.Height, r.X, r.Y + r.Height);
            }
            else if (grid == gridLog && row.Tag == null)
            {
                e.Graphics.DrawLine(m_penBorderLine, r.X, r.Y, r.X + r.Width, r.Y);

                if (e.ColumnIndex == 0)
                    e.Graphics.DrawLine(m_penBoxLine, r.X, r.Y + r.Height, r.X, r.Y);
                else
                    e.Graphics.DrawLine(m_penBorderLine, r.X, r.Y + r.Height, r.X, r.Y);

                if (e.ColumnIndex == grid.Columns.Count - 1)
                    e.Graphics.DrawLine(m_penBoxLine, r.X + r.Width, r.Y, r.X + r.Width, r.Y + r.Height);

                DataGridViewRow nextRow = null;

                if (e.RowIndex + 1 < grid.Rows.Count)
                    nextRow = grid.Rows[e.RowIndex + 1];

                if (nextRow == null || nextRow.Tag != null)
                    e.Graphics.DrawLine(m_penBoxLine, r.X + r.Width, r.Y + r.Height, r.X, r.Y + r.Height);
            }
            else
            {
                DataGridViewRow prevRow = null;

                if (e.RowIndex > 0)
                    prevRow = grid.Rows[e.RowIndex - 1];

                if (prevRow == null || (prevRow.Tag != null && !IsShowingDetails(prevRow)))
                    e.Graphics.DrawLine(m_penBorderLine, r.X, r.Y, r.X + r.Width, r.Y);

                e.Graphics.DrawLine(m_penBorderLine, r.X, r.Y + r.Height, r.X, r.Y);

                if (e.ColumnIndex == grid.Columns.Count - 1)
                    e.Graphics.DrawLine(m_penBorderLine, r.X + r.Width, r.Y, r.X + r.Width, r.Y + r.Height);

                if (e.RowIndex == grid.Rows.Count - 1)
                    e.Graphics.DrawLine(m_penBorderLine, r.X + r.Width, r.Y + r.Height, r.X, r.Y + r.Height);
            }

            if (e.ColumnIndex == LOG_TASK_INDEX && HasDetails(row))
            {
                int nImageWidth = m_imgDetailMark.Size.Width;
                int nImageHeight = m_imgDetailMark.Size.Height;
                int x = e.CellBounds.X + e.CellBounds.Width - nImageWidth * 3 / 2;
                int y = e.CellBounds.Y + (e.CellBounds.Height - nImageHeight) / 2;

                e.Graphics.DrawImage(m_imgDetailMark, x, y);
            }

            e.Handled = true;
        }

        private void DrawCell(UnE.Controls.MergedDataGridView grid, DataGridViewCellPaintingEventArgs e, bool indent)
        {
            e.PaintBackground(e.ClipBounds, true);

            Rectangle r = e.CellBounds;

            r.Width -= 1;
            r.Height -= 1;

            DataGridViewCell cell = grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            DataGridViewContentAlignment align = cell.InheritedStyle.Alignment;

            using (SolidBrush brBk = new SolidBrush(GetDrawingCellColor(e, cell, true)))
            using (SolidBrush brFr = new SolidBrush(GetDrawingCellColor(e, cell, false)))
            {
                e.Graphics.FillRectangle(brBk, r);

                StringFormat sf = GetStringFormat(align);
                r.Y += 2;

                string strCellValue = cell == null || cell.Value == null ? "" : cell.Value.ToString();

                if (indent)
                {
                    r.X += m_nIndentSize;
                    r.Width -= m_nIndentSize;

                    float fHeight = m_textRenderer.DrawText(e.Graphics, strCellValue, e.CellStyle.Font, brFr, r, m_fLineSpacing, sf);

                    if (fHeight > 0.0f && fHeight > cell.OwningRow.Height)
                        cell.OwningRow.Height = (int)fHeight + 1;
                }
                else
                    e.Graphics.DrawString(strCellValue, e.CellStyle.Font, brFr, r, sf);

                sf.Dispose();
            }
        }

        private Color GetDrawingCellColor(DataGridViewCellPaintingEventArgs e, DataGridViewCell cell, bool isBackround)
        {
            if (cell.Selected)
            {
                if (isBackround)
                    return e.CellStyle.SelectionBackColor;
                else
                    return e.CellStyle.SelectionForeColor;
            }

            if (isBackround)
                return e.CellStyle.BackColor;

            return e.CellStyle.ForeColor;
        }

        private StringFormat GetStringFormat(DataGridViewContentAlignment align)
        {
            StringFormat sf = new StringFormat();

            if (align == DataGridViewContentAlignment.BottomCenter)
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Far;
            }
            else if (align == DataGridViewContentAlignment.BottomLeft)
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Far;
            }
            else if (align == DataGridViewContentAlignment.BottomRight)
            {
                sf.Alignment = StringAlignment.Far;
                sf.LineAlignment = StringAlignment.Far;
            }
            else if (align == DataGridViewContentAlignment.MiddleCenter)
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
            }
            else if (align == DataGridViewContentAlignment.MiddleLeft)
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
            }
            else if (align == DataGridViewContentAlignment.MiddleRight)
            {
                sf.Alignment = StringAlignment.Far;
                sf.LineAlignment = StringAlignment.Center;
            }
            else if (align == DataGridViewContentAlignment.TopCenter)
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Near;
            }
            else if (align == DataGridViewContentAlignment.TopLeft)
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Near;
            }
            else if (align == DataGridViewContentAlignment.TopRight)
            {
                sf.Alignment = StringAlignment.Far;
                sf.LineAlignment = StringAlignment.Near;
            }

            return sf;
        }

        public void SetCurrentActionStep(int nActionStepID, bool isRealMode)
        {
            ActionStepHistory actionStepHistory = GetActionStepHistory(nActionStepID, isRealMode);

            if (m_currentActionStepHistory == actionStepHistory)
                m_ignoreAutoChangeActionStep = false;

            if (actionStepHistory == null)
                return;

            if (NeedChangeActionStep(m_currentActionStepHistory, actionStepHistory))
            //if (m_currentActionStepHistory != actionStepHistory)
            {
                if (m_ignoreAutoChangeActionStep)
                    return;

                ChangeActionStepHistory(actionStepHistory);
            }
        }

        private bool NeedChangeActionStep(ActionStepHistory currentActionStepHistory, ActionStepHistory newActionStepHistory)
        {
            if (currentActionStepHistory != newActionStepHistory)
                return true;

            if (currentActionStepHistory == newActionStepHistory)
            {
                if (gridActionStepInfo.Rows.Count == 0)
                {
                    if (newActionStepHistory != null)
                        return true;
                }
                else
                {
                    if (gridActionStepInfo.Rows[0].Cells[SOP_NAME_INDEX].Value == null)
                    {
                        if (newActionStepHistory != null)
                            return true;
                    }
                    else
                    {
                        string strCurrent = gridActionStepInfo.Rows[0].Cells[SOP_NAME_INDEX].Value.ToString();
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

        private ActionStepHistory GetActionStepHistory(int nActionStepID, bool isRealMode)
        {
            lock (LockActionStepInfo)
            {
                int nHistoryCount = m_actionStepHistories.Count;

                for (int i = 0; i < nHistoryCount; i++)
                {
                    ActionStepHistory history = m_actionStepHistories[i];

                    if (history.ActionStepID == nActionStepID && history.RealMode == isRealMode)
                    {
                        return history;
                    }
                }
            }

            return null;
        }

        public void tsMenuItemToHWPFile_Click(object sender, EventArgs e)
        {
            bool isHwpSetup = false;
            isHwpSetup = GetRegistry();

            //한글 설치여부
            //if (isHwpSetup == false)
            //{
            //    MessageBox.Show("아래한글이 설치되지 않았습니다.");
            //    return;
            //}

            //SaveFileDialog dlg = new SaveFileDialog();

            string strSavePath = GetHWPFilePath();

            if (strSavePath == null)
                return;
            /*dlg.Filter = "한글 문서 (*.hwp)|*.hwp";

            dlg.FileName = "상황판_한글파일";
            if (dlg.ShowDialog() == DialogResult.OK)*/
            {
                System.Diagnostics.ProcessStartInfo info = null;

                if (m_currentActionStepHistory == null)
                {
                    MessageBox.Show(FormMain2.Instance, "저장할 데이터가 존재하지 않습니다.");
                    return;
                }

                lock (LockActionStepInfo)
                {
                    // 결과 파일을 초기화한다.
                    ClearResultFile();

                    //내용 txt에 저장
                    SaveDataTxt();
                    SaveAllDataTxt();
                    SaveAllDetailDataTxt();

                    //SavePath = dlg.FileName;
                    //SavePath = subGap(SavePath);
                     
                    string logoFileName = GetReportLogoFileName();
                     
                    info = new System.Diagnostics.ProcessStartInfo();
                    info.CreateNoWindow = true;
                    //info.Arguments = strSavePath + " " + logoFileName + " " + FormMain2.Instance.SiteID;                     
                    //info.FileName = Application.StartupPath + "\\BulletinHwpEXE.exe";
                    info.Arguments = 5 + " 상황판 " + strSavePath + " " + logoFileName + " " + FormMain2.Instance.SiteID;
                    info.FileName = Application.StartupPath + "\\HmlReport.exe";
                }

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
                    //if (bSuccess)
                    //{
                    //    process.Kill();
                    //    break;
                    //}
                }

                if (bSuccess)
                {
                    bSuccess = ReadResultFile();
                }

                if (bSuccess == true)
                {
                    //if (m_strHWPPath != null && m_strHWPPath.Length > 0)
                    if (isHwpSetup)
                        RunHWP(strSavePath);
                    else
                    {
                        if (!isHwpSetup)
                        {
                            int nIndex = strSavePath.LastIndexOf(@"\");
                            string filePath = strSavePath.Substring(0, nIndex);
                            System.Diagnostics.Process.Start(filePath);
                            //MessageBox.Show("저장되었습니다.");
                        }
                    }
                }
                else
                    MessageBox.Show(FormMain2.Instance, "파일을 저장할 수 없습니다.");

                this.Cursor = Cursors.Default;
            }
        }

        private void RunHWP(string strFilePath)
        {
            string strHmlFilePath = strFilePath + ".hml";

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.Arguments = strHmlFilePath;
            info.FileName = m_strHWPPath;

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();
        }

        // 저장할 한글 파일의 경로
        private string GetHWPFilePath()
        {
            TimeInfo tSOP = null;
            string strTime = "";

            //상황발생시간
            if (gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_INDEX].Tag != null)
            {
                tSOP = (TimeInfo)gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_INDEX].Tag;
            }

            if (tSOP == null)
            {
                DateTime dtNow = DateTime.Now;
                strTime = string.Format("__{0}{1:00}{2:00}_{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            }
            else
            {
                DateTime dtSOP = tSOP.m_time;
                strTime = string.Format("{0}{1:00}{2:00}_{3:00}{4:00}{5:00}", dtSOP.Year, dtSOP.Month, dtSOP.Day, dtSOP.Hour, dtSOP.Minute, dtSOP.Second);
            }

            try
            {
                string strFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (strFolderPath != null && strFolderPath.Length > 0)
                {
                    if (!System.IO.Directory.Exists(strFolderPath + "\\상황판"))
                        System.IO.Directory.CreateDirectory(strFolderPath + "\\상황판");

                    //return strFolderPath + "\\상황판\\" + strTime + ".hwp";
                    return strFolderPath + "\\상황판\\" + strTime;
                }
            }
            catch (Exception)
            {
            }

            SaveFileDialog dlg = new SaveFileDialog();

            string strSavePath = "";
            dlg.Filter = "한글 문서 (*.hwp)|*.hwp";

            dlg.FileName = "상황판_" + strTime;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                strSavePath = dlg.FileName;
                strSavePath = subGap(strSavePath);
                return strSavePath;
            }

            return null;
        }

        public bool GetRegistry()
        {
            const string HwpRoot = @"Applications\Hwp.exe";

            RegistryKey R = Registry.ClassesRoot.OpenSubKey(HwpRoot);

            if (R == null)
            {
                // 한글 2018 이후로는 HwpRoot가 생기지 않는다.
                string strProgramPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);

                if (strProgramPath != null && strProgramPath.Length > 0)
                {
                    strProgramPath += "\\Programs";
                    return FindHWPFolder(strProgramPath);
                }

                return false;
            }

            if (m_strHWPPath != null)
                return true;

            m_strHWPPath = "";

            RegistryKey shell = R.OpenSubKey("shell");

            if (shell == null)
                return true;

            RegistryKey open = shell.OpenSubKey("open");

            if (open == null)
                return true;

            RegistryKey command = open.OpenSubKey("command");

            if (command == null)
                return true;

            string[] names = command.GetValueNames();

            if (names == null || names.Count() == 0)
                return true;

            object value = command.GetValue(names[0]);

            if (value == null)
                return true;

            string strValue = value.ToString();
            string strTarget = ".exe";
            int nIndex1 = strValue.IndexOf(strTarget);

            if (nIndex1 < 0)
                return true;

            string strPath = strValue.Substring(0, nIndex1 + strTarget.Length).Trim();

            if (strPath.StartsWith("\""))
                m_strHWPPath = strPath.Substring(1);
            else
                m_strHWPPath = strPath;

            /*int nIndex1 = strValue.IndexOf('\"');

            if (nIndex1 < 0)
                return true;

            int nIndex2 = strValue.IndexOf('\"', nIndex1 + 1);

            if (nIndex2 < 0)
                return true;

            string strPath = strValue.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            m_strHWPPath = strPath;*/
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
                            string strHWPPath = GetShortcutTargetFile(strFile);
                            m_strHWPPath = strHWPPath;
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

        public static string GetShortcutTargetFile(string shortcutFilename)
        {
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshRuntimeLibrary.IWshShortcut link = shell.CreateShortcut(shortcutFilename);
            return link.TargetPath;
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

        // 결과 파일을 초기화한다.
        private void ClearResultFile()
        {
            System.IO.File.Delete(Application.StartupPath + "\\report\\BulletinResult.txt");
        }

        private void SaveDataTxt()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\BulletHwpData.txt"))
            {
                //SOP명
                file.WriteLine(gridActionStepInfo.Rows[0].Cells[SOP_NAME_INDEX].Value);
                //진행총괄
                file.WriteLine(gridActionStepInfo.Rows[0].Cells[SOP_COMMANDER_INDEX].Value);
                //상황발생위치
                file.WriteLine(gridActionStepInfo.Rows[0].Cells[SOP_LOCATION_INDEX].Value);

                //재난대응현황
                /*string str = gridActionStepInfo.Rows[0].Cells[SOP_NAME_INDEX].Value == null ? "" : gridActionStepInfo.Rows[0].Cells[SOP_NAME_INDEX].Value.ToString();
                int nIndex1 = str.LastIndexOf('/');

                if (nIndex1 < 0)
                    file.WriteLine("");
                else
                    file.WriteLine(str.Substring(nIndex1 + 1).Trim());

                //재난명
                int nIndex2 = str.IndexOf('/');

                if (nIndex2 < 0 || nIndex2 >= nIndex1)
                    file.WriteLine("");
                else
                    file.WriteLine(str.Substring(nIndex2 + 1, nIndex1 - nIndex2 - 1));*/

                //상황발생시간
                if (gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_INDEX].Tag != null)
                {
                    TimeInfo beginTimeInfo = (TimeInfo)gridActionStepInfo.Rows[0].Cells[SOP_START_TIME_INDEX].Tag;

                    if (beginTimeInfo == null)
                        file.WriteLine("");
                    else
                    {
                        DateTime beginTime = beginTimeInfo.m_time;
                        file.WriteLine(string.Format("{0}년 {1}월 {2}일 {3}시 {4}분 {5}초", beginTime.Year, beginTime.Month, beginTime.Day, beginTime.Hour, beginTime.Minute, beginTime.Second));
                    }
                }
                else
                    file.WriteLine("");

                // 총 소요시간
                string strSubTime = "";
                strSubTime = GetTimeSpan();

                file.WriteLine(strSubTime);

                //경과시간
                if (m_currentActionStepHistory.EndTime == null && m_currentActionStepHistory.CancelTime == null)
                    file.WriteLine("SOP 진행중");
                else if (m_currentActionStepHistory.EndTime != null)
                    file.WriteLine("SOP 종료");
                else if (m_currentActionStepHistory.CancelTime != null)
                    file.WriteLine("SOP 실행중 취소");

                file.Close();
            }
        }

        private string GetTimeSpan()
        {
            if (gridActionStepInfo.Rows[0].Cells[SOP_ELAPSED_TIME_INDEX].Value == null)
                return "";

            string str = gridActionStepInfo.Rows[0].Cells[SOP_ELAPSED_TIME_INDEX].Value.ToString();
            
            for (int i=str.Length-1;i>=0;i--)
            {
                char ch = str.ElementAt(i);

                if ((ch >= '0' && ch <= '9') || ch == '초' || ch == '분' || ch == '시')
                {
                    str = str.Substring(0, i + 1);
                    break;
                }
            }

            return str;
        }

        private void CollapseDetailGrid()
        {
            for (int i = 0; i < gridLog.Rows.Count; i++)
            {
                DataGridViewRow row2 = gridLog.Rows[i];
                if (HasDetails(row2) && IsShowingDetails(row2))
                    HideDetailComponentHistories(row2);
            }
        }

        private void ExpandDetailGrid()
        {
            for (int i = 0; i < gridLog.Rows.Count; i++)
            {
                DataGridViewRow row = gridLog.Rows[i];

                ComponentHistory componentHistory = (ComponentHistory)row.Tag;
                if (componentHistory == null)
                    continue;

                if (componentHistory.SectionState != null && componentHistory.SectionState.Section != null)
                {
                    Sections.Section.ComponentType sectionType = componentHistory.SectionState.Section.GetComponentType();

                    if ((sectionType == Sections.Section.ComponentType.PROCESS ||
                        sectionType == Sections.Section.ComponentType.INTERNAL) && HasDetails(row))
                    {
                        if (!IsShowingDetails(row))
                            ShowDetailComponentHistories(componentHistory, row);
                    }
                }
            }
        }

        private void SaveGridData(string szFileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\" + szFileName, false, System.Text.Encoding.UTF8))
            {
                for (int i = 0; i < gridLog.Rows.Count; i++)
                {
                    for (int j = 0; j < gridLog.Rows[i].Cells.Count; j++)
                    {
                        //if (j == 2 || j == 3)
                        //    continue;

                        string strValue = gridLog.Rows[i].Cells[j].Value == null ? "" : gridLog.Rows[i].Cells[j].Value.ToString();
                        strValue = strValue.Replace("\r", " ");
                        strValue = strValue.Replace("\n", " ");

                        file.WriteLine(strValue);
                        file.WriteLine("-----문단구분-----");
                    }
                }
            }
        }

        private void SaveAllDetailDataTxt()
        {
            ExpandDetailGrid();

            SaveGridData("BulletHwpDetailData.txt");

            CollapseDetailGrid();
        }
      
        private void SaveAllDataTxt()
        {
            CollapseDetailGrid();
            
            SaveGridData("BulletHwpAllData.txt");
          
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

        // 결과를 읽어온다.
        private bool ReadResultFile()
        {
            return System.IO.File.Exists(Application.StartupPath + "\\report\\BulletinResult.txt");
            /*System.IO.StreamReader reader = new System.IO.StreamReader(Application.StartupPath + "\\BulletinResult.txt");
            int nData = reader.Read();
            reader.Close();

            return nData == 1;*/
        }

        private void DockingRealTime2_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                FormMain2.Instance.ShowContextMenu((Control)sender, e.X, e.Y);
            }
        }

        public void ShowContextMenu(Control ctrl, int x, int y)
        {
            if (m_currentActionStepHistory == null)
            {
                contextMenuStrip1.Items[0].Enabled = false;
                contextMenuStrip1.Items[1].Enabled = false;
            }
            else
            {
                contextMenuStrip1.Items[0].Enabled = true;
                contextMenuStrip1.Items[1].Enabled = m_currentActionStepHistory.EndTime != null || m_currentActionStepHistory.CancelTime != null;
            }

            if (GetRegistry() == false)
                contextMenuStrip1.Items[0].Enabled = false;

            contextMenuStrip1.Show(ctrl, x, y);
        }

        public void SetMenuButtonEnables(Button btnSaveToHWP, Button btnCloseCurrentLog, Button btnShowPrevLog)
        {
            if (m_currentActionStepHistory == null)
            {
                btnSaveToHWP.Enabled = false;
                btnCloseCurrentLog.Enabled = false;

                btnSaveToHWP.FlatStyle = btnCloseCurrentLog.FlatStyle = FlatStyle.Flat;
                btnSaveToHWP.Size = new Size(btnSaveToHWP.Size.Width, 21);
                btnCloseCurrentLog.Size = new Size(btnCloseCurrentLog.Size.Width, 21);
            }
            else
            {
                btnSaveToHWP.Enabled = GetRegistry();
                btnCloseCurrentLog.Enabled = m_currentActionStepHistory.EndTime != null || m_currentActionStepHistory.CancelTime != null;

                btnSaveToHWP.FlatStyle = btnSaveToHWP.Enabled ? FlatStyle.Standard : FlatStyle.Flat;
                btnCloseCurrentLog.FlatStyle = btnCloseCurrentLog.Enabled ? FlatStyle.Standard : FlatStyle.Flat;
                btnSaveToHWP.Size = new Size(btnSaveToHWP.Size.Width, 19);
                btnCloseCurrentLog.Size = btnCloseCurrentLog.Enabled ? new Size(btnCloseCurrentLog.Size.Width, 19) : new Size(btnCloseCurrentLog.Size.Width, 21);
            }
        }

        public void tsMenuItemCloseCurrentLog_Click(object sender, EventArgs e)
        {
            lock (LockActionStepInfo)
            {
                if (m_currentActionStepHistory == null)
                    return;

                int nIndex = m_actionStepHistories.IndexOf(m_currentActionStepHistory);

                if (nIndex < 0)
                    return;

                m_actionStepHistories.Remove(m_currentActionStepHistory);

                //if (m_cboActionStepHistory != null && m_cboActionStepHistory.Items.Contains(m_currentActionStepHistory))
                {
                    m_cboActionStepHistory.Items.Remove(m_currentActionStepHistory);
                    //colSOPFullPath.Items.Remove(m_currentActionStepHistory);
                }

                int nHistoryCount = m_actionStepHistories.Count;

                if (nHistoryCount == 0)
                    ChangeActionStepHistory(null);
                else
                {
                    if (nIndex == 0)
                    {
                        ActionStepHistory history = m_actionStepHistories[0];
                        ChangeActionStepHistory(history);
                        m_cboActionStepHistory.SelectedIndex = 0;
                    }
                    else
                    {
                        ActionStepHistory history = m_actionStepHistories[nIndex - 1];
                        ChangeActionStepHistory(history);
                        m_cboActionStepHistory.SelectedIndex = nIndex - 1;
                    }
                }
            }
        }

        public void tsMenuItemShowPrevLogs_Click(object sender, EventArgs e)
        {
            FormPrevLog frm = new FormPrevLog();

            if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (frm.SelectedActionStepHistory == null)
                    return;


                LoadActionStepHistory(frm.SelectedActionStepHistory);

            }
        }

        public void LoadActionStepHistory(ActionStepHistory actionStepHistory)
        {
            Dictionary<int, ActionStepHistory> dicActionStepHistory = new Dictionary<int, ActionStepHistory>();
            dicActionStepHistory[actionStepHistory.ActionStepHistoryID] = actionStepHistory;

            ActionStepHistory prev = m_currentActionStepHistory;

            AddActionStepHistory(actionStepHistory);

            HistoryManager2.AddActionStepSections(FormMain2.Instance.DBManager, new IOManager(), actionStepHistory.ActionStepID, actionStepHistory);
            HistoryManager2.LoadComponentHistory(this, FormMain2.Instance.DBManager, actionStepHistory.ActionStepHistoryID.ToString(), dicActionStepHistory, true);

            if (prev != actionStepHistory)
            {
                m_ignoreAutoChangeActionStep = true;
                ChangeActionStepHistory(actionStepHistory);
            }
        }

        private void tsMenuItemCloseApplication_Click(object sender, EventArgs e)
        {
            FormMain2.Instance.Close();
        }

        private void gridActionStepInfo_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
        }

        private bool m_bShowCmb = false;
        private void gridActionStepInfo_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (m_actionStepHistories.Count <= 1)
            {
                m_bShowCmb = true;
            }
            e.CellStyle.BackColor = Color.Aqua;           
        }

        private void gridLog_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.ColumnIndex < 0)
                    return;

                Rectangle rect;

                if (e.RowIndex < 0) // Column Header일 경우
                {
                    rect = gridLog.GetColumnDisplayRectangle(e.ColumnIndex, true);
                }
                else                // 일반 Cell일 경우
                    rect = gridLog.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                FormMain2.Instance.ShowContextMenu((Control)sender, e.X + rect.X, e.Y + rect.Y);
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                lock (LockLog)
                {
                    DataGridViewRow row = gridLog.Rows[e.RowIndex];

                    if (row.Tag != null && row.Tag is ComponentHistory)
                    {
                        ComponentHistory componentHistory = (ComponentHistory)row.Tag;

                        if (componentHistory.SectionState != null && componentHistory.SectionState.Section != null)
                        {
                            Sections.Section.ComponentType sectionType = componentHistory.SectionState.Section.GetComponentType();

                            if ((sectionType == Sections.Section.ComponentType.PROCESS ||
                                sectionType == Sections.Section.ComponentType.INTERNAL) && HasDetails(row))
                            {
                                if (!IsShowingDetails(row))
                                    ShowDetailComponentHistories(componentHistory, row);
                                else
                                    HideDetailComponentHistories(row);
                                /*FormDetailLog frm = new FormDetailLog(componentHistory);
                                frm.ShowDialog(this);*/
                            }
                        }
                    }
                }
            }
        }

        private void gridActionStepInfo_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            if (gridActionStepInfo.Rows.Count == 0)
                return;

            Point location;
            Size cellSize = GetComboBoxCellSize(out location);

            m_cboActionStepHistory.Size = cellSize;
            m_cboActionStepHistory.Location = location;
        }

        private string GetReportLogoFileName()
        { 
            string strSQL = "Select PropertyValue from OptionSdms where PropertyName='LogoFileName' and SiteID=" + FormMain2.Instance.SiteID;                
            ArrayList arrResult = FormMain2.Instance.DBManager.GetResultData(strSQL, 0);
                
            if (arrResult == null || arrResult.Count == 0) return string.Empty;
                
            string logoName = DBUtility.WebDBManager.GetStringField(arrResult[0].ToString(), string.Empty);                
            return logoName; 
        }
    }
}
