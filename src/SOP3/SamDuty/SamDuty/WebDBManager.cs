using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SamDuty
{
    public class WebDBManager
    {
        protected StringFile m_StringFile = new StringFile();
        private Utility m_ini = new Utility();
        private string m_strWebServerURL = "";

        private int m_nLevel = -1;

        private static bool m_isLoadSMSAddText = false;
        private static string m_strSmsAddText = "";
        private static string m_strSmsCaller = "";

        public WebDBManager()
        {

            Loadini_ServerConnectionInfo();
            m_strSmsCaller = LoadIni("sms_caller");
        }
        
        // strPhoneNumber에 빈칸이나 '-'등이 들어있을 경우 없앤다. 
        public static string ValidPhoneNumber(string strPhoneNumber, out bool isValid)
        {
            isValid = true;

            string strResult = "";
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch != ' ' && ch != '\t' && ch != '-')
                {
                    if (ch >= '0' && ch <= '9')
                        strResult += ch;
                    else
                    {
                        isValid = false;
                        return "";
                    }
                }
            }

            return strResult;
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
            float result;

            try
            {
                result = float.Parse(dataSrc);
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

        static public DateTime GetDateTimeField(object dataSrc, DateTime dtDefault)
        {
            DateTime result;

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
            if (dataSrc == null || dataSrc == "null")
            {
                return result;
            }
            try
            {
                result = int.Parse(dataSrc);
            }
            catch (Exception)
            {
                result = nDefault;
            }

            return result;
        }


        public string GetReadDB(string strSQLQuery, int nTransaction)
        {
            string resResult = string.Empty;
            //string m_sourceUrl = "http://localhost:8088/SOP/Login.jsp";
            string sourceUrl = m_strWebServerURL + "/DBQuery2.jsp";

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            //string strBase64 = Convert.ToBase64String(bytes1);
            //string strUrlEncode = System.Web.HttpUtility.UrlEncode(bytes1);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "SQLQuery=" + strUrlEncode + "&" + "Transaction=" + nTransaction;
            //string postData = "SQLQuery=" + strSQLQuery + "&" + "Transaction=" + nTransaction;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);

            lock (this)
            {
                wReq.CookieContainer = m_CookieContainer;
                wReq.Method = "POST";
                //wReq.UserAgent = "Mozilla/4.0";
                wReq.ContentType = "application/x-www-form-urlencoded";
                wReq.ContentLength = bytes.Length;
                //wReq.CookieContainer = new CookieContainer();

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

                    readerPost.Close();

                    respPostStream.Close();
                }
                catch (System.Net.WebException e)
                {
                    System.Windows.Forms.MessageBox.Show(e.Message);
                    return "";
                }
            }

            return resResult;
        }

        public ArrayList GetResultData(string strSQLQuery, int nTransaction)
        {
            // str에 '\n', '\r'이 포함되어 있으면 다른 문자로 바꾼다.
            strSQLQuery = strSQLQuery.Replace('\n', (char)6);
            strSQLQuery = strSQLQuery.Replace('\r', (char)7);

            ArrayList arrResult = new ArrayList();
            string resResult = GetReadDB(strSQLQuery, nTransaction);

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
            //string sourceUrl = "http://localhost:8088/SOP/RunStoredProcedure.jsp";
            string sourceUrl = m_strWebServerURL + "/RunStoredProcedure2.jsp";
            //string postData = "SQLQuery=" + strSQLQuery + "&" + "Transaction=" + nTransaction;

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "SQLQuery=" + strUrlEncode +"&" + "Transaction=" + nTransaction;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.CookieContainer = m_CookieContainer;
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
            string resResult = GetStoredProcedure(strSQLQuery, nTransaction);

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

        public string LoadIni(string strTargetName, string strSectionName)
        {
            return m_ini.getinivalue(strSectionName, strTargetName);
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
        private CookieContainer m_CookieContainer = new CookieContainer();
		public string SendSMS(string strPhoneNumber, string strSendPhoneNumber, string strMsg)
		{

			ArrayList arrMessages = MakeMessageList(strMsg);
			foreach (string szMsg in arrMessages)
			{

				string resResult = string.Empty;
				string sourceUrl = m_strWebServerURL + "/SendSMS.jsp";

				Encoding enc = Encoding.GetEncoding(51949);
				byte[] bytes1 = enc.GetBytes(szMsg);
				string strUrlEncode = URLEncoding(bytes1);

				// 테스트 : %c5%d7%bd%ba%c6%ae%0d%0a - ok
				// 테스트 : %c5%d7%bd%ba%c6%ae
				string postData = "Sender=" + strSendPhoneNumber + "&" + "Reciver=" + strPhoneNumber + "&" + "Msg=" + strUrlEncode;

				sourceUrl = sourceUrl + "?" + postData;
				HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);

				lock (this)
				{
					wReq.Method = "GET";
					wReq.CookieContainer = m_CookieContainer;

					try
					{

						HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

						// http 내용 추출
						Stream respPostStream = wRes.GetResponseStream();
						StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

						resResult = readerPost.ReadToEnd();

						readerPost.Close();

						respPostStream.Close();
					}
					catch (System.Net.WebException e)
					{
						System.Windows.Forms.MessageBox.Show(e.Message);
						return "";
					}
				}
			}

			return "OK";
		}


		/*
		public string SendSMS(string strPhoneNumber, string strSendPhoneNumber, string strMsg)
		{

			ArrayList arrMessages = MakeMessageList(strMsg);
			foreach (string szMsg in arrMessages)
			{
				string resResult = string.Empty;
				string sourceUrl = m_strWebServerURL + "/SendSMS.jsp";

				//Encoding enc = Encoding.GetEncoding(51949);
				//byte[] bytes1 = enc.GetBytes(szMsg);
				//string strUrlEncode = URLEncoding(bytes1);

				// 테스트 : %c5%d7%bd%ba%c6%ae%0d%0a - ok
				// 테스트 : %c5%d7%bd%ba%c6%ae
				string postData = "Sender=" + strSendPhoneNumber + "&" + "Reciver=" + strPhoneNumber + "&" + "Msg=" + szMsg;

				//sourceUrl = sourceUrl + "?" + postData;
				HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);

				UTF8Encoding encoding = new UTF8Encoding();
				byte[] bytes = encoding.GetBytes(postData);

				lock (this)
				{
					try
					{
						wReq.CookieContainer = m_CookieContainer;
						wReq.Method = "POST";

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

						readerPost.Close();

						respPostStream.Close();
					}
					catch (System.Net.WebException e)
					{
						//System.Windows.Forms.MessageBox.Show(e.Message);
						return "";
					}
				}

			}

			return "OK";
		}
		*/
        // strMsg를 80바이트씩 자른다.
        private ArrayList MakeMessageList(string strMsg)
        {
            ArrayList arrMessages = new ArrayList();

            int nByteLength = 0;
            int nLen = strMsg.Length;
            int nBeginIndex = 0;

            for (int i = 0; i < nLen; i++)
            {
                if (strMsg.ElementAt(i) < 256)
                    nByteLength++;
                else
                    nByteLength += 2;

                if (nByteLength == 80 ||
                    ((nByteLength == 79) && (i < nLen - 1 && strMsg.ElementAt(i + 1) >= 256)))
                {
                    arrMessages.Add(strMsg.Substring(nBeginIndex, i - nBeginIndex + 1));
                    nBeginIndex = i + 1;
                    nByteLength = 0;
                }
            }

            if (nByteLength > 0)
            {
                arrMessages.Add(strMsg.Substring(nBeginIndex));
            }

            return arrMessages;
        }

        public bool SendSMS(ArrayList arrPhoneNumbers, string strSendPhoneNumber, string strMsg)
        {
            ArrayList arrMessages = MakeMessageList(strMsg);
            foreach (string szPhone in arrPhoneNumbers)
            {
                foreach (string szMsg in arrMessages)
                {
                    SendSMS(szPhone, strSendPhoneNumber, szMsg);
                }
            }
            return true;
        }

        public static string SMSCaller
        {
            get { return m_strSmsCaller; }
        }
    }
}
