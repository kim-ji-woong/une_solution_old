using System;
using System.Collections.Generic;
using System.Text;

namespace Vacation.Model
{
    public class ExternalLogin
    {
        public enum Fields { UserID, LoginKey, LoginTime, Enabled };

        private string m_strUserID = "";
        private long m_nLoginKey = 0;
        private DateTime m_dtLogin = new DateTime();
        private bool m_enabled = false;

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public long LoginKey
        {
            get { return m_nLoginKey; }
            set { m_nLoginKey = value; }
        }

        public DateTime LoginTime
        {
            get { return m_dtLogin; }
            set { m_dtLogin = value; }
        }

        public bool Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public static string GetTableName()
        {
            return "ExternalLogin";
        }
    }
}
