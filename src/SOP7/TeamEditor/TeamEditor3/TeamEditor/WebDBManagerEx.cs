using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using DBUtility2;

namespace TeamEditor
{
    public class WebDBManagerEx : WebDBManager
    {
        // Batch Query는 성능 문제로 인하여 DirectDBManager를 사용한다.
        // [2017/06/11] 김지웅
        private DirectDBManager m_batchManager = null;

        public WebDBManagerEx(int nSiteID)
            : base(nSiteID)
        {
            Init();
        }

        public WebDBManagerEx(string strDatabaseName, int nSiteID)
            : base(strDatabaseName, nSiteID)
        {
            Init();
        }

        public WebDBManagerEx(string strDatabaseName, string strDBType, int nSiteID)
            : base(strDatabaseName, strDBType, nSiteID)
        {
            Init();
        }

        private void Init()
        {
            if (this.DatabaseType == DBType.mysql)
                m_batchManager = new MySQLManager(this);
            else if (this.DatabaseType == DBType.sqlserver)
                m_batchManager = new SQLServerManager(this);
        }

        /*public override bool BeginBatch(string szDBName = null)
        {
            return m_batchManager.BeginBatch();
        }

        public override void BatchCommit()
        {
            m_batchManager.BatchCommit();
        }

        public override void BatchRollback()
        {
            m_batchManager.BatchRollback();
        }

        public override ArrayList GetBatchData(string strSQLQuery, string szDBName = null)
        {
            return m_batchManager.GetBatchData(strSQLQuery);
        }*/
    }

    internal abstract class DirectDBManager
    {
        protected WebDBManagerEx m_dbMgr = null;
        protected string m_strUser = "admin";
        protected string m_strPW = "12345678";

        public DirectDBManager(WebDBManagerEx dbMgr)
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
            m_strUser = AES256Cipher.AES_decrypt("GUk6cJACqVBoIFh7ny7mqQ==", key);
            m_strPW = AES256Cipher.AES_decrypt("SezOwMM9A2mIbUk5DCW/eQ==", key);
        }

        public abstract bool BeginBatch();
        public abstract void BatchCommit();
        public abstract void BatchRollback();
        public abstract ArrayList GetBatchData(string strSQLQuery);
        public abstract bool IsBeginTransaction();
    }

    internal class SQLServerManager : DirectDBManager
    {
        private System.Data.SqlClient.SqlConnection m_connection = null;
        private System.Data.SqlClient.SqlTransaction m_transaction = null;

        public SQLServerManager(WebDBManagerEx dbMgr)
            : base(dbMgr)
        {
        }

        public override bool BeginBatch()
        {
            if (m_connection == null)
            {
                m_connection = NewConnection();

                if (m_connection == null)
                    return false;
            }

            m_transaction = m_connection.BeginTransaction();
            return m_transaction != null;
        }

        public override void BatchCommit()
        {
            if (m_connection == null || m_transaction == null)
                return;

            m_transaction.Commit();

            m_connection.Close();
            m_connection = null;
            m_transaction = null;
        }

        public override void BatchRollback()
        {
            if (m_connection == null || m_transaction == null)
                return;

            m_transaction.Rollback();

            m_connection.Close();
            m_connection = null;
            m_transaction = null;
        }

        public override ArrayList GetBatchData(string strSQLQuery)
        {
            if (m_connection == null || m_transaction == null)
                return null;

            ArrayList arrResult = new ArrayList();
            string strSQL = strSQLQuery.ToLower().Trim();

            if (strSQL.StartsWith("select"))
            {
                System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(strSQLQuery, m_connection, m_transaction);
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
                try
                {
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(strSQLQuery, m_connection, m_transaction);
                    int nResult = command.ExecuteNonQuery();

                    if (nResult < 0)
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return arrResult;
        }

        private System.Data.SqlClient.SqlConnection NewConnection()
        {
            string strURL = GetURL();

            string strConnection = string.Format("Data Source={0};Initial Catalog={1};User id={2};Password={3};",
                strURL, m_dbMgr.DatabaseName, m_strUser, m_strPW);
            System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(strConnection);
            connection.Open();

            if (connection.State != System.Data.ConnectionState.Open)
                return null;

            return connection;
        }

        public override bool IsBeginTransaction()
        {
            return m_transaction != null;
        }
    }

    internal class MySQLManager : DirectDBManager
    {
        private MySql.Data.MySqlClient.MySqlConnection m_connection = null;
        private MySql.Data.MySqlClient.MySqlTransaction m_transaction = null;

        public MySQLManager(WebDBManagerEx dbMgr)
            : base(dbMgr)
        {
        }

        public override bool BeginBatch()
        {
            if (m_connection == null)
            {
                m_connection = NewConnection();

                if (m_connection == null)
                    return false;
            }

            m_transaction = m_connection.BeginTransaction();
            return m_transaction != null;
        }

        public override void BatchCommit()
        {
            if (m_connection == null || m_transaction == null)
                return;

            m_transaction.Commit();

            m_connection.Close();
            m_connection = null;
            m_transaction = null;
        }

        public override void BatchRollback()
        {
            if (m_connection == null || m_transaction == null)
                return;

            m_transaction.Rollback();

            m_connection.Close();
            m_connection = null;
            m_transaction = null;
        }

        public override ArrayList GetBatchData(string strSQLQuery)
        {
            if (m_connection == null || m_transaction == null)
                return null;

            ArrayList arrResult = new ArrayList();
            string strSQL = strSQLQuery.ToLower().Trim();

            if (strSQL.StartsWith("select"))
            {
                MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(strSQLQuery, m_connection, m_transaction);
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
                try
                {
                    MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(strSQLQuery, m_connection, m_transaction);
                    int nResult = command.ExecuteNonQuery();

                    if (nResult < 0)
                        return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return arrResult;
        }

        private MySql.Data.MySqlClient.MySqlConnection NewConnection()
        {
            string strURL = GetURL();

            string strConnection = string.Format("Server={0};Database={1};Uid={2};Pwd={3};",
                strURL, m_dbMgr.DatabaseName, m_strUser, m_strPW);
            MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(strConnection);
            connection.Open();

            if (connection.State != System.Data.ConnectionState.Open)
                return null;

            return connection;
        }

        public override bool IsBeginTransaction()
        {
            return m_transaction != null;
        }
    }
}
