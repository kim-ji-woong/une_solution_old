using System;
using System.Collections.Generic;
using System.Text;
using UnE.Geometry;
using XMLWebServiceManager.Shapes;

namespace XMLWebServiceManager.BIM
{
    public class Column : Shape
    {
        public class Rect
        {
            private Vertex2D m_vTL = null;
            private Vertex2D m_vBL = null;
            private Vertex2D m_vBR = null;
            //private GraphicsPath m_path = new GraphicsPath();
            private Polygon m_boundaryPolygon = new Polygon();

            public Vertex2D TopLeft
            {
                get { return m_vTL; }
                set { m_vTL = value; }
            }

            public Vertex2D BottomLeft
            {
                get { return m_vBL; }
                set { m_vBL = value; }
            }

            public Vertex2D BottomRight
            {
                get { return m_vBR; }
                set { m_vBR = value; }
            }
        }

        public class Circle
        {
            private Vertex2D m_vCenter = null;
            private Vertex2D m_vMovedCenter = null;
            private double m_dRadius = 0.0;


            public Vertex2D Center
            {
                get { return m_vCenter; }
                set { m_vCenter = value; }
            }

            public double Radius
            {
                get { return m_dRadius; }
                set { m_dRadius = value; }
            }

        }

        public enum ColumnType { Rect = 0, Circle };

        private int m_nID = 0;
        private string m_strXMLID = "";

        private ColumnType m_type = ColumnType.Rect;
        private Rect m_rect = null;
        private Circle m_circle = null;
        private List<Property> m_properties = new List<Property>();

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

        public ColumnType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public Rect RectData
        {
            get { return m_rect; }
            set { m_rect = value; }
        }

        public Circle CircleData
        {
            get { return m_circle; }
            set { m_circle = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }
    }
}
