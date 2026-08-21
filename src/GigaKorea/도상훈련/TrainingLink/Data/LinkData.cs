using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainingLink.Data
{
    public class CommonString
    {
        public const string Client_ID = "client";
        public const string Client_SiteID = "300";

        public const string Client1_ID = "client1";
        public const string Client1_SiteID = "301";

        public const string Client2_ID = "client2";
        public const string Client2_SiteID = "302";
    }

    public class Client
    {
        string m_strID = "";
        int m_nSiteID = -1;

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }
    }

    public class MessageData
    {
        int m_nID = -1;
        string m_strSender = "";
        string m_strReceiver = "";
        string m_strMessage = "";
        DateTime m_dtCreateTime;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Sender
        {
            get { return m_strSender; }
            set { m_strSender = value; }
        }

        public string Receiver
        {
            get { return m_strReceiver; }
            set { m_strReceiver = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }

        public DateTime CreateTime
        {
            get { return m_dtCreateTime; }
            set { m_dtCreateTime = value; }
        }
    }
}
