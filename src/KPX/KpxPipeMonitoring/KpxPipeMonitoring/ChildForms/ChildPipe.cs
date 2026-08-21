
using DBUtility;
using KpxPipeMonitoring.Popups;
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

namespace KpxPipeMonitoring.ChildForms
{
    public partial class ChildPipe : Form
    {
        private DBUtility.WebDBManager m_dbMgr { get; set; }
         
        public bool isAlarm { get; set; }
        private int nAlarmHistroyID { get; set; }
        public delegate void AlarmClearEventArgs();
        public event AlarmClearEventArgs alarmClearEventArgs;

        public bool isWork = false;
        public int nPipeID { get; set; }
        public int nConnectWorkTankID = 0; 

        public double nStandardPressure = 0;
        public double nStandardFlow = 0;
        public long timeSpanSeconds { get; set; }

        public List<CommonFunction.AllAlarm> oldAlarmList = new List<CommonFunction.AllAlarm>();

        public ChildPipe(DBUtility.WebDBManager dbMgr, int pipeID)
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            this.BackColor = Color.FromArgb(25, 33, 58);

            this.m_dbMgr = dbMgr;
            this.nPipeID = pipeID;
             
            MainForm.Instance.SetDoubleBuffer(panel1, true); 

            pictureBox_clearAlarm.Parent = pictureBox_title;
            pictureBox_clearAlarm.Location = new Point(300, 8); 
             
            pictureBox_BeginWork.Parent = pictureBox_title;
            pictureBox_BeginWork.Location = new Point(470, 8); 
             
            pictureBox_EndWork.Parent = pictureBox_title;
            pictureBox_EndWork.Location = new Point(470, 8);

            label_workTime.Parent = pictureBox_title;
            label_workTime.Location = new Point(300, 16);

            pictureBox_rangeRefresh.Parent = pictureBox_title;
            pictureBox_rangeRefresh.Location = new Point(410, 8);

            label_workTime.Visible = false;
            pictureBox_clearAlarm.Visible = false;
            pictureBox_EndWork.Visible = false;
            pictureBox_BeginWork.Visible = true;
            label_tankName.Visible = false; 

            label_pipeName.Parent = pictureBox_title;
            label_pipeName.Location = new Point(15, 16);
             
            label_tankName.Parent = pictureBox_title;
            label_tankName.Location = new Point(202, 16);
             
            label_wait.Parent = pictureBox_title;
            label_wait.Location = new Point(label_pipeName.Location.X + label_pipeName.Size.Width + 10, 16);
            label_wait.ForeColor = Color.FromArgb(0xff, 0xf7, 0xbf, 0x91);
             
            MainForm.Instance.commonFunction.SettingButton(pictureBox_clearAlarm, global::KpxPipeMonitoring.Properties.Resources.AlarmClear, global::KpxPipeMonitoring.Properties.Resources.AlarmClear, "알람해제");
            MainForm.Instance.commonFunction.SettingButton(pictureBox_BeginWork, global::KpxPipeMonitoring.Properties.Resources.BeginWork_Click, global::KpxPipeMonitoring.Properties.Resources.BeginWork_Click);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_EndWork, global::KpxPipeMonitoring.Properties.Resources.EndWork_Click, global::KpxPipeMonitoring.Properties.Resources.EndWork_Click);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_rangeRefresh, global::KpxPipeMonitoring.Properties.Resources.RangeRefresh_Normal, global::KpxPipeMonitoring.Properties.Resources.RangeRefresh_Click);

            InitChart(); 
        }

        private List<CommonFunction.ChartField> allChartList { get; set; }        
        Dictionary<DateTime, List<double>> dicTempDatas = new Dictionary<DateTime, List<double>>();
        public void InitChartData(List<CommonFunction.ChartField> chartList)
        {
            try
            {
                if (chartList == null) return;

                List<CommonFunction.ChartField> displayChartData = new List<CommonFunction.ChartField>();
                displayChartData = chartList.Where(p => p.nPipeID == this.nPipeID).ToList();

                chart_pressure.ChartAreas[0].AxisY.StripLines.Clear();
                chart_flow.ChartAreas[0].AxisY.StripLines.Clear();

                label_pressureRange.Text = "압력 (kg/cm²) ";
                label_flowRange.Text = "유량 (kl/h) ";

                if (this.nPipeID < 1 || displayChartData == null || displayChartData.Count == 0)
                {
                    List<CommonFunction.ChartField> temp = new List<KpxPipeMonitoring.CommonFunction.ChartField>();
                    temp.Add(new CommonFunction.ChartField(0, 0, new DateTime(), 0, 0));
                    chart_pressure.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chart_pressure.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                    chart_pressure.ChartAreas[0].AxisY.IsStartedFromZero = true;

                    chart_pressure.DataSource = temp;

                    chart_flow.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chart_flow.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                    chart_flow.ChartAreas[0].AxisY.IsStartedFromZero = true;

                    chart_flow.DataSource = temp;
                    return;
                }

                //조회 시점 
                //DateTime searchDateTime = MainForm.Instance.SystemNow;
                //searchDateTime = searchDateTime.AddMinutes(-30);
                //int displayCondition = MainForm.Instance.commonFunction.GetChartPointCount(searchDateTime, MainForm.Instance.SystemNow);

                // 시간이 지난 데이터 제거
                //List<CommonFunction.ChartField> removeChartList = new List<CommonFunction.ChartField>();
                //foreach (CommonFunction.ChartField item in displayChartData)
                //{
                //    if (item.dtTimeStamp < searchDateTime)
                //        removeChartList.Add(item);
                //    else 
                //        break; // 시간 순서로 저장되기 때문에 더이상 반복할 이유가 없음
                //}
                //for (int i = removeChartList.Count - 1; i >= 0; i--)
                //{
                //    displayChartData.Remove(removeChartList[i]);
                //}

                //foreach (CommonFunction.ChartField item in allChartList)
                //{
                //    if (displayChartData.Contains(item)) continue;

                //    if (displayChartData.Count == 0 || displayChartData[displayChartData.Count - 1].dtTimeStamp < item.dtTimeStamp)
                //    {  
                //        displayChartData.Add(new CommonFunction.ChartField(0, 0, item.dtTimeStamp, item.dPressure, item.dFlow));
                //    } 
                //} 

                if (displayChartData.Count == 0)
                {
                    List<CommonFunction.ChartField> temp = new List<KpxPipeMonitoring.CommonFunction.ChartField>();
                    temp.Add(new CommonFunction.ChartField(0, 0, new DateTime(), 0, 0));
                    chart_pressure.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chart_pressure.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

                    chart_pressure.ChartAreas[0].AxisY.IsStartedFromZero = true;

                    chart_pressure.DataSource = temp;

                    chart_flow.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                    chart_flow.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

                    chart_flow.ChartAreas[0].AxisY.IsStartedFromZero = true;

                    chart_flow.DataSource = temp;
                    return;
                }
                else
                {
                    chart_pressure.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                    chart_pressure.ChartAreas[0].AxisY.MajorGrid.Enabled = true;

                    chart_flow.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                    chart_flow.ChartAreas[0].AxisY.MajorGrid.Enabled = true;

                    chart_pressure.DataSource = null;
                    chart_flow.DataSource = null;
                    chart_pressure.DataSource = displayChartData;
                    chart_flow.DataSource = displayChartData;

                    // 안정범위, 차트 범위 설정                
                    if (!this.isWork) // 작업중이 아닐때
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
                            
                            double max2 = Math.Round(displayChartData.Max(p => p.dFlow), 1);
                            if (max2 == 0)
                                chart_flow.ChartAreas[0].AxisY.Maximum = 1;
                            else
                                chart_flow.ChartAreas[0].AxisY.Maximum = max2 + 0.5;

                            double minFlow = Math.Round(displayChartData.Min(p => p.dFlow), 1);
                            double minVal2 = minFlow - 0.5;
                            if (minFlow == 0)
                                chart_flow.ChartAreas[0].AxisY.Minimum = 0;
                            else
                                chart_flow.ChartAreas[0].AxisY.Minimum = minVal2;                            
                        }
                    }
                    else
                    {
                        int rng_nPipeStableType = 0;
                        double rng_nPipeStableValue = 0;
                        string rng_strPipeRange = "-";
                        int rng_nPipeStableCTimeUse = -1;
                        int rng_nPipeStableCTime = 0;

                        int rng_nTankStableType = 0;
                        double rng_nTankStableValue = 0;
                        string rng_strTankRange = "-";
                        int rng_nTankStableCTimeUse = -1;
                        int rng_nTankStableCTime = 0;

                        string rng_strPipeStableUpdateTime = "";
                        string rng_strTankStableUpdateTime = "";
                        ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData("SELECT StandardPressureUpdateTime, StandardFlowUpdateTime FROM LastWorkHistory WHERE PipeID=" + this.nPipeID + " AND TankID=" + this.nConnectWorkTankID, 0);
                        if (arrResult != null && arrResult.Count == 2)
                        {
                            rng_strPipeStableUpdateTime = (arrResult[0].ToString() == "null") ? "" : Convert.ToDateTime(arrResult[0]).ToString("HH시mm분");
                            rng_strTankStableUpdateTime = (arrResult[1].ToString() == "null") ? "" : Convert.ToDateTime(arrResult[1]).ToString("HH시mm분");
                        }

                        foreach (CommonFunction.AlarmPipeOptionInfo item in MainForm.Instance.alarmPipeOptionInfo)
                        {
                            if (item.nPipeID == this.nPipeID)
                            {
                                if (this.nStandardPressure != -9999 && this.nStandardPressure != -999)
                                {
                                    double minStripLine = 0;
                                    double maxStripLine = 0;

                                    rng_nPipeStableType = item.nPipeStableType;
                                    if (item.nPipeStableType == 0) // 비율 사용
                                    {
                                        minStripLine = nStandardPressure - ((nStandardPressure * item.nPipeStableRatio) / 100);
                                        maxStripLine = nStandardPressure + ((nStandardPressure * item.nPipeStableRatio) / 100);
                                    }
                                    else if (item.nPipeStableType == 1) // 절대값 사용
                                    {
                                        minStripLine = nStandardPressure - item.nPipeStableAbsolute;
                                        maxStripLine = nStandardPressure + item.nPipeStableAbsolute;
                                    }

                                    if (minStripLine < 0)
                                        minStripLine = 0;

                                    if (isWork)
                                    {
                                        chart_pressure.ChartAreas[0].AxisY.StripLines.Add(MainForm.Instance.commonFunction.GetStripLine(maxStripLine, StringAlignment.Far, "Max"));
                                        chart_pressure.ChartAreas[0].AxisY.StripLines.Add(MainForm.Instance.commonFunction.GetStripLine(minStripLine, StringAlignment.Near, "Min"));

                                        rng_strPipeRange = String.Format("{0:F1}", minStripLine) + " ~ " + String.Format("{0:F1}", maxStripLine);
                                    }

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

                                break;
                            }
                        }

                        foreach (CommonFunction.AlarmTankOptionInfo item in MainForm.Instance.alarmTankOptionInfo)
                        {
                            if (item.nTankID == this.nConnectWorkTankID)
                            {
                                if (this.nStandardFlow != -9999 && this.nStandardFlow != -999)
                                {
                                    double minStripLine = 0;
                                    double maxStripLine = 0;

                                    if (item.nTankStableType == 0) // 비율 사용
                                    {
                                        minStripLine = nStandardFlow - Math.Abs((nStandardFlow * item.nTankStableRatio) / 100);
                                        maxStripLine = nStandardFlow + Math.Abs((nStandardFlow * item.nTankStableRatio) / 100);
                                    }
                                    else if (item.nTankStableType == 1) // 절대값 사용
                                    {
                                        minStripLine = nStandardFlow - Math.Abs(item.nTankStableAbsolute);
                                        maxStripLine = nStandardFlow + Math.Abs(item.nTankStableAbsolute);
                                    }

                                    if (isWork)
                                    {
                                        chart_flow.ChartAreas[0].AxisY.StripLines.Add(MainForm.Instance.commonFunction.GetStripLineFlow(Math.Round(maxStripLine, 1), StringAlignment.Far, "Max"));
                                        chart_flow.ChartAreas[0].AxisY.StripLines.Add(MainForm.Instance.commonFunction.GetStripLineFlow(Math.Round(minStripLine, 1), StringAlignment.Near, "Min"));
                                        if (alarmFlow)
                                        {
                                            for (int i = 0; i < chart_flow.ChartAreas[0].AxisY.StripLines.Count; i++)
                                            {
                                                chart_flow.ChartAreas[0].AxisY.StripLines[i].BorderColor = Color.White;
                                                chart_flow.ChartAreas[0].AxisY.StripLines[i].ForeColor = Color.White;
                                            }
                                        }
                                        rng_strTankRange = String.Format("{0:F1}", minStripLine) + " ~ " + String.Format("{0:F1}", maxStripLine);
                                    }

                                    double minNum = nStandardFlow - ((nStandardFlow - minStripLine) * 2);
                                    double maxNum = nStandardFlow + ((maxStripLine - nStandardFlow) * 2);

                                    double convertMinNum = Math.Round(minNum, 1);
                                    double convertMaxNum = Math.Round(maxNum, 1);

                                    if (convertMinNum == convertMaxNum)
                                    {
                                        //if (minStripLine - 1 <= 0)
                                        //    chart_flow.ChartAreas[0].AxisY.Minimum = 0;
                                        //else
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
                                        //if (minNum > 0)
                                        chart_flow.ChartAreas[0].AxisY.Minimum = convertMinNum;
                                        //else
                                        //    chart_flow.ChartAreas[0].AxisY.Minimum = 0;

                                        //if (maxNum > 0)
                                        chart_flow.ChartAreas[0].AxisY.Maximum = convertMaxNum;
                                        //else
                                        //    chart_flow.ChartAreas[0].AxisY.Maximum = 1;
                                    }
                                }
                                else
                                {
                                    //현재 차트에 표현되는 데이터 기준으로 차트 범위 설정
                                    if (displayChartData != null && displayChartData.Count > 0)
                                    {
                                        double max2 = Math.Round(displayChartData.Max(p => p.dFlow), 1);
                                        if (max2 == 0)
                                            chart_flow.ChartAreas[0].AxisY.Maximum = 1;
                                        else
                                            chart_flow.ChartAreas[0].AxisY.Maximum = max2 + 0.5;

                                        double minVal2 = Math.Round(displayChartData.Min(p => p.dFlow), 1) - 0.5;
                                        //if (minVal2 < 0)
                                        //    chart_flow.ChartAreas[0].AxisY.Minimum = 0;
                                        //else
                                        chart_flow.ChartAreas[0].AxisY.Minimum = minVal2;
                                    }
                                }

                                rng_nTankStableType = item.nTankStableType;
                                rng_nTankStableValue = (rng_nTankStableType == 0) ? item.nTankStableRatio : item.nTankStableAbsolute;
                                rng_nTankStableCTimeUse = item.nTankStableCTimeUse;
                                rng_nTankStableCTime = item.nTankStableCTime;

                                break;
                            }
                        }

                        if (rng_strPipeRange.Length == 0) rng_strPipeRange = "-";
                        if (rng_strTankRange.Length == 0) rng_strTankRange = "-";

                        label_pressureRange.Text = string.Format("압력 (kg/cm²) | 범위:{0}({1}{2}) | 설정:{3} | 유지:{4}", rng_strPipeRange, rng_nPipeStableValue, (rng_nPipeStableType == 0) ? "%" : "kg/cm²", rng_strPipeStableUpdateTime, (rng_nPipeStableCTimeUse == 0) ? "-" : rng_nPipeStableCTime + "분");
                        label_flowRange.Text = string.Format("유량 (kl/h) | 범위:{0}({1}{2}) | 설정:{3} | 유지:{4}", rng_strTankRange, rng_nTankStableValue, (rng_nTankStableType == 0) ? "%" : "kl/h", rng_strTankStableUpdateTime, (rng_nTankStableCTimeUse == 0) ? "-" : rng_nTankStableCTime + "분");                        
                    }

                    chart_pressure.ChartAreas[0].AxisY.Interval =
                                Math.Abs((chart_pressure.ChartAreas[0].AxisY.Minimum + chart_pressure.ChartAreas[0].AxisY.Maximum) / 4);

                    //if (chart_pressure.ChartAreas[0].AxisY.Interval < 1)
                    //    chart_pressure.ChartAreas[0].AxisY.Interval = 1;

                    chart_flow.ChartAreas[0].AxisY.Interval =
                    Math.Abs((chart_flow.ChartAreas[0].AxisY.Minimum + chart_flow.ChartAreas[0].AxisY.Maximum) / 4);
                    
                    //if (chart_flow.ChartAreas[0].AxisY.Interval < 1)
                    //    chart_flow.ChartAreas[0].AxisY.Interval = 1;
                }
            }
            catch (Exception ex)
            {
                MainForm.Instance.SetSystemLog("[ERROR] InitChartData() / " + ex.Message);
            } 
        }

        #region 차트 세팅
        private void InitChart()
        {
            chart_pressure.Series.Clear();
            Series series = chart_pressure.Series.Add("series1");
            series.ChartType = SeriesChartType.Line;
            chart_pressure.Series[0].IsXValueIndexed = true;
            chart_pressure.Series[0].XValueMember = "dtTimeStamp";
            chart_pressure.Series[0].YValueMembers = "dPressure";
            chart_pressure.Series[0].ToolTip = "#VALX{HH:mm} - #VALY1{0.00}";
            chart_pressure.Series[0].BorderWidth = 3;
            chart_pressure.Series[0].Color = Color.FromArgb(48, 129, 209);

            chart_pressure.ChartAreas[0].AxisY.Interval = 0;
            chart_pressure.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Near;

            chart_pressure.ChartAreas[0].AxisY.LabelStyle.Format = "F1";
            //chart_pressure.ChartAreas[0].AxisY.label
            chart_pressure.ChartAreas[0].AxisY.IsLabelAutoFit = false;
            chart_pressure.ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont;
            
            chart_pressure.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            chart_pressure.ChartAreas[0].AxisX.IsMarginVisible = false;
             
            chart_flow.Series.Clear();
            series = chart_flow.Series.Add("series2");
            series.ChartType = SeriesChartType.Line;
            chart_flow.Series[0].IsXValueIndexed = true;
            chart_flow.Series[0].XValueMember = "dtTimeStamp";
            chart_flow.Series[0].YValueMembers = "dFlow";
            chart_flow.Series[0].ToolTip = "#VALX{HH:mm} - #VALY1{0.00}";
            chart_flow.Series[0].BorderWidth = 3;
            chart_flow.Series[0].Color = Color.FromArgb(255, 137, 0);

            chart_flow.ChartAreas[0].AxisY.Interval = 0;
            chart_flow.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Near;

            chart_flow.ChartAreas[0].AxisY.LabelStyle.Format = "F1";
            chart_flow.ChartAreas[0].AxisY.IsLabelAutoFit = false;
            chart_flow.ChartAreas[0].AxisY.LabelAutoFitStyle = LabelAutoFitStyles.IncreaseFont;            
            chart_flow.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            chart_flow.ChartAreas[0].AxisX.IsMarginVisible = false;

            chart_pressure.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart_pressure.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart_pressure.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(232, 229, 229);
            chart_pressure.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(232, 229, 229); 
             
            //차트 위치
            chart_pressure.ChartAreas[0].Position.Auto = false; 
            chart_pressure.ChartAreas[0].Position.X = 0;
            chart_pressure.ChartAreas[0].Position.Y = 20;
            chart_pressure.ChartAreas[0].Position.Width = 97;
            chart_pressure.ChartAreas[0].Position.Height = 90;

            chart_pressure.Legends.Clear();
             
            chart_pressure.Customize += (s, e) =>
                {
                    Series curSeries = chart_pressure.Series[0] as Series;
                    if (curSeries == null) return;

                    int pointCnt = curSeries.Points.Count;
                    if (pointCnt <= 1) return;

                    int count = curSeries.Points[pointCnt - 1].YValues.Count();

                    if (count > 0)
                    {
                        //string strLastData = string.Format("{0:F1}", curSeries.Points[pointCnt - 1].YValues[count - 1]);
                        //double strLastData = curSeries.Points[pointCnt - 1].YValues[count - 1];
                        //curSeries.Points[pointCnt - 1].YValues[count - 1] = strLastData;
                    }

                    if (!bViewWork)
                        curSeries.Points[pointCnt - 1].IsValueShownAsLabel = true;
                    curSeries.Points[pointCnt - 1].LabelFormat = "F1";
                    curSeries.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes; 
                };

            chart_flow.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart_flow.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            chart_flow.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.FromArgb(232, 229, 229);
            chart_flow.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(232, 229, 229);

            //차트 위치
            chart_flow.ChartAreas[0].Position.Auto = false;
            chart_flow.ChartAreas[0].Position.X = 0;
            chart_flow.ChartAreas[0].Position.Y = 20;
            chart_flow.ChartAreas[0].Position.Width = 97;
            chart_flow.ChartAreas[0].Position.Height = 90;

            chart_flow.Legends.Clear();

            chart_flow.Customize += (s, e) =>
            {
                Series curSeries = chart_flow.Series[0] as Series;
                if (curSeries == null) return;

                int pointCnt = curSeries.Points.Count;
                if (pointCnt <= 1) return;

                int count = curSeries.Points[pointCnt - 1].YValues.Count();

                if (count > 0)
                {
                    string strLastData = string.Format("{0:F1}", curSeries.Points[pointCnt - 1].YValues[count - 1]);
                    curSeries.Points[pointCnt - 1].YValues[count - 1] = double.Parse(strLastData);
                }

                if (!bViewWork)
                    curSeries.Points[pointCnt - 1].IsValueShownAsLabel = true;
                curSeries.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;

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
            };
        } 
        #endregion

        #region 알람 관련
        Image chartTitleWorkTrueImg = global::KpxPipeMonitoring.Properties.Resources.PT_Green;
        Image chartTitleAlarmTrueImg = global::KpxPipeMonitoring.Properties.Resources.PT_Red;
        Image chartTitleWorkFalseImg = global::KpxPipeMonitoring.Properties.Resources.PT_Orange;
        public bool alarmPressure = false;
        public bool alarmFlow = false;
        Color alarmTrueColor = Color.FromArgb(255, 200, 0, 0);
        Color alarmFalseColor = Color.FromArgb(200, 255, 255, 255);
        public void Setting(bool isAlarm, bool isChgAlarm, List<CommonFunction.AllAlarm> newAlarmList, bool isWork)
        {
            foreach (CommonFunction.PipeInfo item in MainForm.Instance.pipeInfo)
            {
                if (this.nPipeID == item.nPipeID)
                {
                    if (isWork)
                    {
                        foreach (CommonFunction.TankInfo tank in MainForm.Instance.tankInfo)
                        {
                            if (tank.nTankID == nConnectWorkTankID)
                            {
                                label_pipeName.Text = item.strPipeName + item.strPipeType + "(" + tank.strLiquidType + ")";
                                this.label_pipeName.Font = new System.Drawing.Font("나눔바른고딕", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                                label_tankName.Text = "TK-" + tank.strTankName;
                                label_workTime.Visible = true;
                                pictureBox_rangeRefresh.Visible = true;
                                lblFlow.Text = "유량 : " + String.Format("{0:F1}", tank.nFlow);
                                break;
                            }
                        }
                    }
                    else
                    {
                        label_pipeName.Text = item.strPipeName + item.strPipeType;
                        this.label_pipeName.Font = new System.Drawing.Font("나눔바른고딕", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
                        label_workTime.Visible = false;
                        pictureBox_rangeRefresh.Visible = false;
                    }

                    lblPressure.Text = "압력 : " + String.Format("{0:F1}", item.nPressure);
                    break; 
                }
            }

            if (isWork)
            {
                lblMemo.Visible = true;
                if (this.nPipeID == 6)
                    lblMemo.Text = "현대EP 이송중";
                else if (this.nPipeID == 10)
                    lblMemo.Text = "한화종합화학 이송중";
                else
                    lblMemo.Text = "선박 작업중";
            }
            else
                lblMemo.Visible = false;

            // 작업이 새로 시작되거나 종료된 경우, 알람이 생기거나 해제된 경우, 알람 내용이 변경된 경우
            if (isWork != this.isWork || isAlarm != this.isAlarm || isChgAlarm)
            {
                List<int> nsumAlarmType = new List<int>();
                foreach (CommonFunction.AllAlarm item in newAlarmList)
                {
                    if (item.nPipeID != this.nPipeID) continue;
                    if (item.nTankID != this.nConnectWorkTankID) continue;
                    if (item.nAlarmHistoryID <= 0) continue;

                    nsumAlarmType.Add(item.nAlarmType);
                }

                if (isWork)
                { 
                    if (isAlarm)
                    {
                        if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소)))
                        { 
                            // 유량
                            //pictureBox_alarmPressure.Visible = false;
                            //pictureBox_alarmFlow.Visible = true;

                            alarmPressure = false;
                            alarmFlow = true;

                            chart_pressure.ChartAreas[0].BackColor = alarmFalseColor;
                            chart_flow.ChartAreas[0].BackColor = alarmTrueColor;

                            chart_flow.Series[0].Color = Color.White; 
                             
                            // Color.FromArgb(255, 137, 0) //주황

                        }
                        else if (nsumAlarmType.Count == 1 && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                        {
                            // 압력
                            //pictureBox_alarmPressure.Visible = true;
                            //pictureBox_alarmFlow.Visible = false;

                            alarmPressure = true;
                            alarmFlow = false;

                            chart_pressure.ChartAreas[0].BackColor = alarmTrueColor;
                            chart_flow.ChartAreas[0].BackColor = alarmFalseColor;

                            chart_flow.Series[0].Color = Color.FromArgb(255, 137, 0);
                        }
                        else if (nsumAlarmType.Count == 2 && (nsumAlarmType.Contains((int)AlarmType.유량증가) || nsumAlarmType.Contains((int)AlarmType.유량감소))
                                                      && (nsumAlarmType.Contains((int)AlarmType.압력상승) || nsumAlarmType.Contains((int)AlarmType.압력하강)))
                        {
                            // 유량, 압력 
                            //pictureBox_alarmPressure.Visible = true;
                            //pictureBox_alarmFlow.Visible = true;

                            alarmPressure = true;
                            alarmFlow = true;

                            chart_pressure.ChartAreas[0].BackColor = alarmTrueColor;
                            chart_flow.ChartAreas[0].BackColor = alarmTrueColor;

                            chart_flow.Series[0].Color = Color.FromArgb(255, 137, 0);
                        }

                        pictureBox_title.Image = chartTitleAlarmTrueImg;
                        pictureBox_clearAlarm.Visible = true;
                    }
                    else
                    {
                        //pictureBox_alarmPressure.Visible = false;
                        //pictureBox_alarmFlow.Visible = false;

                        pictureBox_title.Image = chartTitleWorkTrueImg;
                        pictureBox_clearAlarm.Visible = false;

                        alarmPressure = false;
                        alarmFlow = false; 

                        chart_pressure.ChartAreas[0].BackColor = alarmFalseColor;
                        chart_flow.ChartAreas[0].BackColor = alarmFalseColor;
                        chart_flow.Series[0].Color = Color.FromArgb(255, 137, 0);
                    } 
                     
                    label_pipeName.Size = new System.Drawing.Size(174, 22);
                    if (bViewWork)
                        label_pipeName.Location = new Point(50, 16); 
                    else
                        label_pipeName.Location = new Point(20, 16);
                    label_tankName.Visible = true;

                    label_wait.Text = "";//"작업중";                     
                    label_wait.Location = new Point(label_tankName.Location.X + label_tankName.Size.Width + 20, 16);

                    pictureBox_BeginWork.Visible = false;
                    pictureBox_EndWork.Visible = true;
                      
                    //label_pressureRange.Visible = true;
                    //label_flowRange.Visible = true;
                }
                else
                {
                    nConnectWorkTankID = -1;
                    label_pipeName.Location = new Point(15, 16);
                    label_pipeName.Size = new Size(134, 22); 
                     
                    label_tankName.Text = "";
                    label_tankName.Visible = false;
                     
                    label_wait.Text = "대기중";                    
                    label_wait.Location = new Point(label_pipeName.Location.X + label_pipeName.Size.Width + 10, 16);

                    pictureBox_title.Image = chartTitleWorkFalseImg;

                    pictureBox_BeginWork.Visible = true;
                    pictureBox_EndWork.Visible = false;
                    pictureBox_clearAlarm.Visible = false; 
                      
                    //label_pressureRange.Visible = false;
                    //label_flowRange.Visible = false;

                    chart_pressure.ChartAreas[0].BackColor = alarmFalseColor;
                    chart_flow.ChartAreas[0].BackColor = alarmFalseColor;
                } 

                oldAlarmList = newAlarmList;
                
                // 작업이 시작된 경우 작업시작시간 가져옴
                if (!this.isWork && isWork)
                {
                    ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData("SELECT BeginTime FROM LastWorkHistory WHERE PipeID=" + this.nPipeID + " AND TankID=" + this.nConnectWorkTankID, 0);
                    if (arrResult != null && arrResult.Count == 1)
                    {
                        m_recentBeginTime = DBUtility.WebDBManager.GetDateTimeField(arrResult[0]); 
                    }
                }

                this.isWork = isWork;
                this.isAlarm = isAlarm;
            } 
        }

        public void AlarmClear(int occurType, string comment)
        {
            int nCmdID = MainForm.Instance.commonFunction.GetMaxTableID("Command") + 1;
            int nCmdHistoryID = MainForm.Instance.commonFunction.GetMaxTableID("CommandHistory") + 1;

            foreach (CommonFunction.AllAlarm item in oldAlarmList)
            {
                item.nAlarmOccurType = occurType;
                item.strAlarmComment = comment;

                if (item.nPipeID <= 0 || item.nTankID <= 0 || this.nConnectWorkTankID <= 0) continue;
                 
                StringBuilder sb = new StringBuilder();                 
                sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
                sb.Append("VALUES(" + nCmdID + ", 0, now(), " + item.nPipeID + ", " + item.nTankID + ", " + MainForm.Instance.nUserID + ") ");
                MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                sb = new StringBuilder();
                sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID, AlarmOccurType, AlarmComment, AlarmHistoryID) ");
                sb.AppendFormat("VALUES ({0}, 0, now(), NULL, {1}, {2}, {3}, {4}, {5}, '{6}', {7})", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, item.nPipeID, item.nTankID, item.nAlarmOccurType, item.strAlarmComment, item.nAlarmHistoryID);
                MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                nCmdID++;
                nCmdHistoryID++;

                if (item.nAlarmType == (int)AlarmType.유량감소 || item.nAlarmType == (int)AlarmType.유량증가)
                {
                    foreach (CommonFunction.AllAlarm alarm in MainForm.Instance.newAlarmInfo)
                    {
                        if (alarm.nTankID == item.nTankID && alarm.nPipeID == item.nPipeID) 
                            continue;
                        if (alarm.nTankID == item.nTankID)
                        {
                            sb = new StringBuilder();
                            sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
                            sb.Append("VALUES(" + nCmdID + ", 0, now(), " + alarm.nPipeID + ", " + alarm.nTankID + ", " + MainForm.Instance.nUserID + ") ");
                            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                            sb = new StringBuilder();
                            sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID, AlarmOccurType, AlarmComment, AlarmHistoryID) ");
                            sb.AppendFormat("VALUES ({0}, 0, now(), NULL, {1}, {2}, {3}, {4}, {5}, '{6}', {7})", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, alarm.nPipeID, alarm.nTankID, alarm.nAlarmOccurType, alarm.strAlarmComment, alarm.nAlarmHistoryID);
                            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

                            nCmdID++;
                            nCmdHistoryID++;
                        }
                    }
                } 
            } 

            if (alarmClearEventArgs != null)
                alarmClearEventArgs();
        }
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

            UnE.Utility.UMessageBox.FrameColor = Color.FromArgb(1, 4, 20);
            if (buttonStatus)
                msg = "알람을 해제하시겠습니까?\r함체박스의 Push 버튼이 눌려져 있으므로 알람을 해제해도 경광등은 꺼지지 않습니다.\r경광등을 끄기 위해서는 함체박스의 Push버튼을 다시 눌러주시기 바랍니다.";

            //if (UnE.Utility.UMessageBox.Show(MainForm_Pipe.Instance, msg, "알람 해제", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;   
            AlarmClear ac = new Popups.AlarmClear(msg);
            ac.StartPosition = FormStartPosition.CenterParent;
            DialogResult dr = ac.ShowDialog();
            if(dr == DialogResult.OK)
                AlarmClear(ac.occurenceType, ac.comment);
        }
        #endregion

        #region 작업 관련

        private void pictureBox_BeginWork_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            BeginWorkSelectTank pop = new BeginWorkSelectTank(this.nPipeID);
            pop.StartPosition = FormStartPosition.CenterParent;
            if (pop.ShowDialog() != System.Windows.Forms.DialogResult.Yes) return;

            if (pop.nTankID < 0) return;
              
            //작업시작 
            int nCmdID = MainForm.Instance.commonFunction.GetMaxTableID("Command") + 1;

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
            sb.Append("VALUES(" + nCmdID + ", 4, now(), " + this.nPipeID + ", " + pop.nTankID + ", " + MainForm.Instance.nUserID + ") ");
            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

            int nCmdHistoryID = MainForm.Instance.commonFunction.GetMaxTableID("CommandHistory") + 1;

            sb = new StringBuilder();
            sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID) ");
            sb.Append("VALUES (" + nCmdHistoryID + ", 4, now(), NULL," + MainForm.Instance.nUserID + ", " + nCmdID + ", " + this.nPipeID + "," + pop.nTankID + ") ");
            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

            //this.isWork = true;
            //pictureBox_BeginWork.Visible = false;
            //pictureBox_EndWork.Visible = true;
            this.nConnectWorkTankID = pop.nTankID;
        }

        private void pictureBox_EndWork_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            UnE.Utility.UMessageBox.FrameColor = Color.FromArgb(1, 4, 20);
            if (UnE.Utility.UMessageBox.Show(MainForm_Pipe.Instance, "작업을 종료하시겠습니까?", "작업 종료", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;
             
            WorkStop();
        } 

        private void WorkStop()
        {
            //작업종료
            int nCmdID = MainForm.Instance.commonFunction.GetMaxTableID("Command") + 1; 

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
            sb.Append("VALUES(" + nCmdID + ", 5, now(), " + nPipeID + ", " + this.nConnectWorkTankID + ", " + MainForm.Instance.nUserID + ") ");
            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

            sb = new StringBuilder();
            sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID) ");
            sb.Append("VALUES ((SELECT ID FROM (SELECT IFNULL(MAX(ID) + 1, 1) ID FROM CommandHistory) X), 5, now(), NULL," + MainForm.Instance.nUserID + ", " + nCmdID + ", " + nPipeID + ", " + this.nConnectWorkTankID + ") ");
            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0); 
        }

        private DBUtility.VariousData<DateTime> m_recentBeginTime = null;
        public void SetWorkTime()
        {
            if (m_recentBeginTime == null)
                label_workTime.Text = "-";
            else
            {
                TimeSpan span = MainForm.Instance.SystemNow - m_recentBeginTime.Data;

                int nTotalSeconds = (int)span.TotalSeconds;
                int nHour = nTotalSeconds / 3600;
                int nMin = (nTotalSeconds - nHour * 3600) / 60;
                int nSec = nTotalSeconds - nHour * 3600 - nMin * 60;

                label_workTime.Text = string.Format("{0:00}:{1:00}:{2:00}", nHour, nMin, nSec);
            }
        }
        #endregion  

        private void pictureBox_rangeRefresh_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.nConnectWorkTankID < 0) return;

            if (UnE.Utility.UMessageBox.Show(MainForm_Pipe.Instance, "현재값을 기준으로 압력과 유량의 정상범위를 새로 설정하시겠습니까?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No) return;

            int nCmdID = MainForm.Instance.commonFunction.GetMaxTableID("Command") + 1;
            int nCmdHistoryID = MainForm.Instance.commonFunction.GetMaxTableID("CommandHistory") + 1;

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ");
            sb.Append("VALUES(" + nCmdID + ", 8, now(), " + this.nPipeID + ", " + this.nConnectWorkTankID + ", " + MainForm.Instance.nUserID + ") ");
            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);

            sb = new StringBuilder();
            sb.Append("INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID) ");
            sb.AppendFormat("VALUES ({0}, 8, now(), NULL, {1}, {2}, {3}, {4})", nCmdHistoryID, MainForm.Instance.nUserID, nCmdID, this.nPipeID, this.nConnectWorkTankID);
            MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0); 
        }

        public bool bViewWork = false;
        public void SetViewWorkModeLocation()
        {
            panel1.Size = new Size(945, 436);
            pictureBox_title.Size = new Size(895, 52);
            pictureBox_rangeRefresh.Location = new Point(710, 8);
            label_pipeName.Location = new Point(170, 22);
            label_tankName.Location = new Point(335, 16);
            label_workTime.Location = new Point(470, 18);

            pictureBox_BeginWork.Location = new Point(775, 8);
            pictureBox_EndWork.Location = new Point(775, 8);

            chart_pressure.Size = new Size(930, 180);
            chart_pressure.Location = new Point(7, 60);
            chart_flow.Size = new Size(930, 180);
            chart_flow.Location = new Point(7, 252);

            label_pressureRange.Location = new Point(285, 59);
            label_flowRange.Location = new Point(285, 242);
            pictureBox_clearAlarm.Location = new Point(605, 8);
            lblPressure.Location = new Point(750, 55);
            lblFlow.Location = new Point(750, 238);

            lblMemo.Location = new Point(10, 55);
            lblMemo.Font = new System.Drawing.Font("나눔바른고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }
    }
}
