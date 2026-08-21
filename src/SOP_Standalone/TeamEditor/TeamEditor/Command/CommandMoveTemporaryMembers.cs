using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Command
{
    public class CommandMoveTemporaryMembers : CommandEx
    {
        public class TemporaryMemberNIndex : IComparable
        {
            private TemporaryMember m_member = null;
            private int m_nIndex = -1;

            public TemporaryMember Member
            {
                get { return m_member; }
                set { m_member = value; }
            }

            public int Index
            {
                get { return m_nIndex; }
                set { m_nIndex = value; }
            }

            public TemporaryMemberNIndex()
            {
            }

            public TemporaryMemberNIndex(TemporaryMember member, int nRowIndex)
            {
                m_member = member;
                m_nIndex = nRowIndex;
            }

            public int CompareTo(object obj)
            {
                TemporaryMemberNIndex row1 = this;
                TemporaryMemberNIndex row2 = (TemporaryMemberNIndex)obj;

                if (row1.Index < row2.Index)
                    return -1;
                else if (row1.Index > row2.Index)
                    return 1;

                return 0;
            }
        }

        private Team m_teamOrigin = null;
        private Team m_teamMoved = null;
        private bool m_isNormal = false, m_isValid = false;
        private List<TemporaryMemberNIndex> m_rows = null;
        private TeamGrid m_grid = null;
        private TeamTreeView m_tree = null;

        public Team TeamOrigin
        {
            get { return m_teamOrigin; }
            set
            {
                m_teamOrigin = value;
                m_isValid = IsValid();
            }
        }

        public Team TeamMoved
        {
            get { return m_teamMoved; }
            set
            {
                m_teamMoved = value;
                m_isValid = IsValid();
            }
        }

        public List<TemporaryMemberNIndex> Rows
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
        
        public CommandMoveTemporaryMembers()
        {
        }

        public CommandMoveTemporaryMembers(Team teamOrigin, Team teamMoved, List<TemporaryMemberNIndex> rows, TeamGrid grid, TeamTreeView tree)
        {
            m_teamOrigin = teamOrigin;
            m_teamMoved = teamMoved;
            m_rows = rows;
            m_grid = grid;
            m_tree = tree;
            m_isValid = IsValid();

            if (m_rows != null)
                m_rows.Sort();
        }

        private bool IsValid()
        {
            if (m_teamOrigin == null || m_teamMoved == null)
                return false;
            else if ((m_teamOrigin is TemporaryNormalTeam) && (m_teamMoved is TemporaryNormalTeam))
                m_isNormal = true;
            else if ((m_teamOrigin is TemporaryEmergencyTeam) && (m_teamMoved is TemporaryEmergencyTeam))
                m_isNormal = false;
            else
                return false;

            return true;
        }

        public override void Do()
        {
            if (m_teamOrigin == null || m_teamMoved == null || m_rows == null || m_grid == null || m_tree == null || m_isValid == false)
                return;

            List<TemporaryMember> members = null, members2 = null;

            if (m_isNormal)
            {
                members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)m_teamOrigin);
                members2 = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)m_teamMoved);

                if (members2 == null)
                {
                    TemporaryNormalTeam teamMoved = (TemporaryNormalTeam)m_teamMoved;
                    DataManager.SetTemporaryNormalMembers(teamMoved, teamMoved.Members);
                    members2 = teamMoved.Members;
                }
            }
            else
            {
                members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_teamOrigin);
                members2 = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_teamMoved);

                if (members2 == null)
                {
                    TemporaryEmergencyTeam teamMoved = (TemporaryEmergencyTeam)m_teamMoved;
                    DataManager.SetTemporaryEmergencyMembers(teamMoved, teamMoved.Members);
                    members2 = teamMoved.Members;
                }
            }

            if (members2 == null)
                return;

            if (members != null)
            {
                foreach (TemporaryMemberNIndex row in m_rows)
                {
                    if (row.Member == null)
                        continue;

                    members.Remove(row.Member);
                }
            }

            if (m_grid.CurrentTeam == m_teamOrigin)
                m_grid.SelectTeam(m_teamOrigin, true);

            foreach (TemporaryMemberNIndex row in m_rows)
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
            if (m_teamOrigin == null || m_teamMoved == null || m_rows == null || m_grid == null || m_tree == null || m_isValid == false)
                return;

            List<TemporaryMember> members = null, members2 = null;

            if (m_isNormal)
            {
                members = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)m_teamOrigin);
                members2 = DataManager.GetTemporaryNormalMembers((TemporaryNormalTeam)m_teamMoved);
            }
            else
            {
                members = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_teamOrigin);
                members2 = DataManager.GetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_teamMoved);
            }

            if (members == null)
                return;

            if (members2 != null)
            {
                foreach (TemporaryMemberNIndex row in m_rows)
                {
                    if (row.Member == null)
                        continue;

                    members2.Remove(row.Member);
                }
            }

            if (m_grid.CurrentTeam == m_teamMoved)
                m_grid.SelectTeam(m_teamMoved, true);

            foreach (TemporaryMemberNIndex row in m_rows)
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
                UpdateTemporaryMemberList(dbMgr, m_teamMoved);
            else
                UpdateTemporaryMemberList(dbMgr, m_teamOrigin);

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPORARY_MEMBER);
        }

        private void UpdateTemporaryMemberList(DBUtility.WebDBManager dbMgr, Team team)
        {
            if (team == null)
                return;

            string strTeamID = team.TeamID < 0 ? "NULL" : team.TeamID.ToString();
            string strMemberIDs = "";

            foreach (TemporaryMemberNIndex row in m_rows)
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

            string strSQL = string.Format("Update TemporaryMemberList set TemporaryTeamID = {0} where ID in ({1})", strTeamID, strMemberIDs);
            dbMgr.GetResultData(strSQL, 0);
        }
    }
}
