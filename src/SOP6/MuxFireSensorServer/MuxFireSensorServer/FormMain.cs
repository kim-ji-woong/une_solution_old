using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using DBUtility2;

namespace MuxFireSensorServer
{
    using Network;
    using Data;

    public partial class FormMain : Form
    {
        private NetworkManager m_netMgr = null;
        private NetworkWebClient m_netWebClient = null;
        private DirectDBManagerEx m_dbMgr = null;
        private Network.Relay.Server m_relayServer = null;

        public FormMain()
        {
            InitializeComponent();

            string strIP = System.Configuration.ConfigurationManager.AppSettings.Get("ip");
            string strPort = System.Configuration.ConfigurationManager.AppSettings.Get("Port");
            string strMuxType = System.Configuration.ConfigurationManager.AppSettings.Get("NMuxType");
            string strLogFile = System.Configuration.ConfigurationManager.AppSettings.Get("logFile");
            string strLogFolder = System.Configuration.ConfigurationManager.AppSettings.Get("logFolder");
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings.Get("site");
            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings.Get("webServerURL");
            string strDBName = System.Configuration.ConfigurationManager.AppSettings.Get("dbName");
            string strDBType = System.Configuration.ConfigurationManager.AppSettings.Get("dbType");
            string strRelayPort = System.Configuration.ConfigurationManager.AppSettings.Get("relayPort");

            int index = strWebServerURL.IndexOf("//");

            if (index > 0)
                strWebServerURL = strWebServerURL.Substring(index + 2).Trim();

            m_netMgr = new NetworkManager(strIP, strPort, strMuxType, strLogFolder, strLogFile);

            int nSiteID, nDBType;

            if (int.TryParse(strSiteID, out nSiteID) && int.TryParse(strDBType, out nDBType))
            {
                string strID, strPW;

                if (DirectDBManagerEx.GetDBInfo(out strID, out strPW))
                {
                    DirectDBManager dbMgr = DirectDBManager.MakeInstance((DirectDBManager.DBType)nDBType, strWebServerURL, strID, strPW, strDBName);
                    dbMgr.SiteID = nSiteID;

                    m_dbMgr = new DirectDBManagerEx(dbMgr);
                    
                    SensorManager.Instance.LoadData(m_dbMgr);
                    m_netWebClient = new NetworkWebClient(m_dbMgr, strLogFolder, strLogFile);
                }
            }

            int nRelayPort;

            if (int.TryParse(strRelayPort, out nRelayPort))
            {
                m_relayServer = new Network.Relay.Server(nRelayPort);
                ClientProvider.RelayServer = m_relayServer;
            }
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
            m_netWebClient.Close();

            if (m_relayServer != null)
                m_relayServer.StopServer();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            int nReceiver = int.Parse(textBoxReceiver.Text.Trim());
            int nLoop = int.Parse(textBoxLoop.Text.Trim());
            int nRelay = int.Parse(textBoxRelay.Text.Trim());
            int nTag = int.Parse(textBoxTag.Text.Trim());
            bool on = checkBoxAlarm.Checked;

            m_netMgr.ProcessFire(nReceiver, nLoop, nRelay, nTag, on);
        }
    }
}
