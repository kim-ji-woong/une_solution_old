using System;
using System.Collections;
using System.Collections.Generic;
using dnsDBUtil;

namespace Weather.DAL
{
    using IDAL;
    using Weather.Model;

    public class CreateManager : QueryManager, ICreate
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        private const int FindCountLimit = 100;

        public CreateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public string GetErrorMessage()
        {
            return m_strErrorMessage;
        }

        public Current CreateCurrent(int nWeatherSiteID, int nState, float fTemp, float? fSensibleTemp, float fRain, float fHumidity, float? fWindSpeed, int? nWindDir, float? fAtm, DateTime dtUpdate)
        {
            Dictionary<Current.Fields, object> dicFieldDatas = new Dictionary<Current.Fields, object>();
            dicFieldDatas[Current.Fields.WeatherSiteID] = nWeatherSiteID;
            dicFieldDatas[Current.Fields.State] = nState;
            dicFieldDatas[Current.Fields.Temperature] = fTemp;
            dicFieldDatas[Current.Fields.SensibleTemp] = fSensibleTemp;
            dicFieldDatas[Current.Fields.Rain] = fRain;
            dicFieldDatas[Current.Fields.Humidity] = fHumidity;
            dicFieldDatas[Current.Fields.WindSpeed] = fWindSpeed;
            dicFieldDatas[Current.Fields.WindDirection] = nWindDir;
            dicFieldDatas[Current.Fields.Atm] = fAtm;
            dicFieldDatas[Current.Fields.UpdateTime] = dtUpdate;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                Current.TableName,
                GetFieldNames<Current.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                Current current = new Current();

                current.Atm = fAtm;
                current.Humidity = fHumidity;
                current.Rain = fRain;
                current.SensibleTemp = fSensibleTemp;
                current.State = nState;
                current.Temperature = fTemp;
                current.UpdateTime = dtUpdate;
                current.WeatherSiteID = nWeatherSiteID;
                current.WindDirection = nWindDir;
                current.WindSpeed = fWindSpeed;

                return current;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public Site CreateSite(string strName, string strDescription = null)
        {
            Dictionary<Site.Fields, object> dicFieldDatas = new Dictionary<Site.Fields, object>();
            dicFieldDatas[Site.Fields.Name] = strName;
            dicFieldDatas[Site.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Site.TableName,
                GetFieldNames<Site.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Site.GetFieldName(Site.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Site> sites = m_dataManager.GetSelectManager().SelectSites(null, strCondition, 1, out strErrorMessage);

                if (sites == null || sites.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSite(sites[0], strName, strDescription))
                    return sites[0];

                return GetSite(strName, strDescription, Site.TableName, sites[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Site GetSite(string strName, string strDescription, string strTableName, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Site.GetFieldName(Site.Fields.ID, out isNullable), id);

            List<Site> sites = m_dataManager.GetSelectManager().SelectSites(null, strCondition, nCount, out strErrorMessage);

            if (sites == null)
                return null;

            foreach (Site site in sites)
            {
                if (IsSameSite(site, strName, strDescription))
                    return site;
            }

            if (nCount < nLimit)
                return GetSite(strName, strDescription, strTableName, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(strTableName);
            return null;
        }

        private bool IsSameSite(Site site, string strName, string strDescription)
        {
            if (site.Name == strName &&
                site.Description == strDescription)
                return true;

            return false;
        }

        public SpecialReport CreateSpecialReport(int nWeatherSiteID, DateTime dtUpdate, string strURL = null, string strImageURL = null)
        {
            Dictionary<SpecialReport.Fields, object> dicFieldDatas = new Dictionary<SpecialReport.Fields, object>();
            dicFieldDatas[SpecialReport.Fields.WeatherSiteID] = nWeatherSiteID;
            dicFieldDatas[SpecialReport.Fields.Url] = strURL;
            dicFieldDatas[SpecialReport.Fields.ImageUrl] = strImageURL;
            dicFieldDatas[SpecialReport.Fields.UpdateTime] = dtUpdate;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                SpecialReport.TableName,
                GetFieldNames<SpecialReport.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                SpecialReport report = new SpecialReport();

                report.ImageUrl = strImageURL;
                report.UpdateTime = dtUpdate;
                report.Url = strURL;
                report.WeatherSiteID = nWeatherSiteID;

                return report;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public Weekly CreateWeekly(int nWeatherSiteID, float fOneDayLaterTemp, int nOneDayLaterState, float fTwoDayLaterTemp, int nTwoDayLaterState, float fThreeDayLaterTemp, int nThreeDayLaterState, float fFourDayLaterTemp, int nFourDayLaterState, float fFiveDayLaterTemp, int nFiveDayLaterState, float fSixDayLaterTemp, int nSixDayLaterState, DateTime dtUpdate)
        {
            Dictionary<Weekly.Fields, object> dicFieldDatas = new Dictionary<Weekly.Fields, object>();
            dicFieldDatas[Weekly.Fields.WeatherSiteID] = nWeatherSiteID;
            dicFieldDatas[Weekly.Fields.OneDayLaterTemp] = fOneDayLaterTemp;
            dicFieldDatas[Weekly.Fields.OneDayLaterState] = nOneDayLaterState;
            dicFieldDatas[Weekly.Fields.TwoDayLaterTemp] = fTwoDayLaterTemp;
            dicFieldDatas[Weekly.Fields.TwoDayLaterState] = nTwoDayLaterState;
            dicFieldDatas[Weekly.Fields.ThreeDayLaterTemp] = fThreeDayLaterTemp;
            dicFieldDatas[Weekly.Fields.ThreeDayLaterState] = nThreeDayLaterState;
            dicFieldDatas[Weekly.Fields.FourDayLaterTemp] = fFourDayLaterTemp;
            dicFieldDatas[Weekly.Fields.FourDayLaterState] = nFourDayLaterState;
            dicFieldDatas[Weekly.Fields.FiveDayLaterTemp] = fFiveDayLaterTemp;
            dicFieldDatas[Weekly.Fields.FiveDayLaterState] = nFiveDayLaterState;
            dicFieldDatas[Weekly.Fields.SixDayLaterTemp] = fSixDayLaterTemp;
            dicFieldDatas[Weekly.Fields.SixDayLaterState] = nSixDayLaterState;
            dicFieldDatas[Weekly.Fields.UpdateTime] = dtUpdate;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                Weekly.TableName,
                GetFieldNames<Weekly.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                Weekly weekly = new Weekly();

                weekly.FiveDayLaterState = nFiveDayLaterState;
                weekly.FiveDayLaterTemp = fFiveDayLaterTemp;
                weekly.FourDayLaterState = nFourDayLaterState;
                weekly.FourDayLaterTemp = fFourDayLaterTemp;
                weekly.OneDayLaterState = nOneDayLaterState;
                weekly.OneDayLaterTemp = fOneDayLaterTemp;
                weekly.SixDayLaterState = nSixDayLaterState;
                weekly.SixDayLaterTemp = fSixDayLaterTemp;
                weekly.ThreeDayLaterState = nThreeDayLaterState;
                weekly.ThreeDayLaterTemp = fThreeDayLaterTemp;
                weekly.TwoDayLaterState = nTwoDayLaterState;
                weekly.TwoDayLaterTemp = fTwoDayLaterTemp;
                weekly.UpdateTime = dtUpdate;
                weekly.WeatherSiteID = nWeatherSiteID;

                return weekly;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private string GetInsertErrorMessage(string tableName)
        {
            return string.Format("{0} 테이블의 데이터 삽입에 실패하였습니다.", tableName);
        }
    }
}
