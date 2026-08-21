using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace KpxPipeMonitoring.ChildForms
{
    public partial class ChildDetailPipe : Form
    { 
        private int nPipeID { get; set; }

        Timer timer = null;
        private int curSec = 3;
        private int refreshSec = 6;
        private Dictionary<int, List<CommonFunction.ChartField>> dicCharts = new Dictionary<int, List<CommonFunction.ChartField>>();
        /// <summary>
        /// 그래프 조회 조건 (분 단위)
        /// </summary>
        private int nSearchCondition = 30; 
        private int minPage = 1;
        private int maxPage = 1;
        private int curPage = 1;
        /// <summary>
        /// 그래프 전체 데이터
        /// </summary>
        List<CommonFunction.ChartField> totalChartData = new List<CommonFunction.ChartField>();
        /// <summary>
        /// 표현할 그래프 데이터
        /// </summary>
        List<CommonFunction.ChartField> displayChartData = new List<CommonFunction.ChartField>();
                   
        private string normalRange { get { return label_avgPressure.Text; } set { label_avgPressure.Text = value; } }
        private string workTime { get { return label1.Text; } set { label1.Text = value; } }

        private IHistoryManager m_historyMgr = null; 

        #region 초기화
        public ChildDetailPipe(int pipeID)
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            this.Opacity = 0.8;
             
            this.nPipeID = pipeID;  
            m_historyMgr = new HistoryManager(MainForm.Instance);

            MainForm.Instance.SetDoubleBuffer(panel1, true);
             
            comboBox1.Items.Add(new ComboItem("최근 30분", 30));
            comboBox1.Items.Add(new ComboItem("최근 1시간", 60));
            comboBox1.Items.Add(new ComboItem("최근 3시간", 180));
            comboBox1.Items.Add(new ComboItem("최근 5시간", 300)); 
            
            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
            comboBox1.SelectedIndex = 0;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

            textBox1.KeyDown += textBox1_KeyDown;
            textBox1.KeyPress += (s, e) =>
                { 
                    int keyCode = (int)e.KeyChar;  // 46: Point  
                    if ((keyCode < 48 || keyCode > 57) && keyCode != 8 && keyCode != 45)
                    {
                        e.Handled = true;
                    } 
                };

            MainForm.Instance.commonFunction.SettingButton(pictureBox_close, KpxPipeMonitoring.Properties.Resources.Close, KpxPipeMonitoring.Properties.Resources.Close_MouseOver);
             
            InitChart();
            //DisplayJustBeforeWork();
            DisplayChart();
            DisplayPipeInfo(); 
             
            this.timer = new Timer();
            this.timer.Interval = 1000;
            this.timer.Tick += timer_Tick;
            this.timer.Start();
        } 

        private void InitChart()
        { 
            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart1.ChartAreas[0].AxisX.IsMarginVisible = false; 
            chart1.ChartAreas[0].AxisY.LabelStyle.Format = "F1";
            chart1.Legends.Clear();
        }
        private void InitSeries(DateTime beforeDate, DateTime afterDate)
        { 
            chart1.Series.Clear();
            Series series = chart1.Series.Add("series1");
            series.ChartType = SeriesChartType.Line;
            chart1.Series[0].IsXValueIndexed = true;
            chart1.Series[0].XValueMember = "dtTimeStamp";
            chart1.Series[0].YValueMembers = "dPressure";
            chart1.Series[0].BorderWidth = 3;
             
            string dateFormat = "HH:mm";

            DateTimeIntervalType IntervalType = MainForm.Instance.commonFunction.GetIntervalType(beforeDate, afterDate);
            //chart1.ChartAreas[0].AxisX.IntervalType = IntervalType;
            if (IntervalType == DateTimeIntervalType.Seconds)
                dateFormat = "HH:mm:ss";
            else if (IntervalType == DateTimeIntervalType.Minutes)
                dateFormat = "HH:mm:ss";
            else if (IntervalType == DateTimeIntervalType.Hours)
                dateFormat = "MM/dd\r\nHH:mm";
            else
                dateFormat = "MM/dd\r\nHH:mm";

            chart1.Series[0].XValueType = ChartValueType.DateTime;
            chart1.Series[0].ToolTip = "#VALX{" + dateFormat + "} - #VALY1{0.00}";
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = dateFormat; 
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            this.timer.Stop();
            this.timer.Dispose();
            this.Dispose();
        }
        #endregion

        #region 콤보 이벤트
        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {  
            curPage = 1;
            minPage = 1;
            maxPage = 1; 

            displayChartData.Clear();
            dicTempDatas.Clear();

            ComboItem selectedItem = (ComboItem)comboBox1.SelectedItem;
            nSearchCondition = Convert.ToInt32(selectedItem.Value);

            DisplayChart(); 
        }
        #endregion 

        #region 타이머 이벤트
        void timer_Tick(object sender, EventArgs e)
        {
            if (refreshSec == curSec)
            { 
                DisplayChart(); 
            }
            else
            { 
                curSec++;
            }
        }
        #endregion 

        #region 그래프 Display  
        Dictionary<DateTime, double> dicTempDatas = new Dictionary<DateTime, double>();
        private void DisplayChart()
        { 
            Cursor = Cursors.WaitCursor;
            try
            { 
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
                 
                #region 2. 파일로 읽기
                DateTime beforeDate = MainForm.Instance.SystemNow;
                beforeDate = beforeDate.AddDays(-2);
                DateTime afterDate = MainForm.Instance.SystemNow;
                
                List<HistoryQuery> historyQueries = new List<HistoryQuery>();                
                int totalDays = (int)(afterDate - beforeDate).TotalDays;
                for (int i = 0; i <= totalDays; i++)
                {
                    string y = beforeDate.AddDays(i).Year.ToString();
                    string m = beforeDate.AddDays(i).Month.ToString();
                    string d = beforeDate.AddDays(i).Day.ToString();

                    HistoryQuery query = new HistoryQuery(nPipeID, y, m, d, HistoryQueryType.압력);
                    historyQueries.Add(query);
                } 
                totalChartData = m_historyMgr.ReadHistory(historyQueries);
                historyQueries.Clear();
                historyQueries = null;

                //if (totalChartData != null && totalChartData.Count > 0)
                //    beforeDate = totalChartData[totalChartData.Count - 1].dtTimeStamp;
                //else
                //    beforeDate = DateTime.Now; 
                #endregion 
                 
                DateTime dtNow = MainForm.Instance.SystemNow;

                if (nSearchCondition > 0)
                {
                    int displayCondition = MainForm.Instance.commonFunction.GetChartPointCount(dtNow.AddMinutes(-nSearchCondition), dtNow);

                    // 조회 시점
                    DateTime searchDateTime = dtNow.AddMinutes(-nSearchCondition);

                    // 시간이 지난 데이터 제거
                    List<CommonFunction.ChartField> removeChartList = new List<CommonFunction.ChartField>();
                    foreach (CommonFunction.ChartField item in displayChartData)
                    {
                        if (item.dtTimeStamp < searchDateTime)
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
                        if (displayChartData.Count == 0 || displayChartData[displayChartData.Count - 1].dtTimeStamp < item.dtTimeStamp)
                        {
                            if (searchDateTime <= item.dtTimeStamp)
                            {
                                displayChartData.Add(new CommonFunction.ChartField(0, 0, item.dtTimeStamp, item.dPressure)); 
                            }
                        }
                    }

                    // 조회조건에 미치지 못해서 (ex: 30분일경우 3개, 1시간일경우 6개) 차트 데이터가 add되지 못한 경우
                    if (dicTempDatas.Count > 0)
                    {
                        Dictionary<DateTime, double> dicTempDatas2 = new Dictionary<DateTime, double>();
                        List<double> doubles2 = new List<double>();
                        DateTime beforeDate2 = dicTempDatas.Keys.Min();
                        DateTime afterDate2 = dicTempDatas.Keys.Max();
                        int displayCondition2 = MainForm.Instance.commonFunction.GetChartPointCount(beforeDate2, afterDate2);
                        foreach (KeyValuePair<DateTime, double> item in dicTempDatas)
                        {
                            if (item.Key >= searchDateTime && item.Key <= dtNow)
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
                                        foreach (KeyValuePair<DateTime, double> item2 in dicTempDatas2)
                                        {
                                            tempPressure = tempPressure + item2.Value; 
                                        }
                                        displayChartData.Add(new CommonFunction.ChartField(0, 0, item.Key, tempPressure / displayCondition2));
                                        dicTempDatas2.Clear();
                                    }
                                }
                            }
                        }
                    } 

                    if (displayChartData.Count == 0)
                    {
                        InitSeries(new DateTime(), new DateTime());
                        List<CommonFunction.ChartField> temp = new List<KpxPipeMonitoring.CommonFunction.ChartField>();
                        temp.Add(new CommonFunction.ChartField(0, 0, new DateTime(), 0));
                        chart1.DataSource = temp;

                        chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                        chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

                        chart1.ChartAreas[0].AxisY.Interval = 0;

                        chart1.ChartAreas[0].AxisY.Maximum = 1;
                        chart1.ChartAreas[0].AxisY.Minimum = 0;
                        return;
                    }
                                        
                    InitSeries(displayChartData[0].dtTimeStamp, displayChartData[displayChartData.Count - 1].dtTimeStamp);

                    chart1.DataSource = null; 
                    chart1.DataSource = displayChartData; 
                } 

                //현재 차트에 표현되는 데이터 기준으로 차트 범위 설정
                double max = Math.Round(displayChartData.Max(p => p.dPressure), 1);
                if (max == 0)
                    chart1.ChartAreas[0].AxisY.Maximum = 1;
                else
                    chart1.ChartAreas[0].AxisY.Maximum = max + 0.5;

                double minVal = Math.Round(displayChartData.Min(p => p.dPressure), 1) - 0.5;
                if (minVal < 0)
                    chart1.ChartAreas[0].AxisY.Minimum = 0;
                else
                    chart1.ChartAreas[0].AxisY.Minimum = minVal;
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
                curSec = 1;
            }
        }
        #endregion

        #region 배관정보 Display
        private void DisplayPipeInfo()
        {   
            StringBuilder sb = new StringBuilder();
            sb.Append("select Name, Type, Pressure, AvgPressure, AvgFlow, workTime ");
            sb.Append("  from (");
            sb.Append("    select Name, Type, Pressure, AvgPressure, AvgFlow, Concat(SEC_TO_TIME(sum(TIME_TO_SEC(timediff(EndTime, BeginTime)))),'') as workTime");
            sb.Append("      from Pipe as p LEFT OUTER JOIN lastworkHistory as lwh ON p.id=lwh.pipeid");
            sb.Append("     where endtime is not null");
            sb.Append("       and p.id=" + this.nPipeID);
            sb.Append("     order by endtime desc) as x limit 1");
             
            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
            if (arrResult == null) return;
             
            for (int i = 0; i < arrResult.Count; i += 6)
            {
                label_pipeName.Text = DBUtility.WebDBManager.GetStringField(arrResult[i]);
                label_type.Text = DBUtility.WebDBManager.GetStringField(arrResult[i + 1]);
                double nPressure = (arrResult[i + 2].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 2]);
                double nAvgPressure = (arrResult[i + 3].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 3]);
                double nAvgFlow = (arrResult[i + 4].ToString() == "null") ? -999 : Convert.ToDouble(arrResult[i + 4]); 
                string workTime = (arrResult[i + 5].ToString() == "null") ? "-" : DBUtility.WebDBManager.GetStringField(arrResult[i + 5]);
                 
                if (nAvgPressure == -999)
                    label_avgPressure.Text = "-";
                else label_avgPressure.Text = MainForm.Instance.commonFunction.removeTailZero(String.Format("{0:F2}", nAvgPressure));

                if (nAvgFlow == -999)
                    label_avgFlow.Text = "-";
                else label_avgFlow.Text = MainForm.Instance.commonFunction.removeTailZero(String.Format("{0:F2}", nAvgFlow));

                label_workTime.Text = workTime; 
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
                chart1.Series[0].Points.Clear();
                //chart1.ChartAreas[0].AxisY.Maximum = Math.Round(dicCharts[curPage].Max(p => p.dPressure)) + 0.5;
                //double minVal = Math.Round(dicCharts[curPage].Min(p => p.dPressure)) - 0.5;
                //if (minVal < 0)
                //    chart1.ChartAreas[0].AxisY.Minimum = 0;
                //else
                //    chart1.ChartAreas[0].AxisY.Minimum = minVal;

                InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);
                chart1.DataSource = dicCharts[curPage];

                textBox1.Text = curPage.ToString();
                label_maxPage.Text = string.Format("/{0}", dicCharts.Count);
            }
        }

        private void pictureBox_left_Click(object sender, EventArgs e)
        {
            if (curPage > minPage)
            {
                curPage--;
                //chart1.ChartAreas[0].AxisY.Maximum = Math.Round(dicCharts[curPage].Max(p => p.dPressure)) + 0.5;
                //double minVal = Math.Round(dicCharts[curPage].Min(p => p.dPressure)) - 0.5;
                //if (minVal < 0)
                //    chart1.ChartAreas[0].AxisY.Minimum = 0;
                //else
                //    chart1.ChartAreas[0].AxisY.Minimum = minVal;

                InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);
                chart1.DataSource = dicCharts[curPage];

                textBox1.Text = curPage.ToString();
                label_maxPage.Text = string.Format("/{0}", dicCharts.Count);
            }
        }

        private void pictureBox_right_Click(object sender, EventArgs e)
        {
            if (curPage < maxPage)
            {
                curPage++;
                //chart1.ChartAreas[0].AxisY.Maximum = Math.Round(dicCharts[curPage].Max(p => p.dPressure)) + 0.5;
                //double minVal = Math.Round(dicCharts[curPage].Min(p => p.dPressure)) - 0.5;
                //if (minVal < 0)
                //    chart1.ChartAreas[0].AxisY.Minimum = 0;
                //else
                //    chart1.ChartAreas[0].AxisY.Minimum = minVal;

                InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);
                chart1.DataSource = dicCharts[curPage];

                textBox1.Text = curPage.ToString();
                label_maxPage.Text = string.Format("/{0}", dicCharts.Count);
            }
        }

        private void pictureBox_doubleRight_Click(object sender, EventArgs e)
        {
            if (minPage == maxPage || maxPage == curPage) return;

            if (maxPage > curPage)
            {
                curPage = maxPage;
                //chart1.ChartAreas[0].AxisY.Maximum = Math.Round(dicCharts[curPage].Max(p => p.dPressure)) + 0.5;
                //double minVal = Math.Round(dicCharts[curPage].Min(p => p.dPressure)) - 0.5;
                //if (minVal < 0)
                //    chart1.ChartAreas[0].AxisY.Minimum = 0;
                //else
                //    chart1.ChartAreas[0].AxisY.Minimum = minVal;

                InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);
                chart1.DataSource = dicCharts[curPage];

                textBox1.Text = curPage.ToString();
                label_maxPage.Text = string.Format("/{0}", dicCharts.Count);
            }
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;

            int selectedPage = 0;
            if (!int.TryParse(textBox1.Text, out selectedPage)) return;

            if (selectedPage > maxPage || selectedPage < minPage || selectedPage == curPage) return;
            curPage = selectedPage;

            //chart1.ChartAreas[0].AxisY.Maximum = Math.Round(dicCharts[curPage].Max(p => p.dPressure)) + 0.5;
            //double minVal = Math.Round(dicCharts[curPage].Min(p => p.dPressure)) - 0.5;
            //if (minVal < 0)
            //    chart1.ChartAreas[0].AxisY.Minimum = 0;
            //else
            //    chart1.ChartAreas[0].AxisY.Minimum = minVal;

            InitSeries(dicCharts[curPage][dicCharts[curPage].Count - 1].dtTimeStamp, dicCharts[curPage][0].dtTimeStamp);

            chart1.DataSource = dicCharts[curPage];

            textBox1.Text = curPage.ToString();
            label_maxPage.Text = string.Format("/{0}", dicCharts.Count);
        } 
        #endregion 
    }

    /// <summary>
    /// 그래프 조회 조건 combobox item
    /// </summary>
    public class ComboItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
        public ComboItem(string text, int value)
        {
            this.Text = text;
            this.Value = value;
        }
    }
}
