using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS.PopupDialog.DisasterPrevention
{
    public partial class FormDPMsgBox : Form
    {
        public FormDPMsgBox(string title, string msg, MessageBoxButtons buttons)
        {
            InitializeComponent();

            this.Text = title;
            this.label1.Text = msg;

            if (buttons == MessageBoxButtons.OK)
            {
                button_no.Visible = false;
                button_yes.Visible = false;
                button_ok.Visible = true;
            }
            else if (buttons == MessageBoxButtons.YesNo)
            {
                button_no.Visible = true;
                button_yes.Visible = true;
                button_ok.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
                 
        private void button_yes_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
        }

        private void button_no_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
        }
    }
}
