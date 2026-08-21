using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlTeamEditor
{
    public class ComboBoxDataGridView : DataGridView
    {
        private Dictionary<DataGridViewCell, ComboBox> m_dicCellComboBox = new Dictionary<DataGridViewCell, ComboBox>();

        public ComboBoxDataGridView()
        {
            this.DoubleBuffered = true;

            this.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnCellClick);
        }

        public void SetComboBox(DataGridViewCell cell, ComboBox cbo)
        {
            if (cbo == null)
            {
                if (m_dicCellComboBox.ContainsKey(cell))
                    this.Controls.Remove(m_dicCellComboBox[cell]);

                m_dicCellComboBox.Remove(cell);
            }
            else
            {
                m_dicCellComboBox[cell] = cbo;
                this.Controls.Add(cbo);

                System.Drawing.Rectangle rect = this.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);

                cbo.DropDownStyle = ComboBoxStyle.DropDownList;
                cbo.Location = rect.Location;
                cbo.Size = rect.Size;

                cbo.Tag = cell;
                cbo.SelectedIndexChanged += new System.EventHandler(this.comboBox_SelectedIndexChanged);

                cbo.Hide();
            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            DataGridViewCell cell = (DataGridViewCell)cbo.Tag;
            cell.Value = cbo.Items[cbo.SelectedIndex];
            cbo.Hide();
        }

        private void OnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                HideComboBox(null);
                return;
            }

            ComboBox cbo;
            DataGridViewCell cell = this.Rows[e.RowIndex].Cells[e.ColumnIndex];

            if (m_dicCellComboBox.TryGetValue(cell, out cbo))
            {
                if (cell.Value != null)
                {
                    int nIndex = cbo.Items.IndexOf(cell.Value);

                    if (nIndex >= 0)
                        cbo.SelectedIndex = nIndex;
                }

                HideComboBox(cbo);
                cbo.Show();
            }
            else
                HideComboBox(null);
        }

        private void HideComboBox(ComboBox except)
        {
            foreach (KeyValuePair<DataGridViewCell, ComboBox> pair in m_dicCellComboBox)
            {
                if (pair.Value == except)
                    continue;

                pair.Value.Visible = false;
            }
        }
    }
}
