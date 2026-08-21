using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using DBUtility;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System.Diagnostics;
using SOPChecker;

namespace SDMSAgent
{
    public class CommandHandling
    { 
        WebDBManager m_dbMgr = null;

        private bool m_isNeedClose = false;
        public bool IsNeedClose
        {
            get { return m_isNeedClose; }
        }

        private Timer timer = null;
        private int timerInterval = 0;

        public CommandHandling(WebDBManager dbMgr)
        { 
            m_dbMgr = dbMgr;
             
        } 
        
        public bool Execute(CommandItem commandItem)
        {
            RemoveCommand(commandItem);

            try
            {
                CommandType type = commandItem.CmdType;

                if (type == CommandType.AGENT_UPDATE)
                {
                    return UpdateNReboot(commandItem);
                }
                else if (type == CommandType.GET_SERVICE_LIST)
                {
                    GetServiceList();
                }
                else if (type == CommandType.GET_PROC_LIST)
                {
                    GetProcList(commandItem.SearchPath);
                }
                else if (type == CommandType.GET_ALL_PROC_LIST)
                {
                    GetProcList();
                }
                else if (type == CommandType.GET_FILE_LIST)
                {
                    GetFileList(commandItem.SearchPath);
                }
                else if (type == CommandType.UPDATE)
                {
                    if (commandItem.IsStop)
                    {
                        if (commandItem.IsStopService)
                            ServiceStop(commandItem.StopName);
                        else
                            ProcessKill(commandItem.StopName);
                    }

                    bool realKill = true;
                    if (commandItem.IsUpdate)
                    {
                        if (commandItem.IsStop)
                        {
                            timer = new Timer(1000);
                            timer.Elapsed += (s, e) =>
                                {
                                    timerInterval++;
                                };
                            timer.Start();

                            realKill = false;

                            while (!realKill)
                            {
                                System.Diagnostics.Process[] proc = System.Diagnostics.Process.GetProcessesByName(commandItem.StopName);
                                if (proc.Length == 0)
                                    realKill = true;
                                if (timerInterval > 10)
                                    break;
                            }

                            timer.Stop();
                            timer = null;
                        }

                        if (realKill)
                            UploadFile2(commandItem.UpdateName);
                        else
                        {
                            FormMain.WriteLog("[ERROR] 프로세스가 종료되지 않아 UPDATE 실패");
                        }
                    }
                    if (commandItem.IsStart && realKill)
                    {
                        if (commandItem.IsStartService)
                            ServiceStart(commandItem.StartName);
                        else
                            ProcessStart(commandItem.StartName);
                    }
                }
                else if (type == CommandType.DOWNLOAD)
                {
                    Download(commandItem.SearchPath); 
                }
                else if (type == CommandType.SDMS_UPDATE)
                {
                    if (FormMain.Instance.UpdateSrcPath.Length > 0 && commandItem.UpdateName.Length > 0)
                    {
                        File.Copy(FormMain.Instance.UpdateSrcPath + "\\" + commandItem.UpdateName, FormMain.Instance.SdmsUpdateSrc + "\\" + commandItem.UpdateName, true);
                        File.Delete(FormMain.Instance.UpdateSrcPath + "\\" + commandItem.UpdateName);

                        SDMS.UpdateManager.Instance.CheckUpdate();                        
                    }
                }
                else if (type == CommandType.SOP_SERVER_RESTART)
                {
                    if (commandItem.IsStartService)
                    {
                        if (!SOPChecker.ServiceManager.IsRunningSerivce(commandItem.StopName))
                            ServiceStart(commandItem.StartName);
                        else
                            RestartService(commandItem.StartName, 5000);
                    }
                }
                else if (type == CommandType.FILE_COPY)
                {
                    FileCopyCommand(commandItem);
                }
                
                if (type != CommandType.AGENT_UPDATE)
                    m_isNeedClose = false;

                commandItem.Result = 1;

                if (type == CommandType.SERVER_STATUS)
                {
                    commandItem.Result = 0;
                    GetServerStatus(commandItem);
                }
            }
            catch (Exception ex)
            {
                FormMain.WriteLog("[ERROR] " + ex.Message); 
            }
            finally
            {
                InsertCommandHistory(commandItem); 
            }

            return true;
        } 

        private bool InsertCommandHistory(CommandItem commandItem)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("INSERT INTO SDMSCommandHistory (ID, Command, TimeStamp, ExecuteTime, SearchPath, IsStop, IsStopService, StopName, IsUpdate, UpdateName, IsStart, IsStartService, StartName, Result) ");
                sb.AppendFormat("           VALUES ((select isnull(max(id)+1,1) from SDMSCommandHistory), {0}, '{1}', '{2}', '{3}', {4}, {5}, '{6}', {7}, '{8}', {9}, {10}, '{11}', {12})"
                    , (int)commandItem.CmdType
                    , commandItem.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    , commandItem.SearchPath
                    , (commandItem.IsStop) ? 1 : 0, (commandItem.IsStopService) ? 1 : 0, commandItem.StopName
                    , (commandItem.IsUpdate) ? 1 : 0, commandItem.UpdateName
                    , (commandItem.IsStart) ? 1 : 0, (commandItem.IsStartService) ? 1 : 0, commandItem.StartName
                    , commandItem.Result);

                m_dbMgr.GetResultData(sb.ToString(), 0);
                 
                return true;
            }
            catch (Exception ex)
            {
                FormMain.WriteLog("[ERROR] InsertCommandHistory : " + ex.Message);
                return false;
            }
        }
        private bool RemoveCommand(CommandItem commandItem)
        {
            try
            {
                string strSQL = "DELETE FROM SDMSCommand where ID = " + commandItem.ID;
                m_dbMgr.GetResultData(strSQL, 0);
                 
                return true;
            }
            catch (Exception ex)
            {
                FormMain.WriteLog("[ERROR] RemoveCommand : " + ex.Message);
                return false;
            }
        }

        private bool GetFolderNFile(string strSrc, ref string strFolderPath, ref string strFileName)
        {
            int nIndex = strSrc.IndexOf('?');

            if (nIndex < 0)
                return false;

            strFolderPath = strSrc.Substring(0, nIndex);
            strFileName = strSrc.Substring(nIndex + 1);
            return true;
        }
        private bool GetFolderNFile2(string strSrc, ref string strFolderPath, ref string strFileName)
        {
            int nIndex = strSrc.LastIndexOf("\\");

            if (nIndex < 0)
                return false;

            strFolderPath = strSrc.Substring(0, nIndex);
            strFileName = strSrc.Substring(nIndex + 1);
            return true;
        }

        #region 프로세스, 서비스 시작/중지
        private bool ProcessKill(string strProcName)
        {
            if (strProcName.Contains(@"\"))
            {
                string strFolderPath = "", strFileName2 = "";
                if (GetFolderNFile2(strProcName, ref strFolderPath, ref strFileName2))
                {
                    strProcName = strFileName2.Replace(".exe", "");
                }
            }

            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(strProcName);
            if (process.Length > 0)
            {
                process[0].Kill();
                FormMain.WriteLog("[INFO] Kill " + strProcName);

                return true;
            }

            return false;
        }
        private bool ProcessStart(string strProcName)
        {
            string strFolderPath = "", strFileName = "";
            if (GetFolderNFile2(strProcName, ref strFolderPath, ref strFileName))
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strFileName;
                startInfo.WorkingDirectory = strFolderPath;
                startInfo.ErrorDialog = true;

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    FormMain.WriteLog("[INFO] Process Start : " + strFolderPath + strFileName);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    FormMain.WriteLog("[ERROR] Process Start : " + ex.Message);
                    return false;
                } 

                return true;
            }
            else
            {
                FormMain.WriteLog("[ERROR] Process Start : 경로 확인");
                return false;
            }
        }
        private bool ServiceStop(string strServiceName)
        {
            if (strServiceName.Contains(@"\"))
            {
                string strFolderPath = "", strFileName2 = "";
                if (GetFolderNFile2(strServiceName, ref strFolderPath, ref strFileName2))
                {
                    strServiceName = strFileName2.Replace(".exe", "");
                }
            }
            
            if (SOPChecker.ServiceManager.IsRunningSerivce(strServiceName))
            {
                SOPChecker.ServiceManager.StopService(strServiceName, 5000);
                FormMain.WriteLog("[INFO] Service Stop : " + strServiceName);
            }

            return true;
        }

        // 서비스가 실제로 종료되었는지 확인한다.
        // timeout : milli seconds
        private bool CheckStopService(string strServiceName, int timeout)
        {
            int nSleep = 200;

            for (int i = 0; i < timeout; i += nSleep)
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(strServiceName);

                if (SOPChecker.ServiceManager.IsRunningSerivce(strServiceName))
                {
                    service.Dispose();
                    return true;
                }

                service.Dispose();
                System.Threading.Thread.Sleep(nSleep);
            }

            return false;
        }

        private bool ServiceStart(string strServiceName)
        {
            try
            {
                if (strServiceName.Contains(@"\"))
                {
                    string strFolderPath = "", strFileName2 = "";
                    if (GetFolderNFile2(strServiceName, ref strFolderPath, ref strFileName2))
                    {
                        strServiceName = strFileName2.Replace(".exe", "");
                    }
                }

                SOPChecker.ServiceManager.StartService(strServiceName, 1000);
                FormMain.WriteLog("[INFO] Service Start : " + strServiceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                FormMain.WriteLog("[ERROR] Service Start : " + ex.Message);
                return false;
            }

            return true;
        }
        #endregion

        #region Agent Update
        private bool UpdateNReboot(CommandItem commandItem)
        {
            string strFileName = "", strFolder = "";
            if (UploadSelf2(ref strFileName, ref strFolder) == false)
                return false;
            FormMain.WriteLog("[INFO] UpdateNReboot : Agent Download OK");
             
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = strFileName;
            startInfo.WorkingDirectory = strFolder;
            startInfo.ErrorDialog = true;
            startInfo.Arguments = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

            try
            {
                System.Diagnostics.Process.Start(startInfo);
                FormMain.WriteLog("[INFO] UpdateNReboot : Agent Start ok");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                FormMain.WriteLog("[ERROR] UpdateNReboot : " + ex.Message);
                return false;
            }

            m_isNeedClose = true;
            return true;  
        }
        private bool UploadSelf2(ref string strDownloadFileName, ref string strDownloadFolderPath)
        {
            string strDownloadURL = FormMain.Instance.UpdateSrcPath;

            if (strDownloadURL.Length == 0)
                return false;

            string strPath = System.Windows.Forms.Application.ExecutablePath;

            int nIndex = strPath.LastIndexOf('\\');
            string strFolder = strPath.Substring(0, nIndex + 1);

            int nIndex2 = strPath.LastIndexOf('.');
            string strLocalFileName = strPath.Substring(nIndex + 1, nIndex2 - nIndex - 1) + "_temp.exe";// +strPath.Substring(nIndex2);
            string strFilePath = strFolder + strLocalFileName;

            strDownloadURL += "\\" + strPath.Substring(nIndex + 1);
            strDownloadURL = strDownloadURL.Replace(".EXE", ".exe");

            strDownloadFileName = strLocalFileName;
            strDownloadFolderPath = strFolder;

            try
            {
                if (System.IO.File.Exists(strFilePath))
                {
                    FormMain.WriteLog(strFilePath + " 삭제함");
                    System.IO.File.Delete(strFilePath);
                }

                FormMain.WriteLog("File.Copy : " + strDownloadURL + ", " + strFilePath);
                File.Copy(strDownloadURL, strFilePath);
                 
                FormMain.WriteLog("[INFO] UploadSelf : DownloadURL / " + strDownloadURL + " , strFilePath / " + strFilePath);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                FormMain.WriteLog("[ERROR] DownloadSelf : " + e.Message);
                return false;
            }

            return true;
        }
        private bool UploadSelf(ref string strDownloadFileName, ref string strDownloadFolderPath)
        {
            string strDownloadURL = FormMain.Instance.UpdateSrcPath;

            if (strDownloadURL.Length == 0)
                return false;
             
            string strPath = System.Windows.Forms.Application.ExecutablePath;

            int nIndex = strPath.LastIndexOf('\\');
            string strFolder = strPath.Substring(0, nIndex + 1);

            int nIndex2 = strPath.LastIndexOf('.');
            string strLocalFileName = strPath.Substring(nIndex + 1, nIndex2 - nIndex - 1) + "_temp.exe";// +strPath.Substring(nIndex2);
            string strFilePath = strFolder + strLocalFileName;

            strDownloadURL += "/" + strPath.Substring(nIndex + 1);
            strDownloadURL = strDownloadURL.Replace(".EXE", ".exe");

            strDownloadFileName = strLocalFileName;
            strDownloadFolderPath = strFolder;

            try
            {
                if (System.IO.File.Exists(strFilePath))
                    System.IO.File.Delete(strFilePath);

                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(strDownloadURL);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");
                 
                web.DownloadFile(strDownloadURL, strFilePath);
                
                FormMain.WriteLog("[INFO] UploadSelf : DownloadURL / " + strDownloadURL + " , strFilePath / " + strFilePath);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                FormMain.WriteLog("[ERROR] DownloadSelf : " + e.Message);
                return false;
            }

            return true;
        }
        #endregion
                
        /// <summary>
        /// 클라이언트 -> 서버로 파일 보내기
        /// </summary> 
        private void UploadFile2(string updateName)
        {
            try
            {
                int nIndex = updateName.LastIndexOf('\\');
                string strFolder = updateName.Substring(0, nIndex + 1);

                string strFileName = updateName.Substring(nIndex + 1);

                // 1. upload할 파일이 압축파일인지 확인 

                //
                bool isZipFile = false;
                int nLastCommaIndex = strFileName.LastIndexOf('.');
                if (nLastCommaIndex >= 0)
                {
                    if (strFileName.Substring(nLastCommaIndex + 1) == "zip")
                        isZipFile = true;
                }

                // 압축파일일때 UpdateSrcPath 폴더로 upload
                if (isZipFile)
                {
                    string updateSrcPath = FormMain.Instance.UpdateSrcPath + "\\" + strFileName;

                    // 압축풀어서 복사
                    CheckUpdate(strFolder);

                    FormMain.WriteLog("[INFO] UploadFile - Zip File Upload OK : " + updateName);
                }
                else
                {
                    if (System.IO.File.Exists(updateName))
                    {
                        System.IO.FileInfo file = new FileInfo(updateName);
                        file.IsReadOnly = false;
                        System.IO.File.Delete(updateName);
                    }

                    File.Copy(FormMain.Instance.UpdateSrcPath + "\\" + strFileName, updateName, true);

                    FormMain.WriteLog("[INFO] UploadFile - File Upload OK : " + updateName);
                }

                File.Delete(FormMain.Instance.UpdateSrcPath + "\\" + strFileName);
            }
            catch (Exception ex)
            {
                FormMain.WriteLog("[ERROR] UploadFile2 : " + ex.Message);
            }

            // 4. update_src 폴더에 upload할 파일 삭제
        }

        #region MyRegion
        //private bool UploadFile(string strExcutePath)
        //{
        //    string strDownloadURL = FormMain.Instance.DownloadURL;

        //    if (strDownloadURL.Length == 0)
        //        return false;

        //    int nIndex = strExcutePath.LastIndexOf('\\');
        //    string strFolder = strExcutePath.Substring(0, nIndex + 1);

        //    string strLocalFileName = strExcutePath.Substring(nIndex + 1);
        //    string strFilePath = strFolder + strLocalFileName;

        //    strDownloadURL += "/" + strExcutePath.Substring(nIndex + 1);
        //    strDownloadURL = strDownloadURL.Replace(".EXE", ".exe");

        //    try
        //    {
        //        System.Net.WebClient web = new System.Net.WebClient();

        //        Uri uri = new Uri(strDownloadURL);

        //        CredentialCache credentials = new CredentialCache();
        //        NetworkCredential netCredential = new NetworkCredential("sop", "sop");
        //        credentials.Add(uri, "Basic", netCredential);
        //        web.Credentials = new NetworkCredential("sop", "sop");

        //        bool isZipFile = false;
        //        int nLastCommaIndex = strLocalFileName.LastIndexOf('.');
        //        if (nLastCommaIndex >= 0)
        //        {
        //            if (strLocalFileName.Substring(nLastCommaIndex + 1) == "zip")
        //                isZipFile = true;
        //        }

        //        // 압축파일일때 UpdateSrcPath 폴더로 upload
        //        if (isZipFile)
        //        {
        //            string updateSrcPath = FormMain.Instance.UpdateSrcPath + "\\" + strLocalFileName;
        //            web.DownloadFile(strDownloadURL, updateSrcPath);

        //            FormMain.WriteLog("[INFO] UploadFile Download URL : " + strDownloadURL + " / Local File Name : " + updateSrcPath);

        //            // 압축풀어서 복사
        //            CheckUpdate(strFolder);
        //        }
        //        else
        //        {
        //            if (System.IO.File.Exists(strFilePath))
        //            {
        //                System.IO.FileInfo file = new FileInfo(strFilePath);
        //                file.IsReadOnly = false;
        //                System.IO.File.Delete(strFilePath);
        //            }

        //            web.DownloadFile(strDownloadURL, strFilePath);

        //            FormMain.WriteLog("[INFO] UploadFile Download URL : " + strDownloadURL + " / Local File Name : " + strFilePath);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        System.Diagnostics.Trace.WriteLine(e.Message);
        //        FormMain.WriteLog("[ERROR - DownloadFile] " + e.Message + " / Down URL : " + strDownloadURL + " / Local File Name : " + strFilePath);
        //        return false;
        //    }

        //    return true;
        //} 
        #endregion

        private string ChangeHangul(string strOrigin)
        {
            int nSlash = strOrigin.LastIndexOf('\\');
            string strTemp = "";

            for (int i = nSlash + 1; i < strOrigin.Length; i++)
            {
                char ch = strOrigin.ElementAt(i);

                if (ch > 256)
                    strTemp += '_';
                else
                    strTemp += ch;
            }

            strOrigin = strOrigin.Substring(0, nSlash + 1) + strTemp;
            return strOrigin;
        }

        /// <summary>
        /// 서버 -> 클라이언트로 파일 보내기
        /// </summary> 
        private bool Download(string searchPath)
        {
            try
            {
                int nIndex = searchPath.LastIndexOf('\\');
                string strFileName = searchPath.Substring(nIndex + 1);

                FileAttributes attr = File.GetAttributes(searchPath);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    DirectoryInfo dir = new DirectoryInfo(searchPath);
                    
                    string strZipPath = ChangeHangul(searchPath);
                    strFileName = ChangeHangul(strFileName);

                    // 1. 압축하기
                    string outPathName = strZipPath + ".zip";
                    if (File.Exists(outPathName + ".zip"))
                        File.Delete(outPathName + ".zip");

                    if (Compress(outPathName, searchPath))
                    {
                        // 2. Tomcat 폴더로 복사
                        File.Copy(outPathName, FormMain.Instance.DownloadURL + "\\" + strFileName + ".zip", true);
                        File.Delete(outPathName);

                        FormMain.WriteLog("[INFO] Download() : " + FormMain.Instance.DownloadURL + "\\" + strFileName + ".zip");
                    }                    
                }
                else
                {  
                    // 1. Tomcat 폴더로 복사
                    string downPath = FormMain.Instance.DownloadURL + "\\" + strFileName;
                    File.Copy(searchPath, downPath, true); 

                    FormMain.WriteLog("[INFO] Download() : " + FormMain.Instance.DownloadURL + "\\" + strFileName);
                }

                return true;
            }
            catch (Exception ex)
            {
                FormMain.WriteLog("[ERROR] Download() : " + ex.Message);
                return false;
            } 
        }

        #region 압축
        public bool Compress(string outPathName, string folderName)
        {
            try
            {
                FileStream fsOut = File.Create(outPathName);
                ZipOutputStream zipStream = new ZipOutputStream(fsOut);

                zipStream.SetLevel(9);

                int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

                CompressFolder(folderName, zipStream, folderOffset);

                zipStream.IsStreamOwner = true;
                zipStream.Close();

                return true;
            }
            catch (Exception ex)
            {
                FormMain.WriteLog("[ERROR] Compress : " + ex.Message);
                return false;
            }
        }

        private void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string fileName in files)
            {
                FileInfo fi = new FileInfo(fileName);

                string entryName = fileName.Substring(folderOffset);
                entryName = ZipEntry.CleanName(entryName);
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime;
                newEntry.Size = fi.Length;
                zipStream.PutNextEntry(newEntry);

                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(fileName))
                {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }

            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        } 
        #endregion

        #region Get List 
        private void GetServiceList()
        {
            try
            { 
                List<ServiceController> serviceList = SOPChecker.ServiceManager.GetAllSerivceList();

                StringBuilder sb = new StringBuilder();
                if (serviceList.Count == 0)
                {
                    sb.Append("Running Service Count : 0");
                }
                else
                {
                    foreach (ServiceController item in serviceList)
                    {                         
                        object path = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\" + item.ServiceName).GetValue("ImagePath");
                        object start = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\" + item.ServiceName).GetValue("Start");
                        string strStart = "";
                        if ((int)start == 0)
                            strStart = "부팅";
                        else if ((int)start == 1)
                            strStart = "시스템";
                        else if ((int)start == 2)
                            strStart = "자동";
                        else if ((int)start == 3)
                            strStart = "수동";
                        else if ((int)start == 4)
                            strStart = "사용불가";

                        sb.AppendLine("SERVICE NAME : " + item.ServiceName + " / STATUS : " + item.Status + " / 시작유형 : " + strStart + " / CanStop : " + item.CanStop + " / Path : " + path);
                        
                    }
                }

                FormMain.WriteLog(sb.ToString());
            }
            catch (Exception ex)
            {
                FormMain.WriteLog("[ERROR] GetServiceList : " + ex.Message);
            }
        }

        //private void GetFileList(string searchPath)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        DirectoryInfo dir = new DirectoryInfo(searchPath);
        //        if (dir.Exists)
        //        {
        //            foreach (DirectoryInfo item in dir.GetDirectories())
        //            {
        //                sb.AppendLine("DIRECTORY NAME " + item.FullName);
        //            }

        //            foreach (FileInfo item in dir.GetFiles())
        //            {
        //                sb.AppendLine("FILE NAME " + item.FullName);
        //            }
        //        }
        //        else
        //        {
        //            sb.Append(searchPath + " Count : 0");
        //        }

        //        FormMain.WriteLog(sb.ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        FormMain.WriteLog("[ERROR] GetFileList : " + ex.Message);
        //    }
        //}

        private List<string> driveList = new List<string>();
        private List<string> searchDrivePathList = new List<string>();

        private void GetFileList(string searchPath)
        {
            searchDrivePathList.Clear();
            driveList.Clear();

            string[] paths = searchPath.Split(',');
            foreach (string path in paths)
            {
                if (Directory.Exists(path.Trim()))
                {
                    searchDrivePathList.Add(path.Trim());
                    driveList.Add("[D]" + path.Trim());
                }
            }
            //searchDrivePathList.Add(@"C:\Program Files\Apache Software Foundation\Tomcat 7.0");
            //searchDrivePathList.Add(@"C:\UNE");

            //driveList.Add("Root");
            //string[] drives = Directory.GetLogicalDrives();
            //foreach (string item in drives)
            //{
            //    DriveInfo info = new DriveInfo(item);
            //    if (!info.IsReady)
            //        continue;

            //    driveList.Add(item);
            //    WriteDirectory(item);
            //}

            foreach (string item in searchDrivePathList)
            {
                WriteDirectory(item);
            }

            FormMain.WriteDrive(driveList);            
        }

        private void WriteDirectory(string path)
        {
            try
            {

                string[] folders = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    driveList.Add("[F]" + file);
                }

                foreach (string folder in folders)
                {
                    //DirectoryInfo info = new DirectoryInfo(folder);
                    //if (info.Attributes == FileAttributes.Hidden || info.Attributes == FileAttributes.Offline ||
                    //    info.Attributes == FileAttributes.System || info.Attributes == FileAttributes.Temporary)
                    //    continue;

                    driveList.Add("[D]" + folder);
                    WriteDirectory(folder);
                }
            }
            catch (Exception ex)
            {
                FormMain.WriteLog(path + " : " + ex.ToString());
            }
        }

        private void GetProcList(string searchName = "")
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                System.Diagnostics.Process[] process = null;
                if (searchName.Length > 0)
                    process = System.Diagnostics.Process.GetProcessesByName(searchName);
                else
                    process = System.Diagnostics.Process.GetProcesses();


                if (process.Length > 0)
                {
                    foreach (System.Diagnostics.Process item in process)
                    {
                        try
                        {
                            System.Diagnostics.ProcessModuleCollection module = item.Modules;

                            sb.AppendLine("PROCESS NAME : " + item.ProcessName + " / PATH : " + item.Modules[0].FileName);
                        }
                        catch (System.ComponentModel.Win32Exception)
                        {
                            sb.AppendLine("PROCESS NAME : " + item.ProcessName);
                        }
                    }
                }
                else
                {
                    sb.Append("PROCESS Count : 0");
                }

                FormMain.WriteLog(sb.ToString());
            }
            catch (Exception ex)
            {
                if (sb.ToString().Length > 0)
                    FormMain.WriteLog(sb.ToString());

                FormMain.WriteLog("[ERROR] GetProcList : " + ex.Message);
            }
        } 
        #endregion
           
        #region 압축파일 업로드 관련
        public void CheckUpdate(string strUploadPath)
        {
            if (strUploadPath.Length > 0 && FormMain.Instance.UpdateTempPath.Length > 0)
            {
                string strSrcFile = ReadSrc();

                if (strSrcFile != null)
                {
                    MakeEmpty();

                    if (ExtractToTrg(strSrcFile, FormMain.Instance.UpdateTempPath))
                    {
                        string strTargetFolderName = "";
                        string strFileListPath = MakeFileList(FormMain.Instance.UpdateTempPath, out strTargetFolderName);

                        if (strFileListPath != null && strFileListPath.Length > 0)
                        {
                            if (ExtractToTrg(strSrcFile, strUploadPath))
                            {
                                File.Delete(strSrcFile);

                                int nIndex = strFileListPath.LastIndexOf('\\');

                                if (nIndex >= 0)
                                {
                                    string strFileListName = strFileListPath.Substring(nIndex + 1);
                                    string strTrgPath = strUploadPath + "\\" + strTargetFolderName + "\\" + strFileListName;

                                    if (File.Exists(strTrgPath))
                                        File.Delete(strTrgPath);

                                    File.Copy(strFileListPath, strTrgPath);
                                }
                            }
                        }

                        MakeEmpty();
                    }
                }
            }
        }

        #region 압축 풀기
        private string ReadSrc()
        {
            string[] arrFiles = Directory.GetFiles(FormMain.Instance.UpdateSrcPath);

            foreach (string strFile in arrFiles)
            {
                int nIndex = strFile.LastIndexOf('.');

                if (nIndex >= 0)
                {
                    string strExt = strFile.Substring(nIndex + 1);

                    if (string.Compare(strExt, "zip", true) == 0)
                        return strFile;
                }
            }

            return null;
        }

        private string MakeFileList(string strPath, out string strTargetFolderName)
        {
            strTargetFolderName = "";

            string[] arrFiles = Directory.GetFiles(strPath);
            string strFileListPath = null, strTargetFileName = null;

            foreach (string strFile in arrFiles)
            {
                int nIndex = strFile.LastIndexOf('\\');

                if (nIndex >= 0)
                {
                    string _strFile = strFile.Substring(nIndex + 1);

                    if (string.Compare(_strFile, "update.xml", true) == 0)
                    {
                        strTargetFolderName = GetTargetFolder(strFile, out strTargetFileName);

                        if (strTargetFolderName != null && strTargetFileName != null &&
                            strTargetFolderName.Length > 0 && strTargetFileName.Length > 0)
                        {
                            strFileListPath = MakeFileList(strPath + "\\" + strTargetFolderName, strTargetFileName);
                        }

                        break;
                    }
                }
            }

            return strFileListPath;
        }

        private string GetTargetFolder(string strXMLPath, out string strTargetFile)
        {
            XmlTextReader reader = new XmlTextReader(strXMLPath);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(reader);

            strTargetFile = "";
            string strTargetFolder = "";

            foreach (XmlNode node in xmlDoc.ChildNodes)
            {
                if (string.Compare(node.Name, "update", true) == 0)
                {
                    strTargetFolder = ReadUpdate(node, out strTargetFile);
                    break;
                }
            }

            reader.Close();
            return strTargetFolder;
        }

        private string ReadUpdate(XmlNode node, out string strTargetFile)
        {
            strTargetFile = "";

            foreach (XmlNode child in node.ChildNodes)
            {
                if (string.Compare(child.Name, "versions", true) == 0)
                {
                    return ReadVersions(child, out strTargetFile);
                }
            }

            return null;
        }

        private string ReadVersions(XmlNode node, out string strTargetFile)
        {
            strTargetFile = "";

            // VersionID, VersionPath
            Dictionary<string, string> dicVersionPath = new Dictionary<string, string>();
            // VersionID, TargetFile
            Dictionary<string, string> dicVersionTarget = new Dictionary<string, string>();
            string strLastVersionID = "", strTargetVersionFile = "";

            foreach (XmlNode child in node.ChildNodes)
            {
                if (string.Compare(child.Name, "version", true) == 0)
                {
                    string strVersionPath = ReadVersion(child, out strTargetVersionFile);

                    if (strVersionPath != null && strTargetVersionFile != null)
                    {
                        XmlAttribute attr = child.Attributes[0];
                        string strVersionID = attr.Value;
                        dicVersionPath[strVersionID] = strVersionPath;
                        dicVersionTarget[strVersionID] = strTargetVersionFile;
                    }
                }
                else if (string.Compare(child.Name, "lastVersion", true) == 0)
                {
                    strLastVersionID = child.InnerText;
                }
            }

            if (dicVersionTarget.ContainsKey(strLastVersionID))
                strTargetFile = dicVersionTarget[strLastVersionID];

            if (dicVersionPath.ContainsKey(strLastVersionID))
            {
                return dicVersionPath[strLastVersionID];
            }

            return null;
        }

        private string ReadVersion(XmlNode node, out string strTargetFile)
        {
            strTargetFile = null;
            string strLocation = null;

            foreach (XmlNode child in node.ChildNodes)
            {
                if (string.Compare(child.Name, "location", true) == 0)
                {
                    strLocation = child.InnerText;
                }
                else if (string.Compare(child.Name, "target", true) == 0)
                {
                    XmlAttribute attr = child.Attributes[0];
                    strTargetFile = attr.Value;
                }
            }

            return strLocation;
        }

        private string MakeFileList(string strFolderPath, string strTargetFileName)
        {
            string strFileListPath = strFolderPath + "\\" + strTargetFileName;
            StreamWriter writer = new StreamWriter(strFileListPath, false, Encoding.UTF8);

            int nLen = strFolderPath.Length;
            string[] arrFiles = Directory.GetFiles(strFolderPath);

            foreach (string strFile in arrFiles)
            {
                if (strFile == strFileListPath)
                    continue;

                string strFileName = strFile.Substring(nLen + 1);
                writer.WriteLine(strFileName);
            }

            string[] arrFolders = Directory.GetDirectories(strFolderPath);

            foreach (string strFolder in arrFolders)
            {
                ReadFolder(writer, strFolder, strFolderPath);
            }

            writer.Close();
            return strFileListPath;
        }

        private void ReadFolder(StreamWriter writer, string strFolderPath, string strBaseFolderPath)
        {
            int nBaseLength = strBaseFolderPath.Length;
            string[] arrFiles = Directory.GetFiles(strFolderPath);

            foreach (string strFile in arrFiles)
            {
                string strFilePath = strFile.Substring(nBaseLength + 1);
                writer.WriteLine(strFilePath);
            }

            string[] arrFolders = Directory.GetDirectories(strFolderPath);

            foreach (string strFolder in arrFolders)
            {
                ReadFolder(writer, strFolder, strBaseFolderPath);
            }
        }

        private bool ExtractToTrg(string strSrcFile, string strTrgPath)
        {
            try
            {
                if (!Directory.Exists(strTrgPath))
                    Directory.CreateDirectory(strTrgPath);

                System.IO.FileStream fs = new System.IO.FileStream(strSrcFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

                ICSharpCode.SharpZipLib.Zip.ZipInputStream zis = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(fs);

                ICSharpCode.SharpZipLib.Zip.ZipEntry ze;

                while ((ze = zis.GetNextEntry()) != null)
                {
                    if (!ze.IsDirectory)
                    {
                        string fileName = System.IO.Path.GetFileName(ze.Name);

                        string destDir = System.IO.Path.Combine(strTrgPath,
                                         System.IO.Path.GetDirectoryName(ze.Name));

                        if (false == Directory.Exists(destDir))
                        {
                            System.IO.Directory.CreateDirectory(destDir);
                        }

                        string destPath = System.IO.Path.Combine(destDir, fileName);

                        System.IO.FileStream writer = new System.IO.FileStream(
                                        destPath, System.IO.FileMode.Create,
                                                System.IO.FileAccess.Write,
                                                    System.IO.FileShare.Write);

                        byte[] buffer = new byte[2048];
                        int len;
                        while ((len = zis.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, len);
                        }

                        writer.Close();
                    }
                }
            }
            catch (Exception e)
            {
                FormMain.WriteLog("[ERROR] ExtractToTrg() : " + e.Message);
                return false;
            }

            return true;
            //return Core.UZip.ExtractFile(strSrcFile, strTrgPath);
        }

        private void MakeEmpty()
        {
            string[] arrFiles = Directory.GetFiles(FormMain.Instance.UpdateTempPath);

            foreach (string strFile in arrFiles)
            {
                File.Delete(strFile);
            }

            string[] arrFolders = Directory.GetDirectories(FormMain.Instance.UpdateTempPath);

            foreach (string strFolder in arrFolders)
            {
                Directory.Delete(strFolder, true);
            }
        }  
        #endregion
        #endregion

        public void DeleteFile(string strFilePath)
        {
            try
            {
                int nIndex = strFilePath.LastIndexOf('\\');
                string strFileName = strFilePath.Substring(nIndex + 1);

                if (!strFileName.Contains('.'))
                {
                    strFileName += ".zip";
                }

                string strFilePath2 = MakePath(FormMain.Instance.DownloadURL, strFileName, false);

                if (System.IO.File.Exists(strFilePath2))
                {
                    System.IO.FileInfo file = new FileInfo(strFilePath2);
                    file.IsReadOnly = false;
                    System.IO.File.Delete(strFilePath2);

                    FormMain.WriteLog("[INFO] " + strFilePath2 + "delete file");
                }
            }
            catch (Exception ex)
            {
                FormMain.WriteLog("[ERROR] DeleteFile() : " + ex.Message);
            }
        }
         
        private string MakePath(string strPath1, string strPath2, bool isUrl)
        {
            if (isUrl) // http://127.0.0.1:8080/SOP/Download/
            {
                if (strPath1.Substring(strPath1.Length - 1) == "/")
                    return strPath1 + strPath2;
                else
                    return strPath1 + "/" + strPath2;
            }
            else // C:\DownloadTemp\
            {
                if (strPath1.Substring(strPath1.Length - 2) == "\\")
                    return strPath1 + strPath2;
                else
                    return strPath1 + "\\" + strPath2;
            }
        }
        private ServiceController GetService(string szServiceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == szServiceName)
                    return service;
            }
            return null;
        }

        public void RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = GetService(serviceName);
            if (service == null)
                return;

            int millisec1 = 0;
            try
            {
                millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception e)
            {
                Process[] proc = Process.GetProcessesByName("SOPServer");
                if (proc != null && proc.Length > 0)
                    proc[0].Kill();

                FormMain.WriteLog("SOPServer 프로세스 강제종료");
            }

            try
            {
                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception e)
            {
                FormMain.WriteLog("[ERROR] RestartService Start: " + e.Message);
            }
        }

        private void GetServerStatus(CommandItem commandItem)
        {
            if (commandItem.SearchPath.Length == 0)
                return;

            if (commandItem.IsStartService)
            {
                if (ServiceManager.IsRunningSerivce(commandItem.SearchPath))
                    commandItem.Result = 1;
            }
            else
            {
                System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(commandItem.SearchPath);
                if (process.Length > 0)
                    commandItem.Result = 1;
            }
        }

        private void FileCopyCommand(CommandItem commandItem)
        {
            if (commandItem.SearchPath.Contains("@"))
            {
                string[] arr = commandItem.SearchPath.Split('@');
                if (arr.Length == 2)
                {
                    string strSourceFileName = arr[0];
                    string strDestFileName = arr[1];

                    bool isDirectory = false;
                    
                    DirectoryInfo dirInfo = new DirectoryInfo(strSourceFileName);
                    if (dirInfo.Attributes.ToString() == "-1")
                        throw new ApplicationException("출발지 경로가 잘못되었습니다. ");

                    if (dirInfo.Attributes.ToString() == "Directory") // 폴더
                    {
                        CopyFolder(strSourceFileName, strDestFileName);
                        isDirectory = true;
                    }
                    else 
                    {
                        int nIndex = strSourceFileName.LastIndexOf("\\");

                        if (nIndex < 0)
                            return;

                        string strFileName = strSourceFileName.Substring(nIndex + 1);
                        string strFolderPath = strSourceFileName.Substring(0, nIndex);
                         
                        // 목적지
                        nIndex = strDestFileName.LastIndexOf("\\");
                        if (nIndex < 0)
                            return;

                        string strFileName2 = strDestFileName.Substring(nIndex + 1);
                        string strFolderPath2 = strDestFileName.Substring(0, nIndex);

                        if (!Directory.Exists(strFolderPath2))
                            Directory.CreateDirectory(strFolderPath2);

                        if (strFileName2 == "")
                            strFileName2 = strFileName;

                        string strDestFilePath = strFolderPath2 + "\\" + strFileName2;
                        File.Copy(strSourceFileName, strDestFilePath, true);
                    }

                    if (commandItem.IsStop)
                    {
                        if (isDirectory)
                            Directory.Delete(strSourceFileName, true);
                        else
                            File.Delete(strSourceFileName);
                    }
                }
            }
        }

        private void CopyFolder(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder);
            string[] folders = Directory.GetDirectories(sourceFolder);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);
                File.Copy(file, dest, true);
            }

            foreach (string folder in folders)
            {
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);
                CopyFolder(folder, dest);
            }
        }
    }
}
