using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SoilMan.Overlay
{
    public class OverlayPainter
    {
        public event InvalidateControl InvalidateControl;

        public enum DrawingType { NONE, CIRCLE, RECTANGLE, POLYLINE, DELETE };

        private PointF m_ptDown;
        private PointF m_ptCurrent;
        private DXFViewer.DXFControl m_ctrlViewer = null;
        private DrawingType m_drawingType = DrawingType.NONE;

        private OverlayShape m_shapeCurrent = null;

        private Pen m_penLine = new Pen(Color.Green);
        private Brush m_brush = new SolidBrush(Color.FromArgb(30 * 255 / 100, Color.Pink));
        private List<SoilMan.Overlay.OverlayShape> m_overlayShapes = new List<SoilMan.Overlay.OverlayShape>();

        private Dictionary<LandType, AreaNCost> m_dicLandTypeAreas = null;

        public Pen LinePen
        {
            get { return m_penLine; }
        }

        public Brush FillBrush
        {
            get { return m_brush; }
        }

        public DrawingType DrawType
        {
            get { return m_drawingType; }
            set { m_drawingType = value; }
        }

        public Dictionary<LandType, AreaNCost> LandTypeAreas
        {
            get { return m_dicLandTypeAreas; }
            set { m_dicLandTypeAreas = value; }
        }

        public OverlayPainter(DXFViewer.DXFControl ctrlViewer)
        {
            m_ctrlViewer = ctrlViewer;
        }

        public void Invalidate()
        {
            if (InvalidateControl != null)
            {
                InvalidateControl();
            }
        }

        private OverlayShape MakeShape(float x, float y)
        {
            if (m_drawingType == DrawingType.CIRCLE)
            {
                m_shapeCurrent = new OverlayCircle(this);
                ((OverlayCircle)m_shapeCurrent).Center = new UnE.Geometry.Vertex2F(x, y);
            }
            else if (m_drawingType == DrawingType.POLYLINE)
            {
                m_shapeCurrent = new OverlayPolyLine(this);
                ((OverlayPolyLine)m_shapeCurrent).AddPoint(x, y);
            }
            else if (m_drawingType == DrawingType.RECTANGLE)
            {
                m_shapeCurrent = new OverlayRectangle(this);
                ((OverlayRectangle)m_shapeCurrent).Position = new UnE.Geometry.Vertex2F(x, y);
            }
            else
                m_shapeCurrent = null;

            return m_shapeCurrent;
        }

        private void AddCommand(OverlayShape shape)
        {
            Command.CommandAddOverlay cmd = new Command.CommandAddOverlay(m_ctrlViewer, this, shape);
            FormMain.Instance.CommandManager.AddCommand(cmd);
            cmd.Do();
        }

        private void RemoveCommand(OverlayShape shape)
        {
            Command.CommandRemoveOverlay cmd = new Command.CommandRemoveOverlay(m_ctrlViewer, this, shape);
            FormMain.Instance.CommandManager.AddCommand(cmd);
            cmd.Do();

            CalcSelectedAreas();
        }

        private void RemoveCommand(OverlayShape shape, List<OverlayShape> shapeList)
        {
            Command.CommandRemoveOverlay cmd = new Command.CommandRemoveOverlay(m_ctrlViewer, this,shape, shapeList);
            FormMain.Instance.CommandManager.AddCommand(cmd);
            cmd.Do();

            CalcSelectedAreas();
        }

        public void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (m_drawingType == DrawingType.NONE)
                return;

            if (e.Button == MouseButtons.Left)
            {
                m_ptDown = ScreenToGlobal(e.Location);
                m_ptCurrent = ScreenToGlobal(e.Location);

                if (m_shapeCurrent == null)
                {
                    OverlayShape shape = MakeShape(m_ptDown.X, m_ptDown.Y);

                    if (shape != null)
                    {
                        if (!((Control.ModifierKeys & Keys.Control) == Keys.Control))
                        {
                            RemoveAllShapes(shape);
                            //CalcSelectedAreas();
                        }
                        else
                        {
                            AddCommand(shape);
                            
                        }
                        
                        //m_overlayShapes.Add(shape);
                    }
                }
                else
                {
                    if (m_shapeCurrent is OverlayPolyLine)
                    {
                        ((OverlayPolyLine)m_shapeCurrent).AddPoint(m_ptDown.X, m_ptDown.Y);
                    }
                    else
                    {
                        m_shapeCurrent.SetTempPoint(m_ptDown.X, m_ptDown.Y);

                        if (!m_shapeCurrent.IsValid())
                            m_overlayShapes.Remove(m_shapeCurrent);
                        else
                        {
                            // 영역 계산을 새로 해야하므로 m_dicLandTypeAreas를 null로 둔다.
                            m_dicLandTypeAreas = null;
                            
                        }

                        FormMain.Instance.SelectionManager.ClearAllSelection();
                        CalcSelectedAreas();
                        m_shapeCurrent = null;
                    }
                }

                Invalidate();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (m_shapeCurrent != null && m_shapeCurrent is OverlayPolyLine)
                {
                    if (m_shapeCurrent.IsValid())
                    {
                        ((OverlayPolyLine)m_shapeCurrent).IsClosed = true;
                        // 영역 계산을 새로 해야하므로 m_dicLandTypeAreas를 null로 둔다.
                        m_dicLandTypeAreas = null;
                    }
                    else
                        m_overlayShapes.Remove(m_shapeCurrent);

                    m_shapeCurrent = null;
                    Invalidate();
                }
            }
        }

        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_drawingType == DrawingType.NONE || m_shapeCurrent == null)
                return;

            //if (e.Button == MouseButtons.Left)
            {
                m_ptCurrent = ScreenToGlobal(e.Location);
                m_shapeCurrent.SetTempPoint(m_ptCurrent.X, m_ptCurrent.Y);

                Invalidate();
            }
        }

        public void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (m_drawingType != DrawingType.DELETE)
                return;

            if (e.Button == MouseButtons.Left)
            {
                PointF pt = ScreenToGlobal(e.Location);

                foreach (OverlayShape shape in m_overlayShapes)
                {
                    if (shape.HitTest(pt.X, pt.Y))
                    {
                        RemoveCommand(shape);
                        break;
                    }
                }
            }
        }

        public void RemoveAllShapes(OverlayShape shape)
        {
            if (m_overlayShapes.Count == 0)
            {
                AddCommand(shape);
                return;
            }
                       
            List<OverlayShape> cloneList = new List<OverlayShape>(m_overlayShapes);
            RemoveCommand(shape, cloneList);
        }

        public void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Escape || e.KeyCode == Keys.Space)
            {
                if (m_shapeCurrent != null && m_shapeCurrent is OverlayPolyLine)
                {
                    if (m_shapeCurrent.IsValid())
                    {
                        ((OverlayPolyLine)m_shapeCurrent).IsClosed = true;
                        // 영역 계산을 새로 해야하므로 m_dicLandTypeAreas를 null로 둔다.
                        m_dicLandTypeAreas = null;
                    }
                    else
                        m_overlayShapes.Remove(m_shapeCurrent);

                    m_shapeCurrent = null;
                    Invalidate();
                }
            }
        }

        private PointF ScreenToGlobal(Point pt)
        {
            UnE.Geometry.Vertex2D vert = m_ctrlViewer.ScreenToGlobal(pt.X, pt.Y);
            return new PointF((float)vert.x, (float)vert.y);
        }

        public void DrawOverlay(PaintEventArgs e)
        {
            if (!FormMain.Instance.DrawingOverlay)
                return;

            foreach (OverlayShape shape in m_overlayShapes)
            {
                shape.Draw(e.Graphics);
            }
        }

        public void AddOverlayShape(SoilMan.Overlay.OverlayShape shape)
        {
            m_overlayShapes.Add(shape);
        }

        public int GetOverlayShapeCount()
        {
            return m_overlayShapes.Count;
        }

        public SoilMan.Overlay.OverlayShape GetOverlayShape(int nIndex)
        {
            if (nIndex < 0 || nIndex >= GetOverlayShapeCount())
                return null;

            return m_overlayShapes[nIndex];
        }

        public void RemoveAllOverlayShape()
        {
            m_overlayShapes.Clear();
        }

        public void RemoveOverlayShape(OverlayShape shape)
        {
            m_overlayShapes.Remove(shape);
        }

        // 전체 필지 가운데 사용자가 선택한 영역에 존재하는 필지들의 면적을 얻어온다.
        // Return 값 : 필지 타입별 면적
        public Dictionary<LandType, AreaNCost> GetSelectedAreas()
        {
            if (m_dicLandTypeAreas != null)
                return m_dicLandTypeAreas;
            else
                CalcSelectedAreas();

            return m_dicLandTypeAreas;
        }

        public void SetSelectArea()
        {
            if (m_dicLandTypeAreas == null)
                CalcSelectedAreas();
        }

        private void CalcSelectedAreas()
        {
            m_dicLandTypeAreas = new Dictionary<LandType, AreaNCost>();
            List<List<UnE.Geometry.Vertex2F>> polygons = new List<List<UnE.Geometry.Vertex2F>>();

            float fMinX = 0.0f, fMinY = 0.0f, fMaxX = 0.0f, fMaxY = 0.0f;
            int nShapeCount = m_overlayShapes.Count();

            if (nShapeCount < 2)
            {
                if (nShapeCount == 1)
                {
                    List<UnE.Geometry.Vertex2F> polySubject = m_overlayShapes[0].GetBoundaryPolygon(ref fMinX, ref fMinY, ref fMaxX, ref fMaxY);
                    polygons.Add(polySubject);
                    CalcSelectedAreas(polygons, fMinX, fMinY, fMaxX, fMaxY);
                }
            }
            else
            {
                List<ClipperLib.ExVertexPolygonF> result = new List<ClipperLib.ExVertexPolygonF>();
                ClipperLib.Clipper clipper = new ClipperLib.Clipper();

                List<UnE.Geometry.Vertex2F> polySubject = m_overlayShapes[0].GetBoundaryPolygon(ref fMinX, ref fMinY, ref fMaxX, ref fMaxY);
                clipper.AddPolygon(polySubject, ClipperLib.PolyType.ptSubject);

                float minX = 0.0f, minY = 0.0f, maxX = 0.0f, maxY = 0.0f;

                for (int i = 1; i < nShapeCount; i++)
                {
                    List<UnE.Geometry.Vertex2F> polyClip = m_overlayShapes[i].GetBoundaryPolygon(ref minX, ref minY, ref maxX, ref maxY);
                    clipper.AddPolygon(polyClip, ClipperLib.PolyType.ptClip);

                    if (fMinX > minX)
                        fMinX = minX;
                    if (fMaxX < maxX)
                        fMaxX = maxX;
                    if (fMinY > minY)
                        fMinY = minY;
                    if (fMaxY < maxY)
                        fMaxY = maxY;
                }

                if (clipper.Execute(ClipperLib.ClipType.ctUnion, result))
                {
                    foreach (ClipperLib.ExVertexPolygonF resultPolygon in result)
                    {
                        polygons.Add(resultPolygon.outer);
                    }
                }

                CalcSelectedAreas(polygons, fMinX, fMinY, fMaxX, fMaxY);
            }

            FormMain.Instance.DxfControl._Refresh();
        }

        private void CalcSelectedAreas(List<List<UnE.Geometry.Vertex2F>> polygons, float fMinX, float fMinY, float fMaxX, float fMaxY)
        {
            DXFViewer.Layer polygonLayer = FormMain.Instance.ShapeFilePolygonLayer;

            if (polygonLayer == null)
                return;

            Drawing.PolygonList polygonList = null;

            foreach (DXFViewer.Shape shape in polygonLayer.Shapes)
            {
                if (shape is Drawing.PolygonList)
                {
                    polygonList = (Drawing.PolygonList)shape;
                    break;
                }
            }

            if (polygonList == null)
                return;

            double dGeneralArea = 0.0, dFieldArea = 0.0, dRiceFieldArea = 0.0, dMountainArea = 0.0;
            double dGeneralCost = 0.0, dFieldCost = 0.0, dRiceFieldCost = 0.0, dMountainCost = 0.0;

            foreach (List<UnE.Geometry.Vertex2F> polygon in polygons)
            {
                GetArea(polygon, ref dGeneralArea, ref dFieldArea, ref dRiceFieldArea, ref dMountainArea, ref dGeneralCost, ref dFieldCost, ref dRiceFieldCost, ref dMountainCost, polygonList, fMinX, fMinY, fMaxX, fMaxY);
            }

            m_dicLandTypeAreas[LandType.General] = new AreaNCost(dGeneralArea, dGeneralCost);
            m_dicLandTypeAreas[LandType.Field] = new AreaNCost(dFieldArea, dFieldCost);
            m_dicLandTypeAreas[LandType.RiceField] = new AreaNCost(dRiceFieldArea, dRiceFieldCost);
            m_dicLandTypeAreas[LandType.Mountain] = new AreaNCost(dMountainArea, dMountainCost);
        }

        private void GetArea(List<UnE.Geometry.Vertex2F> vertices, ref double dGeneralArea, ref double dFieldArea, ref double dRiceFieldArea, ref double dMountainArea, ref double dGeneralCost, ref double dFieldCost, ref double dRiceFieldCost, ref double dMountainCost, Drawing.PolygonList polygonList, float fMinX, float fMinY, float fMaxX, float fMaxY)
        {
            int nVertexCount = vertices.Count;

            if (nVertexCount < 3)
                return;

            List<QuadNode> nodes = FormMain.Instance.QuadTree.GetNodes(fMinX, fMaxY, fMaxX, fMinY);
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
                    if (dArea > 0.0)
                        FormMain.Instance.SelectionManager.SelectShape(polygon);                    
                    
                    Popup.PolygonInfo info = (Popup.PolygonInfo)polygon.Tag;

                    if (info != null)
                    {
                        double dCost = info.Cost < 0.0 ? 0.0 : info.Cost;

                        if (info.Land == LandType.General)
                        {
                            dGeneralCost += dCost * dArea;
                            dGeneralArea += dArea;
                        }
                        else if (info.Land == LandType.Field)
                        {
                            dFieldCost += dCost * dArea;
                            dFieldArea += dArea;
                        }
                        else if (info.Land == LandType.RiceField)
                        {
                            dRiceFieldCost += dCost * dArea;
                            dRiceFieldArea += dArea;
                        }
                        else if (info.Land == LandType.Mountain)
                        {
                            dMountainCost += dCost * dArea;
                            dMountainArea += dArea;
                        }
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
                Drawing.Polygon.PolygonFx subPolygon = polygon.GetSubPolygon(i);

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

                        if (subPolygon.ClockWise)
                            dArea += fArea;
                        else
                            dArea -= fArea;
                    }

                    /*#region 영역체크
                    if (result.Count > 0)
                        polygon.Selected = true;
                    #endregion*/
                }
            }

            return dArea;
        }
    }

    public class AreaNCost
    {
        // m²
        private double m_dArea = 0.0;
        // 원
        private double m_dCost = 0.0;

        // m²
        public double Area
        {
            get { return m_dArea; }
            set { m_dArea = value; }
        }

        // 원
        public double Cost
        {
            get { return m_dCost; }
            set { m_dCost = value; }
        }

        public AreaNCost()
        {
        }

        public AreaNCost(double dArea, double dCost)
        {
            m_dArea = dArea;
            m_dCost = dCost;
        }
    }
}
