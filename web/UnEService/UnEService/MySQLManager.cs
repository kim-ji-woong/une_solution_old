using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace UnEService
{
    public class MySQLManager : DBManager
    {
        private MySqlTransaction m_transaction = null;
        private MySqlConnection m_connection = null;

        private static string GetConnectionString(string strDBName)
        {
            return string.Format("Server={0};Database={1};Uid={2};Pwd={3};CharSet={4};", Host, strDBName, ID, PW, CharSet);
        }

        public static string[] RunQuery(string dbName, string query, MySQLManager transactionOwner)
        {
            string[] results = null;

            try
            {
                if (transactionOwner == null)
                {
                    string strConnection = GetConnectionString(dbName);

                    using (MySqlConnection connection = new MySqlConnection(strConnection))
                    {
                        connection.Open();

                        MySqlCommand cmd = new MySqlCommand(query, connection);

                        if (IsSelectQuery(query))
                            results = SelectQuery(cmd);
                        else
                            results = ExecuteQuery(cmd);
                    }
                }
                else
                {
                    MySqlCommand cmd = new MySqlCommand(query, transactionOwner.m_connection, transactionOwner.m_transaction);

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

        public static string[] RunMultiQuery(string dbName, string query, MySQLManager transactionOwner)
        {
            string[] results = null;

            try
            {
                if (transactionOwner == null)
                {
                    string strConnection = GetConnectionString(dbName);

                    using (MySqlConnection connection = new MySqlConnection(strConnection))
                    {
                        connection.Open();

                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        results = ExecuteQuery(cmd);
                    }
                }
                else
                {
                    MySqlCommand cmd = new MySqlCommand(query, transactionOwner.m_connection, transactionOwner.m_transaction);
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

        public static MySQLManager BeginTransaction(string dbName, out string strErrorMessage)
        {
            strErrorMessage = "";
            string strConnection = GetConnectionString(dbName);

            MySQLManager transactionOwner = new MySQLManager();

            try
            {
                transactionOwner.m_connection = new MySqlConnection(strConnection);
                transactionOwner.m_connection.Open();
                transactionOwner.m_transaction = transactionOwner.m_connection.BeginTransaction();
            }
            catch (Exception e)
            {
                Logger.Instance.Write("BeginTransaction : " + dbName);
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

        private static string[] ExecuteQuery(MySqlCommand cmd)
        {
            cmd.ExecuteNonQuery();
            return MakeSuccess(null);
        }

        private static string[] SelectQuery(MySqlCommand cmd)
        {
            MySqlDataReader reader = cmd.ExecuteReader();
            List<string> datas = new List<string>();

            int nColumnCount = reader.FieldCount;

            while (reader.Read())
            {
                for (int i=0;i<nColumnCount;i++)
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