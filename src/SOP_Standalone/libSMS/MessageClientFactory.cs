using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libSMS
{
    public class MessageClientFactory
    {
        private enum ServiceType
        {
            NotUse = 0,
            ezSMS,
            KT_DB,
            LGU_DB,
            UnE_ezSMS = 100
        }

        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public static IMessageClient CreateMessageClient(int nSiteID, string szServerIP)
        {
            IMessageClient client = new MessageClientDummy(szServerIP, nSiteID);

            string strPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string strFullPath = System.IO.Directory.GetParent(strPath).FullName;
            strFullPath += "\\sms.ini";

            if (System.IO.File.Exists(strFullPath) == false)
                return client;

            DBUtility.Utility util = new DBUtility.Utility();
            string strServiceType = util.getinivalue("SMS_Info", "ServiceType", strFullPath);

            int nServiceType = 0;

            if (int.TryParse(strServiceType, out nServiceType) == false)
                return client;

            string strCallerPhoneNumber = util.getinivalue("SMS_Info", "CallerPhoneNumber", strFullPath);

            ServiceType type = ToServiceType(nServiceType);

            if (type == ServiceType.NotUse)
            {
                return client;
            }
            else if (type == ServiceType.ezSMS)
            {
                IMessageClient client2 = ReadWebService(util, strCallerPhoneNumber, strFullPath);

                if (client2 != null)
                    client = client2;
            }
            else if (type == ServiceType.KT_DB || type == ServiceType.LGU_DB)
            {
                IMessageClient client2 = ReadDB(util, type, strCallerPhoneNumber, strFullPath);

                if (client2 != null)
                    client = client2;
            }
            else if (type == ServiceType.UnE_ezSMS)
            {
                client = new MessageClientUNE(szServerIP, nSiteID);
            }

            return client;
        }

        private static IMessageClient ReadDB(DBUtility.Utility util, ServiceType type, string strCallerPhoneNumber, string strFullPath)
        {
            string strDBName = util.getinivalue("DB_Info", "DBName", strFullPath);

            if (strDBName.Length == 0)
                return null;

            string strDBType = util.getinivalue("DB_Info", "DBType", strFullPath);

            if (strDBType.Length == 0)
                return null;

            string strServerIP = util.getinivalue("DB_Info", "ServerIP", strFullPath);

            if (strServerIP.Length == 0)
                return null;

            int nDBType = 0;
            DBUtility.WebDBManager.DBType dbType = DBUtility.WebDBManager.DBType.TypeCount;

            if (int.TryParse(strDBType, out nDBType) == false)
                return null;

            if (nDBType == (int)DBUtility.WebDBManager.DBType.sqlserver)
                dbType = DBUtility.WebDBManager.DBType.sqlserver;
            else if (nDBType != (int)DBUtility.WebDBManager.DBType.mysql)
                dbType = DBUtility.WebDBManager.DBType.mysql;
            else
                return null;

            string strDBConnection = util.getinivalue("DB_Info", "DBConnection", strFullPath);

            string strDec = DBUtility.AES256Cipher.AES_decrypt(strDBConnection, key);

            int nIndex = strDec.IndexOf('|');

            if (nIndex < 0)
                return null;

            string strUserID = util.getinivalue("DB_Info", "UserID", strFullPath);

            string strID = strDec.Substring(0, nIndex);
            string strPassword = strDec.Substring(nIndex + 1);

            if (type == ServiceType.KT_DB)
                return new MessageClientMCS(strServerIP, dbType, strDBName, strID, strPassword, strUserID, strCallerPhoneNumber);
            else if (type == ServiceType.LGU_DB)
                return new MessageClientLGU(strServerIP, dbType, strDBName, strID, strPassword, strUserID, strCallerPhoneNumber);

            return null;
        }

        private static IMessageClient ReadWebService(DBUtility.Utility util, string strCallerPhoneNumber, string strFullPath)
        {
            string strConnection = util.getinivalue("WebService", "Connection", strFullPath);

            if (strConnection.Length == 0)
                return null;

            string strDec = DBUtility.AES256Cipher.AES_decrypt(strConnection, key);

            int nIndex = strDec.IndexOf('|');

            if (nIndex < 0)
                return null;

            string strUserID = strDec.Substring(0, nIndex);
            string strPassword = strDec.Substring(nIndex + 1);
            return new MessageClientEzSMS(strUserID, strPassword, strCallerPhoneNumber);
        }

        private static ServiceType ToServiceType(int nServiceType)
        {
            foreach (ServiceType type in Enum.GetValues(typeof(ServiceType)))
            {
                if (nServiceType == (int)type)
                    return type;
            }

            return ServiceType.NotUse;
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
