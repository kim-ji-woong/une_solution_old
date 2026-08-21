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
    public partial class FormProcessResult : Form
    {
        private PanelDXFViewer m_panel = null;
        private FormResultProperty m_frmProperty = null;

        public List<ProcessResult> ProcessResults
        {
            get
            {
                List<ProcessResult> results = new List<ProcessResult>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    if (row.Tag != null)
                        results.Add((ProcessResult)row.Tag);
                }

                return results;
            }
        }

        public FormProcessResult(PanelDXFViewer panel)
        {
            InitializeComponent();

            m_panel = panel;
            InitGrid();

			Color backColor = Color.FromArgb(75, 71, 86);
			Color textColor = Color.White;
			UnE.Utility.CustomMenuHelper helper = new UnE.Utility.CustomMenuHelper(FormMain.Instance);
			helper.MakeCustomLookMenu(contextMenuStrip1, backColor, textColor);
        }

        private void InitGrid()
        {
            colNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colETC.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colLevel.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colRatio.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int nRowCount = dataGridView1.Rows.Count;
            if (nRowCount <= 1)
                return;

            DataGridViewRow row = dataGridView1.Rows[nRowCount - 2];
            row.Cells[0].Value = nRowCount - 1;
        }

        private DataGridViewRow AddRow(ProcessResult result)
        {
            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = dataGridView1.Rows.Count + 1;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            row.Cells.Add(cell);

            if (result.ProcessSchedule != null)
            {
                //cell.Value = result.ProcessSchedule.Length;

                if (result.ProcessSchedule.Totald == 0.0)
                    cell.Value = null;
                else
                    cell.Value = string.Format("{0:F1}", result.Total / result.ProcessSchedule.Totald * 100);
            }

            cell = new DataGridViewTextBoxCell();
            row.Cells.Add(cell);

            if (result.ProcessSchedule != null)
            {
                cell.Value = result.ProcessSchedule.Description;
            }

            dataGridView1.Rows.Add(row);
            row.Tag = result;

            return row;
        }

        public void ClearProcessResult()
        {
            dataGridView1.Rows.Clear();
        }

        public void AddProcessResult(ProcessResult result)
        {
            if (result != null)
            {
                SyncData(result);
                AddRow(result);
            }
        }

        public void RemoveProcessSchedule(ProcessSchedule schedule)
        {
            if (schedule == null)
                return;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                ProcessResult result = (ProcessResult)row.Tag;

                if (result != null)
                {
                    if (result.ProcessSchedule == schedule)
                    {
                        dataGridView1.Rows.Remove(row);
						UndoRedoObjectManager.Instance.RemoveUndoRedoDataForRegister(result);
                        break;
                    }
                }
            }

			ReArrageRowIndex();
        }


		private void ReArrageRowIndex()
		{
			int nCount = 1;
			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				if (!row.IsNewRow)
				{
					row.Cells[0].Value = nCount++;

				}
			}
		}


        // ProcessSchedule과 데이터를 동기화시킨다.
        private void SyncData(ProcessResult result)
        {
            if (result.ProcessSchedule != null)
            {
                foreach (ScheduleProperty prop in result.ProcessSchedule.Properties)
                {
                    if (FindResultProperty(prop, result) == null)
                    {
                        ResultProperty prop2 = new ResultProperty();
                        prop2.ScheduleProperty = prop;
                        result.ResultProperties.Add(prop2);
                    }
                }
            }
        }

        private ResultProperty FindResultProperty(ScheduleProperty prop, ProcessResult result)
        {
            foreach (ResultProperty prop2 in result.ResultProperties)
            {
                if (prop2.ScheduleProperty == prop)
                    return prop2;
            }

            return null;
        }

        public void UpdateProcessSchedule(ProcessSchedule schedule)
        {
            if (schedule == null)
                return;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                ProcessResult result = (ProcessResult)row.Tag;

                if (result != null)
                {
                    if (result.ProcessSchedule == schedule)
                    {
                        SyncData(result);
                        UpdateProcessSchedule(schedule, row);
                        break;
                    }
                }
            }
        }

        private void UpdateProcessSchedule(ProcessSchedule schedule, DataGridViewRow row)
        {
            row.Cells[1].Value = schedule.ScheduleName;
            //row.Cells[2].Value = schedule.Length;

            ProcessResult result = (ProcessResult)row.Tag;

            if (result != null)
            {
                double dTotalSchedule = schedule.Totald;

                if (dTotalSchedule == 0.0)
                    row.Cells[2].Value = null;
                else
                    row.Cells[2].Value = string.Format("{0:F1}", result.Total / schedule.Totald * 100);
            }
            else
                row.Cells[2].Value = null;
        }

        public void UpdateProcessResult(ProcessResult result)
        {
            if (result == null)
                return;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Tag == result)
                {
                    if (result.ProcessSchedule != null)
                        UpdateProcessSchedule(result.ProcessSchedule, row);

                    row.Cells[3].Value = result.Description;
                    break;
                }
            }
        }

        private void menuProperty_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                ProcessResult result = (ProcessResult)dataGridView1.CurrentRow.Tag;

                if (result != null)
                {
                    if (m_frmProperty != null && m_frmProperty.Visible)
                        m_frmProperty.Close();

                    m_frmProperty = new FormResultProperty(result, m_panel);
					m_frmProperty.TopMost = true;

					DialogFormFrame frameProperty = new DialogFormFrame(m_frmProperty);
					frameProperty.Sizable = true;
					frameProperty.MinimizeBox = true;
					frameProperty.MaximizeBox = true;
					frameProperty.ShowMaxButton = true;
					frameProperty.ShowMinButton = true;
					frameProperty.Show(this);
                }
            }
        }

        public ProcessResult FindProcessResult(ProcessSchedule schedule)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Tag != null)
                {
                    ProcessResult result = (ProcessResult)row.Tag;

                    if (result.ProcessSchedule == schedule)
                        return result;
                }
            }

            return null;
        }

        public void ChangeSchedule(ProcessSchedule scheduleSrc, ProcessSchedule scheduleTrg)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Tag != null)
                {
                    ProcessResult result = (ProcessResult)row.Tag;

                    if (result.ProcessSchedule == scheduleSrc)
                    {
                        result.ProcessSchedule = scheduleTrg;
                        UpdateProcessSchedule(scheduleTrg, row);
                        break;
                    }
                }
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && dataGridView1.CurrentRow != null)
            {
                ProcessResult result = (ProcessResult)dataGridView1.CurrentRow.Tag;

                if (result != null)
                {
                    Rectangle rect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    contextMenuStrip1.Show(dataGridView1, e.X + rect.Left, e.Y + rect.Top);
                }
            }
        }

		private void button1_Click(object sender, EventArgs e)
		{
			FormMain.Instance.HideProcessResultForm();
		}

        public void ClosePropertyForm()
        {
            if (m_frmProperty != null && m_frmProperty.Visible)
            {
                m_frmProperty.Close();
                m_frmProperty = null;
            }
        }

        public void RemoveScheduleProperty(ScheduleProperty prop)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Tag != null)
                    continue;

                ProcessResult result = (ProcessResult)row.Tag;

                if (result.ProcessSchedule != prop.Schedule)
                    continue;

                foreach (ResultProperty prop2 in result.ResultProperties)
                {
                    if (prop2.ScheduleProperty == prop)
                    {
                        result.ResultProperties.Remove(prop2);
                        break;
                    }
                }

                break;
            }
        }

        public void SetCompleteRatio(bool ratioByArea)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            ProcessResult result = (ProcessResult)row.Tag;

            if (result == null)
                return;

			
			if (e.ColumnIndex == 3)
			{
				if (result.Description != cell.Value.ToString())
				{
					UndoRedoManager.Instance.SaveSnapshot("집행진행상황 수정");
					result.Description = cell.Value.ToString();
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
