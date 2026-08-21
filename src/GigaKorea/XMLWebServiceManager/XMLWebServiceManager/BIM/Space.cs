using System;
using System.Collections.Generic;
using System.Text;
using UnE.Geometry;
using XMLWebServiceManager.Shapes;

namespace XMLWebServiceManager.BIM
{
    public class Space : Shape
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private string m_strName = "";
        private List<Wall> m_walls = new List<Wall>();

        private List<Property> m_properties = new List<Property>();

        private Level m_level = null;

        private Boundary m_boundaryData = null;
        public Boundary BoundaryData
        {
            get { return m_boundaryData; }
            set { m_boundaryData = value; }
        }

        private List<Boundary> m_holeBoundary = null;
        public List<Boundary> HoleBoundary
        {
            get { return m_holeBoundary; }
            set { m_holeBoundary = value; }
        }

        // 방화구획
        private bool m_safetyFire = false;

        public const string SafetyFireTag = "IsSafetyFire";

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

        public bool SafetyFire
        {
            get { return m_safetyFire; }
            set { m_safetyFire = value; }
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

        public List<Wall> Walls
        {
            get { return m_walls; }
        }

        public void AddWall(Wall wall)
        {
            m_walls.Add(wall);
            wall.AddSpace(this);
        }
    }
}
