using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DXFView
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panelLeft.DXFControl = dxfControl1;

            this.Text = "DXF Viewer";
            dxfControl1.DrawHatchFirst = false;
            dxfControl1.UseLastViewport = true;
        }

        private void dxfControl1_MouseMove(object sender, MouseEventArgs e)
        {
            UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(e.X, e.Y);

            if (vertex != null)
                toolStripStatusLabel1.Text = string.Format("({0}, {1})", vertex.x, vertex.y);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;

                dxfControl1.BackColor = Color.Black;
                bool isSuccess = dxfControl1.OpenDXF(dlg.FileName);
                toolStripStatusLabel1.Text = "";

                if (!isSuccess)
                {
                    string strError = "DXF 불러오기가 실패하였습니다.";
                    MessageBox.Show(strError);
                }
                else
                {
                    List<DXFViewer.Shape> shapes = new List<DXFViewer.Shape>();

                    foreach (DXFViewer.Layer layer in dxfControl1.Layers)
                    {
                        if (!layer.LayerName.Contains("L_교통시설"))
                            continue;

                        foreach (DXFViewer.Shape shape in layer.Shapes)
                        {
                            if (shape.Visible && shape is DXFViewer.PolyLine)
                                shapes.Add(shape);
                        }
                    }

                    this.Text = dlg.FileName;
                    panelLeft.Layers = dxfControl1.Layers;
                    panelLeft.Blocks = dxfControl1.Blocks;
                    panelLeft.Init();

                    SetViewport();
                }

                this.Cursor = Cursors.Arrow;
            }
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

        private void tsMenuOnlyBlocks_Click(object sender, EventArgs e)
        {
            // m_dxfControl에서 Block에 포함된 것들을 제외하고 모두 없앤다.
            foreach (DXFViewer.Layer layer in dxfControl1.Layers)
            {
                List<DXFViewer.Shape> removeShapes = new List<DXFViewer.Shape>();

                foreach (DXFViewer.Shape shape in layer.Shapes)
                {
                    if (shape.GetBlock() == null)
                        removeShapes.Add(shape);
                }

                foreach (DXFViewer.Shape shape in removeShapes)
                {
                    layer.Shapes.Remove(shape);
                }
            }

            dxfControl1._Refresh();
        }
    }
}
