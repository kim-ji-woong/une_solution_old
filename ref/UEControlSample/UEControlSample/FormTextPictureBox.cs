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
    public partial class FormTextPictureBox : Form, UnE.GUI.ITextPictureBoxOwner
    {
        public FormTextPictureBox()
        {
            InitializeComponent();
            this.TopLevel = false;
        }

        private void FormTextPictureBox_Load(object sender, EventArgs e)
        {
            textPictureBox1.Owner = this;
        }

        public void TextPictureBox_MouseDown(UnE.GUI.TextPictureBox pictureBox, MouseEventArgs e)
        {
        }

        public void TextPictureBox_MouseUp(UnE.GUI.TextPictureBox pictureBox, MouseEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine(pictureBox.Name + " is clicked");
        }
    }
}
