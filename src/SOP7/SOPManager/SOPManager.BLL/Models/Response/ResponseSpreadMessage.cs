using System.Collections.Generic;

namespace SOPManager.BLL.Models.Response
{
    using Model.Sop.Config;
    using SOPManager.Model.Sop.Account;

    public class ResponseSpreadMessage : MessageResult
    {
        private ShortcutKey m_shortcutKey = null;
        private List<SDMS.Model.Config.SpreadMessage> m_spreadMessages = null;

        public List<SDMS.Model.Config.SpreadMessage> SpreadMessages
        {
            get { return m_spreadMessages; }
            set { m_spreadMessages = value; }
        }
    }
}
