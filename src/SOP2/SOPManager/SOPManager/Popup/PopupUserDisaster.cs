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
    public partial class PopupUserDisaster : Form
    {
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        private string m_strDisaster;
        public string DisasterCaption
        {
            get { return m_strDisaster; }
            set { m_strDisaster = value; }
        }

        public PopupUserDisaster()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DisasterCaption = textDisaster.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        public void ChangeTitle(int nMode)
        {
            switch (nMode)
            {
                case 1:
                    this.Text = "재난 이름 설정";
                    lblDisasterName.Text = "재난 이름 설정";
                    break;
                case 2:
                    this.Text = "조직 이름 설정";
                    lblDisasterName.Text = "조직 이름 설정";
                    break;
                case 3:
                    this.Text = "외부기관 이름 설정";
                    lblDisasterName.Text = "외부기관 이름 설정";
                    break;
            }
        }

        private void textDisaster_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnOK_Click(sender, e);
            }
        }

        private void PopupUserDisaster_Load(object sender, EventArgs e)
        {
            textDisaster.Text = "";
        }

        private void PopupUserDisaster_MouseDown(object sender, MouseEventArgs e)
        {
            m_bLeftMouseDown = true;
            m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
        }

        private void PopupUserDisaster_MouseMove(object sender, MouseEventArgs e)
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

        private void PopupUserDisaster_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
