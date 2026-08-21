using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace ExternalFireSensorDBWatcher
{
    public abstract class FireSensorDBWatcher
    {
        private string m_strServerURL = "";
        private string m_strDatabaseName = "";
        private string m_strUserName = "";
        private string m_strPassword = "";

        protected SensorWatcherOwner m_owner = null;
        protected WebDBManager m_dbMgr = null;
        protected int m_nSiteID = -1;
        protected bool m_runThread = false;

        public string ServerURL
        {
            get { return m_strServerURL; }
            set { m_strServerURL = value; }
        }

        public string DatabaseName
        {
            get { return m_strDatabaseName; }
            set { m_strDatabaseName = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        public FireSensorDBWatcher(SensorWatcherOwner owner, WebDBManager dbMgr, int nSiteID)
        {
            m_owner = owner;
            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;
        }

        public abstract bool Run();

        public void Close()
        {
            m_runThread = false;
        }
    }

    public interface SensorWatcherOwner
    {
        void AddAlarm(ExternalFireSensor sensor);
        void RemoveAlarm(ExternalFireSensor sensor);
    }
}
