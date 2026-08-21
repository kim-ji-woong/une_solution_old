using System;
using TcpLib2;

namespace BroadcastSimulator
{
    public class BroadcastManager
    {
        private int m_nPort = 0;
        private TcpServer m_server = null;
        private ServiceProvider m_provider = null;

        public BroadcastManager(IServiceOwner owner, int nPort)
        {
            m_nPort = nPort;
            m_provider = new ServiceProvider();
            m_provider.ServiceOwner = owner;

            try
            {
                m_server = new TcpServer(m_provider, m_nPort);
                m_server.Start();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Listen Error : " + e.Message);
            }
        }

        public bool SendMessage(int nEquipID, int nChannel, bool onOff)
        {
            return m_provider.SendMessage(nEquipID, nChannel, onOff);
        }
    }
}
