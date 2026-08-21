using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libSMS
{
    public class MessageClientFactory
    {
        public static IMessageClient CreateMessageClient(int nSiteID, string szServerIP)
        {
            if (nSiteID == 3)
            {
                return new MessageClientKDHCLG(szServerIP, nSiteID);
                //return new MessageClientUNE(szServerIP, nSiteID);
                //return new MessageClientMCS(szServerIP, nSiteID);
            }
            else if (nSiteID == 100)
                return new MessageClientSNU(szServerIP, nSiteID);
            else if (nSiteID == 101)
                return new MessageClientMCS(szServerIP, nSiteID);
            else if (nSiteID == 500)
            {
                // test
                //return new MessageClientUNE(szServerIP, nSiteID);
                // Release
                return new MessageClientKPX(szServerIP, nSiteID);
            }
            else if (nSiteID == 1 || nSiteID == 2)
                return new MessageClient(szServerIP, nSiteID);
                //return new MessageClientUNE(szServerIP, nSiteID);
            else
                return new MessageClientDummy(szServerIP, nSiteID);
        }
    }

    public class MessageContent
    {
        private string m_szMsg = "";
        public string Message
        {
            get { return m_szMsg; }
            set { m_szMsg = value; }
        }

        private string m_szCaller = "";
        public string Caller
        {
            get { return m_szCaller; }
            set { m_szCaller = value; }
        }

        private string m_szReciver = "";
        public string Reciver
        {
            get { return m_szReciver; }
            set { m_szReciver = value; }
        }

        private bool m_bEncryptCaller = false;
        public bool EncryptCaller
        {
            get { return m_bEncryptCaller; }
            set { m_bEncryptCaller = value; }
        }

        private string m_szSmsTag = "";
        public string SmsTag
        {
            get { return m_szSmsTag; }
            set { m_szSmsTag = value; }
        }

    }

    public class MessageContentMMS : MessageContent
    {
        public enum ContentType { None = 0, Image, Audio, Video };

        private string m_strTitle = "";
        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }
        
        // Value : 외부 컨텐츠의 파일 경로
        private List<KeyValuePair<ContentType, string>> m_contentsList = new List<KeyValuePair<ContentType, string>>();
        public List<KeyValuePair<ContentType, string>> ContentsList
        {
            get { return m_contentsList; }
        }
    }
}
