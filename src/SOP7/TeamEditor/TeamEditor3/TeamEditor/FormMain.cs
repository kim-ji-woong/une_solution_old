using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Collections;
using TeamEditor.BLL.WinForms;

namespace TeamEditor
{
    public partial class FormMain : Form
    {
        private TeamEditor.IDAL.IDataManager m_dataManager = null;
        private TeamEditor.BLL.LoadManager m_loadManager = null;

        private WebDBManager m_dbMgr = null;//new WebDBManager("SOP4");
        private int m_nSOPGenUserID = -1;
        private int m_nSiteID = -1;
        private string m_strSiteName = "";
        private string m_strSOPGenUserRealName = "";
        private int m_nSplitDistance = 210;
        private bool m_initSplitDistance = false;

        private TeamEditor.BLL.WinForms.Command.CommandManagerEx m_cmdMgr = null;
        private bool m_closeApplication = false;

        //private NetworkManager m_NetWorkClient = null;

        private UnE.GUI.DialogFormFrameRibbon m_frmTemporaryMemberframe = null;
        private Popup.FormSelectTemporaryMember m_frmTemporaryMember = null;

        // 비상조직과 상시조직을 함께 보여줄 것인가?
        private bool m_useSplitContainerEmergency = false;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        #region Property

        public bool IsEditMode
        {
            get { return rbtnEdit.IsChecked; }
        }

        public TeamEditor.BLL.WinForms.Command.CommandManagerEx CommandManager
        {
            get { return m_cmdMgr; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
        }

        public string SiteName
        {
            get { return m_strSiteName; }
        }

        public ImageList ImageListDrag
        {
            get { return imageListDrag; }
        }

        public bool CloseApplication
        {
            get { return m_closeApplication; }
            set { m_closeApplication = value; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        public TeamTreeView RegularTeamTree
        {
            get { return treeRegularTeam; }
        }

        public TeamTreeView TemporaryNormalTeamTree
        {
            get { return treeNormal; }
        }

        public TeamTreeView TemporaryEmergencyTeamTree
        {
            get { return treeEmergency; }
        }

        public string SearchStr
        {
            get { return txtFindRegularMember.Text; }
        }

        public Color ColCustomBlack
        {
            get
            {
                return Color.FromArgb(43, 43, 43);
            }
        }

        public Color ColCustomOrange
        {
            get
            {
                return Color.FromArgb(245, 168, 44);
            }
        }

        #endregion

        #region 윈도우 해상도 변경에 따른 비율 처리

        //개발 환경 윈도우 해상도
        double DevWindowWidth = 1920;
        double DevWindowHeight = 1040;

        private float WinBoundsWidth = 1920;
        private float WinBoundsHeight = 1040;

        private float CurWinBoundsWidth = 1920;
        private float CurWinBoundsHeight = 1040;

        public float WindowWidthRate = 1f;
        public float WindowHeightRate = 1f;

        public void GetWindowRate()
        {
            CurWinBoundsWidth = Screen.FromControl(this).WorkingArea.Width;
            CurWinBoundsHeight = Screen.FromControl(this).WorkingArea.Height;

            if (CurWinBoundsHeight == 1040) CurWinBoundsHeight += 40;

            WindowWidthRate = (float)Math.Round((double)CurWinBoundsWidth / (double)WinBoundsWidth, 1);
            WindowHeightRate = (float)Math.Round((double)CurWinBoundsHeight / (double)WinBoundsHeight, 1);

            if (WindowWidthRate > 2)
                WindowWidthRate = 2;
            if (WindowHeightRate > 2)
                WindowHeightRate = 2;

            if (WindowWidthRate != 1f || WindowHeightRate != 1f)
            {
                WinBoundsWidth = CurWinBoundsWidth;
                WinBoundsHeight = CurWinBoundsHeight;

                event_WinRateChanged();
            }
        }

        public delegate void WindowRateChanged();
        public event WindowRateChanged event_WinRateChanged;

        #endregion

        PageBackstageOption m_pageOption = null;

        TeamGrid.GridType m_gridType;

        public FormMain(int nSOPGenUserID, /*string strSOPGenUserRealName, */int nSiteID)
        {
            m_instance = this;
            m_dbMgr = new TeamEditor.WebDBManagerEx(nSiteID);

            InitializeComponent();

            m_dataManager = new TeamEditor.DAL.DataManager();
            m_loadManager = new BLL.LoadManager(m_dataManager);


            // size 362, 236
            splitContainerMain.Dock = DockStyle.Fill;

            treeRegularTeam.ValidateLabelEdit += new TeamTreeView.ValidateLabelEditEventHandler(TeamTreeView_ValidateLabelEdit);
            treeNormal.ValidateLabelEdit += new TeamTreeView.ValidateLabelEditEventHandler(TeamTreeView_ValidateLabelEdit);
            treeEmergency.ValidateLabelEdit += new TeamTreeView.ValidateLabelEditEventHandler(TeamTreeView_ValidateLabelEdit);
            treeExternalCompanyTeam.ValidateLabelEdit += new TeamTreeView.ValidateLabelEditEventHandler(TeamTreeView_ValidateLabelEdit);
            txtFindRegularMember.KeyDown += textBox1_KeyDown;
            gridRegularMember.DataError += gridRegularMember_DataError;
            m_nSOPGenUserID = nSOPGenUserID;
            m_nSiteID = nSiteID;
            ReadSiteName();
            //m_strSOPGenUserRealName = strSOPGenUserRealName;

            m_cmdMgr = new TeamEditor.BLL.WinForms.Command.CommandManagerEx(rbtnUndo, rbtnRedo, rbtnSave, rbtnEdit, m_dbMgr);

            gridRegularMember.LinkedTree = treeRegularTeam;
            gridExternal.LinkedTree = treeExternalCompanyTeam;

            SetMergeColumnOfTemporaryGrid();

            m_gridType = TeamGrid.GridType.RegularMember;

            m_pageOption = new PageBackstageOption();
            m_pageOption.Location = new Point(0, 0);
            m_pageOption.Dock = DockStyle.Fill;
            m_pageOption.TopLevel = false;
            m_pageOption.Parent = this;
            m_pageOption.Visible = false;
            panelMain.Controls.Add(m_pageOption);

            UnE.Utility.UMessageBoxRibbon.Font = new System.Drawing.Font(Program.prgFont, 13f, FontStyle.Bold);
            UnE.Utility.UMessageBoxRibbon.FrameColor = ColCustomBlack;
            UnE.Utility.UMessageBoxRibbon.TitleColor = ColCustomOrange;
            UnE.Utility.UMessageBoxRibbon.BackColor = ColCustomBlack;
            UnE.Utility.UMessageBoxRibbon.ForeColor = Color.White;
            UnE.Utility.UMessageBoxRibbon.CloseButtonImage = global::TeamEditor.Properties.Resources.Close_40_40_Default;            
            UnE.Utility.UMessageBoxRibbon.CloseButtonOverImage = global::TeamEditor.Properties.Resources.Close_40_40_Click;

            rbtnRedo.Click += rbtnRedo_Click;
            rbtnUndo.Click += rbtnUndo_Click;

            pnlUserDefine.Location = new Point(0, 0);
            pnlUserDefine.Dock = DockStyle.Fill;
            pnlUserDefine.Visible = false;

            event_WinRateChanged += FormMain_event_WinRateChanged;
            gridRegularMember.AddWinRateChangeEvent();
            gridTemporary.AddWinRateChangeEvent();
            gridExternal.AddWinRateChangeEvent();
            gridUserDefinedTeam.AddWinRateChangeEvent();

            FormMain_event_WinRateChanged();
        }

        public double[] GetCurWindowRate()
        {
            double WindowWidthRate = Math.Round(WinBoundsWidth / DevWindowWidth, 1);
            double WindowHeightRate = Math.Round(WinBoundsHeight / DevWindowHeight, 1);

            if (WindowWidthRate > 2)
                WindowWidthRate = 2;
            if (WindowHeightRate > 2)
                WindowHeightRate = 2;

            return new double[] { WindowWidthRate, WindowHeightRate };
        }

        void FormMain_event_WinRateChanged()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            UnE.Utility.UMessageBoxRibbon.WindowRateWidth = WindowRateWidth;
            UnE.Utility.UMessageBoxRibbon.WindowRateHeight = WindowRateHeight;            

            paneRibbonToolBar.Size = new Size(paneRibbonToolBar.Size.Width, (int)((float)paneRibbonToolBar.Size.Height * WindowHeightRate));
            panelMain.Location = new Point(0, paneRibbonToolBar.Size.Height);

            #region 화면 상단 이미지 크기 및 위치 조절

            List<Control> mctlList = new List<Control>(12);
            Int32 iAddWidth = 0;
            Int32 iAddHeight = 0;

            for (Int32 index = 0; index < paneRibbonToolBar.Controls.Count; index++)
            {
                Control ctl = paneRibbonToolBar.Controls[index];

                mctlList.Add(ctl);

                if (ctl.GetType().Name != "RibbonButton") continue;

                iAddWidth = ((int)((float)ctl.Size.Width * WindowWidthRate)) - ctl.Size.Width;
                iAddHeight = ((int)((float)ctl.Size.Height * WindowHeightRate)) - ctl.Size.Height;

                ctl.Size = new Size(ctl.Size.Width + iAddWidth, ctl.Size.Height + iAddHeight);
                ((UnE.GUI.RibbonButton)ctl).CustomImageRect = new Rectangle(0, 0, ctl.Size.Width, ctl.Size.Height);
            }

            mctlList.Sort(delegate(Control A, Control B)
            {
                if (A.Location.X < B.Location.X) return -1;
                else if (A.Location.X > B.Location.X) return 1;
                return 0;
            });

            Int32 iCtlDistance = 0;
            for (Int32 index = 0; index < mctlList.Count; index++)
            {
                mctlList[index].Location = new Point(iCtlDistance, 6);
                iCtlDistance += mctlList[index].Size.Width + mctlList[index].Margin.Right;
            }

            #endregion            

            gridRegularMember.RowHeight = (int)((float)gridRegularMember.RowHeight * FormMain.Instance.WindowHeightRate);
            gridTemporary.RowHeight = (int)((float)gridTemporary.RowHeight * FormMain.Instance.WindowHeightRate);
            gridExternal.RowHeight = (int)((float)gridExternal.RowHeight * FormMain.Instance.WindowHeightRate);
            gridUserDefinedTeam.RowHeight = (int)((float)gridUserDefinedTeam.RowHeight * FormMain.Instance.WindowHeightRate);            

            UpdateWindowRate(contextMenuRegularTeam, FormMain.Instance.WindowWidthRate,FormMain.Instance.WindowHeightRate);
            UpdateWindowRate(contextMenuTemporaryTeam,FormMain.Instance.WindowWidthRate,FormMain.Instance.WindowHeightRate);
            UpdateWindowRate(contextMenuExternal, FormMain.Instance.WindowWidthRate, FormMain.Instance.WindowHeightRate);
              
            UpdateControlFont(treeRegularTeam);
            UpdateControlFont(treeNormal);
            UpdateControlFont(treeEmergency);
            UpdateControlFont(treeExternalCompanyTeam);
             
            UpdateControlFont(lblTeamPathForRegular);
            UpdateControlFont(txtFindRegularMember);
            UpdateControlFont(button_search);
          
            button_search.Height = txtFindRegularMember.Height;            

            UpdateGridControl(gridRegularMember);

            //평일/야간휴일 비상조직
            UpdateControlFont(lblTeamPathForTemporary);
            UpdateControlFont(label1);
            UpdateControlFont(label2);
            UpdateGridControl(gridTemporary);

            UpdateControlFont(lblTeamPathForExternal);
            UpdateGridControl(gridExternal);

            UpdateControlFont(lblUserDefine);
            UpdateGridControl(gridUserDefinedTeam);

            //
            //Regular
            pnlTitleRegular.Height = (int)((float)pnlTitleRegular.Height * FormMain.Instance.WindowHeightRate);
            gridRegularMember.Location = new Point(0, pnlTitleRegular.Height);

            if (pnlTitleRegular.Height + gridRegularMember.Height > panelRegular.Height)
                panelRegular.Padding = new System.Windows.Forms.Padding(0, 0, 0, ((pnlTitleRegular.Height * 2) + gridRegularMember.Height) - panelRegular.Height);
            else
                panelRegular.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);


            lblTeamPathForTemporary.Height = (int)((float)lblTeamPathForTemporary.Height * FormMain.Instance.WindowHeightRate);
            pnlTemporaryBand.Location = new Point(0, lblTeamPathForTemporary.Height);
            pnlTemporaryBand.Height = (int)((float)pnlTemporaryBand.Height * FormMain.Instance.WindowHeightRate);
            gridTemporary.Location = new Point(0, pnlTemporaryBand.Location.Y + pnlTemporaryBand.Height);

            if (lblTeamPathForTemporary.Height + pnlTemporaryBand.Height + gridTemporary.Height > panelTemporary.Height)
                panelTemporary.Padding = new System.Windows.Forms.Padding(0, 0, 0, (lblTeamPathForTemporary.Height + (pnlTemporaryBand.Height*2) + gridTemporary.Height) - panelTemporary.Height);
            else
                panelTemporary.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);


            lblTeamPathForExternal.Height = (int)((float)lblTeamPathForExternal.Height * FormMain.Instance.WindowHeightRate);
            gridExternal.Location = new Point(0, lblTeamPathForExternal.Height);

            if (lblTeamPathForExternal.Height + gridExternal.Height > panelExternal.Height)
                panelExternal.Padding = new System.Windows.Forms.Padding(0, 0, 0, ((lblTeamPathForExternal.Height * 2) + gridExternal.Height) - panelExternal.Height);
            else
                panelExternal.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);


            lblUserDefine.Height = (int)((float)lblUserDefine.Height * FormMain.Instance.WindowHeightRate);
            gridUserDefinedTeam.Location = new Point(0, lblUserDefine.Height);

            if (lblUserDefine.Height + gridUserDefinedTeam.Height > pnlUserDefine.Height)
                pnlUserDefine.Padding = new System.Windows.Forms.Padding(0, 0, 0, ((lblUserDefine.Height * 2) + gridUserDefinedTeam.Height) - pnlUserDefine.Height);
            else
                pnlUserDefine.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);   
      
            gridRegularMember.RefreshGrid();
            gridTemporary.RefreshGrid();
            gridExternal.RefreshGrid();
            gridUserDefinedTeam.RefreshGrid();
        }

        public void UpdateWindowRate(Control ctl, double pWindowRateWidth, double pWindowRateHeight, String pFontFamily = "굴림")
        {
            if (ctl is UnE.GUI.RibbonButton || ctl.GetType().Name == "RibbonButton")
            {
                #region RibbonButton
                ((UnE.GUI.RibbonButton)ctl).CustomImageRect = new Rectangle(0, 0, (int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
                ((UnE.GUI.RibbonButton)ctl).InitButtonWidth = ((UnE.GUI.RibbonButton)ctl).CustomImageRect.Width;
                ((UnE.GUI.RibbonButton)ctl).Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));

                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);

                ((UnE.GUI.RibbonButton)ctl).TextLocation = new Point((int)(((UnE.GUI.RibbonButton)ctl).TextLocation.X * pWindowRateWidth), (int)(((UnE.GUI.RibbonButton)ctl).TextLocation.Y * pWindowRateHeight));
                #endregion
            }
            else if (ctl is Form || ctl.GetType().Name == "Form")
            {
                ctl.Size = new System.Drawing.Size((int)(ctl.Size.Width * pWindowRateWidth), (int)(ctl.Size.Height * pWindowRateHeight));
            }
            else if (ctl is Button || ctl.GetType().Name == "Button")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is Label || ctl.GetType().Name == "Label")
            {
                #region Label
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);

                if (((Label)ctl).AutoSize == false)
                {
                    ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
                }
                #endregion
            }
            else if (ctl is TextBox || ctl.GetType().Name == "TextBox")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is PictureBox || ctl.GetType().Name == "PictureBox")
            {
                ctl.Size = new System.Drawing.Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is GroupBox || ctl.GetType().Name == "GroupBox")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is Panel || ctl.GetType().Name == "Panel")
            {
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is FlowLayoutPanel || ctl.GetType().Name == "FlowLayoutPanel")
            {
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is DataGridView || ctl.GetType().Name == "DataGridView")
            {
                #region DataGridView
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));

                DataGridView dgv = ctl as DataGridView;
                fLabelFontSize = dgv.AlternatingRowsDefaultCellStyle.Font.Size * pWindowRateWidth;
                dgv.AlternatingRowsDefaultCellStyle.Font = new Font(pFontFamily, (float)fLabelFontSize, dgv.Font.Style);

                fLabelFontSize = dgv.DefaultCellStyle.Font.Size * pWindowRateWidth;
                dgv.DefaultCellStyle.Font = new Font(pFontFamily, (float)fLabelFontSize, dgv.Font.Style);

                fLabelFontSize = dgv.RowsDefaultCellStyle.Font.Size * pWindowRateWidth;

                dgv.RowsDefaultCellStyle.Font = new Font(pFontFamily, (float)fLabelFontSize, dgv.Font.Style);

                dgv.ColumnHeadersDefaultCellStyle.Font = new Font(pFontFamily, (float)fLabelFontSize, dgv.ColumnHeadersDefaultCellStyle.Font.Style);

                if (dgv.ColumnCount > 0)
                {
                    for (Int32 index = 0; index < dgv.ColumnCount; index++)
                    {
                        dgv.Columns[index].Width = (int)(dgv.Columns[index].Width * pWindowRateWidth);                        
                    }
                }

                dgv.ColumnHeadersHeight = (int)(dgv.ColumnHeadersHeight * pWindowRateHeight);
                dgv.RowTemplate.Height = (int)(dgv.RowTemplate.Height * pWindowRateHeight);

                #endregion
            }
            else if (ctl is TreeView || ctl.GetType().Name == "TreeView")
            {
                #region TreeView
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ((TreeView)ctl).Indent = (int)((float)((TreeView)ctl).Indent * pWindowRateHeight);
                #endregion
            }
            else if (ctl is RichTextBox || ctl.GetType().Name == "RichTextBox")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
                ctl.Size = new Size((int)(ctl.Width * pWindowRateWidth), (int)(ctl.Height * pWindowRateHeight));
            }
            else if (ctl is ComboBox || ctl.GetType().Name == "ComboBox")
            {
                #region ComboBox
                ComboBox cbo = (ComboBox)ctl;
                float fLabelFontSize = (int)(cbo.Font.Size * pWindowRateWidth);
                cbo.Font = new Font(pFontFamily, fLabelFontSize, ctl.Font.Style);
                cbo.Size = new Size((int)(cbo.Size.Width * pWindowRateWidth), (int)(cbo.Size.Height * pWindowRateHeight));
                #endregion
            }
            else if (ctl is CheckBox || ctl.GetType().Name == "CheckBox")
            {
                float fLabelFontSize = (int)(ctl.Font.Size * pWindowRateWidth);
                ctl.Font = new Font(pFontFamily, fLabelFontSize, ctl.Font.Style);
            }
            else if (ctl is ContextMenuStrip || ctl.GetType().Name == "ContextMenuStrip")
            {
                double fLabelFontSize = ctl.Font.Size * pWindowRateWidth;
                ctl.Font = new Font(pFontFamily, (float)fLabelFontSize, ctl.Font.Style);
            }
            else
            {
                return;
            }

            ctl.Location = new Point((int)(ctl.Location.X * pWindowRateWidth), (int)(ctl.Location.Y * pWindowRateHeight));
        }

        private float UpdateControlFont(Control pctl)
        {
            float fLabelFontSize = pctl.Font.Size * WindowWidthRate;
            pctl.Font = new Font(pctl.Font.FontFamily, fLabelFontSize, FontStyle.Bold);
            return fLabelFontSize;
        }

        private void UpdateGridControl(DataGridView pGrid)
        {
            float fLabelFontSize = pGrid.ColumnHeadersDefaultCellStyle.Font.Size * WindowWidthRate;
            Font mFont = new Font(pGrid.ColumnHeadersDefaultCellStyle.Font.FontFamily, fLabelFontSize);//, FontStyle.Bold);
                        
            DataGridViewCellStyle grid_ColumnHeaderStyle = new DataGridViewCellStyle();
            grid_ColumnHeaderStyle.BackColor = System.Drawing.Color.White;
            grid_ColumnHeaderStyle.Font = mFont;
            grid_ColumnHeaderStyle.ForeColor = System.Drawing.Color.Black;
            
            DataGridViewCellStyle grid_DefaultRowsCellStyle = new DataGridViewCellStyle();
            grid_DefaultRowsCellStyle.BackColor = System.Drawing.Color.White;
            grid_DefaultRowsCellStyle.Font = mFont;
            grid_DefaultRowsCellStyle.ForeColor = System.Drawing.Color.Black;
            grid_DefaultRowsCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(246,169,43);
            grid_DefaultRowsCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            
            DataGridViewCellStyle grid_AlternatingRowsCellStyle = new DataGridViewCellStyle();
            grid_AlternatingRowsCellStyle.BackColor = System.Drawing.Color.FromArgb(224,224,224);
            grid_AlternatingRowsCellStyle.Font = mFont;
            grid_AlternatingRowsCellStyle.ForeColor = System.Drawing.Color.Black;
            grid_AlternatingRowsCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(246, 169, 43);
            grid_AlternatingRowsCellStyle.SelectionForeColor = System.Drawing.Color.Black;            

            pGrid.ColumnHeadersDefaultCellStyle = grid_ColumnHeaderStyle;
            pGrid.RowsDefaultCellStyle = grid_DefaultRowsCellStyle;
            pGrid.AlternatingRowsDefaultCellStyle = grid_AlternatingRowsCellStyle;

            pGrid.ColumnHeadersHeight = (int)((float)pGrid.ColumnHeadersHeight * FormMain.Instance.WindowHeightRate);
            for (Int32 index = 0; index < pGrid.ColumnCount; index++)
            {
                pGrid.Columns[index].Width = (int)((float)pGrid.Columns[index].Width * WindowWidthRate);
            }
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (FormFrame.Instance == null) return;

            if (!m_initSplitDistance && FormFrame.Instance.WindowState == FormWindowState.Maximized)
            {
                splitContainerMain.SplitterDistance = splitContainerMain.Panel1MinSize = m_nSplitDistance;
                m_initSplitDistance = true;
            }

            GetWindowRate();
            GridViewSizeChange();
        }

        private void UpdateGrid()
        {
            try
            {
                if (m_gridType == TeamGrid.GridType.RegularMember)
                    this.gridRegularMember.RefreshGrid();
                else if (m_gridType == TeamGrid.GridType.TemporaryNormal || m_gridType == TeamGrid.GridType.TemporaryEmergency)
                    this.gridTemporary.RefreshGrid();
                else if (m_gridType == TeamGrid.GridType.ExternalCompanyTeam)
                    this.gridExternal.RefreshGrid();
                else if (m_gridType == TeamGrid.GridType.UserDefinedTeam)
                    this.gridUserDefinedTeam.RefreshGrid();
            }
            catch
            {
            }
        }

        void rbtnUndo_Click(object sender, EventArgs e)
        {
            //UpdateGrid();
        }

        void rbtnRedo_Click(object sender, EventArgs e)
        {
            UpdateGrid();
        }

        void textBox1_KeyDown(object sender, KeyEventArgs e)
        { 
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                gridRegularMember.Search(SearchStr);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            gridRegularMember.Search(SearchStr);
        } 

        private void ReadSiteName()
        {
            m_strSiteName = m_dataManager.GetSelectManager().ReadSiteName();
            /* Suhyun
            string strSQL = string.Format("Select TeamName from Site, RegularTeam where Site.ID = {0} and Site.TeamID = RegularTeam.ID", m_nSiteID.ToString());
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;
            */
        }

        private void SetServerConnection()
        {
            return;

            //m_NetWorkClient = new NetworkManager(m_dbMgr, null, FormMain.Instance.SiteID);
        }

        private void SetMergeColumnOfTemporaryGrid()
        {
            gridTemporary.MergeColumns(3, 4);
            gridTemporary.MergeColumns(6, 7);
        }

        private void TeamTreeView_ValidateLabelEdit(object sender, TeamTreeView.ValidateLabelEditEventArgs e)
        {
            if (e.Label.Trim() == "")
            {
                MessageBox.Show("The tree node label cannot be empty",
                    "Label Edit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }
            if (e.Label.IndexOfAny(new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' }) != -1)
            {
                MessageBox.Show("Invalid tree node label.\n" +
                    "The tree node label must not contain following characters:\n \\ / : * ? \" < > |",
                    "Label Edit Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
                return;
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.label2.Text = SiteName + " 조직도";

            splitContainerMain.SplitterDistance = splitContainerMain.Panel1MinSize = m_nSplitDistance;

            DataManager.InitData(m_dbMgr, m_nSiteID);
            SetPositionItems();
            
            treeRegularTeam.LoadData(m_dbMgr, m_nSiteID, TeamTreeView.TeamType.REGULAR); 
            treeExternalCompanyTeam.LoadData(m_dbMgr, m_nSiteID, TeamTreeView.TeamType.EXTERNAL);
            treeNormal.LoadData(m_dbMgr, m_nSiteID, TeamTreeView.TeamType.TEMPORARY_NORMAL);
            treeEmergency.LoadData(m_dbMgr, m_nSiteID, TeamTreeView.TeamType.TEMPORARY_EMERGENCY);
                        
            treeNormal.Dock = DockStyle.Fill;
            treeEmergency.Dock = DockStyle.Fill;
            
            panelRegular.Dock = DockStyle.Fill;
            panelExternal.Dock = DockStyle.Fill;

            gridRegularMember.SetColumnsAlignment(DataGridViewContentAlignment.MiddleCenter);
            gridTemporary.SetColumnsAlignment(DataGridViewContentAlignment.MiddleCenter);
            gridExternal.SetColumnsAlignment(DataGridViewContentAlignment.MiddleCenter);
            gridUserDefinedTeam.SetColumnsAlignment(DataGridViewContentAlignment.MiddleCenter);

            gridRegularMember.MultiSelect = true;
            gridTemporary.MultiSelect = true;
            gridExternal.MultiSelect = true;
            gridUserDefinedTeam.MultiSelect = true;

            gridRegularMember.Type = TeamGrid.GridType.RegularMember;
            // 아직 Normal인지 Emergency인지 결정되지 않았지만, ReadOnly 속성을 위하여 아무것이으로나 설정한다.
            // 정확한 타입은 툴바 메뉴버튼 클릭으로 결정된다.
            gridTemporary.Type = TeamGrid.GridType.TemporaryNormal;
            gridExternal.Type = TeamGrid.GridType.ExternalCompanyTeam;
            gridUserDefinedTeam.Type = TeamGrid.GridType.UserDefinedTeam;

            rbtnRegular_Click(null, null);
            EditMode(rbtnEdit.IsChecked);

            // 정규조직과 비상조직을 함께 화면에 나타내지 않는 옵션일 경우
            if (!m_useSplitContainerEmergency)
            {
                splitContainerEmergency.Panel1.Controls.Remove(treeNormal);
                splitContainerEmergency.Panel1.Controls.Remove(treeEmergency);
                splitContainerEmergency.Panel2.Controls.Remove(panelTemporary);

                splitContainerMain.Panel1.Controls.Add(treeNormal);
                splitContainerMain.Panel1.Controls.Add(treeEmergency);
                splitContainerMain.Panel2.Controls.Add(panelTemporary);

                treeNormal.Visible = treeEmergency.Visible = false;
            }

            if (treeRegularTeam.Nodes.Count > 0)
                treeRegularTeam.SelectedNode = treeRegularTeam.Nodes[0];

            //RememberDefaultControlColor();
            //InitControlColor();
            SetServerConnection();
        }

        public void SetRegularTeamComboItems()
        {
            /*colTeamName.DisplayMember = "TeamName";
            colTeamName.ValueMember = "TeamID";
            
            if (colTeamName.Items.Count > 0)
                colTeamName.Items.Clear();

            foreach (KeyValuePair<int, RegularTeam> item in DataManager.DicRegularTeams)
            { 
                colTeamName.Items.Add(item.Value);
            }*/      
        }

        void gridRegularMember_DataError(object sender, DataGridViewDataErrorEventArgs e) { }

        public void SetExternalTeamComboItems()
        {
            colExternalTeamName.DisplayMember = "TeamName";
            colExternalTeamName.ValueMember = "TeamID";

            if (colExternalTeamName.Items.Count > 0)
                colExternalTeamName.Items.Clear();

            foreach (KeyValuePair<int, ExternalTeam> item in DataManager.DicExternalTeams)
            {
                colExternalTeamName.Items.Add(item.Value);
            }  

            //TreeNodeCollection nodes = treeExternalCompanyTeam.Nodes;
            //foreach (TreeNode item in nodes)
            //{
            //    ExternalTeam rt = item.Tag as ExternalTeam;
            //    if (rt != null)
            //        colExternalTeamName.Items.Add(rt);

            //    SetTeamItems(item, colExternalTeamName);
            //}  
        }
     
        private void SetTeamItems(TreeNode treeNode, DataGridViewComboBoxColumn col)
        {
            foreach (TreeNode tn in treeNode.Nodes)
            {
                RegularTeam rt = tn.Tag as RegularTeam;
                if (rt != null)
                    col.Items.Add(rt);

                SetTeamItems(tn, col);
            }
        } 

        private void SetPositionItems()
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

            /*init = false;

            for (int i = -1; ; i--)
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
            }*/

        }

        private void UpdateGridSize(TeamGrid pGrid, Panel pParentPanel = null)
        {
            Panel pPanel = null;
            if (pParentPanel == null)
                pPanel = (Panel)pGrid.Parent;
            else
                pPanel = pParentPanel;

            Int32 iGridWidth = GetGridWidth(pGrid);

            if (pPanel.Width > iGridWidth)
                pGrid.Width = iGridWidth + 2;
            else
                pGrid.Width = pPanel.Width;
        }

        public void GridViewSizeChange()
        {
            if (m_gridType == TeamGrid.GridType.RegularMember)
            {
                UpdateGridSize(this.gridRegularMember, panelRegular);  
                this.pnlTitleRegular.Width = gridRegularMember.Width;  
            }
            else if (m_gridType == TeamGrid.GridType.TemporaryNormal || m_gridType == TeamGrid.GridType.TemporaryEmergency)
            {
                UpdateGridSize(this.gridTemporary, panelTemporary);
                lblTeamPathForTemporary.Width = gridTemporary.Width;
                UpdateTempraryBandLocation();
            }
            else if (m_gridType == TeamGrid.GridType.ExternalCompanyTeam)
            {
                UpdateGridSize(this.gridExternal, panelExternal);
                lblTeamPathForExternal.Width = gridExternal.Width;
            }
            else if (m_gridType == TeamGrid.GridType.UserDefinedTeam)
            {
                UpdateGridSize(this.gridUserDefinedTeam, panelMain); 
                lblUserDefine.Width = this.gridUserDefinedTeam.Width;
            }
        }

        private void SetToVisibilityForControl(TeamGrid.GridType gridType)
        {
            // 전체 컨트롤에 대해서 비활성화... Visible = false;
            treeRegularTeam.Visible =
                treeNormal.Visible =
                treeEmergency.Visible =
                treeExternalCompanyTeam.Visible = false;

            rbtnImportRegular.IsChecked = 
            rbtnUserDefined.IsChecked =
            rbtnRegular.IsChecked =
            rbtnExternal.IsChecked =
            rbtnNormal.IsChecked =
            rbtnEmergency.IsChecked = false;

            rbtnImportRegular.Refresh();
            rbtnUserDefined.Refresh();
            rbtnRegular.Refresh();
            rbtnNormal.Refresh();
            rbtnEmergency.Refresh();
            rbtnExternal.Refresh();

            pnlUserDefine.Visible = 
            m_pageOption.Visible =
            splitContainerMain.Visible =
            gridUserDefinedTeam.Visible =
            panelRegular.Visible =
            panelExternal.Visible =
            panelTemporary.Visible =
            splitContainerEmergency.Visible = false;

            rbtnImportRegular.Enabled = false;

            lblUserDefine.Visible = false;
            m_gridType = gridType;

            // 각 타입에 따른 활성화 컨트롤 지정
            switch (gridType)
            {
                case TeamGrid.GridType.RegularMember:

                    m_cmdMgr.ChangeCommandTarget(false);

                    // 파일불러오기버튼 비활성
                    rbtnImportRegular.Enabled = IsEditMode;

                    splitContainerMain.Visible =
                    treeRegularTeam.Visible =
                    panelRegular.Visible =
                    rbtnRegular.IsChecked = true;

                    if (treeRegularTeam.SelectedNode == null && treeRegularTeam.Nodes.Count > 0)
                        treeRegularTeam.SelectedNode = treeRegularTeam.Nodes[0];

                    GridViewSizeChange();

                    break;
                case TeamGrid.GridType.TemporaryNormal:

                    m_cmdMgr.ChangeCommandTarget(false);

                    splitContainerMain.Visible = true;
                    treeNormal.Visible = true;
                    rbtnNormal.IsChecked = true;
                    gridTemporary.LinkedTree = treeNormal;
                    gridTemporary.Type = TeamGrid.GridType.TemporaryNormal;
                    panelTemporary.Visible = true;

                    if (m_useSplitContainerEmergency)
                    {
                        splitContainerEmergency.Visible = true;
                    }

                    if (treeNormal.SelectedNode == null)
                    {
                        if (treeNormal.Nodes.Count > 0)
                            treeNormal.SelectedNode = treeNormal.Nodes[0];
                    }
                    else
                    {
                        string strTeamPath = String.Empty;
                        GetTeamPath(treeNormal.SelectedNode, ref strTeamPath);
                        lblTeamPathForTemporary.Text = strTeamPath;

                        gridTemporary.SelectTeam((Team)treeNormal.SelectedNode.Tag, true);
                    }

                    GridViewSizeChange();

                    break;
                case TeamGrid.GridType.TemporaryEmergency:

                    m_cmdMgr.ChangeCommandTarget(false);

                    splitContainerMain.Visible = true;
                    treeEmergency.Visible = true;
                    rbtnEmergency.IsChecked = true;
                    gridTemporary.LinkedTree = treeEmergency;
                    gridTemporary.Type = TeamGrid.GridType.TemporaryEmergency;
                    panelTemporary.Visible = true;

                    if (m_useSplitContainerEmergency)
                    {
                        splitContainerEmergency.Visible = true;
                    }

                    if (treeEmergency.SelectedNode == null)
                    {
                        if (treeEmergency.Nodes.Count > 0)
                            treeEmergency.SelectedNode = treeEmergency.Nodes[0];
                    }
                    else
                    {
                        string strTeamPath = String.Empty;
                        GetTeamPath(treeEmergency.SelectedNode, ref strTeamPath);
                        lblTeamPathForTemporary.Text = strTeamPath;

                        gridTemporary.SelectTeam((Team)treeEmergency.SelectedNode.Tag, true);
                    }


                    GridViewSizeChange();

                    break;
                case TeamGrid.GridType.ExternalCompanyTeam:

                    m_cmdMgr.ChangeCommandTarget(false);

                    splitContainerMain.Visible =
                    treeExternalCompanyTeam.Visible =
                    panelExternal.Visible =
                    rbtnExternal.IsChecked = true;

                    if (treeExternalCompanyTeam.SelectedNode == null && treeExternalCompanyTeam.Nodes.Count > 0)
                    {
                        treeExternalCompanyTeam.SelectedNode = treeExternalCompanyTeam.Nodes[0];
                    }


                    GridViewSizeChange();
            
                    break;
                case TeamGrid.GridType.UserDefinedTeam:

                    m_cmdMgr.ChangeCommandTarget(false);

                    pnlUserDefine.Visible =
                    lblUserDefine.Visible = 
                    gridUserDefinedTeam.Visible =
                    rbtnUserDefined.IsChecked = true;

                    gridUserDefinedTeam.SelectTeam(null, true);

                    GridViewSizeChange();               

                    break;
                default:
                    throw new Exception("타입을 알 수 없는 그리드입니다 확인하여 주세요.");
            }
        }

        public string PrintTeamPath(TreeNode node)
        {
            string strTeamPath = String.Empty;

            GetTeamPath(node, ref strTeamPath);

            return strTeamPath;
        }

        public void SelectRegularTeam(RegularTeam team, bool alwaysDo = false)
        {
            string strTeamPath = PrintTeamPath(treeRegularTeam.SelectedNode);
            lblTeamPathForRegular.Text = strTeamPath;

            gridRegularMember.SelectTeam(team, alwaysDo);
        }

        public void SelectTemporaryTeam(Team team, bool isNormal)
        {
            string strTeamPath = String.Empty;

            if (isNormal == false)
            {
                strTeamPath = PrintTeamPath(treeEmergency.SelectedNode);
            }
            else if (isNormal == true)
            {
                strTeamPath = PrintTeamPath(treeNormal.SelectedNode);
            }

            lblTeamPathForTemporary.Text = strTeamPath;

            gridTemporary.SelectTeam(team);
        }

        public void SelectExternalCompanyTeam(Team team)
        {
            string strTeamPath = PrintTeamPath(treeExternalCompanyTeam.SelectedNode);
            lblTeamPathForExternal.Text = strTeamPath;

            gridExternal.SelectTeam(team);
        }

        private void rbtnRegular_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.RegularMember);
            return;

            treeRegularTeam.Visible = true;
            treeNormal.Visible = treeEmergency.Visible = treeExternalCompanyTeam.Visible = false;

            rbtnRegular.IsChecked = true;
            rbtnExternal.IsChecked = rbtnNormal.IsChecked = rbtnEmergency.IsChecked = false;

            rbtnNormal.Refresh();
            rbtnEmergency.Refresh();
            rbtnExternal.Refresh();

            gridRegularMember.Visible = true;
            gridExternal.Visible = false;
            gridTemporary.Visible = false;
            splitContainerEmergency.Visible = false;

            if (treeRegularTeam.SelectedNode == null)
            {
                if (treeRegularTeam.Nodes.Count > 0)
                    treeRegularTeam.SelectedNode = treeRegularTeam.Nodes[0];
            }
        }

        private void rbtnNormal_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.TemporaryNormal);
            return;

            /*Popup.FormSelectTemporaryTeam frm = new Popup.FormSelectTemporaryTeam(true);

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (frm.SelectedTeam == null)
                    return;

                treeNormal.Nodes.Clear();

            }
            else
                return;*/

            rbtnNormal.IsChecked = true;
            rbtnExternal.IsChecked = rbtnRegular.IsChecked = rbtnEmergency.IsChecked = false;

            rbtnRegular.Refresh();
            rbtnEmergency.Refresh();
            rbtnExternal.Refresh();

            gridRegularMember.Visible = false;
            gridExternal.Visible = false;
            gridTemporary.LinkedTree = treeNormal;
            gridTemporary.Type = TeamGrid.GridType.TemporaryNormal;
            gridTemporary.Visible = true;

            if (m_useSplitContainerEmergency)
            {
                splitContainerEmergency.Visible = true;
                treeNormal.Visible = true;
                treeEmergency.Visible = false;
                treeExternalCompanyTeam.Visible = false;
            }
            else
            {
                treeRegularTeam.Visible = false;
                treeNormal.Visible = true;
                treeEmergency.Visible = false;
                treeExternalCompanyTeam.Visible = false;
            }

            if (treeNormal.SelectedNode == null)
            {
                if (treeNormal.Nodes.Count > 0)
                    treeNormal.SelectedNode = treeNormal.Nodes[0];
            }
            else
                gridTemporary.SelectTeam((Team)treeNormal.SelectedNode.Tag, true);
        }

        private void rbtnEmergency_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.TemporaryEmergency);
            return;

            rbtnEmergency.IsChecked = true;
            rbtnExternal.IsChecked = rbtnRegular.IsChecked = rbtnNormal.IsChecked = false;

            rbtnNormal.Refresh();
            rbtnRegular.Refresh();
            rbtnExternal.Refresh();

            gridRegularMember.Visible = false;
            gridExternal.Visible = false;
            gridTemporary.LinkedTree = treeEmergency;
            gridTemporary.Type = TeamGrid.GridType.TemporaryEmergency;
            gridTemporary.Visible = true;

            if (m_useSplitContainerEmergency)
            {
                splitContainerEmergency.Visible = true;
                treeNormal.Visible = false;
                treeEmergency.Visible = true;
                treeExternalCompanyTeam.Visible = false;
            }
            else
            {
                treeRegularTeam.Visible = false;
                treeNormal.Visible = false;
                treeEmergency.Visible = true;
                treeExternalCompanyTeam.Visible = false;
            }

            if (treeEmergency.SelectedNode == null)
            {
                if (treeEmergency.Nodes.Count > 0)
                    treeEmergency.SelectedNode = treeEmergency.Nodes[0];
            }
            else
                gridTemporary.SelectTeam((Team)treeEmergency.SelectedNode.Tag, true);

        }

        private void rbtnImportRegular_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();

            if (UseExcel())
                openDlg.Filter = "Excel File |*.xls;*.xlsx|Excel CSV File |*.csv|Excel TXT File |*.txt";
            else
                openDlg.Filter = "Excel CSV File |*.csv|Excel TXT File |*.txt";

            if (openDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                RegularMemberReader reader = new RegularMemberReader();
                reader.OpenFile(openDlg.FileName);
            }
            /*FormImportRegularMember pop = new FormImportRegularMember();

            if (pop.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Command.CommandImportRegularMemberInfo cmd = new Command.CommandImportRegularMemberInfo(treeRegularTeam, pop.HeaderPosition, pop.ImportData);
                m_cmdMgr.AddCommand(cmd);
                cmd.ReadImportData();
            }*/
        }

        private bool UseExcel()
        {
            Microsoft.Win32.RegistryKey rkHKCR = Microsoft.Win32.Registry.ClassesRoot;
            Microsoft.Win32.RegistryKey rkExcelKey = rkHKCR.OpenSubKey(@"Excel.Application");
            return rkExcelKey == null ? false : true;
        }

        private void rbtnOption_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.None);
        }

        private void rbtnEdit_Click(object sender, EventArgs e)
        {
            if (rbtnEdit.IsChecked)
            {
                rbtnEdit.IsChecked = false;
                rbtnEdit.Refresh();
            }
            else
                rbtnEdit.IsChecked = true;

            EditMode(rbtnEdit.IsChecked);
            
            treeRegularTeam.AllowDrop = treeNormal.AllowDrop = treeEmergency.AllowDrop = treeExternalCompanyTeam.AllowDrop = rbtnEdit.IsChecked;
        }

        private void rbtnEdit_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_gridType == TeamGrid.GridType.RegularMember)
            {
                if (treeRegularTeam.SelectedNode != null)
                    gridRegularMember.SelectTeam((Team)treeRegularTeam.SelectedNode.Tag, true);
            }
            else if (m_gridType == TeamGrid.GridType.ExternalCompanyTeam)
            {
                if (treeExternalCompanyTeam.SelectedNode != null)
                    gridExternal.SelectTeam((Team)treeExternalCompanyTeam.SelectedNode.Tag, true);
            }
        }

        private void EditMode(bool editable)
        {
            // 파일불러오기버튼 비활성
            rbtnImportRegular.Enabled = (editable ? rbtnRegular.IsChecked : false);

            gridRegularMember.AllowUserToAddRows = gridTemporary.AllowUserToAddRows = gridExternal.AllowUserToAddRows = gridUserDefinedTeam.AllowUserToAddRows = editable;

            gridRegularMember.ReadOnly = gridTemporary.ReadOnly = gridExternal.ReadOnly = gridUserDefinedTeam.ReadOnly = !editable;

            colMemberType.ReadOnly = colManager2.ReadOnly = colTeam.ReadOnly = true;

            if (m_frmTemporaryMember != null)
            {
                if (m_frmTemporaryMember.Visible == true)
                    m_frmTemporaryMember.Close();
            }
        }

        /*private void treeRegularTeam_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (rbtnEdit.IsChecked)
                {
                    if (treeRegularTeam.SelectedNode == e.Node)
                    {
                        treeRegularTeam.LabelEdit = true;
                        e.Node.BeginEdit();
                    }
                }
            }
        }*/

        private void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag == null || e.Label == null)
            {
                if (e.Node != null)
                    e.Node.Text = e.Label;

                e.Node.EndEdit(false);
                return;
            }

            Team team = (Team)e.Node.Tag;

            if (team == null)
            {
                e.Node.EndEdit(false);
                return;
            }

            if (team.TeamName == e.Label)
            {
                e.Node.EndEdit(false);
                return;
            }

            TeamTreeView tree = (TeamTreeView)sender;

            // 빈문자열이거나 중복된 팀 이름은 허용하지 않는다.
            if (e.Label.Length == 0 || IsSameNameNode(e.Node, e.Label, e.Node.Parent == null ? tree.Nodes : e.Node.Parent.Nodes))
            {
                e.CancelEdit = true;
                //e.Node.BeginEdit();
                return;
            }

            ChangedData<string> data = new ChangedData<string>(e.Label, team.TeamName);
            TeamEditor.BLL.WinForms.Command.CommandChangeTeamInfo info = new TeamEditor.BLL.WinForms.Command.CommandChangeTeamInfo(team, data, e.Node, tree.GetTeamType());

            info.Do();
            m_cmdMgr.AddCommand(info);
            e.Node.EndEdit(false);
            tree.LabelEdit = false;

            if (team is RegularTeam)
            {
                lblTeamPathForRegular.Text = PrintTeamPath(e.Node);
            }
            else if (team is TemporaryNormalTeam || team is TemporaryEmergencyTeam)
            {
                lblTeamPathForTemporary.Text = PrintTeamPath(e.Node);
            }
            else if (team is ExternalTeam)
            {
                lblTeamPathForExternal.Text = PrintTeamPath(e.Node);
            }

        }

        // 형제노드들 가운데 중복된 이름이 있는가?
        private bool IsSameNameNode(TreeNode node, string strNodeText, TreeNodeCollection nodes)
        {
            foreach (TreeNode child in nodes)
            {
                if (child == node)
                    continue;

                if (child.Text == strNodeText)
                    return true;
            }

            return false;
        }

        public void OnTreeViewMouseUp(TreeView tree, MouseEventArgs e)
        {
            if (tree == treeRegularTeam)
                OnRegularTreeMouseUp(e);
            else if (tree == treeNormal || tree == treeEmergency)
                OnTemporaryTreeMouseUp(tree, e);
            else if (tree == treeExternalCompanyTeam)
                OnExternalTreeMouseUp(e);
        }

        private void OnExternalTreeMouseUp(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (!IsEditMode)
                {
                    tsMenuNewExternalTeam.Visible =
                    tsMenuAddExternalCompanyTeam.Visible =
                    tsMenuRemoveExternal.Visible =
                    tsMenuRenameExternalCompanyTeam.Visible = false;
                }
                else
                {
                    TreeNode node = treeExternalCompanyTeam.GetNodeAt(e.X, e.Y);

                    tsMenuNewExternalTeam.Visible =
                    tsMenuAddExternalCompanyTeam.Visible =
                    tsMenuRemoveExternal.Visible =
                    tsMenuRenameExternalCompanyTeam.Visible = true;

                    if (node == null)
                    {
                        tsMenuAddExternalCompanyTeam.Enabled = false;
                        tsMenuRemoveExternal.Enabled = false;
                        tsMenuRenameExternalCompanyTeam.Enabled = false;
                    }
                    else
                    {
                        treeExternalCompanyTeam.SelectedNode = node;

                        tsMenuAddExternalCompanyTeam.Enabled = true;
                        tsMenuRemoveExternal.Enabled = true;
                        tsMenuRenameExternalCompanyTeam.Enabled = true;
                    }
                }

                contextMenuExternal.Show(treeExternalCompanyTeam, e.Location);
            }
        }

        private void OnTemporaryTreeMouseUp(TreeView tree, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (!IsEditMode)
                {
                    tsMenuNewGroup.Visible =
                    tsMenuAddTempTeam.Visible =
                    tsMenuDeleteTempTeam.Visible =
                    tsMenuRenameTempTeam.Visible = false;
                }
                else
                {
                    tsMenuNewGroup.Visible =
                    tsMenuAddTempTeam.Visible =
                    tsMenuDeleteTempTeam.Visible =
                    tsMenuRenameTempTeam.Visible = true;

                    TreeNode node = tree.GetNodeAt(e.X, e.Y);

                    if (node == null)
                    {
                        tsMenuAddTempTeam.Enabled = false;
                        tsMenuDeleteTempTeam.Enabled = false;
                        tsMenuRenameTempTeam.Enabled = false;
                    }
                    else
                    {
                        tree.SelectedNode = node;

                        tsMenuAddTempTeam.Enabled = true;
                        tsMenuDeleteTempTeam.Enabled = true;
                        tsMenuRenameTempTeam.Enabled = true;
                    }
                }

                contextMenuTemporaryTeam.Show(tree, e.Location);
            }
        }

        private void OnRegularTreeMouseUp(MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (!IsEditMode)
                {
                    tsMenuAddTeam.Visible =
                    tsMenuDeleteTeam.Visible =
                    tsMenuRenameTeam.Visible = false;
                }
                else
                {
                    TreeNode node = treeRegularTeam.GetNodeAt(e.X, e.Y);

                    if (node != null)
                    {
                        treeRegularTeam.SelectedNode = node;

                        tsMenuAddTeam.Visible =
                        tsMenuDeleteTeam.Visible =
                        tsMenuRenameTeam.Visible = true;
                    }
                    else
                    {
                        tsMenuAddTeam.Visible =
                        tsMenuDeleteTeam.Visible =
                        tsMenuRenameTeam.Visible = false;
                    }
                }

                contextMenuRegularTeam.Show(treeRegularTeam, e.Location);
            }
        }

        private void tsMenuAddTeam_Click(object sender, EventArgs e)
        {
            if (treeRegularTeam.SelectedNode == null)
                return;

            string strTeamName = "이름없는 팀";
            SetNewTeamName(ref strTeamName, treeRegularTeam.SelectedNode.Nodes);

            int nNodeCount = treeRegularTeam.SelectedNode.Nodes.Count;
            TreeNode node = treeRegularTeam.SelectedNode.Nodes.Insert(nNodeCount, strTeamName);

            if (node != null)
            {
                treeRegularTeam.SelectedNode.ExpandAll();
                treeRegularTeam.SelectedNode = node;
                treeRegularTeam.StartLabelEdit();

                TeamEditor.BLL.WinForms.Command.CommandAddRegularTeam cmd = new TeamEditor.BLL.WinForms.Command.CommandAddRegularTeam(m_dataManager, treeRegularTeam, node, null);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);

                //colTeamName.Items.Add(cmd.Team);
            }
        }

        private void tsMenuDeleteTeam_Click(object sender, EventArgs e)
        {
            DeleteRegularTeam();
        }

        private void tsMenuRenameTeam_Click(object sender, EventArgs e)
        {
            if (treeRegularTeam.SelectedNode == null)
                return;

            treeRegularTeam.StartLabelEdit();
        }

        private void treeRegularTeam_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsEditMode)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                DeleteRegularTeam();
            }
        }

        private void DeleteRegularTeam()
        {
            if (treeRegularTeam.SelectedNode == null || treeRegularTeam.SelectedNode.Tag == null)
                return;

            TreeNode node = treeRegularTeam.SelectedNode;

            RegularTeam team = (RegularTeam)node.Tag;
            //int nTeamID = (int)node.Tag;
            //RegularTeam team = DataManager.GetRegularTeam(nTeamID);

            if (team == null)
                return;

            if (treeRegularTeam.Nodes.Contains(treeRegularTeam.SelectedNode))
            {                
                UnE.Utility.UMessageBoxRibbon.Show("최상위 팀은 삭제할 수 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string strMsg = "[" + team.TeamName + "]을 삭제하시겠습니까?\r\n해당팀을 포함한 하위팀과 그 팀에 소속된 직원 정보가 모두 삭제됩니다.\r\n계속 진행할까요?";

            DialogResult _result = UnE.Utility.UMessageBoxRibbon.Show(strMsg, "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            //if(MessageBox.Show(treeRegularTeam, strMsg, "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            if(_result == System.Windows.Forms.DialogResult.Yes)
            {
                TeamEditor.BLL.WinForms.Command.CommandRemoveRegularTeam cmd = new TeamEditor.BLL.WinForms.Command.CommandRemoveRegularTeam();
                cmd.Team = team;
                cmd.TreeNode = node;

                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        public void OnDropNode(TeamTreeView tree, TreeNode nodeSrcParent, TreeNode nodeSrc, TreeNode nodeTrg)
        {
            if (tree == treeRegularTeam)
            {
                TeamEditor.BLL.WinForms.Command.CommandMoveRegularTeam cmd = new TeamEditor.BLL.WinForms.Command.CommandMoveRegularTeam(tree, nodeSrcParent, nodeSrc, nodeTrg);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }            
        }

        public void OnDropRegularMembers(TeamEditor.BLL.WinForms.Command.CommandMoveRegularMembers cmd, TreeNode dropNode)
        {
            if (dropNode == null || dropNode.Tag == null)
                return;

            RegularTeam team = (RegularTeam)dropNode.Tag;
            //int nTeamID = (int)dropNode.Tag;
            //RegularTeam team = DataManager.GetRegularTeam(nTeamID);

            if (team == null)
                return;

            if (team == cmd.TeamOrigin)
                return;

            cmd.TeamMoved = team;            
            cmd.Do();
            m_cmdMgr.AddCommand(cmd);

            treeRegularTeam.SelectedNode = dropNode;
            gridRegularMember.SelectTeam(team, true);
        }

        public void OnDropTemporaryMembers(TeamEditor.BLL.WinForms.Command.CommandMoveTemporaryMembers cmd, TreeNode dropNode)
        {
            if (dropNode == null || dropNode.Tag == null)
                return;

            Team team = (Team)dropNode.Tag;

            if (team == null)
                return;

            if (team == cmd.TeamOrigin)
                return;

            cmd.TeamMoved = team;
            cmd.Do();
            m_cmdMgr.AddCommand(cmd);

            gridTemporary.Refresh();
        }

        public void AddCommand(TeamEditor.BLL.WinForms.Command.CommandEx cmd, bool executeCommand = true)
        {
            if (executeCommand)
                cmd.Do();

            m_cmdMgr.AddCommand(cmd);
        }

        public void SetCurrentRegularTeam(RegularTeam team)
        {
            gridRegularMember.CurrentTeam = team;            
        }

        public void SetCurrentTemporaryTeam(Team team)
        {
            gridTemporary.CurrentTeam = team;
        }

        public void SetCurrentExternalTeam(Team team)
        {
            gridExternal.CurrentTeam = team;
        }

        private void SetNewTeamName(ref string strTeamName, TreeNodeCollection nodes)
        {
            int nMax = -1;

            foreach (TreeNode node in nodes)
            {
                if (node.Text.StartsWith(strTeamName))
                {
                    int n = -1;
                    string str = node.Text.Substring(strTeamName.Length);

                    if (str.Length > 0 && int.TryParse(str, out n))
                    {
                        if (nMax < n)
                            nMax = n;
                    }
                    else if (nMax < 0)
                    {
                        nMax = 0;
                    }

                }
            }

            if (nMax > -1)
            {
                strTeamName = String.Format("{0}{1}", strTeamName, nMax + 1);
            }

        }

        private void tsMenuNewGroup_Click(object sender, EventArgs e)
        {
            TeamTreeView tree = (TeamTreeView)contextMenuTemporaryTeam.SourceControl;
            bool isNormal = tree == treeNormal;

            string strNewGroupName = "신규조직";
            SetNewTeamName(ref strNewGroupName, tree.Nodes);

            TreeNode newNode = tree.Nodes.Add(strNewGroupName);

            if (newNode != null)
            {
                tree.SelectedNode = newNode;
                tree.ExpandAll();
                tree.StartLabelEdit();

                TeamEditor.BLL.WinForms.Command.CommandAddTemporaryTeam cmd = new TeamEditor.BLL.WinForms.Command.CommandAddTemporaryTeam(tree, newNode, isNormal);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        private void tsMenuAddTempTeam_Click(object sender, EventArgs e)
        {
            TeamTreeView tree = (TeamTreeView)contextMenuTemporaryTeam.SourceControl;
            bool isNormal = tree == treeNormal;

            if (tree.SelectedNode == null)
                return;

            string strTeamName = "이름없는 팀";
            SetNewTeamName(ref strTeamName, tree.SelectedNode.Nodes);

            int nNodeCount = tree.SelectedNode.Nodes.Count;
            TreeNode node = tree.SelectedNode.Nodes.Insert(nNodeCount, strTeamName);

            if (node != null)
            {
                tree.SelectedNode.ExpandAll();
                tree.SelectedNode = node;
                tree.StartLabelEdit();

                TeamEditor.BLL.WinForms.Command.CommandAddTemporaryTeam cmd = new TeamEditor.BLL.WinForms.Command.CommandAddTemporaryTeam(tree, node, isNormal);
                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        private void tsMenuDeleteTempTeam_Click(object sender, EventArgs e)
        {
            TeamTreeView tree = (TeamTreeView)contextMenuTemporaryTeam.SourceControl;

            DeleteTemporaryTeam(tree);
        }

        private void tsMenuRenameTempTeam_Click(object sender, EventArgs e)
        {
            TeamTreeView tree = (TeamTreeView)contextMenuTemporaryTeam.SourceControl;

            if (tree.SelectedNode == null)
                return;

            tree.StartLabelEdit();
        }

        private void DeleteTemporaryTeam(TeamTreeView tree)
        {
            if (tree.SelectedNode == null || tree.SelectedNode.Tag == null)
                return;

            TreeNode node = tree.SelectedNode;

            Team team = (Team)node.Tag;

            if (team == null)
                return;

            string strMsg = "[" + team.TeamName + "]을 삭제하시겠습니까?\r\n해당팀을 포함한 하위팀과 그 팀에 연관된 정보가 모두 삭제됩니다.\r\n계속 진행할까요?";
            
            DialogResult _result = UnE.Utility.UMessageBoxRibbon.Show(strMsg, "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            //if (MessageBox.Show(tree, strMsg, "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            if (_result == System.Windows.Forms.DialogResult.Yes)
            {
                TeamEditor.BLL.WinForms.Command.CommandRemoveTemporaryTeam cmd = new TeamEditor.BLL.WinForms.Command.CommandRemoveTemporaryTeam(tree, node, team, tree == treeNormal);

                cmd.Do();
                m_cmdMgr.AddCommand(cmd);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_closeApplication = true;

            //if (m_NetWorkClient != null)
            //{
            //    m_NetWorkClient.ReleaseThread();
            //}

            if (m_frmTemporaryMember != null)
                m_frmTemporaryMember.Close();
        }

        public void ShowTemporaryMemberForm()
        {
            double[] dWinRate = FormMain.Instance.GetCurWindowRate();
            m_frmTemporaryMember = new Popup.FormSelectTemporaryMember(treeRegularTeam, treeNormal, treeEmergency, treeExternalCompanyTeam);
            m_frmTemporaryMember.WindowRateWidth = dWinRate[0];
            m_frmTemporaryMember.WindowRateHeight = dWinRate[1];
            m_frmTemporaryMember.Init(gridRegularMember, gridExternal, gridUserDefinedTeam);
            m_frmTemporaryMember.UpdateControl();

            m_frmTemporaryMemberframe = new UnE.GUI.DialogFormFrameRibbon(m_frmTemporaryMember);
            m_frmTemporaryMemberframe.TitleTextFont = new System.Drawing.Font(Program.prgFont, 12f, FontStyle.Bold);
            m_frmTemporaryMemberframe.TitleTextColor = System.Drawing.Color.Black;
            m_frmTemporaryMemberframe.TitleBarBackColor = Color.FromArgb(246, 169, 43);
            m_frmTemporaryMemberframe.ShowMaxButton = false;
            m_frmTemporaryMemberframe.ShowMinButton = false;
            m_frmTemporaryMemberframe.Sizable = false;

            UnE.GUI.DialogFormFrameRibbon.WindowRateWidth = dWinRate[0];
            UnE.GUI.DialogFormFrameRibbon.WindowRateHeight = dWinRate[1];
            m_frmTemporaryMemberframe.UpdateControlSize();
            m_frmTemporaryMemberframe.ShowDialog(this);
        }

        public string GetLevelName(int nLevelID)
        {
            if (nLevelID < 0 || nLevelID >= colLevel.Items.Count)
                return "";

            return colLevel.Items[nLevelID].ToString();
        }

        public void SetTemporaryMember(object selectedTeam, object selectedMember)
        {
            if (gridTemporary.Visible == false)
                return;

            TeamEditor.BLL.WinForms.Command.CommandChangeTemporaryMemberInfo cmd = gridTemporary.GetTemporaryMemberChangingCommand(selectedTeam, selectedMember, TeamEditor.BLL.WinForms.Command.CommandChangeTemporaryMemberInfo.InfoType.Member);

            if (cmd != null)
            {
                m_cmdMgr.AddCommand(cmd);
                // 편집 도중에 Grid가 정렬되는 것을 막기 위하여 NoSort 속성을 true로 준다.
                gridTemporary.NoSort = true;
                cmd.Do();
                gridTemporary.NoSort = false;

                // 마지막으로 편집한 멤버의 다음 순서에 해당되는 Row를 선택
                foreach(DataGridViewRow row in  gridTemporary.Rows)
                {
                    if(row.Tag != null)
                    {
                        if (object.Equals(row.Tag, cmd.Member))
                        {
                            gridTemporary.ClearSelection();
                            gridTemporary.Rows[row.Index + 1].Selected = true;
                            break;
                        }
                    }
                }
                
            }
        }

        private void rbtnExternal_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.ExternalCompanyTeam);
            return;

            treeExternalCompanyTeam.Visible = true;
            treeNormal.Visible = treeEmergency.Visible = treeRegularTeam.Visible = false;

            rbtnExternal.IsChecked = true;
            rbtnRegular.IsChecked = rbtnNormal.IsChecked = rbtnEmergency.IsChecked = false;

            rbtnNormal.Refresh();
            rbtnEmergency.Refresh();
            rbtnRegular.Refresh();

            gridExternal.Visible = true;
            gridRegularMember.Visible = false;
            gridTemporary.Visible = false;
            splitContainerEmergency.Visible = false;

            if (treeExternalCompanyTeam.SelectedNode == null)
            {
                if (treeExternalCompanyTeam.Nodes.Count > 0)
                    treeExternalCompanyTeam.SelectedNode = treeExternalCompanyTeam.Nodes[0];
            }
        }

        private void tsMenuNewExternalTeam_Click(object sender, EventArgs e)
        {
            //string strNewCompanyName = "신규 협력업체";
            //SetNewTeamName(ref strNewCompanyName, treeExternalCompanyTeam.Nodes);

            //TreeNode newNode = treeExternalCompanyTeam.Nodes.Add(strNewCompanyName);

            //if (newNode != null)
            //{
            //    treeExternalCompanyTeam.SelectedNode = newNode;
            //    treeExternalCompanyTeam.ExpandAll();
            //    treeExternalCompanyTeam.StartLabelEdit();

            //    TeamEditor.BLL.WinForms.Command.CommandAddExternalTeam cmd = new Command.CommandAddExternalTeam(newNode);
            //    cmd.Do();
            //    m_cmdMgr.AddCommand(cmd);

            //    colExternalTeamName.Items.Add(cmd.Team);
            //}
        }

        private void tsMenuAddExternalCompanyTeam_Click(object sender, EventArgs e)
        {
            //if (treeExternalCompanyTeam.SelectedNode == null)
            //    return;

            //string strTeamName = "이름없는 팀";
            //SetNewTeamName(ref strTeamName, treeExternalCompanyTeam.SelectedNode.Nodes);

            //int nNodeCount = treeExternalCompanyTeam.SelectedNode.Nodes.Count;
            //TreeNode node = treeExternalCompanyTeam.SelectedNode.Nodes.Insert(nNodeCount, strTeamName);

            //if (node != null)
            //{
            //    treeExternalCompanyTeam.SelectedNode.ExpandAll();
            //    treeExternalCompanyTeam.SelectedNode = node;
            //    treeExternalCompanyTeam.StartLabelEdit();

            //    Command.CommandAddExternalTeam cmd = new Command.CommandAddExternalTeam(node);
            //    cmd.Do();
            //    m_cmdMgr.AddCommand(cmd);

            //    colExternalTeamName.Items.Add(cmd.Team);
            //}
        }

        private void tsMenuRenameExternalCompanyTeam_Click(object sender, EventArgs e)
        {
            if (treeExternalCompanyTeam.SelectedNode == null)
                return;

            treeExternalCompanyTeam.StartLabelEdit();
        }

        private void tsMenuRemoveExternalCompanyTeam_Click(object sender, EventArgs e)
        {
            DeleteExternalCompanyTeam();
        }

        private void treeExternalCompanyTeam_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsEditMode)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                DeleteExternalCompanyTeam();
            }
        }

        private void DeleteExternalCompanyTeam()
        {
            //if (treeExternalCompanyTeam.SelectedNode == null || treeExternalCompanyTeam.SelectedNode.Tag == null)
            //    return;

            //TreeNode node = treeExternalCompanyTeam.SelectedNode;

            //Team team = (Team)node.Tag;

            //if (team == null)
            //    return;

            //string strMsg = "[" + team.TeamName + "]을 삭제하시겠습니까?\r\n해당팀을 포함한 하위팀과 그 팀에 소속된 직원 정보가 모두 삭제됩니다.\r\n계속 진행할까요?";

            //DialogResult _result = UnE.Utility.UMessageBoxRibbon.Show(strMsg, "경고", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            ////if (MessageBox.Show(treeExternalCompanyTeam, strMsg, "경고", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            //if (_result == System.Windows.Forms.DialogResult.Yes)
            //{
            //    TeamEditor.BLL.WinForms.Command.CommandRemoveExternalTeam cmd = new Command.CommandRemoveExternalTeam(node, team);

            //    cmd.Do();
            //    m_cmdMgr.AddCommand(cmd);
            //}
        }

        private void rbtnUserDefined_Click(object sender, EventArgs e)
        {
            SetToVisibilityForControl(TeamGrid.GridType.UserDefinedTeam);
        }

        private void gridTemporary_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            SetBandsPosition();
        }

        private void gridTemporary_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            SetBandsPosition();
        }

        private void SetBandsPosition()
        {
            //panel1.Visible =
            //panel2.Visible =
            //panel3.Visible = false;

            panelBand1.BringToFront();
            panelBand2.BringToFront();

            panelBand1.BackColor = gridTemporary.GridColor;
            panelBand2.BackColor = gridTemporary.GridColor;
            label1.BackColor = gridTemporary.Columns[0].HeaderCell.InheritedStyle.BackColor;
            label2.BackColor = gridTemporary.Columns[0].HeaderCell.InheritedStyle.BackColor;

            label1.Location = new Point(1, 1);
            label2.Location = new Point(1, 1);

            int nX = -gridTemporary.HorizentalScrollValue;
            int nY = 0;
            int nColumnBorderWidth = 1;

            panelBand1.Location = new Point(nX, nY);
            panelBand1.Width = gridTemporary.Columns[0].Width
                + gridTemporary.Columns[1].Width
                + gridTemporary.Columns[2].Width
                + (nColumnBorderWidth * 1);

            nX += panelBand1.Width- 1 ;

            panelBand2.Location = new Point(nX, nY);
            panelBand2.Width = gridTemporary.Columns[3].Width
                + gridTemporary.Columns[4].Width
                + gridTemporary.Columns[5].Width
                + gridTemporary.Columns[6].Width
                + gridTemporary.Columns[7].Width
                + gridTemporary.Columns[8].Width
                + gridTemporary.Columns[9].Width
                + (nColumnBorderWidth * 1)
                + ((gridTemporary.VerticalScroll.Visible == true) ? gridTemporary.VerticalScroll.Width : 0);

            label1.Width = panelBand1.Width - 2;
            label1.Height = panelBand1.Height - 2;

            label2.Width = panelBand2.Width - 2;
            label2.Height = panelBand2.Height - 2;

            pnlTemporaryBand.Width = panelBand1.Width + panelBand2.Width;
        }

        private void GetTeamPath(TreeNode node, ref string strTeamPath)
        {
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

        public void SetServerConnection(string strIP, bool isConnected)
        {
            string strMsg = "";

            if (!isConnected)
                strMsg = String.Format("SOP Server( {0} )와의 접속 시도중...", strIP);
            else
                strMsg = String.Format("SOP Server( {0} )와의 접속 성공", strIP);

            //this.Invoke((MethodInvoker)delegate
            //{
            //    lblRegularServerState.Text =
            //    lblExternalServerState.Text =
            //    lblTemporaryServerState.Text = strMsg;
            //});
        }

        private void RememberDefaultControlColor()
        {
            m_pageOption.SetDefaultColor(treeRegularTeam.BackColor, treeRegularTeam.ForeColor, gridRegularMember.DefaultCellStyle.BackColor, gridRegularMember.ForeColor);
        }

        public void InitControlColor()
        {
            if (m_pageOption.HasColorInfo() == false)
                return;

            treeRegularTeam.BackColor =
            treeNormal.BackColor =
            treeEmergency.BackColor =
            treeExternalCompanyTeam.BackColor = m_pageOption.ColorTreeBack;

            treeRegularTeam.ForeColor =
            treeNormal.ForeColor =
            treeEmergency.ForeColor =
            treeExternalCompanyTeam.ForeColor = m_pageOption.ColorTreeFont;

            treeRegularTeam.Refresh();
            treeNormal.Refresh();
            treeEmergency.Refresh();
            treeExternalCompanyTeam.Refresh();


            panelRegular.BackColor =
            panelTemporary.BackColor =
            panelExternal.BackColor =
            gridRegularMember.BackgroundColor =
            gridTemporary.BackgroundColor =
            gridExternal.BackgroundColor =
            gridUserDefinedTeam.BackgroundColor = m_pageOption.ColorGridBack;

            //gridRegularMember.GridColor=
            //gridTemporary.GridColor =
            //gridExternal.GridColor =
            //gridUserDefinedTeam.GridColor = m_pageOption.ColorGridBack;

            foreach (DataGridViewColumn column in gridTemporary.Columns)
            {
                column.HeaderCell.Style.BackColor = m_pageOption.ColorGridBack;
            }

            gridRegularMember.DefaultCellStyle.BackColor =
            gridTemporary.DefaultCellStyle.BackColor =
            gridExternal.DefaultCellStyle.BackColor =
            gridUserDefinedTeam.DefaultCellStyle.BackColor = m_pageOption.ColorGridBack;


            gridRegularMember.ForeColor =
            gridTemporary.ForeColor =
            gridExternal.ForeColor =
            gridUserDefinedTeam.ForeColor = m_pageOption.ColorGridFont;


            gridRegularMember.Refresh();
            gridTemporary.Refresh();
            gridExternal.Refresh();
            gridUserDefinedTeam.Refresh();
        }

        public void RefreshRegularMemberGrid()
        {
            UpdateGrid();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            splitContainerMain.SplitterDistance = m_nSplitDistance = 210;

            SetToVisibilityForControl(TeamGrid.GridType.RegularMember);
            this.rbtnRegular.Refresh();
        }

        private Int32 GetGridWidth(TeamGrid pGrid)
        {
            Int32 iWidth = 0;                        

            for (Int32 index = 0; index < pGrid.Columns.Count; index++)
            {
                if (pGrid.Columns[index].Visible == true)
                    iWidth += pGrid.Columns[index].Width;
            }

            if (pGrid.VerticalScroll.Visible == true)
            {
                iWidth += pGrid.VerticalScroll.Width;
            }

            return iWidth;
        }

        private void UpdateTempraryBandLocation()
        {
            SetBandsPosition();

            Int32 LocationX = 0;// (panelTemporary.Width / 2) - (GetGridWidth(gridTemporary) / 2);
            if (LocationX >= 0)
                pnlTemporaryBand.Location = new Point(LocationX, pnlTemporaryBand.Location.Y);
            else
                pnlTemporaryBand.Location = new Point(0, pnlTemporaryBand.Location.Y);
        }

        private void grid_ColumnWidthChange(object sender, DataGridViewColumnEventArgs e)
        {
            GridViewSizeChange();
        }

        private void grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            TeamGrid gdv = sender as TeamGrid;
            if (gdv == null) return;

            foreach (DataGridViewRow row in gdv.Rows)
            {
                row.MinimumHeight = gdv.RowHeight; 
            }
        }

        private void grid_ScrollValueChange(object sender, ScrollEventArgs e)
        {
            TeamGrid grd = sender as TeamGrid;
            if (grd == null) return;

            if (grd.Name == "gridTemporary")
            {
                SetBandsPosition();
                grd.Refresh();
            }
        }

        private void splitContainerMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            FormMain_Resize(null, null);
        }
    }
}
