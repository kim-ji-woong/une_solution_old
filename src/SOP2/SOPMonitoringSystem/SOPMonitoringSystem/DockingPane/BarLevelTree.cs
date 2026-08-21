using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Sections;

namespace SOPMonitoringSystem
{
    public partial class BarLevelTree : Form, IFormDisasterOwner
    {
        private TreeNode m_prevSelectedNode = null;
        private int m_nPrevSelectedDisasterID = -1;
        
        public int PrevSelectedDisasterID
        {
            get { return m_nPrevSelectedDisasterID; }
            set 
            {
                m_nPrevSelectedDisasterID = value;
               
            }
        }

        public void ResetSelect()
        {
            m_nPrevSelectedDisasterID = -1;
            m_prevSelectedNode = null;

        }

        // 등록 모드인가?
        private bool m_isRegular = true;
        // 평일 모드인가?
        private bool m_isNormal = true;

        private bool m_ignoreSelect = false;
        private bool m_ignoreLoadSOP = false;

        // 재난 Tree의 Node별 구분자
        private char m_chDelimeter = (char)6;
        private string m_strDelimeter = "";

        private FormDisaster m_frmDisaster = null;
        private bool m_isTreeMode = false;

        public System.Windows.Forms.TreeView TreeView
        {
            get { return treeView; }
            set { treeView = value; }
        }

        public BarLevelTree()
        {
            InitializeComponent();

            m_strDelimeter = m_chDelimeter.ToString();

            if (m_isTreeMode)
                treeView.Dock = DockStyle.Fill;
            else
            {
                //treeView.Visible = false;
                treeView.Dock = DockStyle.None;
                treeView.Size = new System.Drawing.Size(10, 10);
                treeView.Location = new Point(-100, 0);

                m_frmDisaster = new FormDisaster();
                m_frmDisaster.FormOwner = this;

                this.Controls.Add(m_frmDisaster);
                m_frmDisaster.Show();
            }
        }        
        
        public void ClearTree()
        {
            treeView.Nodes.Clear();
        }

        // Return 값 : strLevelName에 해당하는 노드를 리턴
        public TreeNode AddTreeNode(string strCategory = null, string strSubCategory = null, string strDetailCategory = null, string strLevelName = null)
        {
            //if (strCategory == null)
            //    strCategory = FormMain.Instance.GetPageDisaster().SelectedCategory;
            //if (strSubCategory == null)
            //    strSubCategory = FormMain.Instance.GetPageDisaster().SelectedSubCategory;
            //if (strDetailCategory == null)
            //    strDetailCategory = FormMain.Instance.GetPageDisaster().SelectedDetailCategory;
            //if (strLevelName == null)
            //    strLevelName = FormMain.Instance.GetPageLevel().GetTabPageName();
            
            TreeNode child = FindNode(strCategory, treeView.Nodes);
            if (child == null)
                child = treeView.Nodes.Add(strCategory);

            TreeNode second = FindNode(strSubCategory, child.Nodes);
            if (second == null)
                second = child.Nodes.Add(strSubCategory);

            TreeNode detail = FindNode(strDetailCategory, second.Nodes);
            if (detail == null)
                detail = second.Nodes.Add(strDetailCategory);

            TreeNode level = FindNode(strLevelName, detail.Nodes);
            if (level == null)
                level = detail.Nodes.Add(strLevelName);

            treeView.ExpandAll();

            SelectNode(level);
            
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
            //TreeNode node = FindNode(FormMain.Instance.GetPageLevel().OldTabPageText);
            //node.Text = strValue;
        }

        private int GetActionStepID(TreeNode treeNode)
        {
            if (treeNode == null)
                return -1;
            if (treeNode.Nodes.Count == 0)
                return -1;

            foreach (TreeNode child in treeNode.Nodes)
            {
                if (child.Tag != null)
                {
                    return ((int)child.Tag);
                }
            }
            return -1;
        }

        public void SelectNode(TreeNode node)
        {
            if (node == null)
                return;

            m_ignoreSelect = true;

            if (treeView.SelectedNode != null)
                treeView.SelectedNode.ForeColor = Color.Black;
            
            treeView.SelectedNode = node;
            node.ForeColor = Color.Red;

            TreeNode view = new TreeNode();
            view = treeView.SelectedNode;

            if (view == null)
                return;

            while(view.Parent != null)
            {
                view = view.Parent;
            }
            
            //MessageBox.Show(view.FullPath);
            FormMain.Instance.toolstripSetting(view.FullPath.ToString());

            if (node.Level > 2)
            {
                // node.Level이 2일 경우 Tag는 ActionStep ID가 아닌 Disaster ID
                FormMain.Instance.GetPageHome().SetScenarioName((int)node.Tag);
            }
        }
		char szDeli = (char)0x06;
        public void SelectSop(TreeNode selnode)
        {
            if (selnode == null)
                selnode = treeView.SelectedNode;
            TreeNode node = selnode;
            if (node == null)
                return;
			string strPath = node.FullPath.Replace("\\", szDeli.ToString());
            FormMain.Instance.GetPageHome().GetDockPropertiesLevel().AddTitle(strPath);

            node.ForeColor = Color.Red;
            m_prevSelectedNode = node;

            if (node.Level <= 1)
                return;

            TreeNode nodeDisaster = GetDisasterNode(node);
            if (nodeDisaster == null)
                return;

            if (node == nodeDisaster)   // Disaster 노드를 선택한 경우
            {
                if ((int)nodeDisaster.Tag == m_nPrevSelectedDisasterID)
                    return;
                LoadSOP(nodeDisaster);

            }
            else                        // 대응 단계 노드를 선택한 경우
            {


                int nActionStepID = (int)(node.Tag);
                if ((int)nodeDisaster.Tag == m_nPrevSelectedDisasterID)
                {
                    if (FormMain.Instance.GetPageHome().IsChangeCurrentTab())
                    {
                        LoadSOP(nodeDisaster);
                    }
                    // 단계 탭 전환
                    ChangeTab(nActionStepID);
                }
                else
                {
                    //if (FormMain.Instance.GetPageHome().IsChangeCurrentTab())
                    //{
                        LoadSOP(nodeDisaster);
                    //}
                    // SOP 다시 읽기
                    //LoadSOP(nodeDisaster);
                    // 단계 탭 전환
                    ChangeTab(nActionStepID);
                }
            }
            m_nPrevSelectedDisasterID = (int)nodeDisaster.Tag;
            FormMain.Instance.ChangeWorkflow();

            // 새로운 SOP가 실행되었으므로 기존의 ComponentContents는 모두 지운다.
            FormMain.Instance.GetPageHome().ClearProcess();
            
            FormMain.Instance.EnabledRunGroup();

            m_frmDisaster.LoadSOP(nodeDisaster);
        }

        public void OnTreeViewClicked(TreeNode node, bool noSelect)
        {
            if (node == null)
                return;

            if (treeView.SelectedNode == node || treeView.SelectedNode == null)
            {
                if (treeView.SelectedNode == null)
                {
                    IgnoreSelect = true;
                    treeView.SelectedNode = node;
                    IgnoreSelect = false;
                }
                SelectSop(null);

                /*TreeNode view = new TreeNode();
                view = treeView.SelectedNode;

                while(view.Parent != null)
                {
                    view = view.Parent;
                }*/

                //MessageBox.Show(view.FullPath);
                //FormMain.Instance.toolstripSetting(view.FullPath.ToString());
            }
            else
            {
                if (!noSelect)
                    treeView.SelectedNode = node;
            }
        }

        public void EnableButton(bool isPrevButton, bool enabled)
        {
            if (isPrevButton)
                btnPrev.Enabled = enabled;
            else
                btnNext.Enabled = enabled;
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            OnTreeViewClicked((TreeNode)e.Node, true);
        }

        private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = treeView.SelectedNode;
            if (node == null) return;

            node.ForeColor = Color.Black;

            foreach (SectionTabPage page in FormMain.Instance.GetPageHome().TabControls.Controls)
            {
                FormMain.Instance.GetPageHome().changeLocation(page.Height);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
//             if (m_ignoreSelect)
//             {
//                 m_ignoreSelect = false;
//                 return;
//             }

            TreeNode node = treeView.SelectedNode;
            if (node == null)
                return;

            string strPath = node.FullPath.Replace("\\", szDeli.ToString());
            FormMain.Instance.GetPageHome().GetDockPropertiesLevel().AddTitle(strPath);

            node.ForeColor = Color.Red;

            if (m_prevSelectedNode == node)
                return;

            m_prevSelectedNode = node;

            if (node.Level <= 1)
                return;

            TreeNode nodeDisaster = GetDisasterNode(node);
            if (nodeDisaster == null)
                return;

            if (node == nodeDisaster)   // Disaster 노드를 선택한 경우
            {
                if ((int)nodeDisaster.Tag == m_nPrevSelectedDisasterID)
                    return;

                LoadSOP(nodeDisaster);
            }
            else                        // 대응 단계 노드를 선택한 경우
            {
                int nActionStepID = (int)node.Tag;
                if ((int)nodeDisaster.Tag == m_nPrevSelectedDisasterID)
                {
                    if (FormMain.Instance.GetPageHome().IsChangeCurrentTab())
                    {
                        LoadSOP(nodeDisaster);
                    }

                    // 단계 탭 전환
                    ChangeTab(nActionStepID);
                }
                else
                {
                    // SOP 다시 읽기
                    LoadSOP(nodeDisaster);
                    // 단계 탭 전환
                    ChangeTab(nActionStepID);
                }
            }

            m_nPrevSelectedDisasterID = (int)nodeDisaster.Tag;
            //FormMain.Instance.GetPageHome().WatermarkImage();
            //FormMain.Instance.ChangeWorkflow();
            FormMain.Instance.EnabledRunGroup();
            FormMain.Instance.GetPageHome().panel.Visible = true;
            FormMain.Instance.GetPageHome().SetBackgroundImage(true);

            TreeNode view = new TreeNode();
            view = treeView.SelectedNode;

            while (view.Parent != null)
            {
                view = view.Parent;
            }

            //MessageBox.Show(view.FullPath);
            FormMain.Instance.GetPageHome().toolstripSetting(view.FullPath.ToString());

            foreach (SectionTabPage page in FormMain.Instance.GetPageHome().TabControls.Controls)
            {
                FormMain.Instance.GetPageHome().changeLocation(page.Height);
            }
        }

        public TreeNode GetCurrentDisasterNode()
        {
            if (m_nPrevSelectedDisasterID < 0)
                return null;

            return FindNode(m_nPrevSelectedDisasterID);
        }

        public void ChangeTab(int nActionStepID)
        {
            FormMain frm = FormMain.Instance;
            PageBackstageHome pageHome = frm.GetPageHome();
            ArrayList arrTabPages = pageHome.GetTabPage();

            foreach (Sections.SectionTabPage page in arrTabPages)
            {
                if (page.ActionStepID == nActionStepID)
                {
                    pageHome.SelectTab(page);
                    pageHome.GetDockPersonnel().GetSOPTotalResource();
                    return;
                }
            }
        }

        private bool LoadSOP(TreeNode nodeDisaster)
        {
            if (IgnoreLoadSOP)
                return true;

            string strCategoryName = nodeDisaster.Parent.Parent.Text;
            string strSubCategoryName = nodeDisaster.Parent.Text;

            int nDisasterID = (int)nodeDisaster.Tag;
            string strFullPath = strCategoryName + m_strDelimeter + strSubCategoryName + m_strDelimeter + nodeDisaster.Text;

            FormMain frm = FormMain.Instance;
            SOPManager sopMgr = frm.SOPManager;

            Dictionary<int, VersionInfo> dicVersion = sopMgr.GetVersionDictionary(m_isRegular, m_isNormal);
            if (!dicVersion.ContainsKey(nDisasterID))
                return false;

            Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(m_isRegular, m_isNormal);
            if (!dicSOP.ContainsKey(strFullPath))
                return false;

            DisasterInfo disaster = dicSOP[strFullPath];
            
            PageBackstageHome pageHome = frm.GetPageHome();
            ArrayList arrTabPages = pageHome.GetTabPage();

            int nOldTabPageCount = arrTabPages == null ? 0 : arrTabPages.Count;
            // 기존 Section들의 ID 정보 초기화
            Sections.SectionData.ClearIDList();

            IOManager mgr = new IOManager();

            // Tree의 SelectedNode가 변경되기 때문에 아래 행은 실행시키면 안됨
            //FormMain.Instance.GetPageHome().TabControls.TabPages.Clear();

            if (mgr.Load(frm, frm.DBManager, dicVersion[nDisasterID], disaster.ActionSteps, strCategoryName, strSubCategoryName, nodeDisaster.Text))
            {
                // 기존 탭이 남아 있게 되는데, 불러오기 후 해당 탭들을 삭제한다.
                for (int i = 0; i < nOldTabPageCount; i++)
                {
                    TabPage oldTabPage = (TabPage)arrTabPages[0];
                    pageHome.RemoveTabPage(oldTabPage);
                    arrTabPages.RemoveAt(0);
                    pageHome.GetDockPropertiesLevel().LevelProperties.RemoveAt(0);
                }
                FormMain.Instance.ChangeWorkflow();
                FormMain.Instance.GetPageHome().SelectTab(FormMain.Instance.GetPageHome().TabControls.TabPages[0]);
                //FormMain.Instance.GetPageHome().TabControls.SelectedIndex = 0;
                string strDetail = FormMain.Instance.GetPageHome().TabControls.SelectedTab.Text;
                string strValue = strFullPath + m_strDelimeter + strDetail;
                FormMain.Instance.GetPageOption().SOPInfo(strValue);
                FormMain.Instance.GetPageOption().SOPVersion(dicVersion[nDisasterID]);
                FormMain.Instance.GetPageHome().GetDockPropertiesLevel().GetDisasterInfo(disaster);

                return true;
            }

            return false;
        }

        private TreeNode GetDisasterNode(TreeNode node)
        {
            while (node.Level > 2)
            {
                node = node.Parent;
                if (node.Level == 2)
                    return node;
            }

            return node.Level == 2 ? node : null;
        }

        public new bool Load(SOPManager sopMgr, bool isRegular, bool isNormal)
        {
            m_prevSelectedNode = null;
            treeView.Nodes.Clear();

            m_isRegular = isRegular;
            m_isNormal = isNormal;

            Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(isRegular, isNormal);
            Dictionary<int, VersionInfo> dicVersion = sopMgr.GetVersionDictionary(isRegular, isNormal);

            foreach (KeyValuePair<string, DisasterInfo> pair in dicSOP)
            {
                string strFullPath = pair.Key;

                int nIndex1 = strFullPath.IndexOf(szDeli);
				int nIndex2 = strFullPath.LastIndexOf(szDeli);
                if (nIndex1 < 0 || nIndex2 < 0) continue;

                string strCategoryName = strFullPath.Substring(0, nIndex1);
                string strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                string strDisasterName = strFullPath.Substring(nIndex2 + 1);

                TreeNode nodeCategory = FindNode(strCategoryName, treeView.Nodes);

                if (nodeCategory == null)
                    nodeCategory = treeView.Nodes.Add(strCategoryName);

                TreeNode nodeSubCategory = FindNode(strSubCategoryName, nodeCategory.Nodes);

                if (nodeSubCategory == null)
                    nodeSubCategory = nodeCategory.Nodes.Add(strSubCategoryName);

                TreeNode nodeDisaster = FindNode(strDisasterName, nodeSubCategory.Nodes);

                if (nodeDisaster == null)
                    nodeDisaster = nodeSubCategory.Nodes.Add(strDisasterName);

                if (nodeDisaster.Tag == null)
                    AddActionStep(nodeDisaster, pair.Value, dicVersion);
            }

            treeView.ExpandAll();
            m_frmDisaster.LoadSOP(treeView.Nodes);

            return true;
        }

        private void AddActionStep(TreeNode nodeDisaster, DisasterInfo disaster, Dictionary<int, VersionInfo> dicVersion)
        {
            ArrayList arrActionSteps = disaster.ActionSteps;
            int nDisasterID = disaster.DisasterID;

            if (arrActionSteps == null)
            {
                nodeDisaster.Tag = 0;
                return;
            }

            nodeDisaster.Tag = nDisasterID;
            AddActionStep(nodeDisaster, arrActionSteps);
        }

        private void AddActionStep(TreeNode node, ArrayList arrActionSteps)
        {
            ArrayList arrChildActionStep = new ArrayList();

            ArrayList _arrActionSteps = new ArrayList();
            InsertArray(arrActionSteps, _arrActionSteps);

            while (_arrActionSteps.Count > 0)
            {
                arrChildActionStep.Clear();

                int nChildCount = arrChildActionStep.Count;

                foreach (ActionStepInfo actionStep in _arrActionSteps)
                {
                    if (actionStep.ParentStepID == -1)
                    {
                        TreeNode nodeStep = node.Nodes.Add(actionStep.ActionStepName);
                        nodeStep.Tag = actionStep.ActionStepID;
                    }
                    else
                    {
                        TreeNode nodeParent = FindNode(actionStep.ParentStepID, node.Nodes);

                        if (nodeParent != null)
                        {
                            TreeNode nodeStep = nodeParent.Nodes.Add(actionStep.ActionStepName);
                            nodeStep.Tag = actionStep.ActionStepID;
                        }
                        else
                            arrChildActionStep.Add(actionStep);
                    }
                }

                if (nChildCount == arrChildActionStep.Count)
                    break;

                _arrActionSteps.Clear();

                // 부모 단계가 존재하는 ActionStep들
                InsertArray(arrChildActionStep, _arrActionSteps);
            }
        }

        public TreeNode FindActionStepNode(int nActionStepID, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
                nodes = treeView.Nodes;

            foreach (TreeNode node in nodes)
            {
                if (node.Level >= 3 && node.Tag != null && (int)node.Tag == nActionStepID)
                    return node;

                TreeNode result = FindActionStepNode(nActionStepID, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        public TreeNode FindDisasterNode(int nDisasterID) // DisasterID에 해당돼는 Disaster노드 찾기
        {
            TreeNodeCollection nodes = treeView.Nodes;

            foreach (TreeNode node in nodes)
            {
                TreeNode Dnode = null;
                TreeNode DDnode = null;
                Dnode = node.FirstNode;
                DDnode = Dnode.FirstNode;

                if ((int)DDnode.Tag == nDisasterID)
                    return DDnode;
                DDnode = DDnode.NextNode;

                while (DDnode != null)
                {
                    if ((int)DDnode.Tag == nDisasterID)
                        return DDnode;

                    DDnode = DDnode.NextNode;
                }

                Dnode = Dnode.NextNode;

                while (Dnode != null)
                {
                    DDnode = Dnode.FirstNode;

                    if ((int)DDnode.Tag == nDisasterID)
                        return DDnode;
                    DDnode = DDnode.NextNode;

                    while (DDnode != null)
                    {
                        if ((int)DDnode.Tag == nDisasterID)
                            return DDnode;

                        DDnode = DDnode.NextNode;
                    }

                    Dnode = Dnode.NextNode;
                }
            }

            return null;
        }

        public TreeNode FindNode(int nTag, TreeNodeCollection nodes = null)
        {
            if (nodes == null)
                nodes = treeView.Nodes;

            foreach (TreeNode node in nodes)
            {
                if (node.Tag != null && (int)node.Tag == nTag)
                    return node;

                TreeNode result = FindNode(nTag, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        private void InsertArray(ArrayList arrSrc, ArrayList arrTrg)
        {
            foreach (object obj in arrSrc)
            {
                arrTrg.Add(obj);
            }
        }

        public void UnSelectedNode()
        {
            if (treeView.SelectedNode != null)
            {
                treeView.SelectedNode.ForeColor = Color.Black;
                treeView.SelectedNode = null;
            }            
        }

        public TreeNode GetSelectedNode()
        {
            return treeView.SelectedNode;
        }

        // 등록 모드인가?
        public bool IsRegular
        {
            get { return m_isRegular; }
        }

        // 평일 모드인가?
        public bool IsNormal
        {
            get { return m_isNormal; }
        }

        public bool IgnoreSelect
        {
            get { return m_ignoreSelect; }
            set { m_ignoreSelect = value; }
        }

        public bool IgnoreLoadSOP
        {
            get { return m_ignoreLoadSOP; }
            set { m_ignoreLoadSOP = value; }
        }

        private void BarLevelTree_Resize(object sender, EventArgs e)
        {
            if (!m_isTreeMode)
            {
                m_frmDisaster.Size = new Size(this.Size.Width, this.Size.Height - 60);

                panelLine.Location = new Point(0, m_frmDisaster.Location.Y + m_frmDisaster.Size.Height);
                panelLine.Size = new Size(this.Size.Width, panelLine.Size.Height);

                btnPrev.Location = new Point(btnPrev.Location.X, this.Size.Height - 44);
                btnNext.Location = new Point(btnNext.Location.X, btnPrev.Location.Y);
            }
        }

        private void btnPrevNext_Click(object sender, EventArgs e)
        {
            if (sender == btnPrev)
                m_frmDisaster.GoBack();
            else
                m_frmDisaster.GoForward();
        }
    }

}
