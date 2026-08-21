using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
    public partial class FormResultPropertyHistory : Form
    {
        private ResultProperty m_prop = null;
        private FormResultProperty m_frm = null;
        private DataGridViewRow m_resultPropertyRow = null;
        private DateTimePicker m_timePicker = null;

        private const int PROJECT_NAME = 1;
        private const int BEGIN_TIME = 2;
        private const int END_TIME = 3;
        private const int PROJECT_COST = 4;
        private const int ACCUMUL_LENGTH = 5;
        private const int UNIT_LENGTH = 6;
        private const int ACCUMUL_AREA = 7;
        private const int UNIT_AREA = 8;
        private const int DIR_FROM_BEGIN = 9;

        public ResultProperty ResultProperty
        {
            get { return m_prop; }
        }

        public FormResultPropertyHistory(ResultProperty prop, FormResultProperty frm, DataGridViewRow row)
        {
            InitializeComponent();
            m_prop = prop;
            m_frm = frm;
            m_resultPropertyRow = row;
        }

        private void FormResultPropertyHistory_Load(object sender, EventArgs e)
        {
            InitGridHeader();
            InitGrid(m_prop);

            m_timePicker = new DateTimePicker();
            dataGridView1.Controls.Add(m_timePicker);
            m_timePicker.Visible = false;

            m_timePicker.CloseUp += new EventHandler(DateTimePicker_CloseUp);

            dataGridView1.Select();
        }

        private void DateTimePicker_CloseUp(object sender, EventArgs e)
        {
            string strTime = string.Format("{0}-{1}-{2}", m_timePicker.Value.Year, m_timePicker.Value.Month, m_timePicker.Value.Day);

            if (m_timePicker.Tag != null)
            {
                DataGridViewCell cell = (DataGridViewCell)m_timePicker.Tag;
                cell.Value = strTime;

                if (cell.Tag == null)
                    cell.Tag = new VariousData<DateTime>(m_timePicker.Value);
                else
                    ((VariousData<DateTime>)cell.Tag).Data = m_timePicker.Value;

                m_timePicker.Hide();

                CalcAccumulLength();
                CalcAccumulArea();
            }
        }

        private void InitGridHeader()
        {
            colNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colProjectName.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colBeginTime.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colEndTime.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colProjectCost.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colAcumulatedLength.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colProjectLength.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colAcumulatedArea.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colProjectArea.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colStartDirection.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void InitGrid(ResultProperty prop)
        {
            if (prop == null)
                return;

            dataGridView1.Rows.Clear();

            foreach (ResultPropertyData data in prop.PropertyDatas)
            {
                DataGridViewRow row = MakeNewRow();

                row.Cells[0].Value = row.Index + 1;
                row.Cells[PROJECT_NAME].Value = data.ProjectName;

                if (data.BeginTime != null)
                {
                    row.Cells[BEGIN_TIME].Value = ScheduleProperty.GetDateTimeString(data.BeginTime.Data);
                    row.Cells[BEGIN_TIME].Tag = new VariousData<DateTime>(data.BeginTime.Data);
                }

                if (data.EndTime != null)
                {
                    row.Cells[END_TIME].Value = ScheduleProperty.GetDateTimeString(data.EndTime.Data);
                    row.Cells[END_TIME].Tag = new VariousData<DateTime>(data.EndTime.Data);
                }

                SetProjectCost(row.Cells[PROJECT_COST], data.ProjectCost == null ? null : new VariousData<long>(data.ProjectCost.Data));
                SetIntData(row.Cells[ACCUMUL_LENGTH], data.AccumulLength == null ? null : new VariousData<int>(data.AccumulLength.Data));
                SetIntData(row.Cells[UNIT_LENGTH], data.UnitLength == null ? null : new VariousData<int>(data.UnitLength.Data));
                SetIntData(row.Cells[ACCUMUL_AREA], data.AccumulArea == null ? null : new VariousData<int>(data.AccumulArea.Data));
                SetIntData(row.Cells[UNIT_AREA], data.UnitArea == null ? null : new VariousData<int>(data.UnitArea.Data));
                SetCompleteDirection(row.Cells[DIR_FROM_BEGIN], data.DirectionFromBegin == null ? null : new VariousData<bool>(data.DirectionFromBegin.Data));
            }
        }

        private void SetProjectCost(DataGridViewCell cell, VariousData<long> nProjectCost)
        {
            if (nProjectCost != null)
            {
                SetProjectCost(cell, nProjectCost.Data);
                cell.Tag = nProjectCost;
            }
            else
            {
                cell.Value = null;
                cell.Tag = null;
            }
        }

        private void SetProjectCost(DataGridViewCell cell, long nProjectCost)
        {
            if (nProjectCost == 0)
                cell.Value = "0원";
            else
                cell.Value = string.Format("{0:###,###,###,###,###,###}원", nProjectCost);
        }

        private void SetIntData(DataGridViewCell cell, VariousData<int> nLength)
        {
            if (nLength == null)
            {
                cell.Value = null;
                cell.Tag = null;
            }
            else
            {
                SetIntData(cell, nLength.Data);
                cell.Tag = nLength;
            }
        }

        private void SetIntData(DataGridViewCell cell, int nLength)
        {
            if (nLength == 0)
                cell.Value = "0";
            else
                cell.Value = string.Format("{0:###,###,###,###,###,###}", nLength);
        }

        private void SetCompleteDirection(DataGridViewCell cell, VariousData<bool> fromBegin)
        {
            if (fromBegin == null)
            {
                cell.Value = null;
                cell.Tag = null;
            }
            else
            {
                SetCompleteDirection(cell, fromBegin.Data);
                cell.Tag = fromBegin;
            }
        }

        private void SetCompleteDirection(DataGridViewCell cell, bool fromBegin)
        {
            cell.Value = fromBegin ? "시점으로부터" : "종점으로부터";
            //cell.Tag = fromBegin;
        }

        private DataGridViewRow MakeNewRow()
        {
            int nIndex = 0;

            if (dataGridView1.AllowUserToAddRows)
            {
                nIndex = dataGridView1.Rows.Count - 1;
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;

                nIndex = dataGridView1.Rows.Count - 1;
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                dataGridView1.AllowUserToAddRows = false;
            }

            return dataGridView1.Rows[nIndex];
        }

        private void FormResultPropertyHistory_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_frm != null)
                m_frm.ResultPropertyHistoryForm = null;
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            m_timePicker.Hide();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_timePicker == null)
                return;

            m_timePicker.Hide();

            if ((e.ColumnIndex == BEGIN_TIME || e.ColumnIndex == END_TIME) && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex];

                Rectangle rect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                if (cell.Value != null)
                {
                    DateTime date = new DateTime();

                    if (GetDateTime(cell.Value, ref date))
                    {
                        m_timePicker.Value = date;
                    }
                }

                m_timePicker.Location = new Point(rect.Left, rect.Top);
                m_timePicker.Tag = cell;

                m_timePicker.Size = new Size(rect.Width, rect.Height);

				//DialogFormFrame fameProperty = new DialogFormFrame();
                m_timePicker.Show();
            }
        }

        private bool GetDateTime(object obj, ref DateTime date)
        {
            string strText = obj.ToString();
            string[] arrDate = strText.Split('-');

            if (arrDate.Count() != 3)
                return false;

            int nYear, nMonth, nDay;

            if (!int.TryParse(arrDate[0], out nYear))
                return false;

            if (!int.TryParse(arrDate[1], out nMonth))
                return false;

            if (!int.TryParse(arrDate[2], out nDay))
                return false;

            try
            {
                date = new DateTime(nYear, nMonth, nDay);
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }

            return true;
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex == PROJECT_COST)
            {
                if (cell.Tag != null)
                {
                    long nCost = ((VariousData<long>)cell.Tag).Data;
                    cell.Value = nCost;
                }
            }
            else if (e.ColumnIndex == ACCUMUL_LENGTH || e.ColumnIndex == UNIT_LENGTH ||
                e.ColumnIndex == ACCUMUL_AREA || e.ColumnIndex == UNIT_AREA)
            {
                if (cell.Tag != null)
                {
                    cell.Value = ((VariousData<int>)cell.Tag).Data;
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex == DIR_FROM_BEGIN)
            {
                /*if (cell.Tag == null)
                    return;

                ComboBoxText text = (ComboBoxText)cell.Tag;
                string strText = text.Control.Text;

                if (strText.Length == 0)
                {
                    cell.Value = null;
                    cell.Tag = null;
                }
                else if (strText.Length > 0 && text.EndType != ComboBoxText.EndEditType.CANCEL)
                {
                    DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex];

                    if (!column.Items.Contains(strText))
                    {
                        column.Items.Add(strText);
                        cell.Value = strText;
                        text.EndType = ComboBoxText.EndEditType.NONE;
                    }
                    else
                        cell.Value = strText;
                }
                else
                    return;*/

                if (cell.Value == null || cell.Value.ToString().Length == 0)
                {
                    cell.Tag = null;
                    return;
                }
                else
                {
                    if (cell.Tag != null)
                    {
                        VariousData<bool> fromBegin = GetDirectionFromBegin(dataGridView1.Rows[e.RowIndex]);

                        if (fromBegin == null)
                            return;

                        if (fromBegin.Data == ((VariousData<bool>)cell.Tag).Data)
                            return;
                    }
                }

                CalcAccumulLength();
                CalcAccumulArea();
            }
            else if (e.ColumnIndex == PROJECT_COST)
            {
                if (cell.Value != null && cell.Value.ToString().Length > 0)
                {
                    long nCost;

                    if (!long.TryParse(cell.Value.ToString(), out nCost))
                    {
						string szMsg = "사업비 값이 잘못되었습니다.";
                        UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //MessageBox.Show("사업비 값이 잘못되었습니다.");

                        if (cell.Tag == null)
                            cell.Value = null;
                        else
                            SetProjectCost(cell, (VariousData<long>)cell.Tag);
                    }
                    else
                    {
                        SetProjectCost(cell, nCost);
                        cell.Tag = new VariousData<long>(nCost);
                    }
                }
            }
            /*else if (e.ColumnIndex == BEGIN_TIME || e.ColumnIndex == END_TIME)
            {
                CalcAccumulLength();
                CalcAccumulArea();
            }*/
            else if (e.ColumnIndex == ACCUMUL_LENGTH)
            {
                if (EndEditIntData(cell, "누적길이"))
                {
                    //CalcUnitLength(dataGridView1.Rows[e.RowIndex]);
                }
            }
            else if (e.ColumnIndex == UNIT_LENGTH)
            {
                if (EndEditIntData(cell, "길이"))
                {
                    //CalcAccumulLength(dataGridView1.Rows[e.RowIndex]);
                    CalcAccumulLength();
                }
            }
            else if (e.ColumnIndex == ACCUMUL_AREA)
            {
                if (EndEditIntData(cell, "누적면적"))
                {
                    //CalcUnitArea(dataGridView1.Rows[e.RowIndex]);
                }
            }
            else if (e.ColumnIndex == UNIT_AREA)
            {
                if (EndEditIntData(cell, "면적"))
                {
                    //CalcAccumulArea(dataGridView1.Rows[e.RowIndex]);
                    CalcAccumulArea();
                }
            }
        }

        private void CalcAccumulArea()
        {
            List<ResultPropertyData> datas = MakeTempData4Calc();
            CalcAccumulArea(datas, true);
            CalcAccumulArea(datas, false);
        }

        private void CalcAccumulLength()
        {
            List<ResultPropertyData> datas = MakeTempData4Calc();
            CalcAccumulLength(datas, true);
            CalcAccumulLength(datas, false);
        }

        private void CalcAccumulArea(List<ResultPropertyData> datas, bool fromBegin)
        {
            int nAccumulArea = 0;
            int nDataCount = datas.Count;

            for (int i = 0; i < nDataCount; i++)
            {
                ResultPropertyData data = datas[i];

                if (data.DirectionFromBegin == null || data.DirectionFromBegin.Data != fromBegin)
                    continue;

                // 단위 면적이 null인 값이 있으면 계산을 중단한다.
                if (data.UnitArea == null)
                    return;

                nAccumulArea += data.UnitArea.Data;

                int nRowIndex = (int)data.ProjectCost.Data;
                DataGridViewRow row = dataGridView1.Rows[nRowIndex];

                SetIntData(row.Cells[ACCUMUL_AREA], nAccumulArea);
                row.Cells[ACCUMUL_AREA].Tag = new VariousData<int>(nAccumulArea);
            }
        }

        private void CalcAccumulLength(List<ResultPropertyData> datas, bool fromBegin)
        {
            int nAccumulLength = 0;
            int nDataCount = datas.Count;

            for (int i = 0; i < nDataCount; i++)
            {
                ResultPropertyData data = datas[i];

                if (data.DirectionFromBegin == null || data.DirectionFromBegin.Data != fromBegin)
                    continue;

                // 단위 길이가 null인 값이 있으면 계산을 중단한다.
                if (data.UnitLength == null)
                    return;

                nAccumulLength += data.UnitLength.Data;

                int nRowIndex = (int)data.ProjectCost.Data;
                DataGridViewRow row = dataGridView1.Rows[nRowIndex];

                SetIntData(row.Cells[ACCUMUL_LENGTH], nAccumulLength);
                row.Cells[ACCUMUL_LENGTH].Tag = new VariousData<int>(nAccumulLength);
            }
        }

        /*private void CalcUnitArea(DataGridViewRow row)
        {
            if (row.Cells[ACCUMUL_AREA].Tag == null)
                return;

            VariousData<bool> fromBegin = GetDirectionFromBegin(row);

            if (fromBegin == null)
                return;

            List<ResultPropertyData> datas = MakeTempData4Calc();

            int nAccumulArea = 0, nBeginIndex = -1;
            int nDataCount = datas.Count;

            for (int i = 0; i < nDataCount; i++)
            {
                ResultPropertyData data = datas[i];

                if (data.ProjectCost.Data == (long)row.Index)
                {
                    int nUnitArea = ((VariousData<int>)row.Cells[ACCUMUL_AREA].Tag).Data - nAccumulArea;
                    SetIntData(row.Cells[UNIT_AREA], nUnitArea);
                    row.Cells[UNIT_AREA].Tag = new VariousData<int>(nUnitArea);

                    nAccumulArea += nUnitArea;
                    nBeginIndex = i + 1;
                    break;
                }

                if (data.DirectionFromBegin != null && data.DirectionFromBegin.Data == fromBegin.Data)
                {
                    // 중간에 null인 값이 있으면 계산할 수 없다.
                    if (data.UnitArea == null)
                        return;

                    nAccumulArea += data.UnitArea.Data;
                }
            }

            PostCalcArea(nAccumulArea, nBeginIndex, datas, nDataCount, fromBegin.Data);
        }

        private void CalcAccumulArea(DataGridViewRow row)
        {
            if (row.Cells[UNIT_AREA].Tag == null)
                return;

            VariousData<bool> fromBegin = GetDirectionFromBegin(row);

            if (fromBegin == null)
                return;

            List<ResultPropertyData> datas = MakeTempData4Calc();

            int nAccumulArea = 0, nBeginIndex = -1;
            int nDataCount = datas.Count;

            for (int i = 0; i < nDataCount; i++)
            {
                ResultPropertyData data = datas[i];

                if (data.ProjectCost.Data == (long)row.Index)
                {
                    nAccumulArea += ((VariousData<int>)row.Cells[UNIT_AREA].Tag).Data;
                    SetIntData(row.Cells[ACCUMUL_AREA], nAccumulArea);
                    row.Cells[ACCUMUL_AREA].Tag = new VariousData<int>(nAccumulArea);

                    nBeginIndex = i + 1;
                    break;
                }

                if (data.DirectionFromBegin != null && data.DirectionFromBegin.Data == fromBegin.Data)
                {
                    // 중간에 null인 값이 있으면 계산할 수 없다.
                    if (data.UnitArea == null)
                        return;

                    nAccumulArea += data.UnitArea.Data;
                }
            }

            PostCalcArea(nAccumulArea, nBeginIndex, datas, nDataCount, fromBegin.Data);
        }

        private void CalcUnitLength(DataGridViewRow row)
        {
            if (row.Cells[ACCUMUL_LENGTH].Tag == null)
                return;

            VariousData<bool> fromBegin = GetDirectionFromBegin(row);

            if (fromBegin == null)
                return;

            List<ResultPropertyData> datas = MakeTempData4Calc();

            int nAccumulLength = 0, nBeginIndex = -1;
            int nDataCount = datas.Count;

            for (int i = 0; i < nDataCount; i++)
            {
                ResultPropertyData data = datas[i];

                if (data.ProjectCost.Data == (long)row.Index)
                {
                    int nUnitLength = ((VariousData<int>)row.Cells[ACCUMUL_LENGTH].Tag).Data - nAccumulLength;
                    SetIntData(row.Cells[UNIT_LENGTH], nUnitLength);
                    row.Cells[UNIT_LENGTH].Tag = new VariousData<int>(nUnitLength);

                    nAccumulLength += nUnitLength;
                    nBeginIndex = i + 1;
                    break;
                }

                if (data.DirectionFromBegin != null && data.DirectionFromBegin.Data == fromBegin.Data)
                {
                    // 중간에 null인 값이 있으면 계산할 수 없다.
                    if (data.UnitLength == null)
                        return;

                    nAccumulLength += data.UnitLength.Data;
                }
            }

            PostCalcLength(nAccumulLength, nBeginIndex, datas, nDataCount, fromBegin.Data);
        }*/

        private void CalcAccumulLength(DataGridViewRow row)
        {
            if (row.Cells[UNIT_LENGTH].Tag == null)
                return;

            VariousData<bool> fromBegin = GetDirectionFromBegin(row);

            if (fromBegin == null)
                return;

            List<ResultPropertyData> datas = MakeTempData4Calc();

            int nAccumulLength = 0, nBeginIndex = -1;
            int nDataCount = datas.Count;

            for (int i = 0; i < nDataCount; i++)
            {
                ResultPropertyData data = datas[i];

                if (data.ProjectCost.Data == (long)row.Index)
                {
                    nAccumulLength += ((VariousData<int>)row.Cells[UNIT_LENGTH].Tag).Data;
                    SetIntData(row.Cells[ACCUMUL_LENGTH], nAccumulLength);
                    row.Cells[ACCUMUL_LENGTH].Tag = new VariousData<int>(nAccumulLength);

                    nBeginIndex = i + 1;
                    break;
                }

                if (data.DirectionFromBegin != null && data.DirectionFromBegin.Data == fromBegin.Data)
                {
                    // 중간에 null인 값이 있으면 계산할 수 없다.
                    if (data.UnitLength == null)
                        return;

                    nAccumulLength += data.UnitLength.Data;
                }
            }

            PostCalcLength(nAccumulLength, nBeginIndex, datas, nDataCount, fromBegin.Data);
        }

        private void PostCalcLength(int nAccumulLength, int nBeginIndex, List<ResultPropertyData> datas, int nDataCount, bool fromBegin)
        {
            if (nBeginIndex < 0)
                return;

            for (int i = nBeginIndex; i < nDataCount; i++)
            {
                ResultPropertyData data = datas[i];

                if (data.DirectionFromBegin == null || data.DirectionFromBegin.Data != fromBegin)
                    continue;

                if (data.UnitLength == null && data.AccumulLength == null)
                    break;

                int nRowIndex = (int)data.ProjectCost.Data;
                DataGridViewRow row2 = dataGridView1.Rows[nRowIndex];

                if (data.UnitLength != null)
                {
                    int nUnitLength = data.UnitLength.Data;
                    nAccumulLength += nUnitLength;

                    SetIntData(row2.Cells[ACCUMUL_LENGTH], nAccumulLength);
                    row2.Cells[ACCUMUL_LENGTH].Tag = new VariousData<int>(nAccumulLength);
                }
                else
                {
                    int nUnitLength = data.AccumulLength.Data - nAccumulLength;
                    nAccumulLength += nUnitLength;

                    SetIntData(row2.Cells[UNIT_LENGTH], nUnitLength);
                    row2.Cells[UNIT_LENGTH].Tag = new VariousData<int>(nUnitLength);
                }
            }

            datas.Clear();
        }

        private void PostCalcArea(int nAccumulArea, int nBeginIndex, List<ResultPropertyData> datas, int nDataCount, bool fromBegin)
        {
            if (nBeginIndex < 0)
                return;

            for (int i = nBeginIndex; i < nDataCount; i++)
            {
                ResultPropertyData data = datas[i];

                if (data.DirectionFromBegin == null || data.DirectionFromBegin.Data != fromBegin)
                    continue;

                if (data.UnitArea == null && data.AccumulArea == null)
                    break;

                int nRowIndex = (int)data.ProjectCost.Data;
                DataGridViewRow row2 = dataGridView1.Rows[nRowIndex];

                if (data.UnitArea != null)
                {
                    int nUnitArea = data.UnitArea.Data;
                    nAccumulArea += nUnitArea;

                    SetIntData(row2.Cells[ACCUMUL_AREA], nAccumulArea);
                    row2.Cells[ACCUMUL_AREA].Tag = new VariousData<int>(nAccumulArea);
                }
                else
                {
                    int nUnitArea = data.AccumulArea.Data - nAccumulArea;
                    nAccumulArea += nUnitArea;

                    SetIntData(row2.Cells[UNIT_AREA], nUnitArea);
                    row2.Cells[UNIT_AREA].Tag = new VariousData<int>(nUnitArea);
                }
            }

            datas.Clear();
        }

        private bool EndEditIntData(DataGridViewCell cell, string strItemName)
        {
            if (cell.Value != null && cell.Value.ToString().Length > 0)
            {
                int nData;

                if (!int.TryParse(cell.Value.ToString(), out nData))
                {
					string szMsg = strItemName + " 값이 잘못되었습니다.";
                    UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //MessageBox.Show(strItemName + " 값이 잘못되었습니다.");

                    if (cell.Tag == null)
                        cell.Value = null;
                    else
                    {
                        SetIntData(cell, (VariousData<int>)cell.Tag);
                    }

                    return false;
                }
                else
                {
                    SetIntData(cell, nData);
                    cell.Tag = new VariousData<int>(nData);
                }
            }
            else
            {
                cell.Value = null;
                cell.Tag = null;
            }

            return true;
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCellAddress.X == colStartDirection.DisplayIndex)
            {
                ComboBox cb = e.Control as ComboBox;

                if (cb != null)
                {
                    /*cb.Tag = dataGridView1.CurrentCell;

                    if (dataGridView1.CurrentCell != null)
                    {
                        if (dataGridView1.CurrentCell.Tag == null)
                        {
                            ComboBoxText text = new ComboBoxText();
                            text.Control = cb;
                            dataGridView1.CurrentCell.Tag = text;
                        }
                    }*/

                    cb.DropDownStyle = ComboBoxStyle.DropDownList;
                }
            }
        }

        private List<ResultPropertyData> MakeTempData4Calc()
        {
            List<ResultPropertyData> datas = new List<ResultPropertyData>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                ResultPropertyData data = new ResultPropertyData();

                data.BeginTime = (VariousData<DateTime>)row.Cells[BEGIN_TIME].Tag;
                data.EndTime = (VariousData<DateTime>)row.Cells[END_TIME].Tag;
                data.AccumulLength = (VariousData<int>)row.Cells[ACCUMUL_LENGTH].Tag;
                data.UnitLength = (VariousData<int>)row.Cells[UNIT_LENGTH].Tag;
                data.AccumulArea = (VariousData<int>)row.Cells[ACCUMUL_AREA].Tag;
                data.UnitArea = (VariousData<int>)row.Cells[UNIT_AREA].Tag;
                data.DirectionFromBegin = GetDirectionFromBegin(row);

                // 실제 DataGridViewRow와 링크시키기 위한 값
                data.ProjectCost = new VariousData<long>((long)row.Index);

                datas.Add(data);
            }

            datas.Sort();
            return datas;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (m_prop != null)
            {
                ReadPropertyDatas(m_prop);
                /*m_prop.PropertyDatas.Clear();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    ResultPropertyData data = new ResultPropertyData();

                    if (row.Cells[PROJECT_NAME].Value == null)
                        data.ProjectName = "";
                    else
                        data.ProjectName = row.Cells[PROJECT_NAME].Value.ToString();

                    data.BeginTime = (VariousData<DateTime>)row.Cells[BEGIN_TIME].Tag;
                    data.EndTime = (VariousData<DateTime>)row.Cells[END_TIME].Tag;
                    data.ProjectCost = (VariousData<long>)row.Cells[PROJECT_COST].Tag;
                    data.AccumulLength = (VariousData<int>)row.Cells[ACCUMUL_LENGTH].Tag;
                    data.UnitLength = (VariousData<int>)row.Cells[UNIT_LENGTH].Tag;
                    data.AccumulArea = (VariousData<int>)row.Cells[ACCUMUL_AREA].Tag;
                    data.UnitArea = (VariousData<int>)row.Cells[UNIT_AREA].Tag;

                    if (row.Cells[DIR_FROM_BEGIN].Value != null)
                        data.DirectionFromBegin = GetDirectionFromBegin(row);

                    m_prop.PropertyDatas.Add(data);
                }

                m_prop.Sort();*/

                if (m_frm != null && m_resultPropertyRow != null)
                {
                    m_frm.UpdateRow(m_resultPropertyRow, m_prop);
                }
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void ReadPropertyDatas(ResultProperty prop)
        {
            prop.PropertyDatas.Clear();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                ResultPropertyData data = new ResultPropertyData();

                if (row.Cells[PROJECT_NAME].Value == null)
                    data.ProjectName = "";
                else
                    data.ProjectName = row.Cells[PROJECT_NAME].Value.ToString();

                data.BeginTime = (VariousData<DateTime>)row.Cells[BEGIN_TIME].Tag;
                data.EndTime = (VariousData<DateTime>)row.Cells[END_TIME].Tag;
                data.ProjectCost = (VariousData<long>)row.Cells[PROJECT_COST].Tag;
                data.AccumulLength = (VariousData<int>)row.Cells[ACCUMUL_LENGTH].Tag;
                data.UnitLength = (VariousData<int>)row.Cells[UNIT_LENGTH].Tag;
                data.AccumulArea = (VariousData<int>)row.Cells[ACCUMUL_AREA].Tag;
                data.UnitArea = (VariousData<int>)row.Cells[UNIT_AREA].Tag;

                if (row.Cells[DIR_FROM_BEGIN].Value != null)
                    data.DirectionFromBegin = GetDirectionFromBegin(row);

                prop.PropertyDatas.Add(data);
            }

            prop.Sort();
        }

        private VariousData<bool> GetDirectionFromBegin(DataGridViewRow row)
        {
            if (row.Cells[DIR_FROM_BEGIN].Value == null)
                return null;

            return row.Cells[DIR_FROM_BEGIN].Value.ToString() == "시점으로부터" ? new VariousData<bool>(true) : new VariousData<bool>(false);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int nRowCount = dataGridView1.Rows.Count;
            if (nRowCount <= 1)
                return;

            DataGridViewRow row = dataGridView1.Rows[nRowCount - 2];
            row.Cells[0].Value = nRowCount - 1;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            ResultProperty prop = new ResultProperty();
            ReadPropertyDatas(prop);
            InitGrid(prop);
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (dataGridView1.CurrentRow == null)
                    return;

                if (dataGridView1.CurrentRow.IsNewRow)
                    return;

                string strRowName = "이";

                if (dataGridView1.CurrentRow.Cells[PROJECT_NAME].Value != null && dataGridView1.CurrentRow.Cells[PROJECT_NAME].Value.ToString().Length > 0)
                {
                    strRowName = "[" + dataGridView1.CurrentRow.Cells[PROJECT_NAME].Value.ToString() + "]";
                }

                string strMessage = string.Format("{0} 행을 삭제하시겠습니까?", strRowName);

                if (UnE.Utility.UMessageBox.Show(this, strMessage, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                {
                    dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == WindowMessage.WM_KEYDOWN ||
                msg.Msg == WindowMessage.WM_CHAR ||
                msg.Msg == WindowMessage.WM_SYSKEYDOWN)
            {
                if (keyData == Keys.F1)
                {
                    FormMain.Instance.ShowHelp();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
