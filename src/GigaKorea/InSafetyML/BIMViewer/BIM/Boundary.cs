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
    public class Boundary
    {
        private List<PathItem> m_boundary = new List<PathItem>();

        public void AddLine(Line2D line)
        {
            PathItem item = new PathItem();
            item.SetLine(line);
            m_boundary.Add(item);
        }

        public void AddArc(Arc2D arc)
        {
            PathItem item = new PathItem();
            item.SetArc(arc);
            m_boundary.Add(item);
        }

        public void AddEArc(EArc2D eArc)
        {
            PathItem item = new PathItem();
            item.SetEArc(eArc);
            m_boundary.Add(item);
        }

        public List<PathItem> GetBoundary()
        {
            return m_boundary;
        }

        public GraphicsPath GetPath()
        {
            // 바운더리 영역을 GraphicsPath 형으로 반환
            GraphicsPath path = new GraphicsPath();
            List<PathItem> items = new List<PathItem>();

            items = GetBoundary();

            foreach (PathItem item in items)
            {
                AddPath(path, item);
            }

            return path;
        }

        public static void AddPath(GraphicsPath path, PathItem item, double x = 0.0, double y = 0.0)
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
    }
}
