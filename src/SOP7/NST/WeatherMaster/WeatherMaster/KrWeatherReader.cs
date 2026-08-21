using System;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using System.IO;
using Weather.IDAL;
using Weather.DAL;
using Weather.Model;

namespace WeatherMaster
{
    public class KrWeatherReader
    {
        private class CityData
        {
            private const string BaseURL = "https://www.kr-weathernews.com/mv3/html/today.html?region=";
            private string m_strTargetCity = "";
            private string m_strURL = "";

            public string Target
            {
                get { return m_strTargetCity; }
                set { m_strTargetCity = value; }
            }

            public string URL
            {
                get { return m_strURL; }
            }

            public void SetRegion(string strRegion)
            {
                m_strURL = BaseURL + strRegion;
            }
        }

        private List<CityData> m_cities = new List<CityData>();

        private IDataManager m_dataManager = null;
        private Dictionary<string, int> m_dicWindDirection = new Dictionary<string, int>();

        public KrWeatherReader()
        {
            ReadCities();
            CityReader.SetWindDirection(m_dicWindDirection);
            SetDataManager();
        }

        private void SetDataManager()
        {
            string strSite = ConfigurationManager.AppSettings.Get("siteid");

            if (strSite == null || strSite.Length == 0)
                return;

            int nSiteID, nDBType;

            if (int.TryParse(strSite, out nSiteID) == false)
                return;

            string strWebServerURL = ConfigurationManager.AppSettings.Get("webserverURL");
            string strDBName = ConfigurationManager.AppSettings.Get("dbName");
            string strDBType = ConfigurationManager.AppSettings.Get("dbType");

            if (strWebServerURL == null || strWebServerURL.Length == 0 ||
                strDBName == null || strDBName.Length == 0 ||
                strDBType == null || strDBType.Length == 0)
                return;

            if (int.TryParse(strDBType, out nDBType) == false)
                return;

            m_dataManager = new DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
        }

        private bool ReadCities()
        {
            string strCities = ConfigurationManager.AppSettings.Get("cities");

            if (strCities == null || strCities.Length == 0)
                return false;

            string[] tokens = strCities.Split(',');
            int nTokenCount = tokens.Length;

            for (int i = 0; i < nTokenCount; i++)
            {
                string strToken = tokens[i].Trim();

                int nIndex1 = strToken.IndexOf('(');
                int nIndex2 = strToken.IndexOf(')');

                if (nIndex1 < 0 || nIndex2 < nIndex1)
                    continue;

                string strTrgCity = strToken.Substring(0, nIndex1).Trim();
                string strRegion = strToken.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();

                CityData data = new CityData();

                data.Target = strTrgCity;
                data.SetRegion(strRegion);

                m_cities.Add(data);
            }

            return true;
        }

        public bool ReadData()
        {
            int nCityCount = m_cities.Count;

            bool success = true;

            foreach (CityData city in m_cities)
            {
                if (ReadData(city) == false)
                    success = false;
            }

            return success;
        }

        private bool ReadData(CityData data)
        {
            bool success = true;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(data.URL);
                request.Method = "GET";
                request.Timeout = 10 * 1000; // 10초

                string strResponse = "";

                using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
                {
                    HttpStatusCode status = resp.StatusCode;

                    if (status != HttpStatusCode.OK)
                    {
                        System.Diagnostics.Trace.WriteLine("URL 실패 : " + data.URL);
                        return false;
                    }

                    Stream respStream = resp.GetResponseStream();
                    using (StreamReader sr = new StreamReader(respStream))
                    {
                        strResponse = sr.ReadToEnd();
                    }
                }

                float temp, sensible, rain, humidity, atm, windSpeed;
                int windDir;

                if (ParseData(strResponse, out temp, out sensible, out rain, out humidity, out windDir, out windSpeed, out atm))
                {
                    if (m_dataManager != null)
                        CityReader.WriteCurrentData(m_dataManager, data.Target, (int)Current.WeatherState.Unknown, temp, sensible, rain, humidity, windDir, windSpeed, atm);

                    string strLog = string.Format("{0} : 현재기온({1}), 체감온도({2}), 강수량({3}), 습도({4}), 풍향({5}), 풍속({6}), 기압({7})",
                        data.Target, temp, sensible, rain, humidity, windDir, windSpeed, atm);
                    System.Diagnostics.Trace.WriteLine(strLog);
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("ReadFail : " + data.Target);
                    success = false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("ReadData Error : " + e.Message);
                success = false;
            }

            return success;
        }

        /// <summary>
        /// 각 도시별 기상정보를 얻어온다.
        /// </summary>
        /// <param name="strHTML"></param>
        /// <param name="currentTemperature">현재온도</param>
        /// <param name="sensibleTemperature">체감온도</param>
        /// <param name="rain">강수량(mm)</param>
        /// <param name="humidity">습도(%)</param>
        /// <param name="windDirection">풍향</param>
        /// <param name="windSpeed">풍속(m/s)</param>
        /// <param name="atm">기압(hPa)</param>
        /// <returns></returns>
        private bool ParseData(string strHTML, out float currentTemperature, out float sensibleTemperature, out float rain, out float humidity, out int windDirection, out float windSpeed, out float atm)
        {
            currentTemperature = sensibleTemperature = rain = humidity = windSpeed = atm = 0.0f;
            windDirection = 0;

            string strTag = "class=\"area__current\">";
            int nIndex = strHTML.IndexOf(strTag);

            if (nIndex < 0)
                return false;

            strHTML = strHTML.Substring(nIndex + strTag.Length).Trim();

            nIndex = strHTML.IndexOf("</section>");

            if (nIndex < 0)
                return false;

            strHTML = strHTML.Substring(0, nIndex).Trim();

            string strCurrentTemp = ReadData(strHTML, "id=\"curtemp\"");
            string strSensible = ReadData(strHTML, "id=\"feel\"");
            string strHumidity = ReadData(strHTML, "id=\"rhum\"");
            string strAtm = ReadData(strHTML, "id=\"press\"");
            string strWind = ReadData(strHTML, "id=\"wind\"");

            if (strCurrentTemp == null ||
                strSensible == null ||
                strHumidity == null ||
                strAtm == null ||
                strWind == null)
                return false;

            if (ReadFloat(strCurrentTemp, out currentTemperature) == false)
                return false;

            if (ReadFloat(strSensible, out sensibleTemperature) == false)
                return false;

            if (ReadFloat(strHumidity, out humidity) == false)
                return false;

            if (ReadFloat(strAtm, out atm) == false)
                return false;

            if (ReadWind(strWind, ref windDirection, ref windSpeed) == false)
                return false;

            return true;
        }

        private bool ReadWind(string strValue, ref int windDirection, ref float windSpeed)
        {
            int len = strValue.Length;
            int nIndex = -1;

            for (int i = 0; i < len; i++)
            {
                char ch = strValue[i];

                if (ch == ' ' || ch == '\t')
                {
                    nIndex = i;
                    break;
                }
            }

            if (nIndex < 0)
                return false;

            string strValue1 = strValue.Substring(0, nIndex).Trim();
            string strValue2 = strValue.Substring(nIndex + 1).Trim();

            if (m_dicWindDirection.TryGetValue(strValue1, out windDirection))
            {
                return ReadFloat(strValue2, out windSpeed);
            }

            if (m_dicWindDirection.TryGetValue(strValue2, out windDirection) == false)
                return false;

            return ReadFloat(strValue1, out windSpeed);
        }

        private bool ReadFloat(string strValue, out float data)
        {
            data = 0;
            int len = strValue.Length;

            int nIndex = len;
            bool readPoint = false;

            for (int i=0;i<len;i++)
            {
                char ch = strValue[i];

                if (i == 0)
                {
                    if (ch == '-' || ch == '+')
                        continue;
                    else
                        return false;
                }
                else
                {
                    if (ch == '.')
                    {
                        if (readPoint == false)
                            readPoint = true;
                        else
                        {
                            nIndex = i;
                            break;
                        }
                    }
                    else if (ch < '0' || ch > '9')
                    {
                        nIndex = i;
                        break;
                    }
                }
            }

            strValue = strValue.Substring(0, nIndex).Trim();
            return float.TryParse(strValue, out data);
        }

        private string ReadData(string strHTML, string strTag)
        {
            int nIndex = strHTML.IndexOf(strTag);

            if (nIndex < 0)
                return null;

            int nIndex1 = strHTML.IndexOf('>', nIndex + strTag.Length);
            int nIndex2 = strHTML.IndexOf('<', nIndex + strTag.Length);

            if (nIndex1 < 0 || nIndex2 < nIndex1)
                return null;

            string strValue = strHTML.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1).Trim();
            return strValue;
        }
    }
}
