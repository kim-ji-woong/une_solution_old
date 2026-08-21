using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;

namespace dnsDBUtil
{
    public class WebDBManager
    {
        public enum DBType { sqlserver = 0, mysql, TypeCount };

        private const string NOT_CONNECTED_EXCEPTION = "WebDB 접속이 끊어졌습니다.\r\n서버 관리자에게 문의하세요.";

        private int m_nSiteID = 0;
        private string m_strWebServerURL = "";
        private string m_strDatabaseName = "";
        private DBType m_dbType = DBType.sqlserver;

        private string m_strLastErrorMsg = "";

        private bool m_isBeginBatch = false;
        private string m_strBatchDB = "";
        private long m_nBatchCode = 0;

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
                {
                    m_dbType = DBType.mysql;
                }
                else if (string.Compare(value, "sqlserver", true) == 0)
                {
                    m_dbType = DBType.sqlserver;
                }
            }
        }

        public string DatabaseName
        {
            get { return m_strDatabaseName; }
            set { m_strDatabaseName = value; }
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

        protected bool IsBeginBatch
        {
            get { return m_isBeginBatch; }
        }

        //private Utility m_ini = new Utility();

        public WebDBManager()
        {
        }

        public WebDBManager(int nSiteID)
        {
            m_nSiteID = nSiteID;

            LoadConnectionInfo(nSiteID);
        }

        public WebDBManager(string strDatabaseName, int nSiteID)
        {
            m_nSiteID = nSiteID;
            m_strDatabaseName = strDatabaseName;

            LoadConnectionInfo(nSiteID);
        }

        public WebDBManager(string strDatabaseName, string strDBType, int nSiteID)
        {
            m_nSiteID = nSiteID;
            m_strDatabaseName = strDatabaseName;
            this.DatabaseTypeName = strDBType;

            LoadConnectionInfo(nSiteID);
        }

        public WebDBManager(string strDatabaseName, int nDBType, int nSiteID)
        {
            m_nSiteID = nSiteID;
            m_strDatabaseName = strDatabaseName;
            this.DatabaseType = (DBType)nDBType;

            LoadConnectionInfo(nSiteID);
        }

        public WebDBManager(string strDatabaseName, string strDBType, int nSiteID, string strWebServerURL)
        {
            m_nSiteID = nSiteID;
            m_strDatabaseName = strDatabaseName;
            m_strWebServerURL = strWebServerURL;
            this.DatabaseTypeName = strDBType;

            LoadConnectionInfo(nSiteID);
        }

        public WebDBManager(string strDatabaseName, int nDBType, int nSiteID, string strWebServerURL)
        {
            m_nSiteID = nSiteID;
            m_strDatabaseName = strDatabaseName;
            m_strWebServerURL = strWebServerURL;
            this.DatabaseType = (DBType)nDBType;

            LoadConnectionInfo(nSiteID);
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
            //string strSection = "Server Connection Info";

            //m_strWebServerURL = RegUtil.ReadRegValue(strSection, "webserver_url2", nSiteID);
            if (m_strWebServerURL == null || m_strWebServerURL == "")
            {
                SetDefaultWebServerURL(nSiteID);
                //RegUtil.WriteRegValue(strSection, "webserver_url2", m_strWebServerURL, nSiteID);
            }

            //m_strDatabaseName = RegUtil.ReadRegValue(strSection, "db_name", nSiteID);

            if (m_strDatabaseName == null || m_strDatabaseName == "")
            {
                SetDefaultDBName(nSiteID);
                //RegUtil.WriteRegValue(strSection, "db_name", m_strDatabaseName, nSiteID);
            }

            bool findDBType = false;
            //string strDBType = RegUtil.ReadRegValue(strSection, "db_type", nSiteID);

            string strDBType = this.DatabaseTypeName;

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
            }
            
            int dbType = (int)this.DatabaseType;

            if (dbType >= 0 && dbType < (int)DBType.TypeCount)
            {
                findDBType = true;
                this.DatabaseTypeName = ((DBType)dbType).ToString();
            }

            //if (!findDBType)
            //    RegUtil.WriteRegValue(strSection, "db_type", ((int)this.DatabaseType).ToString(), nSiteID);
        }

        private void SetDefaultWebServerURL(int nSiteID)
        {
            switch (nSiteID)
            {
                // 삼천포
                case 1:
                    m_strWebServerURL = "http://172.18.101.50:8080";
                    break;
                // 영흥
                case 2:
                    m_strWebServerURL = "http://172.20.127.150:8080";
                    break;
                // 광교
                case 3:
                    m_strWebServerURL = "http://192.168.0.195:8080";
                    break;
                // 서울대학교
                case 100:
                    m_strWebServerURL = "http://192.168.250.41:8080";
                    break;
                // 부산대학교
                case 101:
                    m_strWebServerURL = "http://192.168.120.250:8080";
                    break;
                case 999:
                    m_strWebServerURL = "https://localhost:5001";
                    break;
                default:
                    m_strWebServerURL = "";
                    break;
            }
        }

        private void SetDefaultDBName(int nSiteID)
        {
            switch (nSiteID)
            {
                // 삼천포
                case 1:
                    m_strDatabaseName = "SOP_" + nSiteID.ToString();
                    break;
                // 영흥
                case 2:
                    m_strDatabaseName = "SOP_" + nSiteID.ToString();
                    break;
                // 광교
                case 3:
                    m_strDatabaseName = "SOP_" + nSiteID.ToString();
                    break;
                // 서울대학교
                case 100:
                    m_strDatabaseName = "EDU_" + nSiteID.ToString();
                    break;
                // 부산대학교
                case 101:
                    m_strDatabaseName = "EDU_" + nSiteID.ToString();
                    break;
                case 999:
                    m_strDatabaseName = "BLD_202001";
                    break;
                default:
                    m_strDatabaseName = "SOP_" + nSiteID.ToString();
                    break;
            }
        }

        private void CheckLimit(ref string strSQL, int nLimit)
        {
            if (nLimit > 0)
            {
                if (m_dbType == DBType.mysql)
                {
                    strSQL += " LIMIT 0," + nLimit;
                }
                else if (m_dbType == DBType.sqlserver)
                {
                    int nIdx = strSQL.ToLower().IndexOf("select");
                    if (nIdx >= 0)
                    {
                        strSQL = strSQL.Insert(6, " TOP " + nLimit + " ");
                    }
                }
            }
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
            CheckLimit(ref strSQL, nLimit);
            return GetResultData(strSQL, strDBName);
        }

        public virtual ArrayList GetResultData(string strSQL, string strDBName = null)
        {
            if (this.DatabaseType == DBType.mysql)
            {
                ChangeQuery(ref strSQL, "isnull", "ifnull");
                //strSQL = strSQL.Replace("ISNULL", "IFNULL");
                //strSQL = strSQL.Replace("isnull", "ifnull");
                strSQL = strSQL.Replace("\\", "\\\\");
            }
            else if (this.DatabaseType == DBType.sqlserver)
            {
                ChangeQuery(ref strSQL, "ifnull", "isnull");
                //strSQL = strSQL.Replace("IFNULL", "ISNULL");
            }

            if (strDBName == null)
                strDBName = m_strDatabaseName;

            return GetReadDB(strSQL, strDBName);
        }

        public virtual ArrayList GetStoredProcedureResult(string strProcedureName, List<string> fieldNames, List<string> fieldValues, string strDBName = null)
        {
            if (strDBName == null)
                strDBName = m_strDatabaseName;

            return GetReadProcedure(strProcedureName, fieldNames, fieldValues, strDBName);
        }

        private void ChangeQuery(ref string strSQL, string strSrc, string strTrg)
        {
            int nIndex = strSQL.ToLower().IndexOf(strSrc);

            if (nIndex >= 0)
            {
                string str = strSQL.Substring(nIndex, strSrc.Length);
                strSQL = strSQL.Replace(str, strTrg);
            }
        }

        private ArrayList GetReadDB(string strSQL, string strDBName)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/WebDB/RunQuery", m_strWebServerURL));
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "dbName", strDBName },
                    { "dbType", DatabaseTypeName },
                    { "query", strSQL }
                };


                string json = JsonManager.Serialize(values);

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                string[] results = JsonManager.Deserialize<string[]>(resTemp);

                if (results == null)
                {
                    m_strLastErrorMsg = "WebDB 접속에 실패하였습니다.\r\n네트웍 상황을 확인하세요.";
                    return null;
                }
                else if (results[0] != "1")
                {
                    m_strLastErrorMsg = results[1];
                    return null;
                }

                int nDataCount;

                if (int.TryParse(results[1], out nDataCount) == false)
                {
                    m_strLastErrorMsg = "알수없는 오류입니다.";
                    return null;
                }

                ArrayList arrResults = new ArrayList();

                for (int i = 0; i < nDataCount; i++)
                {
                    arrResults.Add(results[i + 2]);
                }

                return arrResults;
            }
            catch (Exception e)
            {
                m_isBeginBatch = false;
                m_strLastErrorMsg = e.Message;
            }

            return null;
        }

        public static void AddStoredProcedureValue(List<string> values, object value)
        {
            if (value == null)
                values.Add(null);
            else if (value is int ||
                value is long)
                values.Add("i" + value.ToString());
            else if (value is bool)
            {
                if ((bool)value)
                    values.Add("i1");
                else
                    values.Add("i0");
            }
            else if (value is float ||
                value is double)
                values.Add("f" + value.ToString());
            else if (value is string)
                values.Add("s" + value.ToString());
        }

        private ArrayList GetReadProcedure(string strProcedureName, List<string> fieldNames, List<string> fieldValues, string strDBName)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/WebDB/RunStoredProcedure", m_strWebServerURL));
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                Dictionary<string, object> values = new Dictionary<string, object>
                {
                    { "dbName", strDBName },
                    { "dbType", DatabaseTypeName },
                    { "procedureName", strProcedureName },
                    { "fieldNames", fieldNames },
                    { "fieldValues", fieldValues }
                };

                string json = JsonManager.Serialize(values);

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                string[] results = JsonManager.Deserialize<string[]>(resTemp);

                if (results == null)
                {
                    m_strLastErrorMsg = "WebDB 접속에 실패하였습니다.\r\n네트웍 상황을 확인하세요.";
                    return null;
                }
                else if (results[0] != "1")
                {
                    m_strLastErrorMsg = results[1];
                    return null;
                }

                int nDataCount;

                if (int.TryParse(results[1], out nDataCount) == false)
                {
                    m_strLastErrorMsg = "알수없는 오류입니다.";
                    return null;
                }

                ArrayList arrResults = new ArrayList();

                for (int i = 0; i < nDataCount; i++)
                {
                    arrResults.Add(results[i + 2]);
                }

                return arrResults;
            }
            catch (Exception e)
            {
                m_isBeginBatch = false;
                m_strLastErrorMsg = e.Message;
            }

            return null;
        }

        public virtual bool BeginBatch(string strDBName = null)
        {
            if (m_isBeginBatch == true)
            {
                throw new WebDBTransactionStateException("이전 트랜잭션이 종료되지 않았습니다.\nRollback이나 Commit이후에 호출 가능합니다.");
            }

            m_isBeginBatch = true;
            m_strBatchDB = (strDBName == null ? m_strDatabaseName : strDBName);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/WebDB/BeginBatch", m_strWebServerURL));
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "dbName", m_strBatchDB },
                    { "dbType", DatabaseTypeName }
                };

                string json = JsonManager.Serialize(values);

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                string[] results = JsonManager.Deserialize<string[]>(resTemp);
                long beginBatchResult = Convert.ToInt64(results[0]);
                string errorMessage = results[1];

                if (beginBatchResult == 0)
                {
                    if (errorMessage == null)
                    {
                        m_strLastErrorMsg = "알수없는 오류입니다.";
                        return false;
                    }
                    else
                    {
                        m_strLastErrorMsg = errorMessage;
                        return false;
                    }
                }
                else
                {
                    m_nBatchCode = beginBatchResult;
                }
            }
            catch (Exception e)
            {
                m_isBeginBatch = false;
                m_strLastErrorMsg = e.Message;
                return false;
            }

            return true;
        }

        public virtual bool BatchCommit()
        {
            if (m_isBeginBatch == false)
            {
                throw new WebDBTransactionStateException("트랜잭션이 시작되지 않았습니다.\nCommit은 BeginBatch이후에 호출 가능합니다.");
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/WebDB/BatchCommit", m_strWebServerURL));
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "transactionKey", m_nBatchCode.ToString() }
                };

                string json = JsonManager.Serialize(values);

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                string result = resTemp;

                m_isBeginBatch = false;

                if (result == null)
                {
                    m_strLastErrorMsg = "알수없는 오류입니다.";
                    return false;
                }
                else if (result.Length > 0)
                {
                    m_strLastErrorMsg = result;
                    return false;
                }
            }
            catch (Exception e)
            {
                m_isBeginBatch = false;
                m_strLastErrorMsg = e.Message;
                return false;
            }

            return true;
        }

        public virtual bool BatchRollback()
        {
            if (m_isBeginBatch == false)
            {
                throw new WebDBTransactionStateException("트랜잭션이 시작되지 않았습니다.\nRollback은 BeginBatch이후에 호출 가능합니다.");
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/WebDB/BatchRollback", m_strWebServerURL));
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "transactionKey", m_nBatchCode.ToString() }
                };

                string json = JsonManager.Serialize(values);

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                string result = resTemp;

                m_isBeginBatch = false;

                if (result == null)
                {
                    m_strLastErrorMsg = "알수없는 오류입니다.";
                    return false;
                }
                else if (result.Length > 0)
                {
                    m_strLastErrorMsg = result;
                    return false;
                }
            }
            catch (Exception e)
            {
                m_isBeginBatch = false;
                m_strLastErrorMsg = e.Message;
            }

            return true;
        }

        public virtual ArrayList GetBatchStoredProcedureResult(string strProcedureName, List<string> fieldNames, List<string> fieldValues)
        {
            if (m_isBeginBatch == false)
            {
                throw new WebDBTransactionStateException("트랜잭션이 시작되지 않았습니다.");
            }

            return GetBatchProcedure(strProcedureName, fieldNames, fieldValues);
        }

        private ArrayList GetBatchProcedure(string strProcedureName, List<string> fieldNames, List<string> fieldValues)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/WebDB/BatchStoredProcedure", m_strWebServerURL));
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                Dictionary<string, object> values = new Dictionary<string, object>
                {
                    { "procedureName", strProcedureName },
                    { "fieldNames", fieldNames },
                    { "fieldValues", fieldValues },
                    { "transactionKey", m_nBatchCode.ToString() }
                };

                string json = JsonManager.Serialize(values);

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                string[] results = JsonManager.Deserialize<string[]>(resTemp);

                if (results == null)
                {
                    m_strLastErrorMsg = "WebDB 접속에 실패하였습니다.\r\n네트웍 상황을 확인하세요.";
                    return null;
                }
                else if (results[0] != "1")
                {
                    m_strLastErrorMsg = results[1];
                    return null;
                }

                int nDataCount;

                if (int.TryParse(results[1], out nDataCount) == false)
                {
                    m_strLastErrorMsg = "알수없는 오류입니다.";
                    return null;
                }

                ArrayList arrResults = new ArrayList();

                for (int i = 0; i < nDataCount; i++)
                {
                    arrResults.Add(results[i + 2]);
                }

                return arrResults;
            }
            catch (Exception e)
            {
                m_isBeginBatch = false;
                m_strLastErrorMsg = e.Message;
            }

            return null;
        }

        public virtual ArrayList GetBatchData(string strSQL)
        {
            if (m_isBeginBatch == false)
            {
                throw new WebDBTransactionStateException("트랜잭션이 시작되지 않았습니다.");
            }

            if (this.DatabaseType == DBType.mysql)
            {
                ChangeQuery(ref strSQL, "isnull", "ifnull");
                //strSQL = strSQL.Replace("ISNULL", "IFNULL");
                //strSQL = strSQL.Replace("isnull", "ifnull");
                strSQL = strSQL.Replace("\\", "\\\\");
            }
            else if (this.DatabaseType == DBType.sqlserver)
            {
                ChangeQuery(ref strSQL, "ifnull", "isnull");
                //strSQL = strSQL.Replace("IFNULL", "ISNULL");
            }

            return GetBatchDB(strSQL);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSQL">실행할 쿼리</param>
        /// <param name="nLimit">최대 행 개수</param>
        /// <returns></returns>
        public virtual ArrayList GetBatchData(string strSQL, int nLimit)
        {
            CheckLimit(ref strSQL, nLimit);
            return GetBatchData(strSQL);
        }

        private ArrayList GetBatchDB(string strSQL)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/WebDB/BatchQuery", m_strWebServerURL));
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "query", strSQL },
                    { "transactionKey", m_nBatchCode.ToString() }
                };

                string json = JsonManager.Serialize(values);

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                string[] results = JsonManager.Deserialize<string[]>(resTemp);

                if (results == null)
                {
                    m_strLastErrorMsg = "WebDB 접속에 실패하였습니다.\r\n네트웍 상황을 확인하세요.";
                    return null;
                }
                else if (results[0] != "1")
                {
                    m_strLastErrorMsg = results[1];
                    return null;
                }

                int nDataCount;

                if (int.TryParse(results[1], out nDataCount) == false)
                {
                    m_strLastErrorMsg = "알수없는 오류입니다.";
                    return null;
                }

                ArrayList arrResults = new ArrayList();

                for (int i = 0; i < nDataCount; i++)
                {
                    arrResults.Add(results[i + 2]);
                }

                return arrResults;
            }
            catch (Exception e)
            {
                m_isBeginBatch = false;
                m_strLastErrorMsg = e.Message;
            }

            return null;
        }

        static public byte GetByteField(string dataSrc, byte bDefault)
        {
            byte result = bDefault;

            if (dataSrc == null || dataSrc.StartsWith("!") == false)
            {
                return result;
            }

            byte num;

            if (byte.TryParse(dataSrc.Substring(1), out num))
                return num;

            return result;
        }

        static public VariousData<byte> GetByteField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.StartsWith("!") == false)
                return null;

            byte num;

            if (byte.TryParse(dataSrc.Substring(1), out num))
                return new VariousData<byte>(num);

            return null;
        }

        static public int GetIntField(string dataSrc, int nDefault)
        {
            int result = nDefault;

            if (dataSrc == null || dataSrc.StartsWith("!") == false)
            {
                return result;
            }

            string strValue = dataSrc.Substring(1);

            if (string.Compare(strValue, "true", true) == 0)
                return 1;
            else if (string.Compare(strValue, "false", true) == 0)
                return 0;

            int num;

            if (int.TryParse(strValue, out num))
                return num;

            return nDefault;
        }

        static public VariousData<int> GetIntField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.StartsWith("!") == false)
                return null;

            string strValue = dataSrc.Substring(1);

            if (string.Compare(strValue, "true", true) == 0)
                return new VariousData<int>(1);
            else if (string.Compare(strValue, "false", true) == 0)
                return new VariousData<int>(0);

            int num;

            if (int.TryParse(strValue, out num))
                return new VariousData<int>(num);

            return null;
        }

        static public float GetFloatField(string dataSrc, float fDefault)
        {
            float result = fDefault;

            if (dataSrc == null || dataSrc.StartsWith("!") == false)
                return result;

            float num;

            if (float.TryParse(dataSrc.Substring(1), out num))
                return num;

            return result;
        }

        static public VariousData<float> GetFloatField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.StartsWith("!") == false)
                return null;

            float num;

            if (float.TryParse(dataSrc.Substring(1), out num))
                return new VariousData<float>(num);

            return null;
        }

        static public double GetDoubleField(string dataSrc, float fDefault)
        {
            double result = fDefault;

            if (dataSrc == null || dataSrc.StartsWith("!") == false)
                return result;

            double num;

            if (double.TryParse(dataSrc.Substring(1), out num))
                return num;

            return result;
        }

        static public VariousData<double> GetDoubleField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.StartsWith("!") == false)
                return null;

            double num;

            if (double.TryParse(dataSrc.Substring(1), out num))
                return new VariousData<double>(num);

            return null;
        }

        static public DateTime GetDateTimeField(object dataSrc, DateTime dtDefault)
        {
            DateTime result = dtDefault;

            if (dataSrc == null)
                return result;

            string strValue = dataSrc.ToString();

            if (strValue.StartsWith("!") == false)
                return result;

            strValue = strValue.Substring(1);

            try
            {
                DateTime time = Convert.ToDateTime(strValue);
                return time;
            }
            catch (Exception)
            {
            }

            return result;
        }

        static public VariousData<DateTime> GetDateTimeField(object dataSrc)
        {
            if (dataSrc == null)
                return null;

            string strValue = dataSrc.ToString();

            if (strValue.StartsWith("!") == false)
                return null;

            strValue = strValue.Substring(1);

            try
            {
                DateTime time = Convert.ToDateTime(strValue);
                return new VariousData<DateTime>(time);
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
                return strDefault;

            string strValue = dataSrc.ToString();

            if (strValue.StartsWith("!") == false)
                return strDefault;

            strValue = strValue.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            strValue = strValue.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

            // (char)5, 6, 7, 8은 DB 입력시 '\t', '\n', '\r', '\''이 임시로 바뀌어 들어간 값이므로, 다시 '\n'으로 되돌려 준다.

            strValue = strValue.Replace((char)6, '\n');
            strValue = strValue.Replace((char)7, '\r');
            strValue = strValue.Replace((char)8, '\'');

            strValue = strValue.Substring(1).Trim();
            return strValue;
        }

        // 문자열 앞뒤의 빈문자들을 제거한다.
        static public string GetStringField(object dataSrc)
        {
            if (dataSrc == null)
                return null;

            string strValue = dataSrc.ToString();

            if (strValue.StartsWith("!") == false)
                return null;

            strValue = (string)dataSrc;
            strValue = strValue.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            strValue = strValue.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

            // (char)5, 6, 7, 8은 DB 입력시 '\t', '\n', '\r', '\''이 임시로 바뀌어 들어간 값이므로, 다시 '\n'으로 되돌려 준다.

            strValue = strValue.Replace((char)6, '\n');
            strValue = strValue.Replace((char)7, '\r');
            strValue = strValue.Replace((char)8, '\'');

            strValue = strValue.Substring(1).Trim();
            return strValue;
        }

        public static string MakeDateTimeString(DateTime time)
        {
            return string.Format("{0} {1:00}:{2:00}:{3:00}", time.ToShortDateString(), time.Hour, time.Minute, time.Second);
        }

        /// <summary>
        /// Model Class의 멤버에 value를 순서대로 넣어준다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"> Model Class </param>
        /// <param name="info"> Model Class의 Field Info </param>
        /// <param name="value"> 세팅할 값들 </param>
        /// <returns> Model Class Type의 객체 </returns>
        public T GetObjectWithParams<T>(T model, PropertyInfo[] info, string[] columnInfo, params object[] value)
        {
            if (columnInfo.Length == value.Length)
            {
                for (int i = 0; i < info.Length; i++)
                {
                    if (columnInfo.Contains(info[i].Name, StringComparer.OrdinalIgnoreCase))
                    {
                        int idx = columnInfo.ToList().FindIndex(x => x.Equals(info[i].Name, StringComparison.OrdinalIgnoreCase));                        

                        if (info[i].CanWrite)
                        {
                            info[i].SetValue(model, value[idx]);
                        }
                        else
                        {
                            // Property 중 Setter가 없는 경우
                            continue;
                        }
                    }
                    else
                    {
                        var modelType = model.GetType();
                        var modelValue = modelType.GetProperty(info[i].Name).GetValue(model);

                        if (info[i].CanWrite)
                        {
                            info[i].SetValue(model, modelValue);
                        }
                        else
                        {
                            // Property 중 Setter가 없는 경우
                            continue;
                        }
                    }
                }
            }

            return (T)Convert.ChangeType(model, typeof(T));
        }

        /// <summary>
        /// Update 시 필요한 Value에 대한 Query String을 받아온다.
        /// </summary>
        /// <param name="info"> 해당 테이블의 칼럼 목록과 자료형으로 구성된 Dictionary, GetColumnInfoDictionary 참조 </param>
        /// <param name="param"> Update할 값 (Model Class, 변수이름 - 값으로 구성된 Dictionary 지원) </param>
        /// <param name="properties"> Model Class 사용 시 필요한 Class Info </param>
        /// <returns> Value에 대한 Query String </returns>
        public string ConvertUpdateParamsToString(Dictionary<string, string> info, object param, PropertyInfo[] properties = null)
        {
            string updateString = null;

            var paramType = param.GetType();

            if (paramType.Name.Contains("Dictionary"))
            {
                Dictionary<string, object> temp = param as Dictionary<string, object>;

                for (int i = 0; i < temp.Count; i++)
                {
                    string columnName = "", columnType = "";
                    KeyValuePair<string, string> existColumn = new KeyValuePair<string, string>();
                    bool isExist = false;

                    existColumn = info.FirstOrDefault(x => string.Equals(x.Key, temp.ElementAt(i).Key, StringComparison.OrdinalIgnoreCase));

                    if (existColumn.Key != null && existColumn.Value != null)
                    {
                        columnName = existColumn.Key;
                        columnType = existColumn.Value;
                        isExist = true;
                    }

                    if (isExist)
                    {
                        var value = temp.ElementAt(i).Value;

                        if (value == null)
                        {
                            Convert.ChangeType(value, typeof(string));
                            value = "NULL";
                        }
                        else
                        {
                            if (!properties[i].PropertyType.FullName.Contains("List"))
                            {
                                if (properties[i].PropertyType.FullName.Contains("string") || properties[i].PropertyType.FullName.Contains("String") ||
                                properties[i].PropertyType.FullName.Contains("bool") || properties[i].PropertyType.FullName.Contains("Boolean"))
                                {
                                    Convert.ChangeType(value, typeof(string));
                                    value = value.ToString().Trim();
                                    value = string.Format("'{0}'", value);
                                }
                                else if (properties[i].PropertyType.FullName.Contains("datetime") || properties[i].PropertyType.FullName.Contains("DateTime"))
                                {
                                    DateTime timeTemp = (DateTime)value;
                                    string format = "yyyy-MM-dd HH:mm:ss";
                                    string timeStr = timeTemp.ToString(format);
                                    Convert.ChangeType(value, typeof(string));
                                    value = string.Format("'{0}'", timeStr);
                                }
                            }
                        }

                        updateString += string.Format("{0} = {1}, ", temp.ElementAt(i).Key, value);
                    }
                }
            }
            else
            {
                if (properties != null)
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        string columnName = "", columnType = "";
                        KeyValuePair<string, string> existColumn = new KeyValuePair<string, string>();
                        bool isExist = false;

                        existColumn = info.FirstOrDefault(x => string.Equals(x.Key, properties[i].Name, StringComparison.OrdinalIgnoreCase));

                        if (existColumn.Key != null && existColumn.Value != null)
                        {
                            columnName = existColumn.Key;
                            columnType = existColumn.Value;
                            isExist = true;
                        }

                        if (isExist)
                        {
                            if (param.GetType().GetProperty(properties[i].Name) == null)
                            {
                                continue;
                            }

                            var value = param.GetType().GetProperty(properties[i].Name).GetValue(param);

                            if (value == null)
                            {
                                Convert.ChangeType(value, typeof(string));
                                value = "NULL";
                            }
                            else
                            {
                                if (!properties[i].PropertyType.FullName.Contains("List"))
                                {
                                    if (properties[i].PropertyType.FullName.Contains("string") || properties[i].PropertyType.FullName.Contains("String") ||
                                    properties[i].PropertyType.FullName.Contains("bool") || properties[i].PropertyType.FullName.Contains("Boolean"))
                                    {
                                        Convert.ChangeType(value, typeof(string));
                                        value = value.ToString().Trim();
                                        value = string.Format("'{0}'", value);
                                    }
                                    else if (properties[i].PropertyType.FullName.Contains("datetime") || properties[i].PropertyType.FullName.Contains("DateTime"))
                                    {
                                        DateTime timeTemp = (DateTime)value;
                                        string format = "yyyy-MM-dd HH:mm:ss";
                                        string timeStr = timeTemp.ToString(format);
                                        Convert.ChangeType(value, typeof(string));
                                        value = string.Format("'{0}'", timeStr);
                                    }
                                }
                            }

                            updateString += string.Format("{0} = {1}, ", properties[i].Name, value);
                        }
                    }
                }
            }

            if (updateString != null && updateString.EndsWith(", "))
            {
                updateString = updateString.Substring(0, updateString.Length - 2);
            }

            return updateString;
        }

        private int GetPropertyIndex(string strColumnName, PropertyInfo[] properties)
        {
            int index = 0;

            foreach (PropertyInfo prop in properties)
            {
                if (string.Compare(prop.Name, strColumnName, true) == 0)
                    return index;

                index++;
            }

            return -1;
        }

        /// <summary>
        /// Select 조건에 맞는 데이터를 Model Class의 Data에 맞게 세팅해주는 메소드
        /// </summary>
        /// <param name="info"> 해당 테이블의 칼럼 목록과 자료형으로 구성된 Dictionary, GetColumnInfoDictionary 참조 </param>
        /// <param name="model"> 참조할 Model Class </param>
        /// <param name="properties"> Model Class에 대한 멤버 정보 </param>
        /// <param name="data"> Select 결과 Data </param>
        /// <param name="notExistMember"> 해당 테이블에 존재하지 않는 칼럼 </param>        
        /// <returns> Model Class 형태의 List </returns>
        public List<object> SetParamsWithColumnInfo(Dictionary<string, string> info, object model, PropertyInfo[] properties, ArrayList data, out string[] notExistMember)
        {
            int loopCnt = 0;
            List<string> notExistTemp = new List<string>();

            if (data.Count % info.Count == 0)
            {
                loopCnt = data.Count / info.Count;
            }

            List<object> ret = new List<object>();
            object[] items = new object[loopCnt];
            Type modelType = model.GetType();

            for (int i = 0; i < loopCnt; i++)
            {
                items[i] = Activator.CreateInstance(modelType);
            }

            object _lock = new object(); // Lock Object

            Parallel.For(0, loopCnt, i =>
            {
                lock (_lock)
                {
                    for (int j = properties.Length * i; j < properties.Length * (i + 1); j++)
                    {

                        string columnName = "", columnType = "";
                        KeyValuePair<string, string> existColumn = new KeyValuePair<string, string>();                        
                        bool isExist = false;                        

                        existColumn = info.FirstOrDefault(x => string.Equals(x.Key, properties[j % properties.Length].Name, StringComparison.OrdinalIgnoreCase));                        

                        if (existColumn.Key != null && existColumn.Value != null)
                        {
                            columnName = existColumn.Key;
                            columnType = existColumn.Value;
                            isExist = true;
                        }

                        if (isExist) // Class Member가 조회한 Column 이름에 있을 경우
                        {
                            int idx = GetPropertyIndex(columnName, properties) + (i * info.Count);
                            //int idx = info.Keys.ToList().IndexOf(columnName) + (i * info.Count);

                            if (data[idx].ToString().StartsWith("!"))
                            {
                                data[idx] = data[idx].ToString().Substring(1);
                            }
                            else
                            {
                                data[idx] = null;
                            }

                            if (data[idx] != null) // 얻어온 Column 데이터가 Null이 아닐 경우
                            {

                                if (this.DatabaseType == DBType.mysql)
                                {
                                    // geometry, geometrycollection, linestring, multilinestring, multipoint, multipolygon, point, polygon
                                    // timestamp, enum, set 은 미포함
                                    // enum, set의 경우 해당 칼럼의 이름 + 0 (ex) col1 + 0으로 select 쿼리)으로 내부 값을 따로 조회하여 처리 해야 함
                                    switch (columnType)
                                    {
                                        case "tinyint":
                                            if (data[idx].ToString().Length == 1) // Boolean
                                            {
                                                var boolTemp = Convert.ToBoolean(Int32.Parse(data[idx].ToString()));
                                                data[idx] = boolTemp;
                                            }
                                            else
                                            {
                                                data[idx] = Convert.ChangeType(data[idx], typeof(sbyte));
                                            }
                                            break;
                                        case "tinyint unsigned":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(byte));
                                            break;
                                        case "smallint":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(Int16));
                                            break;
                                        case "smallint unsigned":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(UInt16));
                                            break;
                                        case "mediumint":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(int));
                                            break;
                                        case "mediumint unsigned":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(uint));
                                            break;
                                        case "bigint":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(long));
                                            break;
                                        case "bigint unsigned":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(ulong));
                                            break;
                                        case "int":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(Int32));
                                            break;
                                        case "int unsigned":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(UInt32));
                                            break;
                                        case "float":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(float));
                                            break;
                                        case "double":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(double));
                                            break;
                                        case "decimal":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(decimal));
                                            break;
                                        case "bool":
                                        case "boolean":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(bool));
                                            break;
                                        case "bit":
                                            var bitStr = Convert.ToString(Int32.Parse(data[idx].ToString()), 2);
                                            var bitTemp = bitStr.Select(c => c == '1').ToArray(); // Linq 구문에서 부하 있을 시 별도의 처리 로직 필요
                                            data[idx] = bitTemp;
                                            break;
                                        case "char":
                                        case "varchar":
                                        case "text":
                                        case "tinytext":
                                        case "mediumtext":
                                        case "longtext":
                                        case "json": // json은 별도로 처리할 것 (JsonManager.Deserialize)
                                            data[idx] = Convert.ChangeType(data[idx], typeof(string));
                                            break;
                                        case "time":
                                            var timespanTemp = TimeSpan.Parse(data[idx].ToString());
                                            data[idx] = timespanTemp;
                                            break;
                                        case "year": // 년도만 반환하므로 우선은 Int16으로 처리
                                            data[idx] = Convert.ChangeType(data[idx], typeof(Int16));
                                            break;
                                        case "date":
                                        case "datetime":
                                            // 형식 지정 필요시
                                            // DateTime.ParseExact(data[idx].ToString(), "yyyyMMdd", null);
                                            data[idx] = Convert.ChangeType(data[idx], typeof(DateTime));
                                            break;
                                        case "binary":
                                        case "varbinary":
                                        case "blob":
                                        case "tinyblob":
                                        case "mediumblob":
                                        case "longblob":
                                            string[] dataTempStr = data[idx].ToString().Split(',');
                                            byte[] dataTempByte = dataTempStr.Select(byte.Parse).ToArray();
                                            data[idx] = dataTempByte;
                                            break;
                                    }
                                }
                                else if (this.DatabaseType == DBType.sqlserver)
                                {
                                    // geography, geometry, hierachyid, timestamp 는 미포함, 필요한 경우 따로 처리 필요
                                    // geography, geometry, hierachyid는 ReadValue 대신 ReadStream으로 읽어 데이터 처리 필요
                                    // timestamp의 경우 binary 형태로 데이터가 들어옴
                                    // sql_variant의 경우 sql_variant -> sql_variant의 inner type으로 대체하여 type check 수행 (CheckVariantInnerType 함수 참고)
                                    switch (columnType)
                                    {
                                        case "int":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(int));
                                            break;
                                        case "real":
                                        case "float": // MS 권장 변경 타입은 double이나 사용에 크게 지장 없으므로 float으로 처리
                                            data[idx] = Convert.ChangeType(data[idx], typeof(float));
                                            break;
                                        case "bit":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(bool));
                                            break;
                                        case "tinyint":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(byte));
                                            break;
                                        case "smallint":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(Int16));
                                            break;
                                        case "bigint":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(Int64));
                                            break;
                                        case "smallmoney":
                                        case "money":
                                        case "numeric":
                                        case "decimal":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(decimal));
                                            break;
                                        case "char":
                                        case "nchar":
                                        case "varchar":
                                        case "nvarchar":
                                        case "text":
                                        case "ntext":
                                        case "xml":
                                            // 필요한 경우 trim 처리할 것, varchar(10) 같은 경우 나머지 자릿수는 공백으로 처리 됨
                                            data[idx] = Convert.ChangeType(data[idx], typeof(string));
                                            break;
                                        case "date":
                                        case "smalldatetime":
                                        case "datetime":
                                        case "datetime2":
                                            // 형식 지정 필요시
                                            // DateTime.ParseExact(data[idx].ToString(), "yyyyMMdd", null);
                                            data[idx] = Convert.ChangeType(data[idx], typeof(DateTime));
                                            break;
                                        case "datetimeoffset":
                                            var offsetTemp = DateTimeOffset.Parse(data[idx].ToString());
                                            data[idx] = offsetTemp;
                                            break;
                                        case "time":
                                            var timespanTemp = TimeSpan.Parse(data[idx].ToString());
                                            data[idx] = timespanTemp;
                                            break;
                                        case "binary":
                                        case "varbinary":
                                        case "rowversion":
                                        case "image":
                                            string[] dataTempStr = data[idx].ToString().Split(',');
                                            byte[] dataTempByte = dataTempStr.Select(byte.Parse).ToArray();
                                            data[idx] = dataTempByte;

                                            // image 테스트 코드, .Net Standard 환경에서 System.Drawing.Image가 기본적으로 없으므로 OpenCV 등을 이용할 것
                                            //if (dataType == "image")
                                            //{
                                            //    using (var ms = new MemoryStream(dataTempByte))
                                            //    {
                                            //        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                                            //        img.Save("D:\\image.jpg");
                                            //    }
                                            //}

                                            break;
                                        case "uniqueidentifier":
                                            Guid guidTemp = Guid.Parse(data[idx].ToString());
                                            data[idx] = guidTemp;
                                            break;
                                        case "sql_variant":
                                            data[idx] = Convert.ChangeType(data[idx], typeof(object));
                                            break;
                                    }
                                }
                                else
                                {
                                    // Not Defined
                                }
                            }

                            if (properties[j % properties.Length].CanWrite)
                            {
                                if (properties[j % properties.Length].PropertyType.Name.Contains("List"))
                                {
                                    // Property 중 List이면서 해당 DB Column Type이 varchar 계열인 경우 해당 string에 대한 후처리가 필요하므로 넘김
                                    continue;
                                }
                                else
                                {
                                    properties[j % properties.Length].SetValue(items[i], data[idx]);
                                }
                            }    
                            else
                            {
                                // Property 중 Setter가 없는 경우
                                continue;
                            }
                        }
                        else
                        {
                            // 같은 Column 이름으로 여러번 루프를 시행하므로, 처음 한 번만 Column에는 이름이 없는 Class Member들을 체크 함
                            if (i == 0)
                            {
                                notExistTemp.Add(properties[j % properties.Length].Name);
                            }

                            if (properties[j % properties.Length].CanWrite)
                            {
                                // 해당 model에서 받아온 값을 그대로 넣어 줌
                                var modelValue = modelType.GetProperty(properties[j % properties.Length].Name).GetValue(model);
                                properties[j % properties.Length].SetValue(items[i], modelValue);
                            }
                            else
                            {
                                // Property 중 Setter가 없는 경우
                                continue;
                            }
                        }
                    }
                }
            });

            for (int i = 0; i < items.Length; i++)
            {
                ret.Add(items[i]);
            }

            notExistMember = new string[notExistTemp.Count];
            notExistTemp.CopyTo(notExistMember);
            return ret;
        }

        /// <summary>
        /// MySQL 사용 시 unsigned 속성을 가진 Column을 체크
        /// </summary>
        /// <param name="info"> Column 정보 </param>
        /// <param name="tableName"> 조회할 테이블 이름 </param>
        /// <returns> Column 이름, 데이터 타입으로 구성된 Dictionary </returns>
        public Dictionary<string, string> CheckUnsignedValue(Dictionary<string, string> info, string tableName)
        {
            ArrayList checkValue = new ArrayList();
            ArrayList checkRes = null;

            if (info.ContainsValue("tinyint") || info.ContainsValue("smallint") ||
                info.ContainsValue("mediumint") || info.ContainsValue("bigint") || info.ContainsValue("int"))
            {
                string checkColumn;

                for (int i = 0; i < info.Count; i++)
                {
                    var typeValue = info.ElementAt(i).Value;

                    if (typeValue.Equals("tinyint") || typeValue.Equals("smallint") ||
                        typeValue.Equals("mediumint") || typeValue.Equals("bigint") || typeValue.Equals("int"))
                    {
                        checkColumn = info.ElementAt(i).Key.ToString();
                        checkValue.Add(checkColumn);
                    }
                }

                string unsignedStr = "";
                for (int i = 0; i < checkValue.Count; i++)
                {
                    if (i == 0)
                    {
                        unsignedStr += string.Format(" and (`COLUMN_NAME` = '{0}'", checkValue[i]);
                    }
                    else if (i == checkValue.Count - 1)
                    {
                        unsignedStr += string.Format(" or `COLUMN_NAME` = '{0}')", checkValue[i]);
                    }
                    else
                    {
                        unsignedStr += string.Format(" or `COLUMN_NAME` = '{0}'", checkValue[i]);
                    }

                }

                unsignedStr = string.Format("select `COLUMN_TYPE` from `INFORMATION_SCHEMA`.`COLUMNS` where `TABLE_SCHEMA` = '{0}' and `TABLE_NAME` = '{1}' {2}"
                                            , m_strDatabaseName, tableName, unsignedStr);

                checkRes = GetResultData(unsignedStr);

                if (checkRes != null)
                {
                    for (int i = 0; i < checkValue.Count; i++)
                    {
                        if (checkRes[i].ToString().StartsWith("!"))
                        {
                            checkRes[i] = checkRes[i].ToString().Substring(1);
                        }

                        if (checkRes[i].ToString().EndsWith("unsigned"))
                        {
                            info[checkValue[i].ToString()] += " unsigned";
                        }
                    }
                }
            }

            return info;
        }

        /// <summary>
        /// MSSQL 사용 시 sql_variant 내부 타입을 체크
        /// </summary>
        /// <param name="info"> Column 정보 </param>
        /// <param name="tableName"> 조회할 테이블 이름 </param>
        /// <returns> Column 이름, 데이터 타입으로 구성된 Dictionary </returns>
        public Dictionary<string, string> CheckVariantInnerType(Dictionary<string, string> info, string tableName)
        {
            ArrayList variant = new ArrayList();
            ArrayList variantRes = null;

            if (info.ContainsValue("sql_variant"))
            {
                string variantColumn;

                for (int i = 0; i < info.Count; i++)
                {
                    if (info.ElementAt(i).Value.Equals("sql_variant"))
                    {
                        variantColumn = info.ElementAt(i).Key.ToString();
                        variant.Add(variantColumn);
                    }
                }

                string variantStr = "";

                for (int i = 0; i < variant.Count; i++)
                {
                    variantStr += string.Format("SQL_VARIANT_PROPERTY({0}, 'BaseType'), ", variant[i].ToString());
                }

                if (variantStr.EndsWith(", "))
                {
                    variantStr = variantStr.Substring(0, variantStr.Length - 2);
                    variantStr = string.Format("select {0} from {1}", variantStr, tableName);
                }

                variantRes = GetResultData(variantStr);

                if (variantRes != null)
                {
                    for (int i = 0; i < variant.Count; i++)
                    {
                        if (variantRes[i].ToString().StartsWith("!"))
                        {
                            variantRes[i] = variantRes[i].ToString().Substring(1);
                        }

                        info[variant[i].ToString()] = variantRes[i].ToString();
                    }
                }
            }

            return info;
        }

        /// <summary>
        /// 해당 테이블의 칼럼들의 이름과 자료형을 받아온다.
        /// </summary>
        /// <param name="strTableName"> 테이블 이름 </param>
        /// <param name="strDBName"> DB 이름 </param>
        /// <returns> 칼럼 이름 (string) - 자료형 (string)으로 구성된 Dictionary </returns>
        public Dictionary<string, string> GetColumnInfoDictionary(string strTableName, string strDBName = null)
        {
            string[] keys = GetColumnNameStringArray(strTableName);
            string[] values = GetColumnTypeStringArray(strTableName);

            Dictionary<string, string> ret = new Dictionary<string, string>();

            if (keys != null && values != null)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    ret.Add(keys[i], values[i]);
                }
            }

            return ret;
        }

        /// <summary>
        /// 해당 테이블의 칼럼 자료형을 받아온다.
        /// </summary>
        /// <param name="strTableName"> 테이블 이름 </param>
        /// <param name="strDBName"> DB 이름 </param>
        /// <returns> 칼럼 자료형 (string)으로 구성된 string array </returns>
        public string[] GetColumnTypeStringArray(string strTableName, string strDBName = null)
        {
            string query = "";

            if (strDBName == null)
            {
                strDBName = m_strDatabaseName;
            }

            if (this.DatabaseType == DBType.mysql)
            {
                query = string.Format("select `DATA_TYPE` from `INFORMATION_SCHEMA`.`COLUMNS` where `TABLE_SCHEMA` = '{0}' and `TABLE_NAME` = '{1}'"
                                        , m_strDatabaseName, strTableName);
            }
            else if (this.DatabaseType == DBType.sqlserver)
            {
                query = string.Format("select DATA_TYPE from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{0}'", strTableName);
            }
            else
            {
                // Not Defined
            }

            ArrayList res = GetReadDB(query, strDBName);

            if (res != null)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i].ToString().StartsWith("!"))
                    {
                        res[i] = res[i].ToString().Substring(1);
                    }
                }

                return res.ToArray(typeof(string)) as string[];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 해당 테이블의 칼럼 이름을 받아온다.
        /// </summary>
        /// <param name="strTableName"> 테이블 이름 </param>
        /// <param name="strDBName"> DB 이름 </param>
        /// <returns> 칼럼 이름으로 구성된 string (구분은 ,) </returns>
        public string GetColumnNameString(string strTableName, string strDBName = null)
        {
            string query = "";

            if (strDBName == null)
            {
                strDBName = m_strDatabaseName;
            }

            if (this.DatabaseType == DBType.mysql)
            {
                query = string.Format("select `COLUMN_NAME` from `INFORMATION_SCHEMA`.`COLUMNS` where `TABLE_SCHEMA` = '{0}' and `TABLE_NAME` = '{1}'"
                                        , m_strDatabaseName, strTableName);
            }
            else if (this.DatabaseType == DBType.sqlserver)
            {
                query = string.Format("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{0}'", strTableName);
            }
            else
            {
                // Not Defined
            }

            ArrayList res = GetReadDB(query, strDBName);

            if (res != null)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i].ToString().StartsWith("!"))
                    {
                        res[i] = res[i].ToString().Substring(1);
                    }
                }

                return string.Join(",", res.ToArray(typeof(string)) as string[]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 해당 테이블의 칼럼 이름을 받아온다.
        /// </summary>
        /// <param name="strTableName"> 테이블 이름 </param>
        /// <param name="strDBName"> DB 이름 </param>
        /// <returns> 칼럼 이름 (string)으로 구성된 string array </returns>
        public string[] GetColumnNameStringArray(string strTableName, string strDBName = null)
        {
            string query = "";

            if (strDBName == null)
            {
                strDBName = m_strDatabaseName;
            }

            if (this.DatabaseType == DBType.mysql)
            {
                query = string.Format("select `COLUMN_NAME` from `INFORMATION_SCHEMA`.`COLUMNS` where `TABLE_SCHEMA` = '{0}' and `TABLE_NAME` = '{1}'"
                                        , m_strDatabaseName, strTableName);
            }
            else if (this.DatabaseType == DBType.sqlserver)
            {
                query = string.Format("select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{0}'", strTableName);
            }
            else
            {
                // Not Defined
            }

            ArrayList res = GetReadDB(query, strDBName);

            if (res != null)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i].ToString().StartsWith("!"))
                    {
                        res[i] = res[i].ToString().Substring(1);
                    }
                }

                return res.ToArray(typeof(string)) as string[];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Insert 시 필요한 Value를 string 형태로 만들어 준다.
        /// </summary>
        /// <param name="param"> 필요한 Value </param>
        /// <returns> Value 값이 포함된 string (구분은 ,) </returns>
        public string ConvertParamsToString(params object[] param)
        {
            if (param.Length > 0)
            {
                string ret = "";
                string temp = "";

                for (int i = 0; i < param.Length; i++)
                {
                    if (param[i] != null)
                    {
                        if (param[i].GetType() == typeof(string) || param[i].GetType() == typeof(bool))
                        {
                            temp = string.Format("'{0}'", param[i].ToString());
                        }
                        else if (param[i].GetType() == typeof(DateTime))
                        {
                            DateTime timeTemp = (DateTime)param[i];
                            string format = "yyyy-MM-dd HH:mm:ss";
                            string timeStr = timeTemp.ToString(format);
                            temp = string.Format("'{0}'", timeStr);
                        }
                        else
                        {
                            temp = string.Format("{0}", param[i].ToString());
                        }
                    }
                    else
                    {
                        temp = string.Format("NULL");
                    }

                    if (i == param.Length - 1)
                    {
                        ret += string.Format("{0}", temp);
                    }
                    else
                    {
                        ret += string.Format("{0},", temp);
                    }
                }

                return ret;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 해당 테이블의 마지막 Index(ID)를 얻어온다.
        /// </summary>
        /// <param name="tableName"> 테이블 이름 </param>
        /// <returns> 마지막 Index(ID) </returns>
        public int? GetLastIndex(string tableName)
        {
            string query = string.Format("select max(ID) from {0}", tableName);
            ArrayList res = GetResultData(query);

            if (res != null)
            {
                if (res[0].ToString().StartsWith("!"))
                {
                    res[0] = res[0].ToString().Substring(1);
                    return Convert.ToInt32(res[0].ToString());
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                // Error
                return null;
            }
        }

        /// <summary>
		/// INI파일에서 항목 가져오기
		/// </summary>
        //public virtual string LoadIni(string strTargetName)
        //{
        //    string strSection = "Server Connection Info";
        //    return m_ini.getinivalue(strSection, strTargetName);
        //}
        //
        //public virtual string LoadIni(string strTargetName, string strSectionName)
        //{
        //    return m_ini.getinivalue(strSectionName, strTargetName);
        //}
        //
        //public virtual string SaveIni(string strTargetName, string strValue, string strSectionName)
        //{
        //    return m_ini.setinivalue(strSectionName, strTargetName, strValue);
        //}
    }
}
