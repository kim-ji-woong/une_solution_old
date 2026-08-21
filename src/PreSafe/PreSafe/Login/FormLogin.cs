using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreSafe
{
    internal partial class FormLogin : Form
    {
        private FormLoginMain m_formParent = null;
        public FormLogin(FormLoginMain form)
        {
            InitializeComponent();
            this.TopLevel = false;

            m_formParent = form;
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            //FormLoginMain.Instance.Dispose();

        }

        private void btnRegMember_Click(object sender, EventArgs e)
        {
            m_formParent.ShowRegisterForm();
        }
    }
}
