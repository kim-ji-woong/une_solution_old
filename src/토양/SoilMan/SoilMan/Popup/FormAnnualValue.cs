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
    public partial class FormAnnualValue : Form
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

        public FormAnnualValue()
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

        public void Show(DataGridView gridCapacity, TechType techType, Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea, double dInheritanceValue, double dExistanceValue, double dBioValue)
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

                double dGeneral = FormMain.Instance.Get화폐화지표FunctionData(type);
                double dField = FormMain.Instance.Get화폐화지표FunctionData(type);
                double dRiceField = FormMain.Instance.Get화폐화지표FunctionData(type);
                double dMountain = FormMain.Instance.Get화폐화지표FunctionData(type);

                double dGeneralCapacity = GetCapacity(gridCapacity, i, GeneralIndex);
                double dFieldCapacity = GetCapacity(gridCapacity, i, FieldIndex);
                double dRiceFieldCapacity = GetCapacity(gridCapacity, i, RiceFieldIndex);
                double dMountainCapacity = GetCapacity(gridCapacity, i, MountainIndex);

                double dTechData = 1.0 / 100000000;
                //double dTechData = FormMain.Instance.Get기능회복율FunctionData(type, techType) / 100000000;

                double period = FormMain.Instance.Get기능회복기간FunctionData(type, techType);

                if (period > 0.0)
                {
                    double recovery = FormMain.Instance.Get기능회복율FunctionData(type, techType);
                    dTechData *= recovery;
                }
                
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
                cell.Value = GetValueString(dGeneral, dGeneralCapacity * dTechData, ref dTotalFactor, ref dGeneralTotal);
                cell.Tag = dGeneral * dGeneralCapacity * dTechData;
                row.Cells.Add(cell);
                
                cell = new DataGridViewTextBoxCell();
                cell.Value = GetValueString(dField, dFieldCapacity * dTechData, ref dTotalFactor, ref dFieldTotal);
                cell.Tag = dField * dFieldCapacity * dTechData;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = GetValueString(dRiceField, dRiceFieldCapacity * dTechData, ref dTotalFactor, ref dRiceFieldTotal);
                cell.Tag = dRiceField * dRiceFieldCapacity * dTechData;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = GetValueString(dMountain, dMountainCapacity * dTechData, ref dTotalFactor, ref dMountainTotal);
                cell.Tag = dMountain * dMountainCapacity * dTechData;
                row.Cells.Add(cell);

                double dTotal = (dGeneral * dGeneralCapacity + dField * dFieldCapacity + dRiceField * dRiceFieldCapacity + dMountain * dMountainCapacity) * dTechData;

                cell = new DataGridViewTextBoxCell();

                
                double dAnualRecovery = FormConfirmArea.AnualRecoveryCost;

                if (i == (int)SoilFunctionType.식물생산기능)
                {
                    dAmount += (dAnualRecovery/ 100000000);
                    //GetValueString(dAnualRecovery, dTotalFactor, ref dTotalFactor, ref dAmount);
                    cell.Value = CostString(dAnualRecovery);
                }

                else if (i <= (int)SoilFunctionType.수질정화)
                    cell.Value = GetValueString(dTotal, dTotalFactor, ref dTotalFactor, ref dAmount);
                else
                {                    
                    if (i == (int)SoilFunctionType.구조물지지기능 || i == (int)SoilFunctionType.원료공급기능)
                        dTotal = 0.0;
                    else if (i == (int)SoilFunctionType.유산가치)
                        dTotal = dInheritanceValue / 100000000;
                    else if (i == (int)SoilFunctionType.존재가치)
                        dTotal = dExistanceValue / 100000000;
                    else if (i == (int)SoilFunctionType.생태학적가치)
                        dTotal = dBioValue / 100000000;
                    else
                        continue;

                    if (dTotalFactor == 0.0)
                    {
                        string strTotal = string.Format("{0:F2}", dTotal);

                        if (strTotal != "0.00")
                            dTotalFactor = 1.0;
                    }

                    cell.Value = GetValueString(dTotal, dTotalFactor, ref dTotalFactor, ref dAmount);
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
            cell2.Value = GetValueString(dGeneralTotal, 1.0, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dFieldTotal, 1.0, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dRiceFieldTotal, 1.0, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dMountainTotal, 1.0, ref dTemp, ref dTemp);
            cell2.Style.BackColor = col2;
            row2.Cells.Add(cell2);

            cell2 = new DataGridViewTextBoxCell();
            cell2.Value = GetValueString(dAmount, 1.0, ref dTemp, ref dTemp);
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
