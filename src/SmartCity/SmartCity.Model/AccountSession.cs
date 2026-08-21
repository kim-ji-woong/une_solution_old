using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class AccountSession
    {
        public enum Fields { ID, AccountUserID, SessionKey, CreateDate, UpdateDate };

        private int m_nID = -1;
        private int m_nAccountUserID = -1;
        private string m_strSessionKey = "";
        private DateTime m_dtCreateDate = new DateTime();
        private DateTime? m_dtUpdateDate = null;

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

        public DateTime? UpdateDate
        {
            get { return m_dtUpdateDate; }
            set { m_dtUpdateDate = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.UpdateDate)
                isNullable = true;
            else 
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "AccountSession"; }
        }
    }
}
