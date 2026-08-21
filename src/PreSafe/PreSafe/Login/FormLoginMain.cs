using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreSafe
{
    internal partial class FormLoginMain : Form
    {
        private string strPath = @"C:\PreSafeTemp\a.txt";
        public string Path
        {
            get { return strPath; }
            set { strPath = value; }
        }

        private static FormLoginMain m_instance = null;
        public static FormLoginMain Instance
        {
            get { return FormLoginMain.m_instance; }
        }

        private Form m_frmCurrent = null;

        internal FormLogin m_loginForm = null;
        //internal FormEditMember m_editForm = null;
        //internal FormDeleteMember m_deleteForm = null;
        //internal FormChangePassword m_changeForm = null;
        internal FormMemberRegister m_registerForm = null;

        public FormLoginMain()
        {
            m_instance = this;
            InitializeComponent();

            m_loginForm = new FormLogin(this);
            //m_editForm = new FormEditMember(this);
            //m_deleteForm = new FormDeleteMember(this);
            //m_changeForm = new FormChangePassword(this);
            m_registerForm = new FormMemberRegister(this);

            this.Controls.Add(m_loginForm);
            //this.Controls.Add(m_editForm);
            //this.Controls.Add(m_deleteForm);
            //this.Controls.Add(m_changeForm);
            this.Controls.Add(m_registerForm);

            SetCurrentForm(m_loginForm);
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

        private void FormLoginMain_Load(object sender, EventArgs e)
        {

        }

        private void FormLoginMain_Resize(object sender, EventArgs e)
        {
            if (m_frmCurrent != null)
            {
                m_frmCurrent.Location = new Point(0, 0);
                m_frmCurrent.Size = this.Size;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
           // this.Close();
            //this.Dispose();
        }

        public void ShowRegisterForm()
        {
            if (CheckCurrentForm(typeof(FormMemberRegister)))
                SetCurrentForm(m_registerForm);
        }

        public void ShowLoginForm()
        {
            if (CheckCurrentForm(typeof(FormLoginMain)))
                SetCurrentForm(m_loginForm);
        }
    }
}
