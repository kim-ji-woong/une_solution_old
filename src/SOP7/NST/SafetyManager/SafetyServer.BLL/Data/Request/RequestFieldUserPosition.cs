using System.Collections.Generic;

namespace SafetyServer.BLL.Data.Request
{
    public class RequestFieldUserPosition
    {
        private int? m_nZoneID = null;
        private List<string> m_userIDs = null;

        public int? FieldID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public List<string> UserIDs
        {
            get { return m_userIDs; }
            set { m_userIDs = value; }
        }
    }
}
