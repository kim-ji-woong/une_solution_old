using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;

namespace DIDViewerAutoUpdate
{
    public partial class Form1 : Form
    {
        private Timer m_timer = null;
        private WebDBManager m_dbMgr = null;
        private string m_strDidPcNo = "";
        private string m_strPath = "";
        private int m_nSiteID = 3;
        private string m_strWebServerUploadPath = "";
        private string m_strDownloadWebServerURL = "";

        public Form1()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.Hide();
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            Loadini();
            m_dbMgr = new WebDBManager(m_nSiteID);

            m_timer = new Timer();
            m_timer.Interval = 10000;
            m_timer.Tick += M_timer_Tick;
            m_timer.Start();
        }

        private void Loadini()
        {
            DBUtility2.Utility util = new DBUtility2.Utility();
            string siteID = util.getinivalue("Server Connection Info", "siteid");
            if (siteID != null && siteID.Length > 0)
            {
                int nSiteID;
                if (int.TryParse(siteID, out nSiteID))
                    m_nSiteID = nSiteID;
            }

            string didPcNo = util.getinivalue("Setting", "did_pc_no");
            if (didPcNo != null && didPcNo.Length > 0)
            {
                m_strDidPcNo = didPcNo;
            }

            string didViewerLocalPath = util.getinivalue("Setting", "didviewer_local_path");
            if (didViewerLocalPath != null && didViewerLocalPath.Length > 0)
            {
                m_strPath = didViewerLocalPath;
            }

            string uploadPath = util.getinivalue("Setting", "WebServerUploadPath");
            if (uploadPath != null && uploadPath.Length > 0)
            {
                m_strWebServerUploadPath = uploadPath;
            }

            string downurl = util.getinivalue("Server Connection Info", "DownloadWebServerURL");
            if (downurl != null && downurl.Length > 0)
            {
                m_strDownloadWebServerURL = downurl;
            }
        }

        private void M_timer_Tick(object sender, EventArgs e)
        {
            ArrayList arrResult = m_dbMgr.GetResultData("select id, filename from didautoupdate where did_pc_no = " + m_strDidPcNo);
            if (arrResult == null || arrResult.Count == 0)
                return;

            int nID = DBUtility2.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            string strFileNames = DBUtility2.WebDBManager.GetStringField(arrResult[1]);
            

            m_dbMgr.GetResultData("delete from didautoupdate where id = " + nID);

            ProcessKill();

            if (strFileNames != null && strFileNames.Length > 0)
            {
                string[] strFileName = strFileNames.Split(',');
                foreach (string item in strFileName)
                {
                    //WriteLog("[File Info] " + item);
                    Download(item);
                } 
            }

            ProcessStart();
        }

        private bool ProcessKill()
        {
            try
            {
                System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("DidViewer");
                if (process.Length > 0)
                {
                    process[0].Kill();

                    bool realKill = true;
                    int timerInterval = 0;

                    Timer timer = new Timer();
                    timer.Interval = 1000;
                    timer.Tick += (s, e) =>
                    {
                        timerInterval++;
                    };
                    timer.Start();

                    realKill = false;

                    while (!realKill)
                    {
                        System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName("DidViewer");
                        if (proc.Length == 0)
                            realKill = true;
                        if (timerInterval > 60) // 1분 대기
                            break;
                    }

                    timer.Stop();
                    timer = null;

                }
                return true;
            }
            catch (Exception ex)
            {
                //WriteLog("[Process Kill ERROR] " + ex.Message);
                return false;
            }
        }

        public bool Download(string fileName)
        {
            if (fileName.Length == 0)
                return false;

            string strErrorMsg = "";
            string webServerFilePath = m_strWebServerUploadPath + "\\" + fileName;
            string copyFilePath =m_strPath + "\\" + fileName;

            try
            {
                if (File.Exists(copyFilePath))
                    File.Delete(copyFilePath);

                DBUtility2.UpDownManager.DownloadFile(webServerFilePath, copyFilePath, m_strDownloadWebServerURL, out strErrorMsg);
                //DBUtility2.UpDownManager.DownloadFile(webServerFilePath, copyFilePath, "http://192.168.0.214", out strErrorMsg);
            }
            catch (Exception ex)
            {
                //WriteLog("[Download ERROR] " + ex.Message + "\r\n" + "webServerFilePath : " + webServerFilePath + "\r\n" + "copyFilePath : " + copyFilePath);
                return false;
            }

            return true;
        }

        private bool ProcessStart()
        {
            try
            {
                //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                //startInfo.FileName = "DidViewer.exe";
                //startInfo.WorkingDirectory = m_strPath;
                //startInfo.ErrorDialog = true;

                System.Diagnostics.Process.Start(m_strPath + "\\DidViewer.exe");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        bool isClose = false;
        private void 닫기ToolStripMenuItem_Click(object sender, EventArgs e)
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

        //private void WriteLog(string txt)
        //{
        //    using (StreamWriter sw = new StreamWriter(Application.StartupPath + "\\log.txt", true))
        //    {
        //        sw.WriteLine(txt);
        //    }
        //}
    }
}
