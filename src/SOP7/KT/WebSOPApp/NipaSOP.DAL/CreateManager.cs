using System;
using System.Collections.Generic;
using System.Collections;
using dnsDBUtil;

namespace NipaSOP.DAL
{
    using IDAL;
    using NipaSOP.Model.Sop;

    public class CreateManager : QueryManager, ICreate
    {
        private string m_strErrorMessage = null;
        private DataManager m_dataManager = null;

        public CreateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public string GetErrorMessage()
        {
            return m_strErrorMessage;
        }

        public LocationLinkedSOP CreateLocationLinkedSOP(int nFacilityID, int nFacilityTypeID, int nDisasterCategoryID, int nSubDisasterCategoryID, string strDisasterName)
        {
            Dictionary<LocationLinkedSOP.Fields, object> dicFieldDatas = new Dictionary<LocationLinkedSOP.Fields, object>();

            dicFieldDatas[LocationLinkedSOP.Fields.FacilityID] = nFacilityID;
            dicFieldDatas[LocationLinkedSOP.Fields.FacilityTypeID] = nFacilityTypeID;
            dicFieldDatas[LocationLinkedSOP.Fields.DisasterCategoryID] = nDisasterCategoryID;
            dicFieldDatas[LocationLinkedSOP.Fields.SubDisasterCategoryID] = nSubDisasterCategoryID;
            dicFieldDatas[LocationLinkedSOP.Fields.DisasterName] = strDisasterName;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                LocationLinkedSOP.TableName,
                GetFieldNames<LocationLinkedSOP.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                string strErrorMessage;
                List<LocationLinkedSOP> sops = m_dataManager.GetSelectManager().SelectLocationLinkedSOPs(dicFieldDatas, null, out strErrorMessage);

                if (sops == null || sops.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                return sops[0];
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public StartInfo CreateStartInfo(DateTime dtTimeStamp, string strAccessMode, string strAccessToken, string strServiceType, int nFacilityID, bool randomID = false)
        {
            Dictionary<StartInfo.Fields, object> dicFieldDatas = new Dictionary<StartInfo.Fields, object>();
            dicFieldDatas[StartInfo.Fields.TimeStamp] = dtTimeStamp;
            dicFieldDatas[StartInfo.Fields.AccessMode] = strAccessMode;
            dicFieldDatas[StartInfo.Fields.AccessToken] = strAccessToken;
            dicFieldDatas[StartInfo.Fields.ServiceType] = strServiceType;
            dicFieldDatas[StartInfo.Fields.FacilityID] = nFacilityID;

            string strSQL = "";
            int id = 0;
            bool find = false;

            if (randomID)
            {
                for (int j = 0; j < 100; j++)
                {
                    Guid guid = System.Guid.NewGuid();
                    byte[] bytes = guid.ToByteArray();
                    int nBytesCount = bytes.Length;

                    for (int i = 0; i < nBytesCount - 4; i++)
                    {
                        id = System.BitConverter.ToInt32(bytes, i);

                        if (id < 0)
                            id = -id;

                        string strErrorMessage;
                        StartInfo info = m_dataManager.GetSelectManager().SelectStartInfo(id, out strErrorMessage);

                        if (info == null)
                        {
                            find = true;
                            break;
                        }
                    }

                    if (find)
                        break;
                }

                if (!find)
                    randomID = false;
            }

            if (randomID)
            {
                dicFieldDatas[StartInfo.Fields.ID] = id;

                strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                    StartInfo.TableName,
                    GetFieldNames<StartInfo.Fields>(),
                    GetFieldValues(dicFieldDatas));

                ArrayList arrResult = m_dbManager.GetResultData(strSQL);

                if (arrResult != null)
                {
                    string strErrorMessage;
                    StartInfo info = m_dataManager.GetSelectManager().SelectStartInfo(id, out strErrorMessage);

                    if (info == null)
                        m_strErrorMessage = strErrorMessage;

                    return info;
                }
                else
                {
                    m_strErrorMessage = m_dbManager.LastErrorMessage;
                }
            }
            else
            {
                strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                    StartInfo.TableName,
                    GetFieldNames<StartInfo.Fields>(),
                    GetFieldValues(dicFieldDatas));

                ArrayList arrResult = m_dbManager.GetResultData(strSQL);

                if (arrResult != null)
                {
                    string strErrorMessage;
                    List<StartInfo> infos = m_dataManager.GetSelectManager().SelectStartInfos(dicFieldDatas, null, out strErrorMessage);

                    if (infos == null || infos.Count == 0)
                    {
                        m_strErrorMessage = strErrorMessage;
                        return null;
                    }

                    return infos[0];
                }
                else
                {
                    m_strErrorMessage = m_dbManager.LastErrorMessage;
                }
            }

            return null;
        }

        public Facility CreateFacility(int id, string strFacilityName, string strSiteName, string strDisplayName, int nSiteID)
        {
            Dictionary<Facility.Fields, object> dicFieldDatas = new Dictionary<Facility.Fields, object>();

            dicFieldDatas[Facility.Fields.ID] = id;
            dicFieldDatas[Facility.Fields.FacilityName] = strFacilityName;
            dicFieldDatas[Facility.Fields.SiteName] = strSiteName;
            dicFieldDatas[Facility.Fields.DisplayName] = strDisplayName;
            dicFieldDatas[Facility.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                Facility.TableName,
                GetFieldNames<Facility.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                string strErrorMessage;
                List<Facility> facilities = m_dataManager.GetSelectManager().SelectFacilities(dicFieldDatas, null, out strErrorMessage);

                if (facilities == null || facilities.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                return facilities[0];
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }
    }
}
