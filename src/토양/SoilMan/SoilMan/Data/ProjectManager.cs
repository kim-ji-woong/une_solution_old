using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace SoilMan.Data
{
    public class ProjectManager
    {
        public class ProjectData
        {
            private List<DXFViewer.Layer> m_수치지도Layers = null;
            private List<DXFViewer.Layer> m_토지이용계획도Layers = null;
            // Key : PNU
            private Dictionary<string, ShapeAttrib> m_dicShapeAttribs = null;

            private string m_str수치지도Path = "";
            private string m_str토지이용계획도Path = "";
            private string m_str지적도Path = "";
            private string m_strTempFolderPath = "";

            private bool m_visible수치지도 = true;
            private bool m_visible토지이용계획도 = true;
            private bool m_visible지적도 = true;

            private DXFViewer.Viewport m_viewport = null;
            private UnE.Geometry.Vertex2D m_vMoved = new UnE.Geometry.Vertex2D();

            private UnE.Geometry.Vertex2D m_v지적도TL = null, m_v지적도BR = null;
            private UnE.Geometry.Vertex2D m_v수치지도TL = null, m_v수치지도BR = null;
            private UnE.Geometry.Vertex2D m_v토지이용계획도TL = null, m_v토지이용계획도BR = null;
            private UnE.Geometry.Vertex2D m_vTopLeft = null, m_vBottomRight = null;

            private List<Overlay.OverlayShape> m_overlayShapes = new List<Overlay.OverlayShape>();
            private Dictionary<LandType, Overlay.AreaNCost> m_dicLandTypeArea = null;
            private TechType m_selectedTechType = TechType.None;
            private Popup.SoilCleanCost m_soilCleanCost = null;

            // 음수값이면 설정되지 않은 상태이다.
            long m_nInheritage = -1, m_nExistance = -1, m_nBio = -1;

            public List<DXFViewer.Layer> 수치지도Layers
            {
                get { return m_수치지도Layers; }
                set { m_수치지도Layers = value; }
            }

            public List<DXFViewer.Layer> 토지이용계획도Layers
            {
                get { return m_토지이용계획도Layers; }
                set { m_토지이용계획도Layers = value; }
            }

            // Key : PNU
            public Dictionary<string, ShapeAttrib> ShapeAttribs
            {
                get { return m_dicShapeAttribs; }
                set { m_dicShapeAttribs = value; }
            }

            public string 수치지도Path
            {
                get { return m_str수치지도Path; }
                set { m_str수치지도Path = value; }
            }

            public string 토지이용계획도Path
            {
                get { return m_str토지이용계획도Path; }
                set { m_str토지이용계획도Path = value; }
            }

            public string 지적도Path
            {
                get { return m_str지적도Path; }
                set { m_str지적도Path = value; }
            }

            public string TempFolderPath
            {
                get { return m_strTempFolderPath; }
                set { m_strTempFolderPath = value; }
            }

            public bool Visible수치지도
            {
                get { return m_visible수치지도; }
                set { m_visible수치지도 = value; }
            }

            public bool Visible토지이용계획도
            {
                get { return m_visible토지이용계획도; }
                set { m_visible토지이용계획도 = value; }
            }

            public bool Visible지적도
            {
                get { return m_visible지적도; }
                set { m_visible지적도 = value; }
            }

            public DXFViewer.Viewport Viewport
            {
                get { return m_viewport; }
                set { m_viewport = value; }
            }

            public UnE.Geometry.Vertex2D 지적도TL
            {
                get { return m_v지적도TL; }
                set { m_v지적도TL = value; }
            }
            
            public UnE.Geometry.Vertex2D 지적도BR
            {
                get { return m_v지적도BR; }
                set { m_v지적도BR = value; }
            }

            public UnE.Geometry.Vertex2D 수치지도TL
            {
                get { return m_v수치지도TL; }
                set { m_v수치지도TL = value; }
            }

            public UnE.Geometry.Vertex2D 수치지도BR
            {
                get { return m_v수치지도BR; }
                set { m_v수치지도BR = value; }
            }

            public UnE.Geometry.Vertex2D 토지이용계획도TL
            {
                get { return m_v토지이용계획도TL; }
                set { m_v토지이용계획도TL = value; }
            }

            public UnE.Geometry.Vertex2D 토지이용계획도BR
            {
                get { return m_v토지이용계획도BR; }
                set { m_v토지이용계획도BR = value; }
            }

            public UnE.Geometry.Vertex2D TopLeft
            {
                get { return m_vTopLeft; }
                set { m_vTopLeft = value; }
            }

            public UnE.Geometry.Vertex2D BottomRight
            {
                get { return m_vBottomRight; }
                set { m_vBottomRight = value; }
            }

            public List<Overlay.OverlayShape> OverlayShapes
            {
                get { return m_overlayShapes; }
            }

            public Dictionary<LandType, Overlay.AreaNCost> LandTypeAreas
            {
                get { return m_dicLandTypeArea; }
                set { m_dicLandTypeArea = value; }
            }

            public TechType SelectedTechType
            {
                get { return m_selectedTechType; }
                set { m_selectedTechType = value; }
            }

            public Popup.SoilCleanCost SoilCleanCost
            {
                get { return m_soilCleanCost; }
                set { m_soilCleanCost = value; }
            }

            // 음수값이면 설정되지 않은 상태이다.
            public long Inheritage
            {
                get { return m_nInheritage; }
                set { m_nInheritage = value; }
            }

            // 음수값이면 설정되지 않은 상태이다.
            public long Existance
            {
                get { return m_nExistance; }
                set { m_nExistance = value; }
            }

            // 음수값이면 설정되지 않은 상태이다.
            public long Bio
            {
                get { return m_nBio; }
                set { m_nBio = value; }
            }

            public UnE.Geometry.Vertex2D MovedVertex
            {
                get { return m_vMoved; }
                set { m_vMoved = value; }
            }

            private long nSelectRegion = 0;
            public long SelectRegion
            {
                get { return nSelectRegion; }
                set { nSelectRegion = value; }
            }

            private long nSelectWTP = 0;
            public long SelectWTP
            {
                get { return nSelectWTP; }
                set { nSelectWTP = value; }
            }

            private double dWTPYear = 0.0;
            public double WTPYear
            {
                get { return dWTPYear; }
                set { dWTPYear = value; }
            }

            private double dRejectionRatio = 0.0;
            public double RejectionRatio
            {
                get { return dRejectionRatio; }
                set { dRejectionRatio = value; }
            }

            private long nHousehold = 0;
            public long Household
            {
                get { return nHousehold; }
                set { nHousehold = value; }
            }

        }

        public const int PNUIndex = 0;
        public const int AreaIndex = 2;
        public const int CostIndex = 3;

        private static string m_strVersionName = "V1.01";

        public static ProjectData Read(string strPath, /*DXFViewer.DXFControl dxfControl, ref string str수치지도Path, ref string str토지이용계획도Path, ref bool visible지적도, ref DXFViewer.Layer layer지적도, DockingForm.FormDetailLayer frm수치지도, DockingForm.FormDetailLayer frm토지이용계획도, */Overlay.OverlayPainter overlayPainter)
        {
            DateTime dtNow = DateTime.Now;
            string strNow = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            string strTempFolder = Path.GetTempPath() + strNow;

            /*dxfControl.Layers.Clear();
            str수치지도Path = "";
            str토지이용계획도Path = "";

            if (layer지적도 != null)
                layer지적도.RemoveAll();*/

            overlayPainter.RemoveAllOverlayShape();

            try
            {
                System.IO.Compression.ZipFile.ExtractToDirectory(strPath, strTempFolder);
                string strXMLPath = GetFilePath(strTempFolder, "xml");

                if (strXMLPath.Length == 0)
                {
                    DeleteFolder(strTempFolder);
                    return null;
                }

                ProjectData pData = ReadXML(strXMLPath, strTempFolder, overlayPainter);

                //if (!ReadXML(strXMLPath, strTempFolder, dxfControl, ref str수치지도Path, ref str토지이용계획도Path, ref visible지적도, ref layer지적도, frm수치지도, frm토지이용계획도, frmCalcCondition, overlayPainter))
                if (pData == null)
                {
                    DeleteFolder(strTempFolder);
                    return null;
                }

                return pData;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }

            return null;
        }

        private static ProjectData ReadXML(string strPath, string strTempFolder, Overlay.OverlayPainter overlayPainter)
        //private static bool ReadXML(string strPath, string strTempFolder, DXFViewer.DXFControl dxfControl, ref string str수치지도Path, ref string str토지이용계획도Path, ref bool visible지적도, ref DXFViewer.Layer layer지적도, DockingForm.FormDetailLayer frm수치지도, DockingForm.FormDetailLayer frm토지이용계획도, Popup.FormCalcCondition frmCalcCondition, Overlay.OverlayPainter overlayPainter)
        {
            XmlTextReader reader = null;
            bool stop = false;

            ProjectData pData = null;
            reader = new XmlTextReader(strPath);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "SoilMan", true) != 0)
                            throw new Exception("다른 형식의 파일입니다.");
                        else
                        {
                            pData = ReadSoilMan(reader, strTempFolder, overlayPainter);
                            //if (!ReadSoilMan(reader, strTempFolder, dxfControl, ref str수치지도Path, ref str토지이용계획도Path, ref visible지적도, ref layer지적도, frm수치지도, frm토지이용계획도, frmCalcCondition, overlayPainter))

                            if (pData == null)
                            {
                                reader.Close();
                                return null;
                            }
                            else
                                stop = true;
                        }

                        break;
                }

                if (stop)
                    break;
            }
    
            reader.Close();
            return pData;
        }

        private static ProjectData ReadSoilMan(XmlTextReader reader, string strTempFolder, Overlay.OverlayPainter overlayPainter)
        //private static bool ReadSoilMan(XmlTextReader reader, string strTempFolder, DXFViewer.DXFControl dxfControl, ref string str수치지도Path, ref string str토지이용계획도Path, ref bool visible지적도, ref DXFViewer.Layer layer지적도, DockingForm.FormDetailLayer frm수치지도, DockingForm.FormDetailLayer frm토지이용계획도, Popup.FormCalcCondition frmCalcCondition, Overlay.OverlayPainter overlayPainter)
        {
            bool stop = false;
            DXFViewer.Viewport viewport = null;

            List<DXFViewer.Layer> 수치지도Layers = null;
            List<DXFViewer.Layer> 토지이용계획도Layers = null;
            string str수치지도Path = "", str토지이용계획도Path = "", str지적도Path = "";
            bool visible지적도 = false, visible수치지도 = false, visible토지이용계획도 = false;
            Dictionary<string, ShapeAttrib> shapeAttribs = null;
            UnE.Geometry.Vertex2D v지적도TL = null, v지적도BR = null;
            UnE.Geometry.Vertex2D v수치지도TL = null, v수치지도BR = null;
            UnE.Geometry.Vertex2D v토지이용계획도TL = null, v토지이용계획도BR = null;
            Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea = null;
            TechType techType = TechType.None;
            Popup.SoilCleanCost cleanCost = null;
            long nInheritage = -1, nExistance = -1, nBio = -1;
            UnE.Geometry.Vertex2D vMoved = null;

            long nSelectRegion = 0;
            long nSelectWTP = 0;
            double dWTPYear = 0.0;
            double dRejectionRatio = 0.0;
            long nHousehold = 0;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Header", true) == 0)
                        {
                            viewport = ReadHeader(reader, out vMoved);

                            if (viewport == null)
                                return null;
                        }
                        else if (string.Compare(reader.Name, "Body", true) == 0)
                        {
                            shapeAttribs = ReadBody(reader, ref str지적도Path, ref str수치지도Path, ref str토지이용계획도Path, 
                                ref visible지적도, ref visible수치지도, ref visible토지이용계획도, overlayPainter, 
                                out 수치지도Layers, out 토지이용계획도Layers, out v지적도TL, out v지적도BR, out v수치지도TL,
                                out v수치지도BR, out v토지이용계획도TL, out v토지이용계획도BR, out dicLandTypeArea, out techType, 
                                out cleanCost, out nInheritage, out nExistance, out nBio, 
                                out nSelectRegion, out nSelectWTP, out dWTPYear, out dRejectionRatio, out nHousehold);
                            //if (!ReadBody(reader, ref str수치지도Path, ref str토지이용계획도Path, ref visible지적도, ref layer지적도, frm수치지도, frm토지이용계획도, frmCalcCondition, overlayPainter, out 수치지도Layers, out 토지이용계획도Layers))

                            if (shapeAttribs == null)
                                return null;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            ProjectData data = new ProjectData();

            data.TempFolderPath = strTempFolder;
            data.ShapeAttribs = shapeAttribs;
            data.Viewport = viewport;
            data.Visible수치지도 = visible수치지도;
            data.Visible지적도 = visible지적도;
            data.Visible토지이용계획도 = visible토지이용계획도;
            data.수치지도Layers = 수치지도Layers;
            data.토지이용계획도Layers = 토지이용계획도Layers;
            data.수치지도Path = str수치지도Path;
            data.지적도Path = str지적도Path;
            data.토지이용계획도Path = str토지이용계획도Path;
            data.지적도TL = v지적도TL;
            data.지적도BR = v지적도BR;
            data.수치지도TL = v수치지도TL;
            data.수치지도BR = v수치지도BR;
            data.토지이용계획도TL = v토지이용계획도TL;
            data.토지이용계획도BR = v토지이용계획도BR;

            int nOverlayCount = overlayPainter.GetOverlayShapeCount();

            for (int i = 0; i < nOverlayCount;i++ )
            {
                Overlay.OverlayShape overlay = overlayPainter.GetOverlayShape(i);
                data.OverlayShapes.Add(overlay);
            }

            overlayPainter.RemoveAllOverlayShape();

            data.LandTypeAreas = dicLandTypeArea;
            data.SelectedTechType = techType;
            data.SoilCleanCost = cleanCost;
            data.Inheritage = nInheritage;
            data.Existance = nExistance;
            data.Bio = nBio;
            data.MovedVertex = vMoved;

            data.SelectRegion = nSelectRegion;
            data.SelectWTP = nSelectWTP;
            data.WTPYear = dWTPYear;
            data.RejectionRatio = dRejectionRatio;
            data.Household = nHousehold;

            return data;
        }

        private static Dictionary<string, ShapeAttrib> ReadBody(
            XmlTextReader reader, ref string str지적도Path, ref string str수치지도Path, ref string str토지이용계획도Path, 
            ref bool visible지적도, ref bool visible수치지도, ref bool visible토지이용계획도, Overlay.OverlayPainter overlayPainter,
            out List<DXFViewer.Layer> 수치지도Layers, out List<DXFViewer.Layer> 토지이용계획도Layers, 
            out UnE.Geometry.Vertex2D v지적도TL, out UnE.Geometry.Vertex2D v지적도BR, out UnE.Geometry.Vertex2D v수치지도TL, 
            out UnE.Geometry.Vertex2D v수치지도BR, out UnE.Geometry.Vertex2D v토지이용계획도TL, out UnE.Geometry.Vertex2D v토지이용계획도BR,
            out Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea, out TechType techType, out Popup.SoilCleanCost cleanCost,
            out long nInheritage, out long nExistance, out long nBio,
            out long nSelectRegion, out long nSelectWTP, out double dWTPYear, out double dRejectionRatio, out long nHousehold)
        //private static bool ReadBody(XmlTextReader reader, ref string str수치지도Path, ref string str토지이용계획도Path, ref bool visible지적도, ref DXFViewer.Layer layer지적도, DockingForm.FormDetailLayer frm수치지도, DockingForm.FormDetailLayer frm토지이용계획도, Popup.FormCalcCondition frmCalcCondition, Overlay.OverlayPainter overlayPainter, out List<DXFViewer.Layer> 수치지도Layers, out List<DXFViewer.Layer> 토지이용계획도Layers)
        {
            bool stop = false;
            double minX = 0.0, minY = 0.0, maxX= 0.0, maxY = 0.0;
            Dictionary<string, ShapeAttrib> shapeAttribs = null;

            수치지도Layers = 토지이용계획도Layers = null;
            v지적도TL = v지적도BR = v수치지도TL = v수치지도BR = v토지이용계획도TL = v토지이용계획도BR = null;
            dicLandTypeArea = null;
            techType = TechType.None;
            cleanCost = null;
            nInheritage = nExistance = nBio = -1;
            nSelectRegion = nSelectWTP = nHousehold = 0;
            dRejectionRatio = dWTPYear = 0.0;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "수치지도", true) == 0)
                        {
                            수치지도Layers = ReadDXF(reader, ref str수치지도Path, ref minX, ref minY, ref maxX, ref maxY, out visible수치지도);

                            if (수치지도Layers == null)
                                return null;
                            else
                            {
                                v수치지도TL = new UnE.Geometry.Vertex2D(minX, maxY);
                                v수치지도BR = new UnE.Geometry.Vertex2D(maxX, minY);
                            }
                        }
                        else if (string.Compare(reader.Name, "토지이용계획도", true) == 0)
                        {
                            토지이용계획도Layers = ReadDXF(reader, ref str토지이용계획도Path, ref minX, ref minY, ref maxX, ref maxY, out visible토지이용계획도);

                            if (토지이용계획도Layers == null)
                                return null;
                            else
                            {
                                v토지이용계획도TL = new UnE.Geometry.Vertex2D(minX, maxY);
                                v토지이용계획도BR = new UnE.Geometry.Vertex2D(maxX, minY);
                            }
                        }
                        else if (string.Compare(reader.Name, "지적도", true) == 0)
                        {
                            shapeAttribs = Read지적도(reader, ref str지적도Path, ref minX, ref minY, ref maxX, ref maxY, out visible지적도);

                            if (shapeAttribs == null)
                                return null;
                            else
                            {
                                v지적도TL = new UnE.Geometry.Vertex2D(minX, maxY);
                                v지적도BR = new UnE.Geometry.Vertex2D(maxX, minY);
                            }
                        }
                        else if (string.Compare(reader.Name, "Input", true) == 0)
                        {
                            if (!ReadInput(reader, overlayPainter, out dicLandTypeArea, out techType, out cleanCost, out nInheritage, out nExistance, out nBio,out nSelectRegion, out nSelectWTP, out dWTPYear, out dRejectionRatio, out nHousehold))
                                return null;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return shapeAttribs;
        }

        private static bool ReadInput(
            XmlTextReader reader, Overlay.OverlayPainter overlayPainter, out Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea,
            out TechType techType, out Popup.SoilCleanCost cleanCost, out long nInheritage, out long nExistance, out long nBio,
            out long nSelectRegion, out long nSelectWTP, out double dWTPYear, out double dRejectionRatio, out long nHousehold)
        {
            bool stop = false;
            dicLandTypeArea = null;
            cleanCost = null;
            techType = TechType.None;
            // 음수값이면 설정되지 않은 상태이다.
            nInheritage = nExistance = nBio = -1;
            nSelectRegion = nSelectWTP = nHousehold = 0;
            dRejectionRatio = dWTPYear = 0.0;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Overlays", true) == 0)
                        {
                            if (!ReadOverlays(reader, overlayPainter))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "Areas", true) == 0)
                        {
                            if (!ReadInputAreas(reader, ref dicLandTypeArea))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "PublicCost", true) == 0)
                        {
                            if (!ReadInputPublicCost(reader, ref dicLandTypeArea))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "Condition", true) == 0)
                        {
                            cleanCost = ReadInputCondition(reader, ref techType);

                            if (cleanCost == null)
                                return false;
                        }
                        else if (string.Compare(reader.Name, "Value", true) == 0)
                        {
                            if (!ReadInputValue(reader, ref nInheritage, ref nExistance, ref nBio, ref nSelectRegion, ref nSelectWTP, ref dWTPYear, ref dRejectionRatio,ref nHousehold))
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

            /*if (dicLandTypeArea != null)
            {
                Popup.FormConfirmArea frm = frmCalcCondition.GetConfirmArea();

                if (frm != null)
                    frm.SetLandTypeInfo(dicLandTypeArea);
            }

            if (techType != TechType.None)
            {
                Popup.FormInputCondition frm = frmCalcCondition.GetInputCondition();

                if (frm != null)
                {
                    if (cleanCost != null)
                        frm.SetSoilCleanCost(techType, cleanCost);

                    frm.SelectedTechType = techType;
                    frm.InheritanceValue = nInheritage;
                    frm.ExistanceValue = nExistance;
                    frm.BioValue = nBio;
                }
            }*/

            return true;
        }

        private static bool ReadInputValue(XmlTextReader reader, 
            ref long nInheritage, ref long nExistance, ref long nBio,
            ref long nSelectRegion, ref long nSelectWTP, ref double dWTPYear, ref double dRejectionRatio, ref long nHousehold)
        {
            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Inheritage", true) == 0)
                        {
                            if (!ReadLong(reader, ref nInheritage, "Inheritage가", "Inheritage는"))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "Existance", true) == 0)
                        {
                            if (!ReadLong(reader, ref nExistance, "Existance가", "Existance는"))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "Bio", true) == 0)
                        {
                            if (!ReadLong(reader, ref nBio, "Bio가", "Bio는"))
                                return false;
                        }
                        if (string.Compare(reader.Name, "SelectRegion", true) == 0)
                        {
                            if (!ReadLong(reader, ref nSelectRegion, "SelectRegion가", "SelectRegion는"))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "SelectWTP", true) == 0)
                        {
                            if (!ReadLong(reader, ref nSelectWTP, "SelectWTP가", "SelectWTP는"))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "WTPYear", true) == 0)
                        {
                            if (!ReadDouble(reader, ref dWTPYear, "WTPYear가", "WTPYear는"))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "RejectionRatio", true) == 0)
                        {
                            if (!ReadDouble(reader, ref dRejectionRatio, "RejectionRatio가", "RejectionRatio는"))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "Household", true) == 0)
                        {
                            if (!ReadLong(reader, ref nHousehold, "Household가", "Household는"))
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

            return true;
        }

        private static Popup.SoilCleanCost ReadInputCondition(XmlTextReader reader, ref TechType techType)
        {
            bool stop = false;
            string strTechnique = "";
            long nCost = 0;
            int nPeriod = 0;
            double discount = 0.0;

            Popup.SoilCleanCost cleanCost = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Selectedtechnique", true) == 0)
                        {
                            if (!ReadText(reader, ref strTechnique))
                                return null;
                            else
                            {
                                techType = Popup.FormInputCondition.ToTechType(strTechnique);
                            }
                        }
                        else if (string.Compare(reader.Name, "CleanCost", true) == 0)
                        {
                            if (!ReadLong(reader, ref nCost, "CleanCost가", "CleanCost는"))
                                return null;
                            else
                            {
                                if (cleanCost == null)
                                    cleanCost = new Popup.SoilCleanCost();

                                cleanCost.Cost = nCost;
                            }
                        }
                        else if (string.Compare(reader.Name, "Period", true) == 0)
                        {
                            if (!ReadInt(reader, ref nPeriod, "Period가", "Period는"))
                                return null;
                            else
                            {
                                if (cleanCost == null)
                                    cleanCost = new Popup.SoilCleanCost();

                                cleanCost.Period = nPeriod;
                            }
                        }
                        else if (string.Compare(reader.Name, "Discount", true) == 0)
                        {
                            if (!ReadDouble(reader, ref discount, "Discount가", "Discount는"))
                                return null;
                            else
                            {
                                if (cleanCost == null)
                                    cleanCost = new Popup.SoilCleanCost();

                                cleanCost.Discount = discount;
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

            if (techType == TechType.None)
                return null;

            return cleanCost;
        }

        private static bool ReadInputPublicCost(XmlTextReader reader, ref Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea)
        {
            bool stop = false;
            double data = 0.0;

            if (dicLandTypeArea == null)
                dicLandTypeArea = new Dictionary<LandType, Overlay.AreaNCost>();

            Overlay.AreaNCost cost = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "General", true) == 0)
                        {
                            if (!ReadDouble(reader, ref data, "General이", "General은"))
                                return false;
                            else
                            {
                                if (!dicLandTypeArea.TryGetValue(LandType.General, out cost))
                                {
                                    cost = new Overlay.AreaNCost();
                                    dicLandTypeArea[LandType.General] = cost;
                                }

                                cost.Cost = data;
                            }
                        }
                        else if (string.Compare(reader.Name, "Field", true) == 0)
                        {
                            if (!ReadDouble(reader, ref data, "Field가", "Field는"))
                                return false;
                            else
                            {
                                if (!dicLandTypeArea.TryGetValue(LandType.Field, out cost))
                                {
                                    cost = new Overlay.AreaNCost();
                                    dicLandTypeArea[LandType.Field] = cost;
                                }

                                cost.Cost = data;
                            }
                        }
                        else if (string.Compare(reader.Name, "RiceField", true) == 0)
                        {
                            if (!ReadDouble(reader, ref data, "RiceField가", "RiceField는"))
                                return false;
                            else
                            {
                                if (!dicLandTypeArea.TryGetValue(LandType.RiceField, out cost))
                                {
                                    cost = new Overlay.AreaNCost();
                                    dicLandTypeArea[LandType.RiceField] = cost;
                                }

                                cost.Cost = data;
                            }
                        }
                        else if (string.Compare(reader.Name, "Mountain", true) == 0)
                        {
                            if (!ReadDouble(reader, ref data, "Mountain이", "Mountain은"))
                                return false;
                            else
                            {
                                if (!dicLandTypeArea.TryGetValue(LandType.Mountain, out cost))
                                {
                                    cost = new Overlay.AreaNCost();
                                    dicLandTypeArea[LandType.Mountain] = cost;
                                }

                                cost.Cost = data;
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

            return true;
        }

        private static bool ReadInputAreas(XmlTextReader reader, ref Dictionary<LandType, Overlay.AreaNCost> dicLandTypeArea)
        {
            bool stop = false;
            double data = 0.0;

            if (dicLandTypeArea == null)
                dicLandTypeArea = new Dictionary<LandType, Overlay.AreaNCost>();

            Overlay.AreaNCost cost = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "General", true) == 0)
                        {
                            if (!ReadDouble(reader, ref data, "General이", "General은"))
                                return false;
                            else
                            {
                                if (!dicLandTypeArea.TryGetValue(LandType.General, out cost))
                                {
                                    cost = new Overlay.AreaNCost();
                                    dicLandTypeArea[LandType.General] = cost;
                                }

                                cost.Area = data;
                            }
                        }
                        else if (string.Compare(reader.Name, "Field", true) == 0)
                        {
                            if (!ReadDouble(reader, ref data, "Field가", "Field는"))
                                return false;
                            else
                            {
                                if (!dicLandTypeArea.TryGetValue(LandType.Field, out cost))
                                {
                                    cost = new Overlay.AreaNCost();
                                    dicLandTypeArea[LandType.Field] = cost;
                                }

                                cost.Area = data;
                            }
                        }
                        else if (string.Compare(reader.Name, "RiceField", true) == 0)
                        {
                            if (!ReadDouble(reader, ref data, "RiceField가", "RiceField는"))
                                return false;
                            else
                            {
                                if (!dicLandTypeArea.TryGetValue(LandType.RiceField, out cost))
                                {
                                    cost = new Overlay.AreaNCost();
                                    dicLandTypeArea[LandType.RiceField] = cost;
                                }

                                cost.Area = data;
                            }
                        }
                        else if (string.Compare(reader.Name, "Mountain", true) == 0)
                        {
                            if (!ReadDouble(reader, ref data, "Mountain이", "Mountain은"))
                                return false;
                            else
                            {
                                if (!dicLandTypeArea.TryGetValue(LandType.Mountain, out cost))
                                {
                                    cost = new Overlay.AreaNCost();
                                    dicLandTypeArea[LandType.Mountain] = cost;
                                }

                                cost.Area = data;
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

            return true;
        }

        private static bool ReadOverlays(XmlTextReader reader, Overlay.OverlayPainter overlayPainter)
        {
            bool stop = false;
            
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Circle", true) == 0)
                        {
                            Overlay.OverlayCircle circle = ReadCircle(reader, overlayPainter);

                            if (circle == null)
                                return false;
                            else
                                overlayPainter.AddOverlayShape(circle);
                        }
                        else if (string.Compare(reader.Name, "Rectangle", true) == 0)
                        {
                            Overlay.OverlayRectangle rect = ReadRectangle(reader, overlayPainter);

                            if (rect == null)
                                return false;
                            else
                                overlayPainter.AddOverlayShape(rect);
                        }
                        else if (string.Compare(reader.Name, "PolyLine", true) == 0)
                        {
                            Overlay.OverlayPolyLine polyline = ReadPolyLine(reader, overlayPainter);

                            if (polyline == null)
                                return false;
                            else
                                overlayPainter.AddOverlayShape(polyline);
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

            return true;
        }

        private static Overlay.OverlayPolyLine ReadPolyLine(XmlTextReader reader, Overlay.OverlayPainter overlayPainter)
        {
            bool stop = false;
            Overlay.OverlayPolyLine polyline = new Overlay.OverlayPolyLine(overlayPainter);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Vertices", true) == 0)
                        {
                            if (!ReadPolyLine(reader, polyline))
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

            return polyline;
        }

        private static bool ReadPolyLine(XmlTextReader reader, Overlay.OverlayPolyLine polyline)
        {
            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Vertex2F", true) == 0)
                        {
                            UnE.Geometry.Vertex2F vertex = ReadVertex2F(reader);

                            if (vertex == null)
                                return false;
                            else
                                polyline.AddPoint(vertex.x, vertex.y);
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

            if (polyline.GetPointCount() < 3)
                return false;

            polyline.IsClosed = true;
            return true;
        }

        private static Overlay.OverlayRectangle ReadRectangle(XmlTextReader reader, Overlay.OverlayPainter overlayPainter)
        {
            bool stop = false;
            UnE.Geometry.Vertex2F vPos = null;
            float fWidth = 0.0f, fHeight = 0.0f;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Position", true) == 0)
                        {
                            vPos = ReadRectPosition(reader, ref fWidth, ref fHeight);
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
                return null;

            Overlay.OverlayRectangle rect = new Overlay.OverlayRectangle(overlayPainter);
            rect.Position = vPos;
            rect.Width = fWidth;
            rect.Height = fHeight;

            return rect;
        }

        private static UnE.Geometry.Vertex2F ReadRectPosition(XmlTextReader reader, ref float fWidth, ref float fHeight)
        {
            bool stop = false;
            UnE.Geometry.Vertex2F vPos = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Vertex2F", true) == 0)
                        {
                            vPos = ReadVertex2F(reader);

                            if (vPos == null)
                                return null;
                        }
                        else if (string.Compare(reader.Name, "Width", true) == 0)
                        {
                            if (!ReadFloat(reader, ref fWidth, "Width가", "Width는"))
                                return null;
                        }
                        else if (string.Compare(reader.Name, "Height", true) == 0)
                        {
                            if (!ReadFloat(reader, ref fHeight, "Height가", "Height는"))
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

            return vPos;
        }

        private static Overlay.OverlayCircle ReadCircle(XmlTextReader reader, Overlay.OverlayPainter overlayPainter)
        {
            bool stop = false;
            UnE.Geometry.Vertex2F vCenter = null;
            float fRadius = 0.0f;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Center", true) == 0)
                        {
                            vCenter = ReadVertexF(reader);
                        }
                        else if (string.Compare(reader.Name, "Radius", true) == 0)
                        {
                            if (!ReadFloat(reader, ref fRadius, "Radius가", "Radius는"))
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

            if (vCenter == null || fRadius <= 0.0f)
                return null;

            Overlay.OverlayCircle circle = new Overlay.OverlayCircle(overlayPainter);
            circle.Center = vCenter;
            circle.Radius = fRadius;

            return circle;
        }

        private static Dictionary<string, ShapeAttrib> Read지적도(XmlTextReader reader, ref string str지적도Path, ref double minX, ref double minY, ref double maxX, ref double maxY, out bool visibleLayer)
        {
            bool stop = false;
            bool isEmpty = reader.IsEmptyElement;

            visibleLayer = true;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "visible", true) == 0)
                {
                    try
                    {
                        string strValue = reader.Value.ToString();

                        if (string.Compare(strValue, "true", true) == 0)
                            visibleLayer = true;
                        else if (string.Compare(strValue, "false", true) == 0)
                            visibleLayer = false;
                        else
                            visibleLayer = int.Parse(strValue) == 0 ? false : true;
                    }
                    catch (Exception)
                    {
                        string strError = string.Format("Line Number {0}, visible은 true, false, 0 또는 1로 표현되어야만 합니다.", reader.LineNumber);
                        throw new Exception(strError);
                    }
                }
            }

            Dictionary<string, ShapeAttrib> shapeAttribs = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "FileName", true) == 0)
                        {
                            if (!ReadText(reader, ref str지적도Path))
                                return null;
                        }
                        else if (string.Compare(reader.Name, "Boundary", true) == 0)
                        {
                            if (!ReadBoundary(reader, ref minX, ref minY, ref maxX, ref maxY))
                                return null;
                        }
                        else if (string.Compare(reader.Name, "Attribs", true) == 0)
                        {
                            shapeAttribs = ReadShapeAttribs(reader);
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

            return shapeAttribs;
        }

        private static Dictionary<string, ShapeAttrib> ReadShapeAttribs(XmlTextReader reader)
        {
            bool stop = false;

            Dictionary<string, ShapeAttrib> shapeAttribs = new Dictionary<string, ShapeAttrib>();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Attrib", true) == 0)
                        {
                            ShapeAttrib attrib = ReadShapeAttrib(reader);

                            if (attrib == null)
                                return null;
                            else
                                shapeAttribs[attrib.PNU] = attrib;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return shapeAttribs;
        }

        private static ShapeAttrib ReadShapeAttrib(XmlTextReader reader)
        {
            bool stop = false;
            ShapeAttrib attrib = new ShapeAttrib();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "PNU", true) == 0)
                        {
                            string strPNU = "";

                            if (!ReadText(reader, ref strPNU))
                                return null;
                            else
                                attrib.PNU = strPNU;
                        }
                        else if (string.Compare(reader.Name, "Area", true) == 0)
                        {
                            double dArea = 0.0;

                            if (!ReadDouble(reader, ref dArea, "Area가", "Area는"))
                                return null;
                            else
                                attrib.Area = dArea;
                        }
                        else if (string.Compare(reader.Name, "Cost", true) == 0)
                        {
                            double dCost = 0.0;

                            if (!ReadDouble(reader, ref dCost, "Cost가", "Cost는"))
                                return null;
                            else
                                attrib.Cost = dCost;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return attrib;
        }

        private static List<DXFViewer.Layer> ReadDXF(XmlTextReader reader, ref string strDXFPath, ref double minX, ref double minY, ref double maxX, ref double maxY, out bool visibleLayer)
        {
            bool stop = false;
            bool isEmpty = reader.IsEmptyElement;

            visibleLayer = true;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "visible", true) == 0)
                {
                    try
                    {
                        string strValue = reader.Value.ToString();

                        if (string.Compare(strValue, "true", true) == 0)
                            visibleLayer = true;
                        else if (string.Compare(strValue, "false", true) == 0)
                            visibleLayer = false;
                        else
                            visibleLayer = int.Parse(strValue) == 0 ? false : true;
                    }
                    catch (Exception)
                    {
                        string strError = string.Format("Line Number {0}, visible은 true, false, 0 또는 1로 표현되어야만 합니다.", reader.LineNumber);
                        throw new Exception(strError);
                    }
                }
            }

            List<DXFViewer.Layer> layers = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "FileName", true) == 0)
                        {
                            if (!ReadText(reader, ref strDXFPath))
                                return null;
                        }
                        else if (string.Compare(reader.Name, "Boundary", true) == 0)
                        {
                            if (!ReadBoundary(reader, ref minX, ref minY, ref maxX, ref maxY))
                                return null;
                        }
                        else if (string.Compare(reader.Name, "Layers", true) == 0)
                        {
                            layers = ReadLayers(reader);
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return layers;
        }

        private static List<DXFViewer.Layer> ReadLayers(XmlTextReader reader)
        {
            bool stop = false;

            List<DXFViewer.Layer> layers = new List<DXFViewer.Layer>();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Layer", true) == 0)
                        {
                            DXFViewer.Layer layer = ReadLayer(reader);

                            if (layer == null)
                                return null;
                            else
                                layers.Add(layer);
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return layers;
        }

        private static DXFViewer.Layer ReadLayer(XmlTextReader reader)
        {
            bool stop = false;
            DXFViewer.Layer layer = new DXFViewer.Layer(null);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Visible", true) == 0)
                        {
                            bool visible = true;

                            if (!ReadBoolean(reader, ref visible, "Visible이", "Visible은"))
                                return null;
                            else
                                layer.Hidden = !visible;
                        }
                        else if (string.Compare(reader.Name, "Color", true) == 0)
                        {
                            System.Drawing.Color color = new System.Drawing.Color();

                            if (!ReadColor(reader, ref color))
                                return null;
                            else
                                layer.LineColor = color;
                        }
                        else if (string.Compare(reader.Name, "Name", true) == 0)
                        {
                            string strName = "";

                            if (!ReadText(reader, ref strName))
                                return null;
                            else
                                layer.LayerName = strName;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return layer;
        }

        private static bool ReadColor(XmlTextReader reader, ref System.Drawing.Color color)
        {
            bool stop = false;
            byte a = 0, r = 0, g = 0, b = 0;
            bool findR = false, findG = false, findB = false, findA = false;

            bool isEmpty = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "a", true) == 0)
                {
                    if (!byte.TryParse(reader.Value.ToString(), out a))
                    {
                        string strError = "Line Number " + reader.LineNumber.ToString() + ", A값은 0~255사이의 정수이어야 합니다.";
                        throw new Exception(strError);
                    }
                    else
                        findA = true;
                }
                else if (string.Compare(reader.Name, "r", true) == 0)
                {
                    if (!byte.TryParse(reader.Value.ToString(), out r))
                    {
                        string strError = "Line Number " + reader.LineNumber.ToString() + ", R값은 0~255사이의 정수이어야 합니다.";
                        throw new Exception(strError);
                    }
                    else
                        findR = true;
                }
                else if (string.Compare(reader.Name, "g", true) == 0)
                {
                    if (!byte.TryParse(reader.Value.ToString(), out g))
                    {
                        string strError = "Line Number " + reader.LineNumber.ToString() + ", G값은 0~255사이의 정수이어야 합니다.";
                        throw new Exception(strError);
                    }
                    else
                        findG = true;
                }
                else if (string.Compare(reader.Name, "b", true) == 0)
                {
                    if (!byte.TryParse(reader.Value.ToString(), out b))
                    {
                        string strError = "Line Number " + reader.LineNumber.ToString() + ", B값은 0~255사이의 정수이어야 합니다.";
                        throw new Exception(strError);
                    }
                    else
                        findB = true;
                }
            }

            if (!isEmpty)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
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

            if (!findA)
            {
                string strError = "Line Number " + reader.LineNumber.ToString() + ", Color에 R값이 존재하지 않습니다.";
                throw new Exception(strError);
            }
            else if (!findR)
            {
                string strError = "Line Number " + reader.LineNumber.ToString() + ", Color에 R값이 존재하지 않습니다.";
                throw new Exception(strError);
            }
            else if (!findG)
            {
                string strError = "Line Number " + reader.LineNumber.ToString() + ", Color에 G값이 존재하지 않습니다.";
                throw new Exception(strError);
            }
            else if (!findB)
            {
                string strError = "Line Number " + reader.LineNumber.ToString() + ", Color에 B값이 존재하지 않습니다.";
                throw new Exception(strError);
            }

            color = System.Drawing.Color.FromArgb((int)r, (int)g, (int)b);
            return true;
        }

        private static bool ReadBoundary(XmlTextReader reader, ref double minX, ref double minY, ref double maxX, ref double maxY)
        {
            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "MinX", true) == 0)
                        {
                            if (!ReadDouble(reader, ref minX, "MinX가", "MinX는"))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "MinY", true) == 0)
                        {
                            if (!ReadDouble(reader, ref minX, "MinY가", "MinY는"))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "MaxX", true) == 0)
                        {
                            if (!ReadDouble(reader, ref minX, "MaxX가", "MaxX는"))
                                return false;
                        }
                        else if (string.Compare(reader.Name, "MaxY", true) == 0)
                        {
                            if (!ReadDouble(reader, ref minX, "MaxY가", "MaxY는"))
                                return false;
                        }
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

        private static DXFViewer.Viewport ReadHeader(XmlTextReader reader, out UnE.Geometry.Vertex2D vMoved)
        {
            bool stop = false;
            DXFViewer.Viewport viewport = null;
            vMoved = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Version", true) == 0)
                        {
                            string strVersion = "";

                            if (!ReadText(reader, ref strVersion))
                                return null;
                        }
                        else if (string.Compare(reader.Name, "Viewport", true) == 0)
                        {
                            viewport = ReadViewport(reader);
                        }
                        else if (string.Compare(reader.Name, "MovedVertex", true) == 0)
                        {
                            vMoved = ReadVertexD(reader);
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            if (vMoved == null)
                return null;

            return viewport;
        }

        private static DXFViewer.Viewport ReadViewport(XmlTextReader reader)
        {
            bool stop = false;
            float data = 0.0f;
            DXFViewer.Viewport viewport = new DXFViewer.Viewport();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "F11", true) == 0)
                        {
                            if (!ReadFloat(reader, ref data, "F11이", "F11은"))
                                return null;
                            else
                                viewport.F11 = data;
                        }
                        else if (string.Compare(reader.Name, "F12", true) == 0)
                        {
                            if (!ReadFloat(reader, ref data, "F12가", "F12는"))
                                return null;
                            else
                                viewport.F12 = data;
                        }
                        else if (string.Compare(reader.Name, "F21", true) == 0)
                        {
                            if (!ReadFloat(reader, ref data, "F21이", "F21은"))
                                return null;
                            else
                                viewport.F21 = data;
                        }
                        else if (string.Compare(reader.Name, "F22", true) == 0)
                        {
                            if (!ReadFloat(reader, ref data, "F22는", "F22는"))
                                return null;
                            else
                                viewport.F22 = data;
                        }
                        else if (string.Compare(reader.Name, "FDx", true) == 0)
                        {
                            if (!ReadFloat(reader, ref data, "FDx는", "FDx는"))
                                return null;
                            else
                                viewport.FDx = data;
                        }
                        else if (string.Compare(reader.Name, "FDy", true) == 0)
                        {
                            if (!ReadFloat(reader, ref data, "FDy는", "FDy는"))
                                return null;
                            else
                                viewport.FDy = data;
                        }
                        else if (string.Compare(reader.Name, "TopLeft", true) == 0)
                        {
                            UnE.Geometry.Vertex2D vertex = ReadVertexD(reader);

                            if (vertex == null)
                                return null;
                            else
                                viewport.TopLeft = vertex;
                        }
                        else if (string.Compare(reader.Name, "BottomLeft", true) == 0)
                        {
                            UnE.Geometry.Vertex2D vertex = ReadVertexD(reader);

                            if (vertex == null)
                                return null;
                            else
                                viewport.BottomLeft = vertex;
                        }
                        else if (string.Compare(reader.Name, "BottomRight", true) == 0)
                        {
                            UnE.Geometry.Vertex2D vertex = ReadVertexD(reader);

                            if (vertex == null)
                                return null;
                            else
                                viewport.BottomRight = vertex;
                        }
                        else if (string.Compare(reader.Name, "Weight", true) == 0)
                        {
                            double dWeight = 0.0;

                            if (!ReadDouble(reader, ref dWeight, "Weight가", "Weight는"))
                                return null;
                            else
                                viewport.Weight = dWeight;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return viewport;
        }

        private static UnE.Geometry.Vertex2D ReadVertexD(XmlTextReader reader)
        {
            bool stop = false;
            UnE.Geometry.Vertex2D vertex = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Vertex2D", true) == 0)
                        {
                            vertex = ReadVertex2D(reader);
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return vertex;
        }

        private static UnE.Geometry.Vertex2F ReadVertexF(XmlTextReader reader)
        {
            bool stop = false;
            UnE.Geometry.Vertex2F vertex = null;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "Vertex2F", true) == 0)
                        {
                            vertex = ReadVertex2F(reader);
                        }
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return vertex;
        }

        private static UnE.Geometry.Vertex2D ReadVertex2D(XmlTextReader reader)
        {
            bool stop = false;
            double x = 0.0, y = 0.0;
            bool findX = false, findY = false;

            bool isEmpty = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "x", true) == 0)
                {
                    if (!double.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out x))
                    {
                        string strError = "Line Number " + reader.LineNumber.ToString() + ", x값은 숫자이어야 합니다.";
                        throw new Exception(strError);
                    }
                    else
                        findX = true;
                }
                else if (string.Compare(reader.Name, "y", true) == 0)
                {
                    if (!double.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out y))
                    {
                        string strError = "Line Number " + reader.LineNumber.ToString() + ", y값은 숫자이어야 합니다.";
                        throw new Exception(strError);
                    }
                    else
                        findY = true;
                }
            }

            if (!isEmpty)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
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

            if (!findX)
            {
                string strError = "Line Number " + reader.LineNumber.ToString() + ", x값이 존재하지 않습니다.";
                throw new Exception(strError);
            }
            else if (!findY)
            {
                string strError = "Line Number " + reader.LineNumber.ToString() + ", y값이 존재하지 않습니다.";
                throw new Exception(strError);
            }

            return new UnE.Geometry.Vertex2D(x, y);
        }

        private static UnE.Geometry.Vertex2F ReadVertex2F(XmlTextReader reader)
        {
            bool stop = false;
            float x = 0.0f, y = 0.0f;
            bool findX = false, findY = false;

            bool isEmpty = reader.IsEmptyElement;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "x", true) == 0)
                {
                    if (!float.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out x))
                    {
                        string strError = "Line Number " + reader.LineNumber.ToString() + ", x값은 숫자이어야 합니다.";
                        throw new Exception(strError);
                    }
                    else
                        findX = true;
                }
                else if (string.Compare(reader.Name, "y", true) == 0)
                {
                    if (!float.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out y))
                    {
                        string strError = "Line Number " + reader.LineNumber.ToString() + ", y값은 숫자이어야 합니다.";
                        throw new Exception(strError);
                    }
                    else
                        findY = true;
                }
            }

            if (!isEmpty)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
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

            if (!findX)
            {
                string strError = "Line Number " + reader.LineNumber.ToString() + ", x값이 존재하지 않습니다.";
                throw new Exception(strError);
            }
            else if (!findY)
            {
                string strError = "Line Number " + reader.LineNumber.ToString() + ", y값이 존재하지 않습니다.";
                throw new Exception(strError);
            }

            return new UnE.Geometry.Vertex2F(x, y);
        }

        private static void PassElement(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        private static bool ReadText(XmlTextReader reader, ref string strText, bool allowEmpty = false)
        {
            if (reader.IsEmptyElement)
            {
                strText = "";
                return allowEmpty;
            }

            if (!ReadElementText(reader, ref strText))
                strText = "";

            return true;
        }

        private static bool ReadInt(XmlTextReader reader, ref int nData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                string strError = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
                throw new Exception(strError);
            }

            if (!int.TryParse(strText, System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out nData))
            {
                string strError = string.Format("Line Number {0}, {1} 정수값이어야만 합니다.", reader.LineNumber, strMessage2);
                throw new Exception(strError);
            }

            return true;
        }

        private static bool ReadLong(XmlTextReader reader, ref long nData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                string strError = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
                throw new Exception(strError);
            }

            if (!long.TryParse(strText, System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out nData))
            {
                string strError = string.Format("Line Number {0}, {1} 정수값이어야만 합니다.", reader.LineNumber, strMessage2);
                throw new Exception(strError);
            }

            return true;
        }

        private static bool ReadDouble(XmlTextReader reader, ref double dData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                string strError = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
                throw new Exception(strError);
            }

            if (!double.TryParse(strText, System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out dData))
            {
                string strError = string.Format("Line Number {0}, {1} 숫자이어야만 합니다.", reader.LineNumber, strMessage2);
                throw new Exception(strError);
            }

            return true;
        }

        private static bool ReadFloat(XmlTextReader reader, ref float fData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                string strError = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
                throw new Exception(strError);
            }

            if (!float.TryParse(strText, System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out fData))
            {
                string strError = string.Format("Line Number {0}, {1} 숫자이어야만 합니다.", reader.LineNumber, strMessage2);
                throw new Exception(strError);
            }

            return true;
        }

        private static bool ReadBoolean(XmlTextReader reader, ref bool bData, string strMessage1, string strMessage2)
        {
            string strText = "";

            if (!ReadElementText(reader, ref strText))
            {
                string strError = string.Format("Line Number {0}, {1} 비어있습니다.", reader.LineNumber, strMessage1);
                throw new Exception(strError);
            }

            try
            {
                if (string.Compare(strText, "true", true) == 0)
                    bData = true;
                else if (string.Compare(strText, "false", true) == 0)
                    bData = false;
                else
                    bData = int.Parse(strText) == 0 ? false : true;
            }
            catch (Exception)
            {
                string strError = string.Format("Line Number {0}, {1} true, false, 0 또는 1로 표현되어야만 합니다.", reader.LineNumber, strMessage2);
                throw new Exception(strError);
            }

            return true;
        }

        private static bool ReadElementText(XmlTextReader reader, ref string strText)
        {
            bool stop = false, readText = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strText = reader.Value;
                        readText = true;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return readText;
        }

        private static string GetFilePath(string strFolderPath, string strExtName)
        {
            string[] arrFiles = Directory.GetFiles(strFolderPath);

            foreach (string strFile in arrFiles)
            {
                int nIndex = strFile.LastIndexOf('.');

                if (nIndex < 0)
                    continue;

                string strExt = strFile.Substring(nIndex + 1);

                if (string.Compare(strExt, strExtName, true) == 0)
                    return strFile;
            }

            return "";
        }

        public static bool Save(string strPath, DXFViewer.DXFControl dxfControl, string strProjectPath, string str수치지도Path, string str토지이용계획도Path, bool visible지적도, DXFViewer.Layer layer지적도, DockingForm.FormDetailLayer frm수치지도, DockingForm.FormDetailLayer frm토지이용계획도, Popup.FormCalcCondition frmCalcCondition, Overlay.OverlayPainter overlayPainter)
        {
            // XML 파일은 일단 임시경로에 저장한다.
            DateTime dtNow = DateTime.Now;
            string strNow = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            string strTempFolder = Path.GetTempPath() + strNow;
            string strTempPath = strTempFolder + "\\temp.xml";

            string str지적도Path = "";

            try
            {
                CreateFolder(strTempFolder);

                libShapeFile.ShapeInfo shapeInfo = null;

                if (layer지적도 != null)
                    str지적도Path = SaveUSH(layer지적도, strTempFolder, out shapeInfo);

                string strProjectTempFolder = SaveXML(strTempPath, dxfControl, strProjectPath, str지적도Path, ref str수치지도Path, ref str토지이용계획도Path, shapeInfo, visible지적도, layer지적도, frm수치지도, frm토지이용계획도, frmCalcCondition, overlayPainter);

                if (strProjectTempFolder != null)
                {
                    if (str수치지도Path.Length > 0)
                    {
                        int nIndex = str수치지도Path.LastIndexOf('\\');

                        if (nIndex >= 0)
                            File.Copy(str수치지도Path, strTempFolder + "\\" + str수치지도Path.Substring(nIndex + 1));
                    }

                    if (str토지이용계획도Path.Length > 0)
                    {
                        int nIndex = str토지이용계획도Path.LastIndexOf('\\');

                        if (nIndex >= 0)
                            File.Copy(str토지이용계획도Path, strTempFolder + "\\" + str토지이용계획도Path.Substring(nIndex + 1));
                    }

                    if (strProjectTempFolder.Length > 0)
                        DeleteFolder(strProjectTempFolder);

                    return PostSave(strTempPath, strPath);
                }

                DeleteFolder(strTempFolder);
                return strProjectTempFolder != null;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }

            return false;
        }

        // strFolderPath가 존재하면 해당 폴더의 파일 및 Sub 폴더를 모두 지운다.
        // strFolderPath가 존재하지 않으면 생성한다.
        private static void CreateFolder(string strFolderPath)
        {
            if (Directory.Exists(strFolderPath))
            {
                string[] arrFiles = Directory.GetFiles(strFolderPath);

                foreach (string strFile in arrFiles)
                {
                    File.Delete(strFile);
                }

                string[] arrFolders = Directory.GetDirectories(strFolderPath);

                foreach (string strFolder in arrFolders)
                {
                    DeleteFolder(strFolder);
                }
            }
            else
                Directory.CreateDirectory(strFolderPath);
        }

        // 임시 폴더에 저장된 파일을 압축 및 암호화하여 사용자가 원래 입력한 경로에 복사해 넣는다.
        private static bool PostSave(string strTempPath, string strPath)
        {
            int nIndex = strTempPath.LastIndexOf('\\');

            if (nIndex < 0)
                return false;

            string strTempFolder = strTempPath.Substring(0, nIndex);
            string strTempZipFolder = strTempFolder + "Temp";
            string strTempZipPath = strTempZipFolder + "\\SoilManTemp.zip";

            CreateFolder(strTempZipFolder);

            System.IO.Compression.ZipFile.CreateFromDirectory(strTempFolder, strTempZipPath);

            if (File.Exists(strPath))
                File.Delete(strPath);
            
            File.Copy(strTempZipPath, strPath);

            DeleteFolder(strTempZipFolder);
            DeleteFolder(strTempFolder);
            return true;
        }

        public static void DeleteFolder(string strFolderPath)
        {
            string[] arrFiles = Directory.GetFiles(strFolderPath);

            foreach (string strFile in arrFiles)
            {
                File.Delete(strFile);
            }

            string[] arrFolders = Directory.GetDirectories(strFolderPath);

            foreach (string strFolder in arrFolders)
            {
                DeleteFolder(strFolder);
            }

            Directory.Delete(strFolderPath);
        }

        private static string SaveXML(string strPath, DXFViewer.DXFControl dxfControl, string strProjectPath, string str지적도Path, ref string str수치지도Path, ref string str토지이용계획도Path, libShapeFile.ShapeInfo shapeInfo, bool visible지적도, DXFViewer.Layer layer지적도, DockingForm.FormDetailLayer frm수치지도, DockingForm.FormDetailLayer frm토지이용계획도, Popup.FormCalcCondition frmCalcCondition, Overlay.OverlayPainter overlayPainter)
        {
            XmlTextWriter writer = null;
            string strProjectTempFolder = null;

            try
            {
                writer = new XmlTextWriter(strPath, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();

                writer.WriteStartElement("SoilMan");
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }

            if (!SaveHeader(writer, dxfControl))
            {
                writer.Close();
                return null;
            }

            strProjectTempFolder = SaveBody(writer, strProjectPath, str지적도Path, ref str수치지도Path, ref str토지이용계획도Path, shapeInfo, visible지적도, layer지적도, frm수치지도, frm토지이용계획도, frmCalcCondition, overlayPainter);

            if (strProjectTempFolder == null)
            {
                writer.Close();
                return null;
            }

            writer.Close();
            return strProjectTempFolder;
        }

        private static string CopyDXFToTemp(string strProjectPath, ref string strDXFFile, string strTempFolder)
        {
            if (strTempFolder.Length == 0)
            {
                DateTime dtNow = DateTime.Now;
                string strNow = string.Format("{0}{1:00}{2:00}{3:00}{4:00}_{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                strTempFolder = Path.GetTempPath() + strNow;

                System.IO.Compression.ZipFile.ExtractToDirectory(strProjectPath, strTempFolder);
            }

            int nIndex1 = strDXFFile.LastIndexOf('\\');
            int nIndex2 = strDXFFile.LastIndexOf('.');
            string strDXFFileName = strDXFFile.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1) + ".dxf";

            string[] arrFiles = Directory.GetFiles(strTempFolder);

            foreach (string strFile in arrFiles)
            {
                int nIndex = strFile.LastIndexOf('\\');

                if (nIndex < 0)
                    continue;

                string strFileName = strFile.Substring(nIndex + 1);

                if (string.Compare(strFileName, strDXFFileName, true) == 0)
                {
                    strDXFFile = strFile;
                    break;
                }
            }

            return strTempFolder;
        }

        private static string SaveBody(XmlTextWriter writer, string strProjectPath, string str지적도Path, ref string str수치지도Path, ref string str토지이용계획도Path, libShapeFile.ShapeInfo shapeInfo, bool visible지적도, DXFViewer.Layer layer지적도, DockingForm.FormDetailLayer frm수치지도, DockingForm.FormDetailLayer frm토지이용계획도, Popup.FormCalcCondition frmCalcCondition, Overlay.OverlayPainter overlayPainter)
        {
            writer.WriteStartElement("Body");

            str수치지도Path = str수치지도Path.ToLower();
            str토지이용계획도Path = str토지이용계획도Path.ToLower();

            string strTempFolder = "";

            // DXF 파일들이 prj에 포함되어 있을경우 압축을 풀어
            if (str수치지도Path.EndsWith(".prj"))
                strTempFolder = CopyDXFToTemp(strProjectPath, ref str수치지도Path, strTempFolder);

            if (str토지이용계획도Path.EndsWith(".prj"))
                strTempFolder = CopyDXFToTemp(strProjectPath, ref str토지이용계획도Path, strTempFolder);

            if (frm수치지도.Layers.Count > 0 && str수치지도Path.Length > 0)
            {
                if (!WriteDXF(writer, "수치지도", str수치지도Path, frm수치지도))
                    return null;
            }

            if (frm토지이용계획도.Layers.Count > 0 && str토지이용계획도Path.Length > 0)
            {
                if (!WriteDXF(writer, "토지이용계획도", str토지이용계획도Path, frm토지이용계획도))
                    return null;
            }

            if (str지적도Path.Length > 0)
            {
                if (!Write지적도(writer, str지적도Path, shapeInfo, visible지적도, layer지적도))
                    return null;
            }

            if (!WriteInput(writer, frmCalcCondition, overlayPainter))
                return null;

            writer.WriteEndElement();
            return strTempFolder;
        }

        private static bool WriteInput(XmlTextWriter writer, Popup.FormCalcCondition frmCalcCondition, Overlay.OverlayPainter overlayPainter)
        {
            writer.WriteStartElement("Input");

            if (!WriteOverlay(writer, overlayPainter))
                return false;

            if (!WriteArea(writer, frmCalcCondition))
                return false;

            if (!WritePublicCost(writer, frmCalcCondition))
                return false;

            if (!WriteCondition(writer, frmCalcCondition))
                return false;

            if (!WriteValue(writer, frmCalcCondition))
                return false;

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteValue(XmlTextWriter writer, Popup.FormCalcCondition frmCalcCondition)
        {
            Popup.FormInputCondition frmCondition = frmCalcCondition.GetInputCondition();

            if (frmCondition == null)
                return false;

            writer.WriteStartElement("Value");

            writer.WriteStartElement("SelectRegion");
            writer.WriteString(frmCondition.SelectedRegion.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("SelectWTP");
            writer.WriteString(((int)frmCondition.SelectedWTPType).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("WTPYear");
            writer.WriteString(((long)(frmCondition.InputWTPYear + 0.5)).ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("RejectionRatio");
            writer.WriteString(((long)(frmCondition.InputRejectionRatio + 0.5)).ToString());
            writer.WriteEndElement();
            writer.WriteStartElement("Household");
            writer.WriteString(((long)(frmCondition.InputHousehold + 0.5)).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Inheritage");
            writer.WriteString(((long)(frmCondition.InheritanceValue + 0.5)).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Existance");
            writer.WriteString(((long)(frmCondition.ExistanceValue + 0.5)).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Bio");
            writer.WriteString(((long)(frmCondition.BioValue + 0.5)).ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteCondition(XmlTextWriter writer, Popup.FormCalcCondition frmCalcCondition)
        {
            Popup.FormInputCondition frmCondition = frmCalcCondition.GetInputCondition();

            if (frmCondition == null)
                return false;

            TechType techType = frmCondition.SelectedTechType;
            Popup.SoilCleanCost cleanCost = frmCondition.GetSoilCleanCost(techType);

            if (cleanCost == null)
                return true;

            writer.WriteStartElement("Condition");

            string strTechType = Popup.FormInputCondition.TechTypeToString(techType);

            if (strTechType.Length == 0)
                return false;

            writer.WriteStartElement("Selectedtechnique");
            writer.WriteString(strTechType);
            writer.WriteEndElement();

            writer.WriteStartElement("CleanCost");
            writer.WriteString(((long)(cleanCost.Cost + 0.5)).ToString());
            writer.WriteEndElement();
            
            writer.WriteStartElement("ExtraCost");
            writer.WriteString(((long)(cleanCost.ExtraCost + 0.5)).ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Period");
            writer.WriteString(cleanCost.Period.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Discount");
            writer.WriteString(cleanCost.Discount.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private static bool WritePublicCost(XmlTextWriter writer, Popup.FormCalcCondition frmCalcCondition)
        {
            Popup.FormConfirmArea frmArea = frmCalcCondition.GetConfirmArea();

            if (frmArea == null)
                return false;

            Dictionary<LandType, Overlay.AreaNCost> dicAreas = frmArea.LandTypeAreas;

            if (dicAreas == null)
                return true;

            writer.WriteStartElement("PublicCost");

            foreach (KeyValuePair<LandType, Overlay.AreaNCost> pair in dicAreas)
            {
                if (pair.Key == LandType.General)
                {
                    writer.WriteStartElement("General");
                }
                else if (pair.Key == LandType.Field)
                {
                    writer.WriteStartElement("Field");
                }
                else if (pair.Key == LandType.RiceField)
                {
                    writer.WriteStartElement("RiceField");
                }
                else if (pair.Key == LandType.Mountain)
                {
                    writer.WriteStartElement("Mountain");
                }
                else
                    return false;

                writer.WriteString(((long)(pair.Value.Cost + 0.5)).ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteArea(XmlTextWriter writer, Popup.FormCalcCondition frmCalcCondition)
        {
            Popup.FormConfirmArea frmArea = frmCalcCondition.GetConfirmArea();

            if (frmArea == null)
                return false;

            Dictionary<LandType, Overlay.AreaNCost> dicAreas = frmArea.LandTypeAreas;

            if (dicAreas == null)
                return true;

            writer.WriteStartElement("Areas");

            foreach (KeyValuePair<LandType, Overlay.AreaNCost> pair in dicAreas)
            {
                if (pair.Key == LandType.General)
                {
                    writer.WriteStartElement("General");
                }
                else if (pair.Key == LandType.Field)
                {
                    writer.WriteStartElement("Field");
                }
                else if (pair.Key == LandType.RiceField)
                {
                    writer.WriteStartElement("RiceField");
                }
                else if (pair.Key == LandType.Mountain)
                {
                    writer.WriteStartElement("Mountain");
                }
                else
                    return false;

                writer.WriteString(pair.Value.Area.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteOverlay(XmlTextWriter writer, Overlay.OverlayPainter overlayPainter)
        {
            writer.WriteStartElement("Overlays");

            int nOverlayCount = overlayPainter.GetOverlayShapeCount();

            for (int i = 0; i < nOverlayCount;i++ )
            {
                Overlay.OverlayShape overlay = overlayPainter.GetOverlayShape(i);

                if (overlay is Overlay.OverlayCircle)
                {
                    if (!WriteOverlayCircle(writer, (Overlay.OverlayCircle)overlay))
                        return false;
                }
                else if (overlay is Overlay.OverlayRectangle)
                {
                    if (!WriteOverlayRectangle(writer, (Overlay.OverlayRectangle)overlay))
                        return false;
                }
                else if (overlay is Overlay.OverlayPolyLine)
                {
                    if (!WriteOverlayPolyLine(writer, (Overlay.OverlayPolyLine)overlay))
                        return false;
                }
            }

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteOverlayPolyLine(XmlTextWriter writer, Overlay.OverlayPolyLine polyline)
        {
            writer.WriteStartElement("PolyLine");

            writer.WriteStartElement("Vertices");

            int nPointCount = polyline.GetPointCount();

            for (int i = 0; i < nPointCount;i++ )
            {
                UnE.Geometry.Vertex2F vertex = polyline.GetPoint(i);

                if (vertex == null)
                    return false;

                WriteVertex2F(writer, vertex);
            }
            
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteOverlayRectangle(XmlTextWriter writer, Overlay.OverlayRectangle rect)
        {
            writer.WriteStartElement("Rectangle");

            writer.WriteStartElement("Position");
            
            WriteVertex2F(writer, rect.Position);

            writer.WriteStartElement("Width");
            writer.WriteString(rect.Width.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("Height");
            writer.WriteString(rect.Height.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteOverlayCircle(XmlTextWriter writer, Overlay.OverlayCircle circle)
        {
            writer.WriteStartElement("Circle");

            writer.WriteStartElement("Center");
            WriteVertex2F(writer, circle.Center);
            writer.WriteEndElement();

            writer.WriteStartElement("Radius");
            writer.WriteString(circle.Radius.ToString());
            writer.WriteEndElement();
            
            writer.WriteEndElement();
            return true;
        }

        private static void WriteVertex2F(XmlTextWriter writer, UnE.Geometry.Vertex2F vertex)
        {
            writer.WriteStartElement("Vertex2F");

            writer.WriteStartAttribute("x");
            writer.WriteString(vertex.x.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y");
            writer.WriteString(vertex.y.ToString());
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

        private static void WriteVertex2D(XmlTextWriter writer, UnE.Geometry.Vertex2D vertex)
        {
            writer.WriteStartElement("Vertex2D");

            writer.WriteStartAttribute("x");
            writer.WriteString(vertex.x.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y");
            writer.WriteString(vertex.y.ToString());
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

        private static bool Write지적도(XmlTextWriter writer, string strFilePath, libShapeFile.ShapeInfo shapeInfo, bool visible, DXFViewer.Layer layer)
        {
            int nIndex = strFilePath.LastIndexOf('\\');

            if (nIndex < 0)
                return false;

            string strFileName = strFilePath.Substring(nIndex + 1);

            writer.WriteStartElement("지적도");

            writer.WriteStartAttribute("visible");
            writer.WriteString(visible ? "true" : "false");
            writer.WriteEndAttribute();

            writer.WriteStartElement("FileName");
            writer.WriteString(strFileName);
            writer.WriteEndElement();

            // WriteBoundary(...)

            writer.WriteStartElement("Attribs");

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape is Drawing.Polygon)
                {
                    if (!WriteAttrib(writer, (Drawing.Polygon)shape, shapeInfo))
                        return false;
                }
                else if (shape is Drawing.PolygonList)
                {
                    Drawing.PolygonList polygonList = (Drawing.PolygonList)shape;
                    List<Drawing.Polygon> polygons = polygonList.GetPolygons(null);

                    foreach (Drawing.Polygon polygon in polygons)
                    {
                        if (!WriteAttrib(writer, polygon, shapeInfo))
                            return false;
                    }
                }
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteAttrib(XmlTextWriter writer, Drawing.Polygon polygon, libShapeFile.ShapeInfo shapeInfo)
        {
            Popup.PolygonInfo info = (Popup.PolygonInfo)polygon.Tag;

            writer.WriteStartElement("Attrib");

            writer.WriteStartElement("PNU");
            writer.WriteString(info.Code);
            writer.WriteEndElement();

            writer.WriteStartElement("Area");
            writer.WriteString(info.Area < 0.0 ? "0.0" : info.Area.ToString());
            writer.WriteEndElement();
        
            writer.WriteStartElement("Cost");
            writer.WriteString(info.Cost < 0.0 ? "0" : ((long)(info.Cost + 0.5)).ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteBoundary(XmlTextWriter writer, double minX, double minY, double maxX, double maxY)
        {
            writer.WriteStartElement("Boundary");

            writer.WriteStartElement("MinX");
            writer.WriteString(minX.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("MinY");
            writer.WriteString(minY.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("MaxX");
            writer.WriteString(maxX.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("MaxY");
            writer.WriteString(maxY.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteDXF(XmlTextWriter writer, string strElementName, string strFilePath, DockingForm.FormDetailLayer frm)
        {
            int nIndex = strFilePath.LastIndexOf('\\');

            if (nIndex < 0)
                return false;

            string strFileName = strFilePath.Substring(nIndex + 1);

            writer.WriteStartElement(strElementName);

            writer.WriteStartAttribute("visible");
            writer.WriteString(frm.Visible ? "true" : "false");
            writer.WriteEndAttribute();

            writer.WriteStartElement("FileName");
            writer.WriteString(strFileName);
            writer.WriteEndElement();

            // WriteBoundary(...)

            writer.WriteStartElement("Layers");

            foreach (DXFViewer.Layer layer in frm.Layers)
            {
                WriteLayer(writer, layer);
            }

            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private static bool WriteLayer(XmlTextWriter writer, DXFViewer.Layer layer)
        {
            writer.WriteStartElement("Layer");

            writer.WriteStartElement("Visible");
            writer.WriteString(layer.Hidden ? "false" : "true");
            writer.WriteEndElement();

            writer.WriteStartElement("Color");
            
            writer.WriteStartAttribute("a");
            writer.WriteString(((int)layer.LineColor.A).ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("r");
            writer.WriteString(((int)layer.LineColor.R).ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("g");
            writer.WriteString(((int)layer.LineColor.G).ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("b");
            writer.WriteString(((int)layer.LineColor.B).ToString());
            writer.WriteEndAttribute();

            writer.WriteEndElement();

            writer.WriteStartElement("Name");
            writer.WriteString(layer.LayerName);
            writer.WriteEndElement();

            writer.WriteEndElement();

            return true;
        }

        private static bool SaveHeader(XmlTextWriter writer, DXFViewer.DXFControl dxfControl)
        {
            writer.WriteStartElement("Header");

            writer.WriteStartElement("Version");
            writer.WriteString(m_strVersionName);
            writer.WriteEndElement();

            if (!WriteViewport(writer, dxfControl))
                return false;

            WriteMovedVertex(writer, dxfControl);

            writer.WriteEndElement();
            return true;
        }

        private static void WriteMovedVertex(XmlTextWriter writer, DXFViewer.DXFControl dxfControl)
        {
            writer.WriteStartElement("MovedVertex");
            WriteVertex2D(writer, dxfControl.MovedVertex);
            writer.WriteEndElement();
        }

        private static bool WriteViewport(XmlTextWriter writer, DXFViewer.DXFControl dxfControl)
        {
            DXFViewer.Viewport viewport = dxfControl.GetViewport();

            if (viewport == null)
                return false;

            writer.WriteStartElement("Viewport");                       

            writer.WriteStartElement("F11");
            writer.WriteString(viewport.F11.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("F12");
            writer.WriteString(viewport.F12.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("F21");
            writer.WriteString(viewport.F21.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("F22");
            writer.WriteString(viewport.F22.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("FDx");
            writer.WriteString(viewport.FDx.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("FDy");
            writer.WriteString(viewport.FDy.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement("TopLeft");
            WriteVertex2D(writer, viewport.TopLeft);
            writer.WriteEndElement();

            writer.WriteStartElement("BottomLeft");
            WriteVertex2D(writer, viewport.BottomLeft);
            writer.WriteEndElement();

            writer.WriteStartElement("BottomRight");
            WriteVertex2D(writer, viewport.BottomRight);
            writer.WriteEndElement();

            writer.WriteStartElement("Weight");
            writer.WriteString(viewport.Weight.ToString());
            writer.WriteEndElement();

            writer.WriteEndElement();
            return true;
        }

        private static string SaveUSH(DXFViewer.Layer layer, string strFolderPath, out libShapeFile.ShapeInfo shapeInfo)
        {
            shapeInfo = null;
            List<Drawing.Polygon> polygons = null;

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape is Drawing.Polygon)
                {
                    if (polygons == null)
                        polygons = new List<Drawing.Polygon>();

                    polygons.Add((Drawing.Polygon)shape);
                }
                else if (shape is Drawing.PolygonList)
                {
                    Drawing.PolygonList polygonList = (Drawing.PolygonList)shape;
                    return SaveUSH(polygonList.GetPolygons(null), strFolderPath, out shapeInfo);
                }
            }

            if (polygons != null)
                return SaveUSH(polygons, strFolderPath, out shapeInfo);

            return "";
        }

        private static string SaveUSH(List<Drawing.Polygon> polygons, string strFolderPath, out libShapeFile.ShapeInfo shapeInfo)
        {
            shapeInfo = GetShapeInfo(polygons);

            if (shapeInfo == null)
                return "";

            PolygonShapeList shapeList = new PolygonShapeList(polygons);
            libShapeFile.USHWriter writer = new libShapeFile.USHWriter();

            string strPath = strFolderPath + "\\temp.ush";

            if (writer.Write(strPath, shapeList, shapeInfo, libShapeFile.ShapeList.RealType.FLOAT, Encoding.UTF8))
                return strPath;

            return "";
        }

        private static libShapeFile.ShapeInfo GetShapeInfo(List<Drawing.Polygon> polygons)
        {
            foreach (Drawing.Polygon polygon in polygons)
            {
                return polygon.ShapeInfo;
            }

            return null;
        }
    }

    class PolygonShapeList : libShapeFile.ShapeList
    {
        private List<Drawing.Polygon> m_polygons = null;

        public override int ShapeCount
        {
            get
            {
                return m_polygons.Count;
            }
        }

        public override int ObjectType
        {
            get
            {
                return (int)libShapeFile.ShapeType.Polygon;
            }
        }

        public PolygonShapeList(List<Drawing.Polygon> polygons)
        {
            m_polygons = polygons;
        }

        protected override bool WritePolygons(System.IO.BinaryWriter writer, int nObjectType, libShapeFile.ShapeList.RealType realType)
        {
            writer.Write(nObjectType);

            foreach (Drawing.Polygon polygon in m_polygons)
            {
                writer.Write(polygon.MinX);
                writer.Write(polygon.MinY);
                writer.Write(polygon.MaxX);
                writer.Write(polygon.MaxY);

                int nSubPolygonCount = polygon.GetSubPolygonCount();
                writer.Write(nSubPolygonCount);

                for (int i = 0; i < nSubPolygonCount; i++)
                {
                    UnE.Geometry.PolygonF subPolygon = polygon.GetSubPolygon(i);

                    if (realType == RealType.FLOAT)
                        WriteVerticesF(writer, subPolygon.GetVertexList());
                    else
                        WriteVerticesD(writer, subPolygon.GetVertexList());
                }
            }

            return true;
        }

        public override bool WriteShapeAttrib(System.IO.BinaryWriter writer, libShapeFile.ShapeInfo shapeInfo, Encoding encoding)
        {
            int nFieldCount = shapeInfo.GetFieldCount();
            writer.Write(nFieldCount);

            for (int i = 0; i < nFieldCount; i++)
            {
                string strFieldName = shapeInfo.GetFieldName(i);
                libShapeFile.USHWriter.WriteString(writer, strFieldName, encoding);
            }

            foreach (Drawing.Polygon polygon in m_polygons)
            {
                for (int i = 0; i < nFieldCount; i++)
                {
                    string strFieldData = shapeInfo.GetFieldData(polygon.ID, i);
                    libShapeFile.USHWriter.WriteString(writer, strFieldData, encoding);
                }
            }

            return true;
        }
    }

    public class ShapeAttrib
    {
        private string m_strPNU = "";
        // 단위 : m²
        private double m_dArea = 0.0;
        // 단위 : 원
        private double m_dCost = 0.0;

        public string PNU
        {
            get { return m_strPNU; }
            set { m_strPNU = value; }
        }

        // 단위 : m²
        public double Area
        {
            get { return m_dArea; }
            set { m_dArea = value; }
        }

        // 단위 : 원
        public double Cost
        {
            get { return m_dCost; }
            set { m_dCost = value; }
        }
    }
}
