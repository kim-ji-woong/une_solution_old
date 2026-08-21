using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace DBUtility
{
    public class WebDBManager : IDisposable
    {
        public enum DBType { sqlserver = 0, mysql, TypeCount };

		// 세션 유지용 쿠키
		private CookieContainer cookieContainer = new CookieContainer();
		private CookieContainer m_batchCookie = new CookieContainer();

        private Utility m_ini = new Utility();

        private string m_strWebServerURL = "";
        private int m_nSiteID = 0;

        public string WebServerURL
        {
            get { return m_strWebServerURL; }
            set { m_strWebServerURL = value; }
        }
		
		private string m_szWebPageName = "DBQuery2.jsp";
		public string WebQueryPage
		{
			get { return m_szWebPageName; }
			set { m_szWebPageName = value; }
		}

		private string m_szProcPageName = "RunStoredProcedure2.jsp";
		public string WebProcPage
		{
			get { return m_szProcPageName; }
			set { m_szProcPageName = value; }
		}

		private string m_szBatchPage = "BatchQuery.jsp";
		public string WebBatchPage
		{
			get { return m_szBatchPage; }
			set { m_szBatchPage = value; }
		}

        private string m_szDBHost = null;
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

		private string m_szDatabaseName = "SOP_1";
		public string DatabaseName
		{
			get { return m_szDatabaseName; }
			set { m_szDatabaseName = value; }
		}

        private DBType m_dbType = DBType.sqlserver;
        public DBType DatabaseType
        {
            get { return m_dbType; }
            set {
                m_dbType = value;
                SetPort();
            }
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

        public WebDBManager(int nSiteID)
        {
            m_nSiteID = nSiteID;
            Loadini_ServerConnectionInfo(nSiteID);

            //if(nSiteID == 100)
            //{
            //    m_szDatabaseName = "EDU_" + nSiteID;
            //}
        }

        public WebDBManager(string strDatabaseName, int nSiteID)
        {
            m_nSiteID = nSiteID;
            Loadini_ServerConnectionInfo(nSiteID);
            m_szDatabaseName = strDatabaseName;
        }

        public WebDBManager(string strDatabaseName, string strDBType, int nSiteID)
        {
            Loadini_ServerConnectionInfo(nSiteID);
            m_szDatabaseName = strDatabaseName;
            this.DatabaseTypeName = strDBType;

            SetPort();
        }

        static public T GetField<T>(object dataSrc, T dataDefault)
        {
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

            if (dataSrc.GetType() != typeof(string))
            {
                return dataSrc.ToString();
            }

            try
            {
                result = (string)dataSrc;
                result = result.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                result = result.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                // (char)5, 6, 7, 8은 DB 입력시 '\t', '\n', '\r', '\''이 임시로 바뀌어 들어간 값이므로, 다시 '\n'으로 되돌려 준다.
               
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

                // (char)5, 6, 7, 8은 DB 입력시 '\t', '\n', '\r', '\''이 임시로 바뀌어 들어간 값이므로, 다시 '\n'으로 되돌려 준다.
              
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
                GetReadDB("close", 0, m_szDatabaseName);
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
            string strDBType  = RegUtil.ReadRegValue(strSection, "db_type", nSiteID);

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

            SetPort();
            
            string szEncoding = RegUtil.ReadRegValue(strSection, "page_encoding", nSiteID);
            if( szEncoding == null || szEncoding == "")
            {
                szEncoding = SetDefaultEncoding(nSiteID);
                RegUtil.WriteRegValue(strSection, "page_encoding", szEncoding, nSiteID);
            }
            try
            {
                int nEncoding = -1;
                if(int.TryParse(szEncoding, out nEncoding))
                {
                    m_PageEncoding = System.Text.Encoding.GetEncoding(nEncoding);
                }
                else
                {
                    m_PageEncoding = Encoding.UTF8;
                }
            }
            catch(Exception)
            {
                m_PageEncoding = Encoding.UTF8;
            }
		}

        private void SetPort()
        {
            if (DatabaseType == DBType.mysql)
            {
                m_szDBPort = "3306";
            }
            else if (DatabaseType == DBType.sqlserver)
            {
                m_szDBPort = "1433";
            }

        }

        private System.Text.Encoding m_PageEncoding = null;
        private string SetDefaultEncoding(int nSiteID)
        {
            m_PageEncoding = System.Text.Encoding.GetEncoding(65001);
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

                case 3:
                    szResult = Encoding.UTF8.CodePage.ToString();
                    break;
                // 서울대
                case 100:
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
                case 3:
                    m_strWebServerURL = "http://192.168.0.195:8080/SOP";
                    break;
                // 서울대학교
                case 100:
                    m_strWebServerURL = "http://192.168.250.41:8080/SOP";
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
                    m_szDatabaseName = "SOP_" + nSiteID.ToString();
                    break;
                // 영흥
                case 2:
                    m_szDatabaseName = "SOP_" + nSiteID.ToString();
                    break;
                case 3:
                    m_szDatabaseName = "SOP_" + nSiteID.ToString();
                    break;
                // 서울대학교
                case 100:
                    m_szDatabaseName = "EDU_" + nSiteID.ToString();
                    break;
                default:
                    m_szDatabaseName = "SOP_" + nSiteID.ToString();
                    break;
            }
            
        }

		///////////////////////////////////////////////////////////////////////////////////
		// INI function
		///////////////////////////////////////////////////////////////////////////////////

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

        private ArrayList GetReadDB(string strSQLQuery)
        {
            string strFilePath = "WebDB.exe";

            if (File.Exists(strFilePath) == false)
            {
                return null;
            }

            Guid guid = Guid.NewGuid();
            string strArgument = string.Format("{0} {1} \"{2}\"", m_nSiteID, guid.ToString(), strSQLQuery);

            System.Diagnostics.Process.Start(strFilePath, strArgument);

            string strTargetFilePath = guid.ToString() + ".txt";

            for (int i = 0; i < 30 && File.Exists(strTargetFilePath) == false; i++)
            {
                System.Threading.Thread.Sleep(100);
            }

            if (File.Exists(strTargetFilePath) == false)
                return null;

            ArrayList arrResult = new ArrayList();
            StreamReader reader = new StreamReader(strTargetFilePath, System.Text.Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine == "null")
                {
                    reader.Close();
                    File.Delete(strTargetFilePath);
                    return null;
                }
                else
                {
                    if (strLine[0] == '~')
                        arrResult.Add("null");
                    else
                        arrResult.Add(strLine.Substring(1));
                }
            }

            reader.Close();
            File.Delete(strTargetFilePath);
            return arrResult;
        }
		
		///////////////////////////////////////////////////////////////////////////////////
		// Normal Mode function
		///////////////////////////////////////////////////////////////////////////////////

        private string GetReadDB(string strSQLQuery, int nTransaction, string szDBName)
        {
            string resResult = string.Empty;

            if (m_strWebServerURL == null || m_strWebServerURL.Equals(""))
            {
                m_strWebServerURL = "http://192.168.0.207:8088/SOP";
            }
            string sourceUrl = m_strWebServerURL + "/" + m_szWebPageName;

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);

			if (szDBName == null)
				szDBName = m_szDatabaseName;

            string postData = "SQLQuery=" + strUrlEncode + "&" + "Transaction=" + nTransaction;
            postData += "&" + "DatabaseName=" + szDBName + "&" + "DatabaseType=" + DatabaseTypeName;//m_szDatabaseType;

            if (m_szDBHost != null)
                postData += "&" + "Host=" + m_szDBHost;
            if (m_szDBPort != null)
                postData += "&" + "Port=" + m_szDBPort;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);
            
            HttpWebRequest mRequest = (HttpWebRequest)WebRequest.Create(sourceUrl);
            //mRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            //lock (this)
            {
                mRequest.Method = "POST";
                mRequest.ContentType = "application/x-www-form-urlencoded";
                mRequest.ContentLength = bytes.Length;
                mRequest.CookieContainer = cookieContainer;

                //System.Diagnostics.Trace.WriteLine(mRequest.Headers["Cookie"]);
                try
                {
                    using (Stream writeStream = mRequest.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }

                    HttpWebResponse wRes = (HttpWebResponse)mRequest.GetResponse();
                   
                    Stream respPostStream = wRes.GetResponseStream();
                    StreamReader readerPost = new StreamReader(respPostStream, m_PageEncoding);

                   


                    resResult = readerPost.ReadToEnd();
                    mRequest.Abort();
                    readerPost.Close();
                    respPostStream.Close();
                }
                catch (System.Net.WebException ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    //System.Windows.Forms.MessageBox.Show(e.Message);
                    return "";
                }
            }

            return resResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSQLQuery">실행할 쿼리</param>
        /// <param name="nTranstion">1인경우 트랜잭션, 0이면 단일쿼리</param>
        /// <param name="nLimit">최대 행 개수</param>
        /// <param name="szDBName">사용할 Database이름</param>
        /// <returns></returns>
        public virtual ArrayList GetResultData(string strSQLQuery, int nTranstion, int nLimit, string szDBName = null)
        {
            if( nLimit > 0)
            {
                if (m_dbType == DBType.mysql)
                //if( m_szDatabaseType == "mysql")
                {
                    strSQLQuery += " LIMIT 0," + nLimit;
                }
                else if (m_dbType == DBType.sqlserver)
                //else if( m_szDatabaseType == "sqlserver")
                {
                    int nIdx = strSQLQuery.ToLower().IndexOf("select");
                    if( nIdx >= 0)
                    {
                        strSQLQuery = strSQLQuery.Insert(6, " TOP " + nLimit + " ");
                    }                    
                }
            }
            return GetResultData(strSQLQuery, nTranstion, szDBName);
        }

		public virtual ArrayList GetResultData(string strSQLQuery, int nTranstion, string szDBName = null)
		{
            //if( m_bBatch == false && nTranstion == 1)
            //{
            //    throw new WebDBTransactionStateException("트랜잭션이 시작되지 않았습니다.");
            //}

            // str에 '\n', '\r' 이 포함되어 있으면 다른 문자로 바꾼다.
			strSQLQuery = strSQLQuery.Replace('\n', (char)6);
			strSQLQuery = strSQLQuery.Replace('\r', (char)7);       

            if (this.DatabaseType == DBType.mysql)
            {
                strSQLQuery = strSQLQuery.Replace("ISNULL", "IFNULL");
                strSQLQuery = strSQLQuery.Replace("isnull", "ifnull");
                strSQLQuery = strSQLQuery.Replace("\\", "\\\\");
            }
            else if (this.DatabaseType == DBType.sqlserver)
            {
                strSQLQuery = strSQLQuery.Replace("IFNULL", "ISNULL");
            }

			if (szDBName == null)
				szDBName = m_szDatabaseName;

            string resResult = "";

            return GetReadDB(strSQLQuery);
            //resResult = GetReadDB(strSQLQuery, nTranstion, szDBName);
            m_lastResult = resResult;

            ArrayList arrResult = ParseResult(resResult);

            if(this.m_szLastError != "성공")
            {
                System.Diagnostics.Trace.WriteLine("Error SQL : " + strSQLQuery);
                System.Diagnostics.Trace.WriteLine("Error Msg : " + m_szLastErrorMsg);
            }        
            return arrResult;
		}

		///////////////////////////////////////////////////////////////////////////////////
		// Batch Mode function
		///////////////////////////////////////////////////////////////////////////////////

		private bool m_bBatch = false;
		private string m_szBatchDB = "";

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

        private bool nBeginBatch = false;
        private string szBatchSessionID = "";
		public virtual bool BeginBatch(string szDBName = null)
		{
            if (m_bBatch == true)
            {
                throw new WebDBTransactionStateException("이전 트랜잭션이 종료되지 않았습니다.\nRollback이나 Commit이후에 호출 가능합니다.");
            }
			m_bBatch = true;
			m_szBatchDB = (szDBName == null ? m_szDatabaseName : szDBName);

            nBeginBatch = true;
            szBatchSessionID = "";
			return true;
		}

		public virtual void BatchCommit()
		{
            nBeginBatch = false;

            if (m_bBatch == false)
            {
                throw new WebDBTransactionStateException("트랜잭션이 시작되지 않았습니다.\nCommit은 BeginBatch이후에 호출 가능합니다.");
            }

			string resResult = GetBatchReadDB("", "Commit", m_szBatchDB);
			m_bBatch = false;
			m_szBatchDB = "";
            szBatchSessionID = "";
		}

		public virtual void BatchRollback()
		{
            nBeginBatch = false;

            if (m_bBatch == false)
            {
                throw new WebDBTransactionStateException("트랜잭션이 시작되지 않았습니다.\nRollback은 BeginBatch이후에 호출 가능합니다.");
            }

			string resResult = GetBatchReadDB("select", "RollBack", m_szBatchDB);
			m_bBatch = false;
			m_szBatchDB = "";

            szBatchSessionID = "";
		}

		private string GetBatchReadDB(string strSQLQuery, string szCmd , string szDBName )
		{
			string resResult = string.Empty;

			if (m_strWebServerURL == null || m_strWebServerURL.Equals(""))
			{
				m_strWebServerURL = "http://192.168.0.207:8088/SOP";
			}
			string sourceUrl = m_strWebServerURL + "/" + m_szBatchPage;

			UTF8Encoding enc = new UTF8Encoding();
			byte[] bytes1 = enc.GetBytes(strSQLQuery);
			string strUrlEncode = URLEncoding(bytes1);

			string postData = "SQLQuery=" + strUrlEncode + "&" + "Cmd=" + szCmd;
            postData += "&" + "DatabaseName=" + szDBName + "&" + "DatabaseType=" + DatabaseTypeName;//m_szDatabaseType;


            if (m_szDBHost != null)
                postData += "&" + "Host=" + m_szDBHost;
            if (m_szDBPort != null)
                postData += "&" + "Port=" + m_szDBPort;

			UTF8Encoding encoding = new UTF8Encoding();
			byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest mRequest = (HttpWebRequest)WebRequest.Create(sourceUrl);
            bool bSessionError = false;
			//lock (this)
			{
                mRequest.Method = "POST";
				//wReq.UserAgent = "Mozilla/4.0";
                mRequest.ContentType = "application/x-www-form-urlencoded";
                mRequest.ContentLength = bytes.Length;
                mRequest.CookieContainer = m_batchCookie;
                
				try
				{
                    using (Stream writeStream = mRequest.GetRequestStream())
					{
						writeStream.Write(bytes, 0, bytes.Length);
					}

                    HttpWebResponse wRes = (HttpWebResponse)mRequest.GetResponse();
					Stream respPostStream = wRes.GetResponseStream();


                    StreamReader readerPost = new StreamReader(respPostStream, m_PageEncoding); 

					resResult = readerPost.ReadToEnd();

                   
                    foreach (Cookie cookieValue in wRes.Cookies)
                    {
                        if (nBeginBatch == true)
                        {                        
                            if (cookieValue.Name == "JSESSIONID")
                            {
                                szBatchSessionID = cookieValue.Value;
                                break;
                            }
                        }
                        else
                        {
                            if (cookieValue.Name == "JSESSIONID")
                            {
                                if (cookieValue.Value != szBatchSessionID)
                                {
                                    bSessionError = true;
                                    break;
                                }
                            }
                        }
                    }

                    mRequest.Abort();
					readerPost.Close();
					respPostStream.Close();    
              
				}
				catch (System.Net.WebException)
				{
					return "";
				}
			}

            if (bSessionError == true)
            {
                throw new WebDBTransactionStateException("트랜잭션 호출 시간이 종료 되었습니다.");
            }

			return resResult;
		}

        private DBResultList ParseResult(string resResult)
        {
            DBResultList arrResult = new DBResultList();
            StringFile strFile = new StringFile(resResult);

            string strResult = "";

            bool isResult = true;
            bool isBegin = false;
            bool bError = false;

            while (isResult)
            {
                isResult = strFile.ReadLine(ref strResult);

                if (isResult)
                {
                    if (strResult == "Begin Data")
                    {
                        isBegin = true;
                        continue;
                    }

                    if (strResult.StartsWith("JDBC 드라이브 연결 오류"))
                    {
                        string szTemp = strResult.Replace("JDBC 드라이브 연결 오류-", "");
                        string[] errs = szTemp.Split(':');
                        if (errs != null && errs.Length > 2)
                        {
                            m_szLastError = errs[0];
                            m_szLastErrorMsg = errs[1];

                        }
                        else
                        {
                            m_szLastError = "실패";
                            m_szLastErrorMsg = szTemp;
                        }
                        bError = true;
                        continue;
                    }

                    if (strResult == "End Data")
                        break;

                    if (isBegin)
                    {
                        if (strResult == "null_SQLError")
                        {
                            m_szLastError = "실패";
                            bError = true;
                            break;
                        }
                        else
                            arrResult.Add(strResult);
                    }
                }
            }

            arrResult.Column = strFile.Column;
            arrResult.Row = strFile.Row;

            if (bError == true)
            {
                return null;
            }
          
            m_szLastError = "성공";
            m_szLastErrorMsg = "성공";

            return arrResult;
        }


        public ArrayList GetBatchData(string strSQLQuery, int nLimit, string szDBName = null)
        {
            if (nLimit > 0)
            {
                if (m_dbType == DBType.mysql)
                {
                    strSQLQuery += " LIMIT 0," + nLimit;
                }
                else if (m_dbType == DBType.sqlserver)
                {
                    int nIdx = strSQLQuery.ToLower().IndexOf("select");
                    if (nIdx >= 0)
                    {
                        strSQLQuery = strSQLQuery.Insert(6, " TOP " + nLimit + " ");
                    }
                }
            }
            return GetBatchData(strSQLQuery, szDBName);
        }

		// GetBatchData는 쿼리 종료시 BatchCommit또는 BatchRollback을 반드시 호출해야 한다.
        public virtual ArrayList GetBatchData(string strSQLQuery, string szDBName = null)
		{
            if (m_bBatch == false)
            {
                throw new WebDBTransactionStateException("트랜잭션이 시작되지 않았습니다.");
            }         

            if (this.DatabaseType == DBType.mysql)
            {
                strSQLQuery = strSQLQuery.Replace("ISNULL", "IFNULL");
                strSQLQuery = strSQLQuery.Replace("isnull", "ifnull");
                strSQLQuery = strSQLQuery.Replace("\\", "\\\\");
            }
            else if (this.DatabaseType == DBType.sqlserver)
            {
                strSQLQuery = strSQLQuery.Replace("IFNULL", "ISNULL");
            }

			if (szDBName == null)
				szDBName = m_szDatabaseName;

			if (m_bBatch == true && !m_szBatchDB.Equals(szDBName))
				return null;

			if (m_bBatch == false)
				BeginBatch(szDBName);			

			// str에 '\n', '\r'이 포함되어 있으면 다른 문자로 바꾼다.
            strSQLQuery = strSQLQuery.Replace('\n', (char)6);
            strSQLQuery = strSQLQuery.Replace('\r', (char)7);
            			
			string resResult = GetBatchReadDB(strSQLQuery, "Batch", szDBName);
            
            m_lastResult = resResult; 
            
            DBResultList arrResult = ParseResult(resResult);
   			return arrResult;
		}


        //////////////////////////////////////////////////////////////////////////
        // StoredProcedure
		//////////////////////////////////////////////////////////////////////////
        private bool SplitStoreProcedureQuery(string strSQL, ref string strMethod, ref string strParam)
        {
            int nLen = strSQL.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strSQL[i];

                if (ch == ' ' || ch == '\t')
                {
                    strMethod = strSQL.Substring(0, i);
                    strParam = strSQL.Substring(i + 1).Trim();
                    return true;
                }
            }

            return false;
        }

        // DB Type에 맞게 Query를 수정한다.
        private void ReshapeStoredProcedureQuery(ref string strSQL)
        {
            string strLower = strSQL.ToLower();
            int nCallIndex = strLower.IndexOf("call");
            int nExecIndex = strLower.IndexOf("exec");

            if (this.DatabaseType == DBType.mysql)
            {
                if (nCallIndex >= 0)
                {
                    strSQL = strSQL.Trim();
                }
                else
                {
                    if (nExecIndex >= 0)
                        strSQL = strSQL.Substring(nExecIndex + 4).Trim();

                    string strMethod = "", strParam = "";

                    if (!SplitStoreProcedureQuery(strSQL, ref strMethod, ref strParam))
                        return;

                    strSQL = "CALL " + strMethod + "(" + strParam + ")";
                }
            }
            else if (this.DatabaseType == DBType.sqlserver)
            {
                if (nCallIndex >= 0)
                {
                    string strSQLQuery = strSQL.Substring(nCallIndex + 4).Trim();

                    int nIndex1 = strSQLQuery.IndexOf('(');
                    int nIndex2 = strSQLQuery.LastIndexOf(')');

                    if (nIndex1 >= 0 && nIndex2 > nIndex1)
                    {
                        string strMethod = strSQLQuery.Substring(0, nIndex1).Trim();
                        string strParam = strSQLQuery.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                        strSQL = strMethod + " " + strParam;
                    }
                    else
                    {
                        string strMethod = "", strParam = "";

                        if (!SplitStoreProcedureQuery(strSQLQuery, ref strMethod, ref strParam))
                            return;
                        else
                            strSQL = strMethod + " " + strParam;
                    }
                }
                else
                {
                    strSQL = strSQL.Trim();
                }
            }
        }

        private string GetStoredProcedure(string strSQLQuery, int nTransaction, string szDBName)
        {            
            string resResult = string.Empty;
			string sourceUrl = m_strWebServerURL + "/" + m_szProcPageName;

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "SQLQuery=" + strUrlEncode + "&" + "Transaction=" + nTransaction;
            postData += "&" + "DatabaseName=" + szDBName + "&" + "DatabaseType=" + DatabaseTypeName;//m_szDatabaseType;

            if (m_szDBHost != null)
                postData += "&" + "Host=" + m_szDBHost;
            if (m_szDBPort != null)
                postData += "&" + "Port=" + m_szDBPort;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.CookieContainer = cookieContainer;
            wReq.Method = "POST";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            wReq.CookieContainer = cookieContainer;

			try
			{
				using (Stream writeStream = wReq.GetRequestStream())
				{
					writeStream.Write(bytes, 0, bytes.Length);
				}

				HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();
				Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, m_PageEncoding); 
				
                resResult = readerPost.ReadToEnd();

                wReq.Abort();
                readerPost.Close();
                respPostStream.Close();
			}
			catch (System.Exception)
			{
				return "";
			}  
            return resResult;
        }

        public ArrayList GetStoredProcedureData(string strSQLQuery, int nTransaction, string szDBName = null)
        {
            ReshapeStoredProcedureQuery(ref strSQLQuery);

            // str에 '\n', '\r'이 포함되어 있으면 다른 문자로 바꾼다.
            strSQLQuery = strSQLQuery.Replace('\n', (char)6);
            strSQLQuery = strSQLQuery.Replace('\r', (char)7);

            if (this.DatabaseType == DBType.mysql)
                strSQLQuery = strSQLQuery.Replace("\\", "\\\\");

			if (szDBName == null)
				szDBName = m_szDatabaseName;

			string resResult = "";

            if( nTransaction == 1 )
            {
                if (m_bBatch == false)
                {
                    throw new WebDBTransactionStateException("트랜잭션이 시작되지 않았습니다.");
                }
            }

            if (m_bBatch == false)
            {
                resResult = GetStoredProcedure(strSQLQuery, nTransaction, szDBName);
            }
            else
            {
                resResult = GetBatchStoredProcedure(strSQLQuery, nTransaction, szDBName);
            }

            m_lastResult = resResult;


            DBResultList arrResult = ParseResult(resResult);
            return arrResult;
        }

        public void RunStoredProcedure(string strProcName, ArrayList arrFields, ArrayList arrValues, int transaction, out ArrayList arrResult, string szDBName = null)
        {			
            arrResult = null;
            int nFieldCount = arrFields.Count;
            int nValueCount = arrValues.Count;
            if (nFieldCount != nValueCount) return;

            string strSQL = strProcName;

            for (int i = 0; i < nValueCount; i++)
            {
                if (i == 0)
                    strSQL += " " + (string)arrValues[i];
                else
                    strSQL += "," + (string)arrValues[i];
            }
			arrResult = GetStoredProcedureData(strSQL, transaction, szDBName);
        }


		public string GetBatchStoredProcedure(string strSQLQuery, int nTransaction, string szDBName = null)
		{
			return GetBatchStoredProcedure(strSQLQuery, nTransaction, "Batch", szDBName);
		}

		private string GetBatchStoredProcedure(string strSQLQuery, int nTransaction, string szCmd, string szDBName = null)
		{
            if (m_bBatch == false)
            {
                throw new WebDBTransactionStateException("트랜잭션이 시작되지 않았습니다.");
            }

			if (szDBName == null)
				szDBName = m_szDatabaseName;

			string resResult = string.Empty;
			string sourceUrl = m_strWebServerURL + "/" + m_szProcPageName;

			UTF8Encoding enc = new UTF8Encoding();
			byte[] bytes1 = enc.GetBytes(strSQLQuery);
			string strUrlEncode = URLEncoding(bytes1);

			string postData = "SQLQuery=" + strUrlEncode + "&" + "Cmd=" + szCmd + "&" + "Proc=true";
			postData += "&" + "DatabaseName=" + szDBName;
			
			UTF8Encoding encoding = new UTF8Encoding();
			byte[] bytes = encoding.GetBytes(postData);
            			

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.CookieContainer = cookieContainer;
            wReq.Method = "POST";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            wReq.Timeout = 20000;
            wReq.CookieContainer = m_batchCookie;

            try
            {
                using (Stream writeStream = wReq.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, m_PageEncoding);

                resResult = readerPost.ReadToEnd();

                wReq.Abort();
                readerPost.Close();
                respPostStream.Close();
            }
            catch (System.Exception)
            {
                return "";
            } 
			return resResult;
		}
    }

   
}
