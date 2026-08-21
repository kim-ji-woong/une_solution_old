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

namespace FireSensorReader
{
    public partial class FormMain : Form
    {
        private FormLayer m_frmLayer = null;
        private List<LayerEx> m_sensorLayers = new List<LayerEx>();
        private List<Layer> m_textLayers = new List<Layer>();
        private HatchEx m_selectedShape = null;
        private Color m_selectedColor = Color.AliceBlue;
        private Color m_originalColor = Color.Red;
        private Color m_linkedColor = Color.Yellow;

        private double m_dVertexDistance = 800;
        private bool m_bMoveText = false;
        private Vertex2D m_vMoveOrigin = null;
        private Dictionary<DXFViewer.Text, Vertex2D> m_dicSensorTextOrigin = new Dictionary<Text, Vertex2D>();

        public FormMain()
        {
            InitializeComponent();

            labelStatus.Text = "";
            labelCoord.Text = "";
        }

        private void dxfSensors_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Count() == 1)
            {
                string strFileName = files[0].ToLower();

                if (strFileName.EndsWith("dxf"))
                {
                    this.Cursor = Cursors.WaitCursor;

                    dxfSensors.OpenDXF(strFileName);
                    AddSensorObject();

                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        private void AddSensorObject()
        {
            LayerEx smokeLayer = new LayerEx(dxfSensors);
            smokeLayer.LineColor = Color.Red;
            smokeLayer.LayerName = "SmokeSensor";
            smokeLayer.NickName = "연기감지기";

            LayerEx heatLayer = new LayerEx(dxfSensors);
            heatLayer.LineColor = Color.Red;
            heatLayer.LayerName = "HeatSensor";
            heatLayer.NickName = "열감지기";

            m_sensorLayers.Clear();
            m_textLayers.Clear();
            m_dicSensorTextOrigin.Clear();

            SetSelectedShape(null);

            foreach (Layer layer in dxfSensors.Layers)
            {
                LayerEx targetLayer = null;

                if (layer.LayerName.Contains("열감지기"))
                    targetLayer = heatLayer;
                else if (layer.LayerName.Contains("연기감지기"))
                    targetLayer = smokeLayer;
                else
                {
                    if (layer.LayerName.Contains("어드레스"))
                    {
                        m_textLayers.Add(layer);

                        foreach (Shape shape in layer.Shapes)
                        {
                            if (shape is DXFViewer.Text)
                            {
                                m_dicSensorTextOrigin[(DXFViewer.Text)shape] = shape.Position;
                                ((DXFViewer.Text)shape).Angle = 0.0;
                            }
                        }
                    }

                    continue;
                }

                m_sensorLayers.Add(targetLayer);
                IPainter owner = null;

                int nShapeCount = layer.Shapes.Count;
                Dictionary<Shape, VertexGroup> dicShapeVertexGroup = new Dictionary<Shape, VertexGroup>();

                VertexGroup group2 = null;

                labelStatus.Text = layer.LayerName + " Loading 0 / " + nShapeCount.ToString();

                for (int i=0;i<nShapeCount-1;i++)
                {
                    VertexGroup group = null;
                    Shape shape1 = (Shape)layer.Shapes[i];

                    if (dicShapeVertexGroup.TryGetValue(shape1, out group))
                        continue;

                    owner = shape1.GetOwner();

                    Vertex2D vertex = null;

                    if (shape1 is DXFViewer.Line)
                    {
                        DXFViewer.Line line = (DXFViewer.Line)shape1;
                        vertex = (line.Begin + line.End) / 2;

                        group = new VertexGroup();
                        group.AddVertex(vertex);
                    }
                    else if (shape1 is DXFViewer.Arc)
                    {
                        DXFViewer.Arc arc = (DXFViewer.Arc)shape1;
                        vertex = arc.Center;

                        Vertex2D vTL = new Vertex2D(arc.Center.x - arc.Radius, arc.Center.y + arc.Radius);
                        Vertex2D vBL = new Vertex2D(arc.Center.x - arc.Radius, arc.Center.y - arc.Radius);
                        Vertex2D vBR = new Vertex2D(arc.Center.x + arc.Radius, arc.Center.y - arc.Radius);
                        Vertex2D vTR = new Vertex2D(arc.Center.x + arc.Radius, arc.Center.y + arc.Radius);

                        group = new VertexGroup();
                        group.AddVertex(vTL);
                        group.AddVertex(vBL);
                        group.AddVertex(vBR);
                        group.AddVertex(vTR);
                    }
                    else if (shape1 is DXFViewer.EArc)
                    {
                        DXFViewer.EArc arc = (DXFViewer.EArc)shape1;

                        Vertex2D vTL = new Vertex2D(arc.BoundaryTL);
                        Vertex2D vBR = new Vertex2D(arc.BoundaryBR);
                        Vertex2D vBL = new Vertex2D(vTL.x, vBR.y);
                        Vertex2D vTR = new Vertex2D(vBR.x, vTL.y);
                        vertex = (vTL + vBR) / 2;

                        group = new VertexGroup();
                        group.AddVertex(vTL);
                        group.AddVertex(vBL);
                        group.AddVertex(vBR);
                        group.AddVertex(vTR);
                    }
                    else
                        continue;

                    dicShapeVertexGroup[shape1] = group;

                    for (int j=i + 1;j<nShapeCount;j++)
                    {
                        Shape shape2 = (Shape)layer.Shapes[j];

                        if (dicShapeVertexGroup.TryGetValue(shape2, out group2))
                            continue;

                        Vertex2D vertex2 = null;

                        if (shape2 is DXFViewer.Line)
                        {
                            DXFViewer.Line line = (DXFViewer.Line)shape2;
                            vertex2 = (line.Begin + line.End) / 2;
                        }
                        else if (shape2 is DXFViewer.Arc)
                        {
                            DXFViewer.Arc arc = (DXFViewer.Arc)shape2;
                            vertex2 = arc.Center;
                        }
                        else
                            continue;

                        if (vertex.GetDistance(vertex2) <= m_dVertexDistance)
                        {
                            if (shape2 is DXFViewer.Line)
                            {
                                group.AddVertex(vertex2);
                            }
                            else if (shape2 is DXFViewer.Arc)
                            {
                                DXFViewer.Arc arc = (DXFViewer.Arc)shape2;

                                Vertex2D vTL = new Vertex2D(arc.Center.x - arc.Radius, arc.Center.y + arc.Radius);
                                Vertex2D vBL = new Vertex2D(arc.Center.x - arc.Radius, arc.Center.y - arc.Radius);
                                Vertex2D vBR = new Vertex2D(arc.Center.x + arc.Radius, arc.Center.y - arc.Radius);
                                Vertex2D vTR = new Vertex2D(arc.Center.x + arc.Radius, arc.Center.y + arc.Radius);

                                group.AddVertex(vTL);
                                group.AddVertex(vBL);
                                group.AddVertex(vBR);
                                group.AddVertex(vTR);
                            }
                            else if (shape2 is DXFViewer.EArc)
                            {
                                DXFViewer.EArc arc = (DXFViewer.EArc)shape2;

                                Vertex2D vTL = new Vertex2D(arc.BoundaryTL);
                                Vertex2D vBR = new Vertex2D(arc.BoundaryBR);
                                Vertex2D vBL = new Vertex2D(vTL.x, vBR.y);
                                Vertex2D vTR = new Vertex2D(vBR.x, vTL.y);

                                group.AddVertex(vTL);
                                group.AddVertex(vBL);
                                group.AddVertex(vBR);
                                group.AddVertex(vTR);
                            }

                            dicShapeVertexGroup[shape2] = group;
                        }
                    }

                    labelStatus.Text = layer.LayerName + " Loading " + i.ToString() + " / " + nShapeCount.ToString();
                    labelStatus.Refresh();
                }

                foreach (KeyValuePair<Shape, VertexGroup> pair in dicShapeVertexGroup)
                {
                    pair.Value.MakeHatch(owner, targetLayer, m_originalColor);
                }

                int nHatchCount = targetLayer.Shapes.Count;
                Dictionary<HatchEx, HatchEx> dicRemoves = new Dictionary<HatchEx, HatchEx>();

                for (int i=0;i<nHatchCount-1;i++)
                {
                    HatchEx hatch1 = (HatchEx)targetLayer.Shapes[i];

                    if (dicRemoves.ContainsKey(hatch1))
                        continue;

                    for (int j=i+1;j<nHatchCount;j++)
                    {
                        HatchEx hatch2 = (HatchEx)targetLayer.Shapes[j];

                        if (hatch1.Position.GetDistance(hatch2.Position) <= m_dVertexDistance)
                        {
                            dicRemoves[hatch2] = hatch2;
                        }
                    }
                }

                foreach (KeyValuePair<HatchEx, HatchEx> pair in dicRemoves)
                {
                    targetLayer.Shapes.Remove(pair.Key);
                }
            }

            foreach (Layer layer in m_sensorLayers)
            {
                dxfSensors.Layers.Add(layer);
            }

            labelStatus.Text = "Loading Complete";
            dxfSensors._Refresh();
        }

        private void dxfSensors_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Count() == 1)
                {
                    string strFileName = files[0].ToLower();

                    if (strFileName.EndsWith("dxf"))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void tsMenuLayer_Click(object sender, EventArgs e)
        {
            if (m_frmLayer == null || m_frmLayer.IsDisposed)
            {
                m_frmLayer = new FormLayer(dxfSensors);

                List<Layer> layers = new List<Layer>();

                foreach (Layer layer in dxfSensors.Layers)
                {
                    layers.Add(layer);
                }

                m_frmLayer.SetLayers(layers);
            }

            m_frmLayer.Show();
        }

        private void dxfSensors_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Vertex2D vPos = dxfSensors.ScreenToGlobal(e.X, e.Y);
                
                if (vPos != null)
                {
                    bool find = false;

                    foreach (Layer layer in m_sensorLayers)
                    {
                        foreach (Shape shape in layer.Shapes)
                        {
                            if (shape is HatchEx && shape.HitTest(vPos.x, vPos.y))
                            {
                                find = true;
                                SetSelectedShape((HatchEx)shape);
                                break;
                            }
                        }

                        if (find)
                            break;
                    }

                    if (find)
                        return;

                    if (m_selectedShape != null)
                    {
                        bool findText = false;

                        foreach (Layer layer in m_textLayers)
                        {
                            foreach (Shape shape in layer.Shapes)
                            {
                                if (shape is DXFViewer.Text)
                                {
                                    shape.Selectable = true;

                                    DXFViewer.Text text = (DXFViewer.Text)shape;

                                    if (shape.HitTest(vPos.x, vPos.y))
                                    {
                                        findText = true;
                                        HatchEx sensor = m_selectedShape;
                                        m_selectedShape.SetLink(text, text.Title, m_linkedColor);
                                        SetSelectedShape(null);
                                        labelStatus.Text = sensor.Name;
                                        break;
                                    }
                                }
                            }

                            if (findText)
                                break;
                        }
                    }

                    if (find == false)
                    {
                        SetSelectedShape(null);
                    }
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (m_bMoveText)
                {
                    m_vMoveOrigin = dxfSensors.ScreenToGlobal(e.X, e.Y);

                    if (m_vMoveOrigin == null)
                        m_bMoveText = false;
                    else
                    {
                        List<DXFViewer.Text> texts = m_dicSensorTextOrigin.Keys.ToList();

                        foreach (DXFViewer.Text text in texts)
                        {
                            m_dicSensorTextOrigin[text] = text.Position;
                        }
                    }
                }
            }
        }

        private void SetSelectedShape(HatchEx shape)
        {
            if (m_selectedShape == shape)
                return;

            if (m_selectedShape != null)
            {
                if (m_selectedShape.LinkedShape != null)
                    m_selectedShape.SetOwnColor(m_linkedColor);
                else
                    m_selectedShape.SetOwnColor(m_originalColor);
            }

            m_selectedShape = shape;

            if (m_selectedShape != null)
            {
                m_selectedShape.SetOwnColor(m_selectedColor);
                labelStatus.Text = m_selectedShape.Name;
            }
            else
                labelStatus.Text = "";

            dxfSensors._Refresh();
        }

        private void tsMenuExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "CSV Files|*.csv|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "센서 파일 저장";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string strFilePath = dlg.FileName;
                SaveCSV(strFilePath);
            }
        }

        private void SaveCSV(string strPath)
        {
            StreamWriter writer = new StreamWriter(strPath, false, Encoding.UTF8);
            writer.WriteLine("센서타입\t센서이름\tX\tY");

            foreach (LayerEx layer in m_sensorLayers)
            {
                foreach (HatchEx sensor in layer.Shapes)
                {
                    writer.Write(layer.NickName);
                    writer.Write("\t" + sensor.Name);
                    writer.Write("\t" + sensor.Position.x);
                    writer.WriteLine("\t" + sensor.Position.y);
                }
            }

            writer.Close();
        }

        private void dxfSensors_MouseMove(object sender, MouseEventArgs e)
        {
            Vertex2D vPos = dxfSensors.ScreenToGlobal(e.X, e.Y);

            if (vPos != null)
            {
                labelCoord.Text = string.Format("{0}, {1}", vPos.x, vPos.y);
            }

            if (m_bMoveText && m_vMoveOrigin != null)
            {
                List<DXFViewer.Text> texts = m_dicSensorTextOrigin.Keys.ToList();
                Vertex2D vMove = vPos - m_vMoveOrigin;
                System.Diagnostics.Trace.WriteLine("Move : " + vMove.x.ToString() + ", " + vMove.y.ToString());

                foreach (DXFViewer.Text text in texts)
                {
                    text.SetPosition(m_dicSensorTextOrigin[text] + vMove);
                }

                dxfSensors._Refresh();
            }
        }

        private void dxfSensors_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                m_bMoveText = false;
                m_vMoveOrigin = null;
            }
        }

        private void tsMenuMoveText_Click(object sender, EventArgs e)
        {
            m_bMoveText = true;
        }
    }

    class LayerEx : Layer
    {
        private string m_strNickName = "";

        public string NickName
        {
            get { return m_strNickName; }
            set { m_strNickName = value; }
        }

        public LayerEx(IPainter owner)
            : base(owner)
        {
        }
    }

    class HatchEx : Hatch
    {
        private Shape m_linkedShape = null;
        private string m_strName = "";

        public Shape LinkedShape
        {
            get { return m_linkedShape; }
            set { m_linkedShape = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public void SetLink(Shape link, string strName, Color color)
        {
            m_linkedShape = link;
            m_strName = strName;
            this.SetOwnColor(color);
        }
    }

    class VertexGroup
    {
        private List<Vertex2D> m_vertex = new List<Vertex2D>();

        public void AddVertex(Vertex2D vertex)
        {
            m_vertex.Add(vertex);
        }

        public Hatch MakeHatch(IPainter painter, Layer layer, Color color)
        {
            Vertex2D vTL = null, vBR = null;

            foreach (Vertex2D vertex in m_vertex)
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

            Vertex2D vTR = new Vertex2D(vBR.x, vTL.y);
            Vertex2D vBL = vBR - vTR + vTL;

            HatchEx hatch = new HatchEx();
            hatch.AddLine(vTL, vTR);
            hatch.AddLine(vTR, vBR);
            hatch.AddLine(vBR, vBL);
            hatch.AddLine(vBL, vTL);

            hatch.MakePath(0, 0);
            hatch.SetOwnColor(color);
            hatch.SetColorOption(Shape.ControlType.BYOWN);
            hatch.SetOwner(painter);
            hatch.Selectable = true;
            layer.Shapes.Add(hatch);

            return hatch;
        }

        public int GetVertexCount()
        {
            return m_vertex.Count;
        }
    }
}
