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
    public partial class FormClockControl : Form
    {
        private DateTime m_dtPrev;

        public FormClockControl()
        {
            InitializeComponent();

            this.TopLevel = false;
        }

        private void FormClockControl_Load(object sender, EventArgs e)
        {
            m_dtPrev = DateTime.Now;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;

            if (dtNow.Minute != m_dtPrev.Minute && this.Visible)
            {
                m_dtPrev = dtNow;
                clockControl1.Refresh();
            }
        }
    }
}
