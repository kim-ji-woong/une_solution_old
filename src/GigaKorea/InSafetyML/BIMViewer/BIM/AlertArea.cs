using BIMViewer.Shapes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace BIMViewer.BIM
{
    public class AlertArea : Shape
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private string m_strName = "";

        private List<Property> m_properties = new List<Property>();
        private Level m_level = null;

        private Polygon m_polygon = new Polygon();

        private GraphicsPath m_path = null;
        private Vertex2D m_vNamePosition = null;

        private Vertex2D m_vOriginTL = null;
        private Vertex2D m_vOriginBR = null;

        private Boundary m_boundary = null;
        public Boundary Boundary
        {
            get { return m_boundary; }
            set { m_boundary = value; }
        }

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

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public Level Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public Vertex2D NamePosition
        {
            get { return m_vNamePosition; }
            set { m_vNamePosition = value; }
        }

        public bool MakeShape(Layer layer, Project.UnitOfLength unit)
        {
            // 바운더리 데이터로 공간영역 넣기
            SetBoundary(this.Boundary.GetBoundary());
            m_path = MakeGraphicsPath(Boundary.GetBoundary(), this, ref m_polygon);


            if (layer != null)
                layer.AddShape(this);

            return true;
        }

        private static GraphicsPath MakeGraphicsPath(List<PathItem> items, AlertArea alertArea, ref Polygon polygon, double x = 0.0, double y = 0.0)
        {
            GraphicsPath path = MakeGraphicsPath(items, x, y);

            if (path != null)
            {
                polygon = new Polygon();
                Vertex2D vBegin = null, vEnd = null, vMiddle = null;

                foreach (PathItem item in items)
                {
                    item.GetVertex(out vBegin, out vEnd, out vMiddle);

                    if (item.GetDrawType() == PathItem.DrawType.Line)
                    {
                        polygon.AddVertex(new Vertex2D(vEnd.x + x, vEnd.y + y));
                    }
                    else if (item.GetDrawType() == PathItem.DrawType.Arc || item.GetDrawType() == PathItem.DrawType.EArc)
                    {
                        polygon.AddVertex(new Vertex2D(vMiddle.x + x, vMiddle.y + y));
                        polygon.AddVertex(new Vertex2D(vEnd.x + x, vEnd.y + y));
                    }
                }

                int nVertexCount = polygon.GetVertexCount();

                if (nVertexCount >= 3)
                {
                    Vertex2D vCenter = polygon.CalcWeightCenter();
                    //alertArea.NamePosition = vCenter;
                }
            }

            return path;
        }

        public static GraphicsPath MakeGraphicsPath(List<PathItem> items, double x = 0.0, double y = 0.0)
        {
            GraphicsPath path = new GraphicsPath();

            foreach (PathItem item in items)
            {
                AddPath(path, item, x, y);
            }

            return path;
        }

        public static void AddPath(GraphicsPath path, PathItem item, double x, double y)
        {
            if (item.GetDrawType() == PathItem.DrawType.Line)
            {
                Vertex2D vBegin = null, vEnd = null, vMiddle = null;
                item.GetVertex(out vBegin, out vEnd, out vMiddle);

                PointF ptBegin = new PointF((float)(vBegin.x + x), (float)(vBegin.y + y));
                PointF ptEnd = new PointF((float)(vEnd.x + x), (float)(vEnd.y + y));

                path.AddLine(ptBegin, ptEnd);
            }
            else if (item.GetDrawType() == PathItem.DrawType.Arc || item.GetDrawType() == PathItem.DrawType.EArc)
            {
                EArc2D earc = item.GetEArc();

                if (earc != null)
                {
                    Vertex2D vTL = earc.GetTL();
                    Vertex2D vBL = earc.GetBL();
                    Vertex2D vBR = earc.GetBR();

                    // 타원의 축이 좌표축과 일치하는지 검사
                    Vertex2D vTop = new Vertex2D(vBL.x, vBL.y + 100);
                    double angle = UnE.Geometry.Math.GetAngle(vTL, vBL, vTop);

                    if (angle <= UnE.Geometry.Math.HALF_TOLERANCE())
                    {
                        RectangleF rect = new RectangleF((float)(vBL.x + x), (float)(vBL.y + y), (float)vBL.GetDistance(vBR), (float)vBL.GetDistance(vTL));

                        // Degree
                        float fBeginAngle = (float)UnE.Geometry.Math.RadToDeg(earc.GetBeginAngle());
                        float fEArcAngle = (float)UnE.Geometry.Math.RadToDeg(earc.GetAngle());

                        if (earc.IsClockWise())
                            fEArcAngle = -fEArcAngle;

                        path.AddArc(rect, fBeginAngle, fEArcAngle);
                    }
                }
                else
                {
                    double dBeginAngle = EArc2D.ValidAngle(earc.GetBeginAngle());
                    double dEndAngle = earc.GetEndAngle();
                    double dEArcAngle = earc.GetAngle();
                    int nPointCount = (int)(100 * dEArcAngle / UnE.Geometry.Math._2PI());
                    double dAngle = earc.IsClockWise() ? -dEArcAngle / nPointCount : dEArcAngle / nPointCount;

                    Vertex2D vBegin = earc.GetBeginVertex();
                    PointF[] points = new PointF[nPointCount + 1];
                    points[0] = new PointF((float)vBegin.x, (float)vBegin.y);

                    Vertex2D vertex;

                    for (int i = 1; i <= nPointCount; i++)
                    {
                        double dTheta = dBeginAngle + dAngle * i;

                        if (earc.GetVertex(dTheta, out vertex))
                            points[i] = new PointF((float)vertex.x, (float)vertex.y);
                        else
                            return;
                    }

                    path.AddLines(points);
                }
            }
        }

        private void SetBoundary(List<PathItem> boundary)
        {
            Vertex2D vTL = null, vBR = null;
            Vertex2D vBegin = null, vEnd = null, vMiddle = null;

            foreach (PathItem item in boundary)
            {
                if (item.GetVertex(out vBegin, out vEnd, out vMiddle))
                {
                    if (vTL == null)
                    {
                        vTL = new Vertex2D(vBegin.x, vBegin.y);
                        vBR = new Vertex2D(vBegin.x, vBegin.y);
                    }
                    else
                    {
                        SetBoundary(vTL, vBR, vBegin);
                    }

                    SetBoundary(vTL, vBR, vEnd);

                    if (vMiddle != null)
                        SetBoundary(vTL, vBR, vMiddle);
                }
            }

            m_vTL = vTL;
            m_vBR = vBR;
            m_vOriginTL = new Vertex2D(m_vTL);
            m_vOriginBR = new Vertex2D(m_vBR);
        }

        private void SetBoundary(UnE.Geometry.Vertex2D vTL, UnE.Geometry.Vertex2D vBR, UnE.Geometry.Vertex2D vPos)
        {
            if (vTL.x > vPos.x)
                vTL.x = vPos.x;
            if (vTL.y < vPos.y)
                vTL.y = vPos.y;
            if (vBR.x < vPos.x)
                vBR.x = vPos.x;
            if (vBR.y > vPos.y)
                vBR.y = vPos.y;
        }

        public override void Render(Graphics g, Pen pen, Brush brush, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            if (m_vBR.x <= vClientAreaTL.x || m_vTL.x >= vClientAreaBR.x)
                return;

            if (m_vTL.y <= vClientAreaBR.y || m_vBR.y >= vClientAreaTL.y)
                return;

            if (m_path != null)
            {
                // 공간 라인 그리는 곳
                if (m_selected)
                {
                    Brush selectedBrush = new SolidBrush(m_selectedFillColor);
                    g.FillPath(selectedBrush, m_path);
                    selectedBrush.Dispose();
                }
                else
                {
                    if (pen != null)
                        g.DrawPath(pen, m_path);

                    if (brush != null)
                        g.FillPath(brush, m_path);

                    if (m_layer != null && m_layer.VisibleText)
                    {
                        g.DrawString(m_strName, m_layer.Font, m_layer.TextBrush, (float)m_vNamePosition.x, (float)m_vNamePosition.y);
                    }
                }
            }
        }

        public override void Move(double dMoveX, double dMoveY)
        {
            if (m_path != null)
            {
                m_path.Dispose();
                m_path = MakeGraphicsPath(this.Boundary.GetBoundary(), this, ref m_polygon, dMoveX, dMoveY);
                //m_path = MakeGraphicsPath(m_shapeInnerItems, this, ref m_polygon, dMoveX, dMoveY);
                //m_path = MakeGraphicsPath(m_shapeInnerItems, dMoveX, dMoveY);
            }

            m_vTL.x = m_vOriginTL.x + dMoveX;
            m_vTL.y = m_vOriginTL.y + dMoveY;
            m_vBR.x = m_vOriginBR.x + dMoveX;
            m_vBR.y = m_vOriginBR.y + dMoveY;
        }
    }
}
