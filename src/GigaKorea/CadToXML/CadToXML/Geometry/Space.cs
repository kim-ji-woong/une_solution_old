using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace CadToXML
{
    public partial class Space
    {
        // 벽체 중심선을 이용한 외곽선 정보
        private List<PathItem> m_shapeCenterItems = new List<PathItem>();
        // 벽체영역을 제외한 외곽선 정보
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

        public Polygon BoundaryPolygon
        {
            get { return m_boundaryPolygon; }
        }

        public bool MakeShape(Project.UnitOfLength unit)
        {
            if (MakeCenterLineBoundary() == false)
                return false;

            if (MakeInnerLineBoundary(unit) == false)
                return false;

            m_path = MakeGraphicsPath(m_shapeInnerItems, this, ref m_boundaryPolygon);
            CheckPolygonValidation(m_boundaryPolygon);

            if (m_boundaryPolygon.GetVertexCount() < 3)
                return false;

            return true;
        }

        public static void CheckPolygonValidation(Polygon polygon)
        {
            int nVertexCount = polygon.GetVertexCount();

            if (nVertexCount < 3)
                return;

            Vertex2D vBegin = polygon.GetVertex(0);
            bool isSame = false;

            do
            {
                Vertex2D vEnd = polygon.GetVertex(nVertexCount - 1);

                if (DXFManager.IsSameVertex(vBegin, vEnd))
                {
                    isSame = true;
                    polygon.RemoveVertex(nVertexCount - 1);
                    nVertexCount--;

                    if (nVertexCount < 3)
                        return;
                }
                else
                    isSame = false;
            }
            while (isSame);

            List < Vertex2D > validVertexList = new List<Vertex2D>();
            validVertexList.Add(polygon.GetVertex(0));
            validVertexList.Add(polygon.GetVertex(1));

            for (int i = 1; i < nVertexCount; i++)
            {
                Vertex2D v1 = polygon.GetVertex(i);
                Vertex2D v2 = i == nVertexCount - 1 ? polygon.GetVertex(0) : polygon.GetVertex(i + 1);
                Line2D line = new Line2D(v1, v2);

                if (CheckIntersection(line, validVertexList, i == nVertexCount - 1))
                {
                    if (i < nVertexCount - 1)
                        validVertexList.Add(v2);
                }
            }

            polygon.Clear();

            foreach (Vertex2D vertex in validVertexList)
            {
                polygon.AddVertex(vertex);
            }

            validVertexList.Clear();
            nVertexCount = polygon.GetVertexCount();

            if (nVertexCount == 0)
                return;

            // 같은 좌표가 연속적으로 나타나면 없애준다.
            Vertex2D vPrev = polygon.GetVertex(0);

            for (int i = 1; i < nVertexCount; i++)
            {
                Vertex2D vCurrent = polygon.GetVertex(i);

                if (DXFManager.IsSameVertex(vPrev, vCurrent))
                {
                    polygon.RemoveVertex(i);
                    nVertexCount--;
                    i--;
                    continue;
                }

                vPrev = vCurrent;
            }

            nVertexCount = polygon.GetVertexCount();
        }

        private static bool CheckIntersection(Line2D line, List<Vertex2D> polygon, bool last)
        {
            Vertex2D vBegin = line.GetVertex(true), vEnd = line.GetVertex(false);
            Vertex2D vResult1, vResult2;
            Line2D.LineType resultType;

            int nVertexCount = polygon.Count;
            
            for (int i = 0; i < nVertexCount; i++)
            {
                Vertex2D v1 = polygon[i];
                Vertex2D v2 = i == nVertexCount - 1 ? polygon[0] : polygon[i + 1];
                
                Line2D line2 = new Line2D(v1, v2);
                int nResult = line.IntersectLine(line2, out vResult1, out vResult2, out resultType);

                if (nResult == 2)
                {
                    nResult = 1;
                    double len1 = vResult1.GetDistance(v2);
                    double len2 = vResult2.GetDistance(v2);

                    if (len2 < len1)
                    {
                        vResult1 = vResult2;
                    }
                }

                if (nResult == 1)
                {
                    if ((i == nVertexCount - 2 && DXFManager.IsSameVertex(vResult1, v2)) || (i == nVertexCount - 1 && DXFManager.IsSameVertex(vResult1, v1)))
                        return true;
                    else if (last && DXFManager.IsSameVertex(vResult1, polygon[0]) && (DXFManager.IsSameVertex(vResult1, vBegin) || DXFManager.IsSameVertex(vResult1, vEnd)))
                        continue;
                    else
                    {
                        // 오류
                        if (DXFManager.IsSameVertex(vResult1, v2))
                        {
                            if (i < nVertexCount - 1)
                            {
                                for (int j=i + 2;j<nVertexCount;j++)
                                {
                                    polygon.RemoveAt(i + 2);
                                }
                            }
                        }
                        else
                        {
                            if (i < nVertexCount - 1)
                            {
                                for (int j = i + 1; j < nVertexCount; j++)
                                {
                                    polygon.RemoveAt(i + 1);
                                }
                            }

                            polygon.Add(vResult1);

                            if (DXFManager.IsSameVertex(vResult1, vEnd) == false)
                                polygon.Add(vEnd);
                        }

                        return false;
                    }
                }
            }

            return true;
        }

        private bool CheckIntersection(PathItem item, List<PathItem> items, bool last)
        {
            if (item.GetDrawType() != PathItem.DrawType.Line)
                return true;

            Vertex2D v1, v2, vMiddle;
            Vertex2D vBegin, vEnd;
            Vertex2D vResult1, vResult2;
            Line2D.LineType resultType;

            if (item.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                return false;

            Line2D line = new Line2D(vBegin, vEnd);
            int nItemCount = items.Count;
            List<Vertex2D> polygon = new List<Vertex2D>();

            for (int i = 0; i < nItemCount; i++)
            {
                PathItem _item = items[i];

                if (_item.GetDrawType() == PathItem.DrawType.Line)
                {
                    if (_item.GetVertex(out v1, out v2, out vMiddle) == false)
                        continue;

                    int nVertexCount = polygon.Count;

                    if (nVertexCount == 0)
                    {
                        polygon.Add(v1);
                        polygon.Add(v2);
                    }
                    else if (nVertexCount == 2)
                    {
                        double len11 = v1.GetDistance(polygon[0]);
                        double len12 = v1.GetDistance(polygon[1]);
                        double len21 = v2.GetDistance(polygon[0]);
                        double len22 = v2.GetDistance(polygon[1]);

                        if (len11 < len12 && len11 < len21 && len11 < len22)
                        {
                            polygon.RemoveAt(0);
                            polygon.Add(v1);
                            polygon.Add(v2);
                        }
                        else if (len12 < len11 && len12 < len21 && len12 < len22)
                        {
                            polygon.Add(v2);
                        }
                        else if (len21 < len11 && len21 < len12 && len21 < len22)
                        {
                            polygon.RemoveAt(0);
                            polygon.Add(v2);
                            polygon.Add(v1);
                        }
                        else// if (len22 < len11 && len22 < len12 && len22 < len21)
                        {
                            polygon.Add(v1);
                        }
                    }
                    else
                    {
                        double len1 = v1.GetDistance(polygon[nVertexCount - 1]);
                        double len2 = v2.GetDistance(polygon[nVertexCount - 1]);

                        if (len1 < len2)
                            polygon.Add(v2);
                        else
                            polygon.Add(v1);
                    }
                }
            }

            for (int i=0;i<nItemCount;i++)
            {
                PathItem _item = items[i];

                if (_item.GetDrawType() == PathItem.DrawType.Line)
                {
                    if (_item.GetVertex(out v1, out v2, out vMiddle) == false)
                        continue;

                    Line2D line2 = new Line2D(v1, v2);
                    int nResult = line.IntersectLine(line2, out vResult1, out vResult2, out resultType);

                    if (nResult == 2)
                    {
                        nResult = 1;
                        double len1 = vResult1.GetDistance(polygon[i + 1]);
                        double len2 = vResult2.GetDistance(polygon[i + 1]);

                        if (len2 < len1)
                        {
                            vResult1 = vResult2;
                        }
                    }

                    if (nResult == 1)
                    {
                        if (i == nItemCount - 1 && DXFManager.IsSameVertex(vResult1, polygon[i + 1]))
                            continue;
                        else if (last && DXFManager.IsSameVertex(vResult1, polygon[0]) && (DXFManager.IsSameVertex(vResult1, vBegin) || DXFManager.IsSameVertex(vResult1, vEnd)))
                            continue;
                        else
                        {
                            // 오류
                            if (DXFManager.IsSameVertex(vResult1, polygon[i + 1]) == false)
                            {
                                _item.SetLine(new Line2D(polygon[i], vResult1));
                            }

                            for (int j = i + 1; j < nItemCount; j++)
                            {
                                items.RemoveAt(i + 1);
                            }

                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static GraphicsPath MakeGraphicsPath(List<PathItem> items, Space space, ref Polygon polygon, double x = 0.0, double y = 0.0)
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

        private bool MakeInnerLineBoundary(Project.UnitOfLength unit)
        {
            m_shapeInnerItems.Clear();

            // m_shapeCenterItems에는 Arc나 EArc가 포함되어 있을수 있으므로 Arc와 EAr를 임시로 직선으로 변환시킨다.
            Polygon polygon = new Polygon();
            Vertex2D vBegin, vMiddle, vEnd;

            int nPathCount = m_shapeCenterItems.Count;

            for (int i = 0; i < nPathCount; i++)
            {
                PathItem path = m_shapeCenterItems[i];

                if (path.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                    return false;

                if (i == 0)
                    polygon.AddVertex(vBegin);

                if (vMiddle != null)
                    polygon.AddVertex(vMiddle);

                polygon.AddVertex(vEnd);
            }

            if (polygon.GetVertexCount() < 3)
                return false;

            bool isClockWise = polygon.IsClockWise();
            polygon.Dispose();

            PathItem prev = m_shapeCenterItems[0];
            double dPrevWallThick = GetWallThick(0, m_shapeCenterItems, unit);
            PathItem prevItem = null;

            for (int i = 1; i <= nPathCount; i++)
            {
                int nIndex = i < nPathCount ? i : 0;

                PathItem path = m_shapeCenterItems[nIndex];
                double dWallThick = GetWallThick(nIndex, m_shapeCenterItems, unit);

                PathItem item1 = prevItem == null ? prev.Offset(dPrevWallThick / 2, isClockWise) : prevItem;
                PathItem item2 = i < nPathCount ? path.Offset(dWallThick / 2, isClockWise) : m_shapeInnerItems[0];

                if (i == 1)
                    m_shapeInnerItems.Add(item1);

                if (i < nPathCount)
                    m_shapeInnerItems.Add(item2);

                int nItem1Index = m_shapeInnerItems.Count - 2;
                int nResult = PathItem.CalcIntersection(item1, item2, m_shapeInnerItems, nItem1Index);

                if (nResult == 0)
                    return false;

                prev = path;
                prevItem = item2;
                dPrevWallThick = dWallThick;
            }

            // 계산결과는 PathItem의 m_innerXXX에 저장되어 있는데 이 정보를 모두 m_XXX으로 옮긴다.
            foreach (PathItem item in m_shapeInnerItems)
            {
                item.InnerToCenter();
            }

            return true;
        }

        public static double GetWallThick(int nPathIndex, List<PathItem> items, Project.UnitOfLength unit)
        {
            int nIndex = nPathIndex;

            do
            {
                PathItem item = items[nIndex--];

                if (item.Wall != null && item.Wall.Thick > 0.0)
                    return item.Wall.Thick;

                if (nIndex < 0)
                    nIndex = items.Count - 1;
            }
            while (nIndex != nPathIndex);

            // 모든 벽의 두께가 0일 경우 기본벽 두께를 30cm로 정한다.
            if (unit == Project.UnitOfLength.MM)
                return 300;
            else if (unit == Project.UnitOfLength.CM)
                return 30;
            //else if (unit == Project.UnitOfLength.M)
            return 0.3;
        }

        private bool MakeCenterLineBoundary()
        {
            m_shapeCenterItems.Clear();

            Vertex2D vNext = null;
            Vertex2D vTL = null;
            Vertex2D vBR = null;

            for (int i = 0; i < m_walls.Count; i++)
            {
                Wall wall = m_walls[i];

                Vertex2D vBegin1 = wall.GetBeginVertex();
                Vertex2D vEnd1 = wall.GetEndVertex();

                if (vBegin1 == null || vEnd1 == null)
                {
                    m_shapeCenterItems.Clear();
                    return false;
                }

                if (vNext == null)
                {
                    Wall wall2 = m_walls[i + 1];

                    Vertex2D vBegin2 = wall2.GetBeginVertex();
                    Vertex2D vEnd2 = wall2.GetEndVertex();

                    if (vBegin2 == null || vEnd2 == null)
                    {
                        m_shapeCenterItems.Clear();
                        return false;
                    }

                    if (vEnd1.GetDistance(vBegin2) < 0.1 || vEnd1.GetDistance(vEnd2) < 0.1)
                    {
                        SetShapeItems(vBegin1, vEnd1, wall);
                        vNext = vEnd1;

                        if (vTL == null)
                        {
                            vTL = new UnE.Geometry.Vertex2D();
                            vBR = new UnE.Geometry.Vertex2D();

                            vTL.x = vBR.x = vBegin1.x;
                            vTL.y = vBR.y = vBegin1.y;
                        }
                        else
                        {
                            SetBoundary(vTL, vBR, vBegin1);
                        }
                    }
                    else
                    {
                        SetShapeItems(vEnd1, vBegin1, wall);
                        vNext = vBegin1;

                        if (vTL == null)
                        {
                            vTL = new UnE.Geometry.Vertex2D();
                            vBR = new UnE.Geometry.Vertex2D();

                            vTL.x = vBR.x = vEnd1.x;
                            vTL.y = vBR.y = vEnd1.y;
                        }
                        else
                        {
                            SetBoundary(vTL, vBR, vEnd1);
                        }
                    }
                }
                else
                {
                    if (vNext.GetDistance(vBegin1) < 0.1)
                    {
                        SetShapeItems(vBegin1, vEnd1, wall);
                        vNext = vEnd1;
                    }
                    else
                    {
                        SetShapeItems(vEnd1, vBegin1, wall);
                        vNext = vBegin1;
                    }
                }

                SetBoundary(vTL, vBR, vNext);
            }

            if (vTL != null && vBR != null)
            {
                m_vTL = vTL;
                m_vBR = vBR;

                m_vOriginTL = new Vertex2D(m_vTL);
                m_vOriginBR = new Vertex2D(m_vBR);
            }

            return true;
        }

        private void SetShapeItems(Vertex2D vBegin, Vertex2D vEnd, Wall wall)
        {
            PathItem item = new PathItem();
            item.Wall = wall;

            if (wall.GetGridType() == Wall.GridType.Line)
                item.SetLine(new Line2D(vBegin, vEnd));
            else if (wall.GetGridType() == Wall.GridType.Arc)
                item.SetArc(wall.Arc, vBegin);
            else if (wall.GetGridType() == Wall.GridType.EArc)
                item.SetEArc(wall.EArc, vBegin);

            m_shapeCenterItems.Add(item);
        }

        private void SetBoundary(Vertex2D vTL, Vertex2D vBR, Vertex2D vPos)
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
    }
}
