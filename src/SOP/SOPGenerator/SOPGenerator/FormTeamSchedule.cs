using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPGen
{
    public partial class FormTeamSchedule : Form
    {
        private ArrayList m_arrBeginTimes = new ArrayList();
        private TeamData m_data = null;
        private bool m_isTeam = true;
        private int m_nHour = -1, m_nMinute = -1;
        private string m_strDesc = null;
        private int m_nSelectedRowIndex = -1, m_nSelectedColIndex = -1;

        public FormTeamSchedule(bool isTeam)
        {
            m_isTeam = isTeam;
            InitializeComponent();
        }

        public void SelectCell(int nRow, int nCol)
        {
            m_nSelectedRowIndex = nRow;
            m_nSelectedColIndex = nCol;
        }

        private void _SelectCell(int nRow, int nCol)
        {
            int nColCount = teamScheduleDataGrid.Columns.Count;
            int nRowCount = teamScheduleDataGrid.Rows.Count;
            if (nRow >= nRowCount || nCol >= nColCount) return;

            DataGridViewRow row = teamScheduleDataGrid.Rows[nRow];
            row.Cells[nCol].Selected = true;
        }

        private void hourTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void minuteTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        public void SetDescription(string strDesc)
        {
            m_strDesc = strDesc;
        }

        public void SetData(TeamData data)
        {
            m_data = data;
        }

        public void AddBeginTime(string strBeginTime)
        {
            if (m_arrBeginTimes.Contains(strBeginTime))
                return;

            m_arrBeginTimes.Add(strBeginTime);
        }

        public int GetDataCount()
        {
            return m_arrBeginTimes.Count;
        }

        private void FormTeamSchedule_Load(object sender, EventArgs e)
        {
            if (!m_isTeam)
            {
                this.Text = "중복된 팀의 스케쥴 설정";
                teamScheduleDataGrid.Columns[1].HeaderText = "팀원 이름";
            }

            if (m_strDesc != null)
            {
                textBox1.Text = m_strDesc;
            }

            if (m_data != null)
            {
                foreach (string strTime in m_arrBeginTimes)
                {
                    DataGridViewRow gridRow = new DataGridViewRow();
                    DataGridViewCell cell = new DataGridViewTextBoxCell();

                    cell.Value = strTime;
                    gridRow.Cells.Add(cell);

                    cell = new DataGridViewTextBoxCell();

                    cell.Value = m_data.FullName;
                    gridRow.Cells.Add(cell);

                    teamScheduleDataGrid.Rows.Add(gridRow);
                }

                teamScheduleDataGrid.ClearSelection();

                if (m_nSelectedColIndex >= 0 && m_nSelectedRowIndex >= 0)
                    _SelectCell(m_nSelectedRowIndex, m_nSelectedColIndex);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            int nHour = int.Parse(hourTextBox.Text);
            int nMinute = int.Parse(minuteTextBox.Text);
            string strTime = string.Format("{0:00}:{1:00}", nHour, nMinute);

            if (m_data != null)
            {
                foreach (string strTime2 in m_arrBeginTimes)
                {
                    if (strTime == strTime2)
                    {
                        MessageBox.Show("이미 동일한 시작 시간이 존재합니다.\r\n시작 시간을 변경해 주세요.");
                        return;
                    }
                }
            }
            else
                return;

            m_nHour = nHour;
            m_nMinute = nMinute;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public void GetBeginTime(out int nHour, out int nMinute)
        {
            nHour = m_nHour;
            nMinute = m_nMinute;
        }

        private void hourTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSelect_Click(null, null);
        }

        private void minuteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSelect_Click(null, null);
        }

        private void FormTeamSchedule_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSelect_Click(null, null);
        }
    }
}
