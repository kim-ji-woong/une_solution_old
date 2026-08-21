using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class MessageBoxEx : Form
    {
        public MessageBoxEx()
        {
            InitializeComponent();
        }

        private void MessageBoxEx_Load(object sender, EventArgs e)
        {
            this.pictureBox1.BackgroundImageLayout = ImageLayout.Center;
            this.pictureBox1.BackgroundImage = Bitmap.FromHicon(System.Drawing.SystemIcons.Asterisk.Handle);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        private void btnYesAll_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Ignore;
            this.Close();
        }
    }
}
