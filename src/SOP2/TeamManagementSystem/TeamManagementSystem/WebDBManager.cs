using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace TeamManagementSystem
{
    public class WebDBManager
    {
        protected StringFile m_StringFile = new StringFile();
        private FormMain m_Main = null;
        private Utility m_ini = new Utility();
        private string m_strWebServerURL = "";

        private int m_nLevel = -1;

        public WebDBManager(FormMain main)
        {
            m_Main = main;
            Loadini_ServerConnectionInfo();
        }

        // User 권한
        public int Level
        {
            get { return m_nLevel; }
            set { m_nLevel = value; }
        }

        public ArrayList GetDisasterCategoryName()
        {
            ArrayList arrCategory = new ArrayList();
            ReadDB_TableDisasterCategory(ref arrCategory);

            return arrCategory;
        }

        public T GetField<T>(object dataSrc, T dataDefault)
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

        // 문자열 앞뒤의 빈문자들을 제거한다.
        public string GetStringField(object dataSrc, string strDefault)
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

        public DateTime GetDateTimeField(object dataSrc, DateTime dtDefault)
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

        public int GetIntField(string dataSrc, int nDefault)
        {
            int result;

            bool isSuccess = int.TryParse(dataSrc, out result);
            if (isSuccess)
                return result;

            return nDefault;

            /*try
            {
                result = int.Parse(dataSrc);
            }
            catch (Exception)
            {
                result = nDefault;
            }

            return result;*/
        }

        public void ReadDB_TableDisasterCategory(ref ArrayList arrCategory)
        {
            arrCategory.Clear();

            string strSql = "SELECT ID, CategoryName FROM DisasterCategory";
            ArrayList arrResult = GetResultData(strSql, 0);

            for (int i=0;i<arrResult.Count-1;i+=2)
            {
                Data_DispasterCategory dataNew = new Data_DispasterCategory();
                dataNew.ID = GetIntField(arrResult[i].ToString(), 0);
                dataNew.CategoryName = GetStringField(arrResult[i+1].ToString(), "");

                arrCategory.Add(dataNew);
            }
        }

        /*public void Execute(string strSQL, SqlTransaction transaction = null)
        {
            SqlCommand cmd = new SqlCommand(strSQL, m_dbConnection);
            if (transaction != null) cmd.Transaction = transaction;
            cmd.ExecuteNonQuery();
        }*/

        public string GetReadDB(string strSQLQuery, int nTransaction)
        {
            string resResult = string.Empty;
            //string m_sourceUrl = "http://localhost:8088/SOP/Login.jsp";
            string sourceUrl = m_strWebServerURL + "/DBQuery2.jsp";
            
            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            //string strBase64 = Convert.ToBase64String(bytes1);
            //string strUrlEncode = System.Web.HttpUtility.UrlEncode(bytes1);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "SQLQuery=" + strUrlEncode + "&" + "Transaction=" + nTransaction;
            //string postData = "SQLQuery=" + strSQLQuery + "&" + "Transaction=" + nTransaction;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.Method = "POST";
            //wReq.UserAgent = "Mozilla/4.0";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            //wReq.CookieContainer = new CookieContainer();

            try
            {
                using (Stream writeStream = wReq.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

                // http 내용 추출
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

                resResult = readerPost.ReadToEnd();
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show(e.Message);
                return "";
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

            m_StringFile.SetData(resResult);

            string strResult = "";
            bool isResult = true;
            bool isBegin = false;

            while (isResult)
            {
                isResult = m_StringFile.ReadLine(ref strResult);

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
                        arrResult.Add(strResult);
                    }
                }
            }

            return arrResult;
        }

        //////////////////////////////////////////////////////////////////////////
        // StoredProcedure
        public string GetStoredProcedure(string strSQLQuery, int nTransaction)
        {
            string resResult = string.Empty;
            //string sourceUrl = "http://localhost:8088/SOP/RunStoredProcedure.jsp";
            string sourceUrl = m_strWebServerURL + "/RunStoredProcedure2.jsp";
            string postData = "SQLQuery=" + strSQLQuery + "&" + "Transaction=" + nTransaction;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.Method = "POST";
            //wReq.UserAgent = "Mozilla/4.0";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            //wReq.CookieContainer = new CookieContainer();

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

            m_StringFile.SetData(resResult);

            string strResult = "";
            bool isResult = true;
            bool isBegin = false;

            while (isResult)
            {
                isResult = m_StringFile.ReadLine(ref strResult);

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

        public void Save_OrganizationChart()
        {

        }

        // History Table에서 값을 읽어와서 NormalTeam 또는 EmergencyTeam에 넣는다.
        public bool RollBackTeam(bool isNormalTeam, int nBeginHistoryTeamID, int nEndHistoryTeamID)
        {
            string strTeamName = isNormalTeam ? "TemporaryNormalTeam" : "TemporaryEmergencyTeam";
            string strHistoryTeamName = strTeamName + "History";

            string strSQL = string.Format("select id from {0}", strTeamName);
            ArrayList arrResult = this.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            ArrayList arrOldID = new ArrayList();

            foreach (object obj in arrResult)
            {
                int nID = this.GetIntField(obj.ToString(), -1);
                if (nID > 0)
                    arrOldID.Add(nID);
            }

            strSQL = string.Format("select ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink from {0} where id >= {1} and id <= {2}",
                strHistoryTeamName, nBeginHistoryTeamID, nEndHistoryTeamID);

            arrResult = this.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nOldCount = arrOldID.Count;
            int nTeamID = nOldCount;

            int nResultCount = arrResult.Count;
            int nBeginID = -1;

            for (int i = 0; i < nResultCount-6; i+=7)
            {
                int nIndex = i / 7;

                int nID = this.GetIntField(arrResult[i].ToString(), -1);
                string _strTeamName = this.GetStringField(arrResult[i + 1], "null");
                int nParentTeamID = this.GetIntField(arrResult[i + 2].ToString(), -1);
                string strGroupName = this.GetStringField(arrResult[i + 3], "null");
                int nLevelNo = this.GetIntField(arrResult[i + 4].ToString(), -1);
                string strDesc = this.GetStringField(arrResult[i + 5], "null");
                string strRegularLink = this.GetStringField(arrResult[i + 6], "null");

                if (nBeginID < 0)
                    nBeginID = nID;

                if (nParentTeamID > 0)
                    nParentTeamID = nParentTeamID - nBeginID + 1;
                
                if (nIndex < nOldCount)
                {
                    strSQL = string.Format("Update {0} set TeamName = '{1}', ParentTeamID = {2}, GroupName = {3}, LevelNo = {4}, Description = {5}, RegularTeamLink = {6} where id = {7}",
                        strTeamName, _strTeamName, nParentTeamID >= 0 ? nParentTeamID.ToString() : "NULL", strGroupName == "null" ? "NULL" : "'" + strGroupName + "'",
                        nLevelNo >= 0 ? nLevelNo.ToString() : "NULL", strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                        strRegularLink == "null" ? "NULL" : "'" + strRegularLink + "'", (int)arrOldID[nIndex]);
                }
                else
                {
                    strSQL = string.Format("Insert into {0} (ID, TeamName, ParentTeamID, GroupName, LevelNo, Description, RegularTeamLink) values ({1}, '{2}', {3}, {4}, {5}, {6}, {7})",
                        strTeamName, ++nTeamID, _strTeamName, nParentTeamID >= 0 ? nParentTeamID.ToString() : "NULL", strGroupName == "null" ? "NULL" : "'" + strGroupName + "'",
                        nLevelNo >= 0 ? nLevelNo.ToString() : "NULL", strDesc == "null" ? "NULL" : "'" + strDesc + "'",
                        strRegularLink == "null" ? "NULL" : "'" + strRegularLink + "'");
                }

                if (this.GetResultData(strSQL, 0) == null)
                    return false;
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

    }
}
