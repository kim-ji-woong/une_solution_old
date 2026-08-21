using System;
using System.Windows.Forms;

namespace SDMS
{
	public partial class FormEditPassword : Form
	{
		private string m_strPassword = null;

		public FormEditPassword()
		{
			InitializeComponent();

			m_strPassword = FormMain.Instance.DataManager.GetEditPassword();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (m_strPassword == null || m_strPassword == textBoxPassword.Text)
			{
				if (FormMain.Instance.CCTVList != null)
					FormMain.Instance.CCTVList.Show();

				DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
				return;
			}
			else
			{
				MessageBox.Show("암호가 일치하지 않습니다.");
				return;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			if (FormMain.Instance.CCTVList != null)
				FormMain.Instance.CCTVList.Show();

			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			return;
		}

		private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				btnOK_Click(null, null);
			}
		}
	}
}