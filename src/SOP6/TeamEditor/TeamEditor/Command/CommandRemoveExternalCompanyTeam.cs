using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor.Command
{
    public class CommandRemoveExternalTeam : CommandEx
    {
        // 부모노드의 유무 관계없이 무조건 DML 실행
        private bool m_isNotCheck = false;

        private TreeView m_tree = null;
        private TreeNode m_node = null;
        private TreeNode m_nodeParent = null;
        private Team m_team = null;
        private int m_nIndex = -1;
        private List<string> m_rollbackSQLs = new List<string>();

        // 팀 최상위 부모로부터 몇단계 떨어진 자식인지 여부에 따라 정렬되어 있다.
        private List<Team> m_removeTeamList = new List<Team>();

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

                    m_nodeParent = m_node.Parent;
                }
            }
        }

        public Team Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public CommandRemoveExternalTeam(TreeNode node, Team team)
        {
            m_tree = node.TreeView;
            m_node = node;
            m_team = team;

            if (m_node == null)
                m_nIndex = -1;
            else
            {
                if (m_node.Parent == null)
                    m_nIndex = -1;
                else
                    m_nIndex = m_node.Parent.Nodes.IndexOf(m_node);

                m_nodeParent = m_node.Parent;
            }
        }

        public CommandRemoveExternalTeam(TreeNode node, Team team, bool isNotCheck)
            : this(node, team)
        {
            m_isNotCheck = isNotCheck;
        }

        public override void Do()
        {
            if (m_node != null)
            {
                TreeNodeCollection nodes = GetParentNodes();

                if (nodes != null)
                    nodes.Remove(m_node);
            }
        }

        private TreeNodeCollection GetParentNodes()
        {
            if (m_nodeParent != null)
                return m_nodeParent.Nodes;
            else if (m_tree != null)
                return m_tree.Nodes;

            return null;
        }

        public override void RollBack()
        {
            if (m_node != null)
            {
                TreeNodeCollection nodes = GetParentNodes();

                if (nodes != null)
                {
                    if (m_nIndex >= 0)
                        nodes.Insert(m_nIndex, m_node);
                    else
                        nodes.Add(m_node);
                }
            }
        }

        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            if (m_node != null)
            {
                TreeNodeCollection nodes = GetParentNodes();

                if (nodes != null || m_isNotCheck)
                {
                    if (dir)
                    {
                        if (m_rollbackSQLs.Count == 0)
                            RemoveDB(dbMgr);
                    }
                    else
                    {
                        if (nodes.Contains(m_node))
                        {
                            // 삭제했다가 RollBack한 상태인 경우 DB에서는 지워졌지만 UI에는 다시 나타나있다.
                            AddDB(dbMgr);
                        }
                    }
                }

                UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM);
            }
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

        private void RemoveDB(WebDBManager dbMgr)
        {
            string strExternalTeamIDs;
            string strExternalCompanyMemberIDs;

            List<string> insertExternalMemberList = new List<string>();
            List<string> insertTemporaryMemberList = new List<string>();
            List<string> updateProcessDataList = new List<string>();
            List<string> updateInternalTransmissionDataList = new List<string>();
            List<string> updateExternalTransmissionDataList = new List<string>();
            List<string> insertExternalCompanyMemberList = new List<string>();
            List<string> insertExternalTeamList = new List<string>();

            GetTeamList(out strExternalTeamIDs);

            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

            if (strExternalTeamIDs.Length != 0)
            {
                insertExternalMemberList = RemoveExternalMemberList(dbMgr, strExternalTeamIDs, out strExternalCompanyMemberIDs);

                if (insertExternalMemberList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return;
                }

                // TeamID에 '-'를 붙인 리스트를 더한다.
                string strPlusMinusTeamIDs = GetPlusMinusTeamIDs();
                insertTemporaryMemberList = RemoveTemporaryMemberList(dbMgr, strPlusMinusTeamIDs, strExternalCompanyMemberIDs);

                if (insertTemporaryMemberList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return;
                }

                updateProcessDataList = UpdateProcessDatas(dbMgr, strExternalTeamIDs);

                if (updateProcessDataList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return;
                }

                updateInternalTransmissionDataList = UpdateInternalTransmissionDatas(dbMgr, strExternalTeamIDs);

                if (updateInternalTransmissionDataList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return;
                }

                updateExternalTransmissionDataList = UpdateExternalTransmissionDatas(dbMgr, strExternalTeamIDs);

                if (updateInternalTransmissionDataList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return;
                }

                insertExternalCompanyMemberList = RemoveExternalCompanyMembers(dbMgr, strExternalCompanyMemberIDs);

                if (insertExternalCompanyMemberList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return;
                }

                insertExternalTeamList = RemoveExternalTeams(dbMgr, strExternalTeamIDs);

                if (insertExternalTeamList == null)
                {
                    // Rollback
                    dbMgr.BatchRollback();
                    return;
                }

            }

            // Batch Job end - Commit
            dbMgr.BatchCommit();

            m_rollbackSQLs.AddRange(insertExternalTeamList);
            m_rollbackSQLs.AddRange(insertExternalCompanyMemberList);
            m_rollbackSQLs.AddRange(updateExternalTransmissionDataList);
            m_rollbackSQLs.AddRange(updateInternalTransmissionDataList);
            m_rollbackSQLs.AddRange(updateProcessDataList);
            m_rollbackSQLs.AddRange(insertTemporaryMemberList);
            m_rollbackSQLs.AddRange(insertExternalMemberList);
        }

        // 팀들을 최상위 부모로부터 떨어진 거리에 따라 정렬하여 리턴한다.
        private void GetTeamList(out string strExternalTeamIDs)
        {
            strExternalTeamIDs = "";

            m_removeTeamList = DataManager.GetExternalTeams(m_team.TeamID);

            foreach (Team team in m_removeTeamList)
            {
                if (strExternalTeamIDs.Length == 0)
                    strExternalTeamIDs = team.TeamID.ToString();
                else
                    strExternalTeamIDs += ", " + team.TeamID.ToString();
            }

            if (strExternalTeamIDs.Length > 0)
                strExternalTeamIDs = "(" + strExternalTeamIDs + ")";

        }

        // 비상조직의 MemberID 검색을 위하여 TeamID가 양수인것과 음수인것들 모두의 리스트를 만든다.
        private string GetPlusMinusTeamIDs()
        {
            string strTeamIDs = "";

            foreach (ExternalTeam team in m_removeTeamList)
            {
                if (strTeamIDs.Length == 0)
                    strTeamIDs = team.TeamID.ToString() + "," + (-team.TeamID).ToString();
                else
                    strTeamIDs += team.TeamID.ToString() + "," + (-team.TeamID).ToString();
            }

            if (strTeamIDs.Length > 0)
                strTeamIDs = "(" + strTeamIDs + ")";

            return strTeamIDs;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveExternalMemberList(WebDBManager dbMgr, string strExternalTeamIDs, out string strExternalCompanyMemberIDs)
        {
            strExternalCompanyMemberIDs = "";

            string strSQL = "Select ExternalCompanyTeamID, ExternalCompanyMemberID, JobLevelID, JobPositionID from ExternalMemberList where ExternalCompanyTeamID in " + strExternalTeamIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            List<string> insertList = new List<string>();

            string strInsertFormat = "Insert into ExternalMemberList (ExternalCompanyTeamID, ExternalCompanyMemberID, JobLevelID, JobPositionID) values ({0}, {1}, {2}, {3})";
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nExternalCompanyTeamID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nExternalCompanyMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nJobLevelID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nJobPositionID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                if (nExternalCompanyTeamID < 0 || nExternalCompanyMemberID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat,
                    nExternalCompanyTeamID,
                    nExternalCompanyMemberID,
                    nJobLevelID < 0 ? "NULL" : nJobLevelID.ToString(),
                    nJobPositionID < 0 ? "NULL" : nJobPositionID.ToString());

                insertList.Add(strInsert);

                if (strExternalCompanyMemberIDs.Length == 0)
                    strExternalCompanyMemberIDs = nExternalCompanyMemberID.ToString();
                else
                    strExternalCompanyMemberIDs += ", " + nExternalCompanyMemberID.ToString();
            }

            if (strExternalCompanyMemberIDs.Length > 0)
                strExternalCompanyMemberIDs = "(" + strExternalCompanyMemberIDs + ")";

            strSQL = "Delete from ExternalMemberList where ExternalCompanyTeamID in " + strExternalTeamIDs;

            if (dbMgr.GetBatchData(strSQL) == null)
                return null;

            return insertList;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        public static List<string> RemoveExternalCompanyMembers(WebDBManager dbMgr, string strExternalCompanyMemberIDs)
        {
            List<string> insertList = new List<string>();

            if (strExternalCompanyMemberIDs.Length == 0)
                return insertList;

            string strInsertFormat = "Insert into ExternalCompanyMember (ID, Name, PhoneNumber, Description) ";
            strInsertFormat += "values ({0}, '{1}', {2}, {3})";

            string strSQL = "SELECT ID, Name, PhoneNumber, Description from ExternalCompanyMember where ID in " + strExternalCompanyMemberIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = WebDBManager.GetStringField(arrResult[i + 1], null);
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], null);
                string strDescription = WebDBManager.GetStringField(arrResult[i + 3], null);

                if (nID < 0 || strName == null)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, strName, 
                    strPhoneNumber == null || strPhoneNumber == "null" ? "NULL" : "'" + strPhoneNumber + "'",
                    strDescription == null || strDescription == "null" ? "NULL" : "'" + strDescription + "'");

                insertList.Add(strInsert);
            }

            //string strUpdateSQL = "Update ExternalCompanyMember Set ParentID = NULL Where ID In " + strExternalCompanyMemberIDs;
            //if (dbMgr.GetResultData(strUpdateSQL, 1) == null)
            //    return null;

            string strDeleteSQL = "Delete from ExternalCompanyMember where ID in " + strExternalCompanyMemberIDs;
            if (dbMgr.GetBatchData(strDeleteSQL) == null)
                return null;

            return insertList;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveExternalTeams(WebDBManager dbMgr, string strExternalTeamIDs)
        {
            List<string> insertList = new List<string>();

            if (strExternalTeamIDs.Length == 0)
                return insertList;

            string strInsertFormat = "Insert into ExternalTeam (ID, TeamName, PhoneNumber, FaxNumber, SiteID, ParentTeamID) ";
            strInsertFormat += "values ({0}, '{1}', {2}, {3}, {4}, {5})";

            int nRemoveCount = m_removeTeamList.Count;

            for (int i = 0; i < nRemoveCount; i++)
            {
                if (m_removeTeamList[i] is ExternalTeam)
                {
                    ExternalTeam team = (ExternalTeam)m_removeTeamList[i];

                    string strInsert = string.Format(strInsertFormat, team.TeamID, team.TeamName,
                        String.IsNullOrWhiteSpace(team.PhoneNumber) ? "NULL" : team.PhoneNumber.ToString(),
                        String.IsNullOrWhiteSpace(team.PhoneNumber) ? "NULL" : team.PhoneNumber.ToString(),
                        FormMain.Instance.SiteID,
                        (team.ParentTeam == null ? "NULL" : team.ParentTeam.TeamID.ToString())
                        );

                    insertList.Add(strInsert);
                }
            }

            string strDeleteSQL = "Delete from ExternalTeam where ID in " + strExternalTeamIDs;
            if (dbMgr.GetBatchData(strDeleteSQL) == null)
                return null;

            return insertList;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveTemporaryMemberList(WebDBManager dbMgr, string strPlusMinusTeamIDs, string strExternalMemberIDs)
        {
            List<string> insertList = new List<string>();

            string strSQL = string.Empty;
            string strDeleteSQL = string.Empty;

            strSQL += "SELECT ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role ";
            strSQL += "FROM TemporaryMemberList ";
            strSQL += "WHERE ";

            if (strExternalMemberIDs.Length > 0 && strPlusMinusTeamIDs.Length > 0)
            {
                strSQL += "(MemberType = 3 and MemberID in " + strPlusMinusTeamIDs + ") or (MemberType = 4 and MemberID in " + strExternalMemberIDs + ")";
                strDeleteSQL = "Delete from TemporaryMemberList where (MemberType = 3 and MemberID in " + strPlusMinusTeamIDs + ") or (MemberType = 4 and MemberID in " + strExternalMemberIDs + ")";
            }
            else if (strPlusMinusTeamIDs.Length > 0)
            {
                strSQL += "MemberType = 3 and MemberID in " + strPlusMinusTeamIDs;
                strDeleteSQL = "Delete from TemporaryMemberList where MemberType = 3 and MemberID in " + strPlusMinusTeamIDs;
            }
            else if (strExternalMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 4 and MemberID in " + strExternalMemberIDs;
                strDeleteSQL = "Delete from TemporaryMemberList where MemberType = 4 and MemberID in " + strExternalMemberIDs;
            }
            else
            {
                strSQL = string.Empty;
                strDeleteSQL = string.Empty;
            }

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "INSERT INTO TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) ";
            strInsertFormat += "VALUES ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})";

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

            if (dbMgr.GetBatchData(strDeleteSQL) == null)
                return null;

            return insertList;
        }

        // RollBack을 위하여 수정한 데이터들의 Update 구문 리스트를 반환한다.
        private List<string> UpdateExternalTransmissionDatas(WebDBManager dbMgr, string strTeamIDs)
        {
            // 2015-08-07 mwkim 변경사항
            // ExternalTransmission 의 발신자, 수신자에서 삭제되도록 적용

            List<string> updateList = new List<string>();

            if (strTeamIDs.Length == 0)
                return updateList;

            List<int> externalTeamIDs = GetExternalTeamIDs(strTeamIDs);

            // Key : 팀 Type
            //       0 : 평일 비상 조직-TemporaryNormalTeam, 
            //       1 : 휴일 비상 조직-TemporaryEmergencyTeam, 
            //       2 : 외부 기관-ExternalTeam, 
            //       3 : 사용자 정의 조직-UserDefinedTeam, 
            //       4 : 상시조직-RegularTeam)
            // Value : 타입별 Team ID List
            Dictionary<int, List<int>> dicTeamIDList = new Dictionary<int, List<int>>();

            string strSQL = string.Empty;
            strSQL += "SELECT ExternalTransmission.ID, ExternalTransmission.useSMS, ExternalTransmission.SMSText, ExternalTransmission.SMSExternalTeamIDList, ExternalTransmission.useEFax, ExternalTransmission.FaxExternalTeamIDList ";
            strSQL += "FROM ExternalTransmission, StepMember, ActionStep, Disaster, Version ";
            strSQL += "WHERE ExternalTransmission.StepMemberID = StepMember.ID ";
            strSQL += "AND StepMember.ActionStepID = ActionStep.ID ";
            strSQL += "AND ActionStep.DisasterID = Disaster.ID ";
            strSQL += "AND Disaster.VersionID = Version.ID ";
            strSQL += "AND Version.SiteID = " + FormMain.Instance.SiteID.ToString();

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            string strUpdateFormat = "UPDATE ExternalTransmission SET useSMS = {0}, SMSText = '{1}', SMSExternalTeamIDList = '{2}', useEFax = {3}, FaxExternalTeamIDList = '{4}' WHERE ID = {5}";

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nUseSMS = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                string strSMSText = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strSMSTeamList = WebDBManager.GetStringField(arrResult[i + 3], "");
                int nUseFax = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                string strFaxTeamList = WebDBManager.GetStringField(arrResult[i + 5], "");

                if (nID < 0 || strSMSTeamList.Length + strFaxTeamList.Length == 0)
                    continue;

                int nChangedUseSMS = nUseSMS;
                string strChangedSMSText = strSMSText;
                string strChangedSMSTeamList = string.Empty;
                int nChangedUseFax = nUseFax;
                string strChangedFaxTeamList = string.Empty;

                // 변경될 SMS수신자 목록 리턴
                if (Convert.ToBoolean(nUseSMS) == true)
                    strChangedSMSTeamList = ParseTeamListOnlyExternal(strSMSTeamList, externalTeamIDs);

                // 변경될 Fax수신자 목록 정보
                if (Convert.ToBoolean(nUseFax) == true)
                    strChangedFaxTeamList = ParseTeamListOnlyExternal(strFaxTeamList, externalTeamIDs);

                // SMS Fax 수신자 목록에서 수정할 항목이 없으면 다음 행으로 이동
                if (strChangedSMSTeamList == null && strChangedFaxTeamList == null)
                    continue;

                if (String.IsNullOrWhiteSpace(strChangedSMSTeamList) == true)
                {
                    nChangedUseSMS = 0;
                    strChangedSMSText = "";
                }

                if (String.IsNullOrWhiteSpace(strChangedFaxTeamList) == true)
                {
                    nChangedUseFax = 0;
                }


                // 리벗용 업데이트문 생성
                string strReverseUpdateSQL = String.Format(strUpdateFormat,
                    nUseSMS,
                    strSMSText,
                    strSMSTeamList,
                    nUseFax,
                    strFaxTeamList,
                    nID);
                updateList.Add(strReverseUpdateSQL);

                // 팀삭제로 인한 적용 쿼리 색성
                string strUpdateSQL = String.Format(strUpdateFormat,
                    nChangedUseSMS,
                    strChangedSMSText,
                    strChangedSMSTeamList,
                    nChangedUseFax,
                    strChangedFaxTeamList,
                    nID);

                if (dbMgr.GetBatchData(strUpdateSQL) == null)
                    return null;
            }

            return updateList;
        }

        // RollBack을 위하여 수정한 데이터들의 Update 구문 리스트를 반환한다.
        private List<string> UpdateInternalTransmissionDatas(WebDBManager dbMgr, string strTeamIDs)
        {
            // 2015-08-07 mwkim 변경사항
            // InternalTransmission 의 발신자, 수신자에서 삭제되도록 적용

            List<string> updateList = new List<string>();

            if (strTeamIDs.Length == 0)
                return updateList;

            List<int> externalTeamIDs = GetExternalTeamIDs(strTeamIDs);

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
                strChangedTeamIDList = ParseTeamList(strTeamList, dicTeamIDList, externalTeamIDs);

                // 변경될 발신처 정보
                if (nCommanderType == 2 && externalTeamIDs.Contains(nCommanderMemberID))
                {
                    nChangedCommanderType = -1;
                    nChangedCommanderMemberID = -1;
                    strChangedCommanderDisplayText = "SOP 제어권 가진곳의 책임자";
                }

                if (strChangedTeamIDList == null && (nCommanderType == 2 && externalTeamIDs.Contains(nCommanderMemberID)) == false)
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

            List<int> externalTeamIDs = GetExternalTeamIDs(strTeamIDs);

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
                strChangedTeamIDList = ParseTeamList(strTeamList, dicTeamIDList, externalTeamIDs);

                // 변경될 발신처 정보
                if (nCommanderType == 2 && externalTeamIDs.Contains(nCommanderMemberID))
                {
                    nChangedCommanderType = -1;
                    nChangedCommanderMemberID = -1;
                    strChangedCommanderDisplayText = "SOP 제어권 가진곳의 책임자";
                }

                if (strChangedTeamIDList == null && (nCommanderType == 2 && externalTeamIDs.Contains(nCommanderMemberID)) == false)
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

        private string ParseTeamList(string strTeamList, Dictionary<int, List<int>> dicTeamIDList, List<int> externalTeamIDs)
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

                if (nTeamType == 2 && externalTeamIDs.Contains(nTeamID))
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

        private string ParseTeamListOnlyExternal(string strTeamList, List<int> externalTeamIDs)
        {
            string rtnTeamList = null;

            bool isChanged = false;
            int nTeamID = -1;
            ArrayList arrTeamList = new ArrayList();

            foreach (string strTeamID in strTeamList.Split(','))
            {
                if (Int32.TryParse(strTeamID.Trim(), out nTeamID) == false)
                    continue;

                if (externalTeamIDs.Contains(nTeamID) == false)
                    arrTeamList.Add(strTeamID.Trim());
                else
                    isChanged = true;
            }

            if (isChanged)
                rtnTeamList = String.Join(",", arrTeamList);

            return rtnTeamList;
        }

        private List<int> GetExternalTeamIDs(string strTeamIDs)
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


    }
}
