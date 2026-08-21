using System;
using System.Collections.Generic;
using System.Text;
using UnE.Geometry;
using XMLWebServiceManager.BIM;

namespace XMLWebServiceManager.Shapes
{
    public class POI : Shape
    {
        public enum DrawType { Circle, Rect, Triangle, Image };

        private int m_nID = 0;
        private string m_strXMLID = "";
        //private string m_strPOIID = "";
        private string m_strPOIName = "";

        //private Color m_fillColor = Color.Red;
        // Pixel
        private int m_nHeight = 200;
        private Vertex2D m_vPos = new Vertex2D();
        private DrawType m_drawType = DrawType.Circle;
        private POIType m_poiType = null;
        private double m_dMoveX = 0.0, m_dMoveY = 0.0;

        private double m_dAngle = 0.0;

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

        public string Name
        {
            get { return m_strPOIName; }
            set { m_strPOIName = value; }
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

        public double Angle
        {
            get { return m_dAngle; }
            set { m_dAngle = value; }
        }

        public List<BIM.Property> Properties
        {
            get { return m_properties; }
        }

        public POIType PoiType
        {
            get { return m_poiType; }
            set { m_poiType = value; }
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

    public class POIType
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private string m_strName = "";
        private bool m_userDefined = false;
        private int m_nParentID = 0;
        private POIType m_parent = null;
        private List<POIType> m_childTypes = new List<POIType>();
        private string m_strCode = "";
        private bool m_bIsGroup = false;
        //private Color m_color = Color.Yellow;
        private string m_strDefaultHeight = null;

        private List<Property> m_properties = new List<Property>();

        public const string POITypeIDTag = "pt";

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

        public bool UserDefined
        {
            get { return m_userDefined; }
            set { m_userDefined = value; }
        }

        public int ParentID
        {
            get { return m_nParentID; }
            set { m_nParentID = value; }
        }

        public POIType Parent
        {
            get { return m_parent; }
            set
            {
                m_parent = value;

                if (m_parent != null && m_parent.m_childTypes.Contains(this) == false)
                    m_parent.m_childTypes.Add(this);
            }
        }


        public List<POIType> ChildTypes
        {
            get { return m_childTypes; }
        }

        public string DefaultHeight
        {
            get { return m_strDefaultHeight; }
            set { m_strDefaultHeight = value; }
        }


        public string Code
        {
            get { return m_strCode; }
            set { m_strCode = value; }
        }

        public bool IsGroup
        {
            get { return m_bIsGroup; }
            set { m_bIsGroup = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

    }

}
