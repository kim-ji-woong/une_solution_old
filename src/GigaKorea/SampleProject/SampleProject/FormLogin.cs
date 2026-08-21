using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XMLWebServiceManager;

namespace SampleProject
{
    public partial class FormLogin : Form
    {
        WebServiceManager m_webManager = null;
        //FormMain m_formMain = null;

        public FormLogin(WebServiceManager webManager, string[] args)
        {
            InitializeComponent();
            //m_formMain = formMain;
            m_webManager = webManager;

            if (args.Length == 2)
            {   // 자동 로그인 및 다운로드 모드
                string strID = args[0];
                string strPW = args[1];
                string strResultMessage = "";

                if (m_webManager.Login(strID, strPW, ref strResultMessage) == false)
                {   // 로그인 실패
                    MessageBox.Show("Login Failed!");
                    this.Close();
                }
                else
                {
                    FormMain formMain = new FormMain(m_webManager, FormMain.Type.Download);
                    this.Hide();
                    formMain.ShowDialog();
                    this.Close();
                }
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void rbtnOK_Click(object sender, EventArgs e)
        {
            string strID = txtUserID.Text.Trim();
            string strPW = txtUserKey.Text.Trim();
            string strResultMessage = "";

            if (m_webManager.Login(strID, strPW, ref strResultMessage) == false)
            {
                MessageBox.Show("Login Failed!");
            }
            else
            {
                FormMain formMain = new FormMain(m_webManager, FormMain.Type.Normal);
                this.Hide();
                formMain.ShowDialog();
                this.Close();
            }
        }

        private void rbtnCancel_Click(object sender, EventArgs e)
        {
            //m_formMain.Close();
            //FormMain.Instance.
            this.Close();
        }
    }
}
