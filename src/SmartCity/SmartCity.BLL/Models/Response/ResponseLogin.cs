using SmartCity.Model;
using System.Collections.Generic;

namespace SmartCity.BLL.Models.Response
{

    public class ResponseLogin : MessageResult
    {
        private string m_strKey = null;
        private ApplicationUser m_User = null;

        public string KEY
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }

        public ApplicationUser User
        {
            get { return m_User; }
            set { m_User = value; }
        }
    }

    public class ApplicationUser
    {
        private int m_nID = -1;
        private AccountUser m_User = null;
        private AccountLevel m_Level = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public AccountUser User
        {
            get { return m_User; }
            set { m_User = value; }
        }

        public AccountLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public static ApplicationUser MakeUser(AccountUser user, AccountLevel level)
        {
            ApplicationUser appUser = new ApplicationUser();
            appUser.ID = user.ID;
            appUser.User = user;
            appUser.Level = level;

            return appUser;
        }
    }
}
