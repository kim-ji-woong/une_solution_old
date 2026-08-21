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
    public partial class FormScheduleProperty : Form, IExcelGridLinker
    {
        private const int STREET_NAME = 1;
        private const int IMPORTANCE = 2;
        //private const int WIDTH = 3;
        private const int AREA = 3;
        private const int LENGTH = 4;
        private const int LAND_ADDRESS = 5;
        private const int FINAL_DATE = 6;
        private const int TOTAL_COST = 7;
        private const int CHECK_COMPLETE = 8;

        private DateTimePicker m_timePicker = null;
        private FormImportance m_frmImportance = null;
        private FormLandNumber m_frmLandNumber = null;

        private List<MovingSchedule> m_movingSchedules = new List<MovingSchedule>();

        public FormImportance ImportanceForm
        {
            set { m_frmImportance = value; }
        }

        private ProcessSchedule m_schedule = null;
        public ProcessSchedule Schedule
        {
            get { return m_schedule; }
            set { m_schedule = value; }
        }

        public FormLandNumber LandNumberForm
        {
            set { m_frmLandNumber = value; }
        }

        public FormScheduleProperty(ProcessSchedule schedule = null)
        {
            InitializeComponent();

            m_schedule = schedule;
        }

        private void ReadRow(DataGridViewRow row, ScheduleProperty prop)
        {
            if (row.Cells[STREET_NAME].Value == null)
                prop.StreetName = "";
            else
                prop.StreetName = row.Cells[STREET_NAME].Value.ToString();

            if (row.Cells[IMPORTANCE].Value != null && row.Cells[IMPORTANCE].Tag != null)
            {
                ImportanceData importanceData = (ImportanceData)row.Cells[IMPORTANCE].Tag;
                prop.Importance = importanceData;
            }
            /*if (row.Cells[2].Value != null)
            {
                if (double.TryParse(row.Cells[2].Value.ToString(), out dImportance))
                    prop.Importance = new VariousData<double>(dImportance);
                else
                    prop.Importance = null;
            }
            else
                prop.Importance = null;*/

            //int nLength;
            //double dArea;

            if (row.Cells[AREA].Tag != null)
            {
                prop.Area = (VariousData<double>)row.Cells[AREA].Tag;
                /*if (double.TryParse(row.Cells[AREA].Value.ToString(), out dArea))
                    prop.Area = new VariousData<double>(dArea);
                else
                    prop.Area = null;*/
            }
            else
                prop.Area = null;
            /*double dWidth;

            if (row.Cells[WIDTH].Value != null)
            {
                if (double.TryParse(row.Cells[WIDTH].Value.ToString(), out dWidth))
                    prop.Width = new VariousData<double>(dWidth);
                else
                    prop.Width = null;
            }
            else
                prop.Width = null;*/

            if (row.Cells[LENGTH].Tag != null)
            {
                prop.Length = (VariousData<int>)row.Cells[LENGTH].Tag;
                /*if (int.TryParse(row.Cells[LENGTH].Value.ToString(), out nLength))
                    prop.Length = new VariousData<int>(nLength);
                else
                    prop.Length = null;*/
            }
            else
                prop.Length = null;

            if (row.Cells[LAND_ADDRESS].Value != null && row.Cells[LAND_ADDRESS].Tag != null)
            {
                List<LandAddressData> landAddrDatas = (List<LandAddressData>)row.Cells[LAND_ADDRESS].Tag;
                prop.LandAddressDatas.Clear();

                if (landAddrDatas != null)
                    ((List<LandAddressData>)prop.LandAddressDatas).AddRange(landAddrDatas);
            }
            else
                prop.LandAddressDatas.Clear();

            /*if (row.Cells[5].Value == null)
                prop.LandAddress = "";
            else
                prop.LandAddress = row.Cells[5].Value.ToString();*/

            if (row.Cells[FINAL_DATE].Tag == null)
                prop.FinalDate = null;
            else
            {
                VariousData<DateTime> time = (VariousData<DateTime>)row.Cells[FINAL_DATE].Tag;

                if (prop.FinalDate == null)
                {
                    prop.FinalDate = new VariousData<DateTime>(time.Data);
                }
                else
                {
                    prop.FinalDate.Data = time.Data;
                }
            }

            /*if (row.Cells[6].Value == null)
                prop.Date = "";
            else
                prop.Date = row.Cells[6].Value.ToString();*/

            if (row.Cells[TOTAL_COST].Value != null && row.Cells[TOTAL_COST].Tag != null)
            {
                TotalCost totalCost = (TotalCost)row.Cells[TOTAL_COST].Tag;
                prop.LandCost = totalCost.LandCost;
                prop.ObjectCost = totalCost.ObjectCost;
                prop.AroundCost = totalCost.AroundCost;
            }
            else
            {
                prop.LandCost = null;
                prop.ObjectCost = null;
                prop.AroundCost = null;
            }

            if (row.Cells[CHECK_COMPLETE].Value == null)
                prop.IsComplete = false;
            else
                prop.IsComplete = (bool)row.Cells[CHECK_COMPLETE].Value;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (m_schedule == null)
                return;

            m_schedule.Properties.Clear();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                ScheduleProperty prop = (ScheduleProperty)row.Tag;

                ReadRow(row, prop);
                
                m_schedule.Properties.Add(prop);
            }

            foreach (MovingSchedule moving in m_movingSchedules)
            {
                ScheduleProperty prop = moving.ScheduleProperty;
                ReadRow(moving.Row, prop);

                prop.Schedule.Properties.Remove(prop);
                moving.Schedule.Properties.Add(prop);
                prop.Schedule = moving.Schedule;
            }

            if (FormMain.Instance.CurrentPanel != null)
            {
                foreach (MovingSchedule moving in m_movingSchedules)
                {
                    ScheduleProperty prop = moving.ScheduleProperty;
                    FormMain.Instance.CurrentPanel.ProcessResultForm.RemoveScheduleProperty(prop);
                }

                // 집행진행상황 속성창이 떠있으면 닫는다.
                FormMain.Instance.CurrentPanel.ProcessResultForm.ClosePropertyForm();
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void FormScheduleProperty_Load(object sender, EventArgs e)
        {
            InitGrid();

            m_timePicker = new DateTimePicker();
            dataGridView1.Controls.Add(m_timePicker);
            m_timePicker.Visible = false;

            m_timePicker.CloseUp += new EventHandler(DateTimePicker_CloseUp);

            checkBoxEdit_CheckedChanged(null, null);

            if (m_schedule != null)
            {
                this.Text = m_schedule.ScheduleName + " - 집행계획 속성";
            }

            FormMain.Instance.CurrentPanel.ExcelGridLinker = this;
        }

        private void InitGrid()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dataGridView1.Rows.Clear();

            if (m_schedule != null)
            {
                int nIndex = 1;
                dataGridView1.AllowUserToAddRows = true;

                foreach (ScheduleProperty prop in m_schedule.Properties)
                {
                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[nIndex - 1].Clone();
                    dataGridView1.Rows.Add(row);

                    row = dataGridView1.Rows[nIndex - 1];
                    UpdateRow(row, prop, nIndex++);
                }

                dataGridView1.AllowUserToAddRows = false;
            }
        }

        private void checkBoxEdit_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBoxEdit.Checked)
            {
                dataGridView1.ReadOnly = true;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                btnUp.Enabled = btnDown.Enabled = false;
                btnOpenDataFile.Enabled = false;
            }
            else
            {
                dataGridView1.ReadOnly = false;
                dataGridView1.AllowUserToAddRows = true;
                dataGridView1.AllowUserToDeleteRows = true;
                btnUp.Enabled = btnDown.Enabled = true;
                btnOpenDataFile.Enabled = true;

                ReadOnlyColumns();
            }
        }

        private void ReadOnlyColumns()
        {
            colNo.ReadOnly = true;
            colImportance.ReadOnly = true;
            colLandAddr.ReadOnly = true;
            colDate.ReadOnly = true;
            colCost.ReadOnly = true;
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            int nRowCount = dataGridView1.Rows.Count;
            if (nRowCount <= 1)
                return;

            DataGridViewRow row = dataGridView1.Rows[nRowCount - 2];
            row.Cells[0].Value = nRowCount - 1;
            row.Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (row.Tag == null)
            {
                ScheduleProperty prop = new ScheduleProperty();
                prop.Schedule = m_schedule;
                row.Tag = prop;
            }
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            int nRowCount = dataGridView1.Rows.Count;

            for (int i = e.RowIndex; i < nRowCount;i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];

                if (!row.IsNewRow)
                    row.Cells[0].Value = i + 1;
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

                dataGridView1.Rows.Remove(dataGridView1.CurrentRow);
            }
            else if (e.KeyCode == Keys.F1)
                FormMain.Instance.ShowHelp();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;

            if (dataGridView1.CurrentRow.Index <= 0)
                return;

            if (dataGridView1.CurrentRow.IsNewRow)
                return;

            int nRowIndex = dataGridView1.CurrentRow.Index;
            int nColumnIndex = dataGridView1.CurrentCell.ColumnIndex;

            DataGridViewRow row = dataGridView1.CurrentRow;
            dataGridView1.Rows.Remove(row);

            dataGridView1.Rows.Insert(nRowIndex - 1, row);

            dataGridView1.Rows[nRowIndex - 1].Cells[0].Value = nRowIndex;
            dataGridView1.Rows[nRowIndex].Cells[0].Value = nRowIndex + 1;

            dataGridView1.ClearSelection();
            row.Cells[nColumnIndex].Selected = true;
            dataGridView1.CurrentCell = row.Cells[nColumnIndex];
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;

            int nRowCount = dataGridView1.Rows.Count;
            int nRowIndex = dataGridView1.CurrentRow.Index;
            int nColumnIndex = dataGridView1.CurrentCell.ColumnIndex;

            if (dataGridView1.CurrentRow.Index >= nRowCount - 1)
                return;

            if (dataGridView1.Rows[nRowIndex + 1].IsNewRow)
                return;

            DataGridViewRow row = dataGridView1.CurrentRow;
            dataGridView1.Rows.Remove(row);

            dataGridView1.Rows.Insert(nRowIndex + 1, row);

            dataGridView1.Rows[nRowIndex].Cells[0].Value = nRowIndex + 1;
            dataGridView1.Rows[nRowIndex + 1].Cells[0].Value = nRowIndex + 2;

            dataGridView1.ClearSelection();
            row.Cells[nColumnIndex].Selected = true;
            dataGridView1.CurrentCell = row.Cells[nColumnIndex];
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void btnOpenDataFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Excel Files|*.xlsx|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "Excel 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FormExcelGrid frm = new FormExcelGrid(dlg.FileName, FormMain.Instance.CurrentPanel);
                frm.Show(this);
            }
        }

        public DataGridViewSelectedCellCollection GetSelectedCells()
        {
            return dataGridView1.SelectedCells;
        }

        public static void GetMinIndex(DataGridViewSelectedCellCollection cells, out int nRowIndex, out int nColIndex)
        {
            nRowIndex = nColIndex = -1;

            foreach (DataGridViewCell cell in cells)
            {
                if (nRowIndex < 0)
                {
                    nRowIndex = cell.RowIndex;
                    nColIndex = cell.ColumnIndex;
                }
                else
                {
                    if (nRowIndex > cell.RowIndex)
                        nRowIndex = cell.RowIndex;

                    if (nColIndex > cell.ColumnIndex)
                        nColIndex = cell.ColumnIndex;
                }
            }
        }

        public void PasteCells(DataGridViewSelectedCellCollection cells)
        {
            /*if (m_frmImportance != null && m_frmImportance.Visible)
            {
                m_frmImportance.PasteCells(cells);
            }
            else if (m_frmLandNumber != null && m_frmLandNumber.Visible)
            {
                m_frmLandNumber.PasteCells(cells);
            }
            else*/
            {
                if (dataGridView1.SelectedCells.Count == 0)
                    return;

                int nRowIndex, nColIndex;
                GetMinIndex(dataGridView1.SelectedCells, out nRowIndex, out nColIndex);

                int nSrcCount = cells.Count;

                if (nSrcCount == 0)
                    return;

                int nBeginRowIndex, nBeginColIndex;
                GetMinIndex(cells, out nBeginRowIndex, out nBeginColIndex);

                for (int i = 0; i < nSrcCount; i++)
                {
                    int nSrcRowIndex = cells[i].RowIndex;
                    int nSrcColIndex = cells[i].ColumnIndex;

                    int nIndex1 = nSrcRowIndex - nBeginRowIndex;
                    int nIndex2 = nSrcColIndex - nBeginColIndex;

                    int nTrgRowIndex = nRowIndex + nIndex1;
                    int nTrgColIndex = nColIndex + nIndex2;

                    while (nTrgRowIndex >= dataGridView1.Rows.Count - 1)
                    {
                        MakeNewRow();
                    }

                    if (nTrgColIndex < dataGridView1.Columns.Count - 1 && nTrgColIndex != 0 && nTrgColIndex != IMPORTANCE && nTrgColIndex != LAND_ADDRESS && nTrgColIndex != TOTAL_COST && nTrgColIndex != CHECK_COMPLETE)
                    {
                        /*if (nTrgColIndex == 3)
                        {
                            DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn)dataGridView1.Columns[nTrgColIndex];

                            if (!column.Items.Contains(cells[i].Value))
                                column.Items.Add(cells[i].Value);
                        }*/

                        dataGridView1.Rows[nTrgRowIndex].Cells[nTrgColIndex].Value = cells[i].Value;
                        CheckCellValue(dataGridView1.Rows[nTrgRowIndex].Cells[nTrgColIndex]);
                    }
                }

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    row.Cells[0].Value = row.Index + 1;
                }
            }
        }

        private void CheckCellValue(DataGridViewCell cell)
        {
            if (cell.ColumnIndex == AREA)
            {
                if (cell.Value == null || cell.Value.ToString().Length == 0)
                    cell.Tag = null;
                else
                {
                    double dData;
                    if (!double.TryParse(cell.Value.ToString(), out dData))
                    {
                        if (cell.Tag != null)
                            cell.Value = string.Format("{0:###,###,###,###,###,###}", (int)((VariousData<double>)cell.Tag).Data);
                        else
                            cell.Value = "";
                    }
                    else
                    {
                        cell.Value = string.Format("{0:###,###,###,###,###,###}", (int)dData);
                        cell.Tag = new VariousData<double>(dData);
                    }
                }
            }
            else if (cell.ColumnIndex == LENGTH)
            {
                if (cell.Value == null || cell.Value.ToString().Length == 0)
                    cell.Tag = null;
                else
                {
                    int nData;

                    if (!int.TryParse(cell.Value.ToString(), out nData))
                    {
                        if (cell.Tag != null)
                            cell.Value = string.Format("{0:###,###,###,###,###,###}", ((VariousData<int>)cell.Tag).Data);
                        else
                            cell.Value = "";
                    }
                    else
                    {
                        cell.Value = string.Format("{0:###,###,###,###,###,###}", nData);
                        cell.Tag = new VariousData<int>(nData);
                    }
                }
            }
            else if (cell.ColumnIndex == FINAL_DATE)
            {
                if (cell.Value == null || cell.Value.ToString().Length == 0)
                    cell.Tag = null;
                else
                {
                    DateTime dtTime;

                    if (ScheduleProperty.ReadDateTimeString(cell.Value.ToString(), out dtTime))
                    {
                        string strDate = ScheduleProperty.GetDateTimeString(dtTime);
                        cell.Value = strDate;
                        cell.Tag = new VariousData<DateTime>(dtTime);
                    }
                    else
                    {
                        if (cell.Tag == null)
                            cell.Value = "";
                        else
                            cell.Value = ScheduleProperty.GetDateTimeString(((VariousData<DateTime>)cell.Tag).Data);
                    }
                }
            }
        }

        private void MakeNewRow()
        {
            if (dataGridView1.AllowUserToAddRows)
            {
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;

                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                dataGridView1.AllowUserToAddRows = false;
            }
        }

        /*private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCellAddress.X == colWidth.DisplayIndex)
            {
                ComboBox cb = e.Control as ComboBox;

                if (cb != null)
                {
                    if (cb.Tag == null)
                    {
                        //cb.TextChanged += new EventHandler(this.ComboBoxCell_TextChanged);
                    }

                    if (dataGridView1.CurrentCell.Value == null || dataGridView1.CurrentCell.Value.ToString().Length == 0)
                        cb.Text = "";
                    else
                        cb.Text = dataGridView1.CurrentCell.Value.ToString();

                    cb.Tag = dataGridView1.CurrentCell;

                    if (dataGridView1.CurrentCell != null)
                    {
                        if (dataGridView1.CurrentCell.Tag == null)
                        {
                            ComboBoxText<double> text = new ComboBoxText<double>();
                            text.Control = cb;
                            dataGridView1.CurrentCell.Tag = text;
                        }
                    }

                    cb.DropDownStyle = ComboBoxStyle.DropDown;
                }
            }
        }*/

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex == AREA)
            {
                if (cell.Value == null || cell.Value.ToString().Length == 0)
                    cell.Tag = null;
                else
                {
                    string strValue = cell.Value.ToString();
                    double dArea;

                    if (!double.TryParse(strValue, out dArea) || dArea <= 0.0)
                    {
                        UnE.Utility.UMessageBox.Show(this, "면적은 0보다 큰 숫자만 입력이 가능합니다.");

                        if (cell.Tag == null)
                            cell.Value = null;
                        else
                            cell.Value = ((VariousData<double>)cell.Tag).Data;

                        return;
                    }

                    if (dArea == 0.0)
                        cell.Value = "0";
                    else
                        cell.Value = string.Format("{0:###,###,###,###,###,###}", (int)dArea);

                    cell.Tag = new VariousData<double>(dArea);
                }
            }
            /*if (e.ColumnIndex == WIDTH)
            {
                if (cell.Tag == null)
                    return;

                ComboBoxText<double> text = (ComboBoxText<double>)cell.Tag;

                if (text.Control.Text == null || text.Control.Text.Length == 0)
                    text.Data = null;
                else
                {
                    string strValue = text.Control.Text;
                    double dWidth;

                    if (!double.TryParse(strValue, out dWidth) || dWidth <= 0.0)
                    {
                        MessageBox.Show("도로폭은 0보다 큰 숫자만 입력이 가능합니다.");

                        if (text.Data == null)
                            cell.Value = null;
                        else
                            cell.Value = text.Data.Data.ToString();

                        return;
                    }

                    DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex];

                    if (!column.Items.Contains(strValue))
                    {
                        column.Items.Add(strValue);
                        cell.Value = strValue;
                    }
                    else
                        cell.Value = strValue;

                    text.Data = new VariousData<double>(dWidth);
                }
            }*/
            else if (e.ColumnIndex == LENGTH)
            {
                if (cell.Value == null || cell.Value.ToString().Length == 0)
                    cell.Tag = null;
                else
                {
                    string strValue = cell.Value.ToString();
                    int nLength;

                    if (!int.TryParse(strValue, out nLength) || nLength <= 0)
                    {
                        UnE.Utility.UMessageBox.Show(this, "연장은 0보다 큰 정수 형태의 값만 입력이 가능합니다.");

                        if (cell.Tag == null)
                            cell.Value = null;
                        else
                            cell.Value = ((VariousData<int>)cell.Tag).Data;

                        return;
                    }

                    if (nLength == 0)
                        cell.Value = "0";
                    else
                        cell.Value = string.Format("{0:###,###,###,###,###,###}", nLength);

                    cell.Tag = new VariousData<int>(nLength);
                }
            }
        }

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
        }*/

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (m_timePicker == null)
                return;

            FormMain.Instance.CurrentPanel.ExcelGridLinker = this;

            m_timePicker.Hide();

            if (e.ColumnIndex == FINAL_DATE && e.RowIndex >= 0)
            {
                if (checkBoxEdit.Checked)
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
                    m_timePicker.Show();
                }
            }

            PanelDXFViewer panel = FormMain.Instance.CurrentPanel;

            if (e.RowIndex >= 0 && panel != null)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (row.IsNewRow)
                {
                    ShowEditBoxHatch(FormEditSection.Instance.CurrentProperty, false);
                    FormEditSection.Instance.CurrentProperty = null;
                    panel.ClearFixedSelection();
                }
                else
                {
                    DXFExternPainter painter = (DXFExternPainter)FormMain.Instance.CurrentDXFControl.ExternalPainter;
                    painter.ClearSelection();

                    ScheduleProperty prop = (ScheduleProperty)row.Tag;

                    if (FormEditSection.Instance.CurrentProperty != prop)
                        panel.ClearFixedSelection();

                    SelectProperty(prop, false);
                }

                string strStreetName = row.Cells[STREET_NAME].Value == null ? "" : row.Cells[STREET_NAME].Value.ToString();
                List<DXFViewer.Shape> shapes = null;

                //List<FixedSelectionData> prevSelections = panel.GetFixedSelections();
                panel.ClearFixedSelection();

                //List<FixedSelectionData> currentSelections = null;

                if (panel.DataManager.StreetShapes.TryGetValue(strStreetName, out shapes))
                {
                    if (Options.Instance.ZoomOnSelectStreet == true)
					{
						panel.DataManager.ObjectZoom(strStreetName, panel);
					}
					else
					{
						FormSettingStreetName.SelectShapes(panel, shapes, true, false);
					}
                    
                    //currentSelections = panel.GetFixedSelections();
                }

                //if (!FixedSelectionData.IsSame(prevSelections, currentSelections))

                FormMain.Instance.RefreshView();
            }
        }

        private void SelectProperty(ScheduleProperty prop, bool refresh)
        {
            if (FormEditSection.Instance.CurrentProperty == prop)
                return;
            else
            {
                FormEditSection.Instance.CurrentHatch = null;

                ShowEditBoxHatch(FormEditSection.Instance.CurrentProperty, false);
                ShowEditBoxHatch(prop, true);
                FormEditSection.Instance.CurrentProperty = prop;

                if (refresh)
                    FormMain.Instance.RefreshView();
            }
        }

        private void ShowEditBoxHatch(ScheduleProperty prop, bool visible)
        {
            if (prop != null)
            {
                foreach (SchedulePropertySector sector in prop.Sectors)
                {
                    sector.Hatch.Visible = visible;
                    sector.Shape.Selected = visible;
                }
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
            }
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            m_timePicker.Hide();
        }

        private void menuEditSector_Click(object sender, EventArgs e)
        {
            ScheduleProperty prop = (ScheduleProperty)dataGridView1.CurrentRow.Tag;
            SelectProperty(prop, true);

            if (!FormEditSection.Instance.Visible)
                FormEditSection.Instance.Show(FormMain.Instance);
        }

        private void menuMoveSchedule_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)sender;
            MovingSchedule moving = (MovingSchedule)menu.Tag;

            if (moving == null || moving.Schedule == null || moving.ScheduleProperty == null || moving.Row == null)
                return;

            string strMessage = string.Format("[{0}]을 [{1}]로 옮기시겠습니까?", moving.ScheduleProperty.StreetName, moving.Schedule.ScheduleName);

            if (UnE.Utility.UMessageBox.Show(this, strMessage, "확인", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                if (moving.ScheduleProperty.Schedule != null)
                {
                    m_movingSchedules.Add(moving);
                    dataGridView1.Rows.Remove(moving.Row);

                    // 확인 버튼을 누를때 실행한다.
                    //moving.ScheduleProperty.Schedule.Properties.Remove(moving.ScheduleProperty);
                    //moving.Schedule.Properties.Add(moving.ScheduleProperty);
                }
            }
        }

        private void menuEditDetail_Click(object sender, EventArgs e)
        {
            ScheduleProperty prop = (ScheduleProperty)dataGridView1.CurrentRow.Tag;
            PanelDXFViewer panel = FormMain.Instance.CurrentPanel;

            if (panel.ScheduleDetailForm != null)
            {
                // 이미 같은 창이 떠있으면 다시 띄우지 않는다.
                if (panel.ScheduleDetailForm.ScheduleProperty == prop)
                    return;
                else
                    panel.ScheduleDetailForm.Close();
            }

            // 기존 행으로부터 데이터를 복사해온다.
            ScheduleProperty clone = prop.Clone();
            // Grid에서 직접 편집된 데이터는 아직 prop에 적용되지 않았으므로 Grid의 데이터도 불러온다.
            ReadRow(dataGridView1.CurrentRow, clone);

            FormScheduleDetail frm = new FormScheduleDetail(clone, panel);
            frm.SchedulePropertyForm = this;
            frm.Row = dataGridView1.CurrentRow;

            if (!checkBoxEdit.Checked)
                frm.DisableEdit();

			DialogFormFrame frameDetail = new DialogFormFrame(frm);
			if (frameDetail.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                prop.CopyFrom(clone);
                UpdateRow(frm.Row, prop, -1);
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right && dataGridView1.CurrentRow != null)
            {
                ScheduleProperty prop = (ScheduleProperty)dataGridView1.CurrentRow.Tag;

                if (prop != null)
                {
                    Rectangle rect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
                    ShowMenu(e.X + rect.Left, e.Y + rect.Top, prop, dataGridView1.CurrentRow);
                }
            }
        }

        private void ShowMenu(int x, int y, ScheduleProperty prop, DataGridViewRow row)
        {
            ClearContextMenu();

            if (prop.Schedule != null && checkBoxEdit.Checked)
            {
                PanelDXFViewer panel = FormMain.Instance.CurrentPanel;

                if (panel != null)
                {
                    List<ProcessSchedule> schedules = panel.ProcessSchedules;

                    foreach (ProcessSchedule schedule in schedules)
                    {
                        if (schedule == prop.Schedule)
                            continue;

                        ToolStripMenuItem menu = new ToolStripMenuItem("[" + schedule.ScheduleName + "]으로 이동", null, menuMoveSchedule_Click);
                        menu.Tag = new MovingSchedule(schedule, prop, row);
                        contextMenuStrip1.Items.Add(menu);
                    }
                }
            }

            contextMenuStrip1.Show(dataGridView1, x, y);
        }

        private void ClearContextMenu()
        {
            List<ToolStripMenuItem> removeItems = new List<ToolStripMenuItem>();
            
            foreach (ToolStripMenuItem menu in contextMenuStrip1.Items)
            {
                if (menu == menuEditDetail)
                    continue;
                else
                    removeItems.Add(menu);
            }

            foreach (ToolStripMenuItem menu in removeItems)
            {
                contextMenuStrip1.Items.Remove(menu);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.ReadOnly)
                return;

            if (e.RowIndex >= 0)
            {
                if (e.ColumnIndex == IMPORTANCE)
                {
                    if (m_frmImportance != null && m_frmImportance.Visible)
                        return;

                    DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    if (cell.Tag == null)
                        cell.Tag = new ImportanceData();

                    SettingImportance_ScheduleProperty setting = new SettingImportance_ScheduleProperty(cell, this);
                    m_frmImportance = new FormImportance(setting);
					DialogFormFrame frameImportance = new DialogFormFrame(m_frmImportance);
					frameImportance.Show(this);
                }
                else if (e.ColumnIndex == LAND_ADDRESS)
                {
                    if (m_frmLandNumber != null && m_frmLandNumber.Visible)
                        return;

                    DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    if (cell.Tag == null)
                        cell.Tag = new List<LandAddressData>();

                    SettingLandNumber_ScheduleProperty setting = new SettingLandNumber_ScheduleProperty(cell, this);

                    string strAddr = dataGridView1.Rows[e.RowIndex].Cells[STREET_NAME].Value == null ? "" : dataGridView1.Rows[e.RowIndex].Cells[STREET_NAME].Value.ToString();
                   
					m_frmLandNumber = new FormLandNumber(setting, strAddr);
					DialogFormFrame fameLand = new DialogFormFrame(m_frmLandNumber);
					fameLand.Show(this);
                }
                else if (e.ColumnIndex == TOTAL_COST)
                {
                    TotalCost totalCost = (TotalCost)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag;

                    if (totalCost == null)
                    {
                        totalCost = new TotalCost();
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag = totalCost;
                    }

                    FormTotalCost frm = new FormTotalCost(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString(), totalCost.LandCost, totalCost.ObjectCost, totalCost.AroundCost);

					DialogFormFrame frameCost = new DialogFormFrame(frm);
					if (frameCost.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[TOTAL_COST].Value = ScheduleProperty.GetTotalCostString(frm.LandCost, frm.ObjectCost, frm.AroundCost);

                        totalCost.LandCost = frm.LandCost;
                        totalCost.ObjectCost = frm.ObjectCost;
                        totalCost.AroundCost = frm.AroundCost;
                    }
                }
            }
        }

        public void SelectScheduleProperty(ScheduleProperty prop)
        {
            dataGridView1.ClearSelection();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                ScheduleProperty _prop = (ScheduleProperty)row.Tag;

                if (_prop == prop)
                {
                    row.Cells[0].Selected = true;

                    // 자동 줄바꿈
                    dataGridView1.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        public void UpdateRow(DataGridViewRow row, ScheduleProperty prop, int nRowIndex)
        {
            row.Tag = prop;

            if (prop.Area == null || prop.Area.Data == 0.0)
                row.Cells[AREA].Value = null;
            else
            {
                string strArea = string.Format("{0:###,###,###,###,###,###}", (int)prop.Area.Data);
                row.Cells[AREA].Value = strArea;
            }

            /*if (prop.Width == null || prop.Width.Data == 0.0)
                row.Cells[WIDTH].Value = null;
            else
            {
                string strWidth = string.Format("{0:F0}", prop.Width.Data);

                if (!colWidth.Items.Contains(strWidth))
                    colWidth.Items.Add(strWidth);

                row.Cells[WIDTH].Value = strWidth;
            }*/

            if (prop.Length == null || prop.Length.Data == 0.0)
                row.Cells[LENGTH].Value = null;
            else
            {
                string strLength = string.Format("{0:###,###,###,###,###,###}", prop.Length.Data);

                /*if (!colLength.Items.Contains(strLength))
                    colLength.Items.Add(strLength);*/

                row.Cells[LENGTH].Value = strLength;
            }

            if (nRowIndex >= 0)
                row.Cells[0].Value = nRowIndex;

            row.Cells[STREET_NAME].Value = prop.StreetName;
            row.Cells[IMPORTANCE].Value = prop.Importance == null ? "" : string.Format("{0:F2}", prop.Importance.Importance);
            row.Cells[IMPORTANCE].Tag = prop.Importance == null ? null : prop.Importance.Clone();
            row.Cells[LENGTH].Tag = prop.Length;
            row.Cells[AREA].Tag = prop.Area;
            //row.Cells[3].Value = prop.Width == null ? "" : string.Format("{0:F0}", prop.Width.Data);
            //row.Cells[4].Value = prop.Length == null ? "" : string.Format("{0:F0}", prop.Length.Data);
            row.Cells[LAND_ADDRESS].Value = prop.GetFirstNLastLandAddressString();
            row.Cells[LAND_ADDRESS].Tag = prop.CloneLandAddressDataList();
            row.Cells[FINAL_DATE].Value = prop.FinalDate == null ? "" : ScheduleProperty.GetDateTimeString(prop.FinalDate.Data);
            row.Cells[FINAL_DATE].Tag = prop.FinalDate;
            row.Cells[TOTAL_COST].Value = prop.TotalCost;
            row.Cells[TOTAL_COST].Tag = new TotalCost(prop.LandCost, prop.ObjectCost, prop.AroundCost);
            row.Cells[CHECK_COMPLETE].Value = prop.IsComplete;

            /*DataGridViewRow row = new DataGridViewRow();

            DataGridViewCell cell = new DataGridViewTextBoxCell();
            cell.Value = nIndex++;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = prop.Address;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = prop.Importance == null ? "" : string.Format("{0:F1}", prop.Importance.Data);
            row.Cells.Add(cell);

            cell = new DataGridViewComboBoxCell();
            cell.Value = prop.Width == null ? "" : string.Format("{0:F0}", prop.Width.Data);
            row.Cells.Add(cell);

            cell = new DataGridViewComboBoxCell();
            cell.Value = prop.Length == null ? "" : string.Format("{0:F0}", prop.Length.Data);
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = prop.LandAddress;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = prop.Date;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = prop.Cost;
            row.Cells.Add(cell);

            DataGridViewCheckBoxCell cell2 = new DataGridViewCheckBoxCell();
            cell2.Value = prop.IsComplete;
            row.Cells.Add(cell2);

            dataGridView1.Rows.Add(row);*/
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            if (e.ColumnIndex == LENGTH)
            {
                if (row.Cells[e.ColumnIndex].Tag == null)
                    row.Cells[e.ColumnIndex].Value = null;
                else
                {
                    VariousData<int> nLength = (VariousData<int>)row.Cells[e.ColumnIndex].Tag;
                    row.Cells[e.ColumnIndex].Value = nLength.Data;
                }
            }
            else if (e.ColumnIndex == AREA)
            {
                if (row.Cells[e.ColumnIndex].Tag == null)
                    row.Cells[e.ColumnIndex].Value = null;
                else
                {
                    VariousData<double> dArea = (VariousData<double>)row.Cells[e.ColumnIndex].Tag;
                    row.Cells[e.ColumnIndex].Value = (int)dArea.Data;
                }
            }
        }

        private void FormScheduleProperty_FormClosing(object sender, FormClosingEventArgs e)
        {
            PanelDXFViewer panel = FormMain.Instance.CurrentPanel;

            if (panel != null)
            {
                if (panel.ExcelGridLinker == this)
                    panel.ExcelGridLinker = null;

                panel.ProcessScheduleForm.UpdateProcessSchedule(m_schedule);
                panel.ClearFixedSelection();
                panel.DXFControl.Refresh();
            }
        }

        public DataGridViewSelectedCellCollection GetPastePositionCells()
        {
            return dataGridView1.SelectedCells;
        }

        private class SettingImportance_ScheduleProperty : FormImportance.SettingImportance
        {
            private DataGridViewCell m_cell = null;
            private FormScheduleProperty m_frmScheduleProperty = null;

            public DataGridViewCell Cell
            {
                get { return m_cell; }
                set { m_cell = value; }
            }

            public override ImportanceData Data
            {
                get { return m_cell == null ? null : (ImportanceData)m_cell.Tag; }
                set
                {
                    if (m_cell != null)
                    {
                        m_cell.Value = string.Format("{0:F2}", value.Importance);
                        m_cell.Tag = value;
                    }
                }
            }

            public SettingImportance_ScheduleProperty(DataGridViewCell cell, FormScheduleProperty frmProperty)
            {
                m_cell = cell;
                m_frmScheduleProperty = frmProperty;
            }

            public override void Close()
            {
                if (m_frmScheduleProperty != null)
                    m_frmScheduleProperty.ImportanceForm = null;
            }
        }

        private class SettingLandNumber_ScheduleProperty : FormLandNumber.SettingLandNumber
        {
            private DataGridViewCell m_cell = null;
            private FormScheduleProperty m_frmProperty = null;
            
            public DataGridViewCell Cell
            {
                get { return m_cell; }
                set { m_cell = value; }
            }

            public FormScheduleProperty PropertyForm
            {
                get { return m_frmProperty; }
                set { m_frmProperty = value; }
            }

            public override List<LandAddressData> Data
            {
                get { return m_cell == null ? null : (List<LandAddressData>)m_cell.Tag; }
                set
                {
                    if (m_cell != null)
                    {
                        if (m_frmProperty != null)
                            m_frmProperty.LandNumberForm = null;

                        m_cell.Value = ScheduleProperty.GetFirstNLastLandAddressString(value);
                        m_cell.Tag = value;
                    }
                }
            }

            public SettingLandNumber_ScheduleProperty(DataGridViewCell cell, FormScheduleProperty frm)
            {
                m_cell = cell;
                m_frmProperty = frm;
            }

            public override void Close()
            {
                if (m_frmProperty != null)
                    m_frmProperty.LandNumberForm = null;
            }
        }

        private class TotalCost
        {
            private VariousData<long> m_nLandCost = null;
            private VariousData<long> m_nObjectCost = null;
            private VariousData<long> m_nAroundCost = null;

            public VariousData<long> LandCost
            {
                get { return m_nLandCost; }
                set { m_nLandCost = value; }
            }

            public VariousData<long> ObjectCost
            {
                get { return m_nObjectCost; }
                set { m_nObjectCost = value; }
            }

            public VariousData<long> AroundCost
            {
                get { return m_nAroundCost; }
                set { m_nAroundCost = value; }
            }

            public TotalCost()
            {
            }

            public TotalCost(VariousData<long> nLandCost, VariousData<long> nObjectCost, VariousData<long> nAroundCost)
            {
                m_nLandCost = nLandCost;
                m_nObjectCost = nObjectCost;
                m_nAroundCost = nAroundCost;
            }
        }

        private class MovingSchedule
        {
            private ProcessSchedule m_scheduleTarget = null;
            private ScheduleProperty m_property = null;
            private DataGridViewRow m_row = null;

            public ProcessSchedule Schedule
            {
                get { return m_scheduleTarget; }
                set { m_scheduleTarget = value; }
            }

            public ScheduleProperty ScheduleProperty
            {
                get { return m_property; }
                set { m_property = value; }
            }

            public DataGridViewRow Row
            {
                get { return m_row; }
                set { m_row = value; }
            }

            public MovingSchedule()
            {
            }

            public MovingSchedule(ProcessSchedule schedule, ScheduleProperty prop, DataGridViewRow row)
            {
                m_scheduleTarget = schedule;
                m_property = prop;
                m_row = row;
            }
        }
    }
}
