using System;
using System.Collections.Generic;
using System.Collections;
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
    using System.Text;
    using dnsData.Sensor;

    public class SelectManager : QueryManager, ISelect
    {
        private DataManager m_dataManager = null;
        //private WebDBManager m_dbManager = null;

        public SelectManager(DataManager dataManager)
        {
            m_dataManager = dataManager;
            m_dbManager = m_dataManager.GetDBManager() as WebDBManager;
        }

        public Building SelectBuilding(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Building.Fields>(out nFieldCount), Building.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Building model = ReadBuilding(arrResult, 0, out strErrorMessage);

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

        private Building ReadBuilding(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Building model = new Building();
            bool isNullable;

            foreach (Building.Fields field in Building.Fields.GetValues(typeof(Building.Fields)))
            {
                string strFieldName = Building.GetFieldName(field, out isNullable);

                if (field == Building.Fields.ID)
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
                else if (field == Building.Fields.BroadcastText)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.BroadcastText = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.BroadcastText = str;
                }
                else if (field == Building.Fields.BuildingCode)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.BuildingCode = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.BuildingCode = str;
                }
                else if (field == Building.Fields.BuildingGroupID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.BuildingGroupID = data.Data;
                }
                else if (field == Building.Fields.BuildingName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.BuildingName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.BuildingName = str;
                }
                else if (field == Building.Fields.DisplayText)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.DisplayText = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DisplayText = str;
                }
                else if (field == Building.Fields.MaxFloor)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.MaxFloor = data.Data;
                }
                else if (field == Building.Fields.MinFloor)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.MinFloor = data.Data;
                }
                else if (field == Building.Fields.TextCenter)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.TextCenter = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.TextCenter = StringToVertex3D(str);
                }

                index++;
            }

            return model;
        }

        public BuildingGroup SelectBuildingGroup(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<BuildingGroup.Fields>(out nFieldCount), BuildingGroup.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                BuildingGroup model = ReadBuildingGroup(arrResult, 0, out strErrorMessage);

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

        private BuildingGroup ReadBuildingGroup(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            bool isNullable;
            BuildingGroup model = new BuildingGroup();

            foreach (BuildingGroup.Fields field in BuildingGroup.Fields.GetValues(typeof(BuildingGroup.Fields)))
            {
                string strFieldName = BuildingGroup.GetFieldName(field, out isNullable);

                if (field == BuildingGroup.Fields.ID)
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
                else if (field == BuildingGroup.Fields.DisplayText)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.DisplayText = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DisplayText = str;
                }
                else if (field == BuildingGroup.Fields.GroupName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.GroupName = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.GroupName = str;
                }
                else if (field == BuildingGroup.Fields.ParentID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.ParentID = null;
                    }
                    else
                        model.ParentID = data.Data;
                }
                else if (field == BuildingGroup.Fields.SiteID)
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
                else if (field == BuildingGroup.Fields.TextCenter)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.TextCenter = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.TextCenter = StringToVertex3D(str);
                }

                index++;
            }

            return model;
        }

        public List<BuildingGroup> SelectBuildingGroups(Dictionary<BuildingGroup.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectBuildingGroups(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<BuildingGroup> SelectBuildingGroups(Dictionary<BuildingGroup.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<BuildingGroup.Fields>(out nFieldCount), BuildingGroup.TableName);

            string strCondition = "";

            if (SetCondition<BuildingGroup.Fields>(ref strCondition, dicConditions, BuildingGroup.GetFieldName, BuildingGroup.TableName, ref strErrorMessage) == false)
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
            List<BuildingGroup> buildingGroups = new List<BuildingGroup>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                BuildingGroup model = ReadBuildingGroup(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    buildingGroups.Add(model);
            }

            return buildingGroups;
        }

        public List<Building> SelectBuildings(Dictionary<Building.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectBuildings(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Building> SelectBuildings(Dictionary<Building.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Building.Fields>(out nFieldCount), Building.TableName);

            string strCondition = "";

            if (SetCondition<Building.Fields>(ref strCondition, dicConditions, Building.GetFieldName, Building.TableName, ref strErrorMessage) == false)
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
            List<Building> buildings = new List<Building>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Building model = ReadBuilding(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    buildings.Add(model);
            }

            return buildings;
        }

        public EquipmentZone SelectEquipmentZone(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<EquipmentZone.Fields>(out nFieldCount), EquipmentZone.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                EquipmentZone model = ReadEquipmentZone(arrResult, 0, out strErrorMessage);

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

        private EquipmentZone ReadEquipmentZone(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            EquipmentZone model = new EquipmentZone();
            bool isNullable;

            foreach (EquipmentZone.Fields field in EquipmentZone.Fields.GetValues(typeof(EquipmentZone.Fields)))
            {
                string strFieldName = EquipmentZone.GetFieldName(field, out isNullable);

                if (field == EquipmentZone.Fields.ID)
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
                else if (field == EquipmentZone.Fields.Boundary)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Boundary = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Boundary = StringToPolygon(str);
                }
                else if (field == EquipmentZone.Fields.BroadcastText)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.BroadcastText = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.BroadcastText = str;
                }
                else if (field == EquipmentZone.Fields.DisplayText)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.DisplayText = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DisplayText = str;
                }
                else if (field == EquipmentZone.Fields.LinkedZoneIDList)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.LinkedZoneIDs = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.LinkedZoneIDs = StringToIntList(str);
                }
                else if (field == EquipmentZone.Fields.SiteID)
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
                else if (field == EquipmentZone.Fields.TextCenter)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.TextCenter = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.TextCenter = StringToVertex3D(str);
                }
                else if (field == EquipmentZone.Fields.Type)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Type = null;
                    }
                    else
                        model.Type = data.Data;
                }
                else if (field == EquipmentZone.Fields.ZoneName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.ZoneName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ZoneName = str;
                }

                index++;
            }

            return model;
        }

        public List<EquipmentZone> SelectEquipmentZones(Dictionary<EquipmentZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectEquipmentZones(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<EquipmentZone> SelectEquipmentZones(Dictionary<EquipmentZone.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<EquipmentZone.Fields>(out nFieldCount), EquipmentZone.TableName);

            string strCondition = "";

            if (SetCondition<EquipmentZone.Fields>(ref strCondition, dicConditions, EquipmentZone.GetFieldName, EquipmentZone.TableName, ref strErrorMessage) == false)
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
            List<EquipmentZone> equipZones = new List<EquipmentZone>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                EquipmentZone model = ReadEquipmentZone(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    equipZones.Add(model);
            }

            return equipZones;
        }

        public FacilityType SelectFacilityType(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<FacilityType.Fields>(out nFieldCount), FacilityType.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                FacilityType model = ReadFacilityType(arrResult, 0, out strErrorMessage);

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

        private FacilityType ReadFacilityType(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            FacilityType model = new FacilityType();
            bool isNullable;

            foreach (FacilityType.Fields field in FacilityType.Fields.GetValues(typeof(FacilityType.Fields)))
            {
                string strFieldName = FacilityType.GetFieldName(field, out isNullable);

                if (field == FacilityType.Fields.ID)
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
                else if (field == FacilityType.Fields.Description)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Description = str;
                }
                else if (field == FacilityType.Fields.LinkedTableName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.LinkedTableName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.LinkedTableName = str;
                }
                else if (field == FacilityType.Fields.SiteID)
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
                else if (field == FacilityType.Fields.TypeName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.TypeName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.TypeName = str;
                }
                else if (field == FacilityType.Fields.DisasterCategoryID || field == FacilityType.Fields.SubDisasterCategoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (field == FacilityType.Fields.DisasterCategoryID)
                            model.DisasterCategoryID = null;
                        else if (field == FacilityType.Fields.SubDisasterCategoryID)
                            model.SubDisasterCategoryID = null;
                    }
                    else
                    {
                        if (field == FacilityType.Fields.DisasterCategoryID)
                            model.DisasterCategoryID = data.Data;
                        else if (field == FacilityType.Fields.SubDisasterCategoryID)
                            model.SubDisasterCategoryID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public List<FacilityType> SelectFacilityTypes(Dictionary<FacilityType.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectFacilityTypes(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<FacilityType> SelectFacilityTypes(Dictionary<FacilityType.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<FacilityType.Fields>(out nFieldCount), FacilityType.TableName);

            string strCondition = "";

            if (SetCondition<FacilityType.Fields>(ref strCondition, dicConditions, FacilityType.GetFieldName, FacilityType.TableName, ref strErrorMessage) == false)
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
            List<FacilityType> facilityTypes = new List<FacilityType>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                FacilityType model = ReadFacilityType(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    facilityTypes.Add(model);
            }

            return facilityTypes;
        }

        public ETC SelectETCSensor(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<ETC.Fields>(out nFieldCount), ETC.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                ETC model = ReadETCSensor(arrResult, 0, out strErrorMessage);

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

        private ETC ReadETCSensor(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            ETC model = new ETC();
            bool isNullable;

            foreach (ETC.Fields field in ETC.Fields.GetValues(typeof(ETC.Fields)))
            {
                string strFieldName = ETC.GetFieldName(field, out isNullable);

                if (field == ETC.Fields.ID)
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
                else if (field == ETC.Fields.Department)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Department = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Department = str;
                }
                else if (field == ETC.Fields.DepartmentPhoneNumber)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.DepartmentPhoneNumber = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DepartmentPhoneNumber = str;
                }
                else if (field == ETC.Fields.Name)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Name = str;
                }
                else if (field == ETC.Fields.PositionName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.PositionName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PositionName = str;
                }
                else if (field == ETC.Fields.X)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.X = null;
                    }
                    else
                        model.X = data.Data;
                }
                else if (field == ETC.Fields.Y)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Y = null;
                    }
                    else
                        model.Y = data.Data;
                }
                else if (field == ETC.Fields.Z)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Z = null;
                    }
                    else
                        model.Z = data.Data;
                }
                else if (field == ETC.Fields.ZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ZoneID = data.Data;
                }
                else if (field == ETC.Fields.CurrentData)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.CurrentData = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.CurrentData = str;
                }
                else if (field == ETC.Fields.Enabled)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Enabled = null;
                    }
                    else
                    {
                        model.Enabled = data.Data == 1;
                    }
                }
                else if (field == ETC.Fields.Status)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Status = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Status = str;
                }
                else if (field == ETC.Fields.UniqueKey)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.UniqueKey = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.UniqueKey = str;
                }
                else if (field == ETC.Fields.MaterialType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.MaterialType = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.MaterialType = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public List<ETC> SelectETCSensors(Dictionary<ETC.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectETCSensors(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<ETC> SelectETCSensors(Dictionary<ETC.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<ETC.Fields>(out nFieldCount), ETC.TableName);

            string strCondition = "";

            if (SetCondition<ETC.Fields>(ref strCondition, dicConditions, ETC.GetFieldName, ETC.TableName, ref strErrorMessage) == false)
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
            List<ETC> sensors = new List<ETC>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                ETC model = ReadETCSensor(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    sensors.Add(model);
            }

            return sensors;
        }

        public Fire SelectFireSensor(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Fire.Fields>(out nFieldCount), Fire.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Fire model = ReadFireSensor(arrResult, 0, out strErrorMessage);

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

        private Fire ReadFireSensor(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Fire model = new Fire();
            bool isNullable;

            foreach (Fire.Fields field in Fire.Fields.GetValues(typeof(Fire.Fields)))
            {
                string strFieldName = Fire.GetFieldName(field, out isNullable);

                if (field == Fire.Fields.ID)
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
                else if (field == Fire.Fields.Department)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Department = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Department = str;
                }
                else if (field == Fire.Fields.DepartmentPhoneNumber)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.DepartmentPhoneNumber = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DepartmentPhoneNumber = str;
                }
                else if (field == Fire.Fields.Name)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Name = str;
                }
                else if (field == Fire.Fields.PositionName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.PositionName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PositionName = str;
                }
                else if (field == Fire.Fields.X)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.X = null;
                    }
                    else
                        model.X = data.Data;
                }
                else if (field == Fire.Fields.Y)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Y = null;
                    }
                    else
                        model.Y = data.Data;
                }
                else if (field == Fire.Fields.Z)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Z = null;
                    }
                    else
                        model.Z = data.Data;
                }
                else if (field == Fire.Fields.ZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ZoneID = data.Data;
                }
                else if (field == Fire.Fields.Enabled)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Enabled = null;
                    }
                    else
                    {
                        model.Enabled = data.Data == 1;
                    }
                }
                else if (field == Fire.Fields.SensorSubType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.SensorSubType = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.SensorSubType = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public List<Fire> SelectFireSensors(Dictionary<Fire.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectFireSensors(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Fire> SelectFireSensors(Dictionary<Fire.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Fire.Fields>(out nFieldCount), Fire.TableName);

            string strCondition = "";

            if (SetCondition<Fire.Fields>(ref strCondition, dicConditions, Fire.GetFieldName, Fire.TableName, ref strErrorMessage) == false)
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
            List<Fire> sensors = new List<Fire>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Fire model = ReadFireSensor(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    sensors.Add(model);
            }

            return sensors;
        }


        public PSM SelectPSMSensor(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<PSM.Fields>(out nFieldCount), PSM.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                PSM model = ReadPSMSensor(arrResult, 0, out strErrorMessage);

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

        private PSM ReadPSMSensor(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            PSM model = new PSM();
            bool isNullable;

            foreach (PSM.Fields field in PSM.Fields.GetValues(typeof(PSM.Fields)))
            {
                string strFieldName = PSM.GetFieldName(field, out isNullable);

                if (field == PSM.Fields.ID)
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
                else if (field == PSM.Fields.CurrentData)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.CurrentData = null;
                    }
                    else
                        model.CurrentData = data.Data;
                }
                else if (field == PSM.Fields.Department)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Department = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Department = str;
                }
                else if (field == PSM.Fields.DepartmentPhoneNumber)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.DepartmentPhoneNumber = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DepartmentPhoneNumber = str;
                }
                else if (field == PSM.Fields.LimitLevel1)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.LimitLevel1 = null;
                    }
                    else
                        model.LimitLevel1 = data.Data;
                }
                else if (field == PSM.Fields.LimitLevel2)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.LimitLevel2 = null;
                    }
                    else
                        model.LimitLevel2 = data.Data;
                }
                else if (field == PSM.Fields.LimitLevel3)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.LimitLevel3 = null;
                    }
                    else
                        model.LimitLevel3 = data.Data;
                }
                else if (field == PSM.Fields.Name)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Name = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Name = str;
                }
                else if (field == PSM.Fields.PositionName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.PositionName = str;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.PositionName = str;
                }
                else if (field == PSM.Fields.UseLimitLevel1)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.UseLimitLevel1 = data.Data == 1;
                }
                else if (field == PSM.Fields.UseLimitLevel2)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.UseLimitLevel2 = data.Data == 1;
                }
                else if (field == PSM.Fields.UseLimitLevel3)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.UseLimitLevel3 = data.Data == 1;
                }
                else if (field == PSM.Fields.X)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.X = null;
                    }
                    else
                        model.X = data.Data;
                }
                else if (field == PSM.Fields.Y)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Y = null;
                    }
                    else
                        model.Y = data.Data;
                }
                else if (field == PSM.Fields.Z)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Z = null;
                    }
                    else
                        model.Z = data.Data;
                }                
                else if (field == PSM.Fields.EquipZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.EquipZoneID = data.Data;
                }
                else if (field == PSM.Fields.ZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ZoneID = data.Data;
                }
                else if (field == PSM.Fields.Enabled)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Enabled = null;
                    }
                    else
                    {
                        model.Enabled = data.Data == 1;
                    }
                }
                else if (field == PSM.Fields.Status)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Status = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Status = str;
                }
                else if (field == PSM.Fields.UniqueKey)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.UniqueKey = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.UniqueKey = str;
                }
                else if (field == PSM.Fields.MaterialType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.MaterialType = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.MaterialType = data.Data;
                }

                index++;
            }

            return model;
        }

        public List<PSM> SelectPSMSensors(Dictionary<PSM.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectPSMSensors(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<PSM> SelectPSMSensors(Dictionary<PSM.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<PSM.Fields>(out nFieldCount), PSM.TableName);

            string strCondition = "";

            if (SetCondition<PSM.Fields>(ref strCondition, dicConditions, PSM.GetFieldName, PSM.TableName, ref strErrorMessage) == false)
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
            List<PSM> sensors = new List<PSM>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                PSM model = ReadPSMSensor(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    sensors.Add(model);
            }

            return sensors;
        }

        public Material SelectMaterial(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Material.Fields>(out nFieldCount), Material.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Material model = ReadMaterial(arrResult, 0, out strErrorMessage);

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

        private Material ReadMaterial(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Material model = new Material();
            bool isNullable;

            foreach (Material.Fields field in Material.Fields.GetValues(typeof(Material.Fields)))
            {
                string strFieldName = Material.GetFieldName(field, out isNullable);

                if (field == Material.Fields.ID)
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
                else if (field == Material.Fields.MaterialName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.MaterialName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.MaterialName = str;
                }
                else if (field == Material.Fields.SiteID)
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
                else if (field == Material.Fields.UOM)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.UOM = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.UOM = str;
                }
                else if (field == Material.Fields.Description)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Description = str;
                }

                index++;
            }

            return model;
        }

        public List<Material> SelectMaterials(Dictionary<Material.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectMaterials(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Material> SelectMaterials(Dictionary<Material.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Material.Fields>(out nFieldCount), Material.TableName);

            string strCondition = "";

            if (SetCondition<Material.Fields>(ref strCondition, dicConditions, Material.GetFieldName, Material.TableName, ref strErrorMessage) == false)
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
            List<Material> materials = new List<Material>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Material model = ReadMaterial(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    materials.Add(model);
            }

            return materials;
        }

        public List<SensorReactionHistory> SelectSensorReactionHistories(Dictionary<SensorReactionHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSensorReactionHistories(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<SensorReactionHistory> SelectSensorReactionHistories(Dictionary<SensorReactionHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<SensorReactionHistory.Fields>(out nFieldCount), SensorReactionHistory.TableName);

            string strCondition = "";

            if (SetCondition<SensorReactionHistory.Fields>(ref strCondition, dicConditions, SensorReactionHistory.GetFieldName, SensorReactionHistory.TableName, ref strErrorMessage) == false)
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
            List<SensorReactionHistory> histories = new List<SensorReactionHistory>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                SensorReactionHistory model = ReadSensorReactionHistory(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    histories.Add(model);
            }

            return histories;
        }

        public SensorReactionHistory SelectSensorReactionHistory(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<SensorReactionHistory.Fields>(out nFieldCount), SensorReactionHistory.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                SensorReactionHistory model = ReadSensorReactionHistory(arrResult, 0, out strErrorMessage);

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

        private SensorReactionHistory ReadSensorReactionHistory(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            SensorReactionHistory model = new SensorReactionHistory();
            bool isNullable;

            foreach (SensorReactionHistory.Fields field in SensorReactionHistory.Fields.GetValues(typeof(SensorReactionHistory.Fields)))
            {
                string strFieldName = SensorReactionHistory.GetFieldName(field, out isNullable);

                if (field == SensorReactionHistory.Fields.ID)
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
                else if (field == SensorReactionHistory.Fields.Message)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Message = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Message = str;
                }
                else if (field == SensorReactionHistory.Fields.Param1)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Param1 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Param1 = str;
                }
                else if (field == SensorReactionHistory.Fields.Param2)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Param2 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Param2 = str;
                }
                else if (field == SensorReactionHistory.Fields.Param3)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Param3 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Param3 = str;
                }
                else if (field == SensorReactionHistory.Fields.Param4)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Param4 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Param4 = str;
                }
                else if (field == SensorReactionHistory.Fields.Param5)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Param5 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Param5 = str;
                }
                else if (field == SensorReactionHistory.Fields.ReactionType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ReactionType = (SensorReactionHistory.ReactionTypes)data.Data;
                }
                else if (field == SensorReactionHistory.Fields.SensorZoneHistoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.SensorZoneHistoryID = data.Data;
                }
                else if (field == SensorReactionHistory.Fields.Time)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Time = data.Data;
                }
                
                index++;
            }

            return model;
        }

        public SensorReactionHistoryDescription SelectSensorReactionHistoryDescription(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<SensorReactionHistoryDescription.Fields>(out nFieldCount), SensorReactionHistoryDescription.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                SensorReactionHistoryDescription model = ReadSensorReactionHistoryDescription(arrResult, 0, out strErrorMessage);

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

        public List<SensorReactionHistoryDescription> SelectSensorReactionHistoryDescriptions(Dictionary<SensorReactionHistoryDescription.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSensorReactionHistoryDescriptions(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<SensorReactionHistoryDescription> SelectSensorReactionHistoryDescriptions(Dictionary<SensorReactionHistoryDescription.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<SensorReactionHistoryDescription.Fields>(out nFieldCount), SensorReactionHistoryDescription.TableName);

            string strCondition = "";

            if (SetCondition<SensorReactionHistoryDescription.Fields>(ref strCondition, dicConditions, SensorReactionHistoryDescription.GetFieldName, SensorReactionHistoryDescription.TableName, ref strErrorMessage) == false)
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
            List<SensorReactionHistoryDescription> descriptions = new List<SensorReactionHistoryDescription>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                SensorReactionHistoryDescription model = ReadSensorReactionHistoryDescription(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    descriptions.Add(model);
            }

            return descriptions;
        }

        private SensorReactionHistoryDescription ReadSensorReactionHistoryDescription(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            SensorReactionHistoryDescription model = new SensorReactionHistoryDescription();
            bool isNullable;

            foreach (SensorReactionHistoryDescription.Fields field in SensorReactionHistoryDescription.Fields.GetValues(typeof(SensorReactionHistoryDescription.Fields)))
            {
                string strFieldName = SensorReactionHistoryDescription.GetFieldName(field, out isNullable);

                if (field == SensorReactionHistoryDescription.Fields.ID)
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
                else if (field == SensorReactionHistoryDescription.Fields.SensorReactionHistoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.SensorReactionHistoryID = data.Data;
                }
                else if (field == SensorReactionHistoryDescription.Fields.DescriptionID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.DescriptionID = data.Data;
                }
                else if (field == SensorReactionHistoryDescription.Fields.SensorZoneHistoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.SensorZoneHistoryID = null;
                    }
                    else
                        model.SensorZoneHistoryID = data.Data;
                }

                index++;
            }

            return model;
        }

        public SensorReactionHistoryDescriptionText SelectSensorReactionHistoryDescriptionText(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<SensorReactionHistoryDescriptionText.Fields>(out nFieldCount), SensorReactionHistoryDescriptionText.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                SensorReactionHistoryDescriptionText model = ReadSensorReactionHistoryDescriptionText(arrResult, 0, out strErrorMessage);

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

        public List<SensorReactionHistoryDescriptionText> SelectSensorReactionHistoryDescriptionTexts(Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSensorReactionHistoryDescriptionTexts(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<SensorReactionHistoryDescriptionText> SelectSensorReactionHistoryDescriptionTexts(Dictionary<SensorReactionHistoryDescriptionText.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<SensorReactionHistoryDescriptionText.Fields>(out nFieldCount), SensorReactionHistoryDescriptionText.TableName);

            string strCondition = "";

            if (SetCondition<SensorReactionHistoryDescriptionText.Fields>(ref strCondition, dicConditions, SensorReactionHistoryDescriptionText.GetFieldName, SensorReactionHistoryDescriptionText.TableName, ref strErrorMessage) == false)
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
            List<SensorReactionHistoryDescriptionText> texts = new List<SensorReactionHistoryDescriptionText>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                SensorReactionHistoryDescriptionText model = ReadSensorReactionHistoryDescriptionText(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    texts.Add(model);
            }

            return texts;
        }

        private SensorReactionHistoryDescriptionText ReadSensorReactionHistoryDescriptionText(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            SensorReactionHistoryDescriptionText model = new SensorReactionHistoryDescriptionText();
            bool isNullable;

            foreach (SensorReactionHistoryDescriptionText.Fields field in SensorReactionHistoryDescriptionText.Fields.GetValues(typeof(SensorReactionHistoryDescriptionText.Fields)))
            {
                string strFieldName = SensorReactionHistoryDescriptionText.GetFieldName(field, out isNullable);

                if (field == SensorReactionHistoryDescriptionText.Fields.ID)
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
                else if (field == SensorReactionHistoryDescriptionText.Fields.RefCount)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.RefCount = data.Data;
                }
                else if (field == SensorReactionHistoryDescriptionText.Fields.Description)
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

        public SensorZone SelectSensorZone(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<SensorZone.Fields>(out nFieldCount), SensorZone.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                SensorZone model = ReadSensorZone(arrResult, 0, out strErrorMessage);

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

        private SensorZone ReadSensorZone(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            SensorZone model = new SensorZone();
            bool isNullable;

            foreach (SensorZone.Fields field in SensorZone.Fields.GetValues(typeof(SensorZone.Fields)))
            {
                string strFieldName = SensorZone.GetFieldName(field, out isNullable);

                if (field == SensorZone.Fields.ID)
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
                else if (field == SensorZone.Fields.EquipZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.EquipZoneID = data.Data;
                }
                else if (field == SensorZone.Fields.OrgSensorID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.OrgSensorID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.OrgSensorID = data.Data;
                }
                else if (field == SensorZone.Fields.SensorType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.SensorType = data.Data;
                }
                else if (field == SensorZone.Fields.IsAlarmStatus)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.IsAlarmStatus = data.Data == 1;
                }
                else if (field == SensorZone.Fields.Data)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Data = null;
                    }
                    else
                        model.Data = data.Data;
                }

                index++;
            }

            return model;
        }

        public List<SensorZoneHistory> SelectSensorZoneHistories(Dictionary<SensorZoneHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSensorZoneHistories(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<SensorZoneHistory> SelectSensorZoneHistories(Dictionary<SensorZoneHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<SensorZoneHistory.Fields>(out nFieldCount), SensorZoneHistory.TableName);

            string strCondition = "";

            if (SetCondition<SensorZoneHistory.Fields>(ref strCondition, dicConditions, SensorZoneHistory.GetFieldName, SensorZoneHistory.TableName, ref strErrorMessage) == false)
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
            List<SensorZoneHistory> histories = new List<SensorZoneHistory>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                SensorZoneHistory model = ReadSensorZoneHistory(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    histories.Add(model);
            }

            return histories;
        }

        public SensorZoneHistory SelectSensorZoneHistory(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<SensorZoneHistory.Fields>(out nFieldCount), SensorZoneHistory.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                SensorZoneHistory model = ReadSensorZoneHistory(arrResult, 0, out strErrorMessage);

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

        private SensorZoneHistory ReadSensorZoneHistory(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            SensorZoneHistory model = new SensorZoneHistory();
            bool isNullable;

            foreach (SensorZoneHistory.Fields field in SensorZoneHistory.Fields.GetValues(typeof(SensorZoneHistory.Fields)))
            {
                string strFieldName = SensorZoneHistory.GetFieldName(field, out isNullable);

                if (field == SensorZoneHistory.Fields.ID)
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
                else if (field == SensorZoneHistory.Fields.Data)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Data = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Data = str;
                }
                else if (field == SensorZoneHistory.Fields.DetectionStatus)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.DetectionStatus = SensorZoneHistory.DetectionType.None;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DetectionStatus = (SensorZoneHistory.DetectionType)data.Data;
                }
                else if (field == SensorZoneHistory.Fields.SensorType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.SensorType = data.Data;
                }
                else if (field == SensorZoneHistory.Fields.SensorZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.SensorZoneID = data.Data;
                }
                else if (field == SensorZoneHistory.Fields.SiteID)
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
                else if (field == SensorZoneHistory.Fields.Time)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.Time = data.Data;
                }
                else if (field == SensorZoneHistory.Fields.ZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ZoneID = data.Data;
                }
                else if (field == SensorZoneHistory.Fields.AllSensorZoneIDs)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                        model.AllSensorZoneIDs = null;
                    else
                        model.AllSensorZoneIDs = StringToIntList(data);
                }

                else if (field == SensorZoneHistory.Fields.Memo)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                        model.Memo = null;
                    else
                        model.Memo = data;
                }

                index++;
            }

            return model;
        }

        public List<SensorZone> SelectSensorZones(Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSensorZones(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<SensorZone> SelectSensorZones(Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<SensorZone.Fields>(out nFieldCount), SensorZone.TableName);

            string strCondition = "";

            if (SetCondition<SensorZone.Fields>(ref strCondition, dicConditions, SensorZone.GetFieldName, SensorZone.TableName, ref strErrorMessage) == false)
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
            List<SensorZone> sensorZones = new List<SensorZone>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                SensorZone model = ReadSensorZone(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    sensorZones.Add(model);
            }

            return sensorZones;
        }

        public Zone SelectZone(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Zone.Fields>(out nFieldCount), Zone.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Zone model = ReadZone(arrResult, 0, out strErrorMessage);

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

        private Zone ReadZone(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Zone model = new Zone();
            bool isNullable;

            foreach (Zone.Fields field in Zone.Fields.GetValues(typeof(Zone.Fields)))
            {
                string strFieldName = Zone.GetFieldName(field, out isNullable);

                if (field == Zone.Fields.ID)
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
                else if (field == Zone.Fields.AddFloor)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.AddFloor = null;
                    }
                    else
                    {
                        model.AddFloor = data.Data;
                    }
                }
                else if (field == Zone.Fields.Boundary)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Boundary = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Boundary = StringToPolygon(str);
                }
                else if (field == Zone.Fields.BroadcastText)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.BroadcastText = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.BroadcastText = str;
                }
                else if (field == Zone.Fields.DisplayText)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.DisplayText = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.DisplayText = str;
                }
                else if (field == Zone.Fields.BuildingID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.BuildingID = null;
                    }
                    else
                    {
                        model.BuildingID = data.Data;
                    }
                }
                else if (field == Zone.Fields.FloorIndex)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.FloorIndex = null;
                    }
                    else
                    {
                        model.FloorIndex = data.Data;
                    }
                }
                else if (field == Zone.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }
                else if (field == Zone.Fields.TextCenter)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.TextCenter = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.TextCenter = StringToVertex3D(str);
                }
                else if (field == Zone.Fields.ZoneName)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.ZoneName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.ZoneName = str;
                }

                index++;
            }

            return model;
        }

        public List<Zone> SelectZones(Dictionary<Zone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectZones(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Zone> SelectZones(Dictionary<Zone.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Zone.Fields>(out nFieldCount), Zone.TableName);

            string strCondition = "";

            if (SetCondition<Zone.Fields>(ref strCondition, dicConditions, Zone.GetFieldName, Zone.TableName, ref strErrorMessage) == false)
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
            List<Zone> zones = new List<Zone>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Zone model = ReadZone(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    zones.Add(model);
            }

            return zones;
        }

        private ServerInfo ReadSensorServerInfo(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            ServerInfo model = new ServerInfo();
            bool isNullable;

            foreach (ServerInfo.Fields field in ServerInfo.Fields.GetValues(typeof(ServerInfo.Fields)))
            {
                string strFieldName = ServerInfo.GetFieldName(field, out isNullable);

                if (field == ServerInfo.Fields.ID)
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
                else if (field == ServerInfo.Fields.Place)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Place = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Place = str;
                }
                else if (field == ServerInfo.Fields.IP)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.IP = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.IP = str;
                }

                index++;
            }

            return model;
        }

        public ServerInfo SelectSensorServerInfo(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<ServerInfo.Fields>(out nFieldCount), SensorReactionHistory.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                ServerInfo model = ReadSensorServerInfo(arrResult, 0, out strErrorMessage);

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

        public List<ServerInfo> SelectSensorServerInfo(Dictionary<ServerInfo.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSensorServerInfo(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<ServerInfo> SelectSensorServerInfo(Dictionary<ServerInfo.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<ServerInfo.Fields>(out nFieldCount), ServerInfo.TableName);

            string strCondition = "";

            if (SetCondition<ServerInfo.Fields>(ref strCondition, dicConditions, ServerInfo.GetFieldName, ServerInfo.TableName, ref strErrorMessage) == false)
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
            List<ServerInfo> infos = new List<ServerInfo>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                ServerInfo model = ReadSensorServerInfo(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    infos.Add(model);
            }

            return infos;
        }

        private TagInfo ReadSensorTagInfo(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            TagInfo model = new TagInfo();
            bool isNullable;

            foreach (TagInfo.Fields field in TagInfo.Fields.GetValues(typeof(TagInfo.Fields)))
            {
                string strFieldName = TagInfo.GetFieldName(field, out isNullable);

                if (field == TagInfo.Fields.ID)
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
                else if(field == TagInfo.Fields.SensorServerID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SensorServerID = data.Data;
                    }
                }
                else if(field == TagInfo.Fields.TagNo)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.TagNo = data.Data;
                    }
                }
                else if(field == TagInfo.Fields.SensorZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SensorZoneID = data.Data;
                    }
                }
                else if(field == TagInfo.Fields.Activate)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.IsActivate = (data.Data == 1) ? true : false;
                    }
                }
                else if (field == TagInfo.Fields.Description)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
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
                        model.Description = str;
                }

                index++;
            }

            return model;
        }

        public TagInfo SelectSensorTagInfo(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<TagInfo.Fields>(out nFieldCount), SensorReactionHistory.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                TagInfo model = ReadSensorTagInfo(arrResult, 0, out strErrorMessage);

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

        public List<TagInfo> SelectSensorTagInfo(Dictionary<TagInfo.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSensorTagInfo(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<TagInfo> SelectSensorTagInfo(Dictionary<TagInfo.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<TagInfo.Fields>(out nFieldCount), TagInfo.TableName);

            string strCondition = "";

            if (SetCondition<TagInfo.Fields>(ref strCondition, dicConditions, TagInfo.GetFieldName, TagInfo.TableName, ref strErrorMessage) == false)
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
            List<TagInfo> infos = new List<TagInfo>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                TagInfo model = ReadSensorTagInfo(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    infos.Add(model);
            }

            return infos;
        }

        public List<FireSensorZone> SelectFireSensorZone(out string strErrorMessage)
        {
            return SelectFireSensorZone(null, out strErrorMessage);
        }

        public List<FireSensorZone> SelectFireSensorZone(int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            StringBuilder sb = new StringBuilder();
            sb.Append("Select f.ID, sz.ID as SensorZoneID, Name, PositionName, ZoneID, EquipZoneID, SensorType ");
            sb.Append("  From SdmsSensorFire as f, SdmsSensorZone as sz ");
            sb.Append(" Where f.ID = sz.OrgSensorID ");
            sb.Append("   And sz.SensorType = 0 ");
            sb.Append("   And sz.IsAlarmStatus = 1 ");

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = "조회 오류입니다. " + m_dbManager.LastErrorMessage;
                return null;
            }

            List<FireSensorZone> datas = new List<FireSensorZone>();

            for (int i = 0; i < arrResult.Count; i += 7)
            {
                int orgID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int sensorZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string sensorName = WebDBManager.GetStringField(arrResult[i + 2]);
                string positionName = WebDBManager.GetStringField(arrResult[i + 3]);
                int zoneID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int equipzoneID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int sensorType = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                FireSensorZone data = new FireSensorZone();
                data.OrgSensorID = orgID;
                data.SensorZoneID = sensorZoneID;
                data.Name = sensorName;
                data.PositionName = positionName;
                data.ZoneID = zoneID;
                data.EquipZoneID = equipzoneID;
                data.SensorType = sensorType;

                datas.Add(data);
            }

            return datas;
        }

        public Broadcast SelectBroadcast(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Broadcast.Fields>(out nFieldCount), Broadcast.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Broadcast model = ReadBroadcast(arrResult, 0, out strErrorMessage);

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

        public List<Broadcast> SelectBroadcasts(Dictionary<Broadcast.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectBroadcasts(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Broadcast> SelectBroadcasts(Dictionary<Broadcast.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Broadcast.Fields>(out nFieldCount), Broadcast.TableName);

            string strCondition = "";

            if (SetCondition<Broadcast.Fields>(ref strCondition, dicConditions, Broadcast.GetFieldName, Broadcast.TableName, ref strErrorMessage) == false)
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
            List<Broadcast> broadcasts = new List<Broadcast>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Broadcast model = ReadBroadcast(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    broadcasts.Add(model);
            }

            return broadcasts;
        }

        private Broadcast ReadBroadcast(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Broadcast model = new Broadcast();
            bool isNullable;

            foreach (Broadcast.Fields field in Broadcast.Fields.GetValues(typeof(Broadcast.Fields)))
            {
                string strFieldName = Broadcast.GetFieldName(field, out isNullable);

                if (field == Broadcast.Fields.ID)
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
                else if (field == Broadcast.Fields.Text)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Text = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Text = data;
                    }
                }
                else if (field == Broadcast.Fields.UseSiren)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.UseSiren = data.Data == 1;
                    }
                }
                else if (field == Broadcast.Fields.PlayOption)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.PlayOption = data.Data;
                    }
                }
                else if (field == Broadcast.Fields.RepeatCount)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.RepeatCount = data.Data;
                    }
                }
                else if (field == Broadcast.Fields.RequestTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.RequestTime = data.Data;
                }
                else if(field == Broadcast.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public Model.Broadcast.History SelectBroadcastHistory(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Model.Broadcast.History.Fields>(out nFieldCount), Model.Broadcast.History.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Model.Broadcast.History model = ReadBroadcastHistory(arrResult, 0, out strErrorMessage);

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

        public List<Model.Broadcast.History> SelectBroadcastHistories(Dictionary<Model.Broadcast.History.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectBroadcastHistories(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Model.Broadcast.History> SelectBroadcastHistories(Dictionary<Model.Broadcast.History.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Model.Broadcast.History.Fields>(out nFieldCount), Model.Broadcast.History.TableName);

            string strCondition = "";

            if (SetCondition<Model.Broadcast.History.Fields>(ref strCondition, dicConditions, Model.Broadcast.History.GetFieldName, Model.Broadcast.History.TableName, ref strErrorMessage) == false)
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
            List<Model.Broadcast.History> histories = new List<Model.Broadcast.History>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Model.Broadcast.History model = ReadBroadcastHistory(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    histories.Add(model);
            }

            return histories;
        }

        private Model.Broadcast.History ReadBroadcastHistory(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Model.Broadcast.History model = new Model.Broadcast.History();
            bool isNullable;

            foreach (Model.Broadcast.History.Fields field in Model.Broadcast.History.Fields.GetValues(typeof(Model.Broadcast.History.Fields)))
            {
                string strFieldName = Model.Broadcast.History.GetFieldName(field, out isNullable);

                if (field == Model.Broadcast.History.Fields.ID)
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
                else if (field == Model.Broadcast.History.Fields.Text)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Text = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Text = data;
                    }
                }
                else if (field == Model.Broadcast.History.Fields.UseSiren)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.UseSiren = data.Data == 1;
                    }
                }
                else if (field == Model.Broadcast.History.Fields.PlayOption)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.PlayOption = data.Data;
                    }
                }
                else if (field == Model.Broadcast.History.Fields.RepeatCount)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.RepeatCount = data.Data;
                    }
                }
                else if (field == Model.Broadcast.History.Fields.RequestTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.RequestTime = data.Data;
                }
                else if (field == Model.Broadcast.History.Fields.ExecuteTime)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                        model.ExecuteTime = data.Data;
                }
                else if (field == Model.Broadcast.History.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public Model.Broadcast.State SelectBroadcastState(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Model.Broadcast.State.Fields>(out nFieldCount), Model.Broadcast.History.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Model.Broadcast.State model = ReadBroadcastState(arrResult, 0, out strErrorMessage);

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

        public List<Model.Broadcast.State> SelectBroadcastStates(Dictionary<Model.Broadcast.State.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectBroadcastStates(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Model.Broadcast.State> SelectBroadcastStates(Dictionary<Model.Broadcast.State.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Model.Broadcast.State.Fields>(out nFieldCount), Model.Broadcast.State.TableName);

            string strCondition = "";

            if (SetCondition<Model.Broadcast.State.Fields>(ref strCondition, dicConditions, Model.Broadcast.State.GetFieldName, Model.Broadcast.State.TableName, ref strErrorMessage) == false)
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
            List<Model.Broadcast.State> histories = new List<Model.Broadcast.State>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Model.Broadcast.State model = ReadBroadcastState(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    histories.Add(model);
            }

            return histories;
        }

        private Model.Broadcast.State ReadBroadcastState(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Model.Broadcast.State model = new Model.Broadcast.State();
            bool isNullable;

            foreach (Model.Broadcast.State.Fields field in Model.Broadcast.State.Fields.GetValues(typeof(Model.Broadcast.State.Fields)))
            {
                string strFieldName = Model.Broadcast.State.GetFieldName(field, out isNullable);

                if (field == Model.Broadcast.State.Fields.ID)
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
                else if (field == Model.Broadcast.State.Fields.HeartBeat)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.HeartBeat = data.Data;
                    }
                }
                else if (field == Model.Broadcast.State.Fields.BState)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.BState = data.Data;
                    }
                }
                else if (field == Model.Broadcast.State.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public SMSHistory SelectSMSHistory(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<SMSHistory.Fields>(out nFieldCount), SMSHistory.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                SMSHistory model = ReadSMSHistory(arrResult, 0, out strErrorMessage);

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

        public List<SMSHistory> SelectSMSHistories(Dictionary<SMSHistory.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSMSHistories(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<SMSHistory> SelectSMSHistories(Dictionary<SMSHistory.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<SMSHistory.Fields>(out nFieldCount), SMSHistory.TableName);

            string strCondition = "";

            if (SetCondition<SMSHistory.Fields>(ref strCondition, dicConditions, SMSHistory.GetFieldName, SMSHistory.TableName, ref strErrorMessage) == false)
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
            List<SMSHistory> histories = new List<SMSHistory>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                SMSHistory model = ReadSMSHistory(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    histories.Add(model);
            }

            return histories;
        }

        private SMSHistory ReadSMSHistory(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            SMSHistory model = new SMSHistory();
            bool isNullable;

            foreach (SMSHistory.Fields field in SMSHistory.Fields.GetValues(typeof(SMSHistory.Fields)))
            {
                string strFieldName = SMSHistory.GetFieldName(field, out isNullable);

                if (field == SMSHistory.Fields.ID)
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
                else if (field == SMSHistory.Fields.SensorZoneHistoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SensorZoneHistoryID = data.Data;
                    }
                }
                else if (field == SMSHistory.Fields.SensorReactionHistoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SensorReactionHistoryID = data.Data;
                    }
                }
                else if (field == SMSHistory.Fields.SMSMessage)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.SMSMessage = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.SMSMessage = data;
                    }
                }
                else if (field == SMSHistory.Fields.SendType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SendType = data.Data == 1;
                    }
                }
                else if (field == SMSHistory.Fields.RegularMemberIDList)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.RegularMemberIDList = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.RegularMemberIDList = StringToIntList(data);
                    }
                }

                index++;
            }

            return model;
        }

        public Model.Config.Broadcast SelectBroadcastConfig(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Model.Config.Broadcast.Fields>(out nFieldCount), Model.Config.Broadcast.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Model.Config.Broadcast model = ReadBroadcastConfig(arrResult, 0, out strErrorMessage);

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

        public List<Model.Config.Broadcast> SelectBroadcastConfigs(Dictionary<Model.Config.Broadcast.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectBroadcastConfigs(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Model.Config.Broadcast> SelectBroadcastConfigs(Dictionary<Model.Config.Broadcast.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Model.Config.Broadcast.Fields>(out nFieldCount), Model.Config.Broadcast.TableName);

            string strCondition = "";

            if (SetCondition<Model.Config.Broadcast.Fields>(ref strCondition, dicConditions, Model.Config.Broadcast.GetFieldName, Model.Config.Broadcast.TableName, ref strErrorMessage) == false)
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
            List<Model.Config.Broadcast> configs = new List<Model.Config.Broadcast>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Model.Config.Broadcast model = ReadBroadcastConfig(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    configs.Add(model);
            }

            return configs;
        }

        private Model.Config.Broadcast ReadBroadcastConfig(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Model.Config.Broadcast model = new Model.Config.Broadcast();
            bool isNullable;

            foreach (Model.Config.Broadcast.Fields field in Model.Config.Broadcast.Fields.GetValues(typeof(Model.Config.Broadcast.Fields)))
            {
                string strFieldName = Model.Config.Broadcast.GetFieldName(field, out isNullable);

                if (field == Model.Config.Broadcast.Fields.ID)
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
                else if (field == Model.Config.Broadcast.Fields.SituationType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SituationType = data.Data;
                    }
                }
                else if (field == Model.Config.Broadcast.Fields.UseBroadcast)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.UseBroadcast = data.Data == 1;
                    }
                }
                else if (field == Model.Config.Broadcast.Fields.Message)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Message = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Message = data;
                    }
                }
                else if (field == Model.Config.Broadcast.Fields.UseSiren)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.UseSiren = data.Data == 1;
                    }
                }
                else if (field == Model.Config.Broadcast.Fields.RepeatCount)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.RepeatCount = data.Data;
                    }
                }
                else if (field == Model.Config.Broadcast.Fields.Description)
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
                    {
                        model.Description = data;
                    }
                }
                else if (field == Model.Config.Broadcast.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public Model.Config.SMS SelectSMSConfig(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Model.Config.SMS.Fields>(out nFieldCount), Model.Config.SMS.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Model.Config.SMS model = ReadSMSConfig(arrResult, 0, out strErrorMessage);

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

        public List<Model.Config.SMS> SelectSMSConfigs(Dictionary<Model.Config.SMS.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSMSConfigs(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Model.Config.SMS> SelectSMSConfigs(Dictionary<Model.Config.SMS.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Model.Config.SMS.Fields>(out nFieldCount), Model.Config.SMS.TableName);

            string strCondition = "";

            if (SetCondition<Model.Config.SMS.Fields>(ref strCondition, dicConditions, Model.Config.SMS.GetFieldName, Model.Config.SMS.TableName, ref strErrorMessage) == false)
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
            List<Model.Config.SMS> configs = new List<Model.Config.SMS>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Model.Config.SMS model = ReadSMSConfig(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    configs.Add(model);
            }

            return configs;
        }

        private Model.Config.SMS ReadSMSConfig(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Model.Config.SMS model = new Model.Config.SMS();
            bool isNullable;

            foreach (Model.Config.SMS.Fields field in Model.Config.SMS.Fields.GetValues(typeof(Model.Config.SMS.Fields)))
            {
                string strFieldName = Model.Config.SMS.GetFieldName(field, out isNullable);

                if (field == Model.Config.SMS.Fields.ID)
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
                else if (field == Model.Config.SMS.Fields.MessageType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.MessageType = data.Data;
                    }
                }
                else if (field == Model.Config.SMS.Fields.UseSMS)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.UseSMS = data.Data == 1;
                    }
                }
                else if (field == Model.Config.SMS.Fields.Description)
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
                    {
                        model.Description = data;
                    }
                }
                else if (field == Model.Config.SMS.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public List<CurrentAlarm> SelectCurrentAlarms(Dictionary<CurrentAlarm.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectCurrentAlarms(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<CurrentAlarm> SelectCurrentAlarms(Dictionary<CurrentAlarm.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<CurrentAlarm.Fields>(out nFieldCount), CurrentAlarm.TableName);

            string strCondition = "";

            if (SetCondition<CurrentAlarm.Fields>(ref strCondition, dicConditions, CurrentAlarm.GetFieldName, CurrentAlarm.TableName, ref strErrorMessage) == false)
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

            List<CurrentAlarm> configs = new List<CurrentAlarm>();

            for (int i = 0; i < arrResult.Count - (nFieldCount - 1); i += nFieldCount)
            {
                CurrentAlarm model = ReadCurrentAlarm(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    configs.Add(model);
            }

            return configs;
        }

        private CurrentAlarm ReadCurrentAlarm(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            CurrentAlarm model = new CurrentAlarm();
            bool isNullable;

            foreach (CurrentAlarm.Fields field in CurrentAlarm.Fields.GetValues(typeof(CurrentAlarm.Fields)))
            {
                string strFieldName = CurrentAlarm.GetFieldName(field, out isNullable);

                if (field == CurrentAlarm.Fields.SensorZoneHistoryID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SensorZoneHistoryID = data.Data;
                    }
                }
                else if (field == CurrentAlarm.Fields.SensorType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SensorType = data.Data;
                    }
                }
                else if (field == CurrentAlarm.Fields.AlarmType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.AlarmType = data.Data;
                    }
                }
                else if (field == CurrentAlarm.Fields.TimeStamp)
                {
                    VariousData<DateTime> data = WebDBManager.GetDateTimeField(arrResult[index]);

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.TimeStamp = data.Data;
                    }
                }
                else if (field == CurrentAlarm.Fields.SopStatus)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SopStatus = data.Data;
                    }
                }
                else if (field == CurrentAlarm.Fields.AlarmDepth)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.AlarmDepth = data.Data;
                    }
                }
                else if (field == CurrentAlarm.Fields.AlarmSensorZoneIDs)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.AlarmSensorZoneIDs = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.AlarmSensorZoneIDs = StringToIntList(data);
                    }
                }

                index++;
            }

            return model;
        }

        public FacilityManager SelectFacilityManager(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<FacilityManager.Fields>(out nFieldCount), FacilityManager.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                FacilityManager model = ReadFacilityManager(arrResult, 0, out strErrorMessage);

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

        public List<FacilityManager> SelectFacilityManagers(Dictionary<FacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectFacilityManagers(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<FacilityManager> SelectFacilityManagers(Dictionary<FacilityManager.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<FacilityManager.Fields>(out nFieldCount), FacilityManager.TableName);

            string strCondition = "";

            if (SetCondition<FacilityManager.Fields>(ref strCondition, dicConditions, FacilityManager.GetFieldName, FacilityManager.TableName, ref strErrorMessage) == false)
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
            List<FacilityManager> configs = new List<FacilityManager>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                FacilityManager model = ReadFacilityManager(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    configs.Add(model);
            }

            return configs;
        }

        private FacilityManager ReadFacilityManager(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            FacilityManager model = new FacilityManager();
            bool isNullable;

            foreach (FacilityManager.Fields field in FacilityManager.Fields.GetValues(typeof(FacilityManager.Fields)))
            {
                string strFieldName = FacilityManager.GetFieldName(field, out isNullable);

                if (field == FacilityManager.Fields.ID)
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
                else if (field == FacilityManager.Fields.MemberID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.MemberID = data.Data;
                    }
                }
                else if (field == FacilityManager.Fields.MemberType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.MemberType = data.Data;
                    }
                }
                else if (field == FacilityManager.Fields.FacilityType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.FacilityType = data.Data;
                    }
                }
                else if (field == FacilityManager.Fields.DetectType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.DetectType = data.Data;
                    }
                }
                else if (field == FacilityManager.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }
                else if (field == FacilityManager.Fields.Description)
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
                    {
                        model.Description = data;
                    }
                }

                index++;
            }

            return model;
        }

        public BuildingFacilityManager SelectBuildingFacilityManager(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<BuildingFacilityManager.Fields>(out nFieldCount), BuildingFacilityManager.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                BuildingFacilityManager model = ReadBuildingFacilityManager(arrResult, 0, out strErrorMessage);

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

        public List<BuildingFacilityManager> SelectBuildingFacilityManagers(Dictionary<BuildingFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectBuildingFacilityManagers(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<BuildingFacilityManager> SelectBuildingFacilityManagers(Dictionary<BuildingFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<BuildingFacilityManager.Fields>(out nFieldCount), BuildingFacilityManager.TableName);

            string strCondition = "";

            if (SetCondition<BuildingFacilityManager.Fields>(ref strCondition, dicConditions, BuildingFacilityManager.GetFieldName, BuildingFacilityManager.TableName, ref strErrorMessage) == false)
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
            List<BuildingFacilityManager> configs = new List<BuildingFacilityManager>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                BuildingFacilityManager model = ReadBuildingFacilityManager(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    configs.Add(model);
            }

            return configs;
        }

        private BuildingFacilityManager ReadBuildingFacilityManager(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            BuildingFacilityManager model = new BuildingFacilityManager();
            bool isNullable;

            foreach (BuildingFacilityManager.Fields field in BuildingFacilityManager.Fields.GetValues(typeof(BuildingFacilityManager.Fields)))
            {
                string strFieldName = BuildingFacilityManager.GetFieldName(field, out isNullable);

                if (field == BuildingFacilityManager.Fields.ID)
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
                else if (field == BuildingFacilityManager.Fields.MemberID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.MemberID = data.Data;
                    }
                }
                else if (field == BuildingFacilityManager.Fields.MemberType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.MemberType = data.Data;
                    }
                }
                else if (field == BuildingFacilityManager.Fields.FacilityType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.FacilityType = data.Data;
                    }
                }
                else if (field == BuildingFacilityManager.Fields.DetectType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.DetectType = data.Data;
                    }
                }
                else if (field == BuildingFacilityManager.Fields.BuildingID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.BuildingID = data.Data;
                    }
                }
                else if (field == BuildingFacilityManager.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }
                else if (field == BuildingFacilityManager.Fields.Description)
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
                    {
                        model.Description = data;
                    }
                }

                index++;
            }

            return model;
        }

        public EquipZoneFacilityManager SelectEquipZoneFacilityManager(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<EquipZoneFacilityManager.Fields>(out nFieldCount), EquipZoneFacilityManager.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                EquipZoneFacilityManager model = ReadEquipZoneFacilityManager(arrResult, 0, out strErrorMessage);

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

        public List<EquipZoneFacilityManager> SelectEquipZoneFacilityManagers(Dictionary<EquipZoneFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectEquipZoneFacilityManagers(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<EquipZoneFacilityManager> SelectEquipZoneFacilityManagers(Dictionary<EquipZoneFacilityManager.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<EquipZoneFacilityManager.Fields>(out nFieldCount), EquipZoneFacilityManager.TableName);

            string strCondition = "";

            if (SetCondition<EquipZoneFacilityManager.Fields>(ref strCondition, dicConditions, EquipZoneFacilityManager.GetFieldName, EquipZoneFacilityManager.TableName, ref strErrorMessage) == false)
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
            List<EquipZoneFacilityManager> configs = new List<EquipZoneFacilityManager>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                EquipZoneFacilityManager model = ReadEquipZoneFacilityManager(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    configs.Add(model);
            }

            return configs;
        }

        private EquipZoneFacilityManager ReadEquipZoneFacilityManager(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            EquipZoneFacilityManager model = new EquipZoneFacilityManager();
            bool isNullable;

            foreach (EquipZoneFacilityManager.Fields field in EquipZoneFacilityManager.Fields.GetValues(typeof(EquipZoneFacilityManager.Fields)))
            {
                string strFieldName = EquipZoneFacilityManager.GetFieldName(field, out isNullable);

                if (field == EquipZoneFacilityManager.Fields.ID)
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
                else if (field == EquipZoneFacilityManager.Fields.MemberID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.MemberID = data.Data;
                    }
                }
                else if (field == EquipZoneFacilityManager.Fields.MemberType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.MemberType = data.Data;
                    }
                }
                else if (field == EquipZoneFacilityManager.Fields.FacilityType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.FacilityType = data.Data;
                    }
                }
                else if (field == EquipZoneFacilityManager.Fields.DetectType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.DetectType = data.Data;
                    }
                }
                else if (field == EquipZoneFacilityManager.Fields.EquipZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.EquipZoneID = data.Data;
                    }
                }
                else if (field == EquipZoneFacilityManager.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }
                else if (field == EquipZoneFacilityManager.Fields.Description)
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
                    {
                        model.Description = data;
                    }
                }

                index++;
            }

            return model;
        }

        public CCTV SelectCCTV(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<CCTV.Fields>(out nFieldCount), CCTV.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                CCTV model = ReadCCTV(arrResult, 0, out strErrorMessage);

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

        public List<CCTV> SelectCCTVs(Dictionary<CCTV.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectCCTVs(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<CCTV> SelectCCTVs(Dictionary<CCTV.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<CCTV.Fields>(out nFieldCount), CCTV.TableName);

            string strCondition = "";

            if (SetCondition<CCTV.Fields>(ref strCondition, dicConditions, CCTV.GetFieldName, CCTV.TableName, ref strErrorMessage) == false)
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
            List<CCTV> cctvs = new List<CCTV>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                CCTV model = ReadCCTV(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    cctvs.Add(model);
            }

            return cctvs;
        }

        private CCTV ReadCCTV(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            CCTV model = new CCTV();
            bool isNullable;

            foreach (CCTV.Fields field in CCTV.Fields.GetValues(typeof(CCTV.Fields)))
            {
                string strFieldName = CCTV.GetFieldName(field, out isNullable);

                if (field == CCTV.Fields.ID)
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
                else if (field == CCTV.Fields.CameraName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.CameraName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.CameraName = data;
                    }
                }
                else if (field == CCTV.Fields.PositionName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.PositionName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.PositionName = data;
                    }
                }
                else if (field == CCTV.Fields.UniqueKey)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.UniqueKey = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.UniqueKey = data;
                    }
                }
                else if (field == CCTV.Fields.X)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.X = null;
                    }
                    else
                    {
                        model.X = data.Data;
                    }
                }
                else if (field == CCTV.Fields.Y)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Y = null;
                    }
                    else
                    {
                        model.Y = data.Data;
                    }
                }
                else if (field == CCTV.Fields.Z)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Z = null;
                    }
                    else
                    {
                        model.Z = data.Data;
                    }
                }
                else if (field == CCTV.Fields.ZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.ZoneID = null;
                    }
                    else
                    {
                        model.ZoneID = data.Data;
                    }
                }
                else if (field == CCTV.Fields.IsIndoor)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.IsIndoor = data.Data == 1;
                    }
                }
                else if (field == CCTV.Fields.Type)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Type = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Type = data;
                    }
                }
                else if (field == CCTV.Fields.Channel)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Channel = null;
                    }
                    else
                    {
                        model.Channel = data.Data;
                    }
                }
                else if (field == CCTV.Fields.UserID)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.UserID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.UserID = data;
                    }
                }
                else if (field == CCTV.Fields.Password)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Password = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Password = data;
                    }
                }
                else if (field == CCTV.Fields.URL)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.URL = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.URL = data;
                    }
                }
                else if (field == CCTV.Fields.BigURL)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.BigURL = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.BigURL = data;
                    }
                }
                else if (field == CCTV.Fields.SmallURL)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.SmallURL = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.SmallURL = data;
                    }
                }
                else if (field == CCTV.Fields.Enabled)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.Enabled = null;
                    }
                    else
                    {
                        model.Enabled = data.Data == 1;
                    }
                }
                else if (field == CCTV.Fields.CameraIP)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.CameraIP = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.CameraIP = data;
                    }
                }
                else if (field == CCTV.Fields.CameraCompanyName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.CameraCompanyName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.CameraCompanyName = data;
                    }
                }
                else if (field == CCTV.Fields.CameraModelName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.CameraModelName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.CameraModelName = data;
                    }
                }
                else if (field == CCTV.Fields.Description)
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
                    {
                        model.Description = data;
                    }
                }

                index++;
            }

            return model;
        }

        public EquipZoneCCTV SelectEquipZoneCCTV(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<EquipZoneCCTV.Fields>(out nFieldCount), EquipZoneCCTV.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                EquipZoneCCTV model = ReadEquipZoneCCTV(arrResult, 0, out strErrorMessage);

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

        public List<EquipZoneCCTV> SelectEquipZoneCCTVs(Dictionary<EquipZoneCCTV.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectEquipZoneCCTVs(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<EquipZoneCCTV> SelectEquipZoneCCTVs(Dictionary<EquipZoneCCTV.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<EquipZoneCCTV.Fields>(out nFieldCount), EquipZoneCCTV.TableName);

            string strCondition = "";

            if (SetCondition<EquipZoneCCTV.Fields>(ref strCondition, dicConditions, EquipZoneCCTV.GetFieldName, EquipZoneCCTV.TableName, ref strErrorMessage) == false)
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
            List<EquipZoneCCTV> cctvs = new List<EquipZoneCCTV>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                EquipZoneCCTV model = ReadEquipZoneCCTV(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    cctvs.Add(model);
            }

            return cctvs;
        }

        private EquipZoneCCTV ReadEquipZoneCCTV(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            EquipZoneCCTV model = new EquipZoneCCTV();
            bool isNullable;

            foreach (EquipZoneCCTV.Fields field in EquipZoneCCTV.Fields.GetValues(typeof(EquipZoneCCTV.Fields)))
            {
                string strFieldName = EquipZoneCCTV.GetFieldName(field, out isNullable);

                if (field == EquipZoneCCTV.Fields.ID)
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
                else if (field == EquipZoneCCTV.Fields.EquipZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.EquipZoneID = data.Data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.CCTV1)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.CCTV1 = null;
                    }
                    else
                    {
                        model.CCTV1 = data.Data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.CCTV2)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.CCTV2 = null;
                    }
                    else
                    {
                        model.CCTV2 = data.Data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.CCTV3)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.CCTV3 = null;
                    }
                    else
                    {
                        model.CCTV3 = data.Data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.CCTV4)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.CCTV4 = null;
                    }
                    else
                    {
                        model.CCTV4 = data.Data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.CCTV5)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.CCTV5 = null;
                    }
                    else
                    {
                        model.CCTV5 = data.Data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.CCTV6)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.CCTV6 = null;
                    }
                    else
                    {
                        model.CCTV6 = data.Data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.Preset1)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Preset1 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Preset1 = data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.Preset2)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Preset2 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Preset2 = data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.Preset3)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Preset3 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Preset3 = data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.Preset4)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Preset4 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Preset4 = data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.Preset5)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Preset5 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Preset5 = data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.Preset6)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Preset6 = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Preset6 = data;
                    }
                }
                else if (field == EquipZoneCCTV.Fields.Description)
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
                    {
                        model.Description = data;
                    }
                }

                index++;
            }

            return model;
        }

        public Model.GLTF.Model SelectGltfModel(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Model.GLTF.Model.Fields>(out nFieldCount), Model.GLTF.Model.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Model.GLTF.Model model = ReadGltfModel(arrResult, 0, out strErrorMessage);

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

        public List<Model.GLTF.Model> SelectGltfModels(Dictionary<Model.GLTF.Model.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectGltfModels(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Model.GLTF.Model> SelectGltfModels(Dictionary<Model.GLTF.Model.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Model.GLTF.Model.Fields>(out nFieldCount), Model.GLTF.Model.TableName);

            string strCondition = "";

            if (SetCondition<Model.GLTF.Model.Fields>(ref strCondition, dicConditions, Model.GLTF.Model.GetFieldName, Model.GLTF.Model.TableName, ref strErrorMessage) == false)
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
            List<Model.GLTF.Model> models = new List<Model.GLTF.Model>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Model.GLTF.Model model = ReadGltfModel(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    models.Add(model);
            }

            return models;
        }

        private Model.GLTF.Model ReadGltfModel(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Model.GLTF.Model model = new Model.GLTF.Model();
            bool isNullable;

            foreach (Model.GLTF.Model.Fields field in Model.GLTF.Model.Fields.GetValues(typeof(Model.GLTF.Model.Fields)))
            {
                string strFieldName = Model.GLTF.Model.GetFieldName(field, out isNullable);

                if (field == Model.GLTF.Model.Fields.ID)
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
                else if (field == Model.GLTF.Model.Fields.ParentID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.ParentID = null;
                    }
                    else
                    {
                        model.ParentID = data.Data;
                    }
                }
                else if (field == Model.GLTF.Model.Fields.ModelName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.ModelName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.ModelName = data;
                    }
                }
                else if (field == Model.GLTF.Model.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public Model.GLTF.ModelData SelectGltfModelData(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Model.GLTF.ModelData.Fields>(out nFieldCount), Model.GLTF.ModelData.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Model.GLTF.ModelData modelData = ReadGltfModelData(arrResult, 0, out strErrorMessage);

                if (modelData == null)
                    return null;

                return modelData;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<Model.GLTF.ModelData> SelectGltfModelDatas(Dictionary<Model.GLTF.ModelData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectGltfModelDatas(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Model.GLTF.ModelData> SelectGltfModelDatas(Dictionary<Model.GLTF.ModelData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Model.GLTF.ModelData.Fields>(out nFieldCount), Model.GLTF.ModelData.TableName);

            string strCondition = "";

            if (SetCondition<Model.GLTF.ModelData.Fields>(ref strCondition, dicConditions, Model.GLTF.ModelData.GetFieldName, Model.GLTF.ModelData.TableName, ref strErrorMessage) == false)
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
            List<Model.GLTF.ModelData> modelDatas = new List<Model.GLTF.ModelData>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Model.GLTF.ModelData modelData = ReadGltfModelData(arrResult, i, out strErrorMessage);

                if (modelData == null)
                    return null;
                else
                    modelDatas.Add(modelData);
            }

            return modelDatas;
        }

        private Model.GLTF.ModelData ReadGltfModelData(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Model.GLTF.ModelData model = new Model.GLTF.ModelData();
            bool isNullable;

            foreach (Model.GLTF.ModelData.Fields field in Model.GLTF.ModelData.Fields.GetValues(typeof(Model.GLTF.ModelData.Fields)))
            {
                string strFieldName = Model.GLTF.ModelData.GetFieldName(field, out isNullable);

                if (field == Model.GLTF.ModelData.Fields.ID)
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
                else if (field == Model.GLTF.ModelData.Fields.ModelID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ModelID = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.ModelFile)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.ModelFile = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.ModelFile = data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.ModelDisplayText)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.ModelDisplayText = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.ModelDisplayText = data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraPositionX)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraPositionX = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraPositionY)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraPositionY = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraPositionZ)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraPositionZ = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraQuaternionX)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraQuaternionX = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraQuaternionY)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraQuaternionY = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraQuaternionZ)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraQuaternionZ = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraQuaternionW)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraQuaternionW = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraRotationX)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraRotationX = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraRotationY)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraRotationY = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraRotationZ)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraRotationZ = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraFov)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraFov = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraNear)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraNear = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.CameraFar)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraFar = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.OrbitTargetX)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.OrbitTargetX = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.OrbitTargetY)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.OrbitTargetY = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.OrbitTargetZ)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.OrbitTargetZ = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.FloorIndex)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.FloorIndex = null;
                    }
                    else
                    {
                        model.FloorIndex = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.BuildingGroupID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.BuildingGroupID = null;
                    }
                    else
                    {
                        model.BuildingGroupID = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.BuildingID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.BuildingID = null;
                    }
                    else
                    {
                        model.BuildingID = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelData.Fields.ZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.ZoneID = null;
                    }
                    else
                    {
                        model.ZoneID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public Model.GLTF.ModelOrthoData SelectGltfModelOrthoData(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Model.GLTF.ModelOrthoData.Fields>(out nFieldCount), Model.GLTF.ModelOrthoData.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Model.GLTF.ModelOrthoData modelData = ReadGltfModelOrthoData(arrResult, 0, out strErrorMessage);

                if (modelData == null)
                    return null;

                return modelData;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<Model.GLTF.ModelOrthoData> SelectGltfModelOrthoDatas(Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectGltfModelOrthoDatas(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Model.GLTF.ModelOrthoData> SelectGltfModelOrthoDatas(Dictionary<Model.GLTF.ModelOrthoData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Model.GLTF.ModelOrthoData.Fields>(out nFieldCount), Model.GLTF.ModelOrthoData.TableName);

            string strCondition = "";

            if (SetCondition<Model.GLTF.ModelOrthoData.Fields>(ref strCondition, dicConditions, Model.GLTF.ModelOrthoData.GetFieldName, Model.GLTF.ModelOrthoData.TableName, ref strErrorMessage) == false)
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
            List<Model.GLTF.ModelOrthoData> modelDatas = new List<Model.GLTF.ModelOrthoData>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Model.GLTF.ModelOrthoData modelData = ReadGltfModelOrthoData(arrResult, i, out strErrorMessage);

                if (modelData == null)
                    return null;
                else
                    modelDatas.Add(modelData);
            }

            return modelDatas;
        }

        private Model.GLTF.ModelOrthoData ReadGltfModelOrthoData(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Model.GLTF.ModelOrthoData model = new Model.GLTF.ModelOrthoData();
            bool isNullable;

            foreach (Model.GLTF.ModelOrthoData.Fields field in Model.GLTF.ModelOrthoData.Fields.GetValues(typeof(Model.GLTF.ModelOrthoData.Fields)))
            {
                string strFieldName = Model.GLTF.ModelOrthoData.GetFieldName(field, out isNullable);

                if (field == Model.GLTF.ModelOrthoData.Fields.ID)
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
                else if (field == Model.GLTF.ModelOrthoData.Fields.ModelID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ModelID = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.ModelFile)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.ModelFile = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.ModelFile = data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.CameraPositionX)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraPositionX = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.CameraPositionY)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraPositionY = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.CameraPositionZ)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraPositionZ = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.CameraQuaternionX)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraQuaternionX = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.CameraQuaternionY)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraQuaternionY = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.CameraQuaternionZ)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraQuaternionZ = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.CameraQuaternionW)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraQuaternionW = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.CameraRotationX)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraRotationX = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.CameraRotationY)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraRotationY = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.CameraRotationZ)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.CameraRotationZ = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.TargetX)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.TargetX = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.TargetY)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.TargetY = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.TargetZ)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.TargetZ = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.Zoom)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.Zoom = data.Data;
                    }
                }
                else if (field == Model.GLTF.ModelOrthoData.Fields.ZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.ZoneID = null;
                    }
                    else
                    {
                        model.ZoneID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public Model.Sensor.Option.Etc SelectOptionEtcSensor(int sensorType, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3}",
                GetFieldNames<Model.Sensor.Option.Etc.Fields>(out nFieldCount),
                Model.Sensor.Option.Etc.TableName,
                Model.Sensor.Option.Etc.GetFieldName(Model.Sensor.Option.Etc.Fields.SensorType, out isNullable),
                sensorType);

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Model.Sensor.Option.Etc option = ReadOptionEtcSensor(arrResult, 0, out strErrorMessage);

                if (option == null)
                    return null;

                return option;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<Model.Sensor.Option.Etc> SelectOptionEtcSensors(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectOptionEtcSensors(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Model.Sensor.Option.Etc> SelectOptionEtcSensors(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Model.Sensor.Option.Etc.Fields>(out nFieldCount), Model.Sensor.Option.Etc.TableName);

            string strCondition = "";

            if (SetCondition<Model.Sensor.Option.Etc.Fields>(ref strCondition, dicConditions, Model.Sensor.Option.Etc.GetFieldName, Model.Sensor.Option.Etc.TableName, ref strErrorMessage) == false)
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
            List<Model.Sensor.Option.Etc> options = new List<Model.Sensor.Option.Etc>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Model.Sensor.Option.Etc option = ReadOptionEtcSensor(arrResult, i, out strErrorMessage);

                if (option == null)
                    return null;
                else
                    options.Add(option);
            }

            return options;
        }

        private Model.Sensor.Option.Etc ReadOptionEtcSensor(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Model.Sensor.Option.Etc model = new Model.Sensor.Option.Etc();
            bool isNullable;

            foreach (Model.Sensor.Option.Etc.Fields field in Model.Sensor.Option.Etc.Fields.GetValues(typeof(Model.Sensor.Option.Etc.Fields)))
            {
                string strFieldName = Model.Sensor.Option.Etc.GetFieldName(field, out isNullable);

                if (field == Model.Sensor.Option.Etc.Fields.SensorType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SensorType = data.Data;
                    }
                }
                else if (field == Model.Sensor.Option.Etc.Fields.DataType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.DataType = data.Data;
                    }
                }
                else if (field == Model.Sensor.Option.Etc.Fields.CloseAlarmSeconds)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.CloseAlarmSeconds = null;
                    }
                    else
                    {
                        model.CloseAlarmSeconds = data.Data;
                    }
                }
                else if (field == Model.Sensor.Option.Etc.Fields.DelaySeconds)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.DelaySeconds = null;
                    }
                    else
                    {
                        model.DelaySeconds = data.Data;
                    }
                }
                else if (field == Model.Sensor.Option.Etc.Fields.SiteID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SiteID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public Model.Sensor.Option.EtcData SelectOptionEtcSensorData(int sensorType, int alarmDepth, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3} and {4} = {5}",
                GetFieldNames<Model.Sensor.Option.EtcData.Fields>(out nFieldCount),
                Model.Sensor.Option.EtcData.TableName,
                Model.Sensor.Option.EtcData.GetFieldName(Model.Sensor.Option.EtcData.Fields.SensorType, out isNullable),
                sensorType,
                Model.Sensor.Option.EtcData.GetFieldName(Model.Sensor.Option.EtcData.Fields.AlarmDepth, out isNullable),
                alarmDepth);

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Model.Sensor.Option.EtcData option = ReadOptionEtcSensorData(arrResult, 0, out strErrorMessage);

                if (option == null)
                    return null;

                return option;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<Model.Sensor.Option.EtcData> SelectOptionEtcSensorDatas(Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectOptionEtcSensorDatas(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Model.Sensor.Option.EtcData> SelectOptionEtcSensorDatas(Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Model.Sensor.Option.EtcData.Fields>(out nFieldCount), Model.Sensor.Option.EtcData.TableName);

            string strCondition = "";

            if (SetCondition<Model.Sensor.Option.EtcData.Fields>(ref strCondition, dicConditions, Model.Sensor.Option.EtcData.GetFieldName, Model.Sensor.Option.EtcData.TableName, ref strErrorMessage) == false)
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
            List<Model.Sensor.Option.EtcData> options = new List<Model.Sensor.Option.EtcData>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Model.Sensor.Option.EtcData option = ReadOptionEtcSensorData(arrResult, i, out strErrorMessage);

                if (option == null)
                    return null;
                else
                    options.Add(option);
            }

            return options;
        }

        private Model.Sensor.Option.EtcData ReadOptionEtcSensorData(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Model.Sensor.Option.EtcData model = new Model.Sensor.Option.EtcData();
            bool isNullable;

            foreach (Model.Sensor.Option.EtcData.Fields field in Model.Sensor.Option.EtcData.Fields.GetValues(typeof(Model.Sensor.Option.EtcData.Fields)))
            {
                string strFieldName = Model.Sensor.Option.EtcData.GetFieldName(field, out isNullable);

                if (field == Model.Sensor.Option.EtcData.Fields.SensorType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SensorType = data.Data;
                    }
                }
                else if (field == Model.Sensor.Option.EtcData.Fields.AlarmDepth)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.AlarmDepth = data.Data;
                    }
                }
                else if (field == Model.Sensor.Option.EtcData.Fields.DataMini)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.DataMini = null;
                    }
                    else
                    {
                        model.DataMini = data.Data;
                    }
                }
                else if (field == Model.Sensor.Option.EtcData.Fields.DataMinf)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.DataMinf = null;
                    }
                    else
                    {
                        model.DataMinf = data.Data;
                    }
                }
                else if (field == Model.Sensor.Option.EtcData.Fields.DataMins)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.DataMins = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.DataMins = data;
                    }
                }
                else if (field == Model.Sensor.Option.EtcData.Fields.DataMaxi)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.DataMaxi = null;
                    }
                    else
                    {
                        model.DataMaxi = data.Data;
                    }
                }
                else if (field == Model.Sensor.Option.EtcData.Fields.DataMaxf)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.DataMaxf = null;
                    }
                    else
                    {
                        model.DataMaxf = data.Data;
                    }
                }
                else if (field == Model.Sensor.Option.EtcData.Fields.DataMaxs)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.DataMaxs = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.DataMaxs = data;
                    }
                }
                else if (field == Model.Sensor.Option.EtcData.Fields.LinkedBuildingIDs)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.LinkedBuildingIDs = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.LinkedBuildingIDs = StringToIntList(data);
                    }
                }
                else if (field == Model.Sensor.Option.EtcData.Fields.LinkedZoneIDs)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.LinkedZoneIDs = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.LinkedZoneIDs = StringToIntList(data);
                    }
                }
                else if (field == Model.Sensor.Option.EtcData.Fields.SendSDMS)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.SendSDMS = data.Data == 1;
                    }
                }

                index++;
            }

            return model;
        }

        public Info SelectFacilityInfo(string strModelName, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = '{3}'",
                GetFieldNames<Info.Fields>(out nFieldCount),
                Info.TableName,
                Info.GetFieldName(Info.Fields.ModelName, out isNullable),
                strModelName);

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Info info = ReadFacilityInfo(arrResult, 0, out strErrorMessage);

                if (info == null)
                    return null;

                return info;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<Info> SelectFacilityInfos(Dictionary<Info.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectFacilityInfos(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Info> SelectFacilityInfos(Dictionary<Info.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Info.Fields>(out nFieldCount), Info.TableName);

            string strCondition = "";

            if (SetCondition<Info.Fields>(ref strCondition, dicConditions, Info.GetFieldName, Info.TableName, ref strErrorMessage) == false)
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
            List<Info> infos = new List<Info>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Info info = ReadFacilityInfo(arrResult, i, out strErrorMessage);

                if (info == null)
                    return null;
                else
                    infos.Add(info);
            }

            return infos;
        }

        private Info ReadFacilityInfo(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Info model = new Info();
            bool isNullable;

            foreach (Info.Fields field in Info.Fields.GetValues(typeof(Info.Fields)))
            {
                string strFieldName = Info.GetFieldName(field, out isNullable);

                if (field == Info.Fields.ID)
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
                else if (field == Info.Fields.ModelName)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.ModelName = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.ModelName = data;
                    }
                }
                else if (field == Info.Fields.FacilityName)
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
                    {
                        model.FacilityName = data;
                    }
                }
                else if (field == Info.Fields.ZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ZoneID = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public InfoData SelectFacilityInfoData(int nFacilityInfoID, int nOrderIndex, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3} and {4} = {5}",
                GetFieldNames<InfoData.Fields>(out nFieldCount),
                InfoData.TableName,
                InfoData.GetFieldName(InfoData.Fields.FacilityInfoID, out isNullable),
                nFacilityInfoID,
                InfoData.GetFieldName(InfoData.Fields.OrderIndex, out isNullable),
                nOrderIndex);

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                InfoData data = ReadFacilityInfoData(arrResult, 0, out strErrorMessage);

                if (data == null)
                    return null;

                return data;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<InfoData> SelectFacilityInfoDatas(Dictionary<InfoData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectFacilityInfoDatas(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<InfoData> SelectFacilityInfoDatas(Dictionary<InfoData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<InfoData.Fields>(out nFieldCount), InfoData.TableName);

            string strCondition = "";

            if (SetCondition<InfoData.Fields>(ref strCondition, dicConditions, InfoData.GetFieldName, InfoData.TableName, ref strErrorMessage) == false)
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
            List<InfoData> datas = new List<InfoData>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                InfoData data = ReadFacilityInfoData(arrResult, i, out strErrorMessage);

                if (data == null)
                    return null;
                else
                    datas.Add(data);
            }

            return datas;
        }

        private InfoData ReadFacilityInfoData(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            InfoData model = new InfoData();
            bool isNullable;

            foreach (InfoData.Fields field in InfoData.Fields.GetValues(typeof(InfoData.Fields)))
            {
                string strFieldName = InfoData.GetFieldName(field, out isNullable);

                if (field == InfoData.Fields.FacilityInfoID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.FacilityInfoID = data.Data;
                    }
                }
                else if (field == InfoData.Fields.OrderIndex)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.OrderIndex = data.Data;
                    }
                }
                else if (field == InfoData.Fields.Value)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Value = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Value = data;
                    }
                }
                else if (field == InfoData.Fields.WithDot)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.WithDot = data.Data == 1;
                    }
                }
                else if (field == InfoData.Fields.IndentDepth)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IndentDepth = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.IndentDepth = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public BuildingData SelectBuildingData(int nBuildingID, int nOrderIndex, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3} and {4} = {5}",
                GetFieldNames<BuildingData.Fields>(out nFieldCount),
                BuildingData.TableName,
                BuildingData.GetFieldName(BuildingData.Fields.BuildingID, out isNullable),
                nBuildingID,
                BuildingData.GetFieldName(BuildingData.Fields.OrderIndex, out isNullable),
                nOrderIndex);

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                BuildingData data = ReadBuildingData(arrResult, 0, out strErrorMessage);

                if (data == null)
                    return null;

                return data;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<BuildingData> SelectBuildingDatas(Dictionary<BuildingData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectBuildingDatas(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<BuildingData> SelectBuildingDatas(Dictionary<BuildingData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<BuildingData.Fields>(out nFieldCount), BuildingData.TableName);

            string strCondition = "";

            if (SetCondition<BuildingData.Fields>(ref strCondition, dicConditions, BuildingData.GetFieldName, BuildingData.TableName, ref strErrorMessage) == false)
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
            List<BuildingData> datas = new List<BuildingData>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                BuildingData data = ReadBuildingData(arrResult, i, out strErrorMessage);

                if (data == null)
                    return null;
                else
                    datas.Add(data);
            }

            return datas;
        }

        private BuildingData ReadBuildingData(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            BuildingData model = new BuildingData();
            bool isNullable;

            foreach (BuildingData.Fields field in BuildingData.Fields.GetValues(typeof(BuildingData.Fields)))
            {
                string strFieldName = BuildingData.GetFieldName(field, out isNullable);

                if (field == BuildingData.Fields.BuildingID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.BuildingID = data.Data;
                    }
                }
                else if (field == BuildingData.Fields.OrderIndex)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.OrderIndex = data.Data;
                    }
                }
                else if (field == BuildingData.Fields.Value)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Value = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Value = data;
                    }
                }
                else if (field == BuildingData.Fields.WithDot)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.WithDot = data.Data == 1;
                    }
                }
                else if (field == BuildingData.Fields.IndentDepth)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IndentDepth = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.IndentDepth = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public BuildingGroupData SelectBuildingGroupData(int nBuildingGroupID, int nOrderIndex, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3} and {4} = {5}",
                GetFieldNames<BuildingGroupData.Fields>(out nFieldCount),
                BuildingGroupData.TableName,
                BuildingGroupData.GetFieldName(BuildingGroupData.Fields.BuildingGroupID, out isNullable),
                nBuildingGroupID,
                BuildingGroupData.GetFieldName(BuildingGroupData.Fields.OrderIndex, out isNullable),
                nOrderIndex);

            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                BuildingGroupData data = ReadBuildingGroupData(arrResult, 0, out strErrorMessage);

                if (data == null)
                    return null;

                return data;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<BuildingGroupData> SelectBuildingGroupDatas(Dictionary<BuildingGroupData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectBuildingGroupDatas(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<BuildingGroupData> SelectBuildingGroupDatas(Dictionary<BuildingGroupData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<BuildingGroupData.Fields>(out nFieldCount), BuildingGroupData.TableName);

            string strCondition = "";

            if (SetCondition<BuildingGroupData.Fields>(ref strCondition, dicConditions, BuildingGroupData.GetFieldName, BuildingGroupData.TableName, ref strErrorMessage) == false)
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
            List<BuildingGroupData> datas = new List<BuildingGroupData>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                BuildingGroupData data = ReadBuildingGroupData(arrResult, i, out strErrorMessage);

                if (data == null)
                    return null;
                else
                    datas.Add(data);
            }

            return datas;
        }

        private BuildingGroupData ReadBuildingGroupData(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            BuildingGroupData model = new BuildingGroupData();
            bool isNullable;

            foreach (BuildingGroupData.Fields field in BuildingGroupData.Fields.GetValues(typeof(BuildingGroupData.Fields)))
            {
                string strFieldName = BuildingGroupData.GetFieldName(field, out isNullable);

                if (field == BuildingGroupData.Fields.BuildingGroupID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.BuildingGroupID = data.Data;
                    }
                }
                else if (field == BuildingGroupData.Fields.OrderIndex)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.OrderIndex = data.Data;
                    }
                }
                else if (field == BuildingGroupData.Fields.Value)
                {
                    string data = WebDBManager.GetStringField(arrResult[index]);

                    if (data == null)
                    {
                        if (isNullable)
                            model.Value = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.Value = data;
                    }
                }
                else if (field == BuildingGroupData.Fields.WithDot)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.WithDot = data.Data == 1;
                    }
                }
                else if (field == BuildingGroupData.Fields.IndentDepth)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.IndentDepth = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.IndentDepth = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        /// <summary>
        /// 현재 진행중인 알람에 대한 정보들을 얻어온다. 
        /// ArrayList에는 EquipmentZone, SensorReactionHistory, SensorZone, SensorZoneHistory 순서대로 객체들이 담겨진다.</summary>
        /// <param name="strAlarmOnReactionTypes"></param>
        /// <param name="strAlarmOffReactionTypes"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public ArrayList SelectCurrentAlarmHistories(string strAlarmOnReactionTypes, string strAlarmOffReactionTypes, out string strErrorMessage)
        {
            return SelectCurrentAlarmHistories(strAlarmOnReactionTypes, strAlarmOffReactionTypes, null, out strErrorMessage);
        }

        public ArrayList SelectCurrentAlarmHistories(string strAlarmOnReactionTypes, string strAlarmOffReactionTypes, int? topNCount, out string strErrorMessage)
        {
            bool isNullable;
            string strAlarmOnSubQuery = "", strAlarmOffSubQuery = "", strCondition = "";

            if (strAlarmOnReactionTypes != null && strAlarmOnReactionTypes.Length > 0)
            {
                strAlarmOnSubQuery = string.Format("{0} in (SELECT {0} FROM {1} WHERE {2} in ({3}))",
                    SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.SensorZoneHistoryID, out isNullable),
                    SensorReactionHistory.TableName,
                    SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.ReactionType, out isNullable),
                    strAlarmOnReactionTypes);
            }

            if (strAlarmOffReactionTypes != null && strAlarmOffReactionTypes.Length > 0)
            {
                strAlarmOnSubQuery = string.Format("{0} not in (SELECT {0} FROM {1} WHERE {2} in ({3}))",
                    SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.SensorZoneHistoryID, out isNullable),
                    SensorReactionHistory.TableName,
                    SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.ReactionType, out isNullable),
                    strAlarmOffReactionTypes);
            }

            if (strAlarmOnSubQuery.Length > 0)
                strCondition = strAlarmOnSubQuery;

            if (strAlarmOffSubQuery.Length > 0)
            {
                if (strCondition.Length > 0)
                    strCondition += " and " + strAlarmOffSubQuery;
                else
                    strCondition = strAlarmOffSubQuery;
            }

            if (strCondition.Length == 0)
            {
                strErrorMessage = "알람 조건을 찾을수 없습니다.";
                return null;
            }
            else
            {
                strCondition += string.Format(" group by {0}, {1}",
                    SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.SensorZoneHistoryID, out isNullable),
                    SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.ReactionType, out isNullable));
            }

            string strSubQuery = string.Format("Select max({0}) from {1} where {2}",
                SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.ID, out isNullable),
                SensorReactionHistory.TableName,
                strCondition);

            strCondition = string.Format("{0}.{1} in ({2}) and {3}.{4} = {5} ORDER BY {0}.{6}, {7}.{8}",
                SensorReactionHistory.TableName,
                SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.ID, out isNullable),
                strSubQuery,
                EquipmentZone.TableName,
                EquipmentZone.GetFieldName(EquipmentZone.Fields.SiteID, out isNullable),
                m_dataManager.SiteID,
                SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.Time, out isNullable),
                SensorZoneHistory.TableName,
                SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.Time, out isNullable));

            return JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory(null, null, null, null, strCondition, topNCount, out strErrorMessage);
        }

        public FakeWall SelectFakeWall(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<FakeWall.Fields>(out nFieldCount), FakeWall.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                FakeWall wall = ReadFakeWall(arrResult, 0, out strErrorMessage);

                if (wall == null)
                    return null;

                return wall;
            }
            else
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
            }

            return null;
        }

        public List<FakeWall> SelectFakeWalls(Dictionary<FakeWall.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectFakeWalls(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<FakeWall> SelectFakeWalls(Dictionary<FakeWall.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<FakeWall.Fields>(out nFieldCount), FakeWall.TableName);

            string strCondition = "";

            if (SetCondition<FakeWall.Fields>(ref strCondition, dicConditions, FakeWall.GetFieldName, FakeWall.TableName, ref strErrorMessage) == false)
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
            List<FakeWall> datas = new List<FakeWall>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                FakeWall data = ReadFakeWall(arrResult, i, out strErrorMessage);

                if (data == null)
                    return null;
                else
                    datas.Add(data);
            }

            return datas;
        }

        private FakeWall ReadFakeWall(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            FakeWall model = new FakeWall();
            bool isNullable;

            foreach (FakeWall.Fields field in FakeWall.Fields.GetValues(typeof(FakeWall.Fields)))
            {
                string strFieldName = FakeWall.GetFieldName(field, out isNullable);

                if (field == FakeWall.Fields.ID)
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
                else if (field == FakeWall.Fields.ZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ZoneID = data.Data;
                    }
                }
                else if (field == FakeWall.Fields.X)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.X = data.Data;
                    }
                }
                else if (field == FakeWall.Fields.Y)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.Y = data.Data;
                    }
                }
                else if (field == FakeWall.Fields.Z)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.Z = data.Data;
                    }
                }
                else if (field == FakeWall.Fields.Rotate)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.Rotate = data.Data;
                    }
                }
                else if (field == FakeWall.Fields.Scale)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.Scale = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public Model.Config.SpreadMessage SelectSpreadMessage(int id, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1} where ID = {2}", GetFieldNames<Model.Config.SpreadMessage.Fields>(out nFieldCount), Model.Config.SpreadMessage.TableName, id);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                Model.Config.SpreadMessage model = ReadSpreadMessage(arrResult, 0, out strErrorMessage);

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

        private Model.Config.SpreadMessage ReadSpreadMessage(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            Model.Config.SpreadMessage model = new Model.Config.SpreadMessage();
            bool isNullable;

            foreach (Model.Config.SpreadMessage.Fields field in Model.Config.SpreadMessage.Fields.GetValues(typeof(Model.Config.SpreadMessage.Fields)))
            {
                string strFieldName = Model.Config.SpreadMessage.GetFieldName(field, out isNullable);

                if (field == Model.Config.SpreadMessage.Fields.ID)
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
                else if (field == Model.Config.SpreadMessage.Fields.FacilityType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.FacilityType = data.Data;
                    }
                }
                else if (field == Model.Config.SpreadMessage.Fields.BuilidingGroupID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.BuildingGroupID = null;
                    }
                    else
                    {
                        model.BuildingGroupID = data.Data;
                    }
                }
                else if (field == Model.Config.SpreadMessage.Fields.BuilidingID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        model.BuildingID = null;
                    }
                    else
                    {
                        model.BuildingID = data.Data;
                    }
                }
                else if (field == Model.Config.SpreadMessage.Fields.RegularID)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.RegularID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.RegularID = str;
                }
                else if (field == Model.Config.SpreadMessage.Fields.RegularMemberID)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.RegularMemberID = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.RegularMemberID = str;
                }
                else if (field == Model.Config.SpreadMessage.Fields.MessageType)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.MessageType = data.Data;
                    }
                }
                else if (field == Model.Config.SpreadMessage.Fields.Message)
                {
                    string str = WebDBManager.GetStringField(arrResult[index]);

                    if (str == null)
                    {
                        if (isNullable)
                            model.Message = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                        model.Message = str;
                }

                index++;
            }

            return model;
        }

        public List<Model.Config.SpreadMessage> SelectSpreadMessages(Dictionary<Model.Config.SpreadMessage.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectSpreadMessages(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<Model.Config.SpreadMessage> SelectSpreadMessages(Dictionary<Model.Config.SpreadMessage.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<Model.Config.SpreadMessage.Fields>(out nFieldCount), Model.Config.SpreadMessage.TableName);

            string strCondition = "";

            if (SetCondition<Model.Config.SpreadMessage.Fields>(ref strCondition, dicConditions, Model.Config.SpreadMessage.GetFieldName, Model.Config.SpreadMessage.TableName, ref strErrorMessage) == false)
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
            List<Model.Config.SpreadMessage> spreadMessages = new List<Model.Config.SpreadMessage>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                Model.Config.SpreadMessage model = ReadSpreadMessage(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    spreadMessages.Add(model);
            }

            return spreadMessages;
        }

        public ZoneData SelectZoneData(int zoneID, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;
            bool isNullable;

            string strSQL = string.Format("select {0} from {1} where {2} = {3}",
                GetFieldNames<ZoneData.Fields>(out nFieldCount),
                ZoneData.TableName,
                ZoneData.GetFieldName(ZoneData.Fields.ZoneID, out isNullable),
                zoneID);
            ArrayList arrResult = m_dbManager.GetResultData(strSQL);

            if (arrResult != null && arrResult.Count >= nFieldCount)
            {
                ZoneData model = ReadZoneData(arrResult, 0, out strErrorMessage);

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

        public List<ZoneData> SelectZoneDatas(Dictionary<ZoneData.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return SelectZoneDatas(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public List<ZoneData> SelectZoneDatas(Dictionary<ZoneData.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            int nFieldCount;

            string strSQL = string.Format("select {0} from {1}", GetFieldNames<ZoneData.Fields>(out nFieldCount), ZoneData.TableName);

            string strCondition = "";

            if (SetCondition<ZoneData.Fields>(ref strCondition, dicConditions, ZoneData.GetFieldName, ZoneData.TableName, ref strErrorMessage) == false)
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
            List<ZoneData> datas = new List<ZoneData>();

            for (int i = 0; i < nResultCount - (nFieldCount - 1); i += nFieldCount)
            {
                ZoneData model = ReadZoneData(arrResult, i, out strErrorMessage);

                if (model == null)
                    return null;
                else
                    datas.Add(model);
            }

            return datas;
        }

        private ZoneData ReadZoneData(ArrayList arrResult, int index, out string strErrorMessage)
        {
            strErrorMessage = null;
            ZoneData model = new ZoneData();
            bool isNullable;

            foreach (ZoneData.Fields field in ZoneData.Fields.GetValues(typeof(ZoneData.Fields)))
            {
                string strFieldName = ZoneData.GetFieldName(field, out isNullable);

                if (field == ZoneData.Fields.ZoneID)
                {
                    VariousData<int> data = WebDBManager.GetIntField(arrResult[index].ToString());

                    if (data == null)
                    {
                        strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                        return null;
                    }
                    else
                    {
                        model.ZoneID = data.Data;
                    }
                }
                else if (field == ZoneData.Fields.FakeWallElevation)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.FakeWallElevation = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.FakeWallElevation = data.Data;
                    }
                }
                else if (field == ZoneData.Fields.PoiElevation)
                {
                    VariousData<float> data = WebDBManager.GetFloatField(arrResult[index].ToString());

                    if (data == null)
                    {
                        if (isNullable)
                            model.PoiElevation = null;
                        else
                        {
                            strErrorMessage = string.Format("{0}는 null이 될수 없습니다.", strFieldName);
                            return null;
                        }
                    }
                    else
                    {
                        model.PoiElevation = data.Data;
                    }
                }

                index++;
            }

            return model;
        }

        public ArrayList JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory(Dictionary<EquipmentZone.Fields, object> dicConditions1, Dictionary<SensorReactionHistory.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, Dictionary<SensorZoneHistory.Fields, object> dicConditions4, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory(dicConditions1, dicConditions2, dicConditions3, dicConditions4, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory(Dictionary<EquipmentZone.Fields, object> dicConditions1, Dictionary<SensorReactionHistory.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, Dictionary<SensorZoneHistory.Fields, object> dicConditions4, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            string strEquipZoneTableName = EquipmentZone.TableName;
            string strSensorReactionHistoryTableName = SensorReactionHistory.TableName;
            string strSensorZoneTableName = SensorZone.TableName;
            string strSensorZoneHistoryTableName = SensorZoneHistory.TableName;
            //string strBuildingTableName = Building.TableName;
            string strZoneTableName = Zone.TableName;

            int nEquipZoneFieldCount, nSensorReactionHistoryFieldCount, nSensorZoneFieldCount, nSensorZoneHistoryFieldCount/*, nBuildingFieldCount*/, nZoneFieldCount;

            string strEquipZoneFields = GetFieldNames<EquipmentZone.Fields>(strEquipZoneTableName, out nEquipZoneFieldCount);
            string strSensorReactionHistoryFields = GetFieldNames<SensorReactionHistory.Fields>(strSensorReactionHistoryTableName, out nSensorReactionHistoryFieldCount);
            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strSensorZoneHistoryFields = GetFieldNames<SensorZoneHistory.Fields>(strSensorZoneHistoryTableName, out nSensorZoneHistoryFieldCount);
            //string strBuildingFields = GetFieldNames<Building.Fields>(strBuildingTableName, out nBuildingFieldCount);
            string strZoneFields = GetFieldNames<Zone.Fields>(strZoneTableName, out nZoneFieldCount);

            int nFieldsCount = nEquipZoneFieldCount + nSensorReactionHistoryFieldCount + nSensorZoneFieldCount + nSensorZoneHistoryFieldCount + /*nBuildingFieldCount +*/ nZoneFieldCount;
            bool isNullable;

            string strSQL = string.Format("Select {0}, {1}, {2}, {3}, {4} from {5}, {6}, {7}, {8}, {9} "
                , strEquipZoneFields, strSensorReactionHistoryFields, strSensorZoneFields, strSensorZoneHistoryFields, /*strBuildingFields, */strZoneFields
                , strEquipZoneTableName, strSensorReactionHistoryTableName, strSensorZoneTableName, strSensorZoneHistoryTableName, /*strBuildingTableName, */strZoneTableName);
            strSQL += string.Format(" where {0}.{1} = {2}.{3} and {2}.{4} = {5}.{6} and {5}.{7} = {8}.{9} and {8}.{10} = {11} and {12}.{13} = {2}.{14} ",
                strSensorReactionHistoryTableName,
                SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.SensorZoneHistoryID, out isNullable),
                strSensorZoneHistoryTableName,
                SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.ID, out isNullable),
                SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.SensorZoneID, out isNullable),
                strSensorZoneTableName,
                SensorZone.GetFieldName(SensorZone.Fields.ID, out isNullable),
                SensorZone.GetFieldName(SensorZone.Fields.EquipZoneID, out isNullable),
                strEquipZoneTableName,
                EquipmentZone.GetFieldName(EquipmentZone.Fields.ID, out isNullable),
                EquipmentZone.GetFieldName(EquipmentZone.Fields.SiteID, out isNullable),
                m_dataManager.SiteID,
                strZoneTableName,
                Zone.GetFieldName(Zone.Fields.ID, out isNullable),
                SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.ZoneID, out isNullable)
                //strBuildingTableName,
                //Building.GetFieldName(Building.Fields.ID, out isNullable),
                //Zone.GetFieldName(Zone.Fields.BuildingID, out isNullable)
                );

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                strSQL += " and " + strAdditionalConditions;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                EquipmentZone equipZone = ReadEquipmentZone(arrResult, i, out strErrorMessage);

                if (equipZone == null)
                    return null;
                else
                    arrDatas.Add(equipZone);

                SensorReactionHistory srh = ReadSensorReactionHistory(arrResult, i + nEquipZoneFieldCount, out strErrorMessage);

                if (equipZone == null)
                    return null;
                else
                    arrDatas.Add(srh);

                SensorZone sz = ReadSensorZone(arrResult, i + nEquipZoneFieldCount + nSensorReactionHistoryFieldCount, out strErrorMessage);

                if (sz == null)
                    return null;
                else
                    arrDatas.Add(sz);

                SensorZoneHistory szh = ReadSensorZoneHistory(arrResult, i + nEquipZoneFieldCount + nSensorReactionHistoryFieldCount + nSensorZoneFieldCount, out strErrorMessage);

                if (szh == null)
                    return null;
                else
                    arrDatas.Add(szh);

                //Building building = ReadBuilding(arrResult, i + nEquipZoneFieldCount + nSensorReactionHistoryFieldCount + nSensorZoneFieldCount + nSensorZoneHistoryFieldCount, out strErrorMessage);
                //if (building == null)
                //    return null;
                //else
                //    arrDatas.Add(building);

                Zone zone = ReadZone(arrResult, i + nEquipZoneFieldCount + nSensorReactionHistoryFieldCount + nSensorZoneFieldCount + nSensorZoneHistoryFieldCount /*+ nBuildingFieldCount*/, out strErrorMessage);
                if (zone == null)
                    return null;
                else
                    arrDatas.Add(zone);
            }

            strErrorMessage = null;
            return arrDatas;
        }

        public ArrayList JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory2(Dictionary<EquipmentZone.Fields, object> dicConditions1, Dictionary<SensorReactionHistory.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, Dictionary<SensorZoneHistory.Fields, object> dicConditions4, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory2(dicConditions1, dicConditions2, dicConditions3, dicConditions4, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinEquipmentZoneSensorReactionHistorySensorZoneSensorZoneHistory2(Dictionary<EquipmentZone.Fields, object> dicConditions1, Dictionary<SensorReactionHistory.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, Dictionary<SensorZoneHistory.Fields, object> dicConditions4, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            string strEquipZoneTableName = EquipmentZone.TableName;
            string strSensorReactionHistoryTableName = SensorReactionHistory.TableName;
            string strSensorZoneTableName = SensorZone.TableName;
            string strSensorZoneHistoryTableName = SensorZoneHistory.TableName;
            string strZoneTableName = Zone.TableName;

            int nEquipZoneFieldCount, nSensorReactionHistoryFieldCount, nSensorZoneFieldCount, nSensorZoneHistoryFieldCount/*, nBuildingFieldCount*/, nZoneFieldCount;

            string strEquipZoneFields = GetFieldNames<EquipmentZone.Fields>(strEquipZoneTableName, out nEquipZoneFieldCount);
            string strSensorReactionHistoryFields = GetFieldNames<SensorReactionHistory.Fields>(strSensorReactionHistoryTableName, out nSensorReactionHistoryFieldCount);
            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strSensorZoneHistoryFields = GetFieldNames<SensorZoneHistory.Fields>(strSensorZoneHistoryTableName, out nSensorZoneHistoryFieldCount);
            string strZoneFields = GetFieldNames<Zone.Fields>(strZoneTableName, out nZoneFieldCount);

            int nFieldsCount = nEquipZoneFieldCount + nSensorReactionHistoryFieldCount + nSensorZoneFieldCount + nSensorZoneHistoryFieldCount + nZoneFieldCount;
            bool isNullable;

            string strSQL = string.Format("Select {0}, {1}, {2}, {3}, {4} from {5}, {6}, {7}, {8}, {9} "
                , strEquipZoneFields, strSensorReactionHistoryFields, strSensorZoneFields, strSensorZoneHistoryFields, strZoneFields
                , strEquipZoneTableName, strSensorReactionHistoryTableName, strSensorZoneTableName, strSensorZoneHistoryTableName, strZoneTableName);
            strSQL += string.Format(" where {0}.{1} = {2}.{3} and {2}.{4} = {5}.{6} and {5}.{7} = {8}.{9} and {8}.{10} = {11} and {12}.{13} = {2}.{14}",
                strSensorReactionHistoryTableName,
                SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.SensorZoneHistoryID, out isNullable),
                strSensorZoneHistoryTableName,
                SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.ID, out isNullable),
                SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.SensorZoneID, out isNullable),
                strSensorZoneTableName,
                SensorZone.GetFieldName(SensorZone.Fields.ID, out isNullable),
                SensorZone.GetFieldName(SensorZone.Fields.EquipZoneID, out isNullable),
                strEquipZoneTableName,
                EquipmentZone.GetFieldName(EquipmentZone.Fields.ID, out isNullable),
                EquipmentZone.GetFieldName(EquipmentZone.Fields.SiteID, out isNullable),
                m_dataManager.SiteID,
                strZoneTableName,
                Zone.GetFieldName(Zone.Fields.ID, out isNullable),
                SensorZoneHistory.GetFieldName(SensorZoneHistory.Fields.ZoneID, out isNullable));

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                strSQL += " and " + strAdditionalConditions;
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(strSQL) : m_dbManager.GetResultData(strSQL, (int)topNCount);

            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                EquipmentZone equipZone = ReadEquipmentZone(arrResult, i, out strErrorMessage);

                if (equipZone == null)
                    return null;
                else
                    arrDatas.Add(equipZone);

                SensorReactionHistory srh = ReadSensorReactionHistory(arrResult, i + nEquipZoneFieldCount, out strErrorMessage);

                if (equipZone == null)
                    return null;
                else
                    arrDatas.Add(srh);

                SensorZone sz = ReadSensorZone(arrResult, i + nEquipZoneFieldCount + nSensorReactionHistoryFieldCount, out strErrorMessage);

                if (sz == null)
                    return null;
                else
                    arrDatas.Add(sz);

                SensorZoneHistory szh = ReadSensorZoneHistory(arrResult, i + nEquipZoneFieldCount + nSensorReactionHistoryFieldCount + nSensorZoneFieldCount, out strErrorMessage);

                if (szh == null)
                    return null;
                else
                    arrDatas.Add(szh);

                Zone zone = ReadZone(arrResult, i + nEquipZoneFieldCount + nSensorReactionHistoryFieldCount + nSensorZoneFieldCount + nSensorZoneHistoryFieldCount, out strErrorMessage);
                if (zone == null)
                    return null;
                else
                    arrDatas.Add(zone);
            }

            strErrorMessage = null;
            return arrDatas;
        }

        public ArrayList GetMinMaxIndexSensorReactionHistory(string strAdditionalConditions, out string strErrorMessage)
        {
            strErrorMessage = null;

            StringBuilder sb = new StringBuilder();
            sb.Append("Select Min(SdmsHistorySensorZone.ID), Max(SdmsHistorySensorZone.ID)");
            sb.Append("  From SdmsSpatialZone, SdmsHistorySensorZone, SdmsHistorySensorReaction");
            sb.Append(" Where SdmsSpatialZone.ID = SdmsHistorySensorZone.ZoneID");
            sb.Append("   And SdmsHistorySensorZone.ID=SdmsHistorySensorReaction.SensorZoneHistoryID");

            if (strAdditionalConditions.Length > 0)
                sb.AppendFormat(" And {0}", strAdditionalConditions);

            ArrayList arrResult = m_dbManager.GetResultData(sb.ToString());
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrResult2 = new ArrayList();
            if (arrResult.Count == 2)
            {
                VariousData<int> nMinID = WebDBManager.GetIntField(arrResult[0].ToString());
                VariousData<int> nMaxID = WebDBManager.GetIntField(arrResult[1].ToString());

                if (nMinID != null && nMaxID != null)
                {
                    arrResult2.Add(nMinID.Data);
                    arrResult2.Add(nMaxID.Data);
                }
            }

            return arrResult2;
        }

        /// <summary>
        /// Join SdmsHistorySensorReaction, SdmsSpatialEquipmentZone, SdmsSensorZone        
        /// </summary>
        public ArrayList JoinHistroysensorreactionSpatialequipmentzoneSensorZone(Dictionary<SensorReactionHistory.Fields, object> dicConditions1, Dictionary<EquipmentZone.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinHistroysensorreactionSpatialequipmentzoneSensorZone(dicConditions1, dicConditions2, dicConditions3, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinHistroysensorreactionSpatialequipmentzoneSensorZone(Dictionary<SensorReactionHistory.Fields, object> dicConditions1, Dictionary<EquipmentZone.Fields, object> dicConditions2, Dictionary<SensorZone.Fields, object> dicConditions3, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorReactionHistoryTableName = SensorReactionHistory.TableName;
            string strEquipZoneTableName = EquipmentZone.TableName;
            string strSensorZoneTableName = SensorZone.TableName;

            int nSensorReactionHistoryFieldCount, nEquipZoneFieldCount, nSensorZoneFieldCount;

            string strSensorReactionHistoryFields = GetFieldNames<SensorReactionHistory.Fields>(strSensorReactionHistoryTableName, out nSensorReactionHistoryFieldCount);
            string strEquipZoneFields = GetFieldNames<EquipmentZone.Fields>(strEquipZoneTableName, out nEquipZoneFieldCount);
            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);

            int nFieldsCount = nEquipZoneFieldCount + nSensorReactionHistoryFieldCount + nSensorZoneFieldCount;
            bool isNullable;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, {2} ", strSensorReactionHistoryFields, strEquipZoneFields, strSensorZoneFields);
            sb.AppendFormat("  From {0}, {1}, {2} ", strSensorReactionHistoryTableName, strEquipZoneTableName, strSensorZoneTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strSensorReactionHistoryTableName, SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.Param1, out isNullable)
                                                       , strEquipZoneTableName, EquipmentZone.GetFieldName(EquipmentZone.Fields.ID, out isNullable));
            sb.AppendFormat("   And {0}.{1} = {2}.{3} ", strSensorReactionHistoryTableName, SensorReactionHistory.GetFieldName(SensorReactionHistory.Fields.Param2, out isNullable)
                                                       , strSensorZoneTableName, SensorZone.GetFieldName(SensorZone.Fields.ID, out isNullable));

            string strCondition1 = "";
            if (SetCondition<SensorReactionHistory.Fields>(ref strCondition1, dicConditions1, SensorReactionHistory.GetFieldName, SensorReactionHistory.TableName, ref strErrorMessage) == false)
                return null;

            if (strCondition1.Length > 0)
                sb.AppendFormat(" and {0}", strCondition1);

            string strCondition2 = "";
            if (SetCondition<EquipmentZone.Fields>(ref strCondition2, dicConditions2, EquipmentZone.GetFieldName, EquipmentZone.TableName, ref strErrorMessage) == false)
                return null;

            if (strCondition2.Length > 0)
                sb.AppendFormat(" and {0}", strCondition2);

            string strCondition3 = "";
            if (SetCondition<SensorZone.Fields>(ref strCondition3, dicConditions3, SensorZone.GetFieldName, SensorZone.TableName, ref strErrorMessage) == false)
                return null;

            if (strCondition3.Length > 0)
                sb.AppendFormat(" and {0}", strCondition3);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorReactionHistory srh = ReadSensorReactionHistory(arrResult, i, out strErrorMessage);

                if (srh == null)
                    return null;
                else
                    arrDatas.Add(srh);

                EquipmentZone equipZone = ReadEquipmentZone(arrResult, i + nSensorReactionHistoryFieldCount, out strErrorMessage);

                if (equipZone == null)
                    return null;
                else
                    arrDatas.Add(equipZone);

                SensorZone sz = ReadSensorZone(arrResult, i + nEquipZoneFieldCount + nSensorReactionHistoryFieldCount, out strErrorMessage);

                if (sz == null)
                    return null;
                else
                    arrDatas.Add(sz);
            }

            return arrDatas;
        }

        public ArrayList JoinOptionEtcSensorOptionEtcSensorData(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions1, Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinOptionEtcSensorOptionEtcSensorData(dicConditions1, dicConditions2, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinOptionEtcSensorOptionEtcSensorData(Dictionary<Model.Sensor.Option.Etc.Fields, object> dicConditions1, Dictionary<Model.Sensor.Option.EtcData.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strEtcTableName = Model.Sensor.Option.Etc.TableName;
            string strEtcDataTableName = Model.Sensor.Option.EtcData.TableName;

            int nEtcFieldCount, nEtcDataFieldCount;

            string strEtcFields = GetFieldNames<Model.Sensor.Option.Etc.Fields>(strEtcTableName, out nEtcFieldCount);
            string strEtcDataFields = GetFieldNames<Model.Sensor.Option.EtcData.Fields>(strEtcDataTableName, out nEtcDataFieldCount);

            int nFieldsCount = nEtcFieldCount + nEtcDataFieldCount;
            bool isNullable;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strEtcFields, strEtcFields);
            sb.AppendFormat("  From {0}, {1} ", strEtcTableName, strEtcDataTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strEtcTableName, Model.Sensor.Option.Etc.GetFieldName(Model.Sensor.Option.Etc.Fields.SensorType, out isNullable)
                                                       , strEtcDataTableName, Model.Sensor.Option.EtcData.GetFieldName(Model.Sensor.Option.EtcData.Fields.SensorType, out isNullable));

            string strCondition1 = "";
            if (SetCondition<Model.Sensor.Option.Etc.Fields>(ref strCondition1, dicConditions1, Model.Sensor.Option.Etc.GetFieldName, strEtcTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition1.Length > 0)
                sb.AppendFormat(" and {0}", strCondition1);

            string strCondition2 = "";
            if (SetCondition<Model.Sensor.Option.EtcData.Fields>(ref strCondition2, dicConditions2, Model.Sensor.Option.EtcData.GetFieldName, strEtcDataTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition2.Length > 0)
                sb.AppendFormat(" and {0}", strCondition2);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                Model.Sensor.Option.Etc optionEtcSensor = ReadOptionEtcSensor(arrResult, i, out strErrorMessage);

                if (optionEtcSensor == null)
                    return null;
                else
                    arrDatas.Add(optionEtcSensor);

                Model.Sensor.Option.EtcData optionEtcSensorData = ReadOptionEtcSensorData(arrResult, i + nEtcFieldCount, out strErrorMessage);

                if (optionEtcSensorData == null)
                    return null;
                else
                    arrDatas.Add(optionEtcSensorData);
            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneTagInfo(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<TagInfo.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneTagInfo(dicConditions1, dicConditions2, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneTagInfo(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<TagInfo.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneTableName = SensorZone.TableName;
            string strTagInfoTableName = TagInfo.TableName;

            int nSensorZoneFieldCount, nTagInfoFieldCount;

            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strTagInfoFields = GetFieldNames<TagInfo.Fields>(strTagInfoTableName, out nTagInfoFieldCount);

            int nFieldsCount = nSensorZoneFieldCount + nTagInfoFieldCount;
            bool isNullable;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strSensorZoneFields, strTagInfoFields);
            sb.AppendFormat("  From {0}, {1} ", strSensorZoneTableName, strTagInfoTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strSensorZoneTableName, SensorZone.GetFieldName(SensorZone.Fields.ID, out isNullable)
                                                       , strTagInfoTableName, TagInfo.GetFieldName(TagInfo.Fields.SensorZoneID, out isNullable));

            string strCondition1 = "";
            if (SetCondition<SensorZone.Fields>(ref strCondition1, dicConditions1, SensorZone.GetFieldName, strSensorZoneTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition1.Length > 0)
                sb.AppendFormat(" and {0}", strCondition1);

            string strCondition2 = "";
            if (SetCondition<TagInfo.Fields>(ref strCondition2, dicConditions2, TagInfo.GetFieldName, strTagInfoTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition2.Length > 0)
                sb.AppendFormat(" and {0}", strCondition2);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZone sensorZone = ReadSensorZone(arrResult, i, out strErrorMessage);

                if (sensorZone == null)
                    return null;
                else
                    arrDatas.Add(sensorZone);

                TagInfo tagInfo = ReadSensorTagInfo(arrResult, i + nSensorZoneFieldCount, out strErrorMessage);

                if (tagInfo == null)
                    return null;
                else
                    arrDatas.Add(tagInfo);
            }

            return arrDatas;
        }

        public ArrayList JoinFacilityInfoFacilityInfoData(Dictionary<Info.Fields, object> dicConditions1, Dictionary<InfoData.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinFacilityInfoFacilityInfoData(dicConditions1, dicConditions2, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinFacilityInfoFacilityInfoData(Dictionary<Info.Fields, object> dicConditions1, Dictionary<InfoData.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strInfoTableName = Info.TableName;
            string strDataTableName = InfoData.TableName;

            int nInfoFieldCount, nDataFieldCount;

            string strInfoFields = GetFieldNames<Info.Fields>(strInfoTableName, out nInfoFieldCount);
            string strDataFields = GetFieldNames<InfoData.Fields>(strDataTableName, out nDataFieldCount);

            int nFieldsCount = nInfoFieldCount + nDataFieldCount;
            bool isNullable;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strInfoFields, strDataFields);
            sb.AppendFormat("  From {0}, {1} ", strInfoTableName, strDataTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strInfoTableName, Info.GetFieldName(Info.Fields.ID, out isNullable)
                                                       , strDataTableName, InfoData.GetFieldName(InfoData.Fields.FacilityInfoID, out isNullable));

            string strCondition1 = "";
            if (SetCondition<Info.Fields>(ref strCondition1, dicConditions1, Info.GetFieldName, strInfoTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition1.Length > 0)
                sb.AppendFormat(" and {0}", strCondition1);

            string strCondition2 = "";
            if (SetCondition<InfoData.Fields>(ref strCondition2, dicConditions2, InfoData.GetFieldName, strDataTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition2.Length > 0)
                sb.AppendFormat(" and {0}", strCondition2);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                Info info = ReadFacilityInfo(arrResult, i, out strErrorMessage);

                if (info == null)
                    return null;
                else
                    arrDatas.Add(info);

                InfoData data = ReadFacilityInfoData(arrResult, i + nInfoFieldCount, out strErrorMessage);

                if (data == null)
                    return null;
                else
                    arrDatas.Add(data);
            }

            return arrDatas;
        }

        /// <summary>
        /// Join BuildingGroup, Building, Zone
        /// </summary>
        /// <param name="dicConditions1"></param>
        /// <param name="dicConditions2"></param>
        /// <param name="strAdditionalConditions"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public ArrayList JoinBuildingGroupBuildingZone(Dictionary<BuildingGroup.Fields, object> dicConditions1, Dictionary<Building.Fields, object> dicConditions2, Dictionary<Zone.Fields, object> dicConditions3, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinBuildingGroupBuildingZone(dicConditions1, dicConditions2, dicConditions3, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinBuildingGroupBuildingZone(Dictionary<BuildingGroup.Fields, object> dicConditions1, Dictionary<Building.Fields, object> dicConditions2, Dictionary<Zone.Fields, object> dicConditions3, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strBuildingGroupTableName = BuildingGroup.TableName;
            string strBuildingTableName = Building.TableName;
            string strZoneTableName = Zone.TableName;

            int nBuildingGroupFieldCount, nBuildingFieldCount, nZoneFieldCount;

            string strBuildingGroupFields = GetFieldNames<BuildingGroup.Fields>(strBuildingGroupTableName, out nBuildingGroupFieldCount);
            string strBuildingFields = GetFieldNames<Building.Fields>(strBuildingTableName, out nBuildingFieldCount);
            string strZoneFields = GetFieldNames<Zone.Fields>(strZoneTableName, out nZoneFieldCount);

            int nFieldsCount = nBuildingGroupFieldCount + nBuildingFieldCount + nZoneFieldCount;
            bool isNullable;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, {2} ", strBuildingGroupFields, strBuildingFields, strZoneFields);
            sb.AppendFormat("  From {0}, {1}, {2} ", strBuildingGroupTableName, strBuildingTableName, strZoneTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strBuildingGroupTableName, BuildingGroup.GetFieldName(BuildingGroup.Fields.ID, out isNullable)
                                                       , strBuildingTableName, Building.GetFieldName(Building.Fields.BuildingGroupID, out isNullable));
            sb.AppendFormat("   And {0}.{1} = {2}.{3} ", strBuildingTableName, Building.GetFieldName(Building.Fields.ID, out isNullable)
                                                       , strZoneTableName, Zone.GetFieldName(Zone.Fields.BuildingID, out isNullable));

            string strCondition1 = "";
            if (SetCondition<BuildingGroup.Fields>(ref strCondition1, dicConditions1, BuildingGroup.GetFieldName, strBuildingGroupTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition1.Length > 0)
                sb.AppendFormat(" and {0}", strCondition1);

            string strCondition2 = "";
            if (SetCondition<Building.Fields>(ref strCondition2, dicConditions2, Building.GetFieldName, strBuildingTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition2.Length > 0)
                sb.AppendFormat(" and {0}", strCondition2);

            string strCondition3 = "";
            if (SetCondition<Zone.Fields>(ref strCondition3, dicConditions3, Zone.GetFieldName, strZoneTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition3.Length > 0)
                sb.AppendFormat(" and {0}", strCondition3);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                BuildingGroup bg = ReadBuildingGroup(arrResult, i, out strErrorMessage);

                if (bg == null)
                    return null;
                else
                    arrDatas.Add(bg);

                Building b = ReadBuilding(arrResult, i + nBuildingGroupFieldCount, out strErrorMessage);

                if (b == null)
                    return null;
                else
                    arrDatas.Add(b);

                Zone z = ReadZone(arrResult, i + nBuildingGroupFieldCount + nBuildingFieldCount, out strErrorMessage);

                if (z == null)
                    return null;
                else
                    arrDatas.Add(z);
            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneFireSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<Fire.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneFireSensor(dicConditions1, dicConditions2, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneFireSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<Fire.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneTableName = SensorZone.TableName;
            string strFireTableName = Fire.TableName;

            int nSensorZoneFieldCount, nFireFieldCount;

            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strFireFields = GetFieldNames<Fire.Fields>(strFireTableName, out nFireFieldCount);

            int nFieldsCount = nSensorZoneFieldCount + nFireFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strSensorZoneFields, strFireFields);
            sb.AppendFormat("  From {0}, {1} ", strSensorZoneTableName, strFireTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strSensorZoneTableName, SensorZone.Fields.OrgSensorID, strFireTableName, Fire.Fields.ID);
            sb.AppendFormat("   And {0}.{1} in ({2}) ", strSensorZoneTableName, SensorZone.Fields.SensorType, string.Join(",", Facility.GetFireTypeAllNumberToList()));

            string strCondition1 = "";
            if (SetCondition<SensorZone.Fields>(ref strCondition1, dicConditions1, SensorZone.GetFieldName, strSensorZoneTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition1.Length > 0)
                sb.AppendFormat(" and {0}", strCondition1);

            string strCondition2 = "";
            if (SetCondition<Fire.Fields>(ref strCondition2, dicConditions2, Fire.GetFieldName, strFireTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition2.Length > 0)
                sb.AppendFormat(" and {0}", strCondition2);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZone info = ReadSensorZone(arrResult, i, out strErrorMessage);

                if (info == null)
                    return null;
                else
                    arrDatas.Add(info);

                Fire data = ReadFireSensor(arrResult, i + nSensorZoneFieldCount, out strErrorMessage);

                if (data == null)
                    return null;
                else
                    arrDatas.Add(data);
            }

            return arrDatas;
        }

        public ArrayList JoinSensorZonePSMSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<PSM.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZonePSMSensor(dicConditions1, dicConditions2, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZonePSMSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<PSM.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneTableName = SensorZone.TableName;
            string strPSMTableName = PSM.TableName;

            int nSensorZoneFieldCount, nPSMFieldCount;

            string strInfoFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strPSMFields = GetFieldNames<PSM.Fields>(strPSMTableName, out nPSMFieldCount);

            int nFieldsCount = nSensorZoneFieldCount + nPSMFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strInfoFields, strPSMFields);
            sb.AppendFormat("  From {0}, {1} ", strSensorZoneTableName, strPSMTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strSensorZoneTableName, SensorZone.Fields.OrgSensorID, strPSMTableName, PSM.Fields.ID);
            sb.AppendFormat("   And {0}.{1} in ({2}) ", strSensorZoneTableName, SensorZone.Fields.SensorType, string.Join(",", Facility.GetPSMTypeAllNumberToList()));

            string strCondition1 = "";
            if (SetCondition<SensorZone.Fields>(ref strCondition1, dicConditions1, SensorZone.GetFieldName, strSensorZoneTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition1.Length > 0)
                sb.AppendFormat(" and {0}", strCondition1);

            string strCondition2 = "";
            if (SetCondition<PSM.Fields>(ref strCondition2, dicConditions2, PSM.GetFieldName, strPSMTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition2.Length > 0)
                sb.AppendFormat(" and {0}", strCondition2);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZone info = ReadSensorZone(arrResult, i, out strErrorMessage);

                if (info == null)
                    return null;
                else
                    arrDatas.Add(info);

                PSM data = ReadPSMSensor(arrResult, i + nSensorZoneFieldCount, out strErrorMessage);

                if (data == null)
                    return null;
                else
                    arrDatas.Add(data);
            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneETCSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<ETC.Fields, object> dicConditions2, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneETCSensor(dicConditions1, dicConditions2, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneETCSensor(Dictionary<SensorZone.Fields, object> dicConditions1, Dictionary<ETC.Fields, object> dicConditions2, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneTableName = SensorZone.TableName;
            string strETCTableName = ETC.TableName;

            int nSensorZoneFieldCount, nETCFieldCount;

            string strInfoFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strETCFields = GetFieldNames<ETC.Fields>(strETCTableName, out nETCFieldCount);

            int nFieldsCount = nSensorZoneFieldCount + nETCFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strInfoFields, strETCFields);
            sb.AppendFormat("  From {0}, {1} ", strSensorZoneTableName, strETCTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strSensorZoneTableName, SensorZone.Fields.OrgSensorID, strETCTableName, ETC.Fields.ID);
            sb.AppendFormat("   And {0}.{1} in ({2}) ", strSensorZoneTableName, SensorZone.Fields.SensorType, string.Join(",", Facility.GetETCTypeAllNumberToList()));

            string strCondition1 = "";
            if (SetCondition<SensorZone.Fields>(ref strCondition1, dicConditions1, SensorZone.GetFieldName, strSensorZoneTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition1.Length > 0)
                sb.AppendFormat(" and {0}", strCondition1);

            string strCondition2 = "";
            if (SetCondition<ETC.Fields>(ref strCondition2, dicConditions2, ETC.GetFieldName, strETCTableName, ref strErrorMessage) == false)
                return null;

            if (strCondition2.Length > 0)
                sb.AppendFormat(" and {0}", strCondition2);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZone info = ReadSensorZone(arrResult, i, out strErrorMessage);

                if (info == null)
                    return null;
                else
                    arrDatas.Add(info);

                ETC data = ReadETCSensor(arrResult, i + nSensorZoneFieldCount, out strErrorMessage);

                if (data == null)
                    return null;
                else
                    arrDatas.Add(data);
            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneSensors(Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneSensors(dicConditions, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneSensors(Dictionary<SensorZone.Fields, object> dicConditions, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, ", SensorZone.Fields.ID, SensorZone.Fields.SensorType);
            sb.Append("             case ");
            sb.AppendFormat("            when {0} in ({1}) then(select name from {2} as f Where f.ID = sz.OrgSensorID)"
                , SensorZone.Fields.SensorType, string.Join(",", Facility.GetFireTypeAllNumberToList()), Fire.TableName);
            sb.AppendFormat("            when {0} in ({1}) then(select name from {2} as p Where p.ID = sz.OrgSensorID)"
                , SensorZone.Fields.SensorType, string.Join(",", Facility.GetPSMTypeAllNumberToList()), PSM.TableName);
            sb.AppendFormat("            when {0} in ({1}) then(select name from {2} as p Where p.ID = sz.OrgSensorID)"
                , SensorZone.Fields.SensorType, string.Join(",", Facility.GetETCTypeAllNumberToList()), ETC.TableName);
            sb.AppendFormat("            when {0} in ({1}) then(select cameraName from {2} as p Where p.ID = sz.OrgSensorID)"
                , SensorZone.Fields.SensorType, string.Join(",", Facility.GetSVMSTypeAllNumberToList()), CCTV.TableName);
            sb.Append("              end as name ");
            sb.AppendFormat("  From {0} as sz ", SensorZone.TableName);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" Where {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrResult2 = new ArrayList();
            int resultCount = arrResult.Count;
            for (int i = 0; i < resultCount; i += 3)
            {
                int nSensorZoneID = dnsDBUtil.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nSensorType = dnsDBUtil.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                string strSensorName = dnsDBUtil.WebDBManager.GetStringField(arrResult[i + 2].ToString());

                arrResult2.Add(nSensorZoneID);
                arrResult2.Add(nSensorType);
                arrResult2.Add(strSensorName);
            }

            return arrResult2;
        }

        public ArrayList JoinEquipmentZoneEquipZoneCCTV(int equipZoneID, string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinEquipmentZoneEquipZoneCCTV(equipZoneID, strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinEquipmentZoneEquipZoneCCTV(int equipZoneID, string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strEquipmentZoneTableName = EquipmentZone.TableName;
            string strEquipZoneCCTVTableName = EquipZoneCCTV.TableName;

            int nEquipmentZoneFieldCount, nEquipZoneCCTVFieldCount;

            string strEquipmentZoneFields = GetFieldNames<EquipmentZone.Fields>(strEquipmentZoneTableName, out nEquipmentZoneFieldCount);
            string strEquipZoneCCTVFields = GetFieldNames<EquipZoneCCTV.Fields>(strEquipZoneCCTVTableName, out nEquipZoneCCTVFieldCount);

            int nFieldsCount = nEquipmentZoneFieldCount + nEquipZoneCCTVFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strEquipmentZoneFields, strEquipZoneCCTVFields);
            sb.AppendFormat("  From {0}, {1} ", strEquipmentZoneTableName, strEquipZoneCCTVTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} and {0}.{1} = {4}", strEquipmentZoneTableName, EquipmentZone.Fields.ID, strEquipZoneCCTVTableName, EquipZoneCCTV.Fields.EquipZoneID, equipZoneID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                EquipmentZone equipmentZone = ReadEquipmentZone(arrResult, i, out strErrorMessage);

                if (equipmentZone == null)
                    return null;
                else
                    arrDatas.Add(equipmentZone);

                EquipZoneCCTV data = ReadEquipZoneCCTV(arrResult, i + nEquipmentZoneFieldCount, out strErrorMessage);

                if (data == null)
                    return null;
                else
                    arrDatas.Add(data);
            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneHistorySensorReactionHistory(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneHistorySensorReactionHistory(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneHistorySensorReactionHistory(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneHistoryTableName = SensorZoneHistory.TableName;
            string strSensorReactionHistoryTableName = SensorReactionHistory.TableName;

            int nSensorZoneHistoryFieldCount, nSensorReactionHistoryFieldCount;

            string strSensorZoneHistoryFields = GetFieldNames<SensorZoneHistory.Fields>(strSensorZoneHistoryTableName, out nSensorZoneHistoryFieldCount);
            string strSensorReactionHistoryFields = GetFieldNames<SensorReactionHistory.Fields>(strSensorReactionHistoryTableName, out nSensorReactionHistoryFieldCount);

            int nFieldsCount = nSensorZoneHistoryFieldCount + nSensorReactionHistoryFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strSensorZoneHistoryFields, strSensorReactionHistoryFields);
            sb.AppendFormat("  From {0}, {1} ", strSensorZoneHistoryTableName, strSensorReactionHistoryTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3}", strSensorZoneHistoryTableName, SensorZoneHistory.Fields.ID, strSensorReactionHistoryTableName, SensorReactionHistory.Fields.SensorZoneHistoryID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZoneHistory sensorZoneHistory = ReadSensorZoneHistory(arrResult, i, out strErrorMessage);

                if (sensorZoneHistory == null)
                    return null;
                else
                    arrDatas.Add(sensorZoneHistory);

                SensorReactionHistory sensorReactionHistory = ReadSensorReactionHistory(arrResult, i + nSensorZoneHistoryFieldCount, out strErrorMessage);

                if (sensorReactionHistory == null)
                    return null;
                else
                    arrDatas.Add(sensorReactionHistory);
            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneSensorZoneHistory(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneSensorZoneHistory(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneSensorZoneHistory(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneTableName = SensorZone.TableName;
            string strSensorZoneHistoryTableName = SensorZoneHistory.TableName;

            int nSensorZoneFieldCount, nSensorZoneHistoryFieldCount;

            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strSensorZoneHistoryFields = GetFieldNames<SensorZoneHistory.Fields>(strSensorZoneHistoryTableName, out nSensorZoneHistoryFieldCount);

            int nFieldsCount = nSensorZoneFieldCount + nSensorZoneHistoryFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strSensorZoneFields, strSensorZoneHistoryFields);
            sb.AppendFormat("  From {0}, {1} ", strSensorZoneTableName, strSensorZoneHistoryTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3}", strSensorZoneTableName, SensorZone.Fields.ID, strSensorZoneHistoryTableName, SensorZoneHistory.Fields.SensorZoneID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZone sensorZone = ReadSensorZone(arrResult, i, out strErrorMessage);

                if (sensorZone == null)
                    return null;
                else
                    arrDatas.Add(sensorZone);

                SensorZoneHistory sensorZoneHistory = ReadSensorZoneHistory(arrResult, i + nSensorZoneFieldCount, out strErrorMessage);

                if (sensorZoneHistory == null)
                    return null;
                else
                    arrDatas.Add(sensorZoneHistory);
            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneEquipmentZoneZoneBuildingBuildingGroup(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneEquipmentZoneZoneBuildingBuildingGroup(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneEquipmentZoneZoneBuildingBuildingGroup(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneTableName = SensorZone.TableName;
            string strEquipmentZoneTableName = EquipmentZone.TableName;
            string strZoneTableName = Zone.TableName;
            string strBuildingTableName = Building.TableName;
            string strBuildingGroupTableName = BuildingGroup.TableName;

            int nSensorZoneFieldCount, nEquipmentZoneFieldCount, nZoneFieldCount, nBuildingFieldCount, nBuildingGroupFieldCount;

            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strEquipmentZoneFields = GetFieldNames<EquipmentZone.Fields>(strEquipmentZoneTableName, out nEquipmentZoneFieldCount);
            string strZoneFields = GetFieldNames<Zone.Fields>(strZoneTableName, out nZoneFieldCount);
            string strBuildingFields = GetFieldNames<Building.Fields>(strBuildingTableName, out nBuildingFieldCount);
            string strBuildingGroupFields = GetFieldNames<BuildingGroup.Fields>(strBuildingGroupTableName, out nBuildingGroupFieldCount);

            int nFieldsCount = nSensorZoneFieldCount + nEquipmentZoneFieldCount + nZoneFieldCount + nBuildingFieldCount + nBuildingGroupFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, {2}, {3}, {4} ", strSensorZoneFields, strEquipmentZoneFields, strZoneFields, strBuildingFields, strBuildingGroupFields);
            sb.AppendFormat("  From {0}, {1}, {2}, {3}, {4} ", strSensorZoneTableName, strEquipmentZoneTableName, strZoneTableName, strBuildingTableName, strBuildingGroupTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strSensorZoneTableName, SensorZone.Fields.EquipZoneID, strEquipmentZoneTableName, EquipmentZone.Fields.ID);
            //EquipmentZone.LinkedZoneIDList 필드는 string이라 Zone.ID와 Join할수 없다
            //sb.AppendFormat(" and {0}.{1} = {2}.{3}", strEquipmentZoneTableName, EquipmentZone.Fields.LinkedZoneIDList, strZoneTableName, Zone.Fields.ID);
            sb.AppendFormat(" and {0}.{1} = {2}.{3}", strZoneTableName, Zone.Fields.BuildingID, strBuildingTableName, Building.Fields.ID);
            sb.AppendFormat(" and {0}.{1} = {2}.{3}", strBuildingTableName, Building.Fields.BuildingGroupID, strBuildingGroupTableName, BuildingGroup.Fields.ID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZone sensorZone = ReadSensorZone(arrResult, i, out strErrorMessage);

                if (sensorZone == null)
                    return null;
                else
                    arrDatas.Add(sensorZone);

                EquipmentZone equipmentZone = ReadEquipmentZone(arrResult, i + nSensorZoneFieldCount, out strErrorMessage);

                if (equipmentZone == null)
                    return null;
                else
                    arrDatas.Add(equipmentZone);

                Zone zone = ReadZone(arrResult, i + nSensorZoneFieldCount + nEquipmentZoneFieldCount, out strErrorMessage);

                if (zone == null)
                    return null;
                else
                    arrDatas.Add(zone);

                Building building = ReadBuilding(arrResult, i + nSensorZoneFieldCount + nEquipmentZoneFieldCount + nZoneFieldCount, out strErrorMessage);

                if (building == null)
                    return null;
                else
                    arrDatas.Add(building);

                BuildingGroup buildingGroup = ReadBuildingGroup(arrResult, i + nSensorZoneFieldCount + nEquipmentZoneFieldCount + nZoneFieldCount + nBuildingFieldCount, out strErrorMessage);

                if (buildingGroup == null)
                    return null;
                else
                    arrDatas.Add(buildingGroup);

            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneHistoryZoneBuildingBuildingGroup(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneHistoryZoneBuildingBuildingGroup(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneHistoryZoneBuildingBuildingGroup(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneHistoryTableName = SensorZoneHistory.TableName;
            string strZoneTableName = Zone.TableName;
            string strBuildingTableName = Building.TableName;
            string strBuildingGroupTableName = BuildingGroup.TableName;

            int nSensorZoneHistoryFieldCount, nZoneFieldCount, nBuildingFieldCount, nBuildingGroupFieldCount;

            string strSensorZoneHistoryFields = GetFieldNames<SensorZoneHistory.Fields>(strSensorZoneHistoryTableName, out nSensorZoneHistoryFieldCount);
            string strZoneFields = GetFieldNames<Zone.Fields>(strZoneTableName, out nZoneFieldCount);
            string strBuildingFields = GetFieldNames<Building.Fields>(strBuildingTableName, out nBuildingFieldCount);
            string strBuildingGroupFields = GetFieldNames<BuildingGroup.Fields>(strBuildingGroupTableName, out nBuildingGroupFieldCount);

            int nFieldsCount = nSensorZoneHistoryFieldCount + nZoneFieldCount + nBuildingFieldCount + nBuildingGroupFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, {2}, {3} ", strSensorZoneHistoryFields, strZoneFields, strBuildingFields, strBuildingGroupFields);
            sb.AppendFormat("  From {0}, {1}, {2}, {3} ", strSensorZoneHistoryTableName, strZoneTableName, strBuildingTableName, strBuildingGroupTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strSensorZoneHistoryTableName, SensorZoneHistory.Fields.ZoneID, strZoneTableName, Zone.Fields.ID);
            sb.AppendFormat(" and {0}.{1} = {2}.{3}", strZoneTableName, Zone.Fields.BuildingID, strBuildingTableName, Building.Fields.ID);
            sb.AppendFormat(" and {0}.{1} = {2}.{3}", strBuildingTableName, Building.Fields.BuildingGroupID, strBuildingGroupTableName, BuildingGroup.Fields.ID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZoneHistory sensorZoneHistory = ReadSensorZoneHistory(arrResult, i, out strErrorMessage);

                if (sensorZoneHistory == null)
                    return null;
                else
                    arrDatas.Add(sensorZoneHistory);

                Zone zone = ReadZone(arrResult, i + nSensorZoneHistoryFieldCount, out strErrorMessage);

                if (zone == null)
                    return null;
                else
                    arrDatas.Add(zone);

                Building building = ReadBuilding(arrResult, i + nSensorZoneHistoryFieldCount + nZoneFieldCount, out strErrorMessage);

                if (building == null)
                    return null;
                else
                    arrDatas.Add(building);

                BuildingGroup buildingGroup = ReadBuildingGroup(arrResult, i + nSensorZoneHistoryFieldCount + nZoneFieldCount + nBuildingFieldCount, out strErrorMessage);

                if (buildingGroup == null)
                    return null;
                else
                    arrDatas.Add(buildingGroup);

            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneHistorySensorZoneZoneBuildingBuildingGroup(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneHistorySensorZoneZoneBuildingBuildingGroup(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneHistorySensorZoneZoneBuildingBuildingGroup(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneHistoryTableName = SensorZoneHistory.TableName;
            string strSensorZoneTableName = SensorZone.TableName;
            string strZoneTableName = Zone.TableName;
            string strBuildingTableName = Building.TableName;
            string strBuildingGroupTableName = BuildingGroup.TableName;

            int nSensorZoneHistoryFieldCount, nSensorZoneFieldCoun, nZoneFieldCount, nBuildingFieldCount, nBuildingGroupFieldCount;

            string strSensorZoneHistoryFields = GetFieldNames<SensorZoneHistory.Fields>(strSensorZoneHistoryTableName, out nSensorZoneHistoryFieldCount);
            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCoun);
            string strZoneFields = GetFieldNames<Zone.Fields>(strZoneTableName, out nZoneFieldCount);
            string strBuildingFields = GetFieldNames<Building.Fields>(strBuildingTableName, out nBuildingFieldCount);
            string strBuildingGroupFields = GetFieldNames<BuildingGroup.Fields>(strBuildingGroupTableName, out nBuildingGroupFieldCount);

            int nFieldsCount = nSensorZoneHistoryFieldCount + nSensorZoneFieldCoun + nZoneFieldCount + nBuildingFieldCount + nBuildingGroupFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, {2}, {3}, {4} ", strSensorZoneHistoryFields, strSensorZoneFields, strZoneFields, strBuildingFields, strBuildingGroupFields);
            sb.AppendFormat("  From {0}, {1}, {2}, {3}, {4} ", strSensorZoneHistoryTableName, strSensorZoneTableName, strZoneTableName, strBuildingTableName, strBuildingGroupTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strSensorZoneHistoryTableName, SensorZoneHistory.Fields.ZoneID, strZoneTableName, Zone.Fields.ID);
            sb.AppendFormat(" and {0}.{1} = {2}.{3}", strSensorZoneHistoryTableName, SensorZoneHistory.Fields.SensorZoneID, strSensorZoneTableName, SensorZone.Fields.ID);
            sb.AppendFormat(" and {0}.{1} = {2}.{3}", strZoneTableName, Zone.Fields.BuildingID, strBuildingTableName, Building.Fields.ID);
            sb.AppendFormat(" and {0}.{1} = {2}.{3}", strBuildingTableName, Building.Fields.BuildingGroupID, strBuildingGroupTableName, BuildingGroup.Fields.ID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" and {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZoneHistory sensorZoneHistory = ReadSensorZoneHistory(arrResult, i, out strErrorMessage);

                if (sensorZoneHistory == null)
                    return null;
                else
                    arrDatas.Add(sensorZoneHistory);

                SensorZone sensorZone = ReadSensorZone(arrResult, i + nSensorZoneHistoryFieldCount, out strErrorMessage);

                if (sensorZone == null)
                    return null;
                else
                    arrDatas.Add(sensorZone);

                Zone zone = ReadZone(arrResult, i + nSensorZoneHistoryFieldCount + nSensorZoneFieldCoun, out strErrorMessage);

                if (zone == null)
                    return null;
                else
                    arrDatas.Add(zone);

                Building building = ReadBuilding(arrResult, i + nSensorZoneHistoryFieldCount + nSensorZoneFieldCoun + nZoneFieldCount, out strErrorMessage);

                if (building == null)
                    return null;
                else
                    arrDatas.Add(building);

                BuildingGroup buildingGroup = ReadBuildingGroup(arrResult, i + nSensorZoneHistoryFieldCount + nSensorZoneFieldCoun + nZoneFieldCount + nBuildingFieldCount, out strErrorMessage);

                if (buildingGroup == null)
                    return null;
                else
                    arrDatas.Add(buildingGroup);

            }

            return arrDatas;
        }

        public ArrayList JoinCurrentAlarmHistory(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinCurrentAlarmHistory(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinCurrentAlarmHistory(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCurrentAlarmTableName = CurrentAlarm.TableName;
            string strSensorZoneHistoryTableName = SensorZoneHistory.TableName;
            string strSensorReactionHistoryTableName = SensorReactionHistory.TableName;

            int nCurrentAlarmFieldCount, nSensorZoneHistoryFieldCount, nSensorReactionHistoryFieldCount;

            string strCurrentAlarmFields = GetFieldNames<CurrentAlarm.Fields>(strCurrentAlarmTableName, out nCurrentAlarmFieldCount);
            string strSensorZoneHistoryFields = GetFieldNames<SensorZoneHistory.Fields>(strSensorZoneHistoryTableName, out nSensorZoneHistoryFieldCount);
            string strSensorReactionHistoryFields = GetFieldNames<SensorReactionHistory.Fields>(strSensorReactionHistoryTableName, out nSensorReactionHistoryFieldCount);

            int nFieldsCount = nCurrentAlarmFieldCount + nSensorZoneHistoryFieldCount + nSensorReactionHistoryFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, {2} ", strCurrentAlarmFields, strSensorZoneHistoryFields, strSensorReactionHistoryFields);
            sb.AppendFormat("  From {0}, {1}, {2} ", strCurrentAlarmTableName, strSensorZoneHistoryTableName, strSensorReactionHistoryTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strCurrentAlarmTableName, CurrentAlarm.Fields.SensorZoneHistoryID, strSensorZoneHistoryTableName, SensorZoneHistory.Fields.ID);
            sb.AppendFormat("   And {0}.{1} = {2}.{3} ", strSensorZoneHistoryTableName, SensorZoneHistory.Fields.ID, strSensorReactionHistoryTableName, SensorReactionHistory.Fields.SensorZoneHistoryID);
            // 탐지신호, 재난신고
            sb.AppendFormat("   And {0}.{1} in (0, 22) ", strSensorReactionHistoryTableName, SensorReactionHistory.Fields.ReactionType);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" And {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                CurrentAlarm curAlarm = ReadCurrentAlarm(arrResult, i, out strErrorMessage);

                if (curAlarm == null)
                    return null;
                else
                    arrDatas.Add(curAlarm);

                SensorZoneHistory szh = ReadSensorZoneHistory(arrResult, i + nCurrentAlarmFieldCount, out strErrorMessage);

                if (szh == null)
                    return null;
                else
                    arrDatas.Add(szh);

                SensorReactionHistory srh = ReadSensorReactionHistory(arrResult, i + nCurrentAlarmFieldCount, out strErrorMessage);

                if (srh == null)
                    return null;
                else
                    arrDatas.Add(srh);
            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneTagInfoETCMaterial(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneTagInfoETCMaterial(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneTagInfoETCMaterial(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneTableName = SensorZone.TableName;
            string strTagInfoTableName = TagInfo.TableName;
            string strMaterialTableName = Material.TableName;
            string strETCTableName = ETC.TableName;

            int nSensorZoneFieldCount, nTagInfoFieldCount, nMaterialFieldCount;

            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strTagInfoFields = GetFieldNames<TagInfo.Fields>(strTagInfoTableName, out nTagInfoFieldCount);
            string strMaterialFields = GetFieldNames<Material.Fields>(strMaterialTableName, out nMaterialFieldCount);

            int nFieldsCount = nSensorZoneFieldCount + nTagInfoFieldCount + nMaterialFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, {2} ", strSensorZoneFields, strTagInfoFields, strMaterialFields);
            sb.AppendFormat("  From {0}, {1}, {2}, {3} ", strSensorZoneTableName, strTagInfoTableName, strMaterialTableName, strETCTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strSensorZoneTableName, SensorZone.Fields.ID, strTagInfoTableName, TagInfo.Fields.SensorZoneID);
            sb.AppendFormat("   And {0}.{1} = {2} ", strSensorZoneTableName, SensorZone.Fields.SensorType, (int)Facility.FacilityType.ETC);
            sb.AppendFormat("   And {0}.{1} = {2}.{3} ", strSensorZoneTableName, SensorZone.Fields.OrgSensorID, strETCTableName, ETC.Fields.ID);
            sb.AppendFormat("   And {0}.{1} = {2}.{3} ", strETCTableName, ETC.Fields.MaterialType, strMaterialTableName, Material.Fields.ID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" And {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZone sensorZone = ReadSensorZone(arrResult, i, out strErrorMessage);

                if (sensorZone == null)
                    return null;
                else
                    arrDatas.Add(sensorZone);

                TagInfo tagInfo = ReadSensorTagInfo(arrResult, i + nSensorZoneFieldCount, out strErrorMessage);

                if (tagInfo == null)
                    return null;
                else
                    arrDatas.Add(tagInfo);

                Material material = ReadMaterial(arrResult, i + nSensorZoneFieldCount + nTagInfoFieldCount, out strErrorMessage);

                if (material == null)
                    return null;
                else
                    arrDatas.Add(material);
            }

            return arrDatas;
        }

        public ArrayList JoinSensorZoneTagInfoPSMMaterial(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinSensorZoneTagInfoPSMMaterial(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinSensorZoneTagInfoPSMMaterial(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strSensorZoneTableName = SensorZone.TableName;
            string strTagInfoTableName = TagInfo.TableName;
            string strMaterialTableName = Material.TableName;
            string strPSMTableName = PSM.TableName;

            int nSensorZoneFieldCount, nTagInfoFieldCount, nMaterialFieldCount;

            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strTagInfoFields = GetFieldNames<TagInfo.Fields>(strTagInfoTableName, out nTagInfoFieldCount);
            string strMaterialFields = GetFieldNames<Material.Fields>(strMaterialTableName, out nMaterialFieldCount);

            int nFieldsCount = nSensorZoneFieldCount + nTagInfoFieldCount + nMaterialFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, {2} ", strSensorZoneFields, strTagInfoFields, strMaterialFields);
            sb.AppendFormat("  From {0}, {1}, {2}, {3} ", strSensorZoneTableName, strTagInfoTableName, strMaterialTableName, strPSMTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strSensorZoneTableName, SensorZone.Fields.ID, strTagInfoTableName, TagInfo.Fields.SensorZoneID);
            sb.AppendFormat("   And {0}.{1} = {2} ", strSensorZoneTableName, SensorZone.Fields.SensorType, (int)Facility.FacilityType.PSM_SENSOR);
            sb.AppendFormat("   And {0}.{1} = {2}.{3} ", strSensorZoneTableName, SensorZone.Fields.OrgSensorID, strPSMTableName, PSM.Fields.ID);
            sb.AppendFormat("   And {0}.{1} = {2}.{3} ", strPSMTableName, PSM.Fields.MaterialType, strMaterialTableName, Material.Fields.ID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" And {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                SensorZone sensorZone = ReadSensorZone(arrResult, i, out strErrorMessage);

                if (sensorZone == null)
                    return null;
                else
                    arrDatas.Add(sensorZone);

                TagInfo tagInfo = ReadSensorTagInfo(arrResult, i + nSensorZoneFieldCount, out strErrorMessage);

                if (tagInfo == null)
                    return null;
                else
                    arrDatas.Add(tagInfo);

                Material material = ReadMaterial(arrResult, i + nSensorZoneFieldCount + nTagInfoFieldCount, out strErrorMessage);

                if (material == null)
                    return null;
                else
                    arrDatas.Add(material);
            }

            return arrDatas;
        }

        public ArrayList JoinPSMSensorMaterial(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinPSMSensorMaterial(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinPSMSensorMaterial(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strPSMTableName = PSM.TableName;
            string strMaterialTableName = Material.TableName;

            int nPSMFieldCount, nMaterialFieldCount;

            string strPSMFields = GetFieldNames<PSM.Fields>(strPSMTableName, out nPSMFieldCount);
            string strMaterialFields = GetFieldNames<Material.Fields>(strMaterialTableName, out nMaterialFieldCount);

            int nFieldsCount = nPSMFieldCount + nMaterialFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strPSMFields, strMaterialFields);
            sb.AppendFormat("  From {0}, {1} ", strPSMTableName, strMaterialTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strPSMTableName, PSM.Fields.MaterialType, strMaterialTableName, Material.Fields.ID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" And {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                PSM psm = ReadPSMSensor(arrResult, i, out strErrorMessage);

                if (psm == null)
                    return null;
                else
                    arrDatas.Add(psm);

                Material material = ReadMaterial(arrResult, i + nPSMFieldCount, out strErrorMessage);

                if (material == null)
                    return null;
                else
                    arrDatas.Add(material);
            }

            return arrDatas;
        }

        public ArrayList JoinETCSensorMaterial(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinETCSensorMaterial(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinETCSensorMaterial(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strETCTableName = ETC.TableName;
            string strMaterialTableName = Material.TableName;

            int nETCFieldCount, nMaterialFieldCount;

            string strETCFields = GetFieldNames<ETC.Fields>(strETCTableName, out nETCFieldCount);
            string strMaterialFields = GetFieldNames<Material.Fields>(strMaterialTableName, out nMaterialFieldCount);

            int nFieldsCount = nETCFieldCount + nMaterialFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1} ", strETCFields, strMaterialFields);
            sb.AppendFormat("  From {0}, {1} ", strETCTableName, strMaterialTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strETCTableName, ETC.Fields.MaterialType, strMaterialTableName, Material.Fields.ID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" And {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                ETC etc = ReadETCSensor(arrResult, i, out strErrorMessage);

                if (etc == null)
                    return null;
                else
                    arrDatas.Add(etc);

                Material material = ReadMaterial(arrResult, i + nETCFieldCount, out strErrorMessage);

                if (material == null)
                    return null;
                else
                    arrDatas.Add(material);
            }

            return arrDatas;
        }

        public ArrayList JoinCurrentAlarmSensorZoneHistorySensorZoneTagInfo(string strAdditionalConditions, out string strErrorMessage)
        {
            return JoinCurrentAlarmSensorZoneHistorySensorZoneTagInfo(strAdditionalConditions, null, out strErrorMessage);
        }

        public ArrayList JoinCurrentAlarmSensorZoneHistorySensorZoneTagInfo(string strAdditionalConditions, int? topNCount, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strCurrentAlarmTableName = CurrentAlarm.TableName;
            string strSensorZoneHistoryTableName = SensorZoneHistory.TableName;
            string strSensorZoneTableName = SensorZone.TableName;
            string strTagInfoTableName = TagInfo.TableName;

            int nCurrentAlarmFieldCount, nSensorZoneHistoryFieldCount, nSensorZoneFieldCount, nTagInfoFieldCount;

            string strCurrentAlarmFields = GetFieldNames<CurrentAlarm.Fields>(strCurrentAlarmTableName, out nCurrentAlarmFieldCount);
            string strSensorZoneHistoryFields = GetFieldNames<SensorZoneHistory.Fields>(strSensorZoneHistoryTableName, out nSensorZoneHistoryFieldCount);
            string strSensorZoneFields = GetFieldNames<SensorZone.Fields>(strSensorZoneTableName, out nSensorZoneFieldCount);
            string strTagInfoFields = GetFieldNames<TagInfo.Fields>(strTagInfoTableName, out nTagInfoFieldCount);

            int nFieldsCount = nCurrentAlarmFieldCount + nSensorZoneHistoryFieldCount + nSensorZoneFieldCount + nTagInfoFieldCount;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Select {0}, {1}, {2}, {3} ", strCurrentAlarmFields, strSensorZoneHistoryFields, strSensorZoneFields, strTagInfoFields);
            sb.AppendFormat("  From {0}, {1}, {2}, {3} ", strCurrentAlarmTableName, strSensorZoneHistoryTableName, strSensorZoneTableName, strTagInfoTableName);
            sb.AppendFormat(" Where {0}.{1} = {2}.{3} ", strCurrentAlarmTableName, CurrentAlarm.Fields.SensorZoneHistoryID, strSensorZoneHistoryTableName, SensorZoneHistory.Fields.ID);
            sb.AppendFormat(" And {0}.{1} = {2}.{3} ", strSensorZoneHistoryTableName, SensorZoneHistory.Fields.SensorZoneID, strSensorZoneTableName, SensorZone.Fields.ID);
            sb.AppendFormat(" And {0}.{1} = {2}.{3} ", strSensorZoneTableName, SensorZone.Fields.ID, strTagInfoTableName, TagInfo.Fields.SensorZoneID);

            if (strAdditionalConditions != null && strAdditionalConditions.Length > 0)
            {
                sb.AppendFormat(" And {0}", strAdditionalConditions);
            }

            ArrayList arrResult = topNCount == null ? m_dbManager.GetResultData(sb.ToString()) : m_dbManager.GetResultData(sb.ToString(), (int)topNCount);
            if (arrResult == null)
            {
                strErrorMessage = m_dbManager.LastErrorMessage;
                return null;
            }

            ArrayList arrDatas = new ArrayList();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (nFieldsCount - 1); i += nFieldsCount)
            {
                CurrentAlarm currentAlarm = ReadCurrentAlarm(arrResult, i, out strErrorMessage);

                if (currentAlarm == null)
                    return null;
                else
                    arrDatas.Add(currentAlarm);

                SensorZoneHistory sensorZoneHistory = ReadSensorZoneHistory(arrResult, i + nCurrentAlarmFieldCount, out strErrorMessage);

                if (sensorZoneHistory == null)
                    return null;
                else
                    arrDatas.Add(sensorZoneHistory);

                SensorZone sensorZone = ReadSensorZone(arrResult, i + nCurrentAlarmFieldCount + nSensorZoneHistoryFieldCount, out strErrorMessage);

                if (sensorZone == null)
                    return null;
                else
                    arrDatas.Add(sensorZone);

                TagInfo tagInfo = ReadSensorTagInfo(arrResult, i + nCurrentAlarmFieldCount + nSensorZoneHistoryFieldCount + nSensorZoneFieldCount, out strErrorMessage);

                if (tagInfo == null)
                    return null;
                else
                    arrDatas.Add(tagInfo);
            }

            return arrDatas;
        }
    }
}
