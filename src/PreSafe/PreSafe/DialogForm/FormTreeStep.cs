using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreSafe
{
    internal partial class FormTreeStep : Form
    {
        ISOPTreeNodeSelection m_Owner = null;

        public ISOPTreeNodeSelection SOPTreeNodeSelectionOwner
        {
            get { return m_Owner; }
            set { m_Owner = value; }
        }

        public FormTreeStep()
        {
            InitializeComponent();
            this.TopLevel = false;
        }

        public bool SetEnabled
        {
            get
            {
                return this.mStepTreeView.Enabled;
            }
            set
            {
                mStepTreeView.Enabled = value;
            }
        }
        
        //TreeView Setting
        public void SetTreeView(string strCategroyName, string strSubCategoryName, string strDisasterName, ArrayList arrStepList)
        {
            mStepTreeView.Nodes.Clear();
            CategoryNode CategoryNode = null;

            CategoryNode = new CategoryNode(strCategroyName);
            mStepTreeView.Nodes.Add(CategoryNode);

            SubCategoryNode SubCategoryNode = new SubCategoryNode(strSubCategoryName);
            CategoryNode.Nodes.Add(SubCategoryNode);

            DisasterNode DisasterNode = new DisasterNode(strDisasterName);
            SubCategoryNode.Nodes.Add(DisasterNode);

            ArrayList arrActionSteps = arrStepList;

            foreach (XMLManager.ActionStep step in arrActionSteps)
            {
                ActionStepNode ActionStepNode = new ActionStepNode(step.StepName);
                ActionStepNode.ActionStep = step;
                DisasterNode.Nodes.Add(ActionStepNode);
            }

            mStepTreeView.ExpandAll();
        }

        private void mStepTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
            {
                e.Cancel = true;
                return;
            }

            Console.WriteLine("aaa");
        }

        private void mStepTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
            {
                return;
            }

            if (e.Node.GetType() == typeof(CategoryNode))
            {
                if (m_Owner != null)
                {
                    m_Owner.OnCategoryNodeSelection((CategoryNode)e.Node);
                }
            }
            else if (e.Node.GetType() == typeof(SubCategoryNode))
            {
                if (m_Owner != null)
                {
                    m_Owner.OnSubCategoryNodeSelection((SubCategoryNode)e.Node);
                }
            }
            else if (e.Node.GetType() == typeof(DisasterNode))
            {
                if (m_Owner != null)
                {
                    m_Owner.OnDisasterNodeSelection((DisasterNode)e.Node);
                }
            }
            else if (e.Node.GetType() == typeof(ActionStepNode))
            {
                if (m_Owner != null)
                {
                    m_Owner.OnActionStepNodeSelection((ActionStepNode)e.Node);
                }
            }
        }

        private void mStepTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                mStepTreeView.SelectedNode = e.Node;

                if (e.Node.GetType() == typeof(CategoryNode))
                {

                }
                else if (e.Node.GetType() == typeof(SubCategoryNode))
                {

                }
                else if (e.Node.GetType() == typeof(DisasterNode))
                {
                    Point pt = mStepTreeView.PointToScreen(new Point(e.X, e.Y));
                    this.disasterTreeToolStripMenu.Show(pt);
                    disasterTreeToolStripMenu.Tag = e.Node;
                }
                else if (e.Node.GetType() == typeof(ActionStepNode))
                {
                    Point pt = mStepTreeView.PointToScreen(new Point(e.X, e.Y));
                    senarioTreeToolStripMenu.Show(pt);
                    senarioTreeToolStripMenu.Tag = e.Node;
                }
            }

        }

        private void disasterTreeToolStripMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string szNodeName = e.ClickedItem.Text;
            DisasterNode node = (DisasterNode)disasterTreeToolStripMenu.Tag;
            if (node != null)
            {

                node.Text = szNodeName;
                node.DisasterName = szNodeName;

                if (e.ClickedItem == toolStripMenuItem3)
                {
                    toolStripMenuItem3.Checked = true;
                    toolStripMenuItem4.Checked = false;
                    toolStripMenuItem5.Checked = false;
                }
                else if (e.ClickedItem == toolStripMenuItem4)
                {
                    toolStripMenuItem3.Checked = false;
                    toolStripMenuItem4.Checked = true;
                    toolStripMenuItem5.Checked = false;
                }
                else if (e.ClickedItem == toolStripMenuItem5)
                {
                    toolStripMenuItem3.Checked = false;
                    toolStripMenuItem4.Checked = false;
                    toolStripMenuItem5.Checked = true;
                }

            }
        }

        private void toolStripMenuItem3_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    public interface ISOPTreeNodeSelection
    {
        void OnCategoryNodeSelection(CategoryNode node);
        void OnSubCategoryNodeSelection(SubCategoryNode node);
        void OnDisasterNodeSelection(DisasterNode node);
        void OnActionStepNodeSelection(ActionStepNode node);

    }

    public class CategoryNode : TreeNode
    {
        public CategoryNode(string strNodeName)
            : base(strNodeName)
        {
        }

        private string strCategoryName = "";
        public string CategoryName
        {
            get { return strCategoryName; }
            set { strCategoryName = value; }
        }
    }

    public class SubCategoryNode : TreeNode
    {
        public SubCategoryNode(string strNodeName)
            : base(strNodeName)
        {
        }

        private string strSubCategoryName = "";
        public string SubCategoryName
        {
            get { return strSubCategoryName; }
            set { strSubCategoryName = value; }
        }
    }

    public class DisasterNode : TreeNode
    {
        public DisasterNode(string strNodeName)
            : base(strNodeName)
        {
        }

        private string strDisasterName = "";
        public string DisasterName
        {
            get { return strDisasterName; }
            set { strDisasterName = value; }
        }
    }

    public class ActionStepNode : TreeNode
    {
        public ActionStepNode(string strNodeName)
            : base(strNodeName)
        {
        }
        private XMLManager.ActionStep m_actionStep = null;
        internal XMLManager.ActionStep ActionStep
        {
            get { return m_actionStep; }
            set { m_actionStep = value; }
        }

        private string strActionStepName = "";
        public string ActionStepName
        {
            get { return strActionStepName; }
            set { strActionStepName = value; }
        }
    }
}
