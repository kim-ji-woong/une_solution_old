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
    public partial class FormLandNumber : Form, IExcelGridLinker
    {
        private SettingLandNumber m_setting = null;
        //private DataGridViewCell m_cell = null;
        private List<LandAddressData> m_arrLandAddrs = null;
        private bool m_noPasteCell = false;

        private const int TOWN_NAME = 1;
        private const int MAJOR = 2;
        private const int HYPHEN = 3;
        private const int MINOR = 4;
        private const int TOTAL_AREA = 5;
        private const int STREET_AREA = 6;
        private const int OWNER_TYPE = 7;
        private const int PUBLIC_ESTIMATION = 8;
        
        public FormLandNumber(SettingLandNumber setting/*DataGridViewCell cell*/, string strAddr)
        {
            InitializeComponent();

            m_setting = setting;
            m_arrLandAddrs = m_setting.Data;
            
            this.Text = strAddr + " - 토지지번";
        }

        public void DisableEdit()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.ReadOnly = true;
            }

            m_noPasteCell = true;
        }

        private void InitGrid()
        {
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dataGridView1.Rows.Clear();

            if (m_arrLandAddrs != null)
            {
                int nIndex = 1;
                
                foreach (LandAddressData land in m_arrLandAddrs)
                {
                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[nIndex - 1].Clone();
                    dataGridView1.Rows.Add(row);

                    row = dataGridView1.Rows[nIndex - 1];
                    row.Tag = land;

                    row.Cells[0].Value = nIndex++;
                    row.Cells[TOWN_NAME].Value = land.TownName;
                    row.Cells[MAJOR].Value = land.MajorAddr;
                    row.Cells[HYPHEN].Value = "-";
                    row.Cells[MINOR].Value = land.MinorAddr;
                    SetAreaCell(row.Cells[TOTAL_AREA], land.TotalArea);
                    row.Cells[TOTAL_AREA].Tag = land.TotalArea;
                    SetAreaCell(row.Cells[STREET_AREA], land.StreetArea);
                    row.Cells[STREET_AREA].Tag = land.StreetArea;
                    row.Cells[OWNER_TYPE].Value = land.OwnerType;
                    SetCostCell(row.Cells[PUBLIC_ESTIMATION], land.PublicEstimation);
                    row.Cells[PUBLIC_ESTIMATION].Tag = land.PublicEstimation;
                }
            }

            if(dataGridView1.Rows.Count == 0)
                dataGridView1.Rows.Add();
        }

        public DataGridViewSelectedCellCollection GetSelectedCells()
        {
            return dataGridView1.SelectedCells;
        }

        public void PasteCells(DataGridViewSelectedCellCollection cells)
        {
            if (m_noPasteCell)
                return;

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

                if (nTrgColIndex < dataGridView1.Columns.Count)
                {
                    //if (nTrgColIndex >= 3 && nTrgColIndex <= 4)
                    {
                        //DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn)dataGridView1.Columns[nTrgColIndex];

                        //if (!column.Items.Contains(cells[i].Value))
                        //    column.Items.Add(cells[i].Value);
                    }

                    dataGridView1.Rows[nTrgRowIndex].Cells[nTrgColIndex].Value = cells[i].Value;
                    CellValueCheck(dataGridView1.Rows[nTrgRowIndex].Cells[nTrgColIndex]);
                }
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                row.Cells[0].Value = row.Index + 1;
                CellValueCheck(row.Cells[0]);
            }
        }

        private void GetMinIndex(DataGridViewSelectedCellCollection cells, out int nRowIndex, out int nColIndex)
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
            /*DataGridViewRow row = new DataGridViewRow();
            int nColumnCount = dataGridView1.Columns.Count;
            
            for (int i=0;i<nColumnCount;i++)
            {
                DataGridViewCell cell = GetDefaultCell(i);
                row.Cells.Add(cell);
            }

            dataGridView1.Rows.Add(row);*/
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (m_arrLandAddrs == null)
                return;

            m_arrLandAddrs.Clear();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                LandAddressData land = (LandAddressData)row.Tag;//new ScheduleProperty();

                if (row.Cells[TOWN_NAME].Value == null)
                    land.TownName = "";
                else
                    land.TownName = row.Cells[TOWN_NAME].Value.ToString();

                if (row.Cells[MAJOR].Value == null)
                    land.MajorAddr = "";
                else
                    land.MajorAddr = row.Cells[MAJOR].Value.ToString();

                if (row.Cells[MINOR].Value == null)
                    land.MinorAddr = "";
                else
                    land.MinorAddr = row.Cells[MINOR].Value.ToString();

                if (land.TownName.Length == 0 && land.MajorAddr.Length == 0 &&
                    land.MinorAddr.Length == 0)
                    continue;

                if (row.Cells[TOTAL_AREA].Tag == null)
                    land.TotalArea = null;
                else
                    land.TotalArea = (VariousData<double>)row.Cells[TOTAL_AREA].Tag;

                if (row.Cells[STREET_AREA].Tag == null)
                    land.StreetArea = null;
                else
                    land.StreetArea = (VariousData<double>)row.Cells[STREET_AREA].Tag;

                if (row.Cells[OWNER_TYPE].Value == null)
                    land.OwnerType = "";
                else
                    land.OwnerType = row.Cells[OWNER_TYPE].Value.ToString();

                if (row.Cells[PUBLIC_ESTIMATION].Tag == null)
                    land.PublicEstimation = null;
                else
                    land.PublicEstimation = (VariousData<long>)row.Cells[PUBLIC_ESTIMATION].Tag;

                m_arrLandAddrs.Add(land);
            }

            m_setting.Data = m_arrLandAddrs;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
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
            row.Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Tag = new LandAddressData();
        }

        private void FormLandNumber_Load(object sender, EventArgs e)
        {
            InitGrid();
            FormMain.Instance.CurrentPanel.ExcelGridLinker = this;
            dataGridView1.Select();
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex == TOTAL_AREA || e.ColumnIndex == STREET_AREA)
            {
                if (cell.Tag != null)
                {
                    double dArea = ((VariousData<double>)cell.Tag).Data;
                    cell.Value = string.Format("{0:F0}", dArea);
                }
            }
            else if (e.ColumnIndex == PUBLIC_ESTIMATION)
            {
                if (cell.Tag != null)
                {
                    long nCost = ((VariousData<long>)cell.Tag).Data;
                    cell.Value = nCost.ToString();
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex == TOTAL_AREA || e.ColumnIndex == STREET_AREA)
            {
                if (cell.Value != null && cell.Value.ToString().Length > 0)
                {
                    double dArea;

                    if (!double.TryParse(cell.Value.ToString(), out dArea) || dArea < 0.0)
                    {
                        string strMessage = string.Format("[{0}]은 0이상의 숫자만 입력 가능합니다.", dataGridView1.Columns[e.ColumnIndex].HeaderText);
                        UnE.Utility.UMessageBox.Show(this, strMessage);

                        SetAreaCell(cell, (VariousData<double>)cell.Tag);
                    }
                    else
                    {
                        cell.Tag = new VariousData<double>(dArea);
                        SetAreaCell(cell, (VariousData<double>)cell.Tag);
                    }
                }
                else
                {
                    cell.Tag = null;
                }
            }
            else if (e.ColumnIndex == PUBLIC_ESTIMATION)
            {
                if (cell.Value != null && cell.Value.ToString().Length > 0)
                {
                    long nCost;

                    if (!long.TryParse(cell.Value.ToString(), out nCost) || nCost < 0)
                    {
                        string strMessage = string.Format("[{0}]은 0이상의 숫자만 입력 가능합니다.", dataGridView1.Columns[e.ColumnIndex].HeaderText);
                        UnE.Utility.UMessageBox.Show(this, strMessage);

                        SetCostCell(cell, (VariousData<long>)cell.Tag);
                    }
                    else
                    {
                        cell.Tag = new VariousData<long>(nCost);
                        SetCostCell(cell, (VariousData<long>)cell.Tag);
                    }
                }
                else
                {
                    cell.Tag = null;
                }
            }
        }

        private void SetCostCell(DataGridViewCell cell, VariousData<long> nCost)
        {
            if (nCost == null)
                cell.Value = "";
            else
            {
                if (nCost.Data == 0)
                    cell.Value = "0원";
                else
                    cell.Value = string.Format("{0:###,###,###,###,###,###}원", nCost.Data);
            }
        }

        private void SetAreaCell(DataGridViewCell cell, VariousData<double> dArea)
        {
            if (dArea == null)
                cell.Value = "";
            else
            {
                if ((int)dArea.Data == 0)
                    cell.Value = "0";
                else
                    cell.Value = string.Format("{0:###,###,###,###,###,###}", (int)dArea.Data);
            }
        }

        private void CellValueCheck(DataGridViewCell cell)
        {
            if (cell.ColumnIndex == TOTAL_AREA || cell.ColumnIndex == STREET_AREA)
            {
                if (cell.Value != null && cell.Value.ToString().Length > 0)
                {
                    double dArea;

                    if (!double.TryParse(cell.Value.ToString(), out dArea) || dArea < 0.0)
                    {
                        SetAreaCell(cell, (VariousData<double>)cell.Tag);
                    }
                    else
                    {
                        cell.Tag = new VariousData<double>(dArea);
                        SetAreaCell(cell, (VariousData<double>)cell.Tag);
                    }
                }
                else
                {
                    cell.Tag = null;
                }
            }
            else if (cell.ColumnIndex == PUBLIC_ESTIMATION)
            {
                if (cell.Value != null && cell.Value.ToString().Length > 0)
                {
                    long nCost;

                    if (!long.TryParse(cell.Value.ToString(), out nCost) || nCost < 0)
                    {
                        SetCostCell(cell, (VariousData<long>)cell.Tag);
                    }
                    else
                    {
                        cell.Tag = new VariousData<long>(nCost);
                        SetCostCell(cell, (VariousData<long>)cell.Tag);
                    }
                }
                else
                {
                    cell.Tag = null;
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FormMain.Instance.CurrentPanel.ExcelGridLinker = this;
        }

        private void FormLandNumber_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (FormMain.Instance.CurrentPanel.ExcelGridLinker == this)
                FormMain.Instance.CurrentPanel.ExcelGridLinker = null;

            if (m_setting != null)
                m_setting.Close();
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

        public DataGridViewSelectedCellCollection GetPastePositionCells()
        {
            return dataGridView1.SelectedCells;
        }

        public abstract class SettingLandNumber
        {
            public abstract List<LandAddressData> Data
            {
                get;
                set;
            }

            public abstract void Close();
        }
    }
}
