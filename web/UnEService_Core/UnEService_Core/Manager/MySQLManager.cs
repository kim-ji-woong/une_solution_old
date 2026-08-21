using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using UnEService_Core.Common;
using UnEService_Core.Models;
using UnEService_Core.Service;

namespace UnEService_Core.Manager
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
                    //strConnection = "server=localhost;port=3306;database=test;user=root;password=test;CharSet=utf8";
                    
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

        public static string[] RunStoredProcedure(string dbName, string procedureName, List<string> fieldNames, List<string> fieldValues, MySQLManager transactionOwner)
        {
            string[] results = null;

            try
            {
                int nFieldCount = fieldNames.Count;

                if (nFieldCount != fieldValues.Count)
                {
                    Logger.Instance.Write("RunStoredProcedure : Parameter 오류, field 이름과 value의 개수가 일치하지 않습니다.");
                    return WebDBService.ErrorMessage("RunStoredProcedure : Parameter 오류, field 이름과 value의 개수가 일치하지 않습니다.");
                }

                if (transactionOwner == null)
                {
                    string strConnection = GetConnectionString(dbName);
                    
                    using (MySqlConnection connection = new MySqlConnection(strConnection))
                    {
                        connection.Open();

                        MySqlCommand cmd = new MySqlCommand(procedureName, connection);
                        results = GetStoredProcedureResults(cmd, nFieldCount, fieldNames, fieldValues);
                    }
                }
                else
                {
                    MySqlCommand cmd = new MySqlCommand(procedureName, transactionOwner.m_connection, transactionOwner.m_transaction);
                    results = GetStoredProcedureResults(cmd, nFieldCount, fieldNames, fieldValues);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write("RunStoredProcedure : " + procedureName + "(...) : " + e.Message);
                return WebDBService.ErrorMessage(e.Message);
            }

            return results;
        }

        private static string[] GetStoredProcedureResults(MySqlCommand cmd, int nFieldCount, List<string> fieldNames, List<string> fieldValues)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;

            for (int i = 0; i < nFieldCount; i++)
            {
                string strFieldName = fieldNames[i];
                string strFieldValue = fieldValues[i];

                if (strFieldValue.StartsWith("i"))
                {
                    int data;

                    if (int.TryParse(strFieldValue.Substring(1), out data))
                        cmd.Parameters.Add(new MySqlParameter(strFieldName, data));
                }
                else if (strFieldValue.StartsWith("s"))
                {
                    cmd.Parameters.Add(new MySqlParameter(strFieldName, strFieldValue.Substring(1)));
                }
                else if (strFieldValue.StartsWith("f"))
                {
                    float data;

                    if (float.TryParse(strFieldValue.Substring(1), out data))
                        cmd.Parameters.Add(new MySqlParameter(strFieldName, data));
                }
            }

            return SelectQuery(cmd);
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
