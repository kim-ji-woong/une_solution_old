using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Collections;
using System.Threading;

namespace SOPWebClient
{
    using PostBoxService;

    public class PostBox : IDisposable, IPostBoxCallback
    {
        private const string NOT_CONNECTED_EXCEPTION = "서버와의 접속이 끊어졌습니다.\r\n서버 관리자에게 문의하세요.";

        private IPostBox m_proxy = null;
        private DuplexChannelFactory<IPostBox> m_factory = null;

        private string m_strWebServerURL = "";
        private int m_nPort = 50523;
        private IPostMan m_postMan = null;

        private string m_strLastErrorMsg = "";

        public string ErrorMessage
        {
            get { return m_strLastErrorMsg; }
        }

        public string WebServerURL
        {
            get { return m_strWebServerURL; }
            set { SetWebServerURL(value); }
        }

        public int Port
        {
            get { return m_nPort; }
            set
            {
                m_nPort = value;
                SetWebServerURL(m_strWebServerURL);
            }
        }

        public IPostMan PostMan
        {
            get { return m_postMan; }
            set { m_postMan = value; }
        }

        public PostBox()
        {
        }

        public PostBox(string strWebServerURL, int nPort, IPostMan postMan)
        {
            m_nPort = nPort;
            m_postMan = postMan;
            SetWebServerURL(strWebServerURL);
        }

        private void SetWebServerURL(string strSrc)
        {
            strSrc = strSrc.Trim();

            string strTag = "://";

            int nIndex1 = strSrc.IndexOf(strTag);

            if (nIndex1 >= 0)
            {
                string strSrc2 = strSrc.Substring(nIndex1 + strTag.Length);
                int nIndex = strSrc2.LastIndexOf(':');

                if (nIndex >= 0)
                {
                    nIndex += nIndex1 + strTag.Length;
                    m_strWebServerURL = strSrc.Substring(0, nIndex + 1) + m_nPort.ToString();
                }
                else
                {
                    if (strSrc.EndsWith("/"))
                        m_strWebServerURL = strSrc.Substring(0, strSrc.Length - 1) + ":" + m_nPort.ToString();
                    else
                        m_strWebServerURL = strSrc + ":" + m_nPort.ToString();
                }

                m_strWebServerURL = m_strWebServerURL.Replace("http", "net.tcp");
            }
            else
            {
                m_strWebServerURL = strSrc;
            }
        }

        public void Dispose()
        {
            if (m_factory != null)
            {
                try
                {
                    m_factory.Close();
                }
                catch (Exception)
                {
                }

                m_proxy = null;
                m_factory = null;
            }
        }

        public bool Connect(int nClientType, int nClientSubType)
        {
            try
            {
                m_strLastErrorMsg = "";

                DuplexChannelFactory<IPostBox> factory;
                IPostBox proxy = GetProxy(out factory);
                return proxy.Regist(nClientType, nClientSubType);
            }
            catch (Exception  ex)
            {
                m_factory = null;
                m_proxy = null;
                m_strLastErrorMsg = NOT_CONNECTED_EXCEPTION;
            }

            return false;
        }

        public bool SendMessage(int header, byte[] messages, out bool closeConnection)
        {
            closeConnection = false;

            try
            {
                m_strLastErrorMsg = "";

                DuplexChannelFactory<IPostBox> factory;
                IPostBox proxy = GetProxy(out factory);

                int nBytesCount = messages == null ? 0 : messages.Length;
                int nMaxCount = proxy.GetMaxMailSize();

                if (nBytesCount <= nMaxCount)
                {
                    int nResult = proxy.SendMail(header, messages, true);

                    if (nResult == SOPWebServer.ErrorMessageType.SUCCESS)
                        return true;

                    m_strLastErrorMsg = SOPWebServer.ErrorMessageType.ToMessage(nResult);
                    return false;
                }
                else
                {
                    int nSendCount = 0;

                    for (int i = 0; nSendCount < nBytesCount; i++)
                    {
                        int length = nMaxCount;

                        if (nSendCount + length > nBytesCount)
                        {
                            length = nBytesCount - nSendCount;
                        }

                        byte[] bytes = new byte[length];
                        Buffer.BlockCopy(messages, nSendCount, bytes, 0, length);
                        nSendCount += length;

                        int nResult = proxy.SendMail(header, bytes, nSendCount >= nBytesCount);

                        if (nResult != SOPWebServer.ErrorMessageType.SUCCESS)
                        {
                            m_strLastErrorMsg = SOPWebServer.ErrorMessageType.ToMessage(nResult);
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                m_factory = null;
                m_proxy = null;
                m_strLastErrorMsg = e.Message;
                //m_strLastErrorMsg = NOT_CONNECTED_EXCEPTION;
                closeConnection = true;
            }

            return false;
        }

        public void OnRing(int header, byte[] messages)
        {
            if (m_postMan != null)
            {
                // 동기화 문제(Deadlock)를 피하기 위하여 Thread에서 메시지를 처리한다.
                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(m_postMan);
                arrDatas.Add(header);
                arrDatas.Add(messages);

                Thread t = new Thread(new ParameterizedThreadStart(MessageThread));
                t.Start(arrDatas);
                //m_postMan.OnMessage(header, messages);
            }
        }

        private void MessageThread(object arg)
        {
            ArrayList arrDatas = (ArrayList)arg;

            IPostMan postMan = (IPostMan)arrDatas[0];
            int header = (int)arrDatas[1];
            byte[] messages = (byte[])arrDatas[2];

            postMan.OnMessage(header, messages);
        }

        private IPostBox GetProxy(out DuplexChannelFactory<IPostBox> factory)
        {
            if (m_proxy != null)
            {
                factory = m_factory;
                return m_proxy;
            }

            Uri uri = new Uri(m_strWebServerURL + "/PostBoxService");

            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IPostBox)),
                new NetTcpBinding(SecurityMode.None),
                new EndpointAddress(uri));

            factory = new DuplexChannelFactory<IPostBox>(new InstanceContext(this), ep);
            IPostBox proxy = factory.CreateChannel();

            m_proxy = proxy;
            m_factory = factory;
            return proxy;
        }
    }

    public interface IPostMan
    {
        void OnMessage(int header, byte[] messages);
    }
}
