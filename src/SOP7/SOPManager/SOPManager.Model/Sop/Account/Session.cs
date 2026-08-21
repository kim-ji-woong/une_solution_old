using System;

namespace SOPManager.Model.Sop.Account
{
    public class Session
    {
        public enum Fields { ID, AccountUserID, SessionKey, CreateDate, UpdateDate, IsAutoLogin };

        private int m_nID = -1;
        private int m_nAccountUserID = -1;
        private string m_strSessionKey = "";
        private DateTime m_dtCreateDate = new DateTime();
        private DateTime m_dtUpdateDate = new DateTime();
        private bool m_isAutoLogin = false;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int AccountUserID
        {
            get { return m_nAccountUserID; }
            set { m_nAccountUserID = value; }
        }

        public string SessionKey
        {
            get { return m_strSessionKey; }
            set { m_strSessionKey = value; }
        }

        public DateTime CreateDate
        {
            get { return m_dtCreateDate; }
            set { m_dtCreateDate = value; }
        }

        public DateTime UpdateDate
        {
            get { return m_dtUpdateDate; }
            set { m_dtUpdateDate = value; }
        }

        public bool IsAutoLogin
        {
            get { return m_isAutoLogin; }
            set { m_isAutoLogin = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "SopAccountSession"; }
        }
    }
}
