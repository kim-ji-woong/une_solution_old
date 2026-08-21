using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnE.Geometry;
using XMLWebServiceManager.Shapes;

namespace XMLWebServiceManager.BIM
{
    public class Level : IComparable
    {
        private class Coord2D
        {
            private class NullChecker
            {
                public static bool IsNull(Object obj)
                {
                    return obj == null;
                }
            }

            private long x = 0;
            private long y = 0;
            private int m_nHashCode = 0;

            public long X
            {
                get { return x; }
                set
                {
                    x = value;
                    SetHashCode();
                }
            }

            public long Y
            {
                get { return y; }
                set
                {
                    y = value;
                    SetHashCode();
                }
            }

            public Coord2D()
            {
                SetHashCode();
            }

            public Coord2D(long x, long y)
            {
                this.x = x;
                this.y = y;
                SetHashCode();
            }

            private void SetHashCode()
            {
                string str = x.ToString() + "_" + y.ToString();
                m_nHashCode = str.GetHashCode();
            }

            public static bool operator ==(Coord2D op1, Coord2D op2)
            {
                bool isNull1 = NullChecker.IsNull(op1);
                bool isNull2 = NullChecker.IsNull(op2);

                if (isNull1 == false && isNull2 == false)
                    return op1.Equals(op2);

                return false;
            }

            public static bool operator !=(Coord2D op1, Coord2D op2)
            {
                bool isNull1 = NullChecker.IsNull(op1);
                bool isNull2 = NullChecker.IsNull(op2);

                if (isNull1 == false && isNull2 == false)
                    return !op1.Equals(op2);

                return true;
            }

            public override bool Equals(object obj)
            {
                if (NullChecker.IsNull(obj))
                    return false;

                if (obj is Coord2D)
                {
                    Coord2D coord = (Coord2D)obj;

                    if (this.x == coord.x && this.y == coord.y)
                        return true;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return m_nHashCode;
            }
        }

        private int m_nID = 0;
        private string m_strXMLID = "";
        private int m_nFloorIndex = 0;
        private string m_strName = "";
        private float m_fElevation = 0.0f;

        private Dictionary<int, Wall> m_dicWalls = new Dictionary<int, Wall>();
        private Dictionary<int, Space> m_dicSpaces = new Dictionary<int, Space>();
        private Dictionary<int, AlertArea> m_dicAlertAreas = new Dictionary<int, AlertArea>();
        private Dictionary<int, Shapes.POI> m_dicPOIs = new Dictionary<int, POI>();
        private Dictionary<int, Shapes.Wire> m_dicWires = new Dictionary<int, Wire>();
        // 벽체들간의 연결관계를 기억시키기 위하여, 벽체의 끝점을 기준으로 끝점과 연결된 벽체들의 리스트를 저장한다.
        private Dictionary<Coord2D, List<Wall>> m_dicCoordWalls = new Dictionary<Coord2D, List<Wall>>();
        private List<Column> m_columns = new List<Column>();

        private List<Topology> m_topologies = new List<Topology>();
        private List<Property> m_properties = new List<Property>();

        private bool m_loadingDatas = false;

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

        public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public float Elevation
        {
            get { return m_fElevation; }
            set { m_fElevation = value; }
        }

        public bool CompleteLoading
        {
            get { return m_loadingDatas; }
        }

        public static int RoofFloorIndex
        {
            get { return 10000; }
        }

        public List<Topology> Topologies
        {
            get { return m_topologies; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public List<Wall> Walls
        {
            get { return m_dicWalls.Values.ToList(); }
        }

        public List<Space> Spaces
        {
            get { return m_dicSpaces.Values.ToList(); }
        }

        public List<AlertArea> AlertAreas
        {
            get { return m_dicAlertAreas.Values.ToList(); }
        }

        public List<Shapes.POI> POIs
        {
            get { return m_dicPOIs.Values.ToList(); }
        }

        public List<Shapes.Wire> Wires
        {
            get { return m_dicWires.Values.ToList(); }
        }

        public List<Column> Columns
        {
            get { return m_columns; }
        }

        public override string ToString()
        {
            string str = "";

            if (m_nFloorIndex == RoofFloorIndex)
                str = "지붕";
            else if (m_nFloorIndex < 0)
                str = "지하 " + (-m_nFloorIndex).ToString();
            else
                str = m_nFloorIndex.ToString();

            if (m_strName.Length == 0)
                return str + "층";

            return str + "층(" + m_strName + ")";
        }

        public void AddWall(Wall wall)
        {
            m_dicWalls[wall.ID] = wall;

            Vertex2D vBegin = wall.GetBeginVertex();
            Vertex2D vEnd = wall.GetEndVertex();

            Coord2D cBegin = new Coord2D((long)vBegin.x, (long)vBegin.y);
            Coord2D cEnd = new Coord2D((long)vEnd.x, (long)vEnd.y);

            List<Wall> walls = null;

            if (m_dicCoordWalls.TryGetValue(cBegin, out walls) == false)
            {
                walls = new List<Wall>();
                m_dicCoordWalls[cBegin] = walls;
            }

            walls.Add(wall);

            if (m_dicCoordWalls.TryGetValue(cEnd, out walls) == false)
            {
                walls = new List<Wall>();
                m_dicCoordWalls[cEnd] = walls;
            }

            walls.Add(wall);
        }

        public void RemoveWall(Wall wall)
        {
            m_dicWalls.Remove(wall.ID);
        }

        public Wall FindWall(int nWallID)
        {
            Wall wall = null;
            m_dicWalls.TryGetValue(nWallID, out wall);
            return wall;
        }

        public List<Wall> GetLinkedWall(Wall wall, bool isBegin)
        {
            if (wall == null)
                return null;

            Vertex2D vertex = isBegin ? wall.GetBeginVertex() : wall.GetEndVertex();
            Coord2D coord = new Coord2D((long)vertex.x, (long)vertex.y);

            List<Wall> walls = null;
            m_dicCoordWalls.TryGetValue(coord, out walls);
            return walls;
        }

        public void AddSpace(Space space)
        {
            m_dicSpaces[space.ID] = space;
        }

        public void RemoveSpace(Space space)
        {
            m_dicSpaces.Remove(space.ID);
        }

        public void AddAlertArea(AlertArea alertArea)
        {
            m_dicAlertAreas[alertArea.ID] = alertArea;
        }

        public void RemoveAlertArea(AlertArea alertArea)
        {
            m_dicAlertAreas.Remove(alertArea.ID);
        }

        public Space FindSpace(int nSpaceID)
        {
            Space space = null;
            m_dicSpaces.TryGetValue(nSpaceID, out space);
            return space;
        }

        public void AddPOI(POI poi)
        {
            m_dicPOIs[poi.ID] = poi;
        }

        public void RemovePOI(POI poi)
        {
            m_dicPOIs.Remove(poi.ID);
        }

        public POI FindPOI(int nPOIID)
        {
            POI poi = null;
            m_dicPOIs.TryGetValue(nPOIID, out poi);
            return poi;
        }

        public void AddWire(Wire wire)
        {
            m_dicWires[wire.ID] = wire;
        }

        public void RemoveWire(Wire wire)
        {
            m_dicWires.Remove(wire.ID);
        }

        public int CompareTo(object obj)
        {
            Level level = (Level)obj;
            return this.m_fElevation.CompareTo(level.m_fElevation);
        }
    }
}
