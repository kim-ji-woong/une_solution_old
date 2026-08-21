using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;

namespace TeamEditor.Command
{
    public class CommandRemoveExternalCompanyMembers : CommandEx
    {
        public class ExternalMemberNIndex : IComparable
        {
            private ExternalCompanyMember m_member = null;
            private int m_nIndex = -1;

            public ExternalCompanyMember Member
            {
                get { return m_member; }
                set { m_member = value; }
            }

            public int Index
            {
                get { return m_nIndex; }
                set { m_nIndex = value; }
            }

            public ExternalMemberNIndex()
            {
            }

            public ExternalMemberNIndex(ExternalCompanyMember member, int nRowIndex)
            {
                m_member = member;
                m_nIndex = nRowIndex;
            }

            public int CompareTo(object obj)
            {
                ExternalMemberNIndex row1 = this;
                ExternalMemberNIndex row2 = (ExternalMemberNIndex)obj;

                if (row1.Index < row2.Index)
                    return -1;
                else if (row1.Index > row2.Index)
                    return 1;

                return 0;
            }
        }

        private ExternalTeam m_team = null;
        private List<ExternalMemberNIndex> m_rows = null;
        private TeamGrid m_grid = null;

        private List<string> m_rollbackSQLs = new List<string>();

        public ExternalTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public List<ExternalMemberNIndex> Rows
        {
            get { return m_rows; }
            set { SetRows(value); }
        }

        public TeamGrid Grid
        {
            get { return m_grid; }
            set { m_grid = value; }
        }

        public CommandRemoveExternalCompanyMembers()
        {
        }

        public CommandRemoveExternalCompanyMembers(ExternalTeam team, List<ExternalMemberNIndex> rows, TeamGrid grid)
        {
            m_team = team;
            SetRows(rows);
            m_grid = grid;
        }

        public override void Do()
        {
            if (m_grid == null || m_team == null || m_rows == null)
                return;

            List<ExternalCompanyMember> members = DataManager.GetExternalCompanyMembers(m_team);

            if (members != null)
            {
                foreach (ExternalMemberNIndex row in m_rows)
                {
                    if (row.Member != null)
                    {
                        members.Remove(row.Member);
                    }
                }
            }

            if (m_grid.CurrentTeam == m_team)
                m_grid.SelectTeam(m_team, true);
        }

        public override void RollBack()
        {
            if (m_grid == null || m_team == null || m_rows == null)
                return;

            List<ExternalCompanyMember> members = DataManager.GetExternalCompanyMembers(m_team);

            if (members != null)
            {
                foreach (ExternalMemberNIndex row in m_rows)
                {
                    if (row.Member != null)
                    {
                        members.Insert(row.Index, row.Member);
                    }
                }
            }

            if (m_grid.CurrentTeam == m_team)
                m_grid.SelectTeam(m_team, true);
        }

        public override void SaveDB(DBUtility.WebDBManager dbMgr, bool dir)
        {
            if (m_grid == null || m_team == null || m_rows == null)
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

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER);
        }

        private bool NeedAddingDB()
        {
            List<ExternalCompanyMember> members = new List<ExternalCompanyMember>();

            foreach (ExternalMemberNIndex row in m_rows)
            {
                if (row.Member != null)
                {
                    members.Add(row.Member);
                }
            }

            foreach (DataGridViewRow row in m_grid.Rows)
            {
                if (row.Tag != null && row.Tag is ExternalCompanyMember)
                {
                    ExternalCompanyMember member = (ExternalCompanyMember)row.Tag;

                    // Grid에 삭제된 데이터가 이미 다시 들어있으니 DB에 저장해야 한다.
                    if (members.Contains(member))
                        return true;
                }
            }

            return false;
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

        private string GetExternalMemberIDs()
        {
            string strMemberIDs = "";

            foreach (ExternalMemberNIndex row in m_rows)
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

        private void RemoveDB(DBUtility.WebDBManager dbMgr)
        {
            string strExternalMemberIDs = GetExternalMemberIDs();
            RemoveDB(dbMgr, strExternalMemberIDs);
        }

        public List<string> RemoveDB(DBUtility.WebDBManager dbMgr, string strExternalMemberIDs)
        {
            m_rollbackSQLs.Clear();

            if (strExternalMemberIDs.Length == 0)
                return m_rollbackSQLs;

            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

            List<string> insertExternalMemberList = RemoveExternalMemberList(dbMgr, strExternalMemberIDs);

            if (insertExternalMemberList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }

            List<string> insertExternalCompanyMemberList = RemoveExternalCompanyMembers(dbMgr, strExternalMemberIDs);
            //List<string> insertExternalCompanyMemberList = CommandRemoveExternalCompanyTeam.RemoveExternalCompanyMembers(dbMgr, strExternalMemberIDs);

            if (insertExternalCompanyMemberList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }

            // Batch Job end - Commit
            dbMgr.BatchCommit();

            m_rollbackSQLs.AddRange(insertExternalCompanyMemberList);
            m_rollbackSQLs.AddRange(insertExternalMemberList);

            return m_rollbackSQLs;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveExternalCompanyMembers(DBUtility.WebDBManager dbMgr, string strExternalMemberIDs)
        {
            string strSQL = "Select ID, Name, PhoneNumber, Description from ExternalCompanyMember where ID in " + strExternalMemberIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            List<string> insertList = new List<string>();

            string strInsertFormat = "Insert into ExternalCompanyMember (ID, Name, PhoneNumber, Description) values ({0}, '{1}', {2}, {3})";
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                string strPhoneNumber = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
                string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 3], "");

                if (nID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nID, strName,
                    strPhoneNumber == "null" ? "NULL" : "'" + strPhoneNumber + "'",
                    strDescription == "null" ? "NULL" : "'" + strDescription + "'");
                insertList.Add(strInsert);
            }

            strSQL = "Delete from ExternalCompanyMember where ID in " + strExternalMemberIDs;

            if (dbMgr.GetBatchData(strSQL) == null)
                return null;

            return insertList;
        }

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveExternalMemberList(DBUtility.WebDBManager dbMgr, string strExternalMemberIDs)
        {
            string strSQL = "Select ExternalCompanyTeamID, ExternalCompanyMemberID, JobLevelID, JobPositionID from ExternalMemberList where ExternalCompanyMemberID in " + strExternalMemberIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            List<string> insertList = new List<string>();

            string strInsertFormat = "Insert into ExternalMemberList (ExternalCompanyTeamID, ExternalCompanyMemberID, JobLevelID, JobPositionID) values ({0}, {1}, {2}, {3})";
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nExternalCompanyTeamID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nExternalCompanyMemberID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nJobLevelID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nPositionID = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);

                if (nExternalCompanyTeamID < 0 || nExternalCompanyMemberID < 0)
                    continue;

                string strInsert = string.Format(strInsertFormat, nExternalCompanyTeamID, nExternalCompanyMemberID,
                    nJobLevelID < 0 ? "NULL" : nJobLevelID.ToString(),
                    nPositionID < 0 ? "NULL" : nPositionID.ToString());
                insertList.Add(strInsert);
            }

            strSQL = "Delete from ExternalMemberList where ExternalCompanyMemberID in " + strExternalMemberIDs;

            if (dbMgr.GetBatchData(strSQL) == null)
                return null;

            return insertList;
        }

        private void SetRows(List<ExternalMemberNIndex> rows)
        {
            m_rows = rows;

            if (m_rows != null)
                m_rows.Sort();
        }
    }
}
