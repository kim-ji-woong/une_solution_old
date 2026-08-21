using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using SOPMonitoringSystem;
using TcpLib2;
using SDMS;

namespace SOPMonitoringSystem
{
    public class NetworkManager_Internal
    {
        private ClientProvider_Internal m_provider = null;
        private int m_nPort = -1;
        private bool shutdownThread = false;

		private static NetworkManager_Internal m_manager = null;
		public static NetworkManager_Internal Instance
		{
			get
			{
				if (m_manager == null)
					m_manager = new NetworkManager_Internal();
				return m_manager; 
			}
		}

        public ClientProvider_Internal ClientProvier
        {
            get { return m_provider; }
        }

        public int Send(byte[] bytes, ClientProvider_Internal provider)
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
            return nResult;
        }

        private int m_nSiteID = 1;
        protected NetworkManager_Internal()
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;

            m_provider = new ClientProvider_Internal(this);

            Thread t = new Thread(ConnectionThread);
            t.Name = "ConnectionLocal";
            t.Start();
        }

        private int GetServerPort()
        {
            WebDBManager dbMgr = FormSOP.Instance.DBManager;

            string strSQL = "Select Port from SDMSServerPort where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort - 1;
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
								m_provider.Connect("127.0.0.1", m_nPort);
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
                m_provider = new ClientProvider_Internal(this);
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

        public bool SendMessage(short header, ArrayList arrDatas)
        {
            if (m_provider == null)
                return false;

            byte[] bytes = ClientProvider_Internal.MakeBytes(header, arrDatas);
            return Send(bytes, m_provider) > 0;
        }
    }
}
