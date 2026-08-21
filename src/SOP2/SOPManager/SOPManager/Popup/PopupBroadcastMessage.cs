using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPManager
{
    public partial class PopupBroadcastMessage : Form
    {
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        public PopupBroadcastMessage()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public void InitText(string szText)
        {
            textBox1.Text = szText;
        }

        public string GetMessage()
        {
            return textBox1.Text;
        }

        private void PopupBroadcastMessage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void PopupBroadcastMessage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        private void PopupBroadcastMessage_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void label3_MouseDown(object sender, MouseEventArgs e)
        {
            PopupBroadcastMessage_MouseDown(sender, e);
        }

        private void label3_MouseMove(object sender, MouseEventArgs e)
        {
            PopupBroadcastMessage_MouseMove(sender, e);
        }

        private void label3_MouseUp(object sender, MouseEventArgs e)
        {
            PopupBroadcastMessage_MouseUp(sender, e);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            PopupBroadcastMessage_MouseDown(sender, e);
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            PopupBroadcastMessage_MouseMove(sender, e);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            PopupBroadcastMessage_MouseDown(sender, e);
        }

        private void btnSpecialMessage_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ShowSpecialMessage();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            DateTime dtTime = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0);

            PopupPreviewMessage preview = new PopupPreviewMessage(textBox1.Text, dtTime, "[재난발생위치]");
            preview.ShowDialog();
        }
    }
}
