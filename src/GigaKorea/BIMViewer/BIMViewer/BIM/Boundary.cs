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
        //private List<Line2D> m_lines = new List<Line2D>();
        //private List<Arc2D> m_arcs = new List<Arc2D>();
        //private List<EArc2D> m_earc = new List<EArc2D>();
        private List<PathItem> m_boundary = new List<PathItem>();

        //public List<Line2D> Lines
        //{
        //    get { return m_lines; }
        //    set { m_lines = value; }
        //}

        //public List<Arc2D> Arcs
        //{
        //    get { return m_arcs; }
        //    set { m_arcs = value; }
        //}

        //public List<EArc2D> EArcs
        //{
        //    get { return m_earc; }
        //    set { m_earc = value; }
        //}

        public void AddLine(Line2D line)
        {
            //m_lines.Add(line);

            PathItem item = new PathItem();
            item.SetLine(line);
            m_boundary.Add(item);
        }

        public void AddArc(Arc2D arc)
        {
            //m_arcs.Add(arc);

            PathItem item = new PathItem();
            item.SetArc(arc);
            m_boundary.Add(item);
        }

        public void AddEArc(EArc2D eArc)
        {
            //m_earc.Add(eArc);

            PathItem item = new PathItem();
            item.SetEArc(eArc);
            m_boundary.Add(item);
        }

        public List<PathItem> GetBoundary()
        {
            //List<PathItem> items = new List<PathItem>();

            //foreach (Line2D line in m_lines)
            //{
            //    PathItem item = new PathItem();
            //    item.SetLine(line);
            //    items.Add(item);

            //    item.SetArc
            //}

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
