using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace BIMViewer.DB
{
    public class _SqlConnection
    {
        public enum DataBaseType { Unknown = 0, SQLServer, SQLite };

        private DataBaseType m_dbType = DataBaseType.SQLServer;
        private SqlConnection m_connection = null;
        private SQLiteConnection m_connectionLite = null;

        public DataBaseType DBType
        {
            get { return m_dbType; }
        }

        public SqlConnection SqlConnection
        {
            get { return m_connection; }
        }

        public SQLiteConnection SQLiteConnection
        {
            get { return m_connectionLite; }
        }

        public _SqlConnection(DataBaseType  dbType, string strConnection)
        {
            if (dbType == DataBaseType.SQLServer)
                m_connection = new SqlConnection(strConnection);
            else if (dbType == DataBaseType.SQLite)
                m_connectionLite = new SQLiteConnection(strConnection);

            m_dbType = dbType;
        }

        public void Open()
        {
            if (m_connection != null)
                m_connection.Open();
            else if (m_connectionLite != null)
                m_connectionLite.Open();
        }

        public void Close()
        {
            if (m_connection != null)
                m_connection.Close();
            else if (m_connectionLite != null)
                m_connectionLite.Close();
        }
    }

    public class _SqlDataReader
    {
        private SqlDataReader m_reader = null;
        private SQLiteDataReader m_readerLite = null;

        public _SqlDataReader(SqlDataReader reader)
        {
            m_reader = reader;
        }

        public _SqlDataReader(SQLiteDataReader reader)
        {
            m_readerLite = reader;
        }

        public bool IsDBNull(int index)
        {
            if (m_reader != null)
                return m_reader.IsDBNull(index);
            else if (m_readerLite != null)
                return m_readerLite.IsDBNull(index);

            return false;
        }

        public int GetInt32(int index)
        {
            if (m_reader != null)
                return m_reader.GetInt32(index);
            else if (m_readerLite != null)
                return m_readerLite.GetInt32(index);

            return 0;
        }

        public double GetDouble(int index)
        {
            if (m_reader != null)
                return m_reader.GetDouble(index);
            else if (m_readerLite != null)
                return m_readerLite.GetDouble(index);

            return 0;
        }

        public bool GetBoolean(int index)
        {
            if (m_reader != null)
                return m_reader.GetBoolean(index);
            else if (m_readerLite != null)
                return m_readerLite.GetBoolean(index);

            return false;
        }

        public string GetString(int index)
        {
            if (m_reader != null)
                return m_reader.GetString(index);
            else if (m_readerLite != null)
                return m_readerLite.GetString(index);

            return "";
        }

        public DateTime GetDateTime(int index)
        {
            if (m_reader != null)
                return m_reader.GetDateTime(index);
            else if (m_readerLite != null)
                return m_readerLite.GetDateTime(index);

            return new DateTime();
        }

        public object GetValue(int index)
        {
            if (m_reader != null)
                return m_reader.GetValue(index);
            else if (m_readerLite != null)
                return m_readerLite.GetValue(index);

            return null;
        }

        public bool Read()
        {
            if (m_reader != null)
                return m_reader.Read();
            else if (m_readerLite != null)
                return m_readerLite.Read();

            return false;
        }

        public void Close()
        {
            if (m_reader != null)
                m_reader.Close();
            else if (m_readerLite != null)
                m_readerLite.Close();
        }
    }

    public class _SqlTransaction
    {
        private SqlTransaction m_transaction = null;
        private SQLiteTransaction m_transactionLite = null;

        public SqlTransaction SqlTransaction
        {
            get { return m_transaction; }
        }

        public SQLiteTransaction SQLiteTransaction
        {
            get { return m_transactionLite; }
        }

        public _SqlTransaction(_SqlConnection.DataBaseType dbType, _SqlConnection con)
        {
            if (dbType == _SqlConnection.DataBaseType.SQLServer)
                m_transaction = con.SqlConnection.BeginTransaction();
            else if (dbType == _SqlConnection.DataBaseType.SQLite)
                m_transactionLite = con.SQLiteConnection.BeginTransaction();
        }

        public void Commit(_SqlConnection.DataBaseType dbType)
        {
            if (dbType == _SqlConnection.DataBaseType.SQLServer && m_transaction != null)
                m_transaction.Commit();
            else if (dbType == _SqlConnection.DataBaseType.SQLite && m_transactionLite != null)
                m_transactionLite.Commit();
        }

        public void Rollback(_SqlConnection.DataBaseType dbType)
        {
            if (dbType == _SqlConnection.DataBaseType.SQLServer && m_transaction != null)
                m_transaction.Rollback();
            else if (dbType == _SqlConnection.DataBaseType.SQLite && m_transactionLite != null)
                m_transactionLite.Rollback();
        }
    }

    public class _SqlCommand
    {
        private SqlCommand m_command = null;
        private SQLiteCommand m_commandLite = null;

        public _SqlCommand(string strSQL, _SqlConnection connection, _SqlTransaction transaction)
        {
            if (connection.DBType == _SqlConnection.DataBaseType.SQLServer)
            {
                if (transaction == null)
                    m_command = new SqlCommand(strSQL, connection.SqlConnection, null);
                else
                    m_command = new SqlCommand(strSQL, connection.SqlConnection, transaction.SqlTransaction);
            }
            else if (connection.DBType == _SqlConnection.DataBaseType.SQLite)
            {
                if (transaction == null)
                    m_commandLite = new SQLiteCommand(strSQL, connection.SQLiteConnection, null);
                else
                    m_commandLite = new SQLiteCommand(strSQL, connection.SQLiteConnection, transaction.SQLiteTransaction);
            }
        }

        public _SqlDataReader ExecuteReader()
        {
            if (m_command != null)
                return new _SqlDataReader(m_command.ExecuteReader());
            else if (m_commandLite != null)
                return new _SqlDataReader(m_commandLite.ExecuteReader());

            return null;
        }

        public int ExecuteNonQuery()
        {
            if (m_command != null)
                return m_command.ExecuteNonQuery();
            else if (m_commandLite != null)
                return m_commandLite.ExecuteNonQuery();

            return 0;
        }
    }
}
