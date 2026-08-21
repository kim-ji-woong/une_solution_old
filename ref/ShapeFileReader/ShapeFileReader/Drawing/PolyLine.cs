using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace ShapeFileReader.Drawing
{
    public class PolyLine : BoundingShape
    {
        private List<System.Drawing.PointF[]> m_arrVertices = new List<System.Drawing.PointF[]>();
        
        public PolyLine()
        {
        }

        public PolyLine(List<List<Vertex2D>> arrVertices, double dScale, Vertex2D vCenter)
        {
            m_arrVertices.Clear();

            foreach (List<Vertex2D> vertices in arrVertices)
            {
                int nVertexCount = vertices.Count();

                if (nVertexCount == 0)
                    continue;

                System.Drawing.PointF[] arrPoints = new System.Drawing.PointF[nVertexCount];
                m_arrVertices.Add(arrPoints);

                for (int i=0;i<nVertexCount;i++)
                {
                    Vertex2D vertex = vertices[i];
                    Vertex2D vertex2 = ScaleTransfer(vertex.x, vertex.y, dScale, vCenter);

                    arrPoints[i].X = (float)vertex2.x;
                    arrPoints[i].Y = (float)vertex2.y;
                }
            }
        }

        public void AddVertices(List<Vertex2D> vertices, double dScale, Vertex2D vCenter)
        {
            int nVertexCount = vertices.Count();

            if (nVertexCount == 0)
                return;

            System.Drawing.PointF[] points = new System.Drawing.PointF[nVertexCount];

            for (int i = 0; i < nVertexCount;i++ )
            {
                Vertex2D vertex = vertices[i];
                Vertex2D vertex2 = ScaleTransfer(vertex.x, vertex.y, dScale, vCenter);

                points[i].X = (float)vertex2.x;
                points[i].Y = (float)vertex2.y;
            }

            m_arrVertices.Add(points);
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
            foreach (System.Drawing.PointF[] vertices in m_arrVertices)
            {
                int nVertexCount = vertices.Count();

                for (int i=0;i<nVertexCount;i++)
                {
                    vertices[i].X += (float)x;
                    vertices[i].Y += (float)y;
                }
            }
        }

        public override DXFViewer.Shape.ShapeType GetShapeType()
        {
            return DXFViewer.Shape.ShapeType.NONE;
        }

        public override DXFViewer.Shape Clone()
        {
            PolyLine polyLine = new PolyLine();

            foreach (System.Drawing.PointF[] vertices in m_arrVertices)
            {
                int nVertexCount = vertices.Count();
                if (nVertexCount == 0)
                    continue;

                System.Drawing.PointF[] vertices2 = new System.Drawing.PointF[nVertexCount];

                for (int i = 0; i < nVertexCount; i++)
                {
                    vertices2[i].X = vertices[i].X;
                    vertices2[i].Y = vertices[i].Y;
                }
            }

            return polyLine;
        }

        // Selectable이 false이면 HitTest 검사가 무조건 실패한다.
        public override bool HitTest(double x, double y)
        {
            if (Selectable == false || m_attr == null)
                return false;

            if (x >= m_dMinX && x <= m_dMaxX && y >= m_dMinY && y <= m_dMaxY)
                return true;

            return false;
        }

        public override bool Draw(System.Drawing.Graphics g)
        {
            if (m_attr == null)
                return false;

            double dSize = m_attr.GetPointSize();

            if (dSize <= 0.0)
                return false;

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

                    DrawLines(g, pen, m_attr.GetLineThickness());

                    m_editBox.Draw(g, (float)m_dMinX, (float)m_dMinY);
                    m_editBox.Draw(g, (float)m_dMaxX, (float)m_dMaxY);
                }
                else if (m_selectedShowingType == DXFViewer.Shape.SelectedShowingType.BRIGHT_EFFECT)
                {
                    float fOldWidth = pen.Width;

                    DrawLines(g, pen, m_attr.GetLineThickness() + 1);

                    pen.Width = fOldWidth;

                    // 밝게 표현하기 위하여 배경색의 보색으로 그린다.
                    DrawLines(g, GetOwner().SelectedBrightPen1, m_attr.GetLineThickness());
                    // 패턴을 주기 위하여 배경색으로 다시한번 그린다.
                    DrawLines(g, GetOwner().SelectedBrightPen2, m_attr.GetLineThickness());
                }
            }
            else
                DrawLines(g, pen, m_attr.GetLineThickness());

            return true;
        }

        private void DrawLines(System.Drawing.Graphics g, System.Drawing.Pen pen, int nLineThick)
        {
            if (nLineThick <= 0)
                return;

            //pen.Width = nLineThick;
            /*SetPenWidth(pen, g, nLineThick);

            foreach (System.Drawing.PointF[] vertices in m_arrVertices)
            {
                g.DrawLines(pen, vertices);
            }*/

            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;

            if (FirstElement)
            {
                layer.PathLine = new System.Drawing.Drawing2D.GraphicsPath();
            }

            foreach (System.Drawing.PointF[] vertices in m_arrVertices)
            {
                layer.PathLine.AddLines(vertices);
            }

            if (LastElement)
            {
                SetPenWidth(pen, g, nLineThick);

                g.DrawPath(pen, layer.PathLine);
                layer.PathLine = null;
                GC.Collect();
            }
        }
    }
}
