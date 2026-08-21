using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Net.Sockets;
using System.Collections;

namespace SOPMessenger
{
    public class ClientProvider : ClientServiceProvider
    {
        public ClientProvider()
        {
            this.Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
        }

        public override void OnReceiveData()
        {
        }

        public override void OnDropConnection()
        {
        }

        public void SendData(int nHeader)
        {
            if (this.IsConnected == false)
                return;

            byte[] bytes = new byte[6] { (byte)nHeader, 0, 0, 0, 0, 0 };

            if (this.IsClientDisposed == false)
                Send(bytes, 0, bytes.Length);
        }
    }
}
