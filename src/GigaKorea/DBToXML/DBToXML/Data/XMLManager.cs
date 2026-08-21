using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using System.Xml;

namespace DBToXML.Data
{
    public class XMLManager
    {
        private string m_strVersion = "1.2";
        private string m_strDoubleFormat = "F1";

        public bool SaveXML(string strFilePath, Project project, WebDBManager dbMgr)
        {
            bool result = true;

            try
            {
                Dictionary<int, POIType> dicPOITypes = POIType.ReadPOIType(dbMgr);
                project.POITypes = dicPOITypes;

                XmlTextWriter writer = new XmlTextWriter(strFilePath, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                result = WriteIndoorModelFile(project, writer, dbMgr);

                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return result;
        }

        private bool WriteIndoorModelFile(Project project, XmlTextWriter writer, WebDBManager dbMgr)
        {
            try
            {
                writer.WriteStartElement("IndoorModelFile");

                writer.WriteStartAttribute("version");
                writer.WriteString(m_strVersion);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("xmlns:xsi");
                writer.WriteString("http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("xsi:noNamespaceSchemaLocation");
                writer.WriteString("http://unes.iptime.org:8001/Schema/InSafetyML.xsd");
                writer.WriteEndAttribute();

                if (WriteProjectInfo(project, writer, dbMgr) == false)
                    return false;
                if (WriteLevels(project, writer, dbMgr) == false)
                    return false;
                if (WriteCommons(project.Materials, project.POITypes, writer) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteCommons(List<Material> materials, Dictionary<int, POIType> dicPOITypes, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Common");
                writer.WriteStartElement("Components");

                foreach (Material material in materials)
                {
                    if (WriteComponent(material, writer) == false)
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
                writer.WriteString(poiType.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("name");
                writer.WriteString(poiType.Name);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("userDefined");
                writer.WriteString(GetBooleanString(poiType.IsUserDefined));
                writer.WriteEndAttribute();

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
                writer.WriteString(poiType.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("name");
                writer.WriteString(poiType.Name);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("userDefined");
                writer.WriteString(GetBooleanString(poiType.IsUserDefined));
                writer.WriteEndAttribute();

                if (poiType.Code != null && poiType.Code.Length > 0)
                {
                    writer.WriteStartAttribute("code");
                    writer.WriteString(poiType.Code);
                    writer.WriteEndAttribute();
                }

                if (poiType.DefaultHeight != null)
                {
                    writer.WriteStartAttribute("defaultHeight");
                    writer.WriteString(GetDoubleString(poiType.DefaultHeight.Data));
                    writer.WriteEndAttribute();
                }

                if (WriteProperty(poiType.Properties, writer) == 0)
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

        private bool WriteComponent(Material material, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Component");

                writer.WriteStartAttribute("id");
                writer.WriteString(material.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("type");
                writer.WriteString(material.TypeName);
                writer.WriteEndAttribute();

                writer.WriteString(material.MaterialName);

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteLevels(Project project, XmlTextWriter writer, WebDBManager dbMgr)
        {
            try
            {
                List<Level> levels = Level.ReadLevel(project, dbMgr);

                writer.WriteStartElement("Levels");

                foreach (Level floor in levels)
                {
                    if (WriteLevel(floor, writer) == false)
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

        private bool WriteLevel(Level floor, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Level");

                writer.WriteStartAttribute("id");
                writer.WriteString(floor.ID);
                writer.WriteEndAttribute();

                WriteProperty(floor.Properties, writer);

                writer.WriteStartElement("Name");
                writer.WriteString(floor.Name);
                writer.WriteFullEndElement();

                writer.WriteStartElement("Elevation");
                writer.WriteString(GetDoubleString(floor.Elevation));
                writer.WriteFullEndElement();

                if (WriteGridCollection(floor.Walls, writer) == false)
                    return false;

                if (WriteElementCollection(floor, writer) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteElementCollection(Level floor, XmlTextWriter writer)
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

                foreach (Topology topology in floor.Topologies)
                {
                    if (WriteTopology(topology, writer) == false)
                        return false;
                }

                foreach (POI poi in floor.POIs)
                {
                    if (WritePOI(poi, writer) == false)
                        return false;
                }

                foreach (POIWire wire in floor.Wires)
                {
                    if (WritePOIWire(wire, writer) == false)
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

        private bool WritePOIWire(POIWire wire, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("POIWire");

                writer.WriteStartAttribute("id");
                writer.WriteString(wire.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("beginPOI");
                writer.WriteString(wire.BeginPOI.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("endPOI");
                writer.WriteString(wire.EndPOI.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("type");
                writer.WriteString(wire.POIType.ID);
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

        private bool WritePOI(POI poi, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("POI");

                writer.WriteStartAttribute("id");
                writer.WriteString(poi.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("type");
                writer.WriteString(poi.POIType.ID);
                writer.WriteEndAttribute();

                WriteProperty(poi.Properties, writer);

                writer.WriteStartElement("Name");
                writer.WriteString(poi.Name);
                writer.WriteFullEndElement();

                WriteVertexElement("Point", poi.Position, writer);

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
                writer.WriteString(topology.ID);
                writer.WriteEndAttribute();

                WriteProperty(topology.Properties, writer);

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
                writer.WriteString(node.ID);
                writer.WriteEndAttribute();

                WriteProperty(node.Properties, writer);

                foreach (Topology.Node link in node.LinkedNodes)
                {
                    writer.WriteStartElement("Target");

                    writer.WriteStartAttribute("id");
                    writer.WriteString(link.ID);
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
                writer.WriteString(door.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("attachedWall");
                writer.WriteString(wall.ID);
                writer.WriteEndAttribute();

                WriteProperty(door.Properties, writer);

                WriteVertexElement("Point", new UnE.Geometry.Vertex2D(door.X, door.Y), writer);
                
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
                    WriteVertexElement("Hinge2", door.Hinge2, writer);
                }

                writer.WriteStartElement("DoorType");
                writer.WriteString(((int)door.Type).ToString());
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
                writer.WriteString(window.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("attachedWall");
                writer.WriteString(wall.ID);
                writer.WriteEndAttribute();

                WriteProperty(window.Properties, writer);

                WriteVertexElement("Point", new UnE.Geometry.Vertex2D(window.X, window.Y), writer);
                
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
                writer.WriteString(space.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("name");
                writer.WriteString(space.Name);
                writer.WriteEndAttribute();

                WriteProperty(space.Properties, writer);

                foreach (Wall wall in space.Walls)
                {
                    writer.WriteStartElement("LinkedWall");

                    writer.WriteStartAttribute("link");
                    writer.WriteString(wall.ID);
                    writer.WriteEndAttribute();

                    writer.WriteEndElement();
                }

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
                writer.WriteString(wall.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("grid");
                writer.WriteString(wall.GridID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("component");
                writer.WriteString(wall.Material.ID);
                writer.WriteEndAttribute();

                WriteProperty(wall.Properties, writer);

                writer.WriteStartElement("Thickness");
                writer.WriteString(GetDoubleString(wall.Thick));
                writer.WriteFullEndElement();

                writer.WriteStartElement("Height");
                writer.WriteString(GetDoubleString(wall.Height));
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
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

        private bool WriteProjectInfo(Project project, XmlTextWriter writer, WebDBManager dbMgr)
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

                if (project.UnitType == Project.UnitOfLength.MM)
                    m_strDoubleFormat = "F0";
                else if (project.UnitType == Project.UnitOfLength.CM)
                    m_strDoubleFormat = "F1";
                else if (project.UnitType == Project.UnitOfLength.Meter)
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

                List<Property> projectProperties = project.ReadProperty(dbMgr);
                WriteProperty(projectProperties, writer);

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private int WriteProperty(List<Property> properties, XmlTextWriter writer)
        {
            if (properties.Count == 0)
                return 0;

            Property first = properties[0];
            writer.WriteStartElement(first.GroupName);

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

        private string GetDoubleString(double data)
        {
            return string.Format("{0:" + m_strDoubleFormat + "}", data);
        }
    }
}
