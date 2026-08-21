using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.IO;

namespace KPXAgent
{
    public class Command
    {
        public enum CommandType { 
            NONE = -1, 
            UPDATE_N_REBOOT = 0, 
            SCREEN_CAPTURE = 1, 
            CLIENT_UPDATE_N_REBOOT = 2,
            SERVER_UPDATE = 3,
            TANK_SERVER_UPDATE = 4,
            PUSH_SERVER_UPDATE = 5,
            USER_ACCEPTANCE_UPDATE = 6,
            JSP_FILE_UPDATE = 7,
            CHECK_STATUS = 8,
            SERVER_DLL_UPDATE = 9,
            ZIP_FILE_UPDATE = 10,
            NORMAL_FILE_UPDATE = 11,
            SEARCH_FOLDER = 12,

            PROCESS_KILL = 1000,
            PROCESS_START = 2000,
            SERVICE_STOP = 3000,
            SERVICE_START = 4000,

            FILE_UPDATE = 100,
            SERVICE_FILE_UPDATE = 200
        };

        private CommandType m_type = CommandType.NONE;
        private int m_nID = -1;
        private DateTime m_timeStamp;
        private bool m_needClose = false;

        public CommandType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        public bool NeedClose
        {
            get { return m_needClose; }
        }

        public Command()
        {
        }

        public Command(int nType)
        {
            m_type = (CommandType)nType;
        }

        public Command(CommandType type)
        {
            m_type = type;
        }

        public bool Execute(WebDBManager dbMgr, int nCommandID, string fileName = "")
        {
            FormMain.SetLog("[INFO] CommandType : " + m_type.ToString());
            
            if (m_type == CommandType.UPDATE_N_REBOOT)
            {
                return UpdateNReboot(dbMgr, nCommandID);
            }
            else if (m_type == CommandType.SCREEN_CAPTURE)
            {
                return SendScreenShot(dbMgr, nCommandID);
            }
            else if (m_type == CommandType.CLIENT_UPDATE_N_REBOOT)
            {
                return ClientUpdateNReboot(dbMgr, nCommandID, @"C:\KPXMonitoring\KpxMonitoring.exe");
            }
            else if (m_type == CommandType.SERVER_UPDATE)
            {
                return ServerUpdateNReboot(dbMgr, nCommandID, @"C:\KPXServer\PSensorServer.exe");
            }
            else if (m_type == CommandType.SERVER_DLL_UPDATE)
            {
                return ServerDllUpdateNReboot(dbMgr, nCommandID, @"C:\KPXServer\", fileName);
            }
            else if (m_type == CommandType.TANK_SERVER_UPDATE)
            {
                return ServiceUpdateNReboot(dbMgr, nCommandID, @"C:\KPXTankServer\KPXTankLevelServer.exe", "TankLevelServer");
            }
            else if (m_type == CommandType.PUSH_SERVER_UPDATE)
            {
                return PushServerUpdateNReboot(dbMgr, nCommandID, @"C:\KPXPushServer\PushServer.exe");
            }
            else if (m_type == CommandType.USER_ACCEPTANCE_UPDATE)
            {
                return UserAccepUpdate(dbMgr, nCommandID, @"C:\KPXUserAcceptance\KpxUserAcceptance.exe");
            }
            else if (m_type == CommandType.JSP_FILE_UPDATE)
            {
                return DownloadJspFile(dbMgr, nCommandID, fileName);
            }
            else if (m_type == CommandType.CHECK_STATUS)
            {
                return CheckStatus(dbMgr, nCommandID, "TankLevelServer");
            }
            else if (m_type == CommandType.ZIP_FILE_UPDATE)
            {
                return DownloadCommand(dbMgr, nCommandID, fileName, true);
            }
            else if (m_type == CommandType.NORMAL_FILE_UPDATE)
            {
                return DownloadCommand(dbMgr, nCommandID, fileName, false);
            }
            else if (m_type == Command.CommandType.SEARCH_FOLDER)
            {
                return SendSearchFolderResult(dbMgr, nCommandID, fileName);
            }
            else if (m_type == CommandType.PROCESS_KILL)
            {
                RemoveCommand(dbMgr, nCommandID);
                return ProcessKill(dbMgr, fileName);
            }
            else if (m_type == CommandType.PROCESS_START)
            {
                RemoveCommand(dbMgr, nCommandID);
                return ProcessStart(dbMgr, fileName);
            }
            else if (m_type == CommandType.FILE_UPDATE)
            {
                RemoveCommand(dbMgr, nCommandID);
                string strFileName = "", strFolder = "";
                return DownloadFile(ref strFileName, ref strFolder, fileName);
            }
            else if (m_type == CommandType.SERVICE_STOP)
            {
                RemoveCommand(dbMgr, nCommandID);
                return ServiceStop(dbMgr, fileName);
            }
            else if (m_type == CommandType.SERVICE_START)
            {
                RemoveCommand(dbMgr, nCommandID);
                return ServiceStart(dbMgr, fileName);
            }

            return true;
        }

        private bool DownloadCommand(WebDBManager dbMgr, int nCommandID, string strSrc, bool zipFile)
        {
            string strFolderPath = "", strFileName = "";

            if (GetFolderNFile(strSrc, ref strFolderPath, ref strFileName))
            {
                if (zipFile)
                    return DownloadZipFile(dbMgr, nCommandID, strFolderPath, strFileName);
                else
                    return DownloadNormalFile(dbMgr, nCommandID, strFolderPath, strFileName);
            }

            RemoveCommand(dbMgr, nCommandID);

            string strTag = zipFile ? "Zip" : "Normal";

            System.Diagnostics.Trace.WriteLine(strTag + " File Parameter 오류 : " + strSrc);
            FormMain.SetLog("err/Update " + strTag + " File Parameter : " + strSrc);
            return false;
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

        private bool RemoveCommand(WebDBManager dbMgr, int nCommandID)
        {
            string strSQL = "Delete from AgentCommand where ID = " + nCommandID.ToString();
            return dbMgr.GetResultData(strSQL, 0) != null;
        } 

        #region Agent Update
        private bool UpdateNReboot(WebDBManager dbMgr, int nCommandID)
        {
            string strFileName = "", strFolder = "";
            if (DownloadSelf(ref strFileName, ref strFolder) == false)
                return false;
            FormMain.SetLog("info:UpdateNReboot : Agent down ok");

            if (RemoveCommand(dbMgr, nCommandID))
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strFileName;
                startInfo.WorkingDirectory = strFolder;
                startInfo.ErrorDialog = true;
                startInfo.Arguments = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    FormMain.SetLog("info:UpdateNReboot : Agent Start ok");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    FormMain.SetLog("err/UpdateNReboot : " + ex.Message);
                    return false;
                }

                m_needClose = true;
                return true;
            }

            return false;
        }
        private bool DownloadSelf(ref string strDownloadFileName, ref string strDownloadFolderPath)
        {
            string strURL = FormMain.Instance.downloadURL;

            if (strURL.Length == 0)
                return false;

            strURL += "/KPX/";

            string strPath = System.Windows.Forms.Application.ExecutablePath;

            int nIndex = strPath.LastIndexOf('\\');
            string strFolder = strPath.Substring(0, nIndex + 1);

            int nIndex2 = strPath.LastIndexOf('.');
            string strLocalFileName = strPath.Substring(nIndex + 1, nIndex2 - nIndex - 1) + "_temp.exe";// +strPath.Substring(nIndex2);
            string strFilePath = strFolder + strLocalFileName;

            strURL += strPath.Substring(nIndex + 1);
            strURL = strURL.Replace(".EXE", ".exe");

            strDownloadFileName = strLocalFileName;
            strDownloadFolderPath = strFolder;

            try
            {
                if (System.IO.File.Exists(strFilePath))
                    System.IO.File.Delete(strFilePath);

                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(strURL);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");

                FormMain.SetLog("msg/DownloadSelf : strURL / " + strURL + " , strFilePath / " + strFilePath);

                web.DownloadFile(strURL, strFilePath);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                FormMain.SetLog("err/DownloadSelf : " + e.Message);
                return false;
            }

            return true;
        } 
        #endregion

        #region Process, Service
        private bool ProcessKill(WebDBManager dbMgr, string strFileName)
        {
            if (strFileName.Contains(@"\"))
            {
                string strFolderPath = "", strFileName2 = "";
                if (GetFolderNFile2(strFileName, ref strFolderPath, ref strFileName2))
                { 
                    strFileName = strFileName2.Replace(".exe", "");
                } 
            }

            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(strFileName);
            if (process.Length > 0)
            {
                process[0].Kill();
                FormMain.SetLog("[INFO] Kill " + strFileName);

                return true;
            } 

            return false;            
        }
        private bool ProcessStart(WebDBManager dbMgr, string strPath)
        {
            string strFolderPath = "", strFileName = "";
            if (GetFolderNFile2(strPath, ref strFolderPath, ref strFileName))
            { 
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strFileName;
                startInfo.WorkingDirectory = strFolderPath;
                startInfo.ErrorDialog = true; 

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    FormMain.SetLog("[INFO] Process Start : " + strFolderPath + strFileName);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    FormMain.SetLog("[ERROR] Process Start : " + ex.Message);
                    return false;
                }

                m_needClose = false;
                return true;
            } 
            else
            {
                FormMain.SetLog("[ERROR] Process Start : 경로 확인");
                return false; 
            } 
        }
        private bool ServiceStop(WebDBManager dbMgr, string strPath)
        {
            if (strPath.Contains(@"\"))
            {
                string strFolderPath = "", strFileName2 = "";
                if (GetFolderNFile2(strPath, ref strFolderPath, ref strFileName2))
                {
                    strPath = strFileName2.Replace(".exe", "");
                }
            }

            if (SOPChecker.ServiceManager.IsRunningSerivce(strPath))
            {
                SOPChecker.ServiceManager.StopService(strPath, 3000);
                FormMain.SetLog("[INFO] Service Stop : " + strPath);
            } 

            return true;
        }
        private bool ServiceStart(WebDBManager dbMgr, string strPath)
        {
            try
            {
                if (strPath.Contains(@"\"))
                {
                    string strFolderPath = "", strFileName2 = "";
                    if (GetFolderNFile2(strPath, ref strFolderPath, ref strFileName2))
                    {
                        strPath = strFileName2.Replace(".exe", "");
                    }
                }

                SOPChecker.ServiceManager.StartService(strPath, 1000);
                FormMain.SetLog("[INFO] Service Start : " + strPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                FormMain.SetLog("[ERROR] Service Start : " + ex.Message);
                return false;
            }

            return true;
        }
        #endregion

        #region Client, Server, Service Update
        private bool ClientUpdateNReboot(WebDBManager dbMgr, int nCommandID, string strExcutePath)
        {
            //Kill
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("KpxMonitoring");
            if (process.Length > 0)
            {
                process[0].Kill();
                FormMain.SetLog("info:ClientUpdateNReboot : client Kill ok");
            }

            //Down 
            string strFileName = "", strFolder = "";
            if (DownloadFile(ref strFileName, ref strFolder, strExcutePath) == false)
                return false;
            FormMain.SetLog("info:ClientUpdateNReboot : client down ok");

            //Start
            if (RemoveCommand(dbMgr, nCommandID))
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strFileName;
                startInfo.WorkingDirectory = strFolder;
                startInfo.ErrorDialog = true;
                startInfo.Arguments = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    FormMain.SetLog("info:ClientUpdateNReboot : client Start ok"); 
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    FormMain.SetLog("err/" + ex.Message);
                    return false;
                }

                m_needClose = false;
                return true;
            }

            return false;
        }
        private bool PushServerUpdateNReboot(WebDBManager dbMgr, int nCommandID, string strExcutePath)
        {
            //Kill
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("PushServer");
            if (process.Length > 0)
            {
                process[0].Kill();
                FormMain.SetLog("info:PushServerUpdateNReboot : PushServer Kill ok");
            }

            //Down 
            string strFileName = "", strFolder = "";
            if (DownloadFile(ref strFileName, ref strFolder, strExcutePath) == false)
                return false;
            FormMain.SetLog("info:PushServerUpdateNReboot : PushServer down ok");

            //Start
            if (RemoveCommand(dbMgr, nCommandID))
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strFileName;
                startInfo.WorkingDirectory = strFolder;
                startInfo.ErrorDialog = true;
                startInfo.Arguments = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    FormMain.SetLog("info:PushServerUpdateNReboot : PushServer Start ok");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    FormMain.SetLog("err/" + ex.Message);
                    return false;
                }

                m_needClose = false;
                return true;
            }

            return false;
        }

        private bool ServerUpdateNReboot(WebDBManager dbMgr, int nCommandID, string strExcutePath)
        {
            //Kill
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("PSensorServer");
            if (process.Length > 0)
            {
                process[0].Kill();
                FormMain.SetLog("info:ServerUpdateNReboot : server Kill ok");
            }

            //Down 
            string strFileName = "", strFolder = "";
            if (DownloadFile(ref strFileName, ref strFolder, strExcutePath) == false)
                return false;
            FormMain.SetLog("info:ServerUpdateNReboot : server down ok");

            //Start
            if (RemoveCommand(dbMgr, nCommandID))
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strFileName;
                startInfo.WorkingDirectory = strFolder;
                startInfo.ErrorDialog = true;
                startInfo.Arguments = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    FormMain.SetLog("info:ServerUpdateNReboot : server Start ok"); 
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    FormMain.SetLog("err/" + ex.Message);
                    return false;
                }

                m_needClose = false;
                return true;
            }

            return false;
        }
        private bool ServerDllUpdateNReboot(WebDBManager dbMgr, int nCommandID, string strExcutePath, string dllName)
        {
            //Kill
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("PSensorServer");
            if (process.Length > 0)
            {
                process[0].Kill();
                FormMain.SetLog("info:ServerDllUpdateNReboot : server Kill ok");
            }

            //Down  
            if (DownloadServerDllFile(strExcutePath, dllName) == false)
                return false;
            FormMain.SetLog("info:ServerDllUpdateNReboot : server dll down ok");

            //Start
            if (RemoveCommand(dbMgr, nCommandID))
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.FileName = strExcutePath + "PSensorServer.exe";
                startInfo.WorkingDirectory = strExcutePath;
                startInfo.ErrorDialog = true;
                startInfo.Arguments = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    FormMain.SetLog("info:ServerDllUpdateNReboot : server Start ok");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    FormMain.SetLog("err/" + ex.Message);
                    return false;
                }

                m_needClose = false;
                return true;
            }

            return false;
        }

        private bool ServiceUpdateNReboot(WebDBManager dbMgr, int nCommandID, string strExcutePath, string strSvcName)
        {
            int nIndex = strExcutePath.LastIndexOf('\\');
            //string strSvcName = strExcutePath.Substring(nIndex + 1).Replace(".exe", "");

            // Service Stop
            if (SOPChecker.ServiceManager.IsRunningSerivce(strSvcName))
            {
                SOPChecker.ServiceManager.StopService(strSvcName, 3000);
                FormMain.SetLog("info:ServiceUpdateNReboot : tank service stop ok");
            }
            else
                FormMain.SetLog("info:ServiceUpdateNReboot : old tank service status : stop");

            //Down 
            string strFileName = "", strFolder = "";
            if (DownloadFile(ref strFileName, ref strFolder, strExcutePath) == false)
                return false;
            FormMain.SetLog("info:ServiceUpdateNReboot : tank service down ok");

            //Start
            if (RemoveCommand(dbMgr, nCommandID))
            {
                try
                {
                    SOPChecker.ServiceManager.StartService(strSvcName, 1000);
                    FormMain.SetLog("info:ServiceUpdateNReboot : tank service restart ok"); 
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    FormMain.SetLog("err/ServiceUpdateNReboot : " + ex.Message);
                    return false;
                }

                m_needClose = false;
                return true;
            }

            return false;
        }
        private bool UserAccepUpdate(WebDBManager dbMgr, int nCommandID, string strExcutePath)
        {
            //Kill
            System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("KpxUserAcceptance");
            if (process.Length > 0)
            {
                process[0].Kill();
                FormMain.SetLog("info:UserAccepUpdate : UserAccepUpdate Kill ok");
            }

            //Down 
            string strFileName = "", strFolder = "";
            if (DownloadFile(ref strFileName, ref strFolder, strExcutePath) == false)
                return false;
            FormMain.SetLog("info:UserAccepUpdate : UserAccepUpdate down ok");

            //Start
            if (RemoveCommand(dbMgr, nCommandID))
            {
                //System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                //startInfo.FileName = strFileName;
                //startInfo.WorkingDirectory = strFolder;
                //startInfo.ErrorDialog = true;
                //startInfo.Arguments = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

                //try
                //{
                //    System.Diagnostics.Process.Start(startInfo);
                //    FormMain.SetLog("info:ClientUpdateNReboot : 클라이언트 Start 완료");
                //}
                //catch (Exception ex)
                //{
                //    System.Diagnostics.Trace.WriteLine(ex.Message);
                //    FormMain.SetLog("err/" + ex.Message);
                //    return false;
                //}

                m_needClose = false;
                return true;
            }

            return false;
        }
        private bool DownloadFile(ref string strDownloadFileName, ref string strDownloadFolderPath, string strExcutePath)
        {
            string strURL = FormMain.Instance.downloadURL;

            if (strURL.Length == 0)
                return false;

            strURL += "/KPX/";

            int nIndex = strExcutePath.LastIndexOf('\\');
            string strFolder = strExcutePath.Substring(0, nIndex + 1);

            string strLocalFileName = strExcutePath.Substring(nIndex + 1);
            string strFilePath = strFolder + strLocalFileName;

            strURL += strExcutePath.Substring(nIndex + 1);
            strURL = strURL.Replace(".EXE", ".exe");

            strDownloadFileName = strLocalFileName;
            strDownloadFolderPath = strFolder;

            try
            {
                if (System.IO.File.Exists(strFilePath))
                {
                    System.IO.FileInfo file = new FileInfo(strFilePath);
                    file.IsReadOnly = false;
                    System.IO.File.Delete(strFilePath);
                }

                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(strURL);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");

                web.DownloadFile(strURL, strFilePath);
            }
            catch (Exception e)
            {
                
                System.Diagnostics.Trace.WriteLine(e.Message);
                FormMain.SetLog("[ERROR - DownloadFile] " + e.Message + " / Down URL : " + strURL + " / Local File Name : " + strFilePath);
                return false;
            }

            return true;
        }
        private bool DownloadServerDllFile(string strFilePath, string strFileName)
        {
            string strURL = FormMain.Instance.downloadURL; 
            if (strURL.Length == 0)
                return false;

            strURL += "/KPX/" + strFileName;

            try
            {
                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(strURL);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");

                web.DownloadFile(uri, strFilePath + strFileName);  
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                FormMain.SetLog("err/DownloadServerDllFile : " + e.Message + " / strURL : " + strURL + " / strFilePath : " + strFilePath + strFileName);
                return false;
            }

            return true;
        } 
        private bool DownloadJspFile(WebDBManager dbMgr, int nCommandID, string strFileName)
        {  
            string strURL = FormMain.Instance.downloadURL;
            string strFilePath = @"D:\Tomcat 7.0\webapps\ROOT\SOP\KPX\" + strFileName; 
            if (strURL.Length == 0)
                return false;

            strURL += "/KPX/" + strFileName;

            try
            {
                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(strURL);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");

                web.DownloadFile(uri, strFilePath);
                System.IO.File.Delete(strFilePath.Replace(".txt", ".jsp"));
                string ss = System.IO.Path.ChangeExtension(strFilePath, ".jsp");
                System.IO.File.Move(strFilePath, ss);

                FormMain.SetLog("info/DownloadJspFile down 완료: " + strFilePath);

                RemoveCommand(dbMgr, nCommandID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                FormMain.SetLog("err/DownloadJspFile : " + e.Message + " / strURL : " + strURL + " / strFilePath : " + strFilePath);
                return false;
            }

            return true;
        } 

        private bool DownloadZipFile(WebDBManager dbMgr, int nCommandID, string strFolderPath, string strFileName)
        {
            string strURL = FormMain.Instance.downloadURL;
            string strFilePath = Path.GetTempPath() + strFileName;
            if (strURL.Length == 0)
                return false;

            if (strFolderPath.Length == 0)
            {
                // strFolderPath가 빈 문자열이면 Agent가 설치된 경로에 파일을 다운로드 받는다.
                strFolderPath = GetCurrentFolder();
            }

            if (strFolderPath.EndsWith("\\"))
                strFolderPath = strFolderPath.Substring(0, strFolderPath.Length - 1);

            strURL += "/KPX/" + strFileName;

            try
            {
                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(strURL);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");

                web.DownloadFile(uri, strFilePath);
                ZipManager.ExtractToTrg(strFilePath, strFolderPath);
                File.Delete(strFilePath);
                RemoveCommand(dbMgr, nCommandID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                FormMain.SetLog("err/Download Zip File : " + e.Message + " / strURL : " + strURL + " / strFilePath : " + strFilePath);
                return false;
            }

            return true;
        }

        private bool DownloadNormalFile(WebDBManager dbMgr, int nCommandID, string strFolderPath, string strFileName)
        {
            if (strFolderPath.Length == 0)
            {
                // strFolderPath가 빈 문자열이면 Agent가 설치된 경로에 파일을 다운로드 받는다.
                strFolderPath = GetCurrentFolder();
            }

            if (strFolderPath.EndsWith("\\") == false)
                strFolderPath += "\\";

            string strURL = FormMain.Instance.downloadURL;
            string strFilePath = strFolderPath + strFileName;
            if (strURL.Length == 0)
                return false;

            strURL += "/KPX/" + strFileName;

            try
            {
                System.Net.WebClient web = new System.Net.WebClient();

                Uri uri = new Uri(strURL);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                web.Credentials = new NetworkCredential("sop", "sop");

                web.DownloadFile(uri, strFilePath);
                RemoveCommand(dbMgr, nCommandID);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                FormMain.SetLog("err/Download Normal File : " + e.Message + " / strURL : " + strURL + " / strFilePath : " + strFilePath);
                return false;
            }

            return true;
        }
        #endregion

        #region 클라이언트, 서버 상태 확인
        private bool CheckStatus(WebDBManager dbMgr, int nCommandID, string strSvcName)
        {
            try
            {
                System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("KpxMonitoring");
                if (process.Length > 0)
                    FormMain.SetLog("status:client run");
                else
                    FormMain.SetLog("status:client stop");

                process = System.Diagnostics.Process.GetProcessesByName("PSensorServer");
                if (process.Length > 0)
                    FormMain.SetLog("status:sensor server run");
                else
                    FormMain.SetLog("status:sensor server stop");

                if (SOPChecker.ServiceManager.IsRunningSerivce(strSvcName))
                    FormMain.SetLog("status:tank service run");
                else
                    FormMain.SetLog("status:tank service stop");

                RemoveCommand(dbMgr, nCommandID);
            }
            catch (Exception ex)
            {
                FormMain.SetLog("err/CheckStatus : " + ex.Message); 
            }
            return true;
        }
        #endregion

        #region ScreenShot
        private bool SendScreenShot(WebDBManager dbMgr, int nCommandID)
        {
            Screen[] allScreens = Screen.AllScreens;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            for (int i = 0; i < allScreens.Count(); i++)
            {
                Screen screen = allScreens[i];

                //Create a new bitmap.
                var bmpScreenshot = new Bitmap(screen.Bounds.Width,
                                               screen.Bounds.Height,
                                               System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                // Create a graphics object from the bitmap.
                var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

                // Take the screenshot from the upper left corner to the right bottom corner.
                gfxScreenshot.CopyFromScreen(screen.Bounds.X,
                                            screen.Bounds.Y,
                                            0,
                                            0,
                                            screen.Bounds.Size,
                                            CopyPixelOperation.SourceCopy);

                string strFolder = GetCurrentFolder();
                string strFileName = "Screen_" + (i + 1).ToString() + "_" + strTime + ".png";

                // Save the screenshot to the specified path that the user has chosen.
                bmpScreenshot.Save(strFileName, System.Drawing.Imaging.ImageFormat.Png);

                if (!UploadFile(strFolder + strFileName))
                {
                    File.Delete(strFolder + strFileName);
                    return false;
                } 
            }

            return RemoveCommand(dbMgr, nCommandID);
        }
        private string GetCurrentFolder()
        {
            string strPath = System.Windows.Forms.Application.ExecutablePath;

            int nIndex = strPath.LastIndexOf('\\');
            string strFolder = strPath.Substring(0, nIndex + 1);
            return strFolder;
        } 
        public static bool UploadFile(string strFilePath)
        {
            try
            {
                string strURL = FormMain.Instance.downloadURL;

                if (strURL.Length == 0)
                    return false;

                //http://unes.iptime.org:10091/KPX/
                strURL = strURL.Replace("SOP", "KPX/");

                string szFileName = strFilePath;

                int nIndex = szFileName.LastIndexOf('\\');
                string strUploadFileName = szFileName.Substring(nIndex + 1);

                WebClient wc = new WebClient();

                Uri uri = new Uri(strURL + strUploadFileName);

                CredentialCache credentials = new CredentialCache();
                NetworkCredential netCredential = new NetworkCredential("sop", "sop");
                credentials.Add(uri, "Basic", netCredential);
                wc.Credentials = new NetworkCredential("sop", "sop");
                wc.UploadFile(uri, "PUT", szFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                return false;
            }

            return true;
        } 

        private bool SendSearchFolderResult(WebDBManager dbMgr, int nCommandID, string strFolderPath)
        {
            if (strFolderPath.EndsWith("\\"))
                strFolderPath = strFolderPath.Substring(0, strFolderPath.Length - 1);

            if (Directory.Exists(strFolderPath) == false)
            {
                RemoveCommand(dbMgr, nCommandID);
                System.Diagnostics.Trace.WriteLine("존재하지 않는 폴더 경로 : " + strFolderPath);
                FormMain.SetLog("err/No Exist Search Folder : " + strFolderPath);
                return false;
            }

            string strFileName = "SearchFolderResult_" + FormMain.Instance.AreaType.ToString() + ".txt";

            StreamWriter writer = new StreamWriter(strFileName, false, Encoding.UTF8);
            writer.WriteLine("탐색 : " + strFolderPath);
            SearchFolder(strFolderPath, writer);
            writer.Close();

            string strFolder = GetCurrentFolder();
            string strFilePath = strFolder.EndsWith("\\") ? strFolder + strFileName : strFolder + "\\" + strFileName;
            UploadFile(strFilePath);

            RemoveCommand(dbMgr, nCommandID);
            return true;
        }

        private void SearchFolder(string strFolderPath, StreamWriter writer)
        {
            string[] files = Directory.GetFiles(strFolderPath);

            foreach (string strFilePath in files)
            {
                writer.WriteLine(strFilePath);
            }

            string[] folders = Directory.GetDirectories(strFolderPath);

            foreach (string strFolder in folders)
            {
                SearchFolder(strFolder, writer);
            }
        }

        #endregion
    }
}
