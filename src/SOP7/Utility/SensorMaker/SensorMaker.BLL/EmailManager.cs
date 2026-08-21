using System;
using System.Net;
using System.Net.Mail;

namespace SensorMaker.BLL
{
    public class EmailManager
    {
        public static bool SendPermitEmail(string strName, string strEmail, string strURL, string strSystemMail, string strSystemCode, string strSolutionName)
        {
            string strSubject = strSolutionName != null && strSolutionName.Length > 0 ? string.Format("[{0}] 계정생성 승인", strSolutionName) : "계정생성 승인";

            string strMessage = string.Format("{0}님의 계정생성이 승인되었습니다.\r\n", strName);
            strMessage += "계정생성시 입력하신 이메일 주소를 사용하여 로그인 하실수 있습니다.\r\n\r\n";
            strMessage += strURL;

            string strEmailTitle = "계정생성 승인안내";
            string strResultMessage = "";

            return SendEmail(strSystemMail, strSystemCode, strEmail, strSubject, strMessage, strEmailTitle, ref strResultMessage);
        }

        public static bool SendDenyEmail(string strName, string strEmail, string strDenyDescription, string strURL, string strSystemMail, string strSystemCode, string strSolutionName)
        {
            string strSubject = strSolutionName != null && strSolutionName.Length > 0 ? string.Format("[{0}] 계정생성 거절", strSolutionName) : "계정생성 거절";

            string strMessage = string.Format("{0}님의 계정생성이 거절되었습니다.\r\n", strName);

            if (strDenyDescription != null && strDenyDescription.Length > 0)
                strMessage += string.Format("사유 : {0}\r\n\r\n", strDenyDescription);

            strMessage += strURL;

            string strEmailTitle = "계정생성 결과안내";
            string strResultMessage = "";

            return SendEmail(strSystemMail, strSystemCode, strEmail, strSubject, strMessage, strEmailTitle, ref strResultMessage);
        }

        private static bool SendEmail(string strSystemMail, string strSystemCode, string strEmail, string strSubject, string strMessage, string strEmailTitle, ref string strResultMessage)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(strSystemMail, strSystemCode);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(strSystemMail),
                    Subject = strSubject,
                    Body = strMessage
                };

                mail.To.Add(new MailAddress(strEmail));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = 587,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = "smtp.gmail.com",
                    EnableSsl = true,
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                strResultMessage = "Error in sending email: " + ex.Message;
                return false;
            }

            if (strEmailTitle != null && strEmailTitle.Length > 0)
                strResultMessage = strEmailTitle + " 메일이 발송되었습니다.\r\n메일을 확인해 주세요.";
            else
                strResultMessage = "메일이 발송되었습니다.\r\n메일을 확인해 주세요.";

            return true;
        }
    }
}
