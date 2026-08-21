using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamEditor.Popup
{
    public partial class FormExcelSheet : Form
    {
        private string m_strSheetName = "";

        public string SheetName
        {
            get { return m_strSheetName; }
        }

        public FormExcelSheet(List<string> sheetNames)
        {
            InitializeComponent();
            InitGrid(sheetNames);
        }

        private void InitGrid(List<string> sheetNames)
        {
            foreach (string strSheetName in sheetNames)
            {
                int nRowIndex = gridSheetNames.Rows.Add();

                if (nRowIndex < 0)
                    return;

                DataGridViewRow row = gridSheetNames.Rows[nRowIndex];
                row.Cells[0].Value = strSheetName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (gridSheetNames.SelectedCells.Count == 0)
            {
                MessageBox.Show("읽어들일 Excel Sheet를 선택하세요.");
                return;
            }

            m_strSheetName = gridSheetNames.SelectedCells[0].Value.ToString();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
