using System.Collections;
using System.Collections.Generic;
using SDMS.Model.Spatial;
using SDMS.Model.Sensor;
using dnsData.Sensor;
using dnsSopID;
using SDMS.IDAL;
using AgentFactory.BLL;

namespace SOPWebServer.BLL
{
    using Response;
    using Models;

    public class SensorManager
    {
        private MainManager m_mainManager = null;
        private Server.FireSensor m_fireSensorServer = null;
        private Server.PSMSensor m_psmSensorServer = null;
        private Server.SecuritySensor m_securitySensorServer = null;
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

        private Dictionary<int, Material> m_dicMaterials = new Dictionary<int, Material>();

        // Key : Sensor ID
        // Value : Sensor
        private Dictionary<int, object> m_dicPSMSensors = new Dictionary<int, object>();


        public SensorManager(MainManager mainManager, Factory factory)
        {
            m_mainManager = mainManager;
            m_fireSensorServer = new Server.FireSensor(mainManager, factory);
            m_psmSensorServer = new Server.PSMSensor(m_mainManager, factory);
            m_securitySensorServer = new Server.SecuritySensor(m_mainManager, factory);
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

            LoadMaterial();
        }

        public void OnLoad()
        {
            m_fireSensorServer.OnLoad(m_mainManager.SDMSDataManager);
            m_psmSensorServer.OnLoad(m_mainManager.SDMSDataManager);
            m_securitySensorServer.OnLoad(m_mainManager.SDMSDataManager);
            m_etcSensorServer.OnLoad(m_mainManager.SDMSDataManager);
        }

        public Result OnReceive(Facility.FacilityType sensorType, int header, string strClientInfo, ArrayList arrDatas)
        {
            if (Facility.IsFireSensorType(sensorType))
                return m_fireSensorServer.OnReceive(header, strClientInfo, arrDatas);
            else if (Facility.IsPSMSensorType(sensorType))
                return m_psmSensorServer.OnReceive(header, strClientInfo, arrDatas);
            else if (Facility.IsSecurityType(sensorType))
                return m_securitySensorServer.OnReceive(header, strClientInfo, arrDatas);
            else if (Facility.IsETCSensorType(sensorType))
                return m_etcSensorServer.OnReceive(header, strClientInfo, arrDatas);

            return new MessageResult(false, ErrorMessageType.ToMessage(ErrorMessageType.UNKNOWN_COMMAND));
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

        private bool LoadPSMSensors(out string strErrorMessage)
        {
            List<PSM> psmSensors = m_mainManager.SDMSDataManager.GetSelectManager().SelectPSMSensors(null, null, out strErrorMessage);

            if (psmSensors == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadPSMSensors Fail : " + strErrorMessage);
                return false;
            }

            foreach (PSM sensor in psmSensors)
            {
                m_dicPSMSensors[sensor.ID] = sensor;
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
            m_dicPSMSensors.Clear();

            if (LoadPSMSensors(out strErrorMessage) == false)
                return false;

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
                Facility.FacilityType sensorType = Facility.FacilityType.NONE;

                if (GetSensorZoneSensorType(sensorZone, out sensorType) == false)
                    continue;

                SensorZoneGroup group = GetSensorZoneGroup(sensorZone.EquipZoneID, sensorType);

                if (group != null)
                    m_dicSensorZoneGroup2[sensorZone.ID] = group;
                else
                    System.Diagnostics.Trace.WriteLine("Unknown SensorZone : " + sensorZone.ID.ToString());
            }

            return true;
        }

        private bool GetSensorZoneSensorType(SensorZone sensorZone, out Facility.FacilityType sensorType)
        {
            sensorType = Facility.FacilityType.NONE;

            // 누출센서의 경우 SensorType 대신 MaterialType을 이용한다.
            if (sensorZone.SensorType == (int)Facility.FacilityType.PSM_SENSOR)
            {
                if (sensorZone.OrgSensorID == null)
                    return false;

                object psmSensor;

                if (m_dicPSMSensors.TryGetValue((int)sensorZone.OrgSensorID, out psmSensor) == false)
                    return false;
                else
                    sensorType = (Facility.FacilityType)((PSM)psmSensor).MaterialType;
            }
            else
                sensorType = (Facility.FacilityType)sensorZone.SensorType;

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

        private bool LoadMaterial()
        {
            string strErrorMessage;
            m_dicMaterials.Clear();

            Dictionary<Material.Fields, object> dicConditions = new Dictionary<Material.Fields, object>();
            string strAdditionalConditions = "";
            List<Material> materials = m_mainManager.SDMSDataManager.GetSelectManager().SelectMaterials(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (materials == null)
            {
                System.Diagnostics.Trace.WriteLine("LoadMaterial Error : " + strErrorMessage);
                return false;
            }
            else if (materials.Count == 0)
                return false;

            foreach (Material material in materials)
            {
                m_dicMaterials[material.ID] = material;
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

            Facility.FacilityType sensorType;

            if (GetSensorZoneSensorType(sensorZone, out sensorType) == false)
                return;

            SensorZoneGroup group = GetSensorZoneGroup(sensorZone.EquipZoneID, sensorType);

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

        public Material GetMaterial(int? nMaterial)
        {
            Material material = null;

            if (nMaterial != null && m_dicMaterials.ContainsKey((int)nMaterial))
            {
                material = m_dicMaterials[(int)nMaterial];
            }

            return material;
        }

        /// <summary>
        /// 알람 신호 수신 여부
        /// </summary>
        /// <returns></returns>
        public bool GetUseReceive(int nSensorType)
        {
            string strPropertyName = "UseReceive";
            if (Facility.IsFireSensorType(Facility.ToFacilityType(nSensorType)))
                strPropertyName += "Fire";
            else if (Facility.IsPSMSensorType(Facility.ToFacilityType(nSensorType)))
                strPropertyName += "PSM";
            else if (Facility.IsETCSensorType(Facility.ToFacilityType(nSensorType)))
                strPropertyName += "ETC";
            else if (Facility.IsSecurityType(Facility.ToFacilityType(nSensorType)))
                strPropertyName += "SVMS";

            string strErrorMessage = null;
            List<Common.Model.Option.Options> options = m_mainManager.CommonDataManager.GetSelectManager().SelectOption(Common.Model.Option.Options.OptionTarget.SDMS, strPropertyName, out strErrorMessage);
            if (options != null && options.Count > 0)
            {
                bool result;
                if (bool.TryParse(options[0].PropertyValue, out result))
                {
                    if (Facility.IsSecurityType(Facility.ToFacilityType(nSensorType)))
                        Logger.Instance.Write("GetUseReceive 수신여부 체크(SensorType: " + nSensorType.ToString() + ", PropertyValue: " + options[0].PropertyValue.ToString() + ")");

                    if (!result)
                        return false;
                }
            }
            else
            {
                if (Facility.IsSecurityType(Facility.ToFacilityType(nSensorType)))
                    Logger.Instance.Write("GetUseReceive 수신여부 체크(SensorType: " + nSensorType.ToString() + ", strErrorMessage: " + strErrorMessage + ", Return: true)");
            }
                

            return true;
        }
    }
}
