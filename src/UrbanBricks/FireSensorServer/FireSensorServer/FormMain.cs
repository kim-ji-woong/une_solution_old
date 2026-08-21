using DBUtility2;
using FireSensorServer.Data;
using FireSensorServer.Network;
using System;
using System.Windows.Forms;

namespace FireSensorServer
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;
        private NetworkWebManager m_netMgr = null;
        private GFSNetworkManager m_gfsMgr = null;
        private AccessControl m_accessCtrl = null;

        private Timer m_timer = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void ReadConfig()
        {
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings["siteid"].ToString();
            int nSiteID = int.Parse(strSiteID);
            m_dbMgr = new WebDBManager(nSiteID);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ReadConfig();

            DataManager.Instance.LoadData(m_dbMgr);
            m_netMgr = new NetworkWebManager(m_dbMgr);
            m_gfsMgr = new GFSNetworkManager(m_netMgr);
            m_gfsMgr.Start();

            m_accessCtrl = new AccessControl(m_dbMgr);

            m_timer = new Timer();
            m_timer.Interval = 1000 * 60 * 60;
            m_timer.Tick += M_timer_Tick;
            M_timer_Tick(null, null);
        }

        private void M_timer_Tick(object sender, System.EventArgs e)
        {
            Logger.Instance.RemoveOldLogs();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_netMgr != null)
                m_netMgr.Close();

            if (m_gfsMgr != null)
                m_gfsMgr.Stop();

            if (m_accessCtrl != null)
                m_accessCtrl.Close();
        }
    }
}
