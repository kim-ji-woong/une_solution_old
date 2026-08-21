using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace SensorSimulator.API
{
    using Data;
    using System.Windows.Forms;

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

        private const string FireOn = "3";
        private const string FireOff = "0";

        private static int m_nEventID = 0;
        private static DateTime m_dtPrev = new DateTime();

        public static bool SendAlarmOn(Zone zone)
        {
            DateTime dtNow = DateTime.Now;
            if (IsSameDay(dtNow, m_dtPrev) == false)
                m_nEventID = 0;

            string strEquipCode = zone.Name + " 화재센서";
            string strEventID = string.Format("evt{4}_{0}{1:00}{2:00}_{3:000}", dtNow.Year, dtNow.Month, dtNow.Day, ++m_nEventID, FormMain.Instance.LocalSiteID);

            return SendAlarmEvent(strEquipCode, FireOn, strEventID, "화재", zone);
        }

        public static bool SendAlarmOff(FireAlarm alarm)
        {
            return SendAlarmEvent(alarm.EquipCode, FireOff, alarm.EventID, alarm.EventType, alarm.Zone);
        }

        private static bool SendAlarmEvent(string strEquipCode, string strEquipStatus, string strEventID, string strEventType, Zone zone)
        {
            string strBaseUrl = ConfigurationManager.AppSettings.Get("url");
            string strApiUrl = ConfigurationManager.AppSettings.Get("alarmURL");

            if (strBaseUrl == null || strBaseUrl.Length == 0 ||
                strApiUrl == null || strApiUrl.Length == 0)
                return false;

            string strURL = strBaseUrl + ":" + strApiUrl;
            DateTime dtNow = DateTime.Now;

            JsonManager mgr = new JsonManager();

            mgr.Add("dvcCd", strEquipCode);
            mgr.Add("dvcStatus", strEquipStatus);
            mgr.Add("evtId", strEventID);
            mgr.Add("evtTime", GetTimeString(dtNow));
            mgr.Add("evtType", strEventType);
            mgr.Add("mapCd", zone.Building.Code);
            mgr.Add("floorId", zone.FloorIndexString);

            string strErrorMessage;
            string strJson = mgr.Json;
            string strResult = SendQuery(strJson, null, strURL, out strErrorMessage);

            m_dtPrev = dtNow;

            if (strResult == null)
                MessageBox.Show(strErrorMessage);

            return strResult != null;
        }

        public static bool SendMalfunction(FireAlarm alarm, bool checkToUnE)
        {
            if (checkToUnE)
                return SendCheckAlarm(alarm, 0);
            else
                SendCheckAlarm(alarm, 0);

            return SendCheckAlarmToOutside(alarm, 0);
        }

        public static bool SendRealFire(FireAlarm alarm, bool checkToUnE)
        {
            if (checkToUnE)
                return SendCheckAlarm(alarm, 1);
            else
                SendCheckAlarm(alarm, 1);

            return SendCheckAlarmToOutside(alarm, 1);
        }

        private static bool SendCheckAlarm(FireAlarm alarm, int isReal)
        {
            string strBaseUrl = ConfigurationManager.AppSettings.Get("url");
            string strApiUrl = ConfigurationManager.AppSettings.Get("checkURL");

            if (strBaseUrl == null || strBaseUrl.Length == 0 ||
                strApiUrl == null || strApiUrl.Length == 0)
                return false;

            string strURL = strBaseUrl + ":" + strApiUrl;

            JsonManager mgr = new JsonManager();

            mgr.Add("evtId", alarm.EventID);
            mgr.Add("isReal", isReal);
            mgr.Add("description", "from SensorSimulator");

            string strErrorMessage;
            string strJson = mgr.Json;
            string strResult = SendQuery(strJson, null, strURL, out strErrorMessage);

            return strResult != null;
        }

        private static bool ReadAPIKey(out string strApiKey, out string strSopUrl)
        {
            strApiKey = strSopUrl = null;

            string strAPITag = "plux_ApiKey", strURLTag = "plux_url";
            string strSQL = "Select PropertyName, PropertyValue from OptionSOPSimulator where PropertyName = '" + strAPITag + "' or PropertyName = '" + strURLTag + "'";
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                string strPropertyName = WebDBManager.GetStringField(arrResult[i]);
                string strPropertyValue = WebDBManager.GetStringField(arrResult[i + 1]);

                if (strPropertyName == null || strPropertyValue == null)
                    continue;

                if (strPropertyName == strAPITag)
                    strApiKey = strPropertyValue;
                else if (strPropertyName == strURLTag)
                    strSopUrl = strPropertyValue;
            }

            return strApiKey != null && strSopUrl != null;
        }

        private static bool SendCheckAlarmToOutside(FireAlarm alarm, int isReal)
        {
            string strSopUrl, strApiKey;

            if (ReadAPIKey(out strApiKey, out strSopUrl) == false)
                return false;

            //string strSopUrl = ConfigurationManager.AppSettings.Get("sopURL2");
            //string strApiKey = ConfigurationManager.AppSettings.Get("apiKey");

            if (strSopUrl == null || strSopUrl.Length == 0 || strApiKey == null | strApiKey.Length == 0)
                return false;

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders["Api-Key"] = strApiKey.Trim();

            JsonManager mgr = new JsonManager();

            mgr.Add("dvcCd", alarm.EquipCode);
            mgr.Add("dvcStatus", alarm.EquipStatus);
            mgr.Add("evtId", alarm.EventID);
            mgr.Add("malfYn", isReal == 1 ? "N" : "Y");

            string strErrorMessage;
            string strJson = mgr.Json;
            string strResult = SendQuery(strJson, dicHeaders, strSopUrl, out strErrorMessage);

            return strResult != null;
        }

        private static string GetTimeString(DateTime timeStamp)
        {
            return string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                timeStamp.Year, timeStamp.Month, timeStamp.Day,
                timeStamp.Hour, timeStamp.Minute, timeStamp.Second);
        }

        private static bool IsSameDay(DateTime dt1, DateTime dt2)
        {
            if (dt1.Year == dt2.Year && dt1.Month == dt2.Month && dt1.Day == dt2.Day)
                return true;

            return false;
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
