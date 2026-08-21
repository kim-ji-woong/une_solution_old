using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using TcpLib2;
using SDMS;

namespace WeatherSimulator
{
    public class NetworkManager
    {
        private ClientProvider m_provider = null;
        private int m_nPort = -1;
        private string m_strServerAddr = "";
        private bool shutdownThread = false;

        private int m_nSiteID = 1;

        public int SiteID
        {
            get { return m_nSiteID; }
            set 
            {
                m_nSiteID = value;
                InitServer();
            }
        }
		private static NetworkManager m_manager = null;
		public static NetworkManager Instance
		{
			get
			{
				if (m_manager == null)
					m_manager = new NetworkManager();
				return m_manager; 
			}
		}

        public ClientProvider ClientProvier
        {
            get { return m_provider; }
        }

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

		/*private void WriteLog(object str)
		{
			if (ConnectionLog.Instance.IsOpened)
				ConnectionLog.Instance.Write(str);
		}

		private void WriteLineLog(object str)
		{
			if (ConnectionLog.Instance.IsOpened)
				ConnectionLog.Instance.WriteLine(str);
		}

		private void InitLog()
		{			
			if (ConnectionLogEx.MakeInstance())
				m_bIsLogOpened = true;
			else
				m_bIsLogOpened = false;			
		}*/

		private bool m_bIsLogOpened = false;
		public bool IsLogOpened
		{
			get { return m_bIsLogOpened; }
			//set { m_bIsLogOpened = value; }
		}

        /*public void RecvLog(byte[] bytes)
        {
			if (!IsLogOpened)
                return;

            if (bytes[0] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
            {
                string strLog = string.Format("RecvMessage : Header({0}), Length({1}), SOPWeather", (int)bytes[0], (int)bytes.Length);
                string strBytes = "";

                foreach (byte b in bytes)
                {
                    if (strBytes.Length == 0)
                        strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                    else
                        strBytes += string.Format(" {0:X2}", (int)b);
                }

                WriteLineLog(strLog + strBytes);
            }
        }*/

        public int Send(byte[] bytes, ClientProvider provider)
        {
			if (provider.IsClientDisposed == true)
				return -1;

			if (provider.IsConnected == false)
			{
				Thread.Sleep(1000);
				if (provider.IsConnected == false)
					return -1;
			}

            int nResult = provider.Send(bytes, 0, bytes.Length);

            if (nResult > 0)
            {
				if (!IsLogOpened)
                    return nResult;

                /*if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                {
                    string strLog = string.Format("SendMessage : Header({0}), Length({1}), SOPWeather", (int)bytes[0], (int)bytes.Length);


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

        protected NetworkManager()
        {
        }


        private bool m_bInitServer = false;
        protected void InitServer()
        {
            if (m_bInitServer == true)
                return;

            m_bInitServer = true;

            string strServerURL = DBUtility.RegUtil.ReadRegValue("Server Connection Info", "webserver_url", m_nSiteID);
            if( strServerURL == null || strServerURL == "")
                strServerURL = DataManager.Instance.DBManager.WebServerURL;

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

            if (FormMain.Instance.SimulationMode)
                m_strServerAddr = "127.0.0.1";
            else
                m_strServerAddr = addr[0].ToString();

            Thread t;
            t = new Thread(ConnectionThread);
            t.Start();
        }

        private int GetServerPort()
        {
            DBUtility.WebDBManager dbMgr = DataManager.Instance.DBManager;

            string strSQL = "Select Port from SDMSServerPort where SiteID = " + DataManager.Instance.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
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

							try
							{
                                //ConnectionLogEx.Instance.WriteLine("PING COUNT EXCEPTION");
								m_provider.Close();
							}
							catch (System.Exception)
							{
								
							}
                            
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
						try
						{
                            if (m_nPort > 0)
                            {
                                m_provider.Connect(m_strServerAddr, m_nPort);
                            }
						}
						catch (System.Exception)
						{

						}
                       
                    }
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

        public bool SendMessage(short header)
        {
            if (m_provider == null)
                return false;

            //lock (this)
            {
                m_provider.SendData(header);
            }
            return true;
        }

		public bool SendMessage(short header, float data)
		{
			if (m_provider == null)
				return false;

			//lock (this)
			{				
				byte[] datas = BitConverter.GetBytes(data);
				m_provider.SendData(header, TCP_TYPE.INTEGER, datas);
			}
			return true;
		}

		public bool SendMessage(short header, string data)
		{
			if (m_provider == null)
				return false;

			//lock (this)
			{
				UTF8Encoding enc = new UTF8Encoding();
				byte[] datas = enc.GetBytes(data);
				m_provider.SendData((short)header, TCP_TYPE.STRING, datas);
			}
			return true;
		}

		public bool SendMessage(short header, int data)
		{
			if (m_provider == null)
				return false;

			//lock (this)
			{
				byte[] datas = BitConverter.GetBytes(data);
				m_provider.SendData((short)header, TCP_TYPE.INTEGER, datas);
			}

			return true;
		}
    }
}
