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
    public partial class FormMain : Form
    {
        private Form m_frmCurrent = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Location = new Point(0, 0);
                m_frmCurrent.Size = this.Size;
            }
        }

        private bool CheckCurrentForm(Type type)
        {
            if (m_frmCurrent != null)
            {
                if (m_frmCurrent.GetType() == type)
                    return false;
                else
                    this.Controls.Remove(m_frmCurrent);
            }

            return true;
        }

        private void SetCurrentForm(Form frm)
        {
            m_frmCurrent = frm;
            this.Controls.Add(m_frmCurrent);
            m_frmCurrent.Show();

            FormMain_Resize(null, null);
        }

        private void clockControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentForm(typeof(FormClockControl)))
                SetCurrentForm(new FormClockControl());
        }

        private void ribbonBtnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentForm(typeof(FormRibbonButton)))
                SetCurrentForm(new FormRibbonButton());
        }

        private void ribbonGaroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentForm(typeof(FormRibbonHorz)))
                SetCurrentForm(new FormRibbonHorz());
        }

        private void imageButtonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentForm(typeof(FormImageButton)))
                SetCurrentForm(new FormImageButton());
        }

        private void textPictureBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckCurrentForm(typeof(FormTextPictureBox)))
                SetCurrentForm(new FormTextPictureBox());
        }

        private void noFrameSizableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnE.GUI.FormNoFrameSizable frm = new UnE.GUI.FormNoFrameSizable(new FormNoFrame());
            frm.ShowDialog();
        }
    }
}
