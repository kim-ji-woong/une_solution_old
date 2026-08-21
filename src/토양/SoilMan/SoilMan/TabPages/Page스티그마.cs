using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoilMan.TabPages
{
    public partial class Page스티그마 : Form, IGridPage
    {
        public const int NoIndex = 0;
        public const int Stigma  = 1;
        public const int Recovery = 2;

        private GridManager스티그마 m_gridMgr = null;

        public bool EditMode
        {
            get { return checkBoxEditMode.Checked; }
            set { EnableGrid(value); }
        }

        public string ConfigSectionName
        {
            get { return m_gridMgr.SectionName; }
        }

        public DataGridView Datas
        {
            get { return dataGridView1; }
        }

        public Page스티그마()
        {
            InitializeComponent();

            m_gridMgr = new GridManager스티그마("[스티그마및회복기간]", dataGridView1);
            m_gridMgr.ReadConfig(Application.StartupPath + "\\Config.ini");
            TabPageManager.Instance.InitStyle(this, dataGridView1, checkBoxEditMode, btnSave, labelUnitInfo, "단위: %,년");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.Rows.Clear();
            GridManager스티그마 m_gridMgr2 = new GridManager스티그마("[스티그마및회복기간_Default]", dataGridView1);
            m_gridMgr2.ReadConfig(Application.StartupPath + "\\Config.ini");
            btnSave.PerformClick();
            dataGridView1.Refresh();
        }

        private void EnableGrid(bool enabled)
        {
            if (enabled)
                dataGridView1.ReadOnly = false;
            else
            {
                dataGridView1.ReadOnly = true;
                return;
            }

            int nColumnCount = dataGridView1.Columns.Count;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                for (int i = 0; i < nColumnCount; i++)
                {
                    DataGridViewCell cell = row.Cells[i];

                    if (i <= NoIndex)
                        cell.ReadOnly = true;
                    else
                        cell.ReadOnly = false;
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex >= Stigma && e.ColumnIndex <= Recovery)
            {
                if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
                {
                    if (cell.Tag != null)
                        TabPageManager.Instance.OnDataChanged(this);

                    cell.Value = null;
                    cell.Tag = null;
                }
                else
                {
                    string strValue = cell.Value.ToString().Trim();
                    double data;

                    if (!double.TryParse(strValue, out data) || data < 0.0)
                    {
                        UnE.Utility.UMessageBox.Show(this, "0 이상의 숫자만 입력이 가능합니다.");

                        if (cell.Tag == null)
                            cell.Value = null;
                        else
                            cell.Value = m_gridMgr.GetDoubleString((double)cell.Tag);

                        return;
                    }

                    if (data == 0.0)
                        cell.Value = "0";
                    else
                        cell.Value = m_gridMgr.GetDoubleString(data);

                    if (cell.Tag == null || (double)cell.Tag != data)
                        TabPageManager.Instance.OnDataChanged(this);

                    cell.Tag = data;
                }
            }

            m_gridMgr.ResetAmount(e.RowIndex);
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
                
        public DataGridViewCell GetStigmaData()
        {
            return dataGridView1.Rows[0].Cells[1];
        }
       
        public DataGridViewCell GetRecoveryData()
        {
            return dataGridView1.Rows[0].Cells[2];
        }
      
    }

    class GridManager스티그마 : GridManager
    {
        public GridManager스티그마(string strSectionName, UnE.Controls.MergedDataGridView grid)
            : base(strSectionName, grid)
        {
        }

        public string GetDoubleString(double data)
        {
            return string.Format("{0:F2}", data);
        }

        public override void ResetAmount(int nRowIndex)
        {
            double data;

            DataGridViewRow row = m_grid.Rows[nRowIndex];

            for (int i = Page스티그마.Stigma; i <= Page스티그마.Recovery; i++)
            {
                DataGridViewCell cell = row.Cells[i];

                if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
                    continue;

                if (double.TryParse(cell.Value.ToString().Trim(), out data))
                    cell.Tag = data;
                else
                    cell.Tag = null;
            }
        }
    }
}
