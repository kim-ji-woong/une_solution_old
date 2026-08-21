using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;
using BIMViewer.Shapes;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BIMViewer.BIM
{
    public class Wall : Shape
    {
        private class WallVertex : UnE.Geometry.Vertex2D
        {
            private object m_tag = null;

            public object Tag
            {
                get { return m_tag; }
                set { m_tag = value; }
            }

            public WallVertex()
            {
            }

            public WallVertex(UnE.Geometry.Vertex2D vertex)
                : base(vertex)
            {
            }

            public WallVertex(double x, double y)
                : base(x, y)
            {
            }
        }

        public enum GridType { Line = 0, Arc, EArc };

        //private List<Vertex2D> m_centerLineVertices = new List<Vertex2D>();
        private Polygon m_boundaryPolygon = null;

        private int m_nID = 0;
        private string m_strXMLID = "";
        private string m_strGridID = "";
        private double m_dThick = 0.0;
        private double m_dHeight = 0.0;
        private Component m_component = null;

        private Line2D m_line = null;
        private Arc2D m_arc = null;
        private EArc2D m_earc = null;
        private GridType m_gridType = GridType.Line;

        private List<Door> m_doors = new List<Door>();
        private List<Window> m_windows = new List<Window>();
        private List<Space> m_linkedSpaces = new List<Space>();

        private List<Property> m_properties = new List<Property>();

        // 벽체가 한쪽면만 공간과 연결되어 있을 경우 나머지 면의 외곽영역 계산을 위한 임시 데이터
        private List<PathItem> m_outsideBoundaryPath = null;
        private List<PathItem> m_boundary = null;
        private GraphicsPath m_path = null;

        private double m_dMoveX = 0.0, m_dMoveY = 0.0;
        private Vertex2D m_vOriginTL = null;
        private Vertex2D m_vOriginBR = null;

        private Boundary m_boundaryData = null;
        
        /*public List<Vertex2D> CenterLineVertices
        {
            get { return m_centerLineVertices; }
        }*/

        public List<PathItem> Boundary
        {
            get { return m_boundary; }
        }

        public Polygon BoundaryPolygon
        {
            get { return m_boundaryPolygon; }
            set { m_boundaryPolygon = value; }
        }

        public Component Component
        {
            get { return m_component; }
            set { m_component = value; }
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

        public string GridID
        {
            get { return m_strGridID; }
            set { m_strGridID = value; }
        }

        public double Thick
        {
            get { return m_dThick; }
            set { m_dThick = value; }
        }

        public double Height
        {
            get { return m_dHeight; }
            set { m_dHeight = value; }
        }

        public Line2D Line
        {
            get { return m_line; }
            set { m_line = value; }
        }

        public Arc2D Arc
        {
            get { return m_arc; }
            set { m_arc = value; }
        }

        public EArc2D EArc
        {
            get { return m_earc; }
            set { m_earc = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public List<Door> Doors
        {
            get { return m_doors; }
        }

        public List<Window> Windows
        {
            get { return m_windows; }
        }

        public GridType GetGridType()
        {
            return m_gridType;
        }

        public Boundary BoundaryData
        {
            get { return m_boundaryData; }
            set { m_boundaryData = value; }
        }

        public void SetGridType(int nGridType)
        {
            foreach (GridType type in Enum.GetValues(typeof(GridType)))
            {
                if (nGridType == (int)type)
                {
                    m_gridType = type;
                    break;
                }
            }
        }

        public Vertex2D GetBeginVertex()
        {
            Vertex2D vBegin = null;

            if (m_gridType == GridType.Line)
            {
                if (m_line != null)
                    vBegin = m_line.GetVertex(true);
            }
            else if (m_gridType == GridType.Arc)
            {
                if (m_arc != null)
                    vBegin = m_arc.GetBeginVertex();
            }
            else if (m_gridType == GridType.EArc)
            {
                if (m_earc != null)
                    vBegin = m_earc.GetBeginVertex();
            }

            return vBegin;
        }

        public Vertex2D GetEndVertex()
        {
            Vertex2D vEnd = null;

            if (m_gridType == GridType.Line)
            {
                if (m_line != null)
                    vEnd = m_line.GetVertex(false);
            }
            else if (m_gridType == GridType.Arc)
            {
                if (m_arc != null)
                    vEnd = m_arc.GetEndVertex();
            }
            else if (m_gridType == GridType.EArc)
            {
                if (m_earc != null)
                    vEnd = m_earc.GetEndVertex();
            }

            return vEnd;
        }

        public Vertex2D GetMiddleVertex()
        {
            Vertex2D vMiddle = null;

            if (m_gridType == GridType.Line)
            {
                if (m_line != null)
                {
                    vMiddle = (m_line.GetVertex(true) + m_line.GetVertex(false)) / 2;
                }
            }
            else if (m_gridType == GridType.Arc || m_gridType == GridType.EArc)
            {
                EArc2D earc = m_earc;

                if (m_gridType == GridType.Arc)
                    earc = m_arc;

                if (earc != null)
                {
                    Vertex2D vBegin = earc.GetBeginVertex();
                    Vertex2D vEnd = earc.GetEndVertex();

                    if (earc.GetVertex(earc.GetBeginAngle() + earc.GetAngle() / 2, out vMiddle) == false)
                        return null;
                }
            }

            return vMiddle;
        }

        public void AddSpace(Space space)
        {
            if (m_linkedSpaces.Contains(space) == false)
                m_linkedSpaces.Add(space);
        }

        public int GetLinkedSpaceCount()
        {
            return m_linkedSpaces.Count;
        }

        public Space GetFirstLinkedSpace()
        {
            if (m_linkedSpaces.Count >= 1)
                return m_linkedSpaces[0];

            return null;
        }

        public Space GetSecondLinkedSpace()
        {
            if (m_linkedSpaces.Count >= 2)
                return m_linkedSpaces[1];

            return null;
        }

        // 공간에 속하지 않는 벽체
        // LineType에 대해서만 구현
        // Arc와 EArc Type은 추후 구현할 것
        private void MakeNoSpaceWallShape(Layer layer, Level level)
        {
            List<Wall> beginLinkedWalls = level.GetLinkedWall(this, true);
            List<Wall> endLinkedWalls = level.GetLinkedWall(this, false);

            if (beginLinkedWalls == null || endLinkedWalls == null)
                return;

            Vertex2D vWallBegin = GetBeginVertex();
            Vertex2D vWallEnd = GetEndVertex();

            //  EndLeft           BeginLeft
            //  E --------------- B
            //  EndRight          BeginRight
            Vertex2D vBeginRight = null, vBeginLeft = null;
            Vertex2D vEndRight = null, vEndLeft = null;

            Wall wallRight = null, wallLeft = null;
            GetLinkedWalls(beginLinkedWalls, vWallEnd, vWallBegin, out wallRight, out wallLeft);

            if (wallRight == null || wallLeft == null)
            {
                vBeginRight = UnE.Geometry.Math.GetRightVertex(vWallBegin, vWallEnd, m_dThick / 2);
                vBeginLeft = vWallBegin * 2 - vBeginRight;
            }
            else
            {
                vBeginRight = GetJointVertex(vWallEnd, vWallBegin, wallRight, true);
                vBeginLeft = GetJointVertex(vWallEnd, vWallBegin, wallLeft, false);
            }

            GetLinkedWalls(endLinkedWalls, vWallBegin, vWallEnd, out wallRight, out wallLeft);

            if (wallRight == null || wallLeft == null)
            {
                vEndLeft = UnE.Geometry.Math.GetRightVertex(vWallEnd, vWallBegin, m_dThick / 2);
                vEndRight = vWallEnd * 2 - vEndLeft;
            }
            else
            {
                vEndLeft = GetJointVertex(vWallBegin, vWallEnd, wallRight, true);
                vEndRight = GetJointVertex(vWallBegin, vWallEnd, wallLeft, false);
            }

            Polygon polygon = new Polygon();

            polygon.AddVertex(vBeginLeft);
            polygon.AddVertex(vWallBegin);
            polygon.AddVertex(vBeginRight);
            polygon.AddVertex(vEndRight);
            polygon.AddVertex(vWallEnd);
            polygon.AddVertex(vEndLeft);

            List<Polygon> polygons = new List<Polygon>();
            polygons.Add(polygon);

            SelectBoundaryPolygon(polygons, layer);
        }

        private Vertex2D GetJointVertex(Vertex2D vBegin, Vertex2D vEnd, Wall wall, bool isRightSide)
        {
            Vertex2D vBB = null, vEE = null;
            Vertex2D vWBB = null, vWEE = null;

            Vertex2D vWallBegin = wall.GetBeginVertex();
            Vertex2D vWallEnd = wall.GetEndVertex();

            if (vWallBegin.GetDistance(vEnd) > vWallEnd.GetDistance(vEnd))
            {
                Vertex2D vTemp = vWallBegin;
                vWallBegin = vWallEnd;
                vWallEnd = vTemp;
            }

            if (isRightSide)
            {
                vEE = UnE.Geometry.Math.GetRightVertex(vEnd, vBegin, m_dThick / 2);
                vBB = vEE - vEnd + vBegin;

                vWBB = UnE.Geometry.Math.GetRightVertex(vWallBegin, vWallEnd, -wall.m_dThick / 2);
                vWEE = vWBB - vWallBegin + vWallEnd;
            }
            else
            {
                vEE = UnE.Geometry.Math.GetRightVertex(vEnd, vBegin, -m_dThick / 2);
                vBB = vEE - vEnd + vBegin;

                vWBB = UnE.Geometry.Math.GetRightVertex(vWallBegin, vWallEnd, wall.m_dThick / 2);
                vWEE = vWBB - vWallBegin + vWallEnd;
            }

            Line2D line1 = new Line2D(vBB, vEE, Line2D.LineType.HALF_LINE_BEGIN_2_END);
            Line2D line2 = new Line2D(vWBB, vWEE, Line2D.LineType.HALF_LINE_END_2_BEGIN);

            Vertex2D v1, v2;
            Line2D.LineType lineType;
            int nResult = line1.IntersectLine(line2, out v1, out v2, out lineType);

            if (nResult == 0)
                return vEE;
            else if (nResult == 2)
                return vEE;

            return v1;
        }

        private void GetLinkedWalls(List<Wall> walls, Vertex2D vBegin, Vertex2D vEnd, out Wall wallRight, out Wall wallLeft)
        {
            Wall maxWall = null, minWall = null;
            double dMaxAngle = 0.0, dMinAngle = 0.0;

            foreach (Wall wall in walls)
            {
                if (wall == this)
                    continue;

                Vertex2D vWallBegin = wall.GetBeginVertex();
                Vertex2D vWallEnd = wall.GetEndVertex();
                Vertex2D vWall = vWallBegin.GetDistance(vEnd) < vWallEnd.GetDistance(vEnd) ? vWallEnd : vWallBegin;

                double dAngle = UnE.Geometry.Math.GetAngle(vBegin, vEnd, vWall);

                if (UnE.Geometry.Math.IsRightSideFromLine(vWall, vEnd, vBegin) == 0)
                    dAngle = UnE.Geometry.Math._2PI() - dAngle;

                if (maxWall == null)
                {
                    maxWall = minWall = wall;
                    dMaxAngle = dMinAngle = dAngle;
                }
                else
                {
                    if (dMaxAngle < dAngle)
                    {
                        dMaxAngle = dAngle;
                        maxWall = wall;
                    }

                    if (dMinAngle > dAngle)
                    {
                        dMinAngle = dAngle;
                        minWall = wall;
                    }
                }
            }

            wallLeft = maxWall;
            wallRight = minWall;
        }

        public bool MakeShape(Layer layer, Level level)
        {
            #region 기존 벽체영역 계산
            //List<PathItem> items1 = null, items2 = null;

            //if (m_linkedSpaces.Count == 0)
            //{
            //    // 공간에 속하지 않는 벽체
            //    MakeNoSpaceWallShape(layer, level);
            //    SetBoundaryPolygon();
            //    return true;
            //}
            //else if (m_linkedSpaces.Count == 1)
            //{
            //    items1 = m_linkedSpaces[0].Boundary;
            //    items2 = m_outsideBoundaryPath;
            //}
            //else// if (m_linkedSpaces.Count == 2)
            //{
            //    items1 = m_linkedSpaces[0].Boundary;
            //    items2 = m_linkedSpaces[1].Boundary;
            //}

            //if (items1 == null || items2 == null)
            //    return false;


            //Vertex2D vBeginNear1, vBeginNear2, vEndNear1, vEndNear2;

            //GetNearVertex(items1, out vBeginNear1, out vEndNear1);
            //GetNearVertex(items2, out vBeginNear2, out vEndNear2);

            //List<Polygon> polygons = MakeWallPolygons(items1, items2, vBeginNear1, vBeginNear2, vEndNear1, vEndNear2);

            //if (polygons == null)
            //    return false;

            //return SelectBoundaryPolygon(polygons, layer);
            #endregion

            // 바운더리 데이터로 벽체영역 넣기
            m_boundary = this.m_boundaryData.GetBoundary();
            m_path = this.BoundaryData.GetPath();

            SetBoundary();
            SetBoundaryPolygon();

            if (layer != null)
                layer.AddShape(this);

            return true;
        }

        public void MakeOpeningShapes(Layer doorLayer, Layer windowLayer)
        {
            foreach (Door door in m_doors)
            {
                door.MakeShape();
                doorLayer.AddShape(door);
            }

            foreach (Window window in m_windows)
            {
                window.MakeShape();
                windowLayer.AddShape(window);
            }
        }

        private bool SelectBoundaryPolygon(List<Polygon> polygons, Layer layer)
        {
            Polygon selectedPolygon = null;

            if (polygons.Count == 1)
            {
                selectedPolygon = polygons[0];
            }
            else
            {
                Vertex2D vMiddle = GetMiddleVertex();

                if (vMiddle == null)
                    return false;

                foreach (Polygon polygon in polygons)
                {
                    if (polygon.HitTest(vMiddle) == 1)
                    {
                        selectedPolygon = polygon;
                        break;
                    }
                }
            }

            if (selectedPolygon == null)
                return false;

            Polygon newPolygon = ReshapePolygon(selectedPolygon);
            m_boundary = PolygonToPathItems(newPolygon);

            if (m_boundary == null)
                return false;

            //m_boundaryPolygon = newPolygon;

            SetBoundary();
            m_path = Space.MakeGraphicsPath(m_boundary);
            SetBoundaryPolygon();

            if (layer != null)
                layer.AddShape(this);

            return true;
        }

        private void SetBoundaryPolygon()
        {
            if (m_path != null)
            {
                if (m_boundaryPolygon == null)
                    m_boundaryPolygon = new Polygon();
                else
                    m_boundaryPolygon.Clear();

                foreach (PointF point in m_path.PathPoints)
                {
                    m_boundaryPolygon.AddVertex(new Vertex2D(point.X, point.Y));
                }
            }
            else
                m_boundaryPolygon = null;
        }

        // 벽체 외곽영역 폴리곤을 벽체의 양끝점을 기준으로 좀더 매끈하게 다듬는다.
        // (벽체 양끝을 뭉뚝하게 만든다.)
        private Polygon ReshapePolygon(Polygon polygon)
        {
            Vertex2D vBegin = GetBeginVertex();
            Vertex2D vEnd = GetEndVertex();

            List<Vertex2D> vertices1 = new List<Vertex2D>();
            List<Vertex2D> vertices2 = new List<Vertex2D>();
            List<Vertex2D> vertices = vertices1;

            int nVertexCount = polygon.GetVertexCount();
            int nBeginIndex = -1, nEndIndex = -1;

            for (int i=0;i<nVertexCount;i++)
            {
                Vertex2D vertex = polygon.GetVertex(i);

                if (vBegin.GetDistance(vertex) < 0.1)
                {
                    nBeginIndex = i;

                    if (nEndIndex >= 0)
                        break;
                }
                else if (vEnd.GetDistance(vertex) < 0.1)
                {
                    nEndIndex = i;

                    if (nBeginIndex >= 0)
                        break;
                }
            }

            if (nBeginIndex < 0 || nEndIndex < 0)
                return polygon;

            int nIndex = nBeginIndex;

            do
            {
                if (++nIndex >= nVertexCount)
                    nIndex = 0;

                Vertex2D vertex = polygon.GetVertex(nIndex);

                if (vEnd.GetDistance(vertex) < 0.1)
                    vertices = vertices2;
                else if (vBegin.GetDistance(vertex) >= 0.1)
                {
                    vertices.Add(vertex);
                }
            }
            while (nIndex != nBeginIndex);

            int nVertexCount1 = vertices1.Count;
            int nVertexCount2 = vertices2.Count;

            if (nVertexCount1 < 2 || nVertexCount2 < 2)
                return polygon;

            Vertex2D vBegin1 = vertices1[0];
            Vertex2D vEnd1 = vertices1[nVertexCount1 - 1];
            Vertex2D vBegin2 = vertices2[nVertexCount2 - 1];
            Vertex2D vEnd2 = vertices2[0];

            if (m_gridType == GridType.Line)
            {
                ReshapeLineVertex(vBegin, vEnd, ref vBegin1, ref vBegin2);
                ReshapeLineVertex(vEnd, vBegin, ref vEnd1, ref vEnd2);
            }
            else
            {
                return polygon;
            }

            vertices1[0] = vBegin1;
            vertices1[nVertexCount1 - 1] = vEnd1;
            vertices2[nVertexCount2 - 1] = vBegin2;
            vertices2[0] = vEnd2;

            Polygon newPolygon = new Polygon();

            foreach (Vertex2D vertex in vertices1)
            {
                newPolygon.AddVertex(vertex);
            }

            foreach (Vertex2D vertex in vertices2)
            {
                newPolygon.AddVertex(vertex);
            }

            /*Vertex2D vFirst = polygon.GetVertex(0);
            Vertex2D vLast = polygon.GetVertex(nVertexCount - 1);

            if (vFirst.GetDistance(vLast) < 0.1)
            {
                int nCount = newPolygon.GetVertexCount();
                Vertex2D vertex = newPolygon.GetVertex(nCount - 1);
                newPolygon.AddVertex(vertex);
            }*/

            return newPolygon;
        }

        private void ReshapeLineVertex(Vertex2D vLineBegin, Vertex2D vLineEnd, ref Vertex2D vBegin1, ref Vertex2D vBegin2)
        {
            Vertex2D v1 = UnE.Geometry.Math.GetNearestVertex(vBegin1, vLineBegin, vLineEnd, true);
            Vertex2D v2 = UnE.Geometry.Math.GetNearestVertex(vBegin2, vLineBegin, vLineEnd, true);

            double len1 = vLineEnd.GetDistance(vBegin1);
            double len2 = vLineEnd.GetDistance(vBegin2);
            double len3 = vLineEnd.GetDistance(vLineBegin);

            if (len1 > len2 && len1 > len3)
            {
                if (UnE.Geometry.Math.IsRightSideFromLine(vBegin1, vLineBegin, vLineEnd) == 1)
                    vBegin2 = UnE.Geometry.Math.GetRightVertex(v1, vLineEnd, -v2.GetDistance(vBegin2));
                else
                    vBegin2 = UnE.Geometry.Math.GetRightVertex(v1, vLineEnd, v2.GetDistance(vBegin2));
            }
            else if (len2 > len1 && len2 > len3)
            {
                if (UnE.Geometry.Math.IsRightSideFromLine(vBegin2, vLineBegin, vLineEnd) == 1)
                    vBegin1 = UnE.Geometry.Math.GetRightVertex(v2, vLineEnd, -v1.GetDistance(vBegin1));
                else
                    vBegin1 = UnE.Geometry.Math.GetRightVertex(v2, vLineEnd, v1.GetDistance(vBegin1));
            }
            else// if (len3 >= len1 && len3 >= len2)
            {
                if (UnE.Geometry.Math.IsRightSideFromLine(vBegin1, vLineBegin, vLineEnd) == 1)
                {
                    vBegin1 = UnE.Geometry.Math.GetRightVertex(vLineBegin, vLineEnd, v1.GetDistance(vBegin1));
                    vBegin2 = UnE.Geometry.Math.GetRightVertex(vLineBegin, vLineEnd, -v2.GetDistance(vBegin2));
                }
                else
                {
                    vBegin1 = UnE.Geometry.Math.GetRightVertex(vLineBegin, vLineEnd, -v1.GetDistance(vBegin1));
                    vBegin2 = UnE.Geometry.Math.GetRightVertex(vLineBegin, vLineEnd, v2.GetDistance(vBegin2));
                }
            }
        }

        private void SetBoundary()
        {
            Vertex2D vTL = null, vBR = null;
            Vertex2D vBegin = null, vEnd = null, vMiddle = null;

            foreach (PathItem item in m_boundary)
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
                        SetBoundaryVertex(vTL, vBR, vBegin);
                    }

                    SetBoundaryVertex(vTL, vBR, vEnd);

                    if (vMiddle != null)
                        SetBoundaryVertex(vTL, vBR, vMiddle);
                }
            }

            m_vTL = vTL;
            m_vBR = vBR;
            m_vOriginTL = new Vertex2D(m_vTL);
            m_vOriginBR = new Vertex2D(m_vBR);
        }

        public static void SetBoundaryVertex(Vertex2D vTL, Vertex2D vBR, Vertex2D vertex)
        {
            if (vTL.x > vertex.x)
                vTL.x = vertex.x;
            if (vBR.x < vertex.x)
                vBR.x = vertex.x;
            if (vTL.y < vertex.y)
                vTL.y = vertex.y;
            if (vBR.y > vertex.y)
                vBR.y = vertex.y;
        }

        private static List<PathItem> PolygonToPathItems(Polygon polygon)
        {
            int nVertexCount = polygon.GetVertexCount();
            Vertex2D vPrev = polygon.GetVertex(nVertexCount - 1);

            List<PathItem> items = new List<PathItem>();

            for (int i=0;i<nVertexCount;i++)
            {
                Vertex2D vertex = polygon.GetVertex(i);

                if (vertex is WallVertex)
                {
                    WallVertex vertex2 = (WallVertex)vertex;
                    PathItem item = (PathItem)vertex2.Tag;

                    if (item == null)
                        continue;

                    items.Add(item);
                    vPrev = polygon.GetVertex(++i);
                }
                else
                {
                    PathItem item = new PathItem();
                    item.SetLine(new Line2D(vPrev, vertex));
                    items.Add(item);
                    vPrev = vertex;
                }
            }

            return items;
        }

        private List<Polygon> MakeWallPolygons(List<PathItem> items1, List<PathItem> items2, Vertex2D vBeginNear1, Vertex2D vBeginNear2, Vertex2D vEndNear1, Vertex2D vEndNear2)
        {
            Polygon polygon = new Polygon();

            polygon.AddVertex(vBeginNear1);
            polygon.AddVertex(GetBeginVertex());
            polygon.AddVertex(vBeginNear2);
            polygon.AddVertex(vEndNear2);
            polygon.AddVertex(GetEndVertex());
            polygon.AddVertex(vEndNear1);

            List<Polygon> polygons = new List<Polygon>();
            polygons.Add(polygon);
            return polygons;
            /*PolygonBuilder polygonBuilder = new PolygonBuilder();

            AddVertexToPolygonBuilder(items1, polygonBuilder);
            AddVertexToPolygonBuilder(items2, polygonBuilder);

            Vertex2D vBegin = GetBeginVertex();
            Vertex2D vEnd = GetEndVertex();

            polygonBuilder.AddLine(vBegin, vBeginNear1);
            polygonBuilder.AddLine(vBegin, vBeginNear2);
            polygonBuilder.AddLine(vEnd, vEndNear1);
            polygonBuilder.AddLine(vEnd, vEndNear2);

            List<Line2D> lines = null;
            List<Polygon> polygons = polygonBuilder.MakePolygon(out lines);
            return polygons;*/
        }

        private static void AddVertexToPolygonBuilder(List<PathItem> items, PolygonBuilder polygonBuilder)
        {
            Vertex2D vBegin, vEnd, vMiddle;

            foreach (PathItem item in items)
            {
                if (item.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                    continue;

                if (item.GetDrawType() == PathItem.DrawType.Line)
                {
                    polygonBuilder.AddLine(vBegin, vEnd);
                }
                else if (item.GetDrawType() == PathItem.DrawType.Arc || item.GetDrawType() == PathItem.DrawType.EArc)
                {
                    if (vMiddle == null)
                        continue;

                    WallVertex middle = new WallVertex(vMiddle);
                    middle.Tag = item;

                    polygonBuilder.AddLine(vBegin, middle);
                    polygonBuilder.AddLine(middle, vEnd);
                }
            }
        }

        private PathItem GetPathItem(List<PathItem> items)
        {
            foreach (PathItem item in items)
            {
                if (item.Wall == this)
                    return item;
            }

            return null;
        }

        private void GetLineNearVertex(PathItem item, ref Vertex2D vBegin, ref Vertex2D vEnd)
        {
            Vertex2D wallBegin = GetBeginVertex();
            Vertex2D wallEnd = GetEndVertex();

            Vertex2D begin, end, middle;

            if (item.GetVertex(out begin, out end, out middle) == false)
                return;

            Vertex2D vB = UnE.Geometry.Math.GetNearestVertex(wallBegin, begin, end, true);
            Vertex2D vE = UnE.Geometry.Math.GetNearestVertex(wallEnd, begin, end, true);

            Line2D line = new Line2D(begin, end);

            bool includeBegin = line.IsInclude(vB);
            bool includeEnd = line.IsInclude(vE);

            if (includeBegin && includeEnd)
            {
                double dLen1 = begin.GetDistance(vB);
                double dLen2 = begin.GetDistance(vE);

                if (dLen1 < dLen2)
                {
                    vBegin = begin;
                    vEnd = end;
                }
                else
                {
                    vBegin = end;
                    vEnd = begin;
                }
            }
            else if (includeBegin == false && includeEnd)
            {
                double dLen1 = vB.GetDistance(begin);
                double dLen2 = vB.GetDistance(end);

                if (dLen1 < dLen2)
                {
                    vBegin = begin;
                    vEnd = end;
                }
                else
                {
                    vBegin = end;
                    vEnd = begin;
                }
            }
            else if (includeEnd == false && includeBegin)
            {
                double dLen1 = vE.GetDistance(begin);
                double dLen2 = vE.GetDistance(end);

                if (dLen1 < dLen2)
                {
                    vBegin = end;
                    vEnd = begin;
                }
                else
                {
                    vBegin = begin;
                    vEnd = end;
                }
            }
            else// if (includeBegin == false && includeEnd == false)
            {
                double dBB = vB.GetDistance(begin);
                double dBE = vB.GetDistance(end);
                double dEB = vE.GetDistance(begin);
                double dEE = vE.GetDistance(end);

                if (dBB < dBE && dEB < dEE)
                {
                    // 둘다 begin쪽에 있는 경우
                    if (dBB > dEB)
                    {
                        vBegin = begin;
                        vEnd = end;
                    }
                    else
                    {
                        vBegin = end;
                        vEnd = begin;
                    }
                }
                else if (dBB > dBE && dEB > dEE)
                {
                    // 둘다 end쪽에 있는 경우
                    if (dEE > dBE)
                    {
                        vBegin = begin;
                        vEnd = end;
                    }
                    else
                    {
                        vBegin = end;
                        vEnd = begin;
                    }
                }
                else
                {
                    // 두 점의 위치가 서로 다른 경우
                    if (dBB < dBE)
                    {
                        vBegin = begin;
                        vEnd = end;
                    }
                    else
                    {
                        vBegin = end;
                        vEnd = begin;
                    }
                }
            }
        }

        private void GetNearVertexFromDistance(Vertex2D begin, Vertex2D end, Vertex2D wallBegin, Vertex2D wallEnd, ref Vertex2D vBegin, ref Vertex2D vEnd)
        {
            double dLen1 = wallBegin.GetDistance(begin);
            double dLen2 = wallBegin.GetDistance(end);

            if (dLen1 < dLen2)
            {
                vBegin = begin;
                vEnd = end;
            }
            else
            {
                vBegin = end;
                vEnd = begin;
            }
        }

        private double GetEArcVertexAngle(Vertex2D vertex, EArc2D earc, Vertex2D vEArcVertex)
        {
            Vertex2D vCenter = earc.GetCenter();
            double dAngle = UnE.Geometry.Math.GetAngle(vertex, vCenter, vEArcVertex);

            if (earc.IsClockWise())
            {
                if (UnE.Geometry.Math.IsRightSideFromLine(vEArcVertex, vertex, vCenter) == 0)
                {
                    dAngle = UnE.Geometry.Math._2PI() - dAngle;
                }
            }
            else
            {
                if (UnE.Geometry.Math.IsRightSideFromLine(vEArcVertex, vertex, vCenter) == 1)
                {
                    dAngle = UnE.Geometry.Math._2PI() - dAngle;
                }
            }

            return dAngle;
        }

        private void GetEArcNearVertex(PathItem item, ref Vertex2D vBegin, ref Vertex2D vEnd)
        {
            EArc2D arcItem = item.GetEArc();
            EArc2D arc = m_gridType == GridType.Arc ? m_arc : m_earc;

            if (arcItem != null && arc != null)
            {
                bool isArc1 = arcItem is Arc2D;
                bool isArc2 = arc is Arc2D;

                if (isArc1 != isArc2)
                    return;

                Vertex2D wallBegin = arc.GetBeginVertex();
                Vertex2D wallEnd = arc.GetEndVertex();
                Vertex2D begin = arcItem.GetBeginVertex();
                Vertex2D end = arcItem.GetEndVertex();

                Line2D lineBegin = new Line2D(m_arc.GetCenter(), wallBegin, Line2D.LineType.HALF_LINE_BEGIN_2_END);
                Line2D lineEnd = new Line2D(m_arc.GetCenter(), wallEnd, Line2D.LineType.HALF_LINE_BEGIN_2_END);

                Vertex2D v1, v2, vB, vE;
                bool includeBegin = false, includeEnd = false;

                int nResult = arcItem.IntersectLine(lineBegin, out v1, out v2);

                if (nResult == 1)
                {
                    includeBegin = true;
                    vB = v1;
                }
                else// if (nResult == 0)
                {
                    arcItem.SetClosed(true);
                    nResult = arcItem.IntersectLine(lineBegin, out v1, out v2);
                    arcItem.SetClosed(false);

                    if (nResult == 1)
                        vB = v1;
                    else
                    {
                        GetNearVertexFromDistance(begin, end, wallBegin, wallEnd, ref vBegin, ref vEnd);
                        return;
                    }
                }

                nResult = arcItem.IntersectLine(lineEnd, out v1, out v2);

                if (nResult == 1)
                {
                    includeEnd = true;
                    vE = v1;
                }
                else// if (nResult == 0)
                {
                    arcItem.SetClosed(true);
                    nResult = arcItem.IntersectLine(lineEnd, out v1, out v2);
                    arcItem.SetClosed(false);

                    if (nResult == 1)
                        vE = v1;
                    else
                    {
                        GetNearVertexFromDistance(begin, end, wallBegin, wallEnd, ref vBegin, ref vEnd);
                        return;
                    }
                }

                if (includeBegin && includeEnd)
                {
                    double dAngleB = GetEArcVertexAngle(begin, arcItem, vB);
                    double dAngleE = GetEArcVertexAngle(begin, arcItem, vE);

                    if (dAngleB < dAngleE)
                    {
                        vBegin = begin;
                        vEnd = end;
                    }
                    else
                    {
                        vBegin = end;
                        vEnd = begin;
                    }
                }
                else if (includeBegin == false && includeEnd)
                {
                    double dAngleB = GetEArcVertexAngle(vB, arcItem, begin);
                    double dAngleE = GetEArcVertexAngle(vB, arcItem, end);

                    if (dAngleB < dAngleE)
                    {
                        vBegin = begin;
                        vEnd = end;
                    }
                    else
                    {
                        vBegin = end;
                        vEnd = begin;
                    }
                }
                else if (includeEnd == false && includeBegin)
                {
                    double dAngleB = GetEArcVertexAngle(vE, arcItem, begin);
                    double dAngleE = GetEArcVertexAngle(vE, arcItem, end);

                    if (dAngleB < dAngleE)
                    {
                        vBegin = end;
                        vEnd = begin;
                    }
                    else
                    {
                        vBegin = begin;
                        vEnd = end;
                    }
                }
                else// if (includeBegin == false && includeEnd == false)
                {
                    double dAngleBB = GetEArcVertexAngle(vB, arcItem, begin);
                    double dAngleBE = GetEArcVertexAngle(vB, arcItem, end);
                    double dAngleEB = GetEArcVertexAngle(vE, arcItem, begin);
                    double dAngleEE = GetEArcVertexAngle(vE, arcItem, end);

                    if (dAngleBB < dAngleBE && dAngleEB < dAngleEE)
                    {
                        // 둘다 begin쪽에 있는 경우
                        if (dAngleBB > dAngleEB)
                        {
                            vBegin = begin;
                            vEnd = end;
                        }
                        else
                        {
                            vBegin = end;
                            vEnd = begin;
                        }
                    }
                    else if (dAngleBB > dAngleBE && dAngleEB > dAngleEE)
                    {
                        // 둘다 end쪽에 있는 경우
                        if (dAngleEE > dAngleBE)
                        {
                            vBegin = begin;
                            vEnd = end;
                        }
                        else
                        {
                            vBegin = end;
                            vEnd = begin;
                        }
                    }
                    else
                    {
                        // 두 점의 위치가 서로 다른 경우
                        if (dAngleBB < dAngleBE)
                        {
                            vBegin = begin;
                            vEnd = end;
                        }
                        else
                        {
                            vBegin = end;
                            vEnd = begin;
                        }
                    }
                }
            }
        }

        private void GetNearVertex(List<PathItem> items, out Vertex2D vBegin, out Vertex2D vEnd)
        {
            vBegin = vEnd = null;
            //double lenBegin = 0.0, lenEnd = 0.0;

            PathItem item = GetPathItem(items);

            if (item == null)
                return;

            if (m_gridType == GridType.Line)
            {
                GetLineNearVertex(item, ref vBegin, ref vEnd);
            }
            else if (m_gridType == GridType.Arc || m_gridType == GridType.EArc)
            {
                GetEArcNearVertex(item, ref vBegin, ref vEnd);
            }

            /*foreach (PathItem item in items)
            {
                Vertex2D vB, vM, vE;

                if (item.GetVertex(out vB, out vE, out vM) == false)
                    continue;

                double len1 = wallBegin.GetDistance(vB);
                double len2 = wallEnd.GetDistance(vB);

                if (vBegin == null || len1 < lenBegin)
                {
                    vBegin = vB;
                    lenBegin = len1;
                }

                if (vEnd == null || len2 < lenEnd)
                {
                    vEnd = vB;
                    lenEnd = len2;
                }
            }*/
        }

        public static void MakeOutsideWallLine(Dictionary<int, Wall> dicWalls, Project.UnitOfLength unit)
        {
            List<Wall> lineWalls = null;
            List<Polygon> polygons = MakeOutsidePolygon(dicWalls, out lineWalls);

            if (polygons == null)
                return;

            foreach (Polygon polygon in polygons)
            {
                List<PathItem> centerItems = MakeOutsideCenterline(polygon, lineWalls);

                if (centerItems == null)
                    return;

                SetOutsideBoundary(polygon, centerItems, unit);
            }
        }

        private static void SetOutsideBoundary(Polygon polygon, List<PathItem> centerItems, Project.UnitOfLength unit)
        {
            bool isClockWise = polygon.IsClockWise();

            int nPathCount = centerItems.Count;
            PathItem prev = centerItems[0];
            double dPrevWallThick = Space.GetWallThick(0, centerItems, unit);
            PathItem prevItem = null;

            List<PathItem> innerItems = new List<PathItem>();

            for (int i = 1; i <= nPathCount; i++)
            {
                int nIndex = i < nPathCount ? i : 0;
                PathItem path = centerItems[nIndex];
                double dWallThick = Space.GetWallThick(nIndex, centerItems, unit);

                PathItem item1 = prevItem == null ? prev.Offset(-dPrevWallThick / 2, isClockWise) : prevItem;
                PathItem item2 = i < nPathCount ? path.Offset(-dWallThick / 2, isClockWise) : innerItems[0];

                if (i == 1)
                    innerItems.Add(item1);

                if (i < nPathCount)
                    innerItems.Add(item2);

                int nItem1Index = innerItems.Count - 2;
                int nResult = PathItem.CalcIntersection(item1, item2, innerItems, nItem1Index);

                if (nResult == 0)
                    return;

                prev = path;
                prevItem = item2;
                dPrevWallThick = dWallThick;
            }

            foreach (PathItem item in innerItems)
            {
                item.InnerToCenter();
            }

            foreach (PathItem item in centerItems)
            {
                Wall wall = item.Wall;

                if (wall == null)
                    continue;

                wall.m_outsideBoundaryPath = innerItems;
            }
        }

        private static List<PathItem> MakeOutsideCenterline(Polygon polygon, List<Wall> lineWalls)
        {
            List<PathItem> centerItems = new List<PathItem>();

            int nVertexCount = polygon.GetVertexCount();

            if (nVertexCount < 3)
                return null;

            Vertex2D vPrev = polygon.GetVertex(nVertexCount - 1);

            for (int i = 0; i < nVertexCount; i++)
            {
                Vertex2D vBegin = vPrev;
                Vertex2D vertex = polygon.GetVertex(i);

                if (vertex is WallVertex)
                {
                    Wall wall = (Wall)((WallVertex)vertex).Tag;
                    PathItem item = null;

                    if (wall == null)
                    {
                        System.Diagnostics.Trace.WriteLine("WallType Error");
                        return null;
                    }

                    if (wall.GetGridType() == Wall.GridType.Arc && wall.Arc != null)
                    {
                        item = new PathItem();
                        item.SetArc(wall.Arc);
                    }
                    else if (wall.GetGridType() == Wall.GridType.EArc && wall.EArc != null)
                    {
                        item = new PathItem();
                        item.SetEArc(wall.EArc);
                        centerItems.Add(item);
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("WallType Error");
                        return null;
                    }

                    centerItems.Add(item);
                    item.Wall = wall;

                    if (i == nVertexCount - 1)
                    {
                        System.Diagnostics.Trace.WriteLine("WallType Error");
                        return null;
                    }

                    vPrev = polygon.GetVertex(++i);
                }
                else
                {
                    Wall wall = FindLineWall(lineWalls, vBegin, vertex);

                    if (wall == null)
                    {
                        System.Diagnostics.Trace.WriteLine("No Wall Error");
                        return null;
                    }

                    PathItem item = new PathItem();
                    item.SetLine(new Line2D(vBegin, vertex));
                    item.Wall = wall;

                    centerItems.Add(item);

                    vPrev = vertex;
                }
            }

            return centerItems;
        }

        private static List<Polygon> MakeOutsidePolygon(Dictionary<int, Wall> dicWalls, out List<Wall> lineWalls)
        {
            PolygonBuilder polygonBuilder = new PolygonBuilder();
            lineWalls = new List<Wall>();

            foreach (KeyValuePair<int, Wall> pair in dicWalls)
            {
                Wall wall = pair.Value;

                if (wall.GetLinkedSpaceCount() == 1)
                {
                    if (wall.GetGridType() == Wall.GridType.Line)
                    {
                        lineWalls.Add(wall);
                        polygonBuilder.AddLine(wall.GetBeginVertex(), wall.GetEndVertex());
                    }
                    else if (wall.GetGridType() == Wall.GridType.Arc || wall.GetGridType() == Wall.GridType.EArc)
                    {
                        Vertex2D vMiddle = wall.GetGridType() == Wall.GridType.Arc ? GetEArcMiddle(wall.Arc) : GetEArcMiddle(wall.EArc);

                        if (vMiddle == null)
                        {
                            System.Diagnostics.Trace.WriteLine("EArc Error");
                            return null;
                        }

                        WallVertex wallVertex = new WallVertex(vMiddle);
                        wallVertex.Tag = wall;

                        polygonBuilder.AddLine(wall.GetBeginVertex(), wallVertex);
                        polygonBuilder.AddLine(wallVertex, wall.GetEndVertex());
                    }
                }
            }

            List<Line2D> lines = null;
            List<Polygon> polygons = polygonBuilder.MakePolygon(out lines);
            return polygons;
        }

        private static Wall FindLineWall(List<Wall> lineWalls, Vertex2D v1, Vertex2D v2)
        {
            foreach (Wall wall in lineWalls)
            {
                if (wall.Line.IsInclude(v1) && wall.Line.IsInclude(v2))
                    return wall;
            }

            return null;
        }

        private static Vertex2D GetEArcMiddle(EArc2D earc)
        {
            Vertex2D vMiddle = null;

            if (earc != null)
            {
                Vertex2D vBegin = earc.GetBeginVertex();
                Vertex2D vEnd = earc.GetEndVertex();

                if (earc.GetVertex(earc.GetBeginAngle() + earc.GetAngle() / 2, out vMiddle) == false)
                    return null;
            }

            return vMiddle;
        }

        public override void Render(Graphics g, Pen pen, Brush brush, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            if (m_dThick == 0.0)
            {
                if (m_selected)
                {
                    // 가벽의 벽체 중심선
                    if (m_layer.CenterLinePen != null)
                    {
                        Pen selectedPen = new Pen(m_selectedLineColor, m_layer.CenterLinePen.Width);
                        DrawCenterLine(g, selectedPen);
                        selectedPen.Dispose();
                    }
                }
                else
                {
                    // 가벽의 벽체 중심선
                    if (m_layer.CenterLinePen != null)
                        DrawCenterLine(g, m_layer.CenterLinePen);
                }

                return;
            }

            if (m_vBR.x <= vClientAreaTL.x || m_vTL.x >= vClientAreaBR.x)
                return;

            if (m_vTL.y <= vClientAreaBR.y || m_vBR.y >= vClientAreaTL.y)
                return;

            if (m_path != null)
            {
                // 벽체 그리는 곳
                if (m_selected)
                {
                    Brush selectedBrush = new SolidBrush(m_selectedFillColor);
                    g.FillPath(selectedBrush, m_path);
                    selectedBrush.Dispose();
                }
                else
                {
                    if (brush != null)
                    {
                        // 커튼월 벽체 색 지정
                        if (m_component.TypeName == "CurtainWall")
                            brush = new SolidBrush(Color.FromArgb(0, 255, 255));

                        g.FillPath(brush, m_path);
                    }
                        
                    // 벽체 외곽선
                    if (pen != null)
                        g.DrawPath(pen, m_path);
                }
            }

            // 벽체 중심선
            if (m_layer.CenterLinePen != null)
            {
                DrawCenterLine(g, m_layer.CenterLinePen);
            }
        }

        private void DrawCenterLine(Graphics g, Pen pen)
        {
            Vertex2D vBegin = this.GetBeginVertex();
            Vertex2D vEnd = this.GetEndVertex();

            //Color clrOld = pen.Color;
            //pen.Color = Color.Yellow;

            g.DrawLine(pen, (float)(vBegin.x + m_dMoveX), (float)(vBegin.y + m_dMoveY), (float)(vEnd.x + m_dMoveX), (float)(vEnd.y + m_dMoveY));

            //pen.Color = clrOld;
        }

        public override void Move(double dMoveX, double dMoveY)
        {
            if (m_path != null)
            {
                m_path.Dispose();
                m_path = Space.MakeGraphicsPath(m_boundary, dMoveX, dMoveY);
                SetBoundaryPolygon();
            }

            m_vTL.x = m_vOriginTL.x + dMoveX;
            m_vTL.y = m_vOriginTL.y + dMoveY;
            m_vBR.x = m_vOriginBR.x + dMoveX;
            m_vBR.y = m_vOriginBR.y + dMoveY;

            m_dMoveX = dMoveX;
            m_dMoveY = dMoveY;
        }

        public void AddDoor(Door door)
        {
            m_doors.Add(door);
            door.Wall = this;
        }

        public void AddWindow(Window window)
        {
            m_windows.Add(window);
            window.Wall = this;
        }

        public void MakeGridID()
        {
            if (m_nID > 0)
            {
                m_strGridID = "grid" + m_nID.ToString();
            }
            else
            {
                m_strGridID = "grid" + System.Guid.NewGuid().ToString();
            }
        }

        public override bool HitTest(Vertex2D vertex)
        {
            if (m_boundaryPolygon == null || m_boundaryPolygon.GetVertexCount() < 3)
            {
                VariousData<double> distance = m_layer.GetSnapDistance();

                if (distance == null)
                    return false;

                double dLen = GetDistanceFromCenterLine(vertex);

                if (dLen <= distance.Data)
                    return true;
            }

            if (m_boundaryPolygon.HitTest(vertex) != 0)
                return true;

            return false;
        }

        public double GetDistanceFromCenterLine(Vertex2D vertex)
        {
            double dLen = 0.0;

            if (m_gridType == GridType.Line && m_line != null)
            {
                dLen = m_line.GetDistance(vertex, false);
            }
            else if (m_gridType == GridType.Arc)
            {
                if (m_arc == null)
                    return -1;

                Vertex2D vCenter = m_arc.GetCenter();

                if (vCenter.GetDistance(vertex) <= UnE.Geometry.Math.HALF_TOLERANCE())
                    return m_arc.GetRadius();

                Vertex2D vR = new Vertex2D(vCenter.x + 100.0, vCenter.y);
                double dAngle = UnE.Geometry.Math.GetAngle(vertex, vCenter, vR);

                if (vertex.y < vCenter.y)
                    dAngle = UnE.Geometry.Math._2PI() - dAngle;

                Vertex2D vArc;

                if (m_arc.GetVertex(dAngle, out vArc))
                    return vArc.GetDistance(vertex);

                double dLen1 = vertex.GetDistance(m_arc.GetBeginVertex());
                double dLen2 = vertex.GetDistance(m_arc.GetEndVertex());
                dLen = dLen1 < dLen2 ? dLen1 : dLen2;
            }
            else if (m_gridType == GridType.EArc)
            {
                if (m_earc == null)
                    return -1;

                Vertex2D vCenter = m_earc.GetCenter();

                if (vCenter.GetDistance(vertex) <= UnE.Geometry.Math.HALF_TOLERANCE())
                    return GetVertexToEArcLength(m_earc, vCenter);

                dLen = GetVertexToEArcLength(m_earc, vertex);

                Vertex2D vR = new Vertex2D(vCenter.x + 100.0, vCenter.y);
                double dAngle = UnE.Geometry.Math.GetAngle(vertex, vCenter, vR);

                if (vertex.y < vCenter.y)
                    dAngle = UnE.Geometry.Math._2PI() - dAngle;

                Vertex2D vEArc;

                if (m_earc.GetVertex(dAngle, out vEArc))
                {
                    double length = vEArc.GetDistance(vertex);

                    if (length < dLen)
                        dLen = length;
                }
            }

            return dLen;
        }

        private double GetVertexToEArcLength(EArc2D earc, Vertex2D vertex)
        {
            Vertex2D vRight = null, vTop = null, vLeft = null, vBottom = null;

            bool right = earc.GetVertex(0.0, out vRight);
            bool top = earc.GetVertex(UnE.Geometry.Math.HALF_PI(), out vTop);
            bool left = earc.GetVertex(UnE.Geometry.Math._3HALF_PI(), out vLeft);
            bool bottom = earc.GetVertex(UnE.Geometry.Math._2PI(), out vBottom);

            double dLen1 = vertex.GetDistance(earc.GetBeginVertex());
            double dLen2 = vertex.GetDistance(earc.GetEndVertex());

            double dLen = dLen1 < dLen2 ? dLen1 : dLen2;

            if (right)
            {
                double length = vertex.GetDistance(vRight);

                if (dLen > length)
                    dLen = length;
            }

            if (top)
            {
                double length = vertex.GetDistance(vTop);

                if (dLen > length)
                    dLen = length;
            }

            if (left)
            {
                double length = vertex.GetDistance(vLeft);

                if (dLen > length)
                    dLen = length;
            }

            if (bottom)
            {
                double length = vertex.GetDistance(vBottom);

                if (dLen > length)
                    dLen = length;
            }

            return dLen;
        }
    }
}
