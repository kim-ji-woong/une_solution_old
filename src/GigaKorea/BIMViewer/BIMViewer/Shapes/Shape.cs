using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;
using System.Drawing;

namespace BIMViewer.Shapes
{
    public abstract class Shape
    {
        protected Vertex2D m_vTL = null;
        protected Vertex2D m_vBR = null;

        protected Color m_clrText = Color.Black;
        protected Font m_font = null;

        protected Layer m_layer = null;
        protected bool m_selected = false;

        protected Color m_selectedFillColor = Color.FromArgb(137, 203, 250);
        protected Color m_selectedLineColor = Color.FromArgb(137, 203, 250);

        public Vertex2D TopLeft
        {
            get { return m_vTL; }
        }

        public Vertex2D BottomRight
        {
            get { return m_vBR; }
        }

        public Color TextColor
        {
            get { return m_clrText; }
            set { m_clrText = value; }
        }

        public Font Font
        {
            get { return m_font; }
            set { m_font = value; }
        }

        public Layer Layer
        {
            get { return m_layer; }
            set { m_layer = value; }
        }

        public bool Selected
        {
            get { return m_selected; }
            set { m_selected = value; }
        }

        public abstract void Render(Graphics g, Pen pen, Brush brush, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR);
        public abstract void Move(double dMoveX, double dMoveY);

        protected bool CheckClipArea(Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            if (m_vBR.x <= vClientAreaTL.x || m_vTL.x >= vClientAreaBR.x)
                return false;

            if (m_vTL.y <= vClientAreaBR.y || m_vBR.y >= vClientAreaTL.y)
                return false;

            return true;
        }

        public virtual bool HitTest(Vertex2D vertex)
        {
            if (vertex.x >= m_vTL.x && vertex.x <= m_vBR.x &&
                    vertex.y >= m_vBR.y && vertex.y <= m_vTL.y)
                return true;

            return false;
        }
    }

    public class Line : Shape
    {
        private Vertex2D m_vBegin = null, m_vEnd = null;

        public Vertex2D Begin
        {
            get { return m_vBegin; }
            set { m_vBegin = value;SetBoundary(value); }
        }

        public Vertex2D End
        {
            get { return m_vEnd; }
            set { m_vEnd = value; SetBoundary(value); }
        }

        private void SetBoundary(Vertex2D vertex)
        {
            if (m_vTL == null)
            {
                m_vTL = new Vertex2D(vertex);
                m_vBR = new Vertex2D();
            }
            else
            {
                if (m_vTL.x > vertex.x)
                    m_vTL.x = vertex.x;
                if (m_vTL.y < vertex.y)
                    m_vTL.y = vertex.y;
                if (m_vBR.x < vertex.x)
                    m_vBR.x = vertex.x;
                if (m_vBR.y > vertex.y)
                    m_vBR.y = vertex.y;
            }
        }

        public override void Render(Graphics g, Pen pen, Brush brush, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            if (pen != null && m_vBegin != null)
                g.DrawLine(pen, new PointF((float)m_vBegin.x, (float)m_vBegin.y), new PointF((float)m_vEnd.x, (float)m_vEnd.y));
        }

        public override void Move(double dMoveX, double dMoveY)
        {
            if (m_vBegin != null)
            {
                m_vBegin.x += dMoveX;
                m_vBegin.y += dMoveY;
                m_vEnd.x += dMoveX;
                m_vEnd.y += dMoveY;
            }
        }
    }
}
