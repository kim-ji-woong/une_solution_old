using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace UnEService
{
    public class SqlServerManager : DBManager
    {
        private SqlTransaction m_transaction = null;
        private SqlConnection m_connection = null;

        private static string GetConnectionString(string strDBName)
        {
            return string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};", Host, strDBName, ID, PW);
        }

        public static string[] RunQuery(string dbName, string query, SqlServerManager transactionOwner)
        {
            string[] results = null;

            try
            {
                if (transactionOwner == null)
                {
                    string strConnection = GetConnectionString(dbName);

                    using (SqlConnection connection = new SqlConnection(strConnection))
                    {
                        connection.Open();
                        
                        SqlCommand cmd = new SqlCommand(query, connection);

                        if (IsSelectQuery(query))
                            results = SelectQuery(cmd);
                        else
                            results = ExecuteQuery(cmd);
                    }
                }
                else
                {
                    SqlCommand cmd = new SqlCommand(query, transactionOwner.m_connection, transactionOwner.m_transaction);

                    if (IsSelectQuery(query))
                        results = SelectQuery(cmd);
                    else
                        results = ExecuteQuery(cmd);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write("RunQuery : " + query);
                return WebDBService.ErrorMessage(e.Message);
            }

            return results;
        }

        public static string[] RunMultiQuery(string dbName, string query, SqlServerManager transactionOwner)
        {
            string[] results = null;

            try
            {
                if (transactionOwner == null)
                {
                    string strConnection = GetConnectionString(dbName);

                    using (SqlConnection connection = new SqlConnection(strConnection))
                    {
                        connection.Open();

                        SqlCommand cmd = new SqlCommand(query, connection);
                        results = ExecuteQuery(cmd);
                    }
                }
                else
                {
                    SqlCommand cmd = new SqlCommand(query, transactionOwner.m_connection, transactionOwner.m_transaction);
                    results = ExecuteQuery(cmd);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write("RunMultiQuery : " + query);
                return WebDBService.ErrorMessage(e.Message);
            }

            return results;
        }

        public static SqlServerManager BeginTransaction(string dbName, out string strErrorMessage)
        {
            strErrorMessage = "";
            string strConnection = GetConnectionString(dbName);

            SqlServerManager transactionOwner = new SqlServerManager();

            try
            {
                transactionOwner.m_connection = new SqlConnection(strConnection);
                transactionOwner.m_connection.Open();
                transactionOwner.m_transaction = transactionOwner.m_connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
            }
            catch (Exception e)
            {
                Logger.Instance.Write("BeginTransaction Fail : " + dbName);
                strErrorMessage = e.Message;
                return null;
            }

            transactionOwner.CreateTime = DateTime.Now;
            return transactionOwner;
        }

        public override string BatchCommit()
        {
            if (m_connection == null)
            {
                Logger.Instance.Write("BatchCommit");
                return WebDBService.ErrorMessage2("DB 연결이 끊어졌거나 유효하지 않습니다.");
            }

            if (m_transaction == null)
            {
                try
                {
                    m_connection.Close();
                }
                catch (Exception)
                {
                }

                m_connection = null;
                Logger.Instance.Write("BatchCommit");
                return WebDBService.ErrorMessage2("커밋할 Transaction이 존재하지 않습니다.");
            }

            string strError = "";

            try
            {
                m_transaction.Commit();
                m_connection.Close();
            }
            catch (Exception e)
            {
                strError = e.Message;
                Logger.Instance.Write("BatchCommit : " + strError);
            }

            m_transaction = null;
            m_connection = null;
            return strError;
        }

        public override string BatchRollback()
        {
            if (m_connection == null)
            {
                Logger.Instance.Write("BatchRollback");
                return WebDBService.ErrorMessage2("DB 연결이 끊어졌거나 유효하지 않습니다.");
            }

            if (m_transaction == null)
            {
                try
                {
                    m_connection.Close();
                }
                catch (Exception)
                {
                }

                m_connection = null;
                Logger.Instance.Write("BatchRollback");
                return WebDBService.ErrorMessage2("롤백할 Transaction이 존재하지 않습니다.");
            }

            string strError = "";

            try
            {
                m_transaction.Rollback();
                m_connection.Close();
            }
            catch (Exception e)
            {
                strError = e.Message;
                Logger.Instance.Write("BatchRollback : " + strError);
            }

            m_transaction = null;
            m_connection = null;
            return strError;
        }

        private static string[] ExecuteQuery(SqlCommand cmd)
        {
            cmd.ExecuteNonQuery();
            return MakeSuccess(null);
        }

        private static string[] SelectQuery(SqlCommand cmd)
        {
            SqlDataReader reader = cmd.ExecuteReader();
            List<string> datas = new List<string>();

            int nColumnCount = reader.FieldCount;

            while (reader.Read())
            {
                for (int i = 0; i < nColumnCount; i++)
                {
                    if (reader.IsDBNull(i))
                        AddNullData(datas);
                    else
                        AddData(datas, reader.GetValue(i));
                }
            }

            reader.Close();
            return MakeSuccess(datas);
        }
    }
}