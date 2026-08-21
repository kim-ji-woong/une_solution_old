using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using ChartDirector;
using UnE.Spatial;

namespace SDMS
{
    public partial class ParetoPage : Form, SDMS.Data.IParetoPage
    {
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

        private class PageData
        {
            private Button btnPrevious = null;
            private Button btnNext = null;
            private ComboBox cboPageIndex = null;
            private Label lblTotalPage = null;
            private int m_nCurrentPage = 1;
            private int m_nTotalPage = 1;

            public Button PreviousButton
            {
                get { return btnPrevious; }
                set { btnPrevious = value; }
            }

            public Button NextButton
            {
                get { return btnNext; }
                set { btnNext = value; }
            }

            public ComboBox PageIndexComboBox
            {
                get { return cboPageIndex; }
                set { cboPageIndex = value; }
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

            public PageData()
            {
            }

            public PageData(Button btnPrev, Button btnNext, ComboBox cboPageIndex, Label lblTotalPage)
            {
                this.btnPrevious = btnPrev;
                this.btnNext = btnNext;
                this.cboPageIndex = cboPageIndex;
                this.lblTotalPage = lblTotalPage;
            }
        }

        public enum ChartType { SENSOR, EQUIPZONE };

        public enum FireSensorGrid
        {
            NO_INDEX = 0,
            SENSOR_NAME_INDEX,
            BUILDING_GROUP_INDEX,
            BUILDING_INDEX,
            FLOOR_INDEX,
            LOCATION_INDEX,
            HISTORY_COUNT_INDEX,
            PERCENT_INDEX
        }

        public enum FireEquipZoneGrid
        {
            NO_INDEX = 0,
            LOCATION_INDEX,
            BUILDING_GROUP_INDEX,
            BUILDING_INDEX,
            FLOOR_INDEX,
            HISTORY_COUNT_INDEX,
            PERCENT_INDEX
        }

        // 센서별 알람 History Count
        private Dictionary<SensorTagInfo, int> m_dicSensorHistories = new Dictionary<SensorTagInfo, int>();
        // 위치별 알람 History Count
        private Dictionary<UnE.Spatial.EquipmentZone, int> m_dicEquipZoneHistories = new Dictionary<UnE.Spatial.EquipmentZone, int>();

        private const string SensorChartTag = "탐지 횟수";
        private const string EquipZoneChartTag = "탐지 횟수";

        // 현재 Grid에 나타나있는 데이터들에 대한 SensorTagHistory의 시작과 끝 Index
        private int m_nBeginIndex = -1, m_nEndIndex = -1;
        private Dictionary<int, UnE.Spatial.Zone> m_dicCurrentZones = new Dictionary<int, UnE.Spatial.Zone>();
        // 현재 Chart에 적용된 최대 표시 개수
        private int m_nViewCount = 20;

        //private int m_nCurrentPage = 1, m_nTotalPage = 1;

        private string m_strBuildingGroup = "모든 건물 그룹";
        private string m_strBuilding = "모든 건물";
        private string m_strFloor = "모든 층";

        private Report.ReactionManager m_detectMgr = null;
        private bool m_systemCall = false;
        private int m_nGridInitY = 0, m_nGridInitSpaceY = 0;

        private const string HWP_SENSOR_TAG = "ParetoSensor";
        private const string HWP_EQUIPZONE_TAG = "ParetoEquipZone";

        private PageData m_pageSensor = null;
        private PageData m_pageEquipZone = null;

        private DetectPage m_detectPage = null;

        // 센서별 알람 History Count
        public Dictionary<SensorTagInfo, int> SensorHistories
        {
            get { return m_dicSensorHistories; }
        }

        // 위치별 알람 History Count
        public Dictionary<UnE.Spatial.EquipmentZone, int> EquipZoneHistories
        {
            get { return m_dicEquipZoneHistories; }
        }

        public ParetoPage(UnE.Sensor.IFacility.FacilityType type, Report.ReactionManager detectMgr, DetectPage detectPage)
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            FormMain.SetDoubleBuffer(dataGridViewEquipZone, true);
            FormMain.SetDoubleBuffer(dataGridViewSensor, true);

            m_detectMgr = detectMgr;
            m_detectPage = detectPage;

            dataGridViewSensor.Font = new Font("맑은 고딕", 12.0f);
            dataGridViewEquipZone.Font = new Font("맑은 고딕", 12.0f);
            m_nGridInitY = dataGridViewSensor.Location.Y;
            m_nGridInitSpaceY = this.Size.Height - (dataGridViewSensor.Location.Y + dataGridViewSensor.Size.Height);

            SetFireSensorColumns();
            SetFireEquipZoneColumns();

            SetPageData();
            SetPageIndices(m_pageSensor);
            SetPageIndices(m_pageEquipZone);

            cboChart_SelectedIndexChanged(null, null);
        }

        private void SetPageData()
        {
            btnPreviousIndexEquipZone.Location = btnPreviousIndexSensor.Location;
            cboPageIndexEquipZone.Location = cboPageIndexSensor.Location;
            lblTotalPageEquipZone.Location = lblTotalPageSensor.Location;
            btnNextIndexEquipZone.Location = btnNextIndexSensor.Location;

            m_pageSensor = new PageData(btnPreviousIndexSensor, btnNextIndexSensor, cboPageIndexSensor, lblTotalPageSensor);
            m_pageEquipZone = new PageData(btnPreviousIndexEquipZone, btnNextIndexEquipZone, cboPageIndexEquipZone, lblTotalPageEquipZone);
        }

        private void SetFireSensorColumns()
        {
            DataGridViewTextBoxColumn colNo = new DataGridViewTextBoxColumn();
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNo.DefaultCellStyle = cellStyle;
            colNo.HeaderText = "No";
            colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colNo.Width = 90;
            dataGridViewSensor.Columns.Add(colNo);

            DataGridViewTextBoxColumn colSensorName = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new Padding(5, 0, 0, 0);
            colSensorName.DefaultCellStyle = cellStyle;
            colSensorName.HeaderText = "센서 이름";
            colSensorName.SortMode = DataGridViewColumnSortMode.NotSortable;
            colSensorName.Width = 240;
            dataGridViewSensor.Columns.Add(colSensorName);

            DataGridViewTextBoxColumn colBuildingGroup = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colBuildingGroup.DefaultCellStyle = cellStyle;
            colBuildingGroup.HeaderText = "건물 그룹";
            colBuildingGroup.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colBuildingGroup.Width = 334;
            dataGridViewSensor.Columns.Add(colBuildingGroup);

            DataGridViewTextBoxColumn colBuilding = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colBuilding.DefaultCellStyle = cellStyle;
            colBuilding.HeaderText = "건물";
            colBuilding.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colBuilding.Width = 334;
            dataGridViewSensor.Columns.Add(colBuilding);

            DataGridViewTextBoxColumn colFloor = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colFloor.DefaultCellStyle = cellStyle;
            colFloor.HeaderText = "층";
            colFloor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colFloor.Width = 50;
            dataGridViewSensor.Columns.Add(colFloor);

            DataGridViewTextBoxColumn colLocation = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            colLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colLocation.DefaultCellStyle = cellStyle;
            colLocation.HeaderText = "위치";
            colLocation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dataGridViewSensor.Columns.Add(colLocation);

            DataGridViewTextBoxColumn colHistoryCount = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colHistoryCount.DefaultCellStyle = cellStyle;
            colHistoryCount.HeaderText = "탐지횟수";
            colHistoryCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dataGridViewSensor.Columns.Add(colHistoryCount);

            DataGridViewTextBoxColumn colPercent = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colPercent.DefaultCellStyle = cellStyle;
            colPercent.HeaderText = "백분율(%)";
            colPercent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colPercent.Width = 90;
            dataGridViewSensor.Columns.Add(colPercent);

            foreach (DataGridViewColumn column in dataGridViewSensor.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void SetFireEquipZoneColumns()
        {
            DataGridViewTextBoxColumn colNo = new DataGridViewTextBoxColumn();
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colNo.DefaultCellStyle = cellStyle;
            colNo.HeaderText = "No";
            colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colNo.Width = 90;
            dataGridViewEquipZone.Columns.Add(colNo);

            DataGridViewTextBoxColumn colLocation = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            colLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colLocation.DefaultCellStyle = cellStyle;
            colLocation.HeaderText = "위치";
            colLocation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dataGridViewEquipZone.Columns.Add(colLocation);

            DataGridViewTextBoxColumn colBuildingGroup = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colBuildingGroup.DefaultCellStyle = cellStyle;
            colBuildingGroup.HeaderText = "건물 그룹";
            colBuildingGroup.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colBuildingGroup.Width = 334;
            dataGridViewEquipZone.Columns.Add(colBuildingGroup);

            DataGridViewTextBoxColumn colBuilding = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colBuilding.DefaultCellStyle = cellStyle;
            colBuilding.HeaderText = "건물";
            colBuilding.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colBuilding.Width = 334;
            dataGridViewEquipZone.Columns.Add(colBuilding);

            DataGridViewTextBoxColumn colFloor = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colFloor.DefaultCellStyle = cellStyle;
            colFloor.HeaderText = "층";
            colFloor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colFloor.Width = 50;
            dataGridViewEquipZone.Columns.Add(colFloor);

            DataGridViewTextBoxColumn colHistoryCount = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colHistoryCount.DefaultCellStyle = cellStyle;
            colHistoryCount.HeaderText = "탐지횟수";
            colHistoryCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dataGridViewEquipZone.Columns.Add(colHistoryCount);

            DataGridViewTextBoxColumn colPercent = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colPercent.DefaultCellStyle = cellStyle;
            colPercent.HeaderText = "백분율(%)";
            colPercent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colPercent.Width = 90;
            dataGridViewEquipZone.Columns.Add(colPercent);

            foreach (DataGridViewColumn column in dataGridViewEquipZone.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void SetPageIndices(PageData pageData)
        {
            pageData.PageIndexComboBox.Items.Clear();

            for (int i = 1; i <= pageData.CurrentPage; i++)
            {
                pageData.PageIndexComboBox.Items.Add(i);
            }

            if (pageData.PageIndexComboBox.Items.Count > 0)
                pageData.PageIndexComboBox.SelectedIndex = 0;
        }

        // arrSelectZoneList : 여기에 포함된 Zone에 대해서만 검색
        // nViewCount : Graph에 최대 몇 개까지의 데이터를 표시할 것인가?
        public void Load_DataGrid(ArrayList arrSelectZoneList, string strGroupName, string strBuildingName, string strFloorName, int nSplitUnitOfMeasure, int nSplitUnitOfMeasureDetail, int nViewCount)
        //public void Load_DataGrid(ArrayList arrSelectZoneList, int nViewCount)
        {
            DateTime startDate, endDate;
            
            if (!FormMain.Instance.GetCurrentReportDate(out startDate, out endDate))
                return;

            DateTime dtNow = DateTime.Now;

            if (endDate.Year == dtNow.Year && endDate.Month == dtNow.Month && endDate.Day == dtNow.Day)
            {
                // 서버와 시간차이가 날수도 있으니 Client의 현재 시간보다 1시간 뒤로 설정한다.
                endDate = dtNow.AddHours(1.0);
            }
            else
                endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 23, 59);

            // SensorTagHistory는 SensorZoneHistory와 개수가 다를수 있기 때문에, SensorTagHistory를 읽지 않고
            // DetectPage로부터 데이터를 받아서 쓰도록 한다.
            // [2017/04/18] 김지웅
            bool needRefresh = GetDataFromDetectPage(arrSelectZoneList, strGroupName, strBuildingName, strFloorName, nSplitUnitOfMeasure, nSplitUnitOfMeasureDetail, startDate, endDate, nViewCount);
            //bool needRefresh = SensorTagHistoryManager.Instance.LoadFireSensorTagHistories(arrSelectZoneList, m_dicCurrentZones, m_dicSensorHistories, m_dicEquipZoneHistories, ref m_nBeginIndex, ref m_nEndIndex, FormMain.Instance.DBManager, startDate, endDate, UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR);

            /*Dictionary<SensorTagInfo, int> dicTagHistories = SensorTagHistoryManager.Instance.LoadSensorTagHistories(FormMain.Instance.DBManager, startDate, endDate, m_type);

            if (m_dicTagHistories.Count != dicTagHistories.Count)
                needRefresh = true;
            else
            {
                int nHistoryCount;

                foreach (KeyValuePair<SensorTagInfo, int> pair in dicTagHistories)
                {
                    if (!m_dicTagHistories.TryGetValue(pair.Key, out nHistoryCount))
                    {
                        needRefresh = true;
                        break;
                    }
                    else
                    {
                        if (nHistoryCount != pair.Value)
                        {
                            needRefresh = true;
                            break;
                        }
                    }
                }
            }*/

            RefreshLabels(startDate, endDate);
            if (needRefresh)
            {
                /*m_dicTagHistories.Clear();

                foreach (KeyValuePair<SensorTagInfo, int> pair in dicTagHistories)
                {
                    m_dicTagHistories[pair.Key] = pair.Value;
                }*/
                m_nViewCount = nViewCount;

                double[] values = null;
                string[] labels = null;

                RefreshSensorGrid(out values, out labels);
                RefreshSensorChart(values, labels, nViewCount);

                RefreshEquipZoneGrid(out values, out labels);
                RefreshEquipZoneChart(values, labels, nViewCount);
            }
            else if (m_nViewCount != nViewCount)
            {
                m_nViewCount = nViewCount;

                double[] values = null;
                string[] labels = null;

                GetSensorGridValues(out values, out labels);
                RefreshSensorChart(values, labels, nViewCount);

                GetEquipZoneGridValues(out values, out labels);
                RefreshEquipZoneChart(values, labels, nViewCount);
            }
        }

        // Return 값 : Refresh가 필요한가?
        private bool GetDataFromDetectPage(ArrayList arrSelectZoneList, string strGroupName, string strBuildingName, string strFloorName, int nSplitUnitOfMeasure, int nSplitUnitOfMeasureDetail, DateTime startDate, DateTime endDate, int nViewCount)
        {
            bool needRefresh = false;

            if (m_detectMgr.NeedRefresh(arrSelectZoneList, startDate, endDate, m_detectPage.RefreshCheckData))
            {
                needRefresh = true;
                m_detectPage.RefreshCheckData.ViewCount = nViewCount;

                m_detectMgr.DataClear();
                m_detectMgr.ZoneSubmit(arrSelectZoneList, startDate, endDate);

                m_detectPage.ComboSubmit(strGroupName, strBuildingName, strFloorName);
                m_detectPage.Load_DataGrid(m_dicSensorHistories, m_dicEquipZoneHistories);
                m_detectPage.CreateBarChart(startDate, endDate, nSplitUnitOfMeasure, nSplitUnitOfMeasureDetail, nViewCount);
            }

            if (needRefresh == false)
                needRefresh = CheckRefresh();

            return needRefresh;
        }

        // Return 값 : true이면 Refresh가 필요하다.
        private bool CheckRefresh()
        {
            if (m_dicSensorHistories.Count != dataGridViewSensor.Rows.Count)
                return true;

            int nCount = 0;

            foreach (DataGridViewRow row in dataGridViewSensor.Rows)
            {
                SensorTagHistoryCount history = (SensorTagHistoryCount)row.Tag;

                if (m_dicSensorHistories.TryGetValue(history.Sensor, out nCount) == false)
                    return true;

                if (nCount != history.HistoryCount)
                    return true;
            }

            return false;
        }

        public void ComboSubmit(string group, string building, string floor)
        {
            m_strBuildingGroup = group;
            m_strBuilding = building;
            m_strFloor = floor;
        }

        private void RefreshLabels(DateTime dtBegin, DateTime dtEnd)
        {
            // 조회기간
            lblMinDate.Text = String.Format("{0}년 {1}월 {2}일 부터", dtBegin.Year, dtBegin.Month, dtBegin.Day);
            lblMaxDate.Text = String.Format("{0}년 {1}월 {2}일 까지", dtEnd.Year, dtEnd.Month, dtEnd.Day);

            // 조회범위
            lblBuilding.Text = m_strBuildingGroup + "  " + m_strBuilding + "  " + m_strFloor;
        }

        private void GetSensorGridValues(out double[] values, out string[] labels)
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

            for (int i=0;i<nRowCount;i++)
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

        private int GetTotalHistoryCount(List<EquipZoneHistoryCount> histories)
        {
            int nCount = 0;

            foreach (EquipZoneHistoryCount history in histories)
            {
                nCount += history.HistoryCount;
            }

            return nCount;
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
                DataGridViewRow row = dataGridViewEquipZone.Rows[j];//MakeNewRow(dataGridView1);

                row.Cells[(int)FireEquipZoneGrid.NO_INDEX].Value = row.Index + 1;
                row.Cells[(int)FireEquipZoneGrid.LOCATION_INDEX].Value = GetLocationName(history);
                row.Cells[(int)FireEquipZoneGrid.PERCENT_INDEX].Value = "화재 센서";
                row.Cells[(int)FireEquipZoneGrid.BUILDING_GROUP_INDEX].Value = GetBuildingGroupName(history);
                row.Cells[(int)FireEquipZoneGrid.BUILDING_INDEX].Value = GetBuildingName(history);
                row.Cells[(int)FireEquipZoneGrid.FLOOR_INDEX].Value = GetFloorName(history);
                row.Cells[(int)FireEquipZoneGrid.HISTORY_COUNT_INDEX].Value = history.HistoryCount;
                row.Cells[(int)FireEquipZoneGrid.PERCENT_INDEX].Value = GetHistoryPercent(nTotalHistoryCount, history.HistoryCount);

                row.Tag = history;

                values[j] = history.HistoryCount;
                labels[j] = row.Cells[(int)FireEquipZoneGrid.LOCATION_INDEX].Value.ToString();
            }

            if (nHistoryCount > 0)
                dataGridViewEquipZone.Rows[0].Cells[0].Selected = true;
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

            for (int i=nHistoryCount-1,j=0;i>=0;i--,j++)
            {
                SensorTagHistoryCount history = historyCounts[i];
                DataGridViewRow row = dataGridViewSensor.Rows[j];//MakeNewRow(dataGridView1);

                row.Cells[(int)FireSensorGrid.NO_INDEX].Value = row.Index + 1;
                row.Cells[(int)FireSensorGrid.SENSOR_NAME_INDEX].Value = history.Sensor.TagName;
                row.Cells[(int)FireSensorGrid.BUILDING_GROUP_INDEX].Value = GetBuildingGroupName(history);
                row.Cells[(int)FireSensorGrid.BUILDING_INDEX].Value = GetBuildingName(history);
                row.Cells[(int)FireSensorGrid.FLOOR_INDEX].Value = GetFloorName(history);
                row.Cells[(int)FireSensorGrid.LOCATION_INDEX].Value = GetLocationName(history);
                row.Cells[(int)FireSensorGrid.HISTORY_COUNT_INDEX].Value = history.HistoryCount;
                row.Cells[(int)FireSensorGrid.PERCENT_INDEX].Value = GetHistoryPercent(nTotalHistoryCount, history.HistoryCount);
                
                row.Tag = history;

                values[j] = history.HistoryCount;
                labels[j] = history.Sensor.TagName;
            }

            if (nHistoryCount > 0)
                dataGridViewSensor.Rows[0].Cells[0].Selected = true;
        }

        private string GetPSMMaterialName(SensorTagHistoryCount history)
        {
            if (SensorManager.Instance.DicPSMSensorZone.ContainsKey(history.Sensor.SensorZoneID))
            {
                UnE.PSM.PSMSensorZone sensorZone = (UnE.PSM.PSMSensorZone)SensorManager.Instance.DicPSMSensorZone[history.Sensor.SensorZoneID];

                if (sensorZone != null)
                {
                    if (sensorZone.OrgSensor != null)
                    {
                        if (sensorZone.OrgSensor.LinkedTankList.Count > 0)
                        {
                            if (sensorZone.OrgSensor.LinkedTankList[0].Material != null)
                                return sensorZone.OrgSensor.LinkedTankList[0].Material.Name;
                        }
                    }
                }
            }

            return "-";
        }

        private string GetBuildingGroupName(SensorTagHistoryCount history)
        {
            if (history.Sensor.EquipmentZone == null)
                return "-";

            if (history.Sensor.EquipmentZone.Building == null)
                return "-";

            return history.Sensor.EquipmentZone.Building.BuildingGroup.BuildingGroupName;
        }

        private string GetBuildingGroupName(EquipZoneHistoryCount history)
        {
            if (history.EquipmentZone == null)
                return "-";

            if (history.EquipmentZone.Building == null)
                return "-";

            return history.EquipmentZone.Building.BuildingGroup.BuildingGroupName;
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

        private int GetCurrentPageIndex(int nValueCount, int nViewCount, PageData pageData)
        {
            if (pageData.PageIndexComboBox.SelectedIndex < 0)
                return pageData.CurrentPage;

            pageData.CurrentPage = (int)pageData.PageIndexComboBox.Items[pageData.PageIndexComboBox.SelectedIndex];

            if (nValueCount == 0)
                pageData.TotalPage = 1;
            else
                pageData.TotalPage = (nValueCount - 1) / nViewCount + 1;

            if (pageData.TotalPage != pageData.PageIndexComboBox.Items.Count)
            {
                pageData.PageIndexComboBox.Items.Clear();

                for (int i = 1; i <= pageData.TotalPage; i++)
                {
                    pageData.PageIndexComboBox.Items.Add(i);
                }

                if (pageData.PageIndexComboBox.Items.Count > 0)
                {
                    m_systemCall = true;
                    pageData.PageIndexComboBox.SelectedIndex = 0;
                    m_systemCall = false;
                }
            }

            pageData.TotalPageCountLabel.Text = "/ " + pageData.TotalPage.ToString();
            return pageData.CurrentPage;
        }

        private void SetPageValue(ref double[] values, ref string[] labels, ref double[] resultDatas, int nCurrentPageIndex, int nViewCount)
        {
            int nValueCount = values.Count();

            if (nValueCount <= nViewCount)
                return;

            int nBeginIndex = (nCurrentPageIndex - 1) * nViewCount;
            int nNewValueCount = nBeginIndex + nViewCount <= nValueCount ? nViewCount : nValueCount - nBeginIndex;
            if (nNewValueCount < 0)
            {
                nBeginIndex = 0;
                nNewValueCount = nViewCount;

                m_pageSensor.CurrentPage = 1;
                m_pageEquipZone.CurrentPage = 1;
                SetNavigatorEnable(m_pageSensor);

                m_pageSensor.TotalPageCountLabel.Text = m_pageSensor.CurrentPage + " / " + m_pageSensor.TotalPage.ToString();
                m_pageSensor.TotalPageCountLabel.Location = new Point(panel1.Location.X + panel1.Width - m_pageSensor.TotalPageCountLabel.Width, panel1.Location.Y + panel1.Height + 5);
                m_pageEquipZone.TotalPageCountLabel.Text = m_pageEquipZone.CurrentPage + " / " + m_pageEquipZone.TotalPage.ToString();
                m_pageEquipZone.TotalPageCountLabel.Location = new Point(panel1.Location.X + panel1.Width - m_pageEquipZone.TotalPageCountLabel.Width, panel1.Location.Y + panel1.Height + 5);
            }
            int nEndIndex = nBeginIndex + nNewValueCount;

            double[] newValues = new double[nNewValueCount];
            string[] newLabels = new string[nNewValueCount];
            double[] newResultDatas = new double[nNewValueCount];

            for (int i = nBeginIndex, j=0;i<nEndIndex;i++,j++)
            {
                newValues[j] = values[i];
                newLabels[j] = labels[i];
                newResultDatas[j] = resultDatas[i];
            }

            values = newValues;
            labels = newLabels;
            resultDatas = newResultDatas;
        }

        // 글자가 겹치는 부분이 있는지 검사하여, 겹치는 구간이 있으면 무조건 Text Angle을 20도로 준다.
        private double GetLabelAngle(WinChartViewer chartViewer, DataGridView grid, Font font, string[] labels, int nChartWidth, ref int nChartHeight, ref int nPlotX, ref int nPlotY, ref int nPlotWidth, ref int nPlotHeight)
        {
            int nLabelCount = labels.Count();

            if (nLabelCount <= 1)
            {
                if (grid.Location.Y != m_nGridInitY)
                    MoveGrid(grid, m_nGridInitY);

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

            if (chartViewer.Location.Y + nChartHeight >= m_nGridInitY)
                MoveGrid(grid, chartViewer.Location.Y + nChartHeight);
            else if (grid.Location.Y != m_nGridInitY)
                MoveGrid(grid, m_nGridInitY);

            return dAngle;
        }

        private void MoveGrid(DataGridView grid, int y)
        {
            grid.Location = new Point(grid.Location.X, y);
            grid.Size = new Size(grid.Size.Width, this.Size.Height - grid.Location.Y - m_nGridInitSpaceY);
        }

        // nViewCount : Graph에 최대 몇 개까지의 데이터를 표시할 것인가?
        private void RefreshSensorChart(double[] values, string[] labels, int nViewCount)
        {
            RefreshChart(values, labels, nViewCount, winChartViewerSensor, dataGridViewSensor, SensorChartTag, m_pageSensor);
            //if (values == null || labels == null)
            //{
            //    EmptyChart(winChartViewerSensor, SensorChartTag);
            //    return;
            //}

            //// In the pareto chart, the line data are just the accumulation of the
            //// raw data, scaled to a range of 0 - 100%
            //ArrayMath lineData = new ArrayMath(values);
            //lineData.acc();
            //double scaleFactor = lineData.max() / 100;
            //if (scaleFactor == 0)
            //{
            //    // Avoid division by zero error for zero data
            //    scaleFactor = 1;
            //}
            //lineData.div2(scaleFactor);

            //double[] resultDatas = lineData.result();
            //int nCurrentPageIndex = GetCurrentPageIndex(values.Count(), nViewCount);
            //SetPageValue(ref values, ref labels, ref resultDatas, nCurrentPageIndex, nViewCount);

            //Size sizeGrid = this.dataGridViewSensor.Size;
            //Point ptGrid = this.dataGridViewSensor.Location;
            //int nLeftSpace = 60, nRightSpace = 60, nLeftSpace2 = 127;

            //double maxValue = values.Max();

            //if (maxValue >= 100000)
            //{
            //    int nLog = (int)Math.Log10(maxValue);
            //    nLeftSpace = 60 + (nLog - 4) * 15;
            //}

            //double dLabelAngle = GetLabelAngle(labels);

            //XYChart c = new XYChart(sizeGrid.Width - nLeftSpace2, 280);//, Chart.brushedSilverColor(), 0xbbbbbb, 2);
            //c.setRoundedFrame();
            //c.setDropShadow();

            //c.setPlotArea(ptGrid.X + nLeftSpace, 15, sizeGrid.Width - nLeftSpace - nRightSpace - nLeftSpace2, 235);

            //// Add a line layer for the pareto line
            //LineLayer lineLayer = c.addLineLayer2();

            //// Add the pareto line using deep blue (0000ff) as the color, with circle
            //// symbols
            //lineLayer.addDataSet(resultDatas, 0x0000ff).setDataSymbol(
            //    Chart.CircleShape, 9, 0x0000ff, 0x0000ff);

            //// Set the line width to 2 pixel
            //lineLayer.setLineWidth(2);

            //// Bind the line layer to the secondary (right) y-axis.
            //lineLayer.setUseYAxis2();

            //// Tool tip for the line layer
            ///*lineLayer.setHTMLImageMap("", "",
            //    "title='Top {={x}+1} items: {value|2}%'");*/

            //// Add a multi-color bar layer using the given data.
            //BarLayer barLayer = c.addBarLayer3(values);

            //// Set soft lighting for the bars with light direction from the right
            //barLayer.setBorderColor(Chart.Transparent, Chart.softLighting(Chart.Right));

            //barLayer.setBarWidth(40); // Bar의 두께 설정

            //// Tool tip for the bar layer
            ////barLayer.setHTMLImageMap("", "", "title='{xLabel}: {value} pieces'");

            //// Set the labels on the x axis.
            //c.xAxis().setLabels(labels);

            //// Set the secondary (right) y-axis scale as 0 - 100 with a tick every 20
            //// units
            //c.yAxis2().setLinearScale(0, 100, 20);

            //// Set the format of the secondary (right) y-axis label to include a
            //// percentage sign
            //c.yAxis2().setLabelFormat("{value}%");

            //// Set the relationship between the two y-axes, which only differ by a
            //// scaling factor
            //c.yAxis().syncAxis(c.yAxis2(), scaleFactor);

            //// Set the format of the primary y-axis label foramt to show no decimal
            //// point
            //c.yAxis().setLabelFormat("{value|0,}");

            //// Add a title to the primary y-axis
            //c.yAxis().setTitle("Frequency");

            //// Set all axes to transparent
            //c.xAxis().setColors(Chart.Transparent);
            //c.yAxis().setColors(Chart.Transparent);
            //c.yAxis2().setColors(Chart.Transparent);

            //c.yAxis().setTitle(SensorChartTag, "Arial Bold", 11.25);
            //c.yAxis().setLabelStyle("Arial", 10.75, 0x000000, dLabelAngle);

            ////c.xAxis().setLabels(m_nCurrentPage < 0 ? null : dicLabels[m_nCurrentPage]);
            ////c.xAxis().setLabelStyle("Arial", 10.75, 0x000000, dbAngle);

            //// Output the chart
            //winChartViewerSensor.Chart = c;
            //SetNavigatorEnable();
        }

        // nViewCount : Graph에 최대 몇 개까지의 데이터를 표시할 것인가?
        private void RefreshEquipZoneChart(double[] values, string[] labels, int nViewCount)
        {
            RefreshChart(values, labels, nViewCount, winChartViewerEquipZone, dataGridViewEquipZone, EquipZoneChartTag, m_pageEquipZone);
            //if (values == null || labels == null)
            //{
            //    EmptyChart(winChartViewerEquipZone, EquipZoneChartTag);
            //    return;
            //}

            //// In the pareto chart, the line data are just the accumulation of the
            //// raw data, scaled to a range of 0 - 100%
            //ArrayMath lineData = new ArrayMath(values);
            //lineData.acc();
            //double scaleFactor = lineData.max() / 100;
            //if (scaleFactor == 0)
            //{
            //    // Avoid division by zero error for zero data
            //    scaleFactor = 1;
            //}
            //lineData.div2(scaleFactor);

            //double[] resultDatas = lineData.result();
            //int nCurrentPageIndex = GetCurrentPageIndex(values.Count(), nViewCount);
            //SetPageValue(ref values, ref labels, ref resultDatas, nCurrentPageIndex, nViewCount);

            //Size sizeGrid = this.dataGridViewSensor.Size;
            //Point ptGrid = this.dataGridViewSensor.Location;
            //int nLeftSpace = 60, nRightSpace = 60, nLeftSpace2 = 127;

            //double maxValue = values.Max();

            //if (maxValue >= 100000)
            //{
            //    int nLog = (int)Math.Log10(maxValue);
            //    nLeftSpace = 60 + (nLog - 4) * 15;
            //}

            //double dLabelAngle = GetLabelAngle(labels);

            //XYChart c = new XYChart(sizeGrid.Width - nLeftSpace2, 280);//, Chart.brushedSilverColor(), 0xbbbbbb, 2);
            //c.setRoundedFrame();
            //c.setDropShadow();

            //c.setPlotArea(ptGrid.X + nLeftSpace, 15, sizeGrid.Width - nLeftSpace - nRightSpace - nLeftSpace2, 235);

            //// Add a line layer for the pareto line
            //LineLayer lineLayer = c.addLineLayer2();

            //// Add the pareto line using deep blue (0000ff) as the color, with circle
            //// symbols
            //lineLayer.addDataSet(resultDatas, 0x0000ff).setDataSymbol(
            //    Chart.CircleShape, 9, 0x0000ff, 0x0000ff);

            //// Set the line width to 2 pixel
            //lineLayer.setLineWidth(2);

            //// Bind the line layer to the secondary (right) y-axis.
            //lineLayer.setUseYAxis2();

            //// Tool tip for the line layer
            ///*lineLayer.setHTMLImageMap("", "",
            //    "title='Top {={x}+1} items: {value|2}%'");

            //// Add a multi-color bar layer using the given data.
            //BarLayer barLayer = c.addBarLayer3(values);

            //// Set soft lighting for the bars with light direction from the right
            //barLayer.setBorderColor(Chart.Transparent, Chart.softLighting(Chart.Right));

            //barLayer.setBarWidth(40); // Bar의 두께 설정

            //// Tool tip for the bar layer
            ////barLayer.setHTMLImageMap("", "", "title='{xLabel}: {value} pieces'");

            //// Set the labels on the x axis.
            //c.xAxis().setLabels(labels);

            //// Set the secondary (right) y-axis scale as 0 - 100 with a tick every 20
            //// units
            //c.yAxis2().setLinearScale(0, 100, 20);

            //// Set the format of the secondary (right) y-axis label to include a
            //// percentage sign
            //c.yAxis2().setLabelFormat("{value}%");

            //// Set the relationship between the two y-axes, which only differ by a
            //// scaling factor
            //c.yAxis().syncAxis(c.yAxis2(), scaleFactor);

            //// Set the format of the primary y-axis label foramt to show no decimal
            //// point
            //c.yAxis().setLabelFormat("{value|0,}");

            //// Add a title to the primary y-axis
            //c.yAxis().setTitle("Frequency");

            //// Set all axes to transparent
            //c.xAxis().setColors(Chart.Transparent);
            //c.yAxis().setColors(Chart.Transparent);
            //c.yAxis2().setColors(Chart.Transparent);

            //c.yAxis().setTitle(EquipZoneChartTag, "Arial Bold", 11.25);
            //c.yAxis().setLabelStyle("Arial", 10.75, 0x000000, dLabelAngle);

            ////c.xAxis().setLabels(m_nCurrentPage < 0 ? null : dicLabels[m_nCurrentPage]);
            ////c.xAxis().setLabelStyle("Arial", 10.75, 0x000000, dbAngle);

            //// Output the chart
            //winChartViewerEquipZone.Chart = c;
            //SetNavigatorEnable();
        }

        // nViewCount : Graph에 최대 몇 개까지의 데이터를 표시할 것인가?
        private void RefreshChart(double[] values, string[] labels, int nViewCount, WinChartViewer chartViewer, DataGridView grid, string strChartTag, PageData pageData)
        {
            if (values == null || labels == null)
            {
                EmptyChart(chartViewer, grid, EquipZoneChartTag);
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
            int nCurrentPageIndex = GetCurrentPageIndex(values.Count(), nViewCount, pageData);
            SetPageValue(ref values, ref labels, ref resultDatas, nCurrentPageIndex, nViewCount);

            Size sizeGrid = this.dataGridViewSensor.Size;
            Point ptGrid = this.dataGridViewSensor.Location;
            int nLeftSpace = 60, nRightSpace = 60, nLeftSpace2 = 127;

            double maxValue = values.Max();

            if (maxValue >= 100000)
            {
                int nLog = (int)Math.Log10(maxValue);
                nLeftSpace = 60 + (nLog - 4) * 15;
            }

            float fXFontHeight = 10.75f;
            Font fontXAxis = new System.Drawing.Font("Arial", fXFontHeight);

            int nPlotWidth = sizeGrid.Width - nLeftSpace - nRightSpace - nLeftSpace2, nPlotHeight = 235;
            int nChartWidth = sizeGrid.Width - nLeftSpace2, nChartHeight = 280;
            int nPlotX = ptGrid.X + nLeftSpace, nPlotY = 15;

            double dLabelAngle = GetLabelAngle(chartViewer, grid, fontXAxis, labels, nChartWidth, ref nChartHeight, ref nPlotX, ref nPlotY, ref nPlotWidth, ref nPlotHeight);

            XYChart c = new XYChart(nChartWidth, nChartHeight);//, Chart.brushedSilverColor(), 0xbbbbbb, 2);
            c.setRoundedFrame();
            c.setDropShadow();

            c.setPlotArea(nPlotX, nPlotY, nPlotWidth, nPlotHeight);

            // Add a line layer for the pareto line
            LineLayer lineLayer = c.addLineLayer2();

            // Add the pareto line using deep blue (0000ff) as the color, with circle
            // symbols
            lineLayer.addDataSet(resultDatas, 0x0000ff).setDataSymbol(
                Chart.CircleShape, 9, 0x0000ff, 0x0000ff);

            // Set the line width to 2 pixel
            lineLayer.setLineWidth(2);

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
            barLayer.setAggregateLabelStyle("Arial Bold", 11.25, 0x000000, 0).setAlignment(Chart.Center);

            // Tool tip for the bar layer
            //barLayer.setHTMLImageMap("", "", "title='{xLabel}: {value} pieces'");

            // Set the labels on the x axis.
            c.xAxis().setLabels(labels);
            c.xAxis().setLabelStyle(fontXAxis.FontFamily.Name, fXFontHeight, 0x000000, dLabelAngle);

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
            c.yAxis().setTitle("Frequency");

            // Set all axes to transparent
            c.xAxis().setColors(Chart.Transparent);
            c.yAxis().setColors(Chart.Transparent);
            c.yAxis2().setColors(Chart.Transparent);

            c.yAxis().setTitle(EquipZoneChartTag, "Arial Bold", 11.25);
            c.yAxis().setLabelStyle("Arial", 10.75, 0x000000);

            //c.xAxis().setLabels(m_nCurrentPage < 0 ? null : dicLabels[m_nCurrentPage]);
            //c.xAxis().setLabelStyle("Arial", 10.75, 0x000000, dbAngle);

            // Output the chart
            chartViewer.Chart = c;
            SetNavigatorEnable(pageData);
        }

        private void EmptyChart(WinChartViewer chartViewer, DataGridView grid, string strChartTag)
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

            if (grid.Location.Y != m_nGridInitY)
                MoveGrid(grid, m_nGridInitY);

            Size sizeGrid = grid.Size;
            Point ptGrid = grid.Location;
            int nSpace = 60, nLeftSpace2 = 127;

            XYChart c = new XYChart(sizeGrid.Width - nLeftSpace2, 280);
            c.setRoundedFrame();
            c.setDropShadow();

            // Tentatively set the plotarea at (50, 40). Set the width to 100 pixels
            // less than the chart width, and the height to 80 pixels less than the
            // chart height. Use pale grey (f4f4f4) background, transparent border,
            // and dark grey (444444) dotted grid lines.
            //c.setPlotArea(50, 40, c.getWidth() - 100, c.getHeight() - 80, 0xf4f4f4,
            //    -1, Chart.Transparent, c.dashLineColor(0x444444, Chart.DotLine));
            c.setPlotArea(ptGrid.X + nSpace, 15, sizeGrid.Width - nSpace * 2 - nLeftSpace2, 235);

            // Add a line layer for the pareto line
            LineLayer lineLayer = c.addLineLayer2();

            // Add the pareto line using deep blue (0000ff) as the color, with circle
            // symbols
            lineLayer.addDataSet(lineData.result(), 0x0000ff).setDataSymbol(
                Chart.CircleShape, 9, 0x0000ff, 0x0000ff);

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

            c.yAxis().setTitle(strChartTag, "Arial Bold", 11.25);
            c.yAxis().setLabelStyle("Arial", 10.75, 0x000000);

            // Output the chart
            chartViewer.Chart = c;
        }

        public static void AddRows(DataGridView grid, int nRowCount)
        {
            if (nRowCount > 0)
                grid.Rows.Add();

            if (nRowCount > 1)
                grid.Rows.AddCopies(0, nRowCount - 1);
        }

        private void SetNavigatorEnable(PageData pageData)
        {
            if (pageData.CurrentPage <= 1)
                pageData.PreviousButton.Enabled = false;
            else
                pageData.PreviousButton.Enabled = true;

            if (pageData.CurrentPage == pageData.TotalPage)
                pageData.NextButton.Enabled = false;
            else
                pageData.NextButton.Enabled = true;

            pageData.PageIndexComboBox.Enabled = pageData.PreviousButton.Enabled || pageData.NextButton.Enabled;
        }

        private void cboPageIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_systemCall)
                return;

            SetNavigatorEnable(sender == m_pageSensor.PageIndexComboBox ? m_pageSensor : m_pageEquipZone);

            if (dataGridViewSensor.Rows.Count == 0)
            {
                EmptyChart(winChartViewerSensor, dataGridViewSensor, SensorChartTag);
                EmptyChart(winChartViewerEquipZone, dataGridViewEquipZone, EquipZoneChartTag);
            }
            else
            {
                double[] values;
                string[] labels;

                GetSensorGridValues(out values, out labels);
                RefreshSensorChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());

                GetEquipZoneGridValues(out values, out labels);
                RefreshEquipZoneChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());
            }
        }

        private void btnPreviousIndex_Click(object sender, EventArgs e)
        {
            PageData pageData = sender == m_pageSensor.PreviousButton ? m_pageSensor : m_pageEquipZone;

            if (pageData.CurrentPage == 1)
                return;

            pageData.CurrentPage--;

            pageData.PageIndexComboBox.SelectedIndex = pageData.CurrentPage - 1;
        }

        private void btnNextIndex_Click(object sender, EventArgs e)
        {
            PageData pageData = sender == m_pageSensor.NextButton ? m_pageSensor : m_pageEquipZone;

            if (pageData.CurrentPage == pageData.TotalPage)
                return;

            pageData.CurrentPage++;

            pageData.PageIndexComboBox.SelectedIndex = pageData.CurrentPage - 1;
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

        private void ParetoPage_Load(object sender, EventArgs e)
        {
            cboChart.SelectedIndex = (int)ChartType.SENSOR;
            InitLoadData();
        }

        private void InitLoadData()
        {
            DateTime startDate, EndDate;
            int nSplitUnitOfMeasure, nSplitUnitOfMeasureDetail, nViewCount;

            if (!FormMain.Instance.GetCurrentReportDate(out startDate, out EndDate))
                return;

            FormMain.Instance.GetCurrentReportOption(out nSplitUnitOfMeasure, out nSplitUnitOfMeasureDetail, out nViewCount);

            string strBuildingGroup = "모든 건물 그룹";
            string strBuilding = "모든 건물";
            string strFloor = "모든 층";
            ArrayList arrSelectedZone = ZoneManager.Instance.FindZoneList(strBuildingGroup, strBuilding, strFloor);

            //설정한 기간, ZoneList를 ZoneSubmit함수에 넘겨줌
            m_detectMgr.ZoneSubmit(arrSelectedZone, startDate, EndDate);
            
            //찾은 검색결과를 DataGrid로 출력
            Load_DataGrid(arrSelectedZone, strBuildingGroup, strBuilding, strFloor, nSplitUnitOfMeasure, nSplitUnitOfMeasureDetail, nViewCount);
            //Load_DataGrid(arrSelectedZone, FormMain.Instance.GetReportChartMaxItemCount());
        }

        private void ParetoPage_Resize(object sender, EventArgs e)
        {
            Rectangle rect = ClientRectangle;

            if (Width == 0 || Height == 0)
                return;


            int width = rect.Width - 100;
            if (width < 200)
            {
                width = 200;
            }

            double[] values = null;
            string[] labels = null;

            GetSensorGridValues(out values, out labels);
            RefreshSensorChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());

            GetEquipZoneGridValues(out values, out labels);
            RefreshEquipZoneChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());
        }

        private void cboChart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_systemCall)
                return;

            if (cboChart.SelectedIndex == (int)ChartType.SENSOR)
            {
                btnPreviousIndexSensor.Visible = cboPageIndexSensor.Visible = lblTotalPageSensor.Visible = btnNextIndexSensor.Visible = true;
                btnPreviousIndexEquipZone.Visible = cboPageIndexEquipZone.Visible = lblTotalPageEquipZone.Visible = btnNextIndexEquipZone.Visible = false;

                winChartViewerSensor.Visible = dataGridViewSensor.Visible = true;
                winChartViewerEquipZone.Visible = dataGridViewEquipZone.Visible = false;
                lblDescription.Text = "작동 빈도가 높은 센서들부터 왼쪽에서 오른쪽 방향으로 표시합니다.";
            }
            else if (cboChart.SelectedIndex == (int)ChartType.EQUIPZONE)
            {
                btnPreviousIndexSensor.Visible = cboPageIndexSensor.Visible = lblTotalPageSensor.Visible = btnNextIndexSensor.Visible = false;
                btnPreviousIndexEquipZone.Visible = cboPageIndexEquipZone.Visible = lblTotalPageEquipZone.Visible = btnNextIndexEquipZone.Visible = true;

                winChartViewerSensor.Visible = dataGridViewSensor.Visible = false;
                winChartViewerEquipZone.Visible = dataGridViewEquipZone.Visible = true;
                lblDescription.Text = "작동 빈도가 높은 장소들부터 왼쪽에서 오른쪽 방향으로 표시합니다.";
            }
        }

        //이미지 캡쳐
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

            if (grid == dataGridViewSensor)
                nExceptColumnIndex = (int)FireSensorGrid.BUILDING_GROUP_INDEX;
            else if (grid == dataGridViewEquipZone)
                nExceptColumnIndex = (int)FireEquipZoneGrid.BUILDING_GROUP_INDEX;

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

        private void btnSaveHWP_Click(object sender, EventArgs e)
        {
            PageBackstageHome.Instance.FrmReport.SaveHWPForPareto(this);
        }

        public string GetHWPFileName()
        {
            return "화재_탐지분석_보고서";
        }

        public SDMS.Data.ReportMode GetReportMode()
        {
            return Data.ReportMode.DetectFireAnalyze;
        }

        public void SetVisibleHWPExport(bool visible)
        {
            btnSaveHWP.Visible = visible;
        }
    }
}
