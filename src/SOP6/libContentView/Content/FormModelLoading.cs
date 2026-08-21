using System;
using System.Windows.Forms;
using UnE.View.Content;

namespace SDMS
{
	public partial class FormModelLoading : Form
	{
		public static FormModelLoading iForm = new FormModelLoading();

		public FormModelLoading()
		{
			InitializeComponent();
		}

		private Form mParent = null;

		public void ThreadModal(Form parent)
		{
			mParent = parent;
			timer1.Interval = 1000;
			timer1.Start();
		}

		public static void RunThread(object parent)
		{
			((Form)parent).Invoke((MethodInvoker)delegate
			{
                FormContentUnity form = (FormContentUnity)parent;
				form.OpenModel();
			});
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			timer1.Stop();
            FormContentUnity form = (FormContentUnity)mParent;
			form.OpenModel();
		}
	}
}