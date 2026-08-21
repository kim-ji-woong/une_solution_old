using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.SenarioMaker
{
    internal partial class FormNewScenario : Form
    {

        private string m_szSenarioName = "";
        public string SenarioName
        {
            get 
            {
                return m_szSenarioName; 
            }
            set { m_szSenarioName = value; }
        }

        private int m_nSenarioType = 1;
        public int SenarioType
        {
            get 
            {
                return m_nSenarioType; 
            }
            set { m_nSenarioType = value; }
        }
        
        public FormNewScenario()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if( mTextSenario.Text == null || mTextSenario.Text == "")
            {

                UnE.Utility.UMessageBox.Show("시나리오 이름을 입력하세요", "새시나리오", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            m_szSenarioName = mTextSenario.Text;

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Rb2_CheckedChanged(object sender, EventArgs e)
        {
            if (mRb2.Checked == true)
            {
                m_nSenarioType = 2;
            }
        }

        private void Rb1_CheckedChanged(object sender, EventArgs e)
        {
            if (mRb1.Checked == true)
            {
                m_nSenarioType = 1;
            }
        }

        /*private void Rb3_CheckedChanged(object sender, EventArgs e)
        {
            if (mRb3.Checked == true)
            {
                m_nSenarioType = 3;
            }
        }

        private void mRb4_CheckedChanged(object sender, EventArgs e)
        {
            if (mRb4.Checked == true)
            {
                m_nSenarioType = 4;
            }
        }

        private void mRb5_CheckedChanged(object sender, EventArgs e)
        {
            if (mRb5.Checked == true)
            {
                m_nSenarioType = 5;
            }
        }

        private void mRb6_CheckedChanged(object sender, EventArgs e)
        {
            if (mRb5.Checked == true)
            {
                m_nSenarioType = 6;
            }
        }*/

       
       
    }
}
