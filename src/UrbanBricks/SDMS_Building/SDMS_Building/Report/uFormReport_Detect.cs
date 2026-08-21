using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Spatial;
using System.Collections;
using UnE.Sensor;
using SDMS;
using SDMS_Building.Data;
using SDMS_Building.PopupDialog;
using SDMS_Building.Report.ReportPopup;

namespace SDMS_Building.Report
{
    public partial class uFormReport_Detect : UserControl
    {
        private Font m_fontRegular = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        private Brush m_brForeColor = new SolidBrush(Color.FromArgb(0x53, 0x65, 0x96));

        private UEWpfControl.WpfComboBox m_cbType = null; // 유형 combobox
        private UEWpfControl.WpfComboBox m_cbBuilding = null; // 빌딩 combobox
        private UEWpfControl.WpfComboBox m_cbFloor = null; // 층 combobox
        private UEWpfControl.WpfComboBox m_cbLevel = null; // 알람 단계 combobox
        private UEWpfControl.WpfComboBox m_cbUnit = null; // 단위 combobox

        private IFacility.FacilityType m_curFacilityType = IFacility.FacilityType.FIRE_SENSOR;
        private ReportType m_curReportType = ReportType.Detect;
        public IFacility.FacilityType CurFacilityType
        {
            get
            {
                return m_curFacilityType;
            }
        }

        private Dictionary<IFacility.FacilityType, List<TypeColumns>> m_dicTypeColumns = new Dictionary<IFacility.FacilityType, List<TypeColumns>>();

        private ReactionManager m_detectMgr = null;

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

        private Dictionary<int, Report.DetectLog> m_dicDetectLog = null;
        
        #region 그래프 옵션
        private int m_nSplitUnitOfMeansure = 2;
        private int m_nSplitUnitOfMeansureDetail = 1;
        private int m_nViewCount = 8;
        private int m_nCurrentPage = -1;
        private int m_nTotalPage = -1;

        private Dictionary<int, string[]> dicLabels = new Dictionary<int, string[]>();
        private Dictionary<int, DateTime[]> dicStartDetectDates = new Dictionary<int, DateTime[]>();
        private Dictionary<int, DateTime[]> dicEndDetectDates = new Dictionary<int, DateTime[]>();
        #endregion

        public uFormReport_Detect(ReactionManager detectMgr)
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(dataGridView1, true);
            FormMain.SetDoubleBuffer(panelChart, true);

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

            m_cbUnit = new UEWpfControl.WpfComboBox();
            eleUnit.Child = m_cbUnit;
            m_cbUnit.SetSize(eleUnit.Width, eleUnit.Height);
            m_cbUnit.customComboBox.SelectionChanged += cbUnit_SelectionChanged;

            txtUnitDetail.Text = "1"; // default 조회 단위 1일
        }

        private void cbType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DisasterTypeItem type = m_cbType.customComboBox.Items[m_cbType.customComboBox.SelectedIndex] as DisasterTypeItem;
            if (type == null)
                return;

            m_curFacilityType = type.Type;
            InitGridView();

            LoadData();
        }

        private void InitPosition(bool unitControlVisible)
        {
            int empty = 10;
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

            // 단위
            //if (unitControlVisible)
            //{
                lblUnit.Location = new Point(lblDateEnd.Location.X + lblDateEnd.Width + empty, 61);
                eleUnit.Location = new Point(lblUnit.Location.X + lblUnit.Width, 61);
                pnUnitDetail.Location = new Point(eleUnit.Location.X + eleUnit.Width + 5, 62);
                lblUnitDetail.Location = new Point(pnUnitDetail.Location.X + pnUnitDetail.Width, 80);

                // 위치
                lblLoaction.Location = new Point(lblUnitDetail.Location.X + lblUnitDetail.Width + empty, 61);
            //}
            //else
            //{
            //    // 위치
            //    lblLoaction.Location = new Point(lblDateEnd.Location.X + lblDateEnd.Width + empty, 61);
            //}
            eleBuilding.Location = new Point(lblLoaction.Location.X + lblLoaction.Width, 60);
            eleFloor.Location = new Point(eleBuilding.Location.X + eleBuilding.Width, 60);

            // 단계
            //lblLevel.Location = new Point(eleFloor.Location.X + eleFloor.Width + empty, 61);
            //eleLevel.Location = new Point(lblLevel.Location.X + lblLevel.Width, 60);

            // 검색
            btnSearch.Location = new Point(eleFloor.Location.X + eleFloor.Width + empty, 61);
            
            panelChart.Location = new Point(50, 200);
            panelChart.Size = new Size(panelChart.Width, 210);
            lblGridViewTitle.Location = new Point(panelChart.Location.X, panelChart.Location.Y + panelChart.Height + 20);
            dataGridView1.Location = new Point(lblGridViewTitle.Location.X, lblGridViewTitle.Location.Y + lblGridViewTitle.Height);
            dataGridView1.Size = new Size(dataGridView1.Width, 425);
        }

        private void uFormReport_Detect_Load(object sender, EventArgs e)
        {
            InitPosition(false);
            
            InitType();
            InitBuildingComboBox();
            
            LoadData();
        }

        private void LoadData()
        {
            this.Cursor = Cursors.WaitCursor;

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
            //찾은 검색결과를 DataGrid로 출력
            Load_DataGrid();
            m_nSplitUnitOfMeansure = m_cbUnit.customComboBox.SelectedIndex;
            m_nSplitUnitOfMeansureDetail = Convert.ToInt32(txtUnitDetail.Text);

            //그래프 그리기 
            CreateChart();
            m_strSearchDate = m_dtStartDate.ToString("yyyy-MM-dd") + " ~ " + m_dtEndDate.ToString("yyyy-MM-dd");
            if (building.BuildingName == "전체")
                m_strSearchZone = "모든 건물";
            else
                m_strSearchZone = building.BuildingName + " " + strFloor;

            this.Cursor = Cursors.Default;
        }

        private string m_strSearchDate = "";
        private string m_strSearchZone = "";
        private void InitGridView()
        {
            panelChart.Invalidate();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            if (!uFormReport.Instance.DicDefineColumns.ContainsKey(m_curFacilityType))
            {
                MessageBox.Show(IFacility.GetFacilityTypeString(m_curFacilityType) + " column 정보가 없음");
                m_cbType.customComboBox.SelectedIndex = 0; // 화재로 넘김

                return;
            }

            foreach (TypeColumns item in uFormReport.Instance.DicDefineColumns[m_curFacilityType][m_curReportType])
            {
                AddColumn(item.DefineColumn.ColumnName, item.HeaderText, item.ColumnWidthRatio);
            }
        }

        private void ResizeColumnWidth()
        {
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

        private void AddColumn(string columnName, string headerText, int columnWidthPer)
        {            
            if (columnName == "colViewDetail")
            {
                DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                imageColumn.Name = columnName;
                imageColumn.HeaderText = headerText;
                imageColumn.Image = SDMS_Building.Properties.Resources.detailView_normal;
                imageColumn.ImageLayout = DataGridViewImageCellLayout.NotSet;
                dataGridView1.Columns.Add(imageColumn);
                
            }
            else
                dataGridView1.Columns.Add(columnName, headerText);

            dataGridView1.Columns[columnName].SortMode = DataGridViewColumnSortMode.NotSortable;

            // 전체값 * 퍼센트 / 100
            int per = dataGridView1.Width * columnWidthPer / 100;
            dataGridView1.Columns[columnName].Width = per;
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

        private void InitType()
        {            
            m_cbType.customComboBox.DisplayMemberPath = "DisplayText";

            List<DisasterTypeItem> items = new List<DisasterTypeItem>();
            items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.FIRE_SENSOR, DisplayText = Data.CommonString.POI_Fire_Kor });

            if (UnE.SOP.ProxySOP.Instance.UsePSM)
            {
                string txt = "누출";
                if (UnE.SOP.ProxySOP.Instance.SiteID == 201)
                    txt = Data.CommonString.POI_Gas_Kor;
                items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.PSM_SENSOR, DisplayText = txt });
            }
            //if (UnE.SOP.ProxySOP.Instance.UseDoor)
            //    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.DOOR, DisplayText = Data.CommonString.POI_Door_Kor });            
            //if (UnE.SOP.ProxySOP.Instance.UseFirewall)
            //    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.FIREWALL, DisplayText = Data.CommonString.POI_FireWall_Kor });
            if (UnE.SOP.ProxySOP.Instance.UseBlackout)
                items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.BLACKOUT, DisplayText = Data.CommonString.POI_Blackout_Kor });
            if (UnE.SOP.ProxySOP.Instance.UseStrongWind)
                items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.STRONG_WIND, DisplayText = Data.CommonString.POI_StrongWind_Kor });
            if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
                items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.Earthquake, DisplayText = Data.CommonString.POI_Earthquake_Kor });
            if (UnE.SOP.ProxySOP.Instance.UseTerror)
                items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.TERROR, DisplayText = Data.CommonString.POI_Terror_Kor });
            if (UnE.SOP.ProxySOP.Instance.UseSubmergency)
                items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.SUBMERGENCY, DisplayText = Data.CommonString.POI_Submergency_Kor });

            m_cbType.customComboBox.ItemsSource = items;

            if (m_cbType.customComboBox.Items.Count > 0)
                m_cbType.customComboBox.SelectedIndex = 0;

            m_cbLevel.customComboBox.Items.Add("관심");
            m_cbLevel.customComboBox.Items.Add("주의");
            m_cbLevel.customComboBox.Items.Add("경계");
            m_cbLevel.customComboBox.Items.Add("심각");
            m_cbLevel.customComboBox.SelectedIndex = 1;

            m_cbUnit.customComboBox.Items.Add("분");
            m_cbUnit.customComboBox.Items.Add("시");
            m_cbUnit.customComboBox.Items.Add("일");
            m_cbUnit.customComboBox.Items.Add("주");
            m_cbUnit.customComboBox.Items.Add("월");
            m_cbUnit.customComboBox.Items.Add("연");
            m_cbUnit.customComboBox.SelectedIndex = 2; // default 조회 단위 일
        }

        public void Load_DataGrid()
        {
            dataGridView1.Rows.Clear();
            
            if (m_dicDetectLog == null)
                m_dicDetectLog = new Dictionary<int, DetectLog>();
            else
                m_dicDetectLog.Clear();

            //SaveArr.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Invalidate();
            
            // SensorHistoryData List
            ArrayList arrSensorZoneHistory = null;//new ArrayList();
            arrSensorZoneHistory = m_detectMgr.DectectList;
            
            // 고속 처리를 위하여 Rows.Add() 대신 AddCopies()를 사용한다.
            int nRowCount = arrSensorZoneHistory.Count;
            int count = 0;

            // Key : SensorReactionHistory ID
            // Value : SensorZone ID
            Dictionary<int, int> dicSensorZoneIDs = GetSensorReactionLogs();

            foreach (Report.DetectLog historyData in from historyData in arrSensorZoneHistory.Cast<Report.DetectLog>()
                                                     orderby historyData.Time descending
                                                     select historyData
                                                     )
            {
                Zone zoneLink = ZoneManager.Instance.GetZone(historyData.zoneID);
                //if (zoneLink == null)
                //{
                //    dataGridView1.Rows.RemoveAt(--nRowCount);
                //    continue;
                //}
                                
                string szBuildingName = (zoneLink != null && zoneLink.Building != null) ? zoneLink.Building.DisplayText : "";
                string szGroupName = szBuildingName != "" ? zoneLink.Building.BuildingGroup.BuildingGroupName : "";

                string strDetectionStatusName = historyData.DetectionStatusName;
                
                //외부공간은 건물그룹과 건물이 없기 때문에 따로 설정..
                if (szGroupName == "")
                    szGroupName = "외부 영역";
                if (szBuildingName == "" && zoneLink == null)
                    szBuildingName = "모든 건물";
                else if (szBuildingName == "" && zoneLink != null)
                    szBuildingName = zoneLink.ZoneName;

                string strFloorIndex = (zoneLink != null && zoneLink.Floor != null) ? zoneLink.Floor.ToString() : "";
                string strType = "";
                
                string equipZoneName = "";

                EquipmentZone equipZone = null;

                strType = historyData.DetectType;
                if (strType == "자탐 센서" || strType == "화재 센서")
                {
                    equipZone = historyData.EquipZone;

                    if (equipZone != null)
                        equipZoneName = equipZone.DisplayText;
                }
                else // 수동신고
                {
                    if (zoneLink == null)
                        equipZoneName = "모든 영역";
                    else
                        equipZoneName = zoneLink.ZoneName;
                }
                
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

                string strMemo = "-";
                if (CurFacilityType == IFacility.FacilityType.Earthquake || CurFacilityType == IFacility.FacilityType.STRONG_WIND)
                    strMemo = GetEarthquakeMemo(historyData.SensorReactionHistoryID);

                int rowIndex = dataGridView1.Rows.Add();
                foreach (TypeColumns column in uFormReport.Instance.DicDefineColumns[m_curFacilityType][m_curReportType])
                {                    
                    switch (column.DefineColumn.ColumnName)
                    {
                        case "colNumber":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = ++count;
                            break;
                        case "colDate":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = historyData.Time;
                            break;
                        case "colTypeName":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = strType;
                            break;
                        case "colSensorName":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = strSensorName;
                            break;
                        case "colBuildingName":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = szBuildingName;
                            break;
                        case "colFoor":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = strFloorIndex;
                            break;
                        case "colLocation":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = equipZoneName;
                            break;
                        case "colStatus":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = strDetectionStatusName;
                            break;
                        case "colChangeAlarmDepth":
                            dataGridView1.Rows[rowIndex].Cells[column.DefineColumn.ColumnName].Value = strMemo;
                            break;
                    }
                }

                dataGridView1.Rows[rowIndex].Tag = historyData;
                
                m_dicDetectLog.Add(count, historyData);
            }
        }

        public void CreateChart()
        {
            dicLabels.Clear();
            dicStartDetectDates.Clear();
            dicEndDetectDates.Clear();

            DateTime StartDate = m_dtStartDate;
            DateTime EndDate = m_dtEndDate;

            if (String.Equals(EndDate.ToShortDateString(), DateTime.Now.ToShortDateString()))
            {
                EndDate = DateTime.Now;
            }
            else
            {
                EndDate = EndDate.AddDays(1).AddSeconds(-1);
            }

            DateTime dtMinDate = m_dtStartDate;
            DateTime dtMaxDate = m_dtStartDate;
            
            switch (m_nSplitUnitOfMeansure)
            {
                case 0:// 분
                    dtMaxDate = dtMaxDate.AddMinutes(m_nSplitUnitOfMeansureDetail);
                    break;
                case 1:// 시
                    dtMaxDate = dtMaxDate.AddHours(m_nSplitUnitOfMeansureDetail);
                    break;
                case 2:// 일
                    dtMaxDate = dtMaxDate.AddDays(m_nSplitUnitOfMeansureDetail);
                    break;
                case 3:// 주
                    dtMaxDate = dtMaxDate.AddDays((m_nSplitUnitOfMeansureDetail * 7) - (int)dtMinDate.DayOfWeek);
                    break;
                case 4:// 월
                    dtMaxDate = new DateTime(dtMaxDate.Year, dtMaxDate.Month, 1).AddMonths(m_nSplitUnitOfMeansureDetail);
                    break;
                case 5:// 연
                    dtMaxDate = new DateTime(dtMaxDate.Year, 1, 1).AddYears(m_nSplitUnitOfMeansureDetail);
                    break;
            }

            if (dtMaxDate > EndDate)
            {
                dtMaxDate = EndDate;
            }

            string strXDate = uFormReport.GetDateTimeParsing(dtMinDate, m_nSplitUnitOfMeansure);

            int nCount = (from logs in m_detectMgr.DectectList.ToArray().Cast<Report.DetectLog>()
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

                strXDate = AddDate(ref dtMinDate, ref dtMaxDate, EndDate, m_nSplitUnitOfMeansure, m_nSplitUnitOfMeansureDetail);

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
            
            panelChart.Invalidate();
        }

        List<double> m_chartDatas = null;
        private void SetChart()
        {
            m_chartDatas = new List<double>();

            // 데이터 수량 계산
            for (int nIndex = 0; nIndex < dicStartDetectDates[m_nCurrentPage].Length; nIndex++)
            {
                m_chartDatas.Add((from logs in m_detectMgr.DectectList.ToArray().Cast<Report.DetectLog>()
                                  where logs.Time >= dicStartDetectDates[m_nCurrentPage][nIndex]
                                  && logs.Time < dicEndDetectDates[m_nCurrentPage][nIndex]
                                  select logs).Count());
            }
        }

        private Brush m_brSmall = new SolidBrush(Color.FromArgb(228, 231, 243));
        private Brush m_brRed = new SolidBrush(Color.FromArgb(0xef, 0x57, 0x57));
        private Brush m_brBig = new SolidBrush(Color.White);

        private Pen m_penSmall = new Pen(Color.FromArgb(228, 231, 243));
        private Pen m_penRed = new Pen(Color.FromArgb(0xef, 0x57, 0x57));
        private Pen m_penBig = new Pen(Color.White);

        private Brush m_foreColor = new SolidBrush(Color.FromArgb(0x25, 0x31, 0x50));
        private Pen m_penRect = new Pen(Color.FromArgb(196, 196, 196));

        private void panelChart_Paint(object sender, PaintEventArgs e)
        {
            SetChart();

            e.Graphics.Clear(Color.FromArgb(228, 231, 243));

            if (m_chartDatas == null || m_chartDatas.Count == 0)
                return;
            
            int nTotalEmpty = panelChart.Width * 10 / 100; // 빈 영역의 총 Width
            int nRectBig = (panelChart.Width - nTotalEmpty) / 8;
            int nRectOutsidePie = nRectBig * 80 / 100;
            int nRectInsidePie = nRectBig * 60 / 100;

            int nTopEmpty = panelChart.Height * 20 / 100;
            int nEmpty = nRectBig * 10 / 100;

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
            g.DrawRectangle(m_penRect, 0, 0, panelChart.Width - 1, panelChart.Height - 1); // panel 테두리
            
            Point drawPT = new Point((panelChart.Width - (nRectBig * 8) - (nEmpty * 7)) / 2, 0);

            int nRectCount = 1;

            double nTotalCnt = 0;
            for (int i = 0; i < m_chartDatas.Count; i++)
            {
                if (nRectCount == 9) // 8개만 보여줌
                    break;

                double cnt = Convert.ToDouble(m_chartDatas[i]);
                nTotalCnt = nTotalCnt + cnt;
                nRectCount++;
            }

            nRectCount = 1;

            for (int i = 0; i < m_chartDatas.Count; i++)
            {
                if (nRectCount == 9) // 8개만 보여줌
                    break;
                
                Rectangle RectValue = new Rectangle(drawPT.X + nEmpty, nTopEmpty, nRectOutsidePie, nRectOutsidePie);
                Rectangle RectOutside = new Rectangle(drawPT.X + nEmpty, nTopEmpty, nRectOutsidePie, nRectOutsidePie);
                Rectangle RectInside = new Rectangle((int)(RectOutside.Width * 0.5 - nRectInsidePie * 0.5) + RectOutside.X, (int)(RectOutside.Width * 0.5 - nRectInsidePie * 0.5) + RectOutside.Y, nRectInsidePie, nRectInsidePie);

                double cnt = Convert.ToInt32(m_chartDatas[i]);
                float value = 0.0f;
                if (nTotalCnt > 0)
                    value = (float)cnt / (float)nTotalCnt * 360.0f;

                g.FillPie(m_brBig, RectOutside, 0.0f, 360.0f);                
                g.FillPie(m_brRed, RectValue, -90.0f, value); // -90 = 0도
                g.FillPie(m_brSmall, RectInside, 0.0f, 360.0f);

                SizeF fontSize = g.MeasureString(dicStartDetectDates[m_nCurrentPage][i].ToString(strDateFormat), m_fontRegular);
                g.DrawString(dicStartDetectDates[m_nCurrentPage][i].ToString(strDateFormat), m_fontRegular, m_foreColor, drawPT.X + nRectBig - (nRectBig / 2) - (fontSize.Width / 2), drawPT.Y + 12);
                fontSize = g.MeasureString(cnt + "회", m_fontRegular);                
                g.DrawString(cnt + "회", m_fontRegular, m_foreColor, drawPT.X + nEmpty + (RectOutside.Width / 2) - (fontSize.Width / 2), nTopEmpty + (RectOutside.Height / 2) - (fontSize.Height / 2));
                
                drawPT = new Point(drawPT.X + nRectBig + nEmpty, drawPT.Y);

                nRectCount++;
            }            
        }

        /// <summary>
        /// Key : SensorReactionHistory ID
        /// Value : SensorZone ID
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, int> GetSensorReactionLogs()
        {
            Dictionary<int, int> dicSensorReactionLogs = new Dictionary<int, int>();

            foreach (SensorReactionLog log in m_detectMgr.AllReactionLog)
            {
                int nSensorZoneID;

                if (int.TryParse(log.Param2, out nSensorZoneID))
                {
                    dicSensorReactionLogs[log.ID] = nSensorZoneID;
                }
            }

            return dicSensorReactionLogs;
        }

        private string GetEarthquakeMemo(int reactionID)
        {
            string strMemo = "-";

            foreach (SensorReactionLog log in m_detectMgr.AllReactionLog)
            {
                if (log.ID == reactionID)
                {
                    strMemo = log.Message.Replace("\r\n", " ");
                    break;
                }
            }

            return strMemo;
        }

        private void cbBuilding_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_cbFloor.customComboBox.Items.Clear();

            object obj = m_cbBuilding.customComboBox.Items[m_cbBuilding.customComboBox.SelectedIndex];
            Type type = obj.GetType();

            if (type == typeof(Building))
            {
                Building building = (Building)obj;
                //if (building.BuildingName == "전체" && building.ID == -1)
                //{
                    m_cbFloor.customComboBox.Items.Add("전체");
                //}
                //else
                //{
                    ArrayList arrFloor = (ArrayList)building.FloorList.Clone();

                    foreach (Zone floor in arrFloor)
                    {
                        m_cbFloor.customComboBox.Items.Add(floor.Floor);
                    }
                //}
            }
            else
            {
                m_cbFloor.customComboBox.Items.Add("-");
            }

            if (m_cbFloor.customComboBox.Items.Count > 0)
                m_cbFloor.customComboBox.SelectedIndex = 0;
        }

        private void cbUnit_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            m_nSplitUnitOfMeansure = m_cbUnit.customComboBox.SelectedIndex;

            lblUnitDetail.Text = m_cbUnit.customComboBox.SelectedItem.ToString() + " 마다";
        }

        private void uFormReport_Detect_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
        }

        #region DateTimePicker
        private void btnDateBefore_Click(object sender, EventArgs e)
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

        private void btnDateAfter_Click(object sender, EventArgs e)
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

        public void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void uFormReport_Detect_Resize(object sender, EventArgs e)
        {
            ResizeColumnWidth();

            int empty = 15;
            // 차트 페이지 이동 버튼
            btnPageNext.Location = new Point(panelChart.Location.X + panelChart.Width - btnPageNext.Width, panelChart.Location.Y - empty - btnPageNext.Height);
            lblTotalPage.Location = new Point(btnPageNext.Location.X - empty - lblTotalPage.Width, panelChart.Location.Y - empty - lblTotalPage.Height - 4);
            btnPageBefore.Location = new Point(lblTotalPage.Location.X - empty - btnPageBefore.Width, panelChart.Location.Y - empty - btnPageBefore.Height);
            btnSaveFile.Location = new Point(btnPageBefore.Location.X - empty - empty - btnSaveFile.Width, panelChart.Location.Y - empty - btnSaveFile.Height);

            if (this.Width == 1920) 
            {
                SetUnitControl(true);
            }
            else
            {
                SetUnitControl(false);
            }
        }

        /// <summary>
        /// 양쪽 슬라이드를 다 접었을 때는 단위, 날짜 형식 조건을 넣어준다
        /// </summary>
        /// <param name="visible"></param>
        private void SetUnitControl(bool visible)
        {
            //if (visible)
            {
                if (lblUnit.Visible && eleUnit.Visible && pnUnitDetail.Visible && lblUnitDetail.Visible)
                    return;

                lblUnit.Visible = eleUnit.Visible = pnUnitDetail.Visible = lblUnitDetail.Visible = true;

                InitPosition(true);
            }
            //else
            //{
            //    if (!lblUnit.Visible && !eleUnit.Visible && !pnUnitDetail.Visible && !lblUnitDetail.Visible)
            //        return;

            //    lblUnit.Visible = eleUnit.Visible = pnUnitDetail.Visible = lblUnitDetail.Visible = false;

            //    InitPosition(false);
            //}
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

                    break;
                case 1:// 시
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddHours(nAddSpacing);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }
                    
                    break;
                case 2:// 일
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddDays(nAddSpacing);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }
                    break;
                case 3:// 주
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddDays(nAddSpacing * 7);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }
                    
                    break;
                case 4:// 월
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddMonths(nAddSpacing);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }
                    
                    break;
                case 5:// 연
                    dtMinDate = dtMaxDate;
                    dtMaxDate = dtMaxDate.AddYears(nAddSpacing);

                    if (dtMaxDate > dtLastDate)
                    {
                        dtMaxDate = dtLastDate;
                    }
                    
                    break;
            }

            strReturn = uFormReport.GetDateTimeParsing(dtMinDate, nAddType);

            return strReturn;
        }

        private void btnPageBefore_Click(object sender, EventArgs e)
        {
            if (m_nCurrentPage == 1)
                return;

            m_nCurrentPage--;
            
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
            panelChart.Invalidate();
        }

        private void btnPageNext_Click(object sender, EventArgs e)
        {
            if (m_nCurrentPage == m_nTotalPage)
                return;

            m_nCurrentPage++;
            
            lblTotalPage.Text = String.Format("{1} / {0}", m_nTotalPage, m_nCurrentPage);
            panelChart.Invalidate();
        }

        private void btnUnitDetailUp_Click(object sender, EventArgs e)
        {
            int unit = Convert.ToInt32(txtUnitDetail.Text);
            unit++;
            txtUnitDetail.Text = unit.ToString();
        }

        private void btnUnitDetailDown_Click(object sender, EventArgs e)
        {
            int unit = Convert.ToInt32(txtUnitDetail.Text);
            unit--;
            txtUnitDetail.Text = unit.ToString();
        }

        private void txtUnitDetail_TextChanged(object sender, EventArgs e)
        {
            int unit = -1;
            if (!int.TryParse(txtUnitDetail.Text, out unit))
            {
                txtUnitDetail.TextChanged -= txtUnitDetail_TextChanged;
                FormMessageBox msgBox = new FormMessageBox("숫자만 입력하세요.", MessageBoxButtons.OK);
                msgBox.StartPosition = FormStartPosition.CenterParent;
                msgBox.ShowDialog();
                txtUnitDetail.Text = m_nSplitUnitOfMeansure.ToString();
                txtUnitDetail.TextChanged += txtUnitDetail_TextChanged;
                return;
            }
            else
                m_nSplitUnitOfMeansure = unit;
        }

        private void panelChart_Resize(object sender, EventArgs e)
        {
            panelChart.Invalidate();
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

            string strSavePath = uFormReport.Instance.GetHWPFilePath(curType + "_탐지이력_보고서", isHwpSetup);
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

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dataGridView1.Columns[e.ColumnIndex].Name != "colViewDetail")
                return;
            
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = SDMS_Building.Properties.Resources.detailView_click;
        }

        private void dataGridView1_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dataGridView1.Columns[e.ColumnIndex].Name != "colViewDetail")
                return;

            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = SDMS_Building.Properties.Resources.detailView_normal;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (dataGridView1.Columns[e.ColumnIndex].Name != "colViewDetail")
                return;

            DetectLog log = dataGridView1.Rows[e.RowIndex].Tag as DetectLog;
            if (log == null)
                return;

            PopupBackground back = new PopupBackground();
            back.StartPosition = FormStartPosition.Manual;
            back.Size = FormMain.Instance.Size;
            back.Location = FormMain.Instance.Location;
            back.Show();

            FormMain.Instance.PopDetailLog = new PopupDetailLog(m_detectMgr, log.HistoryID);
            FormMain.Instance.PopDetailLog.StartPosition = FormStartPosition.CenterParent;
            FormMain.Instance.PopDetailLog.ShowDialog();
            back.Close();
        }
    }

    public class DisasterTypeItem
    {
        private string m_strDisplayText = "";
        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        private IFacility.FacilityType m_type = IFacility.FacilityType.NONE;
        public IFacility.FacilityType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public override string ToString()
        {
            return m_strDisplayText;
        }
    }
}
