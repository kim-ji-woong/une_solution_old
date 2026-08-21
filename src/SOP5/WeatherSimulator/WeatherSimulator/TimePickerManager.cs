using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace WeatherSimulator
{
    public class TimePickerManager
    {
        private DateTimePicker m_timePicker = null;
        private DataGridView m_grid = null;
        private int m_nTimeIndex = -1;

        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem tsMenuRemoveData;

        public TimePickerManager(DataGridView grid, int nTimeIndex, bool noCellClickEvent = false)
        {
            m_grid = grid;
            m_nTimeIndex = nTimeIndex;

            m_timePicker = new DateTimePicker();
            grid.Controls.Add(m_timePicker);
            m_timePicker.Visible = false;

            InitContextMenu();

            m_timePicker.CloseUp += new EventHandler(DateTimePicker_CloseUp);
            m_timePicker.LostFocus += new EventHandler(DateTimePicker_LostFocus);

            grid.Scroll += new System.Windows.Forms.ScrollEventHandler(OnScroll);
            grid.Sorted += new System.EventHandler(this.OnSorted);

            if (!noCellClickEvent)
                grid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(OnCellClick);
                //grid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(OnCellClick);
        }

        private void InitContextMenu()
        {
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.tsMenuRemoveData = new System.Windows.Forms.ToolStripMenuItem();

            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuRemoveData});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(115, 48);

            this.tsMenuRemoveData.Name = "tsMenuRemoveData";
            this.tsMenuRemoveData.Size = new System.Drawing.Size(114, 22);
            this.tsMenuRemoveData.Text = "삭제";
            this.tsMenuRemoveData.Click += new System.EventHandler(this.tsMenuRemoveData_Click);
        }

        private void tsMenuRemoveData_Click(object sender, EventArgs e)
        {
            List<int> removeRowIndecs = new List<int>();

            foreach (DataGridViewCell cell in m_grid.SelectedCells)
            {
                DataGridViewRow row = m_grid.Rows[cell.RowIndex];

                if (row.IsNewRow)
                    continue;

                if (!removeRowIndecs.Contains(row.Index))
                    removeRowIndecs.Add(row.Index);
            }

            if (removeRowIndecs.Count > 0)
            {
                //if (MessageBox.Show("삭제할까요?", "확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    removeRowIndecs.Sort();
                    RemoveRows(removeRowIndecs);
                }
            }
        }

        private void RemoveRows(List<int> removeRowIndecs)
        {
            int nIndexCount = removeRowIndecs.Count;

            for (int i = nIndexCount - 1; i >= 0; i--)
            {
                int nRowIndex = removeRowIndecs[i];
                m_grid.Rows.RemoveAt(nRowIndex);
            }

            ResetIndeces();
        }

        // 정렬이 끝난후 행번호를 새로 지정한다.
        private void OnSorted(object sender, EventArgs e)
        {
            ResetIndeces();
        }

        public void ResetIndeces()
        {
            foreach (DataGridViewRow row in m_grid.Rows)
            {
                if (row.IsNewRow)
                    continue;

                row.Cells[0].Value = row.Index + 1;
            }
        }

        private void DateTimePicker_LostFocus(object sender, EventArgs e)
        {
            DateTimePicker_CloseUp(null, null);
        }

        private void DateTimePicker_CloseUp(object sender, EventArgs e)
        {
            string strTime = GetTimeString();

            if (m_timePicker.Tag != null)
            {
                DataGridViewCell cell = (DataGridViewCell)m_timePicker.Tag;
                cell.Value = strTime;

                if (cell.Tag == null)
                    cell.Tag = new VariousData<DateTime>(m_timePicker.Value);
                else
                    ((VariousData<DateTime>)cell.Tag).Data = m_timePicker.Value;

                m_timePicker.Hide();
            }
        }

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            m_timePicker.Hide();
        }

        public void OnCellClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (m_timePicker == null || e.RowIndex < 0 || m_grid.ReadOnly)
                return;

            m_timePicker.Hide();

            if (e.Button == MouseButtons.Left)
            {
                if (e.ColumnIndex == m_nTimeIndex)
                {
                    DataGridViewRow row = m_grid.Rows[e.RowIndex];
                    DataGridViewCell cell = row.Cells[e.ColumnIndex];

                    Rectangle rect = m_grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                    if (cell.Value != null)
                    {
                        DateTime date = new DateTime();

                        if (GetDateTime(cell.Value.ToString(), ref date))
                        {
                            m_timePicker.Value = date;
                        }
                    }
                    else
                    {
                        cell.Value = GetTimeString();
                        cell.Tag = new VariousData<DateTime>(m_timePicker.Value);
                    }

                    m_timePicker.Location = new Point(rect.Left, rect.Top);
                    m_timePicker.Tag = cell;

                    m_timePicker.Size = new Size(rect.Width, rect.Height);

                    m_timePicker.Format = DateTimePickerFormat.Custom;
                    m_timePicker.CustomFormat = "HH:mm:ss";
                    m_timePicker.Show();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                System.Drawing.Rectangle rect = m_grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                contextMenuStrip1.Show(m_grid, e.X + rect.Left, e.Y + rect.Top);
            }
        }

        public static bool GetDateTime(string strText, ref DateTime date)
        {
            string[] arrTokens = ParseDateTime(strText);

            if (arrTokens == null)
                return false;

            int nYear = 0, nMonth = 0, nDay = 0;
            int nHour = 0, nMinute = 0, nSecond = 0;

            int nTokenCount = arrTokens.Count();

            if (nTokenCount < 3)
                return false;

            for (int i = 0; i < nTokenCount; i++)
            {
                if (i == 0)
                {
                    if (!int.TryParse(arrTokens[i], out nYear))
                        return false;
                }
                else if (i == 1)
                {
                    if (!int.TryParse(arrTokens[i], out nMonth))
                        return false;
                }
                else if (i == 2)
                {
                    if (!int.TryParse(arrTokens[i], out nDay))
                        return false;
                }
                else if (i == 3)
                {
                    if (!int.TryParse(arrTokens[i], out nHour))
                        return false;
                }
                else if (i == 4)
                {
                    if (!int.TryParse(arrTokens[i], out nMinute))
                        return false;
                }
                else if (i == 5)
                {
                    if (!int.TryParse(arrTokens[i], out nSecond))
                        return false;
                }
            }

            if (nTokenCount < 5)
                date = new DateTime(nYear, nMonth, nDay);
            else
                date = new DateTime(nYear, nMonth, nDay, nHour, nMinute, nSecond);

            return true;
        }

        private static string[] ParseDateTime(string strDate)
        {
            string[] arrTokens = strDate.Trim().Split(' ');

            int nTokenCount = arrTokens.Count();

            if (nTokenCount == 1)
            {
                return arrTokens[0].Trim().Split('-');
            }
            else if (nTokenCount == 2)
            {
                string[] arrDate = arrTokens[0].Trim().Split('-');
                string[] arrTime = arrTokens[1].Trim().Split(':');

                int nDateCount = arrDate.Count();
                int nTimeCount = arrTime.Count();

                arrTokens = new string[nDateCount + nTimeCount];

                for (int i = 0; i < nDateCount; i++)
                {
                    arrTokens[i] = arrDate[i];
                }

                for (int i = 0; i < nTimeCount; i++)
                {
                    arrTokens[nDateCount + i] = arrTime[i];
                }

                return arrTokens;
            }

            return null;
        }

        protected string GetTimeString()
        {
            return WeatherData.MakeTimeString(m_timePicker.Value);
        }
    }
}
