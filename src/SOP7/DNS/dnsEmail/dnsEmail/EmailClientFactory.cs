using System;
using System.Collections.Generic;

namespace dnsEmail
{
    public class EmailClientFactory
    {
        public static IEmailClient CreateMailClient()
        {
#if Soulbrain
            return new EmailClientSoulbrain();
#elif UnEInternal
            return new EmailClientUnEInternal();
#else
            return new MessageClientDummy();
#endif
        }
    }

    public class EmailContent
    {
        private string m_strMsg = "";
        public string Message
        {
            get { return m_strMsg; }
            set { m_strMsg = value; }
        }

        // 수신자 이메일
        //private string m_strEmail = "";
        //public string Email
        //{
        //    get { return m_strEmail; }
        //    set { m_strEmail = value; }
        //}
        private List<string> m_listEmail = new List<string>();
        public List<string> EmailList
        {
            get { return m_listEmail; }
        }

        private string m_strSubject = "";
        public string Subject
        {
            get { return m_strSubject; }
            set { m_strSubject = value; }
        }

        private string m_strEmailTitle = "";
        public string Title
        {
            get { return m_strEmailTitle; }
            set { m_strEmailTitle = value; }
        }

        private string m_szCaller = "";
        public string Caller
        {
            get { return m_szCaller; }
            set { m_szCaller = value; }
        }

        private int m_nSensorReactionHistoryID = -1;
        public int SensorReactionHistoryID
        {
            get { return m_nSensorReactionHistoryID; }
            set { m_nSensorReactionHistoryID = value; }
        }

        private DateTime dtTime;
        public DateTime TimeStamp
        {
            get { return dtTime; }
            set { dtTime = value; }
        }
    }
}
