using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using SDMS;
using TcpLib2;
using DBUtility;

namespace IntegratedManagement2
{
    public class NetworkManager
    {
		private Thread conThread = null;

        private ClientProvider m_provider = null;
        private int m_nPort = -1;
        private string m_strServerAddr = "";

        private bool shutdownThread = false;
        private bool threadIsAlive = false;
        private DBUtility.WebDBManager m_dbMgr = null;

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

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

		public bool CheckLogin(string szID)
		{
			if (m_provider != null)
			{
				return m_provider.SendCheckUser(szID);
			}
			return false;
		}

		public bool Logout(string szID)
		{
			if (m_provider != null)
			{
				return m_provider.SendLogout(szID);
			}
			return false;
		}

		public bool LoginUser(string szID, string szPass)
		{
			if (m_provider != null)
			{
				return m_provider.SendLoginUser(szID, szPass);
			}
			return false;
		}

        public bool RegisterUser(int nMemberID, string szID, string szPass, string szNickName, IntegratedManagement2.PopupDialog.Chief chief)
		{
			if (m_provider != null)
			{
				return m_provider.SendRegisterUser(nMemberID, szID, szPass, szNickName, chief);
			}
			return false;
		}

		public bool SetPassword(string szGenUserID, string szNewPass)
		{
			if (m_provider != null)
			{
				return m_provider.SendSetPassword(szGenUserID, szNewPass);
			}
			return false;
		}

		public bool ChangePassword(int nGenUserID, string szPass, string szNewPass)
		{
			if (m_provider != null)
			{
				return m_provider.SendChangePassword(nGenUserID, szPass, szNewPass);
			}
			return false;
		}

        public bool ChangeNickName(int nGenUserID, string szNickName)
        {
            if (m_provider != null)
            {
                return m_provider.SendChangeNickName(nGenUserID, szNickName);
            }
            return false;
        }

        public NetworkManager(DBUtility.WebDBManager dbMgr, int nSiteID)
        {
            InitLog();

            try
            {
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
                m_provider.ConnectionLog = ConnectionLogEx.Instance;
                m_strServerAddr = addr[0].ToString();

                //m_strServerAddr = "127.0.0.1";
            }
            catch (Exception e)
            {
                ConnectionLogEx.Instance.WriteLine(e.Message);
                System.Windows.Forms.MessageBox.Show("서버의 주소를 받아올 수 없습니다.");
                System.Windows.Forms.Application.Exit();
            }

			conThread = new Thread(ConnectionThread);
			conThread.Start();

            // 시간이 경과한 로그 삭제
            Thread t = new Thread(DeleteLogThread);
            t.Start();
        }

        // dtTarget이 dtNow보다 1달 이전의 시간인가?
        private bool IsPassedTime(DateTime dtNow, int nYear, int nMonth, int nDay)
        {
            if (dtNow.Year - nYear > 1)
                return true;
            else if (dtNow.Year - nYear == 1)
            {
                if (dtNow.Month < 12)
                    return true;
                else if (nMonth > 1)
                    return true;
                else if (dtNow.Day < nDay)
                    return true;
                else
                    return false;
            }
            else if (dtNow.Year > nYear)
                return false;

            if (dtNow.Month - nMonth > 1)
                return true;
            else if (dtNow.Month >= nMonth)
                return false;

            return dtNow.Day < nDay;
        }

        // 1달이 경과한 통신로그 삭제
        private void DeleteLogThread()
        {
            try
            {
                string strPath = System.Windows.Forms.Application.ExecutablePath;
                string szParentPath = System.IO.Path.GetDirectoryName(strPath);

                string[] arrFiles = System.IO.Directory.GetFiles(szParentPath + "\\logs");

                string strKey = "IntegratedManager.log-";
                int len = strKey.Length;

                DateTime dtNow = DateTime.Now;
                int nYear, nMonth, nDay;

                foreach (string strFile in arrFiles)
                {
                    int nIndex = strFile.IndexOf(strKey);

                    if (nIndex < 0)
                        continue;

                    string strDate = strFile.Substring(nIndex + len);

                    int nIndex1 = strDate.IndexOf('-');
                    int nIndex2 = strDate.LastIndexOf('-');

                    if (nIndex1 < 0 || nIndex2 < 0 || nIndex1 == nIndex2)
                        continue;

                    string strYear = strDate.Substring(0, nIndex1);
                    string strMonth = strDate.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                    string strDay = strDate.Substring(nIndex2 + 1);

                    if (!int.TryParse(strYear, out nYear))
                        continue;
                    if (!int.TryParse(strMonth, out nMonth))
                        continue;
                    if (!int.TryParse(strDay, out nDay))
                        continue;

                    if (IsPassedTime(dtNow, nYear, nMonth, nDay))
                        System.IO.File.Delete(strFile);
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {
            }
        }

        public static int GetServerPort(DBUtility.WebDBManager dbMgr)
        {
            string strSQL = "Select Port from SDMSServerPort where SiteID = " + FormMain.Instance.SiteID;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        }

        public int GetServerPort()
        {
            return GetServerPort(m_dbMgr);
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
			try
			{
                //if (conThread != null)
                //    conThread.Join();
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

        public void StartThread()
        {
            shutdownThread = false;

            while (threadIsAlive)
            {
                System.Diagnostics.Trace.WriteLine("Thread kill wating");
                Thread.Sleep(1000);
            }

            conThread = new Thread(ConnectionThread);
            conThread.Start();
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            threadIsAlive = true;

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
                        {
                            m_provider.Connect(m_strServerAddr, m_nPort);

                            if (m_provider.IsConnected)
                                FormMain.Instance.OnConnected();
                        }
                    }
                }
                Thread.Sleep(1000);
            }

            threadIsAlive = false;
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

	public class ConnectionLogEx : ConnectionLog
	{
		private log4net.ILog logger = null;
        private static ConnectionLogEx m_instance2 = new ConnectionLogEx();

        public static ConnectionLogEx Instance
        {
            get { return m_instance2; }
        }

		public static bool MakeInstance()
		{
			/*if (m_instance == null)
				m_instance = new ConnectionLogEx();

			ConnectionLogEx instance = (ConnectionLogEx)m_instance;*/
			m_instance2.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

			m_instance2.m_isOpened = true;
			return m_instance2.m_isOpened;
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
