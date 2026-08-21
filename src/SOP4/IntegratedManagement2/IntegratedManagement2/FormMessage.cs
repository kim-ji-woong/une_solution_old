using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IntegratedManagement2
{
    public partial class FormMessage : Form
    {
        public FormMessage()
        {
            InitializeComponent();
        }
        private bool m_bUpdate = true;
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private int m_nMinitue = 29;
        private int m_nSecond = 60;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_nSecond == 0)
            {
                m_nMinitue -= 1;

                m_nSecond = 60;
            }

            m_nSecond -= 1;

            this.label2.Text = string.Format("업데이트 자동시작 : {0:00}:{1:00}", m_nMinitue, m_nSecond);

            if (m_nMinitue <= 0 && m_nSecond <= 0)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void FormMessage_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
        }

        private void FormMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;
        }
    }
}
