using System.Collections.Generic;
using System.Collections;
using DBUtility2;
using System.Configuration;
using System;

namespace SOP_SMS
{
    public class MemberManager
    {
        private enum ManagerType { CompanyMember = 0, RegularTeam, ExternalCompanyMember, ExternalCompanyTeam };
        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

        public static bool ReadActionStepHistoryInfo(int nActionStepHistoryID, out string strPosition, out string strTime, out string strSOPMode)
        {
            strPosition = "";
            strSOPMode = "";
            strTime = "";

            int nSiteID;
            string strSiteID = ConfigurationManager.AppSettings.Get("siteid");

            if (int.TryParse(strSiteID, out nSiteID) == false)
                return false;

            WebDBManager dbMgr = new WebDBManager(nSiteID);

            string strSQL = "Select DetectTime, RealMode, Position from ActionStepHistory where ID = " + nActionStepHistoryID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 3)
                return false;

            VariousData<DateTime> detectTime = WebDBManager.GetDateTimeField(arrResult[0]);
            VariousData<int> realMode = WebDBManager.GetIntField(arrResult[1].ToString());
            strPosition = WebDBManager.GetStringField(arrResult[2]);

            if (detectTime == null || realMode == null || strPosition == null)
                return false;

            strTime = string.Format("{0}월 {1}일 {2}시 {3}분", detectTime.Data.Month, detectTime.Data.Day, detectTime.Data.Hour, detectTime.Data.Minute);
            strSOPMode = realMode.Data == 1 ? "실제" : "훈련";

            return true;
        }

        // Key : 전화번호
        // Value : 전화번호 소유자의 ID(CompanyMember이면 양수, ExternalCompanyMember이면 음수)
        public static Dictionary<string, int> GetMemberList(int nSensorType)
        {
            int nSiteID;
            string strSiteID = ConfigurationManager.AppSettings.Get("siteid");

            if (int.TryParse(strSiteID, out nSiteID) == false)
                return null;

            WebDBManager dbMgr = new WebDBManager(nSiteID);
            return GetMemberList(dbMgr, nSensorType);
        }

        private static Dictionary<string, int> GetMemberList(WebDBManager dbMgr, int nSensorType)
        {
            string strSQL = "Select MemberID, MemberType from FacilityManager where FacilityType = " + nSensorType.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            Dictionary<string, int> dicMembers = new Dictionary<string, int>();

            string strPhoneNumber;
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> memberType = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (memberID == null || memberType == null)
                    continue;

                if (memberType.Data == (int)ManagerType.CompanyMember)
                {
                    if (GetCompanyMember(dbMgr, memberID.Data, out strPhoneNumber))
                        dicMembers[strPhoneNumber] = memberID.Data;
                }
                else if (memberType.Data == (int)ManagerType.RegularTeam)
                {
                    GetRegularTeamMembers(dbMgr, memberID.Data, dicMembers);
                }
                else if (memberType.Data == (int)ManagerType.ExternalCompanyMember)
                {
                    if (GetExternalCompanyMember(dbMgr, memberID.Data, out strPhoneNumber))
                        dicMembers[strPhoneNumber] = -memberID.Data;
                }
                else if (memberType.Data == (int)ManagerType.ExternalCompanyTeam)
                {
                    GetExternalTeamMembers(dbMgr, memberID.Data, dicMembers);
                }
            }

            return dicMembers;
        }

        private static bool GetExternalTeamMembers(WebDBManager dbMgr, int nTeamID, Dictionary<string, int> dicMembers)
        {
            string strSQL = "Select member.ID, member.PhoneNumber from ExternalMemberList as eml, ExternalTeam as team, ExternalCompanyMember as member ";
            strSQL += "where eml.ExternalCompanyTeamID = team.ID and eml.ExternalCompanyMemberID = member.ID and team.ID = " + nTeamID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 1]);

                if (memberID == null || strPhoneNumber == null || strPhoneNumber.Length == 0)
                    continue;

                strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);
                dicMembers[strPhoneNumber] = -memberID.Data;
            }

            return true;
        }

        private static bool GetExternalCompanyMember(WebDBManager dbMgr, int nMemberID, out string strPhoneNumber)
        {
            strPhoneNumber = null;

            string strSQL = "Select PhoneNumber from ExternalCompanyMember where ID = " + nMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            strPhoneNumber = WebDBManager.GetStringField(arrResult[0]);

            if (strPhoneNumber == null || strPhoneNumber.Length == 0)
                return false;

            strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);
            return true;
        }

        private static bool GetRegularTeamMembers(WebDBManager dbMgr, int nTeamID, Dictionary<string, int> dicMembers)
        {
            string strSQL = "Select member.ID, member.PhoneNumber from RegularMemberList as rml, RegularTeam as team, CompanyMember as member ";
            strSQL += "where rml.RegularTeamID = team.ID and rml.CompanyMemberID = member.ID and team.ID = " + nTeamID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> memberID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strPhoneNumber = WebDBManager.GetStringField(arrResult[i + 1]);

                if (memberID == null || strPhoneNumber == null || strPhoneNumber.Length == 0)
                    continue;

                strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);
                dicMembers[strPhoneNumber] = memberID.Data;
            }

            return true;
        }

        private static bool GetCompanyMember(WebDBManager dbMgr, int nMemberID, out string strPhoneNumber)
        {
            strPhoneNumber = null;

            string strSQL = "Select PhoneNumber from CompanyMember where ID = " + nMemberID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            strPhoneNumber = WebDBManager.GetStringField(arrResult[0]);

            if (strPhoneNumber == null || strPhoneNumber.Length == 0)
                return false;

            strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);
            return true;
        }
    }
}
