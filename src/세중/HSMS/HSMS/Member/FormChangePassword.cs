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
    public partial class FormChangePassword : Form, UnE.GUI.IRibbonButtonOwner
    {
        private bool m_isConfirmCode = false;
        private ArrayList m_arrMacAddrList = new ArrayList();
        private string m_strCertCode = "";

        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        private FormLoginMain m_formParent = null;
        public FormChangePassword(FormLoginMain form)
        {
            InitializeComponent();
            this.TopLevel = false;
            m_formParent = form;
            MouseDown += new MouseEventHandler(m_formParent.FormLoginMain_MouseDown);
            MouseMove += new MouseEventHandler(m_formParent.FormLoginMain_MouseMove);
            MouseUp += new MouseEventHandler(m_formParent.FormLoginMain_MouseUp);

            initButton();  
        }
        private void initButton()
        {
            this.btnConfirm.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.btnCancel.Font = new System.Drawing.Font("맑은 고딕", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
        }

        //인터페이스 메서드 구현
        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
        }

        private void FormChangePassword_Load(object sender, EventArgs e)
        {
            this.btnConfirm.Owner = this;
            this.btnCancel.Owner = this;
        }
        public void ClearTextBoxAll()
        {
            textBoxCurrentID.Text = "";
            textBoxChangingPassword.Text = "";
            textBoxConfirmChanging.Text = "";
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearTextBoxAll();
            m_formParent.ShowEidtMemberForm();
        }

        //TextBox내용 검사... 
        private bool CheckTextBox()
        {
            if (textBoxCurrentID.Text.Length == 0)
            {
                MessageBox.Show("아이디를 입력하세요", "비밀번호 번경", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);      
                textBoxCurrentID.Focus();
                return false;
            }
            else if (textBoxChangingPassword.Text.Length == 0)
            {
                MessageBox.Show("새 비밀번호를 입력하세요", "비밀번호 번경", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);      
                textBoxChangingPassword.Focus();
                return false;
            }
            else if (textBoxConfirmChanging.Text.Length == 0)
            {
                MessageBox.Show("비밀번호 확인을 입력하세요", "비밀번호 번경", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);      
                textBoxConfirmChanging.Focus();
                return false;
            }
            else if (textBoxChangingPassword.Text != textBoxConfirmChanging.Text)
            {
                MessageBox.Show("비밀번호와 비밀번호확인이 동일하지 않습니다.", "비밀번호 번경", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);      
                textBoxChangingPassword.Focus();
                return false;
            }
            else if (textBoxChangingPassword.Text.Length < 4) //이부분
            {
                MessageBox.Show("비밀번호가 4자리 이하 입니다.", "비밀번호 번경", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxChangingPassword.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        //아이디와 비밀번호가 맞는지 확인
        private void FunctionDataCompare()
        {
            //입력된 비밀번호를 암호화한다.
            string szNewPass = textBoxConfirmChanging.Text;
            string szUserID = textBoxCurrentID.Text;

            string strMacAddrList = "";

            foreach (string strMacAddr in m_arrMacAddrList)
            {
                strMacAddrList += strMacAddr;
            }

            if(!LoginManager.Instance.ChangePassword(szUserID, m_strCertCode, strMacAddrList, szNewPass))
            {
                MessageBox.Show("서버에 연결할 수 없습니다.\r\n네트웍 접속 상태를 확인해 주세요", "비밀번호 번경", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);      
            }            
        }


        private void btnConfirm_Click(object sender, EventArgs e)
        {
            bool nCheckData = CheckTextBox();
            if (nCheckData == true)
            {
                DialogResult dr = MessageBox.Show("정말 변경하시겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                if (dr == DialogResult.OK)
                {
                    m_formParent.UserID = textBoxCurrentID.Text;
                    FunctionDataCompare();
                    ClearTextBoxAll();
                }                
            }

        }

        public void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (/*sender == textBoxCurrentPassword || */sender == textBoxChangingPassword || sender == textBoxConfirmChanging || sender == textBoxCurrentID)
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

        private void textBoxConfirmCode_TextChanged(object sender, EventArgs e)
        {
            if (!File.Exists(textBoxConfirmCode.Text))
            {
                m_isConfirmCode = false;
                return;
            }

            m_arrMacAddrList.Clear();

            if (!FormMemberRegister.GetCertData(textBoxConfirmCode.Text, out m_strCertCode, m_arrMacAddrList))
                return;

            //실제 등록 될 인증코드
            string strIDCode;
            //관리자인지 일반인지 구별
            bool isAdmin;
            UnE.KeyValidator.CertOption option = UnE.KeyValidator.CertOption.NEW_CREATE;

            string szServerID = "";
            string szDBName = "";
            string szDBUser = "";
            string szDBPass = "";
            UnE.Utility.Properties.GetProperty("Data Source", ref szServerID);
            UnE.Utility.Properties.GetProperty("Initial Catalog", ref szDBName);
            UnE.Utility.Properties.GetProperty("User ID", ref szDBUser);
            UnE.Utility.Properties.GetProperty("Password", ref szDBPass);

            UnE.KeyValidator.CertResult result = UnE.KeyValidator.Manager.VaildKey(szServerID, szDBUser, szDBPass, szDBName, "LoginUser", "code", m_strCertCode, m_arrMacAddrList, out strIDCode, out isAdmin, out option);

            if (result != UnE.KeyValidator.CertResult.SUCCESS)
            {
                m_isConfirmCode = false;
                MessageBox.Show("잘못된 인증코드입니다.");
                return;
            }
        }
    }
}
