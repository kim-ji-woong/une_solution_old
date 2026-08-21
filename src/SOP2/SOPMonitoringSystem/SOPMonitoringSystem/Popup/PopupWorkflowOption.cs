using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPMonitoringSystem.Popup
{
    public partial class PopupWorkflowOption : Form
    {
        private bool m_useSmsMessage = true;
        public bool UseSmsMessage
        {
            get { return m_useSmsMessage; }
            set { m_useSmsMessage = value; }
        }

        private DateTime m_dtDetect = new DateTime();
        public DateTime DetectTime
        {
            get { return m_dtDetect; }
        }

        public PopupWorkflowOption()
        {
            InitializeComponent();

            radioAuto.Checked = true;
            labelManualTime.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioAuto.Checked)
                m_dtDetect = DateTime.Now;

            UseSmsMessage = checkBox2.Checked;
            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void radioAuto_CheckedChanged(object sender, EventArgs e)
        {
            EnableTimeOptionControls(false);
        }

        private void radioManual_CheckedChanged(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;

            if (labelManualTime.Text == "")
            {
                labelManualTime.Text = string.Format("{0}-{1}-{2} {3}:{4}:00",
                    dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute);

                m_dtDetect = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, 0);
            }

            EnableTimeOptionControls(true);
        }

        private void EnableTimeOptionControls(bool enabled)
        {
            labelManualTime.Visible = enabled;
            btnEditManualTime.Visible = enabled;
        }

        private void btnEditManualTime_Click(object sender, EventArgs e)
        {
            PopupDetectTime popup = new PopupDetectTime(m_dtDetect);
            popup.Owner = this;

            if (popup.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                m_dtDetect = popup.DetectTime;

                labelManualTime.Text = string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                    m_dtDetect.Year, m_dtDetect.Month, m_dtDetect.Day, m_dtDetect.Hour, m_dtDetect.Minute, m_dtDetect.Second);
            }
        }
    }
}
