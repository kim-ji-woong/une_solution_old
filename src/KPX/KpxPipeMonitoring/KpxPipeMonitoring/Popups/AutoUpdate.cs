using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpxPipeMonitoring.Popups
{
    public partial class AutoUpdate : FormBase
    {
        public int Sec
        {
            get { return Convert.ToInt32(label_sec.Text); }
            set { label_sec.Text = value.ToString(); }
        }

        System.Windows.Forms.Timer timer = null;
        
        public AutoUpdate()
        {
            InitializeComponent(); 
            button2.Click += button2_Click;


            btnClose.Visible = false;

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (this.Sec == 0)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            }

            this.Sec = this.Sec-1;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            timer.Stop();
            timer.Dispose();   
        }

        void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
        } 
    }
}
