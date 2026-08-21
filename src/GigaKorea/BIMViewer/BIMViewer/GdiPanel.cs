using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using UnE.Geometry;

namespace BIMViewer
{
    using BIMViewer.BIM;
    using Shapes;

    public partial class GdiPanel : Panel, IPainter, DXFViewer.IPainter
    {
        // Global 좌표를 화면좌표로 바꾸는 Matrix
        private Matrix m_currentMatrix = null;
		// 화면좌표를 Global 좌표로 바꾸는 Matrix
		private Matrix m_currentInverseMatrix = null;

        private Vertex2D m_vViewportTL = new Vertex2D(0, 0);
        private Vertex2D m_vViewportBL = new Vertex2D(0, 0);
        private Vertex2D m_vViewportBR = new Vertex2D(0, 0);
        private float m_f11, m_f12, m_f21, m_f22, m_fDx, m_fDy;
        private double m_dViewportWeight = 1.0;

        private bool m_initSize = false;

        private bool m_isPanning = false;
        private Point m_ptPanningOrigin;
        private int m_nMoveX = 0, m_nMoveY = 0;
        private Vertex2D m_vOriginCenter = new Vertex2D();

        private bool m_isDXFPanning = false;
        private Point m_ptDXFPanningOrigin;
        private int m_nDXFMoveX = 0, m_nDXFMoveY = 0;
        private Vertex2D m_vDXFOrigin = new Vertex2D();

        private List<Layer> m_layers = null;
        private List<DXFLayer> m_dxfLayers = null;
        
        private Layer m_poiLayer = null;
        private POI m_selectedPOI = null;
        private Wire m_selectedWire = null;
        private Shape m_selectedShape = null;

        private IGDIOwner m_owner = null;
        private bool m_resizeReshape = true;

        // Pixel 단위
        protected int m_nSnapPixel = 5;
        // Global 단위
        protected VariousData<double> m_pixelDistance = null;

        // 공간 영역과 경계구역 클릭 시 구별하기 위한 변수
        private Shape m_hitAlertAreaShape = null;
        private Shape m_hitSpaceShape = null;

        private bool m_useOrthoSnap = false;
        private bool m_useObjectSnap = false;
        private Vertex2D m_vLastWirePosition = null;
        private const int ObjectSnapDistance = 10;

        // 직교 스냅
        public bool UseOrthoSnap
        {
            get { return m_useOrthoSnap; }
            set { m_useOrthoSnap = value; }
        }

        // 객체 스냅
        public bool UseObjectSnap
        {
            get { return m_useObjectSnap; }
            set { m_useObjectSnap = value; }
        }

        public List<Layer> Layers
        {
            get { return m_layers; }
            set
            {
                m_layers = value;

                if (m_layers != null)
                {
                    m_poiLayer = GetPOILayer();

                    foreach (Layer layer in m_layers)
                    {
                        layer.Painter = this;
                    }
                }
                else
                    m_poiLayer = null;
            }
        }

        public List<DXFLayer> DXFLayers
        {
            get { return m_dxfLayers; }
            set { m_dxfLayers = value; }
        }

        private BIM.Project m_project = null;
        public BIM.Project Project
        {
            get { return m_project; }
            set { m_project = value; }
        }

        private BIM.Level m_level = null;
        public BIM.Level Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        public VariousData<double> SnapDistance
        {
            get { return m_pixelDistance; }
        }

        public GdiPanel()
        {
            InitializeComponent();
            InitDXF();

            this.DoubleBuffered = true;
        }

        public void SetOwner(IGDIOwner owner)
        {
            m_owner = owner;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.ResetTransform();
            g.Transform = m_currentMatrix;

            if (m_dxfLayers != null)
            {
                foreach (DXFLayer layer in m_dxfLayers)
                {
                    layer.Draw(g, true);
                }
            }

            if (m_layers != null)
            {                
                foreach (Layer layer in m_layers)
                {
                    if (layer.IgnoreScale)
                    {
                        //g.ResetTransform();

                    }
                    //else
                    {
                        if (layer is WireLayer)
                        {
                            // 연결된 POI Visible 상태에 따라 보여줄지 말지 결정함
                            foreach (Shape item in layer.Shapes)
                            {
                                if (item is Wire)
                                {
                                    Wire wire = (Wire)item;

                                    POI beginPOI = FindPOI(wire.BeginPOI);
                                    POI endPOI = FindPOI(wire.EndPOI);
                                    if (beginPOI == null || endPOI == null)
                                        continue;

                                    if (beginPOI.POIVisible && endPOI.POIVisible)
                                    {
                                        wire.Visible = true;
                                        wire.POIIcon.POIVisible = true;
                                    }
                                    else
                                    {
                                        wire.Visible = false;
                                        wire.POIIcon.POIVisible = false;
                                    }
                                }
                            }
                        }

                        layer.Render(g, m_vViewportTL, m_vViewportBL, m_vViewportBR);
                    }
                }
            }

            if (m_makingWires)
            {
                if (m_makeWires.Count == 0)
                    return;

                double x = 0.0, y = 0.0;

                if (m_owner != null)
                    m_owner.GetMove(out x, out y);

                Vertex2D beginPT = m_makeWires[m_makeWires.Count - 1].targetVertex2D;
                Vertex2D endPT = new Vertex2D(m_lastMousePT);

                // 직교스냅이 켜져있다면...
                if (m_useOrthoSnap)
                {
                    double beginX = beginPT.x + x, beginY = beginPT.y + y;
                    double xMove = beginX - endPT.x < 0 ? endPT.x - beginX : beginX - endPT.x;
                    double yMove = beginY - endPT.y < 0 ? endPT.y - beginY : beginY - endPT.y;

                    if (xMove >= yMove)
                        endPT.y = beginY;
                    else
                        endPT.x = beginX;

                    if (m_useObjectSnap)
                    {
                        Vertex2D vTarget;
                        Layer poiLayer = GetPOILayer();
                        POI nearestPOI = GetNearestPOI(new Vertex2D(beginX, beginY), endPT, x, y, poiLayer, m_makeWires[m_makeWires.Count - 1].TargetPOI, out vTarget);

                        if (nearestPOI != null)
                        {
                            DrawObjectSnapLine(vTarget, nearestPOI.Position, x, y, g);
                            endPT = vTarget;
                        }
                    }

                    m_vLastWirePosition = endPT;
                }

                List<PointF> drawPaths = new List<PointF>();
                foreach (MakeWire wire in m_makeWires)
                {
                    Vertex2D targetPT = null;
                    if (wire.TargetPOI == null)
                        targetPT = wire.targetVertex2D;
                    else
                        targetPT = wire.TargetPOI.Position;

                    drawPaths.Add(new PointF((float)(targetPT.x + x), (float)(targetPT.y + y)));
                }
                drawPaths.Add(new PointF((float)endPT.x, (float)endPT.y));

                g.DrawLines(m_drawWirePen, drawPaths.ToArray());
            }

            //base.OnPaint(e);
        }

        private void DrawObjectSnapLine(Vertex2D v1, Vertex2D v2, double xMove, double yMove, Graphics g)
        {
            Pen pen = new Pen(Color.White);
            pen.DashStyle = DashStyle.Dot;
            g.DrawLine(pen, (float)v1.x, (float)v1.y, (float)(v2.x + xMove), (float)(v2.y + yMove));
        }

        // 직교모드에서 vLineBegin과 vLineEnd를 잇는 직선(선분)과 가장 가까운(직교좌표 상으로) POI를 얻어온다.
        private POI GetNearestPOI(Vertex2D vLineBegin, Vertex2D vLineEnd, double xMove, double yMove, Layer poiLayer, POI exceptPOI, out Vertex2D vTarget)
        {
            vTarget = null;

            if (poiLayer == null || exceptPOI == null)
                return null;

            Vertex2D vScreenTL = ScreenToGlobal(0, 0);
            Vertex2D vScreenBR = ScreenToGlobal(this.Size.Width, this.Size.Height);

            POI nearestPOI = null;
            double len = 0;
            Vertex2D vNear = null;

            foreach (POI poi in poiLayer.Shapes)
            {
                if (poi == exceptPOI)
                    continue;

                // 화면에 보이는 POI들만 대상으로 한다.
                if ((poi.Position.x + xMove) < vScreenTL.x || (poi.Position.x + xMove) > vScreenBR.x ||
                    (poi.Position.y + yMove) < vScreenBR.y || (poi.Position.y + yMove) > vScreenTL.y)
                    continue;

                Vertex2D vPoi = new Vertex2D(poi.Position.x + xMove, poi.Position.y + yMove);
                Vertex2D vTemp = UnE.Geometry.Math.GetNearestVertex(vPoi, vLineBegin, vLineEnd, true);

                double length = vLineEnd.GetDistance(vTemp);

                if (nearestPOI == null || len > length)
                {
                    nearestPOI = poi;
                    len = length;
                    vNear = vTemp;
                }
            }

            if (nearestPOI != null)
            {
                Point pt1 = GlobalToScreen(vLineEnd);
                Point pt2 = GlobalToScreen(vNear);

                int distance = pt1.X == pt2.X ? pt1.Y - pt2.Y : pt1.X - pt2.X;

                if (distance < 0)
                    distance = -distance;

                if (distance <= ObjectSnapDistance)
                {
                    vTarget = vNear;
                    return nearestPOI;
                }
            }

            return null;
        }

        private Pen m_drawWirePen = new Pen(Color.FromArgb(217, 104, 174));

        private POI FindPOI(int poiID)
        {
            foreach (POI item in m_poiLayer.Shapes)
            {
                if (item.ID == poiID)
                    return item;
            }
            return null;
        }

        private void Reshape(int nWidth, int nHeight)
        {
            if (nWidth <= 0 || nHeight <= 0)
                return;

            float m11 = (float)((m_vViewportBR.x - m_vViewportBL.x) / nWidth);
            float m21 = (float)((m_vViewportBL.x - m_vViewportTL.x) / nHeight);
            float dx = (float)m_vViewportTL.x;
            float m12 = (float)((m_vViewportBR.y - m_vViewportBL.y) / nWidth);
            float m22 = (float)((m_vViewportBL.y - m_vViewportTL.y) / nHeight);
            float dy = (float)m_vViewportTL.y;

            m_currentInverseMatrix = new Matrix(m11, m12, m21, m22, dx, dy);
            m_currentMatrix = m_currentInverseMatrix.Clone();

            try
            {
                m_currentMatrix.Invert();
            }
            catch (System.ArgumentException)
            {
                m_currentMatrix = new Matrix(1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f);
            }
        }

        private void GdiPanel_Resize(object sender, EventArgs e)
        {
            if (m_resizeReshape == false)
                return;

            int nWidth = this.Size.Width;
            int nHeight = this.Size.Height;

            if (m_initSize == false)
            {
                m_vViewportTL.x = m_vViewportBL.x = 0.0;
                m_vViewportTL.y = nHeight;
                m_vViewportBL.y = 0;
                m_vViewportBR.x = nWidth;
                m_vViewportBR.y = m_vViewportBL.y;

                m_dViewportWeight = 1.0;
                m_initSize = true;
            }

            Reshape(nWidth, nHeight);
        }

        public override void Refresh()
        {
            m_resizeReshape = true;
            SetViewportCenter(this.GetViewportCenter());
            base.Refresh();
        }

        public Vertex2D GetViewportCenter()
        {
            return (m_vViewportTL + m_vViewportBR) / 2;
        }

        public double GetViewportWeight()
        {
            return m_dViewportWeight;
        }

        private void SetViewportWeight(double dWeight)
        {
            if (dWeight <= UnE.Geometry.Math.HALF_TOLERANCE())
                return;

            m_dViewportWeight = dWeight;

            if (this.Size.Width > 0 && this.Size.Height > 0)
            {
                Vertex2D vCenter = GetViewportCenter();
                double x = this.Size.Width / dWeight / 2;
                double y = this.Size.Height / dWeight / 2;

                m_vViewportTL.x = vCenter.x - x;
                m_vViewportTL.y = vCenter.y + y;
                m_vViewportBL.x = vCenter.x - x;
                m_vViewportBL.y = vCenter.y - y;
                m_vViewportBR.x = vCenter.x + x;
                m_vViewportBR.y = vCenter.y - y;

                Reshape(this.Size.Width, this.Size.Height);
            }
        }

        public void SetViewportCenter(Vertex2D vCenter)
        {
            double x = this.Size.Width / m_dViewportWeight / 2;
            double y = this.Size.Height / m_dViewportWeight / 2;

            m_vViewportTL.x = vCenter.x - x;
            m_vViewportTL.y = vCenter.y + y;
            m_vViewportBL.x = vCenter.x - x;
            m_vViewportBL.y = vCenter.y - y;
            m_vViewportBR.x = vCenter.x + x;
            m_vViewportBR.y = vCenter.y - y;

            Reshape(this.Size.Width, this.Size.Height);
        }

        private POI SelectPOI(int x, int y)
        {
            if (m_poiLayer != null)
            {
                Vertex2D v1 = ScreenToGlobal(0, 0);
                Vertex2D v2 = ScreenToGlobal(m_nSnapPixel, 0);
                m_pixelDistance = new VariousData<double>(v1.GetDistance(v2));

                Vertex2D vPos = ScreenToGlobal(x, y);

                foreach (Shape shape in m_poiLayer.Shapes)
                {
                    if (shape is POI)
                    {
                        POI poi = (POI)shape;

                        if (poi.HitTest(vPos))
                        {
                            m_selectedPOI = poi;
                            return m_selectedPOI;
                        }
                    }
                }
            }

            m_selectedPOI = null;
            return null;
        }

        private Wire SelectWire(int x, int y)
        {
            if (m_selectedWire != null)
            {
                m_selectedWire.Selected = false;
                m_selectedWire.RectEditVertexVisible = false;
                Refresh();
            }

            Layer layer = GetWireLayer();
            if (layer == null)
                return null;
            
            Vertex2D vPos = ScreenToGlobal(x, y);

            foreach (Shape shape in layer.Shapes)
            {
                if (shape is Wire)
                {
                    Wire wire = (Wire)shape;

                    if (wire.HitTest(vPos))
                    {
                        m_selectedWire = wire;
                        return m_selectedWire;
                    }
                }
            }

            m_selectedWire = null;
            return null;
        }

        private void GdiPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                m_isPanning = true;

                m_ptPanningOrigin.X = e.X;
                m_ptPanningOrigin.Y = e.Y;
                m_nMoveX = m_nMoveY = 0;
                m_vOriginCenter = GetViewportCenter();
            }
            else if (e.Button == MouseButtons.Right)
            {
                m_isDXFPanning = true;

                m_ptDXFPanningOrigin.X = e.X;
                m_ptDXFPanningOrigin.Y = e.Y;
                m_nDXFMoveX = m_nDXFMoveY = 0;
                m_vDXFOrigin = ScreenToGlobal(m_nDXFMoveX, m_nDXFMoveY);
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (m_owner != null && m_owner.IsPOIDeleteMode() == false && m_owner.IsPOIMoveMode())
                    SelectPOI(e.X, e.Y);

                // Rine Move 기능 제거로 주석처리
                //else if (m_owner != null && m_owner.IsWireDeleteMode() == false && m_owner.IsWireMoveMode())
                //{
                //    if (m_selectedWire == null)
                //        SelectWire(e.X, e.Y);
                //    else
                //    {
                //        double x = 0.0, y = 0.0;

                //        if (m_owner != null)
                //            m_owner.GetMove(out x, out y);

                //        Vertex2D vPos = ScreenToGlobal(e.X, e.Y);
                //        vPos.x -= x;
                //        vPos.y -= y;
                //        int index = m_selectedWire.GetRectVertex(vPos);
                //        if (index == -1)
                //        {
                //            m_selectedWire.Selected = false;
                //            m_selectedWire.RectEditVertexVisible = false;
                //            m_selectedWire = null;
                //            Refresh();
                //        }
                //        else
                //        {
                //            m_nMoveWireIndex = index;
                //        }
                //    }                    
                //}
                else if (m_owner != null && (m_owner.IsPOIDoneMode() || m_owner.IsWireDoneMode()))
                {
                    m_selectedPOI = null;
                    m_selectedWire = null;
                }
            }

            if (m_owner != null)
                m_owner.FocusView();
        }

        private int m_nMoveWireIndex = -1; // Line Mode 모드에서 이동할 Line Positions Index
        
        private void GdiPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                m_isPanning = false;
                m_nMoveX = m_nMoveY = 0;
                Refresh();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (m_owner != null && m_owner.IsWireAddMode())
                {
                    if (m_makeWires.Count > 0)
                    {
                        if (m_makeWires[m_makeWires.Count - 1].TargetPOI == null || m_makeWires.Count == 1)
                        {
                            DialogResult result = MessageBox.Show("Line을 생성할 마지막 POI를 선택하세요.\n 아니면 Line 그리기를 중단할까요 ?", "", MessageBoxButtons.YesNo);
                            if (result == DialogResult.Yes)
                            {
                                m_makingWires = false;
                                m_makeWires.Clear();
                            }
                            return;
                        }

                        SaveDBWire();
                    }
                }

                m_isDXFPanning = false;
                m_nDXFMoveX = m_nDXFMoveY = 0;

                m_owner.SetOriginDXF();
                Refresh();
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (m_selectedPOI != null)
                {
                    if (m_owner != null)
                    {
                        m_owner.UpdatePOI(m_selectedPOI, false);
                        List<Shape> updateWires = GetWires(m_selectedPOI.ID);
                        foreach (Shape item in updateWires)
                        {
                            Wire wire = (Wire)item;
                            m_owner.UpdateWire(wire, false);
                        }
                    }
                    m_selectedPOI = null;
                }
                else if (m_owner != null && m_owner.IsPOIAddMode())
                {
                    AddPOI(e.Location);
                }
                else if (m_owner != null && m_owner.IsPOIDeleteMode())
                {
                    POI poi = SelectPOI(e.X, e.Y);

                    if (poi != null)
                    {
                        if (MessageBox.Show("선택한 POI를 삭제하시겠습니까?", "확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            // 연결된 Wire 있으면 삭제하기
                            DeleteWire();
                            
                            m_owner.UpdatePOI(m_selectedPOI, true);                            
                            
                            m_selectedPOI.Layer.RemoveShape(m_selectedPOI);                            
                        }

                        m_selectedPOI = null;
                        Refresh();
                    }
                }
                else if (m_owner != null && m_owner.IsWireAddMode())
                {
                    Shape shape = SelectShape(e.X, e.Y);
                    
                    POI selectedPoi = null;
                    if (shape is POI)                    
                        selectedPoi = (POI)shape;

                    AddWire(e.Location, selectedPoi);
                }
                else if (m_owner != null && m_owner.IsWireDeleteMode())
                {
                    Wire wire = SelectWire(e.X, e.Y);
                    if (wire != null)
                    {
                        if (MessageBox.Show("선택한 Line를 삭제하시겠습니까?", "확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            if (m_owner.UpdateWire(m_selectedWire, true))
                            {
                                if (m_selectedWire.POIIcon != null)
                                    m_selectedWire.Layer.RemoveShape(m_selectedWire.POIIcon);

                                m_selectedWire.Layer.RemoveShape(m_selectedWire);
                                Refresh();
                            }
                        }
                    }
                }
                // Rine Move 기능 제거로 주석처리
                //else if (m_owner != null && m_owner.IsWireMoveMode())
                //{
                //    if (m_selectedWire != null)
                //    {
                //        if (m_nMoveWireIndex > 0)
                //        {
                //            m_nMoveWireIndex = -1;
                //            m_owner.UpdateWire(m_selectedWire, false);
                //        }
                //        else
                //        {
                //            m_selectedWire.Selected = true;
                //            m_selectedWire.RectEditVertexVisible = true;
                //            m_selectedWire.SetRectVertex();                            
                //        }

                //        Refresh();
                //    }
                //}
                else if (m_owner != null && m_owner.IsPropertyMode())
                {
                    Shape shape = SelectShape(e.X, e.Y);

                    if (m_owner != null)
                        m_owner.OnSelectShape(shape);

                    if (shape == null)
                    {
                        if (m_selectedShape != null)
                        {
                            m_selectedShape.Selected = false;
                            m_selectedShape = null;
                            Refresh();
                        }
                    }
                    else
                    {
                        if (m_selectedShape != shape)
                        {
                            if (m_selectedShape != null)
                                m_selectedShape.Selected = false;

                            shape.Selected = true;
                            m_selectedShape = shape;
                            Refresh();
                        }
                    }
                }
                else
                {
                    ClearSelected();
                }
            }
        }

        private void ClearSelected()
        {
            if (m_selectedShape != null)
            {
                m_selectedShape.Selected = false;
                m_selectedShape = null;
                Refresh();
            }

            if (m_selectedWire != null)
            {
                m_selectedWire.Selected = false;
                m_selectedWire.RectEditVertexVisible = false;
                m_nMoveWireIndex = -1;
                Refresh();
            }

            if (m_selectedPOI != null)
            {
                m_selectedPOI.Selected = false;
                m_selectedPOI = null;
            }

            m_selectedWire = null;
        }

        private Shape SelectShape(int x, int y)
        {
            Vertex2D v1 = ScreenToGlobal(0, 0);
            Vertex2D v2 = ScreenToGlobal(m_nSnapPixel, 0);
            m_pixelDistance = new VariousData<double>(v1.GetDistance(v2));

            Vertex2D vPos = ScreenToGlobal(x, y);

            int nLayerCount = m_layers.Count;

            // 건물 외부 클릭시 구별 변수 
            bool bChk = false;

            // 그리기 우선순위가 높은것부터 처리하기 위하여 역순으로 진행한다.
            for (int i=nLayerCount-1;i>=0;i--)
            //foreach (Layer layer in m_layers)
            {
                Layer layer = m_layers[i];

                foreach (Shape shape in layer.Shapes)
                {
                    if (shape is Wire)
                        continue;

                    if (shape.HitTest(vPos))
                    {
                        bChk = true;

                        // 첫번째 클릭시 공간영역 선택, 두번째 클릭시 경계구역 선택
                        if (shape.Layer.Name == "AlertArea" && m_hitAlertAreaShape != shape)
                        {
                            m_hitAlertAreaShape = shape;
                            continue;
                        }
                        else if (shape.Layer.Name == "Space" && m_hitSpaceShape != shape)
                        {
                            m_hitSpaceShape = shape;
                            m_hitAlertAreaShape = null;
                        }
                        else if (m_hitSpaceShape == shape && m_hitAlertAreaShape != null)
                        {
                            Shape tempShape = m_hitAlertAreaShape;
                            m_hitAlertAreaShape = null;

                            return tempShape;
                        }

                        return shape;
                    }
                }
            }

            // 건물 외부 클릭시 선택된 공간영역 초기화
            if (bChk == false)
                m_hitSpaceShape = null;

            return null;
        }

        public void UnSelectAll()
        {
            if (m_selectedShape != null)
                m_selectedShape.Selected = false;
        }

        private void GdiPanel_MouseMove(object sender, MouseEventArgs e)
        {
            bool needRefresh = false;

            if (m_isPanning)
            {
                m_nMoveX = e.X - m_ptPanningOrigin.X;
                m_nMoveY = e.Y - m_ptPanningOrigin.Y;

                Vertex2D vNewCenter = null;

                vNewCenter = new Vertex2D(m_vOriginCenter.x - m_nMoveX / m_dViewportWeight, m_vOriginCenter.y + m_nMoveY / m_dViewportWeight);
                SetViewportCenter(vNewCenter);

                needRefresh = true;
                //Refresh();
            }
            else if (m_isDXFPanning)
            {
                m_nDXFMoveX = e.X - m_ptDXFPanningOrigin.X;
                m_nDXFMoveY = e.Y - m_ptDXFPanningOrigin.Y;

                if (m_owner != null)
                {
                    Vertex2D vDXFMove = ScreenToGlobal(m_nDXFMoveX, m_nDXFMoveY);
                    m_owner.MoveDXF(vDXFMove - m_vDXFOrigin);
                    needRefresh = true;
                }
            }

            Vertex2D vPos = null;

            if (m_selectedPOI != null)
            {
                double dMoveX = 0.0, dMoveY = 0.0;

                if (m_owner != null)
                    m_owner.GetMove(out dMoveX, out dMoveY);

                vPos = ScreenToGlobal(e.X, e.Y);
                m_selectedPOI.Position = new Vertex2D(vPos.x - dMoveX, vPos.y - dMoveY);
                MoveWire();
                needRefresh = true;
            }

            // Rine Move 기능 제거로 주석처리
            //if (m_selectedWire != null && m_owner.IsWireMoveMode() && m_nMoveWireIndex > 0)
            //{
            //    double dMoveX = 0.0, dMoveY = 0.0;

            //    if (m_owner != null)
            //        m_owner.GetMove(out dMoveX, out dMoveY);

            //    vPos = ScreenToGlobal(e.X, e.Y);

            //    m_selectedWire.Positions[m_nMoveWireIndex] = new Vertex2D(vPos.x - dMoveX, vPos.y - dMoveY);
            //    m_selectedWire.SetRectVertex();
            //    needRefresh = true;
            //}

            if (needRefresh || m_makingWires)
                Refresh();

            if (m_owner != null)
            {
                if (vPos == null)
                    vPos = ScreenToGlobal(e.X, e.Y);

                m_owner.OnMouseMove(e.X, e.Y, vPos.x, vPos.y);

                m_lastMousePT = vPos;
            }
        }

        public void TransferMouseWheel(Point location, int delta)
        {
            if (m_isPanning == true)
                return;

            Vertex2D vCurrent = ScreenToGlobal(location.X, location.Y);
            if (vCurrent == null)
                return;

            double dZoomValue = m_dViewportWeight;

            if (delta < 0)
            {
                dZoomValue *= 0.9;
                if (dZoomValue < 0.0001)
                    dZoomValue = 0.0001;
            }
            else
                dZoomValue /= 0.9;

            Zoom(dZoomValue, vCurrent, true);
        }

        private void GdiPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (m_isPanning || m_isDXFPanning)
                return;

            //if (!m_useMouseWheel)
            //    return;

            Vertex2D vCurrent = ScreenToGlobal(e.X, e.Y);
            if (vCurrent == null)
                return;

            //m_bProcessWheel = true;

            /*m_dtLastMouseWheel = DateTime.Now;

		
		    timerMouseWheel.Enabled = true;
		    timerMouseWheel.Start();*/

            double dZoomValue = m_dViewportWeight;

            if (e.Delta < 0)
            {
                dZoomValue *= 0.9;
                if (dZoomValue < 0.0001)
                    dZoomValue = 0.0001;
            }
            else
                dZoomValue /= 0.9;

            /*if (mDrawSpan != nullptr && mDrawSpan.TotalMilliseconds > 150)
		    {

			    if (m_WheelTimer == nullptr || m_WheelTimer.Enabled == false)
			    {
				    if (m_WheelTimer == nullptr)
				    {
					    m_WheelTimer = gcnew System::Windows::Forms::Timer();
					    m_WheelTimer.Interval = 600;
					    m_WheelTimer.Tick += gcnew System::EventHandler(this, &DXFControl::OnWheelTimerTick);
				    }
				    m_WheelTimer.Enabled = true;
				    m_WheelTimer.Start();
			    }

			    m_bDrawText = false;

			    Zoom(dZoomValue, vCurrent, false);
		    }
		    else*/
            {
                Zoom(dZoomValue, vCurrent, true);
            }

            //m_bProcessWheel = false;
        }

        public void Zoom(double dZoomValue, Vertex2D vZoomCenter, bool refresh)
        {

            //System::Diagnostics::Debug::WriteLine(dZoomValue);

            // 이 이상 넘어가면... 죽는다.
            if (dZoomValue > 7.0 || dZoomValue <= UnE.Geometry.Math.HALF_TOLERANCE())
                return;

            // vZoomCenter에 해당하는 화면좌표1 얻어오기
            Point ptZoomCenter = GlobalToScreen(vZoomCenter);

            double left = ptZoomCenter.X / dZoomValue;
            double top = ptZoomCenter.Y / dZoomValue;
            double right = (ptZoomCenter.X - this.Size.Width) / dZoomValue;
            double bottom = (ptZoomCenter.Y - this.Size.Height) / dZoomValue;

            m_vViewportTL.x = vZoomCenter.x - left;
            m_vViewportTL.y = vZoomCenter.y + top;
            m_vViewportBL.x = vZoomCenter.x - left;
            m_vViewportBL.y = vZoomCenter.y + bottom;
            m_vViewportBR.x = vZoomCenter.x - right;
            m_vViewportBR.y = vZoomCenter.y + bottom;

            m_dViewportWeight = dZoomValue;

            if (m_selectedWire != null && m_selectedWire.RectEditVertexVisible)
                m_selectedWire.SetRectVertex();

            if (refresh)
            {
                Refresh();
            }
            else
                Reshape(this.Size.Width, this.Size.Height);
        }

        public Vertex2D ScreenToGlobal(int x, int y)
        {
            Vertex2D vResult = null;

            if (m_currentInverseMatrix == null)
                return null;

            vResult = new Vertex2D();
            vResult.x = m_currentInverseMatrix.Elements[0] * x + m_currentInverseMatrix.Elements[2] * y + m_currentInverseMatrix.Elements[4];
            vResult.y = m_currentInverseMatrix.Elements[1] * x + m_currentInverseMatrix.Elements[3] * y + m_currentInverseMatrix.Elements[5];

            return vResult;
        }

        public Point GlobalToScreen(Vertex2D vertex)
        {
            Point ptResult = new Point();

            if (m_currentMatrix == null)
                return ptResult;

            ptResult.X = (int)(m_currentMatrix.Elements[0] * vertex.x + m_currentMatrix.Elements[2] * vertex.y + m_currentMatrix.Elements[4]);
            ptResult.Y = (int)(m_currentMatrix.Elements[1] * vertex.x + m_currentMatrix.Elements[3] * vertex.y + m_currentMatrix.Elements[5]);

            return ptResult;
        }

        private Layer GetPOILayer()
        {
            foreach (Layer layer in m_layers)
            {
                if (layer.LayerType == typeof(POI))
                    return layer;
            }

            return null;
        }

        private Layer GetWireLayer()
        {
            foreach (Layer layer in m_layers)
            {
                if (layer.LayerType == typeof(Wire))
                    return layer;
            }

            return null;
        }

        private void GdiPanel_DragDrop(object sender, DragEventArgs e)
        {
            string[] dataFormats = e.Data.GetFormats();

            if (dataFormats != null && dataFormats.Count() == 1)
            {
                POIType poiType = (POIType)e.Data.GetData(typeof(POIType));

                if (poiType != null)
                {
                    Layer layer = GetPOILayer();

                    if (layer != null)
                    {
                        double x = 0.0, y = 0.0;

                        if (m_owner != null)
                            m_owner.GetMove(out x, out y);

                        Point pt = this.PointToClient(new Point(e.X, e.Y));
                        Vertex2D vPos = ScreenToGlobal(pt.X, pt.Y);
                        vPos.x -= x;
                        vPos.y -= y;

                        POI poi = new POI();
                        poi.PoiType = poiType;
                        poi.Position = vPos;
                        poi.Move(x, y);
                        poi.Painter = this;

                        if (m_owner != null)
                            m_owner.UpdatePOI(poi, false);

                        layer.AddShape(poi);
                        Refresh();
                    }
                }
            }
        }

        private void AddPOI(Point pt)
        {             
            POIType poiType = m_owner.SelectedPOI();
            POI poiInfo = m_owner.SelectedPOIInfo();

            if (poiType != null)
            {
                Layer layer = GetPOILayer();

                if (layer != null)
                {
                    double x = 0.0, y = 0.0;

                    if (m_owner != null)
                        m_owner.GetMove(out x, out y);
                    
                    Vertex2D vPos = ScreenToGlobal(pt.X, pt.Y);
                    vPos.x -= x;
                    vPos.y -= y;

                    POI poi = new POI();
                    poi.PoiType = poiType;
                    poi.Position = vPos;
                    poi.Move(x, y);
                    poi.Painter = this;
                    poi.FillColor = poiType.Color;
                    
                    poi.Height = poiInfo.Height;

                    // 사용자 정의 POI 일 경우
                    if (poi.PoiType.Code == "F9999")
                    {
                        Property property = new Property();
                        property.Name = "사용자 POI 이름";
                        property.Value = poi.PoiType.Name;
                        poi.Properties.Add(property);
                    }

                    foreach(Property prop in poiInfo.Properties)
                    {
                        poi.Properties.Add(prop);
                    }

                    if (m_owner != null)
                        m_owner.UpdatePOI(poi, false);

                    layer.AddShape(poi);

                    bool visible = FormMain.Instance.BimManager.GetPOIVisible(m_level.XMLID, poiType.ID);
                    if (!visible && m_owner.IsPOIAddMode())
                    {
                        DialogResult digResult = MessageBox.Show("[" + poiType.Name + "] 해당 POI는 현재 비활성화 상태입니다.\r활성화 상태로 변경하실래요?", "", MessageBoxButtons.YesNo);
                        if (digResult == DialogResult.Yes)
                        {
                            FormMain.Instance.BimManager.SetPoiVisibleTrue(m_project.Name, m_level.ID, poiType.ID);
                            
                            foreach (Layer item in m_layers)
                            {
                                if (item is POILayer)
                                {
                                    foreach (Shape shape in item.Shapes)
                                    {
                                        if (shape is POI)
                                        {
                                            POI poi2 = shape as POI;
                                            poi2.POIVisible = FormMain.Instance.BimManager.GetPOIVisible(m_level.XMLID, poi2.PoiType.ID);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                    Refresh();
                }
            }
        }

        private int m_addWirePropertyID = 0;
        private List<MakeWire> m_makeWires = new List<MakeWire>();
        private bool m_makingWires = false; // Line 생성중인가 ?
        private Vertex2D m_lastMousePT = null;
        private void AddWire(Point pt, POI selectedPoi)
        {
            if (m_makeWires.Count == 0 && selectedPoi == null)
            {
                MessageBox.Show("Line을 생성할 POI를 선택하세요.");
                return;
            }

            if (m_poiLayer.Shapes.Count < 2)
            {
                MessageBox.Show("Line을 생성하려면 최소 2개의 POI가 있어야 합니다.");
                return;
            }

            bool result = true;
            int poiCnt = 0;
            foreach (MakeWire item in m_makeWires)
            {
                if (item.TargetPOI == null)
                    continue;
                else
                {
                    if (poiCnt > 2)
                    {
                        MessageBox.Show("2개의 POI만 연결할 수 있습니다.");
                        result = false;
                        break;
                    }
                    poiCnt++;
                }
                
                if (item.TargetPOI == selectedPoi)
                {
                    MessageBox.Show("같은 POI를 선택할 수 없습니다.");
                    result = false;
                    break;
                }
            }
            if (!result)
                return;

            POITypeProperty property = m_owner.SelectedWire();
            if (property != null)
            {
                // 중간에 Wire 종류가 변경됐을 경우를 방지
                if (property.POITypeID != m_addWirePropertyID)
                {
                    m_makeWires.Clear();

                    m_addWirePropertyID = property.POITypeID;
                }

                double x = 0.0, y = 0.0;

                if (m_owner != null)
                    m_owner.GetMove(out x, out y);

                Vertex2D vPos = ScreenToGlobal(pt.X, pt.Y);

                if (m_useOrthoSnap && m_vLastWirePosition != null)
                    vPos = new Vertex2D(m_vLastWirePosition);

                vPos.x -= x;
                vPos.y -= y;

                MakeWire newWire = new MakeWire();
                    newWire.TargetPOI = selectedPoi;
                newWire.targetVertex2D = (selectedPoi == null) ? vPos : selectedPoi.Position;
                    m_makeWires.Add(newWire);
                //}

                m_makingWires = true;

                if (m_makeWires.Count > 1 && m_makeWires[0].TargetPOI != null && m_makeWires[m_makeWires.Count - 1].TargetPOI != null)
                {
                    SaveDBWire();
                    Refresh();
                }
            }
        }

        private void SaveDBWire()
        {
            double x = 0.0, y = 0.0;

            if (m_owner != null)
                m_owner.GetMove(out x, out y);

            Wire wire = new Wire();

            Shapes.POI poi = new Shapes.POI();
            poi.PoiType = poi.PoiType = FormMain.Instance.BimManager.POITypes[m_owner.SelectedWire().POITypeID];
            wire.POIIcon = poi;

            wire.BeginPOI = m_makeWires[0].TargetPOI.ID;
            wire.EndPOI = m_makeWires[m_makeWires.Count - 1].TargetPOI.ID;
            wire.Lines = wire.GetStrPosition(m_makeWires);
            wire.POITypeID = m_owner.SelectedWire().POITypeID;
            wire.Move(x, y);
            wire.LevelID = m_level.ID;
            
            // DB 저장
            if (m_owner != null)
            {
                if (m_owner.UpdateWire(wire, false))
                {
                    Layer layer = GetWireLayer();
                    layer.AddShape(wire);
                    if (wire.POIIcon != null)
                    {
                        wire.POIIcon.Layer = layer;
                        layer.AddShape(poi);
                    }
                }
            }

            wire.SetIconPosition();
            wire.POIIcon.Move(x, y);

            m_makingWires = false;
            m_makeWires.Clear();
        }

        private void DeleteWire()
        {
            Layer layer = GetWireLayer();
            if (layer != null)
            {
                if (layer.Shapes.Count > 0)
                {
                    List<Shape> deleteWire = GetWires(m_selectedPOI.ID);
                    
                    foreach (Wire item in deleteWire)
                    {
                        m_owner.UpdateWire(item, true);

                        if (item.POIIcon != null)
                            layer.RemoveShape(item.POIIcon);
                        layer.RemoveShape(item);
                    }
                }
            }

            m_makeWires.Clear();
            m_addWirePropertyID = -1;
            m_makingWires = false;
        }

        private List<Shape> GetWires(int poiID)
        {
            Layer layer = GetWireLayer();
            if (layer != null)
            {
                if (layer.Shapes.Count > 0)
                {
                    List<Shape> wires = new List<Shape>();
                    foreach (Shape item in layer.Shapes)
                    {
                        if (item is Wire)
                        {
                            Wire wire = item as Wire;
                            if (wire.BeginPOI == poiID || wire.EndPOI == poiID)
                                wires.Add(wire); 
                        }
                    }

                    return wires;
                }
            }

            return new List<Shape>();
        }

        private void MoveWire()
        {
            Layer layer = GetWireLayer();
            if (layer != null)
            {
                if (layer.Shapes.Count > 0)
                {                    
                    foreach (Shape item in layer.Shapes)
                    {
                        if (item is Wire)
                        {
                            Wire wire = item as Wire;
                            if (wire.BeginPOI == m_selectedPOI.ID)
                            {
                                wire.Positions[0] = m_selectedPOI.Position;
                                wire.SetIconPosition();
                            }
                            else if (wire.EndPOI == m_selectedPOI.ID)
                            {
                                wire.Positions[wire.Positions.Count - 1] = m_selectedPOI.Position;
                                wire.SetIconPosition();
                            } 
                        }
                    }
                }
            }
        }

        private void GdiPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (m_owner != null)
            {
                Point pt = this.PointToClient(new Point(e.X, e.Y));
                Vertex2D vertex = ScreenToGlobal(pt.X, pt.Y);

                m_owner.OnMouseMove(e.X, e.Y, vertex.x, vertex.y);
                System.Diagnostics.Trace.WriteLine(string.Format("Coord : {0}, {1}", vertex.x, vertex.y));
            }

            string[] dataFormats = e.Data.GetFormats();

            if (dataFormats != null && dataFormats.Count() == 1)
            {
                POIType poiType = (POIType)e.Data.GetData(typeof(POIType));

                if (poiType != null)
                {
                    System.Diagnostics.Trace.WriteLine("POIType DragEnter");
                    e.Effect = DragDropEffects.Move;
                    return;
                }
            }

            /*if (e.Data.GetDataPresent(DataFormats.))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Count() == 1)
                {
                    string strFileName = files[0].ToLower();

                    if (strFileName.EndsWith("xml"))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }*/

            e.Effect = DragDropEffects.None;
        }

        private void GdiPanel_DragOver(object sender, DragEventArgs e)
        {
            if (m_owner != null)
            {
                Point pt = this.PointToClient(new Point(e.X, e.Y));
                Vertex2D vertex = ScreenToGlobal(pt.X, pt.Y);

                m_owner.OnMouseMove(e.X, e.Y, vertex.x, vertex.y);
                System.Diagnostics.Trace.WriteLine(string.Format("DragOver : {0}, {1} => {2}, {3}", e.X, e.Y, vertex.x, vertex.y));
            }
        }

        public Shape GetSelectedShape()
        {
            return m_selectedShape;
        }

        public void NoResizeReshape()
        {
            m_resizeReshape = false;
        }

        #region DXFViewer.IPainter
        private DXFViewer.Layer m_currentDXFLayer = null;
        private DXFViewer.Block m_currentDXFBlock = null;
        private DXFViewer.LineType m_dxfLineType = null;
        private DXFViewer.DXFControl m_dxfControl = null;
        private int m_nEditBoxLength = 10;
        private SolidBrush m_brushEditBox = new SolidBrush(Color.FromArgb(0, 127, 255));
        private Pen m_penEditBox = new Pen(Color.Gray);
        private Pen m_penSelectedBright1 = null;
        private Pen m_penSelectedBright2 = null;

        private void InitDXF()
        {
            m_dxfControl = new DXFViewer.DXFControl();
            m_dxfLineType = new DXFViewer.LineType(m_dxfControl, System.Drawing.Drawing2D.DashStyle.Dash, 1);
            m_penSelectedBright1 = new Pen(Color.FromArgb(255 - this.BackColor.R, 255 - this.BackColor.G, 255 - this.BackColor.B));
            m_penSelectedBright2 = new Pen(this.BackColor);
        }

        public void SetCurrentLayer(DXFViewer.Layer layer)
        {
            m_currentDXFLayer = layer;
        }

        public DXFViewer.Layer GetCurrentLayer()
        {
            return m_currentDXFLayer;
        }

        public void SetCurrentBlock(DXFViewer.Block block)
        {
            m_currentDXFBlock = block;
        }

        public DXFViewer.Block GetCurrentBlock()
        {
            return m_currentDXFBlock;
        }

        public int GetScreenWidth()
        {
            return this.Size.Width;
        }

        public int GetScreenHeight()
        {
            return this.Size.Height;
        }

        public DXFViewer.LineType GetSelectedLineType()
        {
            return m_dxfLineType;
        }

        // Y축이 화면 아래에서 위쪽으로 증가하는 방향인가?
        public bool DownToTop()
        {
            return true;
        }

        public void _Refresh()
        {
            SetViewportCenter(GetViewportCenter());
            Refresh();
        }

        public Color GetBackColor()
        {
            return this.BackColor;
        }

        public float EditBoxLength
        {

            get
            {
                Vertex2D v1 = ScreenToGlobal(0, 0);
                Vertex2D v2 = ScreenToGlobal(m_nEditBoxLength, 0);
                return (float)v1.GetDistance(v2);
            }
        }

    public SolidBrush EditBoxBrush
    {
        get { return m_brushEditBox; }
    }

    public Pen EditBoxPen
    {
        get { return m_penEditBox; }
    }

    public Pen SelectedBrightPen1
    {
        get
        {
            m_penSelectedBright1.Color = Color.FromArgb(255 - this.BackColor.R, 255 - this.BackColor.G, 255 - this.BackColor.B);
            return m_penSelectedBright1;
        }
    }

    public Pen SelectedBrightPen2
    {
        get
        {
            m_penSelectedBright2.Color = this.BackColor;
            return m_penSelectedBright2;
        }
    }

    public DXFViewer.IPainter.RendererType Renderer
	{
        get { return DXFViewer.IPainter.RendererType.GDI_PLUS; }
        set { }
    }

        #endregion
    }

    public interface IGDIOwner
    {
        void OnMouseMove(int screenX, int screenY, double panelX, double panelY);
        void GetMove(out double x, out double y);
        void FocusView();
        bool IsPOIAddMode();
        bool IsPOIMoveMode();
        bool IsPOIDeleteMode();
        bool IsPOIDoneMode();
        bool IsWireAddMode();
        //bool IsWireMoveMode();    // Rine Move 기능 제거로 주석처리
        bool IsWireDeleteMode();
        bool IsWireDoneMode();
        bool IsPropertyMode();
        bool UpdatePOI(POI poi, bool isDelete);
        bool UpdateWire(Wire wire, bool isDelete);
        void RefreshView();
        void OnSelectShape(Shape shape);
        POIType SelectedPOI();
        POI SelectedPOIInfo();
        POITypeProperty SelectedWire();
        DXFViewer.IPainter GetDXFPainter();
        void MoveDXF(Vertex2D vMove);
        void SetOriginDXF();
        void SetMoveOrNot(int nIndex);
        bool GetMoveOrNot(int nIndex);
    }

    public interface IPainter
    {
        Vertex2D ScreenToGlobal(int x, int y);
        Point GlobalToScreen(Vertex2D vertex);
        VariousData<double> SnapDistance
        {
            get;
        }
    }

    
}
