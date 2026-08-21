using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer
{
    using Shapes;
    using BIM;

    public partial class FormLayer : Form
    {
        private List<Layer> m_layers = null;
        private List<DXFLayer> m_dxfLayers = null;
        private IGDIOwner m_owner = null;
        private bool m_init = false;
        private bool m_systemInput = false;

        public IGDIOwner GDIOwner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        public FormLayer()
        {
            InitializeComponent();
        }

        public void SetLayers(List<Layer> layers, List<DXFLayer> dxfLayers)
        {
            bool changed = false;

            if (m_layers != layers)
            {
                m_layers = layers;
                changed = true;
            }

            if (m_dxfLayers != dxfLayers)
            {
                m_dxfLayers = dxfLayers;
                changed = true;
            }

            if (changed)
                SetControls();
        }

        public new void Show()
        {
            if (m_init == false)
                SetControls();

            base.Show();
        }

        public new void Show(IWin32Window owner)
        {
            if (m_init == false)
                SetControls();

            base.Show(owner);
        }

        private void SetControls()
        {
            checkBoxAll.Visible = false;
            checkBoxWall.Visible = btnColorWall.Visible = false;
            checkBoxWallCenterLine.Visible = btnColorWallCenterLine.Visible = false;
            checkBoxWallBoundary.Visible = btnColorWallBoundary.Visible = false;
            checkBoxSpace.Visible = btnColorSpace.Visible = false;
            checkBoxDoor.Visible = btnColorDoor.Visible = false;
            checkBoxDoorBoundary.Visible = btnColorDoorBoundary.Visible = false;
            checkBoxWindow.Visible = btnColorWindow.Visible = false;
            checkBoxWindowBoundary.Visible = btnColorWindowBoundary.Visible = false;
            checkBoxPOI.Visible = btnColorPOI.Visible = false;
            checkBoxColumn.Visible = btnColColumn.Visible = false;

            if (m_layers != null)
            {
                m_systemInput = true;
                int nVisibleCount = -1;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Wall))
                    {
                        SetWallLayer(layer, ref nVisibleCount);
                    }
                    else if (layer.LayerType == typeof(Space))
                    {
                        SetSpaceLayer(layer, ref nVisibleCount);
                    }
                    else if (layer.LayerType == typeof(POI))
                    {
                        SetPOILayer(layer, ref nVisibleCount);
                    }
                    else if (layer.LayerType == typeof(Door))
                    {
                        SetDoorLayer(layer, ref nVisibleCount);
                    }
                    else if (layer.LayerType == typeof(Window))
                    {
                        SetWindowLayer(layer, ref nVisibleCount);
                    }
                    else if (layer.LayerType == typeof(Column))
                    {
                        SetColumnLayer(layer, ref nVisibleCount);
                    }
                }

                if (nVisibleCount >= 0)
                {
                    checkBoxAll.Checked = nVisibleCount >= 5;
                    checkBoxAll.Visible = true;
                }

                SetDXFLayerControl();
                m_systemInput = false;
            }

            m_init = true;
        }

        private void SetDXFLayerControl()
        {
            if (m_dxfLayers.Count == 0)
            {
                gridBackgroundDXF.Visible = false;
                this.Size = new Size(this.Size.Width, 328);
            }
            else
            {
                int nRowCount = gridBackgroundDXF.Rows.Count;
                int nLayerCount = m_dxfLayers.Count;

                for (int i=nRowCount;i<nLayerCount;i++)
                {
                    DXFViewer.Layer layer = m_dxfLayers[i];

                    int nRowIndex = gridBackgroundDXF.Rows.Add();

                    if (nRowIndex < 0)
                        continue;

                    DataGridViewRow row = gridBackgroundDXF.Rows[nRowIndex];
                    row.Cells[0].Value = !layer.Hidden;
                    row.Cells[1].Value = layer.LayerName;
                    row.Tag = layer;
                }

                if (m_owner != null)
                {
                    foreach (DXFViewer.Layer layer in m_dxfLayers)
                    {
                        layer.Owner = m_owner.GetDXFPainter();
                    }
                }

                gridBackgroundDXF.Visible = true;
                this.Size = new Size(this.Size.Width, 466);
            }
        }

        private void SetDoorLayer(Layer layer, ref int nVisibleCount)
        {
            if (nVisibleCount < 0)
                nVisibleCount = 0;

            checkBoxDoor.Visible = btnColorDoor.Visible = true;
            checkBoxDoorBoundary.Visible = btnColorDoorBoundary.Visible = true;

            checkBoxDoor.Checked = layer.VisibleFill;
            btnColorDoor.BackColor = layer.FillColor;
            checkBoxDoorBoundary.Checked = layer.VisibleLine;
            btnColorDoorBoundary.BackColor = layer.LineColor;

            if (checkBoxDoor.Checked)
                nVisibleCount++;

            if (checkBoxDoorBoundary.Checked)
                nVisibleCount++;
        }

        private void SetWindowLayer(Layer layer, ref int nVisibleCount)
        {
            if (nVisibleCount < 0)
                nVisibleCount = 0;

            checkBoxWindow.Visible = btnColorWindow.Visible = true;
            checkBoxWindowBoundary.Visible = btnColorWindowBoundary.Visible = true;

            checkBoxWindow.Checked = layer.VisibleFill;
            btnColorWindow.BackColor = layer.FillColor;
            checkBoxWindowBoundary.Checked = layer.VisibleLine;
            btnColorWindowBoundary.BackColor = layer.LineColor;

            if (checkBoxWindow.Checked)
                nVisibleCount++;

            if (checkBoxWindowBoundary.Checked)
                nVisibleCount++;
        }

        private void SetColumnLayer(Layer layer, ref int nVisibleCount)
        {
            if (nVisibleCount < 0)
                nVisibleCount = 0;

            checkBoxColumn.Visible = btnColColumn.Visible = true;

            checkBoxColumn.Checked = layer.VisibleLine;
            btnColColumn.BackColor = layer.LineColor;

            if (checkBoxColumn.Checked)
                nVisibleCount++;
        }

        private void SetPOILayer(Layer layer, ref int nVisibleCount)
        {
            if (nVisibleCount < 0)
                nVisibleCount = 0;

            //checkBoxPOI.Visible = btnColorPOI.Visible = true;

            checkBoxPOI.Checked = layer.VisibleFill;
            btnColorPOI.BackColor = layer.FillColor;

            if (checkBoxPOI.Checked)
                nVisibleCount++;
        }

        private void SetSpaceLayer(Layer layer, ref int nVisibleCount)
        {
            if (nVisibleCount < 0)
                nVisibleCount = 0;

            checkBoxSpace.Visible = btnColorSpace.Visible = true;

            checkBoxSpace.Checked = layer.VisibleLine;
            btnColorSpace.BackColor = layer.LineColor;

            if (checkBoxSpace.Checked)
                nVisibleCount++;
        }

        private void SetWallLayer(Layer layer, ref int nVisibleCount)
        {
            if (nVisibleCount < 0)
                nVisibleCount = 0;

            checkBoxWall.Visible = btnColorWall.Visible = true;
            checkBoxWallCenterLine.Visible = btnColorWallCenterLine.Visible = true;
            checkBoxWallBoundary.Visible = btnColorWallBoundary.Visible = true;

            checkBoxWall.Checked = layer.VisibleFill;
            btnColorWall.BackColor = layer.FillColor;
            checkBoxWallCenterLine.Checked = layer.VisibleCenterLine;
            btnColorWallCenterLine.BackColor = layer.CenterLineColor;
            checkBoxWallBoundary.Checked = layer.VisibleLine;
            btnColorWallBoundary.BackColor = layer.LineColor;

            if (checkBoxWall.Checked)
                nVisibleCount++;

            if (checkBoxWallCenterLine.Checked)
                nVisibleCount++;

            if (checkBoxWallBoundary.Checked)
                nVisibleCount++;
        }

        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (m_systemInput)
                return;

            if (sender == checkBoxAll)
            {
                bool isChecked = checkBoxAll.Checked;

                m_systemInput = true;
                checkBoxWall.Checked = checkBoxWallCenterLine.Checked = checkBoxSpace.Checked = checkBoxPOI.Checked = isChecked;
                checkBoxWallBoundary.Checked = checkBoxDoor.Checked = checkBoxDoorBoundary.Checked = checkBoxWindow.Checked = checkBoxWindowBoundary.Checked = isChecked;
                checkBoxColumn.Checked = isChecked;
                
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Wall))
                    {
                        layer.VisibleFill = layer.VisibleLine = layer.VisibleCenterLine = isChecked;
                    }
                    else if (layer.LayerType == typeof(Space))
                    {
                        layer.VisibleLine = isChecked;
                    }
                    else if (layer.LayerType == typeof(POI))
                    {
                        layer.VisibleFill = isChecked;
                    }
                    else if (layer.LayerType == typeof(Door))
                    {
                        layer.VisibleFill = layer.VisibleLine = isChecked;
                    }
                    else if (layer.LayerType == typeof(Window))
                    {
                        layer.VisibleFill = layer.VisibleLine = isChecked;
                    }
                    else if (layer.LayerType == typeof(Column))
                    {
                        layer.VisibleLine = isChecked;
                    }
                }
                m_systemInput = false;
            }
            else if (sender == checkBoxWall)
            {
                bool isChecked = checkBoxWall.Checked;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Wall))
                    {
                        layer.VisibleFill = isChecked;
                        break;
                    }
                }
            }
            else if (sender == checkBoxWallCenterLine)
            {
                bool isChecked = checkBoxWallCenterLine.Checked;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Wall))
                    {
                        layer.VisibleCenterLine = isChecked;
                        break;
                    }
                }
            }
            else if (sender == checkBoxWallBoundary)
            {
                bool isChecked = checkBoxWallBoundary.Checked;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Wall))
                    {
                        layer.VisibleLine = isChecked;
                        break;
                    }
                }
            }
            else if (sender == checkBoxSpace)
            {
                bool isChecked = checkBoxSpace.Checked;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Space))
                    {
                        layer.VisibleLine = isChecked;
                        break;
                    }
                }
            }
            else if (sender == checkBoxPOI)
            {
                bool isChecked = checkBoxPOI.Checked;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(POI))
                    {
                        layer.VisibleFill = isChecked;
                        break;
                    }
                }
            }
            else if (sender == checkBoxDoor)
            {
                bool isChecked = checkBoxDoor.Checked;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Door))
                    {
                        layer.VisibleFill = isChecked;                        
                        break;
                    }
                }
            }
            else if (sender == checkBoxDoorBoundary)
            {
                bool isChecked = checkBoxDoorBoundary.Checked;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Door))
                    {
                        layer.VisibleLine = isChecked;
                        break;
                    }
                }
            }
            else if (sender == checkBoxWindow)
            {
                bool isChecked = checkBoxWindow.Checked;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Window))
                    {
                        layer.VisibleFill = isChecked;
                        break;
                    }
                }
            }
            else if (sender == checkBoxWindowBoundary)
            {
                bool isChecked = checkBoxWindowBoundary.Checked;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Window))
                    {
                        layer.VisibleLine = isChecked;
                        break;
                    }
                }
            }
            else if (sender == checkBoxColumn)
            {
                bool isChecked = checkBoxColumn.Checked;

                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Column))
                    {
                        layer.VisibleLine = isChecked;
                        break;
                    }
                }
            }

            if (m_owner != null)
                m_owner.RefreshView();
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            ColorDialog dlg = new ColorDialog();
            dlg.Color = btn.BackColor;

            if (dlg.ShowDialog() == DialogResult.Cancel)
                return;

            btn.BackColor = dlg.Color;

            if (sender == btnColorWall)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Wall))
                    {
                        layer.FillColor = btn.BackColor;
                        break;
                    }
                }
            }
            else if (sender == btnColorWallCenterLine)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Wall))
                    {
                        layer.CenterLineColor = btn.BackColor;
                        break;
                    }
                }
            }
            else if (sender == btnColorWallBoundary)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Wall))
                    {
                        layer.LineColor = btn.BackColor;
                        break;
                    }
                }
            }
            else if (sender == btnColorSpace)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Space))
                    {
                        layer.LineColor = btn.BackColor;
                        break;
                    }
                }
            }
            else if (sender == btnColorPOI)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(POI))
                    {
                        layer.FillColor = btn.BackColor;
                        break;
                    }
                }
            }
            else if (sender == btnColorDoor)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Door))
                    {
                        layer.FillColor = btn.BackColor;
                        break;
                    }
                }
            }
            else if (sender == btnColorDoorBoundary)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Door))
                    {
                        layer.LineColor = btn.BackColor;
                        break;
                    }
                }
            }
            else if (sender == btnColorWindow)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Window))
                    {
                        layer.FillColor = btn.BackColor;
                        break;
                    }
                }
            }
            else if (sender == btnColorWindowBoundary)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Window))
                    {
                        layer.LineColor = btn.BackColor;
                        break;
                    }
                }
            }
            else if (sender == btnColColumn)
            {
                foreach (Layer layer in m_layers)
                {
                    if (layer.LayerType == typeof(Column))
                    {
                        layer.LineColor = btn.BackColor;
                        break;
                    }
                }
            }

            if (m_owner != null)
                m_owner.RefreshView();
        }

        private void btnAddDXF_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "DXF 파일 (*.dxf)|*.dxf|모든 파일 (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                FormMain.Instance.Cursor = Cursors.WaitCursor;
                this.Enabled = false;

                DXFManager mgr = new DXFManager();
                DXFViewer.Layer layer = mgr.Load(dlg.FileName, m_owner.GetDXFPainter());

                if (layer == null)
                {
                    this.Enabled = true;
                    FormMain.Instance.Cursor = Cursors.Arrow;
                    MessageBox.Show("파일을 읽어오는데 실패하였습니다.\r\n" + dlg.FileName);
                    return;
                }

                int nIndex = dlg.FileName.LastIndexOf('\\');
                int nDotIndex = dlg.FileName.LastIndexOf('.');
                string strFileName = dlg.FileName;

                if (nIndex >= 0 && nDotIndex > nIndex)
                    strFileName = strFileName.Substring(nIndex + 1, nDotIndex - nIndex - 1);
                else if (nIndex >= 0)
                    strFileName = strFileName.Substring(nIndex + 1);

                layer.LayerName = strFileName;
                DXFLayer dxfLayer = ToDXFLayer(layer);

                m_dxfLayers.Add(dxfLayer);
                SetDXFLayerControl();

                this.Enabled = true;
                FormMain.Instance.Cursor = Cursors.Arrow;
            }
        }

        private DXFLayer ToDXFLayer(DXFViewer.Layer layer)
        {
            DXFLayer dxfLayer = new DXFLayer(layer.Owner);

            dxfLayer.Shapes.AddRange(layer.Shapes);
            dxfLayer.Hidden = layer.Hidden;
            dxfLayer.Lock = layer.Lock;
            dxfLayer.Frozen = layer.Frozen;
            dxfLayer.LineColor = layer.LineColor;
            dxfLayer.LayerName = layer.LayerName;
            dxfLayer.UseGroupItem = layer.UseGroupItem;
            dxfLayer.ShapeGroupOption = layer.ShapeGroupOption;
            dxfLayer.VisibleGroup = layer.VisibleGroup;
            dxfLayer.SetLineType(layer.GetLineType());

            return dxfLayer;
        }

        private void gridBackgroundDXF_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= gridBackgroundDXF.RowCount)
                return;

            DataGridViewRow row = gridBackgroundDXF.Rows[e.RowIndex];

            if (row.Tag != null && row.Tag is DXFViewer.Layer)
            {
                DXFViewer.Layer layer = (DXFViewer.Layer)row.Tag;

                if (e.ColumnIndex == 0)
                {
                    if (row.Cells[0].Value == null)
                        return;

                    layer.Hidden = !(bool)row.Cells[0].Value;
                    m_owner.RefreshView();
                }
                else if (e.ColumnIndex == 1)
                {
                    if (row.Cells[1].Value == null)
                        layer.LayerName = "";
                    else
                        layer.LayerName = row.Cells[1].Value.ToString();
                }
            }
        }

        private void gridBackgroundDXF_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
                gridBackgroundDXF.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }
}
