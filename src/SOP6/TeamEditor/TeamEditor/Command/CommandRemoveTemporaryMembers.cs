using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;
using DBUtility2;

namespace TeamEditor.Command
{
    public class CommandRemoveTemporaryMembers : CommandEx
    {
        private Team m_team = null;
        private List<CommandMoveTemporaryMembers.TemporaryMemberNIndex> m_rows = null;
        private TeamGrid m_grid = null;

        private List<string> m_rollbackSQLs = new List<string>();

        public Team Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public List<CommandMoveTemporaryMembers.TemporaryMemberNIndex> Rows
        {
            get { return m_rows; }
            set { SetRows(value); }
        }

        public TeamGrid Grid
        {
            get { return m_grid; }
            set { m_grid = value; }
        }

        public CommandRemoveTemporaryMembers()
        {
        }

        public CommandRemoveTemporaryMembers(Team team, List<CommandMoveTemporaryMembers.TemporaryMemberNIndex> rows, TeamGrid grid)
        {
            m_team = team;
            SetRows(rows);
            m_grid = grid;
        }

        public override void Do()
        {
            if (m_grid == null || m_team == null || m_rows == null)
                return;

            List<TemporaryMember> members = null;
            
            if (m_team is TemporaryNormalTeam)
                members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)m_team);
            else if (m_team is TemporaryEmergencyTeam)
                members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_team);

            if (members != null)
            {
                foreach (CommandMoveTemporaryMembers.TemporaryMemberNIndex row in m_rows)
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

            List<TemporaryMember> members = null;

            if (m_team is TemporaryNormalTeam)
                members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)m_team);
            else if (m_team is TemporaryEmergencyTeam)
                members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_team);

            if (members != null)
            {
                foreach (CommandMoveTemporaryMembers.TemporaryMemberNIndex row in m_rows)
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

        public override void SaveDB(WebDBManager dbMgr, bool dir)
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

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER);
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

        private bool NeedAddingDB()
        {
            List<TemporaryMember> members = new List<TemporaryMember>();

            foreach (CommandMoveTemporaryMembers.TemporaryMemberNIndex row in m_rows)
            {
                if (row.Member != null)
                {
                    members.Add(row.Member);
                }
            }

            foreach (DataGridViewRow row in m_grid.Rows)
            {
                if (row.Tag != null && row.Tag is TemporaryMember)
                {
                    TemporaryMember member = (TemporaryMember)row.Tag;

                    // Grid에 삭제된 데이터가 이미 다시 들어있으니 DB에 저장해야 한다.
                    if (members.Contains(member))
                        return true;
                }
            }

            return false;
        }

        private void RemoveDB(WebDBManager dbMgr)
        {
            string strTemporaryMemberIDs = GetTemporaryMemberIDs();
            RemoveDB(dbMgr, strTemporaryMemberIDs);
        }

        private string GetTemporaryMemberIDs()
        {
            string strMemberIDs = "";

            foreach (CommandMoveTemporaryMembers.TemporaryMemberNIndex row in m_rows)
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

        // RollBack을 위하여 삭제한 데이터들의 Insert 구문 리스트를 반환한다.
        private List<string> RemoveTemporaryMemberList(WebDBManager dbMgr, string strTemporaryMemberIDs)
        {
            string strSQL = "Select ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role from TemporaryMemberList where ID in " + strTemporaryMemberIDs;
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
                return null;

            List<string> insertList = new List<string>();

            string strInsertFormat = "Insert into TemporaryMemberList (ID, MemberName, TemporaryTeamID, IsNormal, MemberID, IsTeamLeader, MemberType, MemberCount, Role) values ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})";
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strDisplayMember = WebDBManager.GetStringField(arrResult[i + 1], "");
                int nTemporaryTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                bool isNormal = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0) == 0 ? false : true;
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nTeamLeader = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0) == 0 ? 0 : 1;
                int nMemberType = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nMemberCount = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                int nRole = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                if (nID < 0 || nTemporaryTeamID < 0)
                    continue;

                TemporaryMember.MemberType memberType;
                if (!TemporaryMember.ToMemberType(nMemberType, out memberType))
                    continue;

                bool isTeamType = TemporaryMember.IsTeamType(memberType);

                string strInsert = string.Format(strInsertFormat, nID, strDisplayMember, nTemporaryTeamID,
                    isNormal ? 1 : 0, nMemberID,
                    isTeamType ? nTeamLeader.ToString() : "NULL",
                    nMemberType,
                    nMemberCount < 0 ? "NULL" : nMemberCount.ToString(),
                    nRole < 0 ? "NULL" : nRole.ToString());
                insertList.Add(strInsert);
            }

            strSQL = "Delete from TemporaryMemberList where ID in " + strTemporaryMemberIDs;

            if (dbMgr.GetBatchData(strSQL) == null)
                return null;

            return insertList;
        }

        public List<string> RemoveDB(WebDBManager dbMgr, string strTemporaryMemberIDs)
        {
            m_rollbackSQLs.Clear();

            if (strTemporaryMemberIDs.Length == 0)
                return m_rollbackSQLs;

            // Batch Job Start - Begin Transaction
            dbMgr.BeginBatch();

            List<string> insertTemporaryMemberList = RemoveTemporaryMemberList(dbMgr, strTemporaryMemberIDs);

            if (insertTemporaryMemberList == null)
            {
                // Rollback
                dbMgr.BatchRollback();
                return null;
            }

            // Batch Job end - Commit
            dbMgr.BatchCommit();

            m_rollbackSQLs.AddRange(insertTemporaryMemberList);

            return m_rollbackSQLs;
        }

        private void SetRows(List<CommandMoveTemporaryMembers.TemporaryMemberNIndex> rows)
        {
            m_rows = rows;

            if (m_rows != null)
                m_rows.Sort();
        }
    }
}
