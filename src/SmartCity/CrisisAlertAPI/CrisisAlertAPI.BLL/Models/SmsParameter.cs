using System;
using System.Collections.Generic;
using System.Text;

namespace CrisisAlertAPI.BLL.Models
{
    public class SmsParameter
    {
        string m_strMessage = "";
        string m_strCaller = "";
        string m_strPhoneNumbers = null;

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public string Caller
        {
            get { return m_strCaller; }
            set { m_strCaller = value; }
        }

        public string PhoneNumbers
        {
            get { return m_strPhoneNumbers; }
            set { m_strPhoneNumbers = value; }
        }
    }
}
