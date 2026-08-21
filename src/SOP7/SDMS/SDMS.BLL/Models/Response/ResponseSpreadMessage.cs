using System.Collections.Generic;

namespace SDMS.BLL.Models.Response
{
    public class ResponseSpreadMessage : MessageResult
    {
        private List<SDMS.Model.Config.SpreadMessage> m_spreadMessages = null;

        public List<SDMS.Model.Config.SpreadMessage> SpreadMessages
        {
            get { return m_spreadMessages; }
            set { m_spreadMessages = value; }
        }
    }
}
