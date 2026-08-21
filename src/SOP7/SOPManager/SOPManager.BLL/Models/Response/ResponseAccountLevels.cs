using System.Collections.Generic;

namespace SOPManager.BLL.Models.Response
{
    using Model.Sop.Config;
    using SOPManager.Model.Sop.Account;

    public class ResponseAccountLevels : MessageResult
    {
        private List<Level> m_accountLevels = null;

        public List<Level> AccountLevels
        {
            get { return m_accountLevels; }
            set { m_accountLevels = value; }
        }
    }
}
