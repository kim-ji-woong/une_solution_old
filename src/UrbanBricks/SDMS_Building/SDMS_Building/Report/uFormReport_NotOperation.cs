using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Sensor;
using UnE.Spatial;
using System.Collections;
using System.Globalization;

namespace SDMS_Building.Report
{
    public partial class uFormReport_NotOperation : UserControl
    {
        private UEWpfControl.WpfComboBox m_cbType = null; // 유형 combobox
        private UEWpfControl.WpfComboBox m_cbBuilding = null; // 빌딩 combobox
        private UEWpfControl.WpfComboBox m_cbFloor = null; // 층 combobox
        private UEWpfControl.WpfComboBox m_cbLevel = null; // 알람 단계 combobox
        private UEWpfControl.WpfComboBox m_cbReportType = null; // 리포트 Type combobox

        private IFacility.FacilityType m_curFacilityType = IFacility.FacilityType.FIRE_SENSOR;
        public IFacility.FacilityType CurFacilityType
        {
            get
            {
                return m_curFacilityType;
            }
        }

        private ReactionManager m_detectMgr = null;
        private ReportType m_curReportType = ReportType.NotOperation;

        private DateTime m_dtStartDate
        {
            get
            {
                DateTime startDate = Convert.ToDateTime(lblDateStart.Text);
                return new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            }
        }

        private DateTime m_dtEndDate
        {
            get
            {
                DateTime endDate = Convert.ToDateTime(lblDateEnd.Text);
                return new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0);
            }
        }
        
        private int m_nViewCount = 8;
        private int m_nCurrentPage = -1;
        private int m_nTotalPage = -1;

        public uFormReport_NotOperation(ReactionManager detectMgr)
        {
            InitializeComponent();

            FormMain.SetDoubleBuffer(panelChart, true);
            FormMain.SetDoubleBuffer(dataGridView1, true);

            m_detectMgr = detectMgr;

            lblDateStart.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            lblDateEnd.Text = DateTime.Now.ToString("yyyy-MM-dd");

            FormMain.Instance.CustomGridView(dataGridView1, 12.0f, Color.FromArgb(0x25, 0x31, 0x50), Color.White, Color.FromArgb(0xf3, 0xf4, 0xfa), Color.FromArgb(0x25, 0x31, 0x50), DataGridViewContentAlignment.MiddleCenter);
            dataGridView1.ScrollBars = ScrollBars.Vertical;

            m_cbType = new UEWpfControl.WpfComboBox();
            eleType.Child = m_cbType;
            m_cbType.SetSize(eleType.Width, eleType.Height);
            m_cbType.customComboBox.SelectionChanged += cbType_SelectionChanged;

            m_cbBuilding = new UEWpfControl.WpfComboBox();
            eleBuilding.Child = m_cbBuilding;
            m_cbBuilding.customComboBox.SelectionChanged += cbBuilding_SelectionChanged;
            m_cbBuilding.SetSize(eleBuilding.Width, eleBuilding.Height);

            m_cbFloor = new UEWpfControl.WpfComboBox();
            eleFloor.Child = m_cbFloor;
            m_cbFloor.SetSize(eleFloor.Width, eleFloor.Height);

            m_cbLevel = new UEWpfControl.WpfComboBox();
            eleLevel.Child = m_cbLevel;
            m_cbLevel.SetSize(eleLevel.Width, eleLevel.Height);

            m_cbReportType = new UEWpfControl.WpfComboBox();
            eleReportType.Child = m_cbReportType;
            m_cbReportType.customComboBox.SelectionChanged += cbReportType_SelectionChanged;
            m_cbReportType.SetSize(eleReportType.Width, eleReportType.Height);
        }

        private void uFormReport_NotOperation_Load(object sender, EventArgs e)
        {
            InitPosition();
            InitType();
            InitBuildingComboBox();

            LoadData();
            CreateBarChart();
        }

        private void uFormReport_NotOperation_Resize(object sender, EventArgs e)
        {
            ResizeColumnWidth();
            InitPosition();
        }

        private void InitType()
        {
            m_cbType.customComboBox.DisplayMemberPath = "DisplayText";

            List<DisasterTypeItem> items = new List<DisasterTypeItem>();
            items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.FIRE_SENSOR, DisplayText = Data.CommonString.POI_Fire_Kor });

            //if (UnE.SOP.ProxySOP.Instance.UsePSM)
            //{
            //    string txt = "누출";
            //    if (UnE.SOP.ProxySOP.Instance.SiteID == 201)
            //        txt = Data.CommonString.POI_Gas_Kor;
            //    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.PSM_SENSOR, DisplayText = txt });
            //}
            
            //if (UnE.SOP.ProxySOP.Instance.UseBlackout)
            //    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.BLACKOUT, DisplayText = Data.CommonString.POI_Blackout_Kor });
            //if (UnE.SOP.ProxySOP.Instance.UseStrongWind)
            //    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.STRONG_WIND, DisplayText = Data.CommonString.POI_StrongWind_Kor });
            //if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
            //    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.Earthquake, DisplayText = Data.CommonString.POI_Earthquake_Kor });
            
            m_cbType.customComboBox.ItemsSource = items;

            if (m_cbType.customComboBox.Items.Count > 0)
                m_cbType.customComboBox.SelectedIndex = 0;

            m_cbLevel.customComboBox.Items.Add("관심");
            m_cbLevel.customComboBox.Items.Add("주의");
            m_cbLevel.customComboBox.Items.Add("경계");
            m_cbLevel.customComboBox.Items.Add("심각");
            m_cbLevel.customComboBox.SelectedIndex = 1;

            m_cbReportType.customComboBox.Items.Add("주별 보기");
            m_cbReportType.customComboBox.Items.Add("월별 보기");
            m_cbReportType.customComboBox.Items.Add("분기별 보기");
            m_cbReportType.customComboBox.Items.Add("연도별 보기");
            m_cbReportType.customComboBox.SelectedIndex = 0;
        }

        private void InitBuildingComboBox()
        {
            m_cbBuilding.customComboBox.DisplayMemberPath = "BuildingName";

            Building building = new Building();
            building.BuildingName = "전체";
            building.ID = -1;
            m_cbBuilding.customComboBox.Items.Add(building);

            foreach (KeyValuePair<int, Building> item in UnE.Spatial.ZoneManager.Instance.DicBuildings)
            {
                m_cbBuilding.customComboBox.Items.Add(item.Value);
            }

            if (m_cbBuilding.customComboBox.Items.Count > 0)
                m_cbBuilding.customComboBox.SelectedIndex = 0;
        }

        private void InitGridView()
        {
            if (!uFormReport.Instance.DicDefineColumns[m_curFacilityType].ContainsKey(m_curReportType))
            {
                MessageBox.Show(IFacility.GetFacilityTypeString(m_curFacilityType) + " column 정보가 없음");
                m_cbType.customComboBox.SelectedIndex = 0; // 화재로 넘김

                return;
            }

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            foreach (TypeColumns item in uFormReport.Instance.DicDefineColumns[m_curFacilityType][m_curReportType])
            {
                AddColumn(dataGridView1, item.DefineColumn.ColumnName, item.HeaderText, item.ColumnWidthRatio);
            }
        }

        private void ResizeColumnWidth()
        {
            if (!uFormReport.Instance.DicDefineColumns[m_curFacilityType].ContainsKey(m_curReportType))
                return;

            foreach (TypeColumns item in uFormReport.Instance.DicDefineColumns[m_curFacilityType][m_curReportType])
            {
                if (dataGridView1.Columns.Count > 0)
                {
                    DataGridViewColumn obj = dataGridView1.Columns[item.DefineColumn.ColumnName];
                    if (!dataGridView1.Columns[item.DefineColumn.ColumnName].Visible)
                        continue;

                    // 전체값 * 퍼센트 / 100
                    int per = dataGridView1.Width * item.ColumnWidthRatio / 100;
                    dataGridView1.Columns[item.DefineColumn.ColumnName].Width = per;
                }
            }
        }

        private void AddColumn(DataGridView gridview, string columnName, string headerText, int columnWidthPer)
        {
            gridview.Columns.Add(columnName, headerText);
            gridview.Columns[columnName].SortMode = DataGridViewColumnSortMode.NotSortable;

            // 전체값 * 퍼센트 / 100
            int per = gridview.Width * columnWidthPer / 100;
            gridview.Columns[columnName].Width = per;
        }

        private void InitPosition()
        {
            int empty = 20;
            // 유형
            lblType.Location = new Point(50, 61);
            eleType.Location = new Point(lblType.Location.X + lblType.Width, 60);

            // 기간
            lblDate.Location = new Point(eleType.Location.X + eleType.Width + empty, 61);
            lblDateStart.Location = new Point(lblDate.Location.X + lblDate.Width, 61);
            btnDateStart.Location = new Point(lblDateStart.Location.X + lblDateStart.Width - btnDateStart.Width - 8, (lblDateStart.Height - btnDateStart.Height) / 2 + lblDateStart.Location.Y);
            label8.Location = new Point(lblDateStart.Location.X + lblDateStart.Width, 60);
            lblDateEnd.Location = new Point(label8.Location.X + label8.Width, 61);
            btnDateEnd.Location = new Point(lblDateEnd.Location.X + lblDateEnd.Width - btnDateEnd.Width - 8, (lblDateEnd.Height - btnDateEnd.Height) / 2 + lblDateEnd.Location.Y);

            // 위치
            lblLoaction.Location = new Point(lblDateEnd.Location.X + lblDateEnd.Width + empty, 61);
            eleBuilding.Location = new Point(lblLoaction.Location.X + lblLoaction.Width, 60);
            eleFloor.Location = new Point(eleBuilding.Location.X + eleBuilding.Width, 60);

            // 단계
            lblLevel.Location = new Point(eleFloor.Location.X + eleFloor.Width + empty, 61);
            eleLevel.Location = new Point(lblLevel.Location.X + lblLevel.Width, 60);

            // 검색
            btnSearch.Location = new Point(eleLevel.Location.X + eleLevel.Width + empty, 61);

            panelChart.Location = new Point(50, 200);
            panelChart.Size = new Size(panelChart.Width, 393);

            lblGridViewTitle.Location = new Point(panelChart.Location.X, panelChart.Location.Y + panelChart.Height + 20);

            dataGridView1.Location = new Point(lblGridViewTitle.Location.X, lblGridViewTitle.Location.Y + lblGridViewTitle.Height);
            dataGridView1.Size = new Size(dataGridView1.Width, 240);
            
            empty = 15;
            // 차트 페이지 이동 버튼
            btnPageNext.Location = new Point(lblGridViewTitle.Location.X + lblGridViewTitle.Width - btnPageNext.Width, panelChart.Location.Y - empty - btnPageNext.Height);
            lblTotalPage.Location = new Point(btnPageNext.Location.X - empty - lblTotalPage.Width, panelChart.Location.Y - empty - lblTotalPage.Height - 4);
            btnPageBefore.Location = new Point(lblTotalPage.Location.X - empty - btnPageBefore.Width, panelChart.Location.Y - empty - btnPageBefore.Height);
            btnSaveFile.Location = new Point(btnPageBefore.Location.X - empty - empty - btnSaveFile.Width, panelChart.Location.Y - empty - btnSaveFile.Height);
            eleReportType.Location = new Point(btnSaveFile.Location.X - empty - empty - eleReportType.Width, panelChart.Location.Y - empty - eleReportType.Height);
        }

        private void cbType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DisasterTypeItem type = m_cbType.customComboBox.Items[m_cbType.customComboBox.SelectedIndex] as DisasterTypeItem;
            if (type == null)
                return;

            m_curFacilityType = type.Type;
            InitGridView();

            LoadData();
            CreateBarChart();
        }

        private void cbBuilding_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_cbFloor.customComboBox.Items.Clear();

            object obj = m_cbBuilding.customComboBox.Items[m_cbBuilding.customComboBox.SelectedIndex];
            Type type = obj.GetType();

            if (type == typeof(Building))
            {
                Building building = (Building)obj;
                m_cbFloor.customComboBox.Items.Add("전체");

                ArrayList arrFloor = (ArrayList)building.FloorList.Clone();
                foreach (Zone floor in arrFloor)
                {
                    m_cbFloor.customComboBox.Items.Add(floor.Floor);
                }
            }
            else
            {
                m_cbFloor.customComboBox.Items.Add("-");
            }

            if (m_cbFloor.customComboBox.Items.Count > 0)
                m_cbFloor.customComboBox.SelectedIndex = 0;
        }

        private void cbReportType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            CreateBarChart();
        }

        private string m_strSearchDate = "";
        private string m_strSearchZone = "";

        private void LoadData()
        {
            dataGridView1.Rows.Clear();

            ArrayList arrSelectZoneList = new ArrayList();

            Building building = m_cbBuilding.customComboBox.SelectedItem as Building;
            if (building == null)
                return;

            string strFloor = "";

            if (building.BuildingName == "전체")
                arrSelectZoneList = ZoneManager.Instance.FindZoneList("모든 건물 그룹", "모든 건물", "모든 층");
            else
            {
                Floor floor = m_cbFloor.customComboBox.SelectedItem as Floor;
                if (floor == null)
                {
                    arrSelectZoneList = ZoneManager.Instance.FindZoneList(building.BuildingGroup.BuildingGroupName, building.BuildingName, "모든 층");
                    strFloor = "모든 층";
                }
                else
                {
                    arrSelectZoneList = ZoneManager.Instance.FindZoneList(building.BuildingGroup.BuildingGroupName, building.BuildingName, floor.Zone.ZoneName);
                    strFloor = floor.Zone.ZoneName;
                }
            }

            m_detectMgr.DataClear();
            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            m_detectMgr.ZoneSubmit(arrSelectZoneList, m_dtStartDate, m_dtEndDate, m_curFacilityType);

            m_strSearchDate = m_dtStartDate.ToString("yyyy-MM-dd") + " ~ " + m_dtEndDate.ToString("yyyy-MM-dd");
            if (building.BuildingName == "전체")
                m_strSearchZone = "모든 건물";
            else
                m_strSearchZone = building.BuildingName + " " + strFloor;

            int count = 0;

            foreach (MulFunctionLog mulFunctionLog in m_detectMgr.MulFunctionList)
            {
                Zone zone = mulFunctionLog.Zone;
                int nReactionCount = mulFunctionLog.ReactionCount;
                int nMulFunctionCount = mulFunctionLog.MulFunctionCount;
                int nFireCount = mulFunctionLog.FireCount;
                string strBuildingName = mulFunctionLog.BuildingName;
                string strFloorName = mulFunctionLog.FloorName;
                double nPercentMulFunction = mulFunctionLog.PercentMulFunction;
                int nNotProcss = mulFunctionLog.Notprocess;

                string strType = mulFunctionLog.DetectType;

                EquipmentZone equipZone = null;
                List<EquipmentZone> arEquipzone = ZoneManager.Instance.GetEquipmentZoneList(zone);
                if (arEquipzone != null && arEquipzone.Count > 0)
                    equipZone = (EquipmentZone)arEquipzone[0];

                int rowIndex = dataGridView1.Rows.Add();
                foreach (TypeColumns column in uFormReport.Instance.DicDefineColumns[m_curFacilityType][m_curReportType])
                {
                    switch (column.DefineColumn.ColumnName)
                    {
                        case "colNumber":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = ++count;
                            break;
                        case "colTypeName":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = strType;
                            break;
                        case "colBuildingName":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = strBuildingName;
                            break;
                        case "colFoor":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = strFloorName;
                            break;
                        case "colLocation":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = equipZone.DisplayText;
                            break;
                        case "colDetectCount":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = nReactionCount;
                            break;
                        case "colDisasterCount":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = nFireCount;
                            break;
                        case "colMalfunctionCount":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = nMulFunctionCount;
                            break;
                        case "colRecoveryCount":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = nNotProcss;
                            break;
                        case "colMalfunctionRate":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = nPercentMulFunction + "%";
                            break;
                    }
                }
            }
        }

        public void CreateBarChart()
        {
            if (m_cbReportType.customComboBox.SelectedIndex == 0)
                SetWeekChart();
            else if (m_cbReportType.customComboBox.SelectedIndex == 1)
                SetMonthChart();
            else if (m_cbReportType.customComboBox.SelectedIndex == 2)
                SetQuarterChart();
            else if (m_cbReportType.customComboBox.SelectedIndex == 3)
                SetYearChart();

            panelChart.Invalidate();
        }

        private string[] labels = null;

        double[] data0 = null;
        double[] data1 = null;
        double[] data2 = null;

        private void SetWeekChart()
        {
            DateTime dtNowDate = DateTime.Now;
            DateTime dtBeforeDate = DateTime.Now;

            string strNowDate = "";
            string strBeforeDate = "";

            strNowDate = string.Format("{0} {1}:{2}:{3}", m_dtEndDate.ToString(), 23, 59, 59);
            strBeforeDate = string.Format("{0} {1}:{2}:{3}", m_dtStartDate.ToString(), 00, 00, 00);
            
            //두 날짜의 달 차이 계산
            //int n_Monthts = 12 * (dtEnd.Year - dtStart.Year) + (dtEnd.Month - dtStart.Month);

            //1년에 55주

            TimeSpan Subdt = DateTime.Now - m_dtStartDate;

            string strWeekday = m_dtStartDate.DayOfWeek.ToString();
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
                int nFireCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

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
                        int aweekNumber = cultureInfo.Calendar.GetWeekOfYear(m_dtStartDate, dateTimeFormatInfo.CalendarWeekRule, dateTimeFormatInfo.FirstDayOfWeek);

                        int nWeekend = 0;

                        if (strDateTime.Year - m_dtStartDate.Year > 0)
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

                        if (bFind)
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
            
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
        }

        private void SetMonthChart()
        {
            //현재날짜
            DateTime dt = DateTime.Now;

            DateTime Old = dt.AddMonths(-1);
            string str = Old.DayOfWeek.ToString();

            DateTime dtStart = m_dtStartDate;
            DateTime dtEnd = m_dtEndDate;
            
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

                foreach (KeyValuePair<MulFunctionLog, ArrayList> pair in m_detectMgr.DicMulFuctionSrLog)
                {
                    ArrayList arrSensorReaction = pair.Value;
                    MulFunctionLog mullog = pair.Key;

                    foreach (Report.SensorReactionLog log in arrSensorReaction)
                    {
                        string nMonth = log.Time.ToShortDateString().Substring(0, 7);
                        int nZoneID = log.Param1;
                        int nReaction = mullog.ReactionCount;
                        int nMulFunction = mullog.MulFunctionCount;
                        int nFire = mullog.FireCount;
                        int nOnlyDetect = mullog.OnlyDetectCount;
                        
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
            
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
        }

        private void SetQuarterChart()
        {
            DateTime dtStart = m_dtStartDate;
            DateTime dtEnd = m_dtEndDate;

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
                int nFireCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

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
            
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
        }

        private void SetYearChart()
        {
            DateTime dtStart = m_dtStartDate;
            DateTime dtEnd = m_dtEndDate;
            
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
                int nReactionCount = 0;
                int nFireCount = 0;
                int nMulFunctionCount = 0;
                int nOnlyDetectCount = 0;

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
                        int nOnlyDetect = mullog.OnlyDetectCount;
                        
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
            
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
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

        private Pen m_penRect = new Pen(Color.FromArgb(196, 196, 196));
        private Brush brushBg = new SolidBrush(Color.FromArgb(228, 231, 243));
        private Brush brushOrange = new SolidBrush(Color.FromArgb(0xff, 0xdd, 0x85, 0x09));
        private Brush brushRed = new SolidBrush(Color.FromArgb(0xff, 0xdc, 0x00, 0x00));
        private Brush brushBlue = new SolidBrush(Color.FromArgb(0xff, 0x0e, 0x8b, 0xe1));
        private Brush brushGray = new SolidBrush(Color.FromArgb(0xff, 0xd1, 0xd0, 0xce));

        private Brush brush2 = new SolidBrush(Color.FromArgb(37, 49, 80));
        private Font m_chartFont = new System.Drawing.Font("나눔바른고딕", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private void panelChart_Paint(object sender, PaintEventArgs e)
        {
            abc();

            e.Graphics.Clear(Color.FromArgb(228, 231, 243));

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //g.DrawRectangle(m_penRect, 0, 0, panelChart.Width - 1, panelChart.Height - 1); // panel 테두리

            int nBigRectSize = 140;
            int nMediumRectSize = 83;

            int nTopEmpty = 40;
            int nEmpty = 8;
            int nSpace = 8; // 한개 한개 간격
            
            Size RectSize = new System.Drawing.Size(450, 192);
            Size PanelSize = new System.Drawing.Size();

            int ncenter = (panelChart.Width / 2) - ((RectSize.Width * 3 + nSpace * 2) / 2);

            Point beginPT = new Point(ncenter, 0);
            Point drawPT = beginPT;

            int nRectCount = 1;

            for (int i = 0; i < dd.Length; i++)
            {
                if (nRectCount == 7)
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

                SizeF size = g.MeasureString(dd2[0][i] + "%", m_chartFont);
                //g.FillRectangle(brushBg, new Rectangle(drawPT.X, drawPT.Y, RectSize.Width, RectSize.Height));
                g.DrawRectangle(m_penRect, new Rectangle(drawPT.X, drawPT.Y, RectSize.Width, RectSize.Height));
                g.FillPie(brushGray, RectGray1, 0.0f, 360.0f);
                g.FillPie(brushOrange, RectRed1, -90.0f, value0);
                g.FillPie(brushBg, RectSmall1, 0.0f, 360.0f);
                g.DrawString(dd2[0][i] + "%", m_chartFont, brush2, RectGray1.X + RectGray1.Width - (RectGray1.Width / 2) - (size.Width / 2), RectGray1.Y + RectGray1.Height - (int)(RectGray1.Height * 0.5) - 8);

                size = g.MeasureString(dd2[1][i] + "%", m_chartFont);
                g.FillPie(brushGray, RectGray2, 0.0f, 360.0f);
                g.FillPie(brushRed, RectRed2, -90.0f, value1);
                g.FillPie(brushBg, RectSmall2, 0.0f, 360.0f);
                g.DrawString(dd2[1][i] + "%", m_chartFont, brush2, RectGray2.X + RectGray2.Width - (RectGray2.Width / 2) - (size.Width / 2), RectGray2.Y + RectGray2.Height - (int)(RectGray2.Height * 0.5) - 8);

                size = g.MeasureString(dd2[2][i] + "%", m_chartFont);
                g.FillPie(brushGray, RectGray3, 0.0f, 360.0f);
                g.FillPie(brushBlue, RectRed3, -90.0f, value2);
                g.FillPie(brushBg, RectSmall3, 0.0f, 360.0f);
                g.DrawString(dd2[2][i] + "%", m_chartFont, brush2, RectGray3.X + RectGray3.Width - (RectGray3.Width / 2) - (size.Width / 2), RectGray3.Y + RectGray3.Height - (int)(RectGray3.Height * 0.5) - 8);

                size = g.MeasureString(dd[i], m_chartFont);
                g.DrawString(dd[i], m_chartFont, brush2, drawPT.X + RectSize.Width - (RectSize.Width / 2) - (size.Width / 2), drawPT.Y + 15);

                if (drawPT.X + RectSize.Width > PanelSize.Width)
                    PanelSize.Width = drawPT.X + RectSize.Width;
                if (drawPT.Y + RectSize.Height > PanelSize.Height)
                    PanelSize.Height = drawPT.Y + RectSize.Height;

                if (nRectCount % 3 == 0)
                {
                    drawPT = new Point(beginPT.X, drawPT.Y + RectSize.Height + nSpace);
                }
                else
                    drawPT = new Point(drawPT.X + RectSize.Width + nSpace, drawPT.Y);

                nRectCount++;
            }
        }

        public void ControllCapture()
        {
            Bitmap bmp = new Bitmap(panelChart.Width, panelChart.Height);
            panelChart.DrawToBitmap(bmp, new Rectangle(0, 0, panelChart.Width, panelChart.Height));

            var gg = Graphics.FromImage(bmp);
            var rect = panelChart.RectangleToScreen(panelChart.ClientRectangle);

            bmp.Save(Application.StartupPath + "\\report\\Malfunction.bmp");
        }

        public void SetHwpData()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveDateTime.txt"))
            {
                file.WriteLine(m_strSearchDate);
                file.WriteLine(m_strSearchZone);
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

        private ArrayList SaveArr = new ArrayList();
        private void SaveHwpCrtl()
        {
            // 한글파일 출력전에 데이터를 저장하도록 함.

            SaveArr.Clear();

            for (int index = 0; index < dataGridView1.RowCount; index++)
            {
                DataGridViewRow row = dataGridView1.Rows[index];

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (dataGridView1.Columns[row.Cells[i].ColumnIndex].CellType.Name == "DataGridViewImageCell")
                        SaveArr.Add("");
                    else
                        SaveArr.Add(row.Cells[i].Value.ToString());
                }
            }
        }

        #region 버튼 이벤트
        public void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
            CreateBarChart();
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            btnSaveFile.Enabled = false;
            bool isHwpSetup = uFormReport.Instance.IsHwpSetup();

            string curType = IFacility.GetFacilityTypeString(m_curFacilityType);
            if (m_curFacilityType == IFacility.FacilityType.PSM_SENSOR)
            {
                if (UnE.SOP.ProxySOP.Instance.SiteID == 201)
                    curType = Data.CommonString.POI_Gas_Kor;
                else
                    curType = "누출";
            }

            string strSavePath = uFormReport.Instance.GetHWPFilePath(curType + "_처리이력_보고서", isHwpSetup);
            if (strSavePath == null)
                return;

            ControllCapture();
            FileWriter();
            SetHwpData();

            System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
            info.CreateNoWindow = true;
            info.Arguments = (int)m_curReportType + " " + (int)m_curFacilityType + " " + strSavePath + " " + uFormReport.Instance.StrLogoFileName + " " + UnE.SOP.ProxySOP.Instance.SiteID;
            info.FileName = Application.StartupPath + "\\HmlReport.exe";

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo = info;

            process.Start();
            this.Cursor = Cursors.WaitCursor;

            int nCount = 0;
            bool bSuccess = true;
            while (process.HasExited == false)
            {
                process.WaitForExit(500);

                if (30 == nCount)
                {
                    process.Kill();
                    MessageBox.Show("오류 발생");
                    bSuccess = false;
                    break;
                }
            }

            if (bSuccess == true)
            {
                if (isHwpSetup)
                    uFormReport.Instance.RunHWP(strSavePath);
                else
                {
                    int nIndex = strSavePath.LastIndexOf(@"\");
                    string filePath = strSavePath.Substring(0, nIndex);
                    System.Diagnostics.Process.Start(filePath);
                }
            }

            this.Cursor = Cursors.Default;
            btnSaveFile.Enabled = true;
        }

        private void btnPageBefore_Click(object sender, EventArgs e)
        {
            if (m_nCurrentPage == 1)
                return;

            m_nCurrentPage--;

            panelChart.Invalidate();
        }

        private void btnPageNext_Click(object sender, EventArgs e)
        {
            if (m_nCurrentPage == m_nTotalPage)
                return;

            m_nCurrentPage++;

            panelChart.Invalidate();
        }

        private void btnDateStart_Click(object sender, EventArgs e)
        {
            dateTimePicker2.Visible = false;

            dateTimePicker1.Value = Convert.ToDateTime(lblDateStart.Text);
            int x = lblDateStart.Location.X;
            int y = lblDateStart.Location.Y + lblDateStart.Height - dateTimePicker1.Height;

            dateTimePicker1.SendToBack();
            dateTimePicker1.Location = new Point(x, y);
            dateTimePicker1.DropDownAlign = LeftRightAlignment.Left;
            dateTimePicker1.Show();
            dateTimePicker1.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void btnDateEnd_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Visible = false;

            dateTimePicker2.Value = Convert.ToDateTime(lblDateEnd.Text);
            int x = lblDateEnd.Location.X;
            int y = lblDateEnd.Location.Y + lblDateEnd.Height - dateTimePicker2.Height;

            dateTimePicker2.SendToBack();
            dateTimePicker2.Location = new Point(x, y);
            dateTimePicker2.DropDownAlign = LeftRightAlignment.Left;
            dateTimePicker2.Show();
            dateTimePicker2.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            lblDateStart.Text = dateTimePicker1.Value.ToString("yyyy-MM-dd");
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            lblDateEnd.Text = dateTimePicker2.Value.ToString("yyyy-MM-dd");
        }

        #endregion
    }
}
