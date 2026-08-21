using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SDMS
{
    public partial class FormBackupState : Form
    {
		public static FormBackupState iForm = new FormBackupState();
		public FormBackupState()
        {
            InitializeComponent();          
        }

        private FormDataBackup mParent = null;

		public void ThreadModal(FormDataBackup parent)
        {
            mParent = parent;

			Thread t = new Thread(RunThread);			
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
