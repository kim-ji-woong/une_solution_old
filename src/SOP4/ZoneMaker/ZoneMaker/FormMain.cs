using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace CCTVLocation
{
    public partial class FormMain : Form
    {
        private bool m_closeApplication = false;
        private string m_strDXFFilePath = "";
        private DBUtility.WebDBManager m_dbMgr = new DBUtility.WebDBManager("SOP4");
        private List<Zone> m_zones = new List<Zone>();

        private string m_strZoneLayerName = "Zone";
        private string m_strOutdoorZoneLayerName = "OutdoorZone";
        private float m_fRectSize = 100.0f;

        // m_nSnapDistance Pixel 이하이면 Snap이 잡힌다.
        private int m_nSnapDistance = 10;

        private FormZoneList m_frmZoneList = new FormZoneList();
        private string m_strDataFilePath = "";

        private bool m_transparentZone = false;

        private static FormMain m_instance = null;

        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public bool CloseApplication
        {
            get { return m_closeApplication; }
            set { m_closeApplication = value; }
        }

        public string DXFFilePath
        {
            get { return m_strDXFFilePath; }
        }

        public UnE.Geometry.Vertex2D MovedVertex
        {
            get { return dxfControl1.MovedVertex; }
        }

        public bool TransparentZone
        {
            get { return m_transparentZone; }
            set { m_transparentZone = value; }
        }

        public FormMain(string strDataFilePath)
        {
            m_instance = this;
            InitializeComponent();

            m_strDataFilePath = strDataFilePath;
        }

        private void tsMenuOpenDXF_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;
                LoadDXF(dlg.FileName);
                this.Cursor = Cursors.Arrow;
            }
        }

        private bool LoadDXF(string strFilePath)
        {
            dxfControl1.BackColor = Color.Black;
            bool isSuccess = dxfControl1.OpenDXF(strFilePath);
            toolStripStatusLabel1.Text = "";

            if (!isSuccess)
            {
                string strError = "DXF 불러오기가 실패하였습니다.";
                MessageBox.Show(strError);
                m_strDXFFilePath = "";
                return false;
            }
            else
            {
                this.Text = strFilePath;

                AddZoneHatch();

                SetViewport();
            }

            m_strDXFFilePath = strFilePath;
            return true;
        }

        private void AddZoneHatch()
        {
            DXFViewer.Layer layer = GetZoneLayer();

            foreach (Zone zone in m_zones)
            {
                foreach (UnE.Geometry.Polygon polygon in zone.Polygons)
                {
                    int nVertexCount = polygon.GetVertexCount();

                    if (nVertexCount < 3)
                        continue;

                    DXFViewer.Hatch hatch = new DXFViewer.Hatch();
                    hatch.SetPointSize(nVertexCount);

                    for (int i=0;i<nVertexCount;i++)
                    {
                        UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i) + dxfControl1.MovedVertex;
                        hatch.UpdatePoint(i, (float)vertex.x, (float)vertex.y);
                    }

                    layer.Add(hatch);
                }
            }
        }

        private DXFViewer.Layer GetZoneLayer()
        {
            foreach (DXFViewer.Layer layer in dxfControl1.Layers)
            {
                if (layer.LayerName == m_strZoneLayerName)
                    return layer;
            }

            DXFViewer.Layer layer2 = new DXFViewer.Layer(dxfControl1);
            layer2.LayerName = m_strZoneLayerName;
            layer2.LineColor = Color.Green;

            dxfControl1.Layers.Add(layer2);
            return layer2;
        }

        private DXFViewer.Layer GetOutdoorZoneLayer()
        {
            foreach (DXFViewer.Layer layer in dxfControl1.Layers)
            {
                if (layer.LayerName == m_strOutdoorZoneLayerName)
                    return layer;
            }

            DXFViewer.Layer layer2 = new DXFViewer.Layer(dxfControl1);
            layer2.LayerName = m_strOutdoorZoneLayerName;
            layer2.LineColor = Color.LightGoldenrodYellow;

            dxfControl1.Layers.Add(layer2);
            return layer2;
        }

        private void SetViewport(DXFViewer.Viewport viewport = null)
        {
            float minX = (float)dxfControl1.ObjectTL.x;
            float maxY = (float)dxfControl1.ObjectTL.y;
            float maxX = (float)dxfControl1.ObjectBR.x;
            float minY = (float)dxfControl1.ObjectBR.y;

            minX += (float)dxfControl1.MovedVertex.x;
            maxX += (float)dxfControl1.MovedVertex.x;
            minY += (float)dxfControl1.MovedVertex.y;
            maxY += (float)dxfControl1.MovedVertex.y;

            double cX = minX + (maxX - minX) / 2.0;
            double cY = minY + (Math.Max(maxY, minY) - Math.Min(maxY, minY)) / 2.0;

            float dx = maxX - minX;
            float dy = Math.Max(maxY, minY) - Math.Min(maxY, minY);

            UnE.Geometry.Vertex2D vCenter = new UnE.Geometry.Vertex2D(cX, cY);
            UnE.Geometry.Vertex2D vTL = new UnE.Geometry.Vertex2D(minX, minY);
            UnE.Geometry.Vertex2D vBR = new UnE.Geometry.Vertex2D(maxX, maxY);
            UnE.Geometry.Vertex2D vBL = new UnE.Geometry.Vertex2D(minX, maxY);

            // Get Contorl Size
            int nWidth = dxfControl1.Size.Width;
            int nHeight = dxfControl1.Size.Height;

            double weight1 = nWidth * 0.85 / dx;
            double weight2 = nHeight * 0.85 / dy;
            double dViewportWeight = weight1 < weight2 ? weight1 : weight2;

            if (viewport == null)
            {
                DXFViewer.Viewport viewport2 = new DXFViewer.Viewport();
                viewport2.TopLeft = vTL;
                viewport2.BottomLeft = vBL;
                viewport2.BottomRight = vBR;
                viewport2.F11 = (float)dViewportWeight;
                viewport2.F21 = 0.0f;
                viewport2.FDx = minX;
                viewport2.F12 = 0.0f;

                if (dxfControl1.DownToTop())
                {
                    viewport2.F22 = -(float)dViewportWeight;
                }
                else
                {
                    viewport2.F22 = (float)dViewportWeight;
                }

                viewport2.FDy = minY;
                viewport2.Weight = dViewportWeight;
                dxfControl1.SetViewportCenter(vCenter);
                dxfControl1.LoadViewport(viewport2, false);
            }
            else
            {

                double minX2 = viewport.TopLeft.x;
                double maxX2 = viewport.BottomRight.x;

                double minY2 = viewport.TopLeft.y;
                double maxY2 = viewport.BottomRight.y;

                double cX2 = minX + (maxX - minX) / 2.0;
                double cY2 = minY + (Math.Max(maxY, minY) - Math.Min(maxY, minY)) / 2.0;
                UnE.Geometry.Vertex2D vCenter2 = new UnE.Geometry.Vertex2D(cX2, cY2);

                dxfControl1.SetViewportCenter(vCenter2);
                dxfControl1.LoadViewport(viewport, false);
            }

            dxfControl1._Refresh();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            dxfControl1.DrawHatchFirst = false;
            dxfControl1.UseLastViewport = true;

            toolStripStatusLabel2.Text = "";

            //LoadZoneInfo();
        }

        private void LoadZoneDataFile()
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(m_strDataFilePath, Encoding.UTF8);

            Dictionary<int, Zone> dicBuildingZones = new Dictionary<int, Zone>();

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] arrTokens = strLine.Split('\t');

                if (arrTokens.Count() < 7)
                    continue;

                int nID, nSiteID, nBuildingID;

                if (!int.TryParse(arrTokens[0].Trim(), out nID) || !int.TryParse(arrTokens[2].Trim(), out nSiteID) || !int.TryParse(arrTokens[3].Trim(), out nBuildingID))
                    continue;

                if (nID < 0 || nSiteID != 2)
                    continue;

                if (nBuildingID > 0)
                {
                    // 하나의 빌딩에 대해서는 하나의 Zone만 읽으면 된다.
                    if (dicBuildingZones.ContainsKey(nBuildingID))
                        continue;
                }

                string strZoneName = arrTokens[1].Trim();
                string strBoundary = arrTokens[6].Trim();

                if (strBoundary.Length == 0 || strBoundary == "NULL")
                    continue;

                AddZone(nID, nBuildingID, strZoneName, strBoundary, dicBuildingZones);
            }

            reader.Close();
        }

        private void LoadZoneDB()
        {
            string strSQL = "Select ID, ZoneName, BuildingID, FloorIndex, AddFloor, Boundary from Zone where SiteID = 2";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            Dictionary<int, Zone> dicBuildingZones = new Dictionary<int, Zone>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strZoneName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                int nBuildingID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFloorIndex = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strAddFloor = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                string strBoundary = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");

                if (strBoundary.Length == 0 || strBoundary == "null")
                    continue;

                if (nID < 0)
                    continue;

                if (nBuildingID > 0)
                {
                    // 하나의 빌딩에 대해서는 하나의 Zone만 읽으면 된다.
                    if (dicBuildingZones.ContainsKey(nBuildingID))
                        continue;
                }

                AddZone(nID, nBuildingID, strZoneName, strBoundary, dicBuildingZones);
            }
        }

        private void AddZone(int nID, int nBuildingID, string strZoneName, string strBoundary, Dictionary<int, Zone> dicBuildingZones)
        {
            Zone zone = new Zone();

            zone.ZoneID = nID;
            zone.BuildingID = nBuildingID;
            zone.ZoneName = strZoneName;

            if (nBuildingID > 0)
                dicBuildingZones[nBuildingID] = zone;

            m_zones.Add(zone);

            string[] strCoords = strBoundary.Split('#');

            foreach (string strCoord in strCoords)
            {
                UnE.Geometry.Polygon polygon = ReadPolygon(strCoord.Trim());

                if (polygon != null)
                    zone.Polygons.Add(polygon);
            }
        }

        private void LoadZoneInfo()
        {
            if (m_strDataFilePath.Length > 0)
                LoadZoneDataFile();
            else
                LoadZoneDB();
        }

        private UnE.Geometry.Polygon ReadPolygon(string strBoundary)
        {
            string[] strCoords = strBoundary.Split(',');

            UnE.Geometry.Polygon polygon = new UnE.Geometry.Polygon();

            int nCoordCount = strCoords.Count();

            for (int j = 0; j < nCoordCount; j += 2)
            {
                try
                {
                    string strX = strCoords[j].Trim();
                    string strY = strCoords[j + 1].Trim();

                    double x, y;

                    if (double.TryParse(strX, out x) && double.TryParse(strY, out y))
                    {
                        UnE.Geometry.Vertex2D vertex = new UnE.Geometry.Vertex2D(x, y);
                        polygon.AddVertex(vertex);
                    }
                }
                catch (System.IndexOutOfRangeException e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    System.Diagnostics.Trace.WriteLine(j);
                    return null;
                }
            }

            return polygon;
        }

        private void dxfControl1_MouseMove(object sender, MouseEventArgs e)
        {
            UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(e.X, e.Y) - dxfControl1.MovedVertex;

            if (vertex != null)
                toolStripStatusLabel1.Text = string.Format("({0}, {1})", vertex.x, vertex.y);
        }

        private const int WM_CLOSE = 0x0010;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CLOSE:
                    FormMain.Instance.CloseApplication = true;
                    break;
            }

            base.WndProc(ref m);
        }

        private void tsMenuOpenDataFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Data Files|*.txt|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "Data 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_frmZoneList.OpenFile(dlg.FileName);
                dxfControl1._Refresh();
            }
        }

        private void tsMenuSaveDataFile_Click(object sender, EventArgs e)
        {    
        }

        private void dxfControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(e.X, e.Y);

                /*Zone zone = FindZone(vertex);

                if (zone != null)
                {
                    toolStripStatusLabel2.Text = zone.ZoneName;
                    return;
                }*/

                OutdoorZone outdoorZone = FindOutdoorZone(vertex);

                if (outdoorZone != null)
                {
                    toolStripStatusLabel2.Text = outdoorZone.ZoneName;
                    return;
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (!m_frmZoneList.Visible || !m_frmZoneList.EditMode)
                    return;

                OutdoorZone zone = m_frmZoneList.GetCurrentZone();

                if (zone == null)
                    return;

                DXFViewer.Layer layer = GetOutdoorZoneLayer();

                if (layer != null)
                {
                    UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(e.X, e.Y);

                    if ((ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        vertex = GetSnapVertex(layer, zone, vertex);

                        if (vertex == null)
                            return;
                    }

                    SetOutdoorZoneVertex(layer, zone, vertex, true);
                    dxfControl1._Refresh();
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (!m_frmZoneList.Visible || !m_frmZoneList.EditMode)
                    return;

                OutdoorZone zone = m_frmZoneList.GetCurrentZone();

                if (zone == null)
                    return;

                DXFViewer.Layer layer = GetOutdoorZoneLayer();

                if (layer != null)
                {
                    SetOutdoorZoneHatch(layer, zone);
                    dxfControl1._Refresh();
                }
            }

            toolStripStatusLabel2.Text = "";
        }

        private UnE.Geometry.Vertex2D GetSnapVertex(DXFViewer.Layer layer, OutdoorZone exceptZone, UnE.Geometry.Vertex2D vertex)
        {
            UnE.Geometry.Vertex2D v1 = dxfControl1.ScreenToGlobal(0, 0);
            UnE.Geometry.Vertex2D v2 = dxfControl1.ScreenToGlobal(m_nSnapDistance, 0);
            double dLimit = v1.GetDistance(v2);

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape == exceptZone.Hatch)
                    continue;

                if ((shape is EdgeLineHatch) == false)
                    continue;

                EdgeLineHatch hatch = (EdgeLineHatch)shape;

                int nPointSize = hatch.GetPointSize();
                float x, y;

                for (int i=0;i<nPointSize;i++)
                {
                    if (!hatch.GetPoint(i, out x, out y))
                    {
                        break;
                    }

                    UnE.Geometry.Vertex2D vertex2 = new UnE.Geometry.Vertex2D(x, y);
                    double distance = vertex.GetDistance(vertex2);

                    if (distance <= dLimit)
                        return vertex2;
                }
            }

            return null;
        }

        private void SetOutdoorZoneHatch(DXFViewer.Layer layer, OutdoorZone zone, bool selectShape = true, bool updateFormList = true)
        {
            int nVertexCount = zone.Vertices.Count;

            if (nVertexCount < 3)
                return;
            else
            {
                if (zone.PolyLine != null)
                {
                    layer.Remove(zone.PolyLine);
                    zone.PolyLine = null;
                }

                zone.Hatch = new EdgeLineHatch();

                zone.Hatch.SetPointSize(nVertexCount);
                zone.Hatch.SetOwner(dxfControl1);

                for (int i = 0; i < nVertexCount; i++)
                {
                    UnE.Geometry.Vertex2D v = zone.Vertices[i] + dxfControl1.MovedVertex;
                    zone.Hatch.UpdatePoint(i, (float)v.x, (float)v.y);
                }

                zone.Hatch.Tag = zone;
                zone.Hatch.Selectable = true;

                if (selectShape)
                {
                    NullSelctedShape(layer);
                    zone.Hatch.Selected = true;
                }

                if (!layer.Shapes.Contains(zone.Hatch))
                {
                    layer.Add(zone.Hatch);
                    zone.Hatch.Done();
                }

                if (updateFormList)
                    m_frmZoneList.UpdateData(zone);
            }
        }

        public void AddOutdoorZone(OutdoorZone zone)
        {
            DXFViewer.Layer layer = GetOutdoorZoneLayer();

            if (layer == null)
                return;

            SetOutdoorZoneHatch(layer, zone, false, false);
        }

        public void RemoveOutdoorZone(OutdoorZone zone)
        {
            DXFViewer.Layer layer = GetOutdoorZoneLayer();

            if (layer == null)
                return;

            List<DXFViewer.Shape> removeShapes = new List<DXFViewer.Shape>();

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape == zone.PolyLine || shape == zone.Hatch)
                    removeShapes.Add(shape);
            }

            zone.PolyLine = null;
            zone.Hatch = null;
            zone.Vertices.Clear();

            foreach (DXFViewer.Shape shape in removeShapes)
            {
                layer.Remove(shape);
            }

            if (removeShapes.Count > 0)
                dxfControl1._Refresh();
        }

        private void SetOutdoorZoneVertex(DXFViewer.Layer layer, OutdoorZone zone, UnE.Geometry.Vertex2D vertex, bool selectShape = false)
        {
            if (zone.Hatch != null)
                return;

            int nVertexCount = zone.Vertices.Count;

            if (nVertexCount == 0)
                SetPolyline(layer, zone, vertex, selectShape);
            else
            {
                DXFViewer.PolyLine pLine = zone.PolyLine;

                if (pLine == null)
                {
                    pLine = new DXFViewer.PolyLine();
                    zone.PolyLine = pLine;
                }

                zone.Vertices.Add(vertex - dxfControl1.MovedVertex);

                pLine.SetOwner(dxfControl1);
                pLine.SetPointSize(nVertexCount + 1);

                for (int i = 0; i <= nVertexCount; i++)
                {
                    UnE.Geometry.Vertex2D v = zone.Vertices[i] + dxfControl1.MovedVertex;
                    pLine.UpdatePoint(i, (float)v.x, (float)v.y);
                }

                pLine.Tag = zone;
                pLine.Selectable = true;

                if (selectShape)
                {
                    NullSelctedShape(layer);
                    pLine.Selected = true;
                }

                if (!layer.Shapes.Contains(pLine))
                    layer.Shapes.Add(pLine);
            }
        }

        private void SetPolyline(DXFViewer.Layer layer, OutdoorZone zone, UnE.Geometry.Vertex2D vertex, bool selectShape = false)
        {
            zone.Vertices.Add(vertex - dxfControl1.MovedVertex);

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape.Tag == zone)
                {
                    // 기존에 존재하던 zone vertex는 삭제한다.
                    layer.Shapes.Remove(shape);
                    break;
                }
            }

            DXFViewer.PolyLine pLine = new DXFViewer.PolyLine();
            pLine.SetOwner(dxfControl1);
            pLine.SetPointSize(4);

            pLine.UpdatePoint(0, (float)vertex.x, (float)vertex.y);
            pLine.UpdatePoint(1, (float)vertex.x + m_fRectSize, (float)vertex.y);
            pLine.UpdatePoint(2, (float)vertex.x + m_fRectSize, (float)vertex.y + m_fRectSize);
            pLine.UpdatePoint(3, (float)vertex.x, (float)vertex.y + m_fRectSize);

            pLine.Tag = zone;
            pLine.Selectable = true;

            if (selectShape)
            {
                NullSelctedShape(layer);
                pLine.Selected = true;
            }

            layer.Shapes.Add(pLine);
            zone.PolyLine = pLine;
        }

        private void NullSelctedShape(DXFViewer.Layer layer)
        {
            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                shape.Selected = false;
            }
        }

        private Zone FindZone(UnE.Geometry.Vertex2D vertex)
        {
            foreach (Zone zone in m_zones)
            {
                foreach (UnE.Geometry.Polygon polygon in zone.Polygons)
                {
                    if (polygon.HitTest(vertex) != 0)
                        return zone;
                }
            }

            return null;
        }

        private OutdoorZone FindOutdoorZone(UnE.Geometry.Vertex2D vertex)
        {
            DXFViewer.Layer layer = GetOutdoorZoneLayer();

            if (layer == null)
                return null;

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape is EdgeLineHatch)
                {
                    EdgeLineHatch hatch = (EdgeLineHatch)shape;

                    if (hatch.GetPolygon().HitTest(vertex) != 0)
                        return (OutdoorZone)hatch.Tag;
                }
            }

            return null;
        }

        private void tsMenuShowZoneList_Click(object sender, EventArgs e)
        {
            if (m_frmZoneList.Visible)
                return;

            m_frmZoneList.Show();
        }

        private void tsMenuTransparentZone_Click(object sender, EventArgs e)
        {
            tsMenuTransparentZone.Checked = !tsMenuTransparentZone.Checked;
            m_transparentZone = tsMenuTransparentZone.Checked;

            dxfControl1._Refresh();
        }

        public void SelectOutdoorZone(OutdoorZone zone)
        {
            DXFViewer.Layer layer = GetOutdoorZoneLayer();

            if (layer == null)
                return;

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape is EdgeLineHatch)
                {
                    EdgeLineHatch hatch = (EdgeLineHatch)shape;
                    hatch.HiLight = zone.Hatch == shape;
                }
            }

            dxfControl1._Refresh();
        }
    }
}
