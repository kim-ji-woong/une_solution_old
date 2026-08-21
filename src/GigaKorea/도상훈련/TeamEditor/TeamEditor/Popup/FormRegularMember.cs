using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor.Popup
{
    public partial class FormRegularMember : Form
    {
        private Size m_initTreeSize;
        private Size m_initGridSize;
        private Point m_initTreeLocation;
        private Point m_initGridLocation;
        private Point m_initLableLocation;

        private string m_strTeamPath = String.Empty;

        private FormSelectTemporaryMember m_frmOwner = null;

        public bool ShowGrid
        {
            get { return gridCompanyMember.Visible; }
            set { _ShowGrid(value); }
        }

        public CompanyMember SelectedCompanyMember
        {
            get
            {
                if (!ShowGrid)
                    return null;

                if (gridCompanyMember.SelectedCells.Count == 0)
                    return null;

                int nRowIndex = gridCompanyMember.SelectedCells[0].RowIndex;

                if (nRowIndex < 0)
                    return null;

                DataGridViewRow row = gridCompanyMember.Rows[nRowIndex];
                CompanyMember member = (CompanyMember)row.Tag;

                return member;
            }
        }

        public RegularTeam SelectedRegularTeam
        {
            get
            {
                if (treeRegularTeam.SelectedNode == null || treeRegularTeam.SelectedNode.Tag == null)
                    return null;

                RegularTeam team = (RegularTeam)treeRegularTeam.SelectedNode.Tag;
                return team;
            }
        }

        public FormRegularMember(FormSelectTemporaryMember frm, TeamGrid gridSource, bool showGrid = false)
        {
            InitializeComponent();
            m_frmOwner = frm;
            Init(gridSource);
            _ShowGrid(showGrid);
        }

        private void Init(TeamGrid gridSource)
        {
            CopyGrid(gridSource);

            m_initTreeSize = treeRegularTeam.Size;
            m_initGridSize = gridCompanyMember.Size;
            m_initTreeLocation = treeRegularTeam.Location;
            m_initGridLocation = gridCompanyMember.Location;
            m_initLableLocation = lblTeamPath.Location;

            InitColumns();
            // CopyGrid에서 처리하였음
            //SetPositionItems();
        }

        private void CopyGrid(TeamGrid gridSource)
        {
            gridCompanyMember.Columns.Clear();

            foreach (DataGridViewColumn column in gridSource.Columns)
            {
                DataGridViewColumn column2 = (DataGridViewColumn)column.Clone();
                gridCompanyMember.Columns.Add(column2);
            }
        }

        private void InitColumns()
        {
            foreach (DataGridViewColumn column in gridCompanyMember.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void _ShowGrid(bool showGrid)
        {
            if (showGrid)
            {
                lblTeamPath.Location = m_initLableLocation;

                treeRegularTeam.Location = m_initTreeLocation;
                treeRegularTeam.Size = new Size(m_initTreeSize.Width, this.Size.Height - 6);
                treeRegularTeam.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            }
            else
            {
                lblTeamPath.Location = new Point(m_initTreeLocation.X, m_initLableLocation.Y);

                treeRegularTeam.Location = new Point(m_initTreeLocation.X, m_initGridLocation.Y);
                treeRegularTeam.Size = new Size(gridCompanyMember.Location.X + gridCompanyMember.Size.Width - treeRegularTeam.Location.X, gridCompanyMember.Size.Height);
                treeRegularTeam.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            }

            gridCompanyMember.Visible = showGrid;
        }

        public void ResetTeamPath()
        {
            lblTeamPath.Text = m_strTeamPath;
        }

        public void Update(TeamTreeView tree)
        {
            // Tree가 업데이트 된 이후에 이전에 선택되어 있던 노드를 다시 선택할 수 있게 한다.
            RegularTeam teamSelected = null;
            TreeNode nodeSelected = null;

            if (treeRegularTeam.SelectedNode != null)
                teamSelected = (RegularTeam)treeRegularTeam.SelectedNode.Tag;

            treeRegularTeam.Nodes.Clear();
            CopyNodes(treeRegularTeam.Nodes, tree.Nodes, teamSelected, ref nodeSelected);

            treeRegularTeam.ExpandAll();

            if (nodeSelected != null)
                treeRegularTeam.SelectedNode = nodeSelected;


            string strTeamPath = String.Empty;
            GetTeamPath(treeRegularTeam.SelectedNode, ref strTeamPath);
            lblTeamPath.Text = strTeamPath;

        }

        private void GetTeamPath(TreeNode node, ref string strTeamPath)
        {
            if (node == null) return;

            if (node.Parent != null)
            {
                GetTeamPath(node.Parent, ref strTeamPath);
            }

            if (strTeamPath.Length != 0)
            {
                strTeamPath += " > ";
            }

            strTeamPath += node.Text;
        }

        private void CopyNodes(TreeNodeCollection nodesTrg, TreeNodeCollection nodesSrc, RegularTeam teamSelected, ref TreeNode nodeSelected)
        {
            foreach (TreeNode node in nodesSrc)
            {
                TreeNode newNode = new TreeNode(node.Text);
                newNode.Tag = node.Tag;
                nodesTrg.Add(newNode);

                if (newNode.Tag != null && newNode.Tag == teamSelected)
                    nodeSelected = newNode;

                CopyNodes(newNode.Nodes, node.Nodes, teamSelected, ref nodeSelected);
            }
        }

        private void treeRegularTeam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeRegularTeam.SelectedNode != null)
                m_frmOwner.SelectedTeam = treeRegularTeam.SelectedNode.Tag;
            else
                m_frmOwner.SelectedTeam = null;

            if (treeRegularTeam.SelectedNode == null)
                gridCompanyMember.SelectTeam(null);
            else
                gridCompanyMember.SelectTeam((RegularTeam)treeRegularTeam.SelectedNode.Tag);

            string strTeamPath = String.Empty;
            GetTeamPath(treeRegularTeam.SelectedNode, ref strTeamPath);
            lblTeamPath.Text = strTeamPath;

            m_strTeamPath = strTeamPath;

            if (ShowGrid)
                CheckSelectedMember();
        }

        private void gridCompanyMember_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ShowGrid)
            {
                CheckSelectedMember();
            }
        }

        public void CheckSelectedMember()
        {
            m_frmOwner.SelectedMember = null;

            if (gridCompanyMember.SelectedCells.Count > 0)
            {
                int nRowIndex = gridCompanyMember.SelectedCells[0].RowIndex;

                if (nRowIndex < 0)
                    return;

                DataGridViewRow row = gridCompanyMember.Rows[nRowIndex];

                if (row.IsNewRow)
                {
                    lblTeamPath.Text = m_strTeamPath;
                }
                else
                {
                    m_frmOwner.SelectedMember = row.Tag;
                    lblTeamPath.Text = String.Format("{0} > {1}", m_strTeamPath, (row.Tag as CompanyMember).Name);
                }
            }
        }

        private void gridCompanyMember_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            TeamGrid gdv = sender as TeamGrid;
            if (gdv == null) return;

            foreach (DataGridViewRow row in gdv.Rows)
            {
                row.MinimumHeight = gdv.RowHeight;
            }
        }

        /*private void SetPositionItems()
        {
            bool init = false;

            for (int i = 0; ; i++)
            {
                string strPositionName = DataManager.GetJobPositionName(i);

                if (strPositionName == null)
                {
                    if (init)
                        break;
                }
                else
                {
                    init = true;
                    colPosition.Items.Add(strPositionName);
                }
            }
        }*/
    }
}
