using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;

namespace WeatherSimulator
{
    public class DataManager
    {
        private WebDBManager m_dbMgr = null;

        // DB 데이터
        private List<RainNWind> m_rainDBDatas = new List<RainNWind>();
        private List<Typhoon> m_typhoonDBDatas = new List<Typhoon>();
        private List<Earthquake> m_earthquakeDBDatas = new List<Earthquake>();

        // 현재 작성중인 데이터
        private List<RainNWind> m_rainCurrentDatas = new List<RainNWind>();
        private List<Typhoon> m_typhoonCurrentDatas = new List<Typhoon>();
        private List<Earthquake> m_earthquakeCurrentDatas = new List<Earthquake>();

        private int m_nSOPGenUserID = -1;
        private int m_nSiteID = -1;

        private static DataManager m_instance = null;

        public static DataManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new DataManager();

                return m_instance;
            }
        }

        public List<RainNWind> RainDBDatas
        {
            get { return m_rainDBDatas; }
        }

        public List<Typhoon> TyphoonDBDatas
        {
            get { return m_typhoonDBDatas; }
        }

        public List<Earthquake> EarthquakeDBDatas
        {
            get { return m_earthquakeDBDatas; }
        }

        public List<RainNWind> RainCurrentDatas
        {
            get { return m_rainCurrentDatas; }
        }

        public List<Typhoon> TyphoonCurrentDatas
        {
            get { return m_typhoonCurrentDatas; }
        }

        public List<Earthquake> EarthquakeCurrentDatas
        {
            get { return m_earthquakeCurrentDatas; }
        }

        public int SOPGenUserID
        {
            get { return m_nSOPGenUserID; }
            set { m_nSOPGenUserID = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set
            {
                m_nSiteID = value;
                m_dbMgr = new WebDBManager(m_nSiteID); 
            }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        private DataManager()
        {
            
        }

        public bool LoadData(out DateTime dtCreate, out int nAvailablePeriodDay)
        {
            dtCreate = new DateTime();
            nAvailablePeriodDay = -1;

            m_rainDBDatas.Clear();
            m_typhoonDBDatas.Clear();
            m_earthquakeDBDatas.Clear();

            int nLogID;
            string strRemoveIDs = ReadLogID(out nLogID);

            if (strRemoveIDs == null)
                return false;

            RemoveExpiredDatas(strRemoveIDs);
            LoadWeatherList(nLogID);

            return LoadWeatherLog(nLogID, ref dtCreate, ref nAvailablePeriodDay);
        }

        private bool LoadWeatherLog(int nID, ref DateTime dtCreate, ref int nAvailablePeriodDay)
        {
            string strSQL = "Select CreatedTime, AvailablePeriod FROM Weather_Log where ID = " + nID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count != 2)
                return false;

            DateTime dtNull = new DateTime();
            dtCreate = WebDBManager.GetDateTimeField(arrResult[0], dtNull);
            nAvailablePeriodDay = WebDBManager.GetIntField(arrResult[1].ToString(), -1);

            return true;
        }

        private void LoadWeatherList(int nLogID)
        {
            if (nLogID < 0)
                return;

            string strRainIDs = "", strTyphoonIDs = "", strEarthquakeIDs = "";

            if (!LoadWeatherDatas(nLogID.ToString(), ref strRainIDs, ref strTyphoonIDs, ref strEarthquakeIDs))
                return;

            LoadRain(strRainIDs);
            LoadTyphoon(strTyphoonIDs);
            LoadEarthquake(strEarthquakeIDs);
        }

        private void LoadRain(string strRainIDs)
        {
            string strSQL = "Select ID, Time, RainHour, RainDay, WindSpeedAve, WindSpeedMax, Region FROM Weather_RainNWind where ID in (" + strRainIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtNull = new DateTime();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-6;i+=7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DateTime dtTime = WebDBManager.GetDateTimeField(arrResult[i + 1], dtNull);
                float fRainHour = WebDBManager.GetFloatField(arrResult[i + 2].ToString(), -1.0f);
                float fRainDay = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), -1.0f);
                float fSpeedAve = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), -1.0f);
                float fSpeedMax = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), -1.0f);
                string strRegion = WebDBManager.GetStringField(arrResult[i + 6], null);

                if (nID < 0)
                    continue;

                RainNWind rain = new RainNWind();

                rain.ID = nID;
                rain.Time = dtTime;

                if (fRainHour >= 0.0f)
                    rain.RainHour = new VariousData<float>(fRainHour);

                if (fRainDay >= 0.0f)
                    rain.RainDay = new VariousData<float>(fRainDay);

                if (fSpeedAve >= 0.0f)
                    rain.WindSpeedAve = new VariousData<float>(fSpeedAve);

                if (fSpeedMax >= 0.0f)
                    rain.WindSpeedMax = new VariousData<float>(fSpeedMax);

                if (strRegion != null && strRegion != "null")
                    rain.Region = strRegion;

                m_rainDBDatas.Add(rain);
            }
        }

        private void LoadTyphoon(string strTyphoonIDs)
        {
            string strSQL = "Select ID, Time, CenterLocation, CenterPressure, MaxSpeed, WindRadius, WindDirection, MoveSpeed, Etc FROM Weather_Typhoon where ID in (" + strTyphoonIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtNull = new DateTime();
            Typhoon.Direction dir;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DateTime dtTime = WebDBManager.GetDateTimeField(arrResult[i + 1], dtNull);
                string strCenterLocation = WebDBManager.GetStringField(arrResult[i + 2].ToString(), null);
                float fCenterPressure = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), -1.0f);
                float fMaxSpeed = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), -1.0f);
                float fRadius = WebDBManager.GetFloatField(arrResult[i + 5].ToString(), -1.0f);
                int nDirection = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                float fMoveSpeed = WebDBManager.GetFloatField(arrResult[i + 7].ToString(), -1.0f);
                string strEtc = WebDBManager.GetStringField(arrResult[i + 8], null);

                if (nID < 0)
                    continue;

                Typhoon typhoon = new Typhoon();

                typhoon.ID = nID;
                typhoon.Time = dtTime;

                if (strCenterLocation != null && strCenterLocation != "null")
                    typhoon.CenterLocation = strCenterLocation;

                if (fCenterPressure >= 0.0f)
                    typhoon.CenterPressure = new VariousData<float>(fCenterPressure);

                if (fMaxSpeed >= 0.0f)
                    typhoon.MaxSpeed = new VariousData<float>(fMaxSpeed);

                if (fRadius >= 0.0f)
                    typhoon.WindRadius = new VariousData<float>(fRadius);

                if (Typhoon.ToDirection(nDirection, out dir))
                    typhoon.WindDirection = new VariousData<Typhoon.Direction>(dir);

                if (fMoveSpeed >= 0.0f)
                    typhoon.MoveSpeed = new VariousData<float>(fMoveSpeed);

                if (strEtc != null && strEtc != "null")
                    typhoon.Etc = strEtc;

                m_typhoonDBDatas.Add(typhoon);
            }
        }

        private void LoadEarthquake(string strEarthquakeIDs)
        {
            string strSQL = "Select ID, Time, Location, Strength, TsunamiHeight, Etc FROM Weather_Earthquake where ID in (" + strEarthquakeIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return;

            DateTime dtNull = new DateTime();
            
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DateTime dtTime = WebDBManager.GetDateTimeField(arrResult[i + 1], dtNull);
                string strLocation = WebDBManager.GetStringField(arrResult[i + 2].ToString(), null);
                float fStrength = WebDBManager.GetFloatField(arrResult[i + 3].ToString(), -1.0f);
                float fTsunamiHeight = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), -1.0f);
                string strEtc = WebDBManager.GetStringField(arrResult[i + 5].ToString(), null);

                if (nID < 0)
                    continue;

                Earthquake earthquake = new Earthquake();

                earthquake.ID = nID;
                earthquake.Time = dtTime;

                if (strLocation != null && strLocation != "null")
                    earthquake.Location = strLocation;

                if (fStrength >= 0.0f)
                    earthquake.Strength = new VariousData<float>(fStrength);

                if (fTsunamiHeight >= 0.0f)
                    earthquake.TsunamiHeight = new VariousData<float>(fTsunamiHeight);

                if (strEtc != null && strEtc != "null")
                    earthquake.Etc = strEtc;

                m_earthquakeDBDatas.Add(earthquake);
            }
        }

        private bool LoadWeatherDatas(string strWeatherIDs, ref string strRainIDs, ref string strTyphoonIDs, ref string strEarthquakeIDs)
        {
            string strSQL = "Select DataID, DataType from Weather_List where WeatherID in (" + strWeatherIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nDataID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nDataType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                if (nDataType == (int)WeatherData.DataType.RainNWind)
                {
                    if (strRainIDs.Length == 0)
                        strRainIDs = nDataID.ToString();
                    else
                        strRainIDs += ", " + nDataID.ToString();
                }
                else if (nDataType == (int)WeatherData.DataType.Typhoon)
                {
                    if (strTyphoonIDs.Length == 0)
                        strTyphoonIDs = nDataID.ToString();
                    else
                        strTyphoonIDs += ", " + nDataID.ToString();
                }
                else if (nDataType == (int)WeatherData.DataType.Earthquake)
                {
                    if (strEarthquakeIDs.Length == 0)
                        strEarthquakeIDs = nDataID.ToString();
                    else
                        strEarthquakeIDs += ", " + nDataID.ToString();
                }
            }

            return true;
        }

        // 유효기간이 경과한 데이터들을 삭제한다.
        private void RemoveExpiredDatas(string strRemoveIDs)
        {
            if (strRemoveIDs.Length == 0)
                return;

            string strRainIDs = "", strTyphoonIDs = "", strEarthquakeIDs = "";

            if (!LoadWeatherDatas(strRemoveIDs, ref strRainIDs, ref strTyphoonIDs, ref strEarthquakeIDs))
                return;

            DeleteDatas("Weather_List", strRemoveIDs, "WeatherID");
            DeleteDatas("Weather_Log", strRemoveIDs);
            DeleteDatas("Weather_RainNWind", strRainIDs);
            DeleteDatas("Weather_Typhoon", strTyphoonIDs);
            DeleteDatas("Weather_Earthquake", strEarthquakeIDs);
        }

        private void DeleteDatas(string strTableName, string strIDs, string strKey = "ID")
        {
            if (strIDs.Length == 0)
                return;

            string strSQL = string.Format("Delete from {0} where {1} in ({2})", strTableName, strKey, strIDs);
            m_dbMgr.GetResultData(strSQL, 0);
        }

        private bool DeleteDatas(string strTableName, int nTranSaction, string strWhere = "")
        {
            string strSQL = "";
            
            if (strWhere.Length == 0)
                strSQL = string.Format("Delete from {0}", strTableName);
            else
                strSQL = string.Format("Delete from {0} where {1}", strTableName, strWhere);

            return m_dbMgr.GetResultData(strSQL, nTranSaction) != null;
        }

        // Return 값 : 유효기간이 경과한 Log Id들
        private string ReadLogID(out int nLogID)
        {
            nLogID = -1;

            // 가장 나중의 것부터 읽기 위하여 시간 반대순서대로 Query를 작성한다.
            string strSQL = "Select ID, CreatedTime, AvailablePeriod from Weather_Log where SiteID = " + m_nSiteID.ToString() + " order by CreatedTime desc";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return null;

            DateTime dtNull = new DateTime();
            DateTime dtNow = DateTime.Now;

            string strRemoveIDs = "";

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                DateTime dtCreate = WebDBManager.GetDateTimeField(arrResult[i + 1], dtNull);
                int nAvailablePeriod = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);

                if (CheckPeriod(ref dtNow, dtCreate, nAvailablePeriod))
                {
                    if (nLogID < 0)
                        nLogID = nID;
                }
                else
                {
                    if (strRemoveIDs.Length == 0)
                        strRemoveIDs = nID.ToString();
                    else
                        strRemoveIDs += ", " + nID.ToString();
                }
            }

            return strRemoveIDs;
        }

        // Return 값 : true이면 유효기간이 경과하지 않았다.
        //             false이면 유효기간이 지났다.
        private bool CheckPeriod(ref DateTime dtNow, DateTime dtCreate, int nAvailablePeriod)
        {
            if (nAvailablePeriod < 0)
                return true;

            TimeSpan span = dtNow - dtCreate;

            if (span.TotalDays >= nAvailablePeriod)
                return false;

            return true;
        }

        public bool SaveDB(int nAvailablePeriodDay, out bool dbIsEmpty)
        {
            dbIsEmpty = false;

            m_dbMgr.BeginBatch();

            if (!DeleteAll())
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            int nLogID = SaveWeatherLog(nAvailablePeriodDay);

            if (nLogID < 0)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            List<int> rainIDs = SaveRainNWind();
            
            if (rainIDs == null)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            List<int> typhoonIDs = SaveTyphoon();

            if (typhoonIDs == null)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            List<int> earthquakeIDs = SaveEarthquake();

            if (earthquakeIDs == null)
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            if (!SaveWeatherList(nLogID, rainIDs, WeatherData.DataType.RainNWind))
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            if (!SaveWeatherList(nLogID, typhoonIDs, WeatherData.DataType.Typhoon))
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            if (!SaveWeatherList(nLogID, earthquakeIDs, WeatherData.DataType.Earthquake))
            {
                m_dbMgr.BatchRollback();
                return false;
            }

            if (rainIDs.Count == 0 && typhoonIDs.Count == 0 && earthquakeIDs.Count == 0)
            {
                DeleteDatas("Weather_Log", 1, "SiteID = " + m_nSiteID.ToString());
                dbIsEmpty = true;
            }

            m_dbMgr.BatchCommit();
            return true;
        }

        private bool SaveWeatherList(int nLogID, List<int> dataIDs, WeatherData.DataType dataType)
        {
            int nID = -1;
            
            if (dataType == WeatherData.DataType.RainNWind)
                nID = GetMaxID("Weather_RainNWind", 1);
            else if (dataType == WeatherData.DataType.Typhoon)
                nID = GetMaxID("Weather_Typhoon", 1);
            else if (dataType == WeatherData.DataType.Earthquake)
                nID = GetMaxID("Weather_Earthquake", 1);

            if (nID < 0)
                return false;

            nID++;

            foreach (int nDataID in dataIDs)
            {
                string strSQL = string.Format("Insert into Weather_List (WeatherID, DataID, DataType) values ({0}, {1}, {2})",
                    nLogID, nDataID, (int)dataType);

                if (m_dbMgr.GetResultData(strSQL, 1) == null)
                    return false;
            }

            return true;
        }

        private int GetMaxID(string strTableName, int nTransaction)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, nTransaction);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            return WebDBManager.GetIntField(arrResult[0].ToString(), 0);
        }

        private List<int> SaveEarthquake()
        {
            int nID = GetMaxID("Weather_Earthquake", 1);

            if (nID < 0)
                return null;

            List<int> ids = new List<int>();

            string strFormat = "Insert into Weather_Earthquake (ID, Time, Location, Strength, TsunamiHeight, Etc, SiteID) values ";
            strFormat += "({0}, '{1}', {2}, {3}, {4}, {5}, {6})";

            foreach (Earthquake earthquake in m_earthquakeCurrentDatas)
            {
                string strTime = GetTimeString(earthquake.Time);

                string strSQL = string.Format(strFormat,
                    ++nID, strTime,
                    GetStringString(earthquake.Location),
                    GetFloatString(earthquake.Strength),
                    GetFloatString(earthquake.TsunamiHeight),
                    GetStringString(earthquake.Etc),
                    m_nSiteID);

                if (m_dbMgr.GetResultData(strSQL, 1) == null)
                    return null;
                else
                    ids.Add(nID);
            }

            return ids;
        }

        private List<int> SaveTyphoon()
        {
            int nID = GetMaxID("Weather_Typhoon", 1);

            if (nID < 0)
                return null;

            List<int> ids = new List<int>();

            string strFormat = "Insert into Weather_Typhoon (ID, Time, CenterLocation, CenterPressure, MaxSpeed, WindRadius, WindDirection, MoveSpeed, Etc, SiteID) values ";
            strFormat += "({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})";

            foreach (Typhoon typhoon in m_typhoonCurrentDatas)
            {
                string strTime = GetTimeString(typhoon.Time);

                string strSQL = string.Format(strFormat,
                    ++nID, strTime,
                    GetStringString(typhoon.CenterLocation),
                    GetFloatString(typhoon.CenterPressure),
                    GetFloatString(typhoon.MaxSpeed),
                    GetFloatString(typhoon.WindRadius),
                    typhoon.WindDirection == null ? "NULL" : ((int)typhoon.WindDirection.Data).ToString(),
                    GetFloatString(typhoon.MoveSpeed),
                    GetStringString(typhoon.Etc),
                    m_nSiteID);

                if (m_dbMgr.GetResultData(strSQL, 1) == null)
                    return null;
                else
                    ids.Add(nID);
            }

            return ids;
        }

        private List<int> SaveRainNWind()
        {
            int nID = GetMaxID("Weather_RainNWind", 1);

            if (nID < 0)
                return null;

            List<int> ids = new List<int>();

            string strFormat = "Insert into Weather_RainNWind (ID, Time, RainHour, RainDay, WindSpeedAve, WindSpeedMax, Region, SiteID) values ";
            strFormat += "({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7})";

            foreach (RainNWind rain in m_rainCurrentDatas)
            {
                string strTime = GetTimeString(rain.Time);

                string strSQL = string.Format(strFormat,
                    ++nID, strTime,
                    GetFloatString(rain.RainHour),
                    GetFloatString(rain.RainDay),
                    GetFloatString(rain.WindSpeedAve),
                    GetFloatString(rain.WindSpeedMax),
                    GetStringString(rain.Region),
                    m_nSiteID);

                if (m_dbMgr.GetResultData(strSQL, 1) == null)
                    return null;
                else
                    ids.Add(nID);
            }

            return ids;
        }

        private string GetStringString(string data)
        {
            return data == null ? "NULL" : "'" + data + "'";
        }

        private string GetFloatString(VariousData<float> data)
        {
            return data == null ? "NULL" : data.Data.ToString();
        }

        private int SaveWeatherLog(int nAvailablePeriodDay)
        {
            int nID = GetMaxID("Weather_Log", 1);

            if (nID < 0)
                return nID;

            nID++;

            string strTime = GetTimeString(DateTime.Now);

            string strSQL = string.Format("Insert into Weather_Log (ID, CreatedTime, AvailablePeriod, SOPGenUserID, SiteID) values ({0}, '{1}', {2}, {3}, {4})",
                nID, strTime, nAvailablePeriodDay <= 0 ? "NULL" : nAvailablePeriodDay.ToString(), m_nSOPGenUserID, m_nSiteID);

            if (m_dbMgr.GetResultData(strSQL, 1) == null)
                return -1;

            return nID;
        }

        private string GetTimeString(DateTime time)
        {
            string strTime = string.Format("{0}-{1}-{2} {3}:{4}:{5}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
            return strTime;
        }

        private bool DeleteAll()
        {
            string strWeatherListWhere = "WeatherID in (select Weather_List.WeatherID from Weather_List, Weather_Log where Weather_List.WeatherID = Weather_Log.ID and Weather_Log.SiteID = " + m_nSiteID.ToString() + ")";
            if (!DeleteDatas("Weather_List", 1, strWeatherListWhere))
                return false;

            if (!DeleteDatas("Weather_Log", 1, "SiteID = " + m_nSiteID.ToString()))
                return false;

            if (!DeleteDatas("Weather_RainNWind", 1, "SiteID = " + m_nSiteID.ToString()))
                return false;

            if (!DeleteDatas("Weather_Typhoon", 1, "SiteID = " + m_nSiteID.ToString()))
                return false;

            if (!DeleteDatas("Weather_Earthquake", 1, "SiteID = " + m_nSiteID.ToString()))
                return false;

            return true;
        }
    }
}
