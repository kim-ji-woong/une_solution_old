using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BIMViewer.BIM
{
    using Shapes;

    public class Window : Shape
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private float m_fWidth = 0.0f;
        private float m_fHeight = 0.0f;
        private float m_fElevation = 0.0f;
        private float m_fThick = 100.0f;
        private Vertex2D m_vPos = null;
        private Wall m_wall = null;

        private List<Property> m_properties = new List<Property>();

        private GraphicsPath m_path = null;
        private Polygon m_boundaryPolygon = new Polygon();

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

        public Vertex2D Position
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public float Width
        {
            get { return m_fWidth; }
            set { m_fWidth = value; }
        }

        public float Height
        {
            get { return m_fHeight; }
            set { m_fHeight = value; }
        }

        public float Elevation
        {
            get { return m_fElevation; }
            set { m_fElevation = value; }
        }

        public Wall Wall
        {
            get { return m_wall; }
            set { m_wall = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public float Thick
        {
            get { return m_fThick; }
            set { m_fThick = value; }
        }

        public void MakeShape(double x = 0.0, double y = 0.0)
        {
            if (m_wall == null || m_vPos == null || m_fWidth <= 0.0f)
                return;

            if (m_wall.GetGridType() == Wall.GridType.Line)
            {
                MakeLineWindow(x, y);
            }
            else if (m_wall.GetGridType() == Wall.GridType.Arc || m_wall.GetGridType() == Wall.GridType.EArc)
            {
                MakeEArcWindow(x, y);
            }

            if (m_path != null)
            {
                m_boundaryPolygon.Clear();

                foreach (PointF point in m_path.PathPoints)
                {
                    m_boundaryPolygon.AddVertex(new Vertex2D(point.X, point.Y));
                }
            }
        }

        // earc위의 점 vPos에서 earc의 진행방향 양쪽으로 각각 dWidth/2 만큼씩 떨어진 점 두개를 구하여
        // 그 두개의 점으로 이루어진 새로운 EArc 객체를 만들어 리턴한다.
        public static EArc2D MakeSubEArc(EArc2D earc, Vertex2D vPos, double dWidth)
        {
            Vertex2D v1, v2;

            if (earc.GetLinearVertex(vPos, dWidth / 2, out v1) == false ||
                earc.GetLinearVertex(vPos, -dWidth / 2, out v2) == false)
                return null;

            double dAngle1 = PathItem.GetEArcAngle(earc, v1);
            double dAngle2 = PathItem.GetEArcAngle(earc, v2);
            double dBeginAngle = dAngle1, dEArcAngle = 0.0;

            if (earc.IsClockWise())
            {
                if (dAngle1 > dAngle2)
                {
                    if (dAngle1 - dAngle2 > UnE.Geometry.Math.PI())
                    {
                        dBeginAngle = dAngle2;
                        dEArcAngle = UnE.Geometry.Math._2PI() - (dAngle1 - dAngle2);
                    }
                    else
                        dEArcAngle = dAngle1 - dAngle2;
                }
                else
                {
                    if (dAngle2 - dAngle1 <= UnE.Geometry.Math.PI())
                    {
                        dBeginAngle = dAngle2;
                        dEArcAngle = dAngle2 - dAngle1;
                    }
                    else
                        dEArcAngle = UnE.Geometry.Math._2PI() - (dAngle2 - dAngle1);
                }
            }
            else
            {
                if (dAngle1 > dAngle2)
                {
                    if (dAngle1 - dAngle2 <= UnE.Geometry.Math.PI())
                    {
                        dBeginAngle = dAngle2;
                        dEArcAngle = dAngle1 - dAngle2;
                    }
                    else
                        dEArcAngle = UnE.Geometry.Math._2PI() - (dAngle1 - dAngle2);
                }
                else
                {
                    if (dAngle2 - dAngle1 > UnE.Geometry.Math.PI())
                    {
                        dBeginAngle = dAngle2;
                        dEArcAngle = UnE.Geometry.Math._2PI() - (dAngle2 - dAngle1);
                    }
                    else
                        dEArcAngle = dAngle2 - dAngle1;
                }
            }

            if (earc is Arc2D)
            {
                Arc2D arc = new Arc2D(earc.GetCenter(), ((Arc2D)earc).GetRadius(), dBeginAngle, dEArcAngle, earc.IsClockWise());
                return arc;
            }

            EArc2D earc2 = new EArc2D(earc.GetTL(), earc.GetBL(), earc.GetBR(), dBeginAngle, dEArcAngle, earc.IsClockWise());
            return earc2;
        }

        private void MakeEArcWindow(double x, double y)
        {
            EArc2D earcOrigin = m_wall.GetGridType() == Wall.GridType.Arc ? m_wall.Arc : m_wall.EArc;

            if (earcOrigin == null)
                return;

            EArc2D earc = MakeSubEArc(earcOrigin, m_vPos, m_fWidth);

            if (earc == null)
                return;

            EArc2D earc1 = earc.Offset(true, m_fThick / 2);
            EArc2D earc2 = earc.Offset(false, m_fThick / 2);

            earc2.SetEArc(earc2.GetTL(), earc2.GetBL(), earc2.GetBR(), earc2.GetEndAngle(), earc2.GetAngle(), !earc2.IsClockWise());

            Vertex2D v1 = earc1.GetBeginVertex();
            Vertex2D v2 = earc1.GetEndVertex();
            Vertex2D v3 = earc2.GetBeginVertex();
            Vertex2D v4 = earc2.GetEndVertex();

            PointF pt1 = new PointF((float)(v1.x + x), (float)(v1.y + y));
            PointF pt2 = new PointF((float)(v2.x + x), (float)(v2.y + y));
            PointF pt3 = new PointF((float)(v3.x + x), (float)(v3.y + y));
            PointF pt4 = new PointF((float)(v4.x + x), (float)(v4.y + y));

            GraphicsPath path = new GraphicsPath();

            AddEArcPath(path, earc1, x, y);
            path.AddLine(pt2, pt3);
            AddEArcPath(path, earc2, x, y);
            path.AddLine(pt4, pt1);

            m_vTL = new Vertex2D(earc1.GetTL());
            m_vBR = new Vertex2D(earc1.GetBR());

            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc1.GetTL());
            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc1.GetBL());
            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc1.GetBR());
            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc2.GetTL());
            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc2.GetBL());
            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc2.GetBR());

            m_vTL.x += x;
            m_vTL.y += y;
            m_vBR.x += x;
            m_vBR.y += y;

            m_path = path;
        }

        public static void AddEArcPath(GraphicsPath path, EArc2D earc, double x, double y)
        {
            Vertex2D vTL = earc.GetTL();
            Vertex2D vBL = earc.GetBL();
            Vertex2D vBR = earc.GetBR();

            RectangleF rect = new RectangleF((float)(vBL.x + x), (float)(vBL.y + y), (float)vBL.GetDistance(vBR), (float)vBL.GetDistance(vTL));

            // Degree
            float fBeginAngle = (float)UnE.Geometry.Math.RadToDeg(earc.GetBeginAngle());
            float fEArcAngle = (float)UnE.Geometry.Math.RadToDeg(earc.GetAngle());

            if (earc.IsClockWise())
                fEArcAngle = -fEArcAngle;

            path.AddArc(rect, fBeginAngle, fEArcAngle);
        }

        private void MakeLineWindow(double x, double y)
        {
            Vertex2D vBegin = m_wall.GetBeginVertex();
            Vertex2D vEnd = m_wall.GetEndVertex();

            double len1 = m_vPos.GetDistance(vBegin);
            double len2 = m_vPos.GetDistance(vEnd);
            Vertex2D vB = null;

            if (len1 > len2)
                vB = UnE.Geometry.Math.GetLinearVertex(m_vPos, vBegin, m_fWidth / 2);
            else
                vB = UnE.Geometry.Math.GetLinearVertex(m_vPos, vEnd, m_fWidth / 2);

            Vertex2D vE = m_vPos * 2 - vB;

            Vertex2D v1 = UnE.Geometry.Math.GetRightVertex(vB, vE, m_fThick / 2);
            Vertex2D v2 = vB * 2 - v1;
            Vertex2D v3 = m_vPos * 2 - v1;
            Vertex2D v4 = m_vPos * 2 - v2;

            GraphicsPath path = new GraphicsPath();

            PointF pt1 = new PointF((float)(v1.x + x), (float)(v1.y + y));
            PointF pt2 = new PointF((float)(v2.x + x), (float)(v2.y + y));
            PointF pt3 = new PointF((float)(v3.x + x), (float)(v3.y + y));
            PointF pt4 = new PointF((float)(v4.x + x), (float)(v4.y + y));

            path.AddLine(pt1, pt2);
            path.AddLine(pt2, pt3);
            path.AddLine(pt3, pt4);
            path.AddLine(pt4, pt1);

            m_vTL = new Vertex2D(v1);
            m_vBR = new Vertex2D(v1);

            Wall.SetBoundaryVertex(m_vTL, m_vBR, v1);
            Wall.SetBoundaryVertex(m_vTL, m_vBR, v2);
            Wall.SetBoundaryVertex(m_vTL, m_vBR, v3);
            Wall.SetBoundaryVertex(m_vTL, m_vBR, v4);

            m_vTL.x += x;
            m_vTL.y += y;
            m_vBR.x += x;
            m_vBR.y += y;

            m_path = path;
        }

        public override void Render(Graphics g, Pen pen, Brush brush, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            if (m_vTL == null)
                return;

            if (m_vBR.x <= vClientAreaTL.x || m_vTL.x >= vClientAreaBR.x)
                return;

            if (m_vTL.y <= vClientAreaBR.y || m_vBR.y >= vClientAreaTL.y)
                return;

            if (m_path != null)
            {
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
                }
            }
        }

        public override void Move(double dMoveX, double dMoveY)
        {
            MakeShape(dMoveX, dMoveY);
        }

        public override bool HitTest(Vertex2D vertex)
        {
            if (m_boundaryPolygon.HitTest(vertex) != 0)
                return true;

            return false;
        }
    }
}
