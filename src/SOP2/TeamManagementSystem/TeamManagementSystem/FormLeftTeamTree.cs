using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace TeamManagementSystem
{
    public partial class FormLeftTeamTree : Form
    {
        private FormMain m_Main = null;

        public FormLeftTeamTree(FormMain main)
        {
            InitializeComponent();
            m_Main = main;

            Init();
            treeViewResize(2);
        }

        public void Init()
        {
            if (!m_Main.IsOpen)
                m_Main.SetAutoAlignRefresh(0, false);
            m_Main.SetAutoAlignRefresh(1, false);
            m_Main.SetAutoAlignRefresh(2, false);

            RemoveData();

            GetRegularTeamInfo(false);
            GetEmergency(false);

            if (!m_Main.IsOpen)
                m_Main.AddRowData(0);
            m_Main.AddRowData(1);
            m_Main.AddRowData(2);

            if (!m_Main.IsOpen)
            {
                m_Main.AllSectionClearSelect(0);
                m_Main.AutoAlign(true, m_Main.GetSections(0));
                m_Main.SetAutoAlignRefresh(0, true);
            }

            m_Main.AllSectionClearSelect(1);
            m_Main.AutoAlign(true, m_Main.GetSections(1));
            m_Main.SetAutoAlignRefresh(1, true);

            m_Main.AllSectionClearSelect(2);
            m_Main.AutoAlign(true, m_Main.GetSections(2));
            m_Main.SetAutoAlignRefresh(2, true);
        }

        public void RemoveData()
        {
            treeViewTeam.Nodes.Clear();
            treeViewEmergency.Nodes.Clear();
        }

        private TreeNode FindNode(int nTag, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewTeam.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if ((int)node.Tag == nTag)
                    return node;

                TreeNode result = FindNode(nTag, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        private int AddTree(ArrayList arrInfo)
        {
            int nTeamCount = arrInfo.Count;

            for (int i = 0; i < nTeamCount; i++)
            {
                Data_RegularTeam data = (Data_RegularTeam)arrInfo[i];

                if (data.ParentTeamID == 0)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                    if (child == null) continue;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data.ID;
                }

                arrInfo.RemoveAt(i);
                i--;
                nTeamCount--;

                m_Main.AddSection(0, data.ID, data.ParentTeamID, data.TeamName);  
            }

            return nTeamCount;
        }

        private void GetRegularTeamInfo(bool autoAlign = true)
        {
            ArrayList arrInfo = new ArrayList();

            foreach (Data_RegularTeam data in m_Main.RegularTeam)
                arrInfo.Add(data);
            //arrInfo = m_Main.RegularTeam;

            int nTeamCount = arrInfo.Count;

            while (nTeamCount > 0)
            {
                int nChangedTeamCount = AddTree(arrInfo);

                if (nTeamCount == nChangedTeamCount)
                    return;

                nTeamCount = nChangedTeamCount;
            }

            /*foreach (Data_RegularTeam data in arrInfo)
            {
                //if (!m_Main.IsOpen)
                {
                    if (data.ParentTeamID == 0)
                    {
                        TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                        node.Tag = data.ID;
                    }
                    else
                    {
                        TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                        if (child == null) return;

                        TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                        newNode.Tag = data.ID;
                    }
                }
                m_Main.AddSection(0, data.ID, data.ParentTeamID, data.TeamName);  
            }*/

            if (autoAlign)
            {
                m_Main.AutoAlign(true, m_Main.GetSections(0));
                m_Main.SetAutoAlignRefresh(0, true);
            }

            //m_Main.AllSectionClearSelect();
            //m_Main.AutoAlign(true);

            treeViewTeam.ExpandAll();
        }

        private TreeNode FindNode_Emergency(int nTag, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewEmergency.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                if ((int)node.Tag == nTag)
                    return node;

                TreeNode result = FindNode_Emergency(nTag, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }
        
        private void GetEmergency(bool autoAlign = true)
        {
            ArrayList arrNormal = new ArrayList();
            arrNormal = m_Main.NormalTeam;
            ArrayList arrEmergency = new ArrayList();
            arrEmergency = m_Main.EmergencyTeam;

            treeViewEmergency.Nodes.Add("비상조직-평일");
            treeViewEmergency.Nodes.Add("비상조직-야간 및 휴일");

            // PrevID를 저장하여 History ID 대신 원래 ID 사용토록 한다.
            m_Main.TempNormalHistoryID = 0;
            bool isChecked = false;

            foreach (Data_NormalHistory data in arrNormal)
            {
                //if(data.TeamVersionName == m_Main.VersionName)
                if (data.TeamVersionID == m_Main.VersionID)
                {
                    isChecked = true;
                    int nParentTeamID = data.ParentTeamID == 0 ? 0 : data.ParentTeamID - m_Main.TempNormalHistoryID;

                    //if (!m_Main.IsOpen)
                    {
                        if (nParentTeamID == 0)
                        {
                            TreeNode node = treeViewEmergency.Nodes[0].Nodes.Add(data.TeamName.TrimEnd());
                            node.Tag = data.ID - m_Main.TempNormalHistoryID;
                        }
                        else
                        {
                            TreeNode child = FindNode_Emergency(nParentTeamID, treeViewEmergency.Nodes[0].Nodes);
                            if (child == null) return;

                            TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                            newNode.Tag = data.ID - m_Main.TempNormalHistoryID;
                        }
                    }
                    m_Main.AddSection(1, data.ID - m_Main.TempNormalHistoryID, nParentTeamID, data.TeamName);
                }
                else
                {
                    if (!isChecked)
                        m_Main.TempNormalHistoryID = data.ID;
                }
            }

            if (autoAlign)
            {
                m_Main.AutoAlign(true, m_Main.GetSections(1));
                m_Main.SetAutoAlignRefresh(1, true);
            }

            // PrevID를 저장하여 History ID 대신 원래 ID 사용토록 한다.
            m_Main.TempEmergencyHistoryID = 0;
            isChecked = false;

            foreach (Data_EmergencyHistory data in m_Main.EmergencyTeam)
            {
                //if (data.TeamVersionName == m_Main.VersionName)
                if (data.TeamVersionID == m_Main.VersionID)
                {
                    isChecked = true;
                    int nParentTeamID = data.ParentTeamID == 0 ? 0 : data.ParentTeamID - m_Main.TempEmergencyHistoryID;

                    //if (!m_Main.IsOpen)
                    {
                        if (nParentTeamID == 0)
                        {
                            TreeNode node = treeViewEmergency.Nodes[1].Nodes.Add(data.TeamName.TrimEnd());
                            node.Tag = data.ID - m_Main.TempEmergencyHistoryID;
                        }
                        else
                        {
                            TreeNode child = FindNode_Emergency(nParentTeamID, treeViewEmergency.Nodes[1].Nodes);
                            if (child == null) return;

                            TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                            newNode.Tag = data.ID - m_Main.TempEmergencyHistoryID;
                        }
                    }
                    m_Main.AddSection(2, data.ID - m_Main.TempEmergencyHistoryID, nParentTeamID, data.TeamName);
                }
                else
                {
                    if (!isChecked)
                        m_Main.TempEmergencyHistoryID = data.ID;
                }
            }

            if (autoAlign)
            {
                m_Main.AutoAlign(true, m_Main.GetSections(2));
                m_Main.SetAutoAlignRefresh(2, true);
            }

            treeViewEmergency.ExpandAll();
        }

        public void treeViewResize(int nSelect)
        {
            switch(nSelect)
            {
                case 0:
                    //treeViewTeam.Visible = true;
                    //treeViewEmergency.Visible = false;

                    //treeViewEmergency.Dock = DockStyle.None;
                    //treeViewTeam.Dock = DockStyle.Fill;
                    //break;
                case 1:
                    //treeViewTeam.Visible = false;
                    //treeViewEmergency.Visible = true;

                    //treeViewTeam.Dock = DockStyle.None;
                    //treeViewEmergency.Dock = DockStyle.Fill;
                    //break;
                case 2:
                    treeViewTeam.Visible = true;
                    treeViewEmergency.Visible = true;

                    treeViewTeam.Dock = DockStyle.None;
                    treeViewEmergency.Dock = DockStyle.None;

                    treeViewTeam.Size = new Size(this.Size.Width, this.Size.Height / 2);
                    treeViewEmergency.Size = new Size(this.Size.Width, this.Size.Height / 2);
                    treeViewEmergency.Location = new Point(0, this.Size.Height / 2);
                    break;
            }
        }

        private void FormLeftTeamTree_SizeChanged(object sender, EventArgs e)
        {
            treeViewTeam.Size = new Size(this.Size.Width, this.Size.Height / 2);
            treeViewEmergency.Size = new Size(this.Size.Width, this.Size.Height / 2);
            treeViewEmergency.Location = new Point(0, this.Size.Height / 2);
        }

    }
}
