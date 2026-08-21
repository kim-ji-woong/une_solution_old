using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace CadToXML
{
    public partial class AlertArea
    {
        private List<PathItem> m_shapeInnerItems = new List<PathItem>();

        protected Vertex2D m_vTL = null;
        protected Vertex2D m_vBR = null;
        private Vertex2D m_vOriginTL = null;
        private Vertex2D m_vOriginBR = null;

        private GraphicsPath m_path = null;
        private Vertex2D m_vNamePos = null;
        private Polygon m_boundaryPolygon = null;

        public Vertex2D NamePosition
        {
            get { return m_vNamePos; }
            set { m_vNamePos = value; }
        }

        public List<PathItem> Boundary
        {
            get { return m_shapeInnerItems; }
        }

        public void AddLineBoundary(PathItem item)
        {
            m_shapeInnerItems.Add(item);
        }

        public bool MakeShape(Project.UnitOfLength unit)
        {
            m_path = MakeGraphicsPath(m_shapeInnerItems, this, ref m_boundaryPolygon);

            return true;
        }

        private static GraphicsPath MakeGraphicsPath(List<PathItem> items, AlertArea space, ref Polygon polygon, double x = 0.0, double y = 0.0)
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
                    space.NamePosition = vCenter;
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
        }
    }
}
