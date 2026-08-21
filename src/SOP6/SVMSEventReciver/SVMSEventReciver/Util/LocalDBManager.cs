using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data;
using MySql.Data.MySqlClient;
using DBUtility2;

namespace DBUtility
{
    public class LocalDBManager : IDisposable
    {
        public enum DBType { sqlserver = 0, mysql, TypeCount };
        
        // 세션 유지용 쿠키
        private CookieContainer cookieContainer = new CookieContainer();
        private CookieContainer m_batchCookie = new CookieContainer();

        private Utility m_ini = new Utility();

        private string m_strWebServerURL = "";
        public string WebServerURL
        {
            get { return m_strWebServerURL; }
            set { m_strWebServerURL = value; }
        }

        private string m_szDBHost = "127.0.0.1";
        public string DatabaseHost
        {
            get { return m_szDBHost; }
            set { m_szDBHost = value; }
        }

        private string m_szDBPort = null;
        public string DatabasePort
        {
            get { return m_szDBPort; }
            set { m_szDBPort = value; }
        }

        private string m_szDatabaseName = "EDU_100";
        public string DatabaseName
        {
            get { return m_szDatabaseName; }
            set { m_szDatabaseName = value; }
        }

        private DBType m_dbType = DBType.mysql;
        public DBType DatabaseType
        {
            get { return m_dbType; }
            set { m_dbType = value; }
        }

        public string DatabaseTypeName
        {
            get { return m_dbType.ToString(); }
            set
            {
                if (string.Compare(value, "mysql", true) == 0)
                    m_dbType = DBType.mysql;
                else if (string.Compare(value, "sqlserver", true) == 0)
                    m_dbType = DBType.sqlserver;
            }
        }
        /*private string m_szDatabaseType = "sqlserver";
        public string DatabaseType
        {
            get { return m_szDatabaseType; }
            set { m_szDatabaseType = value; }
        }*/

        private string m_lastResult = "";
        public string LastResult
        {
            get { return m_lastResult; }
        }

        private string m_szLastError = "";
        public string LastError
        {
            get { return m_szLastError; }
        }

        private string m_szLastErrorMsg = "";
        public string LastErrorMessage
        {
            get { return m_szLastErrorMsg; }
        }

        public LocalDBManager(string strDatabaseName, string strDBType, int nSiteID)
        {
            Loadini_ServerConnectionInfo(nSiteID);
            m_szDatabaseName = strDatabaseName;
            this.DatabaseTypeName = strDBType;

            conn = Connect();
        }

        static public T GetField<T>(object dataSrc, T dataDefault)
        {
            if (dataSrc == DBNull.Value)
                return dataDefault;
            T result;

            try
            {
                result = (T)dataSrc;
            }
            catch (Exception)
            {
                result = dataDefault;
            }

            return result;
        }

        static public float GetFloatField(string dataSrc, float fDefault)
        {
            float result = fDefault;

            if (dataSrc == null || dataSrc.Length == 0 || dataSrc == "null")
                return result;

            try
            {
                result = Convert.ToSingle(dataSrc);
                //float.TryParse(dataSrc, out result);
            }
            catch (Exception)
            {
                result = fDefault;
            }

            return result;
        }

        // 문자열 앞뒤의 빈문자들을 제거한다.
        static public string GetStringField(object dataSrc, string strDefault)
        {
            string result = strDefault;

            if (dataSrc == null)
                return strDefault;

            if (dataSrc == DBNull.Value)
                return strDefault;

            if (dataSrc.GetType() != typeof(string))
            {
                return dataSrc.ToString();
            }

            try
            {
                result = (string)dataSrc;
                result = result.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                result = result.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                // (char)6, 7, 8은 DB 입력시 '\n', '\r', '\''이 임시로 바뀌어 들어간 값이므로, 다시 '\n'으로 되돌려 준다.
                result = result.Replace((char)6, '\n');
                result = result.Replace((char)7, '\r');
                result = result.Replace((char)8, '\'');
            }
            catch (Exception)
            {
                result = strDefault;
            }

            return result;
        }

        static public DateTime GetDateTimeField(object dataSrc, DateTime dtDefault)
        {
            DateTime result = dtDefault;
            if (dataSrc == DBNull.Value)
                return result;

            if (dataSrc.Equals("") || dataSrc.Equals("null"))
                return result;

            try
            {
                result = Convert.ToDateTime(dataSrc);
            }
            catch (Exception)
            {
                result = dtDefault;
            }

            return result;
        }

        static public int GetIntField(string dataSrc, int nDefault)
        {
            int result = nDefault;
            if (dataSrc == null || dataSrc.Length == 0 || dataSrc == "null")
            {
                return result;
            }
            try
            {
                if (string.Compare(dataSrc, "true", true) == 0)
                    return 1;
                else if (string.Compare(dataSrc, "false", true) == 0)
                    return 0;

                result = Convert.ToInt32(dataSrc);
                //int.TryParse(dataSrc, out result);
            }
            catch (Exception)
            {
                result = nDefault;
            }

            return result;
        }

        static public VariousData<int> GetIntField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.Length == 0 || dataSrc == "null")
                return null;

            if (string.Compare(dataSrc, "true", true) == 0)
                return new VariousData<int>(1);
            else if (string.Compare(dataSrc, "false", true) == 0)
                return new VariousData<int>(0);

            int num;

            if (int.TryParse(dataSrc, out num))
                return new VariousData<int>(num);

            return null;
        }

        static public VariousData<float> GetFloatField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.Length == 0 || dataSrc == "null")
                return null;

            float num;

            if (float.TryParse(dataSrc, out num))
                return new VariousData<float>(num);

            return null;
        }

        static public VariousData<DateTime> GetDateTimeField(object dataSrc)
        {
            if (dataSrc == DBNull.Value)
                return null;

            if (dataSrc == null || dataSrc.Equals("") || dataSrc.Equals("null"))
                return null;

            try
            {
                DateTime time = Convert.ToDateTime(dataSrc);
                return new VariousData<DateTime>(time);
            }
            catch (Exception)
            {
            }

            return null;
        }

        // 문자열 앞뒤의 빈문자들을 제거한다.
        static public string GetStringField(object dataSrc)
        {
            if (dataSrc == null)
                return null;

            if (dataSrc == DBNull.Value)
                return null;


            if (dataSrc.GetType() != typeof(string))
            {
                return dataSrc.ToString();
            }

            if (dataSrc.Equals("null"))
                return null;

            string result = null;

            try
            {
                result = (string)dataSrc;
                result = result.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                result = result.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                // (char)6, 7, 8은 DB 입력시 '\n', '\r', '\''이 임시로 바뀌어 들어간 값이므로, 다시 '\n'으로 되돌려 준다.
                result = result.Replace((char)6, '\n');
                result = result.Replace((char)7, '\r');
                result = result.Replace((char)8, '\'');
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        private static char ConvertToHex(char cSource)
        {
            return "0123456789abcdef"[0x0f & cSource];
        }

        public static string URLEncoding(byte[] bytes)
        {
            string strResult = "";

            foreach (byte element in bytes)
            {
                if ((element >= '0' && element <= '9') ||   // 숫자
                    (element >= 'a' && element <= 'z') ||   // 소문자
                    (element >= 'A' && element <= 'Z') ||   // 대문자
                    (element == '!' || element == '*' || element == '(' || element == ')' || element == '_' || element == '-')) // 그 외의 특수기호들
                {
                    strResult += (char)element;
                }
                else
                {
                    strResult += "%";
                    strResult += ConvertToHex((char)((int)element >> 4));
                    strResult += ConvertToHex((char)element);
                }
            }
            return strResult;
        }

        public static string MakeDateTimeString(DateTime time)
        {
            return string.Format("{0} {1:00}:{2:00}:{3:00}", time.ToShortDateString(), time.Hour, time.Minute, time.Second);
        }

        // 해당문자열을 ``으로 감싸서 반환한다 (strQuary:DB이름이나 필드명)
        public string Grave(object obj)
        {
            return "`" + obj.ToString() + "`";
        }

        public void Dispose()
        {
            // try
            {
                //GetReadDB("close", 0, m_szDatabaseName);
            }

        }

        private string GetDBHostFromWebSererURL(string strServerURL)
        {
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

        /// <summary>
        /// Registry에서 서버 URL가져오기
        /// </summary>
        public void Loadini_ServerConnectionInfo(int nSiteID)
        {
            string strSection = "Server Connection Info";

            m_strWebServerURL = RegUtil.ReadRegValue(strSection, "webserver_url", nSiteID);
            if (m_strWebServerURL == null || m_strWebServerURL == "")
            {
                SetDefaultWebServerURL(nSiteID);
                RegUtil.WriteRegValue(strSection, "webserver_url", m_strWebServerURL, nSiteID);
            }

            if (m_strWebServerURL != null && m_strWebServerURL.Length > 0)
                this.DatabaseHost = GetDBHostFromWebSererURL(m_strWebServerURL);

            m_szDatabaseName = RegUtil.ReadRegValue(strSection, "db_name", nSiteID);

            if (m_szDatabaseName == null || m_szDatabaseName == "")
            {
                SetDefaultDBName(nSiteID);
                RegUtil.WriteRegValue(strSection, "db_name", m_szDatabaseName, nSiteID);
            }

            bool findDBType = false;
            string strDBType = RegUtil.ReadRegValue(strSection, "db_type", nSiteID);

            if (strDBType != null && strDBType != "")
            {
                int nDBType;

                if (int.TryParse(strDBType, out nDBType))
                {
                    if (nDBType >= 0 && nDBType < (int)DBType.TypeCount)
                    {
                        this.DatabaseType = (DBType)nDBType;
                        findDBType = true;
                    }
                }

                //this.DatabaseTypeName = strDBType;
            }

            if (!findDBType)
                RegUtil.WriteRegValue(strSection, "db_type", ((int)this.DatabaseType).ToString(), nSiteID);

            string szEncoding = RegUtil.ReadRegValue(strSection, "page_encoding", nSiteID);
            if (szEncoding == null || szEncoding == "")
            {
                szEncoding = SetDefaultEncoding(nSiteID);
                RegUtil.WriteRegValue(strSection, "page_encoding", szEncoding, nSiteID);
            }
            try
            {
                int nEncoding = -1;
                if (int.TryParse(szEncoding, out nEncoding))
                {
                    m_PageEncoding = System.Text.Encoding.GetEncoding(nEncoding);
                }
                else
                {
                    m_PageEncoding = Encoding.UTF8;
                }
            }
            catch (Exception)
            {
                m_PageEncoding = Encoding.UTF8;
            }

        }

        private System.Text.Encoding m_PageEncoding = null;
        private string SetDefaultEncoding(int nSiteID)
        {
            m_PageEncoding = System.Text.Encoding.GetEncoding(51949);
            string szResult = m_PageEncoding.CodePage.ToString();
            switch (nSiteID)
            {
                // 삼천포
                case 1:
                    szResult = Encoding.UTF8.CodePage.ToString();
                    break;

                // 영흥
                case 2:
                    szResult = Encoding.UTF8.CodePage.ToString();
                    break;

                default:
                    szResult = "";
                    break;
            }
            return szResult;
        }

        private void SetDefaultWebServerURL(int nSiteID)
        {
            switch (nSiteID)
            {
                // 삼천포
                case 1:
                    m_strWebServerURL = "http://172.18.101.50:8080/SOP";
                    break;

                // 영흥
                case 2:
                    m_strWebServerURL = "http://172.20.127.150:8080/SOP";
                    break;

                default:
                    m_strWebServerURL = "";
                    break;
            }
        }

        private void SetDefaultDBName(int nSiteID)
        {
            m_szDatabaseName = "SOP_" + nSiteID.ToString();
        }

        public virtual ArrayList GetResultData(string strSQLQuery, int nAutoCommit, string szDBName = null)
        {
            // str에 '\n', '\r'이 포함되어 있으면 다른 문자로 바꾼다.
            strSQLQuery = strSQLQuery.Replace('\n', (char)6);
            strSQLQuery = strSQLQuery.Replace('\r', (char)7);

            if (this.DatabaseType == DBType.mysql)
                strSQLQuery = strSQLQuery.Replace("\\", "\\\\");

            if (szDBName == null)
                szDBName = m_szDatabaseName;

            DBResultList arrResult = GetResultLocalDB(strSQLQuery, nAutoCommit);
            return arrResult;
        }
            
        private MySqlConnection Connect()
        {
            //Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;CharSet = utf8;

            string strConnection = "Server=127.0.0.1;Database=" + m_szDatabaseName + ";Uid=sa;Pwd=9449966Ab;CharSet=utf8;";
            //string strConnection = "Server=192.168.250.41;Database=" + m_szDatabaseName + ";Uid=sa;Pwd=9449966Ab;CharSet=utf8;";

            MySqlConnection conn = new MySqlConnection(strConnection);
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                return null;
            }
            return conn;
        }

        private MySqlConnection conn;
        private MySqlTransaction tranc;
        ///////////////////////////////////////////////////////////////////////////////////
        // Batch Mode function
        ///////////////////////////////////////////////////////////////////////////////////

        private bool m_bBatch = false;
        private string m_szBatchDB = "EDU_100";

        private MySqlConnection batchConn;
        private MySqlTransaction batchTranc;
        
        
        private object lockOjb = new object();
        public DBResultList GetResultLocalDB(string szSQL, int nTranc)
        {
            lock (lockOjb)
            {
                //conn = Connect();
                if (conn == null)
                    conn = Connect();
               
                MySqlCommand cmd = new MySqlCommand(szSQL, conn);
                if (tranc != null)
                    cmd.Transaction = tranc;

                try
                {
                    DBResultList ar = new DBResultList();

                    MySqlDataReader reader = cmd.ExecuteReader();

                    int rowCount = 0;

                    while (reader.Read())
                    {
                        ar.Column = reader.FieldCount;
                        if (reader.FieldCount == 0)
                        {

                        }
                        else
                        {
                            Object[] values = new Object[reader.FieldCount];

                            reader.GetValues(values);
                            ar.AddRange(values);
                        }
                        rowCount++;
                    }

                    reader.Close();

                    ar.Row = rowCount;
                    //conn.Close();
                    return ar;
                }
                catch (Exception e)
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(e, true);
                    System.Diagnostics.Trace.WriteLine("SQL : :" + szSQL);
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    System.Diagnostics.Trace.WriteLine(e.StackTrace);

                   

                    return null;
                }

                try
                {
                    conn.Close();

                }
                catch (Exception)
                {

                }
                conn = null;
            }
        }
    }
    public class DBResultList : ArrayList, IDisposable
    {
        public void Dispose()
        {
        }

        private int m_nRowCount = 0;
        public int Row
        {
            get { return m_nRowCount; }
            set { m_nRowCount = value; }
        }

        private int m_nColCount = 0;
        public int Column
        {
            get { return m_nColCount; }
            set { m_nColCount = value; }
        }

        private string m_szQuery = "";
        public string Query
        {
            get { return m_szQuery; }
            set { m_szQuery = value; }
        }
    }
}
