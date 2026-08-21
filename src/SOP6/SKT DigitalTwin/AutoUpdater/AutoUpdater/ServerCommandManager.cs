using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DBUtility2;
using System.ServiceProcess;

namespace AutoUpdater
{
    public class ServerCommandManager : CommandManager
    {
        public static CommandResultType ProcessCommand(int cmd, string strParameter, WebDBManagerEx dbMgr, ref string strResultMessage)
        {
            if (cmd == (int)CommandType.Start)
                return RunProcess(strParameter, ref strResultMessage);
            else if (cmd == (int)CommandType.Stop)
                return StopProcess(strParameter, ref strResultMessage);
            else if (cmd == (int)CommandType.Update)
                return UpdateProcess(strParameter, dbMgr, ref strResultMessage);

            return CommandResultType.UnknownCommand;
        }

        private static CommandResultType UpdateProcess(string strParameter, WebDBManagerEx dbMgr, ref string strResultMessage)
        {
            strResultMessage = "";

#if !SERVICE
            if (strParameter == null)
                return CommandResultType.UpdateFail;

            strParameter = strParameter.Trim();
            int nIndex = strParameter.IndexOf(' ');

            if (nIndex < 0)
                return CommandResultType.UpdateFail;

            int nIndex2 = strParameter.IndexOf(' ', nIndex + 1);

            if (nIndex2 < 0)
                return CommandResultType.UpdateFail;

            string strVersionName = strParameter.Substring(0, nIndex).Trim();
            string strServiceName = strParameter.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();
            string strZipFileName = strParameter.Substring(nIndex2 + 1).Trim();

            string strTargetFolder = GetServiceFolderPath(strServiceName);

            if (strTargetFolder.Length == 0)
            {
                strResultMessage = strServiceName + " 서비스를 찾을수 없습니다.";
                return CommandResultType.UpdateFail;
            }

            bool needUpdate;

            if (CheckVersion(false, strVersionName, out needUpdate, ref strResultMessage) == false)
                return CommandResultType.UpdateFail;

            if (needUpdate == false)
            {
                strResultMessage = "이미 업데이트 되었습니다.";
                return CommandResultType.UpdateSeccess;
            }

            string strFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            bool newTempFolder = false;

            if (strFolder.EndsWith("\\"))
                strFolder = strFolder + "Temp";
            else
                strFolder = strFolder + "\\Temp";

            if (System.IO.Directory.Exists(strFolder) == false)
            {
                System.IO.Directory.CreateDirectory(strFolder);
                newTempFolder = true;
            }

            string strLocalPath = strFolder.EndsWith("\\") ? strFolder + strZipFileName : strFolder + "\\" + strZipFileName;

            string strURL = GetURL(dbMgr, strZipFileName);

            if (DownloadFile(dbMgr, strURL, strLocalPath, ref strResultMessage) == false)
                return CommandResultType.UpdateFail;

            if (StopService(strServiceName, ref strResultMessage) == false)
            {
                DeleteFile(strLocalPath);
                return CommandResultType.UpdateFail;
            }

            if (UpdateFile(strLocalPath, strTargetFolder, ref strResultMessage) == false)
                return CommandResultType.UpdateFail;

            if (newTempFolder)
                DeleteFolder(strFolder);

            UpdateVersionFile(true, strVersionName);

            if (StartService(strServiceName, ref strResultMessage) == false)
                return CommandResultType.UpdateFail;
#endif
            return CommandResultType.UpdateSeccess;
        }

        private static string GetServiceFolderPath(string strServiceName)
        {
            string strFolderPath = "";
            object path = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Services\\" + strServiceName).GetValue("ImagePath");

            if (path == null)
                return "";

            strFolderPath = (string)path;

            int nIndex = strFolderPath.LastIndexOf('\\');

            if (nIndex < 0)
                return strFolderPath;

            strFolderPath = strFolderPath.Substring(0, nIndex);
            return strFolderPath;
        }

        private static CommandResultType StopProcess(string strParameter, ref string strResultMessage)
        {
            strResultMessage = "";

            if (strParameter == null)
                return CommandResultType.StopFail;

            string strServiceName = strParameter.Trim();

            if (StopService(strServiceName, ref strResultMessage) == false)
                return CommandResultType.StopFail;

            return CommandResultType.StopSuccess;
        }

        private static bool StopService(string strServiceName, ref string strResultMessage)
        {
            ServiceController service = GetService(strServiceName);

            if (service.Status == ServiceControllerStatus.Stopped)
                return true;
            else if (service.Status != ServiceControllerStatus.StopPending)
                service.Stop();

            DateTime dtPrev = DateTime.Now;
            double timeoutSeconds = 5.0;

            while (service.Status != ServiceControllerStatus.Stopped)
            {
                Thread.Sleep(200);
                TimeSpan span = DateTime.Now - dtPrev;

                if (span.TotalSeconds >= timeoutSeconds)
                {
                    strResultMessage = "서비스 중지가 실패하였습니다.";
                    return false;
                }

                service = GetService(strServiceName);
            }

            return true;
        }

        private static CommandResultType RunProcess(string strParameter, ref string strResultMessage)
        {
            strResultMessage = "";

            if (strParameter == null)
                return CommandResultType.StartFail;

            string strServiceName = strParameter.Trim();

            if (StartService(strServiceName, ref strResultMessage) == false)
                return CommandResultType.StartFail;

            return CommandResultType.StartSuccess;
        }

        private static bool StartService(string strServiceName, ref string strResultMessage)
        {
            ServiceController service = GetService(strServiceName);

            if (service.Status == ServiceControllerStatus.Running)
                return true;
            else if (service.Status != ServiceControllerStatus.StartPending)
                service.Start();

            DateTime dtPrev = DateTime.Now;
            double timeoutSeconds = 5.0;

            while (service.Status != ServiceControllerStatus.Running)
            {
                Thread.Sleep(200);
                TimeSpan span = DateTime.Now - dtPrev;

                if (span.TotalSeconds >= timeoutSeconds)
                {
                    strResultMessage = "서비스 실행에 실패하였습니다.";
                    return false;
                }

                service = GetService(strServiceName);
            }

            return true;
        }

        private static ServiceController GetService(string strServiceName)
        {
            ServiceController _service = null;

            foreach (ServiceController service in ServiceController.GetServices())
            {
                if (service.ServiceName == strServiceName)
                {
                    _service = service;
                    break;
                }
            }

            return _service;
        }
    }
}
