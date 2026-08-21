using DBUtility2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor.BLL.WinForms.Command
{
    public class CommandMoveRegularTeam : CommandEx
    {
        private TreeNode m_nodeSrcParent = null;
        private TreeNode m_nodeSrc = null;
        private TreeNode m_nodeTrg = null;

        private RegularTeam m_teamSrcParent = null;
        private RegularTeam m_teamSrc = null;
        private RegularTeam m_teamTrg = null;

        private TreeView m_tree = null;
        private int m_nSrcIndex = -1;

        public TreeNode NodeSrcParent
        {
            get { return m_nodeSrcParent; }
            set { SetTeamNode(ref m_nodeSrcParent, value, ref m_teamSrcParent); }
        }

        public TreeNode NodeSrc
        {
            get { return m_nodeSrc; }
            set { SetTeamNode(ref m_nodeSrc, value, ref m_teamSrc); }
        }

        public TreeNode NodeTrg
        {
            get { return m_nodeTrg; }
            set { SetTeamNode(ref m_nodeTrg, value, ref m_teamTrg); }
        }

        public TreeView Tree
        {
            get { return m_tree; }
            set { m_tree = value; }
        }

        public CommandMoveRegularTeam()
        {
        }

        public CommandMoveRegularTeam(TreeView tree, TreeNode nodeSrcParent, TreeNode nodeSrc, TreeNode nodeTrg)
        {
            SetTeamNode(ref m_nodeSrcParent, nodeSrcParent, ref m_teamSrcParent);
            SetTeamNode(ref m_nodeSrc, nodeSrc, ref m_teamSrc);
            SetTeamNode(ref m_nodeTrg, nodeTrg, ref m_teamTrg);

            m_tree = tree;
        }

        private void SetTeamNode(ref TreeNode nodeTrg, TreeNode nodeSrc, ref RegularTeam team)
        {
            if (nodeSrc == null)
            {
                nodeTrg = null;
                team = null;
                return;
            }

            if (nodeSrc.Tag == null)
                return;

            team = (RegularTeam)nodeSrc.Tag;
            //int nTeamID = (int)nodeSrc.Tag;

            //team = DataManager.GetRegularTeam(nTeamID);
            nodeTrg = nodeSrc;
        }

        public override void Do()
        {
            if (m_nodeSrc == null || m_tree == null || m_nodeTrg == null)
                return;

            // Remove drag node from parent
            if (m_nodeSrcParent == null)
            {
                m_nSrcIndex = m_tree.Nodes.IndexOf(m_nodeSrc);
                m_tree.Nodes.Remove(m_nodeSrc);
            }
            else
            {
                m_nSrcIndex = m_nodeSrcParent.Nodes.IndexOf(m_nodeSrc);
                m_nodeSrcParent.Nodes.Remove(m_nodeSrc);
            }

            // Add drag node to drop node
            m_nodeTrg.Nodes.Add(m_nodeSrc);
            m_nodeTrg.ExpandAll();
        }

        public override void RollBack()
        {
            if (m_nodeSrc == null || m_tree == null || m_nodeTrg == null || m_nSrcIndex < 0)
                return;

            m_nodeTrg.Nodes.Remove(m_nodeSrc);

            if (m_nodeSrcParent == null)
                m_tree.Nodes.Insert(m_nSrcIndex, m_nodeSrc);
            else
                m_nodeSrcParent.Nodes.Insert(m_nSrcIndex, m_nodeSrc);
        }

        public override void SaveDB(WebDBManager dbMgr, bool dir)
        {
            if (m_nodeSrc == null || m_tree == null || m_nodeTrg == null)
                return;

            TreeNodeCollection nodes = m_nodeSrcParent == null ? m_tree.Nodes : m_nodeSrcParent.Nodes;

            if (dir)
            {
                if (!nodes.Contains(m_nodeSrc))
                    UpdateDB(dbMgr, m_teamTrg);
            }
            else
            {
                if (nodes.Contains(m_nodeSrc))
                    UpdateDB(dbMgr, m_teamSrcParent);
            }

            UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.REGULAR_TEAM);

            /*if (nodes.Contains(m_nodeSrc))
                UpdateDB(dbMgr, m_teamSrcParent);
            else
                UpdateDB(dbMgr, m_teamTrg);*/
        }

        private void UpdateDB(WebDBManager dbMgr, RegularTeam teamParent)
        {
            string strSQL = string.Format("Update RegularTeam set ParentTeamID = {0} where ID = {1}",
                teamParent == null ? "NULL" : teamParent.TeamID.ToString(),
                m_teamSrc.TeamID);

            dbMgr.GetResultData(strSQL);
        }
    }
}
