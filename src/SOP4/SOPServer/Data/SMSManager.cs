using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.IO;
using System.Net;

namespace SDMSServer
{
	public class SMSManager
	{
        public enum SMSMessageType
        {
            UNKNOWN = -1,
            RESET_FIRE = 0,     // 화재복구(0)
            DETECT_FIRE,        // 화재탐지(1)
            REPORT_FIRE,        // 화재신고(2)
            DETECT_PSM,         // 누출탐지(3)
            REPORT_PSM,         // 누출신고(4)
            RESET_PSM,          // 누출복구(5)
            DETECT_SECURITY,    // 방범탐지(6)
            REPORT_SECURITY,    // 방범신고(7)
            RESET_SECURITY      // 방범복구(8)
        }

		private CookieContainer m_CookieContainer = new CookieContainer();
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

		private string m_strWebServerURL = "";
        private libSMS.IMessageClient m_msgClient = null;
        private log4net.ILog logger = null;

		private SMSManager()
		{
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			m_strWebServerURL = NetworkServer.Instance.DBManager.WebServerURL;
		}


        private int m_nMessageLength = 80;
		// strMsg를 80바이트씩 자른다.
		/*private ArrayList MakeMessageList(string strMsg)
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

                if (nByteLength == m_nMessageLength ||
					((nByteLength == (m_nMessageLength - 1)) && (i < nLen - 1 && strMsg.ElementAt(i + 1) >= 256)))
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
		}*/
		
		private static char ConvertToHex(char cSource)
		{
			return "0123456789abcdef"[0x0f & cSource];
		}

		public static string URLEncoding(byte[] bytes)
		{
			string strResult = "";

			foreach (byte element in bytes)
			{
				if ((element >= '0' && element <= '9') ||   // 숫자
					(element >= 'a' && element <= 'z') ||   // 소문자
					(element >= 'A' && element <= 'Z') ||   // 대문자
					(element == '!' || element == '*' || element == '(' || element == ')' || element == '_' || element == '-')) // 그 외의 특수기호들
				{
					strResult += (char)element;
				}
				else
				{
					strResult += "%";
					strResult += ConvertToHex((char)((int)element >> 4));
					strResult += ConvertToHex((char)element);
				}
			}

			return strResult;
		}

		private void RespCallback(IAsyncResult asynchronousResult)
		{
		}

        public string GetIP4Address()
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

        private libSMS.IMessageClient GetMessageClient()
        {
            if (m_msgClient == null)
            {
                string ipAddress = GetIP4Address();
                m_msgClient = libSMS.MessageClientFactory.CreateMessageClient(NetworkServer.Instance.SiteID, ipAddress);

                /*if (NetworkServer.Instance.SiteID == 100)
                    m_msgClient = new libSMS.MessageClientSNU(ipAddress);
                else
                    m_msgClient = new libSMS.MessageClient(ipAddress);*/
            }

            return m_msgClient;
        }

        public string SendSMS(string strPhoneNumber, string strSendPhoneNumber, string strMsg, libSMS.MessageContentMMS.ContentType contentType = libSMS.MessageContentMMS.ContentType.None, string strContentPath = "")
		{
            GetMessageClient();
            /*string ipAddress = GetIP4Address();

            if (m_msgClient == null)
            {
                if (NetworkServer.Instance.SiteID == 100)
                    m_msgClient = new libSMS.MessageClientSNU(ipAddress);
                else
                    m_msgClient = new libSMS.MessageClient(ipAddress);
            }*/

            if (NetworkServer.Instance.SimulationMode)
                strMsg = "[연습모드]" + strMsg;

            if (strContentPath.Length > 0)
                contentType = libSMS.MessageContentMMS.ContentType.Image;

            if (contentType == libSMS.MessageContentMMS.ContentType.Image)
            {
                if (m_msgClient.SendMMS(strSendPhoneNumber, strPhoneNumber, strMsg, "", contentType, strContentPath))
                {
                    return "OK";
                }
            }
            else
            {
                if (m_msgClient.SendSMS(strSendPhoneNumber, strPhoneNumber, strMsg))
                {
                    return "OK";
                }
            }

            return "";
		}

		/*
		public string SendSMS(string strPhoneNumber, string strSendPhoneNumber, string strMsg)
		{

			ArrayList arrMessages = MakeMessageList(strMsg);
			foreach (string szMsg in arrMessages)
			{
				string resResult = string.Empty;
				string sourceUrl = m_strWebServerURL + "/SendSMS.jsp";

				//Encoding enc = Encoding.GetEncoding(51949);
				//byte[] bytes1 = enc.GetBytes(szMsg);
				//string strUrlEncode = URLEncoding(bytes1);

				// 테스트 : %c5%d7%bd%ba%c6%ae%0d%0a - ok
				// 테스트 : %c5%d7%bd%ba%c6%ae
				string postData = "Sender=" + strSendPhoneNumber + "&" + "Reciver=" + strPhoneNumber + "&" + "Msg=" + szMsg;

				//sourceUrl = sourceUrl + "?" + postData;
				HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
				
				UTF8Encoding encoding = new UTF8Encoding();
				byte[] bytes = encoding.GetBytes(postData);

				lock (this)
				{
					try
					{
						wReq.CookieContainer = m_CookieContainer;
						wReq.Method = "POST";

						wReq.ContentType = "application/x-www-form-urlencoded";
						wReq.ContentLength = bytes.Length;

						using (Stream writeStream = wReq.GetRequestStream())
						{
							writeStream.Write(bytes, 0, bytes.Length);
						}

											
						HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

						// http 내용 추출
						Stream respPostStream = wRes.GetResponseStream();
						StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

						resResult = readerPost.ReadToEnd();

						readerPost.Close();

						respPostStream.Close();
					}
					catch (System.Net.WebException e)
					{
						//System.Windows.Forms.MessageBox.Show(e.Message);
						return "";
					}
				}

			}

			return "OK";
		}
		*/

        public bool SendSMS(ArrayList arrMembers, string strSendPhoneNumber, string strMsg, bool bToAll, libSMS.MessageContentMMS.ContentType contentType = libSMS.MessageContentMMS.ContentType.None, string strContentPath = "")
		{
            ArrayList arrPhoneNumbers = new ArrayList();

            foreach (DataMember member in arrMembers)
            {
                string szPhone = member.PhoneNumber;
                if (szPhone != null && !szPhone.Equals(""))
                {
                    arrPhoneNumbers.Add(szPhone);
                }
            }

            // 비번인 근무자는 제외한다.
            ArrayList arrValidPhoneNumbers = ControlTeamEditor.VaildMemberPhoneNumber.IsVaildPhoneNumber(arrPhoneNumbers, NetworkServer.Instance.DBManager);

            if (arrValidPhoneNumbers == null)
                return true;

            m_msgClient = GetMessageClient();
            m_nMessageLength = m_msgClient.GetMessageLength();

            // 메시지가 길이제한을(80바이트) 넘어서 여러개로 쪼개어질 경우
            // [메시지 Index/메시지 개수]를 메시지 앞에 붙여넣도록 한다.
            ArrayList arrMessages = (new SOPServer.Data.MessageDivider(m_nMessageLength)).MakeMessageList(strMsg);
			//ArrayList arrMessages = MakeMessageList(strMsg);

            m_msgClient.BeginSend();
             
            foreach (string szMsg in arrMessages)
            {
                foreach (string szPhone in arrValidPhoneNumbers)
                {
                    SendSMS(szPhone, strSendPhoneNumber, szMsg, contentType, strContentPath);
                    //System.Diagnostics.Trace.WriteLine(szPhone);
                }
                /*foreach (DataMember member in arrMembers)
                {
                    string szPhone = member.PhoneNumber;
                    if (szPhone != null && !szPhone.Equals(""))
                    {
                        SendSMS(szPhone, strSendPhoneNumber, szMsg);
                    }
                }*/
            }

            m_msgClient.EndSend();
			return true;
		}


        public bool SendSMSForPhoneNumber(ArrayList arrPhoneNumbers, int nBeginIndex, string strSendPhoneNumber, string strMsg)
        {
            if (nBeginIndex < 0)
                return false;

            m_msgClient = GetMessageClient();
            m_nMessageLength = m_msgClient.GetMessageLength();

            ArrayList arrMessages = (new SOPServer.Data.MessageDivider(m_nMessageLength)).MakeMessageList(strMsg);
            //ArrayList arrMessages = MakeMessageList(strMsg);
            int nPhoneNumberCount = arrPhoneNumbers.Count;

            m_msgClient.BeginSend();

            foreach (string szMsg in arrMessages)
            {
                for (int i = nBeginIndex; i < nPhoneNumberCount;i++ )
                {
                    if ((arrPhoneNumbers[i] is string) == false)
                    {
                        m_msgClient.EndSend();
                        return false;
                    }

                    string szPhone = (string)arrPhoneNumbers[i];                   
                    if (szPhone != null && !szPhone.Equals(""))
                    {
                        SendSMS(szPhone, strSendPhoneNumber, szMsg);
                    }
                }
            }

            m_msgClient.EndSend();
            return true;
        }

        public static bool IsReportType(SMSMessageType type)
        {
            if (type == SMSMessageType.REPORT_FIRE ||
                type == SMSMessageType.REPORT_PSM ||
                type == SMSMessageType.REPORT_SECURITY)
                return true;

            return false;
        }
	}
}
