using DBUtility2;
using ServerMonitoring.Network;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ServerMonitoring
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;
        private WebDBManager m_dbMgrIntegration = null;

        private NetworkWebManager m_netMgr = null;
        private NetworkWebManager m_netMgrIntegration = null;

        private System.Windows.Forms.Timer m_timer = null;

        private string m_strSystemPath = "";
        private string m_strIntegrationSystemPath = "";
        private string m_strProcessName = "";
        private string m_strWebServerURL = "";
        private string m_strIntegrationWebServerURL = "";
        
        public FormMain()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            VisibleThisForm(false);
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            this.TopLevel = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_strSystemPath = ServerMonitoring.Properties.Settings.Default.SystemPath;
            m_strIntegrationSystemPath = ServerMonitoring.Properties.Settings.Default.IntegrationSystemPath;
            m_strProcessName = ServerMonitoring.Properties.Settings.Default.ProcessName;
            m_strIntegrationWebServerURL = ServerMonitoring.Properties.Settings.Default.IntegrationWebServerURL;
            m_strWebServerURL = ServerMonitoring.Properties.Settings.Default.WebServerURL;

            if (m_strSystemPath.Length == 0 || m_strIntegrationSystemPath.Length == 0 || m_strProcessName.Length == 0)
            {
                MessageBox.Show("설정값을 읽을 수 없음");
                this.Close();
            }

            m_dbMgr = new WebDBManager(205);
            m_dbMgr.DatabaseName = "BLD_205";
            m_dbMgr.DatabaseType = WebDBManager.DBType.sqlserver;
            m_dbMgr.WebServerURL = m_strWebServerURL;

            m_dbMgrIntegration = new WebDBManager(205);
            m_dbMgrIntegration.DatabaseName = "BLD_205";
            m_dbMgrIntegration.DatabaseType = WebDBManager.DBType.sqlserver;
            m_dbMgrIntegration.WebServerURL = m_strIntegrationWebServerURL;

            m_netMgr = new NetworkWebManager(m_dbMgr);
            m_netMgrIntegration = new NetworkWebManager(m_dbMgrIntegration);

            m_timer = new System.Windows.Forms.Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += M_timer_Tick;
            m_timer.Start();
        }

        private bool m_bOrgConnected = true;
        private bool m_bOrgConnectedIntegration = true;
        private bool m_bFrist = true;
        private void M_timer_Tick(object sender, EventArgs e)
        {
            if (m_netMgr.IsConnected)
            {
                lblServerStatus.Text = "연결 가능";
                lblServerStatus.ForeColor = Color.Green;
            }
            else
            {
                lblServerStatus.Text = "연결 불가능";
                lblServerStatus.ForeColor = Color.Red;
            }

            if (m_netMgrIntegration.IsConnected)
            {
                lblServerStatusIntegration.Text = "연결 가능";
                lblServerStatusIntegration.ForeColor = Color.Green;
            }
            else
            {
                lblServerStatusIntegration.Text = "연결 불가능";
                lblServerStatusIntegration.ForeColor = Color.Red;
            }

            bool isRunning = IsRunning(m_strSystemPath); // 전용 Client가 실행중인가?
            bool isRunningIntegration = IsRunning(m_strIntegrationSystemPath); // 통합 Client가 실행중인가?
            if (isRunning)
            {
                lblClientStatus.Text = "실행중";
                lblClientStatus.ForeColor = Color.Green;
            }
            else
            {
                lblClientStatus.Text = "실행중아님";
                lblClientStatus.ForeColor = Color.Red;
            }

            if (isRunningIntegration)
            {
                lblClientStatusIntegration.Text = "실행중";
                lblClientStatusIntegration.ForeColor = Color.Green;
            }
            else
            {
                lblClientStatusIntegration.Text = "실행중아님";
                lblClientStatusIntegration.ForeColor = Color.Red;
            }

            if (m_netMgrIntegration.IsConnected)
            {
                if (!isRunningIntegration)
                    btnConnectIntegration.Enabled = true;
                else
                    btnConnectIntegration.Enabled = false;
            }
            if (m_netMgr.IsConnected)
            {
                if (!isRunning)
                    btnConnect.Enabled = true;
                else
                    btnConnect.Enabled = false;
            }

            if (m_bOrgConnected != m_netMgr.IsConnected || m_bOrgConnectedIntegration != m_netMgrIntegration.IsConnected || m_bFrist)
            {
                if (!m_bFrist)
                {
                    if (!m_bOrgConnected && m_netMgr.IsConnected)
                    {
                        //if (!notifyIcon1.Visible)                        
                            VisibleThisForm(true);
                    }
                    else
                        VisibleThisForm(true);
                }
                else
                {
                    m_bFrist = false;
                }

                m_bOrgConnected = m_netMgr.IsConnected;
                m_bOrgConnectedIntegration = m_netMgrIntegration.IsConnected;
            }
        }

        private void VisibleThisForm(bool visible)
        {
            // visible true 면 tray로 숨김
            this.notifyIcon1.Visible = !visible;
            this.Visible = visible;
            this.ShowInTaskbar = visible;

            if (visible)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Minimized;
        }

        private bool IsRunning(string path)
        {
            try
            {
                string fullPath = path + "\\" + m_strProcessName + ".exe";

                Process[] p = Process.GetProcessesByName(m_strProcessName);
                if (p.Length == 0)
                    return false;

                if (p[0].MainModule.FileName == fullPath)
                    return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            return false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnConnect.Enabled = false;
            m_timer.Stop();

            DBUtility2.RegUtil.WriteRegValue("Server Connection Info", "webserver_url2", m_strWebServerURL, 205);

            Run(m_strSystemPath + "\\" + m_strProcessName + ".exe");
            m_timer.Start();
        }

        private void btnConnectIntegration_Click(object sender, EventArgs e)
        {
            btnConnectIntegration.Enabled = false;
            m_timer.Stop();

            DBUtility2.RegUtil.WriteRegValue("Server Connection Info", "webserver_url2", m_strIntegrationWebServerURL, 205);

            Run(m_strIntegrationSystemPath + "\\" + m_strProcessName + ".exe");
            m_timer.Start();
        }

        private void Run(string path)
        {
            bool check2 = RunCheckProcess();
            if (check2)
                KillProcess();

            int nCount = 0;
            while (true)
            {
                Thread.Sleep(100);
                nCount++;
                if (nCount == 100)
                    break;

                bool check = RunCheckProcess();
                if (!check)
                    break;
            }

            RunStartProcess(path, "1");
        }

        public bool RunCheckProcess()
        {
            string[] szTarget = { "SOPSimulator2", "ControlTeamEditor", "TeamEditor", "SOPManager2", "IntegratedManagement4",
            "SensorTester", "HwpReport", "SOPBulletin", "SDMS_Building", "Parc1Unity" };

            foreach (string item in szTarget)
            {
                Process[] p = Process.GetProcessesByName(item);
                if (p.Length != 0)
                    return true; 
            }

            return false;
        }

        private void KillProcess()
        {
            string[] szTarget = { "SOPSimulator2", "ControlTeamEditor", "TeamEditor", "SOPManager2", "IntegratedManagement4",
            "SensorTester", "HwpReport", "SDMS", "libCCTV", "SOPBulletin", "SMSSender", "BroadRunner", "SDMS_Building", "UrbanBrixUnity" };

            foreach (string item in szTarget)
            {
                Process[] p = Process.GetProcessesByName(item);
                if (p.Length > 0)
                {
                    p[0].Kill();
                }
            }
        }

        private bool IsStartWith(string strProcessName, string[] processList)
        {
            foreach (string strName in processList)
            {
                if (strProcessName.StartsWith(strName))
                    return true;
            }

            return false;
        }

        public System.Diagnostics.Process RunStartProcess(string strFileName, string args)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = strFileName;
            //startInfo.WorkingDirectory = GetExecutablePath();
            startInfo.ErrorDialog = true;
            startInfo.Arguments = args;

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);

                return process;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
            return null;
        }

        private bool m_bClosed = false;
        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_timer != null)
                m_timer.Stop();

            m_netMgr.ReleaseThread();
            m_netMgrIntegration.ReleaseThread();

            m_bClosed = true;

            this.Close();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            VisibleThisForm(true);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_bClosed)
            {
                e.Cancel = true;
                VisibleThisForm(false);
            }
        }
    }
}
