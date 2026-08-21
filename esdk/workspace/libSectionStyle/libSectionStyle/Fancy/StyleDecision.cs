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
    class StyleDecision : Style
    {
        public StyleDecision(SectionDecision section, Shape shape)
        {
            m_section = section;
            m_shape = shape;
            m_arrDrawing = new PointF[4];
        }

        protected override void Create(float x, float y)
        {
            if (m_shape == null)
                return;

            float fWidth = m_shape.GetSize(true);
            float fHeight = m_shape.GetSize(false);

            m_arrDrawing[0] = new PointF(x + fWidth / 2, y);
            m_arrDrawing[1] = new PointF(x + fWidth, y + fHeight / 2);
            m_arrDrawing[2] = new PointF(x + fWidth / 2, y + fHeight);
            m_arrDrawing[3] = new PointF(x, y + fHeight / 2);

            MakePath();
        }

        protected override void Resize(float x, float y, float fWidth, float fHeight)
        {
            if (m_shape == null)
                return;

            m_arrDrawing[0].X = x + fWidth / 2;
            m_arrDrawing[0].Y = y;
            m_arrDrawing[1].X = x + fWidth;
            m_arrDrawing[1].Y = y + fHeight / 2;
            m_arrDrawing[2].X = x + fWidth / 2;
            m_arrDrawing[2].Y = y + fHeight;
            m_arrDrawing[3].X = x;
            m_arrDrawing[3].Y = y + fHeight / 2;

            MakePath();
        }

        protected override void Move(float x, float y)
        {
            if (m_shape == null)
                return;

            float fWidth = m_shape.GetSize(true);
            float fHeight = m_shape.GetSize(false);

            m_arrDrawing[0].X = x + fWidth / 2;
            m_arrDrawing[0].Y = y;
            m_arrDrawing[1].X = x + fWidth;
            m_arrDrawing[1].Y = y + fHeight / 2;
            m_arrDrawing[2].X = x + fWidth / 2;
            m_arrDrawing[2].Y = y + fHeight;
            m_arrDrawing[3].X = x;
            m_arrDrawing[3].Y = y + fHeight / 2;

            MakePath();
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
