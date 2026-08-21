using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LoginKeyMaker
{
    public partial class FormMacAddrList : Form
    {
        private string m_strMacAddressList = "";
        public string MacAddressList
        {
            get { return m_strMacAddressList; }
        }

        public FormMacAddrList()
        {
            InitializeComponent();
        }

        public FormMacAddrList(string strMacAddressList)
        {
            InitializeComponent();
            SetMacAddressString(strMacAddressList);
        }

        public void SetMacAddressString(string strMacAddressList)
        {
            dataGridView1.Rows.Clear();

            char[] arrParams = new char[2] { ',', ';' };
            string[] arrList = strMacAddressList.Split(arrParams);

            foreach (string strMacAddress in arrList)
            {
                string str = TrimString(strMacAddress);
                str = str.ToUpper();

                if (str.Length > 0)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();

                    cell.Value = str;
                    row.Cells.Add(cell);
                    dataGridView1.Rows.Add(row);
                }
            }
        }

        // 앞 뒤의 공백문자를 제거
        public static string TrimString(string str)
        {
            str = str.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            str = str.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
            return str;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_strMacAddressList = "";

            int nRowCount = dataGridView1.Rows.Count;

            for (int i = 1; i < nRowCount;i++ )
            {
                DataGridViewRow row = dataGridView1.Rows[i - 1];

                if (row.Cells[0].Value == null)
                    continue;

                string strMacAddress = row.Cells[0].Value.ToString().ToUpper();

                if (strMacAddress.Length > 0)
                {
                    if (m_strMacAddressList.Length == 0)
                        m_strMacAddressList = strMacAddress;
                    else
                        m_strMacAddressList += ";" + strMacAddress;
                }
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}
