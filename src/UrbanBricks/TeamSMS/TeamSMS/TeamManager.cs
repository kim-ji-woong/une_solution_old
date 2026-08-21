using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sections;
using DBUtility2;
using System.Collections;
using Sections;
using UnE.SOP;

namespace TeamSMS
{
    public class TeamManager
    {
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public List<string> GetTeamPhoneNumbers(string strTeamName, bool isNormal, bool includeChildTeams, WebDBManager dbMgr)
        {
            List<string> phoneNumbers = FindTemporaryTeam(strTeamName, isNormal, includeChildTeams, dbMgr);

            if (phoneNumbers != null)
                return phoneNumbers;

            return null;
        }

        private List<string> FindTemporaryTeam(string strTeamName, bool isNormal, bool includeChildTeams, WebDBManager dbMgr)
        {
            string strTableName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strSQL = "Select ID, RegularTeamLink from " + strTableName + " where SiteID = " + dbMgr.SiteID.ToString() + " and TeamName = '" + strTeamName + "'";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            string strRegularTeamLink = WebDBManager.GetStringField(arrResult[1]);

            if (id == null)
                return null;

            string strTeamIDs = id.Data.ToString();

            if (includeChildTeams)
            {
                List<int> teamIDs = new List<int>();
                GetTemporaryTeamIDs(id.Data, isNormal, dbMgr, teamIDs);

                foreach (int nTeamID in teamIDs)
                {
                    strTeamIDs += ", " + nTeamID.ToString();
                }
            }

            List<TemporaryMember> members = ReadTemporaryMembers(strTeamIDs, isNormal, dbMgr);
            Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();

            AddTemporaryMemberPhoneNumbers(dicPhoneNumbers, members, dbMgr);
            return dicPhoneNumbers.Keys.ToList();
        }

        private void AddTemporaryMemberPhoneNumbers(Dictionary<string, string> dicPhoneNumbers, List<TemporaryMember> members, WebDBManager dbMgr)
        {
            Dictionary<int, string> dicCompanyMemberPhoneNumbers = new Dictionary<int, string>();

            foreach (TemporaryMember member in members)
            {
                if (member._MemberType == TemporaryMember.MemberType.CompanyMember)
                {
                    DataCompanyMember companyMember = GetCompanyMember(member.MemberID, dbMgr);

                    if (companyMember == null)
                        continue;

                    dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
                }
                else if (member._MemberType == TemporaryMember.MemberType.RegularTeam)
                {
                    if (member.TeamLeader == 1)
                    {
                        Data_RegularTeam team = GetRegularTeam(member.MemberID, null, dbMgr);

                        if (team == null)
                            continue;

                        int nLeaderID = GetRegularTeamLeaderID(team, dbMgr);

                        if (nLeaderID < 0)
                            continue;

                        DataCompanyMember companyMember = GetCompanyMember(nLeaderID, dbMgr);

                        if (companyMember == null)
                            continue;

                        dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
                    }
                    else
                    {
                        List<DataCompanyMember> companyMembers = new List<DataCompanyMember>();

                        if (GetRegularCompanyMemberList(member.MemberID, member.IncludeChildTeams, companyMembers, dbMgr) == false)
                            continue;

                        if (companyMembers == null)
                            continue;

                        foreach (DataCompanyMember companyMember in companyMembers)
                        {
                            dicPhoneNumbers[companyMember.PhoneNumber] = companyMember.PhoneNumber;
                        }
                    }
                }
                else if (member._MemberType == TemporaryMember.MemberType.ExternalCompanyMember)
                {
                    DataExternalMember externalMember = GetExternalMember(member.MemberID, dbMgr);

                    if (externalMember != null)
                        dicPhoneNumbers[externalMember.PhoneNumber] = externalMember.PhoneNumber;
                }
                else if (member._MemberType == TemporaryMember.MemberType.ExternalTeam || member._MemberType == TemporaryMember.MemberType.ExternalCompanyTeam)
                {
                    List<DataExternalMember> externalMembers = new List<DataExternalMember>();

                    if (GetExternalMemberList(member.MemberID, member.IncludeChildTeams, externalMembers, dbMgr))
                    {
                        foreach (DataExternalMember externalMember in externalMembers)
                        {
                            dicPhoneNumbers[externalMember.PhoneNumber] = externalMember.PhoneNumber;
                        }
                    }
                }
            }
        }

        private List<TemporaryMember> ReadTemporaryMembers(string strTeamIDs, bool isNormal, WebDBManager dbMgr)
        {
            List<TemporaryMember> members = new List<TemporaryMember>();

            if (strTeamIDs.Length == 0)
                return members;

            string strTableName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strFormat = "select team.ID, TeamName, link.MemberID, link.MemberType, link.IsTeamLeader, link.Role, link.MemberName from {0} as team, TemporaryMemberList as link ";
            strFormat += "where link.TemporaryTeamID = team.ID and link.IsNormal = {1} and team.SiteID = {2} and team.ID in ({3})";

            string strSQL = string.Format(strFormat, strTableName, isNormal ? 1 : 0, dbMgr.SiteID, strTeamIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return members;

            TemporaryMember.MemberType memberType;
            TemporaryMember.RoleType roleType;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int _nTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nTeamLeader = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nRoleType = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                bool _includeChildTeams = true;

                if (nMemberID < 0)
                {
                    nMemberID = -nMemberID;
                    _includeChildTeams = false;
                }

                if (_nTeamID < 0 || nMemberID < 0)
                    continue;

                if (!TemporaryMember.GetMemberType(nMemberType, out memberType))
                    continue;

                if (!TemporaryMember.GetRoleType(nRoleType, out roleType))
                {
                    roleType = TemporaryMember.RoleType.Unknown;
                    //continue;
                }

                if (strMemberName == "null")
                    strMemberName = "";

                TemporaryMember member = new TemporaryMember(_nTeamID, isNormal, nMemberID, nTeamLeader, memberType, roleType, strMemberName);
                members.Add(member);

                if (memberType == TemporaryMember.MemberType.ExternalCompanyTeam ||
                    memberType == TemporaryMember.MemberType.ExternalTeam ||
                    memberType == TemporaryMember.MemberType.RegularTeam)
                    member.IncludeChildTeams = _includeChildTeams;
            }

            return members;
        }

        private void GetTemporaryTeamIDs(int nParentTeamID, bool isNormal, WebDBManager dbMgr, List<int> ids)
        {
            string strTableName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strSQL = "Select ID from " + strTableName + " where SiteID = " + dbMgr.SiteID.ToString() + " and ParentTeamID = " + nParentTeamID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    return;

                ids.Add(id.Data);
                GetTemporaryTeamIDs(id.Data, isNormal, dbMgr, ids);
            }
        }

        private DataExternalMember GetExternalMember(int nID, WebDBManager dbMgr)
        {
            string strSQL = "select Name, PhoneNumber FROM ExternalCompanyMember WHERE ID = " + nID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 2)
                return null;

            string strMemberName = WebDBManager.GetStringField(arrResult[0], "");
            string strPhoneNumber = WebDBManager.GetStringField(arrResult[1], "");

            DataExternalMember member = new DataExternalMember();

            member.ID = nID;
            member.Name = strMemberName;
            member.PhoneNumber = strPhoneNumber;

            return member;
        }

        private bool GetExternalMemberList(int nTeamID, bool includeChildTeams, List<DataExternalMember> members, WebDBManager dbMgr)
        {
            string strSQL = "Select member.ID, member.Name, member.PhoneNumber ";
            strSQL += "from ExternalCompanyMember as member, ExternalTeam as team, ExternalMemberList as eml ";
            strSQL += "where eml.ExternalCompanyTeamID = team.ID and eml.ExternalCompanyMemberID = member.ID and team.ID = " + nTeamID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");

                if (id == null)
                    return false;

                DataExternalMember member = new DataExternalMember();

                member.ID = id.Data;
                member.Name = strMemberName;
                member.PhoneNumber = strPhoneNumber;

                members.Add(member);
            }

            if (includeChildTeams)
                GetExternalMemberList(nTeamID, members, dbMgr);

            return true;
        }

        private bool GetExternalMemberList(int nParentTeamID, List<DataExternalMember> members, WebDBManager dbMgr)
        {
            string strSQL = "Select member.ID, member.Name, member.PhoneNumber, team.ID ";
            strSQL += "from ExternalCompanyMember as member, ExternalTeam as team, ExternalMemberList as eml ";
            strSQL += "where eml.ExternalCompanyTeamID = team.ID and eml.ExternalCompanyMemberID = member.ID and team.ParentTeamID = " + nParentTeamID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            Dictionary<int, int> dicTeamIDs = new Dictionary<int, int>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");
                VariousData<int> teamID = WebDBManager.GetIntField(arrResult[i + 3].ToString());

                if (id == null || teamID == null)
                    return false;

                DataExternalMember member = new DataExternalMember();

                member.ID = id.Data;
                member.Name = strMemberName;
                member.PhoneNumber = strPhoneNumber;

                members.Add(member);
                dicTeamIDs[teamID.Data] = teamID.Data;
            }

            foreach (KeyValuePair<int, int> pair in dicTeamIDs)
            {
                GetExternalMemberList(pair.Key, members, dbMgr);
            }

            return true;
        }

        // 명시적으로 팀장이 선언되어 있지 않으면 팀장과 가장 가까운 직책을 선택한다.
        // 그마저도 없으면 가장 먼저 등록된 팀원을 리턴한다.
        private int GetRegularTeamLeaderID(Data_RegularTeam team, WebDBManager dbMgr)
        {
            List<DataCompanyMember> arrMembers = new List<DataCompanyMember>();
            if (GetRegularCompanyMemberList(team.ID, false, arrMembers, dbMgr) == false)
                return -1;

            DataCompanyMember teamLeader = null;

            foreach (DataCompanyMember member in arrMembers)
            {
                if (teamLeader == null)
                    teamLeader = member;
                else
                {
                    int compare = teamLeader.CompareTo(member);

                    if (compare < 0)
                        teamLeader = member;
                    else if (compare == 0)
                    {
                        if (teamLeader.ID > member.ID)
                            teamLeader = member;
                    }
                }
            }

            if (teamLeader == null)
                return -1;

            return teamLeader.ID;
        }

        private bool GetRegularCompanyMemberList(int nTeamID, bool includeChildTeams, List<DataCompanyMember> members, WebDBManager dbMgr)
        {
            string strSQL = "Select member.ID, member.MemberName, member.LevelID, member.MemberID, member.OfficePhoneNumber, member.PhoneNumber ";
            strSQL += "from RegularTeam as team, CompanyMember as member, RegularMemberList as rml ";
            strSQL += "where rml.RegularTeamID = team.ID and rml.CompanyMemberID = member.ID and team.ID = " + nTeamID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 5], "");

                if (id == null)
                    return false;

                DataCompanyMember member = new DataCompanyMember();

                member.ID = id.Data;
                member.MemberName = strMemberName;
                member.MemberID = strMemberID;
                member.OfficePhoneNumber = strOfficePhoneNumber;
                member.PhoneNumber = strPhoneNumber;

                members.Add(member);
            }

            if (includeChildTeams)
                GetRegularCompanyMemberList(nTeamID, members, dbMgr);

            return true;
        }

        private bool GetRegularCompanyMemberList(int nParentTeamID, List<DataCompanyMember> members, WebDBManager dbMgr)
        {
            string strSQL = "Select member.ID, member.MemberName, member.LevelID, member.MemberID, member.OfficePhoneNumber, member.PhoneNumber, team.ID ";
            strSQL += "from RegularTeam as team, CompanyMember as member, RegularMemberList as rml ";
            strSQL += "where rml.RegularTeamID = team.ID and rml.CompanyMemberID = member.ID and team.ParentTeamID = " + nParentTeamID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            Dictionary<int, int> teamIDs = new Dictionary<int, int>();

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 3], "");
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 4], "");
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 5], "");
                VariousData<int> teamID = WebDBManager.GetIntField(arrResult[i + 6].ToString());

                if (id == null || teamID == null)
                    return false;

                DataCompanyMember member = new DataCompanyMember();

                member.ID = id.Data;
                member.MemberName = strMemberName;
                member.MemberID = strMemberID;
                member.OfficePhoneNumber = strOfficePhoneNumber;
                member.PhoneNumber = strPhoneNumber;

                members.Add(member);

                teamIDs[teamID.Data] = teamID.Data;
            }

            foreach (KeyValuePair<int, int> pair in teamIDs)
            {
                GetRegularCompanyMemberList(pair.Key, members, dbMgr);
            }

            return true;
        }

        private DataCompanyMember GetCompanyMember(int nID, WebDBManager dbMgr)
        {
            string strSQL = "select MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber FROM CompanyMember WHERE ID = " + nID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 5)
                return null;

            string strMemberName = WebDBManager.GetStringField(arrResult[0], "");
            int nLevelID = WebDBManager.GetIntField(arrResult[1].ToString(), 0);
            string strMemberID = WebDBManager.GetStringField(arrResult[2], "");
            string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[3], "");
            string strPhoneNumber = WebDBManager.GetStringField(arrResult[4], "");

            DataCompanyMember member = new DataCompanyMember();

            member.ID = nID;
            member.MemberName = strMemberName;
            member.MemberID = strMemberID;
            member.OfficePhoneNumber = strOfficePhoneNumber;
            member.PhoneNumber = strPhoneNumber;

            return member;
        }

        private Data_RegularTeam GetRegularTeam(int nID, Data_RegularTeam teamParent, WebDBManager dbMgr)
        {
            string strSQL = "Select ID, TeamName from RegularTeam";

            if (teamParent != null)
                strSQL += " where ParentTeamID = " + teamParent.ID.ToString();
            else
                strSQL += " where ID = " + nID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            Data_RegularTeam _team = null;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> teamID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1]);

                if (teamID == null || strTeamName == null)
                    continue;

                Data_RegularTeam team = new Data_RegularTeam();

                team.ID = teamID.Data;
                team.TeamName = strTeamName;

                if (teamParent != null)
                {
                    team.ParentTeamID = teamParent.ID;
                    teamParent.ChildTeams.Add(team);
                }

                GetRegularTeam(-1, team, dbMgr);
                _team = team;
            }

            return _team;
        }

        private void AddRegularTeamLink(ref string strRegularTeamLink, string strTableName, int nParentTeamID, WebDBManager dbMgr)
        {
            string strSQL = "Select ID, RegularTeamLink from " + strTableName + " where SiteID = " + dbMgr.SiteID.ToString() + " and ParentTeamID = " + nParentTeamID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strTeamLink = WebDBManager.GetStringField(arrResult[i + 1]);

                if (id == null)
                    continue;

                if (strTeamLink != null && strTeamLink.Length > 0)
                {
                    if (strRegularTeamLink.Length > 0)
                        strRegularTeamLink += ", " + strTeamLink;
                    else
                        strRegularTeamLink = strTeamLink;
                }

                AddRegularTeamLink(ref strRegularTeamLink, strTableName, id.Data, dbMgr);
            }
        }

        private List<string> GetRegularTeamPhoneNumbers(string strIDs, WebDBManager dbMgr)
        {
            string strSQL = "Select member.PhoneNumber ";
            strSQL += "from RegularMemberList as rml, CompanyMember as member ";
            strSQL += "where rml.CompanyMemberID = member.ID and rml.RegularTeamID in (" + strIDs + ")";

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount;i++)
            {
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i]);

                if (strPhoneNumber == null)
                    continue;

                strPhoneNumber = strPhoneNumber.Trim();

                if (strPhoneNumber.Length == 0)
                    continue;

                strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);
                dicPhoneNumbers[strPhoneNumber] = strPhoneNumber;
            }

            return dicPhoneNumbers.Keys.ToList();
        }

        public static string GetPhoneNumber(string str)
        {
            return AES256Cipher.AES_decrypt(str, key);
        }
    }
}
