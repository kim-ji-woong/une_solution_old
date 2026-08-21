using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUtility2;

namespace TeamEditor.BLL.WinForms.Command
{
    public class CommandRemoveTemporaryTeam : CommandEx
    {
        private TreeView m_tree = null;
        private TreeNode m_parent = null;
        private TreeNode m_node = null;
        private Team m_team = null;
        private int m_nIndex = -1;
        private bool m_isNormal = true;
        private List<string> m_rollbackSQLs = new List<string>();

        // 팀 최상위 부모로부터 몇단계 떨어진 자식인지 여부에 따라 정렬되어 있다.
        private List<Team> m_removeTeamList = new List<Team>();

        public TreeView Tree
        {
            get { return m_tree; }
            set { m_tree = value; }
        }

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
                    {
                        if (m_tree == null)
                            m_nIndex = -1;
                        else
                            m_nIndex = m_tree.Nodes.IndexOf(m_node);
                    }
                    else
                        m_nIndex = m_node.Parent.Nodes.IndexOf(m_node);

                    m_parent = m_node.Parent;
                }
            }
        }

        public Team Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }

        public CommandRemoveTemporaryTeam(TreeView tree, TreeNode node, Team team, bool isNormal)
        {
            m_tree = tree;
            Team = team;
            TreeNode = node;
            m_isNormal = isNormal;
        }

        public CommandRemoveTemporaryTeam(TreeView tree, TreeNode node, TreeNode nodeParent, int nNodeIndex, Team team, bool isNormal)
        {
            m_tree = tree;
            m_node = node;
            m_parent = nodeParent;
            m_nIndex = nNodeIndex;
            m_team = team;
            m_isNormal = isNormal;
        }

        public override void Do()
        {
            if (m_node != null)
            {
                if (m_parent != null)
                    m_parent.Nodes.Remove(m_node);
                else
                    m_tree.Nodes.Remove(m_node);
            }
        }

        public override void RollBack()
        {
            if (m_node != null)
            {
                TreeNodeCollection nodes = m_parent == null ? m_tree.Nodes : m_parent.Nodes;

                if (m_nIndex >= 0)
                    nodes.Insert(m_nIndex, m_node);
                else
                    nodes.Add(m_node);
            }
        }

        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            if (m_node == null || m_tree == null)
                return;

            if (dir)
            {
                if (m_rollbackSQLs.Count == 0)
                    RemoveDB(dbMgr);
            }
            else
            {
                if(m_parent == null)
                {
                    AddDB(dbMgr);
                }
                else if ( m_parent.Nodes.Contains(m_node))
                {
                    // 삭제했다가 RollBack한 상태인 경우 DB에서는 지워졌지만 UI에는 다시 나타나있다.
                    AddDB(dbMgr);
                }
            }

            if (m_isNormal)
                UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM);
            else
                UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM);
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

            m_rollbackSQLs.Clear();
        }

        // 팀들을 최상위 부모로부터 떨어진 거리에 따라 정렬하여 리턴한다.
        private string GetTeamList(List<int> teamIDs)
        {
            m_removeTeamList = DataManager.GetTemporaryTeams(m_team.TeamID, m_isNormal);

            string strTeamIDs = "";

            foreach (Team team in m_removeTeamList)
            {
                if (strTeamIDs.Length == 0)
                    strTeamIDs = team.TeamID.ToString();
                else
                    strTeamIDs += ", " + team.TeamID.ToString();

                teamIDs.Add(team.TeamID);
            }

            if (strTeamIDs.Length > 0)
                strTeamIDs = "(" + strTeamIDs + ")";

            return strTeamIDs;
        }

        // 비상조직의 MemberID 검색을 위하여 TeamID가 양수인것과 음수인것들 모두의 리스트를 만든다.
        private string GetPlusMinusTeamIDs()
        {
            string strTeamIDs = "";

            foreach (Team team in m_removeTeamList)
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
            List<int> teamIDs = new List<int>();
            string strTeamIDs = GetTeamList(teamIDs);

            if (strTeamIDs.Length == 0)
                return;

            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

            Dictionary<TemporaryMember.MemberType, string> dicTemporaryMemberIDs = new Dictionary<TemporaryMember.MemberType, string>();

            // TeamID에 '-'를 붙인 리스트를 더한다.
            string strPlusMinusTeamIDs = GetPlusMinusTeamIDs();
            List<string> insertTemporaryMemberList = RemoveTemporaryMemberList(dbMgr, strPlusMinusTeamIDs, dicTemporaryMemberIDs);

            if (insertTemporaryMemberList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            List<string> updateProcessDataList = UpdateProcessDatas(dbMgr, teamIDs);

            if (updateProcessDataList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            List<string> updateInternalTransmissionDataList = UpdateInternalTransmissionDatas(dbMgr, teamIDs);

            if (updateInternalTransmissionDataList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            List<string> insertTemporaryTeamList = RemoveTemporaryTeams(dbMgr, strTeamIDs);

            if (insertTemporaryTeamList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return;
            }

            // Batch Job end - Commit
            dbMgr.BatchCommit();

            m_rollbackSQLs.AddRange(insertTemporaryTeamList);
            m_rollbackSQLs.AddRange(updateInternalTransmissionDataList);
            m_rollbackSQLs.AddRange(updateProcessDataList);
            m_rollbackSQLs.AddRange(insertTemporaryMemberList);
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveTemporaryTeams(WebDBManager dbMgr, string strTeamIDs)
        {
            List<string> insertList = new List<string>();

            if (strTeamIDs.Length == 0)
                return insertList;

            string strTableName = m_isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";

            string strInsertFormat = "Insert into {0} (ID, TeamName, ParentTeamID, GroupName, Description, SiteID) ";
            strInsertFormat += "values ({1}, '{2}', {3}, NULL, NULL, {4})";

            int nRemoveCount = m_removeTeamList.Count;

            string strParentTeamID = "";

            for (int i = 0; i < nRemoveCount; i++)
            {
                Team team = m_removeTeamList[i];

                if (m_isNormal)
                {
                    TemporaryNormalTeam normalTeam = (TemporaryNormalTeam)team;
                    strParentTeamID = normalTeam.ParentTeam == null ? "NULL" : normalTeam.ParentTeam.TeamID.ToString();
                }
                else
                {
                    TemporaryEmergencyTeam emergencyTeam = (TemporaryEmergencyTeam)team;
                    strParentTeamID = emergencyTeam.ParentTeam == null ? "NULL" : emergencyTeam.ParentTeam.TeamID.ToString();
                }

                string strInsert = string.Format(strInsertFormat, strTableName, team.TeamID, team.TeamName, strParentTeamID, FormMain.Instance.SiteID);

                insertList.Add(strInsert);
            }

            string strDeleteSQL = "Delete from " + strTableName + " where ID in " + strTeamIDs;
            if (dbMgr.GetBatchData(strDeleteSQL) == null)
                return null;

            return insertList;
        }

        // RollBack을 위하여 수정한 데이터들의 Update 구문 리스트를 반환한다.
        private List<string> UpdateInternalTransmissionDatas(WebDBManager dbMgr, List<int> temporaryTeamIDs)
        {
            // 2015-08-07 mwkim 변경사항
            // InternalTransmission 의 발신자, 수신자에서 삭제되도록 적용

            List<string> updateList = new List<string>();

            if (temporaryTeamIDs.Count == 0)
                return updateList;

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
                strChangedTeamIDList = ParseTeamList(strTeamList, dicTeamIDList, temporaryTeamIDs);

                // 변경될 발신처 정보
                if (nCommanderType == (m_isNormal ? 0 : 1) && temporaryTeamIDs.Contains(nCommanderMemberID))
                {
                    nChangedCommanderType = -1;
                    nChangedCommanderMemberID = -1;
                    strChangedCommanderDisplayText = "SOP 제어권 가진곳의 책임자";
                }

                if (strChangedTeamIDList == null && (nCommanderType == (m_isNormal ? 0 : 1) && temporaryTeamIDs.Contains(nCommanderMemberID)) == false)
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
        private List<string> UpdateProcessDatas(WebDBManager dbMgr, List<int> temporaryTeamIDs)
        {
            // 2015-08-07 mwkim 변경사항
            // Process 의 수신처 이외에 발신처도 수정하도록 적용

            List<string> updateList = new List<string>();

            if (temporaryTeamIDs.Count == 0)
                return updateList;

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
                strChangedTeamIDList = ParseTeamList(strTeamList, dicTeamIDList, temporaryTeamIDs);

                // 변경될 발신처 정보
                if (nCommanderType == (m_isNormal ? 0 : 1) && temporaryTeamIDs.Contains(nCommanderMemberID))
                {
                    nChangedCommanderType = -1;
                    nChangedCommanderMemberID = -1;
                    strChangedCommanderDisplayText = "SOP 제어권 가진곳의 책임자";
                }

                if (strChangedTeamIDList == null && (nCommanderType == (m_isNormal ? 0 : 1) && temporaryTeamIDs.Contains(nCommanderMemberID)) == false)
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

        private string ParseTeamList(string strTeamList, Dictionary<int, List<int>> dicTeamIDList, List<int> temporaryTeamIDs)
        {
            int nTeamID, nTeamType;
            List<int> teamIDs;
            string strChangedTeamList = "";
            bool isChanged = false;

            int nTargetTeamType = m_isNormal ? 0 : 1;

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

                if (nTeamType == nTargetTeamType && temporaryTeamIDs.Contains(nTeamID))
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

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveTemporaryMemberList(WebDBManager dbMgr, string strTeamIDs, Dictionary<TemporaryMember.MemberType, string> dicMemberIDs)
        {
            string strNormal = m_isNormal ? "1" : "0";
            string strSQL = "Select ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role ";
            strSQL += "from TemporaryMemberList where IsNormal = " + strNormal + " and TemporaryTeamID in " + strTeamIDs;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            List<string> insertList = new List<string>();

            string strInsertFormat = "Insert into TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) ";
            strInsertFormat += "values ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})";

            int nResultCount = arrResult.Count;

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

                if (nID < 0 || nTemporaryTeamID < 0)
                    continue;

                TemporaryMember.MemberType memberType;
                TemporaryMember.ManagerType managerType;

                if (!TemporaryMember.ToMemberType(nMemberType, out memberType))
                    continue;

                if (!TemporaryMember.ToManagerType(nRole, out managerType))
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

                string strTemporaryMemberIDs;

                if (!dicMemberIDs.TryGetValue(memberType, out strTemporaryMemberIDs))
                    strTemporaryMemberIDs = "";

                if (strTemporaryMemberIDs.Length == 0)
                    strTemporaryMemberIDs = nMemberID.ToString();
                else
                    strTemporaryMemberIDs += ", " + nMemberID.ToString();

                dicMemberIDs[memberType] = strTemporaryMemberIDs;
            }

            strSQL = "Delete from TemporaryMemberList where TemporaryTeamID in " + strTeamIDs + " AND  IsNormal = " + strNormal;

            if (dbMgr.GetBatchData(strSQL) == null)
                return null;

            return insertList;
        }
    }
}
