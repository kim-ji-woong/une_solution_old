using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SoilMan.Overlay
{
    public class OverlayPolyLine : OverlayShape
    {
        private bool m_isClosed = false;
        private PointF[] m_arrPoints = null;

        public bool IsClosed
        {
            get { return m_isClosed; }
            set
            {
                m_isClosed = value;

                if (m_isClosed)
                {
                    if (m_arrPoints != null)
                    {
                        int nCount = m_arrPoints.Count();
                        m_arrPoints[nCount - 1].X = m_arrPoints[0].X;
                        m_arrPoints[nCount - 1].Y = m_arrPoints[0].Y;
                    }
                }
            }
        }

        public OverlayPolyLine(OverlayPainter painter)
            : base(painter)
        {
        }

        public override bool Draw(System.Drawing.Graphics g)
        {
            if (m_arrPoints == null)
                return false;

            if (m_isSelected && m_isClosed)
                g.FillPolygon(m_painter.FillBrush, m_arrPoints);

            if (m_isClosed)
                g.DrawPolygon(m_painter.LinePen, m_arrPoints);
            else
                g.DrawLines(m_painter.LinePen, m_arrPoints);

            return true;
        }

        public void AddPoint(float x, float y)
        {
            int nCount = 0;

            if (m_arrPoints == null)
            {
                m_arrPoints = new PointF[2];
                nCount = 2;
            }
            else
            {
                nCount = m_arrPoints.Count();
                PointF[] temp = m_arrPoints;
                m_arrPoints = new PointF[nCount + 1];

                // 배열의 마지막 요소는 Temp Drawing을 위하여 쓰인다.
                for (int i = 0; i < nCount - 1; i++)
                {
                    m_arrPoints[i] = temp[i];
                }

                nCount++;
            }

            m_arrPoints[nCount - 2].X = m_arrPoints[nCount - 1].X = x;
            m_arrPoints[nCount - 2].Y = m_arrPoints[nCount - 1].Y = y;
        }

        // 마지막 요소를 삭제한다.
        public void SubPoint()
        {
            if (m_arrPoints == null)
                return;
            else
            {
                int nCount = m_arrPoints.Count();

                if (nCount == 2)
                {
                    m_arrPoints = null;
                    return;
                }
                else
                {
                    PointF[] temp = m_arrPoints;
                    m_arrPoints = new PointF[nCount - 1];

                    // 배열의 마지막 요소는 Temp Drawing을 위하여 쓰인다.
                    for (int i = 0; i < nCount - 2; i++)
                    {
                        m_arrPoints[i] = temp[i];
                    }

                    m_arrPoints[nCount - 2].X = m_arrPoints[nCount - 3].X;
                    m_arrPoints[nCount - 2].Y = m_arrPoints[nCount - 3].Y;
                }
            }
        }

        // Temp Drawing을 위한 좌표를 입력한다.
        public void SetLastPoint(float x, float y)
        {
            if (m_arrPoints == null)
                return;
            else
            {
                int nCount = m_arrPoints.Count();
                m_arrPoints[nCount - 1].X = x;
                m_arrPoints[nCount - 1].Y = y;
            }
        }

        public int GetPointCount()
        {
            if (m_arrPoints == null)
                return 0;

            // 배열의 마지막 요소는 Temp Drawing을 위하여 쓰인다.
            return m_arrPoints.Count() - 1;
        }

        public UnE.Geometry.Vertex2F GetPoint(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetPointCount())
                return null;

            UnE.Geometry.Vertex2F vertex = new UnE.Geometry.Vertex2F(m_arrPoints[nIndex].X, m_arrPoints[nIndex].Y);
            return vertex;
        }

        // Return 값 : true이면 객체가 완성, false이면 아직 완성되지 않았음
        public override void SetTempPoint(float x, float y)
        {
            SetLastPoint(x, y);
        }

        public override bool IsValid()
        {
            // 최소 3개 이상이어야 하는데, 마지막 요소는 Temp Drawing을 위한 것이므로 4개 이상이어야 한다.
            if (m_arrPoints.Count() >= 4)
                return true;

            return false;
        }

        public override void GetArea(ref double dGeneralArea, ref double dFieldArea, ref double dRiceFieldArea, ref double dMountainArea, DXFViewer.Layer layer)
        {
            if (!m_isClosed || m_arrPoints == null || !IsValid())
                return;

            Drawing.PolygonList polygonList = null;

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape is Drawing.PolygonList)
                {
                    polygonList = (Drawing.PolygonList)shape;
                    break;
                }
            }

            if (polygonList == null)
                return;

            float minX = 0.0f, maxX = 0.0f, minY = 0.0f, maxY = 0.0f;
            List<UnE.Geometry.Vertex2F> vertices = GetBoundaryPolygon(ref minX, ref minY, ref maxX, ref maxY);

            List<QuadNode> nodes = FormMain.Instance.QuadTree.GetNodes(minX, maxY, maxX, minY);
            Dictionary<int, int> dicShapeIndex = new Dictionary<int, int>();

            foreach (QuadNode node in nodes)
            {
                foreach (int nIndex in node.Datas)
                {
                    dicShapeIndex[nIndex] = nIndex;
                }
            }

            foreach (KeyValuePair<int, int> pair in dicShapeIndex)
            {
                Drawing.Polygon polygon = polygonList.GetPolygonFromID(pair.Value);

                if (polygon != null)
                {
                    double dArea = GetArea(polygon, vertices);

                    Popup.PolygonInfo info = (Popup.PolygonInfo)polygon.Tag;

                    if (info != null)
                    {
                        if (info.Land == LandType.General)
                            dGeneralArea += dArea;
                        else if (info.Land == LandType.Field)
                            dFieldArea += dArea;
                        else if (info.Land == LandType.RiceField)
                            dRiceFieldArea += dArea;
                        else if (info.Land == LandType.Mountain)
                            dMountainArea += dArea;
                    }
                }
            }
        }

        // polygon과 겹치는 부분의 영역을 구해온다.
        private double GetArea(Drawing.Polygon polygon, List<UnE.Geometry.Vertex2F> polygonTrg)
        {
            double dArea = 0.0;
            int nCount = polygon.GetSubPolygonCount();

            for (int i = 0; i < nCount; i++)
            {
                UnE.Geometry.PolygonF subPolygon = polygon.GetSubPolygon(i);

                List<UnE.Geometry.Vertex2F> polygonSrc = subPolygon.GetVertexList();

                List<ClipperLib.ExVertexPolygonF> result = new List<ClipperLib.ExVertexPolygonF>();
                ClipperLib.Clipper clipper = new ClipperLib.Clipper();

                clipper.AddPolygon(polygonTrg, ClipperLib.PolyType.ptSubject);
                clipper.AddPolygon(polygonSrc, ClipperLib.PolyType.ptClip);

                if (clipper.Execute(ClipperLib.ClipType.ctIntersection, result))
                {
                    foreach (ClipperLib.ExVertexPolygonF resultPolygon in result)
                    {
                        int nVertexCount = resultPolygon.outer.Count;

                        if (nVertexCount <= 2)
                            continue;

                        UnE.Geometry.PolygonF _polygon = new UnE.Geometry.PolygonF();

                        for (int j = 0; j < nVertexCount; j++)
                        {
                            UnE.Geometry.Vertex2F vertex = resultPolygon.outer[j];
                            _polygon.AddVertex(vertex);
                        }

                        float fArea = _polygon.GetArea();
                        dArea += fArea;
                    }
                }
            }

            return dArea;
        }

        public override List<UnE.Geometry.Vertex2F> GetBoundaryPolygon(ref float minX, ref float minY, ref float maxX, ref float maxY)
        {
            UnE.Geometry.Vertex2D vMoved = FormMain.Instance.DxfControl.MovedVertex;
            List<UnE.Geometry.Vertex2F> vertices = new List<UnE.Geometry.Vertex2F>();

            int nPointCount = m_arrPoints.Count();

            for (int i = 0; i < nPointCount; i++)
            {
                PointF pt = m_arrPoints[i];

                if (i == 0)
                {
                    minX = maxX = pt.X;
                    minY = maxY = pt.Y;
                }
                else
                {
                    if (minX > pt.X)
                        minX = pt.X;
                    if (maxX < pt.X)
                        maxX = pt.X;

                    if (minY > pt.Y)
                        minY = pt.Y;
                    if (maxY < pt.Y)
                        maxY = pt.Y;
                }

                UnE.Geometry.Vertex2F vertex = new UnE.Geometry.Vertex2F(pt.X - (float)vMoved.x, pt.Y - (float)vMoved.y);
                vertices.Add(vertex);
            }

            minX -= (float)vMoved.x;
            maxX -= (float)vMoved.x;
            minY -= (float)vMoved.y;
            maxY -= (float)vMoved.y;

            return vertices;
        }

        public override bool HitTest(float x, float y)
        {
            UnE.Geometry.PolygonF polygon = new UnE.Geometry.PolygonF();

            foreach (PointF pt in m_arrPoints)
            {
                polygon.AddVertex(new UnE.Geometry.Vertex2F(pt.X, pt.Y));
            }

            int nResult = polygon.HitTest(new UnE.Geometry.Vertex2F(x, y));
            return nResult == 0 ? false : true;
        }

        public override void Move(float x, float y)
        {
            int nPointCount = m_arrPoints.Count();

            for (int i=0;i<nPointCount;i++)
            {
                m_arrPoints[i].X += x;
                m_arrPoints[i].Y += y;
            }
        }
    }
}
