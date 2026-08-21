using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class AccountUser
    {
        public enum Fields { ID, UserID, Password, NickName, UserLevel, FacilityType };

        private int m_nID = -1;
        // 실제 로그인시 사용할 문자열 ID
        private string m_strUserID = "";
        private string m_strPassword = "";
        private string m_strNickName = "";
        // Level의 ID
        private int m_nUserLevel = -1;
        private string m_strFacilityType = "";
        private List<int> m_listFacilityType = new List<int>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
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

        // Level의 ID
        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
        }

        public string FacilityType
        {
            get { return m_strFacilityType; }
            set { m_strFacilityType = value; }
        }

        // FacilityType 동일한 값, 다만 리스트 형태로 저장 
        public List<int> ListFacilityType
        {
            get { return m_listFacilityType; }
            set { m_listFacilityType = value; }
        }


        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "AccountUser"; }
        }
    }
}
