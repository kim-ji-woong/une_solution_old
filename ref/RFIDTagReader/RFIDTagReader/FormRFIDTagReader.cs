using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace RFIDTagReader
{
    public partial class FormRFIDTagReader : Form, Ubists.IReaderOwner
    {
        private Ubists.RFIDReader m_reader = new Ubists.RFIDReader();
        private static FormRFIDTagReader m_instance = null;

        public static FormRFIDTagReader Instance
        {
            get { return m_instance; }
        }

        public FormRFIDTagReader()
        {
            m_instance = this;
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (textBoxFilePath.Text.Length == 0)
                MessageBox.Show("파일 경로를 입력하세요.");
            else
            {
                if (!File.Exists(textBoxFilePath.Text))
                    MessageBox.Show(textBoxFilePath.Text + "\r\n존재하지 않는 파일 경로입니다.");
                else
                {
                    if (OpenFile(textBoxFilePath.Text))
                    {
                        m_reader.Owner = this;
                        if (!m_reader.StartReading())
                        {
                            MessageBox.Show("RFID Reader와 연결할 수 없습니다.");
                            return;
                        }
                    }
                }
            }
        }

        private bool OpenFile(string strPath)
        {
            StreamReader reader = new StreamReader(strPath, Encoding.Default);
            bool isFirst = true;
            string strEquipID = "", strEquipName = "";

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();

                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }

                strLine = strLine.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                strLine = strLine.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                if (strLine.Length == 0)
                    continue;

                if (!ReadEquipInfo(strLine, ref strEquipID, ref strEquipName))
                {
                    reader.Close();
                    MessageBox.Show("잘못된 데이터 파일입니다.");
                    return false;
                }

                AddEquipment(strEquipID, strEquipName);
            }

            reader.Close();
            return true;
        }

        private void AddEquipment(string strEquipID, string strEquipName)
        {
            DataGridViewRow row = new DataGridViewRow();

            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
            cell.Value = strEquipID;
            row.Cells.Add(cell);
            cell.ReadOnly = true;
            
            cell = new DataGridViewTextBoxCell();
            cell.Value = cboEquipType.Text;
            row.Cells.Add(cell);
            cell.ReadOnly = true;
            
            cell = new DataGridViewTextBoxCell();
            cell.Value = strEquipName;
            row.Cells.Add(cell);
            cell.ReadOnly = true;
            
            cell = new DataGridViewTextBoxCell();
            cell.Value = "";
            row.Cells.Add(cell);
            
            dataGridView1.Rows.Add(row);
        }

        private bool ReadEquipInfo(string strLine, ref string strEquipID, ref string strEquipName)
        {
            int nIndex = strLine.IndexOf(',');

            if (nIndex < 0)
                return false;

            strEquipID = strLine.Substring(0, nIndex);
            strEquipName = strLine.Substring(nIndex + 1);

            strEquipName = strEquipName.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            strEquipName = strEquipName.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
            return true;
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "csv Files (*.csv)|*.csv";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxFilePath.Text = dlg.FileName;
            }
        }

        private void FormRFIDTagReader_Load(object sender, EventArgs e)
        {
            cboEquipType.SelectedIndex = 0;

            string strDefPath = Application.StartupPath + "\\sample.csv";

            if (File.Exists(strDefPath))
                textBoxFilePath.Text = strDefPath;
        }

        private void FormRFIDTagReader_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_reader.FinishReading(true);
        }

        public void OnReadTag(string strTag)
        {
            int nCount = dataGridView1.SelectedCells.Count;
            if (nCount == 0)
                return;

            int nRowIndex = dataGridView1.SelectedCells[0].RowIndex;

            if (CheckDuplication(nRowIndex, strTag))
                dataGridView1.Rows[nRowIndex].Cells[3].Value = strTag;
        }

        private bool CheckDuplication(int nRowIndex, string strTag)
        {
            int nRowCount = dataGridView1.Rows.Count;

            for (int i = 0; i < nRowCount; i++)
            {
                if (i == nRowIndex)
                    continue;

                if (dataGridView1.Rows[i].Cells[3].Value.ToString() == strTag)
                {
                    string strError = string.Format("데이터 {0}행, 설비번호 {1}, {2}에 이미 같은 RFID Tag가 존재합니다.",
                        nRowIndex + 1,
                        dataGridView1.Rows[i].Cells[0].Value.ToString(),
                        dataGridView1.Rows[i].Cells[2].Value.ToString());

                    MessageBox.Show(strError);
                    return false;
                }
            }

            return true;
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            dataGridView1.Focus();
            dataGridView1.MultiSelect = true;
            dataGridView1.SelectAll();
        }

        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            dataGridView1.MultiSelect = false;
        }
    }
}
