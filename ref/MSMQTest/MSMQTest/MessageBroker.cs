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

namespace MSMQTest
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

        public MessageBroker(string szServerIP)
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

        private void ProcessSMS()
        {
            while (m_bReleaseThread == false)
            {
                List<MessageContent> arResult = ReadAllMessageContent();
                if (arResult != null)
                {
                    SMSDBManager.Instance.InsertMessage(arResult);                   
                }
                else
                {
                    Thread.Sleep(1000);
                }

                Thread.Sleep(50);
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
            catch (Exception)
            {
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
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }
            return null;
        }

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
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
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
    }

}
