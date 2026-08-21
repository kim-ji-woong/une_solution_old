using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MuxFireSensorServer.Network
{
    public class NetworkManager
    {
        private string m_strServerIP = "127.0.0.1";
        private int m_nPort = 4378;
        private bool m_runThread = false;
        private int m_nMuxType = ClientProvider.MUXTYPE_1;

        // 통신 오류가 나더라도 신호를 정상적으로 받도록 하기 위하여 두개의 Client를 사용한다.
        private ClientProvider m_provider1 = null;
        //private ClientProvider m_provider2 = null;

        private static NetworkManager m_instance = null;

        public static NetworkManager Instance
        {
            get { return m_instance; }
        }

        public bool IsConnected
        {
            get
            {
                if (m_provider1 == null || m_provider1.IsClientDisposed)
                    return false;

                return m_provider1.IsConnected;
            }
        }

        public ClientProvider ClientProvider
        {
            get { return m_provider1; }
        }
        
        public NetworkManager(string strIP, string strPort, string strMuxType, string strLogFolder, string strLogFile)
        {
            m_instance = this;

            if (strIP != null && strIP.Length > 0)
                m_strServerIP = strIP;

            if (strPort != null && strPort.Length > 0)
            {
                int nPort;

                if (int.TryParse(strPort.Trim(), out nPort))
                    m_nPort = nPort;
            }

            if (strMuxType != null && strMuxType.Length > 0)
            {
                int nMuxType;

                if (int.TryParse(strMuxType.Trim(), out nMuxType))
                {
                    if (nMuxType == ClientProvider.MUXTYPE_1)
                        m_nMuxType = ClientProvider.MUXTYPE_1;
                    else if (nMuxType == ClientProvider.MUXTYPE_2)
                        m_nMuxType = ClientProvider.MUXTYPE_2;
                }
            }

            m_provider1 = new ClientProvider(this, strLogFolder, strLogFile + "_1");
            m_provider1.LengthAdd = false;
            m_provider1.MuxType = m_nMuxType;

            /*m_provider2 = new ClientProvider(this, strLogFile + "2");
            m_provider2.MuxType = m_nMuxType;
            m_provider2.LengthAdd = false;*/
        }

        public void Start()
        {
            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
        }

        public void Stop()
        {
            m_runThread = false;
        }

        private void ConnectionThread()
        {
            m_runThread = true;
            byte[] pingBytes = new byte[] { 0x00 };

            while (m_runThread)
            {
                try
                {
                    if (m_provider1.IsConnected)
                    {
                        // 10초 이상 아무 신호를 못받으면 접속이 끊어진 것으로 간주한다.
                        if (m_provider1.PingCount > 10)
                        {
                            // 아무 신호나 보내본다.
                            int nResult = m_provider1.Send(pingBytes, 0, 1);
                            
                            if (nResult < 0)
                            {
                                lock (m_provider1)
                                {
                                    m_provider1.PingCount = 0;
                                    m_provider1.Close();

                                    if (m_provider1.Client.Client != null)
                                    {
                                        if (m_provider1.Client.Connected)
                                            m_provider1.Client.Close();

                                        System.Diagnostics.Trace.WriteLine("Close Provider1 : " + !m_provider1.Client.Connected);
                                    }

                                    // SOPWebServer에게 보낸다.
                                    NetworkWebClient.Instance.SendReceiverState(false);
                                }
                            }
                        }
                        else
                            m_provider1.PingCount++;
                    }

                    if (!m_provider1.IsConnected)
                    {
                        lock (m_provider1)
                        {
                            if (m_nPort > 0)
                            {
                                m_provider1.Connect(m_strServerIP, m_nPort);
                                m_provider1.WriteLog("Connection1 : " + m_provider1.IsConnected);
                                System.Diagnostics.Trace.WriteLine("Connection1 : " + m_provider1.IsConnected);
                            }
                        }

                        if (m_provider1.IsConnected)
                        {
                            // SOPWebServer에게 보낸다.
                            NetworkWebClient.Instance.SendReceiverState(true);
                        }
                    }

                    Thread.Sleep(500);

                    /*if (m_provider2.IsConnected)
                    {
                        // 10초 이상 아무 신호를 못받으면 접속이 끊어진 것으로 간주한다.
                        if (m_provider2.PingCount > 10)
                        {
                            lock (m_provider2)
                            {
                                m_provider2.PingCount = 0;
                                m_provider2.Close();

                                if (m_provider2.Client.Connected)
                                    m_provider2.Client.Close();

                                System.Diagnostics.Trace.WriteLine("Close Provider2 : " + !m_provider2.Client.Connected);
                            }
                        }
                        else
                            m_provider2.PingCount++;
                    }

                    if (!m_provider2.IsConnected)
                    {
                        lock (m_provider2)
                        {
                            if (m_nPort > 0)
                            {
                                m_provider2.Connect(m_strServerIP, m_nPort);
                                System.Diagnostics.Trace.WriteLine("Connection2 : " + m_provider2.IsConnected);
                            }
                        }

                        if (m_provider2.IsConnected)
                        {
                            // SOPWebServer에게 보낸다.
                            //SendAllReciverState();
                        }
                    }*/

                    Thread.Sleep(500);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine("ConnectionThread Error : " + e.Message);
                }
            }
        }

        public void ProcessFire(int nReceiverID, int nLoopID, int nRelayID, int nTagID, bool isOn)
        {
            if (isOn)
                m_provider1.ProcessFire(nReceiverID, nLoopID, nRelayID, nTagID, isOn, "", "", "");
            else
                m_provider1.ProcessClear(nReceiverID, nLoopID, nRelayID, nTagID, isOn, "", "", "");
        }
    }
}
