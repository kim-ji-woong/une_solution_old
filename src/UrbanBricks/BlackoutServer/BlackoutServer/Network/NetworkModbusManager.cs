using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlackoutServer.Network
{
    public class NetworkModbusManager
    {
        private string m_strServerIP = "";
        private int m_nPort = -1;
        private bool m_runThread = false;
        
        private ClientProvider m_provider = null;
        public ClientProvider Provider
        {
            get { return m_provider; }
        }

        public NetworkModbusManager(string strIP, string strPort, NetworkWebManager netWebManager)
        {
            if (strIP != null && strIP.Length > 0)
                m_strServerIP = strIP;

            if (strPort != null && strPort.Length > 0)
            {
                int nPort;

                if (int.TryParse(strPort.Trim(), out nPort))
                    m_nPort = nPort;
            }

            m_provider = new ClientProvider(this, netWebManager);
            m_provider.LengthAdd = false;

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
        }

        public void Close()
        {
            m_runThread = false;
            m_provider.RunThread = false;
            m_provider.Close();
        }

        private void WriteLog(string strLog)
        {
            Logger.Instance.Write(strLog);
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
                                WriteLog("[INFO] ConnectionThread() : " + m_strServerIP + ":" + m_nPort + " / " + m_provider.IsConnected);
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
                catch (Exception e)
                {
                    WriteLog("[ERROR] ConnectionThread() : " + e.Message);
                }
            }
        }
    }
}
