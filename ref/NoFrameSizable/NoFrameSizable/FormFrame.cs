using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NoFrameSizable
{
    public class FormFrame : UnE.GUI.FormNoFrameSizable
    {
        private Button m_btn = new Button();

        public FormFrame(Form frmMain)
            : base(frmMain)
        {
            this.Load += new EventHandler(FormFrame_Load);
        }

        void FormFrame_Load(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Maximized;
        }
    }
}
