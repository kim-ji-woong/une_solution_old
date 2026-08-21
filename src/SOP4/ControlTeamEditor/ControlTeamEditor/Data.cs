using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlTeamEditor
{
    // 근무조(DataControlTeam)를 이루는 각 팀원들
    public class DataControlTeamMember
    {
        public enum ControlMemberType { None = 0, RegularMember = 1, ExternalMember = 4 };

        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private DataControlTeam m_Team = null;
        public DataControlTeam Team
        {
            get { return m_Team; }
            set { m_Team = value; }
        }

        private DataControlRoom m_Room = null;
        public DataControlRoom Room
        {
            get { return m_Room; }
            set { m_Room = value; }
        }

        private DataControlTeamJobPosition m_JobPosition = null;
        public DataControlTeamJobPosition JobPosition
        {
            get { return m_JobPosition; }
            set { m_JobPosition = value; }
        }

        private DataCompanyMember m_Member = null;
        public DataCompanyMember Member
        {
            get { return m_Member; }
            set { m_Member = value; }
        }

        private ControlMemberType m_memberType = ControlMemberType.RegularMember;
        public ControlMemberType MemberType
        {
            get { return m_memberType; }
            set { m_memberType = value; }
        }

        private string m_szDescritpion = "";
        public string Descritpion
        {
            get { return m_szDescritpion; }
            set { m_szDescritpion = value; }
        }

        public static ControlMemberType ToMemberType(int nMemberType)
        {
            if (nMemberType == (int)ControlMemberType.RegularMember)
                return ControlMemberType.RegularMember;
            else if (nMemberType == (int)ControlMemberType.ExternalMember)
                return ControlMemberType.ExternalMember;

            return ControlMemberType.None;
        }
    }

    // DataControlRoom별 현재 근무조
    public class DataControlWorkingTeam
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private DataControlTeam m_Team = null;
        public DataControlTeam Team
        {
            get { return m_Team; }
            set { m_Team = value; }
        }

        private DataControlRoom m_Room = null;
        public DataControlRoom Room
        {
            get { return m_Room; }
            set { m_Room = value; }
        }

        private string m_szDescritpion = "";
        public string Descritpion
        {
            get { return m_szDescritpion; }
            set { m_szDescritpion = value; }
        }

    }

    // 근무조원(DataControlTeamMember)별로 맡은 임무
    public class DataControlTeamJobPosition
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_szJobName = "";
        public string JobName
        {
            get { return m_szJobName; }
            set { m_szJobName = value; }
        }

        private DataControlRoomType m_roomType = null;
        public DataControlRoomType RoomType
        {
            get { return m_roomType; }
            set { m_roomType = value; }
        }

        private string m_szDescritpion = "";
        public string Descritpion
        {
            get { return m_szDescritpion; }
            set { m_szDescritpion = value; }
        }
    }

    // 근무조(A조, B조, C조...)
    public class DataControlTeam
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private DataControlRoomType m_roomType = null;
        public DataControlRoomType RoomType
        {
            get { return m_roomType; }
            set { m_roomType = value; }
        }

        private string m_szTeamName = "";
        public string TeamName
        {
            get { return m_szTeamName; }
            set { m_szTeamName = value; }
        }

        private string m_szDisplayText = "";
        public string DisplayText
        {
            get { return m_szDisplayText; }
            set { m_szDisplayText = value; }

        }

        private string m_szDescritpion = "";
        public string Descritpion
        {
            get { return m_szDescritpion; }
            set { m_szDescritpion = value; }
        }

        public override string ToString()
        {
            return m_szDisplayText;
        }
    }

    // 근무조(DataControlTeam)가 근무할 장소
    public class DataControlRoom
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_szLocationName = "";
        public string LocationName
        {
            get { return m_szLocationName; }
            set { m_szLocationName = value; }
        }

        private string m_szDisplayText = "";
        public string DisplayText
        {
            get { return m_szDisplayText; }
            set { m_szDisplayText = value; }

        }

        private DataControlRoomType m_roomType = null;
        public DataControlRoomType RoomType
        {
            get { return m_roomType; }
            set { m_roomType = value; }
        }

        private string m_szDescritpion = "";
        public string Descritpion
        {
            get { return m_szDescritpion; }
            set { m_szDescritpion = value; }
        }
    }

    public class DataTeam
    {
        private int m_nID = -1;
        private string m_szTeamName = "";
        private DataTeam m_teamParent = null;
        private bool m_bExternal = false;
        private ArrayList m_arrChildTeams = new ArrayList();
        private string m_strCompanyName = "";
        private bool m_isCompany = false;

        public bool External
        {
            get { return m_bExternal; }
            set { m_bExternal = value; }
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string TeamName
        {
            get { return m_szTeamName; }
            set { m_szTeamName = value; }
        }

        public DataTeam ParentTeam
        {
            get { return m_teamParent; }
            set
            {
                if (m_teamParent != null)
                    m_teamParent.RemoveChild(this);

                m_teamParent = value;

                if (m_teamParent != null)
                    m_teamParent.AddChild(this);
            }
        }

        public ArrayList ChildTeams
        {
            get { return m_arrChildTeams; }
        }

        public string CompanyName
        {
            get { return m_strCompanyName; }
            set { m_strCompanyName = value; }
        }

        // Team이 아닌 Company인가?
        public bool IsCompany
        {
            get { return m_isCompany; }
            set { m_isCompany = value; }
        }

        protected void RemoveChild(DataTeam team)
        {
            if (team != null)
                m_arrChildTeams.Remove(team);
        }

        protected void AddChild(DataTeam team)
        {
            if (!m_arrChildTeams.Contains(team))
                m_arrChildTeams.Add(team);
        }

        public override string ToString()
        {
            return m_szTeamName;
        }
    }

    public class JobPosition
    {
        public enum PositionType
        {
            UNKNOWN = 0,
            TEAM_MEMBER = 1,
            TEAM_LEADER = 2,
            PART_LEADER = 3,
            CENTER_LEADER = 4,
            실장 = 5,
            처장 = 6,
            본부장 = 7,
            MATERNITY_LEAVE = 100,
            MILITARY_LEAVE = 101,
            ETC_LEAVE = 102
        };

        private static Dictionary<int, PositionType> m_dicPositionType = null;

        private int m_nPositionID = -1;
        private string m_strSubPositionName = "";

        public int PositionID
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }

        public string SubPositionName
        {
            get { return m_strSubPositionName; }
            set { m_strSubPositionName = value; }
        }

        public JobPosition()
        {
        }

        public JobPosition(int nPositionID, string strSubPositionName)
        {
            m_nPositionID = nPositionID;
            m_strSubPositionName = strSubPositionName;
        }

        // Return 값 : 1(nPosition1이 우선순위가 높다.)
        //              0(두 우선순위가 같다.)
        //             -1(nPosition2가 우선순위가 높다.)
        public static int CompareJobPosition(int nPosition1, int nPosition2)
        {
            PositionType pos1 = ToPositionType(nPosition1);
            PositionType pos2 = ToPositionType(nPosition2);

            if (pos1 == pos2)
                return 0;

            if (pos1 == PositionType.UNKNOWN)
            {
                if (pos2 != PositionType.MATERNITY_LEAVE && pos2 != PositionType.MILITARY_LEAVE && pos2 != PositionType.ETC_LEAVE)
                    return -1;
                else
                    return 1;
            }
            else if (pos1 == PositionType.TEAM_MEMBER)
            {
                if (pos2 == PositionType.MATERNITY_LEAVE || pos2 == PositionType.MILITARY_LEAVE || pos2 == PositionType.ETC_LEAVE)
                    return 1;
                else if (pos2 == PositionType.UNKNOWN)
                    return 1;
                else
                    return -1;
            }
            else if (pos1 == PositionType.TEAM_LEADER)
            {
                if (pos2 == PositionType.실장)
                    return 0;
                else if (pos2 == PositionType.본부장 || pos2 == PositionType.처장)
                    return -1;
                else
                    return 1;
            }
            else if (pos1 == PositionType.PART_LEADER)
            {
                if (pos2 == PositionType.TEAM_LEADER || pos2 == PositionType.실장 || pos2 == PositionType.본부장 || pos2 == PositionType.처장)
                    return -1;
                else
                    return 1;
            }
            else if (pos1 == PositionType.CENTER_LEADER)
            {
                if (pos2 == PositionType.TEAM_LEADER || pos2 == PositionType.실장 || pos2 == PositionType.PART_LEADER || pos2 == PositionType.본부장 || pos2 == PositionType.처장)
                    return -1;
                else
                    return 1;
            }
            else if (pos1 == PositionType.실장)
            {
                if (pos2 == PositionType.TEAM_LEADER)
                    return 0;
                else if (pos2 == PositionType.본부장 || pos2 == PositionType.처장)
                    return -1;
                else
                    return 1;
            }
            else if (pos1 == PositionType.처장)
            {
                if (pos2 == PositionType.본부장)
                    return -1;
                else
                    return 1;
            }
            else if (pos1 == PositionType.본부장)
            {
                return 1;
            }
            // pos1은 휴직중...
            else
            {
                if (pos2 == PositionType.MATERNITY_LEAVE || pos2 == PositionType.MILITARY_LEAVE || pos2 == PositionType.ETC_LEAVE)
                    return 0;
                else
                    return -1;
            }

            return 0;
        }

        public static PositionType ToPositionType(int nPosition)
        {
            if (m_dicPositionType == null)
            {
                m_dicPositionType = new Dictionary<int, PositionType>();

                foreach (PositionType type in Enum.GetValues(typeof(PositionType)))
                {
                    m_dicPositionType[(int)type] = type;
                }
            }

            PositionType pType;

            if (m_dicPositionType.TryGetValue(nPosition, out pType))
                return pType;
            /*if (nPosition == (int)PositionType.TEAM_MEMBER)
                return PositionType.TEAM_MEMBER;
            else if (nPosition == (int)PositionType.TEAM_LEADER)
                return PositionType.TEAM_LEADER;
            else if (nPosition == (int)PositionType.PART_LEADER)
                return PositionType.PART_LEADER;
            else if (nPosition == (int)PositionType.CENTER_LEADER)
                return PositionType.CENTER_LEADER;
            else if (nPosition == (int)PositionType.MATERNITY_LEAVE)
                return PositionType.MATERNITY_LEAVE;
            else if (nPosition == (int)PositionType.MILITARY_LEAVE)
                return PositionType.MILITARY_LEAVE;
            else if (nPosition == (int)PositionType.ETC_LEAVE)
                return PositionType.ETC_LEAVE;*/
            
            return PositionType.UNKNOWN;
        }
    }

    public class DataCompanyMember : IComparable
    {
        private int m_nID = -1;
        private string m_strMemberName = "";
        //private DataTeam m_team = null;
        private int m_nLevelID = -1;
        //private int m_nPositionID = -1;
        private string m_strMemberID = "";
        private string m_strPhoneNumber = "";
        private string m_strOfficePhoneNumber = "";
        private Dictionary<DataTeam, JobPosition> m_dicTeamPositions = new Dictionary<DataTeam, JobPosition>();

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

        /*public DataTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }*/

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        /*public int PositionID
        {
            get { return m_nPositionID; }
            set { m_nPositionID = value; }
        }*/

        public string MemberID
        {
            get { return m_strMemberID; }
            set { m_strMemberID = value; }
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

        public Dictionary<DataTeam, JobPosition> TeamPositions
        {
            get { return m_dicTeamPositions; }
        }

        /*public bool IsTeamLeader
        {
            get { return m_nPositionID == 2; }
        }*/

        public int GetFirstTeamPosition()
        {
            foreach (KeyValuePair<DataTeam, JobPosition> pair in m_dicTeamPositions)
            {
                return pair.Value.PositionID;
            }

            return -1;
        }

        public DataTeam GetFirstTeam()
        {
            foreach (KeyValuePair<DataTeam, JobPosition> pair in m_dicTeamPositions)
            {
                return pair.Key;
            }

            return null;
        }

        public bool IsTeamLeader(DataTeam team)
        {
            JobPosition position;

            if (m_dicTeamPositions.TryGetValue(team, out position))
            {
                return position.PositionID == 2;
            }

            return false;
        }

        public int CompareTo(object obj)
        {
            DataCompanyMember member = (DataCompanyMember)obj;
            int nPosition = this.GetFirstTeamPosition();

            if (nPosition != member.GetFirstTeamPosition())
                return nPosition == 2 ? -1 : 1;

            if (this.m_nLevelID > member.m_nLevelID)
                return 1;
            else if (this.m_nLevelID < member.m_nLevelID)
                return -1;

            return this.m_strMemberID.CompareTo(member.m_strMemberID);
        }

        public override string ToString()
        {
            return m_strMemberName;
        }
    }

    public class DataControlRoomType
    {
        private int m_nID = -1;
        private string m_strRoomType = "";
        private string m_strDescription = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string RoomType
        {
            get { return m_strRoomType; }
            set { m_strRoomType = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public DataControlRoomType()
        {
        }

        public DataControlRoomType(int nID, string strRoomType)
        {
            m_nID = nID;
            m_strRoomType = strRoomType;
        }

        public DataControlRoomType(int nID, string strRoomType, string strDescription)
        {
            m_nID = nID;
            m_strRoomType = strRoomType;
            m_strDescription = strDescription;
        }
    }
}
