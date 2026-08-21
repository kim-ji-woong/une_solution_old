using System.Collections.Generic;
using dnsDBUtil;

namespace NipaSOP.DAL
{
    using IDAL;
    using NipaSOP.Model.Sop;

    public class UpdateManager : QueryManager, IUpdate
    {
        private DataManager m_dataManager = null;
        
        public UpdateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public bool UpdateFacility(Facility facility, out string strErrorMessage)
        {
            Dictionary<Facility.Fields, object> dicSets = new Dictionary<Facility.Fields, object>();
            dicSets[Facility.Fields.FacilityName] = facility.FacilityName;
            dicSets[Facility.Fields.SiteName] = facility.SiteName;
            dicSets[Facility.Fields.DisplayName] = facility.DisplayName;
            dicSets[Facility.Fields.SiteID] = facility.SiteID;

            Dictionary<Facility.Fields, object> dicConditions = new Dictionary<Facility.Fields, object>();
            dicConditions[Facility.Fields.ID] = facility.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Facility.Fields>(ref strSets, dicSets, Facility.GetFieldName, Facility.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Facility.Fields>(ref strCondition, dicConditions, Facility.GetFieldName, Facility.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Facility.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFacility(Dictionary<Facility.Fields, object> dicSets, Dictionary<Facility.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Facility.Fields>(ref strSets, dicSets, Facility.GetFieldName, Facility.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Facility.Fields>(ref strCondition, dicConditions, Facility.GetFieldName, Facility.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Facility.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateLocationLinkedSOP(LocationLinkedSOP sop, out string strErrorMessage)
        {
            Dictionary<LocationLinkedSOP.Fields, object> dicSets = new Dictionary<LocationLinkedSOP.Fields, object>();
            dicSets[LocationLinkedSOP.Fields.DisasterCategoryID] = sop.DisasterCategoryID;
            dicSets[LocationLinkedSOP.Fields.FacilityTypeID] = sop.FacilityTypeID;
            dicSets[LocationLinkedSOP.Fields.SubDisasterCategoryID] = sop.SubDisasterCategoryID;

            Dictionary<LocationLinkedSOP.Fields, object> dicConditions = new Dictionary<LocationLinkedSOP.Fields, object>();
            dicConditions[LocationLinkedSOP.Fields.FacilityID] = sop.FacilityID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<LocationLinkedSOP.Fields>(ref strSets, dicSets, LocationLinkedSOP.GetFieldName, LocationLinkedSOP.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<LocationLinkedSOP.Fields>(ref strCondition, dicConditions, LocationLinkedSOP.GetFieldName, LocationLinkedSOP.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", LocationLinkedSOP.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateLocationLinkedSOP(Dictionary<LocationLinkedSOP.Fields, object> dicSets, Dictionary<LocationLinkedSOP.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<LocationLinkedSOP.Fields>(ref strSets, dicSets, LocationLinkedSOP.GetFieldName, LocationLinkedSOP.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<LocationLinkedSOP.Fields>(ref strCondition, dicConditions, LocationLinkedSOP.GetFieldName, LocationLinkedSOP.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", LocationLinkedSOP.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateStartInfo(StartInfo startInfo, out string strErrorMessage)
        {
            Dictionary<StartInfo.Fields, object> dicSets = new Dictionary<StartInfo.Fields, object>();
            dicSets[StartInfo.Fields.AccessMode] = startInfo.AccessMode;
            dicSets[StartInfo.Fields.AccessToken] = startInfo.AccessToken;
            dicSets[StartInfo.Fields.FacilityID] = startInfo.FacilityID;
            dicSets[StartInfo.Fields.ServiceType] = startInfo.ServiceType;
            dicSets[StartInfo.Fields.TimeStamp] = startInfo.TimeStamp;

            Dictionary<StartInfo.Fields, object> dicConditions = new Dictionary<StartInfo.Fields, object>();
            dicConditions[StartInfo.Fields.ID] = startInfo.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<StartInfo.Fields>(ref strSets, dicSets, StartInfo.GetFieldName, StartInfo.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<StartInfo.Fields>(ref strCondition, dicConditions, StartInfo.GetFieldName, StartInfo.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", StartInfo.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateStartInfo(Dictionary<StartInfo.Fields, object> dicSets, Dictionary<StartInfo.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<StartInfo.Fields>(ref strSets, dicSets, StartInfo.GetFieldName, StartInfo.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<StartInfo.Fields>(ref strCondition, dicConditions, StartInfo.GetFieldName, StartInfo.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", StartInfo.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }
    }
}
