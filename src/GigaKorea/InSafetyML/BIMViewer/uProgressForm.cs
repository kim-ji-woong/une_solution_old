using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer
{
    public partial class uProgressForm : Form
    {
        private int m_UpDown = 0;//is Uploading. Downloading is 1

        private DateTime m_dtStartTime;
        public DateTime StartTime
        {
            get { return m_dtStartTime; }
        }

        public uProgressForm(int nType)
        {
            InitializeComponent();
            m_UpDown = nType;

            m_dtStartTime = DateTime.Now;
            lbStartTime.Text = m_dtStartTime.ToString();
        }
        public void SetProgress(string sMessage, int nPercent)
        {
            lblMessage.Text = sMessage;
            if (m_UpDown == 0)
                progressBar1.Value = nPercent;
        }

        private void UProgressForm_Load(object sender, EventArgs e)
        {
            if(m_UpDown == 0)
            {
                lblMessage.Text = "공간데이터 Uploading...";
                progressBar1.Style = ProgressBarStyle.Continuous;
            }
            else
            {
                lblMessage.Text = "공간데이터 Downloading...";
                progressBar1.Style = ProgressBarStyle.Marquee;
            }
        }

        private void ImgBtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //폼움직이게
        private Point mousePoint;
        private void UProgressForm_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void UProgressForm_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Location = new Point(this.Left - (mousePoint.X - e.X), this.Top - (mousePoint.Y - e.Y));
            }
        }

        private void Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void Panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Location = new Point(this.Left - (mousePoint.X - e.X), this.Top - (mousePoint.Y - e.Y));
            }
        }
    }
}
