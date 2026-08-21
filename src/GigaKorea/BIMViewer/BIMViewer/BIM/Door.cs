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

    public class Door : Shape
    {
        //  미닫이문, 외여닫이문, 양쪽 외여닫이문, 쌍여닫이문, 양쪽 쌍여닫이문
        public enum DoorType { Sliding = 0, Hinged,  Hinged2, DualHinged, DualHinged2 };

        private int m_nID = 0;
        private string m_strXMLID = "";
        // 회전각도(Degree)
        //private double m_dDirection = 0;
        private Vertex2D m_vHinge1 = null;
        private Vertex2D m_vHinge2 = null;
        private Vertex2D m_vPos = null;
        private float m_fWidth = 0.0f;
        private float m_fHeight = 0.0f;
        private float m_fElevation = 0.0f;
        private float m_fThick = 50.0f;
        private DoorType m_doorType = DoorType.Sliding;
        private Wall m_wall = null;

        private List<Property> m_properties = new List<Property>();

        // 문의 두께 부분
        private GraphicsPath m_path1 = null;
        // 문의 힌지 부분
        private GraphicsPath m_path2 = null;
        // 쌍여닫이문의 힌지 부분
        private GraphicsPath m_path3 = null;

        // 문에 의하여 벽체가 뚫리게 되는 영역
        // Line Type일 경우
        private Vertex2D m_vEmptyLineBegin = null;
        private Vertex2D m_vEmptyLineEnd = null;
        // Arc 또는 EArc Type일 경우
        private double m_dEmptyBeginAngle = 0.0;
        private double m_dEmptyEndAngle = 0.0;

        private List<Polygon> m_boundaryPolygons = new List<Polygon>();

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

        /*public double Direction
        {
            get { return m_dDirection; }
            set { m_dDirection = value; }
        }*/

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

        public float Thick
        {
            get { return m_fThick; }
            set { m_fThick = value; }
        }

        public Wall Wall
        {
            get { return m_wall; }
            set { m_wall = value; }
        }

        public Vertex2D Hinge1
        {
            get { return m_vHinge1; }
            set { m_vHinge1 = value; }
        }

        public Vertex2D Hinge2
        {
            get { return m_vHinge2; }
            set { m_vHinge2 = value; }
        }

        public DoorType GetDoorType()
        {
            return m_doorType;
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public void SetDoorType(int nDoorType)
        {
            foreach (DoorType type in Enum.GetValues(typeof(DoorType)))
            {
                if (nDoorType == (int)type)
                {
                    m_doorType = type;
                    break;
                }
            }
        }

        public void MakeShape(double x = 0.0, double y = 0.0)
        {
            if (m_wall == null || m_vPos == null || m_fWidth <= 0.0f)
                return;

            if (m_wall.GetGridType() == Wall.GridType.Line)
            {
                MakeLineDoor(x, y);
            }
            else if (m_wall.GetGridType() == Wall.GridType.Arc || m_wall.GetGridType() == Wall.GridType.EArc)
            {
                MakeEArcDoor(x, y);
            }

            // .TODO: 힌지1,2 값이 0 이라면 NULL 처리
            if (m_vHinge1 != null && m_vHinge2 != null)
            {
                if (m_vHinge1.x == 0 && m_vHinge1.y == 0 && m_vHinge2.x == 0 && m_vHinge2.y == 0)
                {
                    m_vHinge1 = null;
                    m_vHinge2 = null;
                }
            }

            SetDoorHinge(m_vEmptyLineBegin, m_vEmptyLineEnd, x, y);

            if (m_vTL != null)
            {
                m_vTL.x += x;
                m_vTL.y += y;
                m_vBR.x += x;
                m_vBR.y += y;
            }

            m_boundaryPolygons.Clear();

            SetBoundaryPolygon(m_path1);
            SetBoundaryPolygon(m_path2);
            SetBoundaryPolygon(m_path3);
        }

        private void SetBoundaryPolygon(GraphicsPath path)
        {
            if (path == null)
                return;

            Polygon polygon = new Polygon();

            foreach (PointF point in path.PathPoints)
            {
                polygon.AddVertex(new Vertex2D(point.X, point.Y));
            }

            m_boundaryPolygons.Add(polygon);
        }

        private void MakeEArcDoor(double x, double y)
        {
            EArc2D earcOrigin = m_wall.GetGridType() == Wall.GridType.Arc ? m_wall.Arc : m_wall.EArc;

            if (earcOrigin == null)
                return;

            EArc2D earc = Window.MakeSubEArc(earcOrigin, m_vPos, m_fWidth);

            if (earc == null)
                return;

            m_vEmptyLineBegin = earc.GetBeginVertex();
            m_vEmptyLineEnd = earc.GetEndVertex();
            m_dEmptyBeginAngle = earc.GetBeginAngle();
            m_dEmptyEndAngle = PathItem.GetEArcAngle(earc, m_vEmptyLineEnd);

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

            Window.AddEArcPath(path, earc1, x, y);
            path.AddLine(pt2, pt3);
            Window.AddEArcPath(path, earc2, x, y);
            path.AddLine(pt4, pt1);

            m_vTL = new Vertex2D(earc1.GetTL());
            m_vBR = new Vertex2D(earc1.GetBR());

            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc1.GetTL());
            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc1.GetBL());
            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc1.GetBR());
            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc2.GetTL());
            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc2.GetBL());
            Wall.SetBoundaryVertex(m_vTL, m_vBR, earc2.GetBR());

            /*m_vTL.x += x;
            m_vTL.y += y;
            m_vBR.x += x;
            m_vBR.y += y;*/

            m_path1 = path;
        }

        private void MakeLineDoor(double x, double y)
        {
            m_vEmptyLineBegin = UnE.Geometry.Math.GetLinearVertex(m_vPos, m_wall.GetBeginVertex(), m_fWidth / 2);
            m_vEmptyLineEnd = m_vPos * 2 - m_vEmptyLineBegin;
            
            Vertex2D v1 = UnE.Geometry.Math.GetRightVertex(m_vEmptyLineBegin, m_vEmptyLineEnd, m_fThick / 2);
            Vertex2D v2 = m_vEmptyLineBegin * 2 - v1;
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

            /*m_vTL.x += x;
            m_vTL.y += y;
            m_vBR.x += x;
            m_vBR.y += y;*/

            m_path1 = path;
        }

        private void SetDoorHinge(Vertex2D vBegin, Vertex2D vEnd, double x, double y)
        {
            // 한방향 외여닫이문
            if (m_doorType == DoorType.Hinged)
            {
                m_path2 = SetOneWayDoorHinge(m_vHinge1, vBegin, vEnd, x, y);
                /*Vertex2D vHinge = GetHingeVertex(vBegin, vEnd);
                PointF ptHinge = new PointF((float)(vHinge.x + x), (float)(vHinge.y + y));

                Vertex2D vB = UnE.Geometry.Math.GetLinearVertex(vBegin, vHinge, m_fThick / 2);
                Vertex2D vE = vB - vBegin + vEnd;

                PointF ptBegin = new PointF((float)(vB.x + x), (float)(vB.y + y));
                PointF ptEnd = new PointF((float)(vE.x + x), (float)(vE.y + y));

                GraphicsPath path = new GraphicsPath();

                path.AddLine(ptEnd, ptBegin);
                path.AddLine(ptBegin, ptHinge);

                float startAngle, sweepAngle;
                RectangleF rect = GetHingeDatas(vHinge, vE, vB, true, x, y, out startAngle, out sweepAngle);
                path.AddArc(rect, startAngle, sweepAngle);

                m_path2 = path;

                if (m_vTL == null)
                {
                    m_vTL = new Vertex2D(vHinge);
                    m_vBR = new Vertex2D(vHinge);
                }

                Vertex2D vTemp = vHinge - vB + vE;
                Wall.SetBoundaryVertex(m_vTL, m_vBR, vHinge);
                Wall.SetBoundaryVertex(m_vTL, m_vBR, vTemp);*/
            }
            // 양방향 외여닫이문
            else if (m_doorType == DoorType.Hinged2)
            {
                m_path2 = SetTwoWayDoorHinge(m_vHinge1, vBegin, vEnd, x, y);
                /*Vertex2D vHinge1 = UnE.Geometry.Math.GetRightVertex(vBegin, vEnd, m_fWidth + m_fThick / 2);
                Vertex2D vHinge2 = vBegin * 2 - vHinge1;
                Vertex2D vB1 = UnE.Geometry.Math.GetLinearVertex(vBegin, vHinge1, m_fThick / 2);
                Vertex2D vB2 = vBegin * 2 - vB1;
                Vertex2D vE2 = vB2 - vBegin + vEnd;
                Vertex2D vE1 = vB1 - vBegin + vEnd;

                PointF ptHinge1 = new PointF((float)(vHinge1.x + x), (float)(vHinge1.y + y));
                PointF ptHinge2 = new PointF((float)(vHinge2.x + x), (float)(vHinge2.y + y));
                PointF ptEnd1 = new PointF((float)(vE1.x + x), (float)(vE1.y + y));
                PointF ptEnd2 = new PointF((float)(vE2.x + x), (float)(vE2.y + y));

                float startAngle, sweepAngle;
                RectangleF rect = GetHingeDatas(vHinge1, vE1, vB1, false, x, y, out startAngle, out sweepAngle);

                GraphicsPath path = new GraphicsPath();

                path.AddArc(rect, startAngle, sweepAngle);
                path.AddLine(ptHinge1, ptHinge2);

                rect = GetHingeDatas(vHinge2, vE2, vB2, true, x, y, out startAngle, out sweepAngle);
                path.AddArc(rect, startAngle, sweepAngle);
                path.AddLine(ptEnd2, ptEnd1);

                m_path2 = path;

                if (m_vTL == null)
                {
                    m_vTL = new Vertex2D(vHinge1);
                    m_vBR = new Vertex2D(vHinge1);
                }

                Vertex2D vTemp1 = vHinge1 - vB1 + vE1;
                Vertex2D vTemp2 = vHinge1 - vB2 + vE2;
                Wall.SetBoundaryVertex(m_vTL, m_vBR, vHinge1);
                Wall.SetBoundaryVertex(m_vTL, m_vBR, vHinge2);
                Wall.SetBoundaryVertex(m_vTL, m_vBR, vTemp1);
                Wall.SetBoundaryVertex(m_vTL, m_vBR, vTemp2);*/
            }
            // 한방향 쌍여닫이문
            else if (m_doorType == DoorType.DualHinged)
            {
                if (m_vHinge1 == null || m_vHinge2 == null)
                    return;

                double len = vBegin.GetDistance(m_vHinge1) - m_fThick / 2;
                Vertex2D vMiddle = UnE.Geometry.Math.GetLinearVertex(vBegin, vEnd, len);

                m_path2 = SetOneWayDoorHinge(m_vHinge1, vBegin, vMiddle, x, y);
                m_path3 = SetOneWayDoorHinge(m_vHinge2, vEnd, vMiddle, x, y);
            }
            // 양방향 쌍여닫이문
            else if (m_doorType == DoorType.DualHinged2)
            {
                if (m_vHinge1 == null || m_vHinge2 == null)
                    return;

                double len = vBegin.GetDistance(m_vHinge1) - m_fThick / 2;
                Vertex2D vMiddle = UnE.Geometry.Math.GetLinearVertex(vBegin, vEnd, len);

                m_path2 = SetTwoWayDoorHinge(m_vHinge1, vBegin, vMiddle, x, y);
                m_path3 = SetTwoWayDoorHinge(m_vHinge2, vEnd, vMiddle, x, y);
            }
            else
                m_path2 = null;
        }

        // vHinge가 vBegin에 더 가까운지를 검사한다.
        private bool IsBeginSide(Vertex2D vHinge, Vertex2D vBegin, Vertex2D vEnd)
        {
            double len1 = vHinge.GetDistance(vBegin);
            double len2 = vHinge.GetDistance(vEnd);
            return len1 < len2;
        }

        private GraphicsPath SetOneWayDoorHinge(Vertex2D vHinge, Vertex2D vBegin, Vertex2D vEnd, double x, double y)
        {
            if (vHinge == null)
                return null;

            if (IsBeginSide(vHinge, vBegin, vEnd) == false)
            {
                Vertex2D vTemporary = vBegin;
                vBegin = vEnd;
                vEnd = vTemporary;
            }

            // 문의 두께만큼 힌지 위치를 이동시킨다.
            vHinge = UnE.Geometry.Math.GetLinearVertex(vBegin, vHinge, vBegin.GetDistance(vHinge) + m_fThick / 2);

            PointF ptHinge = new PointF((float)(vHinge.x + x), (float)(vHinge.y + y));

            Vertex2D vB = UnE.Geometry.Math.GetLinearVertex(vBegin, vHinge, m_fThick / 2);
            Vertex2D vE = vB - vBegin + vEnd;

            PointF ptBegin = new PointF((float)(vB.x + x), (float)(vB.y + y));
            PointF ptEnd = new PointF((float)(vE.x + x), (float)(vE.y + y));

            GraphicsPath path = new GraphicsPath();

            path.AddLine(ptEnd, ptBegin);
            path.AddLine(ptBegin, ptHinge);

            float startAngle, sweepAngle;
            RectangleF rect = GetHingeDatas(vHinge, vE, vB, true, x, y, out startAngle, out sweepAngle);
            path.AddArc(rect, startAngle, sweepAngle);

            if (m_vTL == null)
            {
                m_vTL = new Vertex2D(vHinge);
                m_vBR = new Vertex2D(vHinge);
            }

            Vertex2D vTemp = vHinge - vB + vE;
            Wall.SetBoundaryVertex(m_vTL, m_vBR, vHinge);
            Wall.SetBoundaryVertex(m_vTL, m_vBR, vTemp);

            return path;
        }

        private GraphicsPath SetTwoWayDoorHinge(Vertex2D vHinge, Vertex2D vBegin, Vertex2D vEnd, double x, double y)
        {
            if (vHinge == null)
                return null;

            if (IsBeginSide(vHinge, vBegin, vEnd) == false)
            {
                Vertex2D vTemporary = vBegin;
                vBegin = vEnd;
                vEnd = vTemporary;
            }

            // 문의 두께만큼 힌지 위치를 이동시킨다.
            vHinge = UnE.Geometry.Math.GetLinearVertex(vBegin, vHinge, vBegin.GetDistance(vHinge) + m_fThick / 2);

            Vertex2D vHinge1 = vHinge;
            Vertex2D vHinge2 = vBegin * 2 - vHinge1;
            Vertex2D vB1 = UnE.Geometry.Math.GetLinearVertex(vBegin, vHinge1, m_fThick / 2);
            Vertex2D vB2 = vBegin * 2 - vB1;
            Vertex2D vE2 = vB2 - vBegin + vEnd;
            Vertex2D vE1 = vB1 - vBegin + vEnd;

            PointF ptHinge1 = new PointF((float)(vHinge1.x + x), (float)(vHinge1.y + y));
            PointF ptHinge2 = new PointF((float)(vHinge2.x + x), (float)(vHinge2.y + y));
            PointF ptEnd1 = new PointF((float)(vE1.x + x), (float)(vE1.y + y));
            PointF ptEnd2 = new PointF((float)(vE2.x + x), (float)(vE2.y + y));

            float startAngle, sweepAngle;
            RectangleF rect = GetHingeDatas(vHinge1, vE1, vB1, false, x, y, out startAngle, out sweepAngle);

            GraphicsPath path = new GraphicsPath();

            path.AddArc(rect, startAngle, sweepAngle);
            path.AddLine(ptHinge1, ptHinge2);

            rect = GetHingeDatas(vHinge2, vE2, vB2, true, x, y, out startAngle, out sweepAngle);
            path.AddArc(rect, startAngle, sweepAngle);
            path.AddLine(ptEnd2, ptEnd1);

            if (m_vTL == null)
            {
                m_vTL = new Vertex2D(vHinge1);
                m_vBR = new Vertex2D(vHinge1);
            }

            Vertex2D vTemp1 = vHinge1 - vB1 + vE1;
            Vertex2D vTemp2 = vHinge1 - vB2 + vE2;
            Wall.SetBoundaryVertex(m_vTL, m_vBR, vHinge1);
            Wall.SetBoundaryVertex(m_vTL, m_vBR, vHinge2);
            Wall.SetBoundaryVertex(m_vTL, m_vBR, vTemp1);
            Wall.SetBoundaryVertex(m_vTL, m_vBR, vTemp2);

            return path;
        }

        private RectangleF GetHingeDatas(Vertex2D vTop, Vertex2D vArcEnd, Vertex2D vCenter, bool topToEnd, double x, double y, out float startAngle, out float sweepAngle)
        {
            double dRadius = vTop.GetDistance(vCenter);

            Vertex2D vMiddle = (vTop + vArcEnd) / 2;
            vMiddle = UnE.Geometry.Math.GetLinearVertex(vCenter, vMiddle, dRadius);

            Arc2D arc = null;

            if (topToEnd)
                arc = new Arc2D(vTop, vMiddle, vArcEnd);
            else
                arc = new Arc2D(vArcEnd, vMiddle, vTop);

            bool isClockWise = arc.IsClockWise();
            /*Vertex2D vR = new Vertex2D(vCenter.x + 100.0, vCenter.y);

            double dBeginAngle = GetArcAngle(vTop, vCenter, vR);
            double dEndAngle = GetArcAngle(vArcEnd, vCenter, vR);
            bool isClockWise = true;

            double dTarget1 = InflateAngle(dBeginAngle + UnE.Geometry.Math.HALF_PI());
            double dTarget2 = InflateAngle(dBeginAngle - UnE.Geometry.Math.HALF_PI());

            if (System.Math.Abs(dTarget1 - dEndAngle) < System.Math.Abs(dTarget2 - dEndAngle))
            {
                isClockWise = false;
            }

            double dRadius = vTop.GetDistance(vCenter);
            Arc2D arc = new Arc2D(vCenter, dRadius, dBeginAngle, UnE.Geometry.Math.HALF_PI(), isClockWise);*/

            RectangleF rect = new RectangleF((float)(arc.GetBL().x + x), (float)(arc.GetBL().y + y), (float)arc.GetBL().GetDistance(arc.GetBR()), (float)arc.GetBL().GetDistance(arc.GetTL()));
            sweepAngle = 90.0f;

            if (topToEnd)
            {
                // Degree
                startAngle = (float)UnE.Geometry.Math.RadToDeg(arc.GetBeginAngle());

                if (isClockWise)
                    sweepAngle = -sweepAngle;
            }
            else
            {
                // Degree
                startAngle = (float)UnE.Geometry.Math.RadToDeg(arc.GetEndAngle());

                if (!isClockWise)
                    sweepAngle = -sweepAngle;
            }

            return rect;
        }

        /*private Vertex2D GetHingeVertex(Vertex2D vBegin, Vertex2D vEnd)
        {
            Vertex2D vRight = new Vertex2D(vBegin.x + 100.0, vBegin.y);
            Vertex2D v1 = UnE.Geometry.Math.GetRightVertex(vBegin, vEnd, m_fWidth + m_fThick / 2);

            double dAngle = GetArcAngle(v1, vBegin, vRight);

            double direction = UnE.Geometry.Math.DegToRad(m_dDirection);
            direction = InflateAngle(direction);

            double diff = System.Math.Abs(dAngle - direction);

            if (diff < 0.1 || diff > UnE.Geometry.Math._2PI() - 0.1)
                return v1;

            return vBegin * 2 - v1;
        }*/

        private double InflateAngle(double dAngle)
        {
            while (dAngle >= UnE.Geometry.Math._2PI())
            {
                dAngle -= UnE.Geometry.Math._2PI();
            }

            while (dAngle < 0.0)
            {
                dAngle += UnE.Geometry.Math._2PI();
            }

            return dAngle;
        }

        // Radian
        private double GetArcAngle(Vertex2D vertex, Vertex2D vCenter, Vertex2D vRight)
        {
            double dAngle = UnE.Geometry.Math.GetAngle(vertex, vCenter, vRight);

            if (vertex.y < vRight.y)
                dAngle = UnE.Geometry.Math._2PI() - dAngle;

            return dAngle;
        }

        // Line Type일 경우
        public void GetEmptyLine(out Vertex2D vBegin, out Vertex2D vEnd)
        {
            vBegin = m_vEmptyLineBegin;
            vEnd = m_vEmptyLineEnd;
        }

        // Arc 또는 EArc Type일 경우
        public void GetEmptyAngle(out double dBeginAngle, out double dEndAngle)
        {
            dBeginAngle = m_dEmptyEndAngle;
            dEndAngle = m_dEmptyEndAngle;
        }

        public override void Render(Graphics g, Pen pen, Brush brush, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            if (m_vTL == null)
                return;

            if (m_vBR.x <= vClientAreaTL.x || m_vTL.x >= vClientAreaBR.x)
                return;

            if (m_vTL.y <= vClientAreaBR.y || m_vBR.y >= vClientAreaTL.y)
                return;

            Brush selectedBrush = null;

            if (m_path1 != null)
            {
                if (m_selected)
                {
                    selectedBrush = new SolidBrush(m_selectedFillColor);
                    g.FillPath(selectedBrush, m_path1);
                }
                else
                {
                    if (pen != null)
                        g.DrawPath(pen, m_path1);

                    if (brush != null)
                        g.FillPath(brush, m_path1);
                }
            }

            if (m_path2 != null)
            {
                if (m_selected)
                {
                    if (selectedBrush == null)
                        selectedBrush = new SolidBrush(m_selectedFillColor);

                    g.FillPath(selectedBrush, m_path2);
                }
                else
                {
                    if (pen != null)
                        g.DrawPath(pen, m_path2);

                    if (brush != null)
                        g.FillPath(brush, m_path2);
                }
            }

            if (m_path3 != null)
            {
                if (m_selected)
                {
                    if (selectedBrush == null)
                        selectedBrush = new SolidBrush(m_selectedFillColor);

                    g.FillPath(selectedBrush, m_path3);
                }
                else
                {
                    if (pen != null)
                        g.DrawPath(pen, m_path3);

                    if (brush != null)
                        g.FillPath(brush, m_path3);
                }
            }

            if (selectedBrush != null)
                selectedBrush.Dispose();
        }

        public override void Move(double dMoveX, double dMoveY)
        {
            MakeShape(dMoveX, dMoveY);
        }

        public string GetDoorTypeName()
        {
            if (m_doorType == DoorType.Sliding)
                return "미닫이문";
            else if (m_doorType == DoorType.Hinged)
                return "외여닫이문";
            else if (m_doorType == DoorType.Hinged2)
                return "양쪽 외여닫이문";
            else if (m_doorType == DoorType.DualHinged)
                return "쌍여닫이문";
            else if (m_doorType == DoorType.DualHinged2)
                return "양쪽 쌍여닫이문";

            return "";
        }

        public override bool HitTest(Vertex2D vertex)
        {
            foreach (Polygon polygon in m_boundaryPolygons)
            {
                if (polygon.HitTest(vertex) != 0)
                    return true;
            }

            return false;
        }
    }
}
