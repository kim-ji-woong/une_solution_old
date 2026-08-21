using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Configuration;
using SDMS;
using TcpLib2;

namespace RestoreService
{
    public class NetworkManager
    {
		private static NetworkManager m_instance = null;
		public static NetworkManager Instance
		{
			get { return m_instance; }
		}

        private ClientProvider m_provider = null;
		public RestoreService.ClientProvider SerivceProvider
		{
			get { return m_provider; }
			set { m_provider = value; }
		}
        private int m_nPort = -1;
        private string m_strServerAddr = "";
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

        public void WriteLineLog(object str)
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

        public NetworkManager(DBUtility.WebDBManager dbMgr)
        {
			m_instance = this;

            InitLog();

            m_dbMgr = dbMgr;

            string strPort = m_dbMgr.LoadIni("sdms_port", "Server Connection Info");
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


            Thread t2 = new Thread(ServiceCheckThread);
            t2.Start(this);
        }

        private void ServiceCheckThread(object param)
        {            
            while (!shutdownThread)
            {
                // 처음 시작 시 약 1분간 기다린다.
                for (int i = 0; i < 600; i++)
                {
                    if (!shutdownThread)
                        Thread.Sleep(100);
                } 

                NetworkManager netManager = (NetworkManager)param;
                if (netManager != null && netManager.SerivceProvider != null)
                {
                    if (netManager.SerivceProvider.IsConnected == true)
                    {
                        bool bRun = ServiceManager.IsRunningSerivce("BroadcastServer");
                        if (bRun == false)
                        {
                            if (ServiceManager.GetServiceStartMode("BroadcastServer") == "Automatic")
                                ServiceManager.StartService("BroadcastServer", 300);
                        }

                        bool bRun2 = ServiceManager.IsRunningSerivce("SOPMonitor");
                        if (bRun2 == false)
                        {
                            if (ServiceManager.GetServiceStartMode("SOPMonitor") == "Automatic")
                                ServiceManager.StartService("SOPMonitor", 300);
                        }
                    }
                }
                else
                {
                    bool bRun = ServiceManager.IsRunningSerivce("SOPServer");
                    if (bRun == false)
                    {
                        if (ServiceManager.GetServiceStartMode("SOPServer") == "Automatic")
                            ServiceManager.StartService("SOPServer", 300);
                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    if(!shutdownThread)
                        Thread.Sleep(100);
                }                    
            }
        }

        private int GetServerPort()
        {
            string strSQL = "Select Port from SDMSServerPort";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

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
                        if (m_provider.PingCount > 3)
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
                        m_nPort = NetworkManager.Instance.GetServerPort();

                        if (m_nPort > 0)
							m_provider.Connect(m_strServerAddr, m_nPort);
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


		private Thread m_RestoreThread = null;
		public Thread RestoreThread
		{
			get { return m_RestoreThread; }
		}

		public void BeginRestore()
		{
			m_RestoreThread = new Thread(RunRestoreThread);
			m_RestoreThread.Start(this);
		}

		private void RunRestoreThread(object param)
		{
			NetworkManager mgr = (NetworkManager)param;
			RestoreManager rm = new RestoreManager();

			//mgr.WriteLineLog("Run Restore Thread");
			rm.RestoreProcess();
			//mgr.WriteLineLog("RestoreProcess done.");
			byte[] sendbytes = new byte[6] { TCP_ID.END_RESTORE, 0, 0, 0, 0, 0 };
			mgr.SerivceProvider.Send(sendbytes, 0, sendbytes.Length);
			//mgr.WriteLineLog("Send Endresstore done.");
			rm.PostRestoreProcess();
			//mgr.WriteLineLog("PostRestoreProcess done.");
		}
    }

	public class ConnectionLogEx : ConnectionLog
	{
		private log4net.ILog logger = null;

        public static ConnectionLog Instance
        {
            get { return m_instance; }
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
