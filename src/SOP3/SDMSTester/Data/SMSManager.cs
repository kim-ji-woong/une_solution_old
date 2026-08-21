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
		private SMSManager()
		{
			m_strWebServerURL = NetworkServer.Instance.DBManager.WebServerURL;
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

		public string SendSMS(string strPhoneNumber, string strSendPhoneNumber, string strMsg)
		{

			ArrayList arrMessages = MakeMessageList(strMsg);
			foreach (string szMsg in arrMessages)
			{
				string resResult = string.Empty;
				string sourceUrl = m_strWebServerURL + "/SendSMS.jsp";

				Encoding enc = Encoding.GetEncoding(51949);
				byte[] bytes1 = enc.GetBytes(szMsg);
				string strUrlEncode = URLEncoding(bytes1);

				// 테스트 : %c5%d7%bd%ba%c6%ae%0d%0a - ok
				// 테스트 : %c5%d7%bd%ba%c6%ae
				string postData = "Sender=" + strSendPhoneNumber + "&" + "Reciver=" + strPhoneNumber + "&" + "Msg=" + strUrlEncode;

				sourceUrl = sourceUrl + "?" + postData;
				HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
				lock (this)
				{
					wReq.Method = "GET";
					try
					{
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

		public bool SendSMS(ArrayList arrPhoneNumbers, string strSendPhoneNumber, string strMsg)
		{

			ArrayList arrMessages = MakeMessageList(strMsg);
			foreach (string szPhone in arrPhoneNumbers)
			{
				if (szPhone != null && !szPhone.Equals(""))
				{
					foreach (string szMsg in arrMessages)
					{
						SendSMS(szPhone, strSendPhoneNumber, szMsg);
					}
				}
			}
			return true;
		}
	}



}
