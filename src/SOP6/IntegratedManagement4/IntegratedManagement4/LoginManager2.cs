using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBUtility2;
using System.Collections;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace IntegratedManagement4
{
	public class LoginManager
	{
        public enum CommanderErrorType
        {
            SUCCESS = 0,
            FAIL_DELETE_DAY = 1,
            FAIL_INSERT_DAY,
            FAIL_UPDATE_DAY,
            FAIL_DELETE_NIGHT,
            FAIL_INSERT_NIGHT,
            FAIL_UPDATE_NIGHT
        }

        private static LoginManager m_Instance = null;
		public static LoginManager Instance
		{
			get { return m_Instance; }
		}

		private NetworkWebManager m_NetMgr = null;
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

        // 인자가 없는 생성자는 SimulationMode에서만 사용됨
        public LoginManager()
        {
            m_Instance = this;

            m_strLoginID = "0";
            m_strLoginUserName = "정보없음";
            m_nLoginUserID = 0;
            m_strLoginUserNickName = "정보없음";
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

            IntegratedManagement4.PopupDialog.Chief m_Chief = new IntegratedManagement4.PopupDialog.Chief();

            string strSQL = "SELECT MemberType,MemberID,DisplayText,CallerPhoneNumber "
                                + "FROM SOPGenUserCommander "
                                + "WHERE SOPGenUSerID = " + nUserID;

            string strSQLAdd = " AND DayLight = 0";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL + strSQLAdd);
            if (arrResult != null && arrResult.Count != 0)
            {
                m_Chief.DayLight_Day = true;

                m_Chief.SOPTYPE = (IntegratedManagement4.PopupDialog.SOPTeamType)WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                m_Chief.ID = WebDBManager.GetIntField(arrResult[1].ToString(), -1);
                m_Chief.DisplayText = WebDBManager.GetStringField(arrResult[2].ToString(), "");
                m_Chief.CallerPhoneNumber = WebDBManager.GetStringField(arrResult[3].ToString(), "");
            }

            strSQLAdd = " AND DayLight = 1";
            arrResult.Clear();
            arrResult = m_dbMgr.GetResultData(strSQL + strSQLAdd);
            if (arrResult != null && arrResult.Count != 0)
            {
                m_Chief.DayLight_Night = true;

                m_Chief.SOPTYPE = (IntegratedManagement4.PopupDialog.SOPTeamType)WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                m_Chief.ID = WebDBManager.GetIntField(arrResult[1].ToString(), -1);
                m_Chief.DisplayText = WebDBManager.GetStringField(arrResult[2].ToString(), "");
                m_Chief.CallerPhoneNumber = WebDBManager.GetStringField(arrResult[3].ToString(), "");
            }

			m_frmMain.Invoke((MethodInvoker)delegate
			{
                m_frmMain.SetMode(FormMain.Mode.SUCCESS_LOGIN, m_Chief);
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
					//MessageBox.Show("아이디 또는 비밀번호가 맞지 않습니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    UnE.Utility.UMessageBoxRibbon.Show("아이디 또는 비밀번호가 맞지 않습니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else if (nType == 2)
				{
					//MessageBox.Show("이미 로그인 중인 아이디입니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    UnE.Utility.UMessageBoxRibbon.Show("이미 로그인 중인 아이디입니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				else if (nType == 3)
				{
					//MessageBox.Show("삭제된 사용자이거나 사용할 수 없는 아이디입니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    UnE.Utility.UMessageBoxRibbon.Show("삭제된 사용자이거나 사용할 수 없는 아이디입니다.", "로그인 경고", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				}
				m_frmMain.ClearLoginTextBox();
			});
		}

        public bool JoinUser(int nID, string szMemberID, string szPass, string szNickName, IntegratedManagement4.PopupDialog.Chief chief)
        {
            string strEncrypt = AES256Cipher.AES_encrypt(szPass, key);

            return m_NetMgr.RegisterUser(nID, szMemberID, strEncrypt, szNickName, chief);
        }

		public void OnJoinUser(int nGenUserID)
		{
			if (m_frmMain != null)
			{
				Thread t = new Thread(JoinUserthread);
				t.Start(nGenUserID);
			}
		}

        public void JoinUserthread(object param)
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

        public void OnChangeSOPGenUserCommander(CommanderErrorType nResult)
        {
            if (m_frmMain != null)
            {
                Thread t = new Thread(ChangeSOPGenUserCommanderthread);
                t.Start(nResult);
            }
        }

        public void ChangeSOPGenUserCommanderthread(object param)
        {
            CommanderErrorType nResult = (CommanderErrorType)param;
            m_frmMain.Invoke((MethodInvoker)delegate
            {
                if (nResult == CommanderErrorType.SUCCESS)
                {
                    m_frmMain.SuccessChangeSOPGenUserCommander();
                }
                else
                {
                    m_frmMain.FailChangeSOPGenUserCommander(nResult);
                }
            });
        }

        public bool LogIn(string strID, string strEncrypt, bool isEncryptPass)
        {
            if (isEncryptPass)
            {
                m_strLoginTryID = strID;

                RegUtil.WriteRegValue("Update Info", "LastUser", strID, m_frmMain.SiteID);
                RegUtil.WriteRegValue("Update Info", "LastEncr", strEncrypt, m_frmMain.SiteID);

                return m_NetMgr.LoginUser(strID, strEncrypt);
            }
            return false;
        }

		public bool  LogIn(string strID, string strPassword)
		{		
			string strEncrypt = AES256Cipher.AES_encrypt(strPassword, key);
			
			m_strLoginTryID = strID;

            RegUtil.WriteRegValue("Update Info", "LastUser", strID, m_frmMain.SiteID);
            RegUtil.WriteRegValue("Update Info", "LastEncr", strEncrypt, m_frmMain.SiteID);
            
            return m_NetMgr.LoginUser(strID, strEncrypt);
		}

		public bool SetPassword(string szGenID, string szNewPass)
		{
			string strEncrypt2 = AES256Cipher.AES_encrypt(szNewPass, key);
			return m_NetMgr.SetPassword(szGenID, strEncrypt2);
		}

		public bool ChangePassword(string szPass, string szNewPass)
		{
			string strEncrypt1 = AES256Cipher.AES_encrypt(szPass, key);
			string strEncrypt2 = AES256Cipher.AES_encrypt(szNewPass, key);

			return m_NetMgr.ChangePassword(m_nLoginUserID, strEncrypt1, strEncrypt2);
		}

        public bool ChangeNickName(string szNickName)
        {
            return m_NetMgr.ChangeNickName(m_nLoginUserID, szNickName);
        }

        public bool ChangeSOPGenCommander(IntegratedManagement4.PopupDialog.Chief pchief)
        {
            return m_NetMgr.ChangeSOPGenCommander(LoginUserID,pchief);
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
			if (m_frmMain != null && !m_frmMain.IsDisposed && !m_frmMain.Closing)
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
            //strSQL = "select c.id , j.LevelNo from CompanyMember as c, JobLevel as j where c.MemberID = '{0}' and c.MemberName = '" + strMemberName + "' and c.LevelID = j.ID";

            string strSQL = string.Format("SELECT TeamID FROM Site WHERE ID = {0}", FormMain.Instance.SiteID);
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                return -1;

            int nTeamID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            if (nTeamID == -1)
                return -1;

            ArrayList arrResult2 = GetRegularTeams(nTeamID);
            //strSQL = string.Format("sp_TeamList2 {0}", nTeamID);
            //ArrayList arrResult2 = m_dbMgr.GetStoredProcedureData(strSQL, 0);
            if (arrResult2 == null || arrResult2.Count == 0)
                return -1;

            string szTeamList = "";
            for (int i = 0; i < arrResult2.Count - 2; i += 3)
            {                
                string szTeamID = WebDBManager.GetStringField(arrResult2[i].ToString(), "");
                if (szTeamList != "")
                {
                    szTeamList += ",";
                }
                szTeamList += szTeamID;
            }

            if (szTeamList == "")
            {
                return -1;
            }

            string szText = "select c.id from CompanyMember as c, RegularMemberList as r where c.MemberID = '{0}' and c.MemberName = '{1}' and c.ID = r.CompanyMemberID and r.RegularTeamID in ({2})";
            strSQL = string.Format(szText, strMemberID, strMemberName, szTeamList);			

			ArrayList arrResult3 = m_dbMgr.GetResultData(strSQL);
            if (arrResult3 == null || arrResult3.Count == 0)
				return -1;

            int nID = WebDBManager.GetIntField(arrResult3[0].ToString(), -1);
            /*int nLevel = WebDBManager.GetIntField(arrResult3[1].ToString(), -1);
			if (nLevel == 100)
				return -2;*/
			if (nID <= 0)
				return nID;
            
            szText = "select id, UserID from SOPGenUser where MemberID = {0} and SiteID = {1}";            
			strSQL = string.Format(szText, nID, FormMain.Instance.SiteID);

			arrResult = m_dbMgr.GetResultData(strSQL);

			if (arrResult == null || arrResult.Count < 2)
				return nID;

			int nGenUserID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			strGenUserID = WebDBManager.GetStringField(arrResult[1], "");

			return nGenUserID > 0 ? 0 : nID;
		}

        // nTeamID 및 그 하위에 있는 팀들을 얻어온다.
        private ArrayList GetRegularTeams(int nTeamID)
        {
            string strSQL = "Select ID, TeamName, ParentTeamID from RegularTeam";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                if (id.Data == nTeamID)
                {
                    arrDatas.Add(arrResult[i]);
                    arrDatas.Add(arrResult[i + 1]);
                    arrDatas.Add(arrResult[i + 2]);

                    break;
                }
            }

            AddRegularTeams(nResultCount, nTeamID, arrResult, arrDatas);
            return arrDatas;
        }

        private void AddRegularTeams(int nResultCount, int nParentTeamID, ArrayList src, ArrayList trg)
        {
            List<int> ids = new List<int>();

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> parentTeamID = WebDBManager.GetIntField(src[i + 2].ToString());

                if (parentTeamID == null)
                    continue;

                if (parentTeamID.Data == nParentTeamID)
                {
                    VariousData<int> id = WebDBManager.GetIntField(src[i].ToString());

                    if (id == null)
                        continue;

                    trg.Add(src[i]);
                    trg.Add(src[i + 1]);
                    trg.Add(src[i + 2]);

                    ids.Add(id.Data);
                }
            }

            foreach (int nTeamID in ids)
            {
                AddRegularTeams(nResultCount, nTeamID, src, trg);
            }
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
