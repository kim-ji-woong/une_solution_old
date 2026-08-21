using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using DBUtility2;

namespace PersonalSOP.Models
{
    public class DataManager
    {
        private WebDBManager m_dbMgr = null;

        private static DataManager m_instance = null;

        public static DataManager Instance
        {
            get { return m_instance; }
        }
        
        private DataManager()
        {
            m_dbMgr = Network.NetworkWebManager.Instance.DBMgr;
            m_instance = this;
        }

        public static void InitInstance()
        {
            m_instance = new DataManager();
        }

        private Dictionary<int, List<TemporaryMember>> m_dicTemporaryNormalMemberID = null;
        private Dictionary<int, List<TemporaryMember>> m_dicTemporaryEmergencyMemberID = null;

        // TeamID(TeamType), ... 형태로 되어 있는 strTeamList를 분석하여 Team 이름들을 얻어온다.
        // ex) 1(0), 1(2), 2(3), 5(0)
        public string GetTeamList(string strTeamList)
        {
            int nBeginIndex = 0;
            int nLen = strTeamList.Length;

            string strTeamNameList = "";

            while (nBeginIndex < nLen)
            {
                int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
                if (nDotIndex < 0) break;

                if (!GetTeamName(ref strTeamNameList, strTeamList, nBeginIndex, nDotIndex))
                    return "";

                nBeginIndex = nDotIndex + 1;
            }

            if (!GetTeamName(ref strTeamNameList, strTeamList, nBeginIndex, nLen))
                return "";

            return strTeamNameList;
        }

        private bool GetTeamName(ref string strTeamNameList, string strTeamList, int nBeginIndex, int nEndIndex)
        {
            string strToken = strTeamList.Substring(nBeginIndex, nEndIndex - nBeginIndex);

            int nIndex1 = strTeamList.IndexOf('(', nBeginIndex);
            int nIndex2 = strTeamList.IndexOf(')', nBeginIndex);

            if (nIndex1 < 0 || nIndex2 < 0)
                return false;

            string strTeamID = strTeamList.Substring(nBeginIndex, nIndex1 - nBeginIndex);
            string strTeamType = strTeamList.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

            strTeamID = strTeamID.Trim();
            strTeamType = strTeamType.Trim();

            // TeamID, TeamName
            Dictionary<int, string> dicTeamName = null;
            // TeamID, RegularTeamID List
            //Dictionary<int, ArrayList> dicRegualrTeamID = null;
            string strTeamName = null;
            ArrayList arrLinkedMembers = new ArrayList();

            bool includeChildTeams = true;
            int nTeamID;

            if (!int.TryParse(strTeamID, out nTeamID))
                return false;

            if (nTeamID < 0)
            {
                nTeamID = -nTeamID;
                includeChildTeams = false;
            }

            if (strTeamType == "0")
            {
                //if (dicNormal == null)
                //{
                //    dicNormal = new Dictionary<int, string>();
                //    m_dicTemporaryNormalMemberID = new Dictionary<int, List<TemporaryMember>>();
                //    ReadTeamList(dbMgr, "TemporaryNormalTeam", true, dicNormal, ref m_dicTemporaryNormalMemberID);
                //}

                //dicTeamName = dicNormal;

                if (m_dicTemporaryNormalMemberID == null)
                    m_dicTemporaryNormalMemberID = new Dictionary<int, List<TemporaryMember>>();
                else
                {
                    List<TemporaryMember> members;

                    if (m_dicTemporaryNormalMemberID.TryGetValue(nTeamID, out members))
                    {
                        arrLinkedMembers.AddRange(members);
                    }
                }
                //dicRegualrTeamID = m_dicNormalRegularTeamID;
            }
            else if (strTeamType == "1")
            {
                //if (dicEmergency == null)
                //{
                //    dicEmergency = new Dictionary<int, string>();
                //    m_dicTemporaryEmergencyMemberID = new Dictionary<int, List<TemporaryMember>>();
                //    ReadTeamList(dbMgr, "TemporaryEmergencyTeam", false, dicEmergency, ref m_dicTemporaryEmergencyMemberID);
                //}

                //dicTeamName = dicEmergency;

                if (m_dicTemporaryEmergencyMemberID == null)
                    m_dicTemporaryEmergencyMemberID = new Dictionary<int, List<TemporaryMember>>();
                else
                {
                    List<TemporaryMember> members;

                    if (m_dicTemporaryEmergencyMemberID.TryGetValue(nTeamID, out members))
                    {
                        arrLinkedMembers.AddRange(members);
                    }
                }
                //dicRegualrTeamID = m_dicEmergencyRegularTeamID;
            }
            else if (strTeamType == "2")
            {
                //if (!dicExternal.ContainsKey(nTeamID))
                //    return false;

                //strTeamName = dicExternal[nTeamID].TeamName;
            }
            else if (strTeamType == "3")
            {
                //if (dicUserDefined == null)
                //{
                //    dicUserDefined = new Dictionary<int, string>();
                //    ReadTeamList(dbMgr, "UserDefinedTeam", dicUserDefined);
                //}

                //dicTeamName = dicUserDefined;
            }
            else if (strTeamType == "4")
            {
                //if (dicRegular == null)
                //{
                //    dicRegular = new Dictionary<int, string>();
                //    ReadTeamList(dbMgr, "RegularTeam", dicRegular);
                //}

                //dicTeamName = dicRegular;
            }
            else if (strTeamType == "10")
            {
                //if (dicControlRoom == null)
                //{
                //    dicControlRoom = new Dictionary<int, string>();
                //    ReadTeamList(dbMgr, "ControlRoom", dicControlRoom);
                //}

                //dicTeamName = dicControlRoom;
            }
            else
                return false;

            if (strTeamName == null)
            {
                if (!dicTeamName.ContainsKey(nTeamID))
                    return false;

                strTeamName = dicTeamName[nTeamID];
            }

            if (strTeamNameList.Length == 0)
                strTeamNameList = strTeamName;
            else
                strTeamNameList += ", " + strTeamName;

            int nLevelNo = GetLevelNumber(nTeamID, strTeamType);
            Models.SOPTeam team = new Models.SOPTeam();

            team.TeamID = nTeamID;
            team.TeamType = (Models.SOPTeam.SOPTeamType)int.Parse(strTeamType);
            team.TeamName = strTeamName;
            team.LevelNo = nLevelNo;
            team.LinkedMembers = arrLinkedMembers;

            if (team.TeamType == Models.SOPTeam.SOPTeamType.Regular || team.TeamType == Models.SOPTeam.SOPTeamType.External ||
                team.TeamType == Models.SOPTeam.SOPTeamType.Normal || team.TeamType == Models.SOPTeam.SOPTeamType.Holiday)
                team.IncludeChildTeams = includeChildTeams;

            //if (sectionData is Models.SectionDataProcess)
            //{
            //    ((Models.SectionDataProcess)sectionData).TeamList.Add(team);
            //}
            //else if (sectionData is Models.SectionDataInternal)
            //{
            //    ((Models.SectionDataInternal)sectionData).TeamList.Add(team);
            //}

            return true;
        }

        private bool ReadTemporayTeam(string tableName, int teamID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Select MemberType, MemberID ");
            sb.AppendFormat("  From {0} as t, TemporaryMemberList as tl ", tableName);
            sb.Append(" Where t.ID = tl.TemporaryTeamID ");
            sb.AppendFormat("And t.ID = {0} ", teamID);

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count == 0)
                return false;

            for (int i = 0; i < arrResult.Count; i += 2)
            {
                // 0(RegularTeam), 1(CompanyMember), 2(ExternalCompanyTeam), 3(ExternalTeam), 4(ExternalCompanyMember), 
                // 5(UserDefinedTeam), 6(직급, ID가 1이면 1직급, 2면 2직급 모두를 의미)
                int nMemberType = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = DBUtility2.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);


            }

            return false;
        }
                
        private bool ReadMember(string tableName, int memberID)
        {
            return false;
        }

        // dicTeamName : TeamID, TeamName
        private bool ReadTeamList(WebDBManager dbMgr, string strTableName, bool isNormal, Dictionary<int, string> dicTeamName, ref Dictionary<int, List<TemporaryMember>> dicTemporaryMembers)
        {
            //string strSQL = "select id, TeamName, RegularTeamLink from " + strTableName;
            //ArrayList arrResult = dbMgr.GetResultData(strSQL);

            //if (arrResult == null)
            //	return false;

            string strFormat = "select team.ID, TeamName, link.MemberID, link.MemberType, link.IsTeamLeader, link.Role, link.MemberName from {0} as team, TemporaryMemberList as link ";
            strFormat += "where link.TemporaryTeamID = team.ID and link.IsNormal = {1} and team.SiteID = {2}";

            string strSQL = string.Format(strFormat, strTableName, isNormal ? 1 : 0, m_dbMgr.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            List<TemporaryMember> members;
            TemporaryMember.MemberType memberType;
            TemporaryMember.RoleType roleType;

            int nResultCount = arrResult.Count;

            List<int> teamIDs = new List<int>();

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nTeamLeader = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nRoleType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                if (nTeamID < 0 || nMemberID < 0)
                    continue;

                if (!TemporaryMember.GetMemberType(nMemberType, out memberType))
                    continue;

                if (!TemporaryMember.GetRoleType(nRoleType, out roleType))
                    continue;

                if (strMemberName == "null")
                    strMemberName = "";

                if (!teamIDs.Contains(nTeamID))
                    teamIDs.Add(nTeamID);

                dicTeamName[nTeamID] = strTeamName;

                if (!dicTemporaryMembers.TryGetValue(nTeamID, out members))
                {
                    members = new List<TemporaryMember>();
                    dicTemporaryMembers[nTeamID] = members;
                }

                TemporaryMember member = new TemporaryMember(nTeamID, isNormal, nMemberID, nTeamLeader, memberType, roleType, strMemberName);
                members.Add(member);
            }

            strSQL = "select ID, TeamName from TemporaryNormalTeam where SiteID = " + m_dbMgr.SiteID;

            if (teamIDs.Count > 0)
            {
                strSQL += " and ID not in (";

                string strIDs = "";

                foreach (int nID in teamIDs)
                {
                    if (strIDs.Length == 0)
                        strIDs = nID.ToString();
                    else
                        strIDs += ", " + nID.ToString();
                }

                strSQL += strIDs + ")";
            }

            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                dicTeamName[nTeamID] = strTeamName;
            }
            return true;
        }

        // dicTeamName : TeamID, TeamName
        private bool ReadTeamList(WebDBManager dbMgr, string strTableName, Dictionary<int, string> dicTeamName)
        {
            if (strTableName == "RegularTeam")
            {
                // Site별로 사용할 수 있도록 수정 , Edit by skkim 2015.01.09
                // SiteID로 본부 아이디를 가져온다.
                string szSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", m_dbMgr.SiteID);
                ArrayList arrResult1 = dbMgr.GetResultData(szSQL);
                if (arrResult1 == null || arrResult1.Count == 0)
                    return false;

                int nTopTeamID = WebDBManager.GetIntField(arrResult1[0].ToString(), -1);
                if (nTopTeamID == -1)
                    return false;

                ArrayList arrResult = ExecuteTeamList(dbMgr, nTopTeamID);
                //string strSQL = string.Format("sp_TeamList2 {0}", nTopTeamID);
                //ArrayList arrResult = dbMgr.GetStoredProcedureData(strSQL, 0);
                for (int i = 0; i < arrResult.Count - 2; i += 3)
                {
                    int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                    dicTeamName[nTeamID] = strTeamName;
                }
            }
            else if (strTableName == "ControlRoom")
            {
                //Dictionary<int, Data_ControlRoom> dicControlRoom = FormSOP.Instance.SOPManager.ControlRoom;

                //foreach (KeyValuePair<int, Data_ControlRoom> item in dicControlRoom)
                //{
                //    if (!dicTeamName.ContainsKey(item.Value.ID))
                //        dicTeamName.Add(item.Value.ID, item.Value.TeamName);
                //    else
                //        dicTeamName[item.Value.ID] = item.Value.TeamName;
                //}
            }
            else
            {
                string strSQL = "select id, TeamName from " + strTableName;
                strSQL += " WHERE SiteID = " + m_dbMgr.SiteID.ToString();

                ArrayList arrResult = dbMgr.GetResultData(strSQL);
                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    int nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                    dicTeamName[nTeamID] = strTeamName;
                }
            }
            return true;
        }

        private int GetLevelNumber(int nTeamID, string strTeamType)
        {
            int nLevelNo = -1;
            string strSQL = "";

            if (strTeamType == "0")
            {
                //strSQL = "select ID, TeamName, LevelNo from TemporaryNormalTeam where ID = " + nTeamID.ToString();
                strSQL = "select team.ID, TeamName, link.MemberID from TemporaryNormalTeam as team, TemporaryMemberList as link where team.ID = link.TemporaryTeamID and link.IsNormal = 1 and link.MemberType = 6 and team.ID = {0} and SiteID = {1}";
                strSQL = string.Format(strSQL, nTeamID, m_dbMgr.SiteID);
            }
            else if (strTeamType == "1")
            {
                //strSQL = "select ID, TeamName, LevelNo from TemporaryEmergencyTeam where ID = " + nTeamID.ToString();
                strSQL = "select team.ID, TeamName, link.MemberID from TemporaryEmergencyTeam as team, TemporaryMemberList as link where team.ID = link.TemporaryTeamID and link.IsNormal = 0 and link.MemberType = 6 and team.ID = {0} and SiteID = {1}";
                strSQL = string.Format(strSQL, nTeamID, m_dbMgr.SiteID);
            }
            else
                return nLevelNo;

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                nLevelNo = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
            }

            return nLevelNo;
        }

        public static ArrayList ExecuteTeamList(WebDBManager dbMgr, int nRootTeamID, string strTableName = "RegularTeam")
        {
            string strSQL = "Select ID, TeamName, ParentTeamID from " + strTableName + " order by ParentTeamID, ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            if (nRootTeamID == 0)
                return arrResult;

            int nResultCount = arrResult.Count;

            ArrayList arrNewResult = new ArrayList();
            Dictionary<int, int> dicParentID = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                if (dicParentID.Count == 0)
                {
                    if (nID == nRootTeamID)
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
                else
                {
                    if (parentID == null)
                        continue;

                    if (dicParentID.ContainsKey(parentID.Data))
                    {
                        dicParentID[nID] = nID;

                        arrNewResult.Add(arrResult[i]);
                        arrNewResult.Add(arrResult[i + 1]);
                        arrNewResult.Add(arrResult[i + 2]);
                    }
                }
            }

            return arrNewResult;
        }
    }

    public class TemporaryMember
    {
        public enum MemberType
        {
            RegularTeam = 0,
            CompanyMember,
            ExternalCompanyTeam,    // 외부 협력사의 팀
            ExternalTeam,           // 외부 협력사
            ExternalCompanyMember,
            UserDefinedTeam,
            JobLevel,               // 직급, 1이면 1직급, 2면 2직급
            Unknown
        }

        //역할 : 0(정), 1(부), 2(팀장), 3(일반)
        public enum RoleType { Main = 0, Sub, TeamLeader, General, Unknown };

        private int m_nTemporaryTeamID = -1;
        private bool m_isNormal = true;
        private int m_nMemberID = -1;
        // 1이면 팀장, 0이면 팀원이며 0보다 작으면 null 값이다.
        private int m_nTeamLeader = -1;
        private MemberType m_memberType = MemberType.Unknown;
        private RoleType m_roleType = RoleType.Unknown;
        private string m_strMemberName = "";
        // 하위팀을 포함하는가?
        private bool m_includeChildTeams = true;

        public int TemporaryTeamID
        {
            get { return m_nTemporaryTeamID; }
            set { m_nTemporaryTeamID = value; }
        }

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        // 1이면 팀장, 0이면 팀원이며 0보다 작으면 null 값이다.
        public int TeamLeader
        {
            get { return m_nTeamLeader; }
            set { m_nTeamLeader = value; }
        }

        public MemberType _MemberType
        {
            get { return m_memberType; }
            set { m_memberType = value; }
        }

        public RoleType _RoleType
        {
            get { return m_roleType; }
            set { m_roleType = value; }
        }

        public string MemberName
        {
            get { return m_strMemberName; }
            set { m_strMemberName = value; }
        }

        public bool IncludeChildTeams
        {
            get { return m_includeChildTeams; }
            set { m_includeChildTeams = value; }
        }

        public TemporaryMember()
        {
        }

        public TemporaryMember(int nTemporaryTeamID, bool isNormal, int nMemberID, int nTeamLeader, MemberType memberType, RoleType roleType, string strMemberName)
        {
            m_nTemporaryTeamID = nTemporaryTeamID;
            m_isNormal = isNormal;
            m_nMemberID = nMemberID;
            m_nTeamLeader = nTeamLeader;
            m_memberType = memberType;
            m_roleType = roleType;
            m_strMemberName = strMemberName;
        }

        public static bool GetMemberType(int nMemberType, out MemberType memberType)
        {
            if (nMemberType < 0 || nMemberType >= (int)MemberType.Unknown)
            {
                memberType = MemberType.Unknown;
                return false;
            }

            memberType = (MemberType)nMemberType;
            return true;
        }

        public static bool GetRoleType(int nRoleType, out RoleType roleType)
        {
            if (nRoleType < 0 || nRoleType >= (int)RoleType.Unknown)
            {
                roleType = RoleType.Unknown;
                return false;
            }

            roleType = (RoleType)nRoleType;
            return true;
        }

        public static string GetRoleTypeString(RoleType roleType)
        {
            if (roleType == RoleType.Main)
                return "정";
            else if (roleType == RoleType.Sub)
                return "부";

            return "";
        }
    }
}