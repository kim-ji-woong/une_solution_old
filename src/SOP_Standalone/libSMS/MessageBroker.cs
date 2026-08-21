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
        string m_strUserID = "unes5588";
        string m_strPassword = "ue94499660";
        string m_strCaller = "027144133";

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

            message.SetAccountInfo(m_strUserID, m_strPassword);

            ezSMSComponent.Receivers receivers = (ezSMSComponent.Receivers)message.CreateReceivers();
            receivers.AddDirect(conetnt.Reciver, conetnt.Message, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);

            ezSMSComponent.Contents contents = message.CreateContents();
            contents.Type = ezSMSComponent.EZSMS_MESSAGE_TYPE.EZSMS_MESSAGE_TYPE_SMS;
            contents.Message = conetnt.Message;
            contents.Subject = "";

            ezSMSComponent.ISendResults results = message.Send(m_strCaller, (ezSMSComponent.IReceivers)receivers, contents);
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

    internal class MessageBrokerEzSMS
    {
        string m_strUserID = "";
        string m_strPassword = "";
        string m_strCaller = "";

        dynamic message = null;

        internal MessageBrokerEzSMS(string strUserID, string strPassword, string strCaller)
        {
            m_strUserID = strUserID;
            m_strPassword = strPassword;
            m_strCaller = strCaller;

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

            message.SetAccountInfo(m_strUserID, m_strPassword);

            ezSMSComponent.Receivers receivers = (ezSMSComponent.Receivers)message.CreateReceivers();
            receivers.AddDirect(conetnt.Reciver, conetnt.Message, ezSMSComponent.EZSMS_SENDMODE.EZSMS_DIRECT, DateTime.Now);

            ezSMSComponent.Contents contents = message.CreateContents();
            contents.Type = ezSMSComponent.EZSMS_MESSAGE_TYPE.EZSMS_MESSAGE_TYPE_SMS;
            contents.Message = conetnt.Message;
            contents.Subject = "";

            ezSMSComponent.ISendResults results = message.Send(m_strCaller, (ezSMSComponent.IReceivers)receivers, contents);
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
    
    // LGU+
    internal class MessageBrokerLGU
    {
        private DBManager m_dbMgr = null;
        private string m_strCaller = "";
        private string m_strUserID = "";

        public MessageBrokerLGU(string strServerIP, WebDBManager.DBType dbType, string strDBName, string strDBID, string strPassword, string strUserID, string strCaller)
        {
            m_dbMgr = new DBManager(dbType, strServerIP, strDBName, strDBID, strPassword);
            m_strUserID = strUserID;
            m_strCaller = strCaller;
        }

        // strMessage에 작은 따옴표가 들어있는지 검사한다.
        private string CheckQuotation(string strMessage)
        {
            return strMessage.Replace("'", "''");
        }

        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = m_dbMgr.GetCurrentTime(false);

            if (strTime == null)
                return false;

            //string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);
            bool result = true;
            
            foreach (string phoneNumber in phoneNumberList)
            {
                string strSQL = "INSERT INTO SC_TRAN (TR_SENDDATE, TR_SENDSTAT, TR_MSGTYPE, TR_PHONE, TR_CALLBACK, TR_MSG)  ";
                strSQL += string.Format("VALUES ('{0}', '0', '0', '{1}', '{2}', '{3}')", strTime, phoneNumber, strCaller, strMessage);

                if (!m_dbMgr.RunQuery(strSQL, false))
                    result = false;
            }

            m_dbMgr.CloseConnection();
            return result;
        }

        // LMS (멀티미디어 컨텐츠 미첨부)
        public bool SendLMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage)
        {             
            //INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, TYPE)
            //VALUES ('[차세대MMS 전송테스트]', '수신 번호', '발신 번호', '0', NOW(), 'MESSAGE', '0');

            string strTime = m_dbMgr.GetCurrentTime(false);

            if (strTime == null)
                return false;
            //string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            foreach (string phoneNumber in phoneNumberList)
            {
                string strSQL = "INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, TYPE) ";
                strSQL += string.Format("VALUES ('{0}', '{1}', '{2}', '0', '{3}', '{4}', '0')",
                    strTitle, phoneNumber, strCaller, strTime, strMessage);

                bool result = m_dbMgr.RunQuery(strSQL, false);
                m_dbMgr.CloseConnection();
                return result;
            }

            m_dbMgr.CloseConnection();
            return true;
        }

        // MMS (멀티미디어 컨텐츠 첨부)
        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strTitle, string strMessage, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas)
        {    
            //INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, FILE_CNT, FILE_PATH1, TYPE) 
            //VALUES ('[차세대MMS 전송테스트]', '수신 번호', '발신 번호', '0', NOW(), 'MESSAGE', '1', 'D:\\UPLUSAGT\\image\\test.jpg', '0');

            string strTime = m_dbMgr.GetCurrentTime(false);

            if (strTime == null)
                return false;

            //string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            bool result = true;

            if (contentDatas != null && contentDatas.Count > 0)
            {
                string path = contentDatas[0].Value;
                foreach (string phoneNumber in phoneNumberList)
                {
                    string strSQL1 = "INSERT INTO MMS_MSG (SUBJECT, PHONE, CALLBACK, STATUS, REQDATE, MSG, FILE_CNT, FILE_PATH1, TYPE)";
                    strSQL1 += string.Format("VALUES ('{0}', '{1}', '{2}', '0', '{3}', '{4}', '1', '{5}', '0')",
                        strTitle, phoneNumber, strCaller, strTime, strMessage, path);

                    if (!m_dbMgr.RunQuery(strSQL1))
                        result = false;
                }
            }

            m_dbMgr.CloseConnection();
            return result;
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
        private DBManager m_dbMgr = null;
        // 모노커뮤니케이션즈의 MCS 서비스는 사전에 등록된 전화번호만 발신번호로 사용할 수 있음
        private string m_strCaller = "";
        private string m_strUserID = "";

        public MessageBrokerMCS(string strServerIP, WebDBManager.DBType dbType, string strDBName, string strDBID, string strPassword, string strUserID, string strCaller)
        {
            m_dbMgr = new DBManager(dbType, strServerIP, strDBName, strDBID, strPassword);
            m_strUserID = strUserID;
            m_strCaller = strCaller;
        }

        // strMessage에 작은 따옴표가 들어있는지 검사한다.
        private string CheckQuotation(string strMessage)
        {
            return strMessage.Replace("'", "''");
        }

        public bool SendSMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = m_dbMgr.GetCurrentTime(false);

            if (strTime == null)
                return false;

            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_SMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, SMS_MSG, CALLBACK_URL, NOW_DATE, SEND_DATE, CALLBACK, DEST_TYPE, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, STD_ID) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', NULL, NULL, '{2}', '{3}', 0, {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            bool result = m_dbMgr.RunQuery(strSQL, false);
            m_dbMgr.CloseConnection();
            return result;
        }

        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strMessage)
        {
            string strTime = m_dbMgr.GetCurrentTime(false);

            if (strTime == null)
                return false;

            string strReceiverInfo = GetReceiverInfo(phoneNumberList);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, MMS_MSG, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, MSG_TYPE, STD_ID, CONTENT_COUNT, CONTENT_DATA) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', '{2}', '{2}', '{3}', {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL, 0, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            bool result = m_dbMgr.RunQuery(strSQL, false);
            m_dbMgr.CloseConnection();
            return result;
        }

        public bool SendMMSMessage(string strCaller, List<string> phoneNumberList, string strMessage, string strTitle, List<KeyValuePair<MessageContentMMS.ContentType, string>> contentDatas)
        {
            string strTime = m_dbMgr.GetCurrentTime(false);

            if (strTime == null)
                return false;

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

            bool result = m_dbMgr.RunQuery(strSQL, false);
            m_dbMgr.CloseConnection();
            return result;
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

    internal sealed class DBManager
    {
        private DirectDBManager m_dbMgr = null;
        
        private class SQLServerManager : DirectDBManager
        {
            private System.Data.SqlClient.SqlConnection m_connection = null;

            public SQLServerManager(string strServerIP, string strDBName, string strID, string strPassword)
                : base(strServerIP, strDBName, strID, strPassword)
            {
            }

            private System.Data.SqlClient.SqlConnection Connect()
            {
                string strConnection = string.Format("Data Source={0};Initial Catalog={1};User id={2};Password={3};",
                    m_strServerIP, m_strDBName, m_strID, m_strPassword);

                try
                {
                    System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(strConnection);
                    connection.Open();

                    if (connection.State != System.Data.ConnectionState.Open)
                        return null;

                    return connection;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                return null;
            }

            public override bool RunQuery(string strSQL, bool newConnection)
            {
                System.Data.SqlClient.SqlConnection connection = newConnection ? Connect() : m_connection;

                if (connection == null)
                    return false;

                try
                {
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(strSQL, connection);
                    int nResult = command.ExecuteNonQuery();

                    if (newConnection)
                        connection.Close();

                    return nResult >= 0;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                return false;
            }

            public override string GetCurrentTime(bool closeConnection)
            {
                System.Data.SqlClient.SqlConnection connection = Connect();

                if (connection == null)
                    return null;

                try
                {
                    System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand("Select GetDate()", connection);
                    System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader();

                    if (reader == null)
                    {
                        connection.Close();
                        return null;
                    }

                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            DateTime time = reader.GetDateTime(0);

                            reader.Close();

                            if (closeConnection)
                                connection.Close();
                            else
                                m_connection = connection;

                            return GetDateTimeString(time);
                        }
                    }

                    reader.Close();

                    if (closeConnection)
                        connection.Close();
                    else
                        m_connection = connection;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                return null;
            }

            public override void CloseConnection()
            {
                if (m_connection != null)
                {
                    try
                    {
                        m_connection.Close();
                        m_connection = null;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }
                }
            }
        }

        private class MySQLManager : DirectDBManager
        {
            private MySql.Data.MySqlClient.MySqlConnection m_connection = null;

            public MySQLManager(string strServerIP, string strDBName, string strID, string strPassword)
                : base(strServerIP, strDBName, strID, strPassword)
            {
            }

            private MySql.Data.MySqlClient.MySqlConnection Connect()
            {
                string strConnection = string.Format("Server={0};Database={1};Uid={2};Pwd={3};",
                    m_strServerIP, m_strDBName, m_strID, m_strPassword);

                try
                {
                    MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(strConnection);
                    connection.Open();

                    if (connection.State != System.Data.ConnectionState.Open)
                        return null;

                    return connection;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                return null;
            }

            public override bool RunQuery(string strSQL, bool newConnection)
            {
                MySql.Data.MySqlClient.MySqlConnection connection = newConnection ? Connect() : m_connection;

                if (connection == null)
                    return false;

                try
                {
                    MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(strSQL, connection);
                    int nResult = command.ExecuteNonQuery();

                    if (newConnection)
                        connection.Close();

                    return nResult >= 0;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                return false;
            }

            public override string GetCurrentTime(bool closeConnection)
            {
                MySql.Data.MySqlClient.MySqlConnection connection = Connect();

                if (connection == null)
                    return null;

                try
                {
                    MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand("SELECT current_date(), current_time()", connection);
                    MySql.Data.MySqlClient.MySqlDataReader reader = command.ExecuteReader();

                    if (reader == null)
                    {
                        connection.Close();
                        return null;
                    }

                    if (reader.HasRows)
                    {
                        if (reader.Read())
                        {
                            string strDate = reader[0].ToString();
                            string strTime = strDate + reader[1].ToString();

                            strTime = strTime.Replace("-", "");
                            strTime = strTime.Replace(":", "");
                            strTime = strTime.Replace(" ", "");

                            int nIndex = strTime.LastIndexOf('.');

                            if (nIndex >= 0)
                                strTime = strTime.Substring(0, nIndex);

                            reader.Close();

                            if (closeConnection)
                                connection.Close();
                            else
                                m_connection = connection;

                            return strTime;
                        }
                    }

                    reader.Close();

                    if (closeConnection)
                        connection.Close();
                    else
                        m_connection = connection;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                return null;
            }

            public override void CloseConnection()
            {
                if (m_connection != null)
                {
                    try
                    {
                        m_connection.Close();
                        m_connection = null;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }
                }
            }
        }

        public DBManager(WebDBManager.DBType dbType, string strServerIP, string strDBName, string strID, string strPassword)
        {
            if (dbType == WebDBManager.DBType.sqlserver)
                m_dbMgr = new SQLServerManager(strServerIP, strDBName, strID, strPassword);
            else if (dbType == WebDBManager.DBType.mysql)
                m_dbMgr = new MySQLManager(strServerIP, strDBName, strID, strPassword);
        }

        public bool RunQuery(string strSQL, bool newConnection = true)
        {
            if (m_dbMgr == null)
                return false;

            return m_dbMgr.RunQuery(strSQL, newConnection);
        }

        public string GetCurrentTime(bool closeConnection = true)
        {
            if (m_dbMgr == null)
                return null;

            return m_dbMgr.GetCurrentTime(closeConnection);
        }

        public void CloseConnection()
        {
            m_dbMgr.CloseConnection();
        }
    }

    internal abstract class DirectDBManager
    {
        protected string m_strServerIP = "";
        protected string m_strDBName = "";
        protected string m_strID = "";
        protected string m_strPassword = "";

        public DirectDBManager(string strServerIP, string strDBName, string strID, string strPassword)
        {
            m_strServerIP = strServerIP;
            m_strDBName = strDBName;
            m_strID = strID;
            m_strPassword = strPassword;
        }

        protected string GetDateTimeString(DateTime time)
        {
            return string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }

        public abstract bool RunQuery(string strSQL, bool newConnection);
        public abstract string GetCurrentTime(bool closeConnection);
        public abstract void CloseConnection();
    }
}
