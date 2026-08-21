using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;


namespace ControlMonitoring
{
    public class WebDBManager
    {
        //protected StringFile m_StringFile = new StringFile();
        private Utility m_ini = new Utility();
        private string m_strWebServerURL = "";

        private int m_nLevel = -1;
        private ArrayList m_arrUserInfo = new ArrayList();

        public WebDBManager()
        {
            Loadini_ServerConnectionInfo();
        }

        // User 권한
        public int Level
        {
            get { return m_nLevel; }
            set { m_nLevel = value; }
        }

        static public T GetField<T>(object dataSrc, T dataDefault)
        {
            T result;

            try
            {
                result = (T)dataSrc;
            }
            catch (Exception)
            {
                result = dataDefault;
            }

            return result;
        }

        static public float GetFloatField(string dataSrc, float fDefault)
        {
            float result;

            try
            {
                result = float.Parse(dataSrc);
            }
            catch (Exception)
            {
                result = fDefault;
            }

            return result;
        }

        // 문자열 앞뒤의 빈문자들을 제거한다.
        static public string GetStringField(object dataSrc, string strDefault)
        {
            string result;

            try
            {
                result = (string)dataSrc;
                result = result.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                result = result.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });

                // (char)6, 7, 8은 DB 입력시 '\n', '\r', '\''이 임시로 바뀌어 들어간 값이므로, 다시 '\n'으로 되돌려 준다.
                result = result.Replace((char)6, '\n');
                result = result.Replace((char)7, '\r');
                result = result.Replace((char)8, '\'');
            }
            catch (Exception)
            {
                result = strDefault;
            }

            return result;
        }

        static public DateTime GetDateTimeField(object dataSrc, DateTime dtDefault)
        {
            DateTime result;

            try
            {
                result = Convert.ToDateTime(dataSrc);
            }
            catch (Exception)
            {
                result = dtDefault;
            }

            return result;
        }

        static public int GetIntField(string dataSrc, int nDefault)
        {
            int result = nDefault;
            if (dataSrc == null || dataSrc == "null")
            {
                return result;
            }
            try
            {
                result = int.Parse(dataSrc);
            }
            catch (Exception)
            {
                result = nDefault;
            }

            return result;
        }

        public string GetReadDB(string strSQLQuery, int nTransaction)
        {
            string resResult = string.Empty;
            string sourceUrl = m_strWebServerURL + "/DBQuery2.jsp";
            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);
            string postData = "SQLQuery=" + strUrlEncode + "&" + "Transaction=" + nTransaction;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);

            lock (this)
            {
                wReq.CookieContainer = m_CookieContainer;
                wReq.Method = "POST";
                wReq.ContentType = "application/x-www-form-urlencoded";
                wReq.ContentLength = bytes.Length;
                
                try
                {
                    using (Stream writeStream = wReq.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                    HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();
                    Stream respPostStream = wRes.GetResponseStream();
                    StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

                    resResult = readerPost.ReadToEnd();
                }
                catch (System.Net.WebException)
                {                    
                    return "";
                }
            }
            return resResult;
        }

        public ArrayList GetResultData(string strSQLQuery, int nTransaction)
        {
            // str에 '\n', '\r'이 포함되어 있으면 다른 문자로 바꾼다.
            strSQLQuery = strSQLQuery.Replace('\n', (char)6);
            strSQLQuery = strSQLQuery.Replace('\r', (char)7);

            ArrayList arrResult = new ArrayList();
            string resResult = GetReadDB(strSQLQuery, nTransaction);

            //m_StringFile.SetData(resResult);
            StringFile strFile = new StringFile(resResult);

            string strResult = "";
            bool isResult = true;
            bool isBegin = false;

            while (isResult)
            {
                //isResult = m_StringFile.ReadLine(ref strResult);
                isResult = strFile.ReadLine(ref strResult);

                if (isResult)
                {
                    if (strResult == "Begin Data")
                    {
                        isBegin = true;
                        continue;
                    }
                    if (strResult == "End Data")
                        break;

                    if (isBegin)
                    {
                        if (strResult == "null_SQLError")
                        {
                            return null;
                        }
                        else
                            arrResult.Add(strResult);
                    }
                }
            }
            return arrResult;
        }

        private CookieContainer m_CookieContainer = new CookieContainer();
        //////////////////////////////////////////////////////////////////////////
        // StoredProcedure
        public string GetStoredProcedure(string strSQLQuery, int nTransaction)
        {
            string resResult = string.Empty;
            string sourceUrl = m_strWebServerURL + "/RunStoredProcedure2.jsp";

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "SQLQuery=" + strUrlEncode +"&" + "Transaction=" + nTransaction;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
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

            return resResult;
        }

        public ArrayList GetStoredProcedureData(string strSQLQuery, int nTransaction)
        {
            ArrayList arrResult = new ArrayList();
            string resResult = GetStoredProcedure(strSQLQuery, nTransaction);

            //m_StringFile.SetData(resResult);
            StringFile strFile = new StringFile(resResult);

            string strResult = "";
            bool isResult = true;
            bool isBegin = false;

            while (isResult)
            {
                //isResult = m_StringFile.ReadLine(ref strResult);
                isResult = strFile.ReadLine(ref strResult);

                if (isResult)
                {
                    if (strResult == "Begin Data")
                    {
                        isBegin = true;
                        continue;
                    }
                    if (strResult == "End Data")
                        break;

                    if (isBegin)
                        arrResult.Add(strResult);
                }
            }

            return arrResult;
        }

        public void RunStoredProcedure(string strProcName, ArrayList arrFields, ArrayList arrValues, int transaction, out ArrayList arrResult)
        {
            arrResult = null;

            int nFieldCount = arrFields.Count;
            int nValueCount = arrValues.Count;
            if (nFieldCount != nValueCount) return;

            string strSQL = strProcName;

            for (int i = 0; i < nValueCount; i++)
            {
                if (i == 0)
                    strSQL += " " + (string)arrValues[i];
                else
                    strSQL += "," + (string)arrValues[i];
            }

            arrResult = GetStoredProcedureData(strSQL, transaction);
        }

        // 해당문자열을 ``으로 감싸서 반환한다 (strQuary:DB이름이나 필드명)
        public string Grave(object obj)
        {
            return "`" + obj.ToString() + "`";
        }

        public void Loadini_ServerConnectionInfo()
        {
            string strSection = "Server Connection Info";

            m_strWebServerURL = m_ini.getinivalue(strSection, "webserver_url");
        }

        public string LoadIni(string strTargetName)
        {
            string strSection = "Server Connection Info";
            return m_ini.getinivalue(strSection, strTargetName);
        }

        public string LoadIni(string strTargetName, string strSectionName)
        {
            return m_ini.getinivalue(strSectionName, strTargetName);
        }

        private bool MakeBuildingZone()
        {
            string strSQL = "delete from Zone";
            if (GetResultData(strSQL, 0) == null)
                return false;

            strSQL = "select id, BuildingName, MaxFloor, MinFloor from Building";
            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            string strFloor = "";
            int nIndex = 0;
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                int nID = GetIntField(arrResult[i].ToString(), -1);
                string strBuildingName = GetStringField(arrResult[i + 1], "");
                int nMaxFloor = GetIntField(arrResult[i + 2].ToString(), -1);
                int nMinFloor = GetIntField(arrResult[i + 3].ToString(), -1);

                if (nID < 0)
                    continue;

                for (int j = nMinFloor; j <= nMaxFloor; j++)
                {
                    if (j < 0)
                        strFloor = string.Format(" 지하 {0}층", -j);
                    else
                        strFloor = string.Format(" {0}층", j + 1);

                    strSQL = string.Format("insert into Zone (ID, ZoneName, BuildingID, FloorIndex) values ({0}, '{1}', {2}, {3})",
                        ++nIndex, strBuildingName + strFloor, nID, j);

                    if (GetResultData(strSQL, 0) == null)
                        return false;
                }
            }

            return true;
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

        /*public int Load_Controller()
        {
            string strSQL = "SELECT ControlUser.UserID, ControlCheck.Time FROM ControlUser, ControlCheck WHERE ControlUser.UserID = ControlCheck.UserID and ControlCheck.ControlCheck <> 0";

            ArrayList arrResult = GetResultData(strSQL, 0);
            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;
            DateTime dtDefault = new DateTime();
            DateTime dtNow = DateTime.Now;

            if (nResultCount >= 2)
            {
                int nControlUserID = GetIntField(arrResult[0].ToString(), -1);
                DateTime dt = GetDateTimeField(arrResult[1], dtDefault);

                TimeSpan ts = dtNow - dt;
                if (ts.TotalMilliseconds > 15000) // 현재 로그인되지 않음
                {
                    
                }
                else
                {
                    return nControlUserID;
                }
            }
            return -1;
        }

     

        //접속중인 사용자를 검색
        public void Load_UserInfo(ref ArrayList arrUserInfo)
        {
            arrUserInfo.Clear();

            string strSQL = "SELECT ControlCheck.ID, ControlCheck.UserID, ControlCheck.Time, ControlCheck.ControlCheck, CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel, SOPGenLevel.LevelName " +
                            "FROM ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
                            "where ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID and SOPGenUser.UserLevel = SOPGenLevel.ID";

            ArrayList arrResult = GetResultData(strSQL, 0);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            DateTime dtDefault = new DateTime();

            DateTime dtNow = DateTime.Now;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = GetIntField(arrResult[i].ToString(), -1);
                if (nID < 0)
                    continue;

                ControllerInfo dataNew = new ControllerInfo();

                dataNew.ID = nID;
                dataNew.UserID = GetIntField(arrResult[i + 1].ToString(), -1);
                dataNew.Time = GetDateTimeField(arrResult[i + 2], dtDefault);
                dataNew.ControlCheck = GetIntField(arrResult[i + 3].ToString(), -1);
                dataNew.MemberName = GetStringField(arrResult[i + 4], "");
                dataNew.MemberID = GetStringField(arrResult[i + 5], "");
                dataNew.UserLevel = GetIntField(arrResult[i + 6].ToString(), -1);
                dataNew.LevelName = GetStringField(arrResult[i + 7], "");

                TimeSpan ts = dtNow - dataNew.Time;

                if (ts.TotalMilliseconds <= 15000)
                {
                    ControllerInfo user = FindUser(arrUserInfo, dataNew.UserID);

                    if (user == null)
                        arrUserInfo.Add(dataNew);
                    else
                    {
                        if (dataNew.Time > user.Time)
                        {
                            arrUserInfo.Remove(user);
                            arrUserInfo.Add(dataNew);
                        }
                    }
                }
            }
        }

        private ControllerInfo FindUser(ArrayList arrUsers, int nUserID)
        {
            foreach (ControllerInfo user in arrUsers)
            {
                if (user.UserID == nUserID)
                    return user;
            }

            return null;
        }*/

        // ControlCheck가 1인 UserID를 찾아 레벨이 높은자를 기준으로 정렬
        //private ArrayList FindRequestContol(ArrayList arrUserInfo)
        //{
        //    DateTime dtDefault = new DateTime();
        //    ArrayList arrController = new ArrayList();

        //    string strSQL = "SELECT ControlCheck.ID, ControlCheck.UserID, ControlCheck.Time, CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel " +
        //                    "FROM ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
        //                    "where ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID and SOPGenUser.UserLevel = SOPGenLevel.ID and  ControlCheck.ControlCheck = 1 " +
        //                    "order by SOPGenUser.UserLevel DESC";

        //    ArrayList arrResult = GetResultData(strSQL, 0);
        //    if (arrResult == null)
        //        return arrController;

        //    DateTime dtNow = DateTime.Now;

        //    int nResultCount = arrResult.Count;

        //    for (int i = 0; i < nResultCount - 5; i += 6)
        //    {
        //        int nID = GetIntField(arrResult[i].ToString(), -1);
        //        int nUserID = GetIntField(arrResult[i + 1].ToString(), -1);

        //        if (nID < 0 || nUserID < 0)
        //            continue;

        //        ControllerInfo data = new ControllerInfo();
        //        data.ID = nID;
        //        data.UserID = nUserID;
        //        data.Time = GetDateTimeField(arrResult[i + 2], dtDefault);

        //        TimeSpan ts = DateTime.Now - data.Time;
        //        if (ts.TotalMilliseconds > 15000)
        //        {
        //            continue;
        //        }

        //        data.ControlCheck = 1;
        //        data.MemberName = GetStringField(arrResult[i + 3], "");
        //        data.MemberID = GetStringField(arrResult[i + 4], "");
        //        data.UserLevel = GetIntField(arrResult[i + 5].ToString(), -1);

        //        arrController.Add(data);
        //    }

        //    /*string strSQL = "SELECT ControlCheck.ID, ControlCheck.UserID, ControlCheck.Time, ControlCheck.ControlCheck, CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel, SOPGenLevel.LevelName " +
        //                    "FROM ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
        //                    "where ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID and SOPGenUser.UserLevel = SOPGenLevel.ID and  ControlCheck.ControlCheck = 1 " +
        //                    "order by SOPGenUser.UserLevel DESC";

        //    ArrayList arrResult = GetResultData(strSQL, 0);
        //    if (arrResult == null)
        //        return arrController;

        //    int nResultCount = arrResult.Count;

        //    for (int i = 0; i < nResultCount - 7; i += 8)
        //    {
        //        int nID = GetIntField(arrResult[i].ToString(), -1);
        //        int nUserID = GetIntField(arrResult[i + 1].ToString(), -1);

        //        if (nID < 0 || nUserID < 0)
        //            continue;

        //        ControllerInfo data = new ControllerInfo();
        //        data.ID = nID;
        //        data.UserID = nUserID;
        //        data.Time = GetDateTimeField(arrResult[i + 2], dtDefault);

        //        TimeSpan ts = DateTime.Now - data.Time;
        //        if (ts.TotalMilliseconds > 15000)
        //        {
        //            continue;
        //        }

        //        data.ControlCheck = GetIntField(arrResult[i + 3].ToString(), -1);
        //        data.MemberName = GetStringField(arrResult[i + 4], "");
        //        data.MemberID = GetStringField(arrResult[i + 5], "");

        //        foreach (ControllerInfo info in arrUserInfo)
        //        {
        //            if (info.UserID == data.UserID)
        //            {
        //                arrController.Add(data);
        //                break;
        //            }
        //        }
        //    }*/

        //    return arrController;
        //}

        // ControlCheck가 -1인 UserID를 찾아 레벨이 높은자를 기준으로 정렬
        /*private ArrayList FindUser(ArrayList arrUserInfo)
        {
            DateTime dtDefault = new DateTime();
            ArrayList arrController = new ArrayList();

            string strSQL = "SELECT ControlCheck.ID, ControlCheck.UserID, ControlCheck.Time, ControlCheck.ControlCheck, CompanyMember.MemberName, CompanyMember.MemberID, SOPGenUser.UserLevel, SOPGenLevel.LevelName " +
                            "FROM ControlCheck, CompanyMember, SOPGenUser, SOPGenLevel " +
                            "where ControlCheck.UserID = SOPGenUser.ID AND CompanyMember.ID = SOPGenUser.MemberID and SOPGenUser.UserLevel = SOPGenLevel.ID and  ControlCheck.ControlCheck = -1 " +
                            "order by SOPGenUser.UserLevel DESC";

            ArrayList arrResult = GetResultData(strSQL, 0);
            if (arrResult == null)
                return arrController;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 7; i += 8)
            {
                int nID = GetIntField(arrResult[i].ToString(), -1);
                int nUserID = GetIntField(arrResult[i + 1].ToString(), -1);

                if (nID < 0 || nUserID < 0)
                    continue;

                ControllerInfo data = new ControllerInfo();
                data.ID = nID;
                data.UserID = nUserID;
                data.Time = GetDateTimeField(arrResult[i + 2], dtDefault);

                TimeSpan ts = DateTime.Now - data.Time;
                if (ts.TotalMilliseconds > 15000)
                {
                    continue;
                }
                data.ControlCheck = GetIntField(arrResult[i + 3].ToString(), -1);
                data.MemberName = GetStringField(arrResult[i + 4], "");
                data.MemberID = GetStringField(arrResult[i + 5], "");

                foreach (ControllerInfo info in arrUserInfo)
                {
                    if (info.UserID == data.UserID)
                    {
                        arrController.Add(data);
                        break;
                    }
                }
            }

            return arrController;
        }

        private void Update_Controller(int nUserID)
        {
            if (nUserID <= 0)
                return;

            ControllerInfo user = FindUser(m_arrUserInfo, nUserID);
            if (user == null)
                return;

            string strSQL = "";

            if (user.ControlCheck == 1)
                strSQL = string.Format("update ControlUser set UserID = {0}", nUserID);
            else
                strSQL = string.Format("update ControlUser set UserID = {0};update ControlCheck set ControlCheck = -1 where UserID = {0}", nUserID);

            GetResultData(strSQL, 0);
        }

        private void Update_Reset()
        {
            string strSQL = "update ControlCheck set ControlCheck = -1 where ControlCheck = 1";
            GetResultData(strSQL, 0);
        }

        // 제어권을 반납한 User들 가운데 마지막으로 반납한 User를 제외한 User들 중에서 한 User를 뽑는다.
        private int FindFinal(int nPrevControllerID)
        {
            string strSQL = "select userID, max(Time) from ControlCheck group by userID, ControlCheck having ControlCheck = 0";// and userID <> " + nPrevControllerID.ToString();
            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null)
                return -1;

            int nResultCount = arrResult.Count;

            int nUserID = -1;
            DateTime dtDefault = new DateTime();
            DateTime dtNow = DateTime.Now;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nID = GetIntField(arrResult[i].ToString(), -1);
                DateTime dtTime = GetDateTimeField(arrResult[i + 1], dtDefault);

                TimeSpan ts = dtNow - dtTime;
                if (ts.TotalMilliseconds > 15000)
                    continue;

                nUserID = nID;

                if (nUserID != nPrevControllerID)
                    return nUserID;
            }

            // 제어권 반납자 가운데 현재 로그인한 User가 없을 경우 방금 반납한 User에게 다시 제어권이 돌아간다.
            return nUserID;
        }

        public int TakeControl(int nPrevControllerID)
        {
            int nControllerID = Load_Controller();
            if (nControllerID < 0)
            {
                Load_UserInfo(ref m_arrUserInfo); //현재 로그인한 사용자 중
                if (m_arrUserInfo.Count == 0) return -1;

                ArrayList arrRequestControl = new ArrayList();
                arrRequestControl = FindRequestContol(m_arrUserInfo); //제어권 요청자를 찾음

                if (arrRequestControl.Count == 0) //제어권 요청자가 없는 경우
                {
                    ArrayList arrUser = new ArrayList();
                    arrUser = FindUser(m_arrUserInfo);

                    if (arrUser.Count == 0) // 제어권 반납자들만 존재할 경우 반납자들 가운데 직전 반납자를 제외한 반납자들 가운데 한곳에 제어권을 넘겨준다.
                    {
                        int nUserID = FindFinal(nPrevControllerID);
                        if (nUserID > 0)
                        {
                            Update_Controller(nUserID);
                            return nUserID;
                        }
                        //return;
                    }
                    else //모니터링 사용자를 찾아 제어권을 강제로 넘겨줌
                    {
                        foreach (ControllerInfo data in arrUser)
                        {
                            Update_Controller(data.UserID);
                            return data.UserID;
                        }
                    }

                    return -1;
                }
                else //flag = 1인 사용자가 있는 경우
                {
                    foreach (ControllerInfo data in arrRequestControl)
                    {
                        Update_Controller(data.UserID);
                        Update_Reset();

                        return data.UserID;
                    }

                    return -1;
                }
            }

            return nControllerID;
        }*/
    }
}
