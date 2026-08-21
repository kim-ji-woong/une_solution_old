using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace TeamManagementSystem
{
    public partial class FormTeamList : Form
    {
        private ArrayList m_arrItems = null;
        private ArrayList m_arrNoUsingItems = new ArrayList();
        private int m_nSelectedIndex = -1;

        public FormTeamList(ArrayList arrItems)
        {
            m_arrItems = arrItems;
            InitializeComponent();
        }

        public void AddNoUsingItem(string strItem)
        {
            m_arrNoUsingItems.Add(strItem);
        }

        private void FormTeamList_Load(object sender, EventArgs e)
        {
            if (m_arrItems != null)
            {
                foreach (SectionGrid section in m_arrItems)
                {
                    string strFullPath = SectionGrid.GetFullPath(section);

                    DataGridViewRow gridRow = new DataGridViewRow();
                    DataGridViewCell cell = new DataGridViewTextBoxCell();

                    cell.Value = strFullPath;
                    gridRow.Cells.Add(cell);

                    teamDataGrid.Rows.Add(gridRow);

                    if (m_arrNoUsingItems.Contains(strFullPath))
                    {
                        gridRow.Cells[0].Style.BackColor = Color.Brown;
                        gridRow.Cells[0].Tag = -1;
                    }
                }
            }
        }

        public int GetSelectedIndex()
        {
            return m_nSelectedIndex;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell cell in teamDataGrid.SelectedCells)
            {
                try
                {
                    if ((int)cell.Tag < 0)
                    {
                        MessageBox.Show("Brown으로 표시된 아이템들은 선택할 수 없습니다.");
                        return;
                    }
                }
                catch (Exception)
                {
                }

                m_nSelectedIndex = cell.RowIndex;
                break;
            }

            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void teamDataGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                btnSelect_Click(null, null);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
