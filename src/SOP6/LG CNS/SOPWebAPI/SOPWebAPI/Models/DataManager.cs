using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBUtility2;
using System.Collections;

namespace SOPWebAPI.Models
{
    public class DataManager
    {
        private enum EquipZoneType { FireType = 0, PSMType = 3 };

        public const string AlarmHistoryTable = "WebAlarmHistory";
        public const string AlarmFailHistoryTable = "WebAlarmFailHistory";

        // Key : Building ID
        private Dictionary<int, BuildingGroup> m_dicBuildingGroups = new Dictionary<int, BuildingGroup>();
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        private Dictionary<string, Zone> m_dicZoneNames = new Dictionary<string, Zone>();
        private WebDBManager m_dbMgr = null;

        private static DataManager m_instance = null;

        public static DataManager Instance
        {
            get { return m_instance; }
        }

        public WebDBManager DBManager
        {
            get { return m_dbMgr; }
        }

        /*public static void WriteLog(string strLog)
        {
            string strSQL = "Insert into WebAPIDebug (ID, RecvTime, Message) ";
            strSQL += string.Format("Select isnull(max(id) + 1, 1), getdate(), '{0}' from WebAPIDebug", strLog);
            m_instance.m_dbMgr.GetResultData(strSQL);
        }*/

        private DataManager()
        {
        }

        public static bool LoadData()
        {
            m_instance = new DataManager();
            m_instance.m_dbMgr = MakeDBManager();
            //WriteLog("LoadData");

            //if (m_instance.LoadBuildings())
            {
                //if (m_instance.LoadZones())
                {
                    Network.NetworkWebManager.InitInstance();
                    return true;
                }
            }

            return false;
        }

        public Zone GetZone(string strZoneName)
        {
            Zone zone = null;

            if (m_dicZoneNames.TryGetValue(strZoneName, out zone))
                return zone;

            string strSQL = "Select ID, BuildingID from Zone where ZoneName = '" + strZoneName + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-1;i+=2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null)
                    continue;

                Building building = null;

                if (buildingID != null && buildingID.Data > 0)
                {
                    building = GetBuilding(buildingID.Data);
                }

                zone = new Zone();
                zone.ID = id.Data;
                zone.Name = strZoneName;
                zone.Building = building;
                return zone;
            }

            return null;
        }

        public Building GetBuilding(int nBuildingID)
        {
            Building building = null;

            if (m_dicBuildings.TryGetValue(nBuildingID, out building))
                return building;

            string strSQL = "Select b.BuildingName, bg.ID, bg.GroupName from Building as b, BuildingGroup as bg where b.BuildingGroupID = bg.ID and b.ID = " + nBuildingID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                string strBuildingName = WebDBManager.GetStringField(arrResult[i]);
                VariousData<int> buildingGroupID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strBuildingGroupName = WebDBManager.GetStringField(arrResult[i + 2]);

                if (strBuildingName == null || buildingGroupID == null || strBuildingGroupName == null)
                    continue;

                BuildingGroup buildingGroup = null;

                if (m_dicBuildingGroups.TryGetValue(buildingGroupID.Data, out buildingGroup) == false)
                {
                    buildingGroup = new BuildingGroup();
                    buildingGroup.ID = buildingGroupID.Data;
                    buildingGroup.Name = strBuildingGroupName;
                    m_dicBuildingGroups[buildingGroup.ID] = buildingGroup;
                }

                building = new Building();
                building.ID = nBuildingID;
                building.Name = strBuildingName;
                building.BuildingGroup = buildingGroup;
                break;
            }

            return building;
        }

        /*public Building GetBuilding(string strBuildingCode)
        {
            Building building;

            if (m_dicBuildingCodes.TryGetValue(strBuildingCode, out building))
                return building;

            return null;
        }

        public Zone GetZone(string strZoneName)
        {
            Zone zone;

            if (m_dicZoneNames.TryGetValue(strZoneName, out zone))
                return zone;

            return null;
        }

        public Zone GetZone(Building building, string strFloor)
        {
            List<Zone> zones;

            if (m_dicBuildingZones.TryGetValue(building, out zones) == false)
                return null;

            strFloor = strFloor.ToLower();

            int nFloorIndex = -1;

            if (strFloor.StartsWith("b"))
            {
                int floorIndex = GetFloorIndex(strFloor);

                if (floorIndex < 0)
                    return null;
                else
                    nFloorIndex = -floorIndex;
            }
            else if (strFloor.StartsWith("f"))
            {
                int floorIndex = GetFloorIndex(strFloor);

                if (floorIndex < 0)
                    return null;
                else
                    nFloorIndex = floorIndex - 1;
            }
            else
                return null;

            foreach (Zone zone in zones)
            {
                if (zone.FloorIndex == nFloorIndex)
                    return zone;
            }

            return null;
        }

        private int GetFloorIndex(string strFloor)
        {
            strFloor = strFloor.Substring(1).Trim();
            int nFloorIndex;

            if (int.TryParse(strFloor, out nFloorIndex))
                return nFloorIndex;

            return -1;
        }

        private bool LoadZones()
        {
            if (m_dbMgr == null)
                return false;

            m_dicZones.Clear();
            m_dicZoneNames.Clear();
            m_dicBuildingZones.Clear();

            string strSQL = "Select ID, BuildingID, ZoneName, FloorIndex from Zone where SiteID = " + m_dbMgr.SiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> floorIndex = WebDBManager.GetIntField(arrResult[i + 3].ToString());

                if (id == null || buildingID == null || strName == null || floorIndex == null)
                    continue;

                Building building;

                if (m_dicBuildings.TryGetValue(buildingID.Data, out building) == false)
                    continue;

                List<Zone> zones;

                if (m_dicBuildingZones.TryGetValue(building, out zones) == false)
                {
                    zones = new List<Zone>();
                    m_dicBuildingZones[building] = zones;
                }

                Zone zone = new Zone();

                zone.ID = id.Data;
                zone.Name = strName;
                zone.FloorIndex = floorIndex.Data;
                zone.Building = building;

                m_dicZoneNames[zone.Name] = zone;
                m_dicZones[zone.ID] = zone;
                zones.Add(zone);
            }

            return true;
        }

        private bool LoadBuildings()
        {
            if (m_dbMgr == null)
                return false;

            m_dicBuildings.Clear();
            m_dicBuildingCodes.Clear();

            Dictionary<int, BuildingGroup> dicBuildingGroups = LoadBuildingGroups();

            if (dicBuildingGroups == null)
                return false;

            string strSQL = "Select ID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor from Building";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strCode = WebDBManager.GetStringField(arrResult[i + 1]);
                string strName = WebDBManager.GetStringField(arrResult[i + 2]);
                VariousData<int> groupID = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<int> max = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> min = WebDBManager.GetIntField(arrResult[i + 5].ToString());

                if (id == null || strCode == null || strName == null ||
                    groupID == null || max == null || min == null)
                    continue;

                BuildingGroup group;

                if (dicBuildingGroups.TryGetValue(groupID.Data, out group) == false)
                    continue;

                Building building = new Building();

                building.ID = id.Data;
                building.Code = strCode;
                building.Name = strName;
                building.MaxFloorIndex = max.Data;
                building.MinFloorIndex = min.Data;
                building.BuildingGroup = group;

                m_dicBuildings[building.ID] = building;
                m_dicBuildingCodes[building.Code] = building;
            }

            return true;
        }

        private Dictionary<int, BuildingGroup> LoadBuildingGroups()
        {
            string strSQL = "Select ID, ParentID, GroupName from BuildingGroup";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            // Key : Child
            // Value : Parent
            Dictionary<int, int> dicParents = new Dictionary<int, int>();
            Dictionary<int, BuildingGroup> dicBuildingGroups = new Dictionary<int, BuildingGroup>();
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> parentID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 2]);

                if (id == null || strName == null)
                    continue;

                BuildingGroup group = new BuildingGroup();

                group.ID = id.Data;
                group.Name = strName;

                if (parentID != null)
                    dicParents[group.ID] = parentID.Data;

                dicBuildingGroups[group.ID] = group;
            }

            foreach (KeyValuePair<int, int> pair in dicParents)
            {
                BuildingGroup child, parent;

                if (dicBuildingGroups.TryGetValue(pair.Key, out child) && dicBuildingGroups.TryGetValue(pair.Value, out parent))
                {
                    child.ParentGroup = parent;
                }
            }

            return dicBuildingGroups;
        }*/

        private static WebDBManager MakeDBManager()
        {
            List<string> tags = new List<string>();
            tags.Add("siteid");
            tags.Add("webserver");
            tags.Add("dbname");
            tags.Add("dbtype");

            foreach (string key in System.Configuration.ConfigurationManager.AppSettings.AllKeys)
            {
                tags.Remove(key);
            }


            if (tags.Count > 0)
                return null;

            string strSiteID = System.Configuration.ConfigurationManager.AppSettings["siteid"].ToString();
            string strWebServerURL = System.Configuration.ConfigurationManager.AppSettings["webserver"].ToString();
            string strDBName = System.Configuration.ConfigurationManager.AppSettings["dbname"].ToString();
            string strDBType = System.Configuration.ConfigurationManager.AppSettings["dbtype"].ToString();

            int nDBType;

            if (int.TryParse(strDBType, out nDBType) == false)
                return null;

            int nSiteID = 0;

            if (int.TryParse(strSiteID, out nSiteID) == false)
                return null;

            WebDBManager dbMgr = new WebDBManager(nSiteID);
            dbMgr.WebServerURL = strWebServerURL;
            dbMgr.DatabaseName = strDBName;
            dbMgr.DatabaseType = (WebDBManager.DBType)nDBType;

            return dbMgr;
        }

        public int GetMaxTableID(string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());

            if (id == null)
                return 0;

            return id.Data;
        }
    }
}