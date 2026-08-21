using System;
using System.Windows.Forms;
using System.Configuration;
using System.Collections.Generic;
using SDMS.DAL;
using SDMS.Model.CCTV;

namespace SVMSServer
{
    public partial class FormDBTest : Form
    {
        private DataManager m_dataManager = null;

        public FormDBTest()
        {
            InitializeComponent();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (InitDataManager())
            {
                UpdateCCTVs();
            }
        }

        private bool UpdateCCTVs()
        {
            string strErrorMessage;
            List<CCTV> cctvs = m_dataManager.GetSelectManager().SelectCCTVs(null, null, out strErrorMessage);

            if (cctvs == null)
                return false;

            string strStreamName = ConfigurationManager.AppSettings.Get("streamName");
            string strExeName = ConfigurationManager.AppSettings.Get("rtspServerName");
            string strRunModule = ConfigurationManager.AppSettings.Get("runRTSPServer");
            string strJsonFile = ConfigurationManager.AppSettings.Get("cctvJson");

            if (strExeName == null || strRunModule == null || strJsonFile == null)
                return false;

            if (CCTVManager.KillProcess(strExeName))
            {
                CCTVManager.UpdateCCTV(cctvs, strJsonFile, strStreamName);
                CCTVManager.RunProcess(strRunModule);
            }

            return true;
        }

        private bool InitDataManager()
        {
            int nSiteID, nDBType;
            string strSiteID = ConfigurationManager.AppSettings.Get("siteid");
            string strDBType = ConfigurationManager.AppSettings.Get("dbType");

            if (strSiteID == null || strDBType == null)
                return false;

            if (int.TryParse(strSiteID, out nSiteID) == false || int.TryParse(strDBType, out nDBType) == false)
                return false;

            string strWebServerURL = ConfigurationManager.AppSettings.Get("webserverURL");
            string strDBName = ConfigurationManager.AppSettings.Get("dbName");

            if (strWebServerURL == null || strDBName == null)
                return false;

            m_dataManager = new DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            return true;
        }
    }
}
