using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DBUtility2;
using SOPWebClient;
using ETCSensorServer.Data;

namespace ETCSensorServer.Network
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private bool m_isConnected = false;
        private DateTime m_dtLastSendMessage = new DateTime();

        private int m_nClientType = SOPWebServer.ClientType.ETC;
        private int m_nClientSubType = SOPWebServer.ClientSubType.Parc1;

        private WebDBManager m_dbMgr = null;

        //private log4net.ILog logger = null;

        private bool m_shutdownThread = true;

        public NetworkWebManager(WebDBManager dbMgr)
        {
            //logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            this.m_dbMgr = dbMgr;

            int nPort = ReadServerPort();
            SetPostBox(nPort);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
        }
        
        private void SetPostBox(int nPort)
        {
            m_postBox = new PostBox();
            m_postBox.WebServerURL = m_dbMgr.WebServerURL;
            m_postBox.Port = nPort;
            m_postBox.PostMan = this;
        }

        private void ConnectionThread()
        {
            m_shutdownThread = false;

            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_postBox != null && m_postBox.Port != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(m_nClientType, m_nClientSubType))
                            m_isConnected = true;
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

        public void OnMessage(int header, byte[] messages)
        {
            
        }

        private int ReadServerPort()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select Port from SensorServerPort ");
            sb.AppendFormat("Where Name='{0}' And SiteID={1} ", SOPWebServer.ServerPort.SOP_WEB_SERVER, m_dbMgr.SiteID);

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());
            if (port == null)
                return -1;

            return port.Data;
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

        public bool SendSensorData(SensorTagInfo sensor, int nSensorData)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add((int)sensor.SensorType);
            arrDatas.Add(sensor.ID);
            arrDatas.Add(sensor.SensorZoneID);
            arrDatas.Add(nSensorData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
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

        private void WriteLog(string strLog)
        {
            //logger.Debug(strLog);
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }
    }
}
