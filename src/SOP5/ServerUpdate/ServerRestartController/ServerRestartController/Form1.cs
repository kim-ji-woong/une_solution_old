using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;
using SDMSAgent;

namespace ServerRestartController
{
    public partial class Form1 : Form
    {
        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = 1;

        private Timer m_timer = null;

        public Form1()
        {
            InitializeComponent();

            Utility ini = new Utility();

            string strSection = "Server Connection Info";
            string strWebServerURL = ini.getinivalue(strSection, "webserver_url");
            string strDBName = ini.getinivalue(strSection, "server_db");
            string strSiteID = ini.getinivalue(strSection, "siteid");
            string strServerPort = ini.getinivalue(strSection, "server_port");

            int.TryParse(strSiteID, out m_nSiteID);

            m_dbMgr = new WebDBManager(m_nSiteID);
            m_dbMgr.WebServerURL = strWebServerURL;
            m_dbMgr.DatabaseName = strDBName;
            m_dbMgr.DatabaseHost = "127.0.0.1";

            if (strServerPort == "1433")
                m_dbMgr.DatabaseType = WebDBManager.DBType.sqlserver;
            else if (strServerPort == "3306")
                m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;

            m_timer = new Timer();
            m_timer.Interval = 3000;
            m_timer.Tick += m_timer_Tick;
            m_timer.Start();
            m_timer_Tick(null, null);

            if (m_nSiteID == 1)
            {
                pnSOPServer.Visible = true;
                pnTTSServer.Visible = true;
                this.Size = new Size(423, 137);
            }
            else if (m_nSiteID == 2)
            {
                pnSOPServer.Visible = true;
                pnTTSServer.Visible = false;
                this.Size = new Size(423, 95);
            }
        }

        private List<string> searchDateTime = new List<string>();
        string strDatetime = "";
        void m_timer_Tick(object sender, EventArgs e)
        {
            if (strDatetime.Length > 0)
            {
                string strQuery = "select SearchPath, Result from SDMSCommandHistory where timestamp='" + strDatetime + "' and Command=9";
                ArrayList arrResult = m_dbMgr.GetResultData(strQuery, 0);
                if (arrResult != null && arrResult.Count > 0)
                {
                    for (int i = 0; i < arrResult.Count; i+=2)
                    {
                        string strName = arrResult[i].ToString();
                        int nResult = Convert.ToInt32(arrResult[i + 1]);

                        Color color = Color.Red;
                        string strResult = "중지됨";
                        if (nResult == 1)
                        {
                            color = Color.Green;
                            strResult = "실행중";
                        }

                        if (strName == "SOPServer")
                        {
                            lblSOP.Text = strResult;
                            lblSOP.ForeColor = color;
                        }
                    }
                }
            }
            GetTTSServerStatus();

            DateTime dtDatetime = DateTime.Now;
            strDatetime = dtDatetime.ToString("yyyy-MM-dd HH:mm:ss");

            GetServerStatus(true, "SOPServer", dtDatetime);
        }

        private void GetServerStatus(bool isService, string name, DateTime dtDatetime)
        {
            SDMSAgent.CommandItem cmdItem = new SDMSAgent.CommandItem();
            cmdItem.TimeStamp = dtDatetime;
            cmdItem.CmdType = SDMSAgent.CommandType.SERVER_STATUS;
            cmdItem.SearchPath = name;
            cmdItem.IsStartService = isService;

            sendCmdExecute(cmdItem);
        }

        private void GetTTSServerStatus()
        {
            Color color = Color.Red;
            string strResult = "중지됨";
          
            string strQuery = "select heartbeat,getdate() as currentDate from BroadcastState";
            ArrayList arrResult = m_dbMgr.GetResultData(strQuery, 0);
            if (arrResult != null && arrResult.Count == 2)
            {
                DateTime dtHeartBeat = Convert.ToDateTime(arrResult[0]);
                DateTime dtCurrent = Convert.ToDateTime(arrResult[1]);
                if ((dtCurrent - dtHeartBeat).TotalSeconds <= 20)
                {
                    color = Color.Green;
                    strResult = "실행중";
                }
            }

            lblTTS.Text = strResult;
            lblTTS.ForeColor = color;
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            CommandItem cmdItem = MakeCmd(SDMSAgent.CommandType.SOP_SERVER_RESTART, true, true, "SOPServer", true, true, "SOPServer");

            if (sendCmdExecute(cmdItem))
                MessageBox.Show("Command 전송 완료");
        }

        public bool sendCmdExecute(SDMSAgent.CommandItem cmd)
        {
            if (cmd == null)
                return false;

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO SDMSCommand (ID, Command, TimeStamp, SearchPath, IsStop, IsStopService, StopName, IsUpdate, UpdateName, IsStart, IsStartService, StartName) ");
            sb.AppendFormat("           VALUES ((select isnull(max(id)+1,1) from sdmscommand), {0}, '{1}', '{2}', {3}, {4}, '{5}', {6}, '{7}', {8}, {9}, '{10}')"
                , (int)cmd.CmdType
                , cmd.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")
                , cmd.SearchPath
                , (cmd.IsStop) ? 1 : 0, (cmd.IsStopService) ? 1 : 0, cmd.StopName
                , (cmd.IsUpdate) ? 1 : 0, cmd.UpdateName
                , (cmd.IsStart) ? 1 : 0, (cmd.IsStartService) ? 1 : 0, cmd.StartName);

            if (m_dbMgr.GetResultData(sb.ToString(), 0) == null)
                return false;

            return true;
        }

        private void btnTTS_Click(object sender, EventArgs e)
        {
            CommandItem cmdItem = MakeCmd(SDMSAgent.CommandType.UPDATE, true, false, "TTSServerDotNetCmd", true, false, @"C:\TTSServer\TTSServerDotNetCmd.exe");

            if (sendCmdExecute(cmdItem))
                MessageBox.Show("Command 전송 완료");
        }

        private void btnSOPStop_Click(object sender, EventArgs e)
        {
            CommandItem cmdItem = MakeCmd(SDMSAgent.CommandType.UPDATE, true, true, "SOPServer", false, false, "");

            if (sendCmdExecute(cmdItem))
                MessageBox.Show("Command 전송 완료");
        }

        private void btnTTSStop_Click(object sender, EventArgs e)
        {
            CommandItem cmdItem = MakeCmd(SDMSAgent.CommandType.UPDATE, true, false, "TTSServerDotNetCmd", false, false, "");

            sendCmdExecute(cmdItem);

            // 삼천포 방송서버 바로가기 명칭이 '방송서버'로 되어있다.
            // 바로가기가 실행되고 있는지 아닌지 모르기 때문에 둘다 KILL한다.
            cmdItem = MakeCmd(SDMSAgent.CommandType.UPDATE, true, false, "방송서버", false, false, "");

            if (sendCmdExecute(cmdItem))
                MessageBox.Show("Command 전송 완료");
        }

        private void btnSOPStart_Click(object sender, EventArgs e)
        {
            CommandItem cmdItem = MakeCmd(SDMSAgent.CommandType.UPDATE, false, false, "", true, true, "SOPServer");

            if (sendCmdExecute(cmdItem))
                MessageBox.Show("Command 전송 완료");
        }

        private void btnTTSStart_Click(object sender, EventArgs e)
        {
            CommandItem cmdItem = MakeCmd(SDMSAgent.CommandType.UPDATE, false, false, "", true, false, @"C:\TTSServer\TTSServerDotNetCmd.exe");
            
            if (sendCmdExecute(cmdItem))
                MessageBox.Show("Command 전송 완료");
        }

        private CommandItem MakeCmd(SDMSAgent.CommandType type, bool isStop, bool isStopService, string stopName, bool isStart, bool isStartService, string startName)
        {
            SDMSAgent.CommandItem cmdItem = new SDMSAgent.CommandItem();
            cmdItem.TimeStamp = DateTime.Now;
            cmdItem.CmdType = SDMSAgent.CommandType.UPDATE;
            cmdItem.IsStop = isStop;
            cmdItem.IsStopService = isStopService;
            cmdItem.StopName = stopName;
            cmdItem.IsStart = isStart;
            cmdItem.IsStartService = isStartService;
            cmdItem.StartName = startName;

            return cmdItem;
        }
    }
}
