using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class PopupSOPList : Form, IFormDisasterOwner
    {
        private Popup.SOPLoader m_sopLoader = null;
        private TreeNode m_prevSelectedNode = null;
        private bool m_init = false;
        private string m_strSOPManualFilePath = "";

        private bool m_isTreeMode = false;
        private FormDisaster m_frmDisaster = null;

        private bool IsRegular
        {
            get { return radioRegular.Checked; }
        }

        private bool IsNormal
        {
            get { return radioNormal.Checked; }
        }

        public PopupSOPList()
        {
            InitializeComponent();

            m_sopLoader = new Popup.SOPLoader(treeView);

            m_strSOPManualFilePath = FormMain.Instance.DBManager.LoadIni("url", "SOPManual");

            if (m_strSOPManualFilePath.Length == 0)
                btnShowSOPManual.Enabled = false;

            if (m_isTreeMode)
            {
                btnPrev.Visible = btnNext.Visible = false;
                treeView.Size = new Size(treeView.Size.Width, treeView.Size.Width);
            }
            else
            {
                m_frmDisaster = new FormDisaster(this);
                m_frmDisaster.TopLevel = false;

                this.Controls.Add(m_frmDisaster);
                m_frmDisaster.FormOwner = this;

                m_frmDisaster.Size = treeView.Size;
                m_frmDisaster.Location = treeView.Location;
                m_frmDisaster.Show();

                treeView.Size = new Size(10, 10);
                treeView.Location = new Point(-100, 0);
            }
        }

        private void PopupSOPList_Load(object sender, EventArgs e)
        {
            radioRegular.Checked = true;
            m_init = true;

            bool isNormal = Popup.SOPLoader.IsNormal(DateTime.Now);

            if (isNormal)
                radioNormal.Checked = true;
            else
                radioAbnormal.Checked = true;
        }

        private void radioMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_init)
                return;

            m_sopLoader.LoadTree(FormMain.Instance.SOPManager, IsRegular, IsNormal);
            m_frmDisaster.LoadSOP(treeView.Nodes);
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeView.SelectedNode;

            if (m_prevSelectedNode == node)
                goto RETURN_FALSE;

            m_prevSelectedNode = node;
            if (node == null) goto RETURN_FALSE;

            FormMain frmMain = FormMain.Instance;
            string strSOPInfo = "";

            if (!m_sopLoader.GetSOPVersionInfo(node, frmMain.SOPManager, IsRegular, IsNormal, ref strSOPInfo))
                goto RETURN_FALSE;

            rTextBoxSOPInfo.Text = strSOPInfo;
            return;

        RETURN_FALSE:
            rTextBoxSOPInfo.Text = "";
        }

        private void btnLoadSOP_Click(object sender, EventArgs e)
        {
            if (treeView.SelectedNode == null || rTextBoxSOPInfo.Text.Length == 0)
            {
                MessageBox.Show("SOP를 선택하세요.");
                return;
            }

            ArrayList arrNodes = new ArrayList();
            TreeNode node = treeView.SelectedNode;

            while (node != null)
            {
                arrNodes.Add(node);
                node = node.Parent;
            }

            int nNodeCount = arrNodes.Count;
            if (nNodeCount == 0)
                return;

            TreeNode firstNode = (TreeNode)arrNodes[nNodeCount - 1];
            arrNodes.RemoveAt(nNodeCount - 1);
            
            FormMain frmMain = FormMain.Instance;

            frmMain.ChangeMode(frmMain.IsReal, IsRegular, IsNormal);

            BarLevelTree tree = frmMain.GetPageHome().GetDockScenario().GetBarLevelTree();

            node = FindSOPNode(tree, null, firstNode.Text, arrNodes);
            tree.TreeView.SelectedNode = node;

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnShowSOPManual_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process process;

            try
            {
                process = System.Diagnostics.Process.Start(m_strSOPManualFilePath, "");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private TreeNode FindSOPNode(BarLevelTree tree, TreeNodeCollection nodes, string strNodeName, ArrayList arrNodes)
        {
            TreeNode node = tree.FindNode(strNodeName, nodes);

            int nNodeCount = arrNodes.Count;

            if (nNodeCount == 0 || node == null)
                return node;

            TreeNode nodeNext = (TreeNode)arrNodes[nNodeCount - 1];
            arrNodes.RemoveAt(nNodeCount - 1);

            return FindSOPNode(tree, node.Nodes, nodeNext.Text, arrNodes);
        }

        public void OnTreeViewClicked(TreeNode node, bool noSelect)
        {
            if (node == null)
                return;

            if (!noSelect)
                treeView.SelectedNode = node;
        }

        public void EnableButton(bool isPrevButton, bool enabled)
        {
            if (isPrevButton)
                btnPrev.Enabled = enabled;
            else
                btnNext.Enabled = enabled;
        }

        private void btnPrevNext_Click(object sender, EventArgs e)
        {
            if (sender == btnPrev)
                m_frmDisaster.GoBack();
            else
                m_frmDisaster.GoForward();
        }
    }
}
