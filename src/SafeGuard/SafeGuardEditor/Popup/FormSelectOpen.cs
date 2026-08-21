using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager
{
    public partial class FormSelectOpen : Form
    {
        public FormSelectOpen()
        {
            InitializeComponent();

            button2.Text = "화재, 일반재해" + Environment.NewLine + "시나리오";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.Close();
        }
    }
}
