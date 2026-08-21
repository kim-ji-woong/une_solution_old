using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

//using System.Windows.Shapes;



namespace Sections
{
    public class Shape
    {
        // PrevDrawing : Shape.Draw() 그리기 전에 먼저 그린다.
        // PostDrawing : Shape.Draw() 그린 후에 그린다.
        // OwnDrawing : Shape.Draw()는 무시하고 그린다.
        public enum DrawType { None, PrevDrawing, PostDrawing, OwnDrawing };

        protected float m_fOriginWidth = 0, m_fOriginHeight = 0;
        
        // Polygon을 이루는 Point 집합의 원본 좌표
        protected PointF[] m_arrBoundaryOrigin = null;
       
        // Section 위치 및 화면 Scroll을 고려하여 위치 이동된 좌표 
        public PointF[] m_arrBoundaryTransform = null;

        // Polygon을 이루는 Point 집합에서 OutLineThick만큼 Offset된 좌표
        protected PointF[] m_arrOffsetOrigin = null;
        // Section 위치 및 화면 Scroll을 고려하여 위치 이동된 OffsetPolygon 좌표 
        protected PointF[] m_arrOffsetTransform = null;


        protected bool m_needTransform = false;
        protected float m_fWidth, m_fHeight;

        protected Section m_sectionParent = null;

        protected bool m_transparentFill = false;
        protected bool m_transparentLine = false;

        // Shape의 사각 영역(화면 좌표, 좌측 상단, 우측 하단)
        protected Point[] m_arrBoundaryRect = new Point[2];
        public Point[] ClipBoundRect
        {
            get { return m_arrBoundaryRect; }
            set { m_arrBoundaryRect = value; }
        }

        protected static Color mOutLineColor = Color.FromArgb(95, 146, 201);
        public static Color OutLineColor
        {
            get { return Shape.mOutLineColor; }
            set { Shape.mOutLineColor = value; }
        }    

        

        public static Pen BOUNDARY_PEN = new Pen(Color.FromArgb(185, 255, 185), 1);
        public static Pen BOUNDARY_PEN2 = new Pen(Color.FromArgb(0, 0, 0), 1);
		protected Color FILL_BRUSH = Color.FromArgb(210, 210, 210);

        protected PathNotifier notifier = null;//new GradientNotifier();
        private bool mbDisableNotifier = false;
        public bool DisableNotifier
        {
            get { return mbDisableNotifier; }
            set 
            { 
                mbDisableNotifier = value;

                if (notifier != null)
                {
                    notifier.Selected = !mbDisableNotifier;
                }

                /*if (notifier != null && value == false)
                {
                    notifier.Selected = false;
                    notifier.Painter = null;
                }*/

            }
        }

        private static bool m_useImage = true;
        public static bool UseImage
        {
            get { return m_useImage; }
            set { m_useImage = value; }
        }

        protected ImagePainter m_imgPainter = null;

        public ImagePainter ImagePainter
        {
            get { return m_imgPainter; }
            set 
            { 
                m_imgPainter = value;
                notifier.Painter = m_imgPainter;
            }
        }

        protected static float mOutLineThick = 8.0f;
        public static float OutLineThick
        {
            get { return mOutLineThick; }
            set { mOutLineThick = value; }
        }

        protected static Size m_MinSize = new Size(100, 50);
        public static Size MinSize
        {
            get { return m_MinSize; }
            set
            {
                if (value == null)
                    return;
                m_MinSize = value;
            }
        }

        /// <summary>
        /// 이미지 모드에서 Shape의 상태정보를 구별하기 위하여 사용
        public enum ShapeStatus { NORMAL, SKIPPED, PROCESSING, PROCESSED, WAITING };

        private ShapeStatus m_status = ShapeStatus.NORMAL;
        public ShapeStatus Status
        {
            get { return m_status; }
            set
            {
                m_status = value;

                if (m_shapeStyler != null)
                    m_shapeStyler.SetState(m_status);
            }
        }
        /// </summary>

        // 외부에서 그리기 기능을 수행하기 위한 Interface
        private static IShapeStyleFactory m_shapeStyleFactory = null;
        protected IShapeStyler m_shapeStyler = null;

        public static IShapeStyleFactory ShapeStyleFactory
        {
            get { return m_shapeStyleFactory; }
            set { m_shapeStyleFactory = value; }
        }

        public IShapeStyler ShapeStyler
        {
            get { return m_shapeStyler; }
            set { m_shapeStyler = value; }
        }


        public Shape(Section sectionParent)
        {
            m_fWidth = m_fHeight = 0;
            m_sectionParent = sectionParent;

           

            notifier = new PathNotifier(m_sectionParent.GetParent().timer1);               
            //notifier.GradientStep = 20;
            //notifier.EndColor = m_sectionParent.GetParent().BackColor;
            notifier.Size = new Size((int)GetSize(true), (int)GetSize(false));
            notifier.SetPosition(50, 50);
            notifier.Parent = m_sectionParent.GetParent();
               

            notifier.Mode = NotifierDrawMode.POLYGON;
            //if (mbDisableNotifier != true)
            //    notifier.Select();

            if (m_shapeStyleFactory != null)
                m_shapeStyler = m_shapeStyleFactory.CreateShapeStyler(m_sectionParent, this);
        }

        public virtual void SetBoundary(ArrayList arrBoundary, float x, float y)
        {
            int nCount = arrBoundary.Count;
            if (nCount == 0) return;


            m_arrBoundaryOrigin = new PointF[nCount];
            m_arrBoundaryTransform = new PointF[nCount];

            PointF pt = (PointF)arrBoundary[0];
            m_arrBoundaryOrigin[0] = pt;
            m_arrBoundaryTransform[0] = new PointF(pt.X + x, pt.Y + y);

            float left = pt.X;
            float right = pt.X;
            float top = pt.Y;
            float bottom = pt.Y;

            float _left = m_arrBoundaryTransform[0].X;
            float _top = m_arrBoundaryTransform[0].Y;
            float _right = m_arrBoundaryTransform[0].X;
            float _bottom = m_arrBoundaryTransform[0].Y;

            for (int i = 1; i < nCount; i++)
            {
                pt = (PointF)arrBoundary[i];
                m_arrBoundaryOrigin[i] = pt;
                m_arrBoundaryTransform[i] = new PointF(pt.X + x, pt.Y + y);

                if (left > pt.X) left = pt.X;
                if (right < pt.X) right = pt.X;
                if (top > pt.Y) top = pt.Y;
                if (bottom < pt.Y) bottom = pt.Y;

                if (_left > m_arrBoundaryTransform[i].X) _left = m_arrBoundaryTransform[i].X;
                if (_right < m_arrBoundaryTransform[i].X) _right = m_arrBoundaryTransform[i].X;
                if (_top > m_arrBoundaryTransform[i].Y) _top = m_arrBoundaryTransform[i].Y;
                if (_bottom < m_arrBoundaryTransform[i].Y) _bottom = m_arrBoundaryTransform[i].Y;
            }

            SetScreenBoundaryRect(_left, _top, _right, _bottom);

            m_fWidth = right - left;
            m_fHeight = bottom - top;

            m_fOriginWidth = m_fWidth;
            m_fOriginHeight = m_fHeight;

            //if (mbDisableNotifier != true)              
            //    notifier.VertexList = m_arrBoundaryTransform;


            MakeOffset(x, y);

            if (m_shapeStyler != null)
                m_shapeStyler.OnCreate(x, y);
        }

        protected virtual void MakeOffset(float x, float y)
        {
            int nCount = m_arrBoundaryOrigin.Length;

            List<ClipperLib.IntPoint> paths = new List<ClipperLib.IntPoint>();           
            for( int i = 0 ; i < nCount; i++)
            {
                ClipperLib.IntPoint pt = new ClipperLib.IntPoint();


                pt.X = (int)m_arrBoundaryOrigin[i].X;
                pt.Y = (int)m_arrBoundaryOrigin[i].Y;
                paths.Add(pt);
            }
           
            double offset = mOutLineThick / 2;
            ClipperLib.ClipperOffset pathOffset = new ClipperLib.ClipperOffset();
            pathOffset.MiterLimit = 15;
            pathOffset.AddPath(paths, ClipperLib.JoinType.jtMiter, ClipperLib.EndType.etClosedLine);

            List<List<ClipperLib.IntPoint>> polysResult = new List<List<ClipperLib.IntPoint>>();
            pathOffset.Execute(ref polysResult, offset);



            List<ClipperLib.IntPoint> polyResult = polysResult[0];
            nCount = polyResult.Count;
            m_arrOffsetOrigin = new PointF[nCount];
            m_arrOffsetTransform = new PointF[nCount];

            for (int i = 0; i < nCount; i++)
            {
                m_arrOffsetOrigin[i].X = polyResult[i].X;
                m_arrOffsetOrigin[i].Y = polyResult[i].Y;
            }

            PointF pt2 = (PointF)m_arrOffsetOrigin[0];
            m_arrOffsetTransform[0] = new PointF(pt2.X + x , pt2.Y + y );

            for (int i = 1; i < nCount; i++)
            {
                pt2 = (PointF)m_arrOffsetOrigin[i];
                m_arrOffsetTransform[i] = new PointF(pt2.X + x , pt2.Y + y);
            }
        }

        protected void SetScreenBoundaryRect(float left, float top, float right, float bottom)
        {
            m_arrBoundaryRect[0].X = (int)left;
            m_arrBoundaryRect[0].Y = (int)top;
            m_arrBoundaryRect[1].X = (int)right;
            m_arrBoundaryRect[1].Y = (int)bottom;
            /*PanelSectionEx panel = (PanelSectionEx)m_sectionParent.GetParent();
            m_arrBoundaryRect[0] = panel.GlobalToScreen(new PointF(left, top));
            m_arrBoundaryRect[1] = panel.GlobalToScreen(new PointF(right, bottom));*/
        }

        public virtual void ChangePosition(PointF pt)
        {
            if (m_arrBoundaryTransform != null && m_arrBoundaryOrigin != null)
            {

                float _left = 0.0f, _top = 0.0f, _right = 0.0f, _bottom = 0.0f;
                int nPointCount = m_arrBoundaryOrigin.Count();

                for (int i = 0; i < nPointCount; i++)
                {
                    m_arrBoundaryTransform[i].X = m_arrBoundaryOrigin[i].X * m_fWidth / m_fOriginWidth + pt.X ;
                    m_arrBoundaryTransform[i].Y = m_arrBoundaryOrigin[i].Y * m_fHeight / m_fOriginHeight + pt.Y ;
                    

                    // calc offset boundary
                    if (i < m_arrOffsetTransform.Length)
                    {
                        m_arrOffsetTransform[i].X = m_arrOffsetOrigin[i].X * m_fWidth / m_fOriginWidth + pt.X ;
                        m_arrOffsetTransform[i].Y = m_arrOffsetOrigin[i].Y * m_fHeight / m_fOriginHeight + pt.Y ;
                    }                  

                    if (i == 0)
                    {
                        _left = m_arrBoundaryTransform[0].X;
                        _top = m_arrBoundaryTransform[0].Y;
                        _right = m_arrBoundaryTransform[0].X;
                        _bottom = m_arrBoundaryTransform[0].Y;
                    }
                    else
                    {
                        if (_left > m_arrBoundaryTransform[i].X) _left = m_arrBoundaryTransform[i].X;
                        if (_right < m_arrBoundaryTransform[i].X) _right = m_arrBoundaryTransform[i].X;
                        if (_top > m_arrBoundaryTransform[i].Y) _top = m_arrBoundaryTransform[i].Y;
                        if (_bottom < m_arrBoundaryTransform[i].Y) _bottom = m_arrBoundaryTransform[i].Y;
                    }
                }

                SetScreenBoundaryRect(_left, _top, _right, _bottom);

            }

            if (m_shapeStyler != null)
                m_shapeStyler.OnMove(pt.X, pt.Y);
        }

        public float GetSize(bool isHorz)
        {
            return isHorz ? m_fWidth : m_fHeight;
        }


        protected static bool m_bCheckSize = true;
        protected static bool SizeCheck
        {
            get { return m_bCheckSize; }
            set { m_bCheckSize = value; }
        }

        public virtual bool ChangeSize(float x, float y, float fWidth, float fHeight)
        {
            if (fWidth != m_fWidth || fHeight != m_fHeight)
            {
                if (m_bCheckSize == true)
                {
                    if (fWidth < (float)m_MinSize.Width)
                        fWidth = (float)m_MinSize.Width;

                    if (fHeight < (float)m_MinSize.Height)
                        fHeight = (float)m_MinSize.Height;

                }
               
                m_fWidth = fWidth;
                m_fHeight = fHeight;


                if (m_arrBoundaryOrigin != null && m_arrBoundaryTransform != null)
                {
                    float _left = 0.0f, _top = 0.0f, _right = 0.0f, _bottom = 0.0f;
                    int nPointCount = m_arrBoundaryOrigin.Count();

                    // 원래 크기 대비 Scale 변경
                    for (int i = 0; i < nPointCount; i++)
                    {
                        m_arrBoundaryTransform[i].X = m_arrBoundaryOrigin[i].X * m_fWidth / m_fOriginWidth + x ;
                        m_arrBoundaryTransform[i].Y = m_arrBoundaryOrigin[i].Y * m_fHeight / m_fOriginHeight + y ;

                        if (i < m_arrOffsetTransform.Length)
                        {
                            m_arrOffsetTransform[i].X = m_arrOffsetOrigin[i].X * m_fWidth / m_fOriginWidth + x ;
                            m_arrOffsetTransform[i].Y = m_arrOffsetOrigin[i].Y * m_fHeight / m_fOriginHeight + y ;
                        }

                        if (i == 0)
                        {
                            _left = m_arrBoundaryTransform[0].X;
                            _top = m_arrBoundaryTransform[0].Y;
                            _right = m_arrBoundaryTransform[0].X;
                            _bottom = m_arrBoundaryTransform[0].Y;
                        }
                        else
                        {
                            if (_left > m_arrBoundaryTransform[i].X) _left = m_arrBoundaryTransform[i].X;
                            if (_right < m_arrBoundaryTransform[i].X) _right = m_arrBoundaryTransform[i].X;
                            if (_top > m_arrBoundaryTransform[i].Y) _top = m_arrBoundaryTransform[i].Y;
                            if (_bottom < m_arrBoundaryTransform[i].Y) _bottom = m_arrBoundaryTransform[i].Y;
                        }
                    }

                    SetScreenBoundaryRect(_left, _top, _right, _bottom);
                }

                if (m_shapeStyler != null)
                    m_shapeStyler.OnResize(x, y, fWidth, fHeight);

                return true;
            }

            return false;
        }

        public void Transform()
        {
            m_needTransform = true;
        }

        protected void TransformPolygon(float x, float y)
        {
            if (m_sectionParent == null || m_arrBoundaryOrigin == null)
                return;

            int nCount = m_arrBoundaryOrigin.Count();

            for (int i = 0; i < nCount; i++)
            {
                m_arrBoundaryTransform[i].X = m_arrBoundaryOrigin[i].X * m_fWidth / m_fOriginWidth + x ;
                m_arrBoundaryTransform[i].Y = m_arrBoundaryOrigin[i].Y * m_fHeight / m_fOriginHeight + y ;

                if (i < m_arrOffsetTransform.Length)
                {
                    m_arrOffsetTransform[i].X = m_arrOffsetOrigin[i].X * m_fWidth / m_fOriginWidth + x ;
                    m_arrOffsetTransform[i].Y = m_arrOffsetOrigin[i].Y * m_fHeight / m_fOriginHeight + y ;
                }
            }

            if (mbDisableNotifier != true)
                m_needTransform = false;
        }

        public bool Draw(Graphics g, float x, float y)
        {
            if (m_fWidth <= 0 || m_fHeight <= 0)
                return false;

            bool result = false;

            if (m_shapeStyler != null)
            {
                DrawType drawingType = m_shapeStyler.GetDrawType();

                if (drawingType == DrawType.PrevDrawing)
                {
                    m_shapeStyler.Draw(g, x, y, notifier);
                    result = Paint(g, x, y);
                }
                else if (drawingType == DrawType.PostDrawing)
                {
                    result = Paint(g, x, y);
                    m_shapeStyler.Draw(g, x, y, notifier);
                }
                else if (drawingType == DrawType.OwnDrawing)
                {
                    result = m_shapeStyler.Draw(g, x, y, notifier);
                }
                else// if (drawingType == DrawType.None)
                {
                    result = Paint(g, x, y);
                }
            }
            else
                result = Paint(g, x, y);

            return result;
        }

        protected virtual bool Paint(Graphics g, float x, float y)
        {
            if (m_useImage)
                return DrawImage(g, x, y);

            Point[] bound = ClipBoundRect;

            if (m_arrBoundaryTransform != null)
            {
                int nWidth = (int)GetSize(true);
                int nHeight = (int)GetSize(false);

                g.SmoothingMode = SmoothingMode.AntiAlias;

                if (m_needTransform)
                    TransformPolygon(x, y);

                if (!notifier.Selected && this.m_sectionParent.GetComponentType() != Section.ComponentType.PROCESS)
                {
                    if (m_sectionParent.GetComponentType() != Section.ComponentType.TRANSMISSION)
                    {
                        using (GraphicsPath ShadowPath = new GraphicsPath())
                        {
                            int addThick = (int)(OutLineThick / 2.0f);
                            ShadowPath.StartFigure();

                            for (int i = 1; i < m_arrBoundaryTransform.Length; i++)
                            {
                                ShadowPath.AddLine(
                                    m_arrOffsetTransform[i - 1].X, m_arrOffsetTransform[i - 1].Y,
                                    m_arrOffsetTransform[i].X, m_arrOffsetTransform[i].Y
                                    );
                            }
                            ShadowPath.CloseFigure();
                            DrawShadow(g, ShadowPath);
                        }
                    }

                }

                using (GraphicsPath path = new GraphicsPath())
                {
                    path.StartFigure();
                    for (int i = 1; i < m_arrBoundaryTransform.Length; i++)
                    {
                        path.AddLine(m_arrBoundaryTransform[i - 1].X, m_arrBoundaryTransform[i - 1].Y,
                            m_arrBoundaryTransform[i].X, m_arrBoundaryTransform[i].Y
                            );
                    }
                    path.CloseFigure();

                    if (m_transparentFill)
                        g.DrawPolygon(BOUNDARY_PEN2, m_arrBoundaryTransform);
                    else if (!m_transparentLine)
                    {
                        using (LinearGradientBrush brush = new LinearGradientBrush(new Point((int)x + nWidth / 2, (int)y - 30), new Point((int)x + nWidth / 2, (int)y + nHeight), Color.White, FILL_BRUSH))
                        {
                            if (m_sectionParent.GetComponentType() == Section.ComponentType.TRANSMISSION)
                            {
                                DrawShadow(g, path);
                            }
                            g.FillPath(brush, path);
                        }
                        using (Pen pen = new Pen(OutLineColor))
                        {
                            pen.Width = OutLineThick;
                            g.DrawPath(pen, path);
                        }
                    }


                    if (mbDisableNotifier != true)
                    {
                        notifier.SetPosition((int)x, (int)y);
                        notifier.Size = new Size((int)GetSize(true), (int)GetSize(false));
                        notifier.Path = path;
                        notifier.Paint(g);
                    }
                }

            }
            return true;
        }

        protected virtual bool DrawImage(Graphics g, float x, float y)
        {
            if (m_imgPainter != null)
            {
                m_imgPainter.Draw(g, x, y, m_fWidth, m_fHeight, m_status);
                return true;
            }

            return false;
        }

        protected virtual void DrawShadow(Graphics g, System.Drawing.Drawing2D.GraphicsPath path)
        {
            if (path == null)
                return;
            //float fMoveX = 10.0f, fMoveY = 10.0f;
            float fMoveX = 9.0f, fMoveY = 7.0f;
            g.TranslateTransform(fMoveX, fMoveY);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;       

            System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
            brush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;

            System.Drawing.Drawing2D.ColorBlend clrBlend = new System.Drawing.Drawing2D.ColorBlend(3);
            clrBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(180, Color.DimGray), Color.FromArgb(180, Color.DimGray) };

            clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };

            brush.InterpolationColors = clrBlend;
            g.FillPath(brush, path);

            g.TranslateTransform(-fMoveX, -fMoveY);
        }


        protected virtual void DrawShadow(Graphics g, PointF[] arrPolygons = null)
        {
            float fMoveX = 6.0f, fMoveY = 5.0f;
            g.TranslateTransform(fMoveX, fMoveY);

            if (arrPolygons == null)
                arrPolygons = m_arrBoundaryTransform;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddPolygon(arrPolygons);

            System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
            brush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;

            System.Drawing.Drawing2D.ColorBlend clrBlend = new System.Drawing.Drawing2D.ColorBlend(3);
            clrBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(180, Color.DimGray), Color.FromArgb(180, Color.DimGray) };

            clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };

            brush.InterpolationColors = clrBlend;
            g.FillPath(brush, path);

            g.TranslateTransform(-fMoveX, -fMoveY);
        }

        private void DrawPolygon(Graphics g, Pen pen, PointF[] arrPoint, float fMoveX, float fMoveY)
        {
            int nPointCount = arrPoint.Count();
            PointF[] arrMove = new PointF[nPointCount];

            for (int i = 0; i < nPointCount; i++)
            {
                arrMove[i].X = arrPoint[i].X + fMoveX;
                arrMove[i].Y = arrPoint[i].Y + fMoveY;
            }

            g.DrawPolygon(pen, arrMove);
        }

        // x, y : 화면 Scroll을 고려하지 않은 화면 좌표
        public bool Select(float x, float y)
        {
            if (Geometry.PolygonHitTest(m_arrBoundaryTransform, new PointF(x, y)) != 0)
                return true;

            return false;
        }

        public bool Select(Rectangle rect)
        {
            Point[] bound = ClipBoundRect;

            int mMinX = Math.Min(bound[0].X, bound[1].X);
            int mMaxX = Math.Max(bound[0].X, bound[1].X);

            int mMinY = Math.Min(bound[0].Y, bound[1].Y);
            int mMaxY = Math.Max(bound[0].Y, bound[1].Y);

            Rectangle rectBound = new Rectangle(mMinX, mMinY, mMaxX - mMinX, mMaxY - mMinY);

            return rect.IntersectsWith(rectBound);
        }

        public static void SetLineColor(Color color)
        {
            BOUNDARY_PEN.Color = color;
        }

        public static Color GetLineColor()
        {
            return BOUNDARY_PEN.Color;
        }

		public void SetFillColor(Color color)
		{
			FILL_BRUSH = color;
		}

		public Color GetFillColor()
		{
			return FILL_BRUSH;
		}

        public static void SetLineThick(int nThick)
        {
            BOUNDARY_PEN.Width = nThick;
        }

        public static int GetLineThick()
        {
            return (int)BOUNDARY_PEN.Width;
        }

        public virtual void SetTransparency(bool isLine, bool transparency)
        {
            if (isLine)
                m_transparentLine = transparency;
            else
                m_transparentFill = transparency;
        }

        public virtual bool GetTransparency(bool isLine)
        {
            return isLine ? m_transparentLine : m_transparentFill;
        }
            
        public void SetNotify(bool bNoty)
        {
            notifier.Selected = bNoty;            
        }

        public Point GetScreenBoundaryRect(bool topLeft)
        {
            return topLeft ? m_arrBoundaryRect[0] : m_arrBoundaryRect[1];
        }
    }

    public interface IShapeStyler
    {
        Shape.DrawType GetDrawType();
        bool Draw(Graphics g, float x, float y, PathNotifier notifier);
        void OnCreate(float x, float y);
        void OnResize(float x, float y, float fWidth, float fHeight);
        void OnMove(float x, float y);
        void SetState(Shape.ShapeStatus shapeStatus);
        void SetCurrent(bool isCurrent);
    }

    public interface IShapeStyleFactory
    {
        IShapeStyler CreateShapeStyler(Section section, Shape shape);
    }
}
