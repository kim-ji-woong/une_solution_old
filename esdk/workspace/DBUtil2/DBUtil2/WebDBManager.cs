using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace DBUtility2
{
    using WebDBService;

    public class WebDBManager : IDisposable
    {
        public enum DBType { sqlserver = 0, mysql, TypeCount };

        private const string NOT_CONNECTED_EXCEPTION = "WebDB 접속이 끊어졌습니다.\r\n서버 관리자에게 문의하세요.";
        private const int MaxContentLength = 8192;

        private int m_nSiteID = 0;
        private string m_strWebServerURL = "";
        private string m_strDatabaseName = "";
        private DBType m_dbType = DBType.sqlserver;

        private string m_strLastErrorMsg = "";

        private bool m_isBeginBatch = false;
        private string m_strBatchDB = "";
        private long m_nBatchCode = 0;

        // Transaction을 위한 데이터
        private IWebDB m_proxy = null;
        private ChannelFactory<IWebDB> m_factory = null;

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

        private Utility m_ini = new Utility();

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
            LoadConnectionInfo(nSiteID);

            m_strDatabaseName = strDatabaseName;
        }

        public WebDBManager(string strDatabaseName, string strDBType, int nSiteID)
        {
            m_nSiteID = nSiteID;
            LoadConnectionInfo(nSiteID);

            m_strDatabaseName = strDatabaseName;
            this.DatabaseTypeName = strDBType;
        }

        public void Dispose()
        {
            if (m_factory != null)
            {
                m_factory.Close();
                m_proxy = null;
                m_factory = null;
            }
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
            string strSection = "Server Connection Info";

            m_strWebServerURL = RegUtil.ReadRegValue(strSection, "webserver_url2", nSiteID);
            if (m_strWebServerURL == null || m_strWebServerURL == "")
            {
                SetDefaultWebServerURL(nSiteID);
                RegUtil.WriteRegValue(strSection, "webserver_url2", m_strWebServerURL, nSiteID);
            }

            m_strDatabaseName = RegUtil.ReadRegValue(strSection, "db_name", nSiteID);

            if (m_strDatabaseName == null || m_strDatabaseName == "")
            {
                SetDefaultDBName(nSiteID);
                RegUtil.WriteRegValue(strSection, "db_name", m_strDatabaseName, nSiteID);
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
                default:
                    m_strDatabaseName = "SOP_" + nSiteID.ToString();
                    break;
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
            if (nLimit > 0)
            {
                if (m_dbType == DBType.mysql)
                //if( m_szDatabaseType == "mysql")
                {
                    strSQL += " LIMIT 0," + nLimit;
                }
                else if (m_dbType == DBType.sqlserver)
                //else if( m_szDatabaseType == "sqlserver")
                {
                    int nIdx = strSQL.ToLower().IndexOf("select");
                    if (nIdx >= 0)
                    {
                        strSQL = strSQL.Insert(6, " TOP " + nLimit + " ");
                    }
                }
            }
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

        private void ChangeQuery(ref string strSQL, string strSrc, string strTrg)
        {
            int nIndex = strSQL.ToLower().IndexOf(strSrc);

            if (nIndex >= 0)
            {
                string str = strSQL.Substring(nIndex, strSrc.Length);
                strSQL = strSQL.Replace(str, strTrg);
            }
        }

        // Transaction을 위한 함수
        private IWebDB GetProxy(out ChannelFactory<IWebDB> factory)
        {
            if (m_proxy != null)
            {
                factory = m_factory;
                return m_proxy;
            }

            // 기본옵션은 쿼리 결과가 65536 바이트를 넘어서면 오류를 일으킨다.
            ServiceEndpoint ep = MakeEndpoint(m_strWebServerURL, "WebDBService", typeof(IWebDB));
            /*Uri uri = new Uri(m_strWebServerURL + "/WebDBService.svc");

            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(typeof(IWebDB)),
                new BasicHttpBinding(),
                new EndpointAddress(uri));*/

            factory = new ChannelFactory<IWebDB>(ep);
            IWebDB proxy = factory.CreateChannel();

            m_proxy = proxy;
            m_factory = factory;

            return proxy;
        }

        // Transaction을 위한 함수
        private IWebDB GetProxyTemp(out ChannelFactory<IWebDB> factory)
        {
            // 기본옵션은 쿼리 결과가 65536 바이트를 넘어서면 오류를 일으킨다.
            ServiceEndpoint ep = MakeEndpoint(m_strWebServerURL, "WebDBService", typeof(IWebDB));
            
            factory = new ChannelFactory<IWebDB>(ep);
            IWebDB proxy = factory.CreateChannel();

            return proxy;
        }

        private ServiceEndpoint MakeEndpoint(string strWebServerURL, string strServiceName, Type contractType)
        {
            System.Xml.XmlDictionaryReaderQuotas readerQuotas = new System.Xml.XmlDictionaryReaderQuotas();
            readerQuotas.MaxDepth = 128;
            readerQuotas.MaxStringContentLength = 2147483647;
            readerQuotas.MaxArrayLength = 2147483647;
            readerQuotas.MaxBytesPerRead = 31457280;
            readerQuotas.MaxNameTableCharCount = 16384;

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MessageEncoding = WSMessageEncoding.Text;
            binding.MaxBufferPoolSize = 31457280;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.ReaderQuotas = readerQuotas;

            Uri uri = new Uri(strWebServerURL + "/" + strServiceName + ".svc");
            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(contractType),
                binding,
                new EndpointAddress(uri));

            return ep;
        }

        /*private WebDBClient GetWebDBClient()
        {
            WebDBClient webDB = new WebDBClient("BasicHttpBinding_IWebDB", m_strWebServerURL + "/WebDBService.svc");
            return webDB;
        }*/

        private ArrayList GetReadDB(string strSQL, string strDBName)
        {
            try
            {
                ChannelFactory<IWebDB> factory;
                // 동기화 문제가 있어서 GetProxy() 대신 GetProxyTemp()를 사용한다.
                // GetProxy()는 Transaction 사용시에만 사용한다.
                IWebDB webDB = GetProxyTemp(out factory);
                //IWebDB webDB = GetProxy(out factory);

                string[] results = null;
                int len = strSQL.Length;

                if (len <= MaxContentLength)
                    results = webDB.RunQuery(strDBName, DatabaseTypeName, strSQL);
                else
                {
                    long key = webDB.BeginMultiQuery();

                    if (key <= 0)
                    {
                        m_strLastErrorMsg = "알수없는 오류입니다.";
                        factory.Close();
                        return null;
                    }

                    for (int i=0;i<len;i+=MaxContentLength)
                    {
                        int contentLength = i + MaxContentLength <= len ? MaxContentLength : len - i;
                        string str = strSQL.Substring(i, contentLength);
                        string strResult = webDB.AddMultiQuery(str, key);

                        if (strResult == null)
                        {
                            m_strLastErrorMsg = "알수없는 오류입니다.";
                            factory.Close();
                            return null;
                        }
                        else if (strResult.Length > 0)
                        {
                            m_strLastErrorMsg = strResult;
                            factory.Close();
                            return null;
                        }
                    }

                    results = webDB.RunMultiQuery(strDBName, DatabaseTypeName, key);
                }

                //string[] results = webDB.RunQuery(strDBName, DatabaseTypeName, strSQL);
                factory.Close();
                //m_factory = null;
                //m_proxy = null;

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
                //m_factory = null;
                //m_proxy = null;
                m_strLastErrorMsg = e.Message;
                //m_strLastErrorMsg = NOT_CONNECTED_EXCEPTION;
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
                ChannelFactory<IWebDB> factory;
                IWebDB webDB = GetProxy(out factory);
                //WebDBClient webDB = GetWebDBClient();

                BeginBatchRequest request = new BeginBatchRequest(m_strBatchDB, DatabaseTypeName);
                BeginBatchResponse response = webDB.BeginBatch(request);
                //webDB.Close();
                //factory.Close();

                if (response.BeginBatchResult == 0)
                {
                    if (response.errorMessage == null)
                    {
                        m_strLastErrorMsg = "알수없는 오류입니다.";
                        return false;
                    }
                    else
                    {
                        m_strLastErrorMsg = response.errorMessage;
                        return false;
                    }
                }
                else
                    m_nBatchCode = response.BeginBatchResult;
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                m_isBeginBatch = false;
                m_factory = null;
                m_proxy = null;
                m_strLastErrorMsg = NOT_CONNECTED_EXCEPTION;
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
                ChannelFactory<IWebDB> factory;
                IWebDB webDB = GetProxy(out factory);
                //WebDBClient webDB = GetWebDBClient();
                string strResult = webDB.BatchCommmit(m_nBatchCode);
                //webDB.Close();
                //factory.Close();
                m_factory.Close();
                m_factory = null;
                m_proxy = null;

                m_isBeginBatch = false;

                if (strResult == null)
                {
                    m_strLastErrorMsg = "알수없는 오류입니다.";
                    return false;
                }
                else if (strResult.Length > 0)
                {
                    m_strLastErrorMsg = strResult;
                    return false;
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                m_isBeginBatch = false;
                m_factory = null;
                m_proxy = null;
                m_strLastErrorMsg = NOT_CONNECTED_EXCEPTION;
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
                ChannelFactory<IWebDB> factory;
                IWebDB webDB = GetProxy(out factory);
                //WebDBClient webDB = GetWebDBClient();
                string strResult = webDB.BatchRollback(m_nBatchCode);
                //webDB.Close();
                //factory.Close();
                m_factory.Close();
                m_factory = null;
                m_proxy = null;

                m_isBeginBatch = false;

                if (strResult == null)
                {
                    m_strLastErrorMsg = "알수없는 오류입니다.";
                    return false;
                }
                else if (strResult.Length > 0)
                {
                    m_strLastErrorMsg = strResult;
                    return false;
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                m_isBeginBatch = false;
                m_factory = null;
                m_proxy = null;
                m_strLastErrorMsg = NOT_CONNECTED_EXCEPTION;
            }

            return true;
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

        private ArrayList GetBatchDB(string strSQL)
        {
            try
            {
                ChannelFactory<IWebDB> factory;
                IWebDB webDB = GetProxy(out factory);
                //WebDBClient webDB = GetWebDBClient();

                string[] results = null;
                int len = strSQL.Length;

                if (len <= MaxContentLength)
                    results = webDB.BatchQuery(strSQL, m_nBatchCode);
                else
                {
                    long key = webDB.BeginMultiQuery();

                    if (key <= 0)
                    {
                        m_strLastErrorMsg = "알수없는 오류입니다.";
                        factory.Close();
                        return null;
                    }

                    for (int i = 0; i < len; i += MaxContentLength)
                    {
                        int contentLength = i + MaxContentLength <= len ? MaxContentLength : len - i;
                        string str = strSQL.Substring(i, contentLength);
                        string strResult = webDB.AddMultiQuery(str, key);

                        if (strResult == null)
                        {
                            m_strLastErrorMsg = "알수없는 오류입니다.";
                            factory.Close();
                            return null;
                        }
                        else if (strResult.Length > 0)
                        {
                            m_strLastErrorMsg = strResult;
                            factory.Close();
                            return null;
                        }
                    }

                    results = webDB.BatchMultiQuery(key, m_nBatchCode);
                }
                //webDB.Close();
                //factory.Close();

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
            catch (Exception)
            {
                m_isBeginBatch = false;
                m_factory = null;
                m_proxy = null;
                m_strLastErrorMsg = NOT_CONNECTED_EXCEPTION;
            }

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
