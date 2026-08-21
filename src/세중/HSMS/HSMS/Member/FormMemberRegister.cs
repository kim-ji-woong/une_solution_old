using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Collections;

namespace HSMS
{
    public partial class FormMemberRegister : Form, UnE.GUI.IRibbonButtonOwner
    {
        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        private string m_strCodeKey = "";
        private bool m_isConfirmCode = false;
        private ArrayList m_arrMacAddrList = new ArrayList();
        private UnE.KeyValidator.CertOption m_option = UnE.KeyValidator.CertOption.NEW_CREATE;

        private FormLoginMain m_formParent = null;
        public FormMemberRegister(FormLoginMain form)
        {
            InitializeComponent();
            this.TopLevel = false;

            m_formParent = form;
            MouseDown += new MouseEventHandler(m_formParent.FormLoginMain_MouseDown);
            MouseMove += new MouseEventHandler(m_formParent.FormLoginMain_MouseMove);
            MouseUp += new MouseEventHandler(m_formParent.FormLoginMain_MouseUp);    

            labelisAdmin.Visible = false;
            initButton();
        }




        private void initButton()
        {
            this.btnConfirm.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
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

        //회원가입 쿼리
        private void JoinMember(/*string strCode, */string strName, string strpwd)
        {
            if(!LoginManager.Instance.JoinUser(strName, strpwd, m_nUserLevel, m_arrMacAddrList, m_option))
            {
                MessageBox.Show("서버에 연결할 수 없습니다.\r\n네트웍 접속 상태를 확인해 주세요", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        private void FormMemberRegister_Load(object sender, EventArgs e)
        {
            btnConfirm.Owner = this;
            btnCancel.Owner = this;
        }

        //인터페이스 메서드 구현
        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            //FormLoginMain.Instance.Close();
        }
        
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            bool nCheckData = CheckTextBox();
            
            //nCheckData가 0이면 성공/ -1이면 실패
            if (nCheckData == true)
            {
                m_formParent.UserID = textBoxMemberID.Text;
                JoinMember(/*textBoxConfirmCode.Text, */textBoxMemberID.Text, textBoxMemberPassword.Text);                    
            }
        }

        public void ClearTextBoxAll()
        {
            textBoxConfirmCode.Text = "";
            textBoxMemberID.Text = "";
            textBoxMemberPassword.Text = "";
            textBoxConfirmPassword.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearTextBoxAll();
            m_formParent.ShowLoginForm();
        }

        private int m_nUserLevel = -1;

        private void ConfirmCode(string strCertCode)
        {
            labelisAdmin.Visible = true;

            //실제 등록 될 인증코드
            string strIDCode;
            //관리자인지 일반인지 구별
            bool isAdmin;

            m_formParent.ReadProperties();

            string szServerID = "";
            string szDBName = "";
            string szDBUser = "";
            string szDBPass = "";
            UnE.Utility.Properties.GetProperty("Data Source", ref szServerID);
            UnE.Utility.Properties.GetProperty("Initial Catalog", ref szDBName);
            UnE.Utility.Properties.GetProperty("User ID", ref szDBUser);
            UnE.Utility.Properties.GetProperty("Password", ref szDBPass);

            UnE.KeyValidator.CertResult result = UnE.KeyValidator.Manager.VaildKey(szServerID, szDBUser, szDBPass, szDBName, "LoginUser", "code", strCertCode, m_arrMacAddrList, out strIDCode, out isAdmin, out m_option);

            if (result == UnE.KeyValidator.CertResult.INVALID_CODE)
            {
                labelisAdmin.Text = "잘못된 인증코드 입니다.";
                labelisAdmin.SetBounds(257 - 40, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);
            }
            else if (result == UnE.KeyValidator.CertResult.ALREADY_USED_CODE)
            {
                labelisAdmin.Text = "이미 사용중인 인증코드 입니다.";
                labelisAdmin.SetBounds(257 - 70, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);
            }

            if (result == UnE.KeyValidator.CertResult.SUCCESS)
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

                m_strCodeKey = strIDCode;
                m_isConfirmCode = true;
            }
            else
                m_isConfirmCode = false;

            if (strCertCode.Length == 0)
                labelisAdmin.Visible = false;
        }

        public static bool GetCertData(string strCertFilePath, out string strCertCode, ArrayList arrMacAddrList)
        {
            strCertCode = "";

            try
            {
                StreamReader reader = new StreamReader(strCertFilePath);
                string strData = reader.ReadToEnd();
                reader.Close();

                string strDecrypt = DBUtility.AES256Cipher.AES_decrypt(strData, DBConn.Key);

                char[] separator = new char[] { '\r', '\n' };
                string[] arrTokens = strDecrypt.Split(separator);

                int nTokenCount = arrTokens.Count();

                if (nTokenCount < 1)
                {
                    MessageBox.Show("잘못된 인증파일입니다.");
                    return false;
                }

                strCertCode = arrTokens[0];

                for (int i=1;i<nTokenCount;i++)
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

        //인증코드 확인
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

            /*labelisAdmin.Visible = true;
            
            //실제 등록 될 인증코드
            string strIDCode;
            //관리자인지 일반인지 구별
            bool isAdmin;
            
            m_formParent.ReadProperties();

            string szServerID = "";
            string szDBName = "";
            string szDBUser = "";
            string szDBPass = "";
            UnE.Utility.Properties.GetProperty("Data Source", ref szServerID);
            UnE.Utility.Properties.GetProperty("Initial Catalog", ref szDBName);
            UnE.Utility.Properties.GetProperty("User ID", ref szDBUser);
            UnE.Utility.Properties.GetProperty("Password", ref szDBPass);

            int nResult = UnE.KeyValidator.Manager.VaildKey(szServerID, szDBUser, szDBPass, szDBName, "LoginUser", "code", textBoxConfirmCode.Text, out strIDCode, out isAdmin);
            if (nResult == 1)
            {
                labelisAdmin.Text = "잘못된 인증코드 입니다.";
                labelisAdmin.SetBounds(257 - 40, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);

                m_isConfirmCode = false;
            }
            else if (nResult == 2)
            {
                labelisAdmin.Text = "이미 사용중인 인증코드 입니다.";
                labelisAdmin.SetBounds(257 - 70, labelisAdmin.Location.Y, labelisAdmin.Size.Width, labelisAdmin.Size.Height);

                m_isConfirmCode = false;
            }
            if (nResult == 0)
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

                m_strCodeKey = strIDCode;
                m_isConfirmCode = true;
            }

            if (textBoxConfirmCode.Text == "")
                labelisAdmin.Visible = false;*/
        }

        public void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (sender == textBoxMemberID || sender == textBoxConfirmCode || sender == textBoxConfirmPassword || sender == textBoxMemberPassword)
                    btnConfirm_Click(null, null);

            }
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "인증 Files|*.dat";
            dlg.FilterIndex = 0;
            dlg.Title = "인증파일 불러오기";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxConfirmCode.Text = dlg.FileName;
            }
        }
    }
}
