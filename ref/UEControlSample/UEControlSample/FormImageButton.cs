using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UEControlSample
{
    public partial class FormImageButton : Form, UnE.GUI.IImageButtonOwner
    {
        public FormImageButton()
        {
            InitializeComponent();
            this.TopLevel = false;
        }

        private void FormImageButton_Load(object sender, EventArgs e)
        {
            imageButton1.Owner = this;
            imageButton2.Owner = this;
        }

        public void OnImageButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnImageButtonMouseUp(object sender, MouseEventArgs e)
        {
            UnE.GUI.ImageButton btn = (UnE.GUI.ImageButton)sender;

            System.Diagnostics.Trace.WriteLine(btn.Name + " is clicked");
        }
    }
}
