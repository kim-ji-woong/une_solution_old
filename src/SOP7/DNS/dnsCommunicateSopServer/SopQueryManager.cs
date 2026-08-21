using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace dnsCommunicateSopServer
{
    public class SopQueryManager
    {
        private string m_strRequestURL = "";
        public SopQueryManager(string requestURL = "")
        {
            m_strRequestURL = requestURL;
        }

        /// <summary>
        /// 알람 전송
        /// </summary>
        /// <param name="arrDatas">SensorType, SensorTagID, SensorZoneID, IsAlarm</param>
        /// <param name="strMethodType"></param>
        /// <param name="strURL"></param>
        /// <returns></returns>
        public bool SendAlarmQuery(ArrayList arrDatas, string strMethodType, string strURL = "")
        {
            if (arrDatas != null && arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is bool)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                bool bIsAlarm = (bool)arrDatas[3];
                int nAlarmLevel = (arrDatas.Count >= 5 && arrDatas[4] is int) ? (int)arrDatas[4] : -1;

                Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
                dicHeaders.Add("Content-Type", "application/json");

                JsonManager jsonData = new JsonManager();
                jsonData.Add("Header", dnsSopID.Header.SENSOR_DATA);
                jsonData.Add("ClientInfo", "TEST");

                string value = "";
                if (nAlarmLevel == -1)
                {
                    value = string.Format("[\"{0},{1}\",\"{0},{2}\",\"{0},{3}\",\"{0},{4}\"]", dnsSopID.DATA_TYPE.INT
                                            , nSensorType, nSensorTagID, nSensorZoneID, (bIsAlarm) ? 1 : 0);
                }
                else
                {
                    value = string.Format("[\"{0},{1}\",\"{0},{2}\",\"{0},{3}\",\"{0},{4}\",\"{0},{5}\"]", dnsSopID.DATA_TYPE.INT
                                            , nSensorType, nSensorTagID, nSensorZoneID, (bIsAlarm) ? 1 : 0, nAlarmLevel);
                }

                jsonData.AddNoQuote("Values", value);

                string strErrorMessage = null;
                bool result = SendQuery(strMethodType, jsonData.Json, out strErrorMessage, strURL);
                return result;
            }

            return false;
        }

        /// <summary>
        /// TEST 알람 전송
        /// </summary>
        /// <param name="arrDatas">SensorType, SensorTagID, SensorZoneID, IsAlarm</param>
        /// <param name="strMethodType"></param>
        /// <param name="strURL"></param>
        /// <returns></returns>
        public bool SendAlarmQuery_TEST(ArrayList arrDatas, string strMethodType, string strURL = "")
        {
            if (arrDatas != null && arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is bool)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                bool bIsAlarm = (bool)arrDatas[3];
                int nAlarmLevel = (arrDatas.Count >= 5 && arrDatas[4] is int) ? (int)arrDatas[4] : -1;

                Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
                dicHeaders.Add("Content-Type", "application/json");

                JsonManager jsonData = new JsonManager();
                jsonData.Add("Header", dnsSopID.Header.SENSOR_DATA_TEST);
                jsonData.Add("ClientInfo", "TEST");

                string value = "";

                if (nAlarmLevel == -1)
                {
                    value = string.Format("[\"{0},{1}\",\"{0},{2}\",\"{0},{3}\",\"{0},{4}\"]", dnsSopID.DATA_TYPE.INT
                                            , nSensorType, nSensorTagID, nSensorZoneID, (bIsAlarm) ? 1 : 0);
                }
                else
                {
                    value = string.Format("[\"{0},{1}\",\"{0},{2}\",\"{0},{3}\",\"{0},{4}\",\"{0},{5}\"]", dnsSopID.DATA_TYPE.INT
                                            , nSensorType, nSensorTagID, nSensorZoneID, (bIsAlarm) ? 1 : 0, nAlarmLevel);
                }


                jsonData.AddNoQuote("Values", value);

                string strErrorMessage = null;
                bool result = SendQuery(strMethodType, jsonData.Json, out strErrorMessage, strURL);
                return result;
            }

            return false;
        }

        /// <summary>
        /// 알람 오작동 전송
        /// </summary>
        /// <param name="arrDatas">SensorType, SensorTagID, SensorZoneID, IsAlarm</param>
        /// <param name="strMethodType"></param>
        /// <param name="strURL"></param>
        /// <returns></returns>
        public bool SendAlarmMalfunctionQuery(bool malfunction, ArrayList arrDatas, string strMethodType, string strURL = "")
        {
            if (arrDatas != null && arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is bool)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                bool bIsAlarm = (bool)arrDatas[3];
                int nAlarmLevel = (arrDatas.Count >= 5 && arrDatas[4] is int) ? (int)arrDatas[4] : -1;

                Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
                dicHeaders.Add("Content-Type", "application/json");

                JsonManager jsonData = new JsonManager();
                if (malfunction)
                    jsonData.Add("Header", dnsSopID.Header.SENSOR_MALFUNCTION);
                else
                    jsonData.Add("Header", dnsSopID.Header.SENSOR_USER_RESET);
                jsonData.Add("ClientInfo", "TEST");

                string value = "";
                if (nAlarmLevel == -1)
                {
                    value = string.Format("[\"{0},{1}\",\"{0},{2}\",\"{0},{3}\",\"{0},{4}\"]", dnsSopID.DATA_TYPE.INT
                                            , nSensorType, nSensorTagID, nSensorZoneID, (bIsAlarm) ? 1 : 0);
                }
                else
                {
                    value = string.Format("[\"{0},{1}\",\"{0},{2}\",\"{0},{3}\",\"{0},{4}\",\"{0},{5}\"]", dnsSopID.DATA_TYPE.INT
                                            , nSensorType, nSensorTagID, nSensorZoneID, (bIsAlarm) ? 1 : 0, nAlarmLevel);
                }

                jsonData.AddNoQuote("Values", value);

                string strErrorMessage = null;
                bool result = SendQuery(strMethodType, jsonData.Json, out strErrorMessage, strURL);
                return result;
            }

            return false;
        }

        /// <summary>
        /// 알람 User reset 전송
        /// </summary>
        /// <param name="arrDatas">SensorType, SensorTagID, SensorZoneID, IsAlarm</param>
        /// <param name="strMethodType"></param>
        /// <param name="strURL"></param>
        /// <returns></returns>
        public bool SendAlarmUserResetQuery(bool userReset, ArrayList arrDatas, string strMethodType, string strURL = "")
        {
            if (arrDatas != null && arrDatas.Count >= 2 && arrDatas[0] is int && arrDatas[1] is int)
            {
                int nSensorZoneID = (int)arrDatas[0];
                int nSOPGenUserID = (int)arrDatas[1];

                Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
                dicHeaders.Add("Content-Type", "application/json");

                JsonManager jsonData = new JsonManager();
                if (userReset)
                    jsonData.Add("Header", dnsSopID.Header.SENSOR_USER_RESET);
                else
                    jsonData.Add("Header", dnsSopID.Header.SENSOR_MALFUNCTION);
                jsonData.Add("ClientInfo", "TEST");

                string value = string.Format("[\"{0},{1}\",\"{0},{2}\"]", dnsSopID.DATA_TYPE.INT, nSensorZoneID, nSOPGenUserID);

                jsonData.AddNoQuote("Values", value);

                string strErrorMessage = null;
                bool result = SendQuery(strMethodType, jsonData.Json, out strErrorMessage, strURL);
                return result;
            }

            return false;
        }

        /// <summary>
        /// 전체 알람 해제
        /// </summary>
        /// <param name="strMethodType"></param>
        /// <param name="strURL"></param>
        /// <returns></returns>
        public bool SendAllClearQuery(string strMethodType, string strURL = "")
        {
            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("Content-Type", "application/json");

            JsonManager jsonData = new JsonManager();
            jsonData.Add("Header", dnsSopID.Header.CLEAR_DETECT_ALL);
            jsonData.Add("ClientInfo", "TEST");

            string strErrorMessage = null;
            bool result = SendQuery(strMethodType, jsonData.Json, out strErrorMessage, strURL);
            return result;
        }

        /// <summary>
        /// 상황 전파 (SOP 실행)
        /// </summary>
        /// <param name="arrDatas"></param>
        /// <param name="strMethodType"></param>
        /// <param name="strURL"></param>
        /// <returns></returns>
        public bool SendSituationNotice(ArrayList arrDatas, string strMethodType, string strURL = "")
        {
            if (arrDatas != null && arrDatas.Count >= 2 && arrDatas[0] is int && arrDatas[1] is int)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];

                string strErrorMessage = null;

                Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
                dicHeaders.Add("Content-Type", "application/json");

                JsonManager jsonData = new JsonManager();
                jsonData.Add("Header", dnsSopID.Header.SITUATION_NOTICE);
                jsonData.Add("ClientInfo", "TEST");

                string value = string.Format("[\"{0},{1}\",\"{0},{2}\"]", dnsSopID.DATA_TYPE.INT, nSensorType, nSensorZoneID);

                jsonData.AddNoQuote("Values", value);

                bool result = SendQuery(strMethodType, jsonData.Json, out strErrorMessage, strURL);
                return result;
            }

            return false;
        }

        /// <summary>
        /// 수동 신고
        /// </summary>
        /// <param name="arrDatas"></param>
        /// <param name="strMethodType"></param>
        /// <param name="strURL"></param>
        /// <returns></returns>
        public bool SendManualReport(ArrayList arrDatas, string strMethodType, string strURL = "")
        {
            if (arrDatas.Count >= 7 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is DateTime
                                    && arrDatas[4] is int && arrDatas[5] is string && arrDatas[6] is string)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nZoneID = (int)arrDatas[2];
                DateTime dtDateTime = (DateTime)arrDatas[3];
                int nAlarmDepth = (int)arrDatas[4];
                string strReportPerson = (string)arrDatas[5];
                string strMemo = (string)arrDatas[6];

                string strErrorMessage = null;

                Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
                dicHeaders.Add("Content-Type", "application/json");

                JsonManager jsonData = new JsonManager();
                jsonData.Add("Header", dnsSopID.Header.MANUAL_REPORT);
                jsonData.Add("ClientInfo", "TEST");

                string value = string.Format("[\"{0},{3}\",\"{0},{4}\",\"{0},{5}\",\"{1},{6}\",\"{0},{7}\",\"{2},{8}\",\"{2},{9}\"]"
                    , dnsSopID.DATA_TYPE.INT, dnsSopID.DATA_TYPE.DATETIME, dnsSopID.DATA_TYPE.STRING
                    , nSensorType, nSensorZoneID, nZoneID, dtDateTime, nAlarmDepth, strReportPerson, strMemo);

                jsonData.AddNoQuote("Values", value);

                bool result = SendQuery(strMethodType, jsonData.Json, out strErrorMessage, strURL);
                return result;
            }

            return false;
        }

        /// <summary>
        /// 수동 신고 종료
        /// </summary>
        /// <param name="arrDatas"></param>
        /// <param name="strMethodType"></param>
        /// <param name="strURL"></param>
        /// <returns></returns>
        public bool SendClearManualReport(ArrayList arrDatas, string strMethodType, string strURL = "")
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is int)
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSensorZoneHistoryID = (int)arrDatas[2];
                int nUserID = (int)arrDatas[3];

                string strErrorMessage = null;

                Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
                dicHeaders.Add("Content-Type", "application/json");

                JsonManager jsonData = new JsonManager();
                jsonData.Add("Header", dnsSopID.Header.CLEAR_MANUAL_REPORT);
                jsonData.Add("ClientInfo", "TEST");

                string value = string.Format("[\"{0},{1}\",\"{0},{2}\",\"{0},{3}\",\"{0},{4}\"]"
                    , dnsSopID.DATA_TYPE.INT, nSensorType, nSensorZoneID, nSensorZoneHistoryID, nUserID);

                jsonData.AddNoQuote("Values", value);

                bool result = SendQuery(strMethodType, jsonData.Json, out strErrorMessage, strURL);
                return result;
            }

            return false;
        }

        public bool SendQuery(string strMethodType, string strBody, out string strErrorMessage, string strURL = "")
        {
            strErrorMessage = null;
            if (strURL == "" && m_strRequestURL.Length == 0)
            {
                strErrorMessage = "요청 URL을 확인하세요.";
                return false;
            }

            HttpWebRequest request = null;
            if (strURL.Length > 0)
            {
                request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
            }
            else
            {
                request = (HttpWebRequest)WebRequest.Create(new Uri(m_strRequestURL));
            }
            
            request.Method = strMethodType;
            request.ContentType = "application/json;";
            string strResponse = "";

            try
            {
                if (strBody != null && strBody != "")
                {
                    StreamWriter streamWriter = new StreamWriter(request.GetRequestStream());
                    streamWriter.Write(strBody);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResponse = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

            }
            catch (WebException ex)
            {
                strErrorMessage = ex.Status.ToString();
                return false;
            }

            if (strResponse == null)
                return false;

            return true;
        }

        #region NST
        /// <summary>
        /// 알람 오작동 전송
        /// </summary>
        /// <param name="arrDatas">SensorType, SensorTagID, SensorZoneID, IsAlarm</param>
        /// <param name="strMethodType"></param>
        /// <param name="strURL"></param>
        /// <returns></returns>
        public bool SendAlarmMalfunctionQueryNST(bool malfunction, ArrayList arrDatas, string strMethodType, string strURL = "")
        {
            if (arrDatas.Count >= 8 &&
                arrDatas[0] is int &&
                arrDatas[1] is int &&
                arrDatas[2] is int &&
                arrDatas[3] is string &&
                arrDatas[4] is string &&
                arrDatas[5] is DateTime &&
                arrDatas[6] is int &&
                (arrDatas[7] == null || arrDatas[7] is string))
            {
                int nSensorType = (int)arrDatas[0];
                int nSensorTagID = (int)arrDatas[1];
                int nSensorZoneID = (int)arrDatas[2];
                string strMemberID = (string)arrDatas[3];
                string strCameraID = (string)arrDatas[4];
                DateTime timeStamp = (DateTime)arrDatas[5];
                int nAlarmLevel = (int)arrDatas[6];
                string strMessage = (string)arrDatas[7];

                Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
                dicHeaders.Add("Content-Type", "application/json");

                JsonManager jsonData = new JsonManager();
                if (malfunction)
                    jsonData.Add("Header", dnsSopID.Header.SENSOR_MALFUNCTION);
                else
                    jsonData.Add("Header", dnsSopID.Header.SENSOR_USER_RESET);
                jsonData.Add("ClientInfo", "TEST");

                string value = "";
                value = string.Format("[\"{0},{3}\",\"{0},{4}\",\"{0},{5}\",\"{1},{6}\",\"{1},{7}\",\"{2},{8}\",\"{0},{9}\",\"{1},{10}\"]"
                    , dnsSopID.DATA_TYPE.INT, dnsSopID.DATA_TYPE.STRING, dnsSopID.DATA_TYPE.DATETIME
                    , nSensorType, nSensorTagID, nSensorZoneID, strMemberID, strCameraID, timeStamp, nAlarmLevel, strMessage);
                
                jsonData.AddNoQuote("Values", value);

                string strErrorMessage = null;
                bool result = SendQuery(strMethodType, jsonData.Json, out strErrorMessage, strURL);
                return result;
            }

            return false;
        } 
        #endregion
    }

    public class JsonManager
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

        /// <summary>
        /// 따옴표 없는 Value
        /// </summary>
        public void AddNoQuote(string strName, string strValue)
        {
            string strLine = "\"" + strName + "\": " + strValue.ToString();

            if (m_strValues.Length == 0)
                m_strValues = strLine;
            else
                m_strValues += ", " + strLine;
        }

        public void Add(int nName, string strValue)
        {
            string strLine = nName + ": " + strValue + "\"";

            if (m_strValues.Length == 0)
                m_strValues = strLine;
            else
                m_strValues += ", " + strLine;
        }

        public void Add(int nName, int nValue)
        {
            string strLine = nName + ": " + nValue;

            if (m_strValues.Length == 0)
                m_strValues = strLine;
            else
                m_strValues += ", " + strLine;
        }
    }
}
