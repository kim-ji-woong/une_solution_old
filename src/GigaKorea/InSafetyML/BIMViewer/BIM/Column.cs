using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;
using System.Drawing.Drawing2D;

namespace BIMViewer.BIM
{
    using System.Drawing;
    using Shapes;

    public class Column : Shape
    {
        public class Rect
        {
            private Vertex2D m_vTL = null;
            private Vertex2D m_vBL = null;
            private Vertex2D m_vBR = null;
            private GraphicsPath m_path = new GraphicsPath();
            private Polygon m_boundaryPolygon = new Polygon();

            public Vertex2D TopLeft
            {
                get { return m_vTL; }
                set { m_vTL = value; }
            }

            public Vertex2D BottomLeft
            {
                get { return m_vBL; }
                set { m_vBL = value; }
            }

            public Vertex2D BottomRight
            {
                get { return m_vBR; }
                set { m_vBR = value; }
            }

            public void MakeShape(double x, double y, ref Vertex2D vTL, ref Vertex2D vBR)
            {
                GraphicsPath path = new GraphicsPath();

                Vertex2D vTR = m_vBR - m_vBL + m_vTL;

                PointF ptTL = new PointF((float)(m_vTL.x + x), (float)(m_vTL.y + y));
                PointF ptBL = new PointF((float)(m_vBL.x + x), (float)(m_vBL.y + y));
                PointF ptBR = new PointF((float)(m_vBR.x + x), (float)(m_vBR.y + y));
                PointF ptTR = new PointF((float)(vTR.x + x), (float)(vTR.y + y));

                path.AddLine(ptTL, ptBL);
                path.AddLine(ptBL, ptBR);
                path.AddLine(ptBR, ptTR);
                path.AddLine(ptTR, ptTL);

                m_path = path;

                vTL = new Vertex2D(m_vTL);
                vBR = new Vertex2D(m_vBR);

                Wall.SetBoundaryVertex(vTL, vBR, m_vTL);
                Wall.SetBoundaryVertex(vTL, vBR, m_vBL);
                Wall.SetBoundaryVertex(vTL, vBR, m_vBR);
                Wall.SetBoundaryVertex(vTL, vBR, vTR);

                vTL.x += x;
                vTL.y += y;
                vBR.x += x;
                vBR.y += y;

                m_boundaryPolygon.Clear();

                m_boundaryPolygon.AddVertex(new Vertex2D(m_vTL.x + x, m_vTL.y + y));
                m_boundaryPolygon.AddVertex(new Vertex2D(m_vBL.x + x, m_vBL.y + y));
                m_boundaryPolygon.AddVertex(new Vertex2D(m_vBR.x + x, m_vBR.y + y));
                m_boundaryPolygon.AddVertex(new Vertex2D(vTR.x + x, vTR.y + y));
            }

            public void Move(double dMoveX, double dMoveY, ref Vertex2D vTL, ref Vertex2D vBR)
            {
                MakeShape(dMoveX, dMoveY, ref vTL, ref vBR);
            }

            public bool HitTest(Vertex2D vertex)
            {
                if (m_boundaryPolygon.HitTest(vertex) != 0)
                    return true;

                return false;
            }

            public void DrawEdge(Graphics g, Pen pen)
            {
                g.DrawPath(pen, m_path);
            }

            public void DrawFill(Graphics g, Brush brush)
            {
                g.FillPath(brush, m_path);
            }
        }

        public class Circle
        {
            private Vertex2D m_vCenter = null;
            private Vertex2D m_vMovedCenter = null;
            private double m_dRadius = 0.0;
            private RectangleF m_rect;

            public Vertex2D Center
            {
                get { return m_vCenter; }
                set { m_vCenter = value; }
            }

            public double Radius
            {
                get { return m_dRadius; }
                set { m_dRadius = value; }
            }

            public void MakeShape(double x, double y, ref Vertex2D vTL, ref Vertex2D vBR)
            {
                m_vMovedCenter = new Vertex2D(m_vCenter.x + x, m_vCenter.y + y);

                float fSize = (float)(m_dRadius * 2);
                m_rect = new RectangleF((float)(m_vMovedCenter.x - m_dRadius), (float)(m_vMovedCenter.y - m_dRadius), fSize, fSize);

                vTL = new Vertex2D(m_vMovedCenter.x - m_dRadius, m_vMovedCenter.y + m_dRadius);
                vBR = new Vertex2D(m_vMovedCenter.x + m_dRadius, m_vMovedCenter.y - m_dRadius);
            }

            public void Move(double dMoveX, double dMoveY, ref Vertex2D vTL, ref Vertex2D vBR)
            {
                MakeShape(dMoveX, dMoveY, ref vTL, ref vBR);
            }

            public bool HitTest(Vertex2D vertex)
            {
                if (m_vMovedCenter == null)
                    return false;

                return m_vMovedCenter.GetDistance(vertex) <= m_dRadius;
            }

            public void DrawEdge(Graphics g, Pen pen)
            {
                g.DrawEllipse(pen, m_rect);
            }

            public void DrawFill(Graphics g, Brush brush)
            {
                g.FillEllipse(brush, m_rect);
            }
        }

        public enum ColumnType { Rect = 0, Circle };

        private int m_nID = 0;
        private string m_strXMLID = "";
        
        private ColumnType m_type = ColumnType.Rect;
        private Rect m_rect = null;
        private Circle m_circle = null;
        private List<Property> m_properties = new List<Property>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string XMLID
        {
            get { return m_strXMLID; }
            set { m_strXMLID = value; }
        }

        public ColumnType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public Rect RectData
        {
            get { return m_rect; }
            set { m_rect = value; }
        }

        public Circle CircleData
        {
            get { return m_circle; }
            set { m_circle = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public void MakeShape(double x = 0.0, double y = 0.0)
        {
            if (m_type == ColumnType.Rect && m_rect != null)
            {
                m_rect.MakeShape(x, y, ref m_vTL, ref m_vBR);
            }
            else if (m_type == ColumnType.Circle && m_circle != null)
            {
                m_circle.MakeShape(x, y, ref m_vTL, ref m_vBR);
            }
        }

        public override void Move(double dMoveX, double dMoveY)
        {
            if (m_type == ColumnType.Rect && m_rect != null)
            {
                m_rect.Move(dMoveX, dMoveY, ref m_vTL, ref m_vBR);
            }
            else if (m_type == ColumnType.Circle && m_circle != null)
            {
                m_circle.Move(dMoveX, dMoveY, ref m_vTL, ref m_vBR);
            }
        }

        public override bool HitTest(Vertex2D vertex)
        {
            if (m_type == ColumnType.Rect && m_rect != null)
            {
                return m_rect.HitTest(vertex);
            }
            else if (m_type == ColumnType.Circle && m_circle != null)
            {
                return m_circle.HitTest(vertex);
            }

            return false;
        }

        public override void Render(Graphics g, Pen pen, Brush brush, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            if (m_vTL == null)
                return;

            if (m_vBR.x <= vClientAreaTL.x || m_vTL.x >= vClientAreaBR.x)
                return;

            if (m_vTL.y <= vClientAreaBR.y || m_vBR.y >= vClientAreaTL.y)
                return;

            if (m_selected)
            {
                Brush selectedBrush = new SolidBrush(m_selectedFillColor);

                if (m_type == ColumnType.Rect && m_rect != null)
                {
                    m_rect.DrawFill(g, selectedBrush);
                }
                else if (m_type == ColumnType.Circle && m_circle != null)
                {
                    m_circle.DrawFill(g, selectedBrush);
                }

                selectedBrush.Dispose();
            }
            else
            {
                if (pen != null)
                {
                    if (m_type == ColumnType.Rect && m_rect != null)
                    {
                        m_rect.DrawEdge(g, pen);
                    }
                    else if (m_type == ColumnType.Circle && m_circle != null)
                    {
                        m_circle.DrawEdge(g, pen);
                    }
                }

                if (brush != null)
                {
                    if (m_type == ColumnType.Rect && m_rect != null)
                    {
                        m_rect.DrawFill(g, brush);
                    }
                    else if (m_type == ColumnType.Circle && m_circle != null)
                    {
                        m_circle.DrawFill(g, brush);
                    }
                }
            }
        }
    }
}
