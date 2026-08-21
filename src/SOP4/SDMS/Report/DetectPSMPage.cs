using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChartDirector;
using System.Collections;
using DBUtility;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using SDMS.Report;
using Microsoft.Win32;
using System.Diagnostics;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;

namespace SDMS
{
    public partial class DetectPSMPage : Form
    {
        //현재 콤보박스에 표시되어있는 내용들(건물그룹, 건물, 존, 날짜등..)
        //값은 FormMain2에서 받아옴
        private BuildingGroup m_group = new BuildingGroup();
        private Building m_building = new Building();
        private Zone m_zone = new Zone();

        private string strbuilding = "";

        //private int m_nColumnCount = 8;
        private int m_nNO_INDEX = 0;
        private int m_nTIME_INDEX = 1;
        private int m_nMATERIAL_INDEX = 2;
        private int m_nSENSOR_NAME_INDEX = 3;
        private int m_nBUILDING_INDEX = 4;
        private int m_nDETECT_LOCATION_INDEX = 5;
        private int m_nDETECT_LEVEL_INDEX = 6;
        private int m_nMEMO_INDEX = 7;
        private int m_nPOPUP_DETECT_DATA_INDEX = 8;
        private int m_nSTATUS_INDEX = 9;

        //버튼클릭여부
        bool btnSelect = false;
        //모든데이터 보여줄지 여부
        bool AllBuildingGroup = false;
        bool AllBuilding = false;
        bool AllFloor = false;

        private Dictionary<int, string[]> dicLabels = new Dictionary<int, string[]>();
        private Dictionary<int, DateTime[]> dicStartDetectDates = new Dictionary<int, DateTime[]>();
        private Dictionary<int, DateTime[]> dicEndDetectDates = new Dictionary<int, DateTime[]>();


        //현재 선택된 날짜(선택된 기간이 바뀌었는지 아닌지 알기 위한..)
        private DateTime m_SelectedMinDate;
        private DateTime m_SelectedMaxDate;
        private ArrayList m_arrSelectedZone = null;

        // 그래프 보기 옵션
        /// <summary>
        /// 0:분
        /// 1:시
        /// 2:일
        /// 3:주
        /// 4:월
        /// 5:연
        /// </summary>
        private int m_nSplitUnitOfMeansure = -1;
        private int m_nSplitUnitOfMeansureDetail = -1;
        private int m_nViewCount = -1;
        private int m_nCurrentPage = -1;
        private int m_nTotalPage = -1;

        //DB쿼리로 찾은 결과를 여기에 저장
        ArrayList m_arrHistoryData = null;

        //한글파일 저장에 관련된 변수,배열,클래스...
        private int storage = 0;
        private ArrayList SaveArr = new ArrayList();

        private HwpCtrlData m_hwpCtrl = null;
        internal HwpCtrlData HwpCtrl
        {
            get { return m_hwpCtrl; }
            set { m_hwpCtrl = value; }
        }

        //화면이 처음 로드 되었는가?
        private bool isFirstLoad = false;
        public bool IsFirstLoad
        {
            get { return isFirstLoad; }
            set { isFirstLoad = value; }
        }

        private Dictionary<int, Report.DetectPSMLog> m_dicDetectLog = null;

        private Report.ReactionPSMManager m_detectMgr = null;
        private Report.ReactionPSMManager.RefreshCheckData m_checkData = new ReactionPSMManager.RefreshCheckData();

        public Report.ReactionPSMManager.RefreshCheckData RefreshCheckData
        {
            get { return m_checkData; }
        }

        public DetectPSMPage(Report.ReactionPSMManager detectMgr)
        {
            this.DoubleBuffered = true;

            InitializeComponent();

            Type dgvType1 = this.gvMain.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(gvMain, true, null);

            isFirstLoad = true;

            m_detectMgr = detectMgr;

            //보안모듈 등록
            m_hwpCtrl = new HwpCtrlData();
            m_hwpCtrl.SetRegistry();

            m_arrHistoryData = new ArrayList();

            //gvMain.CellClick += dataGridView1_CellClick;
        }



        private void DetectPage_Load(object sender, EventArgs e)
        {
            SetupDataGrid();
            InitLoadData();
            //화재탐지페이지가 처음 로드될 때 이벤트 한 번 실행
            FormMain.Instance.RefreshReportDetectPSM();
        }

        //이미지 캡쳐
        public void ControllCapture()
        {
            //Image image = new Bitmap(this.winChartViewer1.Width, this.winChartViewer1.Height);
            //Graphics g = Graphics.FromImage(image);
            //g.SmoothingMode = SmoothingMode.AntiAlias;//좀더 해상도 높이기위해서 사용
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;//좀더 해상도 높이기위해서 사용

            //IntPtr hDC = g.GetHdc();
            //SendMessage(this.winChartViewer1.Handle, 791 /*WM_PRINT*/, hDC, (IntPtr)30 /*(PRF_NONCLIENT | PRF_CLIENT | PRF_CHILDREN | PRF_ERASEBKGND)*/);

            //g.ReleaseHdc(hDC);
            //g.Dispose();
            //image.Save(Application.StartupPath + "\\report\\Detect.bmp");

            Bitmap bmp = new Bitmap(this.winChartViewer1.Width, this.winChartViewer1.Height);
            this.winChartViewer1.DrawToBitmap(bmp, new Rectangle(0, 0, this.winChartViewer1.Width, this.winChartViewer1.Height));
            bmp.Save(Application.StartupPath + "\\report\\Detect.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public void SetHwpData()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveDateTime.txt"))
            {
                file.WriteLine(lblMinDate.Text + " " + lblMaxDate.Text);
                file.WriteLine(lblBuilding.Text);
                file.Close();
            }

            try
            {
                System.IO.StreamWriter stream = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveMemo.txt");
                stream.Close();
            }
            catch (Exception)
            {
            }
        }

        public void FileWriter()
        {
            SaveHwpCrtl();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveData.txt"))
            {
                foreach (string line in SaveArr)
                {
                    {
                        file.WriteLine(line);
                    }
                }
                file.Close();
            }
        }

        private void InitLoadData()
        {
            ArrayList arrSelectZoneList = new ArrayList();

            arrSelectZoneList = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");

            //Report.ReactionManager.Instance.ZoneSubmit(arrSelectZoneList, strStartDate, strEndDate);

            //최근6개월
            /*DateTime startDate = DateTime.Now.AddMonths(-6);
            DateTime EndDate = DateTime.Now;*/
            DateTime startDate, EndDate;
            int nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount;

            if (!FormMain.Instance.GetDetectPSMReportDate(out startDate, out EndDate))
                return;

            if (!FormMain.Instance.GetDetectPSMReportOption(out nSplitUnitOfMeansure, out nSplitUnitOfMeansureDetail, out nViewCount))
                return;

            m_SelectedMinDate = startDate;
            m_SelectedMaxDate = EndDate;
            m_arrSelectedZone = arrSelectZoneList;

            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            m_detectMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);


            // 날짜순으로 내림차순으로 정렬
            

            //dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;


            //찾은 검색결과를 DataGrid로 출력
            Load_DataGrid(null, null, null, null);
           

            //그래프 그리기
            CreateBarChart(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, nViewCount, true);
        }

        public void UpdateGraph()
        {

        }

        public void AllSubmit(bool allBuildingGroup, bool allBuilding, bool allFloor)
        {
            this.AllBuildingGroup = allBuildingGroup;
            this.AllBuilding = allBuilding;
            this.AllFloor = allFloor;
        }

        public void ComboSubmit(BuildingGroup group, Building building, Zone zone, bool btnSelect)
        {
            m_group = group;
            m_building = building;
            m_zone = zone;
            this.btnSelect = btnSelect;
        }

        public void ComboSubmit(string building)
        {
            this.strbuilding = building;
        }

        public string AddDate(ref DateTime dtMinDate, ref DateTime dtMaxDate, DateTime dtLastDate, int nAddType, int nAddSpacing)
        {
            string strReturn = string.Empty;

            if (dtLastDate <= dtMaxDate)
                return strReturn;

            switch (nAddType)
            {
                case 0:// 분
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddMinutes(nAddSpacing);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }

                    //strReturn = String.Format("{0:yyyy-MM-dd HH시 mm분}", dtMinDate);

                    break;
                case 1:// 시
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddHours(nAddSpacing);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }

                    //strReturn = String.Format("{0:yyyy-MM-dd HH시}", dtMinDate);

                    break;
                case 2:// 일
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddDays(nAddSpacing);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }

                    //strReturn = String.Format("{0:yyyy-MM-dd}", dtMinDate);

                    break;
                case 3:// 주
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddDays(nAddSpacing * 7);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }

                    //strReturn = String.Format("{0:yyyy년도} {1}주차", dtMinDate,
                    //System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dtMinDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday));

                    break;
                case 4:// 월
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddMonths(nAddSpacing);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }

                    //strReturn = String.Format("{0:yyyy년 M월}", dtMinDate);

                    break;
                case 5:// 연
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddYears(nAddSpacing);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }

                    //strReturn = String.Format("{0:yyyy년도}", dtMinDate);

                    break;
            }

            strReturn = SDMS.PopupDialog.FormDateTimeFormat.ReportDateTimeFormat.GetDateTimeParsing(dtMinDate, nAddType);

            return strReturn;
        }

        public void CreateBarChart(DateTime StartDate, DateTime EndDate, int nSplitUnitOfMeansure, int nSplitUnitOfMeansureDetail, int nViewCount, bool isLoad = false)
        {
            dicLabels.Clear();
            dicStartDetectDates.Clear();
            dicEndDetectDates.Clear();

            WebDBManager m_dbMgr = FormMain.Instance.DBManager;

            if (String.Equals(EndDate.ToShortDateString(), DateTime.Now.ToShortDateString()))
            {
                EndDate = DateTime.Now;
            }
            else
            {
                EndDate = EndDate.AddDays(1).AddSeconds(-1);
            }

            DateTime dtMinDate = StartDate;
            DateTime dtMaxDate = StartDate;

            m_nSplitUnitOfMeansure = nSplitUnitOfMeansure;
            m_nSplitUnitOfMeansureDetail = nSplitUnitOfMeansureDetail;
            m_nViewCount = nViewCount;

            //조회기간
            lblMinDate.Text = String.Format("{0}년 {1}월 {2}일 부터", StartDate.Year, StartDate.Month, StartDate.Day);
            lblMaxDate.Text = String.Format("{0}년 {1}월 {2}일 까지", EndDate.Year, EndDate.Month, EndDate.Day);

            if (isLoad == false)
            {
                lblBuilding.Text = strbuilding;
            }

            switch (nSplitUnitOfMeansure)
            {
                case 0:// 분
                    dtMaxDate = dtMaxDate.AddMinutes(nSplitUnitOfMeansureDetail);
                    break;
                case 1:// 시
                    dtMaxDate = dtMaxDate.AddHours(nSplitUnitOfMeansureDetail);
                    break;
                case 2:// 일
                    dtMaxDate = dtMaxDate.AddDays(nSplitUnitOfMeansureDetail);
                    break;
                case 3:// 주
                    dtMaxDate = dtMaxDate.AddDays((nSplitUnitOfMeansureDetail * 7) - (int)dtMinDate.DayOfWeek);
                    break;
                case 4:// 월
                    dtMaxDate = new DateTime(dtMaxDate.Year, dtMaxDate.Month, 1).AddMonths(nSplitUnitOfMeansureDetail);
                    break;
                case 5:// 연
                    dtMaxDate = new DateTime(dtMaxDate.Year, 1, 1).AddYears(nSplitUnitOfMeansureDetail);
                    break;
            }

            if (dtMaxDate > EndDate)
            {
                dtMaxDate = EndDate;
            }

            string strXDate = SDMS.PopupDialog.FormDateTimeFormat.ReportDateTimeFormat.GetDateTimeParsing(dtMinDate, nSplitUnitOfMeansure);

            int nCount = (from logs in m_detectMgr.DectectList.ToArray().Cast<Report.DetectPSMLog>()
                          where logs.Time >= dtMinDate
                          && logs.Time < dtMaxDate
                          select logs).Count();

            Queue<string> liLabel = new Queue<string>();
            Queue<DateTime> liStartDate = new Queue<DateTime>();
            Queue<DateTime> liEndDate = new Queue<DateTime>();

            liLabel.Enqueue(strXDate);
            liStartDate.Enqueue(dtMinDate);
            liEndDate.Enqueue(dtMaxDate);

            while (true)
            {
                if (dtMaxDate == EndDate)
                    break;

                strXDate = AddDate(ref dtMinDate, ref dtMaxDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail);

                if (String.IsNullOrWhiteSpace(strXDate))
                    break;

                liLabel.Enqueue(strXDate);
                liStartDate.Enqueue(dtMinDate);
                liEndDate.Enqueue(dtMaxDate);
            }

            int nIndex = 1;

            List<string> labels = new List<string>();
            List<DateTime> dateStarts = new List<DateTime>();
            List<DateTime> dateEnds = new List<DateTime>();

            while (liLabel.Count > 0)
            {
                if (labels.Count == nViewCount)
                {
                    dicLabels.Add(nIndex, labels.ToArray());
                    dicStartDetectDates.Add(nIndex, dateStarts.ToArray());
                    dicEndDetectDates.Add(nIndex, dateEnds.ToArray());

                    nIndex++;

                    labels.Clear();
                    dateStarts.Clear();
                    dateEnds.Clear();
                }

                labels.Add(liLabel.Dequeue());
                dateStarts.Add(liStartDate.Dequeue());
                dateEnds.Add(liEndDate.Dequeue());
            }

            if (labels.Count > 0)
            {
                dicLabels.Add(nIndex, labels.ToArray());
                dicStartDetectDates.Add(nIndex, dateStarts.ToArray());
                dicEndDetectDates.Add(nIndex, dateEnds.ToArray());
            }


            m_nCurrentPage = 1;
            m_nTotalPage = nIndex;

            cboPageIndex.Items.Clear();
            for (int i = 1; i <= m_nTotalPage; i++)
            {
                cboPageIndex.Items.Add(i);

                //if (m_nCurrentPage == i)
                //  cboPageIndex.SelectedIndex = i - 1;
            }

            cboPageIndex.SelectedIndex = cboPageIndex.Items.Count - 1;
            lblTotalPage.Text = String.Format("/ {0}", m_nTotalPage);

            SetChart();
        }

        private void SetChart()
        {
            int nParentWidth = this.Size.Width;
            int nSpace = 60;

            Size sizeGrid = this.gvMain.Size;
            Point ptGrid = this.gvMain.Location;

            XYChart c = new XYChart(sizeGrid.Width, 280);

            double dbAngle = 0;
            int nLabelLength = 0;

            List<double> liDatas = new List<double>();

            // x spot Text Angle
            if (m_nCurrentPage > 0)
            {
                int nAnglePoint = 20;

                if (dicLabels[m_nCurrentPage].Length > 0)
                {
                    int nStringLength = 0;

                    foreach (char cha in dicLabels[m_nCurrentPage][0].ToCharArray())
                    {
                        if (char.GetUnicodeCategory(cha) == System.Globalization.UnicodeCategory.OtherLetter)
                        {
                            nStringLength++;
                        }

                        nStringLength++;
                    }

                    nAnglePoint = nAnglePoint - (nStringLength / 2);
                }
                else
                {
                    switch (m_nSplitUnitOfMeansure)
                    {
                        case 0:
                            nAnglePoint = 9; break;
                        case 1:
                        case 2:
                            nAnglePoint = 11;
                            nAnglePoint = 11; break;
                        default:
                            nAnglePoint = 15; break;
                    }
                }

                if (dicLabels[m_nCurrentPage].Length > nAnglePoint)
                {
                    nLabelLength = 18 - dicLabels[m_nCurrentPage][0].Length;
                    nLabelLength = Convert.ToInt32(Math.Ceiling(nLabelLength * 1.35));

                    dbAngle = 20;
                }

                // 데이터 수량 계산
                for (int nIndex = 0; nIndex < dicStartDetectDates[m_nCurrentPage].Length; nIndex++)
                {
                    liDatas.Add((from logs in m_detectMgr.DectectList.ToArray().Cast<Report.DetectPSMLog>()
                                 where logs.Time >= dicStartDetectDates[m_nCurrentPage][nIndex]
                                 && logs.Time < dicEndDetectDates[m_nCurrentPage][nIndex]
                                 select logs).Count());
                }
            }

            // Y축 Label 세팅
            if (liDatas.Count > 0)
            {
                List<string> liYLabel = new List<string>();
                double dMaxValue = liDatas.Max() + 1;
                while (true)
                {
                    if (dMaxValue % 20 == 0)
                        break;

                    dMaxValue = dMaxValue + 1;
                }

                for (int i = 0; i < 11; i++)
                {
                    liYLabel.Add(((dMaxValue / 10) * i).ToString());
                }
                c.yAxis().setLinearScale(0, dMaxValue, liYLabel.ToArray());
            }

            c.yAxis().setTitle("누출 탐지 횟수", "Arial Bold", 11.25);
            c.yAxis().setLabelStyle("Arial", 10.75, 0x000000);
            c.xAxis().setLabels(m_nCurrentPage < 0 ? null : dicLabels[m_nCurrentPage]);
            c.xAxis().setLabelStyle("Arial", 10.75, 0x000000, dbAngle);

            c.setPlotArea(ptGrid.X + nSpace, 15, sizeGrid.Width - nSpace * 2, (dbAngle > 0) ? 195 + nLabelLength : 235);

            BarLayer layer = c.addBarLayer(m_nCurrentPage < 0 ? null : liDatas.ToArray(), 0xff0000);
            layer.setDataLabelStyle().setFontColor(Chart.Transparent);
            layer.setAggregateLabelFormat("{value}");
            layer.setAggregateLabelStyle("Arial Bold", 11.25, 0x000000, 0).setAlignment(Chart.Center);

            layer.setBarWidth(40); // Bar의 두께 설정

            winChartViewer1.Chart = c;
        }

        private void DetectPage_Resize(object sender, EventArgs e)
        {
            Rectangle rect = ClientRectangle;

            if (Width == 0 || Height == 0)
                return;


            int width = rect.Width - 100;
            if (width < 200)
            {
                width = 200;
            }

            SetChart();
            Point ptGrid = gvMain.Location;
            Size SizeGrid = gvMain.Size;
        }

        private void SetupDataGrid()
        {
            gvMain.Columns[m_nNO_INDEX].Width = 80;
            gvMain.Columns[m_nTIME_INDEX].Width = 200;
            gvMain.Columns[m_nMATERIAL_INDEX].Width = 120;
            gvMain.Columns[m_nSENSOR_NAME_INDEX].Width = 260;
            gvMain.Columns[m_nBUILDING_INDEX].Width = 280;
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].Width = 360;
            gvMain.Columns[m_nDETECT_LEVEL_INDEX].Width = 80;
            gvMain.Columns[m_nMEMO_INDEX].Width = 180;
            gvMain.Columns[m_nPOPUP_DETECT_DATA_INDEX].Width = 100;
            gvMain.Columns[m_nSTATUS_INDEX].Width = 120;
            /*this.Controls.Add(gvMain);

            // 컬럼의 AutoSizeMode는 AllCellsExceptHeader, AllCells, DisplayedCells, DisplayedCellsExceptHeader
            // 등의 방법을 사용하는 경우 데이터가 많을시 열너비 조정시간이 많이 걸린다.
            // 길이를 직접 지정할것, 고정길이는 none으로 지정하고 그외에는 디폴트로 처리되로록 한다. 

            gvMain.ColumnCount = m_nColumnCount;

            gvMain.Columns[m_nNO_INDEX].Name = "No";
            gvMain.Columns[m_nNO_INDEX].Width = 80;
            gvMain.Columns[m_nNO_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nNO_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            gvMain.Columns[m_nNO_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvMain.Columns[m_nNO_INDEX].ReadOnly = true;
            
            gvMain.Columns[m_nTIME_INDEX].Name = "일시";
            gvMain.Columns[m_nTIME_INDEX].Width = 240;
            gvMain.Columns[m_nTIME_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nTIME_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvMain.Columns[m_nTIME_INDEX].ReadOnly = true;

            gvMain.Columns[m_nMATERIAL_INDEX].Name = "물질";
            gvMain.Columns[m_nMATERIAL_INDEX].Width = 180;
            gvMain.Columns[m_nMATERIAL_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nMATERIAL_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvMain.Columns[m_nMATERIAL_INDEX].ReadOnly = true;

            gvMain.Columns[m_nBUILDING_INDEX].Name = "건물";
            gvMain.Columns[m_nBUILDING_INDEX].Width = 300;
            gvMain.Columns[m_nBUILDING_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gvMain.Columns[m_nBUILDING_INDEX].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            gvMain.Columns[m_nBUILDING_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvMain.Columns[m_nBUILDING_INDEX].ReadOnly = true;

            gvMain.Columns[m_nDETECT_LOCATION_INDEX].Name = "누출 발생장소";
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].Width = 360;
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].ReadOnly = true;

            gvMain.Columns[m_nDETECT_LEVEL_INDEX].Name = "알람 단계";
            gvMain.Columns[m_nDETECT_LEVEL_INDEX].Width = 80;
            gvMain.Columns[m_nDETECT_LEVEL_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nDETECT_LEVEL_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;
            gvMain.Columns[m_nDETECT_LEVEL_INDEX].ReadOnly = true;

            gvMain.Columns[m_nPOPUP_DETECT_DATA_INDEX].Name = "측정량추이";
            gvMain.Columns[m_nPOPUP_DETECT_DATA_INDEX].Width = 100;
            gvMain.Columns[m_nPOPUP_DETECT_DATA_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            gvMain.Columns[m_nPOPUP_DETECT_DATA_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nPOPUP_DETECT_DATA_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nSTATUS_INDEX].Name = "상태";
            gvMain.Columns[m_nSTATUS_INDEX].Width = 140;
            gvMain.Columns[m_nSTATUS_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            gvMain.Columns[m_nSTATUS_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nSTATUS_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;


            gvMain.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;*/

            Font font = gvMain.Font;
            gvMain.Font = new Font("맑은 고딕", 12.0f);
        }

        //private void SetGridRange(int nRowCount)
        //{
        //    if (nRowCount <= 0)
        //        return;

        //    DataGridViewRow row = new DataGridViewRow();

        //    for (int i = 0; i < m_nColumnCount; i++)
        //    {
        //        DataGridViewCell cell = null;

        //        if (i == m_nSTATUS_INDEX)
        //        {
        //            cell = new DataGridViewComboBoxCell() { Items = { "실제", "오동작", "테스트" } };
        //        }
        //        else if (i == m_nPOPUP_DETECT_DATA_INDEX)
        //        {
        //            cell = new DataGridViewButtonCell();
        //        }
        //        else
        //        {
        //            cell = new DataGridViewTextBoxCell();
        //        }

        //        row.Cells.Add(cell);
        //    }

        //    gvMain.Rows.Add(row);

        //    if (nRowCount > 1)
        //        gvMain.Rows.AddCopies(0, nRowCount - 1);

        //    foreach (DataGridViewRow _row in gvMain.Rows)
        //    {
        //        _row.Height = gvMain.RowTemplate.Height;
        //    }
        //}

        public void Load_DataGrid(Dictionary<SensorTagInfo, int> dicSensorHistories, Dictionary<UnE.PSM.PSMTank, int> dicTankHistories, Dictionary<UnE.Spatial.EquipmentZone, int> dicEquipZoneHistories, Dictionary<UnE.PSM.PSMMaterial, int> dicMaterialHistories)
        {
            if (m_dicDetectLog == null) m_dicDetectLog = new Dictionary<int, DetectPSMLog>();
            else m_dicDetectLog.Clear();

            if (dicSensorHistories != null)
                dicSensorHistories.Clear();

            if (dicTankHistories != null)
                dicTankHistories.Clear();

            if (dicEquipZoneHistories != null)
                dicEquipZoneHistories.Clear();

            if (dicMaterialHistories != null)
                dicMaterialHistories.Clear();

            SaveArr.Clear();
            gvMain.DataSource = null;
            detectPSMPageGridDataBindingSource.Clear();
            //gvMain.Rows.Clear();
            gvMain.Invalidate();          

            WebDBManager m_dbMgr = FormMain.Instance.DBManager;

            // SensorHistoryData List
            ArrayList arrSensorZoneHistory = null;//new ArrayList();
            arrSensorZoneHistory = m_detectMgr.DectectList;

            int nRowNo = 0;
            DateTime dtDetectTime;
            string strMaterialName = String.Empty;
            string strBuildingName = String.Empty;
            string strDetectLocationName = String.Empty;
            string strDetectionStatusName = String.Empty;
            string strAlarmLevel = "0단계";

            // 고속 처리를 위하여 Rows.Add() 대신 AddCopies()를 사용한다.
            int nRowCount = arrSensorZoneHistory.Count;
            //SetGridRange(nRowCount);

            List<Report.DetectPSMLog> logs = new List<DetectPSMLog>();

            for (int i = nRowCount - 1; i >= 0;i-- )
            //foreach (Report.DetectPSMLog historyData in arrSensorZoneHistory)
            {
                Report.DetectPSMLog historyData = (Report.DetectPSMLog)arrSensorZoneHistory[i];
                Zone zoneLink = ZoneManager.Instance.GetZone(historyData.zoneID);
                if (zoneLink == null)
                {
                    gvMain.Rows.RemoveAt(--nRowCount);
                    continue;
                }

                dtDetectTime = historyData.Time;
                strBuildingName = zoneLink.Building != null ? zoneLink.Building.DisplayText : "";
                strDetectionStatusName = historyData.DetectionStatusName;
                strAlarmLevel = String.Format("{0}단계", historyData.AlarmLevel);

                //외부공간은 건물그룹과 건물이 없기 때문에 따로 설정..
                if (strBuildingName == "")
                    strBuildingName = zoneLink.ZoneName;

                //탐지 물질 이름 설정
                if (historyData.PSMMaterial != null)
                    strMaterialName = historyData.PSMMaterial.Name;

                //탐지 지역 이름 설정
                if (historyData.DetectType == "누출 센서")
                {
                    if (historyData.EquipZone != null)
                        strDetectLocationName = historyData.EquipZone.DisplayText;
                }
                else//수동신고
                {
                    //수동신고는 EquipmentZone을 표시하지 않음
                    strDetectLocationName = "-";
                }

                string strSensorName = "-";

                if (historyData.PSMSensor != null)
                {
                    List<ISensor> sensorZones = SensorManager.Instance.GetPSMSensorZone(historyData.PSMSensor.ID);

                    if (sensorZones != null && sensorZones.Count > 0)
                    {
                        ISensor sensorZone = sensorZones[0];
                        SensorTagInfo tag = SensorTagHistoryManager.Instance.GetSensorTagFromSensorZone(sensorZone.ID);

                        if (tag != null)
                            strSensorName = tag.TagName;
                        else
                            strSensorName = sensorZone.Description;

                        SetParetoData(historyData.PSMSensor, tag, dicSensorHistories, dicTankHistories, dicEquipZoneHistories, dicMaterialHistories);
                    }
                }

                DetectPSMPageGridData data = new DetectPSMPageGridData();
                data.No = ++nRowNo;
                data.TimeStamp = dtDetectTime;
                data.Material = strMaterialName;
                data.SensorName = strSensorName;
                data.Building = strBuildingName;
                data.Location = strDetectLocationName;
                data.AlarmDepth = strAlarmLevel;
                data.Status = strDetectionStatusName;
                data.Memo = historyData.Memo;

                detectPSMPageGridDataBindingSource.Add(data);
                m_dicDetectLog.Add(nRowNo, historyData);
                logs.Add(historyData);
                /*DataGridViewRow row = gvMain.Rows[nRowNo++];
                row.Cells[m_nNO_INDEX].Value = nRowCount - nRowNo + 1;
                row.Cells[m_nTIME_INDEX].Value = dtDetectTime;
                row.Cells[m_nMATERIAL_INDEX].Value = strMaterialName;
                row.Cells[m_nBUILDING_INDEX].Value = strBuildingName;
                row.Cells[m_nDETECT_LOCATION_INDEX].Value = strDetectLocationName;
                row.Cells[m_nDETECT_LEVEL_INDEX].Value = strAlarmLevel;
                row.Cells[m_nPOPUP_DETECT_DATA_INDEX].Value = "상세보기";
                row.Cells[m_nSTATUS_INDEX].Value = strDetectionStatusName;

                row.Tag = historyData;

                m_dicDetectLog.Add(nRowCount - nRowNo + 1, historyData);*/

                //row.Cells[nColumnCount -1]

                /*dataGridView1.Rows.Add(rows);
                dataGridView1.Rows[count].Cells[0].Value = nNumber;
                dataGridView1.Rows[count].Cells[1].Value = historyData.Time;*/

                //SaveHwpCtrl(ref nHwpTable, ref count, ref nNumber);
            }

            gvMain.DataSource = detectPSMPageGridDataBindingSource;

            //for (int i = 0; i < nRowNo;i++ )
            //{
            //    DataGridViewRow row = gvMain.Rows[i];
            //    row.Tag = logs[i];
            //}

            logs.Clear();

            //gvMain.Sort(gvMain.Columns[1], ListSortDirection.Descending);
        }

        private void SetParetoData(UnE.PSM.PSMSensor psmSensor, SensorTagInfo tag, Dictionary<SensorTagInfo, int> dicSensorHistories, Dictionary<UnE.PSM.PSMTank, int> dicTankHistories, Dictionary<UnE.Spatial.EquipmentZone, int> dicEquipZoneHistories, Dictionary<UnE.PSM.PSMMaterial, int> dicMaterialHistories)
        {
            int nCount = 0;

            if (dicSensorHistories != null && tag != null)
            {
                if (dicSensorHistories != null)
                {
                    if (dicSensorHistories.TryGetValue(tag, out nCount) == false)
                        dicSensorHistories[tag] = 1;
                    else
                        dicSensorHistories[tag] = nCount + 1;
                }
            }

            if (dicTankHistories != null && psmSensor != null)
            {
                foreach (UnE.PSM.PSMTank tank in psmSensor.LinkedTankList)
                {
                    if (dicTankHistories.TryGetValue(tank, out nCount) == false)
                        dicTankHistories[tank] = 1;
                    else
                        dicTankHistories[tank] = nCount + 1;
                }
            }

            if (dicEquipZoneHistories != null && tag != null && tag.EquipmentZone != null)
            {
                if (dicEquipZoneHistories.TryGetValue(tag.EquipmentZone, out nCount) == false)
                    dicEquipZoneHistories[tag.EquipmentZone] = 1;
                else
                    dicEquipZoneHistories[tag.EquipmentZone] = nCount + 1;
            }

            if (dicMaterialHistories != null && psmSensor != null)
            {
                foreach (UnE.PSM.PSMTank tank in psmSensor.LinkedTankList)
                {
                    if (tank.Material == null)
                        continue;

                    if (dicMaterialHistories.TryGetValue(tank.Material, out nCount) == false)
                        dicMaterialHistories[tank.Material] = 1;
                    else
                        dicMaterialHistories[tank.Material] = nCount + 1;
                }
            }
        }

        private void SaveHwpCrtl()
        {
            // 한글파일 출력전에 데이터를 저장하도록 함.

            SaveArr.Clear();

            for (int index = 0; index < gvMain.RowCount; index++)
            {
                DataGridViewRow row = gvMain.Rows[index];

                SaveArr.Add(row.Cells[m_nNO_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[m_nTIME_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[m_nMATERIAL_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[m_nSENSOR_NAME_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[m_nDETECT_LOCATION_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[m_nSTATUS_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[m_nDETECT_LEVEL_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[m_nMEMO_INDEX].Value.ToString());
            }
        }

        public string GetReactionString(int nType)
        {
            string strType = "";
            switch (nType)
            {
                case 1:
                    strType = "화재 센서";
                    //strType = "자탐 센서";
                    break;
                case 2: strType = "소화 센서";
                    break;
                case 3: strType = "압력 센서";
                    break;
                case 4: strType = "수동 신고";
                    break;
                case 7: strType = "누출 센서";
                    break;
                default:
                    break;
            }

            return strType;
        }

        private void comboBox_Leave(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;

            if (cbo.Tag == null)
                return;

            if (gvMain.SelectedCells.Count < 1)
                return;

            DataGridViewCell cell = cbo.Tag as DataGridViewCell;

            Report.DetectPSMLog detectLog = m_dicDetectLog[Convert.ToInt32(gvMain.Rows[cell.RowIndex].Cells[m_nNO_INDEX].Value)];

            int nSensorReactionHistoryID = detectLog.SensorReactionHistoryID;
            int nSensorReactionHistoryLogID = detectLog.HistoryID;

            // DB에 저장
            m_detectMgr.UpdateStatusForSensorReactionHistory(nSensorReactionHistoryID, nSensorReactionHistoryLogID, cbo.Text);
            
            gvMain.EndEdit();

        }

        private void btnPreviousIndex_Click(object sender, EventArgs e)
        {
            if (m_nCurrentPage == 1)
                return;

            m_nCurrentPage--;

            cboPageIndex.SelectedItem = m_nCurrentPage;
        }

        private void btnNextIndex_Click(object sender, EventArgs e)
        {
            if (m_nCurrentPage == m_nTotalPage)
                return;

            m_nCurrentPage++;

            cboPageIndex.SelectedItem = m_nCurrentPage;
        }

        private void cboPageIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_nCurrentPage = Convert.ToInt32(cboPageIndex.SelectedItem);

            SetNavigatorEnable();
            SetChart();
        }

        private void SetNavigatorEnable()
        {
            if (m_nCurrentPage <= 1)
                btnPreviousIndex.Enabled = false;
            else
                btnPreviousIndex.Enabled = true;

            if (m_nCurrentPage == m_nTotalPage)
                btnNextIndex.Enabled = false;
            else
                btnNextIndex.Enabled = true;

            cboPageIndex.Enabled = btnPreviousIndex.Enabled || btnNextIndex.Enabled;
        }

        private void cboPageIndex_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox == null)
            {
                return;
            }

            e.DrawBackground();

            if (e.Index >= 0)
            {
                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Far;
                sf.Alignment = StringAlignment.Far;

                Brush brush = new SolidBrush(comboBox.ForeColor);

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    brush = SystemBrushes.HighlightText;
                }

                e.Graphics.DrawString(comboBox.Items[e.Index].ToString(), comboBox.Font, brush, e.Bounds, sf);
            }
        }

        private void btnSaveHWP_Click(object sender, EventArgs e)
        {

            btnSaveHWP.Enabled = false;
            PageBackstageHome.Instance.FrmReport.SaveHWPForDetectPSM();
            btnSaveHWP.Enabled = true;
        }

        private void gvMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitInfo = gvMain.HitTest(e.X, e.Y);

                if (hitInfo.RowIndex >= 0 && hitInfo.ColumnIndex >= 0)
                {
                    DataGridViewRow row = gvMain.Rows[hitInfo.RowIndex];
                    gvMain.ClearSelection();
                    row.Selected = true;

                    Report.DetectPSMLog detectLog = null;
                    int nLogIndex = (int)row.Cells[0].Value;

                    if (m_dicDetectLog.TryGetValue(nLogIndex, out detectLog))
                    {
                        popupMenu.Tag = detectLog;
                        popupMenu.Show(gvMain, e.Location);
                    }
                }
            }
        }

        private void gvMain_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == m_nSTATUS_INDEX)
            {
                /*gvMain.BeginEdit(true);

                ComboBox comboBox = (ComboBox)gvMain.EditingControl;

                if (comboBox != null)
                {
                    comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                    if (comboBox.Tag == null)
                    {
                        comboBox.Leave += comboBox_Leave;
                    }

                    comboBox.Tag = gvMain.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }*/
            }
            else if (e.ColumnIndex == m_nPOPUP_DETECT_DATA_INDEX)
            {
                object ob = gvMain.Rows[e.RowIndex].Tag;

                if (ob is DetectPSMLog)
                {
                    ShowChart((DetectPSMLog)ob);
                    //m_frmPSMSensorData = new PopupDialog.FormPSMSensorData(nPSMSensorID, dtDetectStart, dtDetectEnd);

                    /*this.Cursor = Cursors.WaitCursor;

                    DetectPSMLog log = (ob as DetectPSMLog);
                    int nPSMSensorID = log.PSMSensor.ID;
                    DateTime dtDetectStart = log.DetectStartDate;
                    DateTime dtDetectEnd = log.DetectEndDate;


                    int nMonitorPosition = UnE.SOP.ProxySOP.Instance.CCTVMontior;

                    if (nMonitorPosition < 1 || nMonitorPosition > Screen.AllScreens.Length)
                        nMonitorPosition = 1;

                    if (FormMain.Instance.PSMSensorDataForm == null || FormMain.Instance.PSMSensorDataForm.IsDisposed == true)
                    {
                        FormMain.Instance.PSMSensorDataForm = new PopupDialog.FormPSMSensorTrendData(log.PSMSensor, dtDetectStart, dtDetectEnd) { StartPosition = FormStartPosition.CenterParent };

                        Rectangle bounds = FormFrame.Instance.Bounds;

                        int nX = bounds.X + ((bounds.Width - FormMain.Instance.PSMSensorDataForm.Width) / 2);
                        int nY = bounds.Y + ((bounds.Height - FormMain.Instance.PSMSensorDataForm.Height) / 2);

                        FormMain.Instance.PSMSensorDataForm.StartPosition = FormStartPosition.Manual;
                        FormMain.Instance.PSMSensorDataForm.Location = new Point(nX, nY);
                    }
                    else
                    {
                        DBUtility.VariousData<DateTime> dtStart = new VariousData<DateTime>(dtDetectStart);
                        DBUtility.VariousData<DateTime> dtEnd = new VariousData<DateTime>(dtDetectEnd);

                        this.Cursor = Cursors.WaitCursor;
                        FormMain.Instance.PSMSensorDataForm.ChangeSensor(log.PSMSensor, dtStart, dtEnd);
                        this.Cursor = Cursors.Default;
                    }

                    try
                    {
                        FormMain.Instance.PSMSensorDataForm.Show(this);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }*/

                }
            }
        }

        private void ShowChart(DetectPSMLog log)
        {
            if (log == null)
                return;

            this.Cursor = Cursors.WaitCursor;
            if (log.PSMSensor == null)
                return;
            int nPSMSensorID = log.PSMSensor.ID;
            DateTime dtDetectStart = log.DetectStartDate;
            DateTime dtDetectEnd = log.DetectEndDate;


            int nMonitorPosition = UnE.SOP.ProxySOP.Instance.CCTVMontior;

            if (nMonitorPosition < 1 || nMonitorPosition > Screen.AllScreens.Length)
                nMonitorPosition = 1;

            if (FormMain.Instance.PSMSensorDataForm == null || FormMain.Instance.PSMSensorDataForm.IsDisposed == true)
            {
                FormMain.Instance.PSMSensorDataForm = new PopupDialog.FormPSMSensorTrendData(log.PSMSensor, dtDetectStart, dtDetectEnd) { StartPosition = FormStartPosition.CenterParent };

                Rectangle bounds = FormFrame.Instance.Bounds;

                int nX = bounds.X + ((bounds.Width - FormMain.Instance.PSMSensorDataForm.Width) / 2);
                int nY = bounds.Y + ((bounds.Height - FormMain.Instance.PSMSensorDataForm.Height) / 2);

                FormMain.Instance.PSMSensorDataForm.StartPosition = FormStartPosition.Manual;
                FormMain.Instance.PSMSensorDataForm.Location = new Point(nX, nY);
            }
            else
            {
                DBUtility.VariousData<DateTime> dtStart = new VariousData<DateTime>(dtDetectStart);
                DBUtility.VariousData<DateTime> dtEnd = new VariousData<DateTime>(dtDetectEnd);

                this.Cursor = Cursors.WaitCursor;
                FormMain.Instance.PSMSensorDataForm.ChangeSensor(log.PSMSensor, dtStart, dtEnd);
                this.Cursor = Cursors.Default;
            }

            try
            {
                FormMain.Instance.PSMSensorDataForm.Show(this);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void gvMain_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex < m_nPOPUP_DETECT_DATA_INDEX)
                {
                    if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                    {
                        DataGridViewRow row = gvMain.Rows[e.RowIndex];
                        gvMain.ClearSelection();
                        row.Selected = true;

                        Report.DetectPSMLog detectLog = null;
                        int nLogIndex = (int)row.Cells[0].Value;

                        if (m_dicDetectLog.TryGetValue(nLogIndex, out detectLog))
                        {
                            FormMain.Instance.SelectPSMActionPage(detectLog.HistoryID);
                        }
                    }
                }
            }
        }

        private void menuShowPSMReaction_Click(object sender, EventArgs e)
        {
            Report.DetectPSMLog detectLog = (Report.DetectPSMLog)popupMenu.Tag;

            if (detectLog != null)
            {
                FormMain.Instance.SelectPSMActionPage(detectLog.HistoryID);
            }
        }

        private void menuShowChart_Click(object sender, EventArgs e)
        {
            Report.DetectPSMLog detectLog = (Report.DetectPSMLog)popupMenu.Tag;

            if (detectLog != null)
            {
                ShowChart(detectLog);
            }
        }

        private void gvMain_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != m_nSTATUS_INDEX || e.RowIndex < 0)
                return;

            Report.DetectPSMLog detectLog = null;
            int nNo = (int)gvMain.Rows[e.RowIndex].Cells[m_nNO_INDEX].Value;

            //Report.DetectPSMLog detectLog = (Report.DetectPSMLog)gvMain.Rows[e.RowIndex].Tag;

            if (m_dicDetectLog.TryGetValue(nNo, out detectLog))
            //if (detectLog != null)
            {
                string strState = gvMain.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                int nSensorReactionHistoryID = detectLog.SensorReactionHistoryID;
                int nSensorReactionHistoryLogID = detectLog.HistoryID;

                // DB에 저장
                m_detectMgr.UpdateStatusForSensorReactionHistory(nSensorReactionHistoryID, nSensorReactionHistoryLogID, strState);
            }
        }

        public void SetVisibleHWPExport(bool visible)
        {
            btnSaveHWP.Visible = visible;
        }
    }
}
