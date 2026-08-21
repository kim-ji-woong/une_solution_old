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
    public partial class Page계량지표 : Form, IGridPage
    {
        public const int NoIndex = 0;
        public const int FunctionNameIndex = 1;
        //public const int FunctionUnitIndex = 2;
        public const int 계량지표NameIndex = 2;
        //public const int 계량지표UnitIndex = 4;
        public const int GeneralIndex = 3;
        public const int FieldIndex = 4;
        public const int RiceFieldIndex = 5;
        public const int MountainIndex = 6;
        public const int TotalAmountIndex = 7;

        private GridManager계량지표 m_gridMgr = null;

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

        public Page계량지표()
        {
            InitializeComponent();

            m_gridMgr = new GridManager계량지표("[기능별 계량지표]", dataGridView1);
            m_gridMgr.ReadConfig(Application.StartupPath + "\\Config.ini");
            TabPageManager.Instance.InitStyle(this, dataGridView1, checkBoxEditMode, btnSave, labelUnitInfo, "단위 : ton/ha");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.Rows.Clear();
            GridManager계량지표 m_gridMgr2 = new GridManager계량지표("[기능별 계량지표_Default]", dataGridView1);
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
                for (int i=0;i<nColumnCount;i++)
                {
                    DataGridViewCell cell = row.Cells[i];

                    if (i >= GeneralIndex && i <= MountainIndex)
                        cell.ReadOnly = false;
                    else
                        cell.ReadOnly = true;
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex >= GeneralIndex && e.ColumnIndex <= MountainIndex)
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

                m_gridMgr.ResetAmount(e.RowIndex);
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void Page계량지표_Load(object sender, EventArgs e)
        {
           //dataGridView1.MergeColumns(FunctionNameIndex, FunctionUnitIndex);
            dataGridView1.MergeColumns(계량지표NameIndex, GeneralIndex);
        }

        public DataGridViewCell GetFunctionData(SoilFunctionType functionType, LandType landType)
        {
            int nRowIndex = (int)functionType;

            if (dataGridView1.Rows.Count <= nRowIndex)
                return null;

            int nColumnIndex = -1;

            if (landType == LandType.General)
                nColumnIndex = GeneralIndex;
            else if (landType == LandType.Field)
                nColumnIndex = FieldIndex;
            else if (landType == LandType.RiceField)
                nColumnIndex = RiceFieldIndex;
            else if (landType == LandType.Mountain)
                nColumnIndex = MountainIndex;
            else
                return null;

            return dataGridView1.Rows[nRowIndex].Cells[nColumnIndex];
        }

    }

    class GridManager계량지표: GridManager
    {
        public GridManager계량지표(string strSectionName, UnE.Controls.MergedDataGridView grid)
            : base(strSectionName, grid)
        {
        }

        public override void ResetAmount(int nRowIndex)
        {
            double dAmount = 0.0, data;
            bool hasValue = false;

            DataGridViewRow row = m_grid.Rows[nRowIndex];

            for (int i = Page계량지표.GeneralIndex; i <= Page계량지표.MountainIndex; i++)
            {
                DataGridViewCell cell = row.Cells[i];

                if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
                    continue;

                if (double.TryParse(cell.Value.ToString().Trim(), out data))
                {
                    cell.Tag = data;
                    dAmount += data;
                    hasValue = true;
                }
                else
                    cell.Tag = null;
            }

            if (hasValue)
            {
                row.Cells[Page계량지표.TotalAmountIndex].Value = GetDoubleString(dAmount);
                row.Cells[Page계량지표.TotalAmountIndex].Tag = dAmount;
            }
            else
            {
                row.Cells[Page계량지표.TotalAmountIndex].Value = null;
                row.Cells[Page계량지표.TotalAmountIndex].Tag = null;
            }
        }

        public string GetDoubleString(double data)
        {
            return string.Format("{0:F2}", data);
        }
    }
}
