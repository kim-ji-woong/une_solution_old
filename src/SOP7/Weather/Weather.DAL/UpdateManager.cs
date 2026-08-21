using System.Collections;
using System.Collections.Generic;
using dnsDBUtil;

namespace Weather.DAL
{
    using Model;
    using IDAL;

    public class UpdateManager : QueryManager, IUpdate
    {
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        public UpdateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public bool UpdateCurrent(Current current, out string strErrorMessage)
        {
            Dictionary<Current.Fields, object> dicSets = new Dictionary<Current.Fields, object>();
            dicSets[Current.Fields.State] = current.State;
            dicSets[Current.Fields.Atm] = current.Atm;
            dicSets[Current.Fields.Humidity] = current.Humidity;
            dicSets[Current.Fields.Rain] = current.Rain;
            dicSets[Current.Fields.SensibleTemp] = current.SensibleTemp;
            dicSets[Current.Fields.Temperature] = current.Temperature;
            dicSets[Current.Fields.UpdateTime] = current.UpdateTime;
            dicSets[Current.Fields.WindDirection] = current.WindDirection;
            dicSets[Current.Fields.WindSpeed] = current.WindSpeed;

            Dictionary<Current.Fields, object> dicConditions = new Dictionary<Current.Fields, object>();
            dicConditions[Current.Fields.WeatherSiteID] = current.WeatherSiteID;

            return UpdateCurrent(dicSets, dicConditions, null, out strErrorMessage);
        }

        public bool UpdateCurrent(Dictionary<Current.Fields, object> dicSets, Dictionary<Current.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Current.Fields>(ref strSets, dicSets, Current.GetFieldName, Current.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Current.Fields>(ref strCondition, dicConditions, Current.GetFieldName, Current.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Current.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSite(Site site, out string strErrorMessage)
        {
            Dictionary<Site.Fields, object> dicSets = new Dictionary<Site.Fields, object>();
            dicSets[Site.Fields.Name] = site.Name;
            dicSets[Site.Fields.Description] = site.Description;

            Dictionary<Site.Fields, object> dicConditions = new Dictionary<Site.Fields, object>();
            dicConditions[Site.Fields.ID] = site.ID;

            return UpdateSite(dicSets, dicConditions, null, out strErrorMessage);
        }

        public bool UpdateSite(Dictionary<Site.Fields, object> dicSets, Dictionary<Site.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Site.Fields>(ref strSets, dicSets, Site.GetFieldName, Site.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Site.Fields>(ref strCondition, dicConditions, Site.GetFieldName, Site.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Site.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSpecialReport(SpecialReport report, out string strErrorMessage)
        {
            Dictionary<SpecialReport.Fields, object> dicSets = new Dictionary<SpecialReport.Fields, object>();
            dicSets[SpecialReport.Fields.Url] = report.Url;
            dicSets[SpecialReport.Fields.ImageUrl] = report.ImageUrl;
            dicSets[SpecialReport.Fields.UpdateTime] = report.UpdateTime;

            Dictionary<SpecialReport.Fields, object> dicConditions = new Dictionary<SpecialReport.Fields, object>();
            dicConditions[SpecialReport.Fields.WeatherSiteID] = report.WeatherSiteID;

            return UpdateSpecialReport(dicSets, dicConditions, null, out strErrorMessage);
        }

        public bool UpdateSpecialReport(Dictionary<SpecialReport.Fields, object> dicSets, Dictionary<SpecialReport.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SpecialReport.Fields>(ref strSets, dicSets, SpecialReport.GetFieldName, SpecialReport.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SpecialReport.Fields>(ref strCondition, dicConditions, SpecialReport.GetFieldName, SpecialReport.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", SpecialReport.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateWeekly(Weekly weekly, out string strErrorMessage)
        {
            Dictionary<Weekly.Fields, object> dicSets = new Dictionary<Weekly.Fields, object>();
            dicSets[Weekly.Fields.OneDayLaterTemp] = weekly.OneDayLaterTemp;
            dicSets[Weekly.Fields.OneDayLaterState] = weekly.OneDayLaterState;
            dicSets[Weekly.Fields.TwoDayLaterTemp] = weekly.TwoDayLaterTemp;
            dicSets[Weekly.Fields.TwoDayLaterState] = weekly.TwoDayLaterState;
            dicSets[Weekly.Fields.ThreeDayLaterTemp] = weekly.ThreeDayLaterTemp;
            dicSets[Weekly.Fields.ThreeDayLaterState] = weekly.ThreeDayLaterState;
            dicSets[Weekly.Fields.FourDayLaterTemp] = weekly.FourDayLaterTemp;
            dicSets[Weekly.Fields.FourDayLaterState] = weekly.FourDayLaterState;
            dicSets[Weekly.Fields.FiveDayLaterTemp] = weekly.FiveDayLaterTemp;
            dicSets[Weekly.Fields.FiveDayLaterState] = weekly.FiveDayLaterState;
            dicSets[Weekly.Fields.SixDayLaterTemp] = weekly.SixDayLaterTemp;
            dicSets[Weekly.Fields.SixDayLaterState] = weekly.SixDayLaterState;
            dicSets[Weekly.Fields.UpdateTime] = weekly.UpdateTime;

            Dictionary<Weekly.Fields, object> dicConditions = new Dictionary<Weekly.Fields, object>();
            dicConditions[Weekly.Fields.WeatherSiteID] = weekly.WeatherSiteID;

            return UpdateWeekly(dicSets, dicConditions, null, out strErrorMessage);
        }

        public bool UpdateWeekly(Dictionary<Weekly.Fields, object> dicSets, Dictionary<Weekly.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Weekly.Fields>(ref strSets, dicSets, Weekly.GetFieldName, Weekly.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Weekly.Fields>(ref strCondition, dicConditions, Weekly.GetFieldName, Weekly.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Weekly.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }
    }
}
