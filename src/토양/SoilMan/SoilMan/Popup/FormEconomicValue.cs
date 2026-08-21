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
    public partial class FormEconomicValue : Form
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

        public FormEconomicValue()
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
            dataGridView1.ColumnHeadersVisible = false;
        }

        public void Show(DataGridView gridCondition,  TechType techType, Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea, DataGridView gridCostValue, DataGridView gridAnnualCapacity, DataGridView gridPublicCost)
        {
            dataGridView1.Rows.Clear();

            // Column을 대신할 Row를 추가한다.
            AddColumnRows();

            double dGeneralTotal = 0.0, dFieldTotal = 0.0, dRiceFieldTotal = 0.0, dMountainTotal = 0.0, dAmount = 0.0;

            Color col1 = Color.FromArgb(235, 241, 222);
            Color col2 = Color.FromArgb(238, 236, 225);

            int nGridHeight = colFunction.HeaderCell.Size.Height + 2;

            double dCost = 0.0;
            double dExtraCost = 0.0;
            long nYear = 0;
            double dDiscountRate = 0.0;
            long dRecoveryPeriod = (long)FormMain.Instance.Get회복기간();
            double dYearRecoveryCost = 0.0;

            string szTemp1 = gridCondition.Rows[2].Cells[1].Value.ToString();
            double.TryParse(szTemp1, out dCost);

            szTemp1 = gridCondition.Rows[3].Cells[1].Value.ToString();
            double.TryParse(szTemp1, out dExtraCost);

            szTemp1 = gridCondition.Rows[4].Cells[1].Value.ToString();
            long.TryParse(szTemp1, out nYear);

            szTemp1 = gridCondition.Rows[5].Cells[1].Value.ToString();
            double.TryParse(szTemp1, out dDiscountRate);

            szTemp1 = gridPublicCost.Rows[5].Cells[1].Value.ToString();
            double.TryParse(szTemp1, out dYearRecoveryCost);

            dCost *= 100000000;
            dExtraCost *= 100000000;
            dYearRecoveryCost *= 100000000;


            if (nYear == 0)
                return;

            double dPrevNPV1 = 0.0, dPrevNPV2 = 0.0, dPrevNPV3 = 0.0, dPrevNPV4 = 0.0;
            long nColCount = nYear < 30 ? 30 : nYear;
            for (int i = 0; i <= nColCount; i++ )
            {
                               
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = i;
                cell.Style.BackColor = col1;
                cell.Tag = i;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                if (i == 0)
                    cell.Value = CostString(dCost);
                else
                {
                    dCost = 0;
                    cell.Value = "";
                }
                cell.Style.BackColor = col1;
                cell.Tag = dCost;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                if (i == 0)
                    cell.Value = CostString(dExtraCost);
                else
                {
                    dExtraCost = 0;
                    cell.Value = "";
                }
                cell.Tag = dExtraCost;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);


                cell = new DataGridViewTextBoxCell();
                if( i <= dRecoveryPeriod )
                {
                    // 연간 회복금액
                    cell.Value = CostString(dYearRecoveryCost);
                }
                else
                {
                    cell.Value = 0;
                }
                cell.Tag = dYearRecoveryCost;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                ///////////////////////////////////////////////////////////
                #region 간접사용가치

                double d간접사용가치 = 0.0;
                for( int k = (int)SoilFunctionType.오염물질정화 ; k <= (int)SoilFunctionType.수질정화; k++)
                {
                    string szTemp = gridAnnualCapacity.Rows[k].Cells[5].Value.ToString();
                    double dTemp = 0.0;
                    double.TryParse(szTemp, out dTemp);
                    double value1 = FormMain.Instance.Get화폐화지표FunctionData((SoilFunctionType)k) * dTemp;
                    if (FormMain.Instance.Get기능회복기간FunctionData((SoilFunctionType)k, techType) <= i)
                    {
                        double dTechRecoveryRatio = FormMain.Instance.Get기능회복율FunctionData((SoilFunctionType)k, techType);
                        value1 = value1 * dTechRecoveryRatio;
                    }
                    d간접사용가치 += value1;
                }
                //d간접사용가치 /=  100000000;
               
                cell = new DataGridViewTextBoxCell();
                if( d간접사용가치 > 0.0)
                {
                    cell.Value = CostString(d간접사용가치);
                }
                else
                {
                    cell.Value = "";
                }
                cell.Tag = d간접사용가치;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);
                #endregion// 간접사용가치
                ///////////////////////////////////////////////////////////

                double d연간비사용가치 = 0.0;
                string szTemp2 = gridCostValue.Rows[9].Cells[1].Value.ToString();
                double.TryParse(szTemp2, out d연간비사용가치);

                d연간비사용가치 *= 100000000;
                cell = new DataGridViewTextBoxCell();
                if (d연간비사용가치 > 0.0)
                {
                    cell.Value = CostString(d연간비사용가치);
                }
                else
                {
                    cell.Value = "";
                }
                cell.Tag = d연간비사용가치;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);


                // 순편익 입력값 PV
                double dPV = ((dYearRecoveryCost + d간접사용가치 + d연간비사용가치) - (dCost + dExtraCost)) * (1.0 / Math.Pow((1.0 + (dDiscountRate / 100.0)), (double)i));
                cell = new DataGridViewTextBoxCell();
                
                cell.Value = CostString(dPV);
                
                  
                cell.Tag = dPV;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                // 순편익 입력값 NPV
                double dNPV = dPrevNPV1 + dPV;
                cell = new DataGridViewTextBoxCell();
                
                cell.Value = CostString(dNPV);
              
                cell.Tag = dNPV;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                dPrevNPV1 = dNPV;

                // 순편익 2% PV
                double dPV2 = ((dYearRecoveryCost + d간접사용가치 + d연간비사용가치) - (dCost + dExtraCost)) * (1.0 / Math.Pow((1.02), (double)i));
                cell = new DataGridViewTextBoxCell();
               
                    cell.Value = CostString(dPV2);
                
                cell.Tag = dPV2;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                // 순편익 2% NPV
                double dNPV2 = dPrevNPV2 + dPV2;
                cell = new DataGridViewTextBoxCell();
                
                    cell.Value = CostString(dNPV2);
                
                cell.Tag = dNPV2;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                dPrevNPV2 = dNPV2;

                // 순편익 4% PV
                double dPV3 = ((dYearRecoveryCost + d간접사용가치 + d연간비사용가치) - (dCost + dExtraCost)) * (1.0 / Math.Pow((1.04), (double)i));
                cell = new DataGridViewTextBoxCell();
                
                    cell.Value = CostString(dPV3);
                
                cell.Tag = dPV3;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                // 순편익 4% NPV
                double dNPV3 = dPrevNPV3 + dPV3;
                cell = new DataGridViewTextBoxCell();
                
                    cell.Value = CostString(dNPV3);
                
                cell.Tag = dNPV3;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                dPrevNPV3 = dNPV3;

                // 순편익 6% PV
                double dPV4 = ((dYearRecoveryCost + d간접사용가치 + d연간비사용가치) - (dCost + dExtraCost)) * (1.0 / Math.Pow((1.06), (double)i));
                cell = new DataGridViewTextBoxCell();
                
                    cell.Value = CostString(dPV4);
                
                cell.Tag = dPV4;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                // 순편익 4% NPV
                double dNPV4 = dPrevNPV4 + dPV4;
                cell = new DataGridViewTextBoxCell();
                
                    cell.Value = CostString(dNPV4);
                
                cell.Tag = dNPV4;
                cell.Style.BackColor = col1;
                row.Cells.Add(cell);

                dPrevNPV4 = dNPV4;

                dataGridView1.Rows.Add(row);
                nGridHeight += row.Height;
            }

              dataGridView1.Size = new Size(dataGridView1.Size.Width, 326);

            base.Show();
        }

        // Column을 대신할 Row를 추가한다.
        private void AddColumnRows()
        {
            Color backColor = dataGridView1.Columns[0].HeaderCell.Style.BackColor;

            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = "년수";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "비용";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "직접사용가치";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "비사용가치";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "순편익(할인율:입력값)";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "순편익(할인율:2%)";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "순편익(할인율:4%)";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "순편익(할인율:6%)";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            dataGridView1.Rows.Add(row);

            row = new DataGridViewRow();

            cell = new DataGridViewTextBoxCell();
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "정화비용";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "기타비용";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "직접사용가치";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "간접사용가치";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "PV";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "NPV";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "PV";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "NPV";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "PV";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "NPV";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "PV";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = "NPV";
            cell.Style.BackColor = backColor;
            cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            row.Cells.Add(cell);

            dataGridView1.Rows.Add(row);

            dataGridView1.MergeCells(0, 0, 1, 0);
            dataGridView1.MergeCells(0, 1, 0, 2);
            dataGridView1.MergeCells(0, 3, 0, 4);
            dataGridView1.MergeCells(0, 5, 1, 5);
            dataGridView1.MergeCells(0, 6, 0, 7);
            dataGridView1.MergeCells(0, 8, 0, 9);
            dataGridView1.MergeCells(0, 10, 0, 11);
            dataGridView1.MergeCells(0, 12, 0, 13);
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
    }
}
