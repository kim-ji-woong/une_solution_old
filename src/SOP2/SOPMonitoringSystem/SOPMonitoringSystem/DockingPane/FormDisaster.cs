using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.GUI;
using System.Collections;

namespace SOPMonitoringSystem
{
    public partial class FormDisaster : Form, IRibbonButtonOwner
    {
        private IFormDisasterOwner m_frmOwner = null;

        private RibbonButton m_btnSelectedCategory = null;
        private RibbonButton m_btnSelectedSubCategory = null;
        private RibbonButton m_btnSelectedDisaster = null;

        private Dictionary<string, RibbonButton> m_dicCategoryButton = new Dictionary<string, RibbonButton>();
        private Dictionary<RibbonButton, ArrayList> m_dicSubCategoryButton = new Dictionary<RibbonButton, ArrayList>();
        private Dictionary<RibbonButton, ArrayList> m_dicDisasterButton = new Dictionary<RibbonButton, ArrayList>();

        private Dictionary<string, Image> m_dicSubCategoryImage = new Dictionary<string, Image>();

        private Point m_ptCategoryTextLocation = new Point(120, 25);
        private Point m_ptSubCategoryTextLocation = new Point(130, 16);//new Point(150, 26);
        private Point m_ptDisasterTextLocation = new Point(40, 16);//new Point(50, 26);
        private Rectangle m_rectSubCategoryImage = new Rectangle(50, 16, 32, 32);
        private Rectangle m_rectDisasterImage = new Rectangle(5, 16, 32, 32);
        private int m_nSubCategoryButtonHeight = 64;
        private int m_nDisasterButtonHeight = 64;

        private int m_nInitCategoryPanelPos = 0;
        private int m_nInitSubCategoryPanelPos = 0;
        private int m_nInitDisasterPanelPos = 0;

        private int m_nTargetPosition = 0;
        private int m_nCurrentPosition = 0;
        private int m_nMoveDistance = 0;

        private int m_nButtonGap = 24;

        private int m_nCategoryFontSize = 32;
        private int m_nSubCategoryFontSize = 24;
        private Color m_colorButton = Color.AntiqueWhite;//Color.FromArgb(255, 128, 255);
        private string m_strFontName = "HYHeadLine";

        public enum ViewMode { CATEGORY = 0, SUB_CATEGORY, DISASTER, NONE };
        private ViewMode m_mode = ViewMode.NONE;

        public ViewMode Mode
        {
            get { return m_mode; }
            set
            {
                if (m_mode != value)
                {
                    if (value == ViewMode.CATEGORY)
                    {
                        EnableButton(true, false);
                        EnableButton(false, m_btnSelectedCategory != null);
                        //MovePanels(m_nInitCategoryPanelPos);
                    }
                    else if (value == ViewMode.SUB_CATEGORY)
                    {
                        EnableButton(true, true);
                        EnableButton(false, m_btnSelectedSubCategory != null);
                        //MovePanels(m_nInitSubCategoryPanelPos);
                    }
                    else if (value == ViewMode.DISASTER)
                    {
                        EnableButton(true, true);
                        EnableButton(false, false);
                        //MovePanels(m_nInitDisasterPanelPos);
                    }

                    MovePanels(value);
                    m_mode = value;

                    Refresh();
                }
            }
        }

        public IFormDisasterOwner FormOwner
        {
            get { return m_frmOwner; }
            set { m_frmOwner = value; }
        }

        public FormDisaster(IFormDisasterOwner owner = null)
        {
            InitializeComponent();
            this.TopLevel = false;

            m_frmOwner = owner;

            timerAnimation.Tag = null;
            InitPanels();
            InitButtons();

            this.DoubleBuffered = true;
        }

        private void InitPanels()
        {
            m_mode = ViewMode.CATEGORY;

            panelCategory.Location = new Point(0, 0);
            panelSubCategory.Location = new Point(panelCategory.Location.X + panelCategory.Size.Width, 0);
            panelDisaster.Location = new Point(panelSubCategory.Location.X + panelSubCategory.Size.Width, 0);

            m_nInitCategoryPanelPos = panelCategory.Location.X;
            m_nInitSubCategoryPanelPos = panelSubCategory.Location.X;
            m_nInitDisasterPanelPos = panelDisaster.Location.X;

            panelSubCategory.Controls.Clear();
            panelDisaster.Controls.Clear();

            m_dicSubCategoryButton.Clear();
            m_dicDisasterButton.Clear();

            btnFire.Enabled = btnNaturalDisaster.Enabled = btnPollution.Enabled = btnTyphoon.Enabled = false;
            btnTerror.Enabled = btnSavingLife.Enabled = btnETC.Enabled = btnExplosion.Enabled = false;

            EnableButton(true, false);
            EnableButton(false, false);
        }

        private int GetRibbonButtonLength()
        {
            return btnFire.Size.Width;
        }

        private void InitButtons()
        {
            btnFire.Owner = this;
            btnNaturalDisaster.Owner = this;
            btnPollution.Owner = this;
            btnTyphoon.Owner = this;
            btnTerror.Owner = this;
            btnSavingLife.Owner = this;
            btnETC.Owner = this;
            btnExplosion.Owner = this;

            ResizeButtons(panelCategory);
            //btnFire.Size = btnNaturalDisaster.Size = btnPollution.Size = btnTyphoon.Size = btnTerror.Size = btnSavingLife.Size = btnETC.Size = btnExplosion.Size = new Size(250, btnFire.Size.Height);

            m_dicCategoryButton["화재"] = btnFire;
            m_dicCategoryButton["자연재해"] = btnNaturalDisaster;
            m_dicCategoryButton["유출사고"] = btnPollution;
            m_dicCategoryButton["테러"] = btnTerror;
            m_dicCategoryButton["인명구조 및 의료지원"] = btnSavingLife;
            m_dicCategoryButton["기타"] = btnETC;
            m_dicCategoryButton["폭발"] = btnExplosion;
            m_dicCategoryButton["태풍"] = btnTyphoon;

            m_dicSubCategoryImage["태풍"] = global::SOPMonitoringSystem.Properties.Resources.btn_sub_typhoon;
            m_dicSubCategoryImage["지진"] = global::SOPMonitoringSystem.Properties.Resources.btn_sub_earthquake;
            m_dicSubCategoryImage["폭설"] = global::SOPMonitoringSystem.Properties.Resources.btn_sub_snowfall;
            m_dicSubCategoryImage["침수"] = global::SOPMonitoringSystem.Properties.Resources.btn_sub_flooding;
            m_dicSubCategoryImage["일반재해"] = global::SOPMonitoringSystem.Properties.Resources.btnEtc_User;
            m_dicSubCategoryImage["화재"] = global::SOPMonitoringSystem.Properties.Resources.btn_sub_fire;
            m_dicSubCategoryImage["오염"] = global::SOPMonitoringSystem.Properties.Resources.btn_sub_spill;
            m_dicSubCategoryImage["테러"] = global::SOPMonitoringSystem.Properties.Resources.btn_sub_terror;
            m_dicSubCategoryImage["폭발"] = global::SOPMonitoringSystem.Properties.Resources.btn_sub_volcano;
            m_dicSubCategoryImage["119상황"] = global::SOPMonitoringSystem.Properties.Resources.btn_sub_119;
            m_dicSubCategoryImage["SOP상황"] = global::SOPMonitoringSystem.Properties.Resources.btn_sub_sop;

            SetButtonInfo(btnFire, m_nCategoryFontSize, m_colorButton, m_strFontName, m_ptCategoryTextLocation);
            SetButtonInfo(btnNaturalDisaster, m_nCategoryFontSize, m_colorButton, m_strFontName, m_ptCategoryTextLocation);
            SetButtonInfo(btnTyphoon, m_nCategoryFontSize, m_colorButton, m_strFontName, m_ptCategoryTextLocation);
            SetButtonInfo(btnPollution, m_nCategoryFontSize, m_colorButton, m_strFontName, m_ptCategoryTextLocation);
            SetButtonInfo(btnETC, m_nCategoryFontSize, m_colorButton, m_strFontName, m_ptCategoryTextLocation);
            SetButtonInfo(btnTerror, m_nCategoryFontSize, m_colorButton, m_strFontName, m_ptCategoryTextLocation);
            SetButtonInfo(btnExplosion, m_nCategoryFontSize, m_colorButton, m_strFontName, m_ptCategoryTextLocation);
            SetButtonInfo(btnSavingLife, m_nCategoryFontSize, m_colorButton, m_strFontName, m_ptCategoryTextLocation);
        }

        private void SetButtonInfo(RibbonButton btn, int nFontSize, Color color, string strFontName, Point ptTextLocation)
        {
            btn.Font = new Font(strFontName, nFontSize, FontStyle.Bold);
            btn.ForeColor = color;
            btn.TextLocation = ptTextLocation;
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton btn = (RibbonButton)sender;
            TreeNode node = (TreeNode)btn.Tag;

            if (node == null)
                return;

            if (node.Level == 0)
                SelectCategory(btn);
            else if (node.Level == 1)
                SelectSubCategory(btn);
            else if (node.Level == 2)
            {
                if (m_btnSelectedDisaster != btn)
                {
                    if (m_frmOwner != null)
                    {
                        m_frmOwner.OnTreeViewClicked(node, false);
                        /*m_frmOwner.SelectNode(node);
                        m_frmOwner.SelectSop(node);*/
                    }

                    SelectDisaster(btn);
                }
            }
        }

        private void SelectCategory(RibbonButton btn, bool noModeChange = false)
        {
            if (m_btnSelectedCategory == btn)
            {
                if (btn != null && !noModeChange)
                    Mode = ViewMode.SUB_CATEGORY;
                return;
            }

            if (m_btnSelectedCategory != null)
            {
                m_btnSelectedCategory.IsChecked = false;
                m_btnSelectedCategory.Refresh();
            }

            if (btn != null)
            {
                btn.IsChecked = true;
                btn.Refresh();
            }

            m_btnSelectedCategory = btn;
            m_btnSelectedSubCategory = null;
            m_btnSelectedDisaster = null;

            panelSubCategory.Controls.Clear();
            panelDisaster.Controls.Clear();

            if (btn == null)
                return;

            if (!m_dicSubCategoryButton.ContainsKey(btn))
                return;

            ArrayList arrSubCategoryButtons = m_dicSubCategoryButton[btn];

            foreach (RibbonButton btnSubCategory in arrSubCategoryButtons)
            {
                panelSubCategory.Controls.Add(btnSubCategory);
            }

            if (!noModeChange)
                Mode = ViewMode.SUB_CATEGORY;
        }

        private void SelectSubCategory(RibbonButton btn, bool noModeChange = false)
        {
            if (m_btnSelectedSubCategory == btn)
            {
                if (btn != null && !noModeChange)
                    Mode = ViewMode.DISASTER;
                return;
            }

            if (m_btnSelectedSubCategory != null)
            {
                m_btnSelectedSubCategory.IsChecked = false;
                m_btnSelectedSubCategory.Refresh();
            }

            if (btn != null)
            {
                btn.IsChecked = true;
                btn.Refresh();
            }

            m_btnSelectedSubCategory = btn;
            m_btnSelectedDisaster = null;

            panelDisaster.Controls.Clear();

            if (!m_dicDisasterButton.ContainsKey(btn))
                return;

            ArrayList arrDisasterButtons = m_dicDisasterButton[btn];

            foreach (RibbonButton btnDisaster in arrDisasterButtons)
            {
                panelDisaster.Controls.Add(btnDisaster);
            }

            if (!noModeChange)
                Mode = ViewMode.DISASTER;
        }

        private void SelectDisaster(RibbonButton btn)
        {
            if (m_btnSelectedDisaster == btn)
                return;

            if (m_btnSelectedDisaster != null)
            {
                m_btnSelectedDisaster.IsChecked = false;
                m_btnSelectedDisaster.Refresh();
            }

            if (btn != null)
            {
                btn.IsChecked = true;
                btn.Refresh();
            }

            m_btnSelectedDisaster = btn;
        }

        // Sub Category가 존재하지 않는 버튼들은 아래쪽으로 보낸다.
        private void ReorderCategoryButtons()
        {
            ArrayList arrHasChildren = new ArrayList();
            ArrayList arrHasNoChildren = new ArrayList();

            foreach (Control ctrl in panelCategory.Controls)
            {
                if (ctrl.GetType() != typeof(RibbonButton))
                    continue;

                RibbonButton btn = (RibbonButton)ctrl;

                if (!m_dicSubCategoryButton.ContainsKey(btn))
                {
                    arrHasNoChildren.Add(btn);
                    continue;
                }

                ArrayList arrSubCategoryButtons = m_dicSubCategoryButton[btn];

                if (arrSubCategoryButtons.Count > 0)
                    arrHasChildren.Add(btn);
                else
                    arrHasNoChildren.Add(btn);
            }

            int nPos = 0;
            
            foreach (RibbonButton btn in arrHasChildren)
            {
                btn.Location = new Point(btn.Location.X, nPos);
                nPos += btn.Size.Height;
            }

            foreach (RibbonButton btn in arrHasNoChildren)
            {
                btn.Location = new Point(btn.Location.X, nPos);
                nPos += btn.Size.Height;
            }
        }

        private void VisiblePanels(bool visible)
        {
            panelCategory.Visible = visible;
            panelSubCategory.Visible = visible;
            panelDisaster.Visible = visible;
        }

        // SOP를 로딩한 후 Disaster Mode로 바꾼다.
        public bool LoadSOP(TreeNodeCollection nodes, string strCategoryName, string strSubCategoryName)
        {
            // 깜빡임 방지를 위하여 Panel들을 안보이게 한다.
            VisiblePanels(false);

            _LoadSOP(nodes, true);

            if (!m_dicCategoryButton.ContainsKey(strCategoryName))
            {
                VisiblePanels(true);
                return false;
            }

            RibbonButton btnCategory = m_dicCategoryButton[strCategoryName];
            SelectCategory(btnCategory, true);

            if (!m_dicSubCategoryButton.ContainsKey(btnCategory))
            {
                VisiblePanels(true);
                return false;
            }

            RibbonButton btnSubCategory = null;
            ArrayList arrSubCategoryButtons = m_dicSubCategoryButton[btnCategory];

            foreach (RibbonButton btn in arrSubCategoryButtons)
            {
                if (btn.Text == strSubCategoryName)
                {
                    btnSubCategory = btn;
                    break;
                }
            }

            if (btnSubCategory == null)
            {
                VisiblePanels(true);
                return false;
            }

            SelectSubCategory(btnSubCategory, true);

            m_mode = ViewMode.DISASTER;

            panelDisaster.Location = new Point(0, panelDisaster.Location.Y);
            panelSubCategory.Location = new Point(panelDisaster.Location.X - panelSubCategory.Size.Width, panelSubCategory.Location.Y);
            panelCategory.Location = new Point(panelSubCategory.Location.X - panelCategory.Size.Width, panelCategory.Location.Y);

            VisiblePanels(true);
            return true;
        }

        public void LoadSOP(TreeNodeCollection nodes)
        {
            _LoadSOP(nodes, false);

            // Sub Category가 존재하지 않는 버튼들은 아래쪽으로 보낸다.
            ReorderCategoryButtons();

            Mode = ViewMode.CATEGORY;
        }

        private void _LoadSOP(TreeNodeCollection nodes, bool noModeChange)
        {
            SelectCategory(null, noModeChange);
            InitPanels();

            foreach (TreeNode node in nodes)
            {
                if (node.Nodes.Count == 0)
                    continue;

                if (!m_dicCategoryButton.ContainsKey(node.Text))
                    continue;

                RibbonButton btnCategory = m_dicCategoryButton[node.Text];
                btnCategory.Enabled = true;
                btnCategory.Tag = node;

                LoadCategory(btnCategory, node.Nodes);
            }
        }

        private void LoadCategory(RibbonButton btnCategory, TreeNodeCollection nodes)
        {
            ArrayList arrSubCategoryButtons = new ArrayList();
            m_dicSubCategoryButton[btnCategory] = arrSubCategoryButtons;

            int nCount = 0;

            foreach (TreeNode node in nodes)
            {
                if (node.Nodes.Count == 0)
                    continue;

                if (!m_dicSubCategoryImage.ContainsKey(node.Text))
                    continue;

                Image img = m_dicSubCategoryImage[node.Text];

                RibbonButton btn = new RibbonButton(panelSubCategory.Size.Width);

                btn.NormalImage = img;
                btn.CheckedImage = img;
                btn.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
                btn.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;

                btn.Location = new Point(0, m_nSubCategoryButtonHeight * nCount++);
                btn.Text = node.Text;

                btn.UseCustomImageRect = true;
                btn.UseTextLocation = true;
                btn.CustomImageRect = m_rectSubCategoryImage;
                //btn.TextLocation = m_ptSubCategoryTextLocation;
                btn.TextPos = RibbonButton.TextPosition.RIGHT;
                btn.BackColor = Color.Transparent;

                btn.Owner = this;
                btn.Tag = node;
                btn.Size = new Size(GetRibbonButtonLength(), m_nSubCategoryButtonHeight);

                SetButtonInfo(btn, m_nSubCategoryFontSize, m_colorButton, m_strFontName, m_ptSubCategoryTextLocation);

                arrSubCategoryButtons.Add(btn);

                LoadSubCategory(btn, node.Nodes);
            }
        }

        private void LoadSubCategory(RibbonButton btnSubCategory, TreeNodeCollection nodes)
        {
            ArrayList arrDisasterButtons = new ArrayList();
            m_dicDisasterButton[btnSubCategory] = arrDisasterButtons;

            int nCount = 0;

            foreach (TreeNode node in nodes)
            {
                if (node.Nodes.Count == 0)
                    continue;

                Image img = btnSubCategory.NormalImage;

                RibbonButton btn = new RibbonButton(panelDisaster.Size.Width);

                btn.NormalImage = img;
                btn.CheckedImage = img;
                btn.MouseOverBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;
                btn.CheckedBkgndImage = global::SOPMonitoringSystem.Properties.Resources.LeftBar_Click_Area;

                btn.Location = new Point(0, m_nDisasterButtonHeight * nCount++);
                btn.Text = node.Text;                

                btn.UseCustomImageRect = true;
                btn.UseTextLocation = true;
                btn.CustomImageRect = m_rectDisasterImage;
                //btn.TextLocation = m_ptDisasterTextLocation;
                btn.TextPos = RibbonButton.TextPosition.RIGHT;
                btn.BackColor = Color.Transparent;

                btn.Owner = this;
                btn.Tag = node;
                btn.Size = new Size(GetRibbonButtonLength(), m_nDisasterButtonHeight);

                //SetButtonInfo(btn, m_nSubCategoryFontSize, m_colorButton, m_strFontName, m_ptDisasterTextLocation);
                SetButtonInfo(btn, 16, m_colorButton, m_strFontName, new Point(40, 20));

                ToolTip tooltip = new ToolTip();
                tooltip.SetToolTip(btn, btn.Text);

                arrDisasterButtons.Add(btn);
            }
        }

        public void LoadSOP(TreeNode node)
        {
            if (node.Level < 2)
                return;

            TreeNode nodeCategory = null, nodeSubCategory = null, nodeDisaster = null;

            while (node != null)
            {
                if (node.Level == 2)
                    nodeDisaster = node;
                else if (node.Level == 1)
                    nodeSubCategory = node;
                else if (node.Level == 0)
                    nodeCategory = node;

                node = node.Parent;
            }

            if (nodeCategory != null && nodeSubCategory != null && nodeDisaster != null)
                LoadSOP(nodeCategory.Text, nodeSubCategory.Text, nodeDisaster.Text);
        }

        private RibbonButton FindSubCategory(RibbonButton btnCategory, string strSubCategoryName)
        {
            if (!m_dicSubCategoryButton.ContainsKey(btnCategory))
                return null;

            ArrayList arrSubCategoryButton = m_dicSubCategoryButton[btnCategory];

            foreach (RibbonButton btn in arrSubCategoryButton)
            {
                if (btn.Text == strSubCategoryName)
                    return btn;
            }

            return null;
        }

        private RibbonButton FindDisaster(RibbonButton btnSubCategory, string strDisasterName)
        {
            if (!m_dicDisasterButton.ContainsKey(btnSubCategory))
                return null;

            ArrayList arrDisasterButton = m_dicDisasterButton[btnSubCategory];

            foreach (RibbonButton btn in arrDisasterButton)
            {
                if (btn.Text == strDisasterName)
                    return btn;
            }

            return null;
        }

        private void LoadSOP(string strCategoryName, string strSubCategoryName, string strDisasterName)
        {
            if (!m_dicCategoryButton.ContainsKey(strCategoryName))
                return;

            RibbonButton btnCategory = m_dicCategoryButton[strCategoryName];
            SelectCategory(btnCategory);

            RibbonButton btnSubCategory = FindSubCategory(btnCategory, strSubCategoryName);

            if (btnSubCategory == null)
                return;

            SelectSubCategory(btnSubCategory);

            RibbonButton btnDisaster = FindDisaster(btnSubCategory, strDisasterName);

            if (btnDisaster == null)
                return;

            SelectDisaster(btnDisaster);
            Mode = ViewMode.DISASTER;
        }

        /*private void MovePanels(int nMove)
        {
            panelCategory.Location = new Point(m_nInitCategoryPanelPos - nMove, panelCategory.Location.Y);
            panelSubCategory.Location = new Point(m_nInitSubCategoryPanelPos - nMove, panelSubCategory.Location.Y);
            panelDisaster.Location = new Point(m_nInitDisasterPanelPos - nMove, panelDisaster.Location.Y);
        }*/

        private int GetInitPosition(ViewMode mode)
        {
            if (mode == ViewMode.CATEGORY)
                return m_nInitCategoryPanelPos;
            else if (mode == ViewMode.SUB_CATEGORY)
                return m_nInitSubCategoryPanelPos;
            else if (mode == ViewMode.DISASTER)
                return m_nInitDisasterPanelPos;

            return -1;
        }

        private void MovePanels(ViewMode mode)
        {
            if (timerAnimation.Tag != null)
            {
                CompleteMove();
                //return;
            }

            if (m_mode == mode)
                return;

            m_nCurrentPosition = GetInitPosition(m_mode);
            m_nTargetPosition = GetInitPosition(mode);

            if (m_nCurrentPosition < 0 || m_nTargetPosition < 0)
                return;

            int nFrameCount = 20;
            m_nMoveDistance = (m_nTargetPosition - m_nCurrentPosition) / nFrameCount;

            StartTimer(mode);
        }

        private void EnableScroll(bool enabled)
        {
            panelCategory.AutoScroll = enabled;
            panelSubCategory.AutoScroll = enabled;
            panelDisaster.AutoScroll = enabled;
        }

        private void EnableButton(bool isPrevButton, bool enabled)
        {
            if (m_frmOwner != null)
                m_frmOwner.EnableButton(isPrevButton, enabled);
        }

        public void GoBack()
        {
            // 타이머가 동작중일때는 입력을 받지 않는다.
            if (timerAnimation.Tag != null)
                return;

            Mode = (ViewMode)((int)m_mode - 1);
        }

        public void GoForward()
        {
            // 타이머가 동작중일때는 입력을 받지 않는다.
            if (timerAnimation.Tag != null)
                return;

            Mode = (ViewMode)((int)m_mode + 1);
        }

        private void MovePanel(int nMove)
        {
            if (nMove > 0)
            {
                if (panelCategory.Location.X > 0 && panelCategory.Location.X - nMove < 0)
                    nMove = panelCategory.Location.X;
                else if (panelSubCategory.Location.X > 0 && panelSubCategory.Location.X - nMove < 0)
                    nMove = panelSubCategory.Location.X;
                else if (panelDisaster.Location.X > 0 && panelDisaster.Location.X - nMove < 0)
                    nMove = panelDisaster.Location.X;
            }
            else
            {
                if (panelCategory.Location.X < 0 && panelCategory.Location.X - nMove > 0)
                    nMove = panelCategory.Location.X;
                else if (panelSubCategory.Location.X < 0 && panelSubCategory.Location.X - nMove > 0)
                    nMove = panelSubCategory.Location.X;
                else if (panelDisaster.Location.X < 0 && panelDisaster.Location.X - nMove > 0)
                    nMove = panelDisaster.Location.X;
            }

            panelSubCategory.Visible = panelSubCategory.Location.X - nMove < this.Size.Width;
            panelDisaster.Visible = panelDisaster.Location.X - nMove < this.Size.Width;

            if (panelSubCategory.Location.X - nMove >= 0 && panelSubCategory.Location.X - nMove < this.Size.Width)
                panelSubCategory.Size = new Size(this.Size.Width - (panelSubCategory.Location.X - nMove), panelSubCategory.Size.Height);

            if (panelDisaster.Location.X - nMove >= 0 && panelDisaster.Location.X - nMove < this.Size.Width)
                panelDisaster.Size = new Size(this.Size.Width - (panelDisaster.Location.X - nMove), panelDisaster.Size.Height);

            panelCategory.Location = new Point(panelCategory.Location.X - nMove, panelCategory.Location.Y);
            panelSubCategory.Location = new Point(panelSubCategory.Location.X - nMove, panelSubCategory.Location.Y);
            panelDisaster.Location = new Point(panelDisaster.Location.X - nMove, panelDisaster.Location.Y);

            m_nCurrentPosition += nMove;
        }

        private void timerAnimation_Tick(object sender, EventArgs e)
        {
            lock (timerAnimation)
            {
                int nMoveDistance = m_nMoveDistance;

                if (m_nMoveDistance < 0)
                {
                    if (m_nCurrentPosition + m_nMoveDistance < m_nTargetPosition)
                        nMoveDistance = m_nTargetPosition - m_nCurrentPosition;
                }
                else if (m_nMoveDistance > 0)
                {
                    if (m_nCurrentPosition + m_nMoveDistance > m_nTargetPosition)
                        nMoveDistance = m_nTargetPosition - m_nCurrentPosition;
                }

                MovePanel(nMoveDistance);
                
                if (m_nMoveDistance < 0)
                {
                    if (m_nCurrentPosition <= m_nTargetPosition)
                        StopTimer();
                }
                else if (m_nMoveDistance > 0)
                {
                    if (m_nCurrentPosition >= m_nTargetPosition)
                        StopTimer();
                }
                else
                    StopTimer();
            }
        }

        private void CompleteMove()
        {
            StopTimer();

            lock (timerAnimation)
            {
                int nMoveDistance = m_nTargetPosition - m_nCurrentPosition;
                MovePanel(nMoveDistance);
            }
        }

        private void StartTimer(ViewMode mode)
        {
            Panel panelTarget = null;

            if (mode == ViewMode.CATEGORY)
                panelTarget = panelCategory;
            else if (mode == ViewMode.SUB_CATEGORY)
                panelTarget = panelSubCategory;
            else if (mode == ViewMode.DISASTER)
                panelTarget = panelDisaster;

            EnableScroll(false);
            timerAnimation.Tag = panelTarget;
            timerAnimation.Start();
        }

        private void StopTimer()
        {
            Panel panelTarget = (Panel)timerAnimation.Tag;

            EnableScroll(true);
            timerAnimation.Tag = null;
            timerAnimation.Stop();

            if (m_mode == ViewMode.CATEGORY)
                ResizeButtons(panelCategory);
            else if (m_mode == ViewMode.SUB_CATEGORY)
                ResizeButtons(panelSubCategory);
            else if (m_mode == ViewMode.DISASTER)
                ResizeButtons(panelDisaster);

            FormDisaster_Resize(null, null);
        }

        private void FormDisaster_Resize(object sender, EventArgs e)
        {
            panelCategory.Size = this.Size;
            panelSubCategory.Size = this.Size;
            panelDisaster.Size = this.Size;

            if (m_mode == ViewMode.CATEGORY)
            {
                panelSubCategory.Location = new Point(panelCategory.Location.X + panelCategory.Size.Width, panelSubCategory.Location.Y);
                panelDisaster.Location = new Point(panelSubCategory.Location.X + panelSubCategory.Size.Width, panelDisaster.Location.Y);
            }
            else if (m_mode == ViewMode.SUB_CATEGORY)
            {
                panelCategory.Location = new Point(panelSubCategory.Location.X - panelCategory.Size.Width, panelCategory.Location.Y);
                panelDisaster.Location = new Point(panelSubCategory.Location.X + panelSubCategory.Size.Width, panelDisaster.Location.Y);
            }
            else if (m_mode == ViewMode.DISASTER)
            {
                panelSubCategory.Location = new Point(panelDisaster.Location.X - panelSubCategory.Size.Width, panelSubCategory.Location.Y);
                panelCategory.Location = new Point(panelSubCategory.Location.X - panelCategory.Size.Width, panelCategory.Location.Y);
            }

            m_nInitCategoryPanelPos = 0;
            m_nInitSubCategoryPanelPos = panelCategory.Size.Width;
            m_nInitDisasterPanelPos = panelCategory.Size.Width + panelSubCategory.Size.Width;

            ResizeButtons(panelCategory);
            ResizeButtons(panelSubCategory);
            ResizeButtons(panelDisaster);
        }

        private void ResizeButtons(Panel panel)
        {
            foreach (Control ctrl in panel.Controls)
            {
                if (ctrl.GetType() == typeof(RibbonButton))
                {
                    ctrl.Size = new Size(panel.Size.Width - m_nButtonGap, ctrl.Size.Height);
                }
            }
        }

        public bool SelectSOP(TreeNode node)
        {
            if (m_mode != ViewMode.DISASTER)
                return false;

            foreach (Control ctrl in panelDisaster.Controls)
            {
                if (ctrl.GetType() != typeof(RibbonButton))
                    continue;

                if (ctrl.Tag == node)
                {
                    RibbonButton btn = (RibbonButton)ctrl;
                    btn.IsChecked = true;

                    if (m_btnSelectedDisaster != btn && m_btnSelectedDisaster != null)
                    {
                        m_btnSelectedDisaster.IsChecked = false;
                        m_btnSelectedDisaster.Refresh();
                    }

                    btn.Refresh();
                    m_btnSelectedDisaster = btn;

                    panelDisaster.VerticalScroll.Value = ctrl.Location.Y;
                    return true;
                }
            }

            return false;
        }
    }

    public interface IFormDisasterOwner
    {
        //void SelectNode(TreeNode node);
        //void SelectSop(TreeNode node);
        void OnTreeViewClicked(TreeNode node, bool noSelect);
        void EnableButton(bool isPrevButton, bool enabled);
    }

    public class PanelDoubleBuffered : Panel
    {
        public PanelDoubleBuffered()
        {
            this.DoubleBuffered = true;
        }
    }
}
