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
    public partial class FormPSMSensorTrendData : Form
    {

        #region Internal Properties

        private Dictionary<DateTime, Dictionary<DateTime, double>> m_dicOrgSensorData = null;
        private List<double> m_liSensorData = null;
        private List<string> m_liDateLabel = null;
        private List<double> m_liSensorAlarm1 = null;
        private List<double> m_liSensorAlarm2 = null;
        private List<double> m_liSensorAlarm3 = null;

        private Stack<KeyValuePair<DateTime, DateTime>> m_stackUndo = null;
        private Stack<KeyValuePair<DateTime, DateTime>> m_stackRedo = null;
        private KeyValuePair<DateTime, DateTime> m_pairCurrSearchDate;

        // 분간격은 배수로 올라왔을 떄 항상 하루가 되도록 하기 (-1인경우 실시간 데이터로 간주함)
        private double m_nStepMinute = -1;
        // 분간격에 따라서 시 간격되 겹치도록 최소공배수로 지정하기
        private int m_nAddHour = -1;
        // 추가되는 시 간격에 분간격이 몇번이나 들어가는지 확인하여 지정하기 (-1인경우 실시간 데이터로 간주함)
        private int m_nHourLotationCount = -1;

        private UnE.PSM.PSMSensor SelectedSensor = null;

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

        private bool m_isDataLoading = false;
        private bool m_isChartSelecting = false;
        private bool m_isEndClear = false;
        private Point m_ptStartSpot;
        private Point m_ptEndSpsot;

        private Color m_clrLeftMouseDragging = Color.Crimson;
        private Color m_clrRightMouseDragging = Color.Aquamarine;

        private string m_strLastSearchBeginDate = null, m_strLastSearchEndDate = null;

        #endregion Internal Properties


        public FormPSMSensorTrendData()
        {
            this.DoubleBuffered = true;

            InitializeComponent();

            this.m_liSensorData = new List<double>();
            this.m_liDateLabel = new List<string>();
            this.m_liSensorAlarm1 = new List<double>();
            this.m_liSensorAlarm2 = new List<double>();
            this.m_liSensorAlarm3 = new List<double>();

            this.m_stackUndo = new Stack<KeyValuePair<DateTime, DateTime>>();
            this.m_stackRedo = new Stack<KeyValuePair<DateTime, DateTime>>();
            this.m_pairCurrSearchDate = new KeyValuePair<DateTime, DateTime>();

            InitEvent();
            InitControl();
        }

        public FormPSMSensorTrendData(UnE.PSM.PSMSensor sensor)
            : this()
        {
            this.SelectedSensor = sensor;
        }

        public FormPSMSensorTrendData(UnE.PSM.PSMSensor sensor, DateTime dtStartTime, DateTime dtEndTime)
            : this()
        {
            this.SelectedSensor = sensor;
            this.m_pairCurrSearchDate = new KeyValuePair<DateTime, DateTime>(dtStartTime, dtEndTime);
        }


        /// <summary>
        /// 검색 컨트롤 기본 데이터 바인딩
        /// </summary>
        private void InitControl()
        {
            UpdateButtonByRedoUndo();
        }

        /// <summary>
        /// 이벤트 선언
        /// </summary>
        private void InitEvent()
        {
            this.Shown += (s, e) =>
            {
                if (String.Equals(this.m_pairCurrSearchDate.Key.ToString("yyyy-MM-dd HH:mm:ss"), this.m_pairCurrSearchDate.Value.ToString("yyyy-MM-dd HH:mm:ss")))
                    SearchSensorData();
                else
                    SearchSensorData(this.m_pairCurrSearchDate.Key.ToString("yyyy-MM-dd HH:mm:ss"), this.m_pairCurrSearchDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
            };
            this.FormClosing += (s, e) => { this.chart.Dispose(); };

            this.chart.MouseMovePlotArea += (s, e) => { OverMouseOnChart(s as WinChartViewer, e); };
            this.chart.MouseDown += (s, e) => { BeginChartSelection(e.Location, e); };
            this.chart.MouseMoveChart += (s, e) => { OnChartSelection(e.Location, e); };
            this.chart.MouseUp += (s, e) => { EndChartSelection(e); };

            this.btnUndo.Click += (s, e) => { Undo(); };
            this.btnRedo.Click += (s, e) => { Redo(); };
        }



        /// <summary>
        /// 외부에서 조회할 센서 변경
        /// </summary>
        /// <param name="sensor">유해화학물질 센서</param>
        public void ChangeSensor(UnE.PSM.PSMSensor sensor)
        {
            //System.Diagnostics.Trace.WriteLine(sensor.Name);
            ChangeSensor(sensor, null, null);
        }


        public void ChangeSensor(UnE.PSM.PSMSensor sensor, DBUtility.VariousData<DateTime> dtStartTime, DBUtility.VariousData<DateTime> dtEndTime)
        {
            FormMain.Instance.Invoke(new Action(() =>
           {
               if (this.m_isDataLoading == true)
                   return;

               this.Cursor = Cursors.WaitCursor;

               this.m_isDataLoading = true;

               try
               {
                   this.m_stackRedo.Clear();
                   this.m_stackUndo.Clear();
                   UpdateButtonByRedoUndo();

                   this.SelectedSensor = sensor;

                   if (dtStartTime != null && dtEndTime != null)
                   {
                       this.m_pairCurrSearchDate = new KeyValuePair<DateTime, DateTime>(dtStartTime.Data, dtEndTime.Data);
                       SearchSensorData(this.m_pairCurrSearchDate.Key.ToString("yyyy-MM-dd HH:mm:ss"), this.m_pairCurrSearchDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                   }
                   else
                   {
                       SearchSensorData();
                   }
               }
               catch (Exception ex)
               {
                   System.Diagnostics.Trace.WriteLine(ex.Message);
                   throw ex;
               }
               finally
               {
                   m_isDataLoading = false;
                   this.Cursor = Cursors.Default;
               }

           }
           ));
        }


        #region Chart Data Load

        /// <summary>
        /// 검색 조건에 따른 센서 데이터 조회
        /// </summary>
        private void SearchSensorData(string strStrDate = null, string strEndDate = null)
        {
            m_strLastSearchBeginDate = strStrDate;
            m_strLastSearchEndDate = strEndDate;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (this.SelectedSensor == null)
                    return;

                this.lblSensorName.Text = String.Format("{0} {1}", this.SelectedSensor.LinkedTankList[0].EquipZone.ZoneName, this.SelectedSensor.ToString().Substring(3));
                this.lblMaterialName.Text = this.SensorMaterial.Name;

                this.m_dicOrgSensorData = PSMManager.Instance.GetSensorData(ref this.SelectedSensor);

                if (m_dicOrgSensorData == null)
                    return;

                this.m_liSensorData.Clear();
                this.m_liDateLabel.Clear();
                this.m_liSensorAlarm1.Clear();
                this.m_liSensorAlarm2.Clear();
                this.m_liSensorAlarm3.Clear();
                
                // 센서 데이터 계산
                CalcSensorData(strStrDate, strEndDate);
                // 차트 그리기
                DrawChart();

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
        /// 센서 데이터를 차트에 표기할수 있는 데이터로 변환
        /// </summary>
        private void CalcSensorData(string strStrDate, string strEndDate)
        {
            // [ KEY : 기준일자 | VALUE : [ KEY : 기준일자에 따른 데이터 순번(시간값에 의한 Step으로 정확한 일시를 찾는 용도) | VALUE : 시간간격의 최대수치 ] ]
            Dictionary<DateTime, Dictionary<int, double>> dicData = new Dictionary<DateTime, Dictionary<int, double>>();

            // 최대값 저장
            double dTopData = 0;

            DateTime dtMinDate;
            DateTime dtMaxDate;
            DateTime dtTrgDate;
            DateTime dtStrDateTime;
            DateTime dtEndDateTime;
            double dDiffDays;

            bool isEndPoint = false;

            // 조회 시간 설정
            if (String.IsNullOrWhiteSpace(strStrDate) == false && String.IsNullOrWhiteSpace(strEndDate) == false)
            {
                dtMinDate = this.m_pairCurrSearchDate.Key;
                dtMaxDate = this.m_pairCurrSearchDate.Value;

                // 조회 기간일수를 산출하여 간격 지정할것
                TimeSpan ts = dtMaxDate - dtMinDate;
                dDiffDays = ts.TotalDays;

                SetStep(dDiffDays);
            }
            else
            {
                dtMinDate = this.m_dicOrgSensorData[this.m_dicOrgSensorData.Keys.ToArray()[0]].Keys.ToArray()[0];
                dtMaxDate = this.m_dicOrgSensorData[this.m_dicOrgSensorData.Keys.ToArray()[this.m_dicOrgSensorData.Keys.Count - 1]].Keys.ToArray()[this.m_dicOrgSensorData[this.m_dicOrgSensorData.Keys.ToArray()[this.m_dicOrgSensorData.Keys.Count - 1]].Keys.Count - 1];
                //dtMinDate = new DateTime(dtMinDate.Year, dtMinDate.Month, dtMinDate.Day);
                //dtMaxDate = new DateTime(dtMaxDate.Year, dtMaxDate.Month, dtMaxDate.Day).AddDays(1).AddSeconds(-1);

                this.m_pairCurrSearchDate = new KeyValuePair<DateTime, DateTime>(dtMinDate, dtMaxDate);

                dDiffDays = 30;

                SetStep(dDiffDays);
            }

            this.lblSearchDate.Text = String.Format("[ {0}년 {1}월 {2}일 {3} {4}시 {5}분 {6}초 ] 부터 [ {7}년 {8}월 {9}일 {10} {11}시 {12}분 {13}초 ] 까지",
                dtMinDate.Year, dtMinDate.Month, dtMinDate.Day, (dtMinDate.ToString("tt").ToUpper().Equals("AM") ? "오전" : "오후"), dtMinDate.ToString("hh"), dtMinDate.Minute, dtMinDate.Second,
                dtMaxDate.Year, dtMaxDate.Month, dtMaxDate.Day, (dtMaxDate.ToString("tt").ToUpper().Equals("AM") ? "오전" : "오후"), dtMaxDate.ToString("hh"), dtMaxDate.Minute, dtMaxDate.Second
                );
            /////// END


            // 원데이터 저장(가공하지 않고 모든 데이터를 그대로 출력)
            if (this.m_nHourLotationCount == -1)
            {
                dtStrDateTime = new DateTime(dtMinDate.Year, dtMinDate.Month, dtMinDate.Day);
                dtEndDateTime = new DateTime(dtMaxDate.Year, dtMaxDate.Month, dtMaxDate.Day).AddDays(1);

                dicData.Add(dtMinDate, new Dictionary<int, double>());

                int nCount = 0;

                foreach (Dictionary<DateTime, double> data in from datas in this.m_dicOrgSensorData
                                                              where datas.Key >= dtStrDateTime && datas.Key < dtEndDateTime
                                                              orderby datas.Key ascending
                                                              select datas.Value
                                                )
                {
                    foreach (KeyValuePair<DateTime, double> item in from datas in data
                                                                    where datas.Key >= dtMinDate && datas.Key < dtMaxDate
                                                                    orderby datas.Key ascending
                                                                    select datas
                                        )
                    {
                        dicData[dtMinDate].Add(nCount++, item.Value);
                        if (this.m_liDateLabel.Contains(item.Key.ToString("yyyy-MM-dd HH시 mm분")) == false)
                        {
                            this.m_liDateLabel.Add(item.Key.ToString("yyyy-MM-dd HH시 mm분"));
                        }
                        else
                        {
                            this.m_liDateLabel.Add("");
                        }
                    }
                }

                // 데이터가 없을시 처리
                if (dicData[dtMinDate].Keys.Count == 0)
                {
                    for (DateTime dtTrg = dtMinDate; dtTrg <= dtMaxDate; dtTrg = dtTrg.AddMinutes(1))
                    {
                        dicData[dtMinDate].Add(nCount++, 0);
                        this.m_liDateLabel.Add(dtTrg.ToString("yyyy-MM-dd HH시 mm분"));
                    }
                }
                
            }
            // 원데이터 가공처리 후 저장 (일정한 간격에 따라 최대수치만 가져옴.)
            else
            {
                dtStrDateTime = dtMinDate;
                dtEndDateTime = dtMinDate.AddMinutes(this.m_nStepMinute);

                while (dtMinDate < dtMaxDate)
                {
                    if (dtEndDateTime > dtMaxDate)
                    {
                        break;
                    }

                    dicData.Add(dtMinDate, new Dictionary<int, double>());

                    for (int nCount = 0; nCount < this.m_nHourLotationCount; nCount++)
                    {
                        dtStrDateTime = dtMinDate.AddMinutes(this.m_nStepMinute * Convert.ToDouble(nCount));
                        dtEndDateTime = dtMinDate.AddMinutes(this.m_nStepMinute * Convert.ToDouble(nCount + 1));

                        if (dtEndDateTime > dtMaxDate)
                        {
                            isEndPoint = true;
                            //break;
                        }

                        dtTrgDate = new DateTime(dtMinDate.Year, dtMinDate.Month, dtMinDate.Day);

                        dTopData = 0;

                        foreach (Dictionary<DateTime, double> data in from datas in this.m_dicOrgSensorData
                                                                      where datas.Key == dtTrgDate
                                                                      orderby datas.Key ascending
                                                                      select datas.Value
                                                )
                        {
                            foreach (double value in from datas in data
                                                     where datas.Key >= dtStrDateTime && datas.Key < dtEndDateTime
                                                     orderby datas.Value descending
                                                     select datas.Value
                                                )
                            {
                                dTopData = value;
                                break;
                            }

                            dicData[dtMinDate].Add(nCount, dTopData);
                        }

                        // 데이터 누락으로 인해 데이터가 생성되지 않는 경우에는 아래와 같이 최소값으로 데이터를 넣어줌.
                        if (dicData[dtMinDate].ContainsKey(nCount) == false)
                        {
                            dicData[dtMinDate].Add(nCount, dTopData);
                        }

                        if (isEndPoint == true)
                            break;

                    }

                    if (isEndPoint == true)
                        break;

                    dtMinDate = dtMinDate.AddHours(this.m_nAddHour);
                    if (dtMaxDate < dtMinDate)
                    {
                        dtMinDate = dtMaxDate;
                        isEndPoint = true;
                    }

                }
            }
            /////// END



            // 차트에 적용가능한 데이터로 변환
            foreach (KeyValuePair<DateTime, Dictionary<int, double>> pair in dicData)
            {
                int nIndex = 0;

                foreach (double data in pair.Value.Values)
                {
                    AddDateTimeLabel(dDiffDays, pair.Key, nIndex++);
                    this.m_liSensorData.Add(data);

                    // 가성소다의 경우 알람레벨을 별도로 표기하지 않는다.
                    if (this.SensorMaterial.Name != "가성소다")
                    {
                        this.m_liSensorAlarm1.Add(Convert.ToDouble(this.SelectedSensor.LimitLevel1));
                        this.m_liSensorAlarm2.Add(Convert.ToDouble(this.SelectedSensor.LimitLevel2));
                        this.m_liSensorAlarm3.Add(Convert.ToDouble(this.SelectedSensor.LimitLevel3));
                    }

                }

            }
            /////// END

        }

        /// <summary>
        /// 차트에 X축 값으로 보여줄 날짜값 추가(데이터 배열과 길이로 추가함)
        /// </summary>
        /// <param name="dDiffDays">조회기간의 일수 차이</param>
        /// <param name="dt">데이터의 기준 날짜</param>
        /// <param name="nIndex">날짜에 대한 값 순번</param>
        private void AddDateTimeLabel(double dDiffDays, DateTime dt, int nIndex)
        {
            if (this.m_nHourLotationCount == -1)
                return;

            if (dDiffDays >= 7)
            {
                if (this.m_liDateLabel.Contains(dt.ToString("yyyy-MM-dd")) == false)
                    this.m_liDateLabel.Add(dt.ToString("yyyy-MM-dd"));
                else
                    this.m_liDateLabel.Add("");
            }

            else if (dDiffDays >= 4)
            {
                if (this.m_liDateLabel.Count == 0)
                {
                    this.m_liDateLabel.Add(dt.ToString("yyyy-MM-dd HH시"));
                }
                else if (this.m_liDateLabel.Contains(dt.AddMinutes(-(this.m_nStepMinute * 6)).ToString("yyyy-MM-dd HH시")) == true
                    && this.m_liDateLabel.Contains(dt.ToString("yyyy-MM-dd HH시")) == false)
                {
                    this.m_liDateLabel.Add(dt.ToString("yyyy-MM-dd HH시"));
                }
                else
                {
                    this.m_liDateLabel.Add("");
                }
            }
            else if (dDiffDays >= 2)
            {
                if (this.m_liDateLabel.Count == 0)
                {
                    this.m_liDateLabel.Add(dt.ToString("yyyy-MM-dd HH시"));
                }
                else if (this.m_liDateLabel.Contains(dt.AddMinutes(-(this.m_nStepMinute * 4)).ToString("yyyy-MM-dd HH시")) == true
                    && this.m_liDateLabel.Contains(dt.ToString("yyyy-MM-dd HH시")) == false)
                {
                    this.m_liDateLabel.Add(dt.ToString("yyyy-MM-dd HH시"));
                }
                else
                {
                    this.m_liDateLabel.Add("");
                }
            }
            else if (dDiffDays >= 1)
            {
                if (this.m_liDateLabel.Count == 0)
                {
                    this.m_liDateLabel.Add(dt.ToString("yyyy-MM-dd HH시"));
                }
                else if (this.m_liDateLabel.Contains(dt.AddMinutes(-(this.m_nStepMinute * 4)).ToString("yyyy-MM-dd HH시")) == true
                    && this.m_liDateLabel.Contains(dt.ToString("yyyy-MM-dd HH시")) == false)
                {
                    this.m_liDateLabel.Add(dt.ToString("yyyy-MM-dd HH시"));
                }
                else
                {
                    this.m_liDateLabel.Add("");
                }
            }
            else if (dDiffDays >= 0.5)
            {
                DateTime dtLabelDate = dt.AddMinutes(Convert.ToDouble(nIndex) * this.m_nStepMinute);

                if (this.m_liDateLabel.Count == 0)
                {
                    this.m_liDateLabel.Add(dt.ToString("yyyy-MM-dd HH시"));
                }
                else if (this.m_liDateLabel.Contains(dtLabelDate.ToString("yyyy-MM-dd HH시")) == false)
                {
                    //else if (this.m_liDateLabel.Contains(dt.AddMinutes(-(this.m_nStepMinute * 4)).ToString("yyyy-MM-dd HH시")) == true
                    //    && this.m_liDateLabel.Contains(dt.ToString("yyyy-MM-dd HH시")) == false)
                    //{
                    this.m_liDateLabel.Add(dtLabelDate.ToString("yyyy-MM-dd HH시"));
                }
                else
                {
                    this.m_liDateLabel.Add("");
                }
            }
            else
            {
                DateTime dtLabelDate = dt.AddMinutes(Convert.ToDouble(nIndex) * this.m_nStepMinute);

                if (this.m_liDateLabel.Count == 0)
                {
                    this.m_liDateLabel.Add(dt.ToString("yyyy-MM-dd HH시 mm분"));
                }
                else if ((dtLabelDate.Minute == 30 || dtLabelDate.Minute == 0)
                    && this.m_liDateLabel.Contains(dtLabelDate.ToString("yyyy-MM-dd HH시 mm분")) == false)
                {
                    this.m_liDateLabel.Add(dtLabelDate.ToString("yyyy-MM-dd HH시 mm분"));
                }
                else
                {
                    this.m_liDateLabel.Add("");
                }
            }
        }

        /// <summary>
        /// 조회기간 일수차이에 따른 데이터 색출 범위
        /// </summary>
        /// <param name="dDiffDays">조회기간의 일수 차이</param>
        private void SetStep(double dDiffDays)
        {
            if (dDiffDays >= 15)
            {
                this.m_nAddHour = 4;
                this.m_nStepMinute = 240;
                this.m_nHourLotationCount = 1;
            }
            else if (dDiffDays >= 7)
            {
                this.m_nAddHour = 2;
                this.m_nStepMinute = 120;
                this.m_nHourLotationCount = 1;
            }
            else if (dDiffDays >= 4)
            {
                this.m_nAddHour = 1;
                this.m_nStepMinute = 60;
                this.m_nHourLotationCount = 1;
            }
            else if (dDiffDays >= 2)
            {
                this.m_nAddHour = 1;
                this.m_nStepMinute = 30;
                this.m_nHourLotationCount = 2;
            }
            else if (dDiffDays >= 1)
            {
                this.m_nAddHour = 1;
                this.m_nStepMinute = 15;
                this.m_nHourLotationCount = 4;
            }
            else if (dDiffDays >= 0.5)
            {
                this.m_nAddHour = 1;
                this.m_nStepMinute = 1;
                this.m_nHourLotationCount = 60;
            }
            else if (dDiffDays > ((double)1 / (double)24))
            {
                this.m_nAddHour = 1;
                this.m_nStepMinute = 0.5;
                this.m_nHourLotationCount = 120;
            }
            else
            {
                this.m_nAddHour = 1;
                this.m_nStepMinute = -1;
                this.m_nHourLotationCount = -1;
            }
        }

        #endregion Chart Data Load


        #region Drawing Chart

        /// <summary>
        /// 차트에 센서 데이터를 표시
        /// </summary>
        private void DrawChart()
        {
            // Y축 타이틀 지정
            string strTitle = "{0} / {1}{2}";
            int nHeightLength = 125;


            if (this.SensorMaterial.Name == "가성소다")
                strTitle = "{1}{2}";


            if (this.m_nStepMinute >= 60)
                strTitle = String.Format(strTitle, this.SensorMaterial.UOM, Convert.ToInt32(this.m_nStepMinute / 60), "Hour");
            else if (this.m_nStepMinute >= 1)
                strTitle = String.Format(strTitle, this.SensorMaterial.UOM, Convert.ToInt32(this.m_nStepMinute), "Minute");
            else if (this.m_nStepMinute == -1)
                strTitle = this.SensorMaterial.UOM;
            else
                strTitle = String.Format(strTitle, this.SensorMaterial.UOM, Convert.ToInt32(this.m_nStepMinute * 60), "Second");


            // X축 날짜 길이에 따라 차트 높이 변경
            foreach (string item in this.m_liDateLabel)
            {
                if (String.IsNullOrWhiteSpace(item) == false)
                {
                    nHeightLength = Convert.ToInt32(Math.Ceiling(this.CreateGraphics().MeasureString(item, this.Font).Width) * 1.45);
                    break;
                }
            }


            this.chart.clearAllRanges();

            XYChart xyChart = new XYChart(this.chart.Width, this.chart.Height);
            xyChart.setPlotArea(80, 40, this.chart.Width - 220, this.chart.Height - nHeightLength);
            xyChart.yAxis().setTitle(strTitle);
            xyChart.yAxis().setLabelStyle("Arial", 9.75, 0x000000);

            switch (this.SensorMaterial.Name)
            {
                case "염산":
                    xyChart.yAxis().setLinearScale(-1, 11, new string[] { "", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "" });
                    break;

                case "가성소다":
                    xyChart.yAxis().setLinearScale(-1, 2, new string[] { "", "OFF", "ON", "" });
                    break;

                default:
                    xyChart.yAxis().setLinearScale(-10, 110, new string[] { "", "0", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100", "" });
                    break;
            }

            xyChart.xAxis().setLabels(this.m_liDateLabel.ToArray());
            xyChart.xAxis().setLabelStyle("Arial", 9, 0x000000, 75);

            LegendBox legendBox = xyChart.addLegend(xyChart.getWidth() - 10, 90, true, "Arial", 9);
            legendBox.setAlignment(Chart.TopRight);

            if (this.SensorMaterial.Name != "가성소다")
            {
                LineLayer layerAlarm3 = xyChart.addLineLayer(this.m_liSensorAlarm3.ToArray(), 0x241CED, "3단계 수치");
                LineLayer layerAlarm2 = xyChart.addLineLayer(this.m_liSensorAlarm2.ToArray(), 0x277FFF, "2단계 수치");
                LineLayer layerAlarm1 = xyChart.addLineLayer(this.m_liSensorAlarm1.ToArray(), 0x1DE6B5, "1단계 수치");

                layerAlarm3.setLineWidth(3);
                layerAlarm2.setLineWidth(3);
                layerAlarm1.setLineWidth(3);
            }

            xyChart.addTitle("유해화학물질 측정 데이터 추이", "Times New Roman Bold", 18);

            LineLayer layer = xyChart.addLineLayer(this.m_liSensorData.ToArray(), 0xFF0000, "측정 데이터");
            layer.setLineWidth(2);
            // 스무스한 라인 그리기를 할 경우 아래로
            //SplineLayer layerData = xyChart.addSplineLayer(new ArrayMath(this.m_liSensorData.ToArray()).lowess(0.05, 2).result(), 0xFF0000, "측정 데이터");
            //layerData.setLineWidth(2);

            // Output the chart
            this.chart.Chart = xyChart;
        }


        /// <summary>
        /// 차트 위에 데이터 표기
        /// </summary>
        /// <param name="chartViewer">chart</param>
        private void OverMouseOnChart(WinChartViewer chartViewer, MouseEventArgs e)
        {
            // 영역을 선택하는 중에는 측정데이터를 표기하는 함수를 사용 안함.
            if (m_isChartSelecting)
                return;

            long size = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;

            // 1GB메모리가 차면 메모리가 확보되기전까지 드로윙작업을 중지하고 차트컨트롤에 대한 메모리 정리
            if (size > (1024 * 1024 * 1024))
            {
                //System.Diagnostics.Trace.WriteLine("No Drawing");
                if (m_isEndClear == false)
                {
                    System.Diagnostics.Trace.WriteLine(String.Format("PSMSensorDataPopup MEM : {0}", size));
                    GC.Collect();
                    long sizeAfter = size - System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
                    System.Diagnostics.Trace.WriteLine(String.Format("Clear Memory : {0}", sizeAfter));
                    //GC.WaitForPendingFinalizers();

                    //chartViewer.updateDisplay();
                    m_isEndClear = true;
                }
                return;
            }

            m_isEndClear = false;

            TrackLineAxis((XYChart)chartViewer.Chart, chartViewer.PlotAreaMouseX);
            chartViewer.updateDisplay();

            chartViewer.removeDynamicLayer("MouseLeavePlotArea");
        }

        /// <summary>
        /// 마우스 위치에서 가장 근사한 지점에 있는 데이터 색출 및 그리기
        /// </summary>
        /// <param name="c">chart</param>
        /// <param name="nX">x coordinate of the mouse</param>
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

                    //if (dataSet.getDataName() != "측정 데이터")
                    //{
                    //    double dataPoint = dataSet.getPosition(xIndex);
                    //    Axis yAxis = dataSet.getUseYAxis();
                    //    int yCoor = c.getYCoor(dataPoint, yAxis);
                    //    int color = dataSet.getDataColor();

                    //    if ((dataPoint != Chart.NoValue) && (color != Chart.Transparent) && (yCoor >=
                    //        plotArea.getTopY()) && (yCoor <= plotArea.getBottomY()))
                    //    {
                    //        int xPos = yAxis.getX() + 4;
                    //        drawArea.text("<*font,bgColor=" + color.ToString("x") + "*> " + c.formatValue(dataPoint, "{value|P4}") + " <*/font*>", "Arial Bold", 8).draw(xPos, yCoor, 0xffffff, Chart.Left);
                    //    }
                    //}
                    //else
                    if (dataSet.getDataName() == "측정 데이터")
                    {
                        double dataPoint = dataSet.getPosition(xIndex);
                        Axis yAxis = dataSet.getUseYAxis();
                        int yCoor = c.getYCoor(dataPoint, yAxis);
                        int color = dataSet.getDataColor();

                        if ((dataPoint != Chart.NoValue) && (color != Chart.Transparent) && (yCoor >=
                            plotArea.getTopY()) && (yCoor <= plotArea.getBottomY()))
                        {
                            int xPos = yAxis.getX() + 4;
                            string strPrintValue = c.formatValue(dataPoint, "{value|P4}");

                            if (this.SensorMaterial.Name == "가성소다")
                            {
                                strPrintValue = (strPrintValue == "0") ? "OFF" : "ON";
                            }


                            drawArea.hline(xCoor, xCoor - 20/*xPos*/, yCoor, drawArea.dashLineColor(color, 0x0101));
                            drawArea.circle(xCoor, yCoor, 4, 4, color, color);
                            drawArea.text("<*font,bgColor=" + color.ToString("x") + "*> " + strPrintValue + " <*/font*>", "Arial Bold", 8).draw(xCoor - 20/*xPos*/, yCoor, 0xffffff, Chart.Left);
                        }
                    }
                }
            }

        }


        /// <summary>
        /// 확대 영역 지정 시작
        /// </summary>
        /// <param name="pt"></param>
        private void BeginChartSelection(Point pt, MouseEventArgs e)
        {
            if (this.m_nHourLotationCount == -1)
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Right)
                    return;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                return;

            if (this.m_isChartSelecting == false)
            {
                this.m_isChartSelecting = true;
                this.m_ptStartSpot = pt;
                this.m_ptEndSpsot = pt;
            }
        }

        /// <summary>
        /// 확대할 영역 지정
        /// </summary>
        /// <param name="pt"></param>
        private void OnChartSelection(Point pt, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                return;

            if (m_isChartSelecting == true)
            {
                long size = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;

                // 1GB메모리가 차면 메모리가 확보되기전까지 드로윙작업을 중지하고 차트컨트롤에 대한 메모리 정리
                if (size > (1024 * 1024 * 1024))
                {
                    //System.Diagnostics.Trace.WriteLine("No Selecting");
                    if (m_isEndClear == false)
                    {
                        System.Diagnostics.Trace.WriteLine(String.Format("PSMSensorDataPopup MEM : {0}", size));
                        GC.Collect();
                        long sizeAfter = size - System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
                        System.Diagnostics.Trace.WriteLine(String.Format("Clear Memory : {0}", sizeAfter));
                        //GC.WaitForPendingFinalizers();

                        //chartViewer.updateDisplay();
                        m_isEndClear = true;
                    }
                    return;
                }

                m_isEndClear = false;

                XYChart c = this.chart.Chart as XYChart;
                DrawArea drawArea = c.initDynamicLayer();

                this.m_ptEndSpsot = pt;

                int nColor1 = e.Button == System.Windows.Forms.MouseButtons.Left ? m_clrLeftMouseDragging.GetHashCode() : m_clrRightMouseDragging.GetHashCode();
                int nColor2 = e.Button == System.Windows.Forms.MouseButtons.Left ? Color.FromArgb(222, m_clrLeftMouseDragging).GetHashCode() : Color.FromArgb(222, m_clrRightMouseDragging).GetHashCode();

                drawArea.polygon(
                    new int[] { this.m_ptStartSpot.X, this.m_ptEndSpsot.X, this.m_ptEndSpsot.X, this.m_ptStartSpot.X, this.m_ptStartSpot.X },
                    new int[] { this.m_ptStartSpot.Y, this.m_ptStartSpot.Y, this.m_ptEndSpsot.Y, this.m_ptEndSpsot.Y, this.m_ptStartSpot.Y },
                    nColor1,
                    //Color.Crimson.GetHashCode(),
                    nColor2
                    //Color.FromArgb(222, Color.Crimson).GetHashCode()
                    );

                this.chart.updateDisplay();
            }
        }

        private DateTime GetGraphTime(XYChart c, int x)
        {
            if (c == null)
                return DateTime.Now;

            Axis xAxis = c.xAxis();
            double[] datas = xAxis.getTicks();
            double dCurrentValue = c.getXValue(x);

            string strTime = "";

            if (dCurrentValue == 0.0)
                strTime = xAxis.getLabel(dCurrentValue);
            else
            {
                int nDataCount = datas.Count();

                for (int i=1;i<=nDataCount;i++)
                {
                    double data = i == nDataCount ? xAxis.getMaxValue() : datas[i];

                    if (data == dCurrentValue)
                    {
                        strTime = xAxis.getLabel(dCurrentValue);
                        break;
                    }
                    else if (dCurrentValue > datas[i - 1] && dCurrentValue < data)
                    {
                        DateTime dtBegin, dtEnd;
                        string strBegin = xAxis.getLabel(datas[i - 1]);
                        string strEnd = xAxis.getLabel(data);

                        if (DateTime.TryParse(strBegin, out dtBegin) && DateTime.TryParse(strEnd, out dtEnd))
                            return GetTime(dtBegin, dtEnd, datas[i - 1], data, dCurrentValue);
                        else
                            return GetTime(m_pairCurrSearchDate.Key, m_pairCurrSearchDate.Value, xAxis.getMinValue(), xAxis.getMaxValue(), dCurrentValue);
                    }
                }
            }

            if (strTime == "")
                return GetTime(m_pairCurrSearchDate.Key, m_pairCurrSearchDate.Value, xAxis.getMinValue(), xAxis.getMaxValue(), dCurrentValue);

            DateTime dtCurrent;

            if (DateTime.TryParse(strTime, out dtCurrent))
                return dtCurrent;

            return GetTime(m_pairCurrSearchDate.Key, m_pairCurrSearchDate.Value, xAxis.getMinValue(), xAxis.getMaxValue(), dCurrentValue);
        }

        private DateTime GetTime(DateTime dtBegin, DateTime dtEnd, double dBegin, double dEnd, double dCurrent)
        {
            TimeSpan span = dtEnd - dtBegin;
            double dTotalSeconds = span.TotalSeconds;
            double dCurrentSeconds = dTotalSeconds * (dCurrent - dBegin) / (dEnd - dBegin);

            return dtBegin.AddSeconds(dCurrentSeconds);
        }

        /// <summary>
        /// 확대 영역 지정 종료 및 차트 갱신(사용자에게 우선 질의)
        /// </summary>
        private void EndChartSelection(MouseEventArgs e)
        {
            if (this.m_isChartSelecting == false)
                return;

            this.m_isChartSelecting = false;

            XYChart c = this.chart.Chart as XYChart;
            //PlotArea plotArea = c.getPlotArea();

            DateTime dtStrDate = GetGraphTime(c, this.m_ptStartSpot.X);
            DateTime dtEndDate = GetGraphTime(c, this.m_ptEndSpsot.X);

            if (dtStrDate < m_pairCurrSearchDate.Key)
                dtStrDate = m_pairCurrSearchDate.Key;

            if (dtEndDate > m_pairCurrSearchDate.Value)
                dtEndDate = m_pairCurrSearchDate.Value;

            /*int nMinXSpot = c.getLayerByZ(0).getXIndexOf(c.getNearestXValue(Math.Min(this.m_ptStartSpot.X, this.m_ptEndSpsot.X)));
            int nMaxXSpot = c.getLayerByZ(0).getXIndexOf(c.getNearestXValue(Math.Max(this.m_ptStartSpot.X, this.m_ptEndSpsot.X)));
            
            int nPartitionCount = 0;

            DateTime dtStrDate = DateTime.Now;
            DateTime dtEndDate = DateTime.Now;

            nPartitionCount = 0;
            for (int nIndex = nMinXSpot; nIndex > -1; nIndex--)
            {
                if (String.IsNullOrWhiteSpace(this.m_liDateLabel[nIndex]) == false)
                {
                    if (DateTime.TryParseExact(this.m_liDateLabel[nIndex], "yyyy-MM-dd HH시 mm분", null, System.Globalization.DateTimeStyles.None, out dtStrDate))
                    {
                        dtStrDate = dtStrDate.AddMinutes(nPartitionCount * this.m_nStepMinute);
                        break;
                    }
                    else if (DateTime.TryParseExact(this.m_liDateLabel[nIndex], "yyyy-MM-dd HH시", null, System.Globalization.DateTimeStyles.None, out dtStrDate))
                    {
                        dtStrDate = dtStrDate.AddMinutes(nPartitionCount * this.m_nStepMinute);
                        break;
                    }
                    else if (DateTime.TryParseExact(this.m_liDateLabel[nIndex], "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dtStrDate))
                    {
                        dtStrDate = dtStrDate.AddMinutes(nPartitionCount * this.m_nStepMinute);
                        break;
                    }
                }

                nPartitionCount++;
            }

            nPartitionCount = 0;
            int nAddCount = 0;

            for (int nIndex = nMaxXSpot; nIndex > -1; nIndex--)
            {
                if (nMaxXSpot + 1 == this.m_liDateLabel.Count)
                    nAddCount = 1;

                if (String.IsNullOrWhiteSpace(this.m_liDateLabel[nIndex]) == false)
                {
                    if (DateTime.TryParseExact(this.m_liDateLabel[nIndex], "yyyy-MM-dd HH시 mm분", null, System.Globalization.DateTimeStyles.None, out dtEndDate))
                    {
                        dtEndDate = dtEndDate.AddMinutes((nPartitionCount + nAddCount) * this.m_nStepMinute);
                        break;
                    }
                    else if (DateTime.TryParseExact(this.m_liDateLabel[nIndex], "yyyy-MM-dd HH시", null, System.Globalization.DateTimeStyles.None, out dtEndDate))
                    {
                        dtEndDate = dtEndDate.AddMinutes((nPartitionCount + nAddCount) * this.m_nStepMinute);
                        break;
                    }
                    else if (DateTime.TryParseExact(this.m_liDateLabel[nIndex], "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dtEndDate))
                    {
                        dtEndDate = dtEndDate.AddMinutes((nPartitionCount + nAddCount) * this.m_nStepMinute);
                        break;
                    }

                    break;
                }

                nPartitionCount++;
            }*/

            if (dtEndDate > DateTime.Now)
                dtEndDate = DateTime.Now;

            TimeSpan ts = dtEndDate - dtStrDate;

            if (ts.TotalDays >= ((double)1 / (double)720))
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if (MessageBox.Show(
                        String.Format("[ {0} ] 부터 [ {1} ] 까지의\r\n기간에 대해 데이터를 상세하게 보시겠습니까?", dtStrDate.ToString("yyyy-MM-dd HH시 mm분"), dtEndDate.ToString("yyyy-MM-dd HH시 mm분")), "측정 데이터 상세보기", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.m_stackUndo.Push(this.m_pairCurrSearchDate);
                        this.m_stackRedo.Clear();

                        this.m_pairCurrSearchDate = new KeyValuePair<DateTime, DateTime>(dtStrDate, dtEndDate);

                        UpdateButtonByRedoUndo();

                        SearchSensorData(this.m_pairCurrSearchDate.Key.ToString("yyyy-MM-dd HH:mm:ss"), this.m_pairCurrSearchDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                }
                else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    string strPassword = GetPSMSensorLogDeletePassword();

                    if (strPassword != null)
                    {
                        if (MessageBox.Show(
                            String.Format("[ {0} ] 부터 [ {1} ] 까지의\r\n기간에 대한 데이터를 모두 삭제하시겠습니까?\r\n한번 삭제한 데이터는 되돌릴 수 없습니다.", dtStrDate.ToString("yyyy-MM-dd HH시 mm분"), dtEndDate.ToString("yyyy-MM-dd HH시 mm분")), "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
                        {
                            FormInputPassword frm = new FormInputPassword("데이터 삭제를 위한 암호를 입력하세요.");

                            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                string strInput = frm.Password.GetHashCode().ToString();

                                if (strInput != strPassword)
                                    MessageBox.Show("암호가 일치하지 않습니다.");
                                else
                                {
                                    RemoveSensorData(dtStrDate, dtEndDate);
                                }
                            }
                        }
                    }
                }
            }
            
            DrawArea drawArea = c.initDynamicLayer();
            this.chart.updateDisplay();
        }

        /// <summary>
        /// 검색 조건에 따른 센서 데이터 조회
        /// </summary>
        private void RemoveSensorData(DateTime dtBegin, DateTime dtEnd)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                if (this.SelectedSensor == null)
                    return;

                PSMManager.Instance.RemoveSensorValueDBData(this.SelectedSensor.SensorValueIndex, dtBegin, dtEnd);
                SearchSensorData(m_strLastSearchBeginDate, m_strLastSearchEndDate);
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

        private string GetPSMSensorLogDeletePassword()
        {
            string strSQL = "Select PropertyValue from OptionSDMS where PropertyName = 'PSMSensorLogDeletePassword' and SiteID = " + UnE.SOP.ProxySOP.Instance.SiteID.ToString();
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            return DBUtility.WebDBManager.GetStringField(arrResult[0]);
        }

        #endregion Drawing Chart


        #region Re / Un do

        /// <summary>
        /// 확대한 영역 축소하기
        /// </summary>
        private void Undo()
        {
            this.m_stackRedo.Push(this.m_pairCurrSearchDate);
            this.m_pairCurrSearchDate = this.m_stackUndo.Pop();

            UpdateButtonByRedoUndo();

            SearchSensorData(this.m_pairCurrSearchDate.Key.ToString("yyyy-MM-dd HH:mm:ss"), this.m_pairCurrSearchDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        }


        /// <summary>
        /// 축소한 영역 확대하기
        /// </summary>
        private void Redo()
        {
            this.m_stackUndo.Push(this.m_pairCurrSearchDate);
            this.m_pairCurrSearchDate = this.m_stackRedo.Pop();

            UpdateButtonByRedoUndo();

            SearchSensorData(this.m_pairCurrSearchDate.Key.ToString("yyyy-MM-dd HH:mm:ss"), this.m_pairCurrSearchDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));
        }


        /// <summary>
        /// 리두 언두 버튼 갱신
        /// </summary>
        private void UpdateButtonByRedoUndo()
        {
            if (this.m_stackUndo.Count > 0)
            {
                this.btnUndo.Enabled = true;
                this.btnUndo.Image = global::SDMS.Properties.Resources.되돌리기_normal;
            }
            else
            {
                this.btnUndo.Enabled = false;
                this.btnUndo.Image = global::SDMS.Properties.Resources.되돌리기_checked;
            }

            if (this.m_stackRedo.Count > 0)
            {
                this.btnRedo.Enabled = true;
                this.btnRedo.Image = global::SDMS.Properties.Resources.다시실행_normal;
            }
            else
            {
                this.btnRedo.Enabled = false;
                this.btnRedo.Image = global::SDMS.Properties.Resources.다시실행_checked;
            }
        }

        #endregion Re / Un do

    }
}