using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor.Command
{
    public class CommandRemoveUserDefinedTeam : CommandEx
    {
        private List<UserDefinedTeam> m_teams = null;
        private TeamGrid m_grid = null;

        private List<string> m_rollbackSQLs = new List<string>();

        public List<UserDefinedTeam> Teams
        {
            get { return m_teams; }
            set { m_teams = value; }
        }

        public TeamGrid Grid
        {
            get { return m_grid; }
            set { m_grid = value; }
        }

        public CommandRemoveUserDefinedTeam() { }

        public CommandRemoveUserDefinedTeam(List<UserDefinedTeam> teams, TeamGrid grid)
        {
            m_teams = teams;
            m_grid = grid;
        }

        public override void Do()
        {
            if (m_grid == null || m_teams == null)
                return;

            List<UserDefinedTeam> teams = DataManager.GetUserDefinedTeams();

            if (teams != null)
            {
                foreach (UserDefinedTeam team in m_teams)
                {
                    teams.Remove(team);
                }
            }

            m_grid.RefreshGrid();
        }

        public override void RollBack()
        {
            if (m_grid == null || m_teams == null)
                return;

            List<UserDefinedTeam> teams = DataManager.GetUserDefinedTeams();

            if (teams != null)
            {
                foreach (UserDefinedTeam team in m_teams)
                {
                    teams.Add(team);
                }
            }

            m_grid.RefreshGrid();
        }

        public override void SaveDB(DBUtility.WebDBManager dbMgr, bool dir)
        {
            if (m_grid == null || m_teams == null)
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

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.USER_DEFINED_TEAM);
        }

        private void AddDB(DBUtility.WebDBManager dbMgr)
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

        private void RemoveDB(DBUtility.WebDBManager dbMgr)
        {
            string strUserDefinedTeamIDs = String.Empty;

            List<string> liUserDefinedTeamIDs = new List<string>();

            foreach (UserDefinedTeam team in m_teams)
            {
                liUserDefinedTeamIDs.Add(team.TeamID.ToString());
            }

            strUserDefinedTeamIDs = String.Format("({0})", String.Join(",", liUserDefinedTeamIDs.ToArray()));

            RemoveDB(dbMgr, strUserDefinedTeamIDs);
        }

        public List<string> RemoveDB(DBUtility.WebDBManager dbMgr, string strUserDefinedTeamIDs)
        {
            m_rollbackSQLs.Clear();

            if (strUserDefinedTeamIDs.Length == 0)
                return m_rollbackSQLs;

            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

            List<string> insertActionStepUsingUserDefinedTeamList = RemoveActionStepUsingUserDefinedTeam(dbMgr, strUserDefinedTeamIDs);

            if (insertActionStepUsingUserDefinedTeamList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }

            List<string> insertUserDefinedTeamList = RemoveUserDefinedTeam(dbMgr, strUserDefinedTeamIDs);

            if (insertUserDefinedTeamList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }

            // Batch Job end - Commit
            dbMgr.BatchCommit();

            m_rollbackSQLs.AddRange(insertUserDefinedTeamList);
            m_rollbackSQLs.AddRange(insertActionStepUsingUserDefinedTeamList);

            return m_rollbackSQLs;
        }

        private List<string> RemoveUserDefinedTeam(DBUtility.WebDBManager dbMgr, string strUserDefinedTeamIDs)
        {
            List<string> insertList = new List<string>();

            if (strUserDefinedTeamIDs.Length == 0)
                return insertList;

            string strInsertFormat = "Insert into UserDefinedTeam (ID, TeamName, PhoneNumber, FaxNumber, SiteID) ";
            strInsertFormat += "values ({0}, '{1}', {2}, {3}, {4})";

            string strSQL = "SELECT ID, TeamName, PhoneNumber, FaxNumber from UserDefinedTeam where ID in " + strUserDefinedTeamIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strTeamName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], null);
                string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 2].ToString(), null);
                string strFaxNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 3].ToString(), null);

                if (nID < 0 || strTeamName == null )
                    continue;

                string strInsert = string.Format(strInsertFormat,
                    nID,
                    strTeamName,
                    String.IsNullOrWhiteSpace(strPhoneNumber) || strPhoneNumber == "null" ? "NULL" : "'" + strPhoneNumber + "'",
                    String.IsNullOrWhiteSpace(strFaxNumber) || strFaxNumber == "null" ? "NULL" : "'" + strFaxNumber + "'",
                    FormMain.Instance.SiteID);

                insertList.Add(strInsert);
            }

            string strDeleteSQL = "Delete from UserDefinedTeam where ID in " + strUserDefinedTeamIDs;
            if (dbMgr.GetBatchData(strDeleteSQL) == null)
                return null;

            return insertList;
        }

        private List<string> RemoveActionStepUsingUserDefinedTeam(DBUtility.WebDBManager dbMgr, string strUserDefinedTeamIDs)
        {
            List<string> insertList = new List<string>();

            if (strUserDefinedTeamIDs.Length == 0)
                return insertList;

            string strSQL = "SELECt ID, ActionStepHistoryID, UserDefinedTeamID, PhoneNumber, UserName ";
            strSQL += "FROM ActionStepUsingUserDefinedTeam WHERE UserDefinedTeamID IN " + strUserDefinedTeamIDs;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            string strInsertFormat = "INSERT INTO ActionStepUsingUserDefinedTeam (ID, ActionStepHistoryID, UserDefinedTeamID, PhoneNumber, UserName) ";
            strInsertFormat += "VALUES ({0}, {1}, {2}, {3}, {4})";

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nActionStepHistoryID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nUserDefinedTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], null);
                string strUserName = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], null);

                if (nID < 0 || nActionStepHistoryID < 0 || nUserDefinedTeamID < 0)
                    continue;

                insertList.Add(
                    String.Format(strInsertFormat,
                    nID, nActionStepHistoryID, nUserDefinedTeamID,
                    (strPhoneNumber == null ? "NULL" : "'" + strPhoneNumber + "'"),
                    (strUserName == null ? "NULL" : "'" + strUserName + "'"))
                    );
            }

            string strDeleteSQL = "DELETE FROM ActionStepUsingUserDefinedTeam WHERE UserDefinedTeamID IN " + strUserDefinedTeamIDs;
            if (dbMgr.GetBatchData(strDeleteSQL) == null)
                return null;

            return insertList;
        }


        private bool NeedAddingDB()
        {
            List<UserDefinedTeam> teams = new List<UserDefinedTeam>();

            foreach (UserDefinedTeam team in m_teams)
            {
                teams.Add(team);
            }

            foreach (DataGridViewRow row in m_grid.Rows)
            {
                if (row.Tag != null && row.Tag is UserDefinedTeam)
                {
                    UserDefinedTeam member = (UserDefinedTeam)row.Tag;

                    // Grid에 삭제된 데이터가 이미 다시 들어있으니 DB에 저장해야 한다.
                    if (teams.Contains(member))
                        return true;
                }
            }

            return false;
        }



    }
}
