using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlTeamEditor
{
    public static class VaildMemberPhoneNumber
    {
        //private static int m_nLockDB = 0;
        private static object m_lockObj = new object();

        //private static int m_nPrevHour = -1, m_nPrevMinute = -1, m_nPrevSecond = -1;

        /*private static bool m_DB_Loaded = false;
        /// <summary>
        /// DataBase 로딩여부
        /// </summary>
        public static bool DB_Loaded
        {
            set
            {
                if (value == true)
                {
                    m_dicValidPhoneNumbers = GetValidPhoneNumbers();
                }
                else
                {
                    m_dicValidPhoneNumbers = null;
                }

                m_DB_Loaded = value;
            }
            get { return m_DB_Loaded; }
        }*/

        private static bool m_isNeedToUpdateWorkingMemberData = false;

        private static Dictionary<bool, ArrayList> m_dicValidPhoneNumbers = null;
        //private static Dictionary<bool, ArrayList> m_dicLastLoadingValidPhoneNumbers = null;


        //public static void LoadDB()
        //{
        //    lock (m_lockObj)
        //    {
        //        if (m_nLockDB <= 0)
        //            m_dicValidPhoneNumbers = GetValidPhoneNumbers();

        //        m_nLockDB++;
        //    }
        //}

        //public static void ReleaseDB()
        //{
        //    lock (m_lockObj)
        //    {
        //        m_nLockDB--;

        //        if (m_nLockDB <= 0)
        //            m_dicValidPhoneNumbers = null;
        //    }
        //}

        /// <summary>
        /// 입력받은 핸드폰 번호가 유효한 번호인지 확인 (현재 근무조에 해당되는 번호)
        /// </summary>
        /// <param name="strPhoneNumber"></param>
        /// <returns></returns>
        public static bool IsVaildPhoneNumber(string strPhoneNumber, WebDBManager dbMgr)
        {
            bool bReturn = true;
            Dictionary<bool, ArrayList> dicValidPhoneNumbers = null;

            lock (m_lockObj)
            {
                dicValidPhoneNumbers = GetValidPhoneNumbers(dbMgr);
            }

            // 비번근무자에 해당되는 전화번호인지...
            if (dicValidPhoneNumbers[true].Contains(ValidPhoneNumber(strPhoneNumber)) == true)
            {
                // 비번근무자이지만 현재근무조의 대근자일수도 있으므로 다시한번 확인
                if (dicValidPhoneNumbers[true].Contains(ValidPhoneNumber(strPhoneNumber)) == false)
                {
                    bReturn = false;
                }
            }

            return bReturn;
        }
        
        /// <summary>
        /// 핸드폰 번호목록에서 유효한 핸드폰 번호만 리턴 (현재 근무조에 해당되는 번호)
        /// </summary>
        /// <param name="arrPhoneNumbers">검사할 전화번호 목록</param>
        /// <returns></returns>
        public static ArrayList IsVaildPhoneNumber(ArrayList arrPhoneNumbers, WebDBManager dbMgr)
        {
            // SMS 전달을 제외할 핸드폰 번호 목록
            ArrayList arrDelNumbers = new ArrayList();
            Dictionary<bool, ArrayList> dicValidPhoneNumbers = null;

            lock (m_lockObj)
            {
                dicValidPhoneNumbers = GetValidPhoneNumbers(dbMgr);
            }

            // 유효한 전화번호인지 검사
            foreach (string strPhoneNumber in arrPhoneNumbers)
            {
                // 비번근무자에 해당되는 전화번호인지...
                if (dicValidPhoneNumbers[false].Contains(ValidPhoneNumber(strPhoneNumber)) == true)
                {
                    // 비번근무자이지만 현재근무조의 대근자일수도 있으므로 다시한번 확인
                    if (dicValidPhoneNumbers[true].Contains(ValidPhoneNumber(strPhoneNumber)) == false)
                    {
                        arrDelNumbers.Add(strPhoneNumber);
                    }
                }

            }

            // 비번자에 해당되는 전화번호 지우기
            foreach (string strDelNumber in arrDelNumbers)
            {
                arrPhoneNumbers.Remove(strDelNumber);
            }

            //System.Diagnostics.Trace.WriteLine(String.Format("Valid PhoneNumber Count : {0}", arrPhoneNumbers.Count));

            return arrPhoneNumbers;
        }

        /// <summary>
        /// 근무조 데이터 갱신이 필요할 경우 호출.
        /// </summary>
        public static void NeedToUpdateWorkingMemberData()
        {
            m_isNeedToUpdateWorkingMemberData = true;
        }

        /// <summary>
        /// 유효한 핸드폰 번호 모두 리턴 (현재 근무조의 핸드폰 번호 True : 현재 근무조  False : 비번 근무조)
        /// </summary>
        /// <returns></returns>
        private static Dictionary<bool, ArrayList> GetValidPhoneNumbers(WebDBManager dbMgr)
        {
            if (m_dicValidPhoneNumbers != null)
            //if (m_DB_Loaded == true)
            {
                // 근무조 데이터가 변경되었는지 확인하고 변경된 경우에만 다시 로드
                if (m_isNeedToUpdateWorkingMemberData == false)
                    return m_dicValidPhoneNumbers;
            }

            DateTime dtNow = DateTime.Now;

            //// 마지막 DB 갱신 시점으로부터 1초가 지나지 않았으면 마지막 로딩한 데이터를 그대로 사용한다.
            //if (m_nPrevHour == dtNow.Hour && m_nPrevMinute == dtNow.Minute && m_nPrevSecond == dtNow.Second && m_dicLastLoadingValidPhoneNumbers != null)
            //    return m_dicLastLoadingValidPhoneNumbers;

            //WebDBManager dbMgr = FormMemberWorkSchedule.Instance.DBManager;
 
            string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });

            Dictionary<bool, ArrayList> dicReturn = new Dictionary<bool, ArrayList>();
            dicReturn.Add(true, new ArrayList());
            dicReturn.Add(false, new ArrayList());

            string strIFNull = dbMgr.DatabaseType == WebDBManager.DBType.sqlserver ? "ISNULL" : "IFNULL";

            string strSQL = string.Empty;
            strSQL += "SELECT A.ID ";
            strSQL += ", " + strIFNull + "(C.PhoneNumber, D.PhoneNumber) AS PHONENUMBER ";
            strSQL += ", CASE WHEN B.ID IS NULL THEN 0 ELSE 1 END AS IS_WORKING ";
            strSQL += "FROM ControlTeamMembers AS A ";
            strSQL += "LEFT JOIN ControlWorkingTeam AS B ";
            strSQL += "ON (A.RoomID = B.RoomID AND A.TeamID = B.TeamID) ";
            strSQL += "OR (A.RoomID = B.RoomID AND A.RoomID = 8 AND B.TeamID IS NULL) ";        // RoomID 8 은 당직 근무자
            strSQL += "LEFT JOIN CompanyMember AS C ";
            strSQL += "ON (A.MemberID = C.ID AND A.MemberType = 1) ";
            strSQL += "LEFT JOIN ExternalCompanyMember AS D ";
            strSQL += "ON (A.MemberID = D.ID AND A.MemberType = 4) ";

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult != null)
            {
                for (int nIndex = 0; nIndex < arrResult.Count; nIndex += 3)
                {
                    string strPhoneNumber = WebDBManager.GetStringField(arrResult[nIndex + 1], "");

                    if (string.Compare(strPhoneNumber, "null", true) == 0 || strPhoneNumber == "")
                        strPhoneNumber = "";
                    else
                        strPhoneNumber = AES256Cipher.AES_decrypt(strPhoneNumber, key);

                    strPhoneNumber = ValidPhoneNumber(strPhoneNumber);

                    if (String.IsNullOrWhiteSpace(strPhoneNumber) == false)
                    {
                        dicReturn[String.Equals(arrResult[nIndex + 2].ToString(), "1")].Add(strPhoneNumber);
                    }
                }
            }

            //m_dicLastLoadingValidPhoneNumbers = dicReturn;

            //string strTime = string.Format("{0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);

            //System.Diagnostics.Trace.WriteLine(string.Format("{0}, validPhoneNumber Count : {1}", strTime, arrResult.Count));

            //m_nPrevHour = dtNow.Hour;
            //m_nPrevMinute = dtNow.Minute;
            //m_nPrevSecond = dtNow.Second;

            m_dicValidPhoneNumbers = dicReturn;

            m_isNeedToUpdateWorkingMemberData = false;

            return dicReturn;
        }

        /// <summary>
        /// 전화번호를 비교할 수 있도록 변환
        /// </summary>
        /// <param name="strPhoneNumber"></param>
        /// <returns></returns>
        private static string ValidPhoneNumber(string strPhoneNumber)
        {
            string strResult = "";
            int nLen = strPhoneNumber.Length;

            for (int i = 0; i < nLen; i++)
            {
                char ch = strPhoneNumber[i];

                if (ch != ' ' && ch != '\t' && ch != '-')
                    strResult += ch;
            }
            return strResult;
        }


    }
}