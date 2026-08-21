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
using UnE.Spatial;

namespace SDMS
{
    public partial class ParetoPSMPage : Form, SDMS.Data.IParetoPage
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

        private class TankHistoryCount : IComparable
        {
            private UnE.PSM.PSMTank m_tank = null;
            private int m_nHistoryCount = 0;

            public UnE.PSM.PSMTank Tank
            {
                get { return m_tank; }
                set { m_tank = value; }
            }

            public int HistoryCount
            {
                get { return m_nHistoryCount; }
                set { m_nHistoryCount = value; }
            }

            public int CompareTo(object obj)
            {
                TankHistoryCount history = (TankHistoryCount)obj;

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

        private class MaterialHistoryCount : IComparable
        {
            private UnE.PSM.PSMMaterial m_material = null;
            private int m_nHistoryCount = 0;

            public UnE.PSM.PSMMaterial Material
            {
                get { return m_material; }
                set { m_material = value; }
            }

            public int HistoryCount
            {
                get { return m_nHistoryCount; }
                set { m_nHistoryCount = value; }
            }

            public int CompareTo(object obj)
            {
                MaterialHistoryCount history = (MaterialHistoryCount)obj;

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

            public void Show()
            {
                this.btnPrevious.Visible = this.btnNext.Visible = this.cboPageIndex.Visible = this.lblTotalPage.Visible = true;
            }

            public void Hide()
            {
                this.btnPrevious.Visible = this.btnNext.Visible = this.cboPageIndex.Visible = this.lblTotalPage.Visible = false;
            }

            public void SetLocation(PageData data)
            {
                this.btnPrevious.Location = data.btnPrevious.Location;
                this.btnNext.Location = data.btnNext.Location;
                this.cboPageIndex.Location = data.cboPageIndex.Location;
                this.lblTotalPage.Location = data.lblTotalPage.Location;
            }
        }

        public enum ChartType { SENSOR, TANK, EQUIPZONE, MATERIAL };

        public enum PSMSensorGrid
        {
            NO_INDEX = 0,
            SENSOR_NAME_INDEX,
            MATERIAL_INDEX,
            BUILDING_INDEX,
            LOCATION_INDEX,
            HISTORY_COUNT_INDEX,
            PERCENT_INDEX
        }

        public enum PSMTankGrid
        {
            NO_INDEX = 0,
            TANK_NAME_INDEX,
            MATERIAL_INDEX,
            BUILDING_INDEX,
            LOCATION_INDEX,
            HISTORY_COUNT_INDEX,
            PERCENT_INDEX
        }

        public enum PSMEquipZoneGrid
        {
            NO_INDEX = 0,
            LOCATION_INDEX,
            BUILDING_INDEX,
            HISTORY_COUNT_INDEX,
            PERCENT_INDEX
        }

        public enum PSMMaterialGrid
        {
            NO_INDEX = 0,
            MATERIAL_INDEX,
            HISTORY_COUNT_INDEX,
            PERCENT_INDEX
        }

        // 센서별 알람 History Count
        private Dictionary<SensorTagInfo, int> m_dicSensorHistories = new Dictionary<SensorTagInfo, int>();
        // 탱크별 알람 History Count
        private Dictionary<UnE.PSM.PSMTank, int> m_dicTankHistories = new Dictionary<UnE.PSM.PSMTank, int>();
        // 위치별 알람 History Count
        private Dictionary<UnE.Spatial.EquipmentZone, int> m_dicEquipZoneHistories = new Dictionary<UnE.Spatial.EquipmentZone, int>();
        // 물질별 알람 History Count
        private Dictionary<UnE.PSM.PSMMaterial, int> m_dicMaterialHistories = new Dictionary<UnE.PSM.PSMMaterial, int>();

        private const string SensorChartTag = "탐지 횟수";
        private const string TankChartTag = "탐지 횟수";
        private const string EquipZoneChartTag = "탐지 횟수";
        private const string MaterialChartTag = "탐지 횟수";

        private const string HWP_SENSOR_TAG = "ParetoSensor";
        private const string HWP_TANK_TAG = "ParetoTank";
        private const string HWP_EQUIPZONE_TAG = "ParetoEquipZone";
        private const string HWP_MATERIAL_TAG = "ParetoMaterial";

        // 현재 Grid에 나타나있는 데이터들에 대한 SensorTagHistory의 시작과 끝 Index
        private int m_nBeginIndex = -1, m_nEndIndex = -1;
        private Dictionary<int, UnE.Spatial.Zone> m_dicCurrentZones = new Dictionary<int, UnE.Spatial.Zone>();
        // 현재 Chart에 적용된 최대 표시 개수
        private int m_nViewCount = 20;

        //private int m_nCurrentPage = 1, m_nTotalPage = 1;

        private string m_strLocation = "모든 시설";

        private Report.ReactionPSMManager m_detectPSMMgr = null;
        private bool m_systemCall = false;

        private int m_nGridInitY = 0, m_nGridInitSpaceY = 0;

        private PageData m_pageSensor = null;
        private PageData m_pageTank = null;
        private PageData m_pageEquipZone = null;
        private PageData m_pageMaterial = null;

        private DetectPSMPage m_detectPSMPage = null;

        // 센서별 알람 History Count
        public Dictionary<SensorTagInfo, int> SensorHistories
        {
            get { return m_dicSensorHistories; }
        }

        // 물질별 알람 History Count
        public Dictionary<UnE.PSM.PSMMaterial, int> MaterialHistories
        {
            get { return m_dicMaterialHistories; }
        }

        // 탱크별 알람 History Count
        public Dictionary<UnE.PSM.PSMTank, int> TankHistories
        {
            get { return m_dicTankHistories; }
        }

        // 위치별 알람 History Count
        public Dictionary<UnE.Spatial.EquipmentZone, int> EquipZoneHistories
        {
            get { return m_dicEquipZoneHistories; }
        }

        public int ViewCount
        {
            get { return m_nViewCount; }
            set { m_nViewCount = value; }
        }

        public ParetoPSMPage(Report.ReactionPSMManager detectMgr, DetectPSMPage detectPage)
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            FormMain.SetDoubleBuffer(dataGridViewSensor, true);
            FormMain.SetDoubleBuffer(dataGridViewTank, true);
            FormMain.SetDoubleBuffer(dataGridViewEquipZone, true);
            FormMain.SetDoubleBuffer(dataGridViewMaterial, true);

            m_detectPSMMgr = detectMgr;
            m_detectPSMPage = detectPage;

            SetGridFont(dataGridViewSensor, "맑은 고딕", 12.0f);
            SetGridFont(dataGridViewTank, "맑은 고딕", 12.0f);
            SetGridFont(dataGridViewEquipZone, "맑은 고딕", 12.0f);
            SetGridFont(dataGridViewMaterial, "맑은 고딕", 12.0f);

            SetPSMSensorColumns();
            SetPSMTankColumns();
            SetPSMEquipZoneColumns();
            SetPSMMaterialColumns();

            SetPageData();
            SetPageIndices(m_pageSensor);
            SetPageIndices(m_pageTank);
            SetPageIndices(m_pageEquipZone);
            SetPageIndices(m_pageMaterial);

            cboChart_SelectedIndexChanged(null, null);
        }

        private void SetPageData()
        {
            m_pageSensor = new PageData(btnPreviousIndexSensor, btnNextIndexSensor, cboPageIndexSensor, lblTotalPageSensor);
            m_pageTank = new PageData(btnPreviousIndexTank, btnNextIndexTank, cboPageIndexTank, lblTotalPageTank);
            m_pageEquipZone = new PageData(btnPreviousIndexEquipZone, btnNextIndexEquipZone, cboPageIndexEquipZone, lblTotalPageEquipZone);
            m_pageMaterial = new PageData(btnPreviousIndexMaterial, btnNextIndexMaterial, cboPageIndexMaterial, lblTotalPageMaterial);

            m_pageTank.SetLocation(m_pageSensor);
            m_pageEquipZone.SetLocation(m_pageSensor);
            m_pageMaterial.SetLocation(m_pageSensor);
        }

        private void SetGridFont(DataGridView grid, string strFontName, float fFontSize)
        {
            grid.Font = new Font(strFontName, fFontSize);
            m_nGridInitY = grid.Location.Y;
            m_nGridInitSpaceY = this.Size.Height - (grid.Location.Y + grid.Size.Height);
        }

        private void SetPSMSensorColumns()
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

            DataGridViewTextBoxColumn colMaterial = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colMaterial.DefaultCellStyle = cellStyle;
            colMaterial.HeaderText = "물질";
            colMaterial.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colMaterial.Width = 180;
            dataGridViewSensor.Columns.Add(colMaterial);

            DataGridViewTextBoxColumn colBuilding = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colBuilding.DefaultCellStyle = cellStyle;
            colBuilding.HeaderText = "건물";
            colBuilding.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colBuilding.Width = 334;
            dataGridViewSensor.Columns.Add(colBuilding);

            DataGridViewTextBoxColumn colLocation = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            colLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colLocation.DefaultCellStyle = cellStyle;
            colLocation.HeaderText = "누출 발생장소";
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

        private void SetPSMTankColumns()
        {
            DataGridViewTextBoxColumn colNo = new DataGridViewTextBoxColumn();
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNo.DefaultCellStyle = cellStyle;
            colNo.HeaderText = "No";
            colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colNo.Width = 90;
            dataGridViewTank.Columns.Add(colNo);

            DataGridViewTextBoxColumn colSensorName = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new Padding(5, 0, 0, 0);
            colSensorName.DefaultCellStyle = cellStyle;
            colSensorName.HeaderText = "탱크 이름";
            colSensorName.SortMode = DataGridViewColumnSortMode.NotSortable;
            colSensorName.Width = 240;
            dataGridViewTank.Columns.Add(colSensorName);

            DataGridViewTextBoxColumn colMaterial = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colMaterial.DefaultCellStyle = cellStyle;
            colMaterial.HeaderText = "물질";
            colMaterial.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colMaterial.Width = 180;
            dataGridViewTank.Columns.Add(colMaterial);

            DataGridViewTextBoxColumn colBuilding = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colBuilding.DefaultCellStyle = cellStyle;
            colBuilding.HeaderText = "건물";
            colBuilding.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colBuilding.Width = 334;
            dataGridViewTank.Columns.Add(colBuilding);

            DataGridViewTextBoxColumn colLocation = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            colLocation.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colLocation.DefaultCellStyle = cellStyle;
            colLocation.HeaderText = "누출 발생장소";
            colLocation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dataGridViewTank.Columns.Add(colLocation);

            DataGridViewTextBoxColumn colHistoryCount = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colHistoryCount.DefaultCellStyle = cellStyle;
            colHistoryCount.HeaderText = "탐지횟수";
            colHistoryCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dataGridViewTank.Columns.Add(colHistoryCount);

            DataGridViewTextBoxColumn colPercent = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colPercent.DefaultCellStyle = cellStyle;
            colPercent.HeaderText = "백분율(%)";
            colPercent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colPercent.Width = 90;
            dataGridViewTank.Columns.Add(colPercent);

            foreach (DataGridViewColumn column in dataGridViewTank.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void SetPSMEquipZoneColumns()
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
            colLocation.HeaderText = "누출 발생장소";
            colLocation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dataGridViewEquipZone.Columns.Add(colLocation);

            DataGridViewTextBoxColumn colBuilding = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            colBuilding.DefaultCellStyle = cellStyle;
            colBuilding.HeaderText = "건물";
            colBuilding.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colBuilding.Width = 334;
            dataGridViewEquipZone.Columns.Add(colBuilding);

            DataGridViewTextBoxColumn colHistoryCount = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colHistoryCount.DefaultCellStyle = cellStyle;
            colHistoryCount.HeaderText = "탐지횟수";
            colHistoryCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dataGridViewEquipZone.Columns.Add(colHistoryCount);

            DataGridViewTextBoxColumn colPercent = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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

        private void SetPSMMaterialColumns()
        {
            DataGridViewTextBoxColumn colNo = new DataGridViewTextBoxColumn();
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colNo.DefaultCellStyle = cellStyle;
            colNo.HeaderText = "No";
            colNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colNo.Width = 90;
            dataGridViewMaterial.Columns.Add(colNo);

            DataGridViewTextBoxColumn colMaterial = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colMaterial.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            colMaterial.DefaultCellStyle = cellStyle;
            colMaterial.HeaderText = "물질";
            colMaterial.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colMaterial.Width = 180;
            dataGridViewMaterial.Columns.Add(colMaterial);

            DataGridViewTextBoxColumn colHistoryCount = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            colHistoryCount.DefaultCellStyle = cellStyle;
            colHistoryCount.HeaderText = "탐지횟수";
            colHistoryCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            dataGridViewMaterial.Columns.Add(colHistoryCount);

            DataGridViewTextBoxColumn colPercent = new DataGridViewTextBoxColumn();
            cellStyle = new DataGridViewCellStyle();
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colPercent.DefaultCellStyle = cellStyle;
            colPercent.HeaderText = "백분율(%)";
            colPercent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            colPercent.Width = 90;
            dataGridViewMaterial.Columns.Add(colPercent);

            foreach (DataGridViewColumn column in dataGridViewMaterial.Columns)
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
        public void Load_DataGrid(ArrayList arrSelectZoneList, int nSplitUnitOfMeasure, int nSplitUnitOfMeasureDetail, int nViewCount)
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
            bool needRefresh = GetDataFromDetectPage(arrSelectZoneList, nSplitUnitOfMeasure, nSplitUnitOfMeasureDetail, startDate, endDate, nViewCount);
            //bool needRefresh = SensorTagHistoryManager.Instance.LoadPSMSensorTagHistories(arrSelectZoneList, m_dicCurrentZones, m_dicSensorHistories, m_dicTankHistories, m_dicEquipZoneHistories, m_dicMaterialHistories, ref m_nBeginIndex, ref m_nEndIndex, FormMain.Instance.DBManager, startDate, endDate, UnE.Sensor.IFacility.FacilityType.PSM_SENSOR);
            RefreshLabels(startDate, endDate);

            if (needRefresh)
            {
                m_nViewCount = nViewCount;

                double[] values = null;
                string[] labels = null;

                RefreshSensorGrid(out values, out labels);
                RefreshSensorChart(values, labels, nViewCount);

                RefreshTankGrid(out values, out labels);
                RefreshTankChart(values, labels, nViewCount);

                RefreshEquipZoneGrid(out values, out labels);
                RefreshEquipZoneChart(values, labels, nViewCount);

                RefreshMaterialGrid(out values, out labels);
                RefreshMaterialChart(values, labels, nViewCount);
            }
            else if (m_nViewCount != nViewCount)
            {
                m_nViewCount = nViewCount;

                double[] values = null;
                string[] labels = null;

                GetSensorGridValues(out values, out labels);
                RefreshSensorChart(values, labels, nViewCount);

                GetTankGridValues(out values, out labels);
                RefreshTankChart(values, labels, nViewCount);

                GetEquipZoneGridValues(out values, out labels);
                RefreshEquipZoneChart(values, labels, nViewCount);

                GetMaterialGridValues(out values, out labels);
                RefreshMaterialChart(values, labels, nViewCount);
            }
        }

        // Return 값 : Refresh가 필요한가?
        private bool GetDataFromDetectPage(ArrayList arrSelectZoneList, int nSplitUnitOfMeasure, int nSplitUnitOfMeasureDetail, DateTime startDate, DateTime endDate, int nViewCount)
        {
            bool needRefresh = false;

            if (m_detectPSMMgr.NeedRefresh(arrSelectZoneList, startDate, endDate, m_detectPSMPage.RefreshCheckData))
            {
                m_detectPSMPage.RefreshCheckData.ViewCount = nViewCount;

                m_detectPSMMgr.DataClear();
                m_detectPSMMgr.ZoneSubmit(arrSelectZoneList, startDate, endDate);

                m_detectPSMPage.Load_DataGrid(m_dicSensorHistories, m_dicTankHistories, m_dicEquipZoneHistories, m_dicMaterialHistories);

                //그래프그리기
                m_detectPSMPage.CreateBarChart(startDate, endDate, nSplitUnitOfMeasure, nSplitUnitOfMeasureDetail, nViewCount);
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

        public void ComboSubmit(string strLocationName)
        {
            m_strLocation = strLocationName;
        }

        private void RefreshLabels(DateTime dtBegin, DateTime dtEnd)
        {
            // 조회기간
            lblMinDate.Text = String.Format("{0}년 {1}월 {2}일 부터", dtBegin.Year, dtBegin.Month, dtBegin.Day);
            lblMaxDate.Text = String.Format("{0}년 {1}월 {2}일 까지", dtEnd.Year, dtEnd.Month, dtEnd.Day);

            // 조회범위
            lblBuilding.Text = m_strLocation;
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

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridViewSensor.Rows[i];
                SensorTagHistoryCount history = (SensorTagHistoryCount)row.Tag;

                values[i] = history.HistoryCount;
                labels[i] = history.Sensor.TagName;
            }
        }

        private void GetTankGridValues(out double[] values, out string[] labels)
        {
            int nRowCount = dataGridViewTank.Rows.Count;

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
                DataGridViewRow row = dataGridViewTank.Rows[i];
                TankHistoryCount history = (TankHistoryCount)row.Tag;

                values[i] = history.HistoryCount;
                labels[i] = history.Tank.Name;
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

        private void GetMaterialGridValues(out double[] values, out string[] labels)
        {
            int nRowCount = dataGridViewMaterial.Rows.Count;

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
                DataGridViewRow row = dataGridViewMaterial.Rows[i];
                MaterialHistoryCount history = (MaterialHistoryCount)row.Tag;

                values[i] = history.HistoryCount;
                labels[i] = history.Material.Name;
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

        private int GetTotalHistoryCount(List<TankHistoryCount> histories)
        {
            int nCount = 0;

            foreach (TankHistoryCount history in histories)
            {
                nCount += history.HistoryCount;
            }

            return nCount;
        }

        private int GetTotalHistoryCount(List<MaterialHistoryCount> histories)
        {
            int nCount = 0;

            foreach (MaterialHistoryCount history in histories)
            {
                nCount += history.HistoryCount;
            }

            return nCount;
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
                DataGridViewRow row = dataGridViewSensor.Rows[j];//MakeNewRow(dataGridView1);

                row.Cells[(int)PSMSensorGrid.NO_INDEX].Value = row.Index + 1;
                row.Cells[(int)PSMSensorGrid.SENSOR_NAME_INDEX].Value = history.Sensor.TagName;
                row.Cells[(int)PSMSensorGrid.MATERIAL_INDEX].Value = GetPSMMaterialName(history);
                row.Cells[(int)PSMSensorGrid.BUILDING_INDEX].Value = GetBuildingName(history);
                row.Cells[(int)PSMSensorGrid.LOCATION_INDEX].Value = GetLocationName(history);
                row.Cells[(int)PSMSensorGrid.HISTORY_COUNT_INDEX].Value = history.HistoryCount;
                row.Cells[(int)PSMSensorGrid.PERCENT_INDEX].Value = ParetoPage.GetHistoryPercent(nTotalHistoryCount, history.HistoryCount);
            
                row.Tag = history;

                values[j] = history.HistoryCount;
                labels[j] = history.Sensor.TagName;
            }

            if (nHistoryCount > 0)
                dataGridViewSensor.Rows[0].Cells[0].Selected = true;
        }

        private void RefreshTankGrid(out double[] values, out string[] labels)
        {
            List<TankHistoryCount> historyCounts = new List<TankHistoryCount>();

            foreach (KeyValuePair<UnE.PSM.PSMTank, int> pair in m_dicTankHistories)
            {
                TankHistoryCount history = new TankHistoryCount();
                history.Tank = pair.Key;
                history.HistoryCount = pair.Value;

                historyCounts.Add(history);
            }

            historyCounts.Sort();
            dataGridViewTank.Rows.Clear();

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

            AddRows(dataGridViewTank, nHistoryCount);

            int nTotalHistoryCount = GetTotalHistoryCount(historyCounts);

            for (int i = nHistoryCount - 1, j = 0; i >= 0; i--, j++)
            {
                TankHistoryCount history = historyCounts[i];
                DataGridViewRow row = dataGridViewTank.Rows[j];

                row.Cells[(int)PSMTankGrid.NO_INDEX].Value = row.Index + 1;
                row.Cells[(int)PSMTankGrid.TANK_NAME_INDEX].Value = history.Tank.Name;
                row.Cells[(int)PSMTankGrid.MATERIAL_INDEX].Value = history.Tank.Material == null ? "-" : history.Tank.Material.Name;
                row.Cells[(int)PSMTankGrid.BUILDING_INDEX].Value = GetBuildingName(history);
                row.Cells[(int)PSMTankGrid.LOCATION_INDEX].Value = GetLocationName(history);
                row.Cells[(int)PSMTankGrid.HISTORY_COUNT_INDEX].Value = history.HistoryCount;
                row.Cells[(int)PSMTankGrid.PERCENT_INDEX].Value = ParetoPage.GetHistoryPercent(nTotalHistoryCount, history.HistoryCount);

                row.Tag = history;

                values[j] = history.HistoryCount;
                labels[j] = history.Tank.Name;
            }

            if (nHistoryCount > 0)
                dataGridViewTank.Rows[0].Cells[0].Selected = true;
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

                row.Cells[(int)PSMEquipZoneGrid.NO_INDEX].Value = row.Index + 1;
                row.Cells[(int)PSMEquipZoneGrid.LOCATION_INDEX].Value = GetLocationName(history);
                row.Cells[(int)PSMEquipZoneGrid.BUILDING_INDEX].Value = GetBuildingName(history);
                row.Cells[(int)PSMEquipZoneGrid.HISTORY_COUNT_INDEX].Value = history.HistoryCount;
                row.Cells[(int)PSMEquipZoneGrid.PERCENT_INDEX].Value = ParetoPage.GetHistoryPercent(nTotalHistoryCount, history.HistoryCount);

                row.Tag = history;

                values[j] = history.HistoryCount;
                labels[j] = row.Cells[(int)PSMEquipZoneGrid.LOCATION_INDEX].Value.ToString();
            }

            if (nHistoryCount > 0)
                dataGridViewEquipZone.Rows[0].Cells[0].Selected = true;
        }

        private void RefreshMaterialGrid(out double[] values, out string[] labels)
        {
            List<MaterialHistoryCount> historyCounts = new List<MaterialHistoryCount>();

            foreach (KeyValuePair<UnE.PSM.PSMMaterial, int> pair in m_dicMaterialHistories)
            {
                MaterialHistoryCount history = new MaterialHistoryCount();
                history.Material = pair.Key;
                history.HistoryCount = pair.Value;

                historyCounts.Add(history);
            }

            historyCounts.Sort();
            dataGridViewMaterial.Rows.Clear();

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

            AddRows(dataGridViewMaterial, nHistoryCount);

            int nTotalHistoryCount = GetTotalHistoryCount(historyCounts);

            for (int i = nHistoryCount - 1, j = 0; i >= 0; i--, j++)
            {
                MaterialHistoryCount history = historyCounts[i];
                DataGridViewRow row = dataGridViewMaterial.Rows[j];

                row.Cells[(int)PSMMaterialGrid.NO_INDEX].Value = row.Index + 1;
                row.Cells[(int)PSMMaterialGrid.MATERIAL_INDEX].Value = history.Material.Name;
                row.Cells[(int)PSMMaterialGrid.HISTORY_COUNT_INDEX].Value = history.HistoryCount;
                row.Cells[(int)PSMMaterialGrid.PERCENT_INDEX].Value = ParetoPage.GetHistoryPercent(nTotalHistoryCount, history.HistoryCount);

                row.Tag = history;

                values[j] = history.HistoryCount;
                labels[j] = row.Cells[(int)PSMMaterialGrid.MATERIAL_INDEX].Value.ToString();
            }

            if (nHistoryCount > 0)
                dataGridViewMaterial.Rows[0].Cells[0].Selected = true;
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

        private string GetBuildingName(SensorTagHistoryCount history)
        {
            if (history.Sensor.EquipmentZone == null)
                return "-";

            if (history.Sensor.EquipmentZone.Building == null)
                return "-";

            return history.Sensor.EquipmentZone.Building.BuildingName;
        }

        private string GetBuildingName(TankHistoryCount history)
        {
            if (history.Tank.EquipZone == null)
                return "-";

            if (history.Tank.EquipZone.Building == null)
                return "-";

            return history.Tank.EquipZone.Building.BuildingName;
        }

        private string GetBuildingName(EquipZoneHistoryCount history)
        {
            if (history.EquipmentZone == null)
                return "-";

            if (history.EquipmentZone.Building == null)
                return "-";

            return history.EquipmentZone.Building.BuildingName;
        }

        private string GetLocationName(SensorTagHistoryCount history)
        {
            if (history.Sensor.EquipmentZone == null)
                return "-";

            return history.Sensor.EquipmentZone.ZoneName;
        }

        private string GetLocationName(TankHistoryCount history)
        {
            if (history.Tank.EquipZone == null)
                return "-";

            return history.Tank.EquipZone.ZoneName;
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
                m_pageMaterial.CurrentPage = 1;
                m_pageTank.CurrentPage = 1;

                SetNavigatorEnable(m_pageSensor);
                SetNavigatorEnable(m_pageEquipZone);
                SetNavigatorEnable(m_pageMaterial);
                SetNavigatorEnable(m_pageTank);

                m_pageSensor.TotalPageCountLabel.Text = m_pageSensor.CurrentPage + " / " + m_pageSensor.TotalPage.ToString();
                m_pageSensor.TotalPageCountLabel.Location = new Point(panel1.Location.X + panel1.Width - m_pageSensor.TotalPageCountLabel.Width, panel1.Location.Y + panel1.Height + 5);
                m_pageEquipZone.TotalPageCountLabel.Text = m_pageEquipZone.CurrentPage + " / " + m_pageEquipZone.TotalPage.ToString();
                m_pageEquipZone.TotalPageCountLabel.Location = new Point(panel1.Location.X + panel1.Width - m_pageEquipZone.TotalPageCountLabel.Width, panel1.Location.Y + panel1.Height + 5);
                m_pageMaterial.TotalPageCountLabel.Text = m_pageMaterial.CurrentPage + " / " + m_pageMaterial.TotalPage.ToString();
                m_pageMaterial.TotalPageCountLabel.Location = new Point(panel1.Location.X + panel1.Width - m_pageMaterial.TotalPageCountLabel.Width, panel1.Location.Y + panel1.Height + 5);
                m_pageTank.TotalPageCountLabel.Text = m_pageTank.CurrentPage + " / " + m_pageTank.TotalPage.ToString();
                m_pageTank.TotalPageCountLabel.Location = new Point(panel1.Location.X + panel1.Width - m_pageTank.TotalPageCountLabel.Width, panel1.Location.Y + panel1.Height + 5);

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

        private void MoveGrid(DataGridView grid, int y)
        {
            grid.Location = new Point(grid.Location.X, y);
            grid.Size = new Size(grid.Size.Width, this.Size.Height - grid.Location.Y - m_nGridInitSpaceY);
        }

        // 글자가 겹치는 부분이 있는지 검사하여, 겹치는 구간이 있으면 무조건 Text Angle을 20도로 준다.
        private double GetLabelAngle(WinChartViewer chartViewer, DataGridView grid, Font font, string[] labels, int nChartWidth, ref int nChartHeight, ref int nPlotX, ref int nPlotY, ref int nPlotWidth, ref int nPlotHeight)
        {
            int nLabelCount = labels.Count();

            if (nLabelCount <= 1)
            {
                if (grid.Location.Y != m_nGridInitY)
                    MoveGrid(grid, m_nGridInitY);
                    //grid.Location = new Point(grid.Location.X, m_nGridInitY);

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

            for (int i=1;i<nLabelCount;i++)
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

                for (int i=0;i<nLabelCount;i++)
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
                //grid.Location = new Point(grid.Location.X, chartViewer.Location.Y + nChartHeight);
            else if (grid.Location.Y != m_nGridInitY)
                MoveGrid(grid, m_nGridInitY);
                //grid.Location = new Point(grid.Location.X, m_nGridInitY);

            return dAngle;
        }

        // nViewCount : Graph에 최대 몇 개까지의 데이터를 표시할 것인가?
        private void RefreshSensorChart(double[] values, string[] labels, int nViewCount)
        {
            RefreshChart(values, labels, nViewCount, winChartViewerSensor, dataGridViewSensor, SensorChartTag, m_pageSensor);
        }

        // nViewCount : Graph에 최대 몇 개까지의 데이터를 표시할 것인가?
        private void RefreshTankChart(double[] values, string[] labels, int nViewCount)
        {
            RefreshChart(values, labels, nViewCount, winChartViewerTank, dataGridViewTank, TankChartTag, m_pageTank);
        }

        // nViewCount : Graph에 최대 몇 개까지의 데이터를 표시할 것인가?
        private void RefreshEquipZoneChart(double[] values, string[] labels, int nViewCount)
        {
            RefreshChart(values, labels, nViewCount, winChartViewerEquipZone, dataGridViewEquipZone, EquipZoneChartTag, m_pageEquipZone);
        }

        // nViewCount : Graph에 최대 몇 개까지의 데이터를 표시할 것인가?
        private void RefreshMaterialChart(double[] values, string[] labels, int nViewCount)
        {
            RefreshChart(values, labels, nViewCount, winChartViewerMaterial, dataGridViewMaterial, MaterialChartTag, m_pageMaterial);
        }

        // nViewCount : Graph에 최대 몇 개까지의 데이터를 표시할 것인가?
        private void RefreshChart(double[] values, string[] labels, int nViewCount, WinChartViewer chartViewer, DataGridView grid, string strChartTag, PageData pageData)
        {
            if (values == null || labels == null)
            {
                EmptyChart(chartViewer, grid, strChartTag);
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
                //grid.Location = new Point(grid.Location.X, m_nGridInitY);

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

            PageData pageData = null;

            if (sender == m_pageSensor.PageIndexComboBox)
                pageData = m_pageSensor;
            else if (sender == m_pageTank.PageIndexComboBox)
                pageData = m_pageTank;
            else if (sender == m_pageEquipZone.PageIndexComboBox)
                pageData = m_pageEquipZone;
            else if (sender == m_pageMaterial.PageIndexComboBox)
                pageData = m_pageMaterial;
            else
                return;

            SetNavigatorEnable(pageData);

            if (dataGridViewSensor.Rows.Count == 0)
            {
                EmptyChart(winChartViewerSensor, dataGridViewSensor, SensorChartTag);
                EmptyChart(winChartViewerTank, dataGridViewTank, SensorChartTag);
                EmptyChart(winChartViewerEquipZone, dataGridViewEquipZone, EquipZoneChartTag);
                EmptyChart(winChartViewerMaterial, dataGridViewMaterial, SensorChartTag);
            }
            else
            {
                double[] values;
                string[] labels;

                GetSensorGridValues(out values, out labels);
                RefreshSensorChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());

                GetTankGridValues(out values, out labels);
                RefreshTankChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());

                GetEquipZoneGridValues(out values, out labels);
                RefreshEquipZoneChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());

                GetMaterialGridValues(out values, out labels);
                RefreshMaterialChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());
            }
        }

        private void btnPreviousIndex_Click(object sender, EventArgs e)
        {
            PageData pageData = null;

            if (sender == m_pageSensor.PageIndexComboBox)
                pageData = m_pageSensor;
            else if (sender == m_pageTank.PageIndexComboBox)
                pageData = m_pageTank;
            else if (sender == m_pageEquipZone.PageIndexComboBox)
                pageData = m_pageEquipZone;
            else if (sender == m_pageMaterial.PageIndexComboBox)
                pageData = m_pageMaterial;
            else
                return;

            if (pageData.CurrentPage == 1)
                return;

            pageData.CurrentPage--;

            pageData.PageIndexComboBox.SelectedIndex = pageData.CurrentPage - 1;
        }

        private void btnNextIndex_Click(object sender, EventArgs e)
        {
            PageData pageData = null;

            if (sender == m_pageSensor.PageIndexComboBox)
                pageData = m_pageSensor;
            else if (sender == m_pageTank.PageIndexComboBox)
                pageData = m_pageTank;
            else if (sender == m_pageEquipZone.PageIndexComboBox)
                pageData = m_pageEquipZone;
            else if (sender == m_pageMaterial.PageIndexComboBox)
                pageData = m_pageMaterial;
            else
                return;

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

        private void ParetoPSMPage_Load(object sender, EventArgs e)
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
            m_detectPSMMgr.ZoneSubmit(arrSelectedZone, startDate, EndDate);
            
            //찾은 검색결과를 DataGrid로 출력
            Load_DataGrid(arrSelectedZone, nSplitUnitOfMeasure, nSplitUnitOfMeasureDetail, nViewCount);
            //Load_DataGrid(arrSelectedZone, FormMain.Instance.GetReportChartMaxItemCount());
        }

        private void ParetoPSMPage_Resize(object sender, EventArgs e)
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

            GetTankGridValues(out values, out labels);
            RefreshTankChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());

            GetEquipZoneGridValues(out values, out labels);
            RefreshEquipZoneChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());

            GetMaterialGridValues(out values, out labels);
            RefreshMaterialChart(values, labels, FormMain.Instance.GetReportChartMaxItemCount());
        }

        private void cboChart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_systemCall)
                return;

            m_pageSensor.Hide();
            m_pageTank.Hide();
            m_pageEquipZone.Hide();
            m_pageMaterial.Hide();

            if (cboChart.SelectedIndex == (int)ChartType.SENSOR)
            {
                m_pageSensor.Show();

                winChartViewerSensor.Visible = dataGridViewSensor.Visible = true;
                winChartViewerTank.Visible = winChartViewerEquipZone.Visible = winChartViewerMaterial.Visible = false;
                dataGridViewTank.Visible = dataGridViewEquipZone.Visible = dataGridViewMaterial.Visible = false;
                lblDescription.Text = "작동 빈도가 높은 센서들부터 왼쪽에서 오른쪽 방향으로 표시합니다.";
            }
            else if (cboChart.SelectedIndex == (int)ChartType.TANK)
            {
                m_pageTank.Show();

                winChartViewerTank.Visible = dataGridViewTank.Visible = true;
                winChartViewerSensor.Visible = winChartViewerEquipZone.Visible = winChartViewerMaterial.Visible = false;
                dataGridViewSensor.Visible = dataGridViewEquipZone.Visible = dataGridViewMaterial.Visible = false;
                lblDescription.Text = "작동 빈도가 높은 탱크들부터 왼쪽에서 오른쪽 방향으로 표시합니다.";
            }
            else if (cboChart.SelectedIndex == (int)ChartType.EQUIPZONE)
            {
                m_pageEquipZone.Show();

                winChartViewerEquipZone.Visible = dataGridViewEquipZone.Visible = true;
                winChartViewerSensor.Visible = winChartViewerTank.Visible = winChartViewerMaterial.Visible = false;
                dataGridViewSensor.Visible = dataGridViewTank.Visible = dataGridViewMaterial.Visible = false;
                lblDescription.Text = "작동 빈도가 높은 장소들부터 왼쪽에서 오른쪽 방향으로 표시합니다.";
            }
            else if (cboChart.SelectedIndex == (int)ChartType.MATERIAL)
            {
                m_pageMaterial.Show();

                winChartViewerMaterial.Visible = dataGridViewMaterial.Visible = true;
                winChartViewerSensor.Visible = winChartViewerTank.Visible = winChartViewerEquipZone.Visible = false;
                dataGridViewSensor.Visible = dataGridViewTank.Visible = dataGridViewEquipZone.Visible = false;
                lblDescription.Text = "작동 빈도가 높은 물질들부터 왼쪽에서 오른쪽 방향으로 표시합니다.";
            }
        }

        private void btnSaveHWP_Click(object sender, EventArgs e)
        {
            PageBackstageHome.Instance.FrmReport.SaveHWPForPareto(this);
        }

        //이미지 캡쳐
        public void ControllCapture()
        {
            CaptureImage(winChartViewerSensor, HWP_SENSOR_TAG);
            CaptureImage(winChartViewerEquipZone, HWP_EQUIPZONE_TAG);
            CaptureImage(winChartViewerTank, HWP_TANK_TAG);
            CaptureImage(winChartViewerMaterial, HWP_MATERIAL_TAG);
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
            SaveHwpCrtl(dataGridViewTank, lines, HWP_TANK_TAG);
            SaveHwpCrtl(dataGridViewEquipZone, lines, HWP_EQUIPZONE_TAG);
            SaveHwpCrtl(dataGridViewMaterial, lines, HWP_MATERIAL_TAG);

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

            //int nExceptColumnIndex = -1;

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = grid.Rows[i];

                for (int j = 0; j < nColumnCount; j++)
                {
                    //if (j == nExceptColumnIndex)
                    //    continue;

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

        public string GetHWPFileName()
        {
            return "누출_탐지분석_보고서";
        }

        public SDMS.Data.ReportMode GetReportMode()
        {
            return Data.ReportMode.DetectPSMAnalyze;
        }

        public void SetVisibleHWPExport(bool visible)
        {
            btnSaveHWP.Visible = visible;
        }
    }
}
