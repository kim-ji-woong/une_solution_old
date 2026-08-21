using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using DBUtility2;
using System.Collections;
using System.Threading;

namespace BlackoutServer.Network
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private bool m_isConnected = false;
        private DateTime m_dtLastSendMessage = new DateTime();

        private int m_nClientType = SOPWebServer.ClientType.ETC;
        private int m_nClientSubType = SOPWebServer.ClientSubType.OFFICE_BUILDING;

        private bool m_shutdownThread = true;
        private WebDBManager m_dbMgr = null;

        public PostBox PostBox
        {
            get { return m_postBox; }
        }

        private void WriteLog(string strLog)
        {
            Logger.Instance.Write(strLog);
        }

        public NetworkWebManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            int nPort = ReadServerPort(m_dbMgr);

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

        private int ReadServerPort(WebDBManager dbMgr)
        {
            string strSQL = string.Format("Select Port from SensorServerPort where Name = '{0}' and SiteID = {1}", SOPWebServer.ServerPort.SOP_WEB_SERVER, dbMgr.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            VariousData<int> port = WebDBManager.GetIntField(arrResult[0].ToString());

            if (port == null)
                return -1;

            return port.Data;
        }

        public void OnMessage(int header, byte[] messages)
        {
        }

        private void ConnectionThread()
        {
            m_shutdownThread = false;

            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort(m_dbMgr);

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

        private void WriteSendLog(int header, byte[] bytes)
        {
            if (header == SOPWebServer.Header.ARE_YOU_THERE)
                return;

            if (bytes == null)
            {
                string strLog = string.Format("SendMessage : Header({0})", header);
                WriteLog(strLog);
            }
            else
            {
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
        }

        public void Close()
        {
            if (m_isConnected)
            {
                // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                // 실패하더라도 상관없다.
                bool closeConnection;
                m_postBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_isConnected = false;
            }

            m_shutdownThread = true;
        }

        public bool SendSensorData(int nSensorZoneID, int nSensorTagInfoID, int nSensorType, int nData)
        {
            // SOP서버로 연결된 Provider로 전송
            if (m_isConnected == false)
                return false;

            if (nSensorZoneID < 0)
                return false;

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nSensorType);
            arrDatas.Add(nSensorTagInfoID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(nData);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.SENSOR_DATA, bytes);
        }

        public bool SendAllClear()
        {
            return SendMessage(SOPWebServer.Header.CLEAR_DETECT_ALL, null);
        }
    }
}
