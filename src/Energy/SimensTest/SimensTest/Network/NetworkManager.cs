using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using TcpLib2;

namespace SensorTester
{
	public class NetworkManager
	{
		private ClientProvider m_provider = null;
		private int m_nPort = -1;
		private string m_strServerAddr = "";
		//private bool m_isConnected = false;
		private bool shutdownThread = false;
		private DBUtility.WebDBManager m_dbMgr = null;

		// Ping은 로그에 남기지 않는다.
		private bool m_exceptPingLog = true;

        private MessageQueue m_msgQueue = new MessageQueue();

        public ClientProvider ClientProvider
        {
            get { return m_provider; }
        }

		private void WriteLog(object str)
		{
			if (ConnectionLogEx.Instance.IsOpened)
				ConnectionLogEx.Instance.Write(str);
		}

		private void WriteLineLog(object str)
		{
            if (ConnectionLogEx.Instance.IsOpened)
                ConnectionLogEx.Instance.WriteLine(str);
		}

		private void InitLog()
		{
			ConnectionLogEx.MakeInstance();		
		}

		public void RecvLog(byte[] bytes)
		{
            if (!ConnectionLogEx.Instance.IsOpened)
				return;
            			
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
		

        private int WriteSendLog(int nResult, byte[] bytes, ClientProvider provider, int nOffset)
        {
            if (nResult > 0)
            {
                if (!ConnectionLogEx.Instance.IsOpened)
                    return nResult;
                
                string strLog = string.Format("SendMessage : Header({0}), Length({1})", (int)bytes[nOffset], (int)bytes.Length);
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

                provider.PingCount = 0;
            }

            return nResult;
        }

		public int Send(byte[] bytes, ClientProvider provider)
		{
			int nResult = provider.Send(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 0);
		}

        public int Send_NoLengthByte(byte[] bytes, ClientProvider provider)
        {
            int nResult = provider.Send_NoLengthByte(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 4);
        }


        private int m_nSiteID = 1;

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

		public NetworkManager(DBUtility.WebDBManager dbMgr, string strServerAddr, int nSiteID)
		{
            m_nSiteID = nSiteID;

			InitLog();  

			m_dbMgr = dbMgr;

            if (strServerAddr == null)
            {
                string strPort = "19000";
                string strServerURL = m_dbMgr.WebServerURL;

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
                strServerAddr = addr[0].ToString();
            }
                       
			m_provider = new ClientProvider(this);
			m_strServerAddr = strServerAddr;
		}

		public int GetServerPort()
		{

            string strSQL = string.Format("Select Port from SensorServerPort where SiteID = {0} AND Name='{1}'", m_nSiteID.ToString(), "PSMSensor");
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
			return 19000;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        
		}
        

		public void OnDropConnection()
		{
            m_msgQueue.AbleToSend = false;
			//lock (this)
			//{
				//m_isConnected = false;
				//m_provider = new ClientProvider(this);
			//}
		}

        public void MessageQueueReady()
        {
            // 1초 후에 MessageQueue에서 Send할 수 있도록 바꾼다.
            Thread t = new Thread(MessageQueueReadyThread);
            t.Name = "MessageQueueThread";
            t.Start();
        }

        // 1초 후에 MessageQueue에서 Send할 수 있도록 바꾼다.
        private void MessageQueueReadyThread()
        {
            Thread.Sleep(1000);
            m_msgQueue.AbleToSend = true;
        }


		public void SendTestData()
		{
			if (!m_provider.IsConnected)
				return;


            int nSize = 10;
			byte[] bytes = new byte[nSize];
		
			
			
            // 바로 보내지 않고 Queue에 쌓아둔다.
            // Queue에 쌓인 데이터는 ConnectionThread에서 한꺼번에 보낸다.
            m_msgQueue.Add(new QueueData_AllReceiverState(bytes));
			//Send(bytes, m_provider);

			//m_provider.PingCount = 0;
		}


		private void CopyBytes(byte[] bytesDest, ref int nDestOffset, byte[] bytesSrc)
		{
			int nLength = bytesSrc.Length;
			System.Buffer.BlockCopy(bytesSrc, 0, bytesDest, nDestOffset, nLength);
			nDestOffset += nLength;
		}
	}

	public class ConnectionLogEx : ConnectionLog
	{
		private log4net.ILog logger = null;

        public static ConnectionLogEx Instance
        {
            get { return (ConnectionLogEx)m_instance; }
        }

		public static bool MakeInstance()
		{
			if (m_instance == null)
				m_instance = new ConnectionLogEx();

			ConnectionLogEx instance = (ConnectionLogEx)m_instance;
			instance.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

			instance.m_isOpened = true;
			return instance.m_isOpened;
		}

		public override bool Write(object obj, bool writeTime = true)
		{
			if(obj.GetType() == typeof(Exception))
			{
				Exception e = (Exception)obj;
				if (logger != null)
					logger.Debug(e.Message, e);
			}
			else
			{
				if (logger != null)
					logger.DebugFormat("{0}", obj.ToString());
			}
			return true;
		}

		public override bool WriteLine(object obj, bool writeTime = true)
		{
			if(obj.GetType() == typeof(Exception))
			{
				Exception e = (Exception)obj;
				if (logger != null)
					logger.Debug(e.Message, e);
			}
			else
			{
				if (logger != null)
					logger.Debug(obj.ToString());
			}
			return true;
		}
	}
}
