using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CadToXML
{
    public partial class Wall
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

        private Polygon m_boundaryPolygon = null;

        // 벽체가 한쪽면만 공간과 연결되어 있을 경우 나머지 면의 외곽영역 계산을 위한 임시 데이터
        private List<PathItem> m_outsideBoundaryPath = null;
        private List<PathItem> m_boundary = null;
        private GraphicsPath m_path = null;
        protected Vertex2D m_vTL = null;
        protected Vertex2D m_vBR = null;
        private Vertex2D m_vOriginTL = null;
        private Vertex2D m_vOriginBR = null;

        private GridType m_gridType = GridType.Line;
        private Arc2D m_arc = null;
        private EArc2D m_earc = null;

        public Arc2D Arc
        {
            get { return m_arc; }
            set
            {
                m_arc = value;

                if (m_arc != null)
                    m_gridType = GridType.Arc;
            }
        }

        public EArc2D EArc
        {
            get { return m_earc; }
            set
            {
                m_earc = value;

                if (m_earc != null)
                    m_gridType = GridType.EArc;
            }
        }

        public List<PathItem> Boundary
        {
            get { return m_boundary; }
        }

        public Polygon BoundaryPolygon
        {
            get { return m_boundaryPolygon; }
        }

        public Vertex2D GetBeginVertex()
        {
            return m_vBegin;
        }

        public Vertex2D GetEndVertex()
        {
            return m_vEnd;
        }

        public GridType GetGridType()
        {
            return m_gridType;
        }

        public int GetLinkedSpaceCount()
        {
            if (m_linkedSpace1 != null && m_linkedSpace2 != null)
                return 2;
            else if (m_linkedSpace1 != null)
                return 1;
            else if (m_linkedSpace2 != null)
                return 1;

            return 0;
        }

        public static void MakeOutsideWallLine(List<Wall> walls, Project.UnitOfLength unit)
        {
            List<Wall> lineWalls = null;
            List<Polygon> polygons = MakeOutsidePolygon(walls, out lineWalls);

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

                foreach (PathItem path in innerItems)
                {
                    Vertex2D vBegin, vMiddle, vEnd;
                    if (path.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                        continue;

                    // Column
                    /*Line line = new Line();
                    line.Begin = new Vertex2D(vBegin);
                    line.End = new Vertex2D(vEnd);
                    layer.AddShape(line);*/
                }
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

        private static Wall FindLineWall(List<Wall> lineWalls, Vertex2D v1, Vertex2D v2)
        {
            foreach (Wall wall in lineWalls)
            {
                if (wall.Line.IsInclude(v1) && wall.Line.IsInclude(v2))
                    return wall;
            }

            return null;
        }

        private static List<Polygon> MakeOutsidePolygon(List<Wall> walls, out List<Wall> lineWalls)
        {
            // TODO: 디버그용
            PolygonBuilder polygonBuilder = new PolygonBuilder();
            //CadToXML.Geometry.PolygonBuilder polygonBuilder = new CadToXML.Geometry.PolygonBuilder();
            lineWalls = new List<Wall>();

            foreach (Wall wall in walls)
            {
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

            List<Arc2D> arcs = null;
            List<EArc2D> earcs = null;

            // TODO: 디버그용
            List<Polygon> polygons = polygonBuilder.MakePolygon(out lines);
            //List<Polygon> polygons = polygonBuilder.MakePolygon(out lines, out arcs, out earcs);

            return polygons;
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

        public bool MakeShape(Floor level)
        {
            if (m_dThick == 0.0)
            {
                CenterlineToBoundary();
                return true;
            }

            List<PathItem> items1 = null, items2 = null;

            if (GetLinkedSpaceCount() == 0)
            {
                // 공간에 속하지 않는 벽체
                MakeNoSpaceWallShape(level);
                SetBoundaryPolygon();
                return true;
            }
            else if (GetLinkedSpaceCount() == 1)
            {
                Space space = m_linkedSpace1 != null ? m_linkedSpace1 : m_linkedSpace2;
                items1 = space.Boundary;
                items2 = m_outsideBoundaryPath;
            }
            else// if (GetLinkedSpaceCount() == 2)
            {
                items1 = m_linkedSpace1.Boundary;
                items2 = m_linkedSpace2.Boundary;
            }

            // 둘중 하나가 없을 경우 벽체의 중심선을 기준으로 반대편에 복사한다.
            if (items1 == null && items2 == null)
                return false;
            else if (items1 == null)
                items1 = MirrorItems(items2);
            else if (items2 == null)
                items2 = MirrorItems(items1);
            //if (items1 == null || items2 == null)
            //    return false;

            Vertex2D vBeginNear1, vBeginNear2, vEndNear1, vEndNear2;

            GetNearVertex(items1, out vBeginNear1, out vEndNear1);
            GetNearVertex(items2, out vBeginNear2, out vEndNear2);

            List<Polygon> polygons = MakeWallPolygons(items1, items2, vBeginNear1, vBeginNear2, vEndNear1, vEndNear2);

            if (polygons == null)
                return false;

            return SelectBoundaryPolygon(polygons);
        }

        // items를 벽체의 중심선을 기준으로 반대편에 복사한다.
        private List<PathItem> MirrorItems(List<PathItem> items)
        {
            if (this.m_gridType == GridType.Line)
            {
                return MirrorItemsWithLine(items, Line);
            }

            return null;
        }

        private List<PathItem> MirrorItemsWithLine(List<PathItem> items, Line2D line)
        {
            Vertex2D v1 = line.GetVertex(true);
            Vertex2D v2 = line.GetVertex(false);
            List<PathItem> mirrorItems = new List<PathItem>();

            Vertex2D vBegin, vEnd, vMiddle;
            Vertex2D vResult1, vResult2;

            foreach (PathItem item in items)
            {
                if (item.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                    continue;

                if (item.GetDrawType() == PathItem.DrawType.Line)
                {
                    if (vBegin.Mirror(v1, v2, out vResult1) == false)
                        continue;
                    if (vEnd.Mirror(v1, v2, out vResult2) == false)
                        continue;

                    PathItem newItem = new PathItem();
                    newItem.SetLine(new Line2D(vResult1, vResult2));
                    newItem.Wall = item.Wall;
                    mirrorItems.Add(newItem);
                }
                else if (item.GetDrawType() == PathItem.DrawType.Arc)
                {
                    Arc2D arc = (Arc2D)item.GetEArc();
                    Arc2D arcResult;

                    if (arc.Mirror(v1, v2, out arcResult) == false)
                        continue;
                    
                    PathItem newItem = new PathItem();
                    newItem.SetArc(arcResult);
                    newItem.Wall = item.Wall;
                    mirrorItems.Add(newItem);
                }
                else if (item.GetDrawType() == PathItem.DrawType.EArc)
                {
                    EArc2D earc = item.GetEArc();
                    EArc2D earcResult;

                    if (earc.Mirror(v1, v2, out earcResult) == false)
                        continue;

                    PathItem newItem = new PathItem();
                    newItem.SetEArc(earcResult);
                    newItem.Wall = item.Wall;
                    mirrorItems.Add(newItem);
                }
            }

            return mirrorItems;
        }

        private void CenterlineToBoundary()
        {
            PathItem item = new PathItem();
            GridType type = GetGridType();

            if (type == GridType.Line)
            {
                item.SetLine(m_line);
            }
            else if (type == GridType.Arc)
            {
                item.SetArc(this.Arc);
            }
            else if (type == GridType.EArc)
            {
                item.SetEArc(this.EArc);
            }
            else
                return;

            m_boundary = new List<PathItem>();
            m_boundary.Add(item);
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
        }

        private void GetNearVertex(List<PathItem> items, out Vertex2D vBegin, out Vertex2D vEnd)
        {
            vBegin = vEnd = null;
            //double lenBegin = 0.0, lenEnd = 0.0;

            PathItem item = GetPathItem(items);

            if (item == null)
                return;

            GridType gridType = GetGridType();

            if (gridType == GridType.Line)
            {
                GetLineNearVertex(item, ref vBegin, ref vEnd);
            }
            else if (gridType == GridType.Arc || gridType == GridType.EArc)
            {
                GetEArcNearVertex(item, ref vBegin, ref vEnd);
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
            EArc2D arc = GetGridType() == GridType.Arc ? Arc : EArc;

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

                Line2D lineBegin = new Line2D(Arc.GetCenter(), wallBegin, Line2D.LineType.HALF_LINE_BEGIN_2_END);
                Line2D lineEnd = new Line2D(Arc.GetCenter(), wallEnd, Line2D.LineType.HALF_LINE_BEGIN_2_END);

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

        // 두께가 0인 벽체를 제거한다..
        private void RemoveZeroThickWall(List<Wall> walls)
        {
            for (int i=walls.Count-1;i>=0;i--)
            {
                Wall wall = walls[i];

                if (wall.Thick == 0.0)
                    walls.RemoveAt(i);
            }
        }

        // 공간에 속하지 않는 벽체
        // LineType에 대해서만 구현
        // Arc와 EArc Type은 추후 구현할 것
        private void MakeNoSpaceWallShape(Floor level)
        {
            List<Wall> beginLinkedWalls = level.GetLinkedWall(this, true);
            List<Wall> endLinkedWalls = level.GetLinkedWall(this, false);

            if (beginLinkedWalls == null || endLinkedWalls == null)
                return;

            RemoveZeroThickWall(beginLinkedWalls);
            RemoveZeroThickWall(endLinkedWalls);

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

            SelectBoundaryPolygon(polygons);
        }

        private bool SelectBoundaryPolygon(List<Polygon> polygons)
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

            for (int i = 0; i < nVertexCount; i++)
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

            for (int i = 0; i < nVertexCount; i++)
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

            if (GetGridType() == GridType.Line)
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

        public Vertex2D GetMiddleVertex()
        {
            Vertex2D vMiddle = null;
            GridType gridType = GetGridType();

            if (gridType == GridType.Line)
            {
                if (m_line != null)
                {
                    vMiddle = (m_line.GetVertex(true) + m_line.GetVertex(false)) / 2;
                }
            }
            else if (gridType == GridType.Arc || gridType == GridType.EArc)
            {
                EArc2D earc = EArc;

                if (gridType == GridType.Arc)
                    earc = Arc;

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

        public void RemoveSpace(Space space)
        {
            if (m_linkedSpace1 == space)
                m_linkedSpace1 = null;

            if (m_linkedSpace2 == space)
                m_linkedSpace2 = null;
        }
    }
}
