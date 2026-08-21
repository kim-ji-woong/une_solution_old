using dnsDBUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dnsData.Sensor;
using System.Threading;
using SDMS.DAL;
using SDMS.Model.Spatial;
using SDMS.Model.Sensor;
using SDMS.Model.Alarm;
using SDMS.Model.History;
using System.Configuration;

namespace SoulbrainSensorTester.Data
{
    public class DataManager
    {
        //private WebDBManager m_dbMgr = null;
        private SDMS.DAL.DataManager m_dataMgr = null;

        // 빌딩그룹
        Dictionary<int, BuildingGroup> m_dicBuildingGroup = new Dictionary<int, BuildingGroup>();
        // 빌딩
        Dictionary<int, Building> m_dicBuilding = new Dictionary<int, Building>();
        // Zone
        Dictionary<int, Zone> m_dicZone = new Dictionary<int, Zone>();
        // EquipmentZone
        Dictionary<int, EquipmentZone> m_dicEquipmentZone = new Dictionary<int, EquipmentZone>();
        
        // 화재 센서
        Dictionary<int, FireSensorData> m_dicFireSensorData = new Dictionary<int, FireSensorData>();
        // ETC 센서
        Dictionary<int, ETCSensorData> m_dicETCSensorData = new Dictionary<int, ETCSensorData>();
        // PSM 센서 
        Dictionary<int, PSMSensorData> m_dicPSMSensorData = new Dictionary<int, PSMSensorData>();

        // 센서 종류
        Dictionary<int, Material> m_dicMaterial = new Dictionary<int, Material>();

        private Thread m_Thread = null;
        private bool m_shutdownThread = false;

        // URL
        private string m_strAlarm_Fire_URL = "";
        public string StrAlarm_Fire_RUL
        {
            get { return m_strAlarm_Fire_URL; }
            set { m_strAlarm_Fire_URL = value; }
        }
        private string m_strAlarm_ETC_URL = "";
        private string m_strAlarm_PSM_URL = "";

        public DataManager(SDMS.DAL.DataManager dataMgr)
        {
            //m_dbMgr = dbManager;
            m_dataMgr = dataMgr;

            InitAlarmURL();

            // 빌딩그룹 불러오기
            LoadBuildingGroup(m_dicBuildingGroup);
            // 빌딩 불러오기
            LoadBuilding(m_dicBuilding);
            // 존 불러오기
            LoadZone(m_dicZone);
            // 이킵존 불러오기
            LoadEquipmentZone(m_dicEquipmentZone);

            // 센서 종류 불러오기
            LoadMaterial(m_dicMaterial);

            LoadFireSensor(m_dicFireSensorData);
            LoadETCSensor(m_dicETCSensorData);
            LoadPSMSensor(m_dicPSMSensorData);

            m_Thread = new Thread(new ThreadStart(reloadThread));
            m_Thread.Name = "GridReloadThread";
        }

        private void InitAlarmURL()
        {
            string strAlarm_Fire_URL = ConfigurationManager.AppSettings.Get("Alarm_Fire_URL");
            if (strAlarm_Fire_URL == null || strAlarm_Fire_URL.Length == 0)
                strAlarm_Fire_URL = "http://192.168.254.35:44379/api/FireSensor";

            string strAlarm_ETC_URL = ConfigurationManager.AppSettings.Get("Alarm_ETC_URL");
            if (strAlarm_ETC_URL == null || strAlarm_ETC_URL.Length == 0)
                strAlarm_ETC_URL = "http://192.168.254.35:44379/api/EtcSensor";

            string strAlarm_PSM_URL = ConfigurationManager.AppSettings.Get("Alarm_PSM_URL");
            if (strAlarm_PSM_URL == null || strAlarm_PSM_URL.Length == 0)
                strAlarm_PSM_URL = "http://192.168.254.35:44379/api/PSMSensor";

            m_strAlarm_Fire_URL = strAlarm_Fire_URL;
            m_strAlarm_ETC_URL = strAlarm_ETC_URL;
            m_strAlarm_PSM_URL = strAlarm_PSM_URL;
        }

        public void StartThread()
        {
            m_Thread.Start();
        }

        public void Shutdown()
        {
            m_shutdownThread = true;
            m_Thread.Abort();
        }

        private void reloadThread()
        {
            while (!m_shutdownThread)
            {
                FormMain.Instance.reloadGrid();

                Thread.Sleep(1 * 1000);
            }
        }
        
        public TreeNode MakeSensorTree(Facility.FacilityType facility)
        {
            TreeNode facilityTreeNode = null;

            if (facility == Facility.FacilityType.FIRE_SENSOR)
            {
                facilityTreeNode = new TreeNode("화재");

                // 화재 센서가 없다면 트리 생성을 하지 않는다.
                if (m_dicFireSensorData.Count() == 0)
                    return null;
            } else if (facility == Facility.FacilityType.PSM_SENSOR)
            {
                facilityTreeNode = new TreeNode("PSM");

                // PSM 센서가 없다면 트리 생성을 하지 않는다.
                if (m_dicPSMSensorData.Count() == 0)
                    return null;
            }
            else if (facility == Facility.FacilityType.ETC)
            {
                facilityTreeNode = new TreeNode("기타");

                // ETC 센서가 없다면 트리 생성을 하지 않는다.
                if (m_dicETCSensorData.Count() == 0)
                    return null;
            }

            foreach (KeyValuePair<int, BuildingGroup> pair in m_dicBuildingGroup)
            {
                BuildingGroup buildingGroup = pair.Value;
                TreeNode node = new TreeNode(buildingGroup.GroupName);
                node.Tag = buildingGroup;

                // 빌딩그룹에 자식 노드 만들기
                InitBuildingTreeNode(node, facility);

                facilityTreeNode.Nodes.Add(node);
            }

            if (m_dicBuildingGroup != null && m_dicBuildingGroup.Count > 0)
            {
                // 외곽 지역
                TreeNode node = new TreeNode("외곽");
                Zone zone = new Zone();
                zone.ID = 20000;

                node.Tag = zone;

                InitZoneSensorTreeNode(node, facility);

                facilityTreeNode.Nodes.Add(node);
            }


            return facilityTreeNode;
        }

        public void InitBuildingTreeNode(TreeNode buildingGroupNode, Facility.FacilityType facility)
        {
            if (buildingGroupNode == null || buildingGroupNode.Tag == null)
                return;

            BuildingGroup buildingGroup = (BuildingGroup)buildingGroupNode.Tag;

            foreach (KeyValuePair<int, Building> pair in m_dicBuilding)
            {
                Building building = pair.Value;

                if (buildingGroup.ID == building.BuildingGroupID)
                {
                    TreeNode node = new TreeNode(building.DisplayText);
                    node.Tag = building;

                    // 빌딩에 자식 노드 만들기
                    InitZoneTreeNode(node, facility);

                    buildingGroupNode.Nodes.Add(node);
                }
            }
        }

        public void InitZoneTreeNode(TreeNode buildingNode, Facility.FacilityType facility)
        {
            if (buildingNode == null || buildingNode.Tag == null)
                return;

            Building building = (Building)buildingNode.Tag;

            foreach (KeyValuePair<int, Zone> pair in m_dicZone)
            {
                Zone zone = pair.Value;

                if (building.ID == zone.BuildingID)
                {
                    TreeNode node = new TreeNode(zone.DisplayText);
                    node.Tag = zone;

                    // 화재, PSM, ETC 구분하여 트리 작성
                    if (facility == Facility.FacilityType.FIRE_SENSOR || facility == Facility.FacilityType.ETC || facility == Facility.FacilityType.PSM_SENSOR)
                        InitZoneSensorTreeNode(node, facility);
                    //else if (facility == Facility.FacilityType.PSM_SENSOR)
                    //    InitEquipmentZoneTreeNode(node, facility);

                    buildingNode.Nodes.Add(node);
                }
            }
        }

        public void InitZoneSensorTreeNode(TreeNode zoneNode, Facility.FacilityType facility)
        {
            if (zoneNode == null || zoneNode.Tag == null)
                return;

            Zone zone = (Zone)zoneNode.Tag;

            if (facility == Facility.FacilityType.FIRE_SENSOR)
            {
                foreach (KeyValuePair<int, FireSensorData> pair in m_dicFireSensorData)
                {
                    FireSensorData fireSensor = pair.Value;

                    if (zone.ID == fireSensor.ZoneID)
                    {
                        TreeNode node = new TreeNode(fireSensor.Name);
                        node.Tag = fireSensor;

                        zoneNode.Nodes.Add(node);
                    }
                }
            } 
            else if (facility == Facility.FacilityType.ETC)
            {
                foreach (KeyValuePair<int, ETCSensorData> pair in m_dicETCSensorData)
                {
                    ETCSensorData etcSensor = pair.Value;

                    // TODO: 테스트 용도로 임시 타입에 따라 차단 
                    /*if (zone.ID == etcSensor.ZoneID &&
                        (etcSensor.SensorType == (int)Facility.FacilityType.TVOC ||
                        etcSensor.SensorType == (int)Facility.FacilityType.CO2 ||
                        etcSensor.SensorType == (int)Facility.FacilityType.O2))*/
                    if (zone.ID == etcSensor.ZoneID)
                    {
                        TreeNode node = new TreeNode(etcSensor.Name);
                        node.Tag = etcSensor;

                        zoneNode.Nodes.Add(node);
                    }
                }
            }
            else if (facility == Facility.FacilityType.PSM_SENSOR)
            {
                foreach (KeyValuePair<int, PSMSensorData> pair in m_dicPSMSensorData)
                {
                    PSMSensorData psmSensor = pair.Value;

                    if (zone.ID == psmSensor.ZoneID)
                    {
                        TreeNode node = new TreeNode(psmSensor.Name);
                        node.Tag = psmSensor;

                        zoneNode.Nodes.Add(node);
                    }
                }
            }
        }

        //public void InitEquipmentZoneTreeNode(TreeNode zoneNode, Facility.FacilityType facility)
        //{
        //    if (zoneNode == null || zoneNode.Tag == null)
        //        return;

        //    Zone zone = (Zone)zoneNode.Tag;

        //    foreach (KeyValuePair<int, EquipmentZone> pair in m_dicEquipmentZone)
        //    {
        //        EquipmentZone equipmentZone = pair.Value;

        //        if (equipmentZone.ListZoneID.Contains(zone.ID))
        //        {
        //            TreeNode node = new TreeNode(equipmentZone.DisplayText);
        //            node.Tag = equipmentZone;

        //            if (facility == Facility.FacilityType.PSM_SENSOR)
        //            {
        //                InitEquipmentZoneSensorTreeNode(node, facility);
        //            }

        //            zoneNode.Nodes.Add(node);
        //        }
        //    }

        //}

        //public void InitEquipmentZoneSensorTreeNode(TreeNode equipmentZoneNode, Facility.FacilityType facility)
        //{
        //    if (equipmentZoneNode == null || equipmentZoneNode.Tag == null)
        //        return;

        //    EquipmentZone equipmentZone = (EquipmentZone)equipmentZoneNode.Tag;

        //    if (facility == Facility.FacilityType.PSM_SENSOR)
        //    {
        //        foreach (KeyValuePair<int, PSMSensorData> pair in m_dicPSMSensorData)
        //        {
        //            PSMSensorData psmSensor = pair.Value;

        //            if (equipmentZone.ID == psmSensor.EquipZoneID)
        //            {
        //                TreeNode node = new TreeNode(psmSensor.Name);
        //                node.Tag = psmSensor;

        //                equipmentZoneNode.Nodes.Add(node);
        //            }
        //        }
        //    }
        //}

        private bool LoadFireSensor(Dictionary<int, FireSensorData> dicFireSensorData)
        {
            dicFireSensorData.Clear();

            Dictionary<SDMS.Model.Sensor.Fire.Fields, object> dicConditions = new Dictionary<SDMS.Model.Sensor.Fire.Fields, object>();
            string strAdditionalConditions = "";
            string strErrorMessage = "";

            ArrayList arrDatas = m_dataMgr.GetSelectManager().JoinSensorZoneFireSensor(null, null, null, out strErrorMessage);
            if (arrDatas == null)
                return false;

            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] != null && arrDatas[i + 1] != null &&
                    arrDatas[i] is SensorZone && arrDatas[i + 1] is Fire)
                {
                    SensorZone sensorZone = (SensorZone)arrDatas[i];
                    Fire fire = (Fire)arrDatas[i + 1];

                    FireSensorData fireSensor = new FireSensorData();
                    fireSensor.ID = fire.ID;
                    fireSensor.Name = fire.Name;
                    fireSensor.PositionName = fire.PositionName;
                    fireSensor.ZoneID = fire.ZoneID;
                    fireSensor.EquipZoneID = sensorZone.EquipZoneID;

                    dicFireSensorData[fireSensor.ID] = fireSensor;
                }
            }

            //List<SDMS.Model.Sensor.Fire> fireSensors = m_dataMgr.GetSelectManager().SelectFireSensors(dicConditions, strAdditionalConditions, out strErrorMessage);
            //if (fireSensors == null)
            //    return false;

            //foreach (SDMS.Model.Sensor.Fire data in fireSensors)
            //{
            //    FireSensorData fireSensor = new FireSensorData();
            //    fireSensor.ID = data.ID;
            //    fireSensor.Name = data.Name;
            //    fireSensor.PositionName = data.PositionName;
            //    fireSensor.ZoneID = data.ZoneID;

            //    Dictionary<SensorZone.Fields, object> dicConditions_sensorzone = new Dictionary<SensorZone.Fields, object>();
            //    dicConditions_sensorzone[SensorZone.Fields.OrgSensorID] = data.ID;
            //    dicConditions_sensorzone[SensorZone.Fields.SensorType] = (int)Facility.FacilityType.FIRE_SENSOR;
            //    List<SensorZone> sensorZones = m_dataMgr.GetSelectManager().SelectSensorZones(dicConditions_sensorzone, strAdditionalConditions, out strErrorMessage);

            //    if (sensorZones == null)
            //        return false;

            //    if (sensorZones.Count == 0)
            //        continue;

            //    SensorZone sensorZone = sensorZones[0];

            //    fireSensor.EquipZoneID = sensorZone.EquipZoneID;

            //    dicFireSensorData[data.ID] = fireSensor;
            //}

            return true;
        }

        private bool LoadETCSensor(Dictionary<int, ETCSensorData> dicETCSensorData)
        {
            dicETCSensorData.Clear();

            Dictionary<SDMS.Model.Sensor.ETC.Fields, object> dicConditions = new Dictionary<SDMS.Model.Sensor.ETC.Fields, object>();
            string strAdditionalConditions = "";
            string strErrorMessage = "";



            ArrayList arrDatas = m_dataMgr.GetSelectManager().JoinSensorZoneETCSensor(null, null, null, out strErrorMessage);
            if (arrDatas == null)
                return false;

            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] != null && arrDatas[i + 1] != null &&
                    arrDatas[i] is SensorZone && arrDatas[i + 1] is ETC)
                {
                    SensorZone sensorZone = (SensorZone)arrDatas[i];
                    ETC etc = (ETC)arrDatas[i + 1];

                    ETCSensorData etcSensor = new ETCSensorData();
                    etcSensor.ID = etc.ID;

                    string strMaterialType = "기타센서";

                    if (etc.MaterialType != null && m_dicMaterial.ContainsKey((int)etc.MaterialType))
                        strMaterialType = m_dicMaterial[(int)etc.MaterialType].MaterialName;
   
                    etcSensor.Name = etc.Name + "(" + strMaterialType + ")";


                    etcSensor.PositionName = etc.PositionName;
                    etcSensor.ZoneID = etc.ZoneID;
                    etcSensor.MaterialType = etc.MaterialType;
                    etcSensor.EquipZoneID = sensorZone.EquipZoneID;

                    dicETCSensorData[etcSensor.ID] = etcSensor;
                }
            }



            //List<SDMS.Model.Sensor.ETC> etcSensors = m_dataMgr.GetSelectManager().SelectETCSensors(dicConditions, strAdditionalConditions, out strErrorMessage);

            //if (etcSensors == null)
            //    return false;

            //foreach (SDMS.Model.Sensor.ETC data in etcSensors)
            //{
            //    Dictionary<SensorZone.Fields, object> dicCondition = new Dictionary<SensorZone.Fields, object>();
            //    dicCondition.Add(SensorZone.Fields.OrgSensorID, data.ID);

            //    //string strETCType = (int)Facility.FacilityType.FIREWALL + "," + (int)Facility.FacilityType.ETC + "," + (int)Facility.FacilityType.Temp + "," +
            //    //    (int)Facility.FacilityType.Humi + "," + (int)Facility.FacilityType.CO2 + "," + (int)Facility.FacilityType.TVOC + "," +
            //    //    (int)Facility.FacilityType.Dust_PM1 + "," + (int)Facility.FacilityType.Dust_PM2 + "," + (int)Facility.FacilityType.Dust_PM10 + "," +
            //    //    (int)Facility.FacilityType.AirPress + "," + (int)Facility.FacilityType.Inclin_X + "," + (int)Facility.FacilityType.Inclin_Y + "," +
            //    //    (int)Facility.FacilityType.Vib_X + "," + (int)Facility.FacilityType.Vib_Y + "," + (int)Facility.FacilityType.Vib_Z + "," +
            //    //    (int)Facility.FacilityType.Noise + "," + (int)Facility.FacilityType.BLE_Count + "," + (int)Facility.FacilityType.O2 + "," +
            //    //    (int)Facility.FacilityType.Value + "," + (int)Facility.FacilityType.mA + "," + (int)Facility.FacilityType.Contact + "," +
            //    //    (int)Facility.FacilityType.Relay;

            //    //strAdditionalConditions = "SensorType in (" + strETCType + ")";
            //    strAdditionalConditions = "SensorType in (" + string.Join(",", dnsData.Sensor.Facility.GetETCTypeAllNumberToList()) + ")"; 

            //    List<SensorZone> sz = m_dataMgr.GetSelectManager().SelectSensorZones(dicCondition, strAdditionalConditions, out strErrorMessage);

            //    if (sz == null || sz.Count == 0)
            //        return false;

            //    FacilityType facility = GetFacilityType(sz[0].SensorType, out strErrorMessage);

            //    if (facility == null)
            //        return false;

            //    ETCSensorData etcSensor = new ETCSensorData();
            //    etcSensor.ID = data.ID;

            //    //etcSensor.Name = data.Name + "(" + facility.TypeName + ")";
            //    if (data.MaterialType != null && m_dicMaterial.ContainsKey((int)data.MaterialType))
            //    {
            //        string strMaterialType = m_dicMaterial[(int)data.MaterialType].MaterialName;
            //        etcSensor.Name = data.Name + "(" + strMaterialType + ")";
            //    } 
            //    else
            //        etcSensor.Name = data.Name + "(" + facility.TypeName + ")";

            //    etcSensor.SensorType = sz[0].SensorType;
            //    etcSensor.PositionName = data.PositionName;
            //    etcSensor.ZoneID = data.ZoneID;
            //    etcSensor.MaterialType = data.MaterialType;

            //    Dictionary<SensorZone.Fields, object> dicConditions_sensorzone = new Dictionary<SensorZone.Fields, object>();
            //    dicConditions_sensorzone[SensorZone.Fields.OrgSensorID] = data.ID;
            //    dicConditions_sensorzone[SensorZone.Fields.SensorType] = sz[0].SensorType;
            //    List<SensorZone> sensorZones = m_dataMgr.GetSelectManager().SelectSensorZones(dicConditions_sensorzone, strAdditionalConditions, out strErrorMessage);

            //    if (sensorZones == null)
            //        return false;

            //    SensorZone sensorZone = sensorZones[0];
            //    etcSensor.EquipZoneID = sensorZone.EquipZoneID;

            //    dicETCSensorData[data.ID] = etcSensor;
            //}

            return true;
        }



        private FacilityType GetFacilityType(int nID, out string strErrorMessage)
        {
            FacilityType facilityType = null;

            facilityType = m_dataMgr.GetSelectManager().SelectFacilityType(nID, out strErrorMessage);

            return facilityType;
        }

        private bool LoadPSMSensor(Dictionary<int, PSMSensorData> dicPSMSensorData)
        {
            dicPSMSensorData.Clear();

            Dictionary<SDMS.Model.Sensor.PSM.Fields, object> dicConditions = new Dictionary<SDMS.Model.Sensor.PSM.Fields, object>();
            string strAdditionalConditions = "";
            string strErrorMessage;


            ArrayList arrDatas = m_dataMgr.GetSelectManager().JoinSensorZonePSMSensor(null, null, null, out strErrorMessage);
            if (arrDatas == null)
                return false;

            int nDataCount = arrDatas.Count;

            for (int i = 0; i < nDataCount - 1; i += 2)
            {
                if (arrDatas[i] != null && arrDatas[i + 1] != null &&
                    arrDatas[i] is SensorZone && arrDatas[i + 1] is PSM)
                {
                    SensorZone sensorZone = (SensorZone)arrDatas[i];
                    PSM psm = (PSM)arrDatas[i + 1];

                    PSMSensorData psmSensor = new PSMSensorData();
                    psmSensor.ID = psm.ID;

                    string strMaterialType = "누출센서";

                    if (psm.MaterialType != null && m_dicMaterial.ContainsKey((int)psm.MaterialType))
                        strMaterialType = m_dicMaterial[(int)psm.MaterialType].MaterialName;

                    psmSensor.Name = psm.Name + "(" + strMaterialType + ")";


                    psmSensor.PositionName = psm.PositionName;
                    psmSensor.ZoneID = psm.ZoneID;
                    psmSensor.MaterialType = psm.MaterialType;
                    psmSensor.EquipZoneID = sensorZone.EquipZoneID;

                    dicPSMSensorData[psmSensor.ID] = psmSensor;
                }
            }

            //List<SDMS.Model.Sensor.PSM> psmSensors = m_dataMgr.GetSelectManager().SelectPSMSensors(dicConditions, strAdditionalConditions, out strErrorMessage);

            //if (psmSensors == null)
            //    return false;

            //foreach (SDMS.Model.Sensor.PSM data in psmSensors)
            //{
            //    Dictionary<SensorZone.Fields, object> dicCondition = new Dictionary<SensorZone.Fields, object>();
            //    dicCondition.Add(SensorZone.Fields.OrgSensorID, data.ID);

            //    strAdditionalConditions = "SensorType in (" + string.Join(",", dnsData.Sensor.Facility.GetPSMTypeAllNumberToList()) + ")";

            //    List<SensorZone> sz = m_dataMgr.GetSelectManager().SelectSensorZones(dicCondition, strAdditionalConditions, out strErrorMessage);

            //    if (sz == null || sz.Count == 0)
            //        return false;

            //    PSMSensorData psmSensor = new PSMSensorData();
            //    psmSensor.ID = data.ID;

            //    //psmSensor.Name = data.Name + "(" + Facility.GetNFacilityTypeString(sz[0].SensorType) + ")";
            //    if (data.MaterialType != null && m_dicMaterial.ContainsKey((int)data.MaterialType))
            //    {
            //        string strMaterialType = m_dicMaterial[(int)data.MaterialType].MaterialName;
            //        psmSensor.Name = data.Name + "(" + strMaterialType + ")";
            //    }
            //    else
            //        psmSensor.Name = data.Name + "(" + Facility.GetNFacilityTypeString(sz[0].SensorType) + ")";

            //    psmSensor.PositionName = data.PositionName;
            //    psmSensor.ZoneID = data.ZoneID;
            //    psmSensor.EquipZoneID = sz[0].EquipZoneID;
            //    psmSensor.MaterialType = data.MaterialType;

            //    dicPSMSensorData[data.ID] = psmSensor;
            //}

            return true;
        }

        //private Material GetPSMMaterial(int nID, out string strErrorMessage)
        //{
        //    Material psmMaterial = null;

        //    psmMaterial = m_dataMgr.GetSelectManager().SelectPSMMaterial(nID, out strErrorMessage);

        //    return psmMaterial;
        //}

        private bool LoadBuildingGroup(Dictionary<int, BuildingGroup> dicBuildingGroup)
        {
            dicBuildingGroup.Clear();

            Dictionary<SDMS.Model.Spatial.BuildingGroup.Fields, object> dicConditions = new Dictionary<SDMS.Model.Spatial.BuildingGroup.Fields, object>();
            string strAdditionalConditions = "";
            string strErrorMessage = "";

            List<SDMS.Model.Spatial.BuildingGroup> buildingGroups = m_dataMgr.GetSelectManager().SelectBuildingGroups(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (buildingGroups == null)
                return false;

            foreach (SDMS.Model.Spatial.BuildingGroup group in buildingGroups)
            {
                BuildingGroup buildingGroup = new BuildingGroup();
                buildingGroup.ID = group.ID;
                buildingGroup.GroupName = group.GroupName;
                buildingGroup.ParentID = group.ParentID;

                dicBuildingGroup[group.ID] = buildingGroup;
            }

            return true;
        }

        private bool LoadBuilding(Dictionary<int, Building> dicBuilding)
        {
            dicBuilding.Clear();

            Dictionary<SDMS.Model.Spatial.Building.Fields, object> dicConditions = new Dictionary<SDMS.Model.Spatial.Building.Fields, object>();
            string strAdditionalConditions = "";
            string strErrorMessage = "";

            List<SDMS.Model.Spatial.Building> buildings = m_dataMgr.GetSelectManager().SelectBuildings(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (buildings == null)
                return false;

            foreach (SDMS.Model.Spatial.Building data in buildings)
            {
                Building building = new Building();
                building.ID = data.ID;
                building.BuildingName = data.BuildingName;
                building.BuildingGroupID = data.BuildingGroupID;
                building.DisplayText = data.DisplayText;

                dicBuilding[data.ID] = building;
            }

            return true;
        }

        private bool LoadZone(Dictionary<int, Zone> dicZone)
        {
            dicZone.Clear();

            Dictionary<SDMS.Model.Spatial.Zone.Fields, object> dicConditions = new Dictionary<SDMS.Model.Spatial.Zone.Fields, object>();
            string strAdditionalConditions = "";
            string strErrorMessage = "";

            List<SDMS.Model.Spatial.Zone> zones = m_dataMgr.GetSelectManager().SelectZones(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (zones == null)
                return false;

            foreach (SDMS.Model.Spatial.Zone data in zones)
            {
                Zone zone = new Zone();
                zone.ID = data.ID;
                zone.ZoneName = data.ZoneName;
                zone.BuildingID = (data.BuildingID == null) ? -1 : (int)data.BuildingID;
                zone.FloorIndex = (data.FloorIndex == null) ? -1 : (int)data.FloorIndex;
                zone.DisplayText = data.DisplayText;

                dicZone[data.ID] = zone;
            }

            return true;
        }

        private bool LoadEquipmentZone(Dictionary<int, EquipmentZone> dicEquipmentZone)
        {
            dicEquipmentZone.Clear();

            Dictionary<SDMS.Model.Spatial.EquipmentZone.Fields, object> dicConditions = new Dictionary<SDMS.Model.Spatial.EquipmentZone.Fields, object>();
            string strAdditionalConditions = "";
            string strErrorMessage = "";

            List<SDMS.Model.Spatial.EquipmentZone> equipmentZones = m_dataMgr.GetSelectManager().SelectEquipmentZones(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (equipmentZones == null)
                return false;

            foreach (SDMS.Model.Spatial.EquipmentZone data in equipmentZones)
            {
                EquipmentZone equipmentZone = new EquipmentZone();
                equipmentZone.ID = data.ID;
                equipmentZone.ZoneName = data.ZoneName;
                //equipmentZone.LinkedZoneIDList = data.LinkedZoneIDs;
                equipmentZone.DisplayText = data.DisplayText;
                equipmentZone.ListZoneID = data.LinkedZoneIDs;

                dicEquipmentZone[data.ID] = equipmentZone;
            }

            return true;
        }

        private bool LoadMaterial(Dictionary<int, Material> dicMaterial)
        {
            dicMaterial.Clear();

            Dictionary<SDMS.Model.Sensor.Material.Fields, object> dicConditions = new Dictionary<SDMS.Model.Sensor.Material.Fields, object>();
            string strAdditionalConditions = "";
            string strErrorMessage = "";

            List<SDMS.Model.Sensor.Material> materials = m_dataMgr.GetSelectManager().SelectMaterials(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (materials == null)
                return false;

            foreach (SDMS.Model.Sensor.Material data in materials)
            {
                Material material = new Material();
                material.ID = data.ID;
                material.MaterialName = data.MaterialName;
                material.UOM = data.UOM;
                material.Description = data.Description;
                material.SiteID = data.SiteID;

                dicMaterial[material.ID] = material;
            }

            return true;
        }


        public AlarmData GetAlarmData(object sensor)
        {
            if (!(sensor is FireSensorData || sensor is PSMSensorData || sensor is ETCSensorData))
                return null;

            AlarmData alarm = new AlarmData();
            //string strSQL = "";
            int nFacilityType = -1;
            SensorZone sensorZone = null;
            TagInfo tagInfo = null;
            string strErrorMessage = "";

            if (sensor is FireSensorData)
            {
                FireSensorData fireSensor = (FireSensorData)sensor;
                //alarm.URL = CommonString.ALARM_URL_FIRE;
                alarm.URL = m_strAlarm_Fire_URL;
                nFacilityType = (int)Facility.FacilityType.FIRE_SENSOR;

                sensorZone = GetSensorZone(-1, fireSensor.ID, nFacilityType, out strErrorMessage);
                if (sensorZone == null)
                    return null;

                tagInfo = GetTagInfo(sensorZone.ID, out strErrorMessage);
                if (tagInfo == null)
                    return null;
            }
            else if (sensor is PSMSensorData)
            {
                PSMSensorData psmSensor = (PSMSensorData)sensor;
                //alarm.URL = CommonString.ALARM_URL_PSM;
                alarm.URL = m_strAlarm_PSM_URL;

                //sensorZone = GetSensorZone(psmSensor.EquipZoneID, psmSensor.ID, -1, out strErrorMessage);
                sensorZone = GetPSMSensorZone(psmSensor.ID, out strErrorMessage);
                if (sensorZone == null)
                    return null;

                tagInfo = GetTagInfo(sensorZone.ID, out strErrorMessage);
                if (tagInfo == null)
                    return null;

                nFacilityType = sensorZone.SensorType;                
            }
            else if (sensor is ETCSensorData)
            {
                ETCSensorData etcSensor = (ETCSensorData)sensor;
                //alarm.URL = CommonString.ALARM_URL_ETC;
                alarm.URL = m_strAlarm_ETC_URL;
                nFacilityType = etcSensor.SensorType;

                sensorZone = GetSensorZone(-1, etcSensor.ID, etcSensor.SensorType, out strErrorMessage);
                if (sensorZone == null)
                    return null;

                tagInfo = GetTagInfo(sensorZone.ID, out strErrorMessage);
                if (tagInfo == null)
                    return null;
            }

            int nSensorTagInfoID = tagInfo.ID;
            int nSensorZoneID = sensorZone.ID;

            alarm.SensorType = nFacilityType;
            alarm.SensorTagID = nSensorTagInfoID;
            alarm.SensorZoneID = nSensorZoneID;

            return alarm;
        }

        private SensorZone GetPSMSensorZone(int nOrgSensorID, out string strErrorMessage)
        {
            SensorZone sensorZone = null;
            strErrorMessage = "";
            string strAdditionalConditions = "SensorType in (" + string.Join(",", dnsData.Sensor.Facility.GetPSMTypeAllNumberToList()) + ")";

            Dictionary<SensorZone.Fields, object> dicConditions = new Dictionary<SensorZone.Fields, object>();
            dicConditions[SensorZone.Fields.OrgSensorID] = nOrgSensorID;

            List<SensorZone> sensorZones = new List<SensorZone>();
            sensorZones = m_dataMgr.GetSelectManager().SelectSensorZones(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (sensorZones != null && sensorZones.Count > 0)
                sensorZone = sensorZones[0];

            return sensorZone;
        }

        private SensorZone GetSensorZone(int nEquipmentZoneID, int nOrgSensorID, int nFacilityTypeID, out string strErrorMessage)
        {
            SensorZone sensorZone = null;
            string strAdditionalConditions = "";

            Dictionary<SensorZone.Fields, object> dicConditions = new Dictionary<SensorZone.Fields, object>();
            if (nEquipmentZoneID != -1)
                dicConditions.Add(SensorZone.Fields.EquipZoneID, nEquipmentZoneID);

            if (nOrgSensorID != -1)
                dicConditions.Add(SensorZone.Fields.OrgSensorID, nOrgSensorID);

            if (nFacilityTypeID != -1)
                dicConditions.Add(SensorZone.Fields.SensorType, nFacilityTypeID);

            List<SensorZone> sensorZones = new List<SensorZone>();
            sensorZones = m_dataMgr.GetSelectManager().SelectSensorZones(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (sensorZones == null)
                return sensorZone;

            foreach (SensorZone zone in sensorZones)
            {
                sensorZone = zone;
            }

            return sensorZone;
        }

        private SensorZone GetSensorZone(int nID, out string strErrorMessage)
        {
            SensorZone sensorZone = null;

            sensorZone = m_dataMgr.GetSelectManager().SelectSensorZone(nID, out strErrorMessage);

            return sensorZone;
        }

        private TagInfo GetTagInfo(int nSensorZoneID, out string strErrorMessage)
        {
            TagInfo tagInfo = null;
            string strAdditionalConditions = "";

            Dictionary<TagInfo.Fields, object> dicConditions = new Dictionary<TagInfo.Fields, object>();
            dicConditions.Add(TagInfo.Fields.SensorZoneID, nSensorZoneID);

            List<TagInfo> tagInfos = m_dataMgr.GetSelectManager().SelectSensorTagInfo(dicConditions, strAdditionalConditions, out strErrorMessage);

            if (tagInfos == null)
                return tagInfo;

            foreach (TagInfo tag in tagInfos)
            {
                tagInfo = tag;
            }

            return tagInfo;
        }

        public List<AlarmData> GetAlarmList()
        {
            string strErrorMessage = null;
            string strURL = "";
            string strSensorName = "";

            //string strAdditionalConditions = "SensorType in (" + (int)dnsData.Sensor.Facility.FacilityType.PSM_SENSOR + "," + (int)dnsData.Sensor.Facility.FacilityType.ETC + ")";
            string strAdditionalConditions = string.Format("({0}.{1} < {2} or {0}.{1} > {3})", SensorZone.TableName, SensorZone.Fields.SensorType, (int)Facility.FacilityType.Intrusion_S1, (int)Facility.FacilityType.EmergencyBell_S1);

            ArrayList arrDatas = m_dataMgr.GetSelectManager().JoinCurrentAlarmSensorZoneHistorySensorZoneTagInfo(strAdditionalConditions, out strErrorMessage);
            if (arrDatas == null)
                return null;

            int nDataCount = arrDatas.Count;
            List<AlarmData> alarms = new List<AlarmData>();

            for (int i = 0; i < nDataCount - 3; i += 4)
            {
                if (arrDatas[i] != null && arrDatas[i + 1] != null && arrDatas[i + 2] != null && arrDatas[i + 3] != null &&
                    arrDatas[i] is CurrentAlarm && arrDatas[i + 1] is SensorZoneHistory && arrDatas[i + 2] is SensorZone && arrDatas[i + 3] is TagInfo)
                {
                    CurrentAlarm currentAlarm = (CurrentAlarm)arrDatas[i];
                    SensorZoneHistory sensorZoneHistory = (SensorZoneHistory)arrDatas[i + 1];
                    SensorZone sensorZone = (SensorZone)arrDatas[i + 2];
                    TagInfo tagInfo = (TagInfo)arrDatas[i + 3];

                    AlarmData alarm = new AlarmData();
                    alarm.SensorType = sensorZone.SensorType;
                    alarm.SensorTagID = tagInfo.ID;
                    alarm.SensorZoneID = sensorZone.ID;


                    if (sensorZone.SensorType == (int)Facility.FacilityType.FIRE_SENSOR)
                    {
                        strSensorName = m_dicFireSensorData[(int)sensorZone.OrgSensorID].Name + "(화재)";
                        strURL = m_strAlarm_Fire_URL;
                    }
                    else if (sensorZone.SensorType == (int)Facility.FacilityType.PSM_SENSOR)
                    {
                        PSMSensorData psmSensor = m_dicPSMSensorData[(int)sensorZone.OrgSensorID];

                        strSensorName = psmSensor.Name;
                        strURL = m_strAlarm_PSM_URL;
                    }
                    else if (sensorZone.SensorType == (int)Facility.FacilityType.ETC)
                    {
                        ETCSensorData etcSensor = m_dicETCSensorData[(int)sensorZone.OrgSensorID];

                        strSensorName = etcSensor.Name;
                        strURL = m_strAlarm_ETC_URL;
                    }
                    else
                    {
                        continue;
                    }

                    alarm.SensorName = strSensorName;
                    alarm.URL = strURL;

                    alarms.Add(alarm);
                }
            }




            //Dictionary<CurrentAlarm.Fields, object> dicConditions = new Dictionary<CurrentAlarm.Fields, object>();
            //List<CurrentAlarm> currentAlarms = m_dataMgr.GetSelectManager().SelectCurrentAlarms(dicConditions, strAdditionalConditions, out strErrorMessage);

            //if (currentAlarms == null)
            //    return null;

            //List<AlarmData> alarms = new List<AlarmData>();

            //foreach (CurrentAlarm current in currentAlarms)
            //{
            //    SensorZoneHistory zoneHistory = GetSensorZoneHistory(current.SensorZoneHistoryID, out strErrorMessage);
            //    if (zoneHistory == null)
            //        return null;

            //    SensorZone sensorZone = GetSensorZone(zoneHistory.SensorZoneID, out strErrorMessage);
            //    if (sensorZone == null)
            //        return null;

            //    FacilityType facilityType = GetFacilityType(current.SensorType, out strErrorMessage);
            //    if (facilityType == null)
            //        return null;

            //    // SVMS 관련 제외
            //    if (facilityType.ID >= (int)Facility.FacilityType.Intrusion_S1 && facilityType.ID <= (int)Facility.FacilityType.EmergencyBell_S1)
            //        continue;

            //    SDMS.Model.Spatial.EquipmentZone equipment = GetEquipmentZone(sensorZone.EquipZoneID, out strErrorMessage);
            //    if (equipment == null)
            //        return null;

            //    TagInfo tagInfo = GetTagInfo(sensorZone.ID, out strErrorMessage);
            //    if (tagInfo == null)
            //        continue;

            //    string strSensorName = "";

            //    if (Facility.IsFireSensorType((Facility.FacilityType)facilityType.ID))
            //    {
            //        strSensorName = m_dicFireSensorData[(int)sensorZone.OrgSensorID].Name;
            //        //strURL = CommonString.ALARM_URL_FIRE;
            //        strURL = m_strAlarm_Fire_URL;
            //    }
            //    else if (Facility.IsETCSensorType((Facility.FacilityType)facilityType.ID))
            //    {
            //        strSensorName = m_dicETCSensorData[(int)sensorZone.OrgSensorID].Name;
            //        strSensorName = strSensorName + "(" + facilityType.Description + ")";
            //        //strURL = CommonString.ALARM_URL_ETC;
            //        strURL = m_strAlarm_ETC_URL;
            //    }
            //    else if (Facility.IsPSMSensorType((Facility.FacilityType)facilityType.ID))
            //    {
            //        strSensorName = m_dicPSMSensorData[(int)sensorZone.OrgSensorID].Name;

            //        //PSMMaterial material = GetPSMMaterial(m_dicPSMSensorData[sensorZone.OrgSensorID].MaterialType, out strErrorMessage);
            //        //strSensorName = strSensorName + "(" + material.MaterialName + ")";
            //        //strURL = CommonString.ALARM_URL_PSM;
            //        strURL = m_strAlarm_PSM_URL;
            //    }

            //    AlarmData alarm = new AlarmData();
            //    alarm.SensorType = sensorZone.SensorType;
            //    alarm.SensorTagID = tagInfo.ID;
            //    alarm.SensorZoneID = sensorZone.ID;
            //    alarm.SensorName = strSensorName;
            //    alarm.URL = strURL;

            //    alarms.Add(alarm);
            //}

            return alarms;
        }

        private SensorZoneHistory GetSensorZoneHistory(int nID, out string strErrorMessage)
        {
            SensorZoneHistory zoneHistory = null;

            zoneHistory = m_dataMgr.GetSelectManager().SelectSensorZoneHistory(nID, out strErrorMessage);

            return zoneHistory;
        }

        private SDMS.Model.Spatial.EquipmentZone GetEquipmentZone(int nID, out string strErrorMessage)
        {
            SDMS.Model.Spatial.EquipmentZone equipmentZone = null;

            equipmentZone = m_dataMgr.GetSelectManager().SelectEquipmentZone(nID, out strErrorMessage);

            return equipmentZone;
        }

    }
}
