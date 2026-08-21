namespace AgentFactory.BLL
{
    public class ClientMessage
    {
        private int m_nClientType = 0;
        private int m_nClientSubType = 0;
        private int m_nHeader = 0;
        private byte[] m_bytes = null;

        public int ClientType
        {
            get { return m_nClientType; }
            set { m_nClientType = value; }
        }

        public int ClientSubType
        {
            get { return m_nClientSubType; }
            set { m_nClientSubType = value; }
        }

        public int Header
        {
            get { return m_nHeader; }
            set { m_nHeader = value; }
        }

        public byte[] Bytes
        {
            get { return m_bytes; }
            set { m_bytes = value; }
        }
    }
}
