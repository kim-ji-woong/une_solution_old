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
    public partial class FormSheetNames : Form
    {
        private List<string> m_listSheetNames = null;
        private string m_strTargetSheetName = "";

        public string TargetSheetName
        {
            get { return m_strTargetSheetName; }
        }

        public FormSheetNames(List<string> sheetNames)
        {
            m_listSheetNames = sheetNames;
            InitializeComponent();
        }

        private void FormSheetNames_Load(object sender, EventArgs e)
        {
            int nIndex = 1;

            foreach (string strSheetName in m_listSheetNames)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = nIndex++;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = strSheetName;
                row.Cells.Add(cell);

                DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
                checkCell.Value = false;
                row.Cells.Add(checkCell);

                dataGridView1.Rows.Add(row);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 2)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dataGridView1.Rows[e.RowIndex].Cells[2];

                bool isChecked = (bool)cell.Value;

                if (isChecked)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Index == e.RowIndex)
                            continue;

                        row.Cells[2].Value = false;
                    }
                }
            }
        }

        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_strTargetSheetName = "";

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if ((bool)row.Cells[2].Value == true)
                {
                    m_strTargetSheetName = (string)row.Cells[1].Value;
                    break;
                }
            }

            if (m_strTargetSheetName == "")
            {
				string szMsg = "사용할 Excel Sheet를 선택하지 않았습니다.";
                UnE.Utility.UMessageBox.Show(this, szMsg, "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //MessageBox.Show("사용할 Excel Sheet를 선택하지 않았습니다.");
                return;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_strTargetSheetName = "";
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
