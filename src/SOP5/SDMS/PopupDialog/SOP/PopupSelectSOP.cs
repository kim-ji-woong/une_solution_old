using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS.PopupDialog.SOP
{
    public partial class PopupSelectSOP : Form
    {
        private class SOPTreeSim : UnE.SOP.Tree.SOPTreeView
        {
            private TreeNode m_CurrentNode = null;
            public TreeNode CurrentNode
            {
                get { return m_CurrentNode; }
            }

            public override void SelectNode(TreeNode node)
            {
                m_CurrentNode = node;
            }

            public void RemoveActionStepNodes()
            {
                RemoveNodes(UnE.SOP.Tree.TreeNodeType.ACTIONSTEP_NODE, this.Nodes);
            }

            private void RemoveNodes(UnE.SOP.Tree.TreeNodeType type, TreeNodeCollection nodes)
            {
                List<TreeNode> removeNodes = null;

                foreach (UnE.SOP.Tree.SOPTreeNode node in nodes)
                {
                    if (node.TreeNodeType == type)
                    {
                        if (removeNodes == null)
                            removeNodes = new List<TreeNode>();

                        removeNodes.Add(node);
                    }
                    else
                        RemoveNodes(type, node.Nodes);
                }

                if (removeNodes != null)
                {
                    foreach (TreeNode node in removeNodes)
                    {
                        nodes.Remove(node);
                    }
                }
            }
        }

        private UnE.SOP.SOPManager sopManager = null;
        private bool m_isNormal = true;
        private string m_strSOP = "";

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }

        public string TargetSOP
        {
            get { return m_strSOP; }
        }

        public PopupSelectSOP()
        {
            InitializeComponent();

            InitTree();
        }

        private void rdoNormal_CheckedChanged(object sender, EventArgs e)
        {
            SetNormal();
        }

        private void rdoEmergency_CheckedChanged(object sender, EventArgs e)
        {
            SetNormal();
        }

        private void SetNormal()
        {
            if (rdoNormal.Checked)
            {
                lblSenario.Text = "평일 시나리오";
                m_isNormal = true;
            }
            else
            {
                lblSenario.Text = "야간 및 휴일 시나리오";
                m_isNormal = false;
            }
        }

        public void InitTree()
        {
            sopManager = new UnE.SOP.SOPManager(FormMain.Instance.DBManager);
            sopManager.Load(true, m_isNormal);
        }

        private void PopupSelectSOP_Load(object sender, EventArgs e)
        {
            if (m_isNormal)
                rdoNormal.Checked = true;
            else
                rdoEmergency.Checked = true;

            if (treeSOP.Load(sopManager, true, m_isNormal))
                treeSOP.RemoveActionStepNodes();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            UnE.SOP.Tree.SOPTreeNode node = (UnE.SOP.Tree.SOPTreeNode)treeSOP.SelectedNode;
            if (node != null)
            {
                UnE.SOP.Tree.SOPTreeNode targetNode = null;
                string strActionStepName = string.Empty;

                if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.ACTIONSTEP_NODE)
                {
                    targetNode = (UnE.SOP.Tree.SOPTreeNode)node.Parent;
                    strActionStepName = node.Text;
                }
                else if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.CATEGORY_NODE)
                {
                    // 하위 선택하도록 팝업
                    MessageBox.Show("SOP 시나리오를 선택하세요.");
                    return;
                }
                else if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.SUBCATEGOY_NODE)
                {
                    // 하위 선택하도록 팝업
                    MessageBox.Show("SOP 시나리오를 선택하세요.");
                    return;
                }
                else if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.DISASTER_NODE)
                {
                    targetNode = node;
                }

                m_strSOP = targetNode.FullPath.Replace(@"\", "/");

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
