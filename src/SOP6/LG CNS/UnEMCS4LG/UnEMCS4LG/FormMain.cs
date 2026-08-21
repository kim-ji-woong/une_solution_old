using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility2;
using System.Threading;
using System.Collections;
using libSMS;

namespace UnEMCS4LG
{
    public partial class FormMain : Form
    {
        private WebDBManager m_dbMgr = null;
        private bool m_closeSystem = false;

        public FormMain()
        {
            InitializeComponent();
            SetDBManager();

            Thread t = new Thread(new ThreadStart(MonitoringMMS));
            t.Start();
        }

        private void MonitoringMMS()
        {
            if (m_dbMgr == null)
                return;

            while (m_closeSystem == false)
            {
                List<MCSMessage> messages = ReadNewMessage();

                if (messages.Count > 0)
                {
                    if (SendMMS(messages))
                    {
                        RemoveMessage(messages);
                        WriteHistory(messages);

                        this.Invoke((MethodInvoker)delegate
                        {
                            UpdateLastMessage(messages.Last());
                        });
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private void UpdateLastMessage(MCSMessage message)
        {
            string strTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'",
                    message.TimeStamp.Year, message.TimeStamp.Month, message.TimeStamp.Day,
                    message.TimeStamp.Hour, message.TimeStamp.Minute, message.TimeStamp.Second);

            string strImage = message.Image == null || message.Image.Length == 0 ? "없음" : message.Image;

            string strMessage = string.Format("마지막으로 보낸 시각 : {0}\r\n", strTime);
            strMessage += string.Format("수신번호 : {0}\r\n", message.PhoneNumbers);
            strMessage += string.Format("메시지 : {0}\r\n", message.Message);
            strMessage += string.Format("이미지 : {0}", strImage);

            textBoxLastHistory.Text = strMessage;
        }

        private void WriteHistory(List<MCSMessage> messages)
        {
            foreach (MCSMessage message in messages)
            {
                string strTime = string.Format("'{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}'",
                    message.TimeStamp.Year, message.TimeStamp.Month, message.TimeStamp.Day,
                    message.TimeStamp.Hour, message.TimeStamp.Minute, message.TimeStamp.Second);

                string strImage = message.Image == null || message.Image.Length == 0 ? "NULL" : "'" + message.Image + "'";

                string strSQL = "Insert into UnEMCSMessageHistory (TimeStamp, Message, Image, PhoneNumbers) values (";
                strSQL += string.Format("{0}, '{1}', {2}, '{3}')", strTime, message.Message, strImage, message.PhoneNumbers);

                m_dbMgr.GetResultData(strSQL);
            }
        }

        private void RemoveMessage(List<MCSMessage> messages)
        {
            string strIDs = "";

            foreach (MCSMessage message in messages)
            {
                if (strIDs.Length == 0)
                    strIDs = message.ID.ToString();
                else
                    strIDs += ", " + message.ID.ToString();
            }

            if (strIDs.Length > 0)
            {
                string strSQL = "Delete from UnEMCSMessage where ID in (" + strIDs + ")";
                m_dbMgr.GetResultData(strSQL);
            }
        }

        private bool SendMMS(List<MCSMessage> messages)
        {
            IMessageClient client = MessageClientFactory.CreateMessageClient(m_dbMgr.SiteID);

            foreach (MCSMessage message in messages)
            {
                SendMMS(client, message);
            }

            return true;
        }

        private bool SendMMS(IMessageClient client, MCSMessage message)
        {
            string[] phoneNumbers = message.PhoneNumbers.Split(';');

            MessageContentMMS mms = new MessageContentMMS();

            foreach (string strPhoneNumber in phoneNumbers)
            {
                mms.PhoneNumbers.Add(strPhoneNumber.Trim());
            }

            mms.Message = message.Message;

            if (message.Image != null && message.Image.Length > 0)
            {
                mms.ContentsList.Add(new KeyValuePair<MessageContentMMS.ContentType, string>(MessageContentMMS.ContentType.Image, message.Image));
            }

            return client.SendMMS(mms);
        }

        private List<MCSMessage> ReadNewMessage()
        {
            string strSQL = "Select ID, PhoneNumbers, Message, Image, TimeStamp from UnEMCSMessage";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            List<MCSMessage> messages = new List<MCSMessage>();

            if (arrResult == null)
                return messages;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strPhoneNumbers = WebDBManager.GetStringField(arrResult[i + 1]);
                string strMessage = WebDBManager.GetStringField(arrResult[i + 2]);
                string strImage = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<DateTime> timeStamp = WebDBManager.GetDateTimeField(arrResult[i + 4].ToString());

                if (id == null || strPhoneNumbers == null || strMessage == null || timeStamp == null)
                    continue;

                MCSMessage message = new MCSMessage();

                message.ID = id.Data;
                message.PhoneNumbers = strPhoneNumbers;
                message.Message = strMessage;
                message.Image = strImage;
                message.TimeStamp = timeStamp.Data;

                messages.Add(message);
            }

            return messages;
        }

        private void SetDBManager()
        {
            string strSiteID = System.Configuration.ConfigurationManager.AppSettings["siteid"].ToString();
            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings["webserver"].ToString();
            string strDBName = System.Configuration.ConfigurationManager.AppSettings["dbname"].ToString();
            string strDBType = System.Configuration.ConfigurationManager.AppSettings["dbtype"].ToString();

            int nDBType;

            if (int.TryParse(strDBType, out nDBType) == false)
                return;

            int nSiteID = 0;

            if (int.TryParse(strSiteID, out nSiteID) == false)
                return;

            WebDBManager dbMgr = new WebDBManager(nSiteID);
            dbMgr.WebServerURL = strWebServerURL;
            dbMgr.DatabaseName = strDBName;
            dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;

            m_dbMgr = dbMgr;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_closeSystem = true;
        }
    }
}
