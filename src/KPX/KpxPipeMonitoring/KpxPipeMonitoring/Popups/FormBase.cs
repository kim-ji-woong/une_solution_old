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
    public partial class FormBase : Form
    {
        #region Form 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();
        #endregion

        public FormBase()
        {
            InitializeComponent();
        }

        protected void pictureBoxTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        protected void pictureBoxTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        protected void pictureBoxTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }

        Image optionCloseMouseover = global::KpxPipeMonitoring.Properties.Resources.OptionClose_mouseover;
        Image optionCloseNormal = global::KpxPipeMonitoring.Properties.Resources.OptionClose_normal;
        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            this.btnClose.BackgroundImage = optionCloseMouseover;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            this.btnClose.BackgroundImage = optionCloseNormal;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            OnClose();
        }

        virtual protected void OnClose()
        {
            this.Close();
        }
    }
}
