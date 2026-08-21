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
using System.Net;

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
            DBUtility.Utility util = new DBUtility.Utility("SOPChecker.ini");
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


        protected SortedList<int, ServerInfo> m_ServerList = new SortedList<int, ServerInfo>();
        public List<ServerInfo> GetServerList()
        {
            List<ServerInfo> result = new List<ServerInfo>();
            result.AddRange(m_ServerList.Values);
            return result;
        }

        protected virtual void ReadServerInfo()
        {
            string szSection = "Server Info";
            DBUtility.Utility util = new DBUtility.Utility("SOPChecker.ini");
            string szServerList = util.getinivalue(szSection, "server");
            if (!string.IsNullOrEmpty(szServerList))
            {
                string[] servers = szServerList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (servers != null && servers.Length > 0)
                {
                    for (int i = 0; i < servers.Length; i++)
                    {
                        string szTemp1 = util.getinivalue(servers[i], "ID");
                        string szTemp2 = util.getinivalue(servers[i], "Path");
                        string szTemp3 = util.getinivalue(servers[i], "File");
                        string szTemp4 = util.getinivalue(servers[i], "Service");
                        string szTemp5 = util.getinivalue(servers[i], "ShortName");

                        int nID = -1;
                        int.TryParse(szTemp1, out nID);

                        int nService = -1;
                        int.TryParse(szTemp4, out nService);

                        if (nID >= 0)
                        {
                            ServerInfo info = new ServerInfo(szTemp5, szTemp2, szTemp3, nID, nService == 1 ? true : false);
                            m_ServerList.Add(nID, info);
                        }
                    }
                }
            }
        }

        int nCount = 1;
        private Label AddServer(int nServerType, string ServerName, int y)
        {
            Label lableServer = new Label();
            lableServer.AutoSize = true;           
            lableServer.Name = "label11";
            lableServer.Size = new System.Drawing.Size(65, 12);
            lableServer.Text = ServerName;

            Button btnStop = new Button();           
            btnStop.Name = ServerName;
            btnStop.Size = new System.Drawing.Size(44, 26);
            btnStop.Text = "종료";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Tag = nServerType;
           
            Button btnStart = new Button();
            btnStart.Name = ServerName;
            btnStart.Size = new System.Drawing.Size(44, 26);
            btnStart.TabIndex = 25;
            btnStart.Text = "시작";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Tag = nServerType;
            
            Label labelState = new Label();
            labelState.BackColor = System.Drawing.Color.Red;
            labelState.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            labelState.ForeColor = System.Drawing.Color.White;
            labelState.Name = "stateServer6";
            labelState.Size = new System.Drawing.Size(106, 26);
            labelState.Text = "연결안됨";
            labelState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            lableServer.Location = new System.Drawing.Point(18, y + 7);
            btnStart.Location = new System.Drawing.Point(101, y);
            btnStop.Location = new System.Drawing.Point(147, y);
            labelState.Location = new System.Drawing.Point(194, y);
            
            lableServer.Visible = true;
            btnStart.Visible = true;
            btnStop.Visible = true;
            labelState.Visible = true;

            grpMonitor.Controls.Add(lableServer);
            grpMonitor.Controls.Add(btnStart);
            grpMonitor.Controls.Add(btnStop);
            grpMonitor.Controls.Add(labelState);
            
            btnStart.Click += new System.EventHandler(this.btnStartBtnClick);
            btnStop.Click += new System.EventHandler(this.btnStopBtnClick);

            startBtns[nCount] = btnStart;
            stopButtons[nCount] = btnStop;
            
            nCount++;

            return labelState;
        }

        public void btnStartBtnClick(object sensor, EventArgs arg)
        {
            Button btn = (Button)sensor;
            int nType = (int)btn.Tag;
            m_netMgr.SendStartServer(nType);
        }
        public void btnStopBtnClick(object sensor, EventArgs arg)
        {
            Button btn = (Button)sensor;
            int nType = (int)btn.Tag;
            m_netMgr.SendStopServer(nType);
        }

        private UnE.Log.LogFileCleanupTask m_LogCleanTask = null;

        private Label[] stateLables = null;
        private Button[] startBtns = null;
        private Button[] stopButtons = null;

        private string GetFormTitle()
        {
            DBUtility.Utility util = new DBUtility.Utility("SOPChecker.ini");
            string szTitle = util.getinivalue("Monitor", "title");
            if (szTitle == null || szTitle == "")
            {
                szTitle = "서버모니터";
            }
            return szTitle;
        }

        public FormMain()
        {
            InitializeComponent();

            ReadServerInfo();

            this.Text = GetFormTitle();

            List<ServerInfo> serverList = GetServerList();
            int nCount = serverList.Count + 1;
            stateLables = new Label[nCount];
            stateLables[0] = stateMonitor;

            startBtns = new Button[nCount];
            startBtns[0] = btnStart1;
            stopButtons = new Button[nCount];
            stopButtons[0] = btnStop1;

            int y = 69;
            for (int i = 0; i < serverList.Count; i++)
            {
                ServerInfo info = serverList[i];
                stateLables[i+1] = AddServer(info.ServerID, info.ServerName, y);
                y += 45;
            }         

            grpMonitor.Size = new System.Drawing.Size(319, y);

            btnUploadFile.Location = new Point(33, y + 30);
            btnClose.Location = new Point(245, y + 30);
            this.Size = new System.Drawing.Size(356, y + 100);

            Instance = this;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.ContextMenuStrip = null;

            notifyIcon1.BalloonTipTitle = this.Text;
            notifyIcon1.Visible = true;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            
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
        private bool m_bExitProgram = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (m_bExitProgram == false)
            {
                e.Cancel = true;
                Iconize();
                return;
            }

            this.notifyIcon1.Visible = false;

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
                btnStart1.Enabled = false;
            }
            else
            {
                SetServerState(0, false);
                btnStart1.Enabled = true;
            }

		}

        private void SetState(Label lb, bool bRun)
        {
            if (lb == null)
                return;

            if( bRun == true)
            {
                lb.ForeColor = Color.White;                
                lb.BackColor = Color.Blue;
                lb.Text = "정상동작";
            }
            else
            {
                lb.ForeColor = Color.White;    
                lb.BackColor = Color.Red;
                lb.Text = "정지상태";
            }  
        }


        private void button1_Click(object sender, EventArgs e)
        {            
            this.Close();
        }

        bool bShowBalloon = false;
        public void SetServerState(int nServer, bool bRun)
        {
            if (nServer == 0)
            {
                if (bRun == true)
                {
                    stateMonitor.BackColor = Color.Blue;
                    stateMonitor.Text = "정상동작";

                    notifyIcon1.BalloonTipText = "정상동작";
                    if (bShowBalloon == false)
                    {
                        notifyIcon1.ShowBalloonTip(1000);
                        bShowBalloon = true;
                    }                    
                }
                else
                {
                    if (bLoading == false)
                    {
                        stateMonitor.BackColor = Color.Red;
                        stateMonitor.Text = "연결안됨";
                        notifyIcon1.BalloonTipText = "연결안됨";
                        if (bShowBalloon == true)
                        {
                            notifyIcon1.ShowBalloonTip(1000);
                            bShowBalloon = false;
                        }

                        for (int i = 1; i < stateLables.Length; i++)
                        {
                            if (stateLables[i] != null)
                                SetState(stateLables[i], false);
                        }
                    }
                }  
            }
            else if (nServer > 0)
            {
                if( bLoading == false)
                {

                    if (stateLables != null && nServer < stateLables.Length)
                        SetState(stateLables[nServer], bRun);                
                }

            } 
        }


        private void m_CheckTimer_Tick(object sender, EventArgs e)
        {
            if (m_netMgr.IsConnected())
            {
                SetServerState(0, true);
                btnStart1.Enabled = false;
            }
            else
            {
                SetServerState(0, false);
                btnStart1.Enabled = true;
            }
            m_netMgr.SendCheckState();
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

        public void ShowToolTipMessage(string szMessage)
        {
            notifyIcon1.BalloonTipText = szMessage;
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.BalloonTipTitle = "접속정보";
            notifyIcon1.ShowBalloonTip(3000);
        }

        private void Iconize()
        {
            //this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            this.notifyIcon1.Visible = true;
        }

        private void Normalize()
        {
            this.ShowInTaskbar = true;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = false;

            this.BringToFront();
        }

        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                Iconize();
            }
            else
            {
                Normalize();
            }
        }

        private void 종료하기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("이 프로그램을 종료하시면 e재난서버 상태를 확인 할 수 없습니다. \n그래도 종료하시겠습니까?", "종료알림", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                // Save info
                m_bExitProgram = true;
                this.Close();
            }
        }

        private void btnUploadFile_Clicked(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "zip";
            dlg.Filter = "Zip files (*.zip)|*.zip|All files (*.*)|*.*";
            dlg.Multiselect = false;


            if( dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string szFileName = dlg.FileName;
                System.Diagnostics.Trace.WriteLine(szFileName);

                Thread t = new Thread(FileUploadThread);
                t.Start(szFileName);
                
            }
        }

        private void FileUploadThread(object param)
        {
            try
            {
                string szFileName = (string)param;
                string szServerName = m_netMgr.ServerAddr;

                int nIndex = szFileName.LastIndexOf('\\');
                string strUploadFileName = szFileName.Substring(nIndex + 1);
                
                
                WebClient wc = new WebClient();

                Uri uri = new Uri("http://" + szServerName + ":8080/webDAV/" + strUploadFileName);
                System.Diagnostics.Trace.WriteLine(uri.ToString());

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                wc.Credentials = new NetworkCredential("sop", "sop");
                wc.UploadFile(uri, "PUT", szFileName);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }
        }

        private System.Windows.Forms.Timer rdpTimer = new System.Windows.Forms.Timer();
        private System.Diagnostics.Process rdcProcess = null;
        private void btnStart1_Click(object sender, EventArgs e)
        {
            string szPath = ReadConnectionInfo();
            var tempProcess = new Process
            {
                
                StartInfo =
                {
                    FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\cmdkey.exe"),
                   
                    Arguments = String.Format(@"/generic:TERMSRV/{0} /user:{1} /pass:{2}",
                                m_strServerIP,
                                m_strServerID,
                                szPath),
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            tempProcess.Start();


            //rdcProcess = new Process();
            //rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
            //rdcProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //rdcProcess.StartInfo.Arguments = String.Format("/v {0}", m_strServerIP);
            //rdcProcess.Start();


            rdcProcess = new Process();
            rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
            rdcProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            rdcProcess.StartInfo.Arguments = String.Format("/v {0}", m_strServerIP);
            rdcProcess.Start();

            IntPtr nID = rdcProcess.Handle;
            int pid = rdcProcess.Id;

            System.Diagnostics.Trace.WriteLine("ProcessID " + pid);


            if (rdpTimer.Enabled == false)
            {
                rdpTimer.Interval = 6000;
                rdpTimer.Tick += rdpTimer_Tick;
                rdpTimer.Enabled = true;
                rdpTimer.Start();
            }

            InitLoad();
         
        }

        void rdpTimer_Tick(object sender, EventArgs e)
        {
         
            try
            {
                CloseAllTerm();

            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }  
        }

        public void CloseAllTerm()
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();
            ArrayList arList = new ArrayList();
            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == "mstsc")
                    arList.Add(process);
            }

            if (arList.Count == 0)
            {
                rdpTimer.Stop();
                rdpTimer.Enabled = false;
               
            }

            foreach (Process  proc in arList)
            {              
                try
                {
                    proc.CloseMainWindow();
                    proc.Close();
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
            }
        }

        int nTime = 60;
        private bool bLoading = false;
        public void InitLoad()
        {
            if (bLoading == true)
                return;

            bLoading = true;
            string szSection = "Monitor";
            DBUtility.Utility util = new DBUtility.Utility("SOPChecker.ini");
            string szTime = util.getinivalue(szSection, "checkTime");
          
            int.TryParse(szTime, out nTime);


            for (int i = 1; i < startBtns.Length; i++)
            {
                if (startBtns[i] != null)
                {
                    startBtns[i].Enabled = false;
                    stopButtons[i].Enabled = false;
                }
            }

            stateLables[0].BackColor = Color.Orange;
            stateLables[0].Text = "시작중";

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
            timer.Enabled = true;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if(nTime <= 0)
            {
                System.Windows.Forms.Timer timer = (System.Windows.Forms.Timer)sender;
                timer.Enabled = false;
                timer.Stop();

                for (int i = 1; i < startBtns.Length; i++)
                {
                    if (startBtns[i] != null)
                    {
                        startBtns[i].Enabled = true;
                        stopButtons[i].Enabled = true;
                    }
                }

                for (int i = 1; i < stateLables.Length; i++)
                {
                    if (stateLables[i] != null)
                    {
                        stateLables[i].ForeColor = Color.White;
                     }
                }
                bLoading = false;
            }
     
            for (int i = 1; i < stateLables.Length; i++)
            {
                if (stateLables[i] != null)
                {
                    stateLables[i].Text = "" + nTime + "초";
                    stateLables[i].ForeColor = Color.Black;
                    stateLables[i].BackColor = Color.Orange;
                }
            }
            nTime = nTime - 1;
        }

        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        private string m_strServerIP = "";
        private string m_strServerID = "";
        private string ReadConnectionInfo()
        {
            DBUtility.Utility util = new DBUtility.Utility("SOPChecker.ini");
			string strSection = "Monitor";
            string m_strServerPW = "";
            m_strServerIP = util.getinivalue(strSection, "termserverip");			
			try
			{
                string idpass = util.getinivalue(strSection, "termconn");				
				string strDec = DBUtility.AES256Cipher.AES_decrypt(idpass, key);
				
				m_strServerID = strDec.Substring(0, strDec.IndexOf('|'));
				m_strServerPW = strDec.Substring(strDec.IndexOf('|') + 1);
			}
			catch (System.Exception)
			{              
			}
            return m_strServerPW;
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

    public class ServerInfo
    {
        public ServerInfo()
        {
        }

        public ServerInfo(string szServerName, string szFilePath, string szFileName, int nServerID, bool bService)
        {
            m_nServerID = nServerID;
            m_szServerName = szServerName;
            m_szFilePath = szFilePath;
            m_szFileName = szFileName;
            m_bService = bService;
        }

        private int m_nServerID = -1;
        public int ServerID
        {
            get { return m_nServerID; }
            set { m_nServerID = value; }
        }

        private string m_szServerName = "";
        public string ServerName
        {
            get { return m_szServerName; }
            set { m_szServerName = value; }
        }


        private string m_szFilePath = "";
        public string FilePath
        {
            get { return m_szFilePath; }
            set { m_szFilePath = value; }
        }


        private string m_szFileName = "";
        public string FileName
        {
            get { return m_szFileName; }
            set { m_szFileName = value; }
        }

        private bool m_bService = false;
        public bool IsService
        {
            get { return m_bService; }
            set { m_bService = value; }
        }

    }
}
