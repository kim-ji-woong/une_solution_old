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
    public partial class FormShowLayer : Form
    {
        private List<LayerData> m_layersAll = null;

      	private int m_nSpaceAll = 0;
        private int m_nSpaceProcess = 0;

        // 전체
        public List<LayerData> AllLayers
        {
            get { return m_layersAll; }
            set { m_layersAll = value; }
        }
	
		public FormShowLayer()
        {
            InitializeComponent();
        }

        private void FormProcessLayer_Load(object sender, EventArgs e)
        {
            InitGrid();
            SetLayers(m_layersAll);
         
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
            m_nSpaceProcess = gridShow.Location.X - (pictureBoxInsert.Location.X + pictureBoxInsert.Size.Width);
        }

        
        private void SetLayers(List<LayerData> layers)
        {
            gridAll.Rows.Clear();
			gridShow.Rows.Clear();

            foreach (LayerData data in layers)
            {
				if( data.Enabled == true)
				{
					AddLayer(gridShow, data);
				}                
				AddLayer(gridAll, data);
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

      

        private void pictureBoxInsert_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in gridAll.SelectedCells)
            {
                DataGridViewRow row = gridAll.Rows[cell.RowIndex];
                LayerData layer = (LayerData)row.Tag;

                if (FindLayer(gridShow, layer) == null)
                {
					layer.Enabled = true;
                    AddLayer(gridShow, layer);					
                }
            }
        }

        private void pictureBoxRemove_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> removeRows = new List<DataGridViewRow>();

            foreach (DataGridViewCell cell in gridShow.SelectedCells)
            {
                DataGridViewRow row = gridShow.Rows[cell.RowIndex];
                LayerData layer = (LayerData)row.Tag;

                if (!removeRows.Contains(row))
                {
					layer.Enabled = false;
                    removeRows.Add(row);					
                }
            }

            foreach (DataGridViewRow row in removeRows)
            {
                gridShow.Rows.Remove(row);
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
            gridShow.ClearSelection();
        }

        private void gridProcess_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            gridAll.ClearSelection();
        }

        private void FormProcessLayer_Resize(object sender, EventArgs e)
        {
            gridAll.Size = new Size(pictureBoxInsert.Location.X - gridAll.Location.X - m_nSpaceAll, gridAll.Size.Height);

            int nSpaceRight = this.Size.Width - gridShow.Location.X - gridShow.Size.Width;
            gridShow.Location = new Point(pictureBoxInsert.Location.X + pictureBoxInsert.Size.Width + m_nSpaceProcess, gridShow.Location.Y);
            gridShow.Size = new Size(this.Size.Width - gridShow.Location.X - nSpaceRight, gridShow.Size.Height);
        }
    }
}
