using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RFIDReader
{
    public partial class FormMain : Form, IReaderOwner
    {
        private int m_nSelectedRowIndex = -1;
        private int m_nClickX = 0, m_nClickY = 0;
        private RFIDManager m_rfid = new RFIDManager();
        private static FormMain m_Instance = null;

        public static FormMain Instance
        {
            get { return m_Instance; }
        }

        public FormMain()
        {
            m_Instance = this;
            InitializeComponent();
            m_rfid.Owner = this;
        }

        private bool CheckDuplicate(string strTag, int nRowIndex)
        {
            int nRowCount = dataGridView1.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                if (i == nRowIndex)
                    continue;

                if ((string)dataGridView1.Rows[i].Cells[0].Value == strTag)
                    return false;
            }

            return true;
        }

        public void OnReadTag(string strTag)
        {
            if (dataGridView1.SelectedCells == null || dataGridView1.SelectedCells.Count == 0)
                return;

            int nSelectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;

            if (!CheckDuplicate(strTag, nSelectedRowIndex))
            {
                MessageBox.Show("이미 중복된 Tag가 존재합니다.");
                return;
            }

            dataGridView1.Rows[nSelectedRowIndex].Cells[0].Value = strTag;
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                int nRowCount = dataGridView1.Rows.Count;

                if (nRowCount <= 1)
                    return;

                if (e.RowIndex >= 0 && e.RowIndex < nRowCount - 1)
                {
                    m_nSelectedRowIndex = e.RowIndex;
                    contextMenuStrip1.Show(dataGridView1, m_nClickX, m_nClickY);
                }
            }
        }

        private void delRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_nSelectedRowIndex < 0)
                return;

            dataGridView1.Rows.RemoveAt(m_nSelectedRowIndex);
            m_nSelectedRowIndex = -1;
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            m_nClickX = e.X;
            m_nClickY = e.Y;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_rfid.StartReading();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            m_rfid.StartReading();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "fmf Files (*.csv)|*.csv| All Files (*.*)|*.*";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(dlg.FileName, false, Encoding.UTF8);
                writer.WriteLine("RFID Tag,\t설비 번호,\t기타");

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string strRFID = (string)row.Cells[0].Value;
                    if (strRFID == "")
                        continue;

                    writer.WriteLine(string.Format("{0},\t{1},\t{2}", strRFID, (string)row.Cells[1].Value, (string)row.Cells[2].Value));
                }

                writer.Close();
                MessageBox.Show("내보내기 완료");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("모두 삭제하시겠습니까?", "알림", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                dataGridView1.Rows.Clear();
            }
        }
    }
}
