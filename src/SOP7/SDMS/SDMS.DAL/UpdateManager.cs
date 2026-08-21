using System.Collections.Generic;
using dnsDBUtil;

namespace SDMS.DAL
{
    using IDAL;
    using SDMS.Model.Broadcast;
    using SDMS.Model.History;
    using SDMS.Model.Sensor;
    using SDMS.Model.Spatial;
    using SDMS.Model.Alarm;
    using SDMS.Model.CCTV;
    using SDMS.Model.Facility;

    public class UpdateManager : QueryManager, IUpdate
    {
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        public UpdateManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public bool UpdateZone(Zone zone, out string strErrorMessage)
        {
            Dictionary<Zone.Fields, object> dicSets = new Dictionary<Zone.Fields, object>();
            dicSets[Zone.Fields.AddFloor] = zone.AddFloor;
            dicSets[Zone.Fields.Boundary] = PolygonToString(zone.Boundary);
            dicSets[Zone.Fields.BroadcastText] = zone.BroadcastText;
            dicSets[Zone.Fields.BuildingID] = zone.BuildingID;
            dicSets[Zone.Fields.DisplayText] = zone.DisplayText;
            dicSets[Zone.Fields.FloorIndex] = zone.FloorIndex;
            dicSets[Zone.Fields.SiteID] = zone.SiteID;
            dicSets[Zone.Fields.TextCenter] = VertexToString(zone.TextCenter);
            dicSets[Zone.Fields.ZoneName] = zone.ZoneName;

            Dictionary<Zone.Fields, object> dicConditions = new Dictionary<Zone.Fields, object>();
            dicConditions[Zone.Fields.ID] = zone.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Zone.Fields>(ref strSets, dicSets, Zone.GetFieldName, Zone.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Zone.Fields>(ref strCondition, dicConditions, Zone.GetFieldName, Zone.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Zone.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateZone(Dictionary<Zone.Fields, object> dicSets, Dictionary<Zone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Zone.Fields>(ref strSets, dicSets, Zone.GetFieldName, Zone.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Zone.Fields>(ref strCondition, dicConditions, Zone.GetFieldName, Zone.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Zone.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateEquipmentZone(EquipmentZone equipZone, out string strErrorMessage)
        {
            Dictionary<EquipmentZone.Fields, object> dicSets = new Dictionary<EquipmentZone.Fields, object>();
            dicSets[EquipmentZone.Fields.Boundary] = PolygonToString(equipZone.Boundary);
            dicSets[EquipmentZone.Fields.BroadcastText] = equipZone.BroadcastText;
            dicSets[EquipmentZone.Fields.DisplayText] = equipZone.DisplayText;
            dicSets[EquipmentZone.Fields.LinkedZoneIDList] = ListToString(equipZone.LinkedZoneIDs);
            dicSets[EquipmentZone.Fields.Type] = equipZone.Type;
            dicSets[EquipmentZone.Fields.SiteID] = equipZone.SiteID;
            dicSets[EquipmentZone.Fields.TextCenter] = VertexToString(equipZone.TextCenter);
            dicSets[EquipmentZone.Fields.ZoneName] = equipZone.ZoneName;

            Dictionary<EquipmentZone.Fields, object> dicConditions = new Dictionary<EquipmentZone.Fields, object>();
            dicConditions[EquipmentZone.Fields.ID] = equipZone.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<EquipmentZone.Fields>(ref strSets, dicSets, EquipmentZone.GetFieldName, EquipmentZone.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<EquipmentZone.Fields>(ref strCondition, dicConditions, EquipmentZone.GetFieldName, EquipmentZone.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", EquipmentZone.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateEquipmentZone(Dictionary<EquipmentZone.Fields, object> dicSets, Dictionary<EquipmentZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<EquipmentZone.Fields>(ref strSets, dicSets, EquipmentZone.GetFieldName, EquipmentZone.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<EquipmentZone.Fields>(ref strCondition, dicConditions, EquipmentZone.GetFieldName, EquipmentZone.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", EquipmentZone.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBuildingGroup(BuildingGroup buildingGroup, out string strErrorMessage)
        {
            Dictionary<BuildingGroup.Fields, object> dicSets = new Dictionary<BuildingGroup.Fields, object>();
            dicSets[BuildingGroup.Fields.DisplayText] = buildingGroup.DisplayText;
            dicSets[BuildingGroup.Fields.GroupName] = buildingGroup.GroupName;
            dicSets[BuildingGroup.Fields.ParentID] = buildingGroup.ParentID;
            dicSets[BuildingGroup.Fields.SiteID] = buildingGroup.SiteID;
            dicSets[BuildingGroup.Fields.TextCenter] = VertexToString(buildingGroup.TextCenter);

            Dictionary<BuildingGroup.Fields, object> dicConditions = new Dictionary<BuildingGroup.Fields, object>();
            dicConditions[BuildingGroup.Fields.ID] = buildingGroup.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<BuildingGroup.Fields>(ref strSets, dicSets, BuildingGroup.GetFieldName, BuildingGroup.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<BuildingGroup.Fields>(ref strCondition, dicConditions, BuildingGroup.GetFieldName, BuildingGroup.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", BuildingGroup.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBuildingGroup(Dictionary<BuildingGroup.Fields, object> dicSets, Dictionary<BuildingGroup.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<BuildingGroup.Fields>(ref strSets, dicSets, BuildingGroup.GetFieldName, BuildingGroup.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<BuildingGroup.Fields>(ref strCondition, dicConditions, BuildingGroup.GetFieldName, BuildingGroup.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", BuildingGroup.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBuilding(Building building, out string strErrorMessage)
        {
            Dictionary<Building.Fields, object> dicSets = new Dictionary<Building.Fields, object>();
            dicSets[Building.Fields.BroadcastText] = building.BroadcastText;
            dicSets[Building.Fields.BuildingCode] = building.BuildingCode;
            dicSets[Building.Fields.BuildingGroupID] = building.BuildingGroupID;
            dicSets[Building.Fields.BuildingName] = building.BuildingName;
            dicSets[Building.Fields.DisplayText] = building.DisplayText;
            dicSets[Building.Fields.MaxFloor] = building.MaxFloor;
            dicSets[Building.Fields.MinFloor] = building.MinFloor;
            dicSets[Building.Fields.TextCenter] = VertexToString(building.TextCenter);

            Dictionary<Building.Fields, object> dicConditions = new Dictionary<Building.Fields, object>();
            dicConditions[Building.Fields.ID] = building.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Building.Fields>(ref strSets, dicSets, Building.GetFieldName, Building.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Building.Fields>(ref strCondition, dicConditions, Building.GetFieldName, Building.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Building.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBuilding(Dictionary<Building.Fields, object> dicSets, Dictionary<Building.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Building.Fields>(ref strSets, dicSets, Building.GetFieldName, Building.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Building.Fields>(ref strCondition, dicConditions, Building.GetFieldName, Building.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Building.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSensorZone(SensorZone sensorZone, out string strErrorMessage)
        {
            Dictionary<SensorZone.Fields, object> dicSets = new Dictionary<SensorZone.Fields, object>();
            dicSets[SensorZone.Fields.EquipZoneID] = sensorZone.EquipZoneID;
            dicSets[SensorZone.Fields.OrgSensorID] = sensorZone.OrgSensorID;
            dicSets[SensorZone.Fields.Data] = sensorZone.Data;

            Dictionary<SensorZone.Fields, object> dicConditions = new Dictionary<SensorZone.Fields, object>();
            dicConditions[SensorZone.Fields.ID] = sensorZone.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SensorZone.Fields>(ref strSets, dicSets, SensorZone.GetFieldName, SensorZone.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SensorZone.Fields>(ref strCondition, dicConditions, SensorZone.GetFieldName, SensorZone.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", SensorZone.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSensorZone(Dictionary<SensorZone.Fields, object> dicSets, Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SensorZone.Fields>(ref strSets, dicSets, SensorZone.GetFieldName, SensorZone.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SensorZone.Fields>(ref strCondition, dicConditions, SensorZone.GetFieldName, SensorZone.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", SensorZone.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage + " 쿼리 실패 / " + m_dbManager.WebServerURL;
                return false;
            }

            return true;
        }

        public bool UpdatePSMSensor(PSM sensor, out string strErrorMessage)
        {
            Dictionary<PSM.Fields, object> dicSets = new Dictionary<PSM.Fields, object>();
            dicSets[PSM.Fields.CurrentData] = sensor.CurrentData;
            dicSets[PSM.Fields.Department] = sensor.Department;
            dicSets[PSM.Fields.DepartmentPhoneNumber] = sensor.DepartmentPhoneNumber;
            dicSets[PSM.Fields.LimitLevel1] = sensor.LimitLevel1;
            dicSets[PSM.Fields.LimitLevel2] = sensor.LimitLevel2;
            dicSets[PSM.Fields.LimitLevel3] = sensor.LimitLevel3;
            dicSets[PSM.Fields.Name] = sensor.Name;
            dicSets[PSM.Fields.PositionName] = sensor.PositionName;
            dicSets[PSM.Fields.UseLimitLevel1] = sensor.UseLimitLevel1;
            dicSets[PSM.Fields.UseLimitLevel2] = sensor.UseLimitLevel2;
            dicSets[PSM.Fields.UseLimitLevel3] = sensor.UseLimitLevel3;
            dicSets[PSM.Fields.X] = sensor.X;
            dicSets[PSM.Fields.Y] = sensor.Y;
            dicSets[PSM.Fields.Z] = sensor.Z;
            dicSets[PSM.Fields.ZoneID] = sensor.ZoneID;
            dicSets[PSM.Fields.EquipZoneID] = sensor.EquipZoneID;
            dicSets[PSM.Fields.Status] = sensor.Status;
            dicSets[PSM.Fields.UniqueKey] = sensor.UniqueKey;
            dicSets[PSM.Fields.MaterialType] = sensor.MaterialType;

            Dictionary<PSM.Fields, object> dicConditions = new Dictionary<PSM.Fields, object>();
            dicConditions[PSM.Fields.ID] = sensor.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<PSM.Fields>(ref strSets, dicSets, PSM.GetFieldName, PSM.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<PSM.Fields>(ref strCondition, dicConditions, PSM.GetFieldName, PSM.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", PSM.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdatePSMSensor(Dictionary<PSM.Fields, object> dicSets, Dictionary<PSM.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<PSM.Fields>(ref strSets, dicSets, PSM.GetFieldName, PSM.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<PSM.Fields>(ref strCondition, dicConditions, PSM.GetFieldName, PSM.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", PSM.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateMaterial(Material material, out string strErrorMessage)
        {
            Dictionary<Material.Fields, object> dicSets = new Dictionary<Material.Fields, object>();
            dicSets[Material.Fields.MaterialName] = material.MaterialName;
            dicSets[Material.Fields.SiteID] = material.SiteID;
            dicSets[Material.Fields.UOM] = material.UOM;
            dicSets[Material.Fields.Description] = material.Description;

            Dictionary<Material.Fields, object> dicConditions = new Dictionary<Material.Fields, object>();
            dicConditions[Material.Fields.ID] = material.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Material.Fields>(ref strSets, dicSets, Material.GetFieldName, Material.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Material.Fields>(ref strCondition, dicConditions, Material.GetFieldName, Material.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Material.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateMaterial(Dictionary<Material.Fields, object> dicSets, Dictionary<Material.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Material.Fields>(ref strSets, dicSets, Material.GetFieldName, Material.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Material.Fields>(ref strCondition, dicConditions, Material.GetFieldName, Material.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Material.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFireSensor(Fire sensor, out string strErrorMessage)
        {
            Dictionary<Fire.Fields, object> dicSets = new Dictionary<Fire.Fields, object>();
            dicSets[Fire.Fields.Department] = sensor.Department;
            dicSets[Fire.Fields.DepartmentPhoneNumber] = sensor.DepartmentPhoneNumber;
            dicSets[Fire.Fields.Name] = sensor.Name;
            dicSets[Fire.Fields.PositionName] = sensor.PositionName;
            dicSets[Fire.Fields.X] = sensor.X;
            dicSets[Fire.Fields.Y] = sensor.Y;
            dicSets[Fire.Fields.Z] = sensor.Z;
            dicSets[Fire.Fields.ZoneID] = sensor.ZoneID;

            Dictionary<Fire.Fields, object> dicConditions = new Dictionary<Fire.Fields, object>();
            dicConditions[Fire.Fields.ID] = sensor.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Fire.Fields>(ref strSets, dicSets, Fire.GetFieldName, Fire.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Fire.Fields>(ref strCondition, dicConditions, Fire.GetFieldName, Fire.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Fire.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFireSensor(Dictionary<Fire.Fields, object> dicSets, Dictionary<Fire.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Fire.Fields>(ref strSets, dicSets, Fire.GetFieldName, Fire.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Fire.Fields>(ref strCondition, dicConditions, Fire.GetFieldName, Fire.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Fire.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFacilityType(FacilityType type, out string strErrorMessage)
        {
            Dictionary<FacilityType.Fields, object> dicSets = new Dictionary<FacilityType.Fields, object>();
            dicSets[FacilityType.Fields.Description] = type.Description;
            dicSets[FacilityType.Fields.LinkedTableName] = type.LinkedTableName;
            dicSets[FacilityType.Fields.SiteID] = type.SiteID;
            dicSets[FacilityType.Fields.TypeName] = type.TypeName;
            dicSets[FacilityType.Fields.DisasterCategoryID] = type.DisasterCategoryID;
            dicSets[FacilityType.Fields.SubDisasterCategoryID] = type.SubDisasterCategoryID;

            Dictionary<FacilityType.Fields, object> dicConditions = new Dictionary<FacilityType.Fields, object>();
            dicConditions[FacilityType.Fields.ID] = type.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<FacilityType.Fields>(ref strSets, dicSets, FacilityType.GetFieldName, FacilityType.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<FacilityType.Fields>(ref strCondition, dicConditions, FacilityType.GetFieldName, FacilityType.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", FacilityType.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFacilityType(Dictionary<FacilityType.Fields, object> dicSets, Dictionary<FacilityType.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<FacilityType.Fields>(ref strSets, dicSets, FacilityType.GetFieldName, FacilityType.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<FacilityType.Fields>(ref strCondition, dicConditions, FacilityType.GetFieldName, FacilityType.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", FacilityType.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSensorZoneHistory(SensorZoneHistory history, out string strErrorMessage)
        {
            Dictionary<SensorZoneHistory.Fields, object> dicSets = new Dictionary<SensorZoneHistory.Fields, object>();
            dicSets[SensorZoneHistory.Fields.Data] = history.Data;
            dicSets[SensorZoneHistory.Fields.DetectionStatus] = (int)history.DetectionStatus;
            dicSets[SensorZoneHistory.Fields.SiteID] = history.SiteID;
            dicSets[SensorZoneHistory.Fields.SensorZoneID] = history.SensorZoneID;
            dicSets[SensorZoneHistory.Fields.SensorType] = history.SensorType;
            dicSets[SensorZoneHistory.Fields.Time] = history.Time;
            dicSets[SensorZoneHistory.Fields.ZoneID] = history.ZoneID;
            dicSets[SensorZoneHistory.Fields.Memo] = history.Memo;

            Dictionary<SensorZoneHistory.Fields, object> dicConditions = new Dictionary<SensorZoneHistory.Fields, object>();
            dicConditions[SensorZoneHistory.Fields.ID] = history.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SensorZoneHistory.Fields>(ref strSets, dicSets, SensorZoneHistory.GetFieldName, SensorZoneHistory.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SensorZoneHistory.Fields>(ref strCondition, dicConditions, SensorZoneHistory.GetFieldName, SensorZoneHistory.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", SensorZoneHistory.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSensorZoneHistory(Dictionary<SensorZoneHistory.Fields, object> dicSets, Dictionary<SensorZoneHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            object data;

            if (dicSets.TryGetValue(SensorZoneHistory.Fields.AllSensorZoneIDs, out data))
            {
                if (data != null && data is List<int>)
                {
                    dicSets[SensorZoneHistory.Fields.AllSensorZoneIDs] = ListToString((List<int>)data);
                }
            }

            if (dicConditions.TryGetValue(SensorZoneHistory.Fields.AllSensorZoneIDs, out data))
            {
                if (data != null && data is List<int>)
                {
                    dicConditions[SensorZoneHistory.Fields.AllSensorZoneIDs] = ListToString((List<int>)data);
                }
            }

            if (SetData<SensorZoneHistory.Fields>(ref strSets, dicSets, SensorZoneHistory.GetFieldName, SensorZoneHistory.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SensorZoneHistory.Fields>(ref strCondition, dicConditions, SensorZoneHistory.GetFieldName, SensorZoneHistory.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", SensorZoneHistory.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSensorReactionHistory(SensorReactionHistory history, out string strErrorMessage)
        {
            Dictionary<SensorReactionHistory.Fields, object> dicSets = new Dictionary<SensorReactionHistory.Fields, object>();
            dicSets[SensorReactionHistory.Fields.Message] = history.Message;
            dicSets[SensorReactionHistory.Fields.Param1] = history.Param1;
            dicSets[SensorReactionHistory.Fields.Param2] = history.Param2;
            dicSets[SensorReactionHistory.Fields.Param3] = history.Param3;
            dicSets[SensorReactionHistory.Fields.ReactionType] = (int)history.ReactionType;
            dicSets[SensorReactionHistory.Fields.Time] = history.Time;
            dicSets[SensorReactionHistory.Fields.SensorZoneHistoryID] = history.SensorZoneHistoryID;

            Dictionary<SensorReactionHistory.Fields, object> dicConditions = new Dictionary<SensorReactionHistory.Fields, object>();
            dicConditions[SensorReactionHistory.Fields.ID] = history.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SensorReactionHistory.Fields>(ref strSets, dicSets, SensorReactionHistory.GetFieldName, SensorReactionHistory.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SensorReactionHistory.Fields>(ref strCondition, dicConditions, SensorReactionHistory.GetFieldName, SensorReactionHistory.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", SensorReactionHistory.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSensorReactionHistory(Dictionary<SensorReactionHistory.Fields, object> dicSets, Dictionary<SensorReactionHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SensorReactionHistory.Fields>(ref strSets, dicSets, SensorReactionHistory.GetFieldName, SensorReactionHistory.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SensorReactionHistory.Fields>(ref strCondition, dicConditions, SensorReactionHistory.GetFieldName, SensorReactionHistory.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", SensorReactionHistory.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSensorReactionHistoryDescription(SensorReactionHistoryDescription description, out string strErrorMessage)
        {
            Dictionary<SensorReactionHistoryDescription.Fields, object> dicSets = new Dictionary<SensorReactionHistoryDescription.Fields, object>();
            dicSets[SensorReactionHistoryDescription.Fields.SensorReactionHistoryID] = description.SensorReactionHistoryID;
            dicSets[SensorReactionHistoryDescription.Fields.DescriptionID] = description.DescriptionID;
            dicSets[SensorReactionHistoryDescription.Fields.SensorZoneHistoryID] = description.SensorZoneHistoryID;

            Dictionary<SensorReactionHistoryDescription.Fields, object> dicConditions = new Dictionary<SensorReactionHistoryDescription.Fields, object>();
            dicConditions[SensorReactionHistoryDescription.Fields.ID] = description.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SensorReactionHistoryDescription.Fields>(ref strSets, dicSets, SensorReactionHistoryDescription.GetFieldName, SensorReactionHistoryDescription.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SensorReactionHistoryDescription.Fields>(ref strCondition, dicConditions, SensorReactionHistoryDescription.GetFieldName, SensorReactionHistoryDescription.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", SensorReactionHistoryDescription.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSensorReactionHistoryDescription(Dictionary<SensorReactionHistoryDescription.Fields, object> dicSets, Dictionary<SensorReactionHistoryDescription.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SensorReactionHistoryDescription.Fields>(ref strSets, dicSets, SensorReactionHistoryDescription.GetFieldName, SensorReactionHistoryDescription.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SensorReactionHistoryDescription.Fields>(ref strCondition, dicConditions, SensorReactionHistoryDescription.GetFieldName, SensorReactionHistoryDescription.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", SensorReactionHistoryDescription.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSensorReactionHistoryDescriptionText(SensorReactionHistoryDescriptionText text, out string strErrorMessage)
        {
            Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicSets = new Dictionary<SensorReactionHistoryDescriptionText.Fields, object>();
            dicSets[SensorReactionHistoryDescriptionText.Fields.RefCount] = text.RefCount;
            dicSets[SensorReactionHistoryDescriptionText.Fields.Description] = text.Description;

            Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicConditions = new Dictionary<SensorReactionHistoryDescriptionText.Fields, object>();
            dicConditions[SensorReactionHistoryDescriptionText.Fields.ID] = text.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SensorReactionHistoryDescriptionText.Fields>(ref strSets, dicSets, SensorReactionHistoryDescriptionText.GetFieldName, SensorReactionHistoryDescriptionText.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SensorReactionHistoryDescriptionText.Fields>(ref strCondition, dicConditions, SensorReactionHistoryDescriptionText.GetFieldName, SensorReactionHistoryDescriptionText.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", SensorReactionHistoryDescriptionText.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSensorReactionHistoryDescriptionText(Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicSets, Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SensorReactionHistoryDescriptionText.Fields>(ref strSets, dicSets, SensorReactionHistoryDescriptionText.GetFieldName, SensorReactionHistoryDescriptionText.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SensorReactionHistoryDescriptionText.Fields>(ref strCondition, dicConditions, SensorReactionHistoryDescriptionText.GetFieldName, SensorReactionHistoryDescriptionText.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", SensorReactionHistoryDescriptionText.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBroadcast(Broadcast broadcast, out string strErrorMessage)
        {
            Dictionary<Broadcast.Fields, object> dicSets = new Dictionary<Broadcast.Fields, object>();
            dicSets[Broadcast.Fields.Text] = broadcast.Text;
            dicSets[Broadcast.Fields.UseSiren] = broadcast.UseSiren;
            dicSets[Broadcast.Fields.PlayOption] = broadcast.PlayOption;
            dicSets[Broadcast.Fields.RepeatCount] = broadcast.RepeatCount;
            dicSets[Broadcast.Fields.RequestTime] = broadcast.RequestTime;
            dicSets[Broadcast.Fields.SiteID] = broadcast.SiteID;

            Dictionary<Broadcast.Fields, object> dicConditions = new Dictionary<Broadcast.Fields, object>();
            dicConditions[Broadcast.Fields.ID] = broadcast.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Broadcast.Fields>(ref strSets, dicSets, Broadcast.GetFieldName, Broadcast.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Broadcast.Fields>(ref strCondition, dicConditions, Broadcast.GetFieldName, Broadcast.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Broadcast.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBroadcast(Dictionary<Broadcast.Fields, object> dicSets, Dictionary<Broadcast.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Broadcast.Fields>(ref strSets, dicSets, Broadcast.GetFieldName, Broadcast.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Broadcast.Fields>(ref strCondition, dicConditions, Broadcast.GetFieldName, Broadcast.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Broadcast.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBroadcastHistory(Model.Broadcast.History history, out string strErrorMessage)
        {
            Dictionary<Model.Broadcast.History.Fields, object> dicSets = new Dictionary<Model.Broadcast.History.Fields, object>();
            dicSets[Model.Broadcast.History.Fields.Text] = history.Text;
            dicSets[Model.Broadcast.History.Fields.UseSiren] = history.UseSiren;
            dicSets[Model.Broadcast.History.Fields.PlayOption] = history.PlayOption;
            dicSets[Model.Broadcast.History.Fields.RepeatCount] = history.RepeatCount;
            dicSets[Model.Broadcast.History.Fields.RequestTime] = history.RequestTime;
            dicSets[Model.Broadcast.History.Fields.ExecuteTime] = history.ExecuteTime;
            dicSets[Model.Broadcast.History.Fields.SiteID] = history.SiteID;

            Dictionary<Model.Broadcast.History.Fields, object> dicConditions = new Dictionary<Model.Broadcast.History.Fields, object>();
            dicConditions[Model.Broadcast.History.Fields.ID] = history.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Broadcast.History.Fields>(ref strSets, dicSets, Model.Broadcast.History.GetFieldName, Model.Broadcast.History.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Broadcast.History.Fields>(ref strCondition, dicConditions, Model.Broadcast.History.GetFieldName, Model.Broadcast.History.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Broadcast.History.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBroadcastHistory(Dictionary<Model.Broadcast.History.Fields, object> dicSets, Dictionary<Model.Broadcast.History.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Broadcast.History.Fields>(ref strSets, dicSets, Model.Broadcast.History.GetFieldName, Model.Broadcast.History.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Broadcast.History.Fields>(ref strCondition, dicConditions, Model.Broadcast.History.GetFieldName, Model.Broadcast.History.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Broadcast.History.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBroadcastState(Model.Broadcast.State state, out string strErrorMessage)
        {
            Dictionary<Model.Broadcast.State.Fields, object> dicSets = new Dictionary<Model.Broadcast.State.Fields, object>();
            dicSets[Model.Broadcast.State.Fields.HeartBeat] = state.HeartBeat;
            dicSets[Model.Broadcast.State.Fields.BState] = state.BState;
            dicSets[Model.Broadcast.State.Fields.SiteID] = state.SiteID;

            Dictionary<Model.Broadcast.State.Fields, object> dicConditions = new Dictionary<Model.Broadcast.State.Fields, object>();
            dicConditions[Model.Broadcast.State.Fields.ID] = state.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Broadcast.State.Fields>(ref strSets, dicSets, Model.Broadcast.State.GetFieldName, Model.Broadcast.State.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Broadcast.State.Fields>(ref strCondition, dicConditions, Model.Broadcast.State.GetFieldName, Model.Broadcast.State.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Broadcast.State.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBroadcastState(Dictionary<Model.Broadcast.State.Fields, object> dicSets, Dictionary<Model.Broadcast.State.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Broadcast.State.Fields>(ref strSets, dicSets, Model.Broadcast.State.GetFieldName, Model.Broadcast.State.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Broadcast.State.Fields>(ref strCondition, dicConditions, Model.Broadcast.State.GetFieldName, Model.Broadcast.State.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Broadcast.State.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSMSHistory(SMSHistory history, out string strErrorMessage)
        {
            Dictionary<SMSHistory.Fields, object> dicSets = new Dictionary<SMSHistory.Fields, object>();
            dicSets[SMSHistory.Fields.SensorZoneHistoryID] = history.SensorZoneHistoryID;
            dicSets[SMSHistory.Fields.SensorReactionHistoryID] = history.SensorReactionHistoryID;
            dicSets[SMSHistory.Fields.RegularMemberIDList] = history.RegularMemberIDList == null ? null : ListToString(history.RegularMemberIDList);
            dicSets[SMSHistory.Fields.SMSMessage] = history.SMSMessage;
            dicSets[SMSHistory.Fields.SendType] = history.SendType;

            Dictionary<SMSHistory.Fields, object> dicConditions = new Dictionary<SMSHistory.Fields, object>();
            dicConditions[SMSHistory.Fields.ID] = history.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SMSHistory.Fields>(ref strSets, dicSets, SMSHistory.GetFieldName, SMSHistory.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SMSHistory.Fields>(ref strCondition, dicConditions, SMSHistory.GetFieldName, SMSHistory.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", SMSHistory.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSMSHIstory(Dictionary<SMSHistory.Fields, object> dicSets, Dictionary<SMSHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<SMSHistory.Fields>(ref strSets, dicSets, SMSHistory.GetFieldName, SMSHistory.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<SMSHistory.Fields>(ref strCondition, dicConditions, SMSHistory.GetFieldName, SMSHistory.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", SMSHistory.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBroadcastConfig(Model.Config.Broadcast config, out string strErrorMessage)
        {
            Dictionary<Model.Config.Broadcast.Fields, object> dicSets = new Dictionary<Model.Config.Broadcast.Fields, object>();
            dicSets[Model.Config.Broadcast.Fields.SituationType] = config.SituationType;
            dicSets[Model.Config.Broadcast.Fields.UseBroadcast] = config.UseBroadcast;
            dicSets[Model.Config.Broadcast.Fields.Message] = config.Message;
            dicSets[Model.Config.Broadcast.Fields.RepeatCount] = config.RepeatCount;
            dicSets[Model.Config.Broadcast.Fields.UseSiren] = config.UseSiren;
            dicSets[Model.Config.Broadcast.Fields.Description] = config.Description;
            dicSets[Model.Config.Broadcast.Fields.SiteID] = config.SiteID;

            Dictionary<Model.Config.Broadcast.Fields, object> dicConditions = new Dictionary<Model.Config.Broadcast.Fields, object>();
            dicConditions[Model.Config.Broadcast.Fields.ID] = config.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Config.Broadcast.Fields>(ref strSets, dicSets, Model.Config.Broadcast.GetFieldName, Model.Config.Broadcast.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Config.Broadcast.Fields>(ref strCondition, dicConditions, Model.Config.Broadcast.GetFieldName, Model.Config.Broadcast.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Config.Broadcast.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBroadcastConfig(Dictionary<Model.Config.Broadcast.Fields, object> dicSets, Dictionary<Model.Config.Broadcast.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Config.Broadcast.Fields>(ref strSets, dicSets, Model.Config.Broadcast.GetFieldName, Model.Config.Broadcast.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Config.Broadcast.Fields>(ref strCondition, dicConditions, Model.Config.Broadcast.GetFieldName, Model.Config.Broadcast.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Config.Broadcast.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSMSConfig(Model.Config.SMS config, out string strErrorMessage)
        {
            Dictionary<Model.Config.SMS.Fields, object> dicSets = new Dictionary<Model.Config.SMS.Fields, object>();
            dicSets[Model.Config.SMS.Fields.MessageType] = config.MessageType;
            dicSets[Model.Config.SMS.Fields.UseSMS] = config.UseSMS;
            dicSets[Model.Config.SMS.Fields.Description] = config.Description;
            dicSets[Model.Config.SMS.Fields.SiteID] = config.SiteID;

            Dictionary<Model.Config.SMS.Fields, object> dicConditions = new Dictionary<Model.Config.SMS.Fields, object>();
            dicConditions[Model.Config.SMS.Fields.ID] = config.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Config.SMS.Fields>(ref strSets, dicSets, Model.Config.SMS.GetFieldName, Model.Config.SMS.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Config.SMS.Fields>(ref strCondition, dicConditions, Model.Config.SMS.GetFieldName, Model.Config.SMS.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Config.SMS.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSMSConfig(Dictionary<Model.Config.SMS.Fields, object> dicSets, Dictionary<Model.Config.SMS.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Config.SMS.Fields>(ref strSets, dicSets, Model.Config.SMS.GetFieldName, Model.Config.SMS.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Config.SMS.Fields>(ref strCondition, dicConditions, Model.Config.SMS.GetFieldName, Model.Config.SMS.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Config.SMS.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateCurrentAlarm(Dictionary<CurrentAlarm.Fields, object> dicSets, Dictionary<CurrentAlarm.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<CurrentAlarm.Fields>(ref strSets, dicSets, CurrentAlarm.GetFieldName, CurrentAlarm.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<CurrentAlarm.Fields>(ref strCondition, dicConditions, CurrentAlarm.GetFieldName, CurrentAlarm.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", CurrentAlarm.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFacilityManager(FacilityManager manager, out string strErrorMessage)
        {
            Dictionary<FacilityManager.Fields, object> dicSets = new Dictionary<FacilityManager.Fields, object>();
            dicSets[FacilityManager.Fields.MemberID] = manager.MemberID;
            dicSets[FacilityManager.Fields.MemberType] = manager.MemberType;
            dicSets[FacilityManager.Fields.FacilityType] = manager.FacilityType;
            dicSets[FacilityManager.Fields.DetectType] = manager.DetectType;
            dicSets[FacilityManager.Fields.SiteID] = manager.SiteID;
            dicSets[FacilityManager.Fields.Description] = manager.Description;

            Dictionary<FacilityManager.Fields, object> dicConditions = new Dictionary<FacilityManager.Fields, object>();
            dicConditions[FacilityManager.Fields.ID] = manager.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<FacilityManager.Fields>(ref strSets, dicSets, FacilityManager.GetFieldName, FacilityManager.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<FacilityManager.Fields>(ref strCondition, dicConditions, FacilityManager.GetFieldName, FacilityManager.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", FacilityManager.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFacilityManager(Dictionary<FacilityManager.Fields, object> dicSets, Dictionary<FacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<FacilityManager.Fields>(ref strSets, dicSets, FacilityManager.GetFieldName, FacilityManager.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<FacilityManager.Fields>(ref strCondition, dicConditions, FacilityManager.GetFieldName, FacilityManager.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", FacilityManager.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBuildingFacilityManager(BuildingFacilityManager manager, out string strErrorMessage)
        {
            Dictionary<BuildingFacilityManager.Fields, object> dicSets = new Dictionary<BuildingFacilityManager.Fields, object>();
            dicSets[BuildingFacilityManager.Fields.MemberID] = manager.MemberID;
            dicSets[BuildingFacilityManager.Fields.MemberType] = manager.MemberType;
            dicSets[BuildingFacilityManager.Fields.FacilityType] = manager.FacilityType;
            dicSets[BuildingFacilityManager.Fields.DetectType] = manager.DetectType;
            dicSets[BuildingFacilityManager.Fields.BuildingID] = manager.BuildingID;
            dicSets[BuildingFacilityManager.Fields.SiteID] = manager.SiteID;
            dicSets[BuildingFacilityManager.Fields.Description] = manager.Description;

            Dictionary<BuildingFacilityManager.Fields, object> dicConditions = new Dictionary<BuildingFacilityManager.Fields, object>();
            dicConditions[BuildingFacilityManager.Fields.ID] = manager.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<BuildingFacilityManager.Fields>(ref strSets, dicSets, BuildingFacilityManager.GetFieldName, BuildingFacilityManager.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<BuildingFacilityManager.Fields>(ref strCondition, dicConditions, BuildingFacilityManager.GetFieldName, BuildingFacilityManager.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", BuildingFacilityManager.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBuildingFacilityManager(Dictionary<BuildingFacilityManager.Fields, object> dicSets, Dictionary<BuildingFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<BuildingFacilityManager.Fields>(ref strSets, dicSets, BuildingFacilityManager.GetFieldName, BuildingFacilityManager.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<BuildingFacilityManager.Fields>(ref strCondition, dicConditions, BuildingFacilityManager.GetFieldName, BuildingFacilityManager.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", BuildingFacilityManager.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateEquipZoneFacilityManager(EquipZoneFacilityManager manager, out string strErrorMessage)
        {
            Dictionary<EquipZoneFacilityManager.Fields, object> dicSets = new Dictionary<EquipZoneFacilityManager.Fields, object>();
            dicSets[EquipZoneFacilityManager.Fields.MemberID] = manager.MemberID;
            dicSets[EquipZoneFacilityManager.Fields.MemberType] = manager.MemberType;
            dicSets[EquipZoneFacilityManager.Fields.FacilityType] = manager.FacilityType;
            dicSets[EquipZoneFacilityManager.Fields.DetectType] = manager.DetectType;
            dicSets[EquipZoneFacilityManager.Fields.EquipZoneID] = manager.EquipZoneID;
            dicSets[EquipZoneFacilityManager.Fields.SiteID] = manager.SiteID;
            dicSets[EquipZoneFacilityManager.Fields.Description] = manager.Description;

            Dictionary<EquipZoneFacilityManager.Fields, object> dicConditions = new Dictionary<EquipZoneFacilityManager.Fields, object>();
            dicConditions[EquipZoneFacilityManager.Fields.ID] = manager.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<EquipZoneFacilityManager.Fields>(ref strSets, dicSets, EquipZoneFacilityManager.GetFieldName, EquipZoneFacilityManager.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<EquipZoneFacilityManager.Fields>(ref strCondition, dicConditions, EquipZoneFacilityManager.GetFieldName, EquipZoneFacilityManager.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", EquipZoneFacilityManager.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateEquipZoneFacilityManager(Dictionary<EquipZoneFacilityManager.Fields, object> dicSets, Dictionary<EquipZoneFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<EquipZoneFacilityManager.Fields>(ref strSets, dicSets, EquipZoneFacilityManager.GetFieldName, EquipZoneFacilityManager.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<EquipZoneFacilityManager.Fields>(ref strCondition, dicConditions, EquipZoneFacilityManager.GetFieldName, EquipZoneFacilityManager.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", EquipZoneFacilityManager.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateCCTV(CCTV cctv, out string strErrorMessage)
        {
            Dictionary<CCTV.Fields, object> dicSets = new Dictionary<CCTV.Fields, object>();
            dicSets[CCTV.Fields.CameraName] = cctv.CameraName;
            dicSets[CCTV.Fields.PositionName] = cctv.PositionName;
            dicSets[CCTV.Fields.UniqueKey] = cctv.UniqueKey;
            dicSets[CCTV.Fields.X] = cctv.X;
            dicSets[CCTV.Fields.Y] = cctv.Y;
            dicSets[CCTV.Fields.Z] = cctv.Z;
            dicSets[CCTV.Fields.ZoneID] = cctv.ZoneID;
            dicSets[CCTV.Fields.IsIndoor] = cctv.IsIndoor;
            dicSets[CCTV.Fields.Type] = cctv.Type;
            dicSets[CCTV.Fields.Channel] = cctv.Channel;
            dicSets[CCTV.Fields.UserID] = cctv.UserID;
            dicSets[CCTV.Fields.Password] = cctv.Password;
            dicSets[CCTV.Fields.URL] = cctv.URL;
            dicSets[CCTV.Fields.BigURL] = cctv.BigURL;
            dicSets[CCTV.Fields.SmallURL] = cctv.SmallURL;
            dicSets[CCTV.Fields.Enabled] = cctv.Enabled;
            dicSets[CCTV.Fields.CameraIP] = cctv.CameraIP;
            dicSets[CCTV.Fields.CameraCompanyName] = cctv.CameraCompanyName;
            dicSets[CCTV.Fields.CameraModelName] = cctv.CameraModelName;
            dicSets[CCTV.Fields.Description] = cctv.Description;

            Dictionary<CCTV.Fields, object> dicConditions = new Dictionary<CCTV.Fields, object>();
            dicConditions[CCTV.Fields.ID] = cctv.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<CCTV.Fields>(ref strSets, dicSets, CCTV.GetFieldName, CCTV.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<CCTV.Fields>(ref strCondition, dicConditions, CCTV.GetFieldName, CCTV.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", CCTV.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateCCTV(Dictionary<CCTV.Fields, object> dicSets, Dictionary<CCTV.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<CCTV.Fields>(ref strSets, dicSets, CCTV.GetFieldName, CCTV.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<CCTV.Fields>(ref strCondition, dicConditions, CCTV.GetFieldName, CCTV.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", CCTV.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateEquipZoneCCTV(EquipZoneCCTV cctv, out string strErrorMessage)
        {
            Dictionary<EquipZoneCCTV.Fields, object> dicSets = new Dictionary<EquipZoneCCTV.Fields, object>();
            dicSets[EquipZoneCCTV.Fields.EquipZoneID] = cctv.EquipZoneID;
            dicSets[EquipZoneCCTV.Fields.CCTV1] = cctv.CCTV1;
            dicSets[EquipZoneCCTV.Fields.CCTV2] = cctv.CCTV2;
            dicSets[EquipZoneCCTV.Fields.CCTV3] = cctv.CCTV3;
            dicSets[EquipZoneCCTV.Fields.CCTV4] = cctv.CCTV4;
            dicSets[EquipZoneCCTV.Fields.CCTV5] = cctv.CCTV5;
            dicSets[EquipZoneCCTV.Fields.CCTV6] = cctv.CCTV6;
            dicSets[EquipZoneCCTV.Fields.Preset1] = cctv.Preset1;
            dicSets[EquipZoneCCTV.Fields.Preset2] = cctv.Preset2;
            dicSets[EquipZoneCCTV.Fields.Preset3] = cctv.Preset3;
            dicSets[EquipZoneCCTV.Fields.Preset4] = cctv.Preset4;
            dicSets[EquipZoneCCTV.Fields.Preset5] = cctv.Preset5;
            dicSets[EquipZoneCCTV.Fields.Preset6] = cctv.Preset6;
            dicSets[EquipZoneCCTV.Fields.Description] = cctv.Description;

            Dictionary<EquipZoneCCTV.Fields, object> dicConditions = new Dictionary<EquipZoneCCTV.Fields, object>();
            dicConditions[EquipZoneCCTV.Fields.ID] = cctv.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<EquipZoneCCTV.Fields>(ref strSets, dicSets, EquipZoneCCTV.GetFieldName, EquipZoneCCTV.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<EquipZoneCCTV.Fields>(ref strCondition, dicConditions, EquipZoneCCTV.GetFieldName, EquipZoneCCTV.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", EquipZoneCCTV.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateEquipZoneCCTV(Dictionary<EquipZoneCCTV.Fields, object> dicSets, Dictionary<EquipZoneCCTV.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<EquipZoneCCTV.Fields>(ref strSets, dicSets, EquipZoneCCTV.GetFieldName, EquipZoneCCTV.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<EquipZoneCCTV.Fields>(ref strCondition, dicConditions, EquipZoneCCTV.GetFieldName, EquipZoneCCTV.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", EquipZoneCCTV.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateGltfModel(Model.GLTF.Model model, out string strErrorMessage)
        {
            Dictionary<Model.GLTF.Model.Fields, object> dicSets = new Dictionary<Model.GLTF.Model.Fields, object>();
            dicSets[Model.GLTF.Model.Fields.ParentID] = model.ParentID;
            dicSets[Model.GLTF.Model.Fields.ModelName] = model.ModelName;
            dicSets[Model.GLTF.Model.Fields.SiteID] = model.SiteID;

            Dictionary<Model.GLTF.Model.Fields, object> dicConditions = new Dictionary<Model.GLTF.Model.Fields, object>();
            dicConditions[Model.GLTF.Model.Fields.ID] = model.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.GLTF.Model.Fields>(ref strSets, dicSets, Model.GLTF.Model.GetFieldName, Model.GLTF.Model.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.GLTF.Model.Fields>(ref strCondition, dicConditions, Model.GLTF.Model.GetFieldName, Model.GLTF.Model.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.GLTF.Model.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateGltfModel(Dictionary<Model.GLTF.Model.Fields, object> dicSets, Dictionary<Model.GLTF.Model.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.GLTF.Model.Fields>(ref strSets, dicSets, Model.GLTF.Model.GetFieldName, Model.GLTF.Model.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.GLTF.Model.Fields>(ref strCondition, dicConditions, Model.GLTF.Model.GetFieldName, Model.GLTF.Model.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.GLTF.Model.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateGltfModelData(Model.GLTF.ModelData modelData, out string strErrorMessage)
        {
            Dictionary<Model.GLTF.ModelData.Fields, object> dicSets = new Dictionary<Model.GLTF.ModelData.Fields, object>();
            dicSets[Model.GLTF.ModelData.Fields.ModelID] = modelData.ModelID;
            dicSets[Model.GLTF.ModelData.Fields.ModelFile] = modelData.ModelFile;
            dicSets[Model.GLTF.ModelData.Fields.ModelDisplayText] = modelData.ModelDisplayText;
            dicSets[Model.GLTF.ModelData.Fields.CameraPositionX] = modelData.CameraPositionX;
            dicSets[Model.GLTF.ModelData.Fields.CameraPositionY] = modelData.CameraPositionY;
            dicSets[Model.GLTF.ModelData.Fields.CameraPositionZ] = modelData.CameraPositionZ;
            dicSets[Model.GLTF.ModelData.Fields.CameraQuaternionX] = modelData.CameraQuaternionX;
            dicSets[Model.GLTF.ModelData.Fields.CameraQuaternionY] = modelData.CameraQuaternionY;
            dicSets[Model.GLTF.ModelData.Fields.CameraQuaternionZ] = modelData.CameraQuaternionZ;
            dicSets[Model.GLTF.ModelData.Fields.CameraQuaternionW] = modelData.CameraQuaternionW;
            dicSets[Model.GLTF.ModelData.Fields.CameraRotationX] = modelData.CameraRotationX;
            dicSets[Model.GLTF.ModelData.Fields.CameraRotationY] = modelData.CameraRotationY;
            dicSets[Model.GLTF.ModelData.Fields.CameraRotationZ] = modelData.CameraRotationZ;
            dicSets[Model.GLTF.ModelData.Fields.CameraFov] = modelData.CameraFov;
            dicSets[Model.GLTF.ModelData.Fields.CameraNear] = modelData.CameraNear;
            dicSets[Model.GLTF.ModelData.Fields.CameraFar] = modelData.CameraFar;
            dicSets[Model.GLTF.ModelData.Fields.OrbitTargetX] = modelData.OrbitTargetX;
            dicSets[Model.GLTF.ModelData.Fields.OrbitTargetY] = modelData.OrbitTargetY;
            dicSets[Model.GLTF.ModelData.Fields.OrbitTargetZ] = modelData.OrbitTargetZ;
            dicSets[Model.GLTF.ModelData.Fields.FloorIndex] = modelData.FloorIndex;
            dicSets[Model.GLTF.ModelData.Fields.BuildingGroupID] = modelData.BuildingGroupID;
            dicSets[Model.GLTF.ModelData.Fields.BuildingID] = modelData.BuildingID;
            dicSets[Model.GLTF.ModelData.Fields.ZoneID] = modelData.ZoneID;

            Dictionary<Model.GLTF.ModelData.Fields, object> dicConditions = new Dictionary<Model.GLTF.ModelData.Fields, object>();
            dicConditions[Model.GLTF.ModelData.Fields.ID] = modelData.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.GLTF.ModelData.Fields>(ref strSets, dicSets, Model.GLTF.ModelData.GetFieldName, Model.GLTF.ModelData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.GLTF.ModelData.Fields>(ref strCondition, dicConditions, Model.GLTF.ModelData.GetFieldName, Model.GLTF.ModelData.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.GLTF.ModelData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateGltfModelData(Dictionary<Model.GLTF.ModelData.Fields, object> dicSets, Dictionary<Model.GLTF.ModelData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.GLTF.ModelData.Fields>(ref strSets, dicSets, Model.GLTF.ModelData.GetFieldName, Model.GLTF.ModelData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.GLTF.ModelData.Fields>(ref strCondition, dicConditions, Model.GLTF.ModelData.GetFieldName, Model.GLTF.ModelData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.GLTF.ModelData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateGltfModelOrthoData(Model.GLTF.ModelOrthoData modelData, out string strErrorMessage)
        {
            Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicSets = new Dictionary<Model.GLTF.ModelOrthoData.Fields, object>();
            dicSets[Model.GLTF.ModelOrthoData.Fields.ModelID] = modelData.ModelID;
            dicSets[Model.GLTF.ModelOrthoData.Fields.ModelFile] = modelData.ModelFile;
            dicSets[Model.GLTF.ModelOrthoData.Fields.CameraPositionX] = modelData.CameraPositionX;
            dicSets[Model.GLTF.ModelOrthoData.Fields.CameraPositionY] = modelData.CameraPositionY;
            dicSets[Model.GLTF.ModelOrthoData.Fields.CameraPositionZ] = modelData.CameraPositionZ;
            dicSets[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionX] = modelData.CameraQuaternionX;
            dicSets[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionY] = modelData.CameraQuaternionY;
            dicSets[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionZ] = modelData.CameraQuaternionZ;
            dicSets[Model.GLTF.ModelOrthoData.Fields.CameraQuaternionW] = modelData.CameraQuaternionW;
            dicSets[Model.GLTF.ModelOrthoData.Fields.CameraRotationX] = modelData.CameraRotationX;
            dicSets[Model.GLTF.ModelOrthoData.Fields.CameraRotationY] = modelData.CameraRotationY;
            dicSets[Model.GLTF.ModelOrthoData.Fields.CameraRotationZ] = modelData.CameraRotationZ;
            dicSets[Model.GLTF.ModelOrthoData.Fields.TargetX] = modelData.TargetX;
            dicSets[Model.GLTF.ModelOrthoData.Fields.TargetY] = modelData.TargetY;
            dicSets[Model.GLTF.ModelOrthoData.Fields.TargetZ] = modelData.TargetZ;
            dicSets[Model.GLTF.ModelOrthoData.Fields.Zoom] = modelData.Zoom;
            dicSets[Model.GLTF.ModelOrthoData.Fields.ZoneID] = modelData.ZoneID;

            Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicConditions = new Dictionary<Model.GLTF.ModelOrthoData.Fields, object>();
            dicConditions[Model.GLTF.ModelOrthoData.Fields.ID] = modelData.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.GLTF.ModelOrthoData.Fields>(ref strSets, dicSets, Model.GLTF.ModelOrthoData.GetFieldName, Model.GLTF.ModelOrthoData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.GLTF.ModelOrthoData.Fields>(ref strCondition, dicConditions, Model.GLTF.ModelOrthoData.GetFieldName, Model.GLTF.ModelOrthoData.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.GLTF.ModelOrthoData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateGltfModelOrthoData(Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicSets, Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.GLTF.ModelOrthoData.Fields>(ref strSets, dicSets, Model.GLTF.ModelOrthoData.GetFieldName, Model.GLTF.ModelOrthoData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.GLTF.ModelOrthoData.Fields>(ref strCondition, dicConditions, Model.GLTF.ModelOrthoData.GetFieldName, Model.GLTF.ModelOrthoData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.GLTF.ModelOrthoData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateOptionEtcSensor(Model.Sensor.Option.Etc option, out string strErrorMessage)
        {
            Dictionary<Model.Sensor.Option.Etc.Fields, object> dicSets = new Dictionary<Model.Sensor.Option.Etc.Fields, object>();
            dicSets[Model.Sensor.Option.Etc.Fields.DataType] = option.DataType;
            dicSets[Model.Sensor.Option.Etc.Fields.CloseAlarmSeconds] = option.CloseAlarmSeconds;
            dicSets[Model.Sensor.Option.Etc.Fields.DelaySeconds] = option.DelaySeconds;
            dicSets[Model.Sensor.Option.Etc.Fields.SiteID] = option.SiteID;

            Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions = new Dictionary<Model.Sensor.Option.Etc.Fields, object>();
            dicConditions[Model.Sensor.Option.Etc.Fields.SensorType] = option.SensorType;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Sensor.Option.Etc.Fields>(ref strSets, dicSets, Model.Sensor.Option.Etc.GetFieldName, Model.Sensor.Option.Etc.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Sensor.Option.Etc.Fields>(ref strCondition, dicConditions, Model.Sensor.Option.Etc.GetFieldName, Model.Sensor.Option.Etc.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Sensor.Option.Etc.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateOptionEtcSensor(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicSets, Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Sensor.Option.Etc.Fields>(ref strSets, dicSets, Model.Sensor.Option.Etc.GetFieldName, Model.Sensor.Option.Etc.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Sensor.Option.Etc.Fields>(ref strCondition, dicConditions, Model.Sensor.Option.Etc.GetFieldName, Model.Sensor.Option.Etc.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Sensor.Option.Etc.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateOptionEtcSensorData(Model.Sensor.Option.EtcData option, out string strErrorMessage)
        {
            Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicSets = new Dictionary<Model.Sensor.Option.EtcData.Fields, object>();
            dicSets[Model.Sensor.Option.EtcData.Fields.DataMini] = option.DataMini;
            dicSets[Model.Sensor.Option.EtcData.Fields.DataMinf] = option.DataMinf;
            dicSets[Model.Sensor.Option.EtcData.Fields.DataMins] = option.DataMins;
            dicSets[Model.Sensor.Option.EtcData.Fields.DataMaxi] = option.DataMaxi;
            dicSets[Model.Sensor.Option.EtcData.Fields.DataMaxf] = option.DataMaxf;
            dicSets[Model.Sensor.Option.EtcData.Fields.DataMaxs] = option.DataMaxs;
            dicSets[Model.Sensor.Option.EtcData.Fields.LinkedBuildingIDs] = option.LinkedBuildingIDs == null ? null : ListToString<int>(option.LinkedBuildingIDs);
            dicSets[Model.Sensor.Option.EtcData.Fields.LinkedZoneIDs] = option.LinkedZoneIDs == null ? null : ListToString<int>(option.LinkedZoneIDs);
            dicSets[Model.Sensor.Option.EtcData.Fields.SendSDMS] = option.SendSDMS;

            Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions = new Dictionary<Model.Sensor.Option.EtcData.Fields, object>();
            dicConditions[Model.Sensor.Option.EtcData.Fields.SensorType] = option.SensorType;
            dicConditions[Model.Sensor.Option.EtcData.Fields.AlarmDepth] = option.AlarmDepth;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Sensor.Option.EtcData.Fields>(ref strSets, dicSets, Model.Sensor.Option.EtcData.GetFieldName, Model.Sensor.Option.EtcData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Sensor.Option.EtcData.Fields>(ref strCondition, dicConditions, Model.Sensor.Option.EtcData.GetFieldName, Model.Sensor.Option.EtcData.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Sensor.Option.EtcData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateOptionEtcSensorData(Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicSets, Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Sensor.Option.EtcData.Fields>(ref strSets, dicSets, Model.Sensor.Option.EtcData.GetFieldName, Model.Sensor.Option.EtcData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Sensor.Option.EtcData.Fields>(ref strCondition, dicConditions, Model.Sensor.Option.EtcData.GetFieldName, Model.Sensor.Option.EtcData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Sensor.Option.EtcData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateETCSensor(ETC sensor, out string strErrorMessage)
        {
            Dictionary<ETC.Fields, object> dicSets = new Dictionary<ETC.Fields, object>();
            dicSets[ETC.Fields.CurrentData] = sensor.CurrentData;
            dicSets[ETC.Fields.Department] = sensor.Department;
            dicSets[ETC.Fields.DepartmentPhoneNumber] = sensor.DepartmentPhoneNumber;
            dicSets[ETC.Fields.Name] = sensor.Name;
            dicSets[ETC.Fields.PositionName] = sensor.PositionName;
            dicSets[ETC.Fields.X] = sensor.X;
            dicSets[ETC.Fields.Y] = sensor.Y;
            dicSets[ETC.Fields.Z] = sensor.Z;
            dicSets[ETC.Fields.ZoneID] = sensor.ZoneID;
            dicSets[ETC.Fields.Status] = sensor.Status;
            dicSets[ETC.Fields.UniqueKey] = sensor.UniqueKey;
            dicSets[ETC.Fields.MaterialType] = sensor.MaterialType;

            Dictionary<ETC.Fields, object> dicConditions = new Dictionary<ETC.Fields, object>();
            dicConditions[ETC.Fields.ID] = sensor.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<ETC.Fields>(ref strSets, dicSets, ETC.GetFieldName, ETC.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<ETC.Fields>(ref strCondition, dicConditions, ETC.GetFieldName, ETC.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", ETC.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateETCSensors(Dictionary<ETC.Fields, object> dicSets, Dictionary<ETC.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<ETC.Fields>(ref strSets, dicSets, ETC.GetFieldName, ETC.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<ETC.Fields>(ref strCondition, dicConditions, ETC.GetFieldName, ETC.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", ETC.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFacilityInfo(Info info, out string strErrorMessage)
        {
            Dictionary<Info.Fields, object> dicSets = new Dictionary<Info.Fields, object>();
            dicSets[Info.Fields.FacilityName] = info.FacilityName;
            dicSets[Info.Fields.ModelName] = info.ModelName;
            dicSets[Info.Fields.ZoneID] = info.ZoneID;

            Dictionary<Info.Fields, object> dicConditions = new Dictionary<Info.Fields, object>();
            dicConditions[Info.Fields.ID] = info.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Info.Fields>(ref strSets, dicSets, Info.GetFieldName, Info.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Info.Fields>(ref strCondition, dicConditions, Info.GetFieldName, Info.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Info.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFacilityInfos(Dictionary<Info.Fields, object> dicSets, Dictionary<Info.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Info.Fields>(ref strSets, dicSets, Info.GetFieldName, Info.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Info.Fields>(ref strCondition, dicConditions, Info.GetFieldName, Info.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Info.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFacilityInfoData(InfoData data, out string strErrorMessage)
        {
            Dictionary<InfoData.Fields, object> dicSets = new Dictionary<InfoData.Fields, object>();
            dicSets[InfoData.Fields.Value] = data.Value;
            dicSets[InfoData.Fields.WithDot] = data.WithDot;
            dicSets[InfoData.Fields.IndentDepth] = data.IndentDepth;

            Dictionary<InfoData.Fields, object> dicConditions = new Dictionary<InfoData.Fields, object>();
            dicConditions[InfoData.Fields.FacilityInfoID] = data.FacilityInfoID;
            dicConditions[InfoData.Fields.OrderIndex] = data.OrderIndex;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<InfoData.Fields>(ref strSets, dicSets, InfoData.GetFieldName, InfoData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<InfoData.Fields>(ref strCondition, dicConditions, InfoData.GetFieldName, InfoData.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", InfoData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFacilityInfoDatas(Dictionary<InfoData.Fields, object> dicSets, Dictionary<InfoData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<InfoData.Fields>(ref strSets, dicSets, InfoData.GetFieldName, InfoData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<InfoData.Fields>(ref strCondition, dicConditions, InfoData.GetFieldName, InfoData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", InfoData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBuildingData(BuildingData data, out string strErrorMessage)
        {
            Dictionary<BuildingData.Fields, object> dicSets = new Dictionary<BuildingData.Fields, object>();
            dicSets[BuildingData.Fields.Value] = data.Value;
            dicSets[BuildingData.Fields.WithDot] = data.WithDot;
            dicSets[BuildingData.Fields.IndentDepth] = data.IndentDepth;

            Dictionary<BuildingData.Fields, object> dicConditions = new Dictionary<BuildingData.Fields, object>();
            dicConditions[BuildingData.Fields.BuildingID] = data.BuildingID;
            dicConditions[BuildingData.Fields.OrderIndex] = data.OrderIndex;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<BuildingData.Fields>(ref strSets, dicSets, BuildingData.GetFieldName, BuildingData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<BuildingData.Fields>(ref strCondition, dicConditions, BuildingData.GetFieldName, BuildingData.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", BuildingData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBuildingDatas(Dictionary<BuildingData.Fields, object> dicSets, Dictionary<BuildingData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<BuildingData.Fields>(ref strSets, dicSets, BuildingData.GetFieldName, BuildingData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<BuildingData.Fields>(ref strCondition, dicConditions, BuildingData.GetFieldName, BuildingData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", BuildingData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBuildingGroupData(BuildingGroupData data, out string strErrorMessage)
        {
            Dictionary<BuildingGroupData.Fields, object> dicSets = new Dictionary<BuildingGroupData.Fields, object>();
            dicSets[BuildingGroupData.Fields.Value] = data.Value;
            dicSets[BuildingGroupData.Fields.WithDot] = data.WithDot;
            dicSets[BuildingGroupData.Fields.IndentDepth] = data.IndentDepth;

            Dictionary<BuildingGroupData.Fields, object> dicConditions = new Dictionary<BuildingGroupData.Fields, object>();
            dicConditions[BuildingGroupData.Fields.BuildingGroupID] = data.BuildingGroupID;
            dicConditions[BuildingGroupData.Fields.OrderIndex] = data.OrderIndex;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<BuildingGroupData.Fields>(ref strSets, dicSets, BuildingGroupData.GetFieldName, BuildingGroupData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<BuildingGroupData.Fields>(ref strCondition, dicConditions, BuildingGroupData.GetFieldName, BuildingGroupData.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", BuildingGroupData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateBuildingGroupDatas(Dictionary<BuildingGroupData.Fields, object> dicSets, Dictionary<BuildingGroupData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<BuildingGroupData.Fields>(ref strSets, dicSets, BuildingGroupData.GetFieldName, BuildingGroupData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<BuildingGroupData.Fields>(ref strCondition, dicConditions, BuildingGroupData.GetFieldName, BuildingGroupData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", BuildingGroupData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFakeWall(FakeWall data, out string strErrorMessage)
        {
            Dictionary<FakeWall.Fields, object> dicSets = new Dictionary<FakeWall.Fields, object>();
            dicSets[FakeWall.Fields.ZoneID] = data.ZoneID;
            dicSets[FakeWall.Fields.X] = data.X;
            dicSets[FakeWall.Fields.Y] = data.Y;
            dicSets[FakeWall.Fields.Z] = data.Z;
            dicSets[FakeWall.Fields.Rotate] = data.Rotate;
            dicSets[FakeWall.Fields.Scale] = data.Scale;

            Dictionary<FakeWall.Fields, object> dicConditions = new Dictionary<FakeWall.Fields, object>();
            dicConditions[FakeWall.Fields.ID] = data.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<FakeWall.Fields>(ref strSets, dicSets, FakeWall.GetFieldName, FakeWall.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<FakeWall.Fields>(ref strCondition, dicConditions, FakeWall.GetFieldName, FakeWall.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", FakeWall.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateFakeWalls(Dictionary<FakeWall.Fields, object> dicSets, Dictionary<FakeWall.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<FakeWall.Fields>(ref strSets, dicSets, FakeWall.GetFieldName, FakeWall.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<FakeWall.Fields>(ref strCondition, dicConditions, FakeWall.GetFieldName, FakeWall.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", FakeWall.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSpreadMessage(Model.Config.SpreadMessage spreadMessage, out string strErrorMessage)
        {
            Dictionary<Model.Config.SpreadMessage.Fields, object> dicSets = new Dictionary<Model.Config.SpreadMessage.Fields, object>();
            dicSets[Model.Config.SpreadMessage.Fields.FacilityType] = spreadMessage.FacilityType;
            dicSets[Model.Config.SpreadMessage.Fields.BuilidingGroupID] = spreadMessage.BuildingGroupID;
            dicSets[Model.Config.SpreadMessage.Fields.BuilidingID] = spreadMessage.BuildingID;
            dicSets[Model.Config.SpreadMessage.Fields.RegularID] = spreadMessage.RegularID;
            dicSets[Model.Config.SpreadMessage.Fields.RegularMemberID] = spreadMessage.RegularMemberID;
            dicSets[Model.Config.SpreadMessage.Fields.MessageType] = spreadMessage.MessageType;
            dicSets[Model.Config.SpreadMessage.Fields.Message] = spreadMessage.Message;

            Dictionary<Model.Config.SpreadMessage.Fields, object> dicConditions = new Dictionary<Model.Config.SpreadMessage.Fields, object>();
            dicConditions[Model.Config.SpreadMessage.Fields.ID] = spreadMessage.ID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Config.SpreadMessage.Fields>(ref strSets, dicSets, Model.Config.SpreadMessage.GetFieldName, Model.Config.SpreadMessage.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Config.SpreadMessage.Fields>(ref strCondition, dicConditions, Model.Config.SpreadMessage.GetFieldName, Model.Config.SpreadMessage.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Config.SpreadMessage.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateSpreadMessage(Dictionary<Model.Config.SpreadMessage.Fields, object> dicSets, Dictionary<Model.Config.SpreadMessage.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<Model.Config.SpreadMessage.Fields>(ref strSets, dicSets, Model.Config.SpreadMessage.GetFieldName, Model.Config.SpreadMessage.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<Model.Config.SpreadMessage.Fields>(ref strCondition, dicConditions, Model.Config.SpreadMessage.GetFieldName, Model.Config.SpreadMessage.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", Model.Config.SpreadMessage.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateZoneData(ZoneData data, out string strErrorMessage)
        {
            Dictionary<ZoneData.Fields, object> dicSets = new Dictionary<ZoneData.Fields, object>();
            dicSets[ZoneData.Fields.FakeWallElevation] = data.FakeWallElevation;
            dicSets[ZoneData.Fields.PoiElevation] = data.PoiElevation;
            
            Dictionary<ZoneData.Fields, object> dicConditions = new Dictionary<ZoneData.Fields, object>();
            dicConditions[ZoneData.Fields.ZoneID] = data.ZoneID;

            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<ZoneData.Fields>(ref strSets, dicSets, ZoneData.GetFieldName, ZoneData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<ZoneData.Fields>(ref strCondition, dicConditions, ZoneData.GetFieldName, ZoneData.TableName, ref strErrorMessage) == false)
                return false;

            string strSQL = string.Format("Update {0} set {1} where {2}", ZoneData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }

        public bool UpdateZoneData(Dictionary<ZoneData.Fields, object> dicSets, Dictionary<ZoneData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strCondition = "";
            string strSets = "";

            if (SetData<ZoneData.Fields>(ref strSets, dicSets, ZoneData.GetFieldName, ZoneData.TableName, ref strErrorMessage) == false)
                return false;
            if (SetCondition<ZoneData.Fields>(ref strCondition, dicConditions, ZoneData.GetFieldName, ZoneData.TableName, ref strErrorMessage) == false)
                return false;

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAdditionalConditions;
                else
                    strCondition = strAdditionalConditions;
            }

            string strSQL = string.Format("Update {0} set {1} where {2}", ZoneData.TableName, strSets, strCondition);

            if (m_dbManager.GetResultData(strSQL) == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return false;
            }

            return true;
        }
    }
}
