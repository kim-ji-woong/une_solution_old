using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DBUtility;
using System.Collections;
using SDMS;

namespace RestartClients.External
{
    public class NetworkManager
    {
        private Thread conThread = null;

        private ClientProvider m_provider = null;
        private int m_nPort = -1;
        private string m_strServerAddr = "";

        private bool shutdownThread = false;
        private DBUtility.WebDBManager m_dbMgr = null;
        private int m_nSiteID = 1;

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        public ClientProvider ClientProvider
        {
            get { return m_provider; }
        }

        public int Send(byte[] bytes, ClientProvider provider)
        {
            int nResult = provider.Send(bytes, 0, bytes.Length);
            return nResult;
        }

        public NetworkManager(DBUtility.WebDBManager dbMgr, int nSiteID)
        {
            try
            {
                m_nSiteID = nSiteID;

                string strServerURL = RegUtil.ReadRegValue("Server Connection Info", "webserver_url", nSiteID);
                m_dbMgr = dbMgr;
                m_dbMgr.WebServerURL = strServerURL;

                int nIndex1 = strServerURL.IndexOf("http://");
                int nIndex2 = strServerURL.LastIndexOf(':');
                string strURL = strServerURL;

                if (nIndex1 >= 0 && nIndex2 >= 0)
                {
                    int nBeginIndex = nIndex1 + "http://".Length;
                    strURL = strServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
                }
                else if (nIndex1 >= 0)
                {
                    int nBeginIndex = nIndex1 + "http://".Length;
                    strURL = strServerURL.Substring(nBeginIndex);
                }
                else if (nIndex2 >= 0)
                {
                    strURL = strServerURL.Substring(0, nIndex2);
                }

                System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);

                m_provider = new ClientProvider(this);
                m_strServerAddr = addr[0].ToString();

                //m_strServerAddr = "127.0.0.1";
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("서버의 주소를 받아올 수 없습니다.");
                System.Windows.Forms.Application.Exit();
            }

            conThread = new Thread(ConnectionThread);
            conThread.Start();
        }

        public static int GetServerPort(DBUtility.WebDBManager dbMgr, int nSiteID)
        {
            string strSQL = "Select Port from SDMSServerPort where SiteID = " + nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        }

        public int GetServerPort()
        {
            return GetServerPort(m_dbMgr, m_nSiteID);
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
            try
            {
                if (conThread != null)
                    conThread.Join();
            }
            catch (System.Exception)
            {
            }

            try
            {
                if (m_provider != null)
                    m_provider.Close();
            }
            catch (System.Exception)
            {
            }
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
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
                            m_provider.SendData(TCP_ID.I_AM_HERE);
                        else
                            m_provider.PingCount++;
                    }

                    if (!m_provider.IsConnected)
                    {
                        m_nPort = GetServerPort();

                        if (m_nPort > 0)
                            m_provider.Connect(m_strServerAddr, m_nPort);
                    }

                    if (m_provider.IsConnected)
                        FormMain.Instance.SetConnected();
                }

                Thread.Sleep(1000);
            }
        }

        public void OnDropConnection()
        {
            lock (this)
            {
                m_provider = new ClientProvider(this);
            }
        }

        private void CopyBytes(byte[] bytesDest, ref int nDestOffset, byte[] bytesSrc)
        {
            int nLength = bytesSrc.Length;
            System.Buffer.BlockCopy(bytesSrc, 0, bytesDest, nDestOffset, nLength);
            nDestOffset += nLength;
        }
    }
}
