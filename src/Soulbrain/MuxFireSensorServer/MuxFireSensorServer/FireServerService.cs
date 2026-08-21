using System.ServiceProcess;

namespace MuxFireSensorServer
{
    using dnsDBUtil;
    using Network;
    using System.Windows.Forms;

    partial class FireServerService : ServiceBase
    {
        private NetworkManager m_netMgr = null;
        private NetworkWebClient m_netWebClient = null;
        private WebDBManager m_dbMgr = null;
        private Network.Relay.Server m_relayServer = null;
        private Timer m_timer = null;

        public FireServerService()
        {
            InitializeComponent();

            string strIP = System.Configuration.ConfigurationManager.AppSettings.Get("ip");
            string strPort = System.Configuration.ConfigurationManager.AppSettings.Get("Port");
            string strMuxType = System.Configuration.ConfigurationManager.AppSettings.Get("NMuxType");
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings.Get("site");
            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings.Get("webServerURL");
            string strDBName = System.Configuration.ConfigurationManager.AppSettings.Get("dbName");
            string strDBType = System.Configuration.ConfigurationManager.AppSettings.Get("dbType");
            string strRelayPort = System.Configuration.ConfigurationManager.AppSettings.Get("relayPort");

            m_netMgr = new NetworkManager(strIP, strPort, strMuxType);

            int nSiteID, nDBType;

            if (int.TryParse(strSiteID, out nSiteID) && int.TryParse(strDBType, out nDBType))
            {
                m_dbMgr = new WebDBManager(nSiteID);
                m_dbMgr.WebServerURL = strWebServerURL;
                m_dbMgr.DatabaseName = strDBName;
                m_dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;

                SensorManager.Instance.LoadData(m_dbMgr);
                m_netWebClient = new NetworkWebClient();
            }

            int nRelayPort;

            if (int.TryParse(strRelayPort, out nRelayPort))
            {
                m_relayServer = new Network.Relay.Server(nRelayPort);
                ClientProvider.RelayServer = m_relayServer;
            }

            m_timer = new Timer();
            m_timer.Interval = 1000 * 60 * 60;
            m_timer.Tick += M_timer_Tick;
            M_timer_Tick(null, null);
        }

        private void M_timer_Tick(object sender, System.EventArgs e)
        {
            Logger.Instance.RemoveOldLogs();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 여기에 서비스를 시작하는 코드를 추가합니다.
            m_netMgr.Start();

            if (m_relayServer != null)
                m_relayServer.BeginServer();
        }

        protected override void OnStop()
        {
            // TODO: 서비스를 중지하는 데 필요한 작업을 수행하는 코드를 여기에 추가합니다.
            m_netMgr.Stop();
            Logger.Instance.Write("Mux Server Stop");
            if (m_relayServer != null)
            {
                m_relayServer.StopServer();
                Logger.Instance.Write("Mux RelayServer Stop");
            }

            Logger.Instance.Write("Server Stop");
        }
    }
}
