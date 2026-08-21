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

namespace libExternalUI.Lib
{
    public partial class FormAirQuality : Form
    {
        private List<AirQuaility> arr = null;
        public FormAirQuality()
        {
            InitializeComponent();

            this.TopLevel = false;

            btnClose.ImageNormal = global::libExternalUI.Properties.Resources.close_Normal;
            btnClose.ImageClicked = global::libExternalUI.Properties.Resources.close_Click;
            btnClose.ImageMouseOver = global::libExternalUI.Properties.Resources.close_MouseOver;

            btnConfig.ImageNormal = global::libExternalUI.Properties.Resources.config_Normal;
            btnConfig.ImageClicked = global::libExternalUI.Properties.Resources.config_Click;
            btnConfig.ImageMouseOver = global::libExternalUI.Properties.Resources.config_MouseOver;

            picTemp.Image = global::libExternalUI.Properties.Resources.temp;
            picHumi.Image = global::libExternalUI.Properties.Resources.humi;

            InitChart();

            //double minStripLine = 70;
            //double maxStripLine = 90;

            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = true;
            
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 100;

            //chart1.ChartAreas[0].AxisY.StripLines.Add(GetStripLine(minStripLine, StringAlignment.Far, "주의"));
            //chart1.ChartAreas[0].AxisY.StripLines.Add(GetStripLine(maxStripLine, StringAlignment.Far, "위험"));
            
            chart1.Customize += Chart1_Customize;
        }

        private void Chart1_Customize(object sender, EventArgs e)
        {
            Series curSeries = chart1.Series[0] as Series;
            for (int i = 0; i < curSeries.Points.Count; i++)
            {
                foreach (AirQuaility item in arr)
                {
                    if (item.Name.Contains(curSeries.Points[i].AxisLabel))
                    {
                        if (item.Name.Substring(0, 1) == "O") //산소
                        {
                            if (curSeries.Points[i].YValues[0] <= item.Limit2)
                            {
                                curSeries.Points[i].Color = Color.FromArgb(0xf7, 0x53, 0x53); //빨강
                            }
                            else if (curSeries.Points[i].YValues[0] <= item.Limit1 && curSeries.Points[i].YValues[0] >= item.Limit2)
                            {
                                curSeries.Points[i].Color = Color.FromArgb(0xff, 0xd3, 0x48); //노랑
                            }
                            else 
                            {
                                curSeries.Points[i].Color = Color.FromArgb(0x8d, 0xcd, 0x8c); //초록                                
                            } 
                        }
                        else
                        {
                            if (curSeries.Points[i].YValues[0] >= item.Limit2)
                            {
                                curSeries.Points[i].Color = Color.FromArgb(0xf7, 0x53, 0x53); //빨강
                            }
                            else if (curSeries.Points[i].YValues[0] >= item.Limit1)
                            {
                                curSeries.Points[i].Color = Color.FromArgb(0xff, 0xd3, 0x48); //노랑
                            }
                            else
                            {
                                curSeries.Points[i].Color = Color.FromArgb(0x8d, 0xcd, 0x8c); //초록
                            } 
                        }
                        break;
                    }
                }
            }
        }

        public void DisplayAirquaility()
        {
            ArrayList arrConn = UIManager.Instance.DBMgr.GetResultData("SELECT Connected FROM airquaility");
            if (arrConn == null || arrConn.Count == 0)
            {
                lblConn.Visible = false;
                chart1.Visible = false;

                lblTemp.Text = "-";
                lblHumi.Text = "-";

                return;
            }

            bool bVisible = false;
            for (int i = 0; i < arrConn.Count; i++)
            {
                int nConnected = DBUtility2.WebDBManager.GetIntField(arrConn[i].ToString(), 0);

                if (nConnected == 1)
                {
                    bVisible = true;
                    break;
                }
            }

            if (!bVisible)
            {
                lblConn.Visible = true;
                chart1.Visible = false;

                lblTemp.Text = "-";
                lblHumi.Text = "-";

                return;
            }

            chart1.Visible = true;
            lblConn.Visible = false;

            arr = new List<AirQuaility>();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT SensorName, Value, AlarmLimit_1st, AlarmLimit_2nd, Connected, Unit, ShowDidViewer ");
            sb.Append("  FROM Airquaility ");

            ArrayList arrResult = UIManager.Instance.DBMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return;

            bool bShowDidViewer = false;
            for (int i = 0; i < arrResult.Count; i += 7)
            {
                string strSensorName = DBUtility2.WebDBManager.GetStringField(arrResult[i]);
                float nValue = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 1].ToString(), -1.0f);
                float nLimit1 = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 2].ToString(), -1.0f);
                float nLimit2 = DBUtility2.WebDBManager.GetFloatField(arrResult[i + 3].ToString(), -1.0f);
                int nConnected = DBUtility2.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                string strUnit = DBUtility2.WebDBManager.GetStringField(arrResult[i + 5]);
                int nShowDidViewer = DBUtility2.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1); // DidViewer에 실내공기질 정보를 보여줄지 여부

                bool bConnected = (nConnected == 0) ? false : true;
                bShowDidViewer = (nShowDidViewer == 0) ? false : true;

                if (strSensorName == "온도")
                {
                    lblTemp.Text = nValue.ToString();
                    continue;
                }
                else if (strSensorName == "습도")
                {
                    lblHumi.Text = nValue.ToString();
                    continue;
                }
                else
                {
                    AirQuaility air = new AirQuaility();                    
                    float limitPer1 = 0.0f;
                    float limitPer2 = 0.0f;
                    float currentPer = 0.0f;

                    if (strSensorName == "산소") // 산소의 단위는 %임
                    {
                        limitPer1 = nLimit1;
                        limitPer2 = nLimit2;

                        //limitPer1 = limitPer1 - 100;
                        //limitPer2 = limitPer2 - 100;

                        //currentPer = nValue / 100 * 100;

                        currentPer = nValue;
                    }
                    else
                    {
                        limitPer1 = nLimit1 / nLimit2 * 100;
                        limitPer2 = nLimit2 / nLimit2 * 100;

                        currentPer = nValue / nLimit2 * 100;
                    }

                    switch (strSensorName)
                    {
                        case "산소": air.Name = "O₂\r\n" + currentPer + "\r\n" + strUnit + ""; break;
                        case "이산화탄소": air.Name = "CO₂\r\n" + nValue + "\r\n" + strUnit + ""; break;
                        case "일산화탄소": air.Name = "CO\r\n" + nValue + "\r\n" + strUnit + ""; break;
                        case "메탄": air.Name = "CH₄\r\n" + nValue + "\r\n" + strUnit + ""; break;
                    }

                    //switch (strSensorName)
                    //{
                    //    case "산소": air.Name = "O₂"; break;
                    //    case "이산화탄소": air.Name = "CO₂"; break;
                    //    case "일산화탄소": air.Name = "CO"; break;
                    //    case "메탄": air.Name = "CH₄"; break;
                    //}

                    air.Limit1 = limitPer1;
                    air.Limit2 = limitPer2;
                    air.Value = currentPer;

                    arr.Add(air);
                }
            }

            if (cbShowDid.Checked != bShowDidViewer)
                cbShowDid.Checked = bShowDidViewer;

            chart1.DataSource = arr;
            //CCC();
            Chart1_Customize(null, null);
        }

        private void InitChart()
        {
            chart1.Series.Clear();

            System.Windows.Forms.DataVisualization.Charting.Series series = chart1.Series.Add("series1");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            //chart1.Series[0].IsXValueIndexed = true;
            chart1.Series[0].XValueMember = "Name";
            chart1.Series[0].YValueMembers = "Value";
            chart1.Series[0].XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chart1.Series[0].YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            //chart1.Series[0].ToolTip = "#VALX{HH:mm} - #VALY1{0.00}";

            chart1.Series[0].SetCustomProperty("PixelPointWidth", "10");
            chart1.Series[0].SetCustomProperty("PointWidth", "0");

            chart1.Series[0].BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.None;

            chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.FromArgb(0xdd, 0xdb, 0xdb);
            chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.FromArgb(0xdd, 0xdb, 0xdb);
            chart1.ChartAreas[0].AxisX.LineColor = Color.FromArgb(100, 0xe0, 0xe0, 0xe0);
            chart1.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chart1.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("맑은 고딕", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            chart1.ChartAreas[0].AxisY.Interval = 50;
            chart1.ChartAreas[0].AxisY.TitleAlignment = StringAlignment.Near;
            chart1.ChartAreas[0].AxisY.LineColor = Color.FromArgb(150, 0xe0, 0xe0, 0xe0);

            //chart1.ChartAreas[0].AxisY.LabelStyle.Format = "F1";

            chart1.ChartAreas[0].AxisY.IsLabelAutoFit = false;
            chart1.ChartAreas[0].AxisY.LabelAutoFitStyle = System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.IncreaseFont;

            //chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            //chart1.ChartAreas[0].AxisX.IsMarginVisible = false;

            chart1.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.FromArgb(232, 229, 229);
            

            chart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            chart1.ChartAreas[0].BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));

            chart1.Legends.Clear();
        }

        public System.Windows.Forms.DataVisualization.Charting.StripLine GetStripLine(double intervalOffset, StringAlignment textLineAlignment, string textType)
        {
            System.Windows.Forms.DataVisualization.Charting.StripLine stripLine = new System.Windows.Forms.DataVisualization.Charting.StripLine();
            stripLine.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            stripLine.BorderWidth = 1;
            stripLine.TextAlignment = StringAlignment.Near;
            stripLine.TextLineAlignment = textLineAlignment;
            stripLine.IntervalOffset = intervalOffset;
            stripLine.Text = textType + " " + String.Format("{0:F1}", intervalOffset);
            stripLine.BorderColor = Color.FromArgb(147, 188, 228);
            stripLine.ForeColor = Color.FromArgb(147, 188, 228);

            if (textType == "위험")
                stripLine.ForeColor = stripLine.BorderColor = Color.FromArgb(0xf7, 0x53, 0x53);
            else
                stripLine.ForeColor = stripLine.BorderColor = Color.FromArgb(0xff, 0xd3, 0x48);

            return stripLine;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //SDMS.FormMain.TransferExternalForm((int)3);
            libExternalUI.Lib.UIManager.TransferExternalForm((int)3);
            this.Hide();
        }

        private void FormAirQuality_Load(object sender, EventArgs e)
        {
            DisplayAirquaility();
        }

        //private PopupAirQualityLegend pop = null;
        private bool m_bQuestion = false;
        private void btnConfig_Click(object sender, EventArgs e)
        {
            //if (pop == null)
            //    pop = new PopupAirQualityLegend();


            //pop.Show();

            if (!m_bQuestion)
            {
                this.Size = new Size(700, 400);
                this.Location = new Point(this.Location.X - 310, this.Location.Y);

                btnConfig.Location = new Point(594, 11);
                btnClose.Location = new Point(651, 11);
            }
            else
            {
                this.Size = new Size(390, 400);
                this.Location = new Point(this.Location.X + 310, this.Location.Y);

                btnConfig.Location = new Point(282, 10);
                btnClose.Location = new Point(339, 11);
            }

            m_bQuestion = !m_bQuestion;
        }

        private void cbShowDid_CheckedChanged(object sender, EventArgs e)
        {
            int nShow = (cbShowDid.Checked) ? 1 : 0;
            UIManager.Instance.DBMgr.GetResultData("Update AirQuaility Set ShowDidViewer = " + nShow);
        }
    }

    public class AirQuaility
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public float Value { get; set; }
        public float Limit1 { get; set; }
        public float Limit2 { get; set; }
    }
}
