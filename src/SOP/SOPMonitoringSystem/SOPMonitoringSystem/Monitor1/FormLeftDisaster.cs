using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;

namespace SOPMonitoringSystem
{
    public partial class FormLeftDisaster : Form
    {
        private FormMain m_Main = null;
        private int m_nVersionID = -1;
        private string m_strVersionName = "";
        private string m_strOwner = "";
        private DateTime m_dtLastAccess;
        private string m_strDesc = "";

        public FormLeftDisaster(FormMain main)
        {
            InitializeComponent();
            m_Main = main;

            GetDisasterCategoryInfo();
            LoadLastVersion();
            treeViewDisaster.ExpandAll();
        }

        public void GetDisasterCategoryInfo()
        {
            ArrayList arrList = new ArrayList();
            arrList = m_Main.GetDBManager().GetDisasterCategoryName();
            if (arrList.Count == 0) return;

            for (int i = 0; i < arrList.Count; i++)
            {
                Data_DispasterCategory data = (Data_DispasterCategory)arrList[i];
                treeViewDisaster.Nodes.Add(data.CategoryName.TrimEnd());
            }
        }

        private TreeNode FindTreeItem(TreeNodeCollection nodes, string strNodeText)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text == strNodeText)
                    return node;
            }

            return null;
        }

        public int GetVersionID()
        {
            return m_nVersionID;
        }

        private bool LoadLastVersion()
        {
            string strSQL = "select Version.id, Version.VersionName, CompanyMember.MemberName, CreateTime, LastAccessTime, Description";
            strSQL += " from Version inner join CompanyMember";
            strSQL += " on CreateTime = (select Max(CreateTime) from Version) and CompanyMember.ID = (Select MemberID from SOPGenUser where Version.OwnerID = SOPGenUser.ID)";
            //string strSQL = "select * from Version where CreateTime = (select Max(CreateTime) from Version)";
            //string strSQL = "select * from sop.dbo.Version where VersionName = 'V1.1240'";

            WebDBManager dbMgr = m_Main.GetDBManager();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            DateTime dtCreate, dtDefault = new DateTime();

            for (int i=0;i<arrResult.Count - 5;i+=6)
            {
                m_nVersionID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                m_strVersionName = dbMgr.GetStringField(arrResult[i + 1].ToString(), "");
                m_strOwner = dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                dtCreate = dbMgr.GetDateTimeField(arrResult[i + 3], dtDefault);
                m_dtLastAccess = dbMgr.GetDateTimeField(arrResult[i + 4], dtDefault);
                m_strDesc = dbMgr.GetStringField(arrResult[i + 5], "");
            }

            Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster;
            Dictionary<int, TreeNode> dicSubNode;

            if (!LoadSubDisaster(m_nVersionID, out dicSubDisaster, out dicSubNode))
                return false;

            return true;
        }

        public string GetVersionName()
        {
            return m_strVersionName;
        }

        public string GetVersionOwner()
        {
            return m_strOwner;
        }

        public DateTime GetLastAccessTime()
        {
            return m_dtLastAccess;
        }

        public string GetDescription()
        {
            return m_strDesc;
        }

        public bool LoadSubDisaster(int nVersionID, out Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster, out Dictionary<int, TreeNode> dicSubNode)
        {
            string strSQL = "select * from DisasterCategory";

            WebDBManager dbMgr = m_Main.GetDBManager();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            Dictionary<int, string> dicDisaster = new Dictionary<int, string>();

            for (int i=0;i<arrResult.Count - 1;i+=2)
            {
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                string strDisaster = dbMgr.GetStringField(arrResult[i + 1].ToString(), "");
                dicDisaster[nID] = strDisaster;
            }

            /*strSQL = "select * from SubDisasterCategory where VersionID = " + nVersionID.ToString();
            arrResult = dbMgr.GetResultData(strSQL, 0);*/
            ArrayList arrField = new ArrayList();
            ArrayList arrValue = new ArrayList();

            arrField.Add("versionID");
            arrValue.Add(nVersionID.ToString());
            dbMgr.RunStoredProcedure("sp_SubDisasterList", arrField, arrValue, 0, out arrResult);

            dicSubDisaster = new Dictionary<TreeNode, SubDisasterCategoryData>();
            dicSubNode = new Dictionary<int, TreeNode>();

            for (int i=0;i<arrResult.Count - 4;i+=5)
            {
                int nID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                int nDisasterID = dbMgr.GetIntField(arrResult[i + 1].ToString(), 0);
                string strSubDisaster = dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                int nParentID = dbMgr.GetIntField(arrResult[i + 3].ToString(), -1);
                int nActionStepCount = dbMgr.GetIntField(arrResult[i + 4].ToString(), 0);

                if (!dicDisaster.ContainsKey(nDisasterID))
                {
                    return false;
                }

                TreeNode node = FindTreeItem(treeViewDisaster.Nodes, dicDisaster[nDisasterID]);
                if (node == null)
                {
                    return false;
                }

                if (nParentID < 0)
                {
                    node = node.Nodes.Add(strSubDisaster);
                    if (node == null)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!dicSubNode.ContainsKey(nParentID))
                    {
                        return false;
                    }

                    node = dicSubNode[nParentID];
                    node = node.Nodes.Add(strSubDisaster);

                    if (node == null)
                    {
                        return false;
                    }
                }

                // SubDisaster에 연결된 ActionStep이 몇개인지 알려준다.
                node.Tag = nActionStepCount;

                SubDisasterCategoryData data = new SubDisasterCategoryData(nID, nDisasterID, strSubDisaster, nParentID);
                dicSubDisaster[node] = data;
                dicSubNode[nID] = node;
            }

            return true;
        }

        private void treeViewDisaster_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeViewDisaster.SelectedNode;
            if (node == null) return;

            node.ForeColor = Color.Red;

            string strFullPath;
            int nDepth = GetNodeText(node, out strFullPath);
            m_Main.OnSelectedSOP(nDepth, strFullPath, node);
        }

        private void treeViewDisaster_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = treeViewDisaster.SelectedNode;
            if (node == null) return;

            node.ForeColor = Color.Black;
        }

        public TreeNode GetSelectedNode()
        {
            return treeViewDisaster.SelectedNode;
        }

        // node의 Full Text
        public int GetNodeText(TreeNode node, out string strFullPath)
        {
            strFullPath = "";

            if (node == null)
                return 0;

            strFullPath = node.Text;
            int nDepth = 1;

            while (node.Parent != null)
            {
                node = node.Parent;
                strFullPath = node.Text + "/" + strFullPath;
                nDepth++;
            }

            return nDepth;
        }

        public TreeNode FindNode(string strPath)
        {
            string[] strNode = strPath.Split('/');
            //int nIndex = strNode.Length;

            //TreeNode[] node = treeViewDisaster.Nodes.Find(strNode[nIndex - 2], true);
            //int n = node.Length;

            //treeViewDisaster.SelectedNode = node;

            foreach (TreeNode root in treeViewDisaster.Nodes)
            {
                if (strNode[0] == root.Text)
                {
                    foreach (TreeNode nodeLevel1 in root.Nodes)
                    {
                        if (strNode[1] == nodeLevel1.Text)
                        {
                            foreach (TreeNode nodeLevel2 in nodeLevel1.Nodes)
                            {
                                if (strNode[2] == nodeLevel2.Text)
                                {
                                    treeViewDisaster.SelectedNode = nodeLevel2;
                                    treeViewDisaster.Refresh();
                                    return nodeLevel2;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        public void ClearSelection()
        {
            if (treeViewDisaster.SelectedNode != null)
                treeViewDisaster.SelectedNode.ForeColor = Color.Black;
            treeViewDisaster.SelectedNode = null;
        }
    }

    public class VersionData
    {
        private string m_strVersionName = "";
        private string m_strOwner = "";
        private DateTime m_dtCreate;
        private DateTime m_dtLastAccess;
        private string m_strDescription = "";

        public VersionData(string strVersionName, string strOwner, DateTime dtCreate, DateTime dtLastAccess, string strDescription)
        {
            m_strVersionName = strVersionName;
            m_strOwner = strOwner;
            m_dtCreate = dtCreate;
            m_dtLastAccess = dtLastAccess;
            m_strDescription = strDescription;
        }

        public string VersionName
        {
            get { return m_strVersionName; }
            set { m_strVersionName = value; }
        }

        public string Owner
        {
            get { return m_strOwner; }
            set { m_strOwner = value; }
        }

        public DateTime CreateTime
        {
            get { return m_dtCreate; }
            set { m_dtCreate = value; }
        }

        public DateTime LastAccessTime
        {
            get { return m_dtLastAccess; }
            set { m_dtLastAccess = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }

    public class SubDisasterCategoryData
    {
        private int m_nID = -1;
        private int m_nDisasterID = -1;
        private string m_strSubCategoryName = "";
        private int m_nParentSubCategoryID = -1;

        public SubDisasterCategoryData()
        {
        }

        public SubDisasterCategoryData(int nID, int nDisasterID, string strSubCategoryName, int nParentSubCategoryID)
        {
            m_nID = nID;
            m_nDisasterID = nDisasterID;
            m_strSubCategoryName = strSubCategoryName;
            m_nParentSubCategoryID = nParentSubCategoryID;
        }

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }

        public string SubCategoryName
        {
            get { return m_strSubCategoryName; }
            set { m_strSubCategoryName = value; }
        }

        public int ParentSubCategoryID
        {
            get { return m_nParentSubCategoryID; }
            set { m_nParentSubCategoryID = value; }
        }
    }
}
