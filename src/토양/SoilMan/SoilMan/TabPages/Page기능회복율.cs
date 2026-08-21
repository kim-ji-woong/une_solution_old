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
    public partial class Page기능회복율 : Form, IGridPage
    {
        public const int NoIndex = 0;
        public const int FunctionNameIndex = 1;
        //public const int FunctionUnitIndex = 2;
        public const int BioIndex = 2;
        public const int FarmingUnitIndex = 3;
        public const int SteamIndex = 4;
        public const int WashingIndex = 5;
        public const int OxidationIndex = 6;
        public const int HeatIndex = 7;

        //public const int TechIndex = 9;

        private GridManager기능회복율 m_gridMgr = null;

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

        public Page기능회복율()
        {
            InitializeComponent();

            m_gridMgr = new GridManager기능회복율("[토양정화기술별 기능회복율]", dataGridView1);
            m_gridMgr.ReadConfig(Application.StartupPath + "\\Config.ini");
            TabPageManager.Instance.InitStyle(this, dataGridView1, checkBoxEditMode, btnSave, labelUnitInfo, "");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            dataGridView1.Rows.Clear();
            GridManager기능회복율 m_gridMgr2 = new GridManager기능회복율("[토양정화기술별 기능회복율_Default]", dataGridView1);
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

                    if (i <= FunctionNameIndex)
                        cell.ReadOnly = true;
                    else
                        cell.ReadOnly = false;
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.ColumnIndex >= BioIndex && e.ColumnIndex <= HeatIndex)
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

        public DataGridViewCell GetFunctionData(SoilFunctionType functionType, TechType techType)
        {
            int nRowIndex = (int)functionType;

            if (dataGridView1.Rows.Count <= nRowIndex)
                return null;

            int nColumnIndex = -1;

            if (techType == TechType.Bio)
                nColumnIndex = BioIndex;
            else if (techType == TechType.Farm)
                nColumnIndex = FarmingUnitIndex;
            else if (techType == TechType.Steam)
                nColumnIndex = SteamIndex;
            else if (techType == TechType.Washing)
                nColumnIndex = WashingIndex;
            else if (techType == TechType.Oxidation)
                nColumnIndex = OxidationIndex;
            else if (techType == TechType.Heat)
                nColumnIndex = HeatIndex;
            else
                return null;

            return dataGridView1.Rows[nRowIndex].Cells[nColumnIndex];
        }

        private void Page기능회복율_Load(object sender, EventArgs e)
        {
            //dataGridView1.MergeColumns(FunctionNameIndex, FunctionUnitIndex);
        }

    }

    class GridManager기능회복율 : GridManager
    {
        public GridManager기능회복율(string strSectionName, UnE.Controls.MergedDataGridView grid)
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

            for (int i = Page기능회복율.BioIndex; i <= Page기능회복율.HeatIndex; i++)
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
