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
    public partial class FormEconomicSummary : Form
    {
        public const int FunctionNameIndex = 0;
        //public const int FunctionUnitIndex = 1;
        public const int GeneralIndex = 1;
        public const int FieldIndex = 2;
        public const int RiceFieldIndex = 3;
        public const int MountainIndex = 4;
        public const int TotalAmountIndex = 5;

        public DataGridView Values
        {
            get { return dataGridView1; }
        }

        public FormEconomicSummary()
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

            // Column 감추기
            dataGridView1.ColumnHeadersVisible = true;
        }

        private double GetCellValue(DataGridView grid, int rowIdx, int nColIdx)
        {
            DataGridViewCell cell = grid.Rows[rowIdx].Cells[nColIdx];
            string szTemp = cell.Value.ToString();
            double dResult = 0.0;
            double.TryParse(szTemp, out dResult);
            return dResult;            
        }

        private double GetCellValueRange(DataGridView grid, int rowIdx, int nColIdx, int nTargetRow, int nTargetColIdx)
        {
            double dResult = 0.0;

            for(int i = rowIdx; i <= nTargetRow; i++)
            {
                for(int j = nColIdx; j <= nTargetColIdx; j++)
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j];
                    string szTemp = cell.Value.ToString();
                    double dTemp = 0.0;
                    double.TryParse(szTemp, out dTemp);

                    dResult += dTemp;
                }
            }  
            return dResult;            
        }

        public void Show(DataGridView gridCondition,  DataGridView gridEconomicValues)
        {
            dataGridView1.Rows.Clear();



            double dGeneralTotal = 0.0, dFieldTotal = 0.0, dRiceFieldTotal = 0.0, dMountainTotal = 0.0, dAmount = 0.0;

            Color col1 = Color.FromArgb(235, 241, 222);
            Color col2 = Color.FromArgb(238, 236, 225);

            int nGridHeight = colFunction.HeaderCell.Size.Height + 2;
                     
            long nYear = 0;
            string szTemp1 = gridCondition.Rows[4].Cells[1].Value.ToString();
            long.TryParse(szTemp1, out nYear);
            
            if (nYear == 0)
                return;

            //2,1
            

            // 정화 직후
            double [] dCost = new double[5]; // 비용            
            double [] dBenefit = new double[5]; //편익
            double [] dNetProfitValue = new double[5]; // 순편익 
            double [] dNetProfitInputDiscount = new double[5]; // 입력할인율 NPV
            double [] dNetProfitTwoRateDiscount = new double[5];// 2% NPV
            double [] dNetProfitFourRateDiscount = new double[5]; // 4% NPV
            double [] dNetProfitSixRateDiscount = new double[5]; // 6% NPV

            string [] szHeaders = { "정화직후","10년후","20년후","30년후","{0}년후(입력값)"  };
            long [] nPeriods = { 0, 10, 20, 30, nYear };

            DataGridView grid = gridEconomicValues;
            long nRowCount = 5;
            
            
            for(int i = 2 ; i < nRowCount + 2; i++)
            {
                dCost[i-2] = GetCellValue(grid,2,1) + GetCellValue(grid,2,2);
                int nTargetRow = (int)nPeriods[i-2] + 2;
                dBenefit[i - 2] = GetCellValueRange(grid, 2, 3, nTargetRow, 5);
                dNetProfitValue[i - 2] = dBenefit[i - 2] - dCost[i - 2];
                dNetProfitInputDiscount[i - 2] = GetCellValue(grid, nTargetRow, 7);
                dNetProfitTwoRateDiscount[i - 2] = GetCellValue(grid, nTargetRow, 9);
                dNetProfitFourRateDiscount[i - 2] = GetCellValue(grid, nTargetRow, 11);
                dNetProfitSixRateDiscount[i - 2] = GetCellValue(grid, nTargetRow, 13);
            }   
            
            for (int i = 0; i < nRowCount; i++)
            {
                               
                DataGridViewRow row = new DataGridViewRow();


                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                                   
                if( i == 4)
                {
                    cell.Value = string.Format(szHeaders[i], nYear);
                }
                else
                {
                    cell.Value = szHeaders[i];
                }                   
                cell.Style.BackColor = col1;
                cell.Tag = i;
                row.Cells.Add(cell);
               
                cell = new DataGridViewTextBoxCell();
                cell.Value = CostString2(dCost[i]);
                cell.Style.BackColor = col1;
                cell.Tag = dCost[i];
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = CostString2(dBenefit[i]);
                cell.Style.BackColor = col1;
                cell.Tag = dBenefit[i];
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = CostString2(dNetProfitValue[i]);
                cell.Style.BackColor = col1;
                cell.Tag = dNetProfitValue[i];
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = CostString2(dNetProfitInputDiscount[i]);
                cell.Style.BackColor = col1;
                cell.Tag = dNetProfitInputDiscount[i];
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = CostString2(dNetProfitTwoRateDiscount[i]);
                cell.Style.BackColor = col1;
                cell.Tag = dNetProfitTwoRateDiscount[i];
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = CostString2(dNetProfitFourRateDiscount[i]);
                cell.Style.BackColor = col1;
                cell.Tag = dNetProfitFourRateDiscount[i];
                row.Cells.Add(cell);
                
                cell = new DataGridViewTextBoxCell();
                cell.Value = CostString2(dNetProfitSixRateDiscount[i]);
                cell.Style.BackColor = col1;
                cell.Tag = dNetProfitSixRateDiscount[i];
                row.Cells.Add(cell);

                dataGridView1.Rows.Add(row);
                nGridHeight += row.Height;
            }

            dataGridView1.Size = new Size(dataGridView1.Size.Width, 326);

            base.Show();
        }
          
        public string CostString2(double dCost)
        {
            bool bAddMinus = false;
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
        // 단위 : 억원
        private double GetTotalCost(Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea)
        {
            double dTotalCost = 0.0;

            foreach (KeyValuePair<LandType, Overlay.AreaNCost> pair in dicLandTypeArea)
            {
                dTotalCost += pair.Value.Cost;
            }

            return dTotalCost / 100000000;
        }

        private double GetCapacity(DataGridView grid, int nRowIndex, int nColumnIndex)
        {
            if (nRowIndex >= grid.Rows.Count)
                return 0.0;

            DataGridViewCell cell = grid.Rows[nRowIndex].Cells[nColumnIndex];

            if (cell == null || cell.Tag == null)
                return 0.0;

            double dCapacity = (double)cell.Tag;
            return dCapacity;
        }

        private string GetValueString(double dWeight, double dArea, ref double dTotalFactor, ref double dSum)
        {
            if (dWeight == 0.0 && dArea == 0.0)
            {
                return "";
            }

            dTotalFactor = 1.0;

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


            return strValue;
        }
    }
}
