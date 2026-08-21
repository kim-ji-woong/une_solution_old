using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RoadMan
{
    public partial class FormProcessLayer : Form
    {
        private List<LayerData> m_layersAll = null;
        // 개설
        private List<LayerData> m_layersComplete = new List<LayerData>();
        // 미개설
        private List<LayerData> m_layersInComplete = new List<LayerData>();
        // 폭원미개설
        private List<LayerData> m_layersPartial = new List<LayerData>();
        private List<LayerData> m_layersCurrent = null;

        private int m_nSpaceAll = 0;
        private int m_nSpaceProcess = 0;

        // 전체
        public List<LayerData> AllLayers
        {
            get { return m_layersAll; }
            set { m_layersAll = value; }
        }

        // 개설
        public List<LayerData> CompleteLayers
        {
            get { return m_layersComplete; }
            set { SetLayerList(m_layersComplete, value); }
        }

        // 미개설
        public List<LayerData> IncompleteLayers
        {
            get { return m_layersInComplete; }
            set { SetLayerList(m_layersInComplete, value); }
        }

        // 폭원 미개설
        public List<LayerData> PartialLayers
        {
            get { return m_layersPartial; }
            set { SetLayerList(m_layersPartial, value); }
        }

        public FormProcessLayer()
        {
            InitializeComponent();
        }

        private void FormProcessLayer_Load(object sender, EventArgs e)
        {
            InitGrid();
            SetLayers(gridAll, m_layersAll);
            radio_CheckedChanged(null, null);

            gridAll.Select();
        }

        private void InitGrid()
        {
            colVisibleAll.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colLayerNameAll.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colColorAll.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            colVisibleProcess.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colLayerNameProcess.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colColorProcess.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            m_nSpaceAll = pictureBoxInsert.Location.X - (gridAll.Location.X + gridAll.Size.Width);
            m_nSpaceProcess = gridProcess.Location.X - (pictureBoxInsert.Location.X + pictureBoxInsert.Size.Width);
        }

        private void SetLayerList(List<LayerData> layersTrg, List<LayerData> layersSrc)
        {
            layersTrg.Clear();

            foreach (LayerData data in layersSrc)
            {
                layersTrg.Add(data);
            }
        }

        private void SetLayers(DataGridView grid, List<LayerData> layers)
        {
            grid.Rows.Clear();

            foreach (LayerData data in layers)
            {
                AddLayer(grid, data);
            }
        }

        private void AddLayer(DataGridView grid, LayerData data)
        {
            DataGridViewRow row = new DataGridViewRow();

            DataGridViewCheckBoxCell checkedCell = new DataGridViewCheckBoxCell();
            checkedCell.Value = data.Visible;
            row.Cells.Add(checkedCell);

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = data.LayerName;
            row.Cells.Add(cell);

            cell = new DataGridViewTextBoxCell();
            cell.Style.BackColor = data.Color;
            row.Cells.Add(cell);

            row.Tag = data;
            grid.Rows.Add(row);
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            if (radioComplete.Checked)
            {
                SetLayers(gridProcess, m_layersComplete);
                m_layersCurrent = m_layersComplete;
            }
            else if (radioIncomplete.Checked)
            {
                SetLayers(gridProcess, m_layersInComplete);
                m_layersCurrent = m_layersInComplete;
            }
            else
            {
                SetLayers(gridProcess, m_layersPartial);
                m_layersCurrent = m_layersPartial;
            }
        }

        private void pictureBoxInsert_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in gridAll.SelectedCells)
            {
                DataGridViewRow row = gridAll.Rows[cell.RowIndex];
                LayerData layer = (LayerData)row.Tag;

                if (FindLayer(gridProcess, layer) == null)
                {
                    AddLayer(gridProcess, layer);
                    m_layersCurrent.Add(layer);
                }
            }
        }

        private void pictureBoxRemove_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> removeRows = new List<DataGridViewRow>();

            foreach (DataGridViewCell cell in gridProcess.SelectedCells)
            {
                DataGridViewRow row = gridProcess.Rows[cell.RowIndex];
                LayerData layer = (LayerData)row.Tag;

                if (!removeRows.Contains(row))
                {
                    removeRows.Add(row);
                    m_layersCurrent.Remove(layer);
                }
            }

            foreach (DataGridViewRow row in removeRows)
            {
                gridProcess.Rows.Remove(row);
            }
        }

        private DataGridViewRow FindLayer(DataGridView grid, LayerData layer)
        {
            foreach (DataGridViewRow row in grid.Rows)
            {
                LayerData data = (LayerData)row.Tag;

                if (data == layer)
                    return row;
            }

            return null;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void gridAll_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            gridProcess.ClearSelection();
        }

        private void gridProcess_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            gridAll.ClearSelection();
        }

        private void FormProcessLayer_Resize(object sender, EventArgs e)
        {
            gridAll.Size = new Size(pictureBoxInsert.Location.X - gridAll.Location.X - m_nSpaceAll, gridAll.Size.Height);

            int nSpaceRight = this.Size.Width - gridProcess.Location.X - gridProcess.Size.Width;
            gridProcess.Location = new Point(pictureBoxInsert.Location.X + pictureBoxInsert.Size.Width + m_nSpaceProcess, gridProcess.Location.Y);
            gridProcess.Size = new Size(this.Size.Width - gridProcess.Location.X - nSpaceRight, gridProcess.Size.Height);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == WindowMessage.WM_KEYDOWN ||
                msg.Msg == WindowMessage.WM_CHAR ||
                msg.Msg == WindowMessage.WM_SYSKEYDOWN)
            {
                if (keyData == Keys.F1)
                {
                    FormMain.Instance.ShowHelp();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
