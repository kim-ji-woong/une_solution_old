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
    public partial class ChangePassword : Form
    {
        public static DBUtility.WebDBManager dbMgr;
        public ChangePassword()
        {
            InitializeComponent();

            dbMgr = new DBUtility.WebDBManager(500);
            dbMgr.DatabaseName = "KPX";
            dbMgr.DatabasePort = "3306";
            dbMgr.DatabaseType = DBUtility.WebDBManager.DBType.mysql;
            dbMgr.WebServerURL = Login.WebServerURL;
            dbMgr.DatabaseHost = "127.0.0.1";
            MainForm.InitDBManager(dbMgr);

            this.AcceptButton = button_ok;
        } 

        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            try
            { 
                if (textBox1.Text.Length == 0) throw new ApplicationException("현재 비밀번호를 입력하세요.");
                if (textBox2.Text.Length == 0 || textBox3.Text.Length == 0) throw new ApplicationException("변경할 비밀번호를 입력하세요");
                if (textBox2.Text != textBox3.Text) throw new ApplicationException("변경할 비밀번호를 확인하세요");
                 
                string strQuery = "UPDATE Admin SET Password='" + textBox3.Text + "'";

                if (dbMgr.GetResultData(strQuery, 0) != null) 
                    this.DialogResult = System.Windows.Forms.DialogResult.OK; 
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
    }
}
