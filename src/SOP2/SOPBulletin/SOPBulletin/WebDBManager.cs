using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SOPBulletin
{
    public class WebDBManager
    {
        protected StringFile m_StringFile = new StringFile();
        private Utility m_ini = new Utility();
        private string m_strWebServerURL = "";
        private string m_strSirenPath = "";
        private string m_strDoorBellPath = "";
        private ServerTime m_timeServer = new ServerTime();
        private string m_strDefSOPName = "";

        private int m_nLevel = -1;

        public WebDBManager()
        {
            Loadini_ServerConnectionInfo();
            LoadTimeServerInfo();
            m_strSirenPath = LoadIni("siren_file");
            m_strDoorBellPath = LoadIni("doorbell_file");
            m_strDefSOPName = LoadIni("def_sop_name");
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

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);

            lock (this)
            {
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

        //////////////////////////////////////////////////////////////////////////
        // StoredProcedure
        public string GetStoredProcedure(string strSQLQuery, int nTransaction)
        {
            string resResult = string.Empty;
            //string sourceUrl = "http://localhost:8088/SOP/RunStoredProcedure.jsp";
            string sourceUrl = m_strWebServerURL + "/RunStoredProcedure2.jsp";
            //string postData = "SQLQuery=" + strSQLQuery + "&" + "Transaction=" + nTransaction;

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "SQLQuery=" + strUrlEncode +"&" + "Transaction=" + nTransaction;

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

        public void LoadTimeServerInfo()
        {
            string strSection = "Server Connection Info";
            m_timeServer.TimeServerAddress = m_ini.getinivalue(strSection, "time_server");

            try
            {
                m_timeServer.AddTime = double.Parse(m_ini.getinivalue(strSection, "add_time"));
            }
            catch (Exception)
            {
            }
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

        public string SirenPath
        {
            get { return m_strSirenPath; }
        }

        public string DoorBellPath
        {
            get { return m_strDoorBellPath; }
        }

        public SOPBulletin.ServerTime ServerTime
        {
            get { return m_timeServer; }
        }

        public string DefSOPName
        {
            get { return m_strDefSOPName; }
        }
    }

    public class ServerTime
    {
        private string m_strTimeServerAddress = "";
        // Time Server 표준시와의 보정 시간
        private double m_dAddTime = 0.0;

        public string TimeServerAddress
        {
            get { return m_strTimeServerAddress; }
            set { m_strTimeServerAddress = value; }
        }

        public double AddTime
        {
            get { return m_dAddTime; }
            set { m_dAddTime = value; }
        }
    }
}
