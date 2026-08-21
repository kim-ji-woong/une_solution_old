using System.Collections.Generic;

namespace SafetyServer.BLL.Data.Response
{
    public class ResponseFieldUserPosition : MessageResult
    {
        public class UserPosition
        {
            private string m_strID = "";
            private float? m_x = null;
            private float? m_y = null;
            private int? m_nZoneID = null;

            public string ID
            {
                get { return m_strID; }
                set { m_strID = value; }
            }

            public float? X
            {
                get { return m_x; }
                set { m_x = value; }
            }

            public float? Y
            {
                get { return m_y; }
                set { m_y = value; }
            }

            public int? FieldID
            {
                get { return m_nZoneID; }
                set { m_nZoneID = value; }
            }
        }

        private List<UserPosition> m_userPositions = new List<UserPosition>();

        public List<UserPosition> UserPositions
        {
            get { return m_userPositions; }
            set { m_userPositions = value; }
        }
    }
}
