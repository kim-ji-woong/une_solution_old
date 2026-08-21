using System;
using System.Collections.Generic;
using System.Text;
using UnE.Geometry;
using XMLWebServiceManager.Shapes;

namespace XMLWebServiceManager.BIM
{
    public class AlertArea : Shape
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private string m_strName = "";

        private List<Property> m_properties = new List<Property>();
        private Level m_level = null;

        private Polygon m_polygon = new Polygon();

        //private GraphicsPath m_path = null;
        private Vertex2D m_vNamePosition = null;

        private Vertex2D m_vOriginTL = null;
        private Vertex2D m_vOriginBR = null;

        private Boundary m_boundary = null;
        public Boundary Boundary
        {
            get { return m_boundary; }
            set { m_boundary = value; }
        }

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

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public Level Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public Vertex2D NamePosition
        {
            get { return m_vNamePosition; }
            set { m_vNamePosition = value; }
        }
    }
}
