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
using DBUtility2;
using SOPWebClient;

namespace SecomEventReceiver
{
    class NetworkWebManager : /*IMessageQueueOwner,*/ IPostMan
    {
        private PostBox m_postBox = null;
        private bool m_isConnected = false;
        private DateTime m_dtLastSendMessage = new DateTime();

        private int m_nClientType = SOPWebServer.ClientType.SECURITY_SENSOR_SERVER;
        private int m_nClientSubType = SOPWebServer.ClientSubType.S1_SECOM;

        private string m_strServerAddr = "";

        private bool m_shutdownThread = false;

        // Ping은 로그에 남기지 않는다.
        private bool m_exceptPingLog = true;

        private MessageQueue m_msgQueue = new MessageQueue();
        private static log4net.ILog logger = null;

        private static NetworkWebManager m_instance = null;
        private static bool m_isReady = false;

        public static NetworkWebManager Instance
        {
            get { return m_instance; }
        }

        public static bool IsReady
        {
            get { return m_isReady; }
        }

        public NetworkWebManager()
        {
            m_instance = this;

            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            InitLog(); 
            
            int nPort = ReadServerPort();

            SetPostBox(nPort);

            DataManager.Instance.Run();

            Thread t = new Thread(ConnectionThread);
            t.Name = "Server Connection Thread";
            t.Start();
        }

        private int ReadServerPort()
        {
            string strSQL = "Select Port from SensorServerPort where Name = '" + SOPWebServer.ServerPort.SOP_WEB_SERVER + "' and SiteID = " + DataManager.Instance.DBManager.SiteID.ToString();
            ArrayList arrResult = DataManager.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        private void SetPostBox(int nPort)
        {
            if (nPort > 0)
            {
                m_postBox = new PostBox();
                m_postBox.WebServerURL = DataManager.Instance.DBManager.WebServerURL;
                m_postBox.Port = nPort;
                m_postBox.PostMan = this;
            }
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
            DataManager.Instance.Stop();            
        }

        // 서버와의 접속이 끊어지면 다시 연결시킨다.
        private void ConnectionThread()
        {
            m_isReady = true;

            while (!m_shutdownThread)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_postBox != null && m_postBox.Port != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(m_nClientType, m_nClientSubType))
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

#if SERVICE                
#else
                FormMain.Instance.SetServerConnection(DataManager.Instance.DBManager.WebServerURL, m_isConnected);
#endif 

                Thread.Sleep(1000);
                
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

        public void SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData, bool bTest = false)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(nSensorType);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);
            
            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            if (bTest)
                SendMessage(SOPWebServer.Header.SENSOR_DATA_TEST, bytes);
            else
                SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
        }

        public bool SendMessage(int header, byte[] messages)
        {
            if (m_postBox == null || m_isConnected == false)
            {
                m_isConnected = false;
            }
            else
            {
                bool closeConnection;
                bool result = m_postBox.SendMessage(header, messages, out closeConnection);

                if (closeConnection)
                {
                    WriteLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else if (result == true)
                {
                    m_dtLastSendMessage = DateTime.Now;
                    WriteSendLog(header, messages);
                }

                return result;
            }

            return false;
        }

        private void WriteLog(string strLog)
        {
            logger.Debug(strLog);
        }

        private void WriteSendLog(int header, byte[] bytes)
        {
            if (header == SOPWebServer.Header.ARE_YOU_THERE)
                return;

            string strLog = string.Format("SendMessage : Header({0}), Length({1})", header, (int)bytes.Length);
            string strBytes = "";

            for (int i = 0; i < bytes.Length; i++)
            {
                byte b = bytes[i];

                if (strBytes.Length == 0)
                    strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                else
                    strBytes += string.Format(" {0:X2}", (int)b);
            }

            WriteLog(strLog + strBytes);
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

            if (bytes[0] != SOPWebServer.Header.ARE_YOU_THERE || !m_exceptPingLog)
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

        public void OnMessage(int header, byte[] messages)
        {
            
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
