using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace KpxPipeMonitoring
{
    public class WebDBManager2 : DBUtility.WebDBManager
    { 
        private string m_strSirenPath = "";
        private string m_strDoorBellPath = "";

        private int m_nLevel = -1;

        private static bool m_isLoadSMSAddText = false;
        private static string m_strSmsAddText = "";
        private static string m_strSmsCaller = "";

        // Batch Query는 성능 문제로 인하여 DirectDBManager를 사용한다.
        // [2017/06/11] 김지웅
        private DirectDBManager m_batchManager = null;

        public WebDBManager2(int nSiteID)
            : base(nSiteID)
        { 
            m_strSirenPath = LoadIni("siren_file");
            m_strDoorBellPath = LoadIni("doorbell_file");
            m_strSmsCaller = LoadIni("sms_caller");

            if (this.DatabaseType == DBType.mysql)
                m_batchManager = new MySQLManager(this);
            else if (this.DatabaseType == DBType.sqlserver)
                m_batchManager = new SQLServerManager(this);
        }

        // ExternalCompanyMember 휴대폰 암호화
        private void EncryptExternalCompanyMember()
        {
            string strSQL = "select id, PhoneNumber from ExternalCompanyMember";
            
            ArrayList arrResult = GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
            //System.IO.StreamWriter writer = new System.IO.StreamWriter("c:/UnE/ExternalCompanyMember.sql", false, Encoding.UTF8);

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = GetIntField(arrResult[i].ToString(), -1);
                string strPhoneNumber = GetStringField(arrResult[i + 1], "");

                if (nID < 0)
                    continue;

                if (string.Compare(strPhoneNumber, "null", true) == 0)
                    strPhoneNumber = "";

                bool isValid;
                strPhoneNumber = ValidPhoneNumber(strPhoneNumber, out isValid);

                if (!isValid)
                    continue;

                string strEncrypt = strPhoneNumber.Length == 0 ? "" : DBUtility.AES256Cipher.AES_encrypt(strPhoneNumber, key);
                //writer.WriteLine(string.Format("Update ExternalCompanyMember set PhoneNumber = '{0}' where id = {1};", strEncrypt, nID));
            }

            //writer.Close();
        }

        // strPhoneNumber에 빈칸이나 '-'등이 들어있을 경우 없앤다. 
        public static string ValidPhoneNumber(string strPhoneNumber, out bool isValid)
        {
            isValid = true;

            string strResult = "";
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch != ' ' && ch != '\t' && ch != '-')
                {
                    if (ch >= '0' && ch <= '9')
                        strResult += ch;
                    else
                    {
                        isValid = false;
                        return "";
                    }
                }
            }

            return strResult;
        }

        public string SMS_ADD_TEXT
        {
            get
            {
                if (m_isLoadSMSAddText)
                    return m_strSmsAddText;
                else
                {
                    m_strSmsAddText = LoadIni("sms_add_text", "Server Connection Info");
                    m_isLoadSMSAddText = true;
                }

                return m_strSmsAddText;
            }
        }

        // User 권한
        public int Level
        {
            get { return m_nLevel; }
            set { m_nLevel = value; }
        }

        public int GetGenUserLevel(int nGenUserID)
        {
            string strSQL = "select UserLevel from SOPGenUser where ID = " + nGenUserID.ToString();
            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            return GetIntField(arrResult[0].ToString(), -1);
        }

        public string SirenPath
        {
            get { return m_strSirenPath; }
        }

        public string DoorBellPath
        {
            get { return m_strDoorBellPath; }
        }

        public static string SMSCaller
        {
            get { return m_strSmsCaller; }
        }
#if !DEV_TEST
        public override bool BeginBatch(int nBatchCode)
        {
            return m_batchManager.BeginBatch(nBatchCode.ToString());
        }

        public override void BatchCommit(int nBatchCode)
        {
            m_batchManager.BatchCommit(nBatchCode.ToString());
        }

        public override void BatchRollback(int nBatchCode)
        {
            m_batchManager.BatchRollback(nBatchCode.ToString());
        }

        public override ArrayList GetBatchData(int nBatchCode, string strSQLQuery)
        {
            ArrayList arrResult = m_batchManager.GetBatchData(nBatchCode.ToString(), strSQLQuery);
            return arrResult;
        }

        public override ArrayList GetResultData(string strSQLQuery, int nTranstion, int nLimit, string szDBName = null)
        {
            ArrayList arrResult = m_batchManager.GetResultData(strSQLQuery, nLimit);
            return arrResult;
        }

        public override ArrayList GetResultData(string strSQLQuery, int nTranstion, string szDBName = null)
        {
            ArrayList arrResult = m_batchManager.GetResultData(strSQLQuery);
            return arrResult;
        }
#endif
    }

    internal abstract class DirectDBManager
    {
        protected WebDBManager2 m_dbMgr = null;
        protected string m_strUser = "admin";
        protected string m_strPW = "12345678";

        public DirectDBManager(WebDBManager2 dbMgr)
        {
            m_dbMgr = dbMgr;
            Init();
        }

        protected string GetURL()
        {
            string strServerURL = m_dbMgr.WebServerURL;

            int nIndex1 = strServerURL.IndexOf("http://");
            int nIndex2 = strServerURL.LastIndexOf(':');
            string strURL = strServerURL;

            if (nIndex1 >= 0 && nIndex2 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
            }
            else if (nIndex1 >= 0)
            {
                int nBeginIndex = nIndex1 + "http://".Length;
                strURL = strServerURL.Substring(nBeginIndex);
            }
            else if (nIndex2 >= 0)
            {
                strURL = strServerURL.Substring(0, nIndex2);
            }

            return strURL;
        }

        private void Init()
        {
            string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
            m_strUser = DBUtility.AES256Cipher.AES_decrypt("GUk6cJACqVBoIFh7ny7mqQ==", key);
            m_strPW = DBUtility.AES256Cipher.AES_decrypt("SezOwMM9A2mIbUk5DCW/eQ==", key);
        }

        public abstract bool BeginBatch(string strTransactionName);
        public abstract void BatchCommit(string strTransactionName);
        public abstract void BatchRollback(string strTransactionName);
        public abstract ArrayList GetBatchData(string strTransactionName, string strSQLQuery);
        public abstract ArrayList GetResultData(string strSQLQuery, int nLimit);
        public abstract ArrayList GetResultData(string strSQLQuery);
    }

    internal class SQLServerManager : DirectDBManager
    {
        private Dictionary<string, System.Data.SqlClient.SqlConnection> m_dicConnection = new Dictionary<string, System.Data.SqlClient.SqlConnection>();
        private Dictionary<string, System.Data.SqlClient.SqlTransaction> m_dicTransaction = new Dictionary<string,System.Data.SqlClient.SqlTransaction>();

        public SQLServerManager(WebDBManager2 dbMgr)
            : base(dbMgr)
        {
        }

        public override bool BeginBatch(string strTransactionName)
        {
            System.Data.SqlClient.SqlConnection connection = null;

            if (m_dicConnection.TryGetValue(strTransactionName, out connection) == false)
            {
                connection = NewConnection(strTransactionName);

                if (connection == null)
                    return false;
            }

            System.Data.SqlClient.SqlTransaction transaction = connection.BeginTransaction();
            m_dicTransaction[strTransactionName] = transaction;
            return transaction != null;
        }

        public override void BatchCommit(string strTransactionName)
        {
            System.Data.SqlClient.SqlConnection connection = null;
            System.Data.SqlClient.SqlTransaction transaction = null;

            if (m_dicConnection.TryGetValue(strTransactionName, out connection) == false)
                return;

            if (m_dicTransaction.TryGetValue(strTransactionName, out transaction) == false)
                return;

            transaction.Commit();

            connection.Close();
            m_dicConnection.Remove(strTransactionName);
            m_dicTransaction.Remove(strTransactionName);
        }

        public override void BatchRollback(string strTransactionName)
        {
            System.Data.SqlClient.SqlConnection connection = null;
            System.Data.SqlClient.SqlTransaction transaction = null;

            if (m_dicConnection.TryGetValue(strTransactionName, out connection) == false)
                return;

            if (m_dicTransaction.TryGetValue(strTransactionName, out transaction) == false)
                return;

            transaction.Rollback();

            connection.Close();
            m_dicConnection.Remove(strTransactionName);
            m_dicTransaction.Remove(strTransactionName);
        }

        public override ArrayList GetBatchData(string strTransactionName, string strSQLQuery)
        {
            System.Data.SqlClient.SqlConnection connection = null;
            System.Data.SqlClient.SqlTransaction transaction = null;

            if (m_dicConnection.TryGetValue(strTransactionName, out connection) == false)
                return null;

            if (m_dicTransaction.TryGetValue(strTransactionName, out transaction) == false)
                return null;

            ArrayList arrResult = new ArrayList();
            string strSQL = strSQLQuery.ToLower().Trim();

            try
            {
                if (strSQL.StartsWith("select"))
                {
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(strSQLQuery, connection, transaction);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    if (reader == null)
                        return null;

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.IsDBNull(i))
                                    arrResult.Add("null");
                                else
                                    arrResult.Add(reader[i].ToString());
                            }
                        }
                    }

                    reader.Close();
                }
                else
                {
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(strSQLQuery, connection, transaction);
                    int nResult = command.ExecuteNonQuery();

                    if (nResult < 0)
                        return null;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }

            return arrResult;
        }

        public override ArrayList GetResultData(string strSQLQuery, int nLimit)
        {
            int nIdx = strSQLQuery.ToLower().IndexOf("select");
            if( nIdx >= 0)
            {
                strSQLQuery = strSQLQuery.Insert(6, " TOP " + nLimit + " ");
            }

            return GetResultData(strSQLQuery);
        }

        public override ArrayList GetResultData(string strSQLQuery)
        {
            System.Data.SqlClient.SqlConnection connection = NewConnection("");

            if (connection == null)
                return null;

            ArrayList arrResult = new ArrayList();
            string strSQL = strSQLQuery.ToLower().Trim();

            try
            {
                if (strSQL.StartsWith("select"))
                {
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(strSQLQuery, connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    if (reader == null)
                    {
                        connection.Close();
                        return null;
                    }

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.IsDBNull(i))
                                    arrResult.Add("null");
                                else
                                    arrResult.Add(reader[i].ToString());
                            }
                        }
                    }

                    reader.Close();
                }
                else
                {
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(strSQLQuery, connection);
                    int nResult = command.ExecuteNonQuery();

                    if (nResult < 0)
                    {
                        connection.Close();
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                connection.Close();
                return null;
            }

            connection.Close();
            return arrResult;
        }

        private System.Data.SqlClient.SqlConnection NewConnection(string strTransactionName)
        {
            string strURL = GetURL();
            
            string strConnection = string.Format("Data Source={0};Initial Catalog={1};User id={2};Password={3};",
                strURL, m_dbMgr.DatabaseName, m_strUser, m_strPW);
            System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(strConnection);
            connection.Open();

            if (connection.State != System.Data.ConnectionState.Open)
                return null;

            if (strTransactionName.Length > 0)
                m_dicConnection[strTransactionName] = connection;

            return connection;
        }
    }

    internal class MySQLManager : DirectDBManager
    {
        private Dictionary<string, MySql.Data.MySqlClient.MySqlConnection> m_dicConnection = new Dictionary<string, MySql.Data.MySqlClient.MySqlConnection>();
        private Dictionary<string, MySql.Data.MySqlClient.MySqlTransaction> m_dicTransaction = new Dictionary<string, MySql.Data.MySqlClient.MySqlTransaction>();

        public MySQLManager(WebDBManager2 dbMgr)
            : base(dbMgr)
        {
        }

        public override bool BeginBatch(string strTransactionName)
        {
            MySql.Data.MySqlClient.MySqlConnection connection = null;

            if (m_dicConnection.TryGetValue(strTransactionName, out connection) == false)
            {
                connection = NewConnection(strTransactionName);

                if (connection == null)
                    return false;
            }

            MySql.Data.MySqlClient.MySqlTransaction transaction = connection.BeginTransaction();
            m_dicTransaction[strTransactionName] = transaction;
            return transaction != null;
        }

        public override void BatchCommit(string strTransactionName)
        {
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlTransaction transaction = null;

            if (m_dicConnection.TryGetValue(strTransactionName, out connection) == false)
                return;

            if (m_dicTransaction.TryGetValue(strTransactionName, out transaction) == false)
                return;

            transaction.Commit();

            connection.Close();
            m_dicConnection.Remove(strTransactionName);
            m_dicTransaction.Remove(strTransactionName);
        }

        public override void BatchRollback(string strTransactionName)
        {
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlTransaction transaction = null;

            if (m_dicConnection.TryGetValue(strTransactionName, out connection) == false)
                return;

            if (m_dicTransaction.TryGetValue(strTransactionName, out transaction) == false)
                return;

            transaction.Rollback();

            connection.Close();
            m_dicConnection.Remove(strTransactionName);
            m_dicTransaction.Remove(strTransactionName);
        }

        public override ArrayList GetBatchData(string strTransactionName, string strSQLQuery)
        {
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlTransaction transaction = null;

            if (m_dicConnection.TryGetValue(strTransactionName, out connection) == false)
                return null;

            if (m_dicTransaction.TryGetValue(strTransactionName, out transaction) == false)
                return null;

            ArrayList arrResult = new ArrayList();
            string strSQL = strSQLQuery.ToLower().Trim();

            try
            {
                if (strSQL.StartsWith("select"))
                {
                    MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(strSQLQuery, connection, transaction);
                    MySql.Data.MySqlClient.MySqlDataReader reader = command.ExecuteReader();

                    if (reader == null)
                        return null;

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.IsDBNull(i))
                                    arrResult.Add("null");
                                else
                                    arrResult.Add(reader[i].ToString());
                            }
                        }
                    }

                    reader.Close();
                }
                else
                {
                    MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(strSQLQuery, connection, transaction);
                    int nResult = command.ExecuteNonQuery();

                    if (nResult < 0)
                        return null;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }

            return arrResult;
        }

        public override ArrayList GetResultData(string strSQLQuery, int nLimit)
        {
            strSQLQuery += " LIMIT 0," + nLimit;

            return GetResultData(strSQLQuery);
        }

        public override ArrayList GetResultData(string strSQLQuery)
        {
            MySql.Data.MySqlClient.MySqlConnection connection = NewConnection("");

            if (connection == null)
                return null;

            ArrayList arrResult = new ArrayList();
            string strSQL = strSQLQuery.ToLower().Trim();

            try
            {
                if (strSQL.StartsWith("select"))
                {
                    MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(strSQLQuery, connection);
                    MySql.Data.MySqlClient.MySqlDataReader reader = command.ExecuteReader();

                    if (reader == null)
                    {
                        connection.Close();
                        return null;
                    }

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.IsDBNull(i))
                                    arrResult.Add("null");
                                else
                                    arrResult.Add(reader[i].ToString());
                            }
                        }
                    }

                    reader.Close();
                }
                else
                {
                    MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(strSQLQuery, connection);
                    int nResult = command.ExecuteNonQuery();

                    if (nResult < 0)
                    {
                        connection.Close();
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                connection.Close();
                return null;
            }

            connection.Close();
            return arrResult;
        }

        private MySql.Data.MySqlClient.MySqlConnection NewConnection(string strTransactionName)
        {
            string strURL = GetURL();

            string strConnection = string.Format("Server={0};Database={1};Uid={2};Pwd={3};",
                strURL, m_dbMgr.DatabaseName, m_strUser, m_strPW);
            MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(strConnection);
            connection.Open();

            if (connection.State != System.Data.ConnectionState.Open)
                return null;

            if (strTransactionName.Length > 0)
                m_dicConnection[strTransactionName] = connection;

            return connection;
        }
    }
}
