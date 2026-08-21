using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace SOP_SMS
{
    public class SMSManager
    {
        public static bool SendSMS(Dictionary<string, int> dicMembers, int nActionStepHistoryID, string strMessage, bool withURL)
        {
            int nSiteID;
            string strSiteID = ConfigurationManager.AppSettings.Get("siteid");

            if (int.TryParse(strSiteID, out nSiteID) == false)
                return false;

            //string strURL = ConfigurationManager.AppSettings.Get("url");

            //NetworkWebManager mgr = new NetworkWebManager(nSiteID, strURL, nActionStepHistoryID, strMessage);
            //mgr.SetMembers(dicMembers);

            WebDBManager dbMgr = new WebDBManager(nSiteID);

            string strBaseURL = ConfigurationManager.AppSettings.Get("url");
            /*int nID = GetMaxID(dbMgr, "UnEMCSMessage");

            if (nID < 0)
                return false;
            else
                nID++;*/

            foreach (KeyValuePair<string, int> pair in dicMembers)
            {
                string strPhoneNumber = pair.Key;
                strPhoneNumber = strPhoneNumber.Replace("-", "");
                int nMemberID = pair.Value;

                //string strActionStepHistoryID = PersonalSOP.Common.ParameterManager.IDtoString(nActionStepHistoryID);
                //string strUserID = PersonalSOP.Common.ParameterManager.IDtoString(nMemberID);
                //string strURL = strBaseURL + string.Format("?ash={0}&uid={1}", strActionStepHistoryID, strUserID);
                string strURL = strBaseURL + string.Format("?ash={0}&uid={1}", nActionStepHistoryID, nMemberID);
                string strMMSMessage = withURL ? strMessage + "\r\n" + strURL : strMessage;

                strMMSMessage = strMMSMessage.Replace("'", "''");

                string strSQL = string.Format("Insert into UnEMCSMessage (ID, PhoneNumbers, Message, TimeStamp) Select ISNULL(max(ID) + 1, 1), '{0}', '{1}', getdate() from UnEMCSMessage",
                    /*nID++, */strPhoneNumber, strMMSMessage);

                if (dbMgr.GetResultData(strSQL) == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool SendMMS(Dictionary<string, int> dicMembers, int nActionStepHistoryID, string strMessage, string strImage, bool withURL)
        {            
            int nSiteID;
            string strSiteID = ConfigurationManager.AppSettings.Get("siteid");

            if (int.TryParse(strSiteID, out nSiteID) == false)
                return false;

            WebDBManager dbMgr = new WebDBManager(nSiteID);

            string strBaseURL = ConfigurationManager.AppSettings.Get("url");
            /*int nID = GetMaxID(dbMgr, "UnEMCSMessage");

            if (nID < 0)
                return false;
            else
                nID++;*/

            foreach (KeyValuePair<string, int> pair in dicMembers)
            {
                string strPhoneNumber = pair.Key;
                strPhoneNumber = strPhoneNumber.Replace("-", "");
                int nMemberID = pair.Value;

                //string strActionStepHistoryID = PersonalSOP.Common.ParameterManager.IDtoString(nActionStepHistoryID);
                //string strUserID = PersonalSOP.Common.ParameterManager.IDtoString(nMemberID);
                //string strURL = strBaseURL + string.Format("?ash={0}&uid={1}", strActionStepHistoryID, strUserID);
                string strURL = strBaseURL + string.Format("?ash={0}&uid={1}", nActionStepHistoryID, nMemberID);
                string strMMSMessage = withURL ? strMessage + "\r\n" + strURL : strMessage;

                strMMSMessage = strMMSMessage.Replace("'", "''");

                string strSQL = string.Format("Insert into UnEMCSMessage (ID, PhoneNumbers, Message, Image, TimeStamp) Select ISNULL(max(ID) + 1, 1), '{0}', '{1}', '{2}', getdate() from UnEMCSMessage",
                    /*nID++, */strPhoneNumber, strMMSMessage, strImage);

                if (dbMgr.GetResultData(strSQL) == null)
                {
                    return false;
                }
            }

            return true;
        }

        private static int GetMaxID(WebDBManager dbMgr, string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return 0;

            return id.Data;
        }
    }
}
