using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Security;
using System.Security.AccessControl;

namespace SiteRegister
{
    public class Site
    {
        public enum DBType { sqlserver = 0, mysql, TypeCount };

        private int m_nSiteID;
        private string m_strDBName;
        private DBType m_dbType;
        private Encoding m_encoding = Encoding.UTF8;
        private string m_strWebServerURL = "http://127.0.0.1";

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public string DatabaseName
        {
            get { return m_strDBName; }
            set { m_strDBName = value; }
        }

        public DBType DatabaseType
        {
            get { return m_dbType; }
            set { m_dbType = value; }
        }

        public Encoding Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }
        }

        public string WebServerURL
        {
            get { return m_strWebServerURL; }
            set { m_strWebServerURL = value; }
        }

        public Site()
        {
        }

        public Site(int nSiteID, string strDBName, DBType dbType)
        {
            m_nSiteID = nSiteID;
            m_strDBName = strDBName;
            m_dbType = dbType;
        }

        public Site(int nSiteID, string strDBName, DBType dbType, Encoding encoding, string strWebServerURL)
        {
            m_nSiteID = nSiteID;
            m_strDBName = strDBName;
            m_dbType = dbType;
            m_encoding = encoding;
            m_strWebServerURL = strWebServerURL;
        }

        public bool Write()
        {
            string strURL = RegUtil.BaseURL + m_nSiteID.ToString();

            if (RegUtil.IsExist(strURL) == false)
            {
                if (RegUtil.CreateKey(strURL) == false)
                    return false;
            }

            string strConnectionInfo = "Server Connection Info";
            string strURL2 = strURL + "\\" + strConnectionInfo;

            if (RegUtil.IsExist(strURL2) == false)
            {
                if (RegUtil.CreateKey(strURL2) == false)
                    return false;
            }

            if (RegUtil.WriteRegValue(strConnectionInfo, "db_name", m_strDBName, m_nSiteID) == false)
                return false;
            if (RegUtil.WriteRegValue(strConnectionInfo, "db_type", ((int)m_dbType).ToString(), m_nSiteID) == false)
                return false;
            if (RegUtil.WriteRegValue(strConnectionInfo, "page_encoding", m_encoding.CodePage.ToString(), m_nSiteID) == false)
                return false;
            if (RegUtil.WriteRegValue(strConnectionInfo, "webserver_url2", m_strWebServerURL, m_nSiteID) == false)
                return false;
        
            return true;
        }
    }

    class RegUtil
    {
        public const string BaseURL = @"Software\UNE\Site\";

        public static bool IsExist(string strKey)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(strKey);
                return key != null;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("IsExist Error, " + e.Message);
            }

            return false;
        }

        public static bool CreateKey(string strKey)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(strKey);
                return key != null;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("CreateKey Error, " + e.Message);
            }

            return false;
        }

        public static string ReadRegValue(string section, string key, int nSiteID)
        {
            string szResult = "";
            try
            {
                string szKey = @"Software\UNE\Site\" + nSiteID.ToString() + "\\" + section;
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKey);
                if (rkey == null)
                {
                    return "";
                }
                else
                {
                    szResult = (string)rkey.GetValue(key, "");
                }
                if (rkey != null)
                    rkey.Close();
            }
            catch (System.Exception)
            {
            }
            return szResult;
        }

        public static bool WriteRegValue(string section, string key, string szValue, int nSiteID)
        {
            try
            {
                string szUserName = Environment.UserDomainName + "\\" + Environment.UserName;

                RegistrySecurity rs = new RegistrySecurity();

                rs.AddAccessRule(new RegistryAccessRule(szUserName,
                    RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.WriteKey,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                rs.AddAccessRule(new RegistryAccessRule(szUserName,
                    RegistryRights.ChangePermissions,
                    InheritanceFlags.None,
                    PropagationFlags.None,
                    AccessControlType.Deny));

                string szKey = @"Software\UNE\Site\" + nSiteID.ToString() + "\\" + section;
                RegistryKey rkey = Registry.CurrentUser.OpenSubKey(szKey, true);
                if (rkey == null)
                {
                    try
                    {
                        rkey = Registry.CurrentUser.CreateSubKey(szKey, RegistryKeyPermissionCheck.ReadWriteSubTree, rs);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (rkey != null)
                {
                    rkey.SetValue(key, szValue);
                    rkey.Close();
                }
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Trace.WriteLine("WriteRegValue Error : " + e.Message);
                return false;
            }

            return true;
        }
    }
}
