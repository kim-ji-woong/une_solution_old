using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RunQuery
{
    public partial class Form1 : Form
    {
        private DBUtility2.WebDBManager m_dbMgr = new DBUtility2.WebDBManager(200);

        public Form1()
        {
            InitializeComponent();
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            if (textBoxPath.Text.Length == 0)
            {
                MessageBox.Show("스크립트 파일 경로를 입력해 주세요");
                return;
            }
            else if (!System.IO.File.Exists(textBoxPath.Text))
            {
                MessageBox.Show("스크립트 파일 경로가 유효하지 않거나 해당 파일을 사용할 수 없습니다.");
                return;
            }

            if (ReadScript(textBoxPath.Text))
                MessageBox.Show("DB 업데이트에 성공하였습니다.");
            else
                MessageBox.Show("DB 업데이트에 실패하였습니다.");
        }

        private bool ReadScript(string strPath)
        {
            StreamReader reader = new StreamReader(strPath, Encoding.UTF8);

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine();

                if (strLine.Length == 0)
                    continue;

                if (m_dbMgr.GetResultData(strLine, 0) == null)
                {
                    reader.Close();
                    return false;
                }
            }

            reader.Close();
            return true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxPath.Text = Application.StartupPath + "\\script.sql";
        }
    }
}
