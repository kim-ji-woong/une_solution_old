using System;
using System.Collections.Generic;
using System.Text;

namespace Weather.BLL
{
    using IDAL;

    public class ProcessManager
    {
        private IDataManager m_dataManager = null;

        private LoadManager m_loadManager = null;

        public ProcessManager(IDataManager dataManager)
        {
            this.m_dataManager = dataManager;

            m_loadManager = new LoadManager(m_dataManager, this);
        }

        public LoadManager GetLoadManager()
        {
            return m_loadManager;
        }
    }
}
