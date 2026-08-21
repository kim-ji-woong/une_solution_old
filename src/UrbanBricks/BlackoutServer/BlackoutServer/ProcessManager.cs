using BlackoutServer.Data;
using BlackoutServer.Network;
using DBUtility2;
using System;
using System.IO;
using System.Windows.Forms;

namespace BlackoutServer
{
    public class ProcessManager
    {
        private NetworkWebManager m_netWebManager = null;
        public NetworkWebManager NetWebManager
        {
            get { return m_netWebManager; }
        }

        private NetworkModbusManager m_netModbusManager = null;
        public NetworkModbusManager NetModbusManager
        {
            get { return m_netModbusManager; }
        }
        private WebDBManager m_dbManager = null;

        private Timer m_timerDeleteOldLog = null;

        public ProcessManager()
        {
            string strModbusIP = System.Configuration.ConfigurationManager.AppSettings.Get("modbusIP");
            string strModbusPort = System.Configuration.ConfigurationManager.AppSettings.Get("modbusPort");
            string strModbusID = System.Configuration.ConfigurationManager.AppSettings.Get("modbusID");
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings.Get("siteid");
            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings.Get("webServerURL");
            string strDBName = System.Configuration.ConfigurationManager.AppSettings.Get("dbName");
            string strDBType = System.Configuration.ConfigurationManager.AppSettings.Get("dbType");

            Logger.Instance.Write("ModbusIP : " + strModbusIP);
            Logger.Instance.Write("ModbusPort : " + strModbusPort);
            Logger.Instance.Write("ModbusID : " + strModbusID);
            Logger.Instance.Write("SiteID : " + strSiteID);
            Logger.Instance.Write("WebServerURL : " + strWebServerURL);
            Logger.Instance.Write("DBName : " + strDBName);
            Logger.Instance.Write("DBType : " + strDBType);

            int nSiteID, nDBType;

            if (int.TryParse(strSiteID, out nSiteID) && int.TryParse(strDBType, out nDBType))
            {
                m_dbManager = new WebDBManager(nSiteID);
                m_dbManager.WebServerURL = strWebServerURL;
                m_dbManager.DatabaseName = strDBName;
                m_dbManager.DatabaseType = (WebDBManager.DBType)nDBType;

                DataManager.Instance.DisplaySensor(m_dbManager);
            }

            m_netWebManager = new NetworkWebManager(m_dbManager);
            m_netModbusManager = new NetworkModbusManager(strModbusIP, strModbusPort, m_netWebManager);            

            m_timerDeleteOldLog = new Timer();
            m_timerDeleteOldLog.Interval = 1000 * 60 * 60;
            m_timerDeleteOldLog.Tick += timerDeleteOldLog_Tick;
            m_timerDeleteOldLog.Start();
            timerDeleteOldLog_Tick(null, null);
        }

        private void timerDeleteOldLog_Tick(object sender, EventArgs e)
        {
            Logger.Instance.RemoveOldLogs();
        }

        public void Close()
        {
            if (m_netWebManager != null)
                m_netWebManager.Close();
            if (m_netModbusManager != null)
                m_netModbusManager.Close();
        }
    }
}
