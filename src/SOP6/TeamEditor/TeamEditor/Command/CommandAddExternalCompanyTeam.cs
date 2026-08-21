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
    public class CommandAddExternalTeam : CommandEx
    {
        private TreeView m_tree = null;
        private TreeNode m_node = null;
        private TreeNode m_nodeParent = null;
        private int m_nIndex = -1;
        private ExternalTeam m_team = null;

        public ExternalTeam Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public CommandAddExternalTeam(TreeNode node)
        {
            m_tree = node.TreeView;
            m_node = node;

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
                m_team = new ExternalTeam();
                m_team.TeamName = m_node.Text;
                m_node.Tag = m_team;

                if (m_nodeParent != null)
                    (m_team as ExternalTeam).ParentTeam = (m_nodeParent.Tag as ExternalTeam);

                FormMain.Instance.SetCurrentExternalTeam(m_team);

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

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.EXTERNAL_TEAM);
        }

        private void RemoveTeam(WebDBManager dbMgr)
        {
            CommandRemoveExternalTeam cmd = new CommandRemoveExternalTeam(m_node, m_team, true);
            cmd.SaveDB(dbMgr, true);
        }

        private bool CheckNAdd(WebDBManager dbMgr)
        {
            string strSQL = "Select id from ExternalTeam where ID = " + m_team.TeamID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
                return AddTeam(dbMgr);

            return true;
        }

        private bool AddTeam(WebDBManager dbMgr)
        {
            Team teamParent = null;

            if (m_nodeParent != null)
            {
                teamParent = (Team)m_nodeParent.Tag;

                if (teamParent == null || teamParent.TeamID < 0)
                    return false;
            }

            dbMgr.BeginBatch();

            string strIFNull = dbMgr.DatabaseType == WebDBManager.DBType.sqlserver ? "ISNULL" : "IFNULL";

            string strSQL = String.Format("SELECT {1}(MAX(ID), {0} * 1000) FROM ExternalTeam WHERE SiteID = {0}", FormMain.Instance.SiteID, strIFNull);

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            int nID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            if (teamParent == null)
            {
                strSQL = string.Format("Insert into ExternalTeam (ID, TeamName, PhoneNumber, FaxNumber, SiteID) values ({0}, '{1}', '0000', '0000', {2})",
                    nID, m_node.Text, FormMain.Instance.SiteID);
            }
            else
            {
                strSQL = string.Format("Insert into ExternalTeam (ID, TeamName, PhoneNumber, FaxNumber, SiteID, ParentTeamID) values ({0}, '{1}', '0000', '0000', {2}, {3})",
                    nID, m_node.Text, FormMain.Instance.SiteID, teamParent.TeamID);
            }

            if (dbMgr.GetBatchData(strSQL) == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            dbMgr.BatchCommit();

            m_team.TeamID = nID;
            m_team.TeamName = m_node.Text;
            m_node.Tag = m_team;

            DataManager.SetExternalTeam(nID, m_team);

            return true;
        }
    }
}
