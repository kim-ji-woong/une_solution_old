using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPManager
{
    public partial class BarLevelTree : Form
    {
        public BarLevelTree()
        {
            InitializeComponent();
        }
        
        public void ClearTree()
        {
            treeView.Nodes.Clear();
        }

		public bool CheckPathChildNode(TreeNode currentNode, string szTargetValue)
		{
			// 이미 자식 path에 포함되어 있는지 검사
			TreeNode node = FindNode(szTargetValue, currentNode.Nodes);
			if (node != null)
				return false;

			return true;
			// 부모노드에 포함되어 있는지 검사
			//return CheckPathParent(currentNode, szTargetValue);
		}

		private bool CheckPathParent(TreeNode currentNode, string szValue)
		{
			TreeNode parentNode = currentNode.Parent;
			if (parentNode.Parent == null || parentNode.Level <= 3)
				return true;

			if (parentNode.Text == szValue)
				return false;

			return CheckPathParent(parentNode, szValue);
		}


		public bool SetChildNode(TreeNode parentNode, TreeNode childNode)
		{
			if( parentNode == null || childNode == null)
				return false;

			if (!CheckPathChildNode(childNode, parentNode.Text))
				return false;

			

			TreeNode node = null;
			int nStep = 0;
			try
			{
				node = childNode.Parent;
				if( node != null)
				{
					node.Nodes.Remove(childNode);
				}
				nStep = 1;

				if(!parentNode.Nodes.Contains(childNode))
				{
					parentNode.Nodes.Add(childNode);
				}
				nStep = 2;
			}
			catch (System.Exception)
			{
			
			}
			if (nStep == 2)
			{
				parentNode.ExpandAll();
				return true;
			}

			if( nStep == 1)
			{
				childNode = node;
			}
			return false;			
		}

        // Return 값 : strLevelName에 해당하는 노드를 리턴
        public TreeNode AddTreeNode(string strCategory = null, string strSubCategory = null, string strDetailCategory = null, string strLevelName = null)
        {
            if (strCategory == null)
                strCategory = FormMain.Instance.GetPageDisaster().SelectedCategory;
            if (strSubCategory == null)
                strSubCategory = FormMain.Instance.GetPageDisaster().SelectedSubCategory;
            if (strDetailCategory == null)
                strDetailCategory = FormMain.Instance.GetPageDisaster().SelectedDetailCategory;
            if (strLevelName == null)
                strLevelName = FormMain.Instance.GetPageLevel().GetTabPageName();

            if (strCategory == "" || strSubCategory == "" || strDetailCategory == "")
                return null;

            TreeNode child = FindNode(strCategory, treeView.Nodes);
            if (child == null)
                child = treeView.Nodes.Add(strCategory);

            TreeNode second = FindNode(strSubCategory, child.Nodes);
            if (second == null)
                second = child.Nodes.Add(strSubCategory);

            TreeNode detail = FindNode(strDetailCategory, second.Nodes);
            if (detail == null)
                detail = second.Nodes.Add(strDetailCategory);

			if (strLevelName == "")
			{
				treeView.ExpandAll();
				return null;
			}
				

            TreeNode level = FindNode(strLevelName, detail.Nodes);
            if (level == null)
                level = detail.Nodes.Add(strLevelName);

            treeView.ExpandAll();

            SelectNode(level);

            ArrayList arList = FormMain.Instance.GetPageLevel().GetTabPage();
            foreach (TabPage page in arList)
            {
                if (page.Text == level.Text)
                {
                    TabControl control = (TabControl)page.Parent;
					if (control != null)
					{
						control.SelectedTab = null;
						control.SelectedTab = page;
					}                    
                }
            }

            return level;
        }

        public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeView.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if (strValue == node.Text)
                    return node;
                TreeNode result = FindNode(strValue, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        public void RemoveTreeNode(string strValue)
        {
            TreeNode node = FindNode(strValue, treeView.Nodes);
            if (node != null)
            {
                TreeNode node2 = FindNode(strValue, node.Nodes);
                if (node2 == null)
                    node.Nodes.Remove(node);
            }
        }

        public void ChangeLevelText(string strValue)
        {
            TreeNode node = FindNode(FormMain.Instance.GetPageLevel().OldTabPageText);
            if (node != null)
                node.Text = strValue;
        }
       
        public void SelectNode(TreeNode node)
        {  
			if (node == null)
				return;

            if (treeView.SelectedNode != null)
                treeView.SelectedNode.ForeColor = Color.Black;
            
            treeView.SelectedNode = node;
            node.ForeColor = Color.Red;            
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
			if( e.Action != TreeViewAction.Unknown)            
            {
                TreeNode node = treeView.SelectedNode;
                if (node == null) return;

                node.ForeColor = Color.Red;

                ArrayList arList = FormMain.Instance.GetPageLevel().GetTabPage();
                foreach (TabPage page in arList)
                {
                    if (page.Text == node.Text)
                    {
                        TabControl control = (TabControl)page.Parent;
                        control.SelectedTab = page;
                    }
                }
            }            
        }

        private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = treeView.SelectedNode;
            if (node == null)
				return;
            node.ForeColor = Color.Black;
        }

		public bool ExistNode()
		{
			if (treeView.Nodes.Count > 0)
				return true;
			return false;
		}
	}
}
