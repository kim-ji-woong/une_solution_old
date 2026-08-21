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
		public FormStart()
		{
			InitializeComponent();
		}


		private int nOpenType = 1;

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


		
	}
}
