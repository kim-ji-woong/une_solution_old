using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.IO;
using System.Net.Http;
using System.Reflection;
using DBUtility2;
using System.Collections;

namespace SendSensorResult
{
    public static class WebServiceManager
    {
        private class JsonManager
        {
            private string m_strValues = "";

            public string Json
            {
                get
                {
                    string strJson = "{ " + m_strValues + " }";
                    return strJson;
                }
            }

            public void Add(string strName, string strValue)
            {
                string strLine = "\"" + strName + "\": \"" + strValue + "\"";

                if (m_strValues.Length == 0)
                    m_strValues = strLine;
                else
                    m_strValues += ", " + strLine;
            }

            public void Add(string strName, int nValue)
            {
                string strLine = "\"" + strName + "\": " + nValue.ToString();

                if (m_strValues.Length == 0)
                    m_strValues = strLine;
                else
                    m_strValues += ", " + strLine;
            }
        }

        // SOPWebServer에 전송
        public static bool SendSOPWebAPI(string strEvtID, int isReal)
        {
            string strSopUrl = ConfigurationManager.AppSettings.Get("sopURL");

            if (strSopUrl == null || strSopUrl.Length == 0)
                return false;

            JsonManager mgr = new JsonManager();

            mgr.Add("evtId", strEvtID);
            mgr.Add("isReal", isReal);
            mgr.Add("description", "from SOPSystem");

            string strErrorMessage;
            string strJson = mgr.Json;
            string strResult = SendQuery(strJson, null, strSopUrl, out strErrorMessage);

            return strResult != null;
        }

        public static bool GetParameter(string[] args, out string strEvtID, out int isReal)
        {
            strEvtID = "";
            isReal = 0;

            if (args == null || args.Count() < 1)
                return false;

            string strParam = args[0].Trim();

            if (strParam == "0")
                isReal = 0;
            else if (strParam == "1")
                isReal = 1;
            
            // SOP Simulator로부터 전달된 값
            string strProcessName = Assembly.GetEntryAssembly().GetName().Name;
            int nActionStepHistoryID = ReadActionStepHistoryID(strProcessName + ".aid");

            WebDBManager dbMgr = MakeDBManager(false);
            WebDBManager dbMgrMain = MakeDBManager(true);

            if (dbMgr == null || dbMgrMain == null)
                return false;

            string strSQL = string.Format("Select SensorZoneHistoryID from ActionStepHistory where ID = {0}", nActionStepHistoryID.ToString());
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return false;

            VariousData<int> sensorZoneHistoryID = WebDBManager.GetIntField(arrResult[0].ToString());            
            if (sensorZoneHistoryID == null)
                return false;

            string strSiteID = ConfigurationManager.AppSettings.Get("ClientSiteid");
            string strSensorZoneHistoryID = string.Format("{0}_{1}", strSiteID, sensorZoneHistoryID.Data);

            strSQL = string.Format("Select evtId, mapCd from WebFireAlarmHistory where SensorZoneHistoryID = '{0}'", strSensorZoneHistoryID);
            arrResult = dbMgrMain.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count != 2)
                return false;

            string evtID = WebDBManager.GetStringField(arrResult[0]);
            string mapCd = WebDBManager.GetStringField(arrResult[1]);
            strEvtID = evtID;

            strSQL = string.Format("Select ID From Building Where BuildingCode = '{0}'", mapCd);
            arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null || arrResult.Count == 0)
                return false;
            else
                return true;
        }

        private static WebDBManager MakeDBManager(bool main)
        {
            string strSiteID, strDBName, strDBType, strWebServerURL = "";
            if (main)
            {
                strSiteID = ConfigurationManager.AppSettings.Get("MainSiteid");
                strDBName = ConfigurationManager.AppSettings.Get("MainDBName");                
            }
            else
            {
                strSiteID = ConfigurationManager.AppSettings.Get("ClientSiteid");
                strDBName = ConfigurationManager.AppSettings.Get("ClientDBName");
            }
            strDBType = ConfigurationManager.AppSettings.Get("type");
            strWebServerURL = ConfigurationManager.AppSettings.Get("url2");

            if (strSiteID == null || strSiteID.Length == 0)
                return null;
            if (strDBName == null || strDBName.Length == 0)
                return null;
            if (strDBType == null || strDBType.Length == 0)
                return null;
            if (strWebServerURL == null || strWebServerURL.Length == 0)
                return null;

            int nSiteID, nDBType;

            if (int.TryParse(strSiteID.Trim(), out nSiteID) == false)
                return null;
            if (int.TryParse(strDBType.Trim(), out nDBType) == false)
                return null;

            WebDBManager dbMgr = new WebDBManager(nSiteID);

            dbMgr.DatabaseName = strDBName.Trim();
            dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;
            dbMgr.WebServerURL = strWebServerURL.Trim();

            return dbMgr;
        }

        private static int ReadActionStepHistoryID(string strFileName)
        {
            if (File.Exists(strFileName) == false)
                return -1;

            StreamReader reader = new StreamReader(strFileName);
            string strLine = reader.ReadLine().Trim();
            reader.Close();

            // 읽었으면 지운다.
            File.Delete(strFileName);

            int nActionStepHistoryID;

            if (int.TryParse(strLine, out nActionStepHistoryID) == false)
                return -1;

            return nActionStepHistoryID;
        }

        private static string SendQuery(string strJson, Dictionary<string, string> dicHeaders, string strURL, out string strErrorMessage, string strMethodType = "POST")
        {
            strErrorMessage = "";

            string url = strURL;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = strMethodType;
            request.ContentType = "application/json";
            request.Timeout = 5000;

            if (dicHeaders != null)
            {
                foreach (KeyValuePair<string, string> pair in dicHeaders)
                {
                    request.Headers.Add(pair.Key, pair.Value);
                }
            }

            // POST할 데이타를 Request Stream에 쓴다
            byte[] bytes = Encoding.UTF8.GetBytes(strJson);
            request.ContentLength = bytes.Length; // 바이트수 지정

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(bytes, 0, bytes.Length);
            }

            try
            {
                // Response 처리
                string responseText = string.Empty;
                using (WebResponse resp = request.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        responseText = sr.ReadToEnd();
                    }
                }

                System.Diagnostics.Trace.WriteLine("Response : " + responseText);
                return responseText;
            }
            catch (Exception ex)
            {
                strErrorMessage = ex.Message;
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }

            return null;
        }
    }
}
