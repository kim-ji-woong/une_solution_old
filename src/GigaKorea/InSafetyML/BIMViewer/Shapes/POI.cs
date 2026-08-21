using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using UnE.Geometry;
using System.Drawing.Drawing2D;

namespace BIMViewer.Shapes
{
    public class POI : Shape
    {
        private class Icon
        {
            private List<GraphicsPath> m_edgePath = null;
            private List<GraphicsPath> m_fillPath = null;
            private List<POIType.TextData> m_textDatas = null;

            private Vertex2D m_vTL = null;
            private Vertex2D m_vBR = null;
            private Vertex2D m_vPos = null;
            private Vertex2D m_vTextMove = new Vertex2D();

            public Vertex2D BoundaryTL
            {
                get { return m_vTL; }
                set { m_vTL = value; }
            }

            public Vertex2D BoundaryBR
            {
                get { return m_vBR; }
                set { m_vBR = value; }
            }

            public double Width
            {
                get { return m_vBR.x - m_vTL.x; }
            }

            public double Height
            {
                get { return m_vTL.y - m_vBR.y; }
            }

            public List<GraphicsPath> EdgePath
            {
                get { return m_edgePath; }
                set { m_edgePath = value; }
            }

            public List<GraphicsPath> FillPath
            {
                get { return m_fillPath; }
                set { m_fillPath = value; }
            }

            public List<POIType.TextData> TextDatas
            {
                get { return m_textDatas; }
                set { m_textDatas = value; }
            }

            public Vertex2D TextMove
            {
                get { return m_vTextMove; }
                set { m_vTextMove = value; }
            }
            
            public void Render(Graphics g, Pen pen, Brush brush, double x, double y, float fScaleY)
            {
                double dMoveX = x - m_vPos.x;
                double dMoveY = y - m_vPos.y;
                bool needTranslate = System.Math.Abs(dMoveX) > UnE.Geometry.Math.HALF_TOLERANCE() || System.Math.Abs(dMoveY) > UnE.Geometry.Math.HALF_TOLERANCE();

                if (needTranslate)
                    g.TranslateTransform((float)(x - m_vPos.x), (float)(y - m_vPos.y));

                if (brush != null && m_fillPath != null)
                {
                    foreach (GraphicsPath path in m_fillPath)
                    {
                        g.FillPath(brush, path);
                    }
                }

                if (pen != null && m_edgePath != null)
                {
                    foreach (GraphicsPath path in m_edgePath)
                    {
                        g.DrawPath(pen, path);
                    }
                }

                if (pen != null)
                {
                    foreach (POIType.TextData text in m_textDatas)
                    {
                        if (text.BoundaryTL == null)
                            SetTextBoundary(g, text);

                        text.Render(g, (float)(text.Position.x + m_vTextMove.x), (float)(text.Position.y + m_vTextMove.y), pen.Color, fScaleY);
                        //text.Render(g, (float)x, (float)y, pen.Color);
                    }
                }

                if (needTranslate)
                    g.TranslateTransform((float)(-dMoveX), (float)(-dMoveY));
            }

            private void SetTextBoundary(Graphics g, POIType.TextData text)
            {
                Font font = text.GetFont();
                SizeF size = g.MeasureString(text.Text, font);
                font.Dispose();

                double tlX = text.Position.x - size.Width / 2;
                double tlY = text.Position.y + size.Height / 2;
                double brX = text.Position.x + size.Width / 2;
                double brY = text.Position.y - size.Height / 2;

                if (m_vTL == null)
                {
                    m_vTL = new Vertex2D(tlX, tlY);
                    m_vBR = new Vertex2D(brX, brY);
                }
                else
                {
                    if (m_vTL.x > tlX)
                        m_vTL.x = tlX;
                    if (m_vTL.y < tlY)
                        m_vTL.y = tlY;
                    if (m_vBR.x < brX)
                        m_vBR.x = brX;
                    if (m_vBR.y > brY)
                        m_vBR.y = brY;
                }

                text.BoundaryTL = new Vertex2D(tlX, tlY);
                text.BoundaryBR = new Vertex2D(brX, brY);
            }

            public bool HitTest(double x, double y)
            {
                if (x >= m_vTL.x && x <= m_vBR.x &&
                    y <= m_vTL.y && y >= m_vBR.y)
                    return true;

                return false;
            }

            public void SetPosition(double x, double y)
            {
                if (m_vPos == null)
                    m_vPos = new Vertex2D(x, y);
                else
                    m_vPos.SetVertex(x, y);
            }
        }

        public enum DrawType { Circle, Rect, Triangle, Image };

        private int m_nID = 0;
        private string m_strXMLID = "";
        //private string m_strPOIID = "";
        private string m_strPOIName = "";

        private Color m_fillColor = Color.Red;
        // Pixel
        private int m_nHeight = 200;
        private Vertex2D m_vPos = new Vertex2D();
        private DrawType m_drawType = DrawType.Circle;
        private POIType m_poiType = null;
        private double m_dMoveX = 0.0, m_dMoveY = 0.0;

        private double m_dAngle = 0.0;

        private IPainter m_painter = null;
        //private bool m_ignoreScale = false;

        private Icon m_icon = null;
        private List<BIM.Property> m_properties = new List<BIM.Property>();

        public const string POIIDTag = "poi";

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

        /*public string POIID
        {
            get { return m_strPOIID; }
            set { m_strPOIID = value; }
        }*/

        public string Name
        {
            get { return m_strPOIName; }
            set { m_strPOIName = value; }
        }

        public Color FillColor
        {
            get { return m_fillColor; }
            set { m_fillColor = value; }
        }

        // Pixel
        public int Height
        {
            get { return m_nHeight; }
            set { m_nHeight = value; }
        }

        public Vertex2D Position
        {
            get { return m_vPos; }
            set
            {
                m_vPos = value;

                if (m_vPos != null)
                {
                    m_vTL = new Vertex2D(m_vPos.x - m_nHeight / 2, m_vPos.y + m_nHeight / 2);
                    m_vBR = new Vertex2D(m_vPos.x + m_nHeight / 2, m_vPos.y - m_nHeight / 2);
                }
            }
        }

        public DrawType DrawingType
        {
            get { return m_drawType; }
            set { m_drawType = value; }
        }

        public POIType PoiType
        {
            get { return m_poiType; }
            set { m_poiType = value; }
        }

        public IPainter Painter
        {
            get { return m_painter; }
            set { m_painter = value; }
        }

        public double Angle
        {
            get { return m_dAngle; }
            set { m_dAngle = value; }
        }

        public List<BIM.Property> Properties
        {
            get { return m_properties; }
        }

        public bool IgnoreScale
        {
            get
            {
                if (m_layer == null)
                    return false;

                return m_layer.IgnoreScale;
            }
        }

        public override void Render(Graphics g, Pen pen, Brush brush, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {            
            double x = m_vPos.x + m_dMoveX;
            double y = m_vPos.y + m_dMoveY;

            //if (x < vClientAreaBL.x || x > vClientAreaBR.x || y > vClientAreaTL.y || y < vClientAreaBL.y)
            //    return;

            int nHeight = GetPOISize();

            /*if (m_ignoreScale)
            {
                if (m_painter == null)
                    return;

                Point pt = m_painter.GlobalToScreen(new Vertex2D(x, y));

                Matrix matrix = g.Transform;
                g.ResetTransform();

                Draw(g, pen, brush, pt.X - nHeight / 2, pt.Y - nHeight / 2, nHeight);

                g.Transform = matrix;
            }
            else*/
            {
                Matrix oldMatrix = g.Transform;

                if (IgnoreScale && m_vTL != null)
                {
                    float fCurrentScale = g.Transform.Elements[0];

                    g.TranslateTransform((float)x, (float)y);
                    g.ScaleTransform(m_layer.FixedScale / fCurrentScale, m_layer.FixedScale / fCurrentScale);
                    g.TranslateTransform((float)-x, (float)-y);
                }
                else
                {
                    if (Layer.Scale != 1.0f)
                    {
                        float fCurrentScale = g.Transform.Elements[0];
                        
                        g.TranslateTransform((float)x, (float)y);
                        g.ScaleTransform(m_layer.Scale, m_layer.Scale);
                        g.TranslateTransform((float)-x, (float)-y);
                    }
                }

                float fScaleY = g.Transform.Elements[3];
                
                if (m_dAngle != 0.0)
                {
                    g.TranslateTransform((float)x, (float)y);
                    g.RotateTransform((float)m_dAngle);
                    g.TranslateTransform((float)-x, (float)-y);
                }

                if (m_icon != null)
                {
                    if (m_selected)
                    {
                        pen = new Pen(m_selectedLineColor, pen.Width);
                        brush = new SolidBrush(m_selectedFillColor);
                    }
                    else
                    {
                        pen = new Pen(m_fillColor, pen.Width);
                        brush = new SolidBrush(m_fillColor);
                    }

                    m_icon.Render(g, pen, brush, x, y, fScaleY);

                    if (m_selected)
                    {
                        pen.Dispose();
                        brush.Dispose();
                    }
                }
                else
                    Draw(g, pen, brush, (float)x - nHeight / 2, (float)y - nHeight / 2, nHeight);

                if (oldMatrix != null)
                {
                    g.Transform = oldMatrix;
                }
            }

            /*if (m_drawType == DrawType.Circle)
            {
                g.FillEllipse(brush, (float)(m_vPos.x + m_dMoveX), (float)(m_vPos.y + m_dMoveY), m_nHeight, m_nHeight);
            }
            else if (m_drawType == DrawType.Rect)
            {
                g.FillRectangle(brush, (float)(m_vPos.x + m_dMoveX), (float)(m_vPos.y + m_dMoveY), m_nHeight, m_nHeight);
            }
            else if (m_drawType == DrawType.Triangle)
            {
                PointF[] arr = new PointF[4];
                arr[0] = new PointF((float)(m_vPos.x + m_dMoveX), (float)(m_vPos.y + m_dMoveY));
                arr[1] = new PointF((float)(m_vPos.x + m_dMoveX) - m_nHeight / 2, (float)(m_vPos.y + m_dMoveY) - m_nHeight);
                arr[2] = new PointF((float)(m_vPos.x + m_dMoveX) + m_nHeight / 2, (float)(m_vPos.y + m_dMoveY) - m_nHeight);
                arr[3] = arr[0];

                g.FillPolygon(brush, arr);
            }*/
        }

        private int GetPOISize()
        {
            /*if (IgnoreScale)
            {
                return m_nHeight / 10;
            }*/

            return m_nHeight;
        }

        private void Draw(Graphics g, Pen pen, Brush brush, float x, float y, int nHeight)
        {
            if (m_selected)
                brush = new SolidBrush(m_selectedFillColor);

            if (m_drawType == DrawType.Circle)
            {
                g.FillEllipse(brush, x, y, nHeight, nHeight);
            }
            else if (m_drawType == DrawType.Rect)
            {
                g.FillRectangle(brush, x, y, nHeight, nHeight);
            }
            else if (m_drawType == DrawType.Triangle)
            {
                PointF[] arr = new PointF[4];
                arr[0] = new PointF(x, y);
                arr[1] = new PointF(x - nHeight / 2, y - nHeight);
                arr[2] = new PointF(x + nHeight / 2, y - nHeight);
                arr[3] = arr[0];

                g.FillPolygon(brush, arr);
            }

            if (m_selected)
                brush.Dispose();
        }

        public override void Move(double dMoveX, double dMoveY)
        {
            m_dMoveX = dMoveX;
            m_dMoveY = dMoveY;

            if (m_poiType != null && m_poiType.HasIcon)
            {
                m_icon = new Icon();
                m_icon.SetPosition(m_vPos.x + m_dMoveX, m_vPos.y + m_dMoveY);

                Vertex2D vTL = null, vBR = null;

                m_icon.EdgePath = m_poiType.MakePath(m_vPos.x + m_dMoveX, m_vPos.y + m_dMoveY, ref vTL, ref vBR);
                m_icon.FillPath = m_poiType.MakePolygons(m_vPos.x + m_dMoveX, m_vPos.y + m_dMoveY, ref vTL, ref vBR);
                m_icon.TextDatas = m_poiType.TextDatas;
                m_icon.TextMove = new Vertex2D(m_vPos.x + m_dMoveX, m_vPos.y + m_dMoveY);

                m_icon.BoundaryTL = vTL;
                m_icon.BoundaryBR = vBR;
            }
            else
                m_icon = null;
        }

        public override bool HitTest(Vertex2D vertex)
        {
            int nHeight = GetPOISize();

            double x = m_vPos.x + m_dMoveX;
            double y = m_vPos.y + m_dMoveY;

            if (IgnoreScale)
            {
                if (m_painter == null)
                    return false;

                double tlX = m_vTL.x + m_dMoveX;
                double tlY = m_vTL.y + m_dMoveY;
                double brX = m_vBR.x + m_dMoveX;
                double brY = m_vBR.y + m_dMoveY;

                if (vertex.x >= tlX && vertex.x <= brX && vertex.y <= tlY && vertex.y >= brY)
                    return true;

                /*Point ptCenter = m_painter.GlobalToScreen(new Vertex2D(x, y));
                Point pt = m_painter.GlobalToScreen(vertex);

                if (pt.X >= ptCenter.X - nHeight / 2 && pt.X <= ptCenter.X + nHeight / 2 &&
                    pt.Y >= ptCenter.Y - nHeight / 2 && pt.Y <= ptCenter.Y + nHeight / 2)
                    return true;*/
            }
            else
            {
                if (m_icon != null)
                {
                    if (m_icon.TextDatas.Count > 0)
                    {               
                        double dHalfWidth = (m_icon.TextDatas[0].BoundaryBR.x - m_icon.TextDatas[0].BoundaryTL.x) / 2;
                        double dHalfHeight = (m_icon.TextDatas[0].BoundaryTL.y - m_icon.TextDatas[0].BoundaryBR.y) / 2;

                        if (vertex.x >= x - dHalfWidth && vertex.x <= x + dHalfWidth &&
                            vertex.y >= y - dHalfHeight && vertex.y <= y + dHalfHeight)
                            return true;                        
                    }
                    else
                    {
                        double dHalfWidth = m_icon.Width / 2;
                        double dHalfHeight = m_icon.Height / 2;

                        if (vertex.x >= x - dHalfWidth && vertex.x <= x + dHalfWidth &&
                            vertex.y >= y - dHalfHeight && vertex.y <= y + dHalfHeight)
                            return true;
                        //return m_icon.HitTest(vertex.x, vertex.y);
                    }
                }
                else
                {
                    if (vertex.x >= x - nHeight / 2 && vertex.x <= x + nHeight / 2 &&
                        vertex.y >= y - nHeight / 2 && vertex.y <= y + nHeight / 2)
                        return true;
                }
            }

            return false;
        }
    }

    public class POITypeProperty
    {
        private int m_nPOITypeID = 0;
        private string m_strPropertyName = "";
        private string m_strPropertyValue = "";
        private string m_strDescription = "";

        public int POITypeID
        {
            get { return m_nPOITypeID; }
            set { m_nPOITypeID = value; }
        }

        public string PropertyName
        {
            get { return m_strPropertyName; }
            set { m_strPropertyName = value; }
        }

        public string ProperetyValue
        {
            get { return m_strPropertyValue; }
            set { m_strPropertyValue = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }
}
