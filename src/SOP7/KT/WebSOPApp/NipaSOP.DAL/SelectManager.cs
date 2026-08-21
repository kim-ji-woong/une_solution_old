using System;
using System.Collections.Generic;
using System.Collections;
using dnsDBUtil;

namespace NipaSOP.DAL
{
    using IDAL;
    using NipaSOP.Model.Sop;

    public class SelectManager : QueryManager, ISelect
    {
        private DataManager m_dataManager = null;

        public SelectManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public LocationLinkedSOP SelectLocationLinkedSOP(int nFacilityID, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3}",
                GetFieldNames<LocationLinkedSOP.Fields>(out nFieldCount),
                LocationLinkedSOP.TableName,
                LocationLinkedSOP.GetFieldName(LocationLinkedSOP.Fields.FacilityID, out isNullable),
                nFacilityID);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                LocationLinkedSOP model = ReadLocationLinkedSOP(arrResult, 0, out strErrorMessage);

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

        public List<LocationLinkedSOP> SelectLocationLinkedSOPs(Dictionary<LocationLinkedSOP.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<LocationLinkedSOP.Fields>(out nFieldCount), LocationLinkedSOP.TableName);

            string strCondition = "";

            if (SetCondition<LocationLinkedSOP.Fields>(ref strCondition, dicConditions, LocationLinkedSOP.GetFieldName, LocationLinkedSOP.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<LocationLinkedSOP> sops = new List<LocationLinkedSOP>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                LocationLinkedSOP model = ReadLocationLinkedSOP(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    sops.Add(model);
            }

            return sops;
        }

        private LocationLinkedSOP ReadLocationLinkedSOP(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool isNullable;
            LocationLinkedSOP model = new LocationLinkedSOP();

            foreach (LocationLinkedSOP.Fields field in LocationLinkedSOP.Fields.GetValues(typeof(LocationLinkedSOP.Fields)))
            {
                string strFieldName = LocationLinkedSOP.GetFieldName(field, out isNullable);

                if (field == LocationLinkedSOP.Fields.FacilityID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.FacilityID = data.Data;
                    }
                }
                else if (field == LocationLinkedSOP.Fields.FacilityTypeID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.FacilityTypeID = data.Data;
                }
                else if (field == LocationLinkedSOP.Fields.DisasterCategoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.DisasterCategoryID = data.Data;
                }
                else if (field == LocationLinkedSOP.Fields.SubDisasterCategoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.SubDisasterCategoryID = data.Data;
                }
                else if (field == LocationLinkedSOP.Fields.DisasterName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.DisasterName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.DisasterName = data;
                    }
                }

                index++;
            }

            return model;
        }

        public StartInfo SelectStartInfo(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<StartInfo.Fields>(out nFieldCount), StartInfo.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                StartInfo model = ReadStartInfo(arrResult, 0, out strErrorMessage);

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

        public List<StartInfo> SelectStartInfos(Dictionary<StartInfo.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<StartInfo.Fields>(out nFieldCount), StartInfo.TableName);

            string strCondition = "";

            if (SetCondition<StartInfo.Fields>(ref strCondition, dicConditions, StartInfo.GetFieldName, StartInfo.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<StartInfo> sops = new List<StartInfo>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                StartInfo model = ReadStartInfo(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    sops.Add(model);
            }

            return sops;
        }

        private StartInfo ReadStartInfo(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool isNullable;
            StartInfo model = new StartInfo();

            foreach (StartInfo.Fields field in StartInfo.Fields.GetValues(typeof(StartInfo.Fields)))
            {
                string strFieldName = StartInfo.GetFieldName(field, out isNullable);

                if (field == StartInfo.Fields.ID)
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
                else if (field == StartInfo.Fields.AccessMode)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.AccessMode = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.AccessMode = data;
                }
                else if (field == StartInfo.Fields.AccessToken)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.AccessToken = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.AccessToken = data;
                }
                else if (field == StartInfo.Fields.FacilityID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.FacilityID = data.Data;
                }
                else if (field == StartInfo.Fields.ServiceType)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.ServiceType = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ServiceType = data;
                }
                else if (field == StartInfo.Fields.TimeStamp)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.TimeStamp = data.Data;
                }

                index++;
            }

            return model;
        }

        public Facility SelectFacility(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3}",
                GetFieldNames<Facility.Fields>(out nFieldCount),
                Facility.TableName,
                Facility.GetFieldName(Facility.Fields.ID, out isNullable),
                id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Facility model = ReadFacility(arrResult, 0, out strErrorMessage);

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

        public List<Facility> SelectFacilities(Dictionary<Facility.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Facility.Fields>(out nFieldCount), Facility.TableName);

            string strCondition = "";

            if (SetCondition<Facility.Fields>(ref strCondition, dicConditions, Facility.GetFieldName, Facility.TableName, ref strErrorMessage) == false)
                return null;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            if (strCondition.Length > 0)
                strSQL += " where " + strCondition;

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            int nResultCount = arrResult.Count;
            List<Facility> facilities = new List<Facility>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Facility model = ReadFacility(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    facilities.Add(model);
            }

            return facilities;
        }

        private Facility ReadFacility(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool isNullable;
            Facility model = new Facility();

            foreach (Facility.Fields field in Facility.Fields.GetValues(typeof(Facility.Fields)))
            {
                string strFieldName = Facility.GetFieldName(field, out isNullable);

                if (field == Facility.Fields.ID)
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
                else if (field == Facility.Fields.FacilityName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.FacilityName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.FacilityName = data;
                }
                else if (field == Facility.Fields.SiteName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.SiteName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.SiteName = data;
                }
                else if (field == Facility.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.SiteID = data.Data;
                }
                else if (field == Facility.Fields.DisplayName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.DisplayName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DisplayName = data;
                }

                index++;
            }

            return model;
        }
    }
}
