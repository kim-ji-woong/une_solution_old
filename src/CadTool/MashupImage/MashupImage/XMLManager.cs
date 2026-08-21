using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Drawing;

namespace MashupImage
{
    public class XMLManager
    {
        public const string TARGET_VERSION = "1.0";
        private const string m_strDoubleFormat = "F1";

        private string m_strErrorMessage = "";

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public Project Read(string strPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(strPath);

            Project project = new Project();
            bool readMap = false;

            foreach (XmlNode rootNode in xmlDoc.ChildNodes)
            {
                if (rootNode.Name == "Map2D")
                {
                    foreach (XmlNode node in rootNode.ChildNodes)
                    {
                        if (node.Name == "Map")
                        {
                            if (ReadMap(project, node) == false)
                                return null;

                            readMap = true;
                        }
                        else if (node.Name == "Shapes")
                        {
                            if (ReadShapes(project, node) == false)
                                return null;
                        }
                    }
                }
            }

            if (readMap == false)
            {
                m_strErrorMessage = "지정된 형식의 파일이 아닙니다.";
                return null;
            }

            return project;
        }

        private bool ReadShapes(Project project, XmlNode nodeMap)
        {
            foreach (XmlNode node in nodeMap.ChildNodes)
            {
                if (node.Name == "Shape")
                {
                    Shape shape = ReadShape(project.LODs, node);

                    if (shape == null)
                        return false;

                    project.Shapes.Add(shape);
                }
            }

            return true;
        }

        private Shape ReadShape(List<LOD> lods, XmlNode nodeShape)
        {
            Shape shape = new Shape();
            bool readName = false;

            foreach (XmlAttribute attr in nodeShape.Attributes)
            {
                if (attr.Name == "name")
                {
                    shape.Name = attr.Value;
                    readName = true;
                }
            }

            if (readName == false)
            {
                m_strErrorMessage = "name 속성이 없는 Shape이 존재합니다.";
                return null;
            }

            bool readImage = false, readLOD = false;

            foreach (XmlNode node in nodeShape.ChildNodes)
            {
                if (node.Name == "Image")
                {
                    if (ReadShapeImage(shape, node) == false)
                        return null;
                    else
                        readImage = true;
                }
                else if (node.Name == "LODImage")
                {
                    if (ReadShapeLODImage(lods, shape, node) == false)
                        return null;
                    else
                        readLOD = true;
                }
            }

            if (readImage)
                shape.UseLODImage = false;
            else if (readLOD)
                shape.UseLODImage = true;
            else
            {
                m_strErrorMessage = "Shape Element에 LODImage 또는 Image가 존재하지 않습니다.";
                return null;
            }

            return shape;
        }

        private bool ReadShapeLODImage(List<LOD> lods, Shape shape, XmlNode nodeImage)
        {
            foreach (XmlNode node in nodeImage.ChildNodes)
            {
                if (node.Name == "LOD")
                {
                    if (ReadShapeLOD(lods, shape, node) == false)
                        return false;
                }
            }

            return true;
        }

        private bool ReadShapeLOD(List<LOD> lods, Shape shape, XmlNode nodeLOD)
        {
            string strID = null;

            foreach (XmlAttribute attr in nodeLOD.Attributes)
            {
                if (attr.Name == "level")
                {
                    strID = attr.Value;
                }
            }

            if (strID == null)
            {
                m_strErrorMessage = "Shape/LODImage/LOD에 level 속성이 존재하지 않습니다.";
                return false;
            }

            LODImage lodImage = null;

            foreach (LOD lod in lods)
            {
                if (lod.ID == strID)
                {
                    lodImage = new LODImage();
                    lodImage.LOD = lod;
                    break;
                }
            }

            if (lodImage == null)
            {
                m_strErrorMessage = "Shape/LODImage/LOD에 " + strID + "라는 알수없는 level 값이 있습니다.";
                return false;
            }

            bool readPos = false, readPath = false;

            foreach (XmlNode node in nodeLOD.ChildNodes)
            {
                if (node.Name == "Pos")
                {
                    System.Drawing.PointF pos;

                    if (ReadPosition(out pos, node) == false)
                        return false;

                    lodImage.Position = pos;
                    readPos = true;
                }
                else if (node.Name == "Path")
                {
                    lodImage.ImagePath = node.InnerText.Trim();
                    readPath = true;
                }
            }

            if (readPos == false)
            {
                m_strErrorMessage = "Shape/Image에 Pos Element가 존재하지 않습니다.";
                return false;
            }

            if (readPath == false)
            {
                m_strErrorMessage = "Shape/Image에 Path Element가 존재하지 않습니다.";
                return false;
            }

            shape.LODImages.Add(lodImage);
            return true;
        }

        private bool ReadShapeImage(Shape shape, XmlNode nodeImage)
        {
            bool readPos = false, readPath = false;

            foreach (XmlNode node in nodeImage.ChildNodes)
            {
                if (node.Name == "Pos")
                {
                    System.Drawing.PointF position;

                    if (ReadPosition(out position, node) == false)
                        return false;

                    shape.Position = position;
                    readPos = true;
                }
                else if (node.Name == "Path")
                {
                    shape.ImagePath = node.InnerText.Trim();

                    if (File.Exists(shape.ImagePath) == false)
                    {
                        m_strErrorMessage = shape.ImagePath + "는 존재하지 않는 파일입니다.";
                        return false;
                    }

                    shape.Image = Image.FromFile(shape.ImagePath);
                    readPath = true;
                }
            }

            if (readPos == false)
            {
                m_strErrorMessage = "Shape/Image에 Pos Element가 존재하지 않습니다.";
                return false;
            }

            if (readPath == false)
            {
                m_strErrorMessage = "Shape/Image에 Path Element가 존재하지 않습니다.";
                return false;
            }

            return true;
        }

        private bool ReadPosition(out System.Drawing.PointF pos, XmlNode node)
        {
            pos = new System.Drawing.PointF();
            string[] tokens = node.InnerText.Trim().Split(',');

            if (tokens.Count() != 2)
                return false;

            float x, y;

            if (float.TryParse(tokens[0].Trim(), out x) && float.TryParse(tokens[1].Trim(), out y))
            {
                pos.X = x;
                pos.Y = y;
                return true;
            }

            return false;
        }

        private bool ReadMap(Project project, XmlNode nodeMap)
        {
            // Key : LOD ID
            Dictionary<string, LOD> dicLODs = new Dictionary<string, LOD>();
            LOD lodTemp;

            foreach (XmlNode node in nodeMap.ChildNodes)
            {
                if (node.Name == "LOD")
                {
                    LOD lod = ReadLOD(node);

                    if (lod == null)
                        return false;

                    if (dicLODs.TryGetValue(lod.ID, out lodTemp))
                    {
                        m_strErrorMessage = string.Format("level이 {0}인 LOD가 2개 이상 존재합니다.", lod.ID);
                        return false;
                    }

                    dicLODs[lod.ID] = lod;
                }
            }

            foreach (XmlNode node in nodeMap.ChildNodes)
            {
                if (node.Name == "LOD")
                {
                    LOD lod = ReadLOD(node, dicLODs);

                    if (lod == null)
                        return false;

                    project.LODs.Add(lod);
                }
            }

            return true;
        }

        private LOD ReadLOD(XmlNode nodeLOD)
        {
            LOD lod = new LOD();

            foreach (XmlAttribute attr in nodeLOD.Attributes)
            {
                if (attr.Name == "level")
                    lod.ID = attr.Value;
            }

            if (lod.ID.Length == 0)
            {
                m_strErrorMessage = "level이 없는 LOD가 존재합니다.";
                return null;
            }

            return lod;
        }

        private LOD ReadLOD(XmlNode nodeLOD, Dictionary<string, LOD> dicLODs)
        {
            LOD lod = null;

            foreach (XmlAttribute attr in nodeLOD.Attributes)
            {
                if (attr.Name == "level")
                {
                    if (dicLODs.TryGetValue(attr.Value, out lod) == false)
                        return null;
                }
            }

            if (lod == null)
                return null;

            bool readFolder = false, readLayout = false, readRatio = false;

            foreach (XmlNode node in nodeLOD.ChildNodes)
            {
                if (node.Name == "Folder")
                {
                    lod.FolderName = node.InnerText;
                    readFolder = true;
                }
                else if (node.Name == "AddPixel")
                {
                    int nAddPixel;

                    if (int.TryParse(node.InnerText.Trim(), out nAddPixel) == false || nAddPixel < 0)
                    {
                        m_strErrorMessage = "AddPixel에 잘못된 데이터가 존재합니다.";
                        return null;
                    }

                    lod.AddPixel = (uint)nAddPixel;
                }
                else if (node.Name == "Layout")
                {
                    if (ReadLayout(lod, node) == false)
                        return null;

                    readLayout = true;
                }
                else if (node.Name == "Ratio")
                {
                    Ratio ratio = ReadRatio(node, lod, dicLODs);

                    if (ratio == null)
                        return null;
                    else
                    {
                        lod.Ratio = ratio;
                        readRatio = true;
                    }
                }
            }

            if (readFolder == false)
            {
                m_strErrorMessage = "Folder가 없는 LOD가 존재합니다.";
                return null;
            }

            if (readLayout == false)
            {
                m_strErrorMessage = "Layout이 없는 LOD가 존재합니다.";
                return null;
            }

            if (readRatio == false)
                lod.Ratio = null;

            return lod;
        }

        private Ratio ReadRatio(XmlNode nodeRatio, LOD lodException, Dictionary<string, LOD> dicLODs)
        {
            Ratio ratio = new Ratio();

            foreach (XmlAttribute attr in nodeRatio.Attributes)
            {
                if (attr.Name == "base")
                {
                    LOD lod = null;

                    if (dicLODs.TryGetValue(attr.Value, out lod) == false)
                    {
                        m_strErrorMessage = string.Format("base=\"{0}\"는 존재하지 않는 LOD입니다.", attr.Value);
                        return null;
                    }

                    if (lod == lodException)
                    {
                        m_strErrorMessage = string.Format("base=\"{0}\"는 자기 자신을 가르키면 안됩니다.", attr.Value);
                        return null;
                    }

                    ratio.BaseLOD = lod;
                }
            }

            if (ratio.BaseLOD == null)
            {
                m_strErrorMessage = "base가 없는 Ratio가 존재합니다.";
                return null;
            }

            foreach (XmlNode node in nodeRatio.ChildNodes)
            {
                if (node.Name == "Pixel")
                {
                    if (ReadRatioPixel(ratio, node) == false)
                        return null;

                    ratio.UsePercent = false;
                }
                else if (node.Name == "Percent")
                {
                    if (ReadRatioPercent(ratio, node) == false)
                        return null;

                    ratio.UsePercent = true;
                }
            }

            return ratio;
        }

        private bool ReadRatioPercent(Ratio ratio, XmlNode nodePercent)
        {
            double dWidth, dHeight;
            bool readWidth = false, readHeight = false;

            foreach (XmlNode node in nodePercent.ChildNodes)
            {
                if (node.Name == "Width")
                {
                    if (ReadDouble(node, "Ratio/Percent/Width", out dWidth) == false)
                        return false;

                    ratio.HPercent = dWidth;
                    readWidth = true;
                }
                else if (node.Name == "Height")
                {
                    if (ReadDouble(node, "Ratio/Percent/Height", out dHeight) == false)
                        return false;

                    ratio.VPercent = dHeight;
                    readHeight = true;
                }
            }

            if (readWidth == false)
            {
                m_strErrorMessage = "Width가 없는 Percent가 존재합니다.";
                return false;
            }

            if (readHeight == false)
            {
                m_strErrorMessage = "Height가 없는 Percent가 존재합니다.";
                return false;
            }

            return true;
        }

        private bool ReadRatioPixel(Ratio ratio, XmlNode nodePixel)
        {
            bool readWidth = false, readHeight = false;

            foreach (XmlNode node in nodePixel.ChildNodes)
            {
                if (node.Name == "Width")
                {
                    if (ReadRatioPixel(ratio, node.Name, node) == false)
                        return false;

                    readWidth = true;
                }
                else if (node.Name == "Height")
                {
                    if (ReadRatioPixel(ratio, node.Name, node) == false)
                        return false;

                    readHeight = true;
                }
            }

            if (readWidth == false)
            {
                m_strErrorMessage = "Width가 없는 Pixel이 존재합니다.";
                return false;
            }

            if (readHeight == false)
            {
                m_strErrorMessage = "Height가 없는 Pixel이 존재합니다.";
                return false;
            }

            return true;
        }

        private bool ReadRatioPixel(Ratio ratio, string strTag, XmlNode node)
        {
            int nBase = 0, nCurrent = 0;
            bool readBase = false, readCurrent = false;

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "Base")
                {
                    if (ReadInt(child, child.Name, out nBase) == false)
                        return false;

                    readBase = true;
                }
                else if (child.Name == "Current")
                {
                    if (ReadInt(child, child.Name, out nCurrent) == false)
                        return false;

                    readCurrent = true;
                }
            }

            if (readBase == false)
            {
                m_strErrorMessage = string.Format("Base가 없는 {0}이 존재합니다.", strTag);
                return false;
            }

            if (readCurrent == false)
            {
                m_strErrorMessage = string.Format("Current가 없는 {0}이 존재합니다.", strTag);
                return false;
            }

            if (strTag == "Width")
            {
                ratio.BaseWidth = nBase;
                ratio.CurrentWidth = nCurrent;
            }
            else
            {
                ratio.BaseHeight = nBase;
                ratio.CurrentHeight = nCurrent;
            }

            return true;
        }

        private bool ReadLayout(LOD lod, XmlNode nodeLayout)
        {
            int nImageWidth, nImageHeight, nHCount, nVCount;
            bool readImageWidth = false, readImageHeight = false;
            bool readHCount = false, readVCount = false;

            foreach (XmlNode node in nodeLayout.ChildNodes)
            {
                if (node.Name == "ImageWidth")
                {
                    if (ReadInt(node, node.Name, out nImageWidth) == false)
                        return false;

                    lod.ImageWidth = nImageWidth;
                    readImageWidth = true;
                }
                else if (node.Name == "ImageHeight")
                {
                    if (ReadInt(node, node.Name, out nImageHeight) == false)
                        return false;

                    lod.ImageHeight = nImageHeight;
                    readImageHeight = true;
                }
                else if (node.Name == "Horz")
                {
                    if (ReadInt(node, node.Name, out nHCount) == false)
                        return false;

                    lod.ImageHCount = nHCount;
                    readHCount = true;
                }
                else if (node.Name == "Vert")
                {
                    if (ReadInt(node, node.Name, out nVCount) == false)
                        return false;

                    lod.ImageVCount = nVCount;
                    readVCount = true;
                }
            }

            if (readImageWidth == false)
            {
                m_strErrorMessage = "ImageWidth가 없는 Layout이 존재합니다.";
                return false;
            }

            if (readImageHeight == false)
            {
                m_strErrorMessage = "ImageHeight가 없는 Layout이 존재합니다.";
                return false;
            }

            if (readHCount == false)
            {
                m_strErrorMessage = "Horz가 없는 Layout이 존재합니다.";
                return false;
            }

            if (readVCount == false)
            {
                m_strErrorMessage = "Vert가 없는 Layout이 존재합니다.";
                return false;
            }

            return true;
        }

        private bool ReadInt(XmlNode node, string strTag, out int value)
        {
            if (int.TryParse(node.InnerText.Trim(), out value) == false)
            {
                m_strErrorMessage = strTag + "에 잘못된 데이터가 존재합니다.";
                return false;
            }

            return true;
        }

        private bool ReadDouble(XmlNode node, string strTag, out double value)
        {
            if (double.TryParse(node.InnerText.Trim(), out value) == false)
            {
                m_strErrorMessage = strTag + "에 잘못된 데이터가 존재합니다.";
                return false;
            }

            return true;
        }

        public bool Save(Project project)
        {
            bool result = true;

            try
            {
                XmlTextWriter writer = new XmlTextWriter(project.ProjectPath, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                result = WriteMap2D(project, writer);

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

        private bool WriteMap2D(Project project, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Map2D");

                writer.WriteStartAttribute("version");
                writer.WriteString(TARGET_VERSION);
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("xmlns:xsi");
                writer.WriteString("http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteEndAttribute();

                writer.WriteStartAttribute("xsi:noNamespaceSchemaLocation");
                writer.WriteString("http://unes.iptime.org:8001/Schema/LODImage.xsd");
                writer.WriteEndAttribute();

                if (WriteMap(project, writer) == false)
                    return false;

                if (WriteShapes(project, writer) == false)
                    return false;
                
                writer.WriteFullEndElement();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool WriteShapes(Project project, XmlTextWriter writer)
        {
            if (project.Shapes.Count == 0)
            {
                return true;
            }

            try
            {
                writer.WriteStartElement("Shapes");

                foreach (Shape shape in project.Shapes)
                {
                    if (WriteShape(shape, writer) == false)
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

        private bool WriteShape(Shape shape, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Shape");

                writer.WriteStartAttribute("name");
                writer.WriteString(shape.Name);
                writer.WriteEndAttribute();

                if (shape.UseLODImage)
                {
                    if (shape.LODImages.Count == 0)
                    {
                        m_strErrorMessage = "LODImage가 하나도 없는 Shape이 있습니다.";
                        return false;
                    }

                    writer.WriteStartElement("LODImage");

                    foreach (LODImage lodImage in shape.LODImages)
                    {
                        if (WriteLODImage(lodImage, writer) == false)
                            return false;
                    }

                    writer.WriteFullEndElement();
                }
                else
                {
                    if (WriteShapeImage(shape, writer) == false)
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

        private bool WriteShapeImage(Shape shape, XmlTextWriter writer)
        {
            writer.WriteStartElement("Image");

            WritePosition(shape.Position, writer);

            writer.WriteStartElement("Path");
            writer.WriteString(shape.ImagePath);
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();

            return true;
        }

        private bool WriteLODImage(LODImage lodImage, XmlTextWriter writer)
        {
            writer.WriteStartElement("LOD");

            writer.WriteStartAttribute("level");
            writer.WriteString(lodImage.LOD.ID);
            writer.WriteEndAttribute();

            WritePosition(lodImage.Position, writer);

            writer.WriteStartElement("Path");
            writer.WriteString(lodImage.ImagePath);
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();
    
            return true;
        }

        private void WritePosition(System.Drawing.PointF pos, XmlTextWriter writer)
        {
            string strX = string.Format("{0:F1}", pos.X);
            string strY = string.Format("{0:F1}", pos.Y);

            if (strX.EndsWith(".0"))
                strX = strX.Substring(0, strX.Length - 2);

            if (strY.EndsWith(".0"))
                strY = strY.Substring(0, strY.Length - 2);

            writer.WriteStartElement("Pos");
            writer.WriteString(strX + "," + strY);
            writer.WriteFullEndElement();
        }

        private bool WriteMap(Project project, XmlTextWriter writer)
        {
            if (project.LODs.Count == 0)
            {
                m_strErrorMessage = "Project에 LOD 데이터가 하나도 없습니다.";
                return false;
            }

            try
            {
                writer.WriteStartElement("Map");

                foreach (LOD lod in project.LODs)
                {
                    if (WriteLOD(lod, writer) == false)
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

        private bool WriteLOD(LOD lod, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("LOD");

                writer.WriteStartAttribute("level");
                writer.WriteString(lod.ID);
                writer.WriteEndAttribute();

                writer.WriteStartElement("Folder");
                writer.WriteString(lod.FolderName);
                writer.WriteFullEndElement();

                writer.WriteStartElement("AddPixel");
                writer.WriteString(lod.AddPixel.ToString());
                writer.WriteFullEndElement();

                WriteLayout(lod, writer);

                if (lod.Ratio != null)
                {
                    if (WriteRatio(lod.Ratio, writer) == false)
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

        private bool WriteRatio(Ratio ratio, XmlTextWriter writer)
        {
            if (ratio.BaseLOD == null)
            {
                m_strErrorMessage = "Base LOD가 null인 Ratio가 존재합니다.";
                return false;
            }

            try
            {
                writer.WriteStartElement("Ratio");

                writer.WriteStartAttribute("base");
                writer.WriteString(ratio.BaseLOD.ID);
                writer.WriteEndAttribute();

                if (ratio.UsePercent)
                {
                    writer.WriteStartElement("Percent");

                    writer.WriteStartElement("Width");
                    writer.WriteString(GetDoubleString(ratio.HPercent));
                    writer.WriteFullEndElement();

                    writer.WriteStartElement("Height");
                    writer.WriteString(GetDoubleString(ratio.VPercent));
                    writer.WriteFullEndElement();

                    writer.WriteFullEndElement();
                }
                else
                {
                    writer.WriteStartElement("Pixel");

                    WriteRatioPixel("Width", ratio.BaseWidth, ratio.CurrentWidth, writer);
                    WriteRatioPixel("Height", ratio.BaseHeight, ratio.CurrentHeight, writer);

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

        private void WriteRatioPixel(string strTag, int nBase, int nCurrent, XmlTextWriter writer)
        {
            writer.WriteStartElement(strTag);

            writer.WriteStartElement("Base");
            writer.WriteString(nBase.ToString());
            writer.WriteFullEndElement();

            writer.WriteStartElement("Current");
            writer.WriteString(nCurrent.ToString());
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();
        }

        private string GetDoubleString(double data)
        {
            return string.Format("{0:" + m_strDoubleFormat + "}", data);
        }

        private bool WriteLayout(LOD lod, XmlTextWriter writer)
        {
            try
            {
                writer.WriteStartElement("Layout");

                writer.WriteStartElement("ImageWidth");
                writer.WriteString(lod.ImageWidth.ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("ImageHeight");
                writer.WriteString(lod.ImageHeight.ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("Horz");
                writer.WriteString(lod.ImageHCount.ToString());
                writer.WriteFullEndElement();

                writer.WriteStartElement("Vert");
                writer.WriteString(lod.ImageVCount.ToString());
                writer.WriteFullEndElement();

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
