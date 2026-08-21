using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SOPWebClient;
using DBUtility2;
using System.Collections;
using System.Threading;

namespace SOPSimulator.Network
{
    public class NetworkWebManager : IPostMan
    {
        private PostBox m_postBox = null;
        private WebDBManager m_dbMgr = null;
        private bool m_shutdownThread = false;
        private bool m_isConnected = false;
        private int m_nPort = -1;
        private DateTime m_dtLastSendMessage = new DateTime();

        private string m_strTryLoginID = null;
        private string m_strTryLoginPW = null;

        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        public NetworkWebManager(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
            InitLog();

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

        private string GetSOPWebServerURL()
        {
            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'SOPWebServerURL' and SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return m_dbMgr.WebServerURL;

            string strWebServerURL = WebDBManager.GetStringField(arrResult[0]);

            if (strWebServerURL == null)
                return m_dbMgr.WebServerURL;

            return strWebServerURL;
        }

        private void SetPostBox(int nPort)
        {
            if (nPort > 0)
            {
                m_postBox = new PostBox();
                m_postBox.WebServerURL = GetSOPWebServerURL();
                m_postBox.PostMan = this;

                m_nPort = nPort;
            }
        }

        private void ConnectionThread()
        {
            int nPrevMonth = DateTime.Now.Month;

            while (m_shutdownThread == false)
            {
                if (m_isConnected == false)
                {
                    int nPort = ReadServerPort();

                    if (m_nPort != nPort)
                        SetPostBox(nPort);

                    if (m_postBox != null)
                    {
                        if (m_postBox.Connect(SOPWebServer.ClientType.LOGIN_SERVER, SOPWebServer.ClientSubType.INTEGRATED_MANAGER))
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

                    if (m_strTryLoginID != null && m_strTryLoginPW != null)
                    {
                        LoginUser(m_strTryLoginID, m_strTryLoginPW);
                        m_strTryLoginID = m_strTryLoginPW = null;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void WriteLog(object str)
        {
        }

        private void WriteLineLog(object str)
        {
        }

        private void InitLog()
        {
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
                    WriteLog(m_postBox.ErrorMessage);
                    m_isConnected = false;
                }
                else
                    m_dtLastSendMessage = DateTime.Now;

                return result;
            }

            return false;
        }

        public void OnMessage(int header, byte[] messages)
        {
            if (m_shutdownThread)
                return;

            ArrayList arrDatas = messages == null ? null : SOPWebServer.BinaryHelper.ReadBytes(messages);

            if (header == SOPWebServer.Header.CLOSE_CONNECTION)
            {
                m_isConnected = false;
            }
            else if (header == SOPWebServer.Header.ACCEPT_LOGIN)
            {
                ProcessAcceptLogin(arrDatas);
            }
            else if (header == SOPWebServer.Header.REJECT_LOGIN)
            {
                ProcessRejectLogin(arrDatas);
            }
            
            RecvLog(header, messages);
        }

        private void ProcessRejectLogin(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 1 && arrDatas[0] is int)
            {
                int nErrorMessage = (int)arrDatas[0];
                string strErrorMessage = "";

                if (nErrorMessage == SOPWebServer.ErrorMessageType.INVALID_ID_OR_PASSWORD)
                {
                    nErrorMessage = 1;
                    strErrorMessage = "아이디 혹은 비밀번호가 잘못 입력되었습니다.";
                }
                else if (nErrorMessage == SOPWebServer.ErrorMessageType.ALREADY_USING_ID)
                {
                    nErrorMessage = 2;
                    strErrorMessage = "해당 아이디는 이미 로그인 중입니다.";
                }
                else
                {
                    nErrorMessage = 3;
                    strErrorMessage = "로그인에 실패하였습니다.";
                }

                FormLogin.Instance.ReceiveLoginResult(false, strErrorMessage);
            }
        }

        private void ProcessAcceptLogin(ArrayList arrDatas)
        {
            if (arrDatas != null && arrDatas.Count >= 3 && arrDatas[0] is int && arrDatas[1] is string && arrDatas[2] is string)
            {
                int nSOPGenUserID = (int)arrDatas[0];
                string strUserName = (string)arrDatas[1];
                string strNickName = (string)arrDatas[2];

                FormLogin.Instance.ReceiveLoginResult(true, nSOPGenUserID.ToString() + "_" + strNickName);
            }
        }

        private void RecvLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "RecvMessage");
        }

        private void SendLog(int header, byte[] bytes)
        {
            MessageLog(header, bytes, "SendMessage");
        }

        private void MessageLog(int header, byte[] bytes, string strMessageTag)
        {
        }

        public bool LoginUser(string szID, string szPass)
        {
            if (m_isConnected == false)
            {
                m_strTryLoginID = szID;
                m_strTryLoginPW = szPass;
                return false;
            }

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(szID);
            arrDatas.Add(szPass);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            return SendMessage(SOPWebServer.Header.LOGIN_USER, bytes);
        }

        public void Close()
        {
            m_shutdownThread = true;
        }
    }
}
