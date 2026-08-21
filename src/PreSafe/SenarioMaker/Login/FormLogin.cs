using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.SenarioMaker
{
    internal partial class FormLogin : Form
    {
        private FormLoginMain m_formParent = null;
        public FormLogin(FormLoginMain form)
        {
            InitializeComponent();
            this.TopLevel = false;

            m_formParent = form;

            btnSetup.Visible = false;
        }

        private void DivisionString(string strMixText, out string strText1, out string strText2, int n, int m)
        {
            strText1 = "";
            strText2 = "";

            string[] ar = new string[strMixText.Length / (n + m)];

            int nTemp = 0;
            for (int i = 0; i < ar.Length; i++)
            {
                string strTemp = strMixText.Substring(nTemp, n + m);

                strText1 += strTemp.Substring(0, n);
                strText2 += strTemp.Substring(n, m);

                nTemp = (i + 1) * (n + m);
            }
        }

        //아이디(암호화됨),비밀번호(암호화됨),인증코드의 원래 값을 가져옴
        private void GetString(string strMixString, int nLength1, int nLength2, int nCount1, int nCount2, int nLength3, int nLength4 ,out string strID, out string strPwd)
        {
            strID = "";
            strPwd = "";
            string strIDPwd = "";

            string strText1 = "";
            string strText2 = "";

            //마지막에 섞었던 문자부터 분리시킴
            //섞인 문자, 분해한 문자1, 분해한 문자2, n(n:m), m(n:m)
            DivisionString(strMixString, out strText1, out strText2, nCount1, nCount2);

            strText1 = strText1.Substring(0, nLength3);
            strText2 = strText2.Substring(0, nLength4);

            strIDPwd = strText1;

            DivisionString(strIDPwd, out strText1, out strText2, nCount1, nCount2);

            strText1 = strText1.Substring(0, nLength1);
            strText2 = strText2.Substring(0, nLength2);


            char[] c2 = strText1.ToCharArray();

            //원래 문자열을 찾는다.
            for (int i = 0; i < c2.Length; i += 2)
            {
                strID += c2[i].ToString();
            }

            char[] c3 = strText2.ToCharArray();

            //원래 문자열을 찾는다.
            for (int i = 0; i < c3.Length; i += 2)
            {
                strPwd += c3[i].ToString();
            }
        }

        //로그인(아이디,비번 비교)
        private bool FunctionLogin()
        {
            string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);

            //ID 암호화
            string strEncryptedID = DBUtility.AES256Cipher.AES_encrypt(textBoxID.Text, key);

            //입력된 비밀번호를 암호화한다.
            string strEncryptedPW = DBUtility.AES256Cipher.AES_encrypt(textBoxPassword.Text, key);

            //등록되어있는 ID, 비밀번호를 가져온다.
            //경로에 파일이 있는지 확인..
            string strFilePath = FormLoginMain.Instance.FilePath;
            FileInfo fi = new FileInfo(strFilePath);
            
            if (fi.Exists == false)
            {
                MessageBox.Show("아이디 또는 비밀번호가 틀립니다.");
                return false;
            }

            StreamReader sr = new StreamReader(strFilePath);

            string strMixString = "";
            int nLength1 = 0;
            int nLength2 = 0;
            int nCount1 = 0;
            int nCount2 = 0;
            int nLength3 = 0;
            int nLength4 = 0;

            while (sr.Peek() >= 0)
            {
                string[] str = sr.ReadLine().ToString().Split(new char[] { ',' });

                strMixString = str[0].ToString();
                nLength1 = Convert.ToInt32(str[1]);
                nLength2 = Convert.ToInt32(str[2]);
                nCount1 = Convert.ToInt32(str[3]);
                nCount2 = Convert.ToInt32(str[4]);
                nLength3 = Convert.ToInt32(str[5]);
                nLength4 = Convert.ToInt32(str[6]);

                string strID = "";
                string strPwd = "";

                GetString(strMixString, nLength1,nLength2,nCount1,nCount2,nLength3,nLength4, out strID, out strPwd);

                if(strEncryptedID == strID)
                {
                    if(strEncryptedPW == strPwd)
                    {
                        sr.Close();
                        return true;
                    }
                    else
                    {
                        sr.Close();
                        MessageBox.Show("아이디 또는 비밀번호가 틀립니다.");
                        return false;
                    }
                }
            }


            sr.Close();
            MessageBox.Show("아이디 또는 비밀번호가 틀립니다.");
            return false;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            //FormLoginMain.Instance.Dispose();

            if (textBoxPassword.Text == "" || textBoxID.Text == "")
                return;

            if(FunctionLogin())
            {
                FormLoginMain.Instance.DialogResult = DialogResult.OK;
            }

            FormLoginMain.Instance.DialogResult = DialogResult.OK;
        }

        private void btnRegMember_Click(object sender, EventArgs e)
        {
            m_formParent.ShowRegisterForm();
            textBoxID.Text = "";
            textBoxPassword.Text = "";
        }

        private void FormLogin_Shown(object sender, EventArgs e)
        {
            textBoxID.Focus();
        }

        private void textBoxID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (sender == textBoxID || sender == textBoxPassword)
                    button1_Click(null, null);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            m_formParent.ShowFindPasswordForm();
        }
    }
}
