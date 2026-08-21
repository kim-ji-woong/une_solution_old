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
		private static LoginManager m_Instance = null;
		public static LoginManager Instance
		{
			get { return m_Instance; }
		}

		private NetworkManager m_NetMgr = null;
		private WebDBManager m_dbMgr = null;
		private FormMain m_frmMain = null;
		private VData<bool> m_threadIsAlive = null;

		private int m_nLoginUserID = -1;
		private string m_strLoginID = "";
		private string m_strLoginTryID = "";
		private string m_strLoginUserName = "";
        private string m_strLoginUserNickName = "";

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

        public string LoginUserNickName
        {
            get { return m_strLoginUserNickName; }
        }

		private bool m_bLoginState = false;
		public bool LoginState
		{
			get { return m_bLoginState; }
			set { m_bLoginState = value; }
		}

		private string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

		public LoginManager(WebDBManager dbMgr, FormMain frmMain)
		{
			m_Instance = this;

			m_dbMgr = dbMgr;
			m_frmMain = frmMain;
			m_NetMgr = frmMain.NetManager;
		}

		public void OnEndRestore()
		{
			ProcessManager.Instance.RestartAllProcess();
		}		

		public void OnCheckLogin()
		{
			if (m_bLoginState == true)
			{
				m_NetMgr.CheckLogin(m_strLoginID);
			}
		}

		public void OnAcceptLogin(int nUserID, string szUserName, string szNickName)
		{
			m_strLoginID = m_strLoginTryID;
			m_nLoginUserID = nUserID;
			m_strLoginUserName = szUserName;
            m_strLoginUserNickName = szNickName;
			m_bLoginState = true;

			m_frmMain.Invoke((MethodInvoker)delegate
			{
				m_frmMain.SetMode(FormMain.Mode.SUCCESS_LOGIN);
			});
		}

		public void OnRejectLogin(int nType)
		{
			if (m_frmMain != null)
			{
				Thread t = new Thread(RejectLoginThread);
				t.Start(nType);
			}	
		}
		
		private void RejectLoginThread(object param)
		{
			int nType = (int)param;
			m_frmMain.Invoke((MethodInvoker)delegate
			{
				if (nType == 1)
				{
					MessageBox.Show("아이디 또는 비밀번호가 맞지 않습니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else if (nType == 2)
				{
					MessageBox.Show("이미 로그인 중인 아이디입니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else if (nType == 3)
				{
					MessageBox.Show("삭제된 사용자이거나 사용할 수 없는 아이디입니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);		
				}
				m_frmMain.ClearLoginTextBox();
			});
		}

		public bool JoinUser(int nID, string szMemberID, string szPass, string szNickName)
		{
			string strEncrypt = DBUtility.AES256Cipher.AES_encrypt(szPass, key);

			return m_NetMgr.RegisterUser(nID, szMemberID, strEncrypt, szNickName);
		}

		public void OnJoinUser(int nGenUserID)
		{
			if (m_frmMain != null)
			{
				Thread t = new Thread(JoinUserthread);
				t.Start(nGenUserID);
			}
		}
		
		private void JoinUserthread(object param)
		{
			int nGenUserID = (int)param;
			m_frmMain.Invoke((MethodInvoker)delegate
			{
				if (nGenUserID > 0)
				{
					m_frmMain.SuccessRegisterUser();
				}
				else
				{
					m_frmMain.FailRegisterUser(nGenUserID);
				}
			});
		}

        public bool LogIn(string strID, string strEncrypt, bool isEncryptPass)
        {
            if (isEncryptPass)
            {
                m_strLoginTryID = strID;

                DBUtility.RegUtil.WriteRegValue("Update Info", "LastUser", strID);
                DBUtility.RegUtil.WriteRegValue("Update Info", "LastEncr", strEncrypt);

                return m_NetMgr.LoginUser(strID, strEncrypt);
            }
            return false;
        }

		public bool  LogIn(string strID, string strPassword)
		{		
			string strEncrypt = DBUtility.AES256Cipher.AES_encrypt(strPassword, key);
			
			m_strLoginTryID = strID;

            DBUtility.RegUtil.WriteRegValue("Update Info", "LastUser", strID);
            DBUtility.RegUtil.WriteRegValue("Update Info", "LastEncr", strEncrypt);
            
            return m_NetMgr.LoginUser(strID, strEncrypt);

		}

		public bool SetPassword(string szGenID, string szNewPass)
		{
			string strEncrypt2 = DBUtility.AES256Cipher.AES_encrypt(szNewPass, key);
			return m_NetMgr.SetPassword(szGenID, strEncrypt2);
		}

		public bool ChangePassword(string szPass, string szNewPass)
		{
			string strEncrypt1 = DBUtility.AES256Cipher.AES_encrypt(szPass, key);
			string strEncrypt2 = DBUtility.AES256Cipher.AES_encrypt(szNewPass, key);

			return m_NetMgr.ChangePassword(m_nLoginUserID, strEncrypt1, strEncrypt2);
		}

        public bool ChangeNickName(string szNickName)
        {
            return m_NetMgr.ChangeNickName(m_nLoginUserID, szNickName);
        }

		public void OnChangePassword(int nSuccess)
		{
			if (m_frmMain != null)
			{
				Thread t = new Thread(ChangePasswordThread);
				t.Start(nSuccess);
			}
		}

		private void ChangePasswordThread(object param)
		{
            int nSuccess = (int)param;
			m_frmMain.Invoke((MethodInvoker)delegate
			{				
				if (nSuccess > 0)
				{
					if (LoginManager.Instance.LoginState == false)
					{
						m_frmMain.SuccessChangePassword();
					}
					else
					{
						m_frmMain.SuccessChangePassword2();
					}
				}
				else
				{
					m_frmMain.FailChangePassword();
				}
			});
		}

        public void OnChangeNickName(int nSuccess, string szNickName)
        {
            if (m_frmMain != null)
            {
                Thread t = new Thread(ChangeNickNameThread);
                t.Start(nSuccess.ToString() + "_" + szNickName);
            }
        }

        private void ChangeNickNameThread(object param)
        {

            string strParam = param.ToString();
            int nIndex = strParam.IndexOf('_');

            if (nIndex < 0)
                return;

            string strSuccess = strParam.Substring(0, nIndex);
            string strNickName = strParam.Substring(nIndex + 1);

            int nSuccess;

            if (!int.TryParse(strSuccess, out nSuccess))
                return;

            m_frmMain.Invoke((MethodInvoker)delegate
            {
                if (nSuccess > 0)
                {
                    if (LoginManager.Instance.LoginState == false)
                    {
                        m_frmMain.SuccessChangeNickName();
                    }
                    else
                    {
                        m_strLoginUserNickName = strNickName;
                        m_frmMain.SuccessChangeNickName2();
                    }
                }
                else
                {
                    m_frmMain.FailChangeNickName();
                }
            });
        }
		
		public bool LogOut()
		{
			m_bLoginState = false;
			return m_NetMgr.Logout(m_strLoginID);
		}

		public void OnLogout()
		{
			m_bLoginState = false;
			if (m_frmMain != null && !m_frmMain.IsDisposed)
			{
                try
                {
                    m_frmMain.Invoke((MethodInvoker)delegate
                    {
                        m_frmMain.SetLogout();
                    });
                }
                catch (System.Exception ex)
                {
                	
                }
				
			}
		}

		// Return 값 : 0보다 작은 경우(strMemberID, strMemebrName에 해당하는 정규 직원이 존재하지 않음)
		//             0일 경우(이미 회원가입이 되어 있음)
		//             0보다 클 경우(strMemberID, strMemberName에 해당하는 CompanyMember ID)
		public int GetMemberID(string strMemberID, string strMemberName, ref string strGenUserID)
		{
			//string strSQL = "select id from CompanyMember where MemberID = '" + strMemberID + "' and MemberName = '" + strMemberName + "'";
			string strSQL = "select c.id , j.LevelNo from CompanyMember as c, JobLevel as j where c.MemberID = '" + strMemberID + "' and c.MemberName = '" + strMemberName + "' and c.LevelID = j.ID";

			ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null || arrResult.Count == 0)
				return -1;

			int nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			int nLevel = WebDBManager.GetIntField(arrResult[1].ToString(), -1);
			if (nLevel == 100)
				return -2;
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

    /*
     
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
 
    */

}
