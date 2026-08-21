using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace RtspUrlEditor
{
    public class DataManager
    {
        private Dictionary<int, List<Building>> m_dicBuildingGroupBuildings = new Dictionary<int, List<Building>>();
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        private Dictionary<int, CCTV> m_dicCCTV = new Dictionary<int, CCTV>();
        private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();
        private List<Zone> m_outdoorZones = new List<Zone>();

        public Dictionary<int, List<Building>> BuildingGroups
        {
            get { return m_dicBuildingGroupBuildings; }
        }

        public List<Zone> OutdoorZones
        {
            get { return m_outdoorZones; }
        }

        public List<Zone> Zones
        {
            get { return m_dicZones.Values.ToList(); }
        }

        public List<CCTV> CCTVs
        {
            get { return m_dicCCTV.Values.ToList(); }
        }

        public bool ReadDatas(WebDBManager dbMgr)
        {
            if (ReadBuildings(dbMgr))
            {
                if (ReadZones(dbMgr))
                {
                    if (ReadEquipZones(dbMgr) == false)
                        return false;

                    return ReadCCTV(dbMgr);
                }
            }

            return false;
        }

        private bool ReadEquipZones(WebDBManager dbMgr)
        {
            string strSQL = "Select ID, ZoneName, LinkedZoneIDList from EquipmentZone where SiteID = " + dbMgr.SiteID;
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strLinkedZoneList = WebDBManager.GetStringField(arrResult[i + 2]);

                if (id == null || strName == null || strLinkedZoneList == null)
                    continue;

                EquipmentZone equipZone = new EquipmentZone();
                equipZone.ID = id.Data;
                equipZone.Name = strName;

                m_dicEquipZones[equipZone.ID] = equipZone;

                string[] tokens = strLinkedZoneList.Split(',');

                foreach (string strID in tokens)
                {
                    int nZoneID;
                    Zone zone;

                    if (int.TryParse(strID.Trim(), out nZoneID))
                    {
                        if (m_dicZones.TryGetValue(nZoneID, out zone))
                        {
                            zone.EquipZones.Add(equipZone);
                        }
                    }
                }
            }

            return true;
        }

        private bool ReadBuildings(WebDBManager dbMgr)
        {
            string strSQL = "Select b.ID, b.BuildingName, g.ID, g.GroupName from Building as b, BuildingGroup as g where b.BuildingGroupID = g.ID";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i].ToString());
                string strBuildingName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> buildingGroupID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strBuildingGroupName = WebDBManager.GetStringField(arrResult[i + 3]);

                if (buildingID == null || strBuildingName == null || buildingGroupID == null || strBuildingGroupName == null)
                    continue;

                Building building = new Building();

                building.ID = buildingID.Data;
                building.BuildingName = strBuildingName;
                building.BuildingGroupID = buildingGroupID.Data;
                building.BuildingGroupName = strBuildingGroupName;

                m_dicBuildings[building.ID] = building;

                List<Building> buildings;

                if (m_dicBuildingGroupBuildings.TryGetValue(building.BuildingGroupID, out buildings) == false)
                {
                    buildings = new List<Building>();
                    m_dicBuildingGroupBuildings[building.BuildingGroupID] = buildings;
                }

                buildings.Add(building);
            }

            return true;
        }

        private bool ReadZones(WebDBManager dbMgr)
        {
            string strSQL = "Select ID, ZoneName, BuildingID, FloorIndex, AddFloor from Zone where SiteID = " + dbMgr.SiteID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> buildingID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> floorIndex = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                float fAddFloor = WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0);

                if (id == null || strZoneName == null || buildingID == null || floorIndex == null)
                    continue;

                Zone zone = new Zone();

                zone.ID = id.Data;
                zone.BuildingID = buildingID.Data;
                zone.ZoneName = strZoneName;
                zone.FloorIndex = ((float)floorIndex.Data) + fAddFloor;

                m_dicZones[zone.ID] = zone;

                Building building;

                if (zone.BuildingID < 0)
                    m_outdoorZones.Add(zone);
                else
                {
                    if (m_dicBuildings.TryGetValue(zone.BuildingID, out building))
                        building.Zones.Add(zone);
                }
            }

            foreach (KeyValuePair<int, Building> pair in m_dicBuildings)
            {
                pair.Value.Zones.Sort();
            }

            m_outdoorZones.Sort();
            return true;
        }

        private bool ReadCCTV(WebDBManager dbMgr)
        {
            string strSQL = "Select ID, CameraName, ZoneID, URL from CCTV where Type = 'RTSP'";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 3; i += 4)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                string strCameraName = WebDBManager.GetStringField(arrResult[i + 1]);
                VariousData<int> zoneID = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                string strURL = WebDBManager.GetStringField(arrResult[i + 3]);

                if (id == null || strCameraName == null || zoneID == null)
                    continue;

                if (strURL == null)
                    strURL = "";

                Zone zone;

                if (m_dicZones.TryGetValue(zoneID.Data, out zone) == false)
                    zone = null;

                CCTV cctv = new CCTV();

                cctv.ID = id.Data;
                cctv.CCTVName = strCameraName;
                cctv.Zone = zone;
                cctv.URL = strURL;

                m_dicCCTV[cctv.ID] = cctv;
            }

            return true;
        }

        public void AddCCTV(CCTV cctv)
        {
            m_dicCCTV[cctv.ID] = cctv;
        }

        public void DeleteCCTV(CCTV cctv)
        {
            m_dicCCTV.Remove(cctv.ID);
        }

        public EquipmentZone GetEquipmentZone(int nEquipZoneID)
        {
            EquipmentZone equipZone;

            if (m_dicEquipZones.TryGetValue(nEquipZoneID, out equipZone))
                return equipZone;

            return null;
        }

        public CCTV GetCCTV(int nCCTVID)
        {
            CCTV cctv;

            if (m_dicCCTV.TryGetValue(nCCTVID, out cctv))
                return cctv;

            return null;
        }
    }
}
