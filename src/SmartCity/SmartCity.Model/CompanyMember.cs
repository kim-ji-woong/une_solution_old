using System;
using System.Collections.Generic;

namespace SmartCity.Model
{
    public class CompanyMember
    {
        public enum Fields { ID, MemberName, RegularTeamID, LevelID, PhoneNumber, FacilityTypes };

        private int m_nID = -1;
        private string m_strMemberName = "";
        private int m_nRegularTeamID = -1;
        private int m_nLevelID = -1;
        private string m_strPhoneNumber = "";
        private string m_strFacilityTypes = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public int RegularTeamID
        {
            get { return m_nRegularTeamID; }
            set { m_nRegularTeamID = value; }
        }

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public string FacilityTypes
        {
            get { return m_strFacilityTypes; }
            set { m_strFacilityTypes = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.FacilityTypes)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "CompanyMember"; }
        }
    }
}
