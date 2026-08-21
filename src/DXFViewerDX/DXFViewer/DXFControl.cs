using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using DXFDotNet;
using System.Reflection;
using System.ComponentModel;

using SharpDX;

using SharpDX.Direct3D;
using SharpDX.Direct3D10;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics;


using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Device1 = SharpDX.Direct3D10.Device1;
using DriverType = SharpDX.Direct3D10.DriverType;
using Factory = SharpDX.DXGI.Factory;
using FeatureLevel = SharpDX.Direct3D10.FeatureLevel;



namespace DXFViewer
{
    public class DXFControl : DXFDotNet.DXFControl, DXFDotNet.IShapeOwner
    {
        public delegate void BeginPaintEventHandler(bool b);
        public delegate void EndPaintEventHandler(bool b);
        public delegate void RefreshEventHandler(bool b);
        
        public event RefreshEventHandler RefreshEvent;
        public event BeginPaintEventHandler BeginPaint;
        public event EndPaintEventHandler EndPaint;

        protected DXFDotNet.ExternalPainter m_externPainter;
        public DXFDotNet.ExternalPainter ExternalPainter
        {
            get { return m_externPainter; }
            set { m_externPainter = value; }
        }

        // DirectX 영역
        protected SharpDX.DirectWrite.Factory mFactoryDWrite = null;
        public SharpDX.DirectWrite.Factory DWriteFactory
        {
            get { return mFactoryDWrite; }
        }
        
        protected float m_fRatio;
        protected float pplX = 1.0f;
        protected float pplY = 1.0f;
        
        [Browsable(false)]
        protected Device1 device;
        
        [Browsable(false)]
        protected SwapChain swapChain;
        
        [Browsable(false)]
        protected Factory factory;
        
        [Browsable(false)]
        protected RenderTargetView renderView;
        
        [Browsable(false)]
        protected Texture2D backBuffer;
        
        [Browsable(false)]
        protected RenderTarget d2dRenderTarget;

        [Browsable(false)]
        GdiInteropRenderTarget gdiRenderTarget;

        [Browsable(false)]
        public RenderTarget RenderTarget
        {
            get { return d2dRenderTarget; }
        }
        [Browsable(false)]
        protected SolidColorBrush solidColorBrush;
        [Browsable(false)]
        protected SwapChainDescription desc;
        [Browsable(false)]
        protected System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        [Browsable(false)]
        protected bool m_bInitDevice = false;

        [Browsable(false)]
        protected SharpDX.Direct2D1.LayerParameters mLayerParam;
        [Browsable(false)]
        protected SharpDX.Direct2D1.Layer layer;

        [Browsable(false)]
        protected System.Drawing.Printing.PrintDocument mPrintDocument;

        [Browsable(false)]
        public System.Drawing.Printing.PrintDocument PrintDocument
        {
            get
            {
                return mPrintDocument;
            }
            set
            {
                mPrintDocument = value;
            }
        }


        protected bool m_bDrawText = true;
        [Browsable(false)]
        public bool IsDrawText
        {
            get { return m_bDrawText; }
            set { m_bDrawText = value; }
        }

        protected bool m_bExternalWheel = false;
        public bool ExternalWheel
        {
            get { return m_bExternalWheel; }
            set { m_bExternalWheel = value; }
        }

        protected SolidBrush m_brushEditBox;
        protected MouseButtons m_btnPanning = System.Windows.Forms.MouseButtons.Middle;


        protected System.Drawing.Drawing2D.Matrix m_currentInverseMatrix;
        protected System.Drawing.Drawing2D.Matrix m_currentMatrix;


        //protected double m_dHomeViewportWeight;

        //protected Dictionary<long, System.Drawing.Pen> m_dicPens;

        protected bool m_drawHatchFirst = true;
        public bool DrawHatchFirst
        {
            get { return m_drawHatchFirst; }
            set { m_drawHatchFirst = value; }
        }

        //protected DateTime m_dtLastMouseWheel;

        protected double m_dViewportWeight = 1.0f;



        protected float m_fEditBoxLength;

        //protected float m_fHomedx;
        //protected float m_fHomedy;
        //protected float m_fHomem11;
        //protected float m_fHomem12;
        //protected float m_fHomem21;
        //protected float m_fHomem22;

        protected System.Drawing.Image m_img;

        protected bool m_isInitialized = false;

        //protected bool m_isOpened;

        protected bool m_isPanning = false;

        protected LineType m_lineTypeSelected;

        protected int m_nEditBoxSize;
        //protected int m_nGroupItemDistance;
        //protected int m_nGroupItemMinCount;

        protected bool m_openNRefresh = true;
        //protected Block m_pCurrentBlock;
        //protected Layer m_pCurrentLayer;

        protected Pen m_penEditBox;
        protected Pen m_penSelectedBright1;
        protected Pen m_penSelectedBright2;
        protected PlotSettings m_plotSettings;
        protected System.Drawing.Point m_ptPanningOrigin;
        //protected UnitOfLength m_unitOfLength;
        protected bool m_useAntialiasing = true;
        //protected bool m_useGroupItem;
        //protected bool m_useLastViewport;
        //protected bool m_useMouseWheel;

        // 원래의 위치에서 옮겨진 좌표값
        UnE.Geometry.Vertex2D m_vMove = new UnE.Geometry.Vertex2D();
        UnE.Geometry.Vertex2D m_vViewportTL = new UnE.Geometry.Vertex2D();
        UnE.Geometry.Vertex2D m_vViewportBL = new UnE.Geometry.Vertex2D();
        UnE.Geometry.Vertex2D m_vViewportBR = new UnE.Geometry.Vertex2D();
        UnE.Geometry.Vertex2D m_vOriginCenter = new UnE.Geometry.Vertex2D();

        protected int m_nMoveX, m_nMoveY;
        public UnE.Geometry.Vertex2D MovedVertex
        {
            get { return m_vMove;  }
        }
       
        Timer timerMouseWheel = new Timer();
        Timer m_WheelTimer = new Timer();

        private bool m_bProcessWheel = false;

        protected bool m_useMouseWheel = true;

        private bool m_bAntiAliasing = false;

        public bool AntiAliasing
        {
            get { return m_bAntiAliasing; }
            set { m_bAntiAliasing = value; }
        }


        protected List<IDrawableShape> m_ListEntities = new List<IDrawableShape>();
        public void AddDXEntity(IDrawableShape shape)
        {
            shape.CreateDXResource();
            m_ListEntities.Add(shape);

            //long size = System.Diagnostics.Process.GetCurrentProcess().WorkingSet64;
            //string sizeStr = GetMemorySize(size);
            //System.Diagnostics.Trace.WriteLine(string.Format("{0} - {1}", "사용메모리", sizeStr));

            //System.GC.Collect();
        }

        public void RemoveDXEntity(IDrawableShape shape)
        {
            shape.DiscardDXResource();
            m_ListEntities.Remove(shape);
        }

        public float EditBoxLength
		{
			get
            {
                return m_fEditBoxLength; 
            }
		}

		public System.Drawing.SolidBrush EditBoxBrush
		{
			get
            {
                return m_brushEditBox; 
            }
		}

		public  System.Drawing.Pen EditBoxPen
		{
		    get
            {
                return m_penEditBox; 
            }
		}

        public System.Drawing.Pen SelectedBrightPen1
        {
            get
            {
                if (m_penSelectedBright1 == null)
                {
                    m_penSelectedBright1 = new System.Drawing.Pen(System.Drawing.Color.FromArgb(255 - this.BackColor.R, 255 - this.BackColor.G, 255 - this.BackColor.B));
                }
                return m_penSelectedBright1;
            }
        }

        public System.Drawing.Pen SelectedBrightPen2
        {
            get
            {
                if (m_penSelectedBright2 == null)
                {
                    m_penSelectedBright2 = new System.Drawing.Pen(BackColor);
                }
                return m_penSelectedBright2;
            }
        }

        public DXFControl() : base()
        {
            InitializeComponent();

            m_fEditBoxLength = 4.0f;
            m_brushEditBox = new SolidBrush(System.Drawing.Color.White);
            m_penEditBox = new Pen(m_brushEditBox, 1.0f);

            EntityFactory factory = new EntityFactory(this);
            SetShapeFactory(factory);
        }

        public bool OpenDXF(string strPath)
        {
            DXFDotNet.DXFLoader loader = new DXFDotNet.DXFLoader(this, m_arrLayer);

            //loader.UseLastViewport = m_useLastViewPort;

            m_isOpened = loader.Load(strPath);
            if (m_isOpened == true)
            {
                double dViewportWeight = 1.0;
                if (!m_useLastViewport)
                {
                    if (m_vObjectCenter.x <= ObjectTL.x + UnE.Geometry.Math.HALF_TOLERANCE() ||
                        m_vObjectCenter.y <= ObjectBR.y + UnE.Geometry.Math.HALF_TOLERANCE())
                    {
                        m_vObjectCenter.x = (ObjectTL.x + ObjectBR.x) / 2;
                        m_vObjectCenter.y = (ObjectTL.y + ObjectBR.y) / 2;
                    }
                    double weight1 = Size.Width * 0.85 / ((m_vObjectCenter.x - ObjectTL.x) * 2);
                    double weight2 = Size.Height * 0.85 / ((m_vObjectCenter.y - ObjectBR.y) * 2);
                    dViewportWeight = weight1 < weight2 ? weight1 : weight2;
                }

                UnE.Geometry.Vertex2D vCenter = new UnE.Geometry.Vertex2D(m_vObjectCenter.x, m_vObjectCenter.y);

                double dMoveX = -m_vObjectTL.x;
                double dMoveY = -m_vObjectBR.y;

                vCenter.x += dMoveX;
                vCenter.y += dMoveY;

                m_vObjectCenter.x += dMoveX;
                m_vObjectCenter.y += dMoveY;

                MoveAll(dMoveX, dMoveY);
                SetViewportCenter(m_vObjectCenter);
                Zoom(dViewportWeight, m_vObjectCenter, true);


                ReleaseDevice();
                CreateDevice();
            }

            base.Refresh();
            return m_isOpened;
        }

        public void CloseDXF()
        {
            m_isOpened = false;

            m_pCurrentLayer = null;
            m_pCurrentBlock = null;

            m_arrLayer.Clear();
            m_arrBlock.Clear();

            m_currentMatrix = null;
            m_isPanning = false;
        }

        // Y축이 화면 아래에서 위쪽으로 증가하는 방향인가?
        public override bool DownToTop()
        {
	        return true;
        }

        protected void OnLoad(object sender, EventArgs e)
        {
            InitSize();
        }

        public void InitSize()
        {
            int nWidth = this.Size.Width;
            int nHeight = this.Size.Height;
            //CreateImage(nWidth, nHeight);

            m_vViewportTL.x = m_vViewportBL.x = 0.0;
            m_vViewportTL.y = 0.0;
            m_vViewportBL.y = nHeight;
            m_vViewportBR.x = nWidth;
            m_vViewportBR.y = m_vViewportBL.y;

            m_dViewportWeight = 1.0;

            Reshape(nWidth, nHeight);
        }

        protected void ReshapeGDI(int nWidth, int nHeight)
        {
            if (nWidth <= 0 || nHeight <= 0)
                return;

            float m11 = (float)((m_vViewportBR.x - m_vViewportBL.x) / nWidth);
            float m21 = (float)((m_vViewportBL.x - m_vViewportTL.x) / nHeight);
            float dx = (float)m_vViewportTL.x;
            float m12 = (float)((m_vViewportBR.y - m_vViewportBL.y) / nWidth);
            float m22 = (float)((m_vViewportBL.y - m_vViewportTL.y) / nHeight);
            float dy = (float)m_vViewportTL.y;

            m_currentInverseMatrix = new System.Drawing.Drawing2D.Matrix(m11, m12, m21, m22, dx, dy);
            m_currentMatrix = m_currentInverseMatrix.Clone();

            try
            {
                m_currentMatrix.Invert();
            }
            catch (System.ArgumentException)
            {
                m_currentMatrix = new System.Drawing.Drawing2D.Matrix(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);
            }
        }

        protected void Reshape(int nWidth, int nHeight)
        {
            ReshapeGDI(nWidth, nHeight);
        }


        public override UnE.Geometry.Vertex2D ScreenToGlobal(int x, int y)
        {
            UnE.Geometry.Vertex2D vResult = null;
            if (m_currentInverseMatrix == null)
                return null;

            vResult = new UnE.Geometry.Vertex2D();
            vResult.x = m_currentInverseMatrix.Elements[0] * x + m_currentInverseMatrix.Elements[2] * y + m_currentInverseMatrix.Elements[4];
            vResult.y = m_currentInverseMatrix.Elements[1] * x + m_currentInverseMatrix.Elements[3] * y + m_currentInverseMatrix.Elements[5];

            return vResult;
        }

        public override System.Drawing.Point GlobalToScreen(UnE.Geometry.Vertex2D vertex)
        {
            System.Drawing.Point ptResult = new System.Drawing.Point();
            if (m_currentMatrix == null)
                return ptResult;

            ptResult.X = (int)(m_currentMatrix.Elements[0] * vertex.x + m_currentMatrix.Elements[2] * vertex.y + m_currentMatrix.Elements[4]);
            ptResult.Y = (int)(m_currentMatrix.Elements[1] * vertex.x + m_currentMatrix.Elements[3] * vertex.y + m_currentMatrix.Elements[5]);

            return ptResult;
        }

        protected void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == m_btnPanning)
            {
                m_isPanning = true;
                m_ptPanningOrigin.X = e.X;
                m_ptPanningOrigin.Y = e.Y;
                //m_nMoveX = m_nMoveY = 0;
                m_vOriginCenter = GetViewportCenter();

            }
        }

        protected void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == m_btnPanning)
            {
                m_isPanning = false;

                // 화면이 이동되었으니 ScreenImage를 새로 만든다.
                m_nMoveX = m_nMoveY = 0;

                _Refresh();
            }
        }

        void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_isPanning)
            {
                m_nMoveX = e.X - m_ptPanningOrigin.X;
                m_nMoveY = e.Y - m_ptPanningOrigin.Y;

                UnE.Geometry.Vertex2D vNewCenter
                    = new UnE.Geometry.Vertex2D(m_vOriginCenter.x - m_nMoveX / m_dViewportWeight, m_vOriginCenter.y + m_nMoveY / m_dViewportWeight);

                SetViewportCenter(vNewCenter);

                base.Refresh();
            }
        }

        protected void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (m_isPanning == true)
                return;

            if (!m_useMouseWheel)
                return;

            UnE.Geometry.Vertex2D vCurrent = ScreenToGlobal(e.X, e.Y);
            if (vCurrent == null)
                return;

            m_bProcessWheel = true;

            m_dtLastMouseWheel = DateTime.Now;

            //timerMouseWheel.Enabled = true;
            //timerMouseWheel.Start();


            m_img = null;

            double dZoomValue = m_dViewportWeight;
            if (e.Delta < 0)
            {
                dZoomValue *= 0.9;
                if (dZoomValue < 0.0001)
                    dZoomValue = 0.0001;
            }
            else
                dZoomValue /= 0.9;

            //if (mDrawSpan != null && mDrawSpan.TotalMilliseconds > 150)
            //{

            //    if (m_WheelTimer == null || m_WheelTimer.Enabled == false)
            //    {
            //        if (m_WheelTimer == null)
            //        {
            //            m_WheelTimer = new Timer();
            //            m_WheelTimer.Interval = 600;
            //            m_WheelTimer.Tick += new EventHandler(OnWheelTimerTick);
            //        }
            //        m_WheelTimer.Enabled = true;
            //        m_WheelTimer.Start();
            //    }

            //    m_bDrawText = false;

            //    Zoom(dZoomValue, vCurrent, false);
            //}
            //else
            {
                Zoom(dZoomValue, vCurrent, true);
            }

            m_bProcessWheel = false;
        }

        protected void OnWheelTimerTick(object sender, EventArgs e)
        {

        }


        public void SetViewportCenter(UnE.Geometry.Vertex2D vCenter)
        {
            double x = this.Size.Width / m_dViewportWeight / 2;
            double y = this.Size.Height / m_dViewportWeight / 2;

            m_vViewportTL.x = vCenter.x - x;
            m_vViewportTL.y = vCenter.y + y;
            m_vViewportBL.x = vCenter.x - x;
            m_vViewportBL.y = vCenter.y - y;
            m_vViewportBR.x = vCenter.x + x;
            m_vViewportBR.y = vCenter.y - y;

            Reshape(this.Size.Width, this.Size.Height);
        }

        public UnE.Geometry.Vertex2D GetViewportCenter()
        {
            return (m_vViewportTL + m_vViewportBR) / 2;
        }

        public double GetViewportWeight()
        {
            return m_dViewportWeight;
        }

        public override void Refresh()
        {
            //base.Refresh();
        }

        public void SetViewportWeight(double dWeight)
        {
            if (dWeight <= UnE.Geometry.Math.HALF_TOLERANCE())
                return;

            m_dViewportWeight = dWeight;

            if (this.Size.Width > 0 && this.Size.Height > 0)
            {
                UnE.Geometry.Vertex2D vCenter = GetViewportCenter();
                double x = this.Size.Width / dWeight / 2;
                double y = this.Size.Height / dWeight / 2;

                m_vViewportTL.x = vCenter.x - x;
                m_vViewportTL.y = vCenter.y + y;
                m_vViewportBL.x = vCenter.x - x;
                m_vViewportBL.y = vCenter.y - y;
                m_vViewportBR.x = vCenter.x + x;
                m_vViewportBR.y = vCenter.y - y;

                Reshape(this.Size.Width, this.Size.Height);
            }
        }

        public int GetScreenWidth()
        {
            return this.Size.Width;
        }

        public int GetScreenHeight()
        {
            return this.Size.Height;
        }

        public void Zoom(double dZoomValue, UnE.Geometry.Vertex2D vZoomCenter, bool refresh)
        {
            // 이 이상 넘어가면... 죽는다.
            if (dZoomValue > 7.0 || dZoomValue <= UnE.Geometry.Math.HALF_TOLERANCE())
                return;

            // vZoomCenter에 해당하는 화면좌표1 얻어오기
            System.Drawing.Point ptZoomCenter = GlobalToScreen(vZoomCenter);

            double left = ptZoomCenter.X / dZoomValue;
            double top = ptZoomCenter.Y / dZoomValue;
            double right = (ptZoomCenter.X - this.Size.Width) / dZoomValue;
            double bottom = (ptZoomCenter.Y - this.Size.Height) / dZoomValue;

            m_vViewportTL.x = vZoomCenter.x - left;
            m_vViewportTL.y = vZoomCenter.y + top;
            m_vViewportBL.x = vZoomCenter.x - left;
            m_vViewportBL.y = vZoomCenter.y + bottom;
            m_vViewportBR.x = vZoomCenter.x - right;
            m_vViewportBR.y = vZoomCenter.y + bottom;

            m_dViewportWeight = dZoomValue;

            Reshape(this.Size.Width, this.Size.Height);


            if (refresh)
            {
                base.Refresh();
            }
        }


        DXFDotNet.Shape SelectObject(double x, double y)
        {
            foreach (Layer pLayer in m_arrLayer)
            {
                if (pLayer.Hidden)
                    continue;

                DXFDotNet.Shape shape = pLayer.SelectObject(x, y);
                if (shape != null)
                    return shape;
            }

            return null;
        }

        DXFDotNet.Shape PickObject(double x, double y)
        {
            DXFDotNet.Shape shape = SelectObject(x, y);

            if (shape != null)
                shape.Selected = true;

            return shape;
        }

        void PickObject(Shape shape)
        {
            if (shape == null)
                return;

            shape.Selected = true;
        }

        // 모든 객체들을 현재의 위치로부터 (x, y) 만큼 이동시킨다.
        public void MoveAll(double x, double y)
        {
            foreach (Layer pLayer in m_arrLayer)
            {
                pLayer.MoveAll(x, y);
            }

            m_vMove.x = x;
            m_vMove.y = y;
        }

        public void SetEditBoxColor(System.Drawing.Color color, bool isFill)
        {
            if (isFill)
                m_brushEditBox.Color = color;
            else
                m_penEditBox.Color = color;
        }

        public System.Drawing.Color GetColor(bool isFill)
        {
            return isFill ? m_brushEditBox.Color : m_penEditBox.Color;
        }

        public void SetEditBoxSize(int nLen)
        {
            m_nEditBoxSize = nLen;
        }

        public int GetEditBoxSize()
        {
            return m_nEditBoxSize;
        }



        protected void CalcShapeGroup()
        {
            foreach (Layer layer in m_arrLayer)
            {
                layer.CalcGroup(m_nGroupItemMinCount, m_nGroupItemDistance);
            }
        }

        public System.Drawing.Color GetBackColor()
        {
            return this.BackColor;
        }

       

        public LineType GetSelectedLineType()
        {
            return m_lineTypeSelected;
        }

        public void SaveHomeMatrix()
        {
            if (m_currentMatrix == null)
                return;

            m_fHomem11 = m_currentMatrix.Elements[0];
            m_fHomem12 = m_currentMatrix.Elements[1];
            m_fHomem21 = m_currentMatrix.Elements[2];
            m_fHomem22 = m_currentMatrix.Elements[3];
            m_fHomedx = m_currentMatrix.Elements[4];
            m_fHomedy = m_currentMatrix.Elements[5];

            m_vHomeViewportTL = new UnE.Geometry.Vertex2D(m_vViewportTL.x, m_vViewportTL.y);
            m_vHomeViewportBL = new UnE.Geometry.Vertex2D(m_vViewportBL.x, m_vViewportBL.y);
            m_vHomeViewportBR = new UnE.Geometry.Vertex2D(m_vViewportBR.x, m_vViewportBR.y);

            m_dHomeViewportWeight = m_dViewportWeight;
        }

        public void LoadHomeMatrix(bool refresh)
        {
            if (m_vHomeViewportTL == null)
                return;

            m_vViewportTL.x = m_vHomeViewportTL.x;
            m_vViewportTL.y = m_vHomeViewportTL.y;
            m_vViewportBL.x = m_vHomeViewportBL.x;
            m_vViewportBL.y = m_vHomeViewportBL.y;
            m_vViewportBR.x = m_vHomeViewportBR.x;
            m_vViewportBR.y = m_vHomeViewportBR.y;

            m_dViewportWeight = m_dHomeViewportWeight;

            m_currentMatrix = new System.Drawing.Drawing2D.Matrix(m_fHomem11, m_fHomem12, m_fHomem21, m_fHomem22, m_fHomedx, m_fHomedy);

            m_currentInverseMatrix = m_currentMatrix.Clone();

            try
            {
                m_currentInverseMatrix.Invert();
            }
            catch (System.ArgumentException)
            {
                m_currentInverseMatrix = null;
            }

            if (refresh)
                Refresh();
        }

        public DXFDotNet.Viewport GetViewport()
        {
            if (m_currentMatrix == null)
                return null;

            DXFDotNet.Viewport viewport = new DXFDotNet.Viewport();

            viewport.F11 = m_currentMatrix.Elements[0];
            viewport.F12 = m_currentMatrix.Elements[1];
            viewport.F21 = m_currentMatrix.Elements[2];
            viewport.F22 = m_currentMatrix.Elements[3];
            viewport.FDx = m_currentMatrix.Elements[4];
            viewport.FDy = m_currentMatrix.Elements[5];

            viewport.TopLeft = new UnE.Geometry.Vertex2D(m_vViewportTL.x, m_vViewportTL.y);
            viewport.BottomLeft = new UnE.Geometry.Vertex2D(m_vViewportBL.x, m_vViewportBL.y);
            viewport.BottomRight = new UnE.Geometry.Vertex2D(m_vViewportBR.x, m_vViewportBR.y);

            viewport.Weight = m_dViewportWeight;
            return viewport;
        }

        public void LoadViewport(DXFDotNet.Viewport viewport, bool refresh)
        {
            if (viewport == null)
                return;

            m_vViewportTL.x = viewport.TopLeft.x;
            m_vViewportTL.y = viewport.TopLeft.y;
            m_vViewportBL.x = viewport.BottomLeft.x;
            m_vViewportBL.y = viewport.BottomLeft.y;
            m_vViewportBR.x = viewport.BottomRight.x;
            m_vViewportBR.y = viewport.BottomRight.y;

            m_dViewportWeight = viewport.Weight;

            m_currentMatrix = new System.Drawing.Drawing2D.Matrix(viewport.F11, viewport.F12, viewport.F21, viewport.F22, viewport.FDx, viewport.FDy);

            m_currentInverseMatrix = m_currentMatrix.Clone();

            try
            {
                m_currentInverseMatrix.Invert();
            }
            catch (System.ArgumentException)
            {
                m_currentInverseMatrix = null;
            }
            
            if (refresh)
                Refresh();
        }

        SharpDX.RectangleF m_clipRect = new SharpDX.RectangleF();
        public SharpDX.RectangleF ClipRect
        {
            get { return m_clipRect; }
            set { m_clipRect = value; }
        }

        protected SharpDX.Matrix3x2 mat = new SharpDX.Matrix3x2();
        protected void OnPaint(object sender, PaintEventArgs e)
        {

            // Create a bitmap.
//RenderTarget.CreateBitmap(size, nullptr, 0,
//    D2D1::BitmapProperties(
//        D2D1_BITMAP_OPTIONS_TARGET,
//        D2D1::PixelFormat(
//            DXGI_FORMAT_B8G8R8A8_UNORM,
//            D2D1_ALPHA_MODE_PREMULTIPLIED),
//        dpiX, dpiY),
//    &sceneBitmap);

//// Preserve the pre-existing target.
//ComPtr<ID2D1Image> oldTarget;
//m_d2dContext->GetTarget(&oldTarget);

//// Render static content to the sceneBitmap.
//m_d2dContext->SetTarget(sceneBitmap.Get());
//m_d2dContext->BeginDraw();

//m_d2dContext->EndDraw();

//// Render sceneBitmap to oldTarget.
//m_d2dContext->SetTarget(oldTarget.Get());
//m_d2dContext->DrawBitmap(sceneBitmap.Get());


            DateTime dt = DateTime.Now;

            if (BeginPaint != null)
                BeginPaint(true);

            RenderTarget.BeginDraw();
            
            SharpDX.Color4 color = new SharpDX.Color4(0.0f, 0.05f, 0.2f,1.0f);
            RenderTarget.Clear(color);

            mat.M11 = m_currentMatrix.Elements[0];
            mat.M12 = m_currentMatrix.Elements[1];
            mat.M21 = m_currentMatrix.Elements[2];
            mat.M22 = m_currentMatrix.Elements[3];
            mat.M31 = m_currentMatrix.Elements[4];
            mat.M32 = m_currentMatrix.Elements[5];

           

            // Clip영역 지정. Add By skkim 2015.02.25
            // 현재 화면의 TL, BL을 구한다.
            UnE.Geometry.Vertex2D v3 = ScreenToGlobal(0, 0);
            UnE.Geometry.Vertex2D v4 = ScreenToGlobal(this.Size.Width, this.Size.Height);

            // Global에서의 현재화면의 Rect를 구한다.
            float fMaxX = (float)System.Math.Max(v3.x, v4.x);
            float fMinX = (float)System.Math.Min(v3.x, v4.x);
            float fMaxY = (float)System.Math.Max(v3.y, v4.y);
            float fMinY = (float)System.Math.Min(v3.y, v4.y);
            float fWidth = fMaxX - fMinX;
            float fHeight = fMaxY - fMinY;

            ClipRect = new SharpDX.RectangleF(
                fMinX, fMinY, fWidth, fHeight
               );
            // Layer의 Clip Bound를 설정
            mLayerParam.ContentBounds = ClipRect;
            d2dRenderTarget.PushLayer(ref mLayerParam, layer);
            d2dRenderTarget.PushAxisAlignedClip(ClipRect, SharpDX.Direct2D1.AntialiasMode.PerPrimitive);
            // 현재화면의 Rect를 Clip영역으로 지정한다.
            //System.Drawing.Region region = new System.Drawing.Region(rect);
            m_penEditBox.Color = System.Drawing.Color.White;

            if( m_bAntiAliasing == true)
            {
                RenderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            }
            else
            {
                RenderTarget.AntialiasMode = AntialiasMode.Aliased;
            }
            RenderTarget.Transform = mat;

            //System.Diagnostics.Trace.WriteLine("Begin Draw");
            try
            {
                UnE.Geometry.Vertex2D v1 = ScreenToGlobal(0, 0);
                UnE.Geometry.Vertex2D v2 = ScreenToGlobal(m_nEditBoxSize, 0);
                m_fEditBoxLength = (float)v1.GetDistance(v2);


                //IntPtr hDC = gdiRenderTarget.GetDC(DeviceContextInitializeMode.Copy);
                //Graphics graphics = Graphics.FromHdc(hDC);
                //if (ExternalPainter != null)
                //{
                //    //m_externPainter.OnPrevPaint(e);
                //    ExternalPainter.OnPrevPaint(graphics, m_bDrawText);
                //}
                //graphics.Dispose();
                //gdiRenderTarget.ReleaseDC();

               
                if (DrawHatchFirst)
                {                   
                    foreach (Layer hatchLayer in m_arrLayer)
                    {     
                        hatchLayer.DrawShapeByType(RenderTarget, m_bDrawText, Shape.ShapeType.HATCH);
                    }

                    foreach (Layer exceptHatch in m_arrLayer)
                    {
                        exceptHatch.DrawExceptHatchNText(RenderTarget, m_bDrawText);                        
                    }

                    foreach (Layer textLayer in m_arrLayer)
                    {
                        textLayer.DrawShapeByType(RenderTarget, m_bDrawText, Shape.ShapeType.TEXT);
                    }  
                }
                else
                {
                    foreach (Layer normLayer in m_arrLayer)
                    {
                        normLayer.DrawShapeExcludeByType(RenderTarget, m_bDrawText, Shape.ShapeType.TEXT);
                    }

                    foreach (Layer textLayer in m_arrLayer)
                    {
                        textLayer.DrawShapeByType(RenderTarget, m_bDrawText, Shape.ShapeType.TEXT);
                    } 
                }
               
                
                //IntPtr hDC2 = gdiRenderTarget.GetDC(DeviceContextInitializeMode.Copy);
                //Graphics graphics2 = Graphics.FromHdc(hDC);
                //if (ExternalPainter != null)
                //{
                //    ExternalPainter.OnPostPaint(graphics2, m_bDrawText);
                //}
                //graphics2.Dispose();
                //gdiRenderTarget.ReleaseDC();
                                
                //System.Drawing.Drawing2D.Matrix oldMatrix2 = e.Graphics.Transform.Clone();
                //e.Graphics.ResetTransform();
                //e.Graphics.DrawImage(m_img, 0, 0);
                //e.Graphics.Transform = oldMatrix2;                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }

            d2dRenderTarget.PopAxisAlignedClip();
            d2dRenderTarget.PopLayer();
            DateTime dt2 = DateTime.Now;      
      
            RenderTarget.EndDraw();
            TimeSpan span = dt2 - dt;
           // System.Diagnostics.Trace.WriteLine(span.Milliseconds + "ms");
            
            swapChain.Present(0, PresentFlags.None);
            
            
            if (EndPaint != null)
                this.EndPaint(true);
           
        }

        #region 현재 쓰래드의 메모리 사용량
       
        enum MemorySizeType
        {
            Byte, KByte, MByte, GByte, TByte
        }

        private string GetMemorySize(Int64 usageMemory)
        {
            String retStr = String.Empty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int i = 0;

            while (usageMemory > 1024L)
            {
                usageMemory = (Int64)(usageMemory / 1024L);
                i++;
            }

            MemorySizeType sizeType = (MemorySizeType)i;

            sb.AppendFormat("{0}{1}", usageMemory, sizeType.ToString());
            retStr = sb.ToString();

            return retStr;
        }
        #endregion 

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        //void DXFControl.OnPrintPage(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
        //{
        //    int width = this.Size.Width;
        //    int height = this->Size.Height;
        //    if (width == 0 || height == 0)
        //        return;

        //    DXFDotNet.UPrintDocument document = (DXFDotNet.UPrintDocument)sender;
        //    if (document == null)
        //        return;

        //    // Get Document Scale
        //    double a = document->Length;
        //    double b = document->UnitValue;
        //    double t = 25.4;
        //    if (document->LengthOfUnit == LengthUnit.mm)
        //    {
        //        t = 1.0;
        //    }

        //    b = System.Math.Round(b * t, 5);
        //    if (b == 0)
        //        return;

        //    float m_fScale = (float)(a / b);
        //    System.Drawing.Rectangle page = e->MarginBounds;


        //    // Create Back Image
        //    System.Drawing.Rectangle clipRect = new System.Drawing.Rectangle(0, 0, width, height);
        //    Bitmap backImage = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
        //    Graphics gBack = Graphics.FromImage(backImage);


        //    // Draw Content on Image
        //    bool bTemp = m_useAntialiasing;
        //    m_useAntialiasing = true;

        //    //OnPrint(this, new System.Windows.Forms.PaintEventArgs(gBack, *clipRect));

        //    m_useAntialiasing = bTemp;

        //    Bitmap rectImage = null;
        //    if (document->WindowPrintMode == true)
        //    {

        //        rectImage = backImage->Clone(*(document->DrawingRectSize), backImage->PixelFormat);
        //    }

        //    if (rectImage != null)
        //        backImage = rectImage;


        //    e->Graphics->ResetTransform();

        //    float offsetX = (float)document->OffsetX;
        //    float offsetY = (float)document->OffsetY;

        //    bool bUpsideDown = document->UpsideDown;
        //    if (bUpsideDown == true)
        //    {
        //        float fWidth = e->PageBounds.Width * 0.5f;
        //        float fHeight = e->PageBounds.Height * 0.5f;
        //        e->Graphics->TranslateTransform(fWidth, fHeight);
        //        e->Graphics->RotateTransform(180.0f);
        //        e->Graphics->TranslateTransform(-fWidth, -fHeight);

        //    }


        //    // 문서의 Margin을 Draw영역에서 제외한다.
        //    System.Drawing.Region region = new System.Drawing.Region(e->MarginBounds);
        //    e->Graphics->Clip = region;
        //    // 이미지의 사이즈를 구한다.
        //    System.Drawing.Size imageSize = new System.Drawing.Size(width, height);

        //    if (document->PrintOnCenter == true)
        //    {
        //        // 화면 스케일을 적용
        //        e->Graphics->ScaleTransform(m_fScale, m_fScale);

        //        // Scale에 따른 크기 변화랑을 Position에 적용한다.
        //        float dx = (imageSize->Width / m_fScale - imageSize->Width) * 0.5f;
        //        float dy = (imageSize->Height / m_fScale - imageSize->Height) * 0.5f;

        //        // 이미지가 중심에 오도록 Scale이 적용된 Image의 크기를 고려하여 Position을 구한다.
        //        float transX = dx + ((page->Width - imageSize->Width) * 0.5f + page->Location.X) / m_fScale;
        //        float transY = dy + ((page->Height - imageSize->Height) * 0.5f + page->Location.Y) / m_fScale;

        //        RectangleF imgRect = new RectangleF(transX, transY, (float)imageSize->Width, (float)imageSize->Height);
        //        e->Graphics->DrawImage(backImage, *imgRect);
        //        //e->Graphics->TranslateTransform(transX, transY);
        //    }
        //    else
        //    {
        //        // 화면 스케일을 적용
        //        e->Graphics->ScaleTransform(m_fScale, m_fScale);

        //        // Scale에 따른 크기 변화랑을 Position에 적용한다.
        //        float dx = (offsetX / m_fScale - offsetX) * 0.5f;
        //        float dy = (offsetY / m_fScale - offsetY) * 0.5f;

        //        // 이미지가 Offset 위치에 오도록 Scale을 고려하여 Position을 구한다.
        //        float transX = dx + (offsetX + page->Location.X) / m_fScale;
        //        float transY = dy + (offsetY + page->Location.Y) / m_fScale;

        //        RectangleF imgRect = new RectangleF(transX, transY, (float)imageSize->Width, (float)imageSize->Height);
        //        e->Graphics->DrawImage(backImage, *imgRect);

        //        //e->Graphics->TranslateTransform(transX, transY);			
        //    }

        //    //OnPaint(this, new System.Windows.Forms.PaintEventArgs(e->Graphics, *clipRect));
        //}

        void OnSize(object sender, EventArgs e)
        {
            int nWidth = this.Size.Width;
            int nHeight = this.Size.Height;

            if (nWidth == 0 || nHeight == 0)
                return;

            //SizeImage(nWidth, nHeight);

            Reshape(nWidth, nHeight);

            UPrintDocument doc = (UPrintDocument)mPrintDocument;
            if (doc != null)
                doc.DrawingSize = new Size(nWidth, nHeight);

            m_img = null;
            //m_makeImage = true;

            ReleaseDevice();
            CreateDevice();

            Refresh();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DXFContorl
            // 
            this.Name = "DXFContorl";
            this.Size = new System.Drawing.Size(522, 370);
            this.Load += new System.EventHandler(this.OnLoad);
            this.SizeChanged += new System.EventHandler(this.OnSize);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
            this.Resize += new System.EventHandler(this.OnSize);
            this.MouseWheel += new MouseEventHandler(this.OnMouseWheel);
            this.Paint += new PaintEventHandler(this.OnPaint);
            
            this.ResumeLayout(false);

        }

        protected void ReleaseDevice()
        {
            if (m_bInitDevice == false)
                return;

            stopwatch.Stop();
            if (renderView != null)
                renderView.Dispose();

            if (backBuffer!= null)
                backBuffer.Dispose();

            if (device != null)
            {
                
                device.ClearState();
              
            }

            if (swapChain != null)
            {
                swapChain.Dispose();
            }

            if (factory != null)
            {
                factory.Dispose();
            }

            if( mFactoryDWrite != null)
            {
                mFactoryDWrite.Dispose();
            }
            
            foreach(IDrawableShape shape in m_ListEntities)
            {
                if (shape!= null)
                    shape.DiscardDXResource();
            }

            m_bInitDevice = false;
        }

        protected override void Dispose(bool A_0)
        {
            base.Dispose(A_0);
            //ReleaseDevice();
        }       

        protected void CreateDevice()
        {
            if (m_bInitDevice == true)
                return;
            
            desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(ClientSize.Width, ClientSize.Height,
                                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput 
            };

            
            // Create Device and SwapChain          
            Device1.CreateWithSwapChain(DriverType.Hardware,DeviceCreationFlags.BgraSupport, desc, FeatureLevel.Level_10_0, out device, out swapChain);
            
            // Default Factory
            var d2dFactory = new SharpDX.Direct2D1.Factory();
            
            int width = ClientSize.Width;
            int height = ClientSize.Height;

            // Ignore all windows events
            factory = swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(Handle, WindowAssociationFlags.IgnoreAll);
  
            // New RenderTargetView from the backbuffer
            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);

            HwndRenderTargetProperties wtp = new HwndRenderTargetProperties();
            wtp.Hwnd = Handle;
            wtp.PixelSize = new Size2(ClientSize.Width, ClientSize.Height);
            wtp.PresentOptions = PresentOptions.Immediately;
            

            renderView = new RenderTargetView(device, backBuffer);
            using( Surface surface = backBuffer.QueryInterface<Surface>())
            {
                //RenderTargetProperties rtProp = new RenderTargetProperties(RenderTargetType.Default, new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied), 0, 0, RenderTargetUsage.GdiCompatible, SharpDX.Direct2D1.FeatureLevel.Level_10);

                //d2dRenderTarget = new WindowRenderTarget(d2dFactory, rtProp, wtp );
                d2dRenderTarget = new RenderTarget(d2dFactory, surface, new RenderTargetProperties(new PixelFormat(Format.Unknown, AlphaMode.Premultiplied)));
                //gdiRenderTarget = d2dRenderTarget.QueryInterface<GdiInteropRenderTarget>();
                //SharpDX.Direct2D1.Bitmap bitmap = new SharpDX.Direct2D1.Bitmap();
               
            }

            d2dRenderTarget.AntialiasMode = AntialiasMode.Aliased;
            d2dRenderTarget.TextAntialiasMode = TextAntialiasMode.Cleartype;
            
            // Create DirectWrite Factory
            mFactoryDWrite = new SharpDX.DirectWrite.Factory(SharpDX.DirectWrite.FactoryType.Shared);           

            solidColorBrush = new SolidColorBrush(d2dRenderTarget, SharpDX.Color.White);
            
            // Create PreDefine DX Resource
            foreach (IDrawableShape shape in m_ListEntities)
            {
                shape.CreateDXResource();
            }

            // Create Layer Parameter
            mLayerParam = new LayerParameters();
            mLayerParam.LayerOptions = LayerOptions.InitializeForCleartype;
            mLayerParam.MaskAntialiasMode = AntialiasMode.Aliased;
            mLayerParam.MaskTransform = SharpDX.Matrix3x2.Identity;
            mLayerParam.Opacity = 1.0f;

            layer = new SharpDX.Direct2D1.Layer(d2dRenderTarget);

            m_bInitDevice = true;
            stopwatch.Start();
        }



    }

}

