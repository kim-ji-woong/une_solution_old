using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace ShapeFileReader.Drawing
{
    public class MultiPoint : BoundingShape
    {
        private List<Vertex2D> m_vertices = new List<Vertex2D>();
        private System.Drawing.SolidBrush m_brush = null;

        public MultiPoint()
        {
            m_brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        }

        public MultiPoint(List<Vertex2D> vertices)
        {
            m_vertices = vertices;
            m_brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        }

        public void AddVertex(Vertex2D vertex)
        {
            m_vertices.Add(vertex);
        }

        public override Vertex2D Position
        {
            get { return new Vertex2D((m_dMinX + m_dMaxX) / 2, (m_dMinY + m_dMaxY) / 2); }
        }

        public override Vertex2D BoundaryTL
        {
            get { return new Vertex2D(m_dMinX, m_dMaxY); }
        }

        public override Vertex2D BoundaryBR
        {
            get { return new Vertex2D(m_dMaxX, m_dMinY); }
        }

        // (x,y)만큼 객체를 옮긴다.
        public override void Move(double x, double y)
        {
            foreach (Vertex2D vertex in m_vertices)
            {
                vertex.x += x;
                vertex.y += y;
            }
        }

        public override DXFViewer.Shape.ShapeType GetShapeType()
        {
            return DXFViewer.Shape.ShapeType.NONE;
        }

        public override DXFViewer.Shape Clone()
        {
            MultiPoint point = new MultiPoint();

            foreach (Vertex2D vertex in m_vertices)
            {
                point.m_vertices.Add(new Vertex2D(vertex.x, vertex.y));
            }

            return point;
        }

        // Selectable이 false이면 HitTest 검사가 무조건 실패한다.
        public override bool HitTest(double x, double y)
        {
            if (Selectable == false || m_attr == null)
                return false;

            Vertex2D vTarget = new Vertex2D(x, y);

            foreach (Vertex2D vertex in m_vertices)
            {
                double distance = vertex.GetDistance(vTarget);

                if (distance <= m_attr.GetPointSize())
                    return true;
            }

            return false;
        }

        public override bool Draw(System.Drawing.Graphics g)
        {
            if (m_attr == null)
                return false;

            double dSize = m_attr.GetPointSize();

            if (dSize <= 0.0)
                return false;

            DrawFill(g, dSize);

            System.Drawing.Pen pen = m_lineType.GetPen();
            pen.Color = m_attr.GetLineColor();

            if (Selectable && Selected && m_selectedShowingType != DXFViewer.Shape.SelectedShowingType.NONE)
            {
                if (m_selectedShowingType == DXFViewer.Shape.SelectedShowingType.EDIT_BOX)
                {
                    DXFViewer.LineType lineType = m_pOwner.GetSelectedLineType();
                    System.Drawing.Color penColor = pen.Color;
                    pen = lineType.GetPen();
                    pen.Color = penColor;

                    DrawLine(g, pen, dSize, m_attr.GetLineThickness());

                    m_editBox.Draw(g, (float)m_dMinX, (float)m_dMinY);
                    m_editBox.Draw(g, (float)m_dMaxX, (float)m_dMaxY);
                }
                else if (m_selectedShowingType == DXFViewer.Shape.SelectedShowingType.BRIGHT_EFFECT)
                {
                    float fOldWidth = pen.Width;
                    DrawLine(g, pen, dSize, m_attr.GetLineThickness() + 1);
                    pen.Width = fOldWidth;

                    // 밝게 표현하기 위하여 배경색의 보색으로 그린다.
                    DrawLine(g, GetOwner().SelectedBrightPen1, dSize, m_attr.GetLineThickness());
                    // 패턴을 주기 위하여 배경색으로 다시한번 그린다.
                    DrawLine(g, GetOwner().SelectedBrightPen2, dSize, m_attr.GetLineThickness());
                }
            }
            else
                DrawLine(g, pen, dSize, m_attr.GetLineThickness());

            return true;
        }

        private void DrawLine(System.Drawing.Graphics g, System.Drawing.Pen pen, double dSize, int nLineThick)
        {
            if (nLineThick <= 0)
                return;

            //pen.Width = nLineThick;
            PointDrawingType type = m_attr.GetPointDrawingType();

            /*SetPenWidth(pen, g, nLineThick);

            foreach (Vertex2D vertex in m_vertices)
            {
                if (type == PointDrawingType.CIRCLE)
                    g.DrawEllipse(pen, (float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize);
                else if (type == PointDrawingType.RECTANGLE)
                    g.DrawRectangle(pen, (float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize);
            }*/

            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;

            if (FirstElement)
            {
                layer.PathLine = new System.Drawing.Drawing2D.GraphicsPath();
            }

            foreach (Vertex2D vertex in m_vertices)
            {
                if (type == PointDrawingType.CIRCLE)
                    layer.PathLine.AddEllipse((float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize);
                else if (type == PointDrawingType.RECTANGLE)
                    layer.PathLine.AddRectangle(new System.Drawing.RectangleF((float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize));
            }

            if (LastElement)
            {
                SetPenWidth(pen, g, nLineThick);

                g.DrawPath(pen, layer.PathLine);
                layer.PathLine = null;
                GC.Collect();
            }
        }

        private void DrawFill(System.Drawing.Graphics g, double dSize)
        {
            if (m_attr.GetFillColor() == System.Drawing.Color.Transparent)
                return;

            PointDrawingType type = m_attr.GetPointDrawingType();
            /*m_brush.Color = m_attr.GetFillColor();

            foreach (Vertex2D vertex in m_vertices)
            {
                if (type == PointDrawingType.CIRCLE)
                    g.FillEllipse(m_brush, (float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize);
                else if (type == PointDrawingType.RECTANGLE)
                    g.FillRectangle(m_brush, (float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize);
            }*/

            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;

            if (FirstElement)
            {
                layer.PathFill = new System.Drawing.Drawing2D.GraphicsPath();
            }

            foreach (Vertex2D vertex in m_vertices)
            {
                if (type == PointDrawingType.CIRCLE)
                    layer.PathFill.AddEllipse((float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize);
                else if (type == PointDrawingType.RECTANGLE)
                    layer.PathFill.AddRectangle(new System.Drawing.RectangleF((float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize));
            }

            if (LastElement)
            {
                m_brush.Color = m_attr.GetFillColor();

                g.FillPath(m_brush, layer.PathFill);
                layer.PathFill = null;
                GC.Collect();
            }
        }
    }
}
