using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnE.SenarioMaker
{
    internal partial class FormMemberRegister : Form
    {
        private FormLoginMain m_formParent = null;

        private ArrayList m_arrMacAddrList = new ArrayList();
        private bool m_isConfirmCode = false;
        //private int m_nUserLevel = -1;
        private CertOption m_option = CertOption.NEW_CREATE;

        public FormMemberRegister(FormLoginMain form)
        {
            this.TopLevel = false;
            InitializeComponent();

            m_formParent = form;

            labelisAdmin.Visible = false;
            cboAsk.SelectedIndex = 0;
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dig = new OpenFileDialog();

            dig.Filter = "인증 Files|*.dat";
            dig.FilterIndex = 0;
            dig.Title = "인증파일 불러오기";

            if (dig.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
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

            else if (!File.Exists(textBoxConfirmCode.Text))
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

        //암호화 한 아이디를 일정한 규칙으로 섞음
        private string MixString(string strText)
        {
            Random r = new Random();

            char[] arRandom = {'a','b','c','d','e','f','g','f','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'
                     ,'A','B','C','D','E','F','G','F','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
                     ,'1','2','3','4','5','6','7','8','9','!','@','#','$','%','&'};

            int nTextLength = strText.Length;
            char[] c = strText.ToCharArray();

            char[] arrChar = new char[nTextLength * 2];
            int[] arrIndex = new int[nTextLength];

            int nCount = 0;
            for (int i = 0; i < nTextLength * 2; i += 2)
            {
                arrChar[i] = c[nCount];
                arrChar[i + 1] = arRandom[r.Next(arRandom.Length)];
                arrIndex[nCount] = i;
                nCount++;
            }

            string strMixString = "";
            for (int i = 0; i < arrChar.Length; i++)
            {
                strMixString += arrChar[i];
            }

            return strMixString;
        }

        //입력받은 문자열 2개를 정해진 비율로 섞음
        private string MixString(string strText1, string strText2, int nCount1, int nCount2, out string strSaveData)
        {
            strSaveData = "";
            StringBuilder str1 = new StringBuilder();


            //원래 문자열의 길이를 저장함
            int nLength1 = strText1.Length;
            int nLength2 = strText2.Length;

            Random r = new Random();

            char[] arRandom = {'a','b','c','d','e','f','g','f','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z'
                     ,'A','B','C','D','E','F','G','F','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
                     ,'1','2','3','4','5','6','7','8','9','!','@','#','$','%','&'};

            //string strRandomString = arRandom[r.Next(arRandom.Length)].ToString();
            //string strRandomString = "X";

            int nPadding = 0;


            //1, 글자 수가 배수에 맞는지 검사
            if (strText1.Length % nCount1 != 0)
            {
                nPadding = nCount1 - (strText1.Length % nCount1);
                for (int i = 0; i < nPadding; i++)
                {
                    strText1 += arRandom[r.Next(arRandom.Length)].ToString(); ;
                }
            }

            if (strText2.Length % nCount2 != 0)
            {
                nPadding = nCount2 - (strText2.Length % nCount2);
                for (int i = 0; i < nPadding; i++)
                {
                    strText2 += arRandom[r.Next(arRandom.Length)].ToString(); ;
                }
            }

            int nQuota = strText1.Length / nCount1;
            int nQuota2 = strText2.Length / nCount2;

            int nShareCount = 0;
            if (nQuota != nQuota2)
            {
                if (nQuota > nQuota2)
                {
                    int nLength = strText2.Length * nQuota;
                    int nAddLength = nLength - strText2.Length;

                    for (int i = 0; i < nAddLength; i++)
                    {
                        strText2 += arRandom[r.Next(arRandom.Length)].ToString(); ;
                    }
                    nShareCount = nQuota;
                }
                if (nQuota < nQuota2)
                {
                    int nLength = strText1.Length * nQuota2;
                    int nAddLength = nLength - strText1.Length;

                    for (int i = 0; i < nAddLength; i++)
                    {
                        strText1 += arRandom[r.Next(arRandom.Length)].ToString(); ;
                    }
                    nShareCount = nQuota2;
                }
            }
            else
            {
                nShareCount = nQuota;
            }

            char[] n1 = strText1.ToCharArray(0, strText1.Length);
            char[] n2 = strText2.ToCharArray(0, strText2.Length);

            int nTemp = 0;
            int nTemp2 = 0;
            //3. 문자열 섞기
            for (int i = 0; i < nShareCount; i++)
            {
                for (int k = nTemp; k < nTemp + nCount1; k++)
                {
                    str1.Append(n1[k].ToString());
                }

                for (int j = nTemp2; j < nTemp2 + nCount2; j++)
                {
                    str1.Append(n2[j].ToString());
                }

                nTemp += nCount1;
                nTemp2 += nCount2;
            }


            //원래 문자열의 길이, n:m 저장.
            strSaveData += "," + nLength1.ToString() + "," + nLength2.ToString() + "," + nCount1.ToString() + "," + nCount2.ToString();

            return str1.ToString();
        }

        [DllImport("kernel32")]
        public static extern int SetFileAttributes(string lpFileName, int dwFileAttributes);

        private void FileWrite(string strPath,string strFilePath, string strID, string strPassword)
        {
            string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
            string key = "";
            UnE.Utility.Properties.GetProperty(szKey, ref key);

            //아이디 암호화
            string strIDEncrypt = DBUtility.AES256Cipher.AES_encrypt(strID, key);

            //비밀번호 암호화
            string strPwdEncrypt = DBUtility.AES256Cipher.AES_encrypt(strPassword, key);

            string strMixID = MixString(strIDEncrypt);
            string strMixPwd = MixString(strPwdEncrypt);
            string strCode = "";

            //인증코드
            if (File.Exists(textBoxConfirmCode.Text))
            {
                StreamReader sr = new StreamReader(textBoxConfirmCode.Text);

                while (sr.Peek() >= 0)
                {
                    strCode = sr.ReadLine().Trim();
                }
                sr.Close();
            }

            //비율
            int n = 1;
            int m = 1;

            string strSaveData1 ="";
            string strSaveData2 = "";
            //아이디, 비밀번호를 섞음
            string strMixText = MixString(strMixID, strMixPwd, n, m, out strSaveData1);
            //아이디+비밀번호, 인증코드를 섞음(최종)
            string strMixText2 = MixString(strMixText, strCode, n, m, out strSaveData2);


            DirectoryInfo di = new DirectoryInfo(strPath);
            //해당 경로에 폴더가 있는지 확인
            if(di.Exists == false)
            {
                di.Create();
            }

            ArrayList arr = new ArrayList();
            if (File.Exists(strFilePath))
            {
                StreamReader sr = new StreamReader(strFilePath);
                
                while (sr.Peek() >= 0)
                {
                    string strLine = sr.ReadLine().Trim();
                    arr.Add(strLine);
                }
                sr.Close();
            }

            System.Threading.Thread.Sleep(500);

            StreamWriter sw = new StreamWriter(strFilePath);
            foreach (string strLine in arr)
            {
                sw.WriteLine(strLine);
            }

            sw.WriteLine(strMixText2 + strSaveData1 + strSaveData2);
            sw.Close();

            ////숨길파일 지정(FILE_ATTRIBUTE_HIDDEN)
            //int nTest1 = SetFileAttributes(strFilePath, 2);
            ////읽기전용 지정(FILE_ATTRIBUTE_READONLY)
            //int nText2 = SetFileAttributes(strFilePath, 1);
        }

        private StreamReader GetFileStreamReader(string strFilePath)
        {
            if (File.Exists(strFilePath))
            {
                StreamReader sr = new StreamReader(strFilePath);
                return sr;
            }

            return null;
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

        private bool FileRead()
        {
            string strCodePath = textBoxConfirmCode.Text;
            string strFilePath = FormLoginMain.Instance.FilePath;

            if (File.Exists(strFilePath))
            {
                StreamReader sr = new StreamReader(strFilePath);

                StreamReader sr2 = new StreamReader(strCodePath);
                sr2.Peek();
                string strLine2 = sr2.ReadLine().Trim();

                while (sr.Peek() >= 0)
                {
                    string[] str = sr.ReadLine().ToString().Split(new char[] { ',' });

                    string strMixString = str[0].ToString();
                    int nCount1 = Convert.ToInt32(str[3]);
                    int nCount2 = Convert.ToInt32(str[4]);
                    int nLength3 = Convert.ToInt32(str[5]);
                    int nLength4 = Convert.ToInt32(str[6]);

                    string strText1 = "";
                    string strText2 = "";
                    //섞인 문자, 분해한 문자1, 분해한 문자2(인증코드), n(n:m), m(n:m)
                    DivisionString(strMixString, out strText1, out strText2, nCount1, nCount2);

                    //strText1 = strText1.Substring(0, nLength3);
                    strText2 = strText2.Substring(0, nLength4);

                    if(strText2 == strLine2)
                    {
                        sr.Close();
                        sr2.Close();
                        return false;
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
            string strFilePath = FormLoginMain.Instance.FilePath;

            FileWrite(strPath,strFilePath, strID, strPassword);

            return true;
        }

        private bool CheckMemberID()
        {
            string strFilePath = FormLoginMain.Instance.FilePath;

            StreamReader sr = GetFileStreamReader(strFilePath);

            if (sr == null)
                return true;

            //중복 된 아이디가 있는지 찾는다.
            while (sr.Peek() >= 0)
            {
                string strLine = sr.ReadLine();
                if (strLine == textBoxMemberID.Text)
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
            if (CheckTextBox())
            {
                //아이디 중복 체크
                if (!CheckMemberID())
                {
                    MessageBox.Show("아이디가 중복됩니다.");
                    return;
                }

                //회원가입 완료 -> 로그인 창으로 돌아감.
                if (JoinUser())
                {
                    textBoxConfirmCode.Text = "";
                    textBoxMemberID.Text = "";
                    textBoxMemberPassword.Text = "";
                    textBoxConfirmPassword.Text = "";
                    cboAsk.SelectedIndex = 0;
                    textBoxAnswer.Text = "";

                    MessageBox.Show("사용자 등록이 완료되었습니다.");
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

            CertResult result = 0;

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

                //if (isAdmin)
                //{
                //    labelisAdmin.Text = "관리자 계정";
                //    m_nUserLevel = 1;
                //    labelisAdmin.SetBounds(257, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);
                //}
                //else
                //{
                //    labelisAdmin.Text = "일반 계정";
                //    m_nUserLevel = 0;
                //    labelisAdmin.SetBounds(257, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);
                //}

                labelisAdmin.Text = "사용가능한 인증코드 입니다.";
                labelisAdmin.SetBounds(277 - 70, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);

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

        private void FormMemberRegister_Shown(object sender, EventArgs e)
        {
            textBoxConfirmCode.Focus();
        }

        private void textBoxConfirmCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (sender == textBoxConfirmCode || sender == textBoxMemberID || sender == textBoxMemberPassword || sender == textBoxConfirmPassword || sender == textBoxAnswer)
                    btnOK_Click(null, null);
            }
        }
    }
}
