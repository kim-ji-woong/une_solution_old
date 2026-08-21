using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace WebApplication1
{
    public partial class MySQL : System.Web.UI.Page
    {
        // 세션 유지용 쿠키
        //private CookieContainer cookieContainer = new CookieContainer();

        //private const string m_strWebServerURL = "http://unes.iptime.org:10091/SOP";
        //private const string m_szWebPageName = "DBQuery2.jsp";

        private const string DATA_DB = "EDU_100";
        private const string ALARM_DB = "AlarmSimulation";

        private const int m_nSiteID = 100;
        //private const int m_nOutPort = 10092;

        private const string SessionFireEnabled = "MySQLFireEnabled";
        private const string SessionSecurityEnabled = "MySQLSecurityEnabled";

        private System.Text.Encoding m_PageEncoding = System.Text.Encoding.UTF8;

        //private string m_szLastError = "";
        //private string m_szLastErrorMsg = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitSessions();
                /*int nFireCount = GetAlarmCount("화재");
                int nPSMCount = GetAlarmCount("오염");
                int nSecurityCount = GetAlarmCount("지진");

                Session[SessionFireEnabled] = nFireCount == 0;
                Session[SessionPSMEnabled] = nPSMCount == 0;
                Session[SessionSecurityEnabled] = nSecurityCount == 0;*/

                //int nMaxID = GetMaxData("RequestAlarm", ALARM_DB);
                //Session[SessionFireEnabled] = nMaxID == 0;

                /*Uri uri = HttpContext.Current.Request.Url;
                string strURL = uri.Scheme + "://" + uri.Host + ":" + m_nOutPort.ToString() + uri.AbsolutePath;
                string str = "alert('" + strURL + "');";
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", str, true);*/
            }

            btnFire.Enabled = Session[SessionFireEnabled] == null ? true : (bool)Session[SessionFireEnabled];
            btnSecurity.Enabled = Session[SessionSecurityEnabled] == null ? true : (bool)Session[SessionSecurityEnabled];
            //System.Diagnostics.Trace.WriteLine("Page_Load");
        }

        private void InitSessions()
        {
            Session[SessionFireEnabled] = true;
            Session[SessionSecurityEnabled] = true;

            string strSQL = "select AlarmCategory from RequestAlarm group by AlarmCategory, SiteID having SiteID = " + m_nSiteID.ToString();
            //ArrayList arrResult = GetResultData(strSQL, 0, ALARM_DB);
            ArrayList arrResult = GetReadData(strSQL, ALARM_DB);

            if (arrResult == null)
                return;

            foreach (object obj in arrResult)
            {
                if (obj.ToString().Trim() == "화재")
                {
                    Session[SessionFireEnabled] = false;
                    break;
                }
            }

            foreach (object obj in arrResult)
            {
                if (obj.ToString().Trim() == "보안")
                {
                    Session[SessionSecurityEnabled] = false;
                    break;
                }
            }
        }

        protected void btnFire_Click(object sender, EventArgs e)
        {
            if (btnFire.Enabled == false)
                return;

            btnFire.Enabled = false;
            Session[SessionFireEnabled] = false;

            int nMaxID = GetMaxData("RequestAlarm", ALARM_DB);

            string strSQL = "Insert into RequestAlarm (ID, AlarmCategory, AlarmParameter, SiteID) values (" + (nMaxID + 1).ToString();
            strSQL += ", '화재', '1', " + m_nSiteID.ToString() + ")";
            ExecuteData(strSQL, ALARM_DB);
            //GetResultData(strSQL, 0, ALARM_DB);
        }

        protected void btnSecurity_Click(object sender, EventArgs e)
        {
            if (btnSecurity.Enabled == false)
                return;

            btnSecurity.Enabled = false;
            Session[SessionSecurityEnabled] = false;

            int nMaxID = GetMaxData("RequestAlarm", ALARM_DB);

            string strSQL = "Insert into RequestAlarm (ID, AlarmCategory, AlarmParameter, SiteID) values (" + (nMaxID + 1).ToString();
            strSQL += ", '보안', '" + GetRandomSecurity() + "', " + m_nSiteID.ToString() + ")";
            ExecuteData(strSQL, ALARM_DB);
            //GetResultData(strSQL, 0, ALARM_DB);
        }

        private string GetRandomSecurity()
        {
            string[] securities = new string[] { "SVMS", "EMPoll", "Access" };

            Random random = new Random(DateTime.Now.Millisecond);
            int nIndex = random.Next() % securities.Count();
            return securities[nIndex];
        }

        protected void btnStopFire_Click(object sender, EventArgs e)
        {
            string strSQL = "Select SensorTagInfoID from AlarmBoard where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = GetReadData(strSQL, ALARM_DB);
            //ArrayList arrResult = GetResultData(strSQL, 0, ALARM_DB);

            if (arrResult == null || arrResult.Count == 0)
                return;

            int nSensorTagInfoID;
            int nMaxID = GetMaxData("RequestAlarm", ALARM_DB);

            foreach (object obj in arrResult)
            {
                if (int.TryParse(obj.ToString().Trim(), out nSensorTagInfoID))
                {
                    string strParam = "0\t" + nSensorTagInfoID.ToString();
                    string strSQL2 = "Insert into RequestAlarm (ID, AlarmCategory, AlarmParameter, SiteID) values (" + (nMaxID + 1).ToString();
                    strSQL2 += ", '화재', '" + strParam + "', " + m_nSiteID.ToString() + ")";
                    ExecuteData(strSQL2, ALARM_DB);
                    //GetResultData(strSQL2, 0, ALARM_DB);
                }
            }

            /*int nMaxID = GetMaxData("RequestAlarm", ALARM_DB);

            string strSQL = "Insert into RequestAlarm (ID, AlarmCategory, AlarmParameter, SiteID) values (" + (nMaxID + 1).ToString();
            strSQL += ", '화재', '0', " + m_nSiteID.ToString() + ")";
            GetResultData(strSQL, 0, ALARM_DB);*/
        }

        protected void btnStopSecurity_Click(object sender, EventArgs e)
        {
            string strSQL = "Select SensorTagInfoID from AlarmBoard where SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = GetReadData(strSQL, ALARM_DB);
            //ArrayList arrResult = GetResultData(strSQL, 0, ALARM_DB);

            if (arrResult == null || arrResult.Count == 0)
                return;

            int nSensorTagInfoID;
            int nMaxID = GetMaxData("RequestAlarm", ALARM_DB);

            foreach (object obj in arrResult)
            {
                if (int.TryParse(obj.ToString().Trim(), out nSensorTagInfoID))
                {
                    string strParam = "0\t" + nSensorTagInfoID.ToString();
                    string strSQL2 = "Insert into RequestAlarm (ID, AlarmCategory, AlarmParameter, SiteID) values (" + (nMaxID + 1).ToString();
                    strSQL2 += ", '보안', '" + strParam + "', " + m_nSiteID.ToString() + ")";
                    ExecuteData(strSQL2, ALARM_DB);
                    //GetResultData(strSQL2, 0, ALARM_DB);
                }
            }
        }

        private int GetAlarmCount(string strCategory)
        {
            string strSQL = "Select count(ID) from RequestAlarm where AlarmCategory = '" + strCategory + "' and SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = GetReadData(strSQL, ALARM_DB);
            //ArrayList arrResult = GetResultData(strSQL, 0, ALARM_DB);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> count = GetIntField(arrResult[0].ToString());
            return count == null ? 0 : count.Data;
        }

        private int GetMaxData(string strTableName, string strDBName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = GetReadData(strSQL, strDBName);
            //ArrayList arrResult = GetResultData(strSQL, 0, strDBName);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            VariousData<int> id = GetIntField(arrResult[0].ToString());
            return id == null ? 0 : id.Data;
        }

        private static char ConvertToHex(char cSource)
        {
            return "0123456789abcdef"[0x0f & cSource];
        }

        private static string URLEncoding(byte[] bytes)
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

        /*private string GetReadDB(string strSQLQuery, int nTransaction, string szDBName)
        {
            string resResult = string.Empty;
            string sourceUrl = m_strWebServerURL + "/" + m_szWebPageName;

            UTF8Encoding enc = new UTF8Encoding();
            byte[] bytes1 = enc.GetBytes(strSQLQuery);
            string strUrlEncode = URLEncoding(bytes1);

            string postData = "SQLQuery=" + strUrlEncode + "&" + "Transaction=" + nTransaction;
            postData += "&" + "DatabaseName=" + szDBName;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            lock (this)
            {
                wReq.Method = "POST";
                //wReq.UserAgent = "Mozilla/4.0";
                wReq.ContentType = "application/x-www-form-urlencoded";
                wReq.ContentLength = bytes.Length;
                wReq.CookieContainer = cookieContainer;

                try
                {
                    using (Stream writeStream = wReq.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }

                    HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

                    Stream respPostStream = wRes.GetResponseStream();

                    StreamReader readerPost = new StreamReader(respPostStream, m_PageEncoding);

                    resResult = readerPost.ReadToEnd();

                    readerPost.Close();
                    respPostStream.Close();
                }
                catch (System.Net.WebException)
                {
                    return "";
                }
            }

            return resResult;
        }

        public ArrayList GetResultData(string strSQLQuery, int nTransaction, string szDBName = null)
        {
            // str에 '\n', '\r'이 포함되어 있으면 다른 문자로 바꾼다.
            strSQLQuery = strSQLQuery.Replace('\n', (char)6);
            strSQLQuery = strSQLQuery.Replace('\r', (char)7);

            ArrayList arrResult = new ArrayList();

            string resResult = "";

            resResult = GetReadDB(strSQLQuery, nTransaction, szDBName);
            
            StringFile strFile = new StringFile(resResult);

            string strResult = "";
            bool isResult = true;
            bool isBegin = false;

            while (isResult)
            {
                isResult = strFile.ReadLine(ref strResult);

                if (isResult)
                {
                    if (strResult == "Begin Data")
                    {
                        isBegin = true;
                        continue;
                    }

                    if (strResult.StartsWith("JDBC 드라이브 연결 오류"))
                    {
                        string szTemp = strResult.Replace("JDBC 드라이브 연결 오류-", "");
                        string[] errs = szTemp.Split(':');
                        if (errs != null && errs.Length > 2)
                        {
                            m_szLastError = errs[0];
                            m_szLastErrorMsg = errs[1];
                        }
                        else
                        {
                            m_szLastError = "실패";
                            m_szLastErrorMsg = szTemp;
                        }
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

            m_szLastError = "성공";
            m_szLastErrorMsg = "성공";

            return arrResult;
        }*/

        public bool ExecuteData(string strSQLQuery, string strDBName = null)
        {
            string strConnection = "Server=127.0.0.1;Database=" + strDBName + ";UId=sa;pwd=9449966Ab;";
            MySql.Data.MySqlClient.MySqlConnection connection = null;

            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(strConnection);
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(strSQLQuery, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                return false;
            }

            connection.Close();
            /*if (strDBName == null)
                strDBName = DATA_DB;

            string strConnection = "Data Source=127.0.0.1,1433;Initial Catalog=" + strDBName + ";User ID=sa;Password=9449966Ab;";
            SqlConnection connection = new SqlConnection(strConnection);

            SqlCommand cmd = new SqlCommand(strSQLQuery, connection);

            try
            {
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                connection.Close();
                return false;
            }*/

            return true;
        }

        public ArrayList GetReadData(string strSQLQuery, string strDBName = null)
        {
            string strConnection = "Server=127.0.0.1;Database=" + strDBName + ";UId=sa;pwd=9449966Ab;";
            MySql.Data.MySqlClient.MySqlConnection connection = null;
            MySql.Data.MySqlClient.MySqlDataReader reader = null;
            ArrayList arrResult = new ArrayList();

            try
            {
                connection = new MySql.Data.MySqlClient.MySqlConnection(strConnection);
                connection.Open();

                MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(strSQLQuery, connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        arrResult.Add(reader[i]);
                    }
                }

                reader.Close();
            }
            catch (Exception)
            {
                if (connection != null && connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

                return arrResult;
            }

            connection.Close();
            /*if (strDBName == null)
                strDBName = DATA_DB;

            string strConnection = "Data Source=127.0.0.1,1433;Initial Catalog=" + strDBName + ";User ID=sa;Password=9449966Ab;";
            SqlConnection connection = new SqlConnection(strConnection);

            SqlCommand cmd = new SqlCommand(strSQLQuery, connection);
            ArrayList arrResult = new ArrayList();

            try
            {
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        arrResult.Add(reader[i]);
                    }
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                connection.Close();
            }*/

            return arrResult;
        }

        public VariousData<int> GetIntField(string dataSrc)
        {
            if (dataSrc == null || dataSrc.Length == 0 || dataSrc == "null")
                return null;

            if (string.Compare(dataSrc, "true", true) == 0)
                return new VariousData<int>(1);
            else if (string.Compare(dataSrc, "false", true) == 0)
                return new VariousData<int>(0);

            int num;

            if (int.TryParse(dataSrc, out num))
                return new VariousData<int>(num);

            return null;
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            if ((bool)Session[SessionFireEnabled] == false ||
                (bool)Session[SessionSecurityEnabled] == false)
            {
                string strSQL = "select AlarmCategory from RequestAlarm group by AlarmCategory, SiteID having SiteID = " + m_nSiteID.ToString();
                ArrayList arrResult = GetReadData(strSQL, ALARM_DB);
                //ArrayList arrResult = GetResultData(strSQL, 0, ALARM_DB);

                if (arrResult == null)
                    return;

                bool refresh = false;

                if ((bool)Session[SessionFireEnabled] == false)
                {
                    bool find = false;

                    foreach (object obj in arrResult)
                    {
                        if (obj.ToString().Trim() == "화재")
                        {
                            find = true;
                            break;
                        }
                    }

                    if (!find)
                    {
                        Session[SessionFireEnabled] = true;
                        refresh = true;
                    }
                }

                if ((bool)Session[SessionSecurityEnabled] == false)
                {
                    bool find = false;

                    foreach (object obj in arrResult)
                    {
                        if (obj.ToString().Trim() == "보안")
                        {
                            find = true;
                            break;
                        }
                    }

                    if (!find)
                    {
                        Session[SessionSecurityEnabled] = true;
                        refresh = true;
                    }
                }

                if (refresh)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "Redirect", "Redirect()", true);
                }
            }

            /*bool refresh = false;

            if ((bool)Session[SessionFireEnabled] == false)
            {
                int nFireCount = GetAlarmCount("화재");

                if (nFireCount == 0)
                {
                    Session[SessionFireEnabled] = true;
                    refresh = true;
                }
            }

            if ((bool)Session[SessionPSMEnabled] == false)
            {
                int nPSMCount = GetAlarmCount("오염");

                if (nPSMCount == 0)
                {
                    Session[SessionPSMEnabled] = true;
                    refresh = true;
                }
            }

            if ((bool)Session[SessionSecurityEnabled] == false)
            {
                int nSecurityCount = GetAlarmCount("지진");

                if (nSecurityCount == 0)
                {
                    Session[SessionSecurityEnabled] = true;
                    refresh = true;
                }
            }

            if (refresh)
            {
                Session[SessionFireEnabled] = true;
                ScriptManager.RegisterStartupScript(this, GetType(), "Redirect", "Redirect()", true);
            }*/

            /*if ((bool)Session[SessionFireEnabled] == false)
            {
                int nMaxID = GetMaxData("RequestAlarm", ALARM_DB);

                if (nMaxID == 0)
                {
                    Session[SessionFireEnabled] = true;
                    ScriptManager.RegisterStartupScript(this, GetType(), "Redirect", "Redirect()", true);
                    //Response.Redirect(HttpContext.Current.Request.Url.AbsoluteUri);
                    //btnFire.Enabled = true;
                    //ScriptManager.RegisterStartupScript(this, GetType(), "RefreshPage", "", true);
                }
            }*/
        }
    }
}