using dnsDBUtil;
using SmartCity.IDAL;
using System;

namespace SmartCity.DAL
{
    public class DataManager : IDataManager
    {
        private WebDBManager m_dbManager = null;

        private CreateManager m_createManager = null;
        private DeleteManager m_deleteManager = null;
        private SelectManager m_selectManager = null;
        private UpdateManager m_updateManager = null;

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

        public ICreateManager GetCreateManager()
        {
            if (m_createManager != null)
            {
                return m_createManager;
            }
            else
            {
                return null;
            }
        }

        public IDeleteManager GetDeleteManager()
        {
            if (m_deleteManager != null)
            {
                return m_deleteManager;
            }
            else
            {
                return null;
            }
        }

        public ISelectManager GetSelectManager()
        {
            if (m_selectManager != null)
            {
                return m_selectManager;
            }
            else
            {
                return null;
            }
        }

        public IUpdateManager GetUpdateManager()
        {
            if (m_updateManager != null)
            {
                return m_updateManager;
            }
            else
            {
                return null;
            }
        }

        public object GetDBManager()
        {
            if (m_dbManager != null)
            {
                return m_dbManager;
            }
            else
            {
                return null;
            }
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
