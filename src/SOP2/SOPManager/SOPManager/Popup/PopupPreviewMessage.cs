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
    public partial class PopupPreviewMessage : Form
    {
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();

        private string m_strTextOrigin = null;
        private DateTime m_dtTime = new DateTime();
        private string m_strLocation = null;

        public PopupPreviewMessage(string strText, DateTime dtTime, string strLocation)
        {
            InitializeComponent();

            m_strTextOrigin = strText;
            m_dtTime = dtTime;
            m_strLocation = strLocation;

            ParseText();
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
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

        private void TitleBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioMode_CheckedChanged(object sender, EventArgs e)
        {
            ParseText();
        }

        private void ParseText()
        {
            int nRealMode = -1, nNormalMode = -1;

            if (radioDay.Checked)
                nNormalMode = 1;
            else if (radioNight.Checked)
                nNormalMode = 0;

            if (radioReal.Checked)
                nRealMode = 1;
            else if (radioVirtual.Checked)
                nRealMode = 0;

            string strResult = UnE.Utility.SOPSimulatorScript.Parse(m_strTextOrigin, m_dtTime, m_strLocation, nRealMode, nNormalMode);
            textBoxPreview.Text = strResult;
        }
    }
}
