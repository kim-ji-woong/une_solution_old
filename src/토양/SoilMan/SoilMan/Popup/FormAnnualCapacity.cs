using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoilMan.Popup
{
    public partial class FormAnnualCapacity : Form
    {
        public const int FunctionNameIndex = 0;
        //public const int FunctionUnitIndex = 1;
        public const int GeneralIndex = 1;
        public const int FieldIndex = 2;
        public const int RiceFieldIndex = 3;
        public const int MountainIndex = 4;
        public const int TotalAmountIndex = 5;

        public DataGridView Capacities
        {
            get { return dataGridView1; }
        }

        public FormAnnualCapacity()
        {
            InitializeComponent();
            InitGrid();
        }

        private void InitGrid()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            //dataGridView1.MergeColumns(FunctionNameIndex, FunctionUnitIndex);

            dataGridView1.ColumnHeadersHeight = 25;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.BackColor = Color.FromArgb(195, 214, 155);
            }
        }

        public void Show(SoilCleanCost cost, Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea)
        {
            dataGridView1.Rows.Clear();

            double dGeneralTotal = 0.0, dFieldTotal = 0.0, dRiceFieldTotal = 0.0, dMountainTotal = 0.0, dAmount = 0.0;

            Color col1 = Color.FromArgb(235, 241, 222);
            Color col2 = Color.FromArgb(238, 236, 225);

            int nGridHeight = colFunction.HeaderCell.Size.Height + 2;

            for (int i=0;i<(int)SoilFunctionType.TypeCount;i++)
            {
                SoilFunctionType type = (SoilFunctionType)i;
                string strTypeName = SoilMan.FormMain.SoilFunctionTypeName(type);
                string strTypeUnit = SoilMan.FormMain.SoilFunctionTypeUnit(type);

                double dGeneral = FormMain.Instance.Get계량지표FunctionData(type, LandType.General);
                double dField = FormMain.Instance.Get계량지표FunctionData(type, LandType.Field);
                double dRiceField = FormMain.Instance.Get계량지표FunctionData(type, LandType.RiceField);
                double dMountain = FormMain.Instance.Get계량지표FunctionData(type, LandType.Mountain);

                double dGeneralArea = GetArea(LandType.General, dicLandTypeArea);
                double dFieldArea = GetArea(LandType.Field, dicLandTypeArea);
                double dRiceFieldArea = GetArea(LandType.RiceField, dicLandTypeArea);
                double dMountainArea = GetArea(LandType.Mountain, dicLandTypeArea);

                DataGridViewRow row = new DataGridViewRow();
                double dTotalFactor = 1.0;

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strTypeName;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                //cell = new DataGridViewTextBoxCell();
                //cell.Value = strTypeUnit;
                //cell.Style.BackColor = col1;
                //row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = GetValueString(dGeneral, dGeneralArea, ref dTotalFactor, ref dGeneralTotal);
                cell.Tag = dGeneral * dGeneralArea;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = GetValueString(dField, dFieldArea, ref dTotalFactor, ref dFieldTotal);
                cell.Tag = dField * dFieldArea;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = GetValueString(dRiceField, dRiceFieldArea, ref dTotalFactor, ref dRiceFieldTotal);
                cell.Tag = dRiceField * dRiceFieldArea;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = GetValueString(dMountain, dMountainArea, ref dTotalFactor, ref dMountainTotal);
                cell.Tag = dMountain * dMountainArea;
                row.Cells.Add(cell);

                double dTotal = dGeneral * dGeneralArea + dField * dFieldArea + dRiceField * dRiceFieldArea + dMountain * dMountainArea;

                cell = new DataGridViewTextBoxCell();
                cell.Value = GetValueString(dTotal, dTotalFactor, ref dTotalFactor, ref dAmount);
                cell.Style.BackColor = col2;
                row.Cells.Add(cell);

                dataGridView1.Rows.Add(row);
                nGridHeight += row.Height;
            }

            DataGridViewRow row2 = new DataGridViewRow();
            double dTemp = 0.0;

            DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
            cell2.Value = "계";
            cell2.Style.BackColor = col1;
            cell2.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row2.Cells.Add(cell2);

            //cell2 = new DataGridViewTextBoxCell();
            //cell2.Style.BackColor = col1;
            //row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dGeneralTotal, 1.0, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            cell2.Tag = dGeneralTotal / 10000;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dFieldTotal, 1.0, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            cell2.Tag = dFieldTotal / 10000;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dRiceFieldTotal, 1.0, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            cell2.Tag = dRiceFieldTotal / 10000;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dMountainTotal, 1.0, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            cell2.Tag = dMountainTotal / 10000;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dAmount, 1.0, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            row2.Cells.Add(cell2);

            dataGridView1.Rows.Add(row2);
            nGridHeight += row2.Height;

            int nRowCount = dataGridView1.Rows.Count;           
            dataGridView1.Size = new Size(dataGridView1.Size.Width, nGridHeight);

            base.Show();
        }

        private string GetValueString(double dWeight, double dArea, ref double dTotalFactor, ref double dSum)
        {
            if (dWeight == 0.0 && dArea == 0.0)
            {
                dTotalFactor = 0.0;
                return "";
            }

            double data = dWeight * dArea;
            dSum += data;

            string strValue = string.Format("{0:F2}", data);
            int nIndex = strValue.LastIndexOf('.');

            char chBelowFirst = strValue.ElementAt(nIndex + 1);
            char chBelowSecond = strValue.ElementAt(nIndex + 2);

            string strInt = strValue.Substring(0, nIndex);
            long nData = long.Parse(strInt);
            string strValue2 = string.Format("{0:###,###,###,###,###,###}", nData);

            if (chBelowSecond == '0')
            {
                if (chBelowFirst == '0')
                    strValue = strValue2;
                else
                    strValue = strValue2 + strValue.Substring(nIndex, 2);
            }
            else
                strValue = strValue2 + strValue.Substring(nIndex);

            if (strValue.Length > 0 && strValue.ElementAt(0) == '.')
                strValue = "0" + strValue;

            /*double underPoint = data - (int)data;
            string strUnderPoint = string.Format("{0:F2}", underPoint);
            string strUnderPoint2 = string.Format("{0:F1}", underPoint);
            string strValue = "";

            if (strUnderPoint == "0.00")
                strValue = string.Format("{0:###,###,###,###,###,###}", (int)data);
            else if (strUnderPoint2 == "0.0")
                strValue = string.Format("{0:###,###,###,###,###,###}", (int)data) + strUnderPoint.Substring(1);
            else
                strValue = string.Format("{0:###,###,###,###,###,###}", (int)data) + strUnderPoint2.Substring(1);*/

            return strValue;
        }

        // Return 값 : ha(10000m²)
        private double GetArea(LandType type, Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea)
        {
            Overlay.AreaNCost value;

            if (!dicLandTypeArea.TryGetValue(type, out value))
                return 0.0;

            // m²를 ha로 변환하므로 10000으로 나눈다.
            return value.Area / 10000;
        }
    }
}
