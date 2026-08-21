using ChartDirector;
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

namespace SDMS.PopupDialog
{
    public partial class FormPSMSensorData : Form
    {
        private Dictionary<DateTime, double> m_dicOrgSensorDate = null;
        private Dictionary<int, List<double>> m_dicPageSensorData = null;
        private Dictionary<int, DateTime[]> m_dicPageDate = null;
        private Dictionary<int, List<string>> m_dicPageDateLabel = null;
        private Dictionary<int, List<double>> m_dicPageSensorAlarm1 = null;
        private Dictionary<int, List<double>> m_dicPageSensorAlarm2 = null;
        private Dictionary<int, List<double>> m_dicPageSensorAlarm3 = null;

        private int m_nPageMaxDataCount = 600; // 30분
        private int m_nPageBlankDateSector = 100; // 5분 .. 처음 5분 과 마지막 5분은 시간값을 별도로 표기하지 않도록 한다.

        private UnE.PSM.PSMSensor SelectedSensor
        {
            get
            {
                if (this.cmbSensor.SelectedItem == null)
                    return null;
                else if (this.cmbSensor.SelectedItem is UnE.PSM.PSMSensor == false)
                    return null;
                else
                    return (this.cmbSensor.SelectedItem as UnE.PSM.PSMSensor);
            }
        }
        private UnE.PSM.PSMMaterial SensorMaterial
        {
            get
            {
                if (this.SelectedSensor == null)
                    return null;
                else
                    return PSMManager.Instance.GetMaterial(this.SelectedSensor.MaterialType);
            }
        }

        private int CurrentPageIndex
        {
            get
            {
                if (this.cmbPageIndex.SelectedIndex < 0)
                    return -1;
                else
                    return Convert.ToInt32(this.cmbPageIndex.SelectedItem);
            }
            set
            {
                bool isEnableNext = false;
                bool isEnablePrevious = false;

                if (this.cmbPageIndex.Items.Contains(value))
                {
                    this.cmbPageIndex.SelectedItem = value;

                    if (this.CurrentPageIndex != 1)
                        isEnablePrevious = true;

                    if (this.CurrentPageIndex < this.TotalPageCount)
                        isEnableNext = true;
                }
                else
                {
                    this.cmbPageIndex.SelectedIndex = -1;
                }

                this.btnNext.Enabled =
                this.btnNextTen.Enabled =
                this.btnNextEnd.Enabled = isEnableNext;

                this.btnPrevious.Enabled =
                this.btnPreviousTen.Enabled =
                this.btnPreviousEnd.Enabled = isEnablePrevious;
            }
        }
        private int TotalPageCount = -1;

        public FormPSMSensorData()
        {
            this.DoubleBuffered = true;

            InitializeComponent();

            this.m_dicPageSensorData = new Dictionary<int, List<double>>();
            this.m_dicPageDate = new Dictionary<int, DateTime[]>();
            this.m_dicPageDateLabel = new Dictionary<int, List<string>>();
            this.m_dicPageSensorAlarm1 = new Dictionary<int, List<double>>();
            this.m_dicPageSensorAlarm2 = new Dictionary<int, List<double>>();
            this.m_dicPageSensorAlarm3 = new Dictionary<int, List<double>>();

            InitEvent();
            InitControl();
        }

        public FormPSMSensorData(int nPSMSensorID)
            : this()
        {
            ChoiceSensor(nPSMSensorID);
            this.btnSearch.PerformClick();
        }

        public FormPSMSensorData(int nPSMSensorID, DateTime dtStart, DateTime dtEnd)
            : this()
        {
            ChoiceSensor(nPSMSensorID);
            ChoiceDatePrieod(dtStart, dtEnd);
            this.btnSearch.PerformClick();
        }


        /// <summary>
        /// 검색 컨트롤 기본 데이터 바인딩
        /// </summary>
        private void InitControl()
        {
            LoadDatePrieodData();
            LoadPSMSensorLocationData();
        }

        /// <summary>
        /// 이벤트 선언
        /// </summary>
        private void InitEvent()
        {
            this.Shown += (s, e) => { this.btnSearch.PerformClick(); };
            this.btnDateStart.Click += (s, e) => { ClickButtonDateStart(); };
            this.btnDateEnd.Click += (s, e) => { ClickButtonDateEnd(); };
            this.DatePickerStart.ValueChanged += (s, e) => { SelectDateStart(); };
            this.DatePickerEnd.ValueChanged += (s, e) => { SelectDateEnd(); };
            this.DatePickerStart.CloseUp += (s, e) => { SelectDateStart(); };
            this.DatePickerEnd.CloseUp += (s, e) => { SelectDateEnd(); };
            this.cmbDateFix.SelectedIndexChanged += (s, e) => { SelectDatePrieod(); };
            this.cmbSensorLocation.SelectedIndexChanged += (s, e) => { SelectSensorBuilding(); };
            this.cmbPageIndex.SelectedIndexChanged += (s, e) => { this.CurrentPageIndex = this.CurrentPageIndex; ChangePage(); };
            this.btnSearch.Click += (s, e) => { SearchSensorData(); };
            this.btnPrevious.Click += (s, e) => { PrevPageIndex(1); };
            this.btnPreviousTen.Click += (s, e) => { PrevPageIndex(10); };
            this.btnPreviousEnd.Click += (s, e) => { PrevPageIndex(-1); };
            this.btnNext.Click += (s, e) => { NextPageIndex(1); };
            this.btnNextTen.Click += (s, e) => { NextPageIndex(10); };
            this.btnNextEnd.Click += (s, e) => { NextPageIndex(-1); };
            this.chart.MouseMovePlotArea += (s, e) => { OverMouseOnChart(s as WinChartViewer); };
        }
        

        #region Chart Data Load

        /// <summary>
        /// 검색 조건에 따른 센서 데이터 조회
        /// </summary>
        private void SearchSensorData()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (this.SelectedSensor == null)
                    return;


                string strDateStart = String.Format("{0} {1}", this.btnDateStart.Text, this.TimePickerStart.Value.ToString("HH:mm:ss"));
                string strDateEnd = String.Format("{0} {1}", this.btnDateEnd.Text, this.TimePickerEnd.Value.ToString("HH:mm:ss"));

                DateTime dtStart = DateTime.ParseExact(strDateStart, "yyyy-MM-dd HH:mm:ss", null);
                DateTime dtEnd = DateTime.ParseExact(strDateEnd, "yyyy-MM-dd HH:mm:ss", null);

                this.lblSensorName.Text = String.Format("{0} {1}", this.SelectedSensor.LinkedTankList[0].EquipZone.ZoneName, this.SelectedSensor.ToString().Substring(3));
                this.lblMaterialName.Text = this.SensorMaterial.Name;

                this.m_dicOrgSensorDate = PSMManager.Instance.GetSensorData(this.SelectedSensor, dtStart, dtEnd);

                if (m_dicOrgSensorDate == null)
                    return;

                this.lblSearchDate.Text = String.Format("[ {0}년 {1}월 {2}일 {3} {4}시 {5}분 {6}초 ] 부터 [ {7}년 {8}월 {9}일 {10} {11}시 {12}분 {13}초 ] 까지",
                    dtStart.Year, dtStart.Month, dtStart.Day, (dtStart.ToString("tt").ToUpper().Equals("AM") ? "오전" : "오후"), dtStart.ToString("hh"), dtStart.Minute, dtStart.Second,
                    dtEnd.Year, dtEnd.Month, dtEnd.Day, (dtEnd.ToString("tt").ToUpper().Equals("AM") ? "오전" : "오후"), dtEnd.ToString("hh"), dtEnd.Minute, dtEnd.Second
                    );

                this.m_dicPageSensorData.Clear();
                this.m_dicPageDate.Clear();
                this.m_dicPageDateLabel.Clear();
                this.m_dicPageSensorAlarm1.Clear();
                this.m_dicPageSensorAlarm2.Clear();
                this.m_dicPageSensorAlarm3.Clear();

                ConvertSensorData();
                
                this.TotalPageCount = this.m_dicPageDate.Keys.Count;

                this.cmbPageIndex.Items.Clear();
                for (int i = 1; i <= this.TotalPageCount; i++)
                    this.cmbPageIndex.Items.Add(i);

                this.CurrentPageIndex = 1;

                this.lblTotalPage.Text = String.Format("/ {0}", this.TotalPageCount);

                ChangePage();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 센서 데이터를 차트에 표현 가능한 데이터로 변환
        /// </summary>
        private void ConvertSensorData()
        {
            int nDataCount = 0;
            int nPageNo = 1;
            int nDataTotalCount = 0;

            string strTempLabel = String.Empty;

            double dAlarm1 = Convert.ToDouble(this.SelectedSensor.LimitLevel1);
            double dAlarm2 = Convert.ToDouble(this.SelectedSensor.LimitLevel2);
            double dAlarm3 = Convert.ToDouble(this.SelectedSensor.LimitLevel3);

            foreach (KeyValuePair<DateTime, double> data in this.m_dicOrgSensorDate)
            {
                nDataCount++;
                nDataTotalCount++;

                if (nDataCount == 1)
                {
                    if (this.m_dicPageDate.ContainsKey(nPageNo) == false)
                        this.m_dicPageDate.Add(nPageNo, new DateTime[] { DateTime.Now, DateTime.Now });

                    if (this.m_dicPageDateLabel.ContainsKey(nPageNo) == false)
                        this.m_dicPageDateLabel.Add(nPageNo, new List<string>());

                    if (this.m_dicPageSensorData.ContainsKey(nPageNo) == false)
                        this.m_dicPageSensorData.Add(nPageNo, new List<double>());

                    if (this.m_dicPageSensorAlarm1.ContainsKey(nPageNo) == false)
                        this.m_dicPageSensorAlarm1.Add(nPageNo, new List<double>());

                    if (this.m_dicPageSensorAlarm2.ContainsKey(nPageNo) == false)
                        this.m_dicPageSensorAlarm2.Add(nPageNo, new List<double>());

                    if (this.m_dicPageSensorAlarm3.ContainsKey(nPageNo) == false)
                        this.m_dicPageSensorAlarm3.Add(nPageNo, new List<double>());

                    this.m_dicPageDate[nPageNo][0] = data.Key;
                    this.m_dicPageDateLabel[nPageNo].Add(data.Key.ToString("yyyy-MM-dd HH:mm:ss"));
                    this.m_dicPageSensorData[nPageNo].Add(data.Value);
                    this.m_dicPageSensorAlarm1[nPageNo].Add(dAlarm1);
                    this.m_dicPageSensorAlarm2[nPageNo].Add(dAlarm2);
                    this.m_dicPageSensorAlarm3[nPageNo].Add(dAlarm3);
                }
                else if (nDataCount == m_nPageMaxDataCount || m_dicOrgSensorDate.Count == nDataTotalCount)
                {
                    nDataCount = 0;

                    this.m_dicPageDate[nPageNo][1] = data.Key;
                    this.m_dicPageDateLabel[nPageNo].Add(data.Key.ToString("yyyy-MM-dd HH:mm:ss"));
                    this.m_dicPageSensorData[nPageNo].Add(data.Value);
                    this.m_dicPageSensorAlarm1[nPageNo].Add(dAlarm1);
                    this.m_dicPageSensorAlarm2[nPageNo].Add(dAlarm2);
                    this.m_dicPageSensorAlarm3[nPageNo].Add(dAlarm3);

                    nPageNo++;
                }
                else
                {
                    strTempLabel = String.Empty;

                    if (nDataCount > (m_nPageBlankDateSector) && nDataCount <= (m_nPageMaxDataCount - m_nPageBlankDateSector))
                    {
                        if (data.Key.Minute % 10 == 0 && data.Key.Second < 4)
                            if (this.m_dicPageDateLabel[nPageNo].Contains(data.Key.ToString("yyyy-MM-dd HH:mm")) == false)
                                strTempLabel = data.Key.ToString("yyyy-MM-dd HH:mm");
                    }

                    this.m_dicPageDateLabel[nPageNo].Add(strTempLabel);
                    this.m_dicPageSensorData[nPageNo].Add(data.Value);
                    this.m_dicPageSensorAlarm1[nPageNo].Add(dAlarm1);
                    this.m_dicPageSensorAlarm2[nPageNo].Add(dAlarm2);
                    this.m_dicPageSensorAlarm3[nPageNo].Add(dAlarm3);

                }


            }
        }

        /// <summary>
        /// 차트에 센서 데이터를 표시
        /// </summary>
        private void DrawChart(int nPageNo)
        {
            this.chart.clearAllRanges();
            
            XYChart xyChart = new XYChart(this.chart.Width, this.chart.Height);

            xyChart.yAxis().setTitle(this.SensorMaterial.UOM);
            xyChart.yAxis().setLabelStyle("Arial", 9.75, 0x000000);

            switch (this.SensorMaterial.Name)
            {
                case "염산":
                    xyChart.yAxis().setLinearScale(0, 10);
                    break;

                case "가성소다":
                    xyChart.yAxis().setLinearScale(0, 1);
                    break;

                default:
                    xyChart.yAxis().setLinearScale(0, 100);
                    break;
            }

            xyChart.xAxis().setLabels(this.m_dicPageDateLabel[nPageNo].ToArray());
            xyChart.xAxis().setLabelStyle("Arial", 9, 0x000000);

            // Set the plotarea at (30, 20) and of size 200 x 200 pixels
            xyChart.setPlotArea(80, 40, this.chart.Width - 220, this.chart.Height - 100);

            LegendBox legendBox = xyChart.addLegend(xyChart.getWidth() - 10, 90, true, "Arial", 9);
            legendBox.setAlignment(Chart.TopRight);

            LineLayer layerAlarm3 = xyChart.addLineLayer(this.m_dicPageSensorAlarm3[nPageNo].ToArray(), 0x241CED, "3단계 수치");
            LineLayer layerAlarm2 = xyChart.addLineLayer(this.m_dicPageSensorAlarm2[nPageNo].ToArray(), 0x277FFF, "2단계 수치");
            LineLayer layerAlarm1 = xyChart.addLineLayer(this.m_dicPageSensorAlarm1[nPageNo].ToArray(), 0x1DE6B5, "1단계 수치");
            LineLayer layerData = xyChart.addLineLayer(this.m_dicPageSensorData[nPageNo].ToArray(), 0xE8A200, "측정 데이터");

            layerAlarm3.setLineWidth(2);
            layerAlarm2.setLineWidth(2);
            layerAlarm1.setLineWidth(2);
            layerData.setLineWidth(3);

            this.chart.Chart = xyChart;
        }

        #endregion Chart Data Load


        #region Load Search Data

        private void LoadDatePrieodData()
        {
            this.cmbDateFix.Items.Clear();

            this.cmbDateFix.Items.Add("최근 1시간");
            this.cmbDateFix.Items.Add("최근 6시간");
            this.cmbDateFix.Items.Add("최근 1일");
            this.cmbDateFix.Items.Add("최근 1주");
            this.cmbDateFix.Items.Add("최근 2주");
            this.cmbDateFix.Items.Add("최근 1달");
            this.cmbDateFix.Items.Add("사용자 정의");

            this.cmbDateFix.SelectedIndex = 1;
        }

        /// <summary>
        /// 센서위치 데이터 로드 및 ComboBox 컨트롤에 바인딩
        /// </summary>
        private void LoadPSMSensorLocationData()
        {
            this.cmbSensorLocation.Items.Clear();

            this.cmbSensorLocation.Items.Add("모든 시설");

            foreach (UnE.Spatial.Building building in from buildings in PSMManager.Instance.GetTankBuildings()
                                                      orderby buildings.BuildingName ascending
                                                      select buildings
                                                      )
            {
                this.cmbSensorLocation.Items.Add(building);
            }

            //this.cmbSensorLocation.Items.AddRange(PSMManager.Instance.GetTankBuildings().ToArray());

            this.cmbSensorLocation.SelectedIndex = 0;
        }

        /// <summary>
        /// 센서 데이터 로드 및 ComboBox 컨트롤에 바인딩
        /// </summary>
        /// <param name="nBuildingID"></param>
        private void LoadPSMSensorList(int nBuildingID)
        {
            this.cmbSensor.Items.Clear();

            foreach (UnE.PSM.PSMSensor sensor in from sensors in PSMManager.Instance.GetSensorByBuilding(nBuildingID)
                                                 orderby sensors.Name ascending
                                                 select sensors
                                                 )
            {
                this.cmbSensor.Items.Add(sensor);
            }

            //this.cmbSensor.Items.AddRange(PSMManager.Instance.GetSensorByBuilding(nBuildingID).ToArray());

            if (this.cmbSensor.Items.Count > 0)
            {
                this.cmbSensor.SelectedIndex = 0;
            }
            else
            {
                this.cmbSensor.SelectedIndex = -1;
            }
        }
        
        #endregion Load Search Data


        #region DateTime Function

        private void PopupDateTimePickerControl(Button btn, DateTimePicker datePicker)
        {
            HideDateTimePicker();

            datePicker.Value = Convert.ToDateTime(btn.Text);

            int x = btn.Left;
            int y = btn.Top + btn.Height;

            Point pt = new Point(this.pnHeader.Location.X + x, this.pnHeader.Location.Y + y);
            datePicker.Location = new Point(pt.X, pt.Y);
            datePicker.DropDownAlign = LeftRightAlignment.Left;
            datePicker.Show();

            datePicker.Select();
            SendKeys.Send("%{DOWN}");
        }

        private void ChangeDate(DateTimePicker datePicker, Button btn)
        {
            try
            {
                DateTime dtToday = DateTime.Now;
                string szText = datePicker.Value.ToString("yyyy-MM-dd");
                DateTime dtszText = DateTime.ParseExact(szText, "yyyy-MM-dd", null);

                if (dtszText > dtToday)
                {
                    MessageBox.Show("현재 날짜보다 더 클 수 없습니다.");
                    return;
                }

                if (IsValidDatePrieod() == false)
                {
                    MessageBox.Show("시작 날짜가 종료 날짜보다 클 수 없습니다.");
                    return;
                }

                btn.Text = szText;
                btn.Refresh();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
            finally
            {
                datePicker.Value = Convert.ToDateTime(btn.Text);
                HideDateTimePicker();
            }
        }

        private bool IsValidDatePrieod()
        {
            bool isValid = true;

            DateTime dtStart = DateTime.ParseExact(this.DatePickerStart.Value.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null);
            DateTime dtEnd = DateTime.ParseExact(this.DatePickerEnd.Value.ToString("yyyy-MM-dd"), "yyyy-MM-dd", null);

            if (dtStart > dtEnd)
            {
                isValid = false;
            }

            return isValid;

        }

        private void HideDateTimePicker()
        {
            this.DatePickerStart.Visible = false;
            this.DatePickerEnd.Visible = false;
        }


        private void SelectDatePrieod()
        {
            DateTime dtEnd = DateTime.Now;
            DateTime dtStart = DateTime.Now;

            switch (this.cmbDateFix.SelectedIndex)
            {
                // 최근 1시간
                case 0:
                    dtStart = dtEnd.AddHours(-1);
                    break;
                // 최근 6시간
                case 1:
                    dtStart = dtEnd.AddHours(-6);
                    break;
                // 최근 1일
                case 2:
                    dtStart = dtEnd.AddDays(-1);
                    break;
                // 최근 1주
                case 3:
                    dtStart = dtEnd.AddDays(-7);
                    break;
                // 최근 2주
                case 4:
                    dtStart = dtEnd.AddDays(-14);
                    break;
                // 최근 1달
                case 5:
                    dtStart = dtEnd.AddMonths(-1);
                    break;
                // 사용자 정의
                case 6:
                default:
                    dtStart = DateTime.ParseExact(this.btnDateStart.Text, "yyyy-MM-dd", null);
                    dtEnd = DateTime.ParseExact(this.btnDateEnd.Text, "yyyy-MM-dd", null);
                    break;
            }

            this.btnDateStart.Text = dtStart.ToString("yyyy-MM-dd");
            this.btnDateEnd.Refresh();
            this.TimePickerStart.Value = dtStart;

            this.btnDateEnd.Text = dtEnd.ToString("yyyy-MM-dd");
            this.btnDateEnd.Refresh();
            this.TimePickerEnd.Value = dtEnd;
        }

        private void ClickButtonDateStart()
        {
            PopupDateTimePickerControl(this.btnDateStart, this.DatePickerStart);
        }

        private void ClickButtonDateEnd()
        {
            PopupDateTimePickerControl(this.btnDateEnd, this.DatePickerEnd);
        }

        private void SelectDateStart()
        {
            ChangeDate(this.DatePickerStart, this.btnDateStart);
        }

        private void SelectDateEnd()
        {
            ChangeDate(this.DatePickerEnd, this.btnDateEnd);
        }

        #endregion DateTime Function


        #region Page Index Change Function

        private void ChangePage()
        {
            if (this.CurrentPageIndex == -1)
                return;

            DrawChart(this.CurrentPageIndex);
        }

        private void PrevPageIndex(int nMovePage)
        {
            int nSetPageIndex = 1;

            switch (nMovePage)
            {
                case 1:
                    nSetPageIndex = this.CurrentPageIndex - 1;
                    break;

                case 10:
                    if (this.CurrentPageIndex > 10)
                        nSetPageIndex = this.CurrentPageIndex - 10;

                    break;

                default:
                    break;
            }

            this.CurrentPageIndex = nSetPageIndex;
            ChangePage();
        }

        private void NextPageIndex(int nMovePage)
        {
            int nSetPageIndex = this.TotalPageCount;

            switch (nMovePage)
            {
                case 1:
                    nSetPageIndex = this.CurrentPageIndex + 1;
                    break;

                case 10:
                    if (this.CurrentPageIndex + 10 <= this.TotalPageCount)
                        nSetPageIndex = this.CurrentPageIndex + 10;

                    break;

                default:
                    break;
            }

            this.CurrentPageIndex = nSetPageIndex;
            ChangePage();
        }

        #endregion Page Index Change Function


        #region Drawing Chart

        private void OverMouseOnChart(WinChartViewer chartViewer)
        {
            TrackLineAxis((XYChart)chartViewer.Chart, chartViewer.PlotAreaMouseX);
            chartViewer.updateDisplay();

            chartViewer.removeDynamicLayer("MouseLeavePlotArea");
        }

        private void TrackLineAxis(XYChart c, int nX)
        {
            DrawArea drawArea = c.initDynamicLayer();
            PlotArea plotArea = c.getPlotArea();

            double xValue = c.getNearestXValue(nX);
            int xCoor = c.getXCoor(xValue);

            int minY = plotArea.getBottomY();

            for (int i = 0; i < c.getLayerCount(); ++i)
            {
                Layer layer = c.getLayerByZ(i);

                int xIndex = layer.getXIndexOf(xValue);

                for (int j = 0; j < layer.getDataSetCount(); ++j)
                {
                    ChartDirector.DataSet dataSet = layer.getDataSetByZ(j);

                    if (dataSet.getDataName() != "측정 데이터")
                        break;

                    double dataPoint = dataSet.getPosition(xIndex);
                    if ((dataPoint != Chart.NoValue) && (dataSet.getDataColor() != Chart.Transparent))
                    {
                        minY = Math.Min(minY, c.getYCoor(dataPoint, dataSet.getUseYAxis()));
                    }
                }
            }

            drawArea.vline(Math.Max(minY, plotArea.getTopY()), plotArea.getBottomY() + 6, xCoor, drawArea.dashLineColor(0x000000, 0x0101));
            //drawArea.text("<*font,bgColor=000000*> " + c.xAxis().getFormattedLabel(xValue, "yyyy-MM-dd HH:mm:ss") + " <*/font*>", "Arial Bold", 8).draw(xCoor, plotArea.getBottomY() + 6, 0xffffff, Chart.Top);

            for (int i = 0; i < c.getLayerCount(); ++i)
            {
                Layer layer = c.getLayerByZ(i);

                int xIndex = layer.getXIndexOf(xValue);

                for (int j = 0; j < layer.getDataSetCount(); ++j)
                {
                    ChartDirector.DataSet dataSet = layer.getDataSetByZ(j);

                    if (dataSet.getDataName() != "측정 데이터")
                    {
                        double dataPoint = dataSet.getPosition(xIndex);
                        Axis yAxis = dataSet.getUseYAxis();
                        int yCoor = c.getYCoor(dataPoint, yAxis);
                        int color = dataSet.getDataColor();

                        if ((dataPoint != Chart.NoValue) && (color != Chart.Transparent) && (yCoor >=
                            plotArea.getTopY()) && (yCoor <= plotArea.getBottomY()))
                        {
                            int xPos = yAxis.getX() + 4;
                            drawArea.text("<*font,bgColor=" + color.ToString("x") + "*> " + c.formatValue(dataPoint, "{value|P4}") + " <*/font*>", "Arial Bold", 8).draw(xPos, yCoor, 0xffffff, Chart.Left);
                        }
                    }
                    else
                    {
                        double dataPoint = dataSet.getPosition(xIndex);
                        Axis yAxis = dataSet.getUseYAxis();
                        int yCoor = c.getYCoor(dataPoint, yAxis);
                        int color = dataSet.getDataColor();

                        if ((dataPoint != Chart.NoValue) && (color != Chart.Transparent) && (yCoor >=
                            plotArea.getTopY()) && (yCoor <= plotArea.getBottomY()))
                        {
                            int xPos = yAxis.getX() + 4;
                            drawArea.hline(xCoor, xCoor - 20/*xPos*/, yCoor, drawArea.dashLineColor(color, 0x0101));
                            drawArea.circle(xCoor, yCoor, 4, 4, color, color);
                            drawArea.text("<*font,bgColor=" + color.ToString("x") + "*> " + c.formatValue(dataPoint, "{value|P4}") + " <*/font*>", "Arial Bold", 8).draw(xCoor - 20/*xPos*/, yCoor, 0xffffff, Chart.Left);
                        }
                    }
                }
            }

        }

        #endregion Drawing Chart


        private void SelectSensorBuilding()
        {
            int nBuildingID = -1;

            if (this.cmbSensorLocation.SelectedItem is UnE.Spatial.Building)
            {
                nBuildingID = (this.cmbSensorLocation.SelectedItem as UnE.Spatial.Building).ID;
            }

            LoadPSMSensorList(nBuildingID);

        }

        private void ChoiceSensor(int nPSMSensorID)
        {
            foreach (UnE.PSM.PSMSensor sensor in from sensors in this.cmbSensor.Items.Cast<UnE.PSM.PSMSensor>()
                                                 where sensors.ID == nPSMSensorID
                                                 select sensors
                                                )
            {
                this.cmbSensor.SelectedItem = sensor;
                break;
            }
        }

        private void ChoiceDatePrieod(DateTime dtStart, DateTime dtEnd)
        {
            this.cmbDateFix.SelectedIndex = this.cmbDateFix.Items.Count - 1;

            this.btnDateStart.Text = dtStart.Date.ToString("yyyy-MM-dd");
            this.TimePickerStart.Value = dtStart;

            this.btnDateEnd.Text = dtEnd.Date.ToString("yyyy-MM-dd");
            this.TimePickerEnd.Value = dtEnd;
        }

    }
}
