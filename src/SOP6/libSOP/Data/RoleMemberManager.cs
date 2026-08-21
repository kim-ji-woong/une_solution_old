using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace UnE
{
    namespace SOP
    {
        public class RoleMemberManager
        {
            private WebDBManager m_dbMgr = null;
            private SOPManager m_sopMgr = null;

            public RoleMemberManager(WebDBManager dbMgr, SOPManager sopMgr)
            {
                m_dbMgr = dbMgr;
                m_sopMgr = sopMgr;
            }

            /// <summary>
            /// 사용자가 편집한후 SOP수행시에 사용되는 UserDefinedTeam정보
            /// </summary>
            /// <param name="nActionStepHistoryID"></param>
            /// <returns></returns>
            public ArrayList GetUsingTeamsByHistoryID(int nActionStepHistoryID)
            {
                ArrayList teams = new ArrayList();
                WebDBManager dbMgr = m_dbMgr;

                string szTemp = "SELECT ID, TeamType, TeamID, PhoneNumber, UserName, Role, JobName, AllMembers " +
                               " FROM ActionStepUsingTeam where ActionStepHistoryID = {0}";
                string szSQL = string.Format(szTemp, nActionStepHistoryID);
                ArrayList arrResult = dbMgr.GetResultData(szSQL);

                if (arrResult == null)
                    return null;
                int nResultCount = arrResult.Count;

                Dictionary<int, List<DataRoleMember>> dicNormalMembers = new Dictionary<int, List<DataRoleMember>>();
                Dictionary<int, List<DataRoleMember>> dicEmergencyMembers = new Dictionary<int, List<DataRoleMember>>();
                Dictionary<int, List<DataRoleMember>> dicRegularMembers = new Dictionary<int, List<DataRoleMember>>(); 

                for (int i = 0; i < nResultCount - 7; i += 8)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<int> teamType = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                    VariousData<int> teamID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                    string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 3]);
                    string strUserName = WebDBManager.GetStringField(arrResult[i + 4]);
                    string strRole = WebDBManager.GetStringField(arrResult[i + 5]);
                    string strJobName = WebDBManager.GetStringField(arrResult[i + 6]);
                    VariousData<int> allMembers = WebDBManager.GetIntField(arrResult[i + 7].ToString());

                    if (id == null || teamType == null || teamID == null)
                        continue;

                    if (strUserName == null)
                        strUserName = "";
                    if (strRole == null)
                        strRole = "";
                    if (strJobName == null)
                        strJobName = "";

                    bool _allMembers = allMembers == null || allMembers.Data == 0 ? false : true;

                    if (teamType.Data == 0)
                        SetRoleMembers(dicNormalMembers, id.Data, teamID.Data, strPhoneNumber, strUserName, strRole, strJobName, _allMembers);
                    else if (teamType.Data == 1)
                        SetRoleMembers(dicNormalMembers, id.Data, teamID.Data, strPhoneNumber, strUserName, strRole, strJobName, _allMembers);
                    else if (teamType.Data == 2)
                        AddExternalTeams(teams, id.Data, teamID.Data, strPhoneNumber, strUserName, strRole, strJobName, _allMembers);
                    else if (teamType.Data == 3)
                        AddUserDefinedTeams(teams, id.Data, teamID.Data, strPhoneNumber, strUserName, strRole, strJobName, _allMembers);
                    else if (teamType.Data == 4)
                        SetRoleMembers(dicRegularMembers, id.Data, teamID.Data, strPhoneNumber, strUserName, strRole, strJobName, _allMembers); 
                }

                foreach (KeyValuePair<int, List<DataRoleMember>> pair in dicRegularMembers)
                {
                    AddRegularTeams(teams, pair.Key, pair.Value);
                }

                foreach (KeyValuePair<int, List<DataRoleMember>> pair in dicNormalMembers)
                {
                    AddTemporaryNormalTeams(teams, pair.Key, pair.Value);
                }

                foreach (KeyValuePair<int, List<DataRoleMember>> pair in dicEmergencyMembers)
                {
                    AddTemporaryNormalTeams(teams, pair.Key, pair.Value);
                }
                 
                return teams;
            }

            private void SetRoleMembers(Dictionary<int, List<DataRoleMember>> dicRoleMembers, int nID, int nTeamID, string strPhoneNumber, string strUserName, string strRole, string strJobName, bool allMembers)
            {
                List<DataRoleMember> members;

                if (!dicRoleMembers.TryGetValue(nTeamID, out members))
                {
                    members = new List<DataRoleMember>();
                    dicRoleMembers[nTeamID] = members;
                }

                DataRoleMember roleMember = new DataRoleMember(strUserName, strPhoneNumber, strRole, strJobName);
                roleMember.ID = nID;
                roleMember.AllMembers = allMembers;
                members.Add(roleMember);
            }

            private void AddExternalTeams(ArrayList teams, int nID, int nTeamID, string strPhoneNumber, string strUserName, string strRole, string strJobName, bool allMembers)
            {
                Data_ExternalTeam orgTeam = m_sopMgr.GetExternalTeam(nTeamID);

                if (orgTeam != null)
                {
                    Data_ExternalTeam team = new Data_ExternalTeam(nTeamID, orgTeam.TeamName, orgTeam.PhoneNumber, orgTeam.FaxNumber);

                    DataRoleMember roleMember = new DataRoleMember(strUserName, strPhoneNumber, strRole, strJobName);
                    roleMember.ID = nID;
                    roleMember.AllMembers = allMembers;

                    team.Tag = roleMember;
                    teams.Add(team);
                }
            }

            private void AddUserDefinedTeams(ArrayList teams, int nID, int nTeamID, string strPhoneNumber, string strUserName, string strRole, string strJobName, bool allMembers)
            {
                Data_ExternalTeam orgTeam = m_sopMgr.GetUserDefinedTeam(nTeamID);

                if (orgTeam != null)
                {
                    Data_UserDefinedTeam team = new Data_UserDefinedTeam(nTeamID, orgTeam.TeamName, orgTeam.PhoneNumber, orgTeam.FaxNumber);

                    DataRoleMember roleMember = new DataRoleMember(strUserName, strPhoneNumber, strRole, strJobName);
                    roleMember.ID = nID;
                    roleMember.AllMembers = allMembers;

                    team.Tag = roleMember;
                    teams.Add(team);
                }
            }

            private void AddRegularTeams(ArrayList teams, int nTeamID, List<DataRoleMember> roleMembers)
            {
                Data_RegularTeam orgTeam = m_sopMgr.GetRegularTeam(nTeamID);

                if (orgTeam != null)
                {
                    Data_RegularTeam team = new Data_RegularTeam();

                    team.ID = nTeamID;
                    team.TeamName = orgTeam.TeamName;
                    team.Tag = CopyRoleMemberList(roleMembers);

                    teams.Add(team);
                }
            } 
            /*private void AddRegularTeams(ArrayList teams, int nID, int nTeamID, string strPhoneNumber, string strUserName, string strRole, string strJobName)
            {
                Data_RegularTeam orgTeam = m_sopMgr.GetRegularTeam(nTeamID);

                if (orgTeam != null)
                {
                    Data_RegularTeam team = new Data_RegularTeam();

                    team.ID = nTeamID;
                    team.TeamName = team.TeamName;

                    DataRoleMember roleMember = new DataRoleMember(strUserName, strPhoneNumber, strRole, strJobName);
                    roleMember.ID = nID;

                    team.Tag = roleMember;
                    teams.Add(team);
                }
            }*/

            private void AddTemporaryNormalTeams(ArrayList teams, int nTeamID, List<DataRoleMember> roleMembers)
            {
                Data_NormalTeam orgTeam = m_sopMgr.GetTemporaryNormalTeam(nTeamID);

                if (orgTeam != null)
                {
                    Data_NormalTeam team = new Data_NormalTeam();

                    team.ID = nTeamID;
                    team.TeamName = orgTeam.TeamName;
                    team.Tag = CopyRoleMemberList(roleMembers);

                    teams.Add(team);
                }
            }

            private void AddTemporaryEmergencyTeams(ArrayList teams, int nTeamID, List<DataRoleMember> roleMembers)
            {
                Data_EmergencyTeam orgTeam = m_sopMgr.GetTemporaryEmergencyTeam(nTeamID);

                if (orgTeam != null)
                {
                    Data_EmergencyTeam team = new Data_EmergencyTeam();

                    team.ID = nTeamID;
                    team.TeamName = orgTeam.TeamName;
                    team.Tag = CopyRoleMemberList(roleMembers);

                    teams.Add(team);
                }
            }

            private List<DataRoleMember> CopyRoleMemberList(List<DataRoleMember> members)
            {
                List<DataRoleMember> roleMembers = new List<DataRoleMember>();

                foreach (DataRoleMember member in members)
                {
                    DataRoleMember roleMember = new DataRoleMember(member.MemberName, member.PhoneNumber, member.Role, member.JobName);
                    roleMember.ID = member.ID;
                    roleMembers.Add(roleMember);
                }

                return roleMembers;
            }

            private int GetMaxUsingTeamID()
            {
                string strSQL = "select max(id) from ActionStepUsingTeam";

                WebDBManager dbMgr = m_dbMgr;
                ArrayList arrResult = dbMgr.GetResultData(strSQL);

                if (arrResult == null || arrResult.Count == 0)
                    return 0;

                return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
            }

            public void SaveUsingTeams(int nHistoryID, List<Data_UserDefinedTeam> userDefinedTeams, List<Data_ExternalTeam> externalTeams, List<Data_RegularTeam> regularTeams, List<Data_NormalTeam> normalTeams, List<Data_EmergencyTeam> emergencyTeams)
            {
                int nMaxUsingTeamID = GetMaxUsingTeamID();

                if (userDefinedTeams != null)
                {
                    foreach (Data_UserDefinedTeam team in userDefinedTeams)
                    {
                        if (team.Tag == null)
                            continue;

                        DataRoleMember roleMember = (DataRoleMember)team.Tag;
                        SaveUsingTeams(nHistoryID, team.ID, 3, roleMember, ref nMaxUsingTeamID);
                    }
                }

                if (externalTeams != null)
                {
                    foreach (Data_ExternalTeam team in externalTeams)
                    {
                        if (team.Tag == null)
                            continue;

                        DataRoleMember roleMember = (DataRoleMember)team.Tag;
                        SaveUsingTeams(nHistoryID, team.ID, 2, roleMember, ref nMaxUsingTeamID);
                    }
                }

                if (regularTeams != null)
                {
                    foreach (Data_RegularTeam team in regularTeams)
                    {
                        if (team.Tag == null)
                            continue;

                        List<DataRoleMember> roleMembers = (List<DataRoleMember>)team.Tag;

                        foreach (DataRoleMember roleMember in roleMembers)
                        {
                            SaveUsingTeams(nHistoryID, team.ID, 4, roleMember, ref nMaxUsingTeamID);
                        }

                        //DataRoleMember roleMember = (DataRoleMember)team.Tag;
                        //SaveUsingTeams(nHistoryID, team.ID, 4, roleMember, ref nMaxUsingTeamID);
                    }
                }

                if (normalTeams != null)
                {
                    foreach (Data_NormalTeam team in normalTeams)
                    {
                        if (team.Tag == null)
                            continue;

                        List<DataRoleMember> roleMembers = (List<DataRoleMember>)team.Tag;

                        foreach (DataRoleMember roleMember in roleMembers)
                        {
                            SaveUsingTeams(nHistoryID, team.ID, 0, roleMember, ref nMaxUsingTeamID);
                        }
                    }
                }

                if (emergencyTeams != null)
                {
                    foreach (Data_EmergencyTeam team in emergencyTeams)
                    {
                        if (team.Tag == null)
                            continue;

                        List<DataRoleMember> roleMembers = (List<DataRoleMember>)team.Tag;

                        foreach (DataRoleMember roleMember in roleMembers)
                        {
                            SaveUsingTeams(nHistoryID, team.ID, 0, roleMember, ref nMaxUsingTeamID);
                        }
                    }
                } 
            }

            private void SaveUsingTeams(int nActionStepHistoryID, int nTeamID, int nTeamType, DataRoleMember roleMember, ref int nMaxUsingTeamID)
            {
                string strRoleCondition = "", strJobNameCondition = "";

                if (roleMember.Role == null || roleMember.Role.Length == 0)
                    strRoleCondition = "is NULL";
                else
                    strRoleCondition = "= '" + roleMember.Role + "'";

                if (roleMember.JobName == null || roleMember.JobName.Length == 0)
                    strJobNameCondition = "is NULL";
                else
                    strJobNameCondition = "= '" + roleMember.JobName + "'";

                if (roleMember.ID > 0)
                    UpdateRoleMember(nTeamID, nTeamType, roleMember);
                else
                    InsertRoleMember(nActionStepHistoryID, nTeamID, nTeamType, roleMember, ref nMaxUsingTeamID);
            }

            private void InsertRoleMember(int nActionStepHistoryID, int nTeamID, int nTeamType, DataRoleMember roleMember, ref int nMaxUsingTeamID)
            {
                string strRole = "", strJobName = "";

                if (roleMember.Role == null || roleMember.Role.Length == 0)
                    strRole = "NULL";
                else
                    strRole = "'" + roleMember.Role + "'";

                if (roleMember.JobName == null || roleMember.JobName.Length == 0)
                    strJobName = "NULL";
                else
                    strJobName = "'" + roleMember.JobName + "'";

                string strFormat = "Insert into ActionStepUsingTeam (ID, ActionStepHistoryID, TeamType, TeamID, PhoneNumber, UserName, Role, JobName, AllMembers) ";
                strFormat += "values ({0}, {1}, {2}, {3}, '{4}', '{5}', {6}, {7}, {8})";

                string strSQL = string.Format(strFormat,
                    ++nMaxUsingTeamID, nActionStepHistoryID, nTeamType, nTeamID, roleMember.PhoneNumber, roleMember.MemberName, strRole, strJobName, roleMember.AllMembers ? 1 : 0);

                if (m_dbMgr.GetResultData(strSQL) != null)
                    roleMember.ID = nMaxUsingTeamID;
                else
                    nMaxUsingTeamID--;
            }

            private void UpdateRoleMember(int nTeamID, int nTeamType, DataRoleMember roleMember)
            {
                string strRole = "", strJobName = "";

                if (roleMember.Role == null || roleMember.Role.Length == 0)
                    strRole = "NULL";
                else
                    strRole = "'" + roleMember.Role + "'";

                if (roleMember.JobName == null || roleMember.JobName.Length == 0)
                    strJobName = "NULL";
                else
                    strJobName = "'" + roleMember.JobName + "'";

                string strSQL = string.Format("Update ActionStepUsingTeam set TeamType = {0}, TeamID = {1}, PhoneNumber = '{2}', UserName = '{3}', Role = {4}, JobName = {5}, AllMembers = {6} where ID = {7}",
                    nTeamType, nTeamID, roleMember.PhoneNumber, roleMember.MemberName, strRole, strJobName, roleMember.AllMembers ? 1 : 0, roleMember.ID);

                m_dbMgr.GetResultData(strSQL);
            }
        }
    }
}
