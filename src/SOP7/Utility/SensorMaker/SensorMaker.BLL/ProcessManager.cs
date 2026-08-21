using TeamEditor.IDAL;

namespace SensorMaker.BLL
{
    public class ProcessManager
    {
        private IDataManager m_dataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;
        private SOPManager.IDAL.IDataManager m_sopDataManager = null;
        private AccountManager m_accountManager = null;
        private KakaoManager m_kakaoManager = null;
        private XmlManager m_xmlManager = null;
        private LoadManager m_loadManager = null;

        public ProcessManager(IDataManager dataManager, Common.IDAL.IDataManager commonDataManager, SDMS.IDAL.IDataManager sdmsDataManager, SOPManager.IDAL.IDataManager sopDataManager)
        {
            m_dataManager = dataManager;
            m_commonDataManager = commonDataManager;
            m_sdmsDataManager = sdmsDataManager;
            m_sopDataManager = sopDataManager;
            m_accountManager = new AccountManager(m_dataManager, this);
            m_kakaoManager = new KakaoManager(m_commonDataManager, this);
            m_xmlManager = new XmlManager();
            m_loadManager = new LoadManager(this);
        }

        public Common.IDAL.IDataManager CommonDataManager
        {
            get { return m_commonDataManager; }
        }
        
        public SDMS.IDAL.IDataManager SdmsDataManager
        {
            get { return m_sdmsDataManager; }
        }

        public TeamEditor.IDAL.IDataManager TeamDataManager
        {
            get { return m_dataManager; }
        }

        public SOPManager.IDAL.IDataManager SopDataManager
        {
             get { return m_sopDataManager; }
        }

        public AccountManager GetAccountManager()
        {
            return m_accountManager;
        }

        public XmlManager GetXmlManager()
        {
            return m_xmlManager;
        }

        public LoadManager GetLoadManager()
        {
            return m_loadManager;
        }
    }
}
