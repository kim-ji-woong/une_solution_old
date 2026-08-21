using DBUtility;
using KpxPipeMonitoring.ChildForms;
using KpxPipeMonitoring.Popups;
using KpxPipeMonitoring.Report;
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

namespace KpxPipeMonitoring
{
    public partial class MainForm_Pipe : Form
    {
        #region Form 이동 변수
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();
        #endregion

        public KpxPipeMonitoring.MainForm.PageKind Main1PageKind { get; set; }
         
        private static MainForm_Pipe m_instance = null;

        public static MainForm_Pipe Instance
        {
            get { return m_instance; }
        }

        public static bool isMainAlarm = false; // 알람이 있는지 
        public static bool isMainWork = false; // 작업이 있는지
        PipeReport reportPop = null;
        ChildDetailPipe detailPipePop = null;
        ChildDetailWorking detailWorking = null; 
        System.Media.SoundPlayer sp;

        private IHistoryManager m_historyMgr = null;
        
        public MainForm_Pipe()
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            MainForm.Instance.SetDoubleBuffer(pCenter, true);

            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Escape)
                    {
                        if (UnE.Utility.UMessageBox.Show("통합모니터링 시스템을 종료하시겠습니까?", "종료", MessageBoxButtons.YesNo)
                            == System.Windows.Forms.DialogResult.No)
                            return;

                        MainForm.Instance.Close(); 
                    }
                    else if (e.KeyCode == Keys.F1)
                    {
                        System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo();
                        info.FileName = Application.StartupPath + @"\Help\HelpViewer.exe";
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo = info;

                        process.Start(); 
                    }

                }; 
            this.Main1PageKind = KpxPipeMonitoring.MainForm.PageKind.PIPE; 
            m_instance = this;

            m_historyMgr = new HistoryManager(MainForm.Instance);
            
            label_notice.Text = string.Empty;

            noticeSizeWidth = label_notice.Width * -1;
              
            pictureBox_back.Visible = false;
            pictureBox_buttonStatus.Visible = false;

            MainForm.Instance.commonFunction.SettingButton(pictureBox_report, global::KpxPipeMonitoring.Properties.Resources.Report_Blue, global::KpxPipeMonitoring.Properties.Resources.Report_White);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_back, global::KpxPipeMonitoring.Properties.Resources.Back_Blue, global::KpxPipeMonitoring.Properties.Resources.Back_White);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_setting, global::KpxPipeMonitoring.Properties.Resources.Setting_Normal, global::KpxPipeMonitoring.Properties.Resources.Setting_Click);
            MainForm.Instance.commonFunction.SettingButton(btnPageRight, global::KpxPipeMonitoring.Properties.Resources.btnViewmode1, global::KpxPipeMonitoring.Properties.Resources.btnViewmode2);

            sp = new System.Media.SoundPlayer();
            sp.SoundLocation = Application.StartupPath + "\\AlarmSound.WAV";

            InitPanel();
            RefreshPipe();
            RefreshChart();
        } 
         
        #region Time 함수
        public void SetTime(string text)
        {
            label_date.Text = text;
        }
        #endregion

        #region 공지사항 함수
        int noticeLocationX = 0;
        int noticeSizeWidth = 0;

        public void DisplayNotice(string notice)
        { 
            label_notice.Text = notice;
            if (label_notice.Width > 1550)
            {
                if (noticeLocationX >= noticeSizeWidth)
                {
                    label_notice.Location = new Point(noticeLocationX, 7);
                    noticeLocationX -= 30;
                }
                else
                {
                    
                    label_notice.Text = notice;
                    noticeSizeWidth = label_notice.Width * -1;
                    noticeLocationX = 0;
                }
            }
            else
                label_notice.Location = new Point(3, 7);
        }
        #endregion

        #region 패널 세팅
        public delegate void AlarmClearEventArgs();
        public event AlarmClearEventArgs alarmClearEventArgs;
        private void InitPanel()
        {
            pCenter.Controls.Clear();

            int chartWidth = 640;
            int chartHeight = 229;
            int curX = 0;
            int curY = 0;

            if (bViewWork)
            {
                chartWidth = 960;
                chartHeight = 458;
                curX = 0;
                curY = 0;
            }

            int viewCount = 1;

            foreach (CommonFunction.PipeInfo info in MainForm.Instance.pipeInfo)
            {
                if (bViewWork && info.nConnectTankID < 0)
                    continue;

                if (bViewWork && viewCount == 5) // 작업중 화면은 최대 4개
                    break;

                ChildPipe pipe = new ChildPipe(MainForm.Instance.dbMgr, info.nPipeID);
                pipe.Name = "chart" + info.nPipeID;
                if (curX >= 1920)
                {
                    curX = 0;
                    curY += chartHeight;
                }
                pipe.Location = new Point(curX, curY);
                pipe.Size = new Size(chartWidth, chartHeight);
                pipe.TopLevel = false;
                pipe.panel1.BackColor = Color.Transparent;
                pipe.panel1.Parent = pCenter;
                pipe.panel1.Location = new Point(curX + 5, curY + 5);
                if (bViewWork)
                {
                    pipe.bViewWork = true;
                    pipe.SetViewWorkModeLocation();
                    pipe.lblPressure.Visible = pipe.lblFlow.Visible = true;
                }                
                
                pipe.Tag = info.nPipeID;

                pipe.label_pipeName.Text = info.strPipeName;

                pipe.alarmClearEventArgs += () =>
                    {
                        if (alarmClearEventArgs != null)
                            alarmClearEventArgs();
                    //DisplayAlarm();
                };
                pipe.chart_pressure.MouseClick += (s, e) =>
                    {
                        if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

                        this.Cursor = Cursors.WaitCursor;
                        pCenter.Visible = false;

                    //if (pipe.isWork)
                    //{
                    if (detailWorking != null && !detailWorking.IsDisposed && detailWorking.IsHandleCreated)
                        {
                            detailWorking.Close();
                            detailWorking = null;
                        }
                        if (detailWorking != null && detailWorking.IsDisposed)
                        {
                            detailWorking.Dispose();
                            detailWorking = null;
                        }
                        if (detailWorking != null)
                            detailWorking = null;

                        detailWorking = new ChildDetailWorking("P", pipe.nConnectWorkTankID, pipe.nPipeID);
                        detailWorking.Location = new Point(0, 85);
                        detailWorking.Size = new System.Drawing.Size(1920, 913);
                        detailWorking.TopLevel = false;
                        detailWorking.label_tankName.Location = new Point(150, 8);
                        detailWorking.label_pipeName.Location = new Point(7, 8);
                        detailWorking.pictureBox_close.MouseClick += (a, b) =>
                            {
                                if (b.Button != System.Windows.Forms.MouseButtons.Left)
                                    return;
                                pictureBox_back_MouseClick(a, b);
                            };
                        this.Controls.Add(detailWorking);
                        detailWorking.Show();
                    //}
                    //else
                    //{
                    //    if (detailPipePop != null && !detailPipePop.IsDisposed && detailPipePop.IsHandleCreated)
                    //    {
                    //        detailPipePop.Close();
                    //        detailPipePop = null;
                    //    }
                    //    if (detailPipePop != null && detailPipePop.IsDisposed)
                    //    {
                    //        detailPipePop.Dispose();
                    //        detailPipePop = null;
                    //    }
                    //    if (detailPipePop != null)
                    //        detailPipePop = null;

                    //    detailPipePop = new ChildDetailPipe((int)pipe.Tag);
                    //    detailPipePop.Location = new Point(163, 238);
                    //    detailPipePop.Size = new System.Drawing.Size(1594, 603);
                    //    detailPipePop.TopLevel = false;

                    //    detailPipePop.pictureBox_close.MouseClick += (a, b) =>
                    //        {
                    //            pictureBox_back_MouseClick(a, b);
                    //        };
                    //    this.Controls.Add(detailPipePop); 
                    //    detailPipePop.Show();
                    //}

                        pictureBox_report.Visible = false;
                        pictureBox_back.Visible = true;

                        this.Cursor = Cursors.Default;
                    };
                pipe.chart_flow.MouseClick += (s, e) =>
                {
                    if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

                    this.Cursor = Cursors.WaitCursor;
                    pCenter.Visible = false;

                //if (pipe.isWork)
                //{
                if (detailWorking != null && !detailWorking.IsDisposed && detailWorking.IsHandleCreated)
                    {
                        detailWorking.Close();
                        detailWorking = null;
                    }
                    if (detailWorking != null && detailWorking.IsDisposed)
                    {
                        detailWorking.Dispose();
                        detailWorking = null;
                    }
                    if (detailWorking != null)
                        detailWorking = null;

                    detailWorking = new ChildDetailWorking("P", pipe.nConnectWorkTankID, pipe.nPipeID);
                    detailWorking.Location = new Point(0, 85);
                    detailWorking.Size = new System.Drawing.Size(1920, 913);
                    detailWorking.TopLevel = false;
                    detailWorking.label_tankName.Location = new Point(150, 8);
                    detailWorking.label_pipeName.Location = new Point(7, 8);
                    detailWorking.pictureBox_close.MouseClick += (a, b) =>
                    {
                        pictureBox_back_MouseClick(a, b);
                    };
                    this.Controls.Add(detailWorking);
                    detailWorking.Show();
                //}
                //else
                //{
                //    if (detailPipePop != null && !detailPipePop.IsDisposed && detailPipePop.IsHandleCreated)
                //    {
                //        detailPipePop.Close();
                //        detailPipePop = null;
                //    }
                //    if (detailPipePop != null && detailPipePop.IsDisposed)
                //    {
                //        detailPipePop.Dispose();
                //        detailPipePop = null;
                //    }
                //    if (detailPipePop != null)
                //        detailPipePop = null;

                //    detailPipePop = new ChildDetailPipe((int)pipe.Tag);
                //    detailPipePop.Location = new Point(163, 238);
                //    detailPipePop.Size = new System.Drawing.Size(1594, 603);
                //    detailPipePop.TopLevel = false;
                //    detailPipePop.pictureBox_close.MouseClick += (a, b) =>
                //    {
                //        if (b.Button != System.Windows.Forms.MouseButtons.Left) return;
                //        pictureBox_back_MouseClick(a, b);
                //    };
                //    this.Controls.Add(detailPipePop);
                //    detailPipePop.Show();
                //}

                pictureBox_report.Visible = false;
                    pictureBox_back.Visible = true;

                    this.Cursor = Cursors.Default;
                };
                pCenter.Controls.Add(pipe);
                pipe.Show();

                curX += chartWidth;
                viewCount++;
            } 
            
            if (viewCount == 12)
            {
                Empty empty = new Empty();
                empty.Location = new Point(curX, curY);
                empty.TopLevel = false;
                empty.Parent = pCenter;                
                empty.Show();
            }
        }
        #endregion

        #region 패널 데이터 바인딩
        private bool fileNoWriteChk = false;
        private int fileNoWriteChkCnt = 0;
        public void RefreshChart()
        {
            if (detailPipePop != null || reportPop != null || detailWorking != null) return;

            List<CommonFunction.ChartField> chartList = DisplayChart();
            if (chartList.Count == 0)
            {
                if (!fileNoWriteChk && fileNoWriteChkCnt >= 1)
                {
                    fileNoWriteChk = true;
                    UnE.Utility.UMessageBox.Show(this, "그래프에 표현할 데이터를 작성중입니다.\r\n최초 1회만 약30분 소요됩니다.\r\n현재 시간 : " + DateTime.Now, "", MessageBoxButtons.OK);
                }

                fileNoWriteChkCnt++;
            }
            else if (chartList.Count > 0 && fileNoWriteChkCnt > 0)
                fileNoWriteChkCnt = 0;
             
            foreach (CommonFunction.PipeInfo info in MainForm.Instance.pipeInfo)
            {
                foreach (Control item in this.pCenter.Controls)
                {
                    if (item.Name == "chart" + info.nPipeID)
                    {
                        if (item is ChildPipe)
                        {
                            ChildPipe childPipe = item as ChildPipe; 
                            childPipe.InitChartData(chartList);
                            break;
                        }
                    }
                }
            }
        }
          
        private List<CommonFunction.ChartField> DisplayChart()
        {
            List<CommonFunction.ChartField> chartList = new List<CommonFunction.ChartField>();
            try
            {
                List<CommonFunction.ChartField> totalChartData = new List<CommonFunction.ChartField>();

                #region 1. DB로 읽기
                //StringBuilder sb = new StringBuilder();
                //sb.Append("SELECT ph.ID, PipeID, TimeStamp, ph.Pressure ");
                //sb.Append("  FROM PipeHistory ph ");
                //sb.Append(" WHERE TimeStamp >= date_add(now(), interval - 30 minute) ");
                //sb.Append("   AND Pressure > 0.2 AND ID > " + searchMaxID);

                //arrResult = MainForm.Instance.dbMgr.GetResultData(sb.ToString(), 0);
                //if (arrResult == null) return null;

                //for (int i = 0; i < arrResult.Count; i += 4)
                //{
                //    int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                //    if (nID > searchMaxID)
                //    {
                //        searchMaxID = nID;
                //        int nPipeID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                //        string strPipeName = "";
                //        DateTime date = DBUtility.WebDBManager.GetDateTimeField(arrResult[i + 2], new DateTime());
                //        double pressure = (arrResult[i + 3].ToString() == "null") ? -1 : Convert.ToDouble(arrResult[i + 3]);

                //        totalChartData.Add(new CommonFunction.PipeChartField(nPipeID, strPipeName, date, date.ToString("HH:mm"), pressure));
                //    }
                //} 
                #endregion

                DateTime dtNow = MainForm.Instance.commonFunction.GetDateTimeNow(); 
                #region 2. 파일로 읽기 
                DateTime beforeDate = dtNow.AddMinutes(-30);
                DateTime afterDate = dtNow;
                 
                List<HistoryQuery> historyQueries = new List<HistoryQuery>(); 

                int totalDays = (int)(afterDate - beforeDate).TotalDays;
                if (afterDate.Hour == 0 && afterDate.Minute < 30)
                    totalDays++;

                for (int i = 0; i <= totalDays; i++)
                {
                    foreach (CommonFunction.PipeInfo info in MainForm.Instance.pipeInfo)
                    {
                        string y = beforeDate.AddDays(i).Year.ToString();
                        string m = beforeDate.AddDays(i).Month.ToString();
                        string d = beforeDate.AddDays(i).Day.ToString();

                        HistoryQuery query = new HistoryQuery(info.nPipeID, y, m, d, HistoryQueryType.작업중);
                        historyQueries.Add(query); 
                    }
                } 
                totalChartData = m_historyMgr.ReadHistory(historyQueries); 
                historyQueries.Clear();
                historyQueries = null;
                 
                if (totalChartData != null && totalChartData.Count > 0)
                    beforeDate = totalChartData[totalChartData.Count - 1].dtTimeStamp;
                else
                    beforeDate = dtNow;
                #endregion

                
                DateTime tempDateTime = dtNow.AddMinutes(-30);

                foreach (CommonFunction.ChartField item in totalChartData)
                {
                    if (item.dtTimeStamp >= tempDateTime)
                        chartList.Add(item);
                }

                return chartList;
            }
            catch (Exception ex)
            {
                MainForm.Instance.SetSystemLog("[ERROR] DisplayChart() / " + ex.Message);
                MainForm.Instance.SetSystemLog("[ERROR] DisplayChart() / " + ex.InnerException);
                MainForm.Instance.SetSystemLog("[ERROR] DisplayChart() / " + ex.StackTrace);
                return chartList;
            }
        }
         
        public void RefreshPipe()
        {
            try
            {
                if (MainForm.Instance.newAlarmInfo.Count > 0) isMainAlarm = true;
                else isMainAlarm = false;

                int isWorkCnt = 0;

                foreach (CommonFunction.PipeInfo item in MainForm.Instance.pipeInfo)
                {
                    Control[] ctrls = this.pCenter.Controls.Find("chart" + item.nPipeID, false);
                    if (ctrls.Length == 0)
                    {
                        if (bViewWork && item.nConnectTankID > 0)
                        {
                            RefreshPage();
                            return;
                        }
                        continue;
                    }
                    if (!(ctrls[0] is ChildPipe)) continue;

                    ChildPipe childPipe = ctrls[0] as ChildPipe;
                    childPipe.nStandardPressure = item.nStandardPressure;
                    childPipe.nStandardFlow = item.nStandardFlow;

                    bool isWork = false;
                    if (item.nConnectTankID > 0)
                    {
                        isWork = true;
                        isWorkCnt++;
                    }
                    else
                    {
                        if (bViewWork && item.nConnectTankID < 0)
                        {
                            RefreshPage();
                            return;
                        }
                    }
                    childPipe.nConnectWorkTankID = item.nConnectTankID;

                    List<CommonFunction.AllAlarm> alarmList = MainForm.Instance.newAlarmInfo.Where(p => p.nPipeID == item.nPipeID && p.nTankID == childPipe.nConnectWorkTankID).ToList();
                    SettingStatus(childPipe, alarmList, isWork);
                }

                if (isWorkCnt > 0)
                {
                    isMainWork = true;
                    btnPageRight.Enabled = true;
                }
                else
                {                    
                    isMainWork = false;
                    btnPageRight.Enabled = false;

                    if (bViewWork)
                    {
                        bViewWork = false;

                        InitPanel();
                        RefreshPipe();
                        RefreshChart();
                    }
                }
            }
            catch (Exception ex)
            {
                MainForm.Instance.SetSystemLog("[ERROR] RefreshPipe() / " + ex.Message); 
            }
        }

        private DateTime dtLastSound;
        private bool bLastSound = false;

        private void SettingStatus(ChildPipe childPipe, List<CommonFunction.AllAlarm> newAlarmInfo, bool isWork)
        { 
            bool isAlarm = false; // 알람이 있는지
            bool isNewAlarm = false; // 새로운 알람이 발생했는지            
            bool isChgAlarm = false; // 알람이 변경됐는지 (해제, 신규)

            if (newAlarmInfo.Count > 0)
                isAlarm = true;

            if (childPipe.oldAlarmList.Count < newAlarmInfo.Count)
                isNewAlarm = true;

            if (childPipe.oldAlarmList.Count != newAlarmInfo.Count)
                isChgAlarm = true;

            foreach (CommonFunction.AllAlarm newInfo in newAlarmInfo)
            {
                int cnt = childPipe.oldAlarmList.Where(p => p.nAlarmHistoryID == newInfo.nAlarmHistoryID).Count();
                if (cnt == 0)
                {
                    isNewAlarm = true;
                    isChgAlarm = true;
                }
            }
             
            childPipe.Setting(isAlarm, isChgAlarm, newAlarmInfo, isWork);

            if (isAlarm && MainForm.Instance.isSound)
            {
                if (!bLastSound || dtLastSound.AddSeconds(30) <= MainForm.Instance.SystemNow)
                {
                    dtLastSound = MainForm.Instance.SystemNow;
                    bLastSound = true;
                    sp.Play();
                }
            }
            if (isNewAlarm)
            {
                MainForm.Instance.isSound = true;
                isAlarm = true;

                if (!bLastSound || dtLastSound.AddSeconds(30) <= MainForm.Instance.SystemNow)
                {
                    dtLastSound = MainForm.Instance.SystemNow;
                    bLastSound = true;
                    sp.Play();
                }

                if (detailPipePop != null || detailWorking != null || reportPop != null)
                {
                    pCenter.Visible = true;
                    pictureBox_report.Visible = true;
                    pictureBox_back.Visible = false;

                    if (reportPop != null)
                    {
                        reportPop.Close();
                        reportPop = null;
                    }
                    if (detailPipePop != null)
                    {
                        detailPipePop.Close();
                        detailPipePop = null;
                    }
                    if (detailWorking != null)
                    {
                        detailWorking.Close();
                        detailWorking = null;
                    }
                }
            }
        }
        /// <summary>
        /// 작업중인 배관 작업시간 update
        /// </summary>
        public void UpdateBeginTimeWorkPipe()
        {
            if (!isMainWork) return;

            foreach (CommonFunction.PipeInfo item in MainForm.Instance.pipeInfo)
            {
                Control[] ctrls = this.pCenter.Controls.Find("chart" + item.nPipeID, false);
                if (ctrls.Length == 0) continue;
                if (!(ctrls[0] is ChildPipe)) continue;

                ChildPipe childPipe = ctrls[0] as ChildPipe;
                if (!childPipe.isWork) continue;

                childPipe.SetWorkTime();
            }
        }
        #endregion

        #region 환경설정 버튼 이벤트
        private void pictureBox_setting_Click(object sender, EventArgs e)
        {
            EnvironmentPop2 pop = new EnvironmentPop2();
            pop.StartPosition = FormStartPosition.CenterParent;
            if (pop.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //MessageBox.Show("저장되었습니다.");
            }
        }
        #endregion
           
        private void MainForm_Pipe_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.Instance.Close();
        } 

        #region 사운드 
        Image soundOnImg = global::KpxPipeMonitoring.Properties.Resources.SoundOn;
        Image soundOffImg = global::KpxPipeMonitoring.Properties.Resources.SoundOff;

        private void pictureBox_sound_Click(object sender, EventArgs e)
        {
            if (MainForm.Instance.isSound)
            {
                if (isMainAlarm || MainForm_Tank.isMainAlarm) sp.Stop();
                pictureBox_sound.Image = soundOffImg;
                MainForm.Instance.isSound = false;

                MainForm.Instance.SetSystemLog("Pipe page sound / false ");
            }
            else
            {
                if (isMainAlarm || MainForm_Tank.isMainAlarm)
                {
                    dtLastSound = MainForm.Instance.SystemNow;
                    sp.Play();
                }
                pictureBox_sound.Image = soundOnImg;
                MainForm.Instance.isSound = true;

                MainForm.Instance.SetSystemLog("Pipe page sound / true ");
            }
        }
        public void SetSound()
        {
            if (MainForm.Instance.isSound)
            {
                if (!isMainAlarm && !MainForm_Tank.isMainAlarm) sp.Stop(); 
                pictureBox_sound.Image = soundOnImg;
            }
            else
            {
                if (isMainAlarm || MainForm_Tank.isMainAlarm) sp.Stop();
                pictureBox_sound.Image = soundOffImg;
            }
        }
        #endregion

        #region 보고서 버튼 이벤트 
        Image mainLegend = global::KpxPipeMonitoring.Properties.Resources.PipeLegend;
        Image reportLegend = global::KpxPipeMonitoring.Properties.Resources.ReportPipeLegend;
        private void pictureBox_back_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            pictureBox_legend.Image = mainLegend;
            pictureBox_legend.Size = new System.Drawing.Size(531, 37);
            pCenter.Visible = true;
            pictureBox_report.Visible = true;
            pictureBox_back.Visible = false;

            if (reportPop != null)
            {
                if (this.Controls.Contains(reportPop))
                {
                    this.Controls.Remove(reportPop);
                }

                if (!reportPop.IsDisposed)
                {
                    if (reportPop.Visible)
                        reportPop.Close(); 
                        reportPop.Dispose();
                    reportPop = null;
                }
                else
                    reportPop = null;
            }

            if (detailPipePop != null)
            {
                if (this.Controls.Contains(detailPipePop))
                {
                    this.Controls.Remove(detailPipePop);
                }

                if (!detailPipePop.IsDisposed)
                {
                    if (detailPipePop.Visible)
                        detailPipePop.Close(); 
                    detailPipePop.Dispose();
                    detailPipePop = null;
                }
                else
                    detailPipePop = null;
            } 

            if (detailWorking != null)
            {
                if (this.Controls.Contains(detailWorking))
                {
                    this.Controls.Remove(detailWorking);
                }

                if (!detailWorking.IsDisposed)
                {
                    if (detailWorking.Visible)
                        detailWorking.Close(); 
                    detailWorking.Dispose();
                    detailWorking = null;
                }
                else
                    detailWorking = null;
            } 
        }

        private void pictureBox_report_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            pictureBox_legend.Image = reportLegend;
            pictureBox_legend.Size = new System.Drawing.Size(434, 37);
            pCenter.Visible = false;
            pictureBox_report.Visible = false;
            pictureBox_back.Visible = true;

            reportPop = new PipeReport();
            reportPop.Location = new Point(0, panel_top.Size.Height);
            reportPop.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height - panel_top.Size.Height - panel_bottom.Size.Height);
            reportPop.TopLevel = false;
            reportPop.TopLevel = false;
            this.Controls.Add(reportPop);
            reportPop.Show();
        }
        #endregion  

        #region Form 이동 구현
        private void panel_top_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void panel_top_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void panel_top_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }

        private void panel_top_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                SetWindowPosition(this);
            }
        }

        public static bool SetWindowPosition(Form frm)
        {
            if (SetWindowPosition(frm, frm.Location))
                return true;

            Point ptBL = new Point(frm.Location.X, frm.Location.Y + frm.Size.Height);
            Point ptTR = new Point(frm.Location.X + frm.Size.Width, frm.Location.Y);
            Point ptBR = new Point(frm.Location.X + frm.Size.Width, frm.Location.Y + frm.Size.Height);
            Point ptMiddle = new Point((frm.Location.X + ptBR.X) / 2, (frm.Location.Y + ptBR.Y) / 2);

            if (SetWindowPosition(frm, ptBL))
                return true;
            if (SetWindowPosition(frm, ptTR))
                return true;
            if (SetWindowPosition(frm, ptBR))
                return true;
            if (SetWindowPosition(frm, ptMiddle))
                return true;

            return false;
        }

        private static bool SetWindowPosition(Form frm, Point pt)
        {
            foreach (Screen sc in Screen.AllScreens)
            {
                if (pt.X >= sc.Bounds.Left && pt.X <= sc.Bounds.Right &&
                    pt.Y >= sc.Bounds.Top && pt.Y <= sc.Bounds.Bottom)
                {
                    frm.Location = new Point(sc.Bounds.Left, sc.Bounds.Top);
                    return true;
                }
            }

            return false;
        }

        #endregion

        private bool bViewWork = false;
        private void btnPage_Click(object sender, EventArgs e)
        {
            if (!isMainWork)
                return;
            
            bViewWork = !bViewWork;

            RefreshPage();

            if (bViewWork)
                MainForm.Instance.commonFunction.SettingButton(btnPageRight, global::KpxPipeMonitoring.Properties.Resources.btnViewmode1, global::KpxPipeMonitoring.Properties.Resources.btnViewmode2);
            else
                MainForm.Instance.commonFunction.SettingButton(btnPageRight, global::KpxPipeMonitoring.Properties.Resources.btnViewmode2, global::KpxPipeMonitoring.Properties.Resources.btnViewmode1);
        }

        public void RefreshPage()
        {
            InitPanel();
            RefreshPipe();
            RefreshChart();
        }
    } 
}
