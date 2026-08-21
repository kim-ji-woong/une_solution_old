using System.Collections.Generic;

namespace SOPManager.BLL.Models.Response
{
    using Model.Sop.Config;
    using SOPManager.Model.Sop.Account;
    using TeamEditor.Model.Sop.Team;

    public class ResponseAccountUsers : MessageResult
    {
        private List<AccountUser> m_accountUsers = null;

        public List<AccountUser> AccountUsers
        {
            get { return m_accountUsers; }
            set { m_accountUsers = value; }
        }
    }
}
