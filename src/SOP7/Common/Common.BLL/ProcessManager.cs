namespace Common.BLL
{
    public class ProcessManager
    {
        private LoadManager m_loadManager = null;
        private SaveManager m_saveManager = null;
        private OptionManager m_optionManager = null;
        
        private SOPManager.IDAL.IDataManager m_sopDataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private TeamEditor.IDAL.IDataManager m_teamDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;

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

        public SDMS.IDAL.IDataManager SdmsDataManager
        {
            get { return m_sdmsDataManager; }
        }

        public ProcessManager(Common.IDAL.IDataManager commonDataManager, SOPManager.IDAL.IDataManager sopDataManager, TeamEditor.IDAL.IDataManager teamDataManager, SDMS.IDAL.IDataManager sdmsDataManager)
        {
            m_commonDataManager = commonDataManager;
            m_sopDataManager = sopDataManager;
            m_teamDataManager = teamDataManager;
            m_sdmsDataManager = sdmsDataManager;

            m_loadManager = new LoadManager(this);
            m_saveManager = new SaveManager(m_commonDataManager, this);
            m_optionManager = new OptionManager(this);
        }

        public LoadManager GetLoadManager()
        {
            return m_loadManager;
        }

        public SaveManager GetSaveManager()
        {
            return m_saveManager;
        }

        public OptionManager GetOptionManager()
        {
            return m_optionManager;
        }
    }
}
