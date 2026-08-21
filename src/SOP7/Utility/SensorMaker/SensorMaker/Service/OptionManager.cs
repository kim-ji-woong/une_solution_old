using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorMaker.Service
{
    public class OptionManager
    {
        private string m_strSystemMail = "";
        private string m_strAdminMail = "";
        private string m_strSystemCode = "";
        private string m_strSiteURL = "";
        private string m_strSolutionName = "";
        private string m_strWebServerURL = "";
        // Login 기능을 휴가관리 사이트와 호환되게 할 것인가?
        private string m_strExternalLoginURL = null;

        public string SystemMail
        {
            get { return m_strSystemMail; }
            set { m_strSystemMail = value; }
        }

        public string AdminMail
        {
            get { return m_strAdminMail; }
            set { m_strAdminMail = value; }
        }

        public string SystemCode
        {
            get { return m_strSystemCode; }
            set { m_strSystemCode = value; }
        }

        public string SiteURL
        {
            get { return m_strSiteURL; }
            set { m_strSiteURL = value; }
        }
        
        public string SolutionName
        {
            get { return m_strSolutionName; }
            set { m_strSolutionName = value; }
        }

        public string WebServerURL
        {
            get { return m_strWebServerURL; }
            set { m_strWebServerURL = value; }
        }

        // Login 기능을 휴가관리 사이트와 호환되게 할 것인가?
        public string ExternalLogin
        {
            get { return m_strExternalLoginURL; }
            set { m_strExternalLoginURL = value; }
        }

        public OptionManager()
        {
        }

        public OptionManager(string strSystemMail, string strAdminMail, string strSystemCode, string strSiteURL, string strSolutionName, string strWebServerURL, string strExternalLogin)
        {
            m_strSystemMail = strSystemMail;
            m_strAdminMail = strAdminMail;
            m_strSystemCode = strSystemCode;
            m_strSiteURL = strSiteURL;
            m_strSolutionName = strSolutionName;
            m_strWebServerURL = strWebServerURL;
            m_strExternalLoginURL = strExternalLogin;
        }
    }
}
