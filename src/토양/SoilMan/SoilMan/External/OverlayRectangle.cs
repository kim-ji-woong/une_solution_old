using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoilMan.Overlay
{
    public class OverlayRectangle : OverlayShape
    {
        private UnE.Geometry.Vertex2F m_vPos = null;
        private float m_fHeight = 0.0f, m_fWidth = 0.0f;

        public UnE.Geometry.Vertex2F Position
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

        public OverlayRectangle(OverlayPainter painter)
            : base(painter)
        {
        }

        public override bool Draw(System.Drawing.Graphics g)
        {
            if (m_fWidth == 0.0 || m_fHeight == 0.0)
                return false;

            float x = m_fWidth < 0.0 ? m_vPos.x + m_fWidth : m_vPos.x;
            float y = m_fHeight < 0.0 ? m_vPos.y + m_fHeight : m_vPos.y;
            float fWidth = m_fWidth > 0.0 ? m_fWidth : -m_fWidth;
            float fHeight = m_fHeight > 0.0 ? m_fHeight : -m_fHeight;

            if (m_isSelected)
                g.FillRectangle(m_painter.FillBrush, x, y, fWidth, fHeight);

            g.DrawRectangle(m_painter.LinePen, x, y, fWidth, fHeight);
            return true;
        }

        // Return 값 : true이면 객체가 완성, false이면 아직 완성되지 않았음
        public override void SetTempPoint(float x, float y)
        {
            m_fWidth = x - m_vPos.x;
            m_fHeight = y - m_vPos.y;
        }

        public override bool IsValid()
        {
            if (m_fWidth == 0.0 || m_fHeight == 0.0)
                return false;

            return true;
        }

        // Shape File과 사각영역이 겹치는 부분의 면적을 얻어온다.
        public override void GetArea(ref double dGeneralArea, ref double dFieldArea, ref double dRiceFieldArea, ref double dMountainArea, DXFViewer.Layer layer)
        {
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

            float minX = m_vPos.x, maxX = m_vPos.x;
            float minY = m_vPos.y, maxY = m_vPos.y;
            List<UnE.Geometry.Vertex2F> vertices = GetBoundaryPolygon(ref minX, ref minY, ref maxX, ref maxY);

            List<QuadNode> nodes = FormMain.Instance.QuadTree.GetNodes(minX, maxY, maxX, minY);
            Dictionary<int, int> dicShapeIndex = new Dictionary<int,int>();

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

            for (int i=0;i<nCount;i++)
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

                        for (int j=0;j<nVertexCount;j++)
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
            List<UnE.Geometry.Vertex2F> polygonTrg = new List<UnE.Geometry.Vertex2F>();

            GetBoundary(out minX, out minY, out maxX, out maxY);

            minX -= (float)vMoved.x;
            maxX -= (float)vMoved.x;
            minY -= (float)vMoved.y;
            maxY -= (float)vMoved.y;

            polygonTrg.Add(new UnE.Geometry.Vertex2F(minX, maxY));
            polygonTrg.Add(new UnE.Geometry.Vertex2F(minX, minY));
            polygonTrg.Add(new UnE.Geometry.Vertex2F(maxX, minY));
            polygonTrg.Add(new UnE.Geometry.Vertex2F(maxX, maxY));

            return polygonTrg;
        }

        private void GetBoundary(out float minX, out float minY, out float maxX, out float maxY)
        {
            float x = m_fWidth < 0.0 ? m_vPos.x + m_fWidth : m_vPos.x;
            float y = m_fHeight < 0.0 ? m_vPos.y + m_fHeight : m_vPos.y;
            float fWidth = m_fWidth > 0.0 ? m_fWidth : -m_fWidth;
            float fHeight = m_fHeight > 0.0 ? m_fHeight : -m_fHeight;

            minX = x;
            maxX = x + fWidth;
            minY = y;
            maxY = y + fHeight;
        }

        public override bool HitTest(float x, float y)
        {
            float left, top, right, bottom;
            GetBoundary(out left, out bottom, out right, out top);

            if (x >= left && x <= right && y >= bottom && y <= top)
                return true;

            return false;
        }

        public override void Move(float x, float y)
        {
            m_vPos.x += x;
            m_vPos.y += y;
        }
    }
}
