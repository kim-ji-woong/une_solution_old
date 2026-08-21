using System;
using System.Collections;
using System.Collections.Generic;
using dnsDBUtil;

namespace Weather.DAL
{
    using Model;
    using IDAL;

    public class SelectManager : QueryManager, ISelect
    {
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        public SelectManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public Current SelectCurrent(int nWeatherSiteID, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3}", GetFieldNames<Current.Fields>(out nFieldCount), Current.TableName, Current.GetFieldName(Current.Fields.WeatherSiteID, out isNullable), nWeatherSiteID);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Current model = ReadCurrent(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<Current> SelectCurrents(Dictionary<Current.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectCurrents(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Current> SelectCurrents(Dictionary<Current.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Current.Fields>(out nFieldCount), Current.TableName);

            string strCondition = "";

            if (SetCondition<Current.Fields>(ref strCondition, dicConditions, Current.GetFieldName, Current.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<Current> currents = new List<Current>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Current model = ReadCurrent(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    currents.Add(model);
            }

            return currents;
        }

        private Current ReadCurrent(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool isNullable;
            Current model = new Current();

            foreach (Current.Fields field in Current.Fields.GetValues(typeof(Current.Fields)))
            {
                string strFieldName = Current.GetFieldName(field, out isNullable);

                if (field == Current.Fields.WeatherSiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.WeatherSiteID = data.Data;
                    }
                }
                else if (field == Current.Fields.State)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.State = data.Data;
                    }
                }
                else if (field == Current.Fields.Temperature)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Temperature = data.Data;
                }
                else if (field == Current.Fields.SensibleTemp)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.SensibleTemp = null;
                    }
                    else
                        model.SensibleTemp = data.Data;
                }
                else if (field == Current.Fields.Rain)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Rain = data.Data;
                }
                else if (field == Current.Fields.Humidity)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Humidity = data.Data;
                }
                else if (field == Current.Fields.WindSpeed)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.WindSpeed = null;
                    }
                    else
                        model.WindSpeed = data.Data;
                }
                else if (field == Current.Fields.WindDirection)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.WindDirection = null;
                    }
                    else
                        model.WindDirection = data.Data;
                }
                else if (field == Current.Fields.Atm)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Atm = null;
                    }
                    else
                        model.Atm = data.Data;
                }
                else if (field == Current.Fields.UpdateTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.UpdateTime = data.Data;
                }

                index++;
            }

            return model;
        }

        public Site SelectSite(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3}", GetFieldNames<Site.Fields>(out nFieldCount), Site.TableName, Site.GetFieldName(Site.Fields.ID, out isNullable), id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Site model = ReadSite(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<Site> SelectSites(Dictionary<Site.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSites(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Site> SelectSites(Dictionary<Site.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Site.Fields>(out nFieldCount), Site.TableName);

            string strCondition = "";

            if (SetCondition<Site.Fields>(ref strCondition, dicConditions, Site.GetFieldName, Site.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<Site> sites = new List<Site>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Site model = ReadSite(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    sites.Add(model);
            }

            return sites;
        }

        private Site ReadSite(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool isNullable;
            Site model = new Site();

            foreach (Site.Fields field in Site.Fields.GetValues(typeof(Site.Fields)))
            {
                string strFieldName = Site.GetFieldName(field, out isNullable);

                if (field == Site.Fields.ID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ID = data.Data;
                    }
                }
                else if (field == Site.Fields.Name)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Name = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Name = data;
                }
                else if (field == Site.Fields.Description)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Description = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Description = data;
                }
                
                index++;
            }

            return model;
        }

        public SpecialReport SelectSpecialReport(int nWeatherSiteID, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3}", GetFieldNames<SpecialReport.Fields>(out nFieldCount), SpecialReport.TableName, SpecialReport.GetFieldName(SpecialReport.Fields.WeatherSiteID, out isNullable), nWeatherSiteID);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                SpecialReport model = ReadSpecialReport(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<SpecialReport> SelectSpecialReports(Dictionary<SpecialReport.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSpecialReports(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<SpecialReport> SelectSpecialReports(Dictionary<SpecialReport.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<SpecialReport.Fields>(out nFieldCount), SpecialReport.TableName);

            string strCondition = "";

            if (SetCondition<SpecialReport.Fields>(ref strCondition, dicConditions, SpecialReport.GetFieldName, SpecialReport.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<SpecialReport> reports = new List<SpecialReport>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                SpecialReport model = ReadSpecialReport(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    reports.Add(model);
            }

            return reports;
        }

        private SpecialReport ReadSpecialReport(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool isNullable;
            SpecialReport model = new SpecialReport();

            foreach (SpecialReport.Fields field in SpecialReport.Fields.GetValues(typeof(SpecialReport.Fields)))
            {
                string strFieldName = SpecialReport.GetFieldName(field, out isNullable);

                if (field == SpecialReport.Fields.WeatherSiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.WeatherSiteID = data.Data;
                    }
                }
                else if (field == SpecialReport.Fields.Url)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Url = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Url = data;
                }
                else if (field == SpecialReport.Fields.ImageUrl)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.ImageUrl = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ImageUrl = data;
                }
                else if (field == SpecialReport.Fields.UpdateTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.UpdateTime = data.Data;
                }

                index++;
            }

            return model;
        }

        public Weekly SelectWeekly(int nWeatherSiteID, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3}", GetFieldNames<Weekly.Fields>(out nFieldCount), Weekly.TableName, Weekly.GetFieldName(Weekly.Fields.WeatherSiteID, out isNullable), nWeatherSiteID);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Weekly model = ReadWeekly(arrResult, 0, out strErrorMessage);

                if (model == null)
                    return null;

                return model;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<Weekly> SelectWeeklys(Dictionary<Weekly.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectWeeklys(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Weekly> SelectWeeklys(Dictionary<Weekly.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Weekly.Fields>(out nFieldCount), Weekly.TableName);

            string strCondition = "";

            if (SetCondition<Weekly.Fields>(ref strCondition, dicConditions, Weekly.GetFieldName, Weekly.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
            {
                if (strCondition.Trim().ToLower().StartsWith("order by"))
                    strSQL += " " + strCondition;
                else
                    strSQL += " where " + strCondition;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<Weekly> weeklys = new List<Weekly>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Weekly model = ReadWeekly(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    weeklys.Add(model);
            }

            return weeklys;
        }

        private Weekly ReadWeekly(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool isNullable;
            Weekly model = new Weekly();

            foreach (Weekly.Fields field in Weekly.Fields.GetValues(typeof(Weekly.Fields)))
            {
                string strFieldName = Weekly.GetFieldName(field, out isNullable);

                if (field == Weekly.Fields.WeatherSiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.WeatherSiteID = data.Data;
                    }
                }
                else if (field == Weekly.Fields.OneDayLaterTemp)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.OneDayLaterTemp = data.Data;
                }
                else if (field == Weekly.Fields.OneDayLaterState)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.OneDayLaterState = data.Data;
                    }
                }
                else if (field == Weekly.Fields.TwoDayLaterTemp)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.TwoDayLaterTemp = data.Data;
                }
                else if (field == Weekly.Fields.TwoDayLaterState)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.TwoDayLaterState = data.Data;
                    }
                }
                else if (field == Weekly.Fields.ThreeDayLaterTemp)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ThreeDayLaterTemp = data.Data;
                }
                else if (field == Weekly.Fields.ThreeDayLaterState)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ThreeDayLaterState = data.Data;
                    }
                }
                else if (field == Weekly.Fields.FourDayLaterTemp)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.FourDayLaterTemp = data.Data;
                }
                else if (field == Weekly.Fields.FourDayLaterState)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.FourDayLaterState = data.Data;
                    }
                }
                else if (field == Weekly.Fields.FiveDayLaterTemp)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.FiveDayLaterTemp = data.Data;
                }
                else if (field == Weekly.Fields.FiveDayLaterState)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.FiveDayLaterState = data.Data;
                    }
                }
                else if (field == Weekly.Fields.SixDayLaterTemp)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.SixDayLaterTemp = data.Data;
                }
                else if (field == Weekly.Fields.SixDayLaterState)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SixDayLaterState = data.Data;
                    }
                }
                else if (field == Weekly.Fields.UpdateTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.UpdateTime = data.Data;
                }

                index++;
            }

            return model;
        }
    }
}
