using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using dnsTcpLib2;

namespace SiemensClient
{
    public class ClientProvider : ClientServiceProvider
    {
        // 현재 OnReceive()에서 받은 데이터를 처리중인가?
        private bool m_isReadingProcess = false;

        public ClientProvider()
        {
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
        }

        public override void OnDropConnection()
        {
        }

        public override void OnReceiveData()
        {
        }
    }
}
