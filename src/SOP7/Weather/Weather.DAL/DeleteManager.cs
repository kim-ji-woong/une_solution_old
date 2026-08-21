using System.Collections;
using System.Collections.Generic;
using dnsDBUtil;

namespace Weather.DAL
{
    using Model;
    using IDAL;

    public class DeleteManager : QueryManager, IDelete
    {
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        public DeleteManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public bool DeleteCurrent(int nWeatherSiteID, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2}", Current.TableName, Current.GetFieldName(Current.Fields.WeatherSiteID, out isNullable), nWeatherSiteID);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteCurrent(Dictionary<Current.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Current.Fields>(ref strCondition, dicConditions, Current.GetFieldName, Current.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Current.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSite(int id, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2}", Site.TableName, Site.GetFieldName(Site.Fields.ID, out isNullable), id);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSite(Dictionary<Site.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Site.Fields>(ref strCondition, dicConditions, Site.GetFieldName, Site.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Site.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSpecialReport(int nWeatherSiteID, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2}", SpecialReport.TableName, SpecialReport.GetFieldName(SpecialReport.Fields.WeatherSiteID, out isNullable), nWeatherSiteID);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteSpecialReport(Dictionary<SpecialReport.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<SpecialReport.Fields>(ref strCondition, dicConditions, SpecialReport.GetFieldName, SpecialReport.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", SpecialReport.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteWeekly(int nWeatherSiteID, out string strErrorMessage)
        {
            bool isNullable;
            string strSQL = string.Format("Delete from {0} where {1} = {2}", Weekly.TableName, Weekly.GetFieldName(Weekly.Fields.WeatherSiteID, out isNullable), nWeatherSiteID);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }

        public bool DeleteWeekly(Dictionary<Weekly.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";

            if (SetCondition<Weekly.Fields>(ref strCondition, dicConditions, Weekly.GetFieldName, Weekly.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Delete from {0}", Weekly.TableName);

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            strErrorMessage = null;
            return true;
        }
    }
}
