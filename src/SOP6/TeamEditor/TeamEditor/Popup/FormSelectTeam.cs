using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor.Popup
{
    public partial class FormSelectTeam : Form
    {
        private object m_selectedTeam = null;

        public string Title
        {
            get { return labelTitle.Text; }
            set { labelTitle.Text = value; }
        }

        public TreeView Tree
        {
            get { return treeView1; }
            set { SetTree(value); }
        }

        public object SelectedTeam
        {
            get { return m_selectedTeam; }
        }

        private Team _focusedTeam { get; set; }
        public FormSelectTeam()
        {
            InitializeComponent();

            labelTitle.Text = "";
            labelSelectedTeam.Text = "";
        }

        public FormSelectTeam(string strTitle, TreeView tree, Team focusedTeam = null)
        {
            InitializeComponent();

            labelSelectedTeam.Text = "";
            Title = strTitle;
            this._focusedTeam = focusedTeam;
            Tree = tree;

            treeView1.Nodes[0].EnsureVisible();
            treeView1.Nodes[0].TreeView.Focus();
        }

        private void SetTree(TreeView tree)
        {
            treeView1.Nodes.Clear();

            if (tree == null)
                return;

            SetTreeNodes(treeView1.Nodes, tree.Nodes);
        }
         
        private void SetTreeNodes(TreeNodeCollection nodesTrg, TreeNodeCollection nodesSrc)
        {
            foreach (TreeNode node in nodesSrc)
            {
                TreeNode _node = nodesTrg.Add(node.Text);
                _node.Tag = node.Tag;

                Team team = (Team)_node.Tag;
                if (_focusedTeam != null && team != null && team.TeamID == this._focusedTeam.TeamID)
                {
                    treeView1.SelectedNode = _node;
                    
                }
                 
                SetTreeNodes(_node.Nodes, node.Nodes); 
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string strTeamPath = string.Empty;

            if (e.Node != null)
            {
                GetTeamPath(e.Node, ref strTeamPath);
            }

            labelSelectedTeam.Text = strTeamPath;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
                m_selectedTeam = null;
            else
                m_selectedTeam = treeView1.SelectedNode.Tag;

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        private void FormSelectTeam_Load(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void GetTeamPath(TreeNode node, ref string strTeamPath)
        {
            if (node == null) return;

            if (node.Parent != null)
            {
                GetTeamPath(node.Parent, ref strTeamPath);
            }

            if (strTeamPath.Length != 0)
            {
                strTeamPath += " > ";
            }

            strTeamPath += node.Text;
        } 
    }
}
