using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UBMLViewer
{
    public partial class Splash : Form
    {
        private FormMain mainForm = null;
        public Splash(FormMain parent)
        {
            mainForm = parent;
            InitializeComponent();

            Rectangle rect = parent.Bounds;
            Rectangle rect2 = Bounds;

            int cw = parent.Location.X + (int)(( rect.Width )* 0.5);
            int ch = parent.Location.Y + (int)((rect.Height) * 0.5);

            int posx = cw - (int)((rect2.Width) * 0.5);
            int posy = ch - (int)((rect2.Height) * 0.5);

            this.Location = new Point(posx, posy);
           
            Bitmap m_Image = (Bitmap)FormMain.GetImageByName("Logo_UnE");
            this.m_SplashPictureBox.Image = (Image)m_Image;
            m_SplashTimer.Enabled = false;
        }
               

        private void Splash_Load(object sender, EventArgs e)
        {
            m_SplashTimer.Interval = 5000;
            m_SplashTimer.Enabled = true;            
        }

        private void m_SplashTimer_Tick(object sender, EventArgs e)
        {
            mainForm.Visible = true;
            m_SplashTimer.Stop();
            m_SplashTimer.Enabled = false;
            this.Close();
            
            
        }
    }
}
