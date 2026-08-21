using System;
using System.Collections.Generic;
using System.Text;

namespace Common.BLL.Models.Response
{
    public class ResponseAccountPopup : MessageResult
    {
        private List<SOPManager.Model.Sop.Account.Option> m_accountPopups = null;

        public List<SOPManager.Model.Sop.Account.Option> AccountPopups
        {
            get { return m_accountPopups; }
            set { m_accountPopups = value; }
        }
    }
}
