using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Import.Site._201
{
    public class Parc1RegularExcelReader : RegularMemberReader, IRegularReader
    {
        protected enum COLUMN_HEADER { TEAM_PATH = 1, BUILDING_NAME, FLOOR_NAME, MEMBER_NAME, JOB_LEVEL, TEAM_POSITION, MEMBER_ID, MOBILE_PHONE }
        protected enum JOB_POSITION { TEAM_MEMBER = 0, TEAM_LEADER, UNKNOWN };

        private const string FLOOR_MANAGER = "층관리자";

        private Dictionary<COLUMN_HEADER, int> m_dicIndices = new Dictionary<COLUMN_HEADER, int>();

        // 추가될 주간 및 평일 자위소방대 목록
        // Key : Team별 전체경로
        private Dictionary<string, Team> m_dicNewTemporaryNormalTeams = new Dictionary<string, Team>();
        // 추가될 야간 및 휴일 자위소방대 목록
        // Key : Team별 전체경로
        private Dictionary<string, Team> m_dicNewTemporaryEmergencyTeams = new Dictionary<string, Team>();

        // 추가될 주간 및 평일 자위소방대원 목록
        private Dictionary<Team, List<TemporaryMember>> m_dicNewTemporaryNormalMembers = new Dictionary<Team, List<TemporaryMember>>();
        // 추가될 야간 및 휴일 자위소방대원 목록
        private Dictionary<Team, List<TemporaryMember>> m_dicNewTemporaryEmergencyMembers = new Dictionary<Team, List<TemporaryMember>>();

        // 삭제될 주간 및 평일 자위소방대원 목록
        private Dictionary<Team, List<TemporaryMember>> m_dicRemovingOldTemporaryNormalMembers = new Dictionary<Team, List<TemporaryMember>>();
        // 삭제될 야간 및 휴일 자위소방대원 목록
        private Dictionary<Team, List<TemporaryMember>> m_dicRemovingOldTemporaryEmergencyMembers = new Dictionary<Team, List<TemporaryMember>>();

        // Key : Column Index
        // Value : 해당 Column에 대한 실제 File 내 Column Index
        private Dictionary<JOB_POSITION, int> m_dicJobPositionID = null;
        // 사번 중복체크용
        private Dictionary<string, string> m_dicMemberID = null;
        // 전화번호 중복체크용
        private Dictionary<string, string> m_dicPhoneNumber = null;

        public bool UpdateAll
        {
            get { return false; }
        }

        public List<Team> NewTemporaryNormalTeams
        {
            get { return m_dicNewTemporaryNormalTeams.Values.ToList(); }
        }

        public List<Team> NewTemporaryEmergencyTeams
        {
            get { return m_dicNewTemporaryEmergencyTeams.Values.ToList(); }
        }

        public List<Team> RemovingOldTemporaryNormalTeams
        {
            get { return new List<Team>(); }
        }

        public List<Team> RemovingOldTemporaryEmergencyTeams
        {
            get { return new List<Team>(); }
        }

        // 추가될 주간 및 평일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> NewTemporaryNormalMembers
        {
            get { return m_dicNewTemporaryNormalMembers; }
        }

        // 추가될 야간 및 휴일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> NewTemporaryEmergencyMembers
        {
            get { return m_dicNewTemporaryEmergencyMembers; }
        }

        // 삭제될 주간 및 평일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> RemovingOldTemporaryNormalMembers
        {
            get { return m_dicRemovingOldTemporaryNormalMembers; }
        }

        // 삭제될 야간 및 휴일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> RemovingOldTemporaryEmergencyMembers
        {
            get { return m_dicRemovingOldTemporaryEmergencyMembers; }
        }

        public bool FindColumnHeader(string[] tokens)
        {
            int nIndexCount = Enum.GetValues(typeof(COLUMN_HEADER)).Length;

            m_dicIndices.Clear();
            int nTokenCount = tokens.Count();

            for (int i = 0; i < nTokenCount; i++)
            {
                string strToken = tokens[i].Trim();

                if (strToken == "소속")
                    m_dicIndices[COLUMN_HEADER.TEAM_PATH] = i;
                else if (strToken == "건물")
                    m_dicIndices[COLUMN_HEADER.BUILDING_NAME] = i;
                else if (strToken == "층")
                    m_dicIndices[COLUMN_HEADER.FLOOR_NAME] = i;
                else if (strToken == "이름")
                    m_dicIndices[COLUMN_HEADER.MEMBER_NAME] = i;
                else if (strToken == "직급")
                    m_dicIndices[COLUMN_HEADER.JOB_LEVEL] = i;
                else if (strToken == "직위")
                    m_dicIndices[COLUMN_HEADER.TEAM_POSITION] = i;
                else if (strToken == "사번")
                    m_dicIndices[COLUMN_HEADER.MEMBER_ID] = i;
                else if (strToken == "휴대전화")
                    m_dicIndices[COLUMN_HEADER.MOBILE_PHONE] = i;
            }

            if (m_dicIndices.Count < nIndexCount)
                return false;

            return true;
        }

        public bool ReadRegularMember(string[] tokens, Dictionary<RegularTeam, List<CompanyMember>> dicRegularMembers)
        {
            if (m_dicJobPositionID == null)
            {
                m_dicJobPositionID = ReadJobPositions();
                m_dicMemberID = new Dictionary<string, string>();
                m_dicPhoneNumber = new Dictionary<string, string>();
            }

            int nTokenCount = tokens.Count();
            Dictionary<COLUMN_HEADER, string> dicValues = new Dictionary<COLUMN_HEADER, string>();

            foreach (KeyValuePair<COLUMN_HEADER, int> pair in m_dicIndices)
            {
                if (pair.Value >= nTokenCount)
                    continue;

                if (pair.Value >= 0)
                    dicValues[pair.Key] = tokens[pair.Value];
            }

            string strMemberID = null, strName = null, strTeamPath = null, strJobLevel = null, strTeamPosition = null;
            string strBuildingName = null, strFloorName = null, strMobilePhoneNumber = null;

            // 사번은 필수(사번은 출입카드 번호)
            if (dicValues.TryGetValue(COLUMN_HEADER.MEMBER_ID, out strMemberID) == false)
                return false;
            // 성명은 필수
            if (dicValues.TryGetValue(COLUMN_HEADER.MEMBER_NAME, out strName) == false)
                return false;
            // 소속명은 필수
            if (dicValues.TryGetValue(COLUMN_HEADER.TEAM_PATH, out strTeamPath) == false)
                return false;

            strMemberID = strMemberID.Trim();
            strName = strName.Trim();
            strTeamPath = strTeamPath.Trim();

            if (strName.Length == 0)
                return false;

            if (strMemberID.Length > 0)
            {
                // 사번 중복
                if (m_dicMemberID.ContainsKey(strMemberID))
                    strMemberID = "";
                else
                    m_dicMemberID[strMemberID] = strMemberID;
            }

            dicValues.TryGetValue(COLUMN_HEADER.BUILDING_NAME, out strBuildingName);
            dicValues.TryGetValue(COLUMN_HEADER.FLOOR_NAME, out strFloorName);
            dicValues.TryGetValue(COLUMN_HEADER.MOBILE_PHONE, out strMobilePhoneNumber);
            dicValues.TryGetValue(COLUMN_HEADER.TEAM_POSITION, out strTeamPosition);
            dicValues.TryGetValue(COLUMN_HEADER.JOB_LEVEL, out strJobLevel);

            if (strMobilePhoneNumber != null)
            {
                strMobilePhoneNumber = strMobilePhoneNumber.Trim();

                if (strMobilePhoneNumber.Length > 0)
                {
                    // 휴대전화번호 중복
                    if (m_dicPhoneNumber.ContainsKey(strMobilePhoneNumber))
                        strMobilePhoneNumber = "";
                    else
                        m_dicPhoneNumber[strMobilePhoneNumber] = strMobilePhoneNumber;
                }
            }

            Team teamNormal = null, teamEmergency = null;

            if (strBuildingName != null && strFloorName != null && strBuildingName.Length > 0 && strFloorName.Length > 0)
            {
                AddTemporaryTeam(strBuildingName, strFloorName, ref teamNormal, ref teamEmergency);
            }

            RegularTeam team = GetRegularTeam(dicValues[COLUMN_HEADER.TEAM_PATH]);

            if (team != null)
            {
                CompanyMember.JobPositionSubInfo subPosition = null;
                CompanyMember.JobLevelSubInfo subLevel = null;
                CompanyMember member = new CompanyMember();

                member.MemberID = strMemberID;
                member.Name = strName;
                member.PositionID = GetJobPositionID(strTeamPath, strTeamPosition, m_dicJobPositionID, strTeamPosition, ref subPosition);
                member.SubJobPosition = subPosition;
                member.PhoneNumber = GetMobilePhoneNumber(strMobilePhoneNumber);
                member.LevelID = GetJobLevel(strJobLevel, ref subLevel);
                member.SubJobLevel = subLevel;
                member.Team = team;

                if (CheckDuplicateMember(member) == false)
                    return false;

                List<CompanyMember> members = null;

                if (!dicRegularMembers.TryGetValue(team, out members))
                {
                    members = new List<CompanyMember>();
                    dicRegularMembers[team] = members;
                }

                RemoveTemporaryMember(teamNormal, m_dicRemovingOldTemporaryNormalMembers, true);
                RemoveTemporaryMember(teamEmergency, m_dicRemovingOldTemporaryEmergencyMembers, false);

                AddTemporaryMember(teamNormal, true, member);
                AddTemporaryMember(teamEmergency, false, member);

                members.Add(member);
                return true;
            }

            return false;
        }

        private void RemoveTemporaryMember(Team team, Dictionary<Team, List<TemporaryMember>> dicRemovingOldTemporaryMembers, bool isNormal)
        {
            if (team == null)
                return;

            List<TemporaryMember> members = null;

            if (dicRemovingOldTemporaryMembers.TryGetValue(team, out members))
            {
                // 삭제는 한번만 한다.
                return;
            }

            members = new List<TemporaryMember>();
            dicRemovingOldTemporaryMembers[team] = members;

            List<TemporaryMember> removingMembers = null;

            if (isNormal)
            {
                removingMembers = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)team);
            }
            else
            {
                removingMembers = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)team);
            }

            if (removingMembers != null)
            {
                members.AddRange(removingMembers);
                removingMembers.Clear();
            }
        }

        private void AddTemporaryMember(Team team, bool isNormal, CompanyMember regularMember)
        {
            if (team == null)
                return;

            List<TemporaryMember> members = null;

            if (isNormal)
            {
                if (m_dicNewTemporaryNormalMembers.TryGetValue(team, out members) == false)
                {
                    members = new List<TemporaryMember>();
                    m_dicNewTemporaryNormalMembers[team] = members;
                }

                TemporaryNormalMember member = new TemporaryNormalMember();
                member.CompanyMember = regularMember;
                member.DisplayName = "층 관리자";
                member.Team = regularMember.Team;
                member.TemporaryMemberType = TemporaryMember.MemberType.CompanyMember;

                members.Add(member);

                /*List<TemporaryMember> _members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)team);

                if (_members != null)
                    _members.Add(member);
                else
                {
                    _members = new List<TemporaryMember>();
                    _members.Add(member);
                    DataManager.SetTemporaryNormalMembers((TemporaryNormalTeam)team, _members);
                }*/
            }
            else
            {
                if (m_dicNewTemporaryEmergencyMembers.TryGetValue(team, out members) == false)
                {
                    members = new List<TemporaryMember>();
                    m_dicNewTemporaryEmergencyMembers[team] = members;
                }

                TemporaryEmergencyMember member = new TemporaryEmergencyMember();
                member.CompanyMember = regularMember;
                member.DisplayName = "층 관리자";
                member.Team = regularMember.Team;
                member.TemporaryMemberType = TemporaryMember.MemberType.CompanyMember;

                members.Add(member);

                /*List<TemporaryMember> _members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)team);

                if (_members != null)
                    _members.Add(member);
                else
                {
                    _members = new List<TemporaryMember>();
                    _members.Add(member);
                    DataManager.SetTemporaryEmergencyMembers((TemporaryEmergencyTeam)team, _members);
                }*/
            }
        }

        private void AddTemporaryTeam(string strBuildingName, string strFloorName, ref Team teamNormal, ref Team teamEmergency)
        {
            AddTemporaryTeam(strBuildingName, strFloorName, m_dicNewTemporaryNormalTeams, true, ref teamNormal);
            AddTemporaryTeam(strBuildingName, strFloorName, m_dicNewTemporaryEmergencyTeams, false, ref teamEmergency);
        }

        private bool AddTemporaryTeam(string strBuildingName, string strFloorName, Dictionary<string, Team> dicNewTemporaryTeams, bool isNormal, ref Team teamFloor)
        {
            Team teamBuilding = ReadTemporaryBuildingTeam(strBuildingName, isNormal);
            Team teamFloorParent = null;

            if (teamBuilding != null)
            {
                string strPath = strBuildingName + "/" + FLOOR_MANAGER;

                if (isNormal)
                    teamFloorParent = DataManager.GetTemporaryNormalTeam(FLOOR_MANAGER, (TemporaryNormalTeam)teamBuilding);
                else
                    teamFloorParent = DataManager.GetTemporaryEmergencyTeam(FLOOR_MANAGER, (TemporaryEmergencyTeam)teamBuilding);

                if (teamFloorParent == null)
                {
                    if (dicNewTemporaryTeams.TryGetValue(strPath, out teamFloorParent) == false)
                    {
                        teamFloorParent = AddTemporaryTeam(teamBuilding, FLOOR_MANAGER, isNormal);

                        if (teamFloorParent == null)
                            return false;
                        else
                        {
                            // TeamID가 0보다 작을 경우(아직 DB에 저장되지 않은 경우)에는 DataManager에 넣을수 없다.
                            //DataManager.AddTeam(teamFloorParent);
                            dicNewTemporaryTeams[strPath] = teamFloorParent;
                        }
                    }
                }

                if (teamFloorParent != null)
                {
                    string strValidFloorName = GetValidFloorName(strFloorName);

                    if (strValidFloorName == null)
                        return false;

                    strPath += "/" + strValidFloorName;

                    if (dicNewTemporaryTeams.TryGetValue(strPath, out teamFloor) == false)
                    {
                        if (isNormal)
                            teamFloor = DataManager.GetTemporaryNormalTeam(strValidFloorName, (TemporaryNormalTeam)teamFloorParent);
                        else
                            teamFloor = DataManager.GetTemporaryEmergencyTeam(strValidFloorName, (TemporaryEmergencyTeam)teamFloorParent);
                    }

                    if (teamFloor == null)
                    {
                        teamFloor = AddTemporaryTeam(teamFloorParent, strValidFloorName, isNormal);

                        if (teamFloor == null)
                            return false;
                        else
                        {
                            // TeamID가 0보다 작을 경우(아직 DB에 저장되지 않은 경우)에는 DataManager에 넣을수 없다.
                            //DataManager.AddTeam(teamFloor);
                            dicNewTemporaryTeams[strPath] = teamFloor;
                        }
                    }

                    if (teamFloor != null)
                        return true;
                }
            }

            return false;
        }

        private Team AddTemporaryTeam(Team teamParent, string strTeamName, bool isNormal)
        {
            /*string strTableName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strParentTeamID = teamParent == null ? "NULL" : teamParent.TeamID.ToString();

            WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = string.Format("Insert into {0} (ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink, SiteID) (Select ISNULL(max(ID) + 1, 1), '{1}', {2}, NULL, NULL, NULL, NULL, {3} from {0})",
                strTableName, strTeamName, strParentTeamID, dbMgr.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            strParentTeamID = teamParent == null ? "is " + strParentTeamID : "= " + strParentTeamID;
            strSQL = string.Format("Select ID from {0} where ParentTeamID {1} and TeamName = '{2}'", strTableName, strParentTeamID, strTeamName);
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return null;*/

            Team team = null;

            if (isNormal)
            {
                TemporaryNormalTeam _team = new TemporaryNormalTeam();
                _team.ParentTeam = (TemporaryNormalTeam)teamParent;
                team = _team;
            }
            else
            {
                TemporaryEmergencyTeam _team = new TemporaryEmergencyTeam();
                _team.ParentTeam = (TemporaryEmergencyTeam)teamParent;
                team = _team;
            }

            //team.TeamID = id.Data;
            team.TeamName = strTeamName;
            return team;
        }

        private string GetValidFloorName(string strFloorName)
        {
            bool onGround = true;

            if (strFloorName.StartsWith("지하") || strFloorName.StartsWith("B"))
                onGround = false;

            int len = strFloorName.Length;
            int nBeginIndex = -1;
            int nFloorIndex = -1;

            for (int i=0;i<len;i++)
            {
                char ch = strFloorName.ElementAt(i);

                if (nBeginIndex < 0)
                {
                    if (ch >= '0' && ch <= '9')
                        nBeginIndex = i;
                }
                else
                {
                    if (ch < '0' || ch > '9')
                    {
                        string strFloor = strFloorName.Substring(nBeginIndex, i - nBeginIndex);
                        nFloorIndex = int.Parse(strFloor);
                        break;
                    }
                }
            }

            if (nBeginIndex < 0)
                return null;

            if (nFloorIndex < 0)
            {
                string strFloor = strFloorName.Substring(nBeginIndex).Trim();
                nFloorIndex = int.Parse(strFloor);
            }

            if (onGround)
                return string.Format("{0}층", nFloorIndex);

            return string.Format("지하 {0}층", nFloorIndex);
        }

        private Team ReadTemporaryBuildingTeam(string strBuildingName, bool isNormal)
        {
            if (isNormal)
            {
                List<TemporaryNormalTeam> teams = DataManager.GetTemporaryNormalRootTeams();

                if (teams == null)
                    return null;

                foreach (TemporaryNormalTeam team in teams)
                {
                    if (team.TeamName == strBuildingName)
                        return team;
                }
            }
            else
            {
                List<TemporaryEmergencyTeam> teams = DataManager.GetTemporaryEmergencyRootTeams();

                if (teams == null)
                    return null;

                foreach (TemporaryEmergencyTeam team in teams)
                {
                    if (team.TeamName == strBuildingName)
                        return team;
                }
            }

            return null;
        }

        private int GetJobPositionID(string strTeamPath, string strJobPosition, Dictionary<JOB_POSITION, int> dicJobPositionID, string strTeamPosition, ref CompanyMember.JobPositionSubInfo subJob)
        {
            int nPositionID = -1;

            if (strJobPosition != null)
            {
                strJobPosition = strJobPosition.Trim();

                if (strJobPosition.Length > 0)
                {
                    subJob = CompanyMember.JobPositionSubInfo.GetSubPosition(strJobPosition);

                    if (subJob == null)
                    {
                        subJob = new CompanyMember.JobPositionSubInfo();
                        subJob.Name = strJobPosition;
                    }
                }
            }

            if (strTeamPosition != null && strTeamPosition == "팀장")
                dicJobPositionID.TryGetValue(JOB_POSITION.TEAM_LEADER, out nPositionID);
            else
                dicJobPositionID.TryGetValue(JOB_POSITION.TEAM_MEMBER, out nPositionID);

            if (nPositionID < 0)
                dicJobPositionID.TryGetValue(JOB_POSITION.UNKNOWN, out nPositionID);

            if (nPositionID < 0)
                nPositionID = 0;

            return nPositionID;
        }

        protected Dictionary<JOB_POSITION, int> ReadJobPositions()
        {
            Dictionary<JOB_POSITION, int> dicJobPositionID = new Dictionary<JOB_POSITION, int>();

            foreach (KeyValuePair<int, string> pair in DataManager.JobPositions)
            {
                string strPositionName = pair.Value;

                if (strPositionName == "팀원")
                    dicJobPositionID[JOB_POSITION.TEAM_MEMBER] = pair.Key;
                else if (strPositionName == "팀장")
                    dicJobPositionID[JOB_POSITION.TEAM_LEADER] = pair.Key;
                else// if (strPositionName == "알 수 없음" || strPositionName == "알수 없음" || strPositionName == "알 수없음" || strPositionName == "알수없음")
                    dicJobPositionID[JOB_POSITION.UNKNOWN] = pair.Key;
            }

            return dicJobPositionID;
        }
    }
}
