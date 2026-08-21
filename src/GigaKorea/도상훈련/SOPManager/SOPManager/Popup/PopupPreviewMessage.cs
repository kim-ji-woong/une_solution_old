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
            UpdateRadioButtons();
            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            foreach (Control ctl in this.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);

                FormMain.Instance.UpdateWindowRate(ctl, WindowRateWidth, WindowRateHeight);
            }
        }

        private void UpdateRadioButtons()
        {
            if (radioIgnoreVirtual.Checked == true)
            {
                picIgnoreVirtual.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picIgnoreVirtual.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (radioVirtual.Checked == true)
            {
                picVirtual.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picVirtual.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (radioReal.Checked == true)
            {
                picReal.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picReal.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (radioIgnoreDay.Checked == true)
            {
                picIgnoreDay.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picIgnoreDay.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (radioDay.Checked == true)
            {
                picDay.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picDay.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (radioNight.Checked == true)
            {
                picNight.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picNight.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }
            Update();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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

        private void IgnoreVirtual_Click(object sender, EventArgs e)
        {
            radioIgnoreVirtual.Checked = true;
            radioVirtual.Checked = false;
            radioReal.Checked = false;
            UpdateRadioButtons();
            ParseText();
        }

        private void Virtual_Click(object sender, EventArgs e)
        {
            radioIgnoreVirtual.Checked = false;
            radioVirtual.Checked = true;
            radioReal.Checked = false;
            UpdateRadioButtons();
            ParseText();
        }

        private void Real_Click(object sender, EventArgs e)
        {
            radioIgnoreVirtual.Checked = false;
            radioVirtual.Checked = false;
            radioReal.Checked = true;
            UpdateRadioButtons();
            ParseText();
        }

        private void IgnoreDay_Click(object sender, EventArgs e)
        {
            radioIgnoreDay.Checked = true;
            radioDay.Checked = false;
            radioNight.Checked = false;
            UpdateRadioButtons();
            ParseText();
        }

        private void Day_Click(object sender, EventArgs e)
        {
            radioIgnoreDay.Checked = false;
            radioDay.Checked = true;
            radioNight.Checked = false;
            UpdateRadioButtons();
            ParseText();
        }

        private void Night_Click(object sender, EventArgs e)
        {
            radioIgnoreDay.Checked = false;
            radioDay.Checked = false;
            radioNight.Checked = true;
            UpdateRadioButtons();
            ParseText();
        } 
    }
}
