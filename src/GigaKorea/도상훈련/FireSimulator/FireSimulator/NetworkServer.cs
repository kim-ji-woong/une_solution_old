using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;

namespace FireSimulator
{
    class NetworkServer
    {
        private TcpServer m_server = null;
        private ServiceProvider m_provider = null;
        private int m_nPort = 1470;
        private bool m_isOpened = false;

        public NetworkServer(IListener listener = null)
        {
            m_provider = new ServiceProvider(listener);
            m_server = new TcpServer(m_provider, m_nPort);
            m_isOpened = m_server.Start();
        }

        public void Close()
        {
            m_server.Stop();
        }

        public void SendAlarm(Alarm alarm, Project project, short nHeader)
        {
            m_provider.SendAlarm(alarm, project, nHeader);
        }

        public void SendClear(Alarm alarm, Project project, short nHeader)
        {
            m_provider.SendClear(alarm, project, nHeader);
        }
    }
}
