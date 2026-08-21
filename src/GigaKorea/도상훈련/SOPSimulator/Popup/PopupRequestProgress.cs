using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public partial class PopupRequestProgress : Form
    {
        public PopupRequestProgress()
        {
            InitializeComponent();

            this.AllowTransparency = true;
            this.Opacity = 1.0;
            this.TransparencyKey = this.BackColor;

            InitButton();

            timer1.Enabled = true;
        }

        private void InitButton()
        {
            //button1.ImageNormal = global::SOPMonitoringSystem.Properties.Resources.PopupRequestProgress_button;
            //button1.ImageMouseOver = global::SOPMonitoringSystem.Properties.Resources.PopupRequestProgress_button_mouseover;
        }

        #region 폼 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove;

        private void PopupRequestProgress_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void PopupRequestProgress_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void PopupRequestProgress_MouseMove(object sender, MouseEventArgs e)
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
        #endregion

        public void SetMessage(string szMessage)
        {
            label1.Text = szMessage;
            timer1.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.ForceControl();
            });*/

            /*FormSOP.Instance.NetworkManager.ClientProvier.SendData((short)SDMS.TCP_ID.STEAL_CONTROL);

            timer1.Stop();

            this.Close();*/
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            /*FormSOP.Instance.NetworkManager.ClientProvier.SendData((short)SDMS.TCP_ID.CANCEL_REQUEST_CONTROL);
            timer1.Stop();
            this.Close();*/
        }

        private int nProgress = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = nProgress++;
            if (nProgress == 100)
            {
                // 제한시간이 지났으니 요청취소 시킨다.
                btnCancel_Click(null, null);
                /*timer1.Stop();
                nProgress = 0;
                this.Close();*/
            }
                
        }

        private void PopupRequestProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
            
        }
    }
}
