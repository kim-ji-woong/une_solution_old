using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using System.Configuration;

namespace SensorSimulator.Data
{
    public static class DataManager
    {
        // Key : BuildingGroup ID
        private static Dictionary<int, BuildingGroup> m_dicBuildingGroups = new Dictionary<int, BuildingGroup>();
        // Key : Building ID
        private static Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        // Key : Building Code
        private static Dictionary<string, Building> m_dicBuildingCodes = new Dictionary<string, Building>();
        private static Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        private static Dictionary<Building, List<Zone>> m_dicBuildingZones = new Dictionary<Building, List<Zone>>();
        private static Dictionary<int, int> m_dicSensorZoneTypes = new Dictionary<int, int>();
        // Key : Building Code
        // Value : Site ID
        private static Dictionary<string, string> m_dicBuildingCodeSiteIDs = new Dictionary<string, string>();


        private static int m_nBaseBuildingGroupID = -1;
        private static string m_strMapCodeList = "";
        public static string SiteMapCodeList
        {
            get { return m_strMapCodeList; }
        }

        public static int BaseBuildingGroupID
        {
            get { return m_nBaseBuildingGroupID; }
        }

        public static List<BuildingGroup> RootBuildingGroups
        {
            get
            {
                List<BuildingGroup> buildingGroups = new List<BuildingGroup>();

                foreach (KeyValuePair<int, BuildingGroup> pair in m_dicBuildingGroups)
                {
                    if (pair.Value.ParentGroup == null)
                        buildingGroups.Add(pair.Value);
                }

                return buildingGroups;
            }
        }

        public static List<Zone> GetZones(Building building)
        {
            List<Zone> zones;

            if (m_dicBuildingZones.TryGetValue(building, out zones))
                return zones;

            return null;
        }

        public static Building GetBuilding(string strMapCode)
        {
            Building building;

            if (m_dicBuildingCodes.TryGetValue(strMapCode, out building))
                return building;

            return null;
        }

        public static Zone GetZone(int nZoneID)
        {
            Zone zone;

            if (m_dicZones.TryGetValue(nZoneID, out zone))
                return zone;

            return null;
        }

        public static Zone GetZone(Building building, string strFloor)
        {
            List<Zone> zones;

            if (m_dicBuildingZones.TryGetValue(building, out zones) == false)
                return null;

            strFloor = strFloor.ToLower();

            int nFloorIndex = -1;

            if (strFloor.StartsWith("b"))
            {
                int floorIndex = GetFloorIndex(strFloor.Substring(1).Trim());

                if (floorIndex < 0)
                    return null;
                else
                    nFloorIndex = -floorIndex;
            }
            else if (strFloor.EndsWith("f"))
            {
                int floorIndex = GetFloorIndex(strFloor.Substring(0, strFloor.Length - 1).Trim());

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

        private static int GetFloorIndex(string strFloor)
        {
            //strFloor = strFloor.Substring(1).Trim();
            int nFloorIndex;

            if (int.TryParse(strFloor, out nFloorIndex))
                return nFloorIndex;

            return -1;
        }

        public static bool GetSensorZoneType(int nSensorZoneID, out int nSensorZoneType)
        {
            return m_dicSensorZoneTypes.TryGetValue(nSensorZoneID, out nSensorZoneType);
        }

        public static bool InitData(WebDBManager dbMgr)
        {
            string strBaseBuildingGroupID = ConfigurationManager.AppSettings.Get("baseBuildingGroupID");

            if (strBaseBuildingGroupID != null && strBaseBuildingGroupID.Length >= 0)
            {
                int nBuildingGroupID;

                if (int.TryParse(strBaseBuildingGroupID, out nBuildingGroupID))
                    m_nBaseBuildingGroupID = nBuildingGroupID;
            }

            if (LoadBuildings(dbMgr))
            {
                if (LoadZones(dbMgr))
                {
                    LoadSensorZones(dbMgr);
                    return true;
                }
            }

            return false;
        }

        private static void LoadSensorZones(WebDBManager dbMgr)
        {            
            string strSQL = "Select ID, Type from SensorZone";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> type = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (id == null || type == null)
                    continue;

                m_dicSensorZoneTypes[id.Data] = type.Data;
            }
        }

        private static bool LoadZones(WebDBManager dbMgr)
        {
            if (dbMgr == null)
                return false;

            m_dicZones.Clear();
            m_dicBuildingZones.Clear();

            ArrayList arrResult = null;
            if (m_nBaseBuildingGroupID > 0)
            {
                string strSQL = "Select ID, BuildingID, ZoneName, FloorIndex from Zone where SiteID = " + dbMgr.SiteID.ToString();
                arrResult = dbMgr.GetResultData(strSQL);
            }
            else
            {
                int nSiteID = dbMgr.SiteID;
                string strDBName = dbMgr.DatabaseName;

                if (FormMain.Instance.LocalSiteID > 0 && FormMain.Instance.strLocalDBName != null)
                {
                    nSiteID = FormMain.Instance.LocalSiteID;
                    strDBName = FormMain.Instance.strLocalDBName;
                }

                string strSQL = "Select ID, BuildingID, ZoneName, FloorIndex from Zone where SiteID = " + nSiteID;
                arrResult = dbMgr.GetResultData(strSQL, strDBName);
            }

            

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
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

        private static bool LoadBuildings(WebDBManager dbMgr)
        {
            m_dicBuildings.Clear();
            m_dicBuildingCodes.Clear();
            m_dicBuildingGroups.Clear();

            Dictionary<int, BuildingGroup> dicBuildingGroups = LoadBuildingGroups(dbMgr);
            if (dicBuildingGroups == null)
                return false;

            m_dicBuildingGroups = dicBuildingGroups;

            Dictionary<int, int> dicOwnBuildingGroupIDs = null;

            ArrayList arrResult = null;
            if (m_nBaseBuildingGroupID > 0)
            {
                dicOwnBuildingGroupIDs = GetOwnBuildingGroups(dbMgr);

                string strSQL = "Select ID, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor from Building";
                arrResult = dbMgr.GetResultData(strSQL);
            }
            else
            {
                int nSiteID = dbMgr.SiteID;
                string strDBName = dbMgr.DatabaseName;

                if (FormMain.Instance.LocalSiteID > 0 && FormMain.Instance.strLocalDBName != null)
                {
                    nSiteID = FormMain.Instance.LocalSiteID;
                    strDBName = FormMain.Instance.strLocalDBName;
                }

                string strSQL = "Select ID, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor from Building Where BuildingID = " + nSiteID;
                arrResult = dbMgr.GetResultData(strSQL, strDBName);
            }

            if (arrResult == null)
                return false;

            string strOwnSiteID = dbMgr.SiteID.ToString();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                // 원래 BuildingID인데 BLD_202에서는 SiteID로 사용된다.
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

                Building building = new Building();

                building.ID = id.Data;
                building.Code = strCode;
                building.Name = strName;
                building.MaxFloorIndex = max.Data;
                building.MinFloorIndex = min.Data;
                building.BuildingGroup = group;

                m_dicBuildingCodeSiteIDs[strCode] = strSiteID;

                if (dicOwnBuildingGroupIDs == null)
                {
                    if (strSiteID == strOwnSiteID)
                    {
                        if (m_strMapCodeList.Length == 0)
                            m_strMapCodeList = "'" + strCode + "'";
                        else
                            m_strMapCodeList += ", '" + strCode + "'";
                    }
                }
                else
                {
                    if (dicOwnBuildingGroupIDs.ContainsKey(group.ID))
                    {
                        if (m_strMapCodeList.Length == 0)
                            m_strMapCodeList = "'" + strCode + "'";
                        else
                            m_strMapCodeList += ", '" + strCode + "'";
                    }
                }

                m_dicBuildings[building.ID] = building;
                m_dicBuildingCodes[building.Code] = building;
            }

            return true;
        }

        private static Dictionary<int, BuildingGroup> LoadBuildingGroups(WebDBManager dbMgr)
        {
            ArrayList arrResult = null;
            //if (m_nBaseBuildingGroupID > 0)
            //{
            //    string strSQL = "Select ID, ParentID, GroupName from BuildingGroup";
            //    arrResult = dbMgr.GetResultData(strSQL);
            //}
            //else
            //{
                int nSiteID = dbMgr.SiteID;
                string strDBName = dbMgr.DatabaseName;

                if (FormMain.Instance.LocalSiteID > 0 && FormMain.Instance.strLocalDBName != null)
                {
                    nSiteID = FormMain.Instance.LocalSiteID;
                    strDBName = FormMain.Instance.strLocalDBName;
                }

                string strSQL = "Select ID, ParentID, GroupName from BuildingGroup Where SiteID = " + nSiteID;
                arrResult = dbMgr.GetResultData(strSQL, strDBName);
            //}

            if (arrResult == null)
                return null;

            // Key : Child
            // Value : Parent
            Dictionary<int, int> dicParents = new Dictionary<int, int>();
            Dictionary<int, BuildingGroup> dicBuildingGroups = new Dictionary<int, BuildingGroup>();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 2; i += 3)
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

        // Key, Value : BuildingGroup ID
        private static Dictionary<int, int> GetOwnBuildingGroups(WebDBManager dbMgr)
        {
            if (m_nBaseBuildingGroupID < 0)
                return null;

            Dictionary<int, int> dicBuildingGroupIDs = new Dictionary<int, int>();

            dicBuildingGroupIDs[m_nBaseBuildingGroupID] = m_nBaseBuildingGroupID;
            ReadBuildingGroups(dicBuildingGroupIDs, m_nBaseBuildingGroupID.ToString(), dbMgr);

            return dicBuildingGroupIDs;
        }

        private static void ReadBuildingGroups(Dictionary<int, int> dicBuildingGroupIDs, string strParentIDs, WebDBManager dbMgr)
        {
            string strSQL = "Select ID from BuildingGroup where ParentID in (" + strParentIDs + ")";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            string strIDs = "";
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount; i++)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());

                if (id == null)
                    continue;

                dicBuildingGroupIDs[id.Data] = id.Data;

                if (strIDs.Length == 0)
                    strIDs = id.Data.ToString();
                else
                    strIDs += ", " + id.Data.ToString();
            }

            if (strIDs.Length > 0)
                ReadBuildingGroups(dicBuildingGroupIDs, strIDs, dbMgr);
        }

        public static string GetSiteID(string strBuildingCode)
        {
            string strSiteID = "";

            if (m_dicBuildingCodeSiteIDs.TryGetValue(strBuildingCode, out strSiteID))
                return strSiteID;

            return "";
        }
    }
}
