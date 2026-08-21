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
using System.IO;

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
        //private NetworkManager m_Network = null;
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
        private void ReadSiteID(ref string serverDB, ref string WebserverURL, ref WebDBManager.DBType dbType)
        {
            Utility util = new Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            serverDB = util.getinivalue("Server Connection Info", "server_db");
            WebserverURL = util.getinivalue("Server Connection Info", "webserver_url");
            string port = util.getinivalue("Server Connection Info", "server_port");
            
            if (szSiteID == null || szSiteID == "")
            {
                return;
            }
            if (serverDB == null)
                return;

            if (port == "3306")
                dbType = WebDBManager.DBType.mysql;
            else if (port == "1433")
                dbType = WebDBManager.DBType.sqlserver;

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }
            else
            {
                return;
            }
        }

        private UnE.Log.LogFileCleanupTask m_CleanUpTask = null;

		protected override void OnStart(string[] args)
		{
            try
            {
                m_CleanUpTask = new UnE.Log.LogFileCleanupTask();
                m_CleanUpTask.CleanUp();
                m_CleanUpTask.BeginDailyTask(m_CleanUpTask.CleanUp);
            }
            catch (System.Exception)
            {

            }

            string serverDB = "";
            string webserverURL = "";
            WebDBManager.DBType dbType = WebDBManager.DBType.sqlserver;
            ReadSiteID(ref serverDB, ref webserverURL, ref dbType);

            m_dbMgr = new WebDBManager(m_nSiteID);
            m_dbMgr.WebServerURL = webserverURL;
            m_dbMgr.DatabaseName = serverDB;
            m_dbMgr.DatabaseType = dbType;

            m_ioMgr = new IOManager(m_nSiteID);
            m_Network = new NetworkWebManager(m_dbMgr);
            //m_Network = new NetworkManager(m_dbMgr, null, m_nSiteID);
            m_Network.CreateReciverProvider();
		}

        protected override void OnStop()
        {
            m_Network.ReleaseThread();
        }
	}
}
