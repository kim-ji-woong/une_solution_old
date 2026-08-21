using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SoilMan.DockingForm
{
    public partial class FormDetailLayer : Form
    {
        private bool m_sortDescending = true;
        private List<DXFViewer.Layer> m_layers = new List<DXFViewer.Layer>();
        private DockingForm.FormLayer.LayerType m_layerType = DockingForm.FormLayer.LayerType.UNKNOWN;

        private UnE.Geometry.Vertex2D m_vTL = null;
        public UnE.Geometry.Vertex2D LayerBoundTL
        {
            get { return m_vTL; }
            set { m_vTL = value; }
        }
        private UnE.Geometry.Vertex2D m_vBR = null;
        public UnE.Geometry.Vertex2D LayerBoundBR
        {
            get { return m_vBR; }
            set { m_vBR = value; }
        }


        public bool Visible
        {
            get { return checkBoxShow.Checked; }
            set
            {
                if (checkBoxShow.Checked != value)
                {
                    checkBoxShow.Checked = value;
                }

                if (value == true && m_layers.Count > 0 && dataGridView1.Rows.Count == 0)
                {
                    SetLayers();
                }
            }
        }

        public List<DXFViewer.Layer> Layers
        {
            get { return m_layers; }
        }

        public FormDetailLayer(string strLayerName, DockingForm.FormLayer.LayerType layerType)
        {
            InitializeComponent();

            labelLayerName.Text = strLayerName;
            m_layerType = layerType;
        }

        private void labelLayerName_Click(object sender, EventArgs e)
        {
            checkBoxShow.Checked = !checkBoxShow.Checked;
        }

        private void checkBoxShow_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Drawing.ShapeLayer layer in m_layers)
            {
                layer.Usable = checkBoxShow.Checked;
            }

            FormMain.Instance.ShowLayer(m_layerType, checkBoxShow.Checked);
            //FormMain.Instance.SetViewport();
            //FormMain.Instance.RefreshView();
        }

        private void SetLayers()
        {
            
            dataGridView1.Rows.Clear();

            foreach (DXFViewer.Layer layer in m_layers)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewCheckBoxCell checkedCell = new DataGridViewCheckBoxCell();
                checkedCell.Value = !layer.Hidden;
                row.Cells.Add(checkedCell);

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = layer.LayerName;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Style.BackColor = layer.LineColor;
                row.Cells.Add(cell);

                row.Tag = layer;
                dataGridView1.Rows.Add(row);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                if (e.ColumnIndex == 0)
                {
                    if (m_sortDescending)
                        dataGridView1.Sort(colVisible, ListSortDirection.Descending);
                    else
                        dataGridView1.Sort(colVisible, ListSortDirection.Ascending);

                    m_sortDescending = !m_sortDescending;
                }
                else if (e.ColumnIndex == 2)
                {
                    dataGridView1.Sort(new GridViewColorColumnSort());
                }
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[0];
                DXFViewer.Layer layer = (DXFViewer.Layer)dataGridView1.Rows[e.RowIndex].Tag;

                layer.Hidden = !((bool)cell.Value);

                FormMain.Instance.RefreshView();
            }
        }

        private void dataGridView1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex == 2)
                {
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                    DXFViewer.Layer layer = (DXFViewer.Layer)row.Tag;

                    if (layer != null)
                    {
                        ColorDialog dlg = new ColorDialog();
                        dlg.Color = layer.LineColor;

                        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            if (dlg.Color == layer.LineColor)
                                return;
                            else
                            {
                                DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)row.Cells[e.ColumnIndex];
                                cell.Style.BackColor = dlg.Color;
                                layer.LineColor = dlg.Color;
                                FormMain.Instance.RefreshView();
                            }
                        }
                        /*Popup.FormColor frm = new Popup.FormColor();
                        DialogFormFrame frameColor = new DialogFormFrame(frm);
                        frm.Color = data.Color;
                        frm.Alpha = data.Alpha;
                        frm.Text = "도면층 색상 - " + data.LayerName;

                        if (frameColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            row.Cells[2].Style.BackColor = frm.Color;

                            if (data.Color == frm.Color && data.Alpha == frm.Alpha)
                                return;
                            else
                            {
                                data.Color = frm.Color;
                                data.Alpha = frm.Alpha;
                                data.LinkedLayer.LineColor = Color.FromArgb(data.Alpha, data.Color);
                                FormMain.Instance.RefreshView();
                            }
                        }*/
                    }
                }
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        public void Reset()
        {
            SetLayers();
        }

        public void Clear()
        {
            dataGridView1.Rows.Clear();
            m_layers.Clear();
        }
    }

    public class GridViewColorColumnSort : System.Collections.IComparer
    {
        private static bool m_isAscending = true;

        public GridViewColorColumnSort()
        {
            m_isAscending = !m_isAscending;
        }

        public int Compare(object obj1, object obj2)
        {
            DataGridViewRow row1 = (DataGridViewRow)obj1;
            DataGridViewRow row2 = (DataGridViewRow)obj2;

            DataGridViewCell cell1 = row1.Cells[2];
            DataGridViewCell cell2 = row2.Cells[2];

            Color col1 = cell1.Style.BackColor;
            Color col2 = cell2.Style.BackColor;

            int nColor1 = ((int)col1.R) * 256 * 256 + ((int)col1.G) * 256 + (int)col1.B;
            int nColor2 = ((int)col2.R) * 256 * 256 + ((int)col2.G) * 256 + (int)col2.B;

            return m_isAscending ? nColor1 - nColor2 : nColor2 - nColor1;
        }
    }
}
