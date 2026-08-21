using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Threading;

namespace MessageServer
{
    public class MessageBroker
    {
        private bool m_bReleaseThread = false;

        private Thread m_Reader = null;

        // Local 에서 Queue 생성시 이름
        private string m_szQueueName = ".\\private$\\smsreciver";

        private string m_szQueuePath = "";
        public string QueuePath
        {
            get { return m_szQueuePath; }
        }


        private MessageQueue m_MessageQueue = null;

        public MessageBroker()
        {
            LoadConnectionInfo();
            
            if (!InitMessageQueue())
            {
            }
        }

        private void LoadConnectionInfo()
        {
            string strSection = "Message Server Info";

            m_szQueuePath = RegUtil.ReadRegValue(strSection, "MessageQueue");
            if (m_szQueuePath == null || m_szQueuePath == "")
            {
                m_szQueuePath = ".\\private$\\smsreciver";
                RegUtil.WriteRegValue(strSection, "MessageQueue", m_szQueuePath);
            }
        }

        public void SaveConnectionInfo()
        {
            string strSection = "Message Server Info";
            RegUtil.WriteRegValue(strSection, "MessageQueue", m_szQueuePath);
        }

        public void Close()
        {
            m_bReleaseThread = true;

            try
            {
                if (m_Reader != null)
                {
                    m_Reader.Join();
                }
            }
            catch (Exception)
            {
            }
            m_Reader = null;
        }

        public void MessageLoop()
        {
            if (m_Reader != null)
                return;

            m_Reader = new Thread(ProcessSMS);
            m_Reader.Start();
        }

        //private StreamWriter sw = new StreamWriter("C:/UnE/SMSMessage/sms.log", false, Encoding.UTF8);

        private void WriteLog(string strLog)
        {
            //sw.WriteLine(strLog);
            //sw.Flush();
        }

        private int nCount = 0;
        private void ProcessSMS()
        {
            DateTime dtNow = DateTime.Now;
            int nYear = dtNow.Year;
            int nMonth = dtNow.Month;
            int nDay = dtNow.Day;

            string strMessagePath = System.Configuration.ConfigurationManager.AppSettings.Get("MessagePath");
            //SMSDBManager.Instance.m_writer = sw;

            DBUtility2.WebDBManager dbMgr = new DBUtility2.WebDBManager(1);
            dbMgr.WebServerURL = System.Configuration.ConfigurationManager.AppSettings.Get("WebServerURL");
            dbMgr.DatabaseName = "SOP_1";
            dbMgr.DatabaseType = DBUtility2.WebDBManager.DBType.sqlserver;

            string strCaller = System.Configuration.ConfigurationManager.AppSettings.Get("Caller");

            while (m_bReleaseThread == false)
            {
                List<MessageContent> arResult = ReadAllMessageContent(strMessagePath, dbMgr, strCaller);
                //List<MessageContent> arResult = ReadAllMessageContent();
                if (arResult != null && arResult.Count > 0)
                {
                    WriteLog("ReadMessage : " + arResult.Count);
                    MessageContent content = arResult[0];
                    List<MessageContent> arResult2 = new List<MessageContent>(arResult);
                    arResult2.Remove(content);

                    if( SMSDBManager.Instance.InsertMessage(content) == true)
                    {
                        WriteLog("first InsertMessage success");
                        bool result = SMSDBManager.Instance.InsertMessage(arResult2);
                        WriteLog("another InsertMessage is " + result);
                    }
                    else
                    {
                        WriteLog("first InsertMessage fail");
                        bool result = SMSDBManager.Instance.InsertMessage(arResult);
                        WriteLog("another InsertMessage is " + result);
                    }
                    
                }
                else
                {
                    Thread.Sleep(1000);
                }

                Thread.Sleep(100);
                nCount++;

                if( nCount == 3600)
                {
                    nCount = 0;
                    System.GC.Collect();
                }

                dtNow = DateTime.Now;

                if (dtNow.Year != nYear && dtNow.Month != nMonth && dtNow.Day != nDay)
                {
                    nYear = dtNow.Year;
                    nMonth = dtNow.Month;
                    nDay = dtNow.Day;

                    DeleteLog();
                }
            }
        }

        // 한달이 경과한 로그는 삭제한다.
        private void DeleteLog()
        {
            DateTime dtLast = DateTime.Now.AddMonths(-1);
            string strLastDate = string.Format("{0}-{1:00}-{2:00}", dtLast.Year, dtLast.Month, dtLast.Day);

            string[] files = Directory.GetFiles(".", "*.log*");

            foreach (string strFile in files)
            {
                int nIndex = strFile.IndexOf('-');

                if (nIndex < 0)
                    continue;

                string strDate = strFile.Substring(nIndex + 1);

                if (strDate.CompareTo(strLastDate) < 0)
                {
                    File.Delete(strFile);
                }
            }
        }

        private bool InitMessageQueue()
        {
            try
            {
                if (!MessageQueue.Exists(m_szQueuePath))
                {
                    m_MessageQueue = MessageQueue.Create(m_szQueuePath, true);
                    m_MessageQueue.Category = new Guid("FC04D23E-D88E-49F2-894F-EE17F92A7451");

                    m_MessageQueue.UseJournalQueue = false;
                    m_MessageQueue.Authenticate = false;
                    m_MessageQueue.Label = m_szQueueName;
                    m_MessageQueue.SetPermissions("EveryOne", MessageQueueAccessRights.FullControl, AccessControlEntryType.Set);

                    return true;
                }
                else
                {
                    m_MessageQueue = new MessageQueue(m_szQueuePath);
                    List<MessageContent> arList = ReadAllMessageContent();
                    foreach (MessageContent content in arList)
                    {

                    }

                }
            }
            catch (Exception ex)
            {
                MessageService.Logger.Debug(ex.Message);
                MessageService.Logger.Debug(ex.StackTrace);
            }
            return false;
        }

        internal MessageContent ReadMessageContent()
        {
            try
            {
                // Message Queue에서 1개의 Msg를 읽는다. 없으면 1초후에 반환
                System.Messaging.Message msg = m_MessageQueue.Receive(new TimeSpan(0, 0, 1));
                if (msg == null)
                    return null;
                // XMLXerializer를 이용하여 객체 Deserialize
                using (XmlReader xreader = XmlReader.Create(msg.BodyStream))
                {
                    XmlSerializer sz = new XmlSerializer(typeof(MessageContent));
                    MessageContent content = (MessageContent)sz.Deserialize(xreader);
                    return content;
                }

            }
            catch (Exception ex)
            {
                MessageService.Logger.Debug(ex.Message);
                MessageService.Logger.Debug(ex.StackTrace);
            }
            return null;
        }

        // 파일로부터 메시지를 읽어온다.
        internal List<MessageContent> ReadAllMessageContent(string strMessagePath, DBUtility2.WebDBManager dbMgr, string strCaller)
        {
            List<MessageContent> arResult = new List<MessageContent>();

            if (strMessagePath == null || strMessagePath.Length == 0)
                return arResult;

            try
            {
                ReadMessageFromDB(dbMgr, strCaller, arResult);
                /*string[] files = Directory.GetFiles(strMessagePath, "*.sms");

                foreach (string strFile in files)
                {
                    ReadMessageFromFile(strFile, arResult);

                    // 읽은 파일은 지운다.
                    File.Delete(strFile);
                }*/
            }
            catch (Exception ex)
            {
                MessageService.Logger.Debug(ex.Message);
                MessageService.Logger.Debug(ex.StackTrace);
            }
            return arResult;
        }

        private int ReadMessageFromDB(DBUtility2.WebDBManager dbMgr, string strCaller, List<MessageContent> contentsList)
        {
            string strSQL = "Select ID, PhoneNumber, Message from SMS_Data";
            System.Collections.ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return 0;

            int nResultCount = arrResult.Count;
            string strIDs = "";

            for (int i=0;i<nResultCount-2;i+=3)
            {
                DBUtility2.VariousData<int> id = DBUtility2.WebDBManager.GetIntField(arrResult[i].ToString());
                string strPhoneNumber = DBUtility2.WebDBManager.GetStringField(arrResult[i + 1]);
                string strMessage = DBUtility2.WebDBManager.GetStringField(arrResult[i + 2]);

                if (id == null || strPhoneNumber == null || strMessage == null)
                    continue;

                if (strIDs.Length == 0)
                    strIDs = id.Data.ToString();
                else
                    strIDs += ", " + id.Data.ToString();

                MessageContent content = new MessageContent();
                content.Caller = strCaller;
                content.Reciver = strPhoneNumber.Trim();
                content.Message = strMessage.Trim();

                contentsList.Add(content);
            }

            if (strIDs.Length > 0)
            {
                strSQL = "Delete from SMS_Data where ID in (" + strIDs + ")";
                dbMgr.GetResultData(strSQL);
            }

            return contentsList.Count;
        }

        private void ReadMessageFromFile(string strFile, List<MessageContent> contents)
        {
            try
            {
                using (StreamReader reader = new StreamReader(strFile, Encoding.UTF8))
                {
                    string[] phoneNumbers = null;
                    string strCaller = null, strMsg = "";

                    for (int i = 0; reader.EndOfStream == false; i++)
                    {
                        string strLine = reader.ReadLine().Trim();

                        if (i == 0)
                            strCaller = strLine;
                        else if (i == 1)
                            phoneNumbers = strLine.Split(',');
                        else
                        {
                            if (i == 2)
                                strMsg = strLine;
                            else
                                strMsg += "\r\n" + strLine;
                        }
                    }

                    if (strCaller != null && strCaller.Length > 0 &&
                        phoneNumbers != null && phoneNumbers.Count() > 0 &&
                        strMsg.Length > 0)
                    {
                        foreach (string strPhoneNumber in phoneNumbers)
                        {
                            MessageContent content = new MessageContent();

                            content.Caller = strCaller;
                            content.Reciver = strPhoneNumber.Trim();
                            content.Message = strMsg;

                            contents.Add(content);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("ReadMessageContent Error : " + e.Message);
            }
        }

        // MSMQ로부터 메시지를 읽어온다.
        internal List<MessageContent> ReadAllMessageContent()
        {
            List<MessageContent> arResult = new List<MessageContent>();

            try
            {
                // 현재 Message Queue에 있는 모든 Msg를 읽는다. 
                System.Messaging.Message[] msgList = m_MessageQueue.GetAllMessages();
                if (msgList == null)
                    return arResult;

                MessageEnumerator msgEnumerator = m_MessageQueue.GetMessageEnumerator2();
                while (msgEnumerator.MoveNext(new TimeSpan(0, 0, 0)))
                {
                    Message msg = m_MessageQueue.ReceiveById(msgEnumerator.Current.Id, new TimeSpan(0, 0, 0));
                    using (XmlReader xreader = XmlReader.Create(msg.BodyStream))
                    {
                        XmlSerializer sz = new XmlSerializer(typeof(MessageContent));
                        MessageContent content = (MessageContent)sz.Deserialize(xreader);
                        
                        arResult.Add(content);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageService.Logger.Debug(ex.Message);
                MessageService.Logger.Debug(ex.StackTrace);
            }
            return arResult;
        }
    }

    public class MessageContent
    {
        private string m_szMsg = "";
        public string Message
        {
            get { return m_szMsg; }
            set { m_szMsg = value; }
        }

        private string m_szCaller = "";
        public string Caller
        {
            get { return m_szCaller; }
            set { m_szCaller = value; }
        }

        private string m_szReciver = "";
        public string Reciver
        {
            get { return m_szReciver; }
            set { m_szReciver = value; }
        }

        private bool m_bEncryptCaller = false;
        public bool EncryptCaller
        {
            get { return m_bEncryptCaller; }
            set { m_bEncryptCaller = value; }
        }

        private string m_szSmsTag = "";
        public string SmsTag
        {
            get { return m_szSmsTag; }
            set { m_szSmsTag = value; }
        }
    }

}
