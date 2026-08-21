using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MessageServer
{
    public class SMSDBManager
    {

        private static SMSDBManager m_Instance = null;

        public static SMSDBManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new SMSDBManager();

                return m_Instance;
            }
        }

        private bool m_bConnect = false;
        public bool IsConnect
        {
            get { return m_bConnect; }
            set { m_bConnect = value; }
        }

        private string m_ConString = "";

        private string m_szServerIP = "10.131.5.6";
        //private string m_szServerIP = "192.168.0.210";
        public string ServerIP
        {
            get { return m_szServerIP; }
            set { m_szServerIP = value; }
        }

        private string m_szPort = "";
        public string Port
        {
            get { return m_szPort; }
            set { m_szPort = value; }
        }

        private string m_szMsgCharSet = "UTF-8";
        public string MsgCharSet
        {
            get { return m_szMsgCharSet; }
            set { m_szMsgCharSet = value; }
        }

        private string m_szCharSet = "utf8";
        public string CharSet
        {
            get { return m_szCharSet; }
            set { m_szCharSet = value; }
        }

        //private string driver = "com.mysql.jdbc.Driver";
        //private string url = "jdbc:mysql://127.0.0.1:3306/pamts_sms2?useUnicode=true&characterEncoding=UTF8";
        private string id = "smsuser";
        private string pw = "smsnd";
        private string dbName = "pamts_sms2";

        private MySqlConnection m_DBConn = null;

        private SMSDBManager()
        {
            LoadConnectionInfo();
        }

        private string sid = "05140998";

        private void LoadConnectionInfo()
        {
            string strSection = "Message Server Info";

            m_szServerIP = RegUtil.ReadRegValue(strSection, "MessageDBServerIP");
            MessageService.Logger.Debug(m_szServerIP);
            if (m_szServerIP == null || m_szServerIP == "")
            {
                m_szServerIP = "10.131.5.6";
                RegUtil.WriteRegValue(strSection, "MessageDBServerIP", m_szServerIP);
            }
            MessageService.Logger.Debug(m_szServerIP);
            dbName = RegUtil.ReadRegValue(strSection, "MessageDBName");
            if (dbName == null || dbName == "")
            {
                dbName = "pamts_sms2";
                RegUtil.WriteRegValue(strSection, "MessageDBName", dbName);
            }

            id = RegUtil.ReadRegValue(strSection, "MessageDBUser");
            if (id == null || id == "")
            {
                id = "smsuser";
                RegUtil.WriteRegValue(strSection, "MessageDBUser", id);
            }

            //pw = RegUtil.ReadRegValue(strSection, "MessageDBPass");
            //if (id == null || id == "")
            //{
            //    id = "smsnd";
            //    RegUtil.WriteRegValue(strSection, "MessageDBPass", id);
            //}

            m_szCharSet = RegUtil.ReadRegValue(strSection, "MessageDBCharSet");
            if (m_szCharSet == null || m_szCharSet == "")
            {
                m_szCharSet = "utf8";
                RegUtil.WriteRegValue(strSection, "MessageDBCharSet", m_szCharSet);
            }

            m_szMsgCharSet = RegUtil.ReadRegValue(strSection, "MessageCharSet");
            if (m_szMsgCharSet == null || m_szMsgCharSet == "")
            {
                m_szMsgCharSet = "UTF-8";
                RegUtil.WriteRegValue(strSection, "MessageCharSet", m_szMsgCharSet);
            }
        }

        public void SaveConnectionInfo()
        {
            string strSection = "Message Server Info";

            RegUtil.WriteRegValue(strSection, "MessageDBServerIP", m_szServerIP);
            RegUtil.WriteRegValue(strSection, "MessageDBName", dbName);
            RegUtil.WriteRegValue(strSection, "MessageDBUser", id);
            RegUtil.WriteRegValue(strSection, "MessageDBCharSet", m_szCharSet);
            RegUtil.WriteRegValue(strSection, "MessageCharSet", m_szMsgCharSet);
        }

        public bool Connect()
        {
            string strConn = string.Format("Server={0};Database={1};Uid={2};Pwd={3};Charset={4}", m_szServerIP, dbName, id, pw, m_szCharSet);
            
            try
            {
                m_DBConn = new MySqlConnection(strConn);
                m_DBConn.Open();

                m_bConnect = true;

            }
            catch (Exception ex)
            {
                MessageService.Logger.Debug(strConn);
                MessageService.Logger.Debug(ex.Message);
                MessageService.Logger.Debug(ex.StackTrace);
                m_bConnect = false;
            }

            return m_bConnect;
        }

        public void Close()
        {
            try
            {
                if (m_DBConn != null)
                {
                    m_DBConn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageService.Logger.Debug(ex.Message);
                MessageService.Logger.Debug(ex.StackTrace);
            }

            m_bConnect = false;

        }

        public bool InsertMessage(MessageContent content)
        {
            if (content == null)
                return true;

            string strReciver = content.Reciver;
            string strSender = content.Caller;
            string strMsg = content.Message;

            if (strMsg == "" || strReciver == "" || strSender == "")
            {
                return false;
            }

            if (m_bConnect == false)
            {
                Connect();
            }

            if (m_DBConn == null || m_DBConn.State == System.Data.ConnectionState.Closed || m_DBConn.State == System.Data.ConnectionState.Broken)
            {
                m_bConnect = false;
                Connect();
            }

            Encoding enc = Encoding.GetEncoding(m_szMsgCharSet);
            byte[] bytes1 = Encoding.UTF8.GetBytes(strMsg);
            byte[] bytes2 = Encoding.Convert(Encoding.UTF8, enc, bytes1);
            string szMsg = enc.GetString(bytes2);

            MessageService.Logger.Debug(szMsg);

            string strSQL = string.Format("insert into LOG_SMS (user_id, write_time,destination,callback,body,send_flag, del_flag) values('{0}',now(),'{1}','{2}','{3}','1','N')",sid, strReciver, strSender, szMsg);
            //string strSQL = string.Format("insert into LOG_SMS (CLASS,CLIENT,WRITE_TIME,DESTINATION,CALLBACK,BODY,SEND_FLAG) values('TMS','SP',now(),'{0}','{1}','{2}','1')", strReciver, strSender, szMsg);

            try
            {
                using (MySqlCommand cmd = new MySqlCommand(strSQL, m_DBConn))
                {
                    cmd.ExecuteNonQuery();
                }
                m_bConnect = false;
                Close();
               
            }
            catch (Exception ex)
            {
                MessageService.Logger.Debug(ex.Message);
                MessageService.Logger.Debug(ex.StackTrace);
                m_bConnect = false;
                Close();
                return false;
            }
            return true;
        }

        public bool InsertMessage(List<MessageContent> arList)
        {
            if (arList == null || arList.Count == 0)
                return true;

            if (m_bConnect == false)
            {
                Connect();
            }

            if (m_DBConn == null || m_DBConn.State == System.Data.ConnectionState.Closed || m_DBConn.State == System.Data.ConnectionState.Broken)
            {
                m_bConnect = false;
                Connect();
            }

          
            string prevTimeTag = "";
            MySqlTransaction tranc = null;
            try
            {
                tranc = m_DBConn.BeginTransaction();

                foreach (MessageContent content in arList)
                {
                    string strReciver = content.Reciver;
                    string strSender = content.Caller;
                    string strMsg = content.Message;

                    string szTimeTag = content.SmsTag;
                    prevTimeTag = szTimeTag;

                    Encoding enc = Encoding.GetEncoding(m_szMsgCharSet);
                    byte[] bytes1 = Encoding.UTF8.GetBytes(strMsg);
                    byte[] bytes2 = Encoding.Convert(Encoding.UTF8, enc, bytes1);
                    string szMsg = enc.GetString(bytes2);

                    string strSQL = string.Format("insert into LOG_SMS (user_id, write_time,destination,callback,body,send_flag, del_flag) values('{0}',now(),'{1}','{2}','{3}','1','N')", sid, strReciver, strSender, szMsg);
                    MessageService.Logger.Debug(strSQL);
                    //string strSQL = string.Format("insert into LOG_SMS (CLASS,CLIENT,WRITE_TIME,DESTINATION,CALLBACK,BODY,SEND_FLAG) values('TMS','SP',now(),'{0}','{1}','{2}','1')", strReciver, strSender, szMsg);
                      
                    if (strMsg == "" || strReciver == "" || strSender == "")
                    {
                        MessageService.Logger.Debug("SendFail : " + content.ToString());
                        continue;
                    }

                    using (MySqlCommand cmd = new MySqlCommand(strSQL, m_DBConn, tranc))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                tranc.Commit();

                m_bConnect = false;
                Close();                
            }
            catch (Exception ex)
            {
                try
                {
                    if (tranc != null)
                    {

                        tranc.Rollback();
                    }
                }
                catch (Exception exx)
                {
                    System.Diagnostics.Trace.WriteLine(exx.Message);
                    System.Diagnostics.Trace.WriteLine(exx.StackTrace);
                }
                
                MessageService.Logger.Debug(ex.Message);
                MessageService.Logger.Debug(ex.StackTrace);

                Close();

                m_bConnect = false;
                return false;
            }
            return true;
        }
    }
}
