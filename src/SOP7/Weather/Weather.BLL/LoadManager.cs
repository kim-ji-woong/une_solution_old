using System.Collections.Generic;

namespace Weather.BLL
{
    using Model;
    using IDAL;
    using Models.Response;

    public class LoadManager
    {
        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        public LoadManager(IDataManager dataManager, ProcessManager processManager)
        {
            m_dataManager = dataManager;
            m_processManager = processManager;
        }

        public ResponseWeatherInfo GetWeatherInfo()
        {
            string strErrorMessage;
            List<Site> sites = m_dataManager.GetSelectManager().SelectSites(null, null, out strErrorMessage);

            if (sites == null)
                return new ResponseWeatherInfo(false, strErrorMessage);

            if (sites.Count == 0)
                return new ResponseWeatherInfo(false, "날씨정보를 조회할 대상이 존재하지 않습니다.");

            string strSiteIDs = "";
            Dictionary<int, Site> dicSites = new Dictionary<int, Site>();

            foreach (Site site in sites)
            {
                if (strSiteIDs.Length == 0)
                    strSiteIDs = site.ID.ToString();
                else
                    strSiteIDs += ", " + site.ID.ToString();

                dicSites[site.ID] = site;
            }

            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", Current.GetFieldName(Current.Fields.WeatherSiteID, out isNullable), strSiteIDs);
            List<Current> currents = m_dataManager.GetSelectManager().SelectCurrents(null, strCondition, out strErrorMessage);

            if (currents == null)
                return new ResponseWeatherInfo(false, strErrorMessage);

            List<SpecialReport> reports = m_dataManager.GetSelectManager().SelectSpecialReports(null, strCondition, out strErrorMessage);

            if (reports == null)
                return new ResponseWeatherInfo(false, strErrorMessage);

            return MakeWeatherInfo(dicSites, currents, reports);
        }

        private ResponseWeatherInfo MakeWeatherInfo(Dictionary<int, Site> dicSites, List<Current> currents, List<SpecialReport> reports)
        {
            Site site;
            WeatherData data;
            Dictionary<int, WeatherData> dicWeatherDatas = new Dictionary<int, WeatherData>();

            foreach (Current current in currents)
            {
                if (dicSites.TryGetValue(current.WeatherSiteID, out site))
                {
                    if (dicWeatherDatas.TryGetValue(site.ID, out data) == false)
                    {
                        data = new WeatherData();
                        dicWeatherDatas[site.ID] = data;

                        data.Site = site;
                    }

                    data.Current = current;
                }
            }

            foreach (SpecialReport report in reports)
            {
                if (dicSites.TryGetValue(report.WeatherSiteID, out site))
                {
                    if (dicWeatherDatas.TryGetValue(site.ID, out data) == false)
                    {
                        data = new WeatherData();
                        dicWeatherDatas[site.ID] = data;

                        data.Site = site;
                    }

                    data.SpecialReport = report;
                }
            }

            ResponseWeatherInfo response = new ResponseWeatherInfo(true, "");

            foreach (KeyValuePair<int, WeatherData> pair in dicWeatherDatas)
            {
                response.Datas.Add(pair.Value);
            }

            return response;
        }

        public ResponseWeatherWeeklyInfo GetWeatherWeeklyInfo()
        {
            string strErrorMessage;
            ResponseWeatherWeeklyInfo responseWeatherWeeklyInfo = new ResponseWeatherWeeklyInfo();

            List<Site> sites = m_dataManager.GetSelectManager().SelectSites(null, null, out strErrorMessage);

            if (sites == null)
            {
                responseWeatherWeeklyInfo.Success = false;
                responseWeatherWeeklyInfo.Message = strErrorMessage;
                return responseWeatherWeeklyInfo;
            }
            else if (sites.Count == 0)
            {
                responseWeatherWeeklyInfo.Success = false;
                responseWeatherWeeklyInfo.Message = "날씨정보를 조회할 대상이 존재하지 않습니다.";
                return responseWeatherWeeklyInfo;
            }

            List<WeatherWeeklyData> weatherWeeklyDatas = new List<WeatherWeeklyData>();

            foreach (Site site in sites)
            {
                Dictionary<Weekly.Fields, object> dicConditions = new Dictionary<Weekly.Fields, object>();
                dicConditions[Weekly.Fields.WeatherSiteID] = site.ID;

                List<Weekly> weeklies = m_dataManager.GetSelectManager().SelectWeeklys(dicConditions, null, out strErrorMessage);

                if (weeklies == null)
                {
                    responseWeatherWeeklyInfo.Success = false;
                    responseWeatherWeeklyInfo.Message = strErrorMessage;
                    return responseWeatherWeeklyInfo;
                } 
                else if (weeklies.Count == 0)
                {
                    responseWeatherWeeklyInfo.Success = false;
                    responseWeatherWeeklyInfo.Message = "SelectWeeklys 조회 실패. 해당 site ID 정보가 조회되지 않음";
                    return responseWeatherWeeklyInfo;
                }

                WeatherWeeklyData weeklyData = new WeatherWeeklyData();
                weeklyData.Site = site;
                weeklyData.Weekly = weeklies[0];

                weatherWeeklyDatas.Add(weeklyData);
            }

            responseWeatherWeeklyInfo.Datas = weatherWeeklyDatas;
            responseWeatherWeeklyInfo.Success = true;
            return responseWeatherWeeklyInfo;

            
        }
    }
}
