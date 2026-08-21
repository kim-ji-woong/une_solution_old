using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeatherSimulator
{
    public class GridManager
    {
        private DataGridView m_grid = null;
        private List<int> m_columnIndeces = new List<int>();
        private IGridOwner<float> m_owner = null;

        public List<int> ColumnIndeces
        {
            get { return m_columnIndeces; }
        }

        public GridManager(DataGridView grid, IGridOwner<float> owner)
        {
            m_grid = grid;
            m_owner = owner;

            m_grid.MultiSelect = true;

            m_grid.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(OnCellBeginEdit);
            m_grid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(OnCellEndEdit);
            m_grid.KeyDown += new System.Windows.Forms.KeyEventHandler(OnKeyDown);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            int nCount = m_grid.SelectedCells.Count;

            if (nCount != 1)
                return;

            if (m_grid.ReadOnly)
                return;

            int nColumnIndex = m_grid.SelectedCells[0].ColumnIndex;
            int nRowIndex = m_grid.SelectedCells[0].RowIndex;

            if (nRowIndex < 0 || nColumnIndex <= 1)
                return;

            DataGridViewRow row = m_grid.Rows[nRowIndex];

            if (row.IsNewRow)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                row.Cells[nColumnIndex].Value = null;
                row.Cells[nColumnIndex].Tag = null;
            }
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
                if (MessageBox.Show("삭제할까요?", "확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    removeRowIndecs.Sort();
                    RemoveRows(removeRowIndecs);
                }
            }
        }

        private void RemoveRows(List<int> removeRowIndecs)
        {
            int nIndexCount = removeRowIndecs.Count;

            for (int i=nIndexCount-1;i>=0;i--)
            {
                int nRowIndex = removeRowIndecs[i];
                m_grid.Rows.RemoveAt(nRowIndex);
            }
        }

        private void OnCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = m_grid.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            if (m_columnIndeces.Contains(e.ColumnIndex))
            {
                if (row.Cells[e.ColumnIndex].Tag != null)
                {
                    VariousData<float> data = (VariousData<float>)row.Cells[e.ColumnIndex].Tag;
                    row.Cells[e.ColumnIndex].Value = data.Data.ToString();
                }
            }
        }

        private void OnCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = m_grid.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            if (m_columnIndeces.Contains(e.ColumnIndex))
            {
                string strValue = row.Cells[e.ColumnIndex].Value == null ? "" : row.Cells[e.ColumnIndex].Value.ToString().Trim();

                if (strValue.Length == 0)
                {
                    row.Cells[e.ColumnIndex].Value = null;
                    row.Cells[e.ColumnIndex].Tag = null;
                }
                else
                {
                    float fData;

                    if (float.TryParse(strValue, out fData) && m_owner.IsValidData(fData, e.ColumnIndex))
                    {
                        VariousData<float> data = new VariousData<float>(fData);
                        row.Cells[e.ColumnIndex].Value = m_owner.GetCellValueString(data, e.ColumnIndex);
                        row.Cells[e.ColumnIndex].Tag = data;
                    }
                    else
                    {
                        if (row.Cells[e.ColumnIndex].Tag != null)
                        {
                            VariousData<float> data = (VariousData<float>)row.Cells[e.ColumnIndex].Tag;
                            row.Cells[e.ColumnIndex].Value = m_owner.GetCellValueString(data, e.ColumnIndex);
                        }
                        else
                            row.Cells[e.ColumnIndex].Value = null;
                    }
                }
            }
            else if (e.ColumnIndex == 1)
            {
                string strValue = row.Cells[e.ColumnIndex].Value == null ? "" : row.Cells[e.ColumnIndex].Value.ToString().Trim();

                if (strValue.Length == 0)
                {
                    row.Cells[e.ColumnIndex].Value = null;
                    row.Cells[e.ColumnIndex].Tag = null;
                }
                else
                {
                    DateTime time = new DateTime();

                    if (TimePickerManager.GetDateTime(strValue, ref time))
                    {
                        VariousData<DateTime> data = new VariousData<DateTime>(time);
                        row.Cells[e.ColumnIndex].Value = WeatherData.MakeTimeString(time);
                        row.Cells[e.ColumnIndex].Tag = data;
                    }
                    else
                    {
                        if (row.Cells[e.ColumnIndex].Tag != null)
                        {
                            VariousData<DateTime> data = (VariousData<DateTime>)row.Cells[e.ColumnIndex].Tag;
                            row.Cells[e.ColumnIndex].Value = WeatherData.MakeTimeString(data.Data);
                        }
                        else
                            row.Cells[e.ColumnIndex].Value = null;
                    }
                }
            }
        }
    }

    public interface IGridOwner<Type>
    {
        string GetCellValueString(VariousData<Type> data, int nColumnIndex);
        bool IsValidData(Type data, int nColumnIndex);
    }
}
