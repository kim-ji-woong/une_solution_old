using SOPManager.BLL.Models.Request;
using SOPManager.BLL.Models.Response;
using SOPManager.Model.Sop.Account;
using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.Model.Sop.Team;

namespace SOPManager.BLL.Models
{
    public class AccountData
    {
        private LoginData m_login = null;
        //private LogoutData m_logout = null;
        //private RegisterData m_register = null;
        //private RegisterParam m_registerParam = null;
        //private LoginData m_registerPassword = null;
        //private string m_strCurrentUser = null;
        private AutoLoginData m_autoLogin = null;
        private bool? m_getAccountLevels = null;
        private bool? m_getAccountUsers = null;
        private RequestAccountUser m_updateAccountUsers = null;
        //private List<AccountUser> m_updateAccountUsers = null; 
        private List<AccountUser> m_removeAccountUsers = null;
        private List<AccountUser> m_reRegisterAccountUsers = null;
        private ChangePassword m_changePassword = null;
        private CheckParamsCode m_checkParamsCode = null;
        private SetPassword m_setPassword = null;
        private CheckLoginSession m_checkLoginSession = null;

        public LoginData Login
        {
            get { return m_login; }
            set { m_login = value; }
        }

        public AutoLoginData AutoLogin
        {
            get { return m_autoLogin; }
            set { m_autoLogin = value; }
        }

        public bool? GetAccountLevels
        {
            get { return m_getAccountLevels; }
            set { m_getAccountLevels = value; }
        }

        public bool? GetAccountUsers
        {
            get { return m_getAccountUsers; }
            set { m_getAccountUsers = value; }
        }

        public RequestAccountUser UpdateAccountUsers
        {
            get { return m_updateAccountUsers; }
            set { m_updateAccountUsers = value; }
        }

        public List<AccountUser> RemoveAccountUsers
        {
            get { return m_removeAccountUsers; }
            set { m_removeAccountUsers = value; }
        }

        public List<AccountUser> ReRegisterAccountUsers
        {
            get { return m_reRegisterAccountUsers; }
            set { m_reRegisterAccountUsers = value; }
        }

        public ChangePassword ChangePassword
        {
            get { return m_changePassword; }
            set { m_changePassword = value; }
        }

        public CheckParamsCode CheckParamsCode
        {
            get { return m_checkParamsCode; }
            set { m_checkParamsCode = value; }
        }

        public SetPassword SetPassword
        {
            get { return m_setPassword; }
            set { m_setPassword = value; }
        }

        public CheckLoginSession CheckLoginSession
        {
            get { return m_checkLoginSession; }
            set { m_checkLoginSession = value; }
        }
    }

    public class LoginData
    {
        private string m_strValue = "";
        private string m_strKey = "";
        private bool m_isFullVersion = true;
        //private string m_strVersion = "";

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }

        public bool IsFullVersion
        {
            get { return m_isFullVersion; }
            set { m_isFullVersion = value; }
        }
        /*public string Version
        {
            get { return m_strVersion; }
            set { m_strVersion = value; }
        }*/
    }

    public class AutoLoginData
    {
        private string m_strBeginCode = null;
        private string m_strKey = null;

        public string BeginCode
        {
            get { return m_strBeginCode; }
            set { m_strBeginCode = value; }
        }

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }
    }

    public class ChangePassword
    {
        private string m_strName = "";
        private string m_strData = "";
        private string m_strValue = "";
        private string m_strKey = "";
        private int m_nMode = 0;

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Data
        {
            get { return m_strData; }
            set { m_strData = value; }
        }

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }

        public int Mode
        {
            get { return m_nMode; }
            set { m_nMode = value; }
        }
    }

    public class CheckParamsCode
    {
        private string m_strCode = "";

        public string Code
        {
            get { return m_strCode; }
            set { m_strCode = value; }
        }
    }

    public class SetPassword
    {
        private string m_strValue = "";
        private string m_strKey = "";

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }
    }

    public class AccountUser
    {
        // regular member 정보
        private int m_nID = -1;
        private Regular m_regular = null;
        private string m_strMemberName = null;
        private string m_strMemberID = null;
        private string m_strOfficePhoneNumber = null;
        private string m_strPhoneNumber = null;
        private JobLevel m_jobLevel = null;
        private JobPosition m_jobPosition = null;
        private string m_strEmail = null;

        // account user 정보
        private int m_nAccountID = -1;
        private Level m_accountLevel = null;
        private string m_strUserID = null;  // 계정 입력 ID
        private string m_strNickName = null;
        private string m_strPassword = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public Regular Regular
        {
            get { return m_regular; }
            set { m_regular = value; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        public string OfficePhoneNumber
        {
            get { return m_strOfficePhoneNumber; }
            set { m_strOfficePhoneNumber = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public JobLevel JobLevel
        {
            get { return m_jobLevel; }
            set { m_jobLevel = value; }
        }

        public JobPosition JobPosition
        {
            get { return m_jobPosition; }
            set { m_jobPosition = value; }
        }

        public string Email
        {
            get { return m_strEmail; }
            set { m_strEmail = value; }
        }

        public int AccountID
        {
            get { return m_nAccountID; }
            set { m_nAccountID = value; }
        }

        public Level AccountLevel
        {
            get { return m_accountLevel; }
            set { m_accountLevel = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string NickName
        {
            get { return m_strNickName; }
            set { m_strNickName = value; }
        }

        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }
    }

    public class JobLevel
    {
        private int m_nID = -1;
        private string m_strLevelName = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strLevelName; }
            set { m_strLevelName = value; }
        }
    }

    public class JobPosition
    {
        private int m_nID = -1;
        private string m_strPositionName = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strPositionName; }
            set { m_strPositionName = value; }
        }
    }

    public class CheckLoginSession
    {
        private int m_nUserID = -1;
        private string m_strSessionKey = "";


        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public string SessionKey
        {
            get { return m_strSessionKey; }
            set { m_strSessionKey = value; }
        }
    }
}
