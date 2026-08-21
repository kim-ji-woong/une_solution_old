using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Windows.Forms;


namespace TankServer
{
    public class LevelMeterNetworkServer
    {
        private LogFileManager m_logMgr = null;

        private bool m_bCloseServer = false;
        public bool ClosingServer
        {
            get { return m_bCloseServer; }
        }

        private DBUtility.WebDBManager m_dbMgr = null;
        public DBUtility.WebDBManager DBManager
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

        // Delegate 호출을 위한 Form
        private Form m_frmDelegate = null;
        public Form FormDelegate
        {
            get { return m_frmDelegate; }
            set { m_frmDelegate = value; }
        }

        private bool m_finishProcess = false;
        public bool FinishProcess
        {
            get { return m_finishProcess; }
        }

        private static LevelMeterNetworkServer m_instance = null;
        public static LevelMeterNetworkServer Instance
        {
            get { return m_instance; }
        }

           
        public void LoadBaseData()
        {
        }

        private int m_nSiteID = 500;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public void LoadSiteID()
        {
            DBUtility.Utility ini = new DBUtility.Utility();
            string strSiteID = ini.getinivalue("Server Connection Info", "siteid");

            if (strSiteID.Length > 0)
            {
                int.TryParse(strSiteID, out m_nSiteID);
            }
            else
            {
                m_nSiteID = 500;
            }
        }

        public LevelMeterNetworkServer(DBUtility.WebDBManager dbMgr = null)
        {
            m_dbMgr = dbMgr;
            LoadSiteID();

            if (m_dbMgr == null)
                m_dbMgr = new DBUtility.WebDBManager(m_nSiteID);

            m_instance = this;

            LoadBaseData();

            //m_ioMgr = new IOManager(m_nSiteID);

            //m_logMgr = new LogFileManager();
        }

        public void NetworkServerLoad()
        {
        }

        public void NetworkServerClosing()
        {
            //m_logMgr.Stop();

            m_bCloseServer = true;
            m_finishProcess = true;
        }

    }
}
