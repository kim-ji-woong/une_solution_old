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
    public partial class FormTotalValue : Form
    {
        public const int FunctionNameIndex = 0;
       // public const int FunctionUnitIndex = 1;
        public const int GeneralIndex = 1;
        public const int FieldIndex = 2;
        public const int RiceFieldIndex = 3;
        public const int MountainIndex = 4;
        public const int TotalAmountIndex = 5;

        public DataGridView Values
        {
            get { return dataGridView1; }
        }

        public FormTotalValue()
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

        public void Show(DataGridView gridValue, TechType techType, SoilCleanCost cost)
        {
            dataGridView1.Rows.Clear();

            double dGeneralTotal = 0.0, dFieldTotal = 0.0, dRiceFieldTotal = 0.0, dMountainTotal = 0.0, dAmount = 0.0;

            Color col1 = Color.FromArgb(235, 241, 222);
            Color col2 = Color.FromArgb(238, 236, 225);

            int nGridHeight = colFunction.HeaderCell.Size.Height + 2;

            for (int i = 0; i < (int)SoilFunctionType.TypeCount; i++)
            {
                SoilFunctionType type = (SoilFunctionType)i;
                string strTypeName = SoilMan.FormMain.SoilFunctionTypeName(type);
                string strTypeUnit = SoilMan.FormMain.SoilFunctionTypeUnit(type);

                double dRecovery = FormMain.Instance.Get기능회복율FunctionData(type, techType);
                double dRecoveryPeriod = FormMain.Instance.Get기능회복기간FunctionData(type, techType);
                
                double dGeneralValue = GetValue(gridValue, i, GeneralIndex);
                double dFieldValue = GetValue(gridValue, i, FieldIndex);
                double dRiceFieldValue = GetValue(gridValue, i, RiceFieldIndex);
                double dMountainValue = GetValue(gridValue, i, MountainIndex);

                DataGridViewRow row = new DataGridViewRow();
                double dTotalFactor = 0.0;

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strTypeName;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                //cell = new DataGridViewTextBoxCell();
                //cell.Value = strTypeUnit;
                //cell.Style.BackColor = col1;
                //row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                double dGeneral = CalcValue(cost.Discount, cost.Period, dGeneralValue, dRecovery, dRecoveryPeriod);
                cell.Value = GetValueString(dGeneral, ref dTotalFactor, ref dGeneralTotal);
                cell.Tag = dGeneral;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                double dField = CalcValue(cost.Discount, cost.Period, dFieldValue, dRecovery, dRecoveryPeriod);
                cell.Value = GetValueString(dField, ref dTotalFactor, ref dFieldTotal);
                cell.Tag = dField;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                double dRiceField = CalcValue(cost.Discount, cost.Period, dRiceFieldValue, dRecovery, dRecoveryPeriod);
                cell.Value = GetValueString(dRiceField, ref dTotalFactor, ref dRiceFieldTotal);
                cell.Tag = dRiceField;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                double dMountain = CalcValue(cost.Discount, cost.Period, dMountainValue, dRecovery, dRecoveryPeriod);
                cell.Value = GetValueString(dMountain, ref dTotalFactor, ref dMountainTotal);
                cell.Tag = dMountain;
                row.Cells.Add(cell);

                double dTotal = dGeneral + dField + dRiceField + dMountain;

                cell = new DataGridViewTextBoxCell();

                int dTotalRecoveryPeriod = (int)FormMain.Instance.Get회복기간();
                double dAnualRecovery = FormConfirmArea.AnualRecoveryCost;

                if (i == (int)SoilFunctionType.식물생산기능)
                {
                    if( dTotalRecoveryPeriod < cost.Period )
                    {
                        dTotal = CalcValue(cost.Discount, dTotalRecoveryPeriod, dAnualRecovery, 1.0, 0.0);
                        cell.Value = CostString(dTotal);
                    }
                    else
                    {
                        dTotal = CalcValue(cost.Discount, cost.Period, dAnualRecovery, 1.0, 0.0);
                        cell.Value = CostString(dTotal);
                    }
                    dTotal = dTotal / 100000000;
                    GetValueString(dTotal, ref dTotalFactor, ref dAmount);
                }
                else if (i <= (int)SoilFunctionType.수질정화)
                {
                    cell.Value = GetValueString(dTotal, ref dTotalFactor, ref dAmount);                   
                }
                else
                {
                    if (i == (int)SoilFunctionType.구조물지지기능 || i == (int)SoilFunctionType.원료공급기능)
                        dTotal = 0.0;
                    else if (i >= (int)SoilFunctionType.유산가치 && i <= (int)SoilFunctionType.생태학적가치)
                    {
                        double dEconomicValue = GetValue(gridValue, i, TotalAmountIndex);   
                        dTotal = CalcValue(cost.Discount, cost.Period, dEconomicValue, 1.0, 0.0);
                    }
                    else
                        continue;

                    cell.Value = GetValueString(dTotal, ref dTotalFactor, ref dAmount);
                }

                cell.Style.BackColor = col2;
                cell.Tag = dTotal;
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
            cell2.Value = GetValueString(dGeneralTotal, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dFieldTotal, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dRiceFieldTotal, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dMountainTotal, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dAmount, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            row2.Cells.Add(cell2);

            dataGridView1.Rows.Add(row2);
            nGridHeight += row2.Height;

            int nRowCount = dataGridView1.Rows.Count;
            //dataGridView1.MergeCells(nRowCount - 1, 0, nRowCount - 1, 1);
            dataGridView1.MergeCells((int)SoilFunctionType.식물생산기능, TotalAmountIndex, (int)SoilFunctionType.원료공급기능, TotalAmountIndex);

            dataGridView1.Size = new Size(dataGridView1.Size.Width, nGridHeight);

            base.Show();
        }

        public string CostString(double dCost)
        {
            bool bAddMinus = false;
            dCost = dCost / 100000000;
            if (dCost < 0.0)
            {
                bAddMinus = true;
                dCost *= -1.0;
            }

            string strCost = string.Format("{0:F2}", dCost);
            int nIndex = strCost.LastIndexOf('.');

            char chBelowFirst = strCost.ElementAt(nIndex + 1);
            char chBelowSecond = strCost.ElementAt(nIndex + 2);

            string strInt = strCost.Substring(0, nIndex);
            long nData = long.Parse(strInt);
            string strValue2 = string.Format("{0:###,###,###,###,###,###}", nData);

            if (chBelowSecond == '0')
            {
                if (chBelowFirst == '0')
                    strCost = strValue2;
                else
                    strCost = strValue2 + strCost.Substring(nIndex, 2);
            }
            else
                strCost = strValue2 + strCost.Substring(nIndex);

            if (strCost.Length > 0 && strCost.ElementAt(0) == '.')
                strCost = "0" + strCost;

            if (bAddMinus == true)
                strCost = "-" + strCost;

            return strCost;
        }

        private double CalcValue(double dDiscount, int nAnalysisPeriod, double dEconomicValue, double dRecovery, double dRecoveryPeriod)
        {
            // 할인율은 백분율이므로 100으로 나눈다.
            dDiscount /= 100;

            double valueYrFr = 0.0, dResult = 0.0;

            for (int i=0 ; i <= nAnalysisPeriod;i++)
            {
                if (dRecoveryPeriod <= (double)nAnalysisPeriod)
                    valueYrFr = dEconomicValue * dRecovery;
                else
                    valueYrFr = dEconomicValue;

                double pwf = 1.0 / System.Math.Pow((1.0 + dDiscount), i);
                dResult += valueYrFr * pwf;
            }

            return dResult;
        }

        private double GetValue(DataGridView grid, int nRowIndex, int nColumnIndex)
        {
            if (nRowIndex >= grid.Rows.Count)
                return 0.0;

            DataGridViewCell cell = grid.Rows[nRowIndex].Cells[nColumnIndex];

            if (cell == null || cell.Tag == null)
                return 0.0;

            double dCapacity = (double)cell.Tag;
            return dCapacity;
        }

        private string GetValueString(double dValue, ref double dTotalFactor, ref double dSum)
        {
            if (dValue == 0.0)
            {
                return "";
            }

            dTotalFactor = 1.0;

            dSum += dValue;

            string strValue = string.Format("{0:F2}", dValue);
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

            return strValue;
        }
    }
}
