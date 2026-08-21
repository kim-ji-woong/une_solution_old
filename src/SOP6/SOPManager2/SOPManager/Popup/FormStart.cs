using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPManager
{
	public partial class FormStart : Form
	{
        private int nOpenType = 1;

		public FormStart()
		{
			InitializeComponent();
            UpdateControl();
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

		public int OpenType
		{
			get { return nOpenType; }
			set { nOpenType = value; }
		}

		private void rbNewSOP_CheckedChanged(object sender, EventArgs e)
		{
			if (rbNewSOP.Checked == true)
				nOpenType = 1;
		}

		private void rbOpenSOP_CheckedChanged(object sender, EventArgs e)
		{
			if (rbOpenSOP.Checked == true)
				nOpenType = 2;
		}

		private void rbOpenXML_CheckedChanged(object sender, EventArgs e)
		{
			if (rbOpenXML.Checked == true)
				nOpenType = 3;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCanel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void btnConSetting_Click(object sender, EventArgs e)
		{
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

        private void NewSOP_Click(object sender, EventArgs e)
        {
            rbNewSOP.Checked = true;
            rbOpenSOP.Checked = false;
            rbOpenXML.Checked = false;
            UpdateControl();
        }

        private void OpenSOP_Click(object sender, EventArgs e)
        {
            rbNewSOP.Checked = false;
            rbOpenSOP.Checked = true;
            rbOpenXML.Checked = false;
            UpdateControl();
        }

        private void OpenXML_Click(object sender, EventArgs e)
        {
            rbNewSOP.Checked = false;
            rbOpenSOP.Checked = false;
            rbOpenXML.Checked = true;
            UpdateControl();
        }

        private void UpdateControl()
        {
            if (rbNewSOP.Checked == true)
            {
                picNewSOP.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picNewSOP.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (rbOpenSOP.Checked == true)
            {
                picOpenSOP.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picOpenSOP.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }

            if (rbOpenXML.Checked == true)
            {
                picOpenXML.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Enable2;
            }
            else
            {
                picOpenXML.BackgroundImage = global::SOPManager.Properties.Resources.__SOPEDIT_Disable2;
            }
        }
	}
}
