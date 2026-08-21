namespace SafetyServer.BLL.Data.Response
{
    public class ResponseUserPosition : MessageResult
    {
        private string m_strUserID = null;
        private float? m_x = null;
        private float? m_y = null;
        private int? m_nZoneID = null;

        public string ID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
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
}
