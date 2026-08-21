using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.CCTV;
using UnE.Control;

namespace CCTVViewer
{
    public partial class TestForm : Form
    {
        private BigCCTVCtrl mForm = null;
        public TestForm(BigCCTVCtrl innerForm)
        {
            InitializeComponent();


            mForm = innerForm;
            innerForm.TopLevel = false;
            innerForm.Dock = DockStyle.Fill;
            
            panel1.Controls.Add(innerForm);
            innerForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(mForm != null)
            {
                mForm.CCTVCtrl.TestStop(9);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (mForm != null)
            {
                //mForm.CCTVCtrl.TestStop2(-1);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            object obj = comboBox1.SelectedItem;
            if (obj == null)
                return;

            int nCCTV = Convert.ToInt32(obj.ToString());
            if (mForm != null)
            {
                mForm.CCTVCtrl.TestStop(nCCTV);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            object obj = comboBox2.SelectedItem;
            if (obj == null)
                return;

            int nCCTV = Convert.ToInt32(obj.ToString());

            CCTVLoader loader = mForm.CCTVLoader;
            CCTV cctv = loader.LoadCCTV(nCCTV);

           
            

            BigCCTVCtrl innerForm = new BigCCTVCtrl(cctv, IntPtr.Zero);
            innerForm.CCTVLoader = mForm.CCTVLoader;

            if (mForm != null)
            {
                panel1.Controls.Remove(mForm);
                mForm.Close();
            }

            mForm = innerForm;
            innerForm.TopLevel = false;
            innerForm.Dock = DockStyle.Fill;
            panel1.Controls.Add(innerForm);
            innerForm.Show();
        }
    }
}
