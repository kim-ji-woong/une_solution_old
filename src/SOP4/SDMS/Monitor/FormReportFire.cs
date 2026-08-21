using System;
using System.Windows.Forms;

namespace SDMS
{
	public partial class FormReportFire : Form
	{
		

		public FormReportFire(FormMain frmParent)
		{
			InitializeComponent();

			UnE.Win32.NativeMethods.SetParent(this.Handle, frmParent.Handle);
		}

		private void button1_Click(object sender, EventArgs e)
		{
			MessageBox.Show("화재신고");
		}
	}
}