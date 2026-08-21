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
		private ConManager mDBConMan = null;

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

		private int m_nPort = 19500;
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
                    textBoxPort.Text = strPort;
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
			else if (client.Type == ClientData.ClientType.SOP_MONITOR)
				strClientType = "Sensor Monitor";
			else if (client.Type == ClientData.ClientType.SOP_RESOTRE)
				strClientType = "Restore Manager";
			else if (client.Type == ClientData.ClientType.INTEGRATE_MANAGER)
				strClientType = "Integrate Manager";
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
            int nPort;
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


        private static int nHistoryID = 0;
        int nCurrentHistory = 1;
        int nCurZone = 1;
        private void button2_Click(object sender, EventArgs e)
        {
            nHistoryID++;
            
            int nIdx = comboBox1.Items.Add(nHistoryID);
            comboBox1.SelectedIndex = nIdx;
            nCurrentHistory = nHistoryID;
            NetworkServer.Instance.ServiceProvider.SendSensorZoneData(nCurZone, nCurrentHistory, ClientData.ClientType.SDMS_CLIENT);
            NetworkServer.Instance.ServiceProvider.SendTestMessage(nCurZone, nCurrentHistory, SensorReactionLog.ReactionType.BEGIN_STATUS);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            
            NetworkServer.Instance.ServiceProvider.SendSensorZoneData(0, nCurrentHistory, ClientData.ClientType.SDMS_CLIENT);
            NetworkServer.Instance.ServiceProvider.SendTestMessage(1, nCurrentHistory, SensorReactionLog.ReactionType.END_STATUS);
            NetworkServer.Instance.ServiceProvider.SendTestClearDetectReport(nCurZone, nCurrentHistory);

            if (comboBox1.Items.Contains(nCurrentHistory))
            {
                comboBox1.Items.Remove(nCurrentHistory);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            NetworkServer.Instance.ServiceProvider.SendTestMessage(nCurZone, nCurrentHistory, SensorReactionLog.ReactionType.IGNORE_SOP);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //sOP종료
            NetworkServer.Instance.ServiceProvider.SendTestMessage(nCurZone, nCurrentHistory, SensorReactionLog.ReactionType.FINISH_SOP);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            NetworkServer.Instance.ServiceProvider.SendTestMessage(nCurZone, nCurrentHistory, SensorReactionLog.ReactionType.RUN_N_CANCEL_SOP);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            NetworkServer.Instance.ServiceProvider.SendTestMessage(nCurZone, nCurrentHistory, SensorReactionLog.ReactionType.MALFUNCTION);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            NetworkServer.Instance.ServiceProvider.SendTestMessage(nCurZone, nCurrentHistory, SensorReactionLog.ReactionType.NOTIFY_FIRE);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
                return;

            nCurrentHistory = (int)comboBox1.SelectedItem;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
                return;
            string szText = (string)comboBox2.SelectedItem;
            if (szText == null)
                return;

            if (int.TryParse(szText, out nCurZone))
            {

            }
        }


    }
}
