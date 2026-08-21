using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUtility2;

namespace TeamEditor.Command
{
    public class CommandRemoveRegularTeam : CommandEx
    {
        private TreeNode m_parent = null;
        private TreeNode m_node = null;
        private RegularTeam m_team = null;
        private int m_nIndex = -1;
        private List<string> m_rollbackSQLs = new List<string>();

        // 팀 최상위 부모로부터 몇단계 떨어진 자식인지 여부에 따라 정렬되어 있다.
        private List<RegularTeam> m_removeTeamList = new List<RegularTeam>();
        private Dictionary<RegularTeam, List<CompanyMember>> m_removeTeamCompanyMemberList = new Dictionary<RegularTeam, List<CompanyMember>>();
        private List<RegularTeam> m_removeRegularTeamList = new List<RegularTeam>();

        public TreeNode TreeNode
        {
            get { return m_node; }
            set
            {
                m_node = value;

                if (m_node == null)
                    m_nIndex = -1;
                else
                {
                    if (m_node.Parent == null)
                        m_nIndex = -1;
                    else
                        m_nIndex = m_node.Parent.Nodes.IndexOf(m_node);

                    m_parent = m_node.Parent;
                }
            }
        }

        public RegularTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public CommandRemoveRegularTeam()
        {
        }

        public CommandRemoveRegularTeam(TreeNode node, RegularTeam team)
        {
            Team = team;
            TreeNode = node;
        }

        public CommandRemoveRegularTeam(TreeNode node, TreeNode nodeParent, int nNodeIndex, RegularTeam team)
        {
            m_node = node;
            m_parent = nodeParent;
            m_nIndex = nNodeIndex;
            m_team = team;
        }

        public override void Do()
        {
            m_removeTeamCompanyMemberList.Clear();
            m_removeRegularTeamList.Clear();

            if (m_node != null)
            { 
                if (m_parent != null)
                {
                    // Team Visible False로 변경 -> Grid에 표현안함
                    RegularTeam team = m_node.Tag as RegularTeam;
                    if (team != null)
                    { 
                        List<Team> childTeams = DataManager.GetRegularTeams(team.TeamID);
                                                
                        foreach (RegularTeam item in childTeams)
                        {
                            if (item.TeamID > 0)
                                m_removeRegularTeamList.Add(item);

                            List<CompanyMember> members = DataManager.GetRegularMembers(item);
                            m_removeTeamCompanyMemberList[item] = members;
                            //DataManager.RemoveRegularTeam(item);

                            DataManager.SetReularTeamVisible(item, false); 
                        }
                        FormMain.Instance.SetRegularTeamComboItems();
                    } 

                    if (m_nIndex >= 0)
                        m_parent.Nodes.Remove(m_parent.Nodes[m_nIndex]);

                    FormMain.Instance.SelectRegularTeam((RegularTeam)FormMain.Instance.RegularTeamTree.SelectedNode.Tag, true);
                }  
            }
        }

        public override void RollBack()
        {
            if (m_node != null)
            {
                if (m_parent != null)
                {
                    // Team Visible True로 변경 -> Grid에 표현함
                    foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_removeTeamCompanyMemberList)
                    {
                        RegularTeam team = DataManager.GetRegularTeam(item.Key.TeamID);
                        if (team == null)
                        {
                            DataManager.SetRegularTeam(item.Key.TeamID, item.Key);
                            DataManager.SetRegularMembers(item.Key, item.Value);
                        }
                        DataManager.SetReularTeamVisible(item.Key, true);

                        foreach (CompanyMember member in item.Value)
                        {
                            DataManager.AddCompanyMember(member);
                        }
                    }

                    m_removeRegularTeamList.Clear();

                    if (m_nIndex >= 0)
                        m_parent.Nodes.Insert(m_nIndex, m_node);
                    else
                        m_parent.Nodes.Add(m_node); 
                     
                    FormMain.Instance.SetRegularTeamComboItems();
                    FormMain.Instance.SelectRegularTeam((RegularTeam)FormMain.Instance.RegularTeamTree.SelectedNode.Tag, true);
                }
            }
        }

        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            if (m_node == null || m_parent == null)
                return;
             
            if (dir)
            {
                m_rollbackSQLs.Clear();
                //if (m_rollbackSQLs.Count == 0)                
                    RemoveDB(dbMgr);
            }
            else
            {
                if (m_parent.Nodes.Contains(m_node))
                { 
                    // 삭제했다가 RollBack한 상태인 경우 DB에서는 지워졌지만 UI에는 다시 나타나있다.
                    AddDB(dbMgr);
                }
            }

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.REGULAR_TEAM);

            /*if (m_parent.Nodes.Contains(m_node))
            {
                // 삭제했다가 RollBack한 상태인 경우 DB에서는 지워졌지만 UI에는 다시 나타나있다.
                AddDB(dbMgr);
            }
            else
            {
                if (m_rollbackSQLs.Count == 0)
                    RemoveDB(dbMgr);
            }*/
        }

        // 팀들을 최상위 부모로부터 떨어진 거리에 따라 정렬하여 리턴한다.
        private string GetTeamList()
        {
            // 저장되기전 Node가 삭제되면 ID가 할당되지 않아서 팀을 찾을 수 없다.
            //int searchTeamID = m_team.TeamID;
            //if (searchTeamID < 0)
            //{
            //    if (DataManager.DicSaveRegularTeamIDs.ContainsKey(m_team.TeamID))
            //        searchTeamID = DataManager.DicSaveRegularTeamIDs[m_team.TeamID];
            //}

            //m_removeTeamList = DataManager.GetRegularTeams(searchTeamID);

            m_removeTeamList = m_removeRegularTeamList;
            string strTeamIDs = "";

            foreach (RegularTeam team in m_removeTeamList)
            {
                if (strTeamIDs.Length == 0)
                    strTeamIDs = team.TeamID.ToString();
                else
                    strTeamIDs += ", " + team.TeamID.ToString();
            } 

            if (strTeamIDs.Length > 0)
                strTeamIDs = "(" + strTeamIDs + ")";

            return strTeamIDs;
        }

        /*private void GetTeamList(TreeNode node, int nDepth, List<TeamNDepth> teams)
        {
            if (node.Tag != null)
            {
                int nTeamID = (int)node.Tag;
                RegularTeam team = DataManager.GetRegularTeam(nTeamID);

                TeamNDepth _team = new TeamNDepth(team, nDepth);
                teams.Add(_team);
            }

            foreach (TreeNode child in node.Nodes)
            {
                GetTeamList(child, nDepth + 1, teams);
            }
        }*/

        // 비상조직의 MemberID 검색을 위하여 TeamID가 양수인것과 음수인것들 모두의 리스트를 만든다.
        private string GetPlusMinusTeamIDs()
        {
            string strTeamIDs = "";

            foreach (RegularTeam team in m_removeTeamList)
            {
                if (strTeamIDs.Length == 0)
                    strTeamIDs = team.TeamID.ToString() + ", " + (-team.TeamID).ToString();
                else
                    strTeamIDs += team.TeamID.ToString() + ", " + (-team.TeamID).ToString();
            }

            if (strTeamIDs.Length > 0)
                strTeamIDs = "(" + strTeamIDs + ")";

            return strTeamIDs;
        }

        private void RemoveDB(WebDBManager dbMgr)
        {
            string strTeamIDs = GetTeamList();

            if (strTeamIDs.Length == 0)
                return;

            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

            string strCompanyMemberIDs;
            List<string> insertRegularMemberList = RemoveRegularMemberList(dbMgr, strTeamIDs, out strCompanyMemberIDs);

            if (insertRegularMemberList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            List<string> insertFacilityManagerList = RemoveFacilityManagers(dbMgr, strTeamIDs, strCompanyMemberIDs);

            if (insertFacilityManagerList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            // TeamID에 '-'를 붙인 리스트를 더한다.
            string strPlusMinusTeamIDs = GetPlusMinusTeamIDs();
            List<string> insertTemporaryMemberList = RemoveTemporaryMembers(dbMgr, strTeamIDs, strPlusMinusTeamIDs, strCompanyMemberIDs);

            if (insertTemporaryMemberList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            /*List<string> insertDutyList = RemoveDuty(dbMgr, strTeamIDs);

            if (insertDutyList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }*/

            List<string> updateSOPGenUserList = UpdateSOPGenUsers(dbMgr, strCompanyMemberIDs);

            if (updateSOPGenUserList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            List<string> updateProcessDataList = UpdateProcessDatas(dbMgr, strTeamIDs);

            if (updateProcessDataList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            List<string> updateInternalTransmissionDataList = UpdateInternalTransmissionDatas(dbMgr, strTeamIDs);

            if (updateInternalTransmissionDataList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            List<string> insertCompanyMemberList = RemoveCompanyMembers(dbMgr, strCompanyMemberIDs);

            if (insertCompanyMemberList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            List<string> insertRegularTeamList = RemoveRegularTeams(dbMgr, strTeamIDs);

            if (insertRegularTeamList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_removeTeamCompanyMemberList)
            {
                DataManager.RemoveRegularTeam(item.Key);
                foreach (CompanyMember member in item.Value)
                {
                    DataManager.RemoveCompanyMember(member);
                }
            }
              
            // Batch Job end - Commit
            dbMgr.BatchCommit();            

            m_rollbackSQLs.AddRange(insertRegularTeamList);
            m_rollbackSQLs.AddRange(insertCompanyMemberList);
            m_rollbackSQLs.AddRange(updateInternalTransmissionDataList);
            m_rollbackSQLs.AddRange(updateProcessDataList);
            m_rollbackSQLs.AddRange(updateSOPGenUserList);
            //m_rollbackSQLs.AddRange(insertDutyList);
            m_rollbackSQLs.AddRange(insertTemporaryMemberList);
            m_rollbackSQLs.AddRange(insertFacilityManagerList);
            m_rollbackSQLs.AddRange(insertRegularMemberList);
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        public static List<string> RemoveCompanyMembers(WebDBManager dbMgr, string strCompanyMemberIDs)
        {
            List<string> insertList = new List<string>();

            if (strCompanyMemberIDs.Length == 0)
                return insertList;

            string strInsertFormat = "Insert into CompanyMember (ID, MemberName, LevelID, SubLevelID, MemberID, OfficePhoneNumber, PhoneNumber) ";
            strInsertFormat += "values ({0}, '{1}', {2}, {3}, {4}, {5}, {6})";

            string strSQL = "SELECT ID, MemberName, LevelID, SubLevelID, MemberID, OfficePhoneNumber, PhoneNumber from CompanyMember where ID in " + strCompanyMemberIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], null);
                int nLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nSubLevelID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strMemberID = WebDBManager.GetStringField(arrResult[i + 4], null);
                string strOfficePhoneNumber = WebDBManager.GetStringField(arrResult[i + 5], null);
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 6], null);

                if (nID < 0 || strMemberName == null || nLevelID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, strMemberName, nLevelID,
                    nSubLevelID < 0 ? "NULL" : nSubLevelID.ToString(),
                    strMemberID == null || strMemberID == "null" ? "NULL" : "'" + strMemberID + "'",
                    strOfficePhoneNumber == null || strOfficePhoneNumber == "null" ? "NULL" : "'" + strOfficePhoneNumber + "'",
                    strPhoneNumber == null || strPhoneNumber == "null" ? "NULL" : "'" + strPhoneNumber + "'");

                insertList.Add(strInsert);
            }

            string strDeleteSQL = "Delete from CompanyMember where ID in " + strCompanyMemberIDs;
            if (dbMgr.GetBatchData(strDeleteSQL) == null)
                return null;

            return insertList;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveRegularTeams(WebDBManager dbMgr, string strTeamIDs)
        {
            List<string> insertList = new List<string>();

            if (strTeamIDs.Length == 0)
                return insertList;

            string strInsertFormat = "Insert into RegularTeam (ID, TeamName, ParentTeamID) ";
            strInsertFormat += "values ({0}, '{1}', {2})";

            int nRemoveCount = m_removeTeamList.Count;

            for (int i=0;i<nRemoveCount;i++)
            {
                RegularTeam team = (RegularTeam)m_removeTeamList[i];

                string strInsert = string.Format(strInsertFormat, team.TeamID, team.TeamName,
                    team.ParentTeam == null ? "NULL" : team.ParentTeam.TeamID.ToString());

                insertList.Add(strInsert);
            }

            /*string strSQL = "SELECT ID, TeamName, ParentTeamID from RegularTeam where ID in " + strTeamIDs;

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into RegularTeam (ID, TeamName, ParentTeamID) ";
            strInsertFormat += "values ({0}, '{1}', {2})";

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], null);
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                if (nID < 0 || strTeamName == null)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, strTeamName,
                    nParentTeamID < 0 ? "NULL" : nParentTeamID.ToString());

                insertList.Add(strInsert);
            }*/

            string strDeleteSQL = "Delete from RegularTeam where ID in " + strTeamIDs;
            if (dbMgr.GetBatchData(strDeleteSQL) == null)
                return null;

            return insertList;
        }

        // RollBack을 위하여 수정한 데이터들의 Update 구문 리스트를 반환한다.
        private List<string> UpdateInternalTransmissionDatas(WebDBManager dbMgr, string strTeamIDs)
        {
            // 2015-08-07 mwkim 변경사항
            // InternalTransmission 의 발신자, 수신자에서 삭제되도록 적용

            List<string> updateList = new List<string>();

            if (strTeamIDs.Length == 0)
                return updateList;

            List<int> regularTeamIDs = GetRegularTeamIDs(strTeamIDs);

            // Key : 팀 Type
            //       0 : 평일 비상 조직-TemporaryNormalTeam, 
            //       1 : 휴일 비상 조직-TemporaryEmergencyTeam, 
            //       2 : 외부 기관-ExternalTeam, 
            //       3 : 사용자 정의 조직-UserDefinedTeam, 
            //       4 : 상시조직-RegularTeam)
            // Value : 타입별 Team ID List
            Dictionary<int, List<int>> dicTeamIDList = new Dictionary<int, List<int>>();

            string strSQL = string.Empty;
            strSQL += "SELECT InternalTransmission.ID, InternalTransmission.TeamList, InternalTransmission.CommanderMemberType, InternalTransmission.CommanderMemberID, InternalTransmission.CommanderDisplayText ";
            strSQL += "FROM InternalTransmission, StepMember, ActionStep, Disaster, Version ";
            strSQL += "WHERE InternalTransmission.StepMemberID = StepMember.ID ";
            strSQL += "AND StepMember.ActionStepID = ActionStep.ID ";
            strSQL += "AND ActionStep.DisasterID = Disaster.ID ";
            strSQL += "AND Disaster.VersionID = Version.ID ";
            strSQL += "AND Version.SiteID = " + FormMain.Instance.SiteID.ToString();

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            string strUpdateFormat = "UPDATE InternalTransmission SET TeamList = '{0}', CommanderMemberType = {1}, CommanderMemberID = {2}, CommanderDisplayText = '{3}' WHERE ID = {4}";

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamList = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nCommanderType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 4], "");

                if (nID < 0 || strTeamList.Length == 0)
                    continue;

                int nChangedCommanderType = nCommanderType;
                int nChangedCommanderMemberID = nCommanderMemberID;
                string strChangedCommanderDisplayText = strCommanderDisplayText;
                string strChangedTeamIDList = string.Empty;
                
                // 변경될 수신자 목록 리턴
                strChangedTeamIDList = ParseTeamList(strTeamList, dicTeamIDList, regularTeamIDs);

                // 변경될 발신처 정보
                if (nCommanderType == 4 && regularTeamIDs.Contains(nCommanderMemberID))
                {
                    nChangedCommanderType = -1;
                    nChangedCommanderMemberID = -1;
                    strChangedCommanderDisplayText = "SOP 제어권 가진곳의 책임자";
                }

                if (strChangedTeamIDList == null && (nCommanderType == 4 && regularTeamIDs.Contains(nCommanderMemberID)) == false)
                    continue;


                // 리벗용 업데이트문 생성
                string strReverseUpdateSQL = String.Format(strUpdateFormat,
                    strTeamList,
                    nCommanderType,
                    (nCommanderMemberID < 0 ? "NULL" : nCommanderMemberID.ToString()),
                    strCommanderDisplayText,
                    nID);
                updateList.Add(strReverseUpdateSQL);

                // 팀삭제로 인한 적용 쿼리 색성
                string strUpdateSQL = String.Format(strUpdateFormat,
                    (String.IsNullOrWhiteSpace(strChangedTeamIDList) ? strTeamList : strChangedTeamIDList),
                    nChangedCommanderType,
                    (nChangedCommanderMemberID < 0 ? "NULL" : nChangedCommanderMemberID.ToString()),
                    strChangedCommanderDisplayText,
                    nID);

                if (dbMgr.GetBatchData(strUpdateSQL) == null)
                    return null;
            }

            return updateList;
        }

        // RollBack을 위하여 수정한 데이터들의 Update 구문 리스트를 반환한다.
        private List<string> UpdateProcessDatas(WebDBManager dbMgr, string strTeamIDs)
        {
            // 2015-08-07 mwkim 변경사항
            // Process 의 수신처 이외에 발신처도 수정하도록 적용
            
            List<string> updateList = new List<string>();

            if (strTeamIDs.Length == 0)
                return updateList;

            List<int> regularTeamIDs = GetRegularTeamIDs(strTeamIDs);

            // Key : 팀 Type
            //       0 : 평일 비상 조직-TemporaryNormalTeam, 
            //       1 : 휴일 비상 조직-TemporaryEmergencyTeam, 
            //       2 : 외부 기관-ExternalTeam, 
            //       3 : 사용자 정의 조직-UserDefinedTeam, 
            //       4 : 상시조직-RegularTeam)
            // Value : 타입별 Team ID List
            Dictionary<int, List<int>> dicTeamIDList = new Dictionary<int, List<int>>();

            string strSQL = string.Empty;
            strSQL += "SELECT Process.ID, Process.TeamList, Process.CommanderMemberType, Process.CommanderMemberID, Process.CommanderDisplayText ";
            strSQL += "FROM Process, StepMember, ActionStep, Disaster, Version ";
            strSQL += "WHERE Process.StepMemberID = StepMember.ID ";
            strSQL += "AND StepMember.ActionStepID = ActionStep.ID ";
            strSQL += "AND ActionStep.DisasterID = Disaster.ID ";
            strSQL += "AND Disaster.VersionID = Version.ID ";
            strSQL += "AND Version.SiteID = " + FormMain.Instance.SiteID.ToString();

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            string strUpdateFormat = "UPDATE Process SET TeamList = '{0}', CommanderMemberType = {1}, CommanderMemberID = {2}, CommanderDisplayText = '{3}' WHERE ID = {4}";

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamList = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nCommanderType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nCommanderMemberID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strCommanderDisplayText = WebDBManager.GetStringField(arrResult[i + 4], "");

                if (nID < 0 || strTeamList.Length == 0)
                    continue;

                int nChangedCommanderType = nCommanderType;
                int nChangedCommanderMemberID = nCommanderMemberID;
                string strChangedCommanderDisplayText = strCommanderDisplayText;
                string strChangedTeamIDList = string.Empty;

                // 변경될 수신자 목록 리턴
                strChangedTeamIDList = ParseTeamList(strTeamList, dicTeamIDList, regularTeamIDs);

                // 변경될 발신처 정보
                if (nCommanderType == 4 && regularTeamIDs.Contains(nCommanderMemberID))
                {
                    nChangedCommanderType = -1;
                    nChangedCommanderMemberID = -1;
                    strChangedCommanderDisplayText = "SOP 제어권 가진곳의 책임자";
                }

                if (strChangedTeamIDList == null && (nCommanderType == 4 && regularTeamIDs.Contains(nCommanderMemberID)) == false)
                    continue;


                // 리벗용 업데이트문 생성
                string strReverseUpdateSQL = String.Format(strUpdateFormat, 
                    strTeamList,
                    nCommanderType,
                    (nCommanderMemberID < 0 ? "NULL" : nCommanderMemberID.ToString()),
                    strCommanderDisplayText,
                    nID);
                updateList.Add(strReverseUpdateSQL);

                // 팀삭제로 인한 적용 쿼리 색성
                string strUpdateSQL = String.Format(strUpdateFormat,
                    (String.IsNullOrWhiteSpace(strChangedTeamIDList) ? strTeamList : strChangedTeamIDList),
                    nChangedCommanderType,
                    (nChangedCommanderMemberID < 0 ? "NULL" : nChangedCommanderMemberID.ToString()),
                    strChangedCommanderDisplayText,
                    nID);

                if (dbMgr.GetBatchData(strUpdateSQL) == null)
                    return null;
            }

            return updateList;
        }

        private string ParseTeamList(string strTeamList, Dictionary<int, List<int>> dicTeamIDList, List<int> regularTeamIDs)
        {
            int nTeamID, nTeamType;
            List<int> teamIDs;
            string strChangedTeamList = "";
            bool isChanged = false;

            string[] arrTokens = strTeamList.Split(',');

            foreach (string strToken in arrTokens)
            {
                int nIndex1 = strToken.IndexOf('(');
                int nIndex2 = strToken.LastIndexOf(')');

                if (nIndex1 < 0 || nIndex2 < nIndex1)
                    continue;

                string strTeamID = strToken.Substring(0, nIndex1).Trim();
                string strTeamType = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();

                if (!int.TryParse(strTeamID, out nTeamID) || !int.TryParse(strTeamType, out nTeamType))
                    continue;

                if (!dicTeamIDList.TryGetValue(nTeamType, out teamIDs))
                {
                    teamIDs = new List<int>();
                    dicTeamIDList[nTeamType] = teamIDs;
                }

                teamIDs.Add(nTeamID);

                if (nTeamType == 4 && regularTeamIDs.Contains(nTeamID))
                    isChanged = true;
                else
                {
                    if (strChangedTeamList.Length == 0)
                        strChangedTeamList = strTeamID + "(" + strTeamType + ")";
                    else
                        strChangedTeamList += ", " + strTeamID + "(" + strTeamType + ")";
                }
            }

            if (!isChanged)
                return null;

            return strChangedTeamList;
        }

        private List<int> GetRegularTeamIDs(string strTeamIDs)
        {
            List<int> teamIDs = new List<int>();

            strTeamIDs = strTeamIDs.Trim();

            if (strTeamIDs.Length == 0)
                return teamIDs;

            strTeamIDs = strTeamIDs.Substring(1, strTeamIDs.Length - 2);
            string[] arrTokens = strTeamIDs.Trim().Split(',');

            int nTeamID;

            foreach (string strID in arrTokens)
            {
                if (int.TryParse(strID.Trim(), out nTeamID))
                    teamIDs.Add(nTeamID);
            }

            return teamIDs;
        }

        // RollBack을 위하여 수정한 데이터들의 Update 구문 리스트를 반환한다.
        public static List<string> UpdateSOPGenUsers(WebDBManager dbMgr, string strCompanyMemberIDs)
        {
            List<string> updateList = new List<string>();

            if (strCompanyMemberIDs.Length == 0)
                return updateList;

            string strSQL = "SELECT ID, MemberID from SOPGenUser where MemberID in " + strCompanyMemberIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            string strUpdateFormat = "Update SOPGenUser set MemberID = {0} where ID = {1}";

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                
                if (nID < 0 || nMemberID < 0)
                    continue;

                string strUpdate = string.Format(strUpdateFormat, nMemberID, nID);
                updateList.Add(strUpdate);
            }

            string strUpdateSQL = "Update SOPGenUser set MemberID = NULL where MemberID in " + strCompanyMemberIDs;
            if (dbMgr.GetBatchData(strUpdateSQL) == null)
                return null;

            strSQL = "Select SOPGenUserID, DayLight, MemberType, MemberID, DisplayText, CallerPhoneNumber from SOPGenUserCommander where MemberID in " + strCompanyMemberIDs;
            arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            nResultCount = arrResult.Count;
            string strInsertFormat = "Insert into SOPGenUserCommander (SOPGenUserID, DayLight, MemberType, MemberID, DisplayText, CallerPhoneNumber) values ({0}, {1}, {2}, {3}, {4}, {5})";

            for (int i=0;i<nResultCount-5;i+=6)
            {
                int nSOPGenUserID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nDayLight = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 4]);
                string strCallerPhoneNumber = WebDBManager.GetStringField(arrResult[i + 5]);

                if (strDisplayText == null)
                    strDisplayText = "NULL";
                else
                    strDisplayText = "'" + strDisplayText + "'";

                if (strCallerPhoneNumber == null)
                    strCallerPhoneNumber = "NULL";
                else
                    strCallerPhoneNumber = "'" + strCallerPhoneNumber + "'";

                string strInsert = string.Format(strInsertFormat, nSOPGenUserID, nDayLight, nMemberType, nMemberID, strDisplayText, strCallerPhoneNumber);
                updateList.Add(strInsert);
            }

            string strDeleteSQL = "Delete from SOPGenUserCommander where MemberID in " + strCompanyMemberIDs;

            if (dbMgr.GetBatchData(strDeleteSQL) == null)
                return null;

            return updateList;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        /*private List<string> RemoveDuty(WebDBManager dbMgr, string strTeamIDs)
        {
            List<string> insertList = new List<string>();

            if (strTeamIDs.Length == 0)
                return insertList;

            string strSQL = "SELECT ID, MemberID, InsertTime, TeamID, Description, SiteID from ";
            strSQL += "Duty where TeamID in " + strTeamIDs;

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into Duty (ID, MemberID, InsertTime, TeamID, Description, SiteID) ";
            strInsertFormat += "values ({0}, {1}, '{2}', {3}, {4}, {5})";

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strInsertTime = WebDBManager.GetStringField(arrResult[i + 2], null);
                int nTeamID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 4].ToString(), null);
                int nSiteID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                if (nID < 0 || nMemberID < 0 || strInsertTime == null || strInsertTime == "null" || nTeamID < 0 || nSiteID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, nMemberID, strInsertTime, nTeamID,
                    strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                    nSiteID);

                insertList.Add(strInsert);
            }

            string strDeleteSQL = "Delete from Duty where TeamID in " + strTeamIDs;
            if (dbMgr.GetResultData(strDeleteSQL, 1) == null)
                return null;

            return insertList;
        }*/

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveTemporaryMembers(WebDBManager dbMgr, string strTeamIDs, string strPlusMinusTeamIDs, string strCompanyMemberIDs)
        {
            List<string> insertList = new List<string>();

            if (!RemoveTemporaryMemberList(dbMgr, strPlusMinusTeamIDs, strCompanyMemberIDs, insertList))
                return null;

            // TemporaryNormalTeam
            /*if (!RemoveTemporaryTeam(dbMgr, strTeamIDs, true, insertList))
                return null;

            // TemporaryEmergencyTeam
            if (!RemoveTemporaryTeam(dbMgr, strTeamIDs, false, insertList))
                return null;*/

            return insertList;
        }

        /*private bool RemoveTemporaryTeam(WebDBManager dbMgr, string strTeamIDs, bool isNormal, List<string> insertList)
        {
            if (strTeamIDs.Length == 0)
                return true;

            string strTableName = isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";

            string strSQL = "SELECT ID, TeamName, ParentTeamID, GroupName, Description, SiteID from " + strTableName;
            strSQL += " where ID in " + strTeamIDs;

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 1);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into " + strTableName + " (ID, TeamName, ParentTeamID, GroupName, Description, SiteID) ";
            strInsertFormat += "values ({0}, '{1}', {2}, {3}, {4}, {5})";

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strGroupName = WebDBManager.GetStringField(arrResult[i + 3].ToString(), null);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 4].ToString(), null);
                int nSiteID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);

                if (nID < 0 || strTeamName.Length == 0 || nSiteID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, strTeamName,
                    nParentTeamID < 0 ? "NULL" : nParentTeamID.ToString(),
                    strGroupName == null ? "NULL" : "'" + strGroupName + "'",
                    strDesc == null ? "NULL" : "'" + strDesc + "'",
                    nSiteID);

                insertList.Add(strInsert);
            }

            string strDeleteSQL = "Delete from " + strTableName + " where ID in " + strTeamIDs;
            return dbMgr.GetResultData(strDeleteSQL, 1) != null;
        }*/

        private bool RemoveTemporaryMemberList(WebDBManager dbMgr, string strPlusMinusTeamIDs, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role ";
            strSQL += "from TemporaryMemberList where ";

            if (strCompanyMemberIDs.Length > 0 && strPlusMinusTeamIDs.Length > 0)
            {
                strSQL += "(MemberType = 0 and MemberID in " + strPlusMinusTeamIDs + ") or (MemberType = 1 and MemberID in " + strCompanyMemberIDs + ")";
                strDeleteSQL = "Delete from TemporaryMemberList where (MemberType = 0 and MemberID in " + strPlusMinusTeamIDs + ") or (MemberType = 1 and MemberID in " + strCompanyMemberIDs + ")";
            }
            else if (strPlusMinusTeamIDs.Length > 0)
            {
                strSQL += "MemberType = 0 and MemberID in " + strPlusMinusTeamIDs;
                strDeleteSQL = "Delete from TemporaryMemberList where MemberType = 0 and MemberID in " + strPlusMinusTeamIDs;
            }
            else if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 1 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from TemporaryMemberList where MemberType = 1 and MemberID in " + strCompanyMemberIDs;
            }
            else
                return true;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) ";
            strInsertFormat += "values ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strMemberName = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nTemporaryTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                bool isNormal = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nTeamLeader = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nMemberCount = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                int nRole = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                if (nID < 0 || nTemporaryTeamID < 0 || nMemberType < 0 || nRole < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat,
                    nID,
                    strMemberName,
                    nTemporaryTeamID,
                    isNormal ? 1 : 0,
                    //nMemberID < 0 ? "NULL" : nMemberID.ToString(),  
                    nMemberID.ToString(),
                    nTeamLeader < 0 ? "NULL" : nTeamLeader.ToString(),
                    nMemberType,
                    nMemberCount < 0 ? "NULL" : nMemberCount.ToString(),
                    nRole);

                insertList.Add(strInsert);
            }

            return dbMgr.GetBatchData(strDeleteSQL) != null;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveFacilityManagers(WebDBManager dbMgr, string strTeamIDs, string strCompanyMemberIDs)
        {
            List<string> insertList = new List<string>();

            if (!RemoveFacilityManagers(dbMgr, strTeamIDs, strCompanyMemberIDs, insertList))
                return null;

            if (!RemoveBuildingFacilityManagers(dbMgr, strTeamIDs, strCompanyMemberIDs, insertList))
                return null;

            if (!RemoveEquipZoneFacilityManagers(dbMgr, strTeamIDs, strCompanyMemberIDs, insertList))
                return null;

            return insertList;
        }

        private bool RemoveFacilityManagers(WebDBManager dbMgr, string strTeamIDs, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit, SiteID ";
            strSQL += "from FacilityManager where ";

            if (strCompanyMemberIDs.Length > 0 && strTeamIDs.Length > 0)
            {
                strSQL += "(MemberType = 0 and MemberID in " + strCompanyMemberIDs + ") or (MemberType = 1 and MemberID in " + strTeamIDs + ")";
                strDeleteSQL = "Delete from FacilityManager where (MemberType = 0 and MemberID in " + strCompanyMemberIDs + ") or (MemberType = 1 and MemberID in " + strTeamIDs + ")";
            }
            else if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 0 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from FacilityManager where MemberType = 0 and MemberID in " + strCompanyMemberIDs;
            }
            else if (strTeamIDs.Length > 0)
            {
                strSQL += "MemberType = 1 and MemberID in " + strTeamIDs;
                strDeleteSQL = "Delete from FacilityManager where MemberType = 1 and MemberID in " + strTeamIDs;
            }
            else
                return true;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into FacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit, SiteID) ";
            strInsertFormat += "values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})";

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 5], null);
                int nUpperLimit = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nSiteID = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);

                if (nID < 0 || nMemberID < 0 || nMemberType < 0 || nFacilityType < 0 || nSiteID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, nMemberID, nMemberType, nFacilityType,
                    nLevelLimit < 0 ? "NULL" : nLevelLimit.ToString(),
                    strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                    nUpperLimit, nSiteID);

                insertList.Add(strInsert);
            }

            return dbMgr.GetBatchData(strDeleteSQL) != null;
        }

        private bool RemoveEquipZoneFacilityManagers(WebDBManager dbMgr, string strTeamIDs, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberID, MemberType, SiteID, FacilityType, LevelLimit, EquipZoneID, Description, UpperLimit ";
            strSQL += "from EquipZoneFacilityManager where ";

            if (strCompanyMemberIDs.Length > 0 && strTeamIDs.Length > 0)
            {
                strSQL += "(MemberType = 0 and MemberID in " + strCompanyMemberIDs + ") or (MemberType = 1 and MemberID in " + strTeamIDs + ")";
                strDeleteSQL = "Delete from EquipZoneFacilityManager where (MemberType = 0 and MemberID in " + strCompanyMemberIDs + ") or (MemberType = 1 and MemberID in " + strTeamIDs + ")";
            }
            else if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 0 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from EquipZoneFacilityManager where MemberType = 0 and MemberID in " + strCompanyMemberIDs;
            }
            else if (strTeamIDs.Length > 0)
            {
                strSQL += "MemberType = 1 and MemberID in " + strTeamIDs;
                strDeleteSQL = "Delete from EquipZoneFacilityManager where MemberType = 1 and MemberID in " + strTeamIDs;
            }
            else
                return true;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into EquipZoneFacilityManager (ID, MemberID, MemberType, SiteID, FacilityType, LevelLimit, EquipZoneID, Description, UpperLimit) ";
            strInsertFormat += "values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nSiteID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 7], null);
                int nUpperLimit = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                if (nID < 0 || nMemberID < 0 || nMemberType < 0 || nFacilityType < 0 || nEquipZoneID < 0 || nSiteID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, nMemberID, nMemberType, nSiteID, nFacilityType,
                    nLevelLimit < 0 ? "NULL" : nLevelLimit.ToString(),
                    nEquipZoneID,
                    strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                    nUpperLimit);

                insertList.Add(strInsert);
            }

            return dbMgr.GetBatchData(strDeleteSQL) != null;
        }

        private bool RemoveBuildingFacilityManagers(WebDBManager dbMgr, string strTeamIDs, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit, SiteID ";
            strSQL += "from BuildingFacilityManager where ";

            if (strCompanyMemberIDs.Length > 0 && strTeamIDs.Length > 0)
            {
                strSQL += "(MemberType = 0 and MemberID in " + strCompanyMemberIDs + ") or (MemberType = 1 and MemberID in " + strTeamIDs + ")";
                strDeleteSQL = "Delete from BuildingFacilityManager where (MemberType = 0 and MemberID in " + strCompanyMemberIDs + ") or (MemberType = 1 and MemberID in " + strTeamIDs + ")";
            }
            else if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 0 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from BuildingFacilityManager where MemberType = 0 and MemberID in " + strCompanyMemberIDs;
            }
            else if (strTeamIDs.Length > 0)
            {
                strSQL += "MemberType = 1 and MemberID in " + strTeamIDs;
                strDeleteSQL = "Delete from BuildingFacilityManager where MemberType = 1 and MemberID in " + strTeamIDs;
            }
            else
                return true;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "Insert into BuildingFacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit, SiteID) ";
            strInsertFormat += "values ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFacilityType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nLevelLimit = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                string strDesc = WebDBManager.GetStringField(arrResult[i + 6], null);
                int nUpperLimit = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                int nSiteID = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                if (nID < 0 || nMemberID < 0 || nMemberType < 0 || nFacilityType < 0 || nBuildingID < 0 || nSiteID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, nMemberID, nMemberType, nFacilityType,
                    nLevelLimit < 0 ? "NULL" : nLevelLimit.ToString(),
                    nBuildingID,
                    strDesc == null || strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                    nUpperLimit, nSiteID);

                insertList.Add(strInsert);
            }

            return dbMgr.GetBatchData(strDeleteSQL) != null;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveRegularMemberList(WebDBManager dbMgr, string strTeamIDs, out string strCompanyMemberIDs)
        {
            strCompanyMemberIDs = "";

            string strSQL = "Select RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID from RegularMemberList where RegularTeamID in " + strTeamIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            List<string> insertList = new List<string>();

            string strInsertFormat = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values ({0}, {1}, {2}, {3}, {4})";
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                int nRegularTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nCompanyMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nPositionID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nSubPositionID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nGroupPositionID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);

                if (nRegularTeamID < 0 || nCompanyMemberID < 0 || nPositionID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nRegularTeamID, nCompanyMemberID, nPositionID,
                    nSubPositionID < 0 ? "NULL" : nSubPositionID.ToString(),
                    nGroupPositionID < 0 ? "NULL" : nGroupPositionID.ToString());

                insertList.Add(strInsert);

                if (strCompanyMemberIDs.Length == 0)
                    strCompanyMemberIDs = nCompanyMemberID.ToString();
                else
                    strCompanyMemberIDs += ", " + nCompanyMemberID.ToString();
            }

            if (strCompanyMemberIDs.Length > 0)
                strCompanyMemberIDs = "(" + strCompanyMemberIDs + ")";

            strSQL = "Delete from RegularMemberList where RegularTeamID in " + strTeamIDs;

            if (dbMgr.GetBatchData(strSQL) == null)
                return null;

            return insertList;
        }

        private void AddDB(WebDBManager dbMgr)
        {
            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

            foreach (string strSQL in m_rollbackSQLs)
            {
                ArrayList arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return;
                }
            } 

            // Batch Job end - Commit
            dbMgr.BatchCommit();

            foreach (KeyValuePair<RegularTeam, List<CompanyMember>> item in m_removeTeamCompanyMemberList)
            {
                DataManager.SetRegularMembers(item.Key, item.Value);

                foreach (CompanyMember member in item.Value)
                {
                    DataManager.AddCompanyMember(member);
                }
            }

            m_rollbackSQLs.Clear();
        }

        /*private void AddDB(WebDBManager dbMgr)
        {
            if (m_team == null || m_node == null)
                return;

            try
            {
                if (IsExist(dbMgr))
                    return;
            }
            catch (Exception)
            {
                return;
            }

            AddTeam(dbMgr);
        }

        private void AddTeam(WebDBManager dbMgr)
        {
            string strSQL = string.Format("Insert into RegularTeam (ID, TeamName, ParentTeamID) values ({0}, '{1}', {2})",
                m_team.TeamID, m_team.TeamName, m_team.ParentTeam == null ? "NULL" : m_team.ParentTeam.TeamID.ToString());

            dbMgr.GetResultData(strSQL);
        }

        private bool IsExist(WebDBManager dbMgr)
        {
            string strSQL = "Select ID from RegularTeam where ID = " + m_team.TeamID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                throw new Exception();

            if (arrResult.Count == 0)
                return false;

            int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nID == m_team.TeamID;
        }*/
    }
}
