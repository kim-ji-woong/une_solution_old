using System.Collections.Generic;

namespace Weather.IDAL
{
    using Model;

    public interface ISelect
    {
        Site SelectSite(int id, out string strErrorMessage);
        List<Site> SelectSites(Dictionary<Site.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Site> SelectSites(Dictionary<Site.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Current SelectCurrent(int nWeatherSiteID, out string strErrorMessage);
        List<Current> SelectCurrents(Dictionary<Current.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Current> SelectCurrents(Dictionary<Current.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        SpecialReport SelectSpecialReport(int nWeatherSiteID, out string strErrorMessage);
        List<SpecialReport> SelectSpecialReports(Dictionary<SpecialReport.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<SpecialReport> SelectSpecialReports(Dictionary<SpecialReport.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
        Weekly SelectWeekly(int id, out string strErrorMessage);
        List<Weekly> SelectWeeklys(Dictionary<Weekly.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage);
        List<Weekly> SelectWeeklys(Dictionary<Weekly.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage);
    }
}
