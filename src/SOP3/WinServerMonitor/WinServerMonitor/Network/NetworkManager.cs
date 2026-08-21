using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using SDMS;
using TcpLib2;

namespace ServerMonitor
{
	public class NetworkManager
	{
		private ClientProvider m_provider = null;
		private int m_nPort = -1;
		private string m_strServerAddr = "";
        public string ServerAddr
        {
            get { return m_strServerAddr; }
            set { m_strServerAddr = value; }
        }
		//private bool m_isConnected = false;
		private bool shutdownThread = false;
		private DBUtility.WebDBManager m_dbMgr = null;
        public DBUtility.WebDBManager DBManager
        {
            get { return m_dbMgr; }
            set { m_dbMgr = value; }
        }

		// Ping은 로그에 남기지 않는다.
		private bool m_exceptPingLog = true;

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

		public int Send(byte[] bytes, ClientProvider provider)
		{
			int nResult = provider.Send(bytes, 0, bytes.Length);

			if (nResult > 0)
			{
				if (!ConnectionLogEx.Instance.IsOpened)
					return nResult;

				if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
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
				}
			}

			return nResult;
		}

        private int m_nSiteID = 1;
		public NetworkManager(DBUtility.WebDBManager dbMgr, int nSiteID)
		{
            m_nSiteID = nSiteID;

			InitLog();

			m_dbMgr = dbMgr;

            string strPort = DBUtility.RegUtil.ReadRegValue("sdms_port", "Server Connection Info", m_nSiteID);
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

			m_provider = new ClientProvider(this);
			m_strServerAddr = addr[0].ToString();

			//m_strServerAddr = "127.0.0.1";

			Thread t = new Thread(ConnectionThread);
            t.Start();
		}

		private int GetServerPort()
		{
            string strSQL = string.Format("Select Port from SDMSServerPort where SiteID = {0}", m_nSiteID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            nPort += 1;
            //int nPort = 19501;


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
						if (m_provider.PingCount > 10)
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

						if (m_provider.IsConnected)
						{
							Thread.Sleep(10);
                        }
					}
				}
				Thread.Sleep(1000);
			}
		}

        public bool IsConnected()
        {
            if (m_provider == null)
                return false;
            return m_provider.IsConnected;
        }

		public void OnDropConnection()
		{
			lock (this)
			{
				//m_isConnected = false;
				m_provider = new ClientProvider(this);
			}
		}

        public void SendCheckState()
		{
			if (!m_provider.IsConnected)
				return;
            	
			int nSize = 6 ;;
			byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.CHECK_ALL_SERVER);
			bytes[0] = byteHeader[0];
			bytes[1] = byteHeader[1];

			// SET DATA COUNT
			byte[] nCount = BitConverter.GetBytes(0);
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			Send(bytes, m_provider);

			//m_provider.PingCount = 0;
		}

        public void SendStartTTS()
        {
            if (!m_provider.IsConnected)
                return;

            int nSize = 6; ;
            byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.START_TTS_SERVER);
            bytes[0] = byteHeader[0];
            bytes[1] = byteHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(0);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            Send(bytes, m_provider);

            //m_provider.PingCount = 0;
        }
        public void SendStopTTS()
        {
            if (!m_provider.IsConnected)
                return;

            int nSize = 6; ;
            byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.STOP_TTS_SERVER);
            bytes[0] = byteHeader[0];
            bytes[1] = byteHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(0);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            Send(bytes, m_provider);

            //m_provider.PingCount = 0;
        }
        public void SendStartSOP()
        {
            if (!m_provider.IsConnected)
                return;

            int nSize = 6; ;
            byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.START_SOP_SERVER);
            bytes[0] = byteHeader[0];
            bytes[1] = byteHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(0);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            Send(bytes, m_provider);

            //m_provider.PingCount = 0;
        }
        public void SendStopSOP()
        {
            if (!m_provider.IsConnected)
                return;

            int nSize = 6; ;
            byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.STOP_SOP_SERVER);
            bytes[0] = byteHeader[0];
            bytes[1] = byteHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(0);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            Send(bytes, m_provider);

            //m_provider.PingCount = 0;
        }

        public void SendStartSenor()
        {
            if (!m_provider.IsConnected)
                return;

            int nSize = 6; ;
            byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.START_SENSOR_MONITOR);
            bytes[0] = byteHeader[0];
            bytes[1] = byteHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(0);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            Send(bytes, m_provider);
        }
        public void SendStopSensor()
        {
            if (!m_provider.IsConnected)
                return;

            int nSize = 6; ;
            byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.STOP_SENSOR_MONITOR);
            bytes[0] = byteHeader[0];
            bytes[1] = byteHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(0);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            Send(bytes, m_provider);
        }

        private bool m_bDownLog = false;
        public bool DownLog
        {
            get { return m_bDownLog; }
            set { m_bDownLog = value; }
        }
        public bool SendBackupLog()
        {
            if (m_bDownLog == true)
                return true;
            m_bDownLog = true;
            if (!m_provider.IsConnected)
                return false;

            int nSize = 6; ;
            byte[] bytes = new byte[nSize];

            byte[] byteHeader = BitConverter.GetBytes((short)TCP_ID.START_BACKUP_LOG);
            bytes[0] = byteHeader[0];
            bytes[1] = byteHeader[1];

            // SET DATA COUNT
            byte[] nCount = BitConverter.GetBytes(0);
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            Send(bytes, m_provider);
            return true;
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
