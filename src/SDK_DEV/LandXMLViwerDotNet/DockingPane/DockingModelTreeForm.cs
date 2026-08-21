using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
namespace UBMLViewer
{

    public partial class DockingModelTreeForm : Form
    {
        public System.Windows.Forms.TreeView ModelTree
        {
            get { return m_TreeView; }
        }

        public System.Windows.Forms.ImageList CheckImageList
        {
            get { return mImageList; }
        }

        public DockingNodePropertiesForm DockingNodePropertiesForm
        {
            get { return PageBackstageHome.Instance.DockingNodePropertiesForm; }
        }

        public DockingModelTreeForm()
        {
            InitializeComponent();

            CreateModelTree();

            AddPythonFunction();
        }

        public void AddPythonFunction()
        {
            ScriptProxy proxy = ScriptProxy.Instance;
            proxy.UserObject.SelectNode = new Func<string, bool>(ModelViewSelectNode);
            proxy.UserObject.ZoomNode = new Func<string, bool>(ModeViewZoomInNode);
        }

        private void CreateModelTree()
        {
            Bitmap b1 = FormMain.GetImageByName("check");
            Bitmap b2 = FormMain.GetImageByName("uncheck");

            CheckImageList.Images.Add((Image)b2);
            CheckImageList.Images.Add((Image)b1);

            ModelTree.StateImageList = CheckImageList;
            ModelTree.CheckBoxes = true;
            ModelTree.ShowLines = false;
            ModelTree.HideSelection = false;
        }

        private void FindModelRootNode(ArrayList arChild)
        {
            foreach (Core.Scene scene in arChild)
            {
                if (scene.Parent == null)
                {
                    TreeNode node = new TreeNode("Model");
                    node.Checked = true;
                    node.Tag = scene;
                    ModelTree.Nodes.Add(node);
                    arChild.Remove(scene);
                    break;
                }
            }
        }

        private void MakeModelTreeNode(TreeNode parent, ArrayList arChild)
        {
            if (arChild.Count == 0)
                return;

            Core.Scene pScene = (Core.Scene)parent.Tag;
            foreach (Core.Scene scene in arChild)
            {
                if (scene.Parent == pScene)
                {
                    TreeNode node = new TreeNode("Node : " + scene.AliasName);
                    node.Name = scene.AliasName;
                    node.Tag = scene;
                    node.Checked = true;
                    parent.Nodes.Add(node);
                    node.StateImageIndex = 0;
                    ArrayList arTempChild = (ArrayList)arChild.Clone();
                    arTempChild.Remove(scene);
                    MakeModelTreeNode(node, arTempChild);
                }
            }
        }

        public void UpdateModelTree(Core.SceneManager scManager)
        {
            scManager.UpdateData();
            ModelTree.Nodes.Clear();
            ArrayList arChild = (ArrayList)scManager.Childs.Clone();
            FindModelRootNode(arChild);
            TreeNode root = ModelTree.Nodes[0];
            MakeModelTreeNode(root, arChild);
            root.ExpandAll();
        }

        public bool ClearSelect()
        {
            ModelTree.SelectedNode = null;
            DockingNodePropertiesForm.SetSceneData(null);
            return true;
        }

        private TreeNode FindNode(TreeNode parent, string szNodeName)
        {
            TreeNode selectedNode = null;
            foreach (TreeNode cnode in parent.Nodes)
            {
                if (cnode.Name == szNodeName)
                {
                    selectedNode = cnode;
                    return selectedNode;
                }
                selectedNode = FindNode(cnode , szNodeName);
                if (selectedNode != null)
                    return selectedNode;
            }
            return null;
        }       

        public bool ModelViewSelectNode(string szNodeName)
        {
            TreeNode selectedNode = null;
            selectedNode = FindNode(ModelTree.Nodes[0], szNodeName);

            if (selectedNode != null)
            {  
                Core.Scene scene = (Core.Scene)selectedNode.Tag;
                if (scene != null && scene.Visible == true)
                {
                    scene.ShowBoundingBox(true);                    
                }
                ModelTree.Select();
                ModelTree.SelectedNode = selectedNode;
                DockingNodePropertiesForm.SetSceneData(selectedNode);                
                FormMain.Update3DView();
                return true;
            }
            return false;
        }

        public bool ModeViewZoomInNode(string szNodeName)
        {
            TreeNode selectedNode = null;
            selectedNode = FindNode(ModelTree.Nodes[0], szNodeName);
            if (selectedNode != null)
            {
                Core.Scene scene = (Core.Scene)selectedNode.Tag;
                if (scene != null && scene.Visible == true)
                {
                    scene.ShowBoundingBox(true);
                    scene.Zoom(false);
                    DockingNodePropertiesForm.SetSceneData(selectedNode);
                    FormMain.Update3DView();
                    return true;
                }
            }
            return false;            
        }

        private void ModelTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {            
            TreeNode node = ModelTree.SelectedNode;
            if (node != null)
            {
                Core.Scene scene = (Core.Scene)node.Tag;
                if (scene != null)
                {
                    bool bShow = scene.ShowBound;
                    if (bShow == true)
                    {
                        scene.ShowBoundingBox(false);
                        FormMain.Update3DView();
                    }
                }
            }
        }

        private void ModelTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {               
                TreeNode node = ModelTree.SelectedNode;
                if (node != null)
                {
                    DockingNodePropertiesForm.SetSceneData(node);
                }
            }
        }

        private void ModelTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            Core.Scene scene = (Core.Scene)e.Node.Tag;
            if (scene != null && scene.Visible == true)
            {  
                scene.ShowBoundingBox(true);
                scene.Zoom(false);
                DockingNodePropertiesForm.SetSceneData(e.Node);
                FormMain.Update3DView();
            }
        }

        private void ModelTreeView_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
        }
        
        private void OnCheckChange(TreeNode node, bool bCheck)
        {
            foreach (TreeNode cnode in node.Nodes)
            {
                cnode.Checked = bCheck;
                Core.Scene scene = (Core.Scene)cnode.Tag;
                if (scene != null)
                    scene.Visible = bCheck;

                OnCheckChange(cnode, bCheck);
            }
        }

        private void ModelTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.Unknown)
            {
                if (ModelTree.SelectedNode != e.Node)
                    ModelTree.SelectedNode = e.Node;

                Core.Scene scene = (Core.Scene)e.Node.Tag;
                if (scene != null)
                {
                    bool bCheck = e.Node.Checked;
                    OnCheckChange(e.Node, bCheck);
                    scene.Visible = bCheck;

                    DockingNodePropertiesForm.SetSceneData(e.Node);
                    FormMain.Update3DView();
                }
                ModelTree.Update();
            }            
        }

        private void ModelTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
        }      

        //public 
    }
   
}
