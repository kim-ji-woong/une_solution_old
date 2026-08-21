using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreSafe
{
    internal partial class FormMemberRegister : Form
    {
        private FormLoginMain m_formParent = null;

        private ArrayList m_arrMacAddrList = new ArrayList();
        private bool m_isConfirmCode = false;
        private int m_nUserLevel = -1;
        private CertOption m_option = CertOption.NEW_CREATE;

        public FormMemberRegister(FormLoginMain form)
        {
            this.TopLevel = false;
            InitializeComponent();

            m_formParent = form;

            labelisAdmin.Visible = false;
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();

            dig.Filter = "인증 Files|*.dat";
            dig.FilterIndex = 0;
            dig.Title = "인증파일 불러오기";

            if(dig.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxConfirmCode.Text = dig.FileName;
            }

        }

        //TextBox내용 검사/인증번호포함... return값이 -1이면 실패, 0이면 성공
        private bool CheckTextBox()
        {
            if (textBoxConfirmCode.Text.Length == 0)
            {
                MessageBox.Show("인증파일의 경로를 입력하세요", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxConfirmCode.Focus();
                return false;
            }
         
            else if(!File.Exists(textBoxConfirmCode.Text))
            {
                MessageBox.Show("해당 경로에 인증파일이 없습니다.");
                textBoxConfirmCode.Focus();
                return false;
            }

            else if (textBoxMemberID.Text.Length == 0)
            {
                MessageBox.Show("아이디를 입력하세요", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxMemberID.Focus();
                return false;
            }
            else if (textBoxMemberPassword.Text.Length == 0)
            {
                MessageBox.Show("비밀번호를 입력하세요", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxMemberPassword.Focus();
                return false;
            }
            else if (textBoxConfirmPassword.Text.Length == 0)
            {
                MessageBox.Show("비밀번호 확인을 입력하세요", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxConfirmPassword.Focus();
                return false;
            }
            else if (textBoxMemberPassword.Text != textBoxConfirmPassword.Text)
            {
                MessageBox.Show("비밀번호와 비밀번호확인이 동일하지 않습니다.", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxConfirmPassword.Focus();
                return false;
            }
            else if (m_isConfirmCode == false)
            {
                MessageBox.Show("인증파일을 다시 확인하세요.", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxConfirmCode.Focus();
                return false;
            }

            return true;
        }

        private void FileWrite(string strPath, string strID, string strPassword)
        {
            string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);
            string strEncrypt = DBUtility.AES256Cipher.AES_encrypt(strPassword, key);


            //아이디, 비밀번호 기록
            ArrayList arr = new ArrayList();

            if (File.Exists(strPath))
            {
                StreamReader sr = new StreamReader(strPath);

                while (sr.Peek() >= 0)
                {
                    string strLine = sr.ReadLine().Trim();
                    arr.Add(strLine);
                }
                sr.Close();
            }

            arr.Add(strID);
            arr.Add(strEncrypt);
            //

            //인증코드 기록
            if (File.Exists(textBoxConfirmCode.Text))
            {
                StreamReader sr = new StreamReader(textBoxConfirmCode.Text);

                while (sr.Peek() >= 0)
                {
                    string strLine = sr.ReadLine().Trim();
                    arr.Add("[Code]");
                    arr.Add(strLine);
                }
                sr.Close();
            }

            arr.Add("--End--");


            System.Threading.Thread.Sleep(100);

            StreamWriter sw = new StreamWriter(strPath);
            foreach (string strLine in arr)
            {
                sw.WriteLine(strLine);
            }
            sw.Close();
        }

        private StreamReader GetFileStreamReader(string strPath)
        {
            if(File.Exists(strPath))
            {
                StreamReader sr = new StreamReader(strPath);
                return sr;
            }

            return null;
        }

        
        private bool FileRead()
        {
            string strPath = textBoxConfirmCode.Text;

            if (File.Exists(@"C:\\PreSafeTemp\\a.txt"))
            {
                StreamReader sr = new StreamReader(@"C:\\PreSafeTemp\\a.txt");
                string strLine = "";


                StreamReader sr2 = new StreamReader(strPath);
                sr2.Peek();
                string strLine2 = sr2.ReadLine().Trim();

                while (sr.Peek() >= 0)
                {
                    strLine = sr.ReadLine().Trim();

                    if(strLine == "[Code]")
                    {
                        sr.Peek();
                        strLine = sr.ReadLine().Trim();

                        if(strLine == strLine2)
                        {
                            sr.Close();
                            sr2.Close();
                            return false;
                        }
                    }
                }
                sr.Close();
                sr2.Close();
            }

            return true;
        }
        

        private bool JoinUser()
        {
            string strID = textBoxMemberID.Text;
            string strPassword = textBoxMemberPassword.Text;

            string strPath = FormLoginMain.Instance.Path;


            FileWrite(strPath, strID, strPassword);


            return true;
        }

        private bool CheckMemberID()
        {
            StreamReader sr = GetFileStreamReader(@"C:\\PreSafeTemp\\a.txt");

            if (sr == null)
                return true;

            //중복 된 아이디가 있는지 찾는다.
            while(sr.Peek() >= 0)
            {
                string strLine = sr.ReadLine();
                if(strLine == textBoxMemberID.Text)
                {
                    sr.Close();
                    return false;
                }
            }
            sr.Close();
            return true;
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            string strPath = @"C:\PreSafeTemp";
            string destFile = System.IO.Path.Combine(strPath, "test");
            if(CheckTextBox())
            {
                //아이디 중복 체크
                if(!CheckMemberID())
                {
                    MessageBox.Show("아이디가 중복됩니다.");
                    return;
                }

                //회원가입 완료 -> 로그인 창으로 돌아감.
                if(JoinUser())
                {
                    textBoxConfirmCode.Text = "";
                    textBoxMemberID.Text = "";
                    textBoxMemberPassword.Text = "";
                    textBoxConfirmPassword.Text = "";

                    m_formParent.ShowLoginForm();
                }
            }
        }

        private void ConfirmCode(string strCertCode)
        {
            labelisAdmin.Visible = true;

            //실제 등록 될 인증코드
            string strIDCode;
            //관리자인지 일반인지 구별
            bool isAdmin;

            //m_formParent.ReadProperties();

            string szServerID = "";
            string szDBName = "";
            string szDBUser = "";
            string szDBPass = "";
            UnE.Utility.Properties.GetProperty("Data Source", ref szServerID);
            UnE.Utility.Properties.GetProperty("Initial Catalog", ref szDBName);
            UnE.Utility.Properties.GetProperty("User ID", ref szDBUser);
            UnE.Utility.Properties.GetProperty("Password", ref szDBPass);

            PreSafe.CertResult result = 0;

            //이미 사용중인 코드인가.
            bool bUsedCode = FileRead();

            result = KeyManager.VaildKey(szServerID, szDBUser, szDBPass, szDBName, "LoginUser", "code", strCertCode, m_arrMacAddrList, out strIDCode, out isAdmin, out m_option, bUsedCode);


            //UnE.KeyValidator.CertResult result = UnE.KeyValidator.Manager.VaildKey(szServerID, szDBUser, szDBPass, szDBName, "LoginUser", "code", strCertCode, m_arrMacAddrList, out strIDCode, out isAdmin, out m_option);

            if (result == CertResult.INVALID_CODE)
            {
                labelisAdmin.Text = "잘못된 인증코드 입니다.";
                labelisAdmin.SetBounds(257 - 40, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);
            }
            else if (result == CertResult.ALREADY_USED_CODE)
            {
                labelisAdmin.Text = "이미 사용중인 인증코드 입니다.";
                labelisAdmin.SetBounds(257 - 70, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);
            }

            if (result == CertResult.SUCCESS)
            {
                //labelConfirmCode.Text = "인증이 되었습니다.";

                if (isAdmin)
                {
                    labelisAdmin.Text = "관리자 계정";
                    m_nUserLevel = 1;
                    labelisAdmin.SetBounds(257, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);
                }
                else
                {
                    labelisAdmin.Text = "일반 계정";
                    m_nUserLevel = 0;
                    labelisAdmin.SetBounds(257, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);
                }

                //m_strCodeKey = strIDCode;
                m_isConfirmCode = true;
            }
            else
                m_isConfirmCode = false;

            if (strCertCode.Length == 0)
                labelisAdmin.Visible = false;


        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            textBoxConfirmCode.Text = "";
            textBoxMemberID.Text = "";
            textBoxMemberPassword.Text = "";
            textBoxConfirmPassword.Text = "";

            m_formParent.ShowLoginForm();
        }

        private void textBoxConfirmCode_TextChanged(object sender, EventArgs e)
        {
            if (!File.Exists(textBoxConfirmCode.Text))
            {
                
                m_isConfirmCode = false;
                labelisAdmin.Visible = false;
                return;
            }

            string strCertCode;

            m_arrMacAddrList.Clear();

            if (!GetCertData(textBoxConfirmCode.Text, out strCertCode, m_arrMacAddrList))
                return;

            ConfirmCode(strCertCode);
        }


        private bool GetCertData(string strCertFilePath, out string strCertCode, ArrayList arrMacAddrList)
        {
            strCertCode = "";

            try
            {
                StreamReader reader = new StreamReader(strCertFilePath);
                string strData = reader.ReadToEnd();
                reader.Close();


                string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
                string key = "";
                UnE.Utility.Properties.GetProperty(szKey, ref key);

                //string strDecrypt = DBUtility.AES256Cipher.AES_decrypt(strData, DBConn.Key);
                string strDecrypt = DBUtility.AES256Cipher.AES_decrypt(strData, key);

                char[] separator = new char[] { '\r', '\n' };
                string[] arrTokens = strDecrypt.Split(separator);

                int nTokenCount = arrTokens.Count();

                if (nTokenCount < 1)
                {
                    MessageBox.Show("잘못된 인증파일입니다.");
                    return false;
                }

                strCertCode = arrTokens[0];

                for (int i = 1; i < nTokenCount; i++)
                {
                    if (arrTokens[i].Length > 0)
                        arrMacAddrList.Add(arrTokens[i]);
                }

                arrMacAddrList.Sort();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            return true;
        }
    }
}
