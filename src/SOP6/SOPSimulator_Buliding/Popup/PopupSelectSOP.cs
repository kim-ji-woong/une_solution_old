using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPMonitoringSystem.Popup
{
    public partial class PopupSelectSOP : Form
    {
        private PageBackstageSOP.QuickSOPButton m_sop = null;
        public PageBackstageSOP.QuickSOPButton QuickSOP { get { return m_sop; } set { m_sop = value; } }

        private bool m_isNormal = false;
        public bool IsNormal { get { return m_isNormal; } set { m_isNormal = value; } }

        private int m_nID = -1;
        public int DisasterTypeID { get { return m_nID; } set { m_nID = value; } }

        public event EventHandler SelectButtonClickEvent;

        private UnE.SOP.SOPManager sopManager = null;
        private bool m_bHoliday = false;

        // Form Move 를 위한 Panel Top 클릭 체크
        private bool m_bLeftMouseDown = false;
        // Form Move 를 위한 Panel Top 클릭 점
        private Point m_ptMove;

        private Font m_fontButton = new System.Drawing.Font("나눔스퀘어", 12F, System.Drawing.FontStyle.Regular);

        public PopupSelectSOP()
        {
            InitializeComponent();

            InitTree();
            SetRibbonButtonFont();
        }
       
        public void InitTree()
        {
            sopManager = FormSOP.Instance.SOPManager;
        }

        private void SetRibbonButtonFont()
        {
            btnSelect.Font = m_fontButton;
            btnClose.Font = m_fontButton;
        }

        private void PopupSelectSOP_Load(object sender, EventArgs e)
        {
            m_bHoliday = !m_isNormal;
            treeSOP.Load(sopManager, true, !m_bHoliday);

            if (m_isNormal)
                lblSenario.Text = "평일 시나리오";
            else
                lblSenario.Text = "휴일 및 야간 시나리오";


            switch(m_nID)
            {
                case ID.ID_SOP_FIRE :
                    treeSOP.SelectedNode = treeSOP.FindNode("화재");
                    break;
                case ID.ID_SOP_POLLUTION :
                    treeSOP.SelectedNode = treeSOP.FindNode("유출사고");
                    if(treeSOP.SelectedNode != null)
                    {
                        treeSOP.SelectedNode = treeSOP.FindNode("오염", treeSOP.SelectedNode.Nodes);
                    }
                    break;
                default :
                    treeSOP.SelectedNode = treeSOP.FindNode("자연재해");

                    if (treeSOP.SelectedNode != null)
                    {
                        switch (m_nID)
                        {
                            case ID.ID_SOP_EARTHQUAKE:
                                treeSOP.SelectedNode = treeSOP.FindNode("지진", treeSOP.SelectedNode.Nodes);
                                break;
                            case ID.ID_SOP_SUBMERGENCE:
                                treeSOP.SelectedNode = treeSOP.FindNode("침수", treeSOP.SelectedNode.Nodes);
                                break;
                            case ID.ID_SOP_TYPHOON:
                                treeSOP.SelectedNode = treeSOP.FindNode("태풍", treeSOP.SelectedNode.Nodes);
                                break;
                            case ID.ID_SOP_HEAVY_SNOW:
                                treeSOP.SelectedNode = treeSOP.FindNode("폭설", treeSOP.SelectedNode.Nodes);
                                break;
                        }
                    }
                    break;
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            UnE.SOP.Tree.SOPTreeNode node = (UnE.SOP.Tree.SOPTreeNode)treeSOP.SelectedNode;
            if (node != null)
            {
                UnE.SOP.Tree.SOPTreeNode targetNode = null;
                string strActionStepName = string.Empty;

                if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.ACTIONSTEP_NODE)
                {
                    targetNode = (UnE.SOP.Tree.SOPTreeNode)node.Parent;
                    strActionStepName = node.Text;
                }
                else if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.CATEGORY_NODE)
                {
                    // 하위 선택하도록 팝업
                    MessageBox.Show("SOP 시나리오를 선택하세요.");
                    return;
                }
                else if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.SUBCATEGOY_NODE)
                {
                    // 하위 선택하도록 팝업
                    MessageBox.Show("SOP 시나리오를 선택하세요.");
                    return;
                }
                else if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.DISASTER_NODE)
                {
                    targetNode = node;
                }

                string strPath = targetNode.FullPath;

                if (m_isNormal)
                {
                    m_sop.SOPNormal = strPath.Replace(@"\", "/");
                    m_sop.SOPActionStepNameNormal = strActionStepName;
                }
                else
                {
                    m_sop.SOPEmergency = strPath.Replace(@"\", "/");
                    m_sop.SOPActionStepNameEmergency = strActionStepName;
                }
            }

            if (SelectButtonClickEvent != null)
                SelectButtonClickEvent(sender, e);

            //(Owner as PopupTranslucentForm).CloseExternal();
            this.Close();
        }

        private void treeSOP_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UnE.SOP.Tree.SOPTreeNode node = (UnE.SOP.Tree.SOPTreeNode)treeSOP.SelectedNode;

            if (node != null)
            {
                if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.ACTIONSTEP_NODE)
                {
                    btnSelect.Enabled = true;
                }
                else if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.CATEGORY_NODE)
                {
                    btnSelect.Enabled = false;
                }
                else if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.SUBCATEGOY_NODE)
                {
                    btnSelect.Enabled = false;
                }
                else if (node.TreeNodeType == UnE.SOP.Tree.TreeNodeType.DISASTER_NODE)
                {
                    btnSelect.Enabled = true;
                }
            }
            else
                btnSelect.Enabled = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            //(Owner as PopupTranslucentForm).CloseExternal();
            this.Close();
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

    public class SOPTreeSim : UnE.SOP.Tree.SOPTreeView
    {
        private TreeNode m_CurrentNode = null;
        public TreeNode CurrentNode
        {
            get { return m_CurrentNode; }
        }

        public override void SelectNode(TreeNode node)
        {
            m_CurrentNode = node;
        }
    }
}
