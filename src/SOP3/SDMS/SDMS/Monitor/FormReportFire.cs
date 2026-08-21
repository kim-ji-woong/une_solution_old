using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDMS
{
    public partial class FormReportFire : Form
    {
        [System.Runtime.InteropServices.DllImport("User32.dll", EntryPoint = "SetParent", ExactSpelling = false)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);

        public FormReportFire(FormMain frmParent)
        {            
            InitializeComponent();

            SetParent(this.Handle, frmParent.Handle);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("화재신고");
        }
    }
}
