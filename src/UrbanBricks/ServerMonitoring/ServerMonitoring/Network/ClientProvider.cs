using System.Net.Sockets;
using TcpLib2;

namespace ServerMonitoring
{
    public class ClientProvider : ClientServiceProvider
    {
        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        public ClientProvider()
        {
			this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
        }

        public override void OnReceiveData()
        {
            if (ReceivedData != null)
            {
                
            }

            m_isReadingProcess = false;
        }

        public override void OnDropConnection()
        {
            
        }
    }
}
