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
    public partial class FormInputCondition : Form
    {

        private Dictionary<LandType, Overlay.AreaNCost> m_dicLandTypeArea = null;

        public Dictionary<LandType, Overlay.AreaNCost> LandTypeAreas
        {
            get { return m_dicLandTypeArea; }
        }

        private TechType m_selectedTechType = TechType.Bio;
        private Dictionary<TechType, SoilCleanCost> m_dicTechTypeData = new Dictionary<TechType, SoilCleanCost>();
        
        private ComboBox m_comboTechType = new ComboBox();
        
        private ComboBox m_comboRegion = new ComboBox();
        
        private ComboBox m_comboWTP = new ComboBox();


        // 사용자 입력이 아닌 시스템에 의하여 Cell Value가 변경되었는가?
        private bool m_cellValueSystemChanged = false;

        // 가치 : 원
        //        0보다 작은 값이면 아직 입력되지 않은 상태이다.
        private double m_dInheritance = -1.0;
        private double m_dExistance = -1.0;
        private double m_dBio = -1.0;

        public TechType SelectedTechType
        {
            get { return m_selectedTechType; }
            set { m_selectedTechType = value; }
        }

        // 유산가치 : 원
        public double InheritanceValue
        {
            get { return m_dInheritance < 0.0 ? 0.0 : m_dInheritance; }
            set { m_dInheritance = value; }
        }

        // 존재가치 : 원
        public double ExistanceValue
        {
            get { return m_dExistance < 0.0 ? 0.0 : m_dExistance; }
            set { m_dExistance = value; }
        }

        // 생태학적가치 : 원
        public double BioValue
        {
            get { return m_dBio < 0.0 ? 0.0 : m_dBio; }
            set { m_dBio = value; }
        }

        public DataGridView GridCondition
        {
            get { return gridCondition; }
        }

        public DataGridView GridCost
        {
            get { return gridCost; }
        }


        private int m_nSelectRegion = 0;
        public int SelectedRegion
        {
            get { return m_nSelectRegion; }
            set { m_nSelectRegion = value; }
        }

        private WTPType mSelectedWTP = WTPType.대수선형로싯트_WTP;
        public WTPType SelectedWTPType
        {
            get { return mSelectedWTP; }
            set { mSelectedWTP = value; }
        }
        
        public FormInputCondition()
        {
            InitializeComponent();
            InitTechComboBox();

            InitRegionComboBox();
            InitWTPComboBox();

            InitGrid();
        }

        public void SetLandTypeInfo(Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea)
        {
            m_dicLandTypeArea = dicLandTypeArea;

            SetCondition();
            SetCost();
        }


        private static string[] mRegions = { "전국", "서울", "경기/인천", "부산/울산/경남", "대구/경북", "대전/충청", "광주/전라", "강원" };

        public static int RegionIndex(string szRegionName)
        {
            for(int i = 0; i < mRegions.Length; i++)
            {
                if( mRegions[i] == szRegionName)
                    return i;
            }
            return -1;
        }

        public static string RegionString(int nIdx)
        {
            if( nIdx < 0 || nIdx >= mRegions.Length)
                return "";

            return mRegions[nIdx];
        }

        public static string WTPTypeString(WTPType type)
        {
            if( type == WTPType.대수선형로짓트_중앙)
                return "대수선형로짓트-중앙치";
            else if( type == WTPType.대수선형로싯트_WTP)
                return "대수선형로짓트-WTP(절단)";            
            else if( type == WTPType.Weibull_중앙)
                return "Weibull-중앙치";
            else if( type == WTPType.Weibull_WTP)
                return "Weibull-WTP(절단)";

            return "";
        }

        public static WTPType ToWTPType(string szWTPName)
        {
            if( szWTPName == "대수선형로짓트-중앙치")
                return WTPType.대수선형로짓트_중앙;
            else if( szWTPName == "대수선형로짓트-WTP(절단)")
                return WTPType.대수선형로싯트_WTP;            
            else if( szWTPName == "Weibull-중앙치")
                return WTPType.Weibull_중앙;
            else if( szWTPName == "Weibull-WTP(절단)")
                return WTPType.Weibull_WTP;

            return WTPType.None;
        }

        public static string TechTypeToString(TechType type)
        {
            if (type == TechType.Bio)
                return "생물통풍";
            else if (type == TechType.Farm)
                return "토양경작";
            else if (type == TechType.Steam)
                return "증기추출";
            else if (type == TechType.Washing)
                return "토양세척";
            else if (type == TechType.Oxidation)
                return "화학산화";
            else if (type == TechType.Heat)
                return "열탈착";

            return "";
        }

        public static TechType ToTechType(string strTechType)
        {
            if (strTechType == "생물통풍")
                return TechType.Bio;
            else if (strTechType == "토양경작")
                return TechType.Farm;
            else if (strTechType == "증기추출")
                return TechType.Steam;
            else if (strTechType == "토양세척")
                return TechType.Washing;
            else if (strTechType == "화학산화")
                return TechType.Oxidation;
            else if (strTechType == "열탈착")
                return TechType.Heat;

            return TechType.None;
        }

        private void InitGrid()
        {
            // 분석조건Grid 초기화
            gridCondition.EnableHeadersVisualStyles = false;
            gridCondition.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            
            int nRowHeight = gridCondition.RowTemplate.Height * 4;
            gridCondition.Size = new Size(gridCondition.Size.Width, gridCondition.RowTemplate.Height * 5 + 3);
            
            SetCondition();
       
            // 비사용가치Grid 초기화
            gridCost.EnableHeadersVisualStyles = false;
            gridCost.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            foreach (DataGridViewColumn column in gridCost.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.BackColor = Color.FromArgb(247, 150, 70);
                column.InheritedStyle.Font = new Font(column.InheritedStyle.Font, FontStyle.Bold);
            }

            gridCost.MergeColumns(1, 2);

            int nColumnHeight = colCostType.HeaderCell.Size.Height;            
            gridCost.Size = new Size(gridCost.Size.Width, nColumnHeight + gridCondition.RowTemplate.Height * 10 + 2);
            SetCost();
        }


        private double m_dInputWTPYear = 0.0;
        public double InputWTPYear
        {
            get { return m_dInputWTPYear; }
            set { m_dInputWTPYear = value; }
        }

        private double m_dInputRejectionRatio = 0.0;
        public double InputRejectionRatio
        {
            get { return m_dInputRejectionRatio; }
            set { m_dInputRejectionRatio = value; }
        }

        private long m_dInputHousehold = 0;
        public long InputHousehold
        {
            get { return m_dInputHousehold; }
            set { m_dInputHousehold = value; }
        }

        private void SetCost()
        {
            //////////////////////////////////////////////////////////////////////////////
            // Cost 값은 시스템 상수를 이용하므로 초기 로딩시 데이터가 없을 수 있으므로 반드시 체크한다.
            if (FormMain.Instance == null)
                return;
            DataGridView grid = FormMain.Instance.Get지불의사액Grid();
            if (grid == null)
                return;
            ///////////////////////////////////////////////////////////////////////////////
            if (m_nSelectRegion < 0)
                return;

            if (m_dicLandTypeArea == null)
                return;

            double dTotalArea = 0.0;

            // 면적 단위 : m²
            foreach (KeyValuePair<LandType, Overlay.AreaNCost> pair in m_dicLandTypeArea)
            {
                dTotalArea += pair.Value.Area;
            }
            dTotalArea *= 0.0001;

            string szRegionName = RegionString(m_nSelectRegion);
            string szWTPTypeName = WTPTypeString(mSelectedWTP);

            string szWTPMonth = grid.Rows[m_nSelectRegion].Cells[(int)mSelectedWTP+2].Value.ToString();
            double dMonthWTP = 0.0;
            double.TryParse(szWTPMonth, out dMonthWTP);

            string szWTPYear = string.Format("{0:###,###,###,###,###,###}", dMonthWTP * 12.0);
            string szRejectionRatio = grid.Rows[m_nSelectRegion].Cells[6].Value.ToString();

            DataGridView grid2 = FormMain.Instance.Get지역별가구수면적Grid();
            if (grid2 == null)
                return;

            double nHousehold = (double)grid2.Rows[m_nSelectRegion].Cells[2].Tag;
            double nArea = (double)grid2.Rows[m_nSelectRegion].Cells[4].Tag;
            double dCalHousehold = nHousehold / nArea * dTotalArea * 0.01;
            string szHousehold = string.Format("{0:###,###,###,###,###,###}", dCalHousehold);

            string szInputWTPYear = "", szInputRejectionRatio = "", szInputHousehold = "";
            if (m_dInputWTPYear >= 0.0)
                szInputWTPYear = string.Format("{0:###,###,###,###,###,###}", m_dInputWTPYear);

            if (m_dInputRejectionRatio >= 0.0)
                szInputRejectionRatio = string.Format("{0:F2}",m_dInputRejectionRatio);

            if (m_dInputHousehold >= 0)
                szInputHousehold = string.Format("{0:###,###,###,###,###,###}", m_dInputHousehold);


            double dCalcWTPYear = m_dInputWTPYear * m_dInputHousehold * (100 - m_dInputRejectionRatio) / 100;
            if( dCalcWTPYear > 0.0)
            {
                double dHaritageValue = FormMain.Instance.Get유산가치();
                m_dInheritance = dCalcWTPYear * dHaritageValue;
                double dExistValue = FormMain.Instance.Get존재가치();
                m_dExistance = dCalcWTPYear * dExistValue;
                double dBio = FormMain.Instance.Get선택가치();
                m_dBio = dCalcWTPYear * dBio;
            }
            else
            {
                m_dInheritance = 0.0;
                m_dExistance = 0.0;
                m_dBio = 0.0;
            }
            string szCalcWTPYear = InheritanceString(dCalcWTPYear);

            string strInheritance = "", strExistance = "", strBio = "";

            if (m_dInheritance >= 0.0)
                strInheritance = InheritanceString(m_dInheritance);

            if (m_dExistance >= 0.0)
                strExistance = ExistanceString(m_dExistance);

            if (m_dBio >= 0.0)
                strBio = BioString(m_dBio);

            //gridCost.Rows.Clear();

            Color col1 = Color.FromArgb(252, 213, 181);
            Color col2 = Color.White;
            Color col3 = Color.WhiteSmoke;

            int nRowCount = gridCost.Rows.Count;

            DataGridViewRow row1 = nRowCount < 10 ? AddMergeCellRow(gridCost, "지역구분", szRegionName, col1, col2) : SetMergeCellRow(gridCost, 0, "지역구분", szRegionName);
            DataGridViewRow row2 = nRowCount < 10 ? AddMergeCellRow(gridCost, "WTP(월) 적용", szWTPTypeName, col1, col2) : SetMergeCellRow(gridCost, 1, "WTP(월) 적용", szWTPTypeName);
            DataGridViewRow row3 = nRowCount < 10 ? AddRow3(gridCost, "연간WTP[원/가구/년]", szWTPYear, szInputWTPYear, col1, col3, col2) : SetRow3(gridCost, 2, "연간WTP[원/가구/년]", szWTPYear, szInputWTPYear);
            DataGridViewRow row4 = nRowCount < 10 ? AddRow3(gridCost, "지불거부율[%]", szRejectionRatio, szInputRejectionRatio, col1, col3, col2) : SetRow3(gridCost, 3, "지불거부율[%]", szRejectionRatio, szInputRejectionRatio);
            DataGridViewRow row5 = nRowCount < 10 ? AddRow3(gridCost, "가구수(개략치)", szHousehold, szInputHousehold, col1, col3, col2) : SetRow3(gridCost, 4, "가구수(개략치)", szHousehold, szInputHousehold);
            DataGridViewRow row6 = nRowCount < 10 ? AddMergeCellRow(gridCost, "연간WTP[억원/년]", szCalcWTPYear, col1, col3) : SetMergeCellRow(gridCost, 5, "연간WTP[억원/년]", szCalcWTPYear);
            row6.ReadOnly = true;
            DataGridViewRow row7 = nRowCount < 10 ? AddMergeCellRow(gridCost, "유산가치", strInheritance, col1, col3) : SetMergeCellRow(gridCost, 6, "유산가치", strInheritance);
            row7.ReadOnly = true;
            DataGridViewRow row8 = nRowCount < 10 ? AddMergeCellRow(gridCost, "존재가치", strExistance, col1, col3) : SetMergeCellRow(gridCost, 7, "존재가치", strExistance);
            row8.ReadOnly = true;
            DataGridViewRow row9 = nRowCount < 10 ? AddMergeCellRow(gridCost, "생태학적가치", strBio, col1, col3) : SetMergeCellRow(gridCost, 8, "생태학적가치", strBio);
             row9.ReadOnly = true;
             DataGridViewRow row10 = nRowCount < 10 ? AddMergeCellRow(gridCost, "비사용가치[억원/년]", szCalcWTPYear, col1, col3) : SetMergeCellRow(gridCost, 9, "비사용가치[억원/년]", szCalcWTPYear);
            row10.ReadOnly = true;
            List<DataGridViewRow> rows = new List<DataGridViewRow>();
            rows.Add(row1);
            rows.Add(row2);
            rows.Add(row3);
            rows.Add(row4);
            rows.Add(row5);
            rows.Add(row6);
            rows.Add(row7);
            rows.Add(row8);
            rows.Add(row9);
            rows.Add(row10);


            if (row7 != null && m_dInheritance >= 0.0)
                row7.Cells[1].Tag = m_dInheritance;

            if (row8 != null && m_dExistance >= 0.0)
                row8.Cells[1].Tag = m_dExistance;

            if (row9 != null && m_dBio >= 0.0)
                row9.Cells[1].Tag = m_dBio;

            m_cellValueSystemChanged = false;

            int nColumnHeight = colCostType.HeaderCell.Size.Height;
            int nGridHeight = nColumnHeight + 2;

            foreach(DataGridViewRow row in rows)
            {
                if( row != null)
                {
                    nGridHeight += row.Height;
                }
            } 
            gridCost.Size = new Size(gridCost.Size.Width, nGridHeight);

            gridCost.Refresh();
        }

        private string InheritanceString(double dInheritance)
        {
            return FormConfirmArea.CostString(dInheritance);
        }

        private string ExistanceString(double dExistance)
        {
            return FormConfirmArea.CostString(dExistance);
        }

        private string BioString(double dBio)
        {   
            return FormConfirmArea.CostString(dBio);
        }

        private void SetCondition()
        {

            //////////////////////////////////////////////////////////////////////////////
            // Cost 값은 시스템 상수를 이용하므로 초기 로딩시 데이터가 없을 수 있으므로 반드시 체크한다.
            if (FormMain.Instance == null)
                return;            
           
            if (m_dicLandTypeArea == null)
                return;

            double dTotalArea = 0.0;

            // 면적 단위 : m²
            foreach (KeyValuePair<LandType, Overlay.AreaNCost> pair in m_dicLandTypeArea)
            {
                dTotalArea += pair.Value.Area;
            }
            dTotalArea *= 0.0001;

            SoilCleanCost value = null;
            m_dicTechTypeData.TryGetValue(m_selectedTechType, out value);
            string strCost = "", strPeriod = "", strDiscount = "", strExtraCost = "";

            if (value != null)
            {
                strCost = value.Cost >= 0.0 ? FormConfirmArea.CostString(value.Cost) : "";
                strPeriod = value.Period > 0 ? PeriodString(value.Period) : "";
                strDiscount = value.Discount >= 0.0 ? DiscountString(value.Discount) : "";
                strExtraCost = value.ExtraCost >= 0.0 ? FormConfirmArea.CostString(value.ExtraCost) : "";
            }

            double dUnitCost = FormMain.Instance.Get토양정화기술Cost(m_selectedTechType);
            gridCondition.Rows.Clear();

            double dTotalCost = dUnitCost * 100000000 * dTotalArea;
            string szSystemCost = dTotalCost >= 0.0 ? FormConfirmArea.CostString(dTotalCost) : "";

            Color col1 = Color.FromArgb(252, 213, 181);
            Color col2 = Color.White;
            Color col3 = Color.FromArgb(247, 150, 70);

            int nRowCount = gridCondition.Rows.Count;

            DataGridViewRow row0 = nRowCount < 6 ? AddRow(gridCondition, "토양 정화기술 선택", TechTypeToString(m_selectedTechType), col3, col2, true) : SetRow(gridCondition, 0, "토양 정화기술 선택", TechTypeToString(m_selectedTechType));
            DataGridViewRow row5 = nRowCount < 6 ? AddRow(gridCondition, "토양정화비용(억원)-시스템", szSystemCost, col1, Color.WhiteSmoke) : SetRow(gridCondition, 1, "토양정화비용(억원)-시스템", szSystemCost);
            row5.ReadOnly = true;
            DataGridViewRow row1 = nRowCount < 6 ? AddRow(gridCondition, "토양 정화비용(억원)", strCost, col1, col2) : SetRow(gridCondition, 2, "토양 정화비용(억원)", strCost);
            DataGridViewRow row2 = nRowCount < 6 ? AddRow(gridCondition, "기타비용(억원)", strExtraCost, col1, col2) : SetRow(gridCondition, 3, "기타비용(억원)", strExtraCost);
            DataGridViewRow row3 = nRowCount < 6 ? AddRow(gridCondition, "분석기간(년)", strPeriod, col1, col2) : SetRow(gridCondition, 4, "분석기간(년)", strPeriod);
            DataGridViewRow row4 = nRowCount < 6 ? AddRow(gridCondition, "할인율(%)", strDiscount, col1, col2) : SetRow(gridCondition, 5, "할인율(%)", strDiscount);

            if (row1 != null && value != null && value.Cost >= 0.0)
                row1.Cells[1].Tag = value.Cost;

            if (row2 != null && value != null && value.Period > 0)
                row2.Cells[1].Tag = (double)value.Period;

            if (row4 != null && value != null && value.Discount >= 0.0)
                row4.Cells[1].Tag = value.Discount;

            gridCondition.Rows[0].Cells[1].ReadOnly = true;
            m_cellValueSystemChanged = false;

            int nSpace1 = 10;// label2.Location.Y - (gridCondition.Location.Y + gridCondition.Size.Height);
            int nSpace2 = 3;// gridCost.Location.Y - (label2.Location.Y + label2.Size.Height);

            int nGridHeight = 3;

            if (row0 != null)
                nGridHeight += row0.Height;
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

            gridCondition.Size = new Size(gridCondition.Size.Width, nGridHeight);

            label2.Location = new Point(label2.Location.X, gridCondition.Location.Y + gridCondition.Size.Height + nSpace1);
            gridCost.Location = new Point(gridCost.Location.X, label2.Location.Y + label2.Size.Height + nSpace2);
        }

        private DataGridViewRow SetRow(DataGridView grid, int nRowIndex, string strItem1, string strItem2)
        {
            if (nRowIndex >= grid.Rows.Count)
                return null;

            DataGridViewRow row = grid.Rows[nRowIndex];

            row.Cells[0].Value = strItem1;
            row.Cells[1].Value = strItem2;

            return row;
        }

        private DataGridViewRow SetMergeCellRow(DataGridView grid, int nRowIndex, string strItem1, string strItem2, bool bFirst = true)
        {
            if (nRowIndex >= grid.Rows.Count)
                return null;

            DataGridViewRow row = grid.Rows[nRowIndex];

            bool bReadOnly = false;
            if( row.ReadOnly == true)
            {
                bReadOnly = true;
                row.ReadOnly = false;
            }
            row.Cells[0].Value = strItem1;
            if (bFirst == true)
                row.Cells[1].Value = strItem2;
            else
                row.Cells[2].Value = strItem2;


            row.ReadOnly = bReadOnly;
            return row;
        }

        private DataGridViewRow SetRow3(DataGridView grid, int nRowIndex, string strItem1, string strItem2, string strItem3)
        {
            if (nRowIndex >= grid.Rows.Count)
                return null;

            DataGridViewRow row = grid.Rows[nRowIndex];

            row.Cells[0].Value = strItem1;
            row.Cells[1].Value = strItem2;
            row.Cells[2].Value = strItem3;
            return row;
        }

        private DataGridViewRow AddMergeCellRow(DataGridView grid, string strItem1, string strItem2, Color col1, Color col2,  bool bold = false)
        {
            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            m_cellValueSystemChanged = true;
            cell.Value = strItem1;
            cell.Style.BackColor = col1;

            if (bold)
                cell.Style.Font = new System.Drawing.Font(grid.DefaultCellStyle.Font, FontStyle.Bold);

            row.Cells.Add(cell);
            cell.ReadOnly = true;

            cell = new DataGridViewTextBoxCell();

            if (bold)
                cell.Style.Font = new System.Drawing.Font(grid.DefaultCellStyle.Font, FontStyle.Bold);

            m_cellValueSystemChanged = true;
            cell.Value = strItem2;
            cell.Style.BackColor = col2;
            row.Cells.Add(cell);
            //cell.ReadOnly = true;
            cell = new DataGridViewTextBoxCell();

            if (bold)
                cell.Style.Font = new System.Drawing.Font(grid.DefaultCellStyle.Font, FontStyle.Bold);

            m_cellValueSystemChanged = true;
            cell.Style.BackColor = col2;
            row.Cells.Add(cell);
           
            UnE.Controls.MergedDataGridView tgrid = (UnE.Controls.MergedDataGridView)grid;

            grid.Rows.Add(row);
            UnE.Controls.MergedDataGridView.MergedCells cells = tgrid.MergeCells(row.Index, 1, row.Index, 2);

            return row;
        }

        private DataGridViewRow AddRow3(DataGridView grid, string strItem1, string strItem2, string strItem3, Color col1, Color col2, Color col3, bool bold = false)
        {
            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            m_cellValueSystemChanged = true;
            cell.Value = strItem1;
            cell.Style.BackColor = col1;

            if (bold)
                cell.Style.Font = new System.Drawing.Font(grid.DefaultCellStyle.Font, FontStyle.Bold);

            row.Cells.Add(cell);
            cell.ReadOnly = true;

            cell = new DataGridViewTextBoxCell();

            if (bold)
                cell.Style.Font = new System.Drawing.Font(grid.DefaultCellStyle.Font, FontStyle.Bold);

            m_cellValueSystemChanged = true;
            cell.Value = strItem2;
            cell.Style.BackColor = col2;
            row.Cells.Add(cell);
            //cell.ReadOnly = true;
            cell = new DataGridViewTextBoxCell();

            if (bold)
                cell.Style.Font = new System.Drawing.Font(grid.DefaultCellStyle.Font, FontStyle.Bold);

            m_cellValueSystemChanged = true;
            cell.Value = strItem3;
            cell.Style.BackColor = col3;
            row.Cells.Add(cell);

            grid.Rows.Add(row);
            return row;
        }

        private DataGridViewRow AddRow(DataGridView grid, string strItem1, string strItem2, Color col1, Color col2, bool bold = false)
        {
            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            m_cellValueSystemChanged = true;
            cell.Value = strItem1;
            cell.Style.BackColor = col1;

            if (bold)
                cell.Style.Font = new System.Drawing.Font(grid.DefaultCellStyle.Font, FontStyle.Bold);

            row.Cells.Add(cell);
            cell.ReadOnly = true;

            cell = new DataGridViewTextBoxCell();

            if (bold)
                cell.Style.Font = new System.Drawing.Font(grid.DefaultCellStyle.Font, FontStyle.Bold);

            m_cellValueSystemChanged = true;
            cell.Value = strItem2;
            cell.Style.BackColor = col2;
            row.Cells.Add(cell);

            grid.Rows.Add(row);
            return row;
        }
      
        private void InitRegionComboBox()
        {
            gridCost.Controls.Add(m_comboRegion);
            m_comboRegion.Visible = false;

            for (int i = 0; i < mRegions.Length; i++)
            {
                m_comboRegion.Items.Add(mRegions[i]);
            }

            m_comboRegion.DropDownStyle = ComboBoxStyle.DropDownList;
            m_comboRegion.SelectedIndexChanged += new EventHandler(this.OnRegionSelectedIndexChanged);
            m_comboRegion.DropDownClosed += new System.EventHandler(this.OnDropDownClosedRegionComboBox);
            m_comboRegion.Leave += new EventHandler(this.OnLeaveRegionComboBox);
            m_comboRegion.MouseLeave += new System.EventHandler(OnMouseLeaveRegionComboBox);
        }

        private void InitWTPComboBox()
        {
            gridCost.Controls.Add(m_comboWTP);
            m_comboWTP.Visible = false;

            for (int i = (int)WTPType.대수선형로짓트_중앙; i < (int)WTPType.None; i++)
            {
                m_comboWTP.Items.Add(WTPTypeString((WTPType)i));
            }

            m_comboWTP.DropDownStyle = ComboBoxStyle.DropDownList;
            m_comboWTP.SelectedIndexChanged += new EventHandler(this.OnWTPSelectedIndexChanged);
            m_comboWTP.DropDownClosed += new System.EventHandler(this.OnDropDownClosedWTPComboBox);
            m_comboWTP.Leave += new EventHandler(this.OnLeaveWTPComboBox);
            m_comboWTP.MouseLeave += new System.EventHandler(OnMouseLeaveWTPComboBox);
        }


        private void InitTechComboBox()
        {
            gridCondition.Controls.Add(m_comboTechType);
            m_comboTechType.Visible = false;

            for (int i = (int)TechType.Bio; i < (int)TechType.Count; i++)
            {
                TechType type = (TechType)i;
                m_comboTechType.Items.Add(TechTypeToString(type));
            }

            m_comboTechType.DropDownStyle = ComboBoxStyle.DropDownList;
            m_comboTechType.SelectedIndexChanged += new EventHandler(this.OnTechTypeSelectedIndexChanged);
            m_comboTechType.DropDownClosed += new System.EventHandler(this.OnDropDownClosedTechComboBox);
            m_comboTechType.Leave += new EventHandler(this.OnLeaveTechComboBox);
            m_comboTechType.MouseLeave += new System.EventHandler(OnMouseLeaveTechComboBox);
        }

        private void FormInputCondition_Load(object sender, EventArgs e)
        {
        }

        private void HideTechComboBox()
        {
            m_selectedTechType = ToTechType(m_comboTechType.Text);
            gridCondition.Rows[0].Cells[1].Value = m_comboTechType.Text;
            m_comboTechType.Hide();
        }

        private void HideRegionComboBox()
        {
            m_nSelectRegion = RegionIndex(m_comboRegion.Text);
            gridCost.Rows[0].Cells[1].Value = m_comboRegion.Text;
            m_comboRegion.Hide();
        }

        private void HideWTPComboBox()
        {
            mSelectedWTP = ToWTPType(m_comboWTP.Text);
            gridCost.Rows[1].Cells[1].Value = m_comboWTP.Text;
            m_comboWTP.Hide();
        }

        private void OnRegionSelectedIndexChanged(object sender, EventArgs e)
        {
            HideRegionComboBox();
            SetCost();
        }

        private void OnWTPSelectedIndexChanged(object sender, EventArgs e)
        {
            HideWTPComboBox();
            SetCost();
        }

        private void OnTechTypeSelectedIndexChanged(object sender, EventArgs e)
        {
            HideTechComboBox();
            SetCondition();
        }

        private void gridCost_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if( (e.RowIndex == 0 && e.ColumnIndex == 1) || (e.RowIndex == 0 && e.ColumnIndex == 2))
            {
                DataGridViewRow row = gridCost.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex];

                Rectangle rect = gridCost.GetCellDisplayRectangle(1, e.RowIndex, false);
                Rectangle rect2 = gridCost.GetCellDisplayRectangle(2, e.RowIndex, false);
                m_comboRegion.Location = new Point(rect.Left, rect.Top);
                m_comboRegion.Size = new Size(rect.Width + rect2.Width, rect.Height + rect2.Height);
                SetComboBoxIndex(cell.Value == null ? "" : cell.Value.ToString(), m_comboRegion);
                m_comboRegion.Show();
            }
            else if ((e.RowIndex == 1 && e.ColumnIndex == 1) || (e.RowIndex == 1 && e.ColumnIndex == 2))
            {
                DataGridViewRow row = gridCost.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex];

                Rectangle rect = gridCost.GetCellDisplayRectangle(1, e.RowIndex, false);
                Rectangle rect2 = gridCost.GetCellDisplayRectangle(2, e.RowIndex, false);
               

                m_comboWTP.Location = new Point(rect.Left, rect.Top);
                m_comboWTP.Size = new Size(rect.Width + rect2.Width, rect.Height+ rect2.Height);
                SetComboBoxIndex(cell.Value == null ? "" : cell.Value.ToString(), m_comboWTP);
                m_comboWTP.Show();
            }
           
        }

        private void gridCondition_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == 0 && e.ColumnIndex == 1)
            {
                DataGridViewRow row = gridCondition.Rows[e.RowIndex];
                DataGridViewCell cell = row.Cells[e.ColumnIndex];

                Rectangle rect = gridCondition.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                m_comboTechType.Location = new Point(rect.Left, rect.Top);
                m_comboTechType.Size = new Size(rect.Width, rect.Height);
                SetComboBoxIndex(cell.Value == null ? "" : cell.Value.ToString(), m_comboTechType);
                m_comboTechType.Show();
            }
        }

        

        private void SetComboBoxIndex(string strText, ComboBox combobox)
        {
            int nItemCount = combobox.Items.Count;

            for (int i = 0; i < nItemCount; i++)
            {
                string strItem = (string)combobox.Items[i];

                if (strItem == strText)
                {
                    combobox.SelectedIndex = i;
                    break;
                }
            }
        }
        

        private void OnDropDownClosedRegionComboBox(object sender, EventArgs e)
        {
            HideRegionComboBox();
        }

        private void OnDropDownClosedWTPComboBox(object sender, EventArgs e)
        {
            HideWTPComboBox();
        }

        private void OnDropDownClosedTechComboBox(object sender, EventArgs e)
        {
            HideTechComboBox();
        }

        private void OnLeaveWTPComboBox(object sender, EventArgs e)
        {
            HideWTPComboBox();
        }

        private void OnLeaveRegionComboBox(object sender, EventArgs e)
        {
            HideRegionComboBox();
        }

        private void OnLeaveTechComboBox(object sender, EventArgs e)
        {
            HideTechComboBox();
        }

        private void OnMouseLeaveRegionComboBox(object sender, EventArgs e)
        {
            HideRegionComboBox();
        }

        private void OnMouseLeaveWTPComboBox(object sender, EventArgs e)
        {
            HideWTPComboBox();
        }

        private void OnMouseLeaveTechComboBox(object sender, EventArgs e)
        {
            HideTechComboBox();
        }

        private void gridCondition_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_cellValueSystemChanged)
            {
                m_cellValueSystemChanged = false;
                return;
            }

            if (e.ColumnIndex != 1 || e.RowIndex < 0)
                return;

            DataGridViewCell cell = gridCondition.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (e.RowIndex == 2)
                ProcessSoilCleanCost(cell);
            else if (e.RowIndex == 3)
                ProcessExtraCost(cell);
            else if (e.RowIndex == 4)
                ProcessPeriod(cell);
            else if (e.RowIndex == 5)
                ProcessDiscount(cell);

            m_cellValueSystemChanged = false;
        }

        private void ProcessExtraCost(DataGridViewCell cell)
        {
            SoilCleanCost value = null;

            if (!m_dicTechTypeData.TryGetValue(m_selectedTechType, out value))
            {
                value = new SoilCleanCost();
                m_dicTechTypeData[m_selectedTechType] = value;
            }

            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                m_cellValueSystemChanged = true;
                cell.Tag = null;
                cell.Value = null;
                value.ExtraCost = -1.0;
            }
            else
            {
                string strValue = cell.Value.ToString().Trim();

                double dCost;

                if (!double.TryParse(strValue, out dCost))
                {
                    MessageBox.Show("기타비용은 0 이상의 숫자만 입력 가능합니다.");
                    m_cellValueSystemChanged = true;

                    if (cell.Tag == null)
                        cell.Value = cell.Tag;
                    else
                        cell.Value = FormConfirmArea.CostString((double)cell.Tag);

                    return;
                }
                else
                {
                    // 억원을 원으로 바꾸므로 100000000을 곱해준다.
                    dCost *= 100000000;
                }

                m_cellValueSystemChanged = true;
                cell.Value = FormConfirmArea.CostString(dCost);
                cell.Tag = dCost;
                value.ExtraCost = dCost;
            }
        }

        private void ProcessDiscount(DataGridViewCell cell)
        {
            SoilCleanCost value = null;

            if (!m_dicTechTypeData.TryGetValue(m_selectedTechType, out value))
            {
                value = new SoilCleanCost();
                m_dicTechTypeData[m_selectedTechType] = value;
            }

            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                m_cellValueSystemChanged = true;
                cell.Tag = null;
                cell.Value = null;
                value.Discount = -1.0;
            }
            else
            {
                string strValue = cell.Value.ToString().Trim();

                double dDiscount;

                if (!double.TryParse(strValue, out dDiscount) || dDiscount < 0.0 || dDiscount > 100.0)
                {
                    MessageBox.Show("할인율은 0에서 100 사이의 숫자만 입력 가능합니다.");
                    m_cellValueSystemChanged = true;

                    if (cell.Tag == null)
                        cell.Value = cell.Tag;
                    else
                        cell.Value = DiscountString((double)cell.Tag);

                    return;
                }

                m_cellValueSystemChanged = true;
                cell.Value = DiscountString(dDiscount);
                cell.Tag = dDiscount;
                value.Discount = dDiscount;
            }
        }

        private void ProcessPeriod(DataGridViewCell cell)
        {
            SoilCleanCost value = null;

            if (!m_dicTechTypeData.TryGetValue(m_selectedTechType, out value))
            {
                value = new SoilCleanCost();
                m_dicTechTypeData[m_selectedTechType] = value;
            }

            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                m_cellValueSystemChanged = true;
                cell.Tag = null;
                cell.Value = null;
                value.Period = -1;
            }
            else
            {
                string strValue = cell.Value.ToString().Trim();

                int nPeriod;

                if (!int.TryParse(strValue, out nPeriod) || nPeriod <= 0)
                {
                    MessageBox.Show("분석기간은 0보다 큰 정수만 입력 가능합니다.");
                    m_cellValueSystemChanged = true;

                    if (cell.Tag == null)
                        cell.Value = cell.Tag;
                    else
                        cell.Value = PeriodString((int)((double)cell.Tag + 0.1));
                    
                    return;
                }

                m_cellValueSystemChanged = true;
                cell.Value = PeriodString(nPeriod);
                cell.Tag = (double)nPeriod;
                value.Period = nPeriod;
            }
        }

        private string DiscountString(double dDiscount)
        {
            if (dDiscount - (int)dDiscount == 0.0)
                return string.Format("{0:F0}", dDiscount);

            return string.Format("{0:F1}", dDiscount);
        }

        private string PeriodString(int nPeriod)
        {
            return nPeriod.ToString();
            /*if (dPeriod - (int)dPeriod == 0.0)
                return string.Format("{0:F0}", dPeriod);
            
            return string.Format("{0:F1}", dPeriod);*/
        }

        private void ProcessSoilCleanCost(DataGridViewCell cell)
        {
            SoilCleanCost value = null;
            
            if (!m_dicTechTypeData.TryGetValue(m_selectedTechType, out value))
            {
                value = new SoilCleanCost();
                m_dicTechTypeData[m_selectedTechType] = value;
            }

            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                m_cellValueSystemChanged = true;
                cell.Tag = null;
                cell.Value = null;
                value.Cost = -1.0;
            }
            else
            {
                string strValue = cell.Value.ToString().Trim();

                double dCost;

                if (!double.TryParse(strValue, out dCost))
                {
                    MessageBox.Show("토양정화비용은 0 이상의 숫자만 입력 가능합니다.");
                    m_cellValueSystemChanged = true;

                    if (cell.Tag == null)
                        cell.Value = cell.Tag;
                    else
                        cell.Value = FormConfirmArea.CostString((double)cell.Tag);

                    return;
                }
                else
                {
                    // 억원을 원으로 바꾸므로 100000000을 곱해준다.
                    dCost *= 100000000;
                }

                m_cellValueSystemChanged = true;
                cell.Value = FormConfirmArea.CostString(dCost);
                cell.Tag = dCost;
                value.Cost = dCost;
            }
        }

        private void gridCost_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_cellValueSystemChanged)
            {
                m_cellValueSystemChanged = false;
                return;
            }

            if (e.ColumnIndex != 2 || e.RowIndex < 2)
                return;

            DataGridViewCell cell = gridCost.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        
            if (e.RowIndex == 2)
                ProcessWTPYear(cell);
            else if (e.RowIndex == 3)
                ProcessRejectionRatio(cell);
            else if (e.RowIndex == 4)
                ProcessHousehold(cell);
            
            m_cellValueSystemChanged = false;

            FormMain.Instance.BeginInvoke(new Action(() => SetCost()));

        }

        private void ProcessHousehold(DataGridViewCell cell)
        {
            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                m_cellValueSystemChanged = true;
                cell.Tag = null;
                cell.Value = null;
                m_dInputHousehold = -1L;
            }
            else
            {
                string strValue = cell.Value.ToString().Trim();

                double dCost;

                if (!double.TryParse(strValue, out dCost))
                {
                    MessageBox.Show("가구수는 0 이상의 숫자만 입력 가능합니다.");
                    m_cellValueSystemChanged = true;

                    if (cell.Tag == null)
                        cell.Value = cell.Tag;
                    else
                        cell.Value = string.Format("{0:###,###,###,###,###,###}",(long)cell.Tag);

                    return;
                }
                else
                {

                }

                m_cellValueSystemChanged = true;
                cell.Value = string.Format("{0:###,###,###,###,###,###}" ,dCost);
                cell.Tag = (long)dCost;
                m_dInputHousehold = (long)dCost;
            }
        }
        private void ProcessRejectionRatio(DataGridViewCell cell)
        {
            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                m_cellValueSystemChanged = true;
                cell.Tag = null;
                cell.Value = null;
                m_dInputRejectionRatio = -1.0;
            }
            else
            {
                string strValue = cell.Value.ToString().Trim();

                double dCost;

                if (!double.TryParse(strValue, out dCost))
                {
                    MessageBox.Show("거부율은 0 이상의 숫자만 입력 가능합니다.");
                    m_cellValueSystemChanged = true;

                    if (cell.Tag == null)
                        cell.Value = cell.Tag;
                    else
                        cell.Value = string.Format("{0:F2}", (double)cell.Tag);

                    return;
                }
                else
                {

                }
                if(dCost > 100.0)
                {
                    MessageBox.Show("거부율은 100 이하의 숫자만 입력 가능합니다.");
                    dCost = 100.0;
                }

                m_cellValueSystemChanged = true;
                cell.Value = string.Format("{0:F2}", dCost);
                cell.Tag = dCost;
                m_dInputRejectionRatio = dCost;
            }
        }

        private void ProcessWTPYear(DataGridViewCell cell)
        {
            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                m_cellValueSystemChanged = true;
                cell.Tag = null;
                cell.Value = null;
                m_dInputWTPYear = -1.0;
            }
            else
            {
                string strValue = cell.Value.ToString().Trim();

                double dCost;

                if (!double.TryParse(strValue, out dCost))
                {
                    MessageBox.Show("연간 WTP는 0 이상의 숫자만 입력 가능합니다.");
                    m_cellValueSystemChanged = true;

                    if (cell.Tag == null)
                        cell.Value = cell.Tag;
                    else
                        cell.Value = string.Format("{0:###,###,###,###,###,###}" ,(double)cell.Tag);

                    return;
                }               

                m_cellValueSystemChanged = true;
                cell.Value = string.Format("{0:###,###,###,###,###,###}", dCost);
                cell.Tag = dCost;
                m_dInputWTPYear = dCost;
            }
        }

        private void ProcessBio(DataGridViewCell cell)
        {
            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                m_cellValueSystemChanged = true;
                cell.Tag = null;
                cell.Value = null;
                m_dBio = -1.0;
            }
            else
            {
                string strValue = cell.Value.ToString().Trim();

                double dCost;

                if (!double.TryParse(strValue, out dCost))
                {
                    MessageBox.Show("생태학적가치는 0 이상의 숫자만 입력 가능합니다.");
                    m_cellValueSystemChanged = true;

                    if (cell.Tag == null)
                        cell.Value = cell.Tag;
                    else
                        cell.Value = FormConfirmArea.CostString((double)cell.Tag);

                    return;
                }
                else
                {
                    // 억원을 원으로 바꾸므로 100000000을 곱해준다.
                    dCost *= 100000000;
                }

                m_cellValueSystemChanged = true;
                cell.Value = FormConfirmArea.CostString(dCost);
                cell.Tag = dCost;
                m_dBio = dCost;
            }
        }

        private void ProcessExistance(DataGridViewCell cell)
        {
            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                m_cellValueSystemChanged = true;
                cell.Tag = null;
                cell.Value = null;
                m_dExistance = -1.0;
            }
            else
            {
                string strValue = cell.Value.ToString().Trim();

                double dCost;

                if (!double.TryParse(strValue, out dCost))
                {
                    MessageBox.Show("존재가치는 0 이상의 숫자만 입력 가능합니다.");
                    m_cellValueSystemChanged = true;

                    if (cell.Tag == null)
                        cell.Value = cell.Tag;
                    else
                        cell.Value = FormConfirmArea.CostString((double)cell.Tag);

                    return;
                }
                else
                {
                    // 억원을 원으로 바꾸므로 100000000을 곱해준다.
                    dCost *= 100000000;
                }

                m_cellValueSystemChanged = true;
                cell.Value = FormConfirmArea.CostString(dCost);
                cell.Tag = dCost;
                m_dExistance = dCost;
            }
        }

        private void ProcessInheritance(DataGridViewCell cell)
        {
            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                m_cellValueSystemChanged = true;
                cell.Tag = null;
                cell.Value = null;
                m_dInheritance = -1.0;
            }
            else
            {
                string strValue = cell.Value.ToString().Trim();

                double dCost;

                if (!double.TryParse(strValue, out dCost))
                {
                    MessageBox.Show("유산가치는 0 이상의 숫자만 입력 가능합니다.");
                    m_cellValueSystemChanged = true;

                    if (cell.Tag == null)
                        cell.Value = cell.Tag;
                    else
                        cell.Value = FormConfirmArea.CostString((double)cell.Tag);

                    return;
                }
                else
                {
                    // 억원을 원으로 바꾸므로 100000000을 곱해준다.
                    dCost *= 100000000;
                }

                m_cellValueSystemChanged = true;
                cell.Value = FormConfirmArea.CostString(dCost);
                cell.Tag = dCost;
                m_dInheritance = dCost;
            }
        }

        public bool CheckNullData()
        {
            if (!NullCheck(gridCondition, "토양정화비용이", 2, 1))
                return false;

            if (!NullCheck(gridCondition, "분석기간이", 4, 1))
                return false;

            if (!NullCheck(gridCondition, "할인율이", 5, 1))
                return false;

            if (!NullCheck(gridCost, "연간WTP[원/가구/년]이 ",2, 2))
                return false;

            if (!NullCheck(gridCost, "지불거부율이", 3,2))
                return false;

            if (!NullCheck(gridCost, "가구수(개략치)가", 4, 2))
                return false;

            return true;
        }

        private bool NullCheck(DataGridView grid, string strItemName, int nRowIndex, int nColumnIndex)
        {
            DataGridViewCell cell = grid.Rows[nRowIndex].Cells[nColumnIndex];

            if (cell.Value == null || cell.Value.ToString().Trim().Length == 0)
            {
                MessageBox.Show(strItemName + " 비어있습니다.\r\n값을 입력해 주세요.");
                return false;
            }

            return true;
        }

        public SoilCleanCost GetSoilCleanCost(TechType type)
        {
            SoilCleanCost value = null;

            if (!m_dicTechTypeData.TryGetValue(type, out value))
                return null;

            return value;
        }

        public void SetSoilCleanCost(TechType type, SoilCleanCost cost)
        {
            m_dicTechTypeData[type] = cost;
        }

        public void Reset()
        {
            SetCost();
            SetCondition();

            string strTechType = TechTypeToString(m_selectedTechType);

            foreach (object obj in m_comboTechType.Items)
            {
                if (obj.ToString() == strTechType)
                {
                    m_comboTechType.SelectedItem = obj;
                    break;
                }
            }

            string strWTPType = WTPTypeString(mSelectedWTP);
            foreach (object obj in m_comboWTP.Items)
            {
                if (obj.ToString() == strTechType)
                {
                    m_comboWTP.SelectedItem = obj;
                    break;
                }
            }

            string strRegion = RegionString(m_nSelectRegion);
            foreach (object obj in m_comboRegion.Items)
            {
                if (obj.ToString() == strRegion)
                {
                    m_comboRegion.SelectedItem = obj;
                    break;
                }
            }

        }

        
    }

    public class SoilCleanCost
    {
        // 0보다 작은 값이면 아직 입력되지 않은 값이다.
        private double m_dCost = -1.0;
        // 기간 : 년
        private int m_nPeriod = -1;
        // 할인율 : %
        private double m_dDiscount = -1.0;
        // 기타 비용
        private double m_dExtraCost = -1.0;

        public double ExtraCost
        {
            get { return m_dExtraCost; }
            set { m_dExtraCost = value; }
        }

        public double Cost
        {
            get { return m_dCost; }
            set { m_dCost = value; }
        }

        // 기간 : 년
        public int Period
        {
            get { return m_nPeriod; }
            set { m_nPeriod = value; }
        }

        // 할인율 : %
        public double Discount
        {
            get { return m_dDiscount; }
            set { m_dDiscount = value; }
        }

    }
}
