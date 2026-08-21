using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DBUtility2;

namespace AutoUpdater
{
    public class ClientCommandManager : CommandManager
    {
        public static CommandResultType ProcessCommand(int cmd, string strParameter, string strClientBaseFolder, WebDBManagerEx dbMgr, ref string strErrorMessage)
        {
            if (cmd == (int)CommandType.Start)
                return RunProcess(strParameter, strClientBaseFolder, ref strErrorMessage);
            else if (cmd == (int)CommandType.Stop)
                return StopProcess(strParameter, ref strErrorMessage);
            else if (cmd == (int)CommandType.Update)
                return UpdateProcess(strParameter, strClientBaseFolder, dbMgr, ref strErrorMessage);

            return CommandResultType.UnknownCommand;
        }

        private static CommandResultType UpdateProcess(string strParameter, string strClientBaseFolder, WebDBManagerEx dbMgr, ref string strResultMessage)
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
            string strMainProcess = strParameter.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();
            string strZipFileName = strParameter.Substring(nIndex2 + 1).Trim();

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

            bool closeSOPSimulator;

            if (KillProcess(strMainProcess, ref strResultMessage, out closeSOPSimulator) == false)
            {
                DeleteFile(strLocalPath);
                return CommandResultType.UpdateFail;
            }

            if (UpdateFile(strLocalPath, strClientBaseFolder, ref strResultMessage) == false)
                return CommandResultType.UpdateFail;

            UpdateVersionFile(false, strVersionName);

            if (newTempFolder)
                DeleteFolder(strFolder);

            if (closeSOPSimulator == false)
            {
                if (RunProcess(strMainProcess + ".exe", strClientBaseFolder, ref strResultMessage) == CommandResultType.StartFail)
                    return CommandResultType.UpdateFail;
            }
#endif
            return CommandResultType.UpdateSeccess;
        }

        // 업데이트 하기전 기존에 실행중인 Prcess가 있으면 먼저 중지시킨다.
        private static bool KillProcess(string strMainProcess, ref string strResultMessage, out bool alreadyKilled)
        {
            alreadyKilled = true;

            string[] processNameList = new string[] { "BroadRunner", "ControlTeamEditor2", "HelpViewer", "SOPBulletin", "SOPManager2", "HmlReport", strMainProcess, "TeamEditor" };

            foreach (string strProcessName in processNameList)
            {
                Process[] processList = Process.GetProcessesByName(strProcessName);

                if (processList == null)
                    continue;
                else if (processList.Count() > 0)
                {
                    if (strProcessName == strMainProcess)
                        alreadyKilled = false;
                }

                try
                {
                    foreach (Process process in processList)
                    {
                        process.Kill();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    strResultMessage = ex.Message;
                    return false;
                }
            }

            return true;
        }

        private static CommandResultType StopProcess(string strParameter, ref string strErrorMessage)
        {
            strErrorMessage = "";

            if (strParameter == null)
                return CommandResultType.StopFail;

            strParameter = strParameter.Trim();
            string strProcessName = "";

            int nIndex = strParameter.LastIndexOf('.');

            if (nIndex >= 0)
                strProcessName = strParameter.Substring(0, nIndex);
            else
                strProcessName = strParameter;

            Process[] processList = Process.GetProcessesByName(strProcessName);

            if (processList == null)
                return CommandResultType.StopSuccess;

            try
            {
                foreach (Process process in processList)
                {
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                strErrorMessage = ex.Message;
                return CommandResultType.StopFail;
            }

            return CommandResultType.StopSuccess;
        }

        private static CommandResultType RunProcess(string strParameter, string strClientBaseFolder, ref string strErrorMessage)
        {
            strErrorMessage = "";

#if !SERVICE
            if (strParameter == null)
                return CommandResultType.StartFail;

            strParameter = strParameter.Trim();

            int nIndex = strParameter.IndexOf(' ');
            string strFileName = "", strParam = "";

            if (nIndex >= 0)
            {
                strFileName = strParameter.Substring(0, nIndex);
                strParam = strParameter.Substring(nIndex + 1).Trim();
            }
            else
                strFileName = strParameter;

            if (strFileName.ToLower().EndsWith(".exe") == false)
                strFileName += ".exe";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = strClientBaseFolder + "\\" + strFileName;

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                strErrorMessage = ex.Message;
                return CommandResultType.StartFail;
            }
#endif
            return CommandResultType.StartSuccess;
        }
    }
}
