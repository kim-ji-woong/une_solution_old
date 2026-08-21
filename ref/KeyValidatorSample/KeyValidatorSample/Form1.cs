using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeyValidatorSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            labelResult.Text = "";
        }

        private void btnCheckValidation_Click(object sender, EventArgs e)
        {
            if (textBoxCertCode.Text.Length == 0)
            {
                MessageBox.Show("인증코드를 입력하세요");
                return;
            }

            labelResult.Text = "";
            radioAdmin.Checked = false;
            radioNormal.Checked = false;
            textBoxIDCode.Text = "";

            string strIDCode;
            bool isAdmin;
            int nResult = UnE.KeyValidator.Manager.CheckKey("192.168.0.195", "sejoong", "sejoong", "HSMS", "LoginUser", "code", textBoxCertCode.Text, out strIDCode, out isAdmin);

            if (nResult == 1)
                labelResult.Text = "잘못된 인증코드 입니다.";
            else if (nResult == 2)
                labelResult.Text = "이미 사용중인 인증코드 입니다.";
            else if (nResult == 0)
            {
                labelResult.Text = "인증이 되었습니다.";

                if (isAdmin)
                    radioAdmin.Checked = true;
                else
                    radioNormal.Checked = true;

                textBoxIDCode.Text = strIDCode;
            }
        }
    }
}
