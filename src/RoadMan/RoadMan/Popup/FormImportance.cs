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
    public partial class FormImportance : Form, IExcelGridLinker
    {
        private const int PEOPLE_REQUEST = 0;
        private const int NEEDS = 1;
        private const int RIGHT = 2;
        private const int NO_DATE = 3;
        private const int LAND_STATUS = 4;
        private const int AROUND = 5;
        private const int LEVEL = 6;
        private const int TOTAL = 7;

        //private DataGridViewCell m_cell = null;
        private SettingImportance m_setting = null;
        private ImportanceData m_importanceData = new ImportanceData();

        private bool m_cellValueChangedByPasteCells = false;
        private bool m_ignoreValueChanged = false;
        private bool m_noPasteCell = false;

        public FormImportance(SettingImportance setting)
        {
            m_setting = setting;
            //m_cell = cell;

            InitializeComponent();
        }

        private void InitGridStyle()
        {
            colPeopleRequest.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNeeds.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colRight.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNoDate.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colLandStatus.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colAround.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colLevel.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colTotal.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Rows[0].HeaderCell.Value = "평가항목";
            dataGridView1.Rows[1].HeaderCell.Value = "가중치";
            dataGridView1.Rows[2].HeaderCell.Value = "값";

            dataGridView1.Rows[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Rows[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Rows[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            foreach (DataGridViewCell cell in dataGridView1.Rows[1].Cells)
            {
                cell.ReadOnly = true;
            }
        }

        public void DisableEdit()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.ReadOnly = true;
            }

            m_noPasteCell = true;
        }

        private void FormImportance_Load(object sender, EventArgs e)
        {
            SetDataGrid();
            SetFactorGrid();
            SetTotalGrid();

            InitGridStyle();
            FormMain.Instance.CurrentPanel.ExcelGridLinker = this;

            dataGridView1.Select();
        }

        private DataGridViewCell SetTotalGridCell(int nColumnIndex, bool newCell)
        {
            DataGridViewRow rowData = dataGridView1.Rows[0];
            DataGridViewRow rowFactor = dataGridView1.Rows[1];

            DataGridViewCell cell = newCell ? new DataGridViewTextBoxCell() : dataGridView1.Rows[2].Cells[nColumnIndex];
            VariousData<double> dData = GetTotalCellValue(rowData.Cells[nColumnIndex], rowFactor.Cells[nColumnIndex]);

            m_ignoreValueChanged = true;

            if (dData == null)
            {
                cell.Value = "";
            }
            else
            {
                cell.Value = string.Format("{0:F2}", dData.Data);
            }

            m_ignoreValueChanged = false;

            cell.Tag = dData;
            return cell;
        }

        private void SetTotalGridCell(int nColumnIndex, DataGridViewRow row, ref double dTotal)
        {
            DataGridViewCell cell = SetTotalGridCell(nColumnIndex, true);

            if (cell.Tag != null)
            {
                VariousData<double> dData = (VariousData<double>)cell.Tag;
                dTotal += dData.Data;
            }

            row.Cells.Add(cell);
            cell.ReadOnly = true;
        }

        private void SetTotalGrid()
        {
            DataGridViewRow rowData = dataGridView1.Rows[0];
            DataGridViewRow rowFactor = dataGridView1.Rows[1];

            DataGridViewRow row = new DataGridViewRow();
            double dTotal = 0.0;

            for (int i = 0; i <= 6; i++)
            {
                SetTotalGridCell(i, row, ref dTotal);
            }

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            
            cell.Value = string.Format("{0:F2}", dTotal);
            cell.Tag = dTotal;
            row.Cells.Add(cell);
            
            dataGridView1.Rows.Add(row);
        }

        private VariousData<double> GetTotalCellValue(DataGridViewCell cellData, DataGridViewCell cellFactor)
        {
            if (cellData.Value == null || cellData.Value.ToString().Length == 0)
                return null;

            if (cellFactor.Value == null || cellFactor.Value.ToString().Length == 0)
                return null;

            VariousData<int> nData = (VariousData<int>)cellData.Tag;
            
            return new VariousData<double>(nData.Data * (double)cellFactor.Tag);
        }

        private void SetFactorGrid()
        {
            DataGridViewRow row = new DataGridViewRow();
            double dTotal = 0.0;

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Value = string.Format("{0:F2}", ImportanceData.PeopleRequestFactor);
            cell2.Tag = ImportanceData.PeopleRequestFactor;
            row.Cells.Add(cell2);
            dTotal += ImportanceData.PeopleRequestFactor;

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = string.Format("{0:F2}", ImportanceData.NeedsFactor);
            cell2.Tag = ImportanceData.NeedsFactor;
            row.Cells.Add(cell2);
            dTotal += ImportanceData.NeedsFactor;

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = string.Format("{0:F2}", ImportanceData.RightFactor);
            cell2.Tag = ImportanceData.RightFactor;
            row.Cells.Add(cell2);
            dTotal += ImportanceData.RightFactor;

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = string.Format("{0:F2}", ImportanceData.NoDateFactor);
            cell2.Tag = ImportanceData.NoDateFactor;
            row.Cells.Add(cell2);
            dTotal += ImportanceData.NoDateFactor;

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = string.Format("{0:F2}", ImportanceData.LandStatusFactor);
            cell2.Tag = ImportanceData.LandStatusFactor;
            row.Cells.Add(cell2);
            dTotal += ImportanceData.LandStatusFactor;

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = string.Format("{0:F2}", ImportanceData.AroundFactor);
            cell2.Tag = ImportanceData.AroundFactor;
            row.Cells.Add(cell2);
            dTotal += ImportanceData.AroundFactor;

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = string.Format("{0:F2}", ImportanceData.LevelFactor);
            cell2.Tag = ImportanceData.LevelFactor;
            row.Cells.Add(cell2);
            dTotal += ImportanceData.LevelFactor;

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = string.Format("{0:F2}", dTotal);
            row.Cells.Add(cell2);

            dataGridView1.Rows.Add(row);
        }

        private void SetDataGrid()
        {
            DataGridViewRow row = new DataGridViewRow();

            if (m_setting != null && m_setting.Data != null)
            //if (m_cell != null && m_cell.Tag != null)
            {
                int nTotal = 0;
                ImportanceData data = m_setting.Data;
                //ImportanceData data = (ImportanceData)m_cell.Tag;

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = string.Format("{0}", data.PeopleRequest);
                cell.Tag = new VariousData<int>(data.PeopleRequest);
                row.Cells.Add(cell);
                nTotal += data.PeopleRequest;

                cell = new DataGridViewTextBoxCell();
                cell.Value = string.Format("{0}", data.Needs);
                cell.Tag = new VariousData<int>(data.Needs);
                row.Cells.Add(cell);
                nTotal += data.Needs;

                cell = new DataGridViewTextBoxCell();
                cell.Value = string.Format("{0}", data.Right);
                cell.Tag = new VariousData<int>(data.Right);
                row.Cells.Add(cell);
                nTotal += data.Right;

                cell = new DataGridViewTextBoxCell();
                cell.Value = string.Format("{0}", data.NoDate);
                cell.Tag = new VariousData<int>(data.NoDate);
                row.Cells.Add(cell);
                nTotal += data.NoDate;

                cell = new DataGridViewTextBoxCell();
                cell.Value = string.Format("{0}", data.LandStatus);
                cell.Tag = new VariousData<int>(data.LandStatus);
                row.Cells.Add(cell);
                nTotal += data.LandStatus;

                cell = new DataGridViewTextBoxCell();
                cell.Value = string.Format("{0}", data.Around);
                cell.Tag = new VariousData<int>(data.Around);
                row.Cells.Add(cell);
                nTotal += data.Around;

                cell = new DataGridViewTextBoxCell();
                cell.Value = string.Format("{0}", data.Level);
                cell.Tag = new VariousData<int>(data.Level);
                row.Cells.Add(cell);
                nTotal += data.Level;

                cell = new DataGridViewTextBoxCell();
                cell.Value = string.Format("{0}", nTotal);
                cell.Tag = nTotal;
                row.Cells.Add(cell);
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = "";
                    cell.Tag = null;
                    row.Cells.Add(cell);
                }

                row.Cells[7].Tag = 0;
            }

            dataGridView1.Rows.Add(row);
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_ignoreValueChanged)
                return;

            if (e.RowIndex == 0 && e.ColumnIndex >= 0 && e.ColumnIndex < 7)
            {
                DataGridViewCell cellData = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                DataGridViewCell cellValue = dataGridView1.Rows[2].Cells[e.ColumnIndex];

                if (cellData.Value == null || cellData.Value.ToString().Length == 0)
                {
                    cellData.Tag = null;
                }
                else
                {
                    int nData;

                    if (!int.TryParse(cellData.Value.ToString(), out nData) || nData < 0)
                    {
                        if (!m_cellValueChangedByPasteCells)
                            UnE.Utility.UMessageBox.Show(this, "[" + dataGridView1.Columns[e.ColumnIndex].HeaderText + "]은 0보다 큰 정수 형태의 값이어야 합니다.");

                        if (cellData.Tag == null)
                            cellData.Value = "";
                        else
                            cellData.Value = ((VariousData<int>)cellData.Tag).Data.ToString();

                        m_cellValueChangedByPasteCells = false;
                        return;
                    }

                    cellData.Tag = new VariousData<int>(nData);
                }
                
                SetTotalGridCell(e.ColumnIndex, false);
                SetTotalData();
            }

            m_cellValueChangedByPasteCells = false;
        }

        private void SetTotalData()
        {
            int nTotal = 0;
            double dTotal = 0;

            for (int i=0;i<=6;i++)
            {
                DataGridViewCell cellData = dataGridView1.Rows[0].Cells[i];
                DataGridViewCell cellValue = dataGridView1.Rows[2].Cells[i];

                if (cellData.Tag != null)
                    nTotal += ((VariousData<int>)cellData.Tag).Data;

                if (cellValue.Tag != null)
                    dTotal += ((VariousData<double>)cellValue.Tag).Data;
            }

            m_ignoreValueChanged = true;
            dataGridView1.Rows[0].Cells[7].Value = nTotal.ToString();
            dataGridView1.Rows[0].Cells[7].Tag = nTotal;

            dataGridView1.Rows[2].Cells[7].Value = string.Format("{0:F2}", dTotal);
            dataGridView1.Rows[2].Cells[7].Tag = dTotal;
            m_ignoreValueChanged = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (m_setting != null)
            //if (m_cell != null)
            {
                m_importanceData.PeopleRequest = GetData(0);
                m_importanceData.Needs = GetData(1);
                m_importanceData.Right = GetData(2);
                m_importanceData.NoDate = GetData(3);
                m_importanceData.LandStatus = GetData(4);
                m_importanceData.Around = GetData(5);
                m_importanceData.Level = GetData(6);

                m_setting.Data = m_importanceData;
                /*m_cell.Value = string.Format("{0:F2}", m_importanceData.Importance);
                m_cell.Tag = m_importanceData;*/
            }

            this.Close();
        }

        private int GetData(int nIndex)
        {
            if (dataGridView1.Rows[0].Cells[nIndex].Value == null)
                return 0;

            int data;

            if (int.TryParse(dataGridView1.Rows[0].Cells[nIndex].Value.ToString(), out data))
                return data;

            return 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void PasteCells(DataGridViewSelectedCellCollection cells)
        {
            if (m_noPasteCell)
                return;

            if (dataGridView1.SelectedCells.Count == 0)
                return;

            int nRowIndex = dataGridView1.SelectedCells[0].RowIndex, nColIndex = dataGridView1.SelectedCells[0].ColumnIndex;
            
            int nSrcCount = cells.Count;

            if (nSrcCount == 0)
                return;

            int nBeginRowIndex, nBeginColIndex;
            FormScheduleProperty.GetMinIndex(cells, out nBeginRowIndex, out nBeginColIndex);

            for (int i = 0; i < nSrcCount; i++)
            {
                int nSrcRowIndex = cells[i].RowIndex;
                int nSrcColIndex = cells[i].ColumnIndex;

                int nIndex1 = nSrcRowIndex - nBeginRowIndex;
                int nIndex2 = nSrcColIndex - nBeginColIndex;

                int nTrgRowIndex = nRowIndex + nIndex1;
                int nTrgColIndex = nColIndex + nIndex2;

                if (nTrgRowIndex > 0)
                    continue;

                if (nTrgColIndex < dataGridView1.Columns.Count)
                {
                    m_cellValueChangedByPasteCells = true;
                    dataGridView1.Rows[nTrgRowIndex].Cells[nTrgColIndex].Value = cells[i].Value;
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FormMain.Instance.CurrentPanel.ExcelGridLinker = this;
        }

        private void FormImportance_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormMain.Instance.CurrentPanel.ExcelGridLinker == this)
                FormMain.Instance.CurrentPanel.ExcelGridLinker = null;

            if (m_setting != null)
                m_setting.Close();
        }

        public DataGridViewSelectedCellCollection GetPastePositionCells()
        {
            return dataGridView1.SelectedCells;
        }

        public abstract class SettingImportance
        {
            public abstract ImportanceData Data
            {
                get;
                set;
            }

            public abstract void Close();
        }

        private void FormImportance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
                FormMain.Instance.ShowHelp();
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
                FormMain.Instance.ShowHelp();
        }
    }
}
