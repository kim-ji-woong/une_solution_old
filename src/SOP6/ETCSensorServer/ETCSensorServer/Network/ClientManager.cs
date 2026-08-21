using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TcpLib2;
using libUSS;
using ETCSensorServer.Data;

namespace ETCSensorServer.Network
{
    public class ClientManager
    {
        private ClientProvider m_provider = null;
        private int m_nPort = -1;
        private string m_strServerAddr = "";
        private bool shutdownThread = false;
        private IServiceOwner m_owner = null;
        private SensorManager m_sensorMgr = null;

        public ClientManager(string strServerAddr, int nPort, IServiceOwner owner, SensorManager sensorManager, NetworkWebManager webMgr)
        {
            m_strServerAddr = strServerAddr;
            m_nPort = nPort;
            m_owner = owner;
            m_sensorMgr = sensorManager;

            m_provider = new ClientProvider(this, m_sensorMgr, webMgr);

            Thread t = new Thread(ConnectionThread);
            t.Start();
        }

        private void ConnectionThread()
        {
            while (!shutdownThread)
            {
                lock (this)
                {
                    if (m_provider.IsConnected)
                    {
                        if (m_provider.PingCount > 5)
                        {
                            m_provider.PingCount = 0;
                            m_provider.Close();
                        }
                        // IsReadingProcess가 true이면 OnReceive에서 받은 데이터를 처리중이므로 다른 Data를 수신할 수 없는 상태임
                        else if (m_provider.IsReadingProcess)
                            m_provider.SendData(Header.I_AM_HERE);
                        else
                            m_provider.PingCount++;
                    }

                    if (!m_provider.IsConnected)
                    {
                        if (m_nPort > 0)
                        {
                            m_provider.Connect(m_strServerAddr, m_nPort);

                            if (m_provider.IsConnected)
                            {
                                if (m_owner != null)
                                    m_owner.OnConnect();
                            }
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        public void OnDropConnection()
        {
            if (m_owner != null)
                m_owner.OnDropConnection();
        }

        public int Send(byte[] bytes)
        {
            int nResult = m_provider.Send(bytes, 0, bytes.Length);

            if (nResult > 0)
            {
                /*if (bytes[0] != Header.I_AM_HERE)
                {
                    string strLog = string.Format("SendMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }
                    WriteLineLog(strLog + strBytes);
                }*/
            }
            return nResult;
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
        }
    }
}
