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


namespace NamedPipeServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private PipeControl m_PipeServer;

        private void btnBegin_Click(object sender, EventArgs e)
        {
            string szPipeName = textPipeName.Text;
            if( szPipeName != null && szPipeName != "")
            {
                m_PipeServer = new PipeControl(true, szPipeName);
                m_PipeServer.BeginPipe(szPipeName);
                m_PipeServer.OnReciveMessage += OnDataCmd;
            }
        }

        public void OnDataCmd(string cmd)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if( cmd == "1")
                {
                    radioButton1.Checked = true;
                }
                else if (cmd == "2")
                {
                    radioButton2.Checked = true;
                }
                else if (cmd == "3")
                {
                    radioButton3.Checked = true;
                }
            });
        }

        private void btnEndServer_Click(object sender, EventArgs e)
        {
            m_PipeServer.StopPipe();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_PipeServer.Dispose();
        }
    }
}
