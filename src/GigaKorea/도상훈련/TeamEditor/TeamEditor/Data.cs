using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor
{
    /// <summary>
    /// Team Class
    /// </summary>
    public class Team
    {
        private static int m_nTempID = -1;

        private int m_nTeamID = -1;
        private string m_strTeamName = "";
        private bool m_bVisible = true;

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

        public bool Visible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }

        public Team()
        {
            // 임시 ID는 계속해서 바꿔준다.
            m_nTeamID = m_nTempID--;
        }
    }

    /// <summary>
    /// 정규 조직
    /// </summary>
    public class RegularTeam : Team
    {
        private RegularTeam m_teamParent = null;

        public RegularTeam ParentTeam
        {
            get { return m_teamParent; }
            set { m_teamParent = value; }
        }

        /// <summary>
        /// 팀의 이름에 '파트'라는 글자가 포함되어있으면 파트로 간주.
        /// </summary>
        public bool IsPartTeam
        {
            get
            {
                if (TeamName.Contains("파트") == true)
                    return true;
                else
                    return false;
            }
        }

        public override string ToString()
        {
            return base.TeamName;
        }
    }

    /// <summary>
    /// 비상 조직 평일
    /// </summary>
    public class TemporaryNormalTeam : Team
    {
        private TemporaryNormalTeam m_teamParent = null;
        private List<TemporaryMember> m_members = new List<TemporaryMember>();

        public TemporaryNormalTeam ParentTeam
        {
            get { return m_teamParent; }
            set { m_teamParent = value; }
        }

        public List<TemporaryMember> Members
        {
            get { return m_members; }
        }
    }

    /// <summary>
    /// 비상 조직 휴일/야간
    /// </summary>
    public class TemporaryEmergencyTeam : Team
    {
        private TemporaryEmergencyTeam m_teamParent = null;
        private List<TemporaryMember> m_members = new List<TemporaryMember>();

        public TemporaryEmergencyTeam ParentTeam
        {
            get { return m_teamParent; }
            set { m_teamParent = value; }
        }

        public List<TemporaryMember> Members
        {
            get { return m_members; }
        }
    }

    /// <summary>
    /// 사용자 정의 팀
    /// </summary>
    public class UserDefinedTeam : Team, IComparable
    {
        private string m_strPhoneNumber = "";
        private string m_strFaxNumber = "";

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public string FaxNumber
        {
            get { return m_strFaxNumber; }
            set { m_strFaxNumber = value; }
        }


        public int CompareTo(object obj)
        {
            UserDefinedTeam team1 = this;
            UserDefinedTeam team2 = (UserDefinedTeam)obj;

            int nResult = team1.TeamID.CompareTo(team2.TeamID);

            return nResult;
        }

    }

    /// <summary>
    /// 협력 회사 / 팀
    /// </summary>
    public class ExternalTeam : Team
    {
        private ExternalTeam m_teamParent = null;
        private string m_strPhoneNumber = "";
        private string m_strFaxNumber = null;

        public ExternalTeam ParentTeam
        {
            get { return m_teamParent; }
            set { m_teamParent = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public string FaxNumber
        {
            get { return m_strFaxNumber; }
            set { m_strFaxNumber = value; }
        }
    }

    /// <summary>
    /// 정규 조직원
    /// </summary>
    public class CompanyMember : IComparable
    {
        public class JobLevelSubInfo
        {
            private int m_nID = -1;
            private string m_strName = "";
            private static List<JobLevelSubInfo> m_subLevels = new List<JobLevelSubInfo>();

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

            public JobLevelSubInfo()
            {
                m_subLevels.Add(this);
            }

            public override string ToString()
            {
                return m_strName;
            }

            public static JobLevelSubInfo GetJobSubLevel(int nID)
            {
                foreach (JobLevelSubInfo subLevel in m_subLevels)
                {
                    if (subLevel.ID == nID)
                        return subLevel;
                }

                return null;
            }

            public static JobLevelSubInfo GetJobSubLevel(string strName)
            {
                foreach (JobLevelSubInfo subLevel in m_subLevels)
                {
                    if (subLevel.Name == strName)
                        return subLevel;
                }

                return null;
            }
        }

        public class JobGroupPosition
        {
            private int m_nID = -1;
            private string m_strName = "";
            private static List<JobGroupPosition> m_groupPositions = new List<JobGroupPosition>();

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

            public JobGroupPosition()
            {
                m_groupPositions.Add(this);
            }

            public override string ToString()
            {
                return m_strName;
            }

            public static JobGroupPosition GetJobGroupPosition(int nID)
            {
                foreach (JobGroupPosition groupPosition in m_groupPositions)
                {
                    if (nID == groupPosition.ID)
                        return groupPosition;
                }

                return null;
            }

            public static JobGroupPosition GetJobGroupPosition(string strName)
            {
                foreach (JobGroupPosition groupPosition in m_groupPositions)
                {
                    if (strName == groupPosition.Name)
                        return groupPosition;
                }

                return null;
            }
        }

        public class JobPositionSubInfo
        {
            private int m_nID = -1;
            private string m_strName = "";
            private static List<JobPositionSubInfo> m_subPositions = new List<JobPositionSubInfo>();

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

            public JobPositionSubInfo()
            {
                m_subPositions.Add(this);
            }

            public override string ToString()
            {
                return m_strName;
            }

            public static JobPositionSubInfo GetSubPosition(int nID)
            {
                foreach (JobPositionSubInfo subPosition in m_subPositions)
                {
                    if (subPosition.ID == nID)
                        return subPosition;
                }

                return null;
            }

            public static JobPositionSubInfo GetSubPosition(string strName)
            {
                foreach (JobPositionSubInfo subPosition in m_subPositions)
                {
                    if (subPosition.Name == strName)
                        return subPosition;
                }

                return null;
            }
        }

        private int m_nID = -1;
        private string m_strName = "";
        private int m_nLevelID = -1;
        private int m_nPositionID = -1;
        private string m_strMemberID = "";
        private string m_strOfficePhoneNumber = "";
        private string m_strPhoneNumber = "";
        private JobLevelSubInfo m_subLevel = null;
        private JobGroupPosition m_groupPosition = null;
        private JobPositionSubInfo m_subPosition = null;
        private RegularTeam m_team = null;

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

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public int PositionID
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
        }

        public string OfficePhoneNumber
        {
            get { return m_strOfficePhoneNumber; }
            set { m_strOfficePhoneNumber = value; }
        }

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public JobLevelSubInfo SubJobLevel
        {
            get { return m_subLevel; }
            set { m_subLevel = value; }
        }

        public JobGroupPosition GroupPosition
        {
            get { return m_groupPosition; }
            set { m_groupPosition = value; }
        }

        public JobPositionSubInfo SubJobPosition
        {
            get { return m_subPosition; }
            set { m_subPosition = value; }
        }

        public RegularTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public void CopyFrom(CompanyMember member)
        {
            this.m_nID = member.m_nID;
            this.m_strName = member.m_strName;
            this.m_nLevelID = member.m_nLevelID;
            this.m_nPositionID = member.m_nPositionID;
            this.m_strMemberID = member.m_strMemberID;
            this.m_strOfficePhoneNumber = member.m_strOfficePhoneNumber;
            this.m_strPhoneNumber = member.m_strPhoneNumber;
            this.m_subLevel = member.m_subLevel;
            this.m_groupPosition = member.m_groupPosition;
            this.m_subPosition = member.m_subPosition;
            this.m_team = member.m_team;
        }

        public int CompareTo(object obj)
        {
            CompanyMember member1 = this;
            CompanyMember member2 = (CompanyMember)obj;

            string strPositionName1 = DataManager.GetJobPositionName(member1.PositionID);
            string strPositionName2 = DataManager.GetJobPositionName(member2.PositionID);

            // 팀장인지 여부에 따라 팀장을 가장 높은 우선순위로 둔다.
            int nResult = CompareJobPosition(strPositionName1, strPositionName2);

            if (nResult != 0)
                return nResult;

            // 직위가 같을 경우 직급에 따라 정렬한다.
            int nUnknownLevelID = 1000000;
            int nLevelID1 = member1.LevelID <= 0 ? nUnknownLevelID : member1.LevelID;
            int nLevelID2 = member2.LevelID <= 0 ? nUnknownLevelID : member2.LevelID;

            if (nLevelID1 < nLevelID2)
                return -1;
            else if (nLevelID1 > nLevelID2)
                return 1;

            // 직급이 같을 경우 이름순으로 정렬한다.
            nResult = member1.Name.CompareTo(member2.Name);

            if (nResult != 0)
                return nResult;

            // 이름이 같을 경우 사번순으로 정렬한다.
            return member1.MemberID.CompareTo(member2.MemberID);
        }

        private static int CompareJobPosition(string strPositionName1, string strPositionName2)
        {
            if (strPositionName1 == strPositionName2)
                return 0;

            if (strPositionName1 == null)
                return 1;
            else if (strPositionName2 == null)
                return -1;

            bool isLeader1 = strPositionName1.EndsWith("장");
            bool isLeader2 = strPositionName2.EndsWith("장");

            if (isLeader1 && isLeader2)
            {
                // 팀장이 가장 우선순위가 높고, 나머지 장들은 모두 같은 레벨로 본다.
                if (strPositionName1 == "팀장")
                    return -1;
                else if (strPositionName2 == "팀장")
                    return 1;
                else
                    return 0;
            }
            else if (isLeader1)
                return -1;
            else if (isLeader2)
                return 1;

            return 0;
        }
    }

    /// <summary>
    /// 협력사 직원
    /// </summary>
    public class ExternalCompanyMember : IComparable
    {
        public class ExternalJobLevelInfo
        {
            private int m_nID = -1;
            private string m_strName = "";
            private static List<ExternalJobLevelInfo> m_externalLevels = new List<ExternalJobLevelInfo>();

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

            public ExternalJobLevelInfo()
            {
                m_externalLevels.Add(this);
            }

            public override string ToString()
            {
                return m_strName;
            }

            public static ExternalJobLevelInfo GetExternalJobLevel(int nID)
            {
                foreach (ExternalJobLevelInfo externalLevel in m_externalLevels)
                {
                    if (externalLevel.ID == nID)
                        return externalLevel;
                }

                return null;
            }

            public static ExternalJobLevelInfo GetExternalJobLevel(string strName)
            {
                foreach (ExternalJobLevelInfo externalLevel in m_externalLevels)
                {
                    if (externalLevel.Name == strName)
                        return externalLevel;
                }

                return null;
            }
        }

        public class ExternalJobPositionInfo
        {
            private int m_nID = -1;
            private string m_strName = "";
            private static List<ExternalJobPositionInfo> m_externalJobPositions = new List<ExternalJobPositionInfo>();

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

            public ExternalJobPositionInfo()
            {
                m_externalJobPositions.Add(this);
            }

            public override string ToString()
            {
                return m_strName;
            }

            public static ExternalJobPositionInfo GetExternalJobPosition(int nID)
            {
                foreach (ExternalJobPositionInfo externalJobPosition in m_externalJobPositions)
                {
                    if (externalJobPosition.ID == nID)
                        return externalJobPosition;
                }

                return null;
            }

            public static ExternalJobPositionInfo GetExternalJobPosition(string strName)
            {
                foreach (ExternalJobPositionInfo externalJobPosition in m_externalJobPositions)
                {
                    if (externalJobPosition.Name == strName)
                        return externalJobPosition;
                }

                return null;
            }
        }

        private int m_nID = -1;
        private string m_strName = "";
        private string m_strPhoneNumber = "";
        private string m_strDescription = null;
        private ExternalJobLevelInfo m_externalJobLevel = null;
        private ExternalJobPositionInfo m_externalJobPosition = null;
        private ExternalTeam m_team = null;

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

        public string PhoneNumber
        {
            get { return m_strPhoneNumber; }
            set { m_strPhoneNumber = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public ExternalJobLevelInfo ExternalJobLevel
        {
            get { return this.m_externalJobLevel; }
            set { this.m_externalJobLevel = value; }
        }

        public ExternalJobPositionInfo ExternalJobPosition
        {
            get { return this.m_externalJobPosition; }
            set { this.m_externalJobPosition = value; }
        }
        
        public int CompareTo(object obj)
        {
            ExternalCompanyMember member1 = this;
            ExternalCompanyMember member2 = (ExternalCompanyMember)obj;

            return member1.Name.CompareTo(member2.Name);
        }

        public ExternalTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }
    }

    /// <summary>
    /// 비상 조직원
    /// </summary>
    public class TemporaryMember : IComparable
    {
        public enum ManagerType { 정 = 0, 부, GENERAL, NONE };
        public enum MemberType
        {
            RegularTeam = 0,
            CompanyMember = 1,
            ExternalTeam = 3,           // 협력회사 / 팀
            // ExternalCompanyTeam = 3, 사용 안함
            ExternalCompanyMember = 4,
            UserDefinedTeam = 5,
            LevelID = 6,
            None = 7
        };

        private int m_nID = -1;
        // 실제 Member의 이름과 상관없이 조직도 상에 나타내고자 하는 이름
        private string m_strDisplayName = "";
        private ManagerType m_managerType = ManagerType.NONE;
        private MemberType m_memberType = MemberType.None;
        private Team m_team = null;
        private CompanyMember m_companyMember = null;
        private ExternalCompanyMember m_externalCompanyMember = null;
        //private int m_nMemberID = -1;
        private int m_nLevelID = -1;
        private int m_nMemberCount = -1;
        // 하위 팀들을 포함하는가 여부
        private bool m_isIncludeChildTeam = false;
        private bool m_isTeamLeader = false;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int MemberID
        {
            get { return GetMemberID(); }
            //get { return m_nMemberID; }
            //set { m_nMemberID = value; }
        }

        // 실제 Member의 이름과 상관없이 조직도 상에 나타내고자 하는 이름
        public string DisplayName
        {
            get { return m_strDisplayName; }
            set { m_strDisplayName = value; }
        }

        public ManagerType TemporaryManagerType
        {
            get { return m_managerType; }
            set { m_managerType = value; }
        }

        public MemberType TemporaryMemberType
        {
            get { return m_memberType; }
            set { m_memberType = value; }
        }

        public Team Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public CompanyMember CompanyMember
        {
            get { return m_companyMember; }
            set { m_companyMember = value; }
        }

        public ExternalCompanyMember ExternalCompanyMember
        {
            get { return m_externalCompanyMember; }
            set { m_externalCompanyMember = value; }
        }

        public int MemberCount
        {
            get { return m_nMemberCount; }
            set { m_nMemberCount = value; }
        }

        // 하위 팀들을 포함하는가 여부
        public bool IncludeChildTeam
        {
            get { return m_isIncludeChildTeam; }
            set { m_isIncludeChildTeam = value; }
        }

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public bool IsTeamLeader
        {
            get { return m_isTeamLeader; }
            set { m_isTeamLeader = value; }
        }

        public object Member
        {
            get
            {
                object objReturn = null;

                switch (m_memberType)
                {
                    case MemberType.CompanyMember:
                        if (m_companyMember != null)
                            objReturn = m_companyMember;

                        break;

                    case MemberType.ExternalCompanyMember:
                        if (m_externalCompanyMember != null)
                            objReturn = m_externalCompanyMember;

                        break;

                    case MemberType.LevelID:

                        objReturn = m_nLevelID;

                        break;
                }

                return objReturn;
            }
        }

        public static bool ToManagerType(int nManagerType, out ManagerType type)
        {
            type = ManagerType.NONE;

            if (nManagerType < (int)ManagerType.정 || nManagerType >= (int)ManagerType.NONE)
                return false;

            type = (ManagerType)nManagerType;
            return true;
        }

        public static bool ToMemberType(int nMemberType, out MemberType type)
        {
            type = MemberType.None;

            if (nMemberType < (int)MemberType.RegularTeam || nMemberType >= (int)MemberType.None)
                return false;

            type = (MemberType)nMemberType;
            return true;
        }

        public static ManagerType ToManagerType(string strManagerType)
        {
            if (strManagerType == "정")
                return ManagerType.정;
            else if (strManagerType == "부")
                return ManagerType.부;
            else if (strManagerType == "반원")
                return ManagerType.GENERAL;

            return ManagerType.NONE;
        }

        public static string GetManagerTypeString(ManagerType type)
        {
            if (type == ManagerType.정)
                return "정";
            else if (type == ManagerType.부)
                return "부";
            else if (type == ManagerType.GENERAL)
                return "반원";

            return "";
        }

        public static string GetMemberTypeString(MemberType type)
        {
            if (type == MemberType.RegularTeam)
                return "정규조직";
            else if (type == MemberType.CompanyMember)
                return "정직원";
            else if (type == MemberType.ExternalTeam)
                return "협력업체";
            else if (type == MemberType.ExternalCompanyMember)
                return "협력업체 직원";
            else if (type == MemberType.LevelID)
                return "직급";
            else if (type == MemberType.UserDefinedTeam)
                return "기타";

            return "";
        }

        public string GetMemberRealName()
        {
            string strReturn = String.Empty;

            switch (m_memberType)
            {
                case MemberType.RegularTeam:
                    if (m_team == null)
                        break;

                    if (m_isTeamLeader == true)
                    {
                        foreach (CompanyMember member in from members in DataManager.GetRegularMembers(m_team as RegularTeam)
                                                         where members.LevelID > 0
                                                         orderby members.LevelID ascending
                                                         select members
                                                         )
                        {
                            strReturn = member.Name;
                            break;
                        }
                    }

                    break;

                case MemberType.ExternalTeam:
                    if (m_team == null)
                        break;

                    if (m_isTeamLeader == true)
                    {
                        foreach (ExternalCompanyMember member in from members in DataManager.GetExternalCompanyMembers(m_team as ExternalTeam)
                                                                 where members.ExternalJobLevel != null
                                                                 && members.ExternalJobLevel.ID > 0
                                                                 orderby members.ExternalJobLevel.ID ascending
                                                                 select members
                                                         )
                        {
                            strReturn = member.Name;
                            break;
                        }
                    }

                    break;

                case MemberType.CompanyMember:
                    if (m_companyMember == null)
                        break;

                    strReturn = m_companyMember.Name;

                    break;

                case MemberType.ExternalCompanyMember:
                    if (m_externalCompanyMember == null)
                        break;

                    strReturn = m_externalCompanyMember.Name;

                    break;

                case MemberType.LevelID:
                    if (m_nLevelID <= 0)
                        break;

                    strReturn = String.Format("{0} 전체", FormMain.Instance.GetLevelName(m_nLevelID)).Replace("급", "직급");

                    break;
            }

            return strReturn;
        }

        public string GetTeamRealName()
        {
            string strReturn = String.Empty;

            switch (m_memberType)
            {
                case MemberType.RegularTeam:
                case MemberType.CompanyMember:
                    if (m_team == null || (m_team is RegularTeam) == false)
                        break;

                    strReturn = m_team.TeamName;

                    break;

                case MemberType.ExternalTeam:
                case MemberType.ExternalCompanyMember:
                    if (m_team == null || (m_team is ExternalTeam) == false)
                        break;

                    strReturn = m_team.TeamName;

                    break;

                case MemberType.UserDefinedTeam:
                    if (m_team == null || (m_team is UserDefinedTeam) == false)
                        break;

                    strReturn = m_team.TeamName;

                    break;

            }

            return strReturn;
        }

        public int CompareTo(object obj)
        {
            TemporaryMember member1 = this;
            TemporaryMember member2 = (TemporaryMember)obj;

            if (member1.m_managerType < member2.m_managerType)
                return -1;
            else if (member1.m_managerType > member2.m_managerType)
                return 1;

            if (member1.m_isTeamLeader && !member2.m_isTeamLeader)
                return -1;
            else if (!member1.m_isTeamLeader && member2.m_isTeamLeader)
                return 1;

            string strName1 = String.Format("{0} / {1}", member1.GetTeamRealName(), member1.GetMemberRealName());
            string strName2 = String.Format("{0} / {1}", member2.GetTeamRealName(), member2.GetMemberRealName());

            return strName1.CompareTo(strName2);
        }

        private int GetMemberID()
        {
            if (m_memberType == MemberType.RegularTeam || m_memberType == MemberType.ExternalTeam || m_memberType == MemberType.UserDefinedTeam)
            {
                if (m_team == null)
                    return 0;

                if (m_isIncludeChildTeam)
                    return m_team.TeamID;

                return -m_team.TeamID;
            }
            else if (m_memberType == MemberType.CompanyMember)
            {
                if (m_companyMember == null)
                    return 0;

                return m_companyMember.ID;
            }
            else if (m_memberType == MemberType.ExternalCompanyMember)
            {
                if (m_externalCompanyMember == null)
                    return 0;

                return m_externalCompanyMember.ID;
            }
            else if (m_memberType == MemberType.LevelID)
            {
                if (m_nLevelID <= 0)
                    return 0;

                return m_nLevelID;
            }

            return 0;
        }

        public static bool IsTeamType(MemberType type)
        {
            if (type == MemberType.RegularTeam || type == MemberType.ExternalTeam || type == MemberType.UserDefinedTeam)
                return true;

            return false;
        }
    }

    /// <summary>
    /// 비상 조직 평일 구성원
    /// </summary>
    public class TemporaryNormalMember : TemporaryMember
    {
        private TemporaryNormalTeam m_teamTemporaryNormal = null;

        public TemporaryNormalTeam TemporaryTeam
        {
            get { return m_teamTemporaryNormal; }
            set { m_teamTemporaryNormal = value; }
        }
    }

    /// <summary>
    /// 비상 조직 휴일/야간 구성원
    /// </summary>
    public class TemporaryEmergencyMember : TemporaryMember
    {
        private TemporaryEmergencyTeam m_teamTemporaryEmergency = null;

        public TemporaryEmergencyTeam TemporaryTeam
        {
            get { return m_teamTemporaryEmergency; }
            set { m_teamTemporaryEmergency = value; }
        }
    }

    public class ChangedData<T>
    {
        public ChangedData()
        {
        }

        public ChangedData(T changedData, T originData)
        {
            Changed = changedData;
            Origin = originData;
        }

        public T Changed
        {
            get;
            set;
        }

        public T Origin
        {
            get;
            set;
        }
    }

    public class Tree<T>
    {
        public class Node
        {
            private T m_data;
            private List<Node> m_children = new List<Node>();
            private Node m_parent = null;

            public T Data
            {
                get { return m_data; }
                set { m_data = value; }
            }

            public List<Node> Children
            {
                get { return m_children; }
            }

            public Node Parent
            {
                get { return m_parent; }
            }

            public Node()
            {
            }

            public Node(T data)
            {
                m_data = data;
            }

            public void AddChild(Node node)
            {
                if (node == null || m_children.Contains(node))
                    return;

                m_children.Add(node);
                node.m_parent = this;
            }

            public Node AddChild(T data)
            {
                Node node = new Node(data);
                AddChild(node);
                return node;
            }

            public void RemoveNode(Node node)
            {
                m_children.Remove(node);
            }

            public void Clear()
            {
                m_children.Clear();
            }

            public Node Find(T data)
            {
                if (this.Data == null && data == null)
                    return this;
                else if (this.Data != null && this.Data.Equals(data))
                    return this;

                foreach (Node node in this.Children)
                {
                    Node result = node.Find(data);

                    if (result != null)
                        return result;
                }

                return null;
            }
        }

        private Node m_rootNode = new Node();

        public Node RootNode
        {
            get { return m_rootNode; }
            set { m_rootNode = value; }
        }

        public Tree()
        {
        }

        public Tree(T data)
        {
            m_rootNode = new Node(data);
        }

        public Node Find(T data)
        {
            return m_rootNode.Find(data);
        }
    }
}
