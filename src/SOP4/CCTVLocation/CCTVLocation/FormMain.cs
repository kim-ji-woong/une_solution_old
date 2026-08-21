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
        private FormCCTVList m_frmCCTVList = new FormCCTVList("");
        private string m_strDXFFilePath = "";
        private string m_strDataFilePath = "";
        private string m_strCCTVLayerName = "CCTV_COORDS";
        private float m_fRectSize = 100.0f;
        private DBUtility.WebDBManager m_dbMgr = new DBUtility.WebDBManager("SOP4");
        private List<PolygonEx> m_polygons = new List<PolygonEx>();

        private FormCoordText m_frmCoordText = new FormCoordText();

        private DXFViewer.Shape m_shapeSelected = null;

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

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
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

                SetViewport();
            }

            m_strDXFFilePath = strFilePath;
            return true;
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

            LoadZoneInfo();
            m_frmCoordText.Show(this);
        }

        class PolygonEx : UnE.Geometry.Polygon
        {
            private string m_strZoneName = "";

            public string ZoneName
            {
                get { return m_strZoneName; }
                set { m_strZoneName = value; }
            }
        }

        private void LoadZoneInfo()
        {
            string strSQL = "Select ID, ZoneName, BuildingID, FloorIndex, AddFloor, Boundary from Zone where SiteID = 2";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-5;i+=6)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strZoneName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                int nBuildingID = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFloorIndex = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strAddFloor = DBUtility.WebDBManager.GetStringField(arrResult[i + 4], "");
                string strBoundary = DBUtility.WebDBManager.GetStringField(arrResult[i + 5], "");

                if (strBoundary.Length == 0 || strBoundary == "null")
                    continue;

                string[] strCoords = strBoundary.Split('#');
                int nIndex = 0;

                foreach (string strCoord in strCoords)
                {
                    PolygonEx polygon = ReadPolygon(strCoord.Trim(), strZoneName, nIndex++);

                    if (polygon != null)
                        m_polygons.Add(polygon);
                }

                /*string[] strCoords = strBoundary.Split(',');

                PolygonEx polygon = new PolygonEx();
                polygon.ZoneName = strZoneName;
                m_polygons.Add(polygon);

                int nCoordCount = strCoords.Count();

                for (int j=0;j<nCoordCount;j+=2)
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
                    }
                }*/
            }

            //PolygonEx _polygon = HitTestZone(new UnE.Geometry.Vertex2D(1572696.099, 369951.1178));

            //if (_polygon == null)
            //    System.Diagnostics.Trace.WriteLine("Null Polygon");
            //else
            //    System.Diagnostics.Trace.WriteLine(_polygon.ZoneName); ;
        }

        private PolygonEx ReadPolygon(string strBoundary, string strZoneName, int nIndex)
        {
            string[] strCoords = strBoundary.Split(',');

            PolygonEx polygon = new PolygonEx();
            polygon.ZoneName = strZoneName;
            m_polygons.Add(polygon);

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

        private PolygonEx HitTestZone(UnE.Geometry.Vertex2D vertex)
        {
            foreach (PolygonEx polygon in m_polygons)
            {
                int nResult = polygon.HitTest(vertex);

                if (nResult != 0)
                    return polygon;
            }

            return null;
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
                this.Cursor = Cursors.WaitCursor;

                m_frmCCTVList.DataFilePath = dlg.FileName;
                m_strDataFilePath = dlg.FileName;

                tsMenuShowCCTVList.Enabled = true;
                tsMenuSaveDataFile.Enabled = true;

                List<CCTV> cctvList;
                string strDXFFilePath = m_frmCCTVList.Show(out cctvList);

                if (strDXFFilePath != null)
                {
                    ReloadDXF(strDXFFilePath);
                }

                if (m_strDXFFilePath.Length > 0 && cctvList != null)
                {
                    DXFViewer.Layer layer = FindLayer(m_strCCTVLayerName);

                    if (layer == null)
                    {
                        layer = new DXFViewer.Layer(dxfControl1);
                        layer.LayerName = m_strCCTVLayerName;
                        layer.LineColor = Color.Green;

                        dxfControl1.Layers.Add(layer);
                    }

                    foreach (CCTV cctv in cctvList)
                    {
                        if (cctv.Position != null)
                        {
                            SetCCTVVertex(layer, cctv, cctv.Position + dxfControl1.MovedVertex);
                        }
                    }
                }

                this.Cursor = Cursors.Arrow;
                dxfControl1._Refresh();
            }
        }

        private void SetCCTVVertex(DXFViewer.Layer layer, CCTV cctv, UnE.Geometry.Vertex2D vertex, bool selectShape = false)
        {
            /*if (cctv.Position != vertex)
                cctv.Position = vertex;*/
            cctv.Position = vertex - dxfControl1.MovedVertex;

            foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                if (shape.Tag == cctv)
                {
                    // 기존에 존재하던 cctv vertex는 삭제한다.
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

            pLine.Tag = cctv;
            pLine.Selectable = true;

            if (selectShape)
            {
                NullSelctedShape(layer);
                pLine.Selected = true;
                m_shapeSelected = pLine;
            }

            layer.Shapes.Add(pLine);
        }

        private void NullSelctedShape(DXFViewer.Layer layer)
        {
            //if (m_shapeSelected == null)
            //    return;

            /*foreach (DXFViewer.Shape shape in layer.Shapes)
            {
                shape.Selected = false;
            }

            //m_shapeSelected.Selected = false;
            m_shapeSelected = null;*/
        }

        private void ReloadDXF(string strFilePath)
        {
            if (m_strDXFFilePath != strFilePath)
            {
                dxfControl1.Layers.Clear();

                if (!LoadDXF(strFilePath))
                    return;
            }

            DXFViewer.Layer layer = FindLayer(m_strCCTVLayerName);

            if (layer == null)
            {
                layer = new DXFViewer.Layer(dxfControl1);
                layer.LayerName = m_strCCTVLayerName;
                layer.LineColor = Color.Green;

                dxfControl1.Layers.Add(layer);
            }

            layer.Shapes.Clear();
        }

        private DXFViewer.Layer FindLayer(string strLayerName)
        {
            foreach (DXFViewer.Layer layer in dxfControl1.Layers)
            {
                if (layer.LayerName == strLayerName)
                    return layer;
            }

            return null;
        }

        private void tsMenuShowCCTVList_Click(object sender, EventArgs e)
        {
            m_frmCCTVList.Show();
        }

        private void tsMenuSaveDataFile_Click(object sender, EventArgs e)
        {
            m_frmCCTVList.SaveDataFile(m_strDataFilePath);
        }

        private void dxfControl1_MouseClick(object sender, MouseEventArgs e)
        {
            /*if (!m_frmCCTVList.Visible)
                return;

            CCTV cctv = m_frmCCTVList.GetCurrentCCTV();

            if (cctv == null)
                return;*/

            CCTV cctv = new CCTV();

            DXFViewer.Layer layer = FindLayer(m_strCCTVLayerName);

            if (layer == null)
            {
                layer = new DXFViewer.Layer(dxfControl1);
                layer.LayerName = m_strCCTVLayerName;
                layer.LineColor = Color.Green;

                dxfControl1.Layers.Add(layer);
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (layer != null)
                {
                    UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(e.X, e.Y);
                    SetCCTVVertex(layer, cctv, vertex, true);
                    dxfControl1._Refresh();
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (layer != null)
                {
                    int nShapeCount = layer.Shapes.Count;

                    if (nShapeCount > 0)
                    {
                        DXFViewer.Shape shape = (DXFViewer.Shape)layer.Shapes[nShapeCount - 1];
                        layer.Remove(shape);
                    }

                    dxfControl1._Refresh();
                }
            }
        }

        public void OnHideCCTVList()
        {
            DXFViewer.Layer layer = FindLayer(m_strCCTVLayerName);

            if (layer != null)
                NullSelctedShape(layer);
        }

        public void SelectCCTV(CCTV cctv)
        {
            DXFViewer.Layer layer = FindLayer(m_strCCTVLayerName);

            if (layer != null)
            {
                NullSelctedShape(layer);

                foreach (DXFViewer.Shape shape in layer.Shapes)
                {
                    if (shape.Tag == cctv)
                    {
                        shape.Selected = true;
                        break;
                    }
                }

                dxfControl1._Refresh();
            }
        }

        public void ShowAll()
        {
            DXFViewer.Layer layer = FindLayer(m_strCCTVLayerName);

            if (layer != null)
            {
                foreach (DXFViewer.Shape shape in layer.Shapes)
                {
                    shape.Selected = true;
                }

                dxfControl1._Refresh();
            }
        }

        private void tsMenuShowCoordText_Click(object sender, EventArgs e)
        {
            //frm.CoordText = GetCoordText();

            UnE.Geometry.Vertex2D vFirst, vLast;

            if (GetVertices(out vFirst, out vLast))
            {
                m_frmCoordText.SetVertex(vFirst, vLast);
            }

            //frm.Show();
        }

        private bool GetVertices(out UnE.Geometry.Vertex2D vFirst, out UnE.Geometry.Vertex2D vLast)
        {
            vFirst = vLast = null;
            DXFViewer.Layer layer = FindLayer(m_strCCTVLayerName);

            if (layer == null)
                return false;

            if (layer != null)
            {
                UnE.Geometry.Vertex2D vMoved = dxfControl1.MovedVertex;
                float fMovedX = (float)vMoved.x;
                float fMovedY = (float)vMoved.y;

                if (layer.Shapes.Count < 2)
                    return false;

                DXFViewer.PolyLine pLine1 = (DXFViewer.PolyLine)layer.Shapes[0];
                DXFViewer.PolyLine pLine2 = (DXFViewer.PolyLine)layer.Shapes[layer.Shapes.Count - 1];

                PointF pt1 = pLine1.GetVertex(0);
                PointF pt2 = pLine2.GetVertex(0);

                vFirst = new UnE.Geometry.Vertex2D(pt1.X - fMovedX, pt1.Y - fMovedY);
                vLast = new UnE.Geometry.Vertex2D(pt2.X - fMovedX, pt2.Y - fMovedY);
                return true;
            }

            return false;
        }

        private List<PointF> GetVertices()
        {
            DXFViewer.Layer layer = FindLayer(m_strCCTVLayerName);

            if (layer == null)
                return null;

            if (layer != null)
            {
                UnE.Geometry.Vertex2D vMoved = dxfControl1.MovedVertex;
                float fMovedX = (float)vMoved.x;
                float fMovedY = (float)vMoved.y;

                List<PointF> vertices = new List<PointF>();

                foreach (DXFViewer.Shape shape in layer.Shapes)
                {
                    if (shape is DXFViewer.PolyLine)
                    {
                        DXFViewer.PolyLine pLine = (DXFViewer.PolyLine)shape;

                        PointF pt = pLine.GetVertex(0);
                        pt.X = pt.X - fMovedX;
                        pt.Y = pt.Y - fMovedY;

                        vertices.Add(pt);
                    }
                }

                return vertices;
            }

            return null;
        }

        private string GetCoordText()
        {
            DXFViewer.Layer layer = FindLayer(m_strCCTVLayerName);

            if (layer == null)
                return "";

            string strCoords = "";

            if (layer != null)
            {
                UnE.Geometry.Vertex2D vMoved = dxfControl1.MovedVertex;
                float fMovedX = (float)vMoved.x;
                float fMovedY = (float)vMoved.y;

                foreach (DXFViewer.Shape shape in layer.Shapes)
                {
                    DXFViewer.PolyLine pLine = (DXFViewer.PolyLine)shape;
                    PointF pt = pLine.GetVertex(0);

                    string strCoord = (pt.X - fMovedX).ToString() + "," + (pt.Y - fMovedY).ToString();

                    if (strCoords.Length == 0)
                        strCoords = strCoord;
                    else
                        strCoords += "," + strCoord;
                }
            }

            return strCoords;
        }

        private void tsMenuPolylineText_Click(object sender, EventArgs e)
        {
            List<PointF> vertices = GetVertices();
            m_frmCoordText.SetPolylineText(vertices);
        }

        private void tsDeleteAllVertex_Click(object sender, EventArgs e)
        {
            DXFViewer.Layer layer = FindLayer(m_strCCTVLayerName);

            if (layer == null)
                return;

            if (layer != null)
            {
                layer.RemoveAll();
                dxfControl1._Refresh();
            }
        }
    }
}
