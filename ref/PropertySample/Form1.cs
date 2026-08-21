using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string szKey = textBox3.Text;
            string szValue = textBox1.Text;

            if (szKey != "" && szValue != "")
            {
                UnE.Utility.Properties.SetProperty(szKey, szValue);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string szKey = textBox2.Text;

            if (szKey != "")
            {
                string szValue = "";
                UnE.Utility.Properties.GetProperty(szKey, ref szValue);
                label1.Text = szValue;
            }
        }
    }
}
