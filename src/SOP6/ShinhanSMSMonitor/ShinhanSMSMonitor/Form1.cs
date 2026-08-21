using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShinhanSMSMonitor
{
    public partial class Form1 : Form
    {
        private DBUtility2.WebDBManager m_dbMgr = null;
        private int m_nSiteID = 200;
        private Timer m_timer = null;
        private DBUtility2.Utility m_util = new DBUtility2.Utility();

        private StreamWriter m_sw = null;

        public Form1()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.Hide();
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            m_sw = new StreamWriter(Application.StartupPath + "\\SMSMonitorLog.log", true);
        }

        private void WriteLog(string txt)
        {
            DateTime dt = DateTime.Now;
            m_sw.WriteLine("[" + dt.ToString("yyyy-MM-dd HH:mm:ss") + "] " + txt);
            m_sw.Flush();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool connect = InitDB();
            if (!connect)
            {
                MessageBox.Show("[유엔이] DB 연결 실패");
                WriteLog("[유엔이] DB 연결 실패");
                this.Close();
            }

            int nID = ReadLastID();
            m_nLastID = nID;

            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += m_timer_Tick;
            m_timer.Start();
        }
        private int m_nLastID = -1;
        void m_timer_Tick(object sender, EventArgs e)
        {
            string strNow = DateTime.Now.AddMinutes(-5).ToString("yyyy-MM-dd HH:mm:ss");

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT ID, Caller, PhoneNumber, SMSMessage, time ");
            sb.Append("  FROM SendSMS ");
            sb.AppendFormat(" WHERE time > '{0}'", strNow);
            sb.AppendFormat("   AND ID > {0}", m_nLastID);

            int maxId = m_nLastID;

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null)
            {
                WriteLog("[SQL ERROR] : " + sb.ToString());
                WriteLog("WebServerURL : " + m_dbMgr.WebServerURL + " DBName : " + m_dbMgr.DatabaseName);
                WriteLog("Message : " + m_dbMgr.LastErrorMessage);
                return;
            }

            for (int i = 0; i < arrResult.Count; i+=5)
            {
                int nID = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strCaller = DBUtility2.WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                string strPhoneNumber = DBUtility2.WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                string strMessage = DBUtility2.WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                string strDateTime = DBUtility2.WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");

                bool suc = SendSMS(strPhoneNumber, strCaller, strMessage);
                
                maxId = Math.Max(maxId, nID);
            }

            if (m_nLastID != maxId)
            {
                m_nLastID = maxId;
                m_util.setinivalue("Read Info", "last_id", m_nLastID.ToString());                
            }
        }

        private bool SendSMS(string phoneNumber, string caller, string message)
        {
            try
            {
                ProcessStartInfo pInfo = new ProcessStartInfo();
                pInfo.FileName = @"cmd";
                pInfo.CreateNoWindow = true;
                pInfo.RedirectStandardOutput = true;
                pInfo.RedirectStandardError = true;
                pInfo.RedirectStandardInput = true;
                pInfo.UseShellExecute = false;

                Process pro = new Process();
                pro.StartInfo = pInfo;
                pro.Start();

                pro.StandardInput.Write("D:" + Environment.NewLine);
                pro.StandardInput.Write("cd " + m_strBatPath + Environment.NewLine);
                pro.StandardInput.Write("sms.bat " + phoneNumber + " " + caller + " " + "\"" + message + "\"" + Environment.NewLine);

                WriteLog("[INFO] : " + m_strBatPath + " " + "sms.bat " + phoneNumber + " " + caller + " " + "\"" + message + "\"");
                pro.Close();

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                WriteLog("[ERROR] : " + ex.Message);
                return false;
            }

            return true;
        }

        private bool InitDB()
        {            
            string szWebServerURL = m_util.getinivalue("Server Connection Info", "webserver_url");
            
            string szDBName = m_util.getinivalue("Server Connection Info", "db_name");
            
            m_dbMgr = new WebDBManager();
            m_dbMgr.WebServerURL = szWebServerURL;
            m_dbMgr.DatabaseName = szDBName;
            m_dbMgr.DatabaseType = WebDBManager.DBType.sqlserver;

            if (m_dbMgr == null)
            {
                return false;
            }

            WriteLog(szWebServerURL + " / " + szDBName);

            ArrayList arrResult = m_dbMgr.GetResultData("select ID from site".ToString());
            if (arrResult == null)
            {
                WriteLog("[Test SQL ERROR] : " + m_dbMgr.LastErrorMessage);
            }
            else
            {
                if (arrResult.Count > 0)
                {
                    WriteLog("[Test SQL Result] : " + WebDBManager.GetIntField(arrResult[0].ToString(), -1));
                }
            }

            return true;
        }

        private int ReadLastID()
        {
            string strID = m_util.getinivalue("Read Info", "last_id");
            if (strID != null && strID.Length > 0)
            {
                int nID = 0;
                if (int.TryParse(strID, out nID))
                    return nID;
            }

            string strBatPath = m_util.getinivalue("Read Info", "bat_path");
            if (strBatPath != null && strBatPath.Length > 0)
            {
                m_strBatPath = strBatPath;
                WriteLog(m_strBatPath);
            }

            return -1;
        }
        private string m_strBatPath = "D:\\\"00 EIBS\"\\SMS\\smsAgent";

        bool isClose = false;
        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isClose = true;
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!isClose)
            {
                e.Cancel = true;
                this.notifyIcon1.Visible = true;
                this.Hide();
            }
        }
    }
}
