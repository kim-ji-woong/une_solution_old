using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;

namespace SDMSServer
{
    public class ClientDataIntegrateManager : ClientData
    {
        private int m_nSiteID = 1;
        public ClientDataIntegrateManager(ServiceProvider provider)
        {
            m_nSiteID = NetworkServer.Instance.SiteID;

            m_provider = provider;
            Type = ClientType.INTEGRATE_MANAGER;
        }

		protected override bool ProcessFirstConnection(ConnectionState state)
		{
			SendCheckLogin(state);
			return true;
		}

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
			if (nHeader == TCP_ID.LOGIN_USER)
			{
                int nGenUserID = -1;
				int nMemberID = -1;
				string szName = "", szNickName = "";
				string szGenUser;
				
				if (LoginUser(bytes, out nGenUserID, out nMemberID, out szName, out szGenUser, out szNickName))
				{
					if (!LoginManager.Instance.IsValidUser(szName, nMemberID))
					{
						SendRejectLogin(state, 3);
						return true;
					}

					if (LoginManager.Instance.IsLoginUser(szGenUser))
					{
						// 중복 로그인 경우
						SendRejectLogin(state, 2);
						return true;
					}

					SendAcceptLogin(state, nGenUserID, szName, szNickName);

					LoginInfo login = new LoginInfo();
                    login.ID = nGenUserID;
					login.LoginTime = DateTime.Now;
					login.MemberID = nMemberID;
					login.SOPGenUserID = szGenUser;
					login.UserName = szName;
                    login.NickName = szNickName;

					LoginManager.Instance.AddUser(state, login);
				}
				else
				{
					// id, pass가 다른 경우
					SendRejectLogin(state, 1);
				}				
			}
			else if (nHeader == TCP_ID.LOGOUT_USER)
			{
				// 로그인 해제
				LoginManager.Instance.RemoveClient(state);
				SendLogout(state);
			}
			else if (nHeader == TCP_ID.CHECK_LOGIN)
			{
                int nGenUserID = -1;
				int nMemberID = -1;
				string szName = "", szNickName = "";
				string szGenUser;
				if (CheckLoginUser(bytes, out nGenUserID, out nMemberID, out szName, out szGenUser, out szNickName))
				{
					if (!LoginManager.Instance.IsValidUser(szName, nMemberID))
					{
						SendRejectLogin(state, 3);
						return true;
					}

					if (LoginManager.Instance.IsLoginUser(szGenUser))
					{
						// 이미 로그인 되어 있으므로 로그아웃 처리
						SendLogout(state);
						return true;
					}

					LoginInfo login = new LoginInfo();
                    login.ID = nGenUserID;
					login.LoginTime = DateTime.Now;
					login.MemberID = nMemberID;
					login.SOPGenUserID = szGenUser;
					login.UserName = szName;
                    login.NickName = szNickName;
					LoginManager.Instance.AddUser(state, login);

				}				
			}
			else if (nHeader == TCP_ID.CHNAGE_PASSWORD)
			{
				if (ChangePassword(state, bytes))
				{
					SendChangePassword(state, 1);
				}
				else
				{
					SendChangePassword(state,0);
				}
			}
			else if (nHeader == TCP_ID.SET_PASSWORD)
			{
				if (SetPassword(state, bytes))
				{
					SendChangePassword(state,1);
				}
				else
				{
					SendChangePassword(state,0);
				}
			}
            else if (nHeader == TCP_ID.CHANGE_NICKNAME)
            {
                string strNickName;

                if (ChangeNickName(state, bytes, out strNickName))
                {
                    SendChangeNickName(state, 1, strNickName);
                }
                else
                {
                    SendChangeNickName(state, 0, strNickName);
                }
            }
			else if (nHeader == TCP_ID.JOIN_USER)
			{
				int nResult = JoinUser(state, bytes);
				SendJoinUser(state, nResult);

			}
            return true;
        }

		private int JoinUser(ConnectionState state, byte[] bytes)
		{
			int nReadData = 6;

			// Read string
			int nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
			nReadData += 4;
			int nMemberID = BitConverter.ToInt32(bytes, nReadData);
			nReadData += 4;

			nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
			nReadData += 4;
			string szMemberID = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
			nReadData += nDataLength;

			nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
			nReadData += 4;
			string szPass = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
			nReadData += nDataLength;

            nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
            nReadData += 4;
            string szNickName = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
            nReadData += nDataLength;


			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

			string strSQL = "select id from SOPGenUser where UserID = '" + szMemberID + "' and SiteID = " + m_nSiteID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null)
			{
				return -1;
			}
				

			if (arrResult.Count > 0)
			{
				int nUserID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);

				if (nUserID > 0)
					return 0;
			}

			strSQL = "select max(id) from SOPGenUser";
			arrResult = dbMgr.GetBatchData(strSQL);

			if (arrResult == null)
			{
				dbMgr.BatchRollback();
				return -1;
			}

			int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
            
			strSQL = string.Format("Insert into SOPGenUser (ID, MemberID, UserLevel, Password, UserID, NickName, SiteID ) values ({0}, {1}, {2}, '{3}', '{4}', '{5}', {6})",
				nID, nMemberID, 2, szPass, szMemberID, szNickName, m_nSiteID);

			if (dbMgr.GetBatchData(strSQL) == null)
			{
				dbMgr.BatchRollback();
				return -1;
			}

			dbMgr.BatchCommit();
			return nID;
		}

		private bool SetPassword(ConnectionState state, byte[] bytes)
		{
			int nReadData = 6;

			// Read string
			int nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
			nReadData += 4;
			string szGenUserID = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
			nReadData += nDataLength;

			nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
			nReadData += 4;
			string szNewPass = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
			nReadData += nDataLength;
			
			string strSQL = string.Format("Update SOPGenUser set Password = '{0}' where UserID = '{1}' and SiteID = {2}",
				szNewPass, szGenUserID, m_nSiteID);
			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
			return dbMgr.GetResultData(strSQL, 0) == null ? false : true;
		}

		private bool ChangePassword(ConnectionState state, byte[] bytes)
		{
			int nReadData = 6;

			// Read int
			int nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
			nReadData += 4;
			int nUserID = BitConverter.ToInt32(bytes, nReadData);
			nReadData += 4;

			// Read string
			nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
			nReadData += 4;
			string szPass1 = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
			nReadData += nDataLength;
			
			nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
			nReadData += 4;
			string szNewPass = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
			nReadData += nDataLength;

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

			string strSQL = "select Password from SOPGenUser where id = " + nUserID + " and SiteID = " + m_nSiteID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

			if (arrResult == null || arrResult.Count == 0)
				return false;

			if (szPass1 != arrResult[0].ToString())
				return false;

            strSQL = string.Format("Update SOPGenUser set Password = '{0}' where id = {1}  and SiteID = {2}",
				szNewPass, nUserID, m_nSiteID);

			return dbMgr.GetResultData(strSQL, 0) == null ? false : true;

		}

        private bool ChangeNickName(ConnectionState state, byte[] bytes, out string szNickName)
        {
            int nReadData = 6;

            // Read int
            int nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
            nReadData += 4;
            int nUserID = BitConverter.ToInt32(bytes, nReadData);
            nReadData += 4;

            // Read string
            nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
            nReadData += 4;
            szNickName = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
            nReadData += nDataLength;

            DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;

            string strSQL = string.Format("Update SOPGenUser set NickName = '{0}' where id = {1} and SiteID = {2}",
                szNickName, nUserID, m_nSiteID);

            return dbMgr.GetResultData(strSQL, 0) == null ? false : true;
        }

		private bool CheckLoginUser(byte[] bytes, out int nID, out int MemberID, out string MemberName, out string szGenUser, out string szNickName)
		{
            nID = -1;
			MemberID = -1;
			MemberName = "";
			szGenUser = "";
            szNickName = "";

			int nReadData = 6;

			// Read string
			int nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
			nReadData += 4;
			string szUserID = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
			nReadData += nDataLength;

			szGenUser = szUserID;

			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
			string szSQL = string.Format("select Password, gen.MemberID, cm.MemberName, gen.NickName, gen.ID from SOPGenUser as gen, CompanyMember as cm where gen.MemberID = cm.ID and UserID='{0}' and gen.SiteID = {1}", szUserID, m_nSiteID);
			ArrayList arResult = dbMgr.GetResultData(szSQL, 0);
			if (arResult == null || arResult.Count == 0)
				return false;

			if (arResult.Count != 5)
				return false;

			int.TryParse(arResult[1].ToString(), out MemberID);
			MemberName = arResult[2].ToString();
            szNickName = DBUtility.WebDBManager.GetStringField(arResult[3], "");

            if (string.Compare(szNickName, "null", true) == 0)
                szNickName = "";

            nID = DBUtility.WebDBManager.GetIntField(arResult[4].ToString(), -1);
			return true;			
		}

		private bool LoginUser(byte[] bytes, out int nID, out int MemberID, out string MemberName, out string szGenUser, out string szNickName)
		{
            nID = -1;
			MemberID = -1;
			MemberName = "";
			szGenUser = "";
            szNickName = "";

			int nReadData = 6;

			// Read string
			int nDataLength = BitConverter.ToInt32(bytes, ++nReadData);
			nReadData += 4;
			string szUserID = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
			nReadData += nDataLength;
			
			szGenUser = szUserID;
			
			// read string
			nDataLength = BitConverter.ToInt32(bytes, ++nReadData); 
			nReadData += 4;
			string szPass = Encoding.UTF8.GetString(bytes, nReadData, nDataLength);
			
			DBUtility.WebDBManager dbMgr = NetworkServer.Instance.DBManager;
			string szSQL = string.Format("select Password, gen.MemberID, cm.MemberName, gen.NickName, gen.ID from SOPGenUser as gen, CompanyMember as cm where gen.MemberID = cm.ID and UserID='{0}' and gen.SiteID = {1}", szUserID, m_nSiteID);
			ArrayList arResult = dbMgr.GetResultData(szSQL, 0);
			if (arResult == null || arResult.Count == 0)
				return false;

			if (arResult.Count != 5)
				return false;

			if (szPass.Equals(arResult[0].ToString()))
			{
				int.TryParse(arResult[1].ToString(), out MemberID);
				MemberName = arResult[2].ToString();
                szNickName = DBUtility.WebDBManager.GetStringField(arResult[3], "");

                if (string.Compare(szNickName, "null", true) == 0)
                    szNickName = "";

                nID = DBUtility.WebDBManager.GetIntField(arResult[4].ToString(), -1);
				return true;
			}

			return false;
		}

        private void SendSuccessMessage(ConnectionState state, int nSuccess, short nTag, byte[] addBytes = null)
        {
            int nChunkCount = addBytes == null ? 1 : 2;

            byte[] data = ServiceProvider.MakeBytes(nSuccess);

            byte[] bytes;
            
            if (nChunkCount == 1)
                bytes = new byte[6 + data.Length];
            else
                bytes = new byte[6 + data.Length + addBytes.Length];

            byte[] nHader = BitConverter.GetBytes(nTag);
            byte[] nCount = BitConverter.GetBytes(nChunkCount);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(data, 0, bytes, 6, data.Length);

            if (nChunkCount == 2)
                System.Buffer.BlockCopy(addBytes, 0, bytes, 6 + data.Length, addBytes.Length);

            try
            {
                m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendSuccessMessage", ex);       	        
            }
            
        }

		public void SendChangePassword(ConnectionState state, int nSuccess)
		{
            SendSuccessMessage(state, nSuccess, (short)TCP_ID.CHNAGE_PASSWORD);
		}

        public void SendChangeNickName(ConnectionState state, int nSuccess, string strNickName)
        {
            SendSuccessMessage(state, nSuccess, (short)TCP_ID.CHANGE_NICKNAME, ServiceProvider.MakeBytes(strNickName));
        }

		private void SendCheckLogin(ConnectionState state)
		{
			byte[] bytes = new byte[6];
			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.CHECK_LOGIN);
			byte[] nCount = BitConverter.GetBytes(0);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

            try
            {
                m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendCheckLogin", ex);  
            }
		}

		private void SendJoinUser(ConnectionState state, int nType)
		{
			byte[] data = ServiceProvider.MakeBytes(nType);

			byte[] bytes = new byte[6 + data.Length];
			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.JOIN_USER);
			byte[] nCount = BitConverter.GetBytes(1);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			System.Buffer.BlockCopy(data, 0, bytes, 6, data.Length);

            try
            {
                m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendJoinUser", ex);  
            }
		}

		private void SendRejectLogin(ConnectionState state, int nType)
		{
			byte[] data = ServiceProvider.MakeBytes(nType);

			byte[] bytes = new byte[6 + data.Length];
			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.REJECT_LOGIN);
			byte[] nCount = BitConverter.GetBytes(1);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			System.Buffer.BlockCopy(data, 0, bytes, 6, data.Length);

            try
            {
                m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendRejectLogin", ex);  
            }
		}

		private void SendLogout(ConnectionState state)
		{
			byte[] bytes = new byte[6];
			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.LOGOUT_USER);
			byte[] nCount = BitConverter.GetBytes(0);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

            try
            {
                m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendLogout", ex);  
            }
		}

		private void SendAcceptLogin(ConnectionState state, int nMemberID, string szName, string szNickName)
		{
			byte[] data1 = ServiceProvider.MakeBytes(nMemberID);
			byte[] data2 = ServiceProvider.MakeBytes(szName);
            byte[] data3 = ServiceProvider.MakeBytes(szNickName);

			byte[] bytes = new byte[6 + data1.Length + data2.Length + data3.Length];
			byte[] nHader = BitConverter.GetBytes((short)TCP_ID.ACCEPT_LOGIN);
			byte[] nCount = BitConverter.GetBytes(3);

			// SET MESSAGE HeADER
			bytes[0] = nHader[0];
			bytes[1] = nHader[1];

			// SET DATA COUNT
			bytes[2] = nCount[0];
			bytes[3] = nCount[1];
			bytes[4] = nCount[2];
			bytes[5] = nCount[3];

			System.Buffer.BlockCopy(data1, 0, bytes, 6, data1.Length);
			System.Buffer.BlockCopy(data2, 0, bytes, 6 + data1.Length, data2.Length);
            System.Buffer.BlockCopy(data3, 0, bytes, 6 + data1.Length + data2.Length, data3.Length);

            try
            {
                m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                ConnectionLogEx.Instance.WriteLine("SendAcceptLogin", ex); 
            }
		}
    }
}
