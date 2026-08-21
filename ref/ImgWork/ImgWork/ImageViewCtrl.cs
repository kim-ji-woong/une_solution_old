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

namespace ImgWork
{  
	public partial class ImageViewCtrl : Panel
	{
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
        
        private Brush mBrushRect = null;
        private Pen mPenRect = null;
        private int m_nScaleIndex = 0;

        private IContainer components;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mCcontextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddBillboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.mCcontextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mCcontextMenuStrip
            // 
            this.mCcontextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddBillboardToolStripMenuItem});
            this.mCcontextMenuStrip.Name = "mCcontextMenuStrip";
            this.mCcontextMenuStrip.Size = new System.Drawing.Size(139, 26);
            // 
            // AddBillboardToolStripMenuItem
            // 
            this.AddBillboardToolStripMenuItem.Name = "AddBillboardToolStripMenuItem";
            this.AddBillboardToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.AddBillboardToolStripMenuItem.Text = "빌보드 추가";
            this.AddBillboardToolStripMenuItem.Click += new System.EventHandler(this.AddBillboardMenuItemClicked);
            // 
            // mOpenFileDialog
            // 
            this.mOpenFileDialog.FileName = "*.*";
            // 
            // ImageViewCtrl
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Name = "ImageView";
            this.Size = new System.Drawing.Size(352, 470);
            this.mCcontextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.ContextMenuStrip mCcontextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem AddBillboardToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog mOpenFileDialog;

        public ImageViewCtrl()
		{
            this.DoubleBuffered = true;

			InitializeComponent();
			this.MouseWheel += OnMouseWheel;       
     
            mBrushRect = new HatchBrush(HatchStyle.Sphere, Color.Blue, Color.LightGreen);
            mPenRect = new Pen(mBrushRect, 0);
		}

        public void SetImage(string szImagePath)
        {
            ResetTransform();
           
            if( System.IO.File.Exists(szImagePath))
            {
                mBaseImage = Bitmap.FromFile(szImagePath);
                mSizeImage = new Size(mBaseImage.Width, mBaseImage.Height);
                mPtCenter = new Point((int)(Width * 0.5f), (int)(Height * 0.5f));
                mPtGlobalCenter = new Point(Width / 2, Height / 2);
            }
            else
            {
                mBaseImage = null;
            }          
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
            
            if (mBaseImage != null)
            {
                Rectangle rect = new Rectangle(mRectImage.X, mRectImage.Y, mSizeImage.Width, mSizeImage.Height);
                g.DrawImage(mBaseImage, rect);
                mRectImage = rect;
                mPtCenter = new Point((int)(mRectImage.X + (mRectImage.Width * 0.5f)), (int)(mRectImage.Y + (mRectImage.Height * 0.5)));
            }

            if (mbDrag == true && bRectZoomMode == true)
            {
                g.ResetTransform();                
                g.DrawRectangle(mPenRect, mRectDrawing);
            }

            if( m_bDrawBillBoard == true)
            {
                g.ResetTransform();
                foreach (BillBoard billBoard in mBillBoardList)
                {
                    Point pt = GlabalToScreen(new PointF(billBoard.TX, billBoard.TY));
                    Rectangle rect = new Rectangle(pt.X, pt.Y, billBoard.Width, billBoard.Height);
                    g.DrawImage(billBoard.Image, rect);
                }
            }            
        }
        
        public void OnMouseDown(object sender, MouseEventArgs e)
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

        public void OnMouseWheel(object sender, MouseEventArgs e)
        {
            PointF pt = ScreenToGlobal(e.Location);

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

        public void OnMouseMove(object sender, MouseEventArgs e)
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

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            Point pt = e.Location; 
            // Popup Menu
            if (e.Button == MouseButtons.Right)
            {
                Point ptScreen = PointToScreen(pt);
                mCcontextMenuStrip.Show(ptScreen.X, ptScreen.Y);
                mCcontextMenuStrip.Tag = pt;
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

        public Point GlabalToScreen(PointF fpt)
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

        public PointF ScreenToGlobal(Point pt)
        {
            PointF ff = new PointF(pt.X, pt.Y);
            return ScreenToGlobal(ff);
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
        
        private void AddBillboardMenuItemClicked(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "모든파일(*.*)|*.*";
            if(open.ShowDialog() == DialogResult.OK)
            {
                string szBillBoardPath = open.FileName;
                if (szBillBoardPath != "")
                {
                    Image img = Image.FromFile(szBillBoardPath);
                    BillBoard billBoard = new BillBoard();
                    billBoard.Image = img;
                    Point pt = (Point)mCcontextMenuStrip.Tag;
                    PointF fpt2 = ScreenToGlobal(pt);
                    billBoard.TX = fpt2.X;
                    billBoard.TY = fpt2.Y;
                    billBoard.Width = mBillboardWidth;
                    billBoard.Height = mBillboardHeight;
                    mBillBoardList.Add(billBoard);                    
                }
            } 
        }

	}

    public class BillBoard
    {
        public BillBoard()
        {
        }

        private Image img = null;
        public Image Image
        {
            get { return img; }
            set { img = value; }
        }

        private float ntx = 0;
        public float TX
        {
            get { return ntx; }
            set { ntx = value; }
        }

        private float nty = 0;
        public float TY
        {
            get { return nty; }
            set { nty = value; }
        }

        private int nWidth = 0;
        public int Width
        {
            get { return nWidth; }
            set { nWidth = value; }
        }

        private int nHeight = 0;
        public int Height
        {
            get { return nHeight; }
            set { nHeight = value; }
        }
    }
}
