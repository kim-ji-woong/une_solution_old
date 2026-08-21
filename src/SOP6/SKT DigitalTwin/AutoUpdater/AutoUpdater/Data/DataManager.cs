using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using UnE.Spatial;
using System.Configuration;

namespace AutoUpdater.Data
{
    public class DataManager
    {
        public const string FireAlarmHistoryTable = "WebFireAlarmHistory";
        public const string FireAlarmSensorZoneHistoryTable = "WebFireAlarmSensorZoneHistory";
        public const string FireAlarmFailHistoryTable = "WebFireAlarmFailHistory";

        // Key : Building ID
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        // Key : Building Code
        private Dictionary<string, Building> m_dicBuildingCodes = new Dictionary<string, Building>();
        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        private Dictionary<Building, List<Zone>> m_dicBuildingZones = new Dictionary<Building, List<Zone>>();
        private static Dictionary<int, int> m_dicSensorZoneTypes = new Dictionary<int, int>();
        private int m_nBaseBuildingGroupID = -1;

        private string m_strMapCodeList = "";
        // 지자체일 경우 해당 지자체 안에 포함된 모든 SiteID들을 보관한다.
        // Key, Value : SiteID
        private Dictionary<string, string> m_dicSiteIDs = new Dictionary<string, string>();
        private string m_strLocalSiteID = "";

        private WebDBManagerEx m_dbMgr = null;

        private static DataManager m_instance = null;

        public static DataManager Instance
        {
            get { return m_instance; }
        }

        public string SiteMapCodeList
        {
            get { return m_strMapCodeList; }
        }

        public string LocalSiteID
        {
            get { return m_strLocalSiteID; }
        }

        public WebDBManagerEx DBManager
        {
            get { return m_dbMgr; }
        }

        public int BaseBuildingGroupID
        {
            get { return m_nBaseBuildingGroupID; }
        }

        private DataManager()
        {
        }

        public static bool LoadData(WebDBManagerEx dbMgr)
        {
            m_instance = new DataManager();
            m_instance.m_dbMgr = dbMgr;
            m_instance.m_strLocalSiteID = dbMgr.SiteID.ToString();

            string strBaseBuildingGroupID = ConfigurationManager.AppSettings.Get("baseBuildingGroupID");

            if (strBaseBuildingGroupID != null && strBaseBuildingGroupID.Length >= 0)
            {
                int nBuildingGroupID;

                if (int.TryParse(strBaseBuildingGroupID, out nBuildingGroupID))
                    m_instance.m_nBaseBuildingGroupID = nBuildingGroupID;
            }

            if (m_instance.LoadBuildings())
            {
                if (m_instance.LoadZones())
                {
                    m_instance.LoadSensorZones();
                    Network.NetworkWebManager.InitInstance();
                    return true;
                }
            }

            return false;
        }

        private void LoadSensorZones()
        {
            string strSQL = "Select ID, Type from SensorZone";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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

        public bool GetSensorZoneType(int nSensorZoneID, out int nSensorZoneType)
        {
            return m_dicSensorZoneTypes.TryGetValue(nSensorZoneID, out nSensorZoneType);
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

            string strSQL = "Select ID, BuildingID, ZoneName, FloorIndex from Zone where SiteID = " + m_dbMgr.OriginalSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> id = WebDBManagerEx.GetIntField(arrResult[i].ToString());
                VariousData<int> buildingID = WebDBManagerEx.GetIntField(arrResult[i + 1].ToString());
                string strName = WebDBManagerEx.GetStringField(arrResult[i + 2]);
                VariousData<int> floorIndex = WebDBManagerEx.GetIntField(arrResult[i + 3].ToString());

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
                zone.ZoneName = strName;
                zone.FloorIndex = floorIndex.Data;
                zone.Building = building;

                m_dicZones[zone.ID] = zone;
                zones.Add(zone);
            }

            return true;
        }

        // Key, Value : BuildingGroup ID
        private Dictionary<int, int> GetOwnBuildingGroups()
        {
            if (m_nBaseBuildingGroupID < 0)
                return null;

            Dictionary<int, int> dicBuildingGroupIDs = new Dictionary<int, int>();

            dicBuildingGroupIDs[m_nBaseBuildingGroupID] = m_nBaseBuildingGroupID;
            ReadBuildingGroups(dicBuildingGroupIDs, m_nBaseBuildingGroupID.ToString());

            return dicBuildingGroupIDs;
        }

        private void ReadBuildingGroups(Dictionary<int, int> dicBuildingGroupIDs, string strParentIDs)
        {
            string strSQL = "Select ID from BuildingGroup where ParentID in (" + strParentIDs + ")";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

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
                ReadBuildingGroups(dicBuildingGroupIDs, strIDs);
        }

        private bool LoadBuildings()
        {
            if (m_dbMgr == null)
                return false;

            Dictionary<int, int> dicOwnBuildingGroupIDs = GetOwnBuildingGroups();

            m_dicBuildings.Clear();
            m_dicBuildingCodes.Clear();

            Dictionary<int, BuildingGroup> dicBuildingGroups = LoadBuildingGroups();

            if (dicBuildingGroups == null)
                return false;

            string strSQL = "Select ID, BuildingID, BuildingCode, BuildingName, BuildingGroupID, MaxFloor, MinFloor from Building";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            string strOwnSiteID = m_dbMgr.SiteID.ToString();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                VariousData<int> id = WebDBManagerEx.GetIntField(arrResult[i].ToString());
                // 원래 BuildingID인데 BLD_202에서는 SiteID로 사용된다.
                string strSiteID = WebDBManagerEx.GetStringField(arrResult[i + 1]);
                string strCode = WebDBManagerEx.GetStringField(arrResult[i + 2]);
                string strName = WebDBManagerEx.GetStringField(arrResult[i + 3]);
                VariousData<int> groupID = WebDBManagerEx.GetIntField(arrResult[i + 4].ToString());
                VariousData<int> max = WebDBManagerEx.GetIntField(arrResult[i + 5].ToString());
                VariousData<int> min = WebDBManagerEx.GetIntField(arrResult[i + 6].ToString());

                if (id == null || strSiteID == null || strCode == null || strName == null ||
                    groupID == null || max == null || min == null)
                    continue;

                BuildingGroup group;

                if (dicBuildingGroups.TryGetValue(groupID.Data, out group) == false)
                    continue;

                Building building = new Building();

                building.ID = id.Data;
                building.BuildingCode = strCode;
                building.BuildingName = strName;
                building.MaxFloorIndex = max.Data;
                building.MinFloorIndex = min.Data;
                building.BuildingGroup = group;

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
                    if (dicOwnBuildingGroupIDs.ContainsKey(group.GroupID))
                    {
                        if (m_strMapCodeList.Length == 0)
                            m_strMapCodeList = "'" + strCode + "'";
                        else
                            m_strMapCodeList += ", '" + strCode + "'";

                        m_dicSiteIDs[strSiteID] = strSiteID;
                    }
                }

                m_dicBuildings[building.ID] = building;
                m_dicBuildingCodes[building.BuildingCode] = building;
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

            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                VariousData<int> id = WebDBManagerEx.GetIntField(arrResult[i].ToString());
                VariousData<int> parentID = WebDBManagerEx.GetIntField(arrResult[i + 1].ToString());
                string strName = WebDBManagerEx.GetStringField(arrResult[i + 2]);

                if (id == null || strName == null)
                    continue;

                BuildingGroup group = new BuildingGroup();

                group.GroupID = id.Data;
                group.BuildingGroupName = strName;

                if (parentID != null)
                    dicParents[group.GroupID] = parentID.Data;

                dicBuildingGroups[group.GroupID] = group;
            }

            foreach (KeyValuePair<int, int> pair in dicParents)
            {
                BuildingGroup child, parent;

                if (dicBuildingGroups.TryGetValue(pair.Key, out child) && dicBuildingGroups.TryGetValue(pair.Value, out parent))
                {
                    child.Parent = parent;
                }
            }

            return dicBuildingGroups;
        }

        public int GetMaxTableID(string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return -1;

            if (arrResult.Count == 0)
                return 0;

            VariousData<int> id = WebDBManagerEx.GetIntField(arrResult[0].ToString());

            if (id == null)
                return 0;

            return id.Data;
        }

        public bool ContainsSiteID(string strSiteID)
        {
            return m_dicSiteIDs.ContainsKey(strSiteID);
        }
    }
}
