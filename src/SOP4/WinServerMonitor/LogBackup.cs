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

namespace ServerMonitor
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

        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            DateTime dtFile = new DateTime(nYear, nMonth, nDay);
            TimeSpan spant = dtNow - dtFile;
            if (spant.TotalDays < 7.0)
                return true;
            return false;
        }

        private string m_szSDMSPath = "";
        private bool FindSDMS()
        {
            try
            {
                string[] filePaths = Directory.GetFiles("c:\\UNE\\", "SOPSimulator.exe", SearchOption.AllDirectories);

                if (filePaths == null || filePaths.Length == 0)
                {
                    try
                    {
                        filePaths = Directory.GetFiles("c:\\", "SOPSimulator.exe", SearchOption.AllDirectories);
                        if (filePaths == null || filePaths.Length == 0)
                        {
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                m_szSDMSPath = Path.GetDirectoryName(filePaths[0]);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        public void GatherServerLogThread()
        {
            string szLogFileName = "client.log.zip";
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
                
                FormMain.Instance.CompleteClientLog = false;
                return;
            }

            DateTime dtNow = DateTime.Now;
            ArrayList arList = new ArrayList();

            string szServerPath = GetProcessPath("SDMS");
            if (szServerPath == "")
            {
                FindSDMS();
                szServerPath = m_szSDMSPath;
            }
            else
            {
                szServerPath = Path.GetDirectoryName(szServerPath);
            }

            if (szServerPath != "")
            {
                szServerPath += "\\logs";
                string[] arrFiles = Directory.GetFiles(szServerPath);

                string strKey = "SDMSClient.log";
                string strKey2 = "SDMSClient_1.log";

                int len = strKey.Length + 1;
                int len2 = strKey2.Length + 1;
                int nCurLen = len;
                int nYear, nMonth, nDay;
                foreach (string strFile in arrFiles)
                {
                    string strCurKey = strKey;
                    int nIndex = strFile.IndexOf(strKey);
                    if (nIndex < 0)
                    {
                        nIndex = strFile.IndexOf(strKey2);
                        if (nIndex < 0)
                        {
                            continue;
                        }
                        else
                        {
                            nCurLen = len2;
                            strCurKey = strKey2;
                        }

                    }

                    if (Path.GetFileName(strFile) == strCurKey)
                    {
                        arList.Add(strFile);
                        continue;
                    }
                    string strDate = strFile.Substring(nIndex + nCurLen);
                    //string strDate = Path.GetFileName(strFile).Substring(len);

                    int nIndex1 = strDate.IndexOf('-');
                    int nIndex2 = strDate.LastIndexOf('-');

                    if (nIndex1 < 0 || nIndex2 < 0 || nIndex1 == nIndex2)
                        continue;

                    string strYear = strDate.Substring(0, nIndex1);
                    string strMonth = strDate.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                    string strDay = strDate.Substring(nIndex2 + 1);

                    if (!int.TryParse(strYear, out nYear))
                        continue;
                    if (!int.TryParse(strMonth, out nMonth))
                        continue;
                    if (!int.TryParse(strDay, out nDay))
                        continue;

                    if (IsPassedTime(dtNow, nYear, nMonth, nDay))
                        arList.Add(strFile);
                }
            }
            else
            {
                FormMain.Instance.CompleteClientLog = false;
               // r/eturn;
            }

            try
            {
                CompressFiles(szZipFile, arList);
            }
            catch (System.Exception)
            {
                FormMain.Instance.CompleteClientLog = false;
                //return;
            }

            //string szDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string szDir = FormMain.Instance.GetBackupLogFolder();
            
            if( szDir != "")
            {
                DateTime dt = DateTime.Now;
                string szDT = string.Format("{0}_{1:00}_{2:00}", dt.Year, dt.Month, dt.Day);
                string szFileName = szDir + "\\client.log"+ szDT + ".zip";
                try
                {
                    File.Copy(szZipFile, szFileName, true);
                }
                catch (System.Exception)
                {
                    FormMain.Instance.CompleteClientLog = false;
                    return;
                }
            }

            m_bBackupComplete = true;
            if (callback != null)
                callback();

            FormMain.Instance.CompleteClientLog = true;
            return;
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

        private bool m_bBackupComplete = false;
    }



}
