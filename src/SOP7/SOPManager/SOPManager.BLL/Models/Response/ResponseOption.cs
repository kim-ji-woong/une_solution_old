using System.Collections.Generic;

namespace SOPManager.BLL.Models.Response
{
    using Model.Sop.Config;
    using SOPManager.Model.Sop.Account;

    public class ResponseOption : MessageResult
    {
        private List<Option> m_options = null;

        public List<Option> Options
        {
            get { return m_options; }
            set { m_options = value; }
        }
    }
}
