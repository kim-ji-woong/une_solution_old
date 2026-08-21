using System;
using System.Windows.Forms;

namespace MuxFireSensorServer
{
    using dnsDBUtil;
    using Network;

    public partial class FormMain : Form
    {
        private NetworkManager m_netMgr = null;
        private NetworkWebClient m_netWebClient = null;
        private WebDBManager m_dbMgr = null;
        private Network.Relay.Server m_relayServer = null;
        private Timer m_timer = null;
        public FormMain()
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

        private void M_timer_Tick(object sender, EventArgs e)
        {
            Logger.Instance.RemoveOldLogs();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_netMgr.Start();

            if (m_relayServer != null)
                m_relayServer.BeginServer();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netMgr.Stop();
            
            if (m_relayServer != null)
                m_relayServer.StopServer();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            int nReceiver = int.Parse(textBoxReceiver.Text.Trim());
            int nRelayTeam = int.Parse(textBoxRelayTeam.Text.Trim());
            int nLoop = int.Parse(textBoxLoop.Text.Trim());
            int nRelay = int.Parse(textBoxRelay.Text.Trim());
            int nTag = int.Parse(textBoxTag.Text.Trim());
            bool on = checkBoxAlarm.Checked;

            m_netMgr.ProcessFire(nReceiver, nRelayTeam, nLoop, nRelay, nTag, on);
        }

        private void btnSensorTagNoAlarm_Click(object sender, EventArgs e)
        {
            int tagNo = int.Parse(txtSensorTagNo.Text.Trim());
            bool on = cbSensorTagNoAlarm.Checked;

            m_netMgr.ProcessFireFromTagNo(tagNo, on);
        }
    }
}
