using SensorMaker.BLL.Models.Response;
using System;
using System.Collections.Generic;
using TeamEditor.Model.Sop.Team;

namespace SensorMaker.BLL.Models.Account
{
    [Serializable]
    public class ApplicationUser
    {
        public enum UserType { Normal = 0, Developer, Administrator };
        public enum UserStatus { Normal = 0, NotConfirmed };

        public const int NormalLevel = 1;
        public const int DeveloperLevel = 2;
        public const int AdminLevel = 3;

        public const string NormalLevelName = "일반 사용자";
        public const string DeveloperLevelName = "개발자";
        public const string AdminLevelName = "시스템 관리자";

        public const string SystemTeamName = "시스템 개발팀(임시)";

        private int m_nID = -1;
        private string m_strName = "";
        private UserType m_userType = UserType.Normal;
        private UserStatus m_status = UserStatus.NotConfirmed;
        private string m_strEmail = "";
        private string m_strPhoneNumber = "";
        private DateTime m_dtCreate = new DateTime();

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

        public bool IsNormalUser
        {
            get { return m_userType == UserType.Normal; }
        }

        public bool IsDeveloper
        {
            get { return m_userType == UserType.Developer; }
        }

        public bool IsAdmin
        {
            get { return m_userType == UserType.Administrator; }
        }

        public string Email
        {
            get { return m_strEmail; }
            set { m_strEmail = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public DateTime CreateTime
        {
            get { return m_dtCreate; }
            set { m_dtCreate = value; }
        }

        public UserStatus GetStatus()
        {
            return m_status;
        }

        public void SetStatus(UserStatus status)
        {
            m_status = status;
        }
        /*public UserStatus Status
        {
            get { return m_status; }
            set { m_status = value; }
        }*/

        public void SetUserType(UserType type)
        {
            m_userType = type;
        }

        public static int GetStatusID(UserStatus status, UserType type)
        {
            int nStatus = ((int)status) * 100;
            int nUserType = (int)type;
            return nStatus + nUserType;
        }

        public static void FromStatusID(int nStatusID, out UserStatus status, out UserType type)
        {
            status = (UserStatus)(nStatusID / 100);
            type = (UserType)(nStatusID % 100);
        }

        public static ApplicationUser FromRegularMember(RegularMember member, DateTime dtCreate)
        {
            ApplicationUser user = new ApplicationUser();

            user.Email = member.Email;
            user.ID = member.ID;
            user.Name = member.MemberName;
            user.PhoneNumber = member.PhoneNumber;
            user.CreateTime = dtCreate;

            UserStatus status;
            UserType type;
            FromStatusID(member.StatusID, out status, out type);

            user.SetStatus(status);
            user.SetUserType(type);
            return user;
        }
    }

    public class LoginResult : MessageResult
    {
        private ApplicationUser m_user = null;
        private List<ApplicationUser> m_requestUsers = null;
        private GltfOption m_options = null;

        public ApplicationUser User
        {
            get { return m_user; }
            set { m_user = value; }
        }

        public List<ApplicationUser> RequestUsers
        {
            get { return m_requestUsers; }
            set { m_requestUsers = value; }
        }

        public GltfOption Options
        {
            get { return m_options; }
            set { m_options = value; }
        }

        public LoginResult()
        {
        }

        public LoginResult(bool success, string strMessage)
            : base(success, strMessage)
        {
        }
    }
}
