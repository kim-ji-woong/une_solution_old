using System.Collections.Generic;

namespace SOPManager.BLL.Models.Response
{
    using Model.Sop.Config;
    using SOPManager.Model.Sop.Account;

    public class ResponseAccountPopup : MessageResult
    {
        private List<Option> m_accountPopups = null;

        public List<Option> AccountPopups
        {
            get { return m_accountPopups; }
            set { m_accountPopups = value; }
        }
    }
}
