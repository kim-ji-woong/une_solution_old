namespace NipaSOP.BLL.Models.Request
{
    public class RequestData
    {
        private StartInfo m_sopParameter = null;
        private RunSOP m_runSOP = null;

        public StartInfo StartInfo
        {
            get { return m_sopParameter; }
            set { m_sopParameter = value; }
        }

        public RunSOP RunSOP
        {
            get { return m_runSOP; }
            set { m_runSOP = value; }
        }
    }
}
