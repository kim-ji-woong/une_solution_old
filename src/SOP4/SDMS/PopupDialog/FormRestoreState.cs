using System.Threading;
using System.Windows.Forms;

namespace SDMS
{
	public partial class FormRestoreState : Form
	{
		public static FormRestoreState iForm = new FormRestoreState();

		public FormRestoreState()
		{
			InitializeComponent();
		}

		private FormDataBackup mParent = null;

		public void ThreadModal(FormDataBackup parent)
		{
			mParent = parent;

			Thread t = new Thread(RunThread);
            t.Name = "ResotreThread";
			t.Start(mParent);
		}

		public static void RunThread(object parent)
		{
			string szName = ((FormDataBackup)parent).RestoreFileName;
			BackupManager.Instance.RestoreData(szName);

			((FormDataBackup)parent).Invoke((MethodInvoker)delegate
			{
				((FormDataBackup)parent).RestoreComplete();
			});
		}
	}
}