using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;
using System.IO;
using System.Timers;

namespace ServerAgent
{
    partial class AgentService : ServiceBase
    {
        public enum CommandType { NONE = -1, AGENT_UPDATE = 0, GET_SERVICE_LIST, GET_FILE_LIST, UPDATE, GET_PROC_LIST, GET_ALL_PROC_LIST, DOWNLOAD, SDMS_UPDATE }
        public enum StatusType { UNKNOWN = -1, STOP = 0, RUN };

        private WebDBManager m_dbMgr = null;
        private Timer m_timer = null;
        private bool m_runningProcess = false;
        private int m_nSiteID = 1;

        /*private StreamWriter m_writer = new StreamWriter("C:/temp/test.log", false, System.Text.Encoding.UTF8);

        private void WriteLog(string strLog)
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);
            m_writer.WriteLine(strTime + " : " + strLog);
            m_writer.Flush();
        }*/

        public AgentService()
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
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            m_timer = new Timer(1000);
            m_timer.AutoReset = true;
            m_timer.Elapsed += new ElapsedEventHandler(OnTimer);
            m_timer.Start();
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            m_timer.Stop();
        }

        private void OnTimer(object sender, EventArgs e)
        {
            ReadServiceStatus();

            if (m_runningProcess)
                return;

            m_runningProcess = true;
            ReadCommand();
            m_runningProcess = false;
        }

        private void ReadServiceStatus()
        {
            string strSQL = "Select ID, ServiceName, Status from SDMSServiceList where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strServiceName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> status = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (id == null || strServiceName == null || status == null)
                    continue;

                try
                {
                    System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(strServiceName);

                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        if (status.Data != (int)StatusType.STOP)
                            UpdateServiceStatus(id.Data, (int)StatusType.STOP);
                    }
                    else if (service.Status == ServiceControllerStatus.Running)
                    {
                        if (status.Data != (int)StatusType.RUN)
                            UpdateServiceStatus(id.Data, (int)StatusType.RUN);
                    }

                    service.Dispose();
                }
                catch (Exception)
                {
                }
            }
        }

        private void UpdateServiceStatus(int nID, int nStatus)
        {
            string strSQL = string.Format("Update SDMSServiceList set Status = {0} where ID = {1}", nStatus, nID);
            m_dbMgr.GetResultData(strSQL, 0);
        }

        private void ReadCommand()
        {
            string strSQL = "SELECT ID, Command, TimeStamp, IsStop, IsStopService, StopName, IsStart, IsStartService, StartName FROM SDMSCommand";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> cmd = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 2]);
                VariousData<int> isStop = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> isStopService = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                string strStopName = WebDBManager.GetStringField(arrResult[i + 5]);
                VariousData<int> isStart = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<int> isStartService = WebDBManager.GetIntField(arrResult[i + 7].ToString());
                string strStartName = WebDBManager.GetStringField(arrResult[i + 8]);

                if (id == null || cmd == null)
                    continue;

                if (cmd.Data == (int)CommandType.UPDATE)
                {
                    if (timeStamp == null)
                        timeStamp = new VariousData<DateTime>(DateTime.Now);

                    bool result = true;

                    if (isStop != null && isStopService != null && strStopName != null)
                    {
                        if (isStop.Data == 1 && isStopService.Data == 1)
                        {
                            if (StopService(strStopName) == false)
                                result = false;
                        }
                    }

                    if (isStart != null && isStartService != null && strStartName != null)
                    {
                        if (isStart.Data == 1 && isStartService.Data == 1)
                        {
                            if (StartService(strStartName) == false)
                                result = false;
                        }
                    }

                    RemoveCommand(id.Data, cmd.Data, timeStamp.Data, isStop, isStopService, strStopName, isStart, isStartService, strStartName, result);
                }
            }
        }

        // 서비스가 실제로 종료되었는지 확인한다.
        // timeout : milli seconds
        private bool CheckStopService(string strServiceName, int timeout)
        {
            int nSleep = 200;

            for (int i = 0; i < timeout; i += nSleep)
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(strServiceName);

                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Dispose();
                    return true;
                }

                service.Dispose();
                System.Threading.Thread.Sleep(nSleep);
            }

            return false;
        }

        private void RemoveCommand(int nID, int nCommand, DateTime timeStamp, VariousData<int> isStop, VariousData<int> isStopService, string strStopName, VariousData<int> isStart, VariousData<int> isStartService, string strStartName, bool result)
        {
            string strIsStop = isStop == null ? "NULL" : isStop.Data.ToString();
            string strIsStopService = isStopService == null ? "NULL" : isStopService.Data.ToString();
            string strIsStart = isStart == null ? "NULL" : isStart.Data.ToString();
            string strIsStartService = isStartService == null ? "NULL" : isStartService.Data.ToString();

            if (strStopName == null)
                strStopName = "NULL";
            else
                strStopName = "'" + strStopName + "'";

            if (strStartName == null)
                strStartName = "NULL";
            else
                strStartName = "'" + strStartName + "'";

            string strTimeStamp = ToTimeString(timeStamp);
            string strExecuteTime = ToTimeString(DateTime.Now);

            string strSQL = "Insert into SDMSCommandHistory (ID, Command, TimeStamp, ExecuteTime, SearchPath, IsStop, IsStopService, StopName, IsUpdate, UpdateName, IsStart, IsStartService, StartName, Result) ";
            strSQL += string.Format("values ((select isnull(max(id)+1,1) from SDMSCommandHistory), {0}, '{1}', '{2}', NULL, {3}, {4}, {5}, NULL, NULL, {6}, {7}, {8}, {9})",
                nCommand, strTimeStamp, strExecuteTime, strIsStop, strIsStopService, strStopName,
                strIsStart, strIsStartService, strStartName, result ? 1 : 0);

            if (m_dbMgr.GetResultData(strSQL, 0) != null)
            {
                strSQL = "Delete from SDMSCommand where ID = " + nID.ToString();
                m_dbMgr.GetResultData(strSQL, 0);
            }
        }

        private string ToTimeString(DateTime time)
        {
            return string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }

        private bool StartService(string strServiceName)
        {
            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(strServiceName);
                service.Start();

                TimeSpan timeout = TimeSpan.FromMilliseconds(1000);
                service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, timeout);
                service.Dispose();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool StopService(string strServiceName)
        {
            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(strServiceName);
                service.Stop();
                service.Dispose();

                // Timeout : 30초
                if (CheckStopService(strServiceName, 30000) == false)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
