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
    public partial class uBottom : UserControl
    {
        private string m_strDate = "";
        public string DateText
        {
            get { return lblDate.Text; }
            set { lblDate.Text = value; }
        }
        public string TimeText
        {
            get { return lblTime.Text; }
            set { lblTime.Text = value; }
        }

        public Image BackImage = null;

        public uBottom()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            
            this.Size = new Size(1920, 81);
            //this.BackColor = picCalendar.BackColor = picClock.BackColor = picLogo.BackColor = picText.BackColor = Color.FromArgb(125, 0x00, 0x00, 0x00);

            picCalendar.Location = new Point(40, 27);
            picClock.Location = new Point(401, 28);
            picLogo.Location = new Point(1429, 15);
            picText.Location = new Point(1772, 30);
        }

        private void uBottom_Paint(object sender, PaintEventArgs e)
        {
            if (BackImage == null)
                return;

            Graphics g = e.Graphics;
            
            g.DrawImage(BackImage, 0, 0, this.Width, this.Height);
        }
    }
}
