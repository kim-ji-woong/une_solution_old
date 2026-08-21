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
    public partial class FormExternalMember : Form
    {
        private Size m_initTreeSize;
        private Size m_initGridSize;
        private Point m_initTreeLocation;
        private Point m_initGridLocation;
        private Point m_initLableLocation;

        private string m_strTeamPath = String.Empty;

        private FormSelectTemporaryMember m_frmOwner = null;

        private double m_WindowRateWidth = 1;
        public double WindowRateWidth
        {
            get { return m_WindowRateWidth; }
            set { m_WindowRateWidth = value; }
        }

        private double m_WindowRateHeight = 1;
        public double WindowRateHeight
        {
            get { return m_WindowRateHeight; }
            set { m_WindowRateHeight = value; }
        }

        public bool ShowGrid
        {
            get { return gridExternalCompanyMember.Visible; }
            set { _ShowGrid(value); }
        }

        public ExternalCompanyMember SelectedExternalCompanyMember
        {
            get
            {
                if (!ShowGrid)
                    return null;

                if (gridExternalCompanyMember.SelectedCells.Count == 0)
                    return null;

                int nRowIndex = gridExternalCompanyMember.SelectedCells[0].RowIndex;

                if (nRowIndex < 0)
                    return null;

                DataGridViewRow row = gridExternalCompanyMember.Rows[nRowIndex];
                ExternalCompanyMember member = (ExternalCompanyMember)row.Tag;

                return member;
            }
        }

        // ExternalTeam 또는 ExternalCompanyTeam일수 있다.
        public Team SelectedTeam
        {
            get
            {
                if (treeExternalCompanyTeam.SelectedNode == null || treeExternalCompanyTeam.SelectedNode.Tag == null)
                    return null;

                Team team = (Team)treeExternalCompanyTeam.SelectedNode.Tag;
                return team;
            }
        }

        public FormExternalMember(FormSelectTemporaryMember frm, TeamGrid gridSource, bool showGrid = false)
        {
            InitializeComponent();
            m_frmOwner = frm;
            Init(gridSource);
            _ShowGrid(showGrid);
        }

        private void Init(TeamGrid gridSource)
        {
            CopyGrid(gridSource);

            m_initTreeSize = treeExternalCompanyTeam.Size;
            m_initGridSize = gridExternalCompanyMember.Size;
            m_initTreeLocation = treeExternalCompanyTeam.Location;
            m_initGridLocation = gridExternalCompanyMember.Location;
            m_initLableLocation = lblTeamPath.Location;

            InitColumns();
        }

        public void UpdateControl()
        {
            FormMain.Instance.UpdateWindowRate(treeExternalCompanyTeam, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(gridExternalCompanyMember, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(lblTeamPath, WindowRateWidth, WindowRateHeight);
        }

        private void CopyGrid(TeamGrid gridSource)
        {
            gridExternalCompanyMember.Columns.Clear();

            foreach (DataGridViewColumn column in gridSource.Columns)
            {
                DataGridViewColumn col = column.Clone() as DataGridViewColumn;
                gridExternalCompanyMember.Columns.Add(col);
            }
        }

        private void InitColumns()
        {
            foreach (DataGridViewColumn column in gridExternalCompanyMember.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void _ShowGrid(bool showGrid)
        {
            if (showGrid)
            {
                lblTeamPath.Location = new Point((int)(m_initLableLocation.X * WindowRateWidth), (int)(m_initLableLocation.Y * WindowRateHeight));

                treeExternalCompanyTeam.Location = new Point((int)(m_initTreeLocation.X * WindowRateWidth), (int)(m_initTreeLocation.Y * WindowRateHeight));
                treeExternalCompanyTeam.Size = new Size((int)(m_initTreeSize.Width * WindowRateWidth), this.Size.Height - 6);
                treeExternalCompanyTeam.Anchor = AnchorStyles.Left | AnchorStyles.Top;// | AnchorStyles.Bottom;
            }
            else
            {
                
                lblTeamPath.Location = new Point((int)(m_initTreeLocation.X * WindowRateWidth), (int)(m_initLableLocation.Y * WindowRateHeight));

                treeExternalCompanyTeam.Location = new Point((int)(m_initTreeLocation.X * WindowRateWidth), (int)(m_initGridLocation.Y * WindowRateHeight));
                treeExternalCompanyTeam.Size = new Size(gridExternalCompanyMember.Location.X + gridExternalCompanyMember.Size.Width - treeExternalCompanyTeam.Location.X, gridExternalCompanyMember.Size.Height);
                treeExternalCompanyTeam.Anchor = AnchorStyles.Left | AnchorStyles.Top;// | AnchorStyles.Bottom | AnchorStyles.Right;
            }

            gridExternalCompanyMember.Visible = showGrid;
            gridExternalCompanyMember.Location = new Point((int)(m_initGridLocation.X * WindowRateWidth), (int)(m_initGridLocation.Y * WindowRateHeight));
        }

        public void ResetTeamPath()
        {
            lblTeamPath.Text = m_strTeamPath;
        }

        public void Update(TeamTreeView tree)
        {
            lblTeamPath.Text = m_strTeamPath;

            // Tree가 업데이트 된 이후에 이전에 선택되어 있던 노드를 다시 선택할 수 있게 한다.
            Team teamSelected = null;
            TreeNode nodeSelected = null;

            if (treeExternalCompanyTeam.SelectedNode != null)
                teamSelected = (Team)treeExternalCompanyTeam.SelectedNode.Tag;

            treeExternalCompanyTeam.Nodes.Clear();
            CopyNodes(treeExternalCompanyTeam.Nodes, tree.Nodes, teamSelected, ref nodeSelected);

            treeExternalCompanyTeam.ExpandAll();

            if (nodeSelected != null)
                treeExternalCompanyTeam.SelectedNode = nodeSelected;


            string strTeamPath = String.Empty;
            GetTeamPath(treeExternalCompanyTeam.SelectedNode, ref strTeamPath);
            lblTeamPath.Text = strTeamPath;

            m_strTeamPath = strTeamPath;
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

        private void CopyNodes(TreeNodeCollection nodesTrg, TreeNodeCollection nodesSrc, Team teamSelected, ref TreeNode nodeSelected)
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

        private void treeExternalCompanyTeam_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeExternalCompanyTeam.SelectedNode != null)
                m_frmOwner.SelectedTeam = treeExternalCompanyTeam.SelectedNode.Tag;
            else
                m_frmOwner.SelectedTeam = null;

            if (treeExternalCompanyTeam.SelectedNode == null)
                gridExternalCompanyMember.SelectTeam(null);
            else
            {
                gridExternalCompanyMember.Type = TeamGrid.GridType.ExternalCompanyTeam;
                gridExternalCompanyMember.SelectTeam((Team)treeExternalCompanyTeam.SelectedNode.Tag);
            }

            string strTeamPath = String.Empty;
            GetTeamPath(treeExternalCompanyTeam.SelectedNode, ref strTeamPath);
            lblTeamPath.Text = strTeamPath;

            m_strTeamPath = strTeamPath;

            if (ShowGrid)
                CheckSelectedMember();
        }

        private void gridExternalCompanyMember_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (ShowGrid)
            {
                CheckSelectedMember();
            }
        }

        public void CheckSelectedMember()
        {
            m_frmOwner.SelectedMember = null;

            if (gridExternalCompanyMember.SelectedCells.Count > 0)
            {
                int nRowIndex = gridExternalCompanyMember.SelectedCells[0].RowIndex;

                if (nRowIndex < 0)
                    return;

                DataGridViewRow row = gridExternalCompanyMember.Rows[nRowIndex];

                if (row.IsNewRow)
                {
                    lblTeamPath.Text = m_strTeamPath;
                }
                else
                {
                    m_frmOwner.SelectedMember = row.Tag;
                    lblTeamPath.Text = String.Format("{0} > {1}", m_strTeamPath, (row.Tag as ExternalCompanyMember).Name);
                }
            }
        }

        private void gridExternalCompanyMember_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            TeamGrid gdv = sender as TeamGrid;
            if (gdv == null) return;

            foreach (DataGridViewRow row in gdv.Rows)
            {
                row.MinimumHeight = gdv.RowHeight;
            }
        }

    }
}
