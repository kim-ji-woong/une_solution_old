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
        public const string FireAlarmHistoryTable = "WebFireAlarmHistory";
        public const string FireAlarmFailHistoryTable = "WebFireAlarmFailHistory";

        // Key : Building ID
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        // Key : Building Code
        private Dictionary<string, Building> m_dicBuildingCodes = new Dictionary<string, Building>();
        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        private Dictionary<Building, List<Zone>> m_dicBuildingZones = new Dictionary<Building, List<Zone>>();
        // Building Code별 Site ID
        private Dictionary<string, string> m_dicBuildingCodeSiteIDs = new Dictionary<string, string>();
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

        private DataManager()
        {
        }

        public static bool LoadData()
        {
            m_instance = new DataManager();
            m_instance.m_dbMgr = MakeDBManager();

            if (m_instance.LoadBuildings())
            {
                if (m_instance.LoadZones())
                {
                    Network.NetworkWebManager.InitInstance();
                    return true;
                }
            }

            return false;
        }

        public Building GetBuilding(string strBuildingCode)
        {
            Building building;

            if (m_dicBuildingCodes.TryGetValue(strBuildingCode, out building))
                return building;

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
                int floorIndex = GetFloorIndex(strFloor, true);

                if (floorIndex < 0)
                    return null;
                else
                    nFloorIndex = -floorIndex;
            }
            else if (strFloor.EndsWith("f"))
            {
                int floorIndex = GetFloorIndex(strFloor, false);

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

        private int GetFloorIndex(string strFloor, bool fromBegining)
        {
            if (fromBegining)
                strFloor = strFloor.Substring(1).Trim();
            else
                strFloor = strFloor.Substring(0, strFloor.Length - 1).Trim();

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

            string strSQL = "Select ID, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor from Building";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strSiteID = WebDBManager.GetStringField(arrResult[i + 1]);
                string strCode = WebDBManager.GetStringField(arrResult[i + 2]);
                string strName = WebDBManager.GetStringField(arrResult[i + 3]);
                VariousData<int> groupID = WebDBManager.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> max = WebDBManager.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> min = WebDBManager.GetIntField(arrResult[i + 6].ToString());

                if (id == null || strSiteID == null || strCode == null || strName == null ||
                    groupID == null || max == null || min == null)
                    continue;

                BuildingGroup group;

                if (dicBuildingGroups.TryGetValue(groupID.Data, out group) == false)
                    continue;

                m_dicBuildingCodeSiteIDs[strCode] = strSiteID;

                Building building = new Building();

                building.ID = id.Data;
                building.SiteID = strSiteID;
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

        public string GetSiteID(string strMapCode)
        {
            string strSiteID;

            if (m_dicBuildingCodeSiteIDs.TryGetValue(strMapCode, out strSiteID))
                return strSiteID;

            return null;
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
        }

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

        public static string GetDBName(string strSiteID)
        {
            return "BLD_" + strSiteID;
        }
    }
}