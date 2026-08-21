using SensorMaker.BLL.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMaker.BLL.Models.Account
{
    public class AccountData
    {
        private LoginData m_login = null;
        private LogoutData m_logout = null;
        private AutoLoginData m_autoLogin = null;
        private RegisterData m_register = null;
        private RegisterParam m_registerParam = null;
        private LoginData m_registerPassword = null;
        private string m_strCurrentUser = null;
        private RequestRegist m_requestRegist = null;

        public LoginData Login
        {
            get { return m_login; }
            set { m_login = value; }
        }

        public LogoutData Logout
        {
            get { return m_logout; }
            set { m_logout = value; }
        }

        public AutoLoginData AutoLogin
        {
            get { return m_autoLogin; }
            set { m_autoLogin = value; }
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

        public RequestRegist RequestRegist
        {
            get { return m_requestRegist; }
            set { m_requestRegist = value; }
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

    public class LogoutData
    {
        private string m_strUserID = "";

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

    public class RegisterData
    {
        private const string Domain = "unes.co.kr";

        private string m_strName = "";
        private string m_strEmail = "";
        private string m_strPhoneNumber = "";
        private string m_strPassword = "";
        // 이 값이 true이면 새로운 사용자로 등록하는 것이고
        //         false이면 비밀번호를 변경하려는 것이다.
        private bool m_registNewUser = true;

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

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { SetPhoneNumber(value); }
        }

        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        // 이 값이 true이면 새로운 사용자로 등록하는 것이고
        //         false이면 비밀번호를 변경하려는 것이다.
        public bool RegistNewUser
        {
            get { return m_registNewUser; }
            set { m_registNewUser = value; }
        }

        private void SetPhoneNumber(string strPhoneNumber)
        {
            PhoneNumber phoneNumber = new PhoneNumber(strPhoneNumber);

            if (phoneNumber.IsValid)
                m_strPhoneNumber = phoneNumber.Number;
            else
                m_strPhoneNumber = "";
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

        public string IsValidPhoneNumber(out string strErrorMessage)
        {
            strErrorMessage = null;

            if (m_strPhoneNumber.Length > 0)
                return m_strPhoneNumber;

            strErrorMessage = "형식에 맞지않는 전화번호입니다.";
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

    public class RequestRegist
    {
        public class UserInfo
        {
            private int m_nID = 0;
            private bool m_isNormalUser = false;
            private bool m_isDeveloper = false;
            private bool m_isAdmin = false;

            public int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            public bool IsNormalUser
            {
                get { return m_isNormalUser; }
                set { m_isNormalUser = value; }
            }

            public bool IsDeveloper
            {
                get { return m_isDeveloper; }
                set { m_isDeveloper = value; }
            }

            public bool IsAdmin
            {
                get { return m_isAdmin; }
                set { m_isAdmin = value; }
            }
        }

        private bool m_permit = false;
        private List<UserInfo> m_users = new List<UserInfo>();
        private string m_strDenyDescription = null;

        public bool Permit
        {
            get { return m_permit; }
            set { m_permit = value; }
        }

        public List<UserInfo> Users
        {
            get { return m_users; }
            set { m_users = value; }
        }

        public string DenyDescription
        {
            get { return m_strDenyDescription; }
            set { m_strDenyDescription = value; }
        }
    }

    class PhoneNumber
    {
        private int m_nHeader = 0, m_nBody = 0, m_nTail = 0;
        private int m_nBodyLen = 0;
        private bool m_isBlank = false;

        public string Number
        {
            get { return GetPhoneNumber(); }
            set { SetPhoneNumber(value); }
        }

        public bool IsValid
        {
            get
            {
                if (m_isBlank)
                    return false;

                if (m_nHeader == 0 && m_nBody == 0 && m_nTail == 0)
                    return false;

                return true;
            }
        }

        public PhoneNumber()
        {
        }

        public PhoneNumber(string strPhoneNumber)
        {
            if (!String.IsNullOrWhiteSpace(strPhoneNumber))
            {
                SetPhoneNumber(strPhoneNumber);
            }
            else
            {
                m_isBlank = true;
            }
        }

        public int CompareTo(object obj)
        {
            PhoneNumber phone1 = this;
            PhoneNumber phone2 = (PhoneNumber)obj;

            if (phone1.m_nHeader < phone2.m_nHeader)
                return -1;
            else if (phone1.m_nHeader > phone2.m_nHeader)
                return 1;

            if (phone1.m_nBody < phone2.m_nHeader)
                return -1;
            else if (phone1.m_nBody > phone2.m_nBody)
                return 1;

            if (phone1.m_nTail < phone2.m_nTail)
                return -1;
            else if (phone1.m_nTail > phone2.m_nTail)
                return 1;

            return 0;
        }

        public override string ToString()
        {
            string strPhoneNumber = GetPhoneNumber();
            return strPhoneNumber;
        }

        private string GetPhoneNumber()
        {
            if (m_nBodyLen == 3)
                return string.Format("01{0}{1:000}{2:0000}", m_nHeader, m_nBody, m_nTail);
            else if (m_nBodyLen == 4)
                return string.Format("01{0}{1:0000}{2:0000}", m_nHeader, m_nBody, m_nTail);

            return "";
        }

        private void SetPhoneNumber(string strPhoneNumber)
        {
            string[] arrTokens = strPhoneNumber.Trim().Split('-');
            int nTokenCount = arrTokens.Length;

            m_nHeader = m_nBody = m_nTail = m_nBodyLen = 0;

            if (nTokenCount == 3)
                SetPhoneNumber2(arrTokens[0].Trim(), arrTokens[1].Trim(), arrTokens[2].Trim());
            else if (nTokenCount == 2)
                SetPhoneNumber2(arrTokens[0].Trim() + arrTokens[1].Trim());
            else if (nTokenCount == 1)
                SetPhoneNumber2(strPhoneNumber.Trim());
        }

        private bool SetPhoneNumber2(string strHead, string strBody, string strTail)
        {
            if (!strHead.StartsWith("01") || strHead.Length != 3)
                return false;

            char chHead = strHead[2];

            if (chHead < '0' || chHead > '9')
                return false;

            int nBody = 0, nTail = 0;
            int nBodyLen = strBody.Length;
            int nTailLen = strTail.Length;

            if (nBodyLen < 3 || nBodyLen > 4 || nTailLen != 4)
                return false;

            if (!int.TryParse(strBody, out nBody))
                return false;

            if (!int.TryParse(strTail, out nTail))
                return false;

            m_nHeader = chHead - '0';
            m_nBody = nBody;
            m_nTail = nTail;
            m_nBodyLen = nBodyLen;

            return true;
        }

        private bool SetPhoneNumber2(string strPhoneNumber)
        {
            int len = strPhoneNumber.Length;

            bool readNum = false;
            int nIndex1 = -1, nIndex2 = -1;

            for (int i = 0; i < len; i++)
            {
                char ch = strPhoneNumber[i];

                if (ch >= '0' && ch <= '9')
                {
                    readNum = true;
                }
                else if (ch == ' ' || ch == '\t')
                {
                    if (readNum)
                    {
                        readNum = false;

                        if (nIndex1 < 0)
                            nIndex1 = i;
                        else
                        {
                            nIndex2 = i;
                            break;
                        }
                    }
                }
            }

            if (nIndex1 >= 0 && nIndex2 > nIndex1)
            {
                string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
                string str2 = strPhoneNumber.Substring(nIndex1, nIndex2 - nIndex1 - 1).Trim();
                string str3 = strPhoneNumber.Substring(nIndex2).Trim();

                return SetPhoneNumber2(str1, str2, str3);
            }
            else if (nIndex1 >= 0)
            {
                string str1 = strPhoneNumber.Substring(0, nIndex1).Trim();
                string str2 = strPhoneNumber.Substring(nIndex1).Trim();

                int len1 = str1.Length;
                int len2 = str2.Length;

                if (len1 == 3 && (len2 == 7 || len2 == 8))
                {
                    return SetPhoneNumber2(str1, str2.Substring(0, len2 - 4), str2.Substring(len2 - 4));
                }
                else if ((len1 == 6 || len1 == 7) || len2 == 4)
                {
                    return SetPhoneNumber2(str1.Substring(0, 3), str1.Substring(3), str2);
                }
            }
            else
            {
                if (len == 10 || len == 11)
                {
                    string str1 = strPhoneNumber.Substring(0, 3);
                    string str2 = strPhoneNumber.Substring(3, len - 7);
                    string str3 = strPhoneNumber.Substring(len - 4);

                    return SetPhoneNumber2(str1, str2, str3);
                }
            }

            return false;
        }
    }

    public class LogoutResult : Result
    {
    }
}
