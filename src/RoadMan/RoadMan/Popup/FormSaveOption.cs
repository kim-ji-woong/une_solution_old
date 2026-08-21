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
    public partial class FormSaveOption : Form
    {
        private XMLManager.FileOption m_option = XMLManager.FileOption.NO_PASSWORD;
        private string m_strPassword = "";

        public XMLManager.FileOption FileOption
        {
            get { return m_option; }
        }

        public string Password
        {
            get { return m_strPassword; }
        }

        public FormSaveOption()
        {
            InitializeComponent();

            radio_CheckedChanged(radioNoPassword, null);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (radioNoPassword.Checked)
                m_option = XMLManager.FileOption.NO_PASSWORD;
            else if (radioSaveOnly.Checked)
                m_option = XMLManager.FileOption.PASSWORD_SAVE_ONLY;
            else if (radioReadWrite.Checked)
                m_option = XMLManager.FileOption.PASSWORD_READ_WRITE;

            m_strPassword = textBoxPassword.Text;

            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == radioNoPassword)
            {
                textBoxPassword.Enabled = !radioNoPassword.Checked;
            }
            else
            {
                RadioButton radio = (RadioButton)sender;
                textBoxPassword.Enabled = radio.Checked;
            }
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOK_Click(null, null);
        }
    }
}
