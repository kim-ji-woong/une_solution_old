using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace HSMS
{
    public partial class FormAddGroup : Form
    {
        private string m_strTitle = "Group 추가";
        private ArrayList m_arrGroupNames = new ArrayList();
        private string m_strGridHeader = "Group Name";
        private string m_strNewGroupName = "";
        private string m_strDefGroupName = "";
        private string m_strDefGroupNickName = "";

        public string NewGroupName
        {
            get { return m_strNewGroupName; }
        }

        public string DefGroupName
        {
            get { return m_strDefGroupName; }
            set { m_strDefGroupName = value; }
        }

        public string DefGroupNickName
        {
            get { return m_strDefGroupNickName; }
            set { m_strDefGroupNickName = value; }
        }

        public FormAddGroup()
        {
            InitializeComponent();
        }

        private void FormAddGroup_Load(object sender, EventArgs e)
        {
            Text = m_strTitle;

            InitGrid();
        }

        public void SetTitle(string strTitle)
        {
            m_strTitle = strTitle;
        }

        public void SetGridHeader(string strGridHeader)
        {
            m_strGridHeader = strGridHeader;
        }

        public void AddGroupName(string strGroupName)
        {
            if (!m_arrGroupNames.Contains(strGroupName))
                m_arrGroupNames.Add(strGroupName);
        }

        private void InitGrid()
        {
            colGroupName.HeaderText = m_strGridHeader;

            foreach (string strGroupName in m_arrGroupNames)
            {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();

                cell.Value = strGroupName;
                row.Cells.Add(cell);
                dataGridView1.Rows.Add(row);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string strGroupName = row.Cells[0].Value.ToString();

                if (textBoxNewGroupName.Text == strGroupName)
                {
                    MessageBox.Show("이미 같은 이름이 존재합니다.");
                    row.Cells[0].Selected = true;
                    return;
                }
            }

            if (textBoxNewGroupName.Text == m_strDefGroupName ||
                textBoxNewGroupName.Text == m_strDefGroupNickName)
            {
                MessageBox.Show("사용할 수 없는 이름입니다.");
                return;
            }

            m_strNewGroupName = textBoxNewGroupName.Text;
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBoxNewGroupName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOK_Click(null, null);
        }
    }
}
