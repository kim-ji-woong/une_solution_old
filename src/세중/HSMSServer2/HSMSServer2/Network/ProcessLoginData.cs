using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using HSMS;


namespace HSMSServer2
{
    public class ProcessLogin
    {
        private ServiceProvider m_provider = null;

        public ProcessLogin(ServiceProvider provider)
        {
            m_provider = provider;
        }

        public bool ProcessFirstConnection(ConnectionState state, int nUserID, string szUserID)
        {
            if (szUserID == null || szUserID.Length == 0)
                return false;

            if (LoginManager.Instance.IsLoginUser(szUserID))
            {
                // 이미 로그인 되어 있으므로 로그아웃 처리
                System.Diagnostics.Trace.WriteLine("이미 로그인된 사용자");
                SendLogout(state);
                return false;
            }

            LoginInfo login = LoginManager.Instance.FindLoginUser(nUserID);

            if (login == null)
            {
                System.Diagnostics.Trace.WriteLine(szUserID + " 는 잘못된 계정입니다.");
                return false;
            }

            System.Diagnostics.Trace.WriteLine(szUserID + " 로그인 처리 되었음");
            LoginManager.Instance.AddUser(state, login);

            return true;
        }

        // bytes는 length byte가 제거되었음
        public bool ProcessLoginData(ConnectionState state, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.CHECK_LOGIN)
            {
                int nID = -1;
                string szUserID = "";
                int nSiteID;
                bool isLogin;

                if (CheckLoginUser(arrDatas, out nID, out szUserID, out nSiteID, out isLogin) == LoginUserResult.SUCCESS)               
                {
                    if (!LoginManager.Instance.IsValidUser(nID, nSiteID))
                    {
                        SendRejectLogin(state, LoginUserResult.INVALID_ID);
                        return true;
                    }

                    if (LoginManager.Instance.IsLoginUser(szUserID))
                    {
                        // 이미 로그인 되어 있으므로 로그아웃 처리
                        SendLogout(state);
                        return true;
                    }

                    LoginInfo login = LoginManager.Instance.FindLoginUser(nID);                    
                    LoginManager.Instance.AddUser(state, login);
                }
            }
            else if (nHeader == TCP_ID.CHNAGE_PASSWORD)
            {
                ChangePasswordResult result = ChangePassword(state, arrDatas);
                SendChangePassword(state, result);
            }
            /*else if (nHeader == TCP_ID.SET_PASSWORD)
            {
                if (SetPassword(state, arrDatas))
                {
                    SendChangePassword(state, 1);
                }
                else
                {
                    SendChangePassword(state, 0);
                }
            }*/
            else if (nHeader == TCP_ID.JOIN_USER)
            {
                int nResult = (int)JoinUser(state, arrDatas);
                SendJoinUser(state, nResult);

            }
            /*else if (nHeader == TCP_ID.REQUEST_CODE)
            {
                string nResult = RequestCode(state, arrDatas);
                SendRequestCode(state, nResult);

            }*/
            else if(nHeader == TCP_ID.DELETE_USER)
            {
                string strUserID;
                DeleteUserResult result = DeleteUser(state, arrDatas, out strUserID);
                SendDeleteUser(state, result, strUserID);
            }
            return true;
        }

        private DeleteUserResult DeleteUser(ConnectionState state, ArrayList arDatas, out string szUserID)
        {
            szUserID = "";

            if (arDatas.Count < 3)
                return DeleteUserResult.NEED_MORE_DATA;

            szUserID = (string)arDatas[0];
            string szPasswd = (string)arDatas[1];
            int nSiteID = (int)arDatas[2];

            try
            {
                HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
                string szSQL = string.Format("select password, id_name, id_key from LoginUser where id_name = '{0}' and Site_ID = {1}", szUserID, nSiteID);

                ArrayList arResult = DBHelper.GetResultData(dbMgr, szSQL);
                if (arResult == null)
                    return DeleteUserResult.DB_IS_DISCONNECTED;

                int nResultCount = arResult.Count;
                string strIDs = "";

                for (int i = 0; i < nResultCount - 2; i += 3)
                {
                    string pass = (string)arResult[i];
                    string id = (string)arResult[i + 1];
                    int nCode = (int)arResult[i + 2];

                    if (pass != szPasswd)
                        return DeleteUserResult.INVALID_PW;

                    if (strIDs.Length == 0)
                        strIDs = nCode.ToString();
                    else
                        strIDs += ", " + nCode.ToString();
                }

                if (strIDs.Length == 0)
                    return DeleteUserResult.INVALID_ID;

                string strSQL = "Delete from LoginUser where id_key in (" + strIDs + ")";

                System.Data.SqlClient.SqlConnection connection = dbMgr.Connect();
                dbMgr.ExecuteSQL(strSQL, connection);
                connection.Close();

                return DeleteUserResult.SUCCESS;
            }
            catch (Exception)
            {
            }           
            return DeleteUserResult.UNKNOWN_ERROR;
        }

        /*private string RequestCode(ConnectionState state, ArrayList arDatas)
        {
            string szUserID = (string)arDatas[0];
            string szPasswd = (string)arDatas[1];

            int nSiteID = NetworkServer.Instance.SiteID;
            try
            {
                HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
                string szSQL = string.Format("select password, id_name, register_code from LoginUser where id_name='{0}' and Site_ID = {1}", szUserID, nSiteID);

                ArrayList arResult = DBHelper.GetResultData(dbMgr, szSQL);
                if (arResult == null || arResult.Count == 0)
                    return "";

                if (arResult.Count != 3)
                    return "";

                string pass = (string)arResult[0];
                string id = (string)arResult[1];
                string code = (string)arResult[2];
                if (pass == szPasswd && szUserID == id)
                    return code;
            }
            catch (Exception)
            { 
            }
            return "";
        }*/

        private JoinUserResult JoinUser(ConnectionState state, ArrayList arDatas)
        {
            int nDataCount = arDatas.Count;

            if (nDataCount < 5)
                return 0;

            string szUserID = (string)arDatas[0];
            string szPasswd = (string)arDatas[1];
            int nUserLevel = (int)arDatas[2];
            int nOption = (int)arDatas[3];
            int nSiteID = (int)arDatas[4];
            
            if (nOption < 0 || nOption >= (int)UnE.KeyValidator.CertOption.TYPE_COUNT)
                return 0;

            ArrayList arrMacAddrList = new ArrayList();

            for (int i=5;i<nDataCount;i++)
            {
                arrMacAddrList.Add(arDatas[i]);
            }

            UnE.KeyValidator.CertOption option = (UnE.KeyValidator.CertOption)nOption;
            ArrayList arrMembers = new ArrayList();

            HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
            JoinUserResult result = DBLoginHelper.IsValidUser(dbMgr, szUserID, szPasswd, nUserLevel, nSiteID, option, arrMembers);

            if (result == JoinUserResult.SUCCESS)
            {
                int nID = -1;
                nID = DBLoginHelper.JoinMember(dbMgr, szUserID, szPasswd, nUserLevel, nSiteID, option, arrMacAddrList, arrMembers);

                if (nID <= 0)
                    return JoinUserResult.DB_IS_DISCONNECTED;
            }
            else
            {
                //return result;
            }

            return result;
        }

        /*private int JoinUser(ConnectionState state, ArrayList arDatas)
        {
            string szUserID = (string)arDatas[0];
            string szPasswd = (string)arDatas[1];
            string szRegCode = (string)arDatas[2];
            string szCode = (string)arDatas[3];
            int nUserLevel = (int)arDatas[4];

            HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
            if (!DBLoginHelper.IsValidUser(dbMgr, szUserID))
            {
                int nID = -1;
                nID = DBLoginHelper.JoinMember(dbMgr, szUserID, szPasswd, szRegCode, szCode, nUserLevel);
                return nID;
            }
            else
            {
                return 0;
            }
        }*/

        /*private bool SetPassword(ConnectionState state, ArrayList arDatas)
        {
            string szUserID = (string)arDatas[0];
            string szPasswd = (string)arDatas[1];

            HSMS.DBConn con = NetworkServer.Instance.DBManager;
            bool bResult = DBLoginHelper.ChangePassword(con, szUserID, szPasswd);
            return bResult;
        }*/

        private byte ToByte(char ch)
        {
            if (ch >= '0' && ch <= '9')
                return (byte)(ch - '0');
            else if (ch >= 'a' && ch <= 'f')
                return (byte)(10 + ch - 'a');
            else if (ch >= 'A' && ch <= 'F')
                return (byte)(10 + ch - 'A');

            return 0;
        }

        private byte[] ToBytes(string strCertCode)
        {
            string strOrder = strCertCode.Substring(28);

            int len = strOrder.Length;

            if (len == 0)
                return null;

            char ch = strOrder.ElementAt(len - 1);

            long nOrder;
            if (!long.TryParse(strOrder.Substring(0, len - 1), out nOrder))
                return null;

            if (ch == '~')
                nOrder = -nOrder;
            else if (ch != '!')
                return null;

            byte[] bytes = new byte[14];
            byte[] bytesTemp = new byte[14];

            for (int i = 0; i < 14; i++)
            {
                char ch1 = strCertCode.ElementAt(i * 2);
                char ch2 = strCertCode.ElementAt(i * 2 + 1);

                bytesTemp[i] = (byte)((ToByte(ch1) << 4) + ToByte(ch2));
            }

            byte[] bytesOrder = BitConverter.GetBytes(nOrder);

            try
            {
                for (int i = 0, j = 0; i < 8; i++)
                {
                    int nIndex1 = bytesOrder[i] >> 4;
                    int nIndex2 = bytesOrder[i] & 0x0f;

                    if (nIndex1 < 0x0e)
                        bytes[nIndex1] = bytesTemp[j++];

                    if (nIndex2 < 0x0e)
                        bytes[nIndex2] = bytesTemp[j++];
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }

            return bytes;
        }

        private bool CheckCertCode(string strUserID, string strCertCode, string strMacAddrList, string strPCCode)
        {
            byte[] bytes = ToBytes(strCertCode);

            int hash = BitConverter.ToInt32(bytes, 4);
            int hash2 = strMacAddrList.GetHashCode();

            if (hash != hash2)
                return false;

            string strBytes = DBLoginHelper.MakePCCode(strUserID, strMacAddrList);

            if (strBytes != strPCCode)
                return false;

            return true;
        }

        private ChangePasswordResult ChangePassword(ConnectionState state, ArrayList arDatas)
        {
            if (arDatas.Count < 4)
                return ChangePasswordResult.NEED_MORE_DATA;

            string szUserID = (string)arDatas[0];
            string strCertCode = (string)arDatas[1];
            string strMacAddrList = (string)arDatas[2];
            string szNewPasswd = (string)arDatas[3];

            int nSiteID = NetworkServer.Instance.SiteID;
            HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
            string strSQL = string.Format("select pc_code from LoginUser where id_name = '{0}' and Site_ID = {1}", szUserID, nSiteID);
            ArrayList arrResult = DBHelper.GetResultData(dbMgr, strSQL);

            if (arrResult == null)
                return ChangePasswordResult.DB_IS_DISCONNECTED;

            if (arrResult.Count == 0)
                return ChangePasswordResult.INVALID_ID;

            bool success = false;

            for (int i = 0; i < arrResult.Count; i++)
            {
                if (CheckCertCode(szUserID, strCertCode, strMacAddrList, arrResult[0].ToString()))
                {
                    success = true;
                    break;
                }
            }

            if (!success)
                return ChangePasswordResult.INVALID_CERT_CODE;

            strSQL = string.Format("Update LoginUser set password = '{0}' where id_name = '{1}' and Site_ID = {2}", szNewPasswd, szUserID, nSiteID);
            return DBHelper.ExecuteSQL(dbMgr, strSQL) ? ChangePasswordResult.SUCCESS : ChangePasswordResult.UNKNOWN_ERROR;
        }
        /*private ChangePasswordResult ChangePassword(ConnectionState state, ArrayList arDatas)
        {
            if (arDatas.Count < 3)
                return ChangePasswordResult.NEED_MORE_DATA;

            string szUserID = (string)arDatas[0];
            string szPasswd = (string)arDatas[1];
            string szNewPasswd = (string)arDatas[2];

            int nSiteID = NetworkServer.Instance.SiteID;
            HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
            string strSQL = string.Format("select password from LoginUser where id_name = '{0}' and Site_ID = {1}", szUserID, nSiteID);
            ArrayList arrResult = DBHelper.GetResultData(dbMgr, strSQL);

            if (arrResult == null)
                return ChangePasswordResult.DB_IS_DISCONNECTED;

            if (arrResult.Count == 0)
                return ChangePasswordResult.INVALID_ID;

            if (szPasswd != arrResult[0].ToString())
                return ChangePasswordResult.INVALID_PW;

            strSQL = string.Format("Update LoginUser set password = '{0}' where id_name = '{1}' and Site_ID = {2}", szNewPasswd, szUserID,nSiteID);
            return DBHelper.ExecuteSQL(dbMgr, strSQL) ? ChangePasswordResult.SUCCESS : ChangePasswordResult.UNKNOWN_ERROR;
        }*/

        private LoginUserResult CheckLoginUser(ArrayList arrDatas, out int nID, out string szUserID, out int nSiteID, out bool isLogin)        
        {
            nID = -1;
            szUserID = "";
            nSiteID = -1;
            isLogin = false;

            int nDataCount = arrDatas.Count;

            if (nDataCount < 4)
                return LoginUserResult.NEED_MORE_DATA;

            nID = (int)arrDatas[0];
            szUserID = (string)arrDatas[1];
            nSiteID = (int)arrDatas[2];
            isLogin = (bool)arrDatas[3];

            return LoginUserResult.SUCCESS;
            //return LoginUser(arrDatas, out nID, out szUserID, out nUserLevel, out szCode);
        }

        private ArrayList GetHashByteString(string strUserID, ArrayList arrMacAddrList)
        {
            ArrayList arrResult = new ArrayList();

            foreach (string strMacAddr in arrMacAddrList)
            {
				if( strMacAddr.Contains("-"))
				{
					string strBytes = DBLoginHelper.MakePCCode(strUserID, strMacAddr);
					arrResult.Add(strBytes);
				}
				else
				{
					try
					{
                        // MacAddress에 '-'가 없는 상태에서 hash값을 한번 만든다.
						string szTempMac = strMacAddr;
                        string strBytes = DBLoginHelper.MakePCCode(strUserID, szTempMac);
                        arrResult.Add(strBytes);

                        // MacAddress에 '-'를 넣고 hash값을 만든다.
						szTempMac = szTempMac.Insert(10, "-");
						szTempMac = szTempMac.Insert(8, "-");
						szTempMac = szTempMac.Insert(6, "-");
						szTempMac = szTempMac.Insert(4, "-");
						szTempMac = szTempMac.Insert(2, "-");
						strBytes = DBLoginHelper.MakePCCode(strUserID, szTempMac);
						arrResult.Add(strBytes);
					}
					catch(Exception)
					{

					}
					
				}
               
            }

            return arrResult;
        }
		private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public LoginUserResult LoginUser(ArrayList arDatas, out int nID, out string szMemberID, out int nUserLevel, out int nSiteID)
        {
            nID = -1;
            nUserLevel = -1;
            szMemberID = "";
            nSiteID = -1;

            int nDataCount = arDatas.Count;

            if (nDataCount < 4)
                return LoginUserResult.NEED_MORE_DATA;
            
            szMemberID = (string)arDatas[0];

            string szPass = (string)arDatas[1];
            nSiteID = (int)arDatas[2];

            ArrayList arrMacAddrList = new ArrayList();

            for (int i = 3; i < nDataCount;i++ )
            {
                arrMacAddrList.Add(arDatas[i]);
            }

            LoginUserResult result = LoginUserResult.INVALID_ID;

            try
            {
                HSMS.DBConn dbMgr = NetworkServer.Instance.DBManager;
                string szSQL = string.Format("select id_key, id_name, pc_code, password, user_level from LoginUser where id_name = '{0}' and Site_ID = {1}", szMemberID, nSiteID);

                ArrayList arResult = DBHelper.GetResultData(dbMgr, szSQL);
                if (arResult == null)
                    return LoginUserResult.DB_IS_DISCONNECTED;

                ArrayList arrHashBytes = null;
                int nResultCount = arResult.Count;

                for (int i = 0; i < nResultCount - 4;i+=5 )
                {
                    nID = (int)arResult[i];
                    string strUserID = (string)arResult[i + 1];
                    string strPCCode = (string)arResult[i + 2];
                    string strPW = (string)arResult[i + 3];
                    nUserLevel = (int)arResult[i + 4];

                    if (strUserID == szMemberID)
                    {
						
						foreach(string mac in arrMacAddrList)
						{
							logger.Debug(mac);
						}
						
                        result = LoginUserResult.NOT_PERMIT_PC;

                        if (arrHashBytes == null)
                            arrHashBytes = GetHashByteString(szMemberID, arrMacAddrList);

                        if (arrHashBytes.Contains(strPCCode))
                        {
                            if (strPW == szPass)
                                return LoginUserResult.SUCCESS;
                            else
                                return LoginUserResult.INVALID_PW;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return result;
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
            byte[] nCount = BitConverter.GetBytes(2);

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
                //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine("SendSuccessMessage", ex);
            }

        }

        public void SendChangePassword(ConnectionState state, ChangePasswordResult result)
        {
            SendSuccessMessage(state, (int)result, (short)TCP_ID.CHNAGE_PASSWORD);
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
                //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine("SendCheckLogin", ex);
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
                //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine("SendJoinUser", ex);
            }
        }

        /*public void SendRequestCode(ConnectionState state, string szCode)
        {
            byte[] data = ServiceProvider.MakeBytes(szCode);

            byte[] bytes = new byte[6 + data.Length];
            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.REQUEST_CODE);
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
                ConnectionLogEx.Instance.WriteLine("SendRequestCode", ex);
            }
        }*/

        public void SendRejectLogin(ConnectionState state, LoginUserResult type)
        {
            byte[] data = ServiceProvider.MakeBytes((int)type);

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
                //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine("SendRejectLogin", ex);
            }
        }

        public static byte[] MakeLogoutBytes()
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

            return bytes;
        }

        public void SendLogout(ConnectionState state)
        {
            byte[] bytes = MakeLogoutBytes();
            /*byte[] bytes = new byte[6];
            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.LOGOUT_USER);
            byte[] nCount = BitConverter.GetBytes(0);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];*/

            try
            {
                m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine("SendLogout", ex);
            }
        }

        public void SendAcceptLogin(ConnectionState state, int nUserID, string szUserID, int nUserLevel)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add(nUserID);
            arrDatas.Add(szUserID);
            arrDatas.Add(nUserLevel);

            byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.ACCEPT_LOGIN, arrDatas);

            /*byte[] data1 = ServiceProvider.MakeBytes(szUserID);
            byte[] data2 = ServiceProvider.MakeBytes(nUserLevel);

            byte[] bytes = new byte[6 + data1.Length + data2.Length];
            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.ACCEPT_LOGIN);
            byte[] nCount = BitConverter.GetBytes(2);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(data1, 0, bytes, 6, data1.Length);
            System.Buffer.BlockCopy(data2, 0, bytes, 6 + data1.Length, data2.Length);*/

            try
            {
                m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {
                //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine("SendAcceptLogin", ex);
            }
        }

        public void SendDeleteUser(ConnectionState state, DeleteUserResult type, string strUserID)
        {
            ArrayList arrDatas = new ArrayList();

            arrDatas.Add((int)type);
            arrDatas.Add(strUserID);

            byte[] bytes = ServiceProvider.MakeBytes(TCP_ID.DELETE_USER, arrDatas);
            /*byte[] data = ServiceProvider.MakeBytes((int)type);

            byte[] bytes = new byte[6 + data.Length];
            byte[] nHader = BitConverter.GetBytes((short)TCP_ID.DELETE_USER);
            byte[] nCount = BitConverter.GetBytes(1);

            // SET MESSAGE HeADER
            bytes[0] = nHader[0];
            bytes[1] = nHader[1];

            // SET DATA COUNT
            bytes[2] = nCount[0];
            bytes[3] = nCount[1];
            bytes[4] = nCount[2];
            bytes[5] = nCount[3];

            System.Buffer.BlockCopy(data, 0, bytes, 6, data.Length);*/

            try
            {
                m_provider.Send(bytes, 0, bytes.Length, state);
            }
            catch (System.Exception ex)
            {

                //NetworkServer.Instance.ServiceProvider.ConnectionLog.WriteLine("SendDeleteUser", ex);
            }
        }
    }
}
