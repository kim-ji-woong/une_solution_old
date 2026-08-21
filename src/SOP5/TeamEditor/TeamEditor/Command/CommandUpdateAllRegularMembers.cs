using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using System.Collections;

namespace TeamEditor.Command
{
    public class CommandUpdateAllRegularMembers : CommandEx
    {
        private Tree<RegularTeam> m_teamTree = null;
        // 팀별 직원들
        private Dictionary<RegularTeam, List<CompanyMember>> m_dicRegularMembers = null;
        // 삭제될 이전 팀들의 목록
        private List<RegularTeam> m_removingOldTeams = null;
        // 삭제될 이전 직원들의 목록
        private List<CompanyMember> m_removingOldCompanyMembers = null;
        // 삭제되지 않고 남아있을 이전 직원들의 복사본
        private List<CompanyMember> m_notRemovingOldCompanyMembers = null;
        // 삭제될 주간 및 평일 자위소방대원 목록
        private Dictionary<Team, List<TemporaryMember>> m_dicRemovingOldTemporaryNormalMembers = new Dictionary<Team, List<TemporaryMember>>();
        // 삭제될 야간 및 휴일 자위소방대원 목록
        private Dictionary<Team, List<TemporaryMember>> m_dicRemovingOldTemporaryEmergencyMembers = new Dictionary<Team, List<TemporaryMember>>();

        // Updage하기 이전 원래 상태의 데이터
        private Tree<RegularTeam> m_oldTeamTree = null;
        private Dictionary<RegularTeam, List<CompanyMember>> m_dicOldRegularMembers = null;

        private List<TemporaryMember> m_removingOldNormalMembers = new List<TemporaryMember>();
        private List<TemporaryMember> m_removingOldEmergencyMembers = new List<TemporaryMember>();

        private List<string> m_rollbackQueries = new List<string>();

        public Tree<RegularTeam> TeamTree
        {
            get { return m_teamTree; }
            set { m_teamTree = value; }
        }

        public Dictionary<RegularTeam, List<CompanyMember>> RegularMembers
        {
            get { return m_dicRegularMembers; }
            set { m_dicRegularMembers = value; }
        }

        public List<RegularTeam> RemovingOldTeams
        {
            get { return m_removingOldTeams; }
            set { m_removingOldTeams = value; }
        }

        public List<CompanyMember> RemovingOldCompanyMembers
        {
            get { return m_removingOldCompanyMembers; }
            set { m_removingOldCompanyMembers = value; }
        }

        public override void Do()
        {
            SaveOldMembers();
            DeleteOldMembers();

            LoadNewMembers();
        }

        public override void RollBack()
        {
            RemoveNewMembers();
            LoadOldMembers();
        }

        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            dbMgr.BeginBatch();

            if (dir)
            {
                m_rollbackQueries.Clear();

                if (RemoveDBFacilityManagers(dbMgr) == false)
                    goto ROLLBACK;
                if (RemoveDBTemporaryMembers(dbMgr) == false)
                    goto ROLLBACK;
                if (RemoveDBRegularMembers(dbMgr) == false)
                    goto ROLLBACK;
                if (RemoveDBCompanyMembers(dbMgr) == false)
                    goto ROLLBACK;
                if (RemoveDBRegularTeams(dbMgr) == false)
                    goto ROLLBACK;

                Dictionary<string, int> dicRegularTeamPathID = new Dictionary<string, int>();
                Dictionary<CompanyMember, RegularTeam> dicAddedMembers = new Dictionary<CompanyMember, RegularTeam>();
                Dictionary<CompanyMember, int> dicMemberIDs = new Dictionary<CompanyMember,int>();
                Dictionary<RegularTeam, int> dicRegularTeamIDs = new Dictionary<RegularTeam,int>();

                if (AddDBRegularTeams(dbMgr, dicRegularTeamPathID) == false)
                    goto ROLLBACK;
                if (AddNUpdateDBCompanyMembers(dbMgr, dicAddedMembers, dicMemberIDs, dicRegularTeamIDs, dicRegularTeamPathID) == false)
                    goto ROLLBACK;
                if (AddDBRegularMembers(dbMgr, dicAddedMembers, dicMemberIDs, dicRegularTeamIDs) == false)
                    goto ROLLBACK;
            }
            else
            {
                int nRollbackCount = m_rollbackQueries.Count;

                for (int i = nRollbackCount - 1; i >= 0;i-- )
                {
                    string strSQL = m_rollbackQueries[i];

                    if (dbMgr.GetBatchData(strSQL) == null)
                        goto ROLLBACK;
                }
            }

            dbMgr.BatchCommit();

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER);
            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER);
            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER);
            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.COMPANY_MEMBER);
            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.REGULAR_TEAM);
            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER);

            return;

        ROLLBACK:
            dbMgr.BatchRollback();
        }

        private bool AddDBRegularMembers(WebDBManager dbMgr, Dictionary<CompanyMember, RegularTeam> dicAddedMembers, Dictionary<CompanyMember, int> dicMemberIDs, Dictionary<RegularTeam, int> dicRegularTeamIDs)
        {
            string strInsertFormat = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values ";
            strInsertFormat += "({0}, {1}, {2}, {3}, {4})";

            foreach (KeyValuePair<CompanyMember, RegularTeam> pair in dicAddedMembers)
            {
                CompanyMember member = pair.Key;
                RegularTeam team = pair.Value;

                int nMemberID = -1, nTeamID = -1;

                if (dicMemberIDs.TryGetValue(member, out nMemberID) == false)
                    continue;

                if (dicRegularTeamIDs.TryGetValue(team, out nTeamID) == false)
                    continue;

                string strSQL = string.Format(strInsertFormat, nTeamID, nMemberID, member.PositionID,
                    GetSubPositionID(dbMgr, member), GetGroupPositionID(dbMgr, member));

                if (dbMgr.GetBatchData(strSQL) == null)
                    return false;

                /*string strDelete = string.Format("Delete from RegularMemberList where RegularTeamID = {0} and CompanyMemberID = {1}",
                    team.TeamID, member.ID);
                m_rollbackQueries.Add(strDelete);*/
            }

            string strDelete = "Delete from RegularMemberList";
            m_rollbackQueries.Add(strDelete);

            return true;
        }

        private string GetSubPositionID(WebDBManager dbMgr, CompanyMember member)
        {
            if (member.SubJobPosition == null)
                return "NULL";

            if (member.SubJobPosition.ID > 0)
                return member.SubJobPosition.ID.ToString();

            int nID = GetMaxID(dbMgr, "JobSubPosition") + 1;

            string strSQL = string.Format("Insert into JobSubPosition (ID, Name) values ({0}, '{1}')", nID, member.SubJobPosition.Name);

            if (dbMgr.GetBatchData(strSQL) == null)
                return "NULL";

            return nID.ToString();
        }

        private string GetGroupPositionID(WebDBManager dbMgr, CompanyMember member)
        {
            if (member.GroupPosition == null)
                return "NULL";

            if (member.GroupPosition.ID > 0)
                return member.GroupPosition.ID.ToString();

            int nID = GetMaxID(dbMgr, "JobPositionGroup") + 1;

            string strSQL = string.Format("Insert into JobPositionGroup (ID, Name) values ({0}, '{1}')", nID, member.GroupPosition.Name);

            if (dbMgr.GetBatchData(strSQL) == null)
                return "NULL";

            return nID.ToString();
        }

        private bool AddDBRegularTeams(WebDBManager dbMgr, Dictionary<string, int> dicRegularTeamID)
        {
            int nTeamID = GetMaxID(dbMgr, "RegularTeam") + 1;

            string strInsertIDs = "";
            string strInsertFormat = "Insert into RegularTeam (ID, TeamName, ParentTeamID) values ({0}, '{1}', {2})";

            if (AddDBRegularTeams(dbMgr, m_teamTree.RootNode, ref nTeamID, strInsertFormat, ref strInsertIDs, dicRegularTeamID) == false)
                return false;

            if (strInsertIDs.Length > 0)
            {
                int nOldRootTeamID = GetOldRootTeamID();
                string strDelete = "";

                if (nOldRootTeamID > 0)
                    strDelete = "Delete from RegularTeam where ID <> " + nOldRootTeamID.ToString();
                else
                    strDelete = "Delete from RegularTeam";

                m_rollbackQueries.Add(strDelete);
            }

            return true;
        }

        // dicRegularTeamID
        // Key : RegularTeam Full Path
        // Value : Team ID
        private bool AddDBRegularTeams(WebDBManager dbMgr, Tree<RegularTeam>.Node node, ref int nTeamID, string strInsertFormat, ref string strInsertIDs, Dictionary<string, int> dicRegularTeamID)
        {
            if (node.Data == null)
                return true;

            if (node.Data.TeamID > 0)
            {
                string strTeamPath = GetTeamPath(node.Data);
                dicRegularTeamID[strTeamPath] = node.Data.TeamID;

                foreach (Tree<RegularTeam>.Node child in node.Children)
                {
                    if (AddDBRegularTeams(dbMgr, child, ref nTeamID, strInsertFormat, ref strInsertIDs, dicRegularTeamID) == false)
                        return false;
                }

                return true;
            }

            string strParentTeamID = "NULL";

            if (node.Parent != null)
            {
                if (node.Parent.Data != null && node.Parent.Data.TeamID > 0)
                    strParentTeamID = node.Parent.Data.TeamID.ToString();

                if (strInsertIDs.Length == 0)
                    strInsertIDs = nTeamID.ToString();
                else
                    strInsertIDs += ", " + nTeamID.ToString();

                string strSQL = string.Format(strInsertFormat, nTeamID++, node.Data.TeamName, strParentTeamID);

                if (dbMgr.GetBatchData(strSQL) == null)
                    return false;

                string strTeamPath2 = GetTeamPath(node.Data);
                dicRegularTeamID[strTeamPath2] = nTeamID - 1;
            }
            else
            {
                // 최상위 노드는 새로 저장하지 않고 이름만 바꿔준다.
                int nOldRootTeamID = GetOldRootTeamID();
                string strSQL2 = "Update RegularTeam set TeamName = '" + node.Data.TeamName + "' where ID = " + nOldRootTeamID.ToString();

                if (dbMgr.GetBatchData(strSQL2) == null)
                    return false;

                strSQL2 = "Update Site set SiteName = '" + node.Data.TeamName + "' where ID = " + FormMain.Instance.SiteID.ToString();

                if (dbMgr.GetBatchData(strSQL2) == null)
                    return false;

                if (nOldRootTeamID >= 0 && m_oldTeamTree != null && m_oldTeamTree.RootNode != null)
                {
                    RegularTeam teamOld = m_oldTeamTree.RootNode.Data;

                    if (teamOld != null)
                    {
                        string strRollBack = "Update RegularTeam set TeamName = '" + teamOld.TeamName + "' where ID = " + nOldRootTeamID.ToString();
                        m_rollbackQueries.Add(strRollBack);

                        strRollBack = "Update Site set SiteName = '" + teamOld.TeamName + "' where ID = " + FormMain.Instance.SiteID.ToString();
                        m_rollbackQueries.Add(strRollBack);
                    }
                }

                string strTeamPath2 = GetTeamPath(node.Data);
                dicRegularTeamID[strTeamPath2] = nOldRootTeamID;
            }

            foreach (Tree<RegularTeam>.Node child in node.Children)
            {
                if (AddDBRegularTeams(dbMgr, child, ref nTeamID, strInsertFormat, ref strInsertIDs, dicRegularTeamID) == false)
                    return false;
            }

            return true;
        }

        private string GetTeamPath(RegularTeam team)
        {
            string strName = team.TeamName;

            if (team.ParentTeam != null)
            {
                string strParentPath = GetTeamPath(team.ParentTeam);
                return strParentPath + "/" + strName;
            }

            return strName;
        }

        private bool AddNUpdateDBCompanyMembers(WebDBManager dbMgr, Dictionary<CompanyMember, RegularTeam> dicAddedMembers, Dictionary<CompanyMember, int> dicMemberIDs, Dictionary<RegularTeam, int> dicRegularTeamIDs, Dictionary<string, int> dicRegularTeamPathID)
        {
            int nMemberID = GetMaxID(dbMgr, "CompanyMember") + 1;

            string strInsertFormat = "Insert into CompanyMember (ID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber, SubLevelID) values ";
            strInsertFormat += "({0}, '{1}', {2}, {3}, {4}, {5}, {6})";

            string strUpdateFormat = "Update CompanyMember set LevelID = {0}, MemberID = {1}, OfficePhoneNumber = {2}, PhoneNumber = {3}, SubLevelID = {4} where ID = {5}";

            string strIDs = "";

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicRegularMembers)
            {
                if (pair.Key.TeamID > 0)
                    dicRegularTeamIDs[pair.Key] = pair.Key.TeamID;
                else
                {
                    int nTeamID;
                    string strTeamPath = GetTeamPath(pair.Key);

                    if (dicRegularTeamPathID.TryGetValue(strTeamPath, out nTeamID))
                        dicRegularTeamIDs[pair.Key] = nTeamID;
                    else
                        dicRegularTeamIDs[pair.Key] = -1;
                }

                foreach (CompanyMember member in pair.Value)
                {
                    if (member.ID < 0)
                    {
                        if (strIDs.Length == 0)
                            strIDs = nMemberID.ToString();
                        else
                            strIDs += ", " + nMemberID.ToString();

                        string strSQL = string.Format(strInsertFormat, nMemberID++, member.Name, member.LevelID,
                            member.MemberID == null || member.MemberID.Length == 0 ? "NULL" : "'" + member.MemberID + "'",
                            member.OfficePhoneNumber == null || member.OfficePhoneNumber.Length == 0 ? "NULL" : "'" + member.OfficePhoneNumber + "'",
                            member.PhoneNumber == null || member.PhoneNumber.Length == 0 ? "NULL" : "'" + DataManager.EncryptString(member.PhoneNumber) + "'",
                            GetSubLevelID(dbMgr, member));

                        if (dbMgr.GetBatchData(strSQL) == null)
                            return false;

                        dicAddedMembers[member] = pair.Key;
                        dicMemberIDs[member] = nMemberID - 1;
                    }
                    else
                    {
                        CompanyMember oldMember = GetOldCompanyMember(m_dicOldRegularMembers, pair.Key.TeamID, member.ID);

                        if (oldMember != null)
                        {
                            string strUpdate = string.Format(strUpdateFormat, oldMember.LevelID,
                                oldMember.MemberID == null || oldMember.MemberID.Length == 0 ? "NULL" : "'" + oldMember.MemberID + "'",
                                oldMember.OfficePhoneNumber == null || oldMember.OfficePhoneNumber.Length == 0 ? "NULL" : "'" + oldMember.OfficePhoneNumber + "'",
                                oldMember.PhoneNumber == null || oldMember.PhoneNumber.Length == 0 ? "NULL" : "'" + DataManager.EncryptString(oldMember.PhoneNumber) + "'",
                                oldMember.SubJobLevel == null ? "NULL" : oldMember.SubJobLevel.ID.ToString(),
                                oldMember.ID);

                            m_rollbackQueries.Add(strUpdate);

                            string strSQL = string.Format(strUpdateFormat, member.LevelID,
                                member.MemberID == null || member.MemberID.Length == 0 ? "NULL" : "'" + member.MemberID + "'",
                                member.OfficePhoneNumber == null || member.OfficePhoneNumber.Length == 0 ? "NULL" : "'" + member.OfficePhoneNumber + "'",
                                member.PhoneNumber == null || member.PhoneNumber.Length == 0 ? "NULL" : "'" + DataManager.EncryptString(member.PhoneNumber) + "'",
                                GetSubLevelID(dbMgr, member),
                                member.ID);

                            if (dbMgr.GetBatchData(strSQL) == null)
                                return false;
                        }
                    }
                }
            }

            if (strIDs.Length > 0)
            {
                string strDelete = "Delete from CompanyMember";
                m_rollbackQueries.Add(strDelete);
            }

            return true;
        }

        private CompanyMember GetOldCompanyMember(Dictionary<RegularTeam, List<CompanyMember>> dicMembers, int nTeamID, int nMemberID)
        {
            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in dicMembers)
            {
                if (pair.Key.TeamID == nTeamID)
                {
                    foreach (CompanyMember member in pair.Value)
                    {
                        if (member.ID == nMemberID)
                            return member;
                    }

                    return null;
                }
            }

            return null;
        }

        private string GetSubLevelID(WebDBManager dbMgr, CompanyMember member)
        {
            if (member.SubJobLevel == null)
                return "NULL";

            if (member.SubJobLevel.ID > 0)
                return member.SubJobLevel.ID.ToString();

            int nID = GetMaxID(dbMgr, "JobSubLevel") + 1;
            string strSQL = string.Format("Insert into JobSubLevel (ID, Name) values ({0}, '{1}')",
                nID, member.SubJobLevel.Name);

            if (dbMgr.GetBatchData(strSQL) == null)
                return "NULL";

            member.SubJobLevel.ID = nID;
            return nID.ToString();
        }

        public int GetMaxID(WebDBManager dbMgr, string strTableName)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private int GetOldRootTeamID()
        {
            if (m_oldTeamTree == null || m_oldTeamTree.RootNode == null)
                return -1;

            RegularTeam team = m_oldTeamTree.RootNode.Data;

            if (team == null)
                return -1;

            return team.TeamID;
        }

        private bool RemoveDBRegularTeams(WebDBManager dbMgr)
        {
            int nOldRootTeamID = GetOldRootTeamID();

            string strTeamIDs = "";

            foreach (RegularTeam team in m_removingOldTeams)
            {
                if (team.TeamID == nOldRootTeamID)
                    continue;

                if (strTeamIDs.Length == 0)
                    strTeamIDs = team.TeamID.ToString();
                else
                    strTeamIDs += ", " + team.TeamID.ToString();
            }

            if (strTeamIDs.Length > 0)
            {
                string strInsertFormat = "Insert into RegularTeam (ID, TeamName, ParentTeamID) values ({0}, '{1}', {2})";

                string strSQL = "Select ID, TeamName, ParentTeamID from RegularTeam where ID in (" + strTeamIDs + ")";
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 2; i += 3)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strTeamName = WebDBManager.GetStringField(arrResult[i + 1]);
                    VariousData<int> parentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString());

                    if (id == null || strTeamName == null)
                        continue;

                    string strInsert = string.Format(strInsertFormat, id.Data, strTeamName,
                        parentTeamID == null ? "NULL" : parentTeamID.Data.ToString());
                    m_rollbackQueries.Add(strInsert);
                }
            }

            string strSQL2 = "Delete from RegularTeam";
            //strSQL = "Delete from RegularTeam where ID in (" + strTeamIDs + ")";
            return dbMgr.GetBatchData(strSQL2) != null;
        }

        private bool RemoveDBRegularMembers(WebDBManager dbMgr)
        {
            string strMemberIDs = "";

            foreach (CompanyMember member in m_removingOldCompanyMembers)
            {
                if (strMemberIDs.Length == 0)
                    strMemberIDs = member.ID.ToString();
                else
                    strMemberIDs += ", " + member.ID.ToString();
            }

            if (strMemberIDs.Length > 0)
            {
                string strInsertFormat = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values ";
                strInsertFormat += "({0}, {1}, {2}, {3}, {4})";

                string strSQL = "Select RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID from RegularMemberList where ";
                string strWhere = "CompanyMemberID in (" + strMemberIDs + ")";

                strSQL += strWhere;

                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 4; i += 5)
                {
                    VariousData<int> teamID = WebDBManager.GetIntField(arrResult[i].ToString());
                    VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                    VariousData<int> positionID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                    VariousData<int> subPositionID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                    VariousData<int> groupPositionID = WebDBManager.GetIntField(arrResult[i + 4].ToString());

                    if (teamID == null || memberID == null || positionID == null)
                        continue;

                    string strInsert = string.Format(strInsertFormat, teamID.Data, memberID.Data, positionID.Data,
                        subPositionID == null ? "NULL" : subPositionID.Data.ToString(),
                        groupPositionID == null ? "NULL" : groupPositionID.Data.ToString());

                    m_rollbackQueries.Add(strInsert);
                }
            }

            string strSQL2 = "Delete from RegularMemberList";
            //strSQL = "Delete from RegularMemberList where " + strWhere;
            return dbMgr.GetBatchData(strSQL2) != null;
        }

        private bool RemoveDBCompanyMembers(WebDBManager dbMgr)
        {
            string strIDs = "";

            foreach (CompanyMember member in m_removingOldCompanyMembers)
            {
                if (strIDs.Length == 0)
                    strIDs = member.ID.ToString();
                else
                    strIDs += ", " + member.ID.ToString();
            }

            if (strIDs.Length > 0)
            {
                string strInsertFormat = "Insert into CompanyMember (ID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber, SubLevelID) values ";
                strInsertFormat += "({0}, '{1}', {2}, {3}, {4}, {5}, {6})";

                string strSQL = "Select ID, MemberName, LevelID, MemberID, OfficePhoneNumber, PhoneNumber, SubLevelID from CompanyMember where ID in (" + strIDs + ")";
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                    return false;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 6; i += 7)
                {
                    VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                    string strMemberName = WebDBManager.GetStringField(arrResult[i + 1]);
                    VariousData<int> levelID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                    string strMemberID = WebDBManager.GetStringField(arrResult[i + 3]);
                    string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 4]);
                    string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 5]);
                    VariousData<int> subLevelID = WebDBManager.GetIntField(arrResult[i + 6].ToString());

                    if (id == null || strMemberName == null || levelID == null)
                        continue;

                    string strInsert = string.Format(strInsertFormat, id.Data, strMemberName, levelID.Data,
                        strMemberID == null ? "NULL" : "'" + strMemberID + "'",
                        strOfficePhoneNumber == null ? "NULL" : "'" + strOfficePhoneNumber + "'",
                        strPhoneNumber == null ? "NULL" : "'" + strPhoneNumber + "'",
                        subLevelID == null ? "NULL" : subLevelID.Data.ToString());

                    m_rollbackQueries.Add(strInsert);
                }
            }

            string strSQL2 = "Delete from CompanyMember";
            //strSQL = "Delete from CompanyMember where ID in (" + strIDs + ")";
            return dbMgr.GetBatchData(strSQL2) != null;
        }

        private bool RemoveDBTemporaryMembers(WebDBManager dbMgr)
        {
            if (RemoveDBTemporaryMembers(dbMgr, m_dicRemovingOldTemporaryNormalMembers, true) == false)
                return false;
            if (RemoveDBTemporaryMembers(dbMgr, m_dicRemovingOldTemporaryEmergencyMembers, false) == false)
                return false;

            return true;
        }

        private bool RemoveDBTemporaryMembers(WebDBManager dbMgr, Dictionary<Team, List<TemporaryMember>> dicTemporaryMembers, bool isNormal)
        {
            string strIDs = "";

            foreach (KeyValuePair<Team, List<TemporaryMember>> pair in dicTemporaryMembers)
            {
                foreach (TemporaryMember member in pair.Value)
                {
                    if (strIDs.Length == 0)
                        strIDs = member.ID.ToString();
                    else
                        strIDs += ", " + member.ID.ToString();
                }
            }

            if (strIDs.Length == 0)
                return true;

            string strInsertFormat = "Insert into TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) values ";
            strInsertFormat += "({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            string strSQL = "Select ID, MemberName, TemporaryTeamID, MemberID, IsTeamLeader, MemberType, MemberCount, Role from TemporaryMemberList where ID in (" + strIDs + ")";
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return false;

            int nNormal = isNormal ? 1 : 0;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-7;i+=8)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> teamID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> isTeamLeader = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> memberType = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> memberCount = WebDBManager.GetIntField(arrResult[i + 6].ToString());
                VariousData<int> role = WebDBManager.GetIntField(arrResult[i + 7].ToString());

                if (id == null || strMemberName == null || teamID == null || memberID == null || memberType == null)
                    continue;

                string strInsert = string.Format(strInsertFormat, id.Data, strMemberName, teamID.Data, nNormal, memberID.Data,
                    isTeamLeader == null ? "NULL" : isTeamLeader.Data.ToString(),
                    memberType.Data, memberCount == null ? "NULL" : memberCount.Data.ToString(),
                    role == null ? "NULL" : role.Data.ToString());

                m_rollbackQueries.Add(strInsert);
            }

            strSQL = "Delete from TemporaryMemberList where ID in (" + strIDs + ")";
            return dbMgr.GetBatchData(strSQL) != null;
        }

        private bool RemoveDBFacilityManagers(WebDBManager dbMgr)
        {
            string strFacilityManagerIDs = GetTargetFacilityManagerIDs(m_removingOldTeams, m_removingOldCompanyMembers, dbMgr, "FacilityManager");
            string strBuildingFacilityManagerIDs = GetTargetFacilityManagerIDs(m_removingOldTeams, m_removingOldCompanyMembers, dbMgr, "BuildingFacilityManager", "BuildingID");
            string strEquipZoneFacilityManagerIDs = GetTargetFacilityManagerIDs(m_removingOldTeams, m_removingOldCompanyMembers, dbMgr, "EquipZoneFacilityManager", "EquipZoneID");

            if (RemoveFacilityManager(dbMgr, "FacilityManager", strFacilityManagerIDs) == false)
                return false;
            if (RemoveFacilityManager(dbMgr, "BuildingFacilityManager", strBuildingFacilityManagerIDs) == false)
                return false;
            if (RemoveFacilityManager(dbMgr, "EquipZoneFacilityManager", strEquipZoneFacilityManagerIDs) == false)
                return false;

            return true;
        }

        private bool RemoveFacilityManager(WebDBManager dbMgr, string strTableName, string strIDs)
        {
            if (strIDs.Length == 0)
                return true;

            string strSQL = string.Format("Delete from {0} where ID in ({1})", strTableName, strIDs);
            return dbMgr.GetBatchData(strSQL) != null;
        }

        private void GetTeamNMemberIDs(List<RegularTeam> teams, List<CompanyMember> members, ref string strTeamIDs, ref string strMemberIDs)
        {
            foreach (RegularTeam team in teams)
            {
                if (team.TeamID > 0)
                {
                    if (strTeamIDs.Length == 0)
                        strTeamIDs = team.TeamID.ToString();
                    else
                        strTeamIDs += ", " + team.TeamID.ToString();
                }
            }

            foreach (CompanyMember member in members)
            {
                if (member.ID > 0)
                {
                    if (strMemberIDs.Length == 0)
                        strMemberIDs = member.ID.ToString();
                    else
                        strMemberIDs += ", " + member.ID.ToString();
                }
            }
        }

        private string GetTargetFacilityManagerIDs(List<RegularTeam> teams, List<CompanyMember> members, WebDBManager dbMgr, string strTableName, string strFieldName = "")
        {
            string strTeamIDs = "", strMemberIDs = "";
            GetTeamNMemberIDs(teams, members, ref strTeamIDs, ref strMemberIDs);

            if (strTeamIDs.Length == 0 && strMemberIDs.Length == 0)
                return "";

            string strIDs = "";
            string strInsertFormat = "Insert into " + strTableName + " (ID, MemberID, MemberType, FacilityType, LevelLimit, " + strFieldName + "UpperLimit, SiteID, Description) values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}";//, {8})";

            int nFieldCount = 7;

            if (strFieldName.Length > 0)
            {
                strFieldName += ", ";
                nFieldCount++;
                strInsertFormat += ", {8})";
            }
            else
                strInsertFormat += ")";

            string strSQL = "";

            if (strTeamIDs.Length == 0)
            {
                strSQL = string.Format("Select ID, MemberID, MemberType, FacilityType, LevelLimit, " + strFieldName + "UpperLimit, Description from " + strTableName + " where MemberType = 0 and SiteID = {0} and MemberID in ({1})",
                    FormMain.Instance.SiteID, strMemberIDs);
            }
            else if (strMemberIDs.Length == 0)
            {
                strSQL = string.Format("Select ID, MemberID, MemberType, FacilityType, LevelLimit, " + strFieldName + "UpperLimit, Description from " + strTableName + " where MemberType = 1 and SiteID = {0} and MemberID in ({1})",
                    FormMain.Instance.SiteID, strTeamIDs);
            }
            else
            {
                strSQL = string.Format("Select ID, MemberID, MemberType, FacilityType, LevelLimit, " + strFieldName + "UpperLimit, Description from " + strTableName + " where ((MemberType = 1 and MemberID in ({1})) or (MemberType = 0 and MemberID in ({2}))) and SiteID = {0}",
                    FormMain.Instance.SiteID, strTeamIDs, strMemberIDs);
            }

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return "";

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> memberType = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> facilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> levelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> targetID = strFieldName.Length > 0 ? WebDBManager.GetIntField(arrResult[i + nFieldCount - 3].ToString()) : null;
                VariousData<int> upperLimit = WebDBManager.GetIntField(arrResult[i + nFieldCount - 2].ToString());
                string strDescription = WebDBManager.GetStringField(arrResult[i + nFieldCount - 1]);

                if (id == null || memberID == null || memberType == null || facilityType == null || (strFieldName.Length > 0 && targetID == null))
                    continue;

                string strLevelLimit = levelLimit == null ? "NULL" : levelLimit.Data.ToString();
                string strUpperLimit = upperLimit == null ? "NULL" : upperLimit.Data.ToString();

                if (strDescription == null)
                    strDescription = "NULL";
                else
                    strDescription = "'" + strDescription + "'";

                string strInsert = "";

                if (targetID != null)
                {
                    strInsert = string.Format(strInsertFormat, id.Data, memberID.Data, memberType.Data, facilityType.Data,
                                    strLevelLimit, targetID.Data, strUpperLimit, FormMain.Instance.SiteID, strDescription);
                }
                else
                {
                    strInsert = string.Format(strInsertFormat, id.Data, memberID.Data, memberType.Data, facilityType.Data,
                                    strLevelLimit, strUpperLimit, FormMain.Instance.SiteID, strDescription);
                }

                m_rollbackQueries.Add(strInsert);

                if (strIDs.Length == 0)
                    strIDs = id.Data.ToString();
                else
                    strIDs += ", " + id.Data.ToString();
            }

            return strIDs;
        }

        // 삭제되지 않고 남아있을 직원들의 복사본을 만든다.
        public void CopyNotRemovingOldCompanyMembers(List<CompanyMember> members)
        {
            m_notRemovingOldCompanyMembers = new List<CompanyMember>();

            foreach (CompanyMember member in members)
            {
                CompanyMember _member = new CompanyMember();
                _member.CopyFrom(member);
                m_notRemovingOldCompanyMembers.Add(_member);
            }
        }

        private void RemoveNewMembers()
        {
            RemoveNewRegularTeams(m_teamTree.RootNode, FormMain.Instance.RegularTeamTree.Nodes);
            RemoveNewCompanyMembers();
        }

        private void RemoveNewCompanyMembers()
        {
            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicRegularMembers)
            {
                // TeamID가 0보다 작은 신규 팀들은 RemoveNewRegularTeams(...)에서 이미 제거되었다.
                if (pair.Key.TeamID < 0)
                    continue;

                RegularTeam team = DataManager.GetRegularTeam(pair.Key.TeamID);

                if (team == null)
                    continue;

                List<CompanyMember> members = DataManager.GetRegularMembers(team);

                if (members == null)
                    continue;

                foreach (CompanyMember member in pair.Value)
                {
                    if (member.ID < 0)
                    {
                        foreach (CompanyMember member2 in members)
                        {
                            if (member2.MemberID == member.MemberID && member2.Name == member.Name)
                            {
                                members.Remove(member2);
                                break;
                            }
                        }
                    }
                    else
                    {
                        CompanyMember originMember = DataManager.GetCompanyMember(member.ID);

                        if (originMember != null)
                        {
                            // 바뀌기 전의 원래 직원정보로 되돌린다.
                            foreach (CompanyMember member2 in m_notRemovingOldCompanyMembers)
                            {
                                if (member2.ID == member.ID)
                                {
                                    originMember.CopyFrom(member2);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RemoveNewRegularTeams(Tree<RegularTeam>.Node node, TreeNodeCollection nodes)
        {
            if (node.Data != null)
            {
                if (node.Data.TeamID < 0)
                    RemoveRegularTeamNodes(node, nodes);
                else
                {
                    TreeNode node2 = FindRegularTeam(node.Data.TeamID, nodes);

                    if (node2 != null)
                    {
                        foreach (Tree<RegularTeam>.Node child in node.Children)
                        {
                            RemoveNewRegularTeams(child, node2.Nodes);
                        }
                    }
                }
            }
        }

        private TreeNode FindRegularTeam(int nTeamID, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && (node.Tag is RegularTeam))
                {
                    RegularTeam team = (RegularTeam)node.Tag;

                    if (team.TeamID == nTeamID)
                        return node;
                }
            }

            return null;
        }

        private void RemoveRegularTeamNodes(Tree<RegularTeam>.Node node, TreeNodeCollection nodes)
        {
            RegularTeam team = node.Data;

            foreach (TreeNode node2 in nodes)
            {
                if (node2.Tag != null && (node2.Tag is RegularTeam))
                {
                    RegularTeam team2 = (RegularTeam)node2.Tag;

                    if (team.TeamName == team2.TeamName)
                    {
                        RemoveRegularTeamNodes(node2);
                        nodes.Remove(node2);
                        return;
                    }
                }
            }
        }

        // DataManager의 데이터를 삭제한다.
        private void RemoveRegularTeamNodes(TreeNode node)
        {
            if (node.Tag != null && (node.Tag is RegularTeam))
            {
                RegularTeam team = (RegularTeam)node.Tag;
                DataManager.RemoveRegularTeam(team);
            }

            foreach (TreeNode child in node.Nodes)
            {
                RemoveRegularTeamNodes(child);
            }
        }

        private void LoadOldMembers()
        {
            if (m_oldTeamTree == null || m_dicOldRegularMembers == null)
                return;

            LoadOldRegularTeams(m_oldTeamTree.RootNode, FormMain.Instance.RegularTeamTree.Nodes, 0);
            LoadOldCompanyMembers();
            LoadOldTemporaryMembers();

            FormMain.Instance.RegularTeamTree.ExpandAll();
            SelectFirstNode(FormMain.Instance.RegularTeamTree);
        }

        private void LoadOldTemporaryMembers()
        {
            LoadOldTemporaryMembers(FormMain.Instance.TemporaryNormalTeamTree, m_dicRemovingOldTemporaryNormalMembers, true);
            LoadOldTemporaryMembers(FormMain.Instance.TemporaryEmergencyTeamTree, m_dicRemovingOldTemporaryEmergencyMembers, false);
        }

        private void LoadOldTemporaryMembers(TeamTreeView tree, Dictionary<Team, List<TemporaryMember>> dicTemporaryMembers, bool isNormal)
        {
            foreach (KeyValuePair<Team, List<TemporaryMember>> pair in dicTemporaryMembers)
            {
                List<TemporaryMember> members1 = null, members2 = null;

                if (isNormal)
                {
                    TemporaryNormalTeam team = (TemporaryNormalTeam)pair.Key;
                    members1 = team.Members;
                    members2 = DataManager.GetTemporaryNormalMembers(team);
                }
                else
                {
                    TemporaryEmergencyTeam team = (TemporaryEmergencyTeam)pair.Key;
                    members1 = team.Members;
                    members2 = DataManager.GetTemporaryEmergencyMembers(team);
                }

                foreach (TemporaryMember member in pair.Value)
                {
                    members1.Add(member);

                    if (members1 != members2)
                    {
                        members2.Add(member);
                    }
                }
            }
        }

        private TreeNode FindTemporaryTeam(TreeNodeCollection nodes, Team team)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && (node.Tag is Team))
                {
                    Team team2 = (Team)node.Tag;

                    if (team2 == team)
                        return node;
                }

                TreeNode result = FindTemporaryTeam(node.Nodes, team);

                if (result != null)
                    return result;
            }

            return null;
        }

        private void LoadOldCompanyMembers()
        {
            foreach (CompanyMember member in m_removingOldCompanyMembers)
            {
                DataManager.AddCompanyMember(member);
            }

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicOldRegularMembers)
            {
                RegularTeam team = DataManager.GetRegularTeam(pair.Key.TeamID);

                if (team == null)
                    continue;

                List<CompanyMember> members = DataManager.GetRegularMembers(team);

                if (members != null)
                {
                    foreach (CompanyMember member in pair.Value)
                    {
                        CompanyMember member2 = FindCompanyMember(member.ID, members);

                        if (member2 == null)
                        {
                            member2 = DataManager.GetCompanyMember(member.ID);

                            if (member2 != null)
                                members.Add(member2);
                        }
                    }
                }
            }
        }

        private void LoadOldRegularTeams(Tree<RegularTeam>.Node node, TreeNodeCollection nodes, int nIndex)
        {
            if (node.Data != null)
            {
                if (m_removingOldTeams.Contains(node.Data))
                {
                    TreeNode node2 = new TreeNode();
                    node2.Text = node.Data.TeamName;
                    node2.Tag = node.Data;
                    nodes.Insert(nIndex, node2);
                    //nodes.Add(node2);

                    DataManager.AddRegularTeam(node.Data);
                    int nChildIndex = 0;

                    foreach (Tree<RegularTeam>.Node child in node.Children)
                    {
                        LoadOldRegularTeams(child, node2.Nodes, nChildIndex++);
                    }
                }
                else if (node.Data.TeamID > 0)
                {
                    TreeNode node2 = FindRegularTeam(node.Data.TeamID, nodes);

                    if (node2 != null)
                    {
                        int nChildIndex = 0;

                        foreach (Tree<RegularTeam>.Node child in node.Children)
                        {
                            LoadOldRegularTeams(child, node2.Nodes, nChildIndex++);
                        }
                    }
                }
            }
        }

        private void LoadNewMembers()
        {
            LoadNewTeams(m_teamTree.RootNode);

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicRegularMembers)
            {
                List<CompanyMember> members = null;

                if (pair.Key.TeamID < 0)
                {
                    // 새로 추가된 팀
                    DataManager.AddRegularTeam(pair.Key);
                    members = DataManager.GetRegularMembers(pair.Key);
                }
                else
                {
                    // 기존에 존재하면서 삭제되지 않은 팀
                    RegularTeam team = DataManager.GetRegularTeam(pair.Key.TeamID);
                    members = DataManager.GetRegularMembers(team);
                }

                foreach (CompanyMember member in pair.Value)
                {
                    if (member.ID < 0)
                    {
                        DataManager.AddCompanyMember(member);
                        members.Add(member);
                    }
                    else
                    {
                        // 기존에 존재하는 직원의 정보를 member의 정보로 바꾼다.
                        CompanyMember oldMember = DataManager.GetCompanyMember(member.ID);

                        if (oldMember != null)
                            oldMember.CopyFrom(member);
                    }
                }
            }

            FormMain.Instance.RegularTeamTree.LoadRegularTeam(m_teamTree);
            SelectFirstNode(FormMain.Instance.RegularTeamTree);
        }

        private void LoadNewTeams(Tree<RegularTeam>.Node node)
        {
            if (node == null)
                return;

            RegularTeam team = node.Data;

            if (team == null)
                return;

            if (team.TeamID < 0)
            {
                DataManager.AddRegularTeam(team);
                DataManager.GetRegularMembers(team);
            }

            foreach (Tree<RegularTeam>.Node child in node.Children)
            {
                LoadNewTeams(child);
            }
        }

        private void SelectFirstNode(TeamTreeView tree)
        {
            foreach (TreeNode node in tree.Nodes)
            {
                tree.SelectedNode = node;
                break;
            }
        }

        private void DeleteOldMembers()
        {
            m_dicRemovingOldTemporaryNormalMembers.Clear();
            m_dicRemovingOldTemporaryEmergencyMembers.Clear();

            DeleteOldTemporaryMembers(FormMain.Instance.TemporaryNormalTeamTree, true);
            DeleteOldTemporaryMembers(FormMain.Instance.TemporaryEmergencyTeamTree, false);
            DeleteRegularMembers();
        }

        // Tree의 제일 아래쪽에서부터 역순으로 지운다.
        private void DeleteRegularMemberTree(TreeNodeCollection nodes)
        {
            List<TreeNode> removeNodes = new List<TreeNode>();

            foreach (TreeNode node in nodes)
            {
                DeleteRegularMemberTree(node.Nodes);

                if (node.Tag != null && (node.Tag is RegularTeam))
                {
                    RegularTeam team = (RegularTeam)node.Tag;

                    if (m_removingOldTeams.Contains(team))
                        removeNodes.Add(node);
                }
            }

            foreach (TreeNode node in removeNodes)
            {
                nodes.Remove(node);
            }
        }

        private void DeleteRegularMembers()
        {
            DeleteRegularMemberTree(FormMain.Instance.RegularTeamTree.Nodes);
            //FormMain.Instance.RegularTeamTree.Nodes.Clear();

            foreach (RegularTeam team in m_removingOldTeams)
            {
                DataManager.RemoveRegularTeam(team);
            }

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> pair in m_dicOldRegularMembers)
            {
                List<CompanyMember> members = DataManager.GetRegularMembers(pair.Key);

                if (members != null)
                {
                    foreach (CompanyMember member in pair.Value)
                    {
                        if (FindMember(member.ID, m_notRemovingOldCompanyMembers) == null)
                        {
                            members.Remove(member);
                            DataManager.RemoveCompanyMember(member);
                        }
                    }
                }
            }
        }

        private CompanyMember FindMember(int nMemberID, List<CompanyMember> members)
        {
            foreach (CompanyMember member in members)
            {
                if (member.ID == nMemberID)
                    return member;
            }

            return null;
        }

        private void DeleteOldTemporaryMembers(TeamTreeView tempTree, bool isNormal)
        {
            List<TemporaryMember> removingMembers = isNormal ? m_removingOldNormalMembers : m_removingOldEmergencyMembers;

            foreach (TreeNode node in tempTree.Nodes)
            {
                if (node.Tag != null && ((isNormal && (node.Tag is TemporaryNormalTeam)) || (!isNormal && (node.Tag is TemporaryEmergencyTeam))))
                {
                    List<TemporaryMember> members = null;

                    if (isNormal && (node.Tag is TemporaryNormalTeam))
                    {
                        TemporaryNormalTeam team = (TemporaryNormalTeam)node.Tag;
                        members = DataManager.GetTemporaryNormalMembers(team);
                    }
                    else if (!isNormal && (node.Tag is TemporaryEmergencyTeam))
                    {
                        TemporaryEmergencyTeam team = (TemporaryEmergencyTeam)node.Tag;
                        members = DataManager.GetTemporaryEmergencyMembers(team);
                    }

                    if (members != null)
                    {
                        foreach (TemporaryMember member in members)
                        {
                            if (member.TemporaryMemberType == TemporaryMember.MemberType.RegularTeam)
                            {
                                int nTeamID = member.MemberID;

                                if (nTeamID < 0)
                                    nTeamID = -nTeamID;

                                if (FindRegularTeam(nTeamID, m_removingOldTeams) != null)
                                    removingMembers.Add(member);
                            }
                            else if (member.TemporaryMemberType == TemporaryMember.MemberType.CompanyMember)
                            {
                                int nMemberID = member.MemberID;

                                if (FindCompanyMember(nMemberID, m_removingOldCompanyMembers) != null)
                                    removingMembers.Add(member);
                            }
                        }
                    }
                }
            }

            foreach (TemporaryMember member in removingMembers)
            {
                List<TemporaryMember> oldMembers = null;

                if (isNormal)
                {
                    TemporaryNormalTeam team = (TemporaryNormalTeam)member.Team;
                    team.Members.Remove(member);

                    List<TemporaryMember> members = DataManager.GetTemporaryNormalMembers(team);
                    members.Remove(member);

                    if (!m_dicRemovingOldTemporaryNormalMembers.TryGetValue(team, out oldMembers))
                    {
                        oldMembers = new List<TemporaryMember>();
                        m_dicRemovingOldTemporaryNormalMembers[team] = oldMembers;
                    }
                }
                else
                {
                    TemporaryEmergencyTeam team = (TemporaryEmergencyTeam)member.Team;
                    team.Members.Remove(member);

                    List<TemporaryMember> members = DataManager.GetTemporaryEmergencyMembers(team);
                    members.Remove(member);

                    if (!m_dicRemovingOldTemporaryEmergencyMembers.TryGetValue(team, out oldMembers))
                    {
                        oldMembers = new List<TemporaryMember>();
                        m_dicRemovingOldTemporaryEmergencyMembers[team] = oldMembers;
                    }
                }

                oldMembers.Add(member);
            }
        }

        private CompanyMember FindCompanyMember(int nMemberID, List<CompanyMember> members)
        {
            foreach (CompanyMember member in members)
            {
                if (nMemberID == member.ID)
                    return member;
            }

            return null;
        }

        private RegularTeam FindRegularTeam(int nTeamID, List<RegularTeam> teams)
        {
            foreach (RegularTeam team in teams)
            {
                if (team.TeamID == nTeamID)
                    return team;
            }

            return null;
        }

        private void SaveOldMembers()
        {
            m_oldTeamTree = new Tree<RegularTeam>();
            m_dicOldRegularMembers = new Dictionary<RegularTeam, List<CompanyMember>>();

            if (FormMain.Instance.RegularTeamTree.Nodes.Count > 0)
            {
                TreeNode rootNode = FormMain.Instance.RegularTeamTree.Nodes[0];
                SaveOldMember(m_oldTeamTree.RootNode, rootNode);
            }
        }

        private void SaveOldMember(Tree<RegularTeam>.Node nodeTarget, TreeNode nodeSource)
        {
            if (nodeSource.Tag != null && (nodeSource.Tag is RegularTeam))
            {
                RegularTeam team = (RegularTeam)nodeSource.Tag;
                nodeTarget.Data = team;

                List<CompanyMember> members = DataManager.GetRegularMembers(team);

                // members를 m_dicOldRegularMembers에 바로 넣지 않고 복사본을 만든다.
                if (members != null)
                {
                    List<CompanyMember> members2 = new List<CompanyMember>();
                    members2.AddRange(members);
                    m_dicOldRegularMembers[team] = members2;
                }

                foreach (TreeNode child in nodeSource.Nodes)
                {
                    Tree<RegularTeam>.Node node = new Tree<RegularTeam>.Node();
                    nodeTarget.AddChild(node);

                    SaveOldMember(node, child);
                }
            }
        }
    }
}
