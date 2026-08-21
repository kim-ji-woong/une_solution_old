using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TankServer
{
    public partial class FormMain : Form
    {     
#if !SERVICE 
        private bool bStart = false;
        private LevelMeterNetworkServer server = null;
        //private NetworkClient client = null;
        private TankLevelMeterManager sensor = null;
        DBUtility.WebDBManager dbMgr = null;

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public FormMain()
        {
            InitializeComponent();

            m_instance = this;

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            this.Hide();

            //[DEBUG][2017-11-07 13:41:13] : [RECIVED TXT] : 01 00 00 00 00 05 01 04 02 01 F0
            //[DEBUG][2017-11-07 13:41:20] : [RECIVED TXT] : 01 00 00 00 00 05 02 04 02 02 F0
            //[DEBUG][2017-11-07 13:41:29] : [RECIVED TXT] : 01 00 00 00 00 05 03 04 02 03 F0
            //[DEBUG][2017-11-07 13:41:37] : [RECIVED TXT] : 01 00 00 00 00 05 04 04 02 04 F0
            //[DEBUG][2017-11-07 13:41:45] : [RECIVED TXT] : 01 00 00 00 00 05 05 04 02 05 F0
        }
        public void SetText()
        {
            for (int i = 0; i < TankLevelMeterManager.Instance.m_TankList.Count; i++)
            {
                TankInfo info = (TankInfo)TankLevelMeterManager.Instance.m_TankList[i];
                 
                if (info.ID == 11)                    
                    lable_evtStatus211.Text = info.LeakStatus.ToString();
                if (info.ID == 12)
                    lable_evtStatus212.Text = info.LeakStatus.ToString();
                if (info.ID == 13)
                    lable_evtStatus214.Text = info.LeakStatus.ToString();
                if (info.ID == 14)
                    lable_evtStatus215.Text = info.LeakStatus.ToString();
                if (info.ID == 15)
                    lable_evtStatus216.Text = info.LeakStatus.ToString();

            } 
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            OnBeginServer(null, null);
        }
         
        private void OnBeginServer(object sender, EventArgs e)
        {
            if (bStart == true) return;

            server = new LevelMeterNetworkServer();

            dbMgr = LevelMeterNetworkServer.Instance.DBManager;

            //client = new NetworkClient(dbMgr, null, LevelMeterNetworkServer.Instance.SiteID);

            sensor = new TankLevelMeterManager();

            // 새로 접속하니까 일단 모두 접속이 끊긴 것으로 초기화
            //sensor.SaveAllSensorServerInfo(false);

            server.NetworkServerLoad();
            sensor.BeginServer(GasDetector_OnNotifyAlarm);
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();

            bStart = true; 
        }    
         
        void GasDetector_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {

        }
         
        private void timer1_Tick(object sender, EventArgs e)
        {
            SetText();
        }       

        #region 서버 끝
        private void OnStopServer(object sender, EventArgs e)
        {
            //if (client != null)            
            //{
            //    if(client.ClientProvider.IsConnected == true)
            //        client.ClientProvider.Close();
            //    client.ShutdownSensorThread = true;
            //}

            if (sensor != null)
                sensor.StopServer();

            timer1.Stop();
            timer1.Enabled = false;

            bStart = false;
        }
        bool isClose = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        { 
            if (!isClose)
            {
                e.Cancel = true;
                this.notifyIcon1.Visible = true;
                this.Hide();
            }
            else
            {
                timer1.Stop();
                timer1.Enabled = false;

                if (server != null)
                    server.NetworkServerClosing();
                 
                if (sensor != null)
                    sensor.StopServer();
            } 
        } 
        #endregion  
          
        private void button_1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("tank id 입력");
                return;
            }

            int cmdID = GetMaxID("Command") + 1;
            int cmdHistoryID = GetMaxID("CommandHistory") + 1;
            string query = "INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID, CommandValue) VALUES ({0}, 10, now(), {1}, 1, 0)";
            dbMgr.GetResultData(string.Format(query, cmdID, textBox1.Text), 0);

            string query2 = "INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, CommandValue) VALUE ({0}, 10, now(), null, 1, {1},{2}, 0)";
            dbMgr.GetResultData(string.Format(query2, cmdHistoryID, cmdID, textBox1.Text), 0);
        }

        private void button_2_Click(object sender, EventArgs e)
        {
            sensor.Detector.SetControlRegister(1, 4, 0, 1);

            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("tank id 입력");
                return;
            }

            int cmdID = GetMaxID("Command") + 1;
            int cmdHistoryID = GetMaxID("CommandHistory") + 1;
            string query = "INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID, CommandValue) VALUES ({0}, 11, now(), {1}, 1, 0)";
            dbMgr.GetResultData(string.Format(query, cmdID, textBox1.Text), 0);

            string query2 = "INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, CommandValue) VALUE ({0}, 11, now(), null, 1, {1},{2}, 0)";
            dbMgr.GetResultData(string.Format(query2, cmdHistoryID, cmdID, textBox1.Text), 0);
        }

        private void button_3_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length == 0)
            {
                MessageBox.Show("tank id 입력");
                return;
            }

            int cmdID = GetMaxID("Command") + 1;
            int cmdHistoryID = GetMaxID("CommandHistory") + 1;
            string query = "INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID, CommandValue) VALUES ({0}, 12, now(), {1}, 1, 0)";
            dbMgr.GetResultData(string.Format(query, cmdID, textBox1.Text), 0);

            string query2 = "INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, CommandValue) VALUE ({0}, 12, now(), null, 1, {1},{2}, 0)";
            dbMgr.GetResultData(string.Format(query2, cmdHistoryID, cmdID, textBox1.Text), 0);
        }

        private int GetMaxID(string strTableName)
        {
            string strSQL = "select MAX(ID) from " + strTableName;
            System.Collections.ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;

            this.notifyIcon1.Visible = false;
            this.Show();
            this.Activate();
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isClose = true;
            this.Close();
        }
#endif 
    }  
}
