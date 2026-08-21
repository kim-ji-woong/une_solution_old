using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TcpLib2;
using System.Collections;

namespace SVMSCameraReader.Network
{
    public class UDPServer
    {
        private IUDPServerOwner m_owner = null;
        private bool m_closeThread = false;

        public void Start(int nPort, IUDPServerOwner owner)
        {
            m_owner = owner;

            Thread t = new Thread(new ParameterizedThreadStart(Listen));
            t.Start(nPort);
        }

        public void Stop()
        {
            m_closeThread = true;
        }

        private void Listen(object arg)
        {
            m_closeThread = false;
            int nPort = (int)arg;

            try
            {
                Socket udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                EndPoint localEP = new IPEndPoint(IPAddress.Any, nPort);
                EndPoint remoteEP = new IPEndPoint(IPAddress.None, nPort);

                udpSocket.Bind(localEP);

                byte[] receiveBuffer = new byte[512];

                try
                {
                    while (m_closeThread == false)
                    {
                        // 기다리고 있다가 remoteEP 로부터 데이터를 받는다
                        // receivedSize  : 받은 바이트수
                        // receiveBuffer : 받은 데이터가 들어갈 저장소
                        // remoteEP      : 데이터를 받아올 원격컴퓨터의 IP종단점
                        int receivedSize = udpSocket.ReceiveFrom(receiveBuffer, ref remoteEP);

                        if (m_owner != null)
                        {
                            short nHeader;
                            ArrayList arrDatas = TcpHelper.ReadBytes(receiveBuffer, out nHeader);

                            if (arrDatas != null)
                                m_owner.OnReceive(nHeader, arrDatas);
                        }

                        Thread.Sleep(100);
                    }
                }
                catch (SocketException se)
                {
                    if (m_owner != null)
                        m_owner.OnClose(se.Message);
                    else
                        System.Diagnostics.Trace.WriteLine("Listen Thead Close : " + se.Message);
                }
                finally
                {
                    udpSocket.Close();
                }
            }
            catch (SocketException se)
            {
                if (m_owner != null)
                    m_owner.OnClose(se.Message);
                else
                    System.Diagnostics.Trace.WriteLine("Listen Thead Close : " + se.Message);
            }
        }
    }

    public interface IUDPServerOwner
    {
        void OnReceive(short nHeader, ArrayList arrDatas);
        void OnClose(string strErrorMessage);
    }

    public class Header
    {
        public const short ConnectionComplete = 1;
        public const short LoginComplete = 2;
        public const short FinishUpdate = 3;
        public const short DBConnectionError = 4;
        public const short NoSVMSInfo = 5;
        public const short TimeoutClose = 6;
    }
}
