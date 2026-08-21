using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace DXFView
{
    public partial class LeftPanel : Panel
    {
        private ArrayList m_arrLayers = new ArrayList();
        private ArrayList m_arrBlocks = new ArrayList();
        private DXFViewer.DXFControl m_dxfControl = null;

        public DXFViewer.DXFControl DXFControl
        {
            set { m_dxfControl = value; }
        }

        public LeftPanel()
        {
            InitializeComponent();
            InitGridSize();
        }

        public LeftPanel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            InitGridSize();
        }

        private void InitGridSize()
        {
            this.Controls.Add(dataGridViewLayer);
            this.Controls.Add(dataGridViewBlock);

            dataGridViewLayer.Location = new Point(0, 0);
            dataGridViewLayer.Size = new Size(this.Size.Width, this.Size.Height / 2);

            dataGridViewBlock.Location = new Point(0, this.Size.Height / 2);
            dataGridViewBlock.Size = new Size(this.Size.Width, this.Size.Height / 2);
        }

        public void Init()
        {
            InitLayers();
            InitBlocks();
        }

        private void InitLayers()
        {
            dataGridViewLayer.Rows.Clear();

            foreach (DXFViewer.Layer layer in m_arrLayers)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell();
                cell.Value = !layer.Hidden;
                row.Cells.Add(cell);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = layer.LayerName;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Style.BackColor = layer.LineColor;
                cell3.Style.SelectionBackColor = layer.LineColor;
                row.Cells.Add(cell3);

                dataGridViewLayer.Rows.Add(row);
            }
        }

        private void InitBlocks()
        {
            dataGridViewBlock.Rows.Clear();

            foreach (DXFViewer.Block block in m_arrBlocks)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewCheckBoxCell cell = new DataGridViewCheckBoxCell();
                cell.Value = !block.Hidden;
                row.Cells.Add(cell);

                DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = block.Name;
                row.Cells.Add(cell2);

                DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
                cell3.Style.BackColor = block.LineColor;
                cell3.Style.SelectionBackColor = block.LineColor;
                row.Cells.Add(cell3);

                dataGridViewBlock.Rows.Add(row);
            }
        }

        public ArrayList Layers
        {
            get { return m_arrLayers; }
            set { m_arrLayers = value; }
        }

        public ArrayList Blocks
        {
            get { return m_arrBlocks; }
            set { m_arrBlocks = value; }
        }

        private void LeftPanel_SizeChanged(object sender, EventArgs e)
        {
            dataGridViewLayer.Location = new Point(0, 0);
            dataGridViewLayer.Size = new Size(this.Size.Width, this.Size.Height / 2);

            dataGridViewBlock.Location = new Point(0, this.Size.Height / 2);
            dataGridViewBlock.Size = new Size(this.Size.Width, this.Size.Height / 2);
        }

        private void dataGridViewLayer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                MessageBox.Show(string.Format("({0}, {1}) cell clicked", e.RowIndex, e.ColumnIndex));
            }
        }

        private void dataGridViewBlock_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            string strBlockName = dataGridViewBlock.Rows[e.RowIndex].Cells[1].Value.ToString();

            foreach (DXFViewer.Layer layer in m_dxfControl.Layers)
            {
                foreach (DXFViewer.Shape shape in layer.Shapes)
                {
                    DXFViewer.Block block = shape.GetBlock();

                    if (block == null)
                        shape.Selected = false;
                    else
                    {
                        if (block.Name == strBlockName)
                        {
                            shape.Selected = true;
                            shape.Selectable = true;
                        }
                        else
                            shape.Selected = false;
                    }
                }
            }

            m_dxfControl._Refresh();
        }
    }
}
