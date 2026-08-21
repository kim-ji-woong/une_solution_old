using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer.PopupForms
{
    public partial class PopupFormBase : Form
    {
        public Panel PnTitle
        {
            get { return panelTitle; }
        }
        
        public string strTitle
        {
            get { return lblTitle.Text; }
            set { lblTitle.Text = value; }
        }
        public PopupFormBase()
        {
            InitializeComponent();
        }

        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();

        private void panelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = e.Location;
            }

            m_isClicked = true;
        }

        private void panelTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            if (this.WindowState == FormWindowState.Maximized)
            {
                Point test = this.PointToScreen(e.Location);

                this.WindowState = FormWindowState.Normal;

                float xPer = ((float)m_ptOrigin.X / (float)1920) * 100;
                float yPer = ((float)m_ptOrigin.Y / (float)panelTitle.Height) * 100;

                float xPer2 = (float)panelTitle.Width * xPer / 100;
                float yPer2 = (float)panelTitle.Height * yPer / 100;

                this.Location = new Point(test.X - (int)xPer2, test.Y - (int)yPer2);
            }

            Point ptCur = this.Location;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void panelTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }
    }
}
