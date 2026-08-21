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
using SDMS_Building.Data;
using ChartDirector;
using UnE.GUI;

namespace SDMS_Building.Report
{
    public partial class uFormReport_Pareto : UserControl
    {
        private class PageData
        {
            private ImageButton btnPrevious = null;
            private ImageButton btnNext = null;
            private Label lblTotalPage = null;
            private int m_nCurrentPage = 1;
            private int m_nTotalPage = 1;

            private string m_strPageName = "";

            public ImageButton PreviousButton
            {
                get { return btnPrevious; }
                set { btnPrevious = value; }
            }

            public ImageButton NextButton
            {
                get { return btnNext; }
                set { btnNext = value; }
            }
            
            public Label TotalPageCountLabel
            {
                get { return lblTotalPage; }
                set { lblTotalPage = value; }
            }

            public int CurrentPage
            {
                get { return m_nCurrentPage; }
                set { m_nCurrentPage = value; }
            }

            public int TotalPage
            {
                get { return m_nTotalPage; }
                set { m_nTotalPage = value; }
            }

            public string PageName
            {
                get { return m_strPageName; }
                set { m_strPageName = value; }
            }

            public PageData()
            {
            }

            public PageData(ImageButton btnPrev, ImageButton btnNext, Label lblTotalPage, string pageName)
            {
                this.btnPrevious = btnPrev;
                this.btnNext = btnNext;
                this.lblTotalPage = lblTotalPage;
                this.PageName = pageName;
            }
        }

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
        private ReportType m_curReportType = ReportType.Pareto_Sensor;
        
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

        #region 그래프 옵션
        private int m_nViewCount = 10;
        
        private const string m_strChartTitle = "탐\r\n지\r\n횟\r\n수";
        #endregion

        private PageData m_pageSensor = null;
        private PageData m_pageEquipZone = null;

        // 센서별 알람 History Count
        private Dictionary<SensorTagInfo, int> m_dicSensorHistories = new Dictionary<SensorTagInfo, int>();

        // 위치별 알람 History Count
        private Dictionary<UnE.Spatial.EquipmentZone, int> m_dicEquipZoneHistories = new Dictionary<UnE.Spatial.EquipmentZone, int>();

        private bool m_systemCall = false;

        private string m_strSearchDate = "";
        private string m_strSearchZone = "";

        public uFormReport_Pareto(ReactionManager detectMgr)
        {
            InitializeComponent();
            
            m_detectMgr = detectMgr;

            this.DoubleBuffered = true;
            FormMain.SetDoubleBuffer(dataGridViewSensor, true);
            FormMain.SetDoubleBuffer(dataGridViewEquipZone, true);
            FormMain.SetDoubleBuffer(winChartViewerSensor, true);
            FormMain.SetDoubleBuffer(winChartViewerEquipZone, true);
            FormMain.SetDoubleBuffer(btnPageBefore, true);
            FormMain.SetDoubleBuffer(btnPageNext, true);
            FormMain.SetDoubleBuffer(lblTotalPage, true);
            FormMain.SetDoubleBuffer(lblType, true);

            lblDateStart.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            lblDateEnd.Text = DateTime.Now.ToString("yyyy-MM-dd");

            FormMain.Instance.CustomGridView(dataGridViewSensor, 12.0f, Color.FromArgb(0x25, 0x31, 0x50), Color.White, Color.FromArgb(0xf3, 0xf4, 0xfa), Color.FromArgb(0x25, 0x31, 0x50), DataGridViewContentAlignment.MiddleCenter);
            dataGridViewSensor.ScrollBars = ScrollBars.Vertical;
            FormMain.Instance.CustomGridView(dataGridViewEquipZone, 12.0f, Color.FromArgb(0x25, 0x31, 0x50), Color.White, Color.FromArgb(0xf3, 0xf4, 0xfa), Color.FromArgb(0x25, 0x31, 0x50), DataGridViewContentAlignment.MiddleCenter);
            dataGridViewEquipZone.ScrollBars = ScrollBars.Vertical;

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

            m_pageSensor = new PageData(btnPageBefore, btnPageNext, lblTotalPage, "센서별");
            m_pageEquipZone = new PageData(btnPageBefore, btnPageNext, lblTotalPage, "위치별");
        }
        
        private void GetSensorGridValues( out double[] values, out string[] labels)
        {
            int nRowCount = dataGridViewSensor.Rows.Count;

            if (nRowCount == 0)
            {
                values = null;
                labels = null;
                return;
            }
            else
            {
                values = new double[nRowCount];
                labels = new string[nRowCount];
            }

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridViewSensor.Rows[i];
                SensorTagHistoryCount history = (SensorTagHistoryCount)row.Tag;

                values[i] = history.HistoryCount;
                labels[i] = history.Sensor.TagName;
            }
        }

        private void GetEquipZoneGridValues(out double[] values, out string[] labels)
        {
            int nRowCount = dataGridViewEquipZone.Rows.Count;

            if (nRowCount == 0)
            {
                values = null;
                labels = null;
                return;
            }
            else
            {
                values = new double[nRowCount];
                labels = new string[nRowCount];
            }

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridViewEquipZone.Rows[i];
                EquipZoneHistoryCount history = (EquipZoneHistoryCount)row.Tag;

                values[i] = history.HistoryCount;
                labels[i] = history.EquipmentZone == null ? "-" : history.EquipmentZone.ZoneName;
            }
        }

        private void uFormReport_Pareto_Load(object sender, EventArgs e)
        {
            InitPosition();

            InitType();
            InitBuildingComboBox();

            LoadData();
        }

        private void uFormReport_Pareto_Resize(object sender, EventArgs e)
        {
            ResizeColumnWidth();
            InitPosition();

            // 차트 사이즈를 조절할 수 없어서 resize가 끝난 후 재 조회 한다
            if (this.Width == 1920 || this.Width == 1575)
            {
                SetData();
            }
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
            
            winChartViewerSensor.Location = new Point(50, 200);
            winChartViewerSensor.Size = new Size(lblGridViewTitle.Width, 310);
            if (winChartViewerSensor.Chart != null)
                winChartViewerSensor.Chart.setSize(winChartViewerSensor.Size.Width, winChartViewerSensor.Size.Height);

            winChartViewerEquipZone.Location = winChartViewerSensor.Location;
            winChartViewerEquipZone.Size = winChartViewerSensor.Size;
            if (winChartViewerEquipZone.Chart != null)
                winChartViewerEquipZone.Chart.setSize(winChartViewerEquipZone.Size.Width, winChartViewerEquipZone.Size.Height);

            lblGridViewTitle.Location = new Point(winChartViewerSensor.Location.X, winChartViewerSensor.Location.Y + winChartViewerSensor.Height + 20);

            dataGridViewSensor.Location = new Point(lblGridViewTitle.Location.X, lblGridViewTitle.Location.Y + lblGridViewTitle.Height);
            dataGridViewSensor.Size = new Size(dataGridViewSensor.Width, 250);
            dataGridViewEquipZone.Location = dataGridViewSensor.Location;
            dataGridViewEquipZone.Size = dataGridViewSensor.Size;

            empty = 15;
            // 차트 페이지 이동 버튼
            btnPageNext.Location = new Point(lblGridViewTitle.Location.X + lblGridViewTitle.Width - btnPageNext.Width, winChartViewerSensor.Location.Y - empty - btnPageNext.Height);
            lblTotalPage.Location = new Point(btnPageNext.Location.X - empty - lblTotalPage.Width, winChartViewerSensor.Location.Y - empty - lblTotalPage.Height - 4);
            btnPageBefore.Location = new Point(lblTotalPage.Location.X - empty - btnPageBefore.Width, winChartViewerSensor.Location.Y - empty - btnPageBefore.Height);
            btnSaveFile.Location = new Point(btnPageBefore.Location.X - empty - empty - btnSaveFile.Width, winChartViewerSensor.Location.Y - empty - btnSaveFile.Height);
            eleReportType.Location = new Point(btnSaveFile.Location.X - empty - empty - eleReportType.Width, winChartViewerSensor.Location.Y - empty - eleReportType.Height);
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
            //if (UnE.SOP.ProxySOP.Instance.UseDoor)
            //    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.DOOR, DisplayText = Data.CommonString.POI_Door_Kor });
            ////if (UnE.SOP.ProxySOP.Instance.UseEarthquake)
            ////    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.Earthquake, DisplayText = Data.CommonString.POI_Earthquake_Kor });
            //if (UnE.SOP.ProxySOP.Instance.UseFirewall)
            //    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.FIREWALL, DisplayText = Data.CommonString.POI_FireWall_Kor });
            //if (UnE.SOP.ProxySOP.Instance.UseBlackout)
            //    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.BLACKOUT, DisplayText = Data.CommonString.POI_Blackout_Kor });
            //if (UnE.SOP.ProxySOP.Instance.UseStrongWind)
            //    items.Add(new DisasterTypeItem() { Type = IFacility.FacilityType.STRONG_WIND, DisplayText = Data.CommonString.POI_StrongWind_Kor });

            m_cbType.customComboBox.ItemsSource = items;

            if (m_cbType.customComboBox.Items.Count > 0)
                m_cbType.customComboBox.SelectedIndex = 0;

            m_cbLevel.customComboBox.Items.Add("관심");
            m_cbLevel.customComboBox.Items.Add("주의");
            m_cbLevel.customComboBox.Items.Add("경계");
            m_cbLevel.customComboBox.Items.Add("심각");
            m_cbLevel.customComboBox.SelectedIndex = 1;

            m_cbReportType.customComboBox.Items.Add("센서별");
            m_cbReportType.customComboBox.Items.Add("위치별");
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
            if (!uFormReport.Instance.DicDefineColumns.ContainsKey(m_curFacilityType) || 
                !uFormReport.Instance.DicDefineColumns[m_curFacilityType].ContainsKey(ReportType.Pareto_Sensor) || 
                !uFormReport.Instance.DicDefineColumns[m_curFacilityType].ContainsKey(ReportType.Pareto_EquipZone))
            {                
                MessageBox.Show(IFacility.GetFacilityTypeString(m_curFacilityType) + " column 정보가 없음");
                m_cbType.customComboBox.SelectedIndex = 0; // 화재로 넘김

                return;
            }

            dataGridViewSensor.Rows.Clear();
            dataGridViewSensor.Columns.Clear();

            foreach (TypeColumns item in uFormReport.Instance.DicDefineColumns[m_curFacilityType][ReportType.Pareto_Sensor])
            {
                AddColumn(dataGridViewSensor, item.DefineColumn.ColumnName, item.HeaderText, item.ColumnWidthRatio);
            }

            dataGridViewEquipZone.Rows.Clear();
            dataGridViewEquipZone.Columns.Clear();

            foreach (TypeColumns item in uFormReport.Instance.DicDefineColumns[m_curFacilityType][ReportType.Pareto_EquipZone])
            {
                AddColumn(dataGridViewEquipZone, item.DefineColumn.ColumnName, item.HeaderText, item.ColumnWidthRatio);
            }
        }

        private void ResizeColumnWidth()
        {
            foreach (TypeColumns item in uFormReport.Instance.DicDefineColumns[m_curFacilityType][ReportType.Pareto_Sensor])
            {
                if (dataGridViewSensor.Columns.Count > 0)
                {
                    DataGridViewColumn obj = dataGridViewSensor.Columns[item.DefineColumn.ColumnName];
                    if (!dataGridViewSensor.Columns[item.DefineColumn.ColumnName].Visible)
                        continue;

                    // 전체값 * 퍼센트 / 100
                    int per = dataGridViewSensor.Width * item.ColumnWidthRatio / 100;
                    dataGridViewSensor.Columns[item.DefineColumn.ColumnName].Width = per;
                }
            }

            foreach (TypeColumns item in uFormReport.Instance.DicDefineColumns[m_curFacilityType][ReportType.Pareto_EquipZone])
            {
                if (dataGridViewEquipZone.Columns.Count > 0)
                {
                    DataGridViewColumn obj = dataGridViewEquipZone.Columns[item.DefineColumn.ColumnName];
                    if (!dataGridViewEquipZone.Columns[item.DefineColumn.ColumnName].Visible)
                        continue;

                    // 전체값 * 퍼센트 / 100
                    int per = dataGridViewEquipZone.Width * item.ColumnWidthRatio / 100;
                    dataGridViewEquipZone.Columns[item.DefineColumn.ColumnName].Width = per;
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

        private void cbType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            DisasterTypeItem type = m_cbType.customComboBox.Items[m_cbType.customComboBox.SelectedIndex] as DisasterTypeItem;
            if (type == null)
                return;

            m_curFacilityType = type.Type;
            InitGridView();

            m_pageSensor.CurrentPage = m_pageSensor.TotalPage = 1;
            m_pageEquipZone.CurrentPage = m_pageEquipZone.TotalPage = 1;

            LoadData();
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

        private void cbReportType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (m_cbReportType.customComboBox.SelectedIndex == 0)
            {
                dataGridViewSensor.Visible = true;
                dataGridViewEquipZone.Visible = false;

                winChartViewerSensor.Visible = true;
                winChartViewerEquipZone.Visible = false;

                m_curReportType = ReportType.Pareto_Sensor;

                lblTotalPage.Text = m_pageSensor.CurrentPage + " / " + m_pageSensor.TotalPage;

                lblGridViewTitle.Location = new Point(winChartViewerSensor.Location.X, winChartViewerSensor.Location.Y + winChartViewerSensor.Height + 20);
                dataGridViewSensor.Location = new Point(lblGridViewTitle.Location.X, lblGridViewTitle.Location.Y + lblGridViewTitle.Height);
            }
            else
            {
                dataGridViewSensor.Visible = false;
                dataGridViewEquipZone.Visible = true;

                winChartViewerSensor.Visible = false;
                winChartViewerEquipZone.Visible = true;

                m_curReportType = ReportType.Pareto_EquipZone;

                lblTotalPage.Text = m_pageEquipZone.CurrentPage + " / " + m_pageEquipZone.TotalPage;

                lblGridViewTitle.Location = new Point(winChartViewerSensor.Location.X, winChartViewerSensor.Location.Y + winChartViewerEquipZone.Height + 20);
                dataGridViewEquipZone.Location = new Point(lblGridViewTitle.Location.X, lblGridViewTitle.Location.Y + lblGridViewTitle.Height);
            }
        }

        private void LoadData()
        {
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

            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            m_detectMgr.ZoneSubmit(arrSelectZoneList, m_dtStartDate, m_dtEndDate, m_curFacilityType);
                        
            dataGridViewSensor.Rows.Clear();
            dataGridViewEquipZone.Rows.Clear();

            if (m_dicSensorHistories != null)
                m_dicSensorHistories.Clear();

            if (m_dicEquipZoneHistories != null)
                m_dicEquipZoneHistories.Clear();

            dataGridViewSensor.DataSource = null;
            dataGridViewEquipZone.DataSource = null;

            // SensorHistoryData List
            ArrayList arrSensorZoneHistory = null;//new ArrayList();
            arrSensorZoneHistory = m_detectMgr.DectectList;

            // 고속 처리를 위하여 Rows.Add() 대신 AddCopies()를 사용한다.
            int nRowCount = arrSensorZoneHistory.Count;

            // Key : SensorReactionHistory ID
            // Value : SensorZone ID
            Dictionary<int, int> dicSensorZoneIDs = GetSensorReactionLogs();

            foreach (Report.DetectLog historyData in from historyData in arrSensorZoneHistory.Cast<Report.DetectLog>()
                                                     orderby historyData.Time descending
                                                     select historyData
                                                     )
            {
                int nSensorZoneID;
                SensorTagInfo tag = null;

                if (dicSensorZoneIDs.TryGetValue(historyData.SensorReactionHistoryID, out nSensorZoneID))
                    tag = SensorTagHistoryManager.Instance.GetSensorTagFromSensorZone(nSensorZoneID);
                
                if (tag != null)
                {
                    int nCount = 0;

                    if (m_dicSensorHistories != null)
                    {
                        if (m_dicSensorHistories.TryGetValue(tag, out nCount) == false)
                            m_dicSensorHistories[tag] = 1;
                        else
                            m_dicSensorHistories[tag] = nCount + 1;
                    }

                    if (m_dicEquipZoneHistories != null)
                    {
                        if (tag.EquipmentZone != null)
                        {
                            if (m_dicEquipZoneHistories.TryGetValue(tag.EquipmentZone, out nCount) == false)
                                m_dicEquipZoneHistories[tag.EquipmentZone] = 1;
                            else
                                m_dicEquipZoneHistories[tag.EquipmentZone] = nCount + 1;
                        }
                    }
                }
            }

            SetData();

            m_strSearchDate = m_dtStartDate.ToString("yyyy-MM-dd") + " ~ " + m_dtEndDate.ToString("yyyy-MM-dd");
            if (building.BuildingName == "전체")
                m_strSearchZone = "모든 건물";
            else
                m_strSearchZone = building.BuildingName + " " + strFloor;
        }
        
        private void SetData()
        {
            double[] values = null;
            string[] labels = null;

            RefreshSensorGrid(out values, out labels);
            RefreshSensorChart(values, labels);

            RefreshEquipZoneGrid(out values, out labels);
            RefreshEquipZoneChart(values, labels);

            int chartHeight = winChartViewerSensor.Height;
            if (chartHeight < winChartViewerEquipZone.Height)
                chartHeight = winChartViewerEquipZone.Height;

            if (m_cbReportType.customComboBox.SelectedIndex == 0)
                lblGridViewTitle.Location = new Point(winChartViewerSensor.Location.X, winChartViewerSensor.Location.Y + winChartViewerSensor.Height + 20);
            else
                lblGridViewTitle.Location = new Point(winChartViewerSensor.Location.X, winChartViewerSensor.Location.Y + winChartViewerEquipZone.Height + 20);

            dataGridViewSensor.Location = new Point(lblGridViewTitle.Location.X, lblGridViewTitle.Location.Y + lblGridViewTitle.Height);
            dataGridViewEquipZone.Location = new Point(lblGridViewTitle.Location.X, lblGridViewTitle.Location.Y + lblGridViewTitle.Height);
        }

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
        
        private int GetTotalHistoryCount(List<SensorTagHistoryCount> histories)
        {
            int nCount = 0;

            foreach (SensorTagHistoryCount history in histories)
            {
                nCount += history.HistoryCount;
            }

            return nCount;
        }

        private int GetTotalHistoryCount(List<EquipZoneHistoryCount> histories)
        {
            int nCount = 0;

            foreach (EquipZoneHistoryCount history in histories)
            {
                nCount += history.HistoryCount;
            }

            return nCount;
        }

        private void AddRows(DataGridView grid, int nRowCount)
        {
            if (nRowCount > 0)
                grid.Rows.Add();

            if (nRowCount > 1)
                grid.Rows.AddCopies(0, nRowCount - 1);
        }

        private string GetBuildingName(SensorTagHistoryCount history)
        {
            if (history.Sensor.EquipmentZone == null)
                return "-";

            if (history.Sensor.EquipmentZone.Building == null)
                return "-";

            return history.Sensor.EquipmentZone.Building.BuildingName;
        }

        private string GetBuildingName(EquipZoneHistoryCount history)
        {
            if (history.EquipmentZone == null)
                return "-";

            if (history.EquipmentZone.Building == null)
                return "-";

            return history.EquipmentZone.Building.BuildingName;
        }

        private string GetFloorName(SensorTagHistoryCount history)
        {
            if (history.Sensor.EquipmentZone == null)
                return "-";

            if (history.Sensor.EquipmentZone.Building == null)
                return "-";

            return history.Sensor.EquipmentZone.Floor.ToString();
        }

        private string GetFloorName(EquipZoneHistoryCount history)
        {
            if (history.EquipmentZone == null)
                return "-";

            if (history.EquipmentZone.Building == null)
                return "-";

            return history.EquipmentZone.Floor.ToString();
        }

        private string GetLocationName(SensorTagHistoryCount history)
        {
            if (history.Sensor.EquipmentZone == null)
                return "-";

            return history.Sensor.EquipmentZone.ZoneName;
        }

        private string GetLocationName(EquipZoneHistoryCount history)
        {
            if (history.EquipmentZone == null)
                return "-";

            return history.EquipmentZone.ZoneName;
        }

        public static string GetHistoryPercent(int nTotalHistoryCount, int nHistoryCount)
        {
            if (nTotalHistoryCount <= 0)
                return "-";

            double rate = nHistoryCount * 100.0 / nTotalHistoryCount;
            string strRate = string.Format("{0:F2}", rate);

            if (strRate == "0.00")
                strRate = string.Format("{0:F3}", rate);

            return strRate;
        }

        private void RefreshSensorGrid(out double[] values, out string[] labels)
        {
            List<SensorTagHistoryCount> historyCounts = new List<SensorTagHistoryCount>();

            foreach (KeyValuePair<SensorTagInfo, int> pair in m_dicSensorHistories)
            {
                SensorTagHistoryCount history = new SensorTagHistoryCount();
                history.Sensor = pair.Key;
                history.HistoryCount = pair.Value;

                historyCounts.Add(history);
            }

            historyCounts.Sort();
            dataGridViewSensor.Rows.Clear();

            int nHistoryCount = historyCounts.Count;

            if (nHistoryCount == 0)
            {
                values = null;
                labels = null;
            }
            else
            {
                values = new double[nHistoryCount];
                labels = new string[nHistoryCount];
            }

            AddRows(dataGridViewSensor, nHistoryCount);

            int nTotalHistoryCount = GetTotalHistoryCount(historyCounts);

            for (int i = nHistoryCount - 1, j = 0; i >= 0; i--, j++)
            {
                SensorTagHistoryCount history = historyCounts[i];
                DataGridViewRow row = dataGridViewSensor.Rows[j];

                foreach (TypeColumns column in uFormReport.Instance.DicDefineColumns[m_curFacilityType][ReportType.Pareto_Sensor])
                {
                    switch (column.DefineColumn.ColumnName)
                    {
                        case "colNumber":
                            row.Cells[column.DefineColumn.ColumnName].Value = row.Index + 1;
                            break;
                        case "colSensorName":
                            row.Cells[column.DefineColumn.ColumnName].Value = history.Sensor.TagName;
                            break;
                        case "colBuildingName":
                            row.Cells[column.DefineColumn.ColumnName].Value = GetBuildingName(history);
                            break;
                        case "colFoor":
                            row.Cells[column.DefineColumn.ColumnName].Value = GetFloorName(history);
                            break;
                        case "colLocation":
                            row.Cells[column.DefineColumn.ColumnName].Value = GetLocationName(history);
                            break;
                        case "colHistoryCount":
                            row.Cells[column.DefineColumn.ColumnName].Value = history.HistoryCount;
                            break;
                        case "colPercent":
                            row.Cells[column.DefineColumn.ColumnName].Value = GetHistoryPercent(nTotalHistoryCount, history.HistoryCount);
                            break;
                    }
                }

                row.Tag = history;

                values[j] = history.HistoryCount;
                labels[j] = history.Sensor.TagName;
            }

            if (nHistoryCount > 0)
                dataGridViewSensor.Rows[0].Cells[0].Selected = true;
        }

        private void RefreshEquipZoneGrid(out double[] values, out string[] labels)
        {
            List<EquipZoneHistoryCount> historyCounts = new List<EquipZoneHistoryCount>();

            foreach (KeyValuePair<UnE.Spatial.EquipmentZone, int> pair in m_dicEquipZoneHistories)
            {
                EquipZoneHistoryCount history = new EquipZoneHistoryCount();
                history.EquipmentZone = pair.Key;
                history.HistoryCount = pair.Value;

                historyCounts.Add(history);
            }

            historyCounts.Sort();
            dataGridViewEquipZone.Rows.Clear();

            int nHistoryCount = historyCounts.Count;

            if (nHistoryCount == 0)
            {
                values = null;
                labels = null;
            }
            else
            {
                values = new double[nHistoryCount];
                labels = new string[nHistoryCount];
            }

            AddRows(dataGridViewEquipZone, nHistoryCount);

            int nTotalHistoryCount = GetTotalHistoryCount(historyCounts);

            for (int i = nHistoryCount - 1, j = 0; i >= 0; i--, j++)
            {
                EquipZoneHistoryCount history = historyCounts[i];
                DataGridViewRow row = dataGridViewEquipZone.Rows[j];

                foreach (TypeColumns column in uFormReport.Instance.DicDefineColumns[m_curFacilityType][ReportType.Pareto_EquipZone])
                {
                    switch (column.DefineColumn.ColumnName)
                    {
                        case "colNumber":
                            row.Cells[column.DefineColumn.ColumnName].Value = row.Index + 1;
                            break;
                        case "colLocation":
                            row.Cells[column.DefineColumn.ColumnName].Value = GetLocationName(history);
                            break;
                        case "colBuildingName":
                            row.Cells[column.DefineColumn.ColumnName].Value = GetBuildingName(history);
                            break;
                        case "colFoor":
                            row.Cells[column.DefineColumn.ColumnName].Value = GetFloorName(history);
                            break;                        
                        case "colHistoryCount":
                            row.Cells[column.DefineColumn.ColumnName].Value = history.HistoryCount;
                            break;
                        case "colPercent":
                            row.Cells[column.DefineColumn.ColumnName].Value = GetHistoryPercent(nTotalHistoryCount, history.HistoryCount);
                            break;
                    }
                }

                row.Tag = history;

                values[j] = history.HistoryCount;
                labels[j] = row.Cells["colLocation"].Value.ToString();
            }

            if (nHistoryCount > 0)
                dataGridViewEquipZone.Rows[0].Cells[0].Selected = true;
        }

        private void RefreshSensorChart(double[] values, string[] labels)
        {
            RefreshChart(values, labels, winChartViewerSensor, dataGridViewSensor, m_pageSensor);
        }

        private void RefreshEquipZoneChart(double[] values, string[] labels)
        {
            RefreshChart(values, labels, winChartViewerEquipZone, dataGridViewEquipZone, m_pageEquipZone);
        }

        private double m_nChartFontSize = 11.25;
        private string m_strFontName = "나눔바른고딕";

        private void RefreshChart(double[] values, string[] labels, WinChartViewer chartViewer, DataGridView grid, PageData pageData)
        {
            if (values == null || labels == null)
            {
                EmptyChart(chartViewer, grid);
                return;
            }
            
            // In the pareto chart, the line data are just the accumulation of the
            // raw data, scaled to a range of 0 - 100%
            ArrayMath lineData = new ArrayMath(values);
            lineData.acc();
            double scaleFactor = lineData.max() / 100;
            if (scaleFactor == 0)
            {
                // Avoid division by zero error for zero data
                scaleFactor = 1;
            }
            lineData.div2(scaleFactor);

            double[] resultDatas = lineData.result();
            int nCurrentPageIndex = GetCurrentPageIndex(values.Count(), pageData);
            SetPageValue(ref values, ref labels, ref resultDatas, nCurrentPageIndex);
            
            double maxValue = values.Max();

            int nLineWidth = 2;

            Size sizeGrid = grid.Size;
            Point ptGrid = grid.Location;
            int nSpace = 30, nLeftSpace2 = 80;

            int nChartHeight = 310;
            int nPlotX = ptGrid.X + nSpace;
            int nPlotY = 15;
            int nPlotWidth = sizeGrid.Width - nSpace * 2 - nLeftSpace2;
            int nPlotHeight = nChartHeight - nSpace * 2;

            Font fontXAxis = new System.Drawing.Font(m_strFontName, (float)m_nChartFontSize, FontStyle.Regular);

            double dLabelAngle =  GetLabelAngle(chartViewer, grid, fontXAxis, labels, sizeGrid.Width, ref nChartHeight, ref nPlotX, ref nPlotY, ref nPlotWidth, ref nPlotHeight);
            
            XYChart c = new XYChart(sizeGrid.Width, nChartHeight);
            
            c.setPlotArea(nPlotX, nPlotY, nPlotWidth, nPlotHeight);

            // Add a line layer for the pareto line
            LineLayer lineLayer = c.addLineLayer2();

            // Add the pareto line using deep blue (0000ff) as the color, with circle
            // symbols
            lineLayer.addDataSet(resultDatas, 0xf7a92b).setDataSymbol(Chart.CircleShape, 10, 0xf7a92b, 0xf7a92b);

            // Set the line width to 2 pixel
            lineLayer.setLineWidth(nLineWidth);

            // Bind the line layer to the secondary (right) y-axis.
            lineLayer.setUseYAxis2();

            // Tool tip for the line layer
            /*lineLayer.setHTMLImageMap("", "",
                "title='Top {={x}+1} items: {value|2}%'");*/

            // Add a multi-color bar layer using the given data.
            BarLayer barLayer = c.addBarLayer3(values);

            // Set soft lighting for the bars with light direction from the right
            barLayer.setBorderColor(Chart.Transparent, Chart.softLighting(Chart.Right));

            barLayer.setBarWidth(40); // Bar의 두께 설정
            barLayer.setAggregateLabelFormat("{value}");
            barLayer.setAggregateLabelStyle(m_strFontName, m_nChartFontSize, 0x253150, 0).setAlignment(Chart.Center);

            // Tool tip for the bar layer
            //barLayer.setHTMLImageMap("", "", "title='{xLabel}: {value} pieces'");

            // Set the secondary (right) y-axis scale as 0 - 100 with a tick every 20
            // units
            c.yAxis2().setLinearScale(0, 100, 20);

            // Set the format of the secondary (right) y-axis label to include a
            // percentage sign
            c.yAxis2().setLabelFormat("{value}%");

            // Set the relationship between the two y-axes, which only differ by a
            // scaling factor
            c.yAxis().syncAxis(c.yAxis2(), scaleFactor);

            // Set the format of the primary y-axis label foramt to show no decimal
            // point
            c.yAxis().setLabelFormat("{value|0,}");

            // Add a title to the primary y-axis  
            PlotArea pa = c.getPlotArea();
            c.addText(pa.getLeftX() - 50, pa.getBottomY() - 210, m_strChartTitle, m_strFontName, m_nChartFontSize, 0x253150);

            // Set all axes to transparent
            c.xAxis().setColors(0x253150);
            c.xAxis2().setColors(0x253150);
            c.yAxis().setColors(0x253150);
            c.yAxis2().setColors(0x253150);

            c.xAxis().setWidth(nLineWidth);
            c.xAxis2().setWidth(nLineWidth);
            c.yAxis().setWidth(nLineWidth);
            c.yAxis2().setWidth(nLineWidth);

            // Set the labels on the x axis.
            c.xAxis().setLabels(labels);
            c.xAxis().setLabelStyle(m_strFontName, 10, 0x253150, dLabelAngle);
            c.xAxis2().setLabelStyle(m_strFontName, 10, 0x253150);
            c.yAxis().setLabelStyle(m_strFontName, 10, 0x253150);
            c.yAxis2().setLabelStyle(m_strFontName, 10, 0x253150);
            
            // Output the chart
            chartViewer.Chart = c;
            //SetNavigatorEnable(pageData);
        }

        private void EmptyChart(WinChartViewer chartViewer, DataGridView grid)
        {
            double[] values = new double[1] { Chart.NoValue };

            // In the pareto chart, the line data are just the accumulation of the
            // raw data, scaled to a range of 0 - 100%
            ArrayMath lineData = new ArrayMath(values);
            lineData.acc();
            double scaleFactor = lineData.max() / 100;
            if (scaleFactor == 0)
            {
                // Avoid division by zero error for zero data
                scaleFactor = 1;
            }
            lineData.div2(scaleFactor);

            //if (grid.Location.Y != m_nGridInitY)
            //    MoveGrid(grid, m_nGridInitY);
            
            Size sizeGrid = grid.Size;
            Point ptGrid = grid.Location;
            int nSpace = 30, nLeftSpace2 = 80;

            XYChart c = new XYChart(sizeGrid.Width, chartViewer.Height);
            
            // Tentatively set the plotarea at (50, 40). Set the width to 100 pixels
            // less than the chart width, and the height to 80 pixels less than the
            // chart height. Use pale grey (f4f4f4) background, transparent border,
            // and dark grey (444444) dotted grid lines.
            c.setPlotArea(ptGrid.X + nSpace, 15, sizeGrid.Width - nSpace * 2 - nLeftSpace2, chartViewer.Height - nSpace * 2);

            // Add a line layer for the pareto line
            LineLayer lineLayer = c.addLineLayer2();

            // Add the pareto line using deep blue (0000ff) as the color, with circle
            // symbols
            lineLayer.addDataSet(lineData.result(), 0x0000ff).setDataSymbol(Chart.CircleShape, 9, 0x0000ff, 0x0000ff);

            // Set the line width to 2 pixel
            lineLayer.setLineWidth(2);

            // Bind the line layer to the secondary (right) y-axis.
            lineLayer.setUseYAxis2();

            // Add a multi-color bar layer using the given data.
            BarLayer barLayer = c.addBarLayer3(values);

            // Set soft lighting for the bars with light direction from the right
            barLayer.setBorderColor(Chart.Transparent, Chart.softLighting(Chart.Right));

            barLayer.setBarWidth(40); // Bar의 두께 설정

            // Set the secondary (right) y-axis scale as 0 - 100 with a tick every 20
            // units
            c.yAxis2().setLinearScale(0, 100, 20);

            // Set the format of the secondary (right) y-axis label to include a
            // percentage sign
            c.yAxis2().setLabelFormat("{value}%");

            // Set the relationship between the two y-axes, which only differ by a
            // scaling factor
            c.yAxis().syncAxis(c.yAxis2(), scaleFactor);

            // Set the format of the primary y-axis label foramt to show no decimal
            // point
            c.yAxis().setLabelFormat("0");

            // Set all axes to transparent
            c.xAxis().setColors(Chart.Transparent);
            c.yAxis().setColors(Chart.Transparent);
            c.yAxis2().setColors(Chart.Transparent);

            //c.yAxis().setTitle(m_strChartTitle, m_strFontName, m_nChartFontSize);
            PlotArea pa = c.getPlotArea();
            c.addText(pa.getLeftX() - 50, pa.getBottomY() - 210, m_strChartTitle, m_strFontName, m_nChartFontSize, 0x253150);

            c.yAxis().setLabelStyle(m_strFontName, m_nChartFontSize, 0x000000);

            // Output the chart
            chartViewer.Chart = c;
        }

        // 글자가 겹치는 부분이 있는지 검사하여, 겹치는 구간이 있으면 무조건 Text Angle을 20도로 준다.
        private double GetLabelAngle(WinChartViewer chartViewer, DataGridView grid, Font font, string[] labels, int nChartWidth, ref int nChartHeight, ref int nPlotX, ref int nPlotY, ref int nPlotWidth, ref int nPlotHeight)
        {
            int nLabelCount = labels.Count();

            if (nLabelCount <= 1)
            {
                //if (grid.Location.Y != m_nGridInitY)
                //    MoveGrid(grid, m_nGridInitY);

                return 0.0;
            }

            Graphics g = chartViewer.CreateGraphics();
            int nSpace = nPlotWidth / (nLabelCount + 1);
            int nBeginPosition = nSpace / 2;

            SizeF size = g.MeasureString(labels[0], font);
            int nPrevLeft = nBeginPosition - (int)(size.Width / 2);
            int nPrevRight = nPrevLeft + (int)size.Width;

            // Degree
            double dAngle = 0.0;
            int nChangePlotHeight = 0;

            SizeF[] arrSize = new SizeF[nLabelCount];
            arrSize[0] = size;

            for (int i = 1; i < nLabelCount; i++)
            {
                size = g.MeasureString(labels[i], font);
                int nLeft = nBeginPosition + nSpace * i - (int)(size.Width / 2);
                int nRight = nPrevLeft + (int)size.Width;

                if (nLeft <= nPrevRight)
                {
                    dAngle = 20.0;
                    nChangePlotHeight = -30;
                    //break;
                }

                arrSize[i] = size;
                nPrevLeft = nLeft;
                nPrevRight = nRight;
            }

            // 글자 기울임이 생기면 Graph 영역을 축소시키지 말고, Chart Control 크기를 키운다.
            nChartHeight -= nChangePlotHeight;
            //nPlotHeight += nChangePlotHeight;

            if (dAngle > 0.0)
            {
                double dRadian = UnE.Geometry.Math.DegToRad(dAngle);
                double dCos = System.Math.Cos(dRadian);
                double dSin = System.Math.Sin(dRadian);

                double dHeight = font.Height * dCos;
                double dMinLeft = nPlotWidth;
                double dMaxBottom = 0;

                for (int i = 0; i < nLabelCount; i++)
                {
                    int nPosition = nBeginPosition + nSpace * i;
                    SizeF sizeText = arrSize[i];

                    double dLeft = nPosition - sizeText.Width * dCos;
                    // 10은 막대그래프로부터 아래쪽으로 띄운 Padding값
                    double dBottom = 10 + sizeText.Width * dSin + dHeight;

                    if (dLeft < dMinLeft)
                        dMinLeft = dLeft;

                    if (dBottom > dMaxBottom)
                        dMaxBottom = dBottom;
                }

                if (dMinLeft < 0)
                {
                    dMinLeft = -dMinLeft;
                    int nLeft = (int)(dMinLeft + 1);

                    if (nLeft > nPlotX)
                    {
                        nPlotWidth -= (nLeft - nPlotX);
                        nPlotX = nLeft;
                    }
                }

                int nSpaceY = nChartHeight - nPlotY - nPlotHeight;

                if (nSpaceY < (int)(dMaxBottom + 1))
                {
                    int newSpaceY = (int)(dMaxBottom + 1);
                    nChartHeight += newSpaceY - nSpaceY;
                }
            }

            //if (chartViewer.Location.Y + nChartHeight >= m_nGridInitY)
            //    MoveGrid(grid, chartViewer.Location.Y + nChartHeight);
            //else if (grid.Location.Y != m_nGridInitY)
            //    MoveGrid(grid, m_nGridInitY);

            return dAngle;
        }

        private int GetCurrentPageIndex(int nValueCount, PageData pageData)
        {
            //pageData.CurrentPage = (int)pageData.PageIndexComboBox.Items[pageData.PageIndexComboBox.SelectedIndex];

            if (nValueCount == 0)
                pageData.TotalPage = 1;
            else
                pageData.TotalPage = (nValueCount - 1) / m_nViewCount + 1;

            //if (pageData.TotalPage != pageData.PageIndexComboBox.Items.Count)
            //{
            //    pageData.PageIndexComboBox.Items.Clear();

            //    for (int i = 1; i <= pageData.TotalPage; i++)
            //    {
            //        pageData.PageIndexComboBox.Items.Add(i);
            //    }

            //    if (pageData.PageIndexComboBox.Items.Count > 0)
            //    {
            //        m_systemCall = true;
            //        pageData.PageIndexComboBox.SelectedIndex = 0;
            //        m_systemCall = false;
            //    }
            //}

            pageData.TotalPageCountLabel.Text = pageData.CurrentPage + " / " + pageData.TotalPage.ToString();
            //pageData.TotalPageCountLabel.Location = new Point(panel1.Location.X + panel1.Width - pageData.TotalPageCountLabel.Width, panel1.Location.Y + panel1.Height + 5);
            return pageData.CurrentPage;
        }

        private void SetPageValue(ref double[] values, ref string[] labels, ref double[] resultDatas, int nCurrentPageIndex)
        {
            int nValueCount = values.Count();

            if (nValueCount <= m_nViewCount)
                return;

            int nBeginIndex = (nCurrentPageIndex - 1) * m_nViewCount;
            int nNewValueCount = nBeginIndex + m_nViewCount <= nValueCount ? m_nViewCount : nValueCount - nBeginIndex;
            if (nNewValueCount < 0)
            {
                nBeginIndex = 0;
                nNewValueCount = m_nViewCount;

                m_pageSensor.CurrentPage = 1;
                m_pageEquipZone.CurrentPage = 1;
                //SetNavigatorEnable(m_pageSensor);

                //m_pageSensor.TotalPageCountLabel.Text = m_pageSensor.CurrentPage + " / " + m_pageSensor.TotalPage.ToString();
                //m_pageSensor.TotalPageCountLabel.Location = new Point(panel1.Location.X + panel1.Width - m_pageSensor.TotalPageCountLabel.Width, panel1.Location.Y + panel1.Height + 5);
                //m_pageEquipZone.TotalPageCountLabel.Text = m_pageEquipZone.CurrentPage + " / " + m_pageEquipZone.TotalPage.ToString();
                //m_pageEquipZone.TotalPageCountLabel.Location = new Point(panel1.Location.X + panel1.Width - m_pageEquipZone.TotalPageCountLabel.Width, panel1.Location.Y + panel1.Height + 5);
            }
            int nEndIndex = nBeginIndex + nNewValueCount;

            double[] newValues = new double[nNewValueCount];
            string[] newLabels = new string[nNewValueCount];
            double[] newResultDatas = new double[nNewValueCount];

            for (int i = nBeginIndex, j = 0; i < nEndIndex; i++, j++)
            {
                newValues[j] = values[i];
                newLabels[j] = labels[i];
                newResultDatas[j] = resultDatas[i];
            }

            values = newValues;
            labels = newLabels;
            resultDatas = newResultDatas;
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

        private class SensorTagHistoryCount : IComparable
        {
            private SensorTagInfo m_sensor = null;
            private int m_nHistoryCount = 0;

            public SensorTagInfo Sensor
            {
                get { return m_sensor; }
                set { m_sensor = value; }
            }

            public int HistoryCount
            {
                get { return m_nHistoryCount; }
                set { m_nHistoryCount = value; }
            }

            public int CompareTo(object obj)
            {
                SensorTagHistoryCount history = (SensorTagHistoryCount)obj;

                if (this.HistoryCount > history.HistoryCount)
                    return 1;
                else if (this.HistoryCount < history.HistoryCount)
                    return -1;
                //else
                return 0;
            }
        }

        private class EquipZoneHistoryCount : IComparable
        {
            private UnE.Spatial.EquipmentZone m_equipZone = null;
            private int m_nHistoryCount = 0;

            public UnE.Spatial.EquipmentZone EquipmentZone
            {
                get { return m_equipZone; }
                set { m_equipZone = value; }
            }

            public int HistoryCount
            {
                get { return m_nHistoryCount; }
                set { m_nHistoryCount = value; }
            }

            public int CompareTo(object obj)
            {
                EquipZoneHistoryCount history = (EquipZoneHistoryCount)obj;

                if (this.HistoryCount > history.HistoryCount)
                    return 1;
                else if (this.HistoryCount < history.HistoryCount)
                    return -1;
                //else
                return 0;
            }
        }
        
        public void btnSearch_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        
        private void btnPageBefore_Click(object sender, EventArgs e)
        {
            PageData pageData = (m_cbReportType.customComboBox.SelectedIndex == 0) ? m_pageSensor : m_pageEquipZone;

            if (pageData.CurrentPage == 1)
                return;

            pageData.CurrentPage--;

            double[] values;
            string[] labels;

            GetSensorGridValues(out values, out labels);
            RefreshSensorChart(values, labels);

            GetEquipZoneGridValues(out values, out labels);
            RefreshEquipZoneChart(values, labels);

            lblTotalPage.Text = pageData.CurrentPage + " / " + pageData.TotalPage;
        }

        private void btnPageNext_Click(object sender, EventArgs e)
        {
            PageData pageData = (m_cbReportType.customComboBox.SelectedIndex == 0) ? m_pageSensor : m_pageEquipZone;

            if (pageData.CurrentPage == pageData.TotalPage)
                return;

            pageData.CurrentPage++;

            double[] values;
            string[] labels;

            GetSensorGridValues(out values, out labels);
            RefreshSensorChart(values, labels);

            GetEquipZoneGridValues(out values, out labels);
            RefreshEquipZoneChart(values, labels);

            lblTotalPage.Text = pageData.CurrentPage + " / " + pageData.TotalPage;
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

            string strSavePath = uFormReport.Instance.GetHWPFilePath(curType + "_탐지분석_보고서", isHwpSetup);
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

        private const string HWP_SENSOR_TAG = "ParetoSensor";
        private const string HWP_EQUIPZONE_TAG = "ParetoEquipZone";
        public void ControllCapture()
        {
            CaptureImage(winChartViewerSensor, HWP_SENSOR_TAG);
            CaptureImage(winChartViewerEquipZone, HWP_EQUIPZONE_TAG);
        }

        private void CaptureImage(WinChartViewer chartViewer, string strImageName)
        {
            Bitmap bmp = new Bitmap(chartViewer.Width, chartViewer.Height);
            chartViewer.DrawToBitmap(bmp, new Rectangle(0, 0, chartViewer.Width, chartViewer.Height));
            bmp.Save(Application.StartupPath + "\\report\\" + strImageName + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
        }

        public void FileWriter()
        {
            List<string> lines = new List<string>();
            SaveHwpCrtl(dataGridViewSensor, lines, HWP_SENSOR_TAG);
            SaveHwpCrtl(dataGridViewEquipZone, lines, HWP_EQUIPZONE_TAG);

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\report\\SaveData.txt"))
            {
                foreach (string line in lines)
                {
                    file.WriteLine(line);
                }
                file.Close();
            }
        }

        private void SaveHwpCrtl(DataGridView grid, List<string> lines, string strTag)
        {
            // 데이터 구분을 위한 표식(Sensor / EquipZone)
            lines.Add("[" + strTag + "]");

            int nRowCount = grid.RowCount;
            int nColumnCount = grid.ColumnCount;

            int nExceptColumnIndex = -1;

            //if (grid == dataGridViewSensor)
            //    nExceptColumnIndex = (int)FireSensorGrid.BUILDING_GROUP_INDEX;
            //else if (grid == dataGridViewEquipZone)
            //    nExceptColumnIndex = (int)FireEquipZoneGrid.BUILDING_GROUP_INDEX;

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = grid.Rows[i];

                for (int j = 0; j < nColumnCount; j++)
                {
                    if (j == nExceptColumnIndex)
                        continue;

                    lines.Add(row.Cells[j].Value.ToString());
                }
            }
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
    }
}
