using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using TcpLib2;
using System.Net;
using System.Net.Sockets;

namespace SVMSCameraReader.Network
{
    class UDPClient
    {
        public static void SendMessage(short nHeader, ArrayList arrDatas, int nPort)
        {
            byte[] bytes = TcpHelper.MakeBytes(nHeader, arrDatas);

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress broadcast = IPAddress.Parse("127.0.0.1");
            IPEndPoint ep = new IPEndPoint(broadcast, nPort);

            s.SendTo(bytes, ep);
        }
    }
}
