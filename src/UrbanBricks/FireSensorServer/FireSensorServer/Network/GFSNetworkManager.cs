using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FireSensorServer.Network
{
    public class GFSNetworkManager
    {
        private ClientProvider m_provider = null;

        private string m_strServerIP = "";
        private int m_nPort = 502;

        private bool m_runThread = false;

        public bool IsConnected
        {
            get
            {
                if (m_provider == null || m_provider.IsClientDisposed)
                    return false;

                return m_provider.IsConnected;
            }
        }

        public GFSNetworkManager(NetworkWebManager webMgr)
        {
            m_provider = new ClientProvider(webMgr);
            m_provider.LengthAdd = false;

            string strIP = System.Configuration.ConfigurationManager.AppSettings["ip"].ToString();
            string strPort = System.Configuration.ConfigurationManager.AppSettings["port"].ToString();
            string strLogFile = System.Configuration.ConfigurationManager.AppSettings.Get("logFile");
            string strLogFolder = System.Configuration.ConfigurationManager.AppSettings.Get("logFolder");

            m_strServerIP = strIP;
            m_nPort = int.Parse(strPort);
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

                                        System.Diagnostics.Trace.WriteLine("Close Provider1 : " + !m_provider.Client.Connected);
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
                                Logger.Instance.Write("[Connection Info] " + m_strServerIP + ":" + m_nPort + " / " + m_provider.IsConnected);
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
                catch (Exception e)
                {
                    Logger.Instance.Write("ConnectionThread Error : " + e.Message);
                    System.Diagnostics.Trace.WriteLine("ConnectionThread Error : " + e.Message);
                }
            }
        }
    }
}
