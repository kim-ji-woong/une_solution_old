using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DBUtility;
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

        private NetworkManager m_Network = null;
        public NetworkManager Network
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
        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
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

            ReadSiteID();

            m_dbMgr = new WebDBManager(m_nSiteID);
            m_ioMgr = new IOManager(m_nSiteID);
            m_Network = new NetworkManager(m_dbMgr, null, m_nSiteID);

            m_Network.CreateReciverProvider();
		}		

		protected override void OnStop()
		{
			m_Network.ReleaseThread();           
		}		
	}
}
