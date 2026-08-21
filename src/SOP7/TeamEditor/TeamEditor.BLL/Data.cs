using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamEditor.BLL;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL
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

        private Team m_teamParent = null;
        public Team ParentTeam
        {
            get { return m_teamParent; }
            set { m_teamParent = value; }
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
    public class RegularData : Team
    {
        private Regular m_regular = null;
        private RegularData m_teamParent = null;

        public Regular Regular
        {
            get { return m_regular; }
            set { m_regular = value; }
        }
        //public RegularData ParentTeam
        //{
        //    get { return m_teamParent; }
        //    set { m_teamParent = value; }
        //}

        private int m_nBeforeSaveID = 0;
        public int nBeforeSaveID
        {
            get { return m_nBeforeSaveID; }
            set { m_nBeforeSaveID = value; }
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
    public class TemporaryNormalData : Team
    {
        private TemporaryNormalData m_teamParent = null;
        private List<TemporaryMemberData> m_members = new List<TemporaryMemberData>();

        //public TemporaryNormalData ParentTeam
        //{
        //    get { return m_teamParent; }
        //    set { m_teamParent = value; }
        //}

        public List<TemporaryMemberData> Members
        {
            get { return m_members; }
        }
    }

    /// <summary>
    /// 비상 조직 휴일/야간
    /// </summary>
    public class TemporaryEmergencyData : Team
    {
        private TemporaryEmergencyData m_teamParent = null;
        private List<TemporaryMemberData> m_members = new List<TemporaryMemberData>();

        //public TemporaryEmergencyData ParentTeam
        //{
        //    get { return m_teamParent; }
        //    set { m_teamParent = value; }
        //}

        public List<TemporaryMemberData> Members
        {
            get { return m_members; }
        }
    }

    public class RegularMemberData : IComparable
    {
        private TeamEditor.Model.Sop.Team.RegularMember m_regularMember = null;
        private RegularData m_team = null;

        public RegularData Team
        {
            get { return m_team; }
            set { m_team = value; }
        }
        public TeamEditor.Model.Sop.Team.RegularMember regularMember
        {
            get { return m_regularMember; }
            set { m_regularMember = value; }
        }

        public void CopyFrom(RegularData regular, RegularMemberData member)
        {
            m_regularMember = member.regularMember;
            this.m_team = regular;
        }

        public int CompareTo(object obj)
        {
            // TODO : 정렬
            /*
            CompanyMember member1 = this;
            CompanyMember member2 = (CompanyMember)obj;

            //string strPositionName1 = DataManager.GetJobPositionName(member1.PositionID);
            //string strPositionName2 = DataManager.GetJobPositionName(member2.PositionID);
            string strPositionName1 = "";
            string strPositionName2 = "";

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
            */
            return -1;
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
    /// 비상 조직원
    /// </summary>
    public class TemporaryMemberData : IComparable
    {
        public enum ManagerType { 정 = 0, 부, GENERAL, NONE };
        public enum MemberType
        {
            RegularTeam = 0,
            RegularMember = 1,
            ExternalTeam = 3,           // 협력회사 / 팀
            // ExternalCompanyTeam = 3, 사용 안함
            ExternalRegularMember = 4,
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
        private RegularMemberData m_regularMember = null;
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

        public RegularMemberData RegularMember
        {
            get { return m_regularMember; }
            set { m_regularMember = value; }
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
                    case MemberType.RegularMember:
                        if (m_regularMember != null)
                            objReturn = m_regularMember;

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
            else if (type == MemberType.RegularMember)
                return "정직원";
            else if (type == MemberType.ExternalTeam)
                return "협력업체";
            else if (type == MemberType.ExternalRegularMember)
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
                case MemberType.RegularMember:
                    if (m_regularMember == null)
                        break;

                    strReturn = m_regularMember.regularMember.MemberName;

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
                case MemberType.RegularMember:
                    if (m_team == null || (m_team is RegularData) == false)
                        break;

                    strReturn = m_team.TeamName;

                    break;
            }

            return strReturn;
        }

        public int CompareTo(object obj)
        {
            TemporaryMemberData member1 = this;
            TemporaryMemberData member2 = (TemporaryMemberData)obj;

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
            else if (m_memberType == MemberType.RegularMember)
            {
                if (m_regularMember == null)
                    return 0;

                return m_regularMember.regularMember.ID;
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
    public class TemporaryNormalMember : TemporaryMemberData
    {
        private TemporaryNormalData m_teamTemporaryNormal = null;

        public TemporaryNormalData TemporaryTeam
        {
            get { return m_teamTemporaryNormal; }
            set { m_teamTemporaryNormal = value; }
        }
    }

    /// <summary>
    /// 비상 조직 휴일/야간 구성원
    /// </summary>
    public class TemporaryEmergencyMember : TemporaryMemberData
    {
        private TemporaryEmergencyData m_teamTemporaryEmergency = null;

        public TemporaryEmergencyData TemporaryTeam
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

    public class TeamData
    {
        public enum TeamType { REGULAR = 0, TEMPORARY_NORMAL, TEMPORARY_EMERGENCY, EXTERNAL };

        public class TeamNDepth : IComparable
        {
            private Team m_team = null;
            private int m_nDepth = -1;
            private int m_nSortNum = -1;

            public Team Team
            {
                get { return m_team; }
                set { m_team = value; }
            }

            public int Depth
            {
                get { return m_nDepth; }
                set { m_nDepth = value; }
            }

            public int SortNum
            {
                get { return m_nSortNum; }
                set { m_nSortNum = value; }
            }

            public TeamNDepth()
            {
            }

            public TeamNDepth(Team team, int nDepth)
            {
                m_team = team;
                m_nDepth = nDepth;
            }

            public int CompareTo(object obj)
            {
                TeamNDepth team1 = this;
                TeamNDepth team2 = (TeamNDepth)obj;

                if (team1.Depth < 0 && team2.Depth < 0)
                    return 0;

                return team1.Depth < team2.Depth ? -1 : 1;
            }
        }
    }

    public class RegularTeam : Regular
    {
        private List<RegularTeam> m_children = new List<RegularTeam>();
        //private RegularTeam m_parentTeam = null;
        private string m_strPath = "";

        //public RegularTeam ParentTeam
        //{
        //    get { return m_parentTeam; }
        //    set
        //    {
        //        if (m_parentTeam != value)
        //        {
        //            if (m_parentTeam != null)
        //                m_parentTeam.m_children.Remove(this);

        //            if (value != null)
        //                value.m_children.Add(this);

        //            m_parentTeam = value;
        //        }
        //    }
        //}

        public string Path
        {
            get { return m_strPath; }
            set { m_strPath = value; }
        }

        public List<RegularTeam> Children
        {
            get { return m_children; }
            set { m_children = value; }
        }

        // 하위 팀을 포함하여 팀원이 한명이라도 존재하는가?
        //public bool HasChildMembers(Dictionary<RegularTeam, List<RegularMember>> dicTeamMembers)
        //{
        //    if (HasChildMembers(this, dicTeamMembers))
        //        return true;

        //    return false;
        //}

        //private bool HasChildMembers(RegularTeam team, Dictionary<RegularTeam, List<RegularMember>> dicTeamMembers)
        //{
        //    List<RegularMember> members;

        //    if (dicTeamMembers.TryGetValue(team, out members) && members.Count > 0)
        //        return true;

        //    foreach (RegularTeam childTeam in m_children)
        //    {
        //        if (HasChildMembers(childTeam, dicTeamMembers))
        //            return true;
        //    }

        //    return false;
        //}
    }
}
