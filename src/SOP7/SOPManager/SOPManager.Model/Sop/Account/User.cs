namespace SOPManager.Model.Sop.Account
{
    public class User
    {
        public enum Fields { ID, MemberID, UserLevel, Password, UserID, NickName, SiteID, PasswordCode };

        private int m_nID = -1;
        // CompanyMember의 ID
        private int? m_nMemberID = null;
        // Level의 ID
        private int m_nUserLevel = -1;
        // 실제 로그인시 사용할 문자열 ID
        private string m_strUserID = "";
        private string m_strPassword = "";
        private string m_strNickName = "";
        private int m_nSiteID = -1;
        private string m_strPasswordCode = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // CompanyMember의 ID
        public int? MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        // Level의 ID
        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
        }

        // 실제 로그인시 사용할 문자열 ID
        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        public string NickName
        {
            get { return m_strNickName; }
            set { m_strNickName = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public string PasswordCode
        {
            get { return m_strPasswordCode; }
            set { m_strPasswordCode = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.MemberID ||
                field == Fields.PasswordCode )
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "SopAccountUser"; }
        }
    }
}
