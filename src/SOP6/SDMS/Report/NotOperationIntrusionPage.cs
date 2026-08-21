using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChartDirector;
using DBUtility2;
using System.Collections;
using SDMS.Report;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Globalization;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;
using SDMS.Help;

namespace SDMS
{
    public partial class NotOperationIntrusionPage : FormReportBase
    {
        private const int NO_INDEX = 0;
        private const int SENSOR_TYPE_INDEX = 1;
        private const int BUILDING_GROUP_INDEX = 2;
        private const int BUILDING_INDEX = 3;
        private const int FLOOR_INDEX = 4;
        private const int DETECT_INDEX = 5;
        private const int FIRE_INDEX = 6;
        private const int MALFUNCTION_INDEX = 7;
        private const int FIELD_RECOVERY_INDEX = 8;
        private const int MALFUNCTION_RATE_INDEX = 9;
        private const int MANAGER_INDEX = 10;

        private ChartDirector.WinChartViewer c_PercentBarChart = new ChartDirector.WinChartViewer();
        public ChartDirector.WinChartViewer PercentBarChart
        {
            get { return c_PercentBarChart; }
            set { c_PercentBarChart = value; }
        }
        private BuildingGroup group = new BuildingGroup();
        private Building building = new Building();
        private Zone zone = new Zone();

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

        private string[] labels = null;

        double[] data0 = null;
        double[] data1 = null;
        double[] data2 = null;

        private string strManagerName;
        private string strPhoneNumber = "";


        //현재 선택된 날짜(선택된 기간이 바뀌었는지 아닌지 알기 위한..)
        private DateTime m_SelectedMinDate;
        private DateTime m_SelectedMaxDate;
        private ArrayList m_arrSelectedZone = null;


        private HwpCtrlData m_hwpCtrl = null;

        internal HwpCtrlData HwpCtrl
        {
            get { return m_hwpCtrl; }
            set { m_hwpCtrl = value; }
        }

        private ArrayList SaveArr = new ArrayList();

        private ReportData m_ReportData = new ReportData();
        internal ReportData ReportData
        {
            get { return m_ReportData; }
            set { m_ReportData = value; }
        }

        private static NotOperationIntrusionPage m_instance = null;
        public static NotOperationIntrusionPage Instance
        {
            get { return NotOperationIntrusionPage.m_instance; }
            set { NotOperationIntrusionPage.m_instance = value; }
        }

        private Report.ReactionIntrusionManager m_detectMgr = null;
        private Report.ReactionIntrusionManager.RefreshCheckData m_checkData = new ReactionIntrusionManager.RefreshCheckData();

        public Report.ReactionIntrusionManager.RefreshCheckData RefreshCheckData
        {
            get { return m_checkData; }
        }

        private int m_nViewCount = 8;
        private int m_nCurrentPage = -1;
        private int m_nTotalPage = -1;

        private bool m_bCheckCombo = false;

        private ManualManager m_manualManager = null;

        public NotOperationIntrusionPage(Report.ReactionIntrusionManager detectMgr)
        {
            m_instance = this;
            InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(dataGridView1, true);
            FormMain.SetDoubleBuffer(panelChart, true);

            m_detectMgr = detectMgr;
            m_hwpCtrl = new HwpCtrlData();
            
            c_PercentBarChart = winChartViewer1;

            m_bCheckCombo = true;
            SetComboBox();
            m_bCheckCombo = false; 

            InitCtrlSize(this);
            FormMain.Instance.CustomizeGridView(dataGridView1);

            m_manualManager = new ManualManager(this);
            SetManualID();
        }
         
        private void NotOperationIntrusionPage_Load(object sender, EventArgs e)
		{
            SetupDataGrid();
            //setComboBox();

            //SetupDataGrid();
            //Load_DataGrid();
            //createBarChart(winChartViewer1);

            //최근6개월
            /*DateTime startDate = DateTime.Now.AddMonths(-6);
            DateTime EndDate = DateTime.Now;*/
            DateTime startDate, EndDate;

            if (!FormMain.Instance.GetCurrentReportDate(out startDate, out EndDate))
                return;

            m_SelectedMinDate = startDate;
            m_SelectedMaxDate = EndDate;
            m_arrSelectedZone = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");

            FormMain.Instance.RefreshReportIntrusion(); 

            //찾은 검색결과를 DataGrid로 출력
            Load_DataGrid(true);
            CreateBarChart(winChartViewer1);
		}

        public void SetHwpData()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveDateTime.txt"))
            {
                file.WriteLine(lblMinDate.Text+lblMaxDate.Text);
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
            //image.Save(Application.StartupPath + "\\report\\Malfunction.bmp");

            //Bitmap bmp = new Bitmap(this.winChartViewer1.Width, this.winChartViewer1.Height);
            //this.winChartViewer1.DrawToBitmap(bmp, new Rectangle(0, 0, this.winChartViewer1.Width, this.winChartViewer1.Height));
            //bmp.Save(Application.StartupPath + "\\report\\Malfunction.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            Bitmap bmp = new Bitmap(panelChart.Width, panelChart.Height);
            panelChart.DrawToBitmap(bmp, new Rectangle(0, 0, panelChart.Width, panelChart.Height));

            var gg = Graphics.FromImage(bmp);
            var rect = panelChart.RectangleToScreen(panelChart.ClientRectangle);

            //using (Graphics graphics = Graphics.FromImage(bmp))
            //{
            //    graphics.FillRectangle(brushBg, new Rectangle(0, 0, panelChart.Width, pictureBox1.Height));
            //    //범례
            //    graphics.DrawImage(pictureBox1.Image, 0, 0, pictureBox1.Width, pictureBox1.Height);
            //}
            bmp.Save(Application.StartupPath + "\\report\\Malfunction.bmp");
        }

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);


        private void SetComboBox()
        {
            cboChart.Items.Add("주별 보기");
            cboChart.Items.Add("월별 보기");
            cboChart.Items.Add("분기별 보기");
            cboChart.Items.Add("연도별 보기");
            cboChart.SelectedIndex = 0;
        }

        public void AllSubmit(bool allBuildingGroup, bool allBuilding, bool allFloor)
        {
            this.AllBuildingGroup = allBuildingGroup;
            this.AllBuilding = allBuilding;
            this.AllFloor = allFloor;
        }

        public void ComboSubmit(BuildingGroup group, Building building, Zone zone, bool btnSelect)
        {
            this.group = group;
            this.building = building;
            this.zone = zone;
            this.btnSelect = btnSelect;
        }

        public void ComboSubmit(string group, string building, string floor)
        {
            this.strgroup = group;
            this.strbuilding = building;
            this.strfloor = floor;
        }

        public void ComboTxtDate(string strStrat, string strEnd)
        {
            strStartDate = strStrat;
            strEndDate = strEnd;
        }

        public void CreateBarChart(WinChartViewer viewer)
        { 
            if (cboChart.SelectedIndex == 0)
                SetWeekChart();
            else if (cboChart.SelectedIndex == 1)
                SetMonthChart();
            else if (cboChart.SelectedIndex == 2)
                SetQuarterChart();
            else if (cboChart.SelectedIndex == 3)
                SetYearChart();

            panelChart.Invalidate();

            //SetChartBar();
        }

        private void SetupDataGrid()
        {
            float sizePer = 1f;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
                sizePer = 0.5f;
            else if (FormMain.Instance.Resolution == Resolution.Other)
                sizePer = 0.75f;

            dataGridView1.Columns[NO_INDEX].Width = (int)(160 * sizePer);
            dataGridView1.Columns[SENSOR_TYPE_INDEX].Width = (int)(200 * sizePer);
            dataGridView1.Columns[BUILDING_GROUP_INDEX].Width = (int)(600 * sizePer);
            dataGridView1.Columns[BUILDING_INDEX].Width = (int)(1200 * sizePer);
            dataGridView1.Columns[FLOOR_INDEX].Width = (int)(140 * sizePer);
            dataGridView1.Columns[DETECT_INDEX].Width = (int)(160 * sizePer);
            dataGridView1.Columns[FIRE_INDEX].Width = (int)(160 * sizePer);
            dataGridView1.Columns[MALFUNCTION_INDEX].Width = (int)(160 * sizePer);
            dataGridView1.Columns[FIELD_RECOVERY_INDEX].Width = (int)(160 * sizePer);
            dataGridView1.Columns[MALFUNCTION_RATE_INDEX].Width = (int)(160 * sizePer);
            /*this.Controls.Add(dataGridView1);

            dataGridView1.ColumnCount = 11;

            dataGridView1.Columns[0].Name = "No";
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[0].Width = 80;

            dataGridView1.Columns[1].Name = "유형";
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[1].Width = 100;

            dataGridView1.Columns[2].Name = "건물 그룹";
            dataGridView1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[2].Width = 300;

            dataGridView1.Columns[3].Name = "건물";
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[3].Width = 600;

            dataGridView1.Columns[4].Name = "층";
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[4].Width = 70;

            dataGridView1.Columns[5].Name = "탐지";
            dataGridView1.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[5].Width = 80;

            dataGridView1.Columns[6].Name = "화재";
            dataGridView1.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[6].Width = 80;

            dataGridView1.Columns[7].Name = "오작동";
            dataGridView1.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[7].Width = 80;

            dataGridView1.Columns[8].Name = "현장복구";
            dataGridView1.Columns[8].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[8].Width = 80;

            dataGridView1.Columns[9].Name = "오작동률";
            dataGridView1.Columns[9].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[9].Width = 80;

            dataGridView1.Columns[10].Name = "담당자";
            dataGridView1.Columns[10].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[10].Visible = false;


            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[2].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[3].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[8].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[9].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[10].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.Columns[10].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);*/

            //Font font = dataGridView1.Font;
            dataGridView1.Font = new Font(Program.prgFont, (int)(24.0f * sizePer));
            //dataGridView1.ColumnHeadersDefaultCellStyle.Font = font;
        }

        public void Load_DataGrid(bool isLoad = false)
        {
            SaveArr.Clear();
            //lblNullResult.Visible = false;
            dataGridView1.DataSource = null;
            notOperationPageGridDataBindingSource.Clear();
            //dataGridView1.Rows.Clear();

            //string -> DateTime
            string strNowDate = "";
            string strBeforeDate = "";

            strNowDate = string.Format("{0} {1}:{2}:{3}", strEndDate.ToString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", strStartDate.ToString(), 00, 00, 00);

            int count = 0;
            int nCount = 1;

            int nHwpTable = 14;
            int k = 0;
            
            foreach (MulFunctionIntrusionLog mulFunctionLog in m_detectMgr.MulFunctionList)
            {
                //int nType = 
                Zone zone = mulFunctionLog.Zone;
                int nReactionCount = mulFunctionLog.ReactionCount;
                int nMulFunctionCount = mulFunctionLog.MulFunctionCount;
                int nFireCount = mulFunctionLog.FireCount;
                string strBuildingGroupName = mulFunctionLog.GroupName;
                string strBuildingName = mulFunctionLog.BuildingName;
                string strFloorName = mulFunctionLog.FloorName;
                double nPercentMulFunction = mulFunctionLog.PercentMulFunction;
                int nNotProcss = mulFunctionLog.Notprocess;

                string strType = mulFunctionLog.DetectType;


                EquipmentZone equipZone = null;
                List<EquipmentZone> arEquipzone = ZoneManager.Instance.GetEquipmentZoneList(zone);
                if (arEquipzone != null && arEquipzone.Count > 0)
                {
                    equipZone = (EquipmentZone)arEquipzone[0];
                }

                FacilityManagerGroup ManagerGroup = null;

                ManagerGroup = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(IFacility.FacilityType.Intrusion_S1, equipZone, true);


                if (ManagerGroup == null)
                    ManagerGroup = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(IFacility.FacilityType.Intrusion_S1, equipZone.LinkedZone.Building, true);

                if (ManagerGroup == null)
                    ManagerGroup = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.Intrusion_S1, true);

                strManagerName = FormMain.Instance.DataManager.GetFacilityManagerName(ManagerGroup, ref strPhoneNumber);

                NotOperationPageGridData data = new NotOperationPageGridData();
                data.No = nCount;
                data.SensorType = strType;
                data.BuildingGroup = strBuildingGroupName;
                data.Building = strBuildingName;
                data.Floor = strFloorName;
                data.Detect = nReactionCount.ToString();
                data.Fire = nFireCount.ToString();
                data.Malfunction = nMulFunctionCount.ToString();
                data.FieldRecovery = nNotProcss.ToString();
                data.MalfunctionRate = nPercentMulFunction.ToString() + "%";
                data.Manager = strManagerName;

                notOperationPageGridDataBindingSource.Add(data);
                /*string[] rows = { "", strType, strBuildingGroupName, strBuildingName, strFloorName, nReactionCount.ToString(), nFireCount.ToString(), nMulFunctionCount.ToString(), nNotProcss.ToString(), nPercentMulFunction.ToString() + "%", strManagerName };
                dataGridView1.Rows.Add(rows);
                dataGridView1.Rows[count].Cells[0].Value = nCount;*/



                int nColumnCount = dataGridView1.Columns.Count;

                foreach (var prop in data.GetType().GetProperties())
                {
                    if (prop.Name == "Manager")
                        continue;

                    SaveArr.Add(prop.GetValue(data, null).ToString());
                }
                /*int HwpIndex = 0;

                for (k = nHwpTable; k < nHwpTable + nColumnCount; k++)
                {
                    if (dataGridView1.Columns[HwpIndex].Visible)
                        SaveArr.Add(dataGridView1.Rows[count].Cells[HwpIndex].Value.ToString());


                    
                    HwpIndex++;
                }*/
                nHwpTable += nColumnCount;

                count++;
                nCount++;
            }

            if (count == 0)
            {
                //lblNullResult.Visible = true;
            }

            DateTime dtStart = DateTime.ParseExact(strStartDate, "yyyy-MM-dd", null);
            DateTime dtEnd = DateTime.ParseExact(strEndDate, "yyyy-MM-dd", null);

            strNowDate = string.Format("{0} {1}:{2}:{3}", dtEnd.ToShortDateString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", dtStart.ToShortDateString(), 00, 00, 00);

            string strdtMin = "";
            string strdtMax = "";

            if (isLoad == false)
            {
                lblBuilding.Text = strgroup + "  " + strbuilding + "  " + strfloor;
            }

            strdtMin = string.Format("{0}년 {1}월 {2}일", dtStart.Year, dtStart.Month, dtStart.Day);
            strdtMax = string.Format("{0}년 {1}월 {2}일", dtEnd.Year, dtEnd.Month, dtEnd.Day);

            //조회기간
            lblMinDate.Text = strdtMin + " 부터 ";
            lblMaxDate.Text = strdtMax + " 까지";

            dataGridView1.DataSource = notOperationPageGridDataBindingSource;

            lblMinDate.Location = new Point(label4.Location.X + label4.Width, lblMinDate.Location.Y);
            lblMaxDate.Location = new Point(lblMinDate.Location.X + lblMinDate.Width, lblMinDate.Location.Y);
            label7.Location = new Point(lblMaxDate.Location.X + lblMaxDate.Width + 5, lblMinDate.Location.Y);
            label8.Location = new Point(label7.Location.X + label7.Width, lblMinDate.Location.Y);
            lblBuilding.Location = new Point(label8.Location.X + label8.Width, lblMinDate.Location.Y); 
        }

        public void FileWriter()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveData.txt"))
            {
                foreach (string line in SaveArr)
                {
                    // If the line doesn't contain the word 'Second', write the line to the file. 
                    //if (!line.Contains("Second"))
                    {
                        file.WriteLine(line);
                    }
                }
                file.Close();
            }
        }

        private void NotOperationIntrusionPage_Resize(object sender, EventArgs e)
        {  
            SetChildCtrlResize(this, 0, 0);
            SetupDataGrid();

            lblMinDate.Location = new Point(label4.Location.X + label4.Width, lblMinDate.Location.Y);
            lblMaxDate.Location = new Point(lblMinDate.Location.X + lblMinDate.Width, lblMinDate.Location.Y);
            label7.Location = new Point(lblMaxDate.Location.X + lblMaxDate.Width + 5, lblMinDate.Location.Y);
            label8.Location = new Point(label7.Location.X + label7.Width, lblMinDate.Location.Y);
            lblBuilding.Location = new Point(label8.Location.X + label8.Width, lblMinDate.Location.Y);

            pictureBox1.Location = new Point(label3.Location.X + label3.Width + 10, label3.Location.Y + 2);
            
            lblTotalPage.Location = new Point(panel1.Location.X + panel1.Width - lblTotalPage.Width, panel1.Location.Y + panel1.Height + 5);  
        }

        private DateTime dt = DateTime.Now;

        private void SetWeekChart()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            DateTime dtNowDate = DateTime.Now;
            DateTime dtBeforeDate = DateTime.Now;

            string strNowDate = "";
            string strBeforeDate = "";

            strNowDate = string.Format("{0} {1}:{2}:{3}", strEndDate.ToString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", strStartDate.ToString(), 00, 00, 00);

            //주별
            DateTime dtStart, dtEnd;

            try
            {
                dtStart = DateTime.ParseExact(strStartDate, "yyyy-MM-dd", null);
                dtEnd = DateTime.ParseExact(strEndDate, "yyyy-MM-dd", null);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return;
            }

            //두 날짜의 달 차이 계산
            //int n_Monthts = 12 * (dtEnd.Year - dtStart.Year) + (dtEnd.Month - dtStart.Month);

            //1년에 55주

            TimeSpan Subdt = dt - dtStart;

            string strWeekday = dtStart.DayOfWeek.ToString();
            int nWeek = 0;
            int nWeekCount = 0;
            int nRest = 0; 
            switch (strWeekday)
            {
                case "Sunday":
                    nWeek = 6;
                    break;
                case "Monday":
                    nWeek = 5;
                    break;
                case "Tuesday":
                    nWeek = 4;
                    break;
                case "Wednesday":
                    nWeek = 3;
                    break;
                case "Thursday":
                    nWeek = 2;
                    break;
                case "Friday":
                    nWeek = 1;
                    break;
                case "Saturday":
                    nWeek = 0;
                    break;
            }
            //(전체일수 - 첫주의 일수)/7의 몫 nWeekCount = 몇주인지
            nWeekCount = (Subdt.Days - nWeek) / 7;
            //첫주것 더함
            nWeekCount++;

            //나머지
            nRest = (Subdt.Days - nWeek) % 7;

            //나머지가 0이 아닐 경우 남은 한 주를 추가
            if (nRest != 0)
                nWeekCount++;
            if (nRest < 0)
                nWeekCount--;

            ArrayList x_arr = new ArrayList();
            ArrayList y_arr = new ArrayList();

            int y_nCount = 0;

            data0 = new double[nWeekCount];
            data1 = new double[nWeekCount];
            data2 = new double[nWeekCount];

            y_arr.Add(0);
            for (int i = 1; i < nWeekCount+1; i++)
            {
                //if (i == 1)
                //{
                //    x_arr.Add(strStartDate);
                //}
                //else
                //{
                    x_arr.Add(i + "주");
                //}

                int nReactionCount = 0;
                int nFireCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

                bool bFind = false;

                foreach (KeyValuePair<MulFunctionIntrusionLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionIntrusionLog mullog = pair.Key;

                    foreach (Report.SensorReactionIntrusionLog log in arrSensorReaction)
                    {
                        DateTime strDateTime = log.Time;

                        var cultureInfo = CultureInfo.GetCultureInfo("ko-KR");
                        var dateTimeFormatInfo = DateTimeFormatInfo.GetInstance(cultureInfo);
                        int bweekNumber = cultureInfo.Calendar.GetWeekOfYear(strDateTime, dateTimeFormatInfo.CalendarWeekRule, dateTimeFormatInfo.FirstDayOfWeek);
                        int aweekNumber = cultureInfo.Calendar.GetWeekOfYear(dtStart, dateTimeFormatInfo.CalendarWeekRule, dateTimeFormatInfo.FirstDayOfWeek);

                        int nWeekend = 0;

                        if (strDateTime.Year - dtStart.Year > 0)
                            nWeekend = bweekNumber - aweekNumber + 52;
                        else
                            nWeekend = bweekNumber - aweekNumber;


                        int nZoneID = log.Param1;
                        int nReaction = mullog.ReactionCount;
                        int nMulFunction = mullog.MulFunctionCount;
                        int nFire = mullog.FireCount;
                        int nOnlyDetect = mullog.OnlyDetectCount;


                        if (i - 1 == nWeekend)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nFire = (nFire == -1) ? nFire = 0 : nFire;
                            nOnlyDetect = (nFire == -1) ? nOnlyDetect = 0 : nOnlyDetect;

                            //한달치 합을구함, 
                            nReactionCount += nReaction;
                            nFireCount += nFire;
                            nMulFunctionCount += nMulFunction;
                            nOnlyDetectCount += nOnlyDetect;
                            bFind = true;
                        }
                    }
                }
                if (bFind == true) //한달치 합을 배열에 넣음
                {
                    //처리되지않은 신호
                    int nNotCount = nReactionCount - (nFireCount + nMulFunctionCount + nOnlyDetectCount);

                    data0[y_nCount] = Math.Ceiling(Convert.ToDouble(nNotCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                    data1[y_nCount] = Math.Floor(Convert.ToDouble(nFireCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                    data2[y_nCount] = Math.Floor(Convert.ToDouble(nMulFunctionCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                }
                else
                {
                    data0[y_nCount] = 0.0;
                    data1[y_nCount] = 0.0;
                    data2[y_nCount] = 0.0;
                }
                y_nCount++;
            }

            int x_count = 0;
            labels = new string[x_arr.Count];
            foreach (string x in x_arr)
            { 
                labels[x_count] = x;
                x_count++;
            }

            m_nCurrentPage = 1;

            decimal quotient = Math.Truncate((decimal)(labels.Length / m_nViewCount));
            int remainder = labels.Length % m_nViewCount;

            m_nTotalPage = (int)quotient;
            if (remainder > 0)
                m_nTotalPage++;

            cboPageIndex.Items.Clear();
            for (int i = 1; i <= m_nTotalPage; i++)
            {
                cboPageIndex.Items.Add(i);
            }

            cboPageIndex.SelectedIndex = cboPageIndex.Items.Count - 1;
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
            lblTotalPage.Location = new Point(panel1.Location.X + panel1.Width - lblTotalPage.Width, panel1.Location.Y + panel1.Height + 5); 
        }

        private void SetMonthChart()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            DateTime dtNowDate = DateTime.Now;
            DateTime dtBeforeDate = DateTime.Now;

            //현재날짜
            DateTime dt = DateTime.Now;

            DateTime Old = dt.AddMonths(-1);
            string str = Old.DayOfWeek.ToString();


            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;

            if (strStartDate == "")
            {
                dtStart = DateTime.Now.AddMonths(-6);
                dtEnd = DateTime.Now;
            }
            else
            {
                dtStart = DateTime.ParseExact(strStartDate, "yyyy-MM-dd", null);
                dtEnd = DateTime.ParseExact(strEndDate, "yyyy-MM-dd", null);
                //표기할땐 1일을 빼서 표기
                //dtEnd = dtEnd.AddDays(-1);
            }
            

            //두 날짜의 달 차이 계산
            int n_ts = 12 * (dtEnd.Year - dtStart.Year) + (dtEnd.Month - dtStart.Month);

            ArrayList x_arr = new ArrayList();

            int y_nCount = 0;

            data0 = new double[n_ts + 1];
            data1 = new double[n_ts + 1];
            data2 = new double[n_ts + 1];

            for (int i = 0; i < n_ts + 1; i++)
            {
                x_arr.Add((dtStart.AddMonths(i).ToString().Substring(0, 7)));

                int nReactionCount = 0;
                int nFireCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

                bool bFind = false;

                foreach (KeyValuePair<MulFunctionIntrusionLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionIntrusionLog mullog = pair.Key;

                    foreach (Report.SensorReactionIntrusionLog log in arrSensorReaction)
                    {
                        string nMonth = log.Time.ToShortDateString().Substring(0,7);
                        int nZoneID = log.Param1;
                        int nReaction = mullog.ReactionCount;
                        int nMulFunction = mullog.MulFunctionCount;
                        int nFire = mullog.FireCount;
                        int nOnlyDetect = mullog.OnlyDetectCount;

                        //dt.AddDays(-i).ToString().Substring(0, 10) == dtCmp.ToString().Substring(0, 10)
                        string test = dtStart.AddMonths(i).ToString().Substring(0, 7);
                        if (test == nMonth)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nFire = (nFire == -1) ? nFire = 0 : nFire;
                            nOnlyDetect = (nOnlyDetect == -1) ? nOnlyDetect = 0 : nOnlyDetect;

                            //한달치 합을구함, 
                            nReactionCount += nReaction;
                            nFireCount += nFire;
                            nMulFunctionCount += nMulFunction;
                            nOnlyDetectCount += nOnlyDetect;
                            bFind = true;
                        }
                        if (bFind == true)
                            break;
                    }
                }
                if (bFind == true) //한달치 합을 배열에 넣음
                {
                    //처리되지않은 신호
                    int nNotCount = nReactionCount - (nFireCount + nMulFunctionCount + nOnlyDetectCount);

                    data0[y_nCount] = Math.Ceiling(Convert.ToDouble(nNotCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                    data1[y_nCount] = Math.Floor(Convert.ToDouble(nFireCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                    data2[y_nCount] = Math.Floor(Convert.ToDouble(nMulFunctionCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                }
                else
                {
                    data0[y_nCount] = 0.0;
                    data1[y_nCount] = 0.0;
                    data2[y_nCount] = 0.0;
                }
                y_nCount++;
            }

            labels = new string[n_ts + 1];
            int x_count = 0;
            foreach (string x in x_arr)
            {
                labels[x_count] = x;
                x_count++;
            }

            m_nCurrentPage = 1;

            decimal quotient = Math.Truncate((decimal)(labels.Length / m_nViewCount));
            int remainder = labels.Length % m_nViewCount;

            m_nTotalPage = (int)quotient;
            if (remainder > 0)
                m_nTotalPage++;

            cboPageIndex.Items.Clear();
            for (int i = 1; i <= m_nTotalPage; i++)
            {
                cboPageIndex.Items.Add(i);
            }

            cboPageIndex.SelectedIndex = cboPageIndex.Items.Count - 1;
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
            lblTotalPage.Location = new Point(panel1.Location.X + panel1.Width - lblTotalPage.Width, panel1.Location.Y + panel1.Height + 5); 
		}

        private void SetQuarterChart()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            DateTime dtNowDate = DateTime.Now;
            DateTime dtBeforeDate = DateTime.Now;

            string strNowDate = "";
            string strBeforeDate = "";

            strNowDate = string.Format("{0} {1}:{2}:{3}", strEndDate.ToString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", strStartDate.ToString(), 00, 00, 00);

            DateTime dtStart = DateTime.ParseExact(strStartDate, "yyyy-MM-dd", null);
            DateTime dtEnd = DateTime.ParseExact(strEndDate, "yyyy-MM-dd", null);

            int n_ArrayStart = 0;
            if (dtStart.Month >= 1 && dtStart.Month <= 3)
                n_ArrayStart = 1;
            else if (dtStart.Month >= 4 && dtStart.Month <= 6)
                n_ArrayStart = 2;
            else if (dtStart.Month >= 7 && dtStart.Month <= 9)
                n_ArrayStart = 3;
            else if (dtStart.Month >= 10 && dtStart.Month <= 12)
                n_ArrayStart = 4;

            //두 날짜의 년 차이 계산
            int n_ts = dtEnd.Year - dtStart.Year;
            int n_arrayCount = n_ts;

            if (n_arrayCount == 0)
                n_arrayCount = 4;
            else
                n_arrayCount = ((n_ts + 1) * 4);

            n_arrayCount = n_arrayCount - (n_ArrayStart - 1);


            //년도까지 비교해주기위해(몇년도 몇주기인지)
            int nYear = dtStart.Year;
            ArrayList x_arr = new ArrayList();
            int y_nCount = 0;

            data0 = new double[n_arrayCount];
            data1 = new double[n_arrayCount];
            data2 = new double[n_arrayCount];

            int nCount = 0;
            for (int i = n_ArrayStart; nCount < n_arrayCount ; i++, nCount++)
            {
                if (i % 4 == 0)
                    i = 4;
                else
                {
                    if(i == 5)
                        nYear++;

                    i = i % 4;
                }

                if (n_ts == 0)
                    x_arr.Add(String.Format("{0}분기", i));
                else
                    x_arr.Add(String.Format("{0}년도 {1}분기", nYear, i));
                    

                int nReactionCount = 0;
                int nFireCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

                bool bFind = false;

                foreach (KeyValuePair<MulFunctionIntrusionLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    int QuarterNumber = 0;
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionIntrusionLog mullog = pair.Key;

                    foreach (Report.SensorReactionIntrusionLog log in arrSensorReaction)
                    {
                        DateTime strDateTime = log.Time;

                        if (strDateTime.Month <= 3)
                            QuarterNumber = 1;
                        else if (strDateTime.Month >= 4 && strDateTime.Month <= 6)
                            QuarterNumber = 2;
                        else if (strDateTime.Month >= 7 && strDateTime.Month <= 9)
                            QuarterNumber = 3;
                        else if (strDateTime.Month >= 10 && strDateTime.Month <= 12)
                            QuarterNumber = 4;

                        int nYears = strDateTime.Year;
                        int nQuarter = QuarterNumber;
                        int nZoneID = log.Param1;
                        int nReaction = mullog.ReactionCount;
                        int nMulFunction = mullog.MulFunctionCount;
                        int nFire = mullog.FireCount;
                        int nOnlyDetect = mullog.OnlyDetectCount;

                        if (nYear == nYears && i == nQuarter)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nFire = (nFire == -1) ? nFire = 0 : nFire;
                            nOnlyDetect = (nFire == -1) ? nOnlyDetect = 0 : nOnlyDetect;

                            //한달치 합을구함, 
                            nReactionCount += nReaction;
                            nFireCount += nFire;
                            nMulFunctionCount += nMulFunction;
                            nOnlyDetectCount += nOnlyDetect;
                            bFind = true;
                        }
                        if (bFind == true)
                            break;
                    }
                }
                if (bFind == true) //한달치 합을 배열에 넣음
                {
                    //처리되지않은 신호
                    int nNotCount = nReactionCount - (nFireCount + nMulFunctionCount + nOnlyDetectCount);

                    data0[y_nCount] = Math.Ceiling(Convert.ToDouble(nNotCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                    data1[y_nCount] = Math.Floor(Convert.ToDouble(nFireCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                    data2[y_nCount] = Math.Floor(Convert.ToDouble(nMulFunctionCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                }
                else
                {
                    data0[y_nCount] = 0.0;
                    data1[y_nCount] = 0.0;
                    data2[y_nCount] = 0.0;
                }
                y_nCount++;
            }
            int x_count = 0;
            labels = new string[x_arr.Count];
            foreach (string x in x_arr)
            {
                labels[x_count] = x;
                x_count++;
            }

            m_nCurrentPage = 1;

            decimal quotient = Math.Truncate((decimal)(labels.Length / m_nViewCount));
            int remainder = labels.Length % m_nViewCount;

            m_nTotalPage = (int)quotient;
            if (remainder > 0)
                m_nTotalPage++;

            cboPageIndex.Items.Clear();
            for (int i = 1; i <= m_nTotalPage; i++)
            {
                cboPageIndex.Items.Add(i);
            }

            cboPageIndex.SelectedIndex = cboPageIndex.Items.Count - 1;
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
            lblTotalPage.Location = new Point(panel1.Location.X + panel1.Width - lblTotalPage.Width, panel1.Location.Y + panel1.Height + 5); 
        }

        private void SetYearChart()
        {
            WebDBManager dbMgr = FormMain.Instance.DBManager;

            DateTime dtNowDate = DateTime.Now;
            DateTime dtBeforeDate = DateTime.Now;

            //XYChart c = new XYChart(dataGridView1.Size.Width, 280);

            string strNowDate = "";
            string strBeforeDate = "";

            strNowDate = string.Format("{0} {1}:{2}:{3}", strEndDate.ToString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", strStartDate.ToString(), 00, 00, 00);

 
            //현재날짜
            DateTime dt = DateTime.Now;
            DateTime dtStart = DateTime.ParseExact(strStartDate, "yyyy-MM-dd", null);
            DateTime dtEnd = DateTime.ParseExact(strEndDate, "yyyy-MM-dd", null);


            //두 년도의 달 차이 계산
            int n_ts = dtEnd.Year - dtStart.Year;

            ArrayList x_arr = new ArrayList();

            int y_nCount = 0;

            data0 = new double[n_ts + 1];
            data1 = new double[n_ts + 1];
            data2 = new double[n_ts + 1];


            for (int i = 0; i < n_ts + 1; i++)
            {
                x_arr.Add(String.Format("{0}년도", (dtStart.AddYears(i).ToString().Substring(0, 4))));
                //arr.Add(dt.AddDays(-i).ToString());
                int nReactionCount = 0;
                int nFireCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

                bool bFind = false;
                foreach (KeyValuePair<MulFunctionIntrusionLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionIntrusionLog mullog = pair.Key;

                    foreach (Report.SensorReactionIntrusionLog log in arrSensorReaction)
                    {
                        string nYear = log.Time.ToShortDateString().Substring(0, 4);
                        int nZoneID = log.Param1;
                        int nReaction = mullog.ReactionCount;
                        int nMulFunction = mullog.MulFunctionCount;
                        int nFire = mullog.FireCount;
                        int nOnlyDetect = mullog.OnlyDetectCount;

                        //dt.AddDays(-i).ToString().Substring(0, 10) == dtCmp.ToString().Substring(0, 10)
                        string test = dtStart.AddYears(i).ToString().Substring(0, 4);
                        if (test == nYear)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nFire = (nFire == -1) ? nFire = 0 : nFire;
                            nOnlyDetect = (nOnlyDetect == -1) ? nOnlyDetect = 0 : nOnlyDetect;

                            //1년치 합을구함, 
                            nReactionCount += nReaction;
                            nFireCount += nFire;
                            nMulFunctionCount += nMulFunction;
                            nOnlyDetectCount += nOnlyDetect;
                            bFind = true;
                        }
                        if (bFind == true)
                            break;
                    }
                }

                if (bFind == true) //1년치 합을 배열에 넣음
                {
                    //처리되지않은 신호
                    int nNotCount = nReactionCount - (nFireCount + nMulFunctionCount + nOnlyDetectCount);

                    data0[y_nCount] = Math.Ceiling(Convert.ToDouble(nNotCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                    data1[y_nCount] = Math.Floor(Convert.ToDouble(nFireCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                    data2[y_nCount] = Math.Floor(Convert.ToDouble(nMulFunctionCount * 100) / (nNotCount + nFireCount + nMulFunctionCount + nOnlyDetectCount));
                }
                else
                {
                    data0[y_nCount] = 0.0;
                    data1[y_nCount] = 0.0;
                    data2[y_nCount] = 0.0;
                }
                y_nCount++;
            }

            labels = new string[n_ts + 1];
            int x_count = 0;
            foreach (string x in x_arr)
            {
                labels[x_count] = x;
                x_count++;
            }

            m_nCurrentPage = 1;

            decimal quotient = Math.Truncate((decimal)(labels.Length / m_nViewCount));
            int remainder = labels.Length % m_nViewCount;

            m_nTotalPage = (int)quotient;
            if (remainder > 0)
                m_nTotalPage++;

            cboPageIndex.Items.Clear();
            for (int i = 1; i <= m_nTotalPage; i++)
            {
                cboPageIndex.Items.Add(i);
            }

            cboPageIndex.SelectedIndex = cboPageIndex.Items.Count - 1;
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
            lblTotalPage.Location = new Point(panel1.Location.X + panel1.Width - lblTotalPage.Width, panel1.Location.Y + panel1.Height + 5); 
        }

        // 1주, 2주, 3주..
        private string[] dd = null;
        //data0=0, data1=1, data2=2
        private Dictionary<int, double[]> dd2 = new Dictionary<int, double[]>();

        private void abc()
        {
            dd = null;
            dd2.Clear();

            if (labels.Length > m_nViewCount)
            {
                dd = new string[m_nViewCount];
                string[] tempLabel = new string[m_nViewCount];
                double[] tempData0 = new double[m_nViewCount];
                double[] tempData1 = new double[m_nViewCount];
                double[] tempData2 = new double[m_nViewCount];
                int tempCnt = 0;

                int curPage = 1;
                for (int i = 0; i < labels.Length; i++)
                {
                    if (i > 0 && i % m_nViewCount == 0)
                        curPage++;

                    if (curPage == m_nCurrentPage)
                    {
                        tempLabel[tempCnt] = labels[i];
                        tempData0[tempCnt] = data0[i];
                        tempData1[tempCnt] = data1[i];
                        tempData2[tempCnt] = data2[i];
                        tempCnt++;
                    }
                }

                dd = tempLabel;
                dd2[0] = tempData0;
                dd2[1] = tempData1;
                dd2[2] = tempData2;
            }
            else
            {
                dd = new string[labels.Length];
                dd = labels;

                dd2[0] = data0;
                dd2[1] = data1;
                dd2[2] = data2;
            }
        }

        private Brush brushBg = new SolidBrush(Color.FromArgb(0xff, 0x28, 0x28, 0x28));
        private Brush brushOrange = new SolidBrush(Color.FromArgb(0xff, 0xdd, 0x85, 0x09));
        private Brush brushRed = new SolidBrush(Color.FromArgb(0xff, 0xdc, 0x00, 0x00));
        private Brush brushBlue = new SolidBrush(Color.FromArgb(0xff, 0x0e, 0x8b, 0xe1));
        private Brush brushGray = new SolidBrush(Color.FromArgb(0xff, 0xd1, 0xd0, 0xce));
        private Pen mPenRed = new Pen(Color.Red);
        private Pen mPenBlack = new Pen(Color.Black);
        private Pen mPenGray = new Pen(Color.Gray);

        private void panelChart_Paint(object sender, PaintEventArgs e)
        {
            abc();

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            float sizePer = 1.0f;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
                sizePer = 0.5f;
            else if (FormMain.Instance.Resolution == Resolution.Other)
                sizePer = 0.75f;

            Font font = new System.Drawing.Font(Program.prgFont, 20F * sizePer, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

            int nBigRectSize = (int)(258 * sizePer);
            int nMediumRectSize = (int)(166 * sizePer);

            int nTopEmpty = (int)(80 * sizePer);
            int nEmpty = (int)(16 * sizePer);
            int nSpace = (int)(16 * sizePer); // 한개 한개 간격

            Point beginPT = new Point(0, 0);
            Point drawPT = beginPT;

            Size RectSize = new System.Drawing.Size((int)(840 * sizePer), (int)(360 * sizePer));
            Size PanelSize = new System.Drawing.Size();

            int nRectCount = 1;

            for (int i = 0; i < dd.Length; i++)
            {
                if (nRectCount == 9)
                    break;

                if (dd[i] == null)
                    continue;

                Rectangle RectRed1 = new Rectangle(drawPT.X + nEmpty, drawPT.Y + nTopEmpty, nBigRectSize, nBigRectSize);
                Rectangle RectGray1 = new Rectangle(drawPT.X + nEmpty, drawPT.Y + nTopEmpty, nBigRectSize, nBigRectSize);
                Rectangle RectSmall1 = new Rectangle((int)(RectRed1.Width * 0.5 - nMediumRectSize * 0.5) + RectRed1.X, (int)(RectRed1.Width * 0.5 - nMediumRectSize * 0.5) + RectRed1.Y, nMediumRectSize, nMediumRectSize);

                Rectangle RectRed2 = new Rectangle(drawPT.X + nEmpty + RectRed1.Width + nEmpty, drawPT.Y + nTopEmpty, nBigRectSize, nBigRectSize);
                Rectangle RectGray2 = new Rectangle(drawPT.X + nEmpty + RectRed1.Width + nEmpty, drawPT.Y + nTopEmpty, nBigRectSize, nBigRectSize);
                Rectangle RectSmall2 = new Rectangle((int)(RectRed2.Width * 0.5 - nMediumRectSize * 0.5) + RectRed2.X, (int)(RectRed1.Width * 0.5 - nMediumRectSize * 0.5) + RectRed1.Y, nMediumRectSize, nMediumRectSize);

                Rectangle RectRed3 = new Rectangle(drawPT.X + nEmpty + RectRed1.Width + nEmpty + RectRed2.Width + nEmpty, drawPT.Y + nTopEmpty, nBigRectSize, nBigRectSize);
                Rectangle RectGray3 = new Rectangle(drawPT.X + nEmpty + RectRed1.Width + nEmpty + RectRed2.Width + nEmpty, drawPT.Y + nTopEmpty, nBigRectSize, nBigRectSize);
                Rectangle RectSmall3 = new Rectangle((int)(RectRed3.Width * 0.5 - nMediumRectSize * 0.5) + RectRed3.X, (int)(RectRed1.Width * 0.5 - nMediumRectSize * 0.5) + RectRed1.Y, nMediumRectSize, nMediumRectSize);

                float value0 = (float)dd2[0][i] / 100.0f * 360.0f;
                float value1 = (float)dd2[1][i] / 100.0f * 360.0f;
                float value2 = (float)dd2[2][i] / 100.0f * 360.0f;

                SizeF size = g.MeasureString(dd2[0][i] + "%", font);
                g.FillRectangle(brushBg, new Rectangle(drawPT.X, drawPT.Y, RectSize.Width, RectSize.Height));
                g.FillPie(brushGray, RectGray1, 0.0f, 360.0f);
                g.FillPie(brushOrange, RectRed1, -90.0f, value0);
                g.FillPie(brushBg, RectSmall1, 0.0f, 360.0f);
                g.DrawString(dd2[0][i] + "%", font, brushGray, RectGray1.X + RectGray1.Width - (RectGray1.Width / 2) - (size.Width / 2), RectGray1.Y + RectGray1.Height - (int)(RectGray1.Height * 0.5) - 8);

                size = g.MeasureString(dd2[1][i] + "%", font);
                g.FillPie(brushGray, RectGray2, 0.0f, 360.0f);
                g.FillPie(brushRed, RectRed2, -90.0f, value1);
                g.FillPie(brushBg, RectSmall2, 0.0f, 360.0f);
                g.DrawString(dd2[1][i] + "%", font, brushGray, RectGray2.X + RectGray2.Width - (RectGray2.Width / 2) - (size.Width / 2), RectGray2.Y + RectGray2.Height - (int)(RectGray2.Height * 0.5) - 8);

                size = g.MeasureString(dd2[2][i] + "%", font);
                g.FillPie(brushGray, RectGray3, 0.0f, 360.0f);
                g.FillPie(brushBlue, RectRed3, -90.0f, value2);
                g.FillPie(brushBg, RectSmall3, 0.0f, 360.0f);
                g.DrawString(dd2[2][i] + "%", font, brushGray, RectGray3.X + RectGray3.Width - (RectGray3.Width / 2) - (size.Width / 2), RectGray3.Y + RectGray3.Height - (int)(RectGray3.Height * 0.5) - 8);

                size = g.MeasureString(dd[i], font);
                g.DrawString(dd[i], font, brushGray, drawPT.X + RectSize.Width - (RectSize.Width / 2) - (size.Width / 2), drawPT.Y + 15);

                if (drawPT.X + RectSize.Width > PanelSize.Width)
                    PanelSize.Width = drawPT.X + RectSize.Width;
                if (drawPT.Y + RectSize.Height > PanelSize.Height)
                    PanelSize.Height = drawPT.Y + RectSize.Height;

                if (nRectCount % 4 == 0)
                {
                    drawPT = new Point(beginPT.X, drawPT.Y + RectSize.Height + nSpace);
                }
                else
                    drawPT = new Point(drawPT.X + RectSize.Width + nSpace, drawPT.Y);

                nRectCount++;
            }
            //panelChart.Size = PanelSize;
        }  

        private void SetChartBar()
        {
            int nSpace = 25;
            // Create a XYChart object of size 250 x 250 pixels
            XYChart c = new XYChart(dataGridView1.Size.Width - cboChart.Width - nSpace, 290);

            LegendBox legendBox = c.addLegend(c.getWidth() - 10, 90, true, "Arial", 10);
            legendBox.setAlignment(Chart.TopRight);

            c.xAxis().setLabels(labels);
            c.xAxis().setLabelStyle().setPos(c.xAxis().getX(), c.xAxis().getY() + 10);
            c.yAxis().setTitle("백분율(%)");
            c.yAxis().setDateScale(0, 100);

            c.setPlotArea(60, 20, dataGridView1.Size.Width - cboChart.Width - nSpace * 11, 230);

            c.xAxis().setTickOffset(0.1);

            BarLayer layer = c.addBarLayer2(Chart.Side);

            layer.addDataSet(data0, 0x6B9900, "현장에서 꺼진 신호");
            layer.addDataSet(data1, 0xff0000, "방범 발생");
            layer.addDataSet(data2, 0x00D8FF, "오작동");

            layer.setBarWidth(80); // Bar의 두께 설정
            layer.setDataLabelStyle().setFontColor(Chart.Transparent);
            layer.setAggregateLabelFormat("{value}%");
            layer.setAggregateLabelStyle("Arial Bold", 11.25, 0x000000, 0).setAlignment(Chart.Center);

            // Output the chart
            c_PercentBarChart.Chart = c;
        }

        private void cboChart_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (m_bCheckCombo == true)
                return;

            if (cboChart.SelectedIndex == 0) //주별 
                SetWeekChart();
            else if (cboChart.SelectedIndex == 1) //월별 
                SetMonthChart();
            else if (cboChart.SelectedIndex == 2)//분기 
                SetQuarterChart();
            else if (cboChart.SelectedIndex == 3)//년 
                SetYearChart();

            panelChart.Invalidate();

            //SetChartBar();
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

        public void SetVisibleHWPExport(bool visible)
        {
            btnSaveHWP.Visible = visible;
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
 
        //public void Search()
        //{
        //    dataGridView1.Rows.Clear();
        //    if (group.GroupID > 0)
        //    {
        //        ArrayList arrBuildings = group.BuildingList;
        //        if (arrBuildings == null)
        //            return;
        //        foreach (Building building in arrBuildings)
        //        {
        //            ArrayList arrFloors = building.FloorList;

        //            if (arrFloors != null && arrFloors.Count > 0)
        //            {
        //                // Zone이 하나도 없는 빌딩, 즉 도면이 하나도 없는 빌딩은 콤보박스에 보여주지 않는다.
        //                dataGridView1Add(building);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (KeyValuePair<int, Zone> pair in ZoneManager.Instance.DicOutdoorZones)
        //        {
        //            dataGridView1.Items.Add(pair.Value);
        //        }
        //    }

        //}

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

            m_manualManager.SetID(this, "SDMS_Report_Process_Security");
            m_manualManager.SetID(label2, "SDMS_Report_Process_Security");
            m_manualManager.SetID(btnSaveHWP, "Process_Security_ExportReport");
            m_manualManager.SetID(panelChart, "Process_Security_Graph");
            m_manualManager.SetID(btnPreviousIndex, "Process_Security_Graph");
            m_manualManager.SetID(btnNextIndex, "Process_Security_Graph");
            m_manualManager.SetID(lblTotalPage, "Process_Security_Graph");
            m_manualManager.SetID(dataGridView1, "Process_Security_Grid");

            m_manualManager.ProcessEvent();
        }
    }
 }

