using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace MSMQTest
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
        public string MsgCharSetFrom
        {
            get { return m_szMsgCharSet; }
            set { m_szMsgCharSet = value; }
        }

        private string m_szMsgCharSet2 = "UTF-8";
        public string MsgCharSetTo
        {
            get { return m_szMsgCharSet2; }
            set { m_szMsgCharSet2 = value; }
        }
        

        private string m_szCharSet = "UTF8";
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
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }


        private log4net.ILog logger = null;

        private void LoadConnectionInfo()
        {
            string strSection = "Message Server Info";

            m_szServerIP = RegUtil.ReadRegValue(strSection, "MessageDBServerIP");
            if (m_szServerIP == null || m_szServerIP == "")
            {
                m_szServerIP = "10.131.5.6";
                RegUtil.WriteRegValue(strSection, "MessageDBServerIP", m_szServerIP);
            }

            dbName = RegUtil.ReadRegValue(strSection, "MessageDBName");
            if (dbName == null || dbName == "")
            {
                dbName = "pamts_sms0";
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
                m_szCharSet = "UTF8";
                RegUtil.WriteRegValue(strSection, "MessageDBCharSet", m_szCharSet);
            }

            m_szMsgCharSet = RegUtil.ReadRegValue(strSection, "MessageCharSet");
            if (m_szMsgCharSet == null || m_szMsgCharSet == "")
            {
                m_szMsgCharSet = "UTF-8";
                RegUtil.WriteRegValue(strSection, "MessageCharSet", m_szCharSet);
            }

            m_szMsgCharSet2 = RegUtil.ReadRegValue(strSection, "MessageCharSet");
            if (m_szMsgCharSet2 == null || m_szMsgCharSet2 == "")
            {
                m_szMsgCharSet2 = "UTF-8";
                RegUtil.WriteRegValue(strSection, "MessageCharSetTo", m_szCharSet);
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
            RegUtil.WriteRegValue(strSection, "MessageCharSetTo", m_szMsgCharSet2);
        }

        public bool Connect()
        {
            string strConn = string.Format("Server={0};Database={1};Uid={2};Pwd={3};charset={4}", m_szServerIP, dbName, id, pw, m_szCharSet);

            try
            {
                m_DBConn = new MySqlConnection(strConn);
                m_DBConn.Open();

                m_bConnect = true;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
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
                m_DBConn = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }

            m_bConnect = false;

        }
        private string sid = "05140998";
        public bool InsertMessage(MessageContent content)
        {
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
            Encoding enc2 = Encoding.GetEncoding(m_szMsgCharSet2);
            byte[] bytes1 = enc.GetBytes(strMsg);
            byte[] bytes2 = Encoding.Convert(enc, enc2, bytes1);
            string szMsg = enc2.GetString(bytes2);
            string szTime = DateTime.Now.ToString();

            //string strSQL = string.Format("insert into log_sms (CLASS,CLIENT,WRITE_TIME,DESTINATION,CALLBACK,BODY,SEND_FLAG) values('TMS','SP',now(),'{0}','{1}','{2}','1')", strReciver, strSender, szMsg);
            string strSQL = string.Format("insert into LOG_SMS (user_id, write_time,destination,callback,body,send_flag, del_flag) values('{0}',now(),'{1}','{2}','{3}','1','N')", sid, strReciver, strSender, szMsg);
            logger.Debug(strSQL);
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(strSQL, m_DBConn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);

                Close();
                return false;
            }
            return true;
        }

        public bool InsertMessage(List<MessageContent> arList)
        {
            if (m_bConnect == false)
            {
                Connect();
            }

            if (m_DBConn == null || m_DBConn.State == System.Data.ConnectionState.Closed || m_DBConn.State == System.Data.ConnectionState.Broken)
            {
                m_bConnect = false;
                Connect();
            }

            MySqlTransaction tranc = null;
            try
            {
                tranc = m_DBConn.BeginTransaction();

                foreach (MessageContent content in arList)
                {
                    string strReciver = content.Reciver;
                    string strSender = content.Caller;
                    string strMsg = content.Message;

                    Encoding enc = Encoding.GetEncoding(m_szMsgCharSet);
                    Encoding enc2 = Encoding.GetEncoding(m_szMsgCharSet2);
                    byte[] bytes1 = enc.GetBytes(strMsg);
                    byte[] bytes2 = Encoding.Convert(enc, enc2, bytes1);
                    string szMsg = enc2.GetString(bytes2);


                    //string strSQL = string.Format("insert into log_sms(CLASS,CLIENT,WRITE_TIME,DESTINATION,CALLBACK,BODY,SEND_FLAG) values('TMS','SP',now(),'{0}','{1}','{2}','1')", strReciver, strSender, szMsg);
                    string strSQL = string.Format("insert into LOG_SMS (user_id, write_time,destination,callback,body,send_flag, del_flag) values('{0}',now(),'{1}','{2}','{3}','1','N')", sid, strReciver, strSender, szMsg);
                    logger.Debug(strSQL);
                    if (strMsg == "" || strReciver == "" || strSender == "")
                    {
                        //MessageService.Logger.Debug("SendFail : " + content.ToString());
                        continue;
                    }

                    using (MySqlCommand cmd = new MySqlCommand(strSQL, m_DBConn, tranc))
                    {
                        int nTest = cmd.ExecuteNonQuery();
                        if( nTest <= 0)
                        {
                            
                        }
                    }
                }
                tranc.Commit();
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
                catch(Exception exx)
                {
                    System.Diagnostics.Trace.WriteLine(exx.Message);
                    System.Diagnostics.Trace.WriteLine(exx.StackTrace);
                }
                
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);

                Close();
                return false;
            }
            return true;
        }

        public string CheckCharset()
        {
            if (m_bConnect == false)
            {
                Connect();
            }

            if (m_DBConn == null || m_DBConn.State == System.Data.ConnectionState.Closed || m_DBConn.State == System.Data.ConnectionState.Broken)
            {
                m_bConnect = false;
                Connect();
            }
            string szResult = "";
            try
            {
                
                string strSQL = string.Format("SHOW FULL COLUMNS FROM log_sms");
                using (MySqlCommand cmd = new MySqlCommand(strSQL, m_DBConn))
                {
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if(!reader.IsDBNull(i))
                            {
                                string szValue = reader.GetString(i);
                                if (szValue != null)
                                {
                                    szResult += szValue;
                                    szResult += "  ";
                                }
                            }
                            else
                            {
                                szResult += "NULL";
                                szResult += "  ";
                            }
                        }
                        szResult += "\n\r";
                    }
                    return szResult;
                }
           
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);

                Close();
                
            }
            return szResult;
        }
    }
}
