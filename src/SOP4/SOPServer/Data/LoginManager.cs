using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TcpLib2;

namespace SDMSServer
{
	public class LoginManager
	{
		private static LoginManager m_instance = null;
		public static LoginManager Instance
		{
			get 
			{	
				if (m_instance == null)
					m_instance = new LoginManager();
				return m_instance; 
			}
			set { m_instance = value; }
		}

		private Dictionary<ConnectionState, LoginInfo> m_logInUsers = new Dictionary<ConnectionState, LoginInfo>();

        private int m_nSiteID = 1;
		protected LoginManager()
		{
            m_nSiteID = NetworkServer.Instance.SiteID;
		}

		public void RemoveClient(ConnectionState state)
		{
			if (m_logInUsers.ContainsKey(state))
			{
				m_logInUsers.Remove(state);
			}
		}

		public void AddUser(ConnectionState state, LoginInfo login)
		{

			if (!m_logInUsers.ContainsKey(state))
			{
				m_logInUsers.Add(state, login);
			}
		
		}

		public bool IsLoginUser(string szGenUserID)
		{
			foreach (KeyValuePair<ConnectionState, LoginInfo> pair in m_logInUsers)
			{
				if (pair.Value.SOPGenUserID.Equals(szGenUserID))
					return true;
			}
			return false;
		}

		public bool IsValidUser(string szUserName, int nMemberID)
		{
            if (nMemberID < 0)
                return szUserName == LoginInfo.UNKNOWN_USER;

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
			string strSQL = "select ID from CompanyMember where ID = " + nMemberID.ToString() + " and MemberName = '" + szUserName + "'";

			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);
			if (arrResult == null || arrResult.Count == 0)
				return false;
			
			//int nLevel = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			//if (nLevel == 100)
			//	return false;
			return true;
		}

        public LoginInfo FindLoginUser(int nID, string strUserName)
        {
            string strSQL = string.Format("Select MemberID, UserID, NickName from SOPGenUser where ID = {0} and SiteID = {1}",
                nID, m_nSiteID);

            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count != 3)
                return null;

            int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            string strUserID = DBUtility.WebDBManager.GetStringField(arrResult[1].ToString(), "");
            string strUserNickName = DBUtility.WebDBManager.GetStringField(arrResult[2].ToString(), "");

            if (nMemberID < 0)
            {
                if (strUserID == "" || strUserNickName == "")
                    return null;

                //if (strUserName != LoginInfo.UNKNOWN_USER)
                //    return null;

            }
            else
            {
                strSQL = "Select MemberName from CompanyMember where ID = " + nMemberID.ToString();
                arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count != 1)
                    return null;

                string strName = DBUtility.WebDBManager.GetStringField(arrResult[0].ToString(), "");

                if (strUserName != strName)
                    return null;
            }

            /*string strSQL = "select sg.MemberID, sg.UserID, sg.NickName from SOPGenUser as sg, CompanyMember as cm ";
            strSQL += " where sg.SiteID = " + m_nSiteID.ToString() + " and sg.MemberID = cm.ID and sg.ID = " + nID.ToString();
            strSQL += " and cm.MemberName = '" + strUserName + "'";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (nResultCount < 3)
                return null;

            int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            string strUserID = DBUtility.WebDBManager.GetStringField(arrResult[1].ToString(), "");
            string strUserNickName = DBUtility.WebDBManager.GetStringField(arrResult[2].ToString(), "");*/

            LoginInfo info = new LoginInfo();
            info.ID = nID;
            info.UserName = strUserName;
            info.MemberID = nMemberID;
            info.NickName = strUserNickName;
            info.SOPGenUserID = strUserID;
            return info;
        }

		/*public LoginInfo FindLoginUser(int nGenUserID)
		{
			if (nGenUserID < 0)
				return null;

			string strSQL = "select sg.ID, sg.UserID, sg.NickName, cm.MemberName from SOPGenUser as sg, CompanyMember as cm ";
			strSQL += " where sg.MemberID = cm.ID and sg.MemberID = '" + nGenUserID.ToString() + "'";
            strSQL += " and sg.SiteID = " + m_nSiteID.ToString();

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
				return null;

			int nResultCount = arrResult.Count;

			if (nResultCount < 4)
				return null;

			int nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
			string strUserID = DBUtility.WebDBManager.GetStringField(arrResult[1].ToString(),"");
			string strUserNickName = DBUtility.WebDBManager.GetStringField(arrResult[2].ToString(), "");
			string strUserName = DBUtility.WebDBManager.GetStringField(arrResult[3].ToString(), "");

			if (strUserNickName == null || strUserNickName == "null")
				strUserNickName = "";

			LoginInfo info = new LoginInfo();

			info.ID = nID;
			info.UserName = strUserName;
			info.MemberID = nGenUserID;
			info.NickName = strUserNickName;
			info.SOPGenUserID = strUserID;
			return info;
		}*/

        public LoginInfo FindLoginUser(string strUserID)
        {
            string strSQL = string.Format("select ID, MemberID, NickName from SOPGenUser where UserID = '{0}' and SiteID = {1}",
                strUserID, m_nSiteID);

            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count < 3)
                return null;

            int nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), -1);
            string strUserNickName = DBUtility.WebDBManager.GetStringField(arrResult[2].ToString(), "");

            if (nID < 0)
                return null;

            string strUserName = LoginInfo.UNKNOWN_USER;

            if (nMemberID >= 0)
            {
                strSQL = "Select MemberName from CompanyMember where ID = " + nMemberID.ToString();
                arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count != 1)
                    return null;

                strUserName = DBUtility.WebDBManager.GetStringField(arrResult[0].ToString(), "");
            }

            /*string strSQL = "select sg.ID, sg.MemberID, sg.NickName, cm.MemberName from SOPGenUser as sg, CompanyMember as cm ";
            strSQL += "where sg.MemberID = cm.ID and sg.UserID = '" + strUserID + "'";
            strSQL += " and sg.SiteID = " + m_nSiteID.ToString();

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (nResultCount < 4)
                return null;

            int nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            int nMemberID = DBUtility.WebDBManager.GetIntField(arrResult[1].ToString(), -1);
            string strUserNickName = DBUtility.WebDBManager.GetStringField(arrResult[2].ToString(), "");
            string strUserName = DBUtility.WebDBManager.GetStringField(arrResult[3].ToString(), "");*/

            LoginInfo info = new LoginInfo();
            info.ID = nID;
            info.UserName = strUserName;
            info.MemberID = nMemberID;
            info.NickName = strUserNickName;
            info.SOPGenUserID = strUserID;
            return info;            
        }
	}

	public class LoginInfo
	{
        public const string UNKNOWN_USER = "알수없음";

        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

		private string m_szGenUserID = "";
		public string SOPGenUserID
		{
			get { return m_szGenUserID; }
			set { m_szGenUserID = value; }
		}

		private int m_nMemberID = -1;
		public int MemberID
		{
			get { return m_nMemberID; }
			set { m_nMemberID = value; }
		}

		private string m_szUserName = "";
		public string UserName
		{
			get { return m_szUserName; }
			set { m_szUserName = value; }
		}

        private string m_szNickName = "";
        public string NickName
        {
            get { return m_szNickName; }
            set { m_szNickName = value; }
        }

		DateTime m_dtLoginTime;
		public System.DateTime LoginTime
		{
			get { return m_dtLoginTime; }
			set { m_dtLoginTime = value; }
		}
	}
}
