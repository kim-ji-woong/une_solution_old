using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DXFViewer;
using UnE.Geometry;
using System.Xml;

namespace LandAddressReader
{
    public class DataManager
    {
        // Key : 지번 주소
        private List<LandAddressData2> m_listLandAddr = new List<LandAddressData2>();
        private string m_strResult = "";

        public string ResultString
        {
            get { return m_strResult; }
        }

        public DataManager()
        {
        }

        public bool ReadDXF(string strPath, Vertex2D vMoved, out int nOverLayerCount, out int nEmptyLayerCount)
        {
            nOverLayerCount = nEmptyLayerCount = 0;

            DXFControl ctrl = new DXFControl();
            
            if (!ctrl.OpenDXF(strPath))
            {
                m_strResult = "DXF 파일을 열수 없습니다.";
                return false;
            }

            List<Layer> overLayers = new List<Layer>();
            List<Layer> emptyLayers = new List<Layer>();

            foreach (Layer layer in ctrl.Layers)
            {
                if (layer.LayerName == "0")
                    continue;

                LandAddressData2 data = new LandAddressData2(layer.LayerName);

                foreach (Shape shape in layer.Shapes)
                {
                    if (shape.GetShapeType() != Shape.ShapeType.POLYLINE)
                        continue;

                    PolyLine pLine = (PolyLine)shape;

                    if (vMoved != null)
                        pLine.Move(vMoved.x - ctrl.MovedVertex.x, vMoved.y - ctrl.MovedVertex.y);

                    if (data.Boundary == null)
                        data.Boundary = pLine;
                    else
                    {
                        overLayers.Add(layer);
                        break;
                    }
                }

                if (data.Boundary == null)
                {
                    emptyLayers.Add(layer);
                }

                m_listLandAddr.Add(data);
            }

            WriteFile("OverLayer.csv", overLayers);
            WriteFile("EmptyLayer.csv", emptyLayers);

            nOverLayerCount = overLayers.Count;
            nEmptyLayerCount = emptyLayers.Count;

            m_listLandAddr.Sort();
            m_strResult = "성공 : 지번개수(" + m_listLandAddr.Count.ToString() + ")";
            return true;
        }

        private void WriteFile(string strPath, List<Layer> layers)
        {
            if (layers.Count == 0)
                return;

            System.IO.StreamWriter writer = new System.IO.StreamWriter(strPath, false, Encoding.UTF8);
            writer.WriteLine("Layer 이름, PolyLine 개수");

            foreach (Layer layer in layers)
            {
                int nCount = 0;

                foreach (Shape shape in layer.Shapes)
                {
                    if (shape.GetShapeType() == Shape.ShapeType.POLYLINE)
                        nCount++;
                }

                writer.WriteLine(string.Format("{0}, {1}", layer.LayerName, nCount));
            }

            writer.Close();
        }

        public void SaveXML(string strPath)
        {
            XmlTextWriter writer = null;

            try
            {
                writer = new XmlTextWriter(strPath, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                writer.WriteStartElement("LandAddressList");

                SaveLandAddrList(writer);
            }
            catch (Exception)
            {
                return;
            }

            writer.WriteEndElement();
            writer.Close();
        }

        public void SaveLandAddrList(XmlTextWriter writer)
        {
            foreach (LandAddressData2 data in m_listLandAddr)
            {
                writer.WriteStartElement("LandAddress");

                writer.WriteStartElement("Name");
                writer.WriteString(data.ToString());
                writer.WriteEndElement();
                System.Diagnostics.Trace.WriteLine(data.ToString());

                writer.WriteStartElement("PolyLine");
                SavePolyLine(writer, data.Boundary);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        private void SavePolyLine(XmlTextWriter writer, PolyLine pLine)
        {
            int nVertexCount = pLine.GetVertexSize();

            for (int i=0;i<nVertexCount;i++)
            {
                System.Drawing.PointF pt = pLine.GetVertex(i);
                SavePointF(writer, ref pt, "Vertex2D");
            }
        }

        private void SavePointF(XmlTextWriter writer, ref System.Drawing.PointF pt, string strTagName)
        {
            writer.WriteStartElement(strTagName);

            writer.WriteStartAttribute("x");
            writer.WriteString(pt.X.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y");
            writer.WriteString(pt.Y.ToString());
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }
    }
}
