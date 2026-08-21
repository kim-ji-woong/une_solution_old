using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using DBUtility2;
using System.Collections;

namespace PSMExternalServer.Network
{
    public class Server
    {
        private TcpServer m_server = null;
        private ServerServiceProvider m_provider = null;
        private WebDBManager m_dbMgr = null;

        private int m_nPort = 0;
        private bool m_isOpened = false;

        public const string Name = "PSMExternalServer";

        public ServerServiceProvider Provider
        {
            get { return m_provider; }
        }

        public Server(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            m_nPort = ReadPort();
        }

        public bool BeginServer()
        {
            if (m_nPort > 0)
            {
                if (m_provider != null)
                {
                    m_provider.ReleaseThread();
                }

                m_provider = new ServerServiceProvider();

                m_server = new TcpServer(m_provider, m_nPort);
                m_server.ConnectionLog = ConnectionLogClient.Instance;
                m_isOpened = m_server.Start();
            }

            return m_isOpened;
        }

        public void StopServer()
        {
            if (m_provider != null)
            {
                if (m_isOpened)
                {
                    m_server.Stop();
                    m_isOpened = false;
                }

                m_provider.ReleaseThread();
                m_provider = null;
            }
        }

        private int ReadPort()
        {
            string strSQL = string.Format("Select Port from SensorServerPort where SiteID = {0} and Name = '{1}'", m_dbMgr.SiteID, Server.Name);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }
    }
}
