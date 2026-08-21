using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
//using DBUtility;

namespace EarthquakeSensorServer
{
    internal class SOPSMS
    {
        private static int m_nSiteID = 1;
        // 우선적으로 문자메시지를 받을 사람들의 휴대전화 번호
        private static ArrayList m_vipPhoneNumbers = new ArrayList();
        private static string m_strBroadcastMessage = "";

        public static void RunBroadcast(/*WebDBManager dbMgr,*/ int nSiteID)
        {
            /*if (m_strBroadcastMessage != null && m_strBroadcastMessage.Length > 0)
            {
                UnE.SOP.ProxySOP.Instance.SiteID = nSiteID;
                UnE.SOP.TTS.TTSManager.Instance.DBMgr = dbMgr;
                UnE.SOP.TTS.TTSManager.Instance.UseBroadcast = true;
                UnE.SOP.TTS.TTSManager.Instance.AddSpeech(m_strBroadcastMessage, 1, true);
                m_strBroadcastMessage = "";

                FormMain.Instance.EnableBroadcast(false);
            }*/
        }

        public static void SetVipPhoneNumbers(int nSiteID/*, WebDBManager dbMgr*/)
        {
            /*m_vipPhoneNumbers.Clear();

            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'FirstSMSTeamID' and SiteID = " + nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strTeamIDs = WebDBManager.GetStringField(arrResult[0]);

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
                    //m_vipPhoneNumbers.Add(strPhoneNumber);

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
            }*/
        }

        public static void SendSecondSMS(/*WebDBManager dbMgr, */int nComponentID)
        {
            /*string strSQL = "Select BroadcastMessage from internaltransmission where ID = " + nComponentID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strMessage = WebDBManager.GetStringField(arrResult[0]);

            if (strMessage == null || strMessage.Length == 0)
                return;

            int nIndex = strMessage.IndexOf('{');

            if (nIndex >= 0)
            {
                int nIndex2 = strMessage.IndexOf('}', nIndex + 1);

                if (nIndex2 > nIndex)
                {
                    string strTag = strMessage.Substring(nIndex, nIndex2 - nIndex + 1);
                    string strTime = string.Format("{0}시 {1}분", DateTime.Now.Hour, DateTime.Now.Minute);

                    strMessage = strMessage.Replace(strTag, strTime);
                }
            }

            Dictionary<string, string> dicPhoneNumbers = GetAllCompanyMemberPhoneNumbers(dbMgr);

            if (dicPhoneNumbers == null)
                return;

            List<string> messageList = new List<string>();
            messageList.Add(strMessage);

            ArrayList arParam = new ArrayList();
            arParam.Add(dicPhoneNumbers);
            arParam.Add("07088983203");
            arParam.Add(messageList);
            arParam.Add(dbMgr);

            System.Threading.Thread smsThread = new System.Threading.Thread(SendSMSThread);
            smsThread.Name = "SMSSender";
            smsThread.Start(arParam);*/
        }

        public static void SendSecondBroadcast(/*WebDBManager dbMgr, */int nComponentID)
        {
            /*string strSQL = "Select BroadcastMessage from internaltransmission where ID = " + nComponentID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strMessage = WebDBManager.GetStringField(arrResult[0]);

            m_strBroadcastMessage = strMessage;

            FormMain.Instance.EnableBroadcast(true);*/
        }

        public static void SendSOPSMS(int nSiteID/*, WebDBManager dbMgr*/, int nIntensity, float fMagnitude)
        {
            /*m_nSiteID = nSiteID;

            string strSQL = "Select PropertyValue from OptionSOPSimulator where PropertyName = 'EarthquakeLinkedSOP' and SiteID = " + nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return;

            string strSOPLink = WebDBManager.GetStringField(arrResult[0]);

            if (strSOPLink == null)
                return;

            string strCategoryName = "", strSubCategoryName = "", strDisasterName = "";

            if (GetSOPInfo(strSOPLink, ref strCategoryName, ref strSubCategoryName, ref strDisasterName) == false)
                return;

            int nStepMemberID = GetStepMemberID(dbMgr, strCategoryName, strSubCategoryName, strDisasterName);

            if (nStepMemberID < 0)
                return;

            GetBroadcastMessage(dbMgr, nStepMemberID, nIntensity, fMagnitude);
            GetInternalMessage(dbMgr, nStepMemberID, nIntensity, fMagnitude);*/
        }

        private static bool GetInternalMessage(/*WebDBManager dbMgr, */int nStepMemberID, int nIntensity, float fMagnitude)
        {
            // 문자메시지 정렬 문제로 여러개로 분할된 문자메시지
           /* List<string> messageList = new List<string>();
            Dictionary<string, string> dicPhoneNumbers = null;

            string strSQL = "Select ID, BroadcastMessage, TeamList from InternalTransmission where useMobileApp = 1 and StepMemberID = " + nStepMemberID.ToString() + " and AutoRun = 1 order by ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                string strMessage = WebDBManager.GetStringField(arrResult[i + 1]);
                string strTeamList = WebDBManager.GetStringField(arrResult[i + 2]);

                if (strMessage == null || strTeamList == null)
                    return false;

                UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strMessage, DateTime.Now, "");
                strMessage = UnE.SOP.Utility.SOPSimulatorScript.Parse(param);

                if (nIntensity > 0)
                    strMessage = strMessage.Replace("{earthq_intens}", nIntensity.ToString());

                if (fMagnitude > 0.0f)
                    strMessage = strMessage.Replace("{earthq_magnit}", string.Format("{0:F1}", fMagnitude));

                messageList.Add(strMessage);

                if (dicPhoneNumbers == null)
                    dicPhoneNumbers = GetAllCompanyMemberPhoneNumbers(dbMgr);
                //ArrayList arrPhoneNumbers = GetAllCompanyMemberPhoneNumbers(dbMgr);
            }

            if (dicPhoneNumbers == null)
                return true;

            ArrayList arParam = new ArrayList();
            arParam.Add(dicPhoneNumbers);
            arParam.Add("07088983203");
            arParam.Add(messageList);
            arParam.Add(dbMgr);

            System.Threading.Thread smsThread = new System.Threading.Thread(SendSMSThread);
            smsThread.Name = "SMSSender";
            smsThread.Start(arParam);*/

            return true;
        }

        /*private static bool GetBroadcastMessage(WebDBManager dbMgr, int nStepMemberID, int nIntensity, float fMagnitude)
        {
            string strSQL = "Select BroadcastMessage from InternalTransmission where useBroadcast = 1 and StepMemberID = " + nStepMemberID.ToString() + " and AutoRun = 1 order by ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            string strMessage = WebDBManager.GetStringField(arrResult[0]);
            m_strBroadcastMessage = strMessage;

            FormMain.Instance.EnableBroadcast(true);
            return true;
        }

        private static void SendSMSThread(object param)
        {
            ArrayList arParams = (ArrayList)param;
            Dictionary<string, string> dicPhoneNumbers = (Dictionary<string, string>)arParams[0];
            string strSendPhoneNumber = (string)arParams[1];
            List<string> messageList = (List<string>)arParams[2];
            //string strMsg = (string)arParams[2];
            WebDBManager dbMgr = (WebDBManager)arParams[3];

            double nTimeDelay = 1.1;

            libSMS.IMessageClient msgClient = CreateMessageClient(dbMgr);

            if (msgClient == null)
                return;

            ArrayList arrPhoneNumbers = RemoveVIPMembers(dicPhoneNumbers);

            string strSMSTag = FormMain.Instance.GetSMSTag();

            int nMessageCount = messageList.Count;

            for (int i = 0; i < nMessageCount; i++)
            {
                string strMsg = messageList[i];

                if (i == 0 && strSMSTag.Length > 0)
                    strMsg = strSMSTag + strMsg;

                int nMessageLength = msgClient.GetMessageLength();
                ArrayList arrMessages = MakeMessageList(strMsg, nMessageLength);
                //arrMessages.Reverse();

                SendSMS(arrMessages, m_vipPhoneNumbers, strSendPhoneNumber, nTimeDelay, dbMgr, msgClient);
                // 메시지 정렬을 위하여 수신 간격을 최소 1초 이상 두도록 한다.
                System.Threading.Thread.Sleep(5000);
            }

            string strFullMessage = "";

            for (int i = 0; i < nMessageCount; i++)
            {
                string strMsg = messageList[i];
                strFullMessage += strMsg;

                if (i == 0 && strSMSTag.Length > 0)
                    strMsg = strSMSTag + strMsg;

                int nMessageLength = msgClient.GetMessageLength();
                ArrayList arrMessages = MakeMessageList(strMsg, nMessageLength);
                //arrMessages.Reverse();

                SendSMS(arrMessages, arrPhoneNumbers, strSendPhoneNumber, nTimeDelay, dbMgr, msgClient);
                // 메시지 정렬을 위하여 수신 간격을 최소 1초 이상 두도록 한다.
                System.Threading.Thread.Sleep(5000);
            }

            SetSMSDBHistory(strFullMessage, dbMgr);
        }

        private static void SendSMS(ArrayList arrMessages, ArrayList arrPhoneNumbers, string strSendPhoneNumber, double nTimeDelay, WebDBManager dbMgr, libSMS.IMessageClient msgClient)
        {
            List<libSMS.MessageContent> arrSendContent = new List<libSMS.MessageContent>();
            DateTime dtTimeTag = DateTime.Now;

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
                    }
                }

                dtTimeTag.AddSeconds(nTimeDelay);
            }

            SendSMS(arrSendContent, dbMgr, msgClient);
        }

        private static ArrayList RemoveVIPMembers(Dictionary<string, string> dicPhoneNumbers)
        {
            foreach (string strPhoneNumber in m_vipPhoneNumbers)
            {
                dicPhoneNumbers.Remove(strPhoneNumber);
            }

            ArrayList arrPhoneNumbers = new ArrayList();

            foreach (KeyValuePair<string, string> pair in dicPhoneNumbers)
            {
                arrPhoneNumbers.Add(pair.Value);
            }

            return arrPhoneNumbers;
        }*/

        /*private static bool GetInternalMessage(WebDBManager dbMgr, int nStepMemberID, int nIntensity, float fMagnitude)
        {
            string strSQL = "Select BroadcastMessage, TeamList from InternalTransmission where useMobileApp = 1 and StepMemberID = " + nStepMemberID.ToString() + " and AutoRun = 1";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count < 2)
                return false;

            string strMessage = WebDBManager.GetStringField(arrResult[0]);
            string strTeamList = WebDBManager.GetStringField(arrResult[1]);

            if (strMessage == null || strTeamList == null)
                return false;

            UnE.SOP.Utility.SOPSimulatorScript.DataParameter param = new UnE.SOP.Utility.SOPSimulatorScript.DataParameter(strMessage, DateTime.Now, "");
            strMessage = UnE.SOP.Utility.SOPSimulatorScript.Parse(param);

            if (nIntensity > 0)
                strMessage = strMessage.Replace("{earthq_intens}", nIntensity.ToString());

            if (fMagnitude > 0.0f)
                strMessage = strMessage.Replace("{earthq_magnit}", string.Format("{0:F1}", fMagnitude));

            ArrayList arrPhoneNumbers = GetAllCompanyMemberPhoneNumbers(dbMgr);

            ArrayList arParam = new ArrayList();
            arParam.Add(arrPhoneNumbers);
            arParam.Add("07088983115");
            arParam.Add(strMessage);
            arParam.Add(dbMgr);

            System.Threading.Thread smsThread = new System.Threading.Thread(SendSMSThread);
            smsThread.Name = "SMSSender";
            smsThread.Start(arParam);

            return true;
        }

        private static void SendSMSThread(object param)
        {
            ArrayList arParams = (ArrayList)param;
            ArrayList arrPhoneNumbers = (ArrayList)arParams[0];
            string strSendPhoneNumber = (string)arParams[1];
            string strMsg = (string)arParams[2];
            WebDBManager dbMgr = (WebDBManager)arParams[3];

            double nTimeDelay = 1.1;

            libSMS.IMessageClient msgClient = CreateMessageClient(dbMgr);

            if (msgClient == null)
                return;

            string strSMSTag = FormMain.Instance.GetSMSTag();

            if (strSMSTag.Length > 0)
                strMsg = strSMSTag + strMsg;

            int nMessageLength = msgClient.GetMessageLength();
            ArrayList arrMessages = MakeMessageList(strMsg, nMessageLength);
            //arrMessages.Reverse();

            List<libSMS.MessageContent> arrSendContent = new List<libSMS.MessageContent>();
            DateTime dtTimeTag = DateTime.Now;

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
                    }
                }

                dtTimeTag.AddSeconds(nTimeDelay);
            }

            SendSMS(arrSendContent, dbMgr, msgClient);
            SetSMSDBHistory(strMsg, dbMgr);
        }*/

        /*private static void SetSMSDBHistory(string strMsg, WebDBManager dbMgr)
        {
            string strSQL = "Select ID from OptionSOPSimulator where PropertyName = 'LastSMSMessage' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            DateTime dtNow = DateTime.Now;
            int nID = 0;
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            string strBody = strTime + "," + strMsg;

            if (arrResult == null || arrResult.Count == 0)
            {
                strSQL = "Select max(ID) from OptionSOPSimulator";
                arrResult = dbMgr.GetResultData(strSQL, 0);

                if (arrResult == null || arrResult.Count == 0)
                    nID = 1;
                else
                {
                    nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1) + 1;

                    if (nID < 0)
                        nID = 1;
                }

                strSQL = "Insert into OptionSOPSimulator (ID, PropertyName, PropertyValue, Description, SiteID) values (";
                strSQL += string.Format("{0}, 'LastSMSMessage', '{1}', '마지막으로 발송된 문자메시지', {2})", nID, strBody, m_nSiteID);
                dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                nID = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

                if (nID > 0)
                {
                    strSQL = string.Format("Update OptionSOPSimulator set PropertyValue = '{0}' where ID = {1}", strBody, nID);
                    dbMgr.GetResultData(strSQL, 0);
                }
            }

            FormMain.Instance.SetSMSTime(DateTime.Now);
        }

        private static string SendSMS(List<libSMS.MessageContent> arMessages, WebDBManager dbMgr, libSMS.IMessageClient msgClient)
        {
            if (msgClient.SendSMS(arMessages))
                return "OK";

            return "";
        }

        // strMsg를 80바이트씩 자른다.
        private static ArrayList MakeMessageList(string strMsg, int nMessageLength)
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

                if (nByteLength == nMessageLength ||
                    ((nByteLength == (nMessageLength - 1)) && (i < nLen - 1 && strMsg.ElementAt(i + 1) >= 256)))
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

        private static libSMS.IMessageClient CreateMessageClient(WebDBManager dbMgr)
        {
            libSMS.IMessageClient msgClient = null;
            string strWebServerURL = dbMgr.WebServerURL;

            if (msgClient == null)
            {
                int nIndex1 = strWebServerURL.IndexOf("http://");
                int nIndex2 = strWebServerURL.LastIndexOf(':');
                string strURL = strWebServerURL;

                if (nIndex1 >= 0 && nIndex2 >= 0)
                {
                    int nBeginIndex = nIndex1 + "http://".Length;
                    strURL = strWebServerURL.Substring(nBeginIndex, nIndex2 - nBeginIndex);
                }
                else if (nIndex1 >= 0)
                {
                    int nBeginIndex = nIndex1 + "http://".Length;
                    strURL = strWebServerURL.Substring(nBeginIndex);
                }
                else if (nIndex2 >= 0)
                {
                    strURL = strWebServerURL.Substring(0, nIndex2);
                }

                if (strURL.Length == 0)
                    return null;

                System.Net.IPAddress[] addr = System.Net.Dns.GetHostAddresses(strURL);

                if (addr == null || addr.Length == 0)
                    return null;

                string strServerAddr = addr[0].ToString();

                msgClient = libSMS.MessageClientFactory.CreateMessageClient(m_nSiteID, strServerAddr);
            }

            return msgClient;
        }

        private static Dictionary<string, string> GetAllCompanyMemberPhoneNumbers(WebDBManager dbMgr)
        {
            string strSQL = "Select PhoneNumber from CompanyMember where PhoneNumber is not NULL";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            Dictionary<string, string> dicPhoneNumbers = new Dictionary<string, string>();

            if (arrResult == null)
                return dicPhoneNumbers;

            string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

            foreach (object obj in arrResult)
            {
                string strPhoneNumber = WebDBManager.GetStringField(obj);
                strPhoneNumber = DBUtility.AES256Cipher.AES_decrypt(strPhoneNumber, key);
                strPhoneNumber = strPhoneNumber.Replace("-", "");
                strPhoneNumber = strPhoneNumber.Replace(" ", "").Trim();

                if (strPhoneNumber != null)
                    dicPhoneNumbers[strPhoneNumber] = strPhoneNumber;
            }

            return dicPhoneNumbers;
        }

        private static int GetStepMemberID(WebDBManager dbMgr, string strCategoryName, string strSubCategoryName, string strDisasterName)
        {
            string strSQL = "Select ActionStep.ID, StepMember.ID from ActionStep, StepMember where StepMember.ActionStepID = ActionStep.ID and ActionStep.DisasterID = (Select ID from Disaster where VersionID = (Select max(d.VersionID) ";
            strSQL += "from DisasterCategory as dc, SubDisasterCategory as sdc, Disaster as d ";
            strSQL += "where d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and ";
            strSQL += "dc.CategoryName = '" + strCategoryName + "' and sdc.SubCategoryName = '" + strSubCategoryName + "' and d.DisasterName = '" + strDisasterName + "')) ";
            strSQL += "and StepName = '대응'";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count < 2)
                return -1;

            VariousData<int> actionStepID = WebDBManager.GetIntField(arrResult[0].ToString());
            VariousData<int> stepMemberID = WebDBManager.GetIntField(arrResult[1].ToString());

            if (actionStepID == null || stepMemberID == null)
                return -1;

            return stepMemberID.Data;
        }

        private static bool GetSOPInfo(string strSOPLink, ref string strCategoryName, ref string strSubCategoryName, ref string strDisasterName)
        {
            int nIndex1 = strSOPLink.IndexOf('/');

            if (nIndex1 < 0)
                return false;

            int nIndex2 = strSOPLink.IndexOf('/', nIndex1 + 1);

            if (nIndex2 < 0)
                return false;

            strCategoryName = strSOPLink.Substring(0, nIndex1);
            strSubCategoryName = strSOPLink.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
            strDisasterName = strSOPLink.Substring(nIndex2 + 1);
            return true;
        }*/
    }
}
