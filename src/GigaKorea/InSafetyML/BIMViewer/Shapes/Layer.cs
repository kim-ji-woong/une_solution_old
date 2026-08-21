using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using UnE.Geometry;

namespace BIMViewer.Shapes
{
    public class Layer
    {
        private string m_strLayerName = "";

        protected bool m_visibleLine = true;
        protected bool m_visibleCenterLine = false;
        protected bool m_visibleFill = true;
        protected bool m_visibleText = false;
        protected Color m_clrLine = Color.Black;
        protected Color m_clrCenterLine = Color.Black;
        protected Color m_clrFill = Color.Black;
        protected Color m_clrText = Color.Black;
        protected Font m_font = new Font("맑은 고딕", 30, FontStyle.Bold);
        protected float m_fLineThick = 3.0f;
        protected Brush m_textBrush = null;
        // ZoomScale을 무시하고 항상 일정한 크기로 그릴 것인가?
        protected bool m_ignoreScale = false;
        protected float m_fFixedScale = 1.0f;
        protected float m_fScale = 1.0f;

        protected List<Shape> m_shapes = new List<Shape>();

        protected Type m_layerType = null;

        protected Pen m_centerLinePen = null;
        protected IPainter m_painter = null;

        public Pen CenterLinePen
        {
            get { return m_centerLinePen; }
        }
        
        public string Name
        {
            get { return m_strLayerName; }
            set { m_strLayerName = value; }
        }

        public bool VisibleLine
        {
            get { return m_visibleLine; }
            set { m_visibleLine = value; }
        }

        public bool VisibleCenterLine
        {
            get { return m_visibleCenterLine; }
            set { m_visibleCenterLine = value; }
        }

        public bool VisibleFill
        {
            get { return m_visibleFill; }
            set { m_visibleFill = value; }
        }

        public bool VisibleText
        {
            get { return m_visibleText; }
            set { m_visibleText = value; }
        }

        public Color LineColor
        {
            get { return m_clrLine; }
            set { m_clrLine = value; }
        }

        public Color CenterLineColor
        {
            get { return m_clrCenterLine; }
            set { m_clrCenterLine = value; }
        }

        public Color FillColor
        {
            get { return m_clrFill; }
            set { m_clrFill = value; }
        }

        public Color TextColor
        {
            get { return m_clrText; }
            set { m_clrText = value; }
        }

        public Brush TextBrush
        {
            get { return m_textBrush; }
            set { m_textBrush = value; }
        }

        public Font Font
        {
            get { return m_font; }
            set { m_font = value; }
        }

        public float LineThick
        {
            get { return m_fLineThick; }
            set { m_fLineThick = value; }
        }

        public List<Shape> Shapes
        {
            get { return m_shapes; }
        }

        public Type LayerType
        {
            get { return m_layerType; }
            set { m_layerType = value; }
        }

        // ZoomScale을 무시하고 항상 일정한 크기로 그릴 것인가?
        public bool IgnoreScale
        {
            get { return m_ignoreScale; }
            set { m_ignoreScale = value; }
        }

        // IgnoreScale이 true일때 사용
        public float FixedScale
        {
            get { return m_fFixedScale; }
        }

        // IgnoreScale이 false일때 사용
        public float Scale
        {
            get { return m_fScale; }
        }

        public IPainter Painter
        {
            get { return m_painter; }
            set { m_painter = value; }
        }

        public static List<Layer> InitLayers(Dictionary<string, bool> dicLayerLineVisible, Dictionary<string, bool> dicLayerFillVisible, Dictionary<string, bool> dicLayerTextVisible)
        {
            object[] layerItems = new object[]
            {
                "Space", typeof(BIM.Space), 0, 255, 0, 0, 0, 0, 1.0f,
                "AlertArea", typeof(BIM.AlertArea), 255, 0, 255, 0, 0, 0, 1.0f,
                "Wall", typeof(BIM.Wall), 192, 192, 192, 192, 192, 192, 1.0f,
                "Door", typeof(BIM.Door), 0, 0, 255, 0, 0, 255, 1.0f,
                "Window", typeof(BIM.Window), 0, 0, 255, 0, 255, 255, 1.0f,
                "Column", typeof(BIM.Column), 255, 255, 0, 0, 0, 0, 1.0f,
            };

            List<Layer> layers = new List<Layer>();
            int nItemCount = layerItems.Count();

            //layers.Add(MakePOILayer());
            //layers.Add(MakeWireLayer());

            //Layer spaceLayer = null, wallLayer = null;

            for (int i = 0; i < nItemCount - 8; i += 9)
            {
                string strLayerName = (string)layerItems[i];

                Layer layer = strLayerName == "Wall" ? new WallLayer() : new Layer();
                layer.Name = strLayerName;
                layer.LayerType = (Type)layerItems[i + 1];
                layer.LineColor = Color.FromArgb((int)layerItems[i + 2], (int)layerItems[i + 3], (int)layerItems[i + 4]);
                layer.LineThick = (float)layerItems[i + 8];
                layer.FillColor = Color.FromArgb((int)layerItems[i + 5], (int)layerItems[i + 6], (int)layerItems[i + 7]);
                layer.m_visibleFill = false;

                layers.Add(layer);

                //if (strLayerName == "Space")
                //    spaceLayer = layer;
            }

            layers.Add(MakePOILayer());
            layers.Add(MakeWireLayer());

            /*if (spaceLayer != null)
            {
                spaceLayer.VisibleText = true;
                spaceLayer.TextColor = Color.Yellow;
            }*/

            bool visible;

            foreach (Layer layer in layers)
            {
                if (dicLayerLineVisible.TryGetValue(layer.Name, out visible))
                    layer.VisibleLine = visible;

                if (dicLayerFillVisible.TryGetValue(layer.Name, out visible))
                    layer.VisibleFill = visible;

                if (dicLayerTextVisible.TryGetValue(layer.Name, out visible))
                    layer.VisibleText = visible;
            }

            return layers;
        }

        private static Layer MakePOILayer()
        {
            Layer layer = new POILayer();

            layer.m_strLayerName = "POI";
            layer.LayerType = typeof(POI);
            layer.FillColor = Color.Yellow;
            layer.m_visibleFill = true;
            layer.m_visibleLine = false;
            layer.m_visibleText = false;
            //layer.m_ignoreScale = true;
            layer.LineThick = 1.0f;

            return layer;
        }

        private static Layer MakeWireLayer()
        {
            Layer layer = new WireLayer();

            layer.m_strLayerName = "Wire";
            layer.LayerType = typeof(Wire);
            //layer.FillColor = Color.Yellow;
            //layer.m_visibleFill = true;
            //layer.m_visibleLine = false;
            //layer.m_visibleText = false;
            //layer.m_ignoreScale = true;
            //layer.LineThick = 1.0f;

            return layer;
        }


        protected virtual Layer MakeLayer()
        {
            return new Layer();
        }

        public Layer Clone()
        {
            Layer layer = MakeLayer();

            layer.m_strLayerName = this.m_strLayerName;
            layer.m_clrLine = this.m_clrLine;
            layer.m_clrFill = this.m_clrFill;
            layer.m_clrCenterLine = this.m_clrCenterLine;
            layer.m_clrText = this.m_clrText;
            layer.m_font = this.m_font;
            layer.m_fLineThick = this.m_fLineThick;
            layer.m_layerType = this.m_layerType;

            layer.m_visibleFill = this.m_visibleFill;
            layer.m_visibleLine = this.m_visibleLine;
            layer.m_visibleText = this.m_visibleText;
            layer.m_visibleCenterLine = this.m_visibleCenterLine;

            layer.m_ignoreScale = this.m_ignoreScale;
            layer.m_fScale = this.m_fScale;
            layer.m_fFixedScale = this.m_fFixedScale;

            return layer;
        }

        // Line 두께를 화면 Scale에 상관없이 항상 일정하게 유지한다.
        protected float SetScalePenWidth(Pen pen, Graphics g)
        {
            float fOldWidth = pen.Width;

            float fScaleX = g.Transform.Elements[0];
            float fScaleY = g.Transform.Elements[3];

            float fLineWidth1 = 1.0f / fScaleX * fOldWidth;
            float fLineWidth2 = 1.0f / fScaleY * fOldWidth;
            float fLineWidth = fLineWidth1 > fLineWidth2 ? fLineWidth1 : fLineWidth2;

            float fMaxWidth = fScaleX * 31.0f;
            if (fLineWidth > fMaxWidth)
            {
                fLineWidth = fMaxWidth;
            }

            if (fLineWidth < 1.0f)
                fLineWidth = 0.0f;

            pen.Width = fLineWidth;
            return fOldWidth;
        }

        public virtual void Render(Graphics g, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            if (!m_visibleLine && !m_visibleFill && !m_visibleText && !m_visibleCenterLine)
                return;

            Pen pen = m_visibleLine ? new Pen(m_clrLine, m_fLineThick) : null;
            m_centerLinePen = m_visibleCenterLine ? new Pen(m_clrCenterLine, m_fLineThick) : null;
            Brush brush = m_visibleFill ? new SolidBrush(m_clrFill) : null;
            m_textBrush = m_visibleText ? new SolidBrush(m_clrText) : null;

            if (pen != null)
            {
                // Line 두께를 화면 Scale에 상관없이 항상 일정하게 유지한다.
                SetScalePenWidth(pen, g);
            }

            if (m_centerLinePen != null)
            {
                // Line 두께를 화면 Scale에 상관없이 항상 일정하게 유지한다.
                SetScalePenWidth(m_centerLinePen, g);
            }

            foreach (Shape shape in m_shapes)
            {
                if (m_visibleText && m_font != null)
                {
                    shape.TextColor = m_clrText;
                    shape.Font = m_font;
                }
                else
                    shape.Font = null;

                shape.Render(g, pen, brush, vClientAreaTL, vClientAreaBL, vClientAreaBR);
            }

            if (pen != null)
                pen.Dispose();

            if (brush != null)
                brush.Dispose();

            if (m_textBrush != null)
            {
                m_textBrush.Dispose();
                m_textBrush = null;
            }

            if (m_centerLinePen != null)
            {
                m_centerLinePen.Dispose();
                m_centerLinePen = null;
            }
        }

        public void AddShape(Shape shape)
        {
            shape.Layer = this;
            m_shapes.Add(shape);
        }

        public void RemoveShape(Shape shape)
        {
            if (m_shapes.Contains(shape))
            {
                m_shapes.Remove(shape);
                shape.Layer = null;
            }
        }

        public void RemoveShape(int poiID)
        {
            
        }

        public void RemoveAll()
        {
            foreach (Shape shape in m_shapes)
            {
                shape.Layer = null;
            }

            m_shapes.Clear();
        }

        public Shape HitTest(Vertex2D vertex)
        {
            foreach (Shape shape in m_shapes)
            {
                if (shape.HitTest(vertex))
                    return shape;
            }

            return null;
        }

        public VariousData<double> GetSnapDistance()
        {
            if (m_painter == null)
                return null;

            return m_painter.SnapDistance;
        }
    }

    public class WallLayer : Layer
    {
        public WallLayer()
        {
            CenterLineColor = Color.Red;
            VisibleCenterLine = false;
        }

        public override void Render(Graphics g, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            if (!m_visibleLine && !m_visibleFill && !m_visibleText && !m_visibleCenterLine)
                return;

            Brush brush = null;

            // 벽체 채움색을 먼저 그린다.
            if (m_visibleFill || m_visibleText)
            {
                brush = m_visibleFill ? new SolidBrush(m_clrFill) : null;
                m_textBrush = m_visibleText ? new SolidBrush(m_clrText) : null;

                foreach (Shape shape in m_shapes)
                {
                    if (m_visibleText && m_font != null)
                    {
                        shape.TextColor = m_clrText;
                        shape.Font = m_font;
                    }
                    else
                        shape.Font = null;

                    shape.Render(g, null, brush, vClientAreaTL, vClientAreaBL, vClientAreaBR);
                }
            }

            Pen pen = m_visibleLine ? new Pen(m_clrLine, m_fLineThick) : null;

            if (pen != null)
            {
                // Line 두께를 화면 Scale에 상관없이 항상 일정하게 유지한다.
                SetScalePenWidth(pen, g);
            }

            // 벽체 외곽선을 두번째로 그린다.
            foreach (Shape shape in m_shapes)
            {
                shape.Font = null;
                shape.Render(g, pen, null, vClientAreaTL, vClientAreaBL, vClientAreaBR);
            }

            m_centerLinePen = m_visibleCenterLine ? new Pen(m_clrCenterLine, m_fLineThick) : null;

            if (m_centerLinePen != null)
            {
                // Line 두께를 화면 Scale에 상관없이 항상 일정하게 유지한다.
                SetScalePenWidth(m_centerLinePen, g);
            }

            // 벽체 중심선을 마지막에 그린다.
            foreach (Shape shape in m_shapes)
            {
                shape.Font = null;
                shape.Render(g, null, null, vClientAreaTL, vClientAreaBL, vClientAreaBR);
            }

            if (pen != null)
                pen.Dispose();

            if (brush != null)
                brush.Dispose();

            if (m_textBrush != null)
            {
                m_textBrush.Dispose();
                m_textBrush = null;
            }

            if (m_centerLinePen != null)
            {
                m_centerLinePen.Dispose();
                m_centerLinePen = null;
            }
        }

        protected override Layer MakeLayer()
        {
            return new WallLayer();
        }
    }

    public class POILayer : Layer
    {
        public POILayer()
        {
            CenterLineColor = Color.Red;
        }

        public override void Render(Graphics g, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            //if (!m_visibleFill)
            //    return;

            Brush brush = new SolidBrush(m_clrFill);
            Pen pen = new Pen(m_clrFill, m_fLineThick);

            // Line 두께를 화면 Scale에 상관없이 항상 일정하게 유지한다.
            SetScalePenWidth(pen, g);

            float fScale = m_fScale;
            float fFixedScale = m_fFixedScale;

            if (m_ignoreScale)
            {
                float fCurrentScale = g.Transform.Elements[0];
                fScale = m_fFixedScale / fCurrentScale;
            }
            else
            {
                fFixedScale = g.Transform.Elements[0] * m_fScale;
            }

            foreach (Shape shape in m_shapes)
            {
                bool visible = true;
                if (shape is POI)    
                {
                    POI poi = shape as POI;
                    visible = poi.PoiType.POIVisible;
                    brush = new SolidBrush(poi.PoiType.Color);
                }
                if (visible)
                {                    
                    shape.Render(g, pen, brush, vClientAreaTL, vClientAreaBL, vClientAreaBR);
                }
            }

            m_fScale = fScale;
            m_fFixedScale = fFixedScale;

            if (pen != null)
                pen.Dispose();

            if (brush != null)
                brush.Dispose();
        }

        protected override Layer MakeLayer()
        {
            return new POILayer();
        }
    }

    public class WireLayer : Layer
    {
        public WireLayer()
        {
            this.FillColor = Color.FromArgb(255, 0, 255);
            //m_ignoreScale = true;
            m_fScale = 5.0f;
            //m_fFixedScale = 0.22f;
        }

        public override void Render(Graphics g, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            //if (!m_visibleFill)
            //    return;

            Brush brush = new SolidBrush(m_clrFill);
            Pen pen = new Pen(m_clrFill, m_fLineThick);

            // Line 두께를 화면 Scale에 상관없이 항상 일정하게 유지한다.
            SetScalePenWidth(pen, g);

            float fScale = m_fScale;
            float fFixedScale = m_fFixedScale;

            if (m_ignoreScale)
            {
                float fCurrentScale = g.Transform.Elements[0];
                fScale = m_fFixedScale / fCurrentScale;
            }
            else
            {
                fFixedScale = g.Transform.Elements[0] * m_fScale;
            }

            foreach (Shape shape in m_shapes)
            {
                shape.Render(g, pen, brush, vClientAreaTL, vClientAreaBL, vClientAreaBR);
            }

            m_fScale = fScale;
            m_fFixedScale = fFixedScale;

            if (pen != null)
                pen.Dispose();

            if (brush != null)
                brush.Dispose();
        }

        protected override Layer MakeLayer()
        {
            return new WireLayer();
        }
    }
}
