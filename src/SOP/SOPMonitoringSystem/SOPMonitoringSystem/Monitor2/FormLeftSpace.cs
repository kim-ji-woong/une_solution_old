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
        private FormMain m_fromMain = null;

        ArrayList m_arrSite = new ArrayList();
        ArrayList m_arrBuildingGroup = new ArrayList();
        ArrayList m_arrBuilding = new ArrayList();

        private string m_strSelectSpace;

        public string SelectSpace
        {
            get { return m_strSelectSpace; }
            set { m_strSelectSpace = value; }
        }

        public FormLeftSpace(FormMain main)
        {
            InitializeComponent();

            m_fromMain = main;

            Init();
        }

        public void Init()
        {
            GetSite(ref m_arrSite);
            GetBuildingGroup(ref m_arrBuildingGroup);
            GetBuilding(ref m_arrBuilding);
            
            SetSpaceTree();

            treeViewSearch.Hide();
        }

        public void GetSite(ref ArrayList arrSite)
        {
            SOPMonitoringSystem.WebDBManager dbMgr = m_fromMain.GetMain().GetDBManager();

            string strSQL = "SELECT * FROM Site";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            for (int i=0;i<arrResult.Count-1;i+=2)
            {
                SOPMonitoringSystem.Data_Site dataNew = new SOPMonitoringSystem.Data_Site();

                dataNew.SiteID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                dataNew.SiteName = dbMgr.GetStringField(arrResult[i+1], "");
                arrSite.Add(dataNew);
            }
        }

        public void GetBuildingGroup(ref ArrayList arrBuilding)
        {
            SOPMonitoringSystem.WebDBManager dbMgr = m_fromMain.GetMain().GetDBManager();

            string strSQL = "SELECT * FROM BuildingGroup";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            for (int i=0;i<arrResult.Count-2;i+=3)
            {
                SOPMonitoringSystem.Data_BuildingGroup dataNew = new SOPMonitoringSystem.Data_BuildingGroup();
                dataNew.GroupID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                dataNew.GroupName = dbMgr.GetStringField(arrResult[i + 1].ToString(), "");
                dataNew.SiteID = dbMgr.GetIntField(arrResult[i+2].ToString(), 0);
                arrBuilding.Add(dataNew);
            }
        }

        public void GetBuilding(ref ArrayList arrBuilding)
        {
            SOPMonitoringSystem.WebDBManager dbMgr = m_fromMain.GetMain().GetDBManager();

            string strSQL = "SELECT * FROM Building";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            for (int i=0;i<arrResult.Count-4;i+=5)
            {
                SOPMonitoringSystem.Data_Building dataNew = new SOPMonitoringSystem.Data_Building();
                dataNew.BuildingID = dbMgr.GetIntField(arrResult[i].ToString(), 0);
                dataNew.BuildingName = dbMgr.GetStringField(arrResult[i + 1].ToString(), "");
                dataNew.BuildingGroupID = dbMgr.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.MaxFloor = dbMgr.GetIntField(arrResult[i + 3].ToString(), 0);
                if(arrResult[i + 4].ToString() == "")
                    dataNew.MinFloor = 0;
                else
                    dataNew.MinFloor = dbMgr.GetIntField(arrResult[i + 4].ToString(), 0);
                arrBuilding.Add(dataNew);
                //tsTextSearch.AutoCompleteCustomSource.Add(dataNew.BuildingName);
            }
        }

        public void SetSpaceTree()
        {
            if (m_arrBuildingGroup.Count == 0) return;

            for (int i = 0; i < m_arrBuildingGroup.Count; i++)
            {
                SOPMonitoringSystem.Data_BuildingGroup data = (SOPMonitoringSystem.Data_BuildingGroup)m_arrBuildingGroup[i];
                treeSpace.Nodes.Add(data.GroupName);

                foreach (SOPMonitoringSystem.Data_Building dataBuilding in m_arrBuilding)
                {
                    if (data.GroupID == dataBuilding.BuildingGroupID)
                        treeSpace.Nodes[i].Nodes.Add(dataBuilding.BuildingName);
                }
            }

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
                m_fromMain.GetSituation().SetEquipmentTree(SelectSpace);
                m_fromMain.ChangeFloor(1);
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
            foreach (SOPMonitoringSystem.Data_Building data in m_arrBuilding)
            {
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
    }
}
