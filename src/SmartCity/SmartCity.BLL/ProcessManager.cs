using System;

namespace SmartCity.BLL
{
    public class ProcessManager
    {
        private AccountManager m_accountManager = null;
        public AccountManager GetAccountManager()
        {
            return m_accountManager;
        }

        private LoadManager m_loadManager = null;
        public LoadManager GetLoadManager()
        {
            return m_loadManager;
        }

        public ProcessManager(SmartCity.IDAL.IDataManager dataManager)
        {
            m_accountManager = new AccountManager(dataManager);
            m_loadManager = new LoadManager(dataManager);
        }
    }
}
