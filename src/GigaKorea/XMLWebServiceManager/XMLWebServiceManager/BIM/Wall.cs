using System;
using System.Collections.Generic;
using System.Text;
using UnE.Geometry;
using XMLWebServiceManager.Shapes;

namespace XMLWebServiceManager.BIM
{
    public class Wall : Shape
    {
        private class WallVertex : UnE.Geometry.Vertex2D
        {
            private object m_tag = null;

            public object Tag
            {
                get { return m_tag; }
                set { m_tag = value; }
            }

            public WallVertex()
            {
            }

            public WallVertex(UnE.Geometry.Vertex2D vertex)
                : base(vertex)
            {
            }

            public WallVertex(double x, double y)
                : base(x, y)
            {
            }
        }

        public enum GridType { Line = 0, Arc, EArc };

        //private List<Vertex2D> m_centerLineVertices = new List<Vertex2D>();
        private Polygon m_boundaryPolygon = null;

        private int m_nID = 0;
        private string m_strXMLID = "";
        private string m_strGridID = "";
        private double m_dThick = 0.0;
        private double m_dHeight = 0.0;
        private Component m_component = null;

        private Line2D m_line = null;
        private Arc2D m_arc = null;
        private EArc2D m_earc = null;
        private GridType m_gridType = GridType.Line;

        private List<Door> m_doors = new List<Door>();
        private List<Window> m_windows = new List<Window>();
        private List<Space> m_linkedSpaces = new List<Space>();

        private List<Property> m_properties = new List<Property>();

        // 벽체가 한쪽면만 공간과 연결되어 있을 경우 나머지 면의 외곽영역 계산을 위한 임시 데이터
        private List<PathItem> m_outsideBoundaryPath = null;
        private List<PathItem> m_boundary = null;
        //private GraphicsPath m_path = null;

        private double m_dMoveX = 0.0, m_dMoveY = 0.0;
        private Vertex2D m_vOriginTL = null;
        private Vertex2D m_vOriginBR = null;

        private Boundary m_boundaryData = null;

        public Component Component
        {
            get { return m_component; }
            set { m_component = value; }
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

        public string GridID
        {
            get { return m_strGridID; }
            set { m_strGridID = value; }
        }

        public double Thick
        {
            get { return m_dThick; }
            set { m_dThick = value; }
        }

        public double Height
        {
            get { return m_dHeight; }
            set { m_dHeight = value; }
        }

        public Line2D Line
        {
            get { return m_line; }
            set { m_line = value; }
        }

        public Arc2D Arc
        {
            get { return m_arc; }
            set { m_arc = value; }
        }

        public EArc2D EArc
        {
            get { return m_earc; }
            set { m_earc = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public List<Door> Doors
        {
            get { return m_doors; }
        }

        public List<Window> Windows
        {
            get { return m_windows; }
        }

        public GridType GetGridType()
        {
            return m_gridType;
        }

        public Boundary BoundaryData
        {
            get { return m_boundaryData; }
            set { m_boundaryData = value; }
        }

        public void SetGridType(int nGridType)
        {
            foreach (GridType type in Enum.GetValues(typeof(GridType)))
            {
                if (nGridType == (int)type)
                {
                    m_gridType = type;
                    break;
                }
            }
        }

        public Vertex2D GetBeginVertex()
        {
            Vertex2D vBegin = null;

            if (m_gridType == GridType.Line)
            {
                if (m_line != null)
                    vBegin = m_line.GetVertex(true);
            }
            else if (m_gridType == GridType.Arc)
            {
                if (m_arc != null)
                    vBegin = m_arc.GetBeginVertex();
            }
            else if (m_gridType == GridType.EArc)
            {
                if (m_earc != null)
                    vBegin = m_earc.GetBeginVertex();
            }

            return vBegin;
        }

        public Vertex2D GetEndVertex()
        {
            Vertex2D vEnd = null;

            if (m_gridType == GridType.Line)
            {
                if (m_line != null)
                    vEnd = m_line.GetVertex(false);
            }
            else if (m_gridType == GridType.Arc)
            {
                if (m_arc != null)
                    vEnd = m_arc.GetEndVertex();
            }
            else if (m_gridType == GridType.EArc)
            {
                if (m_earc != null)
                    vEnd = m_earc.GetEndVertex();
            }

            return vEnd;
        }

        public Vertex2D GetMiddleVertex()
        {
            Vertex2D vMiddle = null;

            if (m_gridType == GridType.Line)
            {
                if (m_line != null)
                {
                    vMiddle = (m_line.GetVertex(true) + m_line.GetVertex(false)) / 2;
                }
            }
            else if (m_gridType == GridType.Arc || m_gridType == GridType.EArc)
            {
                EArc2D earc = m_earc;

                if (m_gridType == GridType.Arc)
                    earc = m_arc;

                if (earc != null)
                {
                    Vertex2D vBegin = earc.GetBeginVertex();
                    Vertex2D vEnd = earc.GetEndVertex();

                    if (earc.GetVertex(earc.GetBeginAngle() + earc.GetAngle() / 2, out vMiddle) == false)
                        return null;
                }
            }

            return vMiddle;
        }

        public void AddSpace(Space space)
        {
            if (m_linkedSpaces.Contains(space) == false)
                m_linkedSpaces.Add(space);
        }

        public int GetLinkedSpaceCount()
        {
            return m_linkedSpaces.Count;
        }

        public void AddDoor(Door door)
        {
            m_doors.Add(door);
            door.Wall = this;
        }

        public void AddWindow(Window window)
        {
            m_windows.Add(window);
            window.Wall = this;
        }
    }
}
