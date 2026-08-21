using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using SOPMonitoringSystem;

namespace IntegratedManagement
{
    public partial class FormMain : Form
    {
        public WebDBManager m_dbMgr = null;
        private string m_strMemberName;
        private int m_nIndex;
        private AES256Cipher aes;

        public FormMain()
        {
            m_nIndex = -1;
            InitializeComponent();
            pictureBox1.Visible = false;
            Skin_Load();
            
            m_dbMgr = new WebDBManager(this);

            Panel_visible(1); // 1 로그인 / 2 회원가입 / 3 비밀번호 찾기 / 4 비밀번호 바꾸기 / 5 로그인후
            aes = new AES256Cipher();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_nIndex != -1)
            {
                CheckLogoutUser(m_szLoginId);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_nIndex != -1)
            {
                CheckLoginUser(m_szLoginId, "Check");
            }
        }

        public void Panel_visible(int Num) // 1 로그인 / 2 회원가입 / 3 비밀번호 찾기 / 4 비밀번호 바꾸기 / 5 로그인후
        {
            panelLogin.Visible = false;
            panel2.Visible = false;
            btnLogout.Visible = false;
            panelJoin.Visible = false;
            panelCheck.Visible = false;
            panelPassChange.Visible = false;

            switch(Num)
            {
                case 1:
                    panelLogin.Visible = true;
                    break;
                case 2:
                    panelJoin.Visible = true;
                    break;
                case 3:
                    panelCheck.Visible = true;
                    break;
                case 4:
                    panelPassChange.Visible = true;
                    break;
                case 5:
                    panel2.Visible = true;
                    btnLogout.Visible = true;
                    break;
            }

        }
        
        public void Skin_Load()
        {
            string strSkinFolder = StylesPath();

            axSkinFramework.LoadSkin(strSkinFolder + "Vista.cjstyles", "");
            axSkinFramework.ApplyWindow(this.Handle.ToInt32());
            //this.BackColor = axSkinFramework.GetColor(XtremeSkinFramework.XTPColorManagerColor.STDCOLOR_BTNFACE);
        }

        public string StylesPath()
        {
            string strExePath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            System.IO.Directory.Exists(strExePath + "\\Styles\\");

            return strExePath + "\\Styles\\";
        }

        private string m_szLoginId = "";
        private void btnLogin_Click(object sender, EventArgs e) // 로그인
        {
            ArrayList arr = new ArrayList();

            m_szLoginId = textBoxID.Text;
            bool loginResult = false;

            if (CheckLoginUser(m_szLoginId, "Check"))
            {
                if (bFail == false)
                    MessageBox.Show("이미 로그인 중인 아이디입니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                text_clear();
                return;
            }
            else
            {
                loginResult = CheckLoginUser(m_szLoginId, "Login");
                timer1.Start();
            }
            
            int nLevel = GetUserID(textBoxID.Text, textBoxPassword.Text);

            if (nLevel == -1)
            {
                MessageBox.Show("아이디 또는 비밀번호가 맞지 않습니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                if (loginResult)
                {
                    CheckLogoutUser(m_szLoginId);
                }

                text_clear();
                return;
            }
			bLoginState = true;
            Panel_visible(5);
        }
		private bool bLoginState = false;
        bool bFail = false;
        private bool CheckLoginUser(string szUserID, string szCmd)
        {
            if (szUserID == null || szUserID.Equals(""))
                return false;
            
            bool bResult = m_dbMgr.GetResultCheckUser(szUserID, szCmd, ref bFail);
            return bResult; 
        }

        private bool CheckLogoutUser(string szUserID)
        {

            if (szUserID == null || szUserID.Equals(""))
                return false;
            timer1.Stop();
            bool bResult = m_dbMgr.GetResultCheckUser(szUserID, "Logout", ref bFail);
			bLoginState = false;
            return bResult; 
        }

        private void btnJoin_Click(object sender, EventArgs e) // 회원가입
        {
            Panel_visible(2);
        }

        private void btnPasswordCheck_Click(object sender, EventArgs e) // 비밀번호 찾기
        {
            Panel_visible(3);
        }

        private void btnLogout_Click(object sender, EventArgs e) // 로그아웃
        {           
            CheckLogoutUser(m_szLoginId);
            m_nIndex = -1;
            Panel_visible(1); // 1 로그인 / 2 회원가입 / 3 비밀번호 찾기 / 4 비밀번호 바꾸기 / 5 로그인후
            text_clear();

			bLoginState = false;
        }

        private void btnOK_J_Click(object sender, EventArgs e) // 회원가입에서 확인버튼
            // 0: 회원가입 완료 / 1: 비밀번호 불일치 / 2: 사원번호나 이름 불일치 / 3: 이미 가입된 회원
        {
            int n = SetUser(textBoxID_J.Text, textBoxName_J.Text, textBoxPassword_J.Text, textBoxPPassword_J.Text);

            if (n == 0)
            {
                Panel_visible(1);
                MessageBox.Show("회원가입이 완료되었습니다.");
            }
            else if (n == 1)
                MessageBox.Show("비밀번호를 다시 확인하여주십시오.\n( 영어 : 4자 ~ 20자 )\n( 한글 : 2자 ~ 10자 )");
            else if (n == 2)
                MessageBox.Show("사원번호나 이름이 일치하지 않습니다.");
            else if (n == 3)
                MessageBox.Show("이미 가입된 회원입니다.");
            else if (n == 4)
                MessageBox.Show("실패하였습니다");


            text_clear();
        }

        private void btnCancel_J_Click(object sender, EventArgs e) // 회원가입에서 Cencel
        {
            text_clear();
			Panel_visible(1);
        }

        private void btnOK_P_Click(object sender, EventArgs e) // 비밀번호찾기에서 확인
        {
            int n = CheckPassword(textBoxID_C.Text, textBoxName_C.Text);

            if (n == 1)
            {
                MessageBox.Show("사원번호나 이름이 일치하지 않습니다.");
                Panel_visible(3);
            }
            else if (n == 2)
            {
                MessageBox.Show("가입되어있지 않은 회원입니다.");
                Panel_visible(3);
            }
            else
                Panel_visible(1);

            text_clear();
        }

        private void btnCancel_P_Click(object sender, EventArgs e) // 비밀번호찾기에서 캔슬
        {
            text_clear();
            Panel_visible(1); // 1 로그인 / 2 회원가입 / 3 비밀번호 찾기 / 4 비밀번호 바꾸기 / 5 로그인후           
        }

        private void btnOK_C_Click(object sender, EventArgs e) // 비밀번호변경에서 확인
        {
            int i = ChangePassword(textBoxPass_c.Text, textBoxCheckPass_c.Text, textBoxCheckPPass_c.Text);

            if (i == 1)
            {
                MessageBox.Show("비밀번호를 다시 확인하여주십시오.\n( 영어 : 4자 ~ 20자 )\n( 한글 : 2자 ~ 10자 )");
                Panel_visible(4);
            }
            else
                Panel_visible(1);

            text_clear();
        }

        private void btnCancel_C_Click(object sender, EventArgs e) // 비밀번호변경에서 취소
        {
			text_clear();
			if (bLoginState == true)
			{
				Panel_visible(5);
			}
			else
				Panel_visible(1);
        }

        private void btnPassChange_Click(object sender, EventArgs e) // 비밀번호 변경
        {
            Panel_visible(4); // 1 로그인 / 2 회원가입 / 3 비밀번호 찾기 / 4 비밀번호 바꾸기 / 5 로그인후
        }
        private void btnManager_Click(object sender, EventArgs e)
        {
            if (!RunCheckProcess("SOPManager"))
            {
                string strValue = m_nIndex.ToString() + " " + textBoxID.Text + " " + m_strMemberName;
                RunStartProcess("SOPManager.exe", strValue);
            }
        }

        private void btn_SDMS_Click(object sender, EventArgs e)
        {
            if (!RunCheckProcess("SDMS"))
            {
                string strValue = m_nIndex.ToString() + " " + m_strMemberName + " 1";
                RunStartProcess("SDMS.exe", strValue);
            }
            else if (!RunCheckProcess("SDMS1"))
            {
				string strValue = m_nIndex.ToString() + " " + m_strMemberName + " 2";
                RunStartProcess("SDMS1.exe", strValue);
            }
        }

        private void btn_MessageSend_Click(object sender, EventArgs e)
        {
            if (!RunCheckProcess("MessageSend"))
            {
                RunStartProcess("MessageSend.exe", textBoxID.Text);
            }
        }

        private void btnMonitoring_Click(object sender, EventArgs e)
        {
            if (!RunCheckProcess("SOPMonitoringSystem"))
            {
                string strValue = m_nIndex.ToString() + " " + m_strMemberName;
                RunStartProcess("SOPMonitoringSystem.exe", strValue);
            }
        }

        private void btnTeamManager_Click(object sender, EventArgs e)
        {
            if (!RunCheckProcess("TeamManagementSystem"))
            {
                RunStartProcess("TeamManagementSystem.exe", m_nIndex.ToString());
            }
        }

        private void textBoxPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                btnLogin_Click(sender, e);
            }
        }

        private string GetExecutablePath()
        {
            string strExePath = Application.ExecutablePath;
            int nIndex = strExePath.LastIndexOf('\\');
            string strTemp = strExePath.Substring(0, nIndex);

            return strTemp + "\\";
        }

        private void text_clear()
        {
            textBoxID.Text = textBoxPassword.Text = textBoxID_C.Text = textBoxName_C.Text = 
            textBoxName_J.Text = textBoxID_J.Text = textBoxPassword_J.Text = textBoxPPassword_J.Text = 
            textBoxPass_c.Text = textBoxCheckPass_c.Text = textBoxCheckPPass_c.Text = "";
        }

        //private void RunStartProcess(string strFileName)
        //{
        //    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        //    startInfo.FileName = strFileName;
        //    startInfo.WorkingDirectory = GetExecutablePath();
        //    //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
        //    startInfo.ErrorDialog = true;

        //    System.Diagnostics.Process process;
        //    try
        //    {
        //        process = System.Diagnostics.Process.Start(startInfo);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        private void RunStartProcess(string strFileName, string args)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = strFileName;
            startInfo.WorkingDirectory = GetExecutablePath();
            //startInfo.WorkingDirectory = @"E:\work\UnESolution\bin\common\Virtools4.0\\";// GetExecutablePath();
            //startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
            startInfo.ErrorDialog = true;

            System.Diagnostics.Process process;
            try
            {
                process = System.Diagnostics.Process.Start(strFileName, args);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //strProcessName을 가진 프로그램이 실행중인지 체크
        private bool RunCheckProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == strProcessName)
                    return true;
            }

            return false;
        }

        public int GetUserID(string strID, string strPassword)
        {
            int nLevel = -1;
            string strKey = "";

            ArrayList arrUser = new ArrayList();
            ReadDB_TableUsers(ref arrUser);

            for (int i = 0; i < key.Length; i++)
            {
                strKey += key[i];
            }

            for (int nList = 0; nList < arrUser.Count; nList++)
            {
                Data_SOPGenUser dataUser = (Data_SOPGenUser)arrUser[nList];
                if (dataUser == null) continue;

                if (dataUser.UserID == strID)
                {
                    String decode = aes.AES_encrypt(strPassword, strKey); //암호화
                    //String decode = aes.AES_decrypt(dataUser.Password, strKey); //복호화

                    decode = decode.Replace("+"," ");

                    if (dataUser.Password.ToString() == decode.ToString())
                    //if (decode == textBoxPassword.Text)
                    {
                        nLevel = dataUser.UserLevel;
                        m_strMemberName = dataUser.UserName;
                        m_nIndex = dataUser.ID;
                        break;
                    }
                }
            }
            return nLevel;
        }

        private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
        //private char[] key = new char[32] { 'u', 'n', 'e', 'c', 'o', 'm', 'p', 'a', 'n', 'y', 'u', 'n', 'e', 'c', 'o', 'm', 'p', 'a', 'n', 'y', 'u', 'n', 'e', 'c', 'o', 'm', 'p', 'a', 'n', 'y','1','2'};
        //////////////////////////////////////////////////////////////////////////
        // UserID를 가져온다 (strID:아이디, strPassword:비밀번호, return=-1:입력한UserID가DB에존재하지않음, 1:guest,2:member,3:leader,4:admin)

        public int SetUser(string ID, string Name, string Pass, string PPass)//string ID, string Pass, string PPass, string Name) // 회원가입
        {
            int result = WriteDB_TableUsers(ID, Name , Pass, PPass);

            return result;  // 0: 회원가입 완료 / 1: 비밀번호 불일치 / 2: 사원번호나 이름 불일치 / 3: 이미 가입된 회원
        }

        public int CheckPassword(string ID, string Name)
        {
            if (Encoding.Default.GetByteCount(Name) < 1 || Encoding.Default.GetByteCount(ID) < 1)
                return 1;
            else
            {
                int Num = ReadDB_PassSearch(ID, Name);

                return Num;
            }
        }

        public int ChangePassword(string Pass, string Pass_c, string PPass_c)
        {
            if ((Pass_c.ToString() != PPass_c.ToString()) || Encoding.Default.GetByteCount(Pass_c) < 4 || Encoding.Default.GetByteCount(Pass_c) > 20)
            {
                return 1;
            }
            else
            {
                int Num = ReadDB_PassChange(Pass, Pass_c, PPass_c);
                return Num;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        // ReadDB_
        public void ReadDB_TableUsers(ref ArrayList arrUser)
        {
            arrUser.Clear();

            //string strSQL = "SELECT * FROM SOPGenUser";
            string strSQL = "SELECT us.id, us.MemberID, cm.MemberName, us.UserLevel, cm.RegularTeamID, us.Password, us.UserID FROM SOPGenUser as us, CompanyMember as cm WHERE us.MemberID = cm.ID";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            for (int i = 0; i < arrResult.Count - 6; i = i + 7)
            {
                Data_SOPGenUser dataNew = new Data_SOPGenUser();
                dataNew.ID = m_dbMgr.GetIntField(arrResult[i].ToString(), 0);
                dataNew.MemberID = m_dbMgr.GetIntField(arrResult[i + 1].ToString(), 0);
                dataNew.UserName = m_dbMgr.GetStringField(arrResult[i + 2].ToString(), "");
                dataNew.UserLevel = m_dbMgr.GetIntField(arrResult[i + 3].ToString(), 0);
                dataNew.TeamID = m_dbMgr.GetIntField(arrResult[i + 4].ToString(), 0);
                dataNew.Password = m_dbMgr.GetStringField(arrResult[i + 5].ToString(), "");
                dataNew.UserID = m_dbMgr.GetStringField(arrResult[i + 6].ToString(), "");

                arrUser.Add(dataNew);
            }
        }
        // 패스워드 찾기
        public int ReadDB_PassSearch(string ID, string Name)
        {
            ////////////////////////////////사원 이름 불일치/////////////////////////////////////////////////////////////////////////
            string strSQL = "SELECT * FROM CompanyMember where MemberName = '" + Name + "' and MemberID  = " + ID;
            string strKey = "";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                return 1;
            }

            ////////////////////////////////가입되어있는지 확인/////////////////////////////////////////////////////////////////////////
            strSQL = "SELECT * FROM SOPGenUser where UserID = '" + ID + "'";
            arrResult.Clear();
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                return 2;
            }

            ////////////////////////////////비밀번호 변경/////////////////////////////////////////////////////////////////////////
            System.Random Ran = new System.Random(); // 임시 비밀번호 생성
            int pass = Ran.Next(11111, 99999);
            MessageBox.Show("임시 비밀번호\n" + pass.ToString());

            for (int i = 0; i < key.Length; i++)
            {
                strKey += key[i];
            }
            String decode = aes.AES_encrypt(pass.ToString(), strKey); //암호화

            strSQL = "update SOPGenUser set Password='" + decode + "' where UserID='" + ID + "'"; // GenUser의 ID 값

            return m_dbMgr.GetResultData(strSQL, 1) != null ? 0 : 1;
        }
        // 패스워드 변경
        public int ReadDB_PassChange(string Pass, string Pass_c, string PPass_c)
        {
            string strKey = "";
            for (int i = 0; i < key.Length; i++)
            {
                strKey += key[i];
            }
            String decode = aes.AES_encrypt(Pass.ToString(), strKey);
            string strSQL = "SELECT * FROM SOPGenUser where Password = '" + decode + "' and UserID  = '" + textBoxID.Text + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);
            
            int a = arrResult.Count;

            if (a == 0)
                return 1;

            decode = aes.AES_encrypt(Pass_c.ToString(), strKey); //암호화

            strSQL = "update SOPGenUser set Password='" + decode + "' where UserID='" + textBoxID.Text + "'"; // GenUser의 ID 값

            return m_dbMgr.GetResultData(strSQL, 1) != null ? 0 : 1;
        }
        // WriteDB_
        public int WriteDB_TableUsers(string ID, string Name, string Pass, string PPass) // 0: 회원가입 완료 / 1: 비밀번호 불일치 / 2: 사원번호나 이름 불일치 / 3: 이미 가입된 회원
        {
            ////////////////////////////////비밀번호 확인/////////////////////////////////////////////////////////////////////////
            if ((Pass.ToString() != PPass.ToString()) || Encoding.Default.GetByteCount(Pass) < 4 || Encoding.Default.GetByteCount(Pass) > 20)
            {
                return 1;
            }

            //////////////////////////////////////사원번호 이름 불일치//////////////////////////////////////////////////////////////////////////

            string strSQL = "SELECT * FROM CompanyMember where MemberName = '" + Name + "' and MemberID  = " + ID;
            string strKey = "";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
            {
                return 2;
            }

            /////////////////////////////////////////////////이미 가입된 회원/////////////////////////////////////////////////////////////

            strSQL = "SELECT * FROM SOPGenUser where UserID = '" + ID + "'";
            arrResult.Clear();
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult.Count != 0)
            {
                return 3;
            }

            ///////////////////////////////////////////////회원가입//////////////////////////////////////////////////////////////////

            for (int i = 0; i < key.Length; i++)
            {
                    strKey += key[i];
            }
            String decode = aes.AES_encrypt(Pass.ToString(), strKey); //암호화

            strSQL = "SELECT ID FROM SOPGenUser order by 1 desc"; // GenUser의 ID 값
            arrResult.Clear();
            arrResult = m_dbMgr.GetResultData(strSQL, 0);
            int GenUser_Count = int.Parse(arrResult[0].ToString()) + 1;

            strSQL = "SELECT ID,PositionID FROM CompanyMember where MemberID = " + ID; // GenUser의 MemberID 값
            arrResult.Clear();
            arrResult = m_dbMgr.GetResultData(strSQL, 0);
            string Genuser_MID = arrResult[0].ToString();
            string Genuser_PID = arrResult[1].ToString();

            strSQL = "INSERT INTO SOPGenUser(ID, MemberID, UserLevel, Password, UserID)"
            + " VALUES (" + GenUser_Count + ", " + Genuser_MID + ", " + Genuser_PID + ",'" + decode + "'," + ID + ");"; // ID, MemberID, UserLevel, Password,UserID

            /*string.Format("insert into Decision (ID, x, y, width, height, text, ComponentID, StepMemberID) values ({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7})",
                ++nDecisionID, section.Position.X, section.Position.Y, section.RectSize.Width, section.RectSize.Height, section.Title, data.ComponentID, nStepMemberID);*/

            return m_dbMgr.GetResultData(strSQL, 1) != null ? 0 : 4;
        }

      
       
    }

    class Data_SOPGenUser
    {
        private int m_nID;
        private int m_nMemberID;
        private string m_strUserName;
        private int m_nUserLevel;
        private int m_nTeamID;
        private string m_strPassword;
        private string m_strUserID;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int MemberID
        {
            get { return m_nMemberID; }
            set { m_nMemberID = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

    }
}
