namespace SOPManager.BLL
{
    public class ProcessManager
    {
        private LoadManager m_loadManager = null;
        private SaveManager m_saveManager = null;
        private DeleteManager m_deleteManager = null;
        private OptionManager m_optionManager = null;
        private AccountManager m_accountManager = null;

        private SOPManager.IDAL.IDataManager m_sopDataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private TeamEditor.IDAL.IDataManager m_teamDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;
        private INetworkManager m_netMgr = null;

        public INetworkManager NetworkManager
        {
            get { return m_netMgr; }
            set { m_netMgr = value; }
        }

        public SOPManager.IDAL.IDataManager SopDataManager
        {
            get { return m_sopDataManager; }
        }

        public Common.IDAL.IDataManager CommonDataManager
        {
            get { return m_commonDataManager; }
        }

        public TeamEditor.IDAL.IDataManager TeamDataManager
        {
            get { return m_teamDataManager; }
        }

        public SDMS.IDAL.IDataManager SDMSDataManager
        {
            get { return m_sdmsDataManager; }
        }

        public ProcessManager(Common.IDAL.IDataManager commonDataManager, SOPManager.IDAL.IDataManager sopDataManager, TeamEditor.IDAL.IDataManager teamDataManager, SDMS.IDAL.IDataManager sdmsDataManager)
        {
            Resource.ID.Init();

            m_commonDataManager = commonDataManager;
            m_sopDataManager = sopDataManager;
            m_teamDataManager = teamDataManager;
            m_sdmsDataManager = sdmsDataManager;

            m_loadManager = new LoadManager(m_sopDataManager, this);
            m_saveManager = new SaveManager(m_sopDataManager, m_teamDataManager, this);
            m_deleteManager = new DeleteManager(m_commonDataManager, m_sopDataManager, this);
            m_optionManager = new OptionManager(m_commonDataManager, m_sopDataManager, this);
            m_accountManager = new AccountManager(m_sopDataManager, m_teamDataManager, m_commonDataManager, m_sdmsDataManager, this);
        }

        public LoadManager GetLoadManager()
        {
            return m_loadManager;
        }

        public SaveManager GetSaveManager()
        {
            return m_saveManager;
        }

        public DeleteManager GetDeleteManager()
        {
            return m_deleteManager;
        }

        public OptionManager GetOptionManager()
        {
            return m_optionManager;
        }

        public AccountManager GetAccountManager()
        {
            return m_accountManager;
        }
    }
}
