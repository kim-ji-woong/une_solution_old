using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DidViewer.Composition
{
    public partial class uTop : UserControl
    {
        public bool PicWorking
        {
            get { return picWorking.Visible; }
            set { picWorking.Visible = value; }
        }

        public string HiCount
        {
            get { return lblHi.Text; }
            set { lblHi.Text = value; }
        }
        public string ByeCount
        {
            get { return lblBye.Text; }
            set { lblBye.Text = value; }
        }
        public string StayCount
        {
            get { return lblStay.Text; }
            set { lblStay.Text = value; }
        }

        public Image BackImage = null;

        public uTop()
        {
            InitializeComponent();

            this.DoubleBuffered = true;

            this.SetStyle(System.Windows.Forms.ControlStyles.UserPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.EnableNotifyMessage, true);

            FormMain.Instance.SetDoubleBuffer(lblHi, true);
            FormMain.Instance.SetDoubleBuffer(lblBye, true);
            FormMain.Instance.SetDoubleBuffer(lblStay, true);

            FormMain.Instance.SetDoubleBuffer(picSystemLogo, true);
            FormMain.Instance.SetDoubleBuffer(picSystemText, true);
            FormMain.Instance.SetDoubleBuffer(picWorking, true);
            FormMain.Instance.SetDoubleBuffer(picHi, true);
            FormMain.Instance.SetDoubleBuffer(picBye, true);
            FormMain.Instance.SetDoubleBuffer(picStay, true);

            this.Size = new Size(1920, 161);
            picSystemLogo.Location = new Point(40, 30);
            picSystemText.Location = new Point(155, 56);
            picWorking.Location = new Point(710, 33);
            labelAirValue.Location = new Point(picWorking.Location.X - labelAirValue.Width - 10, (this.Height - labelAirValue.Height) / 2);
            labelAir.Location = new Point(labelAirValue.Location.X - labelAir.Width - 5, (this.Height - labelAir.Height) / 2);

            picHi.Location = new Point(998, 40);
            picHiText.Location = new Point(1005, 92);
            panel1.Location = new Point(1265, 55);
            picBye.Location = new Point(1315, 40);
            picByeText.Location = new Point(1308, 92);
            panel2.Location = new Point(1582, 55);
            picStay.Location = new Point(1633, 45);
            picStayText.Location = new Point(1630, 92);

            panel1.BackColor = panel2.BackColor = panel3.BackColor = Color.FromArgb(127, 0xff, 0xff, 0xff);
            panel3.Location = new Point(40, 160);

            lblHi.Location = new Point(1085, 51);
            lblBye.Location = new Point(1402, 51);
            lblStay.Location = new Point(1720, 51);

            lblHi.ForeColor = Color.FromArgb(0x6c, 0xff, 0xc5);
            lblBye.ForeColor = Color.FromArgb(0x67, 0xff, 0xe9);
            lblStay.ForeColor = Color.FromArgb(0xff, 0xdf, 0x90);            
        }
        
        private void uTop_Paint(object sender, PaintEventArgs e)
        {
            if (BackImage == null)
                return;

            Graphics g = e.Graphics;

            g.DrawImage(BackImage, 0, 0, this.Width, this.Height);
        }
    }
}
