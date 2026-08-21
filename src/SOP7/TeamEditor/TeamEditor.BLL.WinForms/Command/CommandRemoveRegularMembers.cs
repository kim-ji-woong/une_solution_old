using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using DBUtility2;
using UnE.Controls;

namespace TeamEditor.BLL.WinForms.Command
{
    public class CommandRemoveRegularMembers : CommandEx
    {
        private RegularTeam m_team = null;
        private List<CommandMoveRegularMembers.CompanyMemberNIndex> m_rows = null;
        private MergedDataGridView m_grid = null;

        private List<string> m_rollbackSQLs = new List<string>();

        public RegularTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public List<CommandMoveRegularMembers.CompanyMemberNIndex> Rows
        {
            get { return m_rows; }
            set { SetRows(value); }
        }

        public MergedDataGridView Grid
        {
            get { return m_grid; }
            set { m_grid = value; }
        }

        public CommandRemoveRegularMembers()
        {
        }

        public CommandRemoveRegularMembers(RegularTeam team, List<CommandMoveRegularMembers.CompanyMemberNIndex> rows, MergedDataGridView grid)
        {
            m_team = team;
            SetRows(rows);
            m_grid = grid;
        }

        public override void Do()
        {
            //if (m_grid == null || m_team == null || m_rows == null)
                //return;
            if (m_grid == null || m_rows == null)
                return;

            //List<CompanyMember> members = DataManager.GetRegularMembers(m_team);

            //if (members != null)
            {
                foreach (CommandMoveRegularMembers.CompanyMemberNIndex row in m_rows)
                {
                    if (row.Member != null)
                    {
                        List<CompanyMember> members = DataManager.GetRegularMembers(row.Member.Team);
                        members.Remove(row.Member);
                    }
                }
            }

            //if (m_grid.CurrentTeamRow == m_team)
                m_grid.SelectTeam(m_grid.CurrentTeam, true);
        }

        public override void RollBack()
        {
            //if (m_grid == null || m_team == null || m_rows == null)
            //    return;
            if (m_grid == null || m_rows == null)
                return;

            ////List<CompanyMember> members = DataManager.GetRegularMembers(m_team);

            //if (members != null)
            {
                foreach (CommandMoveRegularMembers.CompanyMemberNIndex row in m_rows)
                {
                    if (row.Member != null)
                    {
                        //List<CompanyMember> members = DataManager.GetChildRegularMembers(row.Member.Team);                        
                        //members.Insert(row.Index, row.Member);

                        DataManager.DicTeamCompanyMembers[row.Member.Team].Insert(DataManager.DicTeamCompanyMembers[row.Member.Team].Count, row.Member);
                    }
                }
            }

            //if (m_grid.CurrentTeam == m_team)
                m_grid.SelectTeam(m_grid.CurrentTeam, true);
        }

        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            if (m_grid == null || m_rows == null)
                return;

            if (dir)
            {
                if (m_rollbackSQLs.Count == 0)
                    RemoveDB(dbMgr);
            }
            else
            {
                if (NeedAddingDB())
                {
                    // 삭제했다가 RollBack한 상태인 경우 DB에서는 지워졌지만 UI에는 다시 나타나있다.
                    AddDB(dbMgr);
                }
            }

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.COMPANY_MEMBER);
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
            string strCompanyMemberIDs = GetCompanyMemberIDs();
            RemoveDB(dbMgr, strCompanyMemberIDs);
        }

        public List<string> RemoveDB(WebDBManager dbMgr, string strCompanyMemberIDs)
        {
            m_rollbackSQLs.Clear();

            if (strCompanyMemberIDs.Length == 0)
                return m_rollbackSQLs;

            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

            List<string> insertRegularMemberList = RemoveRegularMemberList(dbMgr, strCompanyMemberIDs);

            if (insertRegularMemberList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }

            List<string> insertFacilityManagerList = RemoveFacilityManagers(dbMgr, strCompanyMemberIDs);

            if (insertFacilityManagerList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }

            List<string> insertTemporaryMemberList = RemoveTemporaryMembers(dbMgr, strCompanyMemberIDs);

            if (insertTemporaryMemberList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }

            /*List<string> insertDutyList = RemoveDuty(dbMgr, strCompanyMemberIDs);

            if (insertDutyList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }*/

            List<string> updateSOPGenUserList = CommandRemoveRegularTeam.UpdateSOPGenUsers(dbMgr, strCompanyMemberIDs);

            if (updateSOPGenUserList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }

            List<string> insertCompanyMemberList = CommandRemoveRegularTeam.RemoveCompanyMembers(dbMgr, strCompanyMemberIDs);

            if (insertCompanyMemberList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }

            // Batch Job end - Commit
            dbMgr.BatchCommit();

            m_rollbackSQLs.AddRange(insertCompanyMemberList);
            m_rollbackSQLs.AddRange(updateSOPGenUserList);
            //m_rollbackSQLs.AddRange(insertDutyList);
            m_rollbackSQLs.AddRange(insertTemporaryMemberList);
            m_rollbackSQLs.AddRange(insertFacilityManagerList);
            m_rollbackSQLs.AddRange(insertRegularMemberList);

            return m_rollbackSQLs;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        /*private List<string> RemoveDuty(WebDBManager dbMgr, string strCompanyMemberIDs)
        {
            List<string> insertList = new List<string>();

            if (strCompanyMemberIDs.Length == 0)
                return insertList;

            string strSQL = "SELECT ID, MemberID, InsertTime, TeamID, Description, SiteID from ";
            strSQL += "Duty where MemberID in " + strCompanyMemberIDs;

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

            string strDeleteSQL = "Delete from Duty where MemberID in " + strCompanyMemberIDs;
            if (dbMgr.GetResultData(strDeleteSQL, 1) == null)
                return null;

            return insertList;
        }*/

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveTemporaryMembers(WebDBManager dbMgr, string strCompanyMemberIDs)
        {
            List<string> insertList = new List<string>();

            if (!RemoveTemporaryMemberList(dbMgr, strCompanyMemberIDs, insertList))
                return null;

            return insertList;
        }
        
        private bool RemoveTemporaryMemberList(WebDBManager dbMgr, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role ";
            strSQL += "from TemporaryMemberList where ";

            if (strCompanyMemberIDs.Length > 0)
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

                string strInsert = string.Format(strInsertFormat, nID, strMemberName, nTemporaryTeamID,
                    isNormal ? 1 : 0,
                    nMemberID < 0 ? "NULL" : nMemberID.ToString(),
                    nTeamLeader < 0 ? "NULL" : nTeamLeader.ToString(),
                    nMemberType,
                    nMemberCount < 0 ? "NULL" : nMemberCount.ToString(),
                    nRole);

                insertList.Add(strInsert);
            }

            return dbMgr.GetBatchData(strDeleteSQL) != null;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveFacilityManagers(WebDBManager dbMgr, string strCompanyMemberIDs)
        {
            List<string> insertList = new List<string>();

            if (!RemoveFacilityManagers(dbMgr, strCompanyMemberIDs, insertList))
                return null;

            if (!RemoveBuildingFacilityManagers(dbMgr, strCompanyMemberIDs, insertList))
                return null;

            if (!RemoveEquipZoneFacilityManagers(dbMgr, strCompanyMemberIDs, insertList))
                return null;

            return insertList;
        }

        private bool RemoveFacilityManagers(WebDBManager dbMgr, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberID, MemberType, FacilityType, LevelLimit, Description, UpperLimit, SiteID ";
            strSQL += "from FacilityManager where ";

            if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 0 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from FacilityManager where MemberType = 0 and MemberID in " + strCompanyMemberIDs;
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

        private bool RemoveEquipZoneFacilityManagers(WebDBManager dbMgr, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberID, MemberType, SiteID, FacilityType, LevelLimit, EquipZoneID, Description, UpperLimit ";
            strSQL += "from EquipZoneFacilityManager where ";

            if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 0 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from EquipZoneFacilityManager where MemberType = 0 and MemberID in " + strCompanyMemberIDs;
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

        private bool RemoveBuildingFacilityManagers(WebDBManager dbMgr, string strCompanyMemberIDs, List<string> insertList)
        {
            string strDeleteSQL = "";

            string strSQL = "SELECT ID, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, Description, UpperLimit, SiteID ";
            strSQL += "from BuildingFacilityManager where ";

            if (strCompanyMemberIDs.Length > 0)
            {
                strSQL += "MemberType = 0 and MemberID in " + strCompanyMemberIDs;
                strDeleteSQL = "Delete from BuildingFacilityManager where MemberType = 0 and MemberID in " + strCompanyMemberIDs;
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
        private List<string> RemoveRegularMemberList(WebDBManager dbMgr, string strCompanyMemberIDs)
        {
            string strSQL = "Select RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID from RegularMemberList where CompanyMemberID in " + strCompanyMemberIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            List<string> insertList = new List<string>();

            string strInsertFormat = "Insert into RegularMemberList (RegularTeamID, CompanyMemberID, PositionID, SubPositionID, GroupPositionID) values ({0}, {1}, {2}, {3}, {4})";
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
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
            }

            strSQL = "Delete from RegularMemberList where CompanyMemberID in " + strCompanyMemberIDs;

            if (dbMgr.GetBatchData(strSQL) == null)
                return null;

            return insertList;
        }

        private string GetCompanyMemberIDs()
        {
            string strMemberIDs = "";

            foreach (CommandMoveRegularMembers.CompanyMemberNIndex row in m_rows)
            {
                if (row.Member != null)
                {
                    if (strMemberIDs.Length == 0)
                        strMemberIDs = row.Member.ID.ToString();
                    else
                        strMemberIDs += ", " + row.Member.ID.ToString();
                }
            }

            if (strMemberIDs.Length > 0)
                strMemberIDs = "(" + strMemberIDs + ")";

            return strMemberIDs;
        }

        private bool NeedAddingDB()
        {
            List<CompanyMember> members = new List<CompanyMember>();

            foreach (CommandMoveRegularMembers.CompanyMemberNIndex row in m_rows)
            {
                if (row.Member != null)
                {
                    members.Add(row.Member);
                }
            }

            foreach (DataGridViewRow row in m_grid.Rows)
            {
                if (row.Tag != null && row.Tag is CompanyMember)
                {
                    CompanyMember member = (CompanyMember)row.Tag;

                    // Grid에 삭제된 데이터가 이미 다시 들어있으니 DB에 저장해야 한다.
                    if (members.Contains(member))
                        return true;
                }
            }

            return false;
        }

        private void SetRows(List<CommandMoveRegularMembers.CompanyMemberNIndex> rows)
        {
            m_rows = rows;

            if (m_rows != null)
                m_rows.Sort();
        }
    }
}
