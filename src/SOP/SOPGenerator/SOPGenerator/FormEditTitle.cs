using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPGen
{
    public partial class FormEditTitle : Form
    {
        FormDocking m_Docking = null;

        public FormEditTitle(FormDocking dock)
        {
            InitializeComponent();

            m_Docking = dock;

            GetTitle();
        }

        public void GetTitle()
        {
            if (m_Docking.AddCheck)
                textTitle.Text = m_Docking.GetTitle();
            else
            {
                this.Text = "상황전파 수정";
                textTitle.Text = m_Docking.GetTitle();
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (Utility.TrimString(textTitle.Text) == "")
            {
                MessageBox.Show("창 이름이 빈 문자열입니다.");
                return;
            }

            m_Docking.SetEditTitle(textTitle.Text);
            this.Close();
        }

        private void textTitle_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(sender, e);
            }
        }
    }
}
