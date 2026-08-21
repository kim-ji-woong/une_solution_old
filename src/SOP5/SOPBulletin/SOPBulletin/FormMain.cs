using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;
using DBUtility;


namespace SOPBulletin
{
    public partial class FormMain : Form
    {
        private DockingProgress m_dockProgress = null;
        private DockingRealTime m_dockRealTime = null;

        private HistoryManager m_historyMgr = null;
        private WebDBManager m_dbMgr = null;
        private SOPManager m_sopMgr = null;

        protected string m_strSkinFolder;

        private bool m_isLockedHistory = false;

        private TimeSpan m_timeSpan;
        private static bool m_checkTimeServer = false;
        //private DateTime m_timeBegin;
        private Thread m_threadTimeServer = null;

        private int m_nTimerMilliSecond = 0;

        private static FormMain m_frmMain = null;
        public static FormMain Instance
        {
            get { return m_frmMain; }
        }

        private bool m_closeApplication = false;
        public bool CloseApplication
        {
            get { return m_closeApplication; }
        }

        private int nMonitor = 4;
        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }


        private string m_szTimeServerAddress = "";
        public string TimeServerAddress
        {
            get { return m_szTimeServerAddress; }
        }

        private double m_addTime = 0.0;
        public double AddTime
        {
            get { return m_addTime; }
            set { m_addTime = value; }
        }

        private string m_szDefSOPName = "";
        public string DefSOPName
        {
            get { return m_szDefSOPName; }
            set { m_szDefSOPName = value; }
        }

        public void LoadTimeServerInfo()
        {
            Utility bulletinConfig = new Utility();
            string strSection = "Server Connection Info";
            m_szTimeServerAddress = bulletinConfig.getinivalue(strSection, "time_server", Application.StartupPath + "\\bullet.ini");
            try
            {
                m_addTime = double.Parse(bulletinConfig.getinivalue(strSection, "add_time", Application.StartupPath + "\\bullet.ini"));
            }
            catch (Exception)
            {
            }
        }

        public FormMain()
        {
            Utility bulletinConfig = new Utility();
            string szSiteID = bulletinConfig.getinivalue("Server Info", "siteid", Application.StartupPath + "\\config.ini");
            if (szSiteID == null || szSiteID == "")
            {
                szSiteID = "1";
            }
            try
            {
                m_nSiteID = int.Parse(szSiteID);
            }
            catch (System.Exception)
            {
            }

            LoadTimeServerInfo();

            m_dbMgr = new WebDBManager(m_nSiteID);
            /*if( m_nSiteID == 2)
                m_dbMgr = new WebDBManager("SOP4");
            else
                m_dbMgr = new WebDBManager();*/


            m_frmMain = this;
            InitializeComponent();

            
            //string szMonNum = m_dbMgr.LoadIni("Bulletin", "Monitor Info");
           
            string szMonNum = bulletinConfig.getinivalue("Monitor Info", "Bulletin", Application.StartupPath + "\\bullet.ini");
            if (szMonNum == null || szMonNum == "")
            {
                szMonNum = "4";
            }
            try
            {
                nMonitor = int.Parse(szMonNum);
            }
            catch (System.Exception)
            {            
            }

            
            
            SetMonitorForm(this, nMonitor);
            //m_timeBegin = DateTime.Now;

            InitHistory();
            m_strSkinFolder = StylesPath();

            CreatePane();

            CallTimeServer();

        }

        private bool SetMonitorForm(Form form, int nDisplay)
        {
            //Screen[] sc = Screen.AllScreens;
            Screen[] sc = Screen.AllScreens.OrderBy(p => p.Bounds.Location.Y).OrderBy(p => p.Bounds.Location.X).ToArray();
            if (form == null)
                return false;


            if (sc.Length == 0)
            {
                return false;
            }

            int nIdx = nDisplay - 1;

            /*string szNum = nDisplay.ToString();
            int nIdx = -1;
            for (int i = 0; i < sc.Length; i++)
            {
                if (sc[i].DeviceName.IndexOf(szNum) != -1)
                {
                    nIdx = i;
                    break;
                }
            }

            if (nIdx == -1)
                nIdx = 0;*/

            if (sc.Length >= nDisplay)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = sc[nIdx].Bounds.Location;
                form.Size = new Size(sc[nIdx].Bounds.Width, sc[nIdx].Bounds.Height);
                form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                form.WindowState = FormWindowState.Maximized;
            }
            return true;
        }


        private void CallTimeServer()
        {
            m_threadTimeServer = new Thread(TimeServerThread);
            m_threadTimeServer.Start();
        }

        private void TimeServerThread()
        {
            //DateTime currentTime = GetNetworkTime();
            DateTime currentTime = DateTime.Now;
            m_checkTimeServer = true;

            currentTime = currentTime.AddHours(FormMain.Instance.AddTime);

            DateTime dtNow = DateTime.Now;
            m_timeSpan = currentTime - dtNow;
        }

        public DateTime GetNetworkTime()
        {
            try
            {
                string ntpServer = FormMain.Instance.TimeServerAddress;

                // NTP message size - 16 bytes of the digest (RFC 2030)
                var ntpData = new byte[48];

                //Setting the Leap Indicator, Version Number and Mode values
                ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

                var addresses = System.Net.Dns.GetHostEntry(ntpServer).AddressList;

                //The UDP port number assigned to NTP is 123
                var ipEndPoint = new System.Net.IPEndPoint(addresses[0], 123);
                //NTP uses UDP
                var socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, System.Net.Sockets.ProtocolType.Udp);

                socket.Connect(ipEndPoint);

                socket.Send(ntpData);
                socket.Receive(ntpData);
                socket.Close();

                //Offset to get to the "Transmit Timestamp" field (time at which the reply 
                //departed the server for the client, in 64-bit timestamp format."
                const byte serverReplyTime = 40;

                //Get the seconds part
                ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

                //Get the seconds fraction
                ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

                //Convert From big-endian to little-endian
                intPart = SwapEndianness(intPart);
                fractPart = SwapEndianness(fractPart);

                var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

                //**UTC** time
                var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

                m_checkTimeServer = true;
                return networkDateTime;
            }
            catch (Exception e)
            {
                m_checkTimeServer = true;
                MessageBox.Show("Time Server와 연결할 수 없습니다-----.\r\n프로그램을 종료합니다.");
                throw e;
            }

            //return new DateTime();
        }

        // stackoverflow.com/a/3294698/162671
        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        private void InitHistory()
        {
            m_sopMgr = new SOPManager(m_dbMgr);
            
            if (!m_sopMgr.Load(true, true))
            {
                MessageBox.Show("DB로부터 SOP Data를 불러올 수 없습니다.\r\n프로그램을 종료합니다.");
                Application.Exit();
                return;
            }

            m_historyMgr = new HistoryManager(m_dbMgr, m_sopMgr);

            if (!m_historyMgr.LoadHistory())
            {
                MessageBox.Show("DB로부터 SOP Log를 불러올 수 없습니다.\r\n프로그램을 종료합니다.");
                Application.Exit();
            }

            timer1.Start();
            timer2.Start();
        }

        public string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");

            return strExePath + "\\Styles\\";
        }

        public void CreatePane()
        {

            m_dockRealTime = new DockingRealTime();
            m_dockRealTime.TopLevel = false;
            m_dockRealTime.Dock = DockStyle.Fill;

            m_dockProgress = new DockingProgress();
            m_dockProgress.TopLevel = false;
            m_dockProgress.Dock = DockStyle.Fill;

            panelStatusBottom.Controls.Add(m_dockRealTime);
            panelProgressBottom.Controls.Add(m_dockProgress);

            m_dockRealTime.Show();
            m_dockProgress.Show();

            m_dockProgress.SetContextMenu(m_dockRealTime.ContextMenu);
            splitContainer1.SplitterDistance = splitContainer1.Panel1MinSize = Screen.FromControl(this).Bounds.Height - 120;
        }

        public SOPBulletin.HistoryManager HistoryManager
        {
            get { return m_historyMgr; }
        }

        public SOPBulletin.SOPManager SOPManager
        {
            get { return m_sopMgr; }
        }

        /*private void CheckTimeServer()
        {
            if (!m_checkTimeServer)
            {
                DateTime dtNow = DateTime.Now;
                TimeSpan span = dtNow - m_timeBegin;

                if (span.Seconds > 30)
                {
                    m_checkTimeServer = true;
                    MessageBox.Show("Time Server와 연결할 수 없습니다.\r\n프로그램을 종료합니다.");
                    throw new Exception();
                }
            }
        }*/

        private DateTime GetMaxMessageTime(ref bool isSuccess)
        {
            //string strSQL = "select max(SendTime) from Message";
            
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT max(msg.SendTime) FROM Message as msg ");
            sb.Append(" INNER JOIN ActionStepHistory as ash ON ash.ID = msg.ActionStepHistoryID ");
            sb.Append(" INNER JOIN ActionStep as step ON step.ID = ash.ActionStepID ");
            sb.Append(" INNER JOIN Disaster as dis ON step.DisasterID = dis.ID ");
            sb.Append(" INNER JOIN SubDisasterCategory as sdc ON dis.SubDisasterID = sdc.ID ");
            sb.AppendFormat(" INNER JOIN DisasterCategory as dc ON dc.ID = sdc.DisasterID AND dc.SiteID = {0}", m_nSiteID);

            string strSQL = sb.ToString();
            
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                isSuccess = false;
                return new DateTime();
            }

            DateTime result;

            try
            {
                result = Convert.ToDateTime(arrResult[0]);
                isSuccess = true;
            }
            catch (Exception)
            {
                result = new DateTime();
                isSuccess = false;
            }

            return result;
        }

        public void FileWrite(DateTime dtLastRead) // 현재까지 읽은 마지막 메시지의 발송시간
        {
            string strPath = Application.StartupPath + "\\BulletReceiveMessage.txt";
            StreamWriter WriteFile = new StreamWriter(strPath, false, Encoding.Unicode);

            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtLastRead.Year, dtLastRead.Month, dtLastRead.Day, dtLastRead.Hour, dtLastRead.Minute, dtLastRead.Second);

            WriteFile.Write(strTime);
            WriteFile.Close();
            WriteFile.Dispose();
        }

        public DateTime FileRead() // 프로그램 종료 전까지 읽은 마지막 메시지 발생 시간 읽어오기
        {
            string strPath = Application.StartupPath + "\\BulletReceiveMessage.txt";

            if (!System.IO.File.Exists(strPath))
            {
                StreamWriter WriteFile = new StreamWriter(strPath, false, Encoding.Unicode);

                bool isSuccess = true;
                DateTime dtMax = GetMaxMessageTime(ref isSuccess);

                if (!isSuccess)
                {
                    WriteFile.Close();
                    return dtMax;
                }

                string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtMax.Year, dtMax.Month, dtMax.Day, dtMax.Hour, dtMax.Minute, dtMax.Second);

                WriteFile.Write(strTime);
                WriteFile.Close();
                return dtMax;
            }

            StreamReader ReadFile = new StreamReader(strPath, System.Text.Encoding.Default);
            string Read_Time = ReadFile.ReadToEnd().ToString();
            ReadFile.Close();
            ReadFile.Dispose();

            DateTime result;

            try
            {
                result = Convert.ToDateTime(Read_Time);
            }
            catch (Exception)
            {
                result = new DateTime();
            }

            return result;
        }

        private void ReadMessage(int nActionStepHistoryID)
        {
            if (m_dockRealTime == null)
                return;

            ActionStepHistoryData currentActionStepData = FormMain.Instance.HistoryManager.CurrentActionStepHistory;
            if (currentActionStepData == null)
                return;

            DateTime dtLastRead = FileRead();
            string strTime = string.Format("'{0}-{1}-{2} {3}:{4}:{5}'", dtLastRead.Year, dtLastRead.Month, dtLastRead.Day, dtLastRead.Hour, dtLastRead.Minute, dtLastRead.Second);

            string strSQL = string.Format("select id, SendTime, Message, MemberID from Message where ActionStepHistoryID = {0} and SendTime > {1}",
                nActionStepHistoryID, strTime);

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            DateTime dtDefault = new DateTime();

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DateTime dtMessage = WebDBManager.GetDateTimeField(arrResult[i + 1], dtDefault);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 2], "");
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                m_dockRealTime.AddMessageRow(dtMessage, strMessage, nMemberID, currentActionStepData);
                FileWrite(dtMessage);
            }
        }


        private bool bStopTimer = false;
        public void StopTimer()
        {
            bStopTimer = true;
        }

        public void ResumeTimer()
        {
            bStopTimer = false;
        }
        
        public void ProgressClear()
        {
            m_dockProgress.ClearProgress();
        }

        private void OnTimer(object sender, EventArgs e)
        {

            if (bStopTimer == true)
                return;

            m_nTimerMilliSecond += timer1.Interval;

            if (m_nTimerMilliSecond >= 3000)
            {
                m_nTimerMilliSecond = 0;

                CheckLockHistory();
                _LockHistory();

                // 현재 실행중이거나 새로운 SOP 정보를 얻어온다.
                if (m_historyMgr.LoadHistory())
                {
                    if (m_historyMgr.CurrentActionStepHistory != null)
                    {
                        ReadMessage(m_historyMgr.CurrentActionStepHistory.ActionStepHistoryID);
                    }

                    // 종료된 SOP가 있는지 확인한다.
                    if (m_historyMgr.CheckFinishActionStep())
                    {
                        if (m_historyMgr.ActionStepHistoryList.Count > 0)
                            m_dockRealTime.UpdateActionSteps(m_historyMgr.ActionStepHistoryList);
                    }
                }

                _UnLockHistory();
            }

            //CheckTimeServer();

            if (m_dockRealTime != null)
            {
                m_dockRealTime.SetControlUserName(m_historyMgr.ControlUserName);
                m_dockProgress.UpdateProcessInfo(m_dockRealTime.CurrentActionStepHistory);

                //if (m_checkTimeServer)
                //{
                //    m_dockRealTime.UpdateProcessedTime(m_timeSpan);
                //}

                if (m_dockRealTime.SelectedSOPIndex >= 0)
                {
                    // SelectedSOPIndex에 해당하는 ActionStepHistoryData의 Index 얻어오기
                    ActionStepHistoryData data = m_dockRealTime.GetActionStepHistoryData(m_dockRealTime.SelectedSOPIndex);

                    if (data != null)
                    {
                        // 얻어온 Index를 사용하여 최신 업데이트된 ActionStepHistoryData 객체 얻어오기
                        data = m_historyMgr.FindActionStepHistory(data.ActionStepHistoryID);

                        if (data != null)
                            m_dockRealTime.UpdateActionStepHistory(data);
                    }
                }
                else
                {
                    if (m_historyMgr.ActionStepHistoryList.Count > 0)
                        m_dockRealTime.UpdateActionStepHistory((ActionStepHistoryData)m_historyMgr.ActionStepHistoryList[0]);
                }
            }
        }

        // 경과시간 표시 Timer
        private void OnProcessedTimer(object sender, EventArgs e)
        {
            if (m_checkTimeServer && m_dockRealTime != null)
            {
                m_dockRealTime.UpdateProcessedTime(m_timeSpan);
            }
        }

        private void CheckLockHistory()
        {
            while (m_isLockedHistory)
            {
                System.Threading.Thread.Sleep(100);
            }
        }

        private void _LockHistory()
        {
            m_isLockedHistory = true;
        }

        private void _UnLockHistory()
        {
            m_isLockedHistory = false;
        }

        public bool LockHistory
        {
            get { return m_isLockedHistory; }
            set { m_isLockedHistory = value; }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // 전체 화면
            this.WindowState = FormWindowState.Maximized;
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

            ToNormalWindow();
            ToFullWindow();
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ToNormalWindow();
            }
            else if (e.KeyCode == Keys.F2)
            {
                ToFullWindow();
            }
        }

        public void ToNormalWindow()
        {
            //if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.Sizable)
            //{
            //    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.WindowState = FormWindowState.Normal;
            //}
        }

        public void ToFullWindow()
        {
            //if (this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
            //{
            //    // 전체 화면
            //    this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;
            //}
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_dockRealTime.WriteGridSize(nMonitor);
        }

        private const int WM_CLOSE = 0x0010;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CLOSE:
                    FormMain.Instance.m_closeApplication = true;
                    break;
            }

            base.WndProc(ref m);
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }



        public void InitData()
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
