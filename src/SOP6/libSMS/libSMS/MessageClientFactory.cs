using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libSMS
{
    public class MessageClientFactory
    {
        public static IMessageClient CreateMessageClient(int nSiteID)
        {
#if KDN
            // 한전 KDN의 문자 서비스
            return new MessageClient();
#elif KDHCLG
            return new MessageClientKDHCLG(nSiteID);
#elif SNU
            return new MessageClientSNU(nSiteID);
#elif KPX
            return new MessageClientKPX();
#elif UNE_MCS
            return new MessageClientMCS();
#elif UNE_EZ_SMS
            return new MessageClientUNE();
#elif BLD_200
            return new MessageClientBLD200(nSiteID);
#elif Parc1
            return new MessageClientParc1(nSiteID);
#elif Urbanbrix
            return new MessageClientUrbanbrix(nSiteID);
#elif SKT_MCS
            return new MessageClientSKT_MCS();
#elif Kakao
            return new MessageClientKakao(nSiteID);
#else
            return new MessageClientDummy();
#endif
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

        // 수신자 리스트
        private List<string> m_phoneNumbnerList = new List<string>();
        public List<string> PhoneNumbers
        {
            get { return m_phoneNumbnerList; }
        }

        private object m_tag = null;
        public object Tag
        {
            get { return m_tag; }
            set { m_tag = value; }
        }

        private int m_nSensorReactionHistoryID = -1;
        public int SensorReactionHistoryID
        {
            get { return m_nSensorReactionHistoryID; }
            set { m_nSensorReactionHistoryID = value; }
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
