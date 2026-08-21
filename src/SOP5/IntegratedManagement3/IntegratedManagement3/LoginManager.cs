using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility;
using System.Collections;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace IntegratedManagement2
{
    public class LoginManager
    {
        private WebDBManagerEx m_dbMgr = null; 
        private FormMain m_frmMain = null;
        private VData<bool> m_threadIsAlive = null;

        private int m_nLoginUserID = -1;
        private string m_strLoginID = "";
        private string m_strLoginTryID = "";
        private string m_strLoginUserName = "";

        public string LoginID
        {
            get { return m_strLoginID; }
        }

        public string LoginUserName
        {
            get { return m_strLoginUserName; }
        }

        public int LoginUserID
        {
            get { return m_nLoginUserID; }
        }

        private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public LoginManager(WebDBManagerEx dbMgr, FormMain frmMain)
        {
            m_dbMgr = dbMgr;
            m_frmMain = frmMain;
        }

        public bool LogIn(string strID, string strPassword)
        {
			if (strID == "" || strPassword == "")
			{
				MessageBox.Show("아이디와 비밀번호를 입력하세요.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}

            ArrayList arr = new ArrayList();

            bool loginResult = false;
            m_strLoginTryID = strID;

            if (CheckLoginUser(strID, "Check"))
            {
                if (bFail == false)
                    MessageBox.Show("이미 로그인 중인 아이디입니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                loginResult = CheckLoginUser(strID, "Login");
                
            }

            int nLevel = GetUserID(strID, strPassword);

            if (nLevel == -1)
            {
                MessageBox.Show("아이디 또는 비밀번호가 맞지 않습니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                if (loginResult)
                {
                    CheckLogoutUser(strID);
                }

                return false;
            }

			RunThread();
            bLoginState = true;
            m_frmMain.SetMode(FormMain.Mode.SUCCESS_LOGIN);

            return true;
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
                    String decode = AES256Cipher.AES_encrypt(strPassword, strKey); //암호화
                    //String decode = aes.AES_decrypt(dataUser.Password, strKey); //복호화

                    decode = decode.Replace("+", " ");

                    if (dataUser.Password.ToString() == decode.ToString())
                    //if (decode == textBoxPassword.Text)
                    {
                        nLevel = dataUser.UserLevel;
                        m_strLoginUserName = dataUser.UserName;
                        m_nLoginUserID = dataUser.ID;
                        m_strLoginID = strID;
                        break;
                    }
                }
            }
            return nLevel;
        }

        private void ReadDB_TableUsers(ref ArrayList arrUser)
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
                dataNew.ID = WebDBManager.GetIntField(arrResult[i].ToString(), 0);
                dataNew.MemberID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), 0);
                dataNew.UserName = WebDBManager.GetStringField(arrResult[i + 2].ToString(), "");
                dataNew.UserLevel = WebDBManager.GetIntField(arrResult[i + 3].ToString(), 0);
                dataNew.TeamID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), 0);
                dataNew.Password = WebDBManager.GetStringField(arrResult[i + 5].ToString(), "");
                dataNew.UserID = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                arrUser.Add(dataNew);
            }
        }

        private void RunThread()
        {
            if (m_threadIsAlive == null || m_threadIsAlive.Data == false)
            {
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(CheckLoginThread));
                t.Start(this);
            }
        }

        public static void CheckLoginThread(object param)
        {
            LoginManager mgr = (LoginManager)param;

            mgr.m_threadIsAlive = new VData<bool>();
            mgr.m_threadIsAlive.Data = true;

            VData<bool> continueThread = mgr.m_threadIsAlive;

            while (mgr.m_nLoginUserID > 0 && continueThread.Data)
            {
                mgr.CheckLoginUser(mgr.m_strLoginTryID, "Check");
                Thread.Sleep(10000);
            }

            continueThread.Data = false;
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

            if (m_threadIsAlive != null)
            {
                m_threadIsAlive.Data = false;
                m_threadIsAlive = null;
            }
            
            bool bResult = m_dbMgr.GetResultCheckUser(szUserID, "Logout", ref bFail);
            bLoginState = false;
            return bResult;
        }

        public bool LogOut()
        {
            return CheckLogoutUser(m_strLoginID);
        }

        // Return 값 : 0보다 작은 경우(strMemberID, strMemebrName에 해당하는 정규 직원이 존재하지 않음)
        //             0일 경우(이미 회원가입이 되어 있음)
        //             0보다 클 경우(strMemberID, strMemberName에 해당하는 CompanyMember ID)
        public int GetMemberID(string strMemberID, string strMemberName, ref string strGenUserID)
        {
            string strSQL = "select id from CompanyMember where MemberID = '" + strMemberID + "' and MemberName = '" + strMemberName + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            if (nID <= 0)
                return nID;

            strSQL = "select id, UserID from SOPGenUser where MemberID = " + nID.ToString();
            arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count < 2)
                return nID;

            int nGenUserID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            strGenUserID = WebDBManager.GetStringField(arrResult[1], "");

            return nGenUserID > 0 ? 0 : nID;
        }

        // Return 값 : 새로운 회원가입에 성공하면 0보다 큰 회원 ID를 리턴하고,
        //             실패하면 0보다 작은 값을 리턴한다.
        //             이미 존재하는 ID일 경우 0을 리턴한다.
		public int RegisterUser(int nCompanyMemberID, string strID, string strPassword)
        {
            string strSQL = "select id from SOPGenUser where UserID = '" + strID + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            if (arrResult.Count > 0)
            {
                int nUserID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

                if (nUserID > 0)
                    return 0;
            }

            strSQL = "select max(id) from SOPGenUser";
            arrResult = m_dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                m_dbMgr.BatchRollback();
                return -1;
            }

            int nID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            string strEncPassword = AES256Cipher.AES_encrypt(strPassword, key);

            strSQL = string.Format("Insert into SOPGenUser (ID, MemberID, UserLevel, Password, UserID) values ({0}, {1}, {2}, '{3}', '{4}')",
                nID, nCompanyMemberID, 2, strEncPassword, strID);

            if (m_dbMgr.GetBatchData(strSQL) == null)
            {
                m_dbMgr.BatchRollback();
                return -1;
            }

            m_dbMgr.BatchCommit();
            return nID;
        }

        public bool ChangePassword(string strGenUserID, string strPassword)
        {
            string strEncrypt = AES256Cipher.AES_encrypt(strPassword, key);

            string strSQL = string.Format("Update SOPGenUser set Password = '{0}' where UserID = '{1}'",
                strEncrypt, strGenUserID);

            return m_dbMgr.GetResultData(strSQL, 0) == null ? false : true;
        }

        // Return 값 : 0보다 크면 비밀번호 변경 성공
        //             0보다 작으면 비밀번호 변경 실패
        //             0이면 현재 비밀번호가 일치하지 않음
        public int ChangePassword2(string strCurrentPassword, string strNewPassword)
        {
            string strEncryptCurrent = AES256Cipher.AES_encrypt(strCurrentPassword, key);

            string strSQL = "select Password from SOPGenUser where id = " + m_nLoginUserID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            if (strEncryptCurrent != arrResult[0].ToString())
                return 0;

            string strEncryptNew = AES256Cipher.AES_encrypt(strNewPassword, key);

            strSQL = string.Format("Update SOPGenUser set Password = '{0}' where id = {1}",
                strEncryptNew, m_nLoginUserID);

            return m_dbMgr.GetResultData(strSQL, 0) == null ? -1 : 1;
        }
    }

    public class WebDBManagerEx : WebDBManager
    {
        private CookieContainer cookieContainer = new CookieContainer();

        public bool GetResultCheckUser(string userID, string szCmd, ref bool bFail)
        {
            bFail = false;
            ArrayList arrResult = new ArrayList();
            string resResult = GetCheckUser(userID, szCmd);
            if (resResult == "connection_errer")
            {
                bFail = true;
                return true;
            }

            //m_StringFile.SetData(resResult);
            StringFile strFile = new StringFile(resResult);

            string strResult = "";
            bool isResult = true;
            bool isBegin = false;
            while (isResult)
            {
                //isResult = m_StringFile.ReadLine(ref strResult);
                isResult = strFile.ReadLine(ref strResult);

                if (isResult)
                {
                    if (strResult == "Begin Data")
                    {
                        isBegin = true;
                        continue;
                    }
                    if (strResult == "End Data")
                        break;
                    if (isBegin)
                    {
                        if (strResult == "0")
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        public string GetCheckUser(string userID, string szCmd)
        {
            string resResult = string.Empty;
            string sourceUrl = WebServerURL + "/LoginUsers.jsp";

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(userID);
            string strUrlEncode = URLEncoding(bytes1);
            string postData = "User=" + strUrlEncode + "&" + "Cmd=" + szCmd;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);
            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.CookieContainer = cookieContainer;
            wReq.Method = "POST";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;

            try
            {
                using (Stream writeStream = wReq.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

                // http 내용 추출
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

                resResult = readerPost.ReadToEnd();
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show(e.Message);
                return "connection_errer";
            }
            return resResult;
        }
    }

    public class VData<Type>
    {
        private Type m_data;

        public Type Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
    }

    public class Data_SOPGenUser
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
