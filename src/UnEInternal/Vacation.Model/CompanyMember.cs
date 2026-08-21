using System;

namespace Vacation.Model
{
    public class CompanyMember : IComparable
    {
        public enum Fields { ID, Name, JobLevelID, StartDate, TeamID, IsTeamLeader, IsAdmin, UserID, UserPW, PasswordCode, PhoneNumber };

        private int m_nID = -1;
        private string m_strName = "";
        // 직급
        private int m_nJobLevelID = -1;
        // 입사일자
        private DateTime m_dtStart = new DateTime();
        private int m_nTeamID = -1;
        // 팀장인가?
        private bool m_isTeamLeader = false;
        // 관리자인가?
        private bool m_isAdmin = false;
        private string m_strUserID = "";
        // 사용자 암호의 Hash값
        private string m_strPW = null;
        // 비밀번호 변경을 위한 확인 Code
        private string m_strPasswordCode = null;
        private string m_strPhoneNumber = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public int JobLevelID
        {
            get { return m_nJobLevelID; }
            set { m_nJobLevelID = value; }
        }

        // 입사일자
        public DateTime StartDate
        {
            get { return m_dtStart; }
            set { m_dtStart = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        // 팀장인가?
        public bool IsTeamLeader
        {
            get { return m_isTeamLeader; }
            set { m_isTeamLeader = value; }
        }

        // 관리자인가?
        public bool IsAdmin
        {
            get { return m_isAdmin; }
            set { m_isAdmin = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        // 사용자 암호의 Hash값
        public string Password
        {
            get { return m_strPW; }
            set { m_strPW = value; }
        }

        // 비밀번호 변경을 위한 확인 Code
        public string PasswordCode
        {
            get { return m_strPasswordCode; }
            set { m_strPasswordCode = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.UserPW ||
                field == Fields.PasswordCode ||
                field == Fields.PhoneNumber)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public static string GetTableName()
        {
            return "CompanyMember";
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 0;

            if (obj is CompanyMember)
            {
                CompanyMember member1 = this;
                CompanyMember member2 = (CompanyMember)obj;

                if (member1.JobLevelID > member2.JobLevelID)
                    return -1;
                else if (member1.JobLevelID < member2.JobLevelID)
                    return 1;
                else
                {
                    if (member1.StartDate < member2.StartDate)
                        return -1;
                    else if (member1.StartDate > member2.StartDate)
                        return 1;
                    else
                    {
                        if (member1.ID < member2.ID)
                            return -1;
                        else if (member1.ID > member2.ID)
                            return 1;
                    }
                }
            }

            return 0;
        }
    }
}
