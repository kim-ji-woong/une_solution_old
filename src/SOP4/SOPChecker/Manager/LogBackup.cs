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

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;


namespace SOPChecker
{
    public delegate void LogBackupCallback();

    public class LogBackup
    {
        private log4net.ILog logger = null;

        private int m_nSiteID = 1;
        public LogBackup()
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                m_nSiteID = FormMain.Instance.SiteID;
            });
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


        public void CompressFiles(string outPathname, ArrayList arFiles)
        {
            FileStream fsOut = File.Create(outPathname);
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);
            zipStream.SetLevel(3);

            CompressFolder(arFiles, zipStream);

            zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
            zipStream.Close();
        }

        private void CompressFolder(ArrayList files, ZipOutputStream zipStream)
        {
           
            foreach (string filename in files)
            {
                FileInfo fi = new FileInfo(filename);
                string entryName = Path.GetFileName(filename);
                //string entryName = filename.Substring((folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity

                newEntry.Size = fi.Length;

                zipStream.PutNextEntry(newEntry);
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }           
        }


        private bool CompressedFile(string szZipFileName, ArrayList arFileList)
        {
            ZipOutputStream strmZipOutputStream = new ZipOutputStream(File.Create(szZipFileName));
            try
            {
                // Compression Level: 0-9
                // 0: no(Compression)
                // 9: maximum compression
                strmZipOutputStream.SetLevel(9);
                try
                {
                    byte[] abyBuffer = new byte[4096];
                    foreach (string szFileName in arFileList)
                    {
                        if (!File.Exists(szFileName))
                        {
                            continue;
                        }

                        FileStream strmFile = File.OpenRead(szFileName);

                        try
                        {
                            ZipEntry objZipEntry = new ZipEntry(szFileName);
                            objZipEntry.DateTime = DateTime.Now;
                            objZipEntry.Size = strmFile.Length;

                            strmZipOutputStream.PutNextEntry(objZipEntry);
                            StreamUtils.Copy(strmFile, strmZipOutputStream, abyBuffer);
                        }
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            strmFile.Close();
                        }
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    strmZipOutputStream.Finish();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                strmZipOutputStream.Close();
            }

            return true;
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
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                Debug.WriteLine(ex.StackTrace);
                return;
            }

            DateTime dtNow = DateTime.Now;

            ArrayList arList = new ArrayList();

            string szServerPath = DBUtility.RegUtil.ReadRegValue("Logfile Path", "SOPServer", m_nSiteID);
            if( szServerPath == "")
            {
                szServerPath = GetProcessPath("SOPServer");
                if (szServerPath != "")
                {
                    szServerPath = Path.GetDirectoryName(szServerPath);
                    DBUtility.RegUtil.WriteRegValue("Logfile Path", "SOPServer", szServerPath, m_nSiteID);
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

            string szMonitorPath = DBUtility.RegUtil.ReadRegValue("Logfile Path", "SOPMonitor", m_nSiteID);
            if (szMonitorPath == "")
            {
                szMonitorPath = GetProcessPath("SensorMonitor");
                if (szMonitorPath != "")
                {
                    szMonitorPath = Path.GetDirectoryName(szMonitorPath);
                    DBUtility.RegUtil.WriteRegValue("Logfile Path", "SOPMonitor", szServerPath, m_nSiteID);
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
                CompressedFile(szZipFile, arList);
                //this.CompressFiles(szZipFile, arList);
                //Core.UZip.CompressFile(szZipFile, arList);
                //Core.UZip.CompressFile(szZipFile, arList);
            }
            catch (System.Exception ex)
            {
                logger.Debug(ex.Message);
                logger.Debug(ex.StackTrace);
                //return;
            }

            string szWebPath = DBUtility.RegUtil.ReadRegValue("Logfile Path", "upload", m_nSiteID);
            if (szWebPath == "")
            {
                szWebPath = GetProcessPath("Tomcat7");
                if (szWebPath != "")
                {
                    szWebPath = Path.GetDirectoryName(szWebPath);
                    szWebPath += "\\..\\webapps\\ROOT\\" + szLogFileName;
                    DBUtility.RegUtil.WriteRegValue("Logfile Path", "upload", szWebPath, m_nSiteID);
                }
            }
            if (szWebPath != "")
            {

                try
                {
                    File.Copy(szZipFile, szWebPath, true);
                }
                catch (System.Exception ex)
                {
                    logger.Debug(ex.Message);
                    logger.Debug(ex.StackTrace);
                    return;
                }
            }
            //m_bBackupComplete = true;
            if (callback != null)
                callback();
            return;
        }

        //private bool m_bBackupComplete = false;
    }
}
