using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.SenarioMaker
{
    public partial class FormFindPassword : Form
    {

        public FormFindPassword()
        {
            InitializeComponent();
            this.TopLevel = false;
            
            //m_formParent = form;

            cboAsk.SelectedIndex = 0;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
           // m_formParent.ShowLoginForm();
        }
    }
}
