using DBUtility2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Controls;

namespace TeamEditor.BLL.WinForms.Command
{
    public class CommandMoveRegularMembers : CommandEx
    {
        public class CompanyMemberNIndex : IComparable
        {
            private CompanyMember m_member = null;
            private int m_nIndex = -1;

            public CompanyMember Member
            {
                get { return m_member; }
                set { m_member = value; }
            }

            public int Index
            {
                get { return m_nIndex; }
                set { m_nIndex = value; }
            }

            public CompanyMemberNIndex()
            {
            }

            public CompanyMemberNIndex(CompanyMember member, int nRowIndex)
            {
                m_member = member;
                m_nIndex = nRowIndex;
            }

            public int CompareTo(object obj)
            {
                CompanyMemberNIndex row1 = this;
                CompanyMemberNIndex row2 = (CompanyMemberNIndex)obj;

                if (row1.Index < row2.Index)
                    return -1;
                else if (row1.Index > row2.Index)
                    return 1;

                return 0;
            }
        }

        private RegularTeam m_teamOrigin = null;
        private RegularTeam m_teamMoved = null;
        private List<CompanyMemberNIndex> m_rows = null;
        private MergedDataGridView m_grid = null;
        private TreeView m_tree = null;

        public RegularTeam TeamOrigin
        {
            get { return m_teamOrigin; }
            set { m_teamOrigin = value; }
        }

        public RegularTeam TeamMoved
        {
            get { return m_teamMoved; }
            set { m_teamMoved = value; }
        }

        public List<CompanyMemberNIndex> Rows
        {
            get { return m_rows; }
            set
            {
                m_rows = value;

                if (m_rows != null)
                    m_rows.Sort();
            }
        }

        public MergedDataGridView Grid
        {
            get { return m_grid; }
            set { m_grid = value; }
        }

        public TreeView Tree
        {
            get { return m_tree; }
            set { m_tree = value; }
        }
        
        public CommandMoveRegularMembers()
        {
        }

        public CommandMoveRegularMembers(RegularTeam teamOrigin, RegularTeam teamMoved, List<CompanyMemberNIndex> rows, MergedDataGridView grid, TreeView tree)
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

            List<CompanyMember> members = DataManager.GetRegularMembers(m_teamOrigin);
            List<CompanyMember> members2 = DataManager.GetRegularMembers(m_teamMoved);

            if (members2 == null)
                return;

            if (members != null)
            {
                foreach (CompanyMemberNIndex row in m_rows)
                {
                    if (row.Member == null)
                        continue;

                    members.Remove(row.Member);
                }
            }

            //if (m_grid.CurrentTeam == m_teamOrigin)
            //    m_grid.SelectTeam(m_teamOrigin, true);

            foreach (CompanyMemberNIndex row in m_rows)
            {
                if (row.Member == null)
                    continue;

                if (members2.Contains(row.Member))
                    continue;
                
                members2.Add(row.Member); 
            }

            foreach (CompanyMember item in members2)
            {
                item.Team = m_teamMoved;
            }
            //if (m_grid.CurrentTeam == m_teamMoved)
            //    m_grid.SelectTeam(m_teamMoved, true);

            //m_grid.SelectTeam(m_grid.CurrentTeam, true);
        }

        public override void RollBack()
        {
            if (m_teamOrigin == null || m_teamMoved == null || m_rows == null || m_grid == null || m_tree == null)
                return;

            List<CompanyMember> members = DataManager.GetRegularMembers(m_teamOrigin);
            List<CompanyMember> members2 = DataManager.GetRegularMembers(m_teamMoved);

            if (members == null)
                return;

            if (members2 != null)
            {
                foreach (CompanyMemberNIndex row in m_rows)
                {
                    if (row.Member == null)
                        continue;

                    members2.Remove(row.Member);
                }
            }

            //if (m_grid.CurrentTeam == m_teamMoved)
            //    m_grid.SelectTeam(m_teamMoved, true);

            foreach (CompanyMemberNIndex row in m_rows)
            {
                if (row.Member == null)
                    continue;

                if (members.Contains(row.Member))
                    continue;

                members.Insert(row.Index, row.Member);
            }

            foreach (CompanyMember item in members)
            {
                item.Team = m_teamOrigin;
            }

            //if (m_grid.CurrentTeam == m_teamOrigin)
            //    m_grid.SelectTeam(m_teamOrigin, true);

            m_grid.SelectTeam(m_grid.CurrentTeam, true);
        }

        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            if (dir)
                UpdateRegularMemberList(dbMgr, m_teamMoved);
            else
                UpdateRegularMemberList(dbMgr, m_teamOrigin);

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.COMPANY_MEMBER);
        }

        private void UpdateRegularMemberList(WebDBManager dbMgr, RegularTeam team)
        {
            if (team == null)
                return;

            string strTeamID = team.TeamID < 0 ? "NULL" : team.TeamID.ToString();
            string strMemberIDs = "";

            foreach (CompanyMemberNIndex row in m_rows)
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

            string strSQL = string.Format("Update RegularMemberList set RegularTeamID = {0} where CompanyMemberID in ({1})", strTeamID, strMemberIDs);
            dbMgr.GetResultData(strSQL);
        }
    }
}
