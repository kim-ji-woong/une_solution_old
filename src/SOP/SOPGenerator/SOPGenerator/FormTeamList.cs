using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPGen
{
    public partial class FormTeamList : Form
    {
        private ArrayList m_arrItems = null;
        private int m_nSelectedIndex = -1;

        public FormTeamList(ArrayList arrItems)
        {
            m_arrItems = arrItems;
            InitializeComponent();
        }

        private void FormTeamList_Load(object sender, EventArgs e)
        {
            if (m_arrItems != null)
            {
                foreach (TeamData data in m_arrItems)
                {
                    DataGridViewRow gridRow = new DataGridViewRow();
                    DataGridViewCell cell = new DataGridViewImageCell();

                    cell = new DataGridViewTextBoxCell();
                    cell.Value = data.FullName;
                    gridRow.Cells.Add(cell);

                    teamDataGrid.Rows.Add(gridRow);
                }
            }
        }

        public int GetSelectedIndex()
        {
            return m_nSelectedIndex;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            m_nSelectedIndex = -1;

            foreach (DataGridViewCell cell in teamDataGrid.SelectedCells)
            {
                if (m_nSelectedIndex < cell.RowIndex)
                    m_nSelectedIndex = cell.RowIndex;
            }

            //foreach (DataGridViewRow row in teamDataGrid.SelectedRows)
            //{
            //    if (m_nSelectedIndex < row.Index)
            //        m_nSelectedIndex = row.Index;
            //}

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void teamDataGrid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                btnSelect_Click(null, null);
            }
        }
    }
}
