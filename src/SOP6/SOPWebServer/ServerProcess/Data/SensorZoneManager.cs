using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Spatial;
using DBUtility2;
using System.Collections;
using UnE.Sensor;

namespace ServerProcess.Data
{
    public class SensorZoneManager : libSOPPolicy.Common.ISensorZoneManager
    {
        // Key : BuildingGroup ID
        private Dictionary<int, BuildingGroup> m_dicBuildingGroups = new Dictionary<int, BuildingGroup>();
        // Key : Building ID
        private Dictionary<int, Building> m_dicBuildings = new Dictionary<int, Building>();
        // 전체 Zone
        // Key : Zone ID
        private Dictionary<int, Zone> m_dicZones = new Dictionary<int, Zone>();
        // 건물내에 있는 Zone
        // Key : Building ID
        private Dictionary<int, List<Zone>> m_dicBuildingZones = new Dictionary<int, List<Zone>>();
        // 건물외부에 있는 Zone
        // Key : Zone ID
        private Dictionary<int, Zone> m_dicOutdoorZones = new Dictionary<int, Zone>();

        // Key : EquipmentZone ID
        private Dictionary<int, EquipmentZone> m_dicEquipZones = new Dictionary<int, EquipmentZone>();
        // Zone에 속해있는 EquipmentZone List
        private Dictionary<Zone, List<EquipmentZone>> m_dicZoneEquipZones = new Dictionary<Zone, List<EquipmentZone>>();

        // Key : SensorZone ID
        private Dictionary<int, SensorZone> m_dicSensorZones = new Dictionary<int, SensorZone>();
        //EquipmentZone에 속해있는 SensorZone List(EquipmentZone, SensorZone List)
        private Dictionary<EquipmentZone, List<SensorZone>> m_dicEquipZoneSensors = new Dictionary<EquipmentZone, List<SensorZone>>();
        // 같은 설비영역을 공유하며, Type이 같은 Sensor들을 하나의 그룹으로 묶어 관리한다.
        // Key : SensorZoneGroup의 ID인데 EquipZone ID와 SensorType의 조합이다.
        //       상위 4바이트 : EquipZone ID
        //       하위 4바이트 : SensorType(Facility.FacilityType)
        private Dictionary<long, SensorZoneGroup> m_dicSensorZoneGroup = new Dictionary<long, SensorZoneGroup>();
        // Key : SensorZone ID
        private Dictionary<int, SensorZoneGroup> m_dicSensorZoneGroup2 = new Dictionary<int, SensorZoneGroup>();

        // Key : SensorTagInfo ID
        // Value : 해당 센서가 활성화 상태인가?(false이면 이 센서의 신호는 무시함)
        private Dictionary<int, bool> m_dicSensorTagActivation = new Dictionary<int, bool>();

        private static SensorZoneManager m_instance = new SensorZoneManager();

        public static SensorZoneManager Instance
        {
            get { return m_instance; }
        }

        public void Initialize(DirectDBManager dbMgr)
        {
            LoadBuildingData(dbMgr);
            LoadZones(dbMgr);
            LoadEquipmentZones(dbMgr);

            LoadSensorZone(dbMgr);
            LoadSensorTagInfo(dbMgr);
        }

        private void LoadSensorTagInfo(DirectDBManager dbMgr)
        {
            string strSQL = "Select ID, DeActivate from SensorTagInfo";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int tagID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string deActivationCode = WebDBManager.GetStringField(arrResult[i + 1]);

                if (tagID > 0 && deActivationCode != null)
                {
                    m_dicSensorTagActivation[tagID] = deActivationCode == "N" || deActivationCode == "n";
                }
            }
        }

        private void LoadSensorZone(DirectDBManager dbMgr)
        {
            //string strSQL = "select ID,Type, Connected, EquipZoneID, Data, OrgSensorID from SensorZone";
            // EquipmentZone에 추가된 SiteID를 이용하여 Site별 데이터를 구분하도록 수정함. skkim 2015.01.14
            string szText = "SELECT sz.ID,sz.Type,sz.Connected, sz.EquipZoneID, sz.Data, sz.OrgSensorID, sz.Zone " +
                            " FROM SensorZone as sz, EquipmentZone as ez WHERE sz.EquipZoneID = ez.ID and ez.SiteID = {0}";
            string strSQL = string.Format(szText, dbMgr.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nType = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nConnected = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nEquipZoneID = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nData = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nLinkedSensorID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nZoneID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                EquipmentZone equipZone = null;
                m_dicEquipZones.TryGetValue(nEquipZoneID, out equipZone);

                SensorZone sensorZone = new SensorZone();

                sensorZone.ID = nID;
                sensorZone.Type = IFacility.ToFacilityType(nType);
                sensorZone.IsConnected = nConnected == 1;
                sensorZone.SensorData = nData;
                sensorZone.LinkedSensorID = nLinkedSensorID;
                sensorZone.EquipZone = equipZone;
                sensorZone.ZoneID = nZoneID;

                m_dicSensorZones[nID] = sensorZone;

                if (equipZone != null)
                {
                    List<SensorZone> sensorZones;

                    if (m_dicEquipZoneSensors.TryGetValue(equipZone, out sensorZones))
                    {
                        sensorZones.Add(sensorZone);
                    }
                    else
                    {
                        sensorZones = new List<SensorZone>();
                        sensorZones.Add(sensorZone);

                        m_dicEquipZoneSensors[equipZone] = sensorZones;
                    }
                }

                SensorZoneGroup group = GetSensorZoneGroup(nEquipZoneID, sensorZone.Type);

                if (group != null)
                    m_dicSensorZoneGroup2[sensorZone.ID] = group;
                else
                    System.Diagnostics.Trace.WriteLine("Unknown SensorZone : " + sensorZone.ID.ToString());

                //group.SensorDatas[sensorZone] = null;*/
            }
        }

        public SensorZone GetSensorZone(int nSensorID)
        {
            SensorZone sensorZone;

            if (m_dicSensorZones.TryGetValue(nSensorID, out sensorZone))
                return sensorZone;

            return null;
        }

        public SensorZone GetSensorZone(int nOrgSensorID, IFacility.FacilityType sensorType, EquipmentZone equipZone, out SensorZoneGroup group)
        {
            group = null;

            if (equipZone != null)
            {
                group = GetSensorZoneGroup(equipZone, sensorType);

                if (group != null)
                {
                    foreach (KeyValuePair<SensorZone, int> pair in group.GetSensorDatas())
                    {
                        if (pair.Key.LinkedSensorID == nOrgSensorID)
                            return pair.Key;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, SensorZone> pair in m_dicSensorZones)
                {
                    if (pair.Value.Type == sensorType && pair.Value.LinkedSensorID == nOrgSensorID)
                    {
                        return pair.Value;
                    }
                }
            }

            return null;
        }

        // nSensorZoneID가 포함된 SensorZoneGroup을 리턴한다.
        public SensorZoneGroup GetSensorZoneGroup(int nSensorZoneID)
        {
            SensorZoneGroup group = null;
            m_dicSensorZoneGroup2.TryGetValue(nSensorZoneID, out group);
            return group;
            /*foreach (KeyValuePair<long, SensorZoneGroup> pair in m_dicSensorZoneGroup)
            {
                SensorZoneGroup group = pair.Value;

                foreach (KeyValuePair<SensorZone, object> sensorData in group.SensorDatas)
                {
                    if (sensorData.Key.ID == nSensorZoneID)
                        return group;
                }
            }

            return null;*/
        }

        public SensorZoneGroup GetSensorZoneGroup(EquipmentZone equipZone, IFacility.FacilityType sensorType)
        {
            long nID = SensorZoneGroup.ToID(equipZone, sensorType);
            return GetSensorZoneGroup(nID, -1, equipZone, sensorType);
        }

        public SensorZoneGroup GetSensorZoneGroup(int nEquipZoneID, IFacility.FacilityType sensorType)
        {
            long nID = SensorZoneGroup.ToID(nEquipZoneID, sensorType);
            return GetSensorZoneGroup(nID, nEquipZoneID, null, sensorType);
        }

        private SensorZoneGroup GetSensorZoneGroup(long nSensorZoneGroupID, int nEquipZoneID, EquipmentZone equipZone, IFacility.FacilityType sensorType)
        {
            SensorZoneGroup group = null;

            if (m_dicSensorZoneGroup.TryGetValue(nSensorZoneGroupID, out group))
                return group;

            if (equipZone == null && nEquipZoneID >= 0)
                m_dicEquipZones.TryGetValue(nEquipZoneID, out equipZone);

            group = new SensorZoneGroup();
            group.EquipmentZone = equipZone;
            group.SensorType = sensorType;

            m_dicSensorZoneGroup[group.ID] = group;
            return group;
        }

        private void LoadEquipmentZones(DirectDBManager dbMgr)
        {
            // update by mwkim 2016-05-11 : DisplayText 컬럼도 로드하도록 쿼리 수정
            string szText = "SELECT ID, ZoneName, LinkedZoneIDList, Type, BroadcastName, DisplayText FROM EquipmentZone where ID > 0 AND SiteID = {0}";

            string strSQL = string.Format(szText, dbMgr.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 5; i += 6)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strEquipZoneName = WebDBManager.GetStringField(arrResult[i + 1]);
                string strLinkedZoneIDList = WebDBManager.GetStringField(arrResult[i + 2]);
                int nType = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 4]);
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 5]);

                if (nID < 0)
                    continue;

                List<Zone> arrLinkedZones = GetZoneObjectList(strLinkedZoneIDList);
                if (arrLinkedZones == null)
                    continue;

                EquipmentZone equipZone = new EquipmentZone();

                equipZone.ID = nID;
                equipZone.ZoneName = strEquipZoneName;
                equipZone.ZoneType = (EquipmentZone.EquipZoneType)nType;
                equipZone.BroadcastName = strBroadcastName;
                equipZone.DisplayText = strDisplayText;

                foreach (Zone zone in arrLinkedZones)
                {
                    if (zone.Building != null)
                        equipZone.Building = zone.Building;
                    
                    equipZone.LinkedZoneList.Add(zone);
                    List<EquipmentZone> arrEquipZones = null;

                    if (m_dicZoneEquipZones.TryGetValue(zone, out arrEquipZones) == false)
                    {
                        arrEquipZones = new List<EquipmentZone>();
                        m_dicZoneEquipZones[zone] = arrEquipZones;
                    }

                    if (!arrEquipZones.Contains(equipZone))
                        arrEquipZones.Add(equipZone);
                }

                m_dicEquipZones[nID] = equipZone;
            }
        }

        private List<Zone> GetZoneObjectList(string strZoneIDList)
        {
            int nZoneID;
            Zone zone;
            List<Zone> zones = new List<Zone>();

            string[] strZoneIDs = strZoneIDList.Split(',');

            foreach (string strZoneID in strZoneIDs)
            {
                if (int.TryParse(strZoneID, out nZoneID))
                {
                    if (m_dicZones.TryGetValue(nZoneID, out zone))
                        zones.Add(zone);
                }
            }

            return zones;
        }

        private void LoadZones(DirectDBManager dbMgr)
        {
            // update by mwkim 2016-05-11 : DisplayText 컬럼도 조회하도록 쿼리 수정
            string szText = "select id, ZoneName, BuildingID, FloorIndex, Boundary, DXFFileName, DXFAccessedTime, _3DFileName, _3DAccessedTime, BroadcastName, AddFloor, DisplayText " +
                            " from Zone where SiteID = {0}";

            string strSQL = string.Format(szText, dbMgr.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return;

            DateTime dtDefault = new DateTime();
            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 11; i += 12)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strZoneName = WebDBManager.GetStringField(arrResult[i + 1]);
                int nBuildingID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nFloorIndex = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string strBoundary = WebDBManager.GetStringField(arrResult[i + 4]);
                string strDXFFileName = WebDBManager.GetStringField(arrResult[i + 5]);
                DateTime dtDXF = WebDBManager.GetDateTimeField(arrResult[i + 6], dtDefault);
                string str3DFileName = WebDBManager.GetStringField(arrResult[i + 7]);
                DateTime dt3D = WebDBManager.GetDateTimeField(arrResult[i + 8], dtDefault);
                string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 9]);
                string strAddFloor = WebDBManager.GetStringField(arrResult[i + 10], "0.0");
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 11]);

                Zone zone = new Zone();

                zone.ID = nID;
                zone.ZoneName = strZoneName;
                zone.FloorIndex = nFloorIndex;

                if (strBroadcastName == null || strBroadcastName == "")
                    zone.BroadcastName = strZoneName;
                else
                    zone.BroadcastName = strBroadcastName;

                if (strDisplayText == null || strDisplayText == "")
                    zone.DisplayText = strDisplayText;
                else
                    zone.DisplayText = strDisplayText;

                if (m_dicBuildings.ContainsKey(nBuildingID))
                {
                    zone.Building = m_dicBuildings[nBuildingID];
                    zone.Building.FloorList.Add(zone);
                }

                //지하나 .2.5인 층들 
                try
                {
                    //strAddFloor가 비었다면 0.0f
                    if (strAddFloor == null || strAddFloor.Length == 0)
                        zone.AddFloor = 0.0f;
                    else
                        zone.AddFloor = float.Parse(strAddFloor);
                }
                catch (Exception)
                {
                    zone.AddFloor = 0.0f;
                }

                zone.Floor.FloorIndex = (zone.FloorIndex + zone.AddFloor);

                m_dicZones[nID] = zone;

                if (nBuildingID < 0)
                    m_dicOutdoorZones[nID] = zone;

                if (zone.Building != null)
                {
                    List<Zone> buildingZones = null;

                    if (m_dicBuildingZones.TryGetValue(zone.Building.ID, out buildingZones))
                    {
                        buildingZones.Add(zone);
                    }
                    else
                    {
                        buildingZones = new List<Zone>();
                        m_dicBuildingZones[zone.Building.ID] = buildingZones;
                        buildingZones.Add(zone);
                    }
                }
            }
        }

        private void LoadBuildingData(DirectDBManager dbMgr)
        {
            string szText = "SELECT bd.id, bd.BuildingID,  bd.BuildingCode, bd.BuildingName, bd.BuildingGroupID, bd.MaxFloor,bd. MinFloor," +
                            " bdg.GroupName, bdg.TextCenter, bd.BroadCastingText FROM Building as bd, BuildingGroup as bdg " +
                            " WHERE bd.BuildingGroupID = bdg.ID and bdg.SiteID = {0}";

            string strSQL = string.Format(szText, dbMgr.SiteID);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 9; i += 10)
            {
                try
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strBuildingID = WebDBManager.GetStringField(arrResult[i + 1]);
                    string strBuildingCode = WebDBManager.GetStringField(arrResult[i + 2]);
                    string strBuildingName = WebDBManager.GetStringField(arrResult[i + 3]);
                    int nBuildingGroupID = WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                    int nMaxFloorID = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    int nMinFloorID = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                    string strBuildingGroupName = WebDBManager.GetStringField(arrResult[i + 7]);
                    string strGroupNamePos = WebDBManager.GetStringField(arrResult[i + 8], "");
                    string strBroadcastName = WebDBManager.GetStringField(arrResult[i + 9]);

                    if (strBroadcastName == null)
                    {
                        strBroadcastName = strBuildingName;
                    }
                    else
                    {
                        int nIdx = strBroadcastName.IndexOf('*');
                        if (nIdx != -1)
                        {
                            strBroadcastName = strBroadcastName.Substring(0, nIdx);
                        }
                    }

                    Building building = new Building();
                    BuildingGroup group;

                    if (m_dicBuildingGroups.TryGetValue(nBuildingGroupID, out group))
                    {
                        building.BuildingGroup = group;
                    }
                    else
                    {
                        group = new BuildingGroup();
                        group.BuildingGroupName = strBuildingGroupName;
                        group.GroupID = nBuildingGroupID;
                        
                        if (strGroupNamePos != "")
                        {
                            string[] xy = strGroupNamePos.Split(',');
                            float x, y;
                            float.TryParse(xy[0], out x);
                            float.TryParse(xy[1], out y);
                            group.TextCenterX = x;
                            group.TextCenterY = y;
                        }

                        m_dicBuildingGroups[nBuildingGroupID] = group;
                        building.BuildingGroup = group;
                    }

                    building.ID = nID;
                    building.BuildingName = strBuildingName;
                    building.MaxFloorIndex = nMaxFloorID;
                    building.MinFloorIndex = nMinFloorID;
                    building.BuildingCode = strBuildingCode;
                    building.BuildingID = strBuildingID;
                    building.BroadcastName = strBroadcastName;
                    building.BuildingGroup.BuildingList.Add(building);

                    m_dicBuildings[nID] = building;

                }
                catch (System.Exception)
                {
                    //MessageBox.Show(ex.StackTrace);
                }
            }
        }

        public bool IsActiveSensor(int nSensorTagInfoID)
        {
            bool isActive;

            if (m_dicSensorTagActivation.TryGetValue(nSensorTagInfoID, out isActive))
                return isActive;

            return true;
        }

        public void SetSensorActivation(int nSensorTagInfoID, bool isActive)
        {
            m_dicSensorTagActivation[nSensorTagInfoID] = isActive;
        }

        public Zone GetZone(int nZoneID)
        {
            Zone zone = null;
            m_dicZones.TryGetValue(nZoneID, out zone);
            return zone;
        }

        public EquipmentZone GetEquipmentZone(int nEquipZoneID)
        {
            EquipmentZone zone = null;
            m_dicEquipZones.TryGetValue(nEquipZoneID, out zone);
            return zone;
        }

        public List<SensorZone> GetEquipZoneSensorZones(EquipmentZone equipZone)
        {
            List<SensorZone> sensorZones;

            if (m_dicEquipZoneSensors.TryGetValue(equipZone, out sensorZones))
                return sensorZones;

            return null;
        }

        public void SetEquipZoneSensorZones(EquipmentZone equipZone, List<SensorZone> sensorZones)
        {
            m_dicEquipZoneSensors[equipZone] = sensorZones;
        }

        public BuildingGroup GetBuildingGroup(int nBuildingGroupID)
        {
            BuildingGroup group;

            if (m_dicBuildingGroups.TryGetValue(nBuildingGroupID, out group) == false)
                group = null;

            return group;
        }

        public Building GetBuilding(int nBuildingID)
        {
            Building building;

            if (m_dicBuildings.TryGetValue(nBuildingID, out building) == false)
                building = null;

            return building;
        }
    }
}
