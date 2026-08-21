using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXFViewer;
using UnE.Geometry;
using System.IO;
using DBUtility2;
using System.Collections;

namespace SymbolMaker
{
    public partial class FormMain : Form
    {
        private List<POIData> m_pois = new List<POIData>();
        private bool m_isPOI = true;

        public FormMain()
        {
            InitializeComponent();
        }

        private void tsMenuOpenDXFFromLayer_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (panelDXF.OpenDXF(dlg.FileName))
                {
                    // 한글로 된 레이어를 제외하고 모두 없앤다.
                    RemoveNoKoreanLayers();

                    int nIndex = 0;

                    foreach (Layer layer in panelDXF.Layers)
                    {
                        POIData poi = POIFromLayer(layer);

                        if (poi != null)
                        {
                            poi.POIName = (nIndex++).ToString();
                            m_pois.Add(poi);
                        }
                    }

                    m_isPOI = true;
                    tsMenuExport1By1.Enabled = tsMenuExport.Enabled = m_pois.Count > 0;
                    panelDXF._Refresh();
                }
            }
        }

        private void tsMenuOpenDXF_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (panelDXF.OpenDXF(dlg.FileName))
                {
                    string strBig, strMiddle, strSmall;

                    if (GetSectionNames(dlg.FileName, out strBig, out strMiddle, out strSmall) == false)
                    {
                        MessageBox.Show("파일로부터 대분류/중분류/소분류 정보를 얻어올 수 없습니다.");
                        return;
                    }

                    // 한글로 된 레이어를 제외하고 모두 없앤다.
                    //RemoveNoKoreanLayers();
                    POIFromLayers(strBig, strMiddle, strSmall);

                    m_isPOI = true;
                    tsMenuExport1By1.Enabled = tsMenuExport.Enabled = m_pois.Count > 0;
                }
            }
        }

        private bool GetSectionNames(string strFilePath, out string strBig, out string strMiddle, out string strSmall)
        {
            strBig = strMiddle = strSmall = "";

            int nIndex = strFilePath.LastIndexOf('\\');

            if (nIndex >= 0)
            {
                strFilePath = strFilePath.Substring(nIndex + 1);
            }

            int nDotIndex = strFilePath.LastIndexOf('.');

            if (nDotIndex < 0)
                return false;

            strFilePath = strFilePath.Substring(0, nDotIndex);
            string[] tokens = strFilePath.Split('_');

            if (tokens.Count() != 3)
                return false;

            strBig = tokens[0].Trim();
            strMiddle = tokens[1].Trim();
            strSmall = tokens[2].Trim();
            return true;
        }

        private void POIFromLayers(string strBig, string strMiddle, string strSmall)
        {
            WebDBManager dbMgr = new WebDBManager(1);
            string strDBName = "UnE_BIM";

            int nParentID = GetPOITypeID(dbMgr, strDBName, strBig, strMiddle, strSmall);

            if (nParentID < 0)
                return;

            m_pois.Clear();

            foreach (Layer layer in panelDXF.Layers)
            {
                string strCode;
                int nID = GetPOITypeID(dbMgr, strDBName, nParentID, layer.LayerName, out strCode);

                if (nID < 0)
                    continue;

                if (strCode == null || strCode.Length == 0)
                {
                    System.Diagnostics.Trace.WriteLine(string.Format("POIType ID({0})는 Code가 정의되지 않았습니다.", nID));
                    continue;
                }

                POIData poi = POIFromLayer(layer);

                if (poi != null && (poi.Path.Path.Count > 0 || poi.Polygons.Count > 0 || poi.TextDatas.Count > 0))
                {
                    poi.POIName = strCode;
                    m_pois.Add(poi);
                }
            }
        }

        private int GetPOITypeID(WebDBManager dbMgr, string strDBName, int nParentID, string strTypeName, out string strCode)
        {
            strCode = "";

            string strSQL = string.Format("Select ID, Code from POIType where Name = '{0}' and IsGroup = 0 and ParentID = {1}", strTypeName, nParentID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, strDBName);

            if (arrResult == null || arrResult.Count < 2)
                return -1;

            int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            strCode = WebDBManager.GetStringField(arrResult[1]);

            if (strCode != null)
                strCode = strCode.Trim();

            return nID;
        }

        private int GetPOITypeID(WebDBManager dbMgr, string strDBName, string strBig, string strMiddle, string strSmall)
        {
            string strSQL = string.Format("Select ID from POIType where Name = '{0}' and IsGroup = 1 and ParentID is NULL", strBig);
            ArrayList arrResult = dbMgr.GetResultData(strSQL, strDBName);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            strSQL = string.Format("Select ID from POIType where Name = '{0}' and IsGroup = 1 and ParentID = {1}", strMiddle, nID);
            arrResult = dbMgr.GetResultData(strSQL, strDBName);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            strSQL = string.Format("Select ID from POIType where Name = '{0}' and IsGroup = 1 and ParentID = {1}", strSmall, nID);
            arrResult = dbMgr.GetResultData(strSQL, strDBName);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            return nID;
        }

        private POIData POIFromLayer(Layer layer)
        {
            POIData poi = new POIData(layer.LayerName);

            foreach (Shape shape in layer.Shapes)
            {
                if (shape.GetShapeType() == Shape.ShapeType.LINE)
                    AddLine(poi, (DXFViewer.Line)shape);
                else if (shape.GetShapeType() == Shape.ShapeType.POLYLINE)
                    AddPolyLine(poi, (DXFViewer.PolyLine)shape);
                else if (shape.GetShapeType() == Shape.ShapeType.ARC)
                    AddArc(poi, (DXFViewer.Arc)shape);
                else if (shape.GetShapeType() == Shape.ShapeType.EARC)
                    AddEArc(poi, (DXFViewer.EArc)shape);
                else if (shape.GetShapeType() == Shape.ShapeType.TEXT)
                    AddText(poi, (DXFViewer.Text)shape);
                else if (shape.GetShapeType() == Shape.ShapeType.HATCH)
                    AddHatch(poi, (DXFViewer.Hatch)shape);
            }

            poi.MoveToCenter();
            return poi;
        }

        private void AddHatch(POIData poi, Hatch hatch)
        {
            LinkedPath path = new LinkedPath();

            foreach (Hatch.PathItem item in hatch.PathItems)
            {
                if (item.DrawingType == Hatch.PathItem.DrawType.Line)
                {
                    path.AddLine(item.Line);
                }
                else if (item.DrawingType == Hatch.PathItem.DrawType.Arc)
                {
                    path.AddArc(item.Arc);
                }
                else if (item.DrawingType == Hatch.PathItem.DrawType.EArc)
                {
                    path.AddEArc(item.EArc);
                }
            }

            poi.AddPolygon(path);
        }

        private void AddText(POIData poi, Text text)
        {
            poi.AddText(text.Position, text.Title, text.Font.Size, text.Angle);
        }

        private void AddEArc(POIData poi, DXFViewer.EArc earc)
        {
            double dBeginAngle = UnE.Geometry.Math.DegToRad(earc.BeginAngle);
            double dEArcAngle = earc.IsEllipse ? UnE.Geometry.Math._2PI() : UnE.Geometry.Math.DegToRad(earc.EArcAngle);
            EArc2D _earc = new EArc2D(earc.TopLeft, earc.BottomLeft, earc.BottomRight, dBeginAngle, dEArcAngle, true);

            poi.Path.AddEArc(_earc);
        }

        private void AddArc(POIData poi, DXFViewer.Arc arc)
        {
            double dBeginAngle = UnE.Geometry.Math.DegToRad(arc.BeginAngle);
            double dArcAngle = arc.IsCircle ? UnE.Geometry.Math._2PI() : UnE.Geometry.Math.DegToRad(arc.ArcAngle);
            Arc2D _arc = new Arc2D(arc.Center, arc.Radius, dBeginAngle, dArcAngle, false);

            poi.Path.AddArc(_arc);
        }

        private void AddPolyLine(POIData poi, DXFViewer.PolyLine pLine)
        {
            int nVertexCount = pLine.GetVertexSize();

            if (nVertexCount < 2)
                return;

            PointF ptPrev = pLine.GetVertex(0);
            Vertex2D vPrev = new Vertex2D(ptPrev.X, ptPrev.Y);

            for (int i = 1; i < nVertexCount; i++)
            {
                PointF ptCurrent = pLine.GetVertex(i);
                Vertex2D vCurrent = new Vertex2D(ptCurrent.X, ptCurrent.Y);

                poi.Path.AddLine(vPrev, vCurrent);
                vPrev = vCurrent;
            }
        }

        private void AddLine(POIData poi, DXFViewer.Line line)
        {
            poi.Path.AddLine(line.Begin, line.End);
        }

        // "0"과 "FTEXT"만 제외하고 모두 사용한다.
        // 한글로 된 레이어를 제외하고 모두 없앤다.
        private void RemoveNoKoreanLayers()
        {
            List<Layer> removeLayers = new List<Layer>();
            foreach (Layer layer in panelDXF.Layers)
            {
                char ch = layer.LayerName.ElementAt(0);

                if ((int)ch <= 127)
                    removeLayers.Add(layer);
            }

            foreach (Layer layer in removeLayers)
            {
                panelDXF.Layers.Remove(layer);
            }
        }

        private void tsMenuExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            if (m_isPOI)
            {
                dlg.Filter = "POI Files|*.poi";
                dlg.FilterIndex = 0;
                dlg.Title = "POI 내보내기";
            }
            else
            {
                dlg.Filter = "배선 Files|*.wir";
                dlg.FilterIndex = 0;
                dlg.Title = "배선파일 내보내기";
            }

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                ExportPOI(dlg.FileName);
            }
        }

        private bool ExportPOI(POIData poi, BinaryWriter writer)
        {
            writer.Write(poi.POIName);

            if (WriteBoundary(poi.Path, writer) == false)
            {
                MessageBox.Show("파일을 생성할 수 없습니다.");
                writer.Close();
                return false;
            }

            if (WriteFill(poi, writer) == false)
            {
                MessageBox.Show("파일을 생성할 수 없습니다.");
                writer.Close();
                return false;
            }

            if (WriteText(poi, writer) == false)
            {
                MessageBox.Show("파일을 생성할 수 없습니다.");
                writer.Close();
                return false;
            }

            return true;
        }

        private void ExportPOIFolder(string strFolder)
        {
            foreach (POIData poi in m_pois)
            {
                string strExt = m_isPOI ? ".poi" : ".wir";
                string strFilePath = strFolder + "\\" + poi.POIName + strExt;
                FileStream fs = File.Open(strFilePath, FileMode.Create);
                BinaryWriter writer = new BinaryWriter(fs);

                if (ExportPOI(poi, writer) == false)
                    return;

                writer.Close();
            }

            if (m_isPOI)
                MessageBox.Show("POI 생성이 완료되었습니다.");
            else
                MessageBox.Show("배선파일 생성이 완료되었습니다.");
        }

        private void ExportPOI(string strFilePath)
        {
            FileStream fs = File.Open(strFilePath, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(fs);

            writer.Write(m_pois.Count);

            foreach (POIData poi in m_pois)
            {
                if (ExportPOI(poi, writer) == false)
                    return;
            }

            writer.Close();
            MessageBox.Show("파일이 생성되었습니다.");
        }

        private bool WriteText(POIData poi, BinaryWriter writer)
        {
            int nTextCount = poi.TextDatas.Count;
            writer.Write(nTextCount);

            for (int i = 0; i < nTextCount; i++)
            {
                TextData text = poi.TextDatas[i];

                writer.Write(text.Text);
                writer.Write(text.Position.x);
                writer.Write(text.Position.y);
                writer.Write(text.FontSize);
                writer.Write(text.TextAngle);
            }

            return true;
        }


        private bool WriteFill(POIData poi, BinaryWriter writer)
        {
            int nPolygonCount = poi.Polygons.Count;
            writer.Write(nPolygonCount);

            for (int i=0;i<nPolygonCount;i++)
            {
                LinkedPath polygon = poi.Polygons[i];

                if (!WriteBoundary(polygon, writer))
                    return false;
            }

            return true;
        }

        private bool WriteBoundary(LinkedPath path, BinaryWriter writer)
        {
            int nPathCount = path.Path.Count;
            writer.Write(nPathCount);

            for (int i=0;i<nPathCount;i++)
            {
                PathItem item = path.Path[i];

                if (item.DrawingType == PathItem.DrawType.Line)
                {
                    if (item.Line == null)
                        return false;

                    Vertex2D vBegin = item.Line.GetVertex(true);
                    Vertex2D vEnd = item.Line.GetVertex(false);

                    writer.Write((int)item.DrawingType);
                    writer.Write(vBegin.x);
                    writer.Write(vBegin.y);
                    writer.Write(vEnd.x);
                    writer.Write(vEnd.y);
                }
                else if (item.DrawingType == PathItem.DrawType.Arc)
                {
                    if (item.Arc == null)
                        return false;

                    Vertex2D vCenter = item.Arc.GetCenter();

                    writer.Write((int)item.DrawingType);
                    writer.Write(vCenter.x);
                    writer.Write(vCenter.y);
                    writer.Write(item.Arc.GetRadius());
                    writer.Write(item.Arc.GetBeginAngle());
                    writer.Write(item.Arc.GetAngle());
                    writer.Write(item.Arc.IsClockWise());
                }
                else if (item.DrawingType == PathItem.DrawType.EArc)
                {
                    if (item.EArc == null)
                        return false;

                    Vertex2D vTL = item.EArc.GetTL();
                    Vertex2D vBL = item.EArc.GetBL();
                    Vertex2D vBR = item.EArc.GetBR();

                    writer.Write((int)item.DrawingType);
                    writer.Write(vTL.x);
                    writer.Write(vTL.y);
                    writer.Write(vBL.x);
                    writer.Write(vBL.y);
                    writer.Write(vBR.x);
                    writer.Write(vBR.y);
                    writer.Write(item.EArc.GetBeginAngle());
                    writer.Write(item.EArc.GetAngle());
                    writer.Write(item.EArc.IsClockWise());
                }
            }

            return true;
        }

        private void panelDXF_MouseMove(object sender, MouseEventArgs e)
        {
            Vertex2D vPos = panelDXF.ScreenToGlobal(e.X, e.Y);
            labelCoord.Text = string.Format("좌표 : {0:F1}, {1:F2}", vPos.x - panelDXF.MovedVertex.x, vPos.y - panelDXF.MovedVertex.y);
        }

        private void tsMenuExport1By1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            if (m_isPOI)
                dlg.Description = "POI 내보내기 폴더 지정";
            else
                dlg.Description = "배선파일 내보내기 폴더 지정";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                ExportPOIFolder(dlg.SelectedPath);
            }
        }

        private void WireFromLayers()
        {
            m_pois.Clear();

            foreach (Layer layer in panelDXF.Layers)
            {
                POIData poi = POIFromLayer(layer);

                if (poi != null && (poi.Path.Path.Count > 0 || poi.Polygons.Count > 0 || poi.TextDatas.Count > 0))
                {
                    poi.POIName = layer.LayerName;
                    m_pois.Add(poi);
                }
            }
        }

        private void tsMenuOpenDXF4Wire_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (panelDXF.OpenDXF(dlg.FileName))
                {
                    WireFromLayers();

                    m_isPOI = false;
                    tsMenuExport1By1.Enabled = tsMenuExport.Enabled = m_pois.Count > 0;
                }
            }
        }
    }

    public class POIData
    {
        private string m_strPOIName = "";
        private LinkedPath m_path = new LinkedPath();
        private List<LinkedPath> m_listPolygons = new List<LinkedPath>();
        private List<TextData> m_listText = new List<TextData>();

        private Vertex2D m_vTL = null;
        private Vertex2D m_vBR = null;

        private static double m_dScaleSize = 200.0;

        public string POIName
        {
            get { return m_strPOIName; }
            set { m_strPOIName = value; }
        }

        public LinkedPath Path
        {
            get { return m_path; }
        }

        public List<LinkedPath> Polygons
        {
            get { return m_listPolygons; }
        }

        public List<TextData> TextDatas
        {
            get { return m_listText; }
        }

        public POIData()
        {
        }

        public POIData(string strName)
        {
            m_strPOIName = strName;
        }

        public void AddPolygon(LinkedPath polygon)
        {
            m_listPolygons.Add(polygon);
            LinkedPath.SetBoundary(ref m_vTL, ref m_vBR, polygon.BoundaryTL);
            LinkedPath.SetBoundary(ref m_vTL, ref m_vBR, polygon.BoundaryBR);
        }

        public void AddText(Vertex2D vPos, string strText, float fFontSize, double dTextAngle)
        {
            TextData data = new TextData();

            data.Position = vPos;
            data.Text = strText;
            data.FontSize = fFontSize;
            data.TextAngle = dTextAngle;

            m_listText.Add(data);

            LinkedPath.SetBoundary(ref m_vTL, ref m_vBR, vPos);
        }

        // POI 중심좌표를 원점으로 이동시킨다.
        public void MoveToCenter()
        {
            if (m_path.Path.Count > 0)
            {
                LinkedPath.SetBoundary(ref m_vTL, ref m_vBR, m_path.BoundaryTL);
                LinkedPath.SetBoundary(ref m_vTL, ref m_vBR, m_path.BoundaryBR);
            }

            if (m_vTL == null)
                return;

            Vertex2D vCenter = (m_vTL + m_vBR) / 2;

            m_path.Move(-vCenter.x, -vCenter.y);

            foreach (LinkedPath polygon in m_listPolygons)
            {
                polygon.Move(-vCenter.x, -vCenter.y);
            }

            foreach (TextData text in m_listText)
            {
                text.Move(-vCenter.x, -vCenter.y);
            }

            SetScale(m_dScaleSize);
        }

        private void SetScale(double dSize)
        {
            if (m_vTL == null)
                return;

            double dHeight = m_vTL.y - m_vBR.y;
            double dWidth = m_vBR.x - m_vTL.x;

            if (System.Math.Abs(dHeight) <= UnE.Geometry.Math.HALF_TOLERANCE() ||
                System.Math.Abs(dWidth) <= UnE.Geometry.Math.HALF_TOLERANCE())
                return;

            double dScale = 1.0;

            if (dWidth > dHeight)
            {
                dScale = dSize / dWidth;
            }
            else
            {
                dScale = dSize / dHeight;
            }

            m_path.SetScale(dScale);

            foreach (LinkedPath polygon in m_listPolygons)
            {
                polygon.SetScale(dScale);
            }

            foreach (TextData text in m_listText)
            {
                text.SetScale(dScale);
            }
        }
    }

    public class TextData
    {
        private Vertex2D m_vPos = new Vertex2D();
        private string m_strText = "";
        private float m_fFontSize = 10;
        // Degree
        private double m_dTextAngle = 0.0;

        public Vertex2D Position
        {
            get { return m_vPos; }
            set { m_vPos = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public float FontSize
        {
            get { return m_fFontSize; }
            set { m_fFontSize = value; }
        }

        // Degree
        public double TextAngle
        {
            get { return m_dTextAngle; }
            set { m_dTextAngle = value; }
        }

        public void Move(double x, double y)
        {
            m_vPos.x += x;
            m_vPos.y += y;
        }

        public void SetScale(double dScale)
        {
            m_vPos.x *= dScale;
            m_vPos.y *= dScale;
            m_fFontSize *= (float)dScale;
        }
    }

    public class LinkedPath
    {
        private List<PathItem> m_listPath = new List<PathItem>();
        private Vertex2D m_vTL = null;
        private Vertex2D m_vBR = null;

        public Vertex2D BoundaryTL
        {
            get { return m_vTL; }
        }

        public Vertex2D BoundaryBR
        {
            get { return m_vBR; }
        }

        public List<PathItem> Path
        {
            get { return m_listPath; }
        }

        public PathItem AddLine(Line2D line)
        {
            PathItem item = new PathItem(line);
            m_listPath.Add(item);
            SetBoundary(ref m_vTL, ref m_vBR, line.GetVertex(true));
            SetBoundary(ref m_vTL, ref m_vBR, line.GetVertex(false));
            return item;
        }

        public PathItem AddLine(Vertex2D vBegin, Vertex2D vEnd)
        {
            PathItem item = new PathItem(new Line2D(vBegin, vEnd));
            m_listPath.Add(item);
            SetBoundary(ref m_vTL, ref m_vBR, vBegin);
            SetBoundary(ref m_vTL, ref m_vBR, vEnd);
            return item;
        }

        public PathItem AddArc(Arc2D arc)
        {
            PathItem item = new PathItem(arc);
            m_listPath.Add(item);
            SetBoundary(ref m_vTL, ref m_vBR, arc.GetTL());
            SetBoundary(ref m_vTL, ref m_vBR, arc.GetBR());
            return item;
        }

        public PathItem AddEArc(EArc2D earc)
        {
            PathItem item = new PathItem(earc);
            m_listPath.Add(item);
            SetBoundary(ref m_vTL, ref m_vBR, earc.GetTL());
            SetBoundary(ref m_vTL, ref m_vBR, earc.GetBR());
            return item;
        }

        public void Move(double x, double y)
        {
            foreach (PathItem item in m_listPath)
            {
                item.Move(x, y);
            }

            if (m_vTL != null)
            {
                m_vTL.x += x;
                m_vTL.y += y;
                m_vBR.x += x;
                m_vBR.y += y;
            }
        }

        public void SetScale(double dScale)
        {
            foreach (PathItem item in m_listPath)
            {
                item.SetScale(dScale);
            }

            if (m_vTL != null)
            {
                m_vTL.x *= dScale;
                m_vTL.y *= dScale;
                m_vBR.x *= dScale;
                m_vBR.y *= dScale;
            }
        }

        public static void SetBoundary(ref Vertex2D vTL, ref Vertex2D vBR, Vertex2D vertex)
        {
            if (vTL == null)
            {
                vTL = new Vertex2D(vertex);
                vBR = new Vertex2D(vertex);
            }
            else
            {
                if (vTL.x > vertex.x)
                    vTL.x = vertex.x;
                if (vTL.y < vertex.y)
                    vTL.y = vertex.y;
                if (vBR.x < vertex.x)
                    vBR.x = vertex.x;
                if (vBR.y > vertex.y)
                    vBR.y = vertex.y;
            }
        }
    }

    public class PathItem
    {
        public enum DrawType { None = 0, Line, Arc, EArc };

        private Line2D m_line = null;
        private Arc2D m_arc = null;
        private EArc2D m_earc = null;

        private DrawType m_drawType = DrawType.None;

        public DrawType DrawingType
        {
            get { return m_drawType; }
        }

        public Line2D Line
        {
            get { return m_line; }
        }

        public Arc2D Arc
        {
            get { return m_arc; }
        }

        public EArc2D EArc
        {
            get { return m_earc; }
        }

        public PathItem()
        {
        }

        public PathItem(Line2D line)
        {
            SetLine(line);
        }

        public PathItem(Arc2D arc)
        {
            SetArc(arc);
        }

        public PathItem(EArc2D earc)
        {
            SetEArc(earc);
        }

        public void SetLine(Line2D line)
        {
            m_line = line;
            m_drawType = DrawType.Line;
        }

        public void SetArc(Arc2D arc)
        {
            m_arc = arc;
            m_drawType = DrawType.Arc;
        }

        public void SetEArc(EArc2D earc)
        {
            m_earc = earc;
            m_drawType = DrawType.EArc;
        }

        public void Move(double x, double y)
        {
            if (m_drawType == DrawType.Line)
            {
                Vertex2D vBegin = m_line.GetVertex(true);
                Vertex2D vEnd = m_line.GetVertex(false);

                vBegin.SetVertex(vBegin.x + x, vBegin.y + y);
                vEnd.SetVertex(vEnd.x + x, vEnd.y + y);
            }
            else if (m_drawType == DrawType.Arc)
            {
                Vertex2D vCenter = m_arc.GetCenter();
                m_arc.SetArc(new Vertex2D(vCenter.x + x, vCenter.y + y), m_arc.GetRadius(), m_arc.GetBeginAngle(), m_arc.GetAngle(), m_arc.IsClockWise());
            }
            else if (m_drawType == DrawType.EArc)
            {
                Vertex2D vTL = m_earc.GetTL();
                Vertex2D vBL = m_earc.GetBL();
                Vertex2D vBR = m_earc.GetBR();

                m_earc.SetEArc(new Vertex2D(vTL.x + x, vTL.y + y), new Vertex2D(vBL.x + x, vBL.y + y), new Vertex2D(vBR.x + x, vBR.y + y), m_earc.GetBeginAngle(), m_earc.GetAngle(), m_earc.IsClockWise());
            }
        }

        public void SetScale(double dScale)
        {
            if (m_drawType == DrawType.Line)
            {
                Vertex2D vBegin = m_line.GetVertex(true);
                Vertex2D vEnd = m_line.GetVertex(false);

                vBegin.SetVertex(vBegin.x * dScale, vBegin.y * dScale);
                vEnd.SetVertex(vEnd.x * dScale, vEnd.y * dScale);
            }
            else if (m_drawType == DrawType.Arc)
            {
                Vertex2D vCenter = m_arc.GetCenter() * dScale;
                m_arc.SetArc(vCenter, m_arc.GetRadius() * dScale, m_arc.GetBeginAngle(), m_arc.GetAngle(), m_arc.IsClockWise());
            }
            else if (m_drawType == DrawType.EArc)
            {
                Vertex2D vTL = m_earc.GetTL() * dScale;
                Vertex2D vBL = m_earc.GetBL() * dScale;
                Vertex2D vBR = m_earc.GetBR() * dScale;

                m_earc.SetEArc(vTL, vBL, vBR, m_earc.GetBeginAngle(), m_earc.GetAngle(), m_earc.IsClockWise());
            }
        }
    }
}
