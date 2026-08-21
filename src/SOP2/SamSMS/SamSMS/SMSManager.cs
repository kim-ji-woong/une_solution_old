using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Collections;

namespace SamSMS
{
    public class SMSManager
    {
        private string m_strID = "";
        private string m_strPW = "";
        private MySqlConnection m_dbConnection = null;
        private bool m_isConnection = false;

        private static SMSManager m_instace = null;

        public static SMSManager Instance
        {
            get
            {
                if (m_instace == null)
                    m_instace = new SMSManager();
                return m_instace;
            }
        }

        private SMSManager()
        {
            //if (FormMain.Instance.SMSOn)
                MakeConnection();
        }

        protected void MakeConnection()
        {
            char[] arrID = new char[] { 'r', 'o', 'o', 't' };
            char[] arrPW = new char[] { 'l', 'i', 'b', '1', '!', '#', '%', '&', '(' };

            m_strID = new string(arrID);
            m_strPW = new string(arrPW);

            // DB 열기
            string strConnection = GetStringConnection();
            m_dbConnection = new MySqlConnection(strConnection);

            m_isConnection = OpenConnection();
        }

        public bool OpenConnection()
        {
            if (m_isConnection)
                return true;

            try
            {
                m_dbConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                //System.Windows.Forms.Application.Exit();
            }

            return false;
        }

        public bool OpenConnection(MySqlConnection connection)
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                //System.Windows.Forms.Application.Exit();
            }

            return false;
        }

        public void CloseConnection()
        {
            if (!m_isConnection)
                return;

            m_dbConnection.Close();
            m_isConnection = false;
        }

        public void CloseConnection(MySqlConnection connection)
        {
            connection.Close();
        }

        protected string GetStringConnection()
        {
            string strConnection = "";

            strConnection = "server=10.131.5.6;" +
                            "database=pamts_sms2;" +
                            "port=3306;" +
                            "uid=smsuser;" +
                            "password=smsnd;";

            return strConnection;
        }

        public bool SendSMS(string strPhoneNumber, string strSendPhoneNumber, string strMsg)
        {
            //if (!FormMain.Instance.SMSOn)
            //    return true;

            if (!m_isConnection)
                return false;

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strSQL = string.Format("insert into LOG_SMS (CLASS, CLIENT, WRITE_TIME, DESTINATION, CALLBACK, BODY, SEND_FLAG) values ('TMS', 'SP', {0}, '{1}', '{2}', '{3}','1')",
                strTime, strPhoneNumber, strSendPhoneNumber, strMsg);

            try
            {
                MySqlCommand cmd = new MySqlCommand(strSQL, m_dbConnection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                return false;
            }

            return true;
        }

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
            System.Diagnostics.Trace.WriteLine(string.Format("[SendSMS], From({0}), {1}", strSendPhoneNumber, strMsg));
            if (!m_isConnection)
                return false;

            // 80바이트씩 메시지 쪼개기
            ArrayList arrMessages = MakeMessageList(strMsg);

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("'{0} {1:00}:{2:00}:{3:00}'", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);

            string strConnection = GetStringConnection();
            MySqlConnection dbConnection = new MySqlConnection(strConnection);

            bool isConnection = OpenConnection(dbConnection);

            MySqlTransaction transaction = null;
            try
            {
                transaction = dbConnection.BeginTransaction();
                foreach (string strMessage in arrMessages)
                {
                    foreach (string strPhoneNumber in arrPhoneNumbers)
                    {
                        string strSQL = string.Format("insert into LOG_SMS (CLASS, CLIENT, WRITE_TIME, DESTINATION, CALLBACK, BODY, SEND_FLAG) values ('TMS', 'SP', {0}, '{1}', '{2}', '{3}','1')",
                            strTime, strPhoneNumber, strSendPhoneNumber, strMessage);

                        MySqlCommand cmd = new MySqlCommand(strSQL, dbConnection);
                        cmd.Transaction = transaction;

                        cmd.ExecuteNonQuery();
                    }
                }
                if (transaction != null)
                    transaction.Commit();
            }
            catch (Exception e)
            {
                if (transaction != null)
                    transaction.Rollback();
                System.Windows.Forms.MessageBox.Show(e.Message);
                return false;
            }

            CloseConnection(dbConnection);

            return true;
        }
    }
}
