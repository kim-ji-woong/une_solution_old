using KpxPipeMonitoring.ChildForms;
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
using DBUtility;
using KpxPipeMonitoring.Report;

namespace KpxPipeMonitoring
{ 
    public partial class MainForm_Tank : Form
    {
        #region Form 이동 변수
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();
        #endregion

        public KpxPipeMonitoring.MainForm.PageKind Main2PageKind { get; set; }

        private static MainForm_Tank m_instance = null;
        public static MainForm_Tank Instance
        {
            get { return m_instance; }
        }
         
        ChildDetailTank detailTank = null;
        ChildDetailWorking detailWorking = null;
        TankReport reportPop = null;

        System.Media.SoundPlayer sp;
         
        public static bool isMainAlarm = false;
        public static bool isMainWork = false; // 작업이 있는지

        private Dictionary<int, ChildTank> m_dicChildTank = null;

        private int m_nViewCount = 25;
        private int m_nCurPage = 1;
        private int m_nMinPage = 1;
        private int m_nMaxPage = 1;

        public MainForm_Tank()
        {
            this.DoubleBuffered = true;
            InitializeComponent();

            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Escape)
                    {
                        UnE.Utility.UMessageBox.FrameColor = Color.FromArgb(1, 4, 20);
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

            //pictureBox_back.Visible = false;
            //pictureBox_report.Visible = false; 
              
            this.Main2PageKind = KpxPipeMonitoring.MainForm.PageKind.TANK;
             
            MainForm.Instance.SetDoubleBuffer(pCenter, true); 

            label_notice.Text = string.Empty;
            
            pictureBox_back.Visible = false;

            MainForm.Instance.commonFunction.SettingButton(pictureBox_report, global::KpxPipeMonitoring.Properties.Resources.Report_Blue, global::KpxPipeMonitoring.Properties.Resources.Report_White);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_back, global::KpxPipeMonitoring.Properties.Resources.Back_Blue, global::KpxPipeMonitoring.Properties.Resources.Back_White);
            MainForm.Instance.commonFunction.SettingButton(pictureBox_setting, global::KpxPipeMonitoring.Properties.Resources.Setting_Normal, global::KpxPipeMonitoring.Properties.Resources.Setting_Click);

            noticeSizeWidth = label_notice.Width * -1;

            sp = new System.Media.SoundPlayer();
            sp.SoundLocation = Application.StartupPath + "\\AlarmSound.WAV"; 

            InitPanel();
            RefreshTank();
                    
            //DisplayAlarm(); 
        }

        private void pictureBox_report_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
             
            pCenter.Visible = false;
            pictureBox_report.Visible = false;
            pictureBox_back.Visible = true;

            reportPop = new TankReport();
            reportPop.Location = new Point(0, panel_top.Size.Height);
            reportPop.Size = new System.Drawing.Size(this.Size.Width, this.Size.Height - panel_top.Size.Height - panel_bottom.Size.Height);
            reportPop.TopLevel = false;
            reportPop.TopLevel = false;
            this.Controls.Add(reportPop);
            reportPop.Show();
        }
         
        private object m_lockObject = new object();
        private void pictureBox_back_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;
            
            lock(m_lockObject)
            {
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

                if (detailTank != null)
                {
                    if (this.Controls.Contains(detailTank))
                    {
                        this.Controls.Remove(detailTank);
                    }

                    if (!detailTank.IsDisposed)
                    {
                        if (detailTank.Visible)
                            detailTank.Close(); 
                        detailTank.Dispose();
                        detailTank = null;
                    }
                    else
                        detailTank = null;
                }

                if (detailWorking != null)
                {
                    if(this.Controls.Contains(detailWorking))
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
        private void InitPanel()
        {
            m_dicChildTank = new Dictionary<int, ChildTank>();

            int chartWidth = 384;
            int chartHeight = 182;
            
            for (int i = 0; i < MainForm.Instance.tankInfo.Count; i++)
            {
                CommonFunction.TankInfo tankInfo = MainForm.Instance.tankInfo[i] as CommonFunction.TankInfo;
                ChildTank tank = new ChildTank((int)tankInfo.nTankID);
                tank.Name = "tank" + tankInfo.nTankID;
               
                tank.Size = new Size(chartWidth, chartHeight);
                tank.TopLevel = false;
                tank.panel1.Cursor = Cursors.Hand;
                tank.nTankID = tankInfo.nTankID; //Tank ID 
                tank.nConnectWorkPipeIDs = tankInfo.nConnectPipeIDs;
                if (tankInfo.strLiquidType == "황산")
                {
                    tank.pictureBox_leakStatus.Visible = true;
                    tank.pictureBox_leakStatus.Location = new Point(90, 12);
                    tank.pictureBox_title.Visible = false;
                    tank.pictureBox_title2.Visible = true;
                    tank.label_pipeName.Visible = false;

                    tank.label_tankName.Parent = tank.pictureBox_title2;
                    tank.label_tankName.Location = new Point(6, 9);
                }
                else if (tankInfo.strLiquidType == "PO")
                {
                    tank.pictureBox_leakStatus.Visible = false;
                    tank.pictureBox_title.Visible = false;
                    tank.pictureBox_title2.Visible = true;
                    tank.label_pipeName.Visible = false;

                    tank.label_tankName.Parent = tank.pictureBox_title2;
                    tank.label_tankName.Location = new Point(6, 9);
                }
                else
                {
                    tank.pictureBox_leakStatus.Visible = false;
                    tank.pictureBox_title.Visible = true;
                    tank.pictureBox_title2.Visible = false;
                    tank.label_pipeName.Visible = true;

                    tank.label_tankName.Parent = tank.pictureBox_title;
                    tank.label_tankName.Location = new Point(7, 9);
                    tank.label_pipeName.Parent = tank.pictureBox_title;
                    tank.label_pipeName.Location = new Point(78, 3);
                }
                tank.panel1.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_liquidType.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_density.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_temp.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_mass.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_curLevel.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_flow.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.pictureBox_tank.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_tankName.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label1.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label2.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label3.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label4.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label5.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label6.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_highLevel.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label8.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label9.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label10.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_capacity.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_highLevel.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };
                tank.label_flowRange.MouseClick += (s, e) => { InTankDetail(tank.nTankID, tank.nConnectWorkPipeIDs, tank.bIsWork, e); };

                //pCenter.Controls.Add(tank);
                //tank.Show();

                m_dicChildTank.Add(i, tank);
            }

            int share = m_dicChildTank.Count / m_nViewCount;
            int rest = m_dicChildTank.Count % m_nViewCount;

            m_nMaxPage = share;
            if (rest > 0)
                m_nMaxPage++;

            SetPage();
        }
        private void SetPage()
        {
            pCenter.Controls.Clear();

            int chartWidth = 384;
            int chartHeight = 182;
            int curX = 0;
            int curY = 0;

            int addCount = 0;
            int beginIndex = (m_nCurPage - 1) * 5; // 가로 개수
            for (int i = beginIndex; i < m_dicChildTank.Count; i++)
            {
                if (addCount > m_nViewCount)
                    return;

                if (curX >= 1920)
                {
                    curX = 0;
                    curY += chartHeight;
                }

                m_dicChildTank[i].Location = new Point(curX, curY);
                
                pCenter.Controls.Add(m_dicChildTank[i]);
                m_dicChildTank[i].Show();

                curX += chartWidth;
            }
        }

        private List<int> ReadTankIDs()
        {
            string strSQL = "Select ID from Tank order by Name";
            ArrayList arrResult = MainForm.Instance.dbMgr.GetResultData(strSQL, 0);

            List<int> ids = new List<int>();

            if (arrResult == null)
                return ids;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                DBUtility.VariousData<int> id = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                ids.Add(id.Data);
            }

            return ids;
        }
          
        private void InTankDetail(int tankId, List<int> connectPipeId, bool isWork, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            pCenter.Visible = false;

            //if (isWork)
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
                 
                detailWorking = new ChildDetailWorking("T", tankId, (connectPipeId == null || connectPipeId.Count == 0) ? -1 : connectPipeId[0]);
                detailWorking.Location = new Point(0, 85);
                detailWorking.Size = new System.Drawing.Size(1920, 913);
                detailWorking.TopLevel = false;
                detailWorking.pictureBox_close.MouseClick += (a, b) =>
                {
                    if (b.Button != System.Windows.Forms.MouseButtons.Left) return;
                    pictureBox_back_MouseClick(a, b);
                };
                this.Controls.Add(detailWorking);
                detailWorking.Show();
            //}
            //else
            //{
            //    if (detailTank != null && !detailTank.IsDisposed && detailTank.IsHandleCreated)
            //    {
            //        detailTank.Close();
            //        detailTank = null;
            //    }
            //    if (detailTank != null && detailTank.IsDisposed)
            //    {
            //        detailTank.Dispose();
            //        detailTank = null;
            //    }
            //    if (detailTank != null)
            //        detailTank = null;

            //    detailTank = new ChildDetailTank(tankId);
            //    detailTank.Location = new Point(191, 174);
            //    detailTank.Size = new System.Drawing.Size(1538, 731);
            //    detailTank.TopLevel = false;
            //    detailTank.pictureBox_close.MouseClick += (a, b) =>
            //    {
            //        if (b.Button != System.Windows.Forms.MouseButtons.Left) return;
            //        pictureBox_back_MouseClick(a, b);
            //    };
            //    this.Controls.Add(detailTank);
            //    detailTank.Show();
            //} 

            pictureBox_report.Visible = false;
            pictureBox_back.Visible = true;
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
         
        public void RefreshTank()
        {
            try
            {
                if (MainForm.Instance.newAlarmInfo.Count > 0) isMainAlarm = true;
                else isMainAlarm = false;

                int isWorkCnt = 0;

                foreach (CommonFunction.TankInfo item in MainForm.Instance.tankInfo)
                {
                    Control[] ctrls = this.pCenter.Controls.Find("tank" + item.nTankID, false);
                    if (ctrls.Length == 0) continue;
                    if (!(ctrls[0] is ChildTank)) continue;

                    ChildTank childTank = ctrls[0] as ChildTank;
                    childTank.nConnectWorkPipeIDs = item.nConnectPipeIDs;
                    if (item.nConnectPipeIDs.Count > 0)
                        isWorkCnt++;
                    // 메인 화면이 아니면 갱신안함
                    if (detailTank == null && detailWorking == null)
                        childTank.InitData(item);

                    if (detailTank != null)
                    {
                        detailTank.SettingStatus();
                    }

                    List<CommonFunction.AllAlarm> alarmList = MainForm.Instance.newAlarmInfo.Where(p => p.nTankID == item.nTankID).ToList();
                    SettingStatus(childTank, alarmList, item.bIsWork, item.strLiquidType, item.bIsLeakStatus);
                }

                if (isWorkCnt > 0)
                    isMainWork = true;
                else
                    isMainWork = false;
            }
            catch (Exception ex)
            {
                MainForm.Instance.SetSystemLog("[ERROR] RefreshTank() / " + ex.Message);
            }
        }

        private DateTime dtLastSound;
        private bool bLastSound = false;

        private void SettingStatus(ChildTank childTank, List<CommonFunction.AllAlarm> newAlarmInfo, bool isWork, string liquidType, bool isH2SO4Alarm)
        {  
            bool isAlarm = false; // 알람이 있는지
            bool isNewAlarm = false; // 새로운 알람이 발생했는지            
            bool isChgAlarm = false; // 알람이 변경됐는지 (해제, 신규)

            if (newAlarmInfo.Count > 0)
                isAlarm = true;

            if (childTank.oldAlarmList.Count < newAlarmInfo.Count)
                isNewAlarm = true;

            if (childTank.oldAlarmList.Count != newAlarmInfo.Count)
                isChgAlarm = true;

            foreach (CommonFunction.AllAlarm newInfo in newAlarmInfo)
            { 
                int cnt = childTank.oldAlarmList.Where(p => p.nAlarmHistoryID == newInfo.nAlarmHistoryID).Count();
                if (cnt == 0)
                {
                    isNewAlarm = true;
                    isChgAlarm = true;
                }
            }
             
            childTank.SetAlarm(isAlarm, isChgAlarm, newAlarmInfo, isWork);

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

                if (detailTank != null || detailWorking != null || reportPop != null)
                {
                    pCenter.Visible = true;
                    pictureBox_report.Visible = true;
                    pictureBox_back.Visible = false; 
                }

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

                if (detailTank != null)
                {
                    if (this.Controls.Contains(detailTank))
                    {
                        this.Controls.Remove(detailTank);
                    }

                    if (!detailTank.IsDisposed)
                    {
                        if (detailTank.Visible)
                            detailTank.Close();
                        detailTank.Dispose();
                        detailTank = null;
                    }
                    else
                        detailTank = null;
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
        }

        /// <summary>
        /// 작업중인 탱크 작업시간 update
        /// </summary>
        public void UpdateBeginTimeWorkTank()
        {
            if (!isMainWork) return;

            foreach (CommonFunction.TankInfo item in MainForm.Instance.tankInfo)
            {
                Control[] ctrls = this.pCenter.Controls.Find("tank" + item.nTankID, false);
                if (ctrls.Length == 0) continue;
                if (!(ctrls[0] is ChildTank)) continue;

                ChildTank childTank = ctrls[0] as ChildTank;
                if (!childTank.bIsWork) continue;

                childTank.SetWorkTime();
            }
        }
        
        private bool CompareAlarmList(List<CommonFunction.AllAlarm> newAlarmList, List<CommonFunction.AllAlarm> oldAlarmList)
        {
            if (newAlarmList.Count != oldAlarmList.Count) return false;

            List<int> allAlarm = new List<int>();

            foreach (CommonFunction.AllAlarm item in oldAlarmList)
            {
                allAlarm.Add(item.nAlarmHistoryID);
            }
            foreach (CommonFunction.AllAlarm  item in newAlarmList)
            {
                if (!allAlarm.Contains(item.nAlarmHistoryID))
                    return false;
                
            }
            return true;
        }

        private void MainForm_Tank_FormClosing(object sender, FormClosingEventArgs e)
        {
            MainForm.Instance.Close();
        }

        Image soundOnImg = global::KpxPipeMonitoring.Properties.Resources.SoundOn;
        Image soundOffImg = global::KpxPipeMonitoring.Properties.Resources.SoundOff;
        
        private void pictureBox_sound_Click(object sender, EventArgs e)
        {
            if (MainForm.Instance.isSound)
            {
                if (isMainAlarm || MainForm_Pipe.isMainAlarm) sp.Stop();
                pictureBox_sound.Image = soundOffImg;
                MainForm.Instance.isSound = false;

                MainForm.Instance.SetSystemLog("tank page sound / false ");
            }
            else
            {
                if (isMainAlarm || MainForm_Pipe.isMainAlarm)
                {
                    dtLastSound = MainForm.Instance.SystemNow;
                    sp.Play();
                }
                pictureBox_sound.Image = soundOnImg;
                MainForm.Instance.isSound = true;

                MainForm.Instance.SetSystemLog("tank page sound / true ");
            }
        }
        public void SetSound()
        {
            if (MainForm.Instance.isSound)
            {
                if (!isMainAlarm && !MainForm_Pipe.isMainAlarm) sp.Stop(); 
                pictureBox_sound.Image = soundOnImg; 
            }
            else
            {
                if (isMainAlarm || MainForm_Pipe.isMainAlarm) sp.Stop(); 
                pictureBox_sound.Image = soundOffImg;                 
            }
        }

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
                MainForm_Pipe.SetWindowPosition(this);
            }
        }
        #endregion        
    } 
}
