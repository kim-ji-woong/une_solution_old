using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPManager
{
    public partial class PopupSelectTeam3 : Form
    {
        // 0(평일), 1(휴일), 2(외부 기관), 3(사용자 정의 조직)
        private int m_nCurrentTeamType = -1;
        private int m_nSelectedTeamID = -1;
        private string m_strSelectedTeamName = "";
        private ArrayList m_arrExceptTeams = null;

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        // arrExceptTeams : 선택에서 제외시킬 Team List(long)
        //                  상위 4바이트(TeamID), 하위 4바이트(TeamType)
        public PopupSelectTeam3(int nTeamType, ArrayList arrExceptTeams)
        {
            InitializeComponent();

            m_nCurrentTeamType = nTeamType;
            m_arrExceptTeams = arrExceptTeams;

            InitTree();
        }

        public void InitTree()
        {
            treeViewTeam.Nodes.Clear();

            int nTeamType = m_nCurrentTeamType;

            SetTeamTypeLabel(nTeamType);

            if (nTeamType == 2)         // 외부 조직
                LoadExternalTeamTree();
            else if (nTeamType == 3)    // 사용자 정의 조직
                LoadUserDefinedTeamTree();
            else if (nTeamType == 0)    // 평일 비상 조직
                LoadTemporaryNormalTeamTree();
            else if (nTeamType == 1)    // 야간 및 휴일 비상 조직
                LoadTemporaryEmergencyTeamTree();
            else if (nTeamType == 4)    // 정규 조직
                LoadRegularTeamTree();
        }

        private void LoadRegularTeamTree()
        {
            ArrayList arrRegularTeam = FormMain.Instance.RegularTeam;

            foreach (Data_RegularTeam data in arrRegularTeam)
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

            treeViewTeam.ExpandAll();
        }

        private void SetTeamTypeLabel(int nTeamType)
        {
            if (nTeamType == 0)
                labelTeamType.Text = "평일 비상 조직";
            else if (nTeamType == 1)
                labelTeamType.Text = "야간 및 휴일 비상 조직";
            else if (nTeamType == 2)
                labelTeamType.Text = "외부 기관";
            else if (nTeamType == 3)
                labelTeamType.Text = "사용자 정의 조직";
            else if (nTeamType == 4)
                labelTeamType.Text = "정규 조직";
        }

        private bool IsExceptTeam(int nTeamID, int nTeamType)
        {
            if (m_arrExceptTeams == null)
                return false;

            foreach (long nTeamData in m_arrExceptTeams)
            {
                int teamID = (int)(nTeamData >> 32);
                int teamType = (int)(nTeamData & 0xffffffff);

                if (nTeamID == teamID && nTeamType == teamType)
                    return true;
            }

            return false;
        }

        private void LoadExternalTeamTree()
        {
            ArrayList arrExternalTeam = FormMain.Instance.ExternalTeam;

            foreach (Data_ExternalTeam data in arrExternalTeam)
            {
                TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                node.Tag = data.ID;

                if (IsExceptTeam(data.ID, m_nCurrentTeamType))
                    node.ForeColor = System.Drawing.Color.Red;

                treeViewTeam.ExpandAll();
            }
        }

        private void LoadUserDefinedTeamTree()
        {
            ArrayList arrUserDefinedTeam = FormMain.Instance.UserDefinedTeam;

            foreach (Data_UserDefinedTeam data in arrUserDefinedTeam)
            {
                TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                node.Tag = data.ID;

                if (IsExceptTeam(data.ID, m_nCurrentTeamType))
                    node.ForeColor = System.Drawing.Color.Red;

                treeViewTeam.ExpandAll();
            }
        }

        private void LoadTemporaryNormalTeamTree()
        {
            ArrayList arrRegularTeam = FormMain.Instance.TemporaryNormalTeam;

            foreach (Data_NormalTeam data in arrRegularTeam)
            {
                if (data.ParentTeamID == 0)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;

                    if (IsExceptTeam(data.ID, m_nCurrentTeamType))
                        node.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                    if (child == null) return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data.ID;

                    if (IsExceptTeam(data.ID, m_nCurrentTeamType))
                        newNode.ForeColor = System.Drawing.Color.Red;
                }
            }

            treeViewTeam.ExpandAll();
        }

        private void LoadTemporaryEmergencyTeamTree()
        {
            ArrayList arrEmergencyTeam = FormMain.Instance.TemporaryEmergencyTeam;

            foreach (Data_EmergencyTeam data in arrEmergencyTeam)
            {
                if (data.ParentTeamID == 0)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data.ID;

                    if (IsExceptTeam(data.ID, m_nCurrentTeamType))
                        node.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                    if (child == null) return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data.ID;

                    if (IsExceptTeam(data.ID, m_nCurrentTeamType))
                        newNode.ForeColor = System.Drawing.Color.Red;
                }
            }

            treeViewTeam.ExpandAll();
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

        private void btnChangeTeam_Click(object sender, EventArgs e)
        {
            PopupSelectTeam2 frm = new PopupSelectTeam2(m_nCurrentTeamType, true);

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_nCurrentTeamType = frm.SelectedTeamType;

                SetTeamTypeLabel(m_nCurrentTeamType);
                InitTree();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (treeViewTeam.SelectedNode == null)
            {
                MessageBox.Show("팀을 선택하지 않았습니다.");
            }
            else
            {
                if (treeViewTeam.SelectedNode.ForeColor == System.Drawing.Color.Red)
                {
                    MessageBox.Show("이미 존재하는 팀입니다.\r\n다른 팀을 선택하세요");
                    return;
                }

                m_nSelectedTeamID = (int)treeViewTeam.SelectedNode.Tag;
                m_strSelectedTeamName = treeViewTeam.SelectedNode.Text;

                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        public int SelectedTeamID
        {
            get { return m_nSelectedTeamID; }
        }

        public int SelectedTeamType
        {
            get { return m_nCurrentTeamType; }
        }

        public string SelectedTeamName
        {
            get { return m_strSelectedTeamName; }
        }

        private void PopupSelectTeam3_MouseDown(object sender, MouseEventArgs e)
        {
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupSelectTeam3_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void PopupSelectTeam3_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }
    }
}
