using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using System.Configuration;
using System.Threading;

namespace AutoUpdater
{
    using Data;

    partial class UpdaterService : ServiceBase
    {
        private WebDBManagerEx m_dbMgr = null;
        private bool m_closeSystem = false;
        private int m_nLocalSiteID = 0;

        private string m_strSOPSystemFolder = "";

        public UpdaterService()
        {
            InitializeComponent();
            SetSystemFolder();   
        }

        private void SetSystemFolder()
        {
            m_strSOPSystemFolder = System.Reflection.Assembly.GetEntryAssembly().Location;

            int nIndex = m_strSOPSystemFolder.LastIndexOf('\\');

            if (nIndex > 0)
            {
                m_strSOPSystemFolder = m_strSOPSystemFolder.Substring(0, nIndex);

                nIndex = m_strSOPSystemFolder.LastIndexOf('\\');

                if (nIndex > 0)
                {
                    m_strSOPSystemFolder = m_strSOPSystemFolder.Substring(0, nIndex);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            RunMonitoringThread();
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
        }

        private void RunMonitoringThread()
        {
            string strOriginalSiteID = ConfigurationManager.AppSettings.Get("siteid");
            string strLocalSiteID = ConfigurationManager.AppSettings.Get("localSiteid");
            string strDBName = ConfigurationManager.AppSettings.Get("dbname");
            string strLocalDBName = ConfigurationManager.AppSettings.Get("localDBname");
            string strURL = ConfigurationManager.AppSettings.Get("url");

            if (strOriginalSiteID == null || strOriginalSiteID.Length == 0 || strDBName == null ||
                strDBName.Length == 0 || strLocalSiteID == null || strLocalSiteID.Length == 0 ||
                strURL == null || strURL.Length == 0 || strLocalDBName == null || strLocalDBName.Length == 0)
                return;

            int nOriginalSiteID, nLocalSiteID;

            if (int.TryParse(strOriginalSiteID, out nOriginalSiteID) == false)
                return;
            if (int.TryParse(strLocalSiteID, out nLocalSiteID) == false)
                return;

            m_nLocalSiteID = nLocalSiteID;
            m_dbMgr = new WebDBManagerEx(m_nLocalSiteID);

            m_dbMgr.WebServerURL = strURL;
            m_dbMgr.DatabaseName = strDBName;
            m_dbMgr.DatabaseType = WebDBManager.DBType.sqlserver;
            m_dbMgr.OriginalSiteID = nOriginalSiteID;
            m_dbMgr.LocalDBName = strLocalDBName;

            DataManager.LoadData(m_dbMgr);
            AlarmManager.RunAlarmMonitoring(m_dbMgr);

            Thread t = new Thread(new ThreadStart(DoMonitoring));
            t.Start();
        }

        private void DoMonitoring()
        {
            while (m_closeSystem == false)
            {
                ReadCommand();
                Thread.Sleep(1000);
            }
        }

        private void ReadCommand()
        {
            string strSQL = "Select TimeStamp, ServerCommand, ClientCommand, ServerParameter, ClientParameter from AutoUpdate where ID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 5)
                return;

            VariousData<DateTime> time = WebDBManager.GetDateTimeField(arrResult[0]);
            VariousData<int> serverCmd = WebDBManager.GetIntField(arrResult[1].ToString());
            VariousData<int> clientCmd = WebDBManager.GetIntField(arrResult[2].ToString());
            string strServerParam = WebDBManager.GetStringField(arrResult[3]);
            string strClientParam = WebDBManager.GetStringField(arrResult[4]);

            if (time == null)
                return;

            string strResultMessage = "";

            if (serverCmd == null && clientCmd == null)
                return;
            else if (serverCmd == null)
            {
                CommandManager.CommandResultType result = ClientCommandManager.ProcessCommand(clientCmd.Data, strClientParam, m_strSOPSystemFolder, m_dbMgr, ref strResultMessage);
                UpdateClientHistory(result, time.Data, strResultMessage);
            }
            else if (clientCmd == null)
            {
                CommandManager.CommandResultType result = ServerCommandManager.ProcessCommand(serverCmd.Data, strServerParam, m_dbMgr, ref strResultMessage);
                UpdateServerHistory(result, time.Data, strResultMessage);
            }
            else
            {
                string strClientResultMessage = "";
                CommandManager.CommandResultType serverResult = ServerCommandManager.ProcessCommand(serverCmd.Data, strServerParam, m_dbMgr, ref strResultMessage);
                CommandManager.CommandResultType clientResult = ServerCommandManager.ProcessCommand(clientCmd.Data, strClientParam, m_dbMgr, ref strClientResultMessage);
                UpdateHistory(serverResult, clientResult, time.Data, strResultMessage, strClientResultMessage);
            }
        }

        private void UpdateServerHistory(CommandManager.CommandResultType result, DateTime dtCommand, string strResultMessage)
        {
            // 처리된 Command는 초기화 시킨다.
            string strSQL = "Update AutoUpdate set ServerCommand = NULL, ServerParameter = NULL where ID = " + m_dbMgr.SiteID.ToString();
            m_dbMgr.GetResultData(strSQL);

            string strCommandTime = DateTimeToString(dtCommand);
            string strExecuteTime = DateTimeToString(DateTime.Now);

            // 처리 결과를 남긴다.
            strSQL = "Insert into AutoUpdateHistory (CommandID, CommandTime, ExecuteTime, ServerResult, ClientResult, ServerMessage, ClientMessage) values ";
            strSQL += string.Format("({0}, '{1}', '{2}', {3}, NULL, '{4}', NULL)", m_dbMgr.SiteID, strCommandTime, strExecuteTime, (int)result, strResultMessage);

            m_dbMgr.GetResultData(strSQL);
        }

        private void UpdateClientHistory(CommandManager.CommandResultType result, DateTime dtCommand, string strResultMessage)
        {
            // 처리된 Command는 초기화 시킨다.
            string strSQL = "Update AutoUpdate set ClientCommand = NULL, ClientParameter = NULL where ID = " + m_dbMgr.SiteID.ToString();
            m_dbMgr.GetResultData(strSQL);

            string strCommandTime = DateTimeToString(dtCommand);
            string strExecuteTime = DateTimeToString(DateTime.Now);

            // 처리 결과를 남긴다.
            strSQL = "Insert into AutoUpdateHistory (CommandID, CommandTime, ExecuteTime, ServerResult, ClientResult, ServerMessage, ClientMessage) values ";
            strSQL += string.Format("({0}, '{1}', '{2}', NULL, {3}, NULL, '{4}')", m_dbMgr.SiteID, strCommandTime, strExecuteTime, (int)result, strResultMessage);

            m_dbMgr.GetResultData(strSQL);
        }

        private void UpdateHistory(CommandManager.CommandResultType serverResult, CommandManager.CommandResultType clientResult, DateTime dtCommand, string strServerResultMessage, string strClientResultMessage)
        {
            // 처리된 Command는 초기화 시킨다.
            ClearCommand();

            string strCommandTime = DateTimeToString(dtCommand);
            string strExecuteTime = DateTimeToString(DateTime.Now);

            // 처리 결과를 남긴다.
            string strSQL = "Insert into AutoUpdateHistory (CommandID, CommandTime, ExecuteTime, ServerResult, ClientResult, ServerMessage, ClientMessage) values ";
            strSQL += string.Format("({0}, '{1}', '{2}', {3}, {4}, '{5}', '{6}')",
                m_dbMgr.SiteID, strCommandTime, strExecuteTime,
                (int)serverResult, (int)clientResult, strServerResultMessage, strClientResultMessage);

            m_dbMgr.GetResultData(strSQL);
        }

        private void ClearCommand()
        {
            // 처리된 Command는 초기화 시킨다.
            string strSQL = "Update AutoUpdate set ServerCommand = NULL, ClientCommand = NULL, ServerParameter = NULL, ClientParameter = NULL where ID = " + m_dbMgr.SiteID.ToString();
            m_dbMgr.GetResultData(strSQL);
        }

        private string DateTimeToString(DateTime time)
        {
            return string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }
    }
}
