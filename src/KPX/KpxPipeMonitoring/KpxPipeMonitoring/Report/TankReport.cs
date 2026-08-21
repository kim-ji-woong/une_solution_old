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

namespace KpxPipeMonitoring.Report
{
    public partial class TankReport : Form
    { 
        Timer timer = null;
        bool panelDown = true;

        private string date1 { get { return dateTimePicker_date1.Value.ToString("yyyyMMdd"); } }
        private string date2 { get { return dateTimePicker_date2.Value.ToString("yyyyMMdd"); } }
        private string time1 { get { return dateTimePicker_time1.Value.ToString("HHmmss"); } }
        private string time2 { get { return dateTimePicker_time2.Value.ToString("HHmmss"); } }

        private int nTankID = -1; // 현재 조회하는 탱크ID
        private int nOldTankID = -1; // 직전에 조회했던 탱크ID 

        private HistoryManager m_historyMgr = null; 

        public TankReport()
        {
            InitializeComponent();
             
            m_historyMgr = new HistoryManager(MainForm.Instance);

            comboBox_chartPipeList.SelectedIndexChanged += comboBox_chartPipeList_SelectedIndexChanged;
            comboBox_chartPipeList.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox_chartPipeList.DisplayMember = "strPipeName";
            comboBox_chartPipeList.ValueMember = "nPipeID";
            
            comboBox_tankList.SelectedIndexChanged += comboBox_tankList_SelectedIndexChanged;
            comboBox_tankList.Visible = false;
            comboBox_tankList.DropDownStyle = ComboBoxStyle.DropDownList;

            panel_total.BackgroundImage = tempImg;
            tempReportClickBtn = panel_total;
            tempReportClickBtn.Tag = label_total.Text;

            this.dateTimePicker_date1.Value = DateTime.Now.AddDays(-7);
            this.dateTimePicker_time1.Value = DateTime.Now.AddDays(-7);
            this.dateTimePicker_time1.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker_time1.CustomFormat = "HH:mm:ss";

            this.dateTimePicker_time2.Format = DateTimePickerFormat.Custom;
            this.dateTimePicker_time2.CustomFormat = "HH:mm:ss";

            this.dateTimePicker_date1.Enabled = false;
            this.dateTimePicker_date2.Enabled = false;
            this.dateTimePicker_time1.Enabled = false;
            this.dateTimePicker_time2.Enabled = false;

            this.comboBox_tankList.Visible = false;
            this.pictureBox_doubleLeft.Visible = false;
            this.pictureBox_left.Visible = false;
            this.textBox1.Visible = false;
            this.label_maxPage.Visible = false;
            this.pictureBox_right.Visible = false;
            this.pictureBox_doubleRight.Visible = false;
            this.panel_move.Visible = false;
            this.label2.Visible = false;
            this.label_searchDate.Visible = false;
            this.comboBox_chartPipeList.Visible = false;
             
            if (MainForm.bExcelInstalled)
                panel_printReport.Visible = true;
            else
                panel_printReport.Visible = false;

            Color colHeaderBackground = Color.FromArgb(87, 168, 250);

            MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "TankName", "탱크명", colHeaderBackground, 190);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "PipeName", "연결된 배관", colHeaderBackground, 220);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "AvgFlow", "평균 유량\r\n(kl/h)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "MaxFlow", "최고 유량\r\n(kl/h)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "MinFlow", "최소 유량\r\n(kl/h)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "AvgPressure", "평균 압력\r\n(kg/cm²)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "MaxPressure", "최고 압력\r\n(kg/cm²)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "MinPressure", "최소 압력\r\n(kg/cm²)", colHeaderBackground, 140);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "RecentWorkTime", "직전 작업 시간", colHeaderBackground, 500);
            MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "Status", "상태", colHeaderBackground, 100);
            dataGridView_1.ColumnHeadersHeight = 55;

            dataGridView_1.Location = new Point(5, 115);
            dataGridView_1.Size = new Size(1910, 793);
             
            dataGridView_1.Visible = true; 
            chart1.Visible = false;
            chart1Flow.Visible = false;

            MainForm.Instance.SetDoubleBuffer(dataGridView_1, true);

            InitChart();
            DisplayPipeList();
            DisplayTotal();

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

        #region 콤보 이벤트
        void comboBox_tankList_SelectedIndexChanged(object sender, EventArgs e)
        {
            CommonFunction.TankInfo selectedItem = (CommonFunction.TankInfo)comboBox_tankList.SelectedItem;
            int nSelectedTankId = Convert.ToInt32(selectedItem.nTankID);

            if (nSelectedTankId < 0) return;

            int nTabNameIndex = tempReportClickBtn.Name.IndexOf("_") + 1;
            string strTabName = tempReportClickBtn.Name.Substring(nTabNameIndex);

            int nName = -1;
            int.TryParse(strTabName, out nName); 

            if (nName <= 0 && strTabName.ToUpper() == "ALARMHISTORY") 
                DisplayAlarmHistory(nSelectedTankId);
            else if (nName <= 0 && strTabName.ToUpper() == "WORKHISTORY") 
                DisplayWorkHistory(nSelectedTankId);
        } 
        #endregion

        #region 버튼 세팅
        void timer_Tick(object sender, EventArgs e)
        {
            if (panelDown)
            {
                if (panel_btns.Height >= 203) timer.Enabled = false;
                else panel_btns.Height += 10; 
            }
            else
            {
                if (panel_btns.Height <= 83) timer.Enabled = false;
                else panel_btns.Height -= 10; 
            }
        } 

        private void DisplayPipeList()
        { 
            SettingPanelImage(panel_total, label_total, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_alarmHistory, label_alarmHistory, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_workHistory, label_workHistory, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
            SettingPanelImage(panel_printReport, label_pringReport, global::KpxPipeMonitoring.Properties.Resources.ReportPrintButton_Normal, global::KpxPipeMonitoring.Properties.Resources.ReportPrintButton_Click);

            comboBox_tankList.ValueMember = "nTankID";
            comboBox_tankList.DisplayMember = "strTankName";
            comboBox_tankList.Items.Add(new CommonFunction.TankInfo(0, "전체 탱크", "", 0, 0, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, 0, false, new List<int>(), new List<string>(), false, 0, 0, 0, false, false));
            comboBox_tankList.SelectedIndex = 0;

            foreach (CommonFunction.TankInfo tank in MainForm.Instance.tankInfo)
            {
                foreach (Control ctrl in panel_btns.Controls)
                {
                    if (ctrl is Panel)
                    {
                        Panel panel = ctrl as Panel;
                        if (panel.Controls == null || panel.Controls.Count == 0 || panel.Name != "panel_" + tank.nTankID) continue;
                        if (panel.Controls[0] is Label)
                        {
                            Label label = panel.Controls[0] as Label; 
                            if (label.Name == "label_" + tank.nTankID)
                            {
                                label.Text = "TK-" + tank.strTankName;
                                comboBox_tankList.Items.Add(new CommonFunction.TankInfo(tank.nTankID, tank.strTankName + " " + tank.strType, "", 0, 0, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, 0, false, new List<int>(), new List<string>(), false, 0, 0, 0, false, false));
                                SettingPanelImage(panel, label, KpxPipeMonitoring.Properties.Resources.ReportButton_Normal, KpxPipeMonitoring.Properties.Resources.ReportButton_Click);
                                break;
                            } 
                        }
                    }
                }
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

                    ChangeDisplay();
                }
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

                    ChangeDisplay();
                }
            };
        }
         
        private void ChangeDisplay()
        {
            if (tempReportClickBtn == null)
            {
                label_selectItem.Text = "선택 : ";
                return;
            }

            if (nOldTankID >= 1 && nOldTankID <= 22)
            {
                if (dicTotalChartData.ContainsKey(nOldTankID))
                {
                    dicTotalChartData[nOldTankID].Clear();
                    dicTotalChartData.Remove(nOldTankID);
                    GC.Collect();
                }
            }

            if (chart1 != null && chart1.DataSource != null)
            {
                List<CommonFunction.ChartField> chartList = (List<CommonFunction.ChartField>)chart1.DataSource;
                if (chartList != null)
                {
                    chartList.Clear();
                    chart1.DataSource = null;
                    GC.Collect();
                }               
            }
            if (chart1Flow != null && chart1Flow.DataSource != null)
            {
                List<CommonFunction.ChartField> chartList = (List<CommonFunction.ChartField>)chart1Flow.DataSource;
                if (chartList != null)
                {
                    chartList.Clear();
                    chart1Flow.DataSource = null;
                    GC.Collect();
                }
            }

            if (chart1.ChartAreas[0].AxisX.ScaleView.IsZoomed)
            {
                chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                chart1.Update();
            }
            if (chart1.ChartAreas[0].AxisY.ScaleView.IsZoomed)
            {
                chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                chart1.Update();
            }

            if (chart1Flow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
            {
                chart1Flow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                chart1Flow.Update();
            }
            if (chart1Flow.ChartAreas[0].AxisY.ScaleView.IsZoomed)
            {
                chart1Flow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                chart1Flow.Update();
            }  

            Color colHeaderBackground = Color.FromArgb(87, 168, 250);

            label_selectItem.Text = "선택 : " + tempReportClickBtn.Tag.ToString();

            int nIndex = tempReportClickBtn.Name.IndexOf("_") + 1;
            string strName = tempReportClickBtn.Name.Substring(nIndex);

            dataGridView_1.Columns.Clear();

            int nName = -1;
            int.TryParse(strName, out nName);
            if (nName <= 0)
            { 
                // TOTAL, ALARMHISTORY, WORKHISTORY
                dataGridView_1.Location = new Point(5, 115);
                dataGridView_1.Size = new Size(1910, 793);

                dataGridView_1.Visible = true;
                chart1.Visible = false;
                chart1Flow.Visible = false;

                dataGridView_1.MouseDoubleClick -= gridView_MouseDoubleClick;
                dataGridView_1.CellPainting -= gridView_CellPainting;

                if (strName.ToUpper() == "TOTAL")
                {
                    nTankID = -1;
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "TankName", "탱크명", colHeaderBackground, 190);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "PipeName", "연결된 배관", colHeaderBackground, 220);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "AvgFlow", "평균 유량\r\n(kl/h)", colHeaderBackground, 140);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "MaxFlow", "최고 유량\r\n(kl/h)", colHeaderBackground, 140);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "MinFlow", "최소 유량\r\n(kl/h)", colHeaderBackground, 140);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "AvgPressure", "평균 압력\r\n(kg/cm²)", colHeaderBackground, 140);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "MaxPressure", "최고 압력\r\n(kg/cm²)", colHeaderBackground, 140);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "MinPressure", "최소 압력\r\n(kg/cm²)", colHeaderBackground, 140);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "RecentWorkTime", "직전 작업 시간", colHeaderBackground, 500);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "Status", "상태", colHeaderBackground, 100);
                    dataGridView_1.ColumnHeadersHeight = 55;

                    DisplayTotal();

                    dateTimePicker_date1.Enabled = false;
                    dateTimePicker_date2.Enabled = false;
                    dateTimePicker_time1.Enabled = false;
                    dateTimePicker_time2.Enabled = false;
                    comboBox_tankList.Visible = false;
                }
                else if (strName.ToUpper() == "ALARMHISTORY")
                {
                    nTankID = -2;
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "Num", "No", colHeaderBackground, 40);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "TankId", "탱크ID", colHeaderBackground);
                    dataGridView_1.Columns["TankId"].Visible = false;
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "HistoryID", "이력ID", colHeaderBackground);
                    dataGridView_1.Columns["HistoryID"].Visible = false;
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "TankName", "탱크명", colHeaderBackground, 80);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "PipeName", "연결된 배관", colHeaderBackground, 140);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "BeginTime", "발생 시간", colHeaderBackground, 230);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "EndTime", "종료 시간", colHeaderBackground, 230);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "AlarmTime", "지속 시간", colHeaderBackground, 130);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "StandardPressure", "발생시 기준치", colHeaderBackground, 170);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "AlarmPressure", "발생시 데이터", colHeaderBackground, 160);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "Status", "발생시 상태", colHeaderBackground, 150);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "Terminator", "종료 계정", colHeaderBackground, 130);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "AlarmOccurrence", "발생 유형", colHeaderBackground, 200);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "AlarmComment", "해결 내용", colHeaderBackground, 220);

                    CommonFunction.TankInfo selectedItem = (CommonFunction.TankInfo)comboBox_tankList.SelectedItem;
                    int nSelectedTankId = Convert.ToInt32(selectedItem.nTankID);

                    DisplayAlarmHistory(nSelectedTankId);

                    dateTimePicker_date1.Enabled = true;
                    dateTimePicker_date2.Enabled = true;
                    dateTimePicker_time1.Enabled = true;
                    dateTimePicker_time2.Enabled = true;
                    comboBox_tankList.Visible = true;
                }
                else if (strName.ToUpper() == "WORKHISTORY")
                {
                    nTankID = -3;
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "Num", "No", colHeaderBackground, 26);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "TankId", "탱크ID", colHeaderBackground);
                    dataGridView_1.Columns["TankId"].Visible = false;
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "HistoryID", "이력ID", colHeaderBackground);
                    dataGridView_1.Columns["HistoryID"].Visible = false;
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "TankName", "탱크명", colHeaderBackground, 160);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "PipeName", "연결된 배관", colHeaderBackground, 220);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "BeginTime", "시작 시간", colHeaderBackground, 280);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "EndTime", "종료 시간", colHeaderBackground, 280);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "CTime", "지속 시간", colHeaderBackground, 180);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "IgnoreCTime", "알람 무시 시간", colHeaderBackground, 250);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "BeginUserName", "시작 계정", colHeaderBackground, 200);
                    MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "EndUserId", "종료 계정", colHeaderBackground, 200);

                    CommonFunction.TankInfo selectedItem = (CommonFunction.TankInfo)comboBox_tankList.SelectedItem;
                    int nSelectedTankId = Convert.ToInt32(selectedItem.nTankID);

                    DisplayWorkHistory(nSelectedTankId);

                    dateTimePicker_date1.Enabled = true;
                    dateTimePicker_date2.Enabled = true;
                    dateTimePicker_time1.Enabled = true;
                    dateTimePicker_time2.Enabled = true;
                    comboBox_tankList.Visible = true;
                } 

                pictureBox_doubleLeft.Visible = false;
                pictureBox_doubleRight.Visible = false;
                pictureBox_left.Visible = false;
                pictureBox_right.Visible = false;
                comboBox_chartPipeList.Visible = false;
                textBox1.Visible = false;
                label_maxPage.Visible = false;
                label_searchDate.Visible = false;
                panel_move.Visible = false;
                label2.Visible = false;
            }
            else
            { 
                dataGridView_1.Location = new Point(1152, 115);
                dataGridView_1.Size = new Size(773, 793);
                chart1Flow.Location = new Point(5, 115);
                chart1Flow.Size = new System.Drawing.Size(1147, 397);
                chart1.Location = new Point(5, 512);
                chart1.Size = new System.Drawing.Size(1147, 397);
                 
                dataGridView_1.Visible = true;
                chart1.Visible = true;
                chart1Flow.Visible = true;

                nTankID = nName;

                MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "Num", "No", colHeaderBackground, 50);
                MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "PipeName", "연결된 배관", colHeaderBackground, 120);
                MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "ID", "알람이력ID", colHeaderBackground);
                dataGridView_1.Columns["ID"].Visible = false;
                MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "TankID", "탱크ID", colHeaderBackground);
                dataGridView_1.Columns["TankID"].Visible = false;
                MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "SubBeginTime", "시작시간", colHeaderBackground);
                dataGridView_1.Columns["SubBeginTime"].Visible = false;
                MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "BeginTime", "시작시간", colHeaderBackground, 245);
                MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "EndTime", "종료시간", colHeaderBackground, 245);
                MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "Status", "상태", colHeaderBackground, 100);
                MainForm.Instance.commonFunction.SettingGridView(dataGridView_1, "Type", "타입", colHeaderBackground);
                dataGridView_1.Columns["Type"].Visible = false;
                dataGridView_1.Columns["BeginTime"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView_1.MouseDoubleClick += gridView_MouseDoubleClick;
                dataGridView_1.CellPainting += gridView_CellPainting;

                dateTimePicker_date1.Enabled = true;
                dateTimePicker_date2.Enabled = true;
                dateTimePicker_time1.Enabled = true;
                dateTimePicker_time2.Enabled = true;

                pictureBox_doubleLeft.Visible = true;
                pictureBox_doubleRight.Visible = true;
                pictureBox_left.Visible = true;
                pictureBox_right.Visible = true;
                comboBox_chartPipeList.Visible = true;
                textBox1.Visible = true;
                label_maxPage.Visible = true;
                label_searchDate.Visible = true;
                panel_move.Visible = true;
                label2.Visible = true;
                comboBox_tankList.Visible = false;

                DisplayTankAlarmWorkHistory();
                if (chartPipeID < 0) // comboBox_chartPipeList_SelectedIndexChanged 이벤트로 조회하기때문에 또 조회할 필요 없음
                    DisplayTank();

                chart1Flow.Focus();                                 
            }
            nOldTankID = nTankID;
        }
        #endregion 

        #region 차트 세팅
        private void InitChart()
        { 
            //압력
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisY.Interval = 0;
            chart1.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "F1";
            chart1.Series.Clear();
            Series series = chart1.Series.Add("series1");
            series.ChartType = SeriesChartType.Line;
            chart1.Series[0].IsXValueIndexed = true;
            chart1.Series[0].XValueMember = "dtTimeStamp";
            chart1.Series[0].YValueMembers = "dPressure";
            chart1.Series[0].BorderWidth = 3;
            chart1.Series[0].Color = Color.Transparent;
            chart1.Legends.Clear();

            chart1.Series[0].IsXValueIndexed = true;
            chart1.MouseDown += chart_MouseDown;
            chart1.MouseMove += chart_MouseMove;
            chart1.MouseUp += chart_MouseUp;
            chart1.MouseLeave += chart_MouseLeave;
            chart1.MouseWheel += chart1_MouseWheel;

            chart1.ChartAreas[0].Position.Auto = false;
            chart1.ChartAreas[0].Position.X = 0;
            chart1.ChartAreas[0].Position.Y = 20;
            chart1.ChartAreas[0].Position.Width = 96;
            chart1.ChartAreas[0].Position.Height = 90;

            chart1.ChartAreas[0].AxisX.ScrollBar.LineColor = Color.White; 
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonColor = Color.White;
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;

            chart1.ChartAreas[0].AxisY.ScrollBar.LineColor = Color.White; 
            chart1.ChartAreas[0].AxisY.ScrollBar.ButtonColor = Color.White; 
            chart1.ChartAreas[0].AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll; 

            // 유량
            chart1Flow.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            chart1Flow.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart1Flow.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart1Flow.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart1Flow.ChartAreas[0].AxisY.Interval = 0;
            chart1Flow.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart1Flow.ChartAreas[0].AxisY.LabelStyle.Format = "F1";

            chart1Flow.Series.Clear();
            Series series2 = chart1Flow.Series.Add("series1");
            series2.ChartType = SeriesChartType.Line;
            chart1Flow.Series[0].IsXValueIndexed = true;
            chart1Flow.Series[0].XValueMember = "dtTimeStamp";
            chart1Flow.Series[0].YValueMembers = "dFlow";
            chart1Flow.Series[0].BorderWidth = 3;
            chart1Flow.Series[0].Color = Color.FromArgb(194, 198, 191);
            chart1Flow.Legends.Clear();

            chart1Flow.Series[0].IsXValueIndexed = true;
            chart1Flow.MouseDown += chartFlow_MouseDown;
            chart1Flow.MouseMove += chartFlow_MouseMove;
            chart1Flow.MouseUp += chartFlow_MouseUp;
            chart1Flow.MouseLeave += chartFlow_MouseLeave; 
            chart1Flow.MouseWheel += chart1Flow_MouseWheel;
            
            chart1Flow.ChartAreas[0].Position.Auto = false;
            chart1Flow.ChartAreas[0].Position.X = 0;
            chart1Flow.ChartAreas[0].Position.Y = 20;
            chart1Flow.ChartAreas[0].Position.Width = 96;
            chart1Flow.ChartAreas[0].Position.Height = 90;

            chart1Flow.ChartAreas[0].AxisX.ScrollBar.LineColor = Color.White; 
            chart1Flow.ChartAreas[0].AxisX.ScrollBar.ButtonColor = Color.White;
            chart1Flow.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            
            chart1Flow.ChartAreas[0].AxisY.ScrollBar.LineColor = Color.White; 
            chart1Flow.ChartAreas[0].AxisY.ScrollBar.ButtonColor = Color.White;
            chart1Flow.ChartAreas[0].AxisY.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll; 
        }
         
        private void InitSeries(DateTime beforeDate, DateTime afterDate)
        { 
            string dateFormat = "HH:mm";

            DateTimeIntervalType IntervalType = MainForm.Instance.commonFunction.GetIntervalType(beforeDate, afterDate); 
            if (IntervalType == DateTimeIntervalType.Seconds)
                dateFormat = "HH:mm:ss";
            else if (IntervalType == DateTimeIntervalType.Minutes)
                dateFormat = "HH:mm:ss";
            else if (IntervalType == DateTimeIntervalType.Hours)
                dateFormat = "MM/dd\r\nHH:mm";
            else
                dateFormat = "MM/dd\r\nHH:mm";

            chart1.Series[0].XValueType = ChartValueType.DateTime;
            //chart1.Series[1].ToolTip = "#VALX{" + dateFormat + "} - #VALY1{0.00}"; 
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = dateFormat;

            chart1Flow.Series[0].XValueType = ChartValueType.DateTime;
            //chart1Flow.Series[1].ToolTip = "#VALX{" + dateFormat + "} - #VALY1{0.00}";
            chart1Flow.ChartAreas[0].AxisX.LabelStyle.Format = dateFormat;
        }
        #endregion

        #region 차트 이벤트
        Point mDown = Point.Empty;
        Graphics g;
        Pen pen = new Pen(Brushes.Red);
        void chart_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Location.X <= 72) return;
            if (e.Location.Y >= 336) return;

            mDown = e.Location; 
        }
        void chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (chart1 == null) return;

            chart1.Focus();

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (mDown.X == 0 && mDown.Y == 0) return;

                chart1.Refresh();

                using (g = chart1.CreateGraphics())
                {
                    g.DrawRectangle(Pens.Red, GetRectangle(mDown, e.Location));
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.None)
            {
                chart1.Refresh();

                chart1.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);

                int curPosition = (int)chart1.ChartAreas[0].CursorX.Position;
                if (chart1.ChartAreas[0].CursorX.Position < 0) return; 
                if (chart1.Series[0].Points.Count <= 1) return;
                if (chart1.Series[0].Points.Count < curPosition - 1) return;
                if (chart1.Series[0].Points[curPosition - 1] == null) return;

                using (g = chart1.CreateGraphics())
                {
                    string content = DateTime.FromOADate(chart1.Series[0].Points[curPosition - 1].XValue).ToString("yyyy-MM-dd HH:mm:ss") +
                        "\r\n압력:" + String.Format("{0:F2}", chart1.Series[0].Points[curPosition - 1].YValues[0]);

                    if (comboBox_chartPipeList.Location.X < e.X - 70 + 140)
                    {
                        g.DrawRectangle(pen, e.X - 145, chart1.ChartAreas[0].Position.Y + 50, 140, 30);
                        g.DrawString(content, new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)))
                            , Brushes.Red
                            , new PointF(e.X - 140, chart1.ChartAreas[0].Position.Y + 53)
                            );
                    }
                    else
                    {
                        g.DrawRectangle(pen, e.X - 70, chart1.ChartAreas[0].Position.Y - 2, 140, 30);
                        g.DrawString(content, new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)))
                            , Brushes.Red
                            , new PointF(e.X - 65, chart1.ChartAreas[0].Position.Y)
                            );
                    }
                } 
            }
        }
        void chart_MouseUp(object sender, MouseEventArgs e)
        {
            if (chart1 == null) return;
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
                HitTestResult result = chart1.HitTest(item.X, item.Y);
                if (result.PointIndex != -1 && !ints1.Contains(result.PointIndex))
                    ints1.Add(result.PointIndex);
            }

            List<int> ints2 = new List<int>();
            foreach (Point item in point2)
            {
                HitTestResult result = chart1.HitTest(item.X, item.Y);
                if (result.PointIndex != -1 && !ints2.Contains(result.PointIndex))
                    ints2.Add(result.PointIndex);
            }

            double xValue1 = -1;
            foreach (int dd in ints1)
            {
                if (xValue1 < 0 || xValue1 < chart1.Series[0].Points[dd].XValue)
                    xValue1 = chart1.Series[0].Points[dd].XValue;
            }
            double xValue2 = -1;
            foreach (int dd in ints2)
            {
                if (xValue2 < chart1.Series[0].Points[dd].XValue)
                    xValue2 = chart1.Series[0].Points[dd].XValue;
            }

            DateTime beforeDate = DateTime.FromOADate(xValue1);
            DateTime afterDate = DateTime.FromOADate(xValue2);

            DateTime beforeDatePicker = new DateTime(dateTimePicker_date1.Value.Year, dateTimePicker_date1.Value.Month, dateTimePicker_date1.Value.Day,
                                                   dateTimePicker_time1.Value.Hour, dateTimePicker_time1.Value.Minute, dateTimePicker_time1.Value.Second);
            DateTime afterDatePicker = new DateTime(dateTimePicker_date2.Value.Year, dateTimePicker_date2.Value.Month, dateTimePicker_date2.Value.Day,
                                                  dateTimePicker_time2.Value.Hour, dateTimePicker_time2.Value.Minute, dateTimePicker_time2.Value.Second);

            chart1.Refresh();

            mDown = Point.Empty; 
            if (xValue1 <= 0 || xValue2 <= 0 || beforeDate < beforeDatePicker || afterDate > afterDatePicker || (afterDate - beforeDate).TotalMinutes <= 1)
                return;

            DisplayZoomPipe(beforeDate, afterDate);
        }
        void chart_MouseLeave(object sender, EventArgs e)
        {
            if (chart1 == null) return;

            chart1.Refresh();
            chart1.ChartAreas[0].CursorX.Position = 0;
        }
        static public Rectangle GetRectangle(Point p1, Point p2)
        {
            return new Rectangle(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y),
                Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));
        }

        void chart1_Customize(object sender, EventArgs e)
        {
            chart1.Customize -= chart1_Customize; 
            if (!dicWorkDate.ContainsKey(nTankID)) return;

            int index = 0;
            List<CommonFunction.WorkListField> workList = dicWorkDate[nTankID];
            foreach (CommonFunction.WorkListField item in workList)
            {
                for (int i = index; i < chart1.Series[0].Points.Count; i++)
                {
                    double curWorkTime = DateTime.FromOADate(chart1.Series[0].Points[i].XValue).ToOADate(); 
                    if ((item.dBeginTime <= curWorkTime && item.dEndTime >= curWorkTime) || (item.dBeginTime <= curWorkTime && item.dEndTime == 0))
                    {
                        chart1.Series[0].Points[i].Color = Color.FromArgb(48, 129, 209); // 파랑
                        chart1.Series[0].Points[i].BorderWidth = 4;
                    }
                }
            } 
        }
        void chart1_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0)
                {
                    double xMin = chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double yMin = chart1.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    double yMax = chart1.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = (chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + xMin) / 2;
                    double posXFinish = (chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + xMax) / 2;
                    double posYStart = (chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + yMin) / 2;
                    double posYFinish = (chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + yMax) / 2;

                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    chart1.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
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
            if (e.Location.Y >= 336) return;

            mDownFlow = e.Location; 
        }
        void chartFlow_MouseMove(object sender, MouseEventArgs e)
        {
            if (chart1Flow == null) return;

            chart1Flow.Focus();            

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (mDownFlow.X == 0) return;

                chart1Flow.Refresh();

                using (g = chart1Flow.CreateGraphics())
                {
                    g.DrawRectangle(Pens.Red, GetRectangle(mDownFlow, e.Location));
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.None)
            {
                chart1Flow.Refresh();

                chart1Flow.ChartAreas[0].CursorX.SetCursorPixelPosition(new Point(e.X, e.Y), true);

                int curPosition = (int)chart1Flow.ChartAreas[0].CursorX.Position;
                if (chart1Flow.ChartAreas[0].CursorX.Position < 0) return; 
                if (chart1Flow.Series[0].Points.Count <= 1) return;
                if (chart1Flow.Series[0].Points.Count < curPosition - 1) return;
                if (chart1Flow.Series[0].Points[curPosition - 1] == null) return;

                using (g = chart1Flow.CreateGraphics())
                {
                    string content = DateTime.FromOADate(chart1Flow.Series[0].Points[curPosition - 1].XValue).ToString("yyyy-MM-dd HH:mm:ss") +
                        "\r\n유량:" + String.Format("{0:F2}", chart1Flow.Series[0].Points[curPosition - 1].YValues[0]);

                    g.DrawRectangle(pen, e.X - 5 - 65, chart1Flow.ChartAreas[0].Position.Y - 2, 140, 30);
                    g.DrawString(content, new System.Drawing.Font("나눔바른고딕", 8.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)))
                        , Brushes.Red
                        , new PointF(e.X - 65, chart1Flow.ChartAreas[0].Position.Y)
                        );
                }
            }
        }
        void chartFlow_MouseUp(object sender, MouseEventArgs e)
        {
            if (chart1Flow == null) return;
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
                HitTestResult result = chart1Flow.HitTest(item.X, item.Y);
                if (result.PointIndex != -1 && !ints1.Contains(result.PointIndex))
                    ints1.Add(result.PointIndex);
            }

            List<int> ints2 = new List<int>();
            foreach (Point item in point2)
            {
                HitTestResult result = chart1Flow.HitTest(item.X, item.Y);
                if (result.PointIndex != -1 && !ints2.Contains(result.PointIndex))
                    ints2.Add(result.PointIndex);
            }

            double xValue1 = -1;
            foreach (int dd in ints1)
            {
                if (xValue1 < 0 || xValue1 < chart1Flow.Series[0].Points[dd].XValue)
                    xValue1 = chart1Flow.Series[0].Points[dd].XValue;
            }
            double xValue2 = -1;
            foreach (int dd in ints2)
            {
                if (xValue2 < chart1Flow.Series[0].Points[dd].XValue)
                    xValue2 = chart1Flow.Series[0].Points[dd].XValue;
            }

            DateTime beforeDate = DateTime.FromOADate(xValue1);
            DateTime afterDate = DateTime.FromOADate(xValue2);

            DateTime beforeDatePicker = new DateTime(dateTimePicker_date1.Value.Year, dateTimePicker_date1.Value.Month, dateTimePicker_date1.Value.Day,
                                                   dateTimePicker_time1.Value.Hour, dateTimePicker_time1.Value.Minute, dateTimePicker_time1.Value.Second);
            DateTime afterDatePicker = new DateTime(dateTimePicker_date2.Value.Year, dateTimePicker_date2.Value.Month, dateTimePicker_date2.Value.Day,
                                                  dateTimePicker_time2.Value.Hour, dateTimePicker_time2.Value.Minute, dateTimePicker_time2.Value.Second);

            chart1Flow.Refresh();
            mDownFlow = Point.Empty; 

            if (xValue1 <= 0 || xValue2 <= 0 || beforeDate < beforeDatePicker || afterDate > afterDatePicker || (afterDate - beforeDate).TotalMinutes <= 1)
                return;

            DisplayZoomPipe(beforeDate, afterDate);
        }
        void chartFlow_MouseLeave(object sender, EventArgs e)
        {
            if (chart1Flow == null) return;
            chart1Flow.Refresh();
            chart1Flow.ChartAreas[0].CursorX.Position = 0;
        }
        void chart1Flow_Customize(object sender, EventArgs e)
        {
            chart1Flow.Customize -= chart1Flow_Customize;
            System.Diagnostics.Trace.WriteLine("customize begin " + DateTime.Now);
            if (!dicWorkDate.ContainsKey(nTankID)) return;

            int index = 0;
            List<CommonFunction.WorkListField> workList = dicWorkDate[nTankID];
            foreach (CommonFunction.WorkListField item in workList)
            {
                for (int i = index; i < chart1Flow.Series[0].Points.Count; i++)
                {
                    double curWorkTime = DateTime.FromOADate(chart1Flow.Series[0].Points[i].XValue).ToOADate();
                    //DateTime dtBeginTime = DateTime.FromOADate(item.dBeginTime);
                    if ((item.dBeginTime <= curWorkTime && item.dEndTime >= curWorkTime) || (item.dBeginTime <= curWorkTime && item.dEndTime == 0))
                    {
                        chart1Flow.Series[0].Points[i].Color = Color.FromArgb(255, 137, 0); // 파랑
                        chart1Flow.Series[0].Points[i].BorderWidth = 4;
                    }
                }
            } 
        }
        void chart1Flow_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta < 0)
                { 
                    chart1Flow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1Flow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                }
                else if (e.Delta > 0)
                {
                    double xMin = chart1Flow.ChartAreas[0].AxisX.ScaleView.ViewMinimum;
                    double xMax = chart1Flow.ChartAreas[0].AxisX.ScaleView.ViewMaximum;
                    double yMin = chart1Flow.ChartAreas[0].AxisY.ScaleView.ViewMinimum;
                    double yMax = chart1Flow.ChartAreas[0].AxisY.ScaleView.ViewMaximum;

                    double posXStart = (chart1Flow.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + xMin) / 2;
                    double posXFinish = (chart1Flow.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X) + xMax) / 2;
                    double posYStart = (chart1Flow.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + yMin) / 2;
                    double posYFinish = (chart1Flow.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y) + yMax) / 2;

                    chart1Flow.ChartAreas[0].AxisX.ScaleView.Zoom(posXStart, posXFinish);
                    chart1Flow.ChartAreas[0].AxisY.ScaleView.Zoom(posYStart, posYFinish);
                }
            }
            catch { }
        }
        #endregion 

        #region 조회
        private void DisplayTotal()
        {
            dataGridView_1.Rows.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.ID as tankId, Concat(Name, ' ', Type) as tankName, AvgFlow, MaxFlow, MinFlow, Concat(BeginTime, '~', ifnull(EndTime, ' ')) as BeginTime,  ");
            sb.Append("       CASE  ");
		    sb.Append("           WHEN Status = 0 ");
            sb.Append("           THEN CASE ");
            sb.Append("                WHEN LiquidType='황산' AND ((IsLeakStatus=1 AND IsLeakMonitoring=0) OR (IsLeakStatus=1 AND IsLeakMonitoring=1)) ");
			sb.Append("                THEN '누출감지' ");
            sb.Append("                WHEN LiquidType='황산' AND IsLeakMonitoring=0 ");
			sb.Append("                THEN '통신불능' ");
			sb.Append("                ELSE '정상' ");
			sb.Append("                END ");
            sb.Append("           ELSE CASE  ");
			sb.Append("                WHEN (select Description from AlarmType where id=t.Status) IS NULL ");
            sb.Append("                   THEN '다중알람' ");
            sb.Append("                   ELSE CASE  ");
            sb.Append("		                WHEN LiquidType='황산' AND ((IsLeakStatus=1 AND IsLeakMonitoring=0) OR (IsLeakStatus=1 AND IsLeakMonitoring=1)) ");
			sb.Append("		                THEN '다중알람' ");
            sb.Append("		                WHEN LiquidType='황산' AND IsLeakMonitoring=0 ");
			sb.Append("		                THEN '다중알람' ");
            sb.Append("                           ELSE (select Description from AlarmType where id=t.Status) ");
			sb.Append("                     END  ");
			sb.Append("                END      ");    
	        sb.Append("          END as Status, ");
            sb.Append("       CASE WHEN AnotherLink = -200 THEN '황산' WHEN AnotherLink = -100 THEN 'PO'  ELSE (select Concat(Name, ' ', Type) from pipe as p where p.id=lwh.pipeid) END as pipeName,  ");
            sb.Append("       AvgPressure, MaxPressure, MinPressure ");
            sb.Append("  FROM Tank as t LEFT OUTER JOIN (select * from lastworkhistory as lwh where begintime = ");
			sb.Append("  					case  ");
            sb.Append("                        when (select max(begintime) from lastworkhistory as lwh2 where endtime is null and lwh2.tankid=lwh.tankid) is null ");
            sb.Append("                        then (select max(begintime) from lastworkhistory as lwh2 where lwh2.tankid=lwh.tankid) ");
            sb.Append("                        else (select max(begintime) from lastworkhistory as lwh2 where endtime is null and lwh2.tankid=lwh.tankid)  ");
			sb.Append("  				     end ");
			sb.Append("  				   ) as lwh ON t.id=lwh.tankid ");
            sb.Append(" ORDER BY tankName");

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult == null) return;

            for (int i = 0; i < arrResult.Count; i += 11)
            {
                int nTankID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTankName = (arrResult[i + 1].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                double dAvgFlow = (arrResult[i + 2].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 2]);
                double dMaxFlow = (arrResult[i + 3].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 3]);
                double dMinFlow = (arrResult[i + 4].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 4]);
                string strRecentBeginTime = (arrResult[i + 5].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 5]);
                
                string strStatus = (arrResult[i + 6].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 6]);

                string strPipeName = (arrResult[i + 7].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 7]);
                double dAvgPressure = (arrResult[i + 8].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 8]);
                double dMaxPressure = (arrResult[i + 9].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 9]);
                double dMinPressure = (arrResult[i + 10].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 10]);

                string strAvgPressure = dAvgPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dAvgPressure));
                string strMaxPressure = dMaxPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dMaxPressure));
                string strMinPressure = dMinPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dMaxPressure));

                string strAvgFlow = dAvgPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dAvgFlow));
                string strMaxFlow = dMaxPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dMaxFlow));
                string strMinFlow = dMinPressure == -1 ? "-" : MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F1}", dMinFlow));

                dataGridView_1.Rows.Add(strTankName, strPipeName, strAvgFlow, strMaxFlow, strMinFlow, strAvgPressure, strMaxPressure, strMinPressure, strRecentBeginTime, strStatus);
            }
        }

        private void DisplayAlarmHistory(int tankID = 0)
        {
            dataGridView_1.Rows.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.ID as TankID, ah.ID as HistoryID, Concat(Name, ' ', Type) as Name, BeginTime, (select EndTime from AlarmHistory as ah2 where ah2.id=ah.id) as endtime,  ");
            sb.Append("       Concat(SEC_TO_TIME(TIME_TO_SEC(timediff((select EndTime from AlarmHistory as ah2 where ah2.id=ah.id), BeginTime))),'') as AlarmTime, RealValue, ");
            sb.Append("       (select description from alarmType as at where at.id=ah.AlarmType) as AlarmType,  ");
            sb.Append("       (select CASE WHEN Mobile = 0 THEN UserName WHEN Mobile = 1 THEN concat('모바일(', UserName, ')') END from user where id=ah.alarmterminator) as Terminator  ");
            sb.Append("     , case  ");
            sb.Append("         when alarmtype in (256,512) "); // 압력상승, 압력하강
            sb.Append("         then (select Concat(Name, ' ', Type) from pipe as p where p.id=(select pipeid from alarmhistory as ah2 where ah2.id=ah.id)) ");
            sb.Append("         else '-' ");
            sb.Append("        end as pipename ");
            sb.Append("       , StandardValue, StandardRange, ah.alarmOccurType, ah.alarmComment ");
            sb.Append("  FROM tank as t INNER JOIN ");
            sb.Append("       (select max(id) as id, tankid, begintime, AlarmType, AlarmTerminator, StandardValue, StandardRange, RealValue, alarmOccurType, alarmComment ");
		    sb.Append("          from AlarmHistory ");
            sb.Append("         group by tankid, begintime, AlarmType, AlarmTerminator, StandardValue, StandardRange, RealValue, alarmOccurType, alarmComment ");
	        sb.Append("       ) as ah ON t.id=ah.tankID ");
            sb.AppendFormat("   WHERE date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}'", date1, time1, date2, time2);
            if (tankID > 0)
                sb.Append(" AND t.id = " + tankID);
            sb.Append(" UNION ALL ");
            sb.Append(" SELECT t.Id as TankID, Concat('-',tlh.ID) as HistoryID, Concat(Name, ' ', Type) as TankName, BeginTime, EndTime, ");
            sb.Append("        Concat(SEC_TO_TIME(TIME_TO_SEC(timediff((select EndTime from TankLeakHistory as tlh2 where tlh2.id=tlh.id), BeginTime))),'') as AlarmTime, ");
            sb.Append("        '-1', '황산 누출',  ");
            sb.Append("        (select CASE WHEN Mobile = 0 THEN UserName WHEN Mobile = 1 THEN concat('모바일(', UserName, ')') END from user where id=tlh.alarmterminator) as Terminator, ");
            sb.Append("        '-',-999,-999, alarmOccurType, alarmComment  ");
            sb.Append("   FROM TankLeakHistory as tlh INNER JOIN Tank as t ON tlh.TankID=t.ID ");
            sb.AppendFormat("   WHERE date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}'", date1, time1, date2, time2);
            if (tankID > 0)
                sb.Append(" AND t.id = " + tankID);
            sb.Append(" ORDER BY BeginTime DESC ");

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult == null) return;

            int nNum = 1;
            for (int i = 0; i < arrResult.Count; i += 14)
            {
                int row = 0;
                int nTankID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[i + ++row].ToString(), -1);
                string strTankName = DBUtility.WebDBManager.GetStringField(arrResult[i + ++row]);
                string strBeginTime = (arrResult[i + ++row].ToString() == "null") ? "-" : Convert.ToDateTime(arrResult[i + row]).ToString("yyyy-MM-dd HH:mm:ss").Replace(".0", "");
                string strEndTime = (arrResult[i + ++row].ToString() == "null") ? "-" : Convert.ToDateTime(arrResult[i + row]).ToString("yyyy-MM-dd HH:mm:ss").Replace(".0", "");
                string strAlarmTime = (arrResult[i + ++row].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + row]);
                double dAlarmPressure = (arrResult[i + ++row].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + row]);
                string strStatus = (arrResult[i + ++row].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + row]);
                string strAlarmUnit = "";
                if (strStatus.Contains("압력"))
                    strAlarmUnit = " kg/cm²";
                else if (strStatus.Contains("레벨"))
                    strAlarmUnit = " m";
                else if (strStatus.Contains("유량"))
                    strAlarmUnit = " kl/h";
                else if (strStatus.Contains("온도"))
                    strAlarmUnit = " ℃";
                string strTerminator = (arrResult[i + ++row].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + row]);
                string strPipeName = (arrResult[i + ++row].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + row]);

                string strAlarmPressure = "-";
                if (nHistoryID > 0)
                    strAlarmPressure = MainForm.Instance.commonFunction.removeTailZero(string.Format("{0:F2}", dAlarmPressure)) + strAlarmUnit; 
                double nStandardValue = (arrResult[i + ++row].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + row]);
                double nStandardRange = (arrResult[i + ++row].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + row]);

                string strStandardValue = "-";
                if (nStandardValue != -9999 && nStandardValue != -999)
                {
                    if (strStatus.Contains("레벨") || strStatus.Contains("온도"))
                    {
                        strStandardValue = String.Format("{0:F2}", nStandardValue);
                    }
                    else
                    {
                        if (nStandardValue - nStandardRange == 0 && nStandardValue + nStandardRange == 0)
                            strStandardValue = "0 ~ 0";
                        else
                        {
                            double a = nStandardValue - nStandardRange;
                            double b = nStandardValue + nStandardRange;

                            if (a <= b)
                                strStandardValue = String.Format("{0:F2}", a) + " ~ " + String.Format("{0:F2}", b);
                            else
                                strStandardValue = String.Format("{0:F2}", b) + " ~ " + String.Format("{0:F2}", a);
                        }
                    }
                }

                int nOccurType = DBUtility.WebDBManager.GetIntField(arrResult[i + ++row].ToString(), -1);
                string strOccurType = "-";
                if (nOccurType != -1)
                    strOccurType = KpxPipeMonitoring.Popups.AlarmClear.occurenceTypeString[nOccurType];
                string strComment = (arrResult[i + ++row].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + row]);

                if (nTankID < 0) continue;
                dataGridView_1.Rows.Add(nNum, nTankID, nHistoryID, strTankName, strPipeName, strBeginTime, strEndTime, strAlarmTime, strStandardValue, strAlarmPressure, strStatus, strTerminator, strOccurType, strComment);
                nNum++;
            }
        }

        private void DisplayWorkHistory(int tankID = 0)
        {
            dataGridView_1.Rows.Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT t.id as TankID, wh.id as HistoryID, Concat(Name, ' ', Type) as TankName, BeginTime, EndTime,   ");
            sb.Append("       Concat(SEC_TO_TIME(TIME_TO_SEC(timediff(IFNULL(EndTime, now()), BeginTime))),'') as CTime,  ");
            sb.Append("       IFNULL((select ignoreTime from AlarmIgnoreHistory as aih where aih.workhistoryid=wh.id and aih.ignorebegintime=wh.begintime),'15분') as ignoreTime,  ");
            sb.Append("       CASE  ");
            sb.Append("           WHEN (select concat(date_format(IgnoreBeginTime,'%H:%i:%s'),'~',date_format(IgnoreEndTime,'%H:%i:%s')) from AlarmIgnoreHistory as aih where aih.workhistoryid=wh.id and aih.ignorebegintime=wh.begintime) IS NULL  ");
            sb.Append("           THEN Concat(date_format(BeginTime,'%H:%i:%s'),'~',date_format(date_add(BeginTime, interval 15 minute),'%H:%i:%s'))  ");
            sb.Append("           ELSE (select concat(date_format(IgnoreBeginTime,'%H:%i:%s'),'~',date_format(IgnoreEndTime,'%H:%i:%s')) from AlarmIgnoreHistory as aih where aih.workhistoryid=wh.id and aih.ignorebegintime=wh.begintime)  ");
            sb.Append("        END as ignoreCTime,  ");
            sb.Append("        CASE  ");
            sb.Append("           WHEN BeginCmdHistoryID = -1 THEN '자동시작'  ");
            sb.Append("           ELSE (select case when mobile=0 then username when mobile=1 then concat('모바일(', username, ')') end ");
            sb.Append("                  from user where id = (select userid from commandhistory where id = wh.begincmdhistoryid)) ");
            sb.Append("       END as beginUserName,  ");
            sb.Append("       CASE  ");
            sb.Append("           WHEN EndCmdHistoryID = -1 THEN '자동종료'    ");
            sb.Append("           WHEN EndCmdHistoryID = -2 THEN '작업중'  ");
            sb.Append("           ELSE (select case when mobile=0 then username when mobile=1 then concat('모바일(', username, ')') end  ");
            sb.Append("           from user where id = (select userid from commandhistory where id = wh.EndCmdHistoryID))  ");
            sb.Append("       END as EndUserName  ");
            sb.Append("       , CASE WHEN AnotherLink = -200 THEN '황산' WHEN AnotherLink = -100 THEN 'PO'  ELSE (select concat(Name, ' ', Type) from pipe as p where p.id=wh.pipeid) END as PipeName  ");
            sb.Append("  FROM tank as t INNER JOIN WorkHistory as wh ON t.ID=wh.TankID ");
            //sb.AppendFormat("WHERE date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}'", date1, time1, date2, time2);
            sb.AppendFormat("WHERE (date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}')", date1, time1, date2, time2);
            if (tankID > 0)
                sb.Append(" AND t.id = " + tankID);
            sb.Append(" ORDER BY BeginTime DESC  ");

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult == null) return;
             
            int nNum = 1;
            for (int i = 0; i < arrResult.Count; i += 11)
            {
                int nTankID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                long nHistoryID = Convert.ToInt64(arrResult[i + 1].ToString());
                string strTankName = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);                
                string strBeginTime = (arrResult[i + 3].ToString() == "null") ? "-" : Convert.ToDateTime(arrResult[i + 3]).ToString("yyyy-MM-dd HH:mm:ss").Replace(".0", "");
                string strEndTime = (arrResult[i + 4].ToString() == "null") ? "-" : Convert.ToDateTime(arrResult[i + 4]).ToString("yyyy-MM-dd HH:mm:ss").Replace(".0", "");
                string strCTime = (arrResult[i + 5].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 5]);
                string strIgnoreTime = (arrResult[i + 6].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 6]);
                string strIgnoreCTime = (arrResult[i + 7].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 7]);
                string strBeginUserName = (arrResult[i + 8].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 8]);
                string strEndUserName = (arrResult[i + 9].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 9]);
                string strPipeName = (arrResult[i + 10].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 10]);

                if (nTankID < 0) continue;
                dataGridView_1.Rows.Add(nNum, nTankID, nHistoryID, strTankName, strPipeName, strBeginTime, strEndTime, strCTime, strIgnoreCTime, strBeginUserName, strEndUserName);
                nNum++;
            }
        } 
        #endregion

        #region 탱크 조회
        private int chartPipeID = -1;
        private void DisplayTankConnectedPipes(List<CommonFunction.WorkListField> workList)
        {
            if (comboBox_chartPipeList.Items != null)
                comboBox_chartPipeList.Items.Clear();
            chartPipeID = -1;

            if (workList == null)
            {
                comboBox_chartPipeList.Visible = false;
                return;
            }

            Dictionary<int, CommonFunction.WorkListField> works = new Dictionary<int, CommonFunction.WorkListField>();

            foreach (CommonFunction.WorkListField item in workList)
            {
                if (item.nPipeID < 0) 
                    continue;
                if (!works.ContainsKey(item.nPipeID))
                    works.Add(item.nPipeID, item); 
                else
                {
                    // workList는 최근 작업이력 순으로 들어옴
                    if (works[item.nPipeID].dBeginTime > item.dBeginTime)
                        works[item.nPipeID].dBeginTime = item.dBeginTime;
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
                chartPipeID = -1;
                return;
            }

            chartPipeID = work.nPipeID;
            DisplayTank();
        } 

        private void DisplayTankAlarmWorkHistory()
        {
            #region 알람, 작업 이력
            dataGridView_1.Rows.Clear();

            if (dicWorkDate.ContainsKey(nTankID))
                dicWorkDate[nTankID].Clear();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT Max(ID) as ID, TankID, BeginTime, (select EndTime from AlarmHistory as ah2 where ah2.id=ah.id) as EndTime ");
            sb.Append("     , (select Description from AlarmType where id=ah.AlarmType) as Status, 0 as Type ");
            sb.Append("     , case   ");
            sb.Append("          when alarmtype in (256,512)  ");
            sb.Append("          then (select ID from pipe as p where p.id=(select pipeid from alarmhistory as ah2 where ah2.id=ah.id))  ");
            sb.Append("          else '-1'  ");
            sb.Append("       end as PipeId  ");
            sb.Append("     , case  ");
            sb.Append("         when alarmtype in (256,512) "); // 압력상승, 압력하강
            sb.Append("         then (select Concat(Name, ' ', Type) from pipe as p where p.id=(select pipeid from alarmhistory as ah2 where ah2.id=ah.id)) ");
            sb.Append("         else '-' ");
            sb.Append("        end as pipename ");
            sb.Append("     , (Select Concat(Name, ' ', Type) from tank as t where t.id=ah.tankId) as TankName ");
            sb.Append("  FROM AlarmHistory ah  ");
            sb.Append(" WHERE TankID = " + nTankID);
            sb.AppendFormat(" AND date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}'", date1, time1, date2, time2);
            sb.Append(" GROUP BY TankID, BeginTime, AlarmType ");
            sb.Append(" UNION ALL ");
            // 황산은 History ID 음수
            sb.Append("SELECT Concat('-', ID) as ID, TankID, BeginTime, EndTime, '황산 누출', 0 as Type, -1 as PipeId, '-' as PipeName ");
            sb.Append("     , (Select Concat(Name, ' ', Type) from tank as t where t.id=tlh.tankId) as TankName ");
            sb.Append("  FROM TankLeakHistory as tlh  ");
            sb.Append(" WHERE TankID = " + nTankID);
            sb.AppendFormat(" AND date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}'", date1, time1, date2, time2);
            sb.Append(" UNION ALL ");
            sb.Append("SELECT ID, TankID, ");
            sb.Append("       CONCAT(BeginTime, '/', ");
            sb.Append("             CASE ");
            sb.Append("               WHEN (select concat(date_format(IgnoreBeginTime,'%H:%i:%s'),'~',date_format(IgnoreEndTime,'%H:%i:%s')) from AlarmIgnoreHistory as aih where aih.workhistoryid=wh.id and aih.ignorebegintime=wh.begintime) IS NULL ");
            sb.Append("               THEN Concat(date_format(BeginTime,'%H:%i:%s'),'~',date_format(date_add(BeginTime, interval 15 minute),'%H:%i:%s')) ");
            sb.Append("               ELSE (select concat(date_format(IgnoreBeginTime,'%H:%i:%s'),'~',date_format(IgnoreEndTime,'%H:%i:%s')) from AlarmIgnoreHistory as aih where aih.workhistoryid=wh.id and aih.ignorebegintime=wh.begintime) ");
            sb.Append("             END) as begintime, ");
            sb.Append("       EndTime, CASE WHEN EndTime IS NULL THEN '작업중' WHEN EndTime IS NOT NULL THEN '작업종료' END as status, 1 as Type ");
            sb.Append("     , CASE WHEN AnotherLink = -200 OR AnotherLink = -100 THEN -1  ELSE (select PipeId from pipe as p where p.id=wh.pipeid) END as PipeId ");
            sb.Append("     , CASE WHEN AnotherLink = -200 THEN '황산' WHEN AnotherLink = -100 THEN 'PO'  ELSE (select Concat(Name, ' ', Type) from pipe as p where p.id=wh.pipeid) END as PipeName ");
            sb.Append("     , (Select Concat(Name, ' ', Type) from tank as t where t.id=wh.tankId) as TankName ");
            sb.Append("  FROM WorkHistory as wh ");
            sb.Append(" WHERE TankId = " + nTankID);
            sb.AppendFormat(" AND (date_format(BeginTime, '%Y%m%d%H%i%s') between '{0}{1}' and '{2}{3}')", date1, time1, date2, time2);
            sb.Append(" ORDER BY BeginTime DESC");

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult == null) return;

            Font font = new System.Drawing.Font("나눔바른고딕", 15f);
            List<CommonFunction.WorkListField> workList = new List<CommonFunction.WorkListField>();

            int nNum = 1;
            for (int i = 0; i < arrResult.Count; i += 9)
            {
                long nID = Convert.ToInt64(arrResult[i].ToString());
                int nGridViewTankID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strBeginTime = DBUtility.WebDBManager.GetStringField(arrResult[i + 2]);
                string strEndTime = (arrResult[i + 3].ToString() == "null") ? "-" : Convert.ToDateTime(arrResult[i + 3]).ToString("yyyy-MM-dd HH:mm:ss").Replace(".0", "");
                string strStatus = DBUtility.WebDBManager.GetStringField(arrResult[i + 4]);
                int nType = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nPipeId = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                string strPipeName = (arrResult[i + 7].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 7]).Replace("PT-", "");
                string strTankName = DBUtility.WebDBManager.GetStringField(arrResult[i + 8]);

                if (nGridViewTankID < 0) continue;

                dataGridView_1.Rows.Add(nNum, strPipeName, nID, nGridViewTankID, strBeginTime, "", strEndTime, strStatus, nType);
                dataGridView_1.Rows[nNum - 1].DefaultCellStyle.Font = font;
                if (nType == 0)
                    dataGridView_1.Rows[nNum - 1].DefaultCellStyle.ForeColor = Color.Red;
                else
                {
                    DBUtility.VariousData<DateTime> dtBeginTime = new DBUtility.VariousData<DateTime>();
                    dtBeginTime.Data = Convert.ToDateTime(strBeginTime.Substring(0, strBeginTime.IndexOf('/')));

                    double dEndTime = 0;
                    if (strEndTime != "-")
                        dEndTime = Convert.ToDateTime(strEndTime).ToOADate();
                    workList.Add(new CommonFunction.WorkListField(nPipeId, nTankID, strPipeName, strTankName, dtBeginTime.Data.ToOADate(), dEndTime));
                }
                nNum++;
            }

            if (!dicWorkDate.ContainsKey(nTankID))
                dicWorkDate.Add(nTankID, workList);
            else
                dicWorkDate[nTankID] = workList;

            #endregion

            DisplayTankConnectedPipes(workList);
        }
        SortedList<int, List<CommonFunction.ChartField>> dicTotalChartData = new SortedList<int, List<CommonFunction.ChartField>>();
        SortedList<int, List<CommonFunction.WorkListField>> dicWorkDate = new SortedList<int, List<CommonFunction.WorkListField>>();
        private void DisplayTank()
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {   
                if (dicTotalChartData.ContainsKey(nTankID))
                    dicTotalChartData[nTankID].Clear(); 

                List<CommonFunction.ChartField> totalChartData = new List<CommonFunction.ChartField>();                
                List<CommonFunction.ChartField> displayChartData = new List<CommonFunction.ChartField>();
                List<CommonFunction.ChartField> displayPressureChartData = new List<CommonFunction.ChartField>();
                //Dictionary<DateTime, double> dicTempDatas = new Dictionary<DateTime, double>();
                Dictionary<DateTime, List<double>> dicTempDatas = new Dictionary<DateTime, List<double>>();

                DateTime beforeDate = new DateTime(dateTimePicker_date1.Value.Year, dateTimePicker_date1.Value.Month, dateTimePicker_date1.Value.Day,
                                                   dateTimePicker_time1.Value.Hour, dateTimePicker_time1.Value.Minute, dateTimePicker_time1.Value.Second);
                DateTime afterDate = new DateTime(dateTimePicker_date2.Value.Year, dateTimePicker_date2.Value.Month, dateTimePicker_date2.Value.Day,
                                                  dateTimePicker_time2.Value.Hour, dateTimePicker_time2.Value.Minute, dateTimePicker_time2.Value.Second);

                System.Diagnostics.Trace.WriteLine("pipe history query begin " + DateTime.Now);

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

                //    HistoryQuery query = new HistoryQuery(nTankID, y, m, d, HistoryQueryType.유량);
                //    historyQueries.Add(query);
                //}

                int f = 0;
                while (true)
                {
                    DateTime date = beforeDate.AddDays(f);
                    string y = date.Year.ToString();
                    string m = date.Month.ToString();
                    string d = date.Day.ToString();

                    HistoryQuery query = new HistoryQuery(nTankID, y, m, d, HistoryQueryType.유량);
                    historyQueries.Add(query);
                    if (date > afterDate)
                        break;

                    f++;
                }

                totalChartData = m_historyMgr.ReadHistory(historyQueries);
                historyQueries.Clear();
                historyQueries = null;
                 
                if (!dicTotalChartData.ContainsKey(nTankID))
                    dicTotalChartData.Add(nTankID, totalChartData);
                else
                    dicTotalChartData[nTankID] = totalChartData;
                #endregion

                System.Diagnostics.Trace.WriteLine("pipe history query end " + DateTime.Now);

                foreach (CommonFunction.ChartField item in totalChartData)
                { 
                    //if (item.nPipeID != 0 && item.nPipeID != chartPipeID) continue;
                    
                    if (item.dtTimeStamp >= beforeDate && item.dtTimeStamp <= afterDate)
                    {
                        // 첫번째 데이터는 무조건 넣기
                        if (displayChartData.Count == 0)
                        {
                            displayChartData.Add(new CommonFunction.ChartField(item.nPipeID, item.nTankID, item.dtTimeStamp, item.dPressure, item.dFlow)); 
                        }
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

                System.Diagnostics.Trace.WriteLine("pipe chart data " + DateTime.Now);

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
                    chart1.DataSource = chartList;

                    //chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    //chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

                    chart1.ChartAreas[0].AxisY.Maximum = 1;
                    chart1.ChartAreas[0].AxisY.Minimum = 0;

                    chart1Flow.DataSource = chartList;

                    //chart1Flow.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    //chart1Flow.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

                    chart1Flow.ChartAreas[0].AxisY.Maximum = 1;
                    chart1Flow.ChartAreas[0].AxisY.Minimum = 0;
                }
                else
                {
                    InitSeries(displayChartData[0].dtTimeStamp, displayChartData[displayChartData.Count - 1].dtTimeStamp);
                    chart1.DataSource = null;
                    chart1Flow.DataSource = null;

                    if (chartPipeID > 0)
                        chart1.DataSource = displayChartData;
                    else
                    {
                        List<CommonFunction.ChartField> chartList = new List<CommonFunction.ChartField>();
                        chartList.Add(new CommonFunction.ChartField(0, 0, new DateTime(), 0));
                        chart1.DataSource = chartList;
                    }
                    chart1Flow.DataSource = displayChartData;

                    if (displayChartData != null && displayChartData.Count > 0)
                    {
                        double max = Math.Round(displayChartData.Max(p => p.dPressure));
                        if (max == 0 || double.IsPositiveInfinity(max))
                            chart1.ChartAreas[0].AxisY.Maximum = 1;
                        else
                            chart1.ChartAreas[0].AxisY.Maximum = max + 0.5;

                        double minVal = Math.Round(displayChartData.Min(p => p.dPressure)) - 0.5;
                        if (minVal < 0)
                            chart1.ChartAreas[0].AxisY.Minimum = 0;
                        else
                            chart1.ChartAreas[0].AxisY.Minimum = minVal;

                        double max2 = Math.Round(displayChartData.Max(p => p.dFlow));
                        if (max2 == 0 || double.IsPositiveInfinity(max2))
                            chart1Flow.ChartAreas[0].AxisY.Maximum = 1;
                        else
                            chart1Flow.ChartAreas[0].AxisY.Maximum = max2 + 0.5;

                        double minVal2 = Math.Round(displayChartData.Min(p => p.dFlow)) - 0.5;
                        chart1Flow.ChartAreas[0].AxisY.Minimum = minVal2;
                    }

                    chart1.Customize += chart1_Customize;
                    chart1Flow.Customize += chart1Flow_Customize;

                    //페이징 처리
                    //페이지 정보 초기화
                    if (dicPageEntity.ContainsKey(nTankID))
                    {
                        dicPageEntity[nTankID].nMinPage = 1;
                        dicPageEntity[nTankID].nMaxPage = 1;
                        dicPageEntity[nTankID].nCurPage = 1;
                    }
                    else
                        dicPageEntity.Add(nTankID, new PageEntity(nTankID, 1, 1, 1));

                    //재조회시 페이지 초기화
                    if (dicPageChart.ContainsKey(nTankID))
                        dicPageChart[nTankID].Clear();
                    else
                        dicPageChart.Add(nTankID, new Dictionary<int, List<CommonFunction.ChartField>>());
                    dicPageChart[nTankID].Add(1, displayChartData);

                    string searchDate = displayChartData[0].dtTimeStamp.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " +
                                        displayChartData[displayChartData.Count - 1].dtTimeStamp.ToString("yyyy-MM-dd HH:mm:ss");
                    SetPageText(1, 1, searchDate);
                }

                System.Diagnostics.Trace.WriteLine("pipe chart binding " + DateTime.Now); 
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

                foreach (CommonFunction.ChartField item in dicTotalChartData[nTankID])
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

                if (displayChartData.Count <= 1) return;
                else
                {
                    InitSeries(displayChartData[0].dtTimeStamp, displayChartData[displayChartData.Count - 1].dtTimeStamp);

                    chart1.DataSource = displayChartData;
                    chart1Flow.DataSource = displayChartData;

                    //페이징 처리                      
                    int curPage = ++dicPageEntity[nTankID].nCurPage;

                    if (dicPageChart[nTankID].ContainsKey(curPage))
                        dicPageChart[nTankID][curPage].Clear();
                    dicPageChart[nTankID][curPage] = displayChartData;

                    for (int i = dicPageChart[nTankID].Count; i > curPage; i--)
                    {
                        dicPageChart[nTankID].Remove(i);
                    }

                    dicPageEntity[nTankID] = new PageEntity(nTankID, 1, dicPageChart[nTankID].Count, curPage);

                    string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");

                    SetPageText(curPage, dicPageEntity[nTankID].nMaxPage, searchDate);
                }

                chart1.Customize += chart1_Customize;
                chart1Flow.Customize += chart1Flow_Customize;
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

        #region 그리드 이벤트
        Image tempImg = global::KpxPipeMonitoring.Properties.Resources.ReportButton_Click;
        Image tempImg2 = global::KpxPipeMonitoring.Properties.Resources.ReportButton_Normal;
        void gridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.nTankID <= 0) return;
            if (dataGridView_1.CurrentRow == null) return;
            long nHistoryId = Convert.ToInt64(dataGridView_1.CurrentRow.Cells["ID"].Value);
            int nCurRowTankId = Convert.ToInt32(dataGridView_1.CurrentRow.Cells["TankID"].Value);
            int nType = Convert.ToInt32(dataGridView_1.CurrentRow.Cells["Type"].Value);
            if (nCurRowTankId < 1) return;

            tempReportClickBtn.BackgroundImage = tempImg2;

            comboBox_tankList.SelectedIndexChanged -= comboBox_tankList_SelectedIndexChanged;
            if (nType == 0)
            { 
                panel_alarmHistory.BackgroundImage = tempImg;
                tempReportClickBtn = panel_alarmHistory;
                tempReportClickBtn.Tag = label_alarmHistory.Text;

                ChangeDisplay();

                DisplayAlarmHistory(nCurRowTankId);

                foreach (DataGridViewRow item in dataGridView_1.Rows)
                {
                    if (Convert.ToInt64(item.Cells["HistoryID"].Value) == nHistoryId)
                    {
                        dataGridView_1.Rows[item.Index].Selected = true;
                        dataGridView_1.FirstDisplayedScrollingRowIndex = item.Index;
                        break;
                    }
                }
                for (int i = 0; i < comboBox_tankList.Items.Count; i++)
                {
                    CommonFunction.TankInfo item = comboBox_tankList.Items[i] as CommonFunction.TankInfo;
                    if (item.nTankID == nCurRowTankId)
                    {
                        comboBox_tankList.SelectedIndex = i;
                        break;
                    }
                } 
            }
            else if (nType == 1)
            { 
                panel_workHistory.BackgroundImage = tempImg;
                tempReportClickBtn = panel_workHistory;
                tempReportClickBtn.Tag = label_workHistory.Text;

                ChangeDisplay();

                DisplayWorkHistory(nCurRowTankId);

                int nSelectedGridRowIndex = -1;

                foreach (DataGridViewRow item in dataGridView_1.Rows)
                {
                    if (Convert.ToInt64(item.Cells["HistoryID"].Value) == nHistoryId)
                    {
                        dataGridView_1.Rows[item.Index].Selected = true;
                        dataGridView_1.FirstDisplayedScrollingRowIndex = item.Index;
                        nSelectedGridRowIndex = item.Index;
                        break;
                    }
                }
                for (int i = 0; i < comboBox_tankList.Items.Count; i++)
                {
                    CommonFunction.TankInfo item = comboBox_tankList.Items[i] as CommonFunction.TankInfo;
                    if (item.nTankID == nCurRowTankId)
                    {
                        comboBox_tankList.SelectedIndex = i;
                        break;
                    }
                }

                // comboBox_pipeList.SelectedIndexChanged 이벤트에 의하여 의도하지 않게 바뀐
                // dataGridView_1의 선택행을 다시 설정한다.
                if (nSelectedGridRowIndex >= 0)
                    dataGridView_1.Rows[nSelectedGridRowIndex].Selected = true;                 
            }

            comboBox_tankList.SelectedIndexChanged += comboBox_tankList_SelectedIndexChanged;

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
            label2.Visible = false;
            comboBox_tankList.Visible = true;
        }

        void gridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (this.nTankID <= 0) return;

            DataGridView gridView = sender as DataGridView;
            if (gridView == null) return;
            if (gridView.Columns[e.ColumnIndex].Name != "BeginTime") return;
            if (e.RowIndex < 0) return;
            if (gridView.Rows[e.RowIndex].Cells["SubBeginTime"] == null) return;

            string colData = gridView.Rows[e.RowIndex].Cells["SubBeginTime"].Value.ToString();
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
                int nTabNameIndex = tempReportClickBtn.Name.IndexOf("_") + 1;
                string strTabName = tempReportClickBtn.Name.Substring(nTabNameIndex);

                int nName = -1;
                int.TryParse(strTabName, out nName); 

                if (nTankID >= 1)
                {
                    Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KpxPipeMonitoring.ExcelPattern.TankFlow.xlsx");
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

                    chart1.SaveImage(Path.GetTempPath() + strFileName.Replace(".xlsx", ".png"), ChartImageFormat.Png);
                    string flowTempFileName = strFileName.Replace(".xlsx", "") + "_flow.png";
                    chart1Flow.SaveImage(Path.GetTempPath() + flowTempFileName, ChartImageFormat.Png);
                    //curChartFlow.SaveImage(Path.GetTempPath() + strFileName.Replace(".xlsx", ".png"), ChartImageFormat.Png);

                    excelApp = MainForm.Instance.excelApp;
                    wb = excelApp.Workbooks.Open(strFilePath);
                    // 압력 그래프
                    ws = wb.Worksheets.get_Item(1) as Excel.Worksheet;
                    ws.Cells[1, 1] = "KPX Global 배관 탱크 모니터링 시스템 (" + tempReportClickBtn.Tag.ToString() + " 유량, 압력 조회)";
                    ws.Cells[4, 1] = "조회기간 : " + searchBeforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + searchAfterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    CommonFunction.WorkListField work = comboBox_chartPipeList.SelectedItem as CommonFunction.WorkListField;
                    if (work != null) 
                        ws.Cells[5, 1] = "연결된 배관 : " + work.strPipeName;
                    ws.Shapes.AddPicture(Path.GetTempPath() + strFileName.Replace(".xlsx", ".png"), Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 15, 360, 750, 250);
                    ws.Shapes.AddPicture(Path.GetTempPath() + flowTempFileName, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, 15, 100, 750, 250);
                                        
                    // 작업, 알람이력
                    Excel.Worksheet ws2 = null;
                    ws2 = wb.Worksheets.get_Item(2) as Excel.Worksheet;
                    ws2.Cells[1, 1] = "KPX Global 배관 탱크 모니터링 시스템 (" + tempReportClickBtn.Tag.ToString() + ")";
                    ws2.Cells[4, 1] = "조회기간 : " + searchBeforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + searchAfterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    for (int i = 0; i < dataGridView_1.Rows.Count; i++)
                    {
                        string Num = dataGridView_1.Rows[i].Cells["Num"].Value.ToString();
                        string PipeName = dataGridView_1.Rows[i].Cells["PipeName"].Value.ToString();
                        string[] SubBeginTime = dataGridView_1.Rows[i].Cells["SubBeginTime"].Value.ToString().Split('/');
                        string EndTime = dataGridView_1.Rows[i].Cells["EndTime"].Value.ToString();
                        string Status = dataGridView_1.Rows[i].Cells["Status"].Value.ToString();

                        //ws2.Cells[i + 7, 1] = Num;
                        //ws2.Cells[i + 7, 2] = PipeName;
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
                        strList.Add(PipeName);

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
                    Microsoft.Office.Interop.Excel.Range cell2 = ws2.Cells[7 + dataGridView_1.Rows.Count - 1, 6]; // 데이터가 3개 일경우 7,8,9행에 입력되므로 -1 해줌
                    ws2.get_Range(cell1, cell2).Cells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

                    ReleaseExcelObject(ws2);
                    wb.Close(true);
                    //excelApp.Quit();
                }
                else if (nName <= 0 && strTabName.ToUpper() == "TOTAL")
                {
                    Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KpxPipeMonitoring.ExcelPattern.TotalTank.xlsx");
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
                    for (int i = 0; i < dataGridView_1.Rows.Count; i++)
                    {
                        string TankName = dataGridView_1.Rows[i].Cells["TankName"].Value.ToString();
                        string PipeName = dataGridView_1.Rows[i].Cells["PipeName"].Value.ToString();                        
                        string AvgFlow = dataGridView_1.Rows[i].Cells["AvgFlow"].Value.ToString();
                        string MaxFlow = dataGridView_1.Rows[i].Cells["MaxFlow"].Value.ToString();
                        string MinFlow = dataGridView_1.Rows[i].Cells["MinFlow"].Value.ToString();
                        string AvgPressure = dataGridView_1.Rows[i].Cells["AvgPressure"].Value.ToString();
                        string MaxPressure = dataGridView_1.Rows[i].Cells["MaxPressure"].Value.ToString();
                        string MinPressure = dataGridView_1.Rows[i].Cells["MinPressure"].Value.ToString();
                        string RecentWorkTime = dataGridView_1.Rows[i].Cells["RecentWorkTime"].Value.ToString();
                        string Status = dataGridView_1.Rows[i].Cells["Status"].Value.ToString();

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
                        strList.Add(TankName);
                        strList.Add(PipeName);    
                        strList.Add(AvgFlow);
                        strList.Add(MaxFlow);
                        strList.Add(MinFlow);
                        strList.Add(AvgPressure);
                        strList.Add(MaxPressure);
                        strList.Add(MinPressure);
                        strList.Add(RecentWorkTime);
                        strList.Add(Status);

                        Microsoft.Office.Interop.Excel.Range rng = excelApp.get_Range("A" + (i + 5).ToString(), "K" + (i + 5).ToString());
                        rng.Value = strList.ToArray();
                    }

                    wb.Close(true);
                    //excelApp.Quit();
                }
                else if (nName <= 0 && strTabName.ToUpper() == "ALARMHISTORY")
                {
                    Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KpxPipeMonitoring.ExcelPattern.AlarmHistoryTank.xlsx");
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
                    string pipeName = ((KpxPipeMonitoring.CommonFunction.TankInfo)comboBox_tankList.SelectedItem).strTankName;
                    ws.Cells[1, 1] = "KPX Global 배관 탱크 모니터링 시스템 (알람이력 - " + pipeName + ")";
                    ws.Cells[4, 1] = pipeName;
                    ws.Cells[4, 7] = "조회기간 : " + searchBeforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + searchAfterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    for (int i = 0; i < dataGridView_1.Rows.Count; i++)
                    {
                        string TankName = dataGridView_1.Rows[i].Cells["TankName"].Value.ToString();
                        string PipeName = dataGridView_1.Rows[i].Cells["PipeName"].Value.ToString(); 
                        string BeginTime = dataGridView_1.Rows[i].Cells["BeginTime"].Value.ToString();
                        string EndTime = dataGridView_1.Rows[i].Cells["EndTime"].Value.ToString();
                        string AlarmTime = dataGridView_1.Rows[i].Cells["AlarmTime"].Value.ToString();
                        string StandardPressure = dataGridView_1.Rows[i].Cells["StandardPressure"].Value.ToString();
                        string AlarmPressure = dataGridView_1.Rows[i].Cells["AlarmPressure"].Value.ToString();
                        string Status = dataGridView_1.Rows[i].Cells["Status"].Value.ToString();
                        string Terminator = dataGridView_1.Rows[i].Cells["Terminator"].Value.ToString();
                        string AlarmOccurrence = dataGridView_1.Rows[i].Cells["AlarmOccurrence"].Value.ToString();
                        string AlarmComment = dataGridView_1.Rows[i].Cells["AlarmComment"].Value.ToString();

                        List<string> strList = new List<string>();
                        strList.Add((i + 1).ToString());
                        strList.Add(TankName);
                        strList.Add(PipeName); 
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
                    Microsoft.Office.Interop.Excel.Range cell2 = ws.Cells[6 + dataGridView_1.Rows.Count - 1, 12]; // 데이터가 3개 일경우 7,8,9행에 입력되므로 -1 해줌
                    ws.get_Range(cell1, cell2).Cells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

                    wb.Close(true);
                    //excelApp.Quit();
                    System.Diagnostics.Trace.WriteLine(DateTime.Now);
                }
                else if (nName <= 0 && strTabName.ToUpper() == "WORKHISTORY")
                {
                    System.Diagnostics.Trace.WriteLine("1 : " + DateTime.Now);
                    Stream resourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("KpxPipeMonitoring.ExcelPattern.WorkHistoryTank.xlsx");
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

                    string pipeName = ((KpxPipeMonitoring.CommonFunction.TankInfo)comboBox_tankList.SelectedItem).strTankName;
                    ws.Cells[1, 1] = "KPX Global 배관 탱크 모니터링 시스템 (작업이력 - " + pipeName + ")";
                    ws.Cells[4, 1] = pipeName;
                    ws.Cells[4, 7] = "조회기간 : " + searchBeforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + searchAfterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    System.Diagnostics.Trace.WriteLine("4 : " + DateTime.Now);

                    for (int i = 0; i < dataGridView_1.Rows.Count; i++)
                    {
                        string TankName = dataGridView_1.Rows[i].Cells["TankName"].Value.ToString();
                        string PipeName = dataGridView_1.Rows[i].Cells["PipeName"].Value.ToString(); 
                        string BeginTime = dataGridView_1.Rows[i].Cells["BeginTime"].Value.ToString();
                        string EndTime = dataGridView_1.Rows[i].Cells["EndTime"].Value.ToString();
                        string CTime = dataGridView_1.Rows[i].Cells["CTime"].Value.ToString();
                        string IgnoreCTime = dataGridView_1.Rows[i].Cells["IgnoreCTime"].Value.ToString();
                        string BeginUserName = dataGridView_1.Rows[i].Cells["BeginUserName"].Value.ToString();
                        string EndUserId = dataGridView_1.Rows[i].Cells["EndUserId"].Value.ToString();

                        List<string> strList = new List<string>();
                        strList.Add((i + 1).ToString());
                        strList.Add(TankName);
                        strList.Add(PipeName); 
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
                    Microsoft.Office.Interop.Excel.Range cell2 = ws.Cells[6 + dataGridView_1.Rows.Count - 1, 9]; // 데이터가 3개 일경우 7,8,9행에 입력되므로 -1 해줌
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

        #region 페이징 버튼이벤트
        /// <summary>
        /// TabIndex, PageNo, ChartData
        /// </summary>
        Dictionary<int, Dictionary<int, List<CommonFunction.ChartField>>> dicPageChart = new Dictionary<int, Dictionary<int, List<CommonFunction.ChartField>>>();
        Dictionary<int, PageEntity> dicPageEntity = new Dictionary<int, PageEntity>();
        private void pictureBox_doubleLeft_Click(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            if (!dicPageEntity.ContainsKey(nTankID)) return;

            int minPage = dicPageEntity[nTankID].nMinPage;
            int maxPage = dicPageEntity[nTankID].nMaxPage;
            int curPage = dicPageEntity[nTankID].nCurPage;
            if (minPage == maxPage || minPage == curPage) return;

            if (minPage < curPage)
            {
                curPage = minPage;

                DateTime beforeDate = dicPageChart[nTankID][curPage][0].dtTimeStamp;
                DateTime afterDate = dicPageChart[nTankID][curPage][dicPageChart[nTankID][curPage].Count - 1].dtTimeStamp;
                 
                InitSeries(beforeDate, afterDate);
                chart1.DataSource = dicPageChart[nTankID][curPage];
                chart1Flow.DataSource = dicPageChart[nTankID][curPage];
                dicPageEntity[nTankID].nCurPage = curPage;

                string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");
                SetPageText(curPage, dicPageEntity[nTankID].nMaxPage, searchDate);

                chart1.Customize += chart1_Customize;
                chart1Flow.Customize += chart1Flow_Customize;

                if (chart1.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1.Update();
                }
                if (chart1.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    chart1.Update();
                }

                if (chart1Flow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    chart1Flow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1Flow.Update();
                }
                if (chart1Flow.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    chart1Flow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    chart1Flow.Update();
                }  
            }
        }

        private void pictureBox_left_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            if (!dicPageEntity.ContainsKey(nTankID)) return;

            int minPage = dicPageEntity[nTankID].nMinPage;
            int maxPage = dicPageEntity[nTankID].nMaxPage;
            int curPage = dicPageEntity[nTankID].nCurPage;

            if (curPage > minPage)
            {
                curPage--;

                DateTime beforeDate = dicPageChart[nTankID][curPage][0].dtTimeStamp;
                DateTime afterDate = dicPageChart[nTankID][curPage][dicPageChart[nTankID][curPage].Count - 1].dtTimeStamp;

                InitSeries(beforeDate, afterDate);
                 
                chart1.DataSource = dicPageChart[nTankID][curPage];
                chart1Flow.DataSource = dicPageChart[nTankID][curPage];
                dicPageEntity[nTankID].nCurPage = curPage;

                string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");
                SetPageText(curPage, dicPageEntity[nTankID].nMaxPage, searchDate);

                chart1.Customize += chart1_Customize;
                chart1Flow.Customize += chart1Flow_Customize;

                if (chart1.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1.Update();
                }
                if (chart1.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    chart1.Update();
                }

                if (chart1Flow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    chart1Flow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1Flow.Update();
                }
                if (chart1Flow.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    chart1Flow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    chart1Flow.Update();
                }  
            }
        }

        private void pictureBox_right_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            if (!dicPageEntity.ContainsKey(nTankID)) return;

            int minPage = dicPageEntity[nTankID].nMinPage;
            int maxPage = dicPageEntity[nTankID].nMaxPage;
            int curPage = dicPageEntity[nTankID].nCurPage;

            if (curPage < maxPage)
            {
                curPage++;

                DateTime beforeDate = dicPageChart[nTankID][curPage][0].dtTimeStamp;
                DateTime afterDate = dicPageChart[nTankID][curPage][dicPageChart[nTankID][curPage].Count - 1].dtTimeStamp;

                InitSeries(beforeDate, afterDate);
                 
                chart1.DataSource = dicPageChart[nTankID][curPage];
                chart1Flow.DataSource = dicPageChart[nTankID][curPage];
                dicPageEntity[nTankID].nCurPage = curPage;

                string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");
                SetPageText(curPage, dicPageEntity[nTankID].nMaxPage, searchDate);

                chart1.Customize += chart1_Customize;
                chart1Flow.Customize += chart1Flow_Customize;

                if (chart1.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1.Update();
                }
                if (chart1.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    chart1.Update();
                }

                if (chart1Flow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    chart1Flow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1Flow.Update();
                }
                if (chart1Flow.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    chart1Flow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    chart1Flow.Update();
                }  
            }
        }

        private void pictureBox_doubleRight_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            if (!dicPageEntity.ContainsKey(nTankID)) return;

            int minPage = dicPageEntity[nTankID].nMinPage;
            int maxPage = dicPageEntity[nTankID].nMaxPage;
            int curPage = dicPageEntity[nTankID].nCurPage;

            if (minPage == maxPage || maxPage == curPage) return;

            if (maxPage > curPage)
            {
                curPage = maxPage;

                DateTime beforeDate = dicPageChart[nTankID][curPage][0].dtTimeStamp;
                DateTime afterDate = dicPageChart[nTankID][curPage][dicPageChart[nTankID][curPage].Count - 1].dtTimeStamp;

                InitSeries(beforeDate, afterDate);
                 
                chart1.DataSource = dicPageChart[nTankID][curPage];
                chart1Flow.DataSource = dicPageChart[nTankID][curPage];
                dicPageEntity[nTankID].nCurPage = curPage;

                string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");
                SetPageText(curPage, dicPageEntity[nTankID].nMaxPage, searchDate);

                chart1.Customize += chart1_Customize;
                chart1Flow.Customize += chart1Flow_Customize;

                if (chart1.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1.Update();
                }
                if (chart1.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    chart1.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    chart1.Update();
                }

                if (chart1Flow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                {
                    chart1Flow.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                    chart1Flow.Update();
                }
                if (chart1Flow.ChartAreas[0].AxisY.ScaleView.IsZoomed)
                {
                    chart1Flow.ChartAreas[0].AxisY.ScaleView.ZoomReset();
                    chart1Flow.Update();
                }  
            }
        }

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
            if (!dicPageEntity.ContainsKey(nTankID)) return;

            int minPage = dicPageEntity[nTankID].nMinPage;
            int maxPage = dicPageEntity[nTankID].nMaxPage;
            int curPage = dicPageEntity[nTankID].nCurPage;

            try
            {
                if (textBox1.Text == curPage.ToString()) return;

                int a;
                if (!int.TryParse(textBox1.Text, out a)) throw new ApplicationException("숫자형식으로 입력하세요.");
                if (a >= minPage && a <= maxPage && a != curPage)
                {
                    curPage = a;

                    DateTime beforeDate = dicPageChart[nTankID][curPage][0].dtTimeStamp;
                    DateTime afterDate = dicPageChart[nTankID][curPage][dicPageChart[nTankID][curPage].Count - 1].dtTimeStamp;

                    InitSeries(beforeDate, afterDate);

                    chart1.DataSource = dicPageChart[nTankID][curPage];
                    dicPageEntity[nTankID].nCurPage = curPage;

                    string searchDate = beforeDate.ToString("yyyy-MM-dd HH:mm:ss") + " ~ " + afterDate.ToString("yyyy-MM-dd HH:mm:ss");
                    SetPageText(curPage, dicPageEntity[nTankID].nMaxPage, searchDate);

                    chart1.Customize += chart1_Customize;
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

        private void SetPageText(int curPage, int maxPage, string searchDate)
        {
            textBox1.Text = curPage.ToString();
            label_maxPage.Text = string.Format("/{0}", maxPage);
            label_searchDate.Text = searchDate;
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

                CommonFunction.TankInfo selectedItem = (CommonFunction.TankInfo)comboBox_tankList.SelectedItem;
                int nSelectedTankId = Convert.ToInt32(selectedItem.nTankID);

                if (nTankID == -1) //Total 
                    DisplayTotal();
                else if (nTankID == -2) //알람이력 
                    DisplayAlarmHistory(nSelectedTankId);
                else if (nTankID == -3) //작업이력
                    DisplayWorkHistory(nSelectedTankId);
                else
                {
                    DisplayTankAlarmWorkHistory();
                    if (chartPipeID < 0) // comboBox_chartPipeList_SelectedIndexChanged 이벤트로 조회하기때문에 또 조회할 필요 없음
                        DisplayTank();
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
}
