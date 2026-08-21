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
		protected override void OnStart(string[] args)
		{
            try
            {
                log4net.Config.DOMConfigurator.Configure();
            }
            catch (System.Exception ex)
            {

            }

            m_dbMgr = new WebDBManager(m_nSiteID);
            m_dbMgr.WebServerURL = "http://127.0.0.1:8080/SOP";
            m_Network = new NetworkManager(m_dbMgr, null, m_nSiteID);

            m_Network.CreateReciverProvider();
		}		

		protected override void OnStop()
		{
			m_Network.ReleaseThread();           
		}		
	}
}
