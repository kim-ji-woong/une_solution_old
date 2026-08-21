using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sections
{
    public class ShapeAnnotation : Shape
    {
        private static Image imgOut = null;
        private static Image imgInNormal = null;
        private static Image imgInSkipped = null;
        private static Image imgInProcessing = null;
        private static Image imgInProcessed = null;
        private static Image imgInWaiting = null;
        private static Image imgSelect = null;

        private static float m_fEdgeSize = 20;
        private PointF[] m_arrDrawing = new PointF[5];

        private int m_nAnnotationLineWidth = 0;

        private ImagePainter m_painter = null;

        public ImagePainter ImagePainter
        {
            get { return m_painter; }
            set { m_painter = value; }
        }

        public ShapeAnnotation(Section sectionParent)
            : base(sectionParent)
        {
            if (m_painter == null)
            {
                if( imgOut == null )
                    imgOut = global::Sections.Properties.Resources.Annotation_OUT;
                if( imgInNormal == null)
                    imgInNormal = global::Sections.Properties.Resources.Annotation_IN;
                if( imgInSkipped == null)
                    imgInSkipped = global::Sections.Properties.Resources.Annotation_IN_Skipped;
                if( imgInProcessing == null)
                    imgInProcessing = global::Sections.Properties.Resources.Annotation_IN_Processing;
                if( imgInProcessed == null)
                    imgInProcessed = global::Sections.Properties.Resources.Annotation_IN_Processed;
                if( imgInWaiting == null)
                    imgInWaiting = global::Sections.Properties.Resources.Annotation_IN_Waiting;
                if( imgSelect == null)
                    imgSelect = global::Sections.Properties.Resources.Annotation_OUT_red;
                m_painter = new ImagePainter(imgInNormal, imgInSkipped, imgInProcessing, imgInProcessed, imgInWaiting, imgOut, 50, 50, 4, 4);

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

                if (m_arrBoundaryTransform.Count() != 4)
                    return false;

                int nWidth = (int)GetSize(true);
                int nHeight = (int)GetSize(false);

                // Offset의 순서가 0->1->2->3 에서 2->3->0->1 이 된다.
                m_arrDrawing[0] = m_arrOffsetTransform[2];
                m_arrDrawing[1] = new PointF(m_arrOffsetTransform[3].X - m_fEdgeSize, m_arrOffsetTransform[3].Y);
                m_arrDrawing[2] = new PointF(m_arrOffsetTransform[3].X, m_arrOffsetTransform[3].Y + m_fEdgeSize);
                m_arrDrawing[3] = m_arrOffsetTransform[0];
                m_arrDrawing[4] = m_arrOffsetTransform[1];

                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath ShadowPath = new GraphicsPath())
                {
                    int addThick = (int)(OutLineThick / 2.0f);
                    ShadowPath.StartFigure();

                    for (int i = 1; i < m_arrDrawing.Length; i++)
                    {
                        ShadowPath.AddLine(
                            m_arrDrawing[i - 1].X, m_arrDrawing[i - 1].Y,
                            m_arrDrawing[i].X, m_arrDrawing[i].Y
                            );
                    }
                    ShadowPath.CloseFigure();
                    DrawShadow(g, ShadowPath);
                }


                m_arrDrawing[0] = m_arrBoundaryTransform[0];
                m_arrDrawing[1] = new PointF(m_arrBoundaryTransform[1].X - m_fEdgeSize, m_arrBoundaryTransform[1].Y);
                m_arrDrawing[2] = new PointF(m_arrBoundaryTransform[1].X, m_arrBoundaryTransform[1].Y + m_fEdgeSize);
                m_arrDrawing[3] = m_arrBoundaryTransform[2];
                m_arrDrawing[4] = m_arrBoundaryTransform[3];

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
                    pen.Width = m_nAnnotationLineWidth / 3;
                    g.DrawPath(pen, path);
                    int addThick = (int)(m_nAnnotationLineWidth / 6.0f);
                    PointF pt = new PointF(m_arrDrawing[1].X, m_arrDrawing[2].Y);
                    PointF pt2 = new PointF(m_arrDrawing[1].X - addThick, m_arrDrawing[2].Y);
                    g.DrawLine(pen, m_arrDrawing[1], pt);
                    g.DrawLine(pen, pt2, m_arrDrawing[2]);

                }
            }
            return true;
        }
    }
}
