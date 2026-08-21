using System;
using System.Collections.Generic;

namespace dnsEmail
{
    internal class MessageClientDummy : IEmailClient
    {
        public void Dispose()
        {
            
        }

        //bool IEmailClient.SendEmail(string strEmail, string strSubject, string strMessage, string strTitle, ref string strResultMsg)
        //{
        //    return true;
        //}

        bool IEmailClient.SendEmail(EmailContent message, ref string strResultMsg)
        {
            return true;
        }

        //bool IEmailClient.SendEmail(List<EmailContent> messages, ref string strResultMsg)
        //{
        //    return true;
        //}
    }

#if Soulbrain
    internal class EmailClientSoulbrain : IEmailClient
    {
        private EmailBrokerSoulbrain m_broker = null;

        public EmailClientSoulbrain()
        {
            m_broker = new EmailBrokerSoulbrain();
        }

        public void Dispose()
        {

        }

        //public bool SendEmail(string strEmail, string strSubject, string strMessage, string strTitle, ref string strResultMsg)
        //{
        //    if (m_broker != null)
        //    {
        //        if (m_broker.SendEmail(strEmail, strSubject, strMessage, strTitle, ref strResultMsg) == false)
        //            return false;

        //        return true;
        //    }

        //    return false;
        //}

        public bool SendEmail(EmailContent message, ref string strResultMsg)
        {
            if (m_broker != null && message != null && message.EmailList.Count > 0)
            {
                foreach (string strEmail in message.EmailList)
                {
                    if (m_broker.SendEmail(strEmail, message.Subject, message.Message, message.Title, message.TimeStamp, ref strResultMsg) == false)
                        return false;
                }
                
                return true;
            }

            return false;
        }

        //public bool SendEmail(List<EmailContent> messages, ref string strResultMsg)
        //{
        //    if (messages == null || m_broker == null)
        //        return false;

        //    foreach (EmailContent message in messages)
        //    {
        //        if (SendEmail(message, ref strResultMsg) == false)
        //            return false;
        //    }

        //    return true;
        //}
    }
#endif

#if UnEInternal
    internal class EmailClientUnEInternal : IEmailClient
    {
        private EmailBrokerUnEInternal m_broker = null;

        public EmailClientUnEInternal()
        {
            m_broker = new EmailBrokerUnEInternal();
        }

        void IDisposable.Dispose()
        {
            
        }

        //public bool SendEmail(string strEmail, string strSubject, string strMessage, string strTitle, ref string strResultMsg)
        //{
        //    if (m_broker != null)
        //    {
        //        if (m_broker.SendEmail(strEmail, strSubject, strMessage, strTitle, ref strResultMsg) == false)
        //            return false;

        //        return true;
        //    }

        //    return false;
        //}

        //public bool SendEmail(EmailContent message, ref string strResultMsg)
        //{
        //    if (m_broker != null && message != null && message.EmailList.Count > 0)
        //    {
        //        foreach (string strEmail in message.EmailList)
        //        {
        //            if (m_broker.SendEmail(strEmail, message.Subject, message.Message, message.Title, ref strResultMsg) == false)
        //                return false;
        //        }

        //        return true;
        //    }

        //    return false;
        //}

        public bool SendEmail(EmailContent message, ref string strResultMsg)
        {
            if (m_broker != null && message != null && message.EmailList.Count > 0)
            {
                foreach (string strEmail in message.EmailList)
                {
                    if (m_broker.SendEmail(strEmail, message.Subject, message.Message, message.Title, message.TimeStamp, ref strResultMsg) == false)
                        return false;
                }

                return true;
            }

            return false;
        }

        //public bool SendEmail(List<EmailContent> messages, ref string strResultMsg)
        //{
        //    if (messages == null || m_broker == null)
        //        return false;

        //    foreach (EmailContent message in messages)
        //    {
        //        if (SendEmail(message, ref strResultMsg) == false)
        //            return false;
        //    }

        //    return true;
        //}
    }

#endif
}
