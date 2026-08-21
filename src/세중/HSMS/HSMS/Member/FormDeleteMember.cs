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
    public partial class FormDeleteMember : Form, UnE.GUI.IRibbonButtonOwner
    {
        private FormLoginMain m_formParent = null;

        public ToolStripStatusLabel GetStatusLabel()
        {
            return null;
        }

        public FormDeleteMember(FormLoginMain form)
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

        private void btnCancel_Click(object sender, EventArgs e)
        {

            ClearAllTextBox();
            m_formParent.ShowEidtMemberForm();
        }

        private void FormDeleteMember_Load(object sender, EventArgs e)
        {
            this.btnConfirm.Owner = this;
            this.btnCancel.Owner = this;
        }

        //인터페이스 메서드 구현
        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
        }       

        //TextBox내용 검사... 
        private bool CheckTextBox()
        {
            if (textBoxCurrentID.Text.Length == 0)
            {
                MessageBox.Show("아이디를 입력하세요", "사용자 삭제", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxCurrentID.Focus();
                return false;
            }
            else if (textBoxCurrentPassword.Text.Length == 0)
            {
                MessageBox.Show("비밀번호를 입력하세요", "사용자 삭제", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                textBoxCurrentPassword.Focus();
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
            string szPass = textBoxCurrentPassword.Text;
            string szUser = textBoxCurrentID.Text;

            if (szPass == "" || szUser == "")
                return;

            if(!LoginManager.Instance.DeleteUser(szUser, szPass))
            {
                MessageBox.Show("서버에 연결할 수 없습니다.\r\n네트웍 접속 상태를 확인해 주세요", "사용자 삭제", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            bool nCheckData = CheckTextBox();

            if (nCheckData == true)
            {
                DialogResult dr = MessageBox.Show("정말 삭제하시겠습니까?", "알림", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                if (dr == DialogResult.OK)
                {
                    FunctionDataCompare();
                }                
            }
        }

        public void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (sender == textBoxCurrentID || sender == textBoxCurrentPassword)
                    btnConfirm_Click(null, null);

            }
        }

        public void ClearAllTextBox()
        {
            textBoxCurrentPassword.Text = "";
            textBoxCurrentID.Text = "";
        }

        public void ClearTextBox()
        {
            textBoxCurrentPassword.Text = "";
            //textBoxCurrentID.Text = "";
        }
    }
}
