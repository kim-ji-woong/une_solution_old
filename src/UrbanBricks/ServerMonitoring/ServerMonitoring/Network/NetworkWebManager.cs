using DBUtility2;
using SOPWebClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerMonitoring.Network
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private int m_nClientType = SOPWebServer.ClientType.ETC;
        private int m_nClientSubType = SOPWebServer.ClientSubType.UrbanBricks;

        private WebDBManager m_dbMgr = null;
        private string m_strServerURL = "";
        private int m_nPort = -1;

        private bool m_shutdownThread = false;
        private bool m_isConnected = false;
        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        private DateTime m_dtLastSendMessage = new DateTime();

        public NetworkWebManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            m_strServerURL = m_dbMgr.WebServerURL.Replace("http://", "");

            int nPort = GetServerPort();
            SetPostBox(nPort);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Name = "SDMS.Connection";
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

        private void ConnectionThread()
        {
            DateTime dtPrev = DateTime.Now;

            while (!m_shutdownThread)
            {
                if (m_isConnected == true)
                {
                    TimeSpan span = DateTime.Now - m_dtLastSendMessage;

                    // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                    if (span.TotalSeconds > 3.0)
                    {
                        // 접속이 유지되고 있는지 확인한다.
                        if (SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null) == false)
                        {
                            m_isConnected = false;
                            m_postBox.Dispose();
                            m_postBox = null;
                        }
                    }
                }

                if (m_isConnected == false)
                {
                    int nPort = GetServerPort();

                    if (m_postBox == null || (m_postBox != null && m_postBox.Port != nPort))
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(m_nClientType, m_nClientSubType))
                        {
                            m_isConnected = true;
                        }
                    }
                }

                Thread.Sleep(2000);
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
                    //WriteLineLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else if (result == true)
                {
                    m_dtLastSendMessage = DateTime.Now;
                }

                return result;
            }

            return false;
        }

        private int GetServerPort()
        {
            string strSQL = string.Format("Select Port from SensorServerPort where Name = '{0}' and SiteID = {1}"
                , SOPWebServer.ServerPort.SOP_WEB_SERVER, m_dbMgr.SiteID.ToString());

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            m_nPort = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return m_nPort;
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        public void OnMessage(int header, byte[] messages)
        {
        }
    }
}
