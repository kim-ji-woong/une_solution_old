using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.ServiceProcess;
using SOPChecker;

namespace ServerRestart
{
    public partial class Form1 : Form
    {
        private Timer _timer = null;
        private Timer _timerKill = null;
        private int _timerKillInterval = 0;
        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = 1;

        public Form1()
        {
            InitializeComponent();

            Utility ini = new Utility();

            string strSection = "Server Connection Info";
            string strWebServerURL = ini.getinivalue(strSection, "webserver_url");
            string strDBName = ini.getinivalue(strSection, "server_db");
            string strSiteID = ini.getinivalue(strSection, "siteid");
            string strServerPort = ini.getinivalue(strSection, "server_port");

            int.TryParse(strSiteID, out m_nSiteID);

            m_dbMgr = new WebDBManager(m_nSiteID);
            m_dbMgr.WebServerURL = strWebServerURL;
            m_dbMgr.DatabaseName = strDBName;
            m_dbMgr.DatabaseHost = "127.0.0.1";

            if (strServerPort == "1433")
                m_dbMgr.DatabaseType = WebDBManager.DBType.sqlserver;
            else if (strServerPort == "3306")
                m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;

            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += _timer_Tick;
            _timer.Start();

            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            if (IsStartAgent())
            {                
                Application.Exit();
            }

            ReadCommand();
        }

        private int ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID != null && szSiteID.Length > 0)
            {
                int nSiteId = 1;
                if (int.TryParse(szSiteID, out nSiteId))
                    return nSiteId;
            }
            return -1;
        }

        /// <summary>
        /// Agent가 실행되었는지 감시
        /// Agent 실행되면 Kill
        /// </summary>
        /// <returns></returns>
        private bool IsStartAgent()
        {
            Process[] proc = Process.GetProcessesByName("SDMSAgent");
            if (proc != null && proc.Length > 0)
                return true;

            return false;
        }

        private void ReadCommand()
        {
            string strSQL = "Select ID, Command, TimeStamp, SearchPath, IsStop, IsStopService, StopName, IsUpdate, UpdateName, IsStart, IsStartService, StartName from SDMSCommand where Command in (8,9) ";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            
            if (arrResult == null)
                return;

            if (arrResult.Count == 0)
                return;
            
            int nResultCount = arrResult.Count;
            
            for (int i = 0; i < nResultCount; i += 12)
            {
                nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nCommand = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                string strSearchPath = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                int nIsStop = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nIsStopService = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strStopName = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");
                int nIsUpdate = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                string strUpdateName = WebDBManager.GetStringField(arrResult[i + 8].ToString(), "");
                int nIsStart = WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);
                int nIsStartService = WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                string strStartName = WebDBManager.GetStringField(arrResult[i + 11].ToString(), "");

                SDMSAgent.CommandItem cmd = new SDMSAgent.CommandItem();                
                cmd.CmdType = (SDMSAgent.CommandType)nCommand;                
                cmd.TimeStamp = timeStamp.Data;
                cmd.SearchPath = strSearchPath;
                cmd.IsStop = (nIsStop == 0) ? false : true;
                cmd.IsStopService = (nIsStopService == 0) ? false : true;
                cmd.StopName = strStopName;
                cmd.IsUpdate = (nIsUpdate == 0) ? false : true;
                cmd.UpdateName = strUpdateName;
                cmd.IsStart = (nIsStart == 0) ? false : true;
                cmd.IsStartService = (nIsStartService == 0) ? false : true;
                cmd.StartName = strStartName;
                
                if (Execute(cmd))
                {

                }

                nID = -1;
                nResult = -1;
            }
        }

        private int nID = -1;
        private int nResult = -1;
        public bool Execute(SDMSAgent.CommandItem commandItem)
        {
            RemoveCommand(commandItem);

            try
            {
                SDMSAgent.CommandType type = commandItem.CmdType;

                if (type == SDMSAgent.CommandType.SOP_SERVER_RESTART)
                {
                    if (commandItem.IsStartService)
                    {
                        if (!SOPChecker.ServiceManager.IsRunningSerivce(commandItem.StartName))
                            ServiceStart(commandItem.StartName);
                        else
                            RestartService(commandItem.StartName, 5000);
                    }

                    if (SOPChecker.ServiceManager.IsRunningSerivce(commandItem.StartName))
                        nResult = 1;
                    else
                        nResult = 0;
                }
                else if (type == SDMSAgent.CommandType.SERVER_STATUS)
                {
                    // 1: 실행중
                    nResult = GetServerStatus(commandItem);
                }
            }
            catch (Exception)
            {
                nResult = 0;
            }
            finally
            {
                InsertCommandHistory(commandItem);
            }

            return true;
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

                Trace.WriteLine("[ERROR] RestartService Stop: " + e.Message);
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
                Trace.WriteLine("[ERROR] RestartService Start: " + e.Message);
            }

        }

        private bool InsertCommandHistory(SDMSAgent.CommandItem commandItem)
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
                    , nResult);

                m_dbMgr.GetResultData(sb.ToString(), 0);

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[ERROR] InsertCommandHistory : " + ex.Message);
                return false;
            }
        }
        private bool RemoveCommand(SDMSAgent.CommandItem commandItem)
        {
            try
            {
                string strSQL = "DELETE FROM SDMSCommand where ID = " + nID;
                m_dbMgr.GetResultData(strSQL, 0);

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[ERROR] RemoveCommand : " + ex.Message);
                return false;
            }
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
                Trace.WriteLine("[INFO] Kill " + strProcName);

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
                    Trace.WriteLine("[INFO] Process Start : " + strFolderPath + strFileName);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    Trace.WriteLine("[ERROR] Process Start : " + ex.Message);
                    return false;
                }

                return true;
            }
            else
            {
                Trace.WriteLine("[ERROR] Process Start : 경로 확인");
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

            try
            {
                if (SOPChecker.ServiceManager.IsRunningSerivce(strServiceName))
                {
                    SOPChecker.ServiceManager.StopService(strServiceName, 5000);
                    Trace.WriteLine("[INFO] Service Stop : " + strServiceName);
                }
            }
            catch (Exception)
            {
                Process[] proc = Process.GetProcessesByName(strServiceName);
                if (proc != null && proc.Length > 0)
                    proc[0].Kill();

                Trace.WriteLine(strServiceName + " : 프로세스 강제종료");
            }

            return true;
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
                Trace.WriteLine("[INFO] Service Start : " + strServiceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                Trace.WriteLine("[ERROR] Service Start : " + ex.Message);
                return false;
            }

            return true;
        }
        #endregion

        private bool GetFolderNFile2(string strSrc, ref string strFolderPath, ref string strFileName)
        {
            int nIndex = strSrc.LastIndexOf("\\");

            if (nIndex < 0)
                return false;

            strFolderPath = strSrc.Substring(0, nIndex);
            strFileName = strSrc.Substring(nIndex + 1);
            return true;
        }

        private int GetServerStatus(SDMSAgent.CommandItem commandItem)
        {
            if (commandItem.SearchPath.Length == 0)
                return -1;

            if (commandItem.IsStartService)
            {
                if (ServiceManager.IsRunningSerivce(commandItem.SearchPath))
                    return 1;
            }
            else
            {
                System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName(commandItem.SearchPath);
                if (process.Length > 0)
                    return 1;
            }

            return 0;
        }
    }
}
