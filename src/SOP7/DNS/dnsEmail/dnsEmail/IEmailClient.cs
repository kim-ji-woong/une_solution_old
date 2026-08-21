using System;
using System.Collections.Generic;

namespace dnsEmail
{
    public interface IEmailClient : IDisposable
    {
        //bool SendEmail(string strEmail, string strSubject, string strMessage, string strTitle, ref string strResultMsg);
        bool SendEmail(EmailContent message, ref string strResultMsg);
        //bool SendEmail(List<EmailContent> messages, ref string strResultMsg);
    }
}
