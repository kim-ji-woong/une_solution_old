using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireManagement
{
    public partial class Splash : Form
    {
        private FormFrame form = null;
        public Splash()
        {
            InitializeComponent();
        }
               

        private void Splash_Load(object sender, EventArgs e)
        {
          
            timer2.Interval = 100;
            timer2.Enabled = true;
            timer2.Start();
            timer1.Start();
            m_SplashTimer.Interval = 6000;
            m_SplashTimer.Enabled = true;
            m_SplashTimer.Start();
        }

        private void m_SplashTimer_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            timer2.Enabled = false;

            m_SplashTimer.Stop();
            m_SplashTimer.Enabled = false;
            this.Visible = false;            
            form.Show();
           
            
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;
            form = new FormFrame(new FormMain2());
           
        }

        static int count = 3;
        private void timer2_Tick(object sender, EventArgs e)
        {

            if( count  == 0)
                label2.Text = "남동발전소 소방설비관리자 System↗";
            if (count == 1)
                label2.Text = "남동발전소 소방설비관리자 System↘";
            if (count == 2)
                label2.Text = "남동발전소 소방설비관리자 System↙";
            if (count == 3)
                label2.Text = "남동발전소 소방설비관리자 System↖";

            count++;

            if (count == 4)
                count = 0;
        }
    }
}
