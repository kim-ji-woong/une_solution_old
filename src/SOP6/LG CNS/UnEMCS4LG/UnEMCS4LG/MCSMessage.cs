using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnEMCS4LG
{
    public class MCSMessage
    {
        private int m_nID = -1;
        private string m_strPhoneNumbers = "";
        private string m_strMessage = "";
        private string m_strImage = null;
        private DateTime m_timeStamp;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string PhoneNumbers
        {
            get { return m_strPhoneNumbers; }
            set { m_strPhoneNumbers = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public string Image
        {
            get { return m_strImage; }
            set { m_strImage = value; }
        }

        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }
    }
}
