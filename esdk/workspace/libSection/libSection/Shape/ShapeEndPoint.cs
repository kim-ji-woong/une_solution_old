using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;

namespace Sections
{
    public class ShapeEndPoint : Shape
    {
        private static int CIRCLE_SLICE = 100;
        private PointF[] m_arrRect = new PointF[4];
        private PointF[] m_arrDrawing = null;
        private PointF m_ptPrev = new PointF(0, 0);

        private ImagePainter m_painter = null;

        public ImagePainter ImagePainter
        {
            get { return m_painter; }
            set { m_painter = value; }
        }

        private static Image imgOut = null;
        private static Image imgInNormal = null;
        private static Image imgInSkipped = null;
        private static Image imgInProcessing = null;
        private static Image imgInProcessed = null;
        private static Image imgInWaiting = null;
        private static Image imgSelect = null;

        public ShapeEndPoint(Section sectionParent)
            : base(sectionParent)
        {
            m_arrDrawing = new PointF[CIRCLE_SLICE + 2];
              
            notifier.Mode = NotifierDrawMode.POLYGON;

            if (m_painter == null)
            {
                if (imgOut == null)
                    imgOut = global::Sections.Properties.Resources.EndPoint_OUT;
                if (imgInNormal == null)
                    imgInNormal = global::Sections.Properties.Resources.EndPoint_IN;
                if( imgInSkipped == null)
                    imgInSkipped = global::Sections.Properties.Resources.EndPoint_IN_Skipped;
                if( imgInProcessing == null)
                    imgInProcessing = global::Sections.Properties.Resources.EndPoint_IN_Processing;
                if( imgInProcessed == null)
                    imgInProcessed = global::Sections.Properties.Resources.EndPoint_IN_Processed;
                if( imgInWaiting == null)
                    imgInWaiting = global::Sections.Properties.Resources.EndPoint_IN_Waiting;
                if (imgSelect == null)
                    imgSelect = global::Sections.Properties.Resources.EndPoint_OUT_red;
                m_painter = new ImagePainter(imgInNormal, imgInSkipped, imgInProcessing, imgInProcessed, imgInWaiting, imgOut, 67, 56, 5, 5);

                m_painter.ImageSelected = imgSelect;
            }

            base.ImagePainter = m_painter;
        }

        protected override bool Paint(Graphics g, float x, float y)
        {
            if (UseImage)
                return DrawImage(g, x, y);

            Point[] bound = ClipBoundRect;

            if (m_arrBoundaryTransform != null)
            {
                if (m_needTransform)
                    TransformPolygon(x, y);

                //notifier.SetPosition((int)x, (int)y);
                //notifier.Size = new Size((int)GetSize(true), (int)GetSize(false));
                //notifier.Paint(g);

                int nWidth = (int)GetSize(true);
                int nHeight = (int)GetSize(false);
                GraphicsPath path = new GraphicsPath();

                path.StartFigure();
                for (int i = 1; i < m_arrDrawing.Length; i++)
                {
                    path.AddLine(
                        m_arrDrawing[i - 1].X, m_arrDrawing[i - 1].Y,
                        m_arrDrawing[i].X, m_arrDrawing[i].Y
                        );
                }
                path.CloseFigure();

                g.SmoothingMode = SmoothingMode.AntiAlias;

                DrawShadow(g, path);

                if (!m_transparentFill)
                {
                    LinearGradientBrush brush = new LinearGradientBrush(
                    new Point((int)x + nWidth / 2, (int)y),
                    new Point((int)x + nWidth / 2, (int)y + nHeight),
                    Color.White,
				    FILL_BRUSH
                    );
                    g.FillPath(brush, path);
                }

                if (!m_transparentLine)
                {
                    Pen pen = new Pen(mOutLineColor);
                    pen.Width = mOutLineThick;
                    g.DrawPath(pen, path);    
                }

                notifier.SetPosition((int)x, (int)y);
                notifier.Size = new Size((int)GetSize(true), (int)GetSize(false));
                notifier.Path = path;
                notifier.Paint(g);
            }

            return true;
        }

        public override void SetBoundary(ArrayList arrBoundary, float x, float y)
        {                
            base.SetBoundary(arrBoundary, x, y);
            CalcResize(x, y);                
        }

        public override bool ChangeSize(float x, float y, float fWidth, float fHeight)
        {
            if (fWidth != m_fWidth || fHeight != m_fHeight)
            {

                if (fWidth < (float)m_MinSize.Width)
                    fWidth = (float)m_MinSize.Width;

                if (fHeight < (float)m_MinSize.Height)
                    fHeight = (float)m_MinSize.Height;

                m_fWidth = fWidth;
                m_fHeight = fHeight;

                if (m_arrBoundaryOrigin != null && m_arrBoundaryTransform != null)
                {  
                    int nPointCount = m_arrBoundaryOrigin.Count();

                    // 원래 크기 대비 Scale 변경
                    for (int i = 0; i < nPointCount; i++)
                    {
                        m_arrBoundaryTransform[i].X = m_arrBoundaryOrigin[i].X * m_fWidth / m_fOriginWidth + x;
                        m_arrBoundaryTransform[i].Y = m_arrBoundaryOrigin[i].Y * m_fHeight / m_fOriginHeight + y;
                    }

                    CalcResize(x, y);
                }

                return true;
            }

            return false;
        }

        private void CalcResize(float x, float y)
        {
            if (m_arrBoundaryTransform.Count() != 4)
                return;

            m_ptPrev.X = x;
            m_ptPrev.Y = y;

            double dRadius = m_fHeight / 2.0;
            double centerX = x + dRadius, centerX2 = x + m_fWidth - dRadius;
            double centerY = y + dRadius;
            double dStartAngle = 0.0;
            double dStartAngle2 = System.Math.PI;
            double dArcAngle = System.Math.PI;
            double dAddAngle = 0.0;
            float fArcWidth = (float)dRadius;

            // 전체 너비가 지름의 3배보다 작을 경우
            if (dRadius * 4 > m_fWidth)
            {
                double dAddRadius = dRadius * 4 - m_fWidth;
                centerX += dAddRadius;
                centerX2 -= dAddRadius;

                dRadius = centerX - x;

                dAddAngle = System.Math.Acos((m_fHeight / 2) / dRadius);
                dStartAngle += dAddAngle;
                dStartAngle2 += dAddAngle;
                dArcAngle -= dAddAngle * 2;

                fArcWidth = (float)(dRadius - dRadius * System.Math.Sin(dAddAngle));
            }

            float _left = 0.0f, _top = 0.0f, _right = 0.0f, _bottom = 0.0f;

            int nSliceCount = CIRCLE_SLICE / 2;
            double delta = dArcAngle / nSliceCount;

            for (int i = 0; i <= nSliceCount; i++)
            {
                double dAngle = delta * i + dStartAngle;
                double _x = centerX - dRadius * System.Math.Sin(dAngle);
                double _y = centerY + dRadius * System.Math.Cos(dAngle);

                m_arrDrawing[i] = new PointF((float)_x, (float)_y);

                if (i == 0)
                {
                    _left = m_arrDrawing[0].X;
                    _top = m_arrDrawing[0].Y;
                    _right = m_arrDrawing[0].X;
                    _bottom = m_arrDrawing[0].Y;
                }
                else
                {
                    if (_left > m_arrDrawing[i].X) _left = m_arrDrawing[i].X;
                    if (_right < m_arrDrawing[i].X) _right = m_arrDrawing[i].X;
                    if (_top > m_arrDrawing[i].Y) _top = m_arrDrawing[i].Y;
                    if (_bottom < m_arrDrawing[i].Y) _bottom = m_arrDrawing[i].Y;
                }
            }

            // CIRCLE_SLICE가 홀수일수 있으므로 2로 나누지 않고 nSliceCount를 빼준다.
            int nSliceCount2 = CIRCLE_SLICE - nSliceCount;
            delta = dArcAngle / nSliceCount2;

            for (int i = 0; i <= nSliceCount2; i++)
            {
                double dAngle = delta * i + dStartAngle2;
                double _x = centerX2 - dRadius * System.Math.Sin(dAngle);
                double _y = centerY + dRadius * System.Math.Cos(dAngle);

                int nIndex = i + nSliceCount + 1;
                m_arrDrawing[nIndex] = new PointF((float)_x, (float)_y);

                if (_left > m_arrDrawing[nIndex].X) _left = m_arrDrawing[nIndex].X;
                if (_right < m_arrDrawing[nIndex].X) _right = m_arrDrawing[nIndex].X;
                if (_top > m_arrDrawing[nIndex].Y) _top = m_arrDrawing[nIndex].Y;
                if (_bottom < m_arrDrawing[nIndex].Y) _bottom = m_arrDrawing[nIndex].Y;
            }

            SetScreenBoundaryRect(_left, _top, _right, _bottom);

            //notifier.VertexList = m_arrDrawing;

            if (m_shapeStyler != null)
                m_shapeStyler.OnResize(x, y, m_fWidth, m_fHeight);
        }

        public override void ChangePosition(PointF pt)
        {
            base.ChangePosition(pt);

            if (m_arrBoundaryTransform != null && m_arrBoundaryOrigin != null)
            {
                float fMoveX = pt.X - m_ptPrev.X;
                float fMoveY = pt.Y - m_ptPrev.Y;

                float _left = 0.0f, _top = 0.0f, _right = 0.0f, _bottom = 0.0f;
                int nPointCount = m_arrDrawing.Count();

                for (int i = 0; i < nPointCount;i++ )
                {
                    m_arrDrawing[i].X += fMoveX;
                    m_arrDrawing[i].Y += fMoveY;

                    if (i == 0)
                    {
                        _left = m_arrDrawing[0].X;
                        _top = m_arrDrawing[0].Y;
                        _right = m_arrDrawing[0].X;
                        _bottom = m_arrDrawing[0].Y;
                    }
                    else
                    {
                        if (_left > m_arrDrawing[i].X) _left = m_arrDrawing[i].X;
                        if (_right < m_arrDrawing[i].X) _right = m_arrDrawing[i].X;
                        if (_top > m_arrDrawing[i].Y) _top = m_arrDrawing[i].Y;
                        if (_bottom < m_arrDrawing[i].Y) _bottom = m_arrDrawing[i].Y;
                    }
                }

                SetScreenBoundaryRect(_left, _top, _right, _bottom);

                m_ptPrev = pt;

                //notifier.VertexList = m_arrDrawing;
                    
            }
        }
    }
}
