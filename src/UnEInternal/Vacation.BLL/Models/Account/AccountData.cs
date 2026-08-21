using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vacation.BLL.Models.Account
{
    public class AccountData
    {
        private LoginData m_login = null;
        private ExternalLoginData m_loginExternal = null;
        private ExternalLogoutData m_logoutExternal = null;
        private AutoLoginData m_autoLogin = null;
        private AutoLoginData m_externalAutoLogin = null;
        private RequestNewLoginKey m_requestNewLogin = null;
        private LogoutData m_logout = null;
        private RegisterData m_register = null;
        private RegisterParam m_registerParam = null;
        private LoginData m_registerPassword = null;
        private string m_strCurrentUser = null;

        public LoginData Login
        {
            get { return m_login; }
            set { m_login = value; }
        }

        public ExternalLoginData ExternalLogin
        {
            get { return m_loginExternal; }
            set { m_loginExternal = value; }
        }

        public LogoutData Logout
        {
            get { return m_logout; }
            set { m_logout = value; }
        }

        public ExternalLogoutData ExternalLogout
        {
            get { return m_logoutExternal; }
            set { m_logoutExternal = value; }
        }

        public AutoLoginData AutoLogin
        {
            get { return m_autoLogin; }
            set { m_autoLogin = value; }
        }

        public AutoLoginData ExternalAutoLogin
        {
            get { return m_externalAutoLogin; }
            set { m_externalAutoLogin = value; }
        }

        public RequestNewLoginKey RequestNewLoginKey
        {
            get { return m_requestNewLogin; }
            set { m_requestNewLogin = value; }
        }

        public RegisterData Register
        {
            get { return m_register; }
            set { m_register = value; }
        }

        public RegisterParam RegisterParam
        {
            get { return m_registerParam; }
            set { m_registerParam = value; }
        }

        public LoginData RegisterPassword
        {
            get { return m_registerPassword; }
            set { m_registerPassword = value; }
        }

        public string CurrentUser
        {
            get { return m_strCurrentUser; }
            set { m_strCurrentUser = value; }
        }
    }

    public class LoginData
    {
        private string m_strValue = "";

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }
    }

    public class ExternalLoginData
    {
        private string m_strUserID = null;
        private string m_strHashCode = null;
        private string m_strPassword = null;

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string HashCode
        {
            get { return m_strHashCode; }
            set { m_strHashCode = value; }
        }

        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }
    }

    public class ExternalLogoutData
    {
        private string m_strUserID = null;

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }
    }

    public class AutoLoginData
    {
        private string m_strBeginCode = null;

        public string BeginCode
        {
            get { return m_strBeginCode; }
            set { m_strBeginCode = value; }
        }
    }

    public class RequestNewLoginKey
    {
        private string m_strBeginCode = null;

        public string BeginCode
        {
            get { return m_strBeginCode; }
            set { m_strBeginCode = value; }
        }
    }

    public class LogoutData
    {
        private string m_strUserID = "";

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }
    }

    public class RegisterData
    {
        private const string Domain = "unes.co.kr";

        private string m_strName = "";
        private string m_strEmail = "";
        private string m_strReturnUrl = "";

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Email
        {
            get { return m_strEmail; }
            set { m_strEmail = value; }
        }

        public string ReturnUrl
        {
            get { return m_strReturnUrl; }
            set { m_strReturnUrl = value; }
        }

        public string IsValidEmail(out string strErrorMessage)
        {
            strErrorMessage = null;

            string strEmail = m_strEmail.Trim();
            int nIndex = strEmail.IndexOf('@');

            if (nIndex <= 0)
            {
                strErrorMessage = "전자메일주소 형식에 맞지 않습니다.";
                return null;
            }

            string strID = strEmail.Substring(0, nIndex);
            string strDomain = strEmail.Substring(nIndex + 1);

            if (string.Compare(strDomain, Domain, true) == 0)
            {
                return strID;
            }

            strErrorMessage = "유엔이의 메일주소가 아닙니다.";
            return null;
        }
    }

    public class RegisterParam
    {
        private string m_strValue = "";

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }
    }
}
