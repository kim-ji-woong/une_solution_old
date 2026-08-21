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

namespace FireSensorReader
{
    public partial class FormLayer : Form
    {
        private List<Layer> m_layers = null;
        private bool m_systemInput = false;
        private DXFControl m_dxfControl = null;

        public FormLayer(DXFControl dxfControl)
        {
            InitializeComponent();
            m_dxfControl = dxfControl;
        }

        public void SetLayers(List<Layer> layers)
        {
            m_layers = layers;
            SetGrid();
        }

        private void SetGrid()
        {
            gridLayer.Rows.Clear();

            if (m_layers == null)
                return;

            m_systemInput = true;

            foreach (Layer layer in m_layers)
            {
                int nRowIndex = gridLayer.Rows.Add();

                if (nRowIndex < 0)
                    continue;

                DataGridViewRow row = gridLayer.Rows[nRowIndex];

                row.Cells[0].Value = layer.LayerName;
                row.Cells[1].Value = !layer.Hidden;
                row.Cells[2].Style.BackColor = layer.LineColor;
                row.Tag = layer;
            }

            m_systemInput = false;
        }

        private void gridLayer_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_systemInput)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 1)
            {
                bool isChecked = (bool)gridLayer.Rows[e.RowIndex].Cells[1].Value;
                //System.Diagnostics.Trace.WriteLine("CellValueChanged, " + gridLayer.Rows[e.RowIndex].Cells[0].Value.ToString() + " is " + isChecked);

                DataGridViewRow row = gridLayer.Rows[e.RowIndex];
                Layer layer = (Layer)row.Tag;

                layer.Hidden = !isChecked;
                m_dxfControl._Refresh();
            }
        }

        private void gridLayer_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            gridLayer.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void checkBoxAllLayer_CheckedChanged(object sender, EventArgs e)
        {
            m_systemInput = true;
            bool hidden = !checkBoxAllLayer.Checked;

            foreach (DataGridViewRow row in gridLayer.Rows)
            {
                Layer layer = (Layer)row.Tag;
                layer.Hidden = hidden;
                row.Cells[1].Value = !hidden;
            }

            m_systemInput = false;
            m_dxfControl._Refresh();
        }
    }
}
