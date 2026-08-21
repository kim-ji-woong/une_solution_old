using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Vacation.IDAL;
using Vacation.Model;

namespace Vacation.BLL
{
    public class KakaoManager
    {
        private static IDataManager m_dataManager = null;
        private static KakaoManager m_instance = null;
        public enum MessageType { Permit, Deny, Request, Notify, Timeout };

        private KakaoManager(IDataManager dataManager)
        {
            SetDataManager(dataManager);
        }

        public static void InitInstance(IDataManager dataManager)
        {
            if (m_instance == null)
                m_instance = new KakaoManager(dataManager);
            else
                m_instance.SetDataManager(dataManager);
        }

        public void SetDataManager(IDataManager dataManager)
        {
            m_dataManager = dataManager;
        }

        public static bool SendMessage(MessageType type, string strPhoneNumber, DateTime timeStamp, float fDays, string strPeriod, string strName = null)
        {
            string strMessage = "";
            string strTmpltCode = "";
            string strDays = GetFloatString(fDays); 

            if (type == MessageType.Permit)
            {
                strMessage = GetPermitString(timeStamp, strDays, strPeriod);
                strTmpltCode = "holiday_permit";
            }
            else if (type == MessageType.Deny)
            {
                strMessage = GetDenyString(timeStamp, strDays, strPeriod, strName);
                strTmpltCode = "holiday_deny";
            }
            else if (type == MessageType.Request)
            {
                strMessage = GetRequestString(strDays, strPeriod, strName);
                strTmpltCode = "holiday_request";
            }
            else if (type == MessageType.Timeout)
            {
                strMessage = GetTimeoutString(timeStamp, strDays, strPeriod);
                strTmpltCode = "holiday_timeout";
            }
            
            strMessage += "\r\n\r\n" + ScheduleManager.SiteURL;
            Send(strMessage, strPhoneNumber, strTmpltCode);

            return true;
        }

        public static bool SendSVMessage(MessageType type, string strPhoneNumber, DateTime timeStamp, float fDays, string strName = null, string strTargetNames = null, string strReason = null)
        {
            string strMessage = "";
            string strTmpltCode = "";
            string strDays = GetFloatString(fDays);

            if (type == MessageType.Permit)
            {
                strMessage = GetSVPermitString(timeStamp, strDays);
                strTmpltCode = "sv_holiday_permit";
            }
            else if (type == MessageType.Deny)
            {
                strMessage = GetSVDenyString(timeStamp, strDays, strName);
                strTmpltCode = "sv_holiday_deny";
            }
            else if (type == MessageType.Request)
            {
                strMessage = GetSVRequestString(strDays, strName, strTargetNames);
                strTmpltCode = "sv_holiday_request";
            }
            else if (type == MessageType.Notify)
            {
                strMessage = GetSVNotifyString(strName, strDays, strReason);
                strTmpltCode = "sv_holiday_notify";
            }
            else if (type == MessageType.Timeout)
            {
                strMessage = GetSVTimeoutString(timeStamp, strDays);
                strTmpltCode = "sv_holiday_timeout";
            }

            strMessage += "\r\n\r\n" + ScheduleManager.SiteURL;
            Send(strMessage, strPhoneNumber, strTmpltCode);

            return true;
        }

        private static string GetSVRequestString(string strDays, string strName, string strTargetNames)
        {
            string strMessage = "[특별휴가 승인 요청]\r\n";
            string strAdd = "으로부터";

            if (strName.EndsWith("리") ||
                strName.EndsWith("사") ||
                strName.EndsWith("표"))
            {
                strAdd = "로부터";
            }

            strMessage += string.Format("{0}{1} {2}일간의 특별휴가 승인 요청이 왔습니다.\r\n", strName, strAdd, strDays);
            strMessage += string.Format("대상자 : {0}", strTargetNames);

            return strMessage;
        }

        private static string GetSVDenyString(DateTime timeStamp, string strDays, string strName)
        {
            string strTime = string.Format("{0}년 {1}월 {2}일 {3}시 {4}분", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute);

            string strMessage = "[특별휴가 승인 거절]\r\n";
            strMessage += string.Format("{0}에 요청하였던 {1}일간의 특별휴가 요청을 {2}님께서 거절하였습니다.", strTime, strDays, strName);
            return strMessage;
        }

        private static string GetSVTimeoutString(DateTime timeStamp, string strDays)
        {
            string strTime = string.Format("{0}년 {1}월 {2}일 {3}시 {4}분", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute);

            string strMessage = "[특별휴가 요청 취소]\r\n";
            strMessage += string.Format("{0}에 요청하였던 {1}일간의 특별휴가 요청에 대한 결재처리가 이루어지지 않아 취소되었습니다.", strTime, strDays);

            return strMessage;
        }

        private static string GetSVPermitString(DateTime timeStamp, string strDays)
        {
            string strTime = string.Format("{0}년 {1}월 {2}일 {3}시 {4}분", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute);

            string strMessage = "[특별휴가 승인 완료]\r\n";
            strMessage += string.Format("{0}에 요청하였던 {1}일간의 특별휴가가 승인되었습니다.", strTime, strDays);
            return strMessage;
        }

        private static string GetSVNotifyString(string strName, string strDays, string strReason)
        {
            string strMessage = "[특별휴가 알림]\r\n";
            strMessage += string.Format("{0}님께 {1}일간의 특별휴가가 발생되었습니다.\r\n", strName, strDays);
            strMessage += string.Format("사유 : {0}", strReason);
            return strMessage;
        }

        private static string GetRequestString(string strDays, string strPeriod, string strName)
        { 
            string strMessage = "[휴가 승인 요청]\r\n";
            string strAdd = "으로부터";

            if (strName.EndsWith("리") ||
                strName.EndsWith("사") ||
                strName.EndsWith("표"))
            {
                strAdd = "로부터";
            }

            if (strPeriod.Contains("~"))
                strMessage += string.Format("{0}{3} {1}일간의({2}) 휴가 승인 요청이 왔습니다.", strName, strDays, strPeriod, strAdd);
            else
                strMessage += string.Format("{0}{2} {1} 휴가 승인 요청이 왔습니다.", strName, strPeriod, strAdd);

            return strMessage;
        }

        private static string GetDenyString(DateTime timeStamp, string strDays, string strPeriod, string strName)
        {
            string strTime = string.Format("{0}년 {1}월 {2}일 {3}시 {4}분", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute);

            string strMessage = "[휴가 승인 거절]\r\n";

            if (strPeriod.Contains("~"))
                strMessage += string.Format("{0}에 요청하였던 {1}일간의({2}) 휴가 요청을 {3}님께서 거절하였습니다.", strTime, strDays, strPeriod, strName);
            else
                strMessage += string.Format("{0}에 요청하였던 {1} 휴가 요청을 {2}님께서 거절하였습니다.", strTime, strPeriod, strName);

            return strMessage;
        }

        private static string GetPermitString(DateTime timeStamp, string strDays, string strPeriod)
        {
            string strTime = string.Format("{0}년 {1}월 {2}일 {3}시 {4}분", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute);

            string strMessage = "[휴가 승인 완료]\r\n";

            if (strPeriod.Contains("~"))
                strMessage += string.Format("{0}에 요청하였던 {1}일간의({2}) 휴가가 승인되었습니다.", strTime, strDays, strPeriod);
            else
                strMessage += string.Format("{0}에 요청하였던 {1} 휴가가 승인되었습니다.", strTime, strPeriod);

            return strMessage;
        }

        private static string GetTimeoutString(DateTime timeStamp, string strDays, string strPeriod)
        {
            string strTime = string.Format("{0}년 {1}월 {2}일 {3}시 {4}분", timeStamp.Year, timeStamp.Month, timeStamp.Day, timeStamp.Hour, timeStamp.Minute);

            string strMessage = "[휴가 요청 취소]\r\n";

            if (strPeriod.Contains("~"))
                strMessage += string.Format("{0}에 요청하였던 {1}일간의({2}) 휴가에 대한 결재처리가 이루어지지 않아 취소되었습니다.", strTime, strDays, strPeriod);
            else
                strMessage += string.Format("{0}에 요청하였던 {1} 휴가에 대한 결재처리가 이루어지지 않아 취소되었습니다.", strTime, strPeriod);

            return strMessage;
        }

        private static string GetFloatString(float data)
        {
            return string.Format("{0:F1}", data);
        }

        private static string m_strToken = "";
        private static string m_strFrontURL = "https://www.biztalk-api.com";

        private static void Send(string strMessage, string strPhoneNumber, string strTmpltCode)
        {
            string strErrorMessage = "";
            OptionKakaoInfo info = m_dataManager.GetSelectManager().SelectOptionKakaoInfo(out strErrorMessage);
            if (info.CountryCode == -1 || info.SenderKey.Length == 0 || info.BsID.Length == 0 || info.BsPasswd.Length == 0)       
                return;

            GetToken(info.BsID, info.BsPasswd);
            if (m_strToken.Length == 0)
            {
                return;
            }

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

            AlimTalkParam param = new AlimTalkParam();
            param.msgIdx = "1";
            param.countryCode = info.CountryCode.ToString();
            param.resMethod = "PUSH";
            param.senderKey = info.SenderKey;
            param.tmpltCode = strTmpltCode;
            param.message = strMessage;
            param.recipient = strPhoneNumber;
            param.title = "";

            string strParams = Newtonsoft.Json.JsonConvert.SerializeObject(param, Newtonsoft.Json.Formatting.None);
            strParams = strParams.Replace("\\\\r", "\\r").Replace("\\\\n", "\\n");

            // POST할 데이타를 Request Stream에 쓴다
            byte[] bytes = Encoding.UTF8.GetBytes(strParams);
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("Kakao Error : " + ex.Message);
            }
        }

        private static void GetToken(string bsID, string passwd)
        {
            string strUrl = "/v2/auth/getToken";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_strFrontURL + strUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 5000;

            Token token = new Token();
            token.bsid = bsID;
            token.passwd = passwd;

            string strToken = Newtonsoft.Json.JsonConvert.SerializeObject(token);

            // POST할 데이타를 Request Stream에 쓴다
            byte[] bytes = Encoding.UTF8.GetBytes(strToken);
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

                //int index = responseText.IndexOf("\"token\":\"");
                //string temp = responseText.Substring(index);
                //temp = temp.Replace("\"token\":\"", "").Replace("\"}", "");
                //int index2 = temp.IndexOf(",\"expireDate\":\"");
                //m_strToken = temp.Remove(index2);

                TokenResponse tokenRes = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(responseText);
                if (tokenRes != null)
                {
                    if (tokenRes.responseCode == "1000")
                    {
                        m_strToken = tokenRes.token;
                    }
                    else
                    {
                        throw new ApplicationException("[Kakao GetToken Error] ResultCode : " + tokenRes.responseCode + ", msg : " + tokenRes.msg);
                    }
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private class AlimTalkParam
        {
            public string msgIdx { get; set; }
            public string countryCode { get; set; }
            public string resMethod { get; set; }
            public string senderKey { get; set; }
            public string tmpltCode { get; set; }
            public string message { get; set; }
            public string recipient { get; set; }
            public string title { get; set; }

        }
        private class TokenResponse
        {
            public string responseCode { get; set; }
            public string token { get; set; }
            public string msg { get; set; }
            public string expireDate { get; set; }
        }

        private class Token
        {
            public string bsid { get; set; }
            public string passwd { get; set; }
        }
    }
}
