using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TcpLib2;
using log4net;
using System.Data.SqlClient;

namespace HSMS
{
    public class NetworkManager
    {
        private ClientProvider m_provider = null;
        private int m_nPort = -1;
        private string m_strServerAddr = "";
        //private bool m_isConnected = false;
        private bool shutdownThread = false;
        private DBConn m_dbMgr = null;

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

        public void SensorRecvLog(byte[] bytes)
        {
			if (!ConnectionLogEx.Instance.IsOpened)
                return;

            if (bytes[0] != SERIAL_ID.POLL || !m_exceptPingLog)
            {
                string strLog = string.Format("RecvSensorMessage : Header({0}), Length({1})", (int)bytes[0], (int)bytes.Length);
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

        private int WriteSendLog(int nResult, byte[] bytes, ClientProvider provider, int nOffset)
        {
            if (nResult > 0)
            {
                if (!ConnectionLogEx.Instance.IsOpened)
                    return nResult;

                if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                {

                    string szRemotePort = "";
                    try
                    {
                        szRemotePort = provider.Client.Client.LocalEndPoint.ToString();
                    }
                    catch (System.Exception)
                    {

                    }
                    string strLog = string.Format("SendMessage : Header({0}), Length({1}), {2}", (int)bytes[nOffset], (int)bytes.Length, szRemotePort);
                    string strBytes = "";

                    for (int i = nOffset; i < bytes.Length; i++)
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

        public NetworkManager()
        {
            InitLog();

            m_dbMgr = new DBConn("HSMS");

            m_strServerAddr = DBConn.GetInValue("HSMSServer", "ip_addr");
            m_nPort = GetServerPort();

            m_provider = new ClientProvider(this);

            // 접속이 계속 유지될 수 있도록 한다.
            Thread t = new Thread(ConnectionThread);
            t.Start();

            // 시간이 경과한 로그 삭제
            t = new Thread(DeleteLogThread);
            t.Start();
        }

        private int GetServerPort()
        {
            SqlConnection connection = m_dbMgr.Connect();

            if (connection == null)
                return -1;

            string strSQL = "Select Port from HSMSServerPort";
            SqlDataReader reader = m_dbMgr.ExecuteReader(strSQL, connection);

            if (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    int nPort = (int)reader[0];

                    reader.Close();
                    connection.Close();

                    return nPort;
                }
            }

            reader.Close();
            connection.Close();
            return -1;
        }

        // 1달이 경과한 통신로그 삭제
        private void DeleteLogThread()
        {
            try
            {
                string strPath = System.Windows.Forms.Application.ExecutablePath;
                string szParentPath = System.IO.Path.GetDirectoryName(strPath);

                string[] arrFiles = System.IO.Directory.GetFiles(szParentPath + "\\logs");

                string strKey = "HSMSClient.log-";
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
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public void OnDropConnection()
        {
            //lock (this)
            //{
            //m_isConnected = false;
            //m_provider = new ClientProvider(this);
            //}
        }

        public bool CheckLogin(int nID, string szID)
        {
            if (m_provider != null)
            {
                return m_provider.SendCheckUser(nID, szID);
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

        public bool LoginUser(string szID, string szPass, string szCode)
        {
            if (m_provider != null)
            {
                return m_provider.SendLoginUser(szID, szPass, szCode);
            }
            return false;
        }

        public bool RegisterUser(string szMemberID, string szPass, int nUserLevel, ArrayList arrMacAddrList, UnE.KeyValidator.CertOption option)
        {
            if (m_provider != null)
            {
                return m_provider.SendRegisterUser(szMemberID, szPass, nUserLevel, arrMacAddrList, option);
            }
            return false;
        }

        /*public bool SetPassword(string szGenUserID, string szNewPass)
        {
            if (m_provider != null)
            {
                return m_provider.SendSetPassword(szGenUserID, szNewPass);
            }
            return false;
        }*/

        public bool ChangePassword(string szUserID, string strCertCode, string strMacAddrList, string szNewPass)
        {
            if (m_provider != null)
            {
                return m_provider.SendChangePassword(szUserID, strCertCode, strMacAddrList, szNewPass);
            }
            return false;
        }

        /*public bool RequestCode(string szUserID, string szPass)
        {
            if (m_provider != null)
            {
                return m_provider.SendRequestCode(szUserID, szPass);
            }
            return false;
        }*/

        public bool DeleteUser(string szUserID, string szPass)
        {
            if (m_provider != null)
            {
                return m_provider.SendDeleteUser(szUserID, szPass);
            }
            return false;
        }

        public void SendDBDataList(ChangeDataType type, ArrayList arrDatas)
        {
            if (arrDatas.Count == 0)
                return;

            arrDatas.Insert(0, (int)type);
            byte[] bytes = ClientProvider.MakeBytes(TCP_ID.CHANGE_DB_DATA_LIST, arrDatas);

            Send(bytes, this.ClientProvider);
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

        public override bool Write(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.DebugFormat("{0}", str);

            if (IronPython.ConsoleDebugger.Instance.EnableLogger)
                IronPython.ConsoleDebugger.Write(str.ToString(), ConsoleColor.Yellow);
            return true;
        }

		public override bool WriteLine(object str, bool writeTime = true)
        {
            if (logger != null)
                logger.Debug(str);
            if (IronPython.ConsoleDebugger.Instance.EnableLogger)
            {
                IronPython.ConsoleDebugger.WriteLine(str.ToString(), ConsoleColor.Yellow);
                IronPython.ConsoleDebugger.Write(">> ", ConsoleColor.Red);
            }
            return true;
        }
    }
}
