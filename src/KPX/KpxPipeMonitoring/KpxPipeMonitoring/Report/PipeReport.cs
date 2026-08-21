using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Diagnostics; 

namespace KpxPipeMonitoring.Report
{
    public partial class PipeReport : Form
    {  
        private int nTabIndex
        {
            get
            {
                if (tabControl1.SelectedTab == tabPage_1) return 1;
                else if (tabControl1.SelectedTab == tabPage_2) return 2;
                else if (tabControl1.SelectedTab == tabPage_3) return 3;
                else if (tabControl1.SelectedTab == tabPage_4) return 4;
                else if (tabControl1.SelectedTab == tabPage_5) return 5;
                else if (tabControl1.SelectedTab == tabPage_6) return 6;
                else if (tabControl1.SelectedTab == tabPage_7) return 7;
                else if (tabControl1.SelectedTab == tabPage_8) return 8;
                else if (tabControl1.SelectedTab == tabPage_9) return 9;
                else if (tabControl1.SelectedTab == tabPage_10) return 10;
                else if (tabControl1.SelectedTab == tabPage_11) return 11;
                else if (tabControl1.SelectedTab == tabPage_alarmHistory) return 12;
                else if (tabControl1.SelectedTab == tabPage_workHistory) return 13;
                else return 0;

            }
        }
        private DataGridView curGridView
        {
            get
            {
                if (tabControl1.SelectedTab == tabPage_1) return dataGridView_1;
                else if (tabControl1.SelectedTab == tabPage_2) return dataGridView_2;
                else if (tabControl1.SelectedTab == tabPage_3) return dataGridView_3;
                else if (tabControl1.SelectedTab == tabPage_4) return dataGridView_4;
                else if (tabControl1.SelectedTab == tabPage_5) return dataGridView_5;
                else if (tabControl1.SelectedTab == tabPage_6) return dataGridView_6;
                else if (tabControl1.SelectedTab == tabPage_7) return dataGridView_7;
                else if (tabControl1.SelectedTab == tabPage_8) return dataGridView_8;
                else if (tabControl1.SelectedTab == tabPage_9) return dataGridView_9;
                else if (tabControl1.SelectedTab == tabPage_10) return dataGridView_10;
                else if (tabControl1.SelectedTab == tabPage_11) return dataGridView_11;
                else if (tabControl1.SelectedTab == tabPage_alarmHistory) return dataGridView_alarmHistory;
                else if (tabControl1.SelectedTab == tabPage_total) return dataGridView_total;
                else if (tabControl1.SelectedTab == tabPage_workHistory) return dataGridView_workHistory;
                return null;
            }
        }
        private Chart curChart
        {
            get
            {
                if (tabControl1.SelectedTab == tabPage_1) return chart1;
                else if (tabControl1.SelectedTab == tabPage_2) return chart2;
                else if (tabControl1.SelectedTab == tabPage_3) return chart3;
                else if (tabControl1.SelectedTab == tabPage_4) return chart4;
                else if (tabControl1.SelectedTab == tabPage_5) return chart5;
                else if (tabControl1.SelectedTab == tabPage_6) return chart6;
                else if (tabControl1.SelectedTab == tabPage_7) return chart7;
                else if (tabControl1.SelectedTab == tabPage_8) return chart8;
                else if (tabControl1.SelectedTab == tabPage_9) return chart9;
                else if (tabControl1.SelectedTab == tabPage_10) return chart10;
                else if (tabControl1.SelectedTab == tabPage_11) return chart11;
                return null;
            }
        }
        private Chart curChartFlow
        {
            get
            {
                if (tabControl1.SelectedTab == tabPage_1) return chart1Flow;
                else if (tabControl1.SelectedTab == tabPage_2) return chart2Flow;
                else if (tabControl1.SelectedTab == tabPage_3) return chart3Flow;
                else if (tabControl1.SelectedTab == tabPage_4) return chart4Flow;
                else if (tabControl1.SelectedTab == tabPage_5) return chart5Flow;
                else if (tabControl1.SelectedTab == tabPage_6) return chart6Flow;
                else if (tabControl1.SelectedTab == tabPage_7) return chart7Flow;
                else if (tabControl1.SelectedTab == tabPage_8) return chart8Flow;
                else if (tabControl1.SelectedTab == tabPage_9) return chart9Flow;
                else if (tabControl1.SelectedTab == tabPage_10) return chart10Flow;
                else if (tabControl1.SelectedTab == tabPage_11) return chart11Flow;
                return null;
            }
        }

        private string date1 { get { return dateTimePicker_date1.Value.ToString("yyyyMMdd"); } }
        private string date2 { get { return dateTimePicker_date2.Value.ToString("yyyyMMdd"); } }
        private string time1 { get { return dateTimePicker_time1.Value.ToString("HHmmss"); } }
        private string time2 { get { return dateTimePicker_time2.Value.ToString("HHmmss"); } }
         
        //DateTime searchBeforeDate;
        //DateTime searchAfterDate;

        //MySqlConnection conn = null;

        private HistoryManager m_historyMgr = null;

        Timer timer = null;
        bool panelDown = true;

        #region 초기화
        public PipeReport()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
             
            m_historyMgr = new HistoryManager(MainForm.Instance);

            comboBox_chartPipeList.SelectedIndexChanged += comboBox_chartPipeList_SelectedIndexChanged;
            comboBox_chartPipeList.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_chartPipeList.DisplayMember = "strTankName";
            comboBox_chartPipeList.ValueMember = "nTankID";
             
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;

            foreach (TabPage tab in tabControl1.TabPages)
                tab.Text = "";

            tempReportClickBtn = panel_total;
            tempReportClickBtn.Tag = label_total.Text;
            panel_total.BackgroundImage = global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click;

            SettingPanelImage(panel_1, label_1, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_2, label_2, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_3, label_3, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_4, label_4, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_5, label_5, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_6, label_6, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_7, label_7, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_8, label_8, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_9, label_9, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_10, label_10, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_11, label_11, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_total, label_total, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_alarmHistory, label_alarmHistory, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_workHistory, label_workHistory, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_printReport, label_pringReport, global::KpxPipeMonitoring.Properties.Resources.ReportPrintButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportPrintButton_Click);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_search, global::KpxPipeMonitoring.Properties.Resources.Search_Normal, global::KpxPipeMonitoring.Properties.Resources.Search_Click);

            //pictureBox_search.Visible = false;
            this.dateTimePicker_date1.Value = DateTime.Now.AddDays(-7);
            this.dateTimePicker_time1.Value = DateTime.Now.AddDays(-7);
            this.dateTimePicker_time1.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker_time1.CustomFormat = "HH:mm:ss";

            this.dateTimePicker_time2.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker_time2.CustomFormat = "HH:mm:ss";

            dateTimePicker_date1.Enabled = false;
            dateTimePicker_date2.Enabled = false;
            dateTimePicker_time1.Enabled = false;
            dateTimePicker_time2.Enabled = false;

            pictureBox_doubleLeft.Visible = false;
            pictureBox_doubleRight.Visible = false;
            pictureBox_left.Visible = false;
            pictureBox_right.Visible = false;
            textBox1.Visible = false;
            label_maxPage.Visible = false;
            label_searchDate.Visible = false;
            panel_move.Visible = false;
            comboBox_chartPipeList.Visible = false;
             
            InitGridView();
            InitChart();
            DisplayPipeList();
            comboBox_pipeList.SelectedIndexChanged += comboBox_pipeList_SelectedIndexChanged;
            comboBox_pipeList.Visible = false;
            comboBox_pipeList.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_pipeList.SelectedValue = 4;
            DisplayTotal();

            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;

            MainForm.Instance.SetDoubleBuffer(dataGridView_1, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_2, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_3, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_4, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_5, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_6, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_7, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_8, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_9, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_10, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_11, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_total, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_alarmHistory, true);
            MainForm.Instance.SetDoubleBuffer(dataGridView_workHistory, true);

	        if(MainForm.bExcelInstalled) 
	            panel_printReport.Visible = true; 
	        else 
	            panel_printReport.Visible = false;

            panel_btns.MouseEnter += panel_btns_MouseEnter;
            panel_btns.MouseLeave += panel_btns_MouseLeave;

            timer = new Timer();
            timer.Interval = 10;
            timer.Tick += timer_Tick;
        }

        void panel_btns_MouseEnter(object sender, EventArgs e)
        {
            timer.Enabled = true;
            panelDown = true;
        }
        void panel_btns_MouseLeave(object sender, EventArgs e)
        {
            timer.Enabled = true;
            panelDown = false;
        }

        void comboBox_pipeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommonFunction.PipeInfo selectedItem = (CommonFunction.PipeInfo)comboBox_pipeList.SelectedItem;
            int nPipeId = Convert.ToInt32(selectedItem.nPipeID);

            if (nPipeId < 0) return;

            if (tabControl1.SelectedTab == tabPage_alarmHistory)
                DisplayAlarmHistory(nPipeId);
            else if (tabControl1.SelectedTab == tabPage_workHistory)
                DisplayWorkHistory(nPipeId);
        }
         
        private void InitGridView()
        {
            Color colHeaderBackground = Color.FromArgb(87, 168, 250);
            //dataGridView_total.Columns.Add("PipeId", "배관 ID");
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_total, "PipeName", "배관명", colHeaderBackground, 220);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_total, "TankName", "연결된 탱크", colHeaderBackground, 190);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_total, "AvgPressure", "평균 압력\r\n(kg/cm²)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_total, "MaxPressure", "최고 압력\r\n(kg/cm²)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_total, "MinPressure", "최소 압력\r\n(kg/cm²)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_total, "AvgFlow", "평균 유량\r\n(kl/h)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_total, "MaxFlow", "최고 유량\r\n(kl/h)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_total, "MinFlow", "최소 유량\r\n(kl/h)", colHeaderBackground, 140);
            //commonFunction.SettingGridView(dataGridView_total, "RecentHisotry", "최근 이력", 150);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_total, "RecentWorkTime", "직전 작업 시간", colHeaderBackground, 500);
            //commonFunction.SettingGridView(dataGridView_total, "SumWorkTime", "누적 작업 시간", colHeaderBackground, 150);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_total, "Status", "상태", colHeaderBackground, 100);
            //dataGridView_total.Columns["SumWorkTime"].Visible = false;
            dataGridView_total.ColumnHeadersHeight = 55;


            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "Num", "No", colHeaderBackground, 40);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "PipeId", "배관ID", colHeaderBackground);
            dataGridView_alarmHistory.Columns["PipeId"].Visible = false;
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "HistoryID", "이력ID", colHeaderBackground);
            dataGridView_alarmHistory.Columns["HistoryID"].Visible = false;
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "PipeName", "배관명", colHeaderBackground, 150);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "TankName", "연결된 탱크", colHeaderBackground, 120);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "BeginTime", "발생 시간", colHeaderBackground, 230);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "EndTime", "종료 시간", colHeaderBackground, 230);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "AlarmTime", "지속 시간", colHeaderBackground, 130);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "StandardPressure", "발생시 기준치", colHeaderBackground, 160);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "AlarmPressure", "발생시 데이터", colHeaderBackground, 160);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "Status", "발생시 상태", colHeaderBackground, 150);
            //commonFunction.SettingGridView(dataGridView_alarmHistory, "NormalRange", "정상압력", colHeaderBackground);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "Terminator", "종료 계정", colHeaderBackground, 130);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "AlarmOccurrence", "발생 유형", colHeaderBackground, 200);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_alarmHistory, "AlarmComment", "해결 내용", colHeaderBackground, 170);

            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "Num", "No", colHeaderBackground, 26);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "PipeId", "배관ID", colHeaderBackground);
            dataGridView_workHistory.Columns["PipeId"].Visible = false;
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "HistoryID", "이력ID", colHeaderBackground);
            dataGridView_workHistory.Columns["HistoryID"].Visible = false;
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "PipeName", "배관명", colHeaderBackground, 220);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "TankName", "연결된 탱크", colHeaderBackground, 160);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "BeginTime", "시작 시간", colHeaderBackground, 280);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "EndTime", "종료 시간", colHeaderBackground, 280);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "CTime", "지속 시간", colHeaderBackground, 180);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "IgnoreCTime", "알람 무시 시간", colHeaderBackground, 250);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "BeginUserName", "시작 계정", colHeaderBackground, 200);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_workHistory, "EndUserId", "종료 계정", colHeaderBackground, 200); 
            
            for (int i = 1; i <= 11; i++)
            {
                Control[] ctrl = this.Controls.Find("dataGridView_" + i, true);
                if (ctrl.Length == 0) continue;
                DataGridView gridView = (DataGridView)ctrl[0];

                MainForm.Instance.commonFunction.SettingGridView(gridView, "Num", "No", colHeaderBackground, 50);
                MainForm.Instance.commonFunction.SettingGridView(gridView, "TankName", "연결된 탱크", colHeaderBackground, 120);
                MainForm.Instance.commonFunction.SettingGridView(gridView, "ID", "알람이력ID", colHeaderBackground);
                gridView.Columns["ID"].Visible = false;
                MainForm.Instance.commonFunction.SettingGridView(gridView, "PipeID", "배관ID", colHeaderBackground);
                gridView.Columns["PipeID"].Visible = false;
                MainForm.Instance.commonFunction.SettingGridView(gridView, "SubBeginTime", "시작시간", colHeaderBackground);
                gridView.Columns["SubBeginTime"].Visible = false;
                MainForm.Instance.commonFunction.SettingGridView(gridView, "BeginTime", "시작시간", colHeaderBackground, 245);
                MainForm.Instance.commonFunction.SettingGridView(gridView, "EndTime", "종료시간", colHeaderBackground, 245);
                MainForm.Instance.commonFunction.SettingGridView(gridView, "Status", "상태", colHeaderBackground, 100);
                MainForm.Instance.commonFunction.SettingGridView(gridView, "Type", "타입", colHeaderBackground);
                gridView.Columns["Type"].Visible = false;
                gridView.Columns["BeginTime"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                gridView.MouseDoubleClick += gridView_MouseDoubleClick;
                gridView.CellPainting += gridView_CellPainting;

            } 
        }
         
        #region 차트 세팅
        private void InitChart()
        {
            for (int i = 1; i <= 11; i++)
            {
                Control[] ctrl = this.Controls.Find("chart" + i, true);
                if (ctrl.Length == 0) continue;
                Chart chart = (Chart)ctrl[0];

                chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                chart.ChartAreas[0].AxisY.Interval = 0; 
                chart.ChartAreas[0].AxisX.IsMarginVisible = false;
                chart.ChartAreas[0].AxisY.LabelStyle.Format = "F1"; 
                chart.Series.Clear();
                Series series = chart.Series.Add("series1"); 
                series.ChartType = SeriesChartType.Line;
                chart.Legends.Clear();

                chart.Series[0].IsXValueIndexed = true;
                chart.MouseDown += chart_MouseDown;
                chart.MouseMove += chart_MouseMove;
                chart.MouseUp += chart_MouseUp;
                chart.MouseLeave += chart_MouseLeave;
                chart.MouseWheel += chart_MouseWheel;

                //차트 위치
                chart.ChartAreas[0].Position.Auto = false;
                chart.ChartAreas[0].Position.X = 0;
                chart.ChartAreas[0].Position.Y = 20;
                chart.ChartAreas[0].Position.Width = 96;
                chart.ChartAreas[0].Position.Height = 90;

                chart.ChartAreas[0].AxisX.ScrollBar.LineColor = Color.White;
                chart.ChartAreas[0].AxisX.ScrollBar.ButtonColor = Color.White;
                chart.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

                chart.ChartAreas[0].AxisY.ScrollBar.LineColor = Color.White;
                chart.ChartAreas[0].AxisY.ScrollBar.ButtonColor = Color.White;
                chart.ChartAreas[0].AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll; 
            }
            for (int i = 1; i <= 11; i++)
            {
                Control[] ctrl = this.Controls.Find("chart" + i + "Flow", true);
                if (ctrl.Length == 0) continue;
                Chart chartFlow = (Chart)ctrl[0];

                chartFlow.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                chartFlow.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                chartFlow.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                chartFlow.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
                chartFlow.ChartAreas[0].AxisY.Interval = 0;
                chartFlow.ChartAreas[0].AxisX.IsMarginVisible = false;
                chartFlow.ChartAreas[0].AxisY.LabelStyle.Format = "F1"; 

                chartFlow.Series.Clear();
                Series series = chartFlow.Series.Add("series1");
                series.ChartType = SeriesChartType.Line;
                chartFlow.Legends.Clear();

                chartFlow.Series[0].IsXValueIndexed = true;
                chartFlow.MouseDown += chartFlow_MouseDown;
                chartFlow.MouseMove += chartFlow_MouseMove;
                chartFlow.MouseUp += chartFlow_MouseUp;
                chartFlow.MouseLeave += chartFlow_MouseLeave;
                chartFlow.MouseWheel += chartFlow_MouseWheel;

                chartFlow.ChartAreas[0].Position.Auto = false;
                chartFlow.ChartAreas[0].Position.X = 0;
                chartFlow.ChartAreas[0].Position.Y = 20;
                chartFlow.ChartAreas[0].Position.Width = 96;
                chartFlow.ChartAreas[0].Position.Height = 90;

                chartFlow.ChartAreas[0].AxisX.ScrollBar.LineColor = Color.White;
                chartFlow.ChartAreas[0].AxisX.ScrollBar.ButtonColor = Color.White;
                chartFlow.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

                chartFlow.ChartAreas[0].AxisY.ScrollBar.LineColor = Color.White;
                chartFlow.ChartAreas[0].AxisY.ScrollBar.ButtonColor = Color.White;
                chartFlow.ChartAreas[0].AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll; 
            }          
        } 
         
        private void InitSeries(DateTime beforeDate, DateTime afterDate)
        {
            curChart.Series.Clear();
            Series series = curChart.Series.Add("series1");
            series.ChartType = SeriesChartType.Line;
            curChart.Series[0].IsXValueIndexed = true;
            curChart.Series[0].XValueMember = "dtTimeStamp";
            curChart.Series[0].YValueMembers = "dPressure";
            curChart.Series[0].BorderWidth = 3;
            curChart.Series[0].Color = Color.FromArgb(194, 198, 191);
             
            curChartFlow.Series.Clear();
            series = curChartFlow.Series.Add("series2");
            series.ChartType = SeriesChartType.Line;
            curChartFlow.Series[0].IsXValueIndexed = true;
            curChartFlow.Series[0].XValueMember = "dtTimeStamp";
            curChartFlow.Series[0].YValueMembers = "dFlow"; 
            curChartFlow.Series[0].BorderWidth = 3;
            curChartFlow.Series[0].Color = Color.Transparent;
             
            string dateFormat = "HH:mm";

            DateTimeIntervalType IntervalType = MainForm.Instance.commonFunction.GetIntervalType(beforeDate, afterDate);
            //curChart.ChartAreas[0].AxisX.IntervalType = IntervalType;
            if (IntervalType == DateTimeIntervalType.Seconds)
                dateFormat = "HH:mm:ss";
            else if (IntervalType == DateTimeIntervalType.Minutes)
                dateFormat = "HH:mm:ss";
            else if (IntervalType == DateTimeIntervalType.Hours)
                dateFormat = "MM/dd\r\nHH:mm";
            else
                dateFormat = "MM/dd\r\nHH:mm";

            curChart.Series[0].XValueType = ChartValueType.DateTime; 
            //curChart.Series[1].ToolTip = "#VALX{" + dateFormat + "} - #VALY1{0.00}"; 
            curChart.ChartAreas[0].AxisX.LabelStyle.Format = dateFormat;

            curChartFlow.Series[0].XValueType = ChartValueType.DateTime; 
            //curChartFlow.Series[1].ToolTip = "#VALX{" + dateFormat + "} - #VALY1{0.00}";
            curChartFlow.ChartAreas[0].AxisX.LabelStyle.Format = dateFormat;
        }
        #endregion

        #region 버튼 세팅
        void timer_Tick(object sender, EventArgs e)
        {
            if (panelDown)
            {
                if (panel_btns.Height >= 125) timer.Enabled = false;
                else panel_btns.Height += 5;
            }
            else
            {
                if (panel_btns.Height <= 85) timer.Enabled = false;
                else panel_btns.Height -= 5;
            }
        }

        public Panel tempReportClickBtn = null;
        public void SettingPanelImage(Panel btn, Label label, Image normalImg, Image clickImg)
        {
            btn.Cursor = Cursors.Hand;
            label.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) =>
            {
                btn.BackgroundImage = clickImg;
                timer.Enabled = true;
                panelDown = true;
            };
            btn.MouseLeave += (s, e) =>
            {
                if (btn != tempReportClickBtn)
                    btn.BackgroundImage = normalImg;
                timer.Enabled = true;
                panelDown = false;
            };
            label.MouseEnter += (s, e) =>
            {
                btn.BackgroundImage = clickImg;
                timer.Enabled = true;
                panelDown = true;
            };
            label.MouseLeave += (s, e) =>
            {
                if (btn != tempReportClickBtn)
                    btn.BackgroundImage = normalImg;
                timer.Enabled = true;
                panelDown = false;
            };
            btn.MouseClick += (s, e) =>
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
                if (btn.Name == "panel_printReport")
                {
                    ExportExcel();
                }
                else
                {
                    if (tempReportClickBtn != null)
                    {
                        tempReportClickBtn.BackgroundImage = normalImg;
                        tempReportClickBtn = btn;
                        tempReportClickBtn.BackgroundImage = clickImg;
                        tempReportClickBtn.Tag = label.Text;
                    }

                    string btnName = btn.Name.Replace("panel_", "");
                    foreach (System.Windows.Forms.TabPage item in tabControl1.TabPages)
                    {
                        string tabPageName = item.Name.Replace("tabPage_", "");
                        if (tabPageName == btnName)
                        {
                            tabControl1.SelectedTab = item;
                            break;
                        }
                    } 
                }

                if (tempReportClickBtn == null)
                    label_selectItem.Text = "선택 : ";
                else
                    label_selectItem.Text = "선택 : " + tempReportClickBtn.Tag.ToString();
            };
            label.MouseClick += (s, e) =>
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
                if (label.Name == "label_pringReport")
                {
                    ExportExcel();
                }
                else
                {
                    if (tempReportClickBtn != null)
                    {
                        tempReportClickBtn.BackgroundImage = normalImg;
                        tempReportClickBtn = btn;
                        tempReportClickBtn.BackgroundImage = clickImg;
                        tempReportClickBtn.Tag = label.Text;
                    }

                    string labelName = label.Name.Replace("label_", "");
                    foreach (System.Windows.Forms.TabPage item in tabControl1.TabPages)
                    {
                        string tabPageName = item.Name.Replace("tabPage_", "");
                        if (tabPageName == labelName)
                        {
                            tabControl1.SelectedTab = item;
                            break;
                        }
                    }                    
                }

                if (tempReportClickBtn == null)
                    label_selectItem.Text = "선택 : ";
                else
                    label_selectItem.Text = "선택 : " + tempReportClickBtn.Tag.ToString();
            };
        }
        #endregion

        #endregion

        #region Tab 이벤트 
        Chart oldCurChart = null;
        Chart oldCurChartFlow = null;
        int oldnTabIndex = 0;
        void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        { 
            if (oldnTabIndex >= 1 && oldnTabIndex <= 11)
            {
                if (dicTotalChartData.ContainsKey(oldnTabIndex))
                {
                    dicTotalChartData[oldnTabIndex].Clear();
                    dicTotalChartData.Remove(oldnTabIndex);
                    GC.Collect();
                } 
            }

            if (oldCurChart != null && oldCurChart.DataSource != null)
            { 
                List<CommonFunction.ChartField> chartList = (List<CommonFunction.ChartField>)oldCurChart.DataSource;
                if (chartList != null)
                {
                    chartList.Clear();
                    oldCurChart.DataSource = null;
                    GC.Collect();
                }
            }
            if (oldCurChartFlow != null && oldCurChartFlow.DataSource != null)
            {
                List<CommonFunction.ChartField> chartList = (List<CommonFunction.ChartField>)oldCurChartFlow.DataSource;
                if (chartList != null)
                {
                    chartList.Clear();
                    oldCurChartFlow.DataSource = null;
                    GC.Collect();
                }
            }

            if (tabControl1.SelectedTab == tabPage_1 || tabControl1.SelectedTab == tabPage_2 || tabControl1.SelectedTab == tabPage_3 || tabControl1.SelectedTab == tabPage_4
                || tabControl1.SelectedTab == tabPage_5 || tabControl1.SelectedTab == tabPage_6 || tabControl1.SelectedTab == tabPage_7 || tabControl1.SelectedTab == tabPage_8 
                || tabControl1.SelectedTab == tabPage_9 || tabControl1.SelectedTab == tabPage_10 || tabControl1.SelectedTab == tabPage_11)
            {
                tabPage_Resize(null, null);
                DisplayPipeAlarmWorkHistroy();
                if (chartTankID < 0) // comboBox_chartPipeList_SelectedIndexChanged 이벤트로 조회하기때문에 또 조회할 필요 없음
                    DisplayPipe(); 
            } 
            else if (tabControl1.SelectedTab == tabPage_alarmHistory)
            {
                CommonFunction.PipeInfo selectedItem = (CommonFunction.PipeInfo)comboBox_pipeList.SelectedItem;
                int nSelectedPipeId = Convert.ToInt32(selectedItem.nPipeID);

                DisplayAlarmHistory(nSelectedPipeId);
            }
            else if (tabControl1.SelectedTab == tabPage_workHistory)
            {
                CommonFunction.PipeInfo selectedItem = (CommonFunction.PipeInfo)comboBox_pipeList.SelectedItem;
                int nSelectedPipeId = Convert.ToInt32(selectedItem.nPipeID);

                DisplayWorkHistory(nSelectedPipeId);
            } 

            if (tabControl1.SelectedTab == tabPage_total)
            {
                dateTimePicker_date1.Enabled = false;
                dateTimePicker_date2.Enabled = false;
                dateTimePicker_time1.Enabled = false;
                dateTimePicker_time2.Enabled = false;

                pictureBox_doubleLeft.Visible = false;
                pictureBox_doubleRight.Visible = false;
                pictureBox_left.Visible = false;
                pictureBox_right.Visible = false;
                textBox1.Visible = false;
                label_maxPage.Visible = false;
                label_searchDate.Visible = false;
                panel_move.Visible = false;
                comboBox_pipeList.Visible = false;
                comboBox_chartPipeList.Visible = false;
            }
            else if (tabControl1.SelectedTab == tabPage_alarmHistory || tabControl1.SelectedTab == tabPage_workHistory)
            {
                dateTimePicker_date1.Enabled = true;
                dateTimePicker_date2.Enabled = true;
                dateTimePicker_time1.Enabled = true;
                dateTimePicker_time2.Enabled = true;

                pictureBox_doubleLeft.Visible = false;
                pictureBox_doubleRight.Visible = false;
                pictureBox_left.Visible = false;
                pictureBox_right.Visible = false;
                textBox1.Visible = false;
                label_maxPage.Visible = false;
                label_searchDate.Visible = false;
                panel_move.Visible = false;
                comboBox_pipeList.Visible = true;
                comboBox_chartPipeList.Visible = false;
            } 
            else
            {
                oldCurChart = curChart;
                oldCurChartFlow = curChartFlow;

                dateTimePicker_date1.Enabled = true;
                dateTimePicker_date2.Enabled = true;
                dateTimePicker_time1.Enabled = true;
                dateTimePicker_time2.Enabled = true;

                pictureBox_doubleLeft.Visible = true;
                pictureBox_doubleRight.Visible = true;
                pictureBox_left.Visible = true;
                pictureBox_right.Visible = true;
                textBox1.Visible = true;
                label_maxPage.Visible = true;
                label_searchDate.Visible = true;
                panel_move.Visible = true;
                comboBox_pipeList.Visible = false;
                //comboBox_chartPipeList.Visible = true;

                int maxpage = 1;
                int curPage = 1;
                if (dicPageEntity.ContainsKey(nTabIndex))
                {
                    maxpage = dicPageEntity[nTabIndex].nMaxPage;
                    curPage = dicPageEntity[nTabIndex].nCurPage;
                }
                string searchDate = string.Empty;
                if (dicPageChart.ContainsKey(nTabIndex))
                    if (dicPageChart[nTabIndex].ContainsKey(curPage))
                    {
                        searchDate = dicPageChart[nTabIndex][curPage][0].dtTimeStamp.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " +
                             dicPageChart[nTabIndex][curPage][dicPageChart[nTabIndex][curPage].Count - 1].dtTimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                SetPageText(curPage, maxpage, searchDate);
            }
            oldnTabIndex = nTabIndex; 
        }
        private void tabPage_Resize(object sender, EventArgs e)
        {
            TabPage tabPage = sender as TabPage;
            if (tabPage == null) return;

            int height = this.Size.Height - this.tabControl1.Location.Y;
            Chart chartPressure = null;
             
            for (int i = tabPage.Controls.Count - 1; i >= 0; i--)
            {
                if (tabPage.Controls[i] is Chart)
                {
                    Chart chart = tabPage.Controls[i] as Chart;

                    if (chart.Name.Contains("Flow"))
                    {
                        chart.Size = new Size(chart.Size.Width, height / 2);
                        chart.Location = new Point(chart.Location.X, chartPressure.Location.Y + chartPressure.Size.Height);
                    }
                    else
                    {
                        chartPressure = chart;
                        chart.Size = new Size(chart.Size.Width, height / 2);
                    }
                }
            } 
        } 
        #endregion

        #region 차트 이벤트
        Point mDown = Point.Empty; 
        Graphics g;
        Pen pen = new Pen(Brushes.Red);
        void chart_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Location.X <= 72) return;
            if (e.Location.Y >= 345) return;

            mDown = e.Location; 
        } 
        void chart_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (curChart == null) return;

                curChart.Focus();

                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if (mDown.X == 0) return;

                    curChart.Refresh();

                    using (g = curChart.CreateGraphics())
                    {
                        g.DrawRectangle(Pens.Red, GetRectangle(mDown, e.Location));
                    }
                }
                else if (e.Button == System.Windows.Forms.MouseButtons.None)
                {
                    curChart.Refresh();

                    curChart.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);

                    int curPosition = (int)curChart.ChartAreas[0].CursorX.Position;
                    if (curChart.ChartAreas[0].CursorX.Position < 0) return;
                    if (curChart.Series[0].Points.Count < curPosition - 1) return;
                    if (curChart.Series[0].Points.Count <= 1) return;
                    if (curChart.Series[0].Points.Count < curPosition - 1) return;
                    if (curChart.Series[0].Points[curPosition - 1] == null) return;

                    using (g = curChart.CreateGraphics())
                    {
                        string content = DateTime.FromOADate(curChart.Series[0].Points[curPosition - 1].XValue).ToString("yyyy-MM-dd HH:mm:ss") +
                            "\r\n압력:" + String.Format("{0:F2}", curChart.Series[0].Points[curPosition - 1].YValues[0]);

                        g.DrawRectangle(pen, e.X - 5 - 65, curChart.ChartAreas[0].Position.Y - 2, 140, 30);
                        g.DrawString(content, new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)))
                            , Brushes.Red
                            , new PointF(e.X - 65, curChart.ChartAreas[0].Position.Y)
                            );
                    }
                }
            }
            catch (Exception ex)
            {
                MainForm.Instance.SetSystemLog("[ERROR] PipeReport-chart_MouseMove / " + ex.Message);
            }
        }
        void chart_MouseUp(object sender, MouseEventArgs e)
        {
            if (curChart == null) return;
            if (mDown.X == 0) return;

            int minY = 40;
            int maxY = 740;

            //시작점
            List<Point> point1 = new List<Point>();
            for (int i = minY; i <= maxY; i++)
            {
                Point p = new Point(mDown.X, i);
                if (!point1.Contains(p))
                    point1.Add(p);
            }

            //끝점
            List<Point> point2 = new List<Point>();
            for (int i = minY; i <= maxY; i++)
            {
                Point p = new Point(e.Location.X, i);
                if (!point2.Contains(p))
                    point2.Add(p);
            }

            List<int> ints1 = new List<int>();
            foreach (Point item in point1)
            {
                HitTestResult result = curChart.HitTest(item.X, item.Y);
                if (result.PointIndex != -1 && !ints1.Contains(result.PointIndex))
                    ints1.Add(result.PointIndex);
            }

            List<int> ints2 = new List<int>();
            foreach (Point item in point2)
            {
                HitTestResult result = curChart.HitTest(item.X, item.Y);
                if (result.PointIndex != -1 && !ints2.Contains(result.PointIndex))
                    ints2.Add(result.PointIndex);
            }

            double xValue1 = -1;
            foreach (int dd in ints1)
            {
                if (xValue1 < 0 || xValue1 < curChart.Series[0].Points[dd].XValue)
                    xValue1 = curChart.Series[0].Points[dd].XValue;
            }
            double xValue2 = -1;
            foreach (int dd in ints2)
            {
                if (xValue2 < curChart.Series[0].Points[dd].XValue)
                    xValue2 = curChart.Series[0].Points[dd].XValue;
            }

            DateTime beforeDate = DateTime.FromOADate(xValue1);
            DateTime afterDate = DateTime.FromOADate(xValue2);

            DateTime beforeDatePicker = new DateTime(dateTimePicker_date1.Value.Year, dateTimePicker_date1.Value.Month, dateTimePicker_date1.Value.Day,
                                                   dateTimePicker_time1.Value.Hour, dateTimePicker_time1.Value.Minute, dateTimePicker_time1.Value.Second);
            DateTime afterDatePicker = new DateTime(dateTimePicker_date2.Value.Year, dateTimePicker_date2.Value.Month, dateTimePicker_date2.Value.Day,
                                                  dateTimePicker_time2.Value.Hour, dateTimePicker_time2.Value.Minute, dateTimePicker_time2.Value.Second);
            
            curChart.Refresh();

            mDown = Point.Empty; 
            if (xValue1 <= 0 || xValue2 <= 0 || beforeDate < beforeDatePicker || afterDate > afterDatePicker || (afterDate - beforeDate).TotalMinutes <= 1) 
                return; 

            DisplayZoomPipe(beforeDate, afterDate);
        }
        void chart_MouseLeave(object sender, EventArgs e)
        {
            if (curChart == null) return;

            curChart.Refresh();
            curChart.ChartAreas[0].CursorX.Position = 0;
        } 
        static public Rectangle GetRectangle(Point p1, Point p2)
        {
            return new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
        }

        void curChart_Customize(object sender, EventArgs e)
        {
            curChart.Customize -= curChart_Customize;
            System.Diagnostics.Trace.WriteLine("customize begin " + DateTime.Now);
            if (!dicWorkDate.ContainsKey(nTabIndex)) return;
             
            int index = 0;
            List<CommonFunction.WorkListField> workList = dicWorkDate[nTabIndex];
            foreach (CommonFunction.WorkListField item in workList)
            {
                for (int i = index; i < curChart.Series[0].Points.Count; i++)
                {
                    double curWorkTime = DateTime.FromOADate(curChart.Series[0].Points[i].XValue).ToOADate();                    
                    //DateTime dtBeginTime = DateTime.FromOADate(item.dBeginTime);
                    if ((item.dBeginTime <= curWorkTime && item.dEndTime >= curWorkTime) || (item.dBeginTime <= curWorkTime && item.dEndTime == 0))
                    {
                        curChart.Series[0].Points[i].Color = Color.FromArgb(48, 129, 209); // 파랑
                        curChart.Series[0].Points[i].BorderWidth = 4; 
                    }
                }
            } 

            System.Diagnostics.Trace.WriteLine("customize end " + DateTime.Now);
        }
        void chart_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    curChart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    curChart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0)
                {
                    double xMin = curChart.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = curChart.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double yMin = curChart.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    double yMax = curChart.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = (curChart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + xMin) / 2;
                    double posXFinish = (curChart.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + xMax) / 2;
                    double posYStart = (curChart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + yMin) / 2;
                    double posYFinish = (curChart.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + yMax) / 2;

                    curChart.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    curChart.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        #region Flow 차트 이벤트
        Point mDownFlow = Point.Empty;         
        void chartFlow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Location.X <= 72) return;
            if (e.Location.Y >= 345) return;

            mDownFlow = e.Location; 
        }
        void chartFlow_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (curChartFlow == null) return;

                curChartFlow.Focus();

                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if (mDownFlow.X == 0) return;

                    curChartFlow.Refresh();

                    using (g = curChartFlow.CreateGraphics())
                    {
                        g.DrawRectangle(Pens.Red, GetRectangle(mDownFlow, e.Location));
                    }
                }
                else if (e.Button == System.Windows.Forms.MouseButtons.None)
                {
                    curChartFlow.Refresh();

                    curChartFlow.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);

                    int curPosition = (int)curChartFlow.ChartAreas[0].CursorX.Position;
                    if (curChartFlow.ChartAreas[0].CursorX.Position < 0) return;
                    if (curChartFlow.Series[0].Points.Count < curPosition - 1) return;
                    if (curChartFlow.Series[0].Points.Count <= 1) return;
                    if (curChartFlow.Series[0].Points[curPosition - 1] == null) return;

                    using (g = curChartFlow.CreateGraphics())
                    {
                        string content = DateTime.FromOADate(curChartFlow.Series[0].Points[curPosition - 1].XValue).ToString("yyyy-MM-dd HH:mm:ss") +
                            "\r\n유량:" + String.Format("{0:F2}", curChartFlow.Series[0].Points[curPosition - 1].YValues[0]);

                        if (comboBox_chartPipeList.Location.X < e.X - 70 + 140)
                        {
                            g.DrawRectangle(pen, e.X - 145, curChartFlow.ChartAreas[0].Position.Y + 50, 140, 30);
                            g.DrawString(content, new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)))
                                , Brushes.Red
                                , new PointF(e.X - 140, curChartFlow.ChartAreas[0].Position.Y + 53)
                                );
                        }
                        else
                        {
                            g.DrawRectangle(pen, e.X - 70, curChartFlow.ChartAreas[0].Position.Y - 2, 140, 30);
                            g.DrawString(content, new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)))
                                , Brushes.Red
                                , new PointF(e.X - 65, curChartFlow.ChartAreas[0].Position.Y)
                                );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MainForm.Instance.SetSystemLog("[ERROR] PipeReport-chartFlow_MouseMove / " + ex.Message);
            }
        }
        void chartFlow_MouseUp(object sender, MouseEventArgs e)
        {
            if (curChartFlow == null) return;
            if (mDownFlow.X == 0) return;

            int minY = 40;
            int maxY = 740;

            //시작점
            List<Point> point1 = new List<Point>();
            for (int i = minY; i <= maxY; i++)
            {
                Point p = new Point(mDownFlow.X, i);
                if (!point1.Contains(p))
                    point1.Add(p);
            }

            //끝점
            List<Point> point2 = new List<Point>();
            for (int i = minY; i <= maxY; i++)
            {
                Point p = new Point(e.Location.X, i);
                if (!point2.Contains(p))
                    point2.Add(p);
            }

            List<int> ints1 = new List<int>();
            foreach (Point item in point1)
            {
                HitTestResult result = curChartFlow.HitTest(item.X, item.Y);
                if (result.PointIndex != -1 && !ints1.Contains(result.PointIndex))
                    ints1.Add(result.PointIndex);
            }

            List<int> ints2 = new List<int>();
            foreach (Point item in point2)
            {
                HitTestResult result = curChartFlow.HitTest(item.X, item.Y);
                if (result.PointIndex != -1 && !ints2.Contains(result.PointIndex))
                    ints2.Add(result.PointIndex);
            }

            double xValue1 = -1;
            foreach (int dd in ints1)
            {
                if (xValue1 < 0 || xValue1 < curChartFlow.Series[0].Points[dd].XValue)
                    xValue1 = curChartFlow.Series[0].Points[dd].XValue;
            }
            double xValue2 = -1;
            foreach (int dd in ints2)
            {
                if (xValue2 < curChartFlow.Series[0].Points[dd].XValue)
                    xValue2 = curChartFlow.Series[0].Points[dd].XValue;
            }

            DateTime beforeDate = DateTime.FromOADate(xValue1);
            DateTime afterDate = DateTime.FromOADate(xValue2);

            DateTime beforeDatePicker = new DateTime(dateTimePicker_date1.Value.Year, dateTimePicker_date1.Value.Month, dateTimePicker_date1.Value.Day,
                                                   dateTimePicker_time1.Value.Hour, dateTimePicker_time1.Value.Minute, dateTimePicker_time1.Value.Second);
            DateTime afterDatePicker = new DateTime(dateTimePicker_date2.Value.Year, dateTimePicker_date2.Value.Month, dateTimePicker_date2.Value.Day,
                                                  dateTimePicker_time2.Value.Hour, dateTimePicker_time2.Value.Minute, dateTimePicker_time2.Value.Second);

            curChartFlow.Refresh();
            mDownFlow = Point.Empty; 

            if (xValue1 <= 0 || xValue2 <= 0 || beforeDate < beforeDatePicker || afterDate > afterDatePicker || (afterDate - beforeDate).TotalMinutes <= 1)
                return;

            DisplayZoomPipe(beforeDate, afterDate);
        }
        void chartFlow_MouseLeave(object sender, EventArgs e)
        {
            if (curChartFlow == null) return;
            curChartFlow.Refresh(); 
            curChartFlow.ChartAreas[0].CursorX.Position = 0;
        } 
        void curChartFlow_Customize(object sender, EventArgs e)
        {
            curChartFlow.Customize -= curChartFlow_Customize;
            System.Diagnostics.Trace.WriteLine("customize begin " + DateTime.Now);
            if (!dicWorkDate.ContainsKey(nTabIndex)) return;

            curChartFlow.Series[0].Color = Color.Transparent;
            for (int i = 0; i < curChartFlow.Series[0].Points.Count; i++)
            {
                curChartFlow.Series[0].Points[i].Color = Color.Transparent;
            }
            int index = 0;
            List<CommonFunction.WorkListField> workList = dicWorkDate[nTabIndex];

            //curChartFlow.Series[0].Points
            foreach (CommonFunction.WorkListField item in workList)
            {
                if (item.nTankID != chartTankID) continue;
                for (int i = index; i < curChartFlow.Series[0].Points.Count; i++)
                {
                    double curWorkTime = DateTime.FromOADate(curChartFlow.Series[0].Points[i].XValue).ToOADate();
                    //DateTime dtBeginTime = DateTime.FromOADate(item.dBeginTime);
                    if ((item.dBeginTime <= curWorkTime && item.dEndTime >= curWorkTime) || (item.dBeginTime <= curWorkTime && item.dEndTime == 0))
                    {
                        curChartFlow.Series[0].Points[i].Color = Color.FromArgb(255, 137, 0); // 파랑
                        curChartFlow.Series[0].Points[i].BorderWidth = 4;                        
                    }   
                }
            } 
            System.Diagnostics.Trace.WriteLine("customize end " + DateTime.Now);
        }
        void chartFlow_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    curChartFlow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    curChartFlow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0)
                {
                    double xMin = curChartFlow.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = curChartFlow.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double yMin = curChartFlow.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    double yMax = curChartFlow.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = (curChartFlow.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + xMin) / 2;
                    double posXFinish = (curChartFlow.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + xMax) / 2;
                    double posYStart = (curChartFlow.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + yMin) / 2;
                    double posYFinish = (curChartFlow.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + yMax) / 2;

                    curChartFlow.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    curChartFlow.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        } 
        #endregion 

        #region 그리드 이벤트
        Image tempImg = global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click;
        Image tempImg2 = global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal;
        void gridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (curGridView.CurrentRow == null) return;
            long nHistoryId = Convert.ToInt64(curGridView.CurrentRow.Cells["ID"].Value);
            int nPipeId = Convert.ToInt32(curGridView.CurrentRow.Cells["PipeID"].Value);
            //string strPipeName = curGridView.CurrentRow.Cells["PipeName"].Value.ToString();
            int nType = Convert.ToInt32(curGridView.CurrentRow.Cells["Type"].Value);
            if (nHistoryId < 1 || nPipeId < 1) return;

            tempReportClickBtn.BackgroundImage = tempImg2;

            tabControl1.SelectedIndexChanged -= tabControl1_SelectedIndexChanged;
            if (nType == 0)
            {
                tabControl1.SelectedTab = tabPage_alarmHistory;
                DisplayAlarmHistory(nPipeId);
                
                foreach (DataGridViewRow item in curGridView.Rows)
                {
                    if (Convert.ToInt64(item.Cells["HistoryID"].Value) == nHistoryId)
                    {
                        curGridView.Rows[item.Index].Selected = true;
                        curGridView.FirstDisplayedScrollingRowIndex = item.Index;
                        break;
                    }
                }
                for (int i = 0; i < comboBox_pipeList.Items.Count; i++)
                {
                    CommonFunction.PipeInfo item = comboBox_pipeList.Items[i] as CommonFunction.PipeInfo;
                    if (item.nPipeID == nPipeId)
                    {
                        comboBox_pipeList.SelectedIndex = i;
                        break;
                    }
                }

                panel_alarmHistory.BackgroundImage = tempImg;
                tempReportClickBtn = panel_alarmHistory;
            }
            else if (nType == 1)
            {
                tabControl1.SelectedTab = tabPage_workHistory;
                DisplayWorkHistory(nPipeId);

                int nSelectedGridRowIndex = -1;
                
                foreach (DataGridViewRow item in curGridView.Rows)
                {
                    if (Convert.ToInt64(item.Cells["HistoryID"].Value) == nHistoryId)
                    {
                        curGridView.Rows[item.Index].Selected = true;
                        curGridView.FirstDisplayedScrollingRowIndex = item.Index;
                        nSelectedGridRowIndex = item.Index;
                        break;
                    }
                }
                for (int i = 0; i < comboBox_pipeList.Items.Count; i++)
                {
                    CommonFunction.PipeInfo item = comboBox_pipeList.Items[i] as CommonFunction.PipeInfo;
                    if (item.nPipeID == nPipeId)
                    {
                        comboBox_pipeList.SelectedIndex = i;
                        break;
                    }
                }

                // comboBox_pipeList.SelectedIndexChanged 이벤트에 의하여 의도하지 않게 바뀐
                // curGridView의 선택행을 다시 설정한다.
                if (nSelectedGridRowIndex >= 0)
                    curGridView.Rows[nSelectedGridRowIndex].Selected = true;

                panel_workHistory.BackgroundImage = tempImg;
                tempReportClickBtn = panel_workHistory;
            }

            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;

            dateTimePicker_date1.Enabled = true;
            dateTimePicker_date2.Enabled = true;
            dateTimePicker_time1.Enabled = true;
            dateTimePicker_time2.Enabled = true;

            pictureBox_doubleLeft.Visible = false;
            pictureBox_doubleRight.Visible = false;
            pictureBox_left.Visible = false;
            pictureBox_right.Visible = false;
            textBox1.Visible = false;
            label_maxPage.Visible = false;
            label_searchDate.Visible = false;
            panel_move.Visible = false;
            comboBox_pipeList.Visible = true;
            comboBox_chartPipeList.Visible = false;
        }

        void gridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            if (gridView == null) return;
            if (gridView.Columns[e.ColumnIndex].Name != "BeginTime") return;
            if (e.RowIndex < 0) return;

            string colData = gridView.Rows[e.RowIndex].Cells["SubBeginTime"].Value.ToString();
            //string colData = e.Value.ToString();
            DataGridViewCell cell = gridView[e.ColumnIndex, e.RowIndex];
            if (!colData.Contains("/"))
            {
                cell.Value = colData;
                return;
            }

            int nIdx = colData.IndexOf("/");
            string beginTime = "";
            string ignoreTime = "";

            beginTime = colData.Substring(0, nIdx);
            ignoreTime = colData.Substring(nIdx + 1);

            //Font font = new System.Drawing.Font(new Font("고딕"), 15);

            //Size fullsize = TextRenderer.MeasureText(colData, e.CellStyle.Font);
            //Size size1 = TextRenderer.MeasureText(beginTime, e.CellStyle.Font);
            //Size size2 = TextRenderer.MeasureText(ignoreTime, font);
            ////Rectangle rect1 = new Rectangle(e.CellBounds.Location.X, e.CellBounds.Location.Y, e.CellBounds.Size.Width, e.CellBounds.Size.Height / 2); 
            //Rectangle rect1 = new Rectangle(e.CellBounds.Location, e.CellBounds.Size);

            //e.Graphics.DrawString(beginTime, e.CellStyle.Font, Brushes.Black, rect1); 
            ////rect1.X += (fullsize.Width - size2.Width);
            //rect1.Y += size2.Height;
            //rect1.Width = e.CellBounds.Width;

            //e.Graphics.DrawString(ignoreTime, font, Brushes.Black, rect1.X, rect1.Y); 


            e.PaintBackground(e.ClipBounds, true); // show selection? why not..
            e.PaintContent(e.ClipBounds);          // normal content

            using (Font bigFont = new Font("나눔바른고딕", 15f))
            {
                e.Graphics.DrawString(beginTime, bigFont, cell.Selected ? Brushes.White : Brushes.Black, e.CellBounds.Left + 15, e.CellBounds.Y);
            }

            int y = e.CellBounds.Bottom - 20;  // pick your  font height
            using (Font smallFont = new Font("나눔바른고딕", 10f))
            {
                e.Graphics.DrawString("알람 무시 시간 : " + ignoreTime, smallFont, cell.Selected ? Brushes.White : Brushes.Black, e.CellBounds.Left + 15, y);
            }

            e.Handled = true;
        }
        #endregion

        #region Excel Print
        private void ExportExcel()
        {  
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Title = "Save an Excel File";
            saveDlg.DefaultExt = ".xlsx";
            saveDlg.Filter = "Excel File|*.xl*";
            saveDlg.FileName = tempReportClickBtn.Tag.ToString() + "_" + DateTime.Now.ToString("yyyyMMdd-HHmmss");

            if (saveDlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            this.Cursor = Cursors.WaitCursor;

            Excel.Application excelApp = null;
            Excel.Workbook wb = null;
            Excel.Worksheet ws = null;

            DateTime searchBeforeDate = new DateTime(dateTimePicker_date1.Value.Year, dateTimePicker_date1.Value.Month, dateTimePicker_date1.Value.Day,
                                                      dateTimePicker_time1.Value.Hour, dateTimePicker_time1.Value.Minute, dateTimePicker_time1.Value.Second);
            DateTime searchAfterDate = new DateTime(dateTimePicker_date2.Value.Year, dateTimePicker_date2.Value.Month, dateTimePicker_date2.Value.Day,
                                              dateTimePicker_time2.Value.Hour, dateTimePicker_time2.Value.Minute, dateTimePicker_time2.Value.Second);

            try
            {
                if (nTabIndex >= 1 && nTabIndex <= 11)
                {
                    Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KpxPipeMonitoring.ExcelPattern.PipePressure.xlsx");
                    if (resourceStream == null || !resourceStream.CanRead)
                        throw new ApplicationException("엑셀 양식을 찾을 수 없습니다. ");

                    string strFilePath = saveDlg.FileName;
                    int nIndex = strFilePath.LastIndexOf('\\');
                    string strFileName = strFilePath.Substring(nIndex + 1);

                    FileInfo fileInfo = new FileInfo(strFilePath);
                    using (FileStream fs = fileInfo.Create())
                    {
                        int i;
                        do
                        {
                            i = resourceStream.ReadByte(); // 해당파일을 한 바이트씩 읽음
                            if (i != -1)
                            {
                                fs.WriteByte((byte)i);
                            }
                        } while (i != -1);

                        fs.Close();
                        resourceStream.Close();
                    }

                    curChart.SaveImage(Path.GetTempPath() + strFileName.Replace(".xlsx", ".png"), ChartImageFormat.Png);
                    string flowTempFileName = strFileName.Replace(".xlsx", "") + "_flow.png";
                    curChartFlow.SaveImage(Path.GetTempPath() + flowTempFileName, ChartImageFormat.Png);
                    //curChartFlow.SaveImage(Path.GetTempPath() + strFileName.Replace(".xlsx", ".png"), ChartImageFormat.Png);

                    excelApp = MainForm.Instance.excelApp;
                    wb = excelApp.Workbooks.Open(strFilePath);
                    // 압력 그래프
                    ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;
                    ws.Cells[1, 1] = "KPX Global 배관 탱크 모니터링 시스템 (" + tempReportClickBtn.Tag.ToString() + " 압력, 유량 조회)";
                    ws.Cells[4, 1] = "조회기간 : " + searchBeforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + searchAfterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    CommonFunction.WorkListField work = comboBox_chartPipeList.SelectedItem as CommonFunction.WorkListField;
                    if (work != null)
                        ws.Cells[5, 1] = "연결된 탱크 : " + work.strTankName;
                    ws.Shapes.AddPicture(Path.GetTempPath() + strFileName.Replace(".xlsx", ".png"), Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 15, 100, 750, 250);
                    ws.Shapes.AddPicture(Path.GetTempPath() + flowTempFileName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 15, 360, 750, 250);

                    // 작업, 알람이력
                    Excel.Worksheet ws2 = null;
                    ws2 = wb.Worksheets.get_Item(2) as Excel.Worksheet;
                    ws2.Cells[1, 1] = "KPX Global 배관 탱크 모니터링 시스템 (" + tempReportClickBtn.Tag.ToString() + ")";
                    ws2.Cells[4, 1] = "조회기간 : " + searchBeforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + searchAfterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    for (int i = 0; i < curGridView.Rows.Count; i++)
                    {
                        string Num = curGridView.Rows[i].Cells["Num"].Value.ToString();
                        string TankName = curGridView.Rows[i].Cells["TankName"].Value.ToString();
                        string[] SubBeginTime = curGridView.Rows[i].Cells["SubBeginTime"].Value.ToString().Split('/');
                        string EndTime = curGridView.Rows[i].Cells["EndTime"].Value.ToString();
                        string Status = curGridView.Rows[i].Cells["Status"].Value.ToString();

                        //ws2.Cells[i + 7, 1] = Num;
                        //ws2.Cells[i + 7, 2] = TankName;
                        ////string[] strSubBeginTime = SubBeginTime;

                        //ws2.Cells[i + 7, 3] = strSubBeginTime[0];
                        //// 알람 무시 시간
                        //if (strSubBeginTime.Length == 2)
                        //    ws2.Cells[i + 7, 5] = strSubBeginTime[1];
                        //else
                        //    ws2.Cells[i + 7, 5] = "X";
                        //ws2.Cells[i + 7, 4] = EndTime;
                        //ws2.Cells[i + 7, 6] = Status;

                        List<string> strList = new List<string>();
                        strList.Add(Num);
                        strList.Add(TankName);

                        string[] strSubBeginTime = SubBeginTime;
                        strList.Add(strSubBeginTime[0]);
                        strList.Add(EndTime);
                        // 알람 무시 시간
                        if (strSubBeginTime.Length == 2)
                            strList.Add(strSubBeginTime[1]);
                        else
                            strList.Add("X"); 
                        strList.Add(Status); 

                        Microsoft.Office.Interop.Excel.Range rng = ws2.get_Range("A" + (i + 7).ToString(), "F" + (i + 7).ToString());
                        rng.Value = strList.ToArray();
                    }

                    // 테두리 추가
                    Microsoft.Office.Interop.Excel.Range cell1 = ws2.Cells[7, 1];
                    Microsoft.Office.Interop.Excel.Range cell2 = ws2.Cells[7 + curGridView.Rows.Count - 1, 6]; // 데이터가 3개 일경우 7,8,9행에 입력되므로 -1 해줌
                    ws2.get_Range(cell1, cell2).Cells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

                    ReleaseExcelObject(ws2);
                    wb.Close(true);
                    //excelApp.Quit();
                }
                else if (tabControl1.SelectedTab == tabPage_total)
                {
                    Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KpxPipeMonitoring.ExcelPattern.Total.xlsx");
                    if (resourceStream == null || !resourceStream.CanRead)
                        throw new ApplicationException("엑셀 양식을 찾을 수 없습니다. ");

                    string strFilePath = saveDlg.FileName;
                    int nIndex = strFilePath.LastIndexOf('\\');
                    string strFileName = strFilePath.Substring(nIndex + 1);

                    FileInfo fileInfo = new FileInfo(strFilePath);
                    using (FileStream fs = fileInfo.Create())
                    {
                        int i;
                        do
                        {
                            i = resourceStream.ReadByte(); // 해당파일을 한 바이트씩 읽음
                            if (i != -1)
                            {
                                fs.WriteByte((byte)i);
                            }
                        } while (i != -1);

                        fs.Close();
                        resourceStream.Close();
                    }
                    excelApp = MainForm.Instance.excelApp;
                    wb = excelApp.Workbooks.Open(strFilePath);
                       
                    ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;  
                    for (int i = 0; i < curGridView.Rows.Count; i++)
                    {
                        string PipeName = curGridView.Rows[i].Cells["PipeName"].Value.ToString();
                        string TankName = curGridView.Rows[i].Cells["TankName"].Value.ToString();
                        string AvgPressure = curGridView.Rows[i].Cells["AvgPressure"].Value.ToString();
                        string MaxPressure = curGridView.Rows[i].Cells["MaxPressure"].Value.ToString();
                        string MinPressure = curGridView.Rows[i].Cells["MinPressure"].Value.ToString();
                        string AvgFlow = curGridView.Rows[i].Cells["AvgFlow"].Value.ToString();
                        string MaxFlow = curGridView.Rows[i].Cells["MaxFlow"].Value.ToString();
                        string MinFlow = curGridView.Rows[i].Cells["MinFlow"].Value.ToString();
                        string RecentWorkTime = curGridView.Rows[i].Cells["RecentWorkTime"].Value.ToString();
                        string Status = curGridView.Rows[i].Cells["Status"].Value.ToString();

                        //ws.Cells[i + 5, 2] = PipeName;
                        //ws.Cells[i + 5, 3] = TankName;
                        //ws.Cells[i + 5, 4] = AvgPressure;
                        //ws.Cells[i + 5, 5] = MaxPressure;
                        //ws.Cells[i + 5, 6] = MinPressure;
                        //ws.Cells[i + 5, 7] = AvgFlow;
                        //ws.Cells[i + 5, 8] = MaxFlow;
                        //ws.Cells[i + 5, 9] = MinFlow;
                        //ws.Cells[i + 5, 10] = RecentWorkTime;
                        //ws.Cells[i + 5, 11] = Status;

                        List<string> strList = new List<string>();
                        strList.Add((i + 1).ToString());
                        strList.Add(PipeName);
                        strList.Add(TankName);
                        strList.Add(AvgPressure);
                        strList.Add(MaxPressure);
                        strList.Add(MinPressure);
                        strList.Add(AvgFlow);
                        strList.Add(MaxFlow);
                        strList.Add(MinFlow);
                        strList.Add(RecentWorkTime);
                        strList.Add(Status);

                        Microsoft.Office.Interop.Excel.Range rng = excelApp.get_Range("A" + (i + 5).ToString(), "K" + (i + 5).ToString());
                        rng.Value = strList.ToArray();
                    }
                     
                    wb.Close(true);
                    //excelApp.Quit();
                }
                else if (tabControl1.SelectedTab == tabPage_alarmHistory)
                {
                    Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KpxPipeMonitoring.ExcelPattern.AlarmHistory.xlsx");
                    if (resourceStream == null || !resourceStream.CanRead)
                        throw new ApplicationException("엑셀 양식을 찾을 수 없습니다. ");

                    string strFilePath = saveDlg.FileName;
                    int nIndex = strFilePath.LastIndexOf('\\');
                    string strFileName = strFilePath.Substring(nIndex + 1);

                    FileInfo fileInfo = new FileInfo(strFilePath);
                    using (FileStream fs = fileInfo.Create())
                    {
                        int i;
                        do
                        {
                            i = resourceStream.ReadByte(); // 해당파일을 한 바이트씩 읽음
                            if (i != -1)
                            {
                                fs.WriteByte((byte)i);
                            }
                        } while (i != -1);

                        fs.Close();
                        resourceStream.Close();
                    }
                    excelApp = MainForm.Instance.excelApp;
                    wb = excelApp.Workbooks.Open(strFilePath);

                    ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;

                    System.Diagnostics.Trace.WriteLine(DateTime.Now);
                    string pipeName = ((KpxPipeMonitoring.CommonFunction.PipeInfo)comboBox_pipeList.SelectedItem).strPipeName;
                    ws.Cells[1, 1] = "KPX Global 배관 탱크 모니터링 시스템 (알람이력 - " + pipeName + ")";
                    ws.Cells[4, 1] = pipeName;
                    ws.Cells[4, 7] = "조회기간 : " + searchBeforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + searchAfterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    for (int i = 0; i < curGridView.Rows.Count; i++)
                    {
                        string PipeName = curGridView.Rows[i].Cells["PipeName"].Value.ToString();
                        string TankName = curGridView.Rows[i].Cells["TankName"].Value.ToString();
                        string BeginTime = curGridView.Rows[i].Cells["BeginTime"].Value.ToString();
                        string EndTime = curGridView.Rows[i].Cells["EndTime"].Value.ToString();
                        string AlarmTime = curGridView.Rows[i].Cells["AlarmTime"].Value.ToString();
                        string StandardPressure = curGridView.Rows[i].Cells["StandardPressure"].Value.ToString();
                        string AlarmPressure = curGridView.Rows[i].Cells["AlarmPressure"].Value.ToString();
                        string Status = curGridView.Rows[i].Cells["Status"].Value.ToString();
                        string Terminator = curGridView.Rows[i].Cells["Terminator"].Value.ToString();
                        string AlarmOccurrence = curGridView.Rows[i].Cells["AlarmOccurrence"].Value.ToString();
                        string AlarmComment = curGridView.Rows[i].Cells["AlarmComment"].Value.ToString();
                         
                        List<string> strList = new List<string>();
                        strList.Add((i + 1).ToString());
                        strList.Add(PipeName);
                        strList.Add(TankName);
                        strList.Add(BeginTime);
                        strList.Add(EndTime);
                        strList.Add(AlarmTime);
                        strList.Add(StandardPressure);
                        strList.Add(AlarmPressure);
                        strList.Add(Status);
                        strList.Add(Terminator);
                        strList.Add(AlarmOccurrence);
                        strList.Add(AlarmComment);

                        Microsoft.Office.Interop.Excel.Range rng = excelApp.get_Range("A" + (i + 6).ToString(), "L" + (i + 6).ToString());
                        rng.Value = strList.ToArray();
                    }

                    // 테두리 추가
                    Microsoft.Office.Interop.Excel.Range cell1 = ws.Cells[6, 1];
                    Microsoft.Office.Interop.Excel.Range cell2 = ws.Cells[6 + curGridView.Rows.Count - 1, 12]; // 데이터가 3개 일경우 7,8,9행에 입력되므로 -1 해줌
                    ws.get_Range(cell1, cell2).Cells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

                    wb.Close(true);
                    //excelApp.Quit();
                    System.Diagnostics.Trace.WriteLine(DateTime.Now);
                }
                else if (tabControl1.SelectedTab == tabPage_workHistory)
                {
                    System.Diagnostics.Trace.WriteLine("1 : " + DateTime.Now);
                    Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KpxPipeMonitoring.ExcelPattern.WorkHistory.xlsx");
                    if (resourceStream == null || !resourceStream.CanRead)
                        throw new ApplicationException("엑셀 양식을 찾을 수 없습니다. ");

                    System.Diagnostics.Trace.WriteLine("2 : " + DateTime.Now);

                    string strFilePath = saveDlg.FileName;
                    int nIndex = strFilePath.LastIndexOf('\\');
                    string strFileName = strFilePath.Substring(nIndex + 1);

                    FileInfo fileInfo = new FileInfo(strFilePath);
                    using (FileStream fs = fileInfo.Create())
                    {
                        int i;
                        do
                        {
                            i = resourceStream.ReadByte(); // 해당파일을 한 바이트씩 읽음
                            if (i != -1)
                            {
                                fs.WriteByte((byte)i);
                            }
                        } while (i != -1);

                        fs.Close();
                        resourceStream.Close();
                    }
                    excelApp = MainForm.Instance.excelApp;
                    wb = excelApp.Workbooks.Open(strFilePath);
                    excelApp.ScreenUpdating = false;
                    ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;

                    System.Diagnostics.Trace.WriteLine("3 : " + DateTime.Now);

                    string pipeName = ((KpxPipeMonitoring.CommonFunction.PipeInfo)comboBox_pipeList.SelectedItem).strPipeName;
                    ws.Cells[1, 1] = "KPX Global 배관 탱크 모니터링 시스템 (작업이력 - " + pipeName + ")";
                    ws.Cells[4, 1] = pipeName;
                    ws.Cells[4, 7] = "조회기간 : " + searchBeforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + searchAfterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    System.Diagnostics.Trace.WriteLine("4 : " + DateTime.Now);
                     
                    for (int i = 0; i < curGridView.Rows.Count; i++)
                    {
                        string PipeName = curGridView.Rows[i].Cells["PipeName"].Value.ToString();
                        string TankName = curGridView.Rows[i].Cells["TankName"].Value.ToString();
                        string BeginTime = curGridView.Rows[i].Cells["BeginTime"].Value.ToString();
                        string EndTime = curGridView.Rows[i].Cells["EndTime"].Value.ToString();
                        string CTime = curGridView.Rows[i].Cells["CTime"].Value.ToString();
                        string IgnoreCTime = curGridView.Rows[i].Cells["IgnoreCTime"].Value.ToString();
                        string BeginUserName = curGridView.Rows[i].Cells["BeginUserName"].Value.ToString();
                        string EndUserId = curGridView.Rows[i].Cells["EndUserId"].Value.ToString();

                        List<string> strList = new List<string>();
                        strList.Add((i + 1).ToString());
                        strList.Add(PipeName);
                        strList.Add(TankName);
                        strList.Add(BeginTime);
                        strList.Add(EndTime);
                        strList.Add(CTime);
                        strList.Add(IgnoreCTime);
                        strList.Add(BeginUserName);
                        strList.Add(EndUserId); 

                        Microsoft.Office.Interop.Excel.Range rng = excelApp.get_Range("A" + (i + 6).ToString(), "I" + (i + 6).ToString());
                        rng.Value = strList.ToArray();
                    } 

                    System.Diagnostics.Trace.WriteLine("5 : " + DateTime.Now);
                    // 테두리 추가
                    Microsoft.Office.Interop.Excel.Range cell1 = ws.Cells[6, 1];
                    Microsoft.Office.Interop.Excel.Range cell2 = ws.Cells[6 + curGridView.Rows.Count - 1, 9]; // 데이터가 3개 일경우 7,8,9행에 입력되므로 -1 해줌
                    ws.get_Range(cell1, cell2).Cells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

                    excelApp.ScreenUpdating = true;
                    wb.Close(true);
                    //excelApp.Quit();
                    System.Diagnostics.Trace.WriteLine("6 : " + DateTime.Now);
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    UnE.Utility.UMessageBox.Show("Excel로 Import할 항목이 없습니다.");
                    return;
                }

                System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                info.FileName = saveDlg.FileName;

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo = info;

                process.Start();
                System.Diagnostics.Trace.WriteLine("7 : " + DateTime.Now);
                this.Cursor = Cursors.Default; 

            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                UnE.Utility.UMessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                ReleaseExcelObject(ws);
                ReleaseExcelObject(wb);
                //ReleaseExcelObject(excelApp);
            }
        }
        private void ReleaseExcelObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception)
            {
                obj = null; 
            }
            finally
            {
                GC.Collect();
            }
        }
        #endregion

        #region Display
        private void DisplayPipeList()
        {
            comboBox_pipeList.ValueMember = "nPipeID";
            comboBox_pipeList.DisplayMember = "strPipeName";

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData("SELECT ID, NAME, Type FROM Pipe ORDER BY NAME", 0);
            if (arrResult == null) return;
             
            comboBox_pipeList.Items.Add(new CommonFunction.PipeInfo(0, "전체 배관", "", 0, 0, 0, 0));

            for (int i = 0; i < arrResult.Count; i += 3)
            {
                int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strPipeName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                string strPipeType = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);

                comboBox_pipeList.Items.Add(new CommonFunction.PipeInfo(nPipeID, strPipeName + " " + strPipeType, strPipeType, 0, 0, 0, 0));

                if (nPipeID == 1)
                    label_1.Text = strPipeName;
                else if (nPipeID == 2)
                    label_2.Text = strPipeName;
                else if (nPipeID == 3)
                    label_3.Text = strPipeName;
                else if (nPipeID == 4)
                    label_4.Text = strPipeName;
                else if (nPipeID == 5)
                    label_5.Text = strPipeName;
                else if (nPipeID == 6)
                    label_6.Text = strPipeName;
                else if (nPipeID == 7)
                    label_7.Text = strPipeName;
                else if (nPipeID == 8)
                    label_8.Text = strPipeName;
                else if (nPipeID == 9)
                    label_9.Text = strPipeName;
                else if (nPipeID == 10)
                    label_10.Text = strPipeName;
                else if (nPipeID == 11)
                    label_11.Text = strPipeName;
            }
            if (comboBox_pipeList.Items.Count > 0)
                comboBox_pipeList.SelectedIndex = 0;
        }

        private void DisplayTotal()
        {
            dataGridView_total.Rows.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.ID as pipeId, Concat(Name, ' ', Type) as pipeName, AvgPressure, MaxPressure, MinPressure, Concat(BeginTime, '~', ifnull(EndTime, ' ')) as BeginTime, ");
            sb.Append("       CASE WHEN status=0 THEN '정상' ELSE (select Description from AlarmType where id=p.Status) END as Status,  ");
            //sb.Append("       CONVERT((select dd from (select pipeid, Concat(SEC_TO_TIME(sum(TIME_TO_SEC(timediff(EndTime, BeginTime)))),'') as dd from pipeworkhistory group by pipeid) x where p.id=x.pipeid), char) as SumWorkTime, ");
            sb.Append("       (select Concat(Name, ' ', Type) from tank as t where t.id=lwh.tankid) as tankName, AvgFlow, MaxFlow, MinFlow ");
            sb.Append("  FROM Pipe as p LEFT OUTER JOIN (select * from lastworkhistory as lwh where begintime = (select max(begintime) from lastworkhistory as lwh2 where lwh2.pipeid=lwh.pipeid)) as lwh ON p.id=lwh.pipeid ");
            sb.Append(" ORDER BY pipeName");

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult == null) return;

            for (int i = 0; i < arrResult.Count; i += 11)
            {
                int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strPipeName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                double dAvgPressure = (arrResult[i + 2].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 2]);
                double dMaxPressure = (arrResult[i + 3].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 3]);
                double dMinPressure = (arrResult[i + 4].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 4]);
                string strRecentBeginTime = (arrResult[i + 5].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 5]);
                //string strSumWorkTime = DBUtility.WebDBManager.GetStringField(arrResult[i + 6]);
                string strStatus = (arrResult[i + 6].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 6]);

                string strTankName = (arrResult[i + 7].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 7]);
                double dAvgFlow = (arrResult[i + 8].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 8]);
                double dMaxFlow = (arrResult[i + 9].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 9]);
                double dMinFlow = (arrResult[i + 10].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 10]);

                string strAvgPressure = dAvgPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dAvgPressure));
                string strMaxPressure = dMaxPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dMaxPressure));
                string strMinPressure = dMinPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dMaxPressure));

                string strAvgFlow = dAvgPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dAvgFlow));
                string strMaxFlow = dMaxPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dMaxFlow));
                string strMinFlow = dMinPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dMinFlow));

                dataGridView_total.Rows.Add(strPipeName, strTankName, strAvgPressure, strMaxPressure, strMinPressure, strAvgFlow, strMaxFlow, strMinFlow, strRecentBeginTime, strStatus);
            }
        }

        private void DisplayAlarmHistory(int pipeID = 0)
        {
            dataGridView_alarmHistory.Rows.Clear();

            StringBuilder sb = new StringBuilder(); 
            sb.Append("SELECT p.ID as PipeID, ah.ID as HistoryID, Concat(Name, ' ', Type) as Name, BeginTime, EndTime, alarmOccurType, alarmComment, ");
            sb.Append("       Concat(SEC_TO_TIME(TIME_TO_SEC(timediff(EndTime, BeginTime))),'') as AlarmTime, RealValue, ");
            sb.Append("       (select description from alarmType as at where at.id=ah.AlarmType) as AlarmType, ");
            sb.Append("       (select CASE WHEN Mobile = 0 THEN UserName WHEN Mobile = 1 THEN concat('모바일(', UserName, ')') END from user where id=ah.alarmterminator) as Terminator ");
            sb.Append("       , (select concat(Name, ' ', Type) from tank as t where t.id=ah.tankid) as tankName, StandardValue, StandardRange ");
            sb.Append("  FROM Pipe as p INNER JOIN AlarmHistory as ah ON p.id=ah.PipeID ");
            sb.AppendFormat("   WHERE date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}'", date1, time1, date2, time2);
            sb.Append("   AND AlarmType in (256,512) "); // 압력상승, 압력하강
            if (pipeID > 0)
                sb.Append(" AND p.id = " + pipeID);
            sb.Append(" ORDER BY BeginTime DESC ");

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult == null) return;

            int nNum = 1;
            for (int i = 0; i < arrResult.Count; i += 14)
            {
                int nRow = 0;
                int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[i + ++nRow].ToString(), -1);
                string strPipeName = DBUtility.WebDBManager.GetStringField(arrResult[i + ++nRow]);
                string strBeginTime = (arrResult[i + ++nRow].ToString() == "null") ? "-" : Convert.ToDateTime(arrResult[i + nRow]).ToString("yyyy-MM-dd HH:mm:ss").Replace(".0", "");
                string strEndTime = (arrResult[i + ++nRow].ToString() == "null") ? "-" : Convert.ToDateTime(arrResult[i + nRow]).ToString("yyyy-MM-dd HH:mm:ss").Replace(".0", "");
                int nAlarmOccurType = DBUtility.WebDBManager.GetIntField(arrResult[i + ++nRow].ToString(), -1);
                string strAlarmComment = (arrResult[i + ++nRow].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + nRow]);
                string strAlarmTime = (arrResult[i + ++nRow].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + nRow]);
                double dAlarmPressure = (arrResult[i + ++nRow].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + nRow]);
                string strStatus = (arrResult[i + ++nRow].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + nRow]);
                string strAlarmUnit = "";
                if (strStatus.Contains("압력"))
                    strAlarmUnit = " kg/cm²";
                else if (strStatus.Contains("레벨"))
                    strAlarmUnit = " m";
                else if (strStatus.Contains("유량"))
                    strAlarmUnit = " kl/h";
                else if (strStatus.Contains("온도"))
                    strAlarmUnit = " ℃";
                string strTerminator = (arrResult[i + ++nRow].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + nRow]);
                string strTankName = (arrResult[i + ++nRow].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + nRow]);

                string strAlarmPressure = MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F2}", dAlarmPressure)) + strAlarmUnit;
                double nStandardValue = (arrResult[i + ++nRow].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + nRow]);
                double nStandardRange = (arrResult[i + ++nRow].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + nRow]);

                string strStandardValue = "-";
                if (nStandardValue != -9999 && nStandardValue != -999)
                {
                    if (nStandardValue - nStandardRange == 0 && nStandardValue + nStandardRange == 0)
                        strStandardValue = "0 ~ 0";
                    else
                        strStandardValue = String.Format("{0:F2}", nStandardValue - nStandardRange) + " ~ " + String.Format("{0:F2}", nStandardValue + nStandardRange);
                }

                if (nPipeID < 0) continue;

                string strOccurrence = "-";
                if(nAlarmOccurType != -1)
                    strOccurrence = KpxPipeMonitoring.Popups.AlarmClear.occurenceTypeString[nAlarmOccurType];

                dataGridView_alarmHistory.Rows.Add(nNum, nPipeID, nHistoryID, strPipeName, strTankName, strBeginTime, strEndTime, strAlarmTime, strStandardValue, strAlarmPressure, strStatus, strTerminator, strOccurrence, strAlarmComment);
                nNum++;
            }
        }

        private void DisplayWorkHistory(int pipeID = 0)
        {
            dataGridView_workHistory.Rows.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT p.id as PipeID, wh.id as HistoryID, Concat(Name, ' ', Type) as PipeName, BeginTime, EndTime,  ");
            sb.Append("       Concat(SEC_TO_TIME(TIME_TO_SEC(timediff(IFNULL(EndTime, now()), BeginTime))),'') as CTime, ");
            sb.Append("       IFNULL((select ignoreTime from AlarmIgnoreHistory as aih where aih.workhistoryid=wh.id and aih.ignorebegintime=wh.begintime),'15분') as ignoreTime, ");
            sb.Append("       CASE ");
            sb.Append("           WHEN (select concat(date_format(IgnoreBeginTime,'%H:%i:%s'),'~',date_format(IgnoreEndTime,'%H:%i:%s')) from AlarmIgnoreHistory as aih where aih.workhistoryid=wh.id and aih.ignorebegintime=wh.begintime) IS NULL ");
            sb.Append("           THEN Concat(date_format(BeginTime,'%H:%i:%s'),'~',date_format(date_add(BeginTime, interval 15 minute),'%H:%i:%s')) ");
            sb.Append("           ELSE (select concat(date_format(IgnoreBeginTime,'%H:%i:%s'),'~',date_format(IgnoreEndTime,'%H:%i:%s')) from AlarmIgnoreHistory as aih where aih.workhistoryid=wh.id and aih.ignorebegintime=wh.begintime) ");
            sb.Append("        END as ignoreCTime, ");
            sb.Append("        CASE ");
		    sb.Append("           WHEN BeginCmdHistoryID = -1 THEN '자동시작' ");
            sb.Append("           ELSE (select case when mobile=0 then username when mobile=1 then concat('모바일(', username, ')') end ");
            sb.Append("                  from user where id = (select userid from commandhistory where id = wh.begincmdhistoryid)) ");
            sb.Append("       END as beginUserName, ");
            sb.Append("       CASE ");            
		    sb.Append("           WHEN EndCmdHistoryID = -1 THEN '자동종료'   ");         
		    sb.Append("           WHEN EndCmdHistoryID = -2 THEN '작업중' ");
		    sb.Append("           ELSE (select case when mobile=0 then username when mobile=1 then concat('모바일(', username, ')') end ");
		    sb.Append("           from user where id = (select userid from commandhistory where id = wh.EndCmdHistoryID)) "); 
            sb.Append("       END as EndUserName ");
            sb.Append("       , (select concat(Name, ' ', Type) from tank as t where t.id=wh.tankid) as tankName ");
            sb.Append("  FROM Pipe as p INNER JOIN WorkHistory as wh ON p.ID=wh.PipeID ");
            sb.AppendFormat(" AND (date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}')", date1, time1, date2, time2);
            if (pipeID > 0)
                sb.Append(" AND p.id = " + pipeID);
            sb.Append(" ORDER BY BeginTime DESC  ");
             
            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult == null) return;
             
            int nNum = 1;
            for (int i = 0; i < arrResult.Count; i += 11)
            {
                int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                long nHistoryID = Convert.ToInt64(arrResult[i + 1].ToString());
                string strPipeName = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);
                string strBeginTime = (arrResult[i + 3].ToString() == "null") ? "-" : Convert.ToDateTime(arrResult[i + 3]).ToString("yyyy-MM-dd HH:mm:ss").Replace(".0", "");
                string strEndTime = (arrResult[i + 4].ToString() == "null") ? "-" : Convert.ToDateTime(arrResult[i + 4]).ToString("yyyy-MM-dd HH:mm:ss").Replace(".0", "");
                string strCTime = (arrResult[i + 5].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 5]);
                string strIgnoreTime = (arrResult[i + 6].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 6]);
                string strIgnoreCTime = (arrResult[i + 7].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 7]);
                string strBeginUserName = (arrResult[i + 8].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 8]);
                string strEndUserName = (arrResult[i + 9].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 9]);
                string strTankName = (arrResult[i + 10].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 10]);

                if (nPipeID < 0) continue; 
                dataGridView_workHistory.Rows.Add(nNum, nPipeID, nHistoryID, strPipeName, strTankName, strBeginTime, strEndTime, strCTime, strIgnoreCTime, strBeginUserName, strEndUserName);
                nNum++;
            } 
        }
        #endregion
         
        #region 배관 조회
        private int chartTankID = -1;
        private void DisplayTankConnectedPipes(List<CommonFunction.WorkListField> workList)
        {
            if (comboBox_chartPipeList.Items != null)
                comboBox_chartPipeList.Items.Clear();
            chartTankID = -1;

            if (workList == null)
            {
                comboBox_chartPipeList.Visible = false;
                return;
            }

            Dictionary<int, CommonFunction.WorkListField> works = new Dictionary<int, CommonFunction.WorkListField>();

            foreach (CommonFunction.WorkListField item in workList)
            {
                if (item.nTankID < 0)
                    continue;
                if (!works.ContainsKey(item.nTankID))
                    works.Add(item.nTankID, item);
                else
                {
                    // workList는 최근 작업이력 순으로 들어옴
                    if (works[item.nTankID].dBeginTime > item.dBeginTime)
                        works[item.nTankID].dBeginTime = item.dBeginTime;
                }
            }

            foreach (KeyValuePair<int, CommonFunction.WorkListField> item in works)
            {
                comboBox_chartPipeList.Items.Add(item.Value);
            }

            if (comboBox_chartPipeList.Items.Count > 0)
            {
                comboBox_chartPipeList.SelectedIndex = 0;
                comboBox_chartPipeList.Visible = true;
            }
            else
                comboBox_chartPipeList.Visible = false;
        }

        void comboBox_chartPipeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommonFunction.WorkListField work = comboBox_chartPipeList.SelectedItem as CommonFunction.WorkListField;
            if (work == null)
            {
                chartTankID = -1;
                return;
            }

            chartTankID = work.nTankID;
            DisplayPipe(); 
        } 
        private void DisplayPipeAlarmWorkHistroy()
        {
            #region 알람, 작업 이력
            curGridView.Rows.Clear();

            if (dicWorkDate.ContainsKey(nTabIndex))
                dicWorkDate[nTabIndex].Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ID, PipeID, BeginTime, EndTime, (select Description from AlarmType where id=ah.AlarmType) as Status, 0 as Type ");
            sb.Append("     , (select Concat(Name, ' ', Type) from tank as t where t.id=ah.tankid) as tankName ");
            sb.Append("     , (select id from tank as t where t.id=ah.tankid) as tankId ");
            sb.Append("  FROM AlarmHistory ah  ");
            sb.Append(" WHERE PipeId = " + nTabIndex);
            sb.AppendFormat(" AND date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}'", date1, time1, date2, time2);
            sb.Append("   AND AlarmType in (256,512) "); // 압력상승, 압력하강
            sb.Append(" UNION ALL ");
            sb.Append("SELECT ID, PipeID, ");
            sb.Append("       CONCAT(BeginTime, '/', ");
            sb.Append("             CASE ");
            sb.Append("               WHEN (select concat(date_format(IgnoreBeginTime,'%H:%i:%s'),'~',date_format(IgnoreEndTime,'%H:%i:%s')) from AlarmIgnoreHistory as aih where aih.workhistoryid=wh.id and aih.ignorebegintime=wh.begintime) IS NULL ");
            sb.Append("               THEN Concat(date_format(BeginTime,'%H:%i:%s'),'~',date_format(date_add(BeginTime, interval 15 minute),'%H:%i:%s')) ");
            sb.Append("               ELSE (select concat(date_format(IgnoreBeginTime,'%H:%i:%s'),'~',date_format(IgnoreEndTime,'%H:%i:%s')) from AlarmIgnoreHistory as aih where aih.workhistoryid=wh.id and aih.ignorebegintime=wh.begintime) ");
            sb.Append("             END) as begintime, ");
            sb.Append("       EndTime, CASE WHEN EndTime IS NULL THEN '작업중' WHEN EndTime IS NOT NULL THEN '작업종료' END as status, 1 as Type ");
            sb.Append("     , (select Concat(Name, ' ', Type) from tank as t where t.id=wh.tankid) as tankName ");
            sb.Append("     , (select id from tank as t where t.id=wh.tankid) as tankId ");
            sb.Append("  FROM WorkHistory as wh ");
            sb.Append(" WHERE PipeId = " + nTabIndex);
            sb.AppendFormat(" AND (date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}')", date1, time1, date2, time2);
            sb.Append(" ORDER BY BeginTime DESC");

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult == null) return;

            Font font = new System.Drawing.Font("나눔바른고딕", 15f);
            List<CommonFunction.WorkListField> workList = new List<CommonFunction.WorkListField>();

            int nNum = 1;
            for (int i = 0; i < arrResult.Count; i += 8)
            {
                long nID = Convert.ToInt64(arrResult[i].ToString());
                int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strBeginTime = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);
                string strEndTime = (arrResult[i + 3].ToString() == "null") ? "-" : Convert.ToDateTime(arrResult[i + 3]).ToString("yyyy-MM-dd HH:mm:ss").Replace(".0", "");
                string strStatus = DBUtility.WebDBManager.GetStringField(arrResult[i + 4]);
                int nType = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strTankName = (arrResult[i + 6].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 6]);
                int nTankId = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);

                if (nPipeID < 0) continue;

                curGridView.Rows.Add(nNum, strTankName, nID, nPipeID, strBeginTime, "", strEndTime, strStatus, nType);
                curGridView.Rows[nNum - 1].DefaultCellStyle.Font = font;
                if (nType == 0)
                    curGridView.Rows[nNum - 1].DefaultCellStyle.ForeColor = Color.Red;
                else
                {
                    DBUtility.VariousData<DateTime> dtBeginTime = new DBUtility.VariousData<DateTime>();
                    dtBeginTime.Data = Convert.ToDateTime(strBeginTime.Substring(0, strBeginTime.IndexOf('/')));

                    double dEndTime = 0;
                    if (strEndTime != "-")
                        dEndTime = Convert.ToDateTime(strEndTime).ToOADate();
                    workList.Add(new CommonFunction.WorkListField(nTabIndex, nTankId, "", strTankName, dtBeginTime.Data.ToOADate(), dEndTime));
                }
                nNum++;
            }

            if (!dicWorkDate.ContainsKey(nTabIndex))
                dicWorkDate.Add(nTabIndex, workList);
            else
                dicWorkDate[nTabIndex] = workList;

            #endregion  

            DisplayTankConnectedPipes(workList);
        }
        /// <summary>
        /// 배관별 전체 데이터
        /// </summary>
        SortedList<int, List<CommonFunction.ChartField>> dicTotalChartData = new SortedList<int, List<CommonFunction.ChartField>>();
        SortedList<int, List<CommonFunction.WorkListField>> dicWorkDate = new SortedList<int, List<CommonFunction.WorkListField>>();
        
        private void DisplayPipe()
        { 
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if (dicTotalChartData.ContainsKey(nTabIndex))
                    dicTotalChartData[nTabIndex].Clear();
                 
                List<CommonFunction.ChartField> totalChartData = new List<CommonFunction.ChartField>();
                List<CommonFunction.ChartField> displayChartData = new List<CommonFunction.ChartField>();
                //Dictionary<DateTime, double> dicTempDatas = new Dictionary<DateTime, double>();
                Dictionary<DateTime, List<double>> dicTempDatas = new Dictionary<DateTime, List<double>>();
                
                DateTime beforeDate = new DateTime(dateTimePicker_date1.Value.Year, dateTimePicker_date1.Value.Month, dateTimePicker_date1.Value.Day,
                                                   dateTimePicker_time1.Value.Hour, dateTimePicker_time1.Value.Minute, dateTimePicker_time1.Value.Second);
                DateTime afterDate = new DateTime(dateTimePicker_date2.Value.Year, dateTimePicker_date2.Value.Month, dateTimePicker_date2.Value.Day,
                                                  dateTimePicker_time2.Value.Hour, dateTimePicker_time2.Value.Minute, dateTimePicker_time2.Value.Second);
                 
                #region 1. DB로 읽기
                //StringBuilder sb = new StringBuilder();
                //sb.Append("SELECT PipeID, TimeStamp, ph.Pressure ");
                //sb.Append("  FROM PipeHistory ph ");
                //sb.Append(" WHERE PipeID = " + nTabIndex);
                //sb.Append("   AND timestamp >= '" + beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                //sb.Append("   AND timestamp <= '" + afterDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                
                //ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(TablePartition.GetReportHistorySQL(nTabIndex, beforeDate, afterDate), 0);
                //if (arrResult == null) return;

                //if (!dicTotalChartData.ContainsKey(nTabIndex))
                //    dicTotalChartData.Add(nTabIndex, new List<CommonFunction.PipeChartField>());

                //totalChartData.Capacity = arrResult.Count / 3;
                //dicTotalChartData[nTabIndex].Capacity = arrResult.Count / 3;

                //for (int i = 0; i < arrResult.Count; i += 3)
                //{
                //    int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                //    string strPipeName = "";
                //    DBUtility.VariousData<DateTime> date = new DBUtility.VariousData<DateTime>();
                //    date.Data = Convert.ToDateTime(arrResult[i + 1]);

                //    double pressure = (arrResult[i + 2].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 2]);

                //    totalChartData.Add(new CommonFunction.PipeChartField(nPipeID, strPipeName, date.Data, date.Data.ToString(""), pressure));
                //} 
                #endregion 
                 
                int displayCondition = MainForm.Instance.commonFunction.GetChartPointCount(beforeDate, afterDate);
                 
                #region 2. 파일로 읽기
                List<HistoryQuery> historyQueries = new List<HistoryQuery>();
                //int totalDays = (int)(afterDate - beforeDate).TotalDays;
                //for (int i = 0; i <= totalDays; i++)
                //{
                //    string y = beforeDate.AddDays(i).Year.ToString();
                //    string m = beforeDate.AddDays(i).Month.ToString();
                //    string d = beforeDate.AddDays(i).Day.ToString();

                //    HistoryQuery query = new HistoryQuery(nTabIndex, y, m, d, HistoryQueryType.작업중);
                //    historyQueries.Add(query);
                //}
                
                int f = 0;
                while (true)
                {
                    DateTime date = beforeDate.AddDays(f);
                    string y = date.Year.ToString();
                    string m = date.Month.ToString();
                    string d = date.Day.ToString();

                    HistoryQuery query = new HistoryQuery(nTabIndex, y, m, d, HistoryQueryType.작업중);
                    historyQueries.Add(query);
                    if (date > afterDate)
                        break;

                    f++;
                }

                totalChartData = m_historyMgr.ReadHistory(historyQueries);
                historyQueries.Clear();
                historyQueries = null;
                #endregion
                 
                if (!dicTotalChartData.ContainsKey(nTabIndex))
                    dicTotalChartData.Add(nTabIndex, totalChartData);
                else
                    dicTotalChartData[nTabIndex] = totalChartData;

                int k = 0;
                foreach (CommonFunction.ChartField item in totalChartData)
                {
                    k++;

                    if (k == 2186)
                    {

                    }

                    //if (item.nTankID != 0 && item.nTankID != chartTankID) continue;

                    if (item.dtTimeStamp >= beforeDate && item.dtTimeStamp <= afterDate)
                    {
                        // 첫번째 데이터는 무조건 넣기
                        if (displayChartData.Count == 0)
                            displayChartData.Add(new CommonFunction.ChartField(item.nPipeID, item.nTankID, item.dtTimeStamp, item.dPressure, item.dFlow));
                        else
                        {
                            if (displayChartData[displayChartData.Count - 1].dtTimeStamp < item.dtTimeStamp)
                            {
                                if (!dicTempDatas.ContainsKey(item.dtTimeStamp))
                                {
                                    List<double> doubles = new List<double>();
                                    doubles.Add(item.dPressure);
                                    doubles.Add(item.dFlow);
                                    dicTempDatas.Add(item.dtTimeStamp, doubles);
                                }

                                if (dicTempDatas.Count >= displayCondition)
                                {
                                    double tempPressure = 0;
                                    double tempFlow = 0;
                                    foreach (KeyValuePair<DateTime, List<double>> item2 in dicTempDatas)
                                    {
                                        tempPressure = tempPressure + item2.Value[0];
                                        tempFlow = tempFlow + item2.Value[1];
                                    }
                                    displayChartData.Add(new CommonFunction.ChartField(item.nPipeID, item.nTankID, item.dtTimeStamp, tempPressure / displayCondition, tempFlow / displayCondition));
                                    dicTempDatas.Clear();
                                }
                            }
                        }
                    }
                }
                 
                // 조회조건에 미치지 못해서 (ex: 30분일경우 3개, 1시간일경우 6개) 차트 데이터가 add되지 못한 경우
                if (dicTempDatas.Count > 0)
                {
                    Dictionary<DateTime, List<double>> dicTempDatas2 = new Dictionary<DateTime, List<double>>();
                    List<double> doubles2 = new List<double>();
                    DateTime beforeDate2 = dicTempDatas.Keys.Min();
                    DateTime afterDate2 = dicTempDatas.Keys.Max();
                    int displayCondition2 = MainForm.Instance.commonFunction.GetChartPointCount(beforeDate2, afterDate2); 
                    foreach (KeyValuePair<DateTime, List<double>> item in dicTempDatas)
                    {
                        if (item.Key >= beforeDate && item.Key <= afterDate)
                        {
                            if (displayChartData.Count == 0 || displayChartData[displayChartData.Count - 1].dtTimeStamp < item.Key)
                            {
                                if (!dicTempDatas2.ContainsKey(item.Key))
                                {
                                    dicTempDatas2.Add(item.Key, item.Value);
                                }

                                if (dicTempDatas2.Count >= displayCondition2)
                                {
                                    double tempPressure = 0;
                                    double tempFlow = 0;
                                    foreach (KeyValuePair<DateTime, List<double>> item2 in dicTempDatas2)
                                    {
                                        tempPressure = tempPressure + item2.Value[0];
                                        tempFlow = tempFlow + item2.Value[1];
                                    }
                                    displayChartData.Add(new CommonFunction.ChartField(0, 0, item.Key, tempPressure / displayCondition2, tempFlow / displayCondition2));
                                    dicTempDatas2.Clear();
                                }
                            }
                        }
                    } 
                }
                 
                if (displayChartData.Count == 0)
                {
                    InitSeries(new DateTime(), new DateTime());
                    List<CommonFunction.ChartField> chartList = new List<CommonFunction.ChartField>();
                    chartList.Add(new CommonFunction.ChartField(0, 0, new DateTime(), 0));
                    curChart.DataSource = chartList; 

                    curChart.ChartAreas[0].AxisY.Maximum = 1;
                    curChart.ChartAreas[0].AxisY.Minimum = 0;

                    curChartFlow.DataSource = chartList; 

                    curChartFlow.ChartAreas[0].AxisY.Maximum = 1;
                    curChartFlow.ChartAreas[0].AxisY.Minimum = 0;
                }
                else
                {
                    InitSeries(displayChartData[0].dtTimeStamp, displayChartData[displayChartData.Count - 1].dtTimeStamp);
                    curChart.DataSource = null;
                    curChartFlow.DataSource = null;
                     
                    curChart.DataSource = displayChartData;

                    if (chartTankID > 0)
                        curChartFlow.DataSource = displayChartData;
                    else
                    {
                        List<CommonFunction.ChartField> chartList = new List<CommonFunction.ChartField>();
                        chartList.Add(new CommonFunction.ChartField(0, 0, new DateTime(), 0));
                        curChartFlow.DataSource = chartList;
                    } 

                    if (displayChartData != null && displayChartData.Count > 0)
                    {
                        double max = Math.Round(displayChartData.Max(p => p.dPressure));
                        if (max == 0 || double.IsPositiveInfinity(max))
                            curChart.ChartAreas[0].AxisY.Maximum = 1;
                        else
                            curChart.ChartAreas[0].AxisY.Maximum = max + 0.5;

                        double minVal = Math.Round(displayChartData.Min(p => p.dPressure)) - 0.5;
                        if (minVal < 0)
                            curChart.ChartAreas[0].AxisY.Minimum = 0;
                        else
                            curChart.ChartAreas[0].AxisY.Minimum = minVal;

                        double max2 = Math.Round(displayChartData.Max(p => p.dFlow));
                        if (max2 == 0 || double.IsPositiveInfinity(max2))
                            curChartFlow.ChartAreas[0].AxisY.Maximum = 1;
                        else
                            curChartFlow.ChartAreas[0].AxisY.Maximum = max2 + 0.5;

                        double minVal2 = Math.Round(displayChartData.Min(p => p.dFlow)) - 0.5; 
                        curChartFlow.ChartAreas[0].AxisY.Minimum = minVal2;
                    }

                    curChart.Customize += curChart_Customize;
                    curChartFlow.Customize += curChartFlow_Customize; 

                    //페이징 처리
                    //페이지 정보 초기화
                    if (dicPageEntity.ContainsKey(nTabIndex))
                    {
                        dicPageEntity[nTabIndex].nMinPage = 1;
                        dicPageEntity[nTabIndex].nMaxPage = 1;
                        dicPageEntity[nTabIndex].nCurPage = 1;
                    }
                    else
                        dicPageEntity.Add(nTabIndex, new PageEntity(nTabIndex, 1, 1, 1));

                    //재조회시 페이지 초기화
                    if (dicPageChart.ContainsKey(nTabIndex))
                        dicPageChart[nTabIndex].Clear();
                    else
                        dicPageChart.Add(nTabIndex, new Dictionary<int, List<CommonFunction.ChartField>>());
                    dicPageChart[nTabIndex].Add(1, displayChartData);

                    string searchDate = displayChartData[0].dtTimeStamp.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " +
                                        displayChartData[displayChartData.Count - 1].dtTimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                    SetPageText(1, 1, searchDate);
                } 
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                UnE.Utility.UMessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        } 

        private void DisplayZoomPipe(DateTime beforeDate, DateTime afterDate)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                List<CommonFunction.ChartField> totalChartData = new List<CommonFunction.ChartField>();
                List<CommonFunction.ChartField> displayChartData = new List<CommonFunction.ChartField>();
                Dictionary<DateTime, List<double>> dicTempDatas = new Dictionary<DateTime, List<double>>();

                foreach (CommonFunction.ChartField item in dicTotalChartData[nTabIndex])
                {
                    if (beforeDate <= item.dtTimeStamp && afterDate >= item.dtTimeStamp)
                        totalChartData.Add(item);
                }

                int displayCondition = MainForm.Instance.commonFunction.GetChartPointCount(beforeDate, afterDate);
                foreach (CommonFunction.ChartField item in totalChartData)
                {
                    if (displayChartData.Count == 0 || displayChartData[displayChartData.Count - 1].dtTimeStamp < item.dtTimeStamp)
                    {
                        if (!dicTempDatas.ContainsKey(item.dtTimeStamp))
                        {
                            List<double> doubles = new List<double>();
                            doubles.Add(item.dPressure);
                            doubles.Add(item.dFlow);
                            dicTempDatas.Add(item.dtTimeStamp, doubles);  
                        }

                        if (dicTempDatas.Count >= displayCondition)
                        {
                            double tempPressure = 0;
                            double tempFlow = 0;
                            foreach (KeyValuePair<DateTime, List<double>> item2 in dicTempDatas)
                            { 
                                tempPressure = tempPressure + item2.Value[0];
                                tempFlow = tempFlow + item2.Value[1];
                            }
                            displayChartData.Add(new CommonFunction.ChartField(0, 0, item.dtTimeStamp, tempPressure / displayCondition, tempFlow / displayCondition));
                            dicTempDatas.Clear();
                        }
                    }
                }

                if (displayChartData.Count <= 10) return;
                else
                {
                    InitSeries(displayChartData[0].dtTimeStamp, displayChartData[displayChartData.Count - 1].dtTimeStamp);

                    curChart.DataSource = displayChartData;
                    curChartFlow.DataSource = displayChartData;

                    //페이징 처리                     
                    //int maxPage = ++dicPageEntity[nTabIndex].nMaxPage;
                    int curPage = ++dicPageEntity[nTabIndex].nCurPage;
                    
                    if (dicPageChart[nTabIndex].ContainsKey(curPage))
                        dicPageChart[nTabIndex][curPage].Clear();
                    dicPageChart[nTabIndex][curPage] = displayChartData;

                    for (int i = dicPageChart[nTabIndex].Count; i > curPage; i--)
                    {
                        dicPageChart[nTabIndex].Remove(i);
                    }

                    dicPageEntity[nTabIndex] = new PageEntity(nTabIndex, 1, dicPageChart[nTabIndex].Count, curPage);

                    string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");

                    SetPageText(curPage, dicPageEntity[nTabIndex].nMaxPage, searchDate);
                }

                curChart.Customize += curChart_Customize;
                curChartFlow.Customize += curChartFlow_Customize;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                UnE.Utility.UMessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region 페이징 버튼이벤트
        /// <summary>
        /// TabIndex, PageNo, ChartData
        /// </summary>
        Dictionary<int, Dictionary<int, List<CommonFunction.ChartField>>> dicPageChart = new Dictionary<int, Dictionary<int, List<CommonFunction.ChartField>>>();
        Dictionary<int, PageEntity> dicPageEntity = new Dictionary<int, PageEntity>();
        private void pictureBox_doubleLeft_Click(object sender, EventArgs e)
        {
            if (!dicPageEntity.ContainsKey(nTabIndex)) return;

            int minPage = dicPageEntity[nTabIndex].nMinPage;
            int maxPage = dicPageEntity[nTabIndex].nMaxPage;
            int curPage = dicPageEntity[nTabIndex].nCurPage;
            if (minPage == maxPage || minPage == curPage) return;

            if (minPage < curPage)
            {
                curPage = minPage;

                DateTime beforeDate = dicPageChart[nTabIndex][curPage][0].dtTimeStamp;
                DateTime afterDate = dicPageChart[nTabIndex][curPage][dicPageChart[nTabIndex][curPage].Count - 1].dtTimeStamp;

                InitSeries(beforeDate, afterDate);
                curChart.DataSource = dicPageChart[nTabIndex][curPage];
                curChartFlow.DataSource = dicPageChart[nTabIndex][curPage];
                dicPageEntity[nTabIndex].nCurPage = curPage;

                string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");
                SetPageText(curPage, dicPageEntity[nTabIndex].nMaxPage, searchDate);

                curChart.Customize += curChart_Customize;
                curChartFlow.Customize += curChartFlow_Customize;

                if (curChart.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    curChart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    curChart.Update();
                }
                if (curChart.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    curChart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    curChart.Update();
                }

                if (curChartFlow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    curChartFlow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    curChartFlow.Update();
                }
                if (curChartFlow.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    curChartFlow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    curChartFlow.Update();
                }  
            }
        }

        private void pictureBox_left_Click(object sender, EventArgs e)
        {
            if (!dicPageEntity.ContainsKey(nTabIndex)) return;

            int minPage = dicPageEntity[nTabIndex].nMinPage;
            int maxPage = dicPageEntity[nTabIndex].nMaxPage;
            int curPage = dicPageEntity[nTabIndex].nCurPage;

            if (curPage > minPage)
            {
                curPage--;

                DateTime beforeDate = dicPageChart[nTabIndex][curPage][0].dtTimeStamp;
                DateTime afterDate = dicPageChart[nTabIndex][curPage][dicPageChart[nTabIndex][curPage].Count - 1].dtTimeStamp;

                InitSeries(beforeDate, afterDate);

                curChart.DataSource = dicPageChart[nTabIndex][curPage];
                curChartFlow.DataSource = dicPageChart[nTabIndex][curPage];
                dicPageEntity[nTabIndex].nCurPage = curPage;

                string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");
                SetPageText(curPage, dicPageEntity[nTabIndex].nMaxPage, searchDate);

                curChart.Customize += curChart_Customize;
                curChartFlow.Customize += curChartFlow_Customize;

                if (curChart.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    curChart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    curChart.Update();
                }
                if (curChart.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    curChart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    curChart.Update();
                }

                if (curChartFlow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    curChartFlow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    curChartFlow.Update();
                }
                if (curChartFlow.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    curChartFlow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    curChartFlow.Update();
                }  
            }
        }

        private void pictureBox_right_Click(object sender, EventArgs e)
        {
            if (!dicPageEntity.ContainsKey(nTabIndex)) return;

            int minPage = dicPageEntity[nTabIndex].nMinPage;
            int maxPage = dicPageEntity[nTabIndex].nMaxPage;
            int curPage = dicPageEntity[nTabIndex].nCurPage;

            if (curPage < maxPage)
            {
                curPage++;

                DateTime beforeDate = dicPageChart[nTabIndex][curPage][0].dtTimeStamp;
                DateTime afterDate = dicPageChart[nTabIndex][curPage][dicPageChart[nTabIndex][curPage].Count - 1].dtTimeStamp;

                InitSeries(beforeDate, afterDate);
                curChart.DataSource = dicPageChart[nTabIndex][curPage];
                curChartFlow.DataSource = dicPageChart[nTabIndex][curPage];
                dicPageEntity[nTabIndex].nCurPage = curPage;

                string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");
                SetPageText(curPage, dicPageEntity[nTabIndex].nMaxPage, searchDate);

                curChart.Customize += curChart_Customize;
                curChartFlow.Customize += curChartFlow_Customize;

                if (curChart.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    curChart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    curChart.Update();
                }
                if (curChart.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    curChart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    curChart.Update();
                }

                if (curChartFlow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    curChartFlow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    curChartFlow.Update();
                }
                if (curChartFlow.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    curChartFlow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    curChartFlow.Update();
                }  
            }
        }

        private void pictureBox_doubleRight_Click(object sender, EventArgs e)
        {
            if (!dicPageEntity.ContainsKey(nTabIndex)) return;

            int minPage = dicPageEntity[nTabIndex].nMinPage;
            int maxPage = dicPageEntity[nTabIndex].nMaxPage;
            int curPage = dicPageEntity[nTabIndex].nCurPage;

            if (minPage == maxPage || maxPage == curPage) return;

            if (maxPage > curPage)
            {
                curPage = maxPage;

                DateTime beforeDate = dicPageChart[nTabIndex][curPage][0].dtTimeStamp;
                DateTime afterDate = dicPageChart[nTabIndex][curPage][dicPageChart[nTabIndex][curPage].Count - 1].dtTimeStamp;

                InitSeries(beforeDate, afterDate);
                curChart.DataSource = dicPageChart[nTabIndex][curPage];
                curChartFlow.DataSource = dicPageChart[nTabIndex][curPage];
                dicPageEntity[nTabIndex].nCurPage = curPage;

                string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");
                SetPageText(curPage, dicPageEntity[nTabIndex].nMaxPage, searchDate);

                curChart.Customize += curChart_Customize;
                curChartFlow.Customize += curChartFlow_Customize;

                if (curChart.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    curChart.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    curChart.Update();
                }
                if (curChart.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    curChart.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    curChart.Update();
                }

                if (curChartFlow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    curChartFlow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    curChartFlow.Update();
                }
                if (curChartFlow.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    curChartFlow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    curChartFlow.Update();
                }  
            }
        }
        private void SetPageText(int curPage, int maxPage, string searchDate)
        {
            textBox1.Text = curPage.ToString();
            label_maxPage.Text = string.Format("/{0}", maxPage);
            label_searchDate.Text = searchDate;
        }
        #endregion

        #region 배관탭 이동 버튼 이벤트
        private void panel_move_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            PipeTabMove();
        }

        private void label2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            PipeTabMove();
        }

        private void PipeTabMove()
        {
            if (!dicPageEntity.ContainsKey(nTabIndex)) return;

            int minPage = dicPageEntity[nTabIndex].nMinPage;
            int maxPage = dicPageEntity[nTabIndex].nMaxPage;
            int curPage = dicPageEntity[nTabIndex].nCurPage;

            try
            {
                if (textBox1.Text == curPage.ToString()) return;

                int a;
                if (!int.TryParse(textBox1.Text, out a)) throw new ApplicationException("숫자형식으로 입력하세요.");
                if (a >= minPage && a <= maxPage && a != curPage)
                {
                    curPage = a;

                    DateTime beforeDate = dicPageChart[nTabIndex][curPage][0].dtTimeStamp;
                    DateTime afterDate = dicPageChart[nTabIndex][curPage][dicPageChart[nTabIndex][curPage].Count - 1].dtTimeStamp;

                    InitSeries(beforeDate, afterDate);

                    curChart.DataSource = dicPageChart[nTabIndex][curPage];
                    dicPageEntity[nTabIndex].nCurPage = curPage;

                    string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    SetPageText(curPage, dicPageEntity[nTabIndex].nMaxPage, searchDate);

                    curChart.Customize += curChart_Customize;
                }
                else throw new ApplicationException("범위내의 페이지 번호를 입력하세요.");
            }
            catch (ApplicationException app)
            {
                Cursor = Cursors.Default;
                UnE.Utility.UMessageBox.Show(app.Message);
                textBox1.Text = curPage.ToString();
            }
        }
        #endregion

        #region 조회 버튼 이벤트
        private void pictureBox_search_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            try
            {
                DateTime searchBeforeDate = new DateTime(dateTimePicker_date1.Value.Year, dateTimePicker_date1.Value.Month, dateTimePicker_date1.Value.Day,
                                                       dateTimePicker_time1.Value.Hour, dateTimePicker_time1.Value.Minute, dateTimePicker_time1.Value.Second);
                DateTime searchAfterDate = new DateTime(dateTimePicker_date2.Value.Year, dateTimePicker_date2.Value.Month, dateTimePicker_date2.Value.Day,
                                                  dateTimePicker_time2.Value.Hour, dateTimePicker_time2.Value.Minute, dateTimePicker_time2.Value.Second);

                if (searchBeforeDate > searchAfterDate) throw new ApplicationException("이전 날짜가 이후 날짜보다 클 수 없습니다.");

                CommonFunction.PipeInfo selectedItem = (CommonFunction.PipeInfo)comboBox_pipeList.SelectedItem;
                int nSelectedPipeId = Convert.ToInt32(selectedItem.nPipeID);

                if (nTabIndex == 0) //Total 
                    DisplayTotal();
                else if (nTabIndex == 12) //알람이력 
                    DisplayAlarmHistory(nSelectedPipeId);
                else if (nTabIndex == 13) //작업이력
                    DisplayWorkHistory(nSelectedPipeId);
                else
                {
                    DisplayPipeAlarmWorkHistroy();
                    if (chartTankID < 0) // comboBox_chartPipeList_SelectedIndexChanged 이벤트로 조회하기때문에 또 조회할 필요 없음
                        DisplayPipe(); 
                }
            }
            catch (ApplicationException app)
            {
                UnE.Utility.UMessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                UnE.Utility.UMessageBox.Show(ex.Message);
            }
        }
        #endregion           
    }

    public class PageEntity
    {
        public int nTabIndex { get; set; }
        public int nMinPage { get; set; }
        public int nMaxPage { get; set; }
        public int nCurPage { get; set; }

        public PageEntity(int tabIndex, int minPage, int maxPage, int curPage)
        {
            this.nTabIndex = tabIndex;
            this.nMinPage = minPage;
            this.nMaxPage = maxPage;
            this.nCurPage = curPage;
        }
    }
}
