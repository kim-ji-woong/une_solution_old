using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamReader
{
    public class RegularTeam
    {
        private int m_nID;
        private string m_strTeamName;
        private RegularTeam m_teamParent = null;
        private string m_strTeamCode = "";
        private CompanyMember m_teamLeader = null;

        public static bool IsSame(RegularTeam team1, RegularTeam team2)
        {
            if (team1 == null && team2 == null)
                return true;
            else if (team1 == null)
                return false;
            else if (team2 == null)
                return false;
            else if (team1.TeamName == team2.TeamName &&
                team1.TeamCode == team2.TeamCode)
                return true;

            return false;
        }

        public int ID   // DB Table RegularTeam의 ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public RegularTeam ParentTeam
        {
            get { return m_teamParent; }
            set { m_teamParent = value; }
        }

        public string TeamCode
        {
            get { return m_strTeamCode; }
            set { m_strTeamCode = value; }
        }

        public CompanyMember TeamLeader
        {
            get { return m_teamLeader; }
            set { m_teamLeader = value; }
        }
    }

    public class CompanyMember
    {
        private int m_nID = -1;
        private string m_strMemberID = "";
        private RegularTeam m_teamRegular = null;
        private string m_strMemberName = "";
        private int m_nLevelID = 0;    // 직급
        private int m_nPositionID = 0;  // 직위(0:알수 없음, 1:팀원, 2:팀장, 3:파트장, 4:센터장, 100:육아휴직, 101:군휴직, 102:휴직)
        private string m_strTitle = "";
        private string m_strPhoneNumber = "";
        private string m_strOfficePhoneNumber = "";

        public int ID // DB Table CompanyMember의 ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string MemberID  // 사번
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        public RegularTeam Team
        {
            get { return m_teamRegular; }
            set { m_teamRegular = value; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public int LevelID  // 직급
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public int PositionID   // 직위
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }

        public string Title // 직책
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public string OfficePhoneNumber
        {
            get { return m_strOfficePhoneNumber; }
            set { m_strOfficePhoneNumber = value; }
        }
    }
}
