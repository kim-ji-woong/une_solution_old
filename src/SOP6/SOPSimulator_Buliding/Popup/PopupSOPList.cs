using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using UnE.GUI;
using UnE.SOP;

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

        // Form Move 를 위한 Panel Top 클릭 체크
        private bool m_bLeftMouseDown = false;
        // Form Move 를 위한 Panel Top 클릭 점
        private Point m_ptMove;

        public TreeButton m_btnSelect;

        private Font m_fontButton = new System.Drawing.Font("나눔바른고딕", 12F, System.Drawing.FontStyle.Bold);
        private Font m_fontLoadButton = new System.Drawing.Font("나눔바른고딕", 15F, System.Drawing.FontStyle.Bold);

        public bool IsRegular
        {
            get { return radioRegular.Checked; }
        }

        public bool IsNormal
        {
            get { return radioNormal.IsChecked; } //radioNormal.Checked;
        }

        public Form MainFrame
        {
            get { return SOPMonitoringSystem.FormSOP.Instance; }
        }

        public PopupSOPList()
        {
            InitializeComponent();

            //m_sopLoader = new Popup.SOPLoader(treeView);

            m_strSOPManualFilePath = FormSOP.Instance.DBManager.LoadIni("url", "SOPManual");

            //if (m_strSOPManualFilePath.Length == 0)
                //btnShowSOPManual.Enabled = false;

            if (m_isTreeMode)
            {
                //btnPrev.Visible = btnNext.Visible = false;
                //treeView.Size = new Size(treeView.Size.Width, treeView.Size.Width);
            }
            else
            {
                //m_frmDisaster = new FormDisaster_Building(this);
                //m_frmDisaster.TopLevel = false;

                //this.Controls.Add(m_frmDisaster);
                //m_frmDisaster.FormOwner = this;

                //m_frmDisaster.Size = treeView.Size;
                //m_frmDisaster.Location = treeView.Location;
                //m_frmDisaster.Show();

                //treeView.Size = new Size(10, 10);
                //treeView.Location = new Point(-100, 0);
            }

            SetRibbonButtonFont();

            MainTree mainTree = new MainTree(plMainTree, plSubTree, plDisasterTree, this);
            mainTree.LoadTree();

            this.Location = FormSOP.Instance.GetSOPListLocation();
        }

        private void SetRibbonButtonFont()
        {
            radioNormal.Font = m_fontButton;
            radioAbnormal.Font = m_fontButton;
            btnLoadSOP.Font = m_fontLoadButton;
        }

        private void PopupSOPList_Load(object sender, EventArgs e)
        {
            radioRegular.Checked = true;
            m_init = true;

            bool isNormal = Popup.SOPLoader.IsNormal(DateTime.Now);

            if (isNormal)
            {
                radioNormal.IsChecked = true;
                radioAbnormal.IsChecked = false;
                radioNormal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListNormal_Selected;
                radioAbnormal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListAbnormal_Normal;
            }
            else
            {
                radioAbnormal.IsChecked = true;
                radioNormal.IsChecked = false;
                radioAbnormal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListAbnormal_Selected;
                radioNormal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListNormal_Normal;
            }

            plMainTree.Controls.Clear();
            plSubTree.Controls.Clear();
            plDisasterTree.Controls.Clear();

            MainTree mainTree = new MainTree(plMainTree, plSubTree, plDisasterTree, this);
            mainTree.LoadTree();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            /*
            TreeNode node = treeView.SelectedNode;

            if (m_prevSelectedNode == node)
                goto RETURN_FALSE;

            m_prevSelectedNode = node;
            if (node == null) goto RETURN_FALSE;

            FormSOP frmMain = FormSOP.Instance;
            string strSOPInfo = "";

            if (!m_sopLoader.GetSOPVersionInfo(node, frmMain.SOPManager, IsRegular, IsNormal, ref strSOPInfo))
                goto RETURN_FALSE;

            rTextBoxSOPInfo.Text = strSOPInfo;
            return;

        RETURN_FALSE:
            rTextBoxSOPInfo.Text = "";
            */
        }

        public void btnLoadSOP_Click(object sender, EventArgs e)
        {
            if (m_btnSelect == null)// || rTextBoxSOPInfo.Text.Length == 0)
            {
                MessageBox.Show("SOP를 선택하세요.");
                return;
            }

            List<TreeButton> listBtns = new List<TreeButton>();
            TreeButton btn = m_btnSelect;

            TreeNode node;

            while (btn != null)
            {
                listBtns.Add(btn);
                btn = btn.Parent;
            }

            int nBtnCount = listBtns.Count;
            if (nBtnCount == 0)
                return;

            TreeButton firstButton = (TreeButton)listBtns[nBtnCount - 1];
            listBtns.RemoveAt(nBtnCount - 1);

            FormSOP frmMain = FormSOP.Instance;

            frmMain.ChangeMode(frmMain.IsReal, IsRegular, IsNormal);

            BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();

            node = FindSOPButton(tree, null, firstButton.Name, listBtns);

            /*if (node != null)
            {
                UnE.SOP.Tree.SOPTreeNode sopNode = (UnE.SOP.Tree.SOPTreeNode)node;

                if (sopNode.DisasterID > 0)
                {
                    // Background Loading을 위하여 Thread를 이용한다.
                    IOManager ioMgr = new IOManager();
                    ioMgr.LoadSOPThread(frmMain, frmMain.DBManager, sopNode.DisasterID);
                }
            }*/

            tree.SelectNode(node);


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


        private TreeNode FindSOPButton(BarLevelTree tree, TreeNodeCollection nodes, string strNodeName, List<TreeButton> arrNodes)
        {
            TreeNode node = tree.FindNode(strNodeName, nodes);

            int nNodeCount = arrNodes.Count;

            if (nNodeCount == 0 || node == null)
                return node;

            TreeButton nodeNext = (TreeButton)arrNodes[nNodeCount - 1];
            arrNodes.RemoveAt(nNodeCount - 1);

            return FindSOPButton(tree, node.Nodes, nodeNext.Name, arrNodes);
        }

        public void OnTreeViewClicked(TreeNode node, bool noSelect)
        {
            /*
            if (node == null)
                return;

            if (!noSelect)
                treeView.SelectedNode = node;
            */
        }

        public void OnTreeViewDoubleClicked(TreeNode node)
        {
            /*
            if (node == null)
                return;

            treeView.SelectedNode = node;
            btnLoadSOP_Click(null, null);
            */
        }

        public void EnableButton(bool isPrevButton, bool enabled)
        {
            /*
            if (isPrevButton)
                btnPrev.Enabled = enabled;
            else
                btnNext.Enabled = enabled;
            */
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void plTitleba_MouseMove(object sender, MouseEventArgs e)
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

        private void plTitleba_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitleba.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void plTitleba_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void lbTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = plTitleba.PointToScreen(new Point(e.X, e.Y));
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
                m_ptMove = plTitleba.PointToScreen(new Point(e.X, e.Y));
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

        private void radioMode_CheckedChanged(object sender, EventArgs e)
        {
            /*
            if (!m_init)
                return;

            m_sopLoader.LoadTree(FormSOP.Instance.SOPManager, IsRegular, IsNormal);
            m_frmDisaster.LoadSOP(treeView.Nodes);
            */
        }

        private void radioNormal_Click(object sender, EventArgs e)
        {
            if (radioNormal.IsChecked == false)
            {
                radioNormal.IsChecked = true;
                radioAbnormal.IsChecked = false;
                radioNormal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListNormal_Selected;
                radioAbnormal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListAbnormal_Normal;

                plMainTree.Controls.Clear();
                plSubTree.Controls.Clear();
                plDisasterTree.Controls.Clear();

                MainTree mainTree = new MainTree(plMainTree, plSubTree, plDisasterTree, this);
                mainTree.LoadTree();
            }
            
        }

        private void radioAbnormal_Click(object sender, EventArgs e)
        {
            if (radioAbnormal.IsChecked == false)
            {
                radioAbnormal.IsChecked = true;
                radioNormal.IsChecked = false;
                radioAbnormal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListAbnormal_Selected;
                radioNormal.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListNormal_Normal;

                plMainTree.Controls.Clear();
                plSubTree.Controls.Clear();
                plDisasterTree.Controls.Clear();

                MainTree mainTree = new MainTree(plMainTree, plSubTree, plDisasterTree, this);
                mainTree.LoadTree();
            }
        }
    }

    //public class TreeButton : RibbonButton
    public class TreeButton : ImageButton
    {
        public TreeButton Parent;
        public bool IsChecked = false;
    }

    public class MainTree
    {
        Panel m_plMainTree;
        Panel m_plSubTree;
        Panel m_plDisasterTree;
        PopupSOPList m_form;

        private Point m_ptTextLocation = new Point(40, 12);
       
        private int m_nButtonWidth = 200;
        private int m_nButtonHeight = 50;
        private int m_nFontSize = 13;
       
        private string m_strFontName = "나눔바른고딕";
        private Color m_colorFont = Color.Black;
        private Color m_colorClickFont = Color.White;

        public MainTree(Panel plMainTree, Panel plSubTree, Panel plDisasterTree, PopupSOPList form)
        {
            m_plMainTree = plMainTree;
            m_plSubTree = plSubTree;
            m_plDisasterTree = plDisasterTree;
            m_form = form;
        }

        public void LoadTree()
        {
            int nIdx = 0;
            m_form.m_btnSelect = null;
            

            List<string> listMainCategory = new List<string>();
            UnE.SOP.SOPManager sopMgr = FormSOP.Instance.SOPManager;
            Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(m_form.IsRegular, m_form.IsNormal);

            foreach (KeyValuePair<string, DisasterInfo> pair in dicSOP)
            {
                string strFullPath = pair.Key;

                int nIndex1 = strFullPath.IndexOf((char)0x06);
                int nIndex2 = strFullPath.LastIndexOf((char)0x06);
                if (nIndex1 < 0 || nIndex2 < 0) continue;

                string strCategoryName = strFullPath.Substring(0, nIndex1);
                string strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                string strDisasterName = strFullPath.Substring(nIndex2 + 1);

                if (!listMainCategory.Contains(strCategoryName))
                    listMainCategory.Add(strCategoryName);
            }

            m_plMainTree.Controls.Clear();

            foreach (string strMainCategory in listMainCategory)
            {
                TreeButton btn = new TreeButton();

                //btn.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                //btn.Image = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                btn.ImageMouseOver = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;
                btn.ImageClicked = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;

                btn.Location = new Point(0, 60 * m_plMainTree.Controls.Count);
                btn.Text = String.Format("{0}.{1}", (char)('A' + nIdx), strMainCategory);
                //btn.UseTextLocation = true;
                btn.Name = String.Format("{0}", strMainCategory);

                //btn.Font = new Font(m_strFontName, m_nFontSize, FontStyle.Bold);
                btn.TextFont = new Font(m_strFontName, m_nFontSize, FontStyle.Bold);
                //btn.ForeColor = m_colorFont;
                btn.TextColor = m_colorFont;

                //btn.TextLocation = m_ptTextLocation;

                btn.Size = new Size(m_nButtonWidth, m_nButtonHeight);
                btn.Click += btnCategory_Click;
                btn.MouseHover += btnCategory_MouseHover;
                btn.MouseLeave += btnCategory_MouseLeave;

                m_plMainTree.Controls.Add(btn);
                nIdx++;

                btn.Refresh();

                //m_plMainTree.Controls.
            }
        }

        private void btnCategory_Click(object sender, EventArgs e)
        {
            //string strCategoryName;
            string strCategory;
            int nIndex;

            TreeButton btnSelect = (TreeButton)sender;
            //strCategoryName = btnSelect.Name;

            if (btnSelect.IsChecked == false)
            {
                m_plDisasterTree.Controls.Clear();

                foreach (TreeButton btn in m_plMainTree.Controls)
                {
                    // 클릭시 기존 버튼들 초기화
                    btn.IsChecked = false;
                    //btn.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                    btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                    //btn.Image = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                    //btn.ForeColor = m_colorFont;
                    btn.TextColor = m_colorFont;
                    btn.Refresh();
                }

                btnSelect.IsChecked = true;
                //btnSelect.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;
                btnSelect.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;
                //btnSelect.Image = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;
                //btnSelect.ForeColor = m_colorClickFont;
                btnSelect.TextColor = m_colorClickFont;

                btnSelect.Refresh();

                strCategory = btnSelect.Text;
                nIndex = strCategory.IndexOf((char)0x2E);
                strCategory = strCategory.Substring(0, nIndex);

                SubTree subTree = new SubTree(m_plSubTree, m_plDisasterTree, m_form);
                subTree.LoadSubTree(btnSelect, strCategory);
            }
            
        }

        private void btnCategory_MouseHover (object sender, EventArgs e)
        {
            TreeButton btn = (TreeButton)sender;

            if (btn.IsChecked == false)
            {
                btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;
                //btn.ForeColor = m_colorClickFont;
                btn.TextColor = m_colorClickFont;
                btn.Refresh();
            }
        }

        private void btnCategory_MouseLeave(object sender, EventArgs e)
        {
            TreeButton btn = (TreeButton)sender;

            if (btn.IsChecked == false)
            {
                btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                //btn.ForeColor = m_colorFont;
                btn.TextColor = m_colorFont;
                btn.Refresh();
            }
        }

    }

    public class SubTree
    {
        Panel m_plSubTree;
        Panel m_plDisasterTree;
        PopupSOPList m_form;

        private Point m_ptTextLocation = new Point(40, 12);

        private int m_nButtonWidth = 200;
        private int m_nButtonHeight = 50;
        private int m_nFontSize = 13;

        private string m_strFontName = "나눔바른고딕";

        private Color m_colorFont = Color.Black;
        private Color m_colorClickFont = Color.White;

        public SubTree(Panel plSubTree, Panel plDisasterTree, PopupSOPList form)
        {
            m_plSubTree = plSubTree;
            m_plDisasterTree = plDisasterTree;
            m_form = form;
        }

        public void LoadSubTree(TreeButton btnCategory, string category)
        {
            int nIdx = 1;
            string selectCategory = btnCategory.Name;

            List<string> listSubCategory = new List<string>();
            UnE.SOP.SOPManager sopMgr = FormSOP.Instance.SOPManager;
            Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(m_form.IsRegular, m_form.IsNormal);

            m_plSubTree.Controls.Clear();
            m_plDisasterTree.Controls.Clear();
            m_form.m_btnSelect = null;

            foreach (KeyValuePair<string, DisasterInfo> pair in dicSOP)
            {
                string strFullPath = pair.Key;

                int nIndex1 = strFullPath.IndexOf((char)0x06);
                int nIndex2 = strFullPath.LastIndexOf((char)0x06);
                if (nIndex1 < 0 || nIndex2 < 0) continue;

                string strCategoryName = strFullPath.Substring(0, nIndex1);
                string strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                string strDisasterName = strFullPath.Substring(nIndex2 + 1);

                //TreeNode nodeCategory = FindNode(strCategoryName, m_tree.Nodes);

                if (selectCategory == strCategoryName)
                {
                    if (!listSubCategory.Contains(strSubCategoryName))
                        listSubCategory.Add(strSubCategoryName);
                }

            }

            foreach (string strMainCategory in listSubCategory)
            {
                TreeButton btn = new TreeButton();
                btn.Parent = btnCategory;

                //btn.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                btn.ImageMouseOver = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;
                btn.ImageClicked = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;

                btn.Location = new Point(0, 60 * m_plSubTree.Controls.Count);
                btn.Text = String.Format("{0}{1:D1}.{2}", category, nIdx, strMainCategory);
                //btn.UseTextLocation = true;
                btn.Name = String.Format("{0}", strMainCategory);

                btn.TextFont = new Font(m_strFontName, m_nFontSize, FontStyle.Regular);
                btn.TextColor = m_colorFont;
                //btn.TextLocation = m_ptTextLocation;

                btn.Size = new Size(m_nButtonWidth, m_nButtonHeight);
                btn.Click += btnSubCategory_Click;
                btn.MouseHover += btnSubCategory_MouseHover;
                btn.MouseLeave += btnSubCategory_MouseLeave;
                nIdx++;

                m_plSubTree.Controls.Add(btn);
            }
        }

        private void btnSubCategory_Click(object sender, EventArgs e)
        {
            string strCategory; 
            string strCategoryName; 

            int nIndex;

            TreeButton btnSelect = (TreeButton)sender;
            strCategoryName = btnSelect.Name;

            if (btnSelect.IsChecked == false)
            {
                foreach (TreeButton btn in m_plSubTree.Controls)
                {
                    // 클릭시 기존 버튼들 초기화
                    btn.IsChecked = false;
                    //btn.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                    btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                    btn.TextColor = m_colorFont;
                    btn.Refresh();
                }

                btnSelect.IsChecked = true;
                //btnSelect.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;
                btnSelect.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;
                btnSelect.TextColor = m_colorClickFont;
                btnSelect.Refresh();

                strCategory = btnSelect.Text;
                nIndex = strCategory.IndexOf((char)0x2E);
                strCategory = strCategory.Substring(0, nIndex);

                DisasterTree disasterTree = new DisasterTree(m_plDisasterTree, m_form);
                disasterTree.LoadDisasterTree(btnSelect, strCategory);
            }

        }

        private void btnSubCategory_MouseHover(object sender, EventArgs e)
        {
            TreeButton btn = (TreeButton)sender;

            if (btn.IsChecked == false)
            {
                btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain_clicked;
                btn.TextColor = m_colorClickFont;
                btn.Refresh();
            }
        }

        private void btnSubCategory_MouseLeave(object sender, EventArgs e)
        {
            TreeButton btn = (TreeButton)sender;

            if (btn.IsChecked == false)
            {
                btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListMain;
                btn.TextColor = m_colorFont;
                btn.Refresh();
            }
        }

    }

    public class DisasterTree
    {
        Panel m_plDisasterTree;
        PopupSOPList m_form;

        private Point m_ptTextLocation = new Point(30, 12);

        private int m_nButtonWidth = 400;
        private int m_nButtonHeight = 50;
        private int m_nFontSize = 13;

        private string m_strFontName = "나눔바른고딕";

        private Color m_colorFont = Color.Black;
        private Color m_colorClickFont = Color.White;

        public DisasterTree(Panel pl, PopupSOPList form)
        {
            m_plDisasterTree = pl;
            m_form = form;
        }

        public void LoadDisasterTree(TreeButton btnSubCategory, string category)
        {
            int nIdx = 1;
            string selectCategory = btnSubCategory.Name;
            m_form.m_btnSelect = null;

            List<string> listDisasterCategory = new List<string>();
            UnE.SOP.SOPManager sopMgr = FormSOP.Instance.SOPManager;
            Dictionary<string, DisasterInfo> dicSOP = sopMgr.GetSOPDictionary(m_form.IsRegular, m_form.IsNormal);

            m_plDisasterTree.Controls.Clear();

            foreach (KeyValuePair<string, DisasterInfo> pair in dicSOP)
            {
                string strFullPath = pair.Key;

                int nIndex1 = strFullPath.IndexOf((char)0x06);
                int nIndex2 = strFullPath.LastIndexOf((char)0x06);
                if (nIndex1 < 0 || nIndex2 < 0) continue;

                string strCategoryName = strFullPath.Substring(0, nIndex1);
                string strSubCategoryName = strFullPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                string strDisasterName = strFullPath.Substring(nIndex2 + 1);

                if (selectCategory == strSubCategoryName)
                {
                    if (!listDisasterCategory.Contains(strDisasterName))
                        listDisasterCategory.Add(strDisasterName);
                }

            }

            foreach (string strDisasterCategory in listDisasterCategory)
            {
                TreeButton btn = new TreeButton();
                btn.Parent = btnSubCategory;

                //btn.BackgroundImage = global::SOPMonitoringSystem.Properties.Resources.btnSOPListSub;
                btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListSub;
                btn.ImageMouseOver = global::SOPMonitoringSystem.Properties.Resources.btnSOPListSub_clicked;
                btn.ImageClicked = global::SOPMonitoringSystem.Properties.Resources.btnSOPListSub_clicked;

                btn.Location = new Point(0, 60 * m_plDisasterTree.Controls.Count);
                btn.Text = String.Format("{0}{1:D2}.{2}", category, nIdx, strDisasterCategory);
                //btn.UseTextLocation = true;
                btn.Name = String.Format("{0}", strDisasterCategory);

                btn.TextFont = new Font(m_strFontName, m_nFontSize, FontStyle.Regular);
                btn.TextColor = m_colorFont;
                //btn.TextLocation = m_ptTextLocation;

                btn.Size = new Size(m_nButtonWidth, m_nButtonHeight);
                btn.Click += btnDisasterCategory_Click;
                btn.MouseHover += btnDisasterCategory_MouseHover;
                btn.MouseLeave += btnDisasterCategory_MouseLeave;
                
                btn.MouseDoubleClick += btnDisasterCategory_DoubleClicked;
                
                nIdx++;

                m_plDisasterTree.Controls.Add(btn);
            }
        }



        private void btnDisasterCategory_Click(object sender, EventArgs e)
        {
            TreeButton btnSelect = (TreeButton)sender;

            if (btnSelect.IsChecked == false)
            {
                foreach (TreeButton btn in m_plDisasterTree.Controls)
                {
                    //클릭시 기존 버튼들 상태 초기화
                    btn.IsChecked = false;
                    btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListSub;
                    btn.TextColor = m_colorFont;
                    btn.Refresh();
                }

                btnSelect.IsChecked = true;
                btnSelect.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListSub_clicked;
                btnSelect.TextColor = m_colorClickFont;
                btnSelect.Refresh();
            }

            m_form.m_btnSelect = btnSelect;
        }

        private void btnDisasterCategory_MouseHover(object sender, EventArgs e)
        {
            TreeButton btn = (TreeButton)sender;

            if (btn.IsChecked == false)
            {
                btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListSub_clicked;
                btn.TextColor = m_colorClickFont;
                btn.Refresh();
            }
        }

        private void btnDisasterCategory_MouseLeave(object sender, EventArgs e)
        {
            TreeButton btn = (TreeButton)sender;

            if (btn.IsChecked == false)
            {
                btn.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.btnSOPListSub;
                btn.TextColor = m_colorFont;
                btn.Refresh();
            }
        }

        private void btnDisasterCategory_DoubleClicked(object sender, EventArgs e)
        {
            if (sender == null)
                return;

            m_form.btnLoadSOP_Click(null, null);
        }

    }
}
