namespace SafetyServer.BLL.Data.Request
{
    public class LoginEvent
    {
        private string m_strID = null;
        private bool m_isLogin = true;

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public bool Login
        {
            get { return m_isLogin; }
            set { m_isLogin = value; }
        }
    }
}
