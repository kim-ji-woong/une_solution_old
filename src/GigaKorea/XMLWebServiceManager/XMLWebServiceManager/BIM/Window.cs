using System;
using System.Collections.Generic;
using System.Text;
using UnE.Geometry;
using XMLWebServiceManager.Shapes;

namespace XMLWebServiceManager.BIM
{
    public class Window : Shape
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private float m_fWidth = 0.0f;
        private float m_fHeight = 0.0f;
        private float m_fElevation = 0.0f;
        private float m_fThick = 100.0f;
        private Vertex2D m_vPos = null;
        private Wall m_wall = null;

        private List<Property> m_properties = new List<Property>();

        //private GraphicsPath m_path = null;
        //private Polygon m_boundaryPolygon = new Polygon();

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

        public Vertex2D Position
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

        public float Elevation
        {
            get { return m_fElevation; }
            set { m_fElevation = value; }
        }

        public Wall Wall
        {
            get { return m_wall; }
            set { m_wall = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public float Thick
        {
            get { return m_fThick; }
            set { m_fThick = value; }
        }
    }
}
