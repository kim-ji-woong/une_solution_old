using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChartDirector;
using DBUtility;
using System.Collections;
using SDMS.Report;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Globalization;

namespace SDMS
{
    public partial class NotOperationPage : Form
    {
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
        private int storage = 0;
        private ArrayList SaveArr = new ArrayList();

        private ReportData m_ReportData = new ReportData();
        internal ReportData ReportData
        {
            get { return m_ReportData; }
            set { m_ReportData = value; }
        }

        private static NotOperationPage m_instance = null;
        public static NotOperationPage Instance
        {
            get { return NotOperationPage.m_instance; }
            set { NotOperationPage.m_instance = value; }
        }

        private Report.ReactionManager m_detectMgr = null;

        public NotOperationPage(Report.ReactionManager detectMgr)
        {
            m_instance = this;
            InitializeComponent();

            m_detectMgr = detectMgr;
            m_hwpCtrl = new HwpCtrlData();
            
            c_PercentBarChart = winChartViewer1;

            setComboBox();

            SetupDataGrid();
        }
		
		private void NotOperationPage_Load(object sender, EventArgs e)
		{
            //setComboBox();

            //SetupDataGrid();
            //Load_DataGrid();
            //createBarChart(winChartViewer1);

            //최근6개월
            DateTime startDate = DateTime.Now.AddMonths(-6);
            DateTime EndDate = DateTime.Now;

            m_SelectedMinDate = startDate;
            m_SelectedMaxDate = EndDate;
            m_arrSelectedZone = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");


            //찾은 검색결과를 DataGrid로 출력
            Load_DataGrid(true);
            createBarChart(winChartViewer1);
		}

        public void SetHwpData()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveDateTime.txt"))
            {
                file.WriteLine(lblMinDate.Text+lblMaxDate.Text);
                file.WriteLine(lblBuilding.Text);
                file.Close();
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

            Bitmap bmp = new Bitmap(this.winChartViewer1.Width, this.winChartViewer1.Height);
            this.winChartViewer1.DrawToBitmap(bmp, new Rectangle(0, 0, this.winChartViewer1.Width, this.winChartViewer1.Height));
            bmp.Save(Application.StartupPath + "\\report\\Malfunction.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);


        private void setComboBox()
        {
            cboChart.Items.Add("주별 보기");
            cboChart.Items.Add("월별 보기");
            cboChart.Items.Add("분기별 보기");
            cboChart.Items.Add("연도별 보기");
            cboChart.SelectedIndex = 1;
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

        public void createBarChart(WinChartViewer viewer)
        {
            if (cboChart.SelectedIndex == 0)
                SetWeekChart();
            else if (cboChart.SelectedIndex == 1)
                SetMonthChart();
            else if (cboChart.SelectedIndex == 2)
                SetQuarterChart();
            else if (cboChart.SelectedIndex == 3)
                SetYearChart();

            SetChartBar();
        }

        private void SetupDataGrid()
        {
            this.Controls.Add(dataGridView1);

            dataGridView1.ColumnCount = 11;

            dataGridView1.Columns[0].Name = "No";
            dataGridView1.Columns[0].Width = 60;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            dataGridView1.Columns[1].Name = "유형";
            dataGridView1.Columns[1].Width = 40;
            //dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

           // dataGridView1.Columns[2].Name = "관리ID";
           // dataGridView1.Columns[2].Width = 50;

            dataGridView1.Columns[2].Name = "건물 그룹";
            dataGridView1.Columns[2].Width = 70;

            dataGridView1.Columns[3].Name = "건물";
            dataGridView1.Columns[3].Width = 100;
            dataGridView1.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dataGridView1.Columns[4].Name = "층";
            dataGridView1.Columns[4].Width = 40;

            dataGridView1.Columns[5].Name = "탐지";
            dataGridView1.Columns[5].Width = 30;

            dataGridView1.Columns[6].Name = "화재";
            dataGridView1.Columns[6].Width = 30;

            dataGridView1.Columns[7].Name = "오작동";
            dataGridView1.Columns[7].Width = 30;

            dataGridView1.Columns[8].Name = "처리되지 않음";
            dataGridView1.Columns[8].Width = 30;

            dataGridView1.Columns[9].Name = "오작동률";
            dataGridView1.Columns[9].Width = 30;

            dataGridView1.Columns[10].Name = "담당자";
            dataGridView1.Columns[10].Width = 70;

            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            for (int i = 0; i < dataGridView1.ColumnCount; i++)
            {
                dataGridView1.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        public void Load_DataGrid(bool isLoad = false)
        {
            SaveArr.Clear();
            //lblNullResult.Visible = false;
            dataGridView1.Rows.Clear();

            //string -> DateTime
            string strNowDate = "";
            string strBeforeDate = "";

            strNowDate = string.Format("{0} {1}:{2}:{3}", strEndDate.ToString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", strStartDate.ToString(), 00, 00, 00);

            int count = 0;
            int nCount = 1;

            int nHwpTable = 14;
            int k = 0;

            foreach (MulFunctionLog mulFunctionLog in m_detectMgr.MulFunctionList)
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
                ArrayList arEquipzone = ZoneManager.Instance.GetEquipmentZoneList(zone);
                if (arEquipzone != null && arEquipzone.Count > 0)
                {
                    equipZone = (EquipmentZone)arEquipzone[0];
                }

                FacilityManagerGroup ManagerGroup = null;

                ManagerGroup = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(Facility.FacilityType.FIRE_SENSOR, equipZone);


                if (ManagerGroup == null)
                    ManagerGroup = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(Facility.FacilityType.FIRE_SENSOR, equipZone.LinkedZone.Building);

                if (ManagerGroup == null)
                    ManagerGroup = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(Facility.FacilityType.FIRE_SENSOR);

                strManagerName = FormMain.Instance.DataManager.GetFacilityManagerName(ManagerGroup, ref strPhoneNumber);

                string[] rows = { "", strType, strBuildingGroupName, strBuildingName, strFloorName, nReactionCount.ToString(), nFireCount.ToString(), nMulFunctionCount.ToString(), nNotProcss.ToString(), nPercentMulFunction.ToString() + "%", strManagerName };
                dataGridView1.Rows.Add(rows);
                dataGridView1.Rows[count].Cells[0].Value = nCount;



                int HwpIndex = 0;

                for (k = nHwpTable; k < nHwpTable + 11; k++)
                {
                    SaveArr.Add(dataGridView1.Rows[count].Cells[HwpIndex].Value.ToString());


                    
                    HwpIndex++;
                }
                nHwpTable += 11;

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
            lblMinDate.Text = strdtMin + "부터 ";
            lblMaxDate.Text = strdtMax + "까지";

            //원래있던 표의 줄 수를 저장함
            storage = dataGridView1.Rows.Count;
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

        private void NotOperationPage_Resize(object sender, EventArgs e)
        {
            SetChartBar();
            Point ptGrid = dataGridView1.Location;
            Size SizeGrid = dataGridView1.Size;
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

            DateTime dtStart = DateTime.ParseExact(strStartDate, "yyyy-MM-dd", null);
            DateTime dtEnd = DateTime.ParseExact(strEndDate, "yyyy-MM-dd", null);

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

                bool bFind = false;

                foreach (KeyValuePair<MulFunctionLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionLog mullog = pair.Key;

                    foreach (Report.SensorReactionLog log in arrSensorReaction)
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


                        if (i - 1 == nWeekend)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nFire = (nFire == -1) ? nFire = 0 : nFire;

                            //한달치 합을구함, 
                            nReactionCount += nReaction;
                            nFireCount += nFire;
                            nMulFunctionCount += nMulFunction;
                            bFind = true;
                        }
                    }
                }
                if (bFind == true) //한달치 합을 배열에 넣음
                {
                    //처리되지않은 신호
                    int nNotCount = nReactionCount - (nFireCount + nMulFunctionCount);

                    data0[y_nCount] = nNotCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);
                    data1[y_nCount] = nFireCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);
                    data2[y_nCount] = nMulFunctionCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);

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

                bool bFind = false;

                foreach (KeyValuePair<MulFunctionLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionLog mullog = pair.Key;

                    foreach(Report.SensorReactionLog log in arrSensorReaction)
                    {
                        string nMonth = log.Time.ToShortDateString().Substring(0,7);
                        int nZoneID = log.Param1;
                        int nReaction = mullog.ReactionCount;
                        int nMulFunction = mullog.MulFunctionCount;
                        int nFire = mullog.FireCount;

                        //dt.AddDays(-i).ToString().Substring(0, 10) == dtCmp.ToString().Substring(0, 10)
                        string test = dtStart.AddMonths(i).ToString().Substring(0, 7);
                        if (test == nMonth)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nFire = (nFire == -1) ? nFire = 0 : nFire;

                            //한달치 합을구함, 
                            nReactionCount += nReaction;
                            nFireCount += nFire;
                            nMulFunctionCount += nMulFunction;
                            bFind = true;
                        }
                        if (bFind == true)
                            break;
                    }
                }
                if (bFind == true) //한달치 합을 배열에 넣음
                {
                    //처리되지않은 신호
                    int nNotCount = nReactionCount - (nFireCount + nMulFunctionCount);

                    data0[y_nCount] = nNotCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);
                    data1[y_nCount] = nFireCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);
                    data2[y_nCount] = nMulFunctionCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);

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

                x_arr.Add(i + "분기");
                    

                int nReactionCount = 0;
                int nFireCount = 0;
                int nMulFunctionCount = 0;

                bool bFind = false;

                foreach (KeyValuePair<MulFunctionLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    int QuarterNumber = 0;
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionLog mullog = pair.Key;

                    foreach (Report.SensorReactionLog log in arrSensorReaction)
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

                        if (nYear == nYears && i == nQuarter)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nFire = (nFire == -1) ? nFire = 0 : nFire;

                            //한달치 합을구함, 
                            nReactionCount += nReaction;
                            nFireCount += nFire;
                            nMulFunctionCount += nMulFunction;
                            bFind = true;
                        }
                        if (bFind == true)
                            break;
                    }
                }
                if (bFind == true) //한달치 합을 배열에 넣음
                {
                    //처리되지않은 신호
                    int nNotCount = nReactionCount - (nFireCount + nMulFunctionCount);

                    data0[y_nCount] = nNotCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);
                    data1[y_nCount] = nFireCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);
                    data2[y_nCount] = nMulFunctionCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);

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
                x_arr.Add((dtStart.AddYears(i).ToString().Substring(0, 4)));
                //arr.Add(dt.AddDays(-i).ToString());
                int nReactionCount = 0;
                int nFireCount = 0;
                int nMulFunctionCount = 0;

                bool bFind = false;
                foreach (KeyValuePair<MulFunctionLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionLog mullog = pair.Key;

                    foreach (Report.SensorReactionLog log in arrSensorReaction)
                    {
                        string nYear = log.Time.ToShortDateString().Substring(0, 4);
                        int nZoneID = log.Param1;
                        int nReaction = mullog.ReactionCount;
                        int nMulFunction = mullog.MulFunctionCount;
                        int nFire = mullog.FireCount;

                        //dt.AddDays(-i).ToString().Substring(0, 10) == dtCmp.ToString().Substring(0, 10)
                        string test = dtStart.AddYears(i).ToString().Substring(0, 4);
                        if (test == nYear)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nFire = (nFire == -1) ? nFire = 0 : nFire;

                            //1년치 합을구함, 
                            nReactionCount += nReaction;
                            nFireCount += nFire;
                            nMulFunctionCount += nMulFunction;
                            bFind = true;
                        }
                        if (bFind == true)
                            break;
                    }
                }

                if (bFind == true) //1년치 합을 배열에 넣음
                {
                    //처리되지않은 신호
                    int nNotCount = nReactionCount - (nFireCount + nMulFunctionCount);

                    data0[y_nCount] = nNotCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);
                    data1[y_nCount] = nFireCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);
                    data2[y_nCount] = nMulFunctionCount * 100 / (nNotCount + nFireCount + nMulFunctionCount);

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

        }


        private void SetChartBar()
        {
            int nSpace = 25;
            // Create a XYChart object of size 250 x 250 pixels
            XYChart c = new XYChart(dataGridView1.Size.Width - cboChart.Width - nSpace, 280);


            LegendBox legendBox = c.addLegend(c.getWidth()-10, 90, true, "Arial", 10);
            legendBox.setAlignment(Chart.TopRight);

            c.xAxis().setLabels(labels);
            c.yAxis().setTitle("백분율(%)");

            // Set the plotarea at (30, 20) and of size 200 x 200 pixels
            c.setPlotArea(60, 20, dataGridView1.Size.Width - cboChart.Width - nSpace * 11, 200);
            BarLayer layer = c.addBarLayer2(Chart.Stack);

            layer.addDataSet(data0, 0x6B9900, "처리되지 않은 신호");
            layer.addDataSet(data1, 0xff0000, "화재 발생");
            layer.addDataSet(data2, 0x00D8FF, "오작동");

            layer.setDataLabelStyle().setAlignment(Chart.Center);
            // Output the chart
            c_PercentBarChart.Chart = c;
        }

        private void cboChart_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cboChart.SelectedIndex == 0) //주별
            {
                SetWeekChart();
            }
            else if (cboChart.SelectedIndex == 1) //월별
            {
                SetMonthChart();
            }
            else if (cboChart.SelectedIndex == 2)//분기
            {
                SetQuarterChart();
            }
            else if(cboChart.SelectedIndex == 3)//년
            {
                SetYearChart();
            }

            SetChartBar();
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
    }
 }

