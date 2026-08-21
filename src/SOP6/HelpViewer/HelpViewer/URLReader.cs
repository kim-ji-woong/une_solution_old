using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Forms;
using System.IO;

namespace HelpViewer
{
    class URLReader
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        public static string GetURL(OpenOption option, ref string strWebServerURL, ref Encoding pageEncoding)
        {
            if (option.Encoding != null)
                pageEncoding = option.Encoding;

            if (option.Option == OpenOption.URLOption.SITE_ID)
            {
                if (option.SiteID >= 0)
                {
                    string strURL = GetHTMLURL(option.SiteID, ref strWebServerURL, ref pageEncoding);

                    if (strURL != null && strURL.Length > 0)
                        return strURL;
                }
            }
            else if (option.Option == OpenOption.URLOption.URL)
            {
                if (option.URL.Length > 2)
                {
                    char first = option.URL.ElementAt(0);
                    char second = option.URL.ElementAt(1);

                    // Local 경로인가?
                    if (((first >= 'a' && first <= 'z') || (first >= 'A' && first <= 'Z')) && second == ':')
                    {
                        if (Directory.Exists(option.URL))
                            return option.URL;
                    }
                    else
                    {
                        int nBeginIndex = option.URL.IndexOf("//");

                        if (nBeginIndex < 0)
                            nBeginIndex = 0;
                        else
                            nBeginIndex = nBeginIndex + 2;

                        int nEndIndex = option.URL.IndexOf('/', nBeginIndex);

                        if (nEndIndex > 0)
                            strWebServerURL = option.URL.Substring(0, nEndIndex);
                        else
                            strWebServerURL = option.URL;

                        return option.URL;
                    }
                }
            }

            // 1. Local 경로에 파일이 존재하면 Local 파일을 읽는다.
            int nIndex = Application.ExecutablePath.LastIndexOf('\\');

            if (nIndex >= 0)
            {
                string strLocalURL = Application.ExecutablePath.Substring(0, nIndex) + "\\HelpHtml";

                if (Directory.Exists(strLocalURL))
                    return strLocalURL;
            }

            // 2. Config.ini가 있으면 SiteID를 읽어 Registry로부터 URL을 얻어온다.
            int nSiteID = ReadSiteID();

            if (nSiteID >= 0)
            {
                string strURL = GetHTMLURL(nSiteID, ref strWebServerURL, ref pageEncoding);

                if (option.Encoding != null)
                    pageEncoding = option.Encoding;

                if (strURL != null && strURL.Length > 0)
                    return strURL;
            }

            // 3. 둘다 없으면 U&E의 온라인 버전을 읽는다.
/*#if KPX
            strWebServerURL = "http://183.104.147.144:18080";
#else
            strWebServerURL = "http://unes.iptime.org:10091";
#endif*/
            return strWebServerURL + "/HelpHtml";
        }

        private static string getinivalue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            string strPath = Application.StartupPath + "\\config.ini";
            GetPrivateProfileString(section, key, "", temp, 255, strPath);
            
            return temp.ToString();

        }

        private static int ReadSiteID()
        {
            string szSiteID = getinivalue("Server Connection Info", "siteid");
            if (szSiteID == null || szSiteID == "")
            {
                return -1;
            }

            int nSiteId = 1;

            if (!int.TryParse(szSiteID, out nSiteId))
            {
                return -1;
            }

            return nSiteId;
        }

        private static string GetHTMLURL(int nSiteID, ref string strWebServerURL, ref Encoding pageEncoding)
        {
            string strSection = "Server Connection Info";
            string strURL = "";

            strWebServerURL = ReadRegValue(strSection, "webserver_url2", nSiteID);

            if (strWebServerURL != null && strWebServerURL.Length > 0)
            {
                if (strWebServerURL.EndsWith("/"))
                    strURL = strWebServerURL + "HelpHTML";
                else
                    strURL = strWebServerURL + "/HelpHTML";
            }
            else
                strWebServerURL = "";

            string szEncoding = ReadRegValue(strSection, "page_encoding", nSiteID);

            if (szEncoding != null && szEncoding.Length > 0)
            {
                try
                {
                    int nEncoding = -1;
                    if (int.TryParse(szEncoding, out nEncoding))
                    {
                        pageEncoding = System.Text.Encoding.GetEncoding(nEncoding);
                    }
                    else
                    {
                        pageEncoding = Encoding.UTF8;
                    }
                }
                catch (Exception)
                {
                }
            }

            return strURL;
        }

        private static string ReadRegValue(string section, string key, int nSiteID)
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
    }

    public class OpenOption
    {
        public enum URLOption { NONE = 0, SITE_ID, URL };
        public enum SelectionOption { NONE = 0, NODE, ID };

        private URLOption m_option = URLOption.NONE;
        private int m_nSiteID = -1;
        private string m_strURL = "";
        private System.Text.Encoding m_encoding = null;
        private SelectionOption m_beginSelection = SelectionOption.NONE;
        private string m_strBeginSelectionArgument = "";
        private string m_strAppName = null;
        
        public URLOption Option
        {
            get { return m_option; }
            set { m_option = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public string URL
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }

        public System.Text.Encoding Encoding
        {
            get { return m_encoding; }
            set { m_encoding = value; }
        }

        public SelectionOption BeginSelection
        {
            get { return m_beginSelection; }
        }

        public string BeginSelectionArgument
        {
            get { return m_strBeginSelectionArgument; }
        }

        public string ApplicationName
        {
            get { return m_strAppName; }
            set { m_strAppName = value; }
        }

        public void SetBeginSelection(SelectionOption option, string strArgument = "")
        {
            m_beginSelection = option;
            m_strBeginSelectionArgument = strArgument;
        }
    }
}
