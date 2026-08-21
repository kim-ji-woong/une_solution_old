using System;
using System.Collections.Generic;
using DBUtility2;

namespace FireSimulator
{
    public class DataManager
    {
        public static bool UpdateDB(WebDBManager dbMgr, Project project, out string strErrorMessage)
        {
            strErrorMessage = null;

            if (DeleteDB(dbMgr, ref strErrorMessage) == false)
                return false;

            Dictionary<int, Building> dicBuildings = CreateBuilding(dbMgr, project, ref strErrorMessage);

            if (dicBuildings == null)
                return false;

            Dictionary<int, Zone> dicZones = CreateZone(dbMgr, project, ref strErrorMessage);

            if (dicBuildings == null)
                return false;

            if (RestartServer(dbMgr, ref strErrorMessage) == false)
                return false;

            return true;
        }

        private static bool RestartServer(WebDBManager dbMgr, ref string strErrorMessage)
        {
            string strSQL = "Update RestartServer set Restart = 1 where ID = 1";

            if (dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        private static Dictionary<int, Zone> CreateZone(WebDBManager dbMgr, Project project, ref string strErrorMessage)
        {
            Dictionary<int, Zone> dicZones = new Dictionary<int, Zone>();
            int nLevelCount = project.Levels.Count;
            int nEquipZoneCount = 0;

            for (int i=0;i<nLevelCount;i++)
            {
                Zone zone = new Zone();

                zone.ID = i + 1;
                zone.BuildingID = 1;
                zone.FloorIndex = i;
                zone.ZoneName = project.Levels[i].Name;

                string strSQL = string.Format("Insert into Zone (ID, ZoneName, SiteID, BuildingID, FloorIndex, DisplayText) values ({0}, '{1}', {2}, {3}, {4}, '{1}')",
                    zone.ID, zone.ZoneName, dbMgr.SiteID, zone.BuildingID, zone.FloorIndex);

                if (dbMgr.GetResultData(strSQL) == null)
                {
                    strErrorMessage = dbMgr.LastErrorMessage;
                    return null;
                }

                dicZones[zone.ID] = zone;

                if (CreateEquipmentZone(dbMgr, project.Levels[i], zone, ref nEquipZoneCount, ref strErrorMessage) == false)
                    return null;
            }

            return dicZones;
        }

        private static bool CreateEquipmentZone(WebDBManager dbMgr, Level level, Zone zone, ref int nEquipZoneCount, ref string strErrorMessage)
        {
            foreach (Space space in level.Spaces)
            {
                EquipmentZone equipZone = new EquipmentZone();

                equipZone.ID = ++nEquipZoneCount;
                equipZone.ZoneID = zone.ID;
                equipZone.ZoneName = space.Name;

                string strSQL = string.Format("Insert into EquipmentZone (ID, ZoneName, Boundary, LinkedZoneIDList, Type, BroadcastName, DisplayText, SiteID) values ({0}, '{1}', '', '{2}', 0, '{1}', '{1}', {3})",
                    equipZone.ID, equipZone.ZoneName, equipZone.ZoneID, dbMgr.SiteID);

                if (dbMgr.GetResultData(strSQL) == null)
                {
                    strErrorMessage = dbMgr.LastErrorMessage;
                    return false;
                }

                zone.EquipZones.Add(equipZone);

                if (CreateFireSensor(dbMgr, zone, equipZone, ref strErrorMessage) == false)
                    return false;
            }

            return true;
        }

        private static bool CreateFireSensor(WebDBManager dbMgr, Zone zone, EquipmentZone equipZone, ref string strErrorMessage)
        {
            string strSQL = string.Format("Insert into FireSensor (ID, Name, X, Y, Z, ZoneID, IsIndoor) values ({0}, '{1}', 0, 0, 0, {2}, 1)",
                equipZone.ID, equipZone.ZoneName + " 화재센서", zone.ID);

            if (dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = dbMgr.LastErrorMessage;
                return false;
            }

            strSQL = string.Format("Insert into SensorZone (ID, Type, EquipZoneID, OrgSensorID, Zone) values ({0}, 0, {1}, {1}, {2})", equipZone.ID, equipZone.ID, zone.ID);

            if (dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = dbMgr.LastErrorMessage;
                return false;
            }

            strSQL = string.Format("Insert into SensorTagInfo (ID, SensorServerID, TagNo, SensorName, SensorType, DeActivate, EquipZoneID, SensorZoneID) values ({0}, 1, {0}, '{1}', 0, 'N', {0}, {0})",
                equipZone.ID, equipZone.ZoneName + " 화재센서");

            if (dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }

        private static Dictionary<int, Building> CreateBuilding(WebDBManager dbMgr, Project project, ref string strErrorMessage)
        {
            int nLevelCount = project.Levels.Count;

            if (nLevelCount == 0)
            {
                strErrorMessage = "건물 내부가 비어있습니다.";
                return null;
            }

            Building building = new Building();

            building.ID = 1;
            building.BuildingName = project.Name;
            building.MinFloor = 0;
            building.MaxFloor = nLevelCount - 1;

            string strSQL = string.Format("Insert into Building (ID, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor, BroadcastingText, DisplayText) values ({0}, '', '', '{1}', {2}, {3}, {4}, null, null)",
                building.ID, building.BuildingName, building.BuildingGroupID, building.MaxFloor, building.MinFloor);

            if (dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = dbMgr.LastErrorMessage;
                return null;
            }

            Dictionary<int, Building> dicBuildings = new Dictionary<int, Building>();
            dicBuildings[building.ID] = building;
            return dicBuildings;
        }

        private static bool DeleteDB(WebDBManager dbMgr, ref string strErrorMessage)
        {
            if (DeleteTable(dbMgr, "ComponentHistoryDetail", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "ComponentHistory", ref strErrorMessage) == false)
                return false;

            //if (DeleteTable(dbMgr, "ActionStepAutoClose", ref strErrorMessage) == false)
            //    return false;

            if (DeleteTable(dbMgr, "ActionStepHistory", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "FireSensor", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "FireSensorSOPLink", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "SensorReactionHistoryDescriptionText", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "SensorReactionHistoryDescription", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "SensorReactionHistory", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "SensorZoneHistory", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "SensorTagInfo", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "SensorZone", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "equipmentZone", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "Zone", ref strErrorMessage) == false)
                return false;

            if (DeleteTable(dbMgr, "Building", ref strErrorMessage) == false)
                return false;

            return true;
        }

        private static bool DeleteTable(WebDBManager dbMgr, string strTableName, ref string strErrorMessage)
        {
            string strSQL = string.Format("Delete from {0} where ID >= 0", strTableName);

            if (dbMgr.GetResultData(strSQL) == null)
            {
                strErrorMessage = dbMgr.LastErrorMessage;
                return false;
            }

            return true;
        }
    }
}
