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
using System.IO;
using SDMS.Report;
using Microsoft.Win32;
using System.Diagnostics;

namespace SDMS
{
    public partial class DetectPage : Form
    {
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

        private string[] labels = null;
        private double[] data = null;

        private string strManagerName;
        private string strPhoneNumber = "";


        //현재 선택된 날짜(선택된 기간이 바뀌었는지 아닌지 알기 위한..)
        private DateTime m_SelectedMinDate;
        private DateTime m_SelectedMaxDate;
        private ArrayList m_arrSelectedZone = null;

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

        private Report.ReactionManager m_detectMgr = null;

        public DetectPage(Report.ReactionManager detectMgr)
        {
            InitializeComponent();

            isFirstLoad = true;

            m_detectMgr = detectMgr;

            //보안모듈 등록
            m_hwpCtrl = new HwpCtrlData();
            m_hwpCtrl.SetRegistry();

            m_arrHistoryData = new ArrayList();
        }

        private void DetectPage_Load(object sender, EventArgs e)
        {
            InitLoadData();
            //화재탐지페이지가 처음 로드될 때 이벤트 한 번 실행
            FormMain.Instance.proc_cboLatelyDate_SelectedIndexChanged(sender, e);
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
                file.WriteLine(lblMinDate.Text + lblMaxDate.Text);
                file.WriteLine(lblBuilding.Text);
                file.Close();
            }
        }

        public void FileWriter()
        {
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
            DateTime startDate = DateTime.Now.AddMonths(-6);
            DateTime EndDate = DateTime.Now;

            m_SelectedMinDate = startDate;
            m_SelectedMaxDate = EndDate;
            m_arrSelectedZone = arrSelectZoneList;

            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            m_detectMgr.ZoneSubmit(arrSelectZoneList, startDate, EndDate);
            SetupDataGrid();


            // 날짜순으로 내림차순으로 정렬
            

            //dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;


            //찾은 검색결과를 DataGrid로 출력
            Load_DataGrid();
           

            //그래프 그리기
            CreateLineChart(startDate, EndDate, true);
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

        public void ComboTxtDate(string strStrat, string strEnd)
        {
            strStartDate = strStrat;
            strEndDate = strEnd;
        }

        public void CreateLineChart(DateTime StartDate, DateTime EndDate, bool isLoad = false)
        {
            WebDBManager m_dbMgr = FormMain.Instance.DBManager;

            DateTime dtNowDate = EndDate;
            DateTime dtBeforeDate = StartDate;
            //

            DateTime defaultdt = new DateTime();

            string strNowDate = "";
            string strBeforeDate = "";

            strNowDate = string.Format("{0} {1}:{2}:{3}", dtNowDate.ToShortDateString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", dtBeforeDate.ToShortDateString(), 00, 00, 00);
          
            //두 날짜의 달 차이 계산
            int m_ts = 12 * (dtNowDate.Year - dtBeforeDate.Year) + (dtNowDate.Month - dtBeforeDate.Month);

            //시작일과 종료일의 시간 차 계산
            TimeSpan ts = dtNowDate - dtBeforeDate;
            int totalToday = ts.Days;

            ArrayList arrDays = new ArrayList();
            ArrayList y_arr = new ArrayList();

            int tsTime = 0;
            int totalTime = 0;


            bool isToday = false;
            //기간에 오늘날짜가 포함인가?
            if (dtNowDate.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                isToday = true;
                //dtBeforeDate = DateTime.Now;
            }

            //날짜 차이가 6일 아래일경우
            if (totalToday <= 5)
            {
                arrDays.Add(dtBeforeDate.Month.ToString() + "월 " + dtBeforeDate.Day.ToString() + "일 " + "0시");
                y_arr.Add(0);

                tsTime = totalToday;
                if (isToday == false)
                {
                    //일 * 24시간
                    totalTime = (tsTime + 1) * 24;


                }
                else//날짜에 오늘이 포함되어있을경우
                {
                    //(일-1) * 24시간 + 오늘 하루동안 시간
                    totalTime = tsTime * 24 + DateTime.Now.Hour;

                }

                DateTime dtTemp = DateTime.Now;

                for (int k = 1; k < 7; k++)
                {
                    int nCount = 0;

                    //구한 Time
                    int nAddTime = (totalTime * k) / 6;
                    int nRest = (totalTime * k) % 6;

                    //몫/2
                    int nTemp = nAddTime / 2;
                    //나머지 < (nAddTime/2)
                    if (nRest > nTemp)
                    {
                        nAddTime++;
                    }

                    //기존 DateTime에서 nAddTime을 더함(X축데이터 구함)
                    DateTime dtNextDate = dtBeforeDate.AddHours(nAddTime);

                    if (k == 6 && isToday == true)
                    {
                        dtNextDate = DateTime.Now;
                        arrDays.Add(dtNextDate.Month.ToString() + "월 " + dtNextDate.Day.ToString() + "일 " + dtNextDate.Hour + "시");
                    }
                    else
                    {
                        arrDays.Add(dtNextDate.Month + "월 " + dtNextDate.Day + "일 " + dtNextDate.Hour + "시");
                    }

                    DateTime dtMaxDate = dtNextDate;
                    DateTime dtMinDate = DateTime.Now;
                    
                    if (k == 1)
                        dtMinDate = dtBeforeDate;
                    else
                        dtMinDate = dtTemp;                     

                    int nResultCount = 0;

                    // SensorHistoryData List
                    ArrayList arrSensorZoneHistory = new ArrayList();
                    arrSensorZoneHistory = m_detectMgr.DectectList;

                    foreach (Report.DetectLog detectlog in arrSensorZoneHistory)
                    {
                        if (detectlog.Time >= dtMinDate && detectlog.Time <= dtMaxDate)
                        {
                            nCount++;
                        }
                    }

                    nResultCount = nCount;
                    y_arr.Add(nResultCount);

                    //
                    dtTemp = dtNextDate;
                }
                labels = new string[7];
                data = new double[7];
                int n_count = 0;

                foreach (string x in arrDays)
                {
                    labels[n_count] = x;

                    n_count++;
                }
                n_count = 0;
                foreach (int y in y_arr)
                {
                    data[n_count] = y;
                    n_count++;
                }
            }
            else
            {
                //최근 6개월
                int nMonthCount = 0;
                TimeSpan tsSubMonth = dtNowDate - dtNowDate.AddMonths(-6);
                nMonthCount = tsSubMonth.Days;

                ArrayList arrMonth = new ArrayList();

                arrDays.Add(dtNowDate.ToShortDateString().ToString());

                for (int i = 0; i < 6; i++)
                {
                    int ndays = (totalToday * (i + 1)) / 6;
                    defaultdt = dtNowDate.AddDays(-ndays);

                    arrDays.Add(defaultdt.ToShortDateString().ToString());
                }

                arrDays.Reverse();
                labels = new string[7];
                int n_count = 0;

                y_arr.Add(0);
                foreach (string x in arrDays)
                {
                    labels[n_count] = x;
                    if (n_count > 0)
                    {
                        DateTime dtNowDateTime = DateTime.ParseExact(x, "yyyy-MM-dd", null);
                        //dtNowDateTime = dtNowDateTime.AddDays(-1);

                        strNowDate = string.Format("{0} {1}:{2}:{3}", dtNowDateTime.ToShortDateString(), 23, 59, 59);
                        //strBeforeDate = string.Format("{0} {1}:{2}:{3}", labels[n_count - 1], 00, 00, 00);
                        strBeforeDate = string.Format("{0}", labels[n_count - 1]);

                        //데이터를 비교하기위해 최소,최대날짜를 DateTime으로 변환
                        DateTime dtMaxDate = DateTime.ParseExact(strNowDate, "yyyy-MM-dd HH:mm:ss", null);
                        DateTime dtMinDate = DateTime.ParseExact(strBeforeDate, "yyyy-MM-dd", null);
                        if (n_count != 6)
                            dtMaxDate = dtMaxDate.AddDays(-1);
                        int nResultCount = 0;
                        int nCount = 0;
                        bool bFirst = true;

                        Debug.WriteLine(dtMinDate + "  " + dtMaxDate);
                        foreach (Report.DetectLog log in m_detectMgr.DectectList)
                        {
                            if (log.Time >= dtMinDate && log.Time < dtMaxDate)
                            {
                                if (bFirst == true)
                                {
                                    //Debug.WriteLine(dtMinDate + "  " + dtMaxDate);
                                    bFirst = false;
                                }
                                nCount++;
                            }

                        }
                        nResultCount = nCount;

                        y_arr.Add(nResultCount);
                    }

                    n_count++;
                }
               

                data = new double[7];
                n_count = 0;
                foreach (int y in y_arr)
                {
                    data[n_count] = y;
                    n_count++;
                }
            }

            //조회기간
            string strdtMin = "";
            string strdtMax = "";

            if (isLoad == false)
            {
                lblBuilding.Text = strgroup + "  " + strbuilding + "  " + strfloor;
            }

            strdtMin = string.Format("{0}년 {1}월 {2}일", StartDate.Year, StartDate.Month, StartDate.Day);
            strdtMax = string.Format("{0}년 {1}월 {2}일", EndDate.Year, EndDate.Month, EndDate.Day);

            //조회기간
            lblMinDate.Text = strdtMin + "부터 ";
            lblMaxDate.Text = strdtMax + "까지";

            setChart();
        }

        private void setChart()
        {
            int nParentWidth = this.Size.Width;
            int nSpace = 60;

            Size sizeGrid = this.dataGridView1.Size;
            Point ptGrid = this.dataGridView1.Location;

            XYChart c = new XYChart(sizeGrid.Width, 280);

            // Set the plotarea at (30, 20) and of size 200 x 200 pixels
            c.setPlotArea(ptGrid.X, 40, sizeGrid.Width - nSpace * 2, 200);

            // Set the default line width to 2 pixels

            c.addLegend(50, 0, false, "Arial Bold", 9).setBackground(Chart.Transparent);
            c.yAxis().setTitle("발생 건수");
            c.xAxis().setLabels(labels);
            LineLayer layer1 = c.addLineLayer(data, 0xff0000, "화재 탐지");
            layer1.setLineWidth(2);

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

            setChart();
            Point ptGrid = dataGridView1.Location;
            Size SizeGrid = dataGridView1.Size;
        }

        private void SetupDataGrid()
        {
            this.Controls.Add(dataGridView1);

            // 컬럼의 AutoSizeMode는 AllCellsExceptHeader, AllCells, DisplayedCells, DisplayedCellsExceptHeader
            // 등의 방법을 사용하는 경우 데이터가 많을시 열너비 조정시간이 많이 걸린다.
            // 길이를 직접 지정할것, 고정길이는 none으로 지정하고 그외에는 디폴트로 처리되로록 한다. 


            dataGridView1.ColumnCount = 8;

            dataGridView1.Columns[0].Name = "No";
            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;


            dataGridView1.Columns[1].Name = "날짜";
            dataGridView1.Columns[1].Width = 55;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[2].Name = "유형";
            dataGridView1.Columns[2].Width = 45;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            //dataGridView1.Columns[3].Name = "관리ID";
            //dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView1.Columns[3].Width = 50;

            dataGridView1.Columns[3].Name = "건물 그룹";
            dataGridView1.Columns[3].Width = 60;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[4].Name = "건물";
            dataGridView1.Columns[4].Width = 95;
            dataGridView1.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dataGridView1.Columns[5].Name = "층";
            dataGridView1.Columns[5].Width = 30;
            dataGridView1.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[6].Name = "화재 발생장소";
            dataGridView1.Columns[6].Width = 100;
            dataGridView1.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns[7].Name = "담당자";
            dataGridView1.Columns[7].Width = 50;
            dataGridView1.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public void Load_DataGrid()
        {
            SaveArr.Clear();
            dataGridView1.Rows.Clear();
            dataGridView1.Invalidate();          

            WebDBManager m_dbMgr = FormMain.Instance.DBManager;

            // SensorHistoryData List
            ArrayList arrSensorZoneHistory = new ArrayList();
            arrSensorZoneHistory = m_detectMgr.DectectList;
            

            int nHwpTable = 10;
            int count = 0;
            int nNumber = 1;

            foreach (Report.DetectLog historyData in arrSensorZoneHistory)
            {
                Zone zoneLink = ZoneManager.Instance.GetZone(historyData.zoneID);
                if (zoneLink == null)
                    continue;

                string szBuildingName = zoneLink.Building != null ? zoneLink.Building.BuildingName : "";
                string szGroupName = szBuildingName != "" ? zoneLink.Building.BuildingGroup.BuildingGroupName : "";

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
                if (strType == "자탐 센서")
                {
                    equipZone = historyData.EquipZone;

                    if (equipZone != null)
                        equipZoneName = equipZone.ZoneName;
                }
                else//수동신고
                {
                    ArrayList arEquipzone = ZoneManager.Instance.GetEquipmentZoneList(zoneLink);
                    if (arEquipzone != null && arEquipzone.Count > 0)
                    {
                        equipZone = (EquipmentZone)arEquipzone[0];
                    }

                    //수동신고는 EquipmentZone을 표시하지 않음
                    equipZoneName = "-";
                }


                if (equipZone != null)
                {
                    ManagerGroup = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(Facility.FacilityType.FIRE_SENSOR, equipZone);
                }

                if (ManagerGroup == null)
                    ManagerGroup = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(Facility.FacilityType.FIRE_SENSOR, buildingFind);

                if (ManagerGroup == null)
                    ManagerGroup = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(Facility.FacilityType.FIRE_SENSOR);

                strManagerName = FormMain.Instance.DataManager.GetFacilityManagerName(ManagerGroup, ref strPhoneNumber);
                

                string[] rows = { " ", "", strType, szGroupName, szBuildingName, strFloorIndex, equipZoneName , strManagerName };
                dataGridView1.Rows.Add(rows);
                dataGridView1.Rows[count].Cells[0].Value = nNumber;
                dataGridView1.Rows[count].Cells[1].Value = historyData.Time;

                SaveHwpCtrl(ref nHwpTable, ref count, ref nNumber);
            }

            //원래있던 표의 줄 수를 저장함
            storage = dataGridView1.Rows.Count;

            dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Descending);
        }

        private void SaveHwpCtrl(ref int nHwpTable, ref int count, ref int nNumber)
        {
            int HwpIndex = 0;

            for (int k = nHwpTable; k < nHwpTable + 7; k++)
            {
                SaveArr.Add(dataGridView1.Rows[count].Cells[HwpIndex].Value.ToString());

                HwpIndex++;
            }
            nHwpTable += 7;
            count++;
            nNumber++;
        }


        public string GetReactionString(int nType)
        {
            string strType = "";
            switch (nType)
            {
                case 1: strType = "자탐 센서";
                    break;
                case 2: strType = "소화 센서";
                    break;
                case 3: strType = "압력 센서";
                    break;
                case 4: strType = "수동 신고";
                    break;
                default:
                    break;
            }

            return strType;
        }
    }
}
