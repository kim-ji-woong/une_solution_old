using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Command
{
    public class CommandMoveExternalMembers : CommandEx
    {
        private ExternalTeam m_teamOrigin = null;
        private ExternalTeam m_teamMoved = null;
        private List<CommandRemoveExternalCompanyMembers.ExternalMemberNIndex> m_rows = null;
        private TeamGrid m_grid = null;
        private TeamTreeView m_tree = null;

        public ExternalTeam TeamOrigin
        {
            get { return m_teamOrigin; }
            set { m_teamOrigin = value; }
        }

        public ExternalTeam TeamMoved
        {
            get { return m_teamMoved; }
            set { m_teamMoved = value; }
        }

        public List<CommandRemoveExternalCompanyMembers.ExternalMemberNIndex> Rows
        {
            get { return m_rows; }
            set
            {
                m_rows = value;

                if (m_rows != null)
                    m_rows.Sort();
            }
        }

        public TeamGrid Grid
        {
            get { return m_grid; }
            set { m_grid = value; }
        }

        public TeamTreeView Tree
        {
            get { return m_tree; }
            set { m_tree = value; }
        }
        
        public CommandMoveExternalMembers(ExternalTeam teamOrigin, ExternalTeam teamMoved, List<CommandRemoveExternalCompanyMembers.ExternalMemberNIndex> rows, TeamGrid grid, TeamTreeView tree)
        {
            m_teamOrigin = teamOrigin;
            m_teamMoved = teamMoved;
            m_rows = rows;
            m_grid = grid;
            m_tree = tree;

            if (m_rows != null)
                m_rows.Sort();
        }

        public override void Do()
        {
            if (m_teamOrigin == null || m_teamMoved == null || m_rows == null || m_grid == null || m_tree == null)
                return;

            List<ExternalCompanyMember> members = DataManager.GetExternalCompanyMembers(m_teamOrigin);
            List<ExternalCompanyMember> members2 = DataManager.GetExternalCompanyMembers(m_teamMoved);

            if (members2 == null)
                return;

            if (members != null)
            {
                foreach (CommandRemoveExternalCompanyMembers.ExternalMemberNIndex row in m_rows)
                {
                    if (row.Member == null)
                        continue;

                    members.Remove(row.Member);
                }
            }

            if (m_grid.CurrentTeam == m_teamOrigin)
                m_grid.SelectTeam(m_teamOrigin, true);

            foreach (CommandRemoveExternalCompanyMembers.ExternalMemberNIndex row in m_rows)
            {
                if (row.Member == null)
                    continue;

                if (members2.Contains(row.Member))
                    continue;

                members2.Add(row.Member);
            }

            if (m_grid.CurrentTeam == m_teamMoved)
                m_grid.SelectTeam(m_teamMoved, true);
        }

        public override void RollBack()
        {
            if (m_teamOrigin == null || m_teamMoved == null || m_rows == null || m_grid == null || m_tree == null)
                return;

            List<ExternalCompanyMember> members = DataManager.GetExternalCompanyMembers(m_teamOrigin);
            List<ExternalCompanyMember> members2 = DataManager.GetExternalCompanyMembers(m_teamMoved);

            if (members == null)
                return;

            if (members2 != null)
            {
                foreach (CommandRemoveExternalCompanyMembers.ExternalMemberNIndex row in m_rows)
                {
                    if (row.Member == null)
                        continue;

                    members2.Remove(row.Member);
                }
            }

            if (m_grid.CurrentTeam == m_teamMoved)
                m_grid.SelectTeam(m_teamMoved, true);

            foreach (CommandRemoveExternalCompanyMembers.ExternalMemberNIndex row in m_rows)
            {
                if (row.Member == null)
                    continue;

                if (members.Contains(row.Member))
                    continue;

                members.Insert(row.Index, row.Member);
            }

            if (m_grid.CurrentTeam == m_teamOrigin)
                m_grid.SelectTeam(m_teamOrigin, true);
        }

        public override void SaveDB(DBUtility.WebDBManager dbMgr, bool dir)
        {
            if (dir)
                UpdateExternalMemberList(dbMgr, m_teamMoved);
            else
                UpdateExternalMemberList(dbMgr, m_teamOrigin);

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.EXTERNAL_MEMBER);
        }

        private void UpdateExternalMemberList(DBUtility.WebDBManager dbMgr, ExternalTeam team)
        {
            if (team == null)
                return;

            string strTeamID = team.TeamID < 0 ? "NULL" : team.TeamID.ToString();
            string strMemberIDs = "";

            foreach (CommandRemoveExternalCompanyMembers.ExternalMemberNIndex row in m_rows)
            {
                if (row.Member != null)
                {
                    if (strMemberIDs.Length == 0)
                        strMemberIDs = row.Member.ID.ToString();
                    else
                        strMemberIDs += ", " + row.Member.ID.ToString();
                }
            }

            if (strMemberIDs.Length == 0)
                return;

            string strSQL = string.Format("Update ExternalMemberList set ExternalCompanyTeamID = {0} where ExternalCompanyMemberID in ({1})", strTeamID, strMemberIDs);
            dbMgr.GetResultData(strSQL, 0);
        }
    }
}
