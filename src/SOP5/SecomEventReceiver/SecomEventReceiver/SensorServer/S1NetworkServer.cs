using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Windows.Forms;
using DBUtility;

namespace SecomEventReceiver
{
    public class S1NetworkServer
    {
        private LogFileManager m_logMgr = null;

		private bool m_bCloseServer = false;
		public bool ClosingServer
		{
			get { return m_bCloseServer; }
		}
        private TcpServer m_server = null;
        private S1NetworkServiceProvider m_provider = null;//new ServiceProvider();
        private int m_nPort = 0;
        private bool m_isOpened = false;
        private DBUtility.WebDBManager m_dbMgr = null;//new DBUtility.WebDBManager();
		public DBUtility.WebDBManager DBManager
		{
			get { return m_dbMgr; }
			set { m_dbMgr = value; }
		}
        

        private IOManager m_ioMgr = null;

        private bool m_finishProcess = false;

        public bool FinishProcess
        {
            get { return m_finishProcess; }
        }
        private static S1NetworkServer m_instance = null;
        public static S1NetworkServer Instance
        {
            get { return m_instance; }
        }
        	
        public IOManager IOManager
        {
            get { return m_ioMgr; }
        }

        public S1NetworkServiceProvider ServiceProvider
        {
            get { return m_provider; }
        }

        private bool m_isSimulationMode = false;
        public bool SimulationMode
        {
            get { return m_isSimulationMode; }
        }

        // Value : Client Type
        private Dictionary<TcpLib2.ConnectionState, string> m_dicClientType = new Dictionary<TcpLib2.ConnectionState, string>();

		public void LoadBaseData()
		{            
        }

        private int m_nSiteID = 1;
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
                m_nSiteID = 1;
            }
        }

        public S1NetworkServer(DBUtility.WebDBManager dbMgr = null)
        {
            m_dbMgr = dbMgr;
            LoadSiteID();

            if (m_dbMgr == null)
                m_dbMgr = new DBUtility.WebDBManager(m_nSiteID);

            m_instance = this;
			
			LoadBaseData();

            m_provider = new S1NetworkServiceProvider();
            m_ioMgr = new IOManager(m_nSiteID);

            int nPort = GetPort();

			if (nPort < 0)
			{
				m_nPort = 19000;
                SetPort();
			}
            else
            {
                m_nPort = nPort;
            }
            
            m_logMgr = new LogFileManager();
        }

        private int GetPort()
        {
            string strSQL = "Select Port, Name from SensorServerPort where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            int nPort = -1;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> port = WebDBManager.GetIntField(arrResult[i].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (port == null || strName == null)
                    continue;

                if (strName == "S1SensorServer")
                    return port.Data;

                nPort = port.Data;
            }

            return nPort;
        }

        private void SetPort()
        {
            string strSQL = string.Format("Insert into SensorServerPort (Port, SiteID, Name) values ({0}, {1}, 'S1SensorServer')", m_nPort, m_nSiteID);
            m_dbMgr.GetResultData(strSQL, 0);
        }

		public void NetworkServerLoad()
        {
            if (m_nPort > 0)
            {
                m_server = new TcpServer(m_provider, m_nPort);
                m_server.ConnectionLog = ConnectionLogClient.Instance;
                m_isOpened = m_server.Start();

                if (m_isOpened)
                    WritePortToDB();
            }
        }

		public void NetworkServerClosing()
		{
            m_logMgr.Stop();

			m_bCloseServer = true;
			m_finishProcess = true;

			m_provider.ReleaseThread();

			if (m_server != null && m_isOpened)
			{
				m_isOpened = false;
				m_server.Stop();

			}
		}

        public void AddClient(TcpLib2.ConnectionState state)
        {
            if (state == null)
                return;

            ClientData client = (ClientData)state.Tag;
            if (client == null)
                return;

            string strClientType = GetClientTypeString(client);
            m_dicClientType[state] = strClientType;
        }

        private string GetClientTypeString(ClientData client)
        {            
            string strClientType = " 알수 없음";
            //DdMonitor.Enter(m_provider.LockObject);
            {

                if (client.Type == ClientData.ClientType.SIEMENS)
                    strClientType = " SIEMENS";

                else if (client.Type == ClientData.ClientType.PSMTester)
                    strClientType = " PSMTester";

            }
            //DdMonitor.Exit(m_provider.LockObject);
            return strClientType;
        }

        public void RemoveClient(TcpLib2.ConnectionState state)
        {
            if (state != null)
                m_dicClientType.Remove(state);
        }

        private void WritePortToDB()
        {
            string strSQL = string.Format("Select Max(Port) from SensorServerPort WHERE Name='S1SensorServer'");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            if (arrResult.Count == 0 || arrResult[0].ToString() == "null")
            {
                strSQL = string.Format("Insert into SensorServerPort (Name, Port, SiteID) values ('{0}', {1}, {2})", "S1SensorServer", m_nPort, m_nSiteID);
                m_dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                strSQL = string.Format("Update SensorServerPort Set Port = {0} WHERE SiteID = {1} AND Name='{2}'", m_nPort, m_nSiteID, "S1SensorServer");
                m_dbMgr.GetResultData(strSQL, 0);
            }
        }		
    }
}
