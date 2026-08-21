using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilMan.Overlay
{
    public class OverlayCircle : OverlayShape
    {
        private UnE.Geometry.Vertex2F m_vCenter = null;
        private float m_fRadius = 0.0f;

        public UnE.Geometry.Vertex2F Center
        {
            get { return m_vCenter; }
            set { m_vCenter = value; }
        }

        public float Radius
        {
            get { return m_fRadius; }
            set { m_fRadius = value; }
        }

        public OverlayCircle(OverlayPainter painter)
            : base(painter)
        {
        }

        public override bool Draw(System.Drawing.Graphics g)
        {
            if (m_fRadius <= 0.0f)
                return false;

            if (m_isSelected)
                g.FillEllipse(m_painter.FillBrush, m_vCenter.x - m_fRadius, m_vCenter.y - m_fRadius, m_fRadius * 2, m_fRadius * 2);

            g.DrawEllipse(m_painter.LinePen, m_vCenter.x - m_fRadius, m_vCenter.y - m_fRadius, m_fRadius * 2, m_fRadius * 2);
            return true;
        }

        public override void SetTempPoint(float x, float y)
        {
            m_fRadius = m_vCenter.GetDistance(new UnE.Geometry.Vertex2F(x, y));
        }

        public override bool IsValid()
        {
            return m_fRadius > 0.0f;
        }

        public override void GetArea(ref double dGeneralArea, ref double dFieldArea, ref double dRiceFieldArea, ref double dMountainArea, DXFViewer.Layer layer)
        {
            if (!IsValid())
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

            float minX = 0.0f, maxX = 0.0f;
            float minY = 0.0f, maxY = 0.0f;
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

            // 원을 PolyLine 형태로 만든다.
            int nPointCount = 100;
            double delta = UnE.Geometry.Math._2PI() / nPointCount;

            for (int i = 0; i < nPointCount; i++)
            {
                double dAngle = i * delta;
                float x = (float)(m_vCenter.x + System.Math.Cos(dAngle) * m_fRadius);
                float y = (float)(m_vCenter.y + System.Math.Sin(dAngle) * m_fRadius);

                UnE.Geometry.Vertex2F vertex = new UnE.Geometry.Vertex2F(x - (float)vMoved.x, y - (float)vMoved.y);
                vertices.Add(vertex);
            }

            minX = m_vCenter.x - m_fRadius - (float)vMoved.x;
            maxX = m_vCenter.x + m_fRadius - (float)vMoved.x;
            minY = m_vCenter.y - m_fRadius - (float)vMoved.y;
            maxY = m_vCenter.y + m_fRadius - (float)vMoved.y;

            return vertices;
        }

        public override bool HitTest(float x, float y)
        {
            double distance = m_vCenter.GetDistance(new UnE.Geometry.Vertex2F(x, y));
            return m_fRadius >= distance;
        }

        public override void Move(float x, float y)
        {
            m_vCenter.x += x;
            m_vCenter.y += y;
        }
    }
}
