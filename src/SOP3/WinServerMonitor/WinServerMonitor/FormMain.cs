using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using DBUtility;
using System.Diagnostics;
using System.Threading;

namespace ServerMonitor
{
    public partial class FormMain : Form
    {
        static public FormMain Instance;
        private WebDBManager m_dbMgr = null;      

        NetworkManager m_netMgr = null;
        NetworkManager_4_SOPServer m_netMgr4SOPServer = null;

        private bool m_completeClientLog = false;
        private bool m_completeServerLog = false;

        public bool CompleteClientLog
        {
            set
            {
                m_completeClientLog = value;

                this.Invoke((MethodInvoker)delegate
                {
                    if (m_completeClientLog == false)
                        MessageBox.Show("Client 로그 파일을 받아올 수 없습니다.");
                    else if (m_completeServerLog)
                        MessageBox.Show("지정된 경로에 로그 파일이 생성되었습니다.");
                });
            }
        }

        public bool CompleteServerLog
        {
            set
            {
                m_completeServerLog = value;

                this.Invoke((MethodInvoker)delegate
                {
                    if (m_completeServerLog == false)
                        MessageBox.Show("Server 로그 파일을 받아올 수 없습니다.");
                    else if (m_completeClientLog)
                        MessageBox.Show("지정된 경로에 로그 파일이 생성되었습니다.");
                });
            }
        }
        

        private int m_nSiteID = 1;

        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                MessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
        }

        UnE.Log.LogFileCleanupTask m_LogCleanTask = null;
        public FormMain()
        {
            InitializeComponent();

            Instance = this;

            ReadSiteID();

            m_dbMgr = new WebDBManager(m_nSiteID);
            m_netMgr = new NetworkManager(m_dbMgr, m_nSiteID);
            m_netMgr4SOPServer = new NetworkManager_4_SOPServer(m_dbMgr, m_nSiteID);

            init();

            try
            {
                m_LogCleanTask = new UnE.Log.LogFileCleanupTask();
                m_LogCleanTask.CleanUp();
                m_LogCleanTask.BeginDailyTask(m_LogCleanTask.CleanUp);
            }
            catch(Exception)
            {

            }
        }

        public WebDBManager DbMgr
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

		

        private void init()
        {                   
        }        

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_CheckTimer.Stop();
            m_CheckTimer.Enabled = false;
            m_netMgr.ReleaseThread();
        }

        public void AddLog(object strMsg)
        {
            Debug.WriteLine(strMsg.ToString());
        }

		private void FormMain_Load(object sender, EventArgs e)
		{
            m_CheckTimer.Interval = 2000;
            m_CheckTimer.Enabled = true;
            m_CheckTimer.Start();

            if (m_netMgr.IsConnected())
            {
                SetServerState(0, true);
            }
            else
            {
                SetServerState(0, false);
            }

		}

        private void SetState(Label lb, bool bRun)
        {
            if( bRun == true)
            {
                lb.BackColor = Color.Blue;
                lb.Text = "정상동작";
            }
            else
            {
                lb.BackColor = Color.Red;
                lb.Text = "정지상태";
            }  
        }


        private void button1_Click(object sender, EventArgs e)
        {            
            this.Close();
        }

        public void SetServerState(int nServer, bool bRun)
        {
            if (nServer == 0)
            {
                if (bRun == true)
                {
                    stateMonitor.BackColor = Color.Blue;
                    stateMonitor.Text = "정상동작";
                }
                else
                {
                    stateMonitor.BackColor = Color.Red;
                    stateMonitor.Text = "연결안됨";

                    stateTTS.BackColor = Color.Red;
                    stateTTS.Text = "연결안됨";

                    stateSOP.BackColor = Color.Red;
                    stateSOP.Text = "연결안됨";
                }  
            }
            else if (nServer == 1)
            {
                SetState(stateTTS, bRun);                
            }
            else if (nServer == 2)
            {
                SetState(stateSOP, bRun);  
                 
            }
            else if (nServer == 3)
            {
                SetState(stateBackup, bRun);
            }
        }


        private void m_CheckTimer_Tick(object sender, EventArgs e)
        {
            if (m_netMgr.IsConnected())
            {
                SetServerState(0, true);
                
            }
            else
            {
                SetServerState(0, false);
            }
            m_netMgr.SendCheckState();
        }

        private void btnStartTTS_Click(object sender, EventArgs e)
        {
            m_netMgr.SendStartTTS();
        }

        private void btnStopTTS_Click(object sender, EventArgs e)
        {
            m_netMgr.SendStopTTS();
        }

        private void btnStartSOP_Click(object sender, EventArgs e)
        {
            m_netMgr.SendStartSOP();
        }

        private void btnStopSOP_Click(object sender, EventArgs e)
        {
            m_netMgr.SendStopSOP();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            m_netMgr.SendStartSenor();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            m_netMgr.SendStopSensor();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            m_completeClientLog = m_completeServerLog = false;

            if (!m_netMgr.SendBackupLog())
            {
                MessageBox.Show("서버에 접속할 수 없습니다.");
                return;
            }

            LogBackup backup = new LogBackup();
            backup.GatherServerLog();
        }

        private void btnLogFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            string strOriginPath = GetBackupLogFolder();

            if (strOriginPath.Length > 0)
            {
                dlg.SelectedPath = strOriginPath;
                dlg.Description = string.Format("백업 로그가 저장될 폴더를 지정합니다.\r\n\r\n현재경로 : {0}", strOriginPath);
            }
            else
                dlg.Description = "백업 로그가 저장될 폴더를 지정합니다.";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string strPath = dlg.SelectedPath;
                RegUtil.WriteRegValue("BackupLog", "Path", strPath, m_nSiteID);
            }
        }

        public string GetBackupLogFolder()
        {
            string strPath = RegUtil.ReadRegValue("BackupLog", "Path", m_nSiteID);

            if (strPath == "")
                strPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            return strPath;
        }

        private void btnUpdateImmediately_Click(object sender, EventArgs e)
        {
            if (m_netMgr4SOPServer.SendUpdateInform())
                btnUpdateImmediately.Enabled = false;
        }

        public void EnableUpdateButton()
        {
            Invoke((MethodInvoker)delegate
            {
                btnUpdateImmediately.Enabled = true;
            });
        }
    }

    public class SOPMonitor
    {
        private static FormMain m_nstance = null;

        public static FormMain Instance
        {
            get { return FormMain.Instance; }

        }
    }
}
