using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Import.Common
{
    public class RegularExcelReader : RegularMemberReader, IRegularReader
    {
        protected enum COLUMN_HEADER { MEMBER_ID = 1, MEMBER_NAME, TEAM_PATH, JOB_POSITION, JOB_GROUP, JOB_LEVEL, MOBILE_PHONE, OFFICE_PHONE, TEAM_POSITION };
        protected enum JOB_POSITION { TEAM_MEMBER = 0, TEAM_LEADER, PART_LEADER, 처장, 본부장, LEAVE, UNKNOWN };

        private Dictionary<COLUMN_HEADER, int> m_dicIndices = new Dictionary<COLUMN_HEADER, int>();

        // Key : Column Index
        // Value : 해당 Column에 대한 실제 File 내 Column Index
        private Dictionary<JOB_POSITION, int> m_dicJobPositionID = null;
        // 사번 중복체크용
        private Dictionary<string, string> m_dicMemberID = null;
        // 전화번호 중복체크용
        private Dictionary<string, string> m_dicPhoneNumber = null;

        public bool UpdateAll
        {
            get { return true; }
        }

        public List<Team> NewTemporaryNormalTeams
        {
            get { return null; }
        }

        public List<Team> NewTemporaryEmergencyTeams
        {
            get { return null; }
        }

        public List<Team> RemovingOldTemporaryNormalTeams
        {
            get { return null; }
        }

        public List<Team> RemovingOldTemporaryEmergencyTeams
        {
            get { return null; }
        }

        // 추가될 주간 및 평일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> NewTemporaryNormalMembers
        {
            get { return null; }
        }

        // 추가될 야간 및 휴일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> NewTemporaryEmergencyMembers
        {
            get { return null; }
        }

        // 삭제될 주간 및 평일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> RemovingOldTemporaryNormalMembers
        {
            get { return null; }
        }

        // 삭제될 야간 및 휴일 자위소방대원 목록
        public Dictionary<Team, List<TemporaryMember>> RemovingOldTemporaryEmergencyMembers
        {
            get { return null; }
        }

        public bool FindColumnHeader(string[] tokens)
        {
            int nIndexCount = Enum.GetValues(typeof(COLUMN_HEADER)).Length;

            m_dicIndices.Clear();
            int nTokenCount = tokens.Count();

            for (int i = 0; i < nTokenCount; i++)
            {
                string strToken = tokens[i].Trim();

                if (strToken == "사번")
                    m_dicIndices[COLUMN_HEADER.MEMBER_ID] = i;
                else if (strToken == "성명" || strToken == "이름")
                    m_dicIndices[COLUMN_HEADER.MEMBER_NAME] = i;
                else if (strToken == "소속명" || strToken == "소속")
                    m_dicIndices[COLUMN_HEADER.TEAM_PATH] = i;
                else if (strToken == "직군상세")
                    m_dicIndices[COLUMN_HEADER.JOB_POSITION] = i;
                else if (strToken == "직군명" || strToken == "직군")
                    m_dicIndices[COLUMN_HEADER.JOB_GROUP] = i;
                else if (strToken == "직급명" || strToken == "직급")
                    m_dicIndices[COLUMN_HEADER.JOB_LEVEL] = i;
                else if (strToken == "휴대전화")
                    m_dicIndices[COLUMN_HEADER.MOBILE_PHONE] = i;
                else if (strToken == "근무지전화")
                    m_dicIndices[COLUMN_HEADER.OFFICE_PHONE] = i;
                else if (strToken == "직위명" || strToken == "직위")
                    m_dicIndices[COLUMN_HEADER.TEAM_POSITION] = i;
            }

            // 직위는 양식파일에 없어도 된다.
            if (m_dicIndices.ContainsKey(COLUMN_HEADER.TEAM_POSITION) == false)
                m_dicIndices[COLUMN_HEADER.TEAM_POSITION] = -1;

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
            string strJobPosition = null, strJobGroup = null, strMobilePhoneNumber = null, strOfficePhoneNumber = null;

            // 사번은 필수
            if (dicValues.TryGetValue(COLUMN_HEADER.MEMBER_ID, out strMemberID) == false)
                return false;
            // 성명은 필수
            if (dicValues.TryGetValue(COLUMN_HEADER.MEMBER_NAME, out strName) == false)
                return false;
            // 소속명은 필수
            if (dicValues.TryGetValue(COLUMN_HEADER.TEAM_PATH, out strTeamPath) == false)
                return false;
            // 직급명은 필수
            if (dicValues.TryGetValue(COLUMN_HEADER.JOB_LEVEL, out strJobLevel) == false)
                return false;

            strMemberID = strMemberID.Trim();
            strName = strName.Trim();
            strTeamPath = strTeamPath.Trim();
            strJobLevel = strJobLevel.Trim();

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

            dicValues.TryGetValue(COLUMN_HEADER.JOB_POSITION, out strJobPosition);
            dicValues.TryGetValue(COLUMN_HEADER.JOB_GROUP, out strJobGroup);
            dicValues.TryGetValue(COLUMN_HEADER.MOBILE_PHONE, out strMobilePhoneNumber);
            dicValues.TryGetValue(COLUMN_HEADER.OFFICE_PHONE, out strOfficePhoneNumber);
            dicValues.TryGetValue(COLUMN_HEADER.TEAM_POSITION, out strTeamPosition);

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

            RegularTeam team = GetRegularTeam(dicValues[COLUMN_HEADER.TEAM_PATH]);

            if (team != null)
            {
                CompanyMember.JobPositionSubInfo subPosition = null;
                CompanyMember.JobLevelSubInfo subLevel = null;
                CompanyMember member = new CompanyMember();

                member.MemberID = strMemberID;
                member.Name = strName;
                member.PositionID = GetJobPositionID(strTeamPath, strJobPosition, m_dicJobPositionID, strTeamPosition, ref subPosition);
                member.SubJobPosition = subPosition;
                member.PhoneNumber = GetMobilePhoneNumber(strMobilePhoneNumber);
                member.OfficePhoneNumber = strOfficePhoneNumber;
                member.LevelID = GetJobLevel(strJobLevel, ref subLevel);
                member.SubJobLevel = subLevel;
                member.GroupPosition = GetJobGroup(strJobGroup);
                member.Team = team;

                if (CheckDuplicateMember(member) == false)
                    return false;

                List<CompanyMember> members = null;

                if (!dicRegularMembers.TryGetValue(team, out members))
                {
                    members = new List<CompanyMember>();
                    dicRegularMembers[team] = members;
                }

                members.Add(member);
                return true;
            }

            return false;
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
                else if (strPositionName == "파트장")
                    dicJobPositionID[JOB_POSITION.PART_LEADER] = pair.Key;
                else if (strPositionName == "처장")
                    dicJobPositionID[JOB_POSITION.처장] = pair.Key;
                else if (strPositionName == "본부장")
                    dicJobPositionID[JOB_POSITION.본부장] = pair.Key;
                else if (strPositionName == "휴직")
                    dicJobPositionID[JOB_POSITION.LEAVE] = pair.Key;
                else if (strPositionName == "알 수 없음" || strPositionName == "알수 없음" || strPositionName == "알 수없음" || strPositionName == "알수없음")
                    dicJobPositionID[JOB_POSITION.UNKNOWN] = pair.Key;
            }

            return dicJobPositionID;
        }
    }
}
