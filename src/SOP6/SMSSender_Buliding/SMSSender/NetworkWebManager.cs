using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using DBUtility2;
using System.Collections;
using System.Threading;

namespace SMSSender
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private WebDBManager m_dbMgr = null;
        private bool m_shutdownThread = false;
        private bool m_isConnected = false;
        private int m_nPort = -1;

        public NetworkWebManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            
            int nPort = ReadServerPort();
            SetPostBox(nPort);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
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
            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_nPort != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(SOPWebServer.ClientType.ETC, SOPWebServer.ClientSubType.SMS_SENDER))
                        {
                            m_isConnected = true;
                        }
                    }
                }
                else
                    SendPing();
                
                Thread.Sleep(1000);
            }
        }

        public void ReleaseThread()
        {
            m_shutdownThread = true;
        }

        public void OnMessage(int header, byte[] messages)
        {
            ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

            if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                m_isConnected = false;
            }
        }

        public void SendPing()
        {
            if (m_isConnected && m_postBox != null)
            {
                bool closeConnection;
                m_postBox.SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null, out closeConnection);

                if (closeConnection)
                    m_isConnected = false;
            }
        }

        public void SendSMS(string strCaller, string szMessage, List<string> memberList)
        {
            if (m_isConnected && m_postBox != null)
            {
                ArrayList arrDatas = new ArrayList();

                arrDatas.Add(strCaller);
                arrDatas.Add(szMessage);
                arrDatas.Add(memberList.Count);

                foreach (string strPhoneNumber in memberList)
                {
                    arrDatas.Add(strPhoneNumber);
                }

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);

                bool closeConnection;
                m_postBox.SendMessage(SOPWebServer.Header.SEND_SMS, bytes, out closeConnection);

                if (closeConnection)
                    m_isConnected = false;
            }
        }
    }
}
