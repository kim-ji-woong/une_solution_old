using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

//libSection
using Sections;
//libSOP
using UnE.SOP;
using UnE.SOP.Tree;
using UnE.SOP.History;
using UnE.SOP.Log;
using UnE.SOP.Data;
using UnE.SOP.Sections;


namespace SOPMonitoringSystem
{
    public partial class BarLevelTree : Form, ISOPTreeContainer
    {

        // 재난 Tree의 Node별 구분자
        private char m_chDelimeter = (char)6;
        private string m_strDelimeter = "";
        private bool m_systemCall = false;
        
        public BarLevelTree()
        {
            InitializeComponent();

            treeView.BeforeSelect += treeView_BeforeSelect;
            treeView.AfterSelect += treeView_AfterSelect;
            treeView.NodeMouseClick += treeView_NodeMouseClick;

            m_strDelimeter = m_chDelimeter.ToString();
        }

        public int PrevSelectedDisasterID
        {
            get { return treeView.PrevSelectedDisasterID; }
            set { treeView.PrevSelectedDisasterID = value; }
        }


        public void ResetSelect()
        {
            treeView.ResetSelect();
        }      

        public void ClearTree()
        {
            treeView.ClearTree();
        }

        public TreeNode AddTreeNode(int nCategoryID, string strCategoryName, int nSubCategoryID, string strSubCategoryName, int nDisasterID, string strDisasterName, int nActionStepID, string strActionStepName)
        {
            return treeView.AddTreeNode(nCategoryID, strCategoryName, nSubCategoryID, strSubCategoryName, nDisasterID, strDisasterName, nActionStepID, strActionStepName);
            /*TreeNode node = treeView.FindNode(nCategoryID);

            if (node == null)
            {
                node = treeView.Nodes.Add(strCategoryName);
                node.Tag = nCategoryID;
            }

            TreeNodeCollection nodes = node.Nodes;
            node = treeView.FindNode(nSubCategoryID, nodes);

            if (node == null)
            {
                node = nodes.Add(strSubCategoryName);
                node.Tag = nSubCategoryID;
            }

            nodes = node.Nodes;
            node = treeView.FindNode(nDisasterID, nodes);

            if (node == null)
            {
                node = nodes.Add(strDisasterName);
                node.Tag = nDisasterID;
            }

            nodes = node.Nodes;
            node = treeView.FindNode(nActionStepID, nodes);

            if (node == null)
            {
                node = nodes.Add(strActionStepName);
                node.Tag = nActionStepID;
            }

            return node;*/
        }

        // Return 값 : strLevelName에 해당하는 노드를 리턴
        public TreeNode AddTreeNode(string strCategory = null, string strSubCategory = null, string strDetailCategory = null, string strLevelName = null)
        {
            SOPTreeNode level = (SOPTreeNode)treeView.AddTreeNode(strCategory, strSubCategory, strDetailCategory, strLevelName);
            treeView.ExpandAll();
            treeView.SelectNode(level);            
            return level;
        }

        public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null)
        {
            return treeView.FindNode(strValue, parentNodes);
        }            

        public void RemoveTreeNode(string strValue)
        {
            treeView.RemoveTreeNode(strValue);
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
            treeView.SelectNode(node);
            FormSOP.Instance.EnableOptions(true);
        }

        public void SelectNode(TreeNode node, bool systemCall)
        {
            m_systemCall = systemCall;
            SelectNode(node);
            m_systemCall = false;
        }

        public void SetSelectPath(string szNodePath)
        {
            FormSOP.Instance.toolstripSetting(szNodePath);
        }

        public void SetScenarioName(int nActionStepID)
        {
            FormSOP.Instance.GetPageHome().SetScenarioName(nActionStepID);
        }

        public TreeNode FindActionStepNode(int nActionStepID)
        {
            return treeView.FindActionStepNode(nActionStepID, treeView.Nodes);
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
            //FormSOP.Instance.GetPageHome().GetDockPropertiesLevel().AddTitle(strPath);

            node.ForeColor = Color.Red;
            treeView.PrevSelectedNode = node;

            if (node.Level <= 1)
                return;

            TreeNode nodeDisaster = treeView.GetDisasterNode(node);
            if (nodeDisaster == null)
                return;

            if (node == nodeDisaster)   // Disaster 노드를 선택한 경우
            {
                if ((int)nodeDisaster.Tag == treeView.PrevSelectedDisasterID)
                    return;
                LoadSOP(nodeDisaster, -1);
            }
            else                        // 대응 단계 노드를 선택한 경우
            {
                int nActionStepID = (int)(node.Tag);
                if ((int)nodeDisaster.Tag == treeView.PrevSelectedDisasterID)
                {
                    //if (FormSOP.Instance.GetPageHome().IsChangeCurrentTab())
                    {
                        LoadSOP(nodeDisaster, nActionStepID);
                    }
                    // 단계 탭 전환
                    ChangeTab(nActionStepID);
                }
                else
                {
                    //if (FormMain.Instance.GetPageHome().IsChangeCurrentTab())
                    //{
                        LoadSOP(nodeDisaster, nActionStepID);
                    //}
                    // SOP 다시 읽기
                    //LoadSOP(nodeDisaster);
                    // 단계 탭 전환
                    ChangeTab(nActionStepID);
                }
            }
            treeView.PrevSelectedDisasterID = (int)nodeDisaster.Tag;
            FormSOP.Instance.ChangeWorkflow();

            // SOP가 바뀔때마다 ComponentContents를 지우고 새로 그리는 과정을 거치지 않고, 해당 Panel만 Show/Hide 하는 방식으로
            // 바뀌었으므로 아래 로직은 무시한다.
            // [2014-12-30] 김지웅
            //// Tree에서 실행되는 것이 아니므로 다른곳에서 지운다. 여기서 지우면 안됨. skkim 2014-03-20
            //// 새로운 SOP가 실행되었으므로 기존의 ComponentContents는 모두 지운다.
            //FormSOP.Instance.GetPageHome().ClearProcess();            
            
            FormSOP.Instance.EnabledRunGroup();           
        }

        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = (TreeNode)e.Node;
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
            }
        }

        private void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (m_systemCall)
                return;

            TreeNode node = treeView.SelectedNode;
            if (node == null)
                return;

            node.ForeColor = Color.Black;



          

            foreach (SectionTabPage page in FormSOP.Instance.GetPageHome().TabControls.Controls)
            {
                FormSOP.Instance.GetPageHome().changeLocation(page.Height);
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (m_systemCall)
                return;

            TreeNode node = treeView.SelectedNode;
            if (node == null)
                return;


   

            string strPath = node.FullPath.Replace("\\", szDeli.ToString());          

            node.ForeColor = Color.Red;

            if (treeView.PrevSelectedNode == node)
                return;

            treeView.PrevSelectedNode = node;

            if (node.Level <= 1)
                return;

            TreeNode nodeDisaster = treeView.GetDisasterNode(node);
            if (nodeDisaster == null)
                return;

            if (node == nodeDisaster)   // Disaster 노드를 선택한 경우
            {
                if ((int)nodeDisaster.Tag == treeView.PrevSelectedDisasterID)
                    return;

                LoadSOP(nodeDisaster, -1);
            }
            else                        // 대응 단계 노드를 선택한 경우
            {
                int nActionStepID = (int)node.Tag;
                if ((int)nodeDisaster.Tag == treeView.PrevSelectedDisasterID)
                {
                    if (FormSOP.Instance.GetPageHome().IsChangeCurrentTab())
                    {
                        LoadSOP(nodeDisaster, nActionStepID);
                    }

                    // 단계 탭 전환
                    ChangeTab(nActionStepID);
                }
                else
                {
                    // SOP 다시 읽기
                    LoadSOP(nodeDisaster, nActionStepID);
                    // 단계 탭 전환
                    ChangeTab(nActionStepID);
                }
            }

            treeView.PrevSelectedDisasterID = (int)nodeDisaster.Tag;

            FormSOP.Instance.SelectViewTab(true);
            FormSOP.Instance.EnabledRunGroup();
            FormSOP.Instance.GetPageHome().panel.Visible = true;
            FormSOP.Instance.GetPageHome().SetBackgroundImage(true);

            TreeNode view = treeView.SelectedNode;
            if (view == null)
                return;
            if( view != null)
            {
                while (view.Parent != null)
                {
                    view = view.Parent;
                }
                FormSOP.Instance.GetPageHome().toolstripSetting(view.FullPath.ToString());
            }            

            foreach (SectionTabPage page in FormSOP.Instance.GetPageHome().TabControls.Controls)
            {
                FormSOP.Instance.GetPageHome().changeLocation(page.Height);
            }
        }

        public void ChangeTab(int nActionStepID)
        {
            FormSOP frm = FormSOP.Instance;
            PageBackstageSOP pageHome = frm.GetPageHome();
            ArrayList arrTabPages = pageHome.GetTabPage();

            foreach (SectionTabPage page in arrTabPages)
            {
                if (page.ActionStepID == nActionStepID)
                {
                    pageHome.SelectTab(page);
                    return;
                }
            }
        }

        public bool LoadSOP(TreeNode nodeDisaster, int nActionStepID)
        {
            if (IgnoreLoadSOP)
                return true;

            return LoadSOP(nodeDisaster.Parent.Parent.Text, nodeDisaster.Parent.Text, nodeDisaster.Text, (int)nodeDisaster.Tag, null, null, nActionStepID);
            /*string strCategoryName = nodeDisaster.Parent.Parent.Text;
            string strSubCategoryName = nodeDisaster.Parent.Text;

            int nDisasterID = (int)nodeDisaster.Tag;
            string strFullPath = strCategoryName + m_strDelimeter + strSubCategoryName + m_strDelimeter + nodeDisaster.Text;

            FormSOP frm = FormSOP.Instance;
            UnE.SOP.SOPManager sopMgr = frm.SOPManager;
                  
            Dictionary<int, VersionInfo> dicVersion = sopMgr.GetVersionDictionary(IsRegular, IsNormal);
            if (!dicVersion.ContainsKey(nDisasterID))
                return false;

            Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(IsRegular, IsNormal);
            if (!dicSOP.ContainsKey(strFullPath))
                return false;

            DisasterInfo disaster = dicSOP[strFullPath];
            
            PageBackstageSOP pageHome = frm.GetPageHome();
            ArrayList arrTabPages = pageHome.GetTabPage();

            int nOldTabPageCount = arrTabPages == null ? 0 : arrTabPages.Count;
            // 기존 탭이 남아 있게 되는데, 불러오기 후 해당 탭들을 삭제한다.
            for (int i = 0; i < nOldTabPageCount; i++)
            {
                SectionTabPage oldTabPage = (SectionTabPage)arrTabPages[0];
                pageHome.RemoveTabPage(oldTabPage);
                arrTabPages.RemoveAt(0);                
            } 

            // 기존 Section들의 ID 정보 초기화
            Sections.SectionData.ClearIDList();

            IOManager mgr = new IOManager();

            // Tree의 SelectedNode가 변경되기 때문에 아래 행은 실행시키면 안됨
            //FormMain.Instance.GetPageHome().TabControls.TabPages.Clear();
            
            if (mgr.Load(frm, frm.DBManager, dicVersion[nDisasterID], disaster.ActionSteps, strCategoryName, strSubCategoryName, nodeDisaster.Text))
            {
                FormSOP.Instance.SelectViewTab();

                
                  

                FormSOP.Instance.ChangeWorkflow();
                SectionTabPage tabPage = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.GetFirstPage();
                FormSOP.Instance.GetPageHome().SelectTab(tabPage);
                
                string strDetail = FormSOP.Instance.GetPageHome().TabControls.SelectedTab.Text;
                string strValue = strFullPath + m_strDelimeter + strDetail;
                FormSOP.Instance.GetPageOption().SOPInfo(strValue);
                FormSOP.Instance.GetPageOption().SOPVersion(dicVersion[nDisasterID]);          
               
                if (tabPage.PanelComponentContents.Controls.Count == 0)
                {
                    // 처음 SOP를 불러왔을때는 아직 진행상황이 없으므로 ComponentContents 영역을 감춘다.
                    // 2015-06-25 영흥요청으로 처음부터 나오도록 바꾼다.skkim
                    //FormSOP.Instance.GetPageHome().HideComponentContents();
                    FormSOP.Instance.GetPageHome().ShowComponentContents();
                    FormSOP.Instance.GetPageHome().toolstripSetting("");
                }
                else
                {
                    FormSOP.Instance.GetPageHome().ShowComponentContents();
                    FormSOP.Instance.GetPageHome().toolstripSetting("");
                }
                return true;
            }
            return false;*/
        }

        public bool LoadSOP(string strCategoryName, string strSubCategoryName, string strDisasterName, int nDisasterID, DisasterInfo disaster = null, VersionInfo version = null, int nActionStepID = -1)
        {
            string strFullPath = strCategoryName + m_strDelimeter + strSubCategoryName + m_strDelimeter + strDisasterName;

            FormSOP frm = FormSOP.Instance;
            UnE.SOP.SOPManager sopMgr = frm.SOPManager;

            if (version == null)
            {
                Dictionary<int, VersionInfo> dicVersion = sopMgr.GetVersionDictionary(IsRegular, IsNormal);
                if (!dicVersion.ContainsKey(nDisasterID))
                    return false;

                version = dicVersion[nDisasterID];
            }

            if (disaster == null)
            {
                Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(IsRegular, IsNormal);
                if (!dicSOP.ContainsKey(strFullPath))
                    return false;

               disaster = dicSOP[strFullPath];
            }

            PageBackstageSOP pageHome = frm.GetPageHome();
            ArrayList arrTabPages = pageHome.GetTabPage();

            int nOldTabPageCount = arrTabPages == null ? 0 : arrTabPages.Count;
            // 기존 탭이 남아 있게 되는데, 불러오기 후 해당 탭들을 삭제한다.
            for (int i = 0; i < nOldTabPageCount; i++)
            {
                SectionTabPage oldTabPage = (SectionTabPage)arrTabPages[0];

                /*ActionStepInfo actionStep = disaster.FindActionStep(oldTabPage.ActionStepID);

                // 기존 탭이 새롭게 불러들이려는 SOP와 일치하면 해당 탭을 삭제하지 않는다.
                if (actionStep != null)
                    continue;*/

                pageHome.RemoveTabPage(oldTabPage);
                arrTabPages.RemoveAt(0);
                oldTabPage.LinkedZoneID = -1;
                oldTabPage.LinkedZoneName = "";
                
            }

            // 기존 Section들의 ID 정보 초기화
            Sections.SectionData.ClearIDList();

            IOManager mgr = new IOManager();

            // Tree의 SelectedNode가 변경되기 때문에 아래 행은 실행시키면 안됨
            //FormMain.Instance.GetPageHome().TabControls.TabPages.Clear();

            if (mgr.Load(frm, frm.DBManager, version, disaster.ActionSteps, strCategoryName, strSubCategoryName, strDisasterName))
            {
                FormSOP.Instance.SelectViewTab();

                FormSOP.Instance.ChangeWorkflow();
                SectionTabPage tabPage = GetActionStepTabPage(nActionStepID);
                //SectionTabPage tabPage = (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.GetFirstPage();

                if (tabPage != null)
                    FormSOP.Instance.GetPageHome().SelectTab(tabPage);

                string strDetail = FormSOP.Instance.GetPageHome().TabControls.SelectedTab.Text;
                string strValue = strFullPath + m_strDelimeter + strDetail;
                FormSOP.Instance.GetPageOption().SOPInfo(strValue);
                FormSOP.Instance.GetPageOption().SOPVersion(version);

                if (tabPage.PanelComponentContents.Controls.Count == 0)
                {
                    // 처음 SOP를 불러왔을때는 아직 진행상황이 없으므로 ComponentContents 영역을 감춘다.
                    // 2015-06-25 영흥요청으로 처음부터 나오도록 바꾼다.skkim
                    //FormSOP.Instance.GetPageHome().HideComponentContents();
                    FormSOP.Instance.GetPageHome().ShowComponentContents();
                    FormSOP.Instance.GetPageHome().toolstripSetting("");
                }
                else
                {
                    FormSOP.Instance.GetPageHome().ShowComponentContents();
                    FormSOP.Instance.GetPageHome().toolstripSetting("");
                }
                return true;
            }
            return false;
        }

        private SectionTabPage GetActionStepTabPage(int nActionStepID)
        {
            if (nActionStepID < 0)
            {
                return (SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.GetFirstPage();
            }

            PageBackstageSOP pageHome = FormSOP.Instance.GetPageHome();
            SectionTabPage tabPage = pageHome.GetTabPage(nActionStepID, true);

            if (tabPage != null)
                return tabPage;

            return pageHome.GetTabPage(nActionStepID, false);
        }

        public new bool Load(UnE.SOP.SOPManager sopMgr, bool isRegular, bool isNormal)
        {
            bool bResult = treeView.Load(sopMgr, isRegular, isNormal);
            treeView.ExpandAll();
            return bResult;
        }

        public void AddActionStep(TreeNode node, ArrayList arrActionSteps)
        {
            treeView.AddActionStep(node, arrActionSteps);
            treeView.ExpandAll();
        }

        public TreeNode FindActionStepNode(int nActionStepID, TreeNodeCollection nodes = null)
        {
            return treeView.FindActionStepNode(nActionStepID, nodes);
        }

        public TreeNode FindDisasterNode(int nDisasterID) // DisasterID에 해당돼는 Disaster노드 찾기
        {
            return treeView.FindDisasterNode(nDisasterID);
        }

        public TreeNode FindNode(int nTag, TreeNodeCollection nodes = null)
        {
            return treeView.FindNode(nTag, nodes);
        }

        public void UnSelectedNode()
        {
            treeView.UnSelectedNode();
        }

        public TreeNode GetSelectedNode()
        {
            return treeView.GetSelectedNode();
        }

        // 등록 모드인가?
        public bool IsRegular
        {
            get { return treeView.IsRegular; }
        }

        // 평일 모드인가?
        public bool IsNormal
        {
            get { return treeView.IsNormal; }
        }

        public bool IgnoreSelect
        {
            get { return treeView.IgnoreSelect; }
            set { treeView.IgnoreSelect = value; }
        }

        public bool IgnoreLoadSOP
        {
            get { return treeView.IgnoreLoadSOP; }
            set { treeView.IgnoreLoadSOP = value; }
        }

        private void treeView_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
        }

        // nActionStepID가 사용되는 Tree인지 확인하여(isNormal, isRegular)
        // 다른 버전일 경우 다시 로딩하도록 한다.
        public bool ReloadTree(int nActionStepID, out bool isRegular, out bool isNormal)
        {
            isRegular = isNormal = true;
            VersionInfo version = FormSOP.Instance.SOPManager.GetActionStepVersionInfo(nActionStepID);

            if (version == null)
                return false;

            isRegular = version.IsRegular;
            isNormal = version.IsNormal;

            if (version.IsRegular == this.IsRegular && version.IsNormal == this.IsNormal)
                return true;

            return Load(FormSOP.Instance.SOPManager, version.IsRegular, version.IsNormal);
        }
    }
}
