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
    public partial class PopupMessageBox : Form
    {
        #region Form 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        #endregion

        public string Caption
        {
            get { return labelCaption.Text; }
            set { labelCaption.Text = value; }
        }

        public string Message
        {
            get { return labelMessage.Text; }
            set { labelMessage.Text = value; }
        }

        public PopupMessageBox(string strMessage = "", string strCaption = "")
        {
            InitializeComponent();
            labelMessage.Text = strMessage;
            labelCaption.Text = strCaption;
        }

        private void PopupMessageBoxYesNo_Load(object sender, EventArgs e)
        {
            int nChangedWidth = -1, nChangedHeight = -1;
            int nLeftSpace = labelMessage.Location.X;
            int nRightSpace = this.Size.Width - (labelMessage.Location.X + labelMessage.Size.Width);

            if (nRightSpace < nLeftSpace)
                nChangedWidth = labelMessage.Location.X + labelMessage.Size.Width + nLeftSpace;

            int nTopSpace = labelMessage.Location.Y - panelTitle.Size.Height;
            int nBottomSpace = btnYes.Location.Y - (labelMessage.Location.Y + labelMessage.Size.Height);

            if (nBottomSpace < nTopSpace)
                nChangedHeight = labelMessage.Location.Y + labelMessage.Size.Height + nTopSpace + (this.Size.Height - btnYes.Location.Y);

            if (nChangedWidth >= 0 && nChangedHeight >= 0)
                this.Size = new Size(nChangedWidth, nChangedHeight);
            else if (nChangedWidth >= 0)
                this.Size = new Size(nChangedWidth, this.Size.Height);
            else if (nChangedHeight >= 0)
                this.Size = new Size(this.Size.Width, nChangedHeight);
            else
                return;

            panelTitle.Size = new Size(this.Size.Width, panelTitle.Height);

            int nButtonSpace = btnNo.Location.X - (btnYes.Location.X + btnYes.Size.Width);
            int nWidth = btnNo.Location.X + btnNo.Size.Width - btnYes.Location.X;

            int nBeginPos = (this.Size.Width - nWidth) / 2;

            btnYes.Location = new Point(nBeginPos, btnYes.Location.Y);
            btnNo.Location = new Point(btnYes.Location.X + btnYes.Size.Width + nButtonSpace, btnNo.Location.Y);
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            Close();
        }

        private void OnTitleBarMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
            }
        }

        private void OnTitleBarMouseMove(object sender, MouseEventArgs e)
        {
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

        private void OnTitleBarMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }
    }
}
