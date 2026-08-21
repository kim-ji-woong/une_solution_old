using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DXFViewer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            //Application.Idle += Application_Idle;
        }

        void Application_Idle(object sender, EventArgs e)
        {
            //pictureBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "dxf files (*.dxf)|*.dxf";

            openFileDialog1.DefaultExt = "dxf";
            
            
            if( openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.OpenDXF(openFileDialog1.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.LoadHomeMatrix(true);
        }
    }
}
