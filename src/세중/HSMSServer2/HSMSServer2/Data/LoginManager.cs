using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TcpLib2;

namespace HSMSServer2
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

		public LoginManager()
		{
		}

        public void RemoveUser(string szUserName)
        {
            ConnectionState state = null;
            foreach (KeyValuePair<ConnectionState, LoginInfo> pair in m_logInUsers)
            {
                if (pair.Value.UserID.Equals(szUserName))
                {
                    state = pair.Key;
                    break;
                }                    
            }
            if (state != null)
                RemoveClient(state);
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

		public bool IsLoginUser(string szUserID)
		{
            bool bResult = false;
            ArrayList arDelete = new ArrayList();
			foreach (KeyValuePair<ConnectionState, LoginInfo> pair in m_logInUsers)
			{
                if (pair.Value.UserID == szUserID)
                {
                    ConnectionState state = pair.Key;
                    if( state != null && state.Connected == true)
                    {
                        bResult = true;
                    } 
                    else
                    {
                        arDelete.Add(state);
                    }
                }
			}
            
            foreach(ConnectionState state in arDelete)
            {
                RemoveClient(state);
            }
            return bResult;
		}

		public bool IsValidUser(int nUserID, int nSiteID)
		{
            HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
            return DBLoginHelper.IsValidUser(dbMgr, nUserID, nSiteID);
		}

        public LoginInfo FindLoginUser(int nUserID)
        {
            HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
            return DBLoginHelper.FindLoginUser(dbMgr, nUserID);
        }
       
        public ConnectionState GetLoginUser(string strUserID)
        {
            foreach (KeyValuePair<ConnectionState, LoginInfo> pair in m_logInUsers)
            {
                if (pair.Value.UserID.Equals(strUserID))
                {
                    ConnectionState state = pair.Key;
                    if (state != null && state.Connected == true)
                        return state;
                    else
                        break;
                }
            }

            return null;
        }
	}	
}
