using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Windows.Forms;



namespace SOPChecker
{
    public partial class FormMain : Form
    {
        private DBUtility.WebDBManager m_dbMgr = null;
		public DBUtility.WebDBManager DBManager
		{
			get { return m_dbMgr; }
			set { m_dbMgr = value; }
		}
        private static FormMain m_instance = null;

        private bool m_finishProcess = false;

        public bool FinishProcess
        {
            get { return m_finishProcess; }
        }

        public static FormMain Instance
        {
            get { return m_instance; }
        }

		//private ControlMonitoring.ControlMonitor monitor = null;


		private NetworkServer server = null;
		private static StreamWriter file = null;

		private static bool bEnableLog = false;
		public static void WriteLine(string szMsg)
		{
			if (bEnableLog == true && file != null)
				file.WriteLine(szMsg);
		}

        private int m_nPort = 20701;

        private int m_nSiteID = -1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private bool ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility("SOPChecker.ini");
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                
                return false;
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
                return false;
            }
            return true;
        }

        public FormMain()
        {
            m_instance = this;

            bool bResult = ReadSiteID();
            if (bResult == true)
            {
                m_dbMgr = new DBUtility.WebDBManager(m_nSiteID);

                //this.dataGridView1 = new System.Windows.Forms.DataGridView();

                server = new NetworkServer(m_nSiteID, m_dbMgr);
                this.dataGridView1 = server.DataGridView1;
                server.FormDelegate = this;

                InitializeComponent();
                
                m_nPort = GetServerPort();
                textBoxPort.Text = m_nPort.ToString();               
            }

            this.Text = GetFormTitle();

            ServerManager.Instance.ToString();

        }

        private int GetServerPort()
        {
            string strSQL = "Select Port from SDMSServerPort";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            nPort += 1;

            return nPort;
        }

        private string GetFormTitle()
        {
            DBUtility.Utility util = new DBUtility.Utility("SOPChecker.ini");
            string szTitle = util.getinivalue("Monitor", "title");
            if (szTitle == null || szTitle == "")
            {
                szTitle = "서버관리자";
            }
            return szTitle;
        }


        private LogFileCleanupTask mCleanUpTask = null;
        private void FormMain_Load(object sender, EventArgs e)
        {

			try
			{
				log4net.Config.DOMConfigurator.Configure();
                
                //log4net.Config.XmlConfigurator.Configure();

                mCleanUpTask = new LogFileCleanupTask();
                mCleanUpTask.CleanUp();
                mCleanUpTask.BeginDailyTask(mCleanUpTask.CleanUp);

			}
			catch (System.Exception)
			{

			}

			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;
            

			if (bEnableLog == true)
				file = new System.IO.StreamWriter(szFullPath + "//server.log");


            if( m_nSiteID > 0)
            {
                server.NetworkServerLoad();
                //UpdateManager.Instance.Run();
                timer1.Start();

            }
        }

        private void InitGrid()
        {
            colIndex.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colIP.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colIP.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colType.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colType.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        private int IndexOfClient(TcpLib2.ConnectionState state)
        {
            int nRowCount = dataGridView1.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                if (row.Tag == state)
                    return i;
            }

            return -1;
        }

        private string GetClientTypeString(ClientData client)
        {
            string strClientType = " 알수 없음";

            if (client.Type == ClientData.ClientType.CONTROLOR)
                strClientType = " CONTORLOR";         
            return strClientType;
        }

        private void btnChangePort_Click(object sender, EventArgs e)
        {
            
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
			if (server != null)
			{
				server.NetworkServerClosing();
                //UpdateManager.Instance.Stop();
                timer1.Stop();
			}

			/*if (monitor != null)
			{
				monitor.Stop();
			}*/

			if (file != null)
			{
				file.Close();
			}
        }

        private void OnTimer(object sender, EventArgs e)
        {
            UpdateManager.Instance.CheckUpdate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProcessBackupLog();
        }

        private void CompleteBackupLog()
        {
            int i = 0;
            i++;
        }
        public void ProcessBackupLog()
        {
            LogBackup backup = new LogBackup();
            backup.Callback += new LogBackupCallback(CompleteBackupLog);
            backup.GatherServerLog();

        }
    }
}
