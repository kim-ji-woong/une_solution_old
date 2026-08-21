using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnsSMS
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

#if Soulbrain_MCS
    // 모노 커뮤니케이션즈(KT 크로샷)
    internal class MessageClientSoulbrainMCS : IMessageClient
    {
        private int m_msgBufCount = 100;

        private int m_nSMSLimit = 90;
        private MessageBrokerSoulbrainMCS m_broker = null;

        public MessageClientSoulbrainMCS()
        {
            m_broker = new MessageBrokerSoulbrainMCS();
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

        public MessageClientKakao(Common.IDAL.IDataManager commonDataManager, SDMS.IDAL.IDataManager sdmsDataManager)
        {
            m_broker = new MessageBrokerKakao(commonDataManager, sdmsDataManager);
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

#if Kakaowork
    internal class MessageClientKakaowork : IMessageClient
    {
        private int m_msgBufCount = 100;

        private MessageBrokerKakaowork m_broker = null;

        public MessageClientKakaowork(Common.IDAL.IDataManager commonDataManager, SDMS.IDAL.IDataManager sdmsDataManager)
        {
            m_broker = new MessageBrokerKakaowork(commonDataManager, sdmsDataManager);
        }

        public void Dispose()
        {

        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                if (m_broker.SendSMSMessage(message.EMails, message.Message) == false)
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

#if External_UNE_MCS
    // 외부에서 UNE_MCS 사용하기 위한 버전 >> UNE 서버에 API를 사용하는 방식
    internal class MessageClientExternalMCS : IMessageClient
    {
        private int m_msgBufCount = 100;
        private int m_nSMSLimit = 90;
        private const string m_strCaller = "027144133";

        private MessageBrokerExternal_MCS m_broker = null;

        public MessageClientExternalMCS()
        {
            m_broker = new MessageBrokerExternal_MCS();
        }

        public void Dispose()
        {

        }

        public bool SendSMS(MessageContent message)
        {
            if (m_broker != null && message != null)
            {
                string strPhoneNumbers = "";
                string strMessage = message.Message;

                foreach (string phoneNumber in message.PhoneNumbers)
                {
                    if (strPhoneNumbers == "")
                        strPhoneNumbers = phoneNumber;
                    else
                        strPhoneNumbers += "," + phoneNumber;
                }

                strMessage = strMessage.Replace("\n", "\\n");

                // API를 이용하여 SMS를 보내는 방식
                Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
                string strJson = "{\"message\": \"" + strMessage + "\", \"caller\":\"" + m_strCaller + "\", \"phoneNumbers\":\"" + strPhoneNumbers + "\"}";
                string strURL = "/api/SMS";
                string strErrorMessage = "";

                bool bResult = m_broker.SendQuery(dicHeaders, strJson, strURL, out strErrorMessage, "POST");

                return bResult;
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
            return true;
        }

        public bool SendMMS(List<MessageContentMMS> messages)
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
}
