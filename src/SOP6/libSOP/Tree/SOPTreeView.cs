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

using UnE.SOP.Tree;
using UnE.SOP.History;
using UnE.SOP.Log;
using UnE.SOP.Data;
using UnE.SOP.Sections;
using UnE.SOP.Workstate;


namespace UnE
{
    namespace SOP
    {        
        namespace Tree
        {
            public class SOPTreeView : TreeView
            {
                private class ActionStepComparer : IComparer
                {
                    private Dictionary<string, int> m_dicActionStepPriority = new Dictionary<string, int>();

                    public ActionStepComparer()
                    {
                        for (int i=0;i<UnE.SOP.Sections.SectionTabControl.StandardActionStepNames.Count();i++)
                        {
                            string strActionStepName = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[i];
                            m_dicActionStepPriority[strActionStepName] = i;
                        }
                    }

                    public void SetActionStepPriority(string strActionStepName, int nPriority)
                    {
                        m_dicActionStepPriority[strActionStepName] = nPriority;
                    }

                    private int GetActionStepPriority(string strActionStepName)
                    {
                        int nPriority;

                        if (m_dicActionStepPriority.TryGetValue(strActionStepName, out nPriority))
                            return nPriority;

                        return -1;
                    }

                    public int Compare(object x, object y)
                    {
                        ActionStepInfo actionStep1 = (ActionStepInfo)x;
                        ActionStepInfo actionStep2 = (ActionStepInfo)y;

                        int nPriority1 = GetActionStepPriority(actionStep1.ActionStepName);
                        int nPriority2 = GetActionStepPriority(actionStep2.ActionStepName);

                        if (nPriority1 > nPriority2)
                            return 1;
                        else if (nPriority1 < nPriority2)
                            return -1;
                        //else
                        return 0;
                    }
                }

                //System.Windows.Forms.TreeView treeView = new System.Windows.Forms.TreeView();

                private TreeNode m_prevSelectedNode = null;
                public System.Windows.Forms.TreeNode PrevSelectedNode
                {
                    get { return m_prevSelectedNode; }
                    set { m_prevSelectedNode = value; }
                }

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

                public System.Windows.Forms.TreeView TreeView
                {
                    get { return this; }
                }
                protected ImageList arImageList = new ImageList();
                public SOPTreeView()
                {
                    this.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.Location = new System.Drawing.Point(0, 0);
                    this.Name = "treeView";
                    this.Size = new System.Drawing.Size(284, 262);
                    this.TabIndex = 0;


                    arImageList.Images.Add(global::libSOP.Properties.Resources.btnEtc_User);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_typoon);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_earthquake);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_snowfall);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_flooding);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btnEtc_User);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_fire);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_fire);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_spill);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_spill);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_spill);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_spill);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_terror);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_volcano);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btnEtc_User);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_terror);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_terror);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_terror);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_terror);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_terror);
                    arImageList.Images.Add(global::libSOP.Properties.Resources.btn_sub_strongwind);

                    this.ImageList = arImageList;
                }

                private object[] m_arSubCategorys = 
		        {
			        "SOP상황", "태풍",	"지진", "폭설", "침수", "일반재해", "화재","산불", 	"오염", "누출","유출","암모니아", 
			        "테러",	"폭발", "119상황","무장", "괴선박", "폭탄", "침입", 	"폭약", "자연재해"
		        };

                private int SetSubCategoryImage(string strValue)
                {
                    for (int i = 0; i < m_arSubCategorys.Length; i++)
                    {
                        if (strValue == (string)m_arSubCategorys[i] || strValue.Contains((string)m_arSubCategorys[i]))
                            return i;
                    }
                    return 0;
                }

                private object[] m_arCategorys = 
		        {
			        "화재", "자연재해",  "오염", "누출", "유출", "암모니아", "수소",
			        "테러",	"폭발", "무장", "괴선박", "폭탄", "침입", "폭약", "일반재해","기타", "SOP상황"
		        };

                private int GetSOPTreeIndex(string strValue)
                {
                    for (int i = 0; i < m_arCategorys.Length; i++)
                    {
                        if (strValue == (string)m_arCategorys[i] || strValue.Contains((string)m_arCategorys[i]))
                            return i;
                    }
                    return 0;
                }

                public void ClearTree()
                {
                    Nodes.Clear();
                }

                public TreeNode AddTreeNode(int nCategoryID, string strCategoryName, int nSubCategoryID, string strSubCategoryName, int nDisasterID, string strDisasterName, int nActionStepID, string strActionStepName)
                {
                    TreeNode child = _FindNode(strCategoryName, Nodes);
                    if (child == null)
                    {
                        int nIdx = SetSubCategoryImage(strCategoryName);
                        int nTreeIdx = GetSOPTreeIndex(strCategoryName);

                        child = new SOPTreeNode(strCategoryName);

                        child.ImageIndex = nIdx;
                        child.SelectedImageIndex = nTreeIdx;

                        ((SOPTreeNode)child).TreeNodeType = TreeNodeType.CATEGORY_NODE;
                        Nodes.Add(child);
                    }

                    child.Tag = nCategoryID;

                    TreeNode second = _FindNode(strSubCategoryName, child.Nodes);
                    if (second == null)
                    {
                        second = new SOPTreeNode(strSubCategoryName);
                        ((SOPTreeNode)second).TreeNodeType = TreeNodeType.SUBCATEGOY_NODE;
                        child.Nodes.Add(second);
                    }

                    second.Tag = nSubCategoryID;

                    TreeNode detail = _FindNode(strDisasterName, second.Nodes);
                    if (detail == null)
                    {
                        detail = new SOPTreeNode(strDisasterName);
                        ((SOPTreeNode)detail).TreeNodeType = TreeNodeType.DISASTER_NODE;
                        second.Nodes.Add(detail);
                    }

                    ((SOPTreeNode)detail).DisasterID = nDisasterID;
                    detail.Tag = nDisasterID;

                    TreeNode level = _FindNode(strActionStepName, detail.Nodes);
                    if (level == null)
                    {
                        level = new SOPTreeNode(strActionStepName);
                        ((SOPTreeNode)level).TreeNodeType = TreeNodeType.ACTIONSTEP_NODE;
                        detail.Nodes.Add(level);
                    }

                    ((SOPTreeNode)level).ActionStepID = nActionStepID;
                    level.Tag = nActionStepID;

                    return level;
                }

                // Return 값 : strLevelName에 해당하는 노드를 리턴
                public TreeNode AddTreeNode(string strCategory = null, string strSubCategory = null, string strDetailCategory = null, string strLevelName = null)
                {
                    TreeNode child = FindNode(strCategory, Nodes);
                    if (child == null)
                    {
                        int nIdx = SetSubCategoryImage(strCategory);
                        int nTreeIdx = GetSOPTreeIndex(strCategory);
                        
                        child = new SOPTreeNode(strCategory);

                        child.ImageIndex = nIdx;
                        child.SelectedImageIndex = nTreeIdx;                       

                        ((SOPTreeNode)child).TreeNodeType = TreeNodeType.CATEGORY_NODE;
                        Nodes.Add(child);
                    }

                    TreeNode second = FindNode(strSubCategory, child.Nodes);
                    if (second == null)
                    {
                        second = new SOPTreeNode(strSubCategory);
                        ((SOPTreeNode)second).TreeNodeType = TreeNodeType.SUBCATEGOY_NODE;
                        child.Nodes.Add(second);
                    }                        

                    TreeNode detail = FindNode(strDetailCategory, second.Nodes);
                    if (detail == null)
                    {
                        detail = new SOPTreeNode(strDetailCategory);
                        ((SOPTreeNode)detail).TreeNodeType = TreeNodeType.DISASTER_NODE;
                        second.Nodes.Add(detail);
                    }

                    TreeNode level = FindNode(strLevelName, detail.Nodes);
                    if (level == null)
                    {
                        level = new SOPTreeNode(strLevelName);
                        ((SOPTreeNode)level).TreeNodeType = TreeNodeType.ACTIONSTEP_NODE;
                        detail.Nodes.Add(level);
                    }

                    ExpandAll();

                    SelectNode(level);

                    return level;
                }

                private TreeNode _FindNode(string strValue, TreeNodeCollection nodes)
                {
                    foreach (TreeNode node in nodes)
                    {
                        if (strValue == node.Text)
                            return node;
                    }

                    return null;
                }

                public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null)
                {
                    TreeNodeCollection nodes = parentNodes == null ? Nodes : parentNodes;

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
                    TreeNode node = FindNode(strValue, Nodes);
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

                public virtual void SelectNode(TreeNode node)
                {
                    if (node == null)
                        return;

                    

                    m_ignoreSelect = true;

                    if (SelectedNode != null)
                        SelectedNode.ForeColor = Color.Black;

                    SelectedNode = (TreeNode)node;
                    node.ForeColor = Color.Red;                   

                    ISOPTreeContainer formMain = ProxySOP.Instance.SOPTreeContainer;
                    if (formMain != null)
                    {
                        formMain.SetSelectPath(node.FullPath.ToString());
                        if (node.Level > 2)
                        {
                            formMain.SetScenarioName(((SOPTreeNode)node).ActionStepID);
                        }
                    }                   
                }               

                public TreeNode GetCurrentDisasterNode()
                {
                    if (m_nPrevSelectedDisasterID < 0)
                        return null;

                    return FindNode(m_nPrevSelectedDisasterID);
                }

                public  TreeNode GetDisasterNode(TreeNode node)
                {
                    while (node.Level > 2)
                    {
                        node = node.Parent;
                        if (node.Level == 2)
                            return node;
                    }

                    return node.Level == 2 ? node : null;
                }

                public bool Load(SOPManager sopMgr, bool isRegular, bool isNormal)
                {
                    m_prevSelectedNode = null;
                    Nodes.Clear();

                    m_isRegular = isRegular;
                    m_isNormal = isNormal;

                    Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(isRegular, isNormal);
                    Dictionary<int, VersionInfo> dicVersion = sopMgr.GetVersionDictionary(isRegular, isNormal);

                    foreach (KeyValuePair<string, DisasterInfo> pair in dicSOP)
                    {
                        string strFullPath = pair.Key;

                        int nIndex1 = strFullPath.IndexOf(m_chDelimeter);
                        int nIndex2 = strFullPath.LastIndexOf(m_chDelimeter);
                        if (nIndex1 < 0 || nIndex2 < 0)
                            continue;

                        string strCategoryName = strFullPath.Substring(0, nIndex1);
                        string strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                        string strDisasterName = strFullPath.Substring(nIndex2 + 1);

                        TreeNode nodeCategory = FindNode(strCategoryName, Nodes);
                        if (nodeCategory == null)
                        {
                            int nIdx = SetSubCategoryImage(strCategoryName);                          

                            nodeCategory = new SOPTreeNode(strCategoryName);

                            nodeCategory.ImageIndex = nIdx;
                            nodeCategory.SelectedImageIndex = nIdx;

                            ((SOPTreeNode)nodeCategory).TreeNodeType = TreeNodeType.CATEGORY_NODE;
                            Nodes.Add(nodeCategory);
                        }

                        TreeNode nodeSubCategory = FindNode(strSubCategoryName, nodeCategory.Nodes);
                        if (nodeSubCategory == null)
                        {

                            int nIdx = SetSubCategoryImage(strSubCategoryName);
                           
                            nodeSubCategory = new SOPTreeNode(strSubCategoryName);

                            nodeSubCategory.ImageIndex = nIdx;
                            nodeSubCategory.SelectedImageIndex = nIdx;

                            ((SOPTreeNode)nodeSubCategory).TreeNodeType = TreeNodeType.SUBCATEGOY_NODE;
                            nodeCategory.Nodes.Add(nodeSubCategory);                           
                        }
                        TreeNode nodeDisaster = FindNode(strDisasterName, nodeSubCategory.Nodes);
                        if (nodeDisaster == null)
                        {
                            int nIdx = SetSubCategoryImage(strSubCategoryName);
                            
                            nodeDisaster = new SOPTreeNode(strDisasterName);

                            nodeDisaster.ImageIndex = nIdx;
                            nodeDisaster.SelectedImageIndex = nIdx;

                            ((SOPTreeNode)nodeDisaster).TreeNodeType = TreeNodeType.DISASTER_NODE;
                            nodeSubCategory.Nodes.Add(nodeDisaster);
                        }                        

                        if (nodeDisaster.Tag == null)
                            AddActionStep(nodeDisaster, pair.Value, dicVersion);
                    }

                    ExpandAll();

                    return true;
                }

                private void AddActionStep(TreeNode nodeDisaster, DisasterInfo disaster, Dictionary<int, VersionInfo> dicVersion)
                {
                    ArrayList arrActionSteps = disaster.ActionSteps;
                    int nDisasterID = disaster.DisasterID;

                    ((SOPTreeNode)nodeDisaster).DisasterID = nDisasterID;

                    if (arrActionSteps == null)
                    {
                        nodeDisaster.Tag = 0;
                        return;
                    }
                    
                    nodeDisaster.Tag = nDisasterID;
                    AddActionStep(nodeDisaster, arrActionSteps);
                }

                private void SortActionSteps(ArrayList arrActionSteps)
                {
                    arrActionSteps.Sort(new ActionStepComparer());
                }

                public void AddActionStep(TreeNode node, ArrayList arrActionSteps)
                {
                    ArrayList arrChildActionStep = new ArrayList();

                    ArrayList _arrActionSteps = new ArrayList();
                    InsertArray(arrActionSteps, _arrActionSteps);
                    SortActionSteps(_arrActionSteps);

                    int nDisasterID = ((SOPTreeNode)node).DisasterID;

                    while (_arrActionSteps.Count > 0)
                    {
                        arrChildActionStep.Clear();

                        int nChildCount = arrChildActionStep.Count;

                        foreach (ActionStepInfo actionStep in _arrActionSteps)
                        {
                            if (actionStep.ParentStepID == -1)
                            {
                               
                                SOPTreeNode nodeStep = new SOPTreeNode(actionStep.ActionStepName);
                               
                                nodeStep.ImageIndex = node.ImageIndex;
                                nodeStep.SelectedImageIndex = node.SelectedImageIndex;

                                nodeStep.TreeNodeType = TreeNodeType.ACTIONSTEP_NODE;                                
                                nodeStep.Tag = actionStep.ActionStepID;
                                nodeStep.ActionStepID = actionStep.ActionStepID;
                                nodeStep.DisasterID = nDisasterID;
                                node.Nodes.Add(nodeStep);
                            }
                            else
                            {
                                TreeNode nodeParent = FindNode(actionStep.ParentStepID, node.Nodes);
                                if (nodeParent != null)
                                {
                                    SOPTreeNode nodeStep = new SOPTreeNode(actionStep.ActionStepName);
                                    nodeStep.TreeNodeType = TreeNodeType.ACTIONSTEP_NODE;
                                    
                                    nodeStep.ImageIndex = node.ImageIndex;
                                    nodeStep.SelectedImageIndex = node.SelectedImageIndex;

                                    nodeStep.Tag = actionStep.ActionStepID;
                                    nodeStep.ActionStepID = actionStep.ActionStepID;
                                    nodeStep.DisasterID = nDisasterID;
                                    nodeParent.Nodes.Add(nodeStep);
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
                        nodes = Nodes;

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
                    TreeNodeCollection nodes = Nodes;

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
                        nodes = Nodes;

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
                    if (SelectedNode != null)
                    {
                        SelectedNode.ForeColor = Color.Black;
                        SelectedNode = null;
                    }
                }

                public TreeNode GetSelectedNode()
                {
                    return SelectedNode;
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
            }

        }

    }
}