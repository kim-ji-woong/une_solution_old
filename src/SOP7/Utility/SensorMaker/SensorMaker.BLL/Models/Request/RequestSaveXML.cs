using SensorMaker.BLL.Models.Data;
using SensorMaker.BLL.Models.Data.Sensor;
using SensorMaker.BLL.Models.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace SensorMaker.BLL.Models.Request
{
    public class RequestSaveXML
    {
        // 임시 저장 여부
        private bool m_bTempSave = false;
        private int m_nUserID = -1;
        private string m_strUserName = "";
        private string m_strSiteName = "";

        // SensorType
        private List<Basic.SensorType> m_sensorTypes = null;

        // Spatial
        //private List<BuildingGroupData> m_buildingGroups = null;
        private List<ZoneData> m_outdoorZones = new List<ZoneData>();

        // Sensors
        private List<FireSensor> m_fireSensors = null;
        private List<PSMSensor> m_psmSensors = null;
        private List<EtcSensor> m_etcSensors = null;
        private List<CCTVSensor> m_cctvs = null;

        // GltfModel
        private List<GltfModel> m_gltfModels = null;
        private GltfOption m_gltfOption = null;

        public bool bTempSave
        {
            get { return m_bTempSave; }
            set { m_bTempSave = value; }
        }

        public int UserID
        {
            get { return m_nUserID; }
            set { m_nUserID = value; }
        }

        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        public string SiteName
        {
            get { return m_strSiteName; }
            set { m_strSiteName = value; }
        }
        public List<Basic.SensorType> SensorTypes
        {
            get { return m_sensorTypes; }
            set { m_sensorTypes = value; }
        }

        //public List<BuildingGroupData> BuildingGroups
        //{
        //    get { return m_buildingGroups; }
        //    set { m_buildingGroups = value; }
        //}

        public List<ZoneData> OutdoorZones
        {
            get { return m_outdoorZones; }
            set { m_outdoorZones = value; }
        }

        public List<FireSensor> FireSensors
        {
            get { return m_fireSensors; }
            set { m_fireSensors = value; }
        }

        public List<PSMSensor> PSMSensors
        {
            get { return m_psmSensors; }
            set { m_psmSensors = value; }
        }

        public List<EtcSensor> EtcSensors
        {
            get { return m_etcSensors; }
            set { m_etcSensors = value; }
        }

        public List<CCTVSensor> Cctvs
        {
            get { return m_cctvs; }
            set { m_cctvs = value; }
        }

        public List<GltfModel> Models
        {
            get { return m_gltfModels; }
            set { m_gltfModels = value; }
        }

        public GltfOption GltfOption
        {
            get { return m_gltfOption; }
            set { m_gltfOption = value; }
        }






        //
        private List<BuildingGroupData> testBuildingGroupData = null;
        public List<BuildingGroupData> TestBuildingGroupData
        {
            get { return testBuildingGroupData; }
            set { testBuildingGroupData = value; }
        }

        private List<SDMS.Model.Spatial.Building> testBuildingData = null;
        public List<SDMS.Model.Spatial.Building> TestBuildingData
        {
            get { return testBuildingData; }
            set { testBuildingData = value; }
        }

        private List<SDMS.Model.Spatial.Zone> testZoneData = null;
        public List<SDMS.Model.Spatial.Zone> TestZoneData
        {
            get { return testZoneData; }
            set { testZoneData = value; }
        }

        private List<SDMS.Model.Spatial.EquipmentZone> testEquipmentZoneData = null;
        public List<SDMS.Model.Spatial.EquipmentZone> TestEquipmentZoneData
        {
            get { return testEquipmentZoneData; }
            set { testEquipmentZoneData = value; }
        }
    }

    public class BuildingGroupVisibleData : SDMS.Model.Spatial.BuildingGroup
    {
        private bool m_bVisible = true;
        public bool Visible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }
    }
}
