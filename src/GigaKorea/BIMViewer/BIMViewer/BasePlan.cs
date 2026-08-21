using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BIMViewer.Shapes;

namespace BIMViewer
{
    public partial class BasePlan : UserControl
    {
        private IGDIOwner m_owner = null;
        public IGDIOwner GDIOwner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        private List<DXFLayer> m_dxfLayers = null;
        private List<Layer> m_layers = null;
        private int m_iSelIndex = 0;
        public BasePlan()
        {
            InitializeComponent();            
        }

        public void SetBtnAddEnabel(bool enable)
        {
            //btnAdd.Enabled = enable;
            lblAdd.Enabled = enable;
        }
        /*
        private void btnAdd_Click(object sender, EventArgs e)
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
        }*/

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

        private void SetControls()
        {
            if (m_layers != null)
            {
                //m_systemInput = true;
                //int nVisibleCount = -1;

                //foreach (Layer layer in m_layers)
                //{
                //    if (layer.LayerType == typeof(Wall))
                //    {
                //        SetWallLayer(layer, ref nVisibleCount);
                //    }
                //    else if (layer.LayerType == typeof(Space))
                //    {
                //        SetSpaceLayer(layer, ref nVisibleCount);
                //    }
                //    else if (layer.LayerType == typeof(POI))
                //    {
                //        SetPOILayer(layer, ref nVisibleCount);
                //    }
                //    else if (layer.LayerType == typeof(Door))
                //    {
                //        SetDoorLayer(layer, ref nVisibleCount);
                //    }
                //    else if (layer.LayerType == typeof(Window))
                //    {
                //        SetWindowLayer(layer, ref nVisibleCount);
                //    }
                //}

                //if (nVisibleCount >= 0)
                //{
                //    checkBoxAll.Checked = nVisibleCount >= 5;
                //    checkBoxAll.Visible = true;
                //}

                SetDXFLayerControl();
                //m_systemInput = false;
            }

            //m_init = true;
        }

        private void SetDXFLayerControl()
        {
            gridBackgroundDXF.Rows.Clear();

            if (m_dxfLayers.Count == 0)
            {
                //gridBackgroundDXF.Visible = false;
                //this.Size = new Size(this.Size.Width, 328);
            }
            else
            {
                int nRowCount = gridBackgroundDXF.Rows.Count;
                int nLayerCount = m_dxfLayers.Count;

                for (int i = nRowCount; i < nLayerCount; i++)
                {
                    DXFViewer.Layer layer = m_dxfLayers[i];

                    int nRowIndex = gridBackgroundDXF.Rows.Add();

                    if (nRowIndex < 0)
                        continue;

                    DataGridViewRow row = gridBackgroundDXF.Rows[nRowIndex];
                    row.Cells[0].Value = !layer.Hidden;
                    row.Cells[1].Value = layer.LayerName;
                    row.Cells[3].Value = "-";
                    row.Tag = layer;

                    //open후 첫번째.
                    m_iSelIndex = 0;
                    DrawLockImages();
                }

                if (m_owner != null)
                {
                    foreach (DXFViewer.Layer layer in m_dxfLayers)
                    {
                        layer.Owner = m_owner.GetDXFPainter();
                    }
                }

                gridBackgroundDXF.Visible = true;
                //this.Size = new Size(this.Size.Width, 466);
            }
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

        private void gridBackgroundDXF_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)//레이어빼기
            {
                DataGridViewRow row = gridBackgroundDXF.Rows[e.RowIndex];
                DXFViewer.Layer layer = (DXFViewer.Layer)row.Tag;

                gridBackgroundDXF.Rows.Remove(gridBackgroundDXF.Rows[e.RowIndex]);

                m_dxfLayers.RemoveAt(e.RowIndex);
                m_owner.RefreshView();
                //삭제후 첫번째로 선택
                m_iSelIndex = 0;
                DrawLockImages();
            }
            else
            {
                if (e.ColumnIndex == 0)//show or not
                    gridBackgroundDXF.CommitEdit(DataGridViewDataErrorContexts.Commit);
                else if (e.ColumnIndex == 2)//move or not.ym
                    m_owner.SetMoveOrNot(e.RowIndex);

                m_iSelIndex = e.RowIndex;
                DrawLockImages();
            }
        }
        //Lock.unLock .ym
        private void DrawLockImages()
        {
            for (int i = 0; i < gridBackgroundDXF.Rows.Count; i++)
            {
                if (i == m_iSelIndex)
                {
                    if (m_owner.GetMoveOrNot(i))
                        ((DataGridViewImageCell)gridBackgroundDXF.Rows[i].Cells[2]).Value = Properties.Resources.Lock_Unlock_01;
                    else
                        ((DataGridViewImageCell)gridBackgroundDXF.Rows[i].Cells[2]).Value = Properties.Resources.Lock_locked_01;
                }
                else
                {
                    if (m_owner.GetMoveOrNot(i))
                        ((DataGridViewImageCell)gridBackgroundDXF.Rows[i].Cells[2]).Value = Properties.Resources.Lock_Unlocked_white_01;
                    else
                        ((DataGridViewImageCell)gridBackgroundDXF.Rows[i].Cells[2]).Value = Properties.Resources.Lock_locked_white_01;
                }
            }
        }

        public void RemoveGridList()//도면폼닫힐때.ym
        {
            gridBackgroundDXF.Rows.Clear();
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

                    m_iSelIndex = e.RowIndex;
                    DrawLockImages();
                }
            }
        }

        private void BasePlan_Resize(object sender, EventArgs e)
        {
            //btnAdd.Location = new Point(panel1.Width - btnAdd.Width - 15, btnAdd.Location.Y);
            lblAdd.Location = new Point(panel1.Width - lblAdd.Width - 15, lblAdd.Location.Y);
        }

        private void GridBackgroundDXF_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            DrawLockImages();
            Rectangle rt = gridBackgroundDXF.Rows[e.RowIndex].Cells[2].ContentBounds;
            if (rt.Contains(e.X, e.Y))
            {
                if (m_owner.GetMoveOrNot(e.RowIndex))
                    ((DataGridViewImageCell)gridBackgroundDXF.Rows[e.RowIndex].Cells[2]).Value = Properties.Resources.Lock_UnlockMSover_01;
                else
                    ((DataGridViewImageCell)gridBackgroundDXF.Rows[e.RowIndex].Cells[2]).Value = Properties.Resources.Lock_lockedMSover_01;
            }
        }

        private void GridBackgroundDXF_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            DrawLockImages();
        }

        private void LblAdd_Click(object sender, EventArgs e)
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
    }
}
