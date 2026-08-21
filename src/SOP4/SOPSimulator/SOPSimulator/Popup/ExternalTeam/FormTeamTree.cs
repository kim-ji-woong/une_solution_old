using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.SOP;

namespace SOPMonitoringSystem
{
    public partial class FormTeamTree : Form
    {

        private int nSelectedID = -1;
        public int SelectedMemberID
        {
            get { return nSelectedID; }
            set { nSelectedID = value; }
        }

        private string nSelectedName = "";
        public string SelectedName
        {
            get { return nSelectedName; }
            set { nSelectedName = value; }
        }

        private string szSelectedPhone = "";
        public string SelectedPhone
        {
            get { return szSelectedPhone; }
            set { szSelectedPhone = value; }
        }


        private Sections.SOPTeam.SOPTeamType m_nCurrentTeamType = Sections.SOPTeam.SOPTeamType.Regular;
        public Sections.SOPTeam.SOPTeamType SelectedTeamType
        {
            get { return m_nCurrentTeamType; }
            set
            {
                m_nCurrentTeamType = value;
                if (m_nCurrentTeamType == Sections.SOPTeam.SOPTeamType.Regular)
                {
                    rbBtnRegular.Checked = true;
                }
                else
                    rbBtnExternal.Checked = true;
            }
        }

        public FormTeamTree()
        {
            InitializeComponent();
        }

        private void FormTeamTree_Load(object sender, EventArgs e)
        {
            InitTree();

            ActiveControl = txtSearch;

            labelMemberPath.Text = string.Empty;
        }

        private void LoadExternalTeamTree(string strSearchMemberName)
        {
            TreeNode selectNode = null;

            ArrayList arrExternalTeam = FormSOP.Instance.SOPManager.ExternalCompanyTeams;
            foreach (ExternalCompanyTeam data in arrExternalTeam)
            {
                if (data.CompanyID == -1 || data.CompanyID == data.ID)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data;
                    foreach (ExternalCompanyMember member in data.Members)
                    {
                        if (String.IsNullOrWhiteSpace(strSearchMemberName) || member.MemberName.Contains(strSearchMemberName) == true)
                        {
                            TreeNode node2 = new TreeNode(member.MemberName.TrimEnd());
                            node2.NodeFont = new Font(treeViewTeam.Font, FontStyle.Bold);
                            node2.ForeColor = Color.Black;
                            node2.Tag = member;

                            node.Nodes.Add(node2);

                            selectNode = node2;
                        }
                    }
                }
                else
                {
                    TreeNode child = FindNode(data.CompanyID, treeViewTeam.Nodes);
                    if (child == null) return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data;
                    foreach (ExternalCompanyMember member in data.Members)
                    {
                        if (String.IsNullOrWhiteSpace(strSearchMemberName) || member.MemberName.Contains(strSearchMemberName) == true)
                        {
                            TreeNode node2 = new TreeNode(member.MemberName.TrimEnd());
                            node2.NodeFont = new Font(treeViewTeam.Font, FontStyle.Bold);
                            node2.ForeColor = Color.Black;
                            node2.Tag = member;

                            newNode.Nodes.Add(node2);
                            selectNode = node2;
                        }
                    }
                }
            }
            treeViewTeam.ExpandAll();
            treeViewTeam.SelectedNode = selectNode;
        }

        private void LoadRegularTeamTree(string strSearchMemberName)
        {
            TreeNode selectNode = null;

            List<Data_RegularTeam> arrRegularTeam = FormSOP.Instance.SOPManager.RegularTeams;
            foreach (Data_RegularTeam data in arrRegularTeam)
            {
                if (data.ParentTeamID == -1)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data;

                    ArrayList arMembers = new ArrayList();
                    if (FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(data.ID, ref arMembers))
                    {
                        foreach (Data_CompanyMember member in arMembers)
                        {
                            if (String.IsNullOrWhiteSpace(strSearchMemberName) || member.MemberName.Contains(strSearchMemberName) == true)
                            {
                                TreeNode node2 = new TreeNode(member.MemberName.TrimEnd());
                                node2.NodeFont = new Font(treeViewTeam.Font, FontStyle.Bold);
                                node2.ForeColor = Color.Black;
                                node2.Tag = member;

                                node.Nodes.Add(node2);
                                selectNode = node2;
                            }
                        }
                    }
                }
                else
                {
                    TreeNode child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);
                    if (child == null) return;

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data;

                    ArrayList arMembers = new ArrayList();
                    if (FormSOP.Instance.SOPManager.GetRegularCompanyMemberList(data.ID, ref arMembers))
                    {
                        foreach (Data_CompanyMember member in arMembers)
                        {
                            if (String.IsNullOrWhiteSpace(strSearchMemberName) || member.MemberName.Contains(strSearchMemberName) == true)
                            {
                                TreeNode node2 = new TreeNode(member.MemberName.TrimEnd());
                                node2.NodeFont = new Font(treeViewTeam.Font, FontStyle.Bold);
                                node2.ForeColor = Color.Black;
                                node2.Tag = member;

                                newNode.Nodes.Add(node2);
                                selectNode = node2;
                            }
                        }
                    }
                }
            }

            treeViewTeam.ExpandAll();
            treeViewTeam.SelectedNode = selectNode;
        }
       
        // 트리 초기화시 파라미터로 검색할 직원의 이름을 넣어 빈값을 경우를 제외하고 필터링 가능하도록 함.
        public void InitTree(string strSearchMemberName = "")
        {
            treeViewTeam.SelectedNode = null;
            treeViewTeam.Nodes.Clear();

            Sections.SOPTeam.SOPTeamType nTeamType = m_nCurrentTeamType;//panel.TeamType;
                         
            if (nTeamType == Sections.SOPTeam.SOPTeamType.External)         // 외부 조직
                LoadExternalTeamTree(strSearchMemberName);          
            else if (nTeamType == Sections.SOPTeam.SOPTeamType.Regular)  // 정규 조직
                LoadRegularTeamTree(strSearchMemberName);
        }

        private TreeNode FindNode(int nTag, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewTeam.Nodes : parentNodes;

            foreach (TreeNode node in nodes)
            {
                object tag = node.Tag;
                if( tag != null)
                {
                    if( tag.GetType() == typeof(Data_RegularTeam))
                    {
                        Data_RegularTeam team = (Data_RegularTeam)tag;
                        if (team.ID == nTag)
                            return node;

                    }
                    else if(tag.GetType() == typeof(ExternalCompanyTeam))
                    {
                        ExternalCompanyTeam team = (ExternalCompanyTeam)tag;
                        if (team.ID == nTag)
                            return node;
                    }
                }

                TreeNode result = FindNode(nTag, node.Nodes);
                if (result != null)
                    return result;
            }

            return null;
        }

        public TreeNode FindNode(string strValue, TreeNodeCollection parentNodes = null)
        {
            TreeNodeCollection nodes = parentNodes == null ? treeViewTeam.Nodes : parentNodes;

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

        private void SelectMember(out string strMessage)
        {
            strMessage = string.Empty;

            TreeNode node = treeViewTeam.SelectedNode;
            if (node == null)
            {
                strMessage = "팀원이 선택되지 않았습니다.\n팀원을 선택하십시요.";
            }
            else
            {
                object tag = node.Tag;
                if (tag != null)
                {
                    if (tag.GetType() == typeof(Data_CompanyMember))
                    {
                        Data_CompanyMember member = (Data_CompanyMember)tag;
                        if (member != null)
                        {
                            SelectedName = member.MemberName;
                            SelectedPhone = member.PhoneNumber;
                        }
                    }
                    else if (tag.GetType() == typeof(ExternalCompanyMember))
                    {
                        ExternalCompanyMember member = (ExternalCompanyMember)tag;
                        if (member != null)
                        {
                            SelectedName = member.MemberName;
                            SelectedPhone = member.PhoneNumber;
                        }
                    }
                    else
                    {
                        strMessage = "팀원이 선택되지 않았습니다.\n팀원을 선택하십시요.";
                    }
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string strMessage;

            SelectMember(out strMessage);

            if (String.IsNullOrWhiteSpace(strMessage) == true)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show(strMessage);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void rbBtnExternal_CheckedChanged(object sender, EventArgs e)
        {
            if( rbBtnExternal.Checked == true)
            {
                m_nCurrentTeamType = Sections.SOPTeam.SOPTeamType.External;

                InitTree();
            }
        }

        private void rbBtnRegular_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBtnRegular.Checked == true)
            {
                m_nCurrentTeamType = Sections.SOPTeam.SOPTeamType.Regular;

                InitTree();
            }
        }

        private void treeViewTeam_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string strMessage;

            SelectMember(out strMessage);

            if (String.IsNullOrWhiteSpace(strMessage) == true)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string strSearchWord = txtSearch.Text;

            InitTree(strSearchWord.Trim());
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnSearch.PerformClick();
            }
        }

        private void treeViewTeam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            labelMemberPath.Text = string.Empty;

            if (e.Node != null && e.Node.Tag != null)
            {
                if (e.Node.Tag is Data_CompanyMember || e.Node.Tag is ExternalCompanyMember)
                {
                    labelMemberPath.Text = String.Format("{0} >> {1}", GetMemberDepartmentName(e.Node.Parent), e.Node.Text);
                }
            }

        }

        private string GetMemberDepartmentName(TreeNode node)
        {
            if (node.Parent != null)
            {
                return String.Format("{0} / {1}", GetMemberDepartmentName(node.Parent), node.Text);
            }
            else
            {
                return node.Text;
            }

        }

    }
}
