using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;

namespace WindowsFormsApplication14
{
    public partial class MainForm : Form
    {
        private bool m_bSavedFile = false;
        private string m_SaveDir = "";
        private bool m_bSetSaveDir = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSearchFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "SQL Files (*.sql)|*.sql";
            dlg.DefaultExt = "sql";
            if( dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string szFileName = dlg.FileName;
                txtFileName.Text = szFileName;

                if (m_bSetSaveDir == false)
                {
                    string szDir = Path.GetDirectoryName(dlg.FileName);
                }
            }
        }
        
        private bool CheckState()
        {
            if (txtFileName.Text == "")
            {
                MessageBox.Show(this, "대상 파일이 지정되지 않았습니다.", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (txtSQLServer.Text == "")
            {
                MessageBox.Show(this, "대상 서버가 지정되지 않았습니다.", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (txtSQLDB.Text == "")
            {
                MessageBox.Show(this, "대상 DB가 입력되지 않았습니다.", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (txtSQLID.Text == "")
            {
                MessageBox.Show(this, "연결에 사용될 ID가 입력되지 않았습니다.", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (txtSQLPass.Text == "")
            {
                MessageBox.Show(this, "연결에 사용될 ID의 비밀번호가 입력되지 않았습니다.", "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (CheckState() == false)
                return;

            using(BatchProcess process = new BatchProcess(false))
            {
                process.ServerIP = txtSQLServer.Text;
                process.TargetDB = txtSQLDB.Text;
                process.UserID = txtSQLID.Text;
                process.Password = txtSQLPass.Text;
                process.SavedFile = m_bSavedFile;
                process.SavedPath = m_SaveDir;

                process.UseMySQL = ckbMySQL.Checked;

                Cursor current = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                if( process.Run(txtFileName.Text))
                {
                    Cursor.Current = current;
                    MessageBox.Show("배치 작업 성공");
                }
                else
                {
                    Cursor.Current = current;
                    MessageBox.Show("배치 작업 실패\n모든 데이터는 Rollback 됩니다.");
                }
            } 
        }

        private void btnConTest_Click(object sender, EventArgs e)
        {
            string szServerIP = txtSQLServer.Text;
            string szSQLDB = txtSQLDB.Text;
            string szUserID = txtSQLID.Text;
            string szPass = txtSQLPass.Text;

            bool bMysql = ckbMySQL.Checked;

            Cursor current = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            if (BatchProcess.ConnectionTest(szServerIP, szSQLDB, szUserID, szPass, bMysql))
            {
                Cursor.Current = current;
                MessageBox.Show(this,"연결 성공", "연결 테스트", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Cursor.Current = current;
                MessageBox.Show(this, "연결 실패", "연결 테스트", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
       
        private void ckbSaveFile_CheckedChanged(object sender, EventArgs e)
        {
            if( ckbSaveFile.Checked == true)
            {
                m_bSavedFile = true;
            }
            else
            {
                m_bSavedFile = false;
            }
        }

        private void btnSaveDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if( fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    m_SaveDir = fbd.SelectedPath;
                    m_bSetSaveDir = true;
                }               
            }            
        }
    }
}
