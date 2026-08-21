using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web;
using System.Net;
using MySql.Data.MySqlClient;
using System.Collections;
using DBUtility2;

namespace libSMS
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

#if KDN
    internal class MessageBroker : BaseMessageBroker
    {
        internal MessageBroker()
        {
        }

        internal bool SendMessage(MessageContent content)
        {
            DirectDBManager dbMgr = null;

            if (content.Tag != null && content.Tag is DirectDBManager)
                dbMgr = ((DirectDBManager)content.Tag).Clone();

            if (dbMgr == null)
                return false;

            try
            {
                if (dbMgr.Connect())
                {
                    foreach (string strPhoneNumber in content.PhoneNumbers)
                    {
                        string strSQL = string.Format("Insert into SMS_Data (PhoneNumber, Message) values ('{0}', '{1}')", strPhoneNumber, content.Message);
                        dbMgr.GetResultData(strSQL);
                    }

                    dbMgr.Close();
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
        /*private string m_strServerIP = "10.131.5.6";
        private string m_strID = "smsuser";
        private string m_strPW = "smsnd";
        private string m_strDBName = "pamts_sms2";
        private string m_strCharSet = "utf8";
        private string m_strMsgCharSet = "UTF-8";
        private string m_sid = "05140998";

        internal MessageBroker()
        {
        }

        internal bool SendMessage(MessageContent content)
        {
            MySqlConnection connection = null;

            try
            {
                content.Message = CheckQuotation(content.Message);

                m_strErrorMessage = "";
                connection = Connect();

                if (connection == null || connection.State == System.Data.ConnectionState.Closed || connection.State == System.Data.ConnectionState.Broken)
                    return false;

                MySqlTransaction transaction = connection.BeginTransaction();

                Encoding enc = Encoding.GetEncoding(m_strMsgCharSet);
                byte[] bytes1 = Encoding.UTF8.GetBytes(content.Message);
                byte[] bytes2 = Encoding.Convert(Encoding.UTF8, enc, bytes1);
                string szMsg = enc.GetString(bytes2);

                foreach (string strPhoneNumber in content.PhoneNumbers)
                {
                    string strSQL = string.Format("insert into LOG_SMS (user_id, write_time, destination, callback, body, send_flag, del_flag) values ('{0}',now(),'{1}','{2}','{3}','1','N')", m_sid, strPhoneNumber, content.Caller, szMsg);

                    using (MySqlCommand cmd = new MySqlCommand(strSQL, connection, transaction))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                m_strErrorMessage = "[libSMS]MessageBroker.SendMessage Fail";
                m_strErrorMessage += ex.Message;
                connection.Close();
            }

            return false;
        }

        // 한전 KDN의 문자서비스에 접속
        private MySqlConnection Connect()
        {
            string strConn = string.Format("Server={0};Database={1};Uid={2};Pwd={3};Charset={4}", m_strServerIP, m_strDBName, m_strID, m_strPW, m_strCharSet);

            try
            {
                MySqlConnection connection = new MySqlConnection(strConn);
                connection.Open();
                return connection;

            }
            catch (Exception ex)
            {
                m_strErrorMessage = "[libSMS]MessageBroker.Connect Fail : " + strConn;
                m_strErrorMessage += ex.Message;
            }

            return null;
        }*/
    }
#endif

#if UNE_EZ_SMS
    internal class MessageBrokerUNE : BaseMessageBroker
    {
        string m_strUserID = "unes5588";
        string m_strPassword = "ue94499660";
        string m_strCaller = "027144133";

        dynamic message = null;

        internal MessageBrokerUNE()
        {
            message = new ezSMSComponent.Message();
        }

        public bool SendSMS(List<string> phoneNumbers, string strMessage)
        {
            message.SetAccountInfo(m_strUserID, m_strPassword);

            try
            {
                ezSMSComponent.Receivers receivers = (ezSMSComponent.Receivers)message.CreateReceivers();

                foreach (string strPhoneNumber in phoneNumbers)
                {
                    receivers.AddDirect(strPhoneNumber, "", ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);
                }

                ezSMSComponent.Contents contents = message.CreateContents();
                contents.Type = ezSMSComponent.EZSMS_MESSAGE_TYPE.EZSMS_MESSAGE_TYPE_SMS;
                contents.Message = strMessage;
                contents.Subject = "";

                ezSMSComponent.ISendResults results = message.Send(m_strCaller, (ezSMSComponent.IReceivers)receivers, contents);
                bool succeeded = false;
                if (results.Count > 0)
                {
                    ezSMSComponent.SendResult result = (ezSMSComponent.SendResult)results[0];
                    if (result.Result == ezSMSComponent.EZSMS_RESULT.EZSMS_SUCCEEDED)
                        succeeded = true;
                }

                if (succeeded == false)
                    m_strErrorMessage = "[libSMS]MessageBrokerUNE.SendSMS Fail";

                return succeeded;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("[libSMS]MessageBrokerUNE.SendSMS Fail : " + e.Message);
                m_strErrorMessage = "[libSMS]MessageBrokerUNE.SendSMS Fail : " + e.Message;
            }

            return false;
        }

        public bool SendLMS(List<string> phoneNumbers, string strMessage, string strTitle)
        {
            message.SetAccountInfo(m_strUserID, m_strPassword);

            try
            {
                ezSMSComponent.Receivers receivers = (ezSMSComponent.Receivers)message.CreateReceivers();

                foreach (string strPhoneNumber in phoneNumbers)
                {
                    receivers.AddDirect(strPhoneNumber, "", ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);
                }

                ezSMSComponent.Contents contents = message.CreateContents();
                contents.Type = ezSMSComponent.EZSMS_MESSAGE_TYPE.EZSMS_MESSAGE_TYPE_LMS;
                contents.Message = strMessage;
                contents.Subject = strTitle;

                ezSMSComponent.ISendResults results = message.Send(m_strCaller, (ezSMSComponent.IReceivers)receivers, contents);
                bool succeeded = false;
                if (results.Count > 0)
                {
                    ezSMSComponent.SendResult result = (ezSMSComponent.SendResult)results[0];
                    if (result.Result == ezSMSComponent.EZSMS_RESULT.EZSMS_SUCCEEDED)
                        succeeded = true;
                }

                if (succeeded == false)
                    m_strErrorMessage = "[libSMS]MessageBrokerUNE.SendLMS Fail";

                return succeeded;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("[libSMS]MessageBrokerUNE.SendLMS Fail : " + e.Message);
                m_strErrorMessage = "[libSMS]MessageBrokerUNE.SendLMS Fail : " + e.Message;
            }

            return false;
        }
    }
#endif

#if KPX
    internal class MessageBrokerKPX : BaseMessageBroker
    {
        string m_strUserID = "kpxglobal";
        string m_strPassword = "kpx123";
        string m_strCaller = "0522676655";

        dynamic message = null;

        internal MessageBrokerKPX()
        {
            message = new ezSMSComponent.Message();
        }

        public bool SendSMS(List<string> phoneNumbers, string strMessage)
        {
            message.SetAccountInfo(m_strUserID, m_strPassword);

            ezSMSComponent.Receivers receivers = (ezSMSComponent.Receivers)message.CreateReceivers();

            foreach (string strPhoneNumber in phoneNumbers)
            {
                receivers.AddDirect(strPhoneNumber, "", ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);
            }

            ezSMSComponent.Contents contents = message.CreateContents();
            contents.Type = ezSMSComponent.EZSMS_MESSAGE_TYPE.EZSMS_MESSAGE_TYPE_SMS;
            contents.Message = strMessage;
            contents.Subject = "";

            ezSMSComponent.ISendResults results = message.Send(m_strCaller, (ezSMSComponent.IReceivers)receivers, contents);
            bool succeeded = false;
            if (results.Count > 0)
            {
                ezSMSComponent.SendResult result = (ezSMSComponent.SendResult)results[0];
                if (result.Result == ezSMSComponent.EZSMS_RESULT.EZSMS_SUCCEEDED)
                    succeeded = true;
            }

            if (succeeded == false)
                m_strErrorMessage = "[libSMS]MessageBrokerKPX.SendSMS Fail";

            return succeeded;
        }

        public bool SendLMS(List<string> phoneNumbers, string strMessage, string strTitle)
        {
            message.SetAccountInfo(m_strUserID, m_strPassword);

            ezSMSComponent.Receivers receivers = (ezSMSComponent.Receivers)message.CreateReceivers();

            foreach (string strPhoneNumber in phoneNumbers)
            {
                receivers.AddDirect(strPhoneNumber, "", ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);
            }

            ezSMSComponent.Contents contents = message.CreateContents();
            contents.Type = ezSMSComponent.EZSMS_MESSAGE_TYPE.EZSMS_MESSAGE_TYPE_LMS;
            contents.Message = strMessage;
            contents.Subject = strTitle;

            ezSMSComponent.ISendResults results = message.Send(m_strCaller, (ezSMSComponent.IReceivers)receivers, contents);
            bool succeeded = false;
            if (results.Count > 0)
            {
                ezSMSComponent.SendResult result = (ezSMSComponent.SendResult)results[0];
                if (result.Result == ezSMSComponent.EZSMS_RESULT.EZSMS_SUCCEEDED)
                    succeeded = true;
            }

            if (succeeded == false)
                m_strErrorMessage = "[libSMS]MessageBrokerKPX.SendLMS Fail";

            return succeeded;
        }
    }
#endif

#if SNU
    // 서울대 메시지 서비스 제공업체(코아인텍)의 라이브러리 직접 사용하는 버전
    internal class MessageBrokerSNU : BaseMessageBroker
    {
        private string DBName = "SNU_SMS";
        private WebDBManager.DBType DBType = WebDBManager.DBType.mysql;
        private WebDBManager m_dbMgr = null;

        public MessageBrokerSNU(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dbMgr.DatabaseType = DBType;
        }

        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "insert into SDK_SMS_SEND (USER_ID,SCHEDULE_TYPE,SUBJECT,SMS_MSG,CALLBACK_URL,NOW_DATE,SEND_DATE,CALLBACK,DEST_TYPE,DEST_COUNT,DEST_INFO ";
            strSQL += ",KT_OFFICE_CODE,CDR_ID,RESERVED1,RESERVED2,RESERVED3,RESERVED4,RESERVED5,RESERVED6,RESERVED7,RESERVED8,RESERVED9,SEND_STATUS,SEND_COUNT,SEND_RESULT,SEND_PROC_TIME,STD_ID) ";
            strSQL += string.Format("values ('agent_test', 0, 'e-재난 SMS', '{0}', NULL, '{1}', '{1}', '{2}', 0, {3}, '{4}', ", strMessage, strTime, strCaller, phoneNumberList.Count, strReceiverInfo);
            strSQL += "'', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL)";

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[libSMS]MessageBrokerSNU.SendSMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        public bool SendLMSMessage(string strCaller, List<string> phoneNumberList, string strMessage, int nBeginIndex, int nEndIndex)
        {
            m_strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbMgr, DBName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, MMS_MSG, CONTENT_COUNT, CONTENT_DATA, KT_OFFICE_CODE, CDR_ID ";
            strSQL += ",RESERVED1,RESERVED2,RESERVED3,RESERVED4,RESERVED5,RESERVED6,RESERVED7,RESERVED8,RESERVED9,SEND_STATUS,SEND_COUNT,SEND_RESULT,SEND_PROC_TIME,MSG_TYPE,STD_ID) ";
            strSQL += string.Format("values ('agent_test', 0, 'e-재난 MMS', '{0}', '{0}', '{1}', {2}, '{3}', '{4}', ", strTime, strCaller, phoneNumberList.Count, strReceiverInfo, strMessage);
            strSQL += "0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL)";

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[libSMS]MessageBrokerSNU.SendLMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            return false;
        }

        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas, int nBeginIndex, int nEndIndex)
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

            string strSQL = "insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, MMS_MSG, CONTENT_COUNT, CONTENT_DATA, KT_OFFICE_CODE, CDR_ID ";
            strSQL += ",RESERVED1,RESERVED2,RESERVED3,RESERVED4,RESERVED5,RESERVED6,RESERVED7,RESERVED8,RESERVED9,SEND_STATUS,SEND_COUNT,SEND_RESULT,SEND_PROC_TIME,MSG_TYPE,STD_ID) ";
            strSQL += string.Format("values ('agent_test', 0, '{0}', '{1}', '{1}', '{2}', {3}, '{4}', '{5}', {6}, {7}, ", strTitle, strTime, strCaller, phoneNumberList.Count, strReceiverInfo, strMessage, nContentsCount, strContents);
            strSQL += "NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL)";

            if (m_dbMgr.GetResultData(strSQL, DBName) != null)
                return true;

            m_strErrorMessage = "[libSMS]MessageBrokerSNU.SendMMSMessage Fail : " + m_dbMgr.LastErrorMessage;
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

#if KDHCLG
    // LG
    internal class MessageBrokerKDHCLG : BaseMessageBroker
    {
        private string DBName = "KDHC_SMS";
        private WebDBManager.DBType DBType = WebDBManager.DBType.mysql;
        private WebDBManager m_dbMgr = null;
        
        public MessageBrokerKDHCLG(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dbMgr.DatabaseType = DBType;
        }

        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = GetCurrentTime(m_dbMgr, DBName);
            strMessage = CheckQuotation(strMessage);

            m_strErrorMessage = "";

            foreach (string phoneNumber in phoneNumberList)
            {
                string strSQL = "INSERT INTO SC_TRAN (TR_SENDDATE, TR_SENDSTAT, TR_MSGTYPE, TR_PHONE, TR_CALLBACK, TR_MSG)  ";
                strSQL += string.Format("VALUES ('{0}', '0', '0', '{1}', '{2}', '{3}')", strTime, phoneNumber, strCaller, strMessage);

                if (m_dbMgr.GetResultData(strSQL, DBName) == null)
                {
                    m_strErrorMessage = "[libSMS]MessageBrokerKDHCLG.SendSMSMessage Fail : " + m_dbMgr.LastErrorMessage;
                    return false;
                }
            }

            return true;
        }

        // LMS (멀티미디어 컨텐츠 미첨부)
        public bool SendLMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage)
        {
            string strTime = GetCurrentTime(m_dbMgr, DBName);
            strMessage = CheckQuotation(strMessage);

            m_strErrorMessage = "";

            foreach (string phoneNumber in phoneNumberList)
            {
                string strSQL = "INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, TYPE) ";
                strSQL += string.Format("VALUES ('{0}', '{1}', '{2}', '0', '{3}', '{4}', '0')",
                    strTitle, phoneNumber, strCaller, strTime, strMessage);

                if (m_dbMgr.GetResultData(strSQL, DBName) == null)
                {
                    m_strErrorMessage = "[libSMS]MessageBrokerKDHCLG.SendLMSMessage Fail : " + m_dbMgr.LastErrorMessage;
                    return false;
                }
            }

            return true;
        }

        // MMS (멀티미디어 컨텐츠 첨부)
        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas)
        {
            string strTime = GetCurrentTime(m_dbMgr, DBName);
            strMessage = CheckQuotation(strMessage);

            m_strErrorMessage = "";

            if (contentDatas != null && contentDatas.Count > 0)
            {
                string path = contentDatas[0].Value;
                foreach (string phoneNumber in phoneNumberList)
                {
                    string strSQL = "INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, FILE_CNT, FILE_PATH1, TYPE)";
                    strSQL += string.Format("VALUES ('{0}', '{1}', '{2}', '0', '{3}', '{4}', '1', '{5}', '0')",
                        strTitle, phoneNumber, strCaller, strTime, strMessage, path);

                    if (m_dbMgr.GetResultData(strSQL, DBName) == null)
                    {
                        m_strErrorMessage = "[libSMS]MessageBrokerKDHCLG.SendMMSMessage Fail : " + m_dbMgr.LastErrorMessage;
                        return false;
                    }
                }
            }

            return true;
        }
    }
#endif

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
            m_dbMgr.WebServerURL = "http://218.152.200.123:8010";       // 192.168.0.10 포트포워딩
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

#if BLD_200
    // LG
    internal class MessageBrokerBLD200 : BaseMessageBroker
    {
        private string DBName = "BLD_200";
        private WebDBManager m_dbMgr = null;

        public MessageBrokerBLD200(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dbMgr.WebServerURL = "http://127.0.0.1";
            m_dbMgr.DatabaseType = WebDBManager.DBType.sqlserver;
            m_dbMgr.DatabaseName = "BLD_200";            
        }
        
        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            strMessage = CheckQuotation(strMessage);
            m_strErrorMessage = "";
                        
            foreach (string phoneNumber in phoneNumberList)
            {
                StringBuilder sb = new StringBuilder();
                DateTime now = DateTime.Now;

                sb.Append("INSERT INTO SendSMS (ID, Caller, PhoneNumber, SMSMessage, time) ");
                sb.AppendFormat("VALUES ((select isnull(max(id), 0) +1 from SendSMS), '{0}', '{1}', '{2}', '{3}')"
                    , strCaller, phoneNumber, strMessage, now.ToString("yyyy-MM-dd HH:mm:ss"));

                if (m_dbMgr.GetResultData(sb.ToString(), DBName) == null)
                {
                    m_strErrorMessage = "[libSMS]MessageBrokerBLD200.SendSMSMessage Fail : " + m_dbMgr.LastErrorMessage;
                    return false;
                }
            }

            return true;
        }

        // LMS (멀티미디어 컨텐츠 미첨부)
        public bool SendLMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage)
        {
            string strTime = GetCurrentTime(m_dbMgr, DBName);
            strMessage = CheckQuotation(strMessage);

            m_strErrorMessage = "";

            foreach (string phoneNumber in phoneNumberList)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("INSERT INTO SendSMS (ID, Caller, PhoneNumber, SMSMessage, time) ");
                sb.AppendFormat("VALUES ((select isnull(max(id), 0) +1 from SendSMS), '{0}', '{1}', '{2}', '{3}')"
                    , strCaller, phoneNumber, strMessage, strTime);

                if (m_dbMgr.GetResultData(sb.ToString(), DBName) == null)
                {
                    m_strErrorMessage = "[libSMS]MessageBrokerBLD200.SendSMSMessage Fail : " + m_dbMgr.LastErrorMessage;
                    return false;
                }
            }

            return true;
        }

        // MMS (멀티미디어 컨텐츠 첨부)
        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas)
        {
            return true;
        }
    }
#endif

#if Parc1
    internal class MessageBrokerParc1 : BaseMessageBroker
    {
        private string HostIP = "192.168.95.102"; // Parc1 SI업체
        private string DBName = "SMSInfo";
        private string ID = "unesms";
        private string PW = "unesms";
        private DirectDBManager.DBType DBType = DirectDBManager.DBType.sqlserver;
        private DirectDBManager m_dbMgr = null;
        
        public MessageBrokerParc1(int nSiteID)
        {
            m_dbMgr = DirectDBManager.MakeInstance(DBType, HostIP, ID, PW, DBName);
            m_dbMgr.Connect();
        }

        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage, int nLimit)
        {
            string strTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            strMessage = CheckQuotation(strMessage);

            m_strErrorMessage = "";

            bool isSMS = ClientHelper.IsSMSMessage(strMessage, nLimit);
            string[] msgs = null;
            if (!isSMS)
            {
                // nLimit Byte씩 자르기
                Encoding ec = Encoding.GetEncoding(0);
                byte[] bytes = Encoding.Default.GetBytes(strMessage);
                int bytesLength = bytes.Length;
                int count = bytes.Length / nLimit + 1;
                msgs = new string[count];
                for (int i = 0; i < count; i++)
                {
                    int leng;
                    if (bytesLength >= nLimit)
                    {
                        leng = 80;
                        bytesLength -= nLimit;
                    }
                    else
                    {
                        leng = bytesLength;
                    }
                    char[] divideArray = new char[ec.GetCharCount(bytes, i * nLimit, leng)];
                    ec.GetChars(bytes, i * nLimit, leng, divideArray, 0);
                    msgs[i] = new string(divideArray);
                }
            }
            else
            {
                msgs = new string[1];
                msgs[0] = strMessage;
            }

            foreach (string phoneNumber in phoneNumberList)
            {
                foreach (string msg in msgs)
                {
                    string strSQL = "INSERT INTO t_SMS_Send (SMS_MSG, SMS_SendNum, SMS_RecvNum, SMS_Sender, SMS_Datetime)  ";
                    strSQL += string.Format("VALUES ('{0}', '{1}', '{2}', 1, '{3}')", msg, strCaller, phoneNumber, strTime);

                    if (m_dbMgr.GetResultData(strSQL) == null)
                    {
                        m_strErrorMessage = "[libSMS]MessageBrokerParc1.SendSMSMessage Fail : " + m_dbMgr.ErrorMessage;
                        return false;
                    } 
                }
            }

            return true;
        }

        // LMS (멀티미디어 컨텐츠 미첨부)
        public bool SendLMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage)
        {
            //string strTime = GetCurrentTime(m_dbMgr, DBName);
            //strMessage = CheckQuotation(strMessage);

            //m_strErrorMessage = "";

            //foreach (string phoneNumber in phoneNumberList)
            //{
            //    string strSQL = "INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, TYPE) ";
            //    strSQL += string.Format("VALUES ('{0}', '{1}', '{2}', '0', '{3}', '{4}', '0')",
            //        strTitle, phoneNumber, strCaller, strTime, strMessage);

            //    if (m_dbMgr.GetResultData(strSQL, DBName) == null)
            //    {
            //        m_strErrorMessage = "[libSMS]MessageBrokerParc1.SendLMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            //        return false;
            //    }
            //}

            return true;
        }

        // MMS (멀티미디어 컨텐츠 첨부)
        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas)
        {
            //string strTime = GetCurrentTime(m_dbMgr, DBName);
            //strMessage = CheckQuotation(strMessage);

            //m_strErrorMessage = "";

            //if (contentDatas != null && contentDatas.Count > 0)
            //{
            //    string path = contentDatas[0].Value;
            //    foreach (string phoneNumber in phoneNumberList)
            //    {
            //        string strSQL = "INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, FILE_CNT, FILE_PATH1, TYPE)";
            //        strSQL += string.Format("VALUES ('{0}', '{1}', '{2}', '0', '{3}', '{4}', '1', '{5}', '0')",
            //            strTitle, phoneNumber, strCaller, strTime, strMessage, path);

            //        if (m_dbMgr.GetResultData(strSQL, DBName) == null)
            //        {
            //            m_strErrorMessage = "[libSMS]MessageBrokerParc1.SendMMSMessage Fail : " + m_dbMgr.LastErrorMessage;
            //            return false;
            //        }
            //    }
            //}

            return true;
        }
    }
#endif

#if Urbanbrix
    // LG
    internal class MessageBrokerUrbanbrix : BaseMessageBroker
    {
        private string DBName = "BLD_205_SMS";
        private WebDBManager.DBType DBType = WebDBManager.DBType.sqlserver;
        private WebDBManager m_dbMgr = null;

        public MessageBrokerUrbanbrix(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
            m_dbMgr.DatabaseName = DBName;
            m_dbMgr.WebServerURL = "http://127.0.0.1";
            m_dbMgr.DatabaseType = DBType;
        }

        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            //string strTime = GetCurrentTime(m_dbMgr, DBName);
            strMessage = CheckQuotation(strMessage);

            m_strErrorMessage = "";

            foreach (string phoneNumber in phoneNumberList)
            {
                string strSQL = "INSERT INTO SC_TRAN (TR_SENDDATE, TR_SENDSTAT, TR_MSGTYPE, TR_PHONE, TR_CALLBACK, TR_MSG)  ";
                strSQL += string.Format("VALUES (GetDate(), '0', '0', '{0}', '{1}', '{2}')", phoneNumber, strCaller, strMessage);

                if (m_dbMgr.GetResultData(strSQL, DBName) == null)
                {
                    m_strErrorMessage = "[libSMS]MessageBrokerUrbanbrix.SendSMSMessage Fail : " + m_dbMgr.LastErrorMessage;
                    return false;
                }
            }

            return true;
        }

        // LMS (멀티미디어 컨텐츠 미첨부)
        public bool SendLMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage)
        {
            string strTime = GetCurrentTime(m_dbMgr, DBName);
            strMessage = CheckQuotation(strMessage);

            m_strErrorMessage = "";

            foreach (string phoneNumber in phoneNumberList)
            {
                string strSQL = "INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, TYPE) ";
                strSQL += string.Format("VALUES ('{0}', '{1}', '{2}', '0', '{3}', '{4}', '0')",
                    strTitle, phoneNumber, strCaller, strTime, strMessage);

                if (m_dbMgr.GetResultData(strSQL, DBName) == null)
                {
                    m_strErrorMessage = "[libSMS]MessageBrokerUrbanbrix.SendLMSMessage Fail : " + m_dbMgr.LastErrorMessage;
                    return false;
                }
            }

            return true;
        }

        // MMS (멀티미디어 컨텐츠 첨부)
        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas)
        {
            string strTime = GetCurrentTime(m_dbMgr, DBName);
            strMessage = CheckQuotation(strMessage);

            m_strErrorMessage = "";

            if (contentDatas != null && contentDatas.Count > 0)
            {
                string path = contentDatas[0].Value;
                foreach (string phoneNumber in phoneNumberList)
                {
                    string strSQL = "INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, FILE_CNT, FILE_PATH1, TYPE)";
                    strSQL += string.Format("VALUES ('{0}', '{1}', '{2}', '0', '{3}', '{4}', '1', '{5}', '0')",
                        strTitle, phoneNumber, strCaller, strTime, strMessage, path);

                    if (m_dbMgr.GetResultData(strSQL, DBName) == null)
                    {
                        m_strErrorMessage = "[libSMS]MessageBrokerUrbanbrix.SendMMSMessage Fail : " + m_dbMgr.LastErrorMessage;
                        return false;
                    }
                }
            }

            return true;
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

#if Kakao
    // KT 메시지 서비스 제공업체(모노커뮤니케이션즈)의 라이브러리 직접 사용하는 버전
    internal class MessageBrokerKakao : BaseMessageBroker
    {
        private WebDBManager m_dbMgr = null;
        private string m_strFrontURL = "https://www.biztalk-api.com";
        private string m_strToken = "";
        
        public MessageBrokerKakao(int nSiteID)
        {
            m_dbMgr = new WebDBManager(nSiteID);
        }

        public bool SendSMSMessage(List<string> phoneNumberList, int nSensorReactionHistoryID)
        {
            m_strErrorMessage = "";

            //string strTime = GetCurrentTime(m_dbMgr, m_dbMgr.DatabaseName);

            string strTmpltCode = "";
            string strTitle = "";
            string strMessage = MakeMessage(nSensorReactionHistoryID, ref strTmpltCode, ref strTitle);
            if (strTmpltCode.Length == 0 || strMessage.Length == 0)
            {
                m_strErrorMessage = "메시지 만들 수 없음";
                return false;
            }

            string countryCode = "";
            string senderKey = "";
            string bsId = "";
            string bsPasswd = "";
            GetKakaoInfo(ref countryCode, ref senderKey, ref bsId, ref bsPasswd);
            if (countryCode.Length == 0 || senderKey.Length == 0 || bsId.Length == 0 || bsPasswd.Length == 0)
            {
                m_strErrorMessage = "KakaoInfo Table 정보 없음";
                return false;
            }

            GetToken(bsId, bsPasswd);
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
                mgr.Add("countryCode", countryCode);
                mgr.Add("recipient", phoneNumberList[i]);
                mgr.Add("senderKey", senderKey);
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
                    m_strErrorMessage = "[libSMS]MessageBrokerMCS.SendSMSMessage Fail : " + ex.Message;
                } 
            }

            return false;
        }
        private void GetKakaoInfo(ref string countryCode, ref string senderKey, ref string bsId, ref string bsPasswd)
        {
            ArrayList arrResult = m_dbMgr.GetResultData("Select CountryCode, SenderKey, BsID, BsPasswd From OptionKakaoInfo");
            if (arrResult == null || arrResult.Count != 4)
                return;

            countryCode = DBUtility2.WebDBManager.GetStringField(arrResult[0]);
            senderKey = DBUtility2.WebDBManager.GetStringField(arrResult[1]);
            bsId = DBUtility2.WebDBManager.GetStringField(arrResult[2]);
            bsPasswd = DBUtility2.WebDBManager.GetStringField(arrResult[3]);
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
            
            StringBuilder sb = new StringBuilder();
            sb.Append("Select srh.ReactionType, Time, ZoneName, sz.Type, Message ");
            sb.Append("  From SensorReactionHistory as srh, EquipmentZone as eq, SensorZone as sz ");
            sb.Append(" Where srh.Param1 = eq.ID ");
            sb.Append("   And sz.ID = srh.Param2 ");
            sb.Append(" ");
            sb.Append("   And srh.ReactionType in (0, 21, 50) ");
            sb.AppendFormat(" And srh.ID = {0} ", nSensorReactionHistoryID);

            ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString());
            if (arrResult == null || arrResult.Count != 5)
                return "";

            int nReactionType = DBUtility2.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            DateTime dtDateTime = DBUtility2.WebDBManager.GetDateTimeField(arrResult[1].ToString(), DateTime.Now);
            string strZoneName = DBUtility2.WebDBManager.GetStringField(arrResult[2]);
            int nFacilityType = DBUtility2.WebDBManager.GetIntField(arrResult[3].ToString(), -1);
            string strMessage = DBUtility2.WebDBManager.GetStringField(arrResult[4]);

            string varFacilityType = "";
            string varDateTime = dtDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string varTest = strMessage.Contains("[테스트]") ? "[테스트]" : "";
            string varBuilding = strZoneName;
            string varFloor = "";

            if (nFacilityType == 0)
                varFacilityType = "화재";
            if (nFacilityType == 11)
                varFacilityType = "누출";
            if (nFacilityType == 17)
                varFacilityType = "정전";
            if (nFacilityType == 18)
                varFacilityType = "강풍";
            if (nFacilityType == 19)
                varFacilityType = "침수";
            if (nFacilityType == 20)
                varFacilityType = "테러";
            if (nFacilityType == 50)
                varFacilityType = "지진";

            strTitle = varFacilityType + " 알람 ";

            if (nReactionType == 0) // 알람 탐지
            {
                strTmpltCode = "alarm_detect";
                strTitle += "탐지";
                returnMessage = string.Format("SOP 시스템 {0} 알람 탐지\n{1}\n{2}[{3}]에서 {0} 신호가 탐지되었습니다.", varFacilityType, varDateTime, varTest, varBuilding);
            }
            else if (nReactionType == 21) // 알람 오작동
            {
                strTmpltCode = "alarm_malfunction";
                strTitle += "오작동";
                returnMessage = string.Format("SOP 시스템 {0} 알람 오작동\n{1}\n{2}[{3}]에서 탐지된 {0} 신호가 오작동으로 신고되었습니다.", varFacilityType, varDateTime, varTest, varBuilding);
            }
            else if (nReactionType == 50) // 알람 복구
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
}
