using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace SoilMan.Drawing
{
    public class MultiPoint : BoundingShape
    {
        private List<Vertex2F> m_vertices = new List<Vertex2F>();
        private System.Drawing.SolidBrush m_brush = null;

        public MultiPoint()
        {
            m_brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            // 선택되지 않도록 한다.
            Selectable = false;
        }

        public MultiPoint(List<Vertex2F> vertices)
        {
            m_vertices = vertices;
            m_brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            // 선택되지 않도록 한다.
            Selectable = false;
        }

        public void AddVertex(Vertex2F vertex)
        {
            m_vertices.Add(vertex);
        }

        public override UnE.Geometry.Vertex2D Position
        {
            get { return new UnE.Geometry.Vertex2D((m_dMinX + m_dMaxX) / 2, (m_dMinY + m_dMaxY) / 2); }
        }

        public override UnE.Geometry.Vertex2D BoundaryTL
        {
            get { return new UnE.Geometry.Vertex2D(m_dMinX, m_dMaxY); }
        }

        public override Vertex2D BoundaryBR
        {
            get { return new Vertex2D(m_dMaxX, m_dMinY); }
        }

        // (x,y)만큼 객체를 옮긴다.
        public override void Move(double x, double y)
        {
            foreach (Vertex2F vertex in m_vertices)
            {
                vertex.x += (float)x;
                vertex.y += (float)y;
            }
        }

        public override DXFViewer.Shape.ShapeType GetShapeType()
        {
            return DXFViewer.Shape.ShapeType.NONE;
        }

        public override DXFViewer.Shape Clone()
        {
            MultiPoint point = new MultiPoint();

            foreach (Vertex2F vertex in m_vertices)
            {
                point.m_vertices.Add(new Vertex2F(vertex.x, vertex.y));
            }

            return point;
        }

        // Selectable이 false이면 HitTest 검사가 무조건 실패한다.
        public override bool HitTest(double x, double y)
        {
            if (Selectable == false || m_attr == null)
                return false;

            Vertex2F vTarget = new Vertex2F((float)x, (float)y);

            foreach (Vertex2F vertex in m_vertices)
            {
                double distance = vertex.GetDistance(vTarget);

                if (distance <= m_attr.GetPointSize())
                    return true;
            }

            return false;
        }

        public override bool CheckClipBounds(System.Drawing.Graphics g, Vertex2D vClipTL, Vertex2D vClipBR)
        {
            DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)this.GetOwner();
            return CheckClipBounds(vClipTL - ctrl.MovedVertex, vClipBR - ctrl.MovedVertex, BoundaryTL, BoundaryBR);
        }

        public override bool Draw(System.Drawing.Graphics g, bool bDrawText)
        {
            if (m_attr == null)
                return false;

            double dSize = m_attr.GetPointSize();

            if (dSize <= 0.0)
                return false;

            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;
            layer.LastDrawingShape = this;

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

        public override void PostDraw(System.Drawing.Graphics g)
        {
            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;

            if (m_attr.GetFillColor() != System.Drawing.Color.Transparent && layer.PathFill != null)
            {
                m_brush.Color = m_attr.GetFillColor();
                g.FillPath(m_brush, layer.PathFill);
                layer.PathFill = null;
            }

            if (layer.PathLine != null)
            {
                System.Drawing.Pen pen = m_lineType.GetPen();
                SetPenWidth(pen, g, m_attr.GetLineThickness());

                g.DrawPath(pen, layer.PathLine);
                layer.PathLine = null;
            }

            GC.Collect();
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

            if (layer.PathLine == null)
            //if (FirstElement)
            {
                layer.PathLine = new System.Drawing.Drawing2D.GraphicsPath();
            }

            foreach (Vertex2F vertex in m_vertices)
            {
                if (type == PointDrawingType.CIRCLE)
                    layer.PathLine.AddEllipse((float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize);
                else if (type == PointDrawingType.RECTANGLE)
                    layer.PathLine.AddRectangle(new System.Drawing.RectangleF((float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize));
            }

            // PostDraw(...)에서 담당
            // CheckClipBounds() 호출로 인하여 가장 마지막에 그려질 객체가 누구인지 실시간으로 파악할 수 없으므로
            // 모든 Draw() 함수 호출 이후 마지막에 호출된 객체의 PostDraw()를 호출한다.
            /*if (LastElement)
            {
                SetPenWidth(pen, g, nLineThick);

                g.DrawPath(pen, layer.PathLine);
                layer.PathLine = null;
                GC.Collect();
            }*/
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

            if (layer.PathFill == null)
            //if (FirstElement)
            {
                layer.PathFill = new System.Drawing.Drawing2D.GraphicsPath();
            }

            foreach (Vertex2F vertex in m_vertices)
            {
                if (type == PointDrawingType.CIRCLE)
                    layer.PathFill.AddEllipse((float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize);
                else if (type == PointDrawingType.RECTANGLE)
                    layer.PathFill.AddRectangle(new System.Drawing.RectangleF((float)(vertex.x - dSize / 2), (float)(vertex.y - dSize / 2), (float)dSize, (float)dSize));
            }

            // PostDraw(...)에서 담당
            // CheckClipBounds() 호출로 인하여 가장 마지막에 그려질 객체가 누구인지 실시간으로 파악할 수 없으므로
            // 모든 Draw() 함수 호출 이후 마지막에 호출된 객체의 PostDraw()를 호출한다.
            /*if (LastElement)
            {
                m_brush.Color = m_attr.GetFillColor();

                g.FillPath(m_brush, layer.PathFill);
                layer.PathFill = null;
                GC.Collect();
            }*/
        }
    }
}
