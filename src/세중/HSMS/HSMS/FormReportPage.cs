using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ChartDirector;
using System.Collections;

namespace HSMS
{
    public partial class FormReportPage : Form
    {
        private DataReport m_DataReport = null;
        public FormReportPage(DataReport dataReport)
        {
            InitializeComponent();

            m_DataReport = dataReport;

        }

        private string[] labels = null;
        private double[] data = null;

        private void setChart()
        {
            int nParentWidth = this.Size.Width;
            int nSpace = 60;

            Size sizeGrid = this.gridHistory.Size;
            Point ptGrid = this.gridHistory.Location;

            XYChart c = new XYChart(sizeGrid.Width, 300);

            // Set the plotarea at (30, 20) and of size 200 x 200 pixels
            c.setPlotArea(ptGrid.X + 40, 50, sizeGrid.Width - nSpace * 2, 200);

            // Set the default line width to 2 pixels

            c.addLegend(50, 0, false, "Arial Bold", 9).setBackground(Chart.Transparent);
            c.yAxis().setTitle("발생 건수");
            c.xAxis().setLabels(labels);
            LineLayer layer1 = c.addLineLayer(data, 0xff0000, "알람 발생 빈도");
            layer1.setLineWidth(2);

            winChartViewer1.Chart = c;
        }

        private void SetGridView()
        {
            for (int i = 0; i < gridHistory.Columns.Count; i++)
            {
                gridHistory.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            for (int i = 0; i < gridHistory.Columns.Count; i++)
            {
                gridHistory.Columns[i].SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            }

            int nHeight = this.Height;
            
            gridHistory.Location = new Point(gridHistory.Location.X, winChartViewer1.Location.Y + winChartViewer1.Size.Height + 20);
            int nGridHeight = (nHeight - 20) - gridHistory.Location.Y;

            gridHistory.Height = nGridHeight;
        }

        public void CreateLineChart(DateTime dtStartDate, DateTime dtEndDate)
        {
            //두 날짜의 달 차이 계산
            int m_ts = 12 * (dtEndDate.Year - dtStartDate.Year) + (dtEndDate.Month - dtStartDate.Month);

            //시작일과 종료일의 시간 차 계산
            TimeSpan ts = dtEndDate - dtStartDate;
            int nTotalToday = ts.Days;

            ArrayList arrDays = new ArrayList();
            ArrayList y_arr = new ArrayList();

            int ntsTime = 0;
            int nTotalTime = 0;

            bool bisToday = false;

            //기간에 오늘 날짜가 포함인가?
            if (dtEndDate.ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                bisToday = true;
            }

            //날짜 차이가 6일 아래일 경우
            if (nTotalToday <= 5)
            {
                arrDays.Add(dtStartDate.Month.ToString() + "월 " + dtStartDate.Day.ToString() + "일 " + "0시");
                y_arr.Add(0);

                ntsTime = nTotalToday;
                if (bisToday == false)
                {
                    //일+시간
                    nTotalTime = (ntsTime + 1) * 24;
                }
                else
                {
                    //날짜에 오늘이 포함되어있을 경우

                    //(일-1) * 24시간 + 오늘 하루동안 시간
                    nTotalTime = ntsTime * 24 + DateTime.Now.Hour;
                }

                DateTime dtTemp = DateTime.Now;

                for (int k = 1; k < 7; k++)
                {
                    int nCount = 0;

                    //구한 Time
                    int nAddTime = (nTotalTime * k) / 6;
                    int nRest = (nTotalTime * k) % 6;

                    //몫/2
                    int nTemp = nAddTime / 2;
                    //나머지 < (nAddTime/2)
                    if (nRest > nTemp)
                    {
                        nAddTime++;
                    }

                    //기존 DateTime에서 nAddTime을 더함(X축데이터 구함)
                    DateTime dtNextDate = dtStartDate.AddHours(nAddTime);

                    if (k == 6 && bisToday == true)
                    {
                        dtNextDate = DateTime.Now;
                        arrDays.Add(dtNextDate.Month.ToString() + "월 " + dtNextDate.Day.ToString() + "일 " + dtNextDate.Hour + "시");
                    }
                    else
                    {
                        arrDays.Add(dtNextDate.Month + "월 " + dtNextDate.Day + "일 " + dtNextDate.Hour + "시");
                    }


                    DateTime dtMaxDate = dtNextDate;
                    DateTime dtMinDate = DateTime.Now;

                    if (k == 1)
                        dtMinDate = dtStartDate;
                    else
                        dtMinDate = dtTemp;


                    int nResultCount = 0;


                    foreach (ReportHistory history in m_DataReport.ReportDataList)
                    {
                        if (history.Time >= dtMinDate && history.Time <= dtMaxDate)
                        {
                            nCount++;
                        }

                    }
                    nResultCount = nCount;
                    y_arr.Add(nResultCount);

                    //
                    dtTemp = dtNextDate;
                }
                labels = new string[7];
                data = new double[7];
                int n_count = 0;

                foreach (string x in arrDays)
                {
                    labels[n_count] = x;

                    n_count++;
                }
                n_count = 0;
                foreach (int y in y_arr)
                {
                    data[n_count] = y;
                    n_count++;
                }
            }
            else
            {
                //최근 6개월
                string strNowDate = "";
                string strBeforeDate = "";

                int nMonthCount = 0;
                TimeSpan tsSubMonth = dtEndDate - dtEndDate.AddMonths(-6);
                nMonthCount = tsSubMonth.Days;

                ArrayList arrMonth = new ArrayList();

                arrDays.Add(dtEndDate.ToShortDateString().ToString());

                DateTime defaultdt = new DateTime();
                for (int i = 0; i < 6; i++)
                {
                   
                    int ndays = (nTotalToday * (i + 1)) / 6;
                    defaultdt = dtEndDate.AddDays(-ndays);

                    arrDays.Add(defaultdt.ToShortDateString().ToString());
                }

                arrDays.Reverse();
                labels = new string[7];
                int n_count = 0;

                y_arr.Add(0);
                foreach (string x in arrDays)
                {
                    labels[n_count] = x;
                    if (n_count > 0)
                    {
                        DateTime dtNowDateTime = DateTime.ParseExact(x, "yyyy-MM-dd", null);
                        //dtNowDateTime = dtNowDateTime.AddDays(-1);

                        strNowDate = string.Format("{0} {1}:{2}:{3}", dtNowDateTime.ToShortDateString(), 23, 59, 59);
                        //strBeforeDate = string.Format("{0} {1}:{2}:{3}", labels[n_count - 1], 00, 00, 00);
                        strBeforeDate = string.Format("{0}", labels[n_count - 1]);

                        //데이터를 비교하기위해 최소,최대날짜를 DateTime으로 변환
                        DateTime dtMaxDate = DateTime.ParseExact(strNowDate, "yyyy-MM-dd HH:mm:ss", null);
                        DateTime dtMinDate = DateTime.ParseExact(strBeforeDate, "yyyy-MM-dd", null);
                        if (n_count != 6)
                            dtMaxDate = dtMaxDate.AddDays(-1);
                        int nResultCount = 0;
                        int nCount = 0;
                        bool bFirst = true;

                        //Debug.WriteLine(dtMinDate + "  " + dtMaxDate);

                        foreach (ReportHistory history in m_DataReport.ReportDataList)
                        {
                            if (history.Time >= dtMinDate && history.Time < dtMaxDate)
                            {
                                if (bFirst == true)
                                {
                                    //Debug.WriteLine(dtMinDate + "  " + dtMaxDate);
                                    bFirst = false;
                                }
                                nCount++;
                            }

                        }
                        nResultCount = nCount;
                        y_arr.Add(nResultCount);
                    }

                    n_count++;
                }
                //if (isToday == true)
                //labels[6] = labels[6] + " " + DateTime.Now.Hour + "시" + DateTime.Now.Minute + "분";         


                data = new double[7];
                n_count = 0;
                foreach (int y in y_arr)
                {
                    data[n_count] = y;
                    n_count++;
                }
            }
            //strdtMin = string.Format("{0}년 {1}월 {2}일", StartDate.Year, StartDate.Month, StartDate.Day);
            //strdtMax = string.Format("{0}년 {1}월 {2}일", EndDate.Year, EndDate.Month, EndDate.Day);


            setChart();
        }

        public void SetDataGridView(string strMinDate, string strMaxDate, string strAlarmStep)
        {
            gridHistory.Rows.Clear();

            //m_arrTempDataGrid = FormMain.Instance.DataMgr.GetWorkers();
            ArrayList arrReportHistoryList = m_DataReport.ReportDataList;
            int nCount = 0;

            arrReportHistoryList.Sort();
            

            foreach (ReportHistory report in arrReportHistoryList)
            {
                //gridHistory.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridHistory.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridHistory.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                gridHistory.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                string strSpace = "  ";

                nCount++;

                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value =  nCount;
                row.Cells.Add(cell1);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = report.Time;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = strSpace + report.Type;
                row.Cells.Add(cell3);

                if (report.Equipment == null)
                {
                    if (report.Zone == null)
                    {
                        DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                        cell4.Value = strSpace + report.SensorID;
                        row.Cells.Add(cell4);

                        DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                        cell5.Value = report.Car != null ? strSpace + report.Car.Name : strSpace + report.Etc;
                        row.Cells.Add(cell5);
                    }
                    else
                    {
                        DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                        cell4.Value = strSpace + "-";
                        row.Cells.Add(cell4);

                        DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                        cell5.Value = strSpace + report.Zone;
                        row.Cells.Add(cell5);
                    }
                }
                else
                {
                    DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
                    cell4.Value = strSpace + report.SensorID;
                    row.Cells.Add(cell4);

                    DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
                    cell5.Value = strSpace + report.Equipment.Name;
                    row.Cells.Add(cell5);
                }

                DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
                cell6.Value = strSpace + report.WorkerSensorID;
                row.Cells.Add(cell6);

                DataGridViewTextBoxCell cell7 = new DataGridViewTextBoxCell();
                cell7.Value = report.Worker != null ? strSpace + report.Worker.Name : "";
                row.Cells.Add(cell7);

                row.Tag = report;

                gridHistory.Rows.Add(row);
                //m_dicGridData[worker.MemberID] = worker;

            }

            string[] strMin = strMinDate.Split('-');
            string[] strMax = strMaxDate.Split('-');

            lblMinDate.Text = strMin[0] + "년 " + strMin[1] + "월 " + strMin[2] + "일 부터";
            lblMaxDate.Text = strMax[0] + "년 " + strMax[1] + "월 " + strMax[2] + "일 까지";

            lblMaxDate.Location = new Point(lblMinDate.Location.X + lblMinDate.Size.Width + 5, lblMaxDate.Location.Y);

            lblAlarmStep.Text = strAlarmStep;

            //날짜순으로 정렬
            //gridHistory.Sort(gridHistory.Columns[1], ListSortDirection.Descending);
        }

        private void FormReportPage_Load(object sender, EventArgs e)
        {
            DateTime dtEnd = DateTime.Now;
            DateTime dtStart = dtEnd.AddMonths(-6);

            //m_DataReport.SetDetectedHistory(dtStart, dtEnd, "전체 알람");

            SetDataGridView(dtStart.ToShortDateString(), dtEnd.ToShortDateString(), "전체 알람");
            //CreateLineChart(dtStart, dtEnd);

   
        }

        private void FormReportPage_Resize(object sender, EventArgs e)
        {
            setChart();
            SetGridView();
        }
    }
}
