using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Management;
using System.Management.Instrumentation;

namespace SOPChecker
{
    public delegate void LogBackupCallback();

    public class LogBackup
    {
        public LogBackup()
        {
        }

        protected LogBackupCallback callback;
        public LogBackupCallback Callback
        {
            get { return callback; }
            set { callback = value; }
        }

        public string GetProcessPath(string szProcessName)
        {
            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string)mo["ExecutablePath"],
                                CommandLine = (string)mo["CommandLine"],
                            };
                foreach (var item in query)
                {
                    if (item.Process.ProcessName.ToLower() == szProcessName.ToLower())
                    {
                        return item.Path;
                    }
                }
            }
            return "";
        }

        public bool GatherServerLog()
        {
            Thread t = new Thread(GatherServerLogThread);
            t.Start();
            return true;
        }
        private bool IsPassedTime(DateTime dtNow, DateTime time)
        {
            DateTime dtFile = time;
            TimeSpan spant = dtNow - dtFile;
            if (spant.TotalDays < 7.0)
                return true;
            return false;  
        }
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            DateTime dtFile = new DateTime(nYear, nMonth, nDay);
            TimeSpan spant = dtNow - dtFile;
            if (spant.TotalDays < 7.0)
                return true;
            return false;            
        }

        public void GatherServerLogThread()
        {
            string szLogFileName = "server.log.zip";
            string szPath = System.IO.Path.GetTempPath();
            string szZipFile = szPath + "\\" + szLogFileName;

            try
            {
                if (File.Exists(szZipFile))
                {
                    File.Delete(szZipFile);
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                return;
            }

            DateTime dtNow = DateTime.Now;

            ArrayList arList = new ArrayList();

            string szServerPath = DBUtility.RegUtil.ReadRegValue("Logfile Path", "SOPServer");
            if( szServerPath == "")
            {
                szServerPath = GetProcessPath("SOPServer");
                if (szServerPath != "")
                {
                    szServerPath = Path.GetDirectoryName(szServerPath);
                    DBUtility.RegUtil.WriteRegValue("Logfile Path", "SOPServer", szServerPath);
                }
            }
            
            if (szServerPath != "")
            {                
                string[] arrFiles = Directory.GetFiles(szServerPath);

                string strKey = "sdmsserver.log";
                
                int len = strKey.Length + 1;
                foreach (string strFile in arrFiles)
                {
                    int nIndex = strFile.IndexOf(strKey);
                    if (nIndex < 0)
                    {                        
                        continue;
                    }

                    if (Path.GetFileName(strFile) == strKey)
                    {
                        arList.Add(strFile);
                        continue;
                    }

                    int nSubIndex = nIndex + len;
                    if (nSubIndex > 0 && strFile.Length > nSubIndex)
                    {
                        string strDate = strFile.Substring(nIndex + len);                       
                        DateTime dt = Convert.ToDateTime(strDate); 
                        if (IsPassedTime(dtNow, dt))
                            arList.Add(strFile);
                    }
                }                
            }

            string szMonitorPath = DBUtility.RegUtil.ReadRegValue("Logfile Path", "SOPMonitor");
            if (szMonitorPath == "")
            {
                szMonitorPath = GetProcessPath("SensorMonitor");
                if (szMonitorPath != "")
                {
                    szMonitorPath = Path.GetDirectoryName(szMonitorPath);
                    DBUtility.RegUtil.WriteRegValue("Logfile Path", "SOPMonitor", szServerPath);
                }
            }
           
            if (szMonitorPath != "")
            {
                string strKey2 = "SensorMonitor.log";
                
                string[] arrFiles = Directory.GetFiles(szMonitorPath);

                int len = strKey2.Length + 1;
                foreach (string strFile in arrFiles)
                {
                    int nIndex = strFile.IndexOf(strKey2);
                    if (nIndex < 0)
                    {
                        continue;
                    }
                    if (Path.GetFileName(strFile) == strKey2)
                    {
                        arList.Add(strFile);
                        continue;
                    }
                    int nSubIndex = nIndex + len;
                    if (nSubIndex > 0 && strFile.Length > nSubIndex)
                    {
                        string strDate = strFile.Substring(nIndex + len);
                        DateTime dt = Convert.ToDateTime(strDate);
                        if (IsPassedTime(dtNow, dt))
                            arList.Add(strFile);
                    }
                }  
            }

            try
            {
                Core.UZip.CompressFile(szZipFile, arList);
            }
            catch (System.Exception)
            {
                //return;
            }

            string szWebPath = DBUtility.RegUtil.ReadRegValue("Logfile Path", "upload");
            if (szWebPath == "")
            {
                szWebPath = GetProcessPath("Tomcat7");
                if (szWebPath != "")
                {
                    szWebPath = Path.GetDirectoryName(szWebPath);
                    szWebPath += "\\..\\webapps\\ROOT\\" + szLogFileName;
                    DBUtility.RegUtil.WriteRegValue("Logfile Path", "upload", szWebPath);
                }
            }
            if (szWebPath != "")
            {

                try
                {
                    File.Copy(szZipFile, szWebPath, true);
                }
                catch (System.Exception)
                {
                    return;
                }
            }
            m_bBackupComplete = true;
            if (callback != null)
                callback();
            return;
        }

        private bool m_bBackupComplete = false;
    }
}
