using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HSMS;
using System.Data.SqlClient;

namespace HSMSServer2
{

    public class DBLoginHelper
    {
        public static LoginInfo FindLoginUser(DBConn conn, int nUserID)
        {
            int nSiteID = NetworkServer.Instance.SiteID;

            string strSQL = string.Format("select id_key, id_name, pc_code, user_level from LoginUser where id_key = {0} and Site_ID = {1}", nUserID, nSiteID);
            
            ArrayList arrResult = DBHelper.GetResultData(conn, strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            if (nResultCount < 4)
                return null;

            int nID = (int)(arrResult[0]);
            string szMemberID = (string)(arrResult[1]);
            string szPCCode = (string)arrResult[2];
            int nUserLevel = (int)(arrResult[3]);
            
            LoginInfo info = new LoginInfo();

            info.ID = nID;
            info.UserID = szMemberID;
            info.PCCode = szPCCode;
            info.UserLevel = nUserLevel;            

            return info;
        }

        public static bool ChangePassword(DBConn conn, string szUserID, string szNewPass)
        {
            bool bResult = false;
            int nSiteID = NetworkServer.Instance.SiteID;
            string szSQL = string.Format("Update LoginUser set Password = '{0}' where id_name = '{1}' and Site_ID = {2}", szNewPass, szUserID, nSiteID);
           
            bResult = DBHelper.ExecuteSQL(conn, szSQL);
            return bResult;
        }

        public static JoinUserResult IsValidUser(DBConn conn, string szUserName, string szPassword, int userLevel, int nSiteID, UnE.KeyValidator.CertOption option, ArrayList arrMembers)
        {
            string strSQL = string.Format("select id_key, id_name, pc_code, password, user_level from LoginUser where Site_ID = {0} and id_name = '{1}'", nSiteID, szUserName);

            ArrayList arrResult = DBHelper.GetResultData(conn, strSQL);
            if (arrResult == null)
                return JoinUserResult.DB_IS_DISCONNECTED;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                int nID = (int)arrResult[i];
                string strUserID = (string)arrResult[i + 1];
                string strPCCode = (string)arrResult[i + 2];
                string strPW = (string)arrResult[i + 3];
                int nUserLevel = (int)arrResult[i + 4];

                if (strUserID == szUserName)
                {
                    if (option == UnE.KeyValidator.CertOption.NEW_CREATE)
                        return JoinUserResult.ALREADY_EXIST;
                    else
                    {
                        if (szPassword != strPW)
                            return JoinUserResult.INVALID_PASSWORD;
                    }
                }

                LoginInfoEx info = new LoginInfoEx();

                info.ID = nID;
                info.UserID = strUserID;
                info.Password = strPW;
                info.PCCode = strPCCode;
                info.UserLevel = nUserLevel;

                arrMembers.Add(info);
            }

            if (option == UnE.KeyValidator.CertOption.NEW_CREATE)
            {
                if (arrMembers.Count > 0)
                    return JoinUserResult.ALREADY_EXIST;
            }
            else if (option == UnE.KeyValidator.CertOption.INSERT)
            {
                if (arrMembers.Count > 0)
                {
                    LoginInfoEx info = (LoginInfoEx)arrMembers[0];

                    // 기존 계정에서 사용할 PC Mac Address를 추가하는 옵션이므로
                    // 기존 계정의 User Level과 다른 값을 입력할 수는 없다.
                    if (userLevel != info.UserLevel)
                        return JoinUserResult.INVALID_USER_LEVEL;
                }
            }
            else if (option != UnE.KeyValidator.CertOption.UPDATE)
                return JoinUserResult.UNKNOWN_JOIN_OPTION;

            return JoinUserResult.SUCCESS;
        }

        private static bool DeleteUsers(DBConn conn, ArrayList arrMembers)
        {
            string strIDs = "";

            foreach (LoginInfoEx info in arrMembers)
            {
                if (strIDs.Length == 0)
                    strIDs = info.ID.ToString();
                else
                    strIDs += ", " + info.ID.ToString();
            }

            if (strIDs.Length == 0)
                return true;

            string strSQL = string.Format("Delete from LoginUser where id_key in ({0})", strIDs);
            return DBHelper.ExecuteSQL(conn, strSQL);
        }

        public static bool IsValidUser(DBConn conn, int nUserID, int nSiteID)
        {
            string strSQL = string.Format("select user_level from LoginUser where id_key = {0} and Site_ID = {1}", nUserID, nSiteID);
                       
            ArrayList arrResult = DBHelper.GetResultData(conn, strSQL);
            if (arrResult == null || arrResult.Count == 0)
                return false;

            int nLevel = (int)(arrResult[0]);
            if (nLevel == 1 || nLevel == 0)
                return true;
            return false;
        }

        public static int GetMaxID(string strTableName, string strIDFieldName, string strWhere, SqlConnection connection = null)
        {
            DBConn dbMgr = NetworkServer.Instance.DBManager;

            SqlConnection connection2 = connection == null ? dbMgr.Connect() : connection;

            string strSQL = "Select max(" + strIDFieldName + ") from " + strTableName;

            if (strWhere.Length > 0)
                strSQL += " " + strWhere;

            SqlDataReader reader = dbMgr.ExecuteReader(strSQL, connection2);

            int nID = 0;

            if (reader.Read())
            {
                if (!reader.IsDBNull(0))
                    nID = (int)reader[0];
            }

            reader.Close();

            if (connection == null)
                connection2.Close();

            return nID;
        }

        // Return 값 : (strUserID + strMacAddress)의 Hash값을 byte string으로 변환한 값
        public static string MakePCCode(string strUserID, string strMacAddress)
        {
            int hash = (strUserID + strMacAddress).GetHashCode();
            byte[] bytes = BitConverter.GetBytes(hash);
            string strBytes = BitConverter.ToString(bytes).Replace("-", "");

            return strBytes;
        }

        //회원가입 쿼리
        public static int JoinMember(DBConn conn, string szUserID, string strpwd, int nUserLevel, int nSiteID, UnE.KeyValidator.CertOption option, ArrayList arrMacAddrList, ArrayList arrMembers)
        {
            SqlConnection connection = conn.Connect();

            int nID = -1;

            if (option == UnE.KeyValidator.CertOption.NEW_CREATE)
            {
                nID = GetMaxID("LoginUser", "id_key", "where Site_ID = " + nSiteID.ToString(), connection) + 1;

                foreach (string strMacAddr in arrMacAddrList)
                {
                    string strPCCode = MakePCCode(szUserID, strMacAddr);

                    string strSQL = string.Format("Insert into LoginUser (id_key, id_name, pc_code, password, user_level, Site_ID, description) values ({0}, '{1}', '{2}', '{3}', {4}, {5}, NULL)",
                        nID++, szUserID, strPCCode, strpwd, nUserLevel, nSiteID);

                    conn.ExecuteSQL(strSQL, connection);
                }
            }
            else if (option == UnE.KeyValidator.CertOption.UPDATE)
            {
                // arrMembers에 담겨있는 계정들로 접속된 Client들은 Logout 시킨다.
                NetworkServer.Instance.ServiceProvider.SendLogout(arrMembers);
                // arrMembers에 담겨있는 계정들은 DB에서 삭제한다.
                if (!DeleteUsers(conn, arrMembers))
                    return -1;

                nID = GetMaxID("LoginUser", "id_key", "where Site_ID = " + nSiteID.ToString(), connection) + 1;

                foreach (string strMacAddr in arrMacAddrList)
                {
                    string strPCCode = MakePCCode(szUserID, strMacAddr);

                    string strSQL = string.Format("Insert into LoginUser (id_key, id_name, pc_code, password, user_level, Site_ID, description) values ({0}, '{1}', '{2}', '{3}', {4}, {5}, NULL)",
                        nID++, szUserID, strPCCode, strpwd, nUserLevel, nSiteID);

                    conn.ExecuteSQL(strSQL, connection);
                }
            }
            else if (option == UnE.KeyValidator.CertOption.INSERT)
            {
                nID = GetMaxID("LoginUser", "id_key", "where Site_ID = " + nSiteID.ToString(), connection) + 1;

                foreach (string strMacAddr in arrMacAddrList)
                {
                    string strPCCode = MakePCCode(szUserID, strMacAddr);

                    if (FindLoginInfo(arrMembers, szUserID, strPCCode) != null)
                        continue;

                    string strSQL = string.Format("Insert into LoginUser (id_key, id_name, pc_code, password, user_level, Site_ID, description) values ({0}, '{1}', '{2}', '{3}', {4}, {5}, NULL)",
                        nID++, szUserID, strPCCode, strpwd, nUserLevel, nSiteID);

                    conn.ExecuteSQL(strSQL, connection);
                }
            }

            connection.Close();
            return nID;
        }

        private static LoginInfo FindLoginInfo(ArrayList arrMembers, string strUserID, string strPCCode)
        {
            foreach (LoginInfo info in arrMembers)
            {
                if (info.UserID == strUserID && info.PCCode == strPCCode)
                    return info;
            }

            return null;
        }

        //회원가입 쿼리
        public static int JoinMember(DBConn conn, string szUserID, string strpwd, string szRegCode, string strCode, int nUserLevel)
        {
            int nSiteID = NetworkServer.Instance.SiteID;

            string szSQL = "insert into LoginUser(id_key, id_name, user_level, password, code, register_code, Site_ID) values('"+ DBHelper.MaxID + "', '"
                + szUserID + "', '" + nUserLevel + "' , '" + strpwd + "', '" + strCode + "', '" + szRegCode + "', '" + nSiteID + "')";

            int nMaxID = -1;
            bool bResult = DBHelper.ExecuteSQL(conn, szSQL, "LoginUser", ref nMaxID, "id_key");
            return nMaxID;
        }
    }

    public class LoginInfo
    {
        private int m_nID = -1;
        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        private string m_szUserID = "";
        public string UserID
        {
            get { return m_szUserID; }
            set { m_szUserID = value; }
        }

        private int m_nUserLevel = -1;
        public int UserLevel
        {
            get { return m_nUserLevel; }
            set { m_nUserLevel = value; }
        }

        private string m_szPCCode = "";
        public string PCCode
        {
            get { return m_szPCCode; }
            set { m_szPCCode = value; }
        }

        DateTime m_dtLoginTime;
        public System.DateTime LoginTime
        {
            get { return m_dtLoginTime; }
            set { m_dtLoginTime = value; }
        }
    }

    public class LoginInfoEx : LoginInfo
    {
        private string m_szPW = "";
        public string Password
        {
            get { return m_szPW; }
            set { m_szPW = value; }
        }
    }
}
