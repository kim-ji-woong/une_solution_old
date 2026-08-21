using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libSMS
{
    internal static class ClientHelper
    {
        public static bool IsSMSMessage(string strMsg, int nSMSLimit)
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

            if (nByteLength <= nSMSLimit)
                return true;

            return false;
        }

        public static void CheckTitle(ref string strTitle, string strMessage)
        {
            if (strTitle.Length > 0)
                return;

            if (strMessage.Length <= 5)
                strTitle = strMessage;
            else
                strTitle = strMessage.Substring(0, 5);
        }
    }

    internal class MessageClientDummy : IMessageClient
    {
        internal MessageClientDummy()
        {
        }

        public void Dispose()
        {

        }

        public bool SendSMS(MessageContent message)
        {
            return true;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            return 140;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return false;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(MessageContentMMS message)
        {
            return false;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            return false;
        }

        public string GetErrorMessage()
        {
            return "";
        }
    }

#if KDN
    internal class MessageClient : IMessageClient
    {
        private MessageBroker m_broker = null;

        internal MessageClient()
        {
            m_broker = new MessageBroker();
        }

        public void Dispose()
        {
        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                return m_broker.SendMessage(message);
            }

            return false;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent content in messages)
            {
                if (m_broker != null)
                {
                    if (!m_broker.SendMessage(content))
                        return false;
                }
            }
            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            return 80;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return false;
        }

        public bool SendMMS(MessageContentMMS message)
        {
            return false;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            return false;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif

#if SNU
    // 서울대
    // 서울대 문자메시지 서비스는 다른 서비스와 달리 발신자 번호를 아무거나 입력할 수 있다.
    internal class MessageClientSNU : IMessageClient
    {
        private int m_msgBufCount = 100;

        private int m_nSMSLimit = 90;
        private MessageBrokerSNU m_broker = null;

        public MessageClientSNU(int nSiteID)
        {
            m_broker = new MessageBrokerSNU(nSiteID);
        }

        public void Dispose()
        {

        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                bool isSMS = ClientHelper.IsSMSMessage(message.Message, m_nSMSLimit);

                int nReceiverCount = message.PhoneNumbers.Count;

                for (int i = 0; i < nReceiverCount;)
                {
                    int nEndIndex = i + m_msgBufCount;

                    if (nEndIndex >= nReceiverCount)
                        nEndIndex = nReceiverCount;

                    if (isSMS)
                    {
                        if (m_broker.SendSMSMessage(message.Caller, message.PhoneNumbers, message.Message, i, nEndIndex) == false)
                            return false;
                    }
                    else
                    {
                        if (m_broker.SendLMSMessage(message.Caller, message.PhoneNumbers, message.Message, i, nEndIndex) == false)
                            return false;
                    }

                    i = nEndIndex;
                }

                return true;
            }

            return false;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent message in messages)
            {
                if (SendSMS(message) == false)
                    return false;
            }

            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            // MMS의 최대길이
            return 4000;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        public bool SendMMS(MessageContentMMS message)
        {
            if (m_broker != null && message != null)
            {
                int nReceiverCount = message.PhoneNumbers.Count;

                for (int i = 0; i < nReceiverCount;)
                {
                    int nEndIndex = i + m_msgBufCount;

                    if (nEndIndex >= nReceiverCount)
                        nEndIndex = nReceiverCount;

                    if (message.ContentsList.Count > 0)
                    {
                        if (m_broker.SendMMSMessage(message.Caller, message.PhoneNumbers, message.Message, message.Title, message.ContentsList, i, nEndIndex) == false)
                            return false;
                    }
                    else
                    {
                        if (m_broker.SendLMSMessage(message.Caller, message.PhoneNumbers, message.Message, i, nEndIndex) == false)
                            return false;
                    }

                    i = nEndIndex;
                }

                return true;
            }

            return false;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContentMMS message in messages)
            {
                if (SendMMS(message) == false)
                    return false;
            }

            return true;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif

#if UNE_EZ_SMS
    // ezSMS를 사용하는 버전
    internal class MessageClientUNE : IMessageClient
    {
        private int m_nSMSLimit = 90;
        private MessageBrokerUNE m_broker = null;

        public MessageClientUNE()
        {
            m_broker = new MessageBrokerUNE();
        }

        public void Dispose()
        {
        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker == null || message == null)
                return false;

            bool isSMS = ClientHelper.IsSMSMessage(message.Message, m_nSMSLimit);

            if (isSMS)
                return m_broker.SendSMS(message.PhoneNumbers, message.Message);

            string strTitle = "";
            ClientHelper.CheckTitle(ref strTitle, message.Message);

            return m_broker.SendLMS(message.PhoneNumbers, message.Message, strTitle);
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent message in messages)
            {
                if (SendSMS(message) == false)
                    return false;
            }

            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            return 4000;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        public bool SendMMS(MessageContentMMS message)
        {
            if (m_broker != null && message != null)
            {
                string strTitle = message.Title;
                ClientHelper.CheckTitle(ref strTitle, message.Message);

                return m_broker.SendLMS(message.PhoneNumbers, message.Message, strTitle);
            }

            return false;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            if (m_broker != null && messages != null)
            {
                foreach (MessageContentMMS message in messages)
                {
                    if (SendMMS(message) == false)
                        return false;
                }

                return true;
            }

            return false;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif

#if KPX
    internal class MessageClientKPX : IMessageClient
    {
        private int m_nSMSLimit = 90;
        private MessageBrokerKPX m_broker = null;

        public MessageClientKPX()
        {
            m_broker = new MessageBrokerKPX();
        }

        public void Dispose()
        {
        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                bool isSMS = ClientHelper.IsSMSMessage(message.Message, m_nSMSLimit);

                if (isSMS)
                    return m_broker.SendSMS(message.PhoneNumbers, message.Message);
                else
                {
                    string strTitle = "";
                    ClientHelper.CheckTitle(ref strTitle, message.Message);

                    return m_broker.SendLMS(message.PhoneNumbers, message.Message, strTitle);
                }
            }

            return false;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent message in messages)
            {
                if (SendSMS(message) == false)
                    return false;
            }

            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            return 4000;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        public bool SendMMS(MessageContentMMS message)
        {
            if (m_broker != null && message != null)
            {   
                string strTitle = message.Title;
                ClientHelper.CheckTitle(ref strTitle, message.Message);

                return m_broker.SendLMS(message.PhoneNumbers, message.Message, strTitle);
            }

            return false;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            if (m_broker != null && messages != null)
            {
                foreach (MessageContentMMS message in messages)
                {
                    if (SendMMS(message) == false)
                        return false;
                }

                return true;
            }

            return false;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif

#if KDHCLG
    // 지역난방공사 (LG)
    internal class MessageClientKDHCLG : IMessageClient
    {
        private int m_msgBufCount = 10;
        
        private string m_strCaller = "15225267"; // 발신번호 고정
        private int m_nSMSLimit = 90;
        private MessageBrokerKDHCLG m_broker = null;
        
        public MessageClientKDHCLG(int nSiteID)
        {
            m_broker = new MessageBrokerKDHCLG(nSiteID);
        }

        public void Dispose()
        {

        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                bool isSMS = ClientHelper.IsSMSMessage(message.Message, m_nSMSLimit);

                if (isSMS)
                    return m_broker.SendSMSMessage(m_strCaller, message.PhoneNumbers, message.Message);
                else
                {
                    string strTitle = "";
                    ClientHelper.CheckTitle(ref strTitle, message.Message);

                    return m_broker.SendLMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message);
                }
            }

            return false;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent message in messages)
            {
                if (SendSMS(message) == false)
                    return false;
            }

            return true;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(MessageContentMMS message)
        {
            if (m_broker != null && message != null)
            {
                string strTitle = message.Title;
                ClientHelper.CheckTitle(ref strTitle, message.Message);

                if (message.ContentsList.Count == 0)
                    return m_broker.SendLMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message);
                else
                    return m_broker.SendMMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message, message.ContentsList);
            }

            return false;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContentMMS message in messages)
            {
                if (SendMMS(message) == false)
                    return false;
            }

            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            // MMS의 최대길이
            return 4000;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif

#if UNE_MCS
    // 모노 커뮤니케이션즈(KT 크로샷)
    internal class MessageClientMCS : IMessageClient
    {
        private int m_msgBufCount = 100;

        private int m_nSMSLimit = 90;
        private MessageBrokerMCS m_broker = null;

        public MessageClientMCS()
        {
            m_broker = new MessageBrokerMCS();
        }

        public void Dispose()
        {

        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                bool isSMS = ClientHelper.IsSMSMessage(message.Message, m_nSMSLimit);

                int nReceiverCount = message.PhoneNumbers.Count;

                for (int i = 0; i < nReceiverCount;)
                {
                    int nEndIndex = i + m_msgBufCount;

                    if (nEndIndex >= nReceiverCount)
                        nEndIndex = nReceiverCount;

                    if (isSMS)
                    {
                        if (m_broker.SendSMSMessage(message.PhoneNumbers, message.Message, i, nEndIndex) == false)
                            return false;
                    }
                    else
                    {
                        string strTitle = "";
                        ClientHelper.CheckTitle(ref strTitle, message.Message);

                        if (m_broker.SendLMSMessage(message.PhoneNumbers, message.Message, strTitle, i, nEndIndex) == false)
                            return false;
                    }

                    i = nEndIndex;
                }

                return true;
            }

            return false;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent message in messages)
            {
                if (SendSMS(message) == false)
                    return false;
            }

            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            // MMS의 최대길이
            return 4000;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(MessageContentMMS message)
        {
            if (m_broker == null || message == null)
                return false;

            int nReceiverCount = message.PhoneNumbers.Count;

            for (int i = 0; i < nReceiverCount;)
            {
                int nEndIndex = i + m_msgBufCount;

                if (nEndIndex >= nReceiverCount)
                    nEndIndex = nReceiverCount;

                string strTitle = message.Title;
                ClientHelper.CheckTitle(ref strTitle, message.Message);

                if (message.ContentsList.Count == 0)
                {
                    if (m_broker.SendLMSMessage(message.PhoneNumbers, message.Message, strTitle, i, nEndIndex) == false)
                        return false;
                }
                else
                {
                    if (m_broker.SendMMSMessage(message.PhoneNumbers, message.Message, strTitle, message.ContentsList, i, nEndIndex) == false)
                        return false;
                }

                i = nEndIndex;
            }

            return true;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContentMMS message in messages)
            {
                if (SendMMS(message) == false)
                    return false;
            }

            return true;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif

#if BLD_200
    // 신한은행 UMS
    internal class MessageClientBLD200 : IMessageClient
    {
        private int m_msgBufCount = 10;

        private string m_strCaller = "1"; // RCV_FRID : 고객번호 (임의로 해도됨)
        private int m_nSMSLimit = 90;
        private MessageBrokerBLD200 m_broker = null;

        public MessageClientBLD200(int nSiteID)
        {
            m_broker = new MessageBrokerBLD200(nSiteID);
        }

        public void Dispose()
        {

        }
         
        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                bool isSMS = ClientHelper.IsSMSMessage(message.Message, m_nSMSLimit);
                if (isSMS)
                {
                    return m_broker.SendSMSMessage(m_strCaller, message.PhoneNumbers, message.Message);
                }
                else
                {
                    string strTitle = "";
                    ClientHelper.CheckTitle(ref strTitle, message.Message);

                    return m_broker.SendLMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message);
                }
            }
            return false;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent message in messages)
            {
                if (SendSMS(message) == false)
                    return false;
            }

            return true;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(MessageContentMMS message)
        {
            //if (m_broker != null && message != null)
            //{
            //    string strTitle = message.Title;
            //    ClientHelper.CheckTitle(ref strTitle, message.Message);

            //    if (message.ContentsList.Count == 0)
            //        return m_broker.SendLMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message);
            //    else
            //        return m_broker.SendMMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message, message.ContentsList);
            //}

            return false;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            //if (messages == null || m_broker == null)
            //    return false;

            //foreach (MessageContentMMS message in messages)
            //{
            //    if (SendMMS(message) == false)
            //        return false;
            //}

            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            // MMS의 최대길이
            return 4000;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif

#if Parc1
    internal class MessageClientParc1 : IMessageClient
    {
        private int m_msgBufCount = 10;
        
        private string m_strCaller = "01227039675"; // 발신번호 고정
        private int m_nSMSLimit = 80;
        private MessageBrokerParc1 m_broker = null;
        
        public MessageClientParc1(int nSiteID)
        {
            m_broker = new MessageBrokerParc1(nSiteID);
        }

        public void Dispose()
        {

        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                //bool isSMS = ClientHelper.IsSMSMessage(message.Message, m_nSMSLimit);

                //if (isSMS)
                    return m_broker.SendSMSMessage(m_strCaller, message.PhoneNumbers, message.Message, m_nSMSLimit);
                //else
                //{
                //    string strTitle = "";
                //    ClientHelper.CheckTitle(ref strTitle, message.Message);

                //    return m_broker.SendLMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message);
                //}
            }

            return false;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent message in messages)
            {
                if (SendSMS(message) == false)
                    return false;
            }

            return true;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(MessageContentMMS message)
        {
            if (m_broker != null && message != null)
            {
                string strTitle = message.Title;
                ClientHelper.CheckTitle(ref strTitle, message.Message);

                if (message.ContentsList.Count == 0)
                    return m_broker.SendLMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message);
                else
                    return m_broker.SendMMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message, message.ContentsList);
            }

            return false;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContentMMS message in messages)
            {
                if (SendMMS(message) == false)
                    return false;
            }

            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            // MMS의 최대길이
            return 4000;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif

#if Urbanbrix
    internal class MessageClientUrbanbrix : IMessageClient
    {
        private int m_msgBufCount = 10;

        private string m_strCaller = "01082463766"; // 발신번호 고정 (이승구 방재팀장)
        private int m_nSMSLimit = 90;
        private MessageBrokerUrbanbrix m_broker = null;
        public MessageClientUrbanbrix(int nSiteID)
        {
            m_broker = new MessageBrokerUrbanbrix(nSiteID);
        }

        public void Dispose()
        {

        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                bool isSMS = ClientHelper.IsSMSMessage(message.Message, m_nSMSLimit);
                if (isSMS)
                    return m_broker.SendSMSMessage(m_strCaller, message.PhoneNumbers, message.Message);
                else
                {
                    string strTitle = "";
                    ClientHelper.CheckTitle(ref strTitle, message.Message);

                    return m_broker.SendLMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message);
                }
            }

            return false;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent message in messages)
            {
                if (SendSMS(message) == false)
                    return false;
            }

            return true;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(MessageContentMMS message)
        {
            if (m_broker != null && message != null)
            {
                string strTitle = message.Title;
                ClientHelper.CheckTitle(ref strTitle, message.Message);

                if (message.ContentsList.Count == 0)
                    return m_broker.SendLMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message);
                else
                    return m_broker.SendMMSMessage(m_strCaller, message.PhoneNumbers, strTitle, message.Message, message.ContentsList);
            }

            return false;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContentMMS message in messages)
            {
                if (SendMMS(message) == false)
                    return false;
            }

            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            // MMS의 최대길이
            return 4000;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif

#if SKT_MCS
    // 모노 커뮤니케이션즈(KT 크로샷)
    internal class MessageClientSKT_MCS : IMessageClient
    {
        private int m_msgBufCount = 100;

        private int m_nSMSLimit = 90;
        private MessageBrokerSKT_MCS m_broker = null;

        public MessageClientSKT_MCS()
        {
            m_broker = new MessageBrokerSKT_MCS();
        }

        public void Dispose()
        {

        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                bool isSMS = ClientHelper.IsSMSMessage(message.Message, m_nSMSLimit);

                int nReceiverCount = message.PhoneNumbers.Count;

                for (int i = 0; i < nReceiverCount;)
                {
                    int nEndIndex = i + m_msgBufCount;

                    if (nEndIndex >= nReceiverCount)
                        nEndIndex = nReceiverCount;

                    if (isSMS)
                    {
                        if (m_broker.SendSMSMessage(message.PhoneNumbers, message.Message, i, nEndIndex) == false)
                            return false;
                    }
                    else
                    {
                        string strTitle = "";
                        ClientHelper.CheckTitle(ref strTitle, message.Message);

                        if (m_broker.SendLMSMessage(message.PhoneNumbers, message.Message, strTitle, i, nEndIndex) == false)
                            return false;
                    }

                    i = nEndIndex;
                }

                return true;
            }

            return false;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent message in messages)
            {
                if (SendSMS(message) == false)
                    return false;
            }

            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            // MMS의 최대길이
            return 4000;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(MessageContentMMS message)
        {
            if (m_broker == null || message == null)
                return false;

            int nReceiverCount = message.PhoneNumbers.Count;

            for (int i = 0; i < nReceiverCount;)
            {
                int nEndIndex = i + m_msgBufCount;

                if (nEndIndex >= nReceiverCount)
                    nEndIndex = nReceiverCount;

                string strTitle = message.Title;
                ClientHelper.CheckTitle(ref strTitle, message.Message);

                if (message.ContentsList.Count == 0)
                {
                    if (m_broker.SendLMSMessage(message.PhoneNumbers, message.Message, strTitle, i, nEndIndex) == false)
                        return false;
                }
                else
                {
                    if (m_broker.SendMMSMessage(message.PhoneNumbers, message.Message, strTitle, message.ContentsList, i, nEndIndex) == false)
                        return false;
                }

                i = nEndIndex;
            }

            return true;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContentMMS message in messages)
            {
                if (SendMMS(message) == false)
                    return false;
            }

            return true;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif

#if Kakao
    internal class MessageClientKakao : IMessageClient
    {
        private int m_msgBufCount = 100;

        private MessageBrokerKakao m_broker = null;

        public MessageClientKakao(int nSiteID)
        {
            m_broker = new MessageBrokerKakao(nSiteID);
        }

        public void Dispose()
        {

        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                if (m_broker.SendSMSMessage(message.PhoneNumbers, message.SensorReactionHistoryID) == false)
                    return false;

                return true;
            }

            return false;
        }

        public bool SendSMS(List<MessageContent> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContent message in messages)
            {
                if (SendSMS(message) == false)
                    return false;
            }

            return true;
        }

        // 메시지의 길이제한 바이트 수
        public int GetMessageLength()
        {
            // MMS의 최대길이
            return 4000;
        }

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        public bool CanUseMMS()
        {
            return true;
        }

        // strContentPath : 외부 컨텐츠 파일의 경로
        public bool SendMMS(MessageContentMMS message)
        {
            if (m_broker == null || message == null)
                return false;

            int nReceiverCount = message.PhoneNumbers.Count;

            for (int i = 0; i < nReceiverCount;)
            {
                int nEndIndex = i + m_msgBufCount;

                if (nEndIndex >= nReceiverCount)
                    nEndIndex = nReceiverCount;

                string strTitle = message.Title;
                ClientHelper.CheckTitle(ref strTitle, message.Message);

                if (message.ContentsList.Count == 0)
                {
                    if (m_broker.SendLMSMessage(message.PhoneNumbers, message.Message, strTitle, i, nEndIndex) == false)
                        return false;
                }
                else
                {
                    if (m_broker.SendMMSMessage(message.PhoneNumbers, message.Message, strTitle, message.ContentsList, i, nEndIndex) == false)
                        return false;
                }

                i = nEndIndex;
            }

            return true;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
        {
            if (messages == null || m_broker == null)
                return false;

            foreach (MessageContentMMS message in messages)
            {
                if (SendMMS(message) == false)
                    return false;
            }

            return true;
        }

        public string GetErrorMessage()
        {
            if (m_broker == null)
                return "";

            return m_broker.ErrorMessage;
        }
    }
#endif
}
