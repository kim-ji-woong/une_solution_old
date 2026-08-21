using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibraryReader
{
    public partial class FormSelectLibrary2 : Form
    {
        private string m_strCoord = "";
        private static Point m_ptLastPos = new Point(-1000000, -1000000);
        private bool m_goBack = false, m_stopProgress = false;
        private Dictionary<string, string> m_dicOldTypeAddress = new Dictionary<string, string>();
        private string m_strFinalAddr = "";

        public string SelectedCoord
        {
            get { return m_strCoord; }
        }

        public bool GoBack
        {
            get { return m_goBack; }
        }

        public bool EnableGoBack
        {
            set { btnGoBack.Enabled = value; }
        }

        public bool StopProgress
        {
            get { return m_stopProgress; }
        }

        public string FinalAddress
        {
            get { return m_strFinalAddr; }
        }

        // dicLibraries
        // Key : 주소
        // Value : 좌표
        // dicOldTypeAddress
        // Key : 도로명 주소
        // Value : 지번 주소
        public FormSelectLibrary2(string strAddr, Dictionary<string, string> dicLibraries, Dictionary<string, string> dicOldTypeAddress)
        {
            InitializeComponent();

            textBoxAddr.Text = strAddr;
            Clipboard.SetText(strAddr);

            m_dicOldTypeAddress = dicOldTypeAddress;
            SetOldTypeAddress();

            SetGrid(dicLibraries);

            if (m_ptLastPos.X == -1000000)
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
            else
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = m_ptLastPos;
            }
        }

        private void SetOldTypeAddress()
        {
            string strOldTypeAddr;

            if (m_dicOldTypeAddress.TryGetValue(textBoxAddr.Text, out strOldTypeAddr))
                labelOldTypeAddress.Text = strOldTypeAddr;
            else
                labelOldTypeAddress.Text = textBoxAddr.Text;
        }

        private void SetGrid(Dictionary<string, string> dicLibraries)
        {
            dataGridView1.Rows.Clear();

            foreach (KeyValuePair<string, string> pair in dicLibraries)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = pair.Key;
                row.Cells.Add(cell);

                cell = new DataGridViewTextBoxCell();
                cell.Value = pair.Value;
                row.Cells.Add(cell);

                dataGridView1.Rows.Add(row);
                row.Tag = pair.Value;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                MessageBox.Show("도서관을 선택하세요.");
                return;
            }

            int nRowIndex = dataGridView1.SelectedCells[0].RowIndex;
            
            object obj = dataGridView1.Rows[nRowIndex].Tag;
            m_strCoord = (string)obj;

            m_strFinalAddr = textBoxAddr.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SetOldTypeAddress();
            Dictionary<string, string> dicAddr = DBManagerMySQL.GetAddressCoord(labelOldTypeAddress.Text);
            SetGrid(dicAddr);
        }

        private void textBoxAddr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSearch_Click(null, null);
        }

        private void FormSelectLibrary2_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_ptLastPos = this.Location;
        }

        private void btnGoBack_Click(object sender, EventArgs e)
        {
            m_goBack = true;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnStopProgress_Click(object sender, EventArgs e)
        {
            m_stopProgress = true;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
