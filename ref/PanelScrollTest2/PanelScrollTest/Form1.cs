using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            panel2.Location = new Point(0, 0);

            this.MouseWheel += OnMouseWheel;
        }

        void OnMouseWheel(object sender, MouseEventArgs e)
        {
           
        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            panel2.ZoomIn();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel2.ZoomOut();
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {
            Size size = this.Size;

            if (panel1.VerticalScroll.Visible == true)
            {
                panel2.Width = this.ClientSize.Width - 15;
            }
            else
            {
                panel2.Width = this.ClientSize.Width;
            }

            if (panel1.HorizontalScroll.Visible == true)
            {
                panel2.Height = this.ClientSize.Height - 15 - panel3.Height;
            }
            else
            {
                panel2.Height = this.ClientSize.Height - 1;
            }
            

            panel1.PerformLayout();
        }
    }
}
