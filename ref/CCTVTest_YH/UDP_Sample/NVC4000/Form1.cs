using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NVC4000
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //axAxVCA1.MediaPassword = "pass";
            axAxVCA1.MediaStream = "channel=0,stream=0";
            axAxVCA1.MediaType = "rtp-tcp";
            axAxVCA1.MediaUsername = "root";
            axAxVCA1.MediaURL = "http://172.20.131.76";
            axAxVCA1.MediaPassword = "fa0e6a34fd25d96a";
            axAxVCA1.Play();
        }
    }
}
