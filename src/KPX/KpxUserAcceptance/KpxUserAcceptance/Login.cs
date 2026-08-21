using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KpxUserAcceptance
{
    public partial class Login : Form
    {
        public static DBUtility.WebDBManager dbMgr;
        public const string WebServerURL = "http://211.34.0.91:8080/SOP";

        public Login()
        {
            InitializeComponent();

            this.AcceptButton = button_login;
            dbMgr = new DBUtility.WebDBManager(500);
            dbMgr.DatabaseName = "KPX";
            dbMgr.DatabasePort = "3306";
            dbMgr.DatabaseType = DBUtility.WebDBManager.DBType.mysql;
            dbMgr.WebServerURL = WebServerURL;
            dbMgr.DatabaseHost = "127.0.0.1";
            MainForm.InitDBManager(dbMgr);
            this.FormClosed += Login_FormClosed;
             
            textBox1.Focus();
        }

        void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        } 

        private void button_login_Click(object sender, EventArgs e)
        { 
            try
            {
                //ArrayList arrResult2 = dbMgr.GetResultData("SELECT * FROM USER", 0);
                if (textBox1.Text.Length == 0) throw new ApplicationException("비밀번호를 입력하세요.");

                ArrayList arrResult = dbMgr.GetResultData("SELECT Password FROM Admin", 0);
                if (arrResult == null) return; 
                if (arrResult.Count == 0) throw new ApplicationException("등록된 관리자가 없습니다. \r관리자 변경 메뉴에서 관리자를 먼저 등록하세요");

                if (textBox1.Text == arrResult[0].ToString())
                { 
                    MainForm main = new MainForm();
                    main.Show();

                    this.Hide();
                }
                else throw new ApplicationException("비밀번호가 맞지않습니다.");
            }
            catch (ApplicationException app)
            {
                MessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                ArrayList arrResult = dbMgr.GetResultData("SELECT Password FROM Admin", 0);
                if (arrResult == null) return;
                if (arrResult.Count == 0) throw new ApplicationException("등록된 관리자가 없습니다. \r관리자 변경 메뉴에서 관리자를 먼저 등록하세요");

                ChangePassword cp = new ChangePassword();
                if (cp.ShowDialog() == System.Windows.Forms.DialogResult.OK) 
                    MessageBox.Show("비밀번호가 변경되었습니다."); 
            }
            catch (ApplicationException app)
            {
                MessageBox.Show(app.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AdminRegister ar = new AdminRegister();
            if (ar.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                MessageBox.Show("관리자가 변경되었습니다.");
        }
    }
}
