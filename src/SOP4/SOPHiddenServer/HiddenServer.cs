using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDMSServer;
using System.Collections;

namespace SOPHiddenServer
{
    public class HiddenServer
    {
        private DBUtility.WebDBManager m_dbMgr = null;
        public DBUtility.WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        private static HiddenServer m_instance = null;

        public static HiddenServer Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new HiddenServer();

                return m_instance;
            }
        }

        private int m_nSiteID = -1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private NetworkServer m_server = null;
        private int m_nPort = 19500;
        private bool m_isRunning = false;

        public bool IsRunning
        {
            get { return m_isRunning; }
        }

        protected HiddenServer()
        {
            m_instance = this;
            m_nSiteID = LoadSiteID();

            m_dbMgr = new DBManager(m_nSiteID);
        }

        public int LoadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");
            //string strSiteID = m_dbMgr.LoadIni("siteid", "Server Connection Info");

            int nSiteID = 1;

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out nSiteID);
            }

            return nSiteID;
        }

        public void Start(string strDBFilePath, string strPassword)
        {
            if (!m_isRunning)
                m_isRunning = ((DBManager)m_dbMgr).Open(strDBFilePath, strPassword);

            ReadPort();

            if (m_server == null)
                m_server = new NetworkServer(m_dbMgr);

            if (m_isRunning)
                m_server.NetworkServerLoad();
        }

        public void Stop()
        {
            m_server.NetworkServerClosing();
            ((DBManager)m_dbMgr).Close();
            m_isRunning = false;
        }

        private void ReadPort()
        {
            string strSQL = "Select Port from SDMSServerPort WHERE SiteID = "+ m_nSiteID;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            foreach (string strPort in arrResult)
            {
                int.TryParse(strPort, out m_nPort);
                break;
            }

            ((DBManager)m_dbMgr).PortNo = m_nPort;
        }
    }
}
