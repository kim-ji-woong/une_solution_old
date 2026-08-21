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
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_MouseClick(object sender, MouseEventArgs e)
        {
            Panel btn = new Panel();
            btn.AutoSize = false;
            btn.BackColor = Color.White;
            //btn.Visible = false;
            
            btn.Location = new Point(e.Location.X - 40, e.Location.Y - 40);         
            btn.Size = new Size(80, 80);
        
            panel2.Controls.Add(btn);            

            Rectangle rectBtn = btn.Bounds;
            Rectangle rect = panel2.ClientRectangle;
            rect = Rectangle.Union(rect, rectBtn);           

            int dx = 0;
            if( rect.X < 0)
                dx = Math.Abs(rect.X);

            int dy = 0;
            if( rect.Y < 0)
                dy = Math.Abs(rect.Y);           

            panel2.ClientSize = new Size(rect.Width , rect.Height );

            if (dx != 0 || dy != 0)
            {               
                foreach (Control c in panel2.Controls)
                {                    
                    c.Location = new Point(c.Location.X + dx, c.Location.Y + dy);
                }
            }          

            //btn.Focus();
            btn.Select();
            panel1.ScrollControlIntoView(btn);
            panel1.PerformLayout();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Scroll
        }


    }
}
