using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace ShapeFileReader.Drawing
{
    public class Point : PointShape
    {
        private Vertex2D m_vertex = null;
        private System.Drawing.SolidBrush m_brush = null;

        public Point()
        {
            m_vertex = new Vertex2D();
            m_brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        }

        public Point(double x, double y)
        {
            m_vertex = new Vertex2D(x, y);
            m_brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        }

        public override Vertex2D Position
		{
			get { return m_vertex; }
		}

		public override Vertex2D BoundaryTL
		{
			get
            {
                if (m_attr == null || m_attr.GetPointSize() < 0.0)
                    return m_vertex;

                return new Vertex2D(m_vertex.x - m_attr.GetPointSize() / 2, m_vertex.x - m_attr.GetPointSize() / 2);
            }
		}

		public override Vertex2D BoundaryBR
		{
            get
            {
                if (m_attr == null || m_attr.GetPointSize() < 0.0)
                    return m_vertex;

                return new Vertex2D(m_vertex.x + m_attr.GetPointSize() / 2, m_vertex.x + m_attr.GetPointSize() / 2);
            }
		}

        // (x,y)만큼 객체를 옮긴다.
        public override void Move(double x, double y)
        {
            m_vertex.x += x;
            m_vertex.y += y;
        }

        public override DXFViewer.Shape.ShapeType GetShapeType()
        {
            return DXFViewer.Shape.ShapeType.NONE;
        }

        public override DXFViewer.Shape Clone()
        {
            Point point = new Point(m_vertex.x, m_vertex.y);
            return point;
        }

        // Selectable이 false이면 HitTest 검사가 무조건 실패한다.
        public override bool HitTest(double x, double y)
        {
            if (Selectable == false || m_attr == null)
                return false;

            double distance = m_vertex.GetDistance(new Vertex2D(x, y));
            return distance <= m_attr.GetPointSize();
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

			        m_editBox.Draw(g, (float)m_vertex.x, (float)m_vertex.y);
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

            /*BoundingShape.SetPenWidth(pen, g, nLineThick);

            if (type == PointDrawingType.CIRCLE)
                g.DrawEllipse(pen, (float)(m_vertex.x - dSize / 2), (float)(m_vertex.y - dSize / 2), (float)dSize, (float)dSize);
            else if (type == PointDrawingType.RECTANGLE)
                g.DrawRectangle(pen, (float)(m_vertex.x - dSize / 2), (float)(m_vertex.y - dSize / 2), (float)dSize, (float)dSize);*/

            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;

            if (FirstElement)
            {
                layer.PathLine = new System.Drawing.Drawing2D.GraphicsPath();
            }

            if (type == PointDrawingType.CIRCLE)
                layer.PathLine.AddEllipse((float)(m_vertex.x - dSize / 2), (float)(m_vertex.y - dSize / 2), (float)dSize, (float)dSize);
            else if (type == PointDrawingType.RECTANGLE)
                layer.PathLine.AddRectangle(new System.Drawing.RectangleF((float)(m_vertex.x - dSize / 2), (float)(m_vertex.y - dSize / 2), (float)dSize, (float)dSize));

            if (LastElement)
            {
                BoundingShape.SetPenWidth(pen, g, nLineThick);

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

            if (type == PointDrawingType.CIRCLE)
                g.FillEllipse(m_brush, (float)(m_vertex.x - dSize / 2), (float)(m_vertex.y - dSize / 2), (float)dSize, (float)dSize);
            else if (type == PointDrawingType.RECTANGLE)
                g.FillRectangle(m_brush, (float)(m_vertex.x - dSize / 2), (float)(m_vertex.y - dSize / 2), (float)dSize, (float)dSize);*/

            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;

            if (FirstElement)
            {
                layer.PathFill = new System.Drawing.Drawing2D.GraphicsPath();
            }

            if (type == PointDrawingType.CIRCLE)
                layer.PathFill.AddEllipse((float)(m_vertex.x - dSize / 2), (float)(m_vertex.y - dSize / 2), (float)dSize, (float)dSize);
            else if (type == PointDrawingType.RECTANGLE)
                layer.PathFill.AddRectangle(new System.Drawing.RectangleF((float)(m_vertex.x - dSize / 2), (float)(m_vertex.y - dSize / 2), (float)dSize, (float)dSize));

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
