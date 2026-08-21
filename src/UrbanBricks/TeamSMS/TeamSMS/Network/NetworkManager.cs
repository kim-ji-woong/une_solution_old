using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using SOPWebServer;
using System.Threading;
using DBUtility2;
using System.Collections;

namespace TeamSMS.Network
{
    public class NetworkManager
    {
        private class PostMan : IPostMan
        {
            private PostBox m_postBox = null;
            private NetworkManager m_owner = null;
            private int m_nClientType = -1;
            private int m_nClientSubType = -1;
            private bool m_isConnected = false;
            private int m_nPort = -1;
            private DateTime m_dtLastSendMessage = new DateTime();
            private int m_nPrevIntensity = -1;
            // Key : SensorID
            private Dictionary<int, float> m_dicPrevDataf = new Dictionary<int, float>();

            public PostBox PostBox
            {
                get { return m_postBox; }
                set
                {
                    m_postBox = value;
                    m_nPrevIntensity = -1;
                    m_dicPrevDataf.Clear();
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
                        m_nPrevIntensity = -1;
                        m_dicPrevDataf.Clear();
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

            public int PrevIntensity
            {
                get { return m_nPrevIntensity; }
                set { m_nPrevIntensity = value; }
            }

            public PostMan(NetworkManager owner, int nClientType, int nClientSubType)
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

            public float GetPrevDataf(int nSensorID)
            {
                float fData;

                if (m_dicPrevDataf.TryGetValue(nSensorID, out fData))
                    return fData;

                return -1.0f;
            }

            public void SetPrevDataf(int nSensorID, float fData)
            {
                m_dicPrevDataf[nSensorID] = fData;
            }
        }

        private PostMan m_postManEtc = null;
        private bool m_shutdownThread = false;
        private WebDBManager m_dbMgr = null;

        private static NetworkManager m_instance = null;

        public static NetworkManager Instance
        {
            get { return m_instance; }
        }

        public NetworkManager(WebDBManager dbMgr)
        {
            m_instance = this;
            m_dbMgr = dbMgr;
            int nPort = ReadServerPort();

            m_postManEtc = new PostMan(this, SOPWebServer.ClientType.ETC, SOPWebServer.ClientSubType.SMS_SENDER);
            SetPostBox(m_postManEtc, nPort);

            Thread t = new Thread(new ThreadStart(ConnectionThread));
            t.Start();
        }

        public void OnMessage(int header, byte[] messages, object postMan)
        {
            if (postMan != null && postMan is PostMan)
            {
                ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

                RecvLog(header, messages);

                if (header == SOPWebServer.Header.CLOSE_CONNECTION)
                {
                    ((PostMan)postMan).IsConnected = false;
                }
                else if (header == SOPWebServer.Header.ARE_YOU_THERE)
                {
                    SendMessage(SOPWebServer.Header.I_AM_HERE, null, m_postManEtc);
                }
            }
        }

        private void ConnectionThread()
        {
            while (m_shutdownThread == false)
            {
                if (m_postManEtc.IsConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_postManEtc.Port != nPort)
                        SetPostBox(m_postManEtc, nPort);

                    if (m_postManEtc.PostBox != null)
                    {
                        if (m_postManEtc.PostBox.Connect(m_postManEtc.ClientType, m_postManEtc.ClientSubType))
                        {
                            m_postManEtc.IsConnected = true;
                        }
                    }
                }
                else
                {
                    TimeSpan span = DateTime.Now - m_postManEtc.LastSendMessageTime;

                    // 마지막 메시지를 보낸 이후 3초 이상 지났는지 확인한다.
                    if (span.TotalSeconds > 3.0)
                    {
                        // 접속이 유지되고 있는지 확인한다.
                        m_postManEtc.SendMessage(SOPWebServer.Header.ARE_YOU_THERE, null);
                    }
                }

                Thread.Sleep(1000);
            }
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

        private bool SendMessage(int header, byte[] messages, PostMan postMan)
        {
            if (postMan.IsConnected)
            {
                SendLog(header, messages);
                return postMan.SendMessage(header, messages);
            }

            return false;
        }

        public void RecvLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "RecvMessage");
        }

        private void SendLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "SendMessage");
        }

        private void MessageLog(int header, byte[] bytes, string strMessageTag)
        {
            if (header != SOPWebServer.Header.ARE_YOU_THERE &&
                header != SOPWebServer.Header.I_AM_HERE)
            {
                string strLog = "";

                if (bytes == null)
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length(0)", header);
                }
                else
                {
                    strLog = string.Format(strMessageTag + " : Header({0}), Length({1})", header, bytes.Length);
                    string strBytes = "";

                    foreach (byte b in bytes)
                    {
                        if (strBytes.Length == 0)
                            strBytes = string.Format("\r\n\t\t{0:X2}", (int)b);
                        else
                            strBytes += string.Format(" {0:X2}", (int)b);
                    }

                    strLog += strBytes;
                }

                WriteLog(strLog);
            }
        }

        public void WriteLog(string strLog)
        {
        }

        public void Close()
        {
            if (m_postManEtc.IsConnected)
            {
                // 종료 메시지니까 PostMan이 아니라 PostBox에 직접 보낸다.
                // 실패하더라도 상관없다.
                bool closeConnection;
                m_postManEtc.PostBox.SendMessage(SOPWebServer.Header.CLOSE_CONNECTION, null, out closeConnection);
                m_postManEtc.IsConnected = false;
            }

            m_shutdownThread = true;
        }

        public bool SendSMS(string strCaller, List<string> phoneNumbers, string strMessage)
        {
            if (phoneNumbers == null || phoneNumbers.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("총 0명에게 문자메시지를 전송하였습니다."));
                return false;
            }

            // 접속이 이루어질때까지 최대 5초동안 기다린다.
            for (int i = 0; i < 5; i++)
            {
                if (m_postManEtc.IsConnected)
                    break;

                System.Threading.Thread.Sleep(1000);
            }

            if (m_postManEtc.IsConnected == false)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("서버와 연결이 끊어졌습니다."));
                return false;
            }

            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(strCaller);
            arrDatas.Add(strMessage);
            arrDatas.Add(phoneNumbers.Count);

            int nCount = 0;

            foreach (string strPhoneNumber in phoneNumbers)
            {
                if (strPhoneNumber.Length > 0)
                {
                    arrDatas.Add(TeamManager.GetPhoneNumber(strPhoneNumber));
                    nCount++;
                }
            }

            if (nCount == 0)
            {
                System.Windows.Forms.MessageBox.Show(string.Format("총 0명에게 문자메시지를 전송하였습니다."));
            }
            else
            {
                arrDatas[2] = nCount;

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                bool result = m_postManEtc.SendMessage(SOPWebServer.Header.SEND_SMS, bytes);

                if (result == true)
                    System.Windows.Forms.MessageBox.Show(string.Format("총 {0}명에게 문자메시지를 전송하였습니다.", nCount));
                else
                    System.Windows.Forms.MessageBox.Show(string.Format("문자메시지를 전송하지 못하였습니다."));

                return result;
            }

            return true;
        }
    }
}
