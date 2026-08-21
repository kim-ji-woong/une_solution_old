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
using System.Text.RegularExpressions;

namespace SOPDisasterSystem
{
    public partial class FormLeftSpace : Form
    {
        private FormMain m_frmMain = null;

        ArrayList m_arrSite = new ArrayList();
        //ArrayList m_arrBuildingGroup = new ArrayList();
        //ArrayList m_arrBuilding = new ArrayList();
        // Building ID별 Building 데이터
        Dictionary<int, SOPMonitoringSystem.Data_Building> m_dicBuildingID = new Dictionary<int, SOPMonitoringSystem.Data_Building>();
        // Building Code별 Building 데이터
        Dictionary<string, SOPMonitoringSystem.Data_Building> m_dicBuildingCode = new Dictionary<string, SOPMonitoringSystem.Data_Building>();
        Dictionary<int, SOPMonitoringSystem.Data_BuildingGroup> m_dicBuidlingGroup = new Dictionary<int, SOPMonitoringSystem.Data_BuildingGroup>();
        SOPMonitoringSystem.Data_Building m_buildingSelected = null;

        private string m_strSelectSpace;

        public string SelectSpace
        {
            get { return m_strSelectSpace; }
            set { m_strSelectSpace = value; }
        }

        public FormLeftSpace(FormMain main)
        {
            InitializeComponent();

            m_frmMain = main;

            Init();
        }

        public void Init()
        {
            GetSite(ref m_arrSite);
            string strBuildingGroupIDs = GetBuildingGroup();
            GetBuilding(strBuildingGroupIDs);
            
            SetSpaceTree();

            treeViewSearch.Hide();
        }

        public void GetSite(ref ArrayList arrSite)
        {
            SOPMonitoringSystem.WebDBManager dbMgr = m_frmMain.GetMain().DBManager;

            string strSQL = "SELECT ID, SiteName FROM Site";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            for (int i = 0 ; i < arrResult.Count - 1; i += 2)
            {
                SOPMonitoringSystem.Data_Site dataNew = new SOPMonitoringSystem.Data_Site();

                dataNew.SiteID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.SiteName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 1], "");
                arrSite.Add(dataNew);
            }
        }

        public string GetBuildingGroup()
        {
            if (m_arrSite.Count == 0)
                return "";

            string strBuildingGroupIDs = "";
            SOPMonitoringSystem.Data_Site site = (SOPMonitoringSystem.Data_Site)m_arrSite[0];

            SOPMonitoringSystem.WebDBManager dbMgr = m_frmMain.GetMain().DBManager;

            string strSQL = "SELECT ID, GroupName, SiteID FROM BuildingGroup where SiteID = " + site.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            for (int i=0;i<arrResult.Count-2;i+=3)
            {
                SOPMonitoringSystem.Data_BuildingGroup dataNew = new SOPMonitoringSystem.Data_BuildingGroup();
                dataNew.GroupID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.GroupName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                dataNew.SiteID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.SiteName = site.SiteName;

                m_dicBuidlingGroup[dataNew.GroupID] = dataNew;

                if (strBuildingGroupIDs.Length == 0)
                    strBuildingGroupIDs = dataNew.GroupID.ToString();
                else
                    strBuildingGroupIDs += ", " + dataNew.GroupID.ToString();
            }

            return strBuildingGroupIDs;
        }

        public void GetBuilding(string strBuidingGroupIDs)
        {
            SOPMonitoringSystem.WebDBManager dbMgr = m_frmMain.GetMain().DBManager;

            string strSQL = string.Format("SELECT ID, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor, BroadCastingText FROM Building where BuildingGroupID in ({0})",
                strBuidingGroupIDs);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            for (int i=0;i<arrResult.Count-7;i +=8)
            {
                SOPMonitoringSystem.Data_Building dataNew = new SOPMonitoringSystem.Data_Building();
                dataNew.ID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.BuildingID = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 1], "");
                dataNew.BuildingCode = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 2], "");
                dataNew.BuildingName = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                int nBuildingGroupID = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                dataNew.MaxFloor = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                dataNew.MinFloor = SOPMonitoringSystem.WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                dataNew.BroadCastingText = SOPMonitoringSystem.WebDBManager.GetStringField(arrResult[i + 7].ToString(), "");

                if (!m_dicBuidlingGroup.ContainsKey(nBuildingGroupID))
                    continue;

                dataNew.BuildingGroup = m_dicBuidlingGroup[nBuildingGroupID];

                m_dicBuildingID[dataNew.ID] = dataNew;
                m_dicBuildingCode[dataNew.BuildingCode] = dataNew;
                //tsTextSearch.AutoCompleteCustomSource.Add(dataNew.BuildingName);
            }
        }

        public TreeNode FindBuildingGroupNode(string strBuildingGroupName)
        {
            foreach (TreeNode node in treeSpace.Nodes)
            {
                if (node.Text == strBuildingGroupName)
                    return node;
            }

            return null;
        }

        public SOPMonitoringSystem.Data_Building GetSelectedBuilding()
        {
            return m_buildingSelected;
        }

        private void SetSpaceTree()
        {
            foreach (KeyValuePair<int, SOPMonitoringSystem.Data_Building> pair in m_dicBuildingID)
            {
                SOPMonitoringSystem.Data_Building building = pair.Value;
                TreeNode nodeGroup = FindBuildingGroupNode(building.BuildingGroup.GroupName);

                if (nodeGroup == null)
                {
                    nodeGroup = treeSpace.Nodes.Add(building.BuildingGroup.GroupName);
                    nodeGroup.Tag = building.BuildingGroup;
                }

                TreeNode nodeBuilding = nodeGroup.Nodes.Add(building.BuildingName);
                nodeBuilding.Tag = building;

                if (m_buildingSelected == null)
                {
                    m_buildingSelected = building;
                    treeSpace.SelectedNode = nodeBuilding;
                }
            }

            /*if (m_arrBuildingGroup.Count == 0) return;

            for (int i = 0; i < m_arrBuildingGroup.Count; i++)
            {
                SOPMonitoringSystem.Data_BuildingGroup data = (SOPMonitoringSystem.Data_BuildingGroup)m_arrBuildingGroup[i];
                treeSpace.Nodes.Add(data.GroupName);

                foreach (SOPMonitoringSystem.Data_Building dataBuilding in m_arrBuilding)
                {
                    if (data.GroupID == dataBuilding.BuildingGroupID)
                        treeSpace.Nodes[i].Nodes.Add(dataBuilding.BuildingName);
                }
            }*/

            treeSpace.ExpandAll();
        }

        private void treeSpace_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                contextSpaceTree.Show(treeSpace, new Point(e.X, e.Y));
            }
        }

        private void treeSpace_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeSpace.SelectedNode;
            if (node == null) return;

            node.ForeColor = Color.Red;

            SelectSpace = node.FullPath;
            if(treeSpace.SelectedNode.Level != 0)
            {
                m_buildingSelected = (SOPMonitoringSystem.Data_Building)treeSpace.SelectedNode.Tag;
                m_frmMain.GetSituation().SetEquipmentTree(SelectSpace);
                //m_frmMain.ChangeFloor(1);
                m_frmMain.SetCurrentBuilding(m_buildingSelected);
            }
        }

        private void treeSpace_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = treeSpace.SelectedNode;
            if (node == null) return;

            node.ForeColor = Color.Black;
        }

        private void tsbtnSearch_Click(object sender, EventArgs e)
        {
            GetSpaceSearch();
        }

        private void tsbtnTree_Click(object sender, EventArgs e)
        {
            treeViewSearch.Hide();
        }

        private void treeViewSearch_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeViewSearch.SelectedNode;
            if (node == null) return;

            node.ForeColor = Color.Red;
        }

        private void treeViewSearch_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = treeViewSearch.SelectedNode;
            if (node == null) return;

            node.ForeColor = Color.Black;
        }

        private void tsTextSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GetSpaceSearch();
            }
        }

        private void GetSpaceSearch()
        {
            Point pt = treeSpace.Location;
            treeViewSearch.Location = new Point(pt.X, pt.Y);

            Size sz = treeSpace.Size;
            treeViewSearch.Size = new Size(sz.Width, sz.Height);

            treeViewSearch.Nodes.Clear();
            treeViewSearch.Nodes.Add("");

            int nCount = 0;
            //foreach (SOPMonitoringSystem.Data_Building data in m_arrBuilding)
            foreach (KeyValuePair<int, SOPMonitoringSystem.Data_Building> pair in m_dicBuildingID)
            {
                SOPMonitoringSystem.Data_Building data = pair.Value;

                if (tsTextSearch.Text == "")
                {
                    treeViewSearch.Nodes[0].Nodes.Add(data.BuildingName);
                }
                else
                {
                    int nIndex = data.BuildingName.IndexOf(tsTextSearch.Text);
                    if (!(nIndex < 0))
                    {
                        treeViewSearch.Nodes[0].Nodes.Add(data.BuildingName);
                        nCount++;
                    }
                }
            }

            if (tsTextSearch.Text != "")
                treeViewSearch.Nodes[0].Text = nCount.ToString() + "개 일치";
            else
                treeViewSearch.Nodes[0].Text = "전체 공간";

            treeViewSearch.Show();
            treeViewSearch.ExpandAll();
        }

        public bool SelectItem(string strNodeName, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeSpace.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if (node.FullPath == strNodeName)
                {
                    treeSpace.SelectedNode = node;
                    treeViewSearch_AfterSelect(null, null);
                    return true;
                }

                bool isSelected = SelectItem(strNodeName, node.Nodes);
                if (isSelected)
                    return true;
            }

            return false;
        }

        public void OnEnabled(bool isFlag)
        {
            tsTextSearch.ReadOnly = !isFlag;
            tsbtnSearch.Enabled = isFlag;
            tsbtnTree.Enabled = isFlag;
            treeSpace.Enabled = isFlag;
            treeViewSearch.Enabled = isFlag;
        }
    }
}
