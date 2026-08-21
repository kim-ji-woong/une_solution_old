using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace SectionStyle.Fancy
{
    class StyleAnnotation : Style
    {
        private const float EdgeSize = 20;
        private PointF[] m_edgeLines = new PointF[3];

        public StyleAnnotation(SectionAnnotation section, ShapeAnnotation shape)
        {
            m_section = section;
            m_shape = shape;
            m_arrDrawing = new PointF[5];
        }

        protected override void Create(float x, float y)
        {
            if (m_shape == null)
                return;

            float fWidth = m_shape.GetSize(true);
            float fHeight = m_shape.GetSize(false);

            m_arrDrawing[0] = new PointF(x, y);
            m_arrDrawing[1] = new PointF(x + fWidth - EdgeSize, y);
            m_arrDrawing[2] = new PointF(x + fWidth, y + EdgeSize);
            m_arrDrawing[3] = new PointF(x + fWidth, y + fHeight);
            m_arrDrawing[4] = new PointF(x, y + fHeight);

            MakePath();
        }

        protected override void Resize(float x, float y, float fWidth, float fHeight)
        {
            if (m_shape == null)
                return;

            m_arrDrawing[0].X = x;
            m_arrDrawing[0].Y = y;
            m_arrDrawing[1].X = x + fWidth - EdgeSize;
            m_arrDrawing[1].Y = y;
            m_arrDrawing[2].X = x + fWidth;
            m_arrDrawing[2].Y = y + EdgeSize;
            m_arrDrawing[3].X = x + fWidth;
            m_arrDrawing[3].Y = y + fHeight;
            m_arrDrawing[4].X = x;
            m_arrDrawing[4].Y = y + fHeight;

            MakePath();
        }

        protected override void Move(float x, float y)
        {
            if (m_shape == null)
                return;

            float fWidth = m_shape.GetSize(true);
            float fHeight = m_shape.GetSize(false);

            m_arrDrawing[0].X = x;
            m_arrDrawing[0].Y = y;
            m_arrDrawing[1].X = x + fWidth - EdgeSize;
            m_arrDrawing[1].Y = y;
            m_arrDrawing[2].X = x + fWidth;
            m_arrDrawing[2].Y = y + EdgeSize;
            m_arrDrawing[3].X = x + fWidth;
            m_arrDrawing[3].Y = y + fHeight;
            m_arrDrawing[4].X = x;
            m_arrDrawing[4].Y = y + fHeight;

            MakePath();
        }

        private void MakePath()
        {
            m_path.Reset();
            m_path.StartFigure();

            m_path.AddLine(m_arrDrawing[0].X, m_arrDrawing[0].Y, m_arrDrawing[1].X, m_arrDrawing[1].Y);
            m_path.AddLine(m_arrDrawing[1].X, m_arrDrawing[1].Y, m_arrDrawing[2].X, m_arrDrawing[2].Y);
            m_path.AddLine(m_arrDrawing[2].X, m_arrDrawing[2].Y, m_arrDrawing[3].X, m_arrDrawing[3].Y);
            m_path.AddLine(m_arrDrawing[3].X, m_arrDrawing[3].Y, m_arrDrawing[4].X, m_arrDrawing[4].Y);
            m_path.AddLine(m_arrDrawing[4].X, m_arrDrawing[4].Y, m_arrDrawing[0].X, m_arrDrawing[0].Y);

            m_path.CloseFigure();

            m_edgeLines[0] = m_arrDrawing[1];
            m_edgeLines[2] = m_arrDrawing[2];
            m_edgeLines[1] = new PointF(m_edgeLines[0].X, m_edgeLines[2].Y);
        }

        protected override bool Paint(Graphics g, float x, float y, PathNotifier notifier)
        {
            if (m_shape == null || m_colorSet == null)
                return false;

            g.SmoothingMode = SmoothingMode.AntiAlias;

            int nWidth = (int)m_shape.GetSize(true);
            int nHeight = (int)m_shape.GetSize(false);

            DrawShadow(g, m_path);

            LinearGradientBrush brush = new LinearGradientBrush(
                new Point((int)x + nWidth / 2, (int)y),
                new Point((int)x + nWidth / 2, (int)y + nHeight),
                m_colorSet.FillStartColor,
                m_colorSet.FillEndColor
            );
            g.FillPath(brush, m_path);
            brush.Dispose();

            Pen pen = new Pen(m_colorSet.LineColor);
            pen.Width = 4;
            g.DrawPath(pen, m_path);
            g.DrawLines(pen, m_edgeLines);

            pen.Dispose();
            return true;
        }
    }
}
