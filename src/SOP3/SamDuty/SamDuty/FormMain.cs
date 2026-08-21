using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SamDuty
{
    public partial class FormMain : Form
    {
        static private FormMain m_Instance = null;
        static public FormMain Instance
        {
            get { return m_Instance; }
        }

        private DataManager m_DataMan = new DataManager();
        private ArrayList mCheckedTeam = new ArrayList();
        private ArrayList m_arTeamRoots = new ArrayList();

        private DateTime m_StartTime;
        private DateTime m_EndTime;
        public FormMain()
        {
            if (m_Instance == null)
                m_Instance = this;

            InitializeComponent();
            treeView1.CheckBoxes = true;
            lbDate.ForeColor = Color.Blue;
 
            
        }


        private void FindRootNode(TreeNode rootNode, ArrayList teamList)
        {
            DataTeam rootTeam = null;
            foreach (DataTeam team in teamList)
            {
                if (team.ParentTeamID == -1)
                {
                    rootTeam = team;
                    rootNode.Text = team.TeamName;
                    rootNode.Tag = team;                           
                }
            }
            if (rootTeam != null)
            {
                teamList.Remove(rootTeam);
            }
        }

        //private void MakeExternalNodes(ArrayList teamList)
        //{
        //    foreach (DataTeam team in teamList)
        //    {
        //        if (team.ParentTeamID == -1)
        //        {
        //            TreeNode node = new TreeNode(team.TeamName);
        //            treeView1.Nodes.Add(node);
        //            node.Tag = team;
        //            m_arTeamRoots.Add(node);
        //            ArrayList arChild = (ArrayList)teamList.Clone();
        //            arChild.Remove(team);
        //            MakeTreeView(node, arChild);
        //        }
        //    }
        //}

        private void MakeTreeView(TreeNode nodeParent, ArrayList teamList)
        {
            if (teamList.Count == 0)
                return;

            DataTeam rootTeam = (DataTeam)nodeParent.Tag;
            foreach (DataTeam team in teamList)
            {
                if (team.ParentTeamID == rootTeam.ID)
                {
                    TreeNode node = new TreeNode(team.TeamName);
                    node.Tag = team;
                    nodeParent.Nodes.Add(node);

                    MakeTreeView(node, team);

                    ArrayList arChild = (ArrayList)teamList.Clone();
                    arChild.Remove(team);
                    MakeTreeView(node, teamList);
                }
            }
        }
        private void MakeTreeView(TreeNode nodeParent, DataTeam team)
        {
            ArrayList arTemp = new ArrayList();
            arTemp.Add(team);

            ArrayList arMember = m_DataMan.GetTargetMemberTeam(true, true, arTemp);
            foreach (SendingMember member in arMember)
            {
                TreeNode node = new TreeNode(member.Name);
                node.Tag = member;
                node.ForeColor = Color.Green;               
                nodeParent.Nodes.Add(node);
            }
        }
   

        private void FormMain_Load(object sender, EventArgs e)
        {
            TreeNode root = treeView1.Nodes[0]; // 삼천포 root
            m_arTeamRoots.Add(root);
            ArrayList arRegular = (ArrayList)m_DataMan.RegularTeamList.Clone();
            FindRootNode(root, arRegular);
            MakeTreeView(root, arRegular);

            LoadMembers();

            timer1.Enabled = true;
            timer1.Start();
        }

        private void GetCheckedTeam(TreeNode pNode, ref ArrayList arResult)
        {
            foreach (TreeNode node in pNode.Nodes)
            {
                if (node.Checked == true)
                {
                    arResult.Add(node);                    
                }
                GetCheckedTeam(node, ref arResult);
            } 
        }

        private ArrayList m_arSelectedList = new ArrayList();
        private void SetLabel(TreeNode node, bool bAdd)
        {
            if( node == null)
            {                
                return;
            }

            txtSearch.Text = "";

            if (m_arSelectedList.Contains(node) && bAdd == false)
            {
                m_arSelectedList.Remove(node);
                RemoveMembers((SendingMember)node.Tag);
                return;
            }
            else if (!m_arSelectedList.Contains(node) && bAdd == true)
            {
                m_arSelectedList.Add(node);
                AddMember((SendingMember)node.Tag);
            }
            else
            {
                return;
            }  
        }


        private TreeNode m_preSelectNode = null;
        private void treeView1_AfterCheck_1(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            
            if (e.Action == TreeViewAction.Unknown)
                return;

            object obj = e.Node.Tag;
            if (obj.GetType() != typeof(SendingMember))
            {
                e.Node.Checked = false;
                if (e.Node.IsExpanded == true)
                    node.ExpandAll();
                else
                    node.Collapse();
                return;
            }

            //treeView1.SelectedNode = node;
            if (node != null && node.Checked == true) // 체크 이벤트인 경우
            {
                SetLabel(node, true);
                //node.ForeColor = Color.Red;
                treeView1.SelectedNode = node;

                node.ExpandAll();
            }

            else if (node != null && node.Checked == false)
            {

                SetLabel(node, false);

                node.Collapse(false);
            }
        }

        private void InitToTeam()
        {
            foreach (TreeNode node in m_arTeamRoots)
            {
                node.Checked = false;
            }
            mCheckedTeam.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string szName = txtSearch.Text;
            TreeNode root = treeView1.Nodes[0];
            TreeNode node = FindNode(root, szName);
            if (node != null)
            {                
                treeView1.SelectedNode = node;
                node.Checked = true;
                //node.ForeColor = Color.Red;
                SetLabel(node, true);
                
            }
            txtSearch.Text = "";
        }

        private TreeNode FindNode(TreeNode node, string szName)
        {
            object obj = node.Tag;
            if (obj.GetType() == typeof(SendingMember))
            {
                SendingMember member = (SendingMember)obj;

                if (member.Name == szName)
                {
                    return node;
                }
            }

            if (node.Nodes.Count == 0)
                return null;

            foreach (TreeNode n in node.Nodes)
            {
                TreeNode c = FindNode(n, szName);
                if (c != null)
                    return c;
            }

            return null;
        }

        private TreeNode FindNode(TreeNode node, int nMemberID , string szTeamName)
        {
            object obj = node.Tag;
            if (obj.GetType() == typeof(SendingMember))
            {
                SendingMember member = (SendingMember)obj;

                if (member.MemberID == nMemberID && member.TeamName == szTeamName)
                {
                    return node;
                }
            }

            if (node.Nodes.Count == 0)
                return null;

            foreach (TreeNode n in node.Nodes)
            {
                TreeNode c = FindNode(n, nMemberID, szTeamName);
                if (c != null)
                    return c;
            }

            return null;
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
       
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1_Click(null, null);
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
			// 설정된 당직자가 없는 경우 기존 당직자를 삭제한다. (삼천포 요구사항) skkim 2014-02-20
            //if (m_arSelectedList.Count == 0)
            //    return;

            string szSQL1 = "DELETE FROM Duty";
            ArrayList arResult = m_DataMan.DBManager.GetResultData(szSQL1, 0);

            int nCount = 1;
            foreach (TreeNode node in m_arSelectedList)
            {
                SendingMember member = (SendingMember)node.Tag;
                DataCompanyMember mem = member.Member;

                DateTime nCur = DateTime.Now;
                int nID = mem.ID;

                m_DataMan.SetNightDuty(nCount++, nID, mem.RegularTeamID, nCur);
            }            
        }    

        private void timer1_Tick(object sender, EventArgs e)
        {
            lbDate.Text = DateTime.Now.ToLongDateString();
        }

        private void AddMember(SendingMember member)
        {
            int nID = 1;
            
            if (dataGridViewDuty.Rows.Count > 0)
            {
                nID = (int)dataGridViewDuty.Rows[dataGridViewDuty.Rows.Count - 1].Cells[0].Value + 1;
            }

            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = nID;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = member;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Value = member.TeamName;
            row.Cells.Add(cell);

            dataGridViewDuty.Rows.Add(row);
        }

        private void RemoveMembers(SendingMember member)
        {
            bool removed = false;
            int nMemberCount = dataGridViewDuty.Rows.Count;

            for (int i = 0; i < nMemberCount; i++)
            {
                if (removed)
                {
                    dataGridViewDuty.Rows[i].Cells[0].Value = i + 1;
                }
                else
                {
                    DataGridViewRow row = dataGridViewDuty.Rows[i];
                    SendingMember _member = (SendingMember)row.Cells[1].Value;

                    if (member == _member)
                    {
                        dataGridViewDuty.Rows.RemoveAt(i);
                        
                        removed = true;
                        i--;
                        nMemberCount--;
                    }
                }
            }
        }

        private void LoadMembers()
        {
            WebDBManager dbMgr = m_DataMan.DBManager;

            string strSQL = "Select ID, MemberID, TeamID, InsertTime from Duty";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtDefault = new DateTime();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nMemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                DateTime dtInsert = WebDBManager.GetDateTimeField(arrResult[i + 3], dtDefault);

                SendingMember member = new SendingMember();

                member.MemberID = nMemberID;
                member.Member = m_DataMan.GetCompanyMember(nMemberID);

                if (member.Member == null)
                    continue;

                DataTeam team = m_DataMan.GetRegularTeam(nTeamID);
                if (team == null)
                    continue;

                member.Name = member.Member.MemberName;
                member.PhoneNumber = member.Member.PhoneNumber;
                member.TeamName = team.TeamName;

                TreeNode root = treeView1.Nodes[0];
                TreeNode node = FindNode(root, nMemberID, team.TeamName);
                if (node != null)
                {
                    node.Checked = true;
                    treeView1.SelectedNode = node;
                    node.Checked = true;
                    SetLabel(node, true);
                }
                //AddMember(member);
            }
        }
    }
}
