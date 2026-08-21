using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDMS
{
    public class FormFrame : UnE.GUI.FormNoFrameSizable
    {
        private static FormFrame m_instance = null;
        public static FormFrame Instance
        {
            get { return m_instance; }
        }

        public FormFrame(Form frmMain)
            : base(frmMain)
        {
            m_instance = this;

            this.Load += new EventHandler(FormFrame_Load);
            this.FormClosing += new FormClosingEventHandler(FormFrame_FormClosing);
        }

        private void FormFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_frmMain != null)
            {
                m_frmMain.Visible = false;
                m_frmMain.Close();
            }
        }

        private void FormFrame_Load(object sender, EventArgs e)
        {
            this.TitleBarHeight = 0;
            this.Icon = m_frmMain.Icon;
        }

        protected override void EdgePanelMouseUp(object sender, MouseEventArgs e)
        {
            base.EdgePanelMouseUp(sender, e);

            if (m_frmMain != null)
                m_frmMain.Refresh();
        }
    }
}
