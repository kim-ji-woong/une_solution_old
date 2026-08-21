using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SoilMan
{
    public enum SoilFunctionType
    {
        오염물질정화 = 0,
        수자원저수,
        대기정화_CO2,
        대기정화_O2,
        대기냉각,
        수질정화,
        식물생산기능,
        구조물지지기능,
        원료공급기능,
        유산가치,
        존재가치,
        생태학적가치,
        TypeCount
    }

    public enum LandType { General = 0, Field, RiceField, Mountain, Unknown };
    public enum TechType { None = 0, Bio, Farm, Steam, Washing, Oxidation, Heat, Count };

    public enum WTPType {  대수선형로짓트_중앙 = 0, 대수선형로싯트_WTP, Weibull_중앙, Weibull_WTP, None};

    public partial class FormMain : Form, Drawing.IShapeAttrib, UnE.GUI.IRibbonButtonOwner
    {
        public enum DockingType { NONE = 0, ALL_LAYER = 1, 수치지도_LAYER = 2, 토지이용계획도_LAYER = 4 };

        private DockingForm.FormDetailLayer m_frm수치지도Layer = null;
        private DockingForm.FormDetailLayer m_frm토지이용계획도Layer = null;
        private DockingForm.FormLayer m_frmLayer = new DockingForm.FormLayer();

        private Popup.FormCalcCondition m_frmCalcCondition = null;
        private Popup.FormResult m_frmResult = new Popup.FormResult();

        private string m_str수치지도Path = "";
        private string m_str토지이용계획도Path = "";
        private string m_str지적도Path = "";
        private DXFViewer.Layer m_layer지적도 = null;
        public DXFViewer.Layer Layer지적도
        {
            get { return m_layer지적도; }
            set { m_layer지적도 = value; }
        }

        private UnE.Geometry.Vertex2D m_BoundShapeTL = null;
        public UnE.Geometry.Vertex2D BoundShapeTL
        {
            get { return m_BoundShapeTL; }
            set { m_BoundShapeTL = value; }
        }

        private UnE.Geometry.Vertex2D m_BoundShapeBR = null;
        public UnE.Geometry.Vertex2D BoundShapeBR
        {
            get { return m_BoundShapeBR; }
            set { m_BoundShapeBR = value; }
        }


        private DXFViewer.DXFControl m_dxfControl = null;

        public DXFViewer.DXFControl DxfControl
        {
            get { return m_dxfControl; }
            set { m_dxfControl = value; }
        }
        //private UnE.Geometry.Vertex2D m_vDXFTL = null;
        //private UnE.Geometry.Vertex2D m_vDXFBR = null;

        //private UnE.Geometry.Vertex2D m_prevMovedVertex = null;
        private int m_nDockingMode = 0;
        private int m_nSplitDistance = 150;

        UnE.Geometry.Vertex2D m_vScreenCenter = null;

        private SelectionManager m_selectionMgr = null;
        public SelectionManager SelectionManager
        {
            get 
            {
                if (m_selectionMgr == null)
                    m_selectionMgr = new SelectionManager();
                return m_selectionMgr; 
            }            
        }

        private bool m_noShapeFileDrawing = false;

        private QuadTree m_quadTree = null;
        private bool m_closeApplication = false;

        // PNU 코드별 공시지가
        private Dictionary<string, double> m_dicCodeCost = null;
        // 법정동 코드별 주소
        private Dictionary<string, string> m_dicCodeAddr = null;
        private DXFViewer.Layer m_shapeFilePolygonLayer = null;

        private string m_strProjectPath = "";

        TabPages.Page계량지표 m_page계량지표 = null;
        TabPages.Page화폐화지표 m_page화폐화지표 = null;
        TabPages.Page기능회복율 m_page기능회복율 = null;
        TabPages.Page기능회복기간 m_page기능회복기간 = null;
        TabPages.Page단가 m_page단가 = null;
        TabPages.Page지불의사액 m_page지불의사 = null;
        TabPages.Page가구수면적 m_page가구수및면적 = null;
        TabPages.Page비사용가치 m_page비사용가치 = null;
        TabPages.Page스티그마 m_page스티그마 = null;

        private UnE.Command.CommandManager m_cmdMgr = null;
        private bool m_noRefresh = false;

        private static FormMain m_instance = null;

        // m_ignorePushButton이 True일 동안은 RibbonButton이 동작하지 않도록 한다.
        private bool m_ignorePushButton = false;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public string 수치지도Path
        {
            get { return m_str수치지도Path; }
        }

        public string 토지이용계획도Path
        {
            get { return m_str토지이용계획도Path; }
        }

        public string 지적도Path
        {
            get { return m_str지적도Path; }
        }

        public UnE.Geometry.Vertex2D ScreenCenter
        {
            get { return m_vScreenCenter; }
        }

        public bool NoDrawing
        {
            get { return m_noShapeFileDrawing; }
        }

        public QuadTree QuadTree
        {
            get { return m_quadTree; }
        }

        public bool CloseApplication
        {
            get { return m_closeApplication; }
            set { m_closeApplication = value; }
        }

        private Overlay.OverlaySelectRectPainter mSelectRectPainter = null;
        private Overlay.OverlaySelectRectPainter m2SelectRectPainter = null;
        private Overlay.OverlayPainter m_overlayPainter = null;

        public Overlay.OverlayPainter OverlayPainter
        {
            get { return m_overlayPainter; }
        }

        // PNU 코드별 공시지가
        public Dictionary<string, double> CodeCost
        {
            get { return m_dicCodeCost; }
        }

        // 법정동 코드별 주소
        public Dictionary<string, string> CodeAddress
        {
            get { return m_dicCodeAddr; }
        }

        public DXFViewer.Layer ShapeFilePolygonLayer
        {
            get { return m_shapeFilePolygonLayer; }
            set { m_shapeFilePolygonLayer = value; }
        }

        public bool DrawingOverlay
        {
            get { return m_frmCalcCondition.Visible; }
        }

        public UnE.Command.CommandManager CommandManager
        {
            get { return m_cmdMgr; }
        }

        public bool NoRefresh
        {
            get { return m_noRefresh; }
        }

        private bool m_bChangedData = false;
        public bool ChangedData
        {
            get { return m_bChangedData; }
            set { m_bChangedData = value; }
        }

        public FormMain()
        {
            m_instance = this;

            

            InitializeComponent();

            m_quadTree = new QuadTree();

            // 효율적인 시스템 운영을 위하여 초기 데이터는 Thread에서 읽는다.
            ReadAsyncPublicCost();
            ReadAsyncAddress();


            m_cmdMgr = new UnE.Command.CommandManager(mRibbonForm.UndoBtn, mRibbonForm.RedoBtn);

            rbtnHidePanel.Visible = false;
            rbtnShowPanel.Location = rbtnHidePanel.Location;
            rbtnShowPanel.Visible = true;

            m_frm수치지도Layer = new DockingForm.FormDetailLayer("수치지형도", DockingForm.FormLayer.LayerType.수치지도);
            m_frm토지이용계획도Layer = new DockingForm.FormDetailLayer("토지이용계획도", DockingForm.FormLayer.LayerType.토지이용계획도);

            
        }

        private FormRibbon mRibbonForm = new FormRibbon();
        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
            UnE.GUI.RibbonButton btnRB = ( UnE.GUI.RibbonButton)sender;
            btnRB.Refresh();
        }
        
        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            UnE.GUI.RibbonButton btnRB = (UnE.GUI.RibbonButton)sender;
            OnRibbonButtonExecute(sender, btnRB.ID);
            btnRB.Refresh();
        }
        private void OnRibbonButtonUpdate(UnE.GUI.RibbonButton btn, int nID)
        {
            switch (nID)
            {
                case ID.ID_LAYER:
                    if (m_frmLayer.Visible == true && splitContainerMain.Panel1Collapsed == false && splitContainerLeft.Panel1Collapsed == false)
                    {
                        btn.IsChecked = true;
                    }
                    else
                    {
                        btn.IsChecked = false;
                    }
                    btn.Refresh();
                    break;
                case ID.ID_DETAIL_LAYER:
                    if ((m_frm수치지도Layer.Visible == true || m_frm토지이용계획도Layer.Visible == true)
                        && splitContainerMain.Panel1Collapsed == false && splitContainerLeft.Panel2Collapsed == false)
                    {
                        btn.IsChecked = true;
                    }
                    else
                    {
                        btn.IsChecked = false;
                    }
                    btn.Refresh();
                    break;
                case ID.ID_DETAIL_ATTRIB:
                    if( SelectionManager.DetailAttribForm.Visible == true)
                    {
                        btn.IsChecked = true;
                    }
                    else
                    {
                        btn.IsChecked = false;
                    }
                    btn.Refresh();
                    break; 
            }
        }

        private void OnRibbonButtonExecute(object sender, int nID)
        {
            if (m_ignorePushButton)
                return;

            UnE.GUI.RibbonButton btnRB = (UnE.GUI.RibbonButton)sender;
            switch (nID)
            {
                case ID.ID_PROJECT_OPEN:
                    프로젝트열기ToolStripMenuItem_Click(null, null);
                  //  mRibbonForm.SelectTab(1); 
                    break;
                case ID.ID_PROJECT_SAVE:
                    프로젝트저장ToolStripMenuItem_Click(null, null);
                    break;
                case ID.ID_PROJECT_SAVEAS:
                    다른이름으로저장ToolStripMenuItem_Click(null, null);
                    break;
                case ID.ID_FILE_OPEN_DXF:
                    수치지도열기ToolStripMenuItem_Click(null, null);
                   // mRibbonForm.SelectTab(1); 
                    break;
                case ID.ID_FILE_OPEN_DXF2:
                    토지이용계획도열기ToolStripMenuItem_Click(null, null);
                    //mRibbonForm.SelectTab(1); 
                    break;
                case ID.ID_FILE_OPEN_SHAPE:
                    지적도열기ToolStripMenuItem_Click(null, null);
                  //  mRibbonForm.SelectTab(1); 
                    break;
                case ID.ID_FILE_OPTION:
                    tsMenuSelectedColor_Click(null, null);
                    break;
                case ID.ID_LAYER:
                    btnLayer_Click(null, null);
                    break;
                case ID.ID_DETAIL_LAYER:
                    btnDetailLayer_Click(null, null);
                    break;
                case ID.ID_DETAIL_ATTRIB:
                    btnDetailAttrib_Click(null, null);
                    break;        
                case ID.ID_SYSTEM_CONST:
                    btnSystemConst_Click(null, null);
                    break;
                case ID.ID_CHECK_VALUE:
                    btnCheckValue_Click(null, null);
                    break;
                case ID.ID_UNDO:
                    break;
                case ID.ID_REDO:
                    break;
                case ID.ID_SELECT:
                    tsMenuSelect_Click(null, null);
                    break;
                case ID.ID_DELETE_SELECT:
                    tsMenuRemoveSelectedShapes_Click(null, null);
                    break;
                case ID.ID_DELETE_UNSELECT:
                    tsMenuRemoveUnselectedShapes_Click(null, null);
                    break;
            }
        }

        public ToolStripStatusLabel GetStatusLabel()
        {
            return tsDXFCoord;
        }

        private void ReadAsyncPublicCost()
        {
            Thread t = new Thread(new ThreadStart(ReadAsyncCost));
            t.Start();
        }

        private void ReadAsyncAddress()
        {
            Thread t = new Thread(new ThreadStart(ReadAsyncAddr));
            t.Start();
        }

        private void ReadAsyncCost()
        {
            string strPath = Application.StartupPath + "\\공시지가.txt";

            if (!System.IO.File.Exists(strPath))
            {
                m_dicCodeCost = new Dictionary<string, double>();
                return;
            }

            Dictionary<string, double> dicCodeCost = new Dictionary<string, double>();

            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            System.IO.StreamReader reader = new System.IO.StreamReader(strPath, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrTokens = strLine.Split('\t');

                if (arrTokens.Count() != 2)
                    continue;

                double dCost;

                if (double.TryParse(arrTokens[1].Trim(), out dCost))
                {
                    dicCodeCost[arrTokens[0].Trim()] = dCost;
                }
            }

            reader.Close();
            m_dicCodeCost = dicCodeCost;
        }

        private void ReadAsyncAddr()
        {
            string strPath = Application.StartupPath + "\\법정동코드.txt";

            if (!System.IO.File.Exists(strPath))
            {
                m_dicCodeAddr = new Dictionary<string, string>();
                return;
            }

            Dictionary<string, string> dicCodeAddr = new Dictionary<string, string>();

            Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            System.IO.StreamReader reader = new System.IO.StreamReader(strPath, encEUC_KR);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrTokens = strLine.Split('\t');

                if (arrTokens.Count() != 2)
                    continue;

                dicCodeAddr[arrTokens[0].Trim()] = arrTokens[1].Trim();
            }

            reader.Close();
            m_dicCodeAddr = dicCodeAddr;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.panelRibbonToolbar.Controls.Add(mRibbonForm);
            mRibbonForm.RibbonButtonOwner = this;
            mRibbonForm.Show();
            //m_selectionMgr = new SelectionManager();

            m_frm수치지도Layer.TopLevel = false;
            splitContainerLeftDown.Panel1.Controls.Add(m_frm수치지도Layer);
            m_frm수치지도Layer.Dock = DockStyle.Fill;
            m_frm수치지도Layer.Show();

            m_frm토지이용계획도Layer.TopLevel = false;
            splitContainerLeftDown.Panel2.Controls.Add(m_frm토지이용계획도Layer);
            m_frm토지이용계획도Layer.Dock = DockStyle.Fill;
            m_frm토지이용계획도Layer.Show();

            m_frmLayer.TopLevel = false;
            splitContainerLeft.Panel1.Controls.Add(m_frmLayer);
            m_frmLayer.Dock = DockStyle.Fill;
            m_frmLayer.Show();

            splitContainerMain.Panel1MinSize = 1;
            splitContainerMain.SplitterDistance = m_nSplitDistance;

            InitDXFControl();

            SetDockingMode(0);
            //rbtnHidePanel.PerformClick();
            //mRibbonForm.LayerBtn.IsChecked = true;

            tsDXFCoord.Text = "";

            m_frmCalcCondition = new Popup.FormCalcCondition();
            FormFrame.Instance.WindowState = FormWindowState.Maximized;

            m_vScreenCenter = new UnE.Geometry.Vertex2D(this.Size.Width / 2, this.Size.Height / 2);

            SetTitle("제목없음");

            SetPages();

            if(mRibbonForm.DelSelectBtn.IsChecked ||  mRibbonForm.DelUnSelectBtn.IsChecked)
            {
                mSelectRectPainter = m2SelectRectPainter;
            }
            else if( mRibbonForm.SelectBtn.IsChecked)
            {
                mSelectRectPainter = null;
            }


            mUpdataeTmr.Interval = 500;
            mUpdataeTmr.Enabled = true;
            mUpdataeTmr.Start();
            //if (tsMenuRemoveSelectedShapes.Checked || tsMenuRemoveUnselectedShapes.Checked)
            //    mSelectRectPainter = m2SelectRectPainter;
            //else if (tsMenuSelect.Checked)
            //    mSelectRectPainter = null;

           
        }

        private void SetPages()
        {
            splitContainerMain.Panel2.Controls.Add(tabCtrlSystemConst);
            tabCtrlSystemConst.Dock = DockStyle.Fill;
            tabCtrlSystemConst.Visible = false;

            m_page계량지표 = new TabPages.Page계량지표();
            tabPage계량지표.Controls.Add(m_page계량지표);
            m_page계량지표.Dock = DockStyle.Fill;
            m_page계량지표.Show();

            m_page화폐화지표 = new TabPages.Page화폐화지표();
            tabPage화폐화지표.Controls.Add(m_page화폐화지표);
            m_page화폐화지표.Dock = DockStyle.Fill;
            m_page화폐화지표.Show();

            m_page기능회복율 = new TabPages.Page기능회복율();
            tabPage기능회복율.Controls.Add(m_page기능회복율);
            m_page기능회복율.Dock = DockStyle.Fill;
            m_page기능회복율.Show();

            m_page기능회복기간 = new TabPages.Page기능회복기간();
            tabPage기능회복기간.Controls.Add(m_page기능회복기간);
            m_page기능회복기간.Dock = DockStyle.Fill;
            m_page기능회복기간.Show();

            m_page단가 = new TabPages.Page단가();
            tabPage단가.Controls.Add(m_page단가);
            m_page단가.Dock = DockStyle.Fill;
            m_page단가.Show();

            m_page지불의사 = new TabPages.Page지불의사액();
            tabPage지불의사액.Controls.Add(m_page지불의사);
            m_page지불의사.Dock = DockStyle.Fill;
            m_page지불의사.Show();

            m_page가구수및면적 = new TabPages.Page가구수면적();
            tabPage지역별가구수및면적.Controls.Add(m_page가구수및면적);
            m_page가구수및면적.Dock = DockStyle.Fill;
            m_page가구수및면적.Show();

            m_page비사용가치 = new TabPages.Page비사용가치();
            tabPage비사용가치.Controls.Add(m_page비사용가치);
            m_page비사용가치.Dock = DockStyle.Fill;
            m_page비사용가치.Show();

            m_page스티그마 = new TabPages.Page스티그마();
            tabPage스티그마.Controls.Add(m_page스티그마);
            m_page스티그마.Dock = DockStyle.Fill;
            m_page스티그마.Show();

            
        }

        private void InitDXFControl()
        {

            //DXFPanelEx panel = new DXFPanelEx();


            m_dxfControl = new DXFViewer.DXFControl();
            this.splitContainerMain.Panel2.Controls.Add(m_dxfControl);
            m_dxfControl.Dock = DockStyle.Fill;

            m_dxfControl.AntiAliasing = true;
            m_dxfControl.BackColor = System.Drawing.Color.Black;
            m_dxfControl.Dock = System.Windows.Forms.DockStyle.Fill;
            m_dxfControl.DrawHatchFirst = false;

            m_dxfControl.MouseMove += new MouseEventHandler(OnMouseMove);
            m_dxfControl.MouseClick += new MouseEventHandler(OnMouseClick);
            m_dxfControl.KeyDown += new KeyEventHandler(OnKeyDown);

            m_dxfControl.MouseMove += new MouseEventHandler(OnMouseMoveOverlay);
            m_dxfControl.MouseDown += new MouseEventHandler(OnMouseDownOverlay);
            m_dxfControl.MouseUp += new MouseEventHandler(OnMouseUpOverlay);
            m_dxfControl.Resize += new System.EventHandler(this.OnSize);


            DXFExternPainter exPainter = new DXFExternPainter(m_dxfControl);
            m_dxfControl.ExternalPainter = exPainter;
            
            m2SelectRectPainter = new Overlay.OverlaySelectRectPainter(m_dxfControl, Color.Aqua);
            //mSelectRectPainter = m2SelectRectPainter;
            //mSelectRectPainter = new Overlay.OverlaySelectRectPainter(m_dxfControl, Color.Aqua);
            m2SelectRectPainter.InvalidateControl += m_dxfControl.Refresh;

            m_overlayPainter = new Overlay.OverlayPainter(m_dxfControl);
            m_overlayPainter.InvalidateControl += m_dxfControl.Refresh;

            exPainter.SelectRectOverlayPaint += m2SelectRectPainter.DrawSelectRect;
            exPainter.OverlayObjectPainter += m_overlayPainter.DrawOverlay;
            

            // Info Read File
            m_dxfControl.BeginRead += DXFControl_BeginRead;
            m_dxfControl.EndRead += DXFControl_EndRead;
            m_dxfControl.ReadEntity += DXFControl_ReadEntity;


            //FormPaint form = new FormPaint();
            //form.Show();
            //m_dxfControl.BeginPaint += new DXFViewer.DXFControl.BeginPaintEventHandler(form.BeginPaint);
            //m_dxfControl.EndPaint += new DXFViewer.DXFControl.EndPaintEventHandler(form.EndPaint);

            // m_dxfControl.RefreshEvent += new DXFViewer.DXFControl.RefreshEventHandler(form.RefreshEvent);
            // m_dxfControl.BeginPan += new DXFViewer.DXFControl.beginPanning(form.BeginPan);
            // m_dxfControl.EndPan += new DXFViewer.DXFControl.endPanning(form.EndPan); 

            //m_dxfControl.SetExternalWheelEvent = true;
        }


        private int nProgCount = 0;
        private string m_szReadFileName = "";
        private string szMsg = "";
        private string mType = "";
        public void DXFControl_ReadEntity(string szEntityName, int nCount)
        {

            //if (mType == "SHP" || mType == "DXF")
            //    szMsg = string.Format("Read {0} {1}/{2}", szEntityName, nCount, nProgCount);
            //else
            //    szMsg = string.Format("{0} {1}/{2}", szEntityName, nCount, nProgCount);
            this.Invoke(new Action(delegate()
                {
            
                 int nCurrent = (int)(((float)nCount / (float)nProgCount) * 100.0f);
                toolStripProgressBar1.Value = nCurrent;

                toolStripStatusLabel3.Text = szMsg;

                  }));
           

        }

        public void DXFControl_EndRead(string szPath, string szType)
        {
            this.Invoke(new Action(delegate()
            {
                toolStripProgressBar1.Value = 0;
                this.Cursor = Cursors.Arrow;

                toolStripStatusLabel3.Text = "DONE";
                statusStrip1.Refresh();

            }));
           
        }

        public void DXFControl_BeginRead(string szPath, string szType, int nEntity)
        {
            this.Invoke(new Action(delegate()
            {
                toolStripProgressBar1.Value = 0;
                m_szReadFileName = szPath;
                nProgCount = nEntity;
                this.Cursor = Cursors.WaitCursor;
                mType = szType;

                toolStripStatusLabel3.Text = m_szReadFileName;
                statusStrip1.Refresh();

            }));
            
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (m_overlayPainter != null)
            {
                m_overlayPainter.OnKeyDown(sender, e);
            }
        }

        private void OnSize(object sender, EventArgs e)
        {
            SetTitle(labelTitle.Text);
            RefreshView();
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("OnMouseClick");
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (m_frmCalcCondition.Visible)
            {
                if (m_overlayPainter != null && m_overlayPainter.DrawType != Overlay.OverlayPainter.DrawingType.NONE)
                    return;
            }

            if(!mRibbonForm.SelectBtn.IsChecked)
            //if (!tsMenuSelect.Checked)
                return;

            double x, y;

            if (GetDXFCoord(e.X, e.Y, out x, out y))
            {
                System.Diagnostics.Trace.WriteLine("GetDXFCoord");
                // 지적도에서만 선택한다.
                if (SelectionManager.SelectShape(x, y, m_layer지적도))
                {
                    m_dxfControl._Refresh();
                    System.Diagnostics.Trace.WriteLine("SelectShape");
                }
            }
        }

        private void OnMouseDownOverlay(object sender, MouseEventArgs e)
        {
            if (mSelectRectPainter != null)
            {
                mSelectRectPainter.OnMouseDown(sender, e);
            }
            else if (m_overlayPainter != null)
            {
                if (!m_frmCalcCondition.Visible)
                    return;

                m_overlayPainter.OnMouseDown(sender, e);
            }
        }

        private void OnMouseUpOverlay(object sender, MouseEventArgs e)
        {
            if (mSelectRectPainter != null)
            {
                mSelectRectPainter.OnMouseUp(sender, e);
            }
            else if (m_overlayPainter != null)
            {
                m_overlayPainter.OnMouseUp(sender, e);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_dxfControl.Layers.Count > 0)
            {

                double x, y;

                if (GetDXFCoord(e.X, e.Y, out x, out y))
                {
                    tsDXFCoord.Text = string.Format("({0:f1}, {1:f1})", x, y);
                }
            }

            if (m_overlayPainter != null)
            {
                m_overlayPainter.OnMouseMove(sender, e);
            }
        }

        private void OnMouseMoveOverlay(object sender, MouseEventArgs e)
        {
            if (mSelectRectPainter != null)
            {
                mSelectRectPainter.OnMouseMove(sender, e);
            }
        }


        private bool GetDXFCoord(int x, int y, out double _x, out double _y)
        {
            _x = _y = 0.0;
            UnE.Geometry.Vertex2D vertex = m_dxfControl.ScreenToGlobal(x, y);

            if (vertex != null)
            {
                UnE.Geometry.Vertex2D vMove = m_dxfControl.MovedVertex;
                float fFlag = 1.0f;
                _x = (vertex.x - vMove.x) * fFlag;
                _y = (vertex.y - vMove.y) * fFlag;

                return true;
            }

            return false;
        }

        private void LoadDXF(string strPath, string strRefPath, ref string strDXFPath, DockingForm.FormDetailLayer frmDetailLayer, DockingForm.FormLayer.LayerType layerType, DockingType dockingType, bool setViewport, bool refresh)
        {
            if (strDXFPath != strRefPath)
            {
                if (OpenDXFFile(strPath, frmDetailLayer.Layers, refresh))
                {
                    strDXFPath = strRefPath;
                    m_frmLayer.SetLayer(layerType, true, true);
                    frmDetailLayer.Visible = m_frmLayer.IsChecked(layerType);
                    SetDockingMode(m_nDockingMode | (int)dockingType);

                    if (setViewport)
                        SetViewport();
                }
                else
                {
                    strDXFPath = "";
                    m_frmLayer.SetLayer(layerType, m_frmLayer.IsChecked(layerType), false);
                }
            }
        }

        public void IgnorePushButton()
        {
            m_ignorePushButton = true;

            Thread t = new Thread(new ThreadStart(IgnorePushButtonThread));
            t.Start();
        }

        private void IgnorePushButtonThread()
        {
            System.Threading.Thread.Sleep(500);
            m_ignorePushButton = false;
        }

        private void 수치지도열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "수치지형도 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Mouse Double Click으로 의도하지 않은 버튼이 클릭되는것을 막는다.
                IgnorePushButton();

                LoadDXF(dlg.FileName, dlg.FileName, ref m_str수치지도Path, m_frm수치지도Layer, DockingForm.FormLayer.LayerType.수치지도, DockingType.수치지도_LAYER, true, true);
                /*if (m_str수치지도Path != dlg.FileName)
                {
                    if (OpenDXFFile(dlg.FileName, m_frm수치지도Layer.Layers))
                    {
                        m_str수치지도Path = dlg.FileName;
                        m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.수치지도, true, true);
                        m_frm수치지도Layer.Visible = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.수치지도);
                        SetDockingMode(m_nDockingMode | (int)DockingType.수치지도_LAYER);

                        SetViewport();
                    }
                    else
                    {
                        m_str수치지도Path = "";
                        m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.수치지도, m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.수치지도), false);
                    }
                }*/
            }
        }

        private void 토지이용계획도열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "토지이용계획도 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Mouse Double Click으로 의도하지 않은 버튼이 클릭되는것을 막는다.
                IgnorePushButton();

                LoadDXF(dlg.FileName, dlg.FileName, ref m_str토지이용계획도Path, m_frm토지이용계획도Layer, DockingForm.FormLayer.LayerType.토지이용계획도, DockingType.토지이용계획도_LAYER, true, true);
                /*if (m_str토지이용계획도Path != dlg.FileName)
                {
                    if (OpenDXFFile(dlg.FileName, m_frm토지이용계획도Layer.Layers))
                    {
                        m_str토지이용계획도Path = dlg.FileName;
                        m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.토지이용계획도, true, true);
                        m_frm토지이용계획도Layer.Visible = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.토지이용계획도);
                        SetDockingMode(m_nDockingMode | (int)DockingType.토지이용계획도_LAYER);

                        SetViewport();
                    }
                    else
                    {
                        m_str토지이용계획도Path = "";
                        m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.토지이용계획도, m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.토지이용계획도), false);
                    }
                }*/
            }
        }
        
        private void LoadShapeFile(string strPath, string strRefPath, bool setViewport, bool isShapeFile, Dictionary<string, Data.ShapeAttrib> shapeAttribs = null)
        {
            if (m_str지적도Path != strRefPath)
            {
                ShapeFile.ShapeFileLoader loader = new ShapeFile.ShapeFileLoader(m_dxfControl);
                libShapeFile.ShapeInfo shapeInfo;
                m_noShapeFileDrawing = true;

                bool isOpened = false;
                UnE.Geometry.Vertex2D prevMovedVertex = null;
                
                if (m_dxfControl.Layers.Count > 0)
                    prevMovedVertex = new UnE.Geometry.Vertex2D(m_dxfControl.MovedVertex.x, m_dxfControl.MovedVertex.y);

                if (isShapeFile)
                    isOpened = loader.OpenFile(strPath, ref m_layer지적도, ref prevMovedVertex, ref m_BoundShapeTL, ref m_BoundShapeBR, out shapeInfo);
                else
                    isOpened = loader.OpenUSHFile(strPath, ref m_layer지적도, ref prevMovedVertex, ref m_BoundShapeTL, ref m_BoundShapeBR, out shapeInfo);

                if (isOpened)
                {
                    m_str지적도Path = strRefPath;
                    m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.지적도, true, true);
                    SetDockingMode(m_nDockingMode | (int)DockingType.ALL_LAYER);
                }
                else
                {
                    m_str지적도Path = "";
                    m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.지적도, m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.지적도), true);
                }

                if (shapeAttribs != null && shapeInfo != null)
                {
                    int nFieldCount = shapeInfo.GetFieldCount();

                    foreach (DXFViewer.Shape shape in m_layer지적도.Shapes)
                    {
                        if (shape is Drawing.Polygon)
                        {
                            Drawing.Polygon polygon = (Drawing.Polygon)shape;
                            SelectionManager.DetailAttribForm.SetPolygonInfo(polygon, nFieldCount, shapeInfo, shapeAttribs);
                        }
                        else if (shape is Drawing.PolygonList)
                        {
                            Drawing.PolygonList polygonList = (Drawing.PolygonList)shape;
                            List<Drawing.Polygon> polygons = polygonList.GetPolygons(null);

                            foreach (Drawing.Polygon polygon in polygons)
                            {
                                SelectionManager.DetailAttribForm.SetPolygonInfo(polygon, nFieldCount, shapeInfo, shapeAttribs);
                            }
                        }
                    }
                }

                SelectionManager.SetShapeInfo(shapeInfo, (Drawing.ShapeLayer)m_layer지적도);

                m_noShapeFileDrawing = false;

                if (setViewport)
                    SetViewport();
            }
        }

        private void 지적도열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Shape Files|*.shp|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "지적도 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Mouse Double Click으로 의도하지 않은 버튼이 클릭되는것을 막는다.
                IgnorePushButton();

                System.Diagnostics.Trace.WriteLine("OnRibbonButtonExecute, IgnoreRibbonButton : " + m_ignorePushButton.ToString());
                LoadShapeFile(dlg.FileName, dlg.FileName, true, true);
                /*if (m_str지적도Path != dlg.FileName)
                {
                    ShapeFile.ShapeFileLoader loader = new ShapeFile.ShapeFileLoader(m_dxfControl);
                    libShapeFile.ShapeInfo shapeInfo;
                    m_noShapeFileDrawing = true;

                    if (loader.OpenFile(dlg.FileName, ref m_layer지적도, ref m_prevMovedVertex, ref m_BoundShapeTL, ref m_BoundShapeBR, out shapeInfo))
                    {
                        m_str지적도Path = dlg.FileName;
                        m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.지적도, true, true);
                        SetDockingMode(m_nDockingMode | (int)DockingType.ALL_LAYER);

                        
                    }
                    else
                    {
                        m_str지적도Path = "";
                        m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.지적도, m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.지적도), true);
                    }

                    TimeLog("Prev SetShapeInfo");
                    SelectionManager.SetShapeInfo(shapeInfo, (Drawing.ShapeLayer)m_layer지적도);
                    TimeLog("After SetShapeInfo");

                    m_noShapeFileDrawing = false;

                    SetViewport();
                    //FormMain.Instance.RefreshView();
                }*/
            }
        }

        public void TimeLog(string strLog)
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}:{1}:{2}.{3}", dtNow.Hour, dtNow.Minute, dtNow.Second, dtNow.Millisecond);
            System.Diagnostics.Trace.WriteLine(strTime + ", " + strLog);
        }

        // Return 값이 -1이면 제일 뒤에 붙여넣는다.
        public int GetLayerInsertIndex(DockingForm.FormLayer.LayerType type)
        {
            if (type == DockingForm.FormLayer.LayerType.수치지도)
                return 0;
            else if (type == DockingForm.FormLayer.LayerType.지적도)
            {
                int nLayerCount = m_frm수치지도Layer.Layers.Count;

                if (nLayerCount > 0)
                {
                    DXFViewer.Layer layer = m_frm수치지도Layer.Layers[nLayerCount - 1];
                    return m_dxfControl.Layers.IndexOf(layer) + 1;
                }
                else
                    return 0;
            }
            else// if (type == DockingForm.FormLayer.LayerType.토지이용계획도)
                return -1;
        }

        private bool OpenDXFFile(string strPath, List<DXFViewer.Layer> layers, bool refresh)
        {
            foreach (DXFViewer.Layer layer in layers)
            {
                m_dxfControl.Layers.Remove(layer);
            }

            layers.Clear();

            DXFViewer.DXFControl dxf = new DXFViewer.DXFControl();


            // Info Read File
            dxf.BeginRead += DXFControl_BeginRead;
            dxf.EndRead += DXFControl_EndRead;
            dxf.ReadEntity += DXFControl_ReadEntity;


            dxf.Size = m_dxfControl.Size;
            dxf.OpenNRefresh = false;

            if (dxf.OpenDXF(strPath))
            {                
                //if (m_vDXFTL == null)
                //    m_vDXFTL = dxf.ObjectTL;
                //else
                //{
                //    if (m_vDXFTL.x > dxf.ObjectTL.x)
                //        m_vDXFTL.x = dxf.ObjectTL.x;

                //    if (m_vDXFTL.y < dxf.ObjectTL.y)
                //        m_vDXFTL.y = dxf.ObjectTL.y;
                //}

                //if (m_vDXFBR == null)
                //    m_vDXFBR = dxf.ObjectBR;
                //else
                //{
                //    if (m_vDXFBR.x < dxf.ObjectBR.x)
                //        m_vDXFBR.x = dxf.ObjectBR.x;

                //    if (m_vDXFBR.y > dxf.ObjectBR.y)
                //        m_vDXFBR.y = dxf.ObjectBR.y;
                //}

                bool isNew = m_dxfControl.Layers.Count == 0;

                if (!isNew)
                    dxf.MoveAll(m_dxfControl.MovedVertex.x - dxf.MovedVertex.x, m_dxfControl.MovedVertex.y - dxf.MovedVertex.y);
                /*if (m_prevMovedVertex != null)
                    dxf.MoveAll(m_prevMovedVertex.x - dxf.MovedVertex.x, m_prevMovedVertex.y - dxf.MovedVertex.y);*/

                if (layers == m_frm수치지도Layer.Layers)
                {
                    m_frm수치지도Layer.LayerBoundTL = dxf.ObjectTL;
                    m_frm수치지도Layer.LayerBoundBR = dxf.ObjectBR;
                }
                else
                {
                    m_frm토지이용계획도Layer.LayerBoundTL = dxf.ObjectTL;
                    m_frm토지이용계획도Layer.LayerBoundBR = dxf.ObjectBR;                   
                }

                foreach (DXFViewer.Layer layer in dxf.Layers)
                {
                    layers.Add(ToShapeLayer(layer));
                }

                if (layers == m_frm수치지도Layer.Layers)
                    m_dxfControl.Layers.InsertRange(0, layers);
                else
                    m_dxfControl.Layers.AddRange(layers);

                if (isNew)
                //if (m_prevMovedVertex == null)
                {
                    //m_prevMovedVertex = dxf.MovedVertex;
                    DXFViewer.Viewport viewport = dxf.GetViewport();
                    m_dxfControl.MovedVertex = dxf.MovedVertex;
                    m_dxfControl.LoadViewport(viewport, false);
                }

                dxf.Layers.Clear();

                if (refresh)
                    m_dxfControl._Refresh();


                dxf.BeginRead -= DXFControl_BeginRead;
                dxf.EndRead -= DXFControl_EndRead;
                dxf.ReadEntity -= DXFControl_ReadEntity;

                return true;
            }

            return false;
        }

        private DXFViewer.Layer ToShapeLayer(DXFViewer.Layer layer)
        {
            SoilMan.Drawing.ShapeLayer shapeLayer = new SoilMan.Drawing.ShapeLayer(m_dxfControl);

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                DXFViewer.Block block = shape.GetBlock();
                shapeLayer.Add(shape);
                shape.SetBlock(block);
            }

            shapeLayer.Hidden = layer.Hidden;
            shapeLayer.Frozen = layer.Frozen;
            shapeLayer.Lock = layer.Lock;
            shapeLayer.LineColor = layer.LineColor;
            shapeLayer.LayerName = layer.LayerName;

            return shapeLayer;
        }

        private void SetDockingMode(int nMode)
        {
            bool showAllLayer = (nMode & (int)DockingType.ALL_LAYER) == (int)DockingType.ALL_LAYER;
            bool show수치지도 = (nMode & (int)DockingType.수치지도_LAYER) == (int)DockingType.수치지도_LAYER;
            bool show토지이용계획도 = (nMode & (int)DockingType.토지이용계획도_LAYER) == (int)DockingType.토지이용계획도_LAYER;

            bool bSetSplitPos = false;
            if (splitContainerMain.Panel1Collapsed == true)
            {
                bSetSplitPos = true;
            }

            if (showAllLayer)
            {

                splitContainerMain.Panel1Collapsed = false;
                splitContainerLeft.Panel1Collapsed = false;

                if (show수치지도)
                {
                    splitContainerLeft.Panel2Collapsed = false;
                    splitContainerLeftDown.Panel1Collapsed = false;

                    if (show토지이용계획도)
                        splitContainerLeftDown.Panel2Collapsed = false;
                    else
                        splitContainerLeftDown.Panel2Collapsed = true;
                }
                else
                {
                    splitContainerLeftDown.Panel1Collapsed = true;

                    if (show토지이용계획도)
                    {
                        splitContainerLeft.Panel2Collapsed = false;
                        splitContainerLeftDown.Panel2Collapsed = false;
                    }
                    else
                    {
                        splitContainerLeft.Panel2Collapsed = true;
                        splitContainerLeftDown.Panel2Collapsed = true;
                    }
                }


                splitContainerMain.BringToFront();
                /*if (bSetSplitPos == true)
                {
                    splitContainerMain.SplitterDistance = m_nSplitDistance;
                }*/
                }
            else
            {
                splitContainerLeft.Panel1Collapsed = true;

                if (show수치지도)
                {
                    splitContainerMain.Panel1Collapsed = false;
                    splitContainerLeft.Panel2Collapsed = false;
                    splitContainerLeftDown.Panel1Collapsed = false;

                    if (show토지이용계획도)
                        splitContainerLeftDown.Panel2Collapsed = false;
                    else
                        splitContainerLeftDown.Panel2Collapsed = true;
                }
                else
                {
                    splitContainerLeftDown.Panel1Collapsed = true;

                    if (show토지이용계획도)
                    {
                        splitContainerMain.Panel1Collapsed = false;
                        splitContainerLeft.Panel2Collapsed = false;
                        splitContainerLeftDown.Panel2Collapsed = false;
                    }
                    else
                    {
                        splitContainerMain.Panel1Collapsed = true;
                        splitContainerLeft.Panel2Collapsed = true;
                        splitContainerLeftDown.Panel2Collapsed = true;
                    }
                }
                splitContainerMain.BringToFront();
            }


            if (splitContainerMain.Panel1Collapsed == true)
            {

                rbtnHidePanel.Visible = false;
                rbtnShowPanel.Location = rbtnHidePanel.Location;
                rbtnShowPanel.Visible = true; 
            }
            else
            {
                rbtnShowPanel.Visible = false;
                rbtnHidePanel.Location = rbtnShowPanel.Location;
                rbtnHidePanel.Visible = true;
               
            }

            m_nDockingMode = nMode;
        }

        private void splitContainerMain_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            m_nSplitDistance = e.SplitX;

            
        }

        public void RefreshView()
        {
            if (m_noRefresh)
                return;

            m_dxfControl._Refresh();
        }

        public void ShowLayer(DockingForm.FormLayer.LayerType type, bool visible)
        {
            if (type == DockingForm.FormLayer.LayerType.수치지도)
            {
                m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.수치지도, visible, m_frmLayer.IsEnabled(DockingForm.FormLayer.LayerType.수치지도));
                m_frm수치지도Layer.Visible = visible;
            }
            else if (type == DockingForm.FormLayer.LayerType.토지이용계획도)
            {
                m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.토지이용계획도, visible, m_frmLayer.IsEnabled(DockingForm.FormLayer.LayerType.토지이용계획도));
                m_frm토지이용계획도Layer.Visible = visible;
            }
            else if (type == DockingForm.FormLayer.LayerType.지적도)
            {
                m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.지적도, visible, m_frmLayer.IsEnabled(DockingForm.FormLayer.LayerType.지적도));
                ((Drawing.ShapeLayer)m_layer지적도).Usable = visible;
            }

            SetViewport();
        }

        public Color GetLineColor()
        {
            return Color.White;
            /*if (checkBoxTransparentLine.Checked)
                return Color.Transparent;

            return btnLineColor.BackColor;*/
        }

        public Color GetFillColor()
        {
            return Color.Transparent;
            /*if (checkBoxTransparentFill.Checked)
                return Color.Transparent;

            return btnFillColor.BackColor;*/
        }

        public double GetPointSize()
        {
            return 5.0;
            /*double dSize;

            if (double.TryParse(textBoxPointSize.Text, out dSize) && dSize > 0)
                return dSize;

            return 0;*/
        }

        public Drawing.PointDrawingType GetPointDrawingType()
        {
            return Drawing.PointDrawingType.CIRCLE;
            //return (Drawing.PointDrawingType)cboPointShape.SelectedIndex;
        }

        public int GetLineThickness()
        {
            return 1;
            /*int nThick;

            if (int.TryParse(textBoxLineThick.Text, out nThick) && nThick > 0)
                return nThick;

            return 0;*/
        }

        public bool EnableShapeAttribPopup()
        {
            return mRibbonForm.DetailAttribBtn.IsChecked;
        }

        private void SetTitle(string strTitle)
        {
            labelTitle.Text = strTitle;

            int x = (m_dxfControl.Size.Width - labelTitle.Size.Width) / 2;
            //int x = (panelProjectTitle.Size.Width - labelTitle.Size.Width) / 2;
            labelTitle.Location = new Point(x, labelTitle.Location.Y);
        }

        private void tsMenuSelectedColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            dlg.Color = SelectionManager.SelectedColor;

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (dlg.Color != SelectionManager.SelectedColor)
                {
                    SelectionManager.SelectedColor = dlg.Color;

                    if (SelectionManager.SelectedShapeList.Count > 0)
                        RefreshView();
                }
            }
        }

        private void btnSystemConst_Click(object sender, EventArgs e)
        {
            mRibbonForm.SystemConstBtn.IsChecked = !mRibbonForm.SystemConstBtn.IsChecked;
            mRibbonForm.SystemConstBtn.Refresh();

            panelProjectTitle.Visible = m_dxfControl.Visible = !mRibbonForm.SystemConstBtn.IsChecked;
            tabCtrlSystemConst.Visible = mRibbonForm.SystemConstBtn.IsChecked;
        }

        private void tabCtrlSystemConst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabCtrlSystemConst.SelectedTab != null)
                TabPages.TabPageManager.Instance.OnTabChanged(tabCtrlSystemConst.SelectedTab);
        }

        private void btnCheckValue_Click(object sender, EventArgs e)
        {
            mSelectRectPainter = null;

            mRibbonForm.CheckValueBtn.Enabled = false;

            if (!m_frmCalcCondition.Visible)
                m_frmCalcCondition.Show(this);

            m_dxfControl.Refresh();
        }

        public void NoOverlayDrawing()
        {
            FormMain.Instance.OverlayPainter.DrawType = Overlay.OverlayPainter.DrawingType.NONE;
            mSelectRectPainter = m2SelectRectPainter;
        }

        public void EnableCheckValueButton()
        {
            mRibbonForm.CheckValueBtn.Enabled = true;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_bChangedData == true)
            {
                DialogResult = MessageBox.Show(this, "변경된 데이터가 있습니다. 저장 후 종료하시겠습니까?", "저장 확인", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (DialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    프로젝트저장ToolStripMenuItem_Click(null, null);
                }
                else if (DialogResult == System.Windows.Forms.DialogResult.Cancel)
                {                   
                    e.Cancel = true;
                    return;
                }
            }

            mUpdataeTmr.Stop();
            mUpdataeTmr.Enabled = false;

            m_closeApplication = true;
            m_frmCalcCondition.Close();
        }

        public double Get계량지표FunctionData(SoilFunctionType functionType, LandType landType)
        {
            DataGridViewCell cell = m_page계량지표.GetFunctionData(functionType, landType);
            return GetCellDoubleData(cell);
        }

        public double Get화폐화지표FunctionData(SoilFunctionType functionType)
        {
            DataGridViewCell cell = m_page화폐화지표.GetFunctionData(functionType);
            return GetCellDoubleData(cell);
        }

        public double Get기능회복율FunctionData(SoilFunctionType functionType, TechType techType)
        {
            DataGridViewCell cell = m_page기능회복율.GetFunctionData(functionType, techType);
            return GetCellDoubleData(cell);
        }

        public double Get기능회복기간FunctionData(SoilFunctionType functionType, TechType techType)
        {
            DataGridViewCell cell = m_page기능회복기간.GetFunctionData(functionType, techType);
            return GetCellDoubleData(cell);
        }

        public double Get토양정화기술Cost(TechType techType)
        {
            DataGridViewCell cell = m_page단가.GetFunctionData(techType);
            return GetCellDoubleData(cell);
        }

        public double Get스티그마()
        {
            DataGridViewCell cell = m_page스티그마.GetStigmaData();
            return GetCellDoubleData(cell);
        }
        public double Get회복기간()
        {
            DataGridViewCell cell = m_page스티그마.GetRecoveryData();
            return GetCellDoubleData(cell);
        }

        public double Get유산가치()
        {
            DataGridViewCell cell = m_page비사용가치.Get유산가치();
            return GetCellDoubleData(cell);
        }

        public double Get존재가치()
        {
            DataGridViewCell cell = m_page비사용가치.Get존재가치();
            return GetCellDoubleData(cell);
        }
        public double Get선택가치()
        {
            DataGridViewCell cell = m_page비사용가치.Get선택가치();
            return GetCellDoubleData(cell);
        }

        public DataGridView Get지불의사액Grid()
        {
            if (m_page지불의사 == null)
                return null;
            return m_page지불의사.Datas;
        }


        public DataGridView Get비사용가치Grid()
        {
            return m_page비사용가치.Datas;
        }

        public DataGridView Get스티그마Grid()
        {
            return m_page스티그마.Datas;
        }

        public DataGridView Get지역별가구수면적Grid()
        {
            return m_page가구수및면적.Datas;
        }

        public DataGridView Get계량지표Grid()
        {
            return m_page계량지표.Datas;
        }

        public DataGridView Get화폐화지표Grid()
        {
            return m_page화폐화지표.Datas;
        }

        public DataGridView Get기능회복율Grid()
        {
            return m_page기능회복율.Datas;
        }

        public DataGridView Get기능회복기간Grid()
        {
            return m_page기능회복기간.Datas;
        }

        public DataGridView Get토양정화기술Grid()
        {
            return m_page단가.Datas;
        }

        

        private double GetCellDoubleData(DataGridViewCell cell)
        {
            if (cell == null)
                return 0.0;

            if (cell.Value == null)
                return 0.0;

            string strValue = cell.Value.ToString().Trim();
            double data;

            if (!double.TryParse(strValue, out data))
                return 0.0;

            return data;
        }

        public static string LandTypeName(LandType type)
        {
            if (type == LandType.General)
                return "일반토양";
            else if (type == LandType.Field)
                return "밭토양";
            else if (type == LandType.RiceField)
                return "논토양";
            else if (type == LandType.Mountain)
                return "임야토양";

            return "";
        }

        public static string SoilFunctionTypeName(SoilFunctionType type)
        {
            if (type == SoilFunctionType.오염물질정화)
                return "오염물질정화";
            else if (type == SoilFunctionType.수자원저수)
                return "수자원저수";
            else if (type == SoilFunctionType.대기정화_CO2)
                return "대기정화-CO2";
            else if (type == SoilFunctionType.대기정화_O2)
                return "대기정화-O2";
            else if (type == SoilFunctionType.대기냉각)
                return "대기냉각";
            else if (type == SoilFunctionType.수질정화)
                return "수질정화";
            else if (type == SoilFunctionType.식물생산기능)
                return "식물생산기능(서식처기능)";
            else if (type == SoilFunctionType.구조물지지기능)
                return "구조물지지기능";
            else if (type == SoilFunctionType.원료공급기능)
                return "원료공급기능";
            else if (type == SoilFunctionType.유산가치)
                return "유산가치";
            else if (type == SoilFunctionType.존재가치)
                return "존재가치";
            else if (type == SoilFunctionType.생태학적가치)
                return "생태학적가치";

            return "";
        }

        public static string SoilFunctionTypeUnit(SoilFunctionType type)
        {
            if (type == SoilFunctionType.오염물질정화)
                return "FA";
            else if (type == SoilFunctionType.수자원저수)
                return "FB";
            else if (type == SoilFunctionType.대기정화_CO2)
                return "FC";
            else if (type == SoilFunctionType.대기정화_O2)
                return "FD";
            else if (type == SoilFunctionType.대기냉각)
                return "FE";
            else if (type == SoilFunctionType.수질정화)
                return "FF";
            else if (type == SoilFunctionType.식물생산기능)
                return "FG";
            else if (type == SoilFunctionType.구조물지지기능)
                return "FH";
            else if (type == SoilFunctionType.원료공급기능)
                return "FI";
            else if (type == SoilFunctionType.유산가치)
                return "FJ";
            else if (type == SoilFunctionType.존재가치)
                return "FK";
            else if (type == SoilFunctionType.생태학적가치)
                return "FL";

            return "";
        }

        public void ShowResult(TechType techType, double dInheritanceValue, double dExistanceValue, double BioValue, Popup.SoilCleanCost cost, Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea, DataGridView gridArea, DataGridView gridPublicCost, DataGridView gridCondition, DataGridView gridValueCost)
        {
            m_frmResult.Show(techType, dInheritanceValue, dExistanceValue, BioValue, cost, dicLandTypeArea, gridArea, gridPublicCost, gridCondition, gridValueCost, this);
        }

        private void tsMenuSelect_Click(object sender, EventArgs e)
        {
            mRibbonForm.SelectBtn.IsChecked = true;
            mRibbonForm.DelSelectBtn.IsChecked = false;
            mRibbonForm.DelUnSelectBtn.IsChecked = false;

            mRibbonForm.SelectBtn.Refresh();
            mRibbonForm.DelSelectBtn.Refresh();
            mRibbonForm.DelUnSelectBtn.Refresh();
        }

        private void tsMenuRemoveUnselectedShapes_Click(object sender, EventArgs e)
        {

            mRibbonForm.SelectBtn.IsChecked = false;
            mRibbonForm.DelSelectBtn.IsChecked = false;
            mRibbonForm.DelUnSelectBtn.IsChecked = true;

            mSelectRectPainter = m2SelectRectPainter;

            mRibbonForm.SelectBtn.Refresh();
            mRibbonForm.DelSelectBtn.Refresh();
            mRibbonForm.DelUnSelectBtn.Refresh();
        }

        private void tsMenuRemoveSelectedShapes_Click(object sender, EventArgs e)
        {
            mRibbonForm.SelectBtn.IsChecked = false;
            mRibbonForm.DelSelectBtn.IsChecked = true;
            mRibbonForm.DelUnSelectBtn.IsChecked = false;

            mSelectRectPainter = m2SelectRectPainter;

            mRibbonForm.SelectBtn.Refresh();
            mRibbonForm.DelSelectBtn.Refresh();
            mRibbonForm.DelUnSelectBtn.Refresh();
        }

        public void OnSelectArea(float left, float top, float right, float bottom)
        {
            if (right - left == 0.0f || bottom - top == 0.0f)
                return;

            DXFViewer.Layer polygonLayer = FormMain.Instance.ShapeFilePolygonLayer;

            if (polygonLayer == null)
                return;

            Drawing.PolygonList polygonList = null;

            foreach (DXFViewer.Shape shape in polygonLayer.Shapes)
            {
                if (shape is Drawing.PolygonList)
                {
                    polygonList = (Drawing.PolygonList)shape;
                    break;
                }
            }

            if (polygonList == null)
                return;

            List<QuadNode> nodes = m_quadTree.GetNodes(left, top, right, bottom);

            if (nodes == null || nodes.Count == 0)
                return;

            Dictionary<int, int> dicShapeIndex = new Dictionary<int, int>();

            foreach (QuadNode node in nodes)
            {
                foreach (int nShapeID in node.Datas)
                {
                    dicShapeIndex[nShapeID] = nShapeID;
                }
            }

            List<Drawing.Polygon> polygons = null;
            libShapeFile.ShapeInfo shapeInfo = null;

            List<UnE.Geometry.Vertex2F> polygonSrc = new List<UnE.Geometry.Vertex2F>();
            polygonSrc.Add(new UnE.Geometry.Vertex2F(left, top));
            polygonSrc.Add(new UnE.Geometry.Vertex2F(left, bottom));
            polygonSrc.Add(new UnE.Geometry.Vertex2F(right, bottom));
            polygonSrc.Add(new UnE.Geometry.Vertex2F(right, top));

            if( mRibbonForm.DelSelectBtn.IsChecked)            
            {
                polygons = new List<Drawing.Polygon>();

                foreach (KeyValuePair<int, int> pair in dicShapeIndex)
                {
                    Drawing.Polygon polygon = polygonList.GetPolygonFromID(pair.Value);

                    if (polygon != null && polygon.HitTestArea(polygonSrc))
                    {
                        polygons.Add(polygon);
                        shapeInfo = polygon.ShapeInfo;
                    }
                }
            }
            else if (mRibbonForm.DelUnSelectBtn.IsChecked)
            {
                Dictionary<int, Drawing.Polygon> dicPolygons = new Dictionary<int, Drawing.Polygon>();

                foreach (KeyValuePair<int, int> pair in dicShapeIndex)
                {
                    Drawing.Polygon polygon = polygonList.GetPolygonFromID(pair.Value);

                    if (polygon != null && polygon.HitTestArea(polygonSrc))
                    {
                        dicPolygons[polygon.ID] = polygon;
                        shapeInfo = polygon.ShapeInfo;
                    }
                }

                if (shapeInfo == null)
                    return;

                polygons = polygonList.GetPolygons(dicPolygons);
            }

            if (shapeInfo == null || polygons == null)
                return;

            RemoveShapes(polygons, shapeInfo, (Drawing.ShapeLayer)polygonLayer);
        }

        public void RemoveShapes(List<Drawing.Polygon> polygons, libShapeFile.ShapeInfo shapeInfo, Drawing.ShapeLayer layer)
        {
            Command.CommandRemoveShapes cmd = new Command.CommandRemoveShapes(m_dxfControl, shapeInfo, layer, SelectionManager.DetailAttribForm);
            cmd.RemoveShapes = polygons;
            cmd.Do();
            m_cmdMgr.AddCommand(cmd);
        }

        private void 프로젝트저장ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_strProjectPath.Length == 0)
            {
                SaveFileDialog dlg = new SaveFileDialog();

                dlg.Filter = "Project Files|*.prj|All FIles|*.*";
                dlg.FilterIndex = 0;
                dlg.Title = "프로젝트 저장";

                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    // Mouse Double Click으로 의도하지 않은 버튼이 클릭되는것을 막는다.
                    IgnorePushButton();

                    SaveProject(dlg.FileName);
                    m_bChangedData = false;
                }
            }
            else
            {
                SaveProject(m_strProjectPath);
                m_bChangedData = false;
            }
        }

        private void SaveProject(string strPath)
        {
            bool visible지적도 = m_layer지적도 == null ? false : ((Drawing.ShapeLayer)m_layer지적도).Usable;
            Data.ProjectManager.Save(strPath, m_dxfControl, m_strProjectPath, m_str수치지도Path, m_str토지이용계획도Path, visible지적도, m_layer지적도, m_frm수치지도Layer, m_frm토지이용계획도Layer, m_frmCalcCondition, m_overlayPainter);
            m_strProjectPath = strPath;

            SetTitle();
        }

        private void SetTitle()
        {
            int nIndex1 = m_strProjectPath.LastIndexOf('\\');
            int nIndex2 = m_strProjectPath.LastIndexOf('.');

            if (nIndex1 < 0 || nIndex2 < 0)
            {
                SetTitle("제목없음");
                return;
            }

            string strTitle = m_strProjectPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            SetTitle(strTitle);
        }

        private void 프로젝트열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
        
                OpenFileDialog dlg = new OpenFileDialog();

                dlg.Filter = "Project Files|*.prj|All FIles|*.*";
                dlg.FilterIndex = 0;
                dlg.Title = "파일 열기";

                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    // Mouse Double Click으로 의도하지 않은 버튼이 클릭되는것을 막는다.
                    IgnorePushButton();

                    if (m_strProjectPath == dlg.FileName)
                        return;

                    Data.ProjectManager.ProjectData pData = Data.ProjectManager.Read(dlg.FileName, m_overlayPainter);

                    if (pData != null)
                        OpenProject(pData, dlg.FileName);
                    /*if (m_str지적도Path != dlg.FileName)
                    {
                        ShapeFile.ShapeFileLoader loader = new ShapeFile.ShapeFileLoader(m_dxfControl);
                        libShapeFile.ShapeInfo shapeInfo;
                        m_noShapeFileDrawing = true;
                        m_BoundShapeTL = new UnE.Geometry.Vertex2D();
                        m_BoundShapeBR = new UnE.Geometry.Vertex2D();
                        if (loader.OpenUSHFile(dlg.FileName, ref m_layer지적도, ref m_prevMovedVertex, ref m_BoundShapeTL, ref m_BoundShapeBR, out shapeInfo))
                        {
                            m_str지적도Path = dlg.FileName;
                            m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.지적도, true, true);
                            SetDockingMode(m_nDockingMode | (int)DockingType.ALL_LAYER);
                        }
                        else
                        {
                            m_str지적도Path = "";
                            m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.지적도, m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.지적도), true);
                        }

                        TimeLog("Prev SetShapeInfo");
                        SelectionManager.SetShapeInfo(shapeInfo, (Drawing.ShapeLayer)m_layer지적도);
                        TimeLog("After SetShapeInfo");

                        m_noShapeFileDrawing = false;


                        SetViewport();
                    }*/
                }
            
        }

        private DXFViewer.Layer FindLayer(string strLayerName, List<DXFViewer.Layer> layers)
        {
            foreach (DXFViewer.Layer layer in layers)
            {
                if (layer.LayerName == strLayerName)
                    return layer;
            }

            return null;
        }

        private void OpenProject(Data.ProjectManager.ProjectData data, string strProjectFilePath)
        {
            m_noRefresh = true;
            m_strProjectPath = strProjectFilePath;

            m_cmdMgr.Clear();
            
            m_str수치지도Path = m_str토지이용계획도Path = m_str지적도Path = "";
            m_frm수치지도Layer.Clear();
            m_frm토지이용계획도Layer.Clear();

            SelectionManager.DetailAttribForm.Clear();

            m_frm수치지도Layer.LayerBoundTL = m_frm토지이용계획도Layer.LayerBoundTL = m_BoundShapeTL = null;
            m_frm수치지도Layer.LayerBoundBR = m_frm토지이용계획도Layer.LayerBoundBR = m_BoundShapeBR = null;

            m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.수치지도, false, false);
            m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.토지이용계획도, false, false);
            m_frmLayer.SetLayer(DockingForm.FormLayer.LayerType.지적도, false, false);
            
            m_dxfControl.Layers.Clear();
            m_overlayPainter.RemoveAllOverlayShape();

            if (m_layer지적도 != null)
                m_layer지적도.RemoveAll();

            if (data.수치지도Layers != null && data.수치지도Layers.Count > 0)
            {
                string strRefPath = MakeDXFProjectFilePath(data.TempFolderPath + "\\" + data.수치지도Path, strProjectFilePath);
                LoadDXF(data.TempFolderPath + "\\" + data.수치지도Path, strRefPath, ref m_str수치지도Path, m_frm수치지도Layer, DockingForm.FormLayer.LayerType.수치지도, DockingType.수치지도_LAYER, false, false);

                foreach (DXFViewer.Layer layer in m_frm수치지도Layer.Layers)
                {
                    DXFViewer.Layer layer2 = FindLayer(layer.LayerName, data.수치지도Layers);

                    if (layer2 != null)
                    {
                        layer.Hidden = layer2.Hidden;
                        layer.LineColor = layer2.LineColor;
                    }
                }

                m_frm수치지도Layer.Reset();
            }

            if (data.토지이용계획도Layers != null && data.토지이용계획도Layers.Count > 0)
            {
                string strRefPath = MakeDXFProjectFilePath(data.TempFolderPath + "\\" + data.토지이용계획도Path, strProjectFilePath);
                LoadDXF(data.TempFolderPath + "\\" + data.토지이용계획도Path, strRefPath, ref m_str토지이용계획도Path, m_frm토지이용계획도Layer, DockingForm.FormLayer.LayerType.토지이용계획도, DockingType.토지이용계획도_LAYER, false, false);

                foreach (DXFViewer.Layer layer in m_frm토지이용계획도Layer.Layers)
                {
                    DXFViewer.Layer layer2 = FindLayer(layer.LayerName, data.토지이용계획도Layers);

                    if (layer2 != null)
                    {
                        layer.Hidden = layer2.Hidden;
                        layer.LineColor = layer2.LineColor;
                    }
                }

                m_frm토지이용계획도Layer.Reset();
            }

            if (data.지적도TL != null)
                LoadShapeFile(data.TempFolderPath + "\\" + data.지적도Path, strProjectFilePath, false, false, data.ShapeAttribs);

            Data.ProjectManager.DeleteFolder(data.TempFolderPath);

            //SetViewport();

            foreach (Overlay.OverlayShape shape in data.OverlayShapes)
            {
                //shape.Move((float)m_dxfControl.MovedVertex.x, (float)m_dxfControl.MovedVertex.y);
                m_overlayPainter.AddOverlayShape(shape);
            }

            m_overlayPainter.LandTypeAreas = data.LandTypeAreas;

            if (data.SelectedTechType != TechType.None)
            {
                Popup.FormInputCondition frm = m_frmCalcCondition.GetInputCondition();

                if (frm != null)
                {
                    if (data.SoilCleanCost != null)
                        frm.SetSoilCleanCost(data.SelectedTechType, data.SoilCleanCost);

                    frm.SetLandTypeInfo(data.LandTypeAreas);

                    frm.SelectedTechType = data.SelectedTechType;

                    frm.SelectedRegion = (int)data.SelectRegion;
                    frm.SelectedWTPType = (WTPType)data.SelectWTP;
                    frm.InputWTPYear = data.WTPYear;
                    frm.InputRejectionRatio = data.RejectionRatio;
                    frm.InputHousehold = data.Household;

                    frm.InheritanceValue = data.Inheritage;
                    frm.ExistanceValue = data.Existance;
                    frm.BioValue = data.Bio;

                    frm.Reset();
                }
            }

            if (data.Viewport == null)
                SetViewport();
            else
            {
                UnE.Geometry.Vertex2D vMoved = data.MovedVertex - m_dxfControl.MovedVertex;
                m_dxfControl.MoveAll(vMoved.x, vMoved.y);
                m_dxfControl.MovedVertex = data.MovedVertex;
                m_dxfControl.LoadViewport(data.Viewport, false);
            }

            SetTitle();

            m_noRefresh = false;
            RefreshView();
        }

        // strDXFTempPath는 임시폴더에 저장된 경로이므로, File이름만 가져온다음 확장자를 .prj로 바꾼다.
        private string MakeDXFProjectFilePath(string strDXFTempPath, string strProjectPath)
        {
            int nIndex1 = strDXFTempPath.LastIndexOf('\\');
            int nIndex2 = strDXFTempPath.LastIndexOf('.');

            if (nIndex1 < 0 || nIndex2 < 0)
                return "";

            string strFileName = strDXFTempPath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

            nIndex1 = strProjectPath.LastIndexOf('\\');

            if (nIndex1 < 0)
                return "";

            string strFilePath = strProjectPath.Substring(0, nIndex1) + "\\" + strFileName + ".prj";
            return strFilePath;
        }

        private void 다른이름으로저장ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // EUC-KR : 51949
            /*Encoding encEUC_KR = System.Text.Encoding.GetEncoding(51949);
            System.IO.StreamReader reader = new System.IO.StreamReader("F:/aaa.txt", encEUC_KR);

            Dictionary<string, string> dicAddr = new Dictionary<string, string>();

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                dicAddr[strLine] = strLine;
            }

            reader.Close();

            libShapeFile.ShapeInfo shapeInfo = null;
            List<Drawing.Polygon> polygons2 = new List<Drawing.Polygon>();

            foreach (DXFViewer.Shape shape in m_layer지적도.Shapes)
            {
                if (shape is Drawing.PolygonList)
                {
                    Drawing.PolygonList polygonList = (Drawing.PolygonList)shape;
                    List<Drawing.Polygon> polygons = polygonList.GetPolygons(null);

                    foreach (Drawing.Polygon polygon in polygons)
                    {
                        Popup.PolygonInfo info = (Popup.PolygonInfo)polygon.Tag;
                        shapeInfo = polygon.ShapeInfo;

                        if (!dicAddr.ContainsKey(info.Address))
                            polygons2.Add(polygon);
                    }
                }
            }

            Command.CommandRemoveShapes cmd = new Command.CommandRemoveShapes(m_dxfControl, shapeInfo, (Drawing.ShapeLayer)m_layer지적도, m_selectionMgr.DetailAttribForm, polygons2);
            cmd.Do();
            m_cmdMgr.AddCommand(cmd);*/
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "Project Files|*.prj|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "다른 이름으로 프로젝트 저장";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                // Mouse Double Click으로 의도하지 않은 버튼이 클릭되는것을 막는다.
                IgnorePushButton();

                SaveProject(dlg.FileName);
            }
        }
               
        public void SetViewport(DXFViewer.Viewport viewport = null)
        {
            bool bCheck1 = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.수치지도);
            bool bCheck2 = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.토지이용계획도);
            bool bCheck3 = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.지적도);
            
            if (bCheck1 == false && bCheck2 == false && bCheck3 == false)
                return;

            float minX = 0.0f;
            float maxY = 0.0f;
            float maxX = 0.0f;
            float minY = 0.0f;
            RectangleF rect = new RectangleF();
            if( bCheck1 == true)
            {
                minX = (float)m_frm수치지도Layer.LayerBoundTL.x;
                minY = (float)m_frm수치지도Layer.LayerBoundTL.y;
                maxX = (float)m_frm수치지도Layer.LayerBoundBR.x;
                maxY = (float)m_frm수치지도Layer.LayerBoundBR.y;
                rect = RectangleF.FromLTRB(minX, maxY, maxX, minY);  
                if( bCheck2 == true)
                {
                    if(m_frm토지이용계획도Layer != null && m_frm토지이용계획도Layer.LayerBoundTL != null)
                    {
                        if (!rect.Contains((float)m_frm토지이용계획도Layer.LayerBoundTL.x, (float)m_frm토지이용계획도Layer.LayerBoundTL.y))
                        {
                            minY = Math.Max(minY, (float)m_frm토지이용계획도Layer.LayerBoundTL.y);
                            minX = Math.Min(minX, (float)m_frm토지이용계획도Layer.LayerBoundTL.x);
                        }
                        if (!rect.Contains((float)m_frm토지이용계획도Layer.LayerBoundBR.x, (float)m_frm토지이용계획도Layer.LayerBoundBR.y))
                        {
                            maxY = Math.Min(maxY, (float)m_frm토지이용계획도Layer.LayerBoundBR.y);
                            maxX = Math.Max(maxX, (float)m_frm토지이용계획도Layer.LayerBoundBR.x);                                            
                        }
                        rect = RectangleF.FromLTRB(minX, maxY, maxX, minY);  
                    }                    
                }
                if (bCheck3 == true)
                {
                    if(m_BoundShapeTL != null && m_BoundShapeBR != null)
                    {
                        if (!rect.Contains((float)m_BoundShapeTL.x, (float)m_BoundShapeTL.y))
                        {
                            minY = Math.Max(minY, (float)m_BoundShapeTL.y);
                            minX = Math.Min(minX, (float)m_BoundShapeTL.x);
                        }
                        if (!rect.Contains((float)m_BoundShapeBR.x, (float)m_BoundShapeBR.y))
                        {
                            maxY = Math.Min(maxY, (float)m_BoundShapeBR.y);
                            maxX = Math.Max(maxX, (float)m_BoundShapeBR.x);
                        }                        
                    }                   
                }
            }
            else if( bCheck2 == true)
            { 
                minX = (float)m_frm토지이용계획도Layer.LayerBoundTL.x;
                minY = (float)m_frm토지이용계획도Layer.LayerBoundTL.y;
                maxX = (float)m_frm토지이용계획도Layer.LayerBoundBR.x;
                maxY = (float)m_frm토지이용계획도Layer.LayerBoundBR.y;
                rect = RectangleF.FromLTRB(minX, maxY, maxX, minY);  
                if (bCheck3 == true)
                {
                    if(m_BoundShapeTL != null && m_BoundShapeBR != null)
                    {
                        if (!rect.Contains((float)m_BoundShapeTL.x, (float)m_BoundShapeTL.y))
                        {
                            minY = Math.Max(minY, (float)m_BoundShapeTL.y);
                            minX = Math.Min(minX, (float)m_BoundShapeTL.x);
                        }
                        if (!rect.Contains((float)m_BoundShapeBR.x, (float)m_BoundShapeBR.y))
                        {
                            maxY = Math.Min(maxY, (float)m_BoundShapeBR.y);
                            maxX = Math.Max(maxX, (float)m_BoundShapeBR.x);
                        }                        
                    }                    
                }
            }
            else if( bCheck3 == true)
            {        
                minX = (float)m_BoundShapeTL.x;
                minY = (float)m_BoundShapeTL.y;
                maxX = (float)m_BoundShapeBR.x;
                maxY = (float)m_BoundShapeBR.y;                
            }

            minX += (float)m_dxfControl.MovedVertex.x;
            maxX += (float)m_dxfControl.MovedVertex.x;
            minY += (float)m_dxfControl.MovedVertex.y;
            maxY += (float)m_dxfControl.MovedVertex.y;

            double cX = minX + (maxX - minX)/2.0;
            double cY = minY + (Math.Max(maxY,minY)- Math.Min(maxY,minY))/2.0;
            
            float dx = maxX - minX;
            float dy = Math.Max(maxY, minY) - Math.Min(maxY, minY);           

            UnE.Geometry.Vertex2D vCenter = new UnE.Geometry.Vertex2D(cX, cY);
            UnE.Geometry.Vertex2D vTL = new UnE.Geometry.Vertex2D(minX, minY);
            UnE.Geometry.Vertex2D vBR = new UnE.Geometry.Vertex2D(maxX, maxY);
            UnE.Geometry.Vertex2D vBL = new UnE.Geometry.Vertex2D(minX, maxY);

            // Get Contorl Size
            int nWidth = m_dxfControl.Size.Width;
            int nHeight = m_dxfControl.Size.Height;

            double weight1 = nWidth * 0.85 / dx;
            double weight2 = nHeight * 0.85 / dy;
			double dViewportWeight = weight1 < weight2 ? weight1 : weight2;

            if( viewport == null)
            {
                DXFViewer.Viewport viewport2 = new DXFViewer.Viewport();
                viewport2.TopLeft = vTL;
                viewport2.BottomLeft = vBL;
                viewport2.BottomRight = vBR;
                viewport2.F11 = (float)dViewportWeight;
                viewport2.F21 = 0.0f;
                viewport2.FDx = minX;
                viewport2.F12 = 0.0f;

                if (m_dxfControl.DownToTop())
                {
                    viewport2.F22 = -(float)dViewportWeight;
                }
                else
                {
                    viewport2.F22 = (float)dViewportWeight;
                }

                viewport2.FDy = minY;
                viewport2.Weight = dViewportWeight;
                m_dxfControl.SetViewportCenter(vCenter);
                m_dxfControl.LoadViewport(viewport2, false);
            }
            else
            {                
                
                double minX2 = viewport.TopLeft.x;
                double maxX2 = viewport.BottomRight.x;

                double minY2 = viewport.TopLeft.y;
                double maxY2 = viewport.BottomRight.y;

                double cX2 = minX + (maxX - minX) / 2.0;
                double cY2 = minY + (Math.Max(maxY, minY) - Math.Min(maxY, minY)) / 2.0;
                UnE.Geometry.Vertex2D vCenter2 = new UnE.Geometry.Vertex2D(cX2, cY2);

                m_dxfControl.SetViewportCenter(vCenter2);
                m_dxfControl.LoadViewport(viewport, false);
            }

            RefreshView();
        }


        private int m_prevSplitDist = 0;
        private void rbtnHidePanel_Click(object sender, EventArgs e)
        {
            m_prevSplitDist = splitContainerMain.SplitterDistance;
            splitContainerMain.Panel1MinSize = 1;
            splitContainerMain.SplitterDistance = 1;

            rbtnHidePanel.Visible = false;
            rbtnShowPanel.Location = rbtnHidePanel.Location;
            rbtnShowPanel.Visible = true;
        }

        private void rbtnShowPanel_Click(object sender, EventArgs e)
        {
            if (m_prevSplitDist != 0)
                splitContainerMain.SplitterDistance = m_prevSplitDist;

            rbtnShowPanel.Visible = false;
            rbtnHidePanel.Location = rbtnShowPanel.Location;
            rbtnHidePanel.Visible = true;
        }

        private void splitContainerMain_SplitterMoved(object sender, SplitterEventArgs e)
        {
            //if (e.SplitX > 5)
            //{
            //    rbtnShowPanel.Visible = true;
            //    rbtnHidePanel.Location = rbtnShowPanel.Location;
            //    rbtnHidePanel.Visible = true;
            //}
            //else
            //{
            //    rbtnHidePanel.Visible = true;
            //    rbtnShowPanel.Location = rbtnHidePanel.Location;
            //    rbtnShowPanel.Visible = true;
            //}
        }

        private void btnLayer_Click(object sender, EventArgs e)
        {
            if (splitContainerMain.Panel1Collapsed == false)
            {
                bool bDown2Collpase = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.토지이용계획도);
                bool bDown1Collpase = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.수치지도);

                if (bDown1Collpase == false && bDown2Collpase == false)
                {
                    SetDockingMode(0);
                    splitContainerLeft.Panel2Collapsed = true;
                    return;
                }

                if (splitContainerLeft.Panel2Collapsed == true)
                {
                    SetDockingMode(0);
                    splitContainerLeft.Panel2Collapsed = true;
                    
                }
                splitContainerLeft.Panel1Collapsed = !splitContainerLeft.Panel1Collapsed;
                
            }
            else
            {
                this.splitContainerMain.Panel1Collapsed = false;
                splitContainerLeft.Panel1Collapsed = !splitContainerLeft.Panel1Collapsed;
               
                bool bDown2Collpase = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.토지이용계획도);
                bool bDown1Collpase = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.수치지도);

                if (bDown1Collpase == false && bDown2Collpase == false)
                {
                    splitContainerLeft.Panel2Collapsed = true;
                    return;
                }
                
                if (mRibbonForm.DetailLayerBtn.IsChecked == true)
                {
                    splitContainerLeft.Panel2Collapsed = false;
                    splitContainerLeftDown.Panel1Collapsed = !bDown1Collpase;
                    splitContainerLeftDown.Panel2Collapsed = !bDown2Collpase;
                }
                else
                {
                    splitContainerLeft.Panel2Collapsed = true;
                }
            }
        }

        private void btnDetailLayer_Click(object sender, EventArgs e)
        {
            if (splitContainerMain.Panel1Collapsed == false)
            {
                bool bDown1Collpase = splitContainerLeft.Panel1Collapsed;      
                bool bDown2Collpase = splitContainerLeft.Panel2Collapsed;

                if (bDown1Collpase == true && bDown2Collpase == false)
                {
                    splitContainerMain.Panel1Collapsed = true;
                    return;
                }

                bool b1 = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.토지이용계획도);
                bool b2 = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.수치지도);

                if (b1 == false && b2 == false)
                {
                    splitContainerLeft.Panel2Collapsed = true;
                }
                else
                {
                    splitContainerLeft.Panel2Collapsed = !splitContainerLeft.Panel2Collapsed;
                }
                
            }
            else
            {
               
                
                bool bDown2Collpase = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.토지이용계획도);
                bool bDown1Collpase = m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.수치지도);

                if (bDown1Collpase == false && bDown2Collpase == false)
                {
                    splitContainerLeft.Panel2Collapsed = true;
                    return;
                }

                splitContainerMain.Panel1Collapsed = false;
                splitContainerLeft.Panel2Collapsed = false;


                splitContainerLeft.Panel2Collapsed = false;
                splitContainerLeftDown.Panel1Collapsed = !bDown1Collpase;
                splitContainerLeftDown.Panel2Collapsed = !bDown2Collpase;
            }
        }

        private void btnDetailAttrib_Click(object sender, EventArgs e)
        {
            if( m_frmLayer.IsChecked(DockingForm.FormLayer.LayerType.지적도) == true)
            {
                if (SelectionManager.DetailFrameDialog.Visible)
                {
                    SelectionManager.DetailFrameDialog.Hide();
                }
                else
                {
                    SelectionManager.DetailFrameDialog.Show(this);
                }
            }           
        }


        private bool bUpdateProcess = false;
        private void mUpdataeTmr_Tick(object sender, EventArgs e)
        {
            if (bUpdateProcess == true)
                return;

            bUpdateProcess = true;
            foreach(UnE.GUI.RibbonButton rbtn in mRibbonForm.RibbonButtons)
            {
                OnRibbonButtonUpdate(rbtn, rbtn.ID);
            }
            bUpdateProcess = false;
        }
    }
}
