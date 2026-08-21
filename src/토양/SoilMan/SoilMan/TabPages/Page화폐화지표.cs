using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SoilMan.TabPages
{
    public partial class Page화폐화지표 : Form, IGridPage
    {
        public const int NoIndex = 0;
        public const int FunctionNameIndex = 1;
        //public const int FunctionUnitIndex = 2;
        public const int 화폐화지표NameIndex = 2;
       // public const int 화폐화지표UnitIndex = 4;
        public const int TotalAmountIndex = 3;

        private GridManager화폐화지표 m_gridMgr = null;

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

        public Page화폐화지표()
        {
            InitializeComponent();

            m_gridMgr = new GridManager화폐화지표("[기능별 화폐화지표]", dataGridView1);
            m_gridMgr.ReadConfig(Application.StartupPath + "\\Config.ini");
            TabPageManager.Instance.InitStyle(this, dataGridView1, checkBoxEditMode, btnSave, labelUnitInfo, "단위 : 원/ton");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.Rows.Clear();
            GridManager화폐화지표 m_gridMgr2 = new GridManager화폐화지표("[기능별 화폐화지표_Default]", dataGridView1);
            m_gridMgr2.ReadConfig(Application.StartupPath + "\\Config.ini");
            btnSave.PerformClick();
            dataGridView1.Refresh();
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

                    if (i == TotalAmountIndex)
                        cell.ReadOnly = false;
                    else
                        cell.ReadOnly = true;
                }
            }
        }

        private void Page화폐화지표_Load(object sender, EventArgs e)
        {
            //dataGridView1.MergeColumns(FunctionNameIndex, FunctionUnitIndex);
            //dataGridView1.MergeColumns(화폐화지표NameIndex, 화폐화지표UnitIndex);
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex == TotalAmountIndex)
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
                            cell.Value = m_gridMgr.GetCellString((long)cell.Tag);

                        return;
                    }

                    if (data == 0.0)
                        cell.Value = "0";
                    else
                        cell.Value = m_gridMgr.GetCellString((long)data);

                    if (cell.Tag == null || (double)cell.Tag != data)
                        TabPageManager.Instance.OnDataChanged(this);

                    cell.Tag = (long)data;
                }

                m_gridMgr.ResetAmount(e.RowIndex);
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        public DataGridViewCell GetFunctionData(SoilFunctionType functionType)
        {
            int nRowIndex = (int)functionType;

            if (dataGridView1.Rows.Count <= nRowIndex)
                return null;

            int nColumnIndex = TotalAmountIndex;

            return dataGridView1.Rows[nRowIndex].Cells[nColumnIndex];
        }

       
    }

    class GridManager화폐화지표 : GridManager
    {
        public GridManager화폐화지표(string strSectionName, UnE.Controls.MergedDataGridView grid)
            : base(strSectionName, grid)
        {
        }

        public string GetCellString(long nPrice)
        {
            return string.Format("{0:###,###,###,###,###,###}", nPrice);
        }

        public override void ResetAmount(int nRowIndex)
        {
            DataGridViewCell cell = m_grid.Rows[nRowIndex].Cells[Page화폐화지표.TotalAmountIndex];
            
            if (cell.Value != null)
            {
                string strValue = cell.Value.ToString().Trim();

                if (strValue.Length > 0)
                {
                    long nPrice;

                    if (long.TryParse(strValue, System.Globalization.NumberStyles.AllowThousands, null, out nPrice))
                    {
                        cell.Value = GetCellString(nPrice);
                        cell.Tag = nPrice;
                        return;
                    }
                }
            }

            cell.Value = null;
            cell.Tag = null;
        }
    }
}
