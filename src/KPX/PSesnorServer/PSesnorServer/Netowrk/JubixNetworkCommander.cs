using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using SDMS;
using TcpLib2;
using JubixNetwork;

namespace PSensorServer
{
    public class JubixNetworkCommander : JubixNetwork.IJubixNetwork
	{
        private JubixNetwork.JubixClientProvider m_provider = null;
		private int m_nPort = -1;
		private string m_strServerAddr = "";

		private bool shutdownThread = false;
		private DBUtility.WebDBManager m_dbMgr = null;

		// Ping은 로그에 남기지 않는다.
		private bool m_exceptPingLog = true;
        
        private int m_nSiteID = 2;
        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        private bool shutdownSensorThread = false;
        public bool ShutdownSensorThread
        {
            get { return shutdownSensorThread; }
            set { shutdownSensorThread = value; }
        }

        private static JubixNetworkCommander m_instance = null;
        public static JubixNetworkCommander Instance
        {
            get { return m_instance; }
        }

        public JubixNetwork.JubixClientProvider ClientProvider
        {
            get { return m_provider; }
        }

		private void WriteLog(object str)
		{
            if (ConnectionLogExJubix.Instance.IsOpened)
                ConnectionLogExJubix.Instance.Write(str);
		}

		private void WriteLineLog(object str)
		{
            if (ConnectionLogExJubix.Instance.IsOpened)
                ConnectionLogExJubix.Instance.WriteLine(str);
		}
        
		private void InitLog()
		{
            ConnectionLogExJubix.MakeInstance();		
		}

		public void RecvLog(byte[] bytes)
		{
            if (!ConnectionLogExJubix.Instance.IsOpened)
				return;

			if (bytes[0] != TCP_ID.ARE_YOU_THERE || !m_exceptPingLog)
			{
				string strLog = string.Format("RecvMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
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
		}

		public void SensorRecvLog(byte[] bytes)
		{
            if (!ConnectionLogExJubix.Instance.IsOpened)
				return;

			if (bytes[0] != SERIAL_ID.POLL || !m_exceptPingLog)
			{
				string strLog = string.Format("RecvCommand : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
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
		}

        private int WriteSendLog(int nResult, byte[] bytes, JubixClientProvider provider, int nOffset)
        {
            if (nResult > 0)
            {
                provider.PingCount = 0;

                if (!ConnectionLogExJubix.Instance.IsOpened)
                    return nResult;

                if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                {
                    string strLog = string.Format("SendCommand : Header({0}), Length({1})", (int)bytes[nOffset], (int)bytes.Length);
                    string strBytes = "";

                    for (int i=nOffset;i<bytes.Length;i++)
                    //foreach (byte b in bytes)
                    {
                        byte b = bytes[i];

                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    WriteLineLog(strLog + strBytes);
                }
            }
            return nResult;
        }
        private void Connect()
        {
            m_nPort = GetServerPort();
            if (m_nPort > 0)
                m_provider.Connect(m_strServerAddr, m_nPort);
        }

		public int Send(byte[] bytes, JubixClientProvider provider)
		{
            int nLogResult = -1;
            try
            {
                if (m_provider.IsConnected == false)
                    Connect();


                if (provider.LengthAdd == false)
                {
                    int nResult = provider.Send(bytes, 0, bytes.Length);
                    nLogResult = WriteSendLog(nResult, bytes, provider, 0);
                }
                else
                {
                    int nResult = provider.Send(bytes, 0, bytes.Length);
                    nLogResult = WriteSendLog(nResult, bytes, provider, 0);
                }

                if (m_provider.IsConnected == true)
                    m_provider.Close();
            }
            catch(Exception)
            {

            }
            return nLogResult;           
		}

        public int Send_NoLengthByte(byte[] bytes, JubixClientProvider provider)
        {
            int nResult = provider.Send_NoLengthByte(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 4);
        }
        
        private static log4net.ILog logger = null;
        public JubixNetworkCommander(DBUtility.WebDBManager dbMgr, string strServerAddr, int nSiteID)
		{
            m_instance = this;
            m_nSiteID = nSiteID;
            
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			InitLog();  

			m_dbMgr = dbMgr;
          
			m_provider = new JubixNetwork.JubixClientProvider(this);
			m_strServerAddr = strServerAddr;
		}
        
		private int GetServerPort()
		{
            return KPXServerManager.Instance.LoggerPort;
		}
		
		public void ReleaseThread()
		{
			shutdownThread = true;
			shutdownSensorThread = true;
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
                        // 
					}

					if (!m_provider.IsConnected)
					{
						m_nPort = GetServerPort();

						if (m_nPort > 0)
							m_provider.Connect(m_strServerAddr, m_nPort);

						if (m_provider.IsConnected)
						{
							Thread.Sleep(10);
                        }
					}
				}

                if (m_provider.IsConnected)
                {
                    // Send Command in queue
                }

				Thread.Sleep(1000);
			}
		}

		public void OnDropConnection()
		{
		}
      
		private void CopyBytes(byte[] bytesDest, ref int nDestOffset, byte[] bytesSrc)
		{
			int nLength = bytesSrc.Length;
			System.Buffer.BlockCopy(bytesSrc, 0, bytesDest, nDestOffset, nLength);
			nDestOffset += nLength;
		}
	}   
}
