using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Messaging;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Net;

namespace MSMQTest
{
	public partial class MainForm : Form
	{
        private string m_szMsgEncoding = "ks_c_5601-1987";
        private string m_szMsgEncoding2 = "ks_c_5601-1987";
        private string m_szMysqlEncoding = "euckr";

        // Remote에서 Queue접근시 ID
        private string m_szQueueID = "FormatName:Direct=TCP:{0}\\private$\\smsreciver";

        // Local 에서 Queue 생성시 이름
        private string m_szQueueName = "private$\\smsreciver";
        // {FC04D23E-D88E-49F2-894F-EE17F92A7451}

        private MessageBroker broker = null;

		public MainForm()
		{

            try
            {
                log4net.Config.DOMConfigurator.Configure();
            }
            catch (System.Exception ex)
            {

            }

			InitializeComponent();
		}
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            string szIp = "192.168.0.195";// GetIP4Address();
            editMsgQueue.Text = szIp;
            m_szQueueID = string.Format(m_szQueueID, szIp);

            cmbMsgEncoding1.SelectedIndex = 0;
            cmbMsgEncoding2.SelectedIndex = 1;
            cmbMysqlEncoding.SelectedIndex = 0;

            string strSection = "Message Server Info";
            string  m_szServerIP = RegUtil.ReadRegValue(strSection, "MessageDBServerIP");
            if (m_szServerIP == null || m_szServerIP == "")
            {
                //m_szServerIP = "10.131.5.6";
                RegUtil.WriteRegValue(strSection, "MessageDBServerIP", m_szServerIP);
            }
            this.editMysqlServer.Text = m_szServerIP;
        }

        private string GetIP4Address()
        {
            string IP4Address = String.Empty;
            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }
            return IP4Address;
        }

		public void InitMessageQueue()
		{
			try
			{
				string szQueuePath = ".\\" + m_szQueueName;
				if (!MessageQueue.Exists(szQueuePath))
				{
					MessageQueue msgQ = MessageQueue.Create(szQueuePath, true);
					msgQ.Category = new Guid("FC04D23E-D88E-49F2-894F-EE17F92A7451");

					msgQ.UseJournalQueue = false;					
					msgQ.Authenticate = false;
					msgQ.Label = m_szQueueName;
					msgQ.SetPermissions("EveryOne", MessageQueueAccessRights.FullControl, AccessControlEntryType.Set);
				}
			}
			catch(Exception)
			{
			}	
		}

		public void SendMessage(MessageContent content)
		{
			try
			{				
				// 해당 Message Queue를 연다.				
				using (MessageQueue msgQ = new MessageQueue(m_szQueueID))
				{
					// Message Queue 인증 옵션
					//msgQ.Authenticate = false;
					// Message Queue 사용 권한 설정
					//msgQ.SetPermissions(Environment.UserName, MessageQueueAccessRights.FullControl, AccessControlEntryType.Set);
					// Message 생성
					System.Messaging.Message msg = new System.Messaging.Message();
					// 전송할 Body Data를 지정
					msg.Body = content;
					//msg.UseEncryption = true;
					// Message Queue로 전송					
					msg.Label = "단문전송";
					msg.Recoverable = true;
					
					msgQ.Send(msg, MessageQueueTransactionType.Single);
				}				
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.StackTrace);
			}
		}

		public MessageContent ReadMessageContent()
		{
			try
			{
				// 해당 Message Queue를 연다.	
				using (MessageQueue msgQ = new MessageQueue(m_szQueueID))
				{
					// Message Queue 인증 옵션
					//msgQ.Authenticate = false;
					// Message Queue 권한 설정
					//msgQ.SetPermissions(Environment.UserName, MessageQueueAccessRights.FullControl, AccessControlEntryType.Set);

					// Message Queue에서 1개의 Msg를 읽는다. 없으면 1초후에 반환
					System.Messaging.Message msg = msgQ.Receive(new TimeSpan(0, 0, 1));
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
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.StackTrace);
			}
			return null;
		}

		public List<MessageContent> ReadAllMessageContent()
		{
			List<MessageContent> arResult = new List<MessageContent>();

			try
			{
				// 해당 Message Queue를 연다.	
				using (MessageQueue msgQ = new MessageQueue(m_szQueueID))
				{
					// Message Queue 권한 설정
					msgQ.SetPermissions(Environment.UserName, MessageQueueAccessRights.FullControl, AccessControlEntryType.Set);

					// 현재 Message Queue에 있는 모든 Msg를 읽는다. 
					System.Messaging.Message [] msgList = msgQ.GetAllMessages();
					if (msgList == null)
						return arResult;

					for(int i = 0 ; i < msgList.Length ; i++)
					{
						
						System.Messaging.Message msg = msgList[i];
					
						// XMLXerializer를 이용하여 객체 Deserialize
						using (XmlReader xreader = XmlReader.Create(msg.BodyStream))
						{
							XmlSerializer sz = new XmlSerializer(typeof(MessageContent));
							MessageContent content = (MessageContent)sz.Deserialize(xreader);
							arResult.Add(content);
						}
					}					
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.StackTrace);
			}
			return arResult;
		}

		private void btnSendSMS_Click(object sender, EventArgs e)
		{
            string szSendMsg = editMsg.Text;
            if (szSendMsg == null || szSendMsg == "")
                return;

			string szText = editMsgIter.Text;
			int nCount;

			if( int.TryParse(szText, out nCount))
			{
				for(int i = 1 ; i <= nCount ; i++)
				{
					MessageContent sms = new MessageContent();
                   
                    //E//ncoding enc = Encoding.GetEncoding(m_szMsgEncoding);
                   // byte[] bytes1 = Encoding.Default.GetBytes(szSendMsg);
                    //string szMsg = enc.GetString(bytes1, 0, bytes1.Length);
                    sms.Message = szSendMsg;

                    sms.Caller = editCallback.Text;
                    sms.Reciver = editReciver.Text;
					SendMessage(sms);
				}				
			}			
		}

		private void btnGetAllMessage_Click(object sender, EventArgs e)
		{
			MessageContent content = ReadMessageContent();
			if( content != null)
			{
				MessageBox.Show(content.Message);
			}
		}

		private void btnCreateQueue_Click(object sender, EventArgs e)
		{
			InitMessageQueue();
		}

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSaveRegistry_Click(object sender, EventArgs e)
        {
            SMSDBManager.Instance.SaveConnectionInfo();
            if (broker != null)
                broker.SaveConnectionInfo();
        }

        private void cmbMsgEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMsgEncoding1.SelectedItem != null)
            {
                string szText = (string)cmbMsgEncoding1.SelectedItem;
                if (szText != null)
                {
                    if (szText == "KSC5601")
                    {
                        m_szMsgEncoding = "ks_c_5601-1987";
                    }
                    else if(szText == "UTF-8")
                    {
                        m_szMsgEncoding = "UTF-8";
                    }
                    else
                    {                      
                        m_szMsgEncoding = "iso-8859-1";                   
                    }
                }
            }
        }


        private void cmbMsgEncoding2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMsgEncoding2.SelectedItem != null)
            {
                string szText = (string)cmbMsgEncoding2.SelectedItem;
                if (szText != null)
                {
                    if (szText == "KSC5601")
                    {
                        m_szMsgEncoding2 = "ks_c_5601-1987";
                    }
                    else if (szText == "UTF-8")
                    {
                        m_szMsgEncoding2 = "UTF-8";
                    }
                    else
                    {
                        m_szMsgEncoding2 = "iso-8859-1";
                    }
                }
            }
        }

        private void cmbMysqlEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbMysqlEncoding.SelectedItem != null)
            {
                string szText = (string)cmbMysqlEncoding.SelectedItem;
                if( szText != null)
                {
                    if(szText == "KSC5601")
                    {
                        m_szMysqlEncoding = "euckr";
                    }
                    else if(szText == "UTF-8")
                    {
                        m_szMysqlEncoding = "utf8";
                    }
                    else
                    {
                        m_szMysqlEncoding = "latin1";
                    }
                }
            }
        }

        private void btnMysqlConnect_Click(object sender, EventArgs e)
        {
            if (!SMSDBManager.Instance.IsConnect)
            {
                
                SMSDBManager.Instance.CharSet = m_szMysqlEncoding;
                SMSDBManager.Instance.Port = "3306";
                SMSDBManager.Instance.ServerIP = editMysqlServer.Text;

                if(SMSDBManager.Instance.Connect())
                {
                    cmbMysqlEncoding.Enabled = false;
                    editMysqlServer.Enabled = false;
                    btnMysqlConnect.Enabled = false;
                    btnMysqlDisconnect.Enabled = true;
                    lbStatusMysql.Text = "접속이 되었습니다.";
                    lbStatusMysql.ForeColor = Color.Green;
                }
            }
        }

        private void btnMysqlDisconnect_Click(object sender, EventArgs e)
        {
           // if (!SMSDBManager.Instance.IsConnect)
            {

                cmbMysqlEncoding.Enabled = true;
                editMysqlServer.Enabled = true;

                btnMysqlConnect.Enabled = true;
                btnMysqlDisconnect.Enabled = false;
                lbStatusMysql.Text = "접속되지 않았습니다.";
                lbStatusMysql.ForeColor = Color.Red;

                SMSDBManager.Instance.Close();
            }
        }

       

        private void btnSaveMsgToDB_Click(object sender, EventArgs e)
        {
            if( SMSDBManager.Instance.IsConnect)
            {
                string szSendMsg = editMsg.Text;
                if (szSendMsg == null || szSendMsg == "")
                    return;

                string szText = editMsgIter.Text;
                int nCount;

                if (int.TryParse(szText, out nCount))
                {
                    for (int i = 1; i <= nCount; i++)
                    {
                        MessageContent sms = new MessageContent();
                        
                        //Encoding enc = Encoding.GetEncoding(m_szMsgEncoding);
                        //byte[] bytes1 = Encoding.UTF8.GetBytes(szSendMsg);
                        //byte[] bytes2 = Encoding.Convert(Encoding.UTF8, enc, bytes1);
                        //string szMsg = enc.GetString(bytes2);
                        sms.Message = szSendMsg + "_" + i;
                        
                        sms.Caller = editCallback.Text;
                        sms.Reciver = editReciver.Text;

                        SMSDBManager.Instance.InsertMessage(sms);

                        System.Threading.Thread.Sleep(50);
                    }
                }		
            }
            else
            {
                MessageBox.Show("DB에 접속이 되어 있지 않습니다.");
            }

        }

        private void btnFetchQueue_Click(object sender, EventArgs e)
        {
            if (btnFetchQueue.Text != "감시 해제")
            {
                btnCreateQueue.Enabled = false;
                btnGetAllMessage.Enabled = false;
                cmbMsgEncoding1.Enabled = false;
                editMsgQueue.Enabled = false;
                cmbMsgEncoding2.Enabled = false;
                lbStatusQueue.Text = "MQ가 감시중입니다.";
                lbStatusQueue.ForeColor = Color.Green;

                btnFetchQueue.Text = "감시 해제";

                if (broker != null)
                {
                    broker.Close();
                    broker = null;
                }

                if(! SMSDBManager.Instance.IsConnect)
                    btnMysqlConnect_Click(null, null);

                SMSDBManager.Instance.MsgCharSetFrom = m_szMsgEncoding;
                SMSDBManager.Instance.MsgCharSetTo = m_szMsgEncoding2;

                btnMysqlDisconnect.Enabled = false;

                broker = new MessageBroker(editMsgQueue.Text);
                broker.MessageLoop();
            }
            else
            {
                btnMysqlDisconnect.Enabled = true;
                cmbMsgEncoding2.Enabled = true;
                btnCreateQueue.Enabled = true;
                btnGetAllMessage.Enabled = true;
                cmbMsgEncoding1.Enabled = true;
                editMsgQueue.Enabled = true;

                lbStatusQueue.Text = "MQ 감시 해제 되었습니다.";
                lbStatusQueue.ForeColor = Color.Red;

                btnFetchQueue.Text = "큐 감시";
                if (broker!= null)
                {
                    broker.Close();
                    broker = null;
                }              
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (broker != null)
            {
                broker.Close();
                broker = null;
            }
            SMSDBManager.Instance.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string szText = SMSDBManager.Instance.CheckCharset();

            string [] szLine = szText.Split(new char[]{'\n','\r'});
            szText = "";
            for (int i = 0; i < szLine.Length; i++)
            {
                szText += szLine[i];
                szText += Environment.NewLine;
            }
            
            MessageBox.Show(szText);
        }

	}

	
}
