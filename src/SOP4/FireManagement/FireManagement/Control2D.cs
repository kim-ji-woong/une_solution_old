using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace FireManagement
{
    public class Control2D
    {
        public enum ControlMode { NONE = 0, DXF, Image };

        private DXFViewer.DXFControl m_dxfControl = null;
        private ImageViewCtrl m_imgControl = null;
        private string m_strFileExt = "";
        private ControlMode m_modeCurrent = ControlMode.NONE;
        private Control m_ctrlParent = null;
        // Key : 실제 객체의 Layer(타입을 알수 없으므로 object로 둔다.)
        private Dictionary<object, ControlLayer> m_dicLayers = new Dictionary<object, ControlLayer>();

        public ControlMode Mode
        {
            get { return m_modeCurrent; }
        }

        public string IndoorFileType
        {
            get { return m_strFileExt; }
            set { m_strFileExt = value; }
        }

        public UnE.Geometry.Vertex2D MovedVertex
        {
            get
            {
                switch (m_modeCurrent)
                {
                    case ControlMode.DXF:
                        return m_dxfControl.MovedVertex;

                    case ControlMode.Image:
                        return m_imgControl.MovedVertex;
                }

                return null;
            }
        }

        public Control Control
        {
            get
            {
                switch (m_modeCurrent)
                {
                    case ControlMode.DXF:
                        return m_dxfControl;

                    case ControlMode.Image:
                        return m_imgControl;
                }

                return null;
            }
        }

        public bool IsOpened
        {
            get
            {
                switch (m_modeCurrent)
                {
                    case ControlMode.DXF:
                        return m_dxfControl.IsOpened;

                    case ControlMode.Image:
                        return m_imgControl.Image != null;
                }

                return false;
            }
        }

        public Dictionary<object, ControlLayer> Layers
        {
            get { return m_dicLayers; }
        }

        public System.Drawing.Color BackColor
        {
            get
            {
                switch (m_modeCurrent)
                {
                    case ControlMode.DXF:
                        return m_dxfControl.BackColor;

                    case ControlMode.Image:
                        return m_imgControl.BackColor;
                }

                return System.Drawing.Color.Black;
            }
            set
            {
                switch (m_modeCurrent)
                {
                    case ControlMode.DXF:
                        m_dxfControl.BackColor = value;
                        break;

                    case ControlMode.Image:
                        m_imgControl.BackColor = value;
                        break;
                }
            }
        }

        public System.Drawing.Point Location
        {
            get
            {
                switch (m_modeCurrent)
                {
                    case ControlMode.DXF:
                        return m_dxfControl.Location;

                    case ControlMode.Image:
                        return m_imgControl.Location;
                }

                return new System.Drawing.Point();
            }
            set
            {
                switch (m_modeCurrent)
                {
                    case ControlMode.DXF:
                        m_dxfControl.Location = value;
                        break;

                    case ControlMode.Image:
                        m_imgControl.Location = value;
                        break;
                }
            }
        }

        public System.Drawing.Size Size
        {
            get
            {
                switch (m_modeCurrent)
                {
                    case ControlMode.DXF:
                        return m_dxfControl.Size;

                    case ControlMode.Image:
                        return m_imgControl.Size;
                }

                return new System.Drawing.Size();
            }
            set
            {
                switch (m_modeCurrent)
                {
                    case ControlMode.DXF:
                        m_dxfControl.Size = value;
                        break;

                    case ControlMode.Image:
                        m_imgControl.Size = value;
                        break;
                }
            }
        }

        public UnitOfLength UnitOfLength
        {
            get
            {
                switch (m_modeCurrent)
                {
                    case ControlMode.DXF:
                        return FireManagement.UnitOfLength.MILLIMETER;

                    case ControlMode.Image:
                        return FireManagement.UnitOfLength.METER;
                }

                return FireManagement.UnitOfLength.METER;
            }
        }

        public Control2D(ControlMode mode, Control ctrlParent)
        {
            m_ctrlParent = ctrlParent;

            switch (mode)
            {
                case ControlMode.DXF:
                    m_dxfControl = new DXFViewer.DXFControl();
                    m_strFileExt = "dxf";
                    m_modeCurrent = ControlMode.DXF;
                    InitializeDXF();
                    break;

                case ControlMode.Image:
                    m_imgControl = new ImageViewCtrl();
                    m_strFileExt = "png";
                    m_modeCurrent = ControlMode.Image;
                    InitialzeImage();
                    break;
            }
        }

        private void InitializeDXF()
        {
            if (m_ctrlParent == null)
                return;

            this.m_dxfControl.BackColor = System.Drawing.Color.Black;
            this.m_dxfControl.GroupItemDistance = 30;
            this.m_dxfControl.GroupItemMinCount = 3;
            this.m_dxfControl.Location = new System.Drawing.Point(131, 98);
            this.m_dxfControl.Name = "dxfControl1";
            this.m_dxfControl.Panning = false;
            this.m_dxfControl.PanningMouseButton = System.Windows.Forms.MouseButtons.Middle;
            this.m_dxfControl.Size = new System.Drawing.Size(150, 150);
            this.m_dxfControl.TabIndex = 1;
            this.m_dxfControl.UnitOfLength = DXFViewer.UnitOfLength.MILLIMETER;
            this.m_dxfControl.UseGroupItem = false;
            this.m_dxfControl.UseMouseWheel = true;

            m_ctrlParent.Controls.Add(m_dxfControl);
        }

        private void InitialzeImage()
        {
            if (m_ctrlParent == null)
                return;

            this.m_imgControl.BackColor = System.Drawing.Color.White;
            this.m_imgControl.BillboardHeight = 32;
            this.m_imgControl.BillboardWidth = 32;
            this.m_imgControl.CurrentMouseWorkMode = FireManagement.MouseWorkMode.PICK;
            this.m_imgControl.DrawBillBoard = true;
            this.m_imgControl.EditMode = false;
            this.m_imgControl.IsIndoor = true;
            this.m_imgControl.Location = new System.Drawing.Point(131, 98);
            this.m_imgControl.Name = "dxfControl1";
            this.m_imgControl.Popup = null;
            this.m_imgControl.RectZoomMode = false;
            this.m_imgControl.RotationMode = false;
            this.m_imgControl.Size = new System.Drawing.Size(150, 150);
            this.m_imgControl.TabIndex = 1;
            this.m_imgControl.TranslateMode = false;

            m_ctrlParent.Controls.Add(m_imgControl);
        }

        public static ControlMode GetControlMode(string strIndoorFileType)
        {
            if (string.Compare(strIndoorFileType, "dxf", true) == 0)
                return ControlMode.DXF;
            else if (string.Compare(strIndoorFileType, "png", true) == 0 ||
                string.Compare(strIndoorFileType, "jpg", true) == 0 ||
                string.Compare(strIndoorFileType, "bmp", true) == 0 ||
                string.Compare(strIndoorFileType, "gif", true) == 0)
                return ControlMode.Image;

            return ControlMode.NONE;
        }

        public void Refresh()
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    m_dxfControl._Refresh();
                    break;

                case ControlMode.Image:
                    m_imgControl.Refresh();
                    break;
            }
        }

        public void LoadHome(bool refresh)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    m_dxfControl.LoadHomeMatrix(refresh);
                    break;

                case ControlMode.Image:
                    m_imgControl.FitView();
                    break;
            }
        }

        public UnE.Geometry.Vertex2D ScreenToGlobal(int x, int y)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    return m_dxfControl.ScreenToGlobal(x, y);

                case ControlMode.Image:
                    return m_imgControl.ScreenToGlobal(x, y);
            }

            return null;
        }

        public object PickObject(double x, double y)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    {
                        DXFViewer.Shape shape = m_dxfControl.PickObject(x, y);
                        return shape;
                        /*if (shape == null)
                            return null;
                        else
                            return new ControlShape(shape);*/
                    }

                case ControlMode.Image:
                    {
                        FireManagement.Shape shape = m_imgControl.PickObject(x, y);
                        return shape;
                        /*if (shape == null)
                            return null;
                        else
                            return new ControlShape(shape);*/
                    }
            }

            return null;
        }

        public void SelectShape(object shape, bool isSelected)
        {
            if (shape == null)
                return;

            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    ((DXFViewer.Shape)shape).Selected = isSelected;
                    break;

                case ControlMode.Image:
                    ((FireManagement.Shape)shape).Selected = isSelected;
                    break;
            }
        }

        public void MoveShape(object shape, UnE.Geometry.Vertex2D vPos)
        {
            if (shape == null)
                return;

            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    {
                        UnE.Geometry.Vertex2D vMove = this.MovedVertex;
                        ((DXFViewer.Shape)shape).Move(vPos.x + vMove.x, vPos.y + vMove.y);
                    }
                    break;

                case ControlMode.Image:
                    ((FireManagement.Shape)shape).Move(vPos);
                    break;
            }
        }

        public UnE.Geometry.Vertex2D GetShapePosition(object shape)
        {
            if (shape == null)
                return null;

            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    return ((DXFViewer.Shape)shape).Position;

                case ControlMode.Image:
                    return ((FireManagement.Shape)shape).Position;
            }

            return null;
        }

        public ControlLayer GetLayer(object shape)
        {
            if (shape == null)
                return null;

            object layer = null;

            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    layer = ((DXFViewer.Shape)shape).GetLayer();
                    break;

                case ControlMode.Image:
                    layer = ((FireManagement.Shape)shape).GetLayer();
                    break;
            }

            if (layer == null)
                return null;

            ControlLayer cLayer = null;

            if (m_dicLayers.TryGetValue(layer, out cLayer))
                return cLayer;

            return null;
        }

        private ArrayList GetOriginLayers()
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    return m_dxfControl.Layers;

                case ControlMode.Image:
                    return m_imgControl.Layers;
            }

            return null;
        }

        public bool Open(string strFilePath, Zone zone = null)
        {
            FormMain2.Instance.ClearEquipmentLayer();
            m_dicLayers.Clear();

            bool isOpened = false;

            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    isOpened = m_dxfControl.OpenDXF(strFilePath);
                    break;

                case ControlMode.Image:
                    m_imgControl.Layers.Clear();
                    if (!m_imgControl.SetImage(strFilePath, zone))
                        return false;

                    m_imgControl.FitView();
                    isOpened = true;
                    break;
            }

            if (isOpened)
            {
                ArrayList layers = GetOriginLayers();

                foreach (object layer in layers)
                {
                    ControlLayer cLayer = new ControlLayer(this, layer);
                    m_dicLayers[layer] = cLayer;
                }
            }

            return isOpened;
        }

        public object CloneShape(object shape)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    return ((DXFViewer.Shape)shape).Clone();

                case ControlMode.Image:
                    return ((FireManagement.Shape)shape).Clone();
            }

            return null;
        }

        public object GetShapeTag(object shape)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    return ((DXFViewer.Shape)shape).Tag;

                case ControlMode.Image:
                    return ((FireManagement.Shape)shape).Tag;
            }

            return null;
        }

        public void SetShapeData(object shape, int nID, string strEquipID)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    ((DXFViewer.Shape)shape).ID = nID;
                    break;

                case ControlMode.Image:
                    ((FireManagement.Shape)shape).ID = nID;
                    ((FireManagement.Shape)shape).EquipID = strEquipID;
                    break;
            }
        }

        public ControlLayer GetShapeLayer(object shape)
        {
            ControlLayer cLayer = null;

            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    {
                        DXFViewer.Layer layer = ((DXFViewer.Shape)shape).GetLayer();

                        if (layer != null)
                        {
                            m_dicLayers.TryGetValue(layer, out cLayer);
                        }
                    }
                    break;

                case ControlMode.Image:
                    {
                        FireManagement.Layer layer = ((FireManagement.Shape)shape).GetLayer();

                        if (layer != null)
                        {
                            m_dicLayers.TryGetValue(layer, out cLayer);
                        }
                    }
                    break;
            }

            return cLayer;
        }

        public ControlText IsTextShape(object shape)
        {
            if (shape == null)
                return null;

            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    {
                        if (shape is DXFViewer.Text)
                            return new ControlText(this, shape);
                    }
                    break;

                case ControlMode.Image:
                    {
                        if (shape is FireManagement.Text)
                            return new ControlText(this, shape);
                    }
                    break;
            }

            return null;
        }

        public System.Drawing.Point PointToClient(System.Drawing.Point pt)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    return m_dxfControl.PointToClient(pt);

                case ControlMode.Image:
                    return m_imgControl.PointToClient(pt);
            }

            return new System.Drawing.Point();
        }

        public object MakeHatch(FireEquipment.EquipmentType type)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    if (type == FireEquipment.EquipmentType.FE)
                        return MakeFEHatch();
                    else if (type == FireEquipment.EquipmentType.HD)
                        return MakeHDHatch();
                    else if (type == FireEquipment.EquipmentType.FA)
                        return MakeFAHatch();
                    else if (type == FireEquipment.EquipmentType.FR)
                        return MakeFRHatch();
                    break;

                case ControlMode.Image:
                    return new FireManagement.Shape();
            }

            return null;
        }

        // 소화기
        private DXFViewer.Hatch MakeFEHatch()
        {
            DXFViewer.Hatch hatchFE = new DXFViewer.Hatch();

            int nPointSize = 30;
            hatchFE.SetPointSize(nPointSize);

            double dAngleFE = System.Math.PI * 2 / nPointSize;
            double dRadiusFE = 1250 / 2;

            for (int i = 0; i < nPointSize; i++)
            {
                double x = -dRadiusFE * System.Math.Sin(dAngleFE * i);
                double y = dRadiusFE * System.Math.Cos(dAngleFE * i);
                hatchFE.UpdatePoint(i, (float)x, (float)y);
            }

            return hatchFE;
        }

        // 소화전
        private DXFViewer.Hatch MakeHDHatch()
        {
            DXFViewer.Hatch hatchHD = new DXFViewer.Hatch();

            hatchHD.SetPointSize(4);
            hatchHD.UpdatePoint(0, -1800, -900);
            hatchHD.UpdatePoint(1, 1800, -900);
            hatchHD.UpdatePoint(2, 1800, 900);
            hatchHD.UpdatePoint(3, -1800, 900);

            return hatchHD;
        }

        // 발신기
        private DXFViewer.Hatch MakeFAHatch()
        {
            DXFViewer.Hatch hatchFA = new DXFViewer.Hatch();

            double dTriangleSize = 2500;
            double root3 = System.Math.Pow(3, 0.5);

            hatchFA.SetPointSize(3);
            hatchFA.UpdatePoint(0, 0, (float)(dTriangleSize / root3));
            hatchFA.UpdatePoint(1, (float)(-dTriangleSize / 2), (float)(-dTriangleSize / 2 / root3));
            hatchFA.UpdatePoint(2, (float)(dTriangleSize / 2), (float)(-dTriangleSize / 2 / root3));

            return hatchFA;
        }

        // 수신반
        private DXFViewer.Hatch MakeFRHatch()
        {
            DXFViewer.Hatch hatchFR = new DXFViewer.Hatch();

            double dRadiusFA = 2000;
            // 별을 그리기 위한 원의 5등분 각도
            double dAngleFA = UnE.Geometry.Math.DegToRad(72.0);

            UnE.Geometry.Vertex2D v1 = new UnE.Geometry.Vertex2D(0.0, dRadiusFA);
            UnE.Geometry.Vertex2D v2 = new UnE.Geometry.Vertex2D(-dRadiusFA * System.Math.Sin(dAngleFA), dRadiusFA * System.Math.Cos(dAngleFA));
            UnE.Geometry.Vertex2D v3 = new UnE.Geometry.Vertex2D(-dRadiusFA * System.Math.Sin(dAngleFA * 2), dRadiusFA * System.Math.Cos(dAngleFA * 2));
            UnE.Geometry.Vertex2D v4 = new UnE.Geometry.Vertex2D(-dRadiusFA * System.Math.Sin(dAngleFA * 3), dRadiusFA * System.Math.Cos(dAngleFA * 3));
            UnE.Geometry.Vertex2D v5 = new UnE.Geometry.Vertex2D(-dRadiusFA * System.Math.Sin(dAngleFA * 4), dRadiusFA * System.Math.Cos(dAngleFA * 4));

            UnE.Geometry.Line2D line1 = new UnE.Geometry.Line2D(v1, v3, UnE.Geometry.Line2D.LineType.SEGMENT);
            UnE.Geometry.Line2D line2 = new UnE.Geometry.Line2D(v2, v4, UnE.Geometry.Line2D.LineType.SEGMENT);
            UnE.Geometry.Line2D line3 = new UnE.Geometry.Line2D(v3, v5, UnE.Geometry.Line2D.LineType.SEGMENT);
            UnE.Geometry.Line2D line4 = new UnE.Geometry.Line2D(v4, v1, UnE.Geometry.Line2D.LineType.SEGMENT);
            UnE.Geometry.Line2D line5 = new UnE.Geometry.Line2D(v5, v2, UnE.Geometry.Line2D.LineType.SEGMENT);

            UnE.Geometry.Vertex2D vInner1, vInner2, vInner3, vInner4, vInner5, vTemp;
            UnE.Geometry.Line2D.LineType resultType;

            line1.IntersectLine(line5, out vInner1, out vTemp, out resultType);
            line2.IntersectLine(line1, out vInner2, out vTemp, out resultType);
            line3.IntersectLine(line2, out vInner3, out vTemp, out resultType);
            line4.IntersectLine(line3, out vInner4, out vTemp, out resultType);
            line5.IntersectLine(line4, out vInner5, out vTemp, out resultType);

            hatchFR.SetPointSize(10);

            hatchFR.UpdatePoint(0, (float)v1.x, (float)v1.y);
            hatchFR.UpdatePoint(1, (float)vInner1.x, (float)vInner1.y);
            hatchFR.UpdatePoint(2, (float)v2.x, (float)v2.y);
            hatchFR.UpdatePoint(3, (float)vInner2.x, (float)vInner2.y);
            hatchFR.UpdatePoint(4, (float)v3.x, (float)v3.y);
            hatchFR.UpdatePoint(5, (float)vInner3.x, (float)vInner3.y);
            hatchFR.UpdatePoint(6, (float)v4.x, (float)v4.y);
            hatchFR.UpdatePoint(7, (float)vInner4.x, (float)vInner4.y);
            hatchFR.UpdatePoint(8, (float)v5.x, (float)v5.y);
            hatchFR.UpdatePoint(9, (float)vInner5.x, (float)vInner5.y);

            return hatchFR;
        }

        public void SetShapeColor(object shape, System.Drawing.Color color)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    ((DXFViewer.Shape)shape).SetOwnColor(color);
                    ((DXFViewer.Shape)shape).SetColorOption(DXFViewer.Shape.ControlType.BYOWN);
                    break;

                case ControlMode.Image:
                    ((FireManagement.Shape)shape).Color = color;
                    break;
            }
        }

        public void AddLayer(ControlLayer layer)
        {
            object originLayer = layer.Layer;
            ArrayList originLayers = GetOriginLayers();

            if (!originLayers.Contains(originLayer))
                originLayers.Add(originLayer);

            this.Layers[originLayer] = layer;
        }

        public object FindOriginLayer(string strLayerName)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    {
                        foreach (DXFViewer.Layer layer in m_dxfControl.Layers)
                        {
                            if (layer.LayerName == strLayerName)
                                return layer;
                        }
                    }
                    break;

                case ControlMode.Image:
                    {
                        foreach (FireManagement.Layer layer in m_imgControl.Layers)
                        {
                            if (layer.LayerName == strLayerName)
                                return layer;
                        }
                    }
                    break;
            }

            return null;
        }

        public void SelectableShape(object shape, bool isSelectable)
        {
            switch (m_modeCurrent)
            {
                case ControlMode.DXF:
                    ((DXFViewer.Shape)shape).Selectable = isSelectable;
                    break;
            }
        }
    }

    public class ControlLayer
    {
        private Control2D m_owner = null;
        private DXFViewer.Layer m_dxfLayer = null;
        private FireManagement.Layer m_imgLayer = null;

        public DXFViewer.Layer DXFLayer
        {
            get { return m_dxfLayer; }
            set { m_dxfLayer = value; }
        }

        public FireManagement.Layer ImageLayer
        {
            get { return m_imgLayer; }
            set { m_imgLayer = value; }
        }

        public object Layer
        {
            get
            {
                switch (m_owner.Mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfLayer;

                    case Control2D.ControlMode.Image:
                        return m_imgLayer;
                }

                return null;
            }
        }

        public string LayerName
        {
            get
            {
                switch (m_owner.Mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfLayer.LayerName;

                    case Control2D.ControlMode.Image:
                        return m_imgLayer.LayerName;
                }

                return "";
            }
            set
            {
                switch (m_owner.Mode)
                {
                    case Control2D.ControlMode.DXF:
                        m_dxfLayer.LayerName = value;
                        break;

                    case Control2D.ControlMode.Image:
                        m_imgLayer.LayerName = value;
                        break;
                }
            }
        }

        public bool Frozen
        {
            get
            {
                switch (m_owner.Mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfLayer.Frozen;

                    case Control2D.ControlMode.Image:
                        return m_imgLayer.Frozen;
                }

                return false;
            }
            set
            {
                switch (m_owner.Mode)
                {
                    case Control2D.ControlMode.DXF:
                        m_dxfLayer.Frozen = value;
                        break;

                    case Control2D.ControlMode.Image:
                        m_imgLayer.Frozen = value;
                        break;
                }
            }
        }

        public bool Hidden
        {
            get
            {
                switch (m_owner.Mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfLayer.Hidden;

                    case Control2D.ControlMode.Image:
                        return m_imgLayer.Hidden;
                }

                return false;
            }
            set
            {
                switch (m_owner.Mode)
                {
                    case Control2D.ControlMode.DXF:
                        m_dxfLayer.Hidden = value;
                        break;

                    case Control2D.ControlMode.Image:
                        m_imgLayer.Hidden = value;
                        break;
                }
            }
        }

        public Control2D Owner
        {
            get { return m_owner; }
        }

        public System.Drawing.Color LineColor
        {
            get
            {
                switch (m_owner.Mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfLayer.LineColor;

                    case Control2D.ControlMode.Image:
                        return m_imgLayer.LineColor;
                }

                return System.Drawing.Color.Black;
            }
            set
            {
                switch (m_owner.Mode)
                {
                    case Control2D.ControlMode.DXF:
                        m_dxfLayer.LineColor = value;
                        break;

                    case Control2D.ControlMode.Image:
                        m_imgLayer.LineColor = value;
                        break;
                }
            }
        }

        public ControlLayer(Control2D owner, object layer = null)
        {
            m_owner = owner;

            switch (m_owner.Mode)
            {
                case Control2D.ControlMode.DXF:
                    if (layer == null)
                        m_dxfLayer = new DXFViewer.Layer((DXFViewer.DXFControl)owner.Control);
                    else
                        m_dxfLayer = (DXFViewer.Layer)layer;
                    break;

                case Control2D.ControlMode.Image:
                    if (layer == null)
                        m_imgLayer = new FireManagement.Layer((ImageViewCtrl)owner.Control);
                    else
                        m_imgLayer = (FireManagement.Layer)layer;
                    break;
            }
        }

        public ArrayList Shapes
        {
            get
            {
                switch (m_owner.Mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfLayer.Shapes;

                    case Control2D.ControlMode.Image:
                        return m_imgLayer.Shapes;
                }

                return null;
            }
        }

        public void Add(object shape)
        {
            if (shape == null)
                return;

            switch (m_owner.Mode)
            {
                case Control2D.ControlMode.DXF:
                    if (shape is ControlText)
                        m_dxfLayer.Add((DXFViewer.Text)((ControlText)shape).Text);
                    else
                        m_dxfLayer.Add((DXFViewer.Shape)shape);
                    break;

                case Control2D.ControlMode.Image:
                    if (shape is ControlText)
                        m_imgLayer.Add((FireManagement.Text)((ControlText)shape).Text);
                    else
                        m_imgLayer.Add((FireManagement.Shape)shape);
                    break;
            }
        }

        public void Remove(object shape)
        {
            if (shape == null)
                return;

            switch (m_owner.Mode)
            {
                case Control2D.ControlMode.DXF:
                    if (shape is ControlText)
                        m_dxfLayer.Remove((DXFViewer.Text)((ControlText)shape).Text);
                    else
                        m_dxfLayer.Remove((DXFViewer.Shape)shape);
                    break;

                case Control2D.ControlMode.Image:
                    if (shape is ControlText)
                        m_imgLayer.Remove((FireManagement.Text)((ControlText)shape).Text);
                    else
                        m_imgLayer.Remove((FireManagement.Shape)shape);
                    break;
            }
        }

        public void RemoveAll()
        {
            switch (m_owner.Mode)
            {
                case Control2D.ControlMode.DXF:
                    m_dxfLayer.RemoveAll();
                    break;

                case Control2D.ControlMode.Image:
                    m_imgLayer.RemoveAll();
                    break;
            }
        }
    }

    public class ControlText
    {
        private DXFViewer.Text m_dxfText = null;
        private FireManagement.Text m_imgText = null;
        private Control2D.ControlMode m_mode = Control2D.ControlMode.NONE;

        public object Text
        {
            get
            {
                switch (m_mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfText;

                    case Control2D.ControlMode.Image:
                        return m_imgText;
                }

                return "";
            }
        }

        public string Title
        {
            get
            {
                switch (m_mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfText.Title;

                    case Control2D.ControlMode.Image:
                        return m_imgText.Title;
                }

                return "";
            }
            set
            {
                switch (m_mode)
                {
                    case Control2D.ControlMode.DXF:
                        m_dxfText.Title = value;
                        break;

                    case Control2D.ControlMode.Image:
                        m_imgText.Title = value;
                        break;
                }
            }
        }

        public System.Drawing.Font Font
        {
            get
            {
                switch (m_mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfText.Font;

                    case Control2D.ControlMode.Image:
                        return m_imgText.Font;
                }

                return null;
            }
            set
            {
                switch (m_mode)
                {
                    case Control2D.ControlMode.DXF:
                        m_dxfText.Font = value;
                        break;

                    case Control2D.ControlMode.Image:
                        m_imgText.Font = value;
                        break;
                }
            }
        }

        public object Tag
        {
            get
            {
                switch (m_mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfText.Tag;

                    case Control2D.ControlMode.Image:
                        return m_imgText.Tag;
                }

                return null;
            }
            set
            {
                switch (m_mode)
                {
                    case Control2D.ControlMode.DXF:
                        m_dxfText.Tag = value;
                        break;

                    case Control2D.ControlMode.Image:
                        m_imgText.Tag = value;
                        break;
                }
            }
        }

        public UnE.Geometry.Vertex2D Position
        {
            get
            {
                switch (m_mode)
                {
                    case Control2D.ControlMode.DXF:
                        return m_dxfText.Position;

                    case Control2D.ControlMode.Image:
                        return m_imgText.Position;
                }

                return null;
            }
        }

        public ControlText(Control2D ctrl)
        {
            m_mode = ctrl.Mode;

            switch (m_mode)
            {
                case Control2D.ControlMode.DXF:
                    m_dxfText = new DXFViewer.Text();
                    break;

                case Control2D.ControlMode.Image:
                    m_imgText = new FireManagement.Text();
                    break;
            }
        }

        public ControlText(Control2D ctrl, object shape)
        {
            m_mode = ctrl.Mode;

            switch (m_mode)
            {
                case Control2D.ControlMode.DXF:
                    m_dxfText = (DXFViewer.Text)shape;
                    break;

                case Control2D.ControlMode.Image:
                    m_imgText = (FireManagement.Text)shape;
                    break;
            }
        }

        public void SetPosition(UnE.Geometry.Vertex2D vPos)
        {
            switch (m_mode)
            {
                case Control2D.ControlMode.DXF:
                    m_dxfText.SetPosition(vPos);
                    break;

                case Control2D.ControlMode.Image:
                    m_imgText.SetPosition(vPos);
                    break;
            }
        }
    }
}
