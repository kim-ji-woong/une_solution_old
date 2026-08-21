using dnsDBUtil;
using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.IDAL;

namespace TeamEditor.DAL
{
    public class DataManager : IDataManager
    {
        private WebDBManager m_dbManager = null;

        private ICreate m_createManager = null;
        private IDelete m_deleteManager = null;
        private ISelect m_selectManager = null;
        private IUpdate m_updateManager = null;

        public int SiteID
        {
            get
            {
                if (m_dbManager != null)
                {
                    return m_dbManager.SiteID;
                }
                else
                {
                    return 0;
                }
            }
        }

        public DataManager()
        {
            SetDBConnection();

            CreateAllManager();
        }

        public DataManager(int nSiteID)
        {
            SetDBConnection(nSiteID);

            CreateAllManager();
        }

        public DataManager(string strDatabaseName, int nSiteID)
        {
            SetDBConnection(strDatabaseName, nSiteID);

            CreateAllManager();
        }

        public DataManager(string strDatabaseName, string strDBType, int nSiteID)
        {
            SetDBConnection(strDatabaseName, strDBType, nSiteID);

            CreateAllManager();
        }

        public DataManager(string strDatabaseName, string strDBType, int nSiteID, string strWebServerURL)
        {
            SetDBConnection(strDatabaseName, strDBType, nSiteID, strWebServerURL);

            CreateAllManager();
        }

        public DataManager(string strDatabaseName, int nDBType, int nSiteID, string strWebServerURL)
        {
            SetDBConnection(strDatabaseName, nDBType, nSiteID, strWebServerURL);

            CreateAllManager();
        }

        public void CreateAllManager()
        {
            if (m_createManager == null)
            {
                m_createManager = new CreateManager(this);
            }

            if (m_selectManager == null)
            {
                m_selectManager = new SelectManager(this);
            }

            if (m_deleteManager == null)
            {
                m_deleteManager = new DeleteManager(this);
            }

            if (m_updateManager == null)
            {
                m_updateManager = new UpdateManager(this);
            }
        }

        public ICreate GetCreateManager()
        {
            return m_createManager;
        }

        public IDelete GetDeleteManager()
        {
            return m_deleteManager;
        }

        public ISelect GetSelectManager()
        {
            return m_selectManager;
        }

        public IUpdate GetUpdateManager()
        {
            return m_updateManager;
        }

        public object GetDBManager()
        {
            return m_dbManager;
        }

        public void SetDBConnection()
        {
            if (m_dbManager == null)
            {
                m_dbManager = new WebDBManager();
            }
        }

        public void SetDBConnection(int nSiteID)
        {
            if (m_dbManager == null)
            {
                m_dbManager = new WebDBManager(nSiteID);
            }
        }

        public void SetDBConnection(string strDatabaseName, int nSiteID)
        {
            if (m_dbManager == null)
            {
                m_dbManager = new WebDBManager(strDatabaseName, nSiteID);
            }
        }

        public void SetDBConnection(string strDatabaseName, string strDBType, int nSiteID)
        {
            if (m_dbManager == null)
            {
                m_dbManager = new WebDBManager(strDatabaseName, strDBType, nSiteID);
            }
        }

        public void SetDBConnection(string strDatabaseName, string strDBType, int nSiteID, string strWebServerURL)
        {
            if (m_dbManager == null)
            {
                m_dbManager = new WebDBManager(strDatabaseName, strDBType, nSiteID, strWebServerURL);
            }
        }

        public void SetDBConnection(string strDatabaseName, int nDBType, int nSiteID, string strWebServerURL)
        {
            if (m_dbManager == null)
            {
                m_dbManager = new WebDBManager(strDatabaseName, nDBType, nSiteID, strWebServerURL);
            }
        }
    }
}
