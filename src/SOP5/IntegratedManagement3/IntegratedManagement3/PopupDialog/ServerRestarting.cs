using DBUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntegratedManagement3.PopupDialog
{
    public partial class ServerRestarting : Form
    {
        public bool IsServerRestarted = false;
        private WebDBManager m_dbMgr = null;

        public ServerRestarting(WebDBManager dbMgr)
        {
            InitializeComponent();
            this.Cursor = Cursors.WaitCursor;
            m_dbMgr = dbMgr;
            Restart();
        }

        private void Restart()
        {
            // 서버 재시작 command 
            string strTimeStamp = InsertCommandServerRestart();
            
            int nTimerCount = 0;
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += (s, timerevent) =>
            {
                //isServerRestarting = true;
                if (RestartServer(strTimeStamp) && FormMain.Instance.NetManager.ClientProvider.IsConnected)
                {
                    timer.Stop();
                    IsServerRestarted = true;
                    //System.Threading.Thread.Sleep(7000);
                    this.Close();
                }
                else
                {
                    nTimerCount++;
                    if (labelProgress.Text.Length > 20)
                        labelProgress.Text = "...";
                    else
                        labelProgress.Text += "."; 

                    if (nTimerCount > 60)
                    {
                        nTimerCount = 0;
                        //isServerRestarting = false;
                        timer.Stop();
                        IsServerRestarted = false;
                        this.Cursor = Cursors.Default;
                        this.Close();
                    }
                }
            };
            timer.Start(); 
        }

        private string InsertCommandServerRestart()
        {
            string strTimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO SDMSCommand (ID, Command, TimeStamp, SearchPath, IsStop, IsStopService, StopName, IsUpdate, UpdateName, IsStart, IsStartService, StartName) ");
            sb.AppendFormat("           VALUES ((select isnull(max(id)+1,1) from sdmscommand), {0}, '{1}', '{2}', {3}, {4}, '{5}', {6}, '{7}', {8}, {9}, '{10}')"
                , 8 /*CommandType.SOP_SERVER_RESTART*/
                , strTimeStamp
                , ""
                , 1, 1, "SOPServer"
                , 0, ""
                , 1, 1, "SOPServer");

            if (m_dbMgr.GetResultData(sb.ToString(), 0) == null)
                return "";

            return strTimeStamp;
        }

        private bool RestartServer(string strTimeStamp)
        {
            string strQuery = "select result from SDMSCommandHistory where TimeStamp = '" + strTimeStamp + "'";

            ArrayList arr = m_dbMgr.GetResultData(strQuery, 0);
            if (arr == null || arr.Count == 0)
                return false;

            int nResult = Convert.ToInt32(arr[0]);
            if (nResult < 1)
                return false;
            else
                return true;
        }
    }
}
