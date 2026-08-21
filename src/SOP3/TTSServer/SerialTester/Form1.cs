using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BroadcastServer;

namespace SerialTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SerialManager smgr;
        private void Form1_Load(object sender, EventArgs e)
        {
            smgr = new SerialManager();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string szText = textBox1.Text;
            smgr.Port = szText;
            smgr.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            smgr.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string szText = textBox1.Text;
            smgr.Port = szText;
            smgr.CheckSwitch();
        }
    }
}
