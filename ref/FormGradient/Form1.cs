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
        GradientNotifier gradient = new GradientNotifier();
        public Form1()
        {           
            InitializeComponent();
            BackColor = Color.White;
            gradient.Interval = 500;
            gradient.GradientStep = 50;
            gradient.Size = new Size(200, 200);
            gradient.SetPosition(50, 50);
            gradient.Parent = this;
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            gradient.OnPaint(e);
            base.OnPaint(e);
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            gradient.Select();
            Refresh();
        }
    }
}
