using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libSMS
{
    internal class MessageClientDummy : IMessageClient
    {
        private string m_szServerIP = "";
        private MessageBroker m_broker = null;
        private int m_nSiteID = -1;

        internal MessageClientDummy(string szServerIP, int nSiteID)
        {           
        }

        public void Dispose()
        {

        }

        public bool SendSMS(string szCaller, string szReciver, string szContent, bool bEncryptCaller = false)
        {
            return true;
        }

        public bool SendSMS(List<MessageContent> arMessages)
        {            
            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            return 140;
        }

        // 첫번째 메시지를 보낼때 호출
        public void BeginSend()
        {
        }

        // 마지막 메시지를 보낸후 호출
        public void EndSend()
        {
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return false;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(string szCaller, string szReciver, string szContent, string strTitle = "", MessageContentMMS.ContentType contentType = MessageContentMMS.ContentType.None, string strContentPath = "")
        {
            return false;
        }

        public bool SendMMS(List<MessageContentMMS> arMessages)
        {
            return false;
        }
    }

    // ezSMS를 사용하는 버전
    internal class MessageClientUNE : IMessageClient
    {
        private string m_szServerIP = "";
        private MessageBrokerUNE m_broker = null;
        private int m_nSiteID = -1;

        public MessageClientUNE(string szServerIP, int nSiteID)
        {
            m_szServerIP = szServerIP;
            m_nSiteID = nSiteID;
            m_broker = new MessageBrokerUNE(szServerIP);
        }

        public void Dispose()
        {
        }

        public bool SendSMS(string szCaller, string szReciver, string szContent, bool bEncryptCaller = false)
        {
            if (m_broker != null)
            {
                MessageContent sms = new MessageContent();
                sms.Caller = szCaller;
                sms.Reciver = szReciver;
                sms.EncryptCaller = bEncryptCaller;
                sms.SmsTag = DateTime.Now.ToLongTimeString();


                string szMsg = szContent;
                sms.Message = szMsg;

                m_broker.InsertMessage(sms);
                return true;
            }
            return false;
        }

        public bool SendSMS(List<MessageContent> arMessages)
        {
            if (arMessages == null)
                return false;

            foreach (MessageContent content in arMessages)
            {
                if (m_broker != null)
                {
                    m_broker.InsertMessage(content);
                }
            }
            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            return 120;
        }

        // 첫번째 메시지를 보낼때 호출
        public void BeginSend()
        {
        }

        // 마지막 메시지를 보낸후 호출
        public void EndSend()
        {
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return false;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(string szCaller, string szReciver, string szContent, string strTitle = "", MessageContentMMS.ContentType contentType = MessageContentMMS.ContentType.None, string strContentPath = "")
        {
            return false;
        }

        public bool SendMMS(List<MessageContentMMS> arMessages)
        {
            return false;
        }
    }

    // ezSMS를 사용하는 버전
    internal class MessageClientEzSMS : IMessageClient
    {
        private MessageBrokerEzSMS m_broker = null;

        public MessageClientEzSMS(string strUserID, string strPassword, string strCaller)
        {
            m_broker = new MessageBrokerEzSMS(strUserID, strPassword, strCaller);
        }

        public void Dispose()
        {
        }

        public bool SendSMS(string szCaller, string szReciver, string szContent, bool bEncryptCaller = false)
        {
            if (m_broker != null)
            {
                MessageContent sms = new MessageContent();
                sms.Caller = szCaller;
                sms.Reciver = szReciver;
                sms.EncryptCaller = bEncryptCaller;
                sms.SmsTag = DateTime.Now.ToLongTimeString();


                string szMsg = szContent;
                sms.Message = szMsg;

                m_broker.InsertMessage(sms);
                return true;
            }
            return false;
        }

        public bool SendSMS(List<MessageContent> arMessages)
        {
            if (arMessages == null)
                return false;

            foreach (MessageContent content in arMessages)
            {
                if (m_broker != null)
                {
                    m_broker.InsertMessage(content);
                }
            }
            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            return 120;
        }

        // 첫번째 메시지를 보낼때 호출
        public void BeginSend()
        {
        }

        // 마지막 메시지를 보낸후 호출
        public void EndSend()
        {
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return false;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(string szCaller, string szReciver, string szContent, string strTitle = "", MessageContentMMS.ContentType contentType = MessageContentMMS.ContentType.None, string strContentPath = "")
        {
            return false;
        }

        public bool SendMMS(List<MessageContentMMS> arMessages)
        {
            return false;
        }
    }

    // LGU+
    internal class MessageClientLGU : IMessageClient
    {
        private int m_msgBufCount = 1;
        
        private List<string> m_rcvPhoneNumbers = new List<string>();
        private string m_strCaller = ""; // 발신번호 고정
        private string m_strMessage = "";
        // 메시지가 m_nSMSLimit 바이트 이상이면 MMS로 보낸다.
        private DBUtility.VariousData<bool> m_isMMS = null;
        private int m_nSMSLimit = 90;
        private bool m_isBegin = false;
        private MessageBrokerLGU m_broker = null;
        private List<string> m_paths = new List<string>();

        private string m_strTitle = "";
        private List<KeyValuePair<MessageContentMMS.ContentType, string>> m_arrContents = null;

        public MessageClientLGU(string szServerIP, DBUtility.WebDBManager.DBType dbType, string strDBName, string strDBID, string strPassword, string strUserID, string strCaller)
        {
            m_broker = new MessageBrokerLGU(szServerIP, dbType, strDBName, strDBID, strPassword, strUserID, strCaller);
            m_strCaller = strCaller;
        }

        public void Dispose()
        {

        }

        public bool SendSMS(string szCaller, string szReciver, string szContent, bool bEncryptCaller = false)
        {
            if (m_broker != null)
            {
                m_arrContents = null;
                AddBuffer(szCaller, szReciver, szContent); 
                 
                /*string szResult = m_snuMsg.SendSmsMessage(szCaller, szReciver, szContent);
                if (szResult.StartsWith("-"))
                    return false;*/
            }
            return true;
        }

        public bool SendSMS(List<MessageContent> arMessages)
        {
            if (arMessages == null)
                return false;

            BeginSend();

            foreach (MessageContent content in arMessages)
            {
                if (m_broker != null)
                {
                    AddBuffer(content.Caller, content.Reciver, content.Message);
                    //m_broker.SendSmsMessage(content.Caller, content.Reciver, content.Message);
                }
            }

            EndSend();
            return true;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(string szCaller, string szReciver, string szContent, string strTitle = "", MessageContentMMS.ContentType contentType = MessageContentMMS.ContentType.None, string strContentPath = "")
        {
            if (m_broker != null)
            {
                if (szContent.Length <= 5)
                    m_strTitle = szContent;
                else
                    m_strTitle = szContent.Substring(0, 5);

                if (m_arrContents == null)
                {
                    m_arrContents = new List<KeyValuePair<MessageContentMMS.ContentType, string>>();
                    m_arrContents.Add(new KeyValuePair<MessageContentMMS.ContentType, string>(contentType, strContentPath));
                }

                AddBuffer(szCaller, szReciver, szContent, strTitle, m_arrContents);
            }
            return true;
        }

        public bool SendMMS(List<MessageContentMMS> arMessages)
        {
            if (arMessages == null)
                return false;

            BeginSend();

            foreach (MessageContentMMS content in arMessages)
            {
                if (m_broker != null)
                {
                    AddBuffer(content.Caller, content.Reciver, content.Message, content.Title, content.ContentsList);
                }
            }

            EndSend();
            return true;
        }

        private void AddBuffer(string strCaller, string strReceiver, string strMessage)
        { 
            if (m_isMMS == null)
            {
                if (IsSMSMessage(strMessage))
                    m_isMMS = new DBUtility.VariousData<bool>(false);
                else
                    m_isMMS = new DBUtility.VariousData<bool>(true);
            }
            m_strCaller = strCaller; 
            m_strMessage = strMessage;
            m_rcvPhoneNumbers.Add(strReceiver);
            if (strMessage.Length <= 5)
                m_strTitle = strMessage;
            else
                m_strTitle = strMessage.Substring(0, 5);

            if (m_rcvPhoneNumbers.Count >= m_msgBufCount || m_isBegin == false)
            {
                if (m_isMMS.Data) 
                    m_broker.SendLMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strTitle, m_strMessage); 
                else
                    m_broker.SendSMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strMessage);

                m_rcvPhoneNumbers.Clear();
            } 
        }

        //private void AddBuffer(string strCaller, string strReceiver, string strMessage, string strTitle)
        //{
        //    m_strCaller = strCaller;
        //    m_strMessage = strMessage;
        //    if (strMessage.Length <= 5)
        //        m_strTitle = strMessage;
        //    else
        //        m_strTitle = strMessage.Substring(0, 5); 

        //    m_rcvPhoneNumbers.Add(strReceiver);

        //    if (m_rcvPhoneNumbers.Count >= m_msgBufCount || m_isBegin == false)
        //    {
        //        if (m_paths.Count > 0)
        //        {
        //            m_broker.SendMMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strTitle, m_strMessage, m_paths);

        //            m_rcvPhoneNumbers.Clear();
        //            m_paths.Clear();
        //        } 
        //    }  
        //}

        private void AddBuffer(string strCaller, string strReceiver, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentsData)
        {
            if (m_isMMS == null || m_isMMS.Data == false)
            {
                m_isMMS = new DBUtility.VariousData<bool>(true);
            }

            m_strCaller = strCaller;
            m_strMessage = strMessage;
            m_rcvPhoneNumbers.Add(strReceiver);
            if (strMessage.Length <= 5)
                m_strTitle = strMessage;
            else
                m_strTitle = strMessage.Substring(0, 5);
            m_arrContents = contentsData;

            if (m_rcvPhoneNumbers.Count >= m_msgBufCount || m_isBegin == false)
            {
                m_broker.SendMMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strTitle, m_strMessage, m_arrContents);
                m_rcvPhoneNumbers.Clear();
            }
        }

        private bool IsSMSMessage(string strMsg)
        {
            int nByteLength = 0;
            int nLen = strMsg.Length;

            for (int i = 0; i < nLen; i++)
            {
                if (strMsg.ElementAt(i) < 256)
                    nByteLength++;
                else
                    nByteLength += 2;
            }

            if (nByteLength <= m_nSMSLimit)
                return true;

            return false;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            // MMS의 최대길이
            return 4000;
        }

        // 첫번째 메시지를 보낼때 호출
        public void BeginSend()
        {
            m_rcvPhoneNumbers.Clear();
            m_strCaller = "";
            m_strMessage = "";

            m_arrContents = null;
            m_strTitle = "";

            m_isBegin = true;
        }

        // 마지막 메시지를 보낸후 호출
        public void EndSend()
        {
            if (m_broker != null && m_isMMS != null && m_rcvPhoneNumbers.Count > 0)
            {
                if (m_isMMS.Data)
                {
                    if (m_arrContents == null)
                        m_broker.SendLMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strTitle, m_strMessage);
                    else 
                        m_broker.SendMMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strTitle, m_strMessage, m_arrContents);
                }
                else
                    m_broker.SendSMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strMessage);
            }

            m_isMMS = null;
            m_rcvPhoneNumbers.Clear();
            m_isBegin = false;

            m_strTitle = "";
            m_arrContents = null;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        } 
    }

    // 모노 커뮤니케이션즈(KT 크로샷)
    internal class MessageClientMCS : IMessageClient
    {
        private string m_strCaller = "";
        private int m_msgBufCount = 100;

        private List<string> m_rcvPhoneNumbers = new List<string>();
        private string m_strMessage = "";
        // 메시지가 m_nSMSLimit 바이트 이상이면 MMS로 보낸다.
        private DBUtility.VariousData<bool> m_isMMS = null;
        private int m_nSMSLimit = 90;
        private bool m_isBegin = false;
        private MessageBrokerMCS m_broker = null;

        private string m_strTitle = "";
        private List<KeyValuePair<MessageContentMMS.ContentType, string>> m_arrContents = null;

        public MessageClientMCS(string szServerIP, DBUtility.WebDBManager.DBType dbType, string strDBName, string strDBID, string strPassword, string strUserID, string strCaller)
        {
            m_broker = new MessageBrokerMCS(szServerIP, dbType, strDBName, strDBID, strPassword, strUserID, strCaller);
            m_strCaller = strCaller;
        }

        public void Dispose()
        {

        }

        public bool SendSMS(string szCaller, string szReciver, string szContent, bool bEncryptCaller = false)
        {
            if (m_broker != null)
            {
                m_arrContents = null;
                AddBuffer(szCaller, szReciver, szContent);
                /*string szResult = m_snuMsg.SendSmsMessage(szCaller, szReciver, szContent);
                if (szResult.StartsWith("-"))
                    return false;*/
            }
            return true;
        }

        public bool SendSMS(List<MessageContent> arMessages)
        {
            if (arMessages == null)
                return false;

            BeginSend();

            foreach (MessageContent content in arMessages)
            {
                if (m_broker != null)
                {
                    AddBuffer(content.Caller, content.Reciver, content.Message);
                    //m_broker.SendSmsMessage(content.Caller, content.Reciver, content.Message);
                }
            }

            EndSend();
            return true;
        }

        private void AddBuffer(string strCaller, string strReceiver, string strMessage)
        {
            if (m_isMMS == null)
            {
                if (IsSMSMessage(strMessage))
                    m_isMMS = new DBUtility.VariousData<bool>(false);
                else
                    m_isMMS = new DBUtility.VariousData<bool>(true);
            }

            m_strCaller = strCaller;
            m_strMessage = strMessage;
            m_rcvPhoneNumbers.Add(strReceiver);

            if (m_rcvPhoneNumbers.Count >= m_msgBufCount || m_isBegin == false)
            {
                if (m_isMMS.Data)
                    m_broker.SendMMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strMessage);
                else
                    m_broker.SendSMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strMessage);

                m_rcvPhoneNumbers.Clear();
            }
        }

        private void AddBuffer(string strCaller, string strReceiver, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentsData)
        {
            if (m_isMMS == null || m_isMMS.Data == false)
            {
                m_isMMS = new DBUtility.VariousData<bool>(true);
            }

            m_strCaller = strCaller;
            m_strMessage = strMessage;
            m_rcvPhoneNumbers.Add(strReceiver);
            m_strTitle = strTitle;
            m_arrContents = contentsData;

            if (m_rcvPhoneNumbers.Count >= m_msgBufCount || m_isBegin == false)
            {
                m_broker.SendMMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strMessage, strTitle, m_arrContents);
                m_rcvPhoneNumbers.Clear();
            }
        }

        private bool IsSMSMessage(string strMsg)
        {
            int nByteLength = 0;
            int nLen = strMsg.Length;

            for (int i = 0; i < nLen; i++)
            {
                if (strMsg.ElementAt(i) < 256)
                    nByteLength++;
                else
                    nByteLength += 2;
            }

            if (nByteLength <= m_nSMSLimit)
                return true;

            return false;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            // MMS의 최대길이
            return 4000;
        }

        // 첫번째 메시지를 보낼때 호출
        public void BeginSend()
        {
            m_rcvPhoneNumbers.Clear();
            m_strCaller = "";
            m_strMessage = "";

            m_arrContents = null;
            m_strTitle = "";

            m_isBegin = true;
        }

        // 마지막 메시지를 보낸후 호출
        public void EndSend()
        {
            if (m_broker != null && m_isMMS != null && m_rcvPhoneNumbers.Count > 0)
            {
                if (m_isMMS.Data)
                {
                    if (m_arrContents == null)
                        m_broker.SendMMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strMessage);
                    else
                        m_broker.SendMMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strMessage, m_strTitle, m_arrContents);
                }
                else
                    m_broker.SendSMSMessage(m_strCaller, m_rcvPhoneNumbers, m_strMessage);
            }

            m_isMMS = null;
            m_rcvPhoneNumbers.Clear();
            m_isBegin = false;

            m_strTitle = "";
            m_arrContents = null;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(string szCaller, string szReciver, string szContent, string strTitle = "", MessageContentMMS.ContentType contentType = MessageContentMMS.ContentType.None, string strContentPath = "")
        {
            if (m_broker != null)
            {
                if (m_arrContents == null)
                {
                    m_arrContents = new List<KeyValuePair<MessageContentMMS.ContentType, string>>();
                    m_arrContents.Add(new KeyValuePair<MessageContentMMS.ContentType, string>(contentType, strContentPath));
                }

                AddBuffer(szCaller, szReciver, szContent, strTitle, m_arrContents);
            }
            return true;
        }

        public bool SendMMS(List<MessageContentMMS> arMessages)
        {
            if (arMessages == null)
                return false;

            BeginSend();

            foreach (MessageContentMMS content in arMessages)
            {
                if (m_broker != null)
                {
                    AddBuffer(content.Caller, content.Reciver, content.Message, content.Title, content.ContentsList);
                }
            }

            EndSend();
            return true;
        }
    }
}
