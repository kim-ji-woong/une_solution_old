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
    class StyleProcess : Style
    {
        private static Image m_circleImageAutoRun = null;
        private static float m_fCircleDiameter = 10;

        public StyleProcess(SectionProcess section, ShapeProcess shape)
        {
            m_section = section;
            m_shape = shape;
            m_arrDrawing = new PointF[4];

            if (m_circleImageAutoRun == null)
            {
                m_circleImageAutoRun = global::SectionStyle.Properties.Resources.autoRun;
                m_fCircleDiameter = m_circleImageAutoRun.Size.Width;
            }
        }

        protected override void Create(float x, float y)
        {
            if (m_shape == null)
                return;

            float fWidth = m_shape.GetSize(true);
            float fHeight = m_shape.GetSize(false);

            m_arrDrawing[0] = new PointF(x, y);
            m_arrDrawing[1] = new PointF(x + fWidth, y);
            m_arrDrawing[2] = new PointF(x + fWidth, y + fHeight);
            m_arrDrawing[3] = new PointF(x, y + fHeight);

            MakePath();
        }

        protected override void Resize(float x, float y, float fWidth, float fHeight)
        {
            if (m_shape == null)
                return;

            m_arrDrawing[0].X = x;
            m_arrDrawing[0].Y = y;
            m_arrDrawing[1].X = x + fWidth;
            m_arrDrawing[1].Y = y;
            m_arrDrawing[2].X = x + fWidth;
            m_arrDrawing[2].Y = y + fHeight;
            m_arrDrawing[3].X = x;
            m_arrDrawing[3].Y = y + fHeight;

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
            m_arrDrawing[1].X = x + fWidth;
            m_arrDrawing[1].Y = y;
            m_arrDrawing[2].X = x + fWidth;
            m_arrDrawing[2].Y = y + fHeight;
            m_arrDrawing[3].X = x;
            m_arrDrawing[3].Y = y + fHeight;

            MakePath();
        }

        protected override bool Paint(Graphics g, float x, float y, PathNotifier notifier)
        {
            if (base.Paint(g, x, y, notifier) == false)
                return false;

            if (((SectionDataProcess)m_section.Data).AutoRun)
                DrawIcon(m_circleImageAutoRun, g);

            return true;
        }

        private void DrawIcon(Image img, Graphics g)
        {
            float x = m_arrDrawing[3].X - m_fCircleDiameter / 4;
            float y = m_arrDrawing[3].Y - m_fCircleDiameter / 4;
            g.DrawImage(img, x, y, m_fCircleDiameter / 2, m_fCircleDiameter / 2);
        }

        private void MakePath()
        {
            m_path.Reset();
            m_path.StartFigure();

            m_path.AddLine(m_arrDrawing[0].X, m_arrDrawing[0].Y, m_arrDrawing[1].X, m_arrDrawing[1].Y);
            m_path.AddLine(m_arrDrawing[1].X, m_arrDrawing[1].Y, m_arrDrawing[2].X, m_arrDrawing[2].Y);
            m_path.AddLine(m_arrDrawing[2].X, m_arrDrawing[2].Y, m_arrDrawing[3].X, m_arrDrawing[3].Y);
            m_path.AddLine(m_arrDrawing[3].X, m_arrDrawing[3].Y, m_arrDrawing[0].X, m_arrDrawing[0].Y);

            m_path.CloseFigure();
        }
    }
}
