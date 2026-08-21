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
using System.Text.RegularExpressions;
using UnE.Controls;

namespace UnE.SenarioMaker
{
    interface IUseFormTreeStep
    {
        void SetTreeView(string strCategroyName, string strSubCategoryName, string strDisasterName, ArrayList arrStepList);

        
    }

    internal partial class FormTreeStep : Form, IUseFormTreeStep
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

            for (int i = 1; ; i++)
            {
                string strDisasterType = SenarioManager.ToDisasterType(i);

                if (strDisasterType.Length == 0)
                    break;

                ToolStripMenuItem menu = new ToolStripMenuItem(strDisasterType);
                menu.Click += new System.EventHandler(this.typeToolStripMenuItem_Click);

                typeToolStripMenuItem.DropDownItems.Add(menu);

                if (SenarioManager.Instance.SenarioType == i)
                    menu.Checked = true;
            }
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
            /*CategoryNode categoryNode = null;

            categoryNode = new CategoryNode(strCategroyName);
            categoryNode.CategoryName = strCategroyName;
            mStepTreeView.Nodes.Add(categoryNode);

            SubCategoryNode subCategoryNode = new SubCategoryNode(strSubCategoryName);
            subCategoryNode.SubCategoryName = strSubCategoryName;
            categoryNode.Nodes.Add(subCategoryNode);*/

            DisasterNode disasterNode = new DisasterNode(strDisasterName);
            disasterNode.DisasterName = strDisasterName;
            mStepTreeView.Nodes.Add(disasterNode);
            //subCategoryNode.Nodes.Add(disasterNode);

            ArrayList arrActionSteps = arrStepList;

            foreach (ActionStep step in arrActionSteps)
            {
                ActionStepNode actionStepNode = new ActionStepNode(step.StepName);
                actionStepNode.ActionStep = step;
                actionStepNode.ActionStepName = step.StepName;
                disasterNode.Nodes.Add(actionStepNode);
            }

            mStepTreeView.ExpandAll();
        }

        public void SetActionStepName(string strStepName)
        {
            foreach (TreeNode disaster in mStepTreeView.Nodes)
            {
                foreach (TreeNode actionStep in disaster.Nodes)
                {
                    actionStep.Text = strStepName;
                }

                break;
            }
        }
        
        private void mStepTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown && m_SelectRightClicked == false)
            {                
                e.Cancel = true;
                return;
            }
        }

        private void mStepTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
            {                
                return;
            }

            if (m_SelectRightClicked == true)
            {
                m_SelectRightClicked = false;
                return;
            }
            m_SelectRightClicked = false;

            if(e.Node.GetType() == typeof(CategoryNode))
            {
                if(m_Owner != null)
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
            else if(e.Node.GetType() == typeof(ActionStepNode))
            {
                if (m_Owner != null)
                {
                    m_Owner.OnActionStepNodeSelection((ActionStepNode)e.Node);
                }
            }
        }


        private bool m_SelectRightClicked = false;
        private void mStepTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                m_SelectRightClicked = true;
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
                    ActionStepNode actionStepNode = (ActionStepNode)e.Node;
                    if(actionStepNode.ActionStep.TeamName == "Main")
                    {
                        senarioTreeToolStripMenu.Items.Remove(actionStepToolStripSeparator);
                        senarioTreeToolStripMenu.Items.Remove(deleteActionStepStripMenuItem);
                    }
                    else
                    {
                        senarioTreeToolStripMenu.Items.Add(actionStepToolStripSeparator);
                        senarioTreeToolStripMenu.Items.Add(deleteActionStepStripMenuItem);
                    }
                    /*Point pt = mStepTreeView.PointToScreen(new Point(e.X, e.Y));
                    senarioTreeToolStripMenu.Show(pt);
                    senarioTreeToolStripMenu.Tag = e.Node;*/
                }
            }
           
        }
        
        private void SetCheckMenuItem(ToolStripMenuItem item)
        {
             foreach(ToolStripMenuItem it in typeToolStripMenuItem.DropDownItems)
             {
                 if( it == item)
                 {
                     it.Checked = true;
                 }
                 else
                 {
                     it.Checked = false;
                 }
             }
        }

        private void typeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string szNodeName = ((ToolStripMenuItem)sender).Text;
            DisasterNode node = (DisasterNode)disasterTreeToolStripMenu.Tag;
            if (node != null)
            {

                SetCheckMenuItem((ToolStripMenuItem)sender);

                node.Text = szNodeName;
                node.DisasterName = szNodeName;

                if (m_Owner != null)
                    m_Owner.OnChangeDisasterType(node);
            }
        }

       
        private static int nCount = 1;
        private void disasterTreeToolStripMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {            
            string szNodeName = e.ClickedItem.Text;
            DisasterNode node = (DisasterNode)disasterTreeToolStripMenu.Tag;
            if( node != null )
            {
                if(e.ClickedItem.Text == "새 함수 추가")
                {
                    UndoRedoManager.Instance.SaveSnapshot("새함수추가");

                    ActionStep actionStep = SenarioManager.Instance.AddActionStep("새함수" + nCount.ToString());
                    ActionStepNode actionStepNode = new ActionStepNode(actionStep.StepName);
                    actionStepNode.ActionStep = actionStep;
                    actionStepNode.ActionStepName = actionStep.StepName;
                    node.Nodes.Add(actionStepNode);
                    nCount++;
                    mStepTreeView.ExpandAll();
                }
            }
        }


        private void senarioTreeToolStripMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            ActionStepNode node = (ActionStepNode)senarioTreeToolStripMenu.Tag;
            if (node == null)
                return;      
      
            if(e.ClickedItem.Text == "이름 바꾸기")
            {
                node.EditMode = true;
                node.BeginEdit();
            }
            else if(e.ClickedItem.Text == "삭제") 
            {
                UndoRedoManager.Instance.SaveSnapshot("함수 삭제");

                SenarioManager.Instance.RemoveActionStep(node.ActionStep);
                node.Parent.Nodes.Remove(node);
            }
        }

        private void mStepTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_SelectRightClicked = true;
                mStepTreeView.SelectedNode = e.Node;
                m_SelectRightClicked = false;
                if (e.Node.GetType() == typeof(CategoryNode))
                {

                }
                else if (e.Node.GetType() == typeof(SubCategoryNode))
                {

                }
                else if (e.Node.GetType() == typeof(DisasterNode))
                {
                    
                }
                else if (e.Node.GetType() == typeof(ActionStepNode))
                {
                    if (m_Owner != null)
                        m_Owner.OnActionStepNodeDoubleClicked((ActionStepNode)e.Node);
                }
            }
        }

        private void disasterTreeToolStripMenu_Opening(object sender, CancelEventArgs e)
        {

        }

        private void mStepTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if( e.Node.GetType() != typeof(ActionStepNode))
            {
                e.CancelEdit = true;
                return;
            }         
        }

		private bool CheckSenarioName(string szName)
		{

			if( szName.IndexOf(" ") != -1)
			{
				UnE.Utility.UMessageBox.Show("시나리오와 함수 이름은 공백이 포함될 수 없습니다.", "이름변경 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			//Regex r = new Regex("^[a-zA-Z0-9]*$");
			//if (!r.IsMatch(szName))
			//{
			//	UnE.Utility.UMessageBox.Show("시나리오와 함수 이름은 숫자와 알파벳 포함될 수 없습니다.", "이름변경 오류");
			//	return false;
			//}

			if(SenarioManager.Instance.IsExistActionStepName(szName))
			{
				UnE.Utility.UMessageBox.Show("다른 함수와 이름이 중복됩니다.", "이름변경 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			
			

			return true;
		}

        private void AfterLabelChagned(TreeNode changeNode, string newName)
        {

            ActionStepNode node = (ActionStepNode)changeNode;
            node.EditMode = false;

            string szOrgName = node.ActionStepName;
            string szNewName = newName;
            if (szNewName == null || szNewName == "")
            {
                node.Text = szOrgName;
                return;
            }
            


            if (szOrgName != szNewName)            
            {
				if (CheckSenarioName(szNewName))
				{
					UndoRedoManager.Instance.SaveSnapshot("시나리오 이름 변경");

					ActionStep step = node.ActionStep;
					step.StepName = szNewName;
					node.ActionStepName = step.StepName;
					node.Text = szNewName;
				} 
              	else
				{					
					node.Text = szOrgName;
				}
            }
        }

        private void mStepTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // 주의 : Invoke에서 수행해야 노드 Text가 변경된 이후임
            this.BeginInvoke(new Action(() => AfterLabelChagned(e.Node, e.Label)));
        }
    }

    public interface ISOPTreeNodeSelection
    {
        void OnCategoryNodeSelection(CategoryNode node);
        void OnSubCategoryNodeSelection(SubCategoryNode node);
        void OnDisasterNodeSelection(DisasterNode node);
       
        void OnActionStepNodeSelection(ActionStepNode node);

        void OnChangeDisasterType(DisasterNode node);
        void OnActionStepNodeDoubleClicked(ActionStepNode node);
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

    public class ActionStepNode : UnE.Controls.SOPNode
    {
        public ActionStepNode(string strNodeName)
            : base(strNodeName)
        {
            Text = strNodeName;
            //m_szTypeName = " (Main)";
        }

        public override string TypeText
        {
            get { return m_szTypeName; }
            set { m_szTypeName = value; }
        }

        private ActionStep m_actionStep = null;
        internal ActionStep ActionStep
        {
            get { return m_actionStep; }
            set 
            {
                m_actionStep = value;
                if (m_actionStep.TeamName == "Main")
                {
                    //m_szTypeName = " (Main)";
                    
                }
                else
                {
                    m_szTypeName = " (Sub)";
                }               
            }
        }

        private string strActionStepName = "";
        public string ActionStepName
        {
            get { return strActionStepName; }
            set { strActionStepName = value; }
        }

        private bool m_bEditMode = false;
        public bool EditMode
        {
            get
            {
                return m_bEditMode;
            }
            set
            {
                m_bEditMode = value;               
            }
        }
    }
}
