using CrisisAlertAPI.BLL.Models;
using CrisisAlertAPI.BLL.Response;
using dnsDBUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrisisAlertAPI.BLL
{
    public class SMSManager
    {
        private WebDBManager m_dbManager = null;

        private int m_msgBufCount = 100;
        private int m_nSMSLimit = 90;

        private const string m_strCaller = "027144133";
        private const string m_strUserID = "une9966";

        public SMSManager(WebDBManager dbManager)
        {
            m_dbManager = dbManager;
        }

        public MessageResult SendSMS(SmsParameter parameter)
        {
            MessageResult result = new MessageResult();
            string strErrorMessage = "";

            string[] arrPhoneNumbers = parameter.PhoneNumbers.Split(',');
            List<string> phoneNumbers = new List<string>();

            foreach (string phoneNum in arrPhoneNumbers)
            {
                phoneNumbers.Add(phoneNum);
            }

            bool isSMS = IsSMSMessage(parameter.Message, m_nSMSLimit);

            int nReceiverCount = phoneNumbers.Count;

            for (int i = 0; i < nReceiverCount;)
            {
                int nEndIndex = i + m_msgBufCount;

                if (nEndIndex >= nReceiverCount)
                    nEndIndex = nReceiverCount;

                if (isSMS)
                {
                    if (SendSMSMessage(phoneNumbers, parameter.Message, i, nEndIndex, out strErrorMessage) == false)
                    {
                        result.Success = false;
                        result.Message = strErrorMessage;
                        return result;
                    }
                }
                else
                {
                    string strTitle = "";
                    CheckTitle(ref strTitle, parameter.Message);

                    if (SendLMSMessage(phoneNumbers, parameter.Message, strTitle, i, nEndIndex, out strErrorMessage) == false)
                    {
                        result.Success = false;
                        result.Message = strErrorMessage;
                        return result;
                    }
                }

                i = nEndIndex;
            }

            result.Success = true;
            result.Message = "성공적으로 메시지를 보냈습니다.";
            return result;
        }

        public static void CheckTitle(ref string strTitle, string strMessage)
        {
            if (strTitle.Length > 0)
                return;

            if (strMessage.Length <= 5)
                strTitle = strMessage;
            else
                strTitle = strMessage.Substring(0, 5);
        }

        public bool SendSMSMessage(List<string> phoneNumberList, string strMessage, int nBeginIndex, int nEndIndex, out string strErrorMessage)
        {
            strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbManager, m_dbManager.DatabaseName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_SMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, SMS_MSG, CALLBACK_URL, NOW_DATE, SEND_DATE, CALLBACK, DEST_TYPE, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, STD_ID) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', NULL, NULL, '{2}', '{3}', 0, {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            if (m_dbManager.GetResultData(strSQL, m_dbManager.DatabaseName) != null)
                return true;

            strErrorMessage = "[libSMS]MessageBrokerMCS.SendSMSMessage Fail : " + m_dbManager.LastErrorMessage;
            return false;
        }

        public bool SendLMSMessage(List<string> phoneNumberList, string strMessage, string strTitle, int nBeginIndex, int nEndIndex, out string strErrorMessage)
        {
            strErrorMessage = "";

            string strTime = GetCurrentTime(m_dbManager, m_dbManager.DatabaseName);
            string strReceiverInfo = GetReceiverInfo(phoneNumberList, nBeginIndex, nEndIndex);
            strMessage = CheckQuotation(strMessage);

            string strSQL = "Insert into SDK_MMS_SEND (USER_ID, SCHEDULE_TYPE, SUBJECT, MMS_MSG, NOW_DATE, SEND_DATE, CALLBACK, DEST_COUNT, DEST_INFO, KT_OFFICE_CODE, CDR_ID, RESERVED1, RESERVED2, RESERVED3, RESERVED4, RESERVED5, RESERVED6, RESERVED7, RESERVED8, RESERVED9, ";
            strSQL += "SEND_STATUS, SEND_COUNT, SEND_RESULT, SEND_PROC_TIME, MSG_TYPE, STD_ID, CONTENT_COUNT, CONTENT_DATA) ";
            strSQL += string.Format("values ('{0}', 0, NULL, '{1}', '{2}', '{2}', '{3}', {4}, '{5}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, 0, NULL, 0, NULL)",
                m_strUserID, strMessage, strTime, m_strCaller, phoneNumberList.Count, strReceiverInfo);

            if (m_dbManager.GetResultData(strSQL, m_dbManager.DatabaseName) != null)
                return true;

            strErrorMessage = "[libSMS]MessageBrokerMCS.SendLMSMessage Fail : " + m_dbManager.LastErrorMessage;
            return false;
        }

        string GetCurrentTime(WebDBManager dbMgr, string strDBName)
        {
            string strTime = "";


            if (dbMgr.DatabaseType == WebDBManager.DBType.mysql)
            {
                System.Collections.ArrayList arrResult = dbMgr.GetResultData("SELECT current_date(), current_time()", strDBName);

                if (arrResult == null || arrResult.Count < 2)
                    return "";

                string strDate = WebDBManager.GetStringField(arrResult[0]);
                strTime = strDate + WebDBManager.GetStringField(arrResult[1]);
            }
            else
            {
                DateTime dtNow = DateTime.Now;
                strTime = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
                return strTime;
            }

            strTime = strTime.Replace("-", "");
            strTime = strTime.Replace(":", "");
            strTime = strTime.Replace(" ", "");

            int nIndex = strTime.LastIndexOf('.');

            if (nIndex >= 0)
                strTime = strTime.Substring(0, nIndex);

            return strTime;
        }

        public bool IsSMSMessage(string strMsg, int nSMSLimit)
        {
            int nByteLength = 0;
            int nLen = strMsg.Length;

            for (int i = 0; i < nLen; i++)
            {
                if (strMsg.ElementAt(i) < 256)
                    nByteLength++;
                else
                    nByteLength += 2;
            }

            if (nByteLength <= nSMSLimit)
                return true;

            return false;
        }

        private string GetReceiverInfo(List<string> phoneNumberList, int nBeginIndex, int nEndIndex)
        {
            string strReceivers = "";

            for (int i = nBeginIndex; i < nEndIndex; i++)
            {
                if (strReceivers.Length == 0)
                    strReceivers = "a^" + phoneNumberList[i];
                else
                    strReceivers += "|a^" + phoneNumberList[i];
            }

            return strReceivers;
        }

        string CheckQuotation(string strMessage)
        {
            return strMessage.Replace("'", "''");
        }
    }
}
