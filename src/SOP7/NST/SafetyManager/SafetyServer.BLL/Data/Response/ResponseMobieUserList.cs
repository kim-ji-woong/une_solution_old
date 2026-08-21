using System;
using System.Collections.Generic;
using System.Text;

namespace SafetyServer.BLL.Data.Response
{
    using Models;

    public class ResponseMobieUserList : MessageResult
    {
        private List<MobileUser> m_userList = new List<MobileUser>();

        public List<MobileUser> UserList
        {
            get { return m_userList; }
            set { m_userList = value; }
        }

        public ResponseMobieUserList()
        {
        }

        public ResponseMobieUserList(bool success, string strMessage)
        {
            Success = success;
            Message = strMessage;
        }
    }
}
