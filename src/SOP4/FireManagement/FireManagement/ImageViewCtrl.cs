using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;

namespace FireManagement
{

    public enum MouseWorkMode { NONE = 0, PICK, PANNING, ORBIT, NEW_FIRE_SENSOR, NEW_COOLER_SENSOR, NEW_PRESSURE_SENSOR, NEW_CCTV, DEL_FACILITY };

    public enum MouseEvent { MOUSE_DOWN = 0, MOUSE_UP, MOUSE_MOVE };

    public partial class ImageViewCtrl : Label
	{

        UnE.Geometry.Vertex2D m_MovedVertex = new UnE.Geometry.Vertex2D();
        public UnE.Geometry.Vertex2D MovedVertex
        {
            get { return m_MovedVertex; }
        }
        public Shape PickObject(double x, double y)
        {
            foreach(Layer layer in m_Layers)
            {
                Shape shape = layer.PickShape((float)x, (float)y);
                if (shape != null)
                    return shape;
            }
            return null;
        }

        private Matrix mTransform = new Matrix();
        public Matrix Transform
        {
            get { return mTransform; }
        }

        private PointF mPtTranslation = new PointF(0.0f, 0.0f);
        public PointF PtTranslation
        {
            get { return mPtTranslation; }
        }

        private float[] mScaleList = 
        {
            7.125f, 7.0f, 6.875f, 6.75f, 6.625f, 6.5f, 6.375f, 6.25f, 6.125f, 6.0f, 5.875f, 5.75f, 5.625f, 
            5.5f, 5.375f, 5.25f, 5.125f, 5.0f, 4.875f, 4.75f, 4.625f, 4.5f, 4.375f, 4.25f, 4.125f, 4.0f, 3.875f, 3.75f, 3.625f, 
            3.5f, 3.375f, 3.25f, 3.125f, 3.0f, 2.875f, 2.75f, 2.625f, 2.5f, 2.375f, 2.25f, 2.125f, 2.0f, 1.875f, 1.75f, 1.625f,
            1.5f, 1.375f, 1.25f, 1.125f , 1.0f, 0.8888889f, 0.8f, 0.7272727f, 0.6666667f, 0.6153846f, 0.5714286f, 0.5333334f,
            0.5f, 0.4705882f, 0.4444444f, 0.4210526f, 0.4f, 0.3809524f, 0.3636364f, 0.3478261f, 0.3333333f, 0.32f, 0.3076923f, 0.2962963f, 
            0.2857143f, 0.2758621f, 0.2666667f, 0.2580645f, 0.25f, 0.2424242f, 0.2352941f, 0.2285714f, 0.2222222f, 0.2162162f, 0.2105263f, 
            0.2051282f, 0.2f, 0.1951219f, 0.1904762f, 0.1860465f, 0.1818182f, 0.1777778f, 0.173913f, 0.1702128f, 0.1666667f, 0.1632653f, 
            0.16f, 0.1568628f, 0.1538462f, 0.1509434f, 0.1481481f, 0.1454545f, 0.1428571f, 0.1403509f
        };

        private Image mBaseImage = null;
        public Image BaseImage
        {
            get { return mBaseImage; }          
        }

        private Size mSizeImage = new Size();
        public Size SizeImage
        {
            get { return mSizeImage; }
        }
        
        private ArrayList mBillBoardList = new ArrayList();

        // 마우스 드래그로 그려지는 사각형(Zoom)
        private Rectangle mRectDrawing = new Rectangle();

        // 이미지
        private Rectangle mRectImage = new Rectangle();

        // 이미지 중심점
        private Point mPtCenter = new Point();

        // 화면 중심점
        private PointF mPtGlobalCenter = new PointF();
        public PointF PtGlobalCenter
        {
            get { return mPtGlobalCenter; }
        }
                
        private bool mbDrag = false;
        
        private Point mPtPrev;
        private Point mPtDragStart;
        private Point mPtDragCurrent;
        private Point mPtCurrent;


        private bool m_bEditMode = false;
        public bool EditMode
        {
            get { return m_bEditMode; }
            set { m_bEditMode = value; }
        }


        private bool mbRotationMode = false;
        public bool RotationMode
        {
            get { return mbRotationMode; }
            set 
            {
                SetMode(value);
                mbRotationMode = value;
            }
        }

        private bool mbTranslateMode = false;
        private Timer timer1;
        
        public bool TranslateMode
        {
            get { return mbTranslateMode; }
            set 
            {
                SetMode(value);
                mbTranslateMode = value; 
            }
        }

        private bool bRectZoomMode = false;
        public bool RectZoomMode
        {
            get { return bRectZoomMode; }
            set 
            {
                SetMode(value);
                bRectZoomMode = value;
            }
        }

        private void SetMode(bool bFalse)
        {
            if( bFalse == true)
            {
                mbRotationMode = false;
                mbTranslateMode = false;
                bRectZoomMode = false;
            }
        }

        private int mBillboardWidth = 32;
        public int BillboardWidth
        {
            get { return mBillboardWidth; }
            set { mBillboardWidth = value; }
        }
        private int mBillboardHeight = 32;
        public int BillboardHeight
        {
            get { return mBillboardHeight; }
            set { mBillboardHeight = value; }
        }

        private bool m_bDrawBillBoard = true;
        public bool DrawBillBoard
        {
            get { return m_bDrawBillBoard; }
            set { m_bDrawBillBoard = value; }
        }


        private Zone m_currentIndoorZone = null;


        // Zone별 POI 리스트
        // Indoor View에서만 사용됨
        private Dictionary<Zone, ArrayList> m_dicZonePOIs = new Dictionary<Zone, ArrayList>();

        // Panning 또는 Orbit, Zoom In/Out 등의 동작을 위하여 임시로 숨겨놓은 POI Popup 창 리스트
        private ArrayList m_arrTemporaryHiddenPOIs = new ArrayList();

        private ArrayList m_arrLODShowingPOIs = new ArrayList();
        
        private Brush mBrushRect = null;
        private Pen mPenRect = null;
        private int m_nScaleIndex = 0;

        private int m_nShowTooltipX = 0;
        private int m_nShowTooltipY = 0;
        //private bool m_bShowTooltip = false;

        private Form m_formTooltip = null;
        private Timer m_TooltipTimer = new Timer();

        private void ImageView_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void ImageView_Resize(object sender, EventArgs e)
        {
            OnPanelResize();
        }

        private void ImageView_Paint(object sender, PaintEventArgs e)
        {
            OnPanelPaint(e);
        }

        private void ImageView_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(sender, e);
            Invalidate();
        }

        private void ImageView_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(sender, e);
            Invalidate();
        }

        private void ImageView_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(sender, e);
            Refresh();
        }

        public void ImageView_FitView(object sender, EventArgs e)
        {
            ResetTransform();
            FitView();
            Refresh();
        }
        private System.Windows.Forms.OpenFileDialog mOpenFileDialog;

        public ImageViewCtrl( )
		{
            this.DoubleBuffered = true;

     
            mBrushRect = new HatchBrush(HatchStyle.Sphere, Color.Blue, Color.LightGreen);
            mPenRect = new Pen(mBrushRect, 0);

            PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(OnPreviewKeyDown);
            //m_TooltipTimer.Tick += new EventHandler(OnShowTooltip);
            MouseLeave += new EventHandler(OnMouseLeave);

            this.SizeChanged += new System.EventHandler(this.ImageView_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ImageView_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ImageView_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageView_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ImageView_MouseUp);
            this.Resize += new System.EventHandler(this.ImageView_Resize);
		}

        public double GetViewportWeight()
        {
            return mTransform.Elements[3];
        }

        private void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //FormMain.Instance.EnableFireReportBtn(false);
        }


        private ArrayList m_Layers = new ArrayList();
        public ArrayList Layers
        {
            get { return m_Layers; }
            set { m_Layers = value; }
        }


        public bool SetImage(string szImagePath, Zone zone )
        {
            ResetTransform();

            if (mBaseImage != null)
                mBaseImage.Dispose();
            m_currentIndoorZone = zone;
            if (System.IO.File.Exists(szImagePath))
            {
                mBaseImage = Bitmap.FromFile(szImagePath);
                mSizeImage = new Size(mBaseImage.Width, mBaseImage.Height);
                mPtCenter = new Point((int)(Width * 0.5f), (int)(Height * 0.5f));
                mPtGlobalCenter = new Point(Width / 2, Height / 2);
                return true;
            }
            else
            {
                mBaseImage = null;
            }
            return false;
        }

        public bool SetImage(string szImagePath)
        {
            ResetTransform();
           
            if( System.IO.File.Exists(szImagePath))
            {
                mBaseImage = Bitmap.FromFile(szImagePath);
                mSizeImage = new Size(mBaseImage.Width, mBaseImage.Height);
                mPtCenter = new Point((int)(Width * 0.5f), (int)(Height * 0.5f));
                mPtGlobalCenter = new Point(Width / 2, Height / 2);
                return true;
            }
            else
            {
                mBaseImage = null;
            }
            return false;
        }

        public void SetImage(Image img)
        {           
            ResetTransform();

            mBaseImage = img;
            mSizeImage = new Size(mBaseImage.Width, mBaseImage.Height);
            mPtCenter = new Point((int)(Width * 0.5f), (int)(Height * 0.5f));
            mPtGlobalCenter = new Point(Width / 2, Height / 2);
        }

        public void ResetTransform()
        {
            mTransform.Reset();
            mTransform.Translate(mPtTranslation.X, mPtTranslation.Y);         
        }    
        
        public void FitView()
        {
            mPtTranslation.X = 0.0f;
            mPtTranslation.Y = 0.0f;

            ResetTransform();            

            Rectangle rect = new Rectangle(mRectImage.X, mRectImage.Y, mSizeImage.Width, mSizeImage.Height);

            PointF ptZoomCenter = ScreenToGlobal(new Point(rect.Location.X + (int)(rect.Width / 2.0f),
                   rect.Location.Y + (int)(rect.Height / 2.0f)));
            PointF ptImageOrgin = ScreenToGlobal(new Point(rect.Location.X,rect.Location.Y));
            PointF ptScrCenter = ScreenToGlobal(new PointF(Size.Width / 2.0f, Size.Height / 2.0f));
            //가로                  
            float fWidth = 0.0f;
            float fHeight = 0.0f;
            float fScale = 0.0f;
            //가로비
            fWidth = GetRatio(this.Size.Width, rect.Size.Width);
            //세로비
            fHeight = GetRatio( this.Size.Height, rect.Size.Height);

            // 작은 쪽을 기준으로 한다.
            fScale = (fWidth <= fHeight) ? fWidth : fHeight;

            // 시스템 스케일을 가져온다
            fScale = FindSystemScale(fScale);

            fScale = fScale * mTransform.Elements[3];
            if (fScale < mScaleList[98])
                fScale = mScaleList[98];

            if (fScale > mScaleList[0])
                fScale = mScaleList[0];
            
            float fRevScale = 1.0f / mTransform.Elements[3];
            mTransform.Translate(ptZoomCenter.X, ptZoomCenter.Y);
            mTransform.Scale(fRevScale, fRevScale);
            mTransform.Scale(fScale, fScale);
            mTransform.Translate(-ptZoomCenter.X,- ptZoomCenter.Y);

            ptZoomCenter = ScreenToGlobal(new Point(rect.Location.X + (int)(rect.Width / 2.0f),
                  rect.Location.Y + (int)(rect.Height / 2.0f)));
            ptImageOrgin = ScreenToGlobal(new Point(rect.Location.X, rect.Location.Y));
            ptScrCenter = ScreenToGlobal(new PointF(Size.Width / 2.0f, Size.Height / 2.0f));
            mTransform.Translate(ptScrCenter.X - ptZoomCenter.X, ptScrCenter.Y - ptZoomCenter.Y);

            Invalidate();
        }

        public float GetScale()
        {
            return mTransform.Elements[3];
        }

        public void OnPanelResize()
        {
            Point ptCenter = new Point(Width / 2, Height / 2);
           
            // TX 값을 구한다
            // TY 값도 구한다
            float tx = ptCenter.X - mPtGlobalCenter.X * mTransform.Elements[0];
            float ty = ptCenter.Y - mPtGlobalCenter.Y * mTransform.Elements[0];
            
            // 구한 TX, TY값으로 TransForm
            mTransform.Translate(tx, ty);

            // TranForm시킨만큼 Center값도 옮김
            mPtGlobalCenter.X += tx;
            mPtGlobalCenter.Y += ty;

            mPtTranslation.X = mPtGlobalCenter.X - mRectImage.Width / 2;
            mPtTranslation.Y = mPtGlobalCenter.Y - mRectImage.Height / 2;
        }

        private Rectangle CalcRect(Point ptStart, Point ptEnd)
        {
            int mMinX = Math.Min(mPtDragStart.X, mPtDragCurrent.X);
            int mMaxX = Math.Max(mPtDragStart.X, mPtDragCurrent.X);

            int mMinY = Math.Min(mPtDragStart.Y, mPtDragCurrent.Y);
            int mMaxY = Math.Max(mPtDragStart.Y, mPtDragCurrent.Y);

            int nWidth = 0;
            int nHeight = 0;

            if (mMinX < 0)
                mMinX = 0;

            if (mMinY < 0)
                mMinY = 0;

            if (Width < mMaxX)
            {
                nWidth = Width - mMinX;
            }
            else
            {
                nWidth = mMaxX - mMinX;
            }

            if (Height < mMaxY)
            {
                nHeight = Height - mMinY;
            }
            else
            {
                nHeight = mMaxY - mMinY;
            }  
            return new Rectangle(mMinX, mMinY, nWidth, nHeight);
        }

        public void OnPanelPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            //g.SmoothingMode = SmoothingMode.AntiAlias;
            try
            {
                g.Transform = mTransform;
            }
            catch (Exception)
            {
                return;
            }
            if (m_currentIndoorZone != null && m_currentIndoorZone.Azimuth != 0.0f)
            {
                g.TranslateTransform((float)mSizeImage.Width / 2, (float)mSizeImage.Height / 2);
                //now rotate the image
                g.RotateTransform((float)m_currentIndoorZone.Azimuth);
                g.TranslateTransform((float)-mSizeImage.Width / 2, (float)-mSizeImage.Height / 2);
            }
            if (mBaseImage != null)
            {
                Rectangle rect = new Rectangle(mRectImage.X, mRectImage.Y, mSizeImage.Width, mSizeImage.Height);
                g.DrawImage(mBaseImage, rect);
                mRectImage = rect;
                mPtCenter = new Point((int)(mRectImage.X + (mRectImage.Width * 0.5f)), (int)(mRectImage.Y + (mRectImage.Height * 0.5)));
            }

            if (m_bDrawShape == true)
            {
                //g.ResetTransform();

                foreach (Layer layer in m_Layers)
                {
                    layer.Draw(g);
                }
            }

            if (mbDrag == true && bRectZoomMode == true)
            {
                g.ResetTransform();
                g.DrawRectangle(mPenRect, mRectDrawing);
            }

            if( m_bDrawBillBoard == true)
            {
                int IconWidth = 64;
                foreach (BillBoard billBoard in mBillBoardList)
                {
                    Point pt = GlobalToScreen(billBoard.TX, billBoard.TY);
                    Point mPt = new Point(pt.X - IconWidth / 2, pt.Y - IconWidth);
                    PointF ptDraw = ScreenToGlobal(mPt);
                    billBoard.X = ptDraw.X;
                    billBoard.Y = ptDraw.Y;
                }

                g.ResetTransform();
                foreach (BillBoard billBoard in mBillBoardList)
                {
                    PointF pt = GlobalToScreen(billBoard.X, billBoard.Y );
                    Rectangle rect = new Rectangle((int)pt.X, (int)pt.Y, IconWidth, IconWidth);
                    //if (billBoard.Enabled == true)
                    {
                        if (billBoard.Selected == true)
                        {
                            g.DrawImage(billBoard.SelectImage, rect);
                        }
                        else
                        {
                            g.DrawImage(billBoard.Image, rect);
                        }
                    }
                    
                    
                }
            }
            
            
        }
        private bool m_bDrawShape = true;

        private void BaseMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (ModifierKeys & Keys.Control) == Keys.Control)
            {
                mPtPrev = e.Location;
                mPtDragStart = e.Location;
                mPtDragCurrent = e.Location;
                mbTranslateMode = false;

                mRectDrawing = new Rectangle();
                mbDrag = true;
                bRectZoomMode = true;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                mbDrag = true;
                mPtPrev = e.Location;
                mbTranslateMode = true;
                bRectZoomMode = false;
                mPtDragStart = e.Location;
                mPtDragCurrent = e.Location;
            }
            else if (e.Button == MouseButtons.Right)
            {
                mbDrag = false;
                mPtPrev = e.Location;
                mbTranslateMode = false;
                bRectZoomMode = false;
                mPtDragStart = e.Location;
                mPtDragCurrent = e.Location;
            }
            else
            {
                mbDrag = false;
                bRectZoomMode = false;
                mbTranslateMode = false;

            }
        }
        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            DoMouseWork(sender, e, BaseMouseDown, MouseEvent.MOUSE_DOWN);
            //FormMain.Instance.EnableFireReportBtn(false);           
        }

        public void OnMouseWheel(object sender, MouseEventArgs e)
        {
            Form form = (Form)sender;
            Point ptOrg = form.PointToScreen(e.Location);
            Point ptNew = PointToClient(ptOrg);

            PointF pt = ScreenToGlobal(ptNew);

            float fCurScale = 1.0f / mTransform.Elements[3];

            if (e.Delta < 0)
            {
                m_nScaleIndex = GetSacleIndex(false);
                m_nScaleIndex += 1;
                if (m_nScaleIndex >= 98)
                    m_nScaleIndex = 98;
                mTransform.Translate(pt.X, pt.Y);
                mTransform.Scale(fCurScale, fCurScale);
                mTransform.Scale(mScaleList[m_nScaleIndex], mScaleList[m_nScaleIndex]);

                mTransform.Translate(-pt.X, -pt.Y);
            }
            else
            {

                m_nScaleIndex = GetSacleIndex(true);

                m_nScaleIndex -= 1;

                if (m_nScaleIndex < 0)
                    m_nScaleIndex = 0;
                mTransform.Translate(pt.X, pt.Y);
                mTransform.Scale(fCurScale, fCurScale);
                mTransform.Scale(mScaleList[m_nScaleIndex], mScaleList[m_nScaleIndex]);
                mTransform.Translate(-pt.X, -pt.Y);

            }
            Invalidate();
        }

        public void BaseMouseMove(object sender, MouseEventArgs e)
        {
            mPtCurrent = e.Location;

            if (mbDrag)
            {
                if (mbRotationMode == true)
                {
                    //float delta = 1.0f;
                    //Point ptNew = e.Location;
                    //int dx = mPtPrev.X - ptNew.X;
                    //if (dx < 0)
                    //{
                    //    delta = -1.0f;
                    //}
                    //if (delta == -1.0f)
                    //{
                    //    m_RotationAngle--;
                    //}
                    //else
                    //{
                    //    m_RotationAngle++;
                    //}

                    //if (mbRotationMode == true)
                    //{
                    //    PointF fptStart = ScreenToGlobal(mPtPrev);
                    //    PointF fptCurrent = ScreenToGlobal(mPtCurrent);
                    //    try
                    //    {
                    //        Point ptCenter = new Point();
                    //        ptCenter.X = mRectImage.X + mRectImage.Width / 2;
                    //        ptCenter.Y = mRectImage.Y + mRectImage.Height / 2;

                    //        float fValue = (float)GetAngle(mPtPrev, mPtCurrent, ptCenter);
                    //        System.Diagnostics.Trace.WriteLine("DDD : " + fValue);
                    //        mTransform.Translate(panel1.Width * 0.5f, panel1.Height * 0.5f);
                    //        mTransform.Rotate(fValue);
                    //        mTransform.Translate(-panel1.Width * 0.5f, -panel1.Height * 0.5f);
                    //    }
                    //    catch (Exception)
                    //    {
                    //    }
                    //}
                    //mPtPrev = e.Location;
                }
                else if (mbTranslateMode == true)
                {
                    PointF prevPt = ScreenToGlobal(mPtPrev);
                    PointF fpt = ScreenToGlobal(e.Location);

                    float dx = fpt.X - prevPt.X;
                    float dy = fpt.Y - prevPt.Y;
                    mTransform.Translate(dx, dy);

                    mPtPrev = e.Location;
                }
                else if (bRectZoomMode == true)
                {
                    mRectDrawing = CalcRect(mPtDragStart, e.Location);
                }
                mPtDragCurrent = e.Location;
            }
        }

        public new void OnMouseUp(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            DoMouseWork(sender, e, BaseMouseUp, MouseEvent.MOUSE_UP);

            if (e.Button == MouseButtons.Left)
            {
                if (m_currentMode == MouseWorkMode.PICK)
                {
                    // IF NOT POI MOVE MODE
                    if (m_bDragPoi == false)
                        PickPOI(e.X, e.Y);
                    else
                    {
                        if (mPOIDragged != null)
                            OnPostMovePOI(mPOIDragged, e);

                        //TurnOnTemporaryList();
                    }
                    mPOIDragged = null;
                    m_bDragPoi = false;
                }
              
                else if (m_currentMode == MouseWorkMode.DEL_FACILITY)
                {
                   // DeletePOI(e.X, e.Y);
                }
            }
            //Invalidate(true);
        }

        public new void OnMouseMove(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            DoMouseWork(sender, e, BaseMouseMove, MouseEvent.MOUSE_MOVE);

#if DEBUG
			//Position3D pos = GetCameraPosition();
			//Quaternion3D ori = GetCameraOrientaion();
			//Position3D dir = GetCameraDirection();

			//if (pos != null)
			//{
			//    Debug.WriteLine("POSITION : " + pos.X + "," + pos.Y + "," + pos.Z);
			//    Debug.WriteLine("DIRECTION : " + dir.X + "," + dir.Y + "," + dir.Z);
			//    Debug.WriteLine("ORIENTATION : " + ori.X + "," + ori.Y + "," + ori.Z + ","+ ori.W );
			//}
#endif
        }

        private void OnPostMovePOI(Shape shape, MouseEventArgs e)
        {
            PointF pt = ScreenToGlobal(e.Location);
            shape.SetPosition(new UnE.Geometry.Vertex2D(pt.X, pt.Y));
        }

  

        private Zone GetPOIZone(MouseEventArgs e, float x, float y, float z)
        {
            //if (m_bIndoor)
            //{
            //    float nCurrentFloorIndex = -1.0f;
            //    Building building = m_frmParent.GetCurrentBuilding(ref nCurrentFloorIndex);

            //    if (building == null)
            //        return null;

            //    return ZoneManager.Instance.GetZone(building.BuildingID, nCurrentFloorIndex);
            //}
            return null;
        }


        public void BaseMouseUp(object sender, MouseEventArgs e)
        {
            Point pt = e.Location; 
            // Popup Menu
            if (e.Button == MouseButtons.Right)
            {
                if(mPopup != null)
                {
                    Point ptScreen = PointToScreen(pt);
                    mPopup.Show(ptScreen.X, ptScreen.Y);
                    mPopup.Tag = pt;
                }                
                return;
            }
            // Rect Zoom Mode인 경우 
            else if (e.Button == MouseButtons.Left)
            {
                if (bRectZoomMode == true)
                {
                    OnRectZoom(mPtDragStart, pt);
                }
            }       
          
            mRectDrawing = new Rectangle();
            mbDrag = false;
            bRectZoomMode = false;
            mbRotationMode = false;
            mbTranslateMode = false;

            mPtPrev = pt;
            mPtDragCurrent = pt;
        }

        // POI Drag Target
        private Shape mPOIDragged = null;
        private bool m_bDragPoi = false;

        private void DoMouseWork(Object sender, MouseEventArgs e, MouseEventHandler baseHandler, MouseEvent mouseEvent)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (m_currentMode == MouseWorkMode.PICK)
                {
                    if (this.Focused == false)
                        Focus();

                    if (mouseEvent == MouseEvent.MOUSE_DOWN)
                    {
                        if (m_bEditMode == true)
                        {
                            // SET POI MOVE MODE
                            mPtPrev.X = e.X;
                            mPtPrev.Y = e.Y;

                            PointF pt = ScreenToGlobal(e.Location);
                            Shape shape = PickObject(pt.X, pt.Y);
                            //int nPOIID = OnSelectPOI(e.X, e.Y);
                            if (shape != null)
                            {
                            //    if (m_dicPOIs.ContainsKey(nPOIID))
                            //    {
                                    mPOIDragged = shape;
                                    if (mPOIDragged != null)
                                    {                                        
                                        //OnPostPick(null, m_arrTemporaryHiddenPOIs, false);
                                    }
                                
                            }
                        }
                    }
                    else if (mouseEvent == MouseEvent.MOUSE_MOVE)
                    {
                        if (m_bEditMode == true)
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                int dx = e.X - mPtPrev.X;
                                int dy = e.Y - mPtPrev.Y;
                                // POI MOVE
                                if (mPOIDragged != null && dx != 0 && dy != 0)
                                {
                                    Shape poi = mPOIDragged;
                                    Point pt = GlobalToScreen((float)poi.Position.x, (float)poi.Position.y);
                                    pt.X = pt.X + dx;
                                    pt.Y = pt.Y + dy;


                                    PointF pos = ScreenToGlobal(pt);
                                    mPtPrev.X = e.X;
                                    mPtPrev.Y = e.Y;

                                    if (MovePOI(poi.ID, pos.X, pos.Y))
                                    {
                                        poi.SetPosition(new UnE.Geometry.Vertex2D(pos.X, pos.Y));
                                        Refresh();

                                        m_bDragPoi = true;
                                    }
                                }
                            }
                            else
                            {
                                m_bDragPoi = false;
                                //mPOIDragged = null;
                            }
                        }
                    }
                }
                else if (m_currentMode == MouseWorkMode.PANNING)
                {
                    OnPrevPanning(mouseEvent);

                    MouseEventArgs arg = new MouseEventArgs(MouseButtons.Middle, e.Clicks, e.X, e.Y, e.Delta);
                    baseHandler(sender, arg);

                    OnPostPanning(mouseEvent);
                }
                else if (m_currentMode == MouseWorkMode.NEW_FIRE_SENSOR)
                { }
                else if (m_currentMode == MouseWorkMode.NEW_COOLER_SENSOR)
                { }
                else if (m_currentMode == MouseWorkMode.NEW_PRESSURE_SENSOR)
                { }
                else if (m_currentMode == MouseWorkMode.DEL_FACILITY)
                { }
                else if (m_currentMode == MouseWorkMode.NEW_CCTV)
                { }
                else
                {
                    //OnPrevOrbit(mouseEvent);
                    baseHandler(sender, e);
                    //OnPostOrbit(mouseEvent);
                }
            }
            else
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    //Point pt = PointToScreen(new Point(e.X, e.Y));

                    if (mouseEvent == MouseEvent.MOUSE_UP)
                    {
                        Point pt = PointToScreen(new Point(e.X, e.Y));
                        if (this.Popup != null && this.Popup.Enabled == true)
                        {
                            this.Popup.Show(pt.X, pt.Y);
                        }
                    }
                    if (mouseEvent == MouseEvent.MOUSE_DOWN)
                    {
                        //OnSavePt(e);

                        //PointF pos = ScreenToGlobal(e.Location);

                        //PageBackstageHome.Instance.ContentForm.menuIndoor.Enabled = true;

                        //ToolStripItemCollection c = PageBackstageHome.Instance.ContentForm.menuIndoor.DropDownItems;
                        //c.Clear();

                        //ToolStripItemCollection r = PageBackstageHome.Instance.ContentForm.menuManualReport.DropDownItems;
                        //r.Clear();

                        //ToolStripItemCollection v = PageBackstageHome.Instance.ContentForm.menuManualCCTV.DropDownItems;
                        //v.Clear();

                        //Building building = null;
                        //if (m_bIndoor)
                        //{
                        //    if (m_currentIndoorZone != null)
                        //    {
                        //        //EquipmentZone equipZone = ZoneManager.Instance.CheckEquipmentZone(m_currentIndoorZone, pos.X, pos.Z);
                        //        //if (equipZone == null)
                        //        {
                        //            building = m_currentIndoorZone.Building;
                        //            PageBackstageHome.Instance.ContentForm.menuIndoor.Enabled = false;
                        //            PageBackstageHome.Instance.ContentForm.menuManualReport.Tag = m_currentIndoorZone;
                        //            PageBackstageHome.Instance.ContentForm.menuManualCCTV.Tag = m_currentIndoorZone;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    string szBuildingName = OnPickName();
                        //    building = ZoneManager.Instance.GetBuilding(szBuildingName);

                        //    if (building != null)
                        //    {
                        //        foreach (Zone zone in building.FloorList)
                        //        {
                        //            ToolStripMenuItem item = new ToolStripMenuItem();
                        //            item.Tag = zone;
                        //            item.Click += PageBackstageHome.Instance.ContentForm.IndoorMenuClick;
                        //            item.Text = zone.BroadcastName;
                        //            c.Add(item);

                        //            ToolStripMenuItem item2 = new ToolStripMenuItem();
                        //            item2.Tag = zone;
                        //            item2.Click += PageBackstageHome.Instance.ContentForm.ManualReport_Click;
                        //            item2.Text = zone.BroadcastName;
                        //            r.Add(item2);

                        //            ToolStripMenuItem item3 = new ToolStripMenuItem();
                        //            item3.Tag = zone;
                        //            item3.Click += PageBackstageHome.Instance.ContentForm.ManualCCTV_Click;
                        //            item3.Text = zone.BroadcastName;
                        //            v.Add(item3);
                        //        }

                        //        PageBackstageHome.Instance.ContentForm.menuManualReport.Tag = building;
                        //        PageBackstageHome.Instance.ContentForm.menuManualCCTV.Tag = building;
                        //    }
                        //    if (c.Count == 0)
                        //        PageBackstageHome.Instance.ContentForm.menuIndoor.Enabled = false;

                        //    if (building == null)
                        //    {
                        //        PointF pos3d = ScreenToGlobal(e.Location);
                        //        Zone zone = ZoneManager.Instance.GetOutsideZone(pos3d.X, pos3d.Y);
                        //        if (zone != null)
                        //        {
                        //            PageBackstageHome.Instance.ContentForm.menuManualReport.Tag = null;
                        //            PageBackstageHome.Instance.ContentForm.menuManualCCTV.Tag = null;

                        //            ToolStripMenuItem item2 = new ToolStripMenuItem();
                        //            item2.Tag = zone;
                        //            item2.Click += PageBackstageHome.Instance.ContentForm.ManualReport_Click;
                        //            item2.Text = zone.BroadcastName;
                        //            r.Add(item2);

                        //            ToolStripMenuItem item3 = new ToolStripMenuItem();
                        //            item3.Tag = zone;
                        //            item3.Click += PageBackstageHome.Instance.ContentForm.ManualCCTV_Click;
                        //            item3.Text = zone.BroadcastName;
                        //            v.Add(item3);
                        //        }
                        //    }
                        //}                       
                    }

                    return;
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                    OnPrevPanning(mouseEvent);

                baseHandler(sender, e);

                if (e.Button == System.Windows.Forms.MouseButtons.Middle)
                    OnPostPanning(mouseEvent);
            }

            if (e.Button == MouseButtons.None && mouseEvent == MouseEvent.MOUSE_MOVE)
            {
                ShowTooltip(e);
            }
            else
            {
                OnMouseLeave(this, new EventArgs());
            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            m_TooltipTimer.Stop();
            m_TooltipTimer.Enabled = false;

            if (m_formTooltip != null)
                m_formTooltip.Visible = false;

            m_formTooltip = null;
        }

        //private void OnShowTooltip(object sender, EventArgs e)
        //{
        //    //m_bShowTooltip = false;

        //    m_TooltipTimer.Stop();
        //    m_TooltipTimer.Enabled = false;

        //    int nPoiID = OnSelectPOI(m_nShowTooltipX, m_nShowTooltipY);
        //    if (nPoiID != -1)
        //    {
        //        POI poi = null;
        //        if (m_dicPOIs.TryGetValue(nPoiID, out poi))
        //        {
        //            if (poi.Zone == null)
        //                return;
        //            if (poi == null || poi.Facility == null)
        //                return;

        //            if (poi.Facility.Type != Facility.FacilityType.CCTV)
        //                return;

        //            if (m_bIndoor != poi.IsIndoor)
        //                return;

        //            Point pt = GlobalToScreen(poi.X, poi.Y);
        //            CCTV cctv = (CCTV)poi.Facility;
        //            m_formTooltip = new Form();

        //            string szName = "CCTV : " + cctv.AccessKey;
        //            string szZone = "위치 : " + poi.Zone.BroadcastName;
        //            Label lb = new Label();
        //            lb.AutoSize = true;
        //            lb.Text = szName;
        //            lb.Location = new Point(10, 10);

        //            int width1 = TextRenderer.MeasureText(lb.Text, new Font(lb.Font.FontFamily, lb.Font.Size, lb.Font.Style)).Width;

        //            m_formTooltip.Controls.Add(lb);

        //            Label lb2 = new Label();
        //            lb2.AutoSize = true;
        //            lb2.Text = szZone;
        //            lb2.Location = new Point(10, 28);

        //            int width2 = TextRenderer.MeasureText(lb2.Text, new Font(lb2.Font.FontFamily, lb2.Font.Size, lb2.Font.Style)).Width;

        //            int maxWidth = width1 > width2 ? width1 : width2;
        //            if (maxWidth < 130)
        //            {
        //                maxWidth = 130 + 20;
        //            }
        //            else
        //            {
        //                maxWidth = maxWidth + 20;
        //            }
        //            m_formTooltip.Controls.Add(lb2);

        //            int nTooltipHeight = 50;
        //            m_formTooltip.ShowInTaskbar = false;
        //            m_formTooltip.Size = new Size(maxWidth, nTooltipHeight);
        //            m_formTooltip.FormBorderStyle = FormBorderStyle.None;
        //            m_formTooltip.StartPosition = FormStartPosition.Manual;
        //            m_formTooltip.Opacity = 0.8f;
        //            m_formTooltip.Location = PointToScreen(new Point(pt.X - (maxWidth / 2), pt.Y - nTooltipHeight - 50));
        //            m_formTooltip.Show();

        //            //m_bShowTooltip = true;
        //            return;
        //        }
        //    }
        //}

        private void ShowTooltip(MouseEventArgs e)
        {
            if (m_nShowTooltipX != e.X || m_nShowTooltipY != e.Y)
            {
                m_TooltipTimer.Stop();
                m_TooltipTimer.Enabled = false;

                //m_bShowTooltip = false;
                if (m_formTooltip != null)
                {
                    m_formTooltip.Visible = false;
                    m_formTooltip = null;
                }
            }

            if (m_formTooltip == null)
            {
                m_nShowTooltipX = e.X;
                m_nShowTooltipY = e.Y;
                m_TooltipTimer.Enabled = true;
                m_TooltipTimer.Interval = 800;
                m_TooltipTimer.Start();
                //Debug.WriteLine("X={0}, Y={1}", m_nShowTooltipX, m_nShowTooltipY);
                //Debug.WriteLine(e.ToString());
            }
        }

        protected Point mSavedPt = new Point();
        protected void OnSavePt(MouseEventArgs e)
        {            
            mSavedPt = e.Location;
        }

        private void OnPrevPanning(MouseEvent e)
        {
            if (e != MouseEvent.MOUSE_DOWN)
                return;

            //OnPostPick(null, m_arrTemporaryHiddenPOIs, true);
        }

        private void OnPrevOrbit(MouseEvent e)
        {
            if (e != MouseEvent.MOUSE_DOWN)
                return;

            //OnPostPick(null, m_arrTemporaryHiddenPOIs, true);
        }

        private void OnPostPanning(MouseEvent e)
        {
            if (e == MouseEvent.MOUSE_UP)
            {
                //TurnOnTemporaryList();

                //// LOD에 따라 CCTV POI들을 가시화한다.
                //ProcessCCTVLOD();
            }
            else
                OnScreenMove();
        }


        private bool IsInCamera(float x, float y, float z)
        {
            // 화면좌표로 변환
            // 저장된 화면 클립바운드와 체크
            return false;
        }


        //public void ProcessCCTVLOD()
        //{
        //    Type type = typeof(CCTV);
        //    m_arrLODShowingPOIs.Clear();

        //    foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
        //    {
        //        POI poi = pair.Value;

        //        if (poi.Popup == null || poi.Facility == null || poi.Facility.GetType() != type)
        //            continue;

        //        CCTV cctv = (CCTV)poi.Facility;

        //        if (cctv.LODType == CCTV.LOD.VERY_IMPORTANT)
        //        {
        //            if (IsInCamera(poi.X, poi.Y, poi.Z))
        //            {
        //                if (!poi.Popup.IsVisible())
        //                {
        //                    Point pt = GlobalToScreen(poi.X, poi.Y);
        //                    poi.Popup.Show(pt.X, pt.Y);
        //                }

        //                m_arrLODShowingPOIs.Add(poi);
        //            }
        //            else
        //            {
        //                TooltipCCTVCtrl ctrl = (TooltipCCTVCtrl)poi.Popup;
        //                ctrl.Hide();
        //            }
        //        }
        //        else if (cctv.LODType == CCTV.LOD.IMPORTANT)
        //        {
        //            if (IsInCamera(poi.X, poi.Y, poi.Z))
        //            {
        //                if (!poi.Popup.IsVisible())
        //                {
        //                    Point pt = GlobalToScreen(poi.X, poi.Y);
        //                    poi.Popup.Show(pt.X, pt.Y);
        //                }

        //                m_arrLODShowingPOIs.Add(poi);
        //            }
        //            else
        //            {
        //                TooltipCCTVCtrl ctrl = (TooltipCCTVCtrl)poi.Popup;
        //                ctrl.Hide();
        //            }
        //        }
        //    }
        //}

        //private void TurnOnTemporaryList()
        //{
        //    foreach (POI poi in m_arrTemporaryHiddenPOIs)
        //    {
        //        Point pt = GlobalToScreen(new PointF(poi.X, poi.Y));
        //        poi.Popup.Show(pt.X, pt.Y);
        //    }

        //    m_arrTemporaryHiddenPOIs.Clear();
        //}

        private void OnScreenMove()
        {
            bool refresh = false;

            if (m_bIndoor)
            {
                if (m_currentIndoorZone != null && m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    //ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

                    //foreach (POI poi in arrPOIs)
                    //{
                    //    if (OnMovePOI(poi))
                    //        refresh = true;
                    //}
                }
            }
            else
            {
                //foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
                //{
                //    if (OnMovePOI(pair.Value))
                //        refresh = true;
                //}
            }

            if (refresh)
            {
                Update();
            }
        }

        //private bool OnMovePOI(POI poi)
        //{
        //    IPOIPopup popup = poi.Popup;

        //    if (popup != null && popup.IsVisible())
        //    {
        //        Point pt = GlobalToScreen(new PointF(poi.X, poi.Y));
        //        popup.Show(pt.X, pt.Y);
        //        return true;
        //    }

        //    return false;
        //}


        private bool m_bIndoor = true;
        public bool IsIndoor
        {
            get { return m_bIndoor; }
            set { m_bIndoor = value; }
        }

        //private void OnPostPick(POI poi, ArrayList arrHidden = null, bool absolutely = false)
        //{
        //    bool refresh = false;

        //    if (arrHidden != null)
        //        arrHidden.Clear();

        //    if (m_bIndoor)
        //    {
        //        if (m_currentIndoorZone != null && m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
        //        {
        //            ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

        //            foreach (POI _poi in arrPOIs)
        //            {
        //                if (_poi == poi || _poi.Popup == null || !_poi.Popup.IsVisible())
        //                    continue;

        //                if (arrHidden != null)// && IsLODShowingPOI(_poi))
        //                    arrHidden.Add(_poi);

        //                _poi.Popup.Hide(absolutely);
        //                refresh = true;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (KeyValuePair<int, POI> pair in m_dicPOIs)
        //        {
        //            if (pair.Value == poi || pair.Value.Popup == null || !pair.Value.Popup.IsVisible())
        //                continue;

        //            if (arrHidden != null)// && IsLODShowingPOI(pair.Value))
        //                arrHidden.Add(pair.Value);

        //            pair.Value.Popup.Hide(absolutely);
        //            refresh = true;
        //        }
        //    }

        //    if (poi != null)
        //        FormMain.Instance.PageHome.OnPostPickPOI(poi);

        //    if (refresh)
        //    {
        //        Update();
        //    }
        //}


        private void OnRectZoom(Point ptStart, Point ptEnd)
        {
            Rectangle rect = CalcRect(ptStart, ptEnd);

            PointF ptZoomCenter = ScreenToGlobal(new Point(rect.Location.X + (int)(rect.Width / 2.0f),
                   rect.Location.Y + (int)(rect.Height / 2.0f)));

            //가로                  
            float fWidth = 0.0f;
            float fHeight = 0.0f;
            float fScale = 0.0f;
            //가로비
            fWidth = GetRatio(rect.Size.Width, this.Size.Width);
            //세로비
            fHeight = GetRatio(rect.Size.Height, this.Size.Height);

            // 작은 쪽을 기준으로 한다.
            fScale = (fWidth <= fHeight) ? fWidth : fHeight;

            // 시스템 스케일을 가져온다
            fScale = FindSystemScale(fScale);

            //확대인지 축소인지?
            //축소면 뒤집음(
            if (ptEnd.X - ptStart.X < 0)
                fScale = 1.0f / fScale;

            fScale = fScale * mTransform.Elements[3];
            if (fScale < mScaleList[98])
                fScale = mScaleList[98];

            if (fScale > mScaleList[0])
                fScale = mScaleList[0];

            float fRevScale = 1.0f / mTransform.Elements[3];
            mTransform.Translate(ptZoomCenter.X, ptZoomCenter.Y);
            mTransform.Scale(fRevScale, fRevScale);
            mTransform.Scale(fScale, fScale);
            mTransform.Translate(-ptZoomCenter.X, -ptZoomCenter.Y);
        }

        public Point GlobalToScreen(PointF fpt)
        {
            Matrix mTemp = mTransform.Clone();
            PointF[] myArray =
            {
                fpt
            };
            mTemp.TransformPoints(myArray);
            int x = (int)myArray[0].X;
            int y = (int)myArray[0].Y;
            return new Point(x, y);
        }

        public Point GlobalToScreen(float xf, float yf)
        {          
            PointF fpt = new PointF(xf, yf);
            return GlobalToScreen(fpt);
        }

        public PointF ScreenToGlobal(Point pt)
        {
            PointF ff = new PointF(pt.X, pt.Y);
            return ScreenToGlobal(ff);
        }

        public UnE.Geometry.Vertex2D ScreenToGlobal(int x, int y)
        {
            PointF ff = new PointF(x, y);
            PointF dd = ScreenToGlobal(ff);

            return new UnE.Geometry.Vertex2D(dd.X, dd.Y);
        }

        public PointF ScreenToGlobal(PointF fpt)
        {
            Matrix mTemp = mTransform.Clone();
            try
            {
                mTemp.Invert();

            }
            catch (Exception)
            {
            }
            PointF[] myArray =
            {
                fpt
            };
            mTemp.TransformPoints(myArray);
            return new PointF(myArray[0].X, myArray[0].Y);
        }        
 
        private float FindSystemScale(float fAspectScale)
        {
            float fResult = fAspectScale;
            for (int i = 0; i < mScaleList.Length; i++)
            {
                float fTest = mScaleList[i];
                if (fAspectScale - 0.05f < mScaleList[i] && fAspectScale + 0.05f >= mScaleList[i])
                {
                    fResult = mScaleList[i];
                    break;
                }
            }
            return fResult;
        }

        //현재 Scale값 구하기
        private int GetSacleIndex(bool bZoomIn)
        {
            float fScale = mTransform.Elements[0];
            int nIndex = -1;

            for (int i = 1; i < mScaleList.Length ; i++)
            {
                if (bZoomIn == false && (fScale >= mScaleList[i] && fScale < mScaleList[i - 1]))
                {                  
                    nIndex = i;
                    break;
                }
                if (bZoomIn == true &&  (fScale > mScaleList[i]))
                {
                    nIndex = i - 1;
                    break;
                }
            }
          
            if (fScale <= mScaleList[98])
            {
                nIndex = 98;
            }

            if(fScale >= mScaleList[0])
            {
                nIndex = 0;
            }

            if (nIndex == -1)
            {
                throw new ArithmeticException();
            }

            return nIndex;
        }

        private float GetAngle(Point ptStart, Point ptCurrent, Point ptCenter)
        {
            //int a =0;
            float Ax = (ptCurrent.X - ptCenter.X);
            float Bx = (ptStart.X - ptCenter.X);
            float Ay = (ptCurrent.Y - ptCenter.Y);
            float By = (ptStart.Y - ptCenter.Y);

            //내적
            float fInProduct = (Ax * Bx) + (Ay * By);
            float cross = (Ax * By ) - ( Ay * Bx);

            //lAl * lBl
            float fValueA = (float)Math.Sqrt(Ax * Ax + Ay * Ay);
            float fValueB = (float)Math.Sqrt(By * By + Bx * Bx);

            if( fValueA < 0.001f && fValueA > -0.000f)
            {
                fValueA = 1.0f;
                fValueB = 1.0f;
            }

            if (fValueB < 0.001f && fValueB > -0.000f)
            {
                fValueA = 1.0f;
                fValueB = 1.0f;
            }

            float fValue = fInProduct / (fValueA * fValueB);
            float fValue2 = cross / (fValueA * fValueB);
            //double dAngle = Math.Acos(fValue);
            if( fValue > 1.0f)            
            {
                fValue = 1.0f; 
            }

            if( fValue < - 1.0f)
            {
                fValue = -1.0f;
            }

            if (fValue2 > 1.0f)
            {
                fValue2 = 1.0f;
            }

            if (fValue2 < -1.0f)
            {
                fValue2 = -1.0f;
            }
           
            float fSeta = (float)Math.Acos(fValue);

            //float fSin = (float)Math.ASin(fSeta);

            float dAngle =(float) (fSeta * 180.0f / Math.PI);
            

            if(fValue2 > 0.0f)
            {
                dAngle *= -1.0f;
            }

            return dAngle;
        }

        //화면비 구하기
        private void GetAspectRatio(Size size, out int rWidth, out int rHeight)
        {
            //최대공약수
            int GreatestMeasure = 0;

            if (size.Width > size.Height)
            {
                GreatestMeasure = GetGreatestMeasure(size.Width, size.Height);
            }
            else
            {
                GreatestMeasure = GetGreatestMeasure(size.Height, size.Width);
            }


            rWidth = size.Width / GreatestMeasure;
            rHeight = size.Height / GreatestMeasure;
        }
		
        //최대공약수 구하기
        private int GetGreatestMeasure(int a, int b)
        {

            int temp = 0;
            while (a != 0)
            {
                if (a < b)
                {
                    temp = a;
                    a = b;
                    b = temp;
                    break;
                }
                a = a - b;
            }

            return b;
        }

        //m:n 구하기
        private float GetRatio(int m, int n)
        {
            if (n == 0)
                return 0.0f;

            float fResult = 0.0f;
            fResult = (float)m / (float)n;
            return fResult;
        }               
		
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Point ptCenter = new Point(Width / 2, Height / 2);
            PointF pt1 = ScreenToGlobal(ptCenter);

            if (!base.ProcessCmdKey(ref msg, keyData))
            {
                if (keyData.Equals(Keys.Up))
                {
                    PointF ptOffset = new PointF((float)ptCenter.X, ptCenter.Y - 10 * GetScale());
                    PointF pt2 = ScreenToGlobal(ptOffset);                   
                    float tx = pt2.X - pt1.X;
                    float ty = pt2.Y - pt1.Y;
                    mTransform.Translate(tx, ty);

                    Invalidate();
                }
                else if (keyData.Equals(Keys.Down))
                {
                    PointF ptOffset = new PointF((float)ptCenter.X, ptCenter.Y + 10 * GetScale());
                    PointF pt2 = ScreenToGlobal(ptOffset);
                    float tx = pt2.X - pt1.X;
                    float ty = pt2.Y - pt1.Y;
                    mTransform.Translate(tx, ty);

                    Invalidate();
                }
                else if (keyData.Equals(Keys.Left))
                {
                    PointF ptOffset = new PointF(ptCenter.X - 10 * GetScale(), (float)ptCenter.Y);
                    PointF pt2 = ScreenToGlobal(ptOffset);
                    float tx = pt2.X - pt1.X;
                    float ty = pt2.Y - pt1.Y;
                    mTransform.Translate(tx, ty);

                    Invalidate();
                }
                else if (keyData.Equals(Keys.Right))
                {
                    PointF ptOffset = new PointF(ptCenter.X + 10 * GetScale(), (float)ptCenter.Y);
                    PointF pt2 = ScreenToGlobal(ptOffset);
                    float tx = pt2.X - pt1.X;
                    float ty = pt2.Y - pt1.Y;
                    mTransform.Translate(tx, ty);

                    Invalidate();
                }
            }
            return false;
        }
        
        //private void AddBillboardMenuItemClicked(object sender, EventArgs e)
        //{
        //    OpenFileDialog open = new OpenFileDialog();
        //    open.Filter = "모든파일(*.*)|*.*";
        //    if(open.ShowDialog() == DialogResult.OK)
        //    {
        //        string szBillBoardPath = open.FileName;
        //        if (szBillBoardPath != "")
        //        {
        //            Image img = Image.FromFile(szBillBoardPath);
        //            BillBoard billBoard = new BillBoard();
        //            billBoard.Image = img;
        //            Point pt = (Point)mCcontextMenuStrip.Tag;
        //            PointF fpt2 = ScreenToGlobal(pt);
        //            billBoard.TX = fpt2.X;
        //            billBoard.TY = fpt2.Y;
        //            billBoard.Width = mBillboardWidth;
        //            billBoard.Height = mBillboardHeight;
        //            mBillBoardList.Add(billBoard);                    
        //        }
        //    } 
        //}

        public int AddPOI(string szPath, float x, float y)
        {
            Image img = Image.FromFile(szPath);
            BillBoard billBoard = new BillBoard();
            billBoard.Image = img;
            m_nBillboardID++;
            billBoard.ID = m_nBillboardID;
            //PointF fpt2 = ScreenToGlobal(new PointF(x, y));
            billBoard.TX = x;
            billBoard.TY = y;
            billBoard.Width = mBillboardWidth;
            billBoard.Height = mBillboardHeight;
            mBillBoardList.Add(billBoard);
            return m_nBillboardID;
        }

        public int AddPOI(string szPath)
        {
            Image img = Image.FromFile(szPath);
            BillBoard billBoard = new BillBoard();
            billBoard.Image = img;
            m_nBillboardID++;
            billBoard.ID = m_nBillboardID;
            //PointF fpt2 = ScreenToGlobal(new PointF(x, y));
            billBoard.TX = mPtCurrent.X;
            billBoard.TY = mPtCurrent.Y;
            billBoard.Width = mBillboardWidth;
            billBoard.Height = mBillboardHeight;
            mBillBoardList.Add(billBoard);
            return m_nBillboardID;
        }

        //public void AddPOI(POI poi)
        //{
        //    poi.ViewType = 2;
        //    poi.ParentView = this;

        //    if (poi.Facility == null)
        //        return;

        //    if (poi.Popup == null)
        //        poi.Popup = poi.Facility.CreatePopup(this);

        //    if (!m_bIndoor || (m_bIndoor && poi.Zone == m_currentIndoorZone))
        //    {
        //        string strIconPath = poi.Facility.IconPath;
        //        int nID = AddPOI(strIconPath, poi.X, poi.Y);
        //        poi.ID = nID;
        //        m_dicPOIs[nID] = poi;
        //    }
        //    else if (poi.ID > 0)
        //        m_dicPOIs[poi.ID] = poi;

        //    if (m_bIndoor && poi.Zone != null)
        //    {
        //        if (m_dicZonePOIs.ContainsKey(poi.Zone))
        //        {
        //            ArrayList arrPOIs = m_dicZonePOIs[poi.Zone];
        //            arrPOIs.Add(poi);
        //        }
        //        else
        //        {
        //            ArrayList arrPOIs = new ArrayList();
        //            m_dicZonePOIs[poi.Zone] = arrPOIs;
        //            arrPOIs.Add(poi);
        //        }
        //    }

        //    int nLayerID = poi.Facility.GetLayerID();
        //    m_frmParent.Layers.GetLayer(nLayerID).Add(poi.ID);
        //}

        //public void RemovePOI(int nID)
        //{
        //    BillBoard board = GetBillboard(nID);
        //    if( board != null)
        //    {
        //        board.Visible = false;
        //        mBillBoardList.Remove(board);
        //    }
        //}

        //public void RemovePOI(float x, float y)
        //{
        //    BillBoard delete = null;
        //    foreach (BillBoard board in mBillBoardList)
        //    {
        //        if (board.TX == x && board.TY == y)
        //        {
        //            delete = board;
        //            break;
        //        }
        //    }
        //    if (delete != null)
        //        mBillBoardList.Remove(delete);
        //} 

        public void AddControl(Control c)
        {
            this.Controls.Add(c);
        }

        //public bool IsTemporaryHiddenPOI(POI poi)
        //{
        //    return false;
        //}

        public  void EnablePOI(int nID, bool bEnable)
        {
            BillBoard billboard = GetBillboard(nID);
            if( billboard != null)
            {
                billboard.Enabled = bEnable;
            }
            Refresh();
        }

        private BillBoard GetBillboard(int nID)
        {
            BillBoard find = null;
            foreach (BillBoard board in mBillBoardList)
            {
                if (board.ID == nID)
                {
                    find = board;
                    break;
                }
            }
            return find;
        }

        //public POI FindPOI(int nID)
        //{
        //    if (m_dicPOIs.ContainsKey(nID))
        //        return m_dicPOIs[nID];

        //    return null;
        //}

        public void SetCheckPoistion(bool bCheck)
        {

        }

        //public void HideAllPOIPopup()
        //{
        //    OnPostPick(null, null, true);
        //}


        private void ClearAllSelectedPOI()
        {
            foreach (BillBoard board in mBillBoardList)
            {
                board.Selected = false;
            }
        }

        public void ClearPOISelection()
        {
            ClearAllSelectedPOI();
            m_arSelectedPoi.Clear();
        }

        public void ShowIconPOI(int nID, bool bShow)
        {
            BillBoard billboard = GetBillboard(nID);
            if (billboard != null)
            {
                billboard.Visible = bShow;
            }
            Refresh();
        }

        private void PickPOI(int x, int y)
        {
              
        }
        public void SelectPOI(int nID )
        {
            SelectPOI(nID, true);
        }
        public void SelectPOI(int nID, bool bSelect )
        {
            ClearPOISelection();

            m_arSelectedPoi.Add(nID);

            BillBoard select = null;
            foreach (BillBoard board in mBillBoardList)
            {
                if (board.ID == nID)
                {
                    select = board;
                    break;
                }
            }
     
        }
        
        protected int OnSelectPOI(int x, int y)
        {
            PointF pt = ScreenToGlobal(new Point(x, y));          
            
            BillBoard select = null;
            foreach (BillBoard board in mBillBoardList)
            {

                Point pt1 = GlobalToScreen(board.TX, board.TY);
                Rectangle rect = new Rectangle(pt1.X - 32, pt1.Y - 64, 64, 64);

                if (rect.Contains(x, y))
                {
                    select = board;
                    break;
                }
            }
            if (select != null)
                return select.ID;
            return -1;
        }

        public void SaveScreen(string szPath)
        {

        }

        public void ClearAllData()
        {
            if (m_currentIndoorZone != null)
            {
                if (m_dicZonePOIs.ContainsKey(m_currentIndoorZone))
                {
                    ArrayList arrPOIs = m_dicZonePOIs[m_currentIndoorZone];

                    //foreach (POI poi in arrPOIs)
                    //{
                    //    // 뷰가 바뀌어서 없애는 것이므로 3d 뷰에서만 삭제하고 dictionary에는 남겨둔다.
                    //    RemovePOI(poi.ID);

                    //    if (poi.Popup != null)
                    //    {
                    //        poi.Popup.Close();
                    //        poi.Popup = null;
                    //    }
                    //}
                }
            }
        }

        public void AddZoneName(string szName)
        {

        }

        public bool MovePOI(int nID, float x, float y)
        {
            BillBoard board = GetBillboard(nID);
            if (board == null)
                return false;

            board.TX = x;
            board.TY = y;
            
            return true;
        }

        private ContextMenuStrip mPopup = null;
        public ContextMenuStrip Popup
        {
            get { return mPopup; }
            set { mPopup = value; }
        }

        private MouseWorkMode m_currentMode = MouseWorkMode.PICK;
        public MouseWorkMode CurrentMouseWorkMode
        {
            get { return m_currentMode; }
            set { m_currentMode = value; }
        }

        public void ShowZonePolygon(Zone zone , bool bShow)
        {

        }

        public void ShowEquipmentZone(EquipmentZone zone, bool bShow)
        {

        }

        private ArrayList m_arSelectedPoi = new ArrayList();
        public ArrayList SelectedPOIList
        {
            get { return m_arSelectedPoi; }
        }


        private int m_nBillboardID = 1;


      

      
	}


    public class BillBoard
    {
        public BillBoard()
        {
        }

        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }
        
        private Image img = null;
        public Image Image
        {
            get { return img; }
            set
            { 
                img = value;
                mSelectImage = (Image)ChangeColor((Bitmap)img);
            }
        }

        private Image mSelectImage = null;
        public Image SelectImage
        {
            get { return mSelectImage; }
            set { mSelectImage = value; }
        }

        private float orgx = 0;
        public float X
        {
            get { return orgx; }
            set
            {
                orgx = value;
            }
        }

        private float orgY = 0;
        public float Y
        {
            get { return orgY; }
            set
            {
                orgY = value;
            }
        }

        private float ntx = 0;
        public float TX
        {
            get { return ntx; }
            set { ntx = value;
            rect.X = (int)value;
            }
        }

        private float nty = 0;
        public float TY
        {
            get { return nty; }
            set { nty = value;
            rect.Y = (int)value;

            }
        }

        private Rectangle rect = new Rectangle();
        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }


        private int nWidth = 0;
        public int Width
        {
            get { return nWidth; }
            set 
            { 
                nWidth = value;
                rect.Width = value;
            }
        }

        private int nHeight = 0;
        public int Height
        {
            get { return nHeight; }
            set { nHeight = value;
            rect.Height = value;
            }
        }

        private bool m_bEnabled = false;
        public bool Enabled
        {
            get { return m_bEnabled; }
            set { m_bEnabled = value; }
        }

        private bool m_bSelected = false;

        public bool Selected
        {
            get { return m_bSelected; }
            set { m_bSelected = value; }
        }

        private bool m_bVisible = false;
        public bool Visible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }

        public static Bitmap ChangeColor(Bitmap scrBitmap)
        {
            //You can change your new color here. Red,Green,LawnGreen any..
            Color newColor = Color.MediumSeaGreen;
            Color actulaColor;
            //make an empty bitmap the same size as scrBitmap
            Bitmap newBitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
            for (int i = 0; i < scrBitmap.Width; i++)
            {
                for (int j = 0; j < scrBitmap.Height; j++)
                {
                    //get the pixel from the scrBitmap image
                    actulaColor = scrBitmap.GetPixel(i, j);
                    // > 150 because.. Images edges can be of low pixel colr. if we set all pixel color to new then there will be no smoothness left.
                    if (actulaColor.A > 150)
                        newBitmap.SetPixel(i, j, newColor);
                    else
                        newBitmap.SetPixel(i, j, actulaColor);
                }
            }
            return newBitmap;
        }
    }
}
