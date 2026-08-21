using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using DBUtility;

namespace UnE
{
    namespace SOP
    {
        namespace SMS
        {
            public class SMSManager : IDisposable
            {
                protected static SMSManager m_instance = null;
                public static SMSManager Instance
                {
                    get { return m_instance; }
                }

                private bool m_bExit = false;
                public bool Exit
                {
                    get { return m_bExit; }
                    set { m_bExit = value; }
                }

                private bool m_bUseSMS = false;
                public bool UseSMS
                {
                    get { return m_bUseSMS; }
                    set { m_bUseSMS = value; }
                }


                private string m_strWebServerURL = "";
                public string WebServerURL
                {
                    get { return m_strWebServerURL; }
                    set { m_strWebServerURL = value; }
                }

                private libSMS.IMessageClient m_msgClient = null;
                // 우선적으로 문자메시지를 수신할 전화번호들
                private ArrayList m_vipPhoneNumbers = new ArrayList();

                public SMSManager()
                {
                }

                public void Dispose()
                {
                    m_bExit = true;
                    if (smsThread != null && smsThread.ThreadState == ThreadState.Running)
                    {
                        try
                        {
                            smsThread.Join();
                        }
                        catch (System.Exception)
                        {
                        }
                    }
                }

                // 우선적으로 문자메시지를 받을 사람들을 지정한다.
                public void SetVIPPhoneNumbers(WebDBManager dbMgr, int nSiteID)
                {
                    m_vipPhoneNumbers.Clear();
                    string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'FirstSMSTeamID' and SiteID = " + nSiteID.ToString();
                    ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

                    if (arrResult == null || arrResult.Count == 0)
                        return;

                    string strTeamIDs = WebDBManager.GetStringField(arrResult[0]);

                    if (strTeamIDs == null)
                        return;

                    if (strTeamIDs == null || strTeamIDs.Length == 0)
                        return;

                    strSQL = "Select cm.PhoneNumber from RegularMemberList as rml, CompanyMember as cm ";
                    strSQL += "where rml.RegularTeamID in (" + strTeamIDs + ") and rml.CompanyMemberID = cm.ID and cm.PhoneNumber is not null";
                    arrResult = dbMgr.GetResultData(strSQL, 0);

                    if (arrResult == null)
                        return;

                    string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

                    Dictionary<string, string> dicVIPNumbers = new Dictionary<string, string>();

                    foreach (object obj in arrResult)
                    {
                        string strEncrypt = WebDBManager.GetStringField(obj);

                        if (strEncrypt == null)
                            continue;

                        try
                        {
                            string strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strEncrypt, key);
                            strPhoneNumber = strPhoneNumber.Replace("-", "").Trim();
                            strPhoneNumber = strPhoneNumber.Replace(" ", "");

                            dicVIPNumbers[strPhoneNumber] = strPhoneNumber;
                        }
                        catch(Exception ex)
                        {
                            System.Diagnostics.Trace.WriteLine(ex.Message);
                            System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                        }
                     
                    }

                    foreach (KeyValuePair<string, string> pair in dicVIPNumbers)
                    {
                        m_vipPhoneNumbers.Add(pair.Value);
                    }
                }

                private int m_nMessageLength = 80;

                // strMsg를 80바이트씩 자른다.
                private ArrayList MakeMessageList(string strMsg)
                {
                    strMsg = strMsg.Replace((char)6, '/');

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
                            ((nByteLength == (m_nMessageLength-1)) && (i < nLen - 1 && strMsg.ElementAt(i + 1) >= 256)))
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

                private string SendSMS(string strPhoneNumber, string strSendPhoneNumber, string strMsg)
                {
                    if (m_msgClient == null)
                    {                        
                        int nIndex1 = m_strWebServerURL.IndexOf("http://");
                        int nIndex2 = m_strWebServerURL.LastIndexOf(':');
                        string strURL = m_strWebServerURL;

                        if (nIndex1 >= 0 && nIndex2 >= 0)
                        {
                            int nBeginIndex = nIndex1 + "http://".Length;
                            strURL = m_strWebServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
                        }
                        else if (nIndex1 >= 0)
                        {
                            int nBeginIndex = nIndex1 + "http://".Length;
                            strURL = m_strWebServerURL.Substring(nBeginIndex);
                        }
                        else if (nIndex2 >= 0)
                        {
                            strURL = m_strWebServerURL.Substring(0, nIndex2);
                        }

                        if (strURL.Length == 0)
                            return "";

                        System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);

                        if (addr == null || addr.Length == 0)
                            return "";

                        string strServerAddr = addr[0].ToString();


                        m_msgClient = libSMS.MessageClientFactory.CreateMessageClient(UnE.SOP.ProxySOP.Instance.SiteID, strServerAddr);
                       
                        
                    }                    
                    if (m_msgClient.SendSMS(strSendPhoneNumber, strPhoneNumber, strMsg))
                        return "OK";

                    return "";
                }

                private string SendSMS(List<libSMS.MessageContent> arMessages)
                {
                    if (CreateMessageClient() == false)
                        return "";

                    if (m_msgClient.SendSMS(arMessages))
                        return "OK";

                    return "";
                }

                private bool CreateMessageClient()
                {
                    if (m_msgClient == null)
                    {
                        int nIndex1 = m_strWebServerURL.IndexOf("http://");
                        int nIndex2 = m_strWebServerURL.LastIndexOf(':');
                        string strURL = m_strWebServerURL;

                        if (nIndex1 >= 0 && nIndex2 >= 0)
                        {
                            int nBeginIndex = nIndex1 + "http://".Length;
                            strURL = m_strWebServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
                        }
                        else if (nIndex1 >= 0)
                        {
                            int nBeginIndex = nIndex1 + "http://".Length;
                            strURL = m_strWebServerURL.Substring(nBeginIndex);
                        }
                        else if (nIndex2 >= 0)
                        {
                            strURL = m_strWebServerURL.Substring(0, nIndex2);
                        }

                        if (strURL.Length == 0)
                            return false;

                        System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);

                        if (addr == null || addr.Length == 0)
                            return false;

                        string strServerAddr = addr[0].ToString();


                        m_msgClient = libSMS.MessageClientFactory.CreateMessageClient(UnE.SOP.ProxySOP.Instance.SiteID, strServerAddr);


                    }

                    return true;
                }

                private Thread smsThread = null;

                public bool SendSMS(ArrayList arrPhoneNumbers, string strSendPhoneNumber, string strMsg)
                {
                    if (m_bUseSMS == false)
                        return true;

                    if (m_strWebServerURL == "")
                        return false;

                    if (ProxySOP.Instance.SimulationMode)
                    {
                        arrPhoneNumbers = GetSimulationPhoneNumbers();

                        if (arrPhoneNumbers == null || arrPhoneNumbers.Count == 0)
                            return false;

                        strMsg = "[연습모드]" + strMsg;
                    }

                    ArrayList arParam = new ArrayList();
                    arParam.Add(arrPhoneNumbers);
                    arParam.Add(strSendPhoneNumber);
                    arParam.Add(strMsg);
                    smsThread = new Thread(SendSMSThread);
                    smsThread.Name = "SMSSender";
                    smsThread.Start(arParam);
                    return true;
                }

                protected virtual ArrayList GetSimulationPhoneNumbers()
                {
                    return null;
                }

                public void SendSMSThread(object param)
                {
                    ArrayList arParams = (ArrayList)param;
                    ArrayList arrPhoneNumbers = (ArrayList)arParams[0];
                    string strSendPhoneNumber = (string)arParams[1];
                    string strMsg = (string)arParams[2];

                    double nTimeDelay = 1.1;

                    if (CreateMessageClient() == false)
                        return;

                    m_nMessageLength = m_msgClient.GetMessageLength();
                    ArrayList arrMessages = MakeMessageList(strMsg);
                    //arrMessages.Reverse();

                    List<libSMS.MessageContent> arrSendContent = new List<libSMS.MessageContent>();
                    DateTime dtTimeTag = DateTime.Now;

                    if (m_vipPhoneNumbers.Count == 0)
                    {
                        foreach (string szMsg in arrMessages)
                        {
                            foreach (string szPhone in arrPhoneNumbers)
                            {
                                if (szPhone != null && !szPhone.Equals(""))
                                {
                                    //SendSMS(szPhone, strSendPhoneNumber, szMsg);
                                    //System.Diagnostics.Trace.WriteLine(szPhone + ", " + strSendPhoneNumber + ", " + szMsg);
                                    libSMS.MessageContent content = new libSMS.MessageContent();
                                    content.Message = szMsg;
                                    content.Caller = strSendPhoneNumber;
                                    content.Reciver = szPhone;
                                    content.EncryptCaller = false;
                                    content.SmsTag = dtTimeTag.ToLongTimeString();

                                    arrSendContent.Add(content);

                                    if (m_bExit == true)
                                        return;
                                }
                            }

                            dtTimeTag.AddSeconds(nTimeDelay);

                            System.Diagnostics.Trace.WriteLine(szMsg);
                        }

                        SendSMS(arrSendContent);
                    }
                    else
                    {
                        RemoveVIPMembers(arrPhoneNumbers);

                        foreach (string szMsg in arrMessages)
                        {
                            foreach (string szPhone in m_vipPhoneNumbers)
                            {
                                if (szPhone != null && !szPhone.Equals(""))
                                {
                                    libSMS.MessageContent content = new libSMS.MessageContent();
                                    content.Message = szMsg;
                                    content.Caller = strSendPhoneNumber;
                                    content.Reciver = szPhone;
                                    content.EncryptCaller = false;
                                    content.SmsTag = dtTimeTag.ToLongTimeString();

                                    arrSendContent.Add(content);

                                    if (m_bExit == true)
                                        return;
                                }
                            }

                            dtTimeTag.AddSeconds(nTimeDelay);
                        }

                        SendSMS(arrSendContent);

                        System.Threading.Thread.Sleep(1000);
                        arrSendContent = new List<libSMS.MessageContent>();

                        foreach (string szMsg in arrMessages)
                        {
                            foreach (string szPhone in arrPhoneNumbers)
                            {
                                if (szPhone != null && !szPhone.Equals(""))
                                {
                                    libSMS.MessageContent content = new libSMS.MessageContent();
                                    content.Message = szMsg;
                                    content.Caller = strSendPhoneNumber;
                                    content.Reciver = szPhone;
                                    content.EncryptCaller = false;
                                    content.SmsTag = dtTimeTag.ToLongTimeString();

                                    arrSendContent.Add(content);

                                    if (m_bExit == true)
                                        return;
                                }
                            }

                            dtTimeTag.AddSeconds(nTimeDelay);
                            System.Diagnostics.Trace.WriteLine(szMsg);
                        }

                        SendSMS(arrSendContent);
                    }
                }

                private void RemoveVIPMembers(ArrayList arrPhoneNumbers)
                {
                    Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();

                    foreach (string strPhoneNumber in arrPhoneNumbers)
                    {
                        dicPhoneNumbers[strPhoneNumber] = strPhoneNumber;
                    }

                    foreach (string strPhoneNumber in m_vipPhoneNumbers)
                    {
                        dicPhoneNumbers.Remove(strPhoneNumber);
                    }

                    arrPhoneNumbers.Clear();

                    foreach (KeyValuePair<string, string> pair in dicPhoneNumbers)
                    {
                        arrPhoneNumbers.Add(pair.Value);
                    }
                }
            }
        }
    }
}
