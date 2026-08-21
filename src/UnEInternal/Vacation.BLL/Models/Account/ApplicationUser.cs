using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vacation.Model;

namespace Vacation.BLL.Models.Account
{
    [Serializable]
    public class ApplicationUser
    {
        private int m_nID = -1;
        private string m_strName = "";
        private string m_strLevel = "";
        private string m_strUserID = "";
        private int m_nTeamID = -1;
        private string m_strTeamName = "";
        private string m_strPhoneNumber = "";
        // 팀장인가?
        private bool m_isTeamLeader = false;
        // 시스템 관리자인가?
        private bool m_isAdmin = false;
        // 경영진인가?
        private bool m_isTopManager = false;
        // 입사년도
        private int m_nStartYear = 0;
        // 입사월
        private int m_nStartMonth = 0;
        // 현재 날짜로부터 몇개월 앞까지 휴가 예약이 가능한가?
        private int m_nReservationMonth = 0;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        // 사용자 이름
        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        // 직급
        public string Level
        {
            get { return m_strLevel; }
            set { m_strLevel = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public string TeamName
        {
            get { return m_strTeamName; }
            set { m_strTeamName = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        // 팀장인가?
        public bool IsTeamLeader
        {
            get { return m_isTeamLeader; }
            set { m_isTeamLeader = value; }
        }

        // 시스템 관리자인가?
        public bool IsAdmin
        {
            get { return m_isAdmin; }
            set { m_isAdmin = value; }
        }

        // 경영진인가?
        public bool IsTopManager
        {
            get { return m_isTopManager; }
            set { m_isTopManager = value; }
        }

        // 입사년도
        public int StartYear
        {
            get { return m_nStartYear; }
            set { m_nStartYear = value; }
        }

        // 입사월
        public int StartMonth
        {
            get { return m_nStartMonth; }
            set { m_nStartMonth = value; }
        }

        // 현재 날짜로부터 몇개월 앞까지 휴가 예약이 가능한가?
        public int ReservationMonth
        {
            get { return m_nReservationMonth; }
            set { m_nReservationMonth = value; }
        }

        public static bool CheckTopManager(CompanyMember member, RegularTeam team, string strLevelName)
        {
            if (strLevelName == "고문")
                return false;

            return member.IsTeamLeader && team.ParentTeamID == null;
        }

        public static ApplicationUser MakeUser(CompanyMember member, JobLevel level, RegularTeam team)
        {
            ApplicationUser user = new ApplicationUser();

            user.ID = member.ID;
            user.IsAdmin = member.IsAdmin;
            user.IsTeamLeader = member.IsTeamLeader;
            user.IsTopManager = CheckTopManager(member, team, level.LevelName);
            user.Level = level.LevelName;
            user.Name = member.Name;
            user.PhoneNumber = member.PhoneNumber;
            user.StartMonth = member.StartDate.Month;
            user.StartYear = member.StartDate.Year;
            user.TeamID = team.ID;
            user.TeamName = team.Name;
            user.UserID = member.UserID;

            return user;
        }
    }

    public class LoginResult : MessageResult
    {
        private ApplicationUser m_user = null;
        private Vacation.Options m_options = null;

        public ApplicationUser User
        {
            get { return m_user; }
            set { m_user = value; }
        }

        public Vacation.Options Options
        {
            get { return m_options; }
            set { m_options = value; }
        }

        public LoginResult()
            : base()
        {
        }

        public LoginResult(bool success, string message)
            : base(success, message)
        {
        }
    }
}
