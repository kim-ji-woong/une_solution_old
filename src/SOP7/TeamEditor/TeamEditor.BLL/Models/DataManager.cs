using System;
using System.Collections.Generic;
using TeamEditor.Model.Sop.Team;

namespace TeamEditor.BLL.Models
{
    public class DataManager
    {
        // TeamTreeView.cs 의 TeamType 데이터
        public enum TeamType { REGULAR = 0, TEMPORARY_NORMAL, TEMPORARY_EMERGENCY };

        // 팀을 최상위 부모로부터 몇단계 떨어진 자식인지 여부를 알려주는 클래스
        // DataManager.cs 의 TeamNDepth 클래스
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

        private Dictionary<int, RegularMemberData> m_dicCompanyMembers = new Dictionary<int, RegularMemberData>();
        public Dictionary<int, RegularMemberData> DicCompanyMembers
        {
            get { return m_dicCompanyMembers; }
        }

        private Dictionary<RegularData, List<RegularMemberData>> m_dicTeamCompanyMembers = new Dictionary<RegularData, List<RegularMemberData>>();
        public Dictionary<RegularData, List<RegularMemberData>> DicTeamCompanyMembers
        {
            get { return m_dicTeamCompanyMembers; }
        }

        private Dictionary<int, RegularData> m_dicRegularTeams = new Dictionary<int, RegularData>();
        public Dictionary<int, RegularData> DicRegularTeams
        {
            get { return m_dicRegularTeams; }
        }

        private static Dictionary<TemporaryNormalData, List<TemporaryMember>> m_dicTemporaryNormalMembers = new Dictionary<TemporaryNormalData, List<TemporaryMember>>();
        public static Dictionary<TemporaryNormalData, List<TemporaryMember>> DicTemporaryNormalMembers
        {
            get { return m_dicTemporaryNormalMembers; }
        }

        private static Dictionary<TemporaryEmergencyData, List<TemporaryMember>> m_dicTemporaryEmergencyMembers = new Dictionary<TemporaryEmergencyData, List<TemporaryMember>>();
        public static Dictionary<TemporaryEmergencyData, List<TemporaryMember>> DicTemporaryEmergencyMembers
        {
            get { return m_dicTemporaryEmergencyMembers; }
        }

        private static Dictionary<int, TemporaryNormalData> m_dicTemporaryNormalTeams = new Dictionary<int, TemporaryNormalData>();
        public static Dictionary<int, TemporaryNormalData> DicTemporaryNormalTeams
        {
            get { return m_dicTemporaryNormalTeams; }
        }

        private static Dictionary<int, TemporaryEmergencyData> m_dicTemporaryEmergencyTeams = new Dictionary<int, TemporaryEmergencyData>();
        public static Dictionary<int, TemporaryEmergencyData> DicTemporaryEmergencyTeams
        {
            get { return m_dicTemporaryEmergencyTeams; }
        }

        // 평일 비상조직의 최상위 팀들
        private static List<TemporaryNormalData> m_rootNormalTeams = new List<TemporaryNormalData>();
        public static List<TemporaryNormalData> RootNormalTeams
        {
            get { return m_rootNormalTeams; }
        }

        // 야간 및 휴일 비상조직의 최상위 팀들
        private static List<TemporaryEmergencyData> m_rootEmergencyTeams = new List<TemporaryEmergencyData>();
        public static List<TemporaryEmergencyData> RootEmergencyTeams
        {
            get { return m_rootEmergencyTeams; }
        }

        // 사번 정보가 변경된 상태인지 여부를 기억한다.
        private Dictionary<RegularMemberData, bool> m_dicCompanyMemberMemberIDChanged = new Dictionary<RegularMemberData, bool>();
        public Dictionary<RegularMemberData, bool> DicCompanyMemberMemberIDChanged
        {
            get { return m_dicCompanyMemberMemberIDChanged; }
        }

        // 휴대전화번호가 변경된 상태인지 여부를 기억한다.
        private Dictionary<RegularMemberData, bool> m_dicCompanyMemberPhoneNumberChanged = new Dictionary<RegularMemberData, bool>();
        public Dictionary<RegularMemberData, bool> DicCompanyMemberPhoneNumberChanged
        {
            get { return m_dicCompanyMemberPhoneNumberChanged; }
        }

        // 근무처 전화번호가 변경된 상태인지 여부를 기억한다.
        private Dictionary<RegularMemberData, bool> m_dicCompanyMemberOfficePhoneNumberChanged = new Dictionary<RegularMemberData, bool>();
        public Dictionary<RegularMemberData, bool> DicCompanyMemberOfficePhoneNumberChanged
        {
            get { return m_dicCompanyMemberOfficePhoneNumberChanged; }
        }

        public RegularData GetRegularTeam(int nTeamID)
        {
            // 기존 DataManager.cs 의 GetRegularTeam(int nTeamID)

            RegularData team = null;
            //SaveManager.DicRegularTeams.TryGetValue(nTeamID, out team);
            m_dicRegularTeams.TryGetValue(nTeamID, out team);
            return team;
        }

        private static Dictionary<int, string> m_dicJobPositions = new Dictionary<int, string>();
        public static Dictionary<int, string> JobPositions
        {
            get { return m_dicJobPositions; }
        }

        private static Dictionary<int, string> m_dicJobLevels = new Dictionary<int, string>();
        public static Dictionary<int, string> JobLevels
        {
            get { return m_dicJobLevels; }
        }

        public List<RegularMemberData> GetRegularMembers(RegularData team)
        {
            // 기존 DataManager.cs 의 GetRegularMembers(RegularTeam team)

            if (team == null)
                return null;

            List<RegularMemberData> members = null;

            //foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in SaveManager.DicTeamCompanyMembers)
            foreach (KeyValuePair<RegularData, List<RegularMemberData>> item in m_dicTeamCompanyMembers)
            {
                if (item.Key.TeamID == team.TeamID)
                {
                    members = item.Value;
                    break;
                }
            }

            if (members == null && team != null)
            {
                members = new List<RegularMemberData>();

                //SaveManager.DicTeamCompanyMembers.Add(team, members);
                m_dicTeamCompanyMembers.Add(team, members);
            }

            return members;
        }

        /// <summary>
        /// 하위 팀에 속한 모든 Member 가져옴
        /// </summary> 
        public List<RegularMemberData> GetChildRegularMembers(RegularData team, Boolean OnlySelectTeam = false)
        {
            // 기존 DataManager.cs 의 GetChildRegularMembers(RegularTeam team, Boolean OnlySelectTeam = false)

            List<RegularMemberData> members = null;

            List<Team> childTeams = null;// GetRegularTeams(team.TeamID);  
            if (OnlySelectTeam == false)
            {
                childTeams = GetRegularTeams(team.TeamID);
            }
            else
            {
                childTeams = new List<Team>();
                RegularData childTeam = GetRegularTeam(team.TeamID);

                if (childTeam != null)
                    childTeams.Add(childTeam);
            }

            foreach (RegularData item in childTeams)
            {
                if (members == null)
                    members = new List<RegularMemberData>();

                if (m_dicTeamCompanyMembers == null || m_dicTeamCompanyMembers.ContainsKey(item) ||
                    m_dicTeamCompanyMembers[item] == null || m_dicTeamCompanyMembers[item].Count == 0) continue;

                members.AddRange(m_dicTeamCompanyMembers[item]);
            }

            if (members == null && team != null)
            {
                if (m_dicTeamCompanyMembers.TryGetValue(team, out members) == false)
                {
                    members = new List<RegularMemberData>();
                    m_dicTeamCompanyMembers[team] = members;
                }
                /*members = new List<CompanyMember>();

                if (!m_dicTeamCompanyMembers.ContainsKey(team))
                    m_dicTeamCompanyMembers.Add(team, members);*/
            }

            return members;
        }

        // nTeamID에 해당하는 팀 및 그 하위팀들 모두를 계층구조에 따라 정렬하여 리턴한다.
        public List<Team> GetRegularTeams(int nTeamID)
        {
            // 기존 DataManager.cs 의 GetRegularTeams(int nTeamID)

            return GetTeams(nTeamID, TeamData.TeamType.REGULAR);
        }

        public RegularMemberData GetCompanyMember(int nID)
        {
            // 기존 DataManager.cs 의 GetCompanyMember(int nID)

            RegularMemberData member = null;
            m_dicCompanyMembers.TryGetValue(nID, out member);
            return member;
        }

        private List<Team> GetTeams(int nTeamID, TeamData.TeamType type)
        {
            // 기존 DataManager.cs 의 GetTeams(int nTeamID, SaveManager.TeamType type)

            List<TeamData.TeamNDepth> teams = new List<TeamData.TeamNDepth>();

            Team team = GetTeam(nTeamID, type);

            if (team != null)
            {
                int nDepth = 0;
                TeamData.TeamNDepth _team = new TeamData.TeamNDepth(team, nDepth);
                teams.Add(_team);

                GetChildTeams(team, teams, nDepth + 1, type);
                teams.Sort();
            }

            List<Team> teams2 = new List<Team>();

            foreach (TeamData.TeamNDepth _team in teams)
            {
                teams2.Add(_team.Team);
            }

            return teams2;
        }

        private Team GetTeam(int nTeamID, TeamData.TeamType type)
        {
            // 기존 DataManager.cs 의 GetTeam(int nTeamID, SaveManager.TeamType type)

            if (type == TeamData.TeamType.REGULAR)
            {
                RegularData team;

                if (m_dicRegularTeams.TryGetValue(nTeamID, out team))
                    return team;
            }
            else if (type == TeamData.TeamType.TEMPORARY_NORMAL)
            {
                TemporaryNormalData team;

                if (m_dicTemporaryNormalTeams.TryGetValue(nTeamID, out team))
                    return team;
            }
            else if (type == TeamData.TeamType.TEMPORARY_EMERGENCY)
            {
                TemporaryEmergencyData team;

                if (m_dicTemporaryEmergencyTeams.TryGetValue(nTeamID, out team))
                    return team;
            }
            /*
            else if (type == SaveManager.TeamType.EXTERNAL)
            {
                ExternalTeam team;

                if (SaveManager.DicExternalTeams.TryGetValue(nTeamID, out team))
                    return team;
            }
            */

            return null;
        }

        // teamParent의 자식 팀들을 teams에 담는다.
        private void GetChildTeams(Team teamParent, List<TeamData.TeamNDepth> teams, int nDepth, TeamData.TeamType type)
        {
            // 기존 DataManager.cs 의 GetChildTeams(Team teamParent, List<SaveManager.TeamNDepth> teams, int nDepth, SaveManager.TeamType type)

            if (type == TeamData.TeamType.REGULAR)
            {
                foreach (KeyValuePair<int, RegularData> pair in m_dicRegularTeams)
                {
                    if (pair.Value.ParentTeam == null)
                        continue;

                    if (!pair.Value.Visible)
                        continue;

                    if (pair.Value.ParentTeam.TeamID == teamParent.TeamID)
                    {
                        TeamData.TeamNDepth _team = new TeamData.TeamNDepth(pair.Value, nDepth);
                        teams.Add(_team);

                        GetChildTeams(pair.Value, teams, nDepth + 1, type);
                    }
                }
            }
            else if (type == TeamData.TeamType.TEMPORARY_NORMAL)
            {
                foreach (KeyValuePair<int, TemporaryNormalData> pair in m_dicTemporaryNormalTeams)
                {
                    if (pair.Value.ParentTeam == teamParent)
                    {
                        TeamData.TeamNDepth _team = new TeamData.TeamNDepth(pair.Value, nDepth);
                        teams.Add(_team);

                        GetChildTeams(pair.Value, teams, nDepth + 1, type);
                    }
                }
            }
            else if (type == TeamData.TeamType.TEMPORARY_EMERGENCY)
            {
                foreach (KeyValuePair<int, TemporaryEmergencyData> pair in m_dicTemporaryEmergencyTeams)
                {
                    if (pair.Value.ParentTeam == teamParent)
                    {
                        TeamData.TeamNDepth _team = new TeamData.TeamNDepth(pair.Value, nDepth);
                        teams.Add(_team);

                        GetChildTeams(pair.Value, teams, nDepth + 1, type);
                    }
                }
            }
            /*
            else if (type == SaveManager.TeamType.EXTERNAL)
            {
                foreach (KeyValuePair<int, ExternalTeam> pair in SaveManager.DicExternalTeams)
                {
                    if (pair.Value.ParentTeam == teamParent)
                    {
                        SaveManager.TeamNDepth _team = new SaveManager.TeamNDepth(pair.Value, nDepth);
                        teams.Add(_team);

                        GetChildTeams(pair.Value, teams, nDepth + 1, type);
                    }
                }

            }
            */
        }

        #region 조회 Team 정렬
        public SortedDictionary<int, Team> GetTeamsSort(RegularData nTeamID, TeamData.TeamType type)
        {
            // 기존 DataManager.cs 의 GetTeamsSort(RegularTeam nTeamID, SaveManager.TeamType type)

            List<TeamData.TeamNDepth> teams = new List<TeamData.TeamNDepth>();

            Team team = nTeamID;

            if (team != null)
            {
                int sortNum = 0;
                int nDepth = 0;
                TeamData.TeamNDepth _team = new TeamData.TeamNDepth(team, nDepth);
                _team.SortNum = sortNum;
                teams.Add(_team);

                GetChildTeamsSort(team, teams, nDepth + 1, type, ref sortNum);
                teams.Sort();
            }

            SortedDictionary<int, Team> teams2 = new SortedDictionary<int, Team>();

            foreach (TeamData.TeamNDepth _team in teams)
            {
                teams2.Add(_team.SortNum, _team.Team);
            }

            return teams2;
        }

        private void GetChildTeamsSort(Team teamParent, List<TeamData.TeamNDepth> teams, int nDepth, TeamData.TeamType type, ref int sortNum)
        {
            // 기존 DataManager.cs 의 GetChildTeamsSort(Team teamParent, List<SaveManager.TeamNDepth> teams, int nDepth, SaveManager.TeamType type, ref int sortNum)

            if (type == TeamData.TeamType.REGULAR)
            {
                foreach (KeyValuePair<int, RegularData> pair in m_dicRegularTeams)
                {
                    if (pair.Value.ParentTeam == null)
                        continue;

                    if (!pair.Value.Visible)
                        continue;

                    if (pair.Value.ParentTeam.TeamID == teamParent.TeamID)
                    {
                        sortNum++;

                        TeamData.TeamNDepth _team = new TeamData.TeamNDepth(pair.Value, nDepth);
                        _team.SortNum = sortNum;
                        teams.Add(_team);

                        GetChildTeamsSort(pair.Value, teams, nDepth + 1, type, ref sortNum);
                    }
                }
            }
        }
        #endregion

        public void AddTeam(Team team)
        {
            // 기존 DataManager.cs 의 AddTeam(Team team)

            Type teamType = team.GetType();

            if (teamType == typeof(RegularData))
                m_dicRegularTeams[team.TeamID] = (RegularData)team;
            else if (teamType == typeof(TemporaryNormalData))
            {
                TemporaryNormalData normalTeam = (TemporaryNormalData)team;
                m_dicTemporaryNormalTeams[team.TeamID] = normalTeam;
            }
            else if (teamType == typeof(TemporaryEmergencyData))
            {
                TemporaryEmergencyData emergencyTeam = (TemporaryEmergencyData)team;
                m_dicTemporaryEmergencyTeams[team.TeamID] = emergencyTeam;
            }
        }
        
        public void SetRegularMembers(RegularData team, List<RegularMemberData> members = null)
        {
            if (team == null || members == null)
                members = new List<RegularMemberData>();

            m_dicTeamCompanyMembers[team] = members;
        }

        /// <summary>
        /// 팀 정보 저장후 TeamID Update 
        /// </summary> 
        public void SetRegularTeamMemberInfo(int nOrgTeamID, int nNewTeamID, RegularData orgTeam)
        {
            List<RegularMemberData> members;
            m_dicTeamCompanyMembers.TryGetValue(orgTeam, out members);

            m_dicTeamCompanyMembers.Remove(orgTeam);

            RegularData newTeam = new RegularData();
            newTeam.TeamName = orgTeam.TeamName;
            newTeam.TeamID = nNewTeamID;
            newTeam.nBeforeSaveID = nOrgTeamID;
            newTeam.ParentTeam = orgTeam.ParentTeam;

            if (members == null)
                members = new List<RegularMemberData>();
            else
                m_dicTeamCompanyMembers[newTeam] = members;

            m_dicRegularTeams.Remove(nOrgTeamID);
            m_dicRegularTeams[nNewTeamID] = newTeam;

            //FormMain.Instance.SetRegularTeamComboItems();
        }

        public static void SetTemporaryNormalMembers(TemporaryNormalData team, List<TemporaryMember> members = null)
        {
            // 기존 DataManager.cs 의 SetTemporaryNormalMembers(TemporaryNormalTeam team, List<TemporaryMember> members = null)

            if (members == null)
                members = new List<TemporaryMember>();

            m_dicTemporaryNormalMembers[team] = members;
        }

        public static void SetTemporaryEmergencyMembers(TemporaryEmergencyData team, List<TemporaryMember> members = null)
        {
            // 기존 DataManager.cs 의 SetTemporaryEmergencyMembers(TemporaryEmergencyTeam team, List<TemporaryMember> members = null)

            if (members == null)
                members = new List<TemporaryMember>();

            m_dicTemporaryEmergencyMembers[team] = members;
        }

        public static void SetTemporaryTeam(int nTeamID, Team team, bool isNormal)
        {
            if (isNormal)
                m_dicTemporaryNormalTeams[nTeamID] = (TemporaryNormalData)team;
            else
                m_dicTemporaryEmergencyTeams[nTeamID] = (TemporaryEmergencyData)team;

        }

        public void SetRegularTeam(int nTeamID, RegularData team)
        {
            m_dicRegularTeams[nTeamID] = team;
        }

        public static bool GetJobPositionID(string strPositionName, out int nPositionID)
        {
            foreach (KeyValuePair<int, string> pair in m_dicJobPositions)
            {
                if (pair.Value == strPositionName)
                {
                    nPositionID = pair.Key;
                    return true;
                }
            }

            nPositionID = -100;
            return false;
        }

        public static string GetJobPositionName(int nPositionID)
        {
            string strPositionName;

            if (!m_dicJobPositions.TryGetValue(nPositionID, out strPositionName))
                return null;

            return strPositionName;
        }

        //public ProcessCommand Undo()
        //{
        //    if (m_cmdMgr == null)
        //        return null;

        //    return m_cmdMgr.Undo();
        //}

        //public ProcessCommand Redo()
        //{
        //    if (m_cmdMgr == null)
        //        return null;

        //    return m_cmdMgr.Redo();
        //}

        public void Save()
        {
            //if (m_cmdMgr == null)
            //    return;

            //m_cmdMgr.SaveDB();
        }

        public static void ClearJobPositions()
        {
            m_dicJobPositions.Clear();
        }

        public static void AddJobPosition(int nJobPositionID, string strJobPositionValue)
        {
            m_dicJobPositions[nJobPositionID] = strJobPositionValue;
        }

        public static void ClearJobLevels()
        {
            m_dicJobLevels.Clear();
        }

        public static void AddJobLevel(int nJobLevelID, string strJobLevelValue)
        {
            m_dicJobLevels[nJobLevelID] = strJobLevelValue;
        }
    }
}
