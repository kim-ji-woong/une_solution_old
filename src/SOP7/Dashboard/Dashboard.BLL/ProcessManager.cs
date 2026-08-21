using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.BLL
{
    public class ProcessManager
    {
        private Dashboard.IDAL.IDataManager m_dashboardDataManager = null;

        private LoadManager m_loadManager = null;

        public ProcessManager(Dashboard.IDAL.IDataManager dashboardDataManager)
        {
            this.m_dashboardDataManager = dashboardDataManager;

            m_loadManager = new LoadManager(m_dashboardDataManager, this);

        }

        public Dashboard.IDAL.IDataManager DashboardDataManager
        {
            get { return m_dashboardDataManager; }
        }

        public LoadManager GetLoadManager()
        {
            return m_loadManager;
        }
    }
}
