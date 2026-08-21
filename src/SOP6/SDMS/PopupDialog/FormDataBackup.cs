using System;
using System.IO;
using System.Windows.Forms;

namespace SDMS
{
    public partial class FormDataBackup : PopupFormBase
	{
		public FormDataBackup()
		{
			InitializeComponent();

			InitList();

            InitCtrlSize(this);
            SetChildCtrlResize(this, 440, 277);

			btnDelete.Enabled = false;
			btnRestore.Enabled = false;
		}

		private void InitList()
		{
			try
			{
				lstFileList.Items.Clear();
				string szPath = BackupManager.Instance.BackupDir;
				System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(szPath);
				System.IO.FileInfo[] fi = di.GetFiles("SDMS_Backup_*.zip");
				for (int i = 0; i < fi.Length; i++)
				{
					lstFileList.Items.Add(fi[i]);
				}
			}
			catch (System.Exception)
			{
			}
			lstFileList.SelectedIndex = -1;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

        private void btnDelete_Click(object sender, EventArgs e)
		{
			int nSelIdx = lstFileList.SelectedIndex;
			if (nSelIdx != -1)
			{
				FileInfo info = (FileInfo)lstFileList.SelectedItem;
				if (info != null)
				{
					if (MessageBox.Show("백업 파일을 영구히 삭제 합니다.\n계속 하시겠습니까?", "삭제 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
                        try
                        {
                            info.Delete();
                        }
                        catch(Exception)
                        { }
						
						InitList();
					}
				}
			}
		}

		private void btnRestore_Click(object sender, EventArgs e)
		{
			int nSelIdx = lstFileList.SelectedIndex;
			if (nSelIdx != -1)
			{
				FileInfo info = (FileInfo)lstFileList.SelectedItem;
				if (info != null)
				{
					if (MessageBox.Show("백업 파일을 이용하여 데이터를 복원합니다.\n현재 데이터가 백업 되어 있지 않으면 소실됩니다.\n계속 하시겠습니까?", "복원 확인", MessageBoxButtons.YesNo) == DialogResult.Yes)
					{
						m_szRestoreFileName = info.FullName;
						this.Enabled = false;
						FormRestoreState.iForm.ThreadModal(this);
						FormRestoreState.iForm.ShowDialog(this);
					}
				}
			}
		}

		private string m_szRestoreFileName = "";

		public string RestoreFileName
		{
			get { return m_szRestoreFileName; }
			set { m_szRestoreFileName = value; }
		}

		private void lstFileList_SelectedIndexChanged(object sender, EventArgs e)
		{
			int nSelIdx = lstFileList.SelectedIndex;
			if (nSelIdx != -1)
			{
				btnDelete.Enabled = true;
				btnRestore.Enabled = true;
			}
			else
			{
				btnDelete.Enabled = false;
				btnRestore.Enabled = false;
			}
		}

		private void btnBackup_Click(object sender, EventArgs e)
		{
			this.Enabled = false;
			FormBackupState.iForm.ThreadModal(this);
            FormBackupState.iForm.SetChildCtrlResize(FormBackupState.iForm, 482, 54);
			FormBackupState.iForm.ShowDialog(this);
		}

		public void BackupComplete()
		{
			this.Enabled = true;
			FormBackupState.iForm.Close();
			InitList();
		}

		public void RestoreComplete()
		{
			this.Enabled = true;
			
            if(!SDMS.NetworkWebManager.Instance.SendRequestRestore())
            {
                MessageBox.Show(FormRestoreState.iForm,  "서버가 연결되지 않았습니다. 복원 작업이 중단되었습니다." ,"시스템복원");
            }

            FormRestoreState.iForm.Close();
		}
	}
}