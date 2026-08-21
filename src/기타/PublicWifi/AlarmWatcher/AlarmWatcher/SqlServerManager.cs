using System;
using System.Data.SqlClient;
using System.Collections;

namespace AlarmWatcher
{
    public class SqlServerManager
    {
        private SqlTransaction m_transaction = null;
        private SqlConnection m_connection = null;
        private bool m_isConnected = false;
        private string m_strErrorMessage = "";
        private string m_strHost = "", m_strID = "", m_strPW = "", m_strDBName = "";

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public SqlServerManager(string strHost, string strID, string strPW, string strDBName)
        {
            m_strHost = strHost;
            m_strID = strID;
            m_strPW = strPW;
            m_strDBName = strDBName;
        }

        public bool Connect()
        {
            if (m_isConnected)
                return true;

            m_strErrorMessage = "";
            string strConnection = GetConnectionString();

            try
            {
                SqlConnection connection = new SqlConnection(strConnection);
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                    m_connection = connection;
                else
                {
                    m_strErrorMessage = "DB 접속에 실패하였습니다.\r\n" + strConnection;
                    return false;
                }
            }
            catch (Exception e)
            {
                m_connection = null;
                m_strErrorMessage = e.Message;
                return false;
            }

            m_isConnected = true;
            return true;
        }

        private string GetConnectionString()
        {
            return string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};", m_strHost, m_strDBName, m_strID, m_strPW);
        }

        private ArrayList RunQuery(string strSQL, SqlTransaction transaction)
        {
            m_strErrorMessage = "";

            if (m_connection == null || m_connection.State != System.Data.ConnectionState.Open)
            {
                m_strErrorMessage = "DB와의 연결이 끊어졌습니다.";
                return null;
            }

            ArrayList results = null;

            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, m_connection, transaction);

                if (IsSelectQuery(strSQL))
                    results = SelectQuery(cmd);
                else
                    results = ExecuteQuery(cmd);
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return null;
            }

            return results;
        }

        private static bool IsSelectQuery(string strSQL)
        {
            strSQL = strSQL.Trim().ToLower();
            return strSQL.StartsWith("select");
        }

        public ArrayList GetResultData(string strSQL)
        {
            return RunQuery(strSQL, null);
        }

        public bool BeginBatch()
        {
            m_strErrorMessage = "";

            if (m_transaction != null)
            {
                m_strErrorMessage = "이전 트랜잭션이 종료되지 않았습니다.\nRollback이나 Commit이후에 호출 가능합니다.";
                return false;
            }

            if (m_connection == null || m_connection.State != System.Data.ConnectionState.Open)
            {
                m_strErrorMessage = "DB와의 연결이 끊어졌습니다.";
                m_transaction = null;
                return false;
            }

            try
            {
                m_transaction = m_connection.BeginTransaction();
            }
            catch (Exception e)
            {
                m_transaction = null;
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        public ArrayList GetBatchData(string strSQL)
        {
            if (m_transaction == null)
            {
                m_strErrorMessage = "먼저 BeginTransaction()을 호출하세요.";
                return null;
            }

            return RunQuery(strSQL, m_transaction);
        }

        public bool BatchCommit()
        {
            if (m_connection == null)
            {
                m_strErrorMessage = "DB 연결이 끊어졌거나 유효하지 않습니다.";
                return false;
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
                m_strErrorMessage = "커밋할 Transaction이 존재하지 않습니다.";
                return false;
            }

            try
            {
                m_transaction.Commit();
                m_connection.Close();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            m_transaction = null;
            m_connection = null;
            return true;
        }

        public bool BatchRollback()
        {
            if (m_connection == null)
            {
                m_strErrorMessage = "DB 연결이 끊어졌거나 유효하지 않습니다.";
                return false;
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
                m_strErrorMessage = "롤백할 Transaction이 존재하지 않습니다.";
                return false;
            }

            try
            {
                m_transaction.Rollback();
                m_connection.Close();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            m_transaction = null;
            m_connection = null;
            return true;
        }

        private static ArrayList ExecuteQuery(SqlCommand cmd)
        {
            cmd.ExecuteNonQuery();
            return new ArrayList();
        }

        private static ArrayList SelectQuery(SqlCommand cmd)
        {
            SqlDataReader reader = cmd.ExecuteReader();
            ArrayList datas = new ArrayList();

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
            return datas;
        }

        protected static void AddNullData(ArrayList datas)
        {
            datas.Add("~");
        }

        protected static void AddData(ArrayList datas, object data)
        {
            datas.Add("!" + data.ToString());
        }

        public void Close()
        {
            if (m_connection != null)
                m_connection.Close();

            m_connection = null;
            m_isConnected = false;
        }
    }
}
