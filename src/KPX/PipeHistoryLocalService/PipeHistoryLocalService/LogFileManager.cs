using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;
using System.IO;

namespace PipeHistoryLocalService
{
    public partial class LogFileManager : ServiceBase
    {
        private bool m_appClose = false;
        private static LogFileManager m_instance = null;
        private bool m_endThread = false;
        private System.IO.StreamWriter m_writer = null;
        private System.IO.StreamWriter m_writerError = null;

        public bool AppClose
        {
            get { return m_appClose; }
        }

        public static LogFileManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new LogFileManager();

                return m_instance;
            }
        }

        public bool EndThread
        {
            get { return m_endThread; }
            set { m_endThread = value; }
        }

        private LogFileManager()
        {
            InitializeComponent();

            string szPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string szFullPath = System.IO.Directory.GetParent(szPath).FullName;

            m_writer = new System.IO.StreamWriter(szFullPath + @"\log.txt", false, Encoding.UTF8);
            m_writerError = new System.IO.StreamWriter(szFullPath + @"\errlog.txt", true, Encoding.UTF8);
        }
        System.Threading.Thread t = null;

        System.Threading.Thread t2 = null;
        protected override void OnStart(string[] args)
        {
            if (t != null)
                return;

            m_appClose = false;

            t = new System.Threading.Thread(new System.Threading.ThreadStart(RealTimeThread));
            t.Start();
            System.Threading.Thread.Sleep(3000);
            
            t2 = new System.Threading.Thread(new System.Threading.ThreadStart(RealTimeThread2));
            t2.Start();
        }

        protected override void OnStop()
        {
            m_appClose = true;

            try
            {
                if( t!= null)
                {
                    t.Join();
                    t.Abort();
                }
            }
            catch(Exception)
            {

            }
            t = null;
            try
            {
                if (t2 != null)
                {
                    t2.Join();
                    t2.Abort();
                }
            }
            catch (Exception)
            {

            }

            t2 = null;
            m_writer.Close();
            m_writerError.Close();
        }

        private DateTime dtDelFileCheck1 = new DateTime();
        private DateTime dtDelFileCheck2 = new DateTime();

        private void RealTimeThread2()
        {
            WriteFlowHistory history = new WriteFlowHistory();
            bool isRealTime = false;

            while (m_appClose == false)
            {
                try
                {
                    if ((DateTime.Now - dtDelFileCheck2).Days > 1)
                    {
                        if (history.nTankIDs.Count == 0)
                            history.DisplayIDs();
                        dtDelFileCheck2 = DateTime.Now;
                        DeleteFile(history.nTankIDs, history.LogFolder);
                    }

                    history.Start(isRealTime);
                    isRealTime = true;

                    for (int i = 0; i < 50; i++)
                    {
                        if (m_appClose == true)
                            break;
                        System.Threading.Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    WriteErrorLog("Flow : " + ex.Message);
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }
         
        private void RealTimeThread()
        {
            WritePipeHistory history = new WritePipeHistory(); 
            bool isRealTime = false;

            while (m_appClose == false)
            {
                try
                {
                    if ((DateTime.Now - dtDelFileCheck1).Days > 1)
                    {
                        if (history.nPipeIDs.Count == 0)
                            history.DisplayIDs();
                        dtDelFileCheck1 = DateTime.Now;
                        DeleteFile(history.nPipeIDs, history.LogFolder);
                    }

                    history.Start(isRealTime);
                    isRealTime = true;

                    for (int i = 0; i < 50; i++)
                    {
                        if (m_appClose == true)
                            break;
                        System.Threading.Thread.Sleep(100);
                    }
                }
                catch (Exception ex)
                {
                    WriteErrorLog("Pipe : " + ex.Message);
                    System.Threading.Thread.Sleep(3000);
                }
            }
        }
        
#if !SERVICE
        public void Start()
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(RealTimeThread));
            t.Start();

            System.Threading.Thread t2 = new System.Threading.Thread(new System.Threading.ThreadStart(RealTimeThread2));
            t2.Start();
        }
#endif
        public void WriteLog(string strLog)
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);
            m_writer.WriteLine(strTime + ", " + strLog);
            m_writer.Flush();
        }
        public void WriteErrorLog(string strLog)
        {
            DateTime dtNow = DateTime.Now;
            m_writerError.WriteLine("[" + dtNow.ToString("yyyy-MM-dd HH:mm:ss") + "] " + strLog);
            m_writerError.Flush();
        }

        /// <summary>
        /// 1년간의 데이터만 보존한다.
        /// </summary>
        /// <param name="IDs">Tank, Pipe ID들</param>
        /// <param name="strLogFolder">데이터 파일 위치</param> 
        private void DeleteFile(List<int> IDs, string strLogFolder)
        { 
            DateTime delTime = DateTime.Now.AddYears(-1);

            try
            {
                foreach (int id in IDs)
                {
                    string dir = string.Format("{0}\\{1}", strLogFolder, id);
                    if (!Directory.Exists(dir))
                        continue;

                    // 1. 1년이 지난 Year 있는지 검사
                    string[] dirYears = Directory.GetDirectories(dir);
                    foreach (string dirYear in dirYears)
                    {
                        DirectoryInfo dirYearInfo = new DirectoryInfo(dirYear);

                        int nYear = -1;
                        if (!int.TryParse(dirYearInfo.Name, out nYear))
                            continue;

                        if (nYear < 0 || nYear > delTime.Year)
                            continue;

                        // 2. 1년이 지난 Month 있는지 검사
                        string[] dirMonths = Directory.GetDirectories(dirYear);
                        if (dirMonths.Length == 0)
                            Directory.Delete(dirYear, true);
                        else
                        {
                            foreach (string dirMonth in dirMonths)
                            {
                                DirectoryInfo dirMonthInfo = new DirectoryInfo(dirMonth);

                                int nMonth = -1;
                                if (!int.TryParse(dirMonthInfo.Name, out nMonth))
                                    continue;

                                // 3. 1년이 지난 Day 있는지 검사 
                                foreach (string dirDay in Directory.GetFiles(dirMonth))
                                {
                                    FileInfo fi = new FileInfo(dirDay);
                                    if (fi.Extension != ".dat")
                                        continue;

                                    string strDay = fi.Name.Replace(".dat", "").Replace("_temp", "");

                                    int nDay = -1;
                                    if (int.TryParse(strDay, out nDay))
                                    {
                                        DateTime dt = new DateTime(Convert.ToInt32(dirYearInfo.Name), nMonth, nDay);
                                        if (nMonth > 0 && dt <= delTime)
                                        {
                                            if (File.Exists(dirDay))
                                                File.Delete(dirDay);
                                        }
                                    }
                                }

                                //4. .dat파일 없으면 Directory 삭제 
                                DeleteDir(dirMonth, true);
                            }

                            DeleteDir(dirYear);
                        }
                    }
                    if (dirYears.Length == 0)
                        Directory.Delete(dir, true);
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("DeleteFile : " + ex.Message);
                System.Threading.Thread.Sleep(3000);
            }
        }
        private void DeleteDir(string strPath, bool isFileChk = false)
        {
            bool delDir = true;
            if (!Directory.Exists(strPath))
                return;

            if (isFileChk)
            {
                string[] dirDays = Directory.GetFiles(strPath);
                foreach (string dirDay in dirDays)
                {
                    FileInfo fi = new FileInfo(dirDay);
                    if (fi.Extension != ".dat")
                        continue;

                    string strDay2 = fi.Name.Replace(".dat", "").Replace("_temp", "");
                    int nDay2 = -1;
                    if (int.TryParse(strDay2, out nDay2))
                    {
                        delDir = false;
                        break;
                    }
                } 
            }
            else
            {
                string[] dirDays = Directory.GetDirectories(strPath);
                if (dirDays.Length > 0)
                    delDir = false; 
            }
            //4. .dat 파일 없으면 Directory 삭제
            if (delDir)
                Directory.Delete(strPath, true);
        }
    }
}
