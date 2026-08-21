using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShapeFileReader
{
    public partial class FormMain : Form, Drawing.IShapeAttrib
    {
        // Object 최대값, 최소값의 차이를 m_nScaleSize에 맞춘다.
        private int m_nScaleSize = 2000;
        private double m_dScale = -1.0;
        // 실제 Shape 객체들의 중심 좌표
        private UnE.Geometry.Vertex2D m_vCenter = null;
        // 화면 중심점
        private UnE.Geometry.Vertex2D m_vScreenCenter = null;

        private Attrib.FormShapeAttrib m_frmShapeAttrib = null;

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        public UnE.Geometry.Vertex2D ScreenCenter
        {
            get { return m_vScreenCenter; }
        }

        public FormMain()
        {
            m_instance = this;
            InitializeComponent();
        }

        private void menuOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "Shape Files|*.shp";
            dlg.FilterIndex = 0;
            dlg.Title = "Shape 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OpenFile(dlg.FileName);
            }
        }

        private bool OpenFile(string strShapeFilePath)
        {
            libShapeFile.ShapeInfo shapeInfo = null;
            libShapeFile.FileLoader loader = new libShapeFile.FileLoader();
            List<libShapeFile.Shape> shapes = loader.LoadFile(strShapeFilePath, out shapeInfo);

            if (shapes == null)
                return false;

            m_vScreenCenter = new UnE.Geometry.Vertex2D(this.Size.Width / 2, this.Size.Height / 2);
            m_dScale = GetScale(loader.TopLeft, loader.BottomRight, out m_vCenter);

            UnE.Geometry.Vertex2D vTL = Drawing.BoundingShape.ScaleTransfer(loader.TopLeft.x, loader.TopLeft.y, m_dScale, m_vCenter);
            UnE.Geometry.Vertex2D vBR = Drawing.BoundingShape.ScaleTransfer(loader.BottomRight.x, loader.BottomRight.y, m_dScale, m_vCenter);

            LoadShapes(shapes, m_dScale, m_vCenter, shapeInfo);
            SetViewport(vTL, vBR);
            dxfControl1.Refresh();
            return true;
        }

        private void LoadShapes(List<libShapeFile.Shape> shapes, double dScale, UnE.Geometry.Vertex2D vCenter, libShapeFile.ShapeInfo shapeInfo)
        {
            Type typePolyLine = typeof(libShapeFile.PolyLine);
            Type typePoint = typeof(libShapeFile.Point);
            Type typePolygon = typeof(libShapeFile.Polygon);
            Type typeMultiPoint = typeof(libShapeFile.MultiPoint);

            foreach (libShapeFile.Shape shape in shapes)
            {
                Type type = shape.GetType();

                if (type == typePoint)
                    AddPoint((libShapeFile.Point)shape, dScale, vCenter, shapeInfo);
                else if (type == typePolyLine)
                    AddPolyLine((libShapeFile.PolyLine)shape, dScale, vCenter, shapeInfo);
                else if (type == typePolygon)
                    AddPolygon((libShapeFile.Polygon)shape, dScale, vCenter, shapeInfo);
                else if (type == typeMultiPoint)
                    AddMultiPoint((libShapeFile.MultiPoint)shape, dScale, vCenter, shapeInfo);
            }

            SetFirstLast();
        }

        private void SetFirstLast(string strLayerName)
        {
            DXFViewer.Layer layer = FindLayer(strLayerName);

            if (layer != null)
            {
                int nShapeCount = layer.Shapes.Count;

                if (nShapeCount > 0)
                {
                    Drawing.PointShape shapeFirst = (Drawing.PointShape)layer.Shapes[0];
                    Drawing.PointShape shapeLast = (Drawing.PointShape)layer.Shapes[nShapeCount - 1];

                    shapeFirst.FirstElement = true;
                    shapeLast.LastElement = true;
                }
            }
        }

        private void SetFirstLast()
        {
            SetFirstLast("Point");
            SetFirstLast("PolyLine");
            SetFirstLast("Polygon");
            SetFirstLast("MultiPoint");
        }

        private double GetScale(UnE.Geometry.Vertex2D vTL, UnE.Geometry.Vertex2D vBR, out UnE.Geometry.Vertex2D vCenter)
        {
            vCenter = (vTL + vBR) / 2;

            double dWidth = vBR.x - vTL.x;
            double dHeight = vTL.y - vBR.y;
            double dBig = dWidth > dHeight ? dWidth : dHeight;

            if (dBig <= UnE.Geometry.Math.HALF_TOLERANCE())
                return 1.0;

            double dScale = m_nScaleSize / dBig;
            return dScale;
        }

        private void SetViewport(UnE.Geometry.Vertex2D vTL, UnE.Geometry.Vertex2D vBR)
        {
	        UnE.Geometry.Vertex2D vOrigin = dxfControl1.ScreenToGlobal(0, 0);
	        UnE.Geometry.Vertex2D v100 = dxfControl1.ScreenToGlobal(100, 0);

	        int nCenterX = dxfControl1.Size.Width / 2;
	        int nCenterY = dxfControl1.Size.Height / 2;
	        UnE.Geometry.Vertex2D vCenter = dxfControl1.ScreenToGlobal(nCenterX, nCenterY);

            UnE.Geometry.Vertex2D vObjectCenter = (vTL + vBR) / 2;

	        double distance = vOrigin.GetDistance(v100);
	        double w = vObjectCenter.x - vCenter.x;
	        double h = vObjectCenter.y - vCenter.y;

	        int nMoveX = (int)(100 * w / distance);
	        int nMoveY = (int)(100 * h / distance);

	        UnE.Geometry.Vertex2D vViewportCenter = dxfControl1.GetViewportCenter();
            UnE.Geometry.Vertex2D vNewCenter = new UnE.Geometry.Vertex2D(vViewportCenter.x + nMoveX, -vViewportCenter.y + nMoveY);
	        dxfControl1.SetViewportCenter(vNewCenter);

            UnE.Geometry.Vertex2D vCurrent = dxfControl1.ScreenToGlobal(nCenterX, nCenterY);

            double weight1 = dxfControl1.Size.Width * 0.85 / ((vObjectCenter.x - vTL.x) * 2);
            double weight2 = dxfControl1.Size.Height * 0.85 / ((vObjectCenter.y - vBR.y) * 2);
            double dViewportWeight = weight1 < weight2 ? weight1 : weight2;

            dxfControl1.Zoom(dViewportWeight, vCurrent, false);
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

        private void AddPoint(libShapeFile.Point point, double dScale, UnE.Geometry.Vertex2D vCenter, libShapeFile.ShapeInfo shapeInfo)
        {
            DXFViewer.Layer layer = FindLayer("Point");

            if (layer == null)
            {
                layer = new Drawing.ShapeLayer(dxfControl1);
                layer.LayerName = "Point";
                dxfControl1.Layers.Add(layer);
            }

            UnE.Geometry.Vertex2D vCoord = Drawing.BoundingShape.ScaleTransfer(point.Vertex.x, point.Vertex.y, dScale, vCenter);
            Drawing.Point point2 = new Drawing.Point(vCoord.x, vCoord.y);
            point2.ID = point.ID;
            point2.SetAttrib(this);
            layer.Add(point2);

            point2.ShapeInfo = shapeInfo;
        }

        private void AddPolyLine(libShapeFile.PolyLine polyLine, double dScale, UnE.Geometry.Vertex2D vCenter, libShapeFile.ShapeInfo shapeInfo)
        {
            DXFViewer.Layer layer = FindLayer("PolyLine");

            if (layer == null)
            {
                layer = new Drawing.ShapeLayer(dxfControl1);
                layer.LayerName = "PolyLine";
                dxfControl1.Layers.Add(layer);
            }

            Drawing.PolyLine polyLine2 = new Drawing.PolyLine();
            int nSubLineCount = polyLine.SubPolyLineCount;

            for (int i = 0; i < nSubLineCount;i++ )
            {
                List<UnE.Geometry.Vertex2D> vertices = polyLine.GetSubPolyLine(i);
                polyLine2.AddVertices(vertices, dScale, vCenter);
            }

            UnE.Geometry.Vertex2D vMin = Drawing.BoundingShape.ScaleTransfer(polyLine.MinX, polyLine.MinY, dScale, vCenter);
            UnE.Geometry.Vertex2D vMax = Drawing.BoundingShape.ScaleTransfer(polyLine.MaxX, polyLine.MaxY, dScale, vCenter);

            polyLine2.SetBounding(vMin.x, vMax.x, vMin.y, vMax.y);
            polyLine2.ID = polyLine.ID;
            polyLine2.SetAttrib(this);
            layer.Add(polyLine2);

            polyLine2.ShapeInfo = shapeInfo;
        }

        private void AddPolygon(libShapeFile.Polygon polygon, double dScale, UnE.Geometry.Vertex2D vCenter, libShapeFile.ShapeInfo shapeInfo)
        {
            DXFViewer.Layer layer = FindLayer("Polygon");

            if (layer == null)
            {
                layer = new Drawing.ShapeLayer(dxfControl1);
                layer.LayerName = "Polygon";
                dxfControl1.Layers.Add(layer);
            }

            Drawing.Polygon polygon2 = new Drawing.Polygon();
            int nSubPolygonCount = polygon.SubPolygonCount;

            for (int i = 0; i < nSubPolygonCount; i++)
            {
                List<UnE.Geometry.Vertex2D> vertices = polygon.GetSubPolygon(i);
                polygon2.AddVertices(vertices, dScale, vCenter);
            }

            UnE.Geometry.Vertex2D vMin = Drawing.BoundingShape.ScaleTransfer(polygon.MinX, polygon.MinY, dScale, vCenter);
            UnE.Geometry.Vertex2D vMax = Drawing.BoundingShape.ScaleTransfer(polygon.MaxX, polygon.MaxY, dScale, vCenter);

            polygon2.SetBounding(vMin.x, vMax.x, vMin.y, vMax.y);
            polygon2.ID = polygon.ID;
            polygon2.SetAttrib(this);
            layer.Add(polygon2);

            polygon2.ShapeInfo = shapeInfo;
        }

        private void AddMultiPoint(libShapeFile.MultiPoint multiPoint, double dScale, UnE.Geometry.Vertex2D vCenter, libShapeFile.ShapeInfo shapeInfo)
        {
            DXFViewer.Layer layer = FindLayer("MultiPoint");

            if (layer == null)
            {
                layer = new Drawing.ShapeLayer(dxfControl1);
                layer.LayerName = "MultiPoint";
                dxfControl1.Layers.Add(layer);
            }

            Drawing.MultiPoint multiPoint2 = new Drawing.MultiPoint();
            int nVertexCount = multiPoint.GetVertexCount();

            for (int i = 0; i < nVertexCount; i++)
            {
                UnE.Geometry.Vertex2D vertex = multiPoint.GetVertex(i);
                UnE.Geometry.Vertex2D vCoord = Drawing.BoundingShape.ScaleTransfer(vertex.x, vertex.y, dScale, vCenter);
                multiPoint2.AddVertex(vCoord);
            }

            UnE.Geometry.Vertex2D vMin = Drawing.BoundingShape.ScaleTransfer(multiPoint.MinX, multiPoint.MinY, dScale, vCenter);
            UnE.Geometry.Vertex2D vMax = Drawing.BoundingShape.ScaleTransfer(multiPoint.MaxX, multiPoint.MaxY, dScale, vCenter);

            multiPoint2.SetBounding(vMin.x, vMax.x, vMin.y, vMax.y);
            multiPoint2.ID = multiPoint.ID;
            multiPoint2.SetAttrib(this);
            layer.Add(multiPoint2);

            multiPoint2.ShapeInfo = shapeInfo;
        }

        private void btnLineColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                btnLineColor.BackColor = dlg.Color;
            }
        }

        private void btnFillColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                btnFillColor.BackColor = dlg.Color;
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            cboPointShape.SelectedIndex = 0;
            toolStripStatusLabel1.Text = "";
        }

        public Color GetLineColor()
        {
            if (checkBoxTransparentLine.Checked)
                return Color.Transparent;

            return btnLineColor.BackColor;
        }

        public Color GetFillColor()
        {
            if (checkBoxTransparentFill.Checked)
                return Color.Transparent;

            return btnFillColor.BackColor;
        }

        public double GetPointSize()
        {
            double dSize;

            if (double.TryParse(textBoxPointSize.Text, out dSize) && dSize > 0)
                return dSize;

            return 0;
        }

        public Drawing.PointDrawingType GetPointDrawingType()
        {
            return (Drawing.PointDrawingType)cboPointShape.SelectedIndex;
        }

        public int GetLineThickness()
        {
            int nThick;

            if (int.TryParse(textBoxLineThick.Text, out nThick) && nThick > 0)
                return nThick;

            return 0;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            panelLeft.Location = new Point(0, menuStrip1.Size.Height);
            panelLeft.Size = new Size(200, this.Size.Height - menuStrip1.Size.Height - statusStrip1.Size.Height);

            panelMain.Location = new Point(panelLeft.Location.X + panelLeft.Size.Width, menuStrip1.Size.Height);
            panelMain.Size = new Size(this.Size.Width - panelMain.Location.X, panelLeft.Size.Height);
        }

        private void dxfControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_vCenter == null)
                return;

            double x, y;

            if (GetDXFCoord(e.X, e.Y, out x, out y))
            {
                toolStripStatusLabel1.Text = string.Format("({0:f1}, {1:f1}), 단위(m)", x, y);
            }
        }

        private bool GetDXFCoord(int x, int y, out double _x, out double _y)
        {
            _x = _y = 0.0;
            UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(x, y);

            if (vertex != null)
            {
                UnE.Geometry.Vertex2D vMove = dxfControl1.MovedVertex;
                float fFlag = 1.0f;
                _x = (vertex.x - vMove.x) * fFlag;
                _y = (vertex.y - vMove.y) * fFlag;

                _x = (_x - m_vScreenCenter.x) / m_dScale + m_vCenter.x;
                _y = (_y - m_vScreenCenter.y) / m_dScale + m_vCenter.y;
                /*_x = (_x - m_vCenter.x) / m_dScale + m_vCenter.x;
                _y = (_y - m_vCenter.y) / m_dScale + m_vCenter.y;*/

                return true;
            }

            return false;
        }

        private void checkBoxTransparentLine_CheckedChanged(object sender, EventArgs e)
        {
            dxfControl1.Refresh();
        }

        private void checkBoxTransparentFill_CheckedChanged(object sender, EventArgs e)
        {
            dxfControl1.Refresh();
        }

        private void dxfControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(e.X, e.Y);
                DXFViewer.Shape shape = dxfControl1.SelectObject(vertex.x, vertex.y);

                if (shape != null && shape is Drawing.PointShape)
                {
                    if (m_frmShapeAttrib == null || m_frmShapeAttrib.Visible == false)
                    {
                        m_frmShapeAttrib = new Attrib.FormShapeAttrib();
                        m_frmShapeAttrib.SetShape((Drawing.PointShape)shape);
                        m_frmShapeAttrib.Show(this);
                    }
                    else
                    {
                        m_frmShapeAttrib.SetShape((Drawing.PointShape)shape);
                    }
                }
            }
        }
    }
}
