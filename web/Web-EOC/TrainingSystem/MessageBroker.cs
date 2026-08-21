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

namespace libSMS
{
	public class MessageClient : IDisposable
	{
		private string m_szServerIP = "";
		private MessageBroker m_broker = null;
		public MessageClient(string szServerIP)
		{
			m_szServerIP = szServerIP;
			m_broker = new MessageBroker(szServerIP);
		}

        public void Dispose()
        {

        }
		
		public bool SendSMS(string szCaller, string szReciver, string szContent, bool bEncryptCaller = false)
		{
			if (m_broker != null)
			{
				MessageContent sms = new MessageContent();
				sms.Caller = szCaller;
				sms.Reciver = szReciver;
				sms.EncryptCaller = bEncryptCaller;
                sms.SmsTag = DateTime.Now.ToLongTimeString();
				
                // KDNS용 인코딩 변환
				//Encoding enc = Encoding.GetEncoding("ISO-8859-1");
				//byte[] bytes1 = enc.GetBytes(szContent);
				
                //Encoding enc2 = Encoding.GetEncoding("ks_c_5601-1987");
				//string szMsg = enc2.GetString(bytes1, 0, bytes1.Length);
				string szMsg = szContent;
				sms.Message = szMsg;

				m_broker.SendMessage(sms);
				return true;
			}
			return false;
		}

		public bool SendSMS(List<MessageContent> arMessages)
		{
			if (arMessages == null)
				return false;

			foreach(MessageContent content in arMessages)
			{
				if (m_broker != null)
				{
					m_broker.SendMessage(content);
				}
			}
			return true;
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
		
		internal bool SendMessage(MessageContent content)
		{
			if (m_MessageQueue == null)
				return false;		

			try
			{				
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
}
