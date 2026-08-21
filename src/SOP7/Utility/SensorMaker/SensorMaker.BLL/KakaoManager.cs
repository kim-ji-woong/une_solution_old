using Common.Model.Option;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SensorMaker.BLL
{
    public class KakaoManager
    {
        public enum MessageType { Permit, Deny, Request, Notify, Timeout };

        private static Common.IDAL.IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        private static string m_strToken = "";
        private static string m_strFrontURL = "https://www.biztalk-api.com";

        public KakaoManager(Common.IDAL.IDataManager dataManager, ProcessManager processManager)
        {
            m_dataManager = dataManager;
            m_processManager = processManager;
            
        }

        public static bool SendMessage(MessageType type, string strEmail, string strPhoneNumber, string strSiteURL, string strSolutionName, string strParam = null)
        {
            string strMessage = "";
            string strTmpltCode = "";

            if (type == MessageType.Request)
            {
                strMessage = GetRequestString(strEmail);
                strTmpltCode = "account_request";
            }
            else if (type == MessageType.Permit)
            {
                strMessage = GetPermitString(strEmail);
                strTmpltCode = "account_permit";
            }
            else if (type == MessageType.Deny)
            {
                strMessage = GetDenyString(strEmail, strParam);
                strTmpltCode = "account_deny";
            }

            if (strSolutionName != null && strSolutionName.Length > 0)
                strMessage = string.Format("[{0}] {1}\r\n\r\n{2}", strSolutionName, strMessage, strSiteURL);
            else
                strMessage += "\r\n\r\n" + strSiteURL;

            return Send(strMessage, strPhoneNumber, strTmpltCode);
        }

        private static string GetRequestString(string strEmail)
        {
            return string.Format("[{0}] 계정이 생성되어 관리자의 승인을 기다리는 중입니다.", strEmail);
        }

        private static string GetPermitString(string strEmail)
        {
            return string.Format("[{0}] 계정생성이 승인되었습니다.", strEmail);
        }

        private static string GetDenyString(string strEmail, string strReason)
        {
            if (strReason == null)
                strReason = "";

            if (strReason.Length > 0)
                return string.Format("[{0}] 계정생성이 거절되었습니다.\r\n사유 : {1}", strEmail, strReason);

            return string.Format("[{0}] 계정생성이 거절되었습니다.", strEmail);
        }

        private static bool Send(string strMessage, string strPhoneNumber, string strTmpltCode)
        {
            string strErrorMessage = "";
            KakaoInfo info = m_dataManager.GetSelectManager().SelectKakaoInfo(out strErrorMessage);
            if (info.CountryCode == -1 || info.SenderKey.Length == 0 || info.BsID.Length == 0 || info.BsPasswd.Length == 0)
                return false;

            GetToken(info.BsID, info.BsPasswd);
            if (m_strToken.Length == 0)
                return false;

            string url = "/v2/kko/sendAlimTalk";

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders["bt-token"] = m_strToken;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_strFrontURL + url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 5000;

            foreach (KeyValuePair<string, string> pair in dicHeaders)
            {
                request.Headers.Add(pair.Key, pair.Value);
            }

            JsonManager mgr = new JsonManager();
            mgr.Add("msgIdx", "1");
            mgr.Add("countryCode", info.CountryCode);
            mgr.Add("recipient", strPhoneNumber);
            mgr.Add("senderKey", info.SenderKey);
            mgr.Add("message", strMessage);//"SOP 시스템 화재 알람 탐지\n2020-11-12 00:00:01\n[백화점 1층]에서 화재 신호가 탐지되었습니다.");
            mgr.Add("tmpltCode", strTmpltCode);
            mgr.Add("title", "");
            mgr.Add("resMethod", "PUSH");

            // POST할 데이타를 Request Stream에 쓴다
            byte[] bytes = Encoding.UTF8.GetBytes(mgr.Json);
            request.ContentLength = bytes.Length; // 바이트수 지정
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                System.Diagnostics.Trace.WriteLine("Response : " + responseText);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Kakao Error : " + ex.Message);
                return false;
            }
        }

        private static void GetToken(string bsID, string passwd)
        {
            string strUrl = "/v2/auth/getToken";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_strFrontURL + strUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 5000;

            JsonManager mgr = new JsonManager();

            mgr.Add("bsid", bsID);
            mgr.Add("passwd", passwd);

            // POST할 데이타를 Request Stream에 쓴다
            byte[] bytes = Encoding.UTF8.GetBytes(mgr.Json);
            request.ContentLength = bytes.Length; // 바이트수 지정
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {

                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }
                //"{\"responseCode\":\"1000\",\"token\":\"eyJhbGciOiJIUzI1NiJ9.eyJic2lkIjoidW5lOTk2NiIsImV4cCI6MTYwNTE3MDcxMywiaWF0IjoxNjA1MDg0MzEzLCJpcEFkZHIiOiIyMTguMTUyLjIwMC4xMjMifQ.w4VBw7_MXcL5wYNGIMKXy0dPXhi-Qquig0o6N_aSh-I\"}"
                System.Diagnostics.Trace.WriteLine("Response : " + responseText);

                int index = responseText.IndexOf("\"token\":\"");
                string temp = responseText.Substring(index);
                m_strToken = temp.Replace("\"token\":\"", "").Replace("\"}", "");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private class JsonManager
        {
            private string m_strValues = "";

            public string Json
            {
                get
                {
                    string strJson = "{ " + m_strValues + " }";
                    return strJson;
                }
            }

            public void Add(string strName, string strValue)
            {
                string strLine = "\"" + strName + "\": \"" + strValue + "\"";

                if (m_strValues.Length == 0)
                    m_strValues = strLine;
                else
                    m_strValues += ", " + strLine;
            }

            public void Add(string strName, int nValue)
            {
                string strLine = "\"" + strName + "\": " + nValue.ToString();

                if (m_strValues.Length == 0)
                    m_strValues = strLine;
                else
                    m_strValues += ", " + strLine;
            }
        }
    }
}
