using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Diagnostics;

namespace DBUtility2
{
    using WebDBService;

    public class WebDBManager : IDisposable
    {
        public enum DBType { sqlserver = 0, mysql, sqlite, TypeCount };

        private const string NOT_CONNECTED_EXCEPTION = "WebDB 접속이 끊어졌습니다.\r\n서버 관리자에게 문의하세요.";

        private int m_nSiteID = 0;
        private string m_strWebServerURL = "";
        private string m_strDatabaseName = "sop";
        private string m_strDBPath = "";
        private DBType m_dbType = DBType.sqlite;

        private string m_strLastErrorMsg = "";

        private object lockOjb = new object();
        private SQLiteTransaction tranc = null;

        private bool m_bBatch = false;
        private string m_strBatchDB = "sop";
        private SQLiteConnection batchConn;
        private SQLiteTransaction batchTranc;

        public DBType DatabaseType
        {
            get { return m_dbType; }
            set {  }
        }

        public string DatabaseTypeName
        {
            get { return m_dbType.ToString(); }
            set
            {
            }
        }

        public string DatabaseName
        {
            get { return m_strDatabaseName; }
            set {  }
        }

        public string WebServerURL
        {
            get { return m_strWebServerURL; }
            set { m_strWebServerURL = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public string LastErrorMessage
        {
            get { return m_strLastErrorMsg; }
        }

        private Utility m_ini = new Utility();

        public WebDBManager()
        {
            m_nSiteID = 1;
            LoadConnectionInfo(m_nSiteID);
        }

        public WebDBManager(int nSiteID)
        {
            m_nSiteID = nSiteID;
            LoadConnectionInfo(nSiteID);
        }

        public WebDBManager(string strDatabaseName, int nSiteID)
        {
            m_nSiteID = nSiteID;
            LoadConnectionInfo(nSiteID);
        }

        public WebDBManager(string strDatabaseName, string strDBType, int nSiteID)
        {
            m_nSiteID = nSiteID;
            LoadConnectionInfo(nSiteID);
        }

        public void Dispose()
        {
        }

        public WebDBManager Clone()
        {
            WebDBManager dbMgr = new WebDBManager();

            dbMgr.m_nSiteID = m_nSiteID;
            dbMgr.m_strDatabaseName = m_strDatabaseName;
            dbMgr.m_strWebServerURL = m_strWebServerURL;
            dbMgr.m_dbType = m_dbType;

            return dbMgr;
        }

        public void LoadConnectionInfo(int nSiteID)
        {
            string section = "Server Connection Info";
            
            string strLocalDB = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\LocalDB\\" + m_strDatabaseName + ".db";

            if (System.IO.File.Exists(strLocalDB))
                m_strDBPath = strLocalDB;
            else
                m_strDBPath = "C:\\ProgramData\\SOP\\" + m_strDatabaseName + ".db";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSQL">실행할 쿼리</param>
        /// <param name="nTranstion">1인경우 트랜잭션, 0이면 단일쿼리</param>
        /// <param name="nLimit">최대 행 개수</param>
        /// <param name="strDBName">사용할 Database이름</param>
        /// <returns></returns>
        public virtual ArrayList GetResultData(string strSQL, int nLimit, string strDBName = null)
        {
            if (nLimit > 0)
            {
                if (m_dbType == DBType.mysql)
                {
                    strSQL = strSQL + " LIMIT 0," + nLimit;
                }
                else if (m_dbType == DBType.sqlserver)
                {
                    int num = strSQL.ToLower().IndexOf("select");
                    if (num >= 0)
                    {
                        strSQL = strSQL.Insert(6, " TOP " + nLimit + " ");
                    }
                }
                else if (m_dbType == DBType.sqlite)
                {
                    strSQL = strSQL + " LIMIT " + nLimit;
                }
            }
            return GetResultData(strSQL, strDBName);
        }

        public virtual ArrayList GetResultData(string strSQL, string strDBName = null)
        {
            SQLiteConnection conn = Connect();

            if (conn == null)
                return null;

            strSQL = strSQL.Replace('\n', '\u0006');
            strSQL = strSQL.Replace('\r', '\a');
            if (DatabaseType == DBType.mysql)
            {
                strSQL = strSQL.Replace("\\", "\\\\");
            }
            if (strDBName == null)
            {
                strDBName = m_strDatabaseName;
            }

            ArrayList arrResult = GetResultLocalDB(conn, strSQL);

            conn.Close();
            conn = null;
            return arrResult;
        }

        private DBResultList GetResultLocalDB(SQLiteConnection conn, string szSQL)
        {
            lock (lockOjb)
            {
                if (conn != null)
                {
                    SQLiteCommand sqlCommand = new SQLiteCommand(szSQL, conn);
                    if (tranc != null)
                    {
                        sqlCommand.Transaction = tranc;
                    }
                    try
                    {
                        DBResultList dBResultList = new DBResultList();

                        if (szSQL.Trim().ToLower().StartsWith("select"))
                        {
                            SQLiteDataReader sqlDataReader = sqlCommand.ExecuteReader();
                            int num = 0;
                            while (sqlDataReader.Read())
                            {
                                dBResultList.Column = sqlDataReader.FieldCount;
                                if (sqlDataReader.FieldCount != 0)
                                {
                                    object[] array = new object[sqlDataReader.FieldCount];
                                    sqlDataReader.GetValues(array);
                                    dBResultList.AddRange(array);
                                }
                                num++;
                            }
                            sqlDataReader.Close();
                            dBResultList.Row = num;
                            dBResultList = ToStringArray(dBResultList);
                        }
                        else
                        {
                            sqlCommand.ExecuteNonQuery();
                        }

                        return dBResultList;
                    }
                    catch (Exception ex)
                    {
                        new StackTrace(ex, fNeedFileInfo: true);
                        Trace.WriteLine(ex.StackTrace);
                        return null;
                    }
                }
                return null;
            }
        }

        private DBResultList ToStringArray(DBResultList resultList)
        {
            for (int i = 0; i < resultList.Count; i++)
            {
                object obj = resultList[i];

                if (obj is string)
                    continue;
                else
                    resultList[i] = obj.ToString();
            }

            return resultList;
        }

        private SQLiteConnection Connect()
        {
            string strConnection = "Data Source=" + m_strDBPath + ";Version=3;Password=alone9966;";
            SQLiteConnection conn = new SQLiteConnection(strConnection);

            try
            {
                conn.Open();
                return conn;
            }
            catch (Exception e)
            {
                new StackTrace(e, fNeedFileInfo: true);
            }

            return null;
        }

        public virtual bool BeginBatch(string strDBName = null)
        {
            m_bBatch = true;
            batchConn = Connect();
            if (batchConn != null)
            {
                batchTranc = batchConn.BeginTransaction();
            }
            return true;
        }

        public virtual bool BatchCommit()
        {
            if (m_bBatch)
            {
                m_bBatch = false;
                try
                {
                    if (batchTranc != null)
                    {
                        batchTranc.Commit();
                        batchTranc.Dispose();
                        batchTranc = null;
                    }
                    if (batchConn != null)
                    {
                        batchConn.Close();
                        batchConn = null;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public virtual bool BatchRollback()
        {
            if (m_bBatch)
            {
                try
                {
                    if (batchTranc != null)
                    {
                        batchTranc.Rollback();
                        batchTranc.Dispose();
                        batchTranc = null;
                    }
                    if (batchConn != null)
                    {
                        batchConn.Close();
                        batchConn = null;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

                m_bBatch = false;
                return true;
            }

            return false;
        }

        public virtual ArrayList GetBatchData(string strSQL)
        {
            if (!m_bBatch)
            {
                return null;
            }

            strSQL = strSQL.Replace('\n', '\u0006');
            strSQL = strSQL.Replace('\r', '\a');
            return GetBatchResultLocalDB(batchConn, batchTranc, strSQL);
        }

        private DBResultList GetBatchResultLocalDB(SQLiteConnection connection, SQLiteTransaction tranction, string szSQL)
        {
            if (connection != null)
            {
                SQLiteCommand sqlCommand = new SQLiteCommand(szSQL, connection);
                if (tranction != null)
                {
                    sqlCommand.Transaction = tranction;
                }
                try
                {
                    DBResultList dBResultList = new DBResultList();
                    if (szSQL.ToLower().StartsWith("select"))
                    {
                        SQLiteDataReader sqlDataReader = sqlCommand.ExecuteReader();
                        int num = 0;
                        while (sqlDataReader.Read())
                        {
                            dBResultList.Column = sqlDataReader.FieldCount;
                            if (sqlDataReader.FieldCount != 0)
                            {
                                object[] array = new object[sqlDataReader.FieldCount];
                                sqlDataReader.GetValues(array);
                                dBResultList.AddRange(array);
                            }
                            num++;
                        }
                        sqlDataReader.Close();
                        sqlCommand.Transaction = null;
                        dBResultList.Row = num;
                        dBResultList = ToStringArray(dBResultList);
                        return dBResultList;
                    }
                    int num2 = sqlCommand.ExecuteNonQuery();
                    dBResultList.Add(num2);
                    dBResultList = ToStringArray(dBResultList);
                    return dBResultList;
                }
                catch (Exception ex)
                {
                    new StackTrace(ex, fNeedFileInfo: true);
                    Trace.WriteLine(ex.Message);
                    Trace.WriteLine(ex.StackTrace);
                    return null;
                }
            }
            return null;
        }

        static public int GetIntField(string dataSrc, int nDefault)
        {
            if (dataSrc != null && dataSrc.Length != 0 && !(dataSrc == "null"))
            {
                try
                {
                    if (string.Compare(dataSrc, "true", ignoreCase: true) == 0)
                    {
                        return 1;
                    }
                    if (string.Compare(dataSrc, "false", ignoreCase: true) == 0)
                    {
                        return 0;
                    }
                    return Convert.ToInt32(dataSrc);
                }
                catch (Exception)
                {
                    return nDefault;
                }
            }
            return nDefault;
        }

        static public VariousData<int> GetIntField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.Length == 0 || dataSrc == "null")
            {
                return null;
            }
            if (string.Compare(dataSrc, "true", ignoreCase: true) == 0)
            {
                return new VariousData<int>(1);
            }
            if (string.Compare(dataSrc, "false", ignoreCase: true) == 0)
            {
                return new VariousData<int>(0);
            }

            int result = 0;

            if (int.TryParse(dataSrc, out result))
            {
                return new VariousData<int>(result);
            }
            return null;
        }

        static public float GetFloatField(string dataSrc, float fDefault)
        {
            if (dataSrc != null && dataSrc.Length != 0 && !(dataSrc == "null"))
            {
                try
                {
                    return Convert.ToSingle(dataSrc);
                }
                catch (Exception)
                {
                    return fDefault;
                }
            }
            return fDefault;
        }

        static public VariousData<float> GetFloatField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.Length == 0 || dataSrc == "null")
            {
                return null;
            }

            float result = 0.0f;

            if (float.TryParse(dataSrc, out result))
            {
                return new VariousData<float>(result);
            }
            return null;
        }

        static public DateTime GetDateTimeField(object dataSrc, DateTime dtDefault)
        {
            if (dataSrc == DBNull.Value)
            {
                return dtDefault;
            }
            if (!dataSrc.Equals("") && !dataSrc.Equals("null"))
            {
                try
                {
                    return Convert.ToDateTime(dataSrc);
                }
                catch (Exception)
                {
                    return dtDefault;
                }
            }
            return dtDefault;
        }

        static public VariousData<DateTime> GetDateTimeField(object dataSrc)
        {
            if (dataSrc == DBNull.Value)
            {
                return null;
            }
            if (dataSrc == null || dataSrc.Equals("") || dataSrc.Equals("null"))
            {
                return null;
            }
            try
            {
                DateTime data = Convert.ToDateTime(dataSrc);
                return new VariousData<DateTime>(data);
            }
            catch (Exception)
            {
            }
            return null;
        }

        // 문자열 앞뒤의 빈문자들을 제거한다.
        static public string GetStringField(object dataSrc, string strDefault)
        {
            if (dataSrc == null)
            {
                return strDefault;
            }
            if (dataSrc == DBNull.Value)
            {
                return strDefault;
            }
            if (dataSrc.GetType() == typeof(string))
            {
                try
                {
                    string text = (string)dataSrc;
                    text = text.TrimStart(' ', '\t', '\r', '\n');
                    text = text.TrimEnd(' ', '\t', '\r', '\n');
                    text = text.Replace('\u0006', '\n');
                    text = text.Replace('\a', '\r');
                    return text.Replace('\b', '\'');
                }
                catch (Exception)
                {
                    return strDefault;
                }
            }
            return dataSrc.ToString();
        }

        // 문자열 앞뒤의 빈문자들을 제거한다.
        static public string GetStringField(object dataSrc)
        {
            if (dataSrc == null)
            {
                return null;
            }
            if (dataSrc == DBNull.Value)
            {
                return null;
            }
            if (dataSrc.GetType() != typeof(string))
            {
                return dataSrc.ToString();
            }
            if (!dataSrc.Equals("null"))
            {
                string text = null;
                try
                {
                    text = (string)dataSrc;
                    text = text.TrimStart(' ', '\t', '\r', '\n');
                    text = text.TrimEnd(' ', '\t', '\r', '\n');
                    text = text.Replace('\u0006', '\n');
                    text = text.Replace('\a', '\r');
                    return text.Replace('\b', '\'');
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static string MakeDateTimeString(DateTime time)
        {
            return string.Format("{0} {1:00}:{2:00}:{3:00}", time.ToShortDateString(), time.Hour, time.Minute, time.Second);
        }

        /// <summary>
		/// INI파일에서 항목 가져오기
		/// </summary>
        public virtual string LoadIni(string strTargetName)
        {
            string strSection = "Server Connection Info";
            return m_ini.getinivalue(strSection, strTargetName);
        }

        public virtual string LoadIni(string strTargetName, string strSectionName)
        {
            return m_ini.getinivalue(strSectionName, strTargetName);
        }

        public virtual string SaveIni(string strTargetName, string strValue, string strSectionName)
        {
            return m_ini.setinivalue(strSectionName, strTargetName, strValue);
        }
    }
}
