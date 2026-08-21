using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Sections
{
    public class ShapeLink : Shape
    {
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
        
        public ShapeLink(Section sectionParent)
            : base(sectionParent)
        {
            notifier.Mode = NotifierDrawMode.ARC;

            if (m_painter == null)
            {
                if (imgOut == null)
                    imgOut = global::Sections.Properties.Resources.LINK_OUT;
                if (imgInNormal == null)
                    imgInNormal = global::Sections.Properties.Resources.LINK_IN;
                if( imgInSkipped == null)
                    imgInSkipped = global::Sections.Properties.Resources.Link_IN_Skipped;
                if( imgInProcessing == null)
                    imgInProcessing = global::Sections.Properties.Resources.Link_IN_Processing;
                if( imgInProcessed == null)
                    imgInProcessed = global::Sections.Properties.Resources.Link_IN_Processed;
                if( imgInWaiting == null)
                    imgInWaiting = global::Sections.Properties.Resources.Link_IN_Waiting;
                if (imgSelect == null)
                    imgSelect = global::Sections.Properties.Resources.LINK_OUT_red;

                m_painter = new ImagePainter(imgInNormal, imgInSkipped, imgInProcessing, imgInProcessed, imgInWaiting, imgOut, 64, 74, 5, 5);

                m_painter.ImageSelected = imgSelect;
            }

            base.ImagePainter = m_painter;
        }

        protected override bool Paint(Graphics g, float x, float y)
        {
            if (UseImage)
                return DrawImage(g, x, y);

            if (m_fWidth <= 0 || m_fHeight <= 0)
                return false;
            Point[] bound = ClipBoundRect;

            if (m_arrBoundaryTransform != null)
            {
                if (m_needTransform)
                    TransformPolygon(x, y);

                int nWidth = (int)GetSize(true);
                int nHeight = (int)GetSize(false);

                // DRAW SELECT LINE
                


                
                GraphicsPath path = new GraphicsPath();            
                path.StartFigure();
                path.AddArc(m_arrBoundaryTransform[0].X, m_arrBoundaryTransform[0].Y, m_fWidth, m_fHeight, 0, 360);
                path.CloseFigure();

                DrawShadow(g, path);

                Pen pen = new Pen(mOutLineColor);
                pen.Width = mOutLineThick;
               
                if (!m_transparentLine)
                {
					LinearGradientBrush brush = new LinearGradientBrush(new Point((int)x + nWidth / 2, (int)y), new Point((int)x + nWidth / 2, (int)y + nHeight), Color.White, FILL_BRUSH);

                    g.FillPath(brush, path);                  
                }
                      
                g.DrawPath(pen, path);

                notifier.SetPosition((int)x, (int)y);
                notifier.Size = new Size((int)GetSize(true), (int)GetSize(false));
                notifier.Path = path;
                notifier.Paint(g);
                
            }
            return true;
        }

        protected override void DrawShadow(Graphics g, PointF[] arrPolygons = null)
        {
            float fMoveX = 6.0f, fMoveY = 5.0f;
            g.TranslateTransform(fMoveX, fMoveY);

            if (arrPolygons == null)
                arrPolygons = m_arrBoundaryTransform;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddPie(m_arrBoundaryTransform[0].X, m_arrBoundaryTransform[0].Y, m_fWidth, m_fHeight, 0, 360);

            System.Drawing.Drawing2D.PathGradientBrush brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
            brush.WrapMode = System.Drawing.Drawing2D.WrapMode.Clamp;

            System.Drawing.Drawing2D.ColorBlend clrBlend = new System.Drawing.Drawing2D.ColorBlend(3);
            clrBlend.Colors = new Color[] { Color.Transparent, Color.FromArgb(180, Color.DimGray), Color.FromArgb(180, Color.DimGray) };

            clrBlend.Positions = new float[] { 0.0f, 0.1f, 1.0f };

            brush.InterpolationColors = clrBlend;
            g.FillPath(brush, path);

            g.TranslateTransform(-fMoveX, -fMoveY);
        }
    }
}
