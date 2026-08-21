using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
//using DBUtility2;
using System.Collections;
using UnE.Geometry;

namespace CadToXML
{
    class XMLManager
    {
        private static string XML_VERSION = "1.5";
        private string m_strDoubleFormat = "F1";
        private string m_strError = "";

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
            get { return m_strError; }
        }

        public static string MakeNewFileName(string strOldFileName)
        {
            int nIndex = strOldFileName.LastIndexOf('.');

            if (nIndex < 0)
            {
                return strOldFileName + "_" + XML_VERSION;
            }

            string strFileName = strOldFileName.Substring(0, nIndex);
            string strExt = strOldFileName.Substring(nIndex);
            return string.Format("{0}_{1}{2}", strFileName, XML_VERSION, strExt);
        }

        public bool Save(string strPath, Project project, Dictionary<int, POIType> dicPOITypes, string strVersion = CommonString.XML_VERSION)
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(strPath, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                WriteIndoorModelFile(project, dicPOITypes, writer, strVersion);

                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteIndoorModelFile(Project project, Dictionary<int, POIType> dicPOITypes, XmlTextWriter writer, string strVersion)
        {
            try
            {
                writer.WriteStartElement("IndoorModelFile");

                writer.WriteStartAttribute("version");
                writer.WriteString(strVersion);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("xmlns:xsi");
                writer.WriteString("http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("xsi:noNamespaceSchemaLocation");
                writer.WriteString("http://unes.iptime.org:8001/Schema/InSafetyML.xsd");
                writer.WriteEndAttribute();

                if (WriteProjectInfo(project, writer) == false)
                    return false;
                if (WriteLevels(project.Floors, writer, strVersion) == false)
                    return false;
                if (WriteCommons(Material.Materials, dicPOITypes, writer) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
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
                // WritePOITypes(writer);//db sql에서 읽어온거 쓰기

                //POIType.xml 에서 읽어온 poiTypes 부분 쓰기.ym
                foreach (KeyValuePair<int, POIType> pair in dicPOITypes)
                {
                    if (pair.Value.Parent == null)
                    {
                        if (WritePOIType_xml(pair.Value, writer) == false)
                            return false;
                    }
                }


                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }
        private bool WritePOIType_xml(POIType poiType, XmlTextWriter writer)
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
                writer.WriteString(GetBooleanString(poiType.IsUserDefined));
                writer.WriteEndAttribute();

                if (poiType.Code != null && poiType.Code.Length > 0)
                {
                    //writer.WriteStartAttribute("code");
                    //writer.WriteString(poiType.Code);
                    //writer.WriteEndAttribute();

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
                writer.WriteString(GetBooleanString(poiType.IsUserDefined));
                writer.WriteEndAttribute();

                if (poiType.Code != null && poiType.Code.Length > 0)
                {
                    //writer.WriteStartAttribute("code");
                    //writer.WriteString(poiType.Code);
                    //writer.WriteEndAttribute();

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

        private string GetBooleanString(bool value)
        {
            return value ? "true" : "false";
        }
        private void WritePOIType(POIType poiType, XmlTextWriter writer)
        {
            string strElementName = poiType.IsGroup ? "POITypeGroup" : "POIType";

            writer.WriteStartElement(strElementName);

            writer.WriteStartAttribute("id");
            writer.WriteString("poiType" + poiType.ID.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("name");
            writer.WriteString(poiType.Name);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("userDefined");
            writer.WriteString(poiType.IsUserDefined ? "true" : "false");
            writer.WriteEndAttribute();

            if (poiType.Code != null && poiType.Code.Length > 0)
            {
                writer.WriteStartAttribute("code");
                writer.WriteString(poiType.Code);
                writer.WriteEndAttribute();
            }

            if (poiType.IsGroup == false && poiType.DefaultHeight != null && poiType.DefaultHeight.Length > 0)
            {
                writer.WriteStartAttribute("defaultHeight");
                writer.WriteString(poiType.DefaultHeight);
                writer.WriteEndAttribute();
            }

            foreach (POIType child in poiType.ChildTypes)
            {
                WritePOIType(child, writer);
            }

            if (poiType.IsGroup == false && poiType.IsWireType)
            {
                writer.WriteStartElement("POITypeProperties");

                writer.WriteStartElement("Property");

                writer.WriteStartElement("Name");
                writer.WriteString("Wire");
                writer.WriteFullEndElement();

                writer.WriteStartElement("Value");
                writer.WriteString("1");
                writer.WriteFullEndElement();

                writer.WriteStartElement("Description");
                writer.WriteString("배선심볼로 사용되는가?");
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }

            if (poiType.IsGroup == false && poiType.IsWireType == false)
                writer.WriteEndElement();
            else
                writer.WriteFullEndElement();
        }

        /*private void WritePOITypes(XmlTextWriter writer)
        {
            WebDBManager dbMgr = new WebDBManager("UnE_BIM", 1);

            string strSQL = "Select ID, IsGroup, ParentID, Name, Code, IsUserDefined, DefaultHeight from POIType";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            Dictionary<int, POIType> dicPOITypes = new Dictionary<int, POIType>();
            Dictionary<POIType, int> dicParentID = new Dictionary<POIType, int>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-6;i+=7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> isGroup = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 3]);
                string strCode = WebDBManager.GetStringField(arrResult[i + 4]);
                VariousData<int> isUserDefined = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                string strDefaultHeight = WebDBManager.GetStringField(arrResult[i + 6]);

                if (id == null || isGroup == null || strName == null || isUserDefined == null)
                    continue;

                POIType poiType = new POIType();

                poiType.ID = id.Data;
                poiType.IsGroup = isGroup.Data == 1;
                poiType.Name = strName;
                poiType.IsUserDefined = isUserDefined.Data == 1;

                if (strCode != null)
                    poiType.Code = strCode;

                if (strDefaultHeight != null)
                    poiType.DefaultHeight = strDefaultHeight;

                dicPOITypes[id.Data] = poiType;

                if (parentID != null)
                    dicParentID[poiType] = parentID.Data;
            }

            foreach (KeyValuePair<POIType, int> pair in dicParentID)
            {
                POIType poiType = null;

                if (dicPOITypes.TryGetValue(pair.Value, out poiType))
                {
                    pair.Key.Parent = poiType;
                }
            }

            strSQL = "Select POITypeID, PropertyName, PropertyValue from POITypeProperty";
            arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strPropertyName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 2]);

                if (id == null || strPropertyName == null || strPropertyValue == null)
                    continue;

                POIType poiType = null;

                if (dicPOITypes.TryGetValue(id.Data, out poiType) == false)
                    continue;

                if (strPropertyName == "Wire")
                {
                    poiType.IsWireType = strPropertyValue == "1";
                }
            }

            foreach (KeyValuePair<int, POIType> pair in dicPOITypes)
            {
                POIType poiType = pair.Value;

                if (poiType.Parent == null)
                {
                    WritePOIType(poiType, writer);
                }
            }
        }*/

        private bool WriteComponent(Material material, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Component");

                writer.WriteStartAttribute("id");
                writer.WriteString(ChangeWallComponent(material.ID));
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("type");
                writer.WriteString(material.TypeName);
                writer.WriteEndAttribute();

                //ym.component정의에는 재질넣지않기. 재질은 속성에만 넣기
                //writer.WriteString(material.MaterialName);

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteLevels(List<Floor> floors, XmlTextWriter writer, string strVersion)
        {
            try
            {
                writer.WriteStartElement("Levels");

                foreach (Floor floor in floors)
                {
                    if (WriteLevel(floor, writer, strVersion) == false)
                        return false;
                }

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }
        //ym.0826. Level Height 프로퍼티추가
        private void WriteLevelProperty(Floor floor, XmlTextWriter writer)
        {
            writer.WriteStartElement("LevelProperties");

            foreach (Property tmpprty in floor.Properties)
            {
                WriteProperty(tmpprty.Name, tmpprty.Value, null, writer);
            }

            writer.WriteFullEndElement();
        }

        private bool WriteLevel(Floor floor, XmlTextWriter writer, string strVersion)
        {
            try
            {
                writer.WriteStartElement("Level");

                writer.WriteStartAttribute("id");
                writer.WriteString(floor.ID);
                writer.WriteEndAttribute();

                //ym.0826. Level Height 프로퍼티추가
                WriteLevelProperty(floor, writer);

                string strFloorName = floor.Name.ToLower();
                int nIndex = strFloorName.IndexOf(" plan");

                if (nIndex >= 0)
                    strFloorName = floor.Name.Substring(0, nIndex);
                else
                    strFloorName = floor.Name;

                writer.WriteStartElement("Name");
                //ym.level 명칭에서 " plan"문자열 빼기
                //writer.WriteString(floor.Name);
                writer.WriteString(strFloorName);
                //writer.WriteString(floor.Name.Substring(0, floor.Name.IndexOf(" plan")));
                writer.WriteFullEndElement();

                writer.WriteStartElement("Elevation");
                writer.WriteString(GetDoubleString(floor.Elevation));
                writer.WriteFullEndElement();

                if (WriteGridCollection(floor.Walls, writer) == false)
                    return false;

                if (WriteElementCollection(floor, writer, strVersion) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteElementCollection(Floor floor, XmlTextWriter writer, string strVersion)
        {
            try
            {
                writer.WriteStartElement("ElementCollection");

                foreach (Wall wall in floor.Walls)
                {
                    if (WriteWall(wall, writer, strVersion) == false)
                        return false;
                }

                foreach (Space space in floor.Spaces)
                {
                    if (WriteSpace(space, writer, strVersion) == false)
                        return false;
                }

                if (strVersion == CommonString.XML_VERSION)
                {
                    foreach (AlertArea alertArea in floor.AlertAreas)
                    {
                        if (WriteAlertArea(alertArea, writer) == false)
                            return false;
                    }
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

                foreach (POI poi in floor.POIs)
                {
                    if (WritePOI(poi, writer) == false)
                        return false;
                }

                foreach (Wire wire in floor.Wires)
                {
                    if (WriteWire(wire, writer) == false)
                        return false;
                }

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteWire(Wire wire, XmlTextWriter writer)
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
                writer.WriteString(wire.POIType.XMLID);
                writer.WriteEndAttribute();

                writer.WriteStartElement("Lines");
                writer.WriteString(GetLineString(wire));
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private string GetLineString(Wire wire)
        {
            string strVertices = "";

            foreach (Vertex2D vertex in wire.Positions)
            {
                if (strVertices.Length == 0)
                    strVertices = string.Format("{0},{1}", vertex.x, vertex.y);
                else
                    strVertices += string.Format(",{0},{1}", vertex.x, vertex.y);
            }

            return strVertices;
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
                writer.WriteString(poi.PoiType.XMLID);
                writer.WriteEndAttribute();

                if (poi.Properties.Count > 0)
                    WritePOIProperty(poi, writer);

                writer.WriteStartElement("Name");
                writer.WriteString(poi.Name);
                writer.WriteFullEndElement();

                if (WriteVertex(poi.Position, "Point", writer) == false)
                    return false;

                if (WriteDouble(poi.Angle, "Angle", writer) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private void WritePOIProperty(POI poi, XmlTextWriter writer)
        {
            writer.WriteStartElement("POIProperties");

            //ym0729
            foreach (Property tmpprty in poi.Properties)
            {
                WriteProperty(tmpprty.Name, tmpprty.Value, null, writer);
            }

            writer.WriteFullEndElement();
        }

        private void WriteColumnProperty(Column column, XmlTextWriter writer)
        {
            writer.WriteStartElement("ColumnProperties");

            //ym0729
            foreach (Property tmpprty in column.Properties)
            {
                WriteProperty(tmpprty.Name, tmpprty.Value, null, writer);
            }

            writer.WriteFullEndElement();
        }
        private bool WriteColumn(Column column, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Column");

                writer.WriteStartAttribute("id");
                writer.WriteString(column.ID);
                writer.WriteEndAttribute();

                WriteColumnProperty(column, writer);

                if (column is RectColumn)
                {
                    RectColumn rect = (RectColumn)column;

                    writer.WriteStartElement("Rect");

                    if (WriteVertex(rect.TopLeft, "TL", writer) == false)
                        return false;
                    if (WriteVertex(rect.BottomLeft, "BL", writer) == false)
                        return false;
                    if (WriteVertex(rect.BottomRight, "BR", writer) == false)
                        return false;

                    writer.WriteFullEndElement();
                }
                else if (column is CircleColumn)
                {
                    CircleColumn circle = (CircleColumn)column;

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
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteVertex(UnE.Geometry.Vertex2D vertex, string strElementName, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement(strElementName);

                WritePos(vertex, writer);

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private void WritePos(Vertex2D vertex, XmlTextWriter writer)
        {
            writer.WriteStartElement("Pos");
            writer.WriteString(GetVertexString(vertex.x, vertex.y));
            writer.WriteFullEndElement();
        }

        private bool WriteDouble(double data, string strElementName, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement(strElementName);
                writer.WriteString(GetDoubleString(data));
                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteBoolean(bool flag, string strElementName, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement(strElementName);
                writer.WriteString(flag ? "true" : "false");
                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
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

                foreach (Topology.Node node in topology.Nodes)
                {
                    if (WriteTopologyNode(node, writer) == false)
                        return false;
                }

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
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

                WriteTopologyNodeProperties(node, writer);

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
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteTopologyNodeProperties(Topology.Node node, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("TopologyNodeProperties");

                WriteProperty("OwnerType", node.GetOwnerType(), null, writer);

                string strOwnerID = node.GetOwnerID();

                if (strOwnerID != null)
                    WriteProperty("OwnerId", strOwnerID, null, writer);

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
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

                WriteWindowProperty(window, writer);

                writer.WriteStartElement("Point");

                writer.WriteStartElement("Pos");
                writer.WriteString(GetVertexString(window.X, window.Y));
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();

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
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private void WriteDoorProperty(Door door, XmlTextWriter writer)
        {
            writer.WriteStartElement("DoorProperties");

           // WriteProperty("Thick", GetDoubleString(door.Thick), null, writer);
           //ym0729
            foreach (Property tmpprty in door.Properties)
            {
                WriteProperty(tmpprty.Name, tmpprty.Value, null, writer);
            }

            writer.WriteFullEndElement();
        }

        private void WriteWindowProperty(Window window, XmlTextWriter writer)
        {
            writer.WriteStartElement("WindowProperties");

            WriteProperty("Thick", GetDoubleString(window.Thick), null, writer);

            writer.WriteFullEndElement();
        }

        private void WriteProperty(string strPropertyName, string strValue, string strDescription, XmlTextWriter writer)
        {
            writer.WriteStartElement("Property");

            writer.WriteStartElement("Name");
            writer.WriteString(strPropertyName);
            writer.WriteFullEndElement();

            writer.WriteStartElement("Value");
            writer.WriteString(strValue);
            writer.WriteFullEndElement();

            if (strDescription != null)
            {
                writer.WriteStartElement("Description");
                writer.WriteString(strDescription);
                writer.WriteFullEndElement();
            }

            writer.WriteFullEndElement();
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

                WriteDoorProperty(door, writer);

                writer.WriteStartElement("Point");

                writer.WriteStartElement("Pos");
                writer.WriteString(GetVertexString(door.X, door.Y));
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();

                /*writer.WriteStartElement("Direction");
                writer.WriteString(string.Format("{0:F0}", door.Direction));
                writer.WriteFullEndElement();*/

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
                    writer.WriteStartElement("Hinge1");

                    writer.WriteStartElement("Pos");
                    writer.WriteString(GetVertexString(door.Hinge1.x, door.Hinge1.y));
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                }

                if (door.Hinge2 != null)
                {
                    writer.WriteStartElement("Hinge2");

                    writer.WriteStartElement("Pos");
                    writer.WriteString(GetVertexString(door.Hinge2.x, door.Hinge2.y));
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                }

                writer.WriteStartElement("DoorType");
                writer.WriteString(((int)door.Type).ToString());
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private void WriteAlertAreaProperty(AlertArea alertArea, XmlTextWriter writer)
        {
            writer.WriteStartElement("AlertAreaProperties");

            foreach (Property tmpprty in alertArea.Properties)
            {
                WriteProperty(tmpprty.Name, tmpprty.Value, null, writer);
            }

            writer.WriteFullEndElement();
        }

        private bool WriteAlertArea(AlertArea alertArea, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("AlertArea");

                writer.WriteStartAttribute("id");
                writer.WriteString(alertArea.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("name");
                writer.WriteString(alertArea.Name);
                writer.WriteEndAttribute();

                WriteAlertAreaProperty(alertArea, writer);

                if (WriteBoundary(alertArea.Boundary, writer) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        //ym.0826
        private void WriteSpaceProperty(Space space, XmlTextWriter writer)
        {
            writer.WriteStartElement("SpaceProperties");

            foreach (Property tmpprty in space.Properties)
            {
                WriteProperty(tmpprty.Name, tmpprty.Value, null, writer);
            }

            writer.WriteFullEndElement();
        }

        private bool WriteSpace(Space space, XmlTextWriter writer, string strVersion)
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

                //ym. space Property 쓰기 추가. 0826
                WriteSpaceProperty(space, writer);

                foreach (Wall wall in space.Walls)
                {
                    writer.WriteStartElement("LinkedWall");

                    writer.WriteStartAttribute("link");
                    writer.WriteString(wall.ID);
                    writer.WriteEndAttribute();

                    writer.WriteEndElement();
                }

                if (strVersion == CommonString.XML_VERSION)
                {
                    if (WriteBoundary(space.BoundaryPolygon, writer) == false)
                        return false;
                }
                    

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private void WriteWallProperty(Wall wall, XmlTextWriter writer)//ym
        {
            writer.WriteStartElement("WallProperties");

            // WriteProperty("Thick", GetDoubleString(wall.Thick), null, writer);
            //WriteProperty("재질", "콘크리트", null, writer);
            //WriteProperty("마감재", "페인트", null, writer);
            foreach(Property tmpprty in wall.Properties)
            {
                WriteProperty(tmpprty.Name, tmpprty.Value, null, writer);
            }
            writer.WriteFullEndElement();          

        }
        private bool WriteWall(Wall wall, XmlTextWriter writer, string strVersion)
        {
            try
            {
                writer.WriteStartElement("Wall");

                writer.WriteStartAttribute("id");
                writer.WriteString(wall.ID);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("grid");
                writer.WriteString(GetGridID(wall));
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("component");
                //writer.WriteString(wall.Material.ID);
                writer.WriteString(ChangeWallComponent(wall.Material.ID));
                writer.WriteEndAttribute();

                //ym0729,구조벽, 가벽, 난간벽은 속성추가
                if(wall.Material.ID == "component1" || wall.Material.ID == "component2" || wall.Material.ID == "component4")
                    WriteWallProperty(wall, writer);

                writer.WriteStartElement("Thickness");
                writer.WriteString(GetDoubleString(wall.Thick));
                writer.WriteFullEndElement();

                writer.WriteStartElement("Height");
                writer.WriteString(GetDoubleString(wall.Height));
                writer.WriteFullEndElement();

                if (strVersion == CommonString.XML_VERSION)
                {
                    if (wall.BoundaryPolygon != null && wall.BoundaryPolygon.GetVertexCount() >= 3)
                    {
                        if (WriteBoundary(wall.BoundaryPolygon, writer) == false)
                            return false;
                    }
                    else if (wall.Boundary != null)
                    {
                        List<PathItem> items = new List<PathItem>();
                        items.AddRange(wall.Boundary);

                        while (items.Count > 1)
                        {
                            items.RemoveAt(1);
                        }

                        if (WriteBoundary(items, writer) == false)
                            return false;
                    }
                }
                
                /*if (wall.Material != null)
                {
                    writer.WriteStartElement("Component");

                    writer.WriteStartAttribute("type");
                    writer.WriteString(wall.Material.TypeName);
                    writer.WriteEndAttribute();

                    writer.WriteString(wall.Material.MaterialName);
                    writer.WriteFullEndElement();
                }*/

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private string ChangeWallComponent(string strComponent)
        {
            string strRet = "WTP_01";

            if (strComponent == "component1")
                strRet = "WTP_01";
            else if (strComponent == "component2")
                strRet = "WTP_02";
            else if (strComponent == "component3")
                strRet = "WTP_03";
            else if (strComponent == "component4")
                strRet = "WTP_04";
            else if (strComponent == "component5")
                strRet = "WTP_05";

            return strRet;
        }

        private bool WriteBoundary(Polygon boundary, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Boundary");

                int nVertexCount = boundary.GetVertexCount();

                for (int i= 0;i < nVertexCount;i++)
                {
                    Vertex2D v1 = boundary.GetVertex(i);
                    Vertex2D v2 = i == nVertexCount - 1 ? boundary.GetVertex(0) : boundary.GetVertex(i + 1);

                    if (WriteLine(v1, v2, writer) == false)
                        return false;
                }

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteBoundary(List<PathItem> boundary, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Boundary");

                foreach (PathItem item in boundary)
                {
                    PathItem.DrawType type = item.GetDrawType();

                    if (type == PathItem.DrawType.Line)
                    {
                        if (WriteLine(item, writer) == false)
                            return false;
                    }
                    else if (type == PathItem.DrawType.Arc)
                    {
                        if (WriteArc((Arc2D)item.GetEArc(), writer) == false)
                            return false;
                    }
                    else if (type == PathItem.DrawType.Line)
                    {
                        if (WriteEArc(item.GetEArc(), writer) == false)
                            return false;
                    }
                    else
                        return false;
                }

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteEArc(EArc2D earc, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("EArc");

                if (WriteVertex(earc.GetTL(), "TL", writer) == false)
                    return false;

                if (WriteVertex(earc.GetBL(), "BL", writer) == false)
                    return false;

                if (WriteVertex(earc.GetBR(), "BR", writer) == false)
                    return false;

                if (WriteDouble(earc.GetBeginAngle(), "BeginAngle", writer) == false)
                    return false;

                if (WriteDouble(earc.GetAngle(), "Angle", writer) == false)
                    return false;

                if (WriteBoolean(earc.IsClockWise(), "ClockWise", writer) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteArc(Arc2D arc, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Arc");

                if (WriteVertex(arc.GetCenter(), "Center", writer) == false)
                    return false;

                if (WriteDouble(arc.GetRadius(), "Radius", writer) == false)
                    return false;

                if (WriteDouble(arc.GetBeginAngle(), "BeginAngle", writer) == false)
                    return false;

                if (WriteDouble(arc.GetAngle(), "Angle", writer) == false)
                    return false;

                if (WriteBoolean(arc.IsClockWise(), "ClockWise", writer) == false)
                    return false;

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteLine(Vertex2D vBegin, Vertex2D vEnd, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Line");
                WritePos(vBegin, writer);
                WritePos(vEnd, writer);
                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteLine(PathItem item, XmlTextWriter writer)
        {
            try
            {
                Vertex2D vBegin, vMiddle, vEnd;

                if (item.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                    return false;

                writer.WriteStartElement("Line");
                WritePos(vBegin, writer);
                WritePos(vEnd, writer);
                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
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
            catch (Exception e)
            {
                m_strError = e.Message;
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
                writer.WriteString(GetGridID(wall));
                writer.WriteEndAttribute();

                writer.WriteStartElement("Line");

                writer.WriteStartElement("Pos");
                writer.WriteString(GetVertexString(wall.Begin));
                writer.WriteFullEndElement();

                writer.WriteStartElement("Pos");
                writer.WriteString(GetVertexString(wall.End));
                writer.WriteFullEndElement();

                writer.WriteFullEndElement();   // Line End

                writer.WriteFullEndElement();   // Grid End
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private string GetGridID(Wall wall)
        {
            return "g" + wall.ID.Substring(1);
        }

        private string GetVertexString(double x, double y)
        {
            return GetDoubleString(x) + "," + GetDoubleString(y);
        }

        private string GetVertexString(UnE.Geometry.Vertex2D vertex)
        {
            return GetVertexString(vertex.x, vertex.y);
        }

        private string GetDoubleString(double data)
        {
            return string.Format("{0:" + m_strDoubleFormat + "}", data);
        }

        private bool WriteProjectInfo(Project project, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("ProjectInfo");

                writer.WriteStartAttribute("name");
                writer.WriteString(project.ProjectName);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("unit");
                writer.WriteString(project.Unit);
                writer.WriteEndAttribute();

                if (project.Unit == "mm")
                    m_strDoubleFormat = "F0";
                else if (project.Unit == "cm")
                    m_strDoubleFormat = "F1";
                else if (project.Unit == "meter")
                    m_strDoubleFormat = "F3";

                string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", project.Date.Year, project.Date.Month, project.Date.Day, project.Date.Hour, project.Date.Minute, project.Date.Second);
                writer.WriteStartAttribute("datetime");
                writer.WriteString(strTime);
                writer.WriteEndAttribute();

                if (project.Author.Trim().Length > 0)
                {
                    writer.WriteStartAttribute("author");
                    writer.WriteString(project.Author.Trim());
                    writer.WriteEndAttribute();
                }

                if (project.AnchorNode != null)
                {
                    if (WriteAnchorNode(project.AnchorNode, writer) == false)
                        return false;
                }

                writer.WriteFullEndElement();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool WriteAnchorNode(AnchorNode anchorNode, XmlTextWriter writer)
        {
            try
            {
                if (anchorNode == null)
                    return false;

                writer.WriteStartElement("AnchorNode");

                writer.WriteStartElement("Global");

                writer.WriteStartAttribute("unit");
                writer.WriteString(Project.UnitOfLengthString(anchorNode.GlobalUnitOfLength));
                writer.WriteEndAttribute();

                WritePos(anchorNode.GlobalPosition, writer);

                writer.WriteFullEndElement();   // Global

                writer.WriteStartElement("Local");

                WritePos(anchorNode.LocalPosition, writer);

                if (WriteDouble(anchorNode.Angle, "Angle", writer) == false)
                    return false;

                writer.WriteFullEndElement();   // Local

                writer.WriteFullEndElement();   // AnchorNode
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        public Project LoadXML(string strFilePath, out string strXMLVersion, out Dictionary<int, POIType> dicPOITypes)
        {
            strXMLVersion = "";
            Project project = null;

            dicPOITypes = new Dictionary<int, POIType>();
            Dictionary<string, Component> dicComponents = new Dictionary<string, Component>();

            if (LoadCommonFirst(strFilePath, dicPOITypes, dicComponents) == false)
                return null;

            try
            {
                m_strError = "";

                bool stop = false;

                XmlTextReader reader = new XmlTextReader(strFilePath);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "IndoorModelFile", true) == 0)
                            {
                                project = LoadIndoorModelFile(reader, out strXMLVersion, dicPOITypes, dicComponents);
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
                m_strError = e.Message;
                return null;
            }

            if (project != null)
            {
                foreach (Floor floor in project.Floors)
                {
                    foreach (Topology topology in floor.Topologies)
                    {
                        foreach (Topology.Node node in topology.Nodes)
                        {
                            foreach (Property property in node.Properties)
                            {
                                if (property.Name == "OwnerType")
                                    node.SetOwnerType(property.Value);
                                else if (property.Name == "OwnerId")
                                {
                                    if (node.Type == Topology.Node.NodeType.Door)
                                    {
                                        Door door = FindDoor(floor, property.Value);
                                        node.Owner = door;
                                    }
                                    else if (node.Type == Topology.Node.NodeType.Space)
                                    {
                                        Space space = FindSpace(floor, property.Value);
                                        node.Owner = space;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return project;
        }

        private Door FindDoor(Floor floor, string strDoorID)
        {
            foreach (Wall wall in floor.Walls)
            {
                foreach (Door door in wall.Doors)
                {
                    if (door.ID == strDoorID)
                        return door;
                }
            }

            return null;
        }

        private Space FindSpace(Floor floor, string strSpaceID)
        {
            foreach (Space space in floor.Spaces)
            {
                if (space.ID == strSpaceID)
                    return space;
            }

            return null;
        }

        private bool LoadCommonFirst(string strFilePath, Dictionary<int, POIType> dicPOITypes, Dictionary<string, Component> dicComponents)
        {
            try
            {
                m_strError = "";

                bool stop = false;

                XmlTextReader reader = new XmlTextReader(strFilePath);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "IndoorModelFile", true) == 0)
                            {
                                if (LoadCommonFirst(reader, dicPOITypes, dicComponents) == false)
                                    return false;

                                reader.Close();
                                return true;
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

                reader.Close();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
            }

            return false;
        }

        private bool LoadCommonFirst(XmlTextReader reader, Dictionary<int, POIType> dicPOITypes, Dictionary<string, Component> dicComponents)
        {
            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Common", true) == 0)
                            {
                                if (LoadCommon(reader, dicPOITypes, dicComponents) == false)
                                    return false;

                                reader.Close();
                                return true;
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

                reader.Close();
            }
            catch (Exception e)
            {
                m_strError = e.Message;
            }

            return false;
        }

        private Project LoadIndoorModelFile(XmlTextReader reader, out string strXMLVersion, Dictionary<int, POIType> dicPOITypes, Dictionary<string, Component> dicComponents)
        {
            strXMLVersion = "";
            Project project = null;

            try
            {
                bool stop = false;
                string strVersion = "";

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "version", true) == 0)
                    {
                        strVersion = reader.Value;
                    }
                }

                strXMLVersion = strVersion;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ProjectInfo", true) == 0)
                            {
                                project = LoadProject(reader);
                            }
                            else if (string.Compare(reader.Name, "Levels", true) == 0)
                            {
                                if (LoadLevels(reader, project, dicPOITypes, dicComponents) == false)
                                    return null;

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
                m_strError = e.Message;
                return null;
            }

            return project;
        }

        private bool LoadLevels(XmlTextReader reader, Project project, Dictionary<int, POIType> dicPOITypes, Dictionary<string, Component> dicComponents)
        {
            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Level", true) == 0)
                            {
                                Floor level = LoadLevel(reader, project, dicPOITypes, dicComponents);

                                if (level == null)
                                    return false;
                                else
                                    project.Floors.Add(level);
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
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private Floor LoadLevel(XmlTextReader reader, Project project, Dictionary<int, POIType> dicPOITypes, Dictionary<string, Component> dicComponents)
        {
            Floor level = null;

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
                    m_strError = GetLineCountString(reader) + ", Level Element에 id가 존재하지 않습니다.";
                    return level;
                }

                level = new Floor();
                level.ID = strID;

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
                                    m_strError = GetLineCountString(reader) + ", Elevation에 잘못된 값이 들어있습니다.";
                                    return null;
                                }

                                level.Elevation = fElevation;
                            }
                            else if (string.Compare(reader.Name, "GridCollection", true) == 0)
                            {
                                if (LoadGridCollection(reader, dicGrids) == false)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "ElementCollection", true) == 0)
                            {
                                if (LoadElementCollection(reader, level, dicGrids, project, dicPOITypes, dicComponents) == false)
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
                m_strError = e.Message;
                return null;
            }

            return level;
        }

        private bool LoadElementCollection(XmlTextReader reader, Floor level, Dictionary<string, Grid> dicGrids, Project project, Dictionary<int, POIType> dicPOITypes, Dictionary<string, Component> dicComponents)
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
                                Wall wall = LoadWall(reader, level, dicGrids, project, dicComponents);

                                if (wall == null)
                                    return false;
                                else
                                    level.Walls.Add(wall);
                            }
                            else if (string.Compare(reader.Name, "Space", true) == 0)
                            {
                                Space space = LoadSpace(reader, level);

                                if (space == null)
                                    return false;
                                else
                                    level.Spaces.Add(space);
                            }
                            else if (string.Compare(reader.Name, "Door", true) == 0)
                            {
                                if (LoadDoor(reader, level) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Window", true) == 0)
                            {
                                if (LoadWindow(reader, level) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Column", true) == 0)
                            {
                                if (LoadColumn(reader, level) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Topology", true) == 0)
                            {
                                if (LoadTopology(reader, level) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "POI", true) == 0)
                            {
                                if (LoadPOI(reader, level, dicPOITypes) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "POIWire", true) == 0)
                            {
                                if (LoadPOIWire(reader, level, dicPOITypes) == false)
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
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadPOIWire(XmlTextReader reader, Floor level, Dictionary<int, POIType> dicPOITypes)
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
                    m_strError = GetLineCountString(reader) + ", POIWire Element에 id가 존재하지 않습니다.";
                    return false;
                }

                if (strBeginPOIID == null)
                {
                    m_strError = GetLineCountString(reader) + ", POIWire Element에 beginPOI가 존재하지 않습니다.";
                    return false;
                }

                if (strEndPOIID == null)
                {
                    m_strError = GetLineCountString(reader) + ", POIWire Element에 endPOI가 존재하지 않습니다.";
                    return false;
                }

                if (strTypeID == null)
                {
                    m_strError = GetLineCountString(reader) + ", POIWire Element에 type이 존재하지 않습니다.";
                    return false;
                }

                POI beginPOI = FindPOI(strBeginPOIID, level);

                if (beginPOI == null)
                {
                    m_strError = GetLineCountString(reader) + ", " + strBeginPOIID + "는 존재하지 않는 POI ID입니다.";
                    return false;
                }

                POI endPOI = FindPOI(strEndPOIID, level);

                if (endPOI == null)
                {
                    m_strError = GetLineCountString(reader) + ", " + strEndPOIID + "는 존재하지 않는 POI ID입니다.";
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
                    m_strError = GetLineCountString(reader) + ", " + strTypeID + "는 존재하지 않는 POIType ID입니다.";
                    return false;
                }

                //if (dicPOITypes.TryGetValue(strTypeID.GetHashCode(), out poiType) == false)
                //{
                //    m_strErrorMessage = GetLineCountString(reader) + ", " + strTypeID + "는 존재하지 않는 POIType ID입니다.";
                //    return false;
                //}

                Wire wire = new Wire();

                wire.ID = strID;
                wire.POIType = poiType;
                wire.BeginPOI = beginPOI;
                wire.EndPOI = endPOI;

                level.Wires.Add(wire);

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
                                    m_strError = GetLineCountString(reader) + ", Lines에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                string[] lines = strLines.Split(',');
                                for (int i = 0; i < lines.Length; i += 2)
                                {
                                    double x = Convert.ToDouble(lines[i]);
                                    double y = Convert.ToDouble(lines[i + 1]);
                                    wire.Positions.Add(new Vertex2D(x, y));
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

                if (strLines == null)
                {
                    m_strError = GetLineCountString(reader) + ", Lines가 존재하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadPOI(XmlTextReader reader, Floor level, Dictionary<int, POIType> dicPOITypes)
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
                    m_strError = GetLineCountString(reader) + ", POI Element에 id가 존재하지 않습니다.";
                    return false;
                }

                if (strTypeID == null)
                {
                    m_strError = GetLineCountString(reader) + ", POI Element에 type이 존재하지 않습니다.";
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
                    m_strError = GetLineCountString(reader) + ", " + strTypeID + "는 존재하지 않는 POIType ID입니다.";
                    return false;
                }
                
                POI poi = new POI();

                poi.ID = strID;
                poi.PoiType = poiType;

                level.POIs.Add(poi);

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
                                    m_strError = GetLineCountString(reader) + ", Name에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                poi.Name = strName;
                                readName = true;
                            }
                            else if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                vPos = LoadVertex(reader);

                                if (vPos == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", Point에 잘못된 값이 들어있습니다.";
                                    return false;
                                }
                                else
                                    poi.Position = vPos;
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                double dAngle;

                                if (ReadDouble(reader, reader.Name, out dAngle) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Angle에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                poi.Angle = dAngle;
                                readAngle = true;
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                double dHeight;

                                if (ReadDouble(reader, reader.Name, out dHeight) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Height에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                poi.Height = dHeight;
                                poi.UseHeight = true;
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
                    m_strError = GetLineCountString(reader) + ", Point가 존재하지 않습니다.";
                    return false;
                }

                if (readName == false)
                {
                    m_strError = GetLineCountString(reader) + ", Name이 존재하지 않습니다.";
                    return false;
                }

                if (readAngle == false)
                {
                    m_strError = GetLineCountString(reader) + ", Angle이 존재하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadTopology(XmlTextReader reader, Floor level)
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
                    m_strError = GetLineCountString(reader) + ", Topology Element에 id가 존재하지 않습니다.";
                    return false;
                }

                Topology topology = new Topology();

                topology.ID = strID;
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
                                if (LoadTopologyNode(reader, topology, dicTopologyNodes, dicNodeLinks) == false)
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

                Topology.Node link;

                foreach (KeyValuePair<Topology.Node, List<string>> pair in dicNodeLinks)
                {
                    foreach (string strNodeID in pair.Value)
                    {
                        if (dicTopologyNodes.TryGetValue(strNodeID, out link) == false)
                        {
                            m_strError = GetLineCountString(reader) + ", " + strNodeID + "는 존재하지 않는 Node ID입니다.";
                            return false;
                        }

                        pair.Key.LinkedNodes.Add(link);
                    }
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadTopologyNode(XmlTextReader reader, Topology topology, Dictionary<string, Topology.Node> dicTopologyNodes, Dictionary<Topology.Node, List<string>> dicNodeLinks)
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
                    m_strError = GetLineCountString(reader) + ", Node Element에 id가 존재하지 않습니다.";
                    return false;
                }

                Topology.Node node = new Topology.Node();

                node.ID = strID;
                topology.Nodes.Add(node);

                dicTopologyNodes[strID] = node;
                List<string> links = new List<string>();
                dicNodeLinks[node] = links;

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
                                if (LoadTopologyNodeLink(reader, links) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                vPos = LoadVertex(reader);

                                if (vPos == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", Point에 잘못된 데이터가 들어 있습니다.";
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
                    m_strError = GetLineCountString(reader) + ", Node Element에 Point가 존재하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadTopologyNodeLink(XmlTextReader reader, List<string> nodeIDs)
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
                    m_strError = GetLineCountString(reader) + ", Target Element에 id가 존재하지 않습니다.";
                    return false;
                }

                nodeIDs.Add(strID);
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadColumn(XmlTextReader reader, Floor level)
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
                    m_strError = GetLineCountString(reader) + ", Column Element에 id가 존재하지 않습니다.";
                    return false;
                }

                List<Property> properties = new List<Property>();
                Column column = null;
                RectColumn rect = null;
                CircleColumn circle = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ColumnProperties", true) == 0)
                            {
                                if (ReadProperties(reader, properties) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Rect", true) == 0)
                            {
                                rect = ReadRectColumn(reader);
                                
                                if (rect == null)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Circle", true) == 0)
                            {
                                circle = ReadCircleColumn(reader);
                                
                                if (circle == null)
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

                if (rect != null)
                    column = rect;
                else if (circle != null)
                    column = circle;
                else
                {
                    m_strError = GetLineCountString(reader) + ", Column Element에 Rect 또는 Circle 정보가 없습니다.";
                    return false;
                }

                column.ID = strColumnID;
                column.Properties.AddRange(properties);

                level.Columns.Add(column);
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private CircleColumn ReadCircleColumn(XmlTextReader reader)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return null;

                CircleColumn column = new CircleColumn();

                bool readCenter = false, readRadius = false;
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Center", true) == 0)
                            {
                                column.Center = LoadVertex(reader);

                                if (column.Center == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", Center에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                                else
                                {
                                    readCenter = true;
                                }
                            }
                            else if (string.Compare(reader.Name, "Radius", true) == 0)
                            {
                                double dRadius;

                                if (ReadDouble(reader, reader.Name, out dRadius) == false)
                                    return null;
                                else
                                {
                                    column.Radius = dRadius;
                                    readRadius = true;
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
                    m_strError = GetLineCountString(reader) + ", Column/Circle Element에 Center 정보가 없습니다.";
                    return null;
                }

                if (readRadius == false)
                {
                    m_strError = GetLineCountString(reader) + ", Column Element에 Radius가 없습니다.";
                    return null;
                }

                return column;
            }
            catch (Exception e)
            {
                m_strError = e.Message;
            }

            return null;
        }

        private RectColumn ReadRectColumn(XmlTextReader reader)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return null;

                RectColumn column = new RectColumn();
                
                bool readTL = false, readBL = false, readBR = false;
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TL", true) == 0)
                            {
                                column.TopLeft = LoadVertex(reader);
                                
                                if (column.TopLeft == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", TL에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                                else
                                {
                                    readTL = true;
                                }
                            }
                            else if (string.Compare(reader.Name, "BL", true) == 0)
                            {
                                column.BottomLeft = LoadVertex(reader);

                                if (column.TopLeft == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", BL에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                                else
                                {
                                    readBL = true;
                                }
                            }
                            else if (string.Compare(reader.Name, "BR", true) == 0)
                            {
                                column.BottomRight = LoadVertex(reader);

                                if (column.BottomRight == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", BR에 잘못된 값이 들어 있습니다.";
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
                    m_strError = GetLineCountString(reader) + ", Column/Rect Element에 TL 정보가 없습니다.";
                    return null;
                }

                if (readBL == false)
                {
                    m_strError = GetLineCountString(reader) + ", Column/Rect Element에 BL 정보가 없습니다.";
                    return null;
                }

                if (readBR == false)
                {
                    m_strError = GetLineCountString(reader) + ", Column/Rect Element에 BR 정보가 없습니다.";
                    return null;
                }

                return column;
            }
            catch (Exception e)
            {
                m_strError = e.Message;
            }

            return null;
        }

        private bool LoadWindow(XmlTextReader reader, Floor level)
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
                    m_strError = GetLineCountString(reader) + ", Window Element에 id가 존재하지 않습니다.";
                    return false;
                }

                if (strWallID == null)
                {
                    m_strError = GetLineCountString(reader) + ", Window Element에 attachedWall이 존재하지 않습니다.";
                    return false;
                }

                Wall wall = FindWall(strWallID, level);

                if (wall == null)
                {
                    m_strError = GetLineCountString(reader) + ", " + strWallID + "는 존재하지 않는 Wall ID입니다.";
                    return false;
                }

                Window window = new Window();

                window.ID = strID;
                wall.Windows.Add(window);

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
                            }
                            else if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                vPos = LoadVertex(reader);

                                if (vPos == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", Point에 잘못된 값이 들어있습니다.";
                                    return false;
                                }
                                else
                                {
                                    window.X = vPos.x;
                                    window.Y = vPos.y;
                                }
                            }
                            else if (string.Compare(reader.Name, "Width", true) == 0)
                            {
                                double dWidth;

                                if (ReadDouble(reader, reader.Name, out dWidth) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Width에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                window.Width = (float)dWidth;
                                readWidth = true;
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                double dHeight;

                                if (ReadDouble(reader, reader.Name, out dHeight) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Height에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                window.Height = (float)dHeight;
                                readHeight = true;
                            }
                            else if (string.Compare(reader.Name, "Elevation", true) == 0)
                            {
                                double dElevation;

                                if (ReadDouble(reader, reader.Name, out dElevation) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Elevation에 잘못된 값이 들어있습니다.";
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
                    m_strError = GetLineCountString(reader) + ", Point가 존재하지 않습니다.";
                    return false;
                }

                if (readWidth == false)
                {
                    m_strError = GetLineCountString(reader) + ", Width가 존재하지 않습니다.";
                    return false;
                }

                if (readHeight == false)
                {
                    m_strError = GetLineCountString(reader) + ", Height가 존재하지 않습니다.";
                    return false;
                }

                if (readElevation == false)
                {
                    m_strError = GetLineCountString(reader) + ", Elevation이 존재하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadDoor(XmlTextReader reader, Floor level)
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
                    m_strError = GetLineCountString(reader) + ", Door Element에 id가 존재하지 않습니다.";
                    return false;
                }

                if (strWallID == null)
                {
                    m_strError = GetLineCountString(reader) + ", Door Element에 attachedWall이 존재하지 않습니다.";
                    return false;
                }

                Wall wall = FindWall(strWallID, level);

                if (wall == null)
                {
                    m_strError = GetLineCountString(reader) + ", " + strWallID + "는 존재하지 않는 Wall ID입니다.";
                    return false;
                }

                Door door = new Door();

                door.ID = strID;
                wall.Doors.Add(door);

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
                                vPos = LoadVertex(reader);

                                if (vPos == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", Point에 잘못된 값이 들어있습니다.";
                                    return false;
                                }
                                else
                                {
                                    door.X = vPos.x;
                                    door.Y = vPos.y;
                                }
                            }
                            else if (string.Compare(reader.Name, "Width", true) == 0)
                            {
                                double dWidth;

                                if (ReadDouble(reader, reader.Name, out dWidth) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Width에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                door.Width = (float)dWidth;
                                readWidth = true;
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                double dHeight;

                                if (ReadDouble(reader, reader.Name, out dHeight) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Height에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                door.Height = (float)dHeight;
                                readHeight = true;
                            }
                            else if (string.Compare(reader.Name, "Elevation", true) == 0)
                            {
                                double dElevation;

                                if (ReadDouble(reader, reader.Name, out dElevation) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Elevation에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                door.Elevation = (float)dElevation;
                                readElevation = true;
                            }
                            else if (string.Compare(reader.Name, "DoorType", true) == 0)
                            {
                                int doorType;

                                if (ReadInt(reader, reader.Name, out doorType) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", DoorType에 잘못된 값이 들어있습니다.";
                                    return false;
                                }

                                door.Type = (Door.DoorType)doorType;
                                readDoorType = true;
                            }
                            else if (string.Compare(reader.Name, "Hinge1", true) == 0)
                            {
                                Vertex2D vHinge = LoadVertex(reader);

                                if (vHinge == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", Hinge1에 잘못된 값이 들어있습니다.";
                                    return false;
                                }
                                else
                                    door.Hinge1 = vHinge;
                            }
                            else if (string.Compare(reader.Name, "Hinge2", true) == 0)
                            {
                                Vertex2D vHinge = LoadVertex(reader);

                                if (vHinge == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", Hinge2에 잘못된 값이 들어있습니다.";
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
                    m_strError = GetLineCountString(reader) + ", Point가 존재하지 않습니다.";
                    return false;
                }

                if (readWidth == false)
                {
                    m_strError = GetLineCountString(reader) + ", Width가 존재하지 않습니다.";
                    return false;
                }

                if (readHeight == false)
                {
                    m_strError = GetLineCountString(reader) + ", Height가 존재하지 않습니다.";
                    return false;
                }

                if (readElevation == false)
                {
                    m_strError = GetLineCountString(reader) + ", Elevation이 존재하지 않습니다.";
                    return false;
                }

                if (readDoorType == false)
                {
                    m_strError = GetLineCountString(reader) + ", DoorType이 존재하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private Space LoadSpace(XmlTextReader reader, Floor level)
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
                    m_strError = GetLineCountString(reader) + ", Space Element에 id가 존재하지 않습니다.";
                    return null;
                }

                if (strName == null)
                {
                    m_strError = GetLineCountString(reader) + ", Space Element에 name이 존재하지 않습니다.";
                    return null;
                }

                space = new Space();

                space.ID = strID;
                space.Name = strName;

                bool readBoundary = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "SpaceProperties", true) == 0)
                            {
                                if (ReadProperties(reader, space.Properties) == false)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "LinkedWall", true) == 0)
                            {
                                Wall wall = LoadLinkedWall(reader, level);

                                if (wall == null)
                                    return null;
                                else
                                    space.Walls.Add(wall);
                            }
                            else if (string.Compare(reader.Name, "Boundary", true) == 0)
                            {
                                if (LoadBoundary(reader, space.Boundary) == false)
                                    return null;
                                else
                                    readBoundary = true;
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

                if (readBoundary == false)
                {
                    // 1.5 이전의 버전은 Boundary가 없을수 있다.
                    //m_strError = GetLineCountString(reader) + ", Boundary가 존재하지 않습니다.";
                    //return null;
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return null;
            }

            return space;
        }

        private Wall LoadLinkedWall(XmlTextReader reader, Floor level)
        {
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
                    m_strError = GetLineCountString(reader) + ", LinkedWall Element에 link가 존재하지 않습니다.";
                    return null;
                }

                Wall wall = FindWall(strID, level);

                if (wall != null)
                    return wall;
                
                m_strError = GetLineCountString(reader) + ", " + strID + "는 존재하지 않는 Wall ID입니다.";
                return null;
            }
            catch (Exception e)
            {
                m_strError = e.Message;
            }

            return null;
        }

        private POI FindPOI(string strPOIID, Floor level)
        {
            foreach (POI poi in level.POIs)
            {
                if (poi.ID == strPOIID)
                    return poi;
            }

            return null;
        }

        private Wall FindWall(string strWallID, Floor level)
        {
            foreach (Wall wall in level.Walls)
            {
                if (wall.ID == strWallID)
                    return wall;
            }

            return null;
        }

        private Wall LoadWall(XmlTextReader reader, Floor level, Dictionary<string, Grid> dicGrids, Project project, Dictionary<string, Component> dicComponents)
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
                    m_strError = GetLineCountString(reader) + ", Wall Element에 id가 존재하지 않습니다.";
                    return wall;
                }

                if (strComponentID == null)
                {
                    m_strError = GetLineCountString(reader) + ", Wall Element에 component가 존재하지 않습니다.";
                    return wall;
                }

                if (strGridID == null)
                {
                    m_strError = GetLineCountString(reader) + ", Wall Element에 grid가 존재하지 않습니다.";
                    return wall;
                }

                Grid grid;

                if (dicGrids.TryGetValue(strGridID, out grid) == false)
                {
                    m_strError = GetLineCountString(reader) + ", " + strGridID + "는 존재하지 않는 Grid ID입니다.";
                    return wall;
                }

                Component component;

                if (dicComponents.TryGetValue(strComponentID, out component) == false)
                {
                    m_strError = GetLineCountString(reader) + ", " + strComponentID + "는 존재하지 않는 Component ID입니다.";
                    return null;
                }

                wall = new Wall();

                if (SetWallGrid(wall, grid) == false)
                    return null;

                wall.ID = strID;
                wall.Material = component;

                bool readThick = false, readHeight = false, readBoundary = false;

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

                                if (ReadDouble(reader, reader.Name, out dThick) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Thickness에 잘못된 값이 들어있습니다.";
                                    return null;
                                }

                                wall.Thick = dThick;
                                readThick = true;
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                double dHeight;

                                if (ReadDouble(reader, reader.Name, out dHeight) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Height에 잘못된 값이 들어있습니다.";
                                    return null;
                                }

                                wall.Height = dHeight;
                                readHeight = true;
                            }
                            else if (string.Compare(reader.Name, "Boundary", true) == 0)
                            {
                                if (LoadBoundary(reader, wall.Boundary) == false)
                                    return null;
                                else
                                    readBoundary = true;
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
                    m_strError = GetLineCountString(reader) + ", Thickness가 존재하지 않습니다.";
                    return null;
                }

                if (readHeight == false)
                {
                    m_strError = GetLineCountString(reader) + ", Height가 존재하지 않습니다.";
                    return null;
                }

                if (readBoundary == false)
                {
                    // 1.5 이전의 버전은 Boundary가 없을수 있다.
                    //m_strError = GetLineCountString(reader) + ", Boundary가 존재하지 않습니다.";
                    //return null;
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return null;
            }

            return wall;
        }

        private bool LoadBoundary(XmlTextReader reader, List<PathItem> boundary)
        {
            try
            {
                bool stop = false;
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Line", true) == 0)
                            {
                                if (LoadLine(reader, boundary) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Arc", true) == 0)
                            {
                                if (LoadArc(reader, boundary) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "EArc", true) == 0)
                            {
                                if (LoadArc(reader, boundary) == false)
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
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadEArc(XmlTextReader reader, List<PathItem> boundary)
        {
            try
            {
                bool stop = false, isClockwise = true;
                Vertex2D vTL = null, vBL = null, vBR = null;
                double dBeginAngle = 0.0, dArcAngle = 0.0;
                bool readBeginAngle = false, readArcAngle = false, readClockwise = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TL", true) == 0)
                            {
                                vTL = LoadVertex(reader);

                                if (vTL == null)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "BL", true) == 0)
                            {
                                vBL = LoadVertex(reader);

                                if (vBL == null)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "BR", true) == 0)
                            {
                                vBR = LoadVertex(reader);

                                if (vBR == null)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "BeginAngle", true) == 0)
                            {
                                if (ReadDouble(reader, reader.Name, out dBeginAngle) == false)
                                    return false;
                                else
                                    readBeginAngle = true;
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                if (ReadDouble(reader, reader.Name, out dArcAngle) == false)
                                    return false;
                                else
                                    readArcAngle = true;
                            }
                            else if (string.Compare(reader.Name, "ClockWise", true) == 0)
                            {
                                if (ReadBoolean(reader, reader.Name, out isClockwise) == false)
                                    return false;
                                else
                                    readClockwise = true;
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
                    m_strError = GetLineCountString(reader) + ", Arc Element에 TL이 존재하지 않습니다.";
                    return false;
                }

                if (vBL == null)
                {
                    m_strError = GetLineCountString(reader) + ", Arc Element에 BL이 존재하지 않습니다.";
                    return false;
                }
                if (vBR == null)
                {
                    m_strError = GetLineCountString(reader) + ", Arc Element에 BR이 존재하지 않습니다.";
                    return false;
                }

                if (readBeginAngle == false)
                {
                    m_strError = GetLineCountString(reader) + ", Arc Element에 BeginAngle이 존재하지 않습니다.";
                    return false;
                }

                if (readArcAngle == false)
                {
                    m_strError = GetLineCountString(reader) + ", Arc Element에 Angle이 존재하지 않습니다.";
                    return false;
                }

                if (readClockwise == false)
                {
                    m_strError = GetLineCountString(reader) + ", Arc Element에 ClockWise가 존재하지 않습니다.";
                    return false;
                }

                EArc2D earc = new EArc2D(vTL, vBL, vBR, dBeginAngle, dArcAngle, isClockwise);

                PathItem item = new PathItem();
                item.SetEArc(earc);
                boundary.Add(item);
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadArc(XmlTextReader reader, List<PathItem> boundary)
        {
            try
            {
                bool stop = false, isClockwise = true;
                Vertex2D vCenter = null;
                double dRadius = 0.0, dBeginAngle = 0.0, dArcAngle = 0.0;
                bool readRadius = false, readBeginAngle = false, readArcAngle = false, readClockwise = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Center", true) == 0)
                            {
                                vCenter = LoadVertex(reader);

                                if (vCenter == null)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Radius", true) == 0)
                            {
                                if (ReadDouble(reader, reader.Name, out dRadius) == false)
                                    return false;
                                else
                                    readRadius = true;
                            }
                            else if (string.Compare(reader.Name, "BeginAngle", true) == 0)
                            {
                                if (ReadDouble(reader, reader.Name, out dBeginAngle) == false)
                                    return false;
                                else
                                    readBeginAngle = true;
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                if (ReadDouble(reader, reader.Name, out dArcAngle) == false)
                                    return false;
                                else
                                    readArcAngle = true;
                            }
                            else if (string.Compare(reader.Name, "ClockWise", true) == 0)
                            {
                                if (ReadBoolean(reader, reader.Name, out isClockwise) == false)
                                    return false;
                                else
                                    readClockwise = true;
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
                    m_strError = GetLineCountString(reader) + ", Arc Element에 Center가 존재하지 않습니다.";
                    return false;
                }

                if (readRadius == false)
                {
                    m_strError = GetLineCountString(reader) + ", Arc Element에 Radius가 존재하지 않습니다.";
                    return false;
                }

                if (readBeginAngle == false)
                {
                    m_strError = GetLineCountString(reader) + ", Arc Element에 BeginAngle이 존재하지 않습니다.";
                    return false;
                }

                if (readArcAngle == false)
                {
                    m_strError = GetLineCountString(reader) + ", Arc Element에 Angle이 존재하지 않습니다.";
                    return false;
                }

                if (readClockwise == false)
                {
                    m_strError = GetLineCountString(reader) + ", Arc Element에 ClockWise가 존재하지 않습니다.";
                    return false;
                }

                Arc2D arc = new Arc2D(vCenter, dRadius, dBeginAngle, dArcAngle, isClockwise);

                PathItem item = new PathItem();
                item.SetArc(arc);
                boundary.Add(item);
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private Vertex2D LoadVertex(XmlTextReader reader)
        {
            try
            {
                bool stop = false;
                Vertex2D vPos = null;
                string strElementName = reader.Name;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Pos", true) == 0)
                            {
                                vPos = ReadPos(reader);

                                if (vPos == null)
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

                if (vPos == null)
                {
                    m_strError = string.Format("{0}, {1} Element에 Pos가 존재하지 않습니다.", GetLineCountString(reader), strElementName);
                    return null;
                }

                return vPos;
            }
            catch (Exception e)
            {
                m_strError = e.Message;
            }

            return null;
        }

        private bool LoadLine(XmlTextReader reader, List<PathItem> boundary)
        {
            try
            {
                bool stop = false;
                Vertex2D vBegin = null, vEnd = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Pos", true) == 0)
                            {
                                Vertex2D vPos = ReadPos(reader);

                                if (vPos == null)
                                    return false;

                                if (vBegin == null)
                                    vBegin = vPos;
                                else if (vEnd != null)
                                {
                                    m_strError = GetLineCountString(reader) + ", Line Element에는 좌표가 두 개만 존재할 수 있습니다.";
                                }
                                else
                                {
                                    vEnd = vPos;

                                    PathItem item = new PathItem();
                                    item.SetLine(new Line2D(vBegin, vEnd));
                                    boundary.Add(item);
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

                if (vBegin == null || vEnd == null)
                {
                    m_strError = GetLineCountString(reader) + ", Line Element는 두 개의 좌표가 존재해야 합니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool SetWallGrid(Wall wall, Grid grid)
        {
            if (grid.Line != null)
            {
                Vertex2D vBegin = grid.Line.GetVertex(true);
                Vertex2D vEnd = grid.Line.GetVertex(false);
                wall.Begin = new Vertex2D(vBegin.x, vBegin.y);
                wall.End = new Vertex2D(vEnd.x, vEnd.y);
            }
            else
                return false;

            return true;
        }

        private bool LoadGridCollection(XmlTextReader reader, Dictionary<string, Grid> dicGrids)
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
                                Grid grid = LoadGrid(reader);

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
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private Grid LoadGrid(XmlTextReader reader)
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
                    m_strError = GetLineCountString(reader) + ", Grid Element에 id가 존재하지 않습니다.";
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
                                grid.Line = LoadGridLine(reader);

                                if (grid.Line == null)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "Arc", true) == 0)
                            {
                                grid.Arc = LoadGridArc(reader);

                                if (grid.Arc == null)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "EArc", true) == 0)
                            {
                                grid.EArc = LoadGridEArc(reader);

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
                m_strError = e.Message;
                return null;
            }

            if (grid.Line == null && grid.Arc == null && grid.EArc == null)
            {
                m_strError = GetLineCountString(reader) + ", Grid에 Line, Arc, EArc 가운데 적어도 하나는 존재해야 합니다.";
                return null;
            }

            return grid;
        }

        private Line2D LoadGridLine(XmlTextReader reader)
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
                                Vertex2D vertex = ReadPos(reader);

                                if (vertex == null)
                                {
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
                m_strError = e.Message;
                return null;
            }

            return line;
        }

        private Arc2D LoadGridArc(XmlTextReader reader)
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
                                vCenter = LoadVertex(reader);

                                if (vCenter == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", Center에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "Radius", true) == 0)
                            {
                                readRadius = ReadDouble(reader, reader.Name, out dRadius);

                                if (readRadius == false)
                                {
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "BeginAngle", true) == 0)
                            {
                                readBeginAngle = ReadDouble(reader, reader.Name, out beginAngle);

                                if (readBeginAngle == false)
                                {
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                readAngle = ReadDouble(reader, reader.Name, out angle);

                                if (readAngle == false)
                                {
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "ClockWise", true) == 0)
                            {
                                readClockwise = ReadBoolean(reader, reader.Name, out isClockwise);

                                if (readClockwise == false)
                                {
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
                    m_strError = GetLineCountString(reader) + ", Center가 존재하지 않습니다.";
                    return null;
                }

                if (readRadius == false)
                {
                    m_strError = GetLineCountString(reader) + ", Radius가 존재하지 않습니다.";
                    return null;
                }

                if (readBeginAngle == false)
                {
                    m_strError = GetLineCountString(reader) + ", BeginAngle이 존재하지 않습니다.";
                    return null;
                }

                if (readAngle == false)
                {
                    m_strError = GetLineCountString(reader) + ", Angle이 존재하지 않습니다.";
                    return null;
                }

                if (readClockwise == false)
                {
                    m_strError = GetLineCountString(reader) + ", ClockWise가 존재하지 않습니다.";
                    return null;
                }

                arc = new Arc2D(vCenter, dRadius, beginAngle, angle, isClockwise);
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return null;
            }

            return arc;
        }

        private EArc2D LoadGridEArc(XmlTextReader reader)
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
                                vTL = LoadVertex(reader);

                                if (vTL == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", TL에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "BL", true) == 0)
                            {
                                vBL = LoadVertex(reader);

                                if (vBL == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", BL에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "BR", true) == 0)
                            {
                                vBR = LoadVertex(reader);

                                if (vBR == null)
                                {
                                    m_strError = GetLineCountString(reader) + ", BR에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "BeginAngle", true) == 0)
                            {
                                readBeginAngle = ReadDouble(reader, reader.Name, out beginAngle);

                                if (readBeginAngle == false)
                                {
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                readAngle = ReadDouble(reader, reader.Name, out angle);

                                if (readAngle == false)
                                {
                                    return null;
                                }
                            }
                            else if (string.Compare(reader.Name, "ClockWise", true) == 0)
                            {
                                readClockwise = ReadBoolean(reader, reader.Name, out isClockwise);

                                if (readClockwise == false)
                                {
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
                    m_strError = GetLineCountString(reader) + ", TL이 존재하지 않습니다.";
                    return null;
                }

                if (vBL == null)
                {
                    m_strError = GetLineCountString(reader) + ", BL이 존재하지 않습니다.";
                    return null;
                }

                if (vBR == null)
                {
                    m_strError = GetLineCountString(reader) + ", BR이 존재하지 않습니다.";
                    return null;
                }

                if (readBeginAngle == false)
                {
                    m_strError = GetLineCountString(reader) + ", BeginAngle이 존재하지 않습니다.";
                    return null;
                }

                if (readAngle == false)
                {
                    m_strError = GetLineCountString(reader) + ", Angle이 존재하지 않습니다.";
                    return null;
                }

                if (readClockwise == false)
                {
                    m_strError = GetLineCountString(reader) + ", ClockWise가 존재하지 않습니다.";
                    return null;
                }

                earc = new EArc2D(vTL, vBL, vBR, beginAngle, angle, isClockwise);
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                return null;
            }

            return earc;
        }

        private Project LoadProject(XmlTextReader reader)
        {
            Project project = null;

            try
            {
                bool emptyElement = reader.IsEmptyElement;

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
                    m_strError = GetLineCountString(reader) + ", ProjectInfo Element에 name 속성이 존재하지 않습니다.";
                    return project;
                }

                if (strUnit == null)
                {
                    m_strError = GetLineCountString(reader) + ", ProjectInfo Element에 unit 속성이 존재하지 않습니다.";
                    return project;
                }

                if (strDate == null)
                {
                    m_strError = GetLineCountString(reader) + ", ProjectInfo Element에 datetime 속성이 존재하지 않습니다.";
                    return project;
                }

                project = new Project();

                project.ProjectName = strName;
                project.Unit = strUnit;

                try
                {
                    project.Date = Convert.ToDateTime(strDate);
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
                                AnchorNode anchorNode = LoadAnchorNode(reader);

                                if (anchorNode == null)
                                    return null;
                                else
                                    project.AnchorNode = anchorNode;
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
                m_strError = e.Message;
                reader.Close();
                return null;
            }

            return project;
        }

        private bool LoadGlobal(XmlTextReader reader, AnchorNode anchorNode)
        {
            try
            {
                bool emptyElement = reader.IsEmptyElement;

                bool stop = false;
                string strUnit = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "unit", true) == 0)
                    {
                        strUnit = reader.Value;
                    }
                }

                if (strUnit == null)
                {
                    m_strError = GetLineCountString(reader) + ", AnchorNode Element에 unit 속성이 존재하지 않습니다.";
                    return false;
                }
                else
                    anchorNode.GlobalUnitOfLength = Project.StringToUnit(strUnit);

                Vertex2D vPos = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Pos", true) == 0)
                            {
                                vPos = ReadPos(reader);

                                if (vPos == null)
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

                if (vPos == null)
                {
                    m_strError = GetLineCountString(reader) + ", Global Element에 pos가 존재하지 않습니다.";
                    return false;
                }
                else
                    anchorNode.GlobalPosition = vPos;
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                reader.Close();
                return false;
            }

            return true;
        }

        private bool LoadLocal(XmlTextReader reader, AnchorNode anchorNode)
        {
            try
            {
                bool emptyElement = reader.IsEmptyElement;

                bool stop = false;

                Vertex2D vPos = null;
                double dAngle = 0.0;
                bool readAngle = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Pos", true) == 0)
                            {
                                vPos = ReadPos(reader);

                                if (vPos == null)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                if (ReadDouble(reader, reader.Name, out dAngle) == false)
                                    return false;
                                else
                                    readAngle = true;
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
                    m_strError = GetLineCountString(reader) + ", Local Element에 pos가 존재하지 않습니다.";
                    return false;
                }
                else
                    anchorNode.LocalPosition = vPos;

                if (readAngle == false)
                {
                    m_strError = GetLineCountString(reader) + ", Local Element에 Angle이 존재하지 않습니다.";
                    return false;
                }
                else
                    anchorNode.Angle = dAngle;
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                reader.Close();
                return false;
            }

            return true;
        }

        private Vertex2D ReadPos(XmlTextReader reader)
        {
            string strVertex = "";

            if (ReadElementText(reader, ref strVertex) == false)
            {
                m_strError = GetLineCountString(reader) + ", Pos에 잘못된 값이 존재합니다.";
                return null;
            }

            string[] tokens = strVertex.Split(',');

            if (tokens.Count() != 2)
            {
                m_strError = GetLineCountString(reader) + ", Pos는 x, y 형태이어야만 합니다.";
                return null;
            }

            double x, y;

            if (double.TryParse(tokens[0].Trim(), out x) == false || double.TryParse(tokens[1].Trim(), out y) == false)
            {
                m_strError = GetLineCountString(reader) + ", Pos는 숫자만 사용 가능합니다.";
                return null;
            }

            return new Vertex2D(x, y);
        }

        private bool ReadDouble(XmlTextReader reader, string strElementName, out double data)
        {
            data = 0.0;
            string str = "";

            if (ReadElementText(reader, ref str) == false)
            {
                m_strError = GetLineCountString(reader) + ", " + strElementName + "에 잘못된 값이 존재합니다.";
                return false;
            }

            if (double.TryParse(str.Trim(), out data) == false)
            {
                m_strError = GetLineCountString(reader) + ", " + strElementName + "는 숫자만 사용 가능합니다.";
                return false;
            }

            return true;
        }

        private bool ReadBoolean(XmlTextReader reader, string strElementName, out bool data)
        {
            data = false;
            string str = "";

            if (ReadElementText(reader, ref str) == false)
            {
                m_strError = GetLineCountString(reader) + ", " + strElementName + "에 잘못된 값이 존재합니다.";
                return false;
            }

            if (str == "1" || string.Compare(str, "true", true) == 0)
                data = true;
            else if (str == "0" || string.Compare(str, "false", true) == 0)
                data = false;
            else
                return false;

            return true;
        }

        private bool ReadInt(XmlTextReader reader, string strElementName, out int data)
        {
            string str = null;
            data = 0;

            if (ReadElementText(reader, ref str) == false)
            {
                m_strError = GetLineCountString(reader) + ", " + strElementName + "에 잘못된 값이 존재합니다.";
                return false;
            }

            if (int.TryParse(str.Trim(), out data) == false)
            {
                m_strError = GetLineCountString(reader) + ", " + strElementName + "는 정수 형태의 숫자만 사용 가능합니다.";
                return false;
            }

            return true;
        }

        private AnchorNode LoadAnchorNode(XmlTextReader reader)
        {
            AnchorNode anchorNode = new AnchorNode();

            try
            {
                bool emptyElement = reader.IsEmptyElement;

                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Global", true) == 0)
                            {
                                if (LoadGlobal(reader, anchorNode) == false)
                                    return null;
                            }
                            else if (string.Compare(reader.Name, "Local", true) == 0)
                            {
                                if (LoadLocal(reader, anchorNode) == false)
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
                m_strError = e.Message;
                reader.Close();
                return null;
            }

            return anchorNode;
        }

        private bool LoadCommon(XmlTextReader reader, Dictionary<int, POIType> dicPOITypes, Dictionary<string, Component> dicComponents)
        {
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
                                if (!LoadComponents(reader, dicComponents))
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
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private bool LoadComponents(XmlTextReader reader, Dictionary<string, Component> dicComponents)
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
                                Component component = LoadComponent(reader);

                                if (component == null)
                                    return false;
                                else
                                    dicComponents[component.ID] = component;
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
                m_strError = e.Message;
                return false;
            }

            return true;
        }

        private Component LoadComponent(XmlTextReader reader)
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
                    m_strError = GetLineCountString(reader) + ", Component Element에 id가 존재하지 않습니다.";
                    return null;
                }

                if (strTypeName == null)
                {
                    m_strError = GetLineCountString(reader) + ", Component Element에 type이 존재하지 않습니다.";
                    return null;
                }

                Component component = new Component();

                component.TypeName = strTypeName;
                component.ID = strComponentID;

                if (emtpyElement)
                    return component;

                string strComponentName = "";
                ReadElementText(reader, ref strComponentName);

                component.MaterialName = strComponentName;
                return component;
            }
            catch (Exception e)
            {
                m_strError = e.Message;
            }

            return null;
        }

        public void ReadProject(string strFilePath, Dictionary<int, POIType> dicPOITypes)
        {
            try
            {
                m_strError = "";

                bool stop = false;

                XmlTextReader reader = new XmlTextReader(strFilePath);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "IndoorModelFile", true) == 0)
                            {
                                ReadIndoorModelFile(reader, dicPOITypes);
                                reader.Close();
                                return;
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
                m_strError = e.Message;
                return;
            }

            return;
        }

        private void ReadIndoorModelFile(XmlTextReader reader, Dictionary<int, POIType> dicPOITypes)
        {
            try
            {
                bool stop = false;
                string strVersion = "";

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "version", true) == 0)
                    {
                        strVersion = reader.Value;
                    }
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Common", true) == 0)
                            {
                                ReadCommon(reader, dicPOITypes);
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
                m_strError = e.Message;
                return ;
            }
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

        private bool ReadCommon(XmlTextReader reader, Dictionary<int, POIType> dicPOITypes)
        {
            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "POITypes", true) == 0)
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
                m_strError = e.Message;
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
                m_strError = e.Message;
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
                    //else if (string.Compare(reader.Name, "name", true) == 0)
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
                //    m_strError = GetLineCountString(reader) + ", POITypeGroup Element에 id 속성이 존재하지 않습니다.";
                //    return false;
                //}

                if (strName == null)
                {
                    m_strError = GetLineCountString(reader) + ", POITypeGroup Element에 name 속성이 존재하지 않습니다.";
                    return false;
                }

                if (strUserDefined == null)
                {
                    m_strError = GetLineCountString(reader) + ", POITypeGroup Element에 userDefined 속성이 존재하지 않습니다.";
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
                    m_strError = GetLineCountString(reader) + ", POITypeGroup Element에 userDefined 속성이 잘못된 데이터를 가집니다.";
                    return false;
                }

                POIType poiTypetmp = null;
                POIType poiType = null;
                //int nID = strID.GetHashCode();
                int nID = ++m_nID;
                strID = m_nID.ToString();

                bool bExist = dicPOITypes.TryGetValue(nID, out poiTypetmp);
 
                if (!bExist)
                {
                    
                    poiType = new POIType();

                    poiType.ID = nID;
                    poiType.XMLID = strID;
                    poiType.Name = strName;
                    poiType.IsUserDefined = isUserDefined;
                    poiType.DefaultHeight = strDefaultHeight;
                    poiType.Parent = parent;
                    //  poiType.ParentID = (parent == null) ? 0 : parent.ID;
                    poiType.IsGroup = isGroup;
                    dicPOITypes[nID] = poiType;

                    // 코드 읽는 방식이 달라짐
                    strPOICode = "";
                    //poiType.Code = (strCode == null) ? "" : strCode;
                    if (strCode != null && parent != null)
                        strPOICode = parent.Code + strCode;
                    else if (strCode != null)
                        strPOICode = strCode;

                    poiType.Code = strPOICode;

                    if (poiType.Code != "" && poiType.IsGroup == false)
                    {
                        strPOICode = CheckPOICode(poiType.Code);
                        poiType.Code = strPOICode;
                    }

                    dicPOITypes[nID] = poiType;
                }
                else
                {
                    if(poiTypetmp.IsGroup)
                    {
                        POIType poiTypeChild = new POIType();
                        poiTypeChild.ID = nID;
                        poiTypeChild.XMLID = strID;
                        poiTypeChild.Name = strName;
                        poiTypeChild.IsUserDefined = isUserDefined;
                        poiTypeChild.Code = (strCode == null) ? "" : strCode;
                        poiTypeChild.DefaultHeight = strDefaultHeight;
                        poiTypeChild.Parent = parent;
                        poiTypeChild.IsGroup = isGroup;

                        // 코드 읽는 방식이 달라짐
                        strPOICode = "";
                        //poiType.Code = (strCode == null) ? "" : strCode;
                        if (strCode != null && parent != null)
                            strPOICode = parent.Code + strCode;
                        else if (strCode != null)
                            strPOICode = strCode;

                        poiTypeChild.Code = strPOICode;

                        if (poiTypeChild.Code != "" && poiTypeChild.IsGroup == false)
                        {
                            strPOICode = CheckPOICode(poiTypeChild.Code);
                            poiTypeChild.Code = strPOICode;
                        }

                        dicPOITypes[nID] = poiTypeChild;
                    }
                }

                // Line POIType
                if (m_strPOIWireTable.ContainsKey(strPOICode))
                {
                    bool bChk = false;

                    // Wire 속성 유무 확인
                    foreach (Property pro in poiType.Properties)
                    {
                        if (pro.Name == "Wire")
                            bChk = true;
                    }

                    if (bChk == false)
                    {
                        Property prop = new Property();
                        prop.Name = "Wire";
                        prop.Value = "1";
                        prop.Description = "배선심볼로 사용되는가?";
                        poiType.Properties.Add(prop);
                    }
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
                m_strError = e.Message;
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
                m_strError = e.Message;
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
                                    m_strError = GetLineCountString(reader) + ", Name에 잘못된 값이 존재합니다.";
                                    return null;
                                }
                                else
                                    property.Name = strName;
                            }
                            else if (string.Compare(reader.Name, "Value", true) == 0)
                            {
                                if (ReadElementText(reader, ref strValue) == false)
                                {
                                    m_strError = GetLineCountString(reader) + ", Value에 잘못된 값이 존재합니다.";
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
                    m_strError = GetLineCountString(reader) + ", Property Element에 Name이 존재하지 않습니다.";
                    return null;
                }

                if (strValue == null)
                {
                    m_strError = GetLineCountString(reader) + ", Property Element에 Value가 존재하지 않습니다.";
                    return null;
                }
            }
            catch (Exception e)
            {
                m_strError = e.Message;
                reader.Close();
                return null;
            }

            return property;
        }

        private string GetLineCountString(XmlTextReader reader)
        {
            return "Line : " + reader.LineNumber.ToString();
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

    }
}
