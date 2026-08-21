using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace CCTVLocation
{
    public class Zone
    {
        private string m_strZoneName = "";
        private List<Polygon> m_polygons = new List<Polygon>();
        private int m_nZoneID = -1;
        private int m_nBuildingID = -1;

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public List<Polygon> Polygons
        {
            get { return m_polygons; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public int BuildingID
        {
            get { return m_nBuildingID; }
            set { m_nBuildingID = value; }
        }
    }

    public class OutdoorZone
    {
        private string m_strZoneName = "";
        private List<Vertex2D> m_vertices = new List<Vertex2D>();
        private int m_nZoneID = -1;
        private DXFViewer.PolyLine m_polyline = null;
        private EdgeLineHatch m_hatch = null;

        public string ZoneName
        {
            get { return m_strZoneName; }
            set { m_strZoneName = value; }
        }

        public List<Vertex2D> Vertices
        {
            get { return m_vertices; }
        }

        public int ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public DXFViewer.PolyLine PolyLine
        {
            get { return m_polyline; }
            set { m_polyline = value; }
        }

        public EdgeLineHatch Hatch
        {
            get { return m_hatch; }
            set { m_hatch = value; }
        }

        public string GetBoundaryString()
        {
            string strBoundary = "";

            foreach (Vertex2D vertex in m_vertices)
            {
                if (strBoundary.Length == 0)
                    strBoundary = vertex.x.ToString() + "," + vertex.y.ToString();
                else
                    strBoundary += "," + vertex.x.ToString() + "," + vertex.y.ToString();
            }

            return strBoundary;
        }
    }

    public class EdgeLineHatch : DXFViewer.Hatch
    {
        private System.Drawing.Color m_edgeLineColor = System.Drawing.Color.Red;
        private System.Drawing.PointF[] m_arrPoints = null;
        private Polygon m_polygon = null;
        private bool m_hiLight = false;

        System.Drawing.SolidBrush m_brushHiLight = new System.Drawing.SolidBrush(System.Drawing.Color.BlanchedAlmond);

        public bool HiLight
        {
            get { return m_hiLight; }
            set { m_hiLight = value; }
        }

        public void Done()
        {
            int nPointSize = this.GetPointSize();

            if (nPointSize == 0)
                return;

            m_arrPoints = new System.Drawing.PointF[nPointSize + 1];
            m_polygon = new Polygon();

            float x, y;

            for (int i = 0; i < nPointSize; i++)
            {
                if (!this.GetPoint(i, out x, out y))
                {
                    return;
                }

                m_arrPoints[i].X = x;
                m_arrPoints[i].Y = y;

                m_polygon.AddVertex(new Vertex2D(x, y));
            }

            m_arrPoints[nPointSize] = m_arrPoints[0];
        }

        public override bool Draw(System.Drawing.Graphics g, bool bDrawText)
        {
            bool result = FormMain.Instance.TransparentZone ? true : base.Draw(g, bDrawText);
            
            if (!result)
                return false;

            if (m_arrPoints == null)
                return result;

            System.Drawing.Pen pen = m_lineType.GetPen();
            pen.Width = 10.0f;

	        System.Drawing.Color colorOld = pen.Color;

            pen.Color = m_edgeLineColor;
            g.DrawLines(pen, m_arrPoints);
			pen.Color = colorOld;

            if (m_hiLight)
            {
                System.Drawing.Color oldColor = m_brushHiLight.Color;
                m_brushHiLight.Color = System.Drawing.Color.FromArgb(100, 255 - oldColor.R, 255 - oldColor.G, 255 - oldColor.B);
                g.FillPolygon(m_brushHiLight, m_arrPoints);
                m_brushHiLight.Color = oldColor;
            }

            return result;
        }

        public Polygon GetPolygon()
        {
            return m_polygon;
        }
    }
}
