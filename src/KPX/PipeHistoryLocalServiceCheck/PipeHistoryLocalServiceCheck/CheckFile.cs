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
using SOPChecker;

namespace PipeHistoryLocalServiceCheck
{
    public partial class CheckFile : Form
    {
        private DBUtility.WebDBManager m_dbMgr;

        private List<string> m_pipeIDs = null;
        private List<string> m_tankIDs = null;

        private Timer m_timer = null;

        private string m_strLogFolder = "";
        private string m_strPipePath = "";
        private string m_strFlowPath = "";

        public CheckFile()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon1.Visible = true;
            this.Hide();
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            m_dbMgr = new DBUtility.WebDBManager(500);
            m_dbMgr.DatabaseHost = "127.0.0.1";

            m_strLogFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            
            m_strPipePath = m_strLogFolder + "\\UNE\\KPX\\work\\";
            m_strFlowPath = m_strLogFolder + "\\UNE\\KPX\\flow\\";
        }
        
        private void CheckFile_Load(object sender, EventArgs e)
        {
            LoadIDs();

            m_timer = new Timer();
            m_timer.Interval = 60000;
            m_timer.Tick += M_timer_Tick;
            m_timer.Start();

            WriteLog("감시 시작");

            M_timer_Tick(null, null);
        }

        private void M_timer_Tick(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(DateTime.Now);
            bool bRestart = FileCheck();
            if (bRestart)
            {
                if (ServiceManager.IsRunningSerivce("PipeHistoryLocalService"))
                {
                    WriteLog("PipeHistoryLocalService 서비스 재시작");
                    ServiceManager.RestartService("PipeHistoryLocalService", 5000);
                }
                else
                {
                    WriteLog("PipeHistoryLocalService 서비스 시작");
                    ServiceManager.StartService("PipeHistoryLocalService", 5000);
                }
            }
        }

        private void LoadIDs()
        {
            ArrayList arrResult = m_dbMgr.GetResultData("select id from pipe", 0);
            if (arrResult != null && arrResult.Count > 0)
            {
                m_pipeIDs = new List<string>();
                for (int i = 0; i < arrResult.Count; i++)
                {
                    m_pipeIDs.Add(arrResult[i].ToString());
                }
            }

            arrResult = m_dbMgr.GetResultData("select id from tank", 0);
            if (arrResult != null && arrResult.Count > 0)
            {
                m_tankIDs = new List<string>();
                for (int i = 0; i < arrResult.Count; i++)
                {
                    m_tankIDs.Add(arrResult[i].ToString());
                }
            }
        }

        /// <summary>
        /// true : 서비스 재시작
        /// </summary>
        /// <returns></returns>
        private bool FileCheck()
        {
            try
            {
                DateTime now = DateTime.Now;
                //DateTime now = new DateTime(2019, 11, 04, 13, 36, 3);

                if (m_pipeIDs != null && m_pipeIDs.Count > 0)
                {
                    for (int i = 0; i < m_pipeIDs.Count; i++)
                    {
                        string id = m_pipeIDs[i];
                        string file = m_strPipePath + string.Format("{0}\\{1}\\{2}\\{3}.dat", id, now.Year, now.Month, now.Day);
                        if (!File.Exists(file))
                        {
                            // 24시가 지나서 새로운 파일을 쓰는 시점이라면 하루 전 파일이 써진 시간과 비교한다
                            if (now.Hour == 0 && now.Minute < 2)
                            {
                                now = now.AddDays(-1);
                                file = m_strPipePath + string.Format("{0}\\{1}\\{2}\\{3}.dat", id, now.Year, now.Month, now.Day);

                                if (!File.Exists(file)) // 하루 전 파일도 없으면 서비스 재시작 한다
                                {
                                    WriteLog("1. no file name : " + file + "/ now time : " + now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    return true;
                                }
                            }
                        }

                        DateTime accessTime = File.GetLastAccessTime(file);
                        if (now > accessTime && (now - accessTime).Minutes >= 5) // 마지막 파일이 Access 시간이 5분 지났으면 서비스를 재시작한다
                        {
                            WriteLog("2. no file name : " + file + "/ now time : " + now.ToString("yyyy-MM-dd HH:mm:ss") + "/ accessTime time : " + accessTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            return true;
                        }
                    }
                }

                if (m_tankIDs != null && m_tankIDs.Count > 0)
                {
                    for (int i = 0; i < m_tankIDs.Count; i++)
                    {
                        string id = m_tankIDs[i];
                        string file = m_strFlowPath + string.Format("{0}\\{1}\\{2}\\{3}.dat", id, now.Year, now.Month, now.Day);
                        if (!File.Exists(file))
                        {
                            // 24시가 지나서 새로운 파일을 쓰는 시점이라면 하루 전 파일이 써진 시간과 비교한다
                            if (now.Hour == 0 && now.Minute < 2)
                            {
                                now = now.AddDays(-1);
                                file = m_strPipePath + string.Format("{0}\\{1}\\{2}\\{3}.dat", id, now.Year, now.Month, now.Day);

                                if (!File.Exists(file)) // 하루 전 파일도 없으면 서비스 재시작 한다
                                {
                                    WriteLog("1. no file name : " + file + "/ now time : " + now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    return true;
                                }
                            }
                        }

                        DateTime accessTime = File.GetLastAccessTime(file);
                        if (now > accessTime && (now - accessTime).Minutes >= 5) // 마지막 파일이 Access 시간이 5분 지났으면 서비스를 재시작한다
                        {
                            WriteLog("2. no file name : " + file + "/ now time : " + now.ToString("yyyy-MM-dd HH:mm:ss") + "/ accessTime time : " + accessTime.ToString("yyyy-MM-dd HH:mm:ss"));
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
            }

            return false;
        }

        private void 종료ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_timer != null)
            {
                WriteLog("감시 종료");

                m_timer.Stop();
                m_timer.Dispose();
                m_timer = null;

                this.Close();
            }
        }

        public static void WriteLog(string text)
        {
            string logPath = Application.StartupPath;
            DateTime now = DateTime.Now;

            using (StreamWriter sw = new StreamWriter(logPath + "\\ServiceCheck.log", true))
            {
                sw.WriteLine("[" + now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + text);
            }
        }
    }
}
