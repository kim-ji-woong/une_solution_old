using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DBUtility;

namespace RestoreService
{
	public partial class SOPRestore : ServiceBase
	{
		private NetworkManager m_Network = null;
		private WebDBManager m_dbMgr = null;
		

		public SOPRestore()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
            int nSiteID = ReadSiteID();

            m_dbMgr = new WebDBManager(nSiteID);


			m_Network = new NetworkManager(m_dbMgr);	
		}

        private int ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                return 1;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                return nSiteId;
            }
            return 1;
        }

		protected override void OnStop()
		{
			m_Network.ReleaseThread();

			if (m_Network.RestoreThread != null)
			{
				m_Network.RestoreThread.Join();
			}
		}		
	}
}
