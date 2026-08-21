using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;


namespace SoilMan.Drawing
{
    public class Polygon : BoundingShape, IQuadData, System.IComparable
    {
        // Polygon은 여러개의 Sub Polygon으로 이루어져 있으며, 각 Sub Polygon들은 떨어져 있는 Polygon을 표현하기도 하지만
        // Polygon내의 Hole을 의미하기도 한다.
        // Polygon과 Hole은 Polygon을 구성하는 Vertex의 진행방향으로 구분할 수 있는데,
        // Vertex의 진행방향이 시계방향이면 Polygon, 반시계방향이면 Hole이 된다.
        public class PolygonFx : UnE.Geometry.PolygonF
        {
            private bool m_isClockWise = true;

            public bool ClockWise
            {
                get { return m_isClockWise; }
                set { m_isClockWise = value; }
            }
        }

        // HitTest를 위한 변수
        private List<PolygonFx> m_polygons = new List<PolygonFx>();

        //public List<System.Drawing.PointF[]> m_arrVertices = new List<System.Drawing.PointF[]>();
        public List<System.Drawing.Point[]> m_arrVertices = new List<System.Drawing.Point[]>();

        private System.Drawing.SolidBrush m_brush = null;

        private bool m_drawing = true;
        private bool m_checkedHitTest = false;

        public bool Drawing
        {
            get { return m_drawing; }
            set { m_drawing = value; }
        }

        public bool CheckedHitTest
        {
            get { return m_checkedHitTest; }
            set { m_checkedHitTest = value; }
        }

        public Polygon()
        {
            m_brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        }

        public Polygon(List<List<Vertex2F>> arrVertices, double dScale, Vertex2F vCenter)
        {
            m_brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);

            foreach (List<Vertex2F> vertices in arrVertices)
            {
                int nVertexCount = vertices.Count();

                if (nVertexCount == 0)
                    continue;

                //System.Drawing.PointF[] arrPoints = new System.Drawing.PointF[nVertexCount];
                System.Drawing.Point[] arrPoints = new System.Drawing.Point[nVertexCount];
                m_arrVertices.Add(arrPoints);

                PolygonFx polygon = new PolygonFx();
                m_polygons.Add(polygon);

                for (int i = 0; i < nVertexCount; i++)
                {
                    Vertex2F vertex = vertices[i];
                    //Vertex2F vertex2 = ScaleTransfer(vertex.x, vertex.y, dScale, vCenter);

                    arrPoints[i].X = (int)System.Math.Round(vertex.x);
                    arrPoints[i].Y = (int)System.Math.Round(vertex.y);

                    polygon.AddVertex(vertex);
                }

                polygon.ClockWise = polygon.IsClockWise();
            }
        }

        private void GetAddress(string strAddr, int nHyphenIndex, out int nMajorAddr, out int nMinorAddr)
        {
            nMajorAddr = nMinorAddr = 0;

            if (nHyphenIndex >= 0)
            {
                string[] arrTokens = strAddr.Split('-');

                int nTokenCount = arrTokens.Count();

                for (int i=0;i<nTokenCount;i++)
                {
                    string strAddress = arrTokens[i].Trim();

                    if (i == 0)
                        int.TryParse(strAddress, out nMajorAddr);
                    else
                        int.TryParse(strAddress, out nMinorAddr);
                }
            }
            else
            {
                strAddr = strAddr.Trim();
                int.TryParse(strAddr, out nMajorAddr);
            }

            if (nMajorAddr == 0 && nMinorAddr != 0)
            {
                nMajorAddr = nMinorAddr;
                nMinorAddr = 0;
            }
        }

        // strAddress를 마지막 번지와 그 앞 문자열로 분리한다.
        private void ParseAddress(string strAddress, out string str, out int nMajorAddr, out int nMinorAddr)
        {
            nMajorAddr = nMinorAddr = 0;
            str = "";

            strAddress = strAddress.Trim();
            int len = strAddress.Length;

            if (len == 0)
                return;
        
            int nHyphenIndex = -1;
            int nIndex = -1;

            for (int i = len - 1; i >= 0; i--)
            {
                char ch = strAddress.ElementAt(i);

                if (ch < '0' || ch > '9')
                {
                    if (ch == ' ' || ch == '\t')
                        continue;

                    if (ch == '-')
                    {
                        if (nHyphenIndex < 0)
                        {
                            nHyphenIndex = i;
                            continue;
                        }
                    }

                    nIndex = i;
                    break;
                }
            }

            if (nIndex < 0)
            {
                str = "";
                GetAddress(strAddress, nHyphenIndex, out nMajorAddr, out nMinorAddr);
            }
            else
            {
                str = strAddress.Substring(0, nIndex + 1);

                if (nIndex < len - 1)
                {
                    string strData = strAddress.Substring(nIndex + 1);
                    GetAddress(strData, nHyphenIndex, out nMajorAddr, out nMinorAddr);
                }
            }
        }

        /*// strAddress를 마지막 번지와 그 앞 문자열로 분리한다.
        private void ParseAddress(string strAddress, out string str, out int data)
        {
            strAddress = strAddress.Trim();
            int len = strAddress.Length;

            if (len == 0)
            {
                str = "";
                data = 0;
                return;
            }

            int nIndex = -1;

            for (int i=len-1;i>=0;i--)
            {
                char ch = strAddress.ElementAt(i);

                if (ch < '0' || ch > '9')
                {
                    nIndex = i;
                    break;
                }
            }

            if (nIndex < 0)
            {
                str = "";
                data = int.Parse(strAddress);
            }
            else
            {
                str = strAddress.Substring(0, nIndex + 1);

                if (nIndex == len - 1)
                    data = 0;
                else
                {
                    string strData = strAddress.Substring(nIndex + 1);
                    data = int.Parse(strData);
                }
            }
        }*/

        public int CompareTo(object obj)
        {
            Polygon polygon1 = this;
            Polygon polygon2 = (Polygon)obj;

            Popup.PolygonInfo info1 = (Popup.PolygonInfo)polygon1.Tag;
            Popup.PolygonInfo info2 = (Popup.PolygonInfo)polygon2.Tag;

            if (info1 == null && info2 == null)
                return 0;
            else if (info1 == null)
                return -1;
            else if (info2 == null)
                return 1;

            string strAddr1, strAddr2;
            int nMajor1, nMinor1;
            int nMajor2, nMinor2;
            //int nAddr1, nAddr2;

            ParseAddress(info1.Address, out strAddr1, out nMajor1, out nMinor1);
            ParseAddress(info2.Address, out strAddr2, out nMajor2, out nMinor2);

            if (strAddr1 == strAddr2)
            {
                if (nMajor1 == nMajor2)
                {
                    if (nMinor1 == nMinor2)
                        return 0;
                    else
                        return nMinor1 < nMinor2 ? -1 : 1;
                }
                else
                    return nMajor1 < nMajor2 ? -1 : 1;
            }
            /*ParseAddress(info1.Address, out strAddr1, out nAddr1);
            ParseAddress(info2.Address, out strAddr2, out nAddr2);

            if (strAddr1 == strAddr2)
            {
                if (nAddr1 == nAddr2)
                    return 0;
                else
                    return nAddr1 < nAddr2 ? -1 : 1;
            }*/

            return string.Compare(strAddr1, strAddr2);
        }

        public List<List<System.Drawing.Point[]>> m_levelPolygons = new List<List<System.Drawing.Point[]>>();
        public bool mbGenLevelPolygon = false;
        public void GenerateLevelPolygon()
        {
            for (int i = 1; i < 10; i++)
            {
                List<System.Drawing.Point[]> levels2 = new List<System.Drawing.Point[]>();
                foreach (System.Drawing.Point[] vertices in m_arrVertices)
                {
                    List<System.Drawing.Point> arTarget = new List<System.Drawing.Point>(vertices);
                    List<System.Drawing.Point> arResult = NativeGDI.NativeMethods.DouglasPeuckerReduction(arTarget, (10 - i) * 10.0);
                    System.Drawing.Point[] ptLevel = arResult.ToArray();
                    levels2.Add(ptLevel);
                }
                m_levelPolygons.Add(levels2);
            }

            List<System.Drawing.Point[]> levels = new List<System.Drawing.Point[]>();
            foreach (System.Drawing.Point[] vertices in m_arrVertices)
            {
                System.Drawing.Point[] ptOrg = vertices;
                levels.Add(ptOrg);
            }
            m_levelPolygons.Add(levels);

            mbGenLevelPolygon = true;
        }

        public System.Drawing.RectangleF GetBoundaryRectangle()
        {
            System.Drawing.RectangleF rect = new System.Drawing.RectangleF((float)m_dMinX, (float)m_dMinY, (float)(m_dMaxX - m_dMinX), (float)(m_dMaxY - m_dMinY));
            return rect;
        }

        public void AddVertices(List<Vertex2F> vertices, double dScale, Vertex2D vCenter)
        {
            int nVertexCount = vertices.Count();

            if (nVertexCount == 0)
                return;

            System.Drawing.Point[] points = new System.Drawing.Point[nVertexCount];
            PolygonFx polygon = new PolygonFx();

            UnE.Geometry.Vertex2F _vCenter = new Vertex2F((float)vCenter.x, (float)vCenter.y);

            for (int i = 0; i < nVertexCount; i++)
            {
                Vertex2F vertex = vertices[i];
                //Vertex2F vertex2 = ScaleTransfer(vertex.x, vertex.y, dScale, _vCenter);

                points[i].X = (int)System.Math.Round(vertex.x);
                points[i].Y = (int)System.Math.Round(vertex.y);

                polygon.AddVertex(vertex);
            }

            m_arrVertices.Add(points);
            m_polygons.Add(polygon);
            polygon.ClockWise = polygon.IsClockWise();
        }

        public override UnE.Geometry.Vertex2D Position
        {
            get { return new UnE.Geometry.Vertex2D((m_dMinX + m_dMaxX) / 2, (m_dMinY + m_dMaxY) / 2); }
        }

        public override Vertex2D BoundaryTL
        {
            get { return new Vertex2D(m_dMinX, m_dMaxY); }
        }

        public override Vertex2D BoundaryBR
        {
            get { return new Vertex2D(m_dMaxX, m_dMinY); }
        }

        // (x,y)만큼 객체를 옮긴다.
        public override void Move(double x, double y)
        {
            if (mbGenLevelPolygon == true)
            {
                foreach (List<System.Drawing.Point[]> list in m_levelPolygons)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        System.Drawing.Point[] vertices = list[j];
                        int nVertexCount = vertices.Count();
                        for (int i = 0; i < nVertexCount; i++)
                        {
                            vertices[i].X += (int)x;
                            vertices[i].Y += (int)y;
                        }
                    }
                }
            }
            else
            {
                foreach (System.Drawing.Point[] vertices in m_arrVertices)
                {
                    int nVertexCount = vertices.Count();

                    for (int i = 0; i < nVertexCount; i++)
                    {
                        vertices[i].X += (int)x;
                        vertices[i].Y += (int)y;
                    }
                }
            }
        }

        public override DXFViewer.Shape.ShapeType GetShapeType()
        {
            return DXFViewer.Shape.ShapeType.NONE;
        }

        public override DXFViewer.Shape Clone()
        {
            SoilMan.Drawing.Polygon polygon = new SoilMan.Drawing.Polygon();

            foreach (System.Drawing.Point[] vertices in m_arrVertices)
            {
                int nVertexCount = vertices.Count();
                if (nVertexCount == 0)
                    continue;

                System.Drawing.Point[] vertices2 = new System.Drawing.Point[nVertexCount];

                for (int i = 0; i < nVertexCount; i++)
                {
                    vertices2[i].X = vertices[i].X;
                    vertices2[i].Y = vertices[i].Y;
                }

                polygon.m_arrVertices.Add(vertices2);
            }

            foreach (PolygonFx subPolygon in m_polygons)
            {
                int nVertexCount = subPolygon.GetVertexCount();
                if (nVertexCount == 0)
                    continue;

                PolygonFx _polygon = new PolygonFx();

                for (int i = 0; i < nVertexCount; i++)
                {
                    Vertex2F vertex = subPolygon.GetVertex(i);
                    _polygon.AddVertex(new Vertex2F(vertex.x, vertex.y));
                }

                _polygon.ClockWise = subPolygon.ClockWise;
                polygon.m_polygons.Add(_polygon);
            }

            return polygon;
        }

        // Selectable이 false이면 HitTest 검사가 무조건 실패한다.
        public override bool HitTest(double x, double y)
        {
            if (Selectable == false || m_attr == null)
                return false;

            Vertex2F vertex = new Vertex2F((float)x, (float)y);

            bool inHole = false, inPolygon = false;

            foreach (PolygonFx polygon in m_polygons)
            {
                if (polygon.HitTest(vertex) > 0)
                {
                    if (polygon.ClockWise)
                        inPolygon = true;
                    else
                        inHole = true;
                }
            }

            return inPolygon && !inHole;

            /*foreach (UnE.Geometry.PolygonF polygon in m_polygons)
            {
                if (polygon.HitTest(vertex) > 0)
                    return true;
            }

            return false;*/
        }

        public bool HitTestArea(List<Vertex2F> polygonSrc)
        {
            foreach (UnE.Geometry.PolygonF polygon in m_polygons)
            {
                List<ClipperLib.ExVertexPolygonF> result = new List<ClipperLib.ExVertexPolygonF>();
                ClipperLib.Clipper clipper = new ClipperLib.Clipper();

                clipper.AddPolygon(polygonSrc, ClipperLib.PolyType.ptSubject);
                clipper.AddPolygon(polygon.GetVertexList(), ClipperLib.PolyType.ptClip);

                if (clipper.Execute(ClipperLib.ClipType.ctIntersection, result))
                {
                    if (result.Count > 0)
                        return true;
                }
            }

            return false;
        }

        public override bool CheckClipBounds(System.Drawing.Graphics g, Vertex2D vClipTL, Vertex2D vClipBR)
        {
            DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)this.GetOwner();
            return CheckClipBounds(vClipTL - ctrl.MovedVertex, vClipBR - ctrl.MovedVertex, new Vertex2D(m_dMinX, m_dMaxY), new Vertex2D(m_dMaxX, m_dMinY));
        }

        public override bool Draw(System.Drawing.Graphics g, bool bDrawText)
        {
            if (m_attr == null)
                return false;

            double dSize = m_attr.GetPointSize();

            if (dSize <= 0.0)
                return false;

            /*libShapeFile.ShapeInfo shapeInfo = this.ShapeInfo;

            if (shapeInfo == null || this.ID < 0)
                return false;

            string strFieldData = shapeInfo.GetFieldData(this.ID, 1);

            if (strFieldData != "경상북도")
                return false;*/

            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;
            layer.LastDrawingShape = this;

            DrawFill(g, layer);

            System.Drawing.Pen pen = m_lineType.GetPen();
            pen.Color = m_attr.GetLineColor();

            if (Selectable && Selected && m_selectedShowingType != DXFViewer.Shape.SelectedShowingType.NONE)
            {
                if (m_selectedShowingType == DXFViewer.Shape.SelectedShowingType.EDIT_BOX)
                {
                    DXFViewer.LineType lineType = m_pOwner.GetSelectedLineType();
                    System.Drawing.Color penColor = pen.Color;
                    pen = lineType.GetPen();
                    pen.Color = penColor;

                    DrawLines(g, pen, m_attr.GetLineThickness(), layer);

                    m_editBox.Draw(g, (float)m_dMinX, (float)m_dMinY);
                    m_editBox.Draw(g, (float)m_dMaxX, (float)m_dMaxY);
                }
                else if (m_selectedShowingType == DXFViewer.Shape.SelectedShowingType.BRIGHT_EFFECT)
                {
                    float fOldWidth = pen.Width;

                    DrawLines(g, pen, m_attr.GetLineThickness() + 1, layer);

                    pen.Width = fOldWidth;

                    // 밝게 표현하기 위하여 배경색의 보색으로 그린다.
                    DrawLines(g, GetOwner().SelectedBrightPen1, m_attr.GetLineThickness(), layer);
                    // 패턴을 주기 위하여 배경색으로 다시한번 그린다.
                    DrawLines(g, GetOwner().SelectedBrightPen2, m_attr.GetLineThickness(), layer);
                }
            }
            else
                DrawLines(g, pen, m_attr.GetLineThickness(), layer);

            return true;
        }

        public override void PostDraw(System.Drawing.Graphics g)
        {
            ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;

            PostDraw(g, layer, m_lineType);
        }

        public void PostDraw(System.Drawing.Graphics g, ShapeLayer layer, DXFViewer.LineType lineType)
        {
            if (m_attr.GetFillColor() != System.Drawing.Color.Transparent && layer.PathFill != null)
            {
                m_brush.Color = m_attr.GetFillColor();
                g.FillPath(m_brush, layer.PathFill);
                layer.PathFill = null;
            }

            if (layer.PathLine != null)
            {
                System.Drawing.Pen pen = lineType.GetPen();
                BoundingShape.SetPenWidth(pen, g, m_attr.GetLineThickness());

                g.DrawPath(pen, layer.PathLine);
                layer.PathLine = null;
            }

            //GC.Collect();
        }

        public void DrawLines(System.Drawing.Graphics g, System.Drawing.Pen pen, int nLineThick, ShapeLayer layer)
        {
            if (nLineThick <= 0)
                return;

            if (m_attr.GetLineColor() == System.Drawing.Color.Transparent)
                return;

            if (layer.PathLine == null)
            //if (FirstElement)
            {
                layer.PathLine = new System.Drawing.Drawing2D.GraphicsPath();
            }
            //pen.Width = nLineThick;
            //SetPenWidth(pen, g, nLineThick);

            if (layer.PathLine == null)
                return;

            if (mbGenLevelPolygon == true)
            {
                float fScale = System.Math.Abs(g.Transform.Elements[3]) * 1000.0f;
                float fValue = 800.0f;
                int n = (int)System.Math.Floor(fScale) - 1;
                if (n > 9)
                {
                    n = 9;
                }
                if (n < 0)
                {
                    n = 0;
                }
                List<System.Drawing.Point[]> target = m_levelPolygons[n];
                foreach (System.Drawing.Point[] vertices in target)
                {
                    if (vertices.Length >= 3)
                        layer.PathLine.AddPolygon(vertices);
                }
            }
            else
            {
                foreach (System.Drawing.Point[] vertices in m_arrVertices)
                {
                    layer.PathLine.AddPolygon(vertices);
                }
            }


            // PostDraw(...)에서 담당
            // CheckClipBounds() 호출로 인하여 가장 마지막에 그려질 객체가 누구인지 실시간으로 파악할 수 없으므로
            // 모든 Draw() 함수 호출 이후 마지막에 호출된 객체의 PostDraw()를 호출한다.
            /*if (LastElement)
            {
                SetPenWidth(pen, g, nLineThick);

                g.DrawPath(pen, layer.PathLine);
                layer.PathLine = null;

                GC.Collect();
            }*/
        }

        public void DrawFill(System.Drawing.Graphics g, ShapeLayer layer)
        {
            if (m_attr.GetFillColor() == System.Drawing.Color.Transparent)
                return;

            /*m_brush.Color = m_attr.GetFillColor();
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            foreach (System.Drawing.PointF[] vertices in m_arrVertices)
            {
                path.AddPolygon(vertices);
            }

            g.FillPath(m_brush, path);
            GC.Collect();*/

            if (layer.PathFill == null)
            //if (FirstElement)
            {
                layer.PathFill = new System.Drawing.Drawing2D.GraphicsPath();
            }

            foreach (System.Drawing.Point[] vertices in m_arrVertices)
            {

                layer.PathFill.AddPolygon(vertices);
            }

            // PostDraw(...)에서 담당
            // CheckClipBounds() 호출로 인하여 가장 마지막에 그려질 객체가 누구인지 실시간으로 파악할 수 없으므로
            // 모든 Draw() 함수 호출 이후 마지막에 호출된 객체의 PostDraw()를 호출한다.
            /*if (LastElement)
            {
                m_brush.Color = m_attr.GetFillColor();

                g.FillPath(m_brush, layer.PathFill);
                layer.PathFill = null;
                GC.Collect();
            }*/
        }

        public void DrawSelection(System.Drawing.Graphics g, System.Drawing.Color color)
        {
            m_brush.Color = color;

            /*#region 영역체크
            System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Yellow);
            #endregion*/

            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();

            foreach (System.Drawing.Point[] vertices in m_arrVertices)
            {
                path.AddPolygon(vertices);
                //g.FillPolygon(m_brush, vertices);
                
                //#region 영역체크
                //g.DrawPolygon(pen, vertices);
            }
            g.FillPath(m_brush, path);

            //pen.Dispose();
            //    #endregion
        }

        public float GetArea()
        {
            float fArea = 0.0f;

            foreach (PolygonFx polygon in m_polygons)
            {
                if (polygon.ClockWise)
                    fArea += polygon.GetArea();
                else
                    fArea -= polygon.GetArea();
                //fArea += polygon.GetArea();
            }

            return fArea;
        }

        public int GetSubPolygonCount()
        {
            return m_polygons.Count;
        }

        public PolygonFx GetSubPolygon(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetSubPolygonCount())
                return null;

            return m_polygons[nIndex];
        }
    }

    public class PolygonList : PointShape
    {
        private double m_dMinX = 0, m_dMaxX = 0, m_dMinY = 0, m_dMaxY = 0;
        // Key : Shape ID
        private Dictionary<int, Polygon> m_dicPolygons = new Dictionary<int, Polygon>();
        //private List<Polygon> m_polygons = new List<Polygon>();
        private Polygon m_selectedPolygon = null;

        public override Vertex2D Position
        {
            get { return new Vertex2D((m_dMinX + m_dMaxX) / 2, (m_dMinY + m_dMaxY) / 2); }
        }

        public override Vertex2D BoundaryTL
        {
            get { return new Vertex2D(m_dMinX, m_dMaxY); }
        }

        public override Vertex2D BoundaryBR
        {
            get { return new Vertex2D(m_dMaxX, m_dMinY); }
        }

        public Polygon SelectedPolygon
        {
            get { return m_selectedPolygon; }
        }

        // (x,y)만큼 객체를 옮긴다.
        public override void Move(double x, double y)
        {
            foreach (KeyValuePair<int, Polygon> pair in m_dicPolygons)
            {
                pair.Value.Move(x, y);
            }
            /*foreach (Polygon polygon in m_polygons)
            {
                polygon.Move(x, y);
            }*/
        }

        public override DXFViewer.Shape.ShapeType GetShapeType()
        {
            return DXFViewer.Shape.ShapeType.NONE;
        }

        public override DXFViewer.Shape Clone()
        {
            return null;
        }

        // Selectable이 false이면 HitTest 검사가 무조건 실패한다.
        public override bool HitTest(double x, double y)
        {
            foreach (KeyValuePair<int, Polygon> pair in m_dicPolygons)
            {
                if (pair.Value.HitTest(x, y))
                {
                    m_selectedPolygon = pair.Value;
                    return true;
                }
            }
            /*foreach (Polygon polygon in m_polygons)
            {
                if (polygon.HitTest(x, y))
                {
                    m_selectedPolygon = polygon;
                    return true;
                }
            }*/

            m_selectedPolygon = null;
            return false;
        }

        public override bool CheckClipBounds(System.Drawing.Graphics g, Vertex2D vClipTL, Vertex2D vClipBR)
        {
            DXFViewer.DXFControl ctrl = (DXFViewer.DXFControl)this.GetOwner();
            Vertex2D vMovedVertex = ctrl.MovedVertex;
            //return CheckClipBounds(vClipTL - ctrl.MovedVertex, vClipBR - ctrl.MovedVertex, new Vertex2D(m_dMinX, m_dMaxY), new Vertex2D(m_dMaxX, m_dMinY));

            vClipTL = vClipTL - vMovedVertex;
            vClipBR = vClipBR - vMovedVertex;

            System.Drawing.RectangleF rectClip = new System.Drawing.RectangleF((float)vClipTL.x, (float)vClipBR.y, (float)(vClipBR.x - vClipTL.x), (float)(vClipTL.y - vClipBR.y));

            foreach (KeyValuePair<int, Polygon> pair in m_dicPolygons)
            {
                Polygon polygon = pair.Value;
                System.Drawing.RectangleF rectTarget = new System.Drawing.RectangleF((float)polygon.MinX, (float)polygon.MinY, (float)(polygon.MaxX - polygon.MinX), (float)(polygon.MaxY - polygon.MinY));
                polygon.Drawing = rectClip.IntersectsWith(rectTarget);
            }
            /*foreach (Polygon polygon in m_polygons)
            {
                System.Drawing.RectangleF rectTarget = new System.Drawing.RectangleF((float)polygon.MinX, (float)polygon.MinY, (float)(polygon.MaxX - polygon.MinX), (float)(polygon.MaxY - polygon.MinY));
                polygon.Drawing = rectClip.IntersectsWith(rectTarget);
            }*/
            /*foreach (Polygon polygon in m_polygons)
            {
                if (CheckClipBounds(vClipTL - vMovedVertex, vClipBR - vMovedVertex, new Vertex2D(polygon.MinX, polygon.MaxY), new Vertex2D(polygon.MaxX, polygon.MinY)))
                    polygon.Drawing = true;
                else
                    polygon.Drawing = false;
            }*/

            return true;
        }

        public override bool Draw(System.Drawing.Graphics g, bool bDrawText)
        {
            if (m_attr.NoDrawing)
                return true;

            Polygon lastDrawing = null;
            ShapeLayer layer = (ShapeLayer)m_pOwnLayer;

            if (layer.PathFill == null)
            {
                layer.PathFill = new System.Drawing.Drawing2D.GraphicsPath();
            }

            if (layer.PathLine == null)
            {
                layer.PathLine = new System.Drawing.Drawing2D.GraphicsPath();
            }

            //double dFillTime = 0.0;
            //double dLineTime = 0.0;
            List<Polygon> polygons = m_dicPolygons.Values.ToList();           
            //foreach (KeyValuePair<int, Polygon> pair in m_dicPolygons)
            foreach (Polygon polygon in polygons)
            {
                //Polygon polygon = pair.Value;
                if (polygon.Drawing == false)
                    continue;

                IShapeAttrib attr = polygon.Attrib;
                if (attr == null)
                    continue;

                double dSize = attr.GetPointSize();

                if (dSize <= 0.0)
                    continue;

                //ShapeLayer layer = (ShapeLayer)this.m_pOwnLayer;
                //layer.LastDrawingShape = this;
                lastDrawing = polygon;

                //DateTime dtFillBegin = DateTime.Now;
                #region DrawFill
                if (attr.GetFillColor() != System.Drawing.Color.Transparent)
                {
                    foreach (System.Drawing.Point[] vertices in polygon.m_arrVertices)
                    {
                        layer.PathFill.AddPolygon(vertices);
                    }
                }
                //polygon.DrawFill(g, layer);
                #endregion
                //dFillTime += (DateTime.Now - dtFillBegin).TotalSeconds;

                System.Drawing.Pen pen = m_lineType.GetPen();
                pen.Color = attr.GetLineColor();

                //DateTime dtLineBegin = DateTime.Now;
                #region DrawLines
                int nLineThick = attr.GetLineThickness();

                if (nLineThick <= 0)
                    continue;

                if (attr.GetLineColor() == System.Drawing.Color.Transparent)
                    continue;

                
                {
                    foreach (System.Drawing.Point[] vertices in polygon.m_arrVertices)
                    {
                        layer.PathLine.AddPolygon(vertices);
                    }
                }
                /*if (Selectable && Selected && m_selectedShowingType != DXFViewer.Shape.SelectedShowingType.NONE)
                {
                    if (m_selectedShowingType == DXFViewer.Shape.SelectedShowingType.EDIT_BOX)
                    {
                        DXFViewer.LineType lineType = m_pOwner.GetSelectedLineType();
                        System.Drawing.Color penColor = pen.Color;
                        pen = lineType.GetPen();
                        pen.Color = penColor;

                        polygon.DrawLines(g, pen, attr.GetLineThickness(), layer);

                        m_editBox.Draw(g, (float)m_dMinX, (float)m_dMinY);
                        m_editBox.Draw(g, (float)m_dMaxX, (float)m_dMaxY);
                    }
                    else if (m_selectedShowingType == DXFViewer.Shape.SelectedShowingType.BRIGHT_EFFECT)
                    {
                        float fOldWidth = pen.Width;

                        polygon.DrawLines(g, pen, attr.GetLineThickness() + 1, layer);

                        pen.Width = fOldWidth;

                        // 밝게 표현하기 위하여 배경색의 보색으로 그린다.
                        polygon.DrawLines(g, GetOwner().SelectedBrightPen1, attr.GetLineThickness(), layer);
                        // 패턴을 주기 위하여 배경색으로 다시한번 그린다.
                        polygon.DrawLines(g, GetOwner().SelectedBrightPen2, attr.GetLineThickness(), layer);
                    }
                }
                else
                    polygon.DrawLines(g, pen, attr.GetLineThickness(), layer);*/
                #endregion
                //dLineTime += (DateTime.Now - dtLineBegin).TotalSeconds;
            }

            //DateTime dtPostDrawing = DateTime.Now;
            if (lastDrawing != null)
                lastDrawing.PostDraw(g, layer, m_lineType);
            //double dPostDrawingTime = (DateTime.Now - dtPostDrawing).TotalSeconds;

            //System.Diagnostics.Trace.WriteLine(string.Format("Fill Time : {0:F2}, Line Time : {1:F2}, Post Drawing Time : {2:F2}", dFillTime, dLineTime, dPostDrawingTime));

            return true;
        }

        public override void PostDraw(System.Drawing.Graphics g)
        {
        }

        public void AddPolygon(Polygon polygon)
        {
            if (m_dicPolygons.Count == 0)
            //if (m_polygons.Count == 0)
            {
                m_dMinX = polygon.MinX;
                m_dMaxX = polygon.MaxX;
                m_dMinY = polygon.MinY;
                m_dMaxY = polygon.MaxY;
            }
            else
            {
                if (m_dMinX > polygon.MinX)
                    m_dMinX = polygon.MinX;
                if (m_dMaxX < polygon.MaxX)
                    m_dMaxX = polygon.MaxX;
                if (m_dMinY > polygon.MinY)
                    m_dMinY = polygon.MinY;
                if (m_dMaxY < polygon.MaxY)
                    m_dMaxY = polygon.MaxY;
            }

            m_dicPolygons[polygon.ID] = polygon;
            polygon.SetLayer(m_pOwnLayer);
        }

        // 성능 문제로 사용하지 말자 . skkim 2015-03-24
        //public int GetPolygonCount()
        //{
        //    return m_dicPolygons.Count;
        //}

        //public Polygon GetPolygon(int nIndex)
        //{
        //    if (nIndex < 0 || nIndex >= GetPolygonCount())
        //        return null;           
        //    return m_dicPolygons.ElementAt(nIndex).Value;
        //    //return m_polygons[nIndex];
        //}

        public Polygon GetPolygonFromID(int nShapeID)
        {
            Polygon polygon;
            
            if (m_dicPolygons.TryGetValue(nShapeID, out polygon))
                return polygon;

            return null;
        }

        public void RemovePolygons(List<Polygon> polygons)
        {
            foreach (Polygon polygon in polygons)
            {
                m_dicPolygons.Remove(polygon.ID);
                if (m_selectedPolygon == polygon)
                    m_selectedPolygon = null;
            }

            List<Polygon> polyList = m_dicPolygons.Values.ToList();
            bool bFisrt = true;
            foreach (Polygon polygon in polyList)           
            {
                if (bFisrt == true)
                {
                    bFisrt = false;
                    m_dMinX = polygon.MinX;
                    m_dMaxX = polygon.MaxX;
                    m_dMinY = polygon.MinY;
                    m_dMaxY = polygon.MaxY;
                }
                else
                {
                    if (m_dMinX > polygon.MinX)
                        m_dMinX = polygon.MinX;
                    if (m_dMaxX < polygon.MaxX)
                        m_dMaxX = polygon.MaxX;
                    if (m_dMinY > polygon.MinY)
                        m_dMinY = polygon.MinY;
                    if (m_dMaxY < polygon.MaxY)
                        m_dMaxY = polygon.MaxY;
                }
            }
        }

        public void AddPolygons(List<Polygon> polygons)
        {
            foreach (Polygon polygon in polygons)
            {
                m_dicPolygons[polygon.ID] = polygon;
            }
            
            List<Polygon> polyList = m_dicPolygons.Values.ToList();
            bool bFisrt = true;
            foreach(Polygon polygon in polyList)
            {
                if (bFisrt == true)
                {
                    m_dMinX = polygon.MinX;
                    m_dMaxX = polygon.MaxX;
                    m_dMinY = polygon.MinY;
                    m_dMaxY = polygon.MaxY;
                }
                else
                {
                    if (m_dMinX > polygon.MinX)
                        m_dMinX = polygon.MinX;
                    if (m_dMaxX < polygon.MaxX)
                        m_dMaxX = polygon.MaxX;
                    if (m_dMinY > polygon.MinY)
                        m_dMinY = polygon.MinY;
                    if (m_dMaxY < polygon.MaxY)
                        m_dMaxY = polygon.MaxY;
                }               
            }
        }

        public List<Polygon> GetPolygons(Dictionary<int, Drawing.Polygon> dicExcept)
        {
            if (dicExcept == null)
            {
                return m_dicPolygons.Values.ToList();                
            }
            //else
            //{
                List<Polygon> polygons = new List<Polygon>();
                foreach (KeyValuePair<int, Polygon> pair in m_dicPolygons)
                {
                    if (!dicExcept.ContainsKey(pair.Key))
                        polygons.Add(pair.Value);
                }

                return polygons;
            //}
            //return null;
        }
    }
}
