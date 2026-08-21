using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Windows.Forms;



namespace SOPChecker
{
    public partial class FormMain : Form
    {		
        private DBUtility.WebDBManager m_dbMgr = new DBUtility.WebDBManager();
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
        public FormMain()
        {
            m_instance = this;

			//this.dataGridView1 = new System.Windows.Forms.DataGridView();

			server = new NetworkServer();
			this.dataGridView1 = server.DataGridView1;
            server.FormDelegate = this;
			
			InitializeComponent();

			

            string strPort = m_dbMgr.LoadIni("sdms_port", "Server Connection Info");
            if (strPort.Length > 0)
            {
                if (int.TryParse(strPort, out m_nPort))
                {
                    m_nPort += 1;
                    textBoxPort.Text = m_nPort.ToString();
                }
            }
            else
                int.TryParse(textBoxPort.Text, out m_nPort);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
			try
			{
				log4net.Config.DOMConfigurator.Configure();
			}
			catch (System.Exception ex)
			{

			}

			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;

			if (bEnableLog == true)
				file = new System.IO.StreamWriter(szFullPath + "//server.log");

            server.NetworkServerLoad();
            //UpdateManager.Instance.Run();
            timer1.Start();
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
            int nPort;
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
