using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;

namespace TeamEditor
{
    public static class DataManager
    {
        // 팀을 최상위 부모로부터 몇단계 떨어진 자식인지 여부를 알려주는 클래스
        private class TeamNDepth : IComparable
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

        private static Dictionary<int, string> m_dicJobPositions = new Dictionary<int, string>();
        public static Dictionary<int, string> JobPositions
        {
            get { return m_dicJobPositions; }
        } 
        private static Dictionary<int, RegularTeam> m_dicRegularTeams = new Dictionary<int, RegularTeam>();
        public static Dictionary<int, RegularTeam> DicRegularTeams
        {
            get { return m_dicRegularTeams; }
        }
        private static int m_nNoSaveTeamID = -1;
        public static int NoSaveTeamID()
        {
            int teamID = m_nNoSaveTeamID;
            m_nNoSaveTeamID--;
            return teamID; 
        }
        private static int m_nNoSaveMemberID = -1;
        public static int NoSaveMemberID()
        {
            int memberID = m_nNoSaveMemberID;
            m_nNoSaveMemberID--;
            return memberID; 
        }
        private static Dictionary<RegularTeam, List<CompanyMember>> m_dicTeamCompanyMembers = new Dictionary<RegularTeam, List<CompanyMember>>();
        public static Dictionary<RegularTeam, List<CompanyMember>> DicTeamCompanyMembers
        {
            get { return m_dicTeamCompanyMembers; }
        }
        private static Dictionary<int, CompanyMember> m_dicCompanyMembers = new Dictionary<int, CompanyMember>();
        public static Dictionary<int, CompanyMember> DicCompanyMembers
        {
            get { return m_dicCompanyMembers; }
        }

        public static Dictionary<int, int> DicSaveRegularTeamIDs { get; set; }
        public static Dictionary<int, int> DicSaveRegularMemberIDs { get; set; }

        //private static Dictionary<int, UserDefinedTeam> m_dicUserDefinedTeams = new Dictionary<int, UserDefinedTeam>();
        private static List<UserDefinedTeam> m_UserDefinedTeams = new List<UserDefinedTeam>();

        private static Dictionary<int, TemporaryNormalTeam> m_dicTemporaryNormalTeams = new Dictionary<int, TemporaryNormalTeam>();
        private static Dictionary<int, TemporaryEmergencyTeam> m_dicTemporaryEmergencyTeams = new Dictionary<int, TemporaryEmergencyTeam>();
        private static Dictionary<TemporaryNormalTeam, List<TemporaryMember>> m_dicTemporaryNormalMembers = new Dictionary<TemporaryNormalTeam, List<TemporaryMember>>();
        private static Dictionary<TemporaryEmergencyTeam, List<TemporaryMember>> m_dicTemporaryEmergencyMembers = new Dictionary<TemporaryEmergencyTeam, List<TemporaryMember>>();
        // 협력업체 / 팀
        private static Dictionary<int, ExternalTeam> m_dicExternalTeams = new Dictionary<int, ExternalTeam>();
        public static Dictionary<int, ExternalTeam> DicExternalTeams
        {
            get { return m_dicExternalTeams; }
        }
        private static Dictionary<int, ExternalCompanyMember> m_dicExternalCompanyMembers = new Dictionary<int, ExternalCompanyMember>();
        private static Dictionary<ExternalTeam, List<ExternalCompanyMember>> m_dicExternalCompanyTeamMembers = new Dictionary<ExternalTeam, List<ExternalCompanyMember>>();
        // 평일 비상조직의 최상위 팀들
        private static List<TemporaryNormalTeam> m_rootNormalTeams = new List<TemporaryNormalTeam>();
        // 야간 및 휴일 비상조직의 최상위 팀들
        private static List<TemporaryEmergencyTeam> m_rootEmergencyTeams = new List<TemporaryEmergencyTeam>();

        // 사번 정보가 변경된 상태인지 여부를 기억한다.
        private static Dictionary<CompanyMember, bool> m_dicCompanyMemberMemberIDChanged = new Dictionary<CompanyMember, bool>();
        // 휴대전화번호가 변경된 상태인지 여부를 기억한다.
        private static Dictionary<CompanyMember, bool> m_dicCompanyMemberPhoneNumberChanged = new Dictionary<CompanyMember, bool>();
        // 근무처 전화번호가 변경된 상태인지 여부를 기억한다.
        private static Dictionary<CompanyMember, bool> m_dicCompanyMemberOfficePhoneNumberChanged = new Dictionary<CompanyMember, bool>();
        // 협력업체 직원의 휴대전화번호가 변경된 상태인지 여부를 기억한다.
        private static Dictionary<ExternalCompanyMember, bool> m_dicExternalCompanyMemberPhoneNumberChanged = new Dictionary<ExternalCompanyMember, bool>();
         
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
         
        public static bool InitData(WebDBManager dbMgr, int nSiteID)
        {
            if (!LoadJobPosition(dbMgr))
                return false;

            if (!LoadUserDefinedTeam(dbMgr, nSiteID))
                return false;

            return true;
        }

        private static bool LoadUserDefinedTeam(WebDBManager dbMgr, int nSiteID)
        {
            string strSQL = "select ID, TeamName, PhoneNumber, FaxNumber from UserDefinedTeam where SiteID = " + nSiteID.ToString();
            ArrayList arrResults = dbMgr.GetResultData(strSQL, 0);

            if (arrResults == null)
                return false;

            int nResultCount = arrResults.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResults[i + 1], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResults[i + 2], "");
                string strFaxNumber = WebDBManager.GetStringField(arrResults[i + 3], "");

                if (strFaxNumber.ToUpper() == "NULL") 
                    strFaxNumber = "";

                UserDefinedTeam team = new UserDefinedTeam();

                team.TeamID = nID;
                team.TeamName = strTeamName;
                team.PhoneNumber = strPhoneNumber;
                team.FaxNumber = strFaxNumber;

                //m_dicUserDefinedTeams[nID] = team;
                if (m_UserDefinedTeams.Exists(VAL => VAL.TeamID == nID) == false)
                {
                    m_UserDefinedTeams.Add(team);
                }
            }

            return true;
        }

        private static bool LoadJobPosition(WebDBManager dbMgr)
        {
            string strSQL = "select ID, PositionName from JobPosition";
            ArrayList arrResults = dbMgr.GetResultData(strSQL, 0);

            if (arrResults == null)
                return false;

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return false;

            m_dicJobPositions.Clear();

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                string strPositionName = WebDBManager.GetStringField(arrResults[i + 1], "");

                if (nID < 0)
                    continue;

                m_dicJobPositions[nID] = strPositionName;
            }

            // DB에 저장되지 않고 팀에디터 내에서만 사용되는 항목 (키값을 음수로 줌.. 팀장과 동급으로 간주하되 표현만 다르게 함.)
            /*m_dicJobPositions[-1] = "실장";
            m_dicJobPositions[-2] = "처장";
            m_dicJobPositions[-3] = "본부장";*/

            return true;
        }

        public static string GetJobPositionName(int nPositionID)
        {
            string strPositionName;

            if (!m_dicJobPositions.TryGetValue(nPositionID, out strPositionName))
                return null;

            return strPositionName;
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

        public static void ClearRegularTeams()
        {
            m_dicRegularTeams.Clear();
        }

        public static void AddTeam(Team team)
        {
            Type teamType = team.GetType();

            if (teamType == typeof(RegularTeam))
                m_dicRegularTeams[team.TeamID] = (RegularTeam)team;
            else if (teamType == typeof(TemporaryNormalTeam))
            {
                TemporaryNormalTeam normalTeam = (TemporaryNormalTeam)team;
                m_dicTemporaryNormalTeams[team.TeamID] = normalTeam;
            }
            else if (teamType == typeof(TemporaryEmergencyTeam))
            {
                TemporaryEmergencyTeam emergencyTeam = (TemporaryEmergencyTeam)team;
                m_dicTemporaryEmergencyTeams[team.TeamID] = emergencyTeam;
            }
            else if (teamType == typeof(ExternalTeam))
            {
                m_dicExternalTeams[team.TeamID] = (ExternalTeam)team;
            }
            else if (teamType == typeof(UserDefinedTeam))
            {
                m_UserDefinedTeams.Add((UserDefinedTeam)team);
            }
        }

        public static void SetTemporaryRootTeams(bool isNormal)
        {
            if (isNormal)
            {
                m_rootNormalTeams.Clear();

                foreach (KeyValuePair<int, TemporaryNormalTeam> pair in m_dicTemporaryNormalTeams)
                {
                    if (pair.Value.ParentTeam == null)
                    {
                        if (!m_rootNormalTeams.Contains(pair.Value))
                            m_rootNormalTeams.Add(pair.Value);
                    }
                }
            }
            else
            {
                m_rootEmergencyTeams.Clear();

                foreach (KeyValuePair<int, TemporaryEmergencyTeam> pair in m_dicTemporaryEmergencyTeams)
                {
                    if (pair.Value.ParentTeam == null)
                    {
                        if (!m_rootEmergencyTeams.Contains(pair.Value))
                            m_rootEmergencyTeams.Add(pair.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 팀 정보 저장후 TeamID Update 
        /// </summary> 
        public static void SetRegularTeamMemberInfo(int nOrgTeamID, int nNewTeamID, RegularTeam orgTeam)
        {
            List<CompanyMember> members;
            m_dicTeamCompanyMembers.TryGetValue(orgTeam, out members);

            m_dicTeamCompanyMembers.Remove(orgTeam);

            RegularTeam newTeam = new RegularTeam();
            newTeam.TeamName = orgTeam.TeamName;
            newTeam.TeamID = nNewTeamID; 
            newTeam.ParentTeam = orgTeam.ParentTeam;

            if (members == null)
                members = new List<CompanyMember>();
            else
                m_dicTeamCompanyMembers[newTeam] = members;

            m_dicRegularTeams.Remove(nOrgTeamID);
            m_dicRegularTeams[nNewTeamID] = newTeam;

            FormMain.Instance.SetRegularTeamComboItems();
        }

        /// <summary>
        /// Member ID Update
        /// </summary> 
        public static void SetRegularMemberID(CompanyMember member, int newMemberID)
        {
            CompanyMember orgMember = null;
            orgMember = m_dicCompanyMembers[member.ID];
            m_dicCompanyMembers.Remove(member.ID);

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_dicTeamCompanyMembers)
            {
                if (item.Key.TeamID == member.Team.TeamID)
                {
                    foreach (CompanyMember itemValue in item.Value)
                    {
                        if (itemValue.ID == member.ID)
                        {
                            itemValue.ID = newMemberID;
                            break;
                        }
                    }

                    break;
                }
            }

            orgMember.ID = newMemberID;
            m_dicCompanyMembers[newMemberID] = orgMember;

            
            //
            //List<CompanyMember> members;
            //m_dicTeamCompanyMembers.TryGetValue(member.Team, out members);
            
            //foreach (CompanyMember item in members)
            //{
            //    if (item == member)
            //    {
            //        item.ID = newMemberID;
            //        break;
            //    }
            //} 

        }

        /// <summary>
        /// 신규 직원 팀 이동
        /// </summary> 
        public static void SetMoveRegularTeamMemberInfo(RegularTeam orgTeam, RegularTeam newTeam, CompanyMember member)
        {
            List<CompanyMember> members = new List<CompanyMember>();
            m_dicTeamCompanyMembers.TryGetValue(orgTeam, out members);

            foreach (CompanyMember item in members)
            {
                if (item == member)
                {
                    members.Remove(item);
                    break;
                }
            }

            if (!m_dicTeamCompanyMembers.ContainsKey(newTeam))
                m_dicTeamCompanyMembers.Add(newTeam, new List<CompanyMember>());

            m_dicTeamCompanyMembers[newTeam].Add(member); 
        }

        public static void SetRegularTeam(int nTeamID, RegularTeam team)
        { 
            m_dicRegularTeams[nTeamID] = team;             
        }

        public static void SetTemporaryTeam(int nTeamID, Team team, bool isNormal)
        {
            if (isNormal)
                m_dicTemporaryNormalTeams[nTeamID] = (TemporaryNormalTeam)team;
            else
                m_dicTemporaryEmergencyTeams[nTeamID] = (TemporaryEmergencyTeam)team;
        }

        public static void SetExternalTeam(int nTeamID, ExternalTeam team)
        {
            m_dicExternalTeams[nTeamID] = team;
        }

        public static RegularTeam GetRegularTeam(int nTeamID)
        {
            RegularTeam team = null;
            m_dicRegularTeams.TryGetValue(nTeamID, out team);
            return team;
        } 

        // nTeamID에 해당하는 팀 및 그 하위팀들 모두를 계층구조에 따라 정렬하여 리턴한다.
        public static List<Team> GetRegularTeams(int nTeamID)
        {
            return GetTeams(nTeamID, TeamTreeView.TeamType.REGULAR);
            /*List<TeamNDepth> teams = new List<TeamNDepth>();

            RegularTeam team;

            if (m_dicRegularTeams.TryGetValue(nTeamID, out team))
            {
                int nDepth = 0;
                TeamNDepth _team = new TeamNDepth(team, nDepth);
                teams.Add(_team);
                
                GetRegularChildTeams(team, teams, nDepth + 1);
                teams.Sort();
            }

            List<RegularTeam> teams2 = new List<RegularTeam>();

            foreach (TeamNDepth _team in teams)
            {
                teams2.Add(_team.Team);
            }

            return teams2;*/
        } 


        // nTeamID에 해당하는 팀 및 그 하위팀들 모두를 계층구조에 따라 정렬하여 리턴한다.
        // (ExternalTeam / ExternalCompanyTeam 두가지 타입을 모두 리턴)
        public static List<Team> GetExternalTeams(int nTeamID)
        {
            return GetTeams(nTeamID, TeamTreeView.TeamType.EXTERNAL);
        }

        // nTeamID에 해당하는 팀 및 그 하위팀들 모두를 계층구조에 따라 정렬하여 리턴한다.
        public static List<Team> GetTemporaryTeams(int nTeamID, bool isNormal)
        {
            return GetTeams(nTeamID, isNormal ? TeamTreeView.TeamType.TEMPORARY_NORMAL : TeamTreeView.TeamType.TEMPORARY_EMERGENCY);
        }

        // teamParent의 자식 팀들을 teams에 담는다.
        private static void GetChildTeams(Team teamParent, List<TeamNDepth> teams, int nDepth, TeamTreeView.TeamType type)
        {
            if (type == TeamTreeView.TeamType.REGULAR)
            {
                foreach (KeyValuePair<int, RegularTeam> pair in m_dicRegularTeams)
                {
                    if (pair.Value.ParentTeam == null)
                        continue;

                    //bool isPass = false;
                    //foreach (RegularTeam item in m_deleteRegularTeams)
                    //{
                    //    if (item.TeamID == pair.Value.TeamID)
                    //    {
                    //        isPass = true;
                    //        break;
                    //    }
                    //}
                    //if (isPass)
                    //    continue;

                    if (!pair.Value.Visible)
                        continue;

                    if (pair.Value.ParentTeam.TeamID == teamParent.TeamID)
                    {
                        TeamNDepth _team = new TeamNDepth(pair.Value, nDepth);
                        teams.Add(_team);

                        GetChildTeams(pair.Value, teams, nDepth + 1, type);
                    }
                }
            }
            else if (type == TeamTreeView.TeamType.TEMPORARY_NORMAL)
            {
                foreach (KeyValuePair<int, TemporaryNormalTeam> pair in m_dicTemporaryNormalTeams)
                {
                    if (pair.Value.ParentTeam == teamParent)
                    {
                        TeamNDepth _team = new TeamNDepth(pair.Value, nDepth);
                        teams.Add(_team);

                        GetChildTeams(pair.Value, teams, nDepth + 1, type);
                    }
                }
            }
            else if (type == TeamTreeView.TeamType.TEMPORARY_EMERGENCY)
            {
                foreach (KeyValuePair<int, TemporaryEmergencyTeam> pair in m_dicTemporaryEmergencyTeams)
                {
                    if (pair.Value.ParentTeam == teamParent)
                    {
                        TeamNDepth _team = new TeamNDepth(pair.Value, nDepth);
                        teams.Add(_team);

                        GetChildTeams(pair.Value, teams, nDepth + 1, type);
                    }
                }
            }
            else if (type == TeamTreeView.TeamType.EXTERNAL)
            {
                foreach (KeyValuePair<int, ExternalTeam> pair in m_dicExternalTeams)
                {
                    if (pair.Value.ParentTeam == teamParent)
                    {
                        TeamNDepth _team = new TeamNDepth(pair.Value, nDepth);
                        teams.Add(_team);

                        GetChildTeams(pair.Value, teams, nDepth + 1, type);
                    }
                }

            }
        }

        #region 조회 Team 정렬
        public static SortedDictionary<int, Team> GetTeamsSort(RegularTeam nTeamID, TeamTreeView.TeamType type)
        {
            List<TeamNDepth> teams = new List<TeamNDepth>();

            Team team = nTeamID;

            if (team != null)
            {
                int sortNum = 0;
                int nDepth = 0;
                TeamNDepth _team = new TeamNDepth(team, nDepth);
                _team.SortNum = sortNum;
                teams.Add(_team);
                
                GetChildTeamsSort(team, teams, nDepth + 1, type, ref sortNum);
                teams.Sort();
            }

            SortedDictionary<int, Team> teams2 = new SortedDictionary<int, Team>();

            foreach (TeamNDepth _team in teams)
            {
                teams2.Add(_team.SortNum, _team.Team);
            }

            return teams2;
        }

        private static void GetChildTeamsSort(Team teamParent, List<TeamNDepth> teams, int nDepth, TeamTreeView.TeamType type, ref int sortNum)
        {
            if (type == TeamTreeView.TeamType.REGULAR)
            { 
                foreach (KeyValuePair<int, RegularTeam> pair in m_dicRegularTeams)
                {
                    if (pair.Value.ParentTeam == null)
                        continue;

                    if (!pair.Value.Visible)
                        continue;

                    if (pair.Value.ParentTeam.TeamID == teamParent.TeamID)
                    {
                        sortNum++;

                        TeamNDepth _team = new TeamNDepth(pair.Value, nDepth);
                        _team.SortNum = sortNum;
                        teams.Add(_team);

                        GetChildTeamsSort(pair.Value, teams, nDepth + 1, type, ref sortNum); 
                    }
                }
            }
        } 
        #endregion

        private static List<Team> GetTeams(int nTeamID, TeamTreeView.TeamType type)
        {
            List<TeamNDepth> teams = new List<TeamNDepth>();

            Team team = GetTeam(nTeamID, type);

            if (team != null)
            {
                int nDepth = 0;
                TeamNDepth _team = new TeamNDepth(team, nDepth);
                teams.Add(_team);

                GetChildTeams(team, teams, nDepth + 1, type);
                teams.Sort();
            }

            List<Team> teams2 = new List<Team>();

            foreach (TeamNDepth _team in teams)
            {
                teams2.Add(_team.Team);
            }

            return teams2;
        } 

        private static Team GetTeam(int nTeamID, TeamTreeView.TeamType type)
        {
            if (type == TeamTreeView.TeamType.REGULAR)
            {
                RegularTeam team;

                if (m_dicRegularTeams.TryGetValue(nTeamID, out team))
                    return team;
            }
            else if (type == TeamTreeView.TeamType.TEMPORARY_NORMAL)
            {
                TemporaryNormalTeam team;

                if (m_dicTemporaryNormalTeams.TryGetValue(nTeamID, out team))
                    return team;
            }
            else if (type == TeamTreeView.TeamType.TEMPORARY_EMERGENCY)
            {
                TemporaryEmergencyTeam team;

                if (m_dicTemporaryEmergencyTeams.TryGetValue(nTeamID, out team))
                    return team;
            }
            else if (type == TeamTreeView.TeamType.EXTERNAL)
            {
                ExternalTeam team;

                if (m_dicExternalTeams.TryGetValue(nTeamID, out team))
                    return team;
            }

            return null;
        }

        public static TemporaryNormalTeam GetTemporaryNormalTeam(int nTeamID)
        {
            TemporaryNormalTeam team = null;
            m_dicTemporaryNormalTeams.TryGetValue(nTeamID, out team);
            return team;
        }

        public static TemporaryEmergencyTeam GetTemporaryEmergencyTeam(int nTeamID)
        {
            TemporaryEmergencyTeam team = null;
            m_dicTemporaryEmergencyTeams.TryGetValue(nTeamID, out team);
            return team;
        }

        public static UserDefinedTeam GetUserDefinedTeam(int nTeamID)
        {
            UserDefinedTeam team = null;

            foreach (UserDefinedTeam item in from items in m_UserDefinedTeams.AsEnumerable()
                                             where items.TeamID == nTeamID
                                             select items)
            {
                team = item;
            }

            //m_dicUserDefinedTeams.TryGetValue(nTeamID, out team);

            return team;
        }

        public static List<UserDefinedTeam> GetUserDefinedTeams()
        {
            return m_UserDefinedTeams;
        }

        public static ExternalTeam GetExternalTeam(int nTeamID)
        {
            ExternalTeam team = null;
            m_dicExternalTeams.TryGetValue(nTeamID, out team);
            return team;
        }

        public static ExternalCompanyMember GetExternalCompanyMember(int nExternalCompanyMemberID)
        {
            ExternalCompanyMember member = null;
            m_dicExternalCompanyMembers.TryGetValue(nExternalCompanyMemberID, out member);
            return member;
        }

        private static string MakeLoadCompanyMemberQuery(string strCompanyMemberID)
        {
            string strSQL = "select ID, MemberName, RegularTeamID, LevelID, SubLevelID, PositionID, SubPositionID, GroupPositionID, MemberID, OfficePhoneNumber, PhoneNumber ";
            strSQL += "from CompanyMember, RegularMemberList ";
            strSQL += "where CompanyMember.ID = " + strCompanyMemberID;

            return strSQL;
        }

        private static CompanyMember ReadCompanyMember(ArrayList arrResults, int nIndex, out RegularTeam team)
        {
            team = null;

            int nID = WebDBManager.GetIntField(arrResults[nIndex].ToString(), -1);
            string strMemberName = WebDBManager.GetStringField(arrResults[nIndex + 1], "");
            int nTeamID = WebDBManager.GetIntField(arrResults[nIndex + 2].ToString(), -1);
            int nLevelID = WebDBManager.GetIntField(arrResults[nIndex + 3].ToString(), -1);
            int nSubLevelID = WebDBManager.GetIntField(arrResults[nIndex + 4].ToString(), -1);
            int nPositionID = WebDBManager.GetIntField(arrResults[nIndex + 5].ToString(), -1);
            int nSubPositionID = WebDBManager.GetIntField(arrResults[nIndex + 6].ToString(), -1);
            int nGroupPositionID = WebDBManager.GetIntField(arrResults[nIndex + 7].ToString(), -1);
            string strMemberID = WebDBManager.GetStringField(arrResults[nIndex + 8], "");
            string strOfficePhoneNumber = WebDBManager.GetStringField(arrResults[nIndex + 9], "");
            string strPhoneNumber = WebDBManager.GetStringField(arrResults[nIndex + 10], "");

            if (nID < 0)
                return null;

            team = GetRegularTeam(nTeamID);

            if (team == null)
                return null;

            if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                strPhoneNumber = "";
            else
                strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);

            CompanyMember member = new CompanyMember();

            member.ID = nID;
            member.Name = strMemberName;
            member.Team = team;
            member.LevelID = nLevelID;
            member.PositionID = nPositionID;
            member.MemberID = strMemberID == "null" ? "" : strMemberID;
            member.OfficePhoneNumber = strOfficePhoneNumber == "null" ? "" : strOfficePhoneNumber;
            member.PhoneNumber = strPhoneNumber;
            member.SubJobLevel = CompanyMember.JobLevelSubInfo.GetJobSubLevel(nSubLevelID);
            member.SubJobPosition = CompanyMember.JobPositionSubInfo.GetSubPosition(nSubPositionID);
            member.GroupPosition = CompanyMember.JobGroupPosition.GetJobGroupPosition(nGroupPositionID);

            string strSubPositionName = member.SubJobPosition == null ? null : member.SubJobPosition.Name;

            // 팀장급 이상의 포지션인 경우에는 세부포지션의 명칭에 따라 다르게 표기해준다.
            if (String.IsNullOrWhiteSpace(strSubPositionName) == false
                && String.Equals(GetJobPositionName(member.PositionID), "팀장"))
            {
                int nNewPositionID = 0;
                string strNewPositionName = string.Empty;

                if (strSubPositionName.IndexOf("본부장") > -1)
                {
                    strNewPositionName = "본부장";
                }
                else if (strSubPositionName.IndexOf("처장") > -1)
                {
                    strNewPositionName = "처장";
                }
                else if (strSubPositionName.IndexOf("실장") > -1)
                {
                    strNewPositionName = "실장";
                }
                else if (strSubPositionName.IndexOf("팀장") > -1)
                {
                    strNewPositionName = "팀장";
                }
                else if (strSubPositionName.IndexOf("파트장") > -1)
                {
                    strNewPositionName = "파트장";
                }
                else
                {
                    strNewPositionName = "팀장";
                }

                if (GetJobPositionID(strNewPositionName, out nNewPositionID))
                    member.PositionID = nNewPositionID;

            }

            // 팀이 파트 팀인 경우에는 파트장이 실제DB에는 팀장으로 데이터가 입력되어 있으므로
            // 팀장으로 되어있는 데이터를 다시 파트장으로 변환
            if (team.IsPartTeam)
            {
                if (String.Equals(GetJobPositionName(member.PositionID), "팀장"))
                {
                    int nNewPositionID = 0;

                    if (GetJobPositionID("파트장", out nNewPositionID))
                        member.PositionID = nNewPositionID;
                }
            }

            return member;
        }

        public static CompanyMember LoadCompanyMember(WebDBManager dbMgr, int nTransaction, int nCompanyMemberID)
        {
            string strSQL = MakeLoadCompanyMemberQuery(nCompanyMemberID.ToString());

            ArrayList arrResults = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);// dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResults.Count == 0)
                return null;

            RegularTeam team;
            CompanyMember member = ReadCompanyMember(arrResults, 0, out team);
            return member;
        }

        public static bool LoadCompanyMember(WebDBManager dbMgr)
        {
            if (!LoadJobSubLevel(dbMgr, 0))
                return false;

            if (!LoadJobSubPosition(dbMgr, 0))
                return false;

            if (!LoadJobPositionGroup(dbMgr, 0))
                return false;

            string strSQL = MakeLoadCompanyMemberQuery("RegularMemberList.CompanyMemberID");

            ArrayList arrResults = dbMgr.GetResultData(strSQL, 0);

            if (arrResults == null)
                return false;

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return false;

            m_dicCompanyMembers.Clear();
            m_dicTeamCompanyMembers.Clear();

            foreach (RegularTeam r_team in m_dicRegularTeams.Values)
            {
                if (m_dicTeamCompanyMembers.ContainsKey(r_team) == false)
                {
                    m_dicTeamCompanyMembers.Add(r_team, new List<CompanyMember>());
                }
            }

            RegularTeam team;

            for (int i = 0; i < nResultCount - 10; i += 11)
            {
                CompanyMember member = ReadCompanyMember(arrResults, i, out team);

                if (member == null)
                    continue;

                m_dicCompanyMembers[member.ID] = member;

                List<CompanyMember> members;

                if (!m_dicTeamCompanyMembers.TryGetValue(team, out members))
                {
                    members = new List<CompanyMember>();
                    m_dicTeamCompanyMembers[team] = members;
                }

                members.Add(member);
            }

            return true;
        }

        private static bool LoadJobSubLevel(WebDBManager dbMgr, int nTransaction)
        {
            string strSQL = "Select ID, Name from JobSubLevel";
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");

                CompanyMember.JobLevelSubInfo subLevel = new CompanyMember.JobLevelSubInfo();
                subLevel.ID = nID;
                subLevel.Name = strName;
            }

            return true;
        }

        private static bool LoadJobSubPosition(WebDBManager dbMgr, int nTransaction)
        {
            string strSQL = "Select ID, Name from JobSubPosition";
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");

                CompanyMember.JobPositionSubInfo subPosition = new CompanyMember.JobPositionSubInfo();
                subPosition.ID = nID;
                subPosition.Name = strName;
            }

            return true;
        }

        private static bool LoadJobPositionGroup(WebDBManager dbMgr, int nTransaction)
        {
            string strSQL = "Select ID, Name from JobPositionGroup";
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");

                CompanyMember.JobGroupPosition groupPosition = new CompanyMember.JobGroupPosition();
                groupPosition.ID = nID;
                groupPosition.Name = strName;
            }

            return true;
        }

        private static string MakeLoadExternalCompanyMemberQuery(string strExternalCompanyMemberID)
        {
            string strSQL = "select ID, Name, PhoneNumber, ExternalCompanyTeamID, JobLevelID, JobPositionID, Description ";
            strSQL += "from ExternalCompanyMember, ExternalMemberList ";
            strSQL += "where ExternalCompanyMember.ID = " + strExternalCompanyMemberID;

            return strSQL;
        }

        private static ExternalCompanyMember ReadExternalCompanyMember(ArrayList arrResults, int nIndex, out ExternalTeam team)
        {
            team = null;

            int nID = WebDBManager.GetIntField(arrResults[nIndex].ToString(), -1);
            string strMemberName = WebDBManager.GetStringField(arrResults[nIndex + 1], "");
            string strPhoneNumber = WebDBManager.GetStringField(arrResults[nIndex + 2], "");
            int nTeamID = WebDBManager.GetIntField(arrResults[nIndex + 3].ToString(), -1);
            int nJobLevelID = WebDBManager.GetIntField(arrResults[nIndex + 4].ToString(), -1);
            int nJobPositionID = WebDBManager.GetIntField(arrResults[nIndex + 5].ToString(), -1);
            //bool isTeamLeader = WebDBManager.GetIntField(arrResults[nIndex + 4].ToString(), 0) == 0 ? false : true;
            string strDesc = WebDBManager.GetStringField(arrResults[nIndex + 6], null);

            if (nID < 0)
                return null;

            //string strLevelName = null, strPositionName = null;

            //if (nJobLevelID > 0 && !m_dicExternalJobLevels.TryGetValue(nJobLevelID, out strLevelName))
            //    return null;

            //if (nJobPositionID > 0 && !m_dicExternalJobPositions.TryGetValue(nJobPositionID, out strPositionName))
            //    return null;

            team = GetExternalTeam(nTeamID);

            if (team == null)
                return null;

            if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                strPhoneNumber = "";
            else
                strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);

            ExternalCompanyMember member = new ExternalCompanyMember();

            member.ID = nID;
            member.Name = strMemberName;
            member.PhoneNumber = strPhoneNumber;

            member.ExternalJobLevel =    ExternalCompanyMember.ExternalJobLevelInfo.GetExternalJobLevel(nJobLevelID);
            member.ExternalJobPosition = ExternalCompanyMember.ExternalJobPositionInfo.GetExternalJobPosition(nJobPositionID);

            if (strDesc != null && strDesc != "null")
                member.Description = strDesc;
            member.Team = team;
            return member;
        }

        public static ExternalCompanyMember LoadExternalCompanyMember(WebDBManager dbMgr, int nTransaction, int nExternalCompanyMemberID)
        {
            string strSQL = MakeLoadExternalCompanyMemberQuery(nExternalCompanyMemberID.ToString());

            ArrayList arrResults = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResults.Count == 0)
                return null;

            ExternalTeam team;
            ExternalCompanyMember member = ReadExternalCompanyMember(arrResults, 0, out team);
            return member;
        }

        private static bool LoadExternalJobLevel(WebDBManager dbMgr, int nTransaction)
        {
            string strSQL = "Select ID, LevelName from ExternalJobLevel";
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");

                ExternalCompanyMember.ExternalJobLevelInfo externalJobLevel = new ExternalCompanyMember.ExternalJobLevelInfo();
                externalJobLevel.ID = nID;
                externalJobLevel.Name = strName;
            }
            return true;
        }

        private static bool LoadExternalJobPosition(WebDBManager dbMgr, int nTransaction)
        {
            string strSQL = "Select ID, PositionName from ExternalJobPosition";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            if (nResultCount == 0)
                return false;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], "");

                ExternalCompanyMember.ExternalJobPositionInfo externalJobLevel = new ExternalCompanyMember.ExternalJobPositionInfo();
                externalJobLevel.ID = nID;
                externalJobLevel.Name = strName;
            }

            return true;
        }

        public static bool LoadExternalCompanyMember(WebDBManager dbMgr)
        {
            if (!LoadExternalJobLevel(dbMgr, 0))
                return false;

            if (!LoadExternalJobPosition(dbMgr, 0))
                return false;

            string strSQL = MakeLoadExternalCompanyMemberQuery("ExternalMemberList.ExternalCompanyMemberID");

            ArrayList arrResults = dbMgr.GetResultData(strSQL, 0);

            if (arrResults == null)
                return false;

            int nResultCount = arrResults.Count;

            if (nResultCount == 0)
                return false;

            m_dicExternalCompanyMembers.Clear();
            m_dicExternalCompanyTeamMembers.Clear();

            ExternalTeam team;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                ExternalCompanyMember member = ReadExternalCompanyMember(arrResults, i, out team);

                if (member == null)
                    continue;

                m_dicExternalCompanyMembers[member.ID] = member;

                List<ExternalCompanyMember> members;

                if (!m_dicExternalCompanyTeamMembers.TryGetValue(team, out members))
                {
                    members = new List<ExternalCompanyMember>();
                    m_dicExternalCompanyTeamMembers[team] = members;
                }

                members.Add(member);
            }

            return true;
        }

        public static List<ExternalCompanyMember> GetExternalCompanyMembers(ExternalTeam team)
        {
            List<ExternalCompanyMember> members;

            if (!m_dicExternalCompanyTeamMembers.TryGetValue(team, out members))
            {
                members = new List<ExternalCompanyMember>();
                m_dicExternalCompanyTeamMembers[team] = members;
            }

            return members;
        }

        /// <summary>
        /// 하위 팀에 속한 모든 Member 가져옴
        /// </summary> 
        public static List<ExternalCompanyMember> GetChildExternalCompanyMembers(ExternalTeam team)
        {
            List<ExternalCompanyMember> members = null;

            List<Team> childTeams = GetExternalTeams(team.TeamID);
            childTeams.Sort((a, b) => a.TeamID.CompareTo(b.TeamID));

            foreach (ExternalTeam item in childTeams)
            {
                if (members == null)
                    members = new List<ExternalCompanyMember>();
                if (m_dicExternalCompanyTeamMembers.ContainsKey(item))
                    members.AddRange(m_dicExternalCompanyTeamMembers[item]);
            }

            if (members == null && team != null)
            {
                members = new List<ExternalCompanyMember>();

                m_dicExternalCompanyTeamMembers.Add(team, members);
            }

            return members;
        }

        public static List<ExternalCompanyMember> GetChildExternalCompanyMember(ExternalTeam team)
        {
            List<ExternalCompanyMember> members = null;

            List<ExternalTeam> childTeams = new List<ExternalTeam>();
            ExternalTeam child = GetExternalTeam(team.TeamID);

            if (child != null)
            {
                childTeams.Add(child);
                childTeams.Sort((a, b) => a.TeamID.CompareTo(b.TeamID));
            }

            foreach (ExternalTeam item in childTeams)
            {
                if (members == null)
                    members = new List<ExternalCompanyMember>();
                if (m_dicExternalCompanyTeamMembers.ContainsKey(item))
                    members.AddRange(m_dicExternalCompanyTeamMembers[item]);
            }

            if (members == null && team != null)
            {
                members = new List<ExternalCompanyMember>();

                m_dicExternalCompanyTeamMembers.Add(team, members);
            }

            return members;
        }

        public static string EncryptString(string str)
        {
            return DBUtility.AES256Cipher.AES_encrypt(str, key);
        }

        public static List<CompanyMember> GetRegularMembers(RegularTeam team)
        {
            if (team == null)
                return null;

            List<CompanyMember> members = null;
            //m_dicTeamCompanyMembers.TryGetValue(team, out members);

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_dicTeamCompanyMembers)
            {
                if (item.Key.TeamID == team.TeamID)
                {
                    members = item.Value;
                    break;
                }
            }


            if (members == null && team != null)
            {
                members = new List<CompanyMember>();

                m_dicTeamCompanyMembers.Add(team, members);
            }

            return members;
        }

        public static void SetReularTeamVisible(RegularTeam team, bool visible)
        {
            if (team == null)
                return;

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_dicTeamCompanyMembers)
            {
                if (item.Key.TeamID == team.TeamID)
                {
                    item.Key.Visible = visible;
                    break;
                } 
            }
        } 

        /// <summary>
        /// 하위 팀에 속한 모든 Member 가져옴
        /// </summary> 
        public static List<CompanyMember> GetChildRegularMembers(RegularTeam team, Boolean OnlySelectTeam = false)
        {
            List<CompanyMember> members = null;

            List<Team> childTeams = null;// GetRegularTeams(team.TeamID);  
            if (OnlySelectTeam == false)
            {
                childTeams = GetRegularTeams(team.TeamID);
            }
            else
            {
                childTeams = new List<Team>();
                RegularTeam childTeam = GetRegularTeam(team.TeamID);

                if (childTeam != null)
                    childTeams.Add(childTeam);
            }
             
            foreach (RegularTeam item in childTeams)
            {
                if (members == null)
                    members = new List<CompanyMember>();

                if (m_dicTeamCompanyMembers == null || !m_dicTeamCompanyMembers.ContainsKey(item) || 
                    m_dicTeamCompanyMembers[item] == null || m_dicTeamCompanyMembers[item].Count == 0) continue;
                                   
                members.AddRange(m_dicTeamCompanyMembers[item]);             
            } 

            if (members == null && team != null)
            {
                if (m_dicTeamCompanyMembers.TryGetValue(team, out members) == false)
                {
                    members = new List<CompanyMember>();
                    m_dicTeamCompanyMembers[team] = members;
                }
                /*members = new List<CompanyMember>();

                if (!m_dicTeamCompanyMembers.ContainsKey(team))
                    m_dicTeamCompanyMembers.Add(team, members);*/
            }

            return members;
        }

        
        public static List<CompanyMember>[] GetAllRegularMembers()
        {
            return m_dicTeamCompanyMembers.Values.ToArray();
        }
         
        public static void SetRegularMembers(RegularTeam team, List<CompanyMember> members = null)
        {
            if (team == null || members == null)
                members = new List<CompanyMember>();

            m_dicTeamCompanyMembers[team] = members;
        }

        public static void SetRegularMember(RegularTeam team, CompanyMember member = null)
        {
            if (member == null)
                member = new CompanyMember();
            else
                m_dicCompanyMembers[member.ID] = member;

            RegularTeam team2 = null;
            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_dicTeamCompanyMembers)
            {
                if (item.Key.TeamID == team.TeamID)
                {
                    team2 = item.Key;
                    break;
                }
            }

            if (team2 == null)
            {
                m_dicTeamCompanyMembers.Add(team, new List<CompanyMember>());
                m_dicTeamCompanyMembers[team].Add(member);  
            }
            else
            {
                m_dicTeamCompanyMembers[team2].Add(member);  
            }
            
        }

        public static string OverlapRegularMember(CompanyMember member, string phoneNumber)
        {
            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_dicTeamCompanyMembers)
            {
                foreach (CompanyMember item2 in item.Value)
                {
                    if (member == null || item2 == member || item2.PhoneNumber == null)
                        continue;

                    if (item2.PhoneNumber.Length > 0 && item2.PhoneNumber.Replace("-", "") == phoneNumber.Replace("-", ""))
                    {
                        if (item2.Team != null)
                            return item2.Team.TeamName + " 팀 " + item2.Name + " (" + item2.PhoneNumber + ") 해당 직원과 휴대전화번호가 중복됩니다.\r\n다시 입력해주세요.";
                        else
                            return item2.Name + " (" + item2.PhoneNumber + ") 해당 직원과 휴대전화번호가 중복됩니다.\r\n다시 입력해주세요.";
                    }
                }
            }

            return "";
        }

        public static void SetTemporaryNormalMembers(TemporaryNormalTeam team, List<TemporaryMember> members = null)
        {
            if (members == null)
                members = new List<TemporaryMember>();

            m_dicTemporaryNormalMembers[team] = members;
        }

        public static List<TemporaryMember> GetTemporaryNormalMembers(TemporaryNormalTeam team)
        {
            List<TemporaryMember> members = null;
            m_dicTemporaryNormalMembers.TryGetValue(team, out members);
            return members;
        }

        public static void SetTemporaryEmergencyMembers(TemporaryEmergencyTeam team, List<TemporaryMember> members = null)
        {
            if (members == null)
                members = new List<TemporaryMember>();

            m_dicTemporaryEmergencyMembers[team] = members;
        }

        public static List<TemporaryMember> GetTemporaryEmergencyMembers(TemporaryEmergencyTeam team)
        {
            List<TemporaryMember> members = null;
            m_dicTemporaryEmergencyMembers.TryGetValue(team, out members);
            return members;
        }

        public static CompanyMember GetCompanyMember(int nID)
        {
            CompanyMember member = null;
            m_dicCompanyMembers.TryGetValue(nID, out member);
            return member;
        }

        private static bool IsValidateRegularTeam(System.Windows.Forms.TreeNode node, RegularTeam team)
        {
            bool isValidate = false;

            if (node.Tag == team)
            {
                isValidate = true;
            }
            else
            {
                foreach (System.Windows.Forms.TreeNode childNode in node.Nodes)
                {
                    if (IsValidateRegularTeam(childNode, team))
                    {
                        isValidate = true;
                        break;
                    }

                }
            }

            return isValidate;
        }

        public static CompanyMember GetCompanyMemberByMemberID(string strMemberID, TeamTreeView tree)
        {
            CompanyMember rtnMember = null;

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_dicTeamCompanyMembers)
            {
                foreach (CompanyMember member in from members in item.Value
                                                 where members.MemberID == strMemberID
                                                 select members
                                                )
                {
                    if (IsValidateRegularTeam(tree.TopNode, item.Key))
                    {
                        rtnMember = member;
                        break;
                    }
                }
            }

            return rtnMember;
        }

        public static RegularTeam GetRegularTeamByCompanyMember(CompanyMember member)
        {
            RegularTeam rtnTeam = null;

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_dicTeamCompanyMembers)
            {
                if (item.Value != null && item.Value.Contains(member))
                {
                    rtnTeam = item.Key;
                    break;
                }
            }

            return rtnTeam;
        }

        public static bool LoadNormalMember(WebDBManager dbMgr)
        {
            //string strSQL = "select TemporaryMemberList.ID, TemporaryMemberList.MemberName, TemporaryTeamID, MemberID, IsTeamLeader, MemberType, MemberCount, Role from TemporaryMemberList, TemporaryNormalTeam ";
            //strSQL += "where IsNormal = 1 and TemporaryMemberList.TemporaryTeamID = TemporaryNormalTeam.ID and TemporaryNormalTeam.SiteID = " + FormMain.Instance.SiteID.ToString();

            string strSQL = String.Empty;
            strSQL += "SELECT	A.ID,";
		    strSQL += "         A.MemberName,";
		    strSQL += "         A.TemporaryTeamID,";
		    strSQL += "         A.MemberID,";
		    strSQL += "         A.IsTeamLeader,";
		    strSQL += "         A.MemberType,";
		    strSQL += "         A.MemberCount,";
		    strSQL += "         A.Role,";
		    strSQL += "         (CASE ";
			strSQL += "         WHEN C.ExternalCompanyTeamID IS NOT NULL THEN C.ExternalCompanyTeamID ";
			strSQL += "         WHEN D.RegularTeamID IS NOT NULL THEN D.RegularTeamID ";
			strSQL += "         ELSE -1 ";
		    strSQL += "         END)	AS TeamID ";
            strSQL += "FROM		TemporaryMemberList     AS A ";
            strSQL += "INNER JOIN	TemporaryNormalTeam	AS B	ON (A.TemporaryTeamID = B.ID) ";
            strSQL += "LEFT JOIN	ExternalMemberList	AS C	ON (A.MemberType = 4 AND A.MemberID = C.ExternalCompanyMemberID) ";
            strSQL += "LEFT JOIN	RegularMemberList	AS D	ON (A.MemberType = 1 AND A.MemberID = D.CompanyMemberID) ";
            strSQL += "WHERE	A.IsNormal = 1 ";
            strSQL += "AND		B.SiteID = ";
            strSQL += FormMain.Instance.SiteID.ToString();
            strSQL += " ORDER BY A.ID ASC";

            ArrayList arrResults = dbMgr.GetResultData(strSQL, 0);

            if (arrResults == null)
                return false;

            TemporaryMember.MemberType memberType;
            TemporaryMember.ManagerType managerType;

            int nResultCount = arrResults.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResults[i + 1], "");
                int nTemporaryNormalTeamID = WebDBManager.GetIntField(arrResults[i + 2].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResults[i + 3].ToString(), 0);
                bool isTeamLeader = WebDBManager.GetIntField(arrResults[i + 4].ToString(), 0) == 0 ? false : true;
                int nMemberType = WebDBManager.GetIntField(arrResults[i + 5].ToString(), -1);
                int nMemberCount = WebDBManager.GetIntField(arrResults[i + 6].ToString(), -1);
                int nRole = WebDBManager.GetIntField(arrResults[i + 7].ToString(), -1);
                int nTeamID = WebDBManager.GetIntField(arrResults[i + 8].ToString(), 0);

                if (nID < 0 || nTemporaryNormalTeamID < 0)
                    continue;

                TemporaryMember.ToManagerType(nRole, out managerType);

                if (!TemporaryMember.ToMemberType(nMemberType, out memberType))
                    continue;

                bool isIncludeChildTeams = false;

                if (nMemberID < 0)
                    nMemberID = -nMemberID;
                else if (nMemberID > 0)
                    isIncludeChildTeams = true;

                TemporaryNormalMember member = new TemporaryNormalMember();

                member.ID = nID;
                //member.MemberID = nMemberID;
                member.DisplayName = strMemberName;
                member.TemporaryManagerType = managerType;
                member.TemporaryMemberType = memberType;
                member.IsTeamLeader = isTeamLeader;

                if (memberType == TemporaryMember.MemberType.RegularTeam)
                {
                    member.Team = DataManager.GetRegularTeam(nMemberID);

                    if (member.Team == null)
                        continue;
                }
                else if (memberType == TemporaryMember.MemberType.CompanyMember)
                {
                    member.CompanyMember = DataManager.GetCompanyMember(nMemberID);
                    member.Team = DataManager.GetRegularTeam(nTeamID);

                    if (member.CompanyMember == null || member.Team == null)
                        continue;
                }
                else if (memberType == TemporaryMember.MemberType.ExternalTeam)
                {
                    member.Team = DataManager.GetExternalTeam(nMemberID);

                    if (member.Team == null)
                        continue;
                }
                else if (memberType == TemporaryMember.MemberType.ExternalCompanyMember)
                {
                    member.ExternalCompanyMember = DataManager.GetExternalCompanyMember(nMemberID);
                    member.Team = DataManager.GetExternalTeam(nTeamID);

                    if (member.ExternalCompanyMember == null || member.Team == null)
                        continue;
                }
                else if (memberType == TemporaryMember.MemberType.UserDefinedTeam)
                {
                    member.Team = DataManager.GetUserDefinedTeam(nMemberID);

                    if (member.Team == null)
                        continue;
                }
                else if (memberType == TemporaryMember.MemberType.LevelID)
                {
                    member.LevelID = nMemberID;
                }
                else
                    continue;

                member.TemporaryTeam = GetTemporaryNormalTeam(nTemporaryNormalTeamID);
                member.MemberCount = nMemberCount;
                member.IncludeChildTeam = isIncludeChildTeams;

                if (member.TemporaryTeam != null)
                {
                    List<TemporaryMember> members = GetTemporaryNormalMembers(member.TemporaryTeam);

                    if (members == null)
                        SetTemporaryNormalMembers(member.TemporaryTeam, member.TemporaryTeam.Members);

                    member.TemporaryTeam.Members.Add(member);
                }
            }

            return true;
        }

        public static bool LoadEmergencyMember(WebDBManager dbMgr)
        {
            //string strSQL = "select TemporaryMemberList.ID, TemporaryMemberList.MemberName, TemporaryTeamID, MemberID, IsTeamLeader, MemberType, MemberCount, Role from TemporaryMemberList, TemporaryEmergencyTeam ";
            //strSQL += "where IsNormal = 0 and TemporaryMemberList.TemporaryTeamID = TemporaryEmergencyTeam.ID and TemporaryEmergencyTeam.SiteID = " + FormMain.Instance.SiteID.ToString();

            string strSQL = String.Empty;
            strSQL += "SELECT	A.ID, ";
		    strSQL += "         A.MemberName, ";
		    strSQL += "         A.TemporaryTeamID, ";
		    strSQL += "         A.MemberID, ";
		    strSQL += "         A.IsTeamLeader, ";
		    strSQL += "         A.MemberType, ";
		    strSQL += "         A.MemberCount, ";
		    strSQL += "         A.Role, ";
		    strSQL += "         (CASE ";
			strSQL += "             WHEN C.ExternalCompanyTeamID IS NOT NULL THEN C.ExternalCompanyTeamID ";
			strSQL += "             WHEN D.RegularTeamID IS NOT NULL THEN D.RegularTeamID ";
			strSQL += "             ELSE -1 ";
		    strSQL += "         END)	AS TeamID ";
            strSQL += "FROM		TemporaryMemberList		AS A ";
            strSQL += "INNER JOIN	TemporaryEmergencyTeam	AS B	ON (A.TemporaryTeamID = B.ID) ";
            strSQL += "LEFT JOIN	ExternalMemberList		AS C	ON (A.MemberType = 4 AND A.MemberID = C.ExternalCompanyMemberID) ";
            strSQL += "LEFT JOIN	RegularMemberList		AS D	ON (A.MemberType = 1 AND A.MemberID = D.CompanyMemberID) ";
            strSQL += "WHERE	A.IsNormal = 0  ";
            strSQL += "AND		B.SiteID = ";
            strSQL += FormMain.Instance.SiteID.ToString();
            strSQL += " ORDER BY A.ID ASC";

            ArrayList arrResults = dbMgr.GetResultData(strSQL, 0);

            if (arrResults == null)
                return false;

            TemporaryMember.MemberType memberType;
            TemporaryMember.ManagerType managerType;

            int nResultCount = arrResults.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResults[i].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResults[i + 1], "");
                int nTemporaryEmergencyTeamID = WebDBManager.GetIntField(arrResults[i + 2].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResults[i + 3].ToString(), 0);
                bool isTeamLeader = WebDBManager.GetIntField(arrResults[i + 4].ToString(), 0) == 0 ? false : true;
                int nMemberType = WebDBManager.GetIntField(arrResults[i + 5].ToString(), -1);
                int nMemberCount = WebDBManager.GetIntField(arrResults[i + 6].ToString(), -1);
                int nRole = WebDBManager.GetIntField(arrResults[i + 7].ToString(), -1);
                int nTeamID = WebDBManager.GetIntField(arrResults[i + 8].ToString(), -1);

                if (nID < 0 || nTemporaryEmergencyTeamID < 0)
                    continue;

                TemporaryMember.ToManagerType(nRole, out managerType);

                if (!TemporaryMember.ToMemberType(nMemberType, out memberType))
                    continue;

                bool isIncludeChildTeams = false;

                if (nMemberID < 0)
                    nMemberID = -nMemberID;
                else if (nMemberID > 0)
                    isIncludeChildTeams = true;

                TemporaryEmergencyMember member = new TemporaryEmergencyMember();

                member.ID = nID;
                //member.MemberID = nMemberID;
                member.DisplayName = strMemberName;
                member.TemporaryManagerType = managerType;
                member.TemporaryMemberType = memberType;
                member.IsTeamLeader = isTeamLeader;

                if (memberType == TemporaryMember.MemberType.RegularTeam)
                {
                    member.Team = DataManager.GetRegularTeam(nMemberID);

                    if (member.Team == null)
                        continue;
                }
                else if (memberType == TemporaryMember.MemberType.CompanyMember)
                {
                    member.CompanyMember = DataManager.GetCompanyMember(nMemberID);
                    member.Team = DataManager.GetRegularTeam(nTeamID);

                    if (member.CompanyMember == null || member.Team == null)
                        continue;
                }
                else if (memberType == TemporaryMember.MemberType.ExternalTeam)
                {
                    member.Team = DataManager.GetExternalTeam(nMemberID);

                    if (member.Team == null)
                        continue;
                }
                else if (memberType == TemporaryMember.MemberType.ExternalCompanyMember)
                {
                    member.ExternalCompanyMember = DataManager.GetExternalCompanyMember(nMemberID);
                    member.Team = DataManager.GetExternalTeam(nTeamID);

                    if (member.ExternalCompanyMember == null || member.Team == null)
                        continue;
                }
                else if (memberType == TemporaryMember.MemberType.UserDefinedTeam)
                {
                    member.Team = DataManager.GetUserDefinedTeam(nMemberID);

                    if (member.Team == null)
                        continue;
                }
                else if (memberType == TemporaryMember.MemberType.LevelID)
                {
                    member.LevelID = nMemberID;
                }
                else
                    continue;

                member.TemporaryTeam = GetTemporaryEmergencyTeam(nTemporaryEmergencyTeamID);
                member.MemberCount = nMemberCount;
                member.IncludeChildTeam = isIncludeChildTeams;

                if (member.TemporaryTeam != null)
                {
                    List<TemporaryMember> members = GetTemporaryEmergencyMembers(member.TemporaryTeam);

                    if (members == null)
                        SetTemporaryEmergencyMembers(member.TemporaryTeam, member.TemporaryTeam.Members);

                    member.TemporaryTeam.Members.Add(member);
                }
            }

            return true;
        }

        public static bool GetCompanyMemberMemberIDChanged(CompanyMember member)
        {
            bool isChanged;

            if (m_dicCompanyMemberMemberIDChanged.TryGetValue(member, out isChanged))
                return isChanged;

            return false;
        }

        public static bool GetCompanyMemberPhoneNumberChanged(CompanyMember member)
        {
            bool isChanged;

            if (m_dicCompanyMemberPhoneNumberChanged.TryGetValue(member, out isChanged))
                return isChanged;

            return false;
        }

        public static bool GetCompanyMemberOfficePhoneNumberChanged(CompanyMember member)
        {
            bool isChanged;

            if (m_dicCompanyMemberOfficePhoneNumberChanged.TryGetValue(member, out isChanged))
                return isChanged;

            return false;
        }

        public static bool GetExternalCompanyMemberPhoneNumberChanged(ExternalCompanyMember member)
        {
            bool isChanged;

            if (m_dicExternalCompanyMemberPhoneNumberChanged.TryGetValue(member, out isChanged))
                return isChanged;

            return false;
        }

        public static void SetCompanyMemberMemberIDChanged(CompanyMember member, bool isChanged)
        {
            m_dicCompanyMemberMemberIDChanged[member] = isChanged;
        }

        public static void SetCompanyMemberPhoneNumberChanged(CompanyMember member, bool isChanged)
        {
            m_dicCompanyMemberPhoneNumberChanged[member] = isChanged;
        }
        public static void SetCompanyMemberOfficePhoneNumberChanged(CompanyMember member, bool isChanged)
        {
            m_dicCompanyMemberOfficePhoneNumberChanged[member] = isChanged;
        }

        public static void SetExternalCompanyMemberPhoneNumberChanged(ExternalCompanyMember member, bool isChanged)
        {
            m_dicExternalCompanyMemberPhoneNumberChanged[member] = isChanged;
        }

        public static List<TemporaryNormalTeam> GetTemporaryNormalRootTeams()
        {
            return m_rootNormalTeams;
        }

        public static List<TemporaryEmergencyTeam> GetTemporaryEmergencyRootTeams()
        {
            return m_rootEmergencyTeams;
        }

        public static UserDefinedTeam LoadUserDefinedTeam(WebDBManager dbMgr, int nTransaction, int nID)
        {
            string strSQL = MakeLoadUserDefinedTeamQuery(nID.ToString());

            ArrayList arrResults = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResults.Count == 0)
                return null;

            UserDefinedTeam team = ReadUserDefinedTeam(arrResults, 0);

            return team;
        }

        private static UserDefinedTeam ReadUserDefinedTeam(ArrayList arrResults, int nIndex)
        {
            int nID = WebDBManager.GetIntField(arrResults[nIndex].ToString(), -1);
            string strTeamName = WebDBManager.GetStringField(arrResults[nIndex + 1], "");
            string strPhoneNumber = WebDBManager.GetStringField(arrResults[nIndex + 2].ToString(), "");
            string strFaxNumber = WebDBManager.GetStringField(arrResults[nIndex + 3].ToString(), "");
            
            if (nID < 0)
                return null;

            UserDefinedTeam team = new UserDefinedTeam();

            team.TeamID = nID;
            team.TeamName = strTeamName;
            team.PhoneNumber = strPhoneNumber;
            team.FaxNumber = strFaxNumber;

            return team;
        }

        private static string MakeLoadUserDefinedTeamQuery(string strUserDefinedTeamID)
        {
            string strSQL = "select ID, TeamName, PhoneNumber, FaxNumber";
            strSQL += "from UserDefinedTeam";
            strSQL += "where UserDefnedTeam.ID = " + strUserDefinedTeamID;

            return strSQL;
        }

        public static int GetMaxID(DBUtility.WebDBManager dbMgr, string strTableName, int nTransaction)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            return DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        public static int GetJobSubLevel(DBUtility.WebDBManager dbMgr, int nTransaction, CompanyMember.JobLevelSubInfo subLevel)
        {
            if (subLevel == null)
                return -1;

            if (subLevel.ID > 0)
                return subLevel.ID;

            if (subLevel.Name.Length == 0)
                return -1;

            int nID = GetMaxID(dbMgr, "JobSubLevel", nTransaction) + 1;

            if (nID == 0)
                return -1;

            string strSQL = string.Format("Insert into JobSubLevel (ID, Name) values ({0}, '{1}')", nID, subLevel.Name);
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return -1;

            subLevel.ID = nID;
            return nID;
        }

        public static int GetJobSubPosition(DBUtility.WebDBManager dbMgr, int nTransaction, CompanyMember.JobPositionSubInfo subPosition)
        {
            if (subPosition == null)
                return -1;

            if (subPosition.ID > 0)
                return subPosition.ID;

            if (subPosition.Name.Length == 0)
                return -1;

            int nID = GetMaxID(dbMgr, "JobSubPosition", nTransaction) + 1;

            if (nID == 0)
                return -1;

            string strSQL = string.Format("Insert into JobSubPosition (ID, Name) values ({0}, '{1}')", nID, subPosition.Name);
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return -1;

            subPosition.ID = nID;
            return nID;
        }

        public static int GetGroupPosition(DBUtility.WebDBManager dbMgr, int nTransaction, CompanyMember.JobGroupPosition groupPosition)
        {
            if (groupPosition == null)
                return -1;

            if (groupPosition.ID > 0)
                return groupPosition.ID;

            if (groupPosition.Name.Length == 0)
                return -1;

            int nID = GetMaxID(dbMgr, "JobPositionGroup", nTransaction) + 1;

            if (nID == 0)
                return -1;

            string strSQL = string.Format("Insert into JobPositionGroup (ID, Name) values ({0}, '{1}')", nID, groupPosition.Name);
            ArrayList arrResult = nTransaction != 0 ? dbMgr.GetBatchData(strSQL) : dbMgr.GetResultData(strSQL, 0);//dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return -1;

            groupPosition.ID = nID;
            return nID;
        }

        public static void RemoveRegularTeam(RegularTeam team)
        {
            if (team == null)
                return;

            m_dicRegularTeams.Remove(team.TeamID);
            
            RegularTeam deleteTeam = null;
            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_dicTeamCompanyMembers)
            {
                if (item.Key.TeamID == team.TeamID)
                {
                    deleteTeam = item.Key;
                    break;
                }
            }
            if (deleteTeam != null)
                m_dicTeamCompanyMembers.Remove(deleteTeam);
        }

        public static void RemoveCompanyMember(int memberID)
        {
            m_dicCompanyMembers.Remove(memberID);
             
            RegularTeam team = null;
            CompanyMember member = null;             
            bool isChk = false;

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_dicTeamCompanyMembers)
            {
                foreach (CompanyMember item2 in item.Value)
                {
                    if (item2.ID == memberID)
                    {
                        isChk = true;

                        team = item.Key;
                        member = item2;
                        break;
                    }
                }
                if (isChk)
                    break;
            }

            if (team != null && member != null)
                m_dicTeamCompanyMembers[team].Remove(member);
        } 

        public static void RemoveCompanyMember(CompanyMember member)
        {
            if (member == null || member.Team == null)
                return;

            m_dicCompanyMembers.Remove(member.ID);
            
            RegularTeam removeTeam = null;
            CompanyMember removeMember = null;
            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_dicTeamCompanyMembers)
            {
                if (item.Key.TeamID == member.Team.TeamID)
                {
                    removeTeam = item.Key;
                    foreach (CompanyMember item2 in item.Value)
                    {
                        if (item2.ID == member.ID)
                        {
                            removeMember = item2;
                            break;
                        }
                    }
                    break;
                }
            }

            if (removeTeam != null && removeMember != null)
                m_dicTeamCompanyMembers[removeTeam].Remove(removeMember);
        }

        public static void AddCompanyMember(CompanyMember member)
        {
            if (member == null || member.ID < 0)
                return;

            m_dicCompanyMembers[member.ID] = member;
        }

        public static void AddRegularTeam(RegularTeam team)
        {
            if (team == null)
                return;

            m_dicRegularTeams[team.TeamID] = team;
        }
    }

    public class RegularTeamCompare : Comparer<RegularTeam>
    {
        public override int Compare(RegularTeam x, RegularTeam y)
        {
            if (x.TeamID == y.TeamID)
                return 0;

            return -1;            
        }        
    }
}
