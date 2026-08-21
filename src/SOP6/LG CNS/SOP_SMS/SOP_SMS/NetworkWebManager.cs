using System;
using System.Collections.Generic;
using System.Linq;
using SOPWebClient;
using DBUtility2;
using System.Threading;
using System.Collections;

namespace SOP_SMS
{
    public class NetworkWebManager
    {
        private class PostMan : IPostMan
        {
            private PostBox m_postBox = null;
            private NetworkWebManager m_owner = null;
            private int m_nClientType = -1;
            private int m_nClientSubType = -1;
            private bool m_isConnected = false;
            private int m_nPort = -1;
            private DateTime m_dtLastSendMessage = new DateTime();

            public PostBox PostBox
            {
                get { return m_postBox; }
                set
                {
                    m_postBox = value;
                }
            }

            public int ClientType
            {
                get { return m_nClientType; }
            }

            public int ClientSubType
            {
                get { return m_nClientSubType; }
            }

            public bool IsConnected
            {
                get { return m_isConnected; }
                set
                {
                    if (m_isConnected != value)
                    {
                        m_isConnected = value;
                    }
                }
            }

            public int Port
            {
                get { return m_nPort; }
                set { m_nPort = value; }
            }

            public DateTime LastSendMessageTime
            {
                get { return m_dtLastSendMessage; }
            }

            public PostMan(NetworkWebManager owner, int nClientType, int nClientSubType)
            {
                m_owner = owner;
                m_nClientType = nClientType;
                m_nClientSubType = nClientSubType;
            }

            public void OnMessage(int header, byte[] messages)
            {
                if (m_owner != null)
                    m_owner.OnMessage(header, messages, this);
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
                        if (m_owner != null)
                            m_owner.WriteLog(m_postBox.ErrorMessage);

                        m_isConnected = false;
                    }
                    else
                        m_dtLastSendMessage = DateTime.Now;

                    return result;
                }

                return false;
            }
        }

        private PostMan m_postManEtc = null;
        private List<PostMan> m_postManList = new List<PostMan>();
        private WebDBManager m_dbMgr = null;
        private Dictionary<string, int> m_dicMembers = null;
        private string m_strBaseURL = "";
        private int m_nActionStepHistoryID = -1;
        private string m_strMessage = "";

        private bool m_complete = false;
        private static NetworkWebManager m_instance = null;

        public WebDBManager DBMgr
        {
            get { return m_dbMgr; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public bool IsComplete
        {
            get { return m_complete; }
        }

        public static NetworkWebManager Instance
        {
            get { return m_instance; }
        }

        public NetworkWebManager(int nSiteID, string strBaseURL, int nActionStepHistoryID, string strMessage)
        {
            m_instance = this;
            m_dbMgr = new WebDBManager(nSiteID);
            int nPort = ReadServerPort();
            m_strBaseURL = strBaseURL;
            m_nActionStepHistoryID = nActionStepHistoryID;
            m_strMessage = strMessage;

            m_postManEtc = new PostMan(this, SOPWebServer.ClientType.ETC, SOPWebServer.ClientSubType.SMS_SENDER);
            SetPostBox(m_postManEtc, nPort);
            m_postManList.Add(m_postManEtc);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
        }

        public void OnMessage(int header, byte[] messages, object postMan)
        {
            if (postMan != null && postMan is PostMan)
            {
                ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

                if (header == SOPWebServer.Header.CLOSE_CONNECTION)
                {
                    ((PostMan)postMan).IsConnected = false;
                }
                else if (header == SOPWebServer.Header.ARE_YOU_THERE)
                {
                    ((PostMan)postMan).SendMessage(SOPWebServer.Header.I_AM_HERE, null);
                }
            }
        }

        private void ConnectionThread()
        {
            // 최대 5초간 접속 시도
            for (int i=0;i<5;i++)
            {
                foreach (PostMan postMan in m_postManList)
                {
                    if (postMan.IsConnected == false)
                    {
                        int nPort = ReadServerPort();

                        if (postMan.Port != nPort)
                            SetPostBox(postMan, nPort);

                        if (postMan.PostBox != null)
                        {
                            if (postMan.PostBox.Connect(postMan.ClientType, postMan.ClientSubType))
                            {
                                postMan.IsConnected = true;
                            }
                        }
                    }
                    else
                    {
                        if (m_dicMembers != null)
                        {
                            foreach (KeyValuePair<string, int> pair in m_dicMembers)
                            {
                                SendMessage(pair.Key, pair.Value, m_postManEtc);
                            }

                            break;
                        }
                    }
                }

                Thread.Sleep(1000);
            }

            m_complete = true;
        }

        public void SetMembers(Dictionary<string, int> dicMembers)
        {
            m_dicMembers = dicMembers;
        }

        private bool SendMessage(string strPhoneNumber, int nMemberID, PostMan postMan)
        {
            //string strActionStepHistoryID = PersonalSOP.Common.ParameterManager.IDtoString(m_nActionStepHistoryID);
            //string strUserID = PersonalSOP.Common.ParameterManager.IDtoString(nMemberID);

            //string strURL = m_strBaseURL + string.Format("?ash={0}&uid={1}", strActionStepHistoryID, strUserID);
            string strURL = m_strBaseURL + string.Format("?ash={0}&uid={1}", m_nActionStepHistoryID, nMemberID);

            ArrayList arrDatas = new ArrayList();

            // 발신자
            arrDatas.Add("027144133");
            arrDatas.Add(m_strMessage + "\r\n" + strURL);
            arrDatas.Add(1);
            arrDatas.Add(strPhoneNumber);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return postMan.SendMessage(SOPWebServer.Header.SEND_SMS, bytes);
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

        private void SetPostBox(PostMan postMan, int nPort)
        {
            if (nPort > 0)
            {
                PostBox postBox = new PostBox();
                postBox.WebServerURL = m_dbMgr.WebServerURL;
                postBox.PostMan = postMan;
                postMan.PostBox = postBox;

                postMan.Port = nPort;
                postBox.Port = nPort;
            }
        }

        public void RecvLog(int header, byte[] bytes)
        {
        }

        private void SendLog(int header, byte[] bytes)
        {
        }

        public void WriteLog(string strLog)
        {
            //if (m_logger != null)
            //    m_logger.Write(strLog);
        }

        public void Close()
        {
            foreach (PostMan postMan in m_postManList)
            {
                if (postMan.IsConnected)
                {
                    // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                    // 실패하더라도 상관없다.
                    bool closeConnection;
                    postMan.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                    postMan.IsConnected = false;
                }
            }
        }
    }
}
