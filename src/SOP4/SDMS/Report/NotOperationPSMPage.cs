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
using System.Reflection;
using UnE.Spatial;
using UnE.Sensor;
using UnE.Util.Unity;

namespace SDMS
{
    public partial class NotOperationPSMPage : Form
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

        private string strbuilding = "";

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

        private Report.ReactionPSMManager m_detectMgr = null;
        private Report.ReactionPSMManager.RefreshCheckData m_checkData = new ReactionPSMManager.RefreshCheckData();

        public Report.ReactionPSMManager.RefreshCheckData RefreshCheckData
        {
            get { return m_checkData; }
        }

        private bool m_bCheckCombo = false;
        public NotOperationPSMPage(Report.ReactionPSMManager detectMgr)
        {
            this.DoubleBuffered = true;

            InitializeComponent();

            Type dgvType1 = this.gvMain.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(gvMain, true, null);

            m_detectMgr = detectMgr;
            m_hwpCtrl = new HwpCtrlData();
            
            c_PercentBarChart = winChartViewer1;

            m_bCheckCombo = true;
            SetComboBox();
            m_bCheckCombo = false;
        }
		
		private void NotOperationPSMPage_Load(object sender, EventArgs e)
		{
            SetupDataGrid();

            //최근6개월
            DateTime startDate, EndDate;

            if (!FormMain.Instance.GetNotOperationPSMReportDate(out startDate, out EndDate))
                return;

            m_SelectedMinDate = startDate;
            m_SelectedMaxDate = EndDate;
            m_arrSelectedZone = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");


            FormMain.Instance.RefreshReportNotOperationPSM();

            //찾은 검색결과를 DataGrid로 출력
            //Load_DataGrid(true);
            //CreateBarChart(winChartViewer1);
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
            Bitmap bmp = new Bitmap(this.winChartViewer1.Width, this.winChartViewer1.Height);
            this.winChartViewer1.DrawToBitmap(bmp, new Rectangle(0, 0, this.winChartViewer1.Width, this.winChartViewer1.Height));
            bmp.Save(Application.StartupPath + "\\report\\Malfunction.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        //[DllImport("User32.dll")]
        //private static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);


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

        public void ComboSubmit(string building)
        {
            this.strbuilding = building;
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

            SetChartBar();
        }

        private int m_nColumnCount = 8;
        private int m_nNO_INDEX = 0;
        private int m_nMATERIAL_INDEX = 1;
        private int m_nBUILDING_INDEX = 2;
        private int m_nDETECT_LOCATION_INDEX = 3;
        private int m_nDETECT_COUNT_INDEX = 4;
        private int m_nALARM_COUNT_INDEX = 5;
        private int m_nSYSTEM_RECOVERY_COUNT_INDEX = 6;
        private int m_nEQUIPMENT_RECOVERY_COUNT_INDEX = 7;

        private void SetupDataGrid()
        {
            gvMain.Columns[m_nNO_INDEX].Width = 80;
            gvMain.Columns[m_nMATERIAL_INDEX].Width = 70;
            gvMain.Columns[m_nBUILDING_INDEX].Width = 105;
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].Width = 180;
            gvMain.Columns[m_nDETECT_COUNT_INDEX].Width = 60;
            gvMain.Columns[m_nALARM_COUNT_INDEX].Width = 60;
            gvMain.Columns[m_nSYSTEM_RECOVERY_COUNT_INDEX].Width = 60;
            gvMain.Columns[m_nEQUIPMENT_RECOVERY_COUNT_INDEX].Width = 100;
            /*this.Controls.Add(gvMain);

            gvMain.ColumnCount = m_nColumnCount;

            gvMain.Columns[m_nNO_INDEX].Name = "No";
            gvMain.Columns[m_nNO_INDEX].Width = 80;
            gvMain.Columns[m_nNO_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nNO_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            gvMain.Columns[m_nNO_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nMATERIAL_INDEX].Name = "물질";
            gvMain.Columns[m_nMATERIAL_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nMATERIAL_INDEX].Width = 70;
            gvMain.Columns[m_nMATERIAL_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nBUILDING_INDEX].Name = "건물";
            gvMain.Columns[m_nBUILDING_INDEX].Width = 105;
            gvMain.Columns[m_nBUILDING_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gvMain.Columns[m_nBUILDING_INDEX].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            gvMain.Columns[m_nBUILDING_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nDETECT_LOCATION_INDEX].Name = "누출 발생장소";
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].Width = 180;
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].DefaultCellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            gvMain.Columns[m_nDETECT_LOCATION_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nDETECT_COUNT_INDEX].Name = "탐지";
            gvMain.Columns[m_nDETECT_COUNT_INDEX].Width = 60;
            gvMain.Columns[m_nDETECT_COUNT_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nDETECT_COUNT_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nALARM_COUNT_INDEX].Name = "누출신고";
            gvMain.Columns[m_nALARM_COUNT_INDEX].Width = 60;
            gvMain.Columns[m_nALARM_COUNT_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nALARM_COUNT_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nSYSTEM_RECOVERY_COUNT_INDEX].Name = "시스템복구";
            gvMain.Columns[m_nSYSTEM_RECOVERY_COUNT_INDEX].Width = 60;
            gvMain.Columns[m_nSYSTEM_RECOVERY_COUNT_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nSYSTEM_RECOVERY_COUNT_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;

            gvMain.Columns[m_nEQUIPMENT_RECOVERY_COUNT_INDEX].Name = "현장복구";
            gvMain.Columns[m_nEQUIPMENT_RECOVERY_COUNT_INDEX].Width = 60;
            gvMain.Columns[m_nEQUIPMENT_RECOVERY_COUNT_INDEX].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            gvMain.Columns[m_nEQUIPMENT_RECOVERY_COUNT_INDEX].SortMode = DataGridViewColumnSortMode.NotSortable;


            gvMain.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;*/
            //gvMain.Columns[m_nNO_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //gvMain.Columns[m_nMATERIAL_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //gvMain.Columns[m_nBUILDING_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //gvMain.Columns[m_nDETECT_LOCATION_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //gvMain.Columns[m_nDETECT_COUNT_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //gvMain.Columns[m_nALARM_COUNT_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //gvMain.Columns[m_nSYSTEM_RECOVERY_COUNT_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            //gvMain.Columns[m_nEQUIPMENT_RECOVERY_COUNT_INDEX].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Font font = gvMain.Font;
            gvMain.Font = new Font("맑은 고딕", 12.0f);
        }

        public void Load_DataGrid(bool isLoad = false)
        {
            SaveArr.Clear();
            //lblNullResult.Visible = false;
            gvMain.DataSource = null;
            notOperationPSMPageGridDataBindingSource.Clear();
            //gvMain.Rows.Clear();

            //string -> DateTime
            string strNowDate = "";
            string strBeforeDate = "";

            strNowDate = string.Format("{0} {1}:{2}:{3}", strEndDate.ToString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", strStartDate.ToString(), 00, 00, 00);

            int nRowNo = 0;

            int nHwpTable = 14;
            int k = 0;

            foreach (MulFunctionPSMLog mulFunctionLog in m_detectMgr.MulFunctionList)
            {
                if (mulFunctionLog.PSMMaterial == null)
                    continue;

                //int nType = 
                Zone zone = mulFunctionLog.Zone;
                EquipmentZone equipZone = mulFunctionLog.EquipmentZone;
                UnE.PSM.PSMMaterial material = mulFunctionLog.PSMMaterial;
                int nReactionCount = mulFunctionLog.ReactionCount;
                int nMulFunctionCount = mulFunctionLog.MulFunctionCount;
                int nNotifyCount = mulFunctionLog.NotifyCount;
                string strBuildingGroupName = mulFunctionLog.GroupName;
                string strBuildingName = mulFunctionLog.BuildingName;
                string strFloorName = mulFunctionLog.FloorName;
                double nPercentMulFunction = mulFunctionLog.PercentMulFunction;
                int nNotProcss = mulFunctionLog.Notprocess;

                string strType = mulFunctionLog.DetectType;


                FacilityManagerGroup ManagerGroup = null;

                ManagerGroup = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(IFacility.FacilityType.PSM_SENSOR, equipZone, true);


                if (ManagerGroup == null)
                    ManagerGroup = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(IFacility.FacilityType.PSM_SENSOR, equipZone.LinkedZone.Building, true);

                if (ManagerGroup == null)
                    ManagerGroup = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(IFacility.FacilityType.PSM_SENSOR, true);

                strManagerName = FormMain.Instance.DataManager.GetFacilityManagerName(ManagerGroup, ref strPhoneNumber);

                NotOperationPSMPageGridData data = new NotOperationPSMPageGridData();
                data.No = nRowNo + 1;
                data.Material = mulFunctionLog.PSMMaterial;
                data.Building = strBuildingName;
                data.Location = equipZone.DisplayText;
                data.Detect = nReactionCount.ToString();
                data.Report = nNotifyCount.ToString();
                data.SystemRecovery = nMulFunctionCount.ToString();
                data.FieldRecovery = nNotProcss.ToString();

                notOperationPSMPageGridDataBindingSource.Add(data);

                foreach (var prop in data.GetType().GetProperties())
                {
                    SaveArr.Add(prop.GetValue(data, null).ToString());
                }
                /*gvMain.Rows.Add();
                gvMain.Rows[nRowNo].Cells[m_nNO_INDEX].Value = nRowNo +1;
                gvMain.Rows[nRowNo].Cells[m_nMATERIAL_INDEX].Value = mulFunctionLog.PSMMaterial;
                gvMain.Rows[nRowNo].Cells[m_nBUILDING_INDEX].Value = strBuildingName;
                gvMain.Rows[nRowNo].Cells[m_nDETECT_LOCATION_INDEX].Value = equipZone.DisplayText;
                gvMain.Rows[nRowNo].Cells[m_nDETECT_COUNT_INDEX].Value =nReactionCount.ToString();
                gvMain.Rows[nRowNo].Cells[m_nALARM_COUNT_INDEX].Value =nNotifyCount.ToString();
                gvMain.Rows[nRowNo].Cells[m_nSYSTEM_RECOVERY_COUNT_INDEX].Value =nMulFunctionCount.ToString();
                gvMain.Rows[nRowNo].Cells[m_nEQUIPMENT_RECOVERY_COUNT_INDEX].Value = nNotProcss.ToString();

                int HwpIndex = 0;

                for (k = nHwpTable; k < nHwpTable + m_nColumnCount; k++)
                {
                    SaveArr.Add(gvMain.Rows[nRowNo].Cells[HwpIndex].Value.ToString());

                    HwpIndex++;
                }*/
                nHwpTable += m_nColumnCount;

                nRowNo++;
            }

            if (nRowNo == 0)
            {
                //lblNullResult.Visible = true;
            }

            gvMain.DataSource = notOperationPSMPageGridDataBindingSource;

            DateTime dtStart = DateTime.ParseExact(strStartDate, "yyyy-MM-dd", null);
            DateTime dtEnd = DateTime.ParseExact(strEndDate, "yyyy-MM-dd", null);

            strNowDate = string.Format("{0} {1}:{2}:{3}", dtEnd.ToShortDateString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", dtStart.ToShortDateString(), 00, 00, 00);

            string strdtMin = "";
            string strdtMax = "";

            if (isLoad == false)
            {
                lblBuilding.Text = strbuilding;
            }

            strdtMin = string.Format("{0}년 {1}월 {2}일", dtStart.Year, dtStart.Month, dtStart.Day);
            strdtMax = string.Format("{0}년 {1}월 {2}일", dtEnd.Year, dtEnd.Month, dtEnd.Day);

            //조회기간
            lblMinDate.Text = strdtMin + " 부터 ";
            lblMaxDate.Text = strdtMax + " 까지";
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

        private void NotOperationPSMPage_Resize(object sender, EventArgs e)
        {
            SetChartBar();
            Point ptGrid = gvMain.Location;
            Size SizeGrid = gvMain.Size;
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
            for (int i = 1; i < nWeekCount + 1; i++)
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
                int nNotifyCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

                bool bFind = false;

                foreach (KeyValuePair<MulFunctionPSMLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionPSMLog mullog = pair.Key;

                    foreach (Report.SensorReactionPSMLog log in arrSensorReaction)
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
                        int nNotify = mullog.NotifyCount;
                        int nOnlyDetect = mullog.OnlyDetectCount;


                        if (i - 1 == nWeekend)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nNotify = (nNotify == -1) ? nNotify = 0 : nNotify;
                            nOnlyDetect = (nNotify == -1) ? nOnlyDetect = 0 : nOnlyDetect;

                            //한달치 합을구함, 
                            nReactionCount += nReaction;
                            nNotifyCount += nNotify;
                            nMulFunctionCount += nMulFunction;
                            nOnlyDetectCount += nOnlyDetect;
                            bFind = true;
                        }
                    }
                }
                if (bFind == true) //한달치 합을 배열에 넣음
                {
                    //처리되지않은 신호
                    int nNotCount = nReactionCount - (nNotifyCount + nMulFunctionCount + nOnlyDetectCount);

                    data0[y_nCount] = Math.Ceiling(Convert.ToDouble(nNotCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
                    data1[y_nCount] = Math.Floor(Convert.ToDouble(nNotifyCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
                    data2[y_nCount] = Math.Floor(Convert.ToDouble(nMulFunctionCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
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
                int nNotifyCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

                bool bFind = false;

                foreach (KeyValuePair<MulFunctionPSMLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionPSMLog mullog = pair.Key;

                    foreach (Report.SensorReactionPSMLog log in arrSensorReaction)
                    {
                        string nMonth = log.Time.ToShortDateString().Substring(0, 7);
                        int nZoneID = log.Param1;
                        int nReaction = mullog.ReactionCount;
                        int nMulFunction = mullog.MulFunctionCount;
                        int nNotify = mullog.NotifyCount;
                        int nOnlyDetect = mullog.OnlyDetectCount;

                        //dt.AddDays(-i).ToString().Substring(0, 10) == dtCmp.ToString().Substring(0, 10)
                        string test = dtStart.AddMonths(i).ToString().Substring(0, 7);
                        if (test == nMonth)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nNotify = (nNotify == -1) ? nNotify = 0 : nNotify;
                            nOnlyDetect = (nOnlyDetect == -1) ? nOnlyDetect = 0 : nOnlyDetect;

                            //한달치 합을구함, 
                            nReactionCount += nReaction;
                            nNotifyCount += nNotify;
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
                    int nNotCount = nReactionCount - (nNotifyCount + nMulFunctionCount + nOnlyDetectCount);

                    data0[y_nCount] = Math.Ceiling(Convert.ToDouble(nNotCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
                    data1[y_nCount] = Math.Floor(Convert.ToDouble(nNotifyCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
                    data2[y_nCount] = Math.Floor(Convert.ToDouble(nMulFunctionCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
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
            for (int i = n_ArrayStart; nCount < n_arrayCount; i++, nCount++)
            {
                if (i % 4 == 0)
                    i = 4;
                else
                {
                    if (i == 5)
                        nYear++;

                    i = i % 4;
                }

                if (n_ts == 0)
                    x_arr.Add(String.Format("{0}분기", i));
                else
                    x_arr.Add(String.Format("{0}년도 {1}분기", nYear, i));


                int nReactionCount = 0;
                int nNotifyCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

                bool bFind = false;

                foreach (KeyValuePair<MulFunctionPSMLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    int QuarterNumber = 0;
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionPSMLog mullog = pair.Key;

                    foreach (Report.SensorReactionPSMLog log in arrSensorReaction)
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
                        int nNotify = mullog.NotifyCount;
                        int nOnlyDetect = mullog.OnlyDetectCount;

                        if (nYear == nYears && i == nQuarter)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nNotify = (nNotify == -1) ? nNotify = 0 : nNotify;
                            nOnlyDetect = (nNotify == -1) ? nOnlyDetect = 0 : nOnlyDetect;

                            //한달치 합을구함, 
                            nReactionCount += nReaction;
                            nNotifyCount += nNotify;
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
                    int nNotCount = nReactionCount - (nNotifyCount + nMulFunctionCount + nOnlyDetectCount);

                    data0[y_nCount] = Math.Ceiling(Convert.ToDouble(nNotCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
                    data1[y_nCount] = Math.Floor(Convert.ToDouble(nNotifyCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
                    data2[y_nCount] = Math.Floor(Convert.ToDouble(nMulFunctionCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
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
                x_arr.Add(String.Format("{0}년도", (dtStart.AddYears(i).ToString().Substring(0, 4))));
                //arr.Add(dt.AddDays(-i).ToString());
                int nReactionCount = 0;
                int nNotifyCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

                bool bFind = false;
                foreach (KeyValuePair<MulFunctionPSMLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionPSMLog mullog = pair.Key;

                    foreach (Report.SensorReactionPSMLog log in arrSensorReaction)
                    {
                        string nYear = log.Time.ToShortDateString().Substring(0, 4);
                        int nZoneID = log.Param1;
                        int nReaction = mullog.ReactionCount;
                        int nMulFunction = mullog.MulFunctionCount;
                        int nNotify = mullog.NotifyCount;
                        int nOnlyDetect = mullog.OnlyDetectCount;

                        //dt.AddDays(-i).ToString().Substring(0, 10) == dtCmp.ToString().Substring(0, 10)
                        string test = dtStart.AddYears(i).ToString().Substring(0, 4);
                        if (test == nYear)
                        {
                            //Null값은 0으로 처리
                            nReaction = (nReaction == -1) ? nReaction = 0 : nReaction;
                            nMulFunction = (nMulFunction == -1) ? nMulFunction = 0 : nMulFunction;
                            nNotify = (nNotify == -1) ? nNotify = 0 : nNotify;
                            nOnlyDetect = (nOnlyDetect == -1) ? nOnlyDetect = 0 : nOnlyDetect;

                            //1년치 합을구함, 
                            nReactionCount += nReaction;
                            nNotifyCount += nNotify;
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
                    int nNotCount = nReactionCount - (nNotifyCount + nMulFunctionCount + nOnlyDetectCount);

                    data0[y_nCount] = Math.Ceiling(Convert.ToDouble(nNotCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
                    data1[y_nCount] = Math.Floor(Convert.ToDouble(nNotifyCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
                    data2[y_nCount] = Math.Floor(Convert.ToDouble(nMulFunctionCount * 100) / (nNotCount + nNotifyCount + nMulFunctionCount + nOnlyDetectCount));
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
            XYChart c = new XYChart(gvMain.Size.Width - cboChart.Width - nSpace, 290);

            LegendBox legendBox = c.addLegend(c.getWidth() - 10, 90, true, "Arial", 10);
            legendBox.setAlignment(Chart.TopRight);

            c.xAxis().setLabels(labels);
            c.xAxis().setLabelStyle().setPos(c.xAxis().getX(), c.xAxis().getY() + 10);
            c.yAxis().setTitle("백분율(%)");
            c.yAxis().setDateScale(0, 100);

            c.setPlotArea(60, 20, gvMain.Size.Width - cboChart.Width - nSpace * 11, 230);

            c.xAxis().setTickOffset(0.1);

            BarLayer layer = c.addBarLayer2(Chart.Side);

            layer.addDataSet(data0, 0x6B9900, "현장 복구");
            layer.addDataSet(data1, 0xff0000, "누출 발생");
            layer.addDataSet(data2, 0x00D8FF, "시스템 복구");

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

        private void btnSaveHWP_Click(object sender, EventArgs e)
        {

            btnSaveHWP.Enabled = false;
            PageBackstageHome.Instance.FrmReport.SaveHWPForNotOperationPSM();
            btnSaveHWP.Enabled = true;
        }

        public void SetVisibleHWPExport(bool visible)
        {
            btnSaveHWP.Visible = visible;
        }
    }
 }

