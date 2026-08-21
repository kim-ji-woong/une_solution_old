using System;
using System.Collections.Generic;
using System.Collections;
using dnsDBUtil;

namespace SDMS.DAL
{
    using IDAL;
    using SDMS.Model.Alarm;
    using SDMS.Model.History;
    using SDMS.Model.Sensor;
    using SDMS.Model.Spatial;
    using SDMS.Model.CCTV;
    using SDMS.Model.Facility;
    using UnE.Geometry;

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

        public Building CreateBuilding(string strBuildingCode, string strBuildingName, int nBuildingGroupID, int nMaxFloor, int nMinFloor, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText)
        {
            Dictionary<Building.Fields, object> dicFieldDatas = new Dictionary<Building.Fields, object>();
            dicFieldDatas[Building.Fields.BuildingCode] = strBuildingCode;
            dicFieldDatas[Building.Fields.BuildingName] = strBuildingName;
            dicFieldDatas[Building.Fields.BuildingGroupID] = nBuildingGroupID;
            dicFieldDatas[Building.Fields.MaxFloor] = nMaxFloor;
            dicFieldDatas[Building.Fields.MinFloor] = nMinFloor;
            dicFieldDatas[Building.Fields.TextCenter] = VertexToString(vTextCenter);
            dicFieldDatas[Building.Fields.BroadcastText] = strBroadcastText;
            dicFieldDatas[Building.Fields.DisplayText] = strDisplayText;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Building.TableName,
                GetFieldNames<Building.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Building.GetFieldName(Building.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Building> datas = m_dataManager.GetSelectManager().SelectBuildings(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameBuilding(datas[0], strBuildingCode, strBuildingName, nBuildingGroupID, nMaxFloor, nMinFloor, vTextCenter, strBroadcastText, strDisplayText))
                    return datas[0];

                return GetBuilding(strBuildingCode, strBuildingName, nBuildingGroupID, nMaxFloor, nMinFloor, vTextCenter, strBroadcastText, strDisplayText, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Building GetBuilding(string strBuildingCode, string strBuildingName, int nBuildingGroupID, int nMaxFloor, int nMinFloor, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Building.GetFieldName(Building.Fields.ID, out isNullable), id);

            List<Building> datas = m_dataManager.GetSelectManager().SelectBuildings(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Building data in datas)
            {
                if (IsSameBuilding(data, strBuildingCode, strBuildingName, nBuildingGroupID, nMaxFloor, nMinFloor, vTextCenter, strBroadcastText, strDisplayText))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetBuilding(strBuildingCode, strBuildingName, nBuildingGroupID, nMaxFloor, nMinFloor, vTextCenter, strBroadcastText, strDisplayText, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Building.TableName);
            return null;
        }

        private bool IsSameBuilding(Building building, string strBuildingCode, string strBuildingName, int nBuildingGroupID, int nMaxFloor, int nMinFloor, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText)
        {
            if (building.BuildingCode == strBuildingCode &&
                building.BuildingName == strBuildingName &&
                building.BuildingGroupID == nBuildingGroupID &&
                building.MaxFloor == nMaxFloor &&
                building.MinFloor == nMinFloor &&
                IsSameVertex3D(building.TextCenter, vTextCenter) &&
                building.BroadcastText == strBroadcastText &&
                building.DisplayText == strDisplayText)
                return true;

            return false;
        }

        public BuildingGroup CreateBuildingGroup(string strGroupName, int? nParentID, Vertex3D vTextCenter, string strDisplayText, int nSiteID)
        {
            Dictionary<BuildingGroup.Fields, object> dicFieldDatas = new Dictionary<BuildingGroup.Fields, object>();
            dicFieldDatas[BuildingGroup.Fields.GroupName] = strGroupName;
            dicFieldDatas[BuildingGroup.Fields.ParentID] = nParentID;
            dicFieldDatas[BuildingGroup.Fields.TextCenter] = VertexToString(vTextCenter);
            dicFieldDatas[BuildingGroup.Fields.DisplayText] = strDisplayText;
            dicFieldDatas[BuildingGroup.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                BuildingGroup.TableName,
                GetFieldNames<BuildingGroup.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", BuildingGroup.GetFieldName(BuildingGroup.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<BuildingGroup> datas = m_dataManager.GetSelectManager().SelectBuildingGroups(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameBuildingGroup(datas[0], strGroupName, nParentID, vTextCenter, strDisplayText, nSiteID))
                    return datas[0];

                return GetBuildingGroup(strGroupName, nParentID, vTextCenter, strDisplayText, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private BuildingGroup GetBuildingGroup(string strGroupName, int? nParentID, Vertex3D vTextCenter, string strDisplayText, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", BuildingGroup.GetFieldName(BuildingGroup.Fields.ID, out isNullable), id);

            List<BuildingGroup> datas = m_dataManager.GetSelectManager().SelectBuildingGroups(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (BuildingGroup data in datas)
            {
                if (IsSameBuildingGroup(data, strGroupName, nParentID,vTextCenter, strDisplayText, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetBuildingGroup(strGroupName, nParentID, vTextCenter, strDisplayText, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(BuildingGroup.TableName);
            return null;
        }

        private bool IsSameBuildingGroup(BuildingGroup buildingGroup, string strGroupName, int? nParentID, Vertex3D vTextCenter, string strDisplayText, int nSiteID)
        {
            if (buildingGroup.GroupName == strGroupName &&
                buildingGroup.ParentID == nParentID &&
                IsSameVertex3D(buildingGroup.TextCenter, vTextCenter) &&
                buildingGroup.DisplayText == strDisplayText &&
                buildingGroup.SiteID == nSiteID)
                return true;

            return false;
        }

        public EquipmentZone CreateEquipmentZone(string strZoneName, Polygon boundary, List<int> linkedZoneIDList, int? nType, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText, int nSiteID)
        {
            Dictionary<EquipmentZone.Fields, object> dicFieldDatas = new Dictionary<EquipmentZone.Fields, object>();
            dicFieldDatas[EquipmentZone.Fields.ZoneName] = strZoneName;
            dicFieldDatas[EquipmentZone.Fields.Boundary] = PolygonToString(boundary);
            dicFieldDatas[EquipmentZone.Fields.LinkedZoneIDList] = ListToString(linkedZoneIDList);
            dicFieldDatas[EquipmentZone.Fields.Type] = nType;
            dicFieldDatas[EquipmentZone.Fields.TextCenter] = VertexToString(vTextCenter);
            dicFieldDatas[EquipmentZone.Fields.BroadcastText] = strBroadcastText;
            dicFieldDatas[EquipmentZone.Fields.DisplayText] = strDisplayText;
            dicFieldDatas[EquipmentZone.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                EquipmentZone.TableName,
                GetFieldNames<EquipmentZone.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", EquipmentZone.GetFieldName(EquipmentZone.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<EquipmentZone> datas = m_dataManager.GetSelectManager().SelectEquipmentZones(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameEquipmentZone(datas[0], strZoneName, boundary, linkedZoneIDList, nType, vTextCenter, strBroadcastText, strDisplayText, nSiteID))
                    return datas[0];

                return GetEquipmentZone(strZoneName, boundary, linkedZoneIDList, nType, vTextCenter, strBroadcastText, strDisplayText, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private EquipmentZone GetEquipmentZone(string strZoneName, Polygon boundary, List<int> linkedZoneIDList, int? nType, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", EquipmentZone.GetFieldName(EquipmentZone.Fields.ID, out isNullable), id);

            List<EquipmentZone> datas = m_dataManager.GetSelectManager().SelectEquipmentZones(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (EquipmentZone data in datas)
            {
                if (IsSameEquipmentZone(data, strZoneName, boundary, linkedZoneIDList, nType, vTextCenter, strBroadcastText, strDisplayText, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetEquipmentZone(strZoneName, boundary, linkedZoneIDList, nType, vTextCenter, strBroadcastText, strDisplayText, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(EquipmentZone.TableName);
            return null;
        }

        private bool IsSameEquipmentZone(EquipmentZone equipZone, string strZoneName, Polygon boundary, List<int> linkedZoneIDList, int? nType, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText, int nSiteID)
        {
            if (equipZone.ZoneName == strZoneName &&
                IsSamePolygon(equipZone.Boundary, boundary) &&
                IsSameList<int>(equipZone.LinkedZoneIDs, linkedZoneIDList) &&
                equipZone.Type == nType &&
                IsSameVertex3D(equipZone.TextCenter, vTextCenter) &&
                equipZone.BroadcastText == strBroadcastText &&
                equipZone.DisplayText == strDisplayText &&
                equipZone.SiteID == nSiteID)
                return true;

            return false;
        }

        public FacilityType CreateFacilityType(string strTypeName, string strLinkedTableName, int nSiteID, string strDescription, int? nDisasterCategoryID, int? nSubDisasterCategoryID)
        {
            Dictionary<FacilityType.Fields, object> dicFieldDatas = new Dictionary<FacilityType.Fields, object>();
            dicFieldDatas[FacilityType.Fields.TypeName] = strTypeName;
            dicFieldDatas[FacilityType.Fields.LinkedTableName] = strLinkedTableName;
            dicFieldDatas[FacilityType.Fields.SiteID] = nSiteID;
            dicFieldDatas[FacilityType.Fields.Description] = strDescription;
            dicFieldDatas[FacilityType.Fields.DisasterCategoryID] = nDisasterCategoryID;
            dicFieldDatas[FacilityType.Fields.SubDisasterCategoryID] = nSubDisasterCategoryID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                FacilityType.TableName,
                GetFieldNames<FacilityType.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", FacilityType.GetFieldName(FacilityType.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<FacilityType> datas = m_dataManager.GetSelectManager().SelectFacilityTypes(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameFacilityType(datas[0], strTypeName, strLinkedTableName, nSiteID, strDescription, nDisasterCategoryID, nSubDisasterCategoryID))
                    return datas[0];

                return GetFacilityType(strTypeName, strLinkedTableName, nSiteID, strDescription, nDisasterCategoryID, nSubDisasterCategoryID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private FacilityType GetFacilityType(string strTypeName, string strLinkedTableName, int nSiteID, string strDescription, int? nDisasterCategoryID, int? nSubDisasterCategoryID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", FacilityType.GetFieldName(FacilityType.Fields.ID, out isNullable), id);

            List<FacilityType> datas = m_dataManager.GetSelectManager().SelectFacilityTypes(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (FacilityType data in datas)
            {
                if (IsSameFacilityType(data, strTypeName, strLinkedTableName, nSiteID, strDescription, nDisasterCategoryID, nSubDisasterCategoryID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetFacilityType(strTypeName, strLinkedTableName, nSiteID, strDescription, nDisasterCategoryID, nSubDisasterCategoryID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(FacilityType.TableName);
            return null;
        }

        private bool IsSameFacilityType(FacilityType data, string strTypeName, string strLinkedTableName, int nSiteID, string strDescription, int? nDisasterCategoryID, int? nSubDisasterCategoryID)
        {
            if (data.TypeName == strTypeName &&
                data.LinkedTableName == strLinkedTableName &&
                data.SiteID == nSiteID &&
                data.Description == strDescription &&
                data.DisasterCategoryID == nDisasterCategoryID &&
                data.SubDisasterCategoryID == nSubDisasterCategoryID)
                return true;

            return false;
        }

        public Fire CreateFireSensor(string strName, string strPositionName, float? x, float? y, float? z, int nZoneID, string strDepartment, string strDepartmentPhoneNumber)
        {
            Dictionary<Fire.Fields, object> dicFieldDatas = new Dictionary<Fire.Fields, object>();
            dicFieldDatas[Fire.Fields.Name] = strName;
            dicFieldDatas[Fire.Fields.PositionName] = strPositionName;
            dicFieldDatas[Fire.Fields.X] = x;
            dicFieldDatas[Fire.Fields.Y] = y;
            dicFieldDatas[Fire.Fields.Z] = z;
            dicFieldDatas[Fire.Fields.ZoneID] = nZoneID;
            dicFieldDatas[Fire.Fields.Department] = strDepartment;
            dicFieldDatas[Fire.Fields.DepartmentPhoneNumber] = strDepartmentPhoneNumber;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Fire.TableName,
                GetFieldNames<Fire.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Fire.GetFieldName(Fire.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Fire> datas = m_dataManager.GetSelectManager().SelectFireSensors(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameFire(datas[0], strName, strPositionName, x, y, z, nZoneID, strDepartment, strDepartmentPhoneNumber))
                    return datas[0];

                return GetFire(strName, strPositionName, x, y, z, nZoneID, strDepartment, strDepartmentPhoneNumber, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Fire GetFire(string strName, string strPositionName, float? x, float? y, float? z, int nZoneID, string strDepartment, string strDepartmentPhoneNumber, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Fire.GetFieldName(Fire.Fields.ID, out isNullable), id);

            List<Fire> datas = m_dataManager.GetSelectManager().SelectFireSensors(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Fire data in datas)
            {
                if (IsSameFire(data, strName, strPositionName, x, y, z, nZoneID, strDepartment, strDepartmentPhoneNumber))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetFire(strName, strPositionName, x, y, z, nZoneID, strDepartment, strDepartmentPhoneNumber, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Fire.TableName);
            return null;
        }

        private bool IsSameFire(Fire data, string strName, string strPositionName, float? x, float? y, float? z, int nZoneID, string strDepartment, string strDepartmentPhoneNumber)
        {
            if (data.Name == strName &&
                data.PositionName == strPositionName &&
                IsSameFloatData(data.X, x) &&
                IsSameFloatData(data.Y, y) &&
                IsSameFloatData(data.Z, z) &&
                data.ZoneID == nZoneID &&
                data.Department == strDepartment &&
                data.DepartmentPhoneNumber == strDepartmentPhoneNumber)
                return true;

            return false;
        }

        public PSM CreatePSMSensor(string strName, string strPositionName, float? x, float? y, float? z, float? fCurrentData, float? fLimitLevel1, float? fLimitLevel2, float? fLimitLevel3, int nZoneID, int nEquipZoneID, bool useLimitLevel1, bool useLimitLevel2, bool useLimitLevel3, string strDepartment, string strDepartmentPhoneNumber, string strStatus, string strUniqueKey, int? materialType)
        {
            Dictionary<PSM.Fields, object> dicFieldDatas = new Dictionary<PSM.Fields, object>();
            dicFieldDatas[PSM.Fields.Name] = strName;
            dicFieldDatas[PSM.Fields.PositionName] = strPositionName;
            dicFieldDatas[PSM.Fields.X] = x;
            dicFieldDatas[PSM.Fields.Y] = y;
            dicFieldDatas[PSM.Fields.Z] = z;            
            dicFieldDatas[PSM.Fields.CurrentData] = fCurrentData;
            dicFieldDatas[PSM.Fields.LimitLevel1] = fLimitLevel1;
            dicFieldDatas[PSM.Fields.LimitLevel2] = fLimitLevel2;
            dicFieldDatas[PSM.Fields.LimitLevel3] = fLimitLevel3;
            dicFieldDatas[PSM.Fields.ZoneID] = nZoneID;
            dicFieldDatas[PSM.Fields.EquipZoneID] = nEquipZoneID;
            dicFieldDatas[PSM.Fields.UseLimitLevel1] = useLimitLevel1;
            dicFieldDatas[PSM.Fields.UseLimitLevel2] = useLimitLevel2;
            dicFieldDatas[PSM.Fields.UseLimitLevel3] = useLimitLevel3;
            dicFieldDatas[PSM.Fields.Department] = strDepartment;
            dicFieldDatas[PSM.Fields.DepartmentPhoneNumber] = strDepartmentPhoneNumber;
            dicFieldDatas[PSM.Fields.Status] = strStatus;
            dicFieldDatas[PSM.Fields.UniqueKey] = strUniqueKey;
            dicFieldDatas[PSM.Fields.MaterialType] = materialType;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                PSM.TableName,
                GetFieldNames<PSM.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", PSM.GetFieldName(PSM.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<PSM> datas = m_dataManager.GetSelectManager().SelectPSMSensors(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSamePSM(datas[0], strName, strPositionName, x, y, z, fCurrentData, fLimitLevel1, fLimitLevel2, fLimitLevel3, nZoneID, nEquipZoneID, useLimitLevel1, useLimitLevel2, useLimitLevel3, strDepartment, strDepartmentPhoneNumber, strStatus, strUniqueKey, materialType))
                    return datas[0];

                return GetPSM(strName, strPositionName, x, y, z, fCurrentData, fLimitLevel1, fLimitLevel2, fLimitLevel3, nZoneID, nEquipZoneID, useLimitLevel1, useLimitLevel2, useLimitLevel3, strDepartment, strDepartmentPhoneNumber, strStatus, strUniqueKey, materialType, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private PSM GetPSM(string strName, string strPositionName, float? x, float? y, float? z, float? fCurrentData, float? fLimitLevel1, float? fLimitLevel2, float? fLimitLevel3, int nZoneID, int nEquipZoneID, bool useLimitLevel1, bool useLimitLevel2, bool useLimitLevel3, string strDepartment, string strDepartmentPhoneNumber, string strStatus, string strUniqueKey, int? materialType, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", PSM.GetFieldName(PSM.Fields.ID, out isNullable), id);

            List<PSM> datas = m_dataManager.GetSelectManager().SelectPSMSensors(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (PSM data in datas)
            {
                if (IsSamePSM(data, strName, strPositionName, x, y, z, fCurrentData, fLimitLevel1, fLimitLevel2, fLimitLevel3, nZoneID, nEquipZoneID, useLimitLevel1, useLimitLevel2, useLimitLevel3, strDepartment, strDepartmentPhoneNumber, strStatus, strUniqueKey, materialType))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetPSM(strName, strPositionName, x, y, z, fCurrentData, fLimitLevel1, fLimitLevel2, fLimitLevel3, nZoneID, nEquipZoneID, useLimitLevel1, useLimitLevel2, useLimitLevel3, strDepartment, strDepartmentPhoneNumber, strStatus, strUniqueKey, materialType, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(PSM.TableName);
            return null;
        }

        private bool IsSamePSM(PSM data, string strName, string strPositionName, float? x, float? y, float? z, float? fCurrentData, float? fLimitLevel1, float? fLimitLevel2, float? fLimitLevel3, int nZoneID, int nEquipZoneID, bool useLimitLevel1, bool useLimitLevel2, bool useLimitLevel3, string strDepartment, string strDepartmentPhoneNumber, string strStatus, string strUniqueKey, int? materialType)
        {
            if (data.Name == strName &&
                data.PositionName == strPositionName &&
                IsSameFloatData(data.X, x) &&
                IsSameFloatData(data.Y, y) &&
                IsSameFloatData(data.Z, z) &&
                IsSameFloatData(data.CurrentData, fCurrentData) &&
                IsSameFloatData(data.LimitLevel1, fLimitLevel1) &&
                IsSameFloatData(data.LimitLevel2, fLimitLevel2) &&
                IsSameFloatData(data.LimitLevel3, fLimitLevel3) &&
                data.ZoneID == nZoneID &&
                data.EquipZoneID == nEquipZoneID &&
                data.UseLimitLevel1 == useLimitLevel1 &&
                data.UseLimitLevel2 == useLimitLevel2 &&
                data.UseLimitLevel3 == useLimitLevel3 &&
                data.Department == strDepartment &&
                data.DepartmentPhoneNumber == strDepartmentPhoneNumber &&
                data.Status == strStatus &&
                data.UniqueKey == strUniqueKey &&
                data.MaterialType == materialType)
                return true;

            return false;
        }

        public Material CreateMaterial(string strMaterialName, string strUOM, int nSiteID, string strDescription)
        {
            Dictionary<Material.Fields, object> dicFieldDatas = new Dictionary<Material.Fields, object>();
            dicFieldDatas[Material.Fields.MaterialName] = strMaterialName;
            dicFieldDatas[Material.Fields.UOM] = strUOM;
            dicFieldDatas[Material.Fields.SiteID] = nSiteID;
            dicFieldDatas[Material.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Material.TableName,
                GetFieldNames<Material.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Material.GetFieldName(Material.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Material> datas = m_dataManager.GetSelectManager().SelectMaterials(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameMaterial(datas[0], strMaterialName, strUOM, nSiteID, strDescription))
                    return datas[0];

                return GetMaterial(strMaterialName, strUOM, nSiteID, strDescription, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Material GetMaterial(string strMaterialName, string strUOM, int nSiteID, string strDescription, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Material.GetFieldName(Material.Fields.ID, out isNullable), id);

            List<Material> datas = m_dataManager.GetSelectManager().SelectMaterials(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Material data in datas)
            {
                if (IsSameMaterial(data, strMaterialName, strUOM, nSiteID, strDescription))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetMaterial(strMaterialName, strUOM, nSiteID, strDescription, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Material.TableName);
            return null;
        }

        private bool IsSameMaterial(Material data, string strMaterialName, string strUOM, int nSiteID, string strDescription)
        {
            if (data.MaterialName == strMaterialName &&
                data.UOM == strUOM &&
                data.SiteID == nSiteID &&
                data.Description == strDescription)
                return true;

            return false;
        }

        public SensorReactionHistory CreateSensorReactionHistory(int nSensorZoneHistoryID, int nReactionType, DateTime time, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5)
        {
            Dictionary<SensorReactionHistory.Fields, object> dicFieldDatas = new Dictionary<SensorReactionHistory.Fields, object>();
            dicFieldDatas[SensorReactionHistory.Fields.SensorZoneHistoryID] = nSensorZoneHistoryID;
            dicFieldDatas[SensorReactionHistory.Fields.ReactionType] = nReactionType;
            dicFieldDatas[SensorReactionHistory.Fields.Time] = time;
            dicFieldDatas[SensorReactionHistory.Fields.Message] = strMessage;
            dicFieldDatas[SensorReactionHistory.Fields.Param1] = strParam1;
            dicFieldDatas[SensorReactionHistory.Fields.Param2] = strParam2;
            dicFieldDatas[SensorReactionHistory.Fields.Param3] = strParam3;
            dicFieldDatas[SensorReactionHistory.Fields.Param4] = strParam4;
            dicFieldDatas[SensorReactionHistory.Fields.Param5] = strParam5;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                SensorReactionHistory.TableName,
                GetFieldNames<SensorReactionHistory.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<SensorReactionHistory> datas = m_dataManager.GetSelectManager().SelectSensorReactionHistories(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSensorReactionHistory(datas[0], nSensorZoneHistoryID, nReactionType, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5))
                    return datas[0];

                return GetSensorReactionHistory(nSensorZoneHistoryID, nReactionType, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private SensorReactionHistory GetSensorReactionHistory(int nSensorZoneHistoryID, int nReactionType, DateTime time, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.ID, out isNullable), id);

            List<SensorReactionHistory> datas = m_dataManager.GetSelectManager().SelectSensorReactionHistories(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (SensorReactionHistory data in datas)
            {
                if (IsSameSensorReactionHistory(data, nSensorZoneHistoryID, nReactionType, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSensorReactionHistory(nSensorZoneHistoryID, nReactionType, time, strMessage, strParam1, strParam2, strParam3, strParam4, strParam5, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(SensorReactionHistory.TableName);
            return null;
        }

        private bool IsSameSensorReactionHistory(SensorReactionHistory data, int nSensorZoneHistoryID, int nReactionType, DateTime time, string strMessage, string strParam1, string strParam2, string strParam3, string strParam4, string strParam5)
        {
            if (data.SensorZoneHistoryID == nSensorZoneHistoryID &&
                data.ReactionType == SensorReactionHistory.ToReactionType(nReactionType) &&
                data.Time.ToString("yyyyMMddHHmmss") == time.ToString("yyyyMMddHHmmss") &&
                data.Message == strMessage &&
                data.Param1 == strParam1 &&
                data.Param2 == strParam2 &&
                data.Param3 == strParam3 &&
                data.Param4 == strParam4 &&
                data.Param5 == strParam5)
                return true;

            return false;
        }

        public SensorZone CreateSensorZone(int nType, int nOrgSensorID, int nEquipZoneID, bool isAlarmStatus, int? data)
        {
            Dictionary<SensorZone.Fields, object> dicFieldDatas = new Dictionary<SensorZone.Fields, object>();
            dicFieldDatas[SensorZone.Fields.SensorType] = nType;
            dicFieldDatas[SensorZone.Fields.OrgSensorID] = nOrgSensorID;
            dicFieldDatas[SensorZone.Fields.EquipZoneID] = nEquipZoneID;
            dicFieldDatas[SensorZone.Fields.IsAlarmStatus] = isAlarmStatus;
            dicFieldDatas[SensorZone.Fields.Data] = data;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                SensorZone.TableName,
                GetFieldNames<SensorZone.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", SensorZone.GetFieldName(SensorZone.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<SensorZone> datas = m_dataManager.GetSelectManager().SelectSensorZones(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSensorZone(datas[0], nType, nOrgSensorID, nEquipZoneID, isAlarmStatus, data))
                    return datas[0];

                return GetSensorZone(nType, nOrgSensorID, nEquipZoneID, isAlarmStatus, data, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private SensorZone GetSensorZone(int nType, int nOrgSensorID, int nEquipZoneID, bool isAlarmStatus, int? data, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", SensorZone.GetFieldName(SensorZone.Fields.ID, out isNullable), id);

            List<SensorZone> datas = m_dataManager.GetSelectManager().SelectSensorZones(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (SensorZone sensorZone in datas)
            {
                if (IsSameSensorZone(sensorZone, nType, nOrgSensorID, nEquipZoneID, isAlarmStatus, data))
                    return sensorZone;

                if (sensorZone.ID < id)
                    id = sensorZone.ID;
            }

            if (nCount < nLimit)
                return GetSensorZone(nType, nOrgSensorID, nEquipZoneID, isAlarmStatus, data, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(SensorZone.TableName);
            return null;
        }

        private bool IsSameSensorZone(SensorZone sensorZone, int nType, int nOrgSensorID, int nEquipZoneID, bool isAlarmStatus, int? data)
        {
            if (sensorZone.SensorType == nType &&
                sensorZone.OrgSensorID == nOrgSensorID &&
                sensorZone.EquipZoneID == nEquipZoneID &&
                sensorZone.IsAlarmStatus == isAlarmStatus &&
                sensorZone.Data == data)
                return true;

            return false;
        }

        public SensorZoneHistory CreateSensorZoneHistory(int nSensorZoneID, string strData, DateTime time, int nZoneID, int nSensorType, int? nDetectionStatus, int nSiteID, string strMemo, List<int> allSensorZoneIDs = null)
        {
            Dictionary<SensorZoneHistory.Fields, object> dicFieldDatas = new Dictionary<SensorZoneHistory.Fields, object>();
            dicFieldDatas[SensorZoneHistory.Fields.SensorZoneID] = nSensorZoneID;
            dicFieldDatas[SensorZoneHistory.Fields.Data] = strData;
            dicFieldDatas[SensorZoneHistory.Fields.Time] = time;
            dicFieldDatas[SensorZoneHistory.Fields.ZoneID] = nZoneID;
            dicFieldDatas[SensorZoneHistory.Fields.SensorType] = nSensorType;
            dicFieldDatas[SensorZoneHistory.Fields.DetectionStatus] = nDetectionStatus;
            dicFieldDatas[SensorZoneHistory.Fields.SiteID] = nSiteID;
            dicFieldDatas[SensorZoneHistory.Fields.AllSensorZoneIDs] = allSensorZoneIDs == null ? null : ListToString<int>(allSensorZoneIDs);
            dicFieldDatas[SensorZoneHistory.Fields.Memo] = strMemo;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                SensorZoneHistory.TableName,
                GetFieldNames<SensorZoneHistory.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<SensorZoneHistory> datas = m_dataManager.GetSelectManager().SelectSensorZoneHistories(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSensorZoneHistory(datas[0], nSensorZoneID, strData, time, nZoneID, nSensorType, nDetectionStatus, nSiteID, allSensorZoneIDs, strMemo))
                    return datas[0];

                return GetSensorZoneHistory(nSensorZoneID, strData, time, nZoneID, nSensorType, nDetectionStatus, nSiteID, allSensorZoneIDs, strMemo, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private SensorZoneHistory GetSensorZoneHistory(int nSensorZoneID, string strData, DateTime time, int nZoneID, int nSensorType, int? nDetectionStatus, int nSiteID, List<int> allSensorZoneIDs, string strMemo, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.ID, out isNullable), id);

            List<SensorZoneHistory> datas = m_dataManager.GetSelectManager().SelectSensorZoneHistories(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (SensorZoneHistory data in datas)
            {
                if (IsSameSensorZoneHistory(data, nSensorZoneID, strData, time, nZoneID, nSensorType, nDetectionStatus, nSiteID, allSensorZoneIDs, strMemo))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSensorZoneHistory(nSensorZoneID, strData, time, nZoneID, nSensorType, nDetectionStatus, nSiteID, allSensorZoneIDs, strMemo, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(SensorZoneHistory.TableName);
            return null;
        }

        private bool IsSameSensorZoneHistory(SensorZoneHistory data, int nSensorZoneID, string strData, DateTime time, int nZoneID, int nSensorType, int? nDetectionStatus, int nSiteID, List<int> allSensorZoneIDs, string strMemo)
        {
            if (data.SensorZoneID == nSensorZoneID &&
                data.Data == strData &&
                data.Time.ToString("yyyyMMddHHmmss") == time.ToString("yyyyMMddHHmmss") &&
                data.ZoneID == nZoneID &&
                data.SensorType == nSensorType &&
                ((data.DetectionStatus == SensorZoneHistory.DetectionType.None && nDetectionStatus == null) || (nDetectionStatus != null && data.DetectionStatus == SensorZoneHistory.ToDetectionType((int)nDetectionStatus))) &&
                data.SiteID == nSiteID &&
                IsSameList<int>(data.AllSensorZoneIDs, allSensorZoneIDs) && data.Memo == strMemo)
                return true;

            return false;
        }

        public Zone CreateZone(string strZoneName, int? nBuildingID, int? nFloorIndex, float? fAddFloor, Polygon boundary, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText, int nSiteID)
        {
            Dictionary<Zone.Fields, object> dicFieldDatas = new Dictionary<Zone.Fields, object>();
            dicFieldDatas[Zone.Fields.ZoneName] = strZoneName;
            dicFieldDatas[Zone.Fields.BuildingID] = nBuildingID;
            dicFieldDatas[Zone.Fields.FloorIndex] = nFloorIndex;
            dicFieldDatas[Zone.Fields.AddFloor] = fAddFloor;
            dicFieldDatas[Zone.Fields.Boundary] = PolygonToString(boundary);
            dicFieldDatas[Zone.Fields.TextCenter] = VertexToString(vTextCenter);
            dicFieldDatas[Zone.Fields.BroadcastText] = strBroadcastText;
            dicFieldDatas[Zone.Fields.DisplayText] = strDisplayText;
            dicFieldDatas[Zone.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Zone.TableName,
                GetFieldNames<Zone.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Zone.GetFieldName(Zone.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Zone> datas = m_dataManager.GetSelectManager().SelectZones(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameZone(datas[0], strZoneName, nBuildingID, nFloorIndex, fAddFloor, boundary, vTextCenter, strBroadcastText, strDisplayText, nSiteID))
                    return datas[0];

                return GetZone(strZoneName, nBuildingID, nFloorIndex, fAddFloor, boundary, vTextCenter, strBroadcastText, strDisplayText, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Zone GetZone(string strZoneName, int? nBuildingID, int? nFloorIndex, float? fAddFloor, Polygon boundary, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Zone.GetFieldName(Zone.Fields.ID, out isNullable), id);

            List<Zone> datas = m_dataManager.GetSelectManager().SelectZones(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Zone data in datas)
            {
                if (IsSameZone(data, strZoneName, nBuildingID, nFloorIndex, fAddFloor, boundary, vTextCenter, strBroadcastText, strDisplayText, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetZone(strZoneName, nBuildingID, nFloorIndex, fAddFloor, boundary, vTextCenter, strBroadcastText, strDisplayText, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Zone.TableName);
            return null;
        }

        private bool IsSameZone(Zone data, string strZoneName, int? nBuildingID, int? nFloorIndex, float? fAddFloor, Polygon boundary, Vertex3D vTextCenter, string strBroadcastText, string strDisplayText, int nSiteID)
        {
            if (data.ZoneName == strZoneName &&
                data.BuildingID == nBuildingID &&
                data.FloorIndex == nFloorIndex &&
                IsSameFloatData(data.AddFloor, fAddFloor) &&
                IsSamePolygon(data.Boundary, boundary) &&
                IsSameVertex3D(data.TextCenter, vTextCenter) &&
                data.BroadcastText == strBroadcastText &&
                data.DisplayText == strDisplayText &&
                data.SiteID == nSiteID)
                return true;

            return false;
        }

        public SensorReactionHistoryDescription CreateSensorReactionHistoryDescription(int nSensorReactionHistoryID, int nDescriptionID, int? nSensorZoneHistoryID)
        {
            Dictionary<SensorReactionHistoryDescription.Fields, object> dicFieldDatas = new Dictionary<SensorReactionHistoryDescription.Fields, object>();
            dicFieldDatas[SensorReactionHistoryDescription.Fields.SensorReactionHistoryID] = nSensorReactionHistoryID;
            dicFieldDatas[SensorReactionHistoryDescription.Fields.DescriptionID] = nDescriptionID;
            dicFieldDatas[SensorReactionHistoryDescription.Fields.SensorZoneHistoryID] = nSensorZoneHistoryID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                SensorReactionHistoryDescription.TableName,
                GetFieldNames<SensorReactionHistoryDescription.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", SensorReactionHistoryDescription.GetFieldName(SensorReactionHistoryDescription.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<SensorReactionHistoryDescription> datas = m_dataManager.GetSelectManager().SelectSensorReactionHistoryDescriptions(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSensorReactionHistoryDescription(datas[0], nSensorReactionHistoryID, nDescriptionID, nSensorZoneHistoryID))
                    return datas[0];

                return GetSensorReactionHistoryDescription(nSensorReactionHistoryID, nDescriptionID, nSensorZoneHistoryID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private SensorReactionHistoryDescription GetSensorReactionHistoryDescription(int nSensorReactionHistoryID, int nDescriptionID, int? nSensorZoneHistoryID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", SensorReactionHistoryDescription.GetFieldName(SensorReactionHistoryDescription.Fields.ID, out isNullable), id);

            List<SensorReactionHistoryDescription> datas = m_dataManager.GetSelectManager().SelectSensorReactionHistoryDescriptions(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (SensorReactionHistoryDescription data in datas)
            {
                if (IsSameSensorReactionHistoryDescription(data, nSensorReactionHistoryID, nDescriptionID, nSensorZoneHistoryID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSensorReactionHistoryDescription(nSensorReactionHistoryID, nDescriptionID, nSensorZoneHistoryID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(SensorReactionHistoryDescription.TableName);
            return null;
        }

        private bool IsSameSensorReactionHistoryDescription(SensorReactionHistoryDescription data, int nSensorReactionHistoryID, int nDescriptionID, int? nSensorZoneHistoryID)
        {
            if (data.SensorReactionHistoryID == nSensorReactionHistoryID &&
                data.DescriptionID == nDescriptionID &&
                data.SensorReactionHistoryID == nSensorZoneHistoryID)
                return true;

            return false;
        }

        public SensorReactionHistoryDescriptionText CreateSensorReactionHistoryDescriptionText(int nRefCount, string strDescription)
        {
            Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicFieldDatas = new Dictionary<SensorReactionHistoryDescriptionText.Fields, object>();
            dicFieldDatas[SensorReactionHistoryDescriptionText.Fields.RefCount] = nRefCount;
            dicFieldDatas[SensorReactionHistoryDescriptionText.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                SensorReactionHistoryDescriptionText.TableName,
                GetFieldNames<SensorReactionHistoryDescriptionText.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", SensorReactionHistoryDescriptionText.GetFieldName(SensorReactionHistoryDescriptionText.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<SensorReactionHistoryDescriptionText> datas = m_dataManager.GetSelectManager().SelectSensorReactionHistoryDescriptionTexts(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSensorReactionHistoryDescriptionText(datas[0], nRefCount, strDescription))
                    return datas[0];

                return GetSensorReactionHistoryDescriptionText(nRefCount, strDescription, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private SensorReactionHistoryDescriptionText GetSensorReactionHistoryDescriptionText(int nRefCount, string strDescription, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", SensorReactionHistoryDescriptionText.GetFieldName(SensorReactionHistoryDescriptionText.Fields.ID, out isNullable), id);

            List<SensorReactionHistoryDescriptionText> datas = m_dataManager.GetSelectManager().SelectSensorReactionHistoryDescriptionTexts(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (SensorReactionHistoryDescriptionText data in datas)
            {
                if (IsSameSensorReactionHistoryDescriptionText(data, nRefCount, strDescription))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSensorReactionHistoryDescriptionText(nRefCount, strDescription, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(SensorReactionHistoryDescriptionText.TableName);
            return null;
        }

        private bool IsSameSensorReactionHistoryDescriptionText(SensorReactionHistoryDescriptionText data, int nRefCount, string strDescription)
        {
            if (data.RefCount == nRefCount &&
                data.Description == strDescription)
                return true;

            return false;
        }

        public Model.Broadcast.Broadcast CreateBroadcast(string strText, bool useSiren, int nPlayOption, int nRepeatCount, DateTime requestTime, int nSiteID)
        {
            Dictionary<Model.Broadcast.Broadcast.Fields, object> dicFieldDatas = new Dictionary<Model.Broadcast.Broadcast.Fields, object>();
            dicFieldDatas[Model.Broadcast.Broadcast.Fields.Text] = strText;
            dicFieldDatas[Model.Broadcast.Broadcast.Fields.UseSiren] = useSiren;
            dicFieldDatas[Model.Broadcast.Broadcast.Fields.PlayOption] = nPlayOption;
            dicFieldDatas[Model.Broadcast.Broadcast.Fields.RepeatCount] = nRepeatCount;
            dicFieldDatas[Model.Broadcast.Broadcast.Fields.RequestTime] = requestTime;
            dicFieldDatas[Model.Broadcast.Broadcast.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Model.Broadcast.Broadcast.TableName,
                GetFieldNames<Model.Broadcast.Broadcast.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Model.Broadcast.Broadcast.GetFieldName(Model.Broadcast.Broadcast.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Model.Broadcast.Broadcast> datas = m_dataManager.GetSelectManager().SelectBroadcasts(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameBroadcast(datas[0], strText, useSiren, nPlayOption, nRepeatCount, requestTime, nSiteID))
                    return datas[0];

                return GetBroadcast(strText, useSiren, nPlayOption, nRepeatCount, requestTime, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Model.Broadcast.Broadcast GetBroadcast(string strText, bool useSiren, int nPlayOption, int nRepeatCount, DateTime requestTime, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Model.Broadcast.Broadcast.GetFieldName(Model.Broadcast.Broadcast.Fields.ID, out isNullable), id);

            List<Model.Broadcast.Broadcast> datas = m_dataManager.GetSelectManager().SelectBroadcasts(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Model.Broadcast.Broadcast data in datas)
            {
                if (IsSameBroadcast(data, strText, useSiren, nPlayOption, nRepeatCount, requestTime, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetBroadcast(strText, useSiren, nPlayOption, nRepeatCount, requestTime, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Model.Broadcast.Broadcast.TableName);
            return null;
        }

        private bool IsSameBroadcast(Model.Broadcast.Broadcast data, string strText, bool useSiren, int nPlayOption, int nRepeatCount, DateTime requestTime, int nSiteID)
        {
            if (data.Text == strText &&
                data.UseSiren == useSiren &&
                data.PlayOption == nPlayOption &&
                data.RepeatCount == nRepeatCount &&
                data.RequestTime.ToString("yyyyMMddHHmmss") == requestTime.ToString("yyyyMMddHHmmss") &&
                data.SiteID == nSiteID)
                return true;

            return false;
        }

        public Model.Broadcast.History CreateBroadcastHistory(string strText, bool useSiren, int nPlayOption, int nRepeatCount, DateTime requestTime, DateTime executeTime, int nSiteID)
        {
            Dictionary<Model.Broadcast.History.Fields, object> dicFieldDatas = new Dictionary<Model.Broadcast.History.Fields, object>();
            dicFieldDatas[Model.Broadcast.History.Fields.Text] = strText;
            dicFieldDatas[Model.Broadcast.History.Fields.UseSiren] = useSiren;
            dicFieldDatas[Model.Broadcast.History.Fields.PlayOption] = nPlayOption;
            dicFieldDatas[Model.Broadcast.History.Fields.RepeatCount] = nRepeatCount;
            dicFieldDatas[Model.Broadcast.History.Fields.RequestTime] = requestTime;
            dicFieldDatas[Model.Broadcast.History.Fields.ExecuteTime] = executeTime;
            dicFieldDatas[Model.Broadcast.History.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Model.Broadcast.History.TableName,
                GetFieldNames<Model.Broadcast.History.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Model.Broadcast.History.GetFieldName(Model.Broadcast.History.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Model.Broadcast.History> datas = m_dataManager.GetSelectManager().SelectBroadcastHistories(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameBroadcastHistory(datas[0], strText, useSiren, nPlayOption, nRepeatCount, requestTime, executeTime, nSiteID))
                    return datas[0];

                return GetBroadcastHistory(strText, useSiren, nPlayOption, nRepeatCount, requestTime, executeTime, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Model.Broadcast.History GetBroadcastHistory(string strText, bool useSiren, int nPlayOption, int nRepeatCount, DateTime requestTime, DateTime executeTime, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Model.Broadcast.History.GetFieldName(Model.Broadcast.History.Fields.ID, out isNullable), id);

            List<Model.Broadcast.History> datas = m_dataManager.GetSelectManager().SelectBroadcastHistories(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Model.Broadcast.History data in datas)
            {
                if (IsSameBroadcastHistory(data, strText, useSiren, nPlayOption, nRepeatCount, requestTime, executeTime, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetBroadcastHistory(strText, useSiren, nPlayOption, nRepeatCount, requestTime, executeTime, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Model.Broadcast.History.TableName);
            return null;
        }

        private bool IsSameBroadcastHistory(Model.Broadcast.History data, string strText, bool useSiren, int nPlayOption, int nRepeatCount, DateTime requestTime, DateTime executeTime, int nSiteID)
        {
            if (data.Text == strText &&
                data.UseSiren == useSiren &&
                data.PlayOption == nPlayOption &&
                data.RepeatCount == nRepeatCount &&
                data.RequestTime.ToString("yyyyMMddHHmmss") == requestTime.ToString("yyyyMMddHHmmss") &&
                data.ExecuteTime.ToString("yyyyMMddHHmmss") == executeTime.ToString("yyyyMMddHHmmss") &&
                data.SiteID == nSiteID)
                return true;

            return false;
        }

        public Model.Broadcast.State CreateBroadcastState(DateTime heartBeat, int nBState, int nSiteID)
        {
            Dictionary<Model.Broadcast.State.Fields, object> dicFieldDatas = new Dictionary<Model.Broadcast.State.Fields, object>();
            dicFieldDatas[Model.Broadcast.State.Fields.HeartBeat] = heartBeat;
            dicFieldDatas[Model.Broadcast.State.Fields.BState] = nBState;
            dicFieldDatas[Model.Broadcast.State.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Model.Broadcast.State.TableName,
                GetFieldNames<Model.Broadcast.State.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Model.Broadcast.State.GetFieldName(Model.Broadcast.State.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Model.Broadcast.State> datas = m_dataManager.GetSelectManager().SelectBroadcastStates(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameBroadcastState(datas[0], heartBeat, nBState, nSiteID))
                    return datas[0];

                return GetBroadcastState(heartBeat, nBState, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Model.Broadcast.State GetBroadcastState(DateTime heartBeat, int nBState, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Model.Broadcast.State.GetFieldName(Model.Broadcast.State.Fields.ID, out isNullable), id);

            List<Model.Broadcast.State> datas = m_dataManager.GetSelectManager().SelectBroadcastStates(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Model.Broadcast.State data in datas)
            {
                if (IsSameBroadcastState(data, heartBeat, nBState, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetBroadcastState(heartBeat, nBState, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Model.Broadcast.State.TableName);
            return null;
        }

        private bool IsSameBroadcastState(Model.Broadcast.State data, DateTime heartBeat, int nBState, int nSiteID)
        {
            if (data.HeartBeat.ToString("yyyyMMddHHmmss") == heartBeat.ToString("yyyyMMddHHmmss") &&
                data.BState == nBState &&
                data.SiteID == nSiteID)
                return true;

            return false;
        }

        public SMSHistory CreateSMSHistory(int nSensorZoneHistoryID, int nSensorReactionHistoryID, string strSMSMessage, bool sendType, List<int> regularMemberIDList = null)
        {
            Dictionary<SMSHistory.Fields, object> dicFieldDatas = new Dictionary<SMSHistory.Fields, object>();
            dicFieldDatas[SMSHistory.Fields.SensorZoneHistoryID] = nSensorZoneHistoryID;
            dicFieldDatas[SMSHistory.Fields.SensorReactionHistoryID] = nSensorReactionHistoryID;
            dicFieldDatas[SMSHistory.Fields.SMSMessage] = strSMSMessage;
            dicFieldDatas[SMSHistory.Fields.SendType] = sendType;
            dicFieldDatas[SMSHistory.Fields.RegularMemberIDList] = regularMemberIDList == null ? null : ListToString(regularMemberIDList);

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                SMSHistory.TableName,
                GetFieldNames<SMSHistory.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", SMSHistory.GetFieldName(SMSHistory.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<SMSHistory> datas = m_dataManager.GetSelectManager().SelectSMSHistories(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSMSHistory(datas[0], nSensorZoneHistoryID, nSensorReactionHistoryID, strSMSMessage, sendType, regularMemberIDList))
                    return datas[0];

                return GetSMSHistory(nSensorZoneHistoryID, nSensorReactionHistoryID, strSMSMessage, sendType, regularMemberIDList, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private SMSHistory GetSMSHistory(int nSensorZoneHistoryID, int nSensorReactionHistoryID, string strSMSMessage, bool sendType, List<int> regularMemberIDList, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", SMSHistory.GetFieldName(SMSHistory.Fields.ID, out isNullable), id);

            List<SMSHistory> datas = m_dataManager.GetSelectManager().SelectSMSHistories(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (SMSHistory data in datas)
            {
                if (IsSameSMSHistory(data, nSensorZoneHistoryID, nSensorReactionHistoryID, strSMSMessage, sendType, regularMemberIDList))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSMSHistory(nSensorZoneHistoryID, nSensorReactionHistoryID, strSMSMessage, sendType, regularMemberIDList, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(SMSHistory.TableName);
            return null;
        }

        private bool IsSameSMSHistory(SMSHistory data, int nSensorZoneHistoryID, int nSensorReactionHistoryID, string strSMSMessage, bool sendType, List<int> regularMemberIDList)
        {
            if (data.SensorZoneHistoryID == nSensorZoneHistoryID &&
                data.SensorReactionHistoryID == nSensorReactionHistoryID &&
                data.SMSMessage == strSMSMessage &&
                data.SendType == sendType &&
                IsSameList<int>(data.RegularMemberIDList, regularMemberIDList))
                return true;

            return false;
        }

        public Model.Config.Broadcast CreateBroadcastConfig(int nSituationType, bool useBroadcast, string strMessage, bool useSiren, int nRepeatCount, string strDescription, int nSiteID)
        {
            Dictionary<Model.Config.Broadcast.Fields, object> dicFieldDatas = new Dictionary<Model.Config.Broadcast.Fields, object>();
            dicFieldDatas[Model.Config.Broadcast.Fields.SituationType] = nSituationType;
            dicFieldDatas[Model.Config.Broadcast.Fields.UseBroadcast] = useBroadcast;
            dicFieldDatas[Model.Config.Broadcast.Fields.Message] = strMessage;
            dicFieldDatas[Model.Config.Broadcast.Fields.UseSiren] = useSiren;
            dicFieldDatas[Model.Config.Broadcast.Fields.RepeatCount] = nRepeatCount;
            dicFieldDatas[Model.Config.Broadcast.Fields.Description] = strDescription;
            dicFieldDatas[Model.Config.Broadcast.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Model.Config.Broadcast.TableName,
                GetFieldNames<Model.Config.Broadcast.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Model.Config.Broadcast.GetFieldName(Model.Config.Broadcast.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Model.Config.Broadcast> datas = m_dataManager.GetSelectManager().SelectBroadcastConfigs(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameBroadcastConfig(datas[0], nSituationType, useBroadcast, strMessage, useSiren, nRepeatCount, strDescription, nSiteID))
                    return datas[0];

                return GetBroadcastConfig(nSituationType, useBroadcast, strMessage, useSiren, nRepeatCount, strDescription, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Model.Config.Broadcast GetBroadcastConfig(int nSituationType, bool useBroadcast, string strMessage, bool useSiren, int nRepeatCount, string strDescription, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Model.Config.Broadcast.GetFieldName(Model.Config.Broadcast.Fields.ID, out isNullable), id);

            List<Model.Config.Broadcast> datas = m_dataManager.GetSelectManager().SelectBroadcastConfigs(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Model.Config.Broadcast data in datas)
            {
                if (IsSameBroadcastConfig(data, nSituationType, useBroadcast, strMessage, useSiren, nRepeatCount, strDescription, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetBroadcastConfig(nSituationType, useBroadcast, strMessage, useSiren, nRepeatCount, strDescription, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Model.Config.Broadcast.TableName);
            return null;
        }

        private bool IsSameBroadcastConfig(Model.Config.Broadcast data, int nSituationType, bool useBroadcast, string strMessage, bool useSiren, int nRepeatCount, string strDescription, int nSiteID)
        {
            if (data.SituationType == nSituationType &&
                data.UseBroadcast == useBroadcast &&
                data.Message == strMessage &&
                data.UseSiren == useSiren &&
                data.RepeatCount == nRepeatCount &&
                data.Description == strDescription &&
                data.SiteID == nSiteID)
                return true;

            return false;
        }

        public Model.Config.SMS CreateSMSConfig(int nMessageType, bool useSMS, string strDescription, int nSiteID)
        {
            Dictionary<Model.Config.SMS.Fields, object> dicFieldDatas = new Dictionary<Model.Config.SMS.Fields, object>();
            dicFieldDatas[Model.Config.SMS.Fields.MessageType] = nMessageType;
            dicFieldDatas[Model.Config.SMS.Fields.UseSMS] = useSMS;
            dicFieldDatas[Model.Config.SMS.Fields.Description] = strDescription;
            dicFieldDatas[Model.Config.SMS.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Model.Config.SMS.TableName,
                GetFieldNames<Model.Config.SMS.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Model.Config.SMS.GetFieldName(Model.Config.SMS.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Model.Config.SMS> datas = m_dataManager.GetSelectManager().SelectSMSConfigs(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSMSConfig(datas[0], nMessageType, useSMS, strDescription, nSiteID))
                    return datas[0];

                return GetSMSConfig(nMessageType, useSMS, strDescription, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Model.Config.SMS GetSMSConfig(int nMessageType, bool useSMS, string strDescription, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Model.Config.SMS.GetFieldName(Model.Config.SMS.Fields.ID, out isNullable), id);

            List<Model.Config.SMS> datas = m_dataManager.GetSelectManager().SelectSMSConfigs(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Model.Config.SMS data in datas)
            {
                if (IsSameSMSConfig(data, nMessageType, useSMS, strDescription, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSMSConfig(nMessageType, useSMS, strDescription, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Model.Config.SMS.TableName);
            return null;
        }

        private bool IsSameSMSConfig(Model.Config.SMS data, int nMessageType, bool useSMS, string strDescription, int nSiteID)
        {
            if (data.MessageType == nMessageType &&
                data.UseSMS == useSMS &&
                data.Description == strDescription &&
                data.SiteID == nSiteID)
                return true;

            return false;
        }

        public CurrentAlarm CreateCurrentAlarm(int nSensorZoneHistoryID, int nSensorType, int nAlarmType, DateTime timeStamp, int nSopStatus, int nAlarmDepth, List<int> alarmSensorZoneIDs)
        {
            Dictionary<CurrentAlarm.Fields, object> dicFieldDatas = new Dictionary<CurrentAlarm.Fields, object>();
            dicFieldDatas[CurrentAlarm.Fields.SensorZoneHistoryID] = nSensorZoneHistoryID;
            dicFieldDatas[CurrentAlarm.Fields.SensorType] = nSensorType;
            dicFieldDatas[CurrentAlarm.Fields.AlarmType] = nAlarmType;
            dicFieldDatas[CurrentAlarm.Fields.TimeStamp] = timeStamp;
            dicFieldDatas[CurrentAlarm.Fields.SopStatus] = nSopStatus;
            dicFieldDatas[CurrentAlarm.Fields.AlarmDepth] = nAlarmDepth;
            dicFieldDatas[CurrentAlarm.Fields.AlarmSensorZoneIDs] = ListToString(alarmSensorZoneIDs);

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                CurrentAlarm.TableName,
                GetFieldNames<CurrentAlarm.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                CurrentAlarm alarm = new CurrentAlarm();

                alarm.SensorZoneHistoryID = nSensorZoneHistoryID;
                alarm.SensorType = nSensorType;
                alarm.AlarmType = nAlarmType;
                alarm.TimeStamp = timeStamp;
                alarm.SopStatus = nSopStatus;
                alarm.AlarmDepth = nAlarmDepth;
                alarm.AlarmSensorZoneIDs = alarmSensorZoneIDs;

                return alarm;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public FacilityManager CreateFacilityManager(int nMemberID, int nMemberType, int nFacilityType, int nDetectType, string strDescription, int nSiteID)
        {
            Dictionary<FacilityManager.Fields, object> dicFieldDatas = new Dictionary<FacilityManager.Fields, object>();
            dicFieldDatas[FacilityManager.Fields.MemberID] = nMemberID;
            dicFieldDatas[FacilityManager.Fields.MemberType] = nMemberType;
            dicFieldDatas[FacilityManager.Fields.FacilityType] = nFacilityType;
            dicFieldDatas[FacilityManager.Fields.DetectType] = nDetectType;
            dicFieldDatas[FacilityManager.Fields.Description] = strDescription;
            dicFieldDatas[FacilityManager.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                FacilityManager.TableName,
                GetFieldNames<FacilityManager.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", FacilityManager.GetFieldName(FacilityManager.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<FacilityManager> datas = m_dataManager.GetSelectManager().SelectFacilityManagers(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameFacilityManager(datas[0], nMemberID, nMemberType, nFacilityType, nDetectType, strDescription, nSiteID))
                    return datas[0];

                return GetFacilityManager(nMemberID, nMemberType, nFacilityType, nDetectType, strDescription, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private FacilityManager GetFacilityManager(int nMemberID, int nMemberType, int nFacilityType, int nDetectType, string strDescription, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", FacilityManager.GetFieldName(FacilityManager.Fields.ID, out isNullable), id);

            List<FacilityManager> datas = m_dataManager.GetSelectManager().SelectFacilityManagers(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (FacilityManager data in datas)
            {
                if (IsSameFacilityManager(data, nMemberID, nMemberType, nFacilityType, nDetectType, strDescription, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetFacilityManager(nMemberID, nMemberType, nFacilityType, nDetectType, strDescription, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(FacilityManager.TableName);
            return null;
        }

        private bool IsSameFacilityManager(FacilityManager data, int nMemberID, int nMemberType, int nFacilityType, int nDetectType, string strDescription, int nSiteID)
        {
            if (data.MemberID == nMemberID &&
                data.MemberType == nMemberType &&
                data.FacilityType == nFacilityType &&
                data.DetectType == nDetectType &&
                data.Description == strDescription &&
                data.SiteID == nSiteID)
                return true;

            return false;
        }

        public BuildingFacilityManager CreateBuildingFacilityManager(int nMemberID, int nMemberType, int nFacilityType, int nDetectType, int nBuildingID, string strDescription, int nSiteID)
        {
            Dictionary<BuildingFacilityManager.Fields, object> dicFieldDatas = new Dictionary<BuildingFacilityManager.Fields, object>();
            dicFieldDatas[BuildingFacilityManager.Fields.MemberID] = nMemberID;
            dicFieldDatas[BuildingFacilityManager.Fields.MemberType] = nMemberType;
            dicFieldDatas[BuildingFacilityManager.Fields.FacilityType] = nFacilityType;
            dicFieldDatas[BuildingFacilityManager.Fields.DetectType] = nDetectType;
            dicFieldDatas[BuildingFacilityManager.Fields.Description] = strDescription;
            dicFieldDatas[BuildingFacilityManager.Fields.BuildingID] = nBuildingID;
            dicFieldDatas[BuildingFacilityManager.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                BuildingFacilityManager.TableName,
                GetFieldNames<BuildingFacilityManager.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", BuildingFacilityManager.GetFieldName(BuildingFacilityManager.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<BuildingFacilityManager> datas = m_dataManager.GetSelectManager().SelectBuildingFacilityManagers(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameBuildingFacilityManager(datas[0], nMemberID, nMemberType, nFacilityType, nDetectType, nBuildingID, strDescription, nSiteID))
                    return datas[0];

                return GetBuildingFacilityManager(nMemberID, nMemberType, nFacilityType, nDetectType, nBuildingID, strDescription, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private BuildingFacilityManager GetBuildingFacilityManager(int nMemberID, int nMemberType, int nFacilityType, int nDetectType, int nBuildingID, string strDescription, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", BuildingFacilityManager.GetFieldName(BuildingFacilityManager.Fields.ID, out isNullable), id);

            List<BuildingFacilityManager> datas = m_dataManager.GetSelectManager().SelectBuildingFacilityManagers(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (BuildingFacilityManager data in datas)
            {
                if (IsSameBuildingFacilityManager(data, nMemberID, nMemberType, nFacilityType, nDetectType, nBuildingID, strDescription, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetBuildingFacilityManager(nMemberID, nMemberType, nFacilityType, nDetectType, nBuildingID, strDescription, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(BuildingFacilityManager.TableName);
            return null;
        }

        private bool IsSameBuildingFacilityManager(BuildingFacilityManager data, int nMemberID, int nMemberType, int nFacilityType, int nDetectType, int nBuildingID, string strDescription, int nSiteID)
        {
            if (data.MemberID == nMemberID &&
                data.MemberType == nMemberType &&
                data.FacilityType == nFacilityType &&
                data.DetectType == nDetectType &&
                data.BuildingID == nBuildingID &&
                data.Description == strDescription &&
                data.SiteID == nSiteID)
                return true;

            return false;
        }

        public EquipZoneFacilityManager CreateEquipZoneFacilityManager(int nMemberID, int nMemberType, int nFacilityType, int nDetectType, int nEquipZoneID, string strDescription, int nSiteID)
        {
            Dictionary<EquipZoneFacilityManager.Fields, object> dicFieldDatas = new Dictionary<EquipZoneFacilityManager.Fields, object>();
            dicFieldDatas[EquipZoneFacilityManager.Fields.MemberID] = nMemberID;
            dicFieldDatas[EquipZoneFacilityManager.Fields.MemberType] = nMemberType;
            dicFieldDatas[EquipZoneFacilityManager.Fields.FacilityType] = nFacilityType;
            dicFieldDatas[EquipZoneFacilityManager.Fields.DetectType] = nDetectType;
            dicFieldDatas[EquipZoneFacilityManager.Fields.Description] = strDescription;
            dicFieldDatas[EquipZoneFacilityManager.Fields.EquipZoneID] = nEquipZoneID;
            dicFieldDatas[EquipZoneFacilityManager.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                BuildingFacilityManager.TableName,
                GetFieldNames<BuildingFacilityManager.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", EquipZoneFacilityManager.GetFieldName(EquipZoneFacilityManager.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<EquipZoneFacilityManager> datas = m_dataManager.GetSelectManager().SelectEquipZoneFacilityManagers(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameEquipZoneFacilityManager(datas[0], nMemberID, nMemberType, nFacilityType, nDetectType, nEquipZoneID, strDescription, nSiteID))
                    return datas[0];

                return GetEquipZoneFacilityManager(nMemberID, nMemberType, nFacilityType, nDetectType, nEquipZoneID, strDescription, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private EquipZoneFacilityManager GetEquipZoneFacilityManager(int nMemberID, int nMemberType, int nFacilityType, int nDetectType, int nEquipZoneID, string strDescription, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", EquipZoneFacilityManager.GetFieldName(EquipZoneFacilityManager.Fields.ID, out isNullable), id);

            List<EquipZoneFacilityManager> datas = m_dataManager.GetSelectManager().SelectEquipZoneFacilityManagers(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (EquipZoneFacilityManager data in datas)
            {
                if (IsSameEquipZoneFacilityManager(data, nMemberID, nMemberType, nFacilityType, nDetectType, nEquipZoneID, strDescription, nSiteID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetEquipZoneFacilityManager(nMemberID, nMemberType, nFacilityType, nDetectType, nEquipZoneID, strDescription, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(EquipZoneFacilityManager.TableName);
            return null;
        }

        private bool IsSameEquipZoneFacilityManager(EquipZoneFacilityManager data, int nMemberID, int nMemberType, int nFacilityType, int nDetectType, int nEquipZoneID, string strDescription, int nSiteID)
        {
            if (data.MemberID == nMemberID &&
                data.MemberType == nMemberType &&
                data.FacilityType == nFacilityType &&
                data.DetectType == nDetectType &&
                data.EquipZoneID == nEquipZoneID &&
                data.Description == strDescription &&
                data.SiteID == nSiteID)
                return true;

            return false;
        }

        public CCTV CreateCCTV(string strCameraName, string strPositionName, string strUniqueKey, float? x, float? y, float? z, int? nZoneID, bool isIndoor, string strType, int? nChannel, string strUserID, string strPassword, string strURL, string strBigURL, string strSmallURL, bool? enabled, string strCameraIP, string strCameraCompanyName, string strCameraModelName, string strDescription)
        {
            Dictionary<CCTV.Fields, object> dicFieldDatas = new Dictionary<CCTV.Fields, object>();
            dicFieldDatas[CCTV.Fields.CameraName] = strCameraName;
            dicFieldDatas[CCTV.Fields.PositionName] = strPositionName;
            dicFieldDatas[CCTV.Fields.UniqueKey] = strUniqueKey;
            dicFieldDatas[CCTV.Fields.X] = x;
            dicFieldDatas[CCTV.Fields.Y] = y;
            dicFieldDatas[CCTV.Fields.Z] = z;
            dicFieldDatas[CCTV.Fields.ZoneID] = nZoneID;
            dicFieldDatas[CCTV.Fields.IsIndoor] = isIndoor;
            dicFieldDatas[CCTV.Fields.Type] = strType;
            dicFieldDatas[CCTV.Fields.Channel] = nChannel;
            dicFieldDatas[CCTV.Fields.UserID] = strUserID;
            dicFieldDatas[CCTV.Fields.Password] = strPassword;
            dicFieldDatas[CCTV.Fields.URL] = strURL;
            dicFieldDatas[CCTV.Fields.BigURL] = strBigURL;
            dicFieldDatas[CCTV.Fields.SmallURL] = strSmallURL;
            dicFieldDatas[CCTV.Fields.Enabled] = enabled;
            dicFieldDatas[CCTV.Fields.CameraIP] = strCameraIP;
            dicFieldDatas[CCTV.Fields.CameraCompanyName] = strCameraCompanyName;
            dicFieldDatas[CCTV.Fields.CameraModelName] = strCameraModelName;
            dicFieldDatas[CCTV.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                CCTV.TableName,
                GetFieldNames<CCTV.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", CCTV.GetFieldName(CCTV.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<CCTV> datas = m_dataManager.GetSelectManager().SelectCCTVs(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameCCTV(datas[0], strCameraName, strPositionName, strUniqueKey, x, y, z, nZoneID, isIndoor, strType, nChannel, strUserID, strPassword, strURL, strBigURL, strSmallURL, enabled, strCameraIP, strCameraCompanyName, strCameraModelName, strDescription))
                    return datas[0];

                return GetCCTV(strCameraName, strPositionName, strUniqueKey, x, y, z, nZoneID, isIndoor, strType, nChannel, strUserID, strPassword, strURL, strBigURL, strSmallURL, enabled, strCameraIP, strCameraCompanyName, strCameraModelName, strDescription, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private CCTV GetCCTV(string strCameraName, string strPositionName, string strUniqueKey, float? x, float? y, float? z, int? nZoneID, bool isIndoor, string strType, int? nChannel, string strUserID, string strPassword, string strURL, string strBigURL, string strSmallURL, bool? enabled, string strCameraIP, string strCameraCompanyName, string strCameraModelName, string strDescription, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", CCTV.GetFieldName(CCTV.Fields.ID, out isNullable), id);

            List<CCTV> datas = m_dataManager.GetSelectManager().SelectCCTVs(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (CCTV data in datas)
            {
                if (IsSameCCTV(data, strCameraName, strPositionName, strUniqueKey, x, y, z, nZoneID, isIndoor, strType, nChannel, strUserID, strPassword, strURL, strBigURL, strSmallURL, enabled, strCameraIP, strCameraCompanyName, strCameraModelName, strDescription))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetCCTV(strCameraName, strPositionName, strUniqueKey, x, y, z, nZoneID, isIndoor, strType, nChannel, strUserID, strPassword, strURL, strBigURL, strSmallURL, enabled, strCameraIP, strCameraCompanyName, strCameraModelName, strDescription, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(CCTV.TableName);
            return null;
        }

        private bool IsSameCCTV(CCTV data, string strCameraName, string strPositionName, string strUniqueKey, float? x, float? y, float? z, int? nZoneID, bool isIndoor, string strType, int? nChannel, string strUserID, string strPassword, string strURL, string strBigURL, string strSmallURL, bool? enabled, string strCameraIP, string strCameraCompanyName, string strCameraModelName, string strDescription)
        {
            if (data.CameraName == strCameraModelName &&
                data.PositionName == strPositionName &&
                data.UniqueKey == strUniqueKey &&
                IsSameFloatData(data.X, x) &&
                IsSameFloatData(data.Y, y) &&
                IsSameFloatData(data.Z, z) &&
                data.ZoneID == nZoneID &&
                data.IsIndoor == isIndoor &&
                data.Type == strType &&
                data.Channel == nChannel &&
                data.UserID == strUserID &&
                data.Password == strPassword &&
                data.URL == strURL &&
                data.BigURL == strBigURL &&
                data.SmallURL == strSmallURL &&
                data.Enabled == enabled &&
                data.CameraIP == strCameraIP &&
                data.CameraCompanyName == strCameraCompanyName &&
                data.CameraModelName == strCameraModelName &&
                data.Description == strDescription)
                return true;

            return false;
        }

        public EquipZoneCCTV CreateEquipZoneCCTV(int nEquipZoneID, int? nCCTV1, int? nCCTV2, int? nCCTV3, int? nCCTV4, int? nCCTV5, int? nCCTV6, string strPreset1, string strPreset2, string strPreset3, string strPreset4, string strPreset5, string strPreset6, string strDescription)
        {
            Dictionary<EquipZoneCCTV.Fields, object> dicFieldDatas = new Dictionary<EquipZoneCCTV.Fields, object>();
            dicFieldDatas[EquipZoneCCTV.Fields.EquipZoneID] = nEquipZoneID;
            dicFieldDatas[EquipZoneCCTV.Fields.CCTV1] = nCCTV1;
            dicFieldDatas[EquipZoneCCTV.Fields.CCTV2] = nCCTV2;
            dicFieldDatas[EquipZoneCCTV.Fields.CCTV3] = nCCTV3;
            dicFieldDatas[EquipZoneCCTV.Fields.CCTV4] = nCCTV4;
            dicFieldDatas[EquipZoneCCTV.Fields.CCTV5] = nCCTV5;
            dicFieldDatas[EquipZoneCCTV.Fields.CCTV6] = nCCTV6;
            dicFieldDatas[EquipZoneCCTV.Fields.Preset1] = strPreset1;
            dicFieldDatas[EquipZoneCCTV.Fields.Preset2] = strPreset2;
            dicFieldDatas[EquipZoneCCTV.Fields.Preset3] = strPreset3;
            dicFieldDatas[EquipZoneCCTV.Fields.Preset4] = strPreset4;
            dicFieldDatas[EquipZoneCCTV.Fields.Preset5] = strPreset5;
            dicFieldDatas[EquipZoneCCTV.Fields.Preset6] = strPreset6;
            dicFieldDatas[EquipZoneCCTV.Fields.Description] = strDescription;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                EquipZoneCCTV.TableName,
                GetFieldNames<EquipZoneCCTV.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", EquipZoneCCTV.GetFieldName(EquipZoneCCTV.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<EquipZoneCCTV> datas = m_dataManager.GetSelectManager().SelectEquipZoneCCTVs(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameEquipZoneCCTV(datas[0], nEquipZoneID, nCCTV1, nCCTV2, nCCTV3, nCCTV4, nCCTV5, nCCTV6, strPreset1, strPreset2, strPreset3, strPreset4, strPreset5, strPreset6, strDescription))
                    return datas[0];

                return GetEquipZoneCCTV(nEquipZoneID, nCCTV1, nCCTV2, nCCTV3, nCCTV4, nCCTV5, nCCTV6, strPreset1, strPreset2, strPreset3, strPreset4, strPreset5, strPreset6, strDescription, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private EquipZoneCCTV GetEquipZoneCCTV(int nEquipZoneID, int? nCCTV1, int? nCCTV2, int? nCCTV3, int? nCCTV4, int? nCCTV5, int? nCCTV6, string strPreset1, string strPreset2, string strPreset3, string strPreset4, string strPreset5, string strPreset6, string strDescription, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", EquipZoneCCTV.GetFieldName(EquipZoneCCTV.Fields.ID, out isNullable), id);

            List<EquipZoneCCTV> datas = m_dataManager.GetSelectManager().SelectEquipZoneCCTVs(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (EquipZoneCCTV data in datas)
            {
                if (IsSameEquipZoneCCTV(data, nEquipZoneID, nCCTV1, nCCTV2, nCCTV3, nCCTV4, nCCTV5, nCCTV6, strPreset1, strPreset2, strPreset3, strPreset4, strPreset5, strPreset6, strDescription))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetEquipZoneCCTV(nEquipZoneID, nCCTV1, nCCTV2, nCCTV3, nCCTV4, nCCTV5, nCCTV6, strPreset1, strPreset2, strPreset3, strPreset4, strPreset5, strPreset6, strDescription, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(EquipZoneCCTV.TableName);
            return null;
        }

        private bool IsSameEquipZoneCCTV(EquipZoneCCTV data, int nEquipZoneID, int? nCCTV1, int? nCCTV2, int? nCCTV3, int? nCCTV4, int? nCCTV5, int? nCCTV6, string strPreset1, string strPreset2, string strPreset3, string strPreset4, string strPreset5, string strPreset6, string strDescription)
        {
            if (data.EquipZoneID == nEquipZoneID &&
                data.CCTV1 == nCCTV1 &&
                data.CCTV2 == nCCTV2 &&
                data.CCTV3 == nCCTV3 &&
                data.CCTV4 == nCCTV4 &&
                data.CCTV5 == nCCTV5 &&
                data.CCTV6 == nCCTV6 &&
                data.Preset1 == strPreset1 &&
                data.Preset2 == strPreset2 &&
                data.Preset3 == strPreset3 &&
                data.Preset4 == strPreset4 &&
                data.Preset5 == strPreset5 &&
                data.Preset6 == strPreset6 &&
                data.Description == strDescription)
                return true;

            return false;
        }

        public Model.GLTF.Model CreateGltfModel(int? nParentID, string strModelName, int nSiteID)
        {
            Dictionary<Model.GLTF.Model.Fields, object> dicFieldDatas = new Dictionary<Model.GLTF.Model.Fields, object>();
            dicFieldDatas[Model.GLTF.Model.Fields.ParentID] = nParentID;
            dicFieldDatas[Model.GLTF.Model.Fields.ModelName] = strModelName;
            dicFieldDatas[Model.GLTF.Model.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Model.GLTF.Model.TableName,
                GetFieldNames<Model.GLTF.Model.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Model.GLTF.Model.GetFieldName(Model.GLTF.Model.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Model.GLTF.Model> datas = m_dataManager.GetSelectManager().SelectGltfModels(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameGltfModel(datas[0], nParentID, strModelName, nSiteID))
                    return datas[0];

                return GetGltfModel(nParentID, strModelName, nSiteID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Model.GLTF.Model GetGltfModel(int? nParentID, string strModelName, int nSiteID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Model.GLTF.Model.GetFieldName(Model.GLTF.Model.Fields.ID, out isNullable), id);

            List<Model.GLTF.Model> datas = m_dataManager.GetSelectManager().SelectGltfModels(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Model.GLTF.Model data in datas)
            {
                if (IsSameGltfModel(data, nParentID, strModelName, nSiteID))
                    return data;


                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetGltfModel(nParentID, strModelName, nSiteID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Model.GLTF.Model.TableName);
            return null;
        }

        private bool IsSameGltfModel(Model.GLTF.Model data, int? nParentID, string strModelName, int nSiteID)
        {
            if (data.ParentID == nParentID &&
                data.ModelName == strModelName &&
                data.SiteID == nSiteID)
                return true;

            return false;
        }

        public Model.GLTF.ModelData CreateGltfModelData(int nModelID, string strModelFile, string strModelDisplayText, Vertex3D vCameraPosition, Quaternion qCameraQuaternion, Vertex3D vCameraRotation, int nFov, float fNear, float fFar, Vertex3D vOrbitTarget, float? fFloorIndex, int? nBuildingGroupID, int? nBuildingID, int? nZoneID)
        {
            Dictionary<Model.GLTF.ModelData.Fields, object> dicFieldDatas = new Dictionary<Model.GLTF.ModelData.Fields, object>();
            dicFieldDatas[Model.GLTF.ModelData.Fields.ModelID] = nModelID;
            dicFieldDatas[Model.GLTF.ModelData.Fields.ModelFile] = strModelFile;
            dicFieldDatas[Model.GLTF.ModelData.Fields.ModelDisplayText] = strModelDisplayText;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraPositionX] = (float)vCameraPosition.x;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraPositionY] = (float)vCameraPosition.y;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraPositionZ] = (float)vCameraPosition.z;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraQuaternionX] = (float)qCameraQuaternion.x;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraQuaternionY] = (float)qCameraQuaternion.y;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraQuaternionZ] = (float)qCameraQuaternion.z;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraQuaternionW] = (float)qCameraQuaternion.w;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraRotationX] = (float)vCameraRotation.x;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraRotationY] = (float)vCameraRotation.y;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraRotationZ] = (float)vCameraRotation.z;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraFov] = nFov;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraNear] = fNear;
            dicFieldDatas[Model.GLTF.ModelData.Fields.CameraFar] = fFar;
            dicFieldDatas[Model.GLTF.ModelData.Fields.OrbitTargetX] = (float)vOrbitTarget.x;
            dicFieldDatas[Model.GLTF.ModelData.Fields.OrbitTargetY] = (float)vOrbitTarget.y;
            dicFieldDatas[Model.GLTF.ModelData.Fields.OrbitTargetZ] = (float)vOrbitTarget.z;
            dicFieldDatas[Model.GLTF.ModelData.Fields.FloorIndex] = fFloorIndex;
            dicFieldDatas[Model.GLTF.ModelData.Fields.BuildingGroupID] = nBuildingGroupID;
            dicFieldDatas[Model.GLTF.ModelData.Fields.BuildingID] = nBuildingID;
            dicFieldDatas[Model.GLTF.ModelData.Fields.ZoneID] = nZoneID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Model.GLTF.ModelData.TableName,
                GetFieldNames<Model.GLTF.ModelData.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Model.GLTF.ModelData.GetFieldName(Model.GLTF.ModelData.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Model.GLTF.ModelData> datas = m_dataManager.GetSelectManager().SelectGltfModelDatas(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameGltfModelData(datas[0], nModelID, strModelFile, strModelDisplayText, vCameraPosition, qCameraQuaternion, vCameraRotation, nFov, fNear, fFar, vOrbitTarget, fFloorIndex, nBuildingGroupID, nBuildingID, nZoneID))
                    return datas[0];

                return GetGltfModelData(nModelID, strModelFile, strModelDisplayText, vCameraPosition, qCameraQuaternion, vCameraRotation, nFov, fNear, fFar, vOrbitTarget, fFloorIndex, nBuildingGroupID, nBuildingID, nZoneID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Model.GLTF.ModelData GetGltfModelData(int nModelID, string strModelFile, string strModelDisplayText, Vertex3D vCameraPosition, Quaternion qCameraQuaternion, Vertex3D vCameraRotation, int nFov, float fNear, float fFar, Vertex3D vOrbitTarget, float? fFloorIndex, int? nBuildingGroupID, int? nBuildingID, int? nZoneID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Model.GLTF.ModelData.GetFieldName(Model.GLTF.ModelData.Fields.ID, out isNullable), id);

            List<Model.GLTF.ModelData> datas = m_dataManager.GetSelectManager().SelectGltfModelDatas(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Model.GLTF.ModelData data in datas)
            {
                if (IsSameGltfModelData(data, nModelID, strModelFile, strModelDisplayText, vCameraPosition, qCameraQuaternion, vCameraRotation, nFov, fNear, fFar, vOrbitTarget, fFloorIndex, nBuildingGroupID, nBuildingID, nZoneID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetGltfModelData(nModelID, strModelFile, strModelDisplayText, vCameraPosition, qCameraQuaternion, vCameraRotation, nFov, fNear, fFar, vOrbitTarget, fFloorIndex, nBuildingGroupID, nBuildingID, nZoneID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Model.GLTF.ModelData.TableName);
            return null;
        }

        private bool IsSameGltfModelData(Model.GLTF.ModelData data, int nModelID, string strModelFile, string strModelDisplayText, Vertex3D vCameraPosition, Quaternion qCameraQuaternion, Vertex3D vCameraRotation, int nFov, float fNear, float fFar, Vertex3D vOrbitTarget, float? fFloorIndex, int? nBuildingGroupID, int? nBuildingID, int? nZoneID)
        {
            if (data.ModelID == nModelID &&
                data.ModelFile == strModelFile &&
                data.ModelDisplayText == strModelDisplayText &&
                IsSameVertex3D(data.CameraPosition, vCameraPosition) &&
                IsSameQuaternion(data.CameraQuaternion, qCameraQuaternion) &&
                IsSameVertex3D(data.CameraRotation, vCameraRotation) &&
                data.CameraFov == nFov &&
                IsSameFloatData2(data.CameraNear, fNear) &&
                IsSameFloatData2(data.CameraFar, fFar) &&
                IsSameVertex3D(data.OrbitTarget, vOrbitTarget) &&
                IsSameFloatData(data.FloorIndex, fFloorIndex) &&
                data.BuildingGroupID == nBuildingGroupID &&
                data.BuildingID == nBuildingID &&
                data.ZoneID == nZoneID)
                return true;

            return false;
        }

        public Model.GLTF.ModelOrthoData CreateGltfModelOrthoData(int nModelID, string strModelFile, Vertex3D vCameraPosition, Quaternion qCameraQuaternion, Vertex3D vCameraRotation, Vertex3D vTarget, float fZoom, int? nZoneID)
        {
            Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicFieldDatas = new Dictionary<Model.GLTF.ModelOrthoData.Fields, object>();
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.ModelID] = nModelID;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.ModelFile] = strModelFile;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.CameraPositionX] = (float)vCameraPosition.x;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.CameraPositionY] = (float)vCameraPosition.y;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.CameraPositionZ] = (float)vCameraPosition.z;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionX] = (float)qCameraQuaternion.x;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionY] = (float)qCameraQuaternion.y;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionZ] = (float)qCameraQuaternion.z;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionW] = (float)qCameraQuaternion.w;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.CameraRotationX] = (float)vCameraRotation.x;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.CameraRotationY] = (float)vCameraRotation.y;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.CameraRotationZ] = (float)vCameraRotation.z;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.TargetX] = (float)vTarget.x;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.TargetY] = (float)vTarget.y;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.TargetZ] = (float)vTarget.z;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.Zoom] = fZoom;
            dicFieldDatas[Model.GLTF.ModelOrthoData.Fields.ZoneID] = nZoneID;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Model.GLTF.ModelOrthoData.TableName,
                GetFieldNames<Model.GLTF.ModelOrthoData.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Model.GLTF.ModelOrthoData.GetFieldName(Model.GLTF.ModelOrthoData.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Model.GLTF.ModelOrthoData> datas = m_dataManager.GetSelectManager().SelectGltfModelOrthoDatas(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameGltfModelOrthoData(datas[0], nModelID, strModelFile, vCameraPosition, qCameraQuaternion, vCameraRotation, vTarget, fZoom, nZoneID))
                    return datas[0];

                return GetGltfModelOrthoData(nModelID, strModelFile, vCameraPosition, qCameraQuaternion, vCameraRotation, vTarget, fZoom, nZoneID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Model.GLTF.ModelOrthoData GetGltfModelOrthoData(int nModelID, string strModelFile, Vertex3D vCameraPosition, Quaternion qCameraQuaternion, Vertex3D vCameraRotation, Vertex3D vTarget, float fZoom, int? nZoneID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Model.GLTF.ModelOrthoData.GetFieldName(Model.GLTF.ModelOrthoData.Fields.ID, out isNullable), id);

            List<Model.GLTF.ModelOrthoData> datas = m_dataManager.GetSelectManager().SelectGltfModelOrthoDatas(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Model.GLTF.ModelOrthoData data in datas)
            {
                if (IsSameGltfModelOrthoData(data, nModelID, strModelFile, vCameraPosition, qCameraQuaternion, vCameraRotation, vTarget, fZoom, nZoneID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetGltfModelOrthoData(nModelID, strModelFile, vCameraPosition, qCameraQuaternion, vCameraRotation, vTarget, fZoom, nZoneID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Model.GLTF.ModelOrthoData.TableName);
            return null;
        }

        private bool IsSameGltfModelOrthoData(Model.GLTF.ModelOrthoData data, int nModelID, string strModelFile, Vertex3D vCameraPosition, Quaternion qCameraQuaternion, Vertex3D vCameraRotation, Vertex3D vTarget, float fZoom, int? nZoneID)
        {
            if (data.ModelID == nModelID &&
                data.ModelFile == strModelFile &&
                IsSameVertex3D(data.CameraPosition, vCameraPosition) &&
                IsSameQuaternion(data.CameraQuaternion, qCameraQuaternion) &&
                IsSameVertex3D(data.CameraRotation, vCameraRotation) &&
                IsSameVertex3D(data.Target, vTarget) &&
                IsSameFloatData2(data.Zoom, fZoom) &&
                data.ZoneID == nZoneID)
                return true;

            return false;
        }

        public Model.Sensor.Option.Etc CreateOptionEtcSensor(int nSensorType, int nDataType, int? nCloseAlarmSeconds, int? nDelaySeconds, int nSiteID)
        {
            Dictionary<Model.Sensor.Option.Etc.Fields, object> dicFieldDatas = new Dictionary<Model.Sensor.Option.Etc.Fields, object>();
            dicFieldDatas[Model.Sensor.Option.Etc.Fields.SensorType] = nSensorType;
            dicFieldDatas[Model.Sensor.Option.Etc.Fields.DataType] = nDataType;
            dicFieldDatas[Model.Sensor.Option.Etc.Fields.CloseAlarmSeconds] = nCloseAlarmSeconds;
            dicFieldDatas[Model.Sensor.Option.Etc.Fields.DelaySeconds] = nDelaySeconds;
            dicFieldDatas[Model.Sensor.Option.Etc.Fields.SiteID] = nSiteID;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                Model.Sensor.Option.Etc.TableName,
                GetFieldNames<Model.Sensor.Option.Etc.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                Model.Sensor.Option.Etc etc = new Model.Sensor.Option.Etc();
                etc.SensorType = nSensorType;
                etc.DataType = nDataType;
                etc.CloseAlarmSeconds = nCloseAlarmSeconds;
                etc.DelaySeconds = nDelaySeconds;
                etc.SiteID = nSiteID;

                return etc;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public Model.Sensor.Option.EtcData CreateOptionEtcSensorData(int nSensorType, int nAlarmDepth, int? nDataMin, float? fDataMin, string strDataMin, int? nDataMax, float? fDataMax, string strDataMax, List<int> linkedBuildingIDs, List<int> linkedZoneIDs, bool sendSDMS)
        {
            Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicFieldDatas = new Dictionary<Model.Sensor.Option.EtcData.Fields, object>();
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.SensorType] = nSensorType;
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.AlarmDepth] = nAlarmDepth;
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.DataMini] = nDataMin;
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.DataMinf] = fDataMin;
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.DataMins] = strDataMin;
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.DataMaxi] = nDataMax;
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.DataMaxf] = fDataMax;
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.DataMaxs] = strDataMax;
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.LinkedBuildingIDs] = linkedBuildingIDs == null ? null : ListToString<int>(linkedBuildingIDs);
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.LinkedZoneIDs] = linkedZoneIDs == null ? null : ListToString<int>(linkedZoneIDs);
            dicFieldDatas[Model.Sensor.Option.EtcData.Fields.SendSDMS] = sendSDMS;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                Model.Sensor.Option.EtcData.TableName,
                GetFieldNames<Model.Sensor.Option.EtcData.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                Model.Sensor.Option.EtcData data = new Model.Sensor.Option.EtcData();
                data.SensorType = nSensorType;
                data.AlarmDepth = nAlarmDepth;
                data.DataMini = nDataMin;
                data.DataMinf = fDataMin;
                data.DataMins = strDataMin;
                data.DataMaxi = nDataMax;
                data.DataMaxf = fDataMax;
                data.DataMaxs = strDataMax;
                data.LinkedBuildingIDs = linkedBuildingIDs;
                data.LinkedZoneIDs = linkedZoneIDs;
                data.SendSDMS = sendSDMS;

                return data;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public Info CreateFacilityInfo(string strModelName, string strFacilityName, int nZoneID)
        {
            Dictionary<Info.Fields, object> dicFieldDatas = new Dictionary<Info.Fields, object>();
            dicFieldDatas[Info.Fields.ModelName] = strModelName;
            dicFieldDatas[Info.Fields.FacilityName] = strFacilityName;
            dicFieldDatas[Info.Fields.ZoneID] = nZoneID;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                Info.TableName,
                GetFieldNames<Info.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Info.GetFieldName(Info.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Info> datas = m_dataManager.GetSelectManager().SelectFacilityInfos(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameFacilityInfo(datas[0], strModelName, strFacilityName, nZoneID))
                    return datas[0];

                return GetFacilityInfo(strModelName, strFacilityName, nZoneID, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Info GetFacilityInfo(string strModelName, string strFacilityName, int nZoneID, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Info.GetFieldName(Info.Fields.ID, out isNullable), id);

            List<Info> datas = m_dataManager.GetSelectManager().SelectFacilityInfos(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Info data in datas)
            {
                if (IsSameFacilityInfo(data, strModelName, strFacilityName, nZoneID))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetFacilityInfo(strModelName, strFacilityName, nZoneID, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Info.TableName);
            return null;
        }

        private bool IsSameFacilityInfo(Info data, string strModelName, string strFacilityName, int nZoneID)
        {
            if (data.ModelName == strModelName &&
                data.FacilityName == strFacilityName &&
                data.ZoneID == nZoneID)
                return true;

            return false;
        }

        public InfoData CreateFacilityInfoData(int nFacilityInfoID, int nOrderIndex, string strValue, bool withDot, int? indentDepth)
        {
            Dictionary<InfoData.Fields, object> dicFieldDatas = new Dictionary<InfoData.Fields, object>();
            dicFieldDatas[InfoData.Fields.FacilityInfoID] = nFacilityInfoID;
            dicFieldDatas[InfoData.Fields.OrderIndex] = nOrderIndex;
            dicFieldDatas[InfoData.Fields.Value] = strValue;
            dicFieldDatas[InfoData.Fields.WithDot] = withDot;
            dicFieldDatas[InfoData.Fields.IndentDepth] = indentDepth;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                InfoData.TableName,
                GetFieldNames<InfoData.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                InfoData data = new InfoData();
                data.FacilityInfoID = nFacilityInfoID;
                data.OrderIndex = nOrderIndex;
                data.Value = strValue;
                data.WithDot = withDot;
                data.IndentDepth = indentDepth;

                return data;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public BuildingData CreateBuildingData(int nBuildingID, int nOrderIndex, string strValue, bool withDot, int? indentDepth)
        {
            Dictionary<BuildingData.Fields, object> dicFieldDatas = new Dictionary<BuildingData.Fields, object>();
            dicFieldDatas[BuildingData.Fields.BuildingID] = nBuildingID;
            dicFieldDatas[BuildingData.Fields.OrderIndex] = nOrderIndex;
            dicFieldDatas[BuildingData.Fields.Value] = strValue;
            dicFieldDatas[BuildingData.Fields.WithDot] = withDot;
            dicFieldDatas[BuildingData.Fields.IndentDepth] = indentDepth;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                BuildingData.TableName,
                GetFieldNames<BuildingData.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                BuildingData data = new BuildingData();
                data.BuildingID = nBuildingID;
                data.OrderIndex = nOrderIndex;
                data.Value = strValue;
                data.WithDot = withDot;
                data.IndentDepth = indentDepth;

                return data;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public BuildingGroupData CreateBuildingGroupData(int nBuildingGroupID, int nOrderIndex, string strValue, bool withDot, int? indentDepth)
        {
            Dictionary<BuildingGroupData.Fields, object> dicFieldDatas = new Dictionary<BuildingGroupData.Fields, object>();
            dicFieldDatas[BuildingGroupData.Fields.BuildingGroupID] = nBuildingGroupID;
            dicFieldDatas[BuildingGroupData.Fields.OrderIndex] = nOrderIndex;
            dicFieldDatas[BuildingGroupData.Fields.Value] = strValue;
            dicFieldDatas[BuildingGroupData.Fields.WithDot] = withDot;
            dicFieldDatas[BuildingGroupData.Fields.IndentDepth] = indentDepth;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                BuildingGroupData.TableName,
                GetFieldNames<BuildingGroupData.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                BuildingGroupData data = new BuildingGroupData();
                data.BuildingGroupID = nBuildingGroupID;
                data.OrderIndex = nOrderIndex;
                data.Value = strValue;
                data.WithDot = withDot;
                data.IndentDepth = indentDepth;

                return data;
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        // fRotate : Radian
        public FakeWall CreateFakeWall(int nZoneID, float x, float y, float z, float fRotate, float fScale)
        {
            Dictionary<FakeWall.Fields, object> dicFieldDatas = new Dictionary<FakeWall.Fields, object>();
            dicFieldDatas[FakeWall.Fields.ZoneID] = nZoneID;
            dicFieldDatas[FakeWall.Fields.X] = x;
            dicFieldDatas[FakeWall.Fields.Y] = y;
            dicFieldDatas[FakeWall.Fields.Z] = z;
            dicFieldDatas[FakeWall.Fields.Rotate] = fRotate;
            dicFieldDatas[FakeWall.Fields.Scale] = fScale;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                FakeWall.TableName,
                GetFieldNames<FakeWall.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", FakeWall.GetFieldName(FakeWall.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<FakeWall> datas = m_dataManager.GetSelectManager().SelectFakeWalls(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameFakeWall(datas[0], nZoneID, x, y, z, fRotate, fScale))
                    return datas[0];

                return GetFakeWall(nZoneID, x, y, z, fRotate, fScale, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private FakeWall GetFakeWall(int nZoneID, float x, float y, float z, float fRotate, float fScale, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", FakeWall.GetFieldName(FakeWall.Fields.ID, out isNullable), id);

            List<FakeWall> datas = m_dataManager.GetSelectManager().SelectFakeWalls(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (FakeWall data in datas)
            {
                if (IsSameFakeWall(data, nZoneID, x, y, z, fRotate, fScale))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetFakeWall(nZoneID, x, y, z, fRotate, fScale, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(FakeWall.TableName);
            return null;
        }

        private bool IsSameFakeWall(FakeWall data, int nZoneID, float x, float y, float z, float fRotate, float fScale)
        {
            if (data.ZoneID == nZoneID &&
                IsSameFloatData2(data.X, x) &&
                IsSameFloatData2(data.Y, y) &&
                IsSameFloatData2(data.Z, z) &&
                IsSameFloatData2(data.Rotate, fRotate) &&
                IsSameFloatData2(data.Scale, fScale))
                return true;

            return false;
        }

        public Model.Config.SpreadMessage CreateSpreadMessage(int nFacilityType, int? nBuilidingGroupID, int? nBuilidingID, string strRegularID, string strRegularMemberID, int nMessageType, string strMessage)
        {
            Dictionary<Model.Config.SpreadMessage.Fields, object> dicFieldDatas = new Dictionary<Model.Config.SpreadMessage.Fields, object>();
            dicFieldDatas[Model.Config.SpreadMessage.Fields.FacilityType] = nFacilityType;
            dicFieldDatas[Model.Config.SpreadMessage.Fields.BuilidingGroupID] = nBuilidingGroupID;
            dicFieldDatas[Model.Config.SpreadMessage.Fields.BuilidingID] = nBuilidingID;
            dicFieldDatas[Model.Config.SpreadMessage.Fields.RegularID] = strRegularID;
            dicFieldDatas[Model.Config.SpreadMessage.Fields.RegularMemberID] = strRegularMemberID;
            dicFieldDatas[Model.Config.SpreadMessage.Fields.MessageType] = nMessageType;
            dicFieldDatas[Model.Config.SpreadMessage.Fields.Message] = strMessage;

            string strSQL = string.Format("Insert into {0} ({1}) values (IsNull((SELECT MAX(ID) FROM {0} C), 0) + 1, {2})",
                Model.Config.SpreadMessage.TableName,
                GetFieldNames<Model.Config.SpreadMessage.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                bool isNullable;
                string strCondition = string.Format("order by {0} desc", Model.Config.SpreadMessage.GetFieldName(Model.Config.SpreadMessage.Fields.ID, out isNullable));

                string strErrorMessage;
                // 가장 마지막에 삽입된 객체를 얻어온다.
                List<Model.Config.SpreadMessage> datas = m_dataManager.GetSelectManager().SelectSpreadMessages(null, strCondition, 1, out strErrorMessage);

                if (datas == null || datas.Count == 0)
                {
                    m_strErrorMessage = strErrorMessage;
                    return null;
                }

                if (IsSameSpreadMessage(datas[0], nFacilityType, nBuilidingGroupID, nBuilidingID, strRegularID, strRegularMemberID, nMessageType, strMessage))
                    return datas[0];

                return GetSpreadMessage(nFacilityType, nBuilidingGroupID, nBuilidingID, strRegularID, strRegularMemberID, nMessageType, strMessage, datas[0].ID, 2, FindCountLimit, out m_strErrorMessage);
            }
            else
            {
                m_strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        private Model.Config.SpreadMessage GetSpreadMessage(int nFacilityType, int? nBuilidingGroupID, int? nBuilidingID, string strRegularID, string strRegularMemberID, int nMessageType, string strMessage, int id, int nCount, int nLimit, out string strErrorMessage)
        {
            bool isNullable;
            string strCondition = string.Format("{0} < {1} order by {0} desc", Model.Config.SpreadMessage.GetFieldName(Model.Config.SpreadMessage.Fields.ID, out isNullable), id);

            List<Model.Config.SpreadMessage> datas = m_dataManager.GetSelectManager().SelectSpreadMessages(null, strCondition, nCount, out strErrorMessage);

            if (datas == null)
                return null;

            foreach (Model.Config.SpreadMessage data in datas)
            {
                if (IsSameSpreadMessage(data, nFacilityType, nBuilidingGroupID, nBuilidingID, strRegularID, strRegularMemberID, nMessageType, strMessage))
                    return data;

                if (data.ID < id)
                    id = data.ID;
            }

            if (nCount < nLimit)
                return GetSpreadMessage(nFacilityType, nBuilidingGroupID, nBuilidingID, strRegularID, strRegularMemberID, nMessageType, strMessage, id, nCount * 2, nLimit, out strErrorMessage);

            strErrorMessage = GetInsertErrorMessage(Model.Config.SpreadMessage.TableName);
            return null;
        }

        private bool IsSameSpreadMessage(Model.Config.SpreadMessage data, int nFacilityType, int? nBuilidingGroupID, int? nBuilidingID, string strRegularID, string strRegularMemberID, int nMessageType, string strMessage)
        {
            if (data.FacilityType == nFacilityType &&
                data.BuildingGroupID == nBuilidingGroupID &&
                data.BuildingID == nBuilidingID &&
                data.RegularID == strRegularID &&
                data.RegularMemberID == strRegularMemberID &&
                data.MessageType == nMessageType &&
                data.Message == strMessage)
                return true;

            return false;
        }

        public ZoneData CreateZoneData(int nZoneID, float? fakeWallElevation, float? poiElevation)
        {
            Dictionary<ZoneData.Fields, object> dicFieldDatas = new Dictionary<ZoneData.Fields, object>();
            dicFieldDatas[ZoneData.Fields.ZoneID] = nZoneID;
            dicFieldDatas[ZoneData.Fields.FakeWallElevation] = fakeWallElevation;
            dicFieldDatas[ZoneData.Fields.PoiElevation] = poiElevation;

            string strSQL = string.Format("Insert into {0} ({1}) values ({2})",
                ZoneData.TableName,
                GetFieldNames<ZoneData.Fields>(),
                GetFieldValues(dicFieldDatas));

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null)
            {
                ZoneData data = new ZoneData();
                data.ZoneID = nZoneID;
                data.FakeWallElevation = fakeWallElevation;
                data.PoiElevation = poiElevation;

                return data;
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

        private bool IsSameVertex3D(Vertex3D v1, Vertex3D v2)
        {
            if (v1 == null && v2 == null)
                return true;

            if (v1 != null && v2 != null)
            {
                if (v1.GetDistance(v2) < UnE.Geometry.Math.HALF_TOLERANCE())
                    return true;
            }

            return false;
        }

        private bool IsSameQuaternion(Quaternion q1, Quaternion q2)
        {
            if (q1 == null && q2 == null)
                return true;

            if (q1 != null && q2 != null)
            {
                double distance = System.Math.Sqrt((q1.x - q2.x) * (q1.x - q2.x) + (q1.y - q2.y) * (q1.y - q2.y) + (q1.z - q2.z) * (q1.z - q2.z) + (q1.w - q2.w) * (q1.w - q2.w));

                if (distance < UnE.Geometry.Math.HALF_TOLERANCE())
                    return true;
            }

            return false;
        }

        private bool IsSamePolygon(Polygon polygon1, Polygon polygon2)
        {
            if (polygon1 == null && polygon2 == null)
                return true;

            if (polygon1 != null && polygon2 != null)
            {
                int nVertexCount1 = polygon1.GetVertexCount();
                int nVertexCount2 = polygon2.GetVertexCount();

                if (nVertexCount1 != nVertexCount2)
                    return false;

                for (int  i=0;i<nVertexCount1;i++)
                {
                    Vertex2D v1 = polygon1.GetVertex(i);
                    Vertex2D v2 = polygon2.GetVertex(i);

                    if (v1.GetDistance(v2) >= UnE.Geometry.Math.HALF_TOLERANCE())
                        return false;
                }

                return true;
            }

            return false;
        }

        private bool IsSameList<DataType>(List<DataType> list1, List<DataType> list2)
        {
            if (list1 == null && list2 == null)
                return true;

            if (list1 != null && list2 != null)
            {
                int count1 = list1.Count;
                int count2 = list2.Count;

                if (count1 != count2)
                    return false;

                for (int i = 0; i < count1; i++)
                {
                    DataType data1 = list1[i];
                    DataType data2 = list2[i];

                    if (data1.Equals(data2) == false)
                        return false;
                }

                return true;
            }

            return false;
        }

        private bool IsSameFloatData(float? data1, float? data2)
        {
            if (data1 == null && data2 == null)
                return true;

            if (data1 != null && data2 != null)
            {
                return IsSameFloatData2((float)data1, (float)data2);
            }

            return false;
        }

        private bool IsSameFloatData2(float data1, float data2)
        {
            if (System.Math.Abs(data1 - data2) < UnE.Geometry.Math.HALF_TOLERANCE())
                return true;

            return false;
        }
    }
}
