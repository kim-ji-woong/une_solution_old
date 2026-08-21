using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
using TcpLib2;
using SDMS;
using UnE.Sensor;

namespace SecomEventReceiver
{
    class NetworkManager : IMessageQueueOwner
    {
        private ClientProvider m_provider = null;
        private string m_strServerAddr = "";

        private bool shutdownThread = false;

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        private MessageQueue m_msgQueue = new MessageQueue();
        private static log4net.ILog logger = null;

        private static NetworkManager m_instance = null;
        private static bool m_isReady = false;

        public static NetworkManager Instance
        {
            get { return m_instance; }
        }

        public static bool IsReady
        {
            get { return m_isReady; }
        }

        public NetworkManager()
        {
            m_instance = this;

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            InitLog(); 

            string strServerURL = DataManager.Instance.DBManager.WebServerURL;

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

            DataManager.Instance.Run();

            Thread t = new Thread(ConnectionThread);
            t.Name = "Server Connection Thread";
            t.Start();
        }

        private int GetServerPort()
        {
            string strSQL = "Select Port from SDMSServerPort where SiteID = " + DataManager.Instance.SiteID.ToString();
            ArrayList arrResult = DataManager.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nPort = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nPort;
        }

        public void ReleaseThread()
        {
            shutdownThread = true;
            DataManager.Instance.Stop();

            if (m_provider != null && m_provider.IsClientDisposed == false && m_provider.IsConnected)
                m_provider.Close();

            //CloseAllReciverProvider();
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            m_isReady = true;
            bool prevConnected = false;

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
                        int nPort = GetServerPort();

                        if (nPort > 0)
                            m_provider.Connect(m_strServerAddr, nPort);
                    }
                }

                if (m_provider.IsConnected)
                {
                    // 큐에 데이터가 있으면 내보낸다.
                    m_msgQueue.Send(this, m_provider);
                }

                if (prevConnected != m_provider.IsConnected)
                {
                    prevConnected = m_provider.IsConnected;

#if WIN
                    FormMain.Instance.SetServerConnection(m_strServerAddr, m_provider.IsConnected);
#endif
                }

                // 오래된 로그는 삭제한다.
                DataManager.Instance.CheckOldLog();
                Thread.Sleep(1000);
            }
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

        public int Send(byte[] bytes, ClientProvider provider)
        {
            int nResult = provider.Send(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, provider, 0);
        }

        public int Send_NoLengthByte(byte[] bytes, ClientServiceProvider provider)
        {
            int nResult = ((ClientProvider)provider).Send_NoLengthByte(bytes, 0, bytes.Length);
            return WriteSendLog(nResult, bytes, (ClientProvider)provider, 4);
        }

        public void SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, bool bTest = false)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorType);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            short sHeader = bTest ? (short)TCP_ID.TEST_SENSOR_DATA : (short)TCP_ID.SENSOR_DATA;

            byte[] bytes = TcpHelper.MakeBytes(sHeader, arrDatas);
            Send(bytes, m_provider);
        }

        private int WriteSendLog(int nResult, byte[] bytes, ClientProvider provider, int nOffset)
        {
            if (nResult > 0)
            {
                provider.PingCount = 0;

                if (!ConnectionLogEx.Instance.IsOpened)
                    return nResult;

                if (bytes[0] != TCP_ID.I_AM_HERE || !m_exceptPingLog)
                {
                    string strLog = string.Format("SendMessage : Header({0}), Length({1})", (int)bytes[nOffset], (int)bytes.Length);
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
            }

            return nResult;
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
    }

    class ConnectionLogEx : ConnectionLog
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
