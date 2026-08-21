using System;
using System.Threading;

namespace FireSensorServer.Network
{
    public class JohnsonManager
    {
        private string m_strServerIP = "127.0.0.1";
        private int m_nPort = 4378;
        private bool m_runThread = false;
        private int m_nMuxType = JohnsonClientProvider.MUXTYPE_1;

        private JohnsonClientProvider m_provider = null;
        private Logger m_logger = null;

        public bool IsConnected
        {
            get
            {
                if (m_provider == null || m_provider.IsClientDisposed)
                    return false;

                return m_provider.IsConnected;
            }
        }

        public JohnsonClientProvider ClientProvider
        {
            get { return m_provider; }
        }

        public JohnsonManager(string strIP, string strPort, string strMuxType, Logger logger)
        {
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
                    if (nMuxType == JohnsonClientProvider.MUXTYPE_1)
                        m_nMuxType = JohnsonClientProvider.MUXTYPE_1;
                    else if (nMuxType == JohnsonClientProvider.MUXTYPE_2)
                        m_nMuxType = JohnsonClientProvider.MUXTYPE_2;
                }
            }

            m_logger = logger;
            m_provider = new JohnsonClientProvider(this, logger);
            m_provider.LengthAdd = false;
            m_provider.MuxType = m_nMuxType;
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
                    if (m_provider.IsConnected)
                    {
                        // 10초 이상 아무 신호를 못받으면 접속이 끊어진 것으로 간주한다.
                        if (m_provider.PingCount > 10)
                        {
                            // 아무 신호나 보내본다.
                            int nResult = m_provider.Send(pingBytes, 0, 1);

                            if (nResult < 0)
                            {
                                lock (m_provider)
                                {
                                    m_provider.PingCount = 0;
                                    m_provider.Close();

                                    if (m_provider.Client.Client != null)
                                    {
                                        if (m_provider.Client.Connected)
                                            m_provider.Client.Close();

                                        System.Diagnostics.Trace.WriteLine("Close Provider : " + !m_provider.Client.Connected);
                                    }
                                }
                            }
                        }
                        else
                            m_provider.PingCount++;
                    }

                    if (!m_provider.IsConnected)
                    {
                        lock (m_provider)
                        {
                            if (m_nPort > 0)
                            {
                                m_provider.Connect(m_strServerIP, m_nPort);

                                if (m_provider.IsConnected)
                                    m_logger.Write("[Success connect to " + NetworkManager.GetClientTypeString(NetworkManager.ClientType.Johnson) + " Server] " + m_strServerIP + ":" + m_nPort);
                                else
                                    m_logger.Write("Connection failed : " + NetworkManager.GetClientTypeString(NetworkManager.ClientType.Johnson) + " Server] " + m_strServerIP + ":" + m_nPort);
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
                catch (Exception e)
                {
                    m_logger.Write("ConnectionThread Error : " + e.Message);
                    System.Diagnostics.Trace.WriteLine("ConnectionThread Error : " + e.Message);
                }
            }
        }

        public void ProcessFire(int nReceiverID, int nRelayTeam, int nLoopID, int nRelayID, int nTagID, bool isOn)
        {
            if (isOn)
                m_provider.ProcessFire(nReceiverID, nRelayTeam, nLoopID, nRelayID, nTagID, isOn, "", "", "");
            else
                m_provider.ProcessClear(nReceiverID, nRelayTeam, nLoopID, nRelayID, nTagID, isOn, "", "", "");
        }

        public void ProcessFireFromTagNo(int nSensorTagNo, bool isOn)
        {
            if (isOn)
                m_provider.ProcessFireFromTagNo(nSensorTagNo, isOn);
            else
                m_provider.ProcessClearFromTagNo(nSensorTagNo, isOn);
        }
    }
}
