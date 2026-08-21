using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Data.SQLite;
using System.Data;
using System.Collections;

namespace SOPHiddenServer
{
    public class DBManager : DBUtility.WebDBManager
    {
        private SQLiteConnection m_connection = null;
        private SQLiteTransaction m_transaction = null;
        private SQLiteCommand m_command = null;
        private int m_nPort = 0;

        public int PortNo
        {
            set { m_nPort = value; }
        }

        public DBManager(int nSiteID)
            : base(nSiteID)
        {
        }

        public bool Open(string strDBFilePath, string strPassword)
        {
            string strConnection = "Data Source=" + strDBFilePath;
            
            if (strPassword.Length > 0)
                strConnection += ";Password=" + strPassword;

            try
            {
                m_connection = new SQLiteConnection(strConnection);
                m_connection.Open();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                m_connection = null;
                return false;
            }

            return true;
        }

        public void Close()
        {
            if (m_connection != null)
            {
                m_connection.Close();
                m_connection = null;
            }
        }

        public override void BatchCommit()
        {
            if (m_transaction != null)
                m_transaction.Commit();
            
            m_transaction = null;
            m_command = null;
        }

        public override void BatchRollback()
        {
            if (m_transaction != null)
                m_transaction.Rollback();

            m_transaction = null;
            m_command = null;
        }

        public override bool BeginBatch(string szDBName = null)
        {
            m_transaction = m_connection.BeginTransaction();
            return m_transaction != null;
        }

        public override ArrayList GetResultData(string strSQLQuery, int nTransaction, string szDBName = null)
        {
            if (m_connection == null)
                return null;

            SQLiteCommand cmd = null;

            if (m_transaction != null)
            {
                if (m_command == null)
                {
                    m_command = m_connection.CreateCommand();
                    m_command.Transaction = m_transaction;
                }

                cmd = m_command;
            }
            else
                cmd = m_connection.CreateCommand();

            string strSQL = strSQLQuery.ToLower();

            // MS-SQL 문법을 SQLite 문법으로 변환
            ToSQLiteQuery(ref strSQL, ref strSQLQuery);

            cmd.CommandText = strSQLQuery;

            if (strSQL.StartsWith("select "))
            {
                ArrayList arrResult = null;

                try
                {
                    arrResult = GetResultData(cmd);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                return arrResult;
            }

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }

            return new ArrayList();
        }

        private void ToSQLiteQuery(ref string strSQLLower, ref string strSQLOrigin)
        {
            while (true)
            {
                string strTag = "dateadd";
                int nIndex = strSQLLower.IndexOf(strTag);

                if (nIndex < 0)
                    break;

                int nFullLength = strSQLLower.Length;
                int nBeginIndex = -1, nEndIndex = -1, nOpenCount = 0;

                for (int i=nIndex+strTag.Length;i<nFullLength;i++)
                {
                    char ch = strSQLLower.ElementAt(i);

                    if (ch == '(')
                    {
                        if (nBeginIndex < 0)
                            nBeginIndex = i;
                        nOpenCount++;
                    }
                    else if (ch == ')')
                    {
                        nOpenCount--;

                        if (nBeginIndex >= 0 && nOpenCount == 0)
                        {
                            nEndIndex = i;
                            break;
                        }
                    }
                }

                if (nBeginIndex < 0 || nEndIndex < 0)
                    break;

                string strDateAdd = strSQLLower.Substring(nIndex, nEndIndex - nIndex + 1);

                if (!ToDateTime(ref strDateAdd, nBeginIndex - nIndex))
                    break;

                strSQLLower = strSQLLower.Substring(0, nIndex) + strDateAdd + strSQLLower.Substring(nEndIndex + 1);
                strSQLOrigin = strSQLOrigin.Substring(0, nIndex) + strDateAdd + strSQLOrigin.Substring(nEndIndex + 1);
            }

            while (true)
            {
                string strTag = "getdate()";
                int nTagLen = strTag.Length;

                int nIndex = strSQLLower.IndexOf(strTag);

                if (nIndex < 0)
                    break;

                strSQLLower = strSQLLower.Substring(0, nIndex) + "datetime('now')" + strSQLLower.Substring(nIndex + nTagLen);
                strSQLOrigin = strSQLOrigin.Substring(0, nIndex) + "datetime('now')" + strSQLOrigin.Substring(nIndex + nTagLen);
            }
        }

        private bool ToDateTime(ref string strDateAdd, int nBeginIndex)
        {
            int nIndex1 = strDateAdd.IndexOf(',');
            int nIndex2 = strDateAdd.LastIndexOf(',');

            if (nIndex1 < 0 || nIndex2 < 0)
                return false;

            string strNumber = strDateAdd.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            strNumber = strNumber.Trim();

            if (strNumber.Length == 0)
                return false;

            char ch = strNumber.ElementAt(0);

            if (ch != '+' && ch != '-')
                strNumber = "+" + strNumber;

            string strUnit = strDateAdd.Substring(nBeginIndex + 1, nIndex1 - nBeginIndex - 1);
            strUnit = strUnit.Trim();

            strDateAdd = "datetime('now', '" + strNumber + " " + strUnit + "')";
            return true;
        }

        private ArrayList GetResultData(SQLiteCommand cmd)
        {            
            SQLiteDataReader reader = cmd.ExecuteReader();

            if (reader == null)
                return null;

            ArrayList arrResult = new ArrayList();

            while (reader.Read())
            {
                for (int i=0;i<reader.FieldCount;i++)
                {
                    if (reader.IsDBNull(i))
                        arrResult.Add("");
                    else
                        arrResult.Add(reader[i].ToString());
                }
            }

            reader.Close();
            return arrResult;
        }

        public override string LoadIni(string strTargetName)
        {
            if (strTargetName == "sdms_port")
                return m_nPort.ToString();
            else if (strTargetName == "run_team_reader")
                return "0";

            return base.LoadIni(strTargetName);
        }

        public override string LoadIni(string strTargetName, string strSectionName)
        {
            if (strTargetName == "sdms_port")
                return m_nPort.ToString();
            else if (strTargetName == "run_team_reader")
                return "0";

            return base.LoadIni(strTargetName, strSectionName);
        }
    }
}
