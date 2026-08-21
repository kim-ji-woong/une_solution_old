using System.Collections.Generic;

namespace Weather.IDAL
{
    using Model;

    public interface IDelete
    {
        bool DeleteSite(int id, out string strErrorMessage);
        bool DeleteSite(Dictionary<Site.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteCurrent(int nWeatherSiteID, out string strErrorMessage);
        bool DeleteCurrent(Dictionary<Current.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteSpecialReport(int nWeatherSiteID, out string strErrorMessage);
        bool DeleteSpecialReport(Dictionary<SpecialReport.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        bool DeleteWeekly(int nWeatherSiteID, out string strErrorMessage);
        bool DeleteWeekly(Dictionary<Weekly.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
    }
}
