using SDMS.Model.Spatial;
using System.Collections.Generic;
using SDMS.IDAL;
using SensorMaker.BLL.Models.Data.Sensor;

namespace SensorMaker.BLL.Models.Data
{
    /// <summary>
    /// BuildingGroup > Building > Zone > EquipmentZone
    /// </summary>
    public class SpatialManager
    {
        private Dictionary<int, BuildingGroupData> m_dicBuildingGroups = new Dictionary<int, BuildingGroupData>();
        private Dictionary<int, BuildingData> m_dicBuildings = new Dictionary<int, BuildingData>();
        private Dictionary<int, ZoneData> m_dicZones = new Dictionary<int, ZoneData>();
        private Dictionary<int, EquipmentZoneData> m_dicEquipZones = new Dictionary<int, EquipmentZoneData>();

        public ICollection<BuildingGroupData> BuildingGroups
        {
            get { return m_dicBuildingGroups.Values; }
        }

        public ICollection<BuildingData> Buildings
        {
            get { return m_dicBuildings.Values; }
        }

        public ICollection<ZoneData> Zones
        {
            get { return m_dicZones.Values; }
        }

        public ICollection<EquipmentZoneData> EquipZones
        {
            get { return m_dicEquipZones.Values; }
        }

        private static int m_nInstanceCount = 0;

        public SpatialManager()
        {
            m_nInstanceCount++;
        }

        public BuildingGroupData GetBuildingGroup(int id)
        {
            BuildingGroupData bg;

            if (m_dicBuildingGroups.TryGetValue(id, out bg))
                return bg;

            return null;
        }

        public BuildingData GetBuilding(int id)
        {
            BuildingData building;

            if (m_dicBuildings.TryGetValue(id, out building))
                return building;

            return null;
        }

        public ZoneData GetZone(int id)
        {
            ZoneData zone;

            if (m_dicZones.TryGetValue(id, out zone))
                return zone;

            return null;
        }

        public EquipmentZoneData GetEquipmentZone(int id)
        {
            EquipmentZoneData equipZone;

            if (m_dicEquipZones.TryGetValue(id, out equipZone))
                return equipZone;

            return null;
        }

        public List<ZoneData> GetOutdoorZones()
        {
            List<ZoneData> zones = new List<ZoneData>();

            foreach (KeyValuePair<int, ZoneData> pair in m_dicZones)
            {
                if (pair.Value.BuildingID == null)
                    zones.Add(pair.Value);
            }

            return zones;
        }

        public bool LoadSpatial(IDataManager dataManager)
        {
            string strErrorMessage = null;

            m_dicBuildingGroups.Clear();
            m_dicBuildings.Clear();
            m_dicZones.Clear();
            m_dicEquipZones.Clear();

            List<BuildingGroup> buildingGroups = dataManager.GetSelectManager().SelectBuildingGroups(null, "", out strErrorMessage);

            if (buildingGroups == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadSpatial Error : " + strErrorMessage);
                return false;
            }

            // Key : BuildingGroup ID
            // Value : Parent ID
            Dictionary<int, int> dicBuildingGroupParents = new Dictionary<int, int>();

            foreach (BuildingGroup item in buildingGroups)
            {
                BuildingGroupData bg = new BuildingGroupData();

                bg.ID = item.ID;
                bg.ParentID = item.ParentID;
                bg.SiteID = item.SiteID;
                bg.TextCenter = item.TextCenter;
                bg.GroupName = item.GroupName;
                bg.DisplayText = item.DisplayText;
                bg.ParentID = item.ParentID;

                if (item.ParentID != null)
                    dicBuildingGroupParents[item.ID] = (int)item.ParentID;

                m_dicBuildingGroups[bg.ID] = bg;
            }

            foreach (KeyValuePair<int, int> pair in dicBuildingGroupParents)
            {
                BuildingGroupData bg, parent;

                if (m_dicBuildingGroups.TryGetValue(pair.Key, out bg) && m_dicBuildingGroups.TryGetValue(pair.Value, out parent))
                {
                    bg.Parent = parent;
                }
            }

            List<Building> buildings = dataManager.GetSelectManager().SelectBuildings(null, "", out strErrorMessage);

            if (buildings == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadSpatial Error : " + strErrorMessage);
                return false;
            }

            foreach (Building building in buildings)
            {
                BuildingData buildingData = new BuildingData();

                buildingData.ID = building.ID;
                buildingData.BroadcastText = building.BroadcastText;
                buildingData.BuildingCode = building.BuildingCode;
                buildingData.BuildingGroupID = building.BuildingGroupID;
                buildingData.BuildingName = building.BuildingName;
                buildingData.DisplayText = building.DisplayText;
                buildingData.MaxFloor = building.MaxFloor;
                buildingData.MinFloor = building.MinFloor;
                buildingData.TextCenter = building.TextCenter;

                BuildingGroupData bg;

                if (m_dicBuildingGroups.TryGetValue(buildingData.BuildingGroupID, out bg))
                {
                    bg.BuildingDatas.Add(buildingData);
                }

                m_dicBuildings[buildingData.ID] = buildingData;
            }

            List<Zone> zones = dataManager.GetSelectManager().SelectZones(null, "", out strErrorMessage);

            if (zones == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadSpatial Error : " + strErrorMessage);
                return false;
            }

            foreach (Zone zone in zones)
            {
                ZoneData zoneData = new ZoneData();
                zoneData.ID = zoneData.Datas.ZoneID = zone.ID;
                zoneData.ZoneName = zone.ZoneName;
                zoneData.BuildingID = zone.BuildingID;
                zoneData.FloorIndex = zone.FloorIndex;
                zoneData.AddFloor = zone.AddFloor;
                zoneData.Boundary = zone.Boundary;
                zoneData.TextCenter = zone.TextCenter;
                zoneData.BroadcastText = zone.BroadcastText;
                zoneData.DisplayText = zone.DisplayText;
                zoneData.SiteID = zone.SiteID;

                BuildingData building;
                
                if (zone.BuildingID != null && m_dicBuildings.TryGetValue((int)zone.BuildingID, out building))
                {
                    building.ZoneDatas.Add(zoneData);
                }

                m_dicZones[zone.ID] = zoneData;
            }

            List<SDMS.Model.Spatial.ZoneData> zoneDatas = dataManager.GetSelectManager().SelectZoneDatas(null, null, out strErrorMessage);

            if (zoneDatas == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadSpatial Error : " + strErrorMessage);
                return false;
            }

            foreach (SDMS.Model.Spatial.ZoneData zoneData in zoneDatas)
            {
                ZoneData data;

                if (m_dicZones.TryGetValue(zoneData.ZoneID, out data))
                {
                    data.Datas = zoneData;
                }
            }

            List<EquipmentZone> equipZones = dataManager.GetSelectManager().SelectEquipmentZones(null, "", out strErrorMessage);

            if (equipZones == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadSpatial Error : " + strErrorMessage);
                return false;
            }

            foreach (EquipmentZone equipZone in equipZones)
            {
                EquipmentZoneData equipZoneData = new EquipmentZoneData();

                equipZoneData.Boundary = equipZone.Boundary;
                equipZoneData.BroadcastText = equipZone.BroadcastText;
                equipZoneData.DisplayText = equipZone.DisplayText;
                equipZoneData.ID = equipZone.ID;
                equipZoneData.SiteID = equipZone.SiteID;
                equipZoneData.TextCenter = equipZone.TextCenter;
                equipZoneData.Type = equipZone.Type;
                equipZoneData.ZoneName = equipZone.ZoneName;
                equipZoneData.LinkedZoneIDs = equipZone.LinkedZoneIDs;

                ZoneData zone;

                foreach (int zoneID in equipZone.LinkedZoneIDs)
                {
                    if (m_dicZones.TryGetValue(zoneID, out zone))
                    {
                        zone.EquipmentZoneDatas.Add(equipZoneData);
                        equipZoneData.LinkedZoneDatas.Add(zone);
                    }
                }

                m_dicEquipZones[equipZoneData.ID] = equipZoneData;
            }

            return true;
        }

        // 건물그룹, 건물, 외부 Zone 정보를 새로 읽어온다.
        public bool ReloadOuters(IDataManager dataManager, out string strErrorMessage)
        {
            strErrorMessage = null;

            List<BuildingGroup> buildingGroups = dataManager.GetSelectManager().SelectBuildingGroups(null, "", out strErrorMessage);

            if (buildingGroups == null)
            {
                System.Diagnostics.Trace.WriteLine("ReloadOuters Error : " + strErrorMessage);
                return false;
            }

            foreach (BuildingGroup item in buildingGroups)
            {
                BuildingGroupData buildingGroup;

                if (m_dicBuildingGroups.TryGetValue(item.ID, out buildingGroup))
                {
                    buildingGroup.TextCenter = item.TextCenter;
                    buildingGroup.DisplayText = item.DisplayText;
                }
            }

            List<Building> buildings = dataManager.GetSelectManager().SelectBuildings(null, "", out strErrorMessage);

            if (buildings == null)
            {
                System.Diagnostics.Trace.WriteLine("ReloadOuters Error : " + strErrorMessage);
                return false;
            }

            foreach (Building building in buildings)
            {
                BuildingData buildingData;

                if (m_dicBuildings.TryGetValue(building.ID, out buildingData))
                {
                    buildingData.BuildingName = building.BuildingName;
                    buildingData.DisplayText = building.DisplayText;
                    buildingData.TextCenter = building.TextCenter;
                }
            }

            Dictionary<Zone.Fields, object> dicConditions = new Dictionary<Zone.Fields, object>();
            dicConditions[Zone.Fields.BuildingID] = null;

            List<Zone> zones = dataManager.GetSelectManager().SelectZones(dicConditions, "", out strErrorMessage);

            if (zones == null)
            {
                System.Diagnostics.Trace.WriteLine("ReloadOuters Error : " + strErrorMessage);
                return false;
            }

            foreach (Zone zone in zones)
            {
                ZoneData zoneData;

                if (m_dicZones.TryGetValue(zone.ID, out zoneData))
                {
                    zoneData.ZoneName = zone.ZoneName;
                    zoneData.TextCenter = zone.TextCenter;
                    zoneData.DisplayText = zone.DisplayText;
                }
            }

            return true;
        }

        // 실내공간 정보를 새로 읽어온다.
        public bool ReloadIndoorZone(IDataManager dataManager, int nZoneID, out string strErrorMessage)
        {
            bool isNullable;
            string strAdditionalConditions = string.Format("{0} like '%{1}%'", EquipmentZone.GetFieldName(EquipmentZone.Fields.LinkedZoneIDList, out isNullable), nZoneID);
            List<EquipmentZone> equipZones = dataManager.GetSelectManager().SelectEquipmentZones(null, strAdditionalConditions, out strErrorMessage);

            if (equipZones == null)
            {
                System.Diagnostics.Trace.WriteLine("ReloadIndoorZone Error : " + strErrorMessage);
                return false;
            }

            EquipmentZoneData equipZoneData;

            foreach (EquipmentZone equipZone in equipZones)
            {
                if (equipZone.LinkedZoneIDs.Contains(nZoneID))
                {
                    if (m_dicEquipZones.TryGetValue(equipZone.ID, out equipZoneData))
                    {
                        equipZoneData.BroadcastText = equipZone.BroadcastText;
                        equipZoneData.DisplayText = equipZone.DisplayText;
                        equipZoneData.TextCenter = equipZone.TextCenter;
                        equipZoneData.Type = equipZone.Type;
                        equipZoneData.ZoneName = equipZone.ZoneName;
                    }
                }
            }

            return true;
        }
    }
}
