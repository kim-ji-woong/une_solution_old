using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;

namespace ServerSimulator
{
    class NetworkServer
    {
        private TcpServer m_server = null;
        private ServiceProvider m_provider = null;//new ServiceProvider();
        private int m_nPort = 0;
        private bool m_isOpened = false;

        private static NetworkServer m_instance = null;
        
        public static NetworkServer Instance
        {
            get { return m_instance; }
        }

        public ServiceProvider ServiceProvider
        {
            get { return m_provider; }
        }

        private int m_nSiteID = 1;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private List<ConnectionState> m_clients = new List<ConnectionState>();

        public NetworkServer()
        {
            m_instance = this;
            m_provider = new ServiceProvider();
        }

        public void NetworkServerLoad(int nPort, string strLog = null)
        {
            ConnectionLogEx.Instance.InitLog(strLog);
            m_nPort = nPort;

            if (m_nPort > 0)
            {
                m_server = new TcpServer(m_provider, m_nPort);
                m_server.ConnectionLog = ConnectionLogEx.Instance;
                m_server.Start();
            }
        }

        public void NetworkServerClosing()
        {
            if (m_server != null)
            {
                m_server.Stop();

            }
        }

        public void AddClient(TcpLib2.ConnectionState state)
        {
            lock (this)
            {
                state.LengthAdd = false;
                state.Tag = DateTime.Now;
                m_clients.Add(state);
                System.Diagnostics.Trace.WriteLine("New Client : " + state.RemoteEndPoint.ToString());
            }
        }

        public void RemoveClient(TcpLib2.ConnectionState state)
        {
            lock (this)
            {
                m_clients.Remove(state);
                System.Diagnostics.Trace.WriteLine("Remove Client : " + state.RemoteEndPoint.ToString());
            }
        }

        public void SendLog(ILogManager mgr)
        {
            lock (this)
            {
                foreach (TcpLib2.ConnectionState state in m_clients)
                {
                    DateTime dtBegin = (DateTime)state.Tag;
                    List<byte[]> byteList = mgr.GetLogBytes(dtBegin);

                    if (byteList != null)
                    {
                        foreach (byte[] bytes in byteList)
                        {
                            m_provider.Send(bytes, 0, bytes.Length, state);
                        }
                    }
                }
            }
        }
    }
}
