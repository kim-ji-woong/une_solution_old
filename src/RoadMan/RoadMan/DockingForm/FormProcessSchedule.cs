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
    public partial class FormProcessSchedule : Form
    {
        private FormScheduleProperty m_frmProperty = null;
        private ProcessSchedule m_currentSchedule = null;
        private PanelDXFViewer m_panel = null;

        public ProcessSchedule CurrentSchedule
        {
            get { return m_currentSchedule; }
            set { m_currentSchedule = value; }
        }

        public List<ProcessSchedule> ProcessSchedules
        {
            get
            {
                List<ProcessSchedule> schedules = new List<ProcessSchedule>();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    if (row.Tag != null)
                        schedules.Add((ProcessSchedule)row.Tag);
                }

                return schedules;
            }
        }

        public FormProcessSchedule(PanelDXFViewer panel)
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
            colTotal.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colTotal.HeaderText = Options.Instance.CompleteRatioByArea ? "총면적(m²)" : "총길이(m)";
        }


		private bool m_bAddRowWidthFullData = false;
		public void AddRowWidthFullData(ProcessSchedule schedule, ProcessResult result)
		{
			m_bAddRowWidthFullData = true;
			bool temp = dataGridView1.AllowUserToAddRows;
			dataGridView1.AllowUserToAddRows = true;
			int nRowCount = dataGridView1.Rows.Count;

			DataGridViewRow row = new DataGridViewRow();
			schedule.ParentPane = m_panel;
			schedule.PanelName = m_panel.GetHashCode().ToString();
			row.Tag = schedule;

			DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
			cell1.Value = nRowCount;
			row.Cells.Add(cell1);

			DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
			cell4.Value = schedule.ScheduleName;
			row.Cells.Add(cell4);

			DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
			int nTotal = schedule.Totali;

			if (nTotal == 0)
				cell2.Value = null;
			else
			{
				cell2.Value = string.Format("{0:###,###,###,###,###,###}", nTotal);
			}
			row.Cells.Add(cell2);

			DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
			cell3.Value = schedule.Description;
			row.Cells.Add(cell3);

			dataGridView1.Rows.Add(row);

			result.ProcessSchedule = schedule;
			result.ParentPane = m_panel;
			result.PanelName = m_panel.GetHashCode().ToString();
			result.ScheduleHash = schedule.GetHashCode().ToString();

			m_panel.ProcessResultForm.AddProcessResult(result);
			m_panel.ProcessResultForm.UpdateProcessSchedule(schedule);
			
			m_bAddRowWidthFullData = false;
			dataGridView1.AllowUserToAddRows = temp;
		}




		private bool m_bCreatedEdit = false;
        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
			if (m_bAddRowWidthFullData == true)
				return;


            int nRowCount = dataGridView1.Rows.Count;
            if (nRowCount <= 1)
                return;

            DataGridViewRow row = dataGridView1.Rows[nRowCount - 2];
            row.Cells[0].Value = nRowCount - 1;

			ProcessSchedule schedule = (ProcessSchedule)row.Tag;

			m_bCreatedEdit = false;
			if (row.Tag == null)
			{
				
				schedule = new ProcessSchedule();

				schedule.ParentPane = m_panel;
				schedule.PanelName = m_panel.GetHashCode().ToString();
				row.Tag = schedule;

				m_bCreatedEdit = true;

				UndoRedoObjectManager.Instance.TempSchedule = schedule;
				//UndoRedoObjectManager.Instance.AddUndoRedoDataForRegister(schedule);
			}
			
			ProcessResult result = new ProcessResult(schedule, null, "");
			result.ParentPane = m_panel;
			result.PanelName = m_panel.GetHashCode().ToString();
			result.ScheduleHash = schedule.Hash.ToString();

			if (m_bCreatedEdit == true)
			{
				UndoRedoObjectManager.Instance.TempResult = result;
			}
			else
				UndoRedoObjectManager.Instance.AddUndoRedoDataForRegister(result);
			
			
			m_panel.ProcessResultForm.AddProcessResult(result);	
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && dataGridView1.CurrentRow != null)
            {
                ProcessSchedule schedule = (ProcessSchedule)dataGridView1.CurrentRow.Tag;

                if (schedule != null)
                {
                    Rectangle rect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    ShowMenu(e.X + rect.Left, e.Y + rect.Top);
                }
            }
        }

        /*private void GetCellLocation(int nRowIndex, int nColumnIndex, out int x, out int y)
        {
            x = 0;
            y = dataGridView1.ColumnHeadersHeight;

            for (int i=0;i<nRowIndex-1;i++)
            {
                y += dataGridView1.Rows[i].Height;
            }

            for (int i=0;i<nColumnIndex-1;i++)
            {
                x += dataGridView1.Columns[i].Width;
            }
        }*/

        private void menuProperty_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                ProcessSchedule schedule = (ProcessSchedule)dataGridView1.CurrentRow.Tag;

                if (schedule != null)
                {
                    if (m_frmProperty != null && m_frmProperty.Visible)
                        m_frmProperty.Close();

                    m_frmProperty = new FormScheduleProperty(schedule);
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

        public ProcessSchedule GetCurrentVisibleSchedule()
        {
            if (m_frmProperty == null || !m_frmProperty.Visible)
                return null;

            return m_frmProperty.Schedule;
        }

        private void menuInsertSector_Click(object sender, EventArgs e)
        {
            //FormMain.Instance.Activity = FormMain.ActivityType.ADD_SECTION;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int nRowIndex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow row = dataGridView1.Rows[nRowIndex];

                if (row.IsNewRow)
                    m_currentSchedule = null;
                else
                    m_currentSchedule = (ProcessSchedule)row.Tag;
            }
            else
                m_currentSchedule = null;
        }

        private void ShowMenu(int x, int y)
        {
            /*if (m_currentSchedule != null)
            {
                menuDeleteSector.Enabled = true;
                menuEditSector.Enabled = true;
                menuInsertSector.Enabled = true;
            }
            else
            {
                menuDeleteSector.Enabled = false;
                menuEditSector.Enabled = false;
                menuInsertSector.Enabled = false;
            }*/

            contextMenuStrip1.Show(dataGridView1, x, y);
        }

        public void ClearProcessSchedule()
        {
            dataGridView1.Rows.Clear();
        }

		
        public void AddProcessSchedule(ProcessSchedule schedule)
        {			
			MakeNewRow(schedule);
			
            DataGridViewRow row = dataGridView1.Rows[dataGridView1.Rows.Count - 2];

            if (row.Tag != null)
            {
                m_panel.ProcessResultForm.ChangeSchedule((ProcessSchedule)row.Tag, schedule);
            }

            row.Tag = schedule;
            int nTotal = schedule.Totali;

            if (nTotal == 0)
                row.Cells[2].Value = null;
            else
            {
                row.Cells[2].Value = string.Format("{0:###,###,###,###,###,###}", nTotal);
            }

            row.Cells[0].Value = row.Index + 1;
            row.Cells[1].Value = schedule.ScheduleName;
            row.Cells[3].Value = schedule.Description;

            /*DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = dataGridView1.Rows.Count + 1;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = schedule.ScheduleName;
            row.Cells.Add(cell);

            DataGridViewComboBoxCell cell2 = new DataGridViewComboBoxCell();
            cell2.Value = null;
            row.Cells.Add(cell2);

            cell = new DataGridViewTextBoxCell();
            cell.Value = schedule.Description;
            row.Cells.Add(cell);

            dataGridView1.Rows.Add(row);
            row.Tag = schedule;*/
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
			

			if (m_bAddRowWidthFullData == true)
				return;

            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            if (row.IsNewRow || row.Tag == null)
                return;

            ProcessSchedule schedule = (ProcessSchedule)row.Tag;

            if (e.ColumnIndex == 1)
			{
				string szTemp = row.Cells[e.ColumnIndex].Value == null ? "" : row.Cells[e.ColumnIndex].Value.ToString();

				if( szTemp != schedule.ScheduleName)
				{
					UndoRedoManager.Instance.SaveSnapshot("집행계획 수정 - 이름");

					ProcessSchedule scheduleTemp = UndoRedoObjectManager.Instance.TempSchedule;
					if (scheduleTemp != null && scheduleTemp == schedule)
					{						
						ProcessResult resultTemp = UndoRedoObjectManager.Instance.TempResult;
						if(resultTemp != null)
						{
							UndoRedoObjectManager.Instance.AddUndoRedoDataForRegister(scheduleTemp);
							UndoRedoObjectManager.Instance.AddUndoRedoDataForRegister(resultTemp);
						}
					}					
					schedule.ScheduleName = szTemp;
					m_panel.ProcessResultForm.UpdateProcessSchedule(schedule);
				}
				
			} 
			else if (e.ColumnIndex == 3)
			{
				string szTemp = row.Cells[e.ColumnIndex].Value == null ? "" : row.Cells[e.ColumnIndex].Value.ToString();
				if( szTemp != schedule.Description)
				{
					UndoRedoManager.Instance.SaveSnapshot("집행계획 수정 - 설명");

					ProcessSchedule scheduleTemp = UndoRedoObjectManager.Instance.TempSchedule;
					if (scheduleTemp != null && scheduleTemp == schedule)
					{
						ProcessResult resultTemp = UndoRedoObjectManager.Instance.TempResult;
						if (resultTemp != null)
						{
							UndoRedoObjectManager.Instance.AddUndoRedoDataForRegister(scheduleTemp);
							UndoRedoObjectManager.Instance.AddUndoRedoDataForRegister(resultTemp);
						}
					}					
					schedule.Description = szTemp;
				}
			}				
			else
			{
				
				return;
			}


			m_bCreatedEdit = false;
        }

        public void SetSectors(DXFViewer.DXFControl ctrl)
        {
            DXFExternPainter painter = (DXFExternPainter)ctrl.ExternalPainter;
            DXFViewer.Layer layerProcessSchedule = painter.GetLayer(DXFExternPainter.LayerType.PROCESS_SCHEDULE);

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                ProcessSchedule schedule = (ProcessSchedule)row.Tag;

                if (schedule == null)
                    continue;

                foreach (ScheduleProperty prop in schedule.Properties)
                {
                    foreach (SchedulePropertySector sector in prop.Sectors)
                    {
                        SchedulePropertySector_4_Read sector2 = (SchedulePropertySector_4_Read)sector;

                        if (sector2.LayerIndex < 0 || sector2.ShapeIndex < 0)
                            continue;

                        DXFViewer.Layer layer = (DXFViewer.Layer)ctrl.Layers[sector2.LayerIndex];
                        DXFViewer.Shape shape = (DXFViewer.Shape)layer.Shapes[sector2.ShapeIndex];

                        sector2.Shape = shape;

                        if (sector2.Hatch != null)
                        {
                            layerProcessSchedule.Add(sector2.Hatch);
                            sector2.Hatch.LinkedScheduleProperty = prop;

                            sector2.Hatch.Selectable = true;
                            sector2.Hatch.Visible = false;
                        }
                    }
                }
            }
        }

        public void SelectSchedule(string strScheduleName)
        {
            dataGridView1.ClearSelection();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells[1].Value == null)
                    continue;

                if (row.Cells[1].Value.ToString() == strScheduleName)
                {
                    row.Cells[1].Selected = true;
                    return;
                }
            }
        }

        public ProcessSchedule FindProcessSchedule(string strScheduleName)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells[1].Value == null)
                    continue;

                if (row.Cells[1].Value.ToString() == strScheduleName)
                {
                    return (ProcessSchedule)row.Tag;
                }
            }

            return null;
        }

        public void SelectScheduleProperty(ScheduleProperty prop)
        {
            if (m_frmProperty == null || !m_frmProperty.Visible)
            {
                m_frmProperty = new FormScheduleProperty(prop.Schedule);
				DialogFormFrame frameProperty = new DialogFormFrame(m_frmProperty);
				frameProperty.Sizable = true;
				frameProperty.MinimizeBox = true;
				frameProperty.MaximizeBox = true;
				frameProperty.ShowMaxButton = true;
				frameProperty.ShowMinButton = true;
				frameProperty.Show(this);
            }

            m_frmProperty.SelectScheduleProperty(prop);
        }

        public void CloseScheduleProperty()
        {
            if (m_frmProperty != null && m_frmProperty.Visible)
            {
                m_frmProperty.Close();
                m_frmProperty = null;
            }
        }

        /*private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCellAddress.X == colTotal.DisplayIndex)
            {
                ComboBox cb = e.Control as ComboBox;

                if (cb != null)
                {
                    if (cb.Tag == null)
                    {
                        //cb.TextChanged += new EventHandler(this.ComboBoxCell_TextChanged);
                    }

                    cb.Tag = dataGridView1.CurrentCell;

                    if (dataGridView1.CurrentCell != null)
                    {
                        if (dataGridView1.CurrentCell.Tag == null)
                        {
                            ComboBoxText text = new ComboBoxText();
                            text.Control = cb;
                            dataGridView1.CurrentCell.Tag = text;
                        }
                    }

                    cb.DropDownStyle = ComboBoxStyle.DropDown;
                }
            }
        }*/

        /*private void ComboBoxCell_TextChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;

            if (cbo.Tag == null)
                return;

            DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)cbo.Tag;

            if (cell.Tag == null)
                return;

            ComboBoxText text = (ComboBoxText)cell.Tag;
            text.Text = cbo.Text;

            System.Diagnostics.Trace.WriteLine("ComboBox_TextChanged : " + cbo.Text);
        }*/

        /*private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (cell.GetType() == typeof(DataGridViewComboBoxCell))
            {
                if (cell.Tag == null)
                    return;

                ComboBoxText text = (ComboBoxText)cell.Tag;
                string strText = text.Control.Text;

                if (strText.Length == 0)
                    cell.Value = null;
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
            }
        }*/

        private void MakeNewRow(ProcessSchedule ps = null)
        {
            if (dataGridView1.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
				if (ps != null)
					row.Tag = ps;
                dataGridView1.Rows.Add(row);
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
				if (ps != null)
					row.Tag = ps;
                dataGridView1.Rows.Add(row);

                dataGridView1.AllowUserToAddRows = false;
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (dataGridView1.CurrentRow == null)
                    return;

                if (dataGridView1.CurrentRow.IsNewRow)
                    return;

                menuDelete_Click(null, null);
            }
        }

        private void menuDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                string strScheduleName = "이";

                if (dataGridView1.CurrentRow.Cells[1].Value != null)
                    strScheduleName = "[" + dataGridView1.CurrentRow.Cells[1].Value.ToString() + "]";

                if (UnE.Utility.UMessageBox.Show(this, strScheduleName + " 행을 정말 지우시겠습니까?\r\n연관된 모든 데이터가 삭제됩니다.", "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == System.Windows.Forms.DialogResult.Yes)
                {

					

					UndoRedoManager.Instance.SaveSnapshot("집행계획 삭제");

                    ProcessSchedule schedule = (ProcessSchedule)dataGridView1.CurrentRow.Tag;
                    m_panel.ProcessResultForm.RemoveProcessSchedule(schedule);

                    dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
					schedule.Dispose();

					ReArrageRowIndex();
                }
            }
        }

		private void ReArrageRowIndex()
		{
			int nCount = 1;
			foreach(DataGridViewRow row in dataGridView1.Rows)
			{
				if (!row.IsNewRow)
				{
					row.Cells[0].Value = nCount++;
					
				}
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			FormMain.Instance.HideProcessScheduleForm();
		}

        public void UpdateProcessSchedule(ProcessSchedule schedule)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Tag == schedule)
                {
                    int nTotal = schedule.Totali;

                    if (nTotal == 0)
                        row.Cells[2].Value = null;
                    else
                        row.Cells[2].Value = string.Format("{0:###,###,###,###,###,###}", nTotal);

                    m_panel.ProcessResultForm.UpdateProcessSchedule(schedule);
                    break;
                }
            }
        }

		public void ReplaceProcessSchedule(ProcessSchedule schedule)
		{
			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				if (row.IsNewRow)
					continue;

				if (row.Tag == schedule)
				{
					int nTotal = schedule.Totali;

					if (nTotal == 0)
						row.Cells[2].Value = null;
					else
						row.Cells[2].Value = string.Format("{0:###,###,###,###,###,###}", nTotal);


					row.Cells[1].Value = schedule.ScheduleName;
					row.Cells[3].Value = schedule.Description;

					m_panel.ProcessResultForm.UpdateProcessSchedule(schedule);
					break;
				}
			}
		}

		public void DeleteProcessSchedule(ProcessSchedule schedule)
		{
			DataGridViewRow targetRow = null;
			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				if (row.IsNewRow)
					continue;

				if (row.Tag == schedule)
				{
					targetRow = row;

					
					break;
				}
			}

			if (targetRow != null)
			{
				m_panel.ProcessResultForm.RemoveProcessSchedule(schedule);
				dataGridView1.Rows.Remove(targetRow);
			}
		}

        public void SetCompleteRatio(bool ratioByArea)
        {
            string strHeaderText = ratioByArea ? "총면적(m²)" : "총길이(m)";

            if (strHeaderText == colTotal.HeaderText)
                return;

            colTotal.HeaderText = strHeaderText;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow || row.Tag == null)
                    continue;

                ProcessSchedule schedule = (ProcessSchedule)row.Tag;

                int nTotal = ratioByArea ? (int)schedule.TotalArea : schedule.TotalLength;

                if (nTotal == 0)
                    row.Cells[2].Value = null;
                else
                    row.Cells[2].Value = string.Format("{0:###,###,###,###,###,###}", nTotal);

                m_panel.ProcessResultForm.UpdateProcessSchedule(schedule);
            }
        }

    }
}