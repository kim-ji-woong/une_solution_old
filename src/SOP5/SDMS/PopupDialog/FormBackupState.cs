using System.Threading;
using System.Windows.Forms;

namespace SDMS
{
	public partial class FormBackupState : PopupFormBase
	{
		public static FormBackupState iForm = new FormBackupState();

		public FormBackupState()
		{
			InitializeComponent();

            InitCtrlSize(this);
		}

		private FormDataBackup mParent = null;

		public void ThreadModal(FormDataBackup parent)
		{
			mParent = parent;

			Thread t = new Thread(RunThread);
            t.Name = "BackupThread";
			t.Start(mParent);
		}

		public static void RunThread(object parent)
		{
			BackupManager.Instance.BackupData();

			((FormDataBackup)parent).Invoke((MethodInvoker)delegate
			{
				((FormDataBackup)parent).BackupComplete();
			});
		}
	}
}