using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.Utility;

namespace HSMS
{
    public partial class FormLoginMain : Form
    {
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove;

        private static FormLoginMain m_instance = null;
        public static FormLoginMain Instance
        {
            get { return m_instance; }
        }

        private Form m_frmCurrent = null;

        internal FormLogin m_loginForm = null;
        internal FormEditMember m_editForm = null;
        internal FormDeleteMember m_deleteForm = null;
        internal FormChangePassword m_changeForm = null;
        internal FormMemberRegister m_registerForm = null;

        public FormLoginMain()
        {
            m_instance = this;
            InitializeComponent();

            m_loginForm = new FormLogin(this);
            m_editForm = new FormEditMember(this);
            m_deleteForm = new FormDeleteMember(this);
            m_changeForm = new FormChangePassword(this);
            m_registerForm = new FormMemberRegister(this);

            this.Controls.Add(m_loginForm);
            this.Controls.Add(m_editForm);
            this.Controls.Add(m_deleteForm);
            this.Controls.Add(m_changeForm);
            this.Controls.Add(m_registerForm);

            SetCurrentForm(m_loginForm);
            
            ReadProperties();
        }
        
       

        //사용자가 선택한 폼이 현재 선택된 폼인지 확인
        public bool CheckCurrentForm(Type type)
        {
            if (m_frmCurrent != null)
            {
                if (m_frmCurrent.GetType() == type)
                    return false;
                else
                    if (m_frmCurrent != null)
                        m_frmCurrent.Visible = false;
            }

            return true;
        }

        //선택한 폼을 보여줌
        public void SetCurrentForm(Form frm)
        {
            if (m_frmCurrent != null)
                m_frmCurrent.Visible = false;
            m_frmCurrent = frm;
            //this.Controls.Add(m_frmCurrent);
            m_frmCurrent.Visible = true;

            FormLoginMain_Resize(null, null);
        }

        private void FormLoginMain_Resize(object sender, EventArgs e)
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Location = new Point(0, 0);
                m_frmCurrent.Size = this.Size;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (FormMain.Instance.Commander != null)
                FormMain.Instance.Commander.StopCommander();

            this.Close();
            this.Dispose();
        }

        public void FormLoginMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = PointToScreen(new Point(e.X, e.Y));
            }
        }

        public void FormLoginMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

        public void FormLoginMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        private void FormLoginMain_Load(object sender, EventArgs e)
        {
            
        }

        public void ClearLoginTextBox(string szUserID = "")
        {            
            if (szUserID != "")
                m_loginForm.ClearTextBox(szUserID);
            else
                m_loginForm.ClearTextBox();            
        }

        private string m_szUserID = "";

        public string UserID
        {
            get { return m_szUserID; }
            set { m_szUserID = value; }
        }

        public void AcceptLogin(string strUserID)
        {
            //bLoginSuccess = true;
            FormMain.Instance.LoginID = m_szUserID = strUserID;

            DialogResult = DialogResult.OK;
            Close();

        }

        public void RejectLogin(int nType)
        {
            if (nType < 0 || nType >= (int)LoginUserResult.TYPE_COUNT)
                return;

            LoginUserResult result = (LoginUserResult)nType;

            if (result == LoginUserResult.INVALID_PW)
            {
                MessageBox.Show("비밀번호가 맞지 않습니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (result == LoginUserResult.DUPLICATE_LOGIN)
            {
                MessageBox.Show("이미 로그인 중인 아이디입니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (result == LoginUserResult.INVALID_ID)
            {
                MessageBox.Show("삭제된 사용자이거나 사용할 수 없는 아이디입니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (result == LoginUserResult.NOT_PERMIT_PC)
            {
                MessageBox.Show("등록된 컴퓨터가 아닙니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (result == LoginUserResult.DB_IS_DISCONNECTED)
            {
                MessageBox.Show("DB와의 접속이 끊어졌습니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (result == LoginUserResult.NEED_MORE_DATA || result == LoginUserResult.UNKNOWN_ERROR)
            {
                MessageBox.Show("알수없는 에러로 인하여 로그인에 실패하였습니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            ClearLoginTextBox();
        }

        public void FailRegisterUser(JoinUserResult result)
		{
			if (result == JoinUserResult.ALREADY_EXIST)
			{
                MessageBox.Show("이미 존재하는 아이디입니다.", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else if (result == JoinUserResult.DB_IS_DISCONNECTED)
			{
                MessageBox.Show("DB와의 접속이 끊어졌습니다.", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else if (result == JoinUserResult.INVALID_PASSWORD)
			{
                MessageBox.Show("DB에 등록되어 있는 " + m_szUserID + "의 비밀번호가 입력된 값과 일치하지 않습니다.", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
            else if (result == JoinUserResult.INVALID_USER_LEVEL)
            {
                MessageBox.Show("기존에 등록된 계정의 출입등급과 일치하지 않습니다.\r\n인증파일을 다시 확인하세요.", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (result == JoinUserResult.UNKNOWN_JOIN_OPTION)
            {
                MessageBox.Show("알수없는 계정등록 옵션입니다.\r\n인증파일이 잘못되었습니다.", "사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            m_registerForm.ClearTextBoxAll();
		}

        public void SuccessRegisterUser()
		{
			MessageBox.Show("회원가입에 성공하였습니다.\r\n로그인 화면으로 이동합니다.","사용자 등록", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            m_registerForm.ClearTextBoxAll();
            
            ShowLoginForm();
            
            ClearLoginTextBox(m_szUserID);
		}

        public void SuccessChangePassword()
        {
            MessageBox.Show("비밀번호가 변경되었습니다.\r\n로그인 화면으로 이동합니다.", "비밀번호 번경", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ShowLoginForm();
            ClearLoginTextBox(m_szUserID);
        }

        public void FailChangePassword(int nResult)
        {
            if (nResult == (int)ChangePasswordResult.INVALID_CERT_CODE)
                MessageBox.Show("아이디 생성시 사용한 인증파일이 아닙니다.", "비밀번호 번경", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
                MessageBox.Show("비밀번호 변경에 실패하였습니다.\r\n네트웍 접속 상태를 확인해 주세요", "비밀번호 번경", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }     
 
        public void DeleteUser(int nType, string strUserID)
        {
            if (nType < 0 || nType >= (int)DeleteUserResult.TYPE_COUNT)
                return;

            DeleteUserResult result = (DeleteUserResult)nType;

            if (result == DeleteUserResult.INVALID_ID)
            {
                MessageBox.Show("삭제되거나 사용할 수 없는 사용자 아이디입니다.", "삭제 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (result == DeleteUserResult.INVALID_PW)
            {
                MessageBox.Show("비밀번호가 맞지 않습니다.", "삭제 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else if (result == DeleteUserResult.SUCCESS)
            {
                MessageBox.Show(strUserID + " 계정이 삭제되었습니다.");
                m_deleteForm.ClearAllTextBox();
                ShowEidtMemberForm();
            }
            else
            {
                MessageBox.Show("계정 삭제가 실패하였습니다.", "삭제 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            m_deleteForm.ClearTextBox();
        }

        public void SetLogout()
        {
            ShowLoginForm();
            ClearLoginTextBox();
        }

        /*public void SetCode(string szCode)
        {
            m_loginForm.SetCode(szCode);
        }*/

        public void ShowRegisterForm()
        {
            if (CheckCurrentForm(typeof(FormMemberRegister)))
                SetCurrentForm(m_registerForm);
        }

        public void ShowEidtMemberForm()
        {
            if (CheckCurrentForm(typeof(FormEditMember)))
                SetCurrentForm(m_editForm);
        }  

        public void ShowLoginForm()
        {
            if (CheckCurrentForm(typeof(FormLogin)))
                SetCurrentForm(m_loginForm);
        }  

        public void ShowDeleteForm()
        {
            if (CheckCurrentForm(typeof(FormDeleteMember)))
                SetCurrentForm(m_deleteForm);
        }

        public void ShowChangePassForm()
        {
            if (CheckCurrentForm(typeof(FormChangePassword)))
                SetCurrentForm(m_changeForm);
        }

        public void ReadProperties()
        {
            string szServerID = "";
            UnE.Utility.Properties.GetProperty("Data Source", ref szServerID);
            if (szServerID == null || szServerID == "")
            {
                string strEncrypt2 = DBConn.GetInValue("ServerInfo", "HSMS");
                string szKey = new string(new char[] { 'U', 'N', 'E', 'A', 'E', 'S', 'K', 'E', 'Y' });
                string key = "";
                UnE.Utility.Properties.GetProperty(szKey, ref key);
                string strConnection = DBUtility.AES256Cipher.AES_decrypt(strEncrypt2, key);
                string[] conData = strConnection.Split(';');
                for (int i = 0; i < conData.Length; i++)
                {
                    string[] keyvalue = conData[i].Split('=');
                    UnE.Utility.Properties.SetProperty(keyvalue[0].Trim(), keyvalue[1]);
                }

                string HSMSServerIP = DBConn.GetInValue("HSMSServer", "ip_addr");
                UnE.Utility.Properties.SetProperty("ServerIP", HSMSServerIP);
            }
        }
    }
}
