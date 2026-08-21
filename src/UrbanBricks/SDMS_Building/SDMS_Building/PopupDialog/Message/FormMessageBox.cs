using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS_Building.PopupDialog
{
    public partial class FormMessageBox : Form
    {
        private string m_strYesText = "확인";
        private string m_strNoText = "취소";

        public string YesText
        {
            get { return m_strYesText; }
            set { m_strYesText = value; }
        }
        public string NoText
        {
            get { return m_strNoText; }
            set { m_strNoText = value; }
        }

        private MessageBoxButtons m_msgButtons = MessageBoxButtons.YesNo;
        public FormMessageBox(string msg, MessageBoxButtons msgButton)
        {
            InitializeComponent();

            lblMsg.Text = msg;
            m_msgButtons = msgButton;
        }

        private void FormMessageBox_Load(object sender, EventArgs e)
        {
            Region = System.Drawing.Region.FromHrgn(FormMain.CreateRoundRectRgn(0, 0, this.Width, this.Height, 35, 35));
            SetButton();
        }

        private int m_nBtnLocationY = 177;
        private void SetButton()
        {
            btnYes.ButtonText = m_strYesText;
            btnCancel.ButtonText = m_strNoText;

            if (m_msgButtons == MessageBoxButtons.YesNo || m_msgButtons == MessageBoxButtons.YesNoCancel)
            {
                btnYes.Size = new Size(150, 45);
                btnYes.Location = new Point(26, m_nBtnLocationY);

                btnCancel.Size = new Size(150, 45);
                btnCancel.Location = new Point(196, m_nBtnLocationY);

                lblMsg.Size = new Size(322, 137);

                if (m_msgButtons == MessageBoxButtons.YesNoCancel)
                    btnClose.Visible = true;
            }
            else if (m_msgButtons == MessageBoxButtons.OK)
            {
                btnCancel.Visible = false;
                btnYes.Location = new Point((this.Width / 2) - (btnYes.Width / 2), m_nBtnLocationY);
            }
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();

        private void FormMessageBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        private void FormMessageBox_MouseMove(object sender, MouseEventArgs e)
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

        private void FormMessageBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        }
        #endregion

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
