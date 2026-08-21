using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager.Popup
{
    public partial class FormEditUserDefinedParameterConfig : Form
    {
        private IEditItemOwner m_owner = null;
        private string m_strEditCellOriginText = "";
        private bool m_systemCall = false;

        public FormEditUserDefinedParameterConfig(string strTitle, List<string> items, IEditItemOwner owner)
        {
            InitializeComponent();

            this.Text = strTitle;
            m_systemCall = true;

            foreach (string strItem in items)
            {
                int nRowIndex = gridItems.Rows.Add();
                DataGridViewRow row = gridItems.Rows[nRowIndex];
                row.Cells[0].Value = strItem;
            }

            try
            {
                gridItems.Rows[0].Cells[0].Selected = true;
            }
            catch{ }

            m_owner = owner;
            m_systemCall = false;

            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            FormMain.Instance.UpdateWindowRate(label1, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(gridItems, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnRemove, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnRename, WindowRateWidth, WindowRateHeight);
            FormMain.Instance.UpdateWindowRate(btnClose, WindowRateWidth, WindowRateHeight);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (gridItems.SelectedCells.Count > 0)
            {
                int nRowIndex = gridItems.SelectedCells[0].RowIndex;
                DataGridViewRow row = gridItems.Rows[nRowIndex];

                string strSelectedItem = row.Cells[0].Value.ToString();

                if (m_owner != null && m_owner.OnRemoveItem(strSelectedItem))
                {
                    gridItems.Rows.RemoveAt(nRowIndex);

                    if (gridItems.Rows.Count > 0)
                        gridItems.Rows[0].Cells[0].Selected = true;
                }
            }
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            if (gridItems.SelectedCells.Count > 0)
            {
                int nRowIndex = gridItems.SelectedCells[0].RowIndex;
                DataGridViewRow row = gridItems.Rows[nRowIndex];

                gridItems.CurrentCell = row.Cells[0];
                gridItems.BeginEdit(false);
            }
        }

        private void gridItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (m_systemCall)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0 || e.ColumnIndex >= 1)
                return;

            DataGridViewCell cell = gridItems.Rows[e.RowIndex].Cells[e.ColumnIndex];
            string strValue = cell.Value == null ? "" : cell.Value.ToString().Trim();

            if (m_strEditCellOriginText == "" || (m_owner != null && m_owner.OnRenameItem(m_strEditCellOriginText, strValue)))
                m_strEditCellOriginText = strValue;
            else
            {
                MessageBox.Show(strValue + "는 사용할 수 없는 이름입니다.");
                cell.Value = m_strEditCellOriginText;
            }
        }

        private void gridItems_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (m_systemCall)
                return;

            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex < 0 || e.ColumnIndex >= 1)
                return;

            DataGridViewCell cell = gridItems.Rows[e.RowIndex].Cells[e.ColumnIndex];
            m_strEditCellOriginText = cell.Value.ToString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }

    public interface IEditItemOwner
    {
        bool OnRemoveItem(string strItemName);
        bool OnRenameItem(string strOriginItemName, string strNewItemName);
        bool IsValidName(string strItemName);
    }
}
