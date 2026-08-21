using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SOPManager
{
    public class WebDBManager
    {
        protected StringFile m_StringFile = new StringFile();
        private FormMain m_Main = null;
        private Utility m_ini = new Utility();
        private string m_strWebServerURL = "";

        private int m_nLevel = -1;

        public WebDBManager(FormMain main)
        {
            m_Main = main;
            Loadini_ServerConnectionInfo();
        }

        // User 권한
        public int Level
        {
            get { return m_nLevel; }
            set { m_nLevel = value; }
        }

        

        // 문자열 앞뒤의 빈문자들을 제거한다.
        public string GetStringField(object dataSrc, string strDefault)
        {
            string result;

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
		public T GetField<T>(object dataSrc, T dataDefault)
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

		public float GetFloatField(string dataSrc, float fDefault)
		{
			float result = fDefault;
			if (dataSrc == null || dataSrc.ToString() == "null")
				return result;
			float.TryParse(dataSrc, out result);
			return result;			
		}

        public DateTime GetDateTimeField(object dataSrc, DateTime dtDefault)
        {
			DateTime result = dtDefault;
			if (dataSrc == null || dataSrc.ToString() == "null")
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

        public int GetIntField(string dataSrc, int nDefault)
        {
			int result = nDefault;
			if (dataSrc == null || dataSrc.ToString() == "null")
				return result;
			int.TryParse(dataSrc, out result);
            return result;
        }

        public string GetReadDB(string strSQLQuery, int nTransaction)
        {
            string resResult = string.Empty;
            string sourceUrl = m_strWebServerURL + "/DBQuery2.jsp";

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);
            string postData = "SQLQuery=" + strUrlEncode + "&" + "Transaction=" + nTransaction;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);
            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.CookieContainer = m_cookieContainer;
            wReq.Method = "POST";
            //wReq.UserAgent = "Mozilla/4.0";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            try
            {
                using (Stream writeStream = wReq.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

                // http 내용 추출
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

                resResult = readerPost.ReadToEnd();
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show(e.Message);
                return "";
            }

            return resResult;
        }

        public ArrayList GetResultData(string strSQLQuery, int nTransaction)
        {
            // str에 '\n', '\r'이 포함되어 있으면 다른 문자로 바꾼다.
            strSQLQuery = strSQLQuery.Replace('\n', (char)6);
            strSQLQuery = strSQLQuery.Replace('\r', (char)7);

            ArrayList arrResult = new ArrayList();

            string resResult = "";
            if (bBatchProcess == true)
                resResult = GetBatchReadDB(strSQLQuery, nTransaction);
            else
                resResult = GetReadDB(strSQLQuery, nTransaction);

            m_StringFile.SetData(resResult);

            string strResult = "";
            bool isResult = true;
            bool isBegin = false;

            while (isResult)
            {
                isResult = m_StringFile.ReadLine(ref strResult);

                if (isResult)
                {
                    if (strResult == "Begin Data")
                    {
                        isBegin = true;
                        continue;
                    }

                    if (strResult == "End Data")
                        break;

                    if (isBegin)
                    {
                        if (strResult == "null_SQLError")
                        {
                            return null;
                        }
                        else
                            arrResult.Add(strResult);
                    }
                }
            }

            return arrResult;
        }

        //////////////////////////////////////////////////////////////////////////
        // StoredProcedure
        public string GetStoredProcedure(string strSQLQuery, int nTransaction)
        {
            string resResult = string.Empty;
            string sourceUrl = m_strWebServerURL + "/RunStoredProcedure2.jsp";

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "SQLQuery=" + strUrlEncode + "&" + "Transaction=" + nTransaction;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.CookieContainer = m_cookieContainer;
            wReq.Method = "POST";
            //wReq.UserAgent = "Mozilla/4.0";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            using (Stream writeStream = wReq.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

            // http 내용 추출
            Stream respPostStream = wRes.GetResponseStream();
            StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

            resResult = readerPost.ReadToEnd();

            return resResult;
        }

        public ArrayList GetStoredProcedureData(string strSQLQuery, int nTransaction)
        {
            ArrayList arrResult = new ArrayList();

            string resResult = "";
            if (bBatchProcess == true)
                resResult = GetBatchStoredProcedure(strSQLQuery, nTransaction);
            else
                resResult = GetStoredProcedure(strSQLQuery, nTransaction);

            m_StringFile.SetData(resResult);

            string strResult = "";
            bool isResult = true;
            bool isBegin = false;

            while (isResult)
            {
                isResult = m_StringFile.ReadLine(ref strResult);

                if (isResult)
                {
                    if (strResult == "Begin Data")
                    {
                        isBegin = true;
                        continue;
                    }
                    if (strResult == "End Data")
                        break;

                    if (isBegin)
                        arrResult.Add(strResult);
                }
            }

            return arrResult;
        }

        public void RunStoredProcedure(string strProcName, ArrayList arrFields, ArrayList arrValues, int transaction, out ArrayList arrResult)
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

            arrResult = GetStoredProcedureData(strSQL, transaction);
        }

        // 해당문자열을 ``으로 감싸서 반환한다 (strQuary:DB이름이나 필드명)
        public string Grave(object obj)
        {
            return "`" + obj.ToString() + "`";
        }

        public void Loadini_ServerConnectionInfo()
        {
            string strSection = "Server Connection Info";

            m_strWebServerURL = m_ini.getinivalue(strSection, "webserver_url");
        }

        public string LoadIni(string strTargetName)
        {
            string strSection = "Server Connection Info";
            return m_ini.getinivalue(strSection, strTargetName);
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


        private bool bBatchProcess = false;
        private CookieContainer m_cookieContainer = new CookieContainer();

        public void BeginBatch()
        {
            bBatchProcess = true;
            GetBatchReadDB("", 1, "Rollback");
        }

        public void EndBatch(bool bCommit)
        {
            bBatchProcess = false;
            if (bCommit == false)
            {
                GetBatchReadDB("", 1, "Rollback");
            }
            else
            {
                GetBatchReadDB("", 1, "Commit");
            }
        }
                
        public string GetBatchReadDB(string strSQLQuery, int nTransaction)
        {
            return GetBatchReadDB(strSQLQuery, nTransaction, "Batch");
        }

        public string GetBatchReadDB(string strSQLQuery, int nTransaction, string szCmd)
        {
            string resResult = string.Empty;
            string sourceUrl = m_strWebServerURL + "/BatchQuery.jsp";

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);
            string postData = "SQLQuery=" + strUrlEncode + "&" + "Cmd=" + szCmd;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.CookieContainer = m_cookieContainer;
            wReq.Method = "POST";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            wReq.Timeout = 20000;
            try
            {
                using (Stream writeStream = wReq.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);
                resResult = readerPost.ReadToEnd();
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show(e.Message);
                return "";
            }
            return resResult;
        }

        public string GetBatchStoredProcedure(string strSQLQuery, int nTransaction)
        {
            return GetBatchStoredProcedure(strSQLQuery, nTransaction, "Batch");
        }

        public string GetBatchStoredProcedure(string strSQLQuery, int nTransaction, string szCmd)
        {
            string resResult = string.Empty;
            string sourceUrl = m_strWebServerURL + "/RunStoredProcedure2.jsp";

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "SQLQuery=" + strUrlEncode + "&" + "Cmd=" + szCmd + "&" + "Proc=true";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.CookieContainer = m_cookieContainer;
            wReq.Method = "POST";
            //wReq.UserAgent = "Mozilla/4.0";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            wReq.Timeout = 20000;

            using (Stream writeStream = wReq.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }
            HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();
            // http 내용 추출
            Stream respPostStream = wRes.GetResponseStream();
            StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

            resResult = readerPost.ReadToEnd();

            return resResult;
        }
    }
}
