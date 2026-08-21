using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using DBUtility2;
using SOPWebClient;

namespace SDMSCommander.Network
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private WebDBManager m_dbMgr = null;
        private bool m_shutdownThread = false;
        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        private Thread conThread = null;
        private DateTime m_dtLastSendMessage = new DateTime();

        private int m_nPort = -1;
        private string m_strServerAddr = "";
        
        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;
        
        public NetworkWebManager(WebDBManager dbMgr, int nSiteID)
        {
            m_dbMgr = dbMgr;

            int nPort = ReadServerPort();
            SetPostBox(nPort);

            conThread = new Thread(ConnectionThread);
            conThread.Start();

            // 시간이 경과한 로그 삭제
            Thread t = new Thread(DeleteLogThread);
            t.Start();
        }

        private void SetPostBox(int nPort)
        {
            if (nPort > 0)
            {
                m_postBox = new PostBox();
                m_postBox.WebServerURL = m_dbMgr.WebServerURL;
                m_postBox.PostMan = this;

                m_nPort = nPort;
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

        // 1달이 경과한 통신로그 삭제
        private void DeleteLogThread()
        {
            try
            {
                string strPath = System.Windows.Forms.Application.ExecutablePath;
                string szParentPath = System.IO.Path.GetDirectoryName(strPath);

                string[] arrFiles = System.IO.Directory.GetFiles(szParentPath + "\\logs");

                //string strKey = "IntegratedManager.log-";
                //int len = strKey.Length;

                List<string> keys = new List<string>();
                keys.Add("SOPManager.log-");

                DateTime dtNow = DateTime.Now;
                //int nYear, nMonth, nDay;

                foreach (string strFile in arrFiles)
                {
                    foreach (string strKey in keys)
                    {
                        int len = strKey.Length;

                        if (DeleteLogFile(strFile, strKey, len, dtNow))
                            break;

                        /*int nIndex = strFile.IndexOf(strKey);

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
                            System.IO.File.Delete(strFile);*/
                    }
                }
            }
            catch (System.IO.DirectoryNotFoundException)
            {
            }
        }

        private bool DeleteLogFile(string strFile, string strKey, int len, DateTime dtNow)
        {
            int nYear, nMonth, nDay;
            int nIndex = strFile.IndexOf(strKey);

            if (nIndex < 0)
                return false;

            string strDate = strFile.Substring(nIndex + len);

            int nIndex1 = strDate.IndexOf('-');
            int nIndex2 = strDate.LastIndexOf('-');

            if (nIndex1 < 0 || nIndex2 < 0 || nIndex1 == nIndex2)
                return false;

            string strYear = strDate.Substring(0, nIndex1);
            string strMonth = strDate.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            string strDay = strDate.Substring(nIndex2 + 1);

            if (!int.TryParse(strYear, out nYear))
                return false;
            if (!int.TryParse(strMonth, out nMonth))
                return false;
            if (!int.TryParse(strDay, out nDay))
                return false;

            if (IsPassedTime(dtNow, nYear, nMonth, nDay))
            {
                System.IO.File.Delete(strFile);
                return true;
            }

            return false;
        }

        private int ReadServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
            try
            {
                if (conThread != null)
                    conThread.Join();
            }
            catch (System.Exception)
            {
            }
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            while (!m_shutdownThread)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_nPort != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(SOPWebServer.ClientType.SOP_COMMANDER, SOPWebServer.ClientSubType.SOP_COMMANDER))
                        {
                            m_isConnected = true;
                        }
                    }
                }
                else
                {
                    TimeSpan span = DateTime.Now - m_dtLastSendMessage;

                    // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                    if (span.TotalSeconds > 3.0)
                    {
                        // 접속이 유지되고 있는지 확인한다.
                        SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                    }
                }
                
                Thread.Sleep(1000);
            }
        }

        public bool SendMessage(int header, byte[] messages)
        {
            if (m_postBox == null || m_isConnected == false)
            {
                m_isConnected = false;
            }
            else
            {
                SendLog(header, messages);

                bool closeConnection;
                bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                if (closeConnection)
                {
                    //WriteLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else
                    m_dtLastSendMessage = DateTime.Now;

                return result;
            }

            return false;
        }

        private void SendLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "SendMessage");
        }

        private void MessageLog(int header, byte[] bytes, string strMessageTag)
        {
            //if (!ConnectionLogEx.Instance.IsOpened)
            //    return;

            //if (header != SOPWebServer.Header.ARE_YOU_THERE)
            //{
            //    string strLog = "";

            //    if (bytes == null)
            //    {
            //        strLog = string.Format(strMessageTag + " : Header({0}), Length(0)", header);
            //    }
            //    else
            //    {
            //        strLog = string.Format(strMessageTag + " : Header({0}), Length({1})", header, bytes.Length);
            //        string strBytes = "";

            //        foreach (byte b in bytes)
            //        {
            //            if (strBytes.Length == 0)
            //                strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
            //            else
            //                strBytes += string.Format(" {0:X2}", (int)b);
            //        }

            //        strLog += strBytes;
            //    }

            //    WriteLineLog(strLog);
            //}
        }

        //public void OnDropConnection()
        //{
        //    lock (this)
        //    {
        //        m_provider = new ClientProvider(this);
        //    }
        //}

        private void CopyBytes(byte[] bytesDest, ref int nDestOffset, byte[] bytesSrc)
        {
            int nLength = bytesSrc.Length;
            System.Buffer.BlockCopy(bytesSrc, 0, bytesDest, nDestOffset, nLength);
            nDestOffset += nLength;
        }

        public void OnMessage(int header, byte[] messages)
        {
            //throw new NotImplementedException();
        }

        public void SendUpdateSystem()
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(SOPWebServer.ServerCommandType.UPDATE_SYSTEM);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            SendMessage(SOPWebServer.Header.SERVER_COMMAND, bytes);
            //Send(bytes, 0, bytes.Count());
        }
    }

    /*public class ConnectionLogEx : ConnectionLog
    {
        private log4net.ILog logger = null;
        private static ConnectionLogEx m_instance2 = new ConnectionLogEx();

        public static ConnectionLogEx Instance
        {
            get { return m_instance2; }
        }

        public static bool MakeInstance()
        {
            m_instance2.logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            m_instance2.m_isOpened = true;
            return m_instance2.m_isOpened;
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
    }*/
}
