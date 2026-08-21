using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pipelib;

namespace NamedPipeTest
{
    public partial class Form1 : Form
    {

        private PassivePipeServer client = null;

        public Form1()
        {
            InitializeComponent();
        }


        public void SendCmd(int n)
        {
            //if (client.IsConnected == true)
            {
                client.Send(n.ToString());
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            string szPipeName = textBox1.Text;
            if( szPipeName != null && szPipeName != "")
            {
                client = new PassivePipeServer(false, szPipeName);
                client.BeginPipe();
                client.OnReciveMessage += OnDataCmd;
            }         
        }

        public void OnDataCmd(string cmd)
        {
            System.Diagnostics.Trace.WriteLine(cmd);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendCmd(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendCmd(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SendCmd(2);
        }
    }
}
