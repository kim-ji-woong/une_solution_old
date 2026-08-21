using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.IO;
using System.Net;
using System.Threading;

namespace SOPMonitoringSystem
{
    //public class SMSManager
    //{
    //    private static SMSManager m_instace = null;

    //    public static SMSManager Instance
    //    {
    //        get
    //        {
    //            if (m_instace == null)
    //                m_instace = new SMSManager();
    //            return m_instace;
    //        }
    //    }
    //    private string m_strWebServerURL = "";
    //    private SMSManager()
    //    {
    //        m_strWebServerURL = FormMain.Instance.DBManager.WebServerURL;           
    //    }

    //    // strMsg를 80바이트씩 자른다.
    //    private ArrayList MakeMessageList(string strMsg)
    //    {
    //        ArrayList arrMessages = new ArrayList();

    //        int nByteLength = 0;
    //        int nLen = strMsg.Length;
    //        int nBeginIndex = 0;

    //        for (int i = 0; i < nLen; i++)
    //        {
    //            if (strMsg.ElementAt(i) < 256)
    //                nByteLength++;
    //            else
    //                nByteLength += 2;

    //            if (nByteLength == 80 || 
    //                ((nByteLength == 79) && (i < nLen - 1 && strMsg.ElementAt(i + 1) >= 256)))
    //            {
    //                arrMessages.Add(strMsg.Substring(nBeginIndex, i - nBeginIndex + 1));
    //                nBeginIndex = i + 1;
    //                nByteLength = 0;
    //            }
    //        }

    //        if (nByteLength > 0)
    //        {
    //            arrMessages.Add(strMsg.Substring(nBeginIndex));
    //        }

    //        return arrMessages;
    //    }
       
    //    private static char ConvertToHex(char cSource)
    //    {
    //        return "0123456789abcdef"[0x0f & cSource];
    //    }

    //    public static string URLEncoding(byte[] bytes)
    //    {
    //        string strResult = "";

    //        foreach (byte element in bytes)
    //        {
    //            if ((element >= '0' && element <= '9') ||   // 숫자
    //                (element >= 'a' && element <= 'z') ||   // 소문자
    //                (element >= 'A' && element <= 'Z') ||   // 대문자
    //                (element == '!' || element == '*' || element == '(' || element == ')' || element == '_' || element == '-')) // 그 외의 특수기호들
    //            {
    //                strResult += (char)element;
    //            }
    //            else
    //            {
    //                strResult += "%";
    //                strResult += ConvertToHex((char)((int)element >> 4));
    //                strResult += ConvertToHex((char)element);
    //            }
    //        }

    //        return strResult;
    //    }


    //    private void RespCallback(IAsyncResult asynchronousResult)
    //    {

    //    }

    //    public string SendSMS(string strPhoneNumber, string strSendPhoneNumber, string strMsg)
    //    {

    //        ArrayList arrMessages = MakeMessageList(strMsg);
    //        foreach (string szMsg in arrMessages)
    //        {
    //            string resResult = string.Empty;
    //            string sourceUrl = m_strWebServerURL + "/SendSMS.jsp";

    //            Encoding enc = Encoding.UTF8;
    //            byte[] bytes1 = enc.GetBytes(szMsg);
    //            string strUrlEncode = URLEncoding(bytes1);

    //            // 테스트 : %c5%d7%bd%ba%c6%ae%0d%0a - ok
    //            // 테스트 : %c5%d7%bd%ba%c6%ae
    //            string postData = "Sender=" + strSendPhoneNumber + "&" + "Reciver=" + strPhoneNumber + "&" + "Msg=" + strUrlEncode;

    //            sourceUrl = sourceUrl + "?" + postData;
    //            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
    //            //Thread.Sleep(500);
    //            lock (this)
    //            {
    //                wReq.Method = "POST";
    //                try
    //                {
    //                    HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

    //                    // http 내용 추출
    //                    Stream respPostStream = wRes.GetResponseStream();
    //                    StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

    //                    resResult = readerPost.ReadToEnd();

    //                    readerPost.Close();

    //                    respPostStream.Close();
    //                }
    //                catch (System.Net.WebException e)
    //                {
    //                    System.Windows.Forms.MessageBox.Show(e.Message);
    //                    return "";
    //                }
    //            }

    //        }

    //        return "OK";
    //    }

    //    public bool SendSMS(ArrayList arrPhoneNumbers, string strSendPhoneNumber, string strMsg)
    //    {
    //        ArrayList arParam = new ArrayList();
    //        arParam.Add(arrPhoneNumbers);
    //        arParam.Add(strSendPhoneNumber);
    //        arParam.Add(strMsg);
    //        Thread t = new Thread(SendSMSThread);
    //        t.Start(arParam);
    //        return true;
    //    }


    //    public void SendSMSThread(object param)
    //    {
    //        ArrayList arParams = (ArrayList)param;
    //        ArrayList arrPhoneNumbers = (ArrayList)arParams[0];
    //        string strSendPhoneNumber = (string)arParams[1];
    //        string strMsg = (string)arParams[2];

    //        ArrayList arrMessages = MakeMessageList(strMsg);
    //        foreach (string szPhone in arrPhoneNumbers)
    //        {
    //            if (szPhone != null && !szPhone.Equals(""))
    //            {
    //                foreach (string szMsg in arrMessages)
    //                {
    //                    SendSMS(szPhone, strSendPhoneNumber, szMsg);
    //                }
    //                if( FormMain.Instance.Disposing || FormMain.Instance.IsDisposed)
    //                    break;
    //            } 
    //            //foreach (string szMsg in arrMessages)
    //            //{
    //            //    SendSMS(szPhone, strSendPhoneNumber, szMsg);
    //            //}
    //        }
    //    }
    //    //public bool SendSMS(ArrayList arrPhoneNumbers, string strSendPhoneNumber, string strMsg)
    //    //{

    //    //    ArrayList arrMessages = MakeMessageList(strMsg);
    //    //    foreach (string szPhone in arrPhoneNumbers)
    //    //    {
    //    //        if (szPhone != null && !szPhone.Equals(""))
    //    //        {
    //    //            foreach (string szMsg in arrMessages)
    //    //            {
    //    //                SendSMS(szPhone, strSendPhoneNumber, szMsg);
    //    //            }
    //    //        }                
    //    //    }
    //    //    return true;
    //    //}
    //}

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
        private libSMS.MessageClient m_msgClient = null;
        private log4net.ILog logger = null;

        private SMSManager()
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            m_strWebServerURL = FormMain.Instance.DBManager.WebServerURL;
        }


        private int m_nMessageLength = 80;

        // strMsg를 80바이트씩 자른다.
        private ArrayList MakeMessageList(string strMsg)
        {
            ArrayList arrMessages = new ArrayList();

            int nByteLength = 0;
            int nLen = strMsg.Length;
            int nBeginIndex = 0;

            //StringBuilder sb1 = new StringBuilder();
            //StringBuilder sb2 = new StringBuilder();
            for (int i = 0; i < nLen; i++)
            {
                if (strMsg.ElementAt(i) < 256)
                {
                    nByteLength++;
                    //sb1.Append(strMsg.ElementAt(i));
                    
                }
                else
                {
                    nByteLength += 2;
                    //sb2.Append(strMsg.ElementAt(i));
                }

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
            //System.Diagnostics.Trace.WriteLine("ANSI : " + sb1.ToString());
            //System.Diagnostics.Trace.WriteLine("한글 : " + sb2.ToString());

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

        public string SendSMS(string strPhoneNumber, string strSendPhoneNumber, string strMsg)
        {
            string ipAddress = SDMS.NetworkManager.Instance.ServerAddr;
            if (m_msgClient == null)
                m_msgClient = new libSMS.MessageClient(ipAddress);

            //if (NetworkServer.Instance.SimulationMode)
             //   strMsg = "[연습모드]" + strMsg;

            if (m_msgClient.SendSMS(strSendPhoneNumber, strPhoneNumber, strMsg))
            {
                return "OK";
            }
            return "";
        }

        public bool SendSMS(ArrayList arrMembers, string strSendPhoneNumber, string strMsg)
        {
            ArrayList arrMessages = MakeMessageList(strMsg);

            foreach (string szMsg in arrMessages)
            {
                foreach (string szPhone in arrMembers)
                {
                    //string szPhone = member.PhoneNumber;
                    if (szPhone != null && !szPhone.Equals(""))
                    {
                        SendSMS(szPhone, strSendPhoneNumber, szMsg);
                    }
                }
            }



            return true;
        }


        public bool SendSMSForPhoneNumber(ArrayList arrPhoneNumbers, int nBeginIndex, string strSendPhoneNumber, string strMsg)
        {
            if (nBeginIndex < 0)
                return false;

            ArrayList arrMessages = MakeMessageList(strMsg);
            int nPhoneNumberCount = arrPhoneNumbers.Count;

            foreach (string szMsg in arrMessages)
            {
                for (int i = nBeginIndex; i < nPhoneNumberCount; i++)
                {
                    if ((arrPhoneNumbers[i] is string) == false)
                        return false;

                    string szPhone = (string)arrPhoneNumbers[i];
                    if (szPhone != null && !szPhone.Equals(""))
                    {
                        SendSMS(szPhone, strSendPhoneNumber, szMsg);
                    }
                }
            }
            return true;
        }
    }
}
