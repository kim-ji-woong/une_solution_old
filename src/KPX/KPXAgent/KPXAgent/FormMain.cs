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
using System.IO;

namespace KPXAgent
{
    public partial class FormMain : Form
    {
        private static FormMain m_instance = null;

        private WebDBManager m_dbMgr = new WebDBManager(500);
        private string m_strWebServerRootURL = "";

        public string WebServerRootURL
        {
            get { return m_strWebServerRootURL; }
        }

        public static FormMain Instance
        {
            get { return m_instance; }
        }
        private string m_strDownloadURL = "";
        public string downloadURL
        {
            get { return m_strDownloadURL; }
        }
        // 사무실 0, 휴게실 1
        public int AreaType = 0;

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
            AreaType = KPXAgent.Properties.Settings.Default.AreaType;

            this.WindowState = FormWindowState.Minimized; 
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.Hide();
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
             
            // DB
            //m_dbMgr.WebServerURL = "http://183.104.147.144:18080/SOP";
            m_dbMgr.WebServerURL = "http://127.0.0.1:8080/SOP"; //test할때

            if (AreaType == 0)
                m_dbMgr.DatabaseHost = "127.0.0.1";
            else if (AreaType == 1)
                m_dbMgr.DatabaseHost = "195.1.1.63";

            m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;
            m_dbMgr.DatabaseName = "KPX";

            if (System.Windows.Forms.Application.ExecutablePath.Contains("_temp")) 
                this.notifyIcon1.Text += " temp";  

            SetLog("KPXAgent Start WebServerURL : " + m_dbMgr.WebServerURL + " / DatabaseHost : " + m_dbMgr.DatabaseHost);

            SetWebServerURL();
            timer1.Start();
        } 

        private void SetWebServerURL()
        {
            int nIndex = m_dbMgr.WebServerURL.LastIndexOf('/');

            if (nIndex < 0)
                return;

            m_strWebServerRootURL = m_dbMgr.WebServerURL.Substring(0, nIndex + 1);
            m_strDownloadURL = "http://unes.iptime.org:10091/SOP";
        }

        bool isClose = false;
        private void tsMenuClose_Click(object sender, EventArgs e)
        {
            isClose = true;
            this.Close();
        }

        private void OnTimer(object sender, EventArgs e)
        {
            ReadCommand();
        }

        private void ReadCommand()
        {            
            string strSQL = "Select ID, Command, TimeStamp, FileName from AgentCommand where areatype = " + this.AreaType; 
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
            {
                SetLog("arrResult null");
                return;
            }

            bool needClose = false;
            int nResultCount = arrResult.Count;

            for (int i = 0;i < nResultCount - 2; i += 4)
            {
                SetLog("ReadCommand ResultCount : " + nResultCount);
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> command = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                string fileName = WebDBManager.GetStringField(arrResult[i + 3], "");

                if (id == null || command == null || timeStamp == null)
                    continue;

                if (command.Data == (int)Command.CommandType.JSP_FILE_UPDATE || command.Data == (int)Command.CommandType.SERVER_DLL_UPDATE)
                {
                    if (fileName.Length == 0 || fileName == "null") continue;
                } 

                Command cmd = new Command(command.Data);
                
                if (cmd.Execute(m_dbMgr, id.Data, fileName))
                {
                    // Agent 업데이트 할때만 
                    if (cmd.NeedClose)
                    { 
                        needClose = true;
                        break; 
                    }
                }
            }

            if (needClose)
            {
                timer1.Stop();
                this.Close();

                SetLog("Agent Reboot");
            }
        }

        public static void SetLog(string content)
        {
            string filePath = @"D:\Tomcat 7.0\webapps\ROOT\SOP\KPX\KpxAgent.log";
            string dirPath = @"D:\Tomcat 7.0\webapps\ROOT\SOP\KPX";

            //string filePath = @"C:\Program Files\Apache Software Foundation\Tomcat 7.0\webapps\ROOT\SOP\KPX\KpxAgent.log";
            //string dirPath = @"C:\Program Files\Apache Software Foundation\Tomcat 7.0\webapps\ROOT\SOP\KPX";

            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileInfo fi = new FileInfo(filePath);

            try
            {
                if (!di.Exists) Directory.CreateDirectory(dirPath);
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(filePath))
                    {
                        sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filePath))
                    {
                        sw.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]    " + content);
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
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
