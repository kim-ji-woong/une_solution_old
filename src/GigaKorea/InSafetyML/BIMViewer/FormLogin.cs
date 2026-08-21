using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BIMViewer
{
    public partial class FormLogin : Form
    {
        public bool m_bResult = false;
        public string m_sID = "";
        public string m_sPW = "";
        private Dictionary<string, string> m_spaceUserList = null;
        public FormLogin(Dictionary<string, string> spaceUserList)
        {
            InitializeComponent();
            m_spaceUserList = spaceUserList;
        }
        public new void Show(IWin32Window owner)
        {           
            base.Show(owner);
        }

        private void FormLoginLoad(object sender, EventArgs e)
        {
            txtUserID.Text = "user_spatial";
            txtUserKey.Text = "spatial1234";
            //txtUserID.Text = "ACC_0001_20190822152055207";
            //txtUserKey.Text = "spaceInfo100";
        }
        private void RbtnOK_Click(object sender, EventArgs e)
        {
            string sID = txtUserID.Text.Trim();
            string sPW = txtUserKey.Text.Trim();
            //사용자 목록 체크
            //if(!m_spaceUserList.ContainsKey(sID))
            //{
            //    MessageBox.Show("공간정보 사용자가 아닙니다.", "로그인오류");
            //    return;
            //}
            string tmpValue;
            m_spaceUserList.TryGetValue(sID, out tmpValue);
            //if(sPW != tmpValue)
            //{
            //    MessageBox.Show("패스워드 오류", "로그인오류");
            //    return;
            //}
            
            //로그인
            WebServiceManager mgr = new WebServiceManager(sID, sPW);
            if (!mgr.Login(out m_sID, out m_sPW))
            {
                MessageBox.Show("Login Failed!");
                return;
            }
            else
            {
                m_bResult = true;
                this.Close();
            }                
        }



        private void RbtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //폼움직이게
        private Point mousePoint;
        private void FormLogin_MouseDown(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);
        }

        private void FormLogin_MouseMove(object sender, MouseEventArgs e)
        {
            if((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                Location = new Point(this.Left - (mousePoint.X - e.X), this.Top - (mousePoint.Y - e.Y));
            }
        }
    }
}
