using CrisisAlertManager.Alarm;
using CrisisAlertManager.CrisisAlert;
using CrisisAlertManager.Data;
using CrisisAlertManager.Group;
using CrisisAlertManager.Manual;
using CrisisAlertManager.Popup_Dialog.Alarm;
using CrisisAlertManager.Popup_Dialog.Message;
using CrisisAlertManager.Report;
using DBUtility2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;

namespace CrisisAlertManager
{
    public partial class FormMain : Form
    {
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        public static extern System.IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRect")]
        public static extern System.IntPtr CreateRoundRect(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private Image m_imgTab_CrisisAlert_Normal = global::CrisisAlertManager.Properties.Resources.CrisisAlert_Normal_new;
        private Image m_imgTab_CrisisAlert_Click = global::CrisisAlertManager.Properties.Resources.CrisisAlert_Click_new;
        private Image m_imgTab_CrisisAlert_Hover = global::CrisisAlertManager.Properties.Resources.CrisisAlert_Hover;
        private Image m_imgTab_Group_Normal = global::CrisisAlertManager.Properties.Resources.Group_Normal_new;
        private Image m_imgTab_Group_Click = global::CrisisAlertManager.Properties.Resources.Group_Click_new;
        private Image m_imgTab_Group_Hover = global::CrisisAlertManager.Properties.Resources.Group_Hover;
        private Image m_imgTab_Report_Normal = global::CrisisAlertManager.Properties.Resources.Report_Normal_new;
        private Image m_imgTab_Report_Click = global::CrisisAlertManager.Properties.Resources.Report_Click_new;
        private Image m_imgTab_Report_Hover = global::CrisisAlertManager.Properties.Resources.Report_Hover;
        private Image m_imgTab_Alarm_Normal = global::CrisisAlertManager.Properties.Resources.Alarm_Normal;
        private Image m_imgTab_Alarm_Click = global::CrisisAlertManager.Properties.Resources.Alarm_Click;
        private Image m_imgTab_Manual_Normal = global::CrisisAlertManager.Properties.Resources.Manual_Normal;
        private Image m_imgTab_Manual_Click = global::CrisisAlertManager.Properties.Resources.Manual_Click;
        private Image m_imgTab_Manual_Hover = global::CrisisAlertManager.Properties.Resources.Manual_Hover;


        private uFormCrisisAlert m_uFormCrisisAlert = null;
        private uFormReport m_uFormReport = null;
        private uFormGroup m_uFormGroup = null;
        private uFormManual m_uFormManual = null;
        private uFormAlarmBoard m_uFormAlarmBoard = null;

        private Timer m_timerReload = null;

        private WebDBManager m_dbMgr = null;
        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        private DataManager m_dataMgr = null;
        public DataManager DataManager
        {
            get { return m_dataMgr; }
        }

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private ContentOwnerTab m_CurrentTab = ContentOwnerTab.CRISIS_TAB;
        public ContentOwnerTab CurrentTab
        {
            get { return m_CurrentTab; }
        }

        public void ReloadSensor()
        {
            m_dataMgr.LoadSensors();
        }

        public FormMain()
        {
            InitializeComponent();

            InitPosition();
            m_instance = this;

            string strWebDBServerURL = ConfigurationManager.AppSettings.Get("WebDBServerURL");
            if (strWebDBServerURL == null || strWebDBServerURL.Length == 0)
                strWebDBServerURL = "http://localhost";

            m_dbMgr = new WebDBManager(1);
            m_dbMgr.WebServerURL = strWebDBServerURL;
            m_dbMgr.DatabaseName = "SmartCity";
            m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;

            m_dataMgr = new DataManager(m_dbMgr);

            ShowTime();

            m_timerReload = new Timer();
            m_timerReload.Interval = 5000;
            m_timerReload.Tick += M_timerReload_Tick;
            m_timerReload.Enabled = true;

        }

        private void M_timerReload_Tick(object sender, EventArgs e)
        {
            ShowTime();
        }

        private void ShowTime()
        {
            DateTime dtNow = DateTime.Now;

            string strDate = dtNow.ToString("yyyy년 M월 d일");
            lbDate.Text = strDate;

            string strTime = dtNow.ToString("HH:mm");
            lbTime.Text = strTime;

            string strDay = GetDay(dtNow);
            lbDay.Text = strDay;
        }

        private string GetDay(DateTime dt)
        {
            string strDay = "";

            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    strDay = "월요일";
                    break;
                case DayOfWeek.Tuesday:
                    strDay = "화요일";
                    break;
                case DayOfWeek.Wednesday:
                    strDay = "수요일";
                    break;
                case DayOfWeek.Thursday:
                    strDay = "목요일";
                    break;
                case DayOfWeek.Friday:
                    strDay = "금요일";
                    break;
                case DayOfWeek.Saturday:
                    strDay = "토요일";
                    break;
                case DayOfWeek.Sunday:
                    strDay = "일요일";
                    break;
            }

            return strDay;
        }

        private void InitPosition()
        {
            this.Size = new Size(1920, 1080);
            panelBody.Location = new Point(pnLeft.Location.X + pnLeft.Size.Width, pnLeft.Location.Y);
            panelBody.Size = new Size(pnTop.Size.Width - pnLeft.Size.Width, pnLeft.Size.Height);
        }

        private void btnTab_Click(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            if (btn == null)
                return;

            if (btn == btnTabCrisisAlert)
            {
                ChangeTab(ContentOwnerTab.CRISIS_TAB);
            }
            else if (btn == btnTabGroup)
            {
                ChangeTab(ContentOwnerTab.GROUP_TAB);
            }
            else if (btn == btnTabReport)
            {
                ChangeTab(ContentOwnerTab.REPORT_TAB);
            }
            else if (btn == btnTabAlarm)
            {
                ChangeTab(ContentOwnerTab.ALARM_TAB);
            }
            else if (btn == btnTabManual)
            {
                ChangeTab(ContentOwnerTab.MANUAL_TAB);
            }
        }

        public int ChangeTab(ContentOwnerTab tab)
        {
            if (m_CurrentTab == ContentOwnerTab.GROUP_TAB && m_uFormGroup.IsSave == true)
            {
                FormMessageBox msg = new FormMessageBox("확인", "저장하지 않고 페이지를 이동하게 되면 수정된 데이터는 사라집니다.\n페이지를 이동하시겠습니까?", MessageBoxButtons.YesNo);
                msg.StartPosition = FormStartPosition.CenterParent;

                if (msg.ShowDialog() == DialogResult.No)
                    return (int)m_CurrentTab;
            }

            switch (tab)
            {
                case ContentOwnerTab.CRISIS_TAB:
                    if (m_CurrentTab != ContentOwnerTab.CRISIS_TAB)
                    {
                        SelectCrisisAlertTab();
                    }
                    break;
                case ContentOwnerTab.GROUP_TAB:
                    if (m_CurrentTab != ContentOwnerTab.GROUP_TAB)
                    {
                        m_uFormGroup = new uFormGroup();
                        m_uFormGroup.Parent = panelBody;
                        m_uFormGroup.Dock = DockStyle.Fill;

                        SelectGroupTab();
                    }
                    break;
                case ContentOwnerTab.REPORT_TAB:
                    if (m_CurrentTab != ContentOwnerTab.REPORT_TAB)
                    {
                        SelectReportTab();
                    }
                    break;
                case ContentOwnerTab.ALARM_TAB:
                    if (m_CurrentTab != ContentOwnerTab.ALARM_TAB)
                    {
                        SelectAlarmTab();
                    }
                    break;
                case ContentOwnerTab.MANUAL_TAB:
                    if (m_CurrentTab != ContentOwnerTab.MANUAL_TAB)
                    {
                        SelectManualTab();
                    }
                    break;

            }

            return (int)m_CurrentTab;
        }

        #region Change tab
        private void SelectCrisisAlertTab()
        {
            m_CurrentTab = ContentOwnerTab.CRISIS_TAB;

            btnTabCrisisAlert.Image = m_imgTab_CrisisAlert_Click;
            btnTabCrisisAlert.ImageNormal = m_imgTab_CrisisAlert_Click;
            btnTabCrisisAlert.ImageMouseOver = m_imgTab_CrisisAlert_Click;

            btnTabGroup.Image = m_imgTab_Group_Normal;
            btnTabGroup.ImageNormal = m_imgTab_Group_Normal;
            btnTabGroup.ImageMouseOver = m_imgTab_Group_Hover;

            btnTabReport.Image = m_imgTab_Report_Normal;
            btnTabReport.ImageNormal = m_imgTab_Report_Normal;
            btnTabReport.ImageMouseOver = m_imgTab_Report_Hover;

            btnTabAlarm.Image = m_imgTab_Alarm_Normal;
            btnTabAlarm.ImageNormal = m_imgTab_Alarm_Normal;
            btnTabAlarm.ImageMouseOver = m_imgTab_Alarm_Normal;

            btnTabManual.Image = m_imgTab_Manual_Normal;
            btnTabManual.ImageNormal = m_imgTab_Manual_Normal;
            btnTabManual.ImageMouseOver = m_imgTab_Manual_Hover;

            m_uFormCrisisAlert.Visible = true;
            m_uFormGroup.Visible = false;
            m_uFormReport.Visible = false;
            m_uFormManual.Visible = false;
            m_uFormAlarmBoard.Visible = false;
        }

        private void SelectGroupTab()
        {
            m_CurrentTab = ContentOwnerTab.GROUP_TAB;

            btnTabCrisisAlert.Image = m_imgTab_CrisisAlert_Normal;
            btnTabCrisisAlert.ImageNormal = m_imgTab_CrisisAlert_Normal;
            btnTabCrisisAlert.ImageMouseOver = m_imgTab_CrisisAlert_Hover;

            btnTabGroup.Image = m_imgTab_Group_Click;
            btnTabGroup.ImageNormal = m_imgTab_Group_Click;
            btnTabGroup.ImageMouseOver = m_imgTab_Group_Click;

            btnTabReport.Image = m_imgTab_Report_Normal;
            btnTabReport.ImageNormal = m_imgTab_Report_Normal;
            btnTabReport.ImageMouseOver = m_imgTab_Report_Hover;

            btnTabAlarm.Image = m_imgTab_Alarm_Normal;
            btnTabAlarm.ImageNormal = m_imgTab_Alarm_Normal;
            btnTabAlarm.ImageMouseOver = m_imgTab_Alarm_Normal;

            btnTabManual.Image = m_imgTab_Manual_Normal;
            btnTabManual.ImageNormal = m_imgTab_Manual_Normal;
            btnTabManual.ImageMouseOver = m_imgTab_Manual_Hover;

            m_uFormCrisisAlert.Visible = false;
            m_uFormGroup.Visible = true;
            m_uFormReport.Visible = false;
            m_uFormManual.Visible = false;
            m_uFormAlarmBoard.Visible = false;
        }

        private void SelectReportTab()
        {
            m_CurrentTab = ContentOwnerTab.REPORT_TAB;

            btnTabCrisisAlert.Image = m_imgTab_CrisisAlert_Normal;
            btnTabCrisisAlert.ImageNormal = m_imgTab_CrisisAlert_Normal;
            btnTabCrisisAlert.ImageMouseOver = m_imgTab_CrisisAlert_Hover;

            btnTabGroup.Image = m_imgTab_Group_Normal;
            btnTabGroup.ImageNormal = m_imgTab_Group_Normal;
            btnTabGroup.ImageMouseOver = m_imgTab_Group_Hover;

            btnTabReport.Image = m_imgTab_Report_Click;
            btnTabReport.ImageNormal = m_imgTab_Report_Click;
            btnTabReport.ImageMouseOver = m_imgTab_Report_Click;

            btnTabAlarm.Image = m_imgTab_Alarm_Normal;
            btnTabAlarm.ImageNormal = m_imgTab_Alarm_Normal;
            btnTabAlarm.ImageMouseOver = m_imgTab_Alarm_Normal;

            btnTabManual.Image = m_imgTab_Manual_Normal;
            btnTabManual.ImageNormal = m_imgTab_Manual_Normal;
            btnTabManual.ImageMouseOver = m_imgTab_Manual_Hover;

            if (m_uFormReport != null)
                m_uFormReport.LoadReporDatas();

            m_uFormCrisisAlert.Visible = false;
            m_uFormGroup.Visible = false;
            m_uFormReport.Visible = true;
            m_uFormManual.Visible = false;
            m_uFormAlarmBoard.Visible = false;
        }
        private void SelectAlarmTab()
        {
            m_CurrentTab = ContentOwnerTab.ALARM_TAB;

            btnTabCrisisAlert.Image = m_imgTab_CrisisAlert_Normal;
            btnTabCrisisAlert.ImageNormal = m_imgTab_CrisisAlert_Normal;
            btnTabCrisisAlert.ImageMouseOver = m_imgTab_CrisisAlert_Hover;

            btnTabGroup.Image = m_imgTab_Group_Normal;
            btnTabGroup.ImageNormal = m_imgTab_Group_Normal;
            btnTabGroup.ImageMouseOver = m_imgTab_Group_Hover;

            btnTabReport.Image = m_imgTab_Report_Normal;
            btnTabReport.ImageNormal = m_imgTab_Report_Normal;
            btnTabReport.ImageMouseOver = m_imgTab_Report_Hover;

            btnTabAlarm.Image = m_imgTab_Alarm_Click;
            btnTabAlarm.ImageNormal = m_imgTab_Alarm_Click;
            btnTabAlarm.ImageMouseOver = m_imgTab_Alarm_Click;

            btnTabManual.Image = m_imgTab_Manual_Normal;
            btnTabManual.ImageNormal = m_imgTab_Manual_Normal;
            btnTabManual.ImageMouseOver = m_imgTab_Manual_Hover;

            if (m_uFormAlarmBoard != null)
                m_uFormAlarmBoard.ReloadAlarms();

            m_uFormCrisisAlert.Visible = false;
            m_uFormGroup.Visible = false;
            m_uFormReport.Visible = false;
            m_uFormManual.Visible = false;
            m_uFormAlarmBoard.Visible = true;
        }

        private void SelectManualTab()
        {
            m_CurrentTab = ContentOwnerTab.MANUAL_TAB;

            btnTabCrisisAlert.Image = m_imgTab_CrisisAlert_Normal;
            btnTabCrisisAlert.ImageNormal = m_imgTab_CrisisAlert_Normal;
            btnTabCrisisAlert.ImageMouseOver = m_imgTab_CrisisAlert_Hover;

            btnTabGroup.Image = m_imgTab_Group_Normal;
            btnTabGroup.ImageNormal = m_imgTab_Group_Normal;
            btnTabGroup.ImageMouseOver = m_imgTab_Group_Hover;

            btnTabReport.Image = m_imgTab_Report_Normal;
            btnTabReport.ImageNormal = m_imgTab_Report_Normal;
            btnTabReport.ImageMouseOver = m_imgTab_Report_Hover;

            btnTabAlarm.Image = m_imgTab_Alarm_Normal;
            btnTabAlarm.ImageNormal = m_imgTab_Alarm_Normal;
            btnTabAlarm.ImageMouseOver = m_imgTab_Alarm_Normal;

            btnTabManual.Image = m_imgTab_Manual_Click;
            btnTabManual.ImageNormal = m_imgTab_Manual_Click;
            btnTabManual.ImageMouseOver = m_imgTab_Manual_Click;

            m_uFormCrisisAlert.Visible = false;
            m_uFormGroup.Visible = false;
            m_uFormReport.Visible = false;
            m_uFormManual.Visible = true;
            m_uFormAlarmBoard.Visible = false;
        }
        #endregion

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_uFormCrisisAlert = new uFormCrisisAlert();
            m_uFormCrisisAlert.Parent = panelBody;
            m_uFormCrisisAlert.Dock = DockStyle.Fill;

            m_uFormGroup = new uFormGroup();
            m_uFormGroup.Parent = panelBody;
            m_uFormGroup.Dock = DockStyle.Fill;

            m_uFormReport = new uFormReport();
            m_uFormReport.Parent = panelBody;
            m_uFormReport.Dock = DockStyle.Fill;

            m_uFormManual = new uFormManual();
            m_uFormManual.Parent = panelBody;
            m_uFormManual.Dock = DockStyle.Fill;

            m_uFormAlarmBoard = new uFormAlarmBoard();
            m_uFormAlarmBoard.Parent = panelBody;
            m_uFormAlarmBoard.Dock = DockStyle.Fill;

            SelectCrisisAlertTab();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            panelBody.Size = new Size(pnTop.Size.Width - pnLeft.Size.Width, pnLeft.Size.Height);
        }

        private void FormVisibleChange(Boolean FormVisible)
        {
            if (FormVisible == true && this.Visible == false)
                this.Visible = FormVisible;
            else if (FormVisible == true && this.Visible == true)
            {
                this.WindowState = FormWindowState.Normal;
                this.Show();
                this.Activate();
            }
            else if (FormVisible == false)
                this.Visible = FormVisible;

        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            FormVisibleChange(false);
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            FormVisibleChange(true);
        }

        private void menuClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Application.Exit();
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FormVisibleChange(true);
        }

        public void ShowAlertSensor(FacilityType type, int nSensorID)
        {
            FormVisibleChange(true);                                    // 창 띄우기
            SelectCrisisAlertTab();                                     // 유형별 위기경보 탭 전환
            m_uFormCrisisAlert.ShowAlarmSensor(type, nSensorID);        // 해당 알람 센서 표시
        }

        public void ShowAlarmBoard(FacilityType type)
        {
            FormVisibleChange(true);                                    // 창 띄우기
            SelectAlarmTab();                                           // 알람관리 탭 전환
            m_uFormAlarmBoard.ShowAlarmTab(type);                       // 해당 알람 센서 표시
        }

        public void CheckCloseAlarm(int nID)
        {
            if (m_uFormCrisisAlert == null)
                return;

            m_uFormCrisisAlert.CheckCloseAlarm(nID);
        }
    }
}
