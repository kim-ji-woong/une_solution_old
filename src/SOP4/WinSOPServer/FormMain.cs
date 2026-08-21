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

using SDMSServer;

namespace SOPServer
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

        private System.Windows.Forms.DataGridView dataGridView1 = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIP = null;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType = null;

		private NetworkServer server = null;
		private static StreamWriter file = null;

		private static bool bEnableLog = false;
		public static void WriteLine(string szMsg)
		{
			if (bEnableLog == true && file != null)
				file.WriteLine(szMsg);
		}

		private int m_nPort = 19500;
        public FormMain()
        {
            m_instance = this;

			//this.dataGridView1 = new System.Windows.Forms.DataGridView();

			server = new NetworkServer();
            SetGrid();
            server.FormDelegate = this;
			
			InitializeComponent();

			

            string strPort = m_dbMgr.LoadIni("sdms_port", "Server Connection Info");
            if (strPort.Length > 0)
            {
                if (int.TryParse(strPort, out m_nPort))
                    textBoxPort.Text = strPort;
            }
            else
                int.TryParse(textBoxPort.Text, out m_nPort);
        }

        private void SetGrid()
        {
            this.dataGridView1 = server.DataGridView1;
            this.Controls.Add(dataGridView1);

            this.dataGridView1.Location = new System.Drawing.Point(14, 101);
            this.dataGridView1.Size = new System.Drawing.Size(413, 194);
            this.dataGridView1.TabIndex = 3;

            colIndex = (DataGridViewTextBoxColumn)this.dataGridView1.Columns[0];
            colIP = (DataGridViewTextBoxColumn)this.dataGridView1.Columns[1];
            colType = (DataGridViewTextBoxColumn)this.dataGridView1.Columns[2];

            // 
            // colIndex
            // 
            this.colIndex.HeaderText = "No";
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            this.colIndex.Width = 40;
            // 
            // colIP
            // 
            this.colIP.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colIP.HeaderText = "IP";
            this.colIP.Name = "colIP";
            // 
            // colType
            // 
            this.colType.HeaderText = "Type";
            this.colType.Name = "colType";
            this.colType.Width = 150;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
			try
			{
				log4net.Config.DOMConfigurator.Configure();
			}
			catch (System.Exception)
			{

			}

			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;

			if (bEnableLog == true)
				file = new System.IO.StreamWriter(szFullPath + "//server.log");

			//mDBConMan = new ConManager();

			
			server.NetworkServerLoad();
			/*monitor = new ControlMonitoring.ControlMonitor();
			monitor.Start();*/
			

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

            if (client.Type == ClientData.ClientType.SDMS_CLIENT)
                strClientType = " SDMS Client";
            else if (client.Type == ClientData.ClientType.SENSOR_SIMULATOR)
                strClientType = " Sensor Simulator";
            else if (client.Type == ClientData.ClientType.SOP_SIMULATOR)
                strClientType = " SOP Simulator";
            else if (client.Type == ClientData.ClientType.SOP_MONITOR2)
                strClientType = "Sensor Monitor";
            else if (client.Type == ClientData.ClientType.SOP_RESOTRE)
                strClientType = "Restore Manager";
            else if (client.Type == ClientData.ClientType.INTEGRATE_MANAGER)
                strClientType = "Integrate Manager";
            else if (client.Type == ClientData.ClientType.SOP_WEATHER)
                strClientType = "기후정보 입력기";

            return strClientType;
        }

        private void WritePortToDB()
        {
            string strSQL = string.Format("Select Max(Port) from SDMSServerPort");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            if (arrResult.Count == 0 || arrResult[0].ToString() == "null")
            {
                strSQL = string.Format("Insert into SDMSServerPort (Port) values ({0})", m_nPort);
                m_dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                strSQL = string.Format("Update SDMSServerPort Set Port = {0}", m_nPort);
                m_dbMgr.GetResultData(strSQL, 0);
            }

        }

        private void btnChangePort_Click(object sender, EventArgs e)
        {            
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
			if (server != null)
			{
				server.NetworkServerClosing();
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

		private void button1_Click(object sender, EventArgs e)
		{
			// 복원 작업
			NetworkServer.Instance.ServiceProvider.SendBeginRestore();
		}

        private void btnShowControlUser_Click(object sender, EventArgs e)
        {
            if (ControlMonitoring.ControlManager.Instance.ControlClient == null)
            {
                MessageBox.Show("제어권 가진 SOP Simulator 없음");
            }
            else
            {
                ControlMonitoring.ControlClient client = ControlMonitoring.ControlManager.Instance.ControlClient.GetControlClient();

                if (client != null)
                {
                    MessageBox.Show("제어권 가진 User ID : " + client.UserID.ToString());
                }
                else
                    MessageBox.Show("제어권 가진 User를 확인할 수 없음");
            }
        }
    }
}
