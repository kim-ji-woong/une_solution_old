using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.SOP
{
    public partial class PopupSOPClose : Form
    {
        public PopupSOPClose()
        {
            InitializeComponent();
        }

        private int m_nCloseWaitTime = 30;

        private void PopupSOPClose_Load(object sender, EventArgs e)
        {
            m_nCloseWaitTime = 30;
            tmrClose.Interval = 1000;
            tmrClose.Enabled = true;
            tmrClose.Start();
        }

        private void PopupSOPClose_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        public void SetSOPName(string szSOPName)
        {
            lbSOPName.Text = "대상 SOP : " + szSOPName;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void tmrClose_Tick(object sender, EventArgs e)
        {
            m_nCloseWaitTime--;

            lbCloseTime.Text = "종료까지 " + m_nCloseWaitTime + "초";

            if (m_nCloseWaitTime == 0)
            {
                tmrClose.Enabled = false;
                tmrClose.Stop();

                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }
    }
}
