using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows;
using UnE.GUI;
using DBUtility;

namespace SOPManager
{
    public partial class FormMain : Form, ITextPictureBoxOwner, IRibbonButtonOwner
    {
        private static FormMain m_instance = null;
        private int m_Pagenum = ID.ID_FILE_OPEN;
        public int Pagenum
        {
            get { return m_Pagenum; }
        }

        public static SOPManager.FormMain Instance
        {
            get { return m_instance; }
        }
        // Top 영역에서 마우스 클릭 여부
        private bool m_bPanelTopLeftMouseDown = false;
        // 폼 이동을 위한 Point
        private Point m_ptFormMove;
        // Form 최소 사이즈
        protected Size m_nMinSize = new Size(1600, 900);

        // 선택된 탭
        protected int m_nSelectTab = 0;
        public int SelectedTab
        {
            get { return m_nSelectTab; }
            set { m_nSelectTab = value; }
        }

        // 공용으로 사용될 SOP Tree Form
        protected BarLevelTree m_sopTree = new BarLevelTree();
        public SOPManager.BarLevelTree SopTree
        {
            get { return m_sopTree; }
        }

        protected WebDBManager m_dbMgr = null;
        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        private TabPage m_tapPageCopySrc = null;

        private FormNewSOP m_formNewSOP;
        //private PageBackstagePage m_pagePage;
        private FormPageSOP m_pageSOP;
        private PageBackstageHelp m_pageHelp;

        private Dictionary<int, Data_NormalTeam> m_dicNormalTeam = new Dictionary<int, Data_NormalTeam>();
        private PopupSpecialMessage m_frmSpecialMessage = null;

        ArrayList m_arrPath = new ArrayList();

        bool m_isFirst = false;

        private int m_nSOPGenUserID = -1;
        private string m_strSOPGenUserID = "";
        private string m_strSOPGenUserRealName = "";

        private VersionInfo m_versionCurrent = null;

        //////////////////////////////////////////////////////////////////////////
        ArrayList m_arrFullPath = new ArrayList();

        public ArrayList FullPath
        {
            get { return m_arrFullPath; }
            set { m_arrFullPath = value; }
        }

        //////////////////////////////////////////////////////////////////////////
        // DB List
        private ArrayList m_arrDisaster = new ArrayList();
        private ArrayList m_arrSubDisaster = new ArrayList();
        private ArrayList m_arrDetail = new ArrayList();
        private ArrayList m_arrNormalTeam = new ArrayList();
        private ArrayList m_arrEmergencyTeam = new ArrayList();
        private ArrayList m_arrCheckTask = new ArrayList();
        private ArrayList m_arrRegularTeam = new ArrayList();
        private ArrayList m_arrActionStep = new ArrayList();
        private ArrayList m_arrUserDefinedTeam = new ArrayList();
        private ArrayList m_arrExternalTeam = new ArrayList();
        private ArrayList m_arrSOPVersion = new ArrayList();

        public ArrayList DisasterCategory
        {
            get { return m_arrDisaster; }
            set { m_arrDisaster = value; }
        }

        public ArrayList SubDisasterCategory
        {
            get { return m_arrSubDisaster; }
            set { m_arrSubDisaster = value; }
        }

        public ArrayList DetailDisasterCategory
        {
            get { return m_arrDetail; }
            set { m_arrDetail = value; }
        }

        public ArrayList TemporaryNormalTeam
        {
            get { return m_arrNormalTeam; }
            set { m_arrNormalTeam = value; }
        }

        public ArrayList TemporaryEmergencyTeam
        {
            get { return m_arrEmergencyTeam; }
            set { m_arrEmergencyTeam = value; }
        }

        public ArrayList CheckTask
        {
            get { return m_arrCheckTask; }
            set { m_arrCheckTask = value; }
        }

        public ArrayList RegularTeam
        {
            get { return m_arrRegularTeam; }
            set { m_arrRegularTeam = value; }
        }
        public ArrayList ActionStep
        {
            get { return m_arrActionStep; }
            set { m_arrActionStep = value; }
        }
        public ArrayList UserDefinedTeam
        {
            get { return m_arrUserDefinedTeam; }
            set { m_arrUserDefinedTeam = value; }
        }
        public ArrayList ExternalTeam
        {
            get { return m_arrExternalTeam; }
            set { m_arrExternalTeam = value; }
        }
        public ArrayList SOPVersion
        {
            get { return m_arrSOPVersion; }
            set { m_arrSOPVersion = value; }
        }

        public override string Text
        {
            get
            {
                if (lbTitle == null)
                    return "";
                return lbTitle.Text;
            }
            set
            {
                lbTitle.Text = value;
            }
        }

        protected ArrayList m_arRibbonButtons = new ArrayList();

        /// <summary>
        /// UI 생성되기 이전에 필요한 DB Data 로드
        /// Form 생성 이전에 호출
        /// </summary>
        public void LoadBaseData()
        {
            ReadDisasterCategory();
            ReadSubDisasterCategory();
            ReadDisaster();
            ReadTeamporaryNormalTeam();
            ReadTeamporaryEmergencyTeam();
            ReadCheckTask();
            ReadRegularTeam();
            ReadActionStep();
            ReadUserDefinedTeam();
            ReadExternalTeam();
            ReadVersion();
        }

        /// <summary>
        /// UI가 생성된 이후에 사용될 DB Data 로드
        /// Form Load 이벤트에서 호출
        /// </summary>
        public void LoadExtraData()
        {
        }

        public FormMain(int nSOPGenUserID, string strSOPGenUserID, string strSOPGenUserRealName)
        {
            m_instance = this;

            m_nSOPGenUserID = nSOPGenUserID;
            m_strSOPGenUserID = strSOPGenUserID;
            m_strSOPGenUserRealName = strSOPGenUserRealName;

            m_dbMgr = new WebDBManager();

            LoadBaseData();

            this.WindowState = FormWindowState.Maximized;
            InitializeComponent();

            FindNormalPath();

            GetDefaultBoundary();

            CreateBackstageView();

            InitPanel();

            InitTabButton();

            InitLeftToolBar();

            InitTopToolBar();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadExtraData();

            OnShowForm(typeof(FormOpenDB), true);

            m_tmrCmdUpdate.Enabled = true;
            m_tmrCmdUpdate.Start();
        }

        private FormOpenDB form = null;
        protected void InitPanel()
        {
            panelContent.Dock = DockStyle.Fill;
            panelSection.Dock = DockStyle.Fill;

            panelSection.Visible = false;

            form = new FormOpenDB();
            form.Dock = DockStyle.Fill;
            panelForm.Controls.Add(form);

            //FormNewSOP form2 = new FormNewSOP();
            m_formNewSOP.Dock = DockStyle.Fill;
            panelForm.Controls.Add(m_formNewSOP);
        }

        protected void InitTabButton()
        {
            m_nSelectTab = 0;

            pictureBoxFile.Font = new Font("맑은 고딕", 14, FontStyle.Bold);
            pictureBoxFile.ForeColor = Color.White;
            pictureBoxFile.Text = "열기";
            pictureBoxFile.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Pressed;
            pictureBoxFile.SetPictureBoxOwner(this);

            pictureBoxSOP.Font = new Font("맑은 고딕", 14, FontStyle.Bold);
            pictureBoxSOP.Text = "생성";
            pictureBoxSOP.ForeColor = Color.White;
            pictureBoxSOP.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Normal;
            pictureBoxSOP.SetPictureBoxOwner(this);
        }

        protected void InitTopToolBar()
        {
            ribbonButton1.Owner = this;
            ribbonButton1.Font = new Font("맑은 고딕", 9, FontStyle.Bold);
            ribbonButton1.ForeColor = Color.White;
            ribbonButton1.Text = "되돌리기";
            ribbonButton1.TextPos = RibbonButton.TextPosition.BOTTOM;
            ribbonButton1.Enabled = true;
            ribbonButton1.ID = ID.ID_EDIT_UNDO;


            ribbonButton2.Owner = this;
            ribbonButton2.Font = new Font("맑은 고딕", 9, FontStyle.Bold);
            ribbonButton2.ForeColor = Color.White;
            ribbonButton2.Text = "다시실행";
            ribbonButton2.TextPos = RibbonButton.TextPosition.BOTTOM;
            ribbonButton2.ID = ID.ID_EDIT_REDO;

            ribbonButton3.Owner = this;
            ribbonButton3.Font = new Font("맑은 고딕", 9, FontStyle.Bold);
            ribbonButton3.ForeColor = Color.White;
            ribbonButton3.Text = "단계 붙여넣기";
            ribbonButton3.TextPos = RibbonButton.TextPosition.BOTTOM;
            ribbonButton3.ID = ID.ID_EDIT_LEVEL_PASTE;

            ribbonButton4.Owner = this;
            ribbonButton4.Font = new Font("맑은 고딕", 9, FontStyle.Bold);
            ribbonButton4.ForeColor = Color.White;
            ribbonButton4.Text = "단계 복사";
            ribbonButton4.TextPos = RibbonButton.TextPosition.BOTTOM;
            ribbonButton4.ID = ID.ID_EDIT_LEVEL_COPY;

            ribbonButton5.Owner = this;
            ribbonButton5.Font = new Font("맑은 고딕", 9, FontStyle.Bold);
            ribbonButton5.ForeColor = Color.White;
            ribbonButton5.Text = "단계 삭제";
            ribbonButton5.TextPos = RibbonButton.TextPosition.BOTTOM;
            ribbonButton5.ID = ID.ID_EDIT_LEVEL_DEL;

            ribbonButton6.Owner = this;
            ribbonButton6.Font = new Font("맑은 고딕", 9, FontStyle.Bold);
            ribbonButton6.ForeColor = Color.White;
            ribbonButton6.Text = "단계 추가";
            ribbonButton6.TextPos = RibbonButton.TextPosition.BOTTOM;
            ribbonButton6.ID = ID.ID_EDIT_LEVEL_ADD;

            ribbonButton7.Owner = this;
            ribbonButton7.Font = new Font("맑은 고딕", 9, FontStyle.Bold);
            ribbonButton7.ForeColor = Color.White;
            ribbonButton7.Text = "패널 추가";
            ribbonButton7.TextPos = RibbonButton.TextPosition.BOTTOM;
            ribbonButton7.ID = ID.ID_PANE_ADD;

            ribbonButton8.Owner = this;
            ribbonButton8.Font = new Font("맑은 고딕", 9, FontStyle.Bold);
            ribbonButton8.ForeColor = Color.White;
            ribbonButton8.Text = "패널 삭제";
            ribbonButton8.TextPos = RibbonButton.TextPosition.BOTTOM;
            ribbonButton8.ID = ID.ID_PANE_DELETE;

            ArrangeRibbonButton(ribbonButton1, ribbonButton2);
            ArrangeRibbonButtonAddGap(ribbonButton2, ribbonButton3, 5);
            ArrangeRibbonButton(ribbonButton3, ribbonButton4);
            ArrangeRibbonButton(ribbonButton4, ribbonButton5);
            ArrangeRibbonButton(ribbonButton5, ribbonButton6);
            ArrangeRibbonButtonAddGap(ribbonButton6, ribbonButton7, 5);
            ArrangeRibbonButton(ribbonButton7, ribbonButton8);

            m_arRibbonButtons.Add(ribbonButton1);
            m_arRibbonButtons.Add(ribbonButton2);
            m_arRibbonButtons.Add(ribbonButton3);
            m_arRibbonButtons.Add(ribbonButton4);
            m_arRibbonButtons.Add(ribbonButton5);
            m_arRibbonButtons.Add(ribbonButton6);
            m_arRibbonButtons.Add(ribbonButton7);
            m_arRibbonButtons.Add(ribbonButton8);
        }

        private void ArrangeRibbonButtonAddGap(RibbonButton btnPrev, RibbonButton btnNext, int gap)
        {
            btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width + gap, btnPrev.Location.Y);
        }

        private void ArrangeRibbonButton(RibbonButton btnPrev, RibbonButton btnNext)
        {
            btnNext.Location = new Point(btnPrev.Location.X + btnPrev.Size.Width, btnPrev.Location.Y);
        }

        protected void InitLeftToolBar()
        {
            btnOpen.Owner = this;
            btnOpen.Font = new Font("맑은 고딕", 16, FontStyle.Bold);
            btnOpen.ForeColor = Color.White;
            btnOpen.Text = "열기";
            btnOpen.TextPos = RibbonButton.TextPosition.RIGHT;
            btnOpen.ID = ID.ID_FILE_OPEN;

            btnOpenXML.Owner = this;
            btnOpenXML.Font = new Font("맑은 고딕", 16, FontStyle.Bold);
            btnOpenXML.ForeColor = Color.White;
            btnOpenXML.Text = "XML 열기";
            btnOpenXML.TextPos = RibbonButton.TextPosition.RIGHT;
            btnOpenXML.ID = ID.ID_XML_OPEN;

            btnSave.Owner = this;
            btnSave.Font = new Font("맑은 고딕", 16, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Text = "저장";
            btnSave.TextPos = RibbonButton.TextPosition.RIGHT;
            btnSave.ID = ID.ID_FILE_SAVE;

            btnSaveXML.Owner = this;
            btnSaveXML.Font = new Font("맑은 고딕", 16, FontStyle.Bold);
            btnSaveXML.ForeColor = Color.White;
            btnSaveXML.Text = "XML 저장";
            btnSaveXML.TextPos = RibbonButton.TextPosition.RIGHT;
            btnSaveXML.ID = ID.ID_XML_SAVE;

            btnNewSOP.Owner = this;
            btnNewSOP.Font = new Font("맑은 고딕", 16, FontStyle.Bold);
            btnNewSOP.ForeColor = Color.White;
            btnNewSOP.Text = "새 SOP";
            btnNewSOP.TextPos = RibbonButton.TextPosition.RIGHT;
            btnNewSOP.ID = ID.ID_FILE_NEWSOP;

            btnDeleteSOP.Owner = this;
            btnDeleteSOP.Font = new Font("맑은 고딕", 16, FontStyle.Bold);
            btnDeleteSOP.ForeColor = Color.White;
            btnDeleteSOP.Text = "삭제";
            btnDeleteSOP.TextPos = RibbonButton.TextPosition.RIGHT;
            btnDeleteSOP.ID = ID.ID_FILE_DELETE;

            m_arRibbonButtons.Add(btnNewSOP);
            m_arRibbonButtons.Add(btnSaveXML);
            m_arRibbonButtons.Add(btnSave);
            m_arRibbonButtons.Add(btnOpenXML);
            m_arRibbonButtons.Add(btnOpen);
            m_arRibbonButtons.Add(btnDeleteSOP);

        }

        #region Window Tool Bar Button 이벤트
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Close();
        }

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                this.btnMax.BackgroundImage = global::SOPManager.Properties.Resources.MaxWindow_Normal;
                this.btnMax.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                WindowState = FormWindowState.Normal;
                this.Size = m_nMinSize;
            }
            else
            {
                this.btnMax.BackgroundImage = global::SOPManager.Properties.Resources.NormalWindow_Normal;
                this.btnMax.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                WindowState = FormWindowState.Maximized;
            }
        }
        #endregion

        #region Top패널 Mouse 이벤트 , Maximized, Minimized, Move

        private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bPanelTopLeftMouseDown = true;
                m_ptFormMove = panelTop.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void panelTop_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bPanelTopLeftMouseDown = false;
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bPanelTopLeftMouseDown == true)
                {
                    Point pt = panelTop.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptFormMove.X;
                    int dy = pt.Y - m_ptFormMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptFormMove.X += dx;
                        m_ptFormMove.Y += dy;
                    }
                }
            }
        }

        private void panelTop_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
                btnMax.BackgroundImage = global::SOPManager.Properties.Resources.NormalWindow_Normal;
            }
            else if (this.WindowState == FormWindowState.Maximized)
            {
                Size sizeCur = this.Size;
                this.WindowState = FormWindowState.Normal;
                btnMax.BackgroundImage = global::SOPManager.Properties.Resources.MaxWindow_Normal;
                Size sizeNormal = this.Size;

                double hRate = (double)sizeNormal.Height / (double)sizeCur.Height;
                this.Size = new Size((int)(sizeCur.Width * hRate), sizeNormal.Height);
            }
        }

        #endregion

        #region Change Tab Mouse 이벤트, Select Tab

        public void SelectTab(int nTab)
        {
            m_nSelectTab = nTab;
            if (m_nSelectTab == 0)
            {
                pictureBoxFile.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Pressed;
                pictureBoxSOP.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Normal;

                panelContent.Visible = true;
                panelSection.Visible = false;

                panelRibbon.Visible = false;

                if (panelGap.Visible == false)
                    panelTop.Size = new Size(panelTop.Size.Width, panelTop.Size.Height + panelGap.Size.Height);
                panelGap.Visible = true;
            }
            else if (m_nSelectTab == 1)
            {
                pictureBoxFile.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Normal;
                pictureBoxSOP.BackgroundImage = global::SOPManager.Properties.Resources.Tab_Pressed;

                panelRibbon.Visible = true;
                panelGap.Visible = false;
                panelTop.Size = new Size(panelTop.Size.Width, panelTop.Size.Height - panelGap.Size.Height);

                panelContent.Visible = false;
                panelSection.Visible = true;



            }
        }

        public void TextPictureBox_MouseDown(TextPictureBox pictureBox, MouseEventArgs e)
        {
            if (e != null)
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Left)
                    return;
            }

            if (pictureBox == pictureBoxFile)
            {
                if (m_nSelectTab == 1)
                {
                    SelectTab(0);
                }
            }
            else if (pictureBox == pictureBoxSOP)
            {
                if (m_nSelectTab == 0)
                {
                    SelectTab(1);
                }
            }
        }
        #endregion


        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_tmrCmdUpdate.Stop();
            m_tmrCmdUpdate.Enabled = false;
        }

        private void m_tmrCmdUpdate_Tick(object sender, EventArgs e)
        {
            foreach (RibbonButton rb in m_arRibbonButtons)
            {
                OnRibbonButtonUpdate(rb, rb.ID);
            }
        }

        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
            RibbonButton btnRB = (RibbonButton)sender;
            btnRB.Refresh();
        }

        private bool IsShowForm(Type formType)
        {
            foreach (Control control in panelForm.Controls)
            {
                if (control.GetType() == formType)
                {
                    return control.Visible;
                }
            }
            return false;
        }

        private void OnShowForm(Type formType, bool bShow)
        {
            foreach (Control control in panelForm.Controls)
            {
                if (control.GetType() == formType)
                {
                    control.Visible = bShow;
                    break;
                }
            }
        }

        public Form GetForm(Type formType)
        {
            foreach (Control control in panelForm.Controls)
            {
                if (control.GetType() == formType)
                {
                    return (Form)control;
                }
            }
            return null;
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton btnRB = (RibbonButton)sender;

            OnRibbonButtonExecute(sender, btnRB.ID);

            btnRB.Refresh();
        }

        private static ArrayList GetDefaultBoundary()
        {
            ArrayList arrBoundary = new ArrayList();
            float fWidth = 150.0f, fHeight = 80.0f;

            // Bezier Curve를 그리기 위한 기준점 설정
            UnE.Geometry.Vertex2D[] arrCurvePoints = new UnE.Geometry.Vertex2D[4];

            arrCurvePoints[0] = new UnE.Geometry.Vertex2D(0, 0);
            arrCurvePoints[1] = new UnE.Geometry.Vertex2D(fWidth / 3, fWidth * 0.2);
            arrCurvePoints[2] = new UnE.Geometry.Vertex2D(fWidth * 2 / 3, -fWidth * 0.2);
            arrCurvePoints[3] = new UnE.Geometry.Vertex2D(fWidth, 0);
            ////////////////////////////////////////////////////////////////

            // Bezier Curve 얻어오기
            int nResultCount = 100;
            UnE.Geometry.Vertex2D[] arrResultPoints = new UnE.Geometry.Vertex2D[nResultCount];

            UnE.Geometry.BezierCurve2D bezier = new UnE.Geometry.BezierCurve2D();

            if (!bezier.Calc(arrCurvePoints, arrCurvePoints.Count(), arrResultPoints, nResultCount))
                return arrBoundary;
            ////////////////////////////////////////////////////////////////

            // Boundary Vertex 설정
            for (int i = 0; i < nResultCount; i++)
            {
                UnE.Geometry.Vertex2D vertex = arrResultPoints[i];
                arrBoundary.Add(new PointF((float)vertex.x, (float)vertex.y));
            }

            for (int i = nResultCount - 1; i >= 0; i--)
            {
                UnE.Geometry.Vertex2D vertex = arrResultPoints[i];
                arrBoundary.Add(new PointF((float)vertex.x, (float)vertex.y - fHeight));
            }
            ////////////////////////////////////////////////////////////////

            return arrBoundary;
        }

        private void ReadDB()
        {
            ReadDisasterCategory();
            ReadSubDisasterCategory();
            ReadDisaster();
            ReadTeamporaryNormalTeam();
            ReadTeamporaryEmergencyTeam();
            ReadCheckTask();
            ReadRegularTeam();
            ReadActionStep();
            ReadUserDefinedTeam();
            ReadExternalTeam();
            ReadVersion();
        }

        public static Bitmap GetImageByName(string imageName)
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("neutral");
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            string resourceName = "SOPManager.Properties.Resources";
            var rm = new System.Resources.ResourceManager(resourceName, asm);
            return (Bitmap)rm.GetObject(imageName);
        }

        private void CreateBackstageView()
        {
            if (m_formNewSOP == null) m_formNewSOP = new FormNewSOP();
            if (m_pageSOP == null) m_pageSOP = new FormPageSOP();
            if (m_pageHelp == null) m_pageHelp = new PageBackstageHelp();

            EnableControlPage(false);
            EnableControlLevel(false);
        }

        public void CreateSOP()
        {

            string strRegular = "미등록모드";
            string strWeekday = "야간 및 휴일";

            if (m_formNewSOP.IsRegularMode())
                strRegular = "등록모드";
            if (m_formNewSOP.IsWeekMode())
                strWeekday = "평일";

            this.Text = "SOP Manager  V 2.0 - " + strRegular + ", " + strWeekday;

            m_formNewSOP.SelectedTeams();

            NewSOP();
            m_pageSOP.AddTabPage();


            string szLevelName = m_pageSOP.GetTabPageName();
            string szPath = m_formNewSOP.GetLevelName();
            m_pageSOP.GetBarLevelTree().AddTreeNode();
            m_pageSOP.GetPropertiesLevel().AddTitle(szPath);
            m_pageSOP.GetPropertiesLevel().SetSelectedTabName(szLevelName);

            m_pageSOP.AddPane();
            m_pageSOP.PanelResize();

            m_pageSOP.GetBarPage().SetDataGrid();

            OnRibbonButtonMouseDown(btnOpen, null);
            OnRibbonButtonMouseUp(btnOpen, null);
            SelectTab(1);
        }

        private void OnRibbonButtonUpdate(RibbonButton btn, int nID)
        {
            switch (nID)
            {
                case ID.ID_FILE_OPEN:
                    if (IsShowForm(typeof(FormOpenDB)) && m_Pagenum == ID.ID_FILE_OPEN)
                    {
                        btn.IsChecked = true;
                        form.SelectChangePage(m_Pagenum);
                    }
                    else
                        btn.IsChecked = false;
                    break;
                case ID.ID_FILE_NEWSOP:
                    if (IsShowForm(typeof(FormNewSOP)))
                        btn.IsChecked = true;
                    else
                        btn.IsChecked = false;
                    break;
                case ID.ID_FILE_DELETE:
                    if (IsShowForm(typeof(FormOpenDB)) && m_Pagenum == ID.ID_FILE_DELETE)
                    {
                        btn.IsChecked = true;
                        form.SelectChangePage(m_Pagenum);
                    }
                    else
                        btn.IsChecked = false;
                    break;
                case ID.ID_EDIT_LEVEL_DEL:
                    if (m_pageSOP.GetTabPages().Count == 0)
                        btn.Enabled = false;
                    else
                        btn.Enabled = true;
                    break;
                case ID.ID_EDIT_LEVEL_ADD:
                    if (m_pageSOP.GetBarLevelTree().ExistNode())
                        btn.Enabled = true;
                    else
                        btn.Enabled = false;
                    break;
                case ID.ID_EDIT_LEVEL_COPY:
                    if (m_pageSOP.GetTabPages().Count == 0)
                        btn.Enabled = false;
                    else
                        btn.Enabled = true;
                    break;
                case ID.ID_EDIT_LEVEL_PASTE:
                    if (m_tapPageCopySrc == null)
                        btn.Enabled = false;
                    else
                        btn.Enabled = true;
                    break;

                case ID.ID_EDIT_UNDO:
                    if (UndoRedoManager.Instance.UndoCount > 0)
                        btn.Enabled = true;
                    else
                        btn.Enabled = false;
                    break;
                case ID.ID_EDIT_REDO:
                    if (UndoRedoManager.Instance.RedoCount > 0)
                        btn.Enabled = true;
                    else
                        btn.Enabled = false;
                    break;

                case ID.ID_PANE_DELETE:
                    TabPage page = m_pageSOP.GetCurrentTabPage();
                    if (page != null && Sections.PanelSectionEx.GetTabPageTeamList(page).Count > 0)
                    {
                        btn.Enabled = true;
                    }
                    else
                    {
                        btn.Enabled = false;
                    }
                    break;
                case ID.ID_PANE_ADD:
                    TabPage page2 = m_pageSOP.GetCurrentTabPage();
                    if (page2 != null)
                    {
                        btn.Enabled = true;
                    }
                    else
                    {
                        btn.Enabled = false;
                    }
                    break;

            };

            btn.Refresh();
        }

        private void OnRibbonButtonExecute(object sender, int nID)
        {
            RibbonButton btnRB = (RibbonButton)sender;
            switch (nID)
            {
                case ID.ID_FILE_OPEN:
                    OnShowForm(typeof(FormNewSOP), false);
                    OnShowForm(typeof(FormOpenDB), true);
                    m_Pagenum = nID;
                    break;
                case ID.ID_FILE_DELETE:
                    OnShowForm(typeof(FormNewSOP), false);
                    OnShowForm(typeof(FormOpenDB), true);
                    m_Pagenum = nID;
                    break;
                case ID.ID_FILE_SAVE:
                    if (CheckSOP())
                    {
                        SaveSOP();
                        UndoRedoManager.Instance.Reset();

                    }
                    m_Pagenum = nID;
                    break;
                case ID.ID_XML_SAVE:
                    if (CheckSOP())
                    {
                        SaveSOPXML();
                        UndoRedoManager.Instance.Reset();
                    }
                    m_Pagenum = nID;
                    break;
                case ID.ID_XML_OPEN:

                    UndoRedoManager.Instance.Reset();
                    OpenSOPXML();
                    m_Pagenum = nID;
                    break;
                case ID.ID_FILE_NEWSOP:
                    OnShowForm(typeof(FormOpenDB), false);
                    OnShowForm(typeof(FormNewSOP), true);
                    m_Pagenum = nID;
                    break;
                case ID.ID_EDIT_UNDO:
                    UndoRedoManager.Instance.Undo();
                    break;
                case ID.ID_EDIT_REDO:
                    UndoRedoManager.Instance.Redo();
                    break;
                case ID.ID_APP_EXIT:
                    this.Close();
                    break;
                case ID.ID_EDIT_LEVEL_ADD:
                    {
                        PopupSelectLevel form = new PopupSelectLevel();
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            string szLevelName = form.LevelName;

                            TabPage tabPage = m_pageSOP.GetCurrentTabPage();
                            //if (tabPage == null)
                            {
                                ArrayList arrTeams = (ArrayList)m_pageSOP.UsingTeam.Clone();

                                if (arrTeams != null)
                                {
                                    UndoRedoManager.Instance.SaveSnapshot();

                                    m_pageSOP.AddTabPage(szLevelName);
                                    m_pageSOP.AddPane(arrTeams, null, true);
                                    m_pageSOP.GetBarPage().SetDataGrid(arrTeams);
                                }
                            }
                        }
                    }
                    break;
                case ID.ID_EDIT_LEVEL_DEL:

                    m_pageSOP.RemoveTabPage();
                    if (m_pageSOP.GetTabPages().Count == 0)
                    {
                        // Properties 초기화
                        m_pageSOP.GetPropertiesLevel().ClearProperties();
                        // Panel 리스트 초기화
                        m_pageSOP.GetBarPage().ClearGrid();
                    }
                    break;
                //case ID.ID_EDIT_DELETE:
                //   m_pageSOP.Delete();
                //   break;
                case ID.ID_EDIT_LEVEL_COPY:
                    CopyTab();
                    break;
                case ID.ID_EDIT_LEVEL_PASTE:
                    PasteTab();
                    break;
                case ID.ID_PANE_ADD:
                    {
                        if (MessageBox.Show("현재 패널의 오른쪽에 새로운 패널을 추가합니다.\r\n이 작업은 현재 열려있는 모든 탭들에 영향을 주게 됩니다.\r\n계속하시겠습니까?", "알림", MessageBoxButtons.YesNo)
                            == DialogResult.Yes)
                        {

                            TabPage tabPage = m_pageSOP.GetCurrentTabPage();
                            ArrayList arrOldTeams = Sections.PanelSectionEx.GetTabPageTeamList(tabPage);
                            if (arrOldTeams != null)
                            {
                                bool bWeekly = FormMain.Instance.GetPageDisaster().IsWeekMode();
                                int nTeamType = (bWeekly == true ? 0 : 1);
                                PopupSelectTeam3 frm = new PopupSelectTeam3(nTeamType, arrOldTeams);
                                frm.Text = "새로운 Panel 생성";

                                if (frm.ShowDialog() == DialogResult.OK)
                                {
                                    StepMemberData data = new StepMemberData();
                                    data.TeamID = frm.SelectedTeamID;
                                    data.TeamName = frm.SelectedTeamName;
                                    data.TeamType = frm.SelectedTeamType;

                                    int nCurrentIndex = Sections.PanelSectionEx.GetLastTabPagePanelIndex(tabPage);

                                    UndoRedoManager.Instance.SaveSnapshot();

                                    m_pageSOP.AddPanel(data, nCurrentIndex + 1);
                                    m_pageSOP.GetBarPage().InsertDataGrid(nCurrentIndex + 1, data);
                                    m_pageSOP.PanelResize();
                                }
                            }

                        }
                    }
                    break;

                case ID.ID_PANE_DELETE:
                    m_pageSOP.DeletePanelLast();
                    m_pageSOP.PanelResize();
                    break;
                default:
                    break;
            };
        }

        private void CopyTab()
        {
            m_tapPageCopySrc = GetPageLevel().GetCurrentTabPage();
        }

        private Sections.PanelSectionEx FindPanel(int nTeamID, int nTeamType, TabPage tabPage)
        {
            Type type = typeof(Sections.PanelSectionEx);

            foreach (Control ctrl in tabPage.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    Sections.PanelSectionEx panel = (Sections.PanelSectionEx)ctrl;

                    if (panel.TeamID == nTeamID && panel.TeamType == nTeamType)
                        return panel;
                }
            }
            return null;
        }

        private bool CopyTab(TabPage pageTrg, TabPage pageSrc)
        {
            Type type = typeof(Sections.PanelSectionEx);
            ArrayList arrSectionsTrg = new ArrayList();
            ArrayList arrSectionsSrc = new ArrayList();

            foreach (Control ctrl in pageSrc.Controls)
            {
                if (ctrl.GetType() == type)
                {
                    Sections.PanelSectionEx panelSrc = (Sections.PanelSectionEx)ctrl;
                    Sections.PanelSectionEx panelTrg = FindPanel(panelSrc.TeamID, panelSrc.TeamType, pageTrg);

                    if (panelTrg == null)
                    {
                        string strError = string.Format("{0}탭에 {1} 패널이 존재하지 않습니다.", pageTrg.Text, panelSrc.TeamName);
                        MessageBox.Show(strError);
                        return false;
                    }

                    panelTrg.ClearData();
                    if (!CopyPanel(arrSectionsTrg, arrSectionsSrc, panelTrg, panelSrc))
                        return false;
                }
            }

            CopyLink(arrSectionsTrg, arrSectionsSrc, pageTrg.Text);
            return true;
        }

        private void CopyLink(ArrayList arrSectionsTrg, ArrayList arrSectionsSrc, string strActionStepName)
        {
            int nSectionCount = arrSectionsTrg.Count;

            for (int i = 0; i < nSectionCount; i++)
            {
                Sections.Section section = (Sections.Section)arrSectionsTrg[i];

                if (section.GetComponentType() == Sections.Section.ComponentType.LINK)
                {
                    Sections.SectionDataLink data = (Sections.SectionDataLink)section.Data;
                    Sections.Section sectionSrc = (Sections.Section)arrSectionsSrc[i];
                    Sections.SectionDataLink dataSrc = (Sections.SectionDataLink)sectionSrc.Data;
                    data.LinkedSection = FindLinkSection(dataSrc.LinkedSection.Data.ComponentID, arrSectionsTrg, strActionStepName);
                }
            }
        }

        private bool CopyPanel(ArrayList arrSectionsTrg, ArrayList arrSectionsSrc, Sections.PanelSectionEx pageTrg, Sections.PanelSectionEx pageSrc)
        {
            int nBeginCount = arrSectionsTrg.Count;

            // Section 복사
            foreach (Sections.Section section in pageSrc.Sections)
            {
                Sections.Section sectionTrg = section.Clone(pageTrg);
                if (sectionTrg == null)
                {
                    MessageBox.Show("이미 같은 데이터가 존재합니다.\r\n복사를 계속할 수 없습니다.");
                    m_tapPageCopySrc = null;
                    return false;
                }

                pageTrg.Sections.Add(sectionTrg);

                arrSectionsTrg.Add(sectionTrg);
                arrSectionsSrc.Add(section);
            }

            int nSectionCount = arrSectionsTrg.Count;

            // Arrow 복사
            for (int i = nBeginCount; i < nSectionCount; i++)
            {
                Sections.Section section = (Sections.Section)arrSectionsTrg[i];
                ArrowFrom(section.Arrows, (Sections.Section)arrSectionsSrc[i], arrSectionsTrg, pageTrg.Parent.Text);
            }

            return true;
        }

        private void ArrowFrom(ArrayList arrArrowTrg, Sections.Section sectionSrc, ArrayList arrSectionsTrg, string strActionStepName)
        {
            foreach (Sections.Arrow arrow in sectionSrc.Arrows)
            {
                Sections.Arrow arrowTrg = new Sections.Arrow();

                arrowTrg.BeginLink = FindLinkSection(arrow.BeginLink.Data.ComponentID, arrSectionsTrg, strActionStepName);
                if (arrowTrg.BeginLink == null)
                    continue;

                arrowTrg.EndLink = FindLinkSection(arrow.EndLink.Data.ComponentID, arrSectionsTrg, strActionStepName);
                if (arrowTrg.EndLink == null)
                    continue;

                arrowTrg.BeginPosition = arrow.BeginPosition;
                arrowTrg.EndPosition = arrow.EndPosition;
                arrowTrg.Text = arrow.Text;

                Sections.Arrow.CopyPoints(arrowTrg, arrow);

                arrArrowTrg.Add(arrowTrg);
            }
        }

        private Sections.Section FindLinkSection(string strComponentID, ArrayList arrSections, string strActionStepName)
        {
            string strComponentIDTrg = strActionStepName + strComponentID.Substring(strComponentID.IndexOf('_'));

            foreach (Sections.Section _section in arrSections)
            {
                if (_section.Data.ComponentID == strComponentIDTrg)
                    return _section;
            }

            return null;
        }

        private void PasteTab()
        {
            if (m_tapPageCopySrc == null)
                return;

            TabPage currentTab = GetPageLevel().GetCurrentTabPage();
            if (currentTab == null)
                return;

            if (m_tapPageCopySrc == currentTab)
            {
                MessageBox.Show("같은 탭끼리는 복사할 수 없습니다.");
                return;
            }

            string szMessage = string.Format("기존 [{0}] 단계에 포함된 모든 컴포넌트가 삭제되며, [{1}] 단계의 컴포넌트가 복사됩니다.\n계속하시겠습니까?", m_tapPageCopySrc.Text, currentTab.Text);

            if (MessageBox.Show(szMessage, "붙여넣기", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                UndoRedoManager.Instance.SaveSnapshot();

                CopyTab(currentTab, m_tapPageCopySrc);
                currentTab.Refresh();
            }
            m_tapPageCopySrc = null;
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_formNewSOP != null)
                m_formNewSOP.Dispose();
            if (m_pageSOP != null)
                m_pageSOP.Dispose();
            if (m_pageHelp != null)
                m_pageHelp.Dispose();
        }

        private void FormMain_Activated(object sender, EventArgs e)
        {
            if (!m_isFirst)
            {
                m_pageSOP.Location = new Point(0, 0);
                m_pageSOP.Dock = DockStyle.Fill;
                m_pageSOP.TopLevel = false;
                m_pageSOP.Parent = this;
                panelSectionContent.Controls.Add(m_pageSOP);
                m_pageSOP.Show();
                m_isFirst = true;
            }
        }

        public void NewSOP()
        {
            //m_ctrlPaste.Enabled = false;

            EnableControlDisaster(true);
            EnableControlPage(false);
            EnableControlLevel(false);

            //m_ControlDisaster.DefaultItem = true;
            //m_ControlPage.DefaultItem = false;

            //FormNewSOP.EnabledPage(true);
            // m_pagePage.SelectedTeamList.Clear();

            m_pageSOP.RemoveAll();

            // 기존 Section들의 ID 정보 초기화
            //Sections.SectionData.ClearIDList();

            ArrayListClear();
            ReadDB();

            FindNormalPath();

            m_versionCurrent = null;


            UndoRedoManager.Instance.Reset();
        }

        private void ArrayListClear()
        {
            DisasterCategory.Clear();
            SubDisasterCategory.Clear();
            DetailDisasterCategory.Clear();
            TemporaryNormalTeam.Clear();
            TemporaryEmergencyTeam.Clear();
            CheckTask.Clear();
            RegularTeam.Clear();
            ActionStep.Clear();
            UserDefinedTeam.Clear();
            ExternalTeam.Clear();
            SOPVersion.Clear();
        }
        //////////////////////////////////////////////////////////////////////////
        public FormNewSOP GetPageDisaster()
        {
            return m_formNewSOP;
        }

        public FormPageSOP GetPageLevel()
        {
            return m_pageSOP;
        }

        public PageBackstageHelp GetPageHelp()
        {
            return m_pageHelp;
        }

        public void EnableControlDisaster(bool isDisable)
        {
            //m_ControlDisaster.Enabled = isDisable;
            Refresh();
        }

        public void EnableControlPage(bool isDisable)
        {
            //m_ControlPage.Enabled = isDisable;
            Refresh();
        }

        public void EnableControlLevel(bool isDisable)
        {
            //m_ControlLevel.Enabled = isDisable;
            Refresh();
        }

        //////////////////////////////////////////////////////////////////////////
        // Read DB 
        private void ReadDisasterCategory()
        {
            string strSql = "SELECT * FROM DisasterCategory";
            ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);

            for (int i = 0; i < arrResult.Count - 1; i += 2)
            {
                Data_DisasterCategory dataNew = new Data_DisasterCategory();
                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.CategoryName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");

                DisasterCategory.Add(dataNew);
            }
        }

        private void ReadSubDisasterCategory()
        {
            string strSql = "SELECT * FROM SubDisasterCategory";
            ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);

            for (int i = 0; i < arrResult.Count - 2; i += 3)
            {
                Data_SubDisasterCategory dataNew = new Data_SubDisasterCategory();
                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.DisasterID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                dataNew.CategoryName = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");

                SubDisasterCategory.Add(dataNew);
            }
        }

        private void ReadDisaster()
        {
            string strSql = "SELECT * FROM Disaster";
            ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);

            for (int i = 0; i < arrResult.Count - 4; i += 5)
            {
                Data_Disaster dataNew = new Data_Disaster();
                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.DisasterName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                dataNew.SubDisasterID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.VersionID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                dataNew.Description = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");

                DetailDisasterCategory.Add(dataNew);
            }
        }

        private void ReadTeamporaryNormalTeam()
        {
            string strSql = "SELECT ID,TeamName,ParentTeamID,GroupName,LevelNo,Description,RegularTeamLink FROM TemporaryNormalTeam";
            ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);

            for (int i = 0; i < arrResult.Count - 6; i += 7)
            {
                Data_NormalTeam dataNew = new Data_NormalTeam();
                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                dataNew.ParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.GroupName = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                dataNew.LevelNo = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                dataNew.Description = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
                dataNew.RegularTeamLink = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                m_dicNormalTeam[dataNew.ID] = dataNew;
                TemporaryNormalTeam.Add(dataNew);
            }
        }

        private void ReadTeamporaryEmergencyTeam()
        {
            string strSql = "SELECT ID,TeamName,ParentTeamID,GroupName,LevelNo,Description,RegularTeamLink FROM TemporaryEmergencyTeam";
            ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);

            for (int i = 0; i < arrResult.Count - 6; i += 7)
            {
                Data_EmergencyTeam dataNew = new Data_EmergencyTeam();
                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1].ToString(), "");
                dataNew.ParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.GroupName = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                dataNew.LevelNo = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                dataNew.Description = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
                dataNew.RegularTeamLink = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                TemporaryEmergencyTeam.Add(dataNew);
            }
        }

        private void ReadCheckTask()
        {
            string strSql = "SELECT * FROM CheckTask";
            ArrayList arrResult = m_dbMgr.GetResultData(strSql, 0);

            for (int i = 0; i < arrResult.Count - 6; i += 7)
            {
                Data_CheckTask dataNew = new Data_CheckTask();
                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.ProcessID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                dataNew.Category = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                dataNew.SubCategory = WebDBManager.GetStringField(arrResult[i + 3].ToString(), "");
                dataNew.TaskName = WebDBManager.GetStringField(arrResult[i + 4].ToString(), "");
                dataNew.TargetCount = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                dataNew.Position = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                CheckTask.Add(dataNew);
            }
        }

        private void ReadRegularTeam()
        {
            string strSQL = "SELECT * FROM RegularTeam";

            // 부모 노드가 먼저 나오도록 정렬하기 위한 임시 변수
            ArrayList arrRegularTeams = new ArrayList();

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 2; i += 3)
            {
                Data_RegularTeam dataNew = new Data_RegularTeam();

                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                dataNew.ParentTeamID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);

                arrRegularTeams.Add(dataNew);
                //RegularTeam.Add(dataNew);
            }

            // arrSrc의 Team 데이터들을 부모노드가 먼저 나오도록 정렬하여 arrTrg에 담는다.
            SortNAdd(RegularTeam, arrRegularTeams);
        }

        // arrSrc의 Team 데이터들을 부모노드가 먼저 나오도록 정렬하여 arrTrg에 담는다.
        private void SortNAdd(ArrayList arrTrg, ArrayList arrSrc, int nParentTeamID = 0)
        {
            ArrayList arrRemove = new ArrayList();

            foreach (Data_RegularTeam team in arrSrc)
            {
                if (team.ParentTeamID == nParentTeamID)
                {
                    arrTrg.Add(team);
                    arrRemove.Add(team);
                }
            }

            foreach (Data_RegularTeam team in arrRemove)
            {
                arrSrc.Remove(team);
            }

            foreach (Data_RegularTeam team in arrRemove)
            {
                SortNAdd(arrTrg, arrSrc, team.ID);
            }
        }

        private void ReadActionStep()
        {
            string strSQL = "SELECT * FROM ActionStep";

            DateTime dtDefault = new DateTime();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 11; i += 12)
            {
                Data_ActionStep dataNew = new Data_ActionStep();

                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.StepName = WebDBManager.GetStringField(arrResult[i + 1], "");
                dataNew.PeriodType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.BeginTime = WebDBManager.GetDateTimeField(arrResult[i + 3].ToString(), dtDefault);
                dataNew.EndTime = WebDBManager.GetDateTimeField(arrResult[i + 4].ToString(), dtDefault);
                dataNew.WeekdayOption = WebDBManager.GetIntField(arrResult[i + 5].ToString(), 0);
                dataNew.Iteration = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                dataNew.IterationType = WebDBManager.GetIntField(arrResult[i + 7].ToString(), 0);
                dataNew.ProcessTime = WebDBManager.GetIntField(arrResult[i + 8].ToString(), 0);
                dataNew.ProcessTimeType = WebDBManager.GetIntField(arrResult[i + 9].ToString(), 0);
                dataNew.DisasterID = WebDBManager.GetIntField(arrResult[i + 10].ToString(), 0);
                dataNew.ParentStepID = WebDBManager.GetIntField(arrResult[i + 11].ToString(), 0);

                ActionStep.Add(dataNew);
            }
        }

        private void ReadUserDefinedTeam()
        {
            string strSQL = "SELECT * FROM UserDefinedTeam";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 3; i += 4)
            {
                Data_UserDefinedTeam dataNew = new Data_UserDefinedTeam();

                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                dataNew.PhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");
                dataNew.FaxNumber = WebDBManager.GetStringField(arrResult[i + 3], "");

                UserDefinedTeam.Add(dataNew);
            }
        }

        public void ReadExternalTeam()
        {
            ExternalTeam.Clear();
            string strSQL = "SELECT * FROM ExternalTeam";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 3; i += 4)
            {
                Data_ExternalTeam dataNew = new Data_ExternalTeam();

                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.TeamName = WebDBManager.GetStringField(arrResult[i + 1], "");
                dataNew.PhoneNumber = WebDBManager.GetStringField(arrResult[i + 2], "");
                dataNew.FaxNumber = WebDBManager.GetStringField(arrResult[i + 3], "");

                if (dataNew.PhoneNumber == "null")
                    dataNew.PhoneNumber = "";

                if (dataNew.FaxNumber == "null")
                    dataNew.FaxNumber = "";

                ExternalTeam.Add(dataNew);
            }
        }

        private void ReadVersion()
        {
            string strSQL = "SELECT * FROM Version";
            //string strSQL = "SELECT * FROM Version WHERE id = (SELECT MAX(id) FROM Version )";

            DateTime dtDefault = new DateTime();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            for (int i = 0; i < arrResult.Count - 7; i += 8)
            {
                Data_Version dataNew = new Data_Version();

                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.Regular = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                dataNew.Normal = WebDBManager.GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.CreateTime = WebDBManager.GetDateTimeField(arrResult[i + 3].ToString(), dtDefault);
                dataNew.LastAccessTime = WebDBManager.GetDateTimeField(arrResult[i + 4].ToString(), dtDefault);
                dataNew.VersionName = WebDBManager.GetStringField(arrResult[i + 5], "");
                dataNew.OwnerID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), 0);
                dataNew.Description = WebDBManager.GetStringField(arrResult[i + 7], "");

                SOPVersion.Add(dataNew);
            }
        }
        //////////////////////////////////////////////////////////////////////////

        private void FindNormalPath()
        {
            m_arrFullPath.Clear();
            foreach (Data_NormalTeam data in TemporaryNormalTeam)
            {
                string strPath = data.TeamName;
                TemporaryTeamFullPath fullPath = new TemporaryTeamFullPath();
                if (data.ParentTeamID != 0)
                {
                    ArrayList arrPath = FindParent(data.ParentTeamID);
                    strPath = GetPath(data.ID, arrPath);
                    strPath += data.TeamName;
                }

                fullPath.ID = data.ID;
                fullPath.FullPath = strPath;

                m_arrFullPath.Add(fullPath);

                m_arrPath.Clear();
            }
        }

        private ArrayList FindParent(int nParentID)
        {
            if (m_dicNormalTeam.ContainsKey(nParentID))
            {
                m_arrPath.Add(m_dicNormalTeam[nParentID].TeamName);

                if (m_dicNormalTeam[nParentID].ParentTeamID != 0)
                {
                    FindParent(m_dicNormalTeam[nParentID].ParentTeamID);
                }
            }
            return m_arrPath;
        }

        private string GetPath(int nID, ArrayList arrPath)
        {
            string strPath = "";
            for (int i = arrPath.Count - 1; i >= 0; i--)
            {
                strPath += arrPath[i] + "/";
            }

            return strPath;
        }

        public string ParseCaption(string strValue)
        {
            string[] result = strValue.Split(new char[] { '>' });
            result = result[2].Split(new char[] { '<' });

            return result[0];
        }

        private void OpenSOPXML()
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "XML Files|*.xml|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "XML 파일 열기";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string strFileName = dlg.FileName;

                XMLManager mgr = new XMLManager();

                if (mgr.Load(this, strFileName))
                {
                    SelectTab(1);
                    UndoRedoManager.Instance.SaveSnapshot();
                }
                else
                {
                    string strError = mgr.ErrorMessage.Length == 0 ? "XML 불러오기가 실패하였습니다." : mgr.ErrorMessage;
                    MessageBox.Show(strError);
                }
            }
        }

        public void OpenSOP()
        {

            GetPageLevel().RemoveAll();

            ArrayList arrTabPages = GetPageLevel().GetTabPage();
            int nOldTabPageCount = arrTabPages == null ? 0 : arrTabPages.Count;

            // 기존 Section들의 ID 정보 초기화
            Sections.SectionData.ClearIDList();

            FormOpenDB frm = (FormOpenDB)GetForm(typeof(FormOpenDB));
            VersionInfo version = frm.Version;
            ArrayList arrActionSteps = frm.ActionSteps;
            string strCategoryName = frm.CategoryName;
            string strSubCategoryName = frm.SubCategoryName;
            string strDisasterName = frm.DisasterName;
            // 등록모드인가?
            bool isRegular = frm.IsRegular;
            // 평일모드인가?
            bool isNormal = frm.IsNormal;

            m_formNewSOP.SetWeekMode(isNormal);
            m_formNewSOP.SetRegularMode(isRegular);

            IOManager mgr = new IOManager();
            if (!mgr.Load(this, m_dbMgr, version, arrActionSteps, strCategoryName, strSubCategoryName, strDisasterName))
            {
                m_versionCurrent = null;
                MessageBox.Show("SOP 불러오기가 실패하였습니다.");
                return;
            }

            string strRegular = "미등록모드";
            string strWeekday = "야간 및 휴일";

            if (m_formNewSOP.IsRegularMode())
                strRegular = "등록모드";
            if (m_formNewSOP.IsWeekMode())
                strWeekday = "평일";

            this.Text = "SOP Manager  V 2.0 - " + strRegular + ", " + strWeekday;

            m_formNewSOP.SelectedCategory = strCategoryName;
            m_formNewSOP.SelectedSubCategory = strSubCategoryName;
            m_formNewSOP.SelectedDetailCategory = strDisasterName;

            m_versionCurrent = version;

            // 기존 탭이 남아 있게 되는데, 불러오기 후 해당 탭들을 삭제한다.
            for (int i = 0; i < nOldTabPageCount; i++)
            {
                TabPage oldTabPage = (TabPage)arrTabPages[0];
                GetPageLevel().RemoveTabPage(oldTabPage, false);
                arrTabPages.RemoveAt(0);
                GetPageLevel().GetPropertiesLevel().LevelProperties.RemoveAt(0);
            }

            SelectTab(1);
        }

        public bool SaveXML(string strFileName, out string szError)
        {
            szError = "";
            XMLManager mgr = new XMLManager();

            int nIndex = strFileName.LastIndexOf('\\');
            int nDotIndex = strFileName.LastIndexOf('.');
            string strVersionName = "";

            if (nIndex >= 0 && nDotIndex >= 0)
                strVersionName = strFileName.Substring(nIndex + 1, nDotIndex - 1 - nIndex);
            else if (nIndex >= 0)
                strVersionName = strFileName.Substring(nIndex + 1);
            else if (nDotIndex >= 0)
                strVersionName = strFileName.Substring(0, nDotIndex - 1);
            else
                strVersionName = strFileName;

            if (!mgr.Save(FormMain.Instance, strFileName, strVersionName))
            {
                szError = mgr.ErrorMessage;
                return false;
            }
            return true;
        }

        public bool SaveXML(System.IO.Stream stream, string strVersion, out string szError)
        {
            szError = "";
            XMLManager mgr = new XMLManager();

            if (!mgr.Save(FormMain.Instance, stream, strVersion))
            {
                szError = mgr.ErrorMessage;
                return false;
            }
            return true;
        }

        private void SaveSOPXML()
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "XML Files|*.xml|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "XML 파일로 저장";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string strFileName = dlg.FileName;
                string szError = "";
                if (!SaveXML(strFileName, out szError))
                {
                    MessageBox.Show(szError);
                    return;
                }
            }
        }

        private void SaveSOP()
        {
            string strCategory = this.m_formNewSOP.SelectedCategory;
            string strSubCategory = this.m_formNewSOP.SelectedSubCategory;
            string strDisaster = this.m_formNewSOP.SelectedDetailCategory;

            if (strCategory != null && strSubCategory != null && strDisaster != null && m_nSOPGenUserID > 0)
            {
                bool isRegular = GetPageDisaster().IsRegularMode();
                bool isNormal = GetPageDisaster().IsWeekMode();
                FormSaveVersion saveVersion = new FormSaveVersion(m_dbMgr, m_nSOPGenUserID, strCategory, strSubCategory, strDisaster, isRegular, isNormal, m_versionCurrent);

                if (saveVersion.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string strVersionName = saveVersion.VersionName;
                    // nVersionID가 0보다 크면 기존 버전을 덮어쓴다.
                    int nVersionID = saveVersion.VersionID;
                    string strDescription = saveVersion.Description;

                    VersionInfo version = new VersionInfo();
                    IOManager mgr = new IOManager();
                    int nDisasterID;

                    if (mgr.Save(this, m_dbMgr, strVersionName, nVersionID, m_nSOPGenUserID, strDescription, ref version, out nDisasterID))
                    {
                        version.UserName = m_strSOPGenUserRealName;
                        m_versionCurrent = version;
                    }
                    else
                        m_versionCurrent = null;

                    if (form != null)
                    {
                        form.InitTree();
                        form.SelectNode(3, nDisasterID);
                    }
                }
            }
            ReadActionStep();
        }

        private void AddVersion(VersionInfo version)
        {
            foreach (Data_Version ver in m_arrSOPVersion)
            {
                if (ver.ID == version.VersionID)
                {
                    ver.CreateTime = version.BeginTime;
                    ver.LastAccessTime = version.EndTime;
                    ver.Description = version.Description;
                    ver.VersionName = version.VersionName;
                    return;
                }
            }

            Data_Version newVersion = new Data_Version();

            newVersion.ID = version.VersionID;
            newVersion.CreateTime = version.BeginTime;
            newVersion.LastAccessTime = version.EndTime;
            newVersion.Description = version.Description;
            newVersion.VersionName = version.VersionName;

            m_arrSOPVersion.Add(newVersion);
        }

        private bool CheckProcess(Sections.SectionProcess section)
        {
            Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

            if (data.TeamList.Count < 0)
            {
                MessageBox.Show("임무를 수행할 대상이 지정되지 않은 [프로세스] 태그가 존재합니다.\r\n확인후 저장하십시오.");
                ZoomNSelectSection(section);
                return false;
            }

            return true;
        }

        private void PrepareCheckEndPoint(Sections.SectionEndPoint section, ref int nStart, ref int nEnd)
        {
            Sections.SectionDataEndPoint data = (Sections.SectionDataEndPoint)section.Data;

            if (data.IsBegin)
            {
                nStart++;
            }
            else
                nEnd++;
        }

        private bool CheckTransSOP(Sections.SectionTransSOP section, ref bool useTransSOP)
        {
            Sections.SectionDataTransSOP data = (Sections.SectionDataTransSOP)section.Data;

            if (data.LinkedActionStepID < 0)
            {
                MessageBox.Show("전환할 SOP 대상이 지정되지 않은 [SOP 전환] 태그가 존재합니다.\r\n확인후 저장하십시오.");
                ZoomNSelectSection(section);
                return false;
            }
            else
                useTransSOP = true;

            return true;
        }

        private bool CheckLink(Sections.SectionLink section)
        {
            Sections.SectionDataLink data = (Sections.SectionDataLink)section.Data;

            if (data.LinkedSection == null)
            {
                MessageBox.Show("링크될 대상이 지정되지 않은 [Link] 태그가 존재합니다.\r\n확인후 저장하십시오.");
                ZoomNSelectSection(section);
                return false;
            }
            else
            {
                if (!IsValidSection(data.LinkedSection))
                {
                    data.LinkedSection = null;
                    MessageBox.Show("링크될 대상이 이미 삭제된 [Link] 태그가 존재합니다.\r\n링크될 대상을 다시 지정후 저장하십시오.");
                    ZoomNSelectSection(section);
                    return false;
                }
            }

            return true;
        }

        private bool CheckExternal(Sections.SectionExternal section)
        {
            Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;

            if (data.UseSMS)
            {
                if (data.SMSMessage.Length == 0)
                {
                    MessageBox.Show("SMS 메시지 내용이 비어 있는 [외부 상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.");
                    ZoomNSelectSection(section);
                    return false;
                }
                else if (data.SMSReceivers.Count == 0)
                {
                    MessageBox.Show("SMS 수신처가 비어 있는 [외부 상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.");
                    ZoomNSelectSection(section);
                    return false;
                }
            }

            if (data.UseFax)
            {
                if (data.FaxReceivers.Count == 0)
                {
                    MessageBox.Show("Fax 수신처가 비어 있는 [외부 상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.");
                    ZoomNSelectSection(section);
                    return false;
                }
            }

            return true;
        }

        private bool CheckTransmission(Sections.SectionTransmission section)
        {
            Sections.SectionDataTransmission data = (Sections.SectionDataTransmission)section.Data;
            Sections.SectionDataTransmission.ExternalData external = data.DataExternal;

            if (external.UseSMS)
            {
                if (external.SMSMessage.Length == 0)
                {
                    MessageBox.Show("SMS 메시지 내용이 비어 있는 [상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.");
                    ZoomNSelectSection(section);
                    return false;
                }
                else if (external.SMSReceivers.Count == 0)
                {
                    MessageBox.Show("SMS 수신처가 비어 있는 [상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.");
                    ZoomNSelectSection(section);
                    return false;
                }
            }

            if (external.UseFax)
            {
                if (external.FaxReceivers.Count == 0)
                {
                    MessageBox.Show("Fax 수신처가 비어 있는 [상황전파] 태그가 존재합니다.\r\n확인후 저장하십시오.");
                    ZoomNSelectSection(section);
                    return false;
                }
            }

            return true;
        }

        private bool CheckSOP()
        {
            TabControl ctrl = m_pageSOP.TabControls;
            if (ctrl.TabPages.Count == 0)
                return false;

            foreach (TabPage page in ctrl.TabPages)
            {
                int nStart = 0, nEnd = 0;
                bool useTransSOP = false;

                string szStepName = page.Text;
                string szHeader = string.Format("[{0}] - ", szStepName);
                foreach (Sections.PanelSectionEx panel in page.Controls)
                {
                    string szTeam = panel.TeamName;
                    string szHeader2 = string.Format("[{0}][{1}] - ", szStepName, szTeam);
                    foreach (Sections.Section section in panel.Sections)
                    {
                        Sections.Section.ComponentType type = section.GetComponentType();

                        if (type == Sections.Section.ComponentType.PROCESS)
                        {
                            if (!CheckProcess((Sections.SectionProcess)section))
                                return false;
                        }
                        else if (type == Sections.Section.ComponentType.ENDPOINT) // 시작/끝
                        {
                            PrepareCheckEndPoint((Sections.SectionEndPoint)section, ref nStart, ref nEnd);
                        }
                        else if (type == Sections.Section.ComponentType.TRANSSOP)
                        {
                            if (!CheckTransSOP((Sections.SectionTransSOP)section, ref useTransSOP))
                                return false;
                        }
                        else if (type == Sections.Section.ComponentType.LINK)
                        {
                            if (!CheckLink((Sections.SectionLink)section))
                                return false;
                        }
                        else if (type == Sections.Section.ComponentType.EXTERNAL)
                        {
                            if (!CheckExternal((Sections.SectionExternal)section))
                                return false;
                        }
                        else if (type == Sections.Section.ComponentType.TRANSMISSION)
                        {
                            if (!CheckTransmission((Sections.SectionTransmission)section))
                                return false;
                        }
                    }
                }
                if (nStart == 0)
                {
                    TabControl control = (TabControl)page.Parent;
                    if (control != null)
                    {
                        control.SelectedTab = page;
                    }

                    MessageBox.Show(szHeader + "[시작] 태그가 없습니다.\r\n확인후 저장하십시오.");
                    return false;
                }
                else if (nStart > 1)
                {
                    TabControl control = (TabControl)page.Parent;
                    if (control != null)
                    {
                        control.SelectedTab = page;
                    }

                    MessageBox.Show(szHeader + string.Format("[시작] 태그가 {0}개 존재합니다.\r\n[시작] 태그는 반드시 하나만 존재하여야 합니다.\r\n확인후 저장하십시오.", nStart));
                    return false;
                }
                if (nEnd == 0)
                {
                    // TransSOP가 있으면 [종료] 태그를 대신할 수 있다.
                    if (!useTransSOP)
                    {
                        TabControl control = (TabControl)page.Parent;
                        if (control != null)
                        {
                            control.SelectedTab = page;
                        }

                        MessageBox.Show(szHeader + "[종료] 태그가 없습니다.\r\n확인후 저장하십시오.");
                        return false;
                    }
                }
            }

            return true;
        }

        private void ZoomNSelectSection(Sections.Section section)
        {
            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();
            if (panel != null)
            {
                TabPage page = (TabPage)panel.Parent;
                if (page != null)
                {
                    TabControl control = (TabControl)page.Parent;
                    if (control != null)
                    {
                        control.SelectedTab = page;
                    }
                }

                panel.ClearSelection();
                panel.SelectSection(section);
                panel.ZoomSection(section);
            }
        }

        private bool IsValidSection(Sections.Section section)
        {
            Sections.PanelSectionEx panel = (Sections.PanelSectionEx)section.GetParent();

            foreach (Sections.Section _section in panel.Sections)
            {
                if (section == _section)
                    return true;
            }

            return false;
        }

        public void SaveExternalTeam(string strTeamName)
        {
            IOManager mgr = new IOManager();
            mgr.AddExternalTeam(m_dbMgr, strTeamName, true);
        }

        public void ShowSpecialMessage()
        {
            if (m_frmSpecialMessage == null)
            {
                m_frmSpecialMessage = new PopupSpecialMessage();
                m_frmSpecialMessage.TopMost = true;
            }

            m_frmSpecialMessage.Show();
        }

        public void HideSpecialMessage()
        {
            if (m_frmSpecialMessage != null)
                m_frmSpecialMessage.Hide();
        }

        public VersionInfo CurrentVersion
        {
            get { return m_versionCurrent; }
        }
    }

    public class TemporaryTeamFullPath
    {
        private int m_nID;
        private string m_strFullPath;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string FullPath
        {
            get { return m_strFullPath; }
            set { m_strFullPath = value; }
        }
    }
}
