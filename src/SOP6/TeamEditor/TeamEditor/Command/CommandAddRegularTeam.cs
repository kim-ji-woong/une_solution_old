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
    public class CommandAddRegularTeam : CommandEx
    {
        private RegularTeam m_team = null;
        private TreeNode m_node = null;
        private int m_nIndex = -1;
        private TeamTreeView m_tree = null;
        private TreeNode m_nodeParent = null;

        public RegularTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public TreeNode Node
        {
            get { return m_node; }
            set
            {
                m_node = value;
                SetIndex();
            }
        }

        public TeamTreeView Tree
        {
            get { return m_tree; }
            set
            {
                m_tree = value;
                SetIndex();
            }
        }
        private bool m_refreshGrid = true;

        public CommandAddRegularTeam()
        {
        }

        public CommandAddRegularTeam(TeamTreeView tree, TreeNode node, RegularTeam team, bool refreshGrid = true)
        {
            m_tree = tree;
            m_node = node;
            m_team = team;
            this.m_refreshGrid = refreshGrid;
            SetIndex();
        }

        private void SetIndex()
        {
            if (m_tree == null || m_node == null)
            {
                m_nIndex = -1;
                return;
            }

            TreeNodeCollection nodes = m_node.Parent == null ? m_tree.Nodes : m_node.Parent.Nodes;
            m_nIndex = nodes.IndexOf(m_node);

            m_nodeParent = m_node.Parent;
        }

        public override void Do()
        {
            if (m_nIndex < 0)
                return;

            TreeNodeCollection nodes = m_nodeParent == null ? m_tree.Nodes : m_nodeParent.Nodes;

            if (!nodes.Contains(m_node))
            {
                nodes.Insert(m_nIndex, m_node);

                if (m_nodeParent == null)
                    m_tree.ExpandAll();
                else
                    m_nodeParent.ExpandAll();
            }

            if (m_team == null)
            {
                m_team = new RegularTeam();
                if ((RegularTeam)m_node.Tag != null)
                    m_team = (RegularTeam)m_node.Tag;
                m_team.TeamName = m_node.Text;
                m_node.Tag = m_team;

                if (m_refreshGrid)
                    FormMain.Instance.SetCurrentRegularTeam(m_team);
                DataManager.SetRegularMembers(m_team);

                if (m_nodeParent != null && m_nodeParent.Tag != null)
                {
                    RegularTeam teamParent = (RegularTeam)m_nodeParent.Tag;
                    //int nParentTeamID = (int)m_nodeParent.Tag;
                    //RegularTeam teamParent = DataManager.GetRegularTeam(nParentTeamID);
                    m_team.ParentTeam = teamParent;
                }
            }
        }

        public override void RollBack()
        {
            if (m_nIndex < 0)
                return;

            TreeNodeCollection nodes = m_nodeParent == null ? m_tree.Nodes : m_nodeParent.Nodes;
            nodes.Remove(m_node);
        }
         
        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            if (m_nIndex < 0)
                return;

            TreeNodeCollection nodes = m_nodeParent == null ? m_tree.Nodes : m_nodeParent.Nodes;

            if (dir)
            {
                if (nodes.Contains(m_node))
                {
                    if (m_team.TeamID < 0)
                        AddTeam(dbMgr);
                    else
                        CheckNAdd(dbMgr);
                }
            }
            else
            {
                if (m_team.TeamID >= 0)
                    RemoveTeam(dbMgr);
            }

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.REGULAR_TEAM);

            /*if (nodes.Contains(m_node))
            {
                if (m_team.TeamID < 0)
                    AddTeam(dbMgr);
                else
                    CheckNAdd(dbMgr);
            }
            else
            {
                if (m_team.TeamID >= 0)
                    RemoveTeam(dbMgr);
            }*/
        }

        private void RemoveTeam(WebDBManager dbMgr)
        {
            CommandRemoveRegularTeam cmd = new CommandRemoveRegularTeam(m_node, m_nodeParent, m_nIndex, m_team);
            cmd.SaveDB(dbMgr, true);
        }

        private bool CheckNAdd(WebDBManager dbMgr)
        {
            string strSQL = "Select id from RegularTeam where ID = " + m_team.TeamID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
                return AddTeam(dbMgr);

            return true;
        }

        private bool AddTeam(WebDBManager dbMgr)
        {
            dbMgr.BeginBatch();

            int nParentTeamID = GetParentTeamID(dbMgr);

            if (nParentTeamID < 0)
            {
                dbMgr.BatchRollback();
                return false;
            }

            string strSQL = "Select max(ID) from RegularTeam";
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            int nID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            strSQL = string.Format("Insert into RegularTeam (ID, TeamName, ParentTeamID) values ({0}, '{1}', {2})",
                nID, m_node.Text, nParentTeamID);

            if (dbMgr.GetBatchData(strSQL) == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            dbMgr.BatchCommit();

            DataManager.SetRegularTeamMemberInfo(m_team.TeamID, nID, m_team);            

            m_team.TeamID = nID;
            m_team.TeamName = m_node.Text;
            m_node.Tag = m_team;
            //m_node.Tag = nID; 

            //DataManager.SetRegularTeam(nID, m_team);

            return true;
        }

        private int GetParentTeamID(WebDBManager dbMgr)
        {
            if (m_team.ParentTeam != null)
                return m_team.ParentTeam.TeamID;

            if (m_nodeParent != null && m_nodeParent.Tag != null)
            {
                //int nParentTeamID = (int)m_nodeParent.Tag;
                //m_team.ParentTeam = DataManager.GetRegularTeam(nParentTeamID);
                m_team.ParentTeam = (RegularTeam)m_nodeParent.Tag;
                return m_team.ParentTeam.TeamID;
                //return nParentTeamID;
            }

            string strSQL = "Select TeamID from Site where ID = " + FormMain.Instance.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nTeamID;
        }
    }
}
