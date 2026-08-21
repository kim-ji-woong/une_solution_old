using SOPManager.BLL.Models.Response;
using SOPManager.Model.Sop.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOPManager.BLL.Models
{
    [Serializable]
    public class ApplicationUser
    {
        private int m_nID = -1;
        private int m_nLevelID = -1;
        private string m_strLevel = "";
        private string m_strUserID = "";
        private string m_strNickName = "";
        private string m_strSessionKey = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public string Level
        {
            get { return m_strLevel; }
            set { m_strLevel = value; }
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

        public string SessionKey 
        { 
            get { return m_strSessionKey; }
            set { m_strSessionKey = value; }
        }

        public static ApplicationUser MakeUser(User user, Level level, string strSessionKey)
        {
            ApplicationUser appUser = new ApplicationUser();
            appUser.ID = user.ID;
            appUser.LevelID = user.UserLevel;
            appUser.Level = level.LevelName;
            appUser.UserID = user.UserID;
            appUser.NickName = user.NickName;
            appUser.SessionKey = strSessionKey;

            return appUser;
        }
    }


    public class LoginResult : MessageResult
    {
        private ApplicationUser m_user = null;

        public ApplicationUser User
        {
            get { return m_user; }
            set { m_user = value; }
        }


        public LoginResult()
            : base()
        {
        }

        public LoginResult(bool success, string message)
            : base(success, message)
        {
        }
    }
}
