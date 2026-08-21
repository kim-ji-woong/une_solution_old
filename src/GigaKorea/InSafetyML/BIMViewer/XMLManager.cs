using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnE.Geometry;

namespace BIMViewer
{
    using System.Drawing;
    using BIM;

    public class XMLManager
    {
        private string m_strErrorMessage = "";
        public static string TARGET_VERSION = "1.6";
        public static string MINIMUM_VERSION = "1.5";
        private string m_strDoubleFormat = "F1";

        // 전 층 TopologyNodes가 필요
        Dictionary<string, Topology.Node> m_dicTopologyNodes = new Dictionary<string, Topology.Node>();
        Dictionary<Topology.Node, List<string>> m_dicNodeLinks = new Dictionary<Topology.Node, List<string>>();

        private Dictionary<string, string> m_strPOIWireTable = new Dictionary<string, string>();

        public XMLManager()
        {
            m_strPOIWireTable["F1311"] = "소화설비-옥내소화전설비-일반 옥내소화전설비-옥내소화전 배관";
            m_strPOIWireTable["F1411"] = "소화설비-스프링클러설비-배관-배관";
            m_strPOIWireTable["F1511"] = "소화설비-간이스프링클러설비-배관-배관";
            m_strPOIWireTable["F1611"] = "소화설비-화재조기진압용 스프링클러설비-배관-배관";
            m_strPOIWireTable["F1710"] = "소화설비-물분무 소화설비-배관";
            m_strPOIWireTable["F1810"] = "소화설비-미분무 소화설비-배관";
            m_strPOIWireTable["F1910"] = "소화설비-포 소화설비-배관";
            m_strPOIWireTable["F1A30"] = "소화설비-이산화탄소 소화설비-배관";
            m_strPOIWireTable["F1B30"] = "소화설비-할로겐화합물 소화설비-배관";
            m_strPOIWireTable["F1C30"] = "소화설비-청정소화약제 소화설비-배관";
            m_strPOIWireTable["F1D30"] = "소화설비-분말 소화설비-배관";
            m_strPOIWireTable["F2250"] = "경보설비-비상경보설비-배선";
            m_strPOIWireTable["F2580"] = "경보설비-자동화재탐지설비-배선";
            m_strPOIWireTable["F2640"] = "경보설비-비상방송설비-배선";
            m_strPOIWireTable["F5230"] = "소화활동설비-연결송수관설비-배관";
            m_strPOIWireTable["F5330"] = "소화활동설비-연결살수설비-배관";
            m_strPOIWireTable["F5630"] = "소화활동설비-연소방지설비-배관";
        }

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public bool Save(Project project, Dictionary<int, POIType> dicPOITypes)
        {
            bool result = true;

            try
            {
                XmlTextWriter writer = new XmlTextWriter(project.LocalFilePath, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                result = WriteIndoorModelFile(project, dicPOITypes, writer);

                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return result;
        }

        private bool WriteIndoorModelFile(Project project, Dictionary<int, POIType> dicPOITypes, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("IndoorModelFile");

                writer.WriteStartAttribute("version");
                writer.WriteString(TARGET_VERSION) ;
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("xmlns:xsi");
                writer.WriteString("http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("xsi:noNamespaceSchemaLocation");
                writer.WriteString("http://unes.iptime.org:8001/Schema/InSafetyML.xsd");
                writer.WriteEndAttribute();

                if (WriteProjectInfo(project, writer) == false)
                    return false;
                if (WriteLevels(project, dicPOITypes, writer) == false)
                    return false;
                if (WriteCommons(project.Components, dicPOITypes, writer) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteCommons(List<Component> components, Dictionary<int, POIType> dicPOITypes, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Common");
                writer.WriteStartElement("Components");

                foreach (Component component in components)
                {
                    if (WriteComponent(component, writer) == false)
                        return false;
                }

                writer.WriteFullEndElement();

                writer.WriteStartElement("POITypes");

                foreach (KeyValuePair<int, POIType> pair in dicPOITypes)
                {
                    if (pair.Value.Parent == null)
                    {
                        if (WritePOIType(pair.Value, writer) == false)
                            return false;
                    }
                }

                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WritePOIType(POIType poiType, XmlTextWriter writer)
        {
            try
            {
                if (poiType.IsGroup)
                {
                    if (WritePOITypeGroup(poiType, writer) == false)
                        return false;
                }
                else
                {
                    if (_WritePOIType(poiType, writer) == false)
                        return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WritePOITypeGroup(POIType poiType, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("POITypeGroup");

                writer.WriteStartAttribute("id");
                writer.WriteString(poiType.XMLID);
                //writer.WriteString(poiType.Code);//ym. 다운받은 xml파일과 일치시키기위해 id에 code
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("name");
                writer.WriteString(poiType.Name);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("userDefined");
                writer.WriteString(GetBooleanString(poiType.UserDefined));
                writer.WriteEndAttribute();

                if (poiType.Code != null && poiType.Code.Length > 0)
                {
                    string strCode = poiType.Code;

                    if (poiType.Parent != null && poiType.Parent.Code != null)
                    {
                        int nLength = poiType.Parent.Code.Length;
                        strCode = strCode.Substring(nLength);
                    }

                    writer.WriteStartAttribute("code");
                    writer.WriteString(strCode);
                    writer.WriteEndAttribute();
                }

                foreach (POIType child in poiType.ChildTypes)
                {
                    if (child.IsGroup)
                    {
                        if (WritePOITypeGroup(child, writer) == false)
                            return false;
                    }
                    else
                    {
                        if (_WritePOIType(child, writer) == false)
                            return false;
                    }
                }

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool _WritePOIType(POIType poiType, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("POIType");

                writer.WriteStartAttribute("id");
                   writer.WriteString(poiType.XMLID);
                //writer.WriteString(poiType.Code);//ym. 다운받은 xml파일과 일치시키기위해 id에 code
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("name");
                writer.WriteString(poiType.Name);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("userDefined");
                writer.WriteString(GetBooleanString(poiType.UserDefined));
                writer.WriteEndAttribute();

                if (poiType.Code != null && poiType.Code.Length > 0)
                {
                    string strCode = poiType.Code;

                    int nLength = poiType.Parent.Code.Length;
                    strCode = strCode.Substring(nLength);

                    if (strCode.Length != 1)
                    {
                        strCode = strCode.Substring(0, 1);
                    }

                    writer.WriteStartAttribute("code");
                    writer.WriteString(strCode);
                    writer.WriteEndAttribute();
                }

                if (poiType.DefaultHeight != null && poiType.DefaultHeight.Length > 0)
                {
                    writer.WriteStartAttribute("defaultHeight");
                    writer.WriteString(poiType.DefaultHeight);
                    writer.WriteEndAttribute();
                }

                if (WriteProperty("POITypeProperties", poiType.Properties, writer) == 0)
                    writer.WriteEndElement();
                else
                    writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteComponent(Component component, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Component");

                writer.WriteStartAttribute("id");
                writer.WriteString(component.XMLID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("type");
                writer.WriteString(component.TypeName);
                writer.WriteEndAttribute();

                //ym.component정의에는 재질넣지않기. 재질은 속성에만 넣기
                //writer.WriteString(component.ComponentName);

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteLevels(Project project, Dictionary<int, POIType> dicPOITypes, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Levels");

                foreach (Level level in project.Levels)
                {
                    if (WriteLevel(level, dicPOITypes, writer) == false)
                        return false;
                }

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteLevel(Level level, Dictionary<int, POIType> dicPOITypes, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Level");

                writer.WriteStartAttribute("id");
                writer.WriteString(level.XMLID);
                writer.WriteEndAttribute();

                WriteProperty("LevelProperties", level.Properties, writer);

                writer.WriteStartElement("Name");
                writer.WriteString(level.Name);
                writer.WriteFullEndElement();

                writer.WriteStartElement("Elevation");
                writer.WriteString(GetDoubleString(level.Elevation));
                writer.WriteFullEndElement();

                if (WriteGridCollection(level.Walls, writer) == false)
                    return false;

                if (WriteElementCollection(level, dicPOITypes, writer) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteElementCollection(Level floor, Dictionary<int, POIType> dicPOITypes, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("ElementCollection");

                foreach (Wall wall in floor.Walls)
                {
                    if (WriteWall(wall, writer) == false)
                        return false;
                }

                foreach (Space space in floor.Spaces)
                {
                    if (WriteSpace(space, writer) == false)
                        return false;
                }

                foreach (AlertArea alertArea in floor.AlertAreas)
                {
                    if (WriteAlertArea(alertArea, writer) == false)
                        return false;
                }

                foreach (Wall wall in floor.Walls)
                {
                    foreach (Door door in wall.Doors)
                    {
                        if (WriteDoor(door, wall, writer) == false)
                            return false;
                    }
                }

                foreach (Wall wall in floor.Walls)
                {
                    foreach (Window window in wall.Windows)
                    {
                        if (WriteWindow(window, wall, writer) == false)
                            return false;
                    }
                }
                foreach (Column column in floor.Columns)
                {
                    if (WriteColumn(column, writer) == false)
                        return false;
                }
                foreach (Topology topology in floor.Topologies)
                {
                    if (WriteTopology(topology, writer) == false)
                        return false;
                }

                foreach (Shapes.POI poi in floor.POIs)
                {
                    if (WritePOI(poi, writer) == false)
                        return false;
                }

                foreach (Shapes.Wire wire in floor.Wires)
                {
                    if (WritePOIWire(wire, floor, dicPOITypes, writer) == false)
                        return false;
                }

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        private bool WriteColumn(Column column, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Column");

                writer.WriteStartAttribute("id");
                writer.WriteString(column.XMLID);
                writer.WriteEndAttribute();

                WriteProperty("ColumnProperties", column.Properties, writer);

                if (column.Type is Column.ColumnType.Rect)
                {
                    Column.Rect rect = column.RectData;

                    writer.WriteStartElement("Rect");

                    if (WriteVertex(rect.TopLeft, "TL", writer) == false)
                        return false;
                    if (WriteVertex(rect.BottomLeft, "BL", writer) == false)
                        return false;
                    if (WriteVertex(rect.BottomRight, "BR", writer) == false)
                        return false;

                    writer.WriteFullEndElement();
                }
                else if (column.Type is Column.ColumnType.Circle)
                {
                    Column.Circle circle = column.CircleData;

                    writer.WriteStartElement("Circle");

                    if (WriteVertex(circle.Center, "Center", writer) == false)
                        return false;

                    writer.WriteStartElement("Radius");
                    writer.WriteString(GetDoubleString(circle.Radius));
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                }

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private bool WriteVertex(UnE.Geometry.Vertex2D vertex, string strElementName, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement(strElementName);

                writer.WriteStartElement("Pos");
                writer.WriteString(GetVertexString(vertex.x, vertex.y));
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        private bool WritePOIWire(Shapes.Wire wire, Level level, Dictionary<int, POIType> dicPOITypes, XmlTextWriter writer)
        {
            Shapes.POI beginPOI = level.FindPOI(wire.BeginPOI);
            Shapes.POI endPOI = level.FindPOI(wire.EndPOI);

            if (beginPOI == null || endPOI == null)
                return false;

            POIType poiType = null;

            if (dicPOITypes.TryGetValue(wire.POITypeID, out poiType) == false)
                return false;

            try
            {
                writer.WriteStartElement("POIWire");

                writer.WriteStartAttribute("id");
                writer.WriteString(wire.XMLID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("beginPOI");
                writer.WriteString(beginPOI.XMLID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("endPOI");
                writer.WriteString(endPOI.XMLID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("type");
                //writer.WriteString(poiType.XMLID);
                writer.WriteString(poiType.Code);
                writer.WriteEndAttribute();

                writer.WriteStartElement("Lines");
                writer.WriteString(wire.Lines);
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WritePOI(Shapes.POI poi, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("POI");

                writer.WriteStartAttribute("id");
                writer.WriteString(poi.XMLID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("type");
                //   writer.WriteString(poi.PoiType.XMLID);//ym.0906.poi 아이콘 안보여서 수정해봄.
                writer.WriteString(poi.PoiType.Code);//poi타입코드 쓰기로함.
                writer.WriteEndAttribute();

                WriteProperty("POIProperties", poi.Properties, writer);

                writer.WriteStartElement("Name");
                writer.WriteString(poi.Name);
                writer.WriteFullEndElement();

                WriteVertexElement("Point", poi.Position, writer);

                writer.WriteStartElement("Height");
                // TODO: POI Height 단위 문제
                writer.WriteString(GetDoubleString(poi.Height * 10));
                writer.WriteFullEndElement();

                writer.WriteStartElement("Angle");
                writer.WriteString(GetDoubleString(poi.Angle));
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteTopology(Topology topology, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Topology");

                writer.WriteStartAttribute("id");
                writer.WriteString(topology.XMLID);
                writer.WriteEndAttribute();

                WriteProperty("TopologyProperties", topology.Properties, writer);

                foreach (Topology.Node node in topology.Nodes)
                {
                    if (WriteTopologyNode(node, writer) == false)
                        return false;
                }

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteTopologyNode(Topology.Node node, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Node");

                writer.WriteStartAttribute("id");
                writer.WriteString(node.XMLID);
                writer.WriteEndAttribute();

                WriteProperty("TopologyNodeProperties", node.Properties, writer);

                foreach (Topology.Node link in node.LinkedNodes)
                {
                    writer.WriteStartElement("Target");

                    writer.WriteStartAttribute("id");
                    writer.WriteString(link.XMLID);
                    writer.WriteEndAttribute();

                    writer.WriteEndElement();
                }

                writer.WriteStartElement("Point");

                writer.WriteStartElement("Pos");
                writer.WriteString(GetVertexString(node.X, node.Y));
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteDoor(Door door, Wall wall, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Door");

                writer.WriteStartAttribute("id");
                writer.WriteString(door.XMLID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("attachedWall");
                writer.WriteString(wall.XMLID);
                writer.WriteEndAttribute();

                WriteProperty("DoorProperties", door.Properties, writer);

                WriteVertexElement("Point", door.Position, writer);

                writer.WriteStartElement("Width");
                writer.WriteString(GetDoubleString(door.Width));
                writer.WriteFullEndElement();

                writer.WriteStartElement("Height");
                writer.WriteString(GetDoubleString(door.Height));
                writer.WriteFullEndElement();

                writer.WriteStartElement("Elevation");
                writer.WriteString(GetDoubleString(door.Elevation));
                writer.WriteFullEndElement();

                if (door.Hinge1 != null)
                {
                    WriteVertexElement("Hinge1", door.Hinge1, writer);
                }

                if (door.Hinge2 != null)
                {
                    WriteVertexElement("Hinge2", door.Hinge2, writer);//door.hinge1->hinge2.ym
                }

                writer.WriteStartElement("DoorType");
                writer.WriteString(((int)door.GetDoorType()).ToString());
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteWindow(Window window, Wall wall, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Window");

                writer.WriteStartAttribute("id");
                writer.WriteString(window.XMLID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("attachedWall");
                writer.WriteString(wall.XMLID);
                writer.WriteEndAttribute();

                WriteProperty("WindowProperties", window.Properties, writer);

                WriteVertexElement("Point", window.Position, writer);

                writer.WriteStartElement("Width");
                writer.WriteString(GetDoubleString(window.Width));
                writer.WriteFullEndElement();

                writer.WriteStartElement("Height");
                writer.WriteString(GetDoubleString(window.Height));
                writer.WriteFullEndElement();

                writer.WriteStartElement("Elevation");
                writer.WriteString(GetDoubleString(window.Elevation));
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteSpace(Space space, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Space");

                writer.WriteStartAttribute("id");
                writer.WriteString(space.XMLID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("name");
                writer.WriteString(space.Name);
                writer.WriteEndAttribute();

                WriteProperty("SpaceProperties", space.Properties, writer);                

                foreach (Wall wall in space.Walls)
                {
                    writer.WriteStartElement("LinkedWall");

                    writer.WriteStartAttribute("link");
                    writer.WriteString(wall.XMLID);
                    writer.WriteEndAttribute();

                    writer.WriteEndElement();
                }

                // 공간 바운더리 및 홀 바운더리 쓰기 부분
                WriteBoundary(space.BoundaryData, writer);
                WriteHole(space.HoleBoundary, writer);

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteAlertArea(AlertArea alertArea, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("AlertArea");

                writer.WriteStartAttribute("id");
                writer.WriteString(alertArea.XMLID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("name");
                writer.WriteString(alertArea.Name);
                writer.WriteEndAttribute();

                WriteProperty("AlertAreaProperties", alertArea.Properties, writer);

                WriteBoundary(alertArea.Boundary, writer);

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteWall(Wall wall, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Wall");

                writer.WriteStartAttribute("id");
                writer.WriteString(wall.XMLID);
                writer.WriteEndAttribute();

                if (wall.GridID.Length == 0)
                    wall.MakeGridID();

                writer.WriteStartAttribute("grid");
                writer.WriteString(wall.GridID);                
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("component");
                writer.WriteString(wall.Component.XMLID);
                writer.WriteEndAttribute();

                WriteProperty("WallProperties", wall.Properties, writer);

                writer.WriteStartElement("Thickness");
                writer.WriteString(GetDoubleString(wall.Thick));
                writer.WriteFullEndElement();

                writer.WriteStartElement("Height");
                writer.WriteString(GetDoubleString(wall.Height));
                writer.WriteFullEndElement();

                WriteBoundary(wall.BoundaryData, writer);

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void WriteBoundary(Boundary boundary, XmlTextWriter writer)
        {
            if (boundary.GetBoundary().Count == 0)
                return;

            List<Shapes.PathItem> items = boundary.GetBoundary();

            writer.WriteStartElement("Boundary");

            foreach (Shapes.PathItem path in items)
            {

                if (path.GetDrawType() == Shapes.PathItem.DrawType.Line)
                {
                    Vertex2D vBegin, vEnd, vMiddle = null;
                    path.GetVertex(out vBegin, out vEnd, out vMiddle);

                    writer.WriteStartElement("Line");

                    writer.WriteStartElement("Pos");
                    writer.WriteString(GetVertexString(vBegin));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("Pos");
                    writer.WriteString(GetVertexString(vEnd));
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                }
                else if (path.GetDrawType() == Shapes.PathItem.DrawType.Arc)
                {
                    Arc2D arc = (Arc2D)path.GetEArc();

                    writer.WriteStartElement("Arc");

                    writer.WriteStartElement("Center");
                    writer.WriteStartElement("Pos");
                    writer.WriteString(GetVertexString(arc.GetCenter()));
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("Radius");
                    writer.WriteString(GetDoubleString(arc.GetRadius()));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("BeginAngle");
                    writer.WriteString(GetDoubleString(arc.GetBeginAngle()));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("Angle");
                    writer.WriteString(GetDoubleString(arc.GetAngle()));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("ClockWise");
                    writer.WriteString(GetBooleanString(arc.IsClockWise()));
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();       // Arc
                }
                else if (path.GetDrawType() == Shapes.PathItem.DrawType.EArc)
                {
                    EArc2D eArc = path.GetEArc();

                    writer.WriteStartElement("EArc");

                    WriteVertexElement("TL", eArc.GetTL(), writer);
                    WriteVertexElement("BL", eArc.GetBL(), writer);
                    WriteVertexElement("BR", eArc.GetBR(), writer);

                    writer.WriteStartElement("BeginAngle");
                    writer.WriteString(GetDoubleString(eArc.GetBeginAngle()));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("Angle");
                    writer.WriteString(GetDoubleString(eArc.GetAngle()));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("ClockWise");
                    writer.WriteString(GetBooleanString(eArc.IsClockWise()));
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();       // EArc
                }
            }

            writer.WriteFullEndElement();   // Boundary

        }

        private void WriteHole(List<Boundary> holeBoundarys, XmlTextWriter writer)
        {
            if (holeBoundarys == null || holeBoundarys.Count == 0)
                return;

            writer.WriteStartElement("Hole");

            foreach (Boundary bound in holeBoundarys)
            {
                WriteBoundary(bound, writer);
            }

            writer.WriteFullEndElement();   // Hole
        }

        private bool WriteGridCollection(List<Wall> walls, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("GridCollection");

                foreach (Wall wall in walls)
                {
                    if (WriteGrid(wall, writer) == false)
                        return false;
                }

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteGrid(Wall wall, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Grid");

                if (wall.GridID.Length == 0)
                    wall.MakeGridID();

                writer.WriteStartAttribute("id");
                writer.WriteString(wall.GridID);
                writer.WriteEndAttribute();

                if (wall.Line != null)
                {
                    writer.WriteStartElement("Line");

                    writer.WriteStartElement("Pos");
                    writer.WriteString(GetVertexString(wall.Line.GetVertex(true)));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("Pos");
                    writer.WriteString(GetVertexString(wall.Line.GetVertex(false)));
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                }
                else if (wall.Arc != null)
                {
                    writer.WriteStartElement("Arc");

                    WriteVertexElement("Center", wall.Arc.GetCenter(), writer);

                    writer.WriteStartElement("Center");
                    writer.WriteStartElement("Pos");
                    writer.WriteString(GetVertexString(wall.Arc.GetCenter()));
                    writer.WriteFullEndElement();
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("Radius");
                    writer.WriteString(GetDoubleString(wall.Arc.GetRadius()));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("BeginAngle");
                    writer.WriteString(GetDoubleString(wall.Arc.GetBeginAngle()));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("Angle");
                    writer.WriteString(GetDoubleString(wall.Arc.GetAngle()));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("ClockWise");
                    writer.WriteString(GetBooleanString(wall.Arc.IsClockWise()));
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                }
                else if (wall.EArc != null)
                {
                    writer.WriteStartElement("EArc");

                    WriteVertexElement("TL", wall.EArc.GetTL(), writer);
                    WriteVertexElement("BL", wall.EArc.GetBL(), writer);
                    WriteVertexElement("BR", wall.EArc.GetBR(), writer);

                    writer.WriteStartElement("BeginAngle");
                    writer.WriteString(GetDoubleString(wall.EArc.GetBeginAngle()));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("Angle");
                    writer.WriteString(GetDoubleString(wall.EArc.GetAngle()));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("ClockWise");
                    writer.WriteString(GetBooleanString(wall.EArc.IsClockWise()));
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                }

                writer.WriteFullEndElement();   // Grid End
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteProjectInfo(Project project, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("ProjectInfo");

                writer.WriteStartAttribute("name");
                writer.WriteString(project.Name);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("unit");
                writer.WriteString(project.GetUnitString());
                writer.WriteEndAttribute();

                if (project.Unit == Project.UnitOfLength.MM)
                    m_strDoubleFormat = "F0";
                else if (project.Unit == Project.UnitOfLength.CM)
                    m_strDoubleFormat = "F1";
                else if (project.Unit == Project.UnitOfLength.M)
                    m_strDoubleFormat = "F3";

                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", project.TimeStamp.Year, project.TimeStamp.Month, project.TimeStamp.Day, project.TimeStamp.Hour, project.TimeStamp.Minute, project.TimeStamp.Second);
                writer.WriteStartAttribute("datetime");
                writer.WriteString(strTime);
                writer.WriteEndAttribute();

                if (project.Author != null && project.Author.Trim().Length > 0)
                {
                    writer.WriteStartAttribute("author");
                    writer.WriteString(project.Author.Trim());
                    writer.WriteEndAttribute();
                }

                WriteProperty("ProjectProperties", project.Properties, writer);
                // 앵커노드 쓰기부분
                WriteAnchorNode(project.AnchorNode, writer);

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private int WriteProperty(string strGroupName, List<Property> properties, XmlTextWriter writer)
        {
            if (properties.Count == 0)
                return 0;

            writer.WriteStartElement(strGroupName);

            foreach (Property property in properties)
            {
                writer.WriteStartElement("Property");

                writer.WriteStartElement("Name");
                writer.WriteString(property.Name);
                writer.WriteFullEndElement();

                writer.WriteStartElement("Value");
                writer.WriteString(property.Value);
                writer.WriteFullEndElement();

                if (property.Description != null && property.Description.Length > 0)
                {
                    writer.WriteStartElement("Description");
                    writer.WriteString(property.Description);
                    writer.WriteFullEndElement();
                }

                writer.WriteFullEndElement();
            }

            writer.WriteFullEndElement();
            return properties.Count;
        }

        private void WriteAnchorNode(AnchorNode anchor, XmlTextWriter writer)
        {
            if (anchor.Global == null || anchor.Local == null)
                return;

            writer.WriteStartElement("AnchorNode");

            writer.WriteStartElement("Local");

            writer.WriteStartElement("Pos");
            writer.WriteString(GetVertexString(anchor.Local.Position));
            writer.WriteFullEndElement();

            writer.WriteStartElement("Angle");
            writer.WriteString(GetDoubleString(anchor.Local.Angle));
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();   // Local

            writer.WriteStartElement("Global");

            writer.WriteStartAttribute("unit");
            writer.WriteString(anchor.Global.GetUnitString());
            writer.WriteEndAttribute();

            writer.WriteStartElement("Pos");
            writer.WriteString(GetVertexString(anchor.Global.Position));
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();   // Global

            WriteProperty("AnchorNodeProperties", anchor.Properties, writer);

            writer.WriteFullEndElement();   // AnchorNode
        }

        private void WriteVertexElement(string strElementName, UnE.Geometry.Vertex2D vertex, XmlTextWriter writer)
        {
            writer.WriteStartElement(strElementName);

            writer.WriteStartElement("Pos");
            writer.WriteString(GetVertexString(vertex));
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();
        }

        private string GetBooleanString(bool value)
        {
            return value ? "true" : "false";
        }

        private string GetVertexString(UnE.Geometry.Vertex2D vertex)
        {
            return GetVertexString(vertex.x, vertex.y);
        }

        private string GetVertexString(double x, double y)
        {
            return GetDoubleString(x) + "," + GetDoubleString(y);
        }

        private string GetDoubleString(double data)
        {
            return string.Format("{0:" + m_strDoubleFormat + "}", data);
        }

        public Project ReadProject(string strFilePath, Dictionary<int, POIType> dicPOITypes)
        {
            Project project = null;

            try
            {
                m_strErrorMessage = "";

                bool stop = false;

                XmlTextReader reader = new XmlTextReader(strFilePath);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "IndoorModelFile", true) == 0)
                            {
                                project = ReadIndoorModelFile(reader, dicPOITypes);
                                reader.Close();
                                return project;
                            }

                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                reader.Close();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return project;
        }

        public bool ReadLevels(Project project, Dictionary<int, POIType> dicPOITypes)
        {
            if (project.Levels.Count > 0)
                return true;

            if (project.LocalFilePath == null || project.LocalFilePath.Length == 0 || System.IO.File.Exists(project.LocalFilePath) == false)
            {
                m_strErrorMessage = "project의 XML 파일 경로가 존재하지 않거나 잘못된 경로입니다.";
                return false;
            }

            try
            {
                m_strErrorMessage = "";

                bool stop = false;

                XmlTextReader reader = new XmlTextReader(project.LocalFilePath);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "IndoorModelFile", true) == 0)
                            {
                                bool result = ReadIndoorModelFile(reader, project, dicPOITypes);
                                reader.Close();
                                return result;
                            }

                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                reader.Close();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return false;
        }

        private Project ReadIndoorModelFile(XmlTextReader reader, Dictionary<int, POIType> dicPOITypes)
        {
            Project project = null;

            try
            {
                bool stop = false;
                string strVersion = "";
                double dVersion = 0;
                string strMiniVersion = MINIMUM_VERSION;
                double dMiniVersion = Convert.ToDouble(strMiniVersion);

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "version", true) == 0)
                    {
                        strVersion = reader.Value;
                        dVersion = double.Parse(strVersion);
                    }
                }

                //if (strVersion != TARGET_VERSION)
                //{
                //    m_strErrorMessage = "문서의 버전이 현재버전과 다릅니다.\r\n문서버전 : " + strVersion + ", 타겟버전 : " + TARGET_VERSION;
                //    return project;
                //}
                if (!(dVersion >= dMiniVersion))
                {   // XML 문서 버전이 1.5 이상만 읽기가 가능 
                    m_strErrorMessage = "문서의 버전이 현재버전과 다릅니다.\r\n문서버전 : " + strVersion + ", 타겟버전 : " + dVersion.ToString() + ", " + MINIMUM_VERSION + " 이상 읽을 수 있습니다.";
                    return project;
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ProjectInfo", true) == 0)
                            {
                                project = ReadProject(reader);
                            }
                            else if (string.Compare(reader.Name, "Common", true) == 0)
                            {
                                ReadCommon(reader, project, dicPOITypes);
                                stop = true;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return project;
        }

        private bool ReadIndoorModelFile(XmlTextReader reader, Project project, Dictionary<int, POIType> dicPOITypes)
        {
            try
            {
                bool stop = false;
                string strVersion = "";
                double dVersion = 0;
                string strMiniVersion = MINIMUM_VERSION;
                double dMiniVersion = Convert.ToDouble(strMiniVersion);

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "version", true) == 0)
                    {
                        strVersion = reader.Value;
                        dVersion = double.Parse(strVersion);
                    }
                }

                //if (strVersion != TARGET_VERSION)
                //{
                //    m_strErrorMessage = "문서의 버전이 현재버전과 다릅니다.\r\n문서버전 : " + strVersion + ", 타겟버전 : " + TARGET_VERSION;
                //    return false;
                //}
                if (!(dVersion >= dMiniVersion))
                {   // XML 문서 버전이 1.5 이상만 읽기가 가능
                    m_strErrorMessage = "문서의 버전이 현재버전과 다릅니다.\r\n문서버전 : " + strVersion + ", 타겟버전 : " + dVersion.ToString() + ", " + MINIMUM_VERSION + " 이상 읽을 수 있습니다.";
                    return false;
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Levels", true) == 0)
                            {
                                return ReadLevels(reader, project, dicPOITypes);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return false;
        }

        private bool ReadLevels(XmlTextReader reader, Project project, Dictionary<int, POIType> dicPOITypes)
        {
            try
            {
                bool stop = false;
                m_dicTopologyNodes = new Dictionary<string, Topology.Node>();
                m_dicNodeLinks = new Dictionary<Topology.Node, List<string>>();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Level", true) == 0)
                            {
                                Level level = ReadLevel(reader, project, dicPOITypes);

                                if (level == null)
                                    return false;
                                else
                                    project.Levels.Add(level);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            // Topology 링크 연결 작업 필요.
                            ConnTopologyNodeLink();
                            stop = true;

                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private Level ReadLevel(XmlTextReader reader, Project project, Dictionary<int, POIType> dicPOITypes)
        {
            Level level = null;

            try
            {
                bool stop = false;
                string strID = null;
                Dictionary<string, Grid> dicGrids = new Dictionary<string, Grid>();

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Level Element에 id가 존재하지 않습니다.";
                    return level;
                }

                level = new Level();
                level.XMLID = strID;
                level.ID = strID.GetHashCode();
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "LevelProperties", true) == 0)
                            {
                                if (ReadProperties(reader, level.Properties) == false)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "Name", true) == 0)
                            {
                                string strName = "";

                                if (ReadElementText(reader, ref strName) == false)
                                    return null;

                                level.Name = strName;
                            }
                            else if (string.Compare(reader.Name, "Elevation", true) == 0)
                            {
                                string strElevation = "";

                                if (ReadElementText(reader, ref strElevation) == false)
                                    return null;

                                float fElevation = 0.0f;

                                if (float.TryParse(strElevation, out fElevation) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Elevation에 잘못된 값이 들어있습니다.";
                                    return null;
                                }

                                level.Elevation = fElevation;
                            }
                            else if (string.Compare(reader.Name, "GridCollection", true) == 0)
                            {
                                if (ReadGridCollection(reader, dicGrids) == false)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "ElementCollection", true) == 0)
                            {
                                if (ReadElementCollection(reader, level, dicGrids, project, dicPOITypes) == false)
                                    return null;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return level;
        }

        private bool ReadElementCollection(XmlTextReader reader, Level level, Dictionary<string, Grid> dicGrids, Project project, Dictionary<int, POIType> dicPOITypes)
        {
            try
            {
                bool stop = false;
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Wall", true) == 0)
                            {
                                Wall wall = ReadWall(reader, level, dicGrids, project);

                                if (wall == null)
                                    return false;
                                else
                                    level.AddWall(wall);
                            }
                            else if (string.Compare(reader.Name, "Space", true) == 0)
                            {
                                Space space = ReadSpace(reader, level);

                                if (space == null)
                                    return false;
                                else
                                    level.AddSpace(space);
                            }
                            else if (string.Compare(reader.Name, "AlertArea", true) == 0)
                            {
                                AlertArea alertArea = ReadAlertArea(reader, level);

                                if (alertArea == null)
                                    return false;
                                else
                                    level.AddAlertArea(alertArea);
                            }
                            else if (string.Compare(reader.Name, "Door", true) == 0)
                            {
                                if (ReadDoor(reader, level) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Window", true) == 0)
                            {
                                if (ReadWindow(reader, level) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Column", true) == 0)
                            {
                                if (ReadColumn(reader, level) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Topology", true) == 0)
                            {
                                if (ReadTopology(reader, level) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "POI", true) == 0)
                            {
                                if (ReadPOI(reader, level, dicPOITypes) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "POIWire", true) == 0)
                            {
                                if (ReadPOIWire(reader, level, dicPOITypes) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadColumn(XmlTextReader reader, Level level)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                string strColumnID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strColumnID = reader.Value;
                    }
                }

                if (strColumnID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column Element에 id가 존재하지 않습니다.";
                    return false;
                }
                
                Column column = null;
                column = new Column();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ColumnProperties", true) == 0)
                            {                              
                                if (ReadProperties(reader, column.Properties) == false)
                                    return false;                                
                            }
                            else if (string.Compare(reader.Name, "Rect", true) == 0)
                            {//ym0729
                                Column tmpColumn = ReadRectColumn(reader);
                                column.RectData = tmpColumn.RectData;
                                column.Type = tmpColumn.Type;

                                if (column == null)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Circle", true) == 0)
                            {
                                Column tmpColumn = ReadCircleColumn(reader);
                                column.CircleData = tmpColumn.CircleData;
                                column.Type = tmpColumn.Type;
                             
                                if (column == null)
                                    return false;
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (column == null)
                    return false;

                column.ID = strColumnID.GetHashCode();
                column.XMLID = strColumnID;

                level.Columns.Add(column);
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private Column ReadCircleColumn(XmlTextReader reader)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return null;

                Column column = new Column();
                column.CircleData = new Column.Circle();
                column.Type = Column.ColumnType.Circle;

                bool readCenter = false, readRadius = false;
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Center", true) == 0)
                            {
                                column.CircleData.Center = ReadPos(reader);

                                if (column.CircleData.Center == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Center에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                                else
                                {
                                    readCenter = true;                                    
                                }
                            }
                            else if (string.Compare(reader.Name, "Radius", true) == 0)
                            {
                                string strRadius = "";
                                double dRadius;

                                if (ReadElementText(reader, ref strRadius) == false)
                                    return null;

                                if (double.TryParse(strRadius, out dRadius))
                                {
                                    readRadius = true;
                                    column.CircleData.Radius = dRadius;                                    
                                }
                                else
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Radius에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readCenter == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column/Circle Element에 Center 정보가 없습니다.";
                    return null;
                }

                if (readRadius == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column Element에 Radius가 없습니다.";
                    return null;
                }

                return column;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return null;
        }

        private Column ReadRectColumn(XmlTextReader reader)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return null;

                Column column = new Column();
                column.RectData = new Column.Rect();
                column.Type = Column.ColumnType.Rect;

                bool readTL = false, readBL = false, readBR = false;
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TL", true) == 0)
                            {
                                column.RectData.TopLeft = ReadPos(reader);

                                if (column.RectData.TopLeft == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", TL에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                                else
                                {
                                    readTL = true;
                                }
                            }
                            else if (string.Compare(reader.Name, "BL", true) == 0)
                            {
                                column.RectData.BottomLeft = ReadPos(reader);

                                if (column.RectData.BottomLeft == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", BL에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                                else
                                {
                                    readBL = true;
                                }
                            }
                            else if (string.Compare(reader.Name, "BR", true) == 0)
                            {
                                column.RectData.BottomRight = ReadPos(reader);

                                if (column.RectData.BottomRight == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", BR에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                                else
                                {
                                    readBR = true;
                                }
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readTL == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column/Rect Element에 TL 정보가 없습니다.";
                    return null;
                }

                if (readBL == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column/Rect Element에 BL 정보가 없습니다.";
                    return null;
                }

                if (readBR == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column/Rect Element에 BR 정보가 없습니다.";
                    return null;
                }

                return column;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return null;
        }

        private bool ReadPOIWire(XmlTextReader reader, Level level, Dictionary<int, POIType> dicPOITypes)
        {
            try
            {
                bool stop = false;
                string strID = null, strBeginPOIID = null, strEndPOIID = null, strTypeID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "beginPOI", true) == 0)
                    {
                        strBeginPOIID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "endPOI", true) == 0)
                    {
                        strEndPOIID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "type", true) == 0)
                    {
                        strTypeID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POIWire Element에 id가 존재하지 않습니다.";
                    return false;
                }

                if (strBeginPOIID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POIWire Element에 beginPOI가 존재하지 않습니다.";
                    return false;
                }

                if (strEndPOIID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POIWire Element에 endPOI가 존재하지 않습니다.";
                    return false;
                }

                if (strTypeID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POIWire Element에 type이 존재하지 않습니다.";
                    return false;
                }

                Shapes.POI beginPOI = level.FindPOI(strBeginPOIID.GetHashCode());

                if (beginPOI == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", " + strBeginPOIID + "는 존재하지 않는 POI ID입니다.";
                    return false;
                }

                Shapes.POI endPOI = level.FindPOI(strEndPOIID.GetHashCode());

                if (endPOI == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", " + strEndPOIID + "는 존재하지 않는 POI ID입니다.";
                    return false;
                }

                POIType poiType = null;

                foreach (KeyValuePair<int, POIType> item in dicPOITypes)
                {
                    if (item.Value.Code == strTypeID)
                    {
                        poiType = item.Value;
                        break;
                    }
                }

                if (poiType == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", " + strTypeID + "는 존재하지 않는 POIType ID입니다.";
                    return false;
                }

                //if (dicPOITypes.TryGetValue(strTypeID.GetHashCode(), out poiType) == false)
                //{
                //    m_strErrorMessage = GetLineCountString(reader) + ", " + strTypeID + "는 존재하지 않는 POIType ID입니다.";
                //    return false;
                //}

                Shapes.Wire wire = new Shapes.Wire();

                wire.ID = strID.GetHashCode();
                wire.XMLID = strID;
                wire.POITypeID = poiType.ID;
                wire.BeginPOI = beginPOI.ID;
                wire.EndPOI = endPOI.ID;

                level.AddWire(wire);

                System.Resources.ResourceManager rm = Properties.Resources.ResourceManager;

                string strLines = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Lines", true) == 0)
                            {
                                if (ReadElementText(reader, ref strLines) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Lines에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                POIType poiType2 = null;

                                foreach (KeyValuePair<int, POIType> item in dicPOITypes)
                                {
                                    if (item.Value.Code == strTypeID)
                                    {
                                        poiType2 = item.Value;

                                        Bitmap img = (Bitmap)rm.GetObject(poiType2.Code);
                                        if (img == null)
                                            img = (Bitmap)rm.GetObject("empty");

                                        wire.Icon = img;

                                        Shapes.POI poi = new Shapes.POI();
                                        poi.PoiType = poiType;
                                        wire.POIIcon = poi;

                                        break;
                                    }
                                }

                                //POIType poiType2 = null;
                                //if (dicPOITypes.TryGetValue(strTypeID.GetHashCode(), out poiType2))
                                //{
                                //    Bitmap img = (Bitmap)rm.GetObject(poiType2.Code);
                                //    if (img == null)
                                //        img = (Bitmap)rm.GetObject("empty");

                                //    wire.Icon = img;

                                //    Shapes.POI poi = new Shapes.POI();
                                //    poi.PoiType = poiType;
                                //    wire.POIIcon = poi;
                                //}

                                string[] lines = strLines.Split(',');
                                for (int i = 0; i < lines.Length; i += 2)
                                {
                                    double x = Convert.ToDouble(lines[i]);
                                    double y = Convert.ToDouble(lines[i + 1]);
                                    wire.Positions.Add(new Vertex2D(x, y));
                                }

                                //wire.Lines = strLines;
                                wire.SetIconPosition();
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (strLines == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Lines가 존재하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadPOI(XmlTextReader reader, Level level, Dictionary<int, POIType> dicPOITypes)
        {
            try
            {
                bool stop = false;
                string strID = null, strTypeID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "type", true) == 0)
                    {
                        strTypeID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POI Element에 id가 존재하지 않습니다.";
                    return false;
                }

                if (strTypeID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POI Element에 type이 존재하지 않습니다.";
                    return false;
                }

                POIType poiType = null;

                foreach (KeyValuePair<int, POIType> item in dicPOITypes)
                {
                    if (item.Value.Code == strTypeID)
                    {
                        poiType = item.Value;
                        break;
                    }
                }

                if (poiType == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", " + strTypeID + "는 존재하지 않는 POIType ID입니다.";

                    //while (reader.Read())
                    //{
                    //    switch (reader.NodeType)
                    //    {
                    //        case XmlNodeType.Element:

                    //            break;

                    //        case XmlNodeType.EndElement:
                    //            stop = true;
                    //            break;
                    //    }

                    //    if (stop)
                    //        break;
                    //}

                    return false;
                }

                //if (dicPOITypes.TryGetValue(strTypeID.GetHashCode(), out poiType) == false)
                //{
                //    m_strErrorMessage = GetLineCountString(reader) + ", " + strTypeID + "는 존재하지 않는 POIType ID입니다.";
                //    return false;
                //}

                Shapes.POI poi = new Shapes.POI();

                poi.ID = strID.GetHashCode();
                poi.XMLID = strID;
                poi.PoiType = poiType;

                if (FormMain.Instance.BimManager.DicPOIColor.ContainsKey(poi.PoiType.ID))
                    poi.FillColor = FormMain.Instance.BimManager.DicPOIColor[poi.PoiType.ID];

                level.AddPOI(poi);

                bool readName = false, readAngle = false;
                Vertex2D vPos = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "POIProperties", true) == 0)
                            {
                                if (ReadProperties(reader, poi.Properties) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Name", true) == 0)
                            {
                                string strName = "";

                                if (ReadElementText(reader, ref strName) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Name에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                poi.Name = strName;
                                readName = true;
                            }
                            else if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                vPos = ReadPos(reader);

                                if (vPos == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Point에 잘못된 값이 들어있습니다.";
                                    return false;
                                }
                                else
                                    poi.Position = vPos;
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                double dAngle;

                                if (ReadElementDouble(reader, out dAngle) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Angle에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                poi.Angle = dAngle;
                                readAngle = true;
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                double dHeight;

                                if (ReadElementDouble(reader, out dHeight) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Height에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                // TODO: POI Height 값 단위 문제
                                poi.Height = (int)dHeight / 10;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (vPos == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Point가 존재하지 않습니다.";
                    return false;
                }

                if (readName == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Name이 존재하지 않습니다.";
                    return false;
                }

                if (readAngle == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Angle이 존재하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadTopology(XmlTextReader reader, Level level)
        {
            try
            {
                bool stop = false;
                string strID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Topology Element에 id가 존재하지 않습니다.";
                    return false;
                }

                Topology topology = new Topology();

                topology.ID = strID.GetHashCode();
                topology.XMLID = strID;
                level.Topologies.Add(topology);

                Dictionary<string, Topology.Node> dicTopologyNodes = new Dictionary<string, Topology.Node>();
                Dictionary<Topology.Node, List<string>> dicNodeLinks = new Dictionary<Topology.Node, List<string>>();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TopologyProperties", true) == 0)
                            {
                                if (ReadProperties(reader, topology.Properties) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Node", true) == 0)
                            {
                                if (ReadTopologyNode(reader, topology, dicTopologyNodes, dicNodeLinks) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                // 해당층 노드만 링크 연결을 하고 있는 상황인데, 전층 노드를 링크 연결 필요로 주석처리
                //Topology.Node link;

                //foreach (KeyValuePair<Topology.Node, List<string>> pair in dicNodeLinks)
                //{
                //    foreach (string strNodeID in pair.Value)
                //    {
                //        if (dicTopologyNodes.TryGetValue(strNodeID, out link) == false)
                //        {
                //            m_strErrorMessage = GetLineCountString(reader) + ", " + strNodeID + "는 존재하지 않는 Node ID입니다.";
                //            return false;
                //        }

                //        pair.Key.LinkedNodes.Add(link);
                //    }
                //}
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadTopologyNode(XmlTextReader reader, Topology topology, Dictionary<string, Topology.Node> dicTopologyNodes, Dictionary<Topology.Node, List<string>> dicNodeLinks)
        {
            try
            {
                bool stop = false;
                string strID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Node Element에 id가 존재하지 않습니다.";
                    return false;
                }

                Topology.Node node = new Topology.Node();

                node.ID = strID.GetHashCode();
                node.XMLID = strID;
                topology.Nodes.Add(node);

                dicTopologyNodes[strID] = node;
                List<string> links = new List<string>();
                dicNodeLinks[node] = links;

                m_dicTopologyNodes[strID] = node;           // 전 층 노드 링크 연결시 필요.
                m_dicNodeLinks[node] = links;               // 전 층 노드 링크 연결시 필요.

                Vertex2D vPos = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TopologyNodeProperties", true) == 0)
                            {
                                if (ReadProperties(reader, node.Properties) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Target", true) == 0)
                            {
                                if (ReadTopologyNodeLink(reader, links) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                vPos = ReadPos(reader);

                                if (vPos == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Point에 잘못된 데이터가 들어 있습니다.";
                                    return false;
                                }
                                else
                                {
                                    node.X = vPos.x;
                                    node.Y = vPos.y;
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (vPos == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Node Element에 Point가 존재하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadTopologyNodeLink(XmlTextReader reader, List<string> nodeIDs)
        {
            try
            {
                string strID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Target Element에 id가 존재하지 않습니다.";
                    return false;
                }

                nodeIDs.Add(strID);
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ConnTopologyNodeLink()
        {
            //  전층 노드를 링크 연결 필요.
            Topology.Node link;

            foreach (KeyValuePair<Topology.Node, List<string>> pair in m_dicNodeLinks)
            {
                foreach (string strNodeID in pair.Value)
                {
                    if (m_dicTopologyNodes.TryGetValue(strNodeID, out link) == false)
                    {
                        m_strErrorMessage = "ConnTopologyNodeLink() 함수에서 " + strNodeID + "는 존재하지 않는 Node ID입니다.";
                        return false;
                    }

                    pair.Key.LinkedNodes.Add(link);
                }
            }

            return true;
        }

        private bool ReadDoor(XmlTextReader reader, Level level)
        {
            try
            {
                bool stop = false;
                string strID = null, strWallID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "attachedWall", true) == 0)
                    {
                        strWallID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Door Element에 id가 존재하지 않습니다.";
                    return false;
                }

                if (strWallID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Door Element에 attachedWall이 존재하지 않습니다.";
                    return false;
                }

                Wall wall = level.FindWall(strWallID.GetHashCode());

                if (wall == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", " + strWallID + "는 존재하지 않는 Wall ID입니다.";
                    return false;
                }

                Door door = new Door();

                door.ID = strID.GetHashCode();
                door.XMLID = strID;
                wall.AddDoor(door);

                bool readWidth = false, readHeight = false, readElevation = false, readDoorType = false;
                Vertex2D vPos = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "DoorProperties", true) == 0)
                            {
                                if (ReadProperties(reader, door.Properties) == false)
                                    return false;

                                foreach (Property property in door.Properties)
                                {
                                    if (property.Name == "Thick")
                                    {
                                        double dThick;

                                        if (double.TryParse(property.Value, out dThick))
                                            door.Thick = (float)dThick;
                                    }
                                }
                            }
                            else if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                vPos = ReadPos(reader);

                                if (vPos == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Point에 잘못된 값이 들어있습니다.";
                                    return false;
                                }
                                else
                                    door.Position = vPos;
                            }
                            else if (string.Compare(reader.Name, "Width", true) == 0)
                            {
                                double dWidth;

                                if (ReadElementDouble(reader, out dWidth) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Width에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                door.Width = (float)dWidth;
                                readWidth = true;
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                double dHeight;

                                if (ReadElementDouble(reader, out dHeight) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Height에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                door.Height = (float)dHeight;
                                readHeight = true;
                            }
                            else if (string.Compare(reader.Name, "Elevation", true) == 0)
                            {
                                double dElevation;

                                if (ReadElementDouble(reader, out dElevation) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Elevation에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                door.Elevation = (float)dElevation;
                                readElevation = true;
                            }
                            else if (string.Compare(reader.Name, "DoorType", true) == 0)
                            {
                                int doorType;

                                if (ReadElementInt(reader, out doorType) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", DoorType에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                door.SetDoorType(doorType);
                                readDoorType = true;
                            }
                            else if (string.Compare(reader.Name, "Hinge1", true) == 0)
                            {
                                Vertex2D vHinge = ReadPos(reader);

                                if (vHinge == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Hinge1에 잘못된 값이 들어있습니다.";
                                    return false;
                                }
                                else
                                    door.Hinge1 = vHinge;
                            }
                            else if (string.Compare(reader.Name, "Hinge2", true) == 0)
                            {
                                Vertex2D vHinge = ReadPos(reader);

                                if (vHinge == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Hinge2에 잘못된 값이 들어있습니다.";
                                    return false;
                                }
                                else
                                    door.Hinge2 = vHinge;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (vPos == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Point가 존재하지 않습니다.";
                    return false;
                }

                if (readWidth == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Width가 존재하지 않습니다.";
                    return false;
                }

                if (readHeight == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Height가 존재하지 않습니다.";
                    return false;
                }

                if (readElevation == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Elevation이 존재하지 않습니다.";
                    return false;
                }

                if (readDoorType == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", DoorType이 존재하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadWindow(XmlTextReader reader, Level level)
        {
            try
            {
                bool stop = false;
                string strID = null, strWallID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "attachedWall", true) == 0)
                    {
                        strWallID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Window Element에 id가 존재하지 않습니다.";
                    return false;
                }

                if (strWallID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Window Element에 attachedWall이 존재하지 않습니다.";
                    return false;
                }

                Wall wall = level.FindWall(strWallID.GetHashCode());

                if (wall == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", " + strWallID + "는 존재하지 않는 Wall ID입니다.";
                    return false;
                }

                Window window = new Window();

                window.ID = strID.GetHashCode();
                window.XMLID = strID;
                wall.AddWindow(window);

                bool readWidth = false, readHeight = false, readElevation = false;
                Vertex2D vPos = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "WindowProperties", true) == 0)
                            {
                                if (ReadProperties(reader, window.Properties) == false)
                                    return false;

                                foreach (Property property in window.Properties)
                                {
                                    if (property.Name == "Thick")
                                    {
                                        double dThick;

                                        if (double.TryParse(property.Value, out dThick))
                                            window.Thick = (float)dThick;
                                    }
                                }
                            }
                            else if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                vPos = ReadPos(reader);

                                if (vPos == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Point에 잘못된 값이 들어있습니다.";
                                    return false;
                                }
                                else
                                    window.Position = vPos;
                            }
                            else if (string.Compare(reader.Name, "Width", true) == 0)
                            {
                                double dWidth;

                                if (ReadElementDouble(reader, out dWidth) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Width에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                window.Width = (float)dWidth;
                                readWidth = true;
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                double dHeight;

                                if (ReadElementDouble(reader, out dHeight) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Height에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                window.Height = (float)dHeight;
                                readHeight = true;
                            }
                            else if (string.Compare(reader.Name, "Elevation", true) == 0)
                            {
                                double dElevation;

                                if (ReadElementDouble(reader, out dElevation) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Elevation에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                window.Elevation = (float)dElevation;
                                readElevation = true;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (vPos == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Point가 존재하지 않습니다.";
                    return false;
                }

                if (readWidth == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Width가 존재하지 않습니다.";
                    return false;
                }

                if (readHeight == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Height가 존재하지 않습니다.";
                    return false;
                }

                if (readElevation == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Elevation이 존재하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private Wall ReadWall(XmlTextReader reader, Level level, Dictionary<string, Grid> dicGrids, Project project)
        {
            Wall wall = null;

            try
            {
                bool stop = false;
                string strID = null, strGridID = null, strComponentID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "grid", true) == 0)
                    {
                        strGridID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "component", true) == 0)
                    {
                        strComponentID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Wall Element에 id가 존재하지 않습니다.";
                    return wall;
                }

                if (strComponentID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Wall Element에 component가 존재하지 않습니다.";
                    return wall;
                }

                if (strGridID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Wall Element에 grid가 존재하지 않습니다.";
                    return wall;
                }

                Grid grid;
                
                if (dicGrids.TryGetValue(strGridID, out grid) == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", " + strGridID + "는 존재하지 않는 Grid ID입니다.";
                    return wall;
                }

                int hashCode = strComponentID.GetHashCode();
                Component component = project.FindComponent(hashCode);

                if (component == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", " + strComponentID + "는 존재하지 않는 Component ID입니다.";
                    return wall;
                }

                wall = new Wall();

                wall.ID = strID.GetHashCode();
                wall.XMLID = strID;
                wall.GridID = strGridID;
                SetGrid(wall, grid);
                wall.Component = component;

                bool readThick = false, readHeight = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "WallProperties", true) == 0)
                            {
                                if (ReadProperties(reader, wall.Properties) == false)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "Thickness", true) == 0)
                            {
                                double dThick;

                                if (ReadElementDouble(reader, out dThick) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Thickness에 잘못된 값이 들어있습니다.";
                                    return null;
                                }

                                wall.Thick = dThick;
                                readThick = true;
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                double dHeight;

                                if (ReadElementDouble(reader, out dHeight) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Height에 잘못된 값이 들어있습니다.";
                                    return null;
                                }

                                wall.Height = dHeight;
                                readHeight = true;
                            }
                            else if (string.Compare(reader.Name, "Boundary", true) == 0)
                            {
                                // 벽체 바운더리 읽는 곳
                                Boundary boundaryData = new Boundary();

                                boundaryData= ReadBoundary(reader);

                                if (boundaryData == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Boundary에 잘못된 값이 들어있습니다.";
                                    return null;
                                }

                                wall.BoundaryData = boundaryData;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readThick == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Thickness가 존재하지 않습니다.";
                    return null;
                }

                if (readHeight == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Height가 존재하지 않습니다.";
                    return null;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return wall;
        }

        private Space ReadSpace(XmlTextReader reader, Level level)
        {
            Space space = null;

            try
            {
                bool stop = false;
                string strID = null, strName = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "name", true) == 0)
                    {
                        strName = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Space Element에 id가 존재하지 않습니다.";
                    return null;
                }

                if (strName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Space Element에 name이 존재하지 않습니다.";
                    return null;
                }

                space = new Space();

                space.ID = strID.GetHashCode();
                space.XMLID = strID;
                space.Name = strName;
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "SpaceProperties", true) == 0)
                            {                                
                                if (ReadProperties(reader, space.Properties) == false)
                                    return null;

                                bool bStairRoom = false;
                                bool bStairType = false;

                                // 실종류 속성이 계단실일 경우에 계단실종류 속성 유무 판단
                                foreach (Property prop in space.Properties)
                                {
                                    if (prop.Name == "실종류" && prop.Value == "계단실")
                                        bStairRoom = true;

                                    if (prop.Name == "계단실종류")
                                        bStairType = true;
                                }
                                
                                // 없으면 계단실종류 속성 추가
                                if (bStairRoom == true && bStairType == false)
                                {
                                    Property property = new Property();
                                    property.Name = "계단실종류";
                                    property.Value = "일반계단";

                                    space.Properties.Add(property);
                                }
                            }
                            else if (string.Compare(reader.Name, "LinkedWall", true) == 0)
                            {
                                Wall wall = ReadLinkedWall(reader, level);

                                if (wall == null)
                                    return null;
                                else
                                    space.AddWall(wall);
                            }
                            else if (string.Compare(reader.Name, "Boundary", true) == 0)
                            {
                                // 공간 바운더리 읽는 부분
                                Boundary boundaryData = new Boundary();
                                boundaryData = ReadBoundary(reader);

                                if (boundaryData == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Boundary에 잘못된 값이 들어있습니다.";
                                    return null;
                                }

                                space.BoundaryData = boundaryData;
                            }
                            else if (string.Compare(reader.Name, "Hole", true) == 0)
                            {
                                // 홀 바운더리 읽는 부분
                                List<Boundary> holeBoundary = new List<Boundary>();
                                holeBoundary = ReadHoleBoundary(reader);

                                if (holeBoundary == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Hole에 잘못된 값이 들어있습니다.";
                                    return null;
                                }

                                space.HoleBoundary = holeBoundary;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                //ym. Space객체 방화문 유무속성
                bool flag = false;
                if(space.Properties.Count > 0)
                {
                    foreach (Property property in space.Properties)
                    {
                        if (property.Name == "방화구역유무")
                        {
                            if (property.Value == "0")
                                space.SafetyFire = false;
                            else
                                space.SafetyFire = true;

                             flag = true;
                        }
                    }                  
                }

                //속성이없거나 있어도, 방화구역속성이 없다면 default추가
                if (space.Properties.Count == 0 || !flag)
                {
                    Property prop = new Property();
                    prop.Name = "방화구역유무";
                    prop.Value = "0";
                    space.Properties.Add(prop);
                    if (prop.Value == "0")
                        space.SafetyFire = false;
                    else
                        space.SafetyFire = true;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return space;
        }

        private Wall ReadLinkedWall(XmlTextReader reader, Level level)
        {
            Wall wall = null;

            try
            {
                string strID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "link", true) == 0)
                    {
                        strID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", LinkedWall Element에 link가 존재하지 않습니다.";
                    return null;
                }

                wall = level.FindWall(strID.GetHashCode());

                if (wall == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", " + strID + "는 존재하지 않는 Wall ID입니다.";
                    return null;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return wall;
        }

        private AlertArea ReadAlertArea(XmlTextReader reader, Level level)
        {
            AlertArea alertArea = null;

            try
            {
                bool stop = false;
                string strID = null, strName = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "name", true) == 0)
                    {
                        strName = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", AlertArea Element에 id가 존재하지 않습니다.";
                    return null;
                }

                if (strName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", AlertArea Element에 name이 존재하지 않습니다.";
                    return null;
                }

                alertArea = new AlertArea();

                alertArea.ID = strID.GetHashCode();
                alertArea.XMLID = strID;
                alertArea.Name = strName;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "AlertAreaProperties", true) == 0)
                            {
                                if (ReadProperties(reader, alertArea.Properties) == false)
                                    return null;

                                bool bChkGroup = false;
                                bool bChkType = false;

                                // 속성 유무 판단
                                foreach (Property prop in alertArea.Properties)
                                {
                                    if (prop.Name == "grouping")
                                    {
                                        bChkGroup = true;
                                        FormMain.Instance.AddAlertAreaGroup(prop.Value);
                                    }  
                                    else if (prop.Name == "alertAreaType")
                                    {
                                        bChkType = true;
                                        FormMain.Instance.AddAlertAreaType(prop.Value);
                                    }
                                        

                                }

                                // 없으면 속성 추가
                                if (bChkGroup == false)
                                {
                                    Property property = new Property();
                                    property.Name = "grouping";
                                    property.Value = "";

                                    alertArea.Properties.Add(property);
                                }
                                if (bChkType == false)
                                {
                                    Property property = new Property();
                                    property.Name = "alertAreaType";
                                    property.Value = "";

                                    alertArea.Properties.Add(property);
                                }

                            }
                            else if (string.Compare(reader.Name, "Boundary", true) == 0)
                            {
                                // 공간 바운더리 읽는 부분
                                Boundary boundaryData = new Boundary();
                                boundaryData = ReadBoundary(reader);

                                if (boundaryData == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", AlertArea Boundary에 잘못된 값이 들어있습니다.";
                                    return null;
                                }

                                alertArea.Boundary = boundaryData;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return alertArea;
        }

        private void SetGrid(Wall wall, Grid grid)
        {
            if (grid.Line != null)
            {
                wall.SetGridType((int)Wall.GridType.Line);
                wall.Line = grid.Line;
            }
            else if (grid.Arc != null)
            {
                wall.SetGridType((int)Wall.GridType.Arc);
                wall.Arc = grid.Arc;
            }
            else if (grid.EArc != null)
            {
                wall.SetGridType((int)Wall.GridType.EArc);
                wall.EArc = grid.EArc;
            }
        }

        private bool ReadGridCollection(XmlTextReader reader, Dictionary<string, Grid> dicGrids)
        {
            try
            {
                bool stop = false;
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Grid", true) == 0)
                            {
                                Grid grid = ReadGrid(reader);

                                if (grid == null)
                                    return false;
                                else
                                    dicGrids[grid.ID] = grid;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private Grid ReadGrid(XmlTextReader reader)
        {
            Grid grid = null;

            try
            {
                bool stop = false;
                string strID = null;
                
                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Grid Element에 id가 존재하지 않습니다.";
                    return grid;
                }

                grid = new Grid();
                grid.ID = strID;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Line", true) == 0)
                            {
                                grid.Line = ReadGridLine(reader);

                                if (grid.Line == null)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "Arc", true) == 0)
                            {
                                grid.Arc = ReadGridArc(reader);

                                if (grid.Arc == null)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "EArc", true) == 0)
                            {
                                grid.EArc = ReadGridEArc(reader);

                                if (grid.EArc == null)
                                    return null;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            if (grid.Line == null && grid.Arc == null && grid.EArc == null)
            {
                m_strErrorMessage = GetLineCountString(reader) + ", Grid에 Line, Arc, EArc 가운데 적어도 하나는 존재해야 합니다.";
                return null;
            }

            return grid;
        }

        private Line2D ReadGridLine(XmlTextReader reader)
        {
            Line2D line = new Line2D();

            try
            {
                bool stop = false;
                bool isFirst = true;
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Pos", true) == 0)
                            {
                                Vertex2D vertex = ReadElementVertex(reader);

                                if (vertex == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Pos에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }

                                line.SetVertex(vertex, isFirst);
                                isFirst = false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return line;
        }

        private Arc2D ReadGridArc(XmlTextReader reader)
        {
            Arc2D arc = null;

            try
            {
                bool stop = false, readRadius = false, readBeginAngle = false, readAngle = false, readClockwise = false;
                Vertex2D vCenter = null;
                double dRadius = 0.0, beginAngle = 0.0, angle = 0.0;
                bool isClockwise = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Center", true) == 0)
                            {
                                vCenter = ReadPos(reader);

                                if (vCenter == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Center에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "Radius", true) == 0)
                            {
                                readRadius = ReadElementDouble(reader, out dRadius);

                                if (readRadius == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Radius에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "BeginAngle", true) == 0)
                            {
                                readBeginAngle = ReadElementDouble(reader, out beginAngle);

                                if (readBeginAngle == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", BeginAngle에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                readAngle = ReadElementDouble(reader, out angle);

                                if (readAngle == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Angle에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "ClockWise", true) == 0)
                            {
                                readClockwise = ReadElementBoolean(reader, out isClockwise);

                                if (readClockwise == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", ClockWise에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (vCenter == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Center가 존재하지 않습니다.";
                    return null;
                }

                if (readRadius == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Radius가 존재하지 않습니다.";
                    return null;
                }

                if (readBeginAngle == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", BeginAngle이 존재하지 않습니다.";
                    return null;
                }

                if (readAngle == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Angle이 존재하지 않습니다.";
                    return null;
                }

                if (readClockwise == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", ClockWise가 존재하지 않습니다.";
                    return null;
                }

                arc = new Arc2D(vCenter, dRadius, beginAngle, angle, isClockwise);
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return arc;
        }

        private EArc2D ReadGridEArc(XmlTextReader reader)
        {
            EArc2D earc = null;

            try
            {
                bool stop = false, readBeginAngle = false, readAngle = false, readClockwise = false;
                Vertex2D vTL = null, vBL = null, vBR = null;
                double beginAngle = 0.0, angle = 0.0;
                bool isClockwise = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TL", true) == 0)
                            {
                                vTL = ReadPos(reader);

                                if (vTL == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", TL에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "BL", true) == 0)
                            {
                                vBL = ReadPos(reader);

                                if (vBL == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", BL에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "BR", true) == 0)
                            {
                                vBR = ReadPos(reader);

                                if (vBR == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", BR에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "BeginAngle", true) == 0)
                            {
                                readBeginAngle = ReadElementDouble(reader, out beginAngle);

                                if (readBeginAngle == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", BeginAngle에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                readAngle = ReadElementDouble(reader, out angle);

                                if (readAngle == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Angle에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "ClockWise", true) == 0)
                            {
                                readClockwise = ReadElementBoolean(reader, out isClockwise);

                                if (readClockwise == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", ClockWise에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (vTL == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", TL이 존재하지 않습니다.";
                    return null;
                }

                if (vBL == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", BL이 존재하지 않습니다.";
                    return null;
                }

                if (vBR == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", BR이 존재하지 않습니다.";
                    return null;
                }

                if (readBeginAngle == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", BeginAngle이 존재하지 않습니다.";
                    return null;
                }

                if (readAngle == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Angle이 존재하지 않습니다.";
                    return null;
                }

                if (readClockwise == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", ClockWise가 존재하지 않습니다.";
                    return null;
                }

                earc = new EArc2D(vTL, vBL, vBR, beginAngle, angle, isClockwise);
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return earc;
        }

        private Vertex2D ReadPos(XmlTextReader reader)
        {
            Vertex2D vertex = null;

            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Pos", true) == 0)
                            {
                                vertex = ReadElementVertex(reader);

                                if (vertex == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Pos에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return vertex;
        }

        private bool ReadCommon(XmlTextReader reader, Project project, Dictionary<int, POIType> dicPOITypes)
        {
            if (project == null)
            {
                m_strErrorMessage = "ProjectInfo 정보가 존재하지 않습니다.";
                return false;
            }

            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Components", true) == 0)
                            {
                                if (!ReadComponents(reader, project))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "POITypes", true) == 0)
                            {
                                if (!ReadPOITypes(reader, dicPOITypes))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadPOITypes(XmlTextReader reader, Dictionary<int, POIType> dicPOITypes)
        {
            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "POITypeGroup", true) == 0)
                            {
                                if (ReadPOIType(reader, null, true, dicPOITypes, reader.IsEmptyElement) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private int m_nID = 0;

        private bool ReadPOIType(XmlTextReader reader, POIType parent, bool isGroup, Dictionary<int, POIType> dicPOITypes, bool isEmptyElement)
        {
            try
            {
                bool stop = false;
                string strID = null, strName = null, strUserDefined = null;
                string strCode = null, strDefaultHeight = null, strPOICode = null;

                while (reader.MoveToNextAttribute())
                {
                    //if (string.Compare(reader.Name, "id", true) == 0)
                    //{
                    //    strID = reader.Value;
                    //}
                    //else 
                    if (string.Compare(reader.Name, "name", true) == 0)
                    {
                        strName = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "userDefined", true) == 0)
                    {
                        strUserDefined = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "code", true) == 0)
                    {
                        strCode = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "defaultHeight", true) == 0)
                    {
                        strDefaultHeight = reader.Value;
                    }
                }

                //if (strID == null)
                //{
                //    m_strErrorMessage = GetLineCountString(reader) + ", POITypeGroup Element에 id 속성이 존재하지 않습니다.";
                //    return false;
                //}

                if (strName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POITypeGroup Element에 name 속성이 존재하지 않습니다.";
                    return false;
                }

                if (strUserDefined == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POITypeGroup Element에 userDefined 속성이 존재하지 않습니다.";
                    return false;
                }

                strUserDefined = strUserDefined.ToLower();
                bool isUserDefined = false;

                if (strUserDefined == "true" || strUserDefined == "1")
                    isUserDefined = true;
                else if (strUserDefined == "false" || strUserDefined == "0")
                    isUserDefined = false;
                else
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POITypeGroup Element에 userDefined 속성이 잘못된 데이터를 가집니다.";
                    return false;
                }

                if (!isUserDefined && strCode == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POITypeGroup Element에 Code 속성이 존재하지 않습니다.";
                    return false;
                }

                POIType poiTypetmp = null;
                POIType poiType = null;
                int nID = ++m_nID;//strID.GetHashCode();
                strID = m_nID.ToString();

                if (dicPOITypes.ContainsKey(nID))
                {
                    //System.Windows.Forms.MessageBox.Show("error");
                }

                bool bExist = dicPOITypes.TryGetValue(nID, out poiTypetmp);

                if (!bExist)
                {
                    poiType = new POIType();

                    poiType.ID = nID;
                    poiType.XMLID = strID;
                    poiType.Name = strName;
                    poiType.UserDefined = isUserDefined;
                    poiType.DefaultHeight = strDefaultHeight;
                    poiType.Parent = parent;
                    //  poiType.ParentID = (parent == null) ? 0 : parent.ID;
                    poiType.IsGroup = isGroup;

                    strPOICode = "";
                    //poiType.Code = (strCode == null) ? "" : strCode;
                    if (strCode != null && parent != null)
                    {
                        strPOICode = parent.Code + strCode;
                        poiType.Code = strPOICode;
                    }
                    else if (strCode != null)
                    {
                        poiType.Code = strCode;
                    }
                    else
                    {
                        poiType.Code = "";
                    }

                    if (poiType.Code != "" && poiType.IsGroup == false)
                        poiType.Code = CheckPOICode(poiType.Code);

                    dicPOITypes[nID] = poiType;
                }
                else
                {
                    if (poiTypetmp.IsGroup)
                    {
                        poiType = new POIType();
                        poiType.ID = nID;
                        poiType.XMLID = strID;
                        poiType.Name = strName;
                        poiType.UserDefined = isUserDefined;
                        poiType.DefaultHeight = strDefaultHeight;
                        poiType.Parent = poiTypetmp;
                        poiType.IsGroup = isGroup;

                        strPOICode = "";
                        //poiType.Code = (strCode == null) ? "" : strCode;
                        if (strCode != null && parent != null)
                        {
                            strPOICode = parent.Code + strCode;
                            poiType.Code = strPOICode;
                        }
                        else if (strCode != null)
                        {
                            poiType.Code = strCode;
                        }
                        else
                        {
                            poiType.Code = "";
                        }

                        if (poiType.Code != "" && poiType.IsGroup == false)
                            poiType.Code = CheckPOICode(poiType.Code);

                        dicPOITypes[nID] = poiType;
                    }
                }

                // Line POIType
                if (m_strPOIWireTable.ContainsKey(strCode))
                {
                    Property prop = new Property();
                    prop.Name = "Wire";
                    prop.Value = "1";
                    prop.Description = "배선심볼로 사용되는가?";
                    poiType.Properties.Add(prop);
                }

                if (isEmptyElement)
                    return true;

                //if (isGroup)
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (string.Compare(reader.Name, "POITypeProperties", true) == 0)
                                {
                                    if (ReadProperties(reader, poiType.Properties) == false)
                                        return false;
                                }
                                else if (string.Compare(reader.Name, "POITypeGroup", true) == 0)
                                {
                                    if (ReadPOIType(reader, poiType, true, dicPOITypes, reader.IsEmptyElement) == false)
                                        return false;
                                }
                                else if (string.Compare(reader.Name, "POIType", true) == 0)
                                {
                                    if (isGroup)
                                        parent = poiType;

                                    if (ReadPOIType(reader, parent, false, dicPOITypes, reader.IsEmptyElement) == false)
                                        return false;
                                }
                                else
                                    PassElement(reader);

                                break;

                            case XmlNodeType.EndElement:
                                stop = true;
                                break;
                        }

                        if (stop)
                            break;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return false;
        }

        private string CheckPOICode(string strCode)
        {
            string strPOICode = strCode;

            for (int i = strPOICode.Length; i < 5; i++)
            {
                strPOICode += "0";
            }

            return strPOICode;
        }

        private bool ReadComponents(XmlTextReader reader, Project project)
        {
            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Component", true) == 0)
                            {
                                Component component = ReadComponent(reader);

                                if (component == null)
                                    return false;
                                else
                                    project.AddComponent(component);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private Component ReadComponent(XmlTextReader reader)
        {
            try
            {
                bool emtpyElement = reader.IsEmptyElement;
                string strTypeName = null, strComponentID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strComponentID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "type", true) == 0)
                    {
                        strTypeName = reader.Value;
                    }
                }

                if (strComponentID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Component Element에 id가 존재하지 않습니다.";
                    return null;
                }

                if (strTypeName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Component Element에 type이 존재하지 않습니다.";
                    return null;
                }

                Component component = new Component();

                component.TypeName = strTypeName;
                component.XMLID = strComponentID;
                component.ID = strComponentID.GetHashCode();

                if (emtpyElement)
                    return component;

                string strComponentName = "";
                ReadElementText(reader, ref strComponentName);

                component.ComponentName = strComponentName;
                return component;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return null;
        }

        private Project ReadProject(XmlTextReader reader)
        {
            Project project = null;

            try
            {
                bool emptyElement = reader.IsEmptyElement;
                    //return project;

                bool stop = false;
                string strName = null, strUnit = null, strDate = null, strAuthor = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "name", true) == 0)
                    {
                        strName = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "unit", true) == 0)
                    {
                        strUnit = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "datetime", true) == 0)
                    {
                        strDate = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "author", true) == 0)
                    {
                        strAuthor = reader.Value;
                    }
                }

                if (strName == null)
                {
                    // 건물 이름이 없을 경우도 있음
                    //m_strErrorMessage = GetLineCountString(reader) + ", ProjectInfo Element에 name 속성이 존재하지 않습니다.";
                    //return project;
                    strName = "테스트 프로젝트";
                }

                if (strUnit == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", ProjectInfo Element에 unit 속성이 존재하지 않습니다.";
                    return project;
                }

                if (strDate == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", ProjectInfo Element에 datetime 속성이 존재하지 않습니다.";
                    return project;
                }

                project = new Project();

                project.Name = strName;
                project.Unit = Project.GetUnit(strUnit);

                try
                {
                    project.TimeStamp = Convert.ToDateTime(strDate);
                }
                catch (Exception)
                {
                }

                if (strAuthor != null)
                    project.Author = strAuthor;

                if (emptyElement)
                    return project;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ProjectProperties", true) == 0)
                            {
                                if (ReadProperties(reader, project.Properties) == false)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "AnchorNode", true) == 0)
                            {
                                // 앵커노드 읽는 부분
                                if (ReadAnchorNode(reader, project.AnchorNode) == false)
                                    return null;
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return null;
            }

            return project;
        }

        private Boundary ReadBoundary(XmlTextReader reader)
        {
            Boundary boundary = new Boundary();

            try
            {
                if (reader.IsEmptyElement)
                    return null;

                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Line", true) == 0)
                            {
                                Line2D line = null;
                                line = ReadGridLine(reader);

                                if (line == null)
                                    return null;

                                boundary.AddLine(line);
                            }
                            else if (string.Compare(reader.Name, "Arc", true) == 0)
                            {
                                Arc2D arc = null;
                                arc = ReadGridArc(reader);

                                if (arc == null)
                                    return null;

                                boundary.AddArc(arc);
                            }
                            else if (string.Compare(reader.Name, "EArc", true) == 0)
                            {
                                EArc2D eArc = null;
                                eArc = ReadGridEArc(reader);

                                if (eArc == null)
                                    return null;

                                boundary.AddEArc(eArc);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return null;
            }

            return boundary;
        }

        private List<Boundary> ReadHoleBoundary(XmlTextReader reader)
        {
            List<Boundary> holeBoundary = new List<Boundary>();

            try
            {
                if (reader.IsEmptyElement)
                    return null;

                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Boundary", true) == 0)
                            {
                                Boundary boundary = new Boundary();
                                boundary = ReadBoundary(reader);

                                if (boundary == null)
                                    return null;

                                holeBoundary.Add(boundary);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return null;
            }

            return holeBoundary;
        }

        private bool ReadProperties(XmlTextReader reader, List<Property> properties)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return true;

                bool stop = false;
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Property", true) == 0)
                            {
                                Property property = ReadProperty(reader);

                                if (property == null)
                                    return false;
                                else
                                    properties.Add(property);
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return false;
            }

            return true;
        }

        private bool ReadAnchorNode(XmlTextReader reader, AnchorNode anchor)
        {
            // 앵커노드 읽기
            try
            {
                if (reader.IsEmptyElement)
                    return true;

                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Local", true) == 0)
                            {
                                anchor.Local = new Local();

                                if (ReadLocal(reader, anchor.Local) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Global", true) == 0)
                            {
                                anchor.Global = new Global();

                                if (ReadGlobal(reader, anchor.Global) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "AnchorNodeProperties", true) == 0)
                            {
                                if (ReadProperties(reader, anchor.Properties) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return false;
            }

            return true;
        }

        private bool ReadGlobal(XmlTextReader reader, Global global)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return true;

                bool stop = false;
                string strUnit = null;

                
                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "unit", true) == 0)
                    {
                        strUnit = reader.Value;
                    }
                }

                // TODO: 테스트로 인한 임시 주석처리 >> 노아쪽에 수정요청해야함!! 다운로드 시에 unit 속성이 빠짐
                //if (strUnit == null)
                //{
                //    m_strErrorMessage = GetLineCountString(reader) + ", AnchorNode Global에 unit 속성이 존재하지 않습니다.";
                //    return false;
                //}

                if (strUnit != null)
                    global.Unit = Global.GetUnit(strUnit);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Pos", true) == 0)
                            {
                                Vertex2D vertex = ReadElementVertex(reader);

                                if (vertex == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Pos에 잘못된 값이 들어 있습니다.";
                                    return false;
                                }

                                global.Position = vertex;
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return false;
            }

            return true;
        }

        private bool ReadLocal(XmlTextReader reader, Local local)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return true;

                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Pos", true) == 0)
                            {
                                Vertex2D vertex = ReadElementVertex(reader);

                                if (vertex == null)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Pos에 잘못된 값이 들어 있습니다.";
                                    return false;
                                }

                                local.Position = vertex;
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                double dAngle;

                                if (ReadElementDouble(reader, out dAngle) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Angle에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                local.Angle = dAngle;
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return false;
            }

            return true;
        }

        private Property ReadProperty(XmlTextReader reader)
        {
            Property property = null;

            try
            {
                if (reader.IsEmptyElement)
                    return property;

                property = new Property();

                bool stop = false;
                string strName = null, strValue = null, strDescription = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Name", true) == 0)
                            {
                                if (ReadElementText(reader, ref strName) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Name에 잘못된 값이 존재합니다.";
                                    return null;
                                }
                                else
                                    property.Name = strName;
                            }
                            else if (string.Compare(reader.Name, "Value", true) == 0)
                            {
                                if (ReadElementText(reader, ref strValue) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Value에 잘못된 값이 존재합니다.";
                                    return null;
                                }
                                else
                                    property.Value = strValue;
                            }
                            else if (string.Compare(reader.Name, "Description", true) == 0)
                            {
                                if (ReadElementText(reader, ref strDescription))
                                    property.Description = strDescription;
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (strName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Property Element에 Name이 존재하지 않습니다.";
                    return null;
                }

                if (strValue == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Property Element에 Value가 존재하지 않습니다.";
                    return null;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return null;
            }

            return property;
        }

        private bool ReadElementText(XmlTextReader reader, ref string strText)
        {
            bool stop = false;
            strText = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strText = reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private Vertex2D ReadElementVertex(XmlTextReader reader)
        {
            bool stop = false;
            string strVertex = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strVertex = reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (strVertex == null)
                return null;

            string[] tokens = strVertex.Split(',');

            if (tokens.Count() != 2)
                return null;

            string strX = tokens[0].Trim();
            string strY = tokens[1].Trim();

            double x, y;

            if (double.TryParse(strX, out x) == false || double.TryParse(strY, out y) == false)
                return null;

            return new Vertex2D(x, y);
        }

        private bool ReadElementInt(XmlTextReader reader, out int data)
        {
            bool stop = false;
            string strData = null;
            data = 0;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strData = reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (strData == null)
                return false;

            return int.TryParse(strData, out data);
        }

        private bool ReadElementDouble(XmlTextReader reader, out double data)
        {
            bool stop = false;
            string strData = null;
            data = 0.0;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strData = reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (strData == null)
                return false;

            return double.TryParse(strData, out data);
        }

        private bool ReadElementBoolean(XmlTextReader reader, out bool data)
        {
            bool stop = false;
            string strData = null;
            data = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strData = reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (strData == null)
                return false;

            if (strData == "1" || string.Compare(strData, "true", true) == 0)
                data = true;
            else if (strData == "0" || string.Compare(strData, "false", true) == 0)
                data = false;
            else
                return false;

            return true;
        }

        private string GetLineCountString(XmlTextReader reader)
        {
            return "Line : " + reader.LineNumber.ToString();
        }

        private void PassElement(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        PassElement(reader);
                        break;
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }
    }

    // struct와 같이 null이 허용되지 않는 데이터를 위한 Wrapper 클래스
    public class VariousData<DataType>
    {
        private DataType data;

        public DataType Data
        {
            get { return data; }
            set { data = value; }
        }

        public VariousData()
        {
        }

        public VariousData(DataType data)
        {
            this.data = data;
        }
    }

    class Grid
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
}
