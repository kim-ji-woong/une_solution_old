using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer.uControl
{
    public partial class uSaveMessage : Form
    {
        private string m_strMessage = "";
        public uSaveMessage(string strMessage)
        {
            InitializeComponent();
            m_strMessage = strMessage;
        }

        private void uSaveMessageLoad(object sender, EventArgs e)
        {
            lblMessage.Text = m_strMessage;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //폼움직이게
        private Point mousePoint;
        private void USaveMessage_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void USaveMessage_MouseMove(object sender, MouseEventArgs e)
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
