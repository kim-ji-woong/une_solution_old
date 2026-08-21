using DBUtility;
using System;
using System.Collections;
//using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Data.SQLite;

namespace DBUtility
{
    public class WebDBManager : IDisposable
    {
	    public enum DBType
	    {
		    sqlserver,
		    mysql,
            sqlite,
		    TypeCount
	    }

	    private CookieContainer cookieContainer = new CookieContainer();

	    private CookieContainer m_batchCookie = new CookieContainer();

	    private Utility m_ini = new Utility();

	    private string m_strWebServerURL = "";

	    private string m_szWebPageName = "aaa";

	    private string m_szProcPageName = "aaa";

	    private string m_szBatchPage = "aaa";

	    private string m_szDBHost;

	    private string m_szDBPort;

	    private string m_szDatabaseName = "sop";
        private string m_strDBPath = "";

	    private DBType m_dbType = DBType.sqlite;

	    private string m_lastResult = "";

	    private string m_szLastError = "";

	    private string m_szLastErrorMsg = "";

	    private Encoding m_PageEncoding;

	    //private SqlConnection conn;

        private SQLiteTransaction tranc;
	    //private SqlTransaction tranc;

	    private bool m_bBatch;

	    private string m_szBatchDB = "sop";

        private SQLiteConnection batchConn;
	    //private SqlConnection batchConn;

        private SQLiteTransaction batchTranc;
	    //private SqlTransaction batchTranc;

	    private object lockOjb = new object();

	    public string WebServerURL
	    {
		    get
		    {
			    return m_strWebServerURL;
		    }
		    set
		    {
			    m_strWebServerURL = value;
		    }
	    }

	    public string WebQueryPage
	    {
		    get
		    {
			    return m_szWebPageName;
		    }
		    set
		    {
			    m_szWebPageName = value;
		    }
	    }

	    public string WebProcPage
	    {
		    get
		    {
			    return m_szProcPageName;
		    }
		    set
		    {
			    m_szProcPageName = value;
		    }
	    }

	    public string WebBatchPage
	    {
		    get
		    {
			    return m_szBatchPage;
		    }
		    set
		    {
			    m_szBatchPage = value;
		    }
	    }

	    public string DatabaseHost
	    {
		    get
		    {
			    return m_szDBHost;
		    }
		    set
		    {
			    m_szDBHost = value;
		    }
	    }

	    public string DatabasePort
	    {
		    get
		    {
			    return m_szDBPort;
		    }
		    set
		    {
			    m_szDBPort = value;
		    }
	    }

	    public string DatabaseName
	    {
		    get
		    {
			    return m_szDatabaseName;
		    }
		    set
		    {
			    //m_szDatabaseName = value;
		    }
	    }

	    public DBType DatabaseType
	    {
		    get
		    {
			    return m_dbType;
		    }
		    set
		    {
			    //m_dbType = value;
		    }
	    }

	    public string DatabaseTypeName
	    {
		    get
		    {
			    return m_dbType.ToString();
		    }
		    set
		    {
			    /*if (string.Compare(value, "mysql", ignoreCase: true) == 0)
			    {
				    m_dbType = DBType.mysql;
			    }
			    else if (string.Compare(value, "sqlserver", ignoreCase: true) == 0)
			    {
				    m_dbType = DBType.sqlserver;
			    }*/
		    }
	    }

	    public string LastResult
        {
            get { return m_lastResult; }
        }

	    public string LastError
        {
            get { return m_szLastError; }
        }

	    public string LastErrorMessage
        {
            get { return m_szLastErrorMsg; }
        }

	    public WebDBManager(int nSiteID)
	    {
		    Loadini_ServerConnectionInfo(nSiteID);
		    //conn = Connect();
	    }

	    public WebDBManager(string strDatabaseName, int nSiteID)
	    {
		    Loadini_ServerConnectionInfo(nSiteID);
		    /*m_szDatabaseName = strDatabaseName;
		    conn = Connect();*/
	    }

	    public WebDBManager(string strDatabaseName, string strDBType, int nSiteID)
	    {
		    Loadini_ServerConnectionInfo(nSiteID);
		    /*m_szDatabaseName = strDatabaseName;
		    DatabaseTypeName = strDBType;
		    conn = Connect();*/
	    }

	    public static T GetField<T>(object dataSrc, T dataDefault)
	    {
		    if (dataSrc != DBNull.Value)
		    {
			    try
			    {
				    return (T)dataSrc;
			    }
			    catch (Exception)
			    {
				    return dataDefault;
			    }
		    }
		    return dataDefault;
	    }

	    public static float GetFloatField(string dataSrc, float fDefault)
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

	    public static string GetStringField(object dataSrc, string strDefault)
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

	    public static DateTime GetDateTimeField(object dataSrc, DateTime dtDefault)
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

	    public static int GetIntField(string dataSrc, int nDefault)
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

	    public static VariousData<int> GetIntField(string dataSrc)
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

	    public static VariousData<float> GetFloatField(string dataSrc)
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

	    public static VariousData<DateTime> GetDateTimeField(object dataSrc)
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

	    public static string GetStringField(object dataSrc)
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

	    private static char ConvertToHex(char cSource)
	    {
		    return "0123456789abcdef"[0xF & cSource];
	    }

	    public static string URLEncoding(byte[] bytes)
	    {
		    string text = "";
		    foreach (byte b in bytes)
		    {
			    if ((b >= 48 && b <= 57) || (b >= 97 && b <= 122) || (b >= 65 && b <= 90) || b == 33 || b == 42 || b == 40 || b == 41 || b == 95 || b == 45)
			    {
				    text += (char)b;
			    }
			    else
			    {
				    text += "%";
				    text += ConvertToHex((char)(b >> 4));
				    text += ConvertToHex((char)b);
			    }
		    }
		    return text;
	    }

	    public static string MakeDateTimeString(DateTime time)
	    {
		    return string.Format("{0} {1:00}:{2:00}:{3:00}", time.ToShortDateString(), time.Hour, time.Minute, time.Second);
	    }

	    public string Grave(object obj)
	    {
		    return "`" + obj.ToString() + "`";
	    }

	    public void Dispose()
	    {
		    GetReadDB("close", 0, m_szDatabaseName);
	    }

	    private string GetDBHostFromWebSererURL(string strServerURL)
	    {
		    int num = strServerURL.IndexOf("http://");
		    int num2 = strServerURL.LastIndexOf(':');
		    string result = strServerURL;
		    if (num >= 0 && num2 >= 0)
		    {
			    int num3 = num + "http://".Length;
			    result = strServerURL.Substring(num3, num2 - num3);
		    }
		    else if (num >= 0)
		    {
			    int startIndex = num + "http://".Length;
			    result = strServerURL.Substring(startIndex);
		    }
		    else if (num2 >= 0)
		    {
			    result = strServerURL.Substring(0, num2);
		    }
		    return result;
	    }

	    public void Loadini_ServerConnectionInfo(int nSiteID)
	    {
		    string section = "Server Connection Info";
            //m_szDatabaseName = m_ini.getinivalue(section, "dbname");

            string strLocalDB = Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\LocalDB\\" + m_szDatabaseName + ".db";

            if (System.IO.File.Exists(strLocalDB))
                m_strDBPath = strLocalDB;
            else
                m_strDBPath = "C:\\ProgramData\\SOP\\" + m_szDatabaseName + ".db";

		    /*m_strWebServerURL = RegUtil.ReadRegValue(section, "webserver_url", nSiteID);
		    if (m_strWebServerURL == null || m_strWebServerURL == "")
		    {
			    SetDefaultWebServerURL(nSiteID);
			    RegUtil.WriteRegValue(section, "webserver_url", m_strWebServerURL, nSiteID);
		    }
		    if (m_strWebServerURL != null && m_strWebServerURL.Length > 0)
		    {
			    DatabaseHost = GetDBHostFromWebSererURL(m_strWebServerURL);
		    }
		    m_szDatabaseName = RegUtil.ReadRegValue(section, "db_name", nSiteID);
		    if (m_szDatabaseName == null || m_szDatabaseName == "")
		    {
			    SetDefaultDBName(nSiteID);
			    RegUtil.WriteRegValue(section, "db_name", m_szDatabaseName, nSiteID);
		    }
		    bool flag = false;
		    string text = RegUtil.ReadRegValue(section, "db_type", nSiteID);
		    int result;
		    if (text != null && text != "" && int.TryParse(text, out result) && result >= 0 && result < 2)
		    {
			    DatabaseType = (DBType)result;
			    flag = true;
		    }
		    if (!flag)
		    {
			    RegUtil.WriteRegValue(section, "db_type", ((int)DatabaseType).ToString(), nSiteID);
		    }
		    string text2 = RegUtil.ReadRegValue(section, "page_encoding", nSiteID);
		    if (text2 == null || text2 == "")
		    {
			    text2 = SetDefaultEncoding(nSiteID);
			    RegUtil.WriteRegValue(section, "page_encoding", text2, nSiteID);
		    }
		    try
		    {
			    int result2 = -1;
			    if (int.TryParse(text2, out result2))
			    {
				    m_PageEncoding = Encoding.GetEncoding(result2);
			    }
			    else
			    {
				    m_PageEncoding = Encoding.UTF8;
			    }
		    }
		    catch (Exception)
		    {
			    m_PageEncoding = Encoding.UTF8;
		    }*/
	    }

	    private string SetDefaultEncoding(int nSiteID)
	    {
		    m_PageEncoding = Encoding.GetEncoding(51949);
		    string text = m_PageEncoding.CodePage.ToString();
		    switch (nSiteID)
		    {
		    case 1:
			    return Encoding.UTF8.CodePage.ToString();
		    case 2:
			    return Encoding.UTF8.CodePage.ToString();
		    default:
			    return "";
		    }
	    }

	    private void SetDefaultWebServerURL(int nSiteID)
	    {
		    switch (nSiteID)
		    {
		    case 1:
			    m_strWebServerURL = "http://172.18.101.50:8080/SOP";
			    break;
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
		    //m_szDatabaseName = "SOP_" + nSiteID.ToString();
	    }

	    public virtual string LoadIni(string strTargetName)
	    {
		    string section = "Server Connection Info";
		    return m_ini.getinivalue(section, strTargetName);
	    }

	    public virtual string LoadIni(string strTargetName, string strSectionName)
	    {
		    return m_ini.getinivalue(strSectionName, strTargetName);
	    }

	    public virtual string SaveIni(string strTargetName, string strValue, string strSectionName)
	    {
		    return m_ini.setinivalue(strSectionName, strTargetName, strValue);
	    }

	    private string GetReadDB(string strSQLQuery, int nTransaction, string szDBName)
	    {
		    string empty = string.Empty;
		    if (m_strWebServerURL == null || m_strWebServerURL.Equals(""))
		    {
			    m_strWebServerURL = "http://192.168.0.207:8088/SOP";
		    }
		    string requestUriString = m_strWebServerURL + "/" + m_szWebPageName;
		    UTF8Encoding uTF8Encoding = new UTF8Encoding();
		    byte[] bytes = uTF8Encoding.GetBytes(strSQLQuery);
		    string text = URLEncoding(bytes);
		    if (szDBName == null)
		    {
			    szDBName = m_szDatabaseName;
		    }
		    string text2 = "SQLQuery=" + text + "&Transaction=" + nTransaction;
		    string text3 = text2;
		    text2 = text3 + "&DatabaseName=" + szDBName + "&DatabaseType=" + DatabaseTypeName;
		    if (m_szDBHost != null)
		    {
			    text2 = text2 + "&Host=" + m_szDBHost;
		    }
		    if (m_szDBPort != null)
		    {
			    text2 = text2 + "&Port=" + m_szDBPort;
		    }
		    UTF8Encoding uTF8Encoding2 = new UTF8Encoding();
		    byte[] bytes2 = uTF8Encoding2.GetBytes(text2);
		    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
		    lock (this)
		    {
			    httpWebRequest.Method = "POST";
			    httpWebRequest.ContentType = "application/x-www-form-urlencoded";
			    httpWebRequest.ContentLength = bytes2.Length;
			    httpWebRequest.CookieContainer = cookieContainer;
			    try
			    {
				    using (Stream stream = httpWebRequest.GetRequestStream())
				    {
					    stream.Write(bytes2, 0, bytes2.Length);
				    }
				    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				    Stream responseStream = httpWebResponse.GetResponseStream();
				    StreamReader streamReader = new StreamReader(responseStream, m_PageEncoding);
				    empty = streamReader.ReadToEnd();
				    httpWebRequest.Abort();
				    streamReader.Close();
				    responseStream.Close();
				    return empty;
			    }
			    catch (WebException)
			    {
				    return "";
			    }
		    }
	    }

	    public virtual ArrayList GetResultData(string strSQLQuery, int nAutoCommit, int nLimit, string szDBName = null)
	    {
		    if (nLimit > 0)
		    {
			    if (m_dbType == DBType.mysql)
			    {
				    strSQLQuery = strSQLQuery + " LIMIT 0," + nLimit;
			    }
			    else if (m_dbType == DBType.sqlserver)
			    {
				    int num = strSQLQuery.ToLower().IndexOf("select");
				    if (num >= 0)
				    {
					    strSQLQuery = strSQLQuery.Insert(6, " TOP " + nLimit + " ");
				    }
			    }
                else if (m_dbType == DBType.sqlite)
                {
                    strSQLQuery = strSQLQuery + " LIMIT " + nLimit;
                }
		    }
		    return GetResultData(strSQLQuery, nAutoCommit, szDBName);
	    }

	    public virtual ArrayList GetResultData(string strSQLQuery, int nAutoCommit, string szDBName = null)
	    {
            SQLiteConnection conn = Connect(); 

            if (conn == null)
                return null;

		    strSQLQuery = strSQLQuery.Replace('\n', '\u0006');
		    strSQLQuery = strSQLQuery.Replace('\r', '\a');
		    if (DatabaseType == DBType.mysql)
		    {
			    strSQLQuery = strSQLQuery.Replace("\\", "\\\\");
		    }
		    if (szDBName == null)
		    {
			    szDBName = m_szDatabaseName;
		    }
		    
            ArrayList arrResult = GetResultLocalDB(conn, strSQLQuery, nAutoCommit);

            conn.Close();
            conn = null;
            return arrResult;
	    }

	    private void Commit()
	    {
		    if (tranc != null)
		    {
			    tranc.Commit();
		    }
	    }

	    private void Rollback()
	    {
		    if (tranc != null)
		    {
			    tranc.Rollback();
		    }
	    }

        private SQLiteConnection Connect()
        {
            string strConnection = "Data Source=" + m_strDBPath + ";Version=3;Password=alone9966;";
            //string strConnection = "Data Source=" + m_strDBPath + ";Version=3;Password=_&oiajf9472;";
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
	    /*private SqlConnection Connect()
	    {
		    string connectionString = "Data Source=(localdb)\\V11.0;Initial Catalog=SOP_1;Integrated Security=True;Pooling=False;AttachDbFileName=" + m_strDBPath;
		    SqlConnection sqlConnection = new SqlConnection(connectionString);
		    try
		    {
			    sqlConnection.Open();
			    return sqlConnection;
		    }
		    catch (Exception e)
		    {
			    new StackTrace(e, fNeedFileInfo: true);
			    return null;
		    }
	    }*/

        public virtual bool BeginBatch(int nBatchCode)
        {
            return BeginBatch();
        }

        public virtual void BatchCommit(int nBatchCode)
        {
            BatchCommit();
        }

        public virtual void BatchRollback(int nBatchCode)
        {
            BatchRollback();
        }

        public virtual ArrayList GetBatchData(int nBatchCode, string strSQL)
        {
            return GetBatchData(strSQL);
        }

	    public virtual bool BeginBatch(string szDBName = null)
	    {
		    m_bBatch = true;
		    m_szBatchDB = ((szDBName == null) ? m_szDatabaseName : szDBName);
		    batchConn = Connect();
		    if (batchConn != null)
		    {
			    batchTranc = batchConn.BeginTransaction();
		    }
		    return true;
	    }

	    public virtual void BatchCommit()
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
			    }
			    m_szBatchDB = "";
		    }
	    }

	    public virtual void BatchRollback()
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
			    }
			    m_bBatch = false;
			    m_szBatchDB = "";
		    }
	    }

	    public ArrayList GetBatchData(string strSQLQuery, int nLimit, string szDBName = null)
	    {
		    if (nLimit > 0)
		    {
			    if (m_dbType == DBType.mysql)
			    {
				    strSQLQuery = strSQLQuery + " LIMIT 0," + nLimit;
			    }
			    else if (m_dbType == DBType.sqlserver)
			    {
				    int num = strSQLQuery.ToLower().IndexOf("select");
				    if (num >= 0)
				    {
					    strSQLQuery = strSQLQuery.Insert(6, " TOP " + nLimit + " ");
				    }
			    }
                else if (m_dbType == DBType.sqlite)
                {
                    strSQLQuery = strSQLQuery + " LIMIT " + nLimit;
                }
		    }
		    return GetBatchData(strSQLQuery, szDBName);
	    }

	    public ArrayList GetBatchData(string strSQLQuery, string szDBName = null)
	    {
		    if (szDBName == null)
		    {
			    szDBName = m_szDatabaseName;
		    }
		    if (m_bBatch && !m_szBatchDB.Equals(szDBName))
		    {
			    return null;
		    }
		    if (!m_bBatch)
		    {
			    BeginBatch(szDBName);
		    }
		    strSQLQuery = strSQLQuery.Replace('\n', '\u0006');
		    strSQLQuery = strSQLQuery.Replace('\r', '\a');
		    return GetBatchResultLocalDB(batchConn, batchTranc, strSQLQuery);
	    }

	    private bool SplitStoreProcedureQuery(string strSQL, ref string strMethod, ref string strParam)
	    {
		    int length = strSQL.Length;
		    for (int i = 0; i < length; i++)
		    {
			    char c = strSQL[i];
			    if (c == ' ' || c == '\t')
			    {
				    strMethod = strSQL.Substring(0, i);
				    strParam = strSQL.Substring(i + 1).Trim();
				    return true;
			    }
		    }
		    return false;
	    }

	    private void ReshapeStoredProcedureQuery(ref string strSQL)
	    {
		    string text = strSQL.ToLower();
		    int num = text.IndexOf("call");
		    int num2 = text.IndexOf("exec");
		    if (DatabaseType == DBType.mysql)
		    {
			    if (num >= 0)
			    {
				    strSQL = strSQL.Trim();
			    }
			    else
			    {
				    if (num2 >= 0)
				    {
					    strSQL = strSQL.Substring(num2 + 4).Trim();
				    }
				    string strMethod = "";
				    string strParam = "";
				    if (SplitStoreProcedureQuery(strSQL, ref strMethod, ref strParam))
				    {
					    strSQL = "CALL " + strMethod + "(" + strParam + ")";
				    }
			    }
		    }
		    else if (DatabaseType == DBType.sqlserver)
		    {
			    if (num >= 0)
			    {
				    string text2 = strSQL.Substring(num + 4).Trim();
				    int num3 = text2.IndexOf('(');
				    int num4 = text2.LastIndexOf(')');
				    if (num3 >= 0 && num4 > num3)
				    {
					    string str = text2.Substring(0, num3).Trim();
					    string str2 = text2.Substring(num3 + 1, num4 - num3 - 1);
					    strSQL = str + " " + str2;
				    }
				    else
				    {
					    string strMethod2 = "";
					    string strParam2 = "";
					    if (SplitStoreProcedureQuery(text2, ref strMethod2, ref strParam2))
					    {
						    strSQL = strMethod2 + " " + strParam2;
					    }
				    }
			    }
			    else
			    {
				    strSQL = strSQL.Trim();
			    }
		    }
	    }

	    public ArrayList GetStoredProcedureData(string strSQLQuery, int nTransaction, string szDBName = null)
	    {
            SQLiteConnection conn = Connect();
            //SqlConnection conn = Connect();

            if (conn == null)
                return null;

		    ReshapeStoredProcedureQuery(ref strSQLQuery);
		    strSQLQuery = strSQLQuery.Replace('\n', '\u0006');
		    strSQLQuery = strSQLQuery.Replace('\r', '\a');
		    if (DatabaseType == DBType.mysql)
		    {
			    strSQLQuery = strSQLQuery.Replace("\\", "\\\\");
		    }
		    if (szDBName == null)
		    {
			    szDBName = m_szDatabaseName;
		    }
		    
            ArrayList arrResult = GetResultLocalDB(conn, strSQLQuery, nTransaction);

            conn.Close();
            conn = null;
            return arrResult;
	    }

	    public void RunStoredProcedure(string strProcName, ArrayList arrFields, ArrayList arrValues, int transaction, out ArrayList arrResult, string szDBName = null)
	    {
            arrResult = null;
            SQLiteConnection conn = Connect();
            //SqlConnection conn = Connect();

            if (conn == null)
                return;

		    int count = arrFields.Count;
		    int count2 = arrValues.Count;
		    if (count == count2)
		    {
			    string text = strProcName;
			    for (int i = 0; i < count2; i++)
			    {
				    text = ((i != 0) ? (text + "," + (string)arrValues[i]) : (text + " " + (string)arrValues[i]));
			    }
			    arrResult = GetResultLocalDB(conn, text, transaction);
		    }

            conn.Close();
	    }

        public DBResultList GetBatchResultLocalDB(SQLiteConnection connection, SQLiteTransaction tranction, string szSQL)
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
	    /*public DBResultList GetBatchResultLocalDB(SqlConnection connection, SqlTransaction tranction, string szSQL)
	    {
		    if (connection != null)
		    {
			    SqlCommand sqlCommand = new SqlCommand(szSQL, connection);
			    if (tranction != null)
			    {
				    sqlCommand.Transaction = tranction;
			    }
			    try
			    {
				    DBResultList dBResultList = new DBResultList();
				    if (szSQL.ToLower().StartsWith("select"))
				    {
					    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
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
	    }*/

        private DBResultList ToStringArray(DBResultList resultList)
        {
            for (int i=0;i<resultList.Count;i++)
            {
                object obj = resultList[i];

                if (obj is string)
                    continue;
                else
                    resultList[i] = obj.ToString();
            }

            return resultList;
        }

        public DBResultList GetResultLocalDB(SQLiteConnection conn, string szSQL, int nTranc)
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
	    /*public DBResultList GetResultLocalDB(SqlConnection conn, string szSQL, int nTranc)
	    {
		    lock (lockOjb)
		    {
			    if (conn != null)
			    {
				    SqlCommand sqlCommand = new SqlCommand(szSQL, conn);
				    if (tranc != null)
				    {
					    sqlCommand.Transaction = tranc;
				    }
				    try
				    {
					    DBResultList dBResultList = new DBResultList();
					    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
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
	    }*/
    }
}
