using DBUtility2;
using System.Collections;

namespace MuxFireSensorServer.Data
{
    public class DirectDBManagerEx
    {
        private DirectDBManager m_dbMgr = null;

        public int SiteID
        {
            get { return m_dbMgr.SiteID; }
        }

        public string ErrorMessage
        {
            get { return m_dbMgr.ErrorMessage; }
        }

        public DirectDBManagerEx(DirectDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
        }

        public ArrayList GetResultData(string strSQL)
        {
            if (m_dbMgr.Connect() == false)
                return null;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            m_dbMgr.Close();
            return arrResult;
        }

        public static bool GetDBInfo(out string strID, out string strPW)
        {
            string strDBInfo = System.Configuration.ConfigurationManager.AppSettings.Get("dbInfo");

            if (strDBInfo != null && strDBInfo.Length > 0)
            {
                string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
                string strValue = AES256Cipher.AES_decrypt(strDBInfo, key);

                int index = strValue.IndexOf('|');

                if (index > 0)
                {
                    strID = strValue.Substring(0, index).Trim();
                    strPW = strValue.Substring(index + 1).Trim();
                    return true;
                }
            }

            strID = strPW = null;
            return false;
        }
    }
}
