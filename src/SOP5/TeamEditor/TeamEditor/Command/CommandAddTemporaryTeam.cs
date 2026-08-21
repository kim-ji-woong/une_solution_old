using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace TeamEditor.Command
{
    public class CommandAddTemporaryTeam : CommandEx
    {
        private TeamTreeView m_tree = null;
        private TreeNode m_nodeParent = null;
        private TreeNode m_node = null;
        private bool m_isNormal = true;
        private int m_nIndex = -1;
        private Team m_team = null;

        public TeamTreeView Tree
        {
            get { return m_tree; }
            set
            {
                m_tree = value;
                SetIndex();
            }
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

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }

        public Team Team
        {
            get { return m_team; }
            set { m_team = value; }
        }

        public CommandAddTemporaryTeam(TeamTreeView tree, TreeNode node, bool isNormal)
        {
            m_tree = tree;
            m_node = node;
            m_isNormal = isNormal;

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
                if (m_isNormal)
                    m_team = new TemporaryNormalTeam();
                else
                    m_team = new TemporaryEmergencyTeam();

                m_team.TeamName = m_node.Text;
                m_node.Tag = m_team;

                FormMain.Instance.SetCurrentTemporaryTeam(m_team);

                if (m_isNormal)
                {
                    DataManager.SetTemporaryNormalMembers((TemporaryNormalTeam)m_team);

                    if (m_nodeParent != null && m_nodeParent.Tag != null)
                    {
                        TemporaryNormalTeam teamParent = (TemporaryNormalTeam)m_nodeParent.Tag;
                        ((TemporaryNormalTeam)m_team).ParentTeam = teamParent;
                    }
                }
                else
                {
                    DataManager.SetTemporaryEmergencyMembers((TemporaryEmergencyTeam)m_team);

                    if (m_nodeParent != null && m_nodeParent.Tag != null)
                    {
                        TemporaryEmergencyTeam teamParent = (TemporaryEmergencyTeam)m_nodeParent.Tag;
                        ((TemporaryEmergencyTeam)m_team).ParentTeam = teamParent;
                    }
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

        public override void SaveDB(DBUtility.WebDBManager dbMgr, bool dir)
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

            if (m_isNormal)
                UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPARARY_NORMAL_TEAM);
            else
                UpdateConfig(dbMgr, SOP.SDMSConfig.ConfigType.TEMPARAY_EMERGENCY_TEAM);
        }

        private void RemoveTeam(DBUtility.WebDBManager dbMgr)
        {
            CommandRemoveTemporaryTeam cmd = new CommandRemoveTemporaryTeam(m_tree, m_node, m_nodeParent, m_nIndex, m_team, m_isNormal);
            cmd.SaveDB(dbMgr, true);
        }

        private bool CheckNAdd(DBUtility.WebDBManager dbMgr)
        {
            string strTableName = m_isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";

            string strSQL = "Select id from " + strTableName + " where ID = " + m_team.TeamID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
                return AddTeam(dbMgr);

            return true;
        }

        private bool AddTeam(DBUtility.WebDBManager dbMgr)
        {
            Team teamParent = null;

            if (m_nodeParent != null)
            {
                teamParent = (Team)m_nodeParent.Tag;

                if (teamParent == null || teamParent.TeamID < 0)
                    return false;
            }

            dbMgr.BeginBatch();

            string strTableName = m_isNormal ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strSQL = "Select max(ID) from " + strTableName;

            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            strSQL = string.Format("Insert into {0} (ID, TeamName, ParentTeamID, GroupName, Description , SiteID) values ({1}, '{2}', {3}, NULL, NULL, {4})",
                strTableName, nID, m_node.Text,
                teamParent == null ? "NULL" : teamParent.TeamID.ToString(),
                FormMain.Instance.SiteID);

            if (dbMgr.GetBatchData(strSQL) == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            dbMgr.BatchCommit();

            m_team.TeamID = nID;
            m_team.TeamName = m_node.Text;
            m_node.Tag = m_team;

            DataManager.SetTemporaryTeam(nID, m_team, m_isNormal);

            return true;
        }
    }
}
