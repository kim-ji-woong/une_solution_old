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
    public partial class FormPassword : Form
    {
        private string m_strPassword = "";
        private bool m_open4Save = true;

        public string Password
        {
            get { return m_strPassword; }
        }

        public bool Open4Save
        {
            get { return m_open4Save; }
            set { m_open4Save = value; }
        }

        public FormPassword()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_strPassword = textBoxKey.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void FormPassword_Load(object sender, EventArgs e)
        {
            if (m_open4Save)
                label2.Text = "파일 변경을 위하여 암호를 입력해 주세요.";
            else
                label2.Text = "파일을 열기 위하여 암호를 입력해 주세요.";
        }

        private void textBoxKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOK_Click(null, null);
        }
    }
}
