using DBUtility2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DidViewerAutoUpdateCommander
{
    public partial class Form1 : Form
    {
        private WebDBManager m_dbMgr = null;

        public Form1()
        {
            InitializeComponent();

            m_dbMgr = new WebDBManager(3);
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 1; i <= 4; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO DidAutoUpdate (ID, did_pc_no) ");
                    sb.AppendFormat(" VALUES ((select ifnull(max(id) + 1, 1) from didautoupdate a), {0})", i);

                    m_dbMgr.GetResultData(sb.ToString());
                }

                MessageBox.Show("DID Viewer Restart");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string fileName = textBox1.Text;
            if (fileName.Length == 0)
            {
                MessageBox.Show("파일명을 입력하세요");
                return;
            }

            string[] files = fileName.Split(',');
            DialogResult result = MessageBox.Show(files.Length + "개의 파일을 업데이트 하시겠습니까? ", "", MessageBoxButtons.YesNo);
            if (result == System.Windows.Forms.DialogResult.No)
                return;

            try
            {
                for (int i = 1; i <= 4; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("INSERT INTO DidAutoUpdate (ID, did_pc_no, FileName) ");
                    sb.AppendFormat(" VALUES ((select ifnull(max(id)+1, 1) from didautoupdate a), {0}, '{1}')", i, fileName);

                    m_dbMgr.GetResultData(sb.ToString());
                }

                MessageBox.Show("10초내에 DID Viewer가 업데이트됩니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
