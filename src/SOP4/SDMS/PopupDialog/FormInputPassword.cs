using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS.PopupDialog
{
    public partial class FormInputPassword : Form
    {
        private string m_strPassword = "";

        public string Title
        {
            get { return labelTitle.Text; }
            set { labelTitle.Text = value; }
        }

        public string Password
        {
            get { return m_strPassword; }
        }

        public FormInputPassword()
        {
            InitializeComponent();
        }

        public FormInputPassword(string strTItle)
        {
            InitializeComponent();
            Title = strTItle;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            m_strPassword = textBoxPassword.Text;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOK_Click(null, null);
        }
    }
}
