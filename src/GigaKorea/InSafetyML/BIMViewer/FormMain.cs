using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Data.SqlClient;
using BIMViewer.DB;
using System.Configuration;
using System.Reflection;
using BIMViewer.PopupForms;
using UnE.GUI;
using System.IO;
using BIMViewer.Shapes;
using BIMViewer.BIM;//
using BIMViewer.uControl;
using static BIMViewer.BIM.Column;

namespace BIMViewer
{
         
    public partial class FormMain : Form, IProjectOwner, ILevelOwner, IUIMaster
    {
        private class TabPageData
        {
            private bool m_completeDrawing = false;
            private TabControl m_tabCtrl = null;
            private string m_strName = "";

            private int m_nCloseImageLeft = 0, m_nCloseImageTop = 0;
            private int m_nCloseImageRight = 0, m_nCloseImageBottom = 0;
            private int m_nTabPageLeft = 0, m_nTabPageTop = 0;
            private int m_nTabPageRight = 0, m_nTabPageBottom = 0;

            private bool m_isMouseOverTabPage = false;
            private bool m_isMouseOverCloseImage = false;

            public bool MouseOverTabPage
            {
                get { return m_isMouseOverTabPage; }
                set { m_isMouseOverTabPage = value; }
            }

            public bool MouseOverCloseImage
            {
                get { return m_isMouseOverCloseImage; }
                set { m_isMouseOverCloseImage = value; }
            }

            public string Name
            {
                get { return m_strName; }
            }

            public TabPageData(TabControl tabCtrl, string strTabName)
            {
                m_tabCtrl = tabCtrl;
                m_strName = strTabName;
            }

            public bool IsFirstDrawing()
            {
                foreach (TabPage page in m_tabCtrl.TabPages)
                {
                    TabPageData data = (TabPageData)page.Tag;

                    if (data.m_completeDrawing)
                        return false;
                }

                return true;
            }

            public void CompleteDrawing()
            {
                m_completeDrawing = true;
            }

            public static void CheckComplete(TabControl tabCtrl)
            {
                foreach (TabPage page in tabCtrl.TabPages)
                {
                    TabPageData data = (TabPageData)page.Tag;

                    if (data.m_completeDrawing == false)
                        return;
                }

                foreach (TabPage page in tabCtrl.TabPages)
                {
                    TabPageData data = (TabPageData)page.Tag;
                    data.m_completeDrawing = false;
                }
            }

            public void SetTabPageRect(int x, int y, int width, int height)
            {
                m_nTabPageLeft = x;
                m_nTabPageTop = y;
                m_nTabPageRight = x + width;
                m_nTabPageBottom = y + height;
            }

            public void SetCloseImageRect(int x, int y, int width, int height)
            {
                m_nCloseImageLeft = x;
                m_nCloseImageTop = y;
                m_nCloseImageRight = x + width;
                m_nCloseImageBottom = y + height;
            }

            // Return 값 : 변화가 있는가?
            public bool CheckMouseOver(int x, int y)
            {
                bool isChanged = false;

                if (x >= m_nCloseImageLeft && x <= m_nCloseImageRight &&
                    y >= m_nCloseImageTop && y <= m_nCloseImageBottom)
                {
                    isChanged = m_isMouseOverTabPage == false || m_isMouseOverCloseImage == false;
                    m_isMouseOverTabPage = m_isMouseOverCloseImage = true;
                }
                else if (x >= m_nTabPageLeft && x <= m_nTabPageRight &&
                    y >= m_nTabPageTop && y <= m_nTabPageBottom)
                {
                    isChanged = m_isMouseOverTabPage == false || m_isMouseOverCloseImage;
                    m_isMouseOverTabPage = true;
                    m_isMouseOverCloseImage = false;
                }
                else
                {
                    isChanged = m_isMouseOverTabPage || m_isMouseOverCloseImage;
                    m_isMouseOverTabPage = m_isMouseOverCloseImage = false;
                }

                return isChanged;
            }
        }

        private int m_nSplitDistance = 210;
        private bool m_initSplitDistance = false;

        private BIM.BIMManager m_bimManager = new BIM.BIMManager();
        public BIM.BIMManager BimManager
        {
            get { return m_bimManager; }
        }
        private ProjectView m_viewProject = null;
        private BasePlan m_basePlan = null;
        private LevelView m_viewLevel = null;

        //ym
        private uFwall m_uFwall = null;
        private uSWall m_uSwall = null;
        private uHwall m_uHwall = null;
        private uDoor m_uDoor = null;
        private uWindow m_uWindow = null;
        private uSpace m_uSpace = null;
        private uRect m_uRect = null;
        private uCircle m_uCircle = null;

        private uAlertArea m_uAlertArea = null;
        private uCWall m_uCwall = null;

        private uBuilding m_uBuilding = null;

        private enum SelShapePropType { Swall = 0, Fwall, Hwall, Circle, Rect, Space, Door, Window, AlertArea, Cwall };
        private SelShapePropType m_selShapePropType = SelShapePropType.Swall;
        private BIM.Project m_PropertyProject = null;//현재 pnlProperty에 표현된 Project
        private uSave m_uSave = null;//save 

        private Dictionary<string, string> m_spaceUserList = null;
        private bool m_bLogin = false;//logout. login = true;
        private string m_strLoginID = "";
        private string m_strLoginPW = "";

        private List<Shapes.Layer> m_layers = null;
        private FormLayer m_frmLayer = null;
        private FormProperty m_frmProperty = null;
        private PopupPOILayer m_frmPOILayer = null;

        private Color m_clrTabPageBackNormal = Color.FromArgb(54, 78, 111);
        private Color m_clrTabPageBackSelected = Color.FromArgb(255, 242, 157);
        private Color m_clrTabPageForeNormal = Color.White;
        private Color m_clrTabPageForeSelected = Color.Black;
        private Color m_clrTabBorder = Color.FromArgb(41, 57, 85);

        private TabPage m_prevSelectedPage = null;
        private TabPage m_currentSelectedPage = null;
        
        private Timer m_timerPanelPOI = null;
        private bool m_bPanelPOISizeFull = false;
        private Timer m_timerPanelLine = null;
        private bool m_bPanelLineSizeFull = false;

        private FormWindowState m_lastState = FormWindowState.Minimized;
        private BIM.Project m_currentProject = null;

        private Timer m_timerPanelBuilding = null;
        private Timer m_timerPanelProperty = null;
        private bool m_bPanelBuildingSizeFull = false;
        private bool m_bPanelPropertySizeFull = false;

        private List<string> m_listAlertAreaGroup = new List<string>();
        private List<string> m_listAlertAreaType = new List<string>();


        public bool IsAddMode
        {
            get { return rbtnAdd.IsChecked; }
        }

        public bool IsMoveMode
        {
            get { return rbtnMove.IsChecked; }
        }

        public bool IsDeleteMode
        {
            get { return rbtnDelete.IsChecked; }
        }

        public bool IsDoneMode
        {
            get { return rbtnDone.IsChecked; }
        }

        public bool IsAddModeWire
        {
            get { return rbtnAddLine.IsChecked; }
        }

        public bool IsMoveModeWire
        {
            get { return rbtnMoveLine.IsChecked; }
        }

        public bool IsDeleteModeWire
        {
            get { return rbtnDeleteLine.IsChecked; }
        }

        public bool IsDoneModeWire
        {
            get { return rbtnDoneLine.IsChecked; }
        }

        public bool IsPropertyMode
        {
            get { return rbtnProperty.IsChecked; }
        }

        public POIType SelectedPOI
        {
            get
            {
                POIType poiType = cbPOIList.SelectedItem as POIType;
                return poiType;
            }
        }

        public POITypeProperty SelectedWire
        {
            get
            {
                POITypeProperty poiType = cbLineList.SelectedItem as POITypeProperty;
                return poiType;
            }
        }

        public bool IsTabMode
        {
            get { return tabControl1.Visible; }
        }

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private WebServiceManager m_webServiceMgr = null;

        public List<string> AlertAreaGroup
        {
            get { return m_listAlertAreaGroup; }
        }

        public List<string> AlertAreaType
        {
            get { return m_listAlertAreaType; }
        }

        private void InitAlertAreaList()
        {
            m_listAlertAreaGroup.Add("직접입력");

            m_listAlertAreaType.Add("직접입력");
            m_listAlertAreaType.Add("계단");
            m_listAlertAreaType.Add("램프");
            m_listAlertAreaType.Add("린넨슈트");
            m_listAlertAreaType.Add("파이프덕트");
            m_listAlertAreaType.Add("엘리베이터");
            m_listAlertAreaType.Add("에스컬레이터");
        }

        public void AddAlertAreaGroup(string strAlertAreaGroup)
        {
            if (strAlertAreaGroup == "" || strAlertAreaGroup == null)
                return;

            if (!m_listAlertAreaGroup.Contains(strAlertAreaGroup))
                m_listAlertAreaGroup.Add(strAlertAreaGroup);
        }

        public void AddAlertAreaType(string strAlertAreaType)
        {
            if (strAlertAreaType == "" || strAlertAreaType == null)
                return;

            if (!m_listAlertAreaType.Contains(strAlertAreaType))
                m_listAlertAreaType.Add(strAlertAreaType);
        }

        

        public FormMain()
        {
            InitializeComponent();

            m_webServiceMgr = new WebServiceManager();

            //ribbonButton1.Visible = rbtnFormLayer.Visible = false;

            this.rbtnOpen.Font = new System.Drawing.Font("Montserrat", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnSave.Font = new System.Drawing.Font("Montserrat", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnUpload.Font = new System.Drawing.Font("Montserrat", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbtnDownload.Font = new System.Drawing.Font("Montserrat", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));

            m_instance = this;

            this.DoubleBuffered = true;

            SetDoubleBuffer(panel1, true);
            SetDoubleBuffer(panelTop, true);
            SetDoubleBuffer(panelTitle, true);
            SetDoubleBuffer(panelToolbar, true);
            SetDoubleBuffer(rbtnOpen, true);
            SetDoubleBuffer(rbtnSave, true);
            SetDoubleBuffer(rbtnUpload, true);
            SetDoubleBuffer(ribbonButton1, true);
            SetDoubleBuffer(rbtnFormLayer, true);
            SetDoubleBuffer(imageButton1, true);
            SetDoubleBuffer(btnMin, true);
            SetDoubleBuffer(btnMax, true);
            SetDoubleBuffer(btnClose, true);
            SetDoubleBuffer(splitContainerLeft, true);
            SetDoubleBuffer(splitContainerMain, true);
            SetDoubleBuffer(tbLayoutPanel_top, true);
            SetDoubleBuffer(panelPOI, true);
            SetDoubleBuffer(panelLine, true);
            SetDoubleBuffer(clblLogin, true);
            SetDoubleBuffer(colorLabel2, true);
            SetDoubleBuffer(tabControl1, true);
            
            //panelToolbar.Visible = false;
            //splitContainerMain.Visible = false;

            rbtnPOI.Enabled = rbtnLine.Enabled = false;
            panelPOI.Size = panelLine.Size = new Size(70, 90);
            panelLine.Location = new Point(panelPOI.Location.X + panelPOI.Width, panelPOI.Location.Y);
            //ym 임시가리기
            rbtnPOI.Visible = rbtnLine.Visible = rbtnEdit.Visible = rbtnLayer.Visible = false;
            panelPOI.Visible = panelLine.Visible = false;

            // AlertArea 속성 리스트 초기화 
            InitAlertAreaList();

            //ym
            m_uFwall = new uFwall();
            m_uSwall = new uSWall();
            m_uHwall = new uHwall();
            m_uDoor = new uDoor();
            m_uWindow = new uWindow();
            m_uSpace = new uSpace();
            m_uRect = new uRect();
            m_uCircle = new uCircle();

            m_uAlertArea = new uAlertArea();
            m_uCwall = new uCWall();

            m_uBuilding = new uBuilding();
            pnlBuilding.Width = m_uBuilding.Size.Width;
            pnlBuilding.Controls.Add(m_uBuilding);
            m_uBuilding.Dock = DockStyle.Fill;
            rbtnBuildingDone.Location = new Point(pnlBuilding.Location.X + pnlBuilding.Width, pnlBuilding.Location.Y);

            panelBuilding.Size = new Size(70, 90);
            panelBuilding.Visible = true;
            
            panelProperty.Location = new Point(panelBuilding.Location.X + panelBuilding.Size.Width, panelBuilding.Location.Y);
            panelProperty.Size = new Size(70, 90);

            pnlProperty.Location = new Point(rbtnProperty.Location.X + rbtnProperty.Width, rbtnProperty.Location.Y);
            rbtnPropertyDone.Location = new Point(pnlProperty.Location.X + pnlProperty.Width, pnlProperty.Location.Y);
            pnlProperty.Size = rbtnProperty.Size;
            rbtnPropertyDone.Size = pnlProperty.Size;
            pnlProperty.Visible = rbtnPropertyDone.Visible = false;

            m_uSave = new uSave(this);
            pnlSave.Location = new Point(rbtnSave.Location.X, 0);
            pnlSave.Size = m_uSave.Size;            
            pnlSave.Controls.Add(m_uSave);
            m_uSave.Dock = DockStyle.Fill;
            pnlSave.Visible = false;

            //login user
            // 공간정보 관리자1명, 사용자 2명
            m_spaceUserList = new Dictionary<string, string>();
            m_spaceUserList.Add("user_spatial", "spatial1234");
            m_spaceUserList.Add("ACC_0001_20190822152055207", "spaceInfo100");
            m_spaceUserList.Add("ACC_0001_20190822153213528", "spaceInfo200");

            m_instance = this;
            m_viewProject = new ProjectView(this);
            m_basePlan = new BasePlan();
            m_viewLevel = new LevelView(this);
            splitContainerMain.Panel2.Layout += Panel2_Layout;

            m_timerPanelPOI = new Timer();
            m_timerPanelPOI.Interval = 10;
            m_timerPanelPOI.Tick += M_timerPanelPOI_Tick;

            m_timerPanelLine = new Timer();
            m_timerPanelLine.Interval = 10;
            m_timerPanelLine.Tick += M_timerPanelLine_Tick;

            m_timerPanelBuilding = new Timer();
            m_timerPanelBuilding.Interval = 10;
            m_timerPanelBuilding.Tick += M_timerPanelBuilding_Tick;

            m_timerPanelProperty = new Timer();
            m_timerPanelProperty.Interval = 10;
            m_timerPanelProperty.Tick += M_timerPanelProperty_Tick;

            this.MouseWheel += FormMain_MouseWheel;

            
        }

        void FormMain_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!IsTabMode)
            {
                if (splitContainerMain.Panel2.Controls.Count > 1 && splitContainerMain.Panel2.Controls[0] is FormView)
                {
                    FormView selectedView = splitContainerMain.Panel2.Controls[0] as FormView;
                    if (selectedView != null)
                    {
                        if (e.X < (splitContainerMain.Panel2.Left + selectedView.Location.X))
                            return;
                        if (e.X > (splitContainerMain.Panel2.Left + selectedView.Location.X + selectedView.Size.Width))
                            return;
                        if (e.Y < (splitContainerMain.Top + selectedView.Location.Y))
                            return;
                        if (e.Y > (splitContainerMain.Top + selectedView.Location.Y + selectedView.Size.Height))
                            return;

                        Point location = new Point();
                        location.X = e.X - (splitContainerMain.Panel2.Left + selectedView.Location.X);
                        location.Y = e.Y - (splitContainerMain.Panel2.Top + selectedView.Location.Y);
                        int delta = e.Delta;

                        selectedView.TransferMouseWheel(location, delta);
                    }
                } 
            }
            else
            {
                if (tabControl1.SelectedTab == null)
                    return;

                FormView selectedView = tabControl1.SelectedTab.Controls[0] as FormView;

                Point location = new Point();
                location.X = e.X - (splitContainerMain.Panel2.Left + selectedView.Location.X);
                location.Y = e.Y - (splitContainerMain.Panel2.Top + selectedView.Location.Y);
                int delta = e.Delta;

                selectedView.TransferMouseWheel(location, delta);
            }
        }

        #region Form Reszie, Form 이동
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(Brushes.Silver, Top);
            e.Graphics.FillRectangle(Brushes.Silver, Left);
            e.Graphics.FillRectangle(Brushes.Silver, Right);
            e.Graphics.FillRectangle(Brushes.Silver, Bottom);
        }

        private const int
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;

        const int _ = 5; // you can rename this variable if you like

        Rectangle Top { get { return new Rectangle(0, 0, this.ClientSize.Width, _); } }
        Rectangle Left { get { return new Rectangle(0, 0, _, this.ClientSize.Height); } }
        Rectangle Bottom { get { return new Rectangle(0, this.ClientSize.Height - _, this.ClientSize.Width, _); } }
        Rectangle Right { get { return new Rectangle(this.ClientSize.Width - _, 0, _, this.ClientSize.Height); } }

        Rectangle TopLeft { get { return new Rectangle(0, 0, _, _); } }
        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - _, 0, _, _); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - _, _, _); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - _, this.ClientSize.Height - _, _, _); } }
        
        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == 0x84) // WM_NCHITTEST
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (Top.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (Left.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (Right.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (Bottom.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();

        private void panelTitle_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Maximized;

            SetWindowMaxButton();
        }

        private void panelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = e.Location;
            }

            m_isClicked = true;
        }

        private void panelTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            if (this.WindowState == FormWindowState.Maximized)
            {
                Point test = this.PointToScreen(e.Location);

                this.WindowState = FormWindowState.Normal;

                float xPer = ((float)m_ptOrigin.X / (float)1920) * 100;
                float yPer = ((float)m_ptOrigin.Y / (float)panelTitle.Height) * 100;

                float xPer2 = (float)panelTitle.Width * xPer / 100;
                float yPer2 = (float)panelTitle.Height * yPer / 100;

                this.Location = new Point(test.X - (int)xPer2, test.Y - (int)yPer2);
            }

            Point ptCur = this.Location;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;

            SetWindowMaxButton();
        }

        private void panelTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }
        #endregion

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.MaximizedBounds = Screen.AllScreens[0].WorkingArea;
            this.WindowState = FormWindowState.Maximized;
            //this.Size = this.MaximizedBounds.Size;
            this.Location = new Point(0, 0);

            m_bimManager.LoadPOIColor();

            Dictionary<string, bool> dicLayerLineVisible = GetLayerDefaultVisible("LayerLineVisible");
            Dictionary<string, bool> dicLayerFillVisible = GetLayerDefaultVisible("LayerFillVisible");
            Dictionary<string, bool> dicLayerTextVisible = GetLayerDefaultVisible("LayerTextVisible");
            m_layers = Shapes.Layer.InitLayers(dicLayerLineVisible, dicLayerFillVisible, dicLayerTextVisible);

            splitContainerMain.SplitterDistance = splitContainerMain.Panel1MinSize = m_nSplitDistance;
            splitContainerLeft.SplitterDistance = splitContainerLeft.Size.Height / 5;
            
            splitContainerLeft.Panel1.Controls.Add(m_viewProject);
            m_viewProject.Dock = DockStyle.Fill;
            m_viewProject.Show();

            splitContainerLeft2.Panel1.Controls.Add(m_basePlan);
            m_basePlan.Dock = DockStyle.Fill;
            m_basePlan.Show();         

            splitContainerLeft2.Panel2.Controls.Add(m_viewLevel);
            m_viewLevel.Dock = DockStyle.Fill;
            m_viewLevel.Show();

#if DB_USE
            _SqlConnection.DataBaseType dbType;
            string strConnection = GetConnectionString(out dbType);
            
            _SqlConnection connection = new _SqlConnection(dbType, strConnection);
            {
                connection.Open();

                m_bimManager.LoadPOITypes(connection);
                m_bimManager.LoadPOITypeProperty(connection);

                List<BIM.Project> projects = m_bimManager.GetProjectList(connection);
                connection.Close();

                m_viewProject.SetProjects(projects);
            }
#elif XML_USE
            //string strXMLPath = GetLocalXMLResourceFolder();
            //List<BIM.Project> projects = m_bimManager.GetProjectList(strXMLPath);

            //m_viewProject.SetProjects(projects);
#endif

            // POI ComboBox
            cbPOIList.DrawItem += CbPOIList_DrawItem;
            cbPOIList.ValueMember = "Name";
            cbPOIList.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbPOIList.AutoCompleteSource = AutoCompleteSource.ListItems;
            LoadPOIComboList();

            // Line ComboBox
            cbLineList.DrawItem += CbLineList_DrawItem;
            cbLineList.ValueMember = "PropertyName";
            cbLineList.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbLineList.AutoCompleteSource = AutoCompleteSource.ListItems;
            
            m_lastState = this.WindowState;
        }

        #region DoubleBuffer
        public static void SetDoubleBuffer(Panel panel, bool bEnabled)
        {
            Type dgvType1 = panel.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(panel, bEnabled, null);
        }

        public static void SetDoubleBuffer(SplitContainer panel, bool bEnabled)
        {
            Type dgvType1 = panel.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(panel, bEnabled, null);
        }
        public static void SetDoubleBuffer(RibbonButton btn, bool bEnabled)
        {
            Type dgvType1 = btn.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(btn, bEnabled, null);
        }
        public static void SetDoubleBuffer(ImageButton btn, bool bEnabled)
        {
            Type dgvType1 = btn.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(btn, bEnabled, null);
        }
        public static void SetDoubleBuffer(TableLayoutPanel panel, bool bEnabled)
        {
            Type dgvType1 = panel.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(panel, bEnabled, null);
        }
        public static void SetDoubleBuffer(Label label, bool bEnabled)
        {
            Type dgvType1 = label.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(label, bEnabled, null);
        }
        public static void SetDoubleBuffer(TabControl tab, bool bEnabled)
        {
            Type dgvType1 = tab.GetType();
            PropertyInfo pi1 = dgvType1.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi1.SetValue(tab, bEnabled, null);
        }
        #endregion

        private void Panel2_Layout(object sender, LayoutEventArgs e)
        {
            if (!IsTabMode)
            {
                if (splitContainerMain.Panel2.Controls.Count > 1)
                {
                    FormView frm = splitContainerMain.Panel2.Controls[0] is FormView ? (FormView)splitContainerMain.Panel2.Controls[0] : (FormView)splitContainerMain.Panel2.Controls[1];
                    BIM.Level level = frm.Level;
                    BIM.Project project = frm.Project;
                    
                    if (m_basePlan != null && !m_basePlan.IsDisposed)
                    {
                        m_basePlan.GDIOwner = frm;
                        m_basePlan.SetLayers(level.GetLayers(), level.GetDXFLayers());
                        m_basePlan.SetBtnAddEnabel(true);
                    }

                    if (m_frmProperty != null && !m_frmProperty.IsDisposed)
                    {
                        m_frmProperty.SelectedShape = frm.GetSelectedShape();
                    } 

                    if (m_frmPOILayer != null && !m_frmPOILayer.IsDisposed)
                    {
                        m_frmPOILayer.SetLayers(level.GetLayers(), project.Name, level.ID);
                    }

                    if (m_frmLayer != null)
                    {
                        m_frmLayer.GDIOwner = frm;
                        m_frmLayer.SetLayers(level.GetLayers(), level.GetDXFLayers());
                    }

                    this.OnSelectProject(project);
                    m_viewProject.SetSelectProject(project);
                    m_viewLevel.SetSelectLevel(level);
                }
                else
                {
                    if (m_basePlan != null && !m_basePlan.IsDisposed)
                        m_basePlan.SetBtnAddEnabel(false);
                }
            }
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (!m_initSplitDistance)
            {
                splitContainerMain.SplitterDistance = splitContainerMain.Panel1MinSize = m_nSplitDistance;
                splitContainerLeft.SplitterDistance = splitContainerLeft.Panel1MinSize = splitContainerLeft.Size.Height / 5;

                //ym
                splitContainerLeft.SplitterWidth = 1;
                splitContainerLeft2.SplitterWidth = 1;
                m_initSplitDistance = true;
            }
        }

        private void FormMain_ResizeBegin(object sender, EventArgs e)
        {
            if (tabControl1.TabCount > 0)
            {
                for (int i = 0; i < tabControl1.TabCount; i++)
                {
                    TabPage tabPage = tabControl1.TabPages[i];
                    FormView form = (FormView)tabPage.Controls[0];
                    // 크기 변경중에는 View를 갱신하지 말것
                    form.NoResizeReshape();
                }
            }
        }

        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
            if (tabControl1.TabCount > 0)
            {
                TabPage tabPage = tabControl1.SelectedTab;

                if (tabPage != null)
                {
                    FormView form = (FormView)tabPage.Controls[0];
                    form.RefreshView();
                }
            }
        }

        private void rbtnLogin_Click(object sender, EventArgs e)
        {

        }

        private void rbtnOpen_Click(object sender, EventArgs e)
        {
#if DB_USE
            _SqlConnection.DataBaseType dbType;
            string strConnection = GetConnectionString(out dbType);

            _SqlConnection connection = new _SqlConnection(dbType, strConnection);
            {
                connection.Open();

                List<BIM.Project> projects = m_bimManager.GetProjectList(connection);

                if (projects == null)
                    MessageBox.Show(m_bimManager.ErrorMessage);
                else
                {
                    Point panelTopLocation = panelToolbar.PointToScreen(Point.Empty);
                    int x = panelTopLocation.X + rbtnUpload.Location.X + rbtnUpload.Size.Width;
                    int y = panelTopLocation.Y + rbtnUpload.Location.Y + rbtnUpload.Size.Height / 2;

                    FormSelectProject frm = new FormSelectProject(projects);
                    frm.StartPosition = FormStartPosition.Manual;
                    frm.Location = new Point(x, y);

                    if (frm.ShowDialog(panelToolbar) == DialogResult.OK)
                    {
                        List<BIM.Level> levels = m_bimManager.LoadProject(frm.SelectedProject, connection);
                        System.Diagnostics.Trace.WriteLine(levels);
                    }
                }

                connection.Close();
            }
#elif XML_USE
            //string strXMLPath = GetLocalXMLResourceFolder();
            //List<BIM.Project> projects = m_bimManager.GetProjectList(strXMLPath);

            //Point panelTopLocation = panelToolbar.PointToScreen(Point.Empty);
            //int x = panelTopLocation.X + rbtnUpload.Location.X + rbtnUpload.Size.Width;
            //int y = panelTopLocation.Y + rbtnUpload.Location.Y + rbtnUpload.Size.Height / 2;

            //FormSelectProject frm = new FormSelectProject(projects);
            //frm.StartPosition = FormStartPosition.Manual;
            //frm.Location = new Point(x, y);

            //if (frm.ShowDialog(panelToolbar) == DialogResult.OK)
            //{
            //    List<BIM.Level> levels = m_bimManager.LoadXMLProject(frm.SelectedProject);
            //    System.Diagnostics.Trace.WriteLine(levels);
            //}

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "xml files|*.xml";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                BIM.Project project = m_bimManager.GetProject(dialog.FileName);
                if (project != null)
                {                 
                    m_viewProject.SetProject(project);
                    LoadPOIComboList();
                    LoadPOITypePropertyComboList();
                }
                
            }
#endif

        }

        public void OnSelectProject(BIM.Project project)
        {
#if DB_USE
            _SqlConnection.DataBaseType dbType;
            string strConnection = GetConnectionString(out dbType);

            _SqlConnection connection = new _SqlConnection(dbType, strConnection);
            {
                connection.Open();

                List<BIM.Level> levels = m_bimManager.LoadProject(project, connection);

                if (levels != null)
                    m_viewLevel.SetLevels(levels, project);

                connection.Close();
                m_currentProject = project;
            }
#elif XML_USE
            //m_viewProject.SetSelectProject(project);
            
            List<BIM.Level> levels = m_bimManager.LoadXMLProject(project);

            if (levels != null)
                m_viewLevel.SetLevels(levels, project);
            else//levl is null. project is null.ym
                m_viewLevel.DeleteGridLevels();

            m_currentProject = project;
            m_uBuilding.SetBuildingData(project);
#endif
        }
        //프로젝트 삭제.ym
        public void OnDeleteProject(BIM.Project project)
        {
            m_bimManager.DeleteProject(project);

            //열린 층.도면폼들 닫기
            foreach(Level level in project.Levels)
            {
                FormView frm = FindView(level);
                if (frm != null)
                {
                    if(!IsTabMode)
                        frm.Close();
                    else
                    {
                        foreach(TabPage page in tabControl1.TabPages)
                        {
                            FormView form = (FormView)page.Controls[0];
                            if(frm == form)
                            {
                                RemoveTabPage(page);
                                break;
                            }
                        }
                    }
                }
            }

            //Property창이 현재 닫는 Project의 창이면 닫기
            if(project == m_PropertyProject)
            {
                //pnlProperty.Visible = false;
                //rbtnPropertyDone.Visible = false;
                //pnlProperty.Hide();
                //rbtnPropertyDone.Hide();
                m_bPanelPropertySizeFull = false;
                m_timerPanelProperty.Enabled = true;
            }
        }
        public void RemoveBasePlanGrid()
        {
            m_basePlan.RemoveGridList();
        }
        public void OpenLevel(BIM.Level level, BIM.Project project)
        {
            this.Cursor = Cursors.WaitCursor;
            FormView view = FindView(level);

            /*
            if (view != null)
            {
                this.Cursor = Cursors.Default;
                view.FocusView();                
                return;
            }*/
            //ym. 해당층 탭뷰로 선택되게

            if(view != null)
            {
                if (IsTabMode)
                {
                    foreach (TabPage tabPage in tabControl1.TabPages)
                    {
                        if (view == (FormView)tabPage.Controls[0])
                        {
                            tabControl1.SelectedTab = tabPage;
                            break;
                        }
                    }
                }
                else
                {
                    view.FocusView();
                }

                this.Cursor = Cursors.Default;
                return;
            }


#if DB_USE
            _SqlConnection.DataBaseType dbType;
            string strConnection = GetConnectionString(out dbType);

            _SqlConnection connection = new _SqlConnection(dbType, strConnection);
            {
                connection.Open();

                if (m_bimManager.LoadLevel(m_layers, level, project, connection))
                    _OpenLevel(level, project);

                connection.Close();
            }
#elif XML_USE
            if (m_bimManager.LoadXMLLevel(m_layers, level, project))
            {
                _OpenLevel(level, project);
            }
#endif
            this.Cursor = Cursors.Default;
        }

        private void _OpenLevel(BIM.Level level, BIM.Project project)
        {
            List<Shapes.Layer> layers = level.GetLayers();
            List<DXFLayer> dxfLayers = level.GetDXFLayers();

            FormView frm = new FormView(layers, dxfLayers, level, this, project);
            frm.MdiParent = this;
            frm.SystemInput = true;

            string localFilePath = project.LocalFilePath;
            int index = localFilePath.LastIndexOf(@"\");
            string projectName = localFilePath.Substring(index + 1);
            projectName = projectName.Replace(".xml", "");

            frm.SetTitle(level.Name + " - " + projectName);

            if (IsTabMode)
            {
                TabPage page = AddTabPage(frm, true);
                frm.Show();
                tabControl1.SelectedTab = page;
            }
            else
            {
                frm.Show();
                splitContainerMain.Panel2.Controls.Add(frm);
                frm.FocusView();
            }

            frm.SystemInput = false;
        }

        private TabPage AddTabPage(FormView frm, bool insert = false)
        {
            TabPage page = new TabPage(frm.Text + "     ");
            page.Controls.Add(frm);
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;

            if (insert)
            {
                insert = false;
                int nTabCount = tabControl1.TabCount;

                for (int i = 0; i < nTabCount; i++)
                {
                    TabPage tabPage = tabControl1.TabPages[i];
                    FormView form = (FormView)tabPage.Controls[0];

                    if (form.CompareTo(frm) > 0)
                    {
                        tabControl1.TabPages.Insert(i, page);
                        insert = true;
                        break;
                    }
                }

                if (!insert)
                    tabControl1.TabPages.Add(page);
            }
            else
                tabControl1.TabPages.Add(page);
            page.Tag = new TabPageData(tabControl1, frm.Level.Name);

            return page;
        }

        private void RemoveTabPage(TabPage page)
        {
            FormView frm = (FormView)page.Controls[0];
            page.Controls.Clear();
            frm.Close();
            tabControl1.TabPages.Remove(page);

            if (tabControl1.TabCount == 0)
            {
                ShowTabControl(false);
            }
        }

        private void ShowTabControl(bool visible)
        {
            m_prevSelectedPage = m_currentSelectedPage = null;
            tabControl1.Visible = visible;
        }

        private FormView FindView(BIM.Level level)
        {
            if (IsTabMode)
            {
                foreach (TabPage page in tabControl1.TabPages)
                {
                    FormView frm = (FormView)page.Controls[0];

                    if (frm.Level == level)
                        return frm;
                }
            }
            else
            {
                foreach (Control ctrl in splitContainerMain.Panel2.Controls)
                {
                    if (ctrl is FormView)
                    {
                        FormView view = (FormView)ctrl;

                        if (view.Level == level)
                            return view;
                    }
                }
            }

            return null;
        }

        private void rbtnEdit_Click(object sender, EventArgs e)
        {
            rbtnEdit.IsChecked = !rbtnEdit.IsChecked;
            rbtnDelete.Enabled = rbtnEdit.IsChecked;

            if (rbtnEdit.IsChecked == false)
                rbtnDelete.IsChecked = false;

            if (rbtnEdit.IsChecked)
            {
                rbtnPOI.Enabled = true;
                rbtnLine.Enabled = true;
            }
            else
            {
                rbtnPOI.IsChecked = rbtnAdd.IsChecked = rbtnMove.IsChecked = rbtnDelete.IsChecked = rbtnDone.IsChecked = false;
                rbtnPOI.Enabled = false;
                m_bPanelPOISizeFull = false;
                m_timerPanelPOI.Enabled = true;
                rbtnLine.IsChecked = rbtnAddLine.IsChecked = rbtnMoveLine.IsChecked = rbtnDeleteLine.IsChecked = rbtnDoneLine.IsChecked = false;
                rbtnLine.Enabled = false;
                m_bPanelLineSizeFull = false;
                m_timerPanelLine.Enabled = true;
            }
        }

        private FormPOI m_frmPOI = null;
        private object xRoot;

        private void rbtnPOI_Click(object sender, EventArgs e)
        {
            rbtnPOI.IsChecked = !rbtnPOI.IsChecked;
            
            if (rbtnPOI.IsChecked)
            {
                m_bPanelPOISizeFull = true;
                m_timerPanelPOI.Enabled = true;

                // Line 패널 닫기
                rbtnLine.IsChecked = rbtnAddLine.IsChecked = rbtnMoveLine.IsChecked = rbtnDeleteLine.IsChecked = rbtnDoneLine.IsChecked = false;
                rbtnLine.Refresh();
                m_bPanelLineSizeFull = false;
                m_timerPanelLine.Enabled = true;
            }
            else
            {
                m_bPanelPOISizeFull = false;
                m_timerPanelPOI.Enabled = true;

                rbtnAdd.IsChecked = rbtnMove.IsChecked = rbtnDelete.IsChecked = false;
            }
        }

        private void LoadPOIComboList()
        {
            cbPOIList.Items.Clear();

            foreach (KeyValuePair<int, POIType> item in m_bimManager.POITypes)
            {                
                if (!item.Value.IsGroup)
                    cbPOIList.Items.Add(item.Value);
            }

            if (cbPOIList.Items.Count > 0)
                cbPOIList.SelectedIndex = 0;
        }

        public void SetPOIComboList(POIType chgPoiType, bool isNew, bool isDelete = false)
        {
            POIType selectedItem = (POIType)cbPOIList.SelectedItem;

            cbPOIList.Items.Clear();
            LoadPOIComboList();

            if (chgPoiType.ID == selectedItem.ID && isDelete)
                cbPOIList.SelectedIndex = 0;
            else
                cbPOIList.SelectedItem = selectedItem;
        }

        private void LoadPOITypePropertyComboList() // Line
        {
            cbLineList.Items.Clear();

            foreach (KeyValuePair<int, POIType> item in m_bimManager.POITypes)
            {
                if (item.Value.Properties.Count > 0)
                {
                    POITypeProperty property = new POITypeProperty();
                    property.POITypeID = item.Key;
                    property.PropertyName = item.Value.Name;// item.Value.Properties[0].Name;
                    property.ProperetyValue = item.Value.Properties[0].Value;
                    property.Description = item.Value.Properties[0].Description;

                    cbLineList.Items.Add(property);
                }
            }

            if (cbLineList.Items.Count > 0)
                cbLineList.SelectedIndex = 0;
        }

        private void CbPOIList_DrawItem(object sender, DrawItemEventArgs e)
        {
            POIType _cbData = null;

            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0)
            {
                _cbData = cbPOIList.Items[e.Index] as POIType;
                if (_cbData == null)
                    return;
                                
                int imgWidth = 16;

                System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;
                Bitmap img = (Bitmap)rm.GetObject("empty");
                if (!_cbData.UserDefined)
                {
                    if (_cbData.Code.Length == 0)
                    {
                        e.Graphics.DrawImage(img, e.Bounds.Left, e.Bounds.Top, imgWidth, e.Bounds.Height);
                        //e.Graphics.DrawString(_cbData.Name, e.Font, new SolidBrush(cbPOIList.ForeColor), e.Bounds.Left + imgWidth + 2, e.Bounds.Top);
                        //e.Graphics.DrawString(_cbData.Name, e.Font, new SolidBrush(cbPOIList.ForeColor), e.Bounds.Left, e.Bounds.Top);
                    }
                    else
                    {
                        img = (Bitmap)rm.GetObject(_cbData.Code);
                        if (img == null)
                            img = (Bitmap)rm.GetObject("empty");

                        e.Graphics.DrawImage(img, e.Bounds.Left, e.Bounds.Top, imgWidth, e.Bounds.Height);
                        //e.Graphics.DrawString(_cbData.Name, e.Font, new SolidBrush(cbPOIList.ForeColor), e.Bounds.Left + imgWidth + 2, e.Bounds.Top);
                    }
                }
                else
                {
                    Brush br = new SolidBrush(_cbData.Color);
                    Pen pen = new Pen(_cbData.Color);

                    Rectangle rect = new Rectangle(e.Bounds.Left, e.Bounds.Top, 13, 13);

                    e.Graphics.DrawEllipse(pen, rect);
                    e.Graphics.FillPie(br, rect, 360, -360);                    
                }
                e.Graphics.DrawString(_cbData.Name, e.Font, new SolidBrush(cbPOIList.ForeColor), e.Bounds.Left + imgWidth + 2, e.Bounds.Top);
            }
        }

        private void CbLineList_DrawItem(object sender, DrawItemEventArgs e)
        {
            POITypeProperty _cbData = null;

            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0)
            {
                _cbData = cbLineList.Items[e.Index] as POITypeProperty;
                if (_cbData == null)
                    return;

                int imgWidth = 32;

                System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;
                Bitmap img = (Bitmap)rm.GetObject("empty");

                foreach (KeyValuePair<int, POIType> item in m_bimManager.POITypes)
                {
                    if (item.Key == _cbData.POITypeID)
                    {
                        if (item.Value.Code.Length > 0)
                        {
                            img = (Bitmap)rm.GetObject(item.Value.Code);
                            if (img == null)
                                img = (Bitmap)rm.GetObject("empty");
                        }
                        break;
                    }
                }
                e.Graphics.DrawImage(img, e.Bounds.Left, e.Bounds.Top, imgWidth, e.Bounds.Height);
                

                e.Graphics.DrawString(_cbData.PropertyName, e.Font, new SolidBrush(cbLineList.ForeColor), e.Bounds.Left + imgWidth + 2, e.Bounds.Top);
            }
        }

        private void M_timerPanelPOI_Tick(object sender, EventArgs e)
        {
            int maxWidth = 840;
            int minWidth = 70;
            int gap = 70;

            if (m_bPanelPOISizeFull)
            {
                if (panelPOI.Width == maxWidth)
                    m_timerPanelPOI.Enabled = false;
                else
                {
                    if (panelPOI.Width + gap > maxWidth)
                        panelPOI.Width += maxWidth - panelPOI.Width;
                    else
                        panelPOI.Width += gap;

                    panelLine.Location = new Point(panelPOI.Location.X + panelPOI.Width, panelPOI.Location.Y);
                }
            }
            else
            {
                if (panelPOI.Width <= minWidth)
                    m_timerPanelPOI.Enabled = false;
                else
                {
                    if (panelPOI.Width - gap < minWidth)
                        panelPOI.Width -= minWidth - panelPOI.Width;
                    else
                        panelPOI.Width -= gap;

                    panelLine.Location = new Point(panelPOI.Location.X + panelPOI.Width, panelPOI.Location.Y);
                }
            }
        }

        private void rbtnMove_Click(object sender, EventArgs e)
        {
            rbtnMove.IsChecked = !rbtnMove.IsChecked;
            if (rbtnMove.IsChecked)
            {
                rbtnAdd.IsChecked = false;
                rbtnAdd.Refresh();
                rbtnDelete.IsChecked = false;
                rbtnDelete.Refresh();
            }
        }
        
        private void rbtnDelete_Click(object sender, EventArgs e)
        {
            rbtnDelete.IsChecked = !rbtnDelete.IsChecked;
            if (rbtnDelete.IsChecked)
            {
                rbtnAdd.IsChecked = false;
                rbtnAdd.Refresh();
                rbtnMove.IsChecked = false;
                rbtnMove.Refresh();
            }
        }

        private void rbtnDone_Click(object sender, EventArgs e)
        {
            m_bPanelPOISizeFull = false;
            m_timerPanelPOI.Enabled = true;


            rbtnPOI.IsChecked = rbtnAdd.IsChecked = rbtnMove.IsChecked = rbtnDelete.IsChecked = false;
            rbtnPOI.Refresh();
        }

#if DB_USE
        public bool UpdatePOIToDB(Shapes.POI poi, BIM.Level level, bool isDelete)
        {
            if (level == null || poi == null)
                return false;

            _SqlConnection.DataBaseType dbType;
            string strConnection = GetConnectionString(out dbType);

            _SqlConnection connection = new _SqlConnection(dbType, strConnection);
            {
                connection.Open();

                string strSQL = "";
                int nID = 0;

                _SqlTransaction transaction = null;

                if (isDelete)
                {
                    transaction = new _SqlTransaction(dbType, connection);
                    strSQL = string.Format("Delete From PoiWire Where (BeginPOI = {0} OR EndPOI = {0}) And LevelID = {1}", poi.ID, level.ID);
                    if (m_bimManager.ExecuteQuery(strSQL, connection, transaction))
                    {
                        strSQL = string.Format("Delete from POI where ID = {0}", poi.ID);
                    }
                    else
                    {
                        transaction.Rollback(dbType);
                        connection.Close();
                        return false;
                    }
                }
                else
                {
                    if (poi.ID == 0)
                    {
                        string strFormat = "Insert into POI (ID, TypeID, Name, X, Y, Angle, Height, LevelID) values ";
                        strFormat += "({0}, {1}, '{2}', {3}, {4}, 0.0, NULL, {5})";

                        nID = GetMaxTableID("POI", connection) + 1;

                        strSQL = string.Format(strFormat,
                            nID,
                            poi.PoiType.ID,
                            poi.Name, poi.Position.x, poi.Position.y,
                            /*poi.PoiType.Name*/ level.ID);

                        level.AddPOI(poi);
                    }
                    else
                    {
                        strSQL = string.Format("Update POI set X = {0}, Y = {1} where ID = {2}", poi.Position.x, poi.Position.y, poi.ID);
                    }
                }

                if (m_bimManager.ExecuteQuery(strSQL, connection, transaction))
                {
                    if (transaction != null)
                        transaction.Commit(dbType);

                    if (poi.ID == 0)
                        poi.ID = nID;

                    connection.Close();
                    return true;
                }

                connection.Close();
            }

            return false;
        }
#endif

#if XML_USE
        public bool UpdatePOIToXML(Shapes.POI poi, BIM.Project project, BIM.Level level, bool isDelete)
        {
            if (level == null || poi == null)
                return false;

            if (isDelete)
                level.RemovePOI(poi);
            else
            {
                if (poi.ID == 0)
                {
                    poi.XMLID = Shapes.POI.POIIDTag + System.Guid.NewGuid().ToString();
                    poi.ID = poi.XMLID.GetHashCode();
                    level.AddPOI(poi);
                }
            }

            XMLManager mgr = new XMLManager();
            return mgr.Save(project, m_bimManager.POITypes);
        }
#endif

#if DB_USE
        public bool UpdateWireToDB(Shapes.Wire wire, bool isDelete)
        {
            if (wire == null)
                return false;

            _SqlConnection.DataBaseType dbType;
            string strConnection = GetConnectionString(out dbType);

            _SqlConnection connection = new _SqlConnection(dbType, strConnection);
            {
                connection.Open();

                string strSQL = "";
                int nID = 0;

                if (isDelete)
                    strSQL = string.Format("Delete from POIWire where ID = {0}", wire.ID);
                else
                {
                    if (wire.ID == 0)
                    {
                        string strFormat = "Insert into POIWire (ID, BeginPOI, EndPOI, POITypeID, Lines, LevelID) values ";
                        strFormat += "({0}, {1}, {2}, {3}, '{4}', {5})";

                        nID = GetMaxTableID("POIWire", connection) + 1;

                        strSQL = string.Format(strFormat,
                            nID,
                            wire.BeginPOI, wire.EndPOI, wire.POITypeID, wire.Lines, wire.LevelID);
                    }
                    else
                    {
                        strSQL = string.Format("Update POIWire set Lines = '{0}' where ID = {1}", wire.Lines, wire.ID);
                    }
                }

                if (m_bimManager.ExecuteQuery(strSQL, connection, null))
                {
                    if (wire.ID == 0)
                        wire.ID = nID;

                    connection.Close();
                    return true;
                }

                connection.Close();
                return false;
            }
        }
#endif

#if XML_USE
        public bool UpdateWireToXML(Shapes.Wire wire, BIM.Project project, BIM.Level level, bool isDelete)
        {
            if (wire == null)
                return false;

            if (isDelete)
                level.RemoveWire(wire);
            else
            {
                if (wire.ID == 0)
                {
                    wire.XMLID = Shapes.Wire.WireIDTag + System.Guid.NewGuid().ToString();
                    wire.ID = wire.XMLID.GetHashCode();
                    level.AddWire(wire);
                }
            }

            XMLManager mgr = new XMLManager();
            return mgr.Save(project, m_bimManager.POITypes);
        }
#endif
 
        public int GetMaxTableID(string strTableName, _SqlConnection connection)
        {
            string strSQL = string.Format("Select max(ID) from {0}", strTableName);
            _SqlDataReader reader = m_bimManager.ReadQuery(strSQL, connection, null);
            int nID = 0;

            if (reader != null)
            {
                if (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                        nID = reader.GetInt32(0);
                }

                reader.Close();
            }

            return nID;
        }

        private BIM.Level GetCurrentLevel(out FormView frm)
        {
            frm = null;

            if (IsTabMode)
            {
                if (tabControl1.TabCount == 0)
                    return null;

                TabPage page = tabControl1.SelectedTab; //TabPages[0];
                frm = (FormView)page.Controls[0];
                return frm.Level;
            }

            if (splitContainerMain.Panel2.Controls.Count == 0)
                return null;

            if (splitContainerMain.Panel2.Controls[0] is FormView)
                frm = (FormView)splitContainerMain.Panel2.Controls[0];
            else
                return null;
                        
            return frm.Level;
        }

        public void DeleteUserPOI(int poiID)
        {
            if (!IsTabMode)
            {
                foreach (Control ctrl in splitContainerMain.Panel2.Controls)
                {
                    if (ctrl is FormView)
                    {
                        FormView frm = (FormView)ctrl;
                        BIM.Level level = frm.Level;
                        List<Shapes.Layer> layers = level.GetLayers();
                        RemoveUserPOI(layers, poiID);
                        frm.RefreshView();
                    }
                }
            }
            else
            {
                foreach (TabPage page in tabControl1.TabPages)
                {
                    FormView frm = (FormView)page.Controls[0];
                    BIM.Level level = frm.Level;
                    List<Shapes.Layer> layers = level.GetLayers();
                    RemoveUserPOI(layers, poiID);
                    frm.RefreshView();
                }
            }
        }
        private void RemoveUserPOI(List<Shapes.Layer> layers, int poiID)
        {
            int layerIndex = -1;
            List<Shape> deletePOI = new List<Shape>();
            foreach (Layer layer in layers)
            {
                layerIndex++;
                if (layer.LayerType == typeof(POI))
                {
                    foreach (Shape shape in layer.Shapes)
                    {
                        POI poi = shape as POI;
                        if (poi.PoiType.ID == poiID)
                        {
                            deletePOI.Add(shape);
                        }
                    }
                }

                if (deletePOI.Count > 0)
                    break;
            }

            List<Layer> dd = layers.Where(p => p.LayerType == typeof(POI)).ToList();
            foreach (Shape item in deletePOI)
            {
                dd[layerIndex].RemoveShape(item);
            }
        }

        private void rbtnLayer_Click(object sender, EventArgs e)
        {
            rbtnLayer.IsChecked = !rbtnLayer.IsChecked;
            
            if (rbtnLayer.IsChecked)
            {
                if (m_frmPOILayer == null || m_frmPOILayer.IsDisposed)
                {
                    FormView frm = null;
                    BIM.Level level = GetCurrentLevel(out frm);
                    List<Shapes.Layer> layers = level == null ? null : level.GetLayers();

                    ShowPOILayer(frm, layers);
                }
            }            
            else
            {
                if (!m_frmPOILayer.IsDisposed)
                {
                    m_frmPOILayer.Close();
                }
            }
        }

#if DB_USE
        private void ShowPOILayer(FormView frm, List<Shapes.Layer> layers)
        {
            _SqlConnection.DataBaseType dbType;
            string strConnection = GetConnectionString(out dbType);

            _SqlConnection connection = new _SqlConnection(dbType, strConnection);
            {
                connection.Open();

                m_frmPOILayer = new PopupPOILayer(connection, m_bimManager.POITypes);
                m_frmPOILayer.GDIOwner = frm;

                string projectName = "";
                int levelID = 0;
                if (frm != null)
                {
                    projectName = frm.Project.Name;
                    levelID = frm.Level.ID;
                }

                m_frmPOILayer.SetLayers(layers, projectName, levelID);
                //m_frmPOILayer.Layers = layers;
                m_frmPOILayer.FormClosed += (layerS, layerE) =>
                {
                    rbtnLayer.IsChecked = false;
                    rbtnLayer.Refresh();
                };
                m_frmPOILayer.StartPosition = FormStartPosition.CenterParent;
                m_frmPOILayer.Show(this);

                connection.Close();
            }
        }
#elif XML_USE
        private void ShowPOILayer(FormView frm, List<Shapes.Layer> layers)
        {
            m_frmPOILayer = new PopupPOILayer(m_currentProject, m_bimManager.POITypes);
            m_frmPOILayer.GDIOwner = frm;

            string projectName = "";
            int levelID = 0;
            if (frm != null)
            {
                projectName = frm.Project.Name;
                levelID = frm.Level.ID;
            }

            m_frmPOILayer.SetLayers(layers, projectName, levelID);
            //m_frmPOILayer.Layers = layers;
            m_frmPOILayer.FormClosed += (layerS, layerE) =>
            {
                rbtnLayer.IsChecked = false;
                rbtnLayer.Refresh();
            };
            m_frmPOILayer.StartPosition = FormStartPosition.CenterParent;
            m_frmPOILayer.Show(this);
        }
#endif

        private string GetLocalXMLResourceFolder()
        {
            string strKey = "XML_Resource";
            string strPath = ConfigurationManager.AppSettings.Get(strKey);

            if (strPath.StartsWith("/"))
            {
                int nIndex = Application.ExecutablePath.LastIndexOf('\\');

                if (nIndex < 0)
                    return strPath;

                string strFolder = Application.ExecutablePath.Substring(0, nIndex + 1) + strPath.Substring(1);
                return strFolder;
            }

            return strPath;
        }

        private Dictionary<string, bool> GetLayerDefaultVisible(string strKey)
        {
            Dictionary<string, bool> dicLayerVisible = new Dictionary<string, bool>();
            string strValue = ConfigurationManager.AppSettings.Get(strKey);

            if (strValue == null)
                return dicLayerVisible;

            string[] tokens = strValue.Split(',');

            foreach (string strToken in tokens)
            {
                int nIndex1 = strToken.IndexOf('(');
                int nIndex2 = strToken.IndexOf(')');

                if (nIndex1 > 0 && nIndex2 > nIndex1 + 1)
                {
                    string strLayer = strToken.Substring(0, nIndex1).Trim();
                    string strVisible = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

                    if (strVisible == "1")
                        dicLayerVisible[strLayer] = true;
                    else if (strVisible == "0")
                        dicLayerVisible[strLayer] = false;
                }
            }

            return dicLayerVisible;
        }

        private string GetConnectionString(out _SqlConnection.DataBaseType dbType)
        {
            dbType = _SqlConnection.DataBaseType.Unknown;

            string strKey = "BIMDBConnection";

            string strConnection = ConfigurationManager.ConnectionStrings[strKey].ConnectionString;
            string strProviderName = ConfigurationManager.ConnectionStrings[strKey].ProviderName;

            if (strProviderName == "System.Data.SqlClient")
                dbType = _SqlConnection.DataBaseType.SQLServer;
            else if (strProviderName == "System.Data.SQLite")
                dbType = _SqlConnection.DataBaseType.SQLite;

            return strConnection;
        }

        public void SetTabMode(Control ctrl, bool tabMode)
        {
            if (ctrl is FormView)
            {
                FormView form = (FormView)ctrl;

                if (tabMode == true)
                {
                    if (IsTabMode)
                    {
                        SelectTabView(form);
                        return;
                    }

                    List<FormView> forms = new List<FormView>();
                    int nControlCount = splitContainerMain.Panel2.Controls.Count;                    

                    for (int i = nControlCount - 1; i >= 0; i--)
                    {
                        Control control = splitContainerMain.Panel2.Controls[i];

                        if (control is FormView)
                        {
                            FormView frm = (FormView)control;
                            frm.SystemInput = true;

                            splitContainerMain.Panel2.Controls.RemoveAt(i);

                            // 정렬을 위해 임시로 forms에 저장한다.
                            forms.Add(frm);
                            //AddTabPage(frm);
                            frm.Show();
                        }
                    }

                    forms.Sort();

                    foreach (FormView frm in forms)
                    {
                        AddTabPage(frm);
                    }

                    if (forms.Count > 0)
                        m_basePlan.SetBtnAddEnabel(true);

                    forms.Clear();
                    ShowTabControl(true);
                    SelectTabView(form);

                    foreach (TabPage page in tabControl1.TabPages)
                    {
                        FormView frm = (FormView)page.Controls[0];
                        frm.SystemInput = false;
                    }
                }
                else
                {
                    if (IsTabMode == false)
                    {
                        form.FocusView();                        
                        return;
                    }

                    int nTabCount = tabControl1.TabCount;

                    for (int i = nTabCount - 1; i >= 0; i--)
                    {
                        TabPage tabPage = tabControl1.TabPages[i];
                        FormView frm = (FormView)tabPage.Controls[0];
                        frm.SystemInput = true;

                        tabPage.Controls.Clear();
                        tabControl1.TabPages.RemoveAt(i);

                        splitContainerMain.Panel2.Controls.Add(frm);
                        frm.FormBorderStyle = FormBorderStyle.Sizable;
                        frm.Dock = DockStyle.None;
                        frm.Size = FormView.InitSize;
                        frm.Show();
                    }

                    ShowTabControl(false);
                    form.FocusView();

                    foreach (Control control in splitContainerMain.Panel2.Controls)
                    {
                        if (control is FormView)
                        {
                            FormView frm = (FormView)control;
                            frm.SystemInput = false;
                        }
                    }
                }
            }
        }

        private void SelectTabView(FormView frm)
        {
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                FormView form = (FormView)tabPage.Controls[0];

                if (form == frm)
                {
                    tabControl1.SelectedTab = tabPage;
                    return;
                }
            }
        }

        private void tabControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int nTabCount = tabControl1.TabCount;

                for (int i = 0; i < nTabCount; i++)
                {
                    if (tabControl1.GetTabRect(i).Contains(e.Location))
                    {
                        TabPage page = tabControl1.TabPages[i];
                        FormView frm = (FormView)page.Controls[0];
                        SetTabMode(frm, false);
                        break;
                    }
                }
            }
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                TabPage page = tabControl1.TabPages[e.Index];
                TabPageData data = (TabPageData)page.Tag;

                //if (e.Index == 0 /*data.IsFirstDrawing()*/)
                //{
                //    Brush brush = new SolidBrush(m_clrTabBorder);
                //    e.Graphics.FillRectangle(brush, tabControl1.ClientRectangle);
                //    brush.Dispose();
                //}

                Brush brush1 = null, brush2 = null;
                Image image = null;

                if (e.Index == tabControl1.SelectedIndex)
                {
                    brush1 = new SolidBrush(m_clrTabPageBackSelected);
                    brush2 = new SolidBrush(m_clrTabPageForeSelected);
                    image = data.MouseOverCloseImage ? global::BIMViewer.Properties.Resources.closeTab_over : global::BIMViewer.Properties.Resources.closeTabSelected;
                }
                else
                {
                    brush1 = new SolidBrush(m_clrTabPageBackNormal);
                    brush2 = new SolidBrush(m_clrTabPageForeNormal);
                    image = data.MouseOverCloseImage ? global::BIMViewer.Properties.Resources.closeTab_over : global::BIMViewer.Properties.Resources.closeTabNormal;
                }

                Rectangle rect = tabControl1.GetTabRect(e.Index);
                e.Graphics.FillRectangle(brush1, rect);
                brush1.Dispose();

                e.Graphics.DrawString(page.Text, page.Font, brush2, rect.Left, rect.Top + 5);
                brush2.Dispose();

                int x = rect.Right - image.Width - 1;
                int y = rect.Top + 2;
                e.Graphics.DrawImage(image, x, y);

                data.SetTabPageRect(rect.Left + 10, rect.Top, rect.Width, rect.Height);
                data.SetCloseImageRect(x, y, image.Width, image.Height);
                //data.CompleteDrawing();
                //TabPageData.CheckComplete(tabControl1);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void tabControl1_MouseMove(object sender, MouseEventArgs e)
        {
            bool needRefresh = false;

            foreach (TabPage page in tabControl1.TabPages)
            {
                TabPageData data = (TabPageData)page.Tag;

                if (data.CheckMouseOver(e.X, e.Y))
                    needRefresh = true;
            }

            if (needRefresh)
                tabControl1.Refresh();
        }

        private void tabControl1_MouseLeave(object sender, EventArgs e)
        {
            bool needRefresh = false;

            foreach (TabPage page in tabControl1.TabPages)
            {
                TabPageData data = (TabPageData)page.Tag;

                if (data.MouseOverCloseImage || data.MouseOverTabPage)
                {
                    data.MouseOverTabPage = data.MouseOverCloseImage = false;
                    needRefresh = true;
                    break;
                }
            }

            if (needRefresh)
                tabControl1.Refresh();
        }

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                foreach (TabPage page in tabControl1.TabPages)
                {
                    TabPageData data = (TabPageData)page.Tag;
                    data.CheckMouseOver(e.X, e.Y);

                    if (data.MouseOverCloseImage)
                    {
                        TabPage prevSelectedPage = m_prevSelectedPage;
                        RemoveTabPage(page);

                        if (prevSelectedPage != page && prevSelectedPage != null)
                            tabControl1.SelectedTab = prevSelectedPage;

                        break;
                    }
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_prevSelectedPage = m_currentSelectedPage;
            m_currentSelectedPage = tabControl1.SelectedTab;

            if (m_currentSelectedPage != null)
            {
                FormView frm = m_currentSelectedPage.Controls[0] as FormView;
                BIM.Level level = frm.Level;
                BIM.Project project = frm.Project;

                if (m_basePlan != null && !m_basePlan.IsDisposed)
                {
                    m_basePlan.GDIOwner = frm;
                    m_basePlan.SetLayers(level.GetLayers(), level.GetDXFLayers());
                    m_basePlan.SetBtnAddEnabel(true);
                }

                if (m_frmProperty != null && !m_frmProperty.IsDisposed)
                {
                    m_frmProperty.SelectedShape = frm.GetSelectedShape();
                }

                if (m_frmLayer != null)
                {
                    m_frmLayer.GDIOwner = frm;
                    m_frmLayer.SetLayers(level.GetLayers(), level.GetDXFLayers());
                }
                //ym
                this.OnSelectProject(project);
                m_viewProject.SetSelectProject(project);
                m_viewLevel.SetSelectLevel(level);
            }
        }

        public void ShowShapeProperty(Shapes.Shape shape)
        {
            if (m_frmProperty == null || m_frmProperty.IsDisposed)
            {
                if (shape == null)
                    return;
                else
                {
                    m_frmProperty = new FormProperty(this);
                    m_frmProperty.SelectedShape = shape;
                    // 0717ym
                   /* if (m_frmProperty.OpenSucFrm)
                         m_frmProperty.Show();
                    else
                    {
                        m_frmProperty = null;
                    }*/
                }
            }
            else
            {
                if (shape == null)
                {
                    m_frmProperty.SelectedShape = null;
                    m_frmProperty.Close();
                }
                else
                {
                    m_frmProperty.SelectedShape = shape;
                }
            }

            //ym
           
            if (shape == null || !(rbtnProperty.IsChecked))
                return;

            if (shape is POI) return;//poi편집불가

            if (shape is Wall)
            {
                Wall wall = (Wall)shape;
                if (wall.Component.TypeName == "Partition")
                    return;
            }

            string strLevelName = GetLevelName();

            pnlProperty.Controls.Clear();
            pnlProperty.Location = new Point(rbtnProperty.Location.X + rbtnProperty.Width, rbtnProperty.Location.Y);
            pnlProperty.Height = rbtnProperty.Height;
            if (shape is Wall)
            {
                Wall wall = (Wall)shape;
                if(wall.Component.TypeName == "Structure")
                {
                    m_selShapePropType = SelShapePropType.Swall;
                    pnlProperty.Width = m_uSwall.Size.Width;
                    pnlProperty.Controls.Add(m_uSwall);
                    m_uSwall.Dock = DockStyle.Fill;

                    m_uSwall.SetSwallData((Wall)shape, strLevelName);
                }
                else if (wall.Component.TypeName == "Fake")
                {
                    m_selShapePropType = SelShapePropType.Fwall;
                    pnlProperty.Width = m_uFwall.Size.Width;
                    pnlProperty.Controls.Add(m_uFwall);
                    m_uFwall.Dock = DockStyle.Fill;

                    m_uFwall.SetFwallData((Wall)shape, strLevelName);
                }
                if (wall.Component.TypeName == "CurtainWall")
                {
                    // 커튼월 정보
                    m_selShapePropType = SelShapePropType.Cwall;
                    pnlProperty.Width = m_uCwall.Size.Width;
                    pnlProperty.Controls.Add(m_uCwall);
                    m_uCwall.Dock = DockStyle.Fill;

                    m_uCwall.SetSwallData((Wall)shape, strLevelName);
                }
                else //Handrail
                {
                    m_selShapePropType = SelShapePropType.Hwall;
                    pnlProperty.Width = m_uHwall.Size.Width;
                    pnlProperty.Controls.Add(m_uHwall);
                    m_uHwall.Dock = DockStyle.Fill;

                    m_uHwall.SetHwallData((Wall)shape, strLevelName);
                }
            }
            else if (shape is Door)
            {
                m_selShapePropType = SelShapePropType.Door;
                pnlProperty.Width = m_uDoor.Size.Width;
                pnlProperty.Controls.Add(m_uDoor);
                m_uDoor.Dock = DockStyle.Fill;

                m_uDoor.SetDoorData((Door)shape, strLevelName);
            }
            else if (shape is Window)
            {
                m_selShapePropType = SelShapePropType.Window;
                pnlProperty.Width = m_uWindow.Size.Width;
                pnlProperty.Controls.Add(m_uWindow);
                m_uWindow.Dock = DockStyle.Fill;
                               
                m_uWindow.SetWindowData((Window)shape, strLevelName);
            }
            else if (shape is Space)
            {
                // 공간 속성 창 열리는 부분
                m_selShapePropType = SelShapePropType.Space;
                pnlProperty.Width = m_uSpace.Size.Width;
                pnlProperty.Controls.Add(m_uSpace);
                m_uSpace.Dock = DockStyle.Fill;

                FormView frm = null;
                BIM.Level level = GetCurrentLevel(out frm);

                m_uSpace.SetSpaceData((Space)shape, strLevelName, level);
            }
            else if (shape is AlertArea)
            {
                // 경계구역 속성 창 열리는 부분
                m_selShapePropType = SelShapePropType.AlertArea;
                pnlProperty.Width = m_uAlertArea.Size.Width;
                pnlProperty.Controls.Add(m_uAlertArea);
                m_uAlertArea.Dock = DockStyle.Fill;

                FormView frm = null;
                BIM.Level level = GetCurrentLevel(out frm);

                m_uAlertArea.SetAlertAreaData((AlertArea)shape, strLevelName, level);
            }
            else if (shape is Column)//기둥은 속성편집안함.
            {
                string sHeight = "";
                FormView frm = null;
                BIM.Level level = GetCurrentLevel(out frm);
                foreach (Property prop in level.Properties)
                {
                    if (prop.Name == "Height")
                        sHeight = prop.Value;
                }

                Column column = (Column)shape;
                if(column.Type == 0)
                {
                    m_selShapePropType = SelShapePropType.Rect;
                    pnlProperty.Width = m_uRect.Size.Width;
                    pnlProperty.Controls.Add(m_uRect);
                    m_uRect.Dock = DockStyle.Fill;

                    m_uRect.ShowRectData((Column)shape, sHeight, strLevelName);
                }
                else
                {
                    m_selShapePropType = SelShapePropType.Circle;
                    pnlProperty.Width = m_uCircle.Size.Width;
                    pnlProperty.Controls.Add(m_uCircle);
                    m_uCircle.Dock = DockStyle.Fill;

                    m_uCircle.ShowCircleData((Column)shape, sHeight, strLevelName);
                }          
            }
         
            rbtnPropertyDone.Height = pnlProperty.Height;
            rbtnPropertyDone.Location = new Point(pnlProperty.Location.X + pnlProperty.Width, pnlProperty.Location.Y);

            m_PropertyProject = m_currentProject;
            pnlProperty.Visible = true;
            rbtnPropertyDone.Visible = true;
            pnlProperty.Show();
            rbtnPropertyDone.Show();

            m_bPanelPropertySizeFull = true;
            m_timerPanelProperty.Enabled = true;
        }
        //ym
        private string GetLevelName()
        {
            FormView frm = null;
            BIM.Level level = GetCurrentLevel(out frm);

            string strFrom = level.Name;
            string strName = "";
            int i, j;

            if (strFrom.Contains("Roof"))
                return "Roof";
    
            i = strFrom.IndexOf("B");// B1F  -> B 01, 1F -> F 01
            j = strFrom.IndexOf("F");
            if(i >= 0)
            {
                strFrom = strFrom.Substring(i+1, j-1);
                strName = strFrom.Length > 1 ? string.Format("B {0}", strFrom) : string.Format("B 0{0}", strFrom);
            }
            else
            {
                strFrom = strFrom.Substring(0, j);
                strName = strFrom.Length > 1 ? string.Format("F {0}", strFrom) : string.Format("F 0{0}", strFrom);
            }
            return strName;
        }
        private double CalcColumnHeight()
        {
            FormView frm = null;
            BIM.Level level = GetCurrentLevel(out frm);

            double dHeight = 0.0;

            if (level.Name == "Roof plan") 
                return dHeight;//꼭대기층은 기둥없음.

            int tmpIdx = 0;
            foreach (Level tmpLevel in m_currentProject.Levels)
            {
                if (level == tmpLevel)                
                    dHeight = m_currentProject.Levels[tmpIdx + 1].Elevation - m_currentProject.Levels[tmpIdx].Elevation;
                
                tmpIdx++;
                if (tmpIdx == m_currentProject.Levels.Count -1)
                    break;
            }

            return dHeight;
        }

        private void RbtnPropertyDone_Click(object sender, EventArgs e)
        {
            //사용자편집 속성. 현재 프로젝트에(메모리에만) 반영. ym         
            switch (m_selShapePropType)
            {
                case SelShapePropType.Swall:
                    m_uSwall.UpdateUserData();
                    break;
                case SelShapePropType.Fwall:
                    m_uFwall.UpdateUserData();
                    break;
                case SelShapePropType.Hwall:
                    m_uHwall.UpdateUserData();
                    break;
                case SelShapePropType.Door:
                    m_uDoor.UpdateUserData();
                    break;
                case SelShapePropType.Window:
                    m_uWindow.UpdateUserData();
                    break;
                case SelShapePropType.Space:
                    m_uSpace.UpdateUserData();
                    break;
                case SelShapePropType.AlertArea:
                    m_uAlertArea.UpdateUserData();
                    break;
            }

            //속성창 안보이게
            //pnlProperty.Visible = false;
            //rbtnPropertyDone.Visible = false;
            //pnlProperty.Hide();
            //rbtnPropertyDone.Hide();
            m_bPanelPropertySizeFull = false;
            m_timerPanelProperty.Enabled = true;
        }

        private void RbtnBuildingDone_Click(object sender, EventArgs e)
        {
            m_uBuilding.UpdateUserData();

            m_bPanelBuildingSizeFull = false;
            m_timerPanelBuilding.Enabled = true;
        }

        public bool SaveShapeProperty(Shapes.Shape shape)
        {
            if (shape != null)
            {
                if (shape is BIM.Space)
                {
                    BIM.Space space = (BIM.Space)shape;
                    return SaveSpaceProperty(space);
                }
            }

            return false;
        }

        private void rbtnAdd_Click(object sender, EventArgs e)
        {
            rbtnAdd.IsChecked = !rbtnAdd.IsChecked;
            if (rbtnAdd.IsChecked)
            {
                rbtnMove.IsChecked = false;
                rbtnMove.Refresh();
                rbtnDelete.IsChecked = false;
                rbtnDelete.Refresh();
            }
        }

        private void rbtnAddLine_Click(object sender, EventArgs e)
        {
            rbtnAddLine.IsChecked = !rbtnAddLine.IsChecked;
            if (rbtnAddLine.IsChecked)
            {
                rbtnMoveLine.IsChecked = false;
                rbtnMoveLine.Refresh();
                rbtnDeleteLine.IsChecked = false;
                rbtnDeleteLine.Refresh();
            }
        }

        private void rbtnProperty_Click(object sender, EventArgs e)
        {
            rbtnProperty.IsChecked = !rbtnProperty.IsChecked;

            if (!rbtnProperty.IsChecked)
            {
                if (m_frmProperty != null && !m_frmProperty.IsDisposed)
                {
                    m_frmProperty.SelectedShape = null;
                    m_frmProperty.Close();
                }

                //ym
                //if(pnlProperty.Visible)
                //{
                //    pnlProperty.Visible = false;
                //    rbtnPropertyDone.Visible = false;
                //    pnlProperty.Hide();
                //    rbtnPropertyDone.Hide();
                //}
                m_bPanelPropertySizeFull = false;
                m_timerPanelProperty.Enabled = true;
            }
            else
            {
                rbtnBuilding.IsChecked = false;
                rbtnBuilding.Refresh();
                m_bPanelBuildingSizeFull = false;
                m_timerPanelBuilding.Enabled = true;
            }
        }

        private bool CheckPropertyID(string strTableName, string strFieldName, int nID, string strPropertyName, _SqlConnection connection)
        {
            string strSQL = string.Format("Select {0} from {1} where {0} = {2} and PropertyName = '{3}'", strFieldName, strTableName, nID, strPropertyName);
            _SqlDataReader reader = m_bimManager.ReadQuery(strSQL, connection, null);

            if (reader != null)
            {
                if (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                    {
                        reader.Close();
                        return true;
                    }
                }

                reader.Close();
            }

            return false;
        }

        private bool SaveSpaceProperty(BIM.Space space)
        {
#if DB_USE
            return SaveSpacePropertyToDB(space);
#elif XML_USE
            return SaveSpacePropertyToXML(space);
#endif
        }

#if DB_USE
        private bool SaveSpacePropertyToDB(BIM.Space space)
        {
            bool result = false;
            _SqlConnection.DataBaseType dbType;
            string strConnection = GetConnectionString(out dbType);

            _SqlConnection connection = new _SqlConnection(dbType, strConnection);
            {
                connection.Open();

                if (space.SafetyFire)
                {
                    string strSQL = string.Format("Update SpaceProperty set PropertyValue = 'false' where SpaceID = {0} and PropertyName = '{1}'",
                        space.ID, BIM.Space.SafetyFireTag);
                    result = m_bimManager.ExecuteQuery(strSQL, connection, null);
                }
                else
                {
                    if (CheckPropertyID("SpaceProperty", "SpaceID", space.ID, BIM.Space.SafetyFireTag, connection))
                    {
                        string strSQL = string.Format("Update SpaceProperty set PropertyValue = 'true' where SpaceID = {0} and PropertyName = '{1}'",
                            space.ID, BIM.Space.SafetyFireTag);
                        result = m_bimManager.ExecuteQuery(strSQL, connection, null);
                    }
                    else
                    {
                        string strSQL = string.Format("Insert into SpaceProperty (SpaceID, PropertyName, PropertyValue, LevelID, Description) values ({0}, '{1}', 'true', {2}, NULL)",
                            space.ID, BIM.Space.SafetyFireTag, space.Level.ID);
                        result = m_bimManager.ExecuteQuery(strSQL, connection, null);
                    }
                }

                connection.Close();
            }

            return result;
        }
#elif XML_USE
        private bool SaveSpacePropertyToXML(BIM.Space space)
        {
            XMLManager mgr = new XMLManager();
            return mgr.Save(m_currentProject, m_bimManager.POITypes);
        }
#endif

        private void rbtnLine_Click(object sender, EventArgs e)
        {
            rbtnLine.IsChecked = !rbtnLine.IsChecked;

            if (rbtnLine.IsChecked)
            {
                //POI 패널 닫기
                rbtnPOI.IsChecked = rbtnAdd.IsChecked = rbtnMove.IsChecked = rbtnDelete.IsChecked = rbtnDone.IsChecked = false;
                rbtnPOI.Refresh();
                m_bPanelPOISizeFull = false;
                m_timerPanelPOI.Enabled = true;

                m_bPanelLineSizeFull = true;
                m_timerPanelLine.Enabled = true;
            }
            else
            {
                m_bPanelLineSizeFull = false;
                m_timerPanelLine.Enabled = true;

                rbtnAddLine.IsChecked = rbtnMoveLine.IsChecked = rbtnDeleteLine.IsChecked = false;
            }
        }
        private void M_timerPanelLine_Tick(object sender, EventArgs e)
        {
            int maxWidth = 700;
            int minWidth = 70;
            int gap = 70;

            if (m_bPanelLineSizeFull)
            {
                if (panelLine.Width == maxWidth)
                    m_timerPanelLine.Enabled = false;
                else
                {
                    if (panelLine.Width + gap > maxWidth)
                        panelLine.Width += maxWidth - panelLine.Width;
                    else
                        panelLine.Width += gap;
                }
            }
            else
            {
                if (panelLine.Width <= minWidth)
                    m_timerPanelLine.Enabled = false;
                else
                {
                    if (panelLine.Width - gap < minWidth)
                        panelLine.Width -= minWidth - panelLine.Width;
                    else
                        panelLine.Width -= gap;                    
                }
            }
        }

        private void M_timerPanelBuilding_Tick(object sender, EventArgs e)
        {
            int maxWidth = 560;
            int minWidth = 70;
            int gap = 70;

            if (m_bPanelBuildingSizeFull)
            {
                if (panelBuilding.Width == maxWidth)
                    m_timerPanelBuilding.Enabled = false;
                else
                {
                    if (panelBuilding.Width + gap > maxWidth)
                        panelBuilding.Width += maxWidth - panelBuilding.Width;
                    else
                        panelBuilding.Width += gap;

                    panelProperty.Location = new Point(panelBuilding.Location.X + panelBuilding.Width, panelBuilding.Location.Y);
                }
            }
            else
            {
                if (panelBuilding.Width <= minWidth)
                    m_timerPanelBuilding.Enabled = false;
                else
                {
                    if (panelBuilding.Width - gap < minWidth)
                        panelBuilding.Width -= minWidth - panelBuilding.Width;
                    else
                        panelBuilding.Width -= gap;

                    panelProperty.Location = new Point(panelBuilding.Location.X + panelBuilding.Width, panelBuilding.Location.Y);
                }
            }
        }

        private void M_timerPanelProperty_Tick(object sender, EventArgs e)
        {
            int maxWidth = 980;
            int minWidth = 70;
            int gap = 70;

            if (m_bPanelPropertySizeFull)
            {
                if (panelProperty.Width == maxWidth)
                    m_timerPanelProperty.Enabled = false;
                else
                {
                    if (panelProperty.Width + gap > maxWidth)
                        panelProperty.Width += maxWidth - panelProperty.Width;
                    else
                        panelProperty.Width += gap;
                }
            }
            else
            {
                if (panelProperty.Width <= minWidth)
                    m_timerPanelProperty.Enabled = false;
                else
                {
                    if (panelProperty.Width - gap < minWidth)
                        panelProperty.Width -= minWidth - panelProperty.Width;
                    else
                        panelProperty.Width -= gap;
                }
            }
        }

        private void rbtnMoveLine_Click(object sender, EventArgs e)
        {
            rbtnMoveLine.IsChecked = !rbtnMoveLine.IsChecked;
            if (rbtnMoveLine.IsChecked)
            {
                rbtnAddLine.IsChecked = false;
                rbtnAddLine.Refresh();
                rbtnDeleteLine.IsChecked = false;
                rbtnDeleteLine.Refresh();
            }
        }

        private void FormMain_ClientSizeChanged(object sender, EventArgs e)
        {
            if (m_lastState != this.WindowState)
            {
                m_lastState = this.WindowState;

                FormMain_ResizeEnd(null, null);
            }
        }

        private void rbtnDeleteLine_Click(object sender, EventArgs e)
        {
            rbtnDeleteLine.IsChecked = !rbtnDeleteLine.IsChecked;
            if (rbtnDeleteLine.IsChecked)
            {
                rbtnAddLine.IsChecked = false;
                rbtnAddLine.Refresh();
                rbtnMoveLine.IsChecked = false;
                rbtnMoveLine.Refresh();
            }
        }

        private void rbtnDoneLine_Click(object sender, EventArgs e)
        {
            m_bPanelLineSizeFull = false;
            m_timerPanelLine.Enabled = true;

            rbtnLine.IsChecked = rbtnAddLine.IsChecked = rbtnMoveLine.IsChecked = rbtnDeleteLine.IsChecked = false;
            rbtnLine.Refresh();
        }
        
        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_bimManager.SavePOITypeVisible();
            m_bimManager.SavePOITypeColor();
        }
        private void ribbonButton1_Click(object sender, EventArgs e)
        {
            if (m_frmPOI == null || m_frmPOI.IsDisposed)
            {
                m_frmPOI = new FormPOI(m_bimManager.POITypes);
                m_frmPOI.Show(this);
                m_frmPOI.FormClosed += (layerS, layerE) =>
                {
                    rbtnPOI.IsChecked = false;
                };
                /*_SqlConnection.DataBaseType dbType;
                string strConnection = GetConnectionString(out dbType);

                _SqlConnection connection = new _SqlConnection(dbType, strConnection);
                {
                    connection.Open();

                    m_frmPOI = new FormPOI(connection, m_bimManager.POITypes);
                    m_frmPOI.Show(this);
                    m_frmPOI.FormClosed += (layerS, layerE) =>
                    {
                        rbtnPOI.IsChecked = false;
                    };
                    connection.Close();
                }*/

            }
        }
        /*
        private void rbtnUpload_Click(object sender, EventArgs e)
        {
            if (m_currentProject == null)
                  return;

            if (!m_bLogin)
                DoLogin();

            if (!m_bLogin)
                return;


            FormLocation fLocation = new FormLocation(m_strLoginID, m_strLoginPW, m_spaceUserList);
            if (fLocation.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (fLocation.m_sSelBuildMngNo != "")
                {
                    this.Cursor = Cursors.WaitCursor;
                    WebServiceManager mgr = new WebServiceManager();
                    if (mgr.Upload(m_currentProject, m_bimManager.POITypes, fLocation.m_sSelBuildMngNo, m_strLoginID, m_strLoginPW))
                        MessageBox.Show("Upload 완료!");
                    else
                        MessageBox.Show("Upload 실패");
                    this.Cursor = Cursors.Arrow;
                }
            }
        }*/

        private void rbtnUpload_Click(object sender, EventArgs e)
        {
            if (m_currentProject == null)
                return;

            if (!m_bLogin)
                DoLogin();

            if (!m_bLogin)
                return;

            FormLocation fLocation = new FormLocation(m_strLoginID, m_strLoginPW, m_spaceUserList);
            fLocation.StartPosition = FormStartPosition.CenterParent;
            if (fLocation.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (fLocation.m_sSelBuildMngNo != "")
                {
                    m_webServiceMgr.Upload(m_currentProject, m_bimManager.POITypes, fLocation.m_sSelBuildMngNo, m_strLoginID, m_strLoginPW);
                }
            }
        }

        private void rbtnDownload_Click(object sender, EventArgs e)
        {
            if (!m_bLogin)
                DoLogin();

            if (!m_bLogin)
                return;

            //건물위치
            FormLocation fLocation = new FormLocation(m_strLoginID, m_strLoginPW, m_spaceUserList);

            if (fLocation.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            //저장위치
            string strFileName = "";
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "XML Files|*.xml|All FIles|*.*";
            dialog.FilterIndex = 0;
            dialog.Title = "XML 저장";
            dialog.FileName = fLocation.m_sSelAddress + ("(DownLoad)");

            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                strFileName = dialog.FileName;
            else
                return;

            strFileName = strFileName.Trim();
            if (fLocation.m_sSelBuildMngNo != "")
            {
                m_webServiceMgr.Download(m_currentProject, strFileName, fLocation.m_sSelBuildMngNo, m_strLoginID, m_strLoginPW);
            }

            //추가.0916.ym
            //down후에 실행경로에 있는 id_code.xml의 poiTypes 를 읽어서, down받은 파일의 poitypes에 쓴다.
            //서버 poitypes와 로컬 poitypes 맞을때까지 이렇게
           // XMLManager mgr1 = new XMLManager();
           // XMLManager mgr2 = new XMLManager();
           // Dictionary<int, POIType> dicPOITypes1 = new Dictionary<int, POIType>();
           // Dictionary<int, POIType> dicPOITypes2 = new Dictionary<int, POIType>();
           // Project project1 = new Project();
           // Project project2 = new Project();

           //string strXMLFileDown = strFileName;
           // project1 = mgr1.ReadProject(strXMLFileDown, dicPOITypes1);
           // project1.LocalFilePath = strXMLFileDown;
           // bool b = mgr1.ReadLevels(project1, dicPOITypes1);            

           // string strXMLFilePOITypes = Application.StartupPath + "\\poiTypes.xml";
           // project2 = mgr2.ReadProject(strXMLFilePOITypes, dicPOITypes2);

           // bool result = mgr1.Save(project1, dicPOITypes2);
        }

        /*
private void rbtnDownload_Click(object sender, EventArgs e)
{
   if (!m_bLogin)
       DoLogin();

   if (!m_bLogin)
       return;

   //건물위치
   FormLocation fLocation = new FormLocation(m_strLoginID, m_strLoginPW, m_spaceUserList);
   if (fLocation.ShowDialog() != System.Windows.Forms.DialogResult.OK)
       return;

   //저장위치
   SaveFileDialog dialog = new SaveFileDialog();

   dialog.Filter = "XML Files|*.xml|All FIles|*.*";
   dialog.FilterIndex = 0;
   dialog.Title = "XML 저장";
   dialog.FileName = fLocation.m_sSelAddress + ("(DownLoad)");
   string strFileName = "";

   if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
       strFileName = dialog.FileName;
   else
       return;

   strFileName = strFileName.Trim();

   if (fLocation.m_sSelBuildMngNo != "")
   {
       this.Cursor = Cursors.WaitCursor;
       WebServiceManager mgr = new WebServiceManager();
       if (mgr.Download(m_currentProject, strFileName, fLocation.m_sSelBuildMngNo, m_strLoginID, m_strLoginPW))
       {
           MessageBox.Show(strFileName, "Download 완료!");
       }
       else
       {
           MessageBox.Show("Download 실패", "Download 실패");
       }

       this.Cursor = Cursors.Arrow;
   }
}*/

        private void DoLogin()
        {
            FormLogin fLogin = new FormLogin(m_spaceUserList);
            fLogin.StartPosition = FormStartPosition.CenterParent;
            if (fLogin.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (fLogin.m_bResult)
                {
                    clblLogin.Text = "LOGOUT";
                    m_bLogin = true;
                    m_strLoginID = fLogin.m_sID;
                    m_strLoginPW = fLogin.m_sPW;
                }
            }
        }
        private void ClblLogin_Click(object sender, EventArgs e)
        {
            if (!m_bLogin)
                DoLogin();
            else
            {
                if (MessageBox.Show("로그아웃 하시겠습니까?", "로그아웃", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    clblLogin.Text = "LOGIN";
                    m_bLogin = false;
                    m_strLoginID = "";
                    m_strLoginPW = "";
                }
            }

        }
        public void SaveLocalPathXML()
        {
            pnlSave.Visible = false;
            pnlSave.Hide();

            string strMessage = m_currentProject.LocalFilePath + " 정보를 수정하시겠습니까?";

            uSaveMessage sMessage = new uSaveMessage(strMessage);
            sMessage.StartPosition = FormStartPosition.CenterScreen;
            if (sMessage.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XMLManager mgr = new XMLManager();
                bool result = mgr.Save(m_currentProject, m_bimManager.POITypes);

                this.Cursor = Cursors.Arrow;

                if (result == false)
                    MessageBox.Show(mgr.ErrorMessage);
                else
                    MessageBox.Show("파일이 수정되었습니다.");
            }
        }
        public void SaveAsLocalPathXML()
        {
            pnlSave.Visible = false;
            pnlSave.Hide();

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "XML Files|*.xml|All FIles|*.*";
            dialog.FilterIndex = 0;
            dialog.Title = "XML 저장";
            string strFileName = "";

            if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                strFileName = dialog.FileName;
            else
                return;

            strFileName = strFileName.Trim();
            m_currentProject.LocalFilePath = strFileName;

            XMLManager mgr = new XMLManager();
            bool result = mgr.Save(m_currentProject, m_bimManager.POITypes);

            this.Cursor = Cursors.Arrow;

            if (result == false)
                MessageBox.Show(mgr.ErrorMessage);
            else
                MessageBox.Show("파일이 생성되었습니다.");
        }
        public void HidePnlSave()
        {
            pnlSave.Visible = false;
            pnlSave.Hide();
        }
        private void rbtnSave_Click(object sender, EventArgs e)
        {
            if (m_currentProject == null)
                     return;
            pnlSave.BringToFront();
            pnlSave.Visible = true;
            pnlSave.Show();           
        }

        private void SetWindowMaxButton()
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.btnMax.Image = global::BIMViewer.Properties.Resources.Windowmulti_Base;
                this.btnMax.ImageClicked = global::BIMViewer.Properties.Resources.Windowmulti_1st_MSover;
                this.btnMax.ImageDisabled = null;
                this.btnMax.ImageMouseOver = global::BIMViewer.Properties.Resources.Windowmulti_1st_MSover;
                this.btnMax.ImageNormal = global::BIMViewer.Properties.Resources.Windowmulti_Base;
            }
            else
            {
                this.btnMax.Image = global::BIMViewer.Properties.Resources.WindowMax_Base;
                this.btnMax.ImageClicked = global::BIMViewer.Properties.Resources.Windowmax_1st_MSover;
                this.btnMax.ImageDisabled = null;
                this.btnMax.ImageMouseOver = global::BIMViewer.Properties.Resources.Windowmax_1st_MSover;
                this.btnMax.ImageNormal = global::BIMViewer.Properties.Resources.WindowMax_Base;
            }
            this.btnMax.Refresh();
        }

        private void rbtnBuilding_Click(object sender, EventArgs e)
        {
            rbtnBuilding.IsChecked = !rbtnBuilding.IsChecked;

            if (rbtnBuilding.IsChecked)
            {
                m_uBuilding.SetBuildingData(m_currentProject);
                m_bPanelBuildingSizeFull = true;
                m_timerPanelBuilding.Enabled = true;

                rbtnProperty.IsChecked = false;
                rbtnProperty.Refresh();
                m_bPanelPropertySizeFull = false;
                m_timerPanelProperty.Enabled = true;
            }
            else
            {
                m_bPanelBuildingSizeFull = false;
                m_timerPanelBuilding.Enabled = true;
            }
        }

        private void rbtnFormLayer_Click(object sender, EventArgs e)
        {
            FormView frm = null;
            BIM.Level level = GetCurrentLevel(out frm);
            List<Shapes.Layer> layers = level == null ? null : level.GetLayers();
            List<DXFLayer> dxfLayers = level == null ? null : level.GetDXFLayers();

            if (m_frmLayer == null || m_frmLayer.IsDisposed)
            {
                m_frmLayer = new FormLayer();
                m_frmLayer.GDIOwner = frm;
                m_frmLayer.SetLayers(layers, dxfLayers);
                m_frmLayer.Show(this);
            }
            else
                m_frmLayer.Focus();
        }                

        private void btnMin_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)                            
                this.WindowState = FormWindowState.Normal;
            else if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;

            SetWindowMaxButton();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public BIM.Project GetProject(int projectID)
        {
            return m_viewProject.GetProject(projectID);
        }

        public BIM.Level GetLevel(int levelID)
        {
            return m_viewLevel.GetLevel(levelID);
        }

        public void RefreshAll()
        {
            foreach (Control ctrl in splitContainerMain.Panel2.Controls)
            {
                if (ctrl is FormView)
                {
                    FormView frm = (FormView)ctrl;
                    frm.RefreshView();
                }
                else if (ctrl is TabControl)
                {
                    TabControl tabCtrl = (TabControl)ctrl;

                    foreach (TabPage page in tabCtrl.TabPages)
                    {
                        foreach (Control c in page.Controls)
                        {
                            if (c is FormView)
                            {
                                FormView frm = (FormView)c;
                                frm.RefreshView();
                            }
                        }
                    }
                }
            }
        }

        public void ReloadProject()
        {   // XML 노아 서버에 업로드 후 XML 다운받아 다시 불러오기 작업

            BIM.Project currentProject = m_currentProject;
            string strFilePath = m_currentProject.LocalFilePath;

            List<Control> listCtrl = new List<Control>();

            //열린 층.도면폼들 닫기
            if (!IsTabMode)
            {
                foreach (Control ctrl in splitContainerMain.Panel2.Controls)
                {
                    if (ctrl is FormView)
                        listCtrl.Add(ctrl);
                }

                foreach (Control ctrl in listCtrl)
                {
                    splitContainerMain.Panel2.Controls.Remove(ctrl);
                }
            }
            else
            {
                foreach (TabPage page in tabControl1.TabPages)
                {
                    RemoveTabPage(page);
                }
            }
               

            BIM.Project project = m_bimManager.GetProject(strFilePath);
            if (project != null)
            {
                m_viewProject.ReloadProject(project);
                OnSelectProject(project);
            }
        }
    }

    public interface IUIMaster
    {
        bool IsAddMode
        {
            get;
        }

        bool IsMoveMode
        {
            get;
        }

        bool IsDeleteMode
        {
            get;
        }

        bool IsDoneMode
        {
            get;
        }

        bool IsAddModeWire
        {
            get;
        }

        bool IsMoveModeWire
        {
            get;
        }

        bool IsDeleteModeWire
        {
            get;
        }

        bool IsDoneModeWire
        {
            get;
        }

        bool IsPropertyMode
        {
            get;
        }
        
#if DB_USE
        bool UpdatePOIToDB(Shapes.POI poi, BIM.Level level, bool isDelete);
        bool UpdateWireToDB(Wire wire, bool isDelete);
#elif XML_USE
        bool UpdatePOIToXML(Shapes.POI poi, BIM.Project project, BIM.Level level, bool isDelete);
        bool UpdateWireToXML(Shapes.Wire wire, BIM.Project project, BIM.Level level, bool isDelete);
#endif
        void SetTabMode(Control ctrl, bool tabMode);
        void ShowShapeProperty(Shapes.Shape shape);
        bool SaveShapeProperty(Shapes.Shape shape);
        POIType SelectedPOI { get; }
        POITypeProperty SelectedWire { get; }

        //ym.base plan. 도면폼 닫힐때, 띄워진,그리드 목록 지우기
        void RemoveBasePlanGrid();
    }
}