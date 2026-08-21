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

namespace SMSSender
{
    public partial class FormReciver : Form
    {
        // Form Move 를 위한 Panel Top 클릭 체크
        private bool m_bLeftMouseDown = false;
        // Form Move 를 위한 Panel Top 클릭 점
        private Point m_ptMove;

        private Font m_fontButton = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Regular);

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
        
        private ArrayList m_arRecivers = new ArrayList();
        public ArrayList Recivers
        {
            get { return m_arRecivers; }
            set
            {
                m_arRecivers.Clear();
                m_arRecivers.AddRange(value);
            }
        }


        private UnE.SOP.SOPManager m_SopManager = null;

        public FormReciver(UnE.SOP.SOPManager sopManager )
        {
            InitializeComponent();
            m_SopManager = sopManager;

            SetRibbonButtonFont();
        }

        private void SetRibbonButtonFont()
        {
            btnSearch.Font = m_fontButton;
        }

        private void FormTeamTree_Load(object sender, EventArgs e)
        {
            InitTree();

            foreach (ReciverListItem item in m_arRecivers)
            {
                listReciver.Items.Add(item);
            }

            labelMemberPath.Text = string.Empty;
        }

        private void LoadExternalTeamTree(string strSearchMemberName)
        {
            TreeNode selectNode = null;

            ArrayList arrExternalTeam = m_SopManager.ExternalCompanyTeams;
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

            Dictionary<int, TreeNode> dicRegularTeamNode = new Dictionary<int, TreeNode>();


            List<Data_RegularTeam> arrRegularTeam = m_SopManager.RegularTeams;  
            foreach (Data_RegularTeam data in arrRegularTeam)
            {
                if (data.ParentTeamID == -1)
                {
                    TreeNode node = treeViewTeam.Nodes.Add(data.TeamName.TrimEnd());
                    node.Tag = data;

                    ArrayList arMembers = new ArrayList();
                    if (m_SopManager.GetRegularCompanyMemberList(data.ID, ref arMembers))
                    {
                        foreach(Data_CompanyMember member in arMembers)
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

                    dicRegularTeamNode.Add(data.ID, node);
                }
                else
                {
                    TreeNode child = null;

                    //if (dicRegularTeamNode.ContainsKey(data.ParentTeamID) == false)
                    //{

                    //}

                    child = FindNode(data.ParentTeamID, treeViewTeam.Nodes);

                    if (child == null) return;

                    //dicRegularTeamNode.Add(data.ID, child);

                    TreeNode newNode = child.Nodes.Add(data.TeamName.TrimEnd());
                    newNode.Tag = data;
                   
                    ArrayList arMembers = new ArrayList();
                    if (m_SopManager.GetRegularCompanyMemberList(data.ID, ref arMembers))
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

        private void AddListItem(object o, int nType)
        {
            ArrayList arList = new ArrayList();
            arList.AddRange(listReciver.Items);


            ReciverListItem itemNew = new ReciverListItem(o, nType);
            foreach (ReciverListItem item in arList)
            {
                if(item.Compare(item, itemNew) == 0)
                {
                    // 이미 있는 내용이므로 넣지 않는다.
                    return;
                }
            }
            listReciver.Items.Add(itemNew);
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
                            AddListItem(member, 1);
                            strMessage = member.MemberName;
                        }
                    }
                    else if (tag.GetType() == typeof(ExternalCompanyMember))
                    {
                        ExternalCompanyMember member = (ExternalCompanyMember)tag;
                        if (member != null)
                        {
                            AddListItem(member, 2);
                            strMessage = member.MemberName;
                        }
                    }
                    else if (tag.GetType() == typeof(Data_RegularTeam))
                    {
                        Data_RegularTeam team = (Data_RegularTeam)tag;
                        if (team != null)
                        {
                            AddListItem(team, 3);
                            strMessage = team.TeamName;
                        }
                    }
                    if (tag.GetType() == typeof(ExternalCompanyTeam))
                    {
                        ExternalCompanyTeam team = (ExternalCompanyTeam)tag;
                        if (team != null)
                        {
                            AddListItem(team, 4);
                            strMessage = team.TeamName;
                        }
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listReciver.SelectedItem == null)
                return;

            ArrayList arList = new ArrayList();
            arList.AddRange(listReciver.SelectedItems);
            
            foreach(ReciverListItem item in arList)
            {
                listReciver.Items.Remove(item);
            }
        }       

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string strMessage;

            SelectMember(out strMessage);

            if (String.IsNullOrWhiteSpace(strMessage) == true)
            {
                //this.DialogResult = DialogResult.OK;
                //this.Close();
            }
            else
            {
                //MessageBox.Show(strMessage);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_arRecivers.Clear();
            m_arRecivers.AddRange(listReciver.Items);
            this.DialogResult = DialogResult.OK;
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

        private TreeNode m_LastSearchNode = null; 

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string strSearchWord = txtSearch.Text;
            InitTree(strSearchWord.Trim());

            return;
            ////

            //string strSearchWord = txtSearch.Text;

            //if (String.IsNullOrWhiteSpace(strSearchWord) == true)
            //    return;

            //foreach (TreeNode node in treeViewTeam.Nodes)
            //{
            //    if (SearchMember(node, strSearchWord) == true)
            //        return;
            //}
        }

        private bool SearchMember(TreeNode node, string strSearchWord)
        {
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode c_node in node.Nodes)
                {
                    if (SearchMember(c_node, strSearchWord) == true)
                        return true;
                }
            }
            else if (node.Text.Contains(strSearchWord))
            {
                treeViewTeam.SelectedNode = node;

                return true;
            }

            return false;
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for( int i = 0; i < listReciver.Items.Count; i++)
            {
                bool bSelected = true;// listReciver.GetSelected(i);
                listReciver.SetSelected(i, bSelected);
            }
        }

        private void btnAddManual_Click(object sender, EventArgs e)
        {
            FormManualInput input = new FormManualInput(this);
            input.ShowDialog();

        }     

        public void AddManualPhoneNumber(string szPhoneNumber)
        {
            if (szPhoneNumber != null && szPhoneNumber != "")
                AddListItem(szPhoneNumber, 5);            
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

        private void plTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitle.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void plTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void plTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point ptCur = this.Location;

                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {

                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void lbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitle.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void lbTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void lbTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point ptCur = this.Location;

                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {

                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void pbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitle.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void pbTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void pbTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point ptCur = this.Location;

                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {

                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }
    }
}
