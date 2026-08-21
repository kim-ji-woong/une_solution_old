using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace BIMViewer.Shapes
{
    public class Wire : Shape
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private int m_nBeginPOI = 0;
        private int m_nEndPOI = 0;
        private int m_nPOITypeID = 0;
        private string m_nLines = "";
        private int m_nLevelID = 0;
        private List<Vertex2D> m_positions = new List<Vertex2D>();
        private Bitmap m_Icon = null;
        private bool m_bVisible = true;
        private POI m_POIIcon = null;

        private double m_dMoveX = 0.0, m_dMoveY = 0.0;
        /// <summary>
        /// key : m_positions index
        /// </summary>
        private Dictionary<int, Rectangle> m_rectEditVertex = new Dictionary<int, Rectangle>();
        private bool m_bRectEditVertexVisible = false;

        public const string WireIDTag = "pw";

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

        public int BeginPOI
        {
            get { return m_nBeginPOI; }
            set { m_nBeginPOI = value; }
        }

        public int EndPOI
        {
            get { return m_nEndPOI; }
            set { m_nEndPOI = value; }
        }

        public int POITypeID
        {
            get { return m_nPOITypeID; }
            set { m_nPOITypeID = value; }
        }

        public string Lines
        {
            get {

                string strReturn = "";
                for (int i = 0; i < m_positions.Count; i++)
                {
                    strReturn += string.Format("{0},{1}", m_positions[i].x, m_positions[i].y);
                    if (i < m_positions.Count - 1)
                        strReturn += ",";
                }
                                
                return strReturn;
            }
            set { m_nLines = value; }
        }

        public int LevelID
        {
            get { return m_nLevelID; }
            set { m_nLevelID = value; }
        }

        public List<Vertex2D> Positions
        {
            get { return m_positions; }
            set { m_positions = value; }
        }

        public bool Visible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }

        public Bitmap Icon
        {
            get { return m_Icon; }
            set { m_Icon = value; }
        }

        public POI POIIcon
        {
            get { return m_POIIcon; }
            set { m_POIIcon = value; }
        }

        public Dictionary<int, Rectangle> RectEditVertex
        {
            get { return m_rectEditVertex; }
            set { m_rectEditVertex = value; }
        }

        public bool RectEditVertexVisible
        {
            get { return m_bRectEditVertexVisible; }
            set { m_bRectEditVertexVisible = value; }
        }

        public override void Render(Graphics g, Pen pen, Brush brush, Vertex2D vClientAreaTL, Vertex2D vClientAreaBL, Vertex2D vClientAreaBR)
        {
            if (!Visible)
                return;

            if (m_selected)
            {
                pen = new Pen(m_selectedLineColor, pen.Width);
                brush = new SolidBrush(m_selectedFillColor);
            }

            for (int i = 0; i < m_positions.Count; i++)
            {
                PointF pt1 = new PointF(0, 0);
                PointF pt2 = new PointF(0, 0);

                if (i == 0)
                    continue;
                
                pt1 = new PointF((float)(m_positions[i - 1].x + m_dMoveX), (float)(m_positions[i - 1].y + m_dMoveY));
                pt2 = new PointF((float)(m_positions[i].x + m_dMoveX), (float)(m_positions[i].y + m_dMoveY));
                
                g.DrawLine(pen, pt1, pt2);
            }

            // Move 모드일때 양 각에 사각형 표시하기
            int size = 10;            
            if (m_bRectEditVertexVisible)
            {
                size = (int)(size / m_layer.FixedScale);
                foreach (KeyValuePair<int, Rectangle> item in m_rectEditVertex)
                {
                    g.FillRectangle(brush, (int)(item.Value.X + m_dMoveX), (int)(item.Value.Y + m_dMoveY), size, size);
                }       
            }
            
            //if (m_POIIcon != null)
            //{
            //    m_POIIcon.Render(g, pen, brush, vClientAreaTL, vClientAreaBL, vClientAreaBR);
                
            //}

            if (m_selected)
            {
                pen.Dispose();
                brush.Dispose();
            }
        }

        public override void Move(double dMoveX, double dMoveY)
        {
            m_dMoveX = dMoveX;
            m_dMoveY = dMoveY;
        }

        public override bool HitTest(Vertex2D vertex)
        {
            VariousData<double> snapDistance = m_layer.GetSnapDistance();

            if (snapDistance == null)
                return false;

            for (int i = 0; i < m_positions.Count; i++)
            {
                if (i == 0)
                    continue;

                Vertex2D pt1 = new Vertex2D(m_positions[i - 1].x + m_dMoveX, m_positions[i - 1].y + m_dMoveY);
                Vertex2D pt2 = new Vertex2D(m_positions[i].x + m_dMoveX, m_positions[i].y + m_dMoveY);

                Line2D line = new Line2D(pt1, pt2);
                
                double distance = line.GetDistance(vertex, false);

                if (distance < snapDistance.Data)
                    return true;

                //if (distance < 100)
                //    return true;
            }

            return false;
        }

        public string GetStrPosition(List<MakeWire> makeWire)
        {
            string strReturn = "";
            for (int i = 0; i < makeWire.Count; i++)
            {
                Vertex2D pt = null;
                if (makeWire[i].TargetPOI == null) // 빈 공간
                    pt = makeWire[i].targetVertex2D;
                else
                    pt = makeWire[i].TargetPOI.Position;

                strReturn += string.Format("{0},{1}", pt.x, pt.y);
                if (i < makeWire.Count - 1)
                    strReturn += ",";

                this.m_positions.Add(pt);
            }
            
            this.m_nLines = strReturn;
            return strReturn;
        }

        private Vertex2D m_iconPosition = null;
        public void SetIconPosition()
        {
            Vertex2D v1 = null;
            Vertex2D v2 = null;
            double sumLength = 0;
            for (int i = 1; i < m_positions.Count; i++)
            {
                v1 = new Vertex2D(m_positions[i - 1].x, m_positions[i - 1].y);
                v2 = new Vertex2D(m_positions[i].x, m_positions[i].y);

                sumLength += v1.GetDistance(v2);
            }

            double gg = 0;
            double leng = 0;

            for (int i = 1; i < m_positions.Count; i++)
            {
                v1 = new Vertex2D(m_positions[i - 1].x, m_positions[i - 1].y);
                v2 = new Vertex2D(m_positions[i].x, m_positions[i].y);

                double temp = v1.GetDistance(v2);
                if (leng + temp >= sumLength / 2)
                {
                    while (true)
                    {
                        gg++;
                        if (leng + gg >= sumLength / 2)
                            break;
                    }
                    break;
                }

                leng += temp;
            }

            if (m_POIIcon != null)
                m_POIIcon.Position = UnE.Geometry.Math.GetLinearVertex(v1, v2, gg);

            m_iconPosition = UnE.Geometry.Math.GetLinearVertex(v1, v2, gg);
            m_POIIcon.Angle = Angulo(v1.x, v1.y, v2.x, v2.y);
        }
        private double Angulo(double x1, double y1, double x2, double y2)
        {
            double degrees;

            // Avoid divide by zero run values.
            if (x2 - x1 == 0)
            {
                if (y2 > y1)
                    degrees = 90;
                else
                    degrees = 270;
            }
            else
            {
                // Calculate angle from offset.
                double riseoverrun = (double)(y2 - y1) / (double)(x2 - x1);
                double radians = System.Math.Atan(riseoverrun);
                degrees = radians * ((double)180 / System.Math.PI);

                // Handle quadrant specific transformations.       
                if ((x2 - x1) < 0 || (y2 - y1) < 0)
                    degrees += 180;
                if ((x2 - x1) > 0 && (y2 - y1) < 0)
                    degrees -= 180;
                if (degrees < 0)
                    degrees += 360;
            }
            return degrees;
        }

        public void SetRectVertex()
        {
            int size = 10;
            m_rectEditVertex.Clear();
            for (int i = 0; i < m_positions.Count; i++)
            {                
                if (i == 0 || i == m_positions.Count - 1) // 첫번째랑 마지막꺼 빼고
                    continue;
                double x = (double)(m_positions[i].x) - (size / m_layer.FixedScale) / 2; 
                double y = (double)(m_positions[i].y) - (size / m_layer.FixedScale) / 2;

                Rectangle rect = new Rectangle();
                rect.X = (int)x;
                rect.Y = (int)y;
                
                rect.Width = (int)(size / m_layer.FixedScale);
                rect.Height = (int)(size / m_layer.FixedScale);
                
                if (!m_rectEditVertex.ContainsKey(i))
                    m_rectEditVertex.Add(i, rect);
            }

            SetIconPosition();
        }

        public int GetRectVertex(Vertex2D vertex)
        {
            int i = 0;
            foreach (KeyValuePair<int, Rectangle> item in m_rectEditVertex)
            {
                if (item.Value.X <= vertex.x &&
                    item.Value.Y <= vertex.y &&
                    item.Value.X + item.Value.Width >= vertex.x &&
                    item.Value.Y + item.Value.Height >= vertex.y)
                {
                    return item.Key;
                }
                i++;
            }

            return -1;
        }
    }
    
    public class MakeWire
    {
        private POI m_targetPOI = null; // null이면 빈 영역
        private Vertex2D m_targetVertex2D = null;

        public POI TargetPOI
        {
            get { return m_targetPOI; }
            set { m_targetPOI = value; }
        }
        public Vertex2D targetVertex2D
        {
            get { return m_targetVertex2D; }
            set { m_targetVertex2D = value; }
        }
    }
}
