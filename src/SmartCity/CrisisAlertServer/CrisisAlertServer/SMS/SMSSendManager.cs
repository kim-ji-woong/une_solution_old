using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CrisisAlertServer.Data
{
    public class SMSSendManager
    {
        private FormMain m_form = null;
        private DataManager m_dataMgr = null;
        private Thread m_SMSThread = null;

        private string BaseAddress = "";

        private bool m_shutdownThread = false;
        public void Shutdown()
        {
            m_shutdownThread = true;
            m_SMSThread.Abort();
        }

        private bool m_startThread = false;
        public void StartThread()
        {
            m_startThread = true;
            m_form.ShowTextMessage("문자 서비스 시작");
        }
        public void StopThread()
        {
            m_startThread = false;
            m_form.ShowTextMessage("문자 서비스 종료");
        }

        public SMSSendManager(FormMain form)
        {
            m_form = form;
            m_dataMgr = m_form.DataManager;

            BaseAddress = ConfigurationManager.AppSettings.Get("SMSApiURL");
            if (BaseAddress == null || BaseAddress.Length == 0)
                BaseAddress = "http://221.147.100.161:8099";

            m_SMSThread = new Thread(new ThreadStart(SMSThread));
            m_SMSThread.Name = "SMS.Sender";
            m_SMSThread.Start();
        }

        private void SMSThread()
        {
            // 내부에서 SMS에서 보내는 방식
            //libSMS.IMessageClient client = libSMS.MessageClientFactory.CreateMessageClient(1);

            while (!m_shutdownThread)
            {
                if (m_startThread)
                {
                    // SMSSendMessage 조회
                    Dictionary<int, SMSData> dicSMSData = new Dictionary<int, SMSData>();
                    dicSMSData = m_dataMgr.LoadSMSMessage();

                    // 조회가 된다면 
                    foreach (KeyValuePair<int, SMSData> pair in dicSMSData)
                    {
                        libSMS.MessageContent contents = new libSMS.MessageContent();
                        contents.Caller = "02-714-4133";

                        // 내용 송신
                        int nID = pair.Key;
                        SMSData sms = pair.Value;
                        contents.Message = sms.Message;

                        string strPhoneNumbers = "";
                        string strMessage = sms.Message;

                        strMessage = strMessage.Replace("\r\n", "\\r\\n");

                        foreach (string phoneNumber in sms.NumberList)
                        {
                            contents.PhoneNumbers.Add(phoneNumber);

                            if (strPhoneNumbers == "")
                                strPhoneNumbers = phoneNumber;
                            else
                                strPhoneNumbers += "," + phoneNumber;
                        }

                        // 내부에서 SMS에서 보내는 방식
                        //client.SendSMS(contents);     

                        // API를 이용하여 SMS를 보내는 방식
                        Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
                        string strJson = "{\"message\": \"" + strMessage + "\", \"caller\":\"" + contents.Caller + "\", \"phoneNumbers\":\"" + strPhoneNumbers + "\"}";
                        string strURL = "/api/SMS";
                        string strErrorMessage = "";

                        string strResult = SendQuery(dicHeaders, strJson, strURL, out strErrorMessage, "POST");

                        // 삭제
                        m_dataMgr.DeleteSMSMessage(nID);
                    }

                    Thread.Sleep(1 * 1000);
                }
            }
        }

        private string SendQuery(Dictionary<string, string> dicHeaders, string strBodyJson, string strURL, out string strErrorMessage, string strMethodType = "GET")
        {
            strErrorMessage = "";
            string url = BaseAddress;

            if (strURL.StartsWith("/"))
                url += strURL;
            else
                url += "/" + strURL;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = strMethodType;

            if (dicHeaders != null)
            {
                request.ContentType = "application/json; charset=utf-8";

                // 요청 헤더 추가
                foreach (KeyValuePair<string, string> pair in dicHeaders)
                {
                    string key = pair.Key;
                    string value = pair.Value;
                    request.Headers.Add(key, value);
                }
            }

            string strResponse = "";

            try
            {
                if (strBodyJson != null && strBodyJson != "")
                {
                    StreamWriter streamWriter = new StreamWriter(request.GetRequestStream());
                    streamWriter.Write(strBodyJson);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResponse = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

            }
            catch (WebException ex)
            {
                strErrorMessage = ex.Status.ToString();
                return "";
            }

            if (strResponse == null)
            {
                strErrorMessage = "Request 실패";
                return "";
            }

            strErrorMessage = "success";
            return strResponse;
        }
    }
}

