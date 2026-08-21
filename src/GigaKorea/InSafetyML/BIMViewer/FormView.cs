using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Geometry;
using System.IO;

namespace BIMViewer
{
    using Shapes;
    using BIM;

    public partial class FormView : Form, IGDIOwner, IComparable
    {
        private Project m_project = null;
        private List<Layer> m_layers = null;
        private List<DXFLayer> m_dxfLayers = null;
        private Level m_level = null;
        private double m_dMoveX = 0.0, m_dMoveY = 0.0;
        private FormWindowState m_lastState = FormWindowState.Normal;
        private IUIMaster m_master = null;
        private bool m_systemInput = false;

        private POILayer m_poiLayer = null;
        private WireLayer m_wireLayer = null;

        private static Size m_initSize = new Size(0, 0);

        public static Size InitSize
        {
            get { return m_initSize; }
        }

        public bool SystemInput
        {
            set { m_systemInput = value; }
        }

        public Level Level
        {
            get { return m_level; }
        }

        public Project Project
        {
            get { return m_project; }
        }

        public FormView(List<Layer> layers, List<DXFLayer> dxfLayers, Level level, IUIMaster master, Project project)
        {
            InitializeComponent();

            m_layers = layers;
            m_dxfLayers = dxfLayers;
            m_level = level;
            m_master = master;
            m_project = project;
        }

        public void TransferMouseWheel(Point location, int delta)
        {
            this.panelBody.TransferMouseWheel(location, delta);
        }

        public void SetTitle(string title)
        {
          //  this.Text = title;
           // return;

            if (m_level != null)
            {
                this.Text = m_level.Name + "-" + Path.GetFileNameWithoutExtension(m_project.LocalFilePath);
//                this.Text = m_level.Name.Substring(0, m_level.Name.IndexOf(" plan")) + "-" + Path.GetFileNameWithoutExtension(m_project.LocalFilePath);
                /*if (string.Compare("roof", m_level.LevelID, true) == 0)
                    this.Text = string.Format("지붕층 - {0}", m_level.Name);
                else if (m_level.FloorIndex < 0)
                    this.Text = string.Format("지하 {0}층 - {1}", -m_level.FloorIndex, m_level.Name);
                else
                    this.Text = string.Format("{0}층 - {1}", m_level.FloorIndex + 1, m_level.Name);*/
            }
        }

        private void FormView_Load(object sender, EventArgs e)
        {
            if (m_initSize.Width == 0)
                m_initSize = this.Size;

            //SetTitle();

            labelCoord.Text = "";
            panelBody.SetOwner(this);

            panelBody.Layers = m_layers;            
            panelBody.DXFLayers = m_dxfLayers;
            panelBody.Project = m_project;
            panelBody.Level = m_level;

            Vertex2D vObjectTL = null, vObjectBR = null;
            m_poiLayer = null;
            m_wireLayer = null;

            foreach (Layer layer in m_layers)
            {
                if (layer is WireLayer)
                {
                    m_wireLayer = (WireLayer)layer;
                    continue;
                }

                foreach (Shape shape in layer.Shapes)
                {
                    Vertex2D vTL = shape.TopLeft;
                    Vertex2D vBR = shape.BottomRight;

                    if (vObjectTL == null)
                    {
                        vObjectTL = new Vertex2D(vTL);
                        vObjectBR = new Vertex2D(vTL);
                    }
                    else
                    {
                        Wall.SetBoundaryVertex(vObjectTL, vObjectBR, vTL);
                    }

                    Wall.SetBoundaryVertex(vObjectTL, vObjectBR, vBR);

                    if (shape is POI)
                    {
                        POI poi = (POI)shape;
                        poi.Painter = panelBody;                        
                    }
                }

                if (layer is POILayer)
                {
                    m_poiLayer = (POILayer)layer;
                    checkBoxFixedPOIScale.Checked = m_poiLayer.IgnoreScale;
                }
            }

            Vertex2D vCenter = (vObjectTL + vObjectBR) / 2;

            double weight1 = panelBody.Size.Width * 0.85 / ((vCenter.x - vObjectTL.x) * 2);
            double weight2 = panelBody.Size.Height * 0.85 / ((vCenter.y - vObjectBR.y) * 2);
            double dViewportWeight = weight1 < weight2 ? weight1 : weight2;

            double dMoveX = -vObjectTL.x;
            double dMoveY = -vObjectBR.y;

            vCenter.x += dMoveX;
            vCenter.y += dMoveY;

            MoveAll(dMoveX, dMoveY);

            panelBody.SetViewportCenter(vCenter);
            panelBody.Zoom(dViewportWeight, vCenter, false);
        }

        private void MoveAll(double dMoveX, double dMoveY)
        {
            m_dMoveX = dMoveX;
            m_dMoveY = dMoveY;

            foreach (Layer layer in m_layers)
            {
                foreach (Shape shape in layer.Shapes)
                {
                    shape.Move(dMoveX, dMoveY);
                }
            }
        }

        public void GetMove(out double x, out double y)
        {
            x = m_dMoveX;
            y = m_dMoveY;
        }

        private void FormView_ResizeEnd(object sender, EventArgs e)
        {
            panelBody.Refresh();
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            if (m_systemInput)
            {
                m_lastState = this.WindowState;
                return;
            }
        
            if (m_lastState != this.WindowState)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    if (m_master != null)
                        m_master.SetTabMode(this, true);
                }
                else
                {
                    if (m_master != null)
                        m_master.SetTabMode(this, false);
                }

                panelBody.Refresh();
                m_lastState = this.WindowState;
            }
        }

        public void OnMouseMove(int screenX, int screenY, double panelX, double panelY)
        {
            labelCoord.Text = string.Format("좌표 : {0:F1},{1:F1}", panelX - m_dMoveX, panelY - m_dMoveY);
        }

        private void panelStatus_MouseDown(object sender, MouseEventArgs e)
        {
            FocusView();
        }

        public void FocusView()
        {
            BringToFront();
        }
        
        public bool IsPOIAddMode()
        {
            if (m_master != null)
            {
                return m_master.IsAddMode;
            }

            return false;
        }
        public bool IsPOIMoveMode()
        {
            if (m_master != null)
            {
                return m_master.IsMoveMode;
            }

            return false;
        }

        public bool IsPOIDeleteMode()
        {
            if (m_master != null)
            {
                return m_master.IsDeleteMode;
            }

            return false;
        }

        public bool IsPOIDoneMode()
        {
            if (m_master != null)
            {
                return m_master.IsDoneMode;
            }

            return false;
        }

        public bool IsWireAddMode()
        {
            if (m_master != null)
            {
                return m_master.IsAddModeWire;
            }

            return false;
        }

        public bool IsWireMoveMode()
        {
            if (m_master != null)
            {
                return m_master.IsMoveModeWire;
            }

            return false;
        }

        public bool IsWireDeleteMode()
        {
            if (m_master != null)
            {
                return m_master.IsDeleteModeWire;
            }

            return false;
        }

        public bool IsWireDoneMode()
        {
            if (m_master != null)
            {
                return m_master.IsDoneModeWire;
            }

            return false;
        }

        public bool IsPropertyMode()
        {
            if (m_master != null)
            {
                return m_master.IsPropertyMode;
            }

            return false;
        }

        public bool UpdatePOI(POI poi, bool isDelete)
        {
            if (m_master != null)
            {
#if DB_USE
                return m_master.UpdatePOIToDB(poi, m_level, isDelete);
#elif XML_USE
                return m_master.UpdatePOIToXML(poi, m_project, m_level, isDelete);
#endif
            }

            return false;
        }

        public bool UpdateWire(Wire wire, bool isDelete)
        {
            if (m_master != null)
            {
#if DB_USE
                return m_master.UpdateWireToDB(wire, isDelete);
#elif XML_USE
                return m_master.UpdateWireToXML(wire, m_project, m_level, isDelete);
#endif
            }

            return false;
        }

        public POIType SelectedPOI()
        {
            if (m_master != null)
                return m_master.SelectedPOI;

            return null;
        }

        public POITypeProperty SelectedWire()
        {
            if (m_master != null)
                return m_master.SelectedWire;

            return null;
        }

        public void RefreshView()
        {
            Refresh();
            panelBody.Refresh();
        }

        public void NoResizeReshape()
        {
            panelBody.NoResizeReshape();
        }

        private void FormView_FormClosing(object sender, FormClosingEventArgs e)
        {
            panelBody.UnSelectAll();
            m_master.RemoveBasePlanGrid();//ym.baseplan 그리드 삭제
        }

        public int CompareTo(object obj)
        {
            FormView frm = (FormView)obj;
            return this.Level.Elevation.CompareTo(frm.Level.Elevation);
        }

        public Shape GetSelectedShape()
        {
            return panelBody.GetSelectedShape();
        }

        public void OnSelectShape(Shape shape)
        {
            if (m_master != null)
                m_master.ShowShapeProperty(shape);
        }

        public DXFViewer.IPainter GetDXFPainter()
        {
            return panelBody;
        }

        //ym.
        public void SetMoveOrNot(int nIndex)
        {
            m_dxfLayers[nIndex].m_bMove = !m_dxfLayers[nIndex].m_bMove;
        }
        public bool GetMoveOrNot(int nIndex)
        {
            return m_dxfLayers[nIndex].m_bMove;
        }

        public void MoveDXF(Vertex2D vMove)
        {
            foreach (DXFLayer layer in m_dxfLayers)
            {
                if (layer.Hidden)
                    continue;
                if(layer.m_bMove)//ym. still or move
                    layer.MovedVertex = vMove;                
            }
        }

        private void checkBoxFixedPOIScale_CheckedChanged(object sender, EventArgs e)
        {
            if (m_poiLayer != null)
                m_poiLayer.IgnoreScale = checkBoxFixedPOIScale.Checked;

            if (m_wireLayer != null)
            {
                //m_wireLayer.IgnoreScale = checkBoxFixedPOIScale.Checked;
                //foreach (Shape item in m_wireLayer.Shapes)
                //{
                //    if (item is POI)
                //    {
                //        POI poi = (POI)item;
                //        poi.IgnoreScale = checkBoxFixedPOIScale.Checked;
                //    }
                //}
            }
        }

 
        public void SetOriginDXF()
        {
            foreach (DXFLayer layer in m_dxfLayers)
            {
                if (layer.Hidden)
                    continue;

                layer.OriginVertex = layer.OriginVertex + layer.MovedVertex;
                layer.MovedVertex = new Vertex2D();
            }
        }
    }

    public class DXFLayer : DXFViewer.Layer
    {
        private Vertex2D m_vOrigin = new Vertex2D();
        private Vertex2D m_vMove = new Vertex2D();

        private Vertex2D m_vScreenTL = new Vertex2D();
        private Vertex2D m_vScreenBR = new Vertex2D();

        public bool m_bMove = true;//move or still.ym

        public DXFLayer(DXFViewer.IPainter painter)
            : base(painter)
        {
        }

        public Vertex2D OriginVertex
        {
            get { return m_vOrigin; }
            set { m_vOrigin = value; }
        }

        public Vertex2D MovedVertex
        {
            get { return m_vMove; }
            set { m_vMove = value; }
        }

        public override bool Draw(Graphics g, bool bDrawText)
        {
            if (m_isHidden || m_isFrozen)
                return false;

            Vertex2D vCenter = Owner.GetViewportCenter();
            double dViewportWeight = Owner.GetViewportWeight();
            int nScreenWidth = Owner.GetScreenWidth();
            int nScreenHeight = Owner.GetScreenHeight();

            Vertex2D vScreenTL = Owner.ScreenToGlobal(0, 0);
            Vertex2D vScreenBR = Owner.ScreenToGlobal(nScreenWidth, nScreenHeight);

            Vertex2D vMove = m_vOrigin + m_vMove;
            m_vScreenTL = vScreenTL - vMove;
            m_vScreenBR = vScreenBR - vMove;

            if (vMove.x != 0.0 || vMove.y != 0.0)
            {
                g.TranslateTransform((float)vMove.x, (float)vMove.y);
            }

            bool result = base.Draw(g, bDrawText);

            if (vMove.x != 0.0 || vMove.y != 0.0)
            {
                g.TranslateTransform((float)-vMove.x, (float)-vMove.y);
            }

            return result;
        }

        public override bool DrawObject(DXFViewer.Shape shape, Graphics g, bool bDrawText)
        {
            if (shape.CheckClipBounds(g, m_vScreenTL, m_vScreenBR))
            {
                shape.Draw(g, bDrawText);
            }

            return true;
        }
    }
}
