using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace HSMS
{
    public partial class FormLogin : Form, UnE.GUI.IRibbonButtonOwner
    {
        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        private FormLoginMain m_formParent = null;
        public FormLogin(FormLoginMain form)
        {
            InitializeComponent();
            this.TopLevel = false;
            
            m_formParent = form;
            MouseDown += new MouseEventHandler(m_formParent.FormLoginMain_MouseDown);
            MouseMove += new MouseEventHandler(m_formParent.FormLoginMain_MouseMove);
            MouseUp += new MouseEventHandler(m_formParent.FormLoginMain_MouseUp);           

            initButton();            
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            MouseDown -= m_formParent.FormLoginMain_MouseDown;
            MouseMove -= m_formParent.FormLoginMain_MouseMove;
            MouseUp -= m_formParent.FormLoginMain_MouseUp;          
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            btnLogin.Owner = this;
            btnRegMember.Owner = this;
            btnChangePwd.Owner = this;
            btnSetup.Owner = this;

            textBoxID.Focus();
        }

        private void initButton()
        {
            this.btnLogin.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnRegMember.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnChangePwd.Font = new System.Drawing.Font("맑은 고딕", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }

        /*private string m_szCode = "";



        private bool CheckCode(string szCode, out string strIDCode, out string szMsg)
        {

           

            //실제 등록 될 인증코드
            strIDCode = "";
            //관리자인지 일반인지 구별
            bool isAdmin;
            szMsg = "";

            if (szCode == "")
            {
                MessageBox.Show("아이디 또는 비밀번호가 맞지 않습니다.", "사용자 로그인", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            m_formParent.ReadProperties();

            string szServerID = "";
            string szDBName = "";
            string szDBUser = "";
            string szDBPass = "";
            UnE.Utility.Properties.GetProperty("Data Source", ref szServerID);
            UnE.Utility.Properties.GetProperty("Initial Catalog", ref szDBName);
            UnE.Utility.Properties.GetProperty("User ID", ref szDBUser);
            UnE.Utility.Properties.GetProperty("Password", ref szDBPass);

            int nResult = UnE.KeyValidator.Manager.VaildKey(szServerID, szDBUser, szDBPass, szDBName, "LoginUser", "code", szCode, out strIDCode, out isAdmin);

            UnE.Utility.Properties.SetProperty("isAdmin", isAdmin== true ? 1 : 0);

            if (nResult == 1)
            {
                szMsg = "등록된 코드가 없습니다.\n계정을 삭제한 후에 다시 등록해 주세요.";
            }    
            else if( nResult == 2)
            { 
                szMsg = "등록된 Database가 아닙니다.";
            }
            else if(nResult == 3)
            {
                szMsg = "등록시 사용된 PC가 아닙니다.";
            }
            else if(nResult == 0)
            {
                return true;
            }
            return false;
        }*/

        /*public void SetCode(string szCode)
        {

            
            m_szCode = szCode;

            string szHashCode = "";
            string szMsg = "";
            if(!CheckCode(szCode, out szHashCode, out szMsg))
            {
                MessageBox.Show(szMsg, "사용자 로그인", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            //입력된 비밀번호를 암호화한다.
            string strTextBoxPassword = textBoxPassword.Text;

            //bool bLoginSuccess = false;

            string szUserID = textBoxID.Text;

            if (!LoginManager.Instance.LogIn(szUserID, strTextBoxPassword, szHashCode))
            {
                MessageBox.Show("서버에 연결할 수 없습니다.\r\n네트웍 접속 상태를 확인해 주세요", "사용자 로그인", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }*/

        //로그인(아이디,비번 비교)
        private void FunctionLogin()
        {
            //입력된 비밀번호를 암호화한다.
            string strEncryptedPW = DBUtility.AES256Cipher.AES_encrypt(textBoxPassword.Text, DBConn.Key);

            //bool bLoginSuccess = false;

            string szUserID = textBoxID.Text;

            if (!LoginManager.Instance.RequestLogin(szUserID, strEncryptedPW))
            {
                MessageBox.Show("서버에 연결할 수 없습니다.\r\n네트웍 접속 상태를 확인해 주세요", "사용자 로그인", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            /*if(!LoginManager.Instance.RequestCode(szUserID, strTextBoxPassword))
            {
                MessageBox.Show("서버에 연결할 수 없습니다.\r\n네트웍 접속 상태를 확인해 주세요", "사용자 로그인", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);               
            }*/
        }

        

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (textBoxPassword.Text == "" || textBoxID.Text == "")
                return;
            FunctionLogin();
        }

        private void btnRegMember_Click(object sender, EventArgs e)
        {
            m_formParent.ShowRegisterForm();
        }
     

        //인터페이스 메서드 구현
        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
        }

        private void btnSetup_Click(object sender, EventArgs e)
        {
            m_formParent.ShowEidtMemberForm();
        }

        public void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (sender == textBoxID || sender == textBoxPassword)
                    btnLogin_Click(null, null);
            }
        }

        private void FormLogin_Shown(object sender, EventArgs e)
        {
            textBoxID.Focus();
        }

        public void ClearTextBox()
        {
            textBoxPassword.Clear();
            textBoxPassword.Focus();
        }

        public void ClearTextBox(string szUserID)
        {
            textBoxID.Text = szUserID;
            textBoxPassword.Clear();
            textBoxPassword.Focus();
        }

        private void btnChangePwd_Click(object sender, EventArgs e)
        {
            m_formParent.ShowChangePassForm();
        }
    }
}
