using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Geometry;

namespace CadToXML
{
    public class Material
    {
        private static int m_nInstanceCount = 0;
        private static List<Material> m_materials = new List<Material>();

        private int m_nID = 0;
        private string m_strTypeName = "Type1";
        private string m_strMaterialName = "";

        public virtual string ID
        {
            get { return "component" + m_nID.ToString(); }
            set { }
        }

        public string TypeName
        {
            get { return m_strTypeName; }
            set { m_strTypeName = value; }
        }

        public string MaterialName
        {
            get { return m_strMaterialName; }
            set { m_strMaterialName = value; }
        }

        public static List<Material> Materials
        {
            get { return m_materials; }
        }

        public Material()
        {
            m_nID = ++m_nInstanceCount;
            m_materials.Add(this);
        }

        public Material(string strTypeName, string strMaterialName)
        {
            m_nID = ++m_nInstanceCount;
            m_materials.Add(this);

            m_strTypeName = strTypeName;
            m_strMaterialName = strMaterialName;
        }
    }

    public class Component : Material
    {
        private string m_strID = "";

        public override string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }
    }

    public class Door
    {
        // 미닫이 : 옆으로 밀고 닫는문
        // 여닫이 : 앞뒤로 밀거나 당겨서 여는 문
        // 미닫이문, 한방향 외여닫이문, 양방향 외여닫이문, 한방향 쌍여닫이문, 양방향 쌍여닫이문
        public enum DoorType { Sliding = 0, Hinged, Hinged2, DoubleHinged, DoubleHinged2 };

        private string m_strID = "";
        private double m_x = 0.0;
        private double m_y = 0.0;
        private double m_dWidth = 0.0;
        private double m_dHeight = 0.0;
        private double m_dElevation = 0.0;
        private double m_dThick = 50.0;
        // Angle(Degree)
        //private double m_dDirection = 0.0;
        private DoorType m_type = DoorType.Hinged;
        private UnE.Geometry.Vertex2D m_vHinge1 = null;
        private UnE.Geometry.Vertex2D m_vHinge2 = null;

        //ym0729
        private List<Property> m_properties = new List<Property>();
        //ym0729
        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public double Width
        {
            get { return m_dWidth; }
            set { m_dWidth = value; }
        }

        public double Height
        {
            get { return m_dHeight; }
            set { m_dHeight = value; }
        }

        public double Elevation
        {
            get { return m_dElevation; }
            set { m_dElevation = value; }
        }

        // Angle(Degree)
        /*public double Direction
        {
            get { return m_dDirection; }
            set { m_dDirection = value; }
        }*/

        public UnE.Geometry.Vertex2D Hinge1
        {
            get { return m_vHinge1; }
            set { m_vHinge1 = value; }
        }

        public UnE.Geometry.Vertex2D Hinge2
        {
            get { return m_vHinge2; }
            set { m_vHinge2 = value; }
        }

        public DoorType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public double Thick
        {
            get { return m_dThick; }
            set { m_dThick = value; }
        }

        public void Move(double x, double y)
        {
            m_x += x;
            m_y += y;

            if (m_vHinge1 != null)
            {
                m_vHinge1.x += x;
                m_vHinge1.y += y;
            }

            if (m_vHinge2 != null)
            {
                m_vHinge2.x += x;
                m_vHinge2.y += y;
            }
        }

        public void SetScale(double dScale)
        {
            m_x *= dScale;
            m_y *= dScale;
            m_dWidth *= dScale;

            if (m_vHinge1 != null)
            {
                m_vHinge1.x *= dScale;
                m_vHinge1.y *= dScale;
            }

            if (m_vHinge2 != null)
            {
                m_vHinge2.x *= dScale;
                m_vHinge2.y *= dScale;
            }
        }
    }

    public class Window
    {
        private string m_strID = "";
        private double m_x = 0.0;
        private double m_y = 0.0;
        private double m_dWidth = 0.0;
        private double m_dHeight = 0.0;
        private double m_dElevation = 0.0;
        private double m_dThick = 100.0;

        private List<Property> m_properties = new List<Property>();

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public double Width
        {
            get { return m_dWidth; }
            set { m_dWidth = value; }
        }

        public double Height
        {
            get { return m_dHeight; }
            set { m_dHeight = value; }
        }

        public double Elevation
        {
            get { return m_dElevation; }
            set { m_dElevation = value; }
        }

        public double Thick
        {
            get { return m_dThick; }
            set { m_dThick = value; }
        }

        public void Move(double x, double y)
        {
            m_x += x;
            m_y += y;
        }

        public void SetScale(double dScale)
        {
            m_x *= dScale;
            m_y *= dScale;
            m_dWidth *= dScale;
        }
    }

    public partial class Wall
    {
        // 구조벽, 가벽, 커튼월, NoSpace
        // NoSpace는 문과 벽체만 만들고 공간 생성에는 참여하지 않는다.
        public enum WallType { Structure = 0, Fake, Partition, Handrail, NoSpace, CurtainWall };

        private string m_strID = "";
        private double m_dThick = 0.0;
        private double m_dHeight = 0.0;
        private UnE.Geometry.Vertex2D m_vBegin = null;
        private UnE.Geometry.Vertex2D m_vEnd = null;
        // 빠른 좌표 비교를 위하여 m_vBegin과 m_vEnd를 정수로 변환시켜 둔다.
        private long m_nBegin = 0, m_nEnd = 0;
        private UnE.Geometry.Line2D m_line = new UnE.Geometry.Line2D();
        private WallType m_wallType = WallType.Structure;
        private Material m_material = null;
        private List<Door> m_doors = new List<Door>();
        private List<Window> m_windows = new List<Window>();
        private Space m_linkedSpace1 = null;
        private Space m_linkedSpace2 = null;

        //ym0729
        private List<Property> m_properties = new List<Property>();
        //ym0729
        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public UnE.Geometry.Vertex2D Begin
        {
            get { return m_vBegin; }
            set
            {
                m_vBegin = value;
                m_nBegin = VertexToLong(m_vBegin);
            }
        }

        public UnE.Geometry.Vertex2D End
        {
            get { return m_vEnd; }
            set
            {
                m_vEnd = value;
                m_nEnd = VertexToLong(m_vEnd);
            }
        }

        public UnE.Geometry.Line2D Line
        {
            get
            {
                m_line.SetVertex(m_vBegin, true);
                m_line.SetVertex(m_vEnd, false);
                return m_line;
            }
        }

        public WallType Type
        {
            get { return m_wallType; }
            set { m_wallType = value; }
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

        public Material Material
        {
            get { return m_material; }
            set { m_material = value; }
        }

        public List<Door> Doors
        {
            get { return m_doors; }
        }

        public List<Window> Windows
        {
            get { return m_windows; }
        }
        

        public Space LinkedSpace1
        {
            get { return m_linkedSpace1; }
            set { m_linkedSpace1 = value; }
        }

        public Space LinkedSpace2
        {
            get { return m_linkedSpace2; }
            set { m_linkedSpace2 = value; }
        }

        public void AddLinkedSpace(Space space)
        {
            if (m_linkedSpace1 == null)
                m_linkedSpace1 = space;
            else if (m_linkedSpace1 != space)
                m_linkedSpace2 = space;
        }

        public void Move(double x, double y)
        {
            foreach (Door door in m_doors)
            {
                door.Move(x, y);
            }

            foreach (Window window in m_windows)
            {
                window.Move(x, y);
            }

            Begin.SetVertex(Begin.x + x, Begin.y + y);
            End.SetVertex(End.x + x, End.y + y);
        }

        public void SetScale(double dScale)
        {
            foreach (Door door in m_doors)
            {
                door.SetScale(dScale);
            }

            foreach (Window window in m_windows)
            {
                window.SetScale(dScale);
            }

            Begin *= dScale;
            End *= dScale;
        }

        public static long VertexToLong(UnE.Geometry.Vertex2D vertex)
        {
            if (vertex == null)
                return 0;

            long x = (long)(vertex.x + 0.5);
            long y = (long)(vertex.y + 0.5);

            long key = ((x << 32) | y);
            return key;
        }

        // LineType
        public bool IsSame(UnE.Geometry.Vertex2D vBegin, UnE.Geometry.Vertex2D vEnd, long nBegin, long nEnd, WallType type)
        {
            if (m_wallType != type)
                return false;

            if (m_nBegin == nBegin && m_nEnd == nEnd)
            {
                if (m_vBegin.GetDistance(vBegin) <= UnE.Geometry.Math.HALF_TOLERANCE() && m_vEnd.GetDistance(vEnd) <= UnE.Geometry.Math.HALF_TOLERANCE())
                    return true;
            }
            else if (m_nBegin == nEnd && m_nEnd == nBegin)
            {
                if (m_vBegin.GetDistance(vEnd) <= UnE.Geometry.Math.HALF_TOLERANCE() && m_vEnd.GetDistance(vBegin) <= UnE.Geometry.Math.HALF_TOLERANCE())
                    return true;
            }

            return false;
        }

        // ArcType
        public bool IsSame(Arc2D arc, WallType type)
        {
            if (m_wallType != type)
                return false;

            if (m_arc == null || m_gridType != GridType.Arc)
                return false;

            if (m_arc.GetCenter().GetDistance(arc.GetCenter()) > UnE.Geometry.Math.HALF_TOLERANCE())
                return false;

            if ((System.Math.Abs(arc.GetBeginAngle() - m_arc.GetBeginAngle()) > UnE.Geometry.Math.HALF_TOLERANCE()) ||
                (System.Math.Abs(arc.GetAngle() - m_arc.GetAngle()) > UnE.Geometry.Math.HALF_TOLERANCE()) ||
                (System.Math.Abs(arc.GetRadius() - m_arc.GetRadius()) > UnE.Geometry.Math.HALF_TOLERANCE()))
                return false;

            return true;
        }
    }

    public partial class Space
    {
        public enum SpaceType { Normal = 0, StairRoom, EscalatorRoom, ElevatorRoom };

        private string m_strID = "";
        private string m_strSpaceName = "";
        private SpaceType m_type = SpaceType.Normal;
        private List<Wall> m_walls = new List<Wall>();
        private UnE.Geometry.Polygon m_polygon = null;

        //ym.0826
        private List<Property> m_properties = new List<Property>();
        //ym.0826
        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string Name
        {
            get { return m_strSpaceName; }
            set { m_strSpaceName = value; }
        }

        public SpaceType Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public List<Wall> Walls
        {
            get { return m_walls; }
        }

        public UnE.Geometry.Polygon GetPolygon()
        {
            if (m_polygon != null && m_polygon.GetVertexCount() > 0)
                return m_polygon;

            Vertex2D vBegin, vMiddle, vEnd;
            UnE.Geometry.Polygon polygon = new UnE.Geometry.Polygon();

            foreach (PathItem item in Boundary)
            {
                item.GetVertex(out vBegin, out vEnd, out vMiddle);

                if (item.GetDrawType() == PathItem.DrawType.Line)
                {
                    polygon.AddVertex(new Vertex2D(vEnd.x, vEnd.y));
                }
                else if (item.GetDrawType() == PathItem.DrawType.Arc || item.GetDrawType() == PathItem.DrawType.EArc)
                {
                    polygon.AddVertex(new Vertex2D(vMiddle.x, vMiddle.y));
                    polygon.AddVertex(new Vertex2D(vEnd.x, vEnd.y));
                }
            }

            /*UnE.Geometry.Vertex2D vNext = null;

            for (int i = 0; i < m_walls.Count; i++)
            {
                Wall wall = m_walls[i];

                if (vNext == null)
                {
                    Wall wall2 = m_walls[i + 1];

                    if (wall.End.GetDistance(wall2.Begin) < 0.1 || wall.End.GetDistance(wall2.End) < 0.1)
                    {
                        polygon.AddVertex(wall.Begin);
                        vNext = wall.End;
                    }
                    else
                    {
                        polygon.AddVertex(wall.End);
                        vNext = wall.Begin;
                    }
                }
                else
                {
                    if (vNext.GetDistance(wall.Begin) < 0.1)
                    {
                        polygon.AddVertex(wall.Begin);
                        vNext = wall.End;
                    }
                    else
                    {
                        polygon.AddVertex(wall.End);
                        vNext = wall.Begin;
                    }
                }
            }

            if (vNext != null)
                polygon.AddVertex(vNext);*/

            m_polygon = polygon;
            return m_polygon;
        }
    }

    public partial class AlertArea
    {
        private string m_strID = "";
        private string m_strAlertAreaName = "";
        private UnE.Geometry.Polygon m_polygon = null;

        private List<Property> m_properties = new List<Property>();
        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string Name
        {
            get { return m_strAlertAreaName; }
            set { m_strAlertAreaName = value; }
        }

        public void Move(double x, double y)
        {
            // 이동된 거리만큼 바운더리 값도 변경
            foreach (PathItem item in Boundary)
            {
                // Arc, EArc도 추후 구현 필요.
                if (item.GetDrawType() == PathItem.DrawType.Line)
                {
                    Vertex2D vBegin, vMiddle, vEnd;
                    item.GetVertex(out vBegin, out vEnd, out vMiddle);

                    double dBeginX = vBegin.x + x;
                    double dBeginY = vBegin.y + y;
                    vBegin = new Vertex2D(dBeginX, dBeginY);

                    double dEndX = vEnd.x + x;
                    double dEndY = vEnd.y + y;
                    vEnd = new Vertex2D(dEndX, dEndY);

                    item.SetLine(new Line2D(vBegin, vEnd));
                }
                else if (item.GetDrawType() == PathItem.DrawType.Arc)
                {
                    
                }
            }
        }

        public UnE.Geometry.Polygon GetPolygon()
        {
            if (m_polygon != null && m_polygon.GetVertexCount() > 0)
                return m_polygon;

            Vertex2D vBegin, vMiddle, vEnd;
            UnE.Geometry.Polygon polygon = new UnE.Geometry.Polygon();

            foreach (PathItem item in Boundary)
            {
                item.GetVertex(out vBegin, out vEnd, out vMiddle);

                if (item.GetDrawType() == PathItem.DrawType.Line)
                {
                    polygon.AddVertex(new Vertex2D(vEnd.x, vEnd.y));
                }
                else if (item.GetDrawType() == PathItem.DrawType.Arc || item.GetDrawType() == PathItem.DrawType.EArc)
                {
                    polygon.AddVertex(new Vertex2D(vMiddle.x, vMiddle.y));
                    polygon.AddVertex(new Vertex2D(vEnd.x, vEnd.y));
                }
            }

            m_polygon = polygon;
            return m_polygon;
        }
    }

    public class Topology
    {
        public class Node
        {
            public enum NodeType { None = 0, Door, Space };

            private string m_strID = "";
            private double m_dX = 0.0;
            private double m_dY = 0.0;
            private List<Node> m_linkedNodes = new List<Node>();
            private List<Property> m_properties = new List<Property>();
            private NodeType m_nodeType = NodeType.None;
            private object m_owner = null;

            public List<Property> Properties
            {
                get { return m_properties; }
            }

            public string ID
            {
                get { return m_strID; }
                set { m_strID = value; }
            }

            public double X
            {
                get { return m_dX; }
                set { m_dX = value; }
            }

            public double Y
            {
                get { return m_dY; }
                set { m_dY = value; }
            }

            public List<Node> LinkedNodes
            {
                get { return m_linkedNodes; }
            }

            public NodeType Type
            {
                get { return m_nodeType; }
                set { m_nodeType = value; }
            }

            public object Owner
            {
                get { return m_owner; }
                set { m_owner = value; }
            }

            public string GetOwnerType()
            {
                if (m_nodeType == NodeType.Door)
                    return "Door";
                else if (m_nodeType == NodeType.Space)
                    return "Space";

                return "None";
            }

            public void SetOwnerType(string strOwnerType)
            {
                if (string.Compare(strOwnerType, "Door", true) == 0)
                    m_nodeType = NodeType.Door;
                else if (string.Compare(strOwnerType, "Space", true) == 0)
                    m_nodeType = NodeType.Space;
                else
                    m_nodeType = NodeType.None;
            }

            public string GetOwnerID()
            {
                if (m_owner == null)
                    return null;

                if (m_owner is Space)
                {
                    Space space = (Space)m_owner;
                    return space.ID;
                }
                else if (m_owner is Door)
                {
                    Door door = (Door)m_owner;
                    return door.ID;
                }

                return null;
            }
        }

        private string m_strID = "";
        private List<Node> m_nodes = new List<Node>();
        private List<Property> m_properties = new List<Property>();

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public List<Node> Nodes
        {
            get { return m_nodes; }
        }

        public void Move(double x, double y)
        {
            foreach (Node node in m_nodes)
            {
                node.X += x;
                node.Y += y;
            }
        }

        public void SetScale(double dScale)
        {
            foreach (Node node in m_nodes)
            {
                node.X *= dScale;
                node.Y *= dScale;
            }
        }
    }

    public abstract class Column
    {
        private string m_strID = "";

        //ym0729
        private List<Property> m_properties = new List<Property>();
        //ym0729
        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public abstract void Move(double x, double y);
        public abstract void SetScale(double dScale);
    }

    public class RectColumn : Column
    {
        private Vertex2D m_vTL = null;
        private Vertex2D m_vBL = null;
        private Vertex2D m_vBR = null;

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

        public override void Move(double x, double y)
        {
            m_vTL.SetVertex(m_vTL.x + x, m_vTL.y + y);
            m_vBL.SetVertex(m_vBL.x + x, m_vBL.y + y);
            m_vBR.SetVertex(m_vBR.x + x, m_vBR.y + y);
        }

        public override void SetScale(double dScale)
        {
            m_vTL *= dScale;
            m_vBL *= dScale;
            m_vBR *= dScale;
        }
    }

    public class CircleColumn : Column
    {
        private Vertex2D m_vCenter = null;
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

        public override void Move(double x, double y)
        {
            m_vCenter.SetVertex(m_vCenter.x + x, m_vCenter.y + y);
        }

        public override void SetScale(double dScale)
        {
            m_vCenter *= dScale;
            m_dRadius *= dScale;
        }
    }

    public partial class Floor : IComparable
    {
        private string m_strID = "";
        private string m_strFloorName = "";
        // 0은 1층, 1은 2층, -1은 지하1층
        private int m_nFloorIndex = 0;
        private double m_dElevation = 0.0;
        private UnE.Geometry.Vertex2D m_vMoving = new UnE.Geometry.Vertex2D();
        private List<Wall> m_walls = new List<Wall>();
        private List<Space> m_spaces = new List<Space>();
        private List<Column> m_columns = new List<Column>();
        private List<Topology> m_topologies = new List<Topology>();
        private List<POI> m_pois = new List<POI>();
        private List<Wire> m_wires = new List<Wire>();
        private List<AlertArea> m_alertAreas = new List<AlertArea>();

        //ym.0826
        private List<Property> m_properties = new List<Property>();
        //ym.0826
        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public string ID
        {
            get
            {
                if (m_strID.StartsWith("level"))
                    return m_strID;

                return "level" + m_strID;
            }
            set { m_strID = value; }
        }

        public string Name
        {
            get { return m_strFloorName; }
            set { m_strFloorName = value; }
        }

        // 0은 1층, 1은 2층, -1은 지하1층
        public int FloorIndex
        {
            get { return m_nFloorIndex; }
            set { m_nFloorIndex = value; }
        }

        public double Elevation
        {
            get { return m_dElevation; }
            set { m_dElevation = value; }
        }

        public UnE.Geometry.Vertex2D MovingPosition
        {
            get { return m_vMoving; }
            set { m_vMoving = value; }
        }

        public List<Wall> Walls
        {
            get { return m_walls; }
        }

        public List<Space> Spaces
        {
            get { return m_spaces; }
        }

        public List<Column> Columns
        {
            get { return m_columns; }
        }

        public List<Topology> Topologies
        {
            get { return m_topologies; }
        }

        public List<POI> POIs
        {
            get { return m_pois; }
        }

        public List<Wire> Wires
        {
            get { return m_wires; }
        }

        public List<AlertArea> AlertAreas
        {
            get { return m_alertAreas; }
        }

        public int CompareTo(object obj)
        {
            Floor floor = (Floor)obj;

            if (this.FloorIndex == floor.FloorIndex)
                return 0;
            else if (this.FloorIndex < floor.FloorIndex)
                return -1;

            return 1;
        }

        // MovingPosition만큼 모두 이동
        public void Move(bool topology = false)
        {
            if (m_vMoving.x == 0.0 && m_vMoving.y == 0.0)
                return;

            foreach (Wall wall in m_walls)
            {
                wall.Move(-m_vMoving.x, -m_vMoving.y);
            }

            foreach (AlertArea alertArea in m_alertAreas)
            {
                alertArea.Move(-m_vMoving.x, -m_vMoving.y);
            }

            foreach (Column column in m_columns)
            {
                column.Move(-m_vMoving.x, -m_vMoving.y);
            }

            if (topology)
            {
                foreach (Topology _topology in m_topologies)
                {
                    _topology.Move(-m_vMoving.x, -m_vMoving.y);
                }
            }

            m_vMoving.SetVertex(0.0, 0.0);
        }

        public void SetScale(double dScale, bool topology = false)
        {
            foreach (Wall wall in m_walls)
            {
                wall.SetScale(dScale);
            }

            foreach (Column column in m_columns)
            {
                column.SetScale(dScale);
            }

            if (topology)
            {
                foreach (Topology _topology in m_topologies)
                {
                    _topology.SetScale(dScale);
                }
            }
        }
    }

    public class Project
    {
        public enum UnitOfLength { MM = 0, CM, Meter, Unknown };

        private string m_strProjectName = "";
        private string m_strUnit = "cm";
        private string m_strAuthor = "";
        private DateTime m_date = new DateTime();
        private List<Floor> m_floors = new List<Floor>();
        private List<Property> m_properties = new List<Property>();
        private AnchorNode m_anchorNode = null;

        public string ProjectName
        {
            get { return m_strProjectName; }
            set { m_strProjectName = value; }
        }

        public string Unit
        {
            get { return m_strUnit; }
            set { m_strUnit = value; }
        }

        public UnitOfLength LengthUnit
        {
            get
            {
                return StringToUnit(m_strUnit);
            }
        }

        public string Author
        {
            get { return m_strAuthor; }
            set { m_strAuthor = value; }
        }

        public DateTime Date
        {
            get { return m_date; }
            set { m_date = value; }
        }

        public List<Floor> Floors
        {
            get { return m_floors; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public AnchorNode AnchorNode
        {
            get { return m_anchorNode; }
            set { m_anchorNode = value; }
        }

        public static string UnitOfLengthString(UnitOfLength unit)
        {
            if (unit == UnitOfLength.MM)
                return "mm";
            else if (unit == UnitOfLength.CM)
                return "cm";
            else if (unit == UnitOfLength.Meter)
                return "meter";

            return "unknown";
        }

        public static UnitOfLength StringToUnit(string strUnit)
        {
            if (strUnit == "mm")
                return UnitOfLength.MM;
            else if (strUnit == "cm")
                return UnitOfLength.CM;
            else if (strUnit == "meter")
                return UnitOfLength.Meter;
            return UnitOfLength.Unknown;
        }
    }

    public class Wire
    {
        private string m_strID = "";
        private POI m_beginPOI = null;
        private POI m_endPOI = null;
        private POIType m_poiType = null;
        private List<Vertex2D> m_positions = new List<Vertex2D>();

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public POI BeginPOI
        {
            get { return m_beginPOI; }
            set { m_beginPOI = value; }
        }

        public POI EndPOI
        {
            get { return m_endPOI; }
            set { m_endPOI = value; }
        }

        public POIType POIType
        {
            get { return m_poiType; }
            set { m_poiType = value; }
        }

        public List<Vertex2D> Positions
        {
            get { return m_positions; }
        }
    }

    public class POI
    {
        private string m_strID = "";
        private string m_strPOIName = "";

        private bool m_useHeight = false;
        private double m_dHeight = 200;
        private Vertex2D m_vPos = new Vertex2D();
        private POIType m_poiType = null;
        private double m_dAngle = 0.0;
        private List<Property> m_properties = new List<Property>();

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public string Name
        {
            get { return m_strPOIName; }
            set { m_strPOIName = value; }
        }

        public double Height
        {
            get { return m_dHeight; }
            set { m_dHeight = value; }
        }

        public bool UseHeight
        {
            get { return m_useHeight; }
            set { m_useHeight = value; }
        }

        public Vertex2D Position
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public POIType PoiType
        {
            get { return m_poiType; }
            set { m_poiType = value; }
        }

        public double Angle
        {
            get { return m_dAngle; }
            set { m_dAngle = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }
    }

    public class POIType
    {
        private int m_nID = 0;

        private bool m_isGroup = false;
        private POIType m_parent = null;
        private string m_strName = "";
        private string m_strCode = "";
        private bool m_isUserDefined = false;
        private string m_strDefaultHeight = null;
        private List<POIType> m_childTypes = new List<POIType>();
        private bool m_isWireType = false;
        private int m_nParentID = 0;
        //ym
        private string m_strXMLID = "";
        private List<Property> m_properties = new List<Property>();

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public bool IsGroup
        {
            get { return m_isGroup; }
            set { m_isGroup = value; }
        }
        public int ParentID
        {
            get { return m_nParentID; }
            set { m_nParentID = value; }
        }
/*
        public POIType Parent
        {
            get { return m_parent; }
            set
            {
                m_parent = value;

                if (m_parent == null)
                    m_parent.m_childTypes.Clear();
                else
                {
                    if (m_parent.m_childTypes.Contains(this) == false)
                        m_parent.m_childTypes.Add(this);
                }
            }
        }*/
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

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Code
        {
            get { return m_strCode; }
            set { m_strCode = value; }
        }
        public string XMLID
        {
            get
            {
                if (m_strXMLID.StartsWith("poiType"))
                    return m_strXMLID;

                return "poiType" + m_strXMLID;
            }
            set { m_strXMLID = value; }
        }
        public bool IsUserDefined
        {
            get { return m_isUserDefined; }
            set { m_isUserDefined = value; }
        }

        public string DefaultHeight
        {
            get { return m_strDefaultHeight; }
            set { m_strDefaultHeight = value; }
        }

        public List<POIType> ChildTypes
        {
            get { return m_childTypes; }
        }

        public bool IsWireType
        {
            get { return m_isWireType; }
            set { m_isWireType = value; }
        }
        public List<Property> Properties
        {
            get { return m_properties; }
        }

    }
    //ym0729
    public class Property
    {
        private string m_strName = "";
        private string m_strValue = "";
        private string m_strDescription = null;

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public string Value
        {
            get { return m_strValue; }
            set { m_strValue = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }
    }

    public class AnchorNode
    {
        private Vertex2D m_vGlobal = null;
        private Vertex2D m_vLocal = null;
        // 방위각(Degree)
        private double m_dAngle = 0.0;
        private Project.UnitOfLength m_globalUnitOfLength = Project.UnitOfLength.Meter;

        public Vertex2D GlobalPosition
        {
            get { return m_vGlobal; }
            set { m_vGlobal = value; }
        }

        public Vertex2D LocalPosition
        {
            get { return m_vLocal; }
            set { m_vLocal = value; }
        }

        // 방위각(Degree)
        public double Angle
        {
            get { return m_dAngle; }
            set { m_dAngle = value; }
        }

        public Project.UnitOfLength GlobalUnitOfLength
        {
            get { return m_globalUnitOfLength; }
            set { m_globalUnitOfLength = value; }
        }
    }

    public class Grid
    {
        private string m_strID = "";
        private Line2D m_line = null;
        private Arc2D m_arc = null;
        private EArc2D m_earc = null;

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
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
    }

    public class CommonString
    {
        public const string XML_VERSION = "1.6";
        public const string XML_VERSION_2nd = "1.5";
    }
}
