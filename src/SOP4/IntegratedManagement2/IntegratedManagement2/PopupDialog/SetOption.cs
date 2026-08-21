using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntegratedManagement2.PopupDialog
{
    public partial class SetOption : Form
    { 
        private LoginManager m_logInMgr = null;

        private int m_nCompanyMemberID = -1;
        public int ComapnyMember
        {
            get { return m_nCompanyMemberID; }
        } 

        public SetOption(LoginManager logInMgr)
        {
            InitializeComponent();

            m_logInMgr = logInMgr;
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            try
            {
                string strGenUserID = "";
                int nCompanyMemberID = m_logInMgr.GetMemberID(textBoxMemberID.Text, textBoxMemberName.Text, ref strGenUserID);
                if (nCompanyMemberID == -2)
                {
                    throw new ApplicationException("삭제된 직원이거나 직원 정보가 잘못되었습니다.");
                }
                else if (nCompanyMemberID < 0)
                {
                    throw new ApplicationException("입력된 직원 정보가 잘못되었습니다.");
                }
                else if (nCompanyMemberID == 0)
                {
                    throw new ApplicationException("이미 회원가입이 되어 있습니다.");
                }
                else
                {
                    labelMemberID.Tag = nCompanyMemberID;
                    
                    this.DialogResult = System.Windows.Forms.DialogResult.Yes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
