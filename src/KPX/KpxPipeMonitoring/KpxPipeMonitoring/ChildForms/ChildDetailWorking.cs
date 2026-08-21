using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DBUtility;

namespace KpxPipeMonitoring.ChildForms
{
    public partial class ChildDetailWorking : Form
    {
        private string strBeginPage = ""; // P or T
        private int nBeginPageID = -1; // P : PipeID, T : TankID
        private int nTankID { get; set; }

        /// <summary>
        /// 배관화면에서 Load했을 경우 -> 선택한 배관ID
        /// 탱크화면에서 Load했을 경우 -> 먼저 작업시작한 배관ID
        /// </summary>
        private int nPipeID { get; set; }
        private List<int> nPipeIDs { get; set; }
          
        Timer timer = null;
        private int curSec = 0;
        private int refreshSec = 8;
        private Dictionary<int, List<CommonFunction.ChartField>> dicCharts = new Dictionary<int, List<CommonFunction.ChartField>>();
        /// <summary>
        /// 그래프 조회 조건 (분 단위)
        /// </summary>
        private int nSearchCondition = 30;
        /// <summary>
        /// 그래프 조회 조건 - 사용자 정의 (시간 단위)
        /// </summary>
        private decimal nUserDefine = 0;
        private int minPage = 1;
        private int maxPage = 0;
        private int curPage = 1;
        /// <summary>
        /// 그래프 전체 데이터
        /// </summary>
        List<CommonFunction.ChartField> totalChartData = new List<CommonFunction.ChartField>();
        /// <summary>
        /// 표현할 그래프 데이터
        /// </summary>
        List<CommonFunction.ChartField> displayChartData = new List<CommonFunction.ChartField>();
         
        private DBUtility.VariousData<DateTime> m_recentBeginTime = null;
         
        private IHistoryManager m_historyMgr = null;
         
        public List<CommonFunction.AllAlarm> oldAlarmList = new List<CommonFunction.AllAlarm>();
        public bool isAlarm { get; set; }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int DestroyCaret(); 
        
        #region 초기화 
        public ChildDetailWorking(string pageKind, int tankID, int pipeID)
        {
            this.DoubleBuffered = true;
            InitializeComponent();
              
            m_historyMgr = new HistoryManager(MainForm.Instance);

            LoadTankImage();

            this.strBeginPage = pageKind;
            this.nTankID = tankID;
            this.nPipeID = pipeID;

            // 이 ID는 변하지 않음
            if (strBeginPage == "P")
                nBeginPageID = pipeID;
            else if (strBeginPage == "T")
                nBeginPageID = tankID;

            DisplayConnectedIDs();

            if (radioButton_pipeId1 != null && radioButton_pipeId1.Tag != null && (int)radioButton_pipeId1.Tag == nPipeID)
            {
                radioButton_pipeId1.Checked = true;
                radioButton_pipeId2.Checked = false;
            }
            else if (radioButton_pipeId2 != null && radioButton_pipeId2.Tag != null && (int)radioButton_pipeId2.Tag == nPipeID)
            {
                radioButton_pipeId1.Checked = false;
                radioButton_pipeId2.Checked = true;
            }

            MainForm.Instance.SetDoubleBuffer(panel1, true);

            pictureBox_alarmPressure.Parent = pictureBox_tank;
            pictureBox_alarmPressure.Location = new Point(50, 80);
             
            pictureBox_alarmPressure.Visible = false; 
            pictureBox_clearAlarm.Visible = false; 

            comboBox1.Items.Add(new ComboItem("최근 30분", 30));
            comboBox1.Items.Add(new ComboItem("최근 1시간", 60));
            comboBox1.Items.Add(new ComboItem("최근 3시간", 180));
            comboBox1.Items.Add(new ComboItem("최근 5시간", 300));
            comboBox1.Items.Add(new ComboItem("전체 작업", 0));
            comboBox1.Items.Add(new ComboItem("사용자 정의", -1));                
            
            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
             
            label_begin.Visible = false;
            label_end.Visible = false;
            numericUpDown1.Minimum = 0;
            numericUpDown1.Maximum = 9;
            numericUpDown1.ReadOnly = true; 
            numericUpDown1.Visible = false;
            numericUpDown1.ValueChanged += (s, e) =>
                {
                    DestroyCaret();
                };
            numericUpDown1.MouseClick += (s, e) =>
                {
                    DestroyCaret();
                };

            MainForm.Instance.commonFunction.SettingButton(pictureBox_close, KpxPipeMonitoring.Properties.Resources.Close, KpxPipeMonitoring.Properties.Resources.Close_MouseOver);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_doubleLeft, KpxPipeMonitoring.Properties.Resources.ChildDetailWorking_DoubleLeft_Normal, KpxPipeMonitoring.Properties.Resources.ChildDetailWorking_DoubleLeft_Click);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_left, KpxPipeMonitoring.Properties.Resources.ChildDetailWorking_Left_Normal, KpxPipeMonitoring.Properties.Resources.ChildDetailWorking_Left_Click);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_doubleRight, KpxPipeMonitoring.Properties.Resources.ChildDetailWorking_DoubleRight_Normal, KpxPipeMonitoring.Properties.Resources.ChildDetailWorking_DoubleRight_Click);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_right, KpxPipeMonitoring.Properties.Resources.ChildDetailWorking_Right_Normal, KpxPipeMonitoring.Properties.Resources.ChildDetailWorking_Right_Click);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_rangeRefresh, KpxPipeMonitoring.Properties.Resources.RangeRefresh2_Normal, KpxPipeMonitoring.Properties.Resources.RangeRefresh2_Click);
             
            InitChart();
            DisplayChart();
            DisplayInfo();            
            SettingStatus();

            radioButton_pipeId1.CheckedChanged += radioButton_pipeId_CheckedChanged;
            radioButton_pipeId2.CheckedChanged += radioButton_pipeId_CheckedChanged;

            this.timer = new Timer();
            this.timer.Interval = 1000;
            this.timer.Tick += timer_Tick; 
            this.timer.Start();
        }

        void radioButton_pipeId_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb != null && rb.Checked)
            {
                if (rb.Equals(radioButton_pipeId1))
                    this.nPipeID = (int)radioButton_pipeId1.Tag;
                else if (rb.Equals(radioButton_pipeId2))
                    this.nPipeID = (int)radioButton_pipeId2.Tag;

                curPage = 1;
                minPage = 1;
                maxPage = 1;
                displayChartData.Clear();
                DisplayChart();
                DisplayInfo();
                SettingStatus();
                curSec = 1;  
            }
        } 

        private void InitChart()
        { 
            chart_pressure.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart_pressure.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot; 
            chart_pressure.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart_pressure.ChartAreas[0].AxisY.Interval = 0;
            chart_pressure.Legends.Clear();
             

            chart_flow.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart_flow.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart_flow.ChartAreas[0].AxisX.IsMarginVisible = false;
            chart_flow.ChartAreas[0].AxisY.Interval = 0;
            chart_flow.Legends.Clear();
             
            //차트 위치
            chart_pressure.ChartAreas[0].Position.Auto = false;
            chart_pressure.ChartAreas[0].Position.X = 0;
            chart_pressure.ChartAreas[0].Position.Y = 20;
            chart_pressure.ChartAreas[0].Position.Width = 97;
            chart_pressure.ChartAreas[0].Position.Height = 90;

            chart_flow.ChartAreas[0].Position.Auto = false;
            chart_flow.ChartAreas[0].Position.X = 0;
            chart_flow.ChartAreas[0].Position.Y = 20;
            chart_flow.ChartAreas[0].Position.Width = 97;
            chart_flow.ChartAreas[0].Position.Height = 90; 
        }
        private void InitSeries(DateTime beforeDate, DateTime afterDate)
        { 
            chart_pressure.Series.Clear();
            Series series = chart_pressure.Series.Add("series1");
            series.ChartType = SeriesChartType.Line;
            chart_pressure.Series[0].IsXValueIndexed = true;
            chart_pressure.Series[0].XValueMember = "dtTimeStamp";
            chart_pressure.Series[0].YValueMembers = "dPressure";
            chart_pressure.Series[0].BorderWidth = 3;
            chart_pressure.Series[0].Color = Color.FromArgb(48, 129, 209);

            chart_pressure.ChartAreas[0].AxisY.LabelStyle.Format = "F1";
            chart_pressure.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            chart_pressure.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            
            chart_flow.Series.Clear();
            series = chart_flow.Series.Add("series1");
            series.ChartType = SeriesChartType.Line;
            chart_flow.Series[0].IsXValueIndexed = true;
            chart_flow.Series[0].XValueMember = "dtTimeStamp";
            chart_flow.Series[0].YValueMembers = "dFlow";
            chart_flow.Series[0].BorderWidth = 3;
            chart_flow.Series[0].Color = Color.FromArgb(255, 137, 0);

            chart_flow.ChartAreas[0].AxisY.LabelStyle.Format = "F1";
            chart_flow.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            chart_flow.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            
            string dateFormat = "HH:mm";
            DateTimeIntervalType IntervalType = MainForm.Instance.commonFunction.GetIntervalType(beforeDate, afterDate); 
            if (IntervalType == DateTimeIntervalType.Seconds || IntervalType == DateTimeIntervalType.Minutes)
                dateFormat = "HH:mm"; 
            else
                dateFormat = "MM/dd\r\nHH:mm";

            chart_pressure.Series[0].XValueType = ChartValueType.DateTime;
            chart_pressure.Series[0].ToolTip = "#VALX{yyyy-MM-dd HH:mm:ss} - #VALY1{0.00}";
            chart_pressure.ChartAreas[0].AxisX.LabelStyle.Format = dateFormat;

            chart_flow.Series[0].XValueType = ChartValueType.DateTime;
            chart_flow.Series[0].ToolTip = "#VALX{yyyy-MM-dd HH:mm:ss} - #VALY1{0.00}";
            chart_flow.ChartAreas[0].AxisX.LabelStyle.Format = dateFormat; 
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (timer != null && timer.Enabled)
            {
                this.timer.Stop();
                this.timer.Enabled = false;
                this.timer.Dispose();
                this.timer = null; 
            } 
        } 
        
        public void Dispose()
        {
            if (timer != null && timer.Enabled)
            {
                this.timer.Stop();
                this.timer.Enabled = false;
                this.timer.Dispose();
                this.timer = null;
            } 
        }
        
        #endregion

        #region 콤보 이벤트
        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        { 
            ComboItem selectedItem = (ComboItem)comboBox1.SelectedItem;
            nSearchCondition = Convert.ToInt32(selectedItem.Value);

            if (nSearchCondition == -1)
            {
                label_begin.Visible = true;
                label_end.Visible = true;
                numericUpDown1.Visible = true;
            }
            else
            {
                label_begin.Visible = false;
                label_end.Visible = false;
                numericUpDown1.Visible = false;
            }

            if (nSearchCondition <= 0)
            {
                pictureBox_doubleLeft.Visible = false;
                pictureBox_doubleRight.Visible = false;
                pictureBox_left.Visible = false;
                pictureBox_right.Visible = false;
                label_curPage.Visible = false;
                label_maxPage.Visible = false;
            }
            else
            {
                pictureBox_doubleLeft.Visible = true;
                pictureBox_doubleRight.Visible = true;
                pictureBox_left.Visible = true;
                pictureBox_right.Visible = true;
                label_curPage.Visible = true;
                label_maxPage.Visible = true;
            }

            curPage = 1;
            minPage = 1;
            maxPage = 1; 
            displayChartData.Clear(); 
             
            DisplayChart();
            DisplayInfo();
            curSec = 1;
        }
        #endregion

        #region Spin 이벤트
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        { 
            nUserDefine = numericUpDown1.Value;
                       
            displayChartData.Clear();              
            DisplayChart();
            DisplayInfo();
        }
        #endregion

        #region 타이머 이벤트
        void timer_Tick(object sender, EventArgs e)
        {
            if (refreshSec == curSec)
            {
                DisplayConnectedIDs();

                DisplayChart();
                DisplayInfo();
                SettingStatus();
                curSec = 1; 
            }
            else
            {
                SetWorkTime();
                curSec++;
            }
        }
        #endregion 

        #region 그래프 Display 
        private void DisplayChart()
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                Dictionary<DateTime, List<double>> dicTempDatas = new Dictionary<DateTime, List<double>>();

                #region 1. DB로 읽기
                //if (searchMaxID == 0)
                //{
                //    ArrayList arrList = MainForm.Instance.dbMgr.GetResultData("SELECT min(id) FROM kpx.pipehistory where date_add(now(), interval - 48 hour) <= TimeStamp", 0);
                //    if (arrList != null && arrList.Count > 0)
                //    {
                //        int minId = DBUtility.WebDBManager.GetIntField(arrList[0].ToString(), 0);
                //        searchMaxID = minId;
                //    }
                //}

                //StringBuilder sb = new StringBuilder();
                //sb.Append("SELECT ph.ID, PipeID, TimeStamp, ph.Pressure ");
                //sb.Append("  FROM PipeHistory ph ");
                //sb.Append(" WHERE pipeid = " + this.nPipeID);
                //sb.Append("   AND Pressure > 0.2 AND ph.ID > " + searchMaxID);

                //ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                //if (arrResult == null) return;

                //if (arrResult.Count == 0)
                //{
                //    ArrayList arrList = MainForm.Instance.dbMgr.GetResultData("select max(id) from pipehistory", 0);
                //    if (arrList != null && arrList.Count > 0)
                //    {
                //        int maxId = DBUtility.WebDBManager.GetIntField(arrList[0].ToString(), 0);
                //        searchMaxID = maxId;
                //    }
                //}

                //for (int i = 0; i < arrResult.Count; i += 4)
                //{
                //    int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                //    int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                //    string strPipeName = "";
                //    DateTime date = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 2], new DateTime());
                //    double pressure = (arrResult[i + 3].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 3]);

                //    searchMaxID = nID;

                //    totalChartData.Add(new CommonFunction.PipeChartField(nPipeID, strPipeName, date, date.ToString("HH:mm:ss"), pressure));
                //} 
                #endregion 
                 
                DateTime dtNow = MainForm.Instance.SystemNow;

                #region 2. 파일로 읽기
                DateTime beforeDate = dtNow.AddDays(-2);
                DateTime afterDate = dtNow;

                List<HistoryQuery> historyQueries = new List<HistoryQuery>();                
                int totalDays = (int)(afterDate - beforeDate).TotalDays;
                for (int i = 0; i <= totalDays; i++)
                {
                    string y = beforeDate.AddDays(i).Year.ToString();
                    string m = beforeDate.AddDays(i).Month.ToString();
                    string d = beforeDate.AddDays(i).Day.ToString();

                    HistoryQuery query = null;
                    if (this.nPipeID <= 0)
                        query = new HistoryQuery(nTankID, y, m, d, HistoryQueryType.유량);
                    else
                        query = new HistoryQuery(this.nPipeID, y, m, d, HistoryQueryType.작업중);
                    historyQueries.Add(query);
                }
                totalChartData = m_historyMgr.ReadHistory(historyQueries);
                
                historyQueries.Clear();
                historyQueries = null;
                 
                #endregion 
                 
                if (nSearchCondition > 0)
                {
                    int displayCondition = MainForm.Instance.commonFunction.GetChartPointCount(dtNow.AddMinutes(-nSearchCondition), dtNow);

                    //조회 시점
                    DateTime searchDateTime = dtNow.AddMinutes(-nSearchCondition);
                     
                    foreach (CommonFunction.ChartField item in totalChartData)
                    { 
                        if (displayChartData.Count == 0 || displayChartData[displayChartData.Count -1].dtTimeStamp < item.dtTimeStamp)
                        {
                            if (searchDateTime <= item.dtTimeStamp)
                            {   
                                displayChartData.Add(new CommonFunction.ChartField(0, 0, item.dtTimeStamp, item.dPressure, item.dFlow));
                            } 
                        }
                    }

                    // 조회조건에 미치지 못해서 (ex: 30분일경우 3개, 1시간일경우 6개) 차트 데이터가 add되지 못한 경우
                    //if (dicTempDatas.Count > 0)
                    //{  
                    //    Dictionary<DateTime, List<double>> dicTempDatas2 = new Dictionary<DateTime, List<double>>();
                    //    List<double> doubles2 = new List<double>();
                    //    DateTime beforeDate2 = dicTempDatas.Keys.Min();
                    //    DateTime afterDate2 = dicTempDatas.Keys.Max();
                    //    int displayCondition2 = commonFunction.GetChartPointCount(beforeDate2, afterDate2);
                    //    foreach (KeyValuePair<DateTime, List<double>> item in dicTempDatas)
                    //    {
                    //        if (item.Key >= beforeDate && item.Key <= afterDate)
                    //        {
                    //            if (displayChartData.Count == 0 || displayChartData[displayChartData.Count - 1].dtTimeStamp < item.Key)
                    //            {
                    //                if (!dicTempDatas2.ContainsKey(item.Key))
                    //                {
                    //                    dicTempDatas2.Add(item.Key, item.Value);
                    //                }

                    //                if (dicTempDatas2.Count >= displayCondition2)
                    //                {
                    //                    double tempPressure = 0;
                    //                    double tempFlow = 0;
                    //                    foreach (KeyValuePair<DateTime, List<double>> item2 in dicTempDatas2)
                    //                    {
                    //                        tempPressure = tempPressure + item2.Value[0];
                    //                        tempFlow = tempFlow + item2.Value[1];
                    //                    }
                    //                    displayChartData.Add(new CommonFunction.PipeChartField(0, "", item.Key, "", tempPressure / displayCondition2, tempFlow / displayCondition2));
                    //                    dicTempDatas2.Clear();
                    //                }
                    //            }
                    //        }
                    //    } 
                    //}

                    if (displayChartData.Count == 0)
                    {
                        InitSeries(new DateTime(), new DateTime());
                        ChartNull(chart_pressure);
                        ChartNull(chart_flow); 
                        return;
                    }

                    //조회 조건 단위로 짜르기
                    DateTime lastDataDateTime = displayChartData[displayChartData.Count - 1].dtTimeStamp;
                    DateTime lastDataDateTime2 = lastDataDateTime.AddMinutes(-nSearchCondition);
                    int nDic = 1;
                    int nChangeDic = 0; //0 or 1 
                    dicCharts.Clear();
                    Dictionary<int, List<CommonFunction.ChartField>> dicTempCharts = new Dictionary<int, List<CommonFunction.ChartField>>();
                    for (int i = displayChartData.Count - 1; i >= 0; i--)
                    {
                        if (lastDataDateTime >= displayChartData[i].dtTimeStamp && lastDataDateTime2 <= displayChartData[i].dtTimeStamp)
                        {
                            nChangeDic = 0;
                            if (dicTempCharts.ContainsKey(nDic))
                                dicTempCharts[nDic].Add(displayChartData[i]);
                            else
                            {
                                dicTempCharts.Add(nDic, new List<CommonFunction.ChartField>());
                                dicTempCharts[nDic].Add(displayChartData[i]);
                            }
                        }
                        else
                            nChangeDic = 1;

                        if (nChangeDic == 1)
                        {
                            nDic++;
                            nChangeDic = 0;
                            lastDataDateTime = lastDataDateTime.AddMinutes(-nSearchCondition);
                            lastDataDateTime2 = lastDataDateTime2.AddMinutes(-nSearchCondition);
                        }
                    }

                    //1번 페이지에 조회 조건(분) 맞추기
                    //ex : 최근 30분일때 1번페이지에 01:00~15:00 으로 짤렸을 경우 01:00~31:00 으로 맞춰줌
                    int dicTempChartsCount = dicTempCharts.Count;
                    if (dicTempChartsCount > 1)
                    {
                        DateTime d = dicTempCharts[dicTempChartsCount][0].dtTimeStamp;
                        DateTime d2 = dicTempCharts[dicTempChartsCount][dicTempCharts[dicTempChartsCount].Count - 1].dtTimeStamp;

                        TimeSpan d3 = d - d2;
                        if (d3.Minutes < 1)
                        {
                            for (int i = 0; i < dicTempCharts[dicTempChartsCount - 1].Count; i++)
                            {
                                CommonFunction.ChartField a =
                                    dicTempCharts[dicTempChartsCount - 1][dicTempCharts[dicTempChartsCount - 1].Count - i - 1];

                                //첫번째 배열에 추가 (시간 순서대로 추가하기 위해)
                                dicTempCharts[dicTempChartsCount].Insert(0, a);

                                d = dicTempCharts[dicTempChartsCount][0].dtTimeStamp;
                                d2 = dicTempCharts[dicTempChartsCount][dicTempCharts[dicTempChartsCount].Count - 1].dtTimeStamp;
                                d3 = d - d2;
                                if (d3.Minutes > 1) break;
                            }
                        }
                    }

                    //순서 뒤집기
                    int tempCnt = 1;
                    for (int i = dicTempChartsCount - 1; i >= 0; i--)
                    {
                        if (dicTempCharts.ContainsKey(i + 1))
                        {
                            dicCharts.Add(tempCnt, dicTempCharts[i + 1]);
                            tempCnt++;
                        }
                    }
                     
                    // Page 추가됐을때 새로 추가된 Page를 현재페이지로 설정
                    if (this.maxPage < dicCharts.Count) 
                        this.curPage = dicCharts.Count;

                    this.maxPage = dicCharts.Count;
                    label_curPage.Text = curPage.ToString();
                    label_maxPage.Text = string.Format("/{0}", this.maxPage); 

                    InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);

                    if (this.nPipeID == -1)
                    {
                        InitSeries(new DateTime(), new DateTime());
                        ChartNull(chart_pressure);
                    }
                    else
                        chart_pressure.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);
                    
                    if (this.nTankID == -1)
                    {
                        InitSeries(new DateTime(), new DateTime()); 
                        ChartNull(chart_flow); 
                    }
                    else
                        chart_flow.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp); 
                }
                else if (nSearchCondition == 0) //전체 작업
                {
                    if (this.m_recentBeginTime == null)
                    {
                        InitSeries(new DateTime(), new DateTime());
                        ChartNull(chart_pressure); 
                        ChartNull(chart_flow); 
                        return;
                    }

                    int displayCondition = MainForm.Instance.commonFunction.GetChartPointCount(m_recentBeginTime.Data, dtNow);

                    // 시간이 지난 데이터 제거
                    List<CommonFunction.ChartField> removeChartList = new List<CommonFunction.ChartField>();
                    foreach (CommonFunction.ChartField item in displayChartData)
                    {
                        if (item.dtTimeStamp < m_recentBeginTime.Data)
                            removeChartList.Add(item);
                        else
                            break; // 시간 순서로 저장되기 때문에 더이상 반복할 이유가 없음
                    }
                    for (int i = removeChartList.Count - 1; i >= 0; i--)
                    {
                        displayChartData.Remove(removeChartList[i]);
                    }

                    foreach (CommonFunction.ChartField item in totalChartData)
                    {
                        if (m_recentBeginTime.Data < item.dtTimeStamp)
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
                                    displayChartData.Add(new CommonFunction.ChartField(0, 0, item.dtTimeStamp, tempPressure / dicTempDatas.Count, tempFlow / displayCondition));
                                    dicTempDatas.Clear();
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
                        ChartNull(chart_pressure);
                        ChartNull(chart_flow); 
                        return;
                    } 

                    InitSeries(displayChartData[0].dtTimeStamp, displayChartData[displayChartData.Count - 1].dtTimeStamp);
                    chart_pressure.DataSource = null;
                    chart_flow.DataSource = null;
                    chart_pressure.DataSource = displayChartData; 
                    chart_flow.DataSource = displayChartData; 
                }
                else if (nSearchCondition == -1) // 사용자 정의
                {
                    if (this.m_recentBeginTime == null)
                    {
                        InitSeries(new DateTime(), new DateTime());
                        ChartNull(chart_pressure);
                        ChartNull(chart_flow); 
                        return;
                    }

                    int displayCondition = MainForm.Instance.commonFunction.GetChartPointCount(m_recentBeginTime.Data, dtNow);

                    DateTime userTime = m_recentBeginTime.Data.AddHours(Convert.ToDouble(nUserDefine));

                    // 시간이 지난 데이터 제거
                    List<CommonFunction.ChartField> removeChartList = new List<CommonFunction.ChartField>();
                    foreach (CommonFunction.ChartField item in displayChartData)
                    {
                        if (item.dtTimeStamp < userTime)
                            removeChartList.Add(item);
                        else
                            break; // 시간 순서로 저장되기 때문에 더이상 반복할 이유가 없음
                    }
                    for (int i = removeChartList.Count - 1; i >= 0; i--)
                    {
                        displayChartData.Remove(removeChartList[i]);
                    }

                    foreach (CommonFunction.ChartField item in totalChartData)
                    {
                        if (userTime < item.dtTimeStamp)
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
                                    displayChartData.Add(new CommonFunction.ChartField(0, 0, item.dtTimeStamp, tempPressure / dicTempDatas.Count, tempFlow / dicTempDatas.Count));
                                    dicTempDatas.Clear();
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
                        ChartNull(chart_pressure);
                        ChartNull(chart_flow); 
                        return;
                    } 

                    InitSeries(displayChartData[0].dtTimeStamp, displayChartData[displayChartData.Count - 1].dtTimeStamp);
                    chart_pressure.DataSource = null;
                    chart_flow.DataSource = null;
                    chart_pressure.DataSource = displayChartData;
                    chart_flow.DataSource = displayChartData; 
                } 

                //압력, 유량 차트 시작점 맞추기
                string max = String.Format("{0:F1}", Math.Abs(chart_pressure.ChartAreas[0].AxisY.Maximum));
                string min = String.Format("{0:F1}", Math.Abs(chart_pressure.ChartAreas[0].AxisY.Minimum));

                int pressureChartAxisyMaxLength = Math.Max(max.Length, min.Length);

                string max2 = String.Format("{0:F1}", Math.Abs(chart_flow.ChartAreas[0].AxisY.Maximum));
                string min2 = String.Format("{0:F1}", Math.Abs(chart_flow.ChartAreas[0].AxisY.Minimum));

                int flowChartAxisyMaxLength = Math.Max(max2.Length, min2.Length);

                if (5 - pressureChartAxisyMaxLength >= 0)
                    chart_pressure.ChartAreas[0].Position.X = 5 - pressureChartAxisyMaxLength;
                if (5 - flowChartAxisyMaxLength >= 0)
                    chart_flow.ChartAreas[0].Position.X = 5 - flowChartAxisyMaxLength; 
            }
            catch (ApplicationException app)
            {
                Cursor = Cursors.Default;
                timer.Stop();
                UnE.Utility.UMessageBox.FrameColor = Color.FromArgb(1, 4, 20);
                UnE.Utility.UMessageBox.Show(app.Message); 
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                timer.Stop();
                UnE.Utility.UMessageBox.FrameColor = Color.FromArgb(1, 4, 20);
                UnE.Utility.UMessageBox.Show(ex.Message); 
            }
            finally
            {
                Cursor = Cursors.Default; 
            }
        }
        #endregion 
        
        #region 탱크, 배관정보, 차트Range
        private void InitLabel()
        {
            label_workStatus.Text = "-";  
            label_flow.Text = "-";
            label_flowRange.Text = "-";
            label_flowRange2.Text = "-";
            label_pressure.Text = "-";
            label_pipeRange.Text = "-";
            label_pipeRange2.Text = "-";
            label_liquidType.Text = "-";
            label_density.Text = "-";
            label_mass.Text = "-";
            label_curLevel.Text = "-";
            label_temp.Text = "-"; 
            label_flow2.Text = "-";
            label_highLevel.Text = "(m, - )";
            label_tempRange.Text = "( - )"; 
            m_recentBeginTime = null;
        }
        public void DisplayInfo()
        {
            InitLabel(); 

            double nStandardFlow = -999;
            double nStandardPressure = -999;

            int rng_nPipeStableType = 0;
            double rng_nPipeStableValue = 0;
            string rng_strPipeRange = "-";
            int rng_nPipeStableCTimeUse = -1;
            int rng_nPipeStableCTime = 0;
            string rng_strPipeStableUpdateTime = "-";

            int rng_nTankStableType = 0;
            double rng_nTankStableValue = 0;
            string rng_strTankRange = "-";
            int rng_nTankStableCTimeUse = -1;
            int rng_nTankStableCTime = 0;
            string rng_strTankStableUpdateTime = "-";
                        
            foreach (CommonFunction.TankInfo tankInfo in MainForm.Instance.tankInfo)
            {
                if (tankInfo.nTankID == this.nTankID)
                { 
                    if (tankInfo.strLiquidType == "황산")
                    {
                        if (!tankInfo.bIsLeakStatus && tankInfo.bIsLeakMonitoring)
                        {
                            pictureBox_leakStatus.Image = GetTankImage("Wifi");
                            pictureBox_leakStatus.Visible = true;
                        }
                        else if ((tankInfo.bIsLeakStatus && !tankInfo.bIsLeakMonitoring) || tankInfo.bIsLeakStatus && tankInfo.bIsLeakMonitoring)
                        {
                            pictureBox_leakStatus.Image = GetTankImage("LeakAlarm");
                            pictureBox_leakStatus.Visible = true;
                        }
                        else if (!tankInfo.bIsLeakMonitoring && !tankInfo.bIsLeakStatus)
                        {
                            pictureBox_leakStatus.Image = GetTankImage("NoWifi");
                            pictureBox_leakStatus.Visible = true;
                        }
                        else
                        {
                            pictureBox_leakStatus.Visible = false;
                        }
                    }

                    nStandardFlow = tankInfo.nStandardFlow;

                    if (tankInfo.strLiquidType == "N-BUTANOL") label_liquidType.Text = "BUTANOL";
                    else if (tankInfo.strLiquidType == "메틸렌클로라이드") label_liquidType.Text = "MC";
                    else label_liquidType.Text = tankInfo.strLiquidType;

                    label_tankName.Text = "TK-" + tankInfo.strTankName + tankInfo.strType;

                    if (tankInfo.nFlow != -999 || tankInfo.nFlow != -9999) 
                    {
                        label_flow.Text = String.Format("{0:F1}", tankInfo.nFlow);
                        label_flow2.Text = String.Format("{0:F1}", tankInfo.nFlow);
                    }

                    if (tankInfo.nDensity != -999 && tankInfo.nDensity != -9999) 
                        label_density.Text = String.Format("{0:F2}", tankInfo.nDensity);

                    if (tankInfo.nTemp != -999 && tankInfo.nTemp != -9999) 
                        label_temp.Text = String.Format("{0:F1}", tankInfo.nTemp);

                    if (tankInfo.nMass != -999 && tankInfo.nMass != -9999)
                        //label_mass.Text = String.Format("{0:##,##.#}", tankInfo.nMass);
                    {
                        string strMass = String.Format("{0:##,##.#}", tankInfo.nMass);
                        if (strMass.Substring(0, 1) == ".")
                            strMass = "0" + strMass;
                        label_mass.Text = strMass; 
                    }                        

                    if (tankInfo.nCurLevel != -999 && tankInfo.nCurLevel != -9999) 
                        label_curLevel.Text = String.Format("{0:F1}", tankInfo.nCurLevel);

                    if (tankInfo.nHighLevel != -999 && tankInfo.nHighLevel != -9999) 
                        label_highLevel.Text = "(m, " + String.Format("{0:F1}", tankInfo.nHighLevel) + ")"; 

                    if (tankInfo.nMinTemp == -999 || tankInfo.nMaxTemp == -999)
                        label_tempRange.Text = "( - )";
                    else
                        label_tempRange.Text = "(" + tankInfo.nMinTemp + " ~ " + tankInfo.nMaxTemp + ")";

                    if (tankInfo.nFlow > 10)
                        label_workStatus.Text = "입고중";
                    else if (tankInfo.nFlow < -10 && tankInfo.nFlow != -999)
                        label_workStatus.Text = "출고중"; 

                    if (tankInfo.nCurLevel == -999 || tankInfo.nHighLevel == -999 || tankInfo.nFlow == -999)
                    {
                        pictureBox_tank.Image = GetTankImage("TankDetailNormal0");
                        //return;
                    }

                    int nLevelPer = 0;
                    double dd = Math.Round((tankInfo.nCurLevel / tankInfo.nHighLevel) * 100);
                    double dd2 = dd % 5;
                    if (dd2 > 2.5)
                        nLevelPer = Convert.ToInt32(dd + (5 - dd2));
                    else
                        nLevelPer = Convert.ToInt32(dd - dd2);

                    if (nLevelPer > 0 && nLevelPer <= 100)
                    {
                        if (tankInfo.nFlow > 10)
                            pictureBox_tank.Image = GetTankImage("TankDetailUp" + nLevelPer);
                        else if (tankInfo.nFlow < -10)
                            pictureBox_tank.Image = GetTankImage("TankDetailDown" + nLevelPer);
                        else
                            pictureBox_tank.Image = GetTankImage("TankDetailNormal" + nLevelPer);
                    }
                    else if (nLevelPer > 100)
                    {
                        if (tankInfo.nFlow > 10)
                            pictureBox_tank.Image = GetTankImage("TankDetailUp100");
                        else if (tankInfo.nFlow < -10)
                            pictureBox_tank.Image = GetTankImage("TankDetailDown100");
                        else
                            pictureBox_tank.Image = GetTankImage("TankDetailNormal100");
                    }
                    else
                        pictureBox_tank.Image = GetTankImage("TankDetailNormal0");
                    break;
                }
            }

            foreach (CommonFunction.PipeInfo pipeInfo in MainForm.Instance.pipeInfo)
            {
                if (pipeInfo.nPipeID == this.nPipeID)
                { 
                    nStandardPressure = pipeInfo.nStandardPressure;
                    if (pipeInfo.nPressure != -999 && pipeInfo.nPressure != -9999) 
                        label_pressure.Text = String.Format("{0:F1}", pipeInfo.nPressure);
                    break;
                }
            }            
             
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT StandardPressureUpdateTime, StandardFlowUpdateTime, BeginTime ");
            sb.Append("  FROM LastWorkHistory"); 
            sb.Append(" WHERE EndTime IS NULL");
            if (this.nPipeID > 0)
                sb.Append("   AND PipeID=" + this.nPipeID); 
            if (this.nTankID > 0)
                sb.Append("   AND TankID=" + this.nTankID);

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult != null && arrResult.Count == 3)
            {
                rng_strPipeStableUpdateTime = (arrResult[0].ToString() == "null") ? "" : Convert.ToDateTime(arrResult[0]).ToString("HH시mm분");
                rng_strTankStableUpdateTime = (arrResult[1].ToString() == "null") ? "" : Convert.ToDateTime(arrResult[1]).ToString("HH시mm분");
                m_recentBeginTime = DBUtility.WebDBManager.GetDateTimeField(arrResult[2]);
                label_beginWorkTime.Text = m_recentBeginTime.Data.ToString("yyyy-MM-dd HH:mm:ss");
            }
             
            SetWorkTime();

            foreach (CommonFunction.AlarmPipeOptionInfo item in MainForm.Instance.alarmPipeOptionInfo)
            {
                if (item.nPipeID != this.nPipeID) continue;

                chart_pressure.ChartAreas[0].AxisY.StripLines.Clear();

                if (nStandardPressure != -9999 && nStandardPressure != -999)
                {
                    double minStripLine = 0;
                    double maxStripLine = 0;

                    rng_nPipeStableType = item.nPipeStableType;
                    if (item.nPipeStableType == 0) // 비율 사용
                    {
                        minStripLine = nStandardPressure - ((nStandardPressure * item.nPipeStableRatio) / 100);
                        maxStripLine = nStandardPressure + ((nStandardPressure * item.nPipeStableRatio) / 100);

                        rng_nPipeStableValue = item.nPipeStableRatio;
                    }
                    else if (item.nPipeStableType == 1) // 절대값 사용
                    {
                        minStripLine = nStandardPressure - item.nPipeStableAbsolute;
                        maxStripLine = nStandardPressure + item.nPipeStableAbsolute;

                        rng_nPipeStableValue = item.nPipeStableAbsolute;
                    }

                    if (minStripLine < 0)
                        minStripLine = 0;

                    chart_pressure.ChartAreas[0].AxisY.StripLines.Add(MainForm.Instance.commonFunction.GetStripLine(maxStripLine, StringAlignment.Far, "Max"));
                    chart_pressure.ChartAreas[0].AxisY.StripLines.Add(MainForm.Instance.commonFunction.GetStripLine(minStripLine, StringAlignment.Near, "Min"));
                    rng_strPipeRange = String.Format("{0:F1}", minStripLine) + " ~ " + String.Format("{0:F1}", maxStripLine);


                    double minNum = nStandardPressure - ((nStandardPressure - minStripLine) * 2);
                    if (minNum < 0)
                        minNum = 0;
                    double maxNum = nStandardPressure + ((maxStripLine - nStandardPressure) * 2);

                    double convertMinNum = Math.Round(minNum, 1);
                    double convertMaxNum = Math.Round(maxNum, 1);

                    if (convertMinNum == convertMaxNum)
                    {
                        if (minStripLine - 1 <= 0)
                            chart_pressure.ChartAreas[0].AxisY.Minimum = 0;
                        else
                            chart_pressure.ChartAreas[0].AxisY.Minimum = minStripLine - 1;
                        chart_pressure.ChartAreas[0].AxisY.Maximum = maxStripLine + 1;
                    }
                    else if (minNum > maxNum)
                    {
                        chart_pressure.ChartAreas[0].AxisY.Minimum = 0;
                        chart_pressure.ChartAreas[0].AxisY.Maximum = 1;
                    }
                    else
                    {
                        if (minNum > 0)
                            chart_pressure.ChartAreas[0].AxisY.Minimum = convertMinNum;
                        else
                            chart_pressure.ChartAreas[0].AxisY.Minimum = 0;

                        if (maxNum > 0)
                            chart_pressure.ChartAreas[0].AxisY.Maximum = convertMaxNum;
                        else
                            chart_pressure.ChartAreas[0].AxisY.Maximum = 1;
                    }
                }
                else
                {
                    //현재 차트에 표현되는 데이터 기준으로 차트 범위 설정
                    if (displayChartData != null && displayChartData.Count > 0)
                    {
                        double max = Math.Round(displayChartData.Max(p => p.dPressure), 1);
                        if (max == 0)
                            chart_pressure.ChartAreas[0].AxisY.Maximum = 1;
                        else
                            chart_pressure.ChartAreas[0].AxisY.Maximum = max + 0.5;

                        double minVal = Math.Round(displayChartData.Min(p => p.dPressure), 1) - 0.5;
                        if (minVal < 0)
                            chart_pressure.ChartAreas[0].AxisY.Minimum = 0;
                        else
                            chart_pressure.ChartAreas[0].AxisY.Minimum = minVal;
                    }
                }

                rng_nPipeStableType = item.nPipeStableType;
                rng_nPipeStableValue = (rng_nPipeStableType == 0) ? item.nPipeStableRatio : item.nPipeStableAbsolute;
                rng_nPipeStableCTimeUse = item.nPipeStableCTimeUse;
                rng_nPipeStableCTime = item.nPipeStableCTime;

                if (this.nPipeID > 0)
                    label_pipeRange2.Text = string.Format("{0}{1} | 설정:{2} | 유지:{3}", rng_nPipeStableValue, (rng_nPipeStableType == 0) ? "%" : "kg/cm²", rng_strPipeStableUpdateTime, (rng_nPipeStableCTimeUse == 0) ? "-" : rng_nPipeStableCTime + "분");
                label_pipeRange.Text = rng_strPipeRange;

                break;
            }

            foreach (CommonFunction.AlarmTankOptionInfo item in MainForm.Instance.alarmTankOptionInfo)
            {
                if (item.nTankID != this.nTankID) continue;

                chart_flow.ChartAreas[0].AxisY.StripLines.Clear();

                if (nStandardFlow != -9999 && nStandardFlow != -999)
                {
                    double minStripLine = 0;
                    double maxStripLine = 0;
                    string strStable = "";
                    if (item.nTankStableType == 0) // 비율 사용
                    {
                        minStripLine = nStandardFlow - Math.Abs((nStandardFlow * item.nTankStableRatio) / 100);
                        maxStripLine = nStandardFlow + Math.Abs((nStandardFlow * item.nTankStableRatio) / 100);

                        label_flowRange2.Text = item.nTankStableRatio + "%";
                    }
                    else if (item.nTankStableType == 1) // 절대값 사용
                    {
                        minStripLine = nStandardFlow - item.nTankStableAbsolute;
                        maxStripLine = nStandardFlow + item.nTankStableAbsolute;

                        label_flowRange2.Text = item.nTankStableAbsolute + "kl/h";
                    }

                    if (m_recentBeginTime != null)
                    {
                        chart_flow.ChartAreas[0].AxisY.StripLines.Add(MainForm.Instance.commonFunction.GetStripLineFlow(Math.Round(maxStripLine, 1), StringAlignment.Far, "Max"));
                        chart_flow.ChartAreas[0].AxisY.StripLines.Add(MainForm.Instance.commonFunction.GetStripLineFlow(Math.Round(minStripLine, 1), StringAlignment.Near, "Min"));
                        rng_strTankRange = Math.Round(minStripLine, 1) + " ~ " + Math.Round(maxStripLine, 1) + strStable;
                    }

                    double minNum = nStandardFlow - ((nStandardFlow - minStripLine) * 2);
                    double maxNum = nStandardFlow + ((maxStripLine - nStandardFlow) * 2);

                    double convertMinNum = Math.Round(minNum, 1);
                    double convertMaxNum = Math.Round(maxNum, 1);

                    if (convertMinNum == convertMaxNum)
                    {
                        chart_flow.ChartAreas[0].AxisY.Minimum = minStripLine - 1;
                        chart_flow.ChartAreas[0].AxisY.Maximum = maxStripLine + 1;
                    }
                    else if (minNum > maxNum)
                    {
                        chart_flow.ChartAreas[0].AxisY.Minimum = 0;
                        chart_flow.ChartAreas[0].AxisY.Maximum = 1;
                    }
                    else
                    {
                        chart_flow.ChartAreas[0].AxisY.Minimum = convertMinNum;
                        chart_flow.ChartAreas[0].AxisY.Maximum = convertMaxNum;
                    }
                }
                else
                {
                    //현재 차트에 표현되는 데이터 기준으로 차트 범위 설정
                    if (displayChartData != null && displayChartData.Count > 0)
                    {
                        double max = Math.Round(displayChartData.Max(p => p.dFlow), 1);
                        if (max == 0)
                            chart_flow.ChartAreas[0].AxisY.Maximum = 1;
                        else
                            chart_flow.ChartAreas[0].AxisY.Maximum = max + 0.5;

                        double minVal = Math.Round(displayChartData.Min(p => p.dFlow), 1) - 0.5;
                        chart_flow.ChartAreas[0].AxisY.Minimum = minVal;
                    }
                }

                rng_nTankStableType = item.nTankStableType;
                rng_nTankStableValue = (rng_nTankStableType == 0) ? item.nTankStableRatio : item.nTankStableAbsolute;
                rng_nTankStableCTimeUse = item.nTankStableCTimeUse;
                rng_nTankStableCTime = item.nTankStableCTime;

                label_flowRange2.Text = string.Format("{0}{1} | 설정:{2} | 유지:{3}", rng_nTankStableValue, (rng_nTankStableType == 0) ? "%" : "kl/h", rng_strTankStableUpdateTime, (rng_nTankStableCTimeUse == 0) ? "-" : rng_nTankStableCTime + "분");
                label_flowRange.Text = rng_strTankRange;
                break;
            } 
            
        } 

        private void SetWorkTime()
        {
            if (m_recentBeginTime == null)
            {
                label_workTime.Text = "-";
                label_beginWorkTime.Text = "-";
            }
            else
            {
                TimeSpan span = MainForm.Instance.SystemNow - m_recentBeginTime.Data;

                int nTotalSeconds = (int)span.TotalSeconds;
                int nHour = nTotalSeconds / 3600;
                int nMin = (nTotalSeconds - nHour * 3600) / 60;
                int nSec = nTotalSeconds - nHour * 3600 - nMin * 60;

                label_workTime.Text = string.Format("{0:00}:{1:00}:{2:00}", nHour, nMin, nSec); 
                label_beginWorkTime.Text = m_recentBeginTime.Data.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        #endregion  
         
        #region 알람 상태
         
        private void SettingStatus()
        {
            List<CommonFunction.AllAlarm> newAlarmInfo = MainForm.Instance.newAlarmInfo.Where(p => p.nTankID == this.nTankID).ToList();

            bool isAlarm = false; // 알람이 있는지 
            bool isChgAlarm = false; // 알람이 변경됐는지 (해제, 신규)

            if (newAlarmInfo.Count > 0)
                isAlarm = true;

            if (oldAlarmList.Count != newAlarmInfo.Count)
                isChgAlarm = true;

            foreach (CommonFunction.AllAlarm newInfo in newAlarmInfo)
            {
                int cnt = oldAlarmList.Where(p => p.nAlarmHistoryID == newInfo.nAlarmHistoryID).Count();
                if (cnt == 0)
                    isChgAlarm = true;
            }
             
            // 작업이 새로 시작되거나 종료된 경우, 알람이 생기거나 해제된 경우, 알람 내용이 변경된 경우
            if (isAlarm != this.isAlarm || isChgAlarm)
            {
                List<int> nsumAlarmType = new List<int>();
                foreach (CommonFunction.AllAlarm item in newAlarmInfo)
                { 
                    if (item.nTankID != this.nTankID) continue;
                    if (item.nAlarmHistoryID <= 0) continue;

                    if (!nsumAlarmType.Contains(item.nAlarmType))
                        nsumAlarmType.Add(item.nAlarmType);
                }

                if (isAlarm)
                {
                    //if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                    //{
                    //    // 유량
                    //    pictureBox_alarmPressure.Visible = false;
                    //    pictureBox_alarmFlow.Visible = true;
                    //}
                    //else if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                    //{
                    //    // 압력
                    //    pictureBox_alarmPressure.Visible = true;
                    //    pictureBox_alarmFlow.Visible = false;
                    //}
                    //else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                    //                                && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                    //{
                    //    // 유량, 압력 
                    //    pictureBox_alarmPressure.Visible = true;
                    //    pictureBox_alarmFlow.Visible = true;
                    //}

                    #region 황산
                    if (nsumAlarmType.Count == 1 && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산
                        panel1.BackgroundImage = GetTankImage("Tank_Work_Liquid");
                        pictureBox_alarmPressure.Visible = false;
                    }
                    else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 온도
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidTemp");
                        pictureBox_alarmPressure.Visible = false;
                    }
                    else if (nsumAlarmType.Count == 2 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 레벨
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevel");
                        pictureBox_alarmPressure.Visible = false;
                    }
                    else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 유량
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidFlow");
                        pictureBox_alarmPressure.Visible = false;
                    }
                    else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_Liquid");
                        pictureBox_alarmPressure.Visible = true;
                    }
                    else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                      && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨) && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 온도, 레벨
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelTemp");
                        pictureBox_alarmPressure.Visible = false;
                    }
                    else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                      && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 온도, 유량
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidTempFlow");
                        pictureBox_alarmPressure.Visible = false;
                    }
                    else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                      && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 온도, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidTemp");
                        pictureBox_alarmPressure.Visible = true;
                    }

                    else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                      && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨) && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 레벨, 유량
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelFlow");
                        pictureBox_alarmPressure.Visible = false;
                    }
                    else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                                                      && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 레벨, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevel");
                        pictureBox_alarmPressure.Visible = true;
                    }
                    else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                      && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 유량, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidFlow");
                        pictureBox_alarmPressure.Visible = true;
                    }
                    else if (nsumAlarmType.Count == 4 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                      && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                      && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 온도, 레벨, 유량
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelTempFlow");
                        pictureBox_alarmPressure.Visible = false;
                    }
                    else if (nsumAlarmType.Count == 4 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                      && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                      && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 온도, 레벨, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelTemp");
                        pictureBox_alarmPressure.Visible = true;
                    }
                    else if (nsumAlarmType.Count == 4 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                      && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                      && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 온도, 유량, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidTempFlow");
                        pictureBox_alarmPressure.Visible = true;
                    }
                    else if (nsumAlarmType.Count == 4 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                      && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                      && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 레벨, 유량, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelFlow");
                        pictureBox_alarmPressure.Visible = true;
                    }
                    else if (nsumAlarmType.Count == 5 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                      && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                      && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                      && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                      && nsumAlarmType.Contains((int)AlarmType.황산누출))
                    {
                        // 황산, 온도, 레벨, 유량, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LiquidLevelTempFlow");
                        pictureBox_alarmPressure.Visible = true;
                    }
                    #endregion

                    else if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강)))
                    {
                        // 온도                        
                        panel1.BackgroundImage = GetTankImage("Tank_Work_Temp");
                        pictureBox_alarmPressure.Visible = false; 
                    }
                    else if (nsumAlarmType.Count == 1 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                    {
                        // 레벨 
                        panel1.BackgroundImage = GetTankImage("Tank_Work_Level");
                        pictureBox_alarmPressure.Visible = false; 
                    }
                    else if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                    {
                        // 유량
                        panel1.BackgroundImage = GetTankImage("Tank_Work_Flow");
                        pictureBox_alarmPressure.Visible = false; 
                    }
                    else if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                    {
                        // 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work");
                        pictureBox_alarmPressure.Visible = true; 
                    }
                    else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                  && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                    {
                        // 온도, 레벨
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LevelTemp");
                        pictureBox_alarmPressure.Visible = false; 
                    }
                    else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                    {
                        // 온도, 유량
                        panel1.BackgroundImage = GetTankImage("Tank_Work_TempFlow");
                        pictureBox_alarmPressure.Visible = false; 
                    }
                    else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                    {
                        // 온도, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_Temp");
                        pictureBox_alarmPressure.Visible = true; 
                    }
                    else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                  && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                    {
                        // 레벨, 유량 
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LevelFlow");
                        pictureBox_alarmPressure.Visible = false; 
                    }
                    else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                                         && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                    {
                        // 레벨, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_Level");
                        pictureBox_alarmPressure.Visible = true; 
                    }
                    else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                         && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                    {
                        // 유량, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_Flow");
                        pictureBox_alarmPressure.Visible = true; 
                    }
                    else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                  && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                  && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                    {
                        // 온도, 레벨, 유량
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LevelTempFlow");
                        pictureBox_alarmPressure.Visible = false; 
                    }
                    else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                  && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                  && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨))
                    {
                        // 온도, 레벨, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LevelTemp");
                        pictureBox_alarmPressure.Visible = true; 
                    }
                    else if (nsumAlarmType.Count == 3 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                  && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                  && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                    {
                        // 온도, 유량, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_TempFlow");
                        pictureBox_alarmPressure.Visible = true; 
                    }
                    else if (nsumAlarmType.Count == 3 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                  && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                  && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                    {
                        // 레벨, 유량, 압력 
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LevelFlow");
                        pictureBox_alarmPressure.Visible = true; 
                    }
                    else if (nsumAlarmType.Count == 4 && nsumAlarmType.Contains((int)AlarmType.탱크최고레벨)
                                                      && (nsumAlarmType.Contains((int)AlarmType.탱크온도상승) || nsumAlarmType.Contains((int)AlarmType.탱크온도하강))
                                                      && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강))
                                                      && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                    {
                        // 온도, 레벨, 유량, 압력
                        panel1.BackgroundImage = GetTankImage("Tank_Work_LevelTempFlow");
                        pictureBox_alarmPressure.Visible = true; 
                    } 

                    pictureBox_clearAlarm.Visible = true;
                }
                else
                {
                    panel1.BackgroundImage = GetTankImage("Tank_Work");
                    pictureBox_alarmPressure.Visible = false; 
                    pictureBox_clearAlarm.Visible = false;
                }

                oldAlarmList = newAlarmInfo;

                this.isAlarm = isAlarm;
            } 
        } 
        #endregion

        #region 페이지 버튼 
        private void pictureBox_doubleLeft_Click(object sender, EventArgs e)
        {
            if (minPage == maxPage || minPage == curPage) return;

            if (minPage < curPage)
            {
                curPage = minPage;
                InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);
                chart_pressure.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);
                chart_flow.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);

                label_curPage.Text = curPage.ToString();
                label_maxPage.Text = string.Format("/{0}", dicCharts.Count); 
            }
        }

        private void pictureBox_left_Click(object sender, EventArgs e)
        {
            if (curPage > minPage)
            {
                curPage--;

                InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);
                chart_pressure.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);
                chart_flow.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);

                label_curPage.Text = curPage.ToString();
                label_maxPage.Text = string.Format("/{0}", dicCharts.Count); 
            }
        }

        private void pictureBox_right_Click(object sender, EventArgs e)
        {
            if (curPage < maxPage)
            {
                curPage++;

                InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);
                chart_pressure.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);
                chart_flow.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);

                label_curPage.Text = curPage.ToString();
                label_maxPage.Text = string.Format("/{0}", dicCharts.Count); 
            }
        }

        private void pictureBox_doubleRight_Click(object sender, EventArgs e)
        {
            if (minPage == maxPage || maxPage == curPage) return;

            if (maxPage > curPage)
            {
                curPage = maxPage;
                 
                InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);
                chart_pressure.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);
                chart_flow.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);

                label_curPage.Text = curPage.ToString();
                label_maxPage.Text = string.Format("/{0}", dicCharts.Count); 
            }
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            int selectedPage = 0;
            if (!int.TryParse(label_curPage.Text, out selectedPage)) return;

            if (selectedPage > maxPage || selectedPage < minPage || selectedPage == curPage) return;
            curPage = selectedPage;
             
            InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);
            chart_pressure.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);
            chart_flow.DataSource = dicCharts[curPage].OrderBy(p => p.dtTimeStamp);

            label_curPage.Text = curPage.ToString();
            label_maxPage.Text = string.Format("/{0}", dicCharts.Count);
        }
        void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            int keyCode = (int)e.KeyChar;  // 46: Point  
            if ((keyCode < 48 || keyCode > 57) && keyCode != 8 && keyCode != 45)
            {
                e.Handled = true;
            }
        } 
        #endregion 

        #region 알람
        private void pictureBox_clearAlarm_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            bool buttonStatus = false;
            string msg = "알람을 해제하시겠습니까?";

            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData("SELECT PropertyValue FROM Options WHERE PropertyName='ButtonStatus'", 0);
            if (arrResult != null)
            {
                int result = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 1);
                if (result == 0) buttonStatus = true;
            }

            //UnE.Utility.UMessageBox.FrameColor = Color.FromArgb(1, 4, 20);
            //if (buttonStatus)
            //    msg = "알람을 해제하시겠습니까?\r함체박스의 Push 버튼이 눌려져 있으므로 알람을 해제해도 경광등은 꺼지지 않습니다.\r경광등을 끄기 위해서는 함체박스의 Push버튼을 다시 눌러주시기 바랍니다.";

            //if (UnE.Utility.UMessageBox.Show(this, msg, "알람 해제", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;
            KpxPipeMonitoring.Popups.AlarmClear ac = new Popups.AlarmClear(msg);
            ac.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = ac.ShowDialog();
            if (dr == DialogResult.OK)
                AlarmClear(ac.occurenceType, ac.comment);
        }

        public void AlarmClear(int occurType, string comment)
        {
            int nCmdID = MainForm.Instance.commonFunction.GetMaxTableID("Command") + 1;
            int nCmdHistoryID = MainForm.Instance.commonFunction.GetMaxTableID("CommandHistory") + 1;

            foreach (CommonFunction.AllAlarm item in oldAlarmList)
            {
                int commandType = -1;
                if (item.nPipeID > 0)
                    commandType = 0;
                else
                {
                    if (item.nAlarmType < 0)
                        commandType = 11; // 황산 누출
                    else
                        commandType = 2;
                }
                item.nAlarmOccurType = occurType;
                item.strAlarmComment = comment;

                StringBuilder sb = new StringBuilder();

                if (commandType == 0)
                {
                    sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
                    sb.Append("VALUES(" + nCmdID + ", 0, now(), " + item.nPipeID + ", " + item.nTankID + ", " + MainForm.Instance.nUserID + ") ");
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                    sb = new StringBuilder();
                    sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID, AlarmOccurType, alarmComment, AlarmHistoryID) ");
                    sb.AppendFormat("VALUES ({0}, 0, now(), NULL, {1}, {2}, {3}, {4}, {5}, '{6}', {7})", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, item.nPipeID, this.nTankID, item.nAlarmOccurType, item.strAlarmComment, item.nAlarmHistoryID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                }
                else if (commandType == 2)
                {
                    sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID) ");
                    sb.Append("VALUES(" + nCmdID + ", 0, now(), " + nTankID + ", " + MainForm.Instance.nUserID + ") ");
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                    sb = new StringBuilder();
                    sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, AlarmOccurType, alarmComment, AlarmHistoryID) ");
                    sb.AppendFormat("VALUES ({0}, 0, now(), NULL, {1}, {2}, {3}, {4}, '{5}', {6})", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, this.nTankID, item.nAlarmOccurType, item.strAlarmComment, item.nAlarmHistoryID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                }
                else if (commandType == 11)
                {
                    // 황산 누출 해제

                    // Buzzer OFF
                    sb = new StringBuilder();
                    sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID, CommandValue) ");
                    sb.AppendFormat("VALUES ({0}, 11, now(), {1}, {2}, 1)", nCmdID, item.nTankID, MainForm.Instance.nUserID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                    sb = new StringBuilder();
                    sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, CommandValue, AlarmOccurType, alarmComment, AlarmHistoryID) ");
                    sb.AppendFormat("VALUE ({0},11,now(),null,{1},{2},{3},1,{4},'{5}',{6}) ", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, item.nTankID, item.nAlarmOccurType, item.strAlarmComment, item.nAlarmHistoryID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                    nCmdID++;
                    nCmdHistoryID++;

                    //Reset
                    sb = new StringBuilder();
                    sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID, CommandValue) ");
                    sb.AppendFormat("VALUES ({0}, 13, now(), {1}, {2}, 0) ", nCmdID, item.nTankID, MainForm.Instance.nUserID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                    sb = new StringBuilder();
                    sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, CommandValue, AlarmHistoryID) ");
                    sb.AppendFormat("VALUE ({0},13,now(),null,{1},{2},{3},0,{4}) ", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, item.nTankID, item.nAlarmHistoryID);
                    MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0); 
                }

                nCmdID++;
                nCmdHistoryID++;
            }
        } 
        #endregion

        #region 정상범위 새로고침
        private void pictureBox_rangeRefresh_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.nTankID < 0) return;

            if (UnE.Utility.UMessageBox.Show(this, "현재값을 기준으로 압력과 유량의 정상범위를 새로 설정하시겠습니까?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;

            int nCmdID = MainForm.Instance.commonFunction.GetMaxTableID("Command") + 1;
            int nCmdHistoryID = MainForm.Instance.commonFunction.GetMaxTableID("CommandHistory") + 1;

            StringBuilder sb = null;

            foreach (int pipeId in nPipeIDs)
            {
                int connectPipeID = -1;
                if (pipeId > 0) connectPipeID = pipeId;

                sb = new StringBuilder();
                sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
                sb.Append("VALUES(" + nCmdID + ", 8, now(), " + connectPipeID + ", " + this.nTankID + ", " + MainForm.Instance.nUserID + ") ");
                MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                sb = new StringBuilder();
                sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID) ");
                sb.AppendFormat("VALUES ({0}, 8, now(), NULL, {1}, {2}, {3}, {4})", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, connectPipeID, this.nTankID);
                MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                nCmdID++;
                nCmdHistoryID++;
            }
        } 
        #endregion
        
        private void DisplayConnectedIDs()
        {
            int connectTankId = -1;
            List<int> tempPipeIds = new List<int>();

            if (strBeginPage == "P")
            {
                connectTankId = MainForm.Instance.commonFunction.ReturnConnectTankIDs(this.nBeginPageID); 
                tempPipeIds.Add(this.nBeginPageID);
            }
            else if (strBeginPage == "T")
            {
                connectTankId = this.nBeginPageID;                
            }

            foreach (CommonFunction.TankInfo tankInfo in MainForm.Instance.tankInfo)
            {
                if (tankInfo.nTankID == connectTankId)
                { 
                    tempPipeIds = tankInfo.nConnectPipeIDs;
                    break;
                }
            }

            this.nTankID = connectTankId;
            this.nPipeIDs = tempPipeIds;

            
            for (int i = 0; i < tempPipeIds.Count; i++)
            {
                foreach (CommonFunction.PipeInfo pipeInfo in MainForm.Instance.pipeInfo)
                {
                    if (tempPipeIds[i] == pipeInfo.nPipeID)
                    {
                        if (i == 0)
                        {
                            if (tempPipeIds.Count == 2)
                                radioButton_pipeId1.Visible = true;
                            else
                                radioButton_pipeId1.Visible = false;
                            radioButton_pipeId1.Tag = pipeInfo.nPipeID;
                            radioButton_pipeId1.Text = pipeInfo.strPipeName;
                            label_pipeName.Text = pipeInfo.strPipeName;
                        }
                        else
                        {
                            if (tempPipeIds.Count == 2)
                            {
                                radioButton_pipeId2.Visible = true;
                                radioButton_pipeId2.Tag = pipeInfo.nPipeID;
                                radioButton_pipeId2.Text = pipeInfo.strPipeName;
                            }
                            label_pipeName.Text += "\r\n" + pipeInfo.strPipeName;
                        }

                        break;
                    }
                }
            } 
  
            if (tempPipeIds.Count == 0)
            {
                radioButton_pipeId1.Tag = null;
                radioButton_pipeId1.Visible = false;
                radioButton_pipeId2.Tag = null;
                radioButton_pipeId2.Visible = false;
                this.nPipeID = -1;
                label_pipeName.Text = "-"; 
            }
            else if (tempPipeIds.Count == 1)
            { 
                radioButton_pipeId1.Tag = null;
                radioButton_pipeId1.Visible = false;
                radioButton_pipeId2.Tag = null;
                radioButton_pipeId2.Visible = false;

                this.nPipeID = tempPipeIds[0];

                if (tempPipeIds[0] == -200)
                    label_pipeName.Text = "황산";
                else if (tempPipeIds[0] == -100)
                    label_pipeName.Text = "PO";                
            } 
            else if (tempPipeIds.Count == 2)
            {
                if (radioButton_pipeId1.Checked)
                    this.nPipeID = (int)radioButton_pipeId1.Tag;
                else if (radioButton_pipeId2.Checked)
                    this.nPipeID = (int)radioButton_pipeId2.Tag;
                else if (!radioButton_pipeId1.Checked && !radioButton_pipeId2.Checked)
                {
                    if ((int)radioButton_pipeId1.Tag == this.nBeginPageID)
                    {
                        radioButton_pipeId1.Checked = true;
                        this.nPipeID = (int)radioButton_pipeId1.Tag;
                    }
                    else if ((int)radioButton_pipeId2.Tag == this.nBeginPageID)
                    {
                        radioButton_pipeId2.Checked = true;
                        this.nPipeID = (int)radioButton_pipeId2.Tag;
                    }
                }
            }

            if (this.nTankID != -1)
            {
                foreach (CommonFunction.TankInfo tankInfo in MainForm.Instance.tankInfo)
                {
                    if (tankInfo.nTankID == this.nTankID)
                    {
                        if (tankInfo.strLiquidType == "황산")
                        {
                            pictureBox_title.Visible = false;
                            pictureBox_title2.Visible = true;
                            label_pipeName.Visible = false;

                            label_tankName.Parent = pictureBox_title2;
                            label_tankName.Location = new Point(6, 8);

                            pictureBox_leakStatus.Location = new Point(1420, 542);
                            pictureBox_rangeRefresh.Location = new Point(pictureBox_leakStatus.Location.X + pictureBox_leakStatus.Size.Width + 10, 545);
                        }
                        else if (tankInfo.strLiquidType == "PO")
                        {
                            pictureBox_title.Visible = false;
                            pictureBox_title2.Visible = true;
                            label_pipeName.Visible = false;

                            label_tankName.Parent = pictureBox_title2;
                            label_tankName.Location = new Point(6, 8);
                        }
                        else
                        {
                            pictureBox_title.Visible = true;
                            pictureBox_title2.Visible = false;
                            label_pipeName.Visible = true;
                            label_tankName.Visible = true;

                            label_tankName.Parent = pictureBox_title;
                            label_tankName.Location = new Point(7, 9);
                            label_pipeName.Parent = pictureBox_title;
                            label_pipeName.Location = new Point(150, 8);
                        }
                        break;
                    }
                } 
            }
            else if (this.nTankID == -1)
            {
                pictureBox_title.Visible = true;
                pictureBox_title2.Visible = false;
                label_pipeName.Visible = true;
                label_tankName.Visible = true;
                label_tankName.Text = "-";

                label_tankName.Parent = pictureBox_title;
                label_tankName.Location = new Point(150, 8);
                label_pipeName.Parent = pictureBox_title;                
                label_pipeName.Location = new Point(7, 8);
            }
        } 

        private void ChartNull(Chart chart)
        { 
            List<CommonFunction.ChartField> temp = new List<KpxPipeMonitoring.CommonFunction.ChartField>();
            temp.Add(new CommonFunction.ChartField(0, 0, new DateTime(), 0, 0));

            chart.DataSource = temp;

            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            chart.ChartAreas[0].AxisY.Maximum = 1;
            chart.ChartAreas[0].AxisY.Minimum = 0; 
        }

        private Dictionary<string, Image> dicTankLevelImage = new Dictionary<string, Image>();
        private Image GetTankImage(string imgName)
        {
            if (dicTankLevelImage.ContainsKey(imgName))
                return dicTankLevelImage[imgName];
            else
                return dicTankLevelImage["TankDetailNormal0"];
        }
        private void LoadTankImage()
        {
            dicTankLevelImage.Add("TankDetailNormal0", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal0);
            dicTankLevelImage.Add("TankDetailNormal5", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal5);
            dicTankLevelImage.Add("TankDetailNormal10", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal10);
            dicTankLevelImage.Add("TankDetailNormal15", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal15);
            dicTankLevelImage.Add("TankDetailNormal20", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal20);
            dicTankLevelImage.Add("TankDetailNormal25", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal25);
            dicTankLevelImage.Add("TankDetailNormal30", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal30);
            dicTankLevelImage.Add("TankDetailNormal35", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal35);
            dicTankLevelImage.Add("TankDetailNormal40", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal40);
            dicTankLevelImage.Add("TankDetailNormal45", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal45);
            dicTankLevelImage.Add("TankDetailNormal50", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal50);
            dicTankLevelImage.Add("TankDetailNormal55", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal55);
            dicTankLevelImage.Add("TankDetailNormal60", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal60);
            dicTankLevelImage.Add("TankDetailNormal65", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal65);
            dicTankLevelImage.Add("TankDetailNormal70", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal70);
            dicTankLevelImage.Add("TankDetailNormal75", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal75);
            dicTankLevelImage.Add("TankDetailNormal80", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal80);
            dicTankLevelImage.Add("TankDetailNormal85", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal85);
            dicTankLevelImage.Add("TankDetailNormal90", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal90);
            dicTankLevelImage.Add("TankDetailNormal95", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal95);
            dicTankLevelImage.Add("TankDetailNormal100", global::KpxPipeMonitoring.Properties.Resources.TankDetailNormal100);

            dicTankLevelImage.Add("TankDetailUp5", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp5);
            dicTankLevelImage.Add("TankDetailUp10", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp10);
            dicTankLevelImage.Add("TankDetailUp15", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp15);
            dicTankLevelImage.Add("TankDetailUp20", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp20);
            dicTankLevelImage.Add("TankDetailUp25", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp25);
            dicTankLevelImage.Add("TankDetailUp30", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp30);
            dicTankLevelImage.Add("TankDetailUp35", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp35);
            dicTankLevelImage.Add("TankDetailUp40", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp40);
            dicTankLevelImage.Add("TankDetailUp45", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp45);
            dicTankLevelImage.Add("TankDetailUp50", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp50);
            dicTankLevelImage.Add("TankDetailUp55", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp55);
            dicTankLevelImage.Add("TankDetailUp60", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp60);
            dicTankLevelImage.Add("TankDetailUp65", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp65);
            dicTankLevelImage.Add("TankDetailUp70", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp70);
            dicTankLevelImage.Add("TankDetailUp75", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp75);
            dicTankLevelImage.Add("TankDetailUp80", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp80);
            dicTankLevelImage.Add("TankDetailUp85", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp85);
            dicTankLevelImage.Add("TankDetailUp90", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp90);
            dicTankLevelImage.Add("TankDetailUp95", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp95);
            dicTankLevelImage.Add("TankDetailUp100", global::KpxPipeMonitoring.Properties.Resources.TankDetailUp100);

            dicTankLevelImage.Add("TankDetailDown5", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown5);
            dicTankLevelImage.Add("TankDetailDown10", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown10);
            dicTankLevelImage.Add("TankDetailDown15", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown15);
            dicTankLevelImage.Add("TankDetailDown20", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown20);
            dicTankLevelImage.Add("TankDetailDown25", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown25);
            dicTankLevelImage.Add("TankDetailDown30", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown30);
            dicTankLevelImage.Add("TankDetailDown35", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown35);
            dicTankLevelImage.Add("TankDetailDown40", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown40);
            dicTankLevelImage.Add("TankDetailDown45", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown45);
            dicTankLevelImage.Add("TankDetailDown50", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown50);
            dicTankLevelImage.Add("TankDetailDown55", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown55);
            dicTankLevelImage.Add("TankDetailDown60", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown60);
            dicTankLevelImage.Add("TankDetailDown65", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown65);
            dicTankLevelImage.Add("TankDetailDown70", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown70);
            dicTankLevelImage.Add("TankDetailDown75", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown75);
            dicTankLevelImage.Add("TankDetailDown80", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown80);
            dicTankLevelImage.Add("TankDetailDown85", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown85);
            dicTankLevelImage.Add("TankDetailDown90", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown90);
            dicTankLevelImage.Add("TankDetailDown95", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown95);
            dicTankLevelImage.Add("TankDetailDown100", global::KpxPipeMonitoring.Properties.Resources.TankDetailDown100);

            dicTankLevelImage.Add("Tank_Work", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking);
            dicTankLevelImage.Add("Tank_Work_Level", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_Level);
            dicTankLevelImage.Add("Tank_Work_LevelTemp", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_LevelTemp);
            dicTankLevelImage.Add("Tank_Work_Temp", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_Temp);
            dicTankLevelImage.Add("Tank_Work_Flow", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_Flow);
            dicTankLevelImage.Add("Tank_Work_TempFlow", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_TempFlow);
            dicTankLevelImage.Add("Tank_Work_LevelFlow", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_LevelFlow);
            dicTankLevelImage.Add("Tank_Work_LevelTempFlow", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_LevelTempFlow);
            dicTankLevelImage.Add("Tank_Work_Liquid", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_Liquid);
            dicTankLevelImage.Add("Tank_Work_LiquidFlow", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_LiquidFlow);
            dicTankLevelImage.Add("Tank_Work_LiquidTemp", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_LiquidTemp);
            dicTankLevelImage.Add("Tank_Work_LiquidLevel", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_LiquidLevel);
            dicTankLevelImage.Add("Tank_Work_LiquidLevelTemp", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_LiquidLevelTemp);
            dicTankLevelImage.Add("Tank_Work_LiquidLevelTempFlow", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_LiquidLevelTempFlow);
            dicTankLevelImage.Add("Tank_Work_LiquidLevelFlow", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_LiquidLevelFlow);
            dicTankLevelImage.Add("Tank_Work_LiquidTempFlow", global::KpxPipeMonitoring.Properties.Resources.ChildDetailPipeWorking_LiquidTempFlow);  

            dicTankLevelImage.Add("Wifi", global::KpxPipeMonitoring.Properties.Resources.WIfi_Detail);
            dicTankLevelImage.Add("NoWifi", global::KpxPipeMonitoring.Properties.Resources.NoWifi_Detail);
            dicTankLevelImage.Add("LeakAlarm", global::KpxPipeMonitoring.Properties.Resources.LeakAlarm_Detail);
        }
    } 
}
