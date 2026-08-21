using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;

namespace SOPGen
{
    public partial class FormLogin : Form
    {
        private FormMain m_Main = null;
        private string m_strLoginID = null;

        public FormLogin(FormMain main)
        {
            InitializeComponent();

            m_Main = main;
            m_Main.SkinFolder = m_Main.StylesPath();
            Skin_Load();
        }
 
        public void Skin_Load()
        {
            axSkinFramework1.LoadSkin(m_Main.SkinFolder + "Vista.cjstyles", "");
            axSkinFramework1.ApplyWindow(this.Handle.ToInt32());
            this.BackColor = axSkinFramework1.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BTNFACE);
        }

        public string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");

            return strExePath + "\\Styles\\";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
//             DBManager dbMgr = m_Main.m_dbMgr;
//             dbMgr.Level = dbMgr.GetUserID(textID.Text, textPW.Text);
// 
//             if (dbMgr.Level == -1)
//             {
//                 MessageBox.Show("아이디 또는 비밀번호가 맞지 않습니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
//                 return;
//             }

            WebDBManager dbMgr = m_Main.m_dbMgr;
            ArrayList arr = new ArrayList();
            int nLevel = dbMgr.GetUserID(textID.Text, textPW.Text);

            if (nLevel == -1)
            {
                MessageBox.Show("아이디 또는 비밀번호가 맞지 않습니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            m_strLoginID = textID.Text;
            //m_strLoginID = "kolee";

            this.DialogResult = DialogResult.OK;

            Close();
        }

        public string GetLoginID()
        {
            return m_strLoginID;
        }

        private void textID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void textPW_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void FormLogin_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
                Application.Exit();
        }
    }
}
