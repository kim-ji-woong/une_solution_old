using SmartCity.Model;
using System.Collections.Generic;

namespace SmartCity.BLL.Models.Response
{

    public class ResponseManualList : MessageResult
    {
        private List<Manual> m_listManual = null;

        public List<Manual> Manuals
        {
            get { return m_listManual; }
            set { m_listManual = value; }
        }
    }

    public class Manual
    {
        private int m_nID = -1;
        private int m_nFacilityType = -1;
        private string m_strManualType = "";
        private string m_strManualTitle = "";
        private List<Member> m_listMember = null;
        private int m_nNumber = -1;
        private string m_strManual = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int FacilityType
        {
            get { return m_nFacilityType; }
            set { m_nFacilityType = value; }
        }

        public string ManualType
        {
            get { return m_strManualType; }
            set { m_strManualType = value; }
        }

        public string ManualTitle
        {
            get { return m_strManualTitle; }
            set { m_strManualTitle = value; }
        }

        public List<Member> Members
        {
            get { return m_listMember; }
            set { m_listMember = value; }
        }

        public int Number
        {
            get { return m_nNumber; }
            set { m_nNumber = value; }
        }

        public string ManualContent
        {
            get { return m_strManual; }
            set { m_strManual = value; }
        }
    }

    public class Member
    {
        private int m_nID = -1;
        private string m_strMemberName = "";
        private RegularTeam m_regularTeam = null;
        private JobLevel m_jobLevel = null;
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

        public RegularTeam RegularTeam
        {
            get { return m_regularTeam; }
            set { m_regularTeam = value; }
        }

        public JobLevel JobLevel
        {
            get { return m_jobLevel; }
            set { m_jobLevel = value; }
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
    }

}
