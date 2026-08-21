using Common.Model.Option;
using dnsDBUtil;
using Newtonsoft.Json.Linq;
using SDMS.Model.History;
using SDMS.Model.Sensor;
using SDMS.Model.Spatial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace dnsSMS
{
    internal class BaseMessageBroker
    {
        protected string m_strErrorMessage = "";

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        // strMessage에 작은 따옴표가 들어있는지 검사한다.
        protected string CheckQuotation(string strMessage)
        {
            return strMessage.Replace("'", "''");
        }

        protected string GetCurrentTime(WebDBManager dbMgr, string strDBName)
        {
            string strTime = "";
            if (dbMgr.DatabaseType == WebDBManager.DBType.sqlserver)
            {
                System.Collections.ArrayList arrResult = dbMgr.GetResultData("Select convert(varchar(19), GetDate(), 120)", strDBName);

                if (arrResult == null || arrResult.Count < 1)
                    return "";

                strTime = WebDBManager.GetStringField(arrResult[0]);
            }
            else if (dbMgr.DatabaseType == WebDBManager.DBType.mysql)
            {
                System.Collections.ArrayList arrResult = dbMgr.GetResultData("SELECT current_date(), current_time()", strDBName);

                if (arrResult == null || arrResult.Count < 2)
                    return "";

                string strDate = WebDBManager.GetStringField(arrResult[0]);
                strTime = strDate + WebDBManager.GetStringField(arrResult[1]);
            }
            else
            {
                DateTime dtNow = DateTime.Now;
                strTime = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                return strTime;
            }

            strTime = strTime.Replace("-", "");
            strTime = strTime.Replace(":", "");
            strTime = strTime.Replace(" ", "");

            int nIndex = strTime.LastIndexOf('.');

            if (nIndex >= 0)
                strTime = strTime.Substring(0, nIndex);

            return strTime;
        }
    }

#if UNE_MCS
    // KT 메시지 서비스 제공업체(모노커뮤니케이션즈)의 라이브러리 직접 사용하는 버전
    internal class MessageBrokerMCS : BaseMessageBroker
    {
        private string DBName = "UNE_SMS";
        private WebDBManager.DBType DBType = WebDBManager.DBType.sqlserver;
        private WebDBManager m_dbMgr = null;
        // 모노커뮤니케이션즈의 MCS 서비스는 사전에 등록된 전화번호만 발신번호로 사용할 수 있음
        private const string m_strCaller = "027144133";
        private const string m_strUserID = "une9966";

        public MessageBrokerMCS()
        {
            m_dbMgr = new WebDBManager();
            m_dbMgr.WebServerURL = "http://192.168.0.10:808";
            m_dbMgr.DatabaseType = DBType;
            m_dbMgr.DatabaseName = DBName;
        }

        public bool SendSMSMessage(List<string> phoneNumberList, string strMessage, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_SMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, SMS_MSG, CALLBACK_URL, NOW_DATE, SEND_DATE, CALLBACK, DEST_TYPE, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, STD_ID) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', NULL, NULL, '{2}', '{3}', 0, {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[dnsSMS]MessageBrokerMCS.SendSMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        public bool SendLMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, MMS_MSG, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, MSG_TYPE, STD_ID, CONTENT_COUNT, CONTENT_DATA) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', '{2}', '{2}', '{3}', {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL, 0, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[dnsSMS]MessageBrokerMCS.SendLMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        public bool SendMMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strContents = "";
            int nContentsCount = 0;

            foreach (KeyValuePair<MessageContentMMS.ContentType, string> content in contentDatas)
            {
                if (content.Key == MessageContentMMS.ContentType.Image)
                {
                    if (content.Value.Length > 0)
                    {
                        if (strContents.Length == 0)
                            strContents = content.Value + "^1^0";
                        else
                            strContents += "|" + content.Value + "^1^0";

                        nContentsCount++;
                    }
                }
            }

            if (strContents.Length == 0)
                strContents = "NULL";
            else
                strContents = "'" + strContents + "'";

            string strSQL = "Insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, MMS_MSG, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, MSG_TYPE, STD_ID, CONTENT_COUNT, CONTENT_DATA) ";
            strSQL += string.Format("values ('{0}', 0, '{1}', '{2}', '{3}', '{3}', '{4}', {5}, '{6}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL, {7}, {8})",
                m_strUserID, strTitle, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo, nContentsCount, strContents);

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[dnsSMS]MessageBrokerMCS.SendMMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        private string GetReceiverInfo(List<string> phoneNumberList, int nBeginIndex, int nEndIndex)
        {
            string strReceivers = "";

            for (int i=nBeginIndex;i<nEndIndex;i++)
            {
                if (strReceivers.Length == 0)
                    strReceivers = "a^" + phoneNumberList[i];
                else
                    strReceivers += "|a^" + phoneNumberList[i];
            }

            return strReceivers;
        }
    }
#endif

#if SKT_MCS
    // KT 메시지 서비스 제공업체(모노커뮤니케이션즈)의 라이브러리 직접 사용하는 버전
    internal class MessageBrokerSKT_MCS : BaseMessageBroker
    {
        private string DBName = "UNE_SMS";
        private WebDBManager.DBType DBType = WebDBManager.DBType.sqlserver;
        private WebDBManager m_dbMgr = null;
        // 모노커뮤니케이션즈의 MCS 서비스는 사전에 등록된 전화번호만 발신번호로 사용할 수 있음
        private const string m_strCaller = "027144133";
        private const string m_strUserID = "une4133";

        public MessageBrokerSKT_MCS()
        {
            m_dbMgr = new WebDBManager();
            m_dbMgr.WebServerURL = "http://175.106.95.65";
            m_dbMgr.DatabaseType = DBType;
        }

        public bool SendSMSMessage(List<string> phoneNumberList, string strMessage, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_SMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, SMS_MSG, CALLBACK_URL, NOW_DATE, SEND_DATE, CALLBACK, DEST_TYPE, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, STD_ID) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', NULL, NULL, '{2}', '{3}', 0, {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[dnsSMS]MessageBrokerMCS.SendSMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        public bool SendLMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, MMS_MSG, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, MSG_TYPE, STD_ID, CONTENT_COUNT, CONTENT_DATA) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', '{2}', '{2}', '{3}', {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL, 0, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[dnsSMS]MessageBrokerMCS.SendLMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        public bool SendMMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strContents = "";
            int nContentsCount = 0;

            foreach (KeyValuePair<MessageContentMMS.ContentType, string> content in contentDatas)
            {
                if (content.Key == MessageContentMMS.ContentType.Image)
                {
                    if (content.Value.Length > 0)
                    {
                        if (strContents.Length == 0)
                            strContents = content.Value + "^1^0";
                        else
                            strContents += "|" + content.Value + "^1^0";

                        nContentsCount++;
                    }
                }
            }

            if (strContents.Length == 0)
                strContents = "NULL";
            else
                strContents = "'" + strContents + "'";

            string strSQL = "Insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, MMS_MSG, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, MSG_TYPE, STD_ID, CONTENT_COUNT, CONTENT_DATA) ";
            strSQL += string.Format("values ('{0}', 0, '{1}', '{2}', '{3}', '{3}', '{4}', {5}, '{6}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL, {7}, {8})",
                m_strUserID, strTitle, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo, nContentsCount, strContents);

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[dnsSMS]MessageBrokerMCS.SendMMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        private string GetReceiverInfo(List<string> phoneNumberList, int nBeginIndex, int nEndIndex)
        {
            string strReceivers = "";

            for (int i = nBeginIndex; i < nEndIndex; i++)
            {
                if (strReceivers.Length == 0)
                    strReceivers = "a^" + phoneNumberList[i];
                else
                    strReceivers += "|a^" + phoneNumberList[i];
            }

            return strReceivers;
        }
    }
#endif

#if Soulbrain_MCS
    // KT 메시지 서비스 제공업체(모노커뮤니케이션즈)의 라이브러리 직접 사용하는 버전
    internal class MessageBrokerSoulbrainMCS : BaseMessageBroker
    {
        private string DBName = "Soulbrain_SMS";
        private WebDBManager.DBType DBType = WebDBManager.DBType.sqlserver;
        private WebDBManager m_dbMgr = null;
        // 모노커뮤니케이션즈의 MCS 서비스는 사전에 등록된 전화번호만 발신번호로 사용할 수 있음
        private const string m_strCaller = "0418400911";
        private const string m_strUserID = "sbsmartesh1";
        // id : sbsmartesh1, pw : sbsmartesh1!

        public MessageBrokerSoulbrainMCS()
        {
            m_dbMgr = new WebDBManager();
            m_dbMgr.WebServerURL = "http://192.168.254.201:80";
            m_dbMgr.DatabaseType = DBType;
            m_dbMgr.DatabaseName = DBName;
        }

        public bool SendSMSMessage(List<string> phoneNumberList, string strMessage, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_SMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, SMS_MSG, CALLBACK_URL, NOW_DATE, SEND_DATE, CALLBACK, DEST_TYPE, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, STD_ID) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', NULL, NULL, '{2}', '{3}', 0, {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[libSMS]MessageBrokerMCS.SendSMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        public bool SendLMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, MMS_MSG, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, MSG_TYPE, STD_ID, CONTENT_COUNT, CONTENT_DATA) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', '{2}', '{2}', '{3}', {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL, 0, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[libSMS]MessageBrokerMCS.SendLMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        public bool SendMMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strContents = "";
            int nContentsCount = 0;

            foreach (KeyValuePair<MessageContentMMS.ContentType, string> content in contentDatas)
            {
                if (content.Key == MessageContentMMS.ContentType.Image)
                {
                    if (content.Value.Length > 0)
                    {
                        if (strContents.Length == 0)
                            strContents = content.Value + "^1^0";
                        else
                            strContents += "|" + content.Value + "^1^0";

                        nContentsCount++;
                    }
                }
            }

            if (strContents.Length == 0)
                strContents = "NULL";
            else
                strContents = "'" + strContents + "'";

            string strSQL = "Insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, MMS_MSG, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, MSG_TYPE, STD_ID, CONTENT_COUNT, CONTENT_DATA) ";
            strSQL += string.Format("values ('{0}', 0, '{1}', '{2}', '{3}', '{3}', '{4}', {5}, '{6}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL, {7}, {8})",
                m_strUserID, strTitle, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo, nContentsCount, strContents);

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[libSMS]MessageBrokerMCS.SendMMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        private string GetReceiverInfo(List<string> phoneNumberList, int nBeginIndex, int nEndIndex)
        {
            string strReceivers = "";

            for (int i=nBeginIndex;i<nEndIndex;i++)
            {
                if (strReceivers.Length == 0)
                    strReceivers = "a^" + phoneNumberList[i];
                else
                    strReceivers += "|a^" + phoneNumberList[i];
            }

            return strReceivers;
        }
    }
#endif

#if Kakao
    internal class MessageBrokerKakao : BaseMessageBroker
    {
        private string m_strFrontURL = "https://www.biztalk-api.com";
        private string m_strToken = "";
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;

        public MessageBrokerKakao(Common.IDAL.IDataManager commonDataManager, SDMS.IDAL.IDataManager sdmsDataManager)
        {
            m_commonDataManager = commonDataManager;
            m_sdmsDataManager = sdmsDataManager;
        }

        public bool SendSMSMessage(List<string> phoneNumberList, int nSensorReactionHistoryID)
        {
            m_strErrorMessage = "";

            string strTmpltCode = "";
            string strTitle = "";
            string strMessage = MakeMessage(nSensorReactionHistoryID, ref strTmpltCode, ref strTitle);
            if (strTmpltCode.Length == 0 || strMessage.Length == 0)
            {
                m_strErrorMessage = "메시지 만들 수 없음";
                return false;
            }
                        
            KakaoInfo info = GetKakaoInfo();
            if (info == null || info.CountryCode <= 0 || info.SenderKey.Length == 0 || info.BsID.Length == 0 || info.BsPasswd.Length == 0)
            {
                m_strErrorMessage = "KakaoInfo Table 정보 없음";
                return false;
            }

            GetToken(info.BsID, info.BsPasswd);
            if (m_strToken.Length == 0)
            {
                m_strErrorMessage = "토큰 정보 없음";
                return false;
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

            for (int i = 0; i < phoneNumberList.Count; i++)
            {
                JsonManager mgr = new JsonManager();
                mgr.Add("msgIdx", "1");
                mgr.Add("countryCode", info.CountryCode);
                mgr.Add("recipient", phoneNumberList[i]);
                mgr.Add("senderKey", info.SenderKey);
                mgr.Add("message", strMessage);//"SOP 시스템 화재 알람 탐지\n2020-11-12 00:00:01\n[백화점 1층]에서 화재 신호가 탐지되었습니다.");
                mgr.Add("tmpltCode", strTmpltCode);
                mgr.Add("title", strTitle);
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
                    m_strErrorMessage = "[dnsSMS]MessageBrokerMCS.SendSMSMessage Fail : " + ex.Message;
                } 
            }

            return false;
        }
        private KakaoInfo GetKakaoInfo()
        {
            string strErrorMessage = null;
            KakaoInfo info = m_commonDataManager.GetSelectManager().SelectKakaoInfo(out strErrorMessage);

            return info;
        }
        private void GetToken(string bsID, string passwd)
        {
            string url = "/v2/auth/getToken";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_strFrontURL + url);
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

        /// <summary>
        /// 카카오톡은 템플릿 양식에 맞춰서 보내야 전송이 가능하다.
        /// </summary>
        private string MakeMessage(int nSensorReactionHistoryID, ref string strTmpltCode, ref string strTitle)
        {
            string returnMessage = "";
            string strErrorMessage = null;

            string strCondition = string.Format("{0}.ReactionType in (0, 21, 50) And {0}.ID = {1}", SensorReactionHistory.TableName, nSensorReactionHistoryID);

            ArrayList arrResult = m_sdmsDataManager.GetSelectManager().JoinHistroysensorreactionSpatialequipmentzoneSensorZone(null,null, null, strCondition, out strErrorMessage);

            if (arrResult == null || arrResult.Count != 3)
                return "";

            SensorReactionHistory reactionHistory = arrResult[0] as SensorReactionHistory;
            EquipmentZone equipmentZone = arrResult[1] as EquipmentZone;
            SensorZone sensorZone = arrResult[2] as SensorZone;

            string varFacilityType = "";
            string varDateTime = reactionHistory.Time.ToString("yyyy-MM-dd HH:mm:ss");
            string varTest = reactionHistory.Message.Contains("[테스트]") ? "[테스트]" : "";
            string varBuilding = equipmentZone.ZoneName;

            if (sensorZone.SensorType == (int)dnsData.Sensor.Facility.FacilityType.FIRE_SENSOR)
                varFacilityType = "화재";
            else if (sensorZone.SensorType == (int)dnsData.Sensor.Facility.FacilityType.PSM_SENSOR)
                varFacilityType = "누출";
            else if (sensorZone.SensorType == (int)dnsData.Sensor.Facility.FacilityType.BLACKOUT)
                varFacilityType = "정전";
            else if (sensorZone.SensorType == (int)dnsData.Sensor.Facility.FacilityType.STRONG_WIND)
                varFacilityType = "강풍";
            else if (sensorZone.SensorType == (int)dnsData.Sensor.Facility.FacilityType.SUBMERGENCY)
                varFacilityType = "침수";
            else if (sensorZone.SensorType == (int)dnsData.Sensor.Facility.FacilityType.TERROR)
                varFacilityType = "테러";
            else if (sensorZone.SensorType == (int)dnsData.Sensor.Facility.FacilityType.Earthquake)
                varFacilityType = "지진";

            strTitle = varFacilityType + " 알람 ";

            if (reactionHistory.ReactionType == SensorReactionHistory.ReactionTypes.BEGIN_STATUS) // 알람 탐지
            {
                strTmpltCode = "alarm_detect";
                strTitle += "탐지";
                returnMessage = string.Format("SOP 시스템 {0} 알람 탐지\n{1}\n{2}[{3}]에서 {0} 신호가 탐지되었습니다.", varFacilityType, varDateTime, varTest, varBuilding);
            }
            else if (reactionHistory.ReactionType == SensorReactionHistory.ReactionTypes.MALFUNCTION) // 알람 오작동
            {
                strTmpltCode = "alarm_malfunction";
                strTitle += "오작동";
                returnMessage = string.Format("SOP 시스템 {0} 알람 오작동\n{1}\n{2}[{3}]에서 탐지된 {0} 신호가 오작동으로 신고되었습니다.", varFacilityType, varDateTime, varTest, varBuilding);
            }
            else if (reactionHistory.ReactionType == SensorReactionHistory.ReactionTypes.END_STATUS) // 알람 복구
            {
                strTmpltCode = "alarm_clear";
                strTitle += "복구";
                returnMessage = string.Format("SOP 시스템 {0} 알람 복구\n{1}\n{2}[{3}]에서 탐지된 {0} 신호가 복구되었습니다.", varFacilityType, varDateTime, varTest, varBuilding);
            }

            return returnMessage;
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

        public bool SendLMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, int nBeginIndex, int nEndIndex)
        {            
            return false;
        }

        public bool SendMMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas, int nBeginIndex, int nEndIndex)
        {
            return false;
        }
    }
#endif

#if Kakaowork
    internal class MessageBrokerKakaowork : BaseMessageBroker
    {
        private string m_strFrontURL = "https://api.kakaowork.com/v1/";
        private string m_strFindByEmailURL = "users.find_by_email";
        private string m_strConversationsOpenURL = "conversations.open";
        private string m_strMessageSendURL = "messages.send";

        private string m_strAppKey = "06e9c9e5.503c673b75ba435fa3249c5546799df3";//"47f89ebf.6e63d232301f49368ec9dc9a104e7a70";
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private SDMS.IDAL.IDataManager m_sdmsDataManager = null;

        public MessageBrokerKakaowork(Common.IDAL.IDataManager commonDataManager, SDMS.IDAL.IDataManager sdmsDataManager)
        {
            m_commonDataManager = commonDataManager;
            m_sdmsDataManager = sdmsDataManager;
        }

        public bool SendSMSMessage(List<string> emails, string strMessage)
        {
            m_strErrorMessage = "";
                        
            try
            {
                foreach (string email in emails)
                {
                    string strUserID = "";
                    string strChatID = "";
                    if (GetUserID(email, ref strUserID))
                    {
                        if (strUserID.Length == 0)
                            continue;

                        if (GetChatID(strUserID, ref strChatID))
                        {
                            if (strChatID.Length == 0)
                                continue;

                            string url = m_strFrontURL + m_strMessageSendURL + "?conversation_id=" + strChatID + "&text=" + strMessage;

                            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                            request.Method = "POST";
                            request.ContentType = "application/json";
                            request.Timeout = 5000;
                            request.Headers.Add("Authorization", "Bearer " + m_strAppKey);
                            request.Headers.Add("Content-Type", "application/json");

                            // Response 처리
                            string responseText = string.Empty;
                            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                            {
                                Stream respStream = resp.GetResponseStream();
                                using (StreamReader sr = new StreamReader(respStream))
                                {
                                    responseText = sr.ReadToEnd();

                                    //JObject jobj = JObject.Parse(responseText);
                                    //if (jobj != null && jobj["conversation"] != null && jobj["conversation"]["id"] != null)
                                    //    m_strChatID = lblChatID.Text = jobj["conversation"]["id"].ToString();
                                    //else
                                    //    m_strChatID = lblChatID.Text = "";
                                }
                            }

                            System.Diagnostics.Trace.WriteLine("Response : " + responseText); 
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                m_strErrorMessage = "[dnsSMS]MessageBrokerMCS.SendSMSMessage Fail : " + ex.Message;
            }
            

            return false;
        }

        private bool GetUserID(string strEmail, ref string strUserID)
        {
            if (strEmail.Length == 0)
                return false;

            string url = m_strFrontURL + m_strFindByEmailURL + "?email=" + strEmail;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 25000;
            request.Headers.Add("Authorization", "Bearer " + m_strAppKey);
            request.Headers.Add("Content-Type", "application/json");

            try
            {
                // Response 처리
                string responseText = string.Empty;
                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();

                        JObject jobj = JObject.Parse(responseText);
                        if (Convert.ToBoolean(jobj["success"]))
                        {
                            if (jobj != null && jobj["user"] != null && jobj["user"]["id"] != null)
                                strUserID = jobj["user"]["id"].ToString();
                            else
                                return false;
                        }
                        else
                            return false;

                    }
                }

                //System.Diagnostics.Trace.WriteLine("Response : " + responseText);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        private bool GetChatID(string strUserID, ref string strChatID)
        {
            if (strUserID.Length == 0)
                return false;

            string url = m_strFrontURL + m_strConversationsOpenURL + "?user_id=" + strUserID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 5000;
            request.Headers.Add("Authorization", "Bearer " + m_strAppKey);
            request.Headers.Add("Content-Type", "application/json");

            try
            {
                // Response 처리
                string responseText = string.Empty;
                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();

                        JObject jobj = JObject.Parse(responseText);
                        if (Convert.ToBoolean(jobj["success"]))
                        {
                            if (jobj != null && jobj["conversation"] != null && jobj["conversation"]["id"] != null)
                                strChatID = jobj["conversation"]["id"].ToString();
                            else
                                return false;
                        }
                        else
                            return false;
                    }
                }

                //System.Diagnostics.Trace.WriteLine("Response : " + responseText);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        public bool SendLMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, int nBeginIndex, int nEndIndex)
        {
            return false;
        }

        public bool SendMMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas, int nBeginIndex, int nEndIndex)
        {
            return false;
        }
    }
#endif

#if External_UNE_MCS
    // 외부에서 UNE_MCS 사용하기 위한 버전
    internal class MessageBrokerExternal_MCS : BaseMessageBroker
    {
        public MessageBrokerExternal_MCS()
        {

        }

        public bool SendQuery(Dictionary<string, string> dicHeaders, string strBodyJson, string strURL, out string strErrorMessage, string strMethodType = "GET")
        {
            strErrorMessage = "";
            string url = "http://221.147.100.161:8099";

            if (strURL.StartsWith("/"))
                url += strURL;
            else
                url += "/" + strURL;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = strMethodType;

            if (dicHeaders != null)
            {
                request.ContentType = "application/json; charset=utf-8";

                // 요청 헤더 추가
                foreach (KeyValuePair<string, string> pair in dicHeaders)
                {
                    string key = pair.Key;
                    string value = pair.Value;
                    request.Headers.Add(key, value);
                }
            }

            string strResponse = "";

            try
            {
                if (strBodyJson != null && strBodyJson != "")
                {
                    StreamWriter streamWriter = new StreamWriter(request.GetRequestStream());
                    streamWriter.Write(strBodyJson);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResponse = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

            }
            catch (WebException ex)
            {
                strErrorMessage = ex.Status.ToString();
                m_strErrorMessage = strErrorMessage;
                return false;
            }

            if (strResponse == null)
            {
                strErrorMessage = "Request 실패";
                return false;
            }

            strErrorMessage = "success";
            m_strErrorMessage = "success"; 
            return true;
        }

    }
#endif
}
