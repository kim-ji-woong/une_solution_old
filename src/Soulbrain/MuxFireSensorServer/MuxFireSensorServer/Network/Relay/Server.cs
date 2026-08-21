using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;

namespace MuxFireSensorServer.Network.Relay
{
    public class Server
    {
        private TcpServer m_server = null;
        private ServiceProvider m_provider = null;
        private int m_nPort = 0;
        private bool m_isOpened = false;

        public ServiceProvider Provider
        {
            get { return m_provider; }
        }

        public Server(int nPort)
        {
            m_nPort = nPort;
        }

        public bool BeginServer()
        {
            if (m_nPort > 0)
            {
                if (m_provider != null)
                {
                    m_provider.ReleaseThread();
                }

                m_provider = new ServiceProvider();

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
    }
}
