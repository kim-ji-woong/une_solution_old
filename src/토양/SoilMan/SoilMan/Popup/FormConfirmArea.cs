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
    public partial class FormConfirmArea : Form
    {
        private Dictionary<LandType, Overlay.AreaNCost> m_dicLandTypeArea = null;
        
        public Dictionary<LandType, Overlay.AreaNCost> LandTypeAreas
        {
            get { return m_dicLandTypeArea; }
        }

        public DataGridView GridArea
        {
            get { return gridArea; }
        }

        public DataGridView GridCost
        {
            get { return gridCost; }
        }

        private static double m_dAnualRecovery = 0.0;
        public static double AnualRecoveryCost
        {
            get { return m_dAnualRecovery; }
        }

        public FormConfirmArea()
        {
            InitializeComponent();
            InitGrid();
        }

        public void SetLandTypeInfo(Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea)
        {
            m_dicLandTypeArea = dicLandTypeArea;
            SetAreaDatas();
            SetCostDatas();

            gridArea.ClearSelection();
            gridCost.ClearSelection();
        }

        private void FormConfirmArea_Load(object sender, EventArgs e)
        {
        }

        private void InitGrid()
        {
            gridArea.EnableHeadersVisualStyles = false;
            gridArea.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridCost.EnableHeadersVisualStyles = false;
            gridCost.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            int nSpace1 = gridArea.Location.Y - (label1.Location.Y + label1.Size.Height);
            int nSpace2 = label2.Location.Y - (gridArea.Location.Y + gridArea.Size.Height);

            foreach (DataGridViewColumn column in gridArea.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.BackColor = Color.FromArgb(247, 150, 70);
                column.InheritedStyle.Font = new Font(column.InheritedStyle.Font, FontStyle.Bold);
            }

            gridArea.MergeColumns(1, 2);
            
            foreach (DataGridViewColumn column in gridCost.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.BackColor = Color.FromArgb(247, 150, 70);
                column.InheritedStyle.Font = new Font(column.InheritedStyle.Font, FontStyle.Bold);
            }

            int nColumnHeight = colArea.HeaderCell.Size.Height;
            int nRowHeight = gridArea.RowTemplate.Height * 5;
            gridArea.Size = new Size(gridArea.Size.Width, nColumnHeight + nRowHeight + 2);
            gridCost.Size = new Size(gridCost.Size.Width, nColumnHeight + nRowHeight + 2);

            label2.Location = new Point(label2.Location.X, gridArea.Location.Y + gridArea.Size.Height + nSpace2);
            gridCost.Location = new Point(gridCost.Location.X, label2.Location.Y + label2.Size.Height + nSpace1);
        }

        

        private void SetCostDatas()
        {
            gridCost.Rows.Clear();

            if (m_dicLandTypeArea == null)
                return;

            double dTotalCost = 0.0;

            foreach (KeyValuePair<LandType, Overlay.AreaNCost> pair in m_dicLandTypeArea)
            {
                dTotalCost += pair.Value.Cost;
            }

            Color col1 = Color.FromArgb(252, 213, 181);
            Color col2 = Color.White;
            Color col3 = Color.FromArgb(247, 150, 70);
            Color col4 = Color.FromArgb(238, 236, 225);

            DataGridViewRow row1 = AddRow2(LandType.General, dTotalCost, col1, col2);
            DataGridViewRow row2 = AddRow2(LandType.Field, dTotalCost, col1, col2);
            DataGridViewRow row3 = AddRow2(LandType.RiceField, dTotalCost, col1, col2);
            DataGridViewRow row4 = AddRow2(LandType.Mountain, dTotalCost, col1, col2);
            DataGridViewRow row5 = AddRow2("총지가[억원]", dTotalCost, dTotalCost, col3, col4, true);

            double dAnualRecovery = dTotalCost * FormMain.Instance.Get스티그마() / 100.0 / FormMain.Instance.Get회복기간();
            DataGridViewRow row6 = AddRow2("연간회복금액[억원]", dAnualRecovery, dAnualRecovery, col3, col4, true);
            m_dAnualRecovery = dAnualRecovery;
            int nColumnHeight = colCostType.HeaderCell.Size.Height;
            int nGridHeight = nColumnHeight + 2;

            if (row1 != null)
                nGridHeight += row1.Height;
            if (row2 != null)
                nGridHeight += row2.Height;
            if (row3 != null)
                nGridHeight += row3.Height;
            if (row4 != null)
                nGridHeight += row4.Height;
            if (row5 != null)
                nGridHeight += row5.Height;
            if (row5 != null)
                nGridHeight += row6.Height;

            gridCost.Size = new Size(gridCost.Size.Width, nGridHeight);
        }

        private DataGridViewRow AddRow2(LandType type, double dTotalCost, Color col1, Color col2)
        {
            Overlay.AreaNCost data;

            if (m_dicLandTypeArea.TryGetValue(type, out data))
            {
                if (type == LandType.General)
                    return AddRow2("일반토양", data.Cost, dTotalCost, col1, col2);
                else if (type == LandType.Field)
                    return AddRow2("밭토양", data.Cost, dTotalCost, col1, col2);
                else if (type == LandType.RiceField)
                    return AddRow2("논토양", data.Cost, dTotalCost, col1, col2);
                else if (type == LandType.Mountain)
                    return AddRow2("임야토양", data.Cost, dTotalCost, col1, col2);
            }

            return null;
        }

        private DataGridViewRow AddRow2(string strTypeName, double dCost, double dTotalCost, Color col1, Color col2, bool bold = false)
        {
            DataGridViewRow row = new DataGridViewRow();

            if (dTotalCost == 0.0)
            {
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strTypeName;
                cell.Style.BackColor = col1;

                if (bold)
                    cell.Style.Font = new System.Drawing.Font(gridArea.DefaultCellStyle.Font, FontStyle.Bold);

                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                cell.Style.BackColor = col2;
                row.Cells.Add(cell);
            }
            else
            {
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strTypeName;
                cell.Style.BackColor = col1;

                if (bold)
                    cell.Style.Font = new System.Drawing.Font(gridArea.DefaultCellStyle.Font, FontStyle.Bold);

                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = CostString(dCost);
                cell.Style.BackColor = col2;
                row.Cells.Add(cell);
            }

            gridCost.Rows.Add(row);
            return row;
        }

        // 원 단위의 금액을 억원 단위로 바꾼 문자열을 리턴한다.
        public static string CostString(double dCost)
        {
            dCost = dCost / 100000000;

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

            /*double underPoint = dCost - (int)dCost;
            string strUnderPoint = string.Format("{0:F2}", underPoint);
            string strCost = "";

            if (strUnderPoint == "0.00")
                strCost = string.Format("{0:###,###,###,###,###,###}", (int)dCost);
            else
                strCost = string.Format("{0:###,###,###,###,###,###}", (int)dCost) + strUnderPoint.Substring(1);*/

            if (dCost < 0 && !strCost.StartsWith("-"))
                return "-" + strCost;

            return strCost;
        }

        private void SetAreaDatas()
        {
            gridArea.Rows.Clear();

            if (m_dicLandTypeArea == null)
                return;

            double dTotalArea = 0.0;

            // 면적 단위 : m²
            foreach (KeyValuePair<LandType, Overlay.AreaNCost> pair in m_dicLandTypeArea)
            {
                dTotalArea += pair.Value.Area;
            }

            Color col1 = Color.FromArgb(252, 213, 181);
            Color col2 = Color.White;
            Color col3 = Color.FromArgb(247, 150, 70);
            Color col4 = Color.FromArgb(238, 236, 225);

            DataGridViewRow row1 = AddRow(LandType.General, dTotalArea, col1, col2);
            DataGridViewRow row2 = AddRow(LandType.Field, dTotalArea, col1, col2);
            DataGridViewRow row3 = AddRow(LandType.RiceField, dTotalArea, col1, col2);
            DataGridViewRow row4 = AddRow(LandType.Mountain, dTotalArea, col1, col2);
            DataGridViewRow row5 = AddRow("총면적", dTotalArea, dTotalArea, col3, col4, true);

            int nSpace1 = label2.Location.Y - (gridArea.Location.Y + gridArea.Size.Height);
            int nSpace2 = gridCost.Location.Y - (label2.Location.Y + label2.Size.Height);

            int nColumnHeight = colArea.HeaderCell.Size.Height;
            int nGridHeight = nColumnHeight + 2;

            if (row1 != null)
                nGridHeight += row1.Height;
            if (row2 != null)
                nGridHeight += row2.Height;
            if (row3 != null)
                nGridHeight += row3.Height;
            if (row4 != null)
                nGridHeight += row4.Height;
            if (row5 != null)
                nGridHeight += row5.Height;

            gridArea.Size = new Size(gridArea.Size.Width, nGridHeight);

            label2.Location = new Point(label2.Location.X, gridArea.Location.Y + gridArea.Size.Height + nSpace1);
            gridCost.Location = new Point(gridCost.Location.X, label2.Location.Y + label2.Size.Height + nSpace2);
        }

        private DataGridViewRow AddRow(LandType type, double dTotalArea, Color col1, Color col2)
        {
            Overlay.AreaNCost data;

            if (m_dicLandTypeArea.TryGetValue(type, out data))
            {
                if (type == LandType.General)
                    return AddRow("일반토양", data.Area, dTotalArea, col1, col2);
                else if (type == LandType.Field)
                    return AddRow("밭토양", data.Area, dTotalArea, col1, col2);
                else if (type == LandType.RiceField)
                    return AddRow("논토양", data.Area, dTotalArea, col1, col2);
                else if (type == LandType.Mountain)
                    return AddRow("임야토양", data.Area, dTotalArea, col1, col2);
            }

            return null;
        }

        private DataGridViewRow AddRow(string strTypeName, double dArea, double dTotalArea, Color col1, Color col2, bool bold = false)
        {
            DataGridViewRow row = new DataGridViewRow();

            if (dTotalArea == 0.0)
            {
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strTypeName;
                cell.Style.BackColor = col1;

                if (bold)
                    cell.Style.Font = new System.Drawing.Font(gridArea.DefaultCellStyle.Font, FontStyle.Bold);

                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                cell.Style.BackColor = col2;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = "";
                cell.Style.BackColor = col2;
                row.Cells.Add(cell);
            }
            else
            {
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strTypeName;
                cell.Style.BackColor = col1;

                if (bold)
                    cell.Style.Font = new System.Drawing.Font(gridArea.DefaultCellStyle.Font, FontStyle.Bold);

                row.Cells.Add(cell);

                // m²를 ha로 바꾸므로 10000을 나누어준다.
                double area = dArea / 10000;

                string strValue = string.Format("{0:F2}", area);
                int nIndex = strValue.LastIndexOf('.');

                char chBelowFirst = strValue.ElementAt(nIndex + 1);
                char chBelowSecond = strValue.ElementAt(nIndex + 2);

                string strInt = strValue.Substring(0, nIndex);
                int nData = int.Parse(strInt);
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

                cell = new DataGridViewTextBoxCell();
                cell.Value = strValue;
                /*double underPoint = area - (int)area;
                string strUnderPoint = string.Format("{0:F2}", underPoint);

                cell = new DataGridViewTextBoxCell();

                if (strUnderPoint == "0.00")
                    cell.Value = string.Format("{0:###,###,###,###,###,###}", (int)area);
                else
                    cell.Value = string.Format("{0:###,###,###,###,###,###}", (int)area) + strUnderPoint.Substring(1);*/

                cell.Style.BackColor = col2;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                double dRatio = dArea * 100 / dTotalArea;
                cell.Value = string.Format("{0:F1}%", dRatio);
                cell.Style.BackColor = col2;
                row.Cells.Add(cell);
            }

            gridArea.Rows.Add(row);
            return row;
        }
    }
}
