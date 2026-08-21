using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TcpLib2;
using SDMS;
using DBUtility;
using System.Collections;

namespace ServerCommander
{
    public class NetworkManager
    {
        private Thread conThread = null;

        private ClientProvider m_provider = null;
        private int m_nPort = -1;
        private string m_strServerAddr = "";

        private bool shutdownThread = false;

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        private DBUtility.WebDBManager m_dbMgr = null;

        private void WriteLog(object str)
        {
            //if (ConnectionLogEx.Instance.IsOpened)
            //    ConnectionLogEx.Instance.Write(str);
        }

        private void WriteLineLog(object str)
        {
            //if (ConnectionLogEx.Instance.IsOpened)
            //    ConnectionLogEx.Instance.WriteLine(str);
        }

        private void InitLog()
        {
            ConnectionLogEx.MakeInstance();
        }

        public void RecvLog(byte[] bytes)
        {
            /*if (!ConnectionLog.Instance.IsOpened)
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
            }*/
        }

        public int Send(byte[] bytes, ClientProvider provider)
        {
            int nResult = provider.Send(bytes, 0, bytes.Length);

            if (nResult > 0)
            {
                /*if (!ConnectionLog.Instance.IsOpened)
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
                }*/
            }
            return nResult;
        }

        public void SendRunSDMS()
        {
            m_provider.SendRunSDMS();
        }

        public void UpdateSystem()
        {
            m_provider.SendUpdateSystem();
        }

        private int m_nSiteID = 1;
        private void ReadSiteID()
        {
            DBUtility.Utility util = new DBUtility.Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {                
                return;
            }

            int nSiteId = 1;
            if (int.TryParse(szSiteID, out nSiteId))
            {
                m_nSiteID = nSiteId;
            }            
        }

        public NetworkManager()
        {
            InitLog();


            ReadSiteID();

            m_dbMgr = new WebDBManager(m_nSiteID);

            string strServerURL = RegUtil.ReadRegValue("Server Connection Info", "webserver_url", m_nSiteID);

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

                string strKey = "ServerCommander.log-";
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
                        m_nPort = GetServerPort();
                        
                        if (m_nPort > 0)
                            m_provider.Connect(m_strServerAddr, m_nPort);

                        FormMain.Instance.SetConnection(m_provider.IsConnected, m_strServerAddr, m_nPort.ToString());
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
            if (obj.GetType() == typeof(Exception))
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
            if (obj.GetType() == typeof(Exception))
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
