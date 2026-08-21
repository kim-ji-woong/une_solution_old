using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web;
using System.Net;
using DBUtility; 

namespace libSMS
{    
	internal class MessageBroker
	{
		private MessageQueue m_MessageQueue = null;

		private string m_szServerIP = "";
		// Remote에서 Queue접근시 ID
		private string m_szQueueID = "";

		internal MessageBroker(string szServerIP)
		{

            m_szServerIP = szServerIP;

            m_szQueueID = string.Format("FormatName:Direct=TCP:{0}\\private$\\smsreciver", szServerIP);
			m_MessageQueue = new MessageQueue(m_szQueueID);
		}

        // strMessage에 작은 따옴표가 들어있는지 검사한다.
        private string CheckQuotation(string strMessage)
        {
            return strMessage.Replace("'", "''");
        }
		
		internal bool SendMessage(MessageContent content)
		{
			if (m_MessageQueue == null)
				return false;		

			try
			{
                content.Message = CheckQuotation(content.Message);

				// Message 생성
				System.Messaging.Message msg = new System.Messaging.Message();
				// 전송할 Body Data를 지정
				msg.Body = content;
				//msg.UseEncryption = true;
				// Message Queue로 전송					
				msg.Label = "단문전송";
				
                // 장애발생할 경우 재전송여부
                msg.Recoverable = false;

				m_MessageQueue.Send(msg, MessageQueueTransactionType.Single);
				return true;				
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.StackTrace);
			}
			return false;
		}
	}

    internal class MessageBrokerUNE
    {       
        string strUserID = "unes5588";
        string strPassword = "ue94499660";
        string strCaller = "027144133";

        private string m_szServerIP = "";

        dynamic message = null;

        internal MessageBrokerUNE(string szServerIP)
		{
            m_szServerIP = szServerIP;

            message = new ezSMSComponent.Message();
		}

        public bool InsertMessage(List<MessageContent> arList)
        {
            bool bResult = true;
            foreach (MessageContent content in arList)
            {
                bResult = InsertMessage(content);
            }
            return bResult;
        }

        public bool InsertMessage(MessageContent conetnt)
        {
            if (conetnt == null)
            {
                return false;
            }

            try
            {
                message.SetAccountInfo(strUserID, strPassword);

                ezSMSComponent.Receivers receivers = (ezSMSComponent.Receivers)message.CreateReceivers();
                receivers.AddDirect(conetnt.Reciver, conetnt.Message, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);

                ezSMSComponent.Contents contents = message.CreateContents();
                contents.Type = ezSMSComponent.EZSMS_MESSAGE_TYPE.EZSMS_MESSAGE_TYPE_SMS;
                contents.Message = conetnt.Message;
                contents.Subject = "";

                ezSMSComponent.ISendResults results = message.Send(strCaller, (ezSMSComponent.IReceivers)receivers, contents);
                bool succeeded = false;
                if (results.Count > 0)
                {
                    ezSMSComponent.SendResult result = (ezSMSComponent.SendResult)results[0];
                    if (result.Result == ezSMSComponent.EZSMS_RESULT.EZSMS_SUCCEEDED)
                        succeeded = true;
                }

                return succeeded;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }

            return false;
        }

    }

    internal class MessageBrokerKPX
    {
      
        string strUserID = "kpxglobal";
        string strPassword = "kpx123";
        string strCaller = "0522676655";

        private string m_szServerIP = "";

        dynamic message = null;

        internal MessageBrokerKPX(string szServerIP)
		{
            m_szServerIP = szServerIP;

            message = new ezSMSComponent.Message();
		}

        public bool InsertMessage(List<MessageContent> arList)
        {
            bool bResult = true;
            foreach (MessageContent content in arList)
            {
                bResult = InsertMessage(content);
            }
            return bResult;
        }

        public bool InsertMessage(MessageContent conetnt)
        {
            if (conetnt == null)
            {
                return false;
            }

            message.SetAccountInfo(strUserID, strPassword);

            ezSMSComponent.Receivers receivers = (ezSMSComponent.Receivers)message.CreateReceivers();
            receivers.AddDirect(conetnt.Reciver, conetnt.Message, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);

            ezSMSComponent.Contents contents = message.CreateContents();
            contents.Type = ezSMSComponent.EZSMS_MESSAGE_TYPE.EZSMS_MESSAGE_TYPE_SMS;
            contents.Message = conetnt.Message;
            contents.Subject = "";

            ezSMSComponent.ISendResults results = message.Send(strCaller, (ezSMSComponent.IReceivers)receivers, contents);
            bool succeeded = false;
            if (results.Count > 0)
            {
                ezSMSComponent.SendResult result = (ezSMSComponent.SendResult)results[0];
                if (result.Result == ezSMSComponent.EZSMS_RESULT.EZSMS_SUCCEEDED)
                    succeeded = true;
            }

            return succeeded;

        }
    }

    // 서울대 메시지 서비스 제공업체(코아인텍)의 라이브러리 직접 사용하는 버전
    internal class MessageBrokerSNU
    {
        private string DBName = "SNU_SMS";
        private WebDBManager.DBType DBType = WebDBManager.DBType.mysql;
        private WebDBManager m_dbMgr = null;

        public MessageBrokerSNU(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dbMgr.DatabaseType = DBType;
            m_dbMgr.DatabaseName = DBName;
        }

        private string GetCurrentTime()
        {
            string strTime = "";

            if (m_dbMgr.DatabaseType == WebDBManager.DBType.sqlserver)
            {
                System.Collections.ArrayList arrResult = m_dbMgr.GetResultData("Select GetDate()", 0);

                if (arrResult == null || arrResult.Count < 1)
                    return "";

                strTime = WebDBManager.GetStringField(arrResult[0]);
            }
            else if (m_dbMgr.DatabaseType == WebDBManager.DBType.mysql)
            {
                System.Collections.ArrayList arrResult = m_dbMgr.GetResultData("SELECT current_date(), current_time()", 0);

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

        // strMessage에 작은 따옴표가 들어있는지 검사한다.
        private string CheckQuotation(string strMessage)
        {
            return strMessage.Replace("'", "''");
        }

        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = GetCurrentTime();
            /*DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);*/
            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "insert into SDK_SMS_SEND (USER_ID,SCHEDULE_TYPE,SUBJECT,SMS_MSG,CALLBACK_URL,NOW_DATE,SEND_DATE,CALLBACK,DEST_TYPE,DEST_COUNT,DEST_INFO ";
            strSQL += ",KT_OFFICE_CODE,CDR_ID,RESERVED1,RESERVED2,RESERVED3,RESERVED4,RESERVED5,RESERVED6,RESERVED7,RESERVED8,RESERVED9,SEND_STATUS,SEND_COUNT,SEND_RESULT,SEND_PROC_TIME,STD_ID) ";
            strSQL += string.Format("values ('agent_test', 0, 'e-재난 SMS', '{0}', NULL, '{1}', '{1}', '{2}', 0, {3}, '{4}', ", strMessage, strTime, strCaller, phoneNumberList.Count, strReceiverInfo);
            strSQL += "'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL)";

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = GetCurrentTime();
            /*DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);*/
            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, MMS_MSG, CONTENT_COUNT, CONTENT_DATA, KT_OFFICE_CODE, CDR_ID ";
            strSQL += ",RESERVED1,RESERVED2,RESERVED3,RESERVED4,RESERVED5,RESERVED6,RESERVED7,RESERVED8,RESERVED9,SEND_STATUS,SEND_COUNT,SEND_RESULT,SEND_PROC_TIME,MSG_TYPE,STD_ID) ";
            strSQL += string.Format("values ('agent_test', 0, 'e-재난 MMS', '{0}', '{0}', '{1}', {2}, '{3}', '{4}', ", strTime, strCaller, phoneNumberList.Count, strReceiverInfo, strMessage);
            strSQL += "0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL)";

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas)
        {
            string strTime = GetCurrentTime();
            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
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

            string strSQL = "insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, MMS_MSG, CONTENT_COUNT, CONTENT_DATA, KT_OFFICE_CODE, CDR_ID ";
            strSQL += ",RESERVED1,RESERVED2,RESERVED3,RESERVED4,RESERVED5,RESERVED6,RESERVED7,RESERVED8,RESERVED9,SEND_STATUS,SEND_COUNT,SEND_RESULT,SEND_PROC_TIME,MSG_TYPE,STD_ID) ";
            strSQL += string.Format("values ('agent_test', 0, '{0}', '{1}', '{1}', '{2}', {3}, '{4}', '{5}', {6}, {7}, ", strTitle, strTime, strCaller, phoneNumberList.Count, strReceiverInfo, strMessage, nContentsCount, strContents);
            strSQL += "NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL)";

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        private string GetReceiverInfo(List<string> phoneNumberList)
        {
            string strReceivers = "";

            foreach (string strReceiver in phoneNumberList)
            {
                if (strReceivers.Length == 0)
                    strReceivers = "a^" + strReceiver;
                else
                    strReceivers += "|a^" + strReceiver;
            }

            return strReceivers;
        }
    }

    // 서울대 SMS Web Service를 이용하는 버전
    /*internal class MessageBrokerSNU
    {
        private CookieContainer cookieContainer = new CookieContainer();

        private System.Text.Encoding m_PageEncoding = Encoding.UTF8;

        string ConnStr = "http://survey1.snu.ac.kr:8080/jsp/sms/smsApi.jsp"; // SMS 서버 주소 URL

        string SysCodeStr = "0031A_003"; // 시스템코드

        // 시스템 ID (SYS_ID)                                         : 620
        // 시스템 코드 (SYS_CODE)                               : 0031A_003
        // 시스템명 (SYSTEM_NAME)                            : 캠퍼스관리과_e재난시스템

        private static int m_nMsgID = 620;

        private static char ConvertToHex(char cSource)
        {
            return "0123456789abcdef"[0x0f & cSource];
        }

        private static string URLEncoding(byte[] bytes)
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

        // Return 값 : MsgIdStr(발송 완료)
        //             -0 : 시스템 오류
        //             -1 : 메시지 ID 값 없음
        //             -2 : 시스템 코드 값 오류(미등록 서버)
        //             -3 : 수신자 핸드폰 번호가 없음
        //             -4 : 전송할 메시지가 없음
        //             -5 : 발송제한 (과금기관 발송한도 금액 초과 등으로 인한 거절 등)
        internal string SendSmsMessage(string FromNumberStr, string ToNumber, string TextStr)
        {
            string resResult = string.Empty;

            string MsgIdStr = m_nMsgID.ToString();

            if ((MsgIdStr == null) || (MsgIdStr.Equals("")))
            {
                return "-1";
            }
            if ((SysCodeStr == null) || (SysCodeStr.Equals("")))
            {
                return "-2";
            }
            if ((ToNumber == null) || (ToNumber.Equals("")))
            {
                return "-3";
            }
            if ((TextStr == null) || (TextStr.Equals("")))
            {
                return "-4";
            }

            string sourceUrl = ConnStr;

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(TextStr);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "MSG_ID=" + MsgIdStr + "&" + "SYS_CODE=" + SysCodeStr + "&" +
                              "FROM_NUMBER=" + FromNumberStr + "&" + "TO_NUMBER=" + ToNumber + "&" +
                              "TEXT=" + strUrlEncode;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest mRequest = (HttpWebRequest)WebRequest.Create(sourceUrl);

            //lock (this)
            {
                mRequest.Method = "POST";
                mRequest.ContentType = "application/x-www-form-urlencoded";
                mRequest.ContentLength = bytes.Length;
                mRequest.CookieContainer = cookieContainer;

                try
                {
                    using (Stream writeStream = mRequest.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }

                    HttpWebResponse wRes = (HttpWebResponse)mRequest.GetResponse();

                    if (wRes.StatusCode == HttpStatusCode.OK)
                    {
                        Stream respPostStream = wRes.GetResponseStream();
                        StreamReader readerPost = new StreamReader(respPostStream, m_PageEncoding);
                        resResult = readerPost.ReadToEnd();

                        readerPost.Close();
                        respPostStream.Close();
                    }
                    else
                    {
                        resResult = "-2";
                    }

                    wRes.Close();
                    mRequest.Abort();

                }
                catch (System.Net.WebException ex)
                {
                    System.Diagnostics.Trace.WriteLine("A recoverable exception occurred, retrying.  " + ex.Message);
                    resResult = "-2";
                }
                catch (System.IO.IOException exx)
                {
                    System.Diagnostics.Trace.WriteLine("Fail ! : " + exx.Message);
                    resResult = "-2";
                }
            }
            return resResult;
        }
    }*/

    // KT 메시지 서비스 제공업체(모노커뮤니케이션즈)의 라이브러리 직접 사용하는 버전
    internal class MessageBrokerKDHC
    {
        private string DBName = "KDHC_SMS";
        private WebDBManager.DBType DBType = WebDBManager.DBType.mysql;
        private WebDBManager m_dbMgr = null;
        // 모노커뮤니케이션즈의 MCS 서비스는 사전에 등록된 전화번호만 발신번호로 사용할 수 있음
        private const string m_strCaller = "";//"027144133";
        private const string m_strUserID = "";//"une9966";

        public MessageBrokerKDHC(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dbMgr.DatabaseType = DBType;
            m_dbMgr.DatabaseName = DBName;
        }

        private string GetCurrentTime()
        {
            string strTime = "";

            if (m_dbMgr.DatabaseType == WebDBManager.DBType.sqlserver)
            {
                System.Collections.ArrayList arrResult = m_dbMgr.GetResultData("Select GetDate()", 0);

                if (arrResult == null || arrResult.Count < 1)
                    return "";

                strTime = WebDBManager.GetStringField(arrResult[0]);
            }
            else if (m_dbMgr.DatabaseType == WebDBManager.DBType.mysql)
            {
                System.Collections.ArrayList arrResult = m_dbMgr.GetResultData("SELECT current_date(), current_time()", 0);

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

        // strMessage에 작은 따옴표가 들어있는지 검사한다.
        private string CheckQuotation(string strMessage)
        {
            return strMessage.Replace("'", "''");
        }

        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = GetCurrentTime();
            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_SMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, SMS_MSG, CALLBACK_URL, NOW_DATE, SEND_DATE, CALLBACK, DEST_TYPE, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, STD_ID) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', NULL, NULL, '{2}', '{3}', 0, {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL)",
                "agent_test", strMessage, strTime, strCaller, phoneNumberList.Count, strReceiverInfo);

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = GetCurrentTime();
            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, MMS_MSG, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, MSG_TYPE, STD_ID, CONTENT_COUNT, CONTENT_DATA) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', '{2}', '{2}', '{3}', {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL, 0, NULL)",
                "agent_test", strMessage, strTime, strCaller, phoneNumberList.Count, strReceiverInfo);

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas)
        {
            string strTime = GetCurrentTime();
            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
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

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        private string GetReceiverInfo(List<string> phoneNumberList)
        {
            string strReceivers = "";

            foreach (string strReceiver in phoneNumberList)
            {
                if (strReceivers.Length == 0)
                    strReceivers = "a^" + strReceiver;
                else
                    strReceivers += "|a^" + strReceiver;
            }

            return strReceivers;
        }
    }

    // LG
    internal class MessageBrokerKDHCLG
    {
        private string DBName = "KDHC_SMS";
        private WebDBManager.DBType DBType = WebDBManager.DBType.mysql;
        private WebDBManager m_dbMgr = null;
        // 모노커뮤니케이션즈의 MCS 서비스는 사전에 등록된 전화번호만 발신번호로 사용할 수 있음
        private const string m_strCaller = "15225267";//"027144133";
        private const string m_strUserID = "";//"une9966";

        public MessageBrokerKDHCLG(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dbMgr.DatabaseType = DBType;
            m_dbMgr.DatabaseName = DBName;
        }

        private string GetCurrentTime()
        {
            string strTime = "";

            if (m_dbMgr.DatabaseType == WebDBManager.DBType.sqlserver)
            {
                System.Collections.ArrayList arrResult = m_dbMgr.GetResultData("Select GetDate()", 0);

                if (arrResult == null || arrResult.Count < 1)
                    return "";

                strTime = WebDBManager.GetStringField(arrResult[0]);
            }
            else if (m_dbMgr.DatabaseType == WebDBManager.DBType.mysql)
            {
                System.Collections.ArrayList arrResult = m_dbMgr.GetResultData("SELECT current_date(), current_time()", 0);

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

        // strMessage에 작은 따옴표가 들어있는지 검사한다.
        private string CheckQuotation(string strMessage)
        {
            return strMessage.Replace("'", "''");
        }

        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = GetCurrentTime();
            //string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);
            
            foreach (string phoneNumber in phoneNumberList)
            {
                string strSQL = "INSERT INTO SC_TRAN (TR_SENDDATE, TR_SENDSTAT, TR_MSGTYPE, TR_PHONE, TR_CALLBACK, TR_MSG)  ";
                strSQL += string.Format("VALUES ('{0}', '0', '0', '{1}', '{2}', '{3}')", strTime, phoneNumber, strCaller, strMessage);

                m_dbMgr.GetResultData(strSQL, 0);
            }

            return true;
        }

        // LMS (멀티미디어 컨텐츠 미첨부)
        public bool SendLMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage)
        {             
            //INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, TYPE)
            //VALUES ('[차세대MMS 전송테스트]', '수신 번호', '발신 번호', '0', NOW(), 'MESSAGE', '0');

            string strTime = GetCurrentTime();
            //string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            foreach (string phoneNumber in phoneNumberList)
            {
                string strSQL = "INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, TYPE) ";
                strSQL += string.Format("VALUES ('{0}', '{1}', '{2}', '0', '{3}', '{4}', '0')",
                    strTitle, phoneNumber, strCaller, strTime, strMessage);

                return m_dbMgr.GetResultData(strSQL, 0) != null; 
            }

            return true;
        }

        // MMS (멀티미디어 컨텐츠 첨부)
        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas)
        {    
            //INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, FILE_CNT, FILE_PATH1, TYPE) 
            //VALUES ('[차세대MMS 전송테스트]', '수신 번호', '발신 번호', '0', NOW(), 'MESSAGE', '1', 'D:\\UPLUSAGT\\image\\test.jpg', '0');

            string strTime = GetCurrentTime();
            //string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            if (contentDatas != null && contentDatas.Count > 0)
            {
                string path = contentDatas[0].Value;
                foreach (string phoneNumber in phoneNumberList)
                {
                    string strSQL1 = "INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, FILE_CNT, FILE_PATH1, TYPE)";
                    strSQL1 += string.Format("VALUES ('{0}', '{1}', '{2}', '0', '{3}', '{4}', '1', '{5}', '0')",
                        strTitle, phoneNumber, strCaller, strTime, strMessage, path);
                     
                    m_dbMgr.GetResultData(strSQL1, 0);
                }
            } 

            return true;
        } 

        private string GetReceiverInfo(List<string> phoneNumberList)
        {
            string strReceivers = "";

            foreach (string strReceiver in phoneNumberList)
            {
                if (strReceivers.Length == 0)
                    strReceivers = "a^" + strReceiver;
                else
                    strReceivers += "|a^" + strReceiver;
            }

            return strReceivers;
        }
    }

    // KT 메시지 서비스 제공업체(모노커뮤니케이션즈)의 라이브러리 직접 사용하는 버전
    internal class MessageBrokerMCS
    {
        private string DBName = "UNE_SMS";
        private WebDBManager.DBType DBType = WebDBManager.DBType.sqlserver;
        private WebDBManager m_dbMgr = null;
        // 모노커뮤니케이션즈의 MCS 서비스는 사전에 등록된 전화번호만 발신번호로 사용할 수 있음
        private const string m_strCaller = "027144133";
        private const string m_strUserID = "une9966";

        public MessageBrokerMCS(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dbMgr.WebServerURL = "http://192.168.0.250:8080/SOP";
            //m_dbMgr.WebServerURL = "http://unes.iptime.org:10091/SOP";
            m_dbMgr.DatabaseHost = "127.0.0.1";
            m_dbMgr.DatabaseType = DBType;
            m_dbMgr.DatabaseName = DBName;
        }

        private string GetCurrentTime()
        {
            string strTime = "";

            if (m_dbMgr.DatabaseType == WebDBManager.DBType.sqlserver)
            {
                System.Collections.ArrayList arrResult = m_dbMgr.GetResultData("Select GetDate()", 0);

                if (arrResult == null || arrResult.Count < 1)
                    return "";

                strTime = WebDBManager.GetStringField(arrResult[0]);
            }
            else if (m_dbMgr.DatabaseType == WebDBManager.DBType.mysql)
            {
                System.Collections.ArrayList arrResult = m_dbMgr.GetResultData("SELECT current_date(), current_time()", 0);

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

        // strMessage에 작은 따옴표가 들어있는지 검사한다.
        private string CheckQuotation(string strMessage)
        {
            return strMessage.Replace("'", "''");
        }

        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = GetCurrentTime();
            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_SMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, SMS_MSG, CALLBACK_URL, NOW_DATE, SEND_DATE, CALLBACK, DEST_TYPE, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, STD_ID) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', NULL, NULL, '{2}', '{3}', 0, {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = GetCurrentTime();
            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, MMS_MSG, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, MSG_TYPE, STD_ID, CONTENT_COUNT, CONTENT_DATA) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', '{2}', '{2}', '{3}', {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL, 0, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);
            
            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas)
        {
            string strTime = GetCurrentTime();
            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
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

            return m_dbMgr.GetResultData(strSQL, 0) != null;
        }

        private string GetReceiverInfo(List<string> phoneNumberList)
        {
            string strReceivers = "";

            foreach (string strReceiver in phoneNumberList)
            {
                if (strReceivers.Length == 0)
                    strReceivers = "a^" + strReceiver;
                else
                    strReceivers += "|a^" + strReceiver;
            }

            return strReceivers;
        }
    }
}
