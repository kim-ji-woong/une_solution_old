using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChartDirector;
using System.Collections;
using DBUtility2;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.IO;
using SDMS.Report;
using Microsoft.Win32;
using System.Diagnostics;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;
using SDMS.Help;

namespace SDMS.Report
{
    public partial class DetectTHPage : FormReportBase
    {
        private const int NO_INDEX = 0;
        private const int TIME_STAMP_INDEX = 1;
        private const int SENSOR_TYPE_INDEX = 2;
        private const int ALARMTYPE_INDEX = 3;
        private const int SENSOR_NAME_INDEX = 4;
        private const int LOCATION_INDEX = 5;
        private const int MEMO_INDEX = 6;
        private const int STATUS_INDEX = 7;

        //현재 콤보박스에 표시되어있는 내용들(건물그룹, 건물, 존, 날짜등..)
        //값은 FormMain2에서 받아옴
        private BuildingGroup m_group = new BuildingGroup();
        private Building m_building = new Building();
        private Zone m_zone = new Zone();

        private string strgroup = "";
        private string strbuilding = "";
        private string strfloor = "";

        private string strStartDate = "";
        private string strEndDate = "";

        //버튼클릭여부
        bool btnSelect = false;
        //모든데이터 보여줄지 여부
        bool AllBuildingGroup = false;
        bool AllBuilding = false;
        bool AllFloor = false;

        private Dictionary<int, string[]> dicLabels = new Dictionary<int, string[]>();
        private Dictionary<int, DateTime[]> dicStartDetectDates = new Dictionary<int, DateTime[]>();
        private Dictionary<int, DateTime[]> dicEndDetectDates = new Dictionary<int, DateTime[]>();

        private string strManagerName;
        private string strPhoneNumber = "";

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
        private int m_nViewCount = 16;
        private int m_nCurrentPage = -1;
        private int m_nTotalPage = -1;

        //DB쿼리로 찾은 결과를 여기에 저장
        //ArrayList m_arrHistoryData = null;

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

        private Dictionary<int, Report.DetectTHLog> m_dicDetectLog = null;

        private Report.ReactionTHManager m_detectMgr = null;
        private Report.ReactionTHManager.RefreshCheckData m_checkData = new ReactionTHManager.RefreshCheckData();

        public Report.ReactionTHManager.RefreshCheckData RefreshCheckData
        {
            get { return m_checkData; }
        }

        private ManualManager m_manualManager = null;

        public DetectTHPage(Report.ReactionTHManager detectMgr)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(dataGridView1, true);
            FormMain.SetDoubleBuffer(panelChart, true);

            isFirstLoad = true;

            m_detectMgr = detectMgr;

            //보안모듈 등록
            m_hwpCtrl = new HwpCtrlData();
            m_hwpCtrl.SetRegistry();

            //m_arrHistoryData = new ArrayList(); 

            this.InitCtrlSize(this);
            FormMain.Instance.CustomizeGridView(dataGridView1);

            m_manualManager = new ManualManager(this);
            SetManualID();
        }

        private void DetectTHPage_Load(object sender, EventArgs e)
        {
            SetupDataGrid();
            InitLoadData();
            // 탐지페이지가 처음 로드될 때 이벤트 한 번 실행
            FormMain.Instance.proc_cboLatelyDate_SelectedIndexChanged(sender, e);
        }

        //이미지 캡쳐
        public void ControllCapture()
        {
            Bitmap bmp = new Bitmap(panelChart.Width, panelChart.Height);
            panelChart.DrawToBitmap(bmp, new Rectangle(0, 0, panelChart.Width, panelChart.Height));

            var gg = Graphics.FromImage(bmp);
            var rect = panelChart.RectangleToScreen(panelChart.ClientRectangle);

            bmp.Save(Application.StartupPath + "\\report\\Detect.bmp");
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

            if (!FormMain.Instance.GetCurrentReportDate(out startDate, out EndDate))
                return;

            if (!FormMain.Instance.GetCurrentReportOption(out nSplitUnitOfMeansure, out nSplitUnitOfMeansureDetail, out nViewCount))
                return;

            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            m_detectMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);
            //SetupDataGrid();


            // 날짜순으로 내림차순으로 정렬


            //dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;


            //찾은 검색결과를 DataGrid로 출력
            Load_DataGrid(null, null);


            //그래프 그리기 
            CreateBarChart(startDate, EndDate, nSplitUnitOfMeansure, nSplitUnitOfMeansureDetail, true);
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

        public void ComboSubmit(string group, string building, string floor)
        {
            this.strgroup = group;
            this.strbuilding = building;
            this.strfloor = floor;
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

        public void CreateBarChart(DateTime StartDate, DateTime EndDate, int nSplitUnitOfMeansure, int nSplitUnitOfMeansureDetail, bool isLoad = false)
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

            //조회기간
            lblMinDate.Text = String.Format("{0}년 {1}월 {2}일 부터", StartDate.Year, StartDate.Month, StartDate.Day);
            lblMaxDate.Text = String.Format("{0}년 {1}월 {2}일 까지", EndDate.Year, EndDate.Month, EndDate.Day);

            if (isLoad == false)
            {
                lblBuilding.Text = strgroup + "  " + strbuilding + "  " + strfloor;
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

            int nCount = (from logs in m_detectMgr.DectectList.ToArray().Cast<Report.DetectTHLog>()
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
                if (labels.Count == m_nViewCount)
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
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
            lblTotalPage.Location = new Point(panel1.Location.X + panel1.Width - lblTotalPage.Width, panel1.Location.Y + panel1.Height + 5);

            //SetChart();
            SetLabelLocation();
            panelChart.Invalidate();
        }

        List<double> m_chartDatas = null;
        private void SetChart()
        {
            m_chartDatas = new List<double>();

            // 데이터 수량 계산
            for (int nIndex = 0; nIndex < dicStartDetectDates[m_nCurrentPage].Length; nIndex++)
            {
                m_chartDatas.Add((from logs in m_detectMgr.DectectList.ToArray().Cast<Report.DetectTHLog>()
                                  where logs.Time >= dicStartDetectDates[m_nCurrentPage][nIndex]
                                  && logs.Time < dicEndDetectDates[m_nCurrentPage][nIndex]
                                  select logs).Count());
            }
        }

        private Brush brushBg = new SolidBrush(Color.FromArgb(0xff, 0x28, 0x28, 0x28));
        private Brush brushRed = new SolidBrush(Color.FromArgb(0xff, 0xdb, 0x00, 0x00));
        private Brush brushGray = new SolidBrush(Color.FromArgb(0xff, 0xd1, 0xd0, 0xce));

        private void panelChart_Paint(object sender, PaintEventArgs e)
        {
            SetChart();

            e.Graphics.Clear(Color.FromArgb(47, 45, 40));

            if (m_chartDatas == null || m_chartDatas.Count == 0)
                return;

            float sizePer = 1.0f;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
                sizePer = 0.5f;
            else if (FormMain.Instance.Resolution == Resolution.Other)
                sizePer = 0.75f;

            Font font = new System.Drawing.Font(Program.prgFont, 20F * sizePer, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            int nBigRectSize = (int)(304 * sizePer);
            int nMediumRectSize = (int)(212 * sizePer);

            int nTopEmpty = (int)(75 * sizePer);
            int nEmpty = (int)(60 * sizePer);
            int nSpace = (int)(8 * sizePer); // 한개 한개 간격

            Point beginPT = new Point(0, 0);
            Point drawPT = beginPT;

            Size RectSize = new System.Drawing.Size((int)(424 * sizePer), (int)(405 * sizePer));
            Size PanelSize = new System.Drawing.Size();

            int nRectCount = 1;
            double nTotalCnt = 0;
            for (int i = 0; i < m_chartDatas.Count; i++)
            {
                if (nRectCount == 17)
                    break;

                double cnt = Convert.ToDouble(m_chartDatas[i]);
                nTotalCnt = nTotalCnt + cnt;
                nRectCount++;
            }

            nRectCount = 1;

            string strDateFormat = "yyyy-MM-dd";
            switch (m_nSplitUnitOfMeansure)
            {
                case 0:// 분
                    strDateFormat = "yyyy-MM-dd HH시mm분";
                    break;
                case 1:// 시
                    strDateFormat = "yyyy-MM-dd HH시";
                    break;
                case 2:// 일
                    strDateFormat = "yyyy-MM-dd";
                    break;
                case 3:// 주
                    strDateFormat = "yyyy-MM-dd";
                    break;
                case 4:// 월
                    strDateFormat = "yyyy-MM";
                    break;
                case 5:// 연
                    strDateFormat = "yyyy년";
                    break;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (int i = 0; i < m_chartDatas.Count; i++)
            {
                if (nRectCount == 17)
                    break;

                Rectangle RectRed = new Rectangle(drawPT.X + nEmpty, drawPT.Y + nTopEmpty, nBigRectSize, nBigRectSize);
                Rectangle RectGray = new Rectangle(drawPT.X + nEmpty, drawPT.Y + nTopEmpty, nBigRectSize, nBigRectSize);
                Rectangle RectSmall = new Rectangle((int)(RectRed.Width * 0.5 - nMediumRectSize * 0.5) + RectRed.X, (int)(RectRed.Width * 0.5 - nMediumRectSize * 0.5) + RectRed.Y, nMediumRectSize, nMediumRectSize);

                double cnt = Convert.ToInt32(m_chartDatas[i]);
                float value = 0.0f;
                if (nTotalCnt > 0)
                    value = (float)cnt / (float)nTotalCnt * 360.0f;

                g.FillRectangle(brushBg, new Rectangle(drawPT.X, drawPT.Y, RectSize.Width, RectSize.Height));
                g.FillPie(brushGray, RectGray, 0.0f, 360.0f);
                // -90 = 0도
                g.FillPie(brushRed, RectRed, -90.0f, value);
                g.FillPie(brushBg, RectSmall, 0.0f, 360.0f);

                SizeF fontSize = g.MeasureString(dicStartDetectDates[m_nCurrentPage][i].ToString(strDateFormat), font);
                g.DrawString(dicStartDetectDates[m_nCurrentPage][i].ToString(strDateFormat), font, brushGray, drawPT.X + RectSize.Width - (RectSize.Width / 2) - (fontSize.Width / 2), drawPT.Y + (25 * sizePer));
                fontSize = g.MeasureString(cnt + "회", font);
                g.DrawString(cnt + "회", font, brushGray, drawPT.X + RectSize.Width - (RectSize.Width / 2) - (fontSize.Width / 2), drawPT.Y + (200 * sizePer));

                if (drawPT.X + RectSize.Width > PanelSize.Width)
                    PanelSize.Width = drawPT.X + RectSize.Width;
                if (drawPT.Y + RectSize.Height > PanelSize.Height)
                    PanelSize.Height = drawPT.Y + RectSize.Height;

                if (nRectCount % 8 == 0)
                {
                    drawPT = new Point(beginPT.X, drawPT.Y + RectSize.Height + nSpace);
                }
                else
                    drawPT = new Point(drawPT.X + RectSize.Width + nSpace, drawPT.Y);

                nRectCount++;
            }
        }

        private void DetectTHPage_Resize(object sender, EventArgs e)
        {
            SetChildCtrlResize(this, 0, 0);
            SetLabelLocation();
            SetupDataGrid();
            dataGridView1.Size = new System.Drawing.Size(this.Width, dataGridView1.Height);
            lblTotalPage.Location = new Point(panel1.Location.X + panel1.Width - lblTotalPage.Width, panel1.Location.Y + panel1.Height + 5);
        }

        private void SetLabelLocation()
        {
            label3.Location = new Point(label2.Location.X + label2.Width, label3.Location.Y);
            label5.Location = new Point(label3.Location.X + label3.Width, label5.Location.Y);
            lblMinDate.Location = new Point(label5.Location.X + label5.Width, lblMinDate.Location.Y);
            lblMaxDate.Location = new Point(lblMinDate.Location.X + lblMinDate.Width, lblMaxDate.Location.Y);

            label4.Location = new Point(lblMaxDate.Location.X + lblMaxDate.Width + 10, label4.Location.Y);
            label6.Location = new Point(label4.Location.X + label4.Width, label6.Location.Y);
            lblBuilding.Location = new Point(label6.Location.X + label6.Width, lblBuilding.Location.Y);
        }

        private void SetupDataGrid()
        {
            //dataGridView1.CellClick += dataGridView1_CellClick;

            float sizePer = 1f;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
                sizePer = 0.5f;
            else if (FormMain.Instance.Resolution == Resolution.Other)
                sizePer = 0.75f;

            dataGridView1.Columns[NO_INDEX].Width = (int)(180 * sizePer);
            dataGridView1.Columns[TIME_STAMP_INDEX].Width = (int)(400 * sizePer);
            dataGridView1.Columns[SENSOR_TYPE_INDEX].Width = (int)(400 * sizePer);
            dataGridView1.Columns[SENSOR_NAME_INDEX].Width = (int)(160 * sizePer);
            dataGridView1.Columns[ALARMTYPE_INDEX].Width = (int)(400 * sizePer);
            dataGridView1.Columns[LOCATION_INDEX].Width = (int)(260 * sizePer);
            dataGridView1.Columns[MEMO_INDEX].Width = (int)(170 * sizePer);
            dataGridView1.Columns[STATUS_INDEX].Width = (int)(200 * sizePer);

            Font font = dataGridView1.Font;
            dataGridView1.Font = new Font(Program.prgFont, 24.0f * sizePer);
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != STATUS_INDEX || e.RowIndex < 0)
                return;

            object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

            if (value == null)
                return;

            string strState = value.ToString();
            Report.DetectTHLog detectLog = m_dicDetectLog[Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value)];

            int nSensorReactionHistoryID = detectLog.SensorReactionHistoryID;
            int nSensorReactionHistoryLogID = detectLog.HistoryID;

            // DB에 저장
            m_detectMgr.UpdateStatusForSensorReactionHistory(nSensorReactionHistoryID, nSensorReactionHistoryLogID, strState);
        }

        private void SetGridRange(int nRowCount)
        {
            if (nRowCount <= 0)
                return;

            DataGridViewRow row = new DataGridViewRow();
            int nColumnCount = dataGridView1.Columns.Count;

            for (int i = 0; i < nColumnCount; i++)
            {
                DataGridViewCell cell = new DataGridViewTextBoxCell();

                if (i + 1 == nColumnCount)
                {
                    cell = new DataGridViewComboBoxCell() { Items = { "실제", "오동작", "테스트" } };
                }

                row.Cells.Add(cell);
            }

            dataGridView1.Rows.Add(row);

            if (nRowCount > 1)
                dataGridView1.Rows.AddCopies(0, nRowCount - 1);

            foreach (DataGridViewRow _row in dataGridView1.Rows)
            {
                _row.Height = dataGridView1.RowTemplate.Height;
            }
        }

        public void Load_DataGrid(Dictionary<SensorTagInfo, int> dicSensorHistories, Dictionary<UnE.Spatial.EquipmentZone, int> dicEquipZoneHistories)
        {
            if (dicSensorHistories != null)
                dicSensorHistories.Clear();

            if (dicEquipZoneHistories != null)
                dicEquipZoneHistories.Clear();
            //SetupDataGrid();

            if (m_dicDetectLog == null)
                m_dicDetectLog = new Dictionary<int, DetectTHLog>();
            else
                m_dicDetectLog.Clear();

            SaveArr.Clear();
            dataGridView1.DataSource = null;
            gridDataBindingSource.Clear();
            //dataGridView1.Rows.Clear();
            dataGridView1.Invalidate();

            WebDBManager m_dbMgr = FormMain.Instance.DBManager;

            // SensorHistoryData List
            List<DetectTHLog> arrSensorZoneHistory = null;//new ArrayList();
            arrSensorZoneHistory = m_detectMgr.DectectList;

            //int nHwpTable = 10;
            int count = 0;
            //int nNumber = 1;

            // 고속 처리를 위하여 Rows.Add() 대신 AddCopies()를 사용한다.
            int nRowCount = arrSensorZoneHistory.Count;
            //SetGridRange(nRowCount);

            // Key : SensorReactionHistory ID
            // Value : SensorZone ID
            Dictionary<int, int> dicSensorZoneIDs = GetSensorReactionLogs();

            foreach (Report.DetectTHLog historyData in from historyData in arrSensorZoneHistory.Cast<Report.DetectTHLog>()
                                                     orderby historyData.Time descending
                                                     select historyData
                                                     )
            {
                Zone zoneLink = ZoneManager.Instance.GetZone(historyData.zoneID);
                if (zoneLink == null)
                {
                    dataGridView1.Rows.RemoveAt(--nRowCount);
                    continue;
                }

                string szBuildingName = zoneLink.Building != null ? zoneLink.Building.DisplayText : "";
                string szGroupName = szBuildingName != "" ? zoneLink.Building.BuildingGroup.BuildingGroupName : "";

                string strDetectionStatusName = historyData.DetectionStatusName;


                //외부공간은 건물그룹과 건물이 없기 때문에 따로 설정..
                if (szGroupName == "")
                    szGroupName = "외부 영역";
                if (szBuildingName == "")
                    szBuildingName = zoneLink.ZoneName;

                string strFloorIndex = zoneLink.Floor != null ? zoneLink.Floor.ToString() : "";
                string strType = "";

                FacilityManagerGroup ManagerGroup = null;
                Building buildingFind = zoneLink.Building;


                string equipZoneName = "";

                EquipmentZone equipZone = null;

                strType = historyData.DetectType;
                if (strType == "자탐 센서" || strType == "화재 센서")
                {
                    equipZone = historyData.EquipZone;

                    if (equipZone != null)
                        equipZoneName = equipZone.DisplayText;
                }
                else//수동신고
                {
                    equipZoneName = zoneLink.ZoneName;
                    /*ArrayList arEquipzone = ZoneManager.Instance.GetEquipmentZoneList(zoneLink);
                    if (arEquipzone != null && arEquipzone.Count > 0)
                    {
                        equipZone = (EquipmentZone)arEquipzone[0];
                    }

                    //수동신고는 EquipmentZone을 표시하지 않음
                    equipZoneName = "-";*/
                }


                if (equipZone != null)
                {
                    ManagerGroup = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(IFacility.FacilityType.TEMPERATURE_HUMIDITY, equipZone, true);
                }

                if (ManagerGroup == null)
                    ManagerGroup = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(IFacility.FacilityType.TEMPERATURE_HUMIDITY, buildingFind, true);

                if (ManagerGroup == null)
                    ManagerGroup = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.TEMPERATURE_HUMIDITY, true);

                strManagerName = FormMain.Instance.DataManager.GetFacilityManagerName(ManagerGroup, ref strPhoneNumber);

                int nSensorZoneID;
                string strSensorName = "-";
                SensorTagInfo tag = null;

                if (dicSensorZoneIDs.TryGetValue(historyData.SensorReactionHistoryID, out nSensorZoneID))
                {
                    tag = SensorTagHistoryManager.Instance.GetSensorTagFromSensorZone(nSensorZoneID);

                    if (tag != null)
                        strSensorName = tag.TagName;
                    else
                    {
                        ISensor sensorZone = SensorManager.Instance.GetSensorZone(nSensorZoneID);

                        if (sensorZone != null)
                        {
                            strSensorName = sensorZone.Description;
                        }
                    }
                }

                // ParetoChart에서 쓰이는 데이터를 DetectPage에서 구해온다.
                if (tag != null)
                {
                    int nCount = 0;

                    if (dicSensorHistories != null)
                    {
                        if (dicSensorHistories.TryGetValue(tag, out nCount) == false)
                            dicSensorHistories[tag] = 1;
                        else
                            dicSensorHistories[tag] = nCount + 1;
                    }

                    if (dicEquipZoneHistories != null)
                    {
                        if (tag.EquipmentZone != null)
                        {
                            if (dicEquipZoneHistories.TryGetValue(tag.EquipmentZone, out nCount) == false)
                                dicEquipZoneHistories[tag.EquipmentZone] = 1;
                            else
                                dicEquipZoneHistories[tag.EquipmentZone] = nCount + 1;
                        }
                    }
                }

                // Data Binding
                DetectPageTHGridData data = new DetectPageTHGridData();
                data.No = ++count;
                data.TimeStamp = historyData.Time;
                data.SensorType = strType;
                data.SensorName = strSensorName;
                data.AlarmType = historyData.AlarmType;
                //data.Floor = strFloorIndex;
                data.Location = equipZoneName;
                data.Status = strDetectionStatusName;
                data.Memo = historyData.Memo;
                gridDataBindingSource.Add(data);

                // Data 직접 삽입
                /*string[] rows = { " ", "", strType, szGroupName, szBuildingName, strFloorIndex, equipZoneName, strDetectionStatusName };

                int nColumnCount = rows.Count();
                DataGridViewRow row = dataGridView1.Rows[count++];

                for (int j = 0; j < nColumnCount; j++)
                {
                    row.Cells[j].Value = rows[j];
                }

                row.Cells[0].Value = count;
                row.Cells[1].Value = historyData.Time;*/

                m_dicDetectLog.Add(count, historyData);

                //row.Cells[nColumnCount -1]

                /*dataGridView1.Rows.Add(rows);
                dataGridView1.Rows[count].Cells[0].Value = nNumber;
                dataGridView1.Rows[count].Cells[1].Value = historyData.Time;*/

                //SaveHwpCtrl(ref nHwpTable, ref count, ref nNumber);
            }

            dataGridView1.DataSource = gridDataBindingSource;

            // 미리 정렬하여 가져오므로 별도 정렬은 불필요.
            //dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Descending);
        }

        // Key : SensorReactionHistory ID
        // Value : SensorZone ID
        private Dictionary<int, int> GetSensorReactionLogs()
        {
            Dictionary<int, int> dicSensorReactionLogs = new Dictionary<int, int>();

            foreach (SensorReactionTHLog log in m_detectMgr.AllReactionLog)
            {
                int nSensorZoneID;

                if (int.TryParse(log.Param2, out nSensorZoneID))
                {
                    dicSensorReactionLogs[log.ID] = nSensorZoneID;
                }
            }

            return dicSensorReactionLogs;
        }

        private void SaveHwpCrtl()
        {
            // 한글파일 출력전에 데이터를 저장하도록 함.

            SaveArr.Clear();

            for (int index = 0; index < dataGridView1.RowCount; index++)
            {
                DataGridViewRow row = dataGridView1.Rows[index];

                SaveArr.Add(row.Cells[NO_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[TIME_STAMP_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[SENSOR_TYPE_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[SENSOR_NAME_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[ALARMTYPE_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[LOCATION_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[STATUS_INDEX].Value.ToString());
                SaveArr.Add(row.Cells[MEMO_INDEX].Value.ToString());
            }
        }

        //private void SaveHwpCtrl(ref int nHwpTable, ref int count, ref int nNumber)
        //{
        //    int HwpIndex = 0;

        //    for (int k = nHwpTable; k < nHwpTable + 7; k++)
        //    {
        //        SaveArr.Add(dataGridView1.Rows[count].Cells[HwpIndex].Value.ToString());

        //        HwpIndex++;
        //    }
        //    nHwpTable += 7;
        //    count++;
        //    nNumber++;
        //}

        public string GetReactionString(int nType)
        {
            string strType = "온도/습도 센서";
            return strType;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != STATUS_INDEX || e.RowIndex < 0)
                return;

            dataGridView1.BeginEdit(true);

            ComboBox comboBox = (ComboBox)dataGridView1.EditingControl;

            if (comboBox != null)
            {
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;

                if (comboBox.Tag == null)
                {
                    comboBox.Leave += comboBox_Leave;
                }

                comboBox.Tag = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void comboBox_Leave(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;

            if (cbo.Tag == null)
                return;

            if (dataGridView1.SelectedCells.Count < 1)
                return;

            DataGridViewCell cell = cbo.Tag as DataGridViewCell;

            Report.DetectTHLog detectLog = m_dicDetectLog[Convert.ToInt32(dataGridView1.Rows[cell.RowIndex].Cells[0].Value)];

            int nSensorReactionHistoryID = detectLog.SensorReactionHistoryID;
            int nSensorReactionHistoryLogID = detectLog.HistoryID;

            // DB에 저장
            m_detectMgr.UpdateStatusForSensorReactionHistory(nSensorReactionHistoryID, nSensorReactionHistoryLogID, cbo.Text);

            // ComboBox 편집후 Enter Key 누르면 NullReference 예외가 생기는것 방지
            //dataGridView1.EndEdit();

        }

        private void btnPreviousIndex_Click(object sender, EventArgs e)
        {
            CloseReportMenu();

            if (m_nCurrentPage == 1)
                return;

            m_nCurrentPage--;

            cboPageIndex.SelectedItem = m_nCurrentPage;
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
            lblTotalPage.Location = new Point(panel1.Location.X + panel1.Width - lblTotalPage.Width, panel1.Location.Y + panel1.Height + 5);
        }

        private void btnNextIndex_Click(object sender, EventArgs e)
        {
            CloseReportMenu();

            if (m_nCurrentPage == m_nTotalPage)
                return;

            m_nCurrentPage++;

            cboPageIndex.SelectedItem = m_nCurrentPage;
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
            lblTotalPage.Location = new Point(panel1.Location.X + panel1.Width - lblTotalPage.Width, panel1.Location.Y + panel1.Height + 5);
        }

        private void cboPageIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_nCurrentPage = Convert.ToInt32(cboPageIndex.SelectedItem);

            SetNavigatorEnable();
            //SetChart(); 
            panelChart.Invalidate();
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
            if (m_manualManager.IsHelpMode)
                return;

            CloseReportMenu();

            btnSaveHWP.Enabled = false;
            PageBackstageHome.Instance.FrmReport.SaveHWPForDetectAndNotPoeration();
            btnSaveHWP.Enabled = true;
        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridView.HitTestInfo hitInfo = dataGridView1.HitTest(e.X, e.Y);

                if (hitInfo.RowIndex >= 0 && hitInfo.ColumnIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[hitInfo.RowIndex];
                    dataGridView1.ClearSelection();
                    row.Selected = true;

                    Report.DetectTHLog detectLog;
                    int nLogIndex = (int)row.Cells[0].Value;

                    if (m_dicDetectLog.TryGetValue(nLogIndex, out detectLog))
                    {
                        popupMenu.Tag = detectLog;
                        popupMenu.Show(dataGridView1, e.Location);
                    }
                }
            }
        }

        private void menuShowFireReaction_Click(object sender, EventArgs e)
        {
            Report.DetectTHLog log = (Report.DetectTHLog)popupMenu.Tag;

            if (log != null)
                FormMain.Instance.SelectActionPage(log.HistoryID);
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.ColumnIndex >= 0 && e.ColumnIndex < STATUS_INDEX && e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    dataGridView1.ClearSelection();
                    row.Selected = true;

                    Report.DetectTHLog detectLog;
                    int nLogIndex = (int)row.Cells[0].Value;

                    if (m_dicDetectLog.TryGetValue(nLogIndex, out detectLog))
                    {
                        FormMain.Instance.SelectActionTHPage(detectLog.HistoryID);
                        int nSpace3 = 26;
                        if (FormMain.Instance.Resolution == Resolution.FullHD)
                        {
                            nSpace3 = (int)(nSpace3 * 0.5);
                        }
                        FormMain.Instance.ReportCtrlWidthLineUp(nSpace3, true);
                        FormMain.Instance.FromDetectTHPageToActionTHPage();
                    }
                }
            }
        }

        public void SetVisibleHWPExport(bool visible)
        {
            btnSaveHWP.Visible = visible;
        }

        private void CloseReportMenu()
        {
            FormMain.Instance.CloseOtherReportMenu(PopupDialog.Report.ReportCategory.NONE);
        }

        private void this_MouseDown(object sender, MouseEventArgs e)
        {
            CloseReportMenu();
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(this, "SDMS_Report_Detect_TH");
            m_manualManager.SetID(label2, "SDMS_Report_Detect_TH");
            m_manualManager.SetID(btnSaveHWP, "Detect_TH_ExportReport");
            m_manualManager.SetID(panelChart, "Detect_TH_Graph");
            m_manualManager.SetID(btnPreviousIndex, "Detect_TH_Graph");
            m_manualManager.SetID(btnNextIndex, "Detect_TH_Graph");
            m_manualManager.SetID(lblTotalPage, "Detect_TH_Graph");
            m_manualManager.SetID(dataGridView1, "Detect_TH_Grid");

            m_manualManager.ProcessEvent();
        }
    }
}
