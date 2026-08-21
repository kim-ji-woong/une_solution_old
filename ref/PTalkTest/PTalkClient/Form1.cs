using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PTalkClient
{
    public partial class Form1 : Form
    {
        UnE.TRS.PTalkLib libTrs = new UnE.TRS.PTalkLib();
        public Form1()
        {
            InitializeComponent();

            libTrs.SetTrsNumber(100150003);
            libTrs.SetLoginInfo("www.ptalk20.kr", "une0003", "ktp1234!");

            libTrs.InitPtalk();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            libTrs.CallPrivate(100150005);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            libTrs.PttOff();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            libTrs.CallEnd();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            libTrs.PttOff();

            string szTemp = textBox1.Text.Replace("\"", ",,,");
            string szMsg = ",,,,,,,,,," + szTemp.Replace(".", ",,,");
            libTrs.SendTTS(100150005, szMsg);
        }
    }
}
