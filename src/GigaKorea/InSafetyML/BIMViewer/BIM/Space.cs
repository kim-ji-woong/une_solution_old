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
    public class Space : Shape
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private string m_strName = "";
        private List<Wall> m_walls = new List<Wall>();

        private List<Property> m_properties = new List<Property>();

        private Level m_level = null;

        // 벽체 중심선을 이용한 외곽선 정보
        private List<PathItem> m_shapeCenterItems = new List<PathItem>();
        // 벽체영역을 제외한 외곽선 정보
        private List<PathItem> m_shapeInnerItems = new List<PathItem>();
        // HitTest를 위한 Polygon
        private Polygon m_polygon = new Polygon();

        private GraphicsPath m_path = null;
        private Vertex2D m_vNamePosition = null;

        private Vertex2D m_vOriginTL = null;
        private Vertex2D m_vOriginBR = null;

        private Boundary m_boundaryData = null;
        public Boundary BoundaryData
        {
            get { return m_boundaryData; }
            set { m_boundaryData = value; }
        }

        private List<Boundary> m_holeBoundary = null;
        public List<Boundary> HoleBoundary
        {
            get { return m_holeBoundary; }
            set { m_holeBoundary = value; }
        }

        // 방화구획
        private bool m_safetyFire = false;

        public const string SafetyFireTag = "IsSafetyFire";

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

        public List<PathItem> Boundary
        {
            get { return m_shapeInnerItems; }
        }

        public Vertex2D NamePosition
        {
            get { return m_vNamePosition; }
            set { m_vNamePosition = value; }
        }

        public bool SafetyFire
        {
            get { return m_safetyFire; }
            set { m_safetyFire = value; }
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

        public List<Wall> Walls
        {
            get { return m_walls; }
        }

        public bool MakeShape(Layer layer, Project.UnitOfLength unit)
        {
            #region 기존 공간영역 계산
            //if (MakeCenterLineBoundary() == false)
            //    return false;

            //if (MakeInnerLineBoundary(layer, unit) == false)
            //    return false;

            //m_path = MakeGraphicsPath(m_shapeInnerItems, this, ref m_polygon);
            ////m_path = MakeGraphicsPath(m_shapeInnerItems);

            //if (vNext != null)
            //    polyLine.UpdatePoint(space.Walls.Count, (float)vNext.x, (float)vNext.y);
            #endregion

            SetBoundary(this.BoundaryData.GetBoundary());
            m_path = MakeGraphicsPath(this.BoundaryData.GetBoundary(), this, ref m_polygon);


            if (layer != null)
                layer.AddShape(this);

            return true;
        }

        private bool MakeInnerLineBoundary(Layer layer, Project.UnitOfLength unit)
        {
            m_shapeInnerItems.Clear();

            // m_shapeCenterItems에는 Arc나 EArc가 포함되어 있을수 있으므로 Arc와 EAr를 임시로 직선으로 변환시킨다.
            Polygon polygon = new Polygon();
            Vertex2D vBegin, vMiddle, vEnd;

            int nPathCount = m_shapeCenterItems.Count;

            for (int i=0;i<nPathCount;i++)
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

            for (int i=1;i<=nPathCount;i++)
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

        public void AddWall(Wall wall)
        {
            m_walls.Add(wall);
            wall.AddSpace(this);
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

        public override void Move(double dMoveX, double dMoveY)
        {
            if (m_path != null)
            {
                m_path.Dispose();
                m_path = MakeGraphicsPath(this.BoundaryData.GetBoundary(), this, ref m_polygon, dMoveX, dMoveY);
                //m_path = MakeGraphicsPath(m_shapeInnerItems, this, ref m_polygon, dMoveX, dMoveY);
                //m_path = MakeGraphicsPath(m_shapeInnerItems, dMoveX, dMoveY);
            }

            m_vTL.x = m_vOriginTL.x + dMoveX;
            m_vTL.y = m_vOriginTL.y + dMoveY;
            m_vBR.x = m_vOriginBR.x + dMoveX;
            m_vBR.y = m_vOriginBR.y + dMoveY;
        }

        public override bool HitTest(Vertex2D vertex)
        {
            return m_polygon.HitTest(vertex) != 0;
        }
    }
}
