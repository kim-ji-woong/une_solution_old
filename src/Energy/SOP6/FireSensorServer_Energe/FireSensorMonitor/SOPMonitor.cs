using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DBUtility2;
using log4net;

namespace SensorMonitor
{
    public partial class SOPMonitor : ServiceBase
	{
        private IOManager m_ioMgr = null;
        public IOManager IoMgr
        {
            get { return m_ioMgr; }
            set { m_ioMgr = value; }
        }

        private NetworkWebManager m_Network = null;
        public NetworkWebManager Network
        {
            get { return m_Network; }
            set { m_Network = value; }
        }

        private WebDBManager m_dbMgr = null;
        public WebDBManager DbMgr
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        public static SOPMonitor Instance = null;

		public SOPMonitor()
		{
            Instance = this;
			InitializeComponent();
		}


        private int m_nSiteID = 1;
        private void ReadSiteID(out string strWebServerURL, out string strDBName, out WebDBManager.DBType dbType)
        {
            strWebServerURL = strDBName = "";
            dbType = WebDBManager.DBType.mysql;

            Utility util = new Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
             
                return;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                return;
            }

            strWebServerURL = util.getinivalue("Server Connection Info", "webserver_url");
            strDBName = util.getinivalue("Server Connection Info", "server_db");
            string strPort = util.getinivalue("Server Connection Info", "server_port");

            if (strPort == "1433")
                dbType = WebDBManager.DBType.sqlserver;
            else if (strPort == "3306")
                dbType = WebDBManager.DBType.mysql;
        }

        private UnE.Log.LogFileCleanupTask m_CleanUpTask = null;

		protected override void OnStart(string[] args)
		{
            try
            {
                log4net.Config.DOMConfigurator.Configure();

                m_CleanUpTask = new UnE.Log.LogFileCleanupTask();
                m_CleanUpTask.CleanUp();
                m_CleanUpTask.BeginDailyTask(m_CleanUpTask.CleanUp);

            }
            catch (System.Exception)
            {

            }

            string strWebServerURL, strDBName;
            WebDBManager.DBType dbType;
            ReadSiteID(out strWebServerURL, out strDBName, out dbType);

            m_dbMgr = new WebDBManager(m_nSiteID);
            m_dbMgr.WebServerURL = strWebServerURL;
            m_dbMgr.DatabaseName = strDBName;
            m_dbMgr.DatabaseType = dbType;

            m_ioMgr = new IOManager(m_nSiteID);
            m_Network = new NetworkWebManager(m_dbMgr);

            m_Network.CreateReciverProvider();
		}		

		protected override void OnStop()
		{
			m_Network.ReleaseThread();           
		}		
	}
}
