using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using UnE.Geometry;

namespace CadToXML
{
    public partial class FormMain : Form
    {
        private string m_strIniPath = "data.opt";
        private int m_nPrevUnitType = -1;

        private const string SpaceLayerName = "Space";
        private const string WallBoundaryLayerName = "WallBoundary";
        private const string WallCenterLineLayerName = "WallCenterLine";
        private const string AlertAreaLayerName = "AlertArea";
        private const string TopologyNodeLayerName = "TopologyNode";
        private const string TopologyLinkLayerName = "TopologyLink";

        public FormMain()
        {
            InitializeComponent();

            //VertexLinkTest2();
            cboDXFUnit.SelectedIndex = 0;
            cboXMLUnit.SelectedIndex = 0;

            labelCoord.Text = "";

            //ym0729.추가속성들(구조벽,가벽,난간,기둥,방화문) 콤보박스 첫째항목
            cmbColumnMaterial.SelectedIndex = 0;
            cmbDoorPrtyYN.SelectedIndex = 0;
            cmbSwallPrtyMaterial.SelectedIndex = 0;
            cmbSwallPrtyFinMaterial.SelectedIndex = 0;
            cmbFwallPrtyMaterial.SelectedIndex = 0;
            cmbFwallPrtyFinMaterial.SelectedIndex = 0;
            cmbHwallMaterial.SelectedIndex = 0;
        }

        private void btnOpenDXF_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string strFileName = dlg.FileName;
                textBoxDXFPath.Text = strFileName;
            }
        }

        private void btnSaveXML_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "XML Files|*.xml|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "XML 저장";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                string strFileName = dlg.FileName;
                textBoxXMLPath.Text = strFileName;
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            // 변환 버전 체크
            Button btn = sender as Button;
            string strVersion = CommonString.XML_VERSION;

            if (btn != null && btn == btnTransfer2nd)
                strVersion = CommonString.XML_VERSION_2nd;

            string strDXFPath = textBoxDXFPath.Text.Trim();

            if (strDXFPath.Length == 0)
            {
                textBoxDXFPath.Focus();
                MessageBox.Show("DXF 파일 경로를 입력하세요.");
                return;
            }

            if (File.Exists(strDXFPath) == false)
            {
                textBoxDXFPath.Focus();
                MessageBox.Show(strDXFPath + "는 존재하지 않는 파일입니다.");
                return;
            }

            string strXMLPath = textBoxXMLPath.Text.Trim();

            if (strXMLPath.Length == 0)
            {
                textBoxXMLPath.Focus();
                MessageBox.Show("XML 파일 경로를 입력하세요.");
                return;
            }

            double dWallSHeight, dWallFHeight, dWallPHeight, dWallHHeight;
            double dWallSThick, dWallFThick, dWallPThick, dWallHThick;

            //ym0729
            string strSwallPrtyMaterial, strSwallPrtyFinMaterial, strFwallPrtyMaterial, strFwallPrtyFinMaterial, strPwallPrtyMaterial, strHwallPrtyMaterial;
            strSwallPrtyMaterial = cmbSwallPrtyMaterial.SelectedItem.ToString();
            strSwallPrtyFinMaterial = cmbSwallPrtyFinMaterial.SelectedItem.ToString();
            strFwallPrtyMaterial = cmbFwallPrtyMaterial.SelectedItem.ToString();
            strFwallPrtyFinMaterial = cmbFwallPrtyFinMaterial.SelectedItem.ToString();            
            strHwallPrtyMaterial = cmbHwallMaterial.SelectedItem.ToString();
            strPwallPrtyMaterial = "";//partition은 재질속성없음

            int iDoorPrtyYN = cmbDoorPrtyYN.SelectedIndex;
            string strColumnMaterial = cmbColumnMaterial.SelectedItem.ToString();

            if (!GetWallInfo(textBoxWallSHeight, textBoxWallSThick,  "벽체(구조벽)",  out dWallSHeight, out dWallSThick))
                return;
            if (!GetWallInfo(textBoxWallFHeight, textBoxWallFThick,  "벽체(가벽)",  out dWallFHeight, out dWallFThick))
                return;
            if (!GetWallInfo(textBoxWallPHeight, textBoxWallPThick,  "벽체(커튼월)", out dWallPHeight, out dWallPThick))
                return;
            if (!GetWallInfo(textBoxWallHHeight, textBoxWallHThick,  "벽체(난간)",  out dWallHHeight, out dWallHThick))
                return;

            string strProjectName = textBoxProjectName.Text.Trim();

            if (strProjectName.Length == 0 && checkBoxDXFNameToProjectName.Checked == false)
            {
                textBoxProjectName.Focus();
                MessageBox.Show("프로젝트 이름을 입력하세요.");
                return;
            }

            if (checkBoxDXFNameToProjectName.Checked)
            {
                int nDotIndex = strDXFPath.LastIndexOf('.');
                int nIndex1 = strDXFPath.LastIndexOf('\\');
                int nIndex2 = strDXFPath.LastIndexOf('/');

                int nSlashIndex = nIndex1 > nIndex2 ? nIndex1 : nIndex2;

                if (nSlashIndex >= 0 && nDotIndex > nSlashIndex)
                    strProjectName = strDXFPath.Substring(nSlashIndex + 1, nDotIndex - nSlashIndex - 1);
            }

            string strDoorHeight, strDoorElevation;
            string strWindowHeight, strWindowElevation;

            if (CheckValidation(textBoxDoorHeight, "문 높이", true, out strDoorHeight) == false)
                return;
            if (CheckValidation(textBoxDoorElevation, "벽체하단으로부터의 문 위치", false, out strDoorElevation) == false)
                return;
            if (CheckValidation(textBoxWindowHeight, "창문 높이", true, out strWindowHeight) == false)
                return;
            if (CheckValidation(textBoxWindowElevation, "벽체하단으로부터의 창문 위치", true, out strWindowElevation) == false)
                return;

            AnchorNode anchorNode = null;
            if (CheckAnchorNode(out anchorNode) == false)
                return;

            this.Cursor = Cursors.WaitCursor;

            float fOriginTolerance = UnE.Geometry.Math.HALF_TOLERANCE();
            UnE.Geometry.Math.SetHalfTolerance(1.0f);

            // 좌표 정확도를 낮춘다.
            double dMoveX, dMoveY;
            Project project = DXFManager.ReadFile(strDXFPath, out dMoveX, out dMoveY);

            UnE.Geometry.Math.SetHalfTolerance(fOriginTolerance);

            if (project != null)
            {
                project.AnchorNode = anchorNode;
                project.ProjectName = strProjectName;
                project.Date = DateTime.Now;
                project.Author = textBoxAuthor.Text.Trim();
                if (cboXMLUnit.SelectedIndex == 0)
                    project.Unit = "mm";
                else if (cboXMLUnit.SelectedIndex == 1)
                    project.Unit = "cm";
                else if (cboXMLUnit.SelectedIndex == 2)
                    project.Unit = "meter";

                // DXF와 XML의 단위가 다르면 단위변환을 해준다.
                SetScale(project);
                SetFloorElevation(project.Floors, dWallSHeight);

                //ym0729    
                SetWallInfo(project.Floors, strSwallPrtyMaterial, strSwallPrtyFinMaterial, strFwallPrtyMaterial, strFwallPrtyFinMaterial, strPwallPrtyMaterial, strHwallPrtyMaterial, dWallSHeight, dWallFHeight, dWallPHeight, dWallHHeight, dWallSThick, dWallFThick, dWallPThick, dWallHThick);                
                SetOpeningInfo(project.Floors, strDoorHeight, strDoorElevation, strWindowHeight, strWindowElevation, iDoorPrtyYN, strColumnMaterial, txtLevelHeight.Text);

                m_nCurrentFloorIndex = 0;
                btnPrev.Enabled = false;
                btnNext.Enabled = true;
                m_currentProject = project;
                m_dCurrentWeight = -1.0;

                if (m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex < 0)
                    labelFloorIndex.Text = string.Format("지하 {0}층", -m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex);
                else
                    labelFloorIndex.Text = string.Format("{0}층", m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex + 1);

                Floor prevFloor = null;
                Dictionary<Space, Topology.Node> prevSpaceTopologyNodes = null;

                foreach (Floor floor in project.Floors)
                {
                    if (floor.MakeShapes(project.LengthUnit) == false)
                    {
                        this.Cursor = Cursors.Arrow;

                        MessageBox.Show("실패하였습니다.");
                        return;
                    }
                    else
                    {
                        prevSpaceTopologyNodes = DXFManager.MakeTopology(floor, prevFloor, prevSpaceTopologyNodes);
                        DXFManager.SetTopologyIDs(floor);
                        prevFloor = floor;
                    }
                }

                Plot(project, dMoveX, dMoveY);
            }

            XMLManager mgr = new XMLManager();
            //result.xml의 poiTypes 한줄씩 poiTypes 읽어다가.
            Dictionary<int, POIType> dicPOITypes = new Dictionary<int, POIType>();
            
            string strXMLFilePOITypes = Application.StartupPath + "\\POIType.xml";
            if (strVersion == CommonString.XML_VERSION_2nd)
                strXMLFilePOITypes = Application.StartupPath + "\\POIType_2nd.xml";

            mgr.ReadProject(strXMLFilePOITypes, dicPOITypes);

            bool result = mgr.Save(strXMLPath, project, dicPOITypes, strVersion);

            this.Cursor = Cursors.Arrow;

            if (result == false)
                MessageBox.Show(mgr.ErrorMessage);
            else
                MessageBox.Show("파일이 생성되었습니다.");

            /*if (project != null && project.Floors.Count > 0)
            {
                for (int i = 0; i < project.Floors.Count; i++)
                {
                    FormWallLine frm = new FormWallLine(project.Floors[i]);
                    frm.ShowDialog();
                }
            }*/
        }

        private bool CheckAnchorNode(out AnchorNode anchorNode)
        {
            anchorNode = null;

            if (checkBoxUseAnchorNode.Checked == false)
                return true;

            UnE.Geometry.Vertex2D vGlobal = GetVertex(textBoxGlobal, "global 기준점");

            if (vGlobal == null)
                return false;

            UnE.Geometry.Vertex2D vLocal = GetVertex(textBoxLocal, "dxf 기준점");

            if (vLocal == null)
                return false;

            string strAngle;

            if (CheckValidation(textBoxAngle, "방위각", false, out strAngle) == false)
                return false;

            anchorNode = new AnchorNode();

            anchorNode.GlobalPosition = vGlobal;
            anchorNode.LocalPosition = vLocal;
            anchorNode.Angle = double.Parse(strAngle);
            anchorNode.GlobalUnitOfLength = (Project.UnitOfLength)cboGlobalUnit.SelectedIndex;

            return true;
        }

        private UnE.Geometry.Vertex2D GetVertex(TextBox textBox, string strTextBoxName)
        {
            string str = textBox.Text.Trim();

            if (str.Length == 0)
            {
                string strMessage = string.Format("{0} 항목이 비어있습니다.", strTextBoxName);
                textBox.Focus();
                MessageBox.Show(strMessage);
                return null;
            }

            string[] tokens = str.Split(',');

            if (tokens.Count() != 2)
            {
                string strMessage = string.Format("{0}은 x,y 형태이어야 합니다.", strTextBoxName);
                textBox.Focus();
                MessageBox.Show(strMessage);
                return null;
            }

            double x, y;

            if (double.TryParse(tokens[0].Trim(), out x) == false || double.TryParse(tokens[1].Trim(), out y) == false)
            {
                string strMessage = string.Format("{0}에는 숫자만 입력 가능합니다.", strTextBoxName);
                textBox.Focus();
                MessageBox.Show(strMessage);
                return null;
            }

            return new UnE.Geometry.Vertex2D(x, y);
        }

        private Wall GetSpaceWall(UnE.Geometry.Vertex2D vBegin, UnE.Geometry.Vertex2D vEnd, Space space, ref Color color)
        {
            Wall selectedWall = null;

            foreach (Wall wall in space.Walls)
            {
                if (wall.Begin.GetDistance(vBegin) < 0.1 && wall.End.GetDistance(vEnd) < 0.1)
                {
                    selectedWall = wall;
                    break;
                }
                else if (wall.End.GetDistance(vBegin) < 0.1 && wall.Begin.GetDistance(vEnd) < 0.1)
                {
                    selectedWall = wall;
                    break;
                }
            }

            if (selectedWall != null)
            {
                color = GetWallColor(selectedWall);
                /*switch (selectedWall.Type)
                {
                    case Wall.WallType.Structure:
                        color = panelStructureColor.BackColor;
                        break;
                    case Wall.WallType.Partition:
                        color = panelPartitionColor.BackColor;
                        break;
                    case Wall.WallType.NoSpace:
                        color = Color.FromArgb(255, 191, 0);
                        break;
                    case Wall.WallType.Fake:
                        color = panelFakeColor.BackColor;
                        break;
                    case Wall.WallType.Handrail:
                        color = panelHandrailColor.BackColor;
                        break;
                }*/
            }

            return null;
        }

        private Color GetWallColor(Wall wall)
        {
            Color color = Color.Yellow;

            switch (wall.Type)
            {
                case Wall.WallType.Structure:
                    color = panelStructureColor.BackColor;
                    break;
                case Wall.WallType.Partition:
                    color = panelPartitionColor.BackColor;
                    break;
                case Wall.WallType.NoSpace:
                    color = Color.FromArgb(255, 191, 0);
                    break;
                case Wall.WallType.Fake:
                    color = panelFakeColor.BackColor;
                    break;
                case Wall.WallType.Handrail:
                    color = panelHandrailColor.BackColor;
                    break;
                case Wall.WallType.CurtainWall:
                    color = Color.Cyan;  // 커튼월 색 변경
                    break;
            }

            return color;
        }

        private void Plot(Project project, double dMoveX, double dMoveY)
        {
            m_vCurrentMove.x = dMoveX;
            m_vCurrentMove.y = dMoveY;

            if (m_nCurrentFloorIndex < 0 || m_nCurrentFloorIndex >= project.Floors.Count)
                return;

            Floor floor = project.Floors[m_nCurrentFloorIndex];
            DXFViewer.Layer layer = null;

            if (dxfControl.Layers.Count == 0)
            {
                layer = new DXFViewer.Layer(dxfControl);
                dxfControl.Layers.Add(layer);
            }
            else
                layer = (DXFViewer.Layer)dxfControl.Layers[0];

            foreach (DXFViewer.Layer _layer in dxfControl.Layers)
            {
                _layer.RemoveAll();
            }

            layer.LayerName = WallCenterLineLayerName;
            layer.Hidden = !checkBoxWallCenterLine.Checked;

            DXFViewer.Layer spaceLayer = new DXFViewer.Layer(layer.Owner);
            spaceLayer.LayerName = SpaceLayerName;
            spaceLayer.LineColor = btnSpaceColor.BackColor;
            spaceLayer.Hidden = !checkBoxSpace.Checked;
            dxfControl.Layers.Add(spaceLayer);

            DXFViewer.Layer wallBoundaryLayer = new DXFViewer.Layer(layer.Owner);
            wallBoundaryLayer.LayerName = WallBoundaryLayerName;
            wallBoundaryLayer.LineColor = btnWallBoundaryColor.BackColor;
            wallBoundaryLayer.Hidden = !checkBoxWallBoundary.Checked;
            dxfControl.Layers.Add(wallBoundaryLayer);

            DXFViewer.Layer alertAreaLayer = new DXFViewer.Layer(layer.Owner);
            alertAreaLayer.LayerName = AlertAreaLayerName;
            alertAreaLayer.LineColor = btnAlertAreaColor.BackColor;      
            alertAreaLayer.Hidden = !checkBoxAlertArea.Checked;          
            dxfControl.Layers.Add(alertAreaLayer);

            DXFViewer.Layer topologyNodeLayer = new DXFViewer.Layer(layer.Owner);
            topologyNodeLayer.LayerName = TopologyNodeLayerName;
            topologyNodeLayer.LineColor = btnTopologyNodeColor.BackColor;
            topologyNodeLayer.Hidden = !checkBoxTopologyNode.Checked;
            dxfControl.Layers.Add(topologyNodeLayer);

            DXFViewer.Layer topologyLinkLayer = new DXFViewer.Layer(layer.Owner);
            topologyLinkLayer.LayerName = TopologyLinkLayerName;
            topologyLinkLayer.LineColor = btnTopologyLinkColor.BackColor;
            topologyLinkLayer.Hidden = !checkBoxTopologyLink.Checked;
            dxfControl.Layers.Add(topologyLinkLayer);

            UnE.Geometry.Vertex2D vTL = null;
            UnE.Geometry.Vertex2D vBR = null;
            //int nIndex = 0;

            foreach (Space space in floor.Spaces)
            {
                UnE.Geometry.Polygon polygon = space.GetPolygon();

                int nVertexCount = polygon.GetVertexCount();

                if (nVertexCount >= 2)
                {
                    UnE.Geometry.Vertex2D vPrev = polygon.GetVertex(0);

                    if (vTL == null)
                    {
                        vTL = new UnE.Geometry.Vertex2D(vPrev);
                        vBR = new UnE.Geometry.Vertex2D(vPrev);
                    }

                    Color color = Color.Yellow;
                    SetBoundary(vTL, vBR, vPrev);

                    for (int i = 1; i < nVertexCount; i++)
                    {
                        UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i);

                        Wall wall = GetSpaceWall(vPrev, vertex, space, ref color);

                        /*DXFViewer.Line line = new DXFViewer.Line(vPrev, vertex);

                        line.SetOwnColor(color);
                        line.SetColorOption(DXFViewer.Shape.ControlType.BYOWN);
                        layer.Add(line);*/

                        SetBoundary(vTL, vBR, vertex);
                        vPrev = vertex;
                    }
                }

                AddSpaceBoundary(space, spaceLayer);
            }

            foreach (AlertArea alertArea in floor.AlertAreas)
            {
                UnE.Geometry.Polygon polygon = alertArea.GetPolygon();

                int nVertexCount = polygon.GetVertexCount();

                if (nVertexCount >= 2)
                {
                    UnE.Geometry.Vertex2D vPrev = polygon.GetVertex(0);

                    if (vTL == null)
                    {
                        vTL = new UnE.Geometry.Vertex2D(vPrev);
                        vBR = new UnE.Geometry.Vertex2D(vPrev);
                    }

                    Color color = Color.Yellow;
                    SetBoundary(vTL, vBR, vPrev);

                    for (int i = 1; i < nVertexCount; i++)
                    {
                        UnE.Geometry.Vertex2D vertex = polygon.GetVertex(i);

                        SetBoundary(vTL, vBR, vertex);
                        vPrev = vertex;
                    }
                }

                AddAlertAreaBoundary(alertArea, alertAreaLayer);
            }

            foreach (Wall wall in floor.Walls)
            {
                AddWall(wall, layer, wallBoundaryLayer);
            }

            foreach (Topology topology in floor.Topologies)
            {
                AddTopology(topology, topologyNodeLayer, topologyLinkLayer);
            }

            if (vTL != null)
            {
                vTL.x -= dMoveX;
                vTL.y -= dMoveY;
                vBR.x -= dMoveX;
                vBR.y -= dMoveY;

                UnE.Geometry.Vertex2D vCenter = (vTL + vBR) / 2;

                dxfControl.MoveAll(-dMoveX, -dMoveY);
                dxfControl.SetViewportCenter(vCenter);

                UnE.Geometry.Vertex2D _vTL = dxfControl.ScreenToGlobal(0, 0);
                UnE.Geometry.Vertex2D _vBR = dxfControl.ScreenToGlobal(dxfControl.Width, dxfControl.Height);

                if (m_dCurrentWeight < 0.0)
                {
                    double dWeight = GetViewportWeight(vTL, vBR, _vTL, _vBR);
                    dxfControl.SetViewportWeight(dWeight);
                    m_dCurrentWeight = dWeight;
                }
                else
                    dxfControl.SetViewportWeight(m_dCurrentWeight);
            }

            /*DXFViewer.Viewport viewport = new DXFViewer.Viewport();
            viewport.TopLeft = new UnE.Geometry.Vertex2D(-38515.9750718318, -39235.9728528225);
            viewport.BottomLeft = new UnE.Geometry.Vertex2D(-38515.9750718318, -49338.8352609188);
            viewport.BottomRight = new UnE.Geometry.Vertex2D(-21421.2904257514, -49338.8352609188);
            viewport.F11 = (float)0.03117928;
            viewport.F12 = (float)0.0;
            viewport.F21 = (float)0.0;
            viewport.F22 = (float)-0.03117928;
            viewport.FDx = (float)1200.9;
            viewport.FDy = (float)-1223.349;
            viewport.Weight = 0.0311792823930338;

            dxfControl.LoadViewport(viewport, true);*/
            dxfControl._Refresh();

            btnNext.Location = new Point(labelFloorIndex.Location.X + labelFloorIndex.Size.Width + 5, btnNext.Location.Y);
        }

        private void AddTopology(Topology topology, DXFViewer.Layer nodeLayer, DXFViewer.Layer linkLayer)
        {
            double dRadius = 100;

            List<KeyValuePair<Topology.Node, Topology.Node>> nodeLinks = new List<KeyValuePair<Topology.Node, Topology.Node>>();

            foreach (Topology.Node node in topology.Nodes)
            {
                DXFViewer.Hatch hatch = new DXFViewer.Hatch();
                Arc2D arc = new Arc2D(new Vertex2D(node.X, node.Y), dRadius, 0.0, UnE.Geometry.Math._2PI(), true);
                hatch.AddArc(arc);

                nodeLayer.Add(hatch);

                foreach (Topology.Node linkNode in node.LinkedNodes)
                {
                    if (CheckLinks(node, linkNode, nodeLinks) == false)
                    {
                        DXFViewer.Line line = new DXFViewer.Line();
                        line.Begin = new Vertex2D(node.X, node.Y);
                        line.End = new Vertex2D(linkNode.X, linkNode.Y);

                        linkLayer.Add(line);

                        KeyValuePair<Topology.Node, Topology.Node> pair = new KeyValuePair<Topology.Node, Topology.Node>(node, linkNode);
                        nodeLinks.Add(pair);
                    }
                }
            }
        }

        private static bool CheckLinks(Topology.Node node1, Topology.Node node2, List<KeyValuePair<Topology.Node, Topology.Node>> nodeLinks)
        {
            foreach (KeyValuePair<Topology.Node, Topology.Node> pair in nodeLinks)
            {
                if ((pair.Key == node1 && pair.Value == node2) || (pair.Key == node2 && pair.Value == node1))
                    return true;
            }

            return false;
        }

        private void AddWall(Wall wall, DXFViewer.Layer centerLineLayer, DXFViewer.Layer boundaryLayer)
        {
            Wall.GridType gridType = wall.GetGridType();

            if (gridType == Wall.GridType.Line)
            {
                UnE.Geometry.Vertex2D vBegin = wall.GetBeginVertex();
                UnE.Geometry.Vertex2D vEnd = wall.GetEndVertex();

                DXFViewer.Line line = new DXFViewer.Line();
                line.Begin = new Vertex2D(vBegin.x, vBegin.y);
                line.End = new Vertex2D(vEnd.x, vEnd.y);

                line.SetOwnColor(GetWallColor(wall));
                line.SetColorOption(DXFViewer.Shape.ControlType.BYOWN);
                centerLineLayer.Add(line);
            }
            else if (gridType == Wall.GridType.Arc)
            {
                UnE.Geometry.Arc2D arc1 = wall.Arc;
                DXFViewer.Arc arc2 = new DXFViewer.Arc();

                if (arc1.IsClockWise())
                    arc2.BeginAngle = UnE.Geometry.Math.RadToDeg(arc1.GetBeginAngle());
                else
                    arc2.BeginAngle = UnE.Geometry.Math.RadToDeg(arc1.GetEndAngle());

                arc2.ArcAngle = UnE.Geometry.Math.RadToDeg(arc1.GetAngle());
                arc2.Radius = arc1.GetRadius();
                arc2.Center = new Vertex2D(arc1.GetCenter().x, arc1.GetCenter().y);

                if (arc1.IsClosed())
                    arc2.IsCircle = true;

                arc2.SetOwnColor(GetWallColor(wall));
                arc2.SetColorOption(DXFViewer.Shape.ControlType.BYOWN);
                centerLineLayer.Add(arc2);
            }
            else if (gridType == Wall.GridType.EArc)
            {
                UnE.Geometry.EArc2D earc1 = wall.EArc;
                DXFViewer.EArc earc2 = new DXFViewer.EArc();

                if (earc1.IsClockWise())
                    earc2.BeginAngle = UnE.Geometry.Math.RadToDeg(earc1.GetBeginAngle());
                else
                    earc2.BeginAngle = UnE.Geometry.Math.RadToDeg(earc1.GetEndAngle());

                earc2.EArcAngle = UnE.Geometry.Math.RadToDeg(earc1.GetAngle());
                earc2.TopLeft = new Vertex2D(earc1.GetTL().x, earc1.GetTL().y);
                earc2.BottomLeft = new Vertex2D(earc1.GetBL().x, earc1.GetBL().y);
                earc2.BottomRight = new Vertex2D(earc1.GetBR().x, earc1.GetBR().y);

                if (earc1.IsClosed())
                    earc2.IsEllipse = true;

                earc2.SetOwnColor(GetWallColor(wall));
                earc2.SetColorOption(DXFViewer.Shape.ControlType.BYOWN);
                centerLineLayer.Add(earc2);
            }

            if (wall.Boundary != null)
            {
                foreach (PathItem item in wall.Boundary)
                {
                    AddPathItem(item, boundaryLayer);
                }
            }
        }

        private void AddSpaceBoundary(Space space, DXFViewer.Layer layer)
        {
            //UnE.Geometry.Vertex2D vBegin, vEnd, vMiddle;

            foreach (PathItem item in space.Boundary)
            {
                AddPathItem(item, layer);
                /*PathItem.DrawType type = item.GetDrawType();

                if (type == PathItem.DrawType.Line)
                {
                    if (item.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                        continue;

                    DXFViewer.Line line = new DXFViewer.Line();
                    line.Begin = vBegin;
                    line.End = vEnd;
                    layer.Add(line);
                }
                else if (type == PathItem.DrawType.Arc)
                {
                    UnE.Geometry.Arc2D arc1 = (UnE.Geometry.Arc2D)item.GetEArc();
                    DXFViewer.Arc arc2 = new DXFViewer.Arc();

                    if (arc1.IsClockWise())
                        arc2.BeginAngle = UnE.Geometry.Math.RadToDeg(arc1.GetBeginAngle());
                    else
                        arc2.BeginAngle = UnE.Geometry.Math.RadToDeg(arc1.GetEndAngle());

                    arc2.ArcAngle = UnE.Geometry.Math.RadToDeg(arc1.GetAngle());
                    arc2.Radius = arc1.GetRadius();
                    arc2.Center = arc1.GetCenter();

                    if (arc1.IsClosed())
                        arc2.IsCircle = true;

                    layer.Add(arc2);
                }
                else if (type == PathItem.DrawType.EArc)
                {
                    UnE.Geometry.EArc2D earc1 = item.GetEArc();
                    DXFViewer.EArc earc2 = new DXFViewer.EArc();

                    if (earc1.IsClockWise())
                        earc2.BeginAngle = UnE.Geometry.Math.RadToDeg(earc1.GetBeginAngle());
                    else
                        earc2.BeginAngle = UnE.Geometry.Math.RadToDeg(earc1.GetEndAngle());

                    earc2.EArcAngle = UnE.Geometry.Math.RadToDeg(earc1.GetAngle());
                    earc2.TopLeft = earc1.GetTL();
                    earc2.BottomLeft = earc1.GetBL();
                    earc2.BottomRight = earc1.GetBR();

                    if (earc1.IsClosed())
                        earc2.IsEllipse = true;

                    layer.Add(earc2);
                }*/
            }
        }

        private void AddAlertAreaBoundary(AlertArea alertArea, DXFViewer.Layer layer)
        {
            foreach (PathItem item in alertArea.Boundary)
            {
                AddPathItem(item, layer); 
            }
        }

        private void AddPathItem(PathItem item, DXFViewer.Layer layer)
        {
            UnE.Geometry.Vertex2D vBegin, vEnd, vMiddle;
            PathItem.DrawType type = item.GetDrawType();

            if (type == PathItem.DrawType.Line)
            {
                if (item.GetVertex(out vBegin, out vEnd, out vMiddle) == false)
                    return;

                DXFViewer.Line line = new DXFViewer.Line();
                line.Begin = new Vertex2D(vBegin.x, vBegin.y);
                line.End = new Vertex2D(vEnd.x, vEnd.y);
                layer.Add(line);
            }
            else if (type == PathItem.DrawType.Arc)
            {
                UnE.Geometry.Arc2D arc1 = (UnE.Geometry.Arc2D)item.GetEArc();
                DXFViewer.Arc arc2 = new DXFViewer.Arc();

                if (arc1.IsClockWise())
                    arc2.BeginAngle = UnE.Geometry.Math.RadToDeg(arc1.GetBeginAngle());
                else
                    arc2.BeginAngle = UnE.Geometry.Math.RadToDeg(arc1.GetEndAngle());

                arc2.ArcAngle = UnE.Geometry.Math.RadToDeg(arc1.GetAngle());
                arc2.Radius = arc1.GetRadius();
                arc2.Center = new Vertex2D(arc1.GetCenter().x, arc1.GetCenter().y);

                if (arc1.IsClosed())
                    arc2.IsCircle = true;

                layer.Add(arc2);
            }
            else if (type == PathItem.DrawType.EArc)
            {
                UnE.Geometry.EArc2D earc1 = item.GetEArc();
                DXFViewer.EArc earc2 = new DXFViewer.EArc();

                if (earc1.IsClockWise())
                    earc2.BeginAngle = UnE.Geometry.Math.RadToDeg(earc1.GetBeginAngle());
                else
                    earc2.BeginAngle = UnE.Geometry.Math.RadToDeg(earc1.GetEndAngle());

                earc2.EArcAngle = UnE.Geometry.Math.RadToDeg(earc1.GetAngle());
                earc2.TopLeft = new Vertex2D(earc1.GetTL().x, earc1.GetTL().y);
                earc2.BottomLeft = new Vertex2D(earc1.GetBL().x, earc1.GetBL().y);
                earc2.BottomRight = new Vertex2D(earc1.GetBR().x, earc1.GetBR().y);

                if (earc1.IsClosed())
                    earc2.IsEllipse = true;

                layer.Add(earc2);
            }
        }

        private void SetBoundary(UnE.Geometry.Vertex2D vTL, UnE.Geometry.Vertex2D vBR, UnE.Geometry.Vertex2D vPos)
        {
            if (vTL.x > vPos.x)
                vTL.x = vPos.x;
            if (vTL.y < vPos.y)
                vTL.y = vPos.y;
            if (vBR.x < vPos.x)
                vBR.x = vPos.x;
            if (vBR.y > vPos.y)
                vBR.y = vPos.y;
        }

        private double GetViewportWeight(UnE.Geometry.Vertex2D vTL, UnE.Geometry.Vertex2D vBR, UnE.Geometry.Vertex2D _vTL, UnE.Geometry.Vertex2D _vBR)
        {
            double _width = _vBR.x - _vTL.x;
            double _height = _vTL.y - _vBR.y;
            double width = vBR.x - vTL.x;
            double height = vTL.y - vBR.y;
            double dRatio = 0.85;

            double dFullWidth = (width * dxfControl.Size.Width) / _width;
            double dFullHeight = (height * dxfControl.Size.Height) / _height;

            double dWeight1 = dxfControl.Size.Width / dFullWidth;
            double dWeight2 = dxfControl.Size.Height / dFullHeight;
            double dWeight = dWeight1 < dWeight2 ? dWeight1 : dWeight2;
            return dWeight * dRatio;
        }

        private void SetOpeningInfo(List<Floor> floors, string strDoorHeight, string strDoorElevation, string strWindowHeight, string strWindowElevation, int iDoorPrtyYN, string strColumnMaterial, string strLevelHeight)
        {
            double dDoorHeight = double.Parse(strDoorHeight);
            double dDoorElevation = double.Parse(strDoorElevation);
            double dWindowHeight = double.Parse(strWindowHeight);
            double dWindowElevation = double.Parse(strWindowElevation);
           
            //ym0729

            //방화문유무속성추가.ym            
            foreach (Floor floor in floors)
            {
                //0826.level Property height 추가
                Property tmpLevelPrty = new Property();
                tmpLevelPrty.Name = "Height";
                tmpLevelPrty.Value = strLevelHeight;
                floor.Properties.Add(tmpLevelPrty);

                foreach (Wall wall in floor.Walls)
                {                   
                    foreach (Door door in wall.Doors)
                    {
                        door.Height = dDoorHeight;
                        door.Elevation = dDoorElevation;

                        Property tmpDoorPrty1 = new Property();
                        tmpDoorPrty1.Name = "Thick";
                        tmpDoorPrty1.Value = door.Thick.ToString();
                        
                        Property tmpDoorPrty2 = new Property();
                        tmpDoorPrty2.Name = "방화문유무";
                        if (iDoorPrtyYN == 0)
                            tmpDoorPrty2.Value = "0";
                        else
                            tmpDoorPrty2.Value = "1";

                        door.Properties.Add(tmpDoorPrty1);
                        door.Properties.Add(tmpDoorPrty2);
                    }

                    foreach (Window window in wall.Windows)
                    {
                        window.Height = dWindowHeight;
                        window.Elevation = dWindowElevation;
                    }
                   
                }

                //0826.ym. space 방화구역 속성 추가
                Property tmpSpacePrty = new Property();
                tmpSpacePrty.Name = "방화구역유무";
                tmpSpacePrty.Value = "0";
                foreach (Space space in floor.Spaces)
                    space.Properties.Add(tmpSpacePrty);

                //기둥재질속성추가.ym
                Property tmpColulmPrty = new Property();
                tmpColulmPrty.Name = "재질";
                tmpColulmPrty.Value = strColumnMaterial;
                foreach(Column column in floor.Columns)
                    column.Properties.Add(tmpColulmPrty);
            }
        }

        private bool GetWallInfo(TextBox textBoxWallHeight, TextBox textBoxWallThick, string strWallName, out double dWallHeight, out double dWallThick)
        {            
            dWallHeight = dWallThick = 0.0;

            string strWallHeight, strWallThick;

            if (CheckValidation(textBoxWallHeight, strWallName + " 높이", true, out strWallHeight) == false)
                return false;

            dWallHeight = double.Parse(strWallHeight);

            if (CheckValidation(textBoxWallThick, strWallName + " 두께", true, out strWallThick) == false)
                return false;

            dWallThick = double.Parse(strWallThick);
            
            return true;
        }
        
        private void SetWallInfo(List<Floor> floors, string strSwallPrtyMaterial, string strSwallPrtyFinMaterial, string strFwallPrtyMaterial, string strFwallPrtyFinMaterial, string strPWallMaterial, string strHWallMaterial, double dWallSHeight, double dWallFHeight, double dWallPHeight, double dWallHHeight, double dWallSThick, double dWallFThick, double dWallPThick, double dWallHThick)
        {
            String strCWallMaterial = "유리벽";

            Material matS = new Material("Structure", strSwallPrtyMaterial); 
            Material matF = new Material("Fake", strFwallPrtyMaterial); 
            Material matP = new Material("Partition", strPWallMaterial);
            Material matH = new Material("Handrail", strHWallMaterial);
            Material matC = new Material("CurtainWall", strCWallMaterial);

            foreach (Floor floor in floors)
            {
                foreach (Wall wall in floor.Walls)
                {
                    if (wall.Type == Wall.WallType.Structure)
                    {
                        wall.Material = matS;
                        wall.Height = dWallSHeight;
                        wall.Thick = dWallSThick;

                        //ym0729
                        Property tmpPrtyMat = new Property();
                        tmpPrtyMat.Name = "재질";
                        tmpPrtyMat.Value = strSwallPrtyMaterial;
                        wall.Properties.Add(tmpPrtyMat);

                        Property tmpPrtyFin = new Property();
                        tmpPrtyFin.Name = "마감재";
                        tmpPrtyFin.Value = strSwallPrtyFinMaterial;
                        wall.Properties.Add(tmpPrtyFin);                     

                    }
                    else if (wall.Type == Wall.WallType.Fake || wall.Type == Wall.WallType.NoSpace)
                    {
                        wall.Material = matF;
                        wall.Height = dWallFHeight;
                        wall.Thick = dWallFThick;

                        //ym0729
                        Property tmpPrtyMat = new Property();
                        tmpPrtyMat.Name = "재질";
                        tmpPrtyMat.Value = strFwallPrtyMaterial;
                        wall.Properties.Add(tmpPrtyMat);

                        Property tmpPrtyFin = new Property();
                        tmpPrtyFin.Name = "마감재";
                        tmpPrtyFin.Value = strFwallPrtyFinMaterial;
                        wall.Properties.Add(tmpPrtyFin);
                    }
                    else if (wall.Type == Wall.WallType.Partition)
                    {
                        wall.Material = matP;
                        wall.Height = dWallPHeight;
                        wall.Thick = dWallPThick;
                    }
                    else if (wall.Type == Wall.WallType.CurtainWall)
                    {
                        // 커튼월 속성정보 >> 높이는 구조벽과 동일, 두께는 5cm로 고정
                        // 
                        wall.Material = matC;
                        wall.Height = dWallSHeight;
                        //wall.Thick = dWallSThick;
                        wall.Thick = 50;
                    }
                    else// if (wall.Type == Wall.WallType.Handrail)
                    {
                        wall.Material = matH;
                        wall.Height = dWallHHeight;
                        wall.Thick = dWallHThick;
                        //ym
                        Property tmpPrty = new Property();
                        tmpPrty.Name = "재질";
                        tmpPrty.Value = strHWallMaterial;
                        wall.Properties.Add(tmpPrty);
                    }
                }
            }
        }

        private void SetScale(Project project)
        {
            int nDXF = cboDXFUnit.SelectedIndex;
            int nXML = cboXMLUnit.SelectedIndex;

            if (nDXF == nXML)
                return;

            double dScale = 1.0;

            if (nDXF == 0)
            {
                if (nXML == 1)
                    dScale = 0.1;
                else if (nXML == 2)
                    dScale = 0.001;
            }
            else if (nDXF == 1)
            {
                if (nXML == 0)
                    dScale = 10.0;
                else if (nXML == 2)
                    dScale = 0.001;
            }
            else if (nDXF == 2)
            {
                if (nXML == 0)
                    dScale = 1000.0;
                else if (nXML == 1)
                    dScale = 100;
            }

            if (nXML == 0)
                project.Unit = "mm";
            else if (nXML == 1)
                project.Unit = "cm";
            else if (nXML == 2)
                project.Unit = "meter";

            List<Floor> floors = project.Floors;

            foreach (Floor floor in floors)
            {
                floor.SetScale(dScale, true);
            }
        }

        private void SetFloorElevation(List<Floor> floors, double dFloorHeight)
        {
            if (floors.Count == 1)
            {
                floors[0].Elevation = 0.0;
            }
            else
            {
                int nBaseIndex = 0;

                for (int i=0;i<floors.Count;i++)
                {
                    Floor floor = floors[i];

                    if (floor.FloorIndex == 0)
                    {
                        nBaseIndex = i;
                        break;
                    }
                }

                for (int i=0;i<floors.Count;i++)
                {
                    Floor floor = floors[i];
                    floor.Elevation = dFloorHeight * (i - nBaseIndex);
                }
            }
        }

        /*void VertexLinkTest2()
        {
            UnE.Geometry.Vertex2D v1 = new UnE.Geometry.Vertex2D(0, 200);
            UnE.Geometry.Vertex2D v2 = new UnE.Geometry.Vertex2D(100, 200);
            UnE.Geometry.Vertex2D v3 = new UnE.Geometry.Vertex2D(200, 200);
            UnE.Geometry.Vertex2D v4 = new UnE.Geometry.Vertex2D(300, 200);

            UnE.Geometry.Vertex2D v5 = new UnE.Geometry.Vertex2D(0, 0);
            UnE.Geometry.Vertex2D v6 = new UnE.Geometry.Vertex2D(300, 0);

            UnE.Geometry.Vertex2D v7 = new UnE.Geometry.Vertex2D(100, 190);
            UnE.Geometry.Vertex2D v8 = new UnE.Geometry.Vertex2D(100, 180);
            UnE.Geometry.Vertex2D v9 = new UnE.Geometry.Vertex2D(100, 170);
            UnE.Geometry.Vertex2D v10 = new UnE.Geometry.Vertex2D(100, 160);

            UnE.Geometry.Vertex2D v11 = new UnE.Geometry.Vertex2D(200, 190);
            UnE.Geometry.Vertex2D v12 = new UnE.Geometry.Vertex2D(200, 180);
            UnE.Geometry.Vertex2D v13 = new UnE.Geometry.Vertex2D(200, 170);
            UnE.Geometry.Vertex2D v14 = new UnE.Geometry.Vertex2D(200, 160);


            UnE.Geometry.Line2D line1 = new UnE.Geometry.Line2D(v1, v2);
            UnE.Geometry.Line2D line2 = new UnE.Geometry.Line2D(v2, v3);
            UnE.Geometry.Line2D line3 = new UnE.Geometry.Line2D(v3, v4);

            UnE.Geometry.Line2D line4 = new UnE.Geometry.Line2D(v1, v5);
            UnE.Geometry.Line2D line5 = new UnE.Geometry.Line2D(v4, v6);

            UnE.Geometry.Line2D line6 = new UnE.Geometry.Line2D(v5, v6);

            UnE.Geometry.Line2D line7 = new UnE.Geometry.Line2D(v2, v7);
            UnE.Geometry.Line2D line8 = new UnE.Geometry.Line2D(v7, v8);
            UnE.Geometry.Line2D line9 = new UnE.Geometry.Line2D(v8, v9);
            UnE.Geometry.Line2D line10 = new UnE.Geometry.Line2D(v9, v10);

            UnE.Geometry.Line2D line11 = new UnE.Geometry.Line2D(v3, v11);
            UnE.Geometry.Line2D line12 = new UnE.Geometry.Line2D(v11, v12);
            UnE.Geometry.Line2D line13 = new UnE.Geometry.Line2D(v12, v13);
            UnE.Geometry.Line2D line14 = new UnE.Geometry.Line2D(v13, v14);
            
            Geometry.PolygonBuilder builder = new Geometry.PolygonBuilder();

            builder.Lines.Add(line1);
            builder.Lines.Add(line2);
            builder.Lines.Add(line3);
            builder.Lines.Add(line4);
            builder.Lines.Add(line5);
            builder.Lines.Add(line6);
            builder.Lines.Add(line7);
            builder.Lines.Add(line8);
            builder.Lines.Add(line9);
            builder.Lines.Add(line10);
            builder.Lines.Add(line11);
            builder.Lines.Add(line12);
            builder.Lines.Add(line13);
            builder.Lines.Add(line14);

            List<UnE.Geometry.Line2D> lines = null;
            List<UnE.Geometry.Polygon> polygons = builder.MakePolygon(out lines);
            System.Diagnostics.Trace.WriteLine("Polygon Count : " + polygons.Count);
        }

        void VertexLinkTest()
        {
            UnE.Geometry.Vertex2D v1 = new UnE.Geometry.Vertex2D(0, 200);
            UnE.Geometry.Vertex2D v2 = new UnE.Geometry.Vertex2D(100, 200);
            UnE.Geometry.Vertex2D v3 = new UnE.Geometry.Vertex2D(200, 200);
            UnE.Geometry.Vertex2D v4 = new UnE.Geometry.Vertex2D(300, 200);
            UnE.Geometry.Vertex2D v5 = new UnE.Geometry.Vertex2D(400, 200);

            UnE.Geometry.Vertex2D v6 = new UnE.Geometry.Vertex2D(0, 100);
            UnE.Geometry.Vertex2D v7 = new UnE.Geometry.Vertex2D(50, 100);
            UnE.Geometry.Vertex2D v8 = new UnE.Geometry.Vertex2D(100, 100);
            UnE.Geometry.Vertex2D v9 = new UnE.Geometry.Vertex2D(150, 100);
            UnE.Geometry.Vertex2D v10 = new UnE.Geometry.Vertex2D(200, 100);
            UnE.Geometry.Vertex2D v11 = new UnE.Geometry.Vertex2D(250, 100);
            UnE.Geometry.Vertex2D v12 = new UnE.Geometry.Vertex2D(300, 100);
            UnE.Geometry.Vertex2D v13 = new UnE.Geometry.Vertex2D(350, 100);
            UnE.Geometry.Vertex2D v14 = new UnE.Geometry.Vertex2D(400, 100);

            UnE.Geometry.Vertex2D v15 = new UnE.Geometry.Vertex2D(200, 50);

            UnE.Geometry.Vertex2D v16 = new UnE.Geometry.Vertex2D(50, 0);
            UnE.Geometry.Vertex2D v17 = new UnE.Geometry.Vertex2D(150, 0);
            UnE.Geometry.Vertex2D v18 = new UnE.Geometry.Vertex2D(250, 0);
            UnE.Geometry.Vertex2D v19 = new UnE.Geometry.Vertex2D(350, 0);

            UnE.Geometry.Vertex2D v20 = new UnE.Geometry.Vertex2D(50, -100);
            UnE.Geometry.Vertex2D v21 = new UnE.Geometry.Vertex2D(350, -100);

            UnE.Geometry.Line2D line1 = new UnE.Geometry.Line2D(v1, v2);
            UnE.Geometry.Line2D line2 = new UnE.Geometry.Line2D(v2, v3);
            UnE.Geometry.Line2D line3 = new UnE.Geometry.Line2D(v3, v4);
            UnE.Geometry.Line2D line4 = new UnE.Geometry.Line2D(v4, v5);

            UnE.Geometry.Line2D line5 = new UnE.Geometry.Line2D(v1, v6);
            UnE.Geometry.Line2D line6 = new UnE.Geometry.Line2D(v2, v7);
            UnE.Geometry.Line2D line7 = new UnE.Geometry.Line2D(v2, v8);
            UnE.Geometry.Line2D line8 = new UnE.Geometry.Line2D(v2, v9);
            UnE.Geometry.Line2D line9 = new UnE.Geometry.Line2D(v3, v10);
            UnE.Geometry.Line2D line10 = new UnE.Geometry.Line2D(v4, v11);
            UnE.Geometry.Line2D line11 = new UnE.Geometry.Line2D(v4, v12);
            UnE.Geometry.Line2D line12 = new UnE.Geometry.Line2D(v4, v13);
            UnE.Geometry.Line2D line13 = new UnE.Geometry.Line2D(v5, v14);

            UnE.Geometry.Line2D line14 = new UnE.Geometry.Line2D(v6, v7);
            UnE.Geometry.Line2D line15 = new UnE.Geometry.Line2D(v7, v8);
            UnE.Geometry.Line2D line16 = new UnE.Geometry.Line2D(v8, v9);
            UnE.Geometry.Line2D line17 = new UnE.Geometry.Line2D(v9, v10);
            UnE.Geometry.Line2D line18 = new UnE.Geometry.Line2D(v10, v11);
            UnE.Geometry.Line2D line19 = new UnE.Geometry.Line2D(v11, v12);
            UnE.Geometry.Line2D line20 = new UnE.Geometry.Line2D(v12, v13);
            UnE.Geometry.Line2D line21 = new UnE.Geometry.Line2D(v13, v14);

            UnE.Geometry.Line2D line22 = new UnE.Geometry.Line2D(v7, v20);
            UnE.Geometry.Line2D line23 = new UnE.Geometry.Line2D(v9, v17);
            UnE.Geometry.Line2D line24 = new UnE.Geometry.Line2D(v11, v18);
            UnE.Geometry.Line2D line25 = new UnE.Geometry.Line2D(v13, v21);

            UnE.Geometry.Line2D line26 = new UnE.Geometry.Line2D(v15, v17);
            UnE.Geometry.Line2D line27 = new UnE.Geometry.Line2D(v15, v18);

            UnE.Geometry.Line2D line28 = new UnE.Geometry.Line2D(v16, v17);
            UnE.Geometry.Line2D line29 = new UnE.Geometry.Line2D(v17, v18);
            UnE.Geometry.Line2D line30 = new UnE.Geometry.Line2D(v18, v19);

            Geometry.PolygonBuilder builder = new Geometry.PolygonBuilder();

            builder.Lines.Add(line1);
            builder.Lines.Add(line2);
            builder.Lines.Add(line3);
            builder.Lines.Add(line4);
            builder.Lines.Add(line5);
            builder.Lines.Add(line6);
            builder.Lines.Add(line7);
            builder.Lines.Add(line8);
            builder.Lines.Add(line9);
            builder.Lines.Add(line10);
            builder.Lines.Add(line11);
            builder.Lines.Add(line12);
            builder.Lines.Add(line13);
            builder.Lines.Add(line14);
            builder.Lines.Add(line15);
            builder.Lines.Add(line16);
            builder.Lines.Add(line17);
            builder.Lines.Add(line18);
            builder.Lines.Add(line19);
            builder.Lines.Add(line20);
            builder.Lines.Add(line21);
            builder.Lines.Add(line22);
            builder.Lines.Add(line23);
            builder.Lines.Add(line24);
            builder.Lines.Add(line25);
            builder.Lines.Add(line26);
            builder.Lines.Add(line27);
            builder.Lines.Add(line28);
            builder.Lines.Add(line29);
            builder.Lines.Add(line30);

            List<UnE.Geometry.Line2D> lines = null;
            List<UnE.Geometry.Polygon> polygons = builder.MakePolygon(out lines);
            System.Diagnostics.Trace.WriteLine("Polygon Count : " + polygons.Count);
        }*/

        private bool CheckValidation(TextBox textBox, string strControlName, bool overZero, out string strText)
        {
            strText = textBox.Text.Trim();

            if (strText.Length == 0)
            {
                textBox.Focus();
                MessageBox.Show(strControlName + "를 입력하세요.");
                return false;
            }

            double data;

            if (double.TryParse(strText, out data) == false)
            {
                textBox.Focus();
                MessageBox.Show(strControlName + "는 숫자이어야만 합니다.");
                return false;
            }

            if (overZero && data < 0.0)
            {
                textBox.Focus();
                MessageBox.Show(strControlName + "는 0보다 큰 숫자이어야만 합니다.");
                return false;
            }

            return true;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            StreamWriter writer = new StreamWriter(m_strIniPath, false, Encoding.UTF8);

            if (checkBoxRemember.Checked)
                writer.WriteLine("1 " + textBoxXMLPath.Text.Trim());
            else
                writer.WriteLine("0");

            if (checkBoxDXFNameToProjectName.Checked)
                writer.WriteLine("1");
            else
                writer.WriteLine("0");

            writer.Close();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            labelFloorIndex.Text = "";

            if (File.Exists(m_strIniPath))
            {
                StreamReader reader = new StreamReader(m_strIniPath, Encoding.UTF8);
                string strLine = reader.ReadLine().Trim();
                
                int nIndex = strLine.IndexOf(' ');

                if (nIndex >= 0)
                {
                    string strCheckedOption = strLine.Substring(0, nIndex);

                    if (strCheckedOption == "1")
                        checkBoxRemember.Checked = true;

                    string strPath = strLine.Substring(nIndex + 1).Trim();
                    textBoxXMLPath.Text = strPath;
                }
                else
                {
                    if (strLine == "1")
                        checkBoxRemember.Checked = true;
                }

                if (reader.EndOfStream == false)
                {
                    strLine = reader.ReadLine().Trim();

                    if (strLine == "1")
                        checkBoxDXFNameToProjectName.Checked = true;
                }

                reader.Close();
            }

            cboGlobalUnit.SelectedIndex = 0;
        }

        private void cboXMLUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboXMLUnit.SelectedIndex == 0)
                SetUnit("mm");
            else if (cboXMLUnit.SelectedIndex == 1)
                SetUnit("cm");
            else if (cboXMLUnit.SelectedIndex == 2)
                SetUnit("m");
        }

        private void SetUnit(string strUnit)
        {
            labelUnitDoorHeight.Text = strUnit;
            labelUnitDoorElevation.Text = strUnit;
            labelUnitWallFHeight.Text = strUnit;
            labelUnitWallFThick.Text = strUnit;
            labelUnitWallPHeight.Text = strUnit;
            labelUnitWallPHeight.Text = strUnit;
            labelUnitWallPThick.Text = strUnit;
            labelUnitWallSHeight.Text = strUnit;
            labelUnitWallSThick.Text = strUnit;
            labelUnitWallHHeight.Text = strUnit;
            labelUnitWallHThick.Text = strUnit;
            labelUnitWindowElevation.Text = strUnit;
            labelUnitWindowHeight.Text = strUnit;
            labelUnitFloorHeight.Text = strUnit;

            if (m_nPrevUnitType < 0)
                m_nPrevUnitType = cboXMLUnit.SelectedIndex;
            else
            {
                if (m_nPrevUnitType == cboXMLUnit.SelectedIndex)
                    return;

                ChangeUnitNumbers(m_nPrevUnitType, cboXMLUnit.SelectedIndex);
                m_nPrevUnitType = cboXMLUnit.SelectedIndex;
            }
        }

        private void ChangeUnitNumbers(int nOldUnitType, int nNewUnitType)
        {
            int op = 1;

            // mm
            if (nOldUnitType == 0)
            {
                // cm
                if (nNewUnitType == 1)
                    op = -10;
                // m
                else// if (nNewUnitType == 2)
                    op = -1000;
            }
            // cm
            else if (nOldUnitType == 1)
            {
                // mm
                if (nNewUnitType == 0)
                    op = 10;
                // m
                else// if (nNewUnitType == 2)
                    op = -100;
            }
            // m
            else// if (nOldUnitType == 2)
            {
                // mm
                if (nNewUnitType == 0)
                    op = 1000;
                // cm
                else// if (nNewUnitType == 1)
                    op = 100;
            }

            ChangeUnitNumber(textBoxDoorHeight, op);
            ChangeUnitNumber(textBoxDoorElevation, op);
            ChangeUnitNumber(textBoxWindowHeight, op);
            ChangeUnitNumber(textBoxWindowElevation, op);
            ChangeUnitNumber(textBoxWallSHeight, op);
            ChangeUnitNumber(textBoxWallSThick, op);
            ChangeUnitNumber(textBoxWallFHeight, op);
            ChangeUnitNumber(textBoxWallFThick, op);
            ChangeUnitNumber(textBoxWallPHeight, op);
            ChangeUnitNumber(textBoxWallPThick, op);
            ChangeUnitNumber(textBoxWallHHeight, op);
            ChangeUnitNumber(textBoxWallHThick, op);
            ChangeUnitNumber(txtLevelHeight, op);
        }

        private void ChangeUnitNumber(TextBox textBox, int op)
        {
            string strNumber = textBox.Text.Trim();

            double num;

            if (double.TryParse(strNumber, out num))
            {
                if (op > 0)
                    num *= op;
                else
                    num /= -op;

                textBox.Text = DoubleToString(num);
            }

            /*int num;

            if (int.TryParse(strNumber, out num))
            {
                if (op > 0)
                    num *= op;
                else
                    num /= -op;

                textBox.Text = num.ToString();
            }*/
        }

        private string DoubleToString(double num)
        {
            string str = string.Format("{0:F3}", num);

            int nDotIndex = str.IndexOf('.');

            if (nDotIndex < 0)
                return str;

            for (int i=str.Length-1;i>nDotIndex;i--)
            {
                char ch = str.ElementAt(i);

                if (ch != '0')
                    return str.Substring(0, i + 1);
            }

            return str.Substring(0, nDotIndex);
        }

        private int m_nCurrentFloorIndex = -1;
        private UnE.Geometry.Vertex2D m_vCurrentMove = new UnE.Geometry.Vertex2D();
        private Project m_currentProject = null;
        private double m_dCurrentWeight = -1.0;

        //private int m_nCurrentPolylineIndex = -1;

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (m_currentProject == null)
                return;

            m_nCurrentFloorIndex--;

            if (m_nCurrentFloorIndex < 0)
                labelFloorIndex.Text = "";
            else
            {
                if (m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex < 0)
                    labelFloorIndex.Text = string.Format("지하 {0}층", -m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex);
                else
                {
                    if (m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex == 10000)
                        labelFloorIndex.Text = "지붕층";
                    else
                        labelFloorIndex.Text = string.Format("{0}층", m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex + 1);
                }
            }

            if (m_nCurrentFloorIndex < 0)
                btnPrev.Enabled = false;
            else
                btnPrev.Enabled = true;

            btnNext.Enabled = true;
            Plot(m_currentProject, m_vCurrentMove.x, m_vCurrentMove.y);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (m_currentProject == null)
                return;

            if (m_currentProject.Floors.Count - 1 <= m_nCurrentFloorIndex)
                return;

            m_nCurrentFloorIndex++;

            if (m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex < 0)
                labelFloorIndex.Text = string.Format("지하 {0}층", -m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex);
            else
            {
                if (m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex == 10000)
                    labelFloorIndex.Text = "지붕층";
                else
                    labelFloorIndex.Text = string.Format("{0}층", m_currentProject.Floors[m_nCurrentFloorIndex].FloorIndex + 1);
            }

            if (m_nCurrentFloorIndex >= m_currentProject.Floors.Count - 1)
                btnNext.Enabled = false;
            else
                btnNext.Enabled = true;

            btnPrev.Enabled = true;
            Plot(m_currentProject, m_vCurrentMove.x, m_vCurrentMove.y);
        }

        private void dxfControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_currentProject != null)
            {
                /*UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(e.X, e.Y);

                if (vertex != null)
                {
                    UnE.Geometry.Vertex2D vMove = dxfControl1.MovedVertex;
                    float fFlag = FormMain2.Instance.GetUnitFlag(DXFViewer.UnitOfLength.METER);
                    FormMain2.Instance.StatusText = string.Format("{0}, {1}, 단위(m)", (vertex.x - vMove.x) * fFlag, (vertex.y - vMove.y) * fFlag);
                }*/
                double x, y;

                if (GetDXFCoord(e.X, e.Y, out x, out y))
                {
                    labelCoord.Text = string.Format("{0}, {1}, 단위(" + m_currentProject.Unit + ")", x, y);
                }
            }
            else
                labelCoord.Text = "";
        }

        private bool GetDXFCoord(int x, int y, out double _x, out double _y)
        {
            _x = _y = 0.0;
            UnE.Geometry.Vertex2D vertex = dxfControl.ScreenToGlobal(x, y);

            if (vertex != null)
            {
                UnE.Geometry.Vertex2D vMove = dxfControl.MovedVertex;
                _x = (vertex.x - vMove.x);
                _y = (vertex.y - vMove.y);

                return true;
            }

            return false;
        }

        private void btnExportIndoorGML_Click(object sender, EventArgs e)
        {
            string strXML = textBoxXMLPath.Text.Trim();
            int nIndex = strXML.LastIndexOf('\\');

            string strFileName = strXML.Substring(nIndex + 1);
            nIndex = strFileName.LastIndexOf('.');

            if (nIndex >= 0)
            {
                strFileName = strFileName.Substring(0, nIndex);
            }

            strFileName = strFileName + ".gml";

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "XmlToIndoorGml.exe";
            startInfo.ErrorDialog = true;
            startInfo.Arguments = string.Format("\"{0}\" \"{1}\"", strXML, strFileName);
            /*startInfo.FileName = "IndoorGMLConverter.exe";
            startInfo.ErrorDialog = true;
            startInfo.Arguments = string.Format("-i \"{0}\" -o \"{1}\"", strXML, strFileName);*/

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);
                MessageBox.Show("IndoorGML 생성성공 : " + strFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("IndoorGML 생성실패 : " + ex.Message);
            }
        }

        private void btnLayerColor_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            ColorDialog dlg = new ColorDialog();

            dlg.Color = btn.BackColor;

            if (dlg.ShowDialog() == DialogResult.Cancel)
                return;

            btn.BackColor = dlg.Color;
            string strLayerName = "";

            if (btn == btnWallBoundaryColor)
            {
                strLayerName = WallBoundaryLayerName;
            }
            else if (btn == btnSpaceColor)
            {
                strLayerName = SpaceLayerName;
            }
            else if (btn == btnTopologyNodeColor)
            {
                strLayerName = TopologyNodeLayerName;
            }
            else if (btn == btnTopologyLinkColor)
            {
                strLayerName = TopologyLinkLayerName;
            }
            else
                return;

            foreach (DXFViewer.Layer layer in dxfControl.Layers)
            {
                if (layer.LayerName == strLayerName)
                {
                    layer.LineColor = btn.BackColor;
                    dxfControl._Refresh();
                    break;
                }
            }
        }

        private void checkBoxLayer_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            bool isHidden = !checkBox.Checked;
            string strLayerName = "";

            if (checkBox == checkBoxWallCenterLine)
            {
                strLayerName = WallCenterLineLayerName;
            }
            else if (checkBox == checkBoxWallBoundary)
            {
                strLayerName = WallBoundaryLayerName;
            }
            else if (checkBox == checkBoxSpace)
            {
                strLayerName = SpaceLayerName;
            }
            else if (checkBox == checkBoxTopologyNode)
            {
                strLayerName = TopologyNodeLayerName;
            }
            else if (checkBox == checkBoxTopologyLink)
            {
                strLayerName = TopologyLinkLayerName;
            }
            else if (checkBox == checkBoxAlertArea)
            {
                strLayerName = AlertAreaLayerName;
            }
            else
                return;

            foreach (DXFViewer.Layer layer in dxfControl.Layers)
            {
                if (layer.LayerName == strLayerName)
                {
                    layer.Hidden = isHidden;
                    dxfControl._Refresh();
                    break;
                }
            }
        }

        private void btnChangeXML_Click(object sender, EventArgs e)
        {
            AnchorNode anchorNode = null;
            
            if (checkBoxUseAnchorNode.Checked)
            {
                DialogResult result = MessageBox.Show("변환시 AnchorNode 정보를 설정하시겠습니까?", "확인", MessageBoxButtons.YesNoCancel);

                if (result == DialogResult.Cancel)
                    return;

                if (result == DialogResult.Yes)
                {
                    if (CheckAnchorNode(out anchorNode) == false)
                        return;
                }
            }

            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "XML Files|*.xml";
            dlg.FilterIndex = 0;
            dlg.Title = "이전 파일 변환하기";
            dlg.Multiselect = true;

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                XMLManager mgr = new XMLManager();
                Dictionary<int, POIType> dicPOITypes = null;

                foreach (string strFilePath in dlg.FileNames)
                {
                    string strXMLVersion;
                    Project project = mgr.LoadXML(strFilePath, out strXMLVersion, out dicPOITypes);
                    
                    if (project == null)
                    {
                        MessageBox.Show("[" + strFilePath + "], " + mgr.ErrorMessage, "Error");
                        return;
                    }
                    else
                    {
                        if (string.Compare("1.5", strXMLVersion) <= 0)
                            continue;

                        foreach (Floor floor in project.Floors)
                        {
                            if (floor.MakeShapes(project.LengthUnit) == false)
                            {
                                MessageBox.Show("실패하였습니다.");
                                return;
                            }
                        }

                        string strNewFilePath = XMLManager.MakeNewFileName(strFilePath);

                        if (mgr.Save(strNewFilePath, project, dicPOITypes) == false)
                        {
                            MessageBox.Show("[" + strFilePath + "], " + mgr.ErrorMessage, "Error");
                            break;
                        }
                    }
                }

                MessageBox.Show("파일 변환에 성공하였습니다.");
            }
        }

        private void dxfControl_MouseDown(object sender, MouseEventArgs e)
        {
            /*if (e.Button == MouseButtons.Left)
            {
                if (checkBoxSpace.Checked && checkBoxWallBoundary.Checked == false && checkBoxWallCenterLine.Checked == false)
                {
                    if (m_currentProject != null)
                    {
                        Floor floor = m_currentProject.Floors[m_nCurrentFloorIndex];
                        double x, y;

                        if (GetDXFCoord(e.X, e.Y, out x, out y))
                        {
                            Vertex2D vPos = new Vertex2D(x, y);
                            Dictionary<Space, Polygon> dicSpaceResults = new Dictionary<Space, Polygon>();

                            foreach (Space space in floor.Spaces)
                            {
                                Polygon polygon = space.GetPolygon();

                                if (polygon.HitTest(vPos) != 0)
                                    dicSpaceResults[space] = polygon;
                            }

                            Space minSpace = null;
                            double dMinArea = 0.0;

                            foreach (KeyValuePair<Space, Polygon> pair in dicSpaceResults)
                            {
                                double dArea = pair.Value.GetArea();

                                if (minSpace == null || dMinArea > dArea)
                                {
                                    minSpace = pair.Key;
                                    dMinArea = dArea;
                                }
                            }

                            if (minSpace != null)
                            {
                                int nSpaceIndex = floor.Spaces.IndexOf(minSpace);
                                MessageBox.Show(minSpace.Name + " is Clicked : " + nSpaceIndex);
                            }
                        }
                    }
                }
            }*/
        }
    }
}
