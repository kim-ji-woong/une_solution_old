using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;

namespace ETCSensorServer
{
    using Network;
    using Data;

    public partial class FormMain : Form, IServiceOwner
    {        
        //private UnE.Log.LogFileCleanupTask m_CleanUpTask = null;
        private NetworkWebManager m_netMgr = null;
        private ClientManager m_clientMgr = null;
        private WebDBManager m_dbMgr = null;
        private SensorManager m_sensorMgr = null;

        public FormMain()
        {
            InitializeComponent();

            string serverDB = "";
            string webserverURL = "";
            WebDBManager.DBType dbType = WebDBManager.DBType.sqlserver;
            int siteID = -1;
            ReadSiteID(ref serverDB, ref webserverURL, ref dbType, ref siteID);

            if (siteID <= 0)
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다.");
                Application.Exit();
                return;
            }

            m_dbMgr = new WebDBManager(siteID);
            /*m_dbMgr.WebServerURL = webserverURL;
            m_dbMgr.DatabaseName = serverDB;
            m_dbMgr.DatabaseType = dbType;*/

            m_sensorMgr = new SensorManager(m_dbMgr);
            m_netMgr = new NetworkWebManager(m_dbMgr);

            int nPort;
            string strPort = System.Configuration.ConfigurationManager.AppSettings.Get("port");
            string strIP = System.Configuration.ConfigurationManager.AppSettings.Get("server");

            if (strIP.Length > 0 && int.TryParse(strPort, out nPort))
            {
                m_clientMgr = new ClientManager(strIP, nPort, this, m_sensorMgr, m_netMgr);
            }
        }

        private void ReadSiteID(ref string serverDB, ref string WebserverURL, ref WebDBManager.DBType dbType, ref int siteID)
        {
            Utility util = new Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");

            serverDB = util.getinivalue("Server Connection Info", "server_db");
            WebserverURL = util.getinivalue("Server Connection Info", "webserver_url");
            string port = util.getinivalue("Server Connection Info", "server_port");

            if (serverDB == null)
                return;

            if (port == "3306")
                dbType = WebDBManager.DBType.mysql;
            else if (port == "1433")
                dbType = WebDBManager.DBType.sqlserver;

            if (szSiteID == null || szSiteID == "")
            {
                MessageBox.Show("Site ID가 지정되지 않았습니다. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                siteID = nSiteId;
            }
            else
            {
                MessageBox.Show("잘못된 Site ID입니다.. ini파일을 확인하세요", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_netMgr.ReleaseThread();
        }

        public void OnConnect()
        {
            System.Diagnostics.Trace.WriteLine("OnConnect");
        }

        public void OnDropConnection()
        {
            System.Diagnostics.Trace.WriteLine("OnDisconnect");
        }
    }
}
