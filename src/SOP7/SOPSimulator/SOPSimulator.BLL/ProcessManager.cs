namespace SOPSimulator.BLL
{
    public class ProcessManager
    {
        private LoadManager m_loadManager = null;
        private CreateManager m_createManager = null;
        private UpdateManager m_updateManager = null;
        private SMSManager m_smsManager = null;
        private SOPRunManager m_sopRunManager = null;

        private SOPManager.IDAL.IDataManager m_sopDataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private TeamEditor.IDAL.IDataManager m_teamDataManager = null;
        private SOPSimulator.IDAL.IDataManager m_sopSimulatorDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsManager = null;

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

        public SOPSimulator.IDAL.IDataManager SopSimulatorDataManager
        {
             get { return m_sopSimulatorDataManager; }
        }

        public SDMS.IDAL.IDataManager SdmsManager
        {
            get { return m_sdmsManager; }
        }

        public ProcessManager(Common.IDAL.IDataManager commonDataManager, 
            SOPManager.IDAL.IDataManager sopDataManager, 
            TeamEditor.IDAL.IDataManager teamDataManager, 
            SOPSimulator.IDAL.IDataManager sopSimulatorDataManager,
            SDMS.IDAL.IDataManager sdmsManager)
        {
            m_commonDataManager = commonDataManager;
            m_sopDataManager = sopDataManager;
            m_teamDataManager = teamDataManager;
            m_sopSimulatorDataManager = sopSimulatorDataManager;
            m_sdmsManager = sdmsManager;
            
            m_loadManager = new LoadManager(this);
            m_createManager = new CreateManager(this);
            m_updateManager = new UpdateManager(this);
            m_smsManager = new SMSManager(this);

            m_sopRunManager = new SOPRunManager(this);
        }

        public LoadManager GetLoadManager()
        {
            return m_loadManager;
        }

        public CreateManager GetCreateManager()
        {
            return m_createManager;
        }

        public UpdateManager GetUpdateManager()
        {
            return m_updateManager;
        }

        public SMSManager GetSMSManager()
        {
            return m_smsManager;
        }

        public SOPRunManager GetSopRunManager()
        {
            return m_sopRunManager;
        }
    }
}
