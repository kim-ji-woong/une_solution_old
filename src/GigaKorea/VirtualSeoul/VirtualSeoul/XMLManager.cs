using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace VirtualSeoul
{
    public class XMLManager
    {
        private string m_strError = "";

        public string ErrorMessage
        {
            get { return m_strError; }
        }

        public bool Export(string strPath, List<Level> levels)
        {
            try
            {
                levels.Sort();

                XmlTextWriter writer = new XmlTextWriter(strPath, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                WritePML(levels, writer);

                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public List<Level> Import(string strPath, Dictionary<string, POIType> dicPOITypes)
        {
            XElement pml = XElement.Load(strPath);

            if (pml.Name != "PML")
                return null;

            XElement xLevels = FindElement(pml, "Levels");

            if (xLevels == null)
                return null;

            List<Level> levels = new List<Level>();

            foreach (XElement element in xLevels.Elements())
            {
                if (element.Name == "Level")
                {
                    Level level = ReadLevel(element, dicPOITypes);

                    if (level == null)
                        return null;

                    levels.Add(level);
                }
            }

            levels.Sort();
            return levels;
        }

        private Level ReadLevel(XElement xLevel, Dictionary<string, POIType> dicPOITypes)
        {
            Level level = new Level();

            foreach (XElement element in xLevel.Elements())
            {
                if (element.Name == "ID")
                    level.ID = element.Value.Trim();
                else if (element.Name == "FloorIndex")
                {
                    int nIndex;

                    if (int.TryParse(element.Value.Trim(), out nIndex))
                        level.FloorIndex = nIndex;
                    else
                        return null;
                }
                else if (element.Name == "Height")
                {
                    int nHeight;

                    if (int.TryParse(element.Value.Trim(), out nHeight))
                        level.Height = nHeight;
                    else
                        return null;
                }
                else if (element.Name == "Elevation")
                {
                    int nElevation;

                    if (int.TryParse(element.Value.Trim(), out nElevation))
                        level.Elevation = nElevation;
                    else
                        return null;
                }
                else if (element.Name == "POIs")
                {
                    foreach (XElement node in element.Elements())
                    {
                        if (node.Name == "POI")
                        {
                            POI poi = ReadPOI(node, dicPOITypes);

                            if (poi == null)
                                return null;
                            else
                            {
                                level.POIs.Add(poi);
                                poi.Name = string.Format("{0}_{1}_{2}", poi.POIType.Name, level.Name, level.POIs.Count);
                            }
                        }
                    }
                }
            }

            return level;
        }

        private POI ReadPOI(XElement xPOI, Dictionary<string, POIType> dicPOITypes)
        {
            POIType poiType = null;
            UnE.Geometry.Vertex2D vPos = null;

            foreach (XElement element in xPOI.Elements())
            {
                if (element.Name == "Code")
                {
                    if (dicPOITypes.TryGetValue(element.Value.Trim(), out poiType) == false)
                    {
                        m_strError = string.Format("{0}는 알수없는 POI Code입니다.", element.Value.Trim());
                        return null;
                    }
                }
                else if (element.Name == "Position")
                {
                    string strValue = element.Value.Trim();
                    int nIndex = strValue.IndexOf(',');

                    if (nIndex < 0)
                        return null;

                    string strX = strValue.Substring(0, nIndex).Trim();
                    string strY = strValue.Substring(nIndex + 1).Trim();
                    double x, y;

                    if (double.TryParse(strX, out x) == false || double.TryParse(strY, out y) == false)
                        return null;

                    vPos = new UnE.Geometry.Vertex2D(x, y);
                }
            }

            if (poiType == null || vPos == null)
                return null;

            POI poi = poiType.MakePOI(vPos);

            if (poi != null)
            {
                poi.TL = poiType.TL + vPos;
                poi.BL = poiType.BL + vPos;
                poi.BR = poiType.BR + vPos;

                poi.Position = vPos;
                poi.SetShapePosition();
            }

            return poi;
        }

        private XElement FindElement(XElement node, string strNodeName)
        {
            if (node.Name == strNodeName)
                return node;

            foreach (XElement element in node.Elements())
            {
                XElement _element = FindElement(element, strNodeName);

                if (_element != null)
                    return _element;
            }

            return null;
        }

        private bool WritePML(List<Level> levels, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("PML");

                if (WriteLevels(levels, writer) == false)
                    return false;
                
                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteLevels(List<Level> levels, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Levels");

                foreach (Level level in levels)
                {
                    writer.WriteStartElement("Level");

                    if (WriteLevel(level, writer) == false)
                        return false;

                    writer.WriteFullEndElement();
                }

                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteLevel(Level level, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("ID");
                writer.WriteString(level.ID);
                writer.WriteFullEndElement();

                writer.WriteStartElement("FloorIndex");
                writer.WriteString(level.FloorIndex.ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("Height");
                writer.WriteString(level.Height.ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("Elevation");
                writer.WriteString(level.Elevation.ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("POIs");

                foreach (POI poi in level.POIs)
                {
                    if (poi.POIType == null)
                        continue;

                    writer.WriteStartElement("POI");

                    if (WritePOI(poi, writer) == false)
                        return false;

                    writer.WriteFullEndElement();
                }

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
                writer.WriteStartElement("Code");
                writer.WriteString(poi.POIType.Code);
                writer.WriteFullEndElement();

                writer.WriteStartElement("Position");
                writer.WriteString(string.Format("{0:F2}, {1:F2}", poi.Position.x, poi.Position.y));
                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
