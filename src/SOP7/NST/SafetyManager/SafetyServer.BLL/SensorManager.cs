using System;
using System.Collections;
using System.Collections.Generic;
using SDMS.Model.Spatial;
using SDMS.Model.Sensor;
using dnsData.Sensor;
using dnsSopID;
using SDMS.IDAL;
using TeamEditor.Model.Sop.Team;
using AgentFactory.BLL;

namespace SafetyServer.BLL
{
    using Data.Response;
    using Data.Models;
    using SDMS.Model.CCTV;

    public class SensorManager
    {
        private MainManager m_mainManager = null;
        private Server.FireSensor m_fireSensorServer = null;
        /*private Server.PSMSensor m_psmSensorServer = null;
        private Server.SecuritySensor m_securitySensorServer = null;*/
        private Server.EtcSensor m_etcSensorServer = null;

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

        public SensorManager(MainManager mainManager, Factory factory)
        {
            m_mainManager = mainManager;
            m_fireSensorServer = new Server.FireSensor(mainManager, factory);
            /*m_psmSensorServer = new Server.PSMSensor(m_mainManager, factory);
            m_securitySensorServer = new Server.SecuritySensor(m_mainManager, factory);*/
            m_etcSensorServer = new Server.EtcSensor(m_mainManager, factory);
        }

        public void Initialize()
        {
            if (LoadBuildingData())
            {
                if (LoadZones())
                {
                    if (LoadEquipmentZones())
                    {
                        if (LoadSensorZones())
                        {
                            LoadSensorTagInfo();
                        }
                    }
                }
            }
        }

        public void OnLoad()
        {
            m_fireSensorServer.OnLoad(m_mainManager.SDMSDataManager);
            //m_psmSensorServer.OnLoad(m_mainManager.SDMSDataManager);
            //m_securitySensorServer.OnLoad(m_mainManager.SDMSDataManager);
            m_etcSensorServer.OnLoad(m_mainManager.SDMSDataManager);
        }

        public Result OnReceive(Facility.FacilityType sensorType, int header, string strClientInfo, ArrayList arrDatas)
        {
            if (Facility.IsFireSensorType(sensorType))
                return m_fireSensorServer.OnReceive(header, strClientInfo, arrDatas);
            /*else if (Facility.IsPSMSensorType(sensorType))
                return m_psmSensorServer.OnReceive(header, strClientInfo, arrDatas);
            else if (Facility.IsSecurityType(sensorType))
                return m_securitySensorServer.OnReceive(header, strClientInfo, arrDatas);*/
            else if (Facility.IsETCSensorType(sensorType))
                return m_etcSensorServer.OnReceive(header, strClientInfo, arrDatas);

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.UNKNOWN_COMMAND));
        }

        public MessageResult ProcessAreaAlarm(string strMemberID, string strCameraID, DateTime timeStamp, int nAlarmLevel, string strMessage)
        {
            int nSensorZoneID, nSensorTagID;
            string strErrorMessage = null;

            if (strMemberID == null || strMemberID.Length == 0)
                strErrorMessage = "userID is empty";

            if (strCameraID == null || strCameraID.Length == 0)
            {
                if (strErrorMessage != null)
                    strErrorMessage += ", cameraID is empty";
                else
                    strErrorMessage = "cameraID is empty";
            }    

            if (strErrorMessage != null)
                return new MessageResult(false, strErrorMessage);

            if (GetSensorZoneIDFromMemberID(strMemberID, strCameraID, "areaalarm", out nSensorZoneID, out nSensorTagID, out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add((int)dnsData.Sensor.Facility.FacilityType.ETC);
            arrDatas.Add(nSensorTagID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(strMemberID);
            arrDatas.Add(strCameraID);
            arrDatas.Add(timeStamp);
            arrDatas.Add(nAlarmLevel);
            arrDatas.Add(strMessage);
            arrDatas.Add(true);

            MessageResult result = (MessageResult)m_etcSensorServer.OnReceive(Header.SENSOR_DATA, "", arrDatas);

            if (result.Success)
            {
                // 알람 전달
                // 알람전달 성공에 대한 리턴을 보낸뒤 100ms 간격을 두고 알람을 다른 곳에 전달한다.
                Process.NetvisionManager mgr = new Process.NetvisionManager();
                mgr.SendAreaAlarmAsync(strCameraID, strMemberID, timeStamp, nAlarmLevel, strMessage);
            }

            return result;
        }

        public MessageResult ProcessNoEquipmentAlarm(string strMemberID, string strCameraID, DateTime timeStamp, bool helmet, bool shoes, bool belt, int nAlarmLevel, string strMessage)
        {
            int nSensorZoneID, nSensorTagID;
            string strErrorMessage = null;

            if (strMemberID == null || strMemberID.Length == 0)
                strErrorMessage = "userID is empty";

            if (strCameraID == null || strCameraID.Length == 0)
            {
                if (strErrorMessage != null)
                    strErrorMessage += ", cameraID is empty";
                else
                    strErrorMessage = "cameraID is empty";
            }

            if (strErrorMessage != null)
                return new MessageResult(false, strErrorMessage);

            if (GetSensorZoneIDFromMemberID(strMemberID, strCameraID, "noequipment", out nSensorZoneID, out nSensorTagID, out strErrorMessage) == false)
                return new MessageResult(false, strErrorMessage);

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add((int)dnsData.Sensor.Facility.FacilityType.ETC);
            arrDatas.Add(nSensorTagID);
            arrDatas.Add(nSensorZoneID);
            arrDatas.Add(strMemberID);
            arrDatas.Add(strCameraID);
            arrDatas.Add(timeStamp);
            arrDatas.Add(nAlarmLevel);
            arrDatas.Add(strMessage);
            arrDatas.Add(true);

            MessageResult result = (MessageResult)m_etcSensorServer.OnReceive(Header.SENSOR_DATA, "", arrDatas);

            if (result.Success)
            {
                // 알람 전달
                // 알람전달 성공에 대한 리턴을 보낸뒤 100ms 간격을 두고 알람을 다른 곳에 전달한다.
                Process.NetvisionManager mgr = new Process.NetvisionManager();
                mgr.SendNoEquipmentAlarmAsync(strCameraID, strMemberID, timeStamp, helmet, shoes, belt, nAlarmLevel, strMessage);
            }

            return result;
        }

        private bool GetSensorZoneIDFromMemberID(string strMemberID, string strCameraID, string strSensorTag, out int nSensorZoneID, out int nSensorTagID, out string strErrorMessage)
        {
            nSensorZoneID = nSensorTagID = -1;

            bool isNullable;
            string strCondition = string.Format("{0} = '{1}'",
                RegularMember.GetFieldName(RegularMember.Fields.MemberID, out isNullable),
                strMemberID);

            List<RegularMember> members = m_mainManager.TeamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);

            if (members == null)
                return false;

            if (members.Count == 0)
            {
                strErrorMessage = string.Format("{0}에 해당하는 사용자 정보를 찾을수 없습니다.", strMemberID);
                return false;
            }

            Dictionary<CCTV.Fields, object> dicConditions2 = new Dictionary<CCTV.Fields, object>();
            dicConditions2[CCTV.Fields.UniqueKey] = strCameraID;

            List<CCTV> cctvs = m_mainManager.SDMSDataManager.GetSelectManager().SelectCCTVs(dicConditions2, null, out strErrorMessage);
            if (cctvs == null)
                return false;

            if (cctvs.Count == 0)
            {
                strErrorMessage = string.Format("CCTV UniqueKey {0}에 연결된 CCTV 정보가 존재하지 않습니다.", strCameraID);
                return false;
            }

            Dictionary<ETC.Fields, object> dicConditions = new Dictionary<ETC.Fields, object>();
            dicConditions[ETC.Fields.Department] = members[0].ID.ToString();
            dicConditions[ETC.Fields.ZoneID] = cctvs[0].ZoneID.ToString();

            List<ETC> sensors = m_mainManager.SDMSDataManager.GetSelectManager().SelectETCSensors(dicConditions, null, out strErrorMessage);

            if (sensors == null)
                return false;

            ETC sensor = null;

            foreach (ETC _sensor in sensors)
            {
                if (_sensor.Name.ToLower().Contains(strSensorTag))
                {
                    sensor = _sensor;
                    break;
                }
            }

            if (sensors.Count == 0)
            {
                strErrorMessage = string.Format("사용자 {0}에 연결된 EtcSensor 정보가 존재하지 않습니다.", strMemberID);
                return false;
            }

            Dictionary<SensorZone.Fields, object> dicCondition2 = new Dictionary<SensorZone.Fields, object>();
            dicCondition2[SensorZone.Fields.SensorType] = (int)dnsData.Sensor.Facility.FacilityType.ETC;
            dicCondition2[SensorZone.Fields.OrgSensorID] = sensor.ID;

            List<SensorZone> sensorZones = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorZones(dicCondition2, null, out strErrorMessage);

            if (sensorZones == null)
                return false;

            if (sensorZones.Count == 0)
            {
                strErrorMessage = string.Format("사용자 {0}에 연결된 SensorZone 정보가 존재하지 않습니다.", strMemberID);
                return false;
            }

            nSensorZoneID = sensorZones[0].ID;

            Dictionary<TagInfo.Fields, object> dicCondition3 = new Dictionary<TagInfo.Fields, object>();
            dicCondition3[TagInfo.Fields.SensorZoneID] = nSensorZoneID;

            List<TagInfo> tagInfos = m_mainManager.SDMSDataManager.GetSelectManager().SelectSensorTagInfo(dicCondition3, null, out strErrorMessage);

            if (tagInfos == null)
                return false;

            if (tagInfos.Count == 0)
            {
                strErrorMessage = string.Format("사용자 {0}에 연결된 SensorTagInfo 정보가 존재하지 않습니다.", strMemberID);
                return false;
            }

            nSensorTagID = tagInfos[0].ID;
            return true;
        }

        public MessageResult ProcessManualReport(Facility.FacilityType sensorType, string strMemberID, int? nBuildingID, int? nZoneID, string strMessage)
        {
            DateTime dtNow = DateTime.Now;

            if (nBuildingID == null && nZoneID == null)
                return new MessageResult(false, "재난위치가 지정되지 않았습니다.");

            if (strMemberID == null)
                return new MessageResult(false, "신고자의 ID가 null입니다.");

            bool isNullable;

            string strCondition = string.Format("{0} = '{1}'",
                RegularMember.GetFieldName(RegularMember.Fields.MemberID, out isNullable),
                strMemberID);

            string strErrorMessage;
            List<RegularMember> members = m_mainManager.TeamDataManager.GetSelectManager().SelectRegularMembers(strCondition, out strErrorMessage);

            if (members == null)
                return new MessageResult(false, strErrorMessage);

            if (members.Count == 0)
                return new MessageResult(false, string.Format("{0}에 해당하는 사용자 정보를 찾을수 없습니다.", strMemberID));

            if (sensorType == Facility.FacilityType.FIRE_SENSOR)
                return ProcessFireManualReport(members[0], nBuildingID, nZoneID, strMessage, dtNow);

            return new MessageResult(false, "확인할 수 없는 재난타입니다.");
        }

        private MessageResult ProcessFireManualReport(RegularMember member, int? nBuildingID, int? nZoneID, string strMessage, DateTime timeStamp)
        {
            if (nZoneID == null)
            {
                Dictionary<Zone.Fields, object> dicConditions = new Dictionary<Zone.Fields, object>();
                dicConditions[Zone.Fields.BuildingID] = nBuildingID;

                string strErrorMessage;
                List<Zone> zones = m_mainManager.SDMSDataManager.GetSelectManager().SelectZones(dicConditions, null, out strErrorMessage);

                if (zones == null)
                    return new MessageResult(false, strErrorMessage);

                if (zones.Count == 0)
                    return new MessageResult(false, string.Format("BuildingID가 잘못 지정되었습니다.({0})", nBuildingID));

                nZoneID = zones[0].ID;
            }

            ArrayList arrDatas = new ArrayList();
            arrDatas.Add((int)Facility.FacilityType.FIRE_SENSOR);
            arrDatas.Add(Header.ManualReportDefaultID);
            arrDatas.Add((int)nZoneID);
            arrDatas.Add(timeStamp);
            // 3 => 경계
            arrDatas.Add(3);
            arrDatas.Add(member.ID.ToString());
            arrDatas.Add(strMessage == null ? "" : strMessage);

            return (MessageResult)m_fireSensorServer.OnReceive(Header.MANUAL_REPORT, "", arrDatas);
        }

        private bool LoadBuildingData()
        {
            if (m_mainManager == null || m_mainManager.SDMSDataManager == null)
                return false;

            string strErrorMessage;
            ISelect selectManager = m_mainManager.SDMSDataManager.GetSelectManager();

            List<BuildingGroup> buildingGroups = selectManager.SelectBuildingGroups(null, null, out strErrorMessage);

            if (buildingGroups == null)
                return false;

            m_dicBuildingGroups.Clear();
            string strBuildingGroupIDs = "";

            foreach (BuildingGroup buildingGroup in buildingGroups)
            {
                if (strBuildingGroupIDs.Length == 0)
                    strBuildingGroupIDs = buildingGroup.ID.ToString();
                else
                    strBuildingGroupIDs += ", " + buildingGroup.ID.ToString();

                m_dicBuildingGroups[buildingGroup.ID] = buildingGroup;
            }

            if (strBuildingGroupIDs.Length == 0)
                return true;

            bool isNullable;
            string strCondition = string.Format("{0} in ({1})", Building.GetFieldName(Building.Fields.BuildingGroupID, out isNullable), strBuildingGroupIDs);

            List<Building> buildings = selectManager.SelectBuildings(null, strCondition, out strErrorMessage);

            if (buildings == null)
                return false;

            m_dicBuildings.Clear();

            foreach (Building building in buildings)
            {
                m_dicBuildings[building.ID] = building;
            }

            return true;
        }

        private bool LoadZones()
        {
            if (m_mainManager == null || m_mainManager.SDMSDataManager == null)
                return false;

            string strErrorMessage;
            ISelect selectManager = m_mainManager.SDMSDataManager.GetSelectManager();

            List<Zone> zones = selectManager.SelectZones(null, null, out strErrorMessage);

            if (zones == null)
                return false;

            m_dicZones.Clear();
            m_dicBuildingZones.Clear();
            m_dicOutdoorZones.Clear();

            List<Zone> buildingZones;

            foreach (Zone zone in zones)
            {
                m_dicZones[zone.ID] = zone;

                if (zone.BuildingID == null)
                    m_dicOutdoorZones[zone.ID] = zone;
                else
                {
                    if (m_dicBuildingZones.TryGetValue((int)zone.BuildingID, out buildingZones) == false)
                    {
                        buildingZones = new List<Zone>();
                        m_dicBuildingZones[(int)zone.BuildingID] = buildingZones;
                    }

                    buildingZones.Add(zone);
                }
            }

            return true;
        }

        private bool LoadEquipmentZones()
        {
            if (m_mainManager == null || m_mainManager.SDMSDataManager == null)
                return false;

            string strErrorMessage;
            ISelect selectManager = m_mainManager.SDMSDataManager.GetSelectManager();

            List<EquipmentZone> equipZones = selectManager.SelectEquipmentZones(null, null, out strErrorMessage);

            if (equipZones == null)
                return false;

            m_dicEquipZones.Clear();
            m_dicZoneEquipZones.Clear();

            Zone zone;
            List<EquipmentZone> zoneEquipZones;

            foreach (EquipmentZone equipZone in equipZones)
            {
                m_dicEquipZones[equipZone.ID] = equipZone;

                foreach (int zoneID in equipZone.LinkedZoneIDs)
                {
                    if (m_dicZones.TryGetValue(zoneID, out zone) == false)
                        continue;

                    if (m_dicZoneEquipZones.TryGetValue(zone, out zoneEquipZones) == false)
                    {
                        zoneEquipZones = new List<EquipmentZone>();
                        m_dicZoneEquipZones[zone] = zoneEquipZones;
                    }

                    zoneEquipZones.Add(equipZone);
                }
            }

            return true;
        }

        private bool LoadSensorZones()
        {
            if (m_mainManager == null || m_mainManager.SDMSDataManager == null)
                return false;

            string strErrorMessage;
            ISelect selectManager = m_mainManager.SDMSDataManager.GetSelectManager();

            List<SensorZone> sensorZones = selectManager.SelectSensorZones(null, null, out strErrorMessage);

            if (sensorZones == null)
                return false;

            m_dicSensorZones.Clear();
            m_dicEquipZoneSensors.Clear();
            m_dicSensorZoneGroup.Clear();
            m_dicSensorZoneGroup2.Clear();

            EquipmentZone equipZone;
            List<SensorZone> equipZoneSensors;

            foreach (SensorZone sensorZone in sensorZones)
            {
                m_dicSensorZones[sensorZone.ID] = sensorZone;

                if (m_dicEquipZones.TryGetValue(sensorZone.EquipZoneID, out equipZone) == false)
                    continue;

                if (m_dicEquipZoneSensors.TryGetValue(equipZone, out equipZoneSensors) == false)
                {
                    equipZoneSensors = new List<SensorZone>();
                    m_dicEquipZoneSensors[equipZone] = equipZoneSensors;
                }

                equipZoneSensors.Add(sensorZone);

                SensorZoneGroup group = GetSensorZoneGroup(sensorZone.EquipZoneID, (Facility.FacilityType)sensorZone.SensorType);

                if (group != null)
                    m_dicSensorZoneGroup2[sensorZone.ID] = group;
                else
                    System.Diagnostics.Trace.WriteLine("Unknown SensorZone : " + sensorZone.ID.ToString());
            }

            return true;
        }

        private bool LoadSensorTagInfo()
        {
            if (m_mainManager == null || m_mainManager.SDMSDataManager == null)
                return false;

            string strErrorMessage;
            ISelect selectManager = m_mainManager.SDMSDataManager.GetSelectManager();

            List<TagInfo> sensors = selectManager.SelectSensorTagInfo(null, null, out strErrorMessage);

            if (sensors == null)
                return false;

            m_dicSensorTagActivation.Clear();

            foreach (TagInfo sensor in sensors)
            {
                m_dicSensorTagActivation[sensor.ID] = sensor.IsActivate;
            }

            return true;
        }

        public SensorZoneGroup GetSensorZoneGroup(EquipmentZone equipZone, Facility.FacilityType sensorType)
        {
            long nID = SensorZoneGroup.ToID(equipZone, sensorType);
            return GetSensorZoneGroup(nID, -1, equipZone, sensorType);
        }

        public SensorZoneGroup GetSensorZoneGroup(int nEquipZoneID, Facility.FacilityType sensorType)
        {
            long nID = SensorZoneGroup.ToID(nEquipZoneID, sensorType);
            return GetSensorZoneGroup(nID, nEquipZoneID, null, sensorType);
        }

        private SensorZoneGroup GetSensorZoneGroup(long nSensorZoneGroupID, int nEquipZoneID, EquipmentZone equipZone, Facility.FacilityType sensorType)
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

        // nSensorZoneID가 포함된 SensorZoneGroup을 리턴한다.
        public SensorZoneGroup GetSensorZoneGroup(int nSensorZoneID)
        {
            SensorZoneGroup group = null;
            m_dicSensorZoneGroup2.TryGetValue(nSensorZoneID, out group);
            return group;
        }

        public void AddSensorZone(SensorZone sensorZone)
        {
            m_dicSensorZones[sensorZone.ID] = sensorZone;

            SensorZoneGroup group = GetSensorZoneGroup(sensorZone.EquipZoneID, (Facility.FacilityType)sensorZone.SensorType);

            if (group != null)
                m_dicSensorZoneGroup2[sensorZone.ID] = group;
        }

        public SensorZone GetSensorZone(int nSensorID)
        {
            SensorZone sensorZone;

            if (m_dicSensorZones.TryGetValue(nSensorID, out sensorZone))
                return sensorZone;

            return null;
        }

        public SensorZone GetSensorZone(int nOrgSensorID, Facility.FacilityType sensorType, EquipmentZone equipZone, out SensorZoneGroup group)
        {
            group = null;

            if (equipZone != null)
            {
                group = GetSensorZoneGroup(equipZone, sensorType);

                if (group != null)
                {
                    foreach (KeyValuePair<SensorZone, int> pair in group.GetSensors())
                    {
                        SensorZone sensorZone = pair.Key;

                        if (sensorZone.OrgSensorID == nOrgSensorID)
                            return sensorZone;
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, SensorZone> pair in m_dicSensorZones)
                {
                    if (pair.Value.SensorType == (int)sensorType && pair.Value.OrgSensorID == nOrgSensorID)
                    {
                        return pair.Value;
                    }
                }
            }

            return null;
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

        public PSM GetPSMSensor(int nSensorID, out string strErrorMessage)
        {
            return m_mainManager.SDMSDataManager.GetSelectManager().SelectPSMSensor(nSensorID, out strErrorMessage);
        }
    }
}
